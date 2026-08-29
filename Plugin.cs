using System;
using HarmonyLib;
using UnityEngine;

namespace KerbalismRSC
{
    [KSPAddon(KSPAddon.Startup.Instantly, true)]
    public class Plugin : MonoBehaviour
    {
        private const string HarmonyId = "KerbalismRSC";

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);

            Debug.Log("[KerbalismRSC] Initializing.");

            try
            {
                var harmony = new Harmony(HarmonyId);
                harmony.PatchAll();

                Debug.Log("[KerbalismRSC] Harmony patches applied.");
            }
            catch (Exception ex)
            {
                Debug.LogError("[KerbalismRSC] Failed to initialize.");
                Debug.LogException(ex);
            }
        }
    }
}
