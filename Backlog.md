# RSCKerbalismED — Backlog

</br>

Please reach out if you stumble upon any issue that is not listed here, or if you have more ideas that could be implemented in RSCKE. 

Thanks for your help everyone!


> Most things achieved during "proof of concept" phase are not listed here as they are already implemented or under active development. This list helps me not to forget issues already identified.

### General Pending Issues

| Issue | Notes | Status |
|:---|:---|:---|
| After confirming and storing sample, reset Science Spot | Dome and ROC despawn  | Done |
| Remove RSC outputs for ScienceDecay but leave its usage for RSC | its not used by RSCKE | Done |
| RSC considers Localization | Inestigate | After 1st |
| RSC considers Localization - Add PT/BR Localization | nice to have | After 1st |
| Kerbalism Drive name used for samples storage is sometimes empty. only command parts seem to have names | nice to have | For 1st release |
| There's still a log mentioning 1 invalid access to ModuleScienceContainer | onVesselLoad | Done |
| Review remaining original RSC error logs and prevent them if feasible | only happened with cheats -> set position | Maybe leave it |

### Situation: RoverBrain Terminal GUI

| Issue | Notes | Status |
|:---|:---|:---|
| Terminal keeps running even after losing vessel's Full Control | RSC bug | Done |
| Show current biome while driving | GUI - new | For 1st release |
| Show (keep track) of how many spots were visited per biome | GUI - correction | For 1st release |
| Show (keep track) of collected samples quantity and their mass | GUI - new | For 1st release |
| Generate new GUI/screen for sample obtained result | GUI - new | For 1st release |
| Review button Reset - it seems there's no changes necessary (just do it after collecting sample) | integration | For 1st release |
| Review button Upgrade - Costs science to assure better science gathering | integration | (investigating)/after 1st |


### Situation: Kerbalism Science Archive

| Issue | Notes | Status |
|:---|:---|:---|
| Experiment not listed when filtering by current vessel | Patched parts | Done |
| Old Experiment still listed in experiments list | not anymore | Done |
| Experiment is listed but not shwoing experiment info | config | Done |
| Kerbalism shows Completed value "1.9x" meaning the player recovered 90% more of what Experiment limits | Science is still limited to "1x" | This is OK | 
| Balance Science across biomes based on a single value? | Kerbalism assigns different values per Body | After 1st |

### Situation: The "Auto" tab on Kerbalism "Vessel Pane"

| Issue | Notes | Status |
|:---|:---|:---|
| Experiment not listed on "Vessel Pane Auto" | Listed now. Changing "Auto pane" status, also changes TerminalGUI state | Done |

</br></br>

## Other Ideas for RSCKE 
> Just ideas that were discussed with a few people. Feasibility was not assessed.

- Compatibility with RP-1 / RO / RSS

- Compatibility with `ProbeControlRoom` and `RasterPropMonitor` (maybe control Rover from Mission Control and control experiment via MFDs. Or maybe, even, have the Rover Terminal itself in MFDs)

- Allow configurable Energy Consumption for `RoverBrain` parts, depending on current activity (actively scanning for science spots or not). DONE.

- The `Album` idea - RSC spawns objects ROC1 or ROC2 at science spots. If not too costly, maybe try to have RoverBrain parts recognize a camera in the vessel to take pictures with, when Sample is Gathered. The picture would be stored in the RoverBrain "Album" and visible in both Terminal GUI and MFDs. Too much? Oh, this could be interesting for Telescopes and Cameras in general, too.


## RSCKE ideas for other mods
> Just ideas that were discussed with a few people. Feasibility was not assessed".

- For now I can only think of `Tarsier Space Technology with Galaxies Continued`.
 That mod has a few telescopes that can gather science from photos. Seems like the same case, but with Transmissable Data instead of Samples.

</br>

### **For future Reference**
- [Tarsier Space Technology with Galaxies Continued](https://github.com/JPLRepo/TarsierSpaceTechnology)
- [ProbeControlRoom](https://github.com/tabakhase/KSP-ProbeControlRoom)
- [RasterPropMonitor](https://github.com/Mihara/RasterPropMonitor)
- [Realistic Progression One (RP-1)](https://github.com/KSP-RO/RP-1)

