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