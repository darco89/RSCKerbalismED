# Rover Science Continued — Kerbalism Edition

>### **RSCKerbalismED (RSCKE) - A KSP Mod**

A compatibility mod that brings [**Rover Science Continued**](https://github.com/linuxgurugamer/RoverScience-Continued) experiments to [**Kerbalism**](https://github.com/Kerbalism/Kerbalism). **RSCKE** brings the necessary bundle for compatibility, Science balancing configurations and UI enrichment for playthroughs with RSC and Kerbalism. 

>RSC Science Analysis will produce the new RSCKE Experiment Samples to be stored in a Kerbalism Drive. Experiment was defined by having the famous "Surface Sample" has a baseline. </br>

| Project Details||
|:---|:---|
| **Project / Mod Name** | RSCKerbalismED |
| **License** | [Unlicense](https://unlicense.org/) |
| **Full Name** | RoverScience Continued — Kerbalism Edition |
| **Short Name** | RSCKE |
| **RSCKE General Settings** | RSCKerbalismED.cfg |
| **Kerbalism Experiment Config** | RSCK_RoverBrain_Experiment.cfg |
| **C# Namespace** | `RSCKerbalismED` |
| **Harmony ID** | `RSCKerbalismED` |
| **Log Prefix** | [RSCKerbalismED] |

></br>**🔗 RSCKE Dependencies**</br></br> - [**Rover Science Continued**](https://github.com/linuxgurugamer/RoverScience-Continued) — RSC provides the rover-based ScienceSpot discovery and sample-gathering gameplay that RSCKE integrates with Kerbalism.</br> - [**Kerbalism**](https://github.com/Kerbalism/Kerbalism) — Kerbalism provides the Science Experiment, Science Storage and related systems used by RSCKE. </br>

Adding `RSC` and `RSCKE` to your current `Kerbalism` run, should be smooth and error-free. </br>
</br>
Note: This mod alone provides no parts nor functionalities.
</br></br>
# 🟢 WHAT IT IS
</br>

### 📡 **RSC ↔ Kerbalism Integration** - *Compatibility and Configurability* </br>
**»** **RSC** `RoverBrain` parts now perform *Analysis* of a `Kerbalism Experiment` (configurable) </br>
**»** **RSCKE** safely handles `RSC` calls to **Stock's** `ModuleScienceContainer`, preventing errors </br>
**»** **RSC** `ScienceAnalysis` operation is intercepted by **RSCKE** which creates `Kerbalism samples` </br>
**»** **RSCKE** provides sample mass calculation based on `RSC` and delegates storage to `Kerbalism` </br>
**»** Configurable relation between **RSC** `ScienceSpot.Potentials` and  Science yield </br>
**»** Seamless integration with `Kerbalism's `**Science Archive** and **Vessels** `LaunchApps` features </br>
</br>

### 🧬 **Keeps the Best of RSC's gameplay** - *Adapted Sample Gathering Logic*  </br>
**»** Keeps RSC gameplay logic to find `ScienceSpots`, where samples can be collected  </br>
**»** Keeps RSC `ScienceSpot.Potentials` logic, so better spots yield heavier samples  </br>
**»** RSC `Science Analysis` operations now produce `Kerbalism Experiment` samples  </br>
**»** `RSCKE Rover Samples` will be stored in Vessel's available `Kerbalism Drives`  </br>
**»** Keeps `RoverBrain Terminal` for UI but with new information available  </br>
**»** Does not keep RSC `ScienceDecay` logic; Sample gathering is limited by Kerbalism</br>
</br>

### ⚖️ **Beyond Compatibility** - *Improvements & Emergent Rebalancing*  </br>
**»** No more infinite Science. Kerbalism Experiments define `SampleMass` (see config file)  </br>
**»** Analysis in a particular biome stops yielding Science after `SampleMass` is collected  </br>
**»** RSCKE adds its own flavor to each Sample mass calculation and lets you configure it  </br>
**»** Many improvements to RSC `Terminal GUI` (situation, biome and current samples representation)  </br>
**»** Fixes a few RSC inconsistencies *(ex: `Terminal` now closes when losing Full Control over vessel)* 

</br>

# 🔴 WHAT IT IS NOT
</br>

###  **RSCKE is NOT standalone** </br>
**»** This mod is only meant to be added to playthroughs with both RSC and Kerbalism </br>
</br>
### **RSCKE does not patch anything other than RSC** </br>
**»** Suggestions for changes are welcome, but they must remain in scope (RSC <-> Kerbalism relationship) </br>

</br>

# 🟡 WHAT ELSE
</br>

See [BACKLOG.md](Backlog.md) for current issues, planned improvements and ideas.
</br>

See [ABOUT.md](about.md) for project information, development notes, contributions and contact information.
</br>

See [License](LICENSE) for license information
</br>

Enjoy the mod! 🚀