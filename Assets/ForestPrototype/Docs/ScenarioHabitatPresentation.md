# Scenario One — Flora & Fauna Presentation v1

This is a **derived habitat presentation**, not a new ecological process. It
reads the existing tree, canopy/light, six understorey proxy, regeneration and
fallen-deadwood state. It does not write cells, change the annual Forestry step,
introduce wildlife populations, or enter a forest save. The frozen v12
Reference Future keeps its original schedule and Year-100 full-world hash
`7AD177B3CC2F73C7`; canonical Sitka stays `7E39B70A14959FAD`.

## What the player sees

`ScenarioHabitatVisuals` builds eight collision-free, procedural meshes from
the current 5×5 m ecology cells: moss/bryophyte carpet, shade fern,
bracken-*type* gap fern, grass, bramble-*type* shrub, dwarf-shrub *type*, generic
herbs, and fungi on **recorded** decaying logs. Shade and gap responses are
display interpretations of already-saved cover proxies, not additional
competitors or recorded species occurrences. They rebuild on load and annual
resolution, including frozen historical previews; no additional meshes are
persisted. Fallen logs become darker/mossier with their recorded decay class;
fungi appear only once there is a sufficiently decayed log.

There is **no ancient-woodland specialist palette** by default. Source
confidence defaults to zero. Suitable light alone does not imply woodland
continuity, dispersal, or colonisation by slow woodland plants. Soil acidity,
moisture/drainage and connection to old/native woodland have not been defined
for this site; generic herbs and shrub silhouettes must not be labelled as
confirmed ancient-woodland species, bilberry or bracken field observations.
Bracken is native in Ireland, not an invasive alien in this presentation.

## What the player hears

Seven independent, habitat-routed cues replace one increasing "biodiversity
birdsong" scale: standing-conifer birds; mixed-woodland birds; gap/edge birds;
understorey insects; canopy wind; occasional deadwood/woodpecker-type events;
and litter/fungi/decomposing-wood ambience. Woodpecker cues are intermittent
and depend on structural context; their volume does **not** increase linearly
with the number or volume of logs. Retained Sitka keep the conifer layer active
in the mixed Year-100 forest. **No audio is generated:** layers with no assigned
recording are silent. Add `ScenarioOneSoundscapePlayer` to the Scenario One
manager object in the scene before Play and populate its `clips` bindings with
your recordings and the `layerId` values in `ScenarioSoundscape.cs`. The seven
layers then crossfade the assigned clips on annual changes and reference loads.

## Historical-stand presentation check

The Play Mode gate `ScenarioHabitatPresentationVerification.Begin` walks the
same verified Reference Future stand at years 0, 20, 50 and 100, checks silent
unbound layers and each functional visual class, verifies the original frozen
archive hash, and verifies that returning from every preview leaves the
player's own full-world hash unchanged.

| Year | Derived presentation reading |
| --- | --- |
| 0 | 336 Sitka; moss/litter and sparse shade fern; conifer mix target dominates; no log fungi. |
| 20 | Strongly coniferous; local fern, bracken-type, grass and bramble-type responses; 17 logs. |
| 50 | More patch contrast, mixed-woodland audio target strengthens, fungi on older of 37 logs. |
| 100 | 162 Sitka plus 11 Oak/33 Beech, 70 logs; varied floor; both conifer and mixed-woodland mix targets remain. Not ancient Atlantic oakwood. |

Irish_CCF_ecology.pdf supplied the **broad habitat/process direction only**;
its unverified country-specific claims, taxa and numeric targets are not coded
as parameters or Scenario One objectives. The site definition and independent
Irish species/source verification remain prerequisites for a species-level flora
palette. Deer, invasive shrubs, causal bracken feedback, wildlife populations,
snags, epiphyte dynamics, hydrology and specialist fungal ecology are deferred.
