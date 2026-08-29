using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

namespace KerbalismRSC
{
    internal static class SampleConfig
    {
        private const string KRSC_CONF_SETTINGS = "KERBALISMRSC_SETTINGS";
        private const string KRSC_CONF_BIOME_SAMPLES = "KERBALISMRSC_BIOME_SAMPLES";
        private const string KRSC_CONF_CATEGORIES_RANGES = "KERBALISMRSC_RSCCATEGORIES_KRSCRANGES";
        private const string KRSC_CONF_ANOMALY_RANGES = "KERBALISMRSC_RSCANOMALY_KRSCRANGES";

        private sealed class BiomeSample
        {
            internal string BiomeName;
            internal double ExperimentMassKg;
        }

        private sealed class PercentageRange
        {
            internal double Min;
            internal double Max;
        }

        private static readonly Dictionary<string, List<BiomeSample>> values = new Dictionary<string, List<BiomeSample>>(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<string, double> sciencePerKgPercent = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
        private static readonly Dictionary<string, PercentageRange> potentialRanges = new Dictionary<string, PercentageRange>(StringComparer.OrdinalIgnoreCase);
        private static PercentageRange anomalyRange;

        private static double defaultExperimentMassKg;
        private static double defaultSciencePerKgPercent = 100.0;
        private static double scaledScience = 100.0;
        private static bool loaded;

        // Gets the configured maximum physical sample mass for a body and KSP biome.
        internal static double GetExperimentMassKg(string body, string biome)
        {
            Load();
            BiomeSample sample = GetBiomeSample(body, biome);
            return sample != null ? sample.ExperimentMassKg : defaultExperimentMassKg;
        }

        // Gets the configured science-per-kilogram percentage for a body.
        internal static double GetSciencePerKgPercent(string body)
        {
            Load();

            double ratio;
            if (!string.IsNullOrEmpty(body) && sciencePerKgPercent.TryGetValue(body, out ratio))
                return ratio;

            return defaultSciencePerKgPercent;
        }

        // Gets the global ScaledScience percentage.
        internal static double GetScaledScience()
        {
            Load();
            return scaledScience;
        }

        // Gets the configured collection range for an actual RSC potential category.
        // The lookup key is the exact RSC category name configured in Name,
        // normalized to lowercase and trimmed.
        internal static bool TryGetPotentialRange(string potential, out double min, out double max)
        {
            Load();
            return TryGetRange(potentialRanges, potential, out min, out max);
        }

        // Gets the single configured collection range used by all RSC anomalies.
        internal static bool TryGetAnomalyRange(out double min, out double max)
        {
            Load();

            min = 0.0;
            max = 0.0;

            if (anomalyRange == null)
                return false;

            min = anomalyRange.Min;
            max = anomalyRange.Max;
            return true;
        }

        // Looks up a percentage range by the normalized RSC potential category name.
        private static bool TryGetRange(
            Dictionary<string, PercentageRange> ranges,
            string potential,
            out double min,
            out double max)
        {
            min = 0.0;
            max = 0.0;

            string normalizedPotential = NormalizePotentialName(potential);

            if (string.IsNullOrEmpty(normalizedPotential))
                return false;

            if (!ranges.TryGetValue(normalizedPotential, out PercentageRange range) || range == null)
                return false;

            min = range.Min;
            max = range.Max;
            return true;
        }

        // Normalizes RSC potential names so config Name values and RSC values use the same key.
        private static string NormalizePotentialName(string value)
        {
            if (string.IsNullOrEmpty(value))
                return "";

            return value
                .Trim()
                .TrimEnd('!')
                .Trim()
                .ToLowerInvariant();
        }

        // Loads KerbalismRSC configuration once and initializes all configured values.
        private static void Load()
        {
            if (loaded)
                return;

            loaded = true;

            string configPath = Path.Combine(KSPUtil.ApplicationRootPath, "GameData", "KerbalismRSC", "KerbalismRSC.cfg");
            Debug.Log("[KerbalismRSC] CONFIG | Loading: " + configPath);

            if (!File.Exists(configPath))
            {
                Debug.LogError("[KerbalismRSC] CONFIG ERROR | File not found: " + configPath);
                return;
            }

            ConfigNode root;

            try
            {
                root = ConfigNode.Load(configPath);
            }
            catch (Exception ex)
            {
                Debug.LogError("[KerbalismRSC] CONFIG ERROR | Could not parse KerbalismRSC.cfg: " + ex);
                return;
            }

            if (root == null)
            {
                Debug.LogError("[KerbalismRSC] CONFIG ERROR | ConfigNode.Load returned null.");
                return;
            }

            ConfigNode settings = root.GetNode(KRSC_CONF_SETTINGS);
            ConfigNode samples = root.GetNode(KRSC_CONF_BIOME_SAMPLES);
            ConfigNode potential = root.GetNode(KRSC_CONF_CATEGORIES_RANGES);
            ConfigNode anomaly = root.GetNode(KRSC_CONF_ANOMALY_RANGES);

            Debug.Log("[KerbalismRSC] CONFIG | SETTINGS = " + (settings != null ? "FOUND" : "MISSING"));
            Debug.Log("[KerbalismRSC] CONFIG | BIOME_SAMPLES = " + (samples != null ? "FOUND" : "MISSING"));
            Debug.Log("[KerbalismRSC] CONFIG | POTENTIAL = " + (potential != null ? "FOUND" : "MISSING"));
            Debug.Log("[KerbalismRSC] CONFIG | ANOMALY = " + (anomaly != null ? "FOUND" : "MISSING"));

            LoadSettings(settings);
            LoadKRSCBiomeSamplesConfig(samples);
            LoadKRSCCategoriesRanges(potential);
            LoadAnomalyRange(anomaly);

            Debug.Log("[KerbalismRSC] CONFIG | Loaded. ScaledScience=" + scaledScience + "% | Bodies=" + values.Count + " | SciencePerKgPercent=" + sciencePerKgPercent.Count + " | PotentialRanges=" + potentialRanges.Count + " | AnomalyRange=" + (anomalyRange != null ? "FOUND" : "MISSING"));
        }

        // Loads the global science difficulty modifier.
        private static void LoadSettings(ConfigNode node)
        {
            if (node == null)
            {
                Debug.LogWarning("[KerbalismRSC] CONFIG | Missing " + KRSC_CONF_SETTINGS + ".");
                return;
            }

            string value = node.GetValue("ScaledScience");
            if (string.IsNullOrEmpty(value))
                return;

            double parsed;
            if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out parsed))
                scaledScience = Math.Max(0.0, parsed);
            else
                Debug.LogWarning("[KerbalismRSC] CONFIG | Invalid ScaledScience='" + value + "'. Using 100.");
        }

