using System;
using System.Reflection;
using CommNet;
using HarmonyLib;
using RSCKerbalismED.Source.Services;
using UnityEngine;

namespace RSCKerbalismED;

/// <summary>
/// Patches RSC's OnUpdate() to close the rover terminal
/// when the rover loses control through CommNet.
/// </summary>
[HarmonyPatch]
internal static class PatchRSCTerminalOnUpdate
{
    /// <summary>
    /// Finds the RSC RoverScience OnUpdate method to patch.
    /// </summary>
    /// <returns>The RSC update method.</returns>
    private static MethodBase TargetMethod()
    {
        try
        {
            MethodInfo targetMethod = AccessTools.Method(
                typeof(RoverScience.RoverScience),
                nameof(RoverScience.RoverScience.OnUpdate));

            RSCKELogger.Info("Found RSC update method: " +
                targetMethod.DeclaringType.FullName + "." + targetMethod.Name);

            return targetMethod;
        }
        catch (Exception ex)
        {
            RSCKELogger.Error("Could not properly obtain RoverScience.RoverScience.OnUpdate.", ex);
            throw;
        }
    }

    /// <summary>
    /// Runs after RSC's RoverScience OnUpdate().
    /// </summary>
    /// <param name="__instance">The RSC RoverScience instance.</param>
    private static void Postfix(RoverScience.RoverScience __instance)
    {
        try
        {
            RSCKETerminalService.CloseTerminalWhenFullControlLost(__instance);
        }
        catch (Exception ex)
        {
            RSCKELogger.Error("Could not close Rover Terminal GUI.", ex);
        }
    }

}