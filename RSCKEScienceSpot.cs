using System.Reflection;
using HarmonyLib;

namespace RSCKerbalismED
{
    // TOOD: clean up whatever was used just for logging (understand RSC)
    internal sealed class RSCKEScienceSpot
    {
        internal string Potential { get; }
        internal string PotentialGenerated { get; }
        internal string PredictedSpot { get; }
        internal string PotentialScience { get; }

        // AdjustedPotentialGenerated is the field RSCKE will use to determine sample's "mass roll range"
        internal string AdjustedPotentialGenerated { get; }

        internal bool IsValid { get; }

        /// <summary>
        /// RSCKE representation of the original RSC's scienceSpot object (relevant data only).
        /// NOTE: We are currently only using AdjustedPotentialGenerated. The rest was just useful at some point.
        /// </summary>
        /// <param name="scienceSpot">The RSC scienceSpot object obtained from RoverScience.</param>
        internal RSCKEScienceSpot(object scienceSpot)
        {
            IsValid = false;

            if (scienceSpot == null)
                return;

            System.Type scienceSpotType = scienceSpot.GetType();
            Potential = GetFieldValue(scienceSpotType, scienceSpot, "potential");
            PotentialGenerated = GetFieldValue(scienceSpotType, scienceSpot, "potentialGenerated");
            AdjustedPotentialGenerated = GetFieldValue(scienceSpotType, scienceSpot, "adjustedPotentialGenerated");
            PredictedSpot = GetFieldValue(scienceSpotType, scienceSpot, "predictedSpot");
            PotentialScience = GetFieldValue(scienceSpotType, scienceSpot, "potentialScience");

            IsValid = Potential != null &&
                      PotentialGenerated != null &&
                      AdjustedPotentialGenerated != null &&
                      PredictedSpot != null &&
                      PotentialScience != null;
        }

        /// <summary>
        /// Gets a value from an RSC scienceSpot field.
        /// </summary>
        /// <param name="scienceSpotType">The runtime type of the RSC scienceSpot object.</param>
        /// <param name="scienceSpot">The RSC scienceSpot object.</param>
        /// <param name="fieldName">The name of the RSC field to retrieve.</param>
        /// <returns>The value corresponding to fieldName, or null if the field or value cannot be found.</returns>
        private string GetFieldValue(System.Type scienceSpotType, object scienceSpot, string fieldName)
        {
            FieldInfo field = AccessTools.Field(scienceSpotType, fieldName);
            object value = field?.GetValue(scienceSpot);

            return value?.ToString();
        }

        /// <summary>
        /// Returns a readable representation of this object.
        /// </summary>
        /// <returns>A formatted representation of the science spot data.</returns>
        public override string ToString()
        {
            return "potential=" + (Potential ?? "<null>") +
                " | potentialGenerated=" + (PotentialGenerated ?? "<null>") +
                " | adjustedPotentialGenerated=" + (AdjustedPotentialGenerated ?? "<null>") +
                " | predictedSpot=" + (PredictedSpot ?? "<null>") +
                " | potentialScience=" + (PotentialScience ?? "<null>");
        }
    }
}