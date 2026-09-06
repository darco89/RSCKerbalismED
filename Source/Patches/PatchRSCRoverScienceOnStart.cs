using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace RSCKerbalismED;

/// <summary>
/// Prevents RSC from attempting to resolve Stock's ModuleScienceContainer
/// during RoverScience.OnStart().
/// Kerbalism replaces Stock science storage, so RSC's ModuleScienceContainer
/// lookup is incompatible and produces an error when the vessel is loaded.
/// </summary>
[HarmonyPatch]
internal static class PatchRSCRoverScienceOnStart
{
	/// <summary>
	/// Finds RSC's OnStart(PartModule.StartState) method.
	/// </summary>
	/// <returns>The RSC OnStart method.</returns>
	private static MethodBase TargetMethod()
	{
		MethodInfo method = AccessTools.Method(
			typeof(RoverScience.RoverScience),
			nameof(RoverScience.RoverScience.OnStart),
			new[] { typeof(PartModule.StartState) });

		if (method == null)
		{
			RSCKELogger.Error("PatchRSCOnStart - Could not find OnStart(RoverScience.OnStart).");
			throw new MissingMethodException(
				typeof(RoverScience.RoverScience).FullName,
				nameof(RoverScience.RoverScience.OnStart));
		}

		return method;
	}

	/// <summary>
	/// Replaces RSC's PartModuleList indexer with our compatibility lookup.
	/// </summary>
	private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
	{
		MethodInfo moduleGetter = null;
		foreach (PropertyInfo property in typeof(PartModuleList).GetProperties())
		{
			if (property.Name != "Item")
				continue;

			MethodInfo getter = property.GetGetMethod();
			if (getter == null)
				continue;

			ParameterInfo[] parameters = getter.GetParameters();
			if (parameters.Length == 1 && parameters[0].ParameterType == typeof(string))
			{
				moduleGetter = getter;
				break;
			}
		}

		MethodInfo compatibilityGetter = GetCompatibilityGetter();
		if (moduleGetter == null)
		{
			throw new MissingMethodException(typeof(PartModuleList).FullName, "get_Item(string)");
		}
		foreach (CodeInstruction instruction in instructions)
		{
			yield return ReplaceInstruction(instruction, moduleGetter, compatibilityGetter);
		}
	}

	/// <summary>
	/// Gets the compatibility module lookup method used to replace RSC's
	/// PartModuleList string indexer.
	/// </summary>
	private static MethodInfo GetCompatibilityGetter()
	{
		MethodInfo compatibilityGetter = AccessTools.Method(
			typeof(PatchRSCRoverScienceOnStart), nameof(GetModule));

		if (compatibilityGetter == null)
		{
			throw new MissingMethodException(
				typeof(PatchRSCRoverScienceOnStart).FullName,
				nameof(GetModule));
		}

		return compatibilityGetter;
	}

	/// <summary>
	/// Replaces RSC's PartModuleList string indexer instruction with
	/// the RSCKE compatibility module lookup.
	/// </summary>
	private static CodeInstruction ReplaceInstruction(CodeInstruction instruction, MethodInfo moduleGetter, MethodInfo compatibilityGetter)
	{
		if (instruction.opcode == OpCodes.Callvirt && instruction.operand is MethodInfo method && method == moduleGetter)
			return new CodeInstruction(OpCodes.Call, compatibilityGetter);

		return instruction;
	}

	/// <summary>
	/// Resolves RSC part modules while suppressing its incompatible
	/// ModuleScienceContainer lookup.
	/// </summary>
	private static PartModule GetModule(PartModuleList modules, string moduleName)
	{
		if (moduleName == nameof(ModuleScienceContainer))
		{
			return null;
		}

		return modules[moduleName];
	}
}