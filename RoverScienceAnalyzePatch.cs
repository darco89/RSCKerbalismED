using System;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace KerbalismRSC
{
    [HarmonyPatch]
    internal static class RoverScienceAnalyzePatch
    {
        private static MethodBase TargetMethod()
        {
            Type roverScienceType = AccessTools.TypeByName("RoverScience.RoverScience");
            if (roverScienceType == null)
            {
                Debug.LogError("[KerbalismRSC] ERROR: Could not find RoverScience.RoverScience.");
                return null;
            }

            MethodInfo method = AccessTools.Method(roverScienceType, "AnalyzeScienceSample");
            if (method == null)
            {
                Debug.LogError("[KerbalismRSC] ERROR: Could not find AnalyzeScienceSample().");
                return null;
            }

            Debug.Log("[KerbalismRSC] Found RSC analysis method: " + method.DeclaringType.FullName + "." + method.Name);
            return method;
        }

        private static bool Prefix(object __instance)
        {
            if (__instance == null)
            {
                Debug.LogError("[KerbalismRSC] RSC INSTANCE NOT FOUND");
                return false;
            }

            try
            {
                return ProcessAnalysis(__instance);
            }
            catch (Exception ex)
            {
                Debug.LogError("[KerbalismRSC] Analysis processing failed.");
                Debug.LogException(ex);
                return true;
            }
        }

        private static bool ProcessAnalysis(object roverScience)
        {
            object scienceSpot = GetScienceSpot(roverScience);
            if (scienceSpot == null)
            {
                Debug.LogError("[KerbalismRSC] RSC didn't provide a ScienceSpot.");
                return false;
            }

            Type scienceSpotType = scienceSpot.GetType();
            bool isAnomaly = GetIsAnomaly(scienceSpot, scienceSpotType);
            string actualPotential = GetActualPotential(scienceSpot, scienceSpotType);
            LogRSCFieldsForScieceSpot(scienceSpot, scienceSpotType, isAnomaly, actualPotential);

            Vessel vessel = FlightGlobals.ActiveVessel;
            CelestialBody body = vessel?.mainBody;

            string bodyName = body != null ? body.name : "<unknown>";
            string biomeName = GetCurrentBiome();

            CalculateSample(
                bodyName,
                biomeName,
                actualPotential,
                isAnomaly,
                out double sampleMassKg,
                out double finalScience);

            TestKerbalismSubject(vessel, body, biomeName);

            // Suppress RSC's original analysis while we test the Kerbalism integration.
            return false;
        }

        private static void TestKerbalismSubject(
            Vessel vessel,
            CelestialBody body,
            string biomeName)
        {
            Debug.Log("[KerbalismRSC] SUBJECT TEST | Asking Kerbalism for SubjectData.");

            bool subjectFound = KerbalismSampleBridge.TestSubject(
                vessel,
                body,
                biomeName);

            if (!subjectFound)
            {
                Debug.LogError("[KerbalismRSC] SUBJECT TEST | Subject lookup failed.");
            }
            else
            {
                Debug.Log("[KerbalismRSC] SUBJECT TEST | Subject lookup succeeded.");
            }
        }

        private static object GetScienceSpot(object roverScience)
        {
            Type roverScienceType = roverScience.GetType();

            FieldInfo roverField = AccessTools.Field(roverScienceType, "rover");
            object rover = roverField?.GetValue(roverScience);

            if (rover == null)
            {
                Debug.LogError("[KerbalismRSC] ANALYSIS ERROR | RSC rover is null.");
                return null;
            }

            Type roverType = rover.GetType();

            FieldInfo scienceSpotField = AccessTools.Field(roverType, "scienceSpot");
            object scienceSpot = scienceSpotField?.GetValue(rover);

            if (scienceSpot == null)
            {
                Debug.LogError("[KerbalismRSC] ANALYSIS ERROR | RSC scienceSpot is null.");
                return null;
            }

            return scienceSpot;
        }

        private static bool GetIsAnomaly(object scienceSpot, Type scienceSpotType)
        {
            FieldInfo isAnomalyField = AccessTools.Field(scienceSpotType, "isAnomaly");

            return isAnomalyField != null &&
                   Convert.ToBoolean(isAnomalyField.GetValue(scienceSpot));
        }

        private static string GetActualPotential(object scienceSpot, Type scienceSpotType)
        {
            // RSC adjusts the original potential during the gameplay loop.
            // adjustedPotentialGenerated is the final category shown when
            // the player can confirm the sample.
            FieldInfo adjustedPotentialGeneratedField = AccessTools.Field(
                scienceSpotType,
                "adjustedPotentialGenerated");

            object adjustedPotentialGenerated =
                adjustedPotentialGeneratedField?.GetValue(scienceSpot);

            if (adjustedPotentialGenerated == null)
            {
                return "";
            }

            // RSC adds "!" for display purposes (for example, "Very high!").
            // Normalize it so it matches the config Name property.
            return NormalizePotentialName(adjustedPotentialGenerated.ToString());
        }

        private static string NormalizePotentialName(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";

            return value
                .Trim()
                .TrimEnd('!')
                .Trim()
                .ToLowerInvariant();
        }

        // This is the method that will grab the RSC data necessary to create a Kerbalism Sample.
        private static void CalculateSample(
            string bodyName,
            string biomeName,
            string actualPotential,
            bool isAnomaly,
            out double sampleMassKg,
            out double finalScience)
        {
            double fraction;
            string rangeDescription;

            if (isAnomaly)
            {
                if (!SampleConfig.TryGetAnomalyRange(out double anomalyMin, out double anomalyMax))
                {
                    throw new InvalidOperationException("Anomaly range is not configured.");
                }

                fraction = UnityEngine.Random.Range((float)anomalyMin, (float)anomalyMax);
                rangeDescription = FormatRange(anomalyMin, anomalyMax);

                Debug.Log("[KerbalismRSC] ANALYSIS | Anomaly detected.");
            }
            else
            {
                if (!SampleConfig.TryGetPotentialRange(actualPotential, out double normalMin, out double normalMax))
                {
                    throw new InvalidOperationException(
                        "Unknown actual RSC potential category '" + actualPotential + "'.");
                }

                fraction = UnityEngine.Random.Range((float)normalMin, (float)normalMax);
                rangeDescription = FormatRange(normalMin, normalMax);

                Debug.Log("[KerbalismRSC] ANALYSIS | Actual RSC potential = " + actualPotential + ".");
            }

            fraction = Math.Max(0.0, Math.Min(1.0, fraction));

            double experimentMassKg = SampleConfig.GetExperimentMassKg(bodyName, biomeName);
            sampleMassKg = experimentMassKg * fraction;

            double sciencePerKgPercent = SampleConfig.GetSciencePerKgPercent(bodyName);
            double scaledScience = SampleConfig.GetScaledScience();
            double sciencePerKg = sciencePerKgPercent / 100.0;

            finalScience = Math.Round(
                sampleMassKg *
                sciencePerKg *
                (scaledScience / 100.0),
                1);

            Debug.Log("[KerbalismRSC] SAMPLE | Body=" + bodyName +
                " | Biome=" + biomeName +
                " | Anomaly=" + isAnomaly +
                " | ActualPotential=" + actualPotential +
                " | Range=" + rangeDescription +
                " | Fraction=" + fraction.ToString("P2") +
                " | ExperimentMass=" + experimentMassKg + "kg" +
                " | Gathered=" + sampleMassKg + "kg" +
                " | SciencePerKg=" + sciencePerKg.ToString("0.00") +
                " | ScaledScience=" + scaledScience + "%" +
                " | FinalScience=" + finalScience.ToString("0.0"));
        }

        private static string GetCurrentBiome()
        {
            try
            {
                Vessel vessel = FlightGlobals.ActiveVessel;
                if (vessel == null)
                {
                    Debug.LogError("[KerbalismRSC] BIOME | Active vessel is null.");
                    return "<unknown>";
                }

                CelestialBody body = vessel.mainBody;
                if (body == null)
                {
                    Debug.LogError("[KerbalismRSC] BIOME | Active vessel has no main body.");
                    return "<unknown>";
                }

                string biome = ScienceUtil.GetExperimentBiome(body, vessel.latitude, vessel.longitude);
                if (string.IsNullOrEmpty(biome))
                {
                    Debug.LogError("[KerbalismRSC] BIOME | KSP returned an empty biome.");
                    return "<unknown>";
                }

                Debug.Log("[KerbalismRSC] BIOME | KSP active vessel biome = " + biome);
                return biome;
            }
            catch (Exception ex)
            {
                Debug.LogError("[KerbalismRSC] BIOME | Failed to determine active vessel biome.");
                Debug.LogException(ex);
                return "<unknown>";
            }
        }

        private static string FormatRange(double min, double max)
        {
            return (min * 100.0).ToString("0.##") + "%-" + (max * 100.0).ToString("0.##") + "%";
        }

        private static void LogRSCFieldsForScieceSpot(object scienceSpot, Type scienceSpotType, bool isAnomaly, string actualPotential)
        {
            FieldInfo potentialField = AccessTools.Field(scienceSpotType, "potential");
            object potential = potentialField?.GetValue(scienceSpot);
            FieldInfo potentialGeneratedField = AccessTools.Field(scienceSpotType, "potentialGenerated");
            object potentialGenerated = potentialGeneratedField?.GetValue(scienceSpot);
            FieldInfo adjustedPotentialGeneratedField = AccessTools.Field(scienceSpotType, "adjustedPotentialGenerated");
            object adjustedPotentialGenerated = adjustedPotentialGeneratedField?.GetValue(scienceSpot);
            FieldInfo predictedSpotField = AccessTools.Field(scienceSpotType, "predictedSpot");
            object predictedSpot = predictedSpotField?.GetValue(scienceSpot);
            FieldInfo potentialScienceField = AccessTools.Field(scienceSpotType, "potentialScience");
            object potentialScience = potentialScienceField?.GetValue(scienceSpot);

            Debug.Log("[KerbalismRSC] ANALYSIS | RSC POTENTIAL STATE" + " | potential=" +
                (potential != null ? potential.ToString() : "<null>") + " | potentialGenerated=" +
                (potentialGenerated != null ? potentialGenerated.ToString() : "<null>") +
                " | adjustedPotentialGenerated=" + (adjustedPotentialGenerated != null ? adjustedPotentialGenerated.ToString() : "<null>") +
                " | predictedSpot=" + (predictedSpot != null ? predictedSpot.ToString() : "<null>") +
                " | potentialScience=" + (potentialScience != null ? potentialScience.ToString() : "<null>") +
                " | isAnomaly=" + isAnomaly + " | actualPotential=" + actualPotential);
        }
    }
}