using System;
using KERBALISM;
using RoverScience;

namespace RSCKerbalismED;

/// <summary>
/// Coordinates the experiments analysis process and prepares the
/// resulting sample data for storage through Kerbalism.
/// </summary>
public static class RSCKEAnalysisService
{
    /// <summary>
    /// Processes the current RSC science spot Analysis and stores
    /// the resulting sample in active vessel through Kerbalism.
    /// </summary>
    /// <param name="roverScience">The RSC RoverScience instance performing the analysis.</param>
    public static void ProcessAnalysisConfirmation(RoverScience.RoverScience roverScience, string experimentId )
    {
        RSCKELogger.Info("Experiment " + experimentId + " Analysis starting.");

        // ksp - vessel indicates biome where sample is gathered
        Vessel vesselAnalyzing = RSCKEHub.GetVesselPerformingAnalysis();
        // rsc - partially determines sample mass to be gathered
        ScienceSpot roverScienceSpot = RSCKEHub.GetRSCScienceSpot(roverScience);
        // kerbalism - data necessary for science tracking and storage
        SubjectData kerbSubData = RSCKEHub.FindKerbalismSubjectData(vesselAnalyzing, experimentId);
        // Controller to handle the sample gathered
        RSCKESampleController sampleController = new(vesselAnalyzing, roverScienceSpot, kerbSubData);
        // Store the sample through Kerbalism.
        if (!sampleController.StoreSample())
            throw new InvalidOperationException("[RSCKerbalismED] ERROR: Kerbalism sample storage failed.");

        RSCKELogger.Info("Analysis for Experiment " + experimentId +
         " was completed. Science obtained: " + sampleController.GetSample().ScienceValue);

        // NOTE: Probably have to do things after. Increment samples obtained, etc.
    }
}