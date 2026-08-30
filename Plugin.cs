using System;
using HarmonyLib;
using UnityEngine;

namespace RSCKerbalismED
{
    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    public class Plugin : MonoBehaviour
    {
        private const string HarmonyId = "RSCKerbalismED";

        internal static RSCKEConfig rcskeConfig { get; private set; }

        /// <summary>
        /// Initializes RSCKerbalismED and loads the configuration once.
        /// </summary>
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            Debug.Log("[RSCKerbalismED] INFO: Initializing.");

            try
            {
                rcskeConfig = new RSCKEConfig();
                rcskeConfig.Load();

                var harmony = new Harmony(HarmonyId);
                harmony.PatchAll();
                Debug.Log("[RSCKerbalismED] INFO: Harmony patches applied.");
            }
            catch (Exception ex)
            {
                Debug.LogError("[RSCKerbalismED] ERROR: Failed to initialize.");
                Debug.LogException(ex);
            }
        }
    }
}