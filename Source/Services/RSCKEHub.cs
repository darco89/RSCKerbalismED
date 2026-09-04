using System;
using RoverScience;


namespace RSCKerbalismED;

/// <summary>
/// Reads from/Writes to 3rd party APIs (KSP, Kerbalism, RSC).
/// </summary>
public static class RSCKEHub
{
    /// <summary>
    /// Gets the Kerbalism SubjectData for our experiment and situation
    /// </summary>
    /// <param name="body">The celestial body where the sample was collected.</param>
    /// <param name="biomeName">The biome where the sample was collected.</param>
    /// <param name="experimentId">The experiment id, subject of analysis.</param>
    /// <returns>The matching Kerbalism subject, or null if it could not be obtained.</returns>
    public static KERBALISM.SubjectData GetKerbalismSubjectData(CelestialBody body, string biomeName, string experimentId)
    {
        try
        {
            // KSP data (KSP.CelestialBody.biomemap[biomeName].id)
            int biomeIndex = GetBiomeIndex(body, biomeName);
            if (biomeIndex >= 0)
            {
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
            RSCKELogger.Error("Failed to obtain Kerbalism SubjectData.", ex);
            return null;
        }
    }

    /// <summary>
    /// Gets the KSP biome at the vessel's current position.
    /// </summary>
    /// <param name="vessel">The vessel whose current biome should be resolved.</param>
    /// <returns>The biome name at the vessel's current position.</returns>
    public static string GetKSPBiomeByVessel(Vessel vessel)
    {
        // KSP data: Biome name, by Vessels' current Body (Mun, etc..)
        double lat = vessel.latitude;
        double lng = vessel.longitude;
        string biomeName = ScienceUtil.GetExperimentBiome(vessel.mainBody, lat, lng);
        return biomeName;
    }

    /// <summary>
    /// Resolves the vessel's current biome using KSP and gets the
    /// corresponding Kerbalism SubjectData for the RSCKE experiment.
    /// </summary>
    /// <param name="vessel">The vessel for which the current biome should be resolved.</param>
    /// <param name="experimentId">The Experiment id for the Kerbalism subject.</param>
    /// <returns>The Kerbalism SubjectData. Returns null if not discovered.</returns>
    public static KERBALISM.SubjectData FindKerbalismSubjectData(Vessel vessel, string experimentId)
    {
        string biomeName = GetKSPBiomeByVessel(vessel);
        if (!String.IsNullOrEmpty(biomeName))
        {
            // returns Kerbalism Subject Data
            RSCKELogger.Info("Biome for building Sample (active vessel's) » Body: " +
                vessel.mainBody.name + " | Biome: " + biomeName);
            return GetKerbalismSubjectData(vessel.mainBody, biomeName, experimentId);
        }

        RSCKELogger.Error("Couldn't obtain Kerbalism Subject Data. Invalid Biome Name.");
        return null;
    }

    /// <summary>
    /// Gets the total mass of the specified Kerbalism SubjectData
    /// currently stored across all sample drives on the vessel.
    /// </summary>
    /// <param name="sampleData">The RSCKE sample data containing the vessel and Kerbalism subject to check.</param>
    /// <returns>The total mass already stored for the sample's Kerbalism subject.</returns>
    public static double GetKerbalismSubjectTotalMassStored(RSCKESampleData sampleData)
    {
        double totalMassStored = 0.0;
        foreach (KERBALISM.Drive drive in KERBALISM.Drive.GetDrives(sampleData.ScienceVessel))
        {
            if (drive.samples.TryGetValue(sampleData.SampleSubjectData, out KERBALISM.Sample sample))
               totalMassStored += sample.mass;
        }

        RSCKELogger.Info("Current vessel has a total of " + totalMassStored + "Kg for current Subject.");
        return totalMassStored;
    }

    /// <summary>
    /// Finds the biome index matching the specified biome name.
    /// </summary>
    /// <param name="body">The KSP celestial body containing the biome.</param>
    /// <param name="biomeName">The biome name to find.</param>
    /// <returns>The biome index if found; otherwise -1.</returns>
    public static int GetBiomeIndex(CelestialBody body, string biomeName)
    {
        int biomeIdx = -1;
        for (int i = 0; i < body.BiomeMap.Attributes.Length; i++)
        {
            if (string.Equals(body.BiomeMap.Attributes[i].name, biomeName, StringComparison.OrdinalIgnoreCase))
            {
                biomeIdx = i;
                RSCKELogger.Info("Biome index for " + biomeName + " is "+ biomeIdx);
                break;
            }
        }
        return biomeIdx;
    }

    /// <summary>
    /// Finds the first Kerbalism sample drive on the vessel with sufficient
    /// available capacity for the specified sample.
    /// </summary>
    /// <param name="sample">The RSCKE sample requiring storage.</param>
    /// <returns>A Kerbalism Drive with sufficient sample capacity, or null if none is available.</returns>
    public static KERBALISM.Drive FindAvailableDriveForSample(RSCKESampleData sample)
    {
        KERBALISM.Drive storageDrive = null;
        foreach (KERBALISM.Drive drive in KERBALISM.Drive.GetDrives(sample.ScienceVessel))
        {
            // choose a drive with available slots.
            if (drive.SampleCapacityAvailable(sample.SampleSubjectData) >= sample.DataAmount)
            {
                storageDrive = drive;
                RSCKELogger.Info("Drive considered to store sample: " + storageDrive.name);
                break;
            }
        }
        return storageDrive;
    }

    /// <summary>
    /// Stores the prepared RSCKE sample in an available Kerbalism sample drive.
    /// </summary>
    /// <param name="sample">The prepared RSCKE sample to store.</param>
    /// <returns>Kerbalism drive name where sample is recorded; otherwise Empty.</returns>
    public static string StoreKerbalismExperimentSample(RSCKESampleData sample)
    {
        // Get drive to store sample at
        KERBALISM.Drive drive = FindAvailableDriveForSample(sample);
        if (drive != null)
        {
            // Effectively Store Science Sample 
            bool stored = drive.Record_sample(sample.SampleSubjectData, sample.DataAmount, sample.MassToStore);
            if (stored)
            {
                RSCKELogger.Info("Sample Stored: " + sample.GetFormattedLog());
                return drive.name;
            }
            else
            {
                RSCKELogger.Error("Couldn't Record the Sample via Kerbalism.");
            }
        }
        else
        {
            RSCKELogger.Error("Didn't find available storage for Sample via Kerbalism.");
        }

        return String.Empty;
    }

    /// <summary>
    /// Gets the vessel currently performing the RSCKerbalismED analysis.
    /// </summary>
    /// <returns>The KSP active vessel.</returns>
    // NOTE: Check if we should get Active Vessel from RSC, instead of KSP directly
    public static Vessel GetVesselPerformingAnalysis()
    {
        return FlightGlobals.ActiveVessel;
    }

    /// <summary>
    /// Gets the current RSC Science Spot associated with the specified RoverScience instance.
    /// </summary>
    /// <param name="roverScience">The RSC RoverScience instance performing the analysis.</param>
    /// <returns>The current RSC Science Spot.</returns>
    public static ScienceSpot GetRSCScienceSpot(RoverScience.RoverScience roverScience)
    {
        // RSC scienceSpot is a required input for the analysis workflow.
        // If it cannot be accessed, let an exception be thrown.
        if (roverScience == null || roverScience.rover == null || roverScience.rover.scienceSpot == null)
        {
            const string msg = "RSC ScienceSpot instance is null.";
            RSCKELogger.Error(msg);
            throw new InvalidOperationException(msg);
        }

        return roverScience.rover.scienceSpot;
    }
}