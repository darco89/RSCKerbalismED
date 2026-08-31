using RoverScience;

namespace RSCKerbalismED
{
    internal sealed class RSCKEScienceSpot
    {
        // TODO: Leave only the RSC ScienceSpot fields that are actually required by RSCKE.
        internal bool IsValid { get; }
        internal string Potential { get; }
        internal string PotentialGenerated { get; }
        internal string PredictedSpot { get; }
        internal string PotentialScience { get; }
        // NOTE: AdjustedPotentialGenerated is the value RSCKE uses to determine the sample's mass roll range.
        internal string AdjustedPotentialGenerated { get; }

        /// <summary>
        /// RSCKE representation of an RSC ScienceSpot.
        /// strictly with the data RSCKE needs to create a Sample.
        /// </summary>
        /// <param name="scienceSpot">The RSC ScienceSpot object.</param>
        internal RSCKEScienceSpot(ScienceSpot scienceSpot)
        {
            IsValid = false;
            if (scienceSpot == null)
                return;

            Potential = scienceSpot.potential.ToString();
            PotentialGenerated = scienceSpot.potentialGenerated;
            AdjustedPotentialGenerated = scienceSpot.adjustedPotentialGenerated;
            PredictedSpot = scienceSpot.predictedSpot;
            PotentialScience = scienceSpot.potentialScience.ToString(); // int

            // simple validation
            IsValid = Potential != null &&
                      PotentialGenerated != null &&
                      AdjustedPotentialGenerated != null &&
                      PredictedSpot != null &&
                      PotentialScience != null;
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