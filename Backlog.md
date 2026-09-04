# RSCKerbalismED — Backlog


### General issues
| Issue | Notes | Status |
|---|---|---|
| After confirming and storing sample, reset Science Spot | despawn dome and ROC | For 1st release |
| Remove RSC outputs for ScienceDecay - its not used  | For 1st release |
| RSC considers KSP Localization | consider those | For 1st release |
| Add PT/BR Localization | nice to have | 2nd release |


### Situation: RoverBrain Terminal GUI

| Issue | Notes | Status |
|---|---|---|
| Review buttons Upgrade, Reset | integration | For 1st release |
| Terminal keeps running even after losing vessel's Full Control | RSC bug | Fixed |
| Try to show (keep track) of how many spots were visited per biome | GUI - correction | For 1st release |
| Try to show (keep track) of collected samples quantity and mass | GUI - new | For 1st release |
| Show current biome and amount of mass gathered for current biome while ScienceSpot is not reached | GUI - new | For 1st release |
| Generate new GUI/screen for sample obtained result | GUI - new | For 1st release |

### Situation: Kerbalism Science Archive

| Issue | Notes | Status |
|---|---|---|
| Experiment not listed when filtering by current vessel | RSC part `roverBrain` still uses old Experiment Id. Parts might need to be patched| For 1st release|
| Science points balance according to Surface Sample across biomes | Kerbalism is somehow using different values per Body | Investigate|

### Situation: The "Auto" tab on Kerbalism "Vessel Pane"

| Issue | Notes | Status |
|---|---|---|
| Experiment not listed on "Vessel Pane Auto" | RSC part roverBrain still uses old Experiment Id. | For 1st release |

</br></br>

## Other Ideas for RSCKE 
> Just ideas that were discussed with a few people. Feasibility was not assessed.

- Compatibility with RP-1 / RO / RSS

- Compatibility with `ProbeControlRoom` and `RasterPropMonitor` (maybe control Rover from Mission Control and control experiment via MFDs. Or maybe, even, have the Rover Terminal itself in MFDs)

- Allow configurable Energy Consumption for `RoverBrain` parts

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

