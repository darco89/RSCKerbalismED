# Rover Science Continued — Kerbalism Edition

>## **RSCKerbalismED (RSCKE) - A KSP Mod**

A compatibility mod that brings **Rover Science Continued** experiments to **Kerbalism**. **RSCKE** brings the necessary bundle for seamless compatibility, Science balancing and UI enrichment between RSC and Kerbalism.

| | |
|---|---|
| **Project / Mod Name** | RSCKerbalismED |
| **License** | [Unlicense](https://unlicense.org/) |
| **Full Name** | RoverScience Continued — Kerbalism Edition |
| **Short Name** | RSCKE |
| **RSCKE General Settings** | RSCKerbalismED.cfg |
| **Kerbalism Experiment Config** | RSCKerbalismED_Experiment.cfg |
| **C# Namespace** | `RSCKerbalismED` |
| **Harmony ID** | `RSCKerbalismED` |
| **Log Prefix** | [RSCKerbalismED] |

></br>**🔗 RSCKE Dependencies**</br></br> - [**Rover Science Continued**](https://github.com/linuxgurugamer/RoverScience-Continued) — RSC provides the rover-based ScienceSpot discovery and sample-gathering gameplay that RSCKE integrates with Kerbalism.</br> - [**Kerbalism**](https://github.com/Kerbalism/Kerbalism) — Kerbalism provides the Science Experiment, Science Storage and related systems used by RSCKE. </br></br> Note: This mod alone provides no parts nor functionalities.

</br>Adding `RSC` and `RSCKE` to your current `Kerbalism` run, should be smooth and error-free. 
</br></br>
# 🟢 WHAT IT IS

| 📡 **RSC ↔ Kerbalism Integration** - *Compatibility and Configurability* |
|:---|
» RSCKE safely handles RSC calls to Stock's `ModuleScienceContainer`, preventing errors 
» RSC `RoverBrain` parts now perform *Analysis* of a `Kerbalism Experiment` (configurable) 
» RSC `ScienceAnalysis` is altered in order to use `Kerbalism Storage` 
» Uses RSC `ScienceSpot.Potentials` (but not only) to calculate each sample volume 
» Configurable relation between `ScienceSpot.Potentials` and Experiment `SampleMass` |
» Seamless integration with Kerbalism's **Science Archive** and **Vessels** `LaunchApps` features

| 🧬 **Keeps the Best of RSC's gameplay** - *Adapted Sample Gathering Logic* |
|:---|
» Keeps RSC gameplay logic to find `ScienceSpots`, where samples can be collected 
» Keeps RSC `ScienceSpot.Potentials` logic, so better spots yield heavier samples 
» Keeps RSC's `Science Analysis` conditions, now producing a `Kerbalism Experiment` Sample 
» Keeps `RoverBrain Terminal` for UI but with new information available 
» Does not keep RSC `ScienceDecay` logic; Sample gathering is limited by Kerbalism

| ⚖️ **Beyond Compatibility** - *Improvements & Emergent Rebalancing* |
|:---|
» No more infinite Science. Kerbalism Experiments define `SampleMass` (see config file) 
» Analysis in a particular biome stops yielding Science after `SampleMass` is collected 
» RSCKE adds its own flavor to each Sample mass calculation and lets you configure it 
» Many improvements to RSC `Terminal GUI` (situation, biome and current samples representation) 
» Fixes a few RSC inconsistencies *(ex: `Terminal` now closes when losing Full Control over vessel)* 

</br>

# 🔴 WHAT IT IS NOT

| 🛰️ **RSCKE is NOT standalone** |
|:---|
» This mod is only meant to be added to playthroughs with both RSC and Kerbalism 

| 🛰️ **RSCKE does not include patches for anything else and it never will** |
|:---|
» Suggestions for changes are welcome, but they must remain in scope (RSC <-> Kerbalism relationship) 

</br>

# 🟡 WHAT ELSE

See [BACKLOG.md](BACKLOG.md) for current issues, planned improvements, and ideas.
</br>

See [ABOUT.md](ABOUT.md) for project information, development notes, contributions, and contact information.
</br>

See [License.md](LICENSE.md) for license information
</br>

Enjoy the mod! 🚀