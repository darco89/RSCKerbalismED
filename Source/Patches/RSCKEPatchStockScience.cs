using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RSCKerbalismED.Source.Services;
using UnityEngine;

namespace RSCKerbalismED;

/// <summary>
/// Patches RSC's DrawRoverConsoleGUI() references to Stock ModuleScienceContainer
/// data and capacity values with RSCKE-safe runtime wrappers.
/// </summary>
internal sealed class RSCKEPatchStockScience : IRSCKEPatchTerminalDrawGUI
{
    /// <summary>
    /// Indicates that RSC's GetStoredDataCount() access was patched.
    /// </summary>
    private bool dataCountPatched;

    /// <summary>
    /// Indicates that RSC's ModuleScienceContainer.capacity access was patched.
    /// </summary>
    private bool capacityPatched;

    /// <summary>
    /// Indicates whether this patch replaced at least one instruction during transpilation.
    /// </summary>
    public bool Applied => dataCountPatched || capacityPatched;

    /// <summary>
    /// Processes one instruction and replaces matching Stock ModuleScienceContainer references.
    /// </summary>
    /// <param name="instruction">The instruction to process.</param>
    /// <returns>The original instruction or its RSCKE replacement.</returns>
    public CodeInstruction ProcessInstruction(CodeInstruction instruction)
    {
        if (IsGetStoredDataCountInstruction(instruction))
        {
            dataCountPatched = true;
            return PatchGetStoredDataCount(instruction);
        }

        if (IsCapacityInstruction(instruction))
        {
            capacityPatched = true;
            return PatchCapacity(instruction);
        }

        return instruction;
    }

    /// <summary>
    /// Logs the result of this patch after transpilation.
    /// </summary>
    public void LogResult()
    {
        if (dataCountPatched)
            RSCKELogger.Info("PatchRSCTerminalDrawGui - Applied patches for ModuleScienceContainer.data");

        if (capacityPatched)
            RSCKELogger.Info("PatchRSCTerminalDrawGui - Applied patches ModuleScienceContainer.capacity");
    }

    /// <summary>
    /// Determines whether an instruction accesses ModuleScienceContainer.GetStoredDataCount().
    /// </summary>
    /// <param name="instruction">The instruction to test.</param>
    /// <returns>True if the instruction calls GetStoredDataCount().</returns>
    private static bool IsGetStoredDataCountInstruction(CodeInstruction instruction)
    {
        return instruction.opcode == OpCodes.Callvirt &&
               instruction.operand is MethodInfo method &&
               method.Name == "GetStoredDataCount" &&
               method.DeclaringType == typeof(ModuleScienceContainer);
    }

    /// <summary>
    /// Determines whether an instruction accesses ModuleScienceContainer.capacity.
    /// </summary>
    /// <param name="instruction">The instruction to test.</param>
    /// <returns>True if the instruction reads the capacity field.</returns>
    private static bool IsCapacityInstruction(CodeInstruction instruction)
    {
        return instruction.opcode == OpCodes.Ldfld &&
               instruction.operand is FieldInfo field &&
               field.Name == "capacity" &&
               field.DeclaringType == typeof(ModuleScienceContainer);
    }

    /// <summary>
    /// Creates the replacement instruction for ModuleScienceContainer.GetStoredDataCount().
    /// </summary>
    /// <param name="instruction">The original instruction.</param>
    /// <returns>The replacement instruction.</returns>
    private static CodeInstruction PatchGetStoredDataCount(CodeInstruction instruction)
    {
        MethodInfo safeGetStoredDataCount = AccessTools.Method(
            typeof(RSCKETerminalService),
            nameof(RSCKETerminalService.SafeGetStoredDataCount));

        return new CodeInstruction(OpCodes.Call, safeGetStoredDataCount);
    }

    /// <summary>
    /// Creates the replacement instruction for ModuleScienceContainer.capacity.
    /// </summary>
    /// <param name="instruction">The original instruction.</param>
    /// <returns>The replacement instruction.</returns>
    private static CodeInstruction PatchCapacity(CodeInstruction instruction)
    {
        MethodInfo safeGetCapacity = AccessTools.Method(
            typeof(RSCKETerminalService),
            nameof(RSCKETerminalService.SafeGetCapacity));

        return new CodeInstruction(OpCodes.Call, safeGetCapacity);
    }
}