using System.Globalization;

namespace RSCKerbalismED;

public class RSCKEPercentageRange
{
    public double Min;

    public double Max;

    /// <summary>
    /// Returns the range formatted as percentages.
    /// </summary>
    /// <returns>The formatted percentage range.</returns>
    public string ToPercentageString()
    {
        return (Min * 100.0).ToString("0.##", CultureInfo.InvariantCulture) + "%-" +
            (Max * 100.0).ToString("0.##", CultureInfo.InvariantCulture) + "%";
    }
}