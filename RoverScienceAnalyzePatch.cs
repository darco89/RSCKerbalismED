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
            try
            {
                MethodInfo targetMethod = AccessTools.Method(
                    typeof(RoverScience.RoverScience),
                    nameof(RoverScience.RoverScience.AnalyzeScienceSample));

                Debug.Log("[RSCKerbalismED] INFO: Found RSC analysis method: " +
                    targetMethod.DeclaringType.FullName + "." + targetMethod.Name);

                return targetMethod;
            }
            catch (Exception ex)
            {
                Debug.LogError("[RSCKerbalismED] ERROR: Could not properly obtain RoverScience.RoverScience.AnalyzeScienceSample.");
                Debug.LogException(ex);
                throw;
            }
        }

        /// <summary>
        /// Intercepts RSC's "Analyze Science -> confirm" button execution.
        /// Returning false prevents the original RSC method from executing,
        /// since RSC attempts to use Stock's ModuleScienceContainer.
        /// </summary>
        /// <param name="__instance">The RSC RoverScience instance.</param>
        /// <returns>False to suppress the original RSC method.</returns>
        private static bool Prefix(RoverScience.RoverScience __instance)
        {
            try
            {
                // Get scienceSpot data from RSC
                RSCKEScienceSpot scienceSpot = RSCKEHub.GetRSCScienceSpot(__instance);
                if (scienceSpot.IsValid)
                    ProcessAnalysis(scienceSpot);
            }
            catch (Exception ex)
            {
                Debug.LogError("[RSCKerbalismED] ERROR: Analysis processing failed.");
                Debug.LogException(ex);
            }

            // Suppress the original RSC method.
            return false;
        }

        /// <summary>
        /// Processes the current RSC science spot and stores the resulting
        /// sample through Kerbalism.
        /// </summary>
        /// <param name="scienceSpot">The current RSC science spot.</param>
        private static void ProcessAnalysis(RSCKEScienceSpot scienceSpot)
        {
            // Get RSC data and calculate the current analysis' sample mass.
            double massRoll = GetMassRoll(scienceSpot.AdjustedPotentialGenerated);

            // Get KSP data (active vessel) for sample storage and biome identification.
            Vessel vessel = FlightGlobals.ActiveVessel;
            CelestialBody body = vessel?.mainBody;
            string biomeName = ScienceUtil.GetExperimentBiome(body, vessel.latitude, vessel.longitude);

            Debug.Log("[RSCKerbalismED] INFO: Biome for building Sample (active vessel's) » Body: " +
                body?.name + " | Biome: " + biomeName);

            // Get Kerbalism data based on the active vessel's situation.
            KERBALISM.SubjectData subjectData = RSCKEHub.GetKerbalismScienceSubject(vessel, body, biomeName);
            if (subjectData == null)
                throw new InvalidOperationException("[RSCKerbalismED] ERROR: Could not obtain Kerbalism Science Subject.");

            // Prepare and store the sample through the storage controller.
            if (!RSCKEStorageController.StoreSample(vessel, subjectData, massRoll))
                throw new InvalidOperationException("[RSCKerbalismED] ERROR: Kerbalism sample storage failed.");
        }

        /// <summary>
        /// Determines the sample mass roll from the current RSC science spot
        /// potential and the configured RSC potential range.
        /// </summary>
        /// <param name="potential">The RSC potential category.</param>
        /// <returns>The calculated sample mass roll.</returns>
        private static double GetMassRoll(string potential)
        {
            Debug.Log("[RSCKerbalismED] INFO: RSC Science Spot (final) Potential='" + potential);

            // Get from RSCKE config, according to RSC current data.
            RSCKEPercentageRange range = Plugin.rcskeConfig.GetPotentialRange(potential);

            if (range == null)
            {
                string msg = "[RSCKerbalismED] ERROR: Couldnt find a configured ranged for category '" + potential + "'.";
                Debug.LogError(msg);
                throw new InvalidOperationException(msg);
            }

            // massRoll will determine sample mass for each analysis.
            double massRoll = UnityEngine.Random.Range((float)range.Min, (float)range.Max);
            massRoll = Math.Round(Math.Max(0.0, Math.Min(1.0, massRoll)), 1);

            Debug.Log("[RSCKerbalismED] INFO | RSC Potential='" + potential +
                "' | Configured Range: " + range.ToPercentageString() +
                " | RSCKE Mass Roll: " + massRoll.ToString("0.0"));

            return massRoll;
        }
    }
}