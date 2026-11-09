# #357 — Section Five material-drift triage and remaining Scenario One Unity qualification — RESULT

**Date:** 9 November 2026. **Executor:** Claude Sonnet 5.5 (`claude-sonnet-5-5`), provider Anthropic, in Claude Code inside T3 Code, local; effort level not exposed to the session. One owner.
**Binding packet:** `MANAGER-356-TO-357-MATERIAL-DRIFT-TRIAGE-AND-REMAINING-UNITY-GATES-20261109.md`. **Claim:** [#357](https://github.com/jermur788/CCF/issues/357).

## Verdict: **BLOCKED — STOPPED AT THE SIX-PATH BOUNDARY (two extra rewritten materials). NOT READY FOR INDEPENDENT #316.**

* **Triage passed every condition** and the six exact paths were restored (worktree only) before the lease. The material rewrites are byte-identical to the protected Workbench worktree **and** to hashes already committed in three older evidence files (Section 2). Their rendering effect is analysed statically only (Section 2.4): **no before/after render was made, so no visual-equivalence claim is made.**
* One lease, **five** Editor sessions, ~12 minutes. Real results on exact `045966d2`: Rack V2 persistence **v2 PASS (31 checks)**, **cold-start PASS (3)**, **Legacy40 PASS (7)**. **StandMapParity FAIL** twice (kept as FAIL; Section 3.2).
* After the fifth session (Legacy40 persistence) `git status` showed **eight** dirty paths: the six, **plus `Berry_Calyx.mat` and `Bilberry_Berries.mat`**. The packet says an extra dirty path is a hard stop; my guard script refused to restore (`REFUSE: dirty path outside the six`) and I ran **no further Editor**. `save03b` and every other remaining gate are **NOT RUN**. U3 was left dirty with those eight paths, not touched.
* No production fault, no C# error, no texture disappearance, no source/scene/package change. #287 Workbench still **NOT FULLY QUALIFIED** (strict runner exit 1 stays FAIL). #316 not started, nothing merged to `main`, no Player build.

## 1. Identities and admission

| Item | Value |
|---|---|
| Unity lane / branch / HEAD | `/media/jer/ZX20/CCF-U3`, `qualify/scenario-one-ui-lifecycle-20261109-v2` @ **`045966d2f8e26368e30a79f14a1e34167952b95e`** (unchanged; no source, scene, package or reference-archive change) |
| Report lane | `/media/jer/Files/CCF/Operations/CCF-FocusedSourceStage-20261109`, `integration/scenario-one-focused-playtest-20261109-v1`, entry tip `a3bba8e1…` (clean, remote equal) |
| Lock (actual ID) | **`b12cf343a4cb4a43b9919d382dacdc3c`**, task `CCF-357-MATERIAL-DRIFT-REMAINING-QUALIFICATION`; acquired 16:22:24Z, parsed from JSON metadata and verified with `status --json` (owner/task/lane/branch/head all True); released by that ID at ≈16:37Z (holder log DONE 16:37:29Z), `forced: False`; `status` → `free`, no Editor |
| Admission (fresh, all passed) | ZX20 36.14 GiB free (≥35), I/O 0.0 MB/s, 20-s monitor sampling (28/25/20 GiB), lock free, no Editor, licensed pack 27/27 hashes OK, user saves OK, scene/Packages/Reference Future v1 `.bytes`+`.meta` hashes equal to HEAD, original eight-dirty Workbench worktree identical (status + 8 hashes), Hub licence 14:42Z |
| Monitor | 47 samples, min **36.14 GiB**, 0 ALERT/STOP/KILL, exited on stop file; ZX20 36.15 GiB after |
| Evidence | `/media/jer/Files/CCF/Verification/CCF-357-MaterialDriftTriage-20261109/` (8.2 MiB, `evidence-manifest.sha256`) |

## 2. Material drift triage (read-only, before any restore)

### 2.1 Conditions checked
Registered U3 path; common store `/media/jer/Files/CCF/Repository.git`; exact HEAD/branch; no other owner, Editor or lease; `git status --porcelain=v1 -uall` = **exactly** the six ` M` paths (full paths from Git below), **nothing staged, deleted, renamed or untracked**; index blobs equal HEAD blobs. `.vscode/settings.json` SHA-256 `2ccabfd4f0aa2f1619b540b33200964a7c2dfce1425b9cd549306d91fc2dcacb` and its two-hunk diff (trailing comma after `"*.unity": "yaml"`; `CCF.slnx` → `CCF-U3.slnx`) equal the known values. Before-images, index state, per-path SHA-256/blob ids and the full `git diff HEAD` (text and `--binary`) are in `before-image/` and `triage/`.

### 2.2 The six paths

| Path (under `Assets/ForestPrototype/Art/SectionFive/` unless `.vscode`) | HEAD SHA-256 | Dirty (rewritten) SHA-256 | Byte-identical to protected Workbench dirty file? |
|---|---|---|---|
| `.vscode/settings.json` | `04d629e484c9…` | `2ccabfd4f0aa…` | n/a (that lane's solution name differs; this one matches #315/#356) |
| `Bilberry_Type_Cover_01/Materials/Bilberry_Leaves.mat` | `8ed7a9fca475…` | `106f237bf5d8…` | **yes** (`cmp`) |
| `Bilberry_Type_Cover_01/Materials/Bilberry_Stems.mat` | `ad636ff1c5bf…` | `1fd084251b82…` | **yes** |
| `Woodland_Forbs_01/Materials/Forb_Flower_Centre.mat` | `c5415205a3fe…` | `39385672532b…` | **yes** |
| `Woodland_Forbs_01/Materials/Forb_Leaves.mat` | `67467f78e289…` | `e2d888926519…` | **yes** |
| `Woodland_Forbs_01/Materials/Forb_Stems.mat` | `7e7d2207d862…` | `b32b7b5f507c…` | **yes** |

12-hex prefixes shown; full 64-hex values: `triage/six-path-hashes.txt`, `triage/six-path-head-sha256.txt`. The #356 receipt (`run/drift-after-gate2/materials-vs-workbench.txt`) says the same.

### 2.3 Property-level differences (complete: the whole diff is these lines)

| Material | Property | HEAD | Rewritten | Classification |
|---|---|---|---|---|
| `Bilberry_Leaves` | `_MainTex.m_Texture` | `{fileID: 0}` | `guid faef64c9fbaf6f40cbfaac720da31b28` | = the material's own `_BaseMap` texture; GUID resolves to `Bilberry_Type_Cover_01/Textures/Bilberry_Leaf_BaseColor.png.meta` |
| `Bilberry_Leaves` | `_AlphaToMask` | `0` | `1` | alpha-clip material (`_AlphaClip: 1`, `_Cutoff 0.35`) |
| `Forb_Leaves` | `_MainTex.m_Texture` | `{fileID: 0}` | `guid 06dab503099a8e602b514aa6f0410b02` | = its `_BaseMap` GUID; resolves to `Woodland_Forbs_01/Textures/Woodland_Forb_Leaf_BaseColor.png.meta` |
| `Forb_Leaves` | `_AlphaToMask` | `0` | `1` | alpha-clip material |
| `Bilberry_Stems`, `Forb_Flower_Centre`, `Forb_Stems` | `_Color` | e.g. `0.16 / 0.22 / 0.095` | `0.15999997 / 0.21999997 / 0.09499996` (similarly the others) | float round-trip of `_BaseColor`; error ≈ 3×10⁻⁸ per channel |

No other line changed in any material: shader GUID `933532a4fcc9baf4fa0491de14d08ed7` (resolves to the installed URP `Lit.shader`), render queue (2450/2000), `stringTagMap`, keywords, cull, cutoff, blend, `_BaseMap`/`_BaseColor` and every other texture slot are identical. All referenced GUIDs are existing project assets; **no unrecognised GUID, no unlisted property**. Extracts: `triage/material-semantic-extract.txt`.

### 2.4 Origin and rendering relevance

**Origin (established).** The installed URP `BaseShaderGUI` (`Library/PackageCache/com.unity.render-pipelines.universal@b9a66914c09e/Editor/ShaderGUI/BaseShaderGUI.cs`) copies `_BaseMap`→`_MainTex` and `_BaseColor`→`_Color` ("temporary fix for lightmapping", lines ~937–944) and sets `_AlphaToMask` from the alpha-clip state (`UpdateMaterialSurfaceOptions`, ~1127). The same five hashes (plus `Berry_Calyx`, `Bilberry_Berries` and the deadwood/fungi set) are already recorded as Unity-generated "import/generated" bytes, each restored to HEAD, in **three committed files in this tree**: `Docs/Verification/Enlarged80Calibration/serialized-restoration.json` (attribution "installed URP BaseShaderGUI SetMaterialKeywords/UpdateMaterialSurfaceOptions aliases"), `Docs/Verification/StormsWindthrowIntegration/editor-normalization.json` and `Docs/Verification/StartupScreen/Evidence/Audit/final-audit.json` (`restored_exact: true`). My script confirmed all five current U3 hashes equal those recorded generated hashes (`triage/historical-committed-match.txt`). So this is a long-known, reproducible, deterministic Editor rewrite, not user work.

**Rendering effect (static analysis only — NOT a visual verification).**
* `_MainTex` is not what URP/Lit samples (it samples `_BaseMap`, unchanged and set in HEAD); URP re-syncs `_MainTex` only for lightmap bakes/legacy lookups. Expected on-screen effect: none.
* `_AlphaToMask = 1` only changes output when the render target uses MSAA (it turns hard cut-out edges into alpha-to-coverage). Both URP assets in the project (`Assets/Settings/PC_RPAsset.asset`, `Mobile_RPAsset.asset`) have `m_MSAA: 1` (off) and `QualitySettings.antiAliasing` is 0 for both quality levels. Expected effect under shipped settings: none. A camera cannot exceed the asset's MSAA, so this is an inference from settings, not an observation.
* `_Color` differences are far below 8-bit colour resolution (3.9×10⁻³).
* **Timing observation (inference):** in the Legacy40 session all eight rewritten files carry the same mtime (16:34:54Z) while the Editor exited at ~16:35:05Z, and the log shows a final `Asset Pipeline Refresh … StopAssetImportingV2` with re-imports at shutdown. This suggests the rewrite landed at Editor shutdown, i.e. captures taken earlier in a session probably rendered the HEAD bytes. Not proven (the earlier-in-session re-imports could also have written them).
* **I made no screenshots in this task** and no before/after pixel comparison, so every material-dependent visual gate remains **NOT VERIFIED**. The 24 #356 Objective 1 captures are labelled *potentially post-Editor-material-state*: in #356 the rewrite followed that session, same shutdown pattern, but equivalence is not claimed.

### 2.5 Restores performed (worktree only; index unchanged, no `clean`/`stash`/`skip-worktree`)
Guard `restore6.sh` (exact six-path allowlist + known after-hashes + HEAD blob hashes; refuses with Unity running, anything staged/untracked/deleted, or any path outside the six). Receipts in `run/restores/NN-*/` (before-image tree, `before-restore.diff`, before/after SHA-256, status): `00-pre-lease` (6 paths, the authorised pre-lease restore), `01-after-standmap`, `02-after-standmap-r2`, `03-after-persist-v2` (4 paths), `04-after-persist-cold` (4 paths) — each a repeat of the same allowlist and known hashes while the lease was held, never inside a runner. `05` **REFUSED** (below).

## 3. Editor sessions (one lease, exact `045966d2`, strict-clean driver)

Driver: the #356 `RackV2Objective1/run_gate.py` generalisation (stages the disposable harness from the lane's own bytes, private Xephyr display, isolated `XDG_CONFIG_HOME`, licence copied then deleted, strict clean-lane precondition, post-verdict reaper), copied with only `TASK` changed and an optional `--seed-save` added for the cold-start seed. Wrapper `gate.sh` checks the protected hashes (saves, scene, Packages, Reference Future v1) before and after every session — **all five sessions: unchanged**. No staged harness or `.meta` remained under `Assets/ForestPrototype/`.

| # | Gate | Harness / env | Result | Notes |
|---|---|---|---|---|
| 1 | StandMapParity (Full vs Mini) — normal startup | `StandMapParity.Begin` | **FAIL** | 11 hard checks then harness NRE; 0 shots. See 3.2 |
| 2 | StandMapParity — direct entry (original runner's setup) | same, `CCF_TEST_DIRECT_ENTRY=1` | **FAIL** | 48 PASS / 5 FAIL then harness `InvalidOperationException`; 0 shots |
| 3 | Rack V2 persistence `v2` | `CCF_RACKV2_11_MODE=v2` | **PASS** `RACKV2PERSIST_VERIFY_PASS mode=v2 checks=31` | Section 3.1 |
| 4 | Rack V2 persistence `coldstart` | seeded from #3's partial-mark save + expectation | **PASS** `mode=coldstart checks=3` | new Editor process |
| 5 | Rack V2 persistence `legacy40` | `CCF_STAND_GEOMETRY=0` | **PASS** `mode=legacy40 checks=7` | then rewrote **8** paths — STOP |

All five logs: 0 `error CS`, 0 `VisualElementCreation`, 0 `ClearNextAction`; the only exception outside harness code is Unity's own `SearchDatabase` `ArgumentOutOfRangeException` (engine noise, also in #315/#356) and, in session 1 only, the harness NRE. (JSON-parse messages in session 3 are the deliberate malformed-save cases.)

### 3.1 Persistence evidence (the PASS gates)
* **v2 (31 checks):** new game writes a v24 disk save, Continue offered, reload exact; draft with non-contiguous ids `1,3,4,5,6` and `nextRackId` survives reload; confirmation save exact; partial entrance marks (racks 1,6) survive; all marks plus a second save cycle exact; **21 malformed V2 files refused by Continue and Load with no crash and no world change**; synthetic projected v20 Enlarged80 save loads as model 0, re-saves as explicit model 0, never becomes V2; v20/v21 model-0 saves with legacy rack progress stay ordinary work, never V2; v24 teaching-history flag round trips; **Reference Future v1 archive verified, Year-100 hash `7AD177B3CC2F73C7` untouched**.
* **coldstart (3):** a new Editor process continues the partial-mark V2 save and restores the exact rack plan, ids, orders, marks and world hash; teaching flags applied from disk.
* **legacy40 (7):** synthetic projected Legacy40 v20 and v21 saves load unchanged (geometry, signature, world hash) and never enter V2; Legacy40 files claiming V2 are refused with no crash; Legacy40 with legacy rack progress unchanged; Reference Future v1 verified again.
* The real player saves were never read or written (isolated config; before/after hashes equal).

### 3.2 StandMapParity — kept as FAIL, cause only partly classified
The harness is the pre-Rack-V2 B5/B8 gate and was last run before the V2 world; RACK-V2-08 explicitly records it as not rerun. Failures from session 2 (`run/standmap-r2/summary-lines.txt`):
* `MODEL_HAS_EVERY_LAYOUT_RACK lines=5 racks=0` — the harness assumes the authored five racks already exist in the model at game start; the V2 world starts with an empty rack plan (line 229). Stale assumption.
* `MINIMAP_TEXT_IS_ONLY_THE_NORTH_MARK` — the minimap now contains the production teaching note `HudMinimapView.TeachingNote` ("Dashed lines show suggested rack positions…"), which post-dates the harness. Stale assumption, but it is production text inside the Mini.
* `MINIMAP_DRAWS_PATH_CLEARING_RACKS_AND_YOU items=19` — depends on racks (above).
* **`PLAN_EVERY_MINIMAP_ITEM_HAS_A_FULL_MAP_COUNTERPART matched=9 missing=10 of 19` and `PLAN_PATH_RACK_AND_BOUNDARY_POINTS_ARE_AT_THE_SAME_FRACTIONS points=212 of 242` — NOT explained.** Counterparts are matched by symbol + source index + point count; I did not establish whether the cause is the stale rack assumption, V2/D-072 draw-plan changes, or a real Mini/Full divergence. Without a baseline run I cannot classify it, so **gate D is FAIL (as run), not "stale"**.
* Session 1 (normal startup) additionally failed `MINIMAP_EXISTS_AND_IS_SHOWN_ON_THE_WALKING_HUD`: an unreliable setup (startup screen), so session 2 reproduced the original runner's environment; both are retained.

## 4. The stop

After session 5, `git status --porcelain=v1 -uall` listed the six paths **plus** `…/Bilberry_Type_Cover_01/Materials/Berry_Calyx.mat` and `…/Bilberry_Berries.mat` (nothing staged/untracked). `restore6.sh 05-after-persist-legacy40` printed `REFUSE: dirty path outside the six: …Berry_Calyx.mat`, restored nothing, and the chain stopped before `save03b`. I then did **not** restore any path, run another Editor, or widen the allowlist myself.

Read-only facts about the two extra files (`run/stop-extra-paths/`, before-images and full diff kept):

| Path | HEAD SHA-256 | Dirty SHA-256 | Difference |
|---|---|---|---|
| `Berry_Calyx.mat` | `297bfa2377de…` | `b158d1e681f2…` | one line: `_Color` `{0.035, 0.055, 0.095}` → `{0.035, 0.054999977, 0.09499996}` |
| `Bilberry_Berries.mat` | `c92681e85407…` | `f21652fc10e1…` | one line: `_Color` `{0.045, 0.07, 0.035}` → `{0.044999972, 0.06999998, 0.035}` |

Both dirty hashes equal the `generated_sha256`/`import_sha256` already committed in `Docs/Verification/Enlarged80Calibration/serialized-restoration.json` and `StormsWindthrowIntegration/editor-normalization.json`; the change is the same sub-10⁻⁷ `_Color` float round trip as the three stem/centre materials. They are **not** in the Workbench lane's dirty set (that lane has only the five). Which materials a session rewrites varies: sessions 1–2 rewrote all six paths, sessions 3–4 four (`.vscode` + three Forb), session 5 eight. That variability is why the set cannot be a fixed allowlist without a Manager ruling.

## 5. A–O on exact `045966d2` (PASS / FAIL / NOT RUN / NOT VERIFIED)

| Gate | Result | Evidence / note |
|---|---|---|
| A Import/compile | **PASS** | 0 `error CS` in the two #356 and these five Editor logs |
| B V2 surface lifecycle | **NOT RUN** (persistence-level V2↔non-V2 separation PASS in 3.1) | no surface create/clear/recreate transition test |
| C Map geometry vs 3D | **NOT RUN** | no paired capture |
| D Full vs Mini invariance | **FAIL (StandMapParity, partly stale harness, unresolved)** | Section 3.2; offline headland 92/92 from #356 is source-level only |
| E Legend / paths | **NOT RUN** | no captures this task |
| F Performance | **NOT RUN** | |
| G Five-rack diagram | **NOT RUN** | no capture, no grayscale |
| H Old-world diagram guard | **NOT RUN** | Legacy40 *persistence* PASS is not the diagram guard |
| I Card fit / guidance | **NOT RUN** | `UxPedagogyShots` not run |
| J Native controls / safe focus | **NOT RUN as a gate**; Objective 1 native route PASS (inherited #356) | Help-close/focus order untested |
| K Work Plan / rack boundary | **PASS for the Objective 1 route only** (inherited #356); MFM boundary **NOT RUN** | |
| L Workbench retirement | **107/107 in-Editor PASS; strict runner exit 1 = FAIL** (inherited #356, unchanged) | not rerun, not qualified |
| M CTA / feedback checker | **PASS, source-static** (inherited #356 10/0/2) | #309 independent status not assessed |
| N Save/load compatibility | **PARTIAL — PASS: Enlarged80/V2 disk save/load, malformed refusal, cold start, old-save projections, Legacy40, Reference Future v1 archive (3 sessions, 41 checks). NOT RUN: `save03b` (35-check schema gate), real Legacy40 saved-world/site-mask fixtures, Enlarged80 annual** | |
| O Final integrated regression | **NOT RUN** | |
| Also: native Reference Preview return pose | **NOT RUN** | |
| **All material-dependent visual items** | **NOT VERIFIED** | Section 2.4 |

Carried unchanged from #356 at their exact scoped statuses and **not rerun**: UI startup gate PASS; Workbench 107/107 in-Editor PASS (runner FAIL); Objective 1 end-to-end `RACKV2_O1E2E_PASS checks=173 shots=24`.

## 6. Protected state after release
Lock free, no Editor, monitor stopped (min 36.14 GiB, 0 alerts). U3: HEAD/branch `045966d2`/`qualify/scenario-one-ui-lifecycle-20261109-v2`, **dirty with exactly the eight paths above, deliberately unrestored**, nothing staged/untracked; Library 6.6 GiB untouched; licensed pack 27/27; scene, Packages, Reference Future v1 `.bytes`/`.meta` hashes equal HEAD; real saves OK; original eight-dirty Workbench worktree unchanged (status + hashes); no harness or `.meta` left in `Assets/ForestPrototype/`; licence copies removed by the driver.

## 7. Decisions needed from Manager #200
1. **Two-path extension.** May the allowlist be extended by exactly `Berry_Calyx.mat` and `Bilberry_Berries.mat` (hashes above, historically documented, sub-10⁻⁷ `_Color` change, no shader/texture/alpha change)? If yes the same guard can run with an eight-path allowlist; if some other session rewrites a further file the stop recurs. Alternatively cap the remaining run to harnesses already shown to touch only the six (sessions 1–4).
2. **StandMapParity.** Treat as a stale V2-era gate to be retired/updated by its own task, or require an independent baseline run to classify the unexplained 9/19 counterpart mismatch before gate D can be called anything but FAIL.
3. **Visual items.** Material-dependent rendering needs either an explicit before/after capture pair or an accepted static argument (Section 2.4); I recommend a short before/after capture pair inside the next lease, since MSAA-off is an inference.
4. **Remaining gates still to run (no new code needed, harnesses exist in the tree):** `save03b`; `RackV2Headland` qualification (124) and access (8); `RackV2Combined` write/continue + model-0/Legacy40 regression; `UxPedagogyShots` (card fit, 1280×720 and 1920×1080, colour); `ScenarioOneFiveRackDiagram` live render, headland Full-vs-Mini and old-world guard have **no existing Unity harness** (would need new code, not authorised here); performance has none.

#316 must **not** start automatically. #287 full qualification remains Manager-held. Nothing merged to `main`; no Windows/Linux Player build.
