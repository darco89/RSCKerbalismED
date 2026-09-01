using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

namespace RSCKerbalismED;

/// <summary>
/// Handles RSCKerbalismED configuration file.
/// </summary>
public class RSCKEConfig
{

    public const string RSCKE_EXPERIMENT_ID = "RSCKerbalismED";
    private const string RSCKE_CONF_SETTINGS = "RSCKERBALISMED_SETTINGS";
    private const string RSCKE_CONF_CATEGORIES_RANGES = "RSCCATEGORIES_RSCKERANGES";

    // Dictionary of ranges per RSC category (ex: categoryKey[min, max] or vlow[0.1, 0.3])
    private readonly Dictionary<string, RSCKEPercentageRange> potentialRanges = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Loads RSCKerbalismED configuration once and initializes all configured values.
    /// </summary>
    internal void Load()
    {
        try
        {
            // get config file
            string configPath = Path.Combine(KSPUtil.ApplicationRootPath, "GameData", "RSCKerbalismED", "RSCKerbalismED.cfg");
            Debug.Log("[RSCKerbalismED] INFO: Loading Configuration file from " + configPath);

            // load and populate potentialRanges
            LoadKRSCCategoriesRanges(ConfigNode.Load(configPath));
        }
        catch (Exception)
        {
            Debug.LogError("[RSCKerbalismED] ERROR: Couldn't obtain own configuration correctly.");
            throw;
        }
    }

    /// <summary>
    /// Loads the configured science potential ranges.
    /// Each config node uses Name for the exact category name returned by RSC.
    /// Anomaly is handled as a normal potential category.
    /// </summary>
    /// <param name="root">The root configuration node.</param>
    private void LoadKRSCCategoriesRanges(ConfigNode root)
    {
        // general settings node
        ConfigNode settings = root.GetNode(RSCKE_CONF_SETTINGS);
        // ranges definitions per RSC "category" node
        ConfigNode categories = settings.GetNode(RSCKE_CONF_CATEGORIES_RANGES);

        foreach (ConfigNode node in categories.nodes)
        {
            string rangeCategoryName = node.GetValue("Name");
            if (!TryReadRange(node, out double min, out double max))
            {
                Debug.LogError("[RSCKerbalismED] ERROR: Invalid potential range in node: " + node.name);
                continue;
            }

            // populate potentialRanges for category, from config
            string categoryKey = NormalizePotentialName(rangeCategoryName);
            potentialRanges[categoryKey] = new RSCKEPercentageRange
            {
                Min = min,
                Max = max
            };

            // log mapping
            Debug.Log("[RSCKerbalismED] INFO: Potential='" + categoryKey + "' | Range=" +
                potentialRanges[categoryKey].ToPercentageString());
        }

        Debug.Log("[RSCKerbalismED] INFO: Ranges for ScienceSpot Categories Loaded.");
    }

    /// <summary>
    /// Validates a configured fractional range between 0 and 1.
    /// </summary>
    /// <param name="node">The configuration node containing the range values.</param>
    /// <param name="min">Receives the configured minimum mass fraction.</param>
    /// <param name="max">Receives the configured maximum mass fraction.</param>
    /// <returns>True if the configured range is valid; otherwise false.</returns>
    private bool TryReadRange(ConfigNode node, out double min, out double max)
    {
        min = ReadDouble(node, "MinimumObtainableMassRoll", double.NaN);
        max = ReadDouble(node, "MaxObtainableMassRoll", double.NaN);

        if (double.IsNaN(min) || double.IsNaN(max))
            return false;

        return !(min <= 0.0 || max > 1.0 || min > max);
    }

    /// <summary>
    /// Reads a numeric configuration value using invariant culture.
    /// </summary>
    /// <param name="node">The configuration node containing the value.</param>
    /// <param name="key">The configuration key to read.</param>
    /// <param name="fallback">The value to return if the configured value is missing or invalid.</param>
    /// <returns>The parsed numeric value or the fallback value.</returns>
    private double ReadDouble(ConfigNode node, string key, double fallback)
    {
        string value = node.GetValue(key);

        if (string.IsNullOrEmpty(value))
            return fallback;

        if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double parsed))
            return parsed;

        Debug.LogWarning("[RSCKerbalismED] CONFIG | Invalid numeric value " + node.name + "/" + key + "='" + value + "'.");
        return fallback;
    }

    /// <summary>
    /// Gets the configured range for an actual RSC potential category.
    /// The lookup key is the exact RSC category name configured in Name,
    /// normalized to lowercase and trimmed.
    /// </summary>
    /// <param name="potential">The actual RSC potential category returned by RSC.</param>
    /// <returns>The configured percentage range, or null if no matching range was found.</returns>
    internal RSCKEPercentageRange GetPotentialRange(string potential)
    {
        return GetRange(potentialRanges, potential);
    }

    /// <summary>
    /// Looks up a percentage range by the normalized RSC potential category name.
    /// </summary>
    /// <param name="ranges">The configured potential ranges.</param>
    /// <param name="potential">The RSC potential category name.</param>
    /// <returns>The matching configured percentage range, or null if no matching range was found.</returns>
    private RSCKEPercentageRange GetRange(
        Dictionary<string, RSCKEPercentageRange> ranges,
        string potential)
    {
        string normalizedPotential = NormalizePotentialName(potential);

        if (string.IsNullOrEmpty(normalizedPotential))
            return null;

        if (!ranges.TryGetValue(normalizedPotential, out RSCKEPercentageRange range))
            return null;

        return range;
    }

    /// <summary>
    /// Normalizes RSC potential names so config Name values and RSC values use the same key.
    /// RSC returns "Very high!" and we must map it with our config "Name = very high".
    /// </summary>
    /// <param name="rscPotentialName">RSC's potential science value for the current science spot.</param>
    /// <returns>The normalized potential name.</returns>
    private string NormalizePotentialName(string rscPotentialName)
    {
        if (string.IsNullOrEmpty(rscPotentialName))
        {
            return "";
        }

        return rscPotentialName.Trim().TrimEnd('!').Trim().ToLowerInvariant();
    }
}