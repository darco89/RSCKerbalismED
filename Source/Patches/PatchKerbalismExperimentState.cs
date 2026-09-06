using HarmonyLib;
using RSCKerbalismED.Source.Services;

namespace RSCKerbalismED;

/// <summary>
/// Patches Kerbalism's Experiment.State setter so the RSC Rover Terminal
/// follows manual changes to the RSCKerbalismED experiment state.
/// </summary>
[HarmonyPatch(typeof(KERBALISM.Experiment))]
internal static class PatchKerbalismExperimentState
{
    [HarmonyPatch("set_State")]
    [HarmonyPostfix]
    private static void Postfix(KERBALISM.Experiment __instance, KERBALISM.Experiment.RunningState value)
    {
        if (__instance == null || __instance.experiment_id != RSCKEConstants.ROVER_EXPERIMENT_ID)
            return;

        RSCKELogger.Info("RSCKE Experiment.State changed. " +
            "Experiment instance ID=" + __instance.GetInstanceID() +
            ", State=" + value +
            ", Running=" + __instance.Running + ".");

        RSCKETerminalService.SyncRSCConsoleToKerbalismState(__instance);
    }
}