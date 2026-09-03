using System;
using HarmonyLib;
using UnityEngine;

namespace RSCKerbalismED;

[KSPAddon(KSPAddon.Startup.Instantly, true)]
public class Plugin : MonoBehaviour
{
    private const string HarmonyId = RSCKEConstants.HARMONY_ID;

    /// <summary>
    /// Initializes RSCKerbalismED and loads the configuration once.
    /// </summary>
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Harmony harmony = null;

        try
        {
            RSCKELogger.Info("Woke up.");

            // RSCKE shared components to avoid multiple instances
            RSCKEMod.Initialize();

            // Harmony thing
            harmony = new Harmony(HarmonyId);
            harmony.PatchAll();
            RSCKELogger.Info("Harmony patches applied.");
        }
        catch (Exception ex)
        {
            harmony?.UnpatchAll(HarmonyId);
            RSCKELogger.Error("Failed to initialize patches.", ex);
        }


    }
}