using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace RSCKerbalismED;

/// <summary>
/// Controls the lifecycle of RSCKE patches applied to RSC's DrawRoverConsoleGUI() method.
/// Dispatches each instruction through the individual RSCKE GUI patches and records
/// the patches actually applied during transpilation.
/// </summary>
public class RSCKEPatcherTerminalDrawGUI
{
    /// <summary>
    /// Individual patches applied to RSC's DrawRoverConsoleGUI() instruction stream.
    /// </summary>
    private readonly List<IRSCKEPatchTerminalDrawGUI> patches = new List<IRSCKEPatchTerminalDrawGUI>
    {
        new RSCKEPatchStockScience(),
        new RSCKEPatchLabelScienceDecay()
    };

    /// <summary>
    /// Processes one instruction through all registered RSCKE GUI patches.
    /// </summary>
    /// <param name="instruction">The instruction to process.</param>
    /// <returns>The instruction after all registered patches have processed it.</returns>
    internal CodeInstruction ProcessInstruction(CodeInstruction instruction)
    {
        CodeInstruction patchedInstruction = instruction;

        foreach (IRSCKEPatchTerminalDrawGUI patch in patches)
            patchedInstruction = patch.ProcessInstruction(patchedInstruction);

        return patchedInstruction;
    }

    /// <summary>
    /// Logs the results of all registered patches after transpilation.
    /// </summary>
    internal void LogResults()
    {
        foreach (IRSCKEPatchTerminalDrawGUI patch in patches)
            patch.LogResult();
    }
}