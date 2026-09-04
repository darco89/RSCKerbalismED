using System;
using RoverScience;

namespace RSCKerbalismED;

/// <summary>
/// To contain all the Sample's lifecycle information
/// Just a DTO class. Might be a bit sloppy atm
/// </summary>
public class RSCKESampleData
{
    // Get from RSC
    public string AdjustedPotentialGenerated { get; internal set; } = String.Empty; // Defines Roll range

    // Get from KSP
    public Vessel ScienceVessel { get; internal set; } = null; // For situation and storage

    // Get from KERBALISM
    public KERBALISM.SubjectData SampleSubjectData { get; internal set; } = null; // Experiment Info

    // RSCKE calculates sample mass
    public double MassRoll { get; internal set; } = 0.0; // Roll for sample volume
    public double CollectedMass { get; internal set; } = 0.0; // in tons (1 = 1000kg)

    // Final sample values
    public double MassToStore { get; internal set; } = 0.0; // Mass to store (Kg)
    public double DataAmount { get; internal set; } = 0.0; // (MB)
    public double ScienceValue { get; internal set; } = 0.0; // KSP Resource
    public string StorageDriveName { get; internal set; } = String.Empty; // Kerbalism Drive

    /// <summary>
    /// The Sample always originates from an RSC Science Spot. However,
    /// only its adjustedPotentialGenerated property matters for RSCKE calculation
    /// </summary>
    /// <param name="scienceSpot">The RSC ScienceSpot where Analysis takes place.</param>
    public RSCKESampleData(ScienceSpot scienceSpot)
    {
        AdjustedPotentialGenerated = scienceSpot?.adjustedPotentialGenerated;
        if (AdjustedPotentialGenerated == "anomaly")
            RSCKELogger.Info("Creating sample at an RSC science spot from an " + AdjustedPotentialGenerated+"!");
        else
            RSCKELogger.Info("Creating sample at an RSC science spot with a " + AdjustedPotentialGenerated + " scientific value!");
    }

    /// <summary>
    /// Returns the sample data formatted for logging.
    /// </summary>
    /// <returns>A formatted representation of the sample data for logging.</returns>
    public string GetFormattedLog()
    {
        return
            " | Vessel conducting Analysis: " + (ScienceVessel?.vesselName ?? "<null>") +
            " | Biome where Analysis Occurred: " + (SampleSubjectData?.BiomeTitle ?? "<null>") +
            " | Experiment Title: " + (SampleSubjectData?.ExperimentTitle ?? "<null>") +
            " | Experiment Mass total to gather: " + (SampleSubjectData?.ExpInfo?.SampleMass.ToString() ?? "<null>") +
            " | Experiment Science total to obtain: " + SampleSubjectData?.ScienceMaxValue +
            " | RSC Site Potential: " + (AdjustedPotentialGenerated ?? "<null>") +
            " | RSCKE Roll for Sample Mass: " + MassRoll +
            " | Analysis Mass Collected : " + CollectedMass + "Kg " +
            " | Sample Mass to be Stored: " + MassToStore + "Kg " +
            " | Sample Data Amount (Kerbalism):" + DataAmount + "MB "+
            " | Stored in Kerbalism Drive: " + StorageDriveName +
            " | Sample Final Science Value:" + ScienceValue;
    }
}