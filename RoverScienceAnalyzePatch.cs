using System;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace RSCKerbalismED
{
    [HarmonyPatch]
    internal static class RoverScienceAnalyzePatch
    {
        /// <summary>
        /// Finds the RSC AnalyzeScienceSample method to patch.
        /// </summary>
        /// <returns>The RSC analysis method.</returns>
        private static MethodBase TargetMethod()
        {
            const string RSC_RoverScience_Member = "RoverScience.RoverScience";
            const string RSC_Member_Method = "AnalyzeScienceSample";

            try
            {
                Type roverScienceType = AccessTools.TypeByName(RSC_RoverScience_Member);
                MethodInfo targetMethod = AccessTools.Method(roverScienceType, RSC_Member_Method);
                Debug.Log("[RSCKerbalismED] INFO: Found RSC analysis method: " + targetMethod.DeclaringType.FullName + "." + targetMethod.Name);
                return targetMethod;
            }
            catch (Exception ex)
            {
                Debug.LogError("[RSCKerbalismED] ERROR: Could not properly obtain " + RSC_RoverScience_Member + ".");
                Debug.LogException(ex);
                throw;
            }
        }

        /// <summary>
        /// Replaces RSC's Analyze Science button execution.
        /// Returning false prevents the original RSC method from executing,
        /// since RSC attempts to use Stock's ModuleScienceContainer.
        /// </summary>
        /// <param name="__instance">The RSC RoverScience instance.</param>
        /// <returns>False to suppress the original RSC method.</returns>
        private static bool Prefix(object __instance)
        {
            try
            {
                // Get relevant data from RSC's roverScience instance.
                RSCKEScienceSpot scienceSpot = GetRSCScienceSpot(__instance);

                if (scienceSpot.IsValid)
                    ProcessAnalysis(scienceSpot);
            }
            catch (Exception ex)
            {
                Debug.LogError("[RSCKerbalismED] ERROR: Analysis processing failed.");
                Debug.LogException(ex);
            }

            // Suppress the rest of RSC's gameplay cycle.
            return false;
        }

        /// <summary>
        /// Processes the current RSC science spot and stores the resulting
        /// sample through Kerbalism.
        /// </summary>
        /// <param name="scienceSpot">The current RSC science spot.</param>
        private static void ProcessAnalysis(RSCKEScienceSpot scienceSpot)
        {
            // Get from RSC current data
            string actualPotential = scienceSpot.AdjustedPotentialGenerated;
            Debug.Log("[RSCKerbalismED] INFO: RSC Science Spot Actual Potential: " + actualPotential + ".");

            // Get from KSP (active vessel).
            Vessel vessel = FlightGlobals.ActiveVessel;
            CelestialBody body = vessel?.mainBody;
            string bodyName = body?.name;
            string biomeName = ScienceUtil.GetExperimentBiome(body, vessel.latitude, vessel.longitude);
            Debug.Log("[RSCKerbalismED] INFO: Biome for building Sample (active vessel's) » Body: " +
                bodyName + " | Biome: " + biomeName);

            // Get from RSCKE config, according to RSC current data.
            if (!Plugin.rcskeConfig.TryGetPotentialRange(actualPotential, out double min, out double max))
            {
                string msg = "[RSCKerbalismED] ERROR: Couldnt find a configured ranged for category '" + actualPotential + "'.";
                Debug.LogError(msg);
                throw new InvalidOperationException(msg);
            }
            Debug.Log("[RSCKerbalismED] INFO | RSC Science Spot (final) Potential='" + actualPotential);

            // Get from Kerbalism Data
            KERBALISM.SubjectData subjectData = RSCKEHub.GetKerbalismScienceSubject(vessel, body, biomeName);
            if (subjectData == null)
                throw new InvalidOperationException("[RSCKerbalismED] ERROR: Could not obtain Kerbalism Science Subject.");

            // Get RCSKE calculations
            double massRoll = UnityEngine.Random.Range((float)min, (float)max);
            // massRoll will determine sample mass for each analysis
            massRoll = Math.Round(Math.Max(0.0, Math.Min(1.0, massRoll)), 1);
            Debug.Log("[RSCKerbalismED] INFO | RSC Potential='" + actualPotential +
                "' | Configured Range: " + FormatRangeForLog(min, max) +
                " | RSCKE Mass Roll: " + massRoll.ToString("0.0"));

            // Prepare the sample to store.
            RSCKEScienceSample sample = RoverScienceStoragePatch.PrepareDataToStore(vessel, subjectData, massRoll);

            if (sample == null)
            {
                Debug.Log("[RSCKerbalismED] INFO: No sample data available to store.");
                return;
            }

            // Store the sample through Kerbalism.
            if (!RoverScienceStoragePatch.StoreKerbalismSample(vessel, sample))
                throw new InvalidOperationException("[RSCKerbalismED] ERROR: Kerbalism sample storage failed.");
        }

        /// <summary>
        /// Retrieves RSC's current ScienceSpot from the RoverScience instance.
        /// </summary>
        /// <param name="roverScienceInstance">The RSC RoverScience instance.</param>
        /// <returns>The current RSC science spot.</returns>
        private static RSCKEScienceSpot GetRSCScienceSpot(object roverScienceInstance)
        {
            // Get Rover Member.
            Type roverScienceType = roverScienceInstance.GetType();
            FieldInfo roverField = AccessTools.Field(roverScienceType, "rover");
            object rover = roverField?.GetValue(roverScienceInstance);

            if (rover == null)
                Debug.LogError("[RSCKerbalismED] ERROR: RSC rover instance is null.");

            // Get Rover's Science Spot Member.
            Type roverType = rover.GetType();
            FieldInfo scienceSpotField = AccessTools.Field(roverType, "scienceSpot");
            object scienceSpot = scienceSpotField?.GetValue(rover);

            if (scienceSpot == null)
                Debug.LogError("[RSCKerbalismED] ERROR: RSC scienceSpot instance is null.");

            // Build our object to work with.
            RSCKEScienceSpot returningSpot = new(scienceSpot);
            Debug.Log("[RSCKerbalismED] INFO: Science Spot Data obtained: " + returningSpot + ".");
            return returningSpot;
        }

        /// <summary>
        /// Formats a fractional range as percentages for logging.
        /// </summary>
        /// <param name="min">The minimum fractional value.</param>
        /// <param name="max">The maximum fractional value.</param>
        /// <returns>The formatted percentage range.</returns>
        private static string FormatRangeForLog(double min, double max)
        {
            return (min * 100.0).ToString("0.##") + "%-" + (max * 100.0).ToString("0.##") + "%";
        }

    }
}