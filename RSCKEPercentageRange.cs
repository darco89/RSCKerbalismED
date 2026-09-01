using System.Globalization;

namespace RSCKerbalismED;

internal sealed class RSCKEPercentageRange
{
    internal double Min;

    internal double Max;

    /// <summary>
    /// Returns the range formatted as percentages.
    /// </summary>
    /// <returns>The formatted percentage range.</returns>
    internal string ToPercentageString()
    {
        return (Min * 100.0).ToString("0.##", CultureInfo.InvariantCulture) + "%-" +
            (Max * 100.0).ToString("0.##", CultureInfo.InvariantCulture) + "%";
    }
}