using System;
using System.Diagnostics;
using HarmonyLib;

namespace RSCKerbalismED;

/// <summary>
/// Patches Kerbalism's Experiment.RunningUpdate() for the RSCKerbalismED experiment.
/// RSCKE uses the Kerbalism experiment for state and EC consumption,
/// but must not generate experiment data or samples.
/// </summary>
[HarmonyPatch(typeof(KERBALISM.Experiment))]
internal static class PatchKerbalismExperimentRunningUpdate
{
    [HarmonyPatch("RunningUpdate")]
    [HarmonyPrefix]
    private static bool Prefix(
        KERBALISM.Situation vs,
        KERBALISM.Experiment prefab,
        KERBALISM.ResourceInfo ec,
        KERBALISM.ExperimentInfo expInfo,
        KERBALISM.Experiment.RunningState expState,
        double elapsed_s,
        ref int lastSituationId,
        out KERBALISM.SubjectData subjectData,
        out string mainIssue)
    {
        subjectData = null;
        mainIssue = string.Empty;

        // only want to change behaviour of this experiment
        if (expInfo?.ExperimentId != RSCKEConstants.ROVER_EXPERIMENT_ID)
            return true;

        // get kerbalism subject
        subjectData = KERBALISM.ScienceDB.GetSubjectData(expInfo, vs);
        if (subjectData == null)
        {
            mainIssue = "Invalid experiment situation.";
            RSCKELogger.Error("RSCKE RunningUpdate stopped: SubjectData is null. " +
                "Experiment instance ID=" + prefab.GetInstanceID() +
                ", State=" + prefab.State +
                ", Running=" + prefab.Running +
                ", expState=" + expState + ".");
            return false;
        }

        // IMPORTANT:
        // Pass the current situation ID back to Kerbalism so it can track the experiment's situation between updates.
        lastSituationId = subjectData.Situation.Id;

        // elapsed time is <= 0.
        if (elapsed_s <= 0.0)
            return false;

        // get configured EC_Rate
        double ecRate = prefab.ec_rate;
        // EC rate is <= 0
        if (ecRate <= 0.0)
            return false;

        // Consume ElectricCharge while the experiment is running.
        double ecToConsume = ecRate * elapsed_s;
        if (ec.Amount <= 0.0)
        {
            mainIssue = "No ElectricCharge.";
            return false;
        }

        // Limit the requested consumption to the EC currently available,
        // then consume it through Kerbalism's Experiment resource broker.
        ecToConsume = Math.Min(ecToConsume, ec.Amount);
        ec.Consume(ecToConsume, KERBALISM.ResourceBroker.Experiment);
        return false;
    }
}