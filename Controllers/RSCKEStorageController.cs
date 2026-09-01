using System;
using UnityEngine;

namespace RSCKerbalismED;

/// <summary>
/// Handles samples storage but delegates
/// the actual Kerbalism storage operation to RSCKEHub.
/// </summary>
internal static class RSCKEStorageController
{
    /// <summary>
    /// Prepares and stores an RSCKerbalismED sample via Kerbalism.
    /// </summary>
    /// <param name="sampleData">The current RSCKE SampleData to be stored.</param>
    /// <returns>True if the sample was stored successfully; otherwise false.</returns>
    internal static bool StoreSample(RSCKESampleData sampleData)
    {
        // Prepares sample to store 
        RSCKESampleData sample = PrepareSampleForStorage(sampleData);
        if (sample == null)
        {
            Debug.LogError("[RSCKerbalismED] ERROR: Couldn't prepare sample to store.");
            return false;
        }
        // Sends for Kerbalism storage 
        return RSCKEHub.StoreKerbalismExperimentSample(sample);
    }

    /// <summary>
    /// Completes the sample data required before it can be stored by Kerbalism.
    /// Calculates the effective physical sample mass, converts it to Kerbalism
    /// data amount, and calculates the science value represented by that data.
    /// </summary>
    /// <param> </param>
    /// <returns>The prepared sample, or null if no sample can be stored.</returns>
    private static RSCKESampleData PrepareSampleForStorage(RSCKESampleData sampleData)
    {
        // Kerbalism: Gets total mass already stored for this Subject
        KERBALISM.ExperimentInfo expInf = sampleData.SampleSubjectData.ExpInfo;
        double massPreviouslyStored = RSCKEHub.GetKerbalismSubjectMassAlreadyStored(sampleData);
        Debug.Log("[RSCKerbalismED] INFO: Current vessel has a total of "+ massPreviouslyStored+"Kg for current Subject.");
        // finishes sample data.
        sampleData.FinalSampleMass = CalculateEffectiveMassToBeStored(expInf, massPreviouslyStored, sampleData.MassRoll);
        sampleData.DataAmount = sampleData.FinalSampleMass / expInf.MassPerMB;
        sampleData.ScienceValue = sampleData.DataAmount * sampleData.SampleSubjectData.SciencePerMB;
        Debug.Log("[RSCKerbalismED] INFO: Sample Prepared to Store: " + sampleData.ToString());
        return sampleData;
    }

    /// <summary>
    /// Calculates the physical sample mass produced by the current analysis,
    /// limiting it to the amount of sample mass still available for the subject.
    /// </summary>
    /// <param name="expInfo">Kerbalism experiment information for the current subject.</param>
    /// <param name="MassStored">The amount of this subject's sample mass already stored.</param>
    /// <param name="roll">The RSCKE mass roll determining the fraction of the experiment sample mass collected.</param>
    /// <returns>The effective sample mass that can be stored.</returns>
    private static double CalculateEffectiveMassToBeStored(KERBALISM.ExperimentInfo expInfo, double MassStored, double roll)
    {
        Debug.Log("[RSCKerbalismED] INFO: Experiment Info Sample mass: " + expInfo.SampleMass + "Kg | ");
        Debug.Log("[RSCKerbalismED] INFO: Total Subject mass Stored: " + MassStored + "Kg | ");

        // ARCO: Double check if kerbalism handles storing an exceeding Mass for a Subject
        double remainingMass = expInfo.SampleMass - MassStored;
        if (remainingMass <= double.Epsilon)
        {
            Debug.Log("[RSCKerbalismED] INFO: Total Experiment Samples mass gathered would be higher than allowed ("+ expInfo.SampleMass +"Kg).");
            Debug.Log("[RSCKerbalismED] INFO: Considering to store only "+ remainingMass +"Kg. ");
        }
        Debug.Log("[RSCKerbalismED] INFO: Remaining Mass to Gather: " + remainingMass + "Kg | ");

        // (ex: 20*0.08 = 8% of 20 Kg = 1,6Kg)
        double mass = expInfo.SampleMass * roll;
        // return full analysis mass, or what is left to store from that subject.
        return Math.Min(mass, remainingMass);
    }
}