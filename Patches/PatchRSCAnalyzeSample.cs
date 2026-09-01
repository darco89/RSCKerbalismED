using System;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace RSCKerbalismED;

/// <summary>
/// Intercepts RSC's AnalyzeScienceSample()
/// to prevent "Stock Science Storage"
/// </summary>
[HarmonyPatch]
internal static class PatchRSCAnalyzeSample
{
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

            Debug.Log("[RSCKerbalismED] INFO: Found RSC analysis method: " +
                targetMethod.DeclaringType.FullName + "." + targetMethod.Name);

            return targetMethod;
        }
        catch (Exception ex)
        {
            Debug.LogError("[RSCKerbalismED] ERROR: Could not properly obtain RoverScience.RoverScience.AnalyzeScienceSample.");
            Debug.LogException(ex);
            throw;
        }
    }

    /// <summary>
    /// Intercepts RSC's "Analyze Science -> confirm" button execution.
    /// Returning false prevents the original RSC method from executing,
    /// because RSC attempts to use Stock's ModuleScienceContainer.
    /// </summary>
    /// <param name="__instance">The RSC RoverScience instance.</param>
    /// <returns>False to suppress the original RSC method.</returns>
    private static bool Prefix(RoverScience.RoverScience __instance)
    {
        try
        {
            RSCKEAnalysisController.ProcessAnalysis(__instance);
        }
        catch (Exception ex)
        {
            Debug.LogError("[RSCKerbalismED] ERROR: Analysis processing failed.");
            Debug.LogException(ex);
        }

        // Suppress the original RSC method.
        return false;

    }
}