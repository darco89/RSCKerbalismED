using System;
using System.Reflection;
using CommNet;
using HarmonyLib;
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

            Debug.Log("[RSCKerbalismED] INFO: Found RSC update method: " +
                targetMethod.DeclaringType.FullName + "." + targetMethod.Name);

            return targetMethod;
        }
        catch (Exception ex)
        {
            Debug.LogError("[RSCKerbalismED] ERROR: Could not properly obtain RoverScience.RoverScience.OnUpdate.");
            Debug.LogException(ex);
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
            CloseTerminalWhenFullControlLost(__instance);
        }
        catch (Exception ex)
        {
            Debug.LogError("[RSCKerbalismED] ERROR: Failed to enforce RSC terminal control state.");
            Debug.LogException(ex);
        }
    }

    /// <summary>
    /// Closes the RSC terminal when there's no longer full
    /// control of the vessel via CommNet.
    /// </summary>
    /// <param name="roverInstance">The RSC RoverScience instance whose terminal should be checked.</param>
    private static void CloseTerminalWhenFullControlLost(RoverScience.RoverScience roverInstance)
    {
        ModuleCommand command = roverInstance.command;
        if (command != null)
        {
            VesselControlState controlState = command.VesselControlState;
            if (controlState != VesselControlState.Full &&
                controlState != VesselControlState.ProbeFull)
            {
                if (roverInstance.IsPrimary
                    && roverInstance.roverScienceGUI?.consoleGUI != null
                    && roverInstance.roverScienceGUI.consoleGUI.isOpen)
                {
                    // TODO: method to "Shutdown" - Resetscience spot, hide terminal, etc..
                    roverInstance.roverScienceGUI.consoleGUI.Hide();
                    Debug.Log("[RSCKerbalismED] INFO: RSC terminal closed because rover control was lost. " +
                        "Control state: " + controlState);
                }
            }
        }
    }
}