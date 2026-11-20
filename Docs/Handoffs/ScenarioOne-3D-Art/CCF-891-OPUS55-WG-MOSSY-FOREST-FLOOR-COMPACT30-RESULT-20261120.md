# CCF #891: WG Mossy Forest Floor on Compact30, result

**Status: AUTHOR SMOKE PASSED (one non-WG Editor exception noted) / READY FOR OWNER VISUAL TEST / NOT INDEPENDENTLY REVIEWED / NOT MERGED**

| | |
|---|---|
| Actor | Anthropic Claude Opus 5.5 (`claude-opus-5-5`), Claude Code in T3 Code, jer-laptop UID 1000 |
| Packet | [#891 handoff](https://github.com/jermur788/CCF/blob/docs/891-opus55-ground-surface-pilot-20261120-v1/Docs/Handoffs/ScenarioOne-3D-Art/CCF-891-OPUS55-FAB-GROUND-ASSET-INSPECTION-AND-SINGLE-SURFACE-PILOT-20261120.md) at `2a492959` (WG-only, actual Compact30, minimum testing) |
| Base | `fce41bd3b77a054bf82d2521648d15ec18f81141` (#876). The #876 and #886 lanes were not touched. |
| Candidate | branch `art/ccf-891-ground-surface-pilot-20261120-v1`, HEAD `6ea568748541c4df95455ecba2a94c12623a1c7d`. It exists **only in the local repository** and has not been pushed. |
| Worktree | `/media/jer/ZX20/CCF-891-WGGround-20261120`. Its Library is already imported. |
| Board / lane | `CCF-891-WG-GROUND` (VERIFY) / `CCF891-WGGround`. Preflight READY (exit 0) before writing. |
| Unity lease | `d1ca0349f8ef4c719936c49b77bbc4ce`, acquired 22:16 UTC and released normally after the runs. The lock is **FREE**. |
| Evidence | `/media/jer/Files/CCF/Verification/CCF891-WG-Ground-20261120/` (local only; see licence note) |

## 1. WG identity (proven from the downloaded files)

The owner's folder is `/media/jer/ZX20/NewAssets/`. The one-word spelling is the real path; nothing was moved or renamed.

| | WG, used | VM, **not used** |
|---|---|---|
| Archive | `mossy_forest_floor_wgstfan_2k.zip` (41,496,148 B) | `mossy_forest_floor_vmclaaalw_2k.zip` |
| SHA-256 | `cf109ebbf841e0877a9eb52c2f5ac4606ba4f5e30426b5d7ad4b7324e2947041` | — |
| Internal ID (`wgstfan.json` `id`) | `wgstfan` | `vmclaaalw` |
| Name | Mossy Forest Floor | Mossy Forest Floor |

WG metadata, from `wgstfan.json`:
- Scan area is **2×2 m** and tileable in X and Y. Height range is 0.059 m. Texel density is 4096 px/m.
- Contents: moss, ground, pine needles. Origin: United States, forest biome. Process date 2024-04-16. The highest available resolution is 8K; the 2K set was used.

**Unknowns:**
- The Fab listing URL/UUID is not recoverable from the archive; the JSON has no licence or URL field.
- The licence/entitlement record was not in the folder.
- It is assumed to be the owner's Fab acquisition under the Fab licence. Owner confirmation is needed before anything beyond local use.

No VM file was extracted or imported. The other archives in the folder (pine floor, stumps, trunks) were listed but not opened.

## 2. What changed (two commits on the candidate branch)

| File | Change |
|---|---|
| `Assets/ForestPrototype/Scenes/ScenarioOneCompact30.unity` | **One line**: the `Ground` MeshRenderer's only material slot, `Sen1_SpruceFloor` `f3725908…` → `CCF891_WG_MossyForestFloor` `8e91166e…` |
| `Assets/ForestPrototype/Art/Experiments/CCF891/Materials/CCF891_WG_MossyForestFloor.mat` (+ .meta) | New URP Lit material |
| `Assets/ForestPrototype/Art/Experiments{,/CCF891,/CCF891/Materials,/CCF891/Textures}.meta` | Folder metas |
| `Assets/ForestPrototype/Art/Experiments/CCF891/Textures/.gitignore` | Keeps the WG bytes out of Git |
| `Tools/Verification/CCF891WGGround/{CCF891WGGroundSmoke.cs,README.md,build_candidate.py}` | Disposable harness and a local rebuild script |

**Not changed:**
- `Sen1_SpruceFloor.mat` and `Forest Ground.mat`.
- The Ground collider, transform and geometry; the outside continuation plane; H1 apron; path and verge.
- G1, trees, marks, spawn, C#, saves, Packages and ProjectSettings.

`Forest Ground.mat` was never on the Compact30 Ground at `fce41bd3`: the Ground is a 30×1×30 built-in Cube with `Sen1_SpruceFloor`.

**Material settings:**
- **Base map:** WG BaseColor with WG Gloss packed into alpha (sRGB, alpha from input). Smoothness source is Albedo Alpha, multiplier 1, which gives a mean smoothness of about 0.09, matte.
- **Normal and AO:** WG Normal is imported as a normal map, OpenGL convention, no green flip, strength 1. WG AO is linear, strength 1.
- **Texture size:** all maps 2048.
- **Tiling:** 15×15 on `_BaseMap`, i.e. a 2 m repeat on the 30 m face, at real-world scan scale. URP Lit uses the `_BaseMap` UV for the normal and AO maps too.
- **Not used:** Displacement and Bump are not used, so there is no physical relief.

**Licence/storage (D-032 open):**
- The WG JPGs, the packed PNG and their metas exist only in the local worktree and are git-ignored.
- The committed material and scene contain GUID references only.
- In another checkout the material shows without maps until rebuilt locally; see the tool README.
- The evidence captures render WG pixels, so they stay local and are not attached publicly.
- No WG content was given to any generative/AI model as input; only the JSON metadata and file listings were read.

## 3. Author checks (minimum, per owner)

The smoke harness ran twice in batch Play Mode on the actual Compact30 scene, with a task-local `XDG_CONFIG_HOME`. Run 1 was the cold import; run 2 added warm-up renders.

Run 2 (`run2/verdict.txt`): **19 PASS / 1 FAIL**.
- **Compile:** 0 `error CS`.
- **Import:** all three importer settings are as intended.
- **Material:**
  - Ground renders WG on supported URP Lit, with a 2048 base map, normal and AO present, and 15×15 tiling.
  - **Ground is the only renderer using WG**, so H1, path and verge keep their own materials.
- **Collider:** the Ground BoxCollider is unchanged, top y=0.000, 30.00×30.00.
- **Movement:** the player spawns at (2.00, 0.03, −8.00) and walked 1.02 m forward. The height change was −0.009, and Ground was under the player after the move.
- **FAIL:** one exception was logged, `ArgumentOutOfRangeException` in `UnityEditor.Search.SearchDatabase.EnumerateAll`, called from `SearchInit.IndexationOnStartup`. It is a Unity Editor QuickSearch indexing delay-call, identical in both runs, with no project or material frame in the stack. I attribute it to the fresh isolated Editor config, not to WG. **Not verified** against a baseline run without WG; that was deliberately skipped under the minimum-test rule.
- **C/X marking not run:** marking and picking use colliders, and the material swap changes no collider.

Shared Flatpak prefs are unchanged (SHA-256 `6f97cf31…` before and after). Unity re-saved some files on exit: 7 SectionFive `.mat`, `.vscode/settings.json` and the Packages auto-bump. All were restored to HEAD; the worktree is clean.

## 4. Paired captures (same camera and lighting, local only)

All in `run2/`. Each pair has `before-sen1sprucefloor` and `after-wg`, rendered in the same frame window with only the Ground material switched. The first pass of each camera was discarded as a shader warm-up.

| View | Camera |
|---|---|
| `walking-eye-*` | player view camera, (2.00, 1.68, −8.00), yaw 16.9°, looking along the footpath from H1 |
| `overview-h1-path-*` | (0.83, 9.03, −11.83), pitch 39.3°, yaw 16.9° |
| `down-close-*` | 1.65 m eye height, pitch 35.5° down |

**Author's visual notes** (not a quality judgement; the owner decides):
- WG reads as a darker, richer brown organic and needle litter with fine detail, replacing the lighter, flatter grey-brown Sitka floor.
- It is **less green than its name suggests**: the WG base colour averages a dark brown (sRGB about 49/36/15), with small moss flecks.
- The 2 m repeat is not obvious at walking eye; the overview may show it on close inspection.
- It does not look shiny or plastic, and there is no pink or missing texture.
- The H1 worn-earth apron and the path stay clearly distinct. The path verge now blends more softly into the ground.
- **The ground is still physically flat. WG is a texture, not relief or roots.**

## 5. Owner test steps

1. Open the Unity Hub project `/media/jer/ZX20/CCF-891-WGGround-20261120`, Unity 6000.6.0f1. The Unity lock is free; take it as usual if your workflow requires.
2. Choose **CCF → Player Test → Play Compact30**, walk around, and look at the floor near H1, the path and the stand.
3. A/B in the Editor:
   - Select `Ground` and set its Material to `Assets/ForestPrototype/Art/Sen1SpruceFloor/Materials/Sen1_SpruceFloor.mat` to see the original.
   - Set it back to `Art/Experiments/CCF891/Materials/CCF891_WG_MossyForestFloor.mat` for WG.
   - In Play Mode this change is temporary. If you make it outside Play Mode, don't save the scene unless you mean to.
4. Full rollback in Git: `git checkout fce41bd3b77a054bf82d2521648d15ec18f81141 -- Assets/ForestPrototype/Scenes/ScenarioOneCompact30.unity`.

## 6. Limits and next step

- **Not done:**
  - independent review;
  - second-scene reuse run;
  - performance profiling;
  - a GUI or real-input walk by me;
  - C/X marking;
  - Windows build;
  - merge or push of the candidate.
- **Reuse:** to reuse WG, assign the material and set the `_BaseMap` tiling to the surface size in metres ÷ 2.
- **Owner decisions:**
  - Is WG worth keeping?
  - If it is, the Fab URL and licence record are needed, and a D-032 storage/publication decision before the texture bytes can go to any shared repository.
  - A greener variant, tint or blend can be considered after your look.
- **Next milestone after owner feedback:** the separately scoped uneven, walkable ground and root prototype (SPEC REQUIRED). It is not part of this trial.
- **#886** is unaffected and remains owner-test-first.
