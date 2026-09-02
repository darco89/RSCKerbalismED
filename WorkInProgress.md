# **Rover Science Continued — Kerbalism Edition**

># **RSCKerbalismED** (RSCKE) - A KSP Mod
A compatibility mod that brings **RoverScience Continued** experiments to **Kerbalism**. **RSCKE** brings the bundle  necessary for seamless compatibility, Science balancing and UI enrichment, for playthroughs with Kerbalism (and RSC).

| | |
|---|---|
| **Project / Mod Name** | RSCKerbalismED |
| **License** | [Unlicense](https://unlicense.org/) |
| **Full Name** | RoverScience Continued — Kerbalism Edition |
| **Short Name** | RSCKE |
| **RSCKE General Settings** | RSCKerbalismED.cfg |
| **Kerbalism Experiment Config** | RSCKerbalismED_Experiment.cfg |
| **Kerbalism Experiment Title** | RSCKE Rover Sample
| **C# Namespace** | `RSCKerbalismED` |
| **Harmony ID** | `RSCKerbalismED` |
| **Log Prefix** | [RSCKerbalismED] |

>#### *INTRO*

`Rover Science Continued - Kerbalism Edition` is a KSP mod/patch that provides a way for `Rover Science Continued` mod (adopted by LinuxGuruGamer) and `Kerbalism` mod to work well along with each other. 

With `RSCKE` installed, players will be able to store `RSC` *Analysis samples* in a `Kerbalism Storage Drive`. All the *RSC gameplay loop* stays for you to enjoy. In parallel, includes rebalances that Kerbalism naturally brought along for an Experiment (ex: limited science to be gathered per situation) and other improvements, including visual representation of all new relevant data to the RSC `Terminal GUI`. RSCKE experiments and sample data will also be shown in both Kerbalism LaunchApps as expected.

RSCKE aims to go beyond basic compatibility by providing a richer and yet more balanced way for RSC science instruments to gather samples during Kerbalism playthroughs.

Adding `RSC` and `RSCKE` to your `Kerbalism` run should be smooth and error-free.

</br>

# 🟢 WHAT IT IS

| 📡 **RSC ↔ Kerbalism Integration** - *Compatibility and Configurability*|
|---|
|    » RSCKE safely handles RSC calls to Stock's `ModuleScienceContainer`, preventing errors  |
|    » RSC `RoverBrain` parts now perform *Analysis* of a `Kerbalism Experiment`  (configurable)|
|    » RSC `ScienceAnalysis` is altered in order to use `Kerbalism Storage` instead of Stock's |
|    » Uses RSC `ScienceSpot.Potentials` (but not only) to calculate each sample volume  |
|    » Configurable relation between `ScienceSpot.Potentials` and Experiment `SampleMass` |
|    » Seamless integration with Kerbalism's ***Science Archive*** and ***Vessels*** `LaunchApps` features  |

| 🧬 **Keeps the Best of RSC's gameplay** - *Adapted Sample Gathering Logic*|
|---|
|    » Keeps RSC gameplay logic to find `ScienceSpots`, where samples can be collected |
|    » Keeps RSC `ScienceSpot.Potentials` logic, so better spots yield heavier samples |
|    » Keeps RSC's `Science Analysis` conditions, now producing a `Kerbalism Experiment` Sample |
|    » Keeps the pretty `RoverBrain Terminal` with coherent (and more) information 
|    » Does not keep RSC `ScienceDecay` logic; Sample gathering is limited by Kerbalism Mechanics |

| ⚖️ **Only with RSCKE** - *Improvements & Emergent Rebalancing* |
|---|
| » No more infinite Science. Kerbalism Experiments define `SampleMass` (configure it) |
| » Analysis in a particular biome stops yielding Science after `SampleMass` is collected |
| » RSCKE adds its own flavor to each Sample mass calculation and lets you configure it |
| » Many improvements to RSC `Terminal GUI` (situation and current samples representation) |
| » Fixes a few RSC inconsistencies *(ex: `Terminal` now closes when losing Full Control over vessel)*
</br></br>

# 🔴 WHAT IT IS NOT

| 🛰️ **RSCKE is NOT standalone** |
|---|
| » This mod is only meant to be added to playthroughs with both RSC and Kerbalism |

| 🛰️ **RSCKE does not include patches for anything else and it never will**|
|---|
|» Taking suggestions for changes but must remain in scope (RSC <-> Kerbalism relationship)|

</br></br>


# 🟡 WHAT IS COMING AND/OR BEING CONSIDERED


## Situation: RoverBrain Terminal GUI

| Issue | Notes | Status
|---|---|---|
| Terminal keeps running even after losing vessel's Full Control | RSC bug | Fixed |
| After confirming and storing sample, Science Spot needs to be reset | despawn dome and ROC | Priority |
| Try to show (keep track) of how many spots were visited per biome | GUI - correction | For 1st release |
| Try to show (keep track) of collected samples quantity and mass  | GUI - new| |
| Show current biome and amount of mass gathered for current biome while ScienceSpot is not reached | nice to have | For 1st release |
| Generate new GUI/screen for sample obtained result | GUI - new | For 1st release | 

## Situation: Kerbalism Science Archive

| Issue | Notes | Status
|---|---|---|
| Experiment not listed when filtering by current vessel | RSC part roverBrain still uses old Experiment Id. |
| Science points balance according to Surface Sample across biomes | Kerbalism is somehow using different values per Body |


## Situation: The "Auto" tab on Kerbalism "Vessel Pane"

| Issue | Notes | Status
|---|---|---|
| Experiment not listed on "Vessel Pane" | RSC part roverBrain still uses old Experiment Id.|


</br></br>
## **Support RSCKerbalismED**  

Contributions from other developers are more than welcome.

>Disclaimer<br> This is the first code I have published for public use. I'm committed to delivering a good, easy-to-maintain product that is well organized and follows long-established community practices. Any criticism, suggestions, or ideas are very welcome.

Enjoy the mod! 🚀

</br>

|Other Ideas that might be coming to same mod (RSCKE)|
|---|
|Compatibility with RP-1</br>|
|Compatibility with `ProbeControlRoom` and `RasterPropMonitor` (maybe control Rover from Misison Control and control experiment via MFDs)</br>|
|Allow configurable Energy Consumption for `RoverBrain` parts</br>|
|The `Album` idea - RSC spawns objects ROC1 or ROC2 at science spots. If not too costly, maybe try to have Roverbrain parts recognize a camera in the vessel to take pictures with, when Sample is Gathered. The picture would be stored in the RoverBrain "Album" and visible in both Terminal GUI and MFDs. Too much? Oh, this could be interesting for Telescopes and Cameras in general, too. |

These are a few ideas that i talked about with a few people. Feasibility was still not acessed.
</br>

|Other mods that might have same Idea coming|
|---|
For now I can only think of `Tarsier Space Technology with Galaxies Continued`

</br></br>

# Contact me

|🛰️ **For more mods like this**|
|---|
|For the time being I'm willing to try to patch other mods that have parts with Science Experiments that aren't working with Kerbalism. I'm planning to take a look at Tarsier Space Technologies mod next. Do you know other mods? Let me know. |

|🛰️ **Like my work?**|
|---|
|I'm available to contribute for bigger projects. |

|🛰️ **Need a custom mod?**|
|---|
|You can commission me, reach out. |

### Choose your preferred way to do so and get in touch!☻  </br> [**> Contacts here <**](https://bit.ly/m/kaputzztv) </br>

KSP Forum Profile: https://forum.kerbalspaceprogram.com/profile/231133-kaputzz/ </br>  GitHub Profile: https://github.com/darco89