        // Loads physical sample mass per biome and science-per-kilogram modifier (per body).
        private static void LoadKRSCBiomeSamplesConfig(ConfigNode root)
        {
            if (root == null)
            {
                Debug.LogWarning("[KerbalismRSC] CONFIG | Missing " + KRSC_CONF_BIOME_SAMPLES + ".");
                return;
            }

            ConfigNode globalDefault = root.GetNode("default");

            if (globalDefault != null)
            {
                defaultExperimentMassKg = ReadDouble(globalDefault, "ExperimentMassKg", 0.0);
                defaultSciencePerKgPercent = ReadDouble(globalDefault, "SciencePerKgPercent", 100.0);

                if (defaultSciencePerKgPercent < 0.0)
                    defaultSciencePerKgPercent = 0.0;
            }

            foreach (ConfigNode bodyNode in root.nodes)
            {
                if (bodyNode.name.Equals("default", StringComparison.OrdinalIgnoreCase))
                    continue;

                List<BiomeSample> biomeValues = new List<BiomeSample>();

                foreach (ConfigNode biomeNode in bodyNode.nodes)
                {
                    if (biomeNode.name.Equals("default", StringComparison.OrdinalIgnoreCase))
                        continue;

                    string biomeName = biomeNode.GetValue("Biome");

                    if (string.IsNullOrEmpty(biomeName))
                    {
                        Debug.LogWarning("[KerbalismRSC] CONFIG | Body=" + bodyNode.name + " | Biome node=" + biomeNode.name + " has no Biome property.");
                        continue;
                    }

                    double mass = ReadDouble(biomeNode, "ExperimentMassKg", 0.0);

                    biomeValues.Add(new BiomeSample
                    {
                        BiomeName = biomeName,
                        ExperimentMassKg = mass
                    });

                    Debug.Log("[KerbalismRSC] CONFIG | Body=" + bodyNode.name + " | Biome='" + biomeName + "' | ExperimentMassKg=" + mass + "kg");
                }

                values[bodyNode.name] = biomeValues;

                double bodySciencePerKgPercent = ReadDouble(bodyNode, "SciencePerKgPercent", defaultSciencePerKgPercent);

                if (bodySciencePerKgPercent < 0.0)
                {
                    Debug.LogWarning("[KerbalismRSC] CONFIG | Negative SciencePerKgPercent for " + bodyNode.name + ". Clamping to 0.");
                    bodySciencePerKgPercent = 0.0;
                }

                sciencePerKgPercent[bodyNode.name] = bodySciencePerKgPercent;

                Debug.Log("[KerbalismRSC] CONFIG | Body=" + bodyNode.name + " | SciencePerKgPercent=" + bodySciencePerKgPercent + " | Biomes=" + biomeValues.Count);
            }
        }

