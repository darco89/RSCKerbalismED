using System;
using UnityEngine;

namespace RSCKerbalismED
{
    internal static class RSCKEHub
    {
        private const string ExperimentId = "RSCKerbalismED";

        /// <summary>
        /// Gets the Kerbalism subject for the specified vessel, body, and biome.
        /// </summary>
        /// <param name="vessel">The vessel containing the sample.</param>
        /// <param name="body">The celestial body where the sample was collected.</param>
        /// <param name="biomeName">The biome where the sample was collected.</param>
        /// <returns>The matching Kerbalism subject, or null if it could not be obtained.</returns>
        internal static KERBALISM.SubjectData GetKerbalismScienceSubject(Vessel vessel, CelestialBody body, string biomeName)
        {
            if (vessel == null || body == null || string.IsNullOrEmpty(biomeName))
                return null;
            try
            {
                KERBALISM.ExperimentInfo experimentInfo = KERBALISM.ScienceDB.GetExperimentInfo(ExperimentId);
                if (experimentInfo == null)
                    return null;

                int biomeIndex = FindBiomeIndex(body, biomeName);
                if (biomeIndex < 0)
                    return null;

                KERBALISM.Situation situation = new(body.flightGlobalsIndex, KERBALISM.ScienceSituation.Surface, biomeIndex);
                return KERBALISM.ScienceDB.GetSubjectData(experimentInfo, situation);
            }
            catch (Exception ex)
            {
                Debug.LogError("[RSCKerbalismED] ERROR | Failed to obtain Kerbalism SubjectData.");
                Debug.LogException(ex);
                return null;
            }
        }

        /// <summary>
        /// Finds the biome index matching the specified biome name.
        /// </summary>
        /// <param name="body">The celestial body containing the biome.</param>
        /// <param name="biomeName">The biome name to find.</param>
        /// <returns>The biome index if found; otherwise -1.</returns>
        internal static int FindBiomeIndex(CelestialBody body, string biomeName)
        {
            if (body == null || body.BiomeMap == null || string.IsNullOrEmpty(biomeName))
                return -1;

            for (int i = 0; i < body.BiomeMap.Attributes.Length; i++)
            {
                if (string.Equals(body.BiomeMap.Attributes[i].name, biomeName, StringComparison.OrdinalIgnoreCase))
                    return i;
            }

            return -1;
        }
    }
}