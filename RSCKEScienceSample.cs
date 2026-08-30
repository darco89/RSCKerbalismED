namespace RSCKerbalismED
{
    internal sealed class RSCKEScienceSample
    {
        internal KERBALISM.SubjectData SubjectData { get; }
        internal double DataAmount { get; }
        internal double Mass { get; }

        /// <summary>
        /// A class to represent a "Kerbalism sample".
        /// </summary>
        /// <param name="subjectData">The Kerbalism subject receiving the sample.</param>
        /// <param name="dataAmount">The amount of Kerbalism data in MB.</param>
        /// <param name="mass">The physical sample mass in tons.</param>
        internal RSCKEScienceSample(
            KERBALISM.SubjectData subjectData,
            double dataAmount,
            double mass)
        {
            SubjectData = subjectData;
            DataAmount = dataAmount;
            Mass = mass;
        }
    }
}