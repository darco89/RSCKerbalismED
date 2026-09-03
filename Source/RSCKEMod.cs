namespace RSCKerbalismED;

/// <summary>
/// Provides access to shared Mod components.
/// Objects that need application-wide lifetime/state control
/// </summary>
internal static class RSCKEMod
{
    /// <summary>
    /// Configuration to be used throughout the mod components
    /// </summary>
    internal static RSCKEConfig Config { get; private set; }
    internal static RSCKETracker Tracker { get; private set; }

    /// <summary>
    /// Initializes RSCKerbalismED Mod state.
    /// </summary>
    internal static void Initialize()
    {
        RSCKELogger.Info("Loading configs.");
        Config = new RSCKEConfig();
        RSCKELogger.Info("Initializing Tracker.");
        Tracker = new RSCKETracker();
    }
}