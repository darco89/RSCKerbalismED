using System;
using UnityEngine;

namespace KerbalismRSC
{
    internal static class KerbalismSampleBridge
    {
        // KerbalismRSC experiment ID according to KerbalismRSC_Science.cfg
        private const string ExperimentId = "kerbalismRSC";

        internal static bool TestSubject(Vessel vessel, CelestialBody body, string biomeName)
        {
            if (vessel == null || body == null || string.IsNullOrEmpty(biomeName))
            {
                Debug.LogError("[KerbalismRSC] SUBJECT TEST ERROR | Missing vessel, body, or biome.");
                return false;
            }

            try
            {
                KERBALISM.ExperimentInfo experimentInfo = KERBALISM.ScienceDB.GetExperimentInfo(ExperimentId);
                if (experimentInfo == null)
                {
                    Debug.LogError("[KerbalismRSC] SUBJECT TEST ERROR | Experiment '" + ExperimentId + "' was not found.");
                    return false;
                }

                Debug.Log("[KerbalismRSC] SUBJECT TEST | Experiment found: " + experimentInfo.ExperimentId + " | Title=" + experimentInfo.Title + " | IsSample=" + experimentInfo.IsSample);

                int biomeIndex = FindBiomeIndex(body, biomeName);

                if (biomeIndex < 0)
                {
                    Debug.LogError("[KerbalismRSC] SUBJECT TEST ERROR | Could not find biome index for '" + biomeName + "'.");
                    return false;
                }

                // DAA: 
                // body.flightGlobalsIndex = index of current Body (is currently Duna)
                // RSC Sample is always SrfLanded 
                // BiomeIndex is currently 5 (Midland Sea is biomeName) 
                KERBALISM.Situation situation = new KERBALISM.Situation(body.flightGlobalsIndex, KERBALISM.ScienceSituation.Surface, biomeIndex);
                //Kerbalism returns situtation.Id = 5

                Debug.Log("[KerbalismRSC] SUBJECT TEST | Body=" + body.name + " | Biome=" + biomeName + " | BiomeIndex=" + biomeIndex + " | SituationId=" + situation.Id);

                KERBALISM.SubjectData subjectData = KERBALISM.ScienceDB.GetSubjectData(experimentInfo, situation);
                if (subjectData == null)
                {
                    Debug.LogError("[KerbalismRSC] SUBJECT TEST ERROR | Kerbalism returned no SubjectData.");
                    return false;
                }

                Debug.Log("[KerbalismRSC] SUBJECT TEST SUCCESS | Subject=" + subjectData.Id + " | StockSubject=" + subjectData.StockSubjectId + " | SciencePerMB=" + subjectData.SciencePerMB);
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError("[KerbalismRSC] SUBJECT TEST ERROR | Exception while retrieving SubjectData.");
                Debug.LogException(ex);
                return false;
            }
        }

        private static int FindBiomeIndex(CelestialBody body, string biomeName)
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