# RSCKerbalismED — Backlog


### Situation: RoverBrain Terminal GUI

| Issue | Notes | Status |
|---|---|---|
| Terminal keeps running even after losing vessel's Full Control | RSC bug | Fixed |
| After confirming and storing sample, Science Spot needs to be reset | despawn dome and ROC | Priority |
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
> These are a few ideas that I talked about with a few people. Feasibility was not assessed and therefore they remain "ideas".

- Compatibility with RP-1

- Compatibility with `ProbeControlRoom` and `RasterPropMonitor` (maybe control Rover from Mission Control and control experiment via MFDs)

- Allow configurable Energy Consumption for `RoverBrain` parts

- The `Album` idea - RSC spawns objects ROC1 or ROC2 at science spots. If not too costly, maybe try to have RoverBrain parts recognize a camera in the vessel to take pictures with, when Sample is Gathered. The picture would be stored in the RoverBrain "Album" and visible in both Terminal GUI and MFDs. Too much? Oh, this could be interesting for Telescopes and Cameras in general, too.



## RSCKE ideas for other mods

- For now I can only think of `Tarsier Space Technology with Galaxies Continued`.
 That mod has a few telescopes that can gather science from photos. Seems like the same case, but with Transmissable Data instead of Samples.
> These are a few ideas that I talked about with a few people. Feasibility was not assessed and therefore they remain "ideas".

</br>

### **For future Reference**
- [Tarsier Space Technology with Galaxies Continued](https://github.com/JPLRepo/TarsierSpaceTechnology)
- [ProbeControlRoom](https://github.com/tabakhase/KSP-ProbeControlRoom)
- [RasterPropMonitor](https://github.com/Mihara/RasterPropMonitor)
- [Realistic Progression One (RP-1)](https://github.com/KSP-RO/RP-1)

