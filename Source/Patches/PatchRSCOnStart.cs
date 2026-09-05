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
internal static class PatchRSCOnStart
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

        RSCKELogger.Info("Found RSC GUI method: " + method.DeclaringType.FullName + "." + method.Name);
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
            {
                continue;
            }

            MethodInfo getter = property.GetGetMethod();

            if (getter == null)
            {
                continue;
            }

            ParameterInfo[] parameters = getter.GetParameters();

            if (parameters.Length == 1 && parameters[0].ParameterType == typeof(string))
            {
                moduleGetter = getter;
                break;
            }
        }

        MethodInfo compatibilityGetter = AccessTools.Method(
            typeof(PatchRSCOnStart),
            nameof(GetModule));

        if (moduleGetter == null)
        {
            throw new MissingMethodException(
                typeof(PartModuleList).FullName,
                "get_Item(string)");
        }

        if (compatibilityGetter == null)
        {
            throw new MissingMethodException(
                typeof(PatchRSCOnStart).FullName,
                nameof(GetModule));
        }

        RSCKELogger.Info("PatchRSCOnStart - IL: Transpiler reached");

        foreach (CodeInstruction instruction in instructions)
        {
            if (instruction.opcode == OpCodes.Callvirt && instruction.operand is MethodInfo method && method == moduleGetter)
            {
                RSCKELogger.Info("PatchRSCOnStart - IL: " + instruction + " was found. applying patch");
                yield return new CodeInstruction(OpCodes.Call, compatibilityGetter);
            }
            else
            {
                yield return instruction;
            }
        }
    }

    /// <summary>
    /// Resolves RSC part modules while suppressing its incompatible
    /// ModuleScienceContainer lookup.
    /// </summary>
    private static PartModule GetModule(PartModuleList modules, string moduleName)
    {
        if (moduleName == nameof(ModuleScienceContainer))
        {
            RSCKELogger.Info("Replacing science container with null");
            return null;
        }

        RSCKELogger.Info("Modules to be used: " + modules);
        return modules[moduleName];
    }
}