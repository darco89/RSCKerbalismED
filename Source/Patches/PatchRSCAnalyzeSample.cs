using System;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace RSCKerbalismED;

/// <summary>
/// Patches RSC's AnalyzeScienceSample() method:
/// RSCKE will handle Sample creation and delegate storage
/// </summary>
[HarmonyPatch]
internal static class PatchRSCAnalyzeSample
{
    // Kerbalism Experiment ID to use in all RSC AnalyzeScienceSample()
    const String RoverExperimentId = RSCKEConstants.ROVER_EXPERIMENT_ID;

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

            Debug.Log("[RSCKerbalismED] INFO: Found RSC AnalyzeScienceSample method: " +
                targetMethod.DeclaringType.FullName + "." + targetMethod.Name);

            return targetMethod;
        }
        catch (Exception ex)
        {
            Debug.LogError("[RSCKerbalismED] ERROR: Could not find RoverScience.AnalyzeScienceSample to intercept");
            Debug.LogException(ex);
            throw;
        }
    }

    /// <summary>
    /// Replaces RSC's "Analyze Science -> confirm" button execution.
    /// Returning false prevents the original RSC method from executing,
    /// thus RSC doesn't use Stock's ModuleScienceContainer in the process.
    /// </summary>
    /// <param name="__instance">The RSC RoverScience instance.</param>
    /// <returns>False to suppress the original RSC method.</returns>
    private static bool Prefix(RoverScience.RoverScience __instance)
    {
        try
        {
            RSCKEAnalysisService.ProcessAnalysisConfirmation(__instance, RoverExperimentId);
        }
        catch (Exception ex)
        {
            Debug.LogError("[RSCKerbalismED] ERROR: Analysis processing failed.");
            Debug.LogException(ex);
            // do not throw.
        }

        // Suppress the original RSC method, to prevent errors (original incompatibility).
        return false;

    }
}