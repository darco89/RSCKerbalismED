namespace RSCKerbalismED;

/// <summary>
/// Tracks RSCKerbalismED statistics for the current game save.
/// </summary>
public class RSCKETracker
{
    private int analysisCount;

    /// <summary>
    /// Gets the total number of analyses performed in the current game save.
    /// </summary>
    /// <returns>The total number of analyses performed.</returns>
    public int GetAnalysisCount()
    {
        return analysisCount;
    }

    /// <summary>
    /// Increments the total number of analyses performed in the current game save.
    /// </summary>
    public void IncrementAnalysisCount()
    {
        analysisCount++;
    }
}