# Ultimate Nature – Starter — local setup (required for ForestTest visuals)

The polished tree, stump and understory visuals come from the Asset Store pack
**Ultimate Nature – Starter** by **Innerverse Interactive**.

This pack is a **locally acquired dependency**. Its raw assets (FBX, textures,
materials, prefabs, readme) are deliberately **not part of this repository**
and must never be committed. The whole `Assets/InnerverseInteractive/` folder
is gitignored.

## One-time setup per contributor

1. Acquire *Ultimate Nature – Starter* on the Unity Asset Store.
2. Import it into this project (Package Manager → My Assets → Import), default
   options. It unpacks to `Assets/InnerverseInteractive/Ultimate Nature – Starter/`.
3. Nothing else is needed — the folder stays local and untracked.

## Why scene references still resolve

Unity `.unitypackage` files embed each asset's original GUID. Importing the
same pack therefore reproduces identical GUIDs on every machine, so the
prefab references serialized in `Assets/Scenes/ForestTest.unity` resolve
without any pack files living in the repository.

## GUID verification checklist

The scene and the runtime code reference exactly these pack assets by GUID.
After importing, open each `.meta` in the pack and confirm the GUID matches.
If a GUID differs you imported a different pack version — the visuals will
not link; check back with the project chat before substituting anything.

| Asset (under `Ultimate Nature – Starter/`) | GUID |
|---|---|
| `Environment/Trees/Fir/Prefabs/UNS_Spruce_01.prefab` | `1f9036ec905b920479091aca9ba81305` |
| `Environment/Props/Logs/Prefabs/UNS_Stump.prefab` | `3aa54be08e092174ab6b689e524fdf44` |
| `Environment/Vegetation/Bushes/Prefabs/UNS_Bush.prefab` | `3f8eda438d8aeab4d9223c5b53f5ef62` |
| `Environment/Vegetation/Flowers/Prefabs/UNS_Flower.prefab` | `43999c511a596754fb989ff074c5955d` |
| `Environment/Vegetation/Grass/Prefabs/UNS_Grass.prefab` | `4db940c31d61ac149801c850fc645d68` |
| `Environment/Vegetation/Mushrooms/Prefabs/UNS_Mushroom_Patch.prefab` | `d0fa3c49b2c9cd84bb5a644c53ef025e` |

All 68 stand trees serialize `UNS_Spruce_01` as the visual and `UNS_Stump` as
the felling swap. `UNS_Spruce_02` ships with the pack but is not referenced.
The editor-side `ForestSceneBuilder` also loads these prefabs by path, which
resolves automatically once the pack is imported at this location.

## Design notes (do not regress)

- The UNS visual is an additive override only; `ForestTree`'s authoritative
  biological state is untouched, invisible placeholder trunk/canopy proxies
  remain as interaction/simulation objects, and visual scale follows
  `heightMeters`.
- Understory scatter reads `ForestEcologyController.Cells[].Light`.
  Thresholds [D]: grass ≥ 0.50, flowers ≥ 0.55, bushes ≥ 0.65; mushrooms may
  establish anywhere on the needle litter.
- Scatter is currently static; re-run it from `ForestSceneBuilder` after
  canopy changes. Forestry state remains authoritative — never turn this into
  a second ecological simulation.
- Decorative UNS log props were removed on purpose (moss baked into the mesh
  reads as a glitch at distance). Do not restore them without asking.
