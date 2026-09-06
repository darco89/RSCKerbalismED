// using System.Reflection;
// using HarmonyLib;
// using RSCKerbalismED.Source.Services;

// namespace RSCKerbalismED;

// /// <summary>
// /// Patches RSC's private ShowGUI() method so the Kerbalism
// /// experiment follows the RSC Rover Terminal open/closed state.
// /// </summary>
// [HarmonyPatch]
// internal static class PatchRSCRoverTerminalToggle
// {
// 	private static MethodBase TargetMethod()
// 	{
// 		return AccessTools.Method(typeof(RoverScience.RoverScience), "ShowGUI");
// 	}

// 	private static void Postfix(RoverScience.RoverScience __instance)
// 	{
// 		if (__instance?.roverScienceGUI?.consoleGUI == null)
// 			return;

// 		RSCKELogger.Info("RSC ShowGUI completed. " +
// 			"RSC instance ID=" + __instance.GetInstanceID() +
// 			", IsPrimary=" + __instance.IsPrimary +
// 			", TerminalOpen=" + __instance.roverScienceGUI.consoleGUI.isOpen + ".");

// 		KERBALISM.Experiment experiment = null;

// 		foreach (PartModule module in __instance.part.Modules)
// 		{
// 			KERBALISM.Experiment candidate = module as KERBALISM.Experiment;
// 			if (candidate != null && candidate.experiment_id == RSCKEConstants.ROVER_EXPERIMENT_ID)
// 			{
// 				experiment = candidate;
// 				break;
// 			}
// 		}

// 		if (experiment == null)
// 		{
// 			RSCKELogger.Error("RSC ShowGUI could not find the Kerbalism Experiment. " +
// 				"Experiment ID='" + RSCKEConstants.ROVER_EXPERIMENT_ID + "'.");
// 			return;
// 		}

// 		RSCKELogger.Info("RSC ShowGUI before synchronization. " +
// 			"Experiment instance ID=" + experiment.GetInstanceID() +
// 			", State=" + experiment.State +
// 			", Running=" + experiment.Running + ".");

// 		RSCKETerminalService.SyncKerbalismExperimentState(__instance);

// 		RSCKELogger.Info("RSC ShowGUI after synchronization. " +
// 			"Experiment instance ID=" + experiment.GetInstanceID() +
// 			", State=" + experiment.State +
// 			", Running=" + experiment.Running + ".");
// 	}
// }

using System.Reflection;
using HarmonyLib;
using RSCKerbalismED.Source.Services;

namespace RSCKerbalismED;

/// <summary>
/// Patches RSC's private ShowGUI() method so the Kerbalism
/// experiment follows the RSC Rover Terminal open/closed state.
/// </summary>
[HarmonyPatch]
internal static class PatchRSCRoverTerminalToggle
{
	private static MethodBase TargetMethod()
	{
		return AccessTools.Method(typeof(RoverScience.RoverScience), "ShowGUI");
	}

	private static void Postfix(RoverScience.RoverScience __instance)
	{
		if (__instance?.roverScienceGUI?.consoleGUI == null)
			return;

		// Synchronize the Kerbalism experiment with the RSC Rover Terminal state.
		// The service handles the Kerbalism experiment lookup and synchronization.
		RSCKETerminalService.SyncKerbalismExperimentState(__instance);
	}
}