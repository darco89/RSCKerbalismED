using System.Reflection.Emit;
using HarmonyLib;

namespace RSCKerbalismED;

/// <summary>
/// Defines a patch that can process instructions from RSC's DrawRoverConsoleGUI() method.
/// </summary>
public interface IRSCKEPatchTerminalDrawGUI
{
    /// <summary>
    /// Indicates whether this patch was applied during the current transpilation.
    /// </summary>
    bool Applied { get; }

    /// <summary>
    /// Processes one instruction and returns the original instruction or its replacement.
    /// </summary>
    /// <param name="instruction">The instruction to process.</param>
    /// <returns>The instruction that should be supplied to Harmony.</returns>
    CodeInstruction ProcessInstruction(CodeInstruction instruction);

    /// <summary>
    /// Logs the result of this patch after transpilation.
    /// </summary>
    void LogResult();
}