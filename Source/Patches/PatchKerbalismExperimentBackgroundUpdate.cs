using HarmonyLib;

namespace RSCKerbalismED;

/// <summary>
/// Prevents the RSCKerbalismED Kerbalism experiment from running in the
/// background simulation when its RSC terminal is not available.
/// </summary>
[HarmonyPatch(typeof(KERBALISM.Experiment))]
internal static class PatchKerbalismExperimentBackgroundUpdate
{
    [HarmonyPatch("BackgroundUpdate")]
    [HarmonyPrefix]
    private static bool Prefix(KERBALISM.Experiment __instance, ProtoPartModuleSnapshot m)
    {
        if (__instance == null || __instance.experiment_id != RSCKEConstants.ROVER_EXPERIMENT_ID)
            return true;

        KERBALISM.Experiment.RunningState expState = KERBALISM.Lib.Proto.GetEnum(m, RSCKEConstants.KERBALISM_PROTO_EXP_STATE, KERBALISM.Experiment.RunningState.Stopped);

        // RSCKE does not support background operation because the RSC Terminal
        // is only available while the RoverScience vessel is actively running.
        // Force the background proto state to Stopped so the experiment cannot
        // continue consuming EC or otherwise operate while out of focus.
        KERBALISM.Lib.Proto.Set(m, RSCKEConstants.KERBALISM_PROTO_EXP_STATE, KERBALISM.Experiment.RunningState.Stopped);
        KERBALISM.Lib.Proto.Set(m, RSCKEConstants.KERBALISM_PROTO_STATUS, KERBALISM.Experiment.ExpStatus.Stopped);
        KERBALISM.Lib.Proto.Set(m, RSCKEConstants.KERBALISM_PROTO_ISSUE, string.Empty);

        return false;
    }
}