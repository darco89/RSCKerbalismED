using System;
using RoverScience;

namespace RSCKerbalismED;

/// <summary>
/// Contains all data regarding the Sample, from gathering to Storage.
/// </summary>
internal sealed class RSCKESampleData
{
    // RSC
    internal string AdjustedPotentialGenerated { get; } = String.Empty;// defines roll range
    // KSP
    internal Vessel ScienceVessel { get; set; } = null; // for situation and storage
    // KERBALISM
    internal KERBALISM.SubjectData SampleSubjectData { get; set; } = null;
    // RSCKE
    internal double MassRoll { get; set; } = 0.0; // roll for sample volume
    public double CollectedMass { get; set; } // in tons (1 = 1000kg)
    // Results
    internal double FinalSampleMass {get; set;} = 0.0;
    internal double DataAmount { get; set; }
    internal double ScienceValue {get; set; }


    /// <summary>
    /// The Sample always originates from a Science Spot
    /// </summary>
    /// <param name="scienceSpot">The RSC ScienceSpot where Analysis takes place.</param>
    internal RSCKESampleData(ScienceSpot scienceSpot)
    {
        AdjustedPotentialGenerated = scienceSpot?.adjustedPotentialGenerated;
    }

    /// <summary>
    /// Returns a readable representation of the sample data.
    /// </summary>
    /// <returns>A formatted representation of the sample data.</returns>
    public override string ToString()
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
            " | Sample Mass to be Stored: " + FinalSampleMass + "Kg " +
            " | Sample Data Amount to be Stored:" + DataAmount +
            " | Sample Final Science Value:" + ScienceValue;
    }
}