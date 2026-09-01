using System;
using UnityEngine;

namespace RSCKerbalismED;

/// <summary>
/// Coordinates the RSC science spot analysis process and prepares the
/// resulting sample data for storage through Kerbalism.
/// </summary>
internal static class RSCKEAnalysisController
{
    /// <summary>
    /// Processes the current RSC science spot Analysis and stores
    /// the resulting sample in active vessel through Kerbalism.
    /// </summary>
    /// <param name="roverScience">The RSC RoverScience instance performing the analysis.</param>
    internal static void ProcessAnalysis(RoverScience.RoverScience roverScience)
    {
        // create sampleData based on current situation and RSC
        RSCKESampleData sampleData = CreateScienceSample(roverScience);
        // Store the sample through Kerbalism.
        if (!RSCKEStorageController.StoreSample(sampleData))
            throw new InvalidOperationException("[RSCKerbalismED] ERROR: Kerbalism sample storage failed.");

        // NOTE: Probably have to do things after. Increment samples obtained, etc.
    }

    /// <summary>
    /// Determines the sample mass (roll) by current Science spot's
    /// AdjustedPotentialGenerated and RSCKE's configured Potential Range.
    /// </summary>
    /// <param name="potential">The RSC ScienceSpot.AdjustedPotentialGenerated »
    /// After normalizing, it should match RSCCATEGORIES_RSCKERANGES keys.</param>
    /// <returns>The calculated sample mass roll.</returns>
    private static double GetMassRoll(string potential)
    {
        // range sets roll interval
        RSCKEPercentageRange range = Plugin.RcskeConfig.GetPotentialRange(potential);
        if (range == null)
        {
            string msg = "[RSCKerbalismED] ERROR: Couldnt find a configured ranged for category '" + potential + "'.";
            Debug.LogError(msg);
            throw new InvalidOperationException(msg);
        }

        // massRoll will determine sample mass for each analysis.
        double massRoll = UnityEngine.Random.Range((float)range.Min, (float)range.Max);
        massRoll = Math.Round(Math.Max(0.0, Math.Min(1.0, massRoll)), 1);
        Debug.Log("[RSCKerbalismED] INFO | RSC AdjustedPotentialGenerated='" + potential +
                "' | Configured Range: " + range.ToPercentageString() +
                " | RSCKE Mass Roll: " + massRoll.ToString("0.0"));

        return massRoll;
    }

    /// <summary>
    /// Creates a RSCKESampleData. Gets RSC and KSP information and also rolls
    /// a concrete mass (in Kg) for the sample being collected, based on
    /// AdjustedPotentialGenerated and RSCKE's configured Potential Range.
    /// </summary>
    /// <param name="roverScience">The RSC RoverScience instance performing the analysis.</param>
    /// <returns>The created RSCKESampleData.</returns>
    private static RSCKESampleData CreateScienceSample(RoverScience.RoverScience roverScience)
    {
        // Vessel gathering sample
        Vessel vessel = RSCKEHub.GetVesselPerformingAnalysis();
        // Start building our sample, from RSC ScienceSpot
        RSCKESampleData sampleData = RSCKEHub.GetRSCScienceSpotDataForSample(roverScience);
        sampleData.ScienceVessel = vessel;
        // SubjectData required to use Kerbalism Storage
        sampleData.SampleSubjectData = RSCKEHub.FindKerbalismSubjectData(vessel);
        if (sampleData.SampleSubjectData != null && vessel != null)
        {
            // Get roll for current analysis' sample mass.
            sampleData.MassRoll = GetMassRoll(sampleData.AdjustedPotentialGenerated);
            // Current Analysis Sample Mass
            sampleData.CollectedMass = sampleData.SampleSubjectData.ExpInfo.SampleMass * sampleData.MassRoll;
            return sampleData;
        }
        throw new InvalidOperationException("[RSCKerbalismED] ERROR: Could not obtain Kerbalism Science Subject.");
    }
}