        // Loads the five normal RSC actual-potential collection ranges.
        // Each config node uses Name for the exact category name returned by RSC.
        private static void LoadKRSCCategoriesRanges(ConfigNode root)
        {
            if (root == null)
            {
                Debug.LogError("[KerbalismRSC] CONFIG ERROR | Missing " + KRSC_CONF_CATEGORIES_RANGES + ".");
                return;
            }

            foreach (ConfigNode node in root.nodes)
            {
                if (!TryReadRange(node, out double min, out double max))
                {
                    Debug.LogError("[KerbalismRSC] CONFIG ERROR | Invalid potential range: " + node.name);
                    continue;
                }

                string potentialName = node.GetValue("Name");

                if (string.IsNullOrEmpty(potentialName))
                {
                    Debug.LogError("[KerbalismRSC] CONFIG ERROR | Potential range '" + node.name + "' has no Name property.");
                    continue;
                }

                string normalizedPotentialName = NormalizePotentialName(potentialName);

                potentialRanges[normalizedPotentialName] = new PercentageRange
                {
                    Min = min,
                    Max = max
                };

                Debug.Log("[KerbalismRSC] CONFIG | Potential='" + normalizedPotentialName + "' | Range=" +
                    (min * 100.0).ToString("0.##", CultureInfo.InvariantCulture) + "%-" +
                    (max * 100.0).ToString("0.##", CultureInfo.InvariantCulture) + "%");
            }
        }

        // Loads the single collection range shared by every RSC anomaly.
        private static void LoadAnomalyRange(ConfigNode root)
        {
            anomalyRange = null;

            if (root == null)
            {
                Debug.LogError("[KerbalismRSC] CONFIG ERROR | Missing " + KRSC_CONF_ANOMALY_RANGES + ".");
                return;
            }

            if (!TryReadRange(root, out double min, out double max))
            {
                Debug.LogError("[KerbalismRSC] CONFIG ERROR | Invalid anomaly range.");
                return;
            }

            anomalyRange = new PercentageRange
            {
                Min = min,
                Max = max
            };

            Debug.Log("[KerbalismRSC] CONFIG | Anomaly range=" +
                (min * 100.0).ToString("0.##", CultureInfo.InvariantCulture) + "%-" +
                (max * 100.0).ToString("0.##", CultureInfo.InvariantCulture) + "%");
        }

        // Finds the configured sample for the exact KSP biome returned by ScienceUtil.
        private static BiomeSample GetBiomeSample(string body, string biome)
        {
            List<BiomeSample> bodyValues;

            if (!string.IsNullOrEmpty(body) && values.TryGetValue(body, out bodyValues))
            {
                if (!string.IsNullOrEmpty(biome))
                {
                    foreach (BiomeSample sample in bodyValues)
                    {
                        if (string.Equals(sample.BiomeName, biome, StringComparison.OrdinalIgnoreCase))
                        {
                            Debug.Log("[KerbalismRSC] CONFIG | BIOME MATCH | Body=" + body + " | KSP Biome='" + biome + "' | Mass=" + sample.ExperimentMassKg + "kg");
                            return sample;
                        }
                    }
                }

                Debug.LogWarning("[KerbalismRSC] CONFIG | BIOME MISS | Body=" + body + " | KSP Biome='" + biome + "'. Using global mass fallback.");
            }

            return null;
        }

        // Validates a configured fractional range between 0 and 1.
        private static bool TryReadRange(ConfigNode node, out double min, out double max)
        {
            min = ReadDouble(node, "MinimumObtainableMassRoll", double.NaN);
            max = ReadDouble(node, "MaxObtainableMassRoll", double.NaN);

            if (double.IsNaN(min) || double.IsNaN(max))
                return false;

            return !(min < 0.0 || max > 1.0 || min > max);
        }

        // Reads a numeric configuration value using invariant culture.
        private static double ReadDouble(ConfigNode node, string key, double fallback)
        {
            string value = node.GetValue(key);

            if (string.IsNullOrEmpty(value))
                return fallback;

            double parsed;

            if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out parsed))
                return parsed;

            Debug.LogWarning("[KerbalismRSC] CONFIG | Invalid numeric value " + node.name + "/" + key + "='" + value + "'.");
            return fallback;
        }
    }
}