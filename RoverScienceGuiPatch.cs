using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace KerbalismRSC
{
    [HarmonyPatch]
    internal static class RoverScienceGuiPatch
    {
        private static MethodBase TargetMethod()
        {
            Type guiType =
                AccessTools.TypeByName("RoverScience.RoverScienceGUI");

            if (guiType == null)
            {
                UnityEngine.Debug.LogError(
                    "[KerbalismRSC] ERROR: Could not find " +
                    "RoverScience.RoverScienceGUI.");

                return null;
            }

            MethodInfo method =
                AccessTools.Method(
                    guiType,
                    "DrawRoverConsoleGUI");

            if (method == null)
            {
                UnityEngine.Debug.LogError(
                    "[KerbalismRSC] ERROR: Could not find " +
                    "RoverScienceGUI.DrawRoverConsoleGUI.");

                return null;
            }

            UnityEngine.Debug.Log(
                "[KerbalismRSC] Found RSC GUI method: " +
                method.DeclaringType.FullName +
                "." +
                method.Name);

            return method;
        }

        private static IEnumerable<CodeInstruction> Transpiler(
            IEnumerable<CodeInstruction> instructions)
        {
            MethodInfo safeGetStoredDataCount =
                AccessTools.Method(
                    typeof(RoverScienceGuiPatch),
                    nameof(SafeGetStoredDataCount));

            MethodInfo safeGetCapacity =
                AccessTools.Method(
                    typeof(RoverScienceGuiPatch),
                    nameof(SafeGetCapacity));

            bool getStoredDataCountPatched = false;
            bool capacityPatched = false;

            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Callvirt &&
                    instruction.operand is MethodInfo method &&
                    method.Name == "GetStoredDataCount" &&
                    method.DeclaringType == typeof(ModuleScienceContainer))
                {
                    yield return new CodeInstruction(
                        OpCodes.Call,
                        safeGetStoredDataCount);

                    getStoredDataCountPatched = true;
                    continue;
                }

                if (instruction.opcode == OpCodes.Ldfld &&
                    instruction.operand is FieldInfo field &&
                    field.Name == "capacity" &&
                    field.DeclaringType == typeof(ModuleScienceContainer))
                {
                    yield return new CodeInstruction(
                        OpCodes.Call,
                        safeGetCapacity);

                    capacityPatched = true;
                    continue;
                }

                yield return instruction;
            }

            if (getStoredDataCountPatched)
            {
                UnityEngine.Debug.Log(
                    "[KerbalismRSC] Patched " +
                    "ModuleScienceContainer.GetStoredDataCount().");
            }
            else
            {
                UnityEngine.Debug.LogError(
                    "[KerbalismRSC] ERROR: Could not find " +
                    "ModuleScienceContainer.GetStoredDataCount().");
            }

            if (capacityPatched)
            {
                UnityEngine.Debug.Log(
                    "[KerbalismRSC] Patched " +
                    "ModuleScienceContainer.capacity.");
            }
            else
            {
                UnityEngine.Debug.LogError(
                    "[KerbalismRSC] ERROR: Could not find " +
                    "ModuleScienceContainer.capacity.");
            }
        }

        private static int SafeGetStoredDataCount(
            ModuleScienceContainer container)
        {
            if (container == null)
                return 0;

            return container.GetStoredDataCount();
        }

        private static int SafeGetCapacity(
            ModuleScienceContainer container)
        {
            if (container == null)
                return 0;

            return container.capacity;
        }
    }
}
