using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace RSCKerbalismED;

/// <summary>
/// Harmony patch for RSC's DrawRoverConsoleGUI() method.
/// Provides the Harmony-facing transpiler and delegates all patch logic to RSCKEPatchTerminalDrawGui.
/// </summary>
[HarmonyPatch]
internal static class PatchRSCTerminalDrawGui
{
    private const string rscType = "RoverScience.RoverScienceGUI";
    private const string rscMethod = "DrawRoverConsoleGUI";

    /// <summary>
    /// Finds the RSC DrawRoverConsoleGUI method to patch.
    /// </summary>
    /// <returns>The RSC GUI method, or null if it cannot be found.</returns>
    private static MethodBase TargetMethod()
    {
        Type guiType = AccessTools.TypeByName(rscType);
        if (guiType == null)
        {
            RSCKELogger.Error("PatchRSCTerminalDrawGui - Could not find RoverScience.RoverScienceGUI.");
            return null;
        }

        MethodInfo method = AccessTools.Method(guiType, rscMethod);
        if (method == null)
        {
            RSCKELogger.Error("PatchRSCTerminalDrawGui - Could not find RoverScienceGUI.DrawRoverConsoleGUI.");
            return null;
        }

        RSCKELogger.Info("Found RSC GUI method: " + method.DeclaringType.FullName + "." + method.Name);
        return method;
    }

    /// <summary>
    /// Passes each RSC GUI instruction through a single RSCKE patching session.
    /// </summary>
    /// <param name="instructions">The original method instructions.</param>
    /// <returns>The patched method instructions.</returns>
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        RSCKEPatcherTerminalDrawGUI patcher = new RSCKEPatcherTerminalDrawGUI();

        foreach (CodeInstruction instruction in instructions)
            yield return patcher.ProcessInstruction(instruction);

        patcher.LogResults();
    }
}