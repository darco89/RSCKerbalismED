using System;
using KERBALISM;
using RoverScience;

namespace RSCKerbalismED;

/// <summary>
/// Responsible for handling a sample, from creation to storage
/// </summary>
public class RSCKESampleController
{
    private readonly RSCKESampleData sample;

    /// <summary>
    /// Creates a new RSCKE sample controller for the specified vessel,
    /// RSC Science Spot, and Kerbalism experiment subject.
    /// </summary>
    /// <param name="vessel">The vessel performing the analysis.</param>
    /// <param name="scienceSpot">The RSC Science Spot where the sample is collected.</param>
    /// <param name="kerbalismSubject">The Kerbalism SubjectData associated with the sample.</param>
    public RSCKESampleController(Vessel vessel, ScienceSpot scienceSpot, SubjectData kerbalismSubject)
    {
        RSCKELogger.Info("Creating an " + kerbalismSubject.ExperimentTitle + " sample in " + vessel.name );
        // Create sample from RSC.ScienceSpot
        sample = new(scienceSpot);
        // Get current vessel (KSP) for current Situation
        sample.ScienceVessel = vessel;
        sample.SampleSubjectData = kerbalismSubject;
        // Get roll for current analysis' sample mass.
        sample.MassRoll = GetMassRoll(sample.AdjustedPotentialGenerated);
        // Current Analysis Sample Mass
        sample.CollectedMass = sample.SampleSubjectData.ExpInfo.SampleMass * sample.MassRoll;
    }

    /// <summary>
    /// Returns the sample data managed by this controller.
    /// </summary>
    public RSCKESampleData GetSample()
    {
        return sample;
    }

    /// <summary>
    /// Prepares and stores a RSCKerbalismED sample in a Kerbalism Drive.
    /// </summary>
    /// <returns>True if the sample was stored successfully; otherwise false.</returns>
    public bool StoreSample()
    {
        // Account for current Experiment progress
        PrepareSampleForStorage();
        // Store with final values
        string storageDriveName = RSCKEHub.StoreKerbalismExperimentSample(sample);
        if (!String.IsNullOrEmpty(storageDriveName))
        {
            sample.StorageDriveName = storageDriveName;
            return true;
        }
        RSCKELogger.Error("Could not obtain a Drive to store the sample.");
        return false;
    }

    /// <summary>
    /// Calculates the final sample mass to be stored.
    /// Completes the sample data with the values that storage will consider.
    /// </summary>
    private void PrepareSampleForStorage()
    {
        RSCKELogger.Info("Preparing Sample to store. Resolving vessel's experiment progress.");
        double totalMassStored = RSCKEHub.GetKerbalismSubjectTotalMassStored(sample);
        // Calculates the final sample mass to be stored
        sample.MassToStore = CalculateEffectiveMassToBeStored(totalMassStored);
        // Finishes sample data (these are just for log).
        SubjectData ssd = sample.SampleSubjectData;
        sample.DataAmount = sample.MassToStore / ssd.ExpInfo.MassPerMB;
        sample.ScienceValue = sample.DataAmount * ssd.SciencePerMB;

        // Log prepared sample
        RSCKELogger.Info("Sample Prepared to Store: " + sample.GetFormattedLog());
    }

    /// <summary>
    /// Calculates the sample mass produced by the current analysis, limiting
    /// it to the amount of sample mass still available for the subject.
    /// </summary>
    /// <param name="massStored">The amount of this subject's sample mass already stored.</param>
    /// <returns>The effective sample mass that can be stored.</returns>
    private double CalculateEffectiveMassToBeStored(double massStored)
    {
        KERBALISM.ExperimentInfo expInfo = sample.SampleSubjectData.ExpInfo;
        RSCKELogger.Info("Experiment Mass Expected: " + expInfo.SampleMass + "Kg.");
        RSCKELogger.Info("Total Experiment Mass already Stored: " + massStored + "Kg.");

        // ARCO: Double check if kerbalism handles storing an exceeding Mass for a Subject
        double storableMass = expInfo.SampleMass - massStored;
        if (storableMass <= double.Epsilon)
            RSCKELogger.Info("Experiment only allows for "+ expInfo.SampleMass +"Kg to be stored.");
        if (storableMass < sample.CollectedMass)
            RSCKELogger.Info("Considering to store only " + storableMass + "Kg.");

        // return full analysis mass, or what is left to store from that subject.
        return Math.Min(sample.CollectedMass, storableMass);
    }

    /// <summary>
    /// Determines the sample mass (roll) by current Science spot's
    /// AdjustedPotentialGenerated and RSCKE's configured Potential Range.
    /// </summary>
    /// <param name="potential">The RSC ScienceSpot.AdjustedPotentialGenerated »
    /// After normalizing, it should match RSCCATEGORIES_RSCKERANGES keys.</param>
    /// <returns>The calculated sample mass roll.</returns>
    private double GetMassRoll(string potential)
    {
        // range sets roll interval
        RSCKEPercentageRange range = RSCKEMod.Config.GetPotentialRange(potential);
        if (range == null)
        {
            string msg = "Couldn't find a configured Range for RSC Science potential '" + potential + "'.";
            InvalidOperationException ex = new(msg);
            RSCKELogger.Error(msg, ex);
            // throwing exception because all RSC potentials should be contemplated
            throw ex;
        }

        // massRoll will determine sample mass for each analysis.
        double massRoll = UnityEngine.Random.Range((float)range.Min, (float)range.Max);
        massRoll = Math.Round(Math.Max(0.0, Math.Min(1.0, massRoll)), 1);

        RSCKELogger.Info("RSC AdjustedPotentialGenerated='" + potential +
                "' | Configured Range: " + range.ToPercentageString() +
                " | RSCKE Mass Roll: " + massRoll.ToString("0.0"));

        return massRoll;
    }
}