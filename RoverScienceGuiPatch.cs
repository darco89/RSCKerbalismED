using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace RSCKerbalismED;

[HarmonyPatch]
internal static class RoverScienceGuiPatch
{
    /// <summary>
    /// Finds the RSC DrawRoverConsoleGUI method to patch.
    /// </summary>
    /// <returns>The RSC GUI method, or null if it cannot be found.</returns>
    private static MethodBase TargetMethod()
    {
        Type guiType = AccessTools.TypeByName("RoverScience.RoverScienceGUI");

        if (guiType == null)
        {
            UnityEngine.Debug.LogError("[RSCKerbalismED] ERROR: Could not find RoverScience.RoverScienceGUI.");
            return null;
        }

        MethodInfo method = AccessTools.Method(guiType, "DrawRoverConsoleGUI");

        if (method == null)
        {
            UnityEngine.Debug.LogError("[RSCKerbalismED] ERROR: Could not find RoverScienceGUI.DrawRoverConsoleGUI.");
            return null;
        }

        UnityEngine.Debug.Log("[RSCKerbalismED] Found RSC GUI method: " + method.DeclaringType.FullName + "." + method.Name);

        return method;
    }

    /// <summary>
    /// Replaces direct ModuleScienceContainer access with safe wrapper methods.
    /// </summary>
    /// <param name="instructions">The original method instructions.</param>
    /// <returns>The patched method instructions.</returns>
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        MethodInfo safeGetStoredDataCount = AccessTools.Method(typeof(RoverScienceGuiPatch), nameof(SafeGetStoredDataCount));
        MethodInfo safeGetCapacity = AccessTools.Method(typeof(RoverScienceGuiPatch), nameof(SafeGetCapacity));

        bool getStoredDataCountPatched = false;
        bool capacityPatched = false;

        foreach (CodeInstruction instruction in instructions)
        {
            if (instruction.opcode == OpCodes.Callvirt &&
                instruction.operand is MethodInfo method &&
                method.Name == "GetStoredDataCount" &&
                method.DeclaringType == typeof(ModuleScienceContainer))
            {
                yield return new CodeInstruction(OpCodes.Call, safeGetStoredDataCount);

                getStoredDataCountPatched = true;
                continue;
            }

            if (instruction.opcode == OpCodes.Ldfld &&
                instruction.operand is FieldInfo field &&
                field.Name == "capacity" &&
                field.DeclaringType == typeof(ModuleScienceContainer))
            {
                yield return new CodeInstruction(OpCodes.Call, safeGetCapacity);

                capacityPatched = true;
                continue;
            }

            yield return instruction;
        }

        if (getStoredDataCountPatched)
        {
            UnityEngine.Debug.Log("[RSCKerbalismED] Patched ModuleScienceContainer.GetStoredDataCount().");
        }
        else
        {
            UnityEngine.Debug.LogError("[RSCKerbalismED] ERROR: Could not find ModuleScienceContainer.GetStoredDataCount().");
        }

        if (capacityPatched)
        {
            UnityEngine.Debug.Log("[RSCKerbalismED] Patched ModuleScienceContainer.capacity.");
        }
        else
        {
            UnityEngine.Debug.LogError("[RSCKerbalismED] ERROR: Could not find ModuleScienceContainer.capacity.");
        }
    }

    /// <summary>
    /// Safely gets the number of stored science data entries.
    /// </summary>
    /// <param name="container">The science container.</param>
    /// <returns>The number of stored entries, or zero if the container is null.</returns>
    private static int SafeGetStoredDataCount(ModuleScienceContainer container)
    {
        if (container == null)
            return 0;

        return container.GetStoredDataCount();
    }

    /// <summary>
    /// Safely gets the science container capacity.
    /// </summary>
    /// <param name="container">The science container.</param>
    /// <returns>The container capacity, or zero if the container is null.</returns>
    private static int SafeGetCapacity(ModuleScienceContainer container)
    {
        if (container == null)
            return 0;

        return container.capacity;
    }
}