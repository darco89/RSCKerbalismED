using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RSCKerbalismED.Source.Services;
using UnityEngine;

namespace RSCKerbalismED;

/// <summary>
/// Patches RSC's Science Loss due to Reuse GUI field in DrawRoverConsoleGUI().
/// </summary>
internal sealed class RSCKEPatchLabelScienceDecay : IRSCKEPatchTerminalDrawGUI
{
    private const string LabelScienceReuseLoss = "#LOC_RoverScience_GUI_ReuseLoss";

    /// <summary>
    /// Indicates that the RSC Science Loss due to Reuse localization string
    /// has been encountered and its associated GUI label is expected.
    /// </summary>
    private bool reuseLossLabelPending;

    /// <summary>
    /// Indicates whether this patch replaced the Reuse Loss GUI label during transpilation.
    /// </summary>
    public bool Applied { get; private set; }

    /// <summary>
    /// Processes one instruction and suppresses the RSC Science Loss due to Reuse GUI label
    /// when its associated GUILayout.Label() instruction is encountered.
    /// </summary>
    /// <param name="instruction">The instruction to process.</param>
    /// <returns>The original instruction or its RSCKE replacement.</returns>
    public CodeInstruction ProcessInstruction(CodeInstruction instruction)
    {
        if (IsReuseLossInstruction(instruction))
        {
            RSCKELogger.Info("Found RSC Reuse Loss localization key.");
            reuseLossLabelPending = true;
            return instruction;
        }

        if (reuseLossLabelPending && IsReuseLossLabelInstruction(instruction))
        {
            reuseLossLabelPending = false;
            Applied = true;

            RSCKELogger.Info("PatchReuseLoss - Applied.");

            return PatchReuseLossLabel(instruction);
        }

        return instruction;
    }

    /// <summary>
    /// Logs the result of this patch after transpilation.
    /// </summary>
    public void LogResult()
    {
        if (Applied)
            RSCKELogger.Info("PatchRSCTerminalDrawGui - Suppressed Label for Science Reuse Loss.");
    }

    /// <summary>
    /// Determines whether an instruction contains the RSC Science Loss due to Reuse
    /// localization key.
    /// </summary>
    /// <param name="instruction">The instruction to test.</param>
    /// <returns>True if the instruction contains the RSC localization key.</returns>
    private static bool IsReuseLossInstruction(CodeInstruction instruction)
    {
        return instruction.opcode == OpCodes.Ldstr &&
               instruction.operand is string text &&
               text == LabelScienceReuseLoss;
    }

    /// <summary>
    /// Determines whether an instruction calls the GUILayout.Label() used by
    /// the RSC Science Loss due to Reuse GUI field.
    /// </summary>
    /// <param name="instruction">The instruction to test.</param>
    /// <returns>True if the instruction is the expected GUI label call.</returns>
    private static bool IsReuseLossLabelInstruction(CodeInstruction instruction)
    {
        if (instruction.opcode != OpCodes.Call)
            return false;

        if (!(instruction.operand is MethodInfo labelMethod))
            return false;

        if (labelMethod.DeclaringType != typeof(GUILayout))
            return false;

        if (labelMethod.Name != "Label")
            return false;

        ParameterInfo[] parameters = labelMethod.GetParameters();

        if (parameters.Length != 3)
            return false;

        return parameters[0].ParameterType == typeof(string) &&
               parameters[1].ParameterType == typeof(GUIStyle) &&
               parameters[2].ParameterType == typeof(GUILayoutOption[]);
    }

    /// <summary>
    /// Creates the replacement instruction for the RSC Science Loss due to Reuse GUI label.
    /// </summary>
    /// <param name="instruction">The original GUILayout.Label() instruction.</param>
    /// <returns>The replacement instruction.</returns>
    private static CodeInstruction PatchReuseLossLabel(CodeInstruction instruction)
    {
        return new CodeInstruction(
            OpCodes.Call,
            AccessTools.Method(
                typeof(RSCKETerminalService),
                nameof(RSCKETerminalService.HideReuseLossLabel)));
    }
}