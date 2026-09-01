using System;
using UnityEngine;

namespace RSCKerbalismED;

/// <summary>
/// Reads from/Writes to 3rd party APIs (KSP, Kerbalism, RSC).
/// </summary>
internal static class RSCKEHub
{

    /// <summary>
    /// Gets the Kerbalism SubjectData for our experiment
    /// </summary>
    /// <param name="vessel">The vessel containing the sample.</param>
    /// <param name="body">The celestial body where the sample was collected.</param>
    /// <param name="biomeName">The biome where the sample was collected.</param>
    /// <returns>The matching Kerbalism subject, or null if it could not be obtained.</returns>
    internal static KERBALISM.SubjectData GetKerbalismScienceSubjectData(Vessel vessel, CelestialBody body, string biomeName)
    {
        if (vessel == null || body == null || string.IsNullOrEmpty(biomeName))
            return null;
        try
        {
            // KSP data (KSP.CelestialBody.biomemap[biomeName].id)
            int biomeIndex = FindBiomeIndex(body, biomeName);
            if (biomeIndex > 0)
            {
                // Kerbalism
                const String experimentId = RSCKEConfig.RSCKE_EXPERIMENT_ID;
                KERBALISM.ExperimentInfo experimentInfo = KERBALISM.ScienceDB.GetExperimentInfo(experimentId);
                if (experimentInfo != null)
                {
                    KERBALISM.Situation situation = new(body.flightGlobalsIndex, KERBALISM.ScienceSituation.Surface, biomeIndex);
                    return KERBALISM.ScienceDB.GetSubjectData(experimentInfo, situation);
                }
            }
            return null;
        }
        catch (Exception ex)
        {
            Debug.LogError("[RSCKerbalismED] ERROR | Failed to obtain Kerbalism SubjectData.");
            Debug.LogException(ex);
            return null;
        }
    }

    /// <summary>
    /// Resolves the vessel's current biome using KSP and gets
    /// the corresponding Kerbalism SubjectData for the RSCKerbalismED experiment.
    /// </summary>
    /// <param name="vessel">The vessel for which the current biome should be resolved.</param>
    /// <returns>The matching Kerbalism SubjectData, or null if the biome could not be resolved.</returns>
    internal static KERBALISM.SubjectData FindKerbalismSubjectData(Vessel vessel)
    {
        // KSP data: Biome name, by Vessels' current Body (Mun, etc..)
        CelestialBody body = vessel.mainBody;
        double lat = vessel.latitude;
        double lng = vessel.longitude;
        string biomeName = ScienceUtil.GetExperimentBiome(body, lat, lng);

        if (!String.IsNullOrEmpty(biomeName))
        {
            // returns Kerbalism Subject Data
            Debug.Log("[RSCKerbalismED] INFO: Biome for building Sample (active vessel's) » Body: " +
                body.name + " | Biome: " + biomeName);
            return GetKerbalismScienceSubjectData(vessel, body, biomeName);
        }

        Debug.LogError("[RSCKerbalismED] ERROR: Couldn't obtain Kerbalism Subject Data. Invalid Biome Name.");
        return null;
    }

    /// <summary>
    /// Gets the total mass of the specified Kerbalism SubjectData
    /// currently stored across all sample drives on the vessel.
    /// </summary>
    /// <param name="sampleData">The RSCKE sample data containing the vessel and Kerbalism subject to check.</param>
    /// <returns>The total mass already stored for the sample's Kerbalism subject.</returns>
    internal static double GetKerbalismSubjectMassAlreadyStored(RSCKESampleData sampleData)
    {
        double returning = 0.0;
        // Kerbalism
        foreach (KERBALISM.Drive drive in KERBALISM.Drive.GetDrives(sampleData.ScienceVessel))
        {
            if (drive.samples.TryGetValue(sampleData.SampleSubjectData, out KERBALISM.Sample sample))
               returning += sample.mass;
        }


        return returning;
    }

    /// <summary>
    /// Finds the biome index matching the specified biome name.
    /// </summary>
    /// <param name="body">The KSP celestial body containing the biome.</param>
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

    /// <summary>
    /// Retrieves RSC's current ScienceSpot directly from the RoverScience instance
    /// and creates the corresponding RSCKE sample data.
    /// </summary>
    /// <param name="roverScienceInstance">The RSC RoverScience instance performing the analysis.</param>
    /// <returns>The RSCKE sample data created from RSC's current ScienceSpot.</returns>
    internal static RSCKESampleData GetRSCScienceSpotDataForSample(RoverScience.RoverScience roverScienceInstance)
    {
        if (roverScienceInstance == null)
            throw new InvalidOperationException("[RSCKerbalismED] ERROR: RSC RoverScience instance is null.");

        if (roverScienceInstance.rover == null)
            throw new InvalidOperationException("[RSCKerbalismED] ERROR: RSC rover instance is null.");

        if (roverScienceInstance.rover.scienceSpot == null)
            throw new InvalidOperationException("[RSCKerbalismED] ERROR: RSC scienceSpot instance is null.");

        // Build our object directly from RSC's public ScienceSpot instance.
        RSCKESampleData scienceSpotSampleData = new(roverScienceInstance.rover.scienceSpot);
        Debug.Log("[RSCKerbalismED] INFO: RSC Science Spot Data obtained: " + scienceSpotSampleData + ".");
        return scienceSpotSampleData;
    }

    /// <summary>
    /// Finds the first Kerbalism sample drive on the vessel with sufficient
    /// available capacity for the specified sample.
    /// </summary>
    /// <param name="sample">The RSCKE sample requiring storage.</param>
    /// <returns>A Kerbalism Drive with sufficient sample capacity, or null if none is available.</returns>
    internal static KERBALISM.Drive FindAvailableDriveForSample(RSCKESampleData sample)
    {
        KERBALISM.Drive storageDrive = null;
        foreach (KERBALISM.Drive drive in KERBALISM.Drive.GetDrives(sample.ScienceVessel))
        {
            // choose a drive with available slots.
            if (drive.SampleCapacityAvailable(sample.SampleSubjectData) >= sample.DataAmount)
            {
                storageDrive = drive;
                break;
            }
        }
        return storageDrive;
    }

    /// <summary>
    /// Stores the prepared RSCKE sample in an available Kerbalism sample drive.
    /// </summary>
    /// <param name="sample">The prepared RSCKE sample to store.</param>
    /// <returns>True if the sample was successfully recorded by Kerbalism; otherwise false.</returns>
    internal static bool StoreKerbalismExperimentSample(RSCKESampleData sample)
    {
        KERBALISM.Drive drive = FindAvailableDriveForSample(sample);
        if (drive != null)
        {
            // Effectively Store Science Sample 
            bool stored = drive.Record_sample(sample.SampleSubjectData, sample.DataAmount, sample.FinalSampleMass);
            if (stored)
            {
                Debug.Log("[RSCKerbalismED] INFO: Sample Stored in Vessel. " + sample.ToString());
                return true;
            }
            else
            {
                Debug.LogError("[RSCKerbalismED] ERROR: Couldn't Record the Sample via Kerbalism.");
            }
        }

       Debug.LogError("[RSCKerbalismED] ERROR: Didn't find available storage for Sample via Kerbalism.");
       return false;
    }

    /// <summary>
    /// Gets the vessel currently performing the RSCKerbalismED analysis.
    /// </summary>
    /// <returns>The KSP active vessel.</returns>
    // NOTE: Check if we should get Acitve Vessel from RSC, instead of KSP directly
    internal static Vessel GetVesselPerformingAnalysis()
    {
        // KSP
        return FlightGlobals.ActiveVessel;
    }
}