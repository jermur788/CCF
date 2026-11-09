# #358 — Finite URP material envelope, V2 Stand Map parity witness and remaining Scenario One Unity evidence — RESULT

**Date:** 9 November 2026. **Executor:** Claude Sonnet 5.5 (`claude-sonnet-5-5`), provider Anthropic, in Claude Code inside T3 Code, local; effort level not exposed to the session. One writer, no fan-out.
**Claim:** [#358](https://github.com/jermur788/CCF/issues/358) ([RUNNING comment](https://github.com/jermur788/CCF/issues/358#issuecomment-6745997062)). **Binding packet:** [`MANAGER-357-TO-358-…-20261109.md`](https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Overnight-Playtest-Readiness-20261107/MANAGER-357-TO-358-FINITE-URP-DRIFT-V2-MAP-PARITY-AND-REMAINING-UNITY-PACKET-20261109.md).

## Verdict: **NEEDS CHANGES — scoped evidence only. No product defect found; the integrated candidate is NOT QUALIFIED and #316 is NOT ready to start.**

* **The StandMapParity mismatch is explained, not a defect.** On the real canvases the 10 unmatched minimap items and 30 points are *precisely* the five intentional V2 suggested-rack bands (4 pts each) and five dashed centre lines (2 pts each), each at its own foundation route's place. A new V2-specific witness passes (**59 checks**, two viewports); the **historic `StandMapParity` FAIL ×2 is kept unchanged** (it is outdated for the V2 world, not edited or relabelled).
* **The Section Five material rewrites made no visible difference.** A same-frame, same-camera render of the committed-state, the generated-state and the live Editor-session materials is **pixel-identical (0 differing pixels, noise floor 0)** in 4 views × 2 resolutions. This is an in-memory emulation under the project's MSAA-off URP asset (Section 2.4), not an on-disk swap; it is evidence, not a Manager acceptance.
* **Remaining gates were mixed.** PASS: Rack V2 headland 124 + access 8, combined write 35 / continue 46, model-0 progression 21, Legacy40 progression 15, MFM07 approval boundary 106, frozen Reference Future v1 (canonical Legacy40 environment), 80B/B3 Reference Preview return pose (12 checks). **FAIL (strict, kept):** `save03b` (stale save-version pin) and `UxPedagogyShots` (19 distinct stale/fixture checks × 2 sizes; the Objective-card fit checks themselves pass). **NOT RUN:** B surface lifecycle transitions, paired Full-map/3D capture, performance, five-rack diagram live render, old-world diagram guard, native Help-close/default focus.
* Exactly **one** lease, **17** Editor sessions, ~43 min, lock released owner-safely. Every post-session dirty set was a subset of the finite 12-path envelope with matching hashes; no unknown path ever appeared. No game source, scene, package, save, Reference archive or licensed-pack change. No Player build, no `main` merge, #316 not started.

## 1. Identities and admission

| Item | Value |
|---|---|
| Tested game source | `045966d2f8e26368e30a79f14a1e34167952b95e`, unchanged (0 files differ under `Assets`, `Packages`, `ProjectSettings` between it and this report branch) |
| Unity lane | `/media/jer/ZX20/CCF-U3`, branch `qualify/scenario-one-ui-lifecycle-20261109-v2`, common store `/media/jer/Files/CCF/Repository.git`, warm 6.6 GiB `Library` reused |
| Report lane | `/media/jer/Files/CCF/Operations/CCF-FocusedSourceStage-20261109`, branch `integration/scenario-one-focused-playtest-20261109-v1`, entry tip `b4f744d74651ec42915403dac5c560a1a9096334` (clean, equal to origin) |
| Verification-only commit (separate) | `f2a69e3ef97dc23aceb7b776a7002579f2479aca` adds `Tools/Verification/StandMap/V2Witness/` (5 files, byte-identical to the evidence copies that were staged and run; SHA-256 in Section 9). **It changed no game source.** |
| Lock (actual ID) | **`9648fc88e1c146d1a3cca35dfe8c5b9e`**, task `CCF-358-V2-MAP-PARITY-FINAL-UNITY-EVIDENCE`, acquired 17:06:29Z, released 17:49:14Z by that ID, `forced: False`; `status --json` after acquire: owner/task/lane/branch/head all `True`; after release `free`, no Editor |
| Fresh admission (all passed) | ZX20 36.15 GiB (≥35), write I/O 0.0 MB/s, lock free, no Editor process, Hub licence refreshed 16:42Z (every Editor started; no exit 198), licensed pack 27/27 SHA-256 OK, user saves OK, scene/Packages/Reference Future v1 `.bytes`+`.meta` hashes = HEAD, original eight-dirty Workbench worktree identical (status + 8 hashes) |
| Disk monitor | 20 s interval (28/25/20 thresholds): 130 samples, **min 36.13 GiB**, 0 ALERT/STOP/KILL, final 36.14 GiB |
| Unrelated state preserved | primary checkout `ops/workflow-foundation-v1` has user changes (`AGENTS.md`, `Docs/Operations/StorageArchitecture.md`): untouched. `active-tasks.md`: untouched (Manager only) |

## 2. The finite 12-path material envelope

### 2.1 The manifest
Read from the committed `Docs/Verification/Enlarged80Calibration/serialized-restoration.json` at `045966d2` (blob SHA-256 of the file: `6c5eb3ee8680f4fc754254cdb4b65367e7b3444a350851b1623977cc021298c2`): **12 records = 1 generic `.vscode/settings.json` (not used) + exactly 11 `.mat`**. Cardinality checked = 11; for every record `canonical_sha256` equals the file at `045966d2`'s HEAD blob. [`Docs/Verification/StormsWindthrowIntegration/editor-normalization.json`](https://github.com/jermur788/CCF/blob/045966d2f8e26368e30a79f14a1e34167952b95e/Docs/Verification/StormsWindthrowIntegration/editor-normalization.json) independently contains 11 of the 12 generated hashes (the 12th is the settings value that file never recorded, as expected). All under `Assets/ForestPrototype/Art/SectionFive/`.

| Material (`…/<Package>/Materials/`) | HEAD / canonical SHA-256 | Exact generated SHA-256 |
|---|---|---|
| `Berry_Calyx.mat` | `297bfa2377de1387d9d403ac7dfe12871c89eb96c83e8599ef373610e4b422b2` | `b158d1e681f2ba19b0918613bed432936760ac1e63ace67d6f29a3892da72a55` |
| `Bilberry_Berries.mat` | `c92681e854071a682487051a86d1b542170042857d38969144a4a9c583d0a678` | `f21652fc10e1ac130eae6fc008d1ee31b79186c3f3dcbd9bd5f35a915df7a40d` |
| `Bilberry_Leaves.mat` | `8ed7a9fca4756ce27d6d5ea6054021d95b725bf39ae2d8c5b1c50e1dec087d2b` | `106f237bf5d8bcbb3d05cfc4c3eae57e6e90eaee729114bde15485a4cc81d3fd` |
| `Bilberry_Stems.mat` | `ad636ff1c5bf8e4a8838d2e10ff70be01b4b26403d75ce5af07eb65e7b0a6976` | `1fd084251b82cc78b44e9458aed2d031c91e97ccc6df7d069b5c317f6554906f` |
| `Bracket_Cap.mat` | `117888a2da6225b40022a4d7a3ae96082074b507b46bd8bd5be096a1a3d5b080` | `47639d95b9074d435c2953080e3b0073a9d2cd305eaa62409295eb2d88586701` |
| `Bracket_Pores.mat` | `beaad27a17af5d3d6ca445fde44fb72da34b7c4f214356253aa32042d348e5e8` | `d5761d7ac8780df5929d6f37235ce19e4540ae73a24faa2b86a8d2f9e1630077` |
| `Fungi_Cap.mat` | `6125be65f87301d1db9a30f20325bb47d0f52e989ace8b41f3d242ebe5556429` | `ff4ebdd76b76076c68ce09851576e3d6e91a7f895e3ce8ab444c66feca831db2` |
| `Fungi_Underside.mat` | `28d77be4f0b3597853ca47a6756a00eecb539273cb44d7d8bca2408b12637186` | `d2903a3b52a969651119451f65092adc3b467a67285096772af751bafbd95fd9` |
| `Forb_Flower_Centre.mat` | `c5415205a3fe1ce6b6b2aa9f8b62eef84798f0d21687e895f9ed6b9d268e8d84` | `39385672532b45322cee228e43886fc6006814e44f466ca228e1d4f6c8483774` |
| `Forb_Leaves.mat` | `67467f78e289d2e4c762ce719909cecd370b7796bc0690e2d016d52fd5dfce15` | `e2d8889265191156241a16516c28d30255be9ffaa127dbcfc4f2da920fb17a41` |
| `Forb_Stems.mat` | `7e7d2207d8628c5a299748ae6abdfe144aae0fd45d06d7a8b5abf1a9944e2471` | `b32b7b5f507c4f1dc049a08d97ca48beea36e7027eed0eeb6afdb41f2eb4dcd3` |
| `settings.json` | `04d629e484c9278cf6b52e6051790e93278c0c6a08b5f3dc93dfedbe67cd8b5a` | `2ccabfd4f0aa2f1619b540b33200964a7c2dfce1425b9cd549306d91fc2dcacb` |

The last row, `settings.json`, is **U3's own `.vscode/settings.json`** (the separate 12th exact allowed path): HEAD `04d629e4…`, known U3 generated hash `2ccabfd4f0aa2f1619b540b33200964a7c2dfce1425b9cd549306d91fc2dcacb`, whose diff is exactly two hunks (trailing comma after `"*.unity": "yaml"`; `CCF.slnx` → `CCF-U3.slnx`). The ledger's old generic VS Code hash (`efd12103…`) was **not** used.

### 2.2 Entry state and preservation (before any restore)
`git status --porcelain=v1 -uall` at entry listed exactly **eight** ` M` paths (settings + `Berry_Calyx`, `Bilberry_Berries`, `Bilberry_Leaves`, `Bilberry_Stems`, `Forb_Flower_Centre`, `Forb_Leaves`, `Forb_Stems`); nothing staged, deleted, renamed or untracked; index unchanged. **All eight SHA-256 values equalled the manifest's generated values**, each HEAD blob equalled its canonical value, and the property-level diff was only `_Color` float round-trips plus `_MainTex` and `_AlphaToMask` on the two leaf materials. Full before-image bytes (`before-image/tree/`), the full `git diff HEAD` (text and `--binary`), per-path SHA-256 and blob ids, and the entry index listing were saved to Files **before** the first restore.

### 2.3 Restores (worktree only, never inside a runner)
One guard script (`restore12.sh`): refuses with an Editor running, anything staged/untracked/deleted, a path outside the twelve, a dirty hash ≠ the known generated value, a HEAD-blob mismatch, a manifest cardinality ≠ 12, or a settings diff that is not the known two-hunk diff; saves a before-image, diff and hashes; restores exactly the dirty named paths with `git restore --source=HEAD --worktree`; then re-checks clean status and an unchanged index. It ran **18 times** (once at admission, 17 times after each Editor process had exited); **it never refused**. Receipts: `run/restores/*`. The dirty sets Unity produced, by session:

| Rewritten set | Paths | Sessions |
|---|---|---|
| settings only | 1 | B3 plain |
| settings + 3 Forb (`Forb_Flower_Centre`, `Forb_Leaves`, `Forb_Stems`) | 4 | V2 witness, save03b, save03b diagnostic, combined-write, model-0, MFM07, reference (invalid env) |
| above + `Bilberry_Leaves`, `Bilberry_Stems` | 6 | headland qualification, headland access, combined-continue, UxPedagogyShots |
| above + `Berry_Calyx`, `Bilberry_Berries` | 8 | material pair ×3, Legacy40 progression |
| above + `Bracket_Cap`, `Bracket_Pores`, `Fungi_Cap`, `Fungi_Underside` | 12 | frozen reference (canonical env) |

So the **whole finite envelope was observed**, including the four Deadwood_Fungi materials, and which materials Unity rewrites follows which assets a session loads. No other path appeared in any of 17 sessions. U3 ends **tracked-clean at `045966d2`**; the eight entry-dirty paths were restored by the first guard run (their before-image remains in evidence). The original Workbench worktree (eight dirty paths) was never touched and was re-verified identical after release.

### 2.4 Visual evidence: committed-state vs generated-state vs live (`SectionFiveMaterialPair`)
Method (new, bounded, verification-only; Section 9): the three flora prefabs the game itself uses (`RecentAssetVisualCatalog`: Bilberry cover, Woodland Forb rosette, Woodland Forb flowering, all seven target materials) are staged at one fixed place in the live world; one camera (settings copied from the player camera, fixed pose per view, LOD0) renders into a render texture at 1920×1080 and 1280×720; per view, in the **same frame**, three states: **committed** (in-memory material built from the committed `.mat` bytes), **generated** (built from the Unity-generated bytes saved in the entry before-image), **committed again** (noise floor), then the **live Editor-session material**. Nothing is written to any asset. Values are parsed from the files, not typed in; the harness asserts that each in-memory state equals its file for every property that differs.

| Result | Value |
|---|---|
| Views | overview, Bilberry cover, Forb rosette, Forb flowering × 2 resolutions = 8 |
| Noise floor (committed vs committed again) | **0 differing pixels** in all 8 |
| Committed vs generated | **0 differing pixels, max channel difference 0** in all 8 |
| Live Editor state vs committed / vs generated | **0 / 0** in all 8; saved PNG pixel data byte-identical for every triple |
| Harness checks | 40 PASS (`MATPAIR_CAPTURE_PASS shots=24 checks=40`); colour + grayscale at both resolutions kept (`screens/matpair-gray`) |

What this shows and does not show:
* **Shows:** for these views, under the project's own URP asset (`PC_RPAsset` `m_MSAA: 1` = off; `QualitySettings.antiAliasing` 0), the generated `_MainTex`, `_AlphaToMask` and `_Color` values render identically to the committed ones. Forb flowering and rosette views are clear (leaf cut-outs, stems and flower centres in frame); the Bilberry view is partly crossed by one spruce branch; the overview is mostly stand and the flora is small — the three close-ups carry the evidence.
* **Does not show:** behaviour with MSAA on (`_AlphaToMask` only matters with MSAA, and the project cannot enable it), other places/lighting, or that an on-disk rewrite is accepted — the Manager's acceptance of the operational classification is still required.
* **New finding 1 — the leaf rewrite is already live in memory at session start.** For `Bilberry_Leaves` and `Forb_Leaves` the live in-memory material equals the **generated** bytes exactly (`_MainTex` set to the `_BaseMap` texture, `_AlphaToMask` 1) when the probe ran, a few seconds after the scene loaded and before its first render, so #357's "rewrite landed at Editor shutdown" inference is **wrong for these two in this session**. By the same mechanism earlier sessions' captures (for example #356's 24 Objective 1 captures) very probably also rendered the generated state; that is not verified there, and this pair shows the rendered difference is zero in any case.
* **New finding 2 — the `_Color` change is Unity's own read-back.** Applying the *committed* `_Color` to a material and reading it back returns the *generated* value (e.g. `Berry_Calyx` r 0.045 → 0.044999972, g 0.07 → 0.06999998), and one more round returns the next one (0.04499995). The five `_Color`-only rewrites are therefore a deterministic ~3×10⁻⁸ float round-trip by the Editor, below 8-bit colour resolution (3.9×10⁻³).
* **Correction to #357's report:** its property table swapped the two berry rows. Committed `Berry_Calyx` `_Color` is `{0.045, 0.07, 0.035}` and committed `Bilberry_Berries` `_Color` is `{0.035, 0.055, 0.095}`. SHA-256 values and all conclusions are unaffected.

Two earlier runs of the probe failed on **harness defects** and are preserved (`run/matpair`, `run/matpair2`, `probe/*.run1/run2-defective.cs.txt`): run 1 compared colours with Unity's approximate `Color ==` (hiding the 3×10⁻⁸ differences) and assumed the live material was the committed state; run 2 asserted bit-exact colour read-back that Unity does not provide. Run 3 (`run/matpair3`) is the valid one and is the only one counted.

## 3. StandMapParity: attribution of the 10 missing items / 30 points

### 3.1 Phase A (offline, before any Unity)
* Source read at `045966d2`: `StandMapSource.ReadRacks` puts the foundation's five suggested routes into `model.MiniGuidance` while the Stand Map is not planning (`ShowRackSuggestions` false) and into `model.Racks` only while planning; `StandMapDrawPlan.BuildStatic` draws, for every `MiniGuidance` route and **Mini only**, a `RackBand` (4 points, `SourceIndex` −1) and a `RackSuggested` line (2 points, −1). RACK-V2-07 recorded this as an intentional superseding of the RACK-V2-04 "minimap does not expand" rule (`StandMapOfflineCheck.cs`).
* The old `StandMapParity.WholePlanParity` matches by symbol + `SourceIndex` + point count, **many-to-one**, and requires every minimap item to have a Stand Map counterpart; it also assumes the authored five racks exist at start and that the minimap's only text is "N".
* An offline reconstruction on the real production draw-plan code (`StandMapV2WitnessOfflineCheck`, plain .NET): honest plan → **19 minimap items, 10 Mini-only items, 30 Mini-only points**; the historic counting on the same plans returns exactly **`matched=9 missing=10 of 19`** and **`points=212 of 242`**. The path model used (two 52-point pieces) was chosen to fit the historic counts and was then **confirmed** by the live inventory below. The witness logic also **fails 17 defective plans** (missing shared item, shifted point on either canvas, both canvases shifted together, extra/missing/misplaced/solid guidance, suggestion drawn on the Stand Map, headland on the minimap, missing player dot, unexplained extra or missing Full-only item, foundation/model disagreement) and one **duplicate-Mini-item** plan that the historic many-to-one counting accepts.
* Offline analysis did not contradict the hypothesis, so Unity was admitted.

### 3.2 Real canvases (session 1 of the lease; V2 new game; `STANDMAP_V2_WITNESS_PASS shots=6 checks=59`)
Read from `StandMapCanvas.LastPlan` of the walking-HUD minimap and the Stand Map, at 1920×1080 (sides 195.83 / 687.50 px) and 1280×720 (195.00 / 687.50). Both canvases painted model version = current; rack mode off; racks 0; plan not confirmed.

| Count | Value (identical at both sizes) |
|---|---|
| Minimap items / Stand Map items | **19 / 32** |
| One-to-one matched shared items | **9** (`Landmark`, 2 × `PathCasing` + 2 × `Path` (two 52-point path pieces), `Property`, `NorthArrow`, `Player` (8 pts), `PlayerDot`) |
| Mini-only items / points | **10 / 30** = 5 × `RackBand` (4 pts) + 5 × `RackSuggested` (2 pts), all `SourceIndex` −1, dashed lines |
| Stand Map-only items | **23** = 14 `GridLine` + 1 `ScaleBar` + 8 existing headland/feeder strokes and pads (4 `PathCasing` + 4 `Path`, `SourceIndex` −1) |

Per-item identities (each Mini-only band and line found at its own place, fractions computed here from the property bounds, not through `StandMapTransform`; entrance x −35.40 m, far x 40.00 m, width 2.4 m; foundation routes from `GetSuggestedRackLayout()` equal `MiniGuidance[i]` exactly):

| Route | z (m) | Fraction y (north = 0) | Mini-only items |
|---|---|---|---|
| 0 | −35.27 | 0.94089 | `RackSuggested` + `RackBand` |
| 1 | −16.42 | 0.70527 | `RackSuggested` + `RackBand` |
| 2 | 0.24 | 0.49702 | `RackSuggested` + `RackBand` |
| 3 | 15.79 | 0.30258 | `RackSuggested` + `RackBand` |
| 4 | 35.23 | 0.05960 | `RackSuggested` + `RackBand` |

Other results at both sizes: the Stand Map holds **no** `RackSuggested`; the minimap draws **none** of the existing headland (`Path`/`PathCasing` −1 count 0; minimap `Path` count = path pieces); shared points **212/212 at the same fractions on both canvases (worst 6.7×10⁻⁸)** and **430 independent-expectation checks (worst 6.7×10⁻⁸)**; the Full-only set equals the model-derived expectation exactly. Screenshots (colour in `run/v2witness/evidence/v2witness/`, grayscale in `screens/witness-gray`) show five dashed routes with the teaching note on the HUD minimap, and a Stand Map with the L-shaped existing headland/feeder and no rack lines; the Overview panel inside the Stand Map is itself a Mini presentation and shows the dashed routes, as designed.

**Informational (not gated):** with Plan Racks on, the five routes move into `Racks` (`modelGuidance=0`, `fullRackSuggested=5`, Stand Map 42 items). I did not assert minimap-versus-Stand Map parity in planning mode.

### 3.3 Classification
* **Historic `StandMapParity`: FAIL ×2 retained** (#357 sessions 1–2). Its obsolete assumptions: pre-created authored racks, minimap text only "N" (the production teaching note now exists), and many-to-one counting. It was not edited, rerun or weakened.
* **New V2 witness: PASS at its actual scope** — default V2 new game outside planning mode, two viewports, item-level identities. It does **not** cover confirmed-rack or planning-mode parity, the forest picture, or cell buttons.
* **Gate D:** no genuine unexplained discrepancy remains; I do not mark D as PASS on the historic token (see Section 5).

## 4. Execution record — every Editor session (one lease, strict-clean lane before each, protected hashes before and after each: all unchanged)

Driver: the #357 `run_gate.py` with only its lock task id changed (stages the disposable harness from the lane's own bytes or an external file, private Xephyr display, isolated `XDG_CONFIG_HOME`, licence copied then deleted, post-verdict reaper). Wrapper `gate.sh`: protected-hash check → one Editor → protected-hash check → record dirt → guarded restore. All logs: **0 `error CS`** (17 logs). The only exception text in the logs is Unity's own `SearchDatabase` `ArgumentOutOfRangeException` (engine noise, also in #315/#356/#357). Times UTC.

| # | Label | Gate / harness | Result | Seconds | Notes |
|---|---|---|---|---|---|
| 1 | `v2witness` | `StandMapV2Witness.Begin` (new) | **PASS** `checks=59 shots=6` | 98 | Section 3 |
| 2 | `matpair` | `SectionFiveMaterialPair` (new) | FAIL (harness defect) | 88 | preserved, not counted |
| 3 | `matpair2` | same, fixed once | FAIL (harness defect) | 102 | preserved, not counted |
| 4 | `matpair3` | same, fixed twice | **PASS** `checks=40 shots=24` | 93 | Section 2.4; 0 differing pixels |
| 5 | `save03b` | `RackV2SaveVerification.Begin` (canonical, unchanged) | **FAIL** — `capture writes save v23` | 78 | stale pin: `ForestSaveData.CurrentVersion` is 24; the harness asserts 23 at lines 124 and 237 |
| 6 | `headland-qual` | `RackV2HeadlandQualification.Begin` | **PASS** `checks=124` | 152 | |
| 7 | `headland-access` | `RackV2AccessVerification.Begin` | **PASS** `checks=8` | 104 | |
| 8 | `combined-write` | `RackV2CombinedVerification` mode `write` | **PASS** `checks=35` | 104 | isolated save, state file on Files |
| 9 | `combined-continue` | same, mode `continue`, real startup screen, seeded with #8's save | **PASS** `checks=46` | 128 | cold Editor Continue, approve, execute, second round trip |
| 10 | `model0` | `RackV2LegacyProgressionRegression` mode `model0` | **PASS** `checks=21` | 104 | |
| 11 | `legacy40prog` | same, mode `legacy40` (`CCF_STAND_GEOMETRY=0`) | **PASS** `checks=15` | 92 | |
| 12 | `uxpedagogy` | `UxPedagogyShots.Begin`, 1280×720 + 1920×1080 | **FAIL (strict)** `pass=778 fail=38 shots=62` | 253 | Section 5 gate I |
| 13 | `mfm07` | `MfmApprovalBoundaryUnityGate.Begin` | **PASS** `checks=106` | 112 | proposal → prescription approval → pending ordinary work |
| 14 | `save03b-diag` | **labelled diagnostic variant** of #5 (only the two pins 23 → 24; `probe/save03b-v24-pin-diagnostic/variant-vs-canonical.diff`) | FAIL — `an empty-plan world hashes exactly as its pre-v21 layout` | 86 | **not the canonical gate and not a pass substitute**; see Section 5 gate N |
| 15 | `reference` | `ScenarioReferenceVerification.Begin`, default (V2) environment | FAIL — **invocation error**: `archive.Matches` requires Legacy40 | 84 | archive integrity + negative control passed first; canonical runner convention (`LEGACY_GATES`) sets `CCF_STAND_GEOMETRY=0`, which I had omitted |
| 16 | `reference2` | same, canonical env (`CCF_STAND_GEOMETRY=0 CCF_T03_LEGACY=1`, `TeachingCapGateSetup` companion) | **PASS** | 120 | archive integrity (milestones 0/20/50/100, Year-100 `7AD177B3CC2F73C7`), tamper negative control, four previews, deterministic continuation (`year50=v12 year100=v24`), `REFERENCE_FUTURE_V1_CONTRACT_PASS` |
| 17 | `b3-plain` | canonical `StandGeometry/run_enlarged80.py --b3-mode plain --candidate-sha 045966d2…` | **PASS** `ENLARGED80_VERIFY_PASS checks=12`, `EVIDENCE_COMPLETE` | 139 | see below |

The #17 runner uses a git-ignored scratch licence copy (`Build/Enlarged80/config`, sha16 `9f359ae8a5c3a70f`) that is stale; as in #353 it was **temporarily replaced by the Hub licence for the run and the original bytes were restored afterwards** (sha16 verified equal; only the hashes are in evidence, no licence bytes). B3 witness: 14 resets, 35 pose records, archive unchanged; after `EndReferencePreview` and at `reset13-entry`/`reset14-entry` the player is at `(-0.43, 0.03, 5.25)` (the authored start; #351 saw `(-13, 0.08, -9)` there) with the character controller enabled. It is a batch, headless, in-process witness — **not** a native interactive return — and the harness itself declares `b3Verdict=NOT_DECIDED_BY_HARNESS`.

## 5. A–O on exact `045966d2` (PASS / FAIL / NOT RUN / NOT VERIFIED)

| Gate | Result | Evidence and honest scope |
|---|---|---|
| A Import/compile | **PASS** | 17 Editor logs this task, 0 `error CS`; exact HEAD imported with a warm Library |
| B V2 surface lifecycle | **NOT RUN** (partial support) | no V2↔non-V2 surface create/clear/recreate transition test. Headland qualification 124/124 shows the V2 world's 3D headland/feeder/pad state; Legacy40 progression 15 and model-0 21 pass; neither is the transition test |
| C Map geometry vs 3D | **NOT VERIFIED** | Stand Map shows the existing headland + single feeder with no northern connector (screenshots) and headland qualification 124 + access 8 pass in 3D, but **no paired Stand Map + 3D capture** was made; no dark-edge pixel check |
| D Full-vs-Mini invariance | **Historic FAIL retained; V2 witness PASS (scope in 3.3)** | difference fully attributed to the five intentional guidance band/line pairs. Rendered Full vs Mini screenshots at both sizes exist. Whether this discharges D is the Manager's call |
| E Legend and paths | **NOT VERIFIED** | colour and grayscale Stand Map screenshots at both sizes captured; the legend contains the "Existing V2 headland / feeder — not a planned rack" entry; no independent observer comprehension check |
| F Performance | **NOT RUN** | no existing harness; none invented |
| G Five-rack diagram | **NOT RUN** | no live-render harness; no capture |
| H Old-world diagram guard | **NOT RUN** | Legacy40 (15) and model-0 (21) *progression* PASS are not the diagram guard |
| I Card fit / guidance | **FAIL (strict) — card-fit checks PASS** | `UxPedagogyShots`: 778 PASS / **38 FAIL = 19 distinct × 2 sizes**. Objective card: `OBJECTIVE_CARD_VISIBLE`, `_FITS_THE_SCREEN`, `_NEEDS_NO_SCROLLING` **16/16 each** (8 objectives × 2 sizes), `OBJECTIVE_PART_INSIDE_CARD` 82, `OBJECTIVE_NOWRAP_TEXT_FITS` 70, `OBJECTIVE_PARTS_DO_NOT_OVERLAP` 48, every objective `DIAGRAM_*` check PASS (`DIAGRAM_PRESENT_intro` is among the 19 failures), HUD objective text rows PASS. **The 19 failures:** the intro/help-window checks (`INTRO_OPENS_WITH_ITS_TITLE`, four `INTRO_EXPLAINS_*`, `DIAGRAM_PRESENT_intro`, `HELP_WINDOW_VISIBLE/ON_SCREEN/CLOSE_BUTTON_REACHABLE` ×2, `MORE_DETAIL_OPENS_THE_FULL_TEXT`, `TEXT_help-intro*` ×2) — the screenshot shows the intro now opens as "What is Continuous-Cover Forestry?", so the harness's older intro/help assertions are stale; and `STAGE_FIXTURE_PRESENT_5/7` + `WORKPLAN_LOCKED_SECTIONS_AT_STAGE_5/7` — `CCF_UXSHOTS_STAGE5/7_SAVE` fixtures were not supplied (older Oct-2026 fixtures exist on Files; not used, provenance/schema unverified). Not a verdict that the intro or Work Plan lock is correct |
| J Native controls / safe focus | **NOT RUN as a gate** | no existing harness for Help-close/default focus on this candidate; Objective 1 native route PASS is inherited (#356) |
| K Work Plan / MFM boundary | **PASS (MFM07 106 checks); Objective 1 route inherited** | `MFM07_UNITY_PASS checks=106`; combined `continue` 46 also exercises approval, warn-on-edit, execution and locks on a V2 world |
| L Workbench retirement | **107/107 in-Editor PASS (inherited); strict runner exit 1 = FAIL (unchanged)** | not rerun; not qualified |
| M CTA / feedback checker | **PASS source-static (inherited #356 10/0/2)** | #309 independent review status not assessed |
| N Save/load compatibility | **PARTIAL.** FAIL: `save03b` (canonical, stale pin) and its diagnostic variant. PASS: combined write 35 / continue 46, model-0 21, Legacy40 15, frozen Reference Future v1 (full contract), V2 persistence 41 (inherited) | The diagnostic variant passed the first two pins and then failed `an empty-plan world hashes exactly as its pre-v21 layout`: the 03B gate's own pre-v21 layout comparator is older than save v24 (`129d0ace` added a v24 field after the v23 hash-strip it knows; `RackV2Persistence` replaced these comparisons with v24 `HistoricalLayout` projections). **That is a probable cause, not isolated**, and I did not keep patching a historical gate. No player save was read or written (isolated config; before/after hashes equal) |
| O Final integrated regression | **NOT RUN** | integrated candidate not qualified; no claim |
| Reference Preview return pose | **PASS (batch witness, 12 checks)** | Section 4 #17 |
| Material-dependent visual items | **0-pixel difference measured** | Section 2.4; acceptance is the Manager's |

## 6. Antecedents (same exact source, **not rerun**, scoped only)
Workbench 107/107 in-Editor PASS with **strict runner `exit1` FAIL kept**; Objective 1 native `RACKV2_O1E2E_PASS checks=173 shots=24`; Rack V2 persistence v2 31 + coldstart 3 + Legacy40 7 = **41 real checks PASS** (#357). CTA 884-negative/112-benign, #355/#356 mutation suites and the old five-rack checker were also not rerun.

## 7. Protection (verified before and after; unchanged)
Frozen Reference Future v1 archive and `.meta` (hash = HEAD; the reference gate re-verified the Year-100 hash), scene, `Packages`, Legacy40 behaviour, the real player saves (`forest-save.json(.bak)` SHA-256 equal at every session boundary), the **27 ignored licensed-pack files** (27/27 hashes OK), the original **eight-dirty Workbench** worktree (status + 8 hashes identical), `.meta` files, `Library` (not copied or rebuilt). No harness source or generated `.meta` remained under `Assets/ForestPrototype/` (lane tracked-clean after every restore). Licence copies were deleted by the driver; the scratch licence copy changed for #17 was restored byte-for-byte.

## 8. Findings, blockers and what the Manager must decide
1. **No new blocker in the product.** Nothing in this task points to a game defect: no compile error, no map discrepancy, no visual difference, no unknown dirty path, no protected-state change, no unsafe disk or lock state.
2. **Two strict gate failures are harness-staleness, not verdicts on the game, and need an owner:** `save03b` (version pin 23 vs 24, then an older pre-v21 hash comparator) and `UxPedagogyShots` (intro redesign; Work Plan fixtures). Options: a verification-only update task, or retire them in favour of the v24-aware persistence/combined gates. I did not edit either.
3. **Adopt or retire the V2 witness.** `StandMapV2WitnessLogic` (+ offline proof) could replace `StandMapParity` as gate D for the V2 world after an independent review; confirmed-rack and planning-mode parity would still need their own checks.
4. **Material policy.** The measured 0-pixel difference and the new findings (leaf rewrite already live in memory; `_Color` change is Unity's own read-back) support classifying the twelve-path rewrite as a known, deterministic, visually null Editor side effect, but the emulation caveats in 2.4 stand; the decision to accept it, and to stop the strict runners failing on it, is the Manager's. Correct the swapped berry rows in #357's report.
5. **Still without a harness (NOT RUN):** B surface transitions, C paired Stand Map/3D capture, F performance/GC, G five-rack diagram live render, H old-world diagram guard, J native Help-close/default focus. These need either new bounded harnesses (Manager-authorised) or explicit descoping.
6. **Independent review:** an independent review is warranted for the new witness and material-pair probes (verification code written by the same worker that ran them) before their results are cited as accepted evidence. The integrated candidate remains **NOT QUALIFIED for #316**, which was **not** started; nothing was merged to `main`; no Player/Windows build.

## 9. Reproduction and evidence
* Evidence (333 MiB, 1027 files, manifest `evidence-manifest.sha256`, itself SHA-256 `02b41dc8d1f60d329be6a0c5a2b1997355f097372bc734625c3b4631cd931351`): `/media/jer/Files/CCF/Verification/CCF-358-V2MapParityFinalUnity-20261109/` — `triage/`, `before-image/` (byte copies of the eight entry-dirty files), `probe/`, `offline/`, `matpair/`, `run/<gate>/` (logs, screenshots, `result.json`), `run/gates/<gate>/`, `run/restores/<label>/`, `run/holder.log`, `run/disk-monitor.log`, `README.txt`.
* Staged sources (SHA-256, equal to `staged-sources.json` of the runs): `StandMapV2Witness.cs` `b6a95750a28ae870aca59da161f2db7ef039c672d7d418308e675da3db11b630`; `StandMapV2WitnessOfflineCheck.cs` `b864a01e126da999f2505fc024024b1ddcb21bcfb2273826b914102e4126d8c3`; `run_v2witness_offline.py` `484844ad68464bc468c94919ec69ae42749c281a8361fb17ee1cf0a9097dfc0a`; `SectionFiveMaterialPair.cs` (run 3) `b8d6d8e5ab37d7119aa9bd6f3ae7b2ccedc4db6ce318fac03766205e8ce9a271`; save03b variant `17189d0b770a14255fbcb6145bd0b72909ce3071bbbcb358bf68ac174160f2d3`. Committed in `f2a69e3e…` under `Tools/Verification/StandMap/V2Witness/` with a README (copy → run → remove workflow, scope, what it does not prove). The save03b variant lives only in evidence.
* Reproduce the offline proof: `python3 -I Tools/Verification/StandMap/V2Witness/run_v2witness_offline.py --lane <repo root> --probe Tools/Verification/StandMap/V2Witness --out <scratch>` (expects `STANDMAP_V2_WITNESS_OFFLINE PASS checks=25 failed=0`).
