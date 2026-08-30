using System;
using UnityEngine;

namespace RSCKerbalismED
{
    internal static class RoverScienceStoragePatch
    {
        private const string ExperimentId = "RSCKerbalismED";


        /// <summary>
        /// Prepares one RSCKerbalismED sample for storage in Kerbalism.
        /// </summary>
        /// <param name="vessel">The active vessel containing the sample storage.</param>
        /// <param name="subjectData">The Kerbalism subject receiving the sample.</param>
        /// <param name="massRoll">The RSC mass roll used to determine the sample mass.</param>
        /// <returns>The prepared sample, or null if no sample can be stored.</returns>
        internal static RSCKEScienceSample PrepareDataToStore(Vessel vessel, KERBALISM.SubjectData subjectData, double massRoll)
        {
            if (vessel == null || subjectData == null)
                return null;

            KERBALISM.ExperimentInfo experimentInfo = subjectData.ExpInfo;
            double collectedMass = 0.0;

            foreach (KERBALISM.Drive drive in KERBALISM.Drive.GetDrives(vessel))
            {
                if (drive.samples.TryGetValue(subjectData, out KERBALISM.Sample sample))
                    collectedMass += sample.mass;
            }

            double remainingMass = experimentInfo.SampleMass - collectedMass;

            if (remainingMass <= double.Epsilon)
            {
                Debug.Log("[RSCKerbalismED] SAMPLE | Subject has reached its configured sample mass.");
                return null;
            }

            double sampleMass = experimentInfo.SampleMass * massRoll;
            sampleMass = Math.Min(sampleMass, remainingMass);

            if (sampleMass <= double.Epsilon)
                return null;

            double dataAmount = sampleMass / experimentInfo.MassPerMB;

            Debug.Log("[RSCKerbalismED] SAMPLE PREPARED | Mass=" + sampleMass +
                "t | Data=" + dataAmount +
                "MB | ExistingMass=" + collectedMass +
                "t | RemainingMass=" + remainingMass + "t");

            return new RSCKEScienceSample(subjectData, dataAmount, sampleMass);
        }

        /// <summary>
        /// Stores the prepared sample in a Kerbalism sample drive.
        /// </summary>
        /// <param name="vessel">The active vessel receiving the sample.</param>
        /// <param name="sample">The prepared RSCKerbalismED sample.</param>
        /// <returns>True if the sample was stored successfully; otherwise false.</returns>
        internal static bool StoreKerbalismSample(Vessel vessel, RSCKEScienceSample sample)
        {
            if (vessel == null || sample == null)
                return false;

            KERBALISM.Drive storageDrive = null;

            foreach (KERBALISM.Drive drive in KERBALISM.Drive.GetDrives(vessel))
            {
                if (drive.SampleCapacityAvailable(sample.SubjectData) >= sample.DataAmount)
                {
                    storageDrive = drive;
                    break;
                }
            }

            if (storageDrive == null)
            {
                Debug.LogError("[RSCKerbalismED] SAMPLE ERROR | No Kerbalism sample storage available.");
                return false;
            }

            bool stored = storageDrive.Record_sample(sample.SubjectData, sample.DataAmount, sample.Mass);

            if (!stored)
            {
                Debug.LogError("[RSCKerbalismED] SAMPLE ERROR | Kerbalism could not store the sample.");
                return false;
            }

            Debug.Log("[RSCKerbalismED] SAMPLE STORED | Subject=" + sample.SubjectData.Id +
                " | Mass=" + sample.Mass +
                "t | Data=" + sample.DataAmount + "MB");

            return true;
        }
    }
}