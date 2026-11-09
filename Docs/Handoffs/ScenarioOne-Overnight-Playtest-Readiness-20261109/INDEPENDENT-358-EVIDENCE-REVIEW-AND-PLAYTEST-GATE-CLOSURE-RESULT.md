# #359: Independent #358 evidence review and bounded personal-playtest gate closure (RESULT)

**Date:** 9 November 2026. **Claim:** [#359](https://github.com/jermur788/CCF/issues/359) ([RUNNING comment](https://github.com/jermur788/CCF/issues/359#issuecomment-6748429189)). **Binding packet:** [`MANAGER-358-TO-359-…-CLOSURE.md`](https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Overnight-Playtest-Readiness-20261109/MANAGER-358-TO-359-INDEPENDENT-VERIFIER-AUDIT-AND-PERSONAL-PLAYTEST-GATE-CLOSURE.md).
**Executor:** Anthropic **Claude Opus 5.5** (`claude-opus-5-5`) in Claude Code inside T3 Code, local. Effort level is not exposed to the session. One writer, no fan-out.
**Independence caveat:** the packet asked for a different *provider* if one was available. This worker is the **same provider (Anthropic)** as the #358 author (Claude Sonnet 5.5), but a different model in a separate session. Treat every "independent" opinion below as same-provider, different-model independence only. The #359 probes were written and run by this worker, so they are **author evidence** for the #316 reviewer, not independently reviewed.

## Verdict: **NEEDS CHANGES.** One probable product defect (minor, keyboard focus in the onboarding). Unity was stopped after session 1 of a planned 3. The integrated candidate is **NOT QUALIFIED**; personal playtest is **not** released; #316 was **not** started.

1. **#358 evidence review: both probes are APPROVED at their stated limited scope** (Sections 2–3). No false green or provenance break was found. The weaknesses found are documented and do not change their results.
2. **save03b and all 19 distinct UxPedagogyShots failures are stale harness assertions, not product defects** (Section 4). Their canonical FAILs are preserved unchanged. The exact cause of the save03b diagnostic-variant failure is now isolated from source: the v24 field `firstGuidedRackReviewComplete` is stripped by production (`129d0ace`) but not by save03b's independent strip. The intro failures follow from the approved 3-page onboarding (`6bff87dd`). The Stage 5/7 fixtures were never supplied. Uncovered residue was identified and replacement probes were authored, but only one ran.
3. **New finding (gate J, probable minor product defect).** In the 3-page onboarding, after the player advances a page with the **Right arrow**, keyboard focus lands on **"← Back"**, not on Next/Begin. A following **Enter therefore goes back a page.** Observed at 1920×1080 with queued New Input System events in the Editor; **not** verified with native OS/Windows input. Mouse use, initial page-1 focus, Esc/Left (back), Right (forward), F1 lockout and completion all behave correctly (Section 5.1). The packet says to stop on a true UI fault or unknown residual, so **no further Unity session ran**: the old-world (Legacy40/Reference) probe and the v24 save-contract probe are **authored and typechecked but NOT RUN**.
4. **What session 1 did verify (PASS, on exact `045966d2`):** the V2 surface lifecycle across a model-0 load and a Reference Preview (B); the 3D mesh, Stand Map model and Stand Map strokes describe one western headland and one southern feeder that meets the forest path, with no northern spur (C); the live five-rack figure (G); planning-mode and confirmed-rack Mini/Full parity (D+); F1 Help close and focus at both sizes (J-help); and old-world fail-close after a pre-V2 load (H, partial). **Three defects in my own probe** make three sub-checks NOT VERIFIED (Section 5.2). They are not product results.

## 1. Identities, lanes, lock and workload

| Item | Value |
|---|---|
| Game source tested (unchanged) | `045966d2f8e26368e30a79f14a1e34167952b95e`; 0 files differ under `Assets`, `Packages`, `ProjectSettings` between it and this branch |
| Unity lane | `/media/jer/ZX20/CCF-U3`, branch `qualify/scenario-one-ui-lifecycle-20261109-v2`, warm 6.6 GiB `Library` reused (no cold import) |
| Writer lane | `/media/jer/Files/CCF/Operations/CCF-FocusedSourceStage-20261109`, branch `integration/scenario-one-focused-playtest-20261109-v1`, entry tip = origin = `63ae9af6f9a97874e71d6fa6d5c10c44325283e1` |
| Verifier source commit (separate) | `81818e7f` adds `Tools/Verification/ScenarioOneFocusedClosure/` (3 probes + README). `Ccf359V2Closure.cs` is byte-identical to the staged copy that ran (SHA-256 `ac6b19fbdbd17586a41a3fe972c9316012232c1825be4131d9449373a5975655`) and is committed **with its three known defects unfixed**. `Ccf359OldWorld.cs` (`e4fb1032…`) and `Ccf359V24SaveContract.cs` (`b8e8ab6b…`) are authored and typechecked only |
| Report tip | this report-only successor commit |
| Byte identity kept | save03b `RackV2SaveVerification.cs`, `UxPedagogyShots.cs`, `StandMapParity.cs`, `run_workbench_retirement.py` unchanged since `045966d2`; the five #358 `StandMap/V2Witness/` files unchanged since `63ae9af6` |
| Lock (actual ID) | **`c7663db5d5ab4235a3023d8f2f89b922`**, owner `Claude Code (Opus 5.5) via T3 Code #359`, task `CCF-359-INDEPENDENT-AUDIT-PLAYTEST-GATE-CLOSURE`; acquired 19:08:56Z, `status --json` owner/task/lane/branch/head all `True`; released 19:16:07Z **by that ID**, `forced: False`; afterwards `free`, no Editor |
| Admission | ZX20 36.14 GiB (≥35), write I/O 0.0 MB/s, lock free, no Editor, licensed pack 27/27, user saves/prefs SHA-256 OK, scene/Packages/ProjectVersion/Reference Future v1 `.bytes`+`.meta` = HEAD blobs (manifests rebuilt independently from git, equal to #358's), original eight-dirty Workbench status + 8 hashes identical, U3 tracked-clean (guard: nothing to restore) |
| Disk monitor | 20 s (28/25/20 GiB thresholds): 23 samples, **min 36.13 GiB**, 0 ALERT/STOP/KILL |
| **Workload** | **1 lease, 1 Editor session** (294 s), 1 guarded restore. #358 used 17 sessions; the packet asked for 2–4 |

## 2. Independent opinion: `StandMapV2Witness` (#358): **APPROVED at its limited scope**

*Scope:* the default V2 new game, outside planning mode, at two viewports, comparing the minimap and Stand Map plans item by item.

* **Provenance:** the committed `f2a69e3e` files hash exactly to the report's values and to `staged-sources.json` of the run. The #358 evidence manifest re-verified **1028/1028** (manifest SHA `02b41dc8…`).
* **Offline proof reproduced independently:** run against U3's production map sources, it gives `STANDMAP_V2_WITNESS_OFFLINE PASS checks=25 failed=0`: honest plan 10 Mini-only items / 30 points; historic counting `matched=9 missing=10 of 19`, `points=212 of 242`; all 17 mutants plus the duplicate case FAIL the witness. *Reproducibility defect:* the README's documented command fails with a relative `--probe` path (the runner never makes it absolute); an absolute path works.
* **Real-canvas receipts:** 59 = 52 logic checks (26 × 2 sizes) + 7 runner hard checks; 0 FAIL lines; 0 `error CS`. The screenshots match the claims: five dashed routes and no headland on the minimap; L-shaped headland and feeder with no rack lines on the Stand Map.
* **False-green review (none found that changes the result):** matching is genuinely one-to-one, and shared keys are unique per index. Guidance positions come from model bounds, not `StandMapTransform`. Weaknesses: (a) **Full-only items, including the headland strokes, are checked by count only, not position.** #359 session 1 closed this: the strokes and pads sit at the live 3D mesh place, with negative controls. (b) "Independent" positions still rest on the production model's bounds and paths. (c) `pairedPoints == expectedPoints` is tautological (harmless; the worst-distance test still covers every point). (d) Planning mode is logged, not asserted. #359 asserted planning and confirmed parity (Section 5.1).

## 3. Independent opinion: `SectionFiveMaterialPair` (#358): **APPROVED at its limited scope**

*Scope:* the current URP configuration (MSAA off), these flora views and seven materials, as an in-memory emulation.

* **Hash binding:** the seven parsed committed/generated SHA-256 values in the run log equal the report table. All **11** envelope materials' canonical hashes equal their `045966d2` blobs, and their generated hashes equal the committed `serialized-restoration.json` ledger. The probe's staged source equals its committed bytes.
* **Captures independently re-compared:** all **8** committed / generated / live-editor PNG triples are pixel-identical (decoded RGB). None of the frames is blank. The Forb rosette close-up (1280×720) and the Bilberry cover (1920×1080) clearly show the flora (Bilberry partly crossed by a branch, as disclosed).
* **Static corroboration (new):** all of these materials use URP `Lit.shader` (GUID `933532a4…`). In it, `_Color` and `_MainTex` are `[HideInInspector]` legacy-compatibility properties that the Lit HLSL never samples (it reads `_BaseColor`/`_BaseMap`), and `AlphaToMask[_AlphaToMask]` affects only MSAA targets. No game script reads `_Color`/`_MainTex` on these materials (the hits are Editor setup scripts for other assets, plus `SitkaFoliage`). This explains the 0-pixel result rather than relying on it alone.
* **Weaknesses:** (a) **no positive control** proves that a swap visibly changes pixels; (b) the comparison render texture is created **without MSAA**, so the probe can never exercise `_AlphaToMask`; (c) the four **Deadwood_Fungi** materials were never rendered. Their rewrites are the same `_MainTex` ← `_BaseMap` alias on URP Lit (#358 restore receipt), so they are statically render-neutral but not measured. (d) Comparison is per pixel, maximum over R/G/B (alpha ignored, as on screen).
* **Recommended classification:** reproducible Unity Editor re-serialisation of the finite 11-material envelope, with **no render effect in the current URP Lit / MSAA-off configuration**, by static shader semantics and the 0-pixel measurement. This is **not** a global equivalence claim across MSAA-on, other pipelines or platforms.
* **Erratum (dated 9 Nov 2026, #359) for the #357 report, which is not edited:** its property table swapped the `Berry_Calyx` and `Bilberry_Berries` `_Color` rows. Committed `Berry_Calyx` is `{0.045, 0.07, 0.035}` (confirmed here from the `045966d2` blob) and `Bilberry_Berries` is `{0.035, 0.055, 0.095}`. Its hashes and conclusions are unaffected.

## 4. Historic failing gates: per-assertion mapping (canonical FAILs preserved, no re-pin)

### 4.1 `save03b` (`RackV2SaveVerification`): canonical **FAIL** (line 124, pin `== 23`); labelled diagnostic variant **FAIL** (line 268)

The gate throws at its first failure, so the canonical run never executed sections B–J. The diagnostic variant reached F.
**Cause of the F failure (isolated from source):** `129d0ace` added `"firstGuidedRackReviewComplete":false,` to v24, and `ScenarioReferenceArchive.HashLayout` strips it with `WithoutTeachingCompletion`. save03b's `StripV21` predates v24 and leaves the field in, so its hash cannot equal production's. This is stale harness logic; no defect is implied.

| Section | Original assertion | v24 coverage today | Class |
|---|---|---|---|
| A | fresh V2 identity, capture version, v21 round trip, hash-exact | RackV2Persistence `v2` "new game: v24 disk save, Continue, reload exact" + coldstart; combined write 35 / continue 46 | **Adequately tested** (stronger: real disk path). Version pin stale |
| B | model-1 draft round trip | Persistence "draft … non-contiguous ids and nextRackId survive disk reload" | **Adequate** |
| C | positive rackId order round trip | Persistence partial/all marks with rack-owned orders, coldstart exact signature | **Adequate** |
| D | confirmed five-rack invariant | Persistence "save/reload after confirmation exact on disk" | **Adequate** |
| E1/E2 | v20 loads model 0 and re-saves hash-stable; legacy orders rackId 0 | Persistence §7/§8 (audited v24→v20/v21 projections) | **Adequate** |
| E3 | `RestoreSaveData(null)` stays model 0 | none found | **Stale / uncovered:** replacement authored (`Ccf359V24SaveContract`), **NOT RUN** |
| F1 | empty-plan hash == independent pre-v21 layout | `TeachingPersistence/run_offline.py`, **rerun by #359 at `045966d2`**: v23 and v24 all-hash dumps **byte-identical** (80 rows, SHA `5d69d87e…`; latch true/false equal). Offline stand-in serializer; its final protected-source assert fails by design (pinned to old base `c8cfae8c`) | **Transitively covered offline.** Unity JsonUtility path stale: replacement with provenance witness authored, **NOT RUN** |
| F2/F3 | rack content → new identity; flag alone does not move hash | offline fixtures include rack rows; flag not varied | **Partly uncovered:** authored, **NOT RUN** |
| G | 19 semantic + 3 missing-field rejections | Persistence 21 disk cases cover 10 semantic + all 3 missing-field. **9 uncovered:** nextRackId 0, degenerate geometry, empty tree id, a tree owned by two racks, Crop Tree in a removal set, Fell on a marked entrance, an independent order on a rack tree, model-0 with rack-owned orders, model-0 with V2 id state. The validator changed after save03b (`3eb9c954`) | **Stale / uncovered:** authored, **NOT RUN** |
| H | refused save leaves world untouched | Persistence malformed cases "without … any world change" (hash + signature) | **Adequate** |
| I | Reference Future v1 archive untouched | Persistence + #358 reference2 PASS | **Adequate** |
| J | V2 never active on Legacy40 with model 1 | Persistence legacy40 "files claiming V2 are refused" (disk refusal) | **Partly covered** (in-memory predicate): authored, **NOT RUN** |

### 4.2 `UxPedagogyShots`: canonical **FAIL** 778 PASS / 38 FAIL = **19 distinct × 2 sizes**

| # | Distinct failure(s) | Original requirement → current production | Coverage | Class |
|---|---|---|---|---|
| 1 | `INTRO_OPENS_WITH_ITS_TITLE` | "Transforming a Sitka Plantation" in the help-window intro → now the title of **page 3** of the approved 3-page onboarding (`ScenarioOneOnboardingCopy`, `6bff87dd`) | #359 J: page-1 approved title PASS; page-3 title check not reached (knock-on of the focus defect) | **Stale**, replacement partly run |
| 2–5 | `INTRO_EXPLAINS_{What_you_have_inherited, What_you_are_trying_to_create, How_transformation_works, access_racks}` | old headings → `InitialState`/`TargetState` (page 3), process + `KeyIdea` (page 2). **"access racks" no longer appears in the intro**; it is taught on the Objective 1 card | #359 G `CARD_USES_THE_V2_WORDING` PASS ×2 | **Stale.** Design note for the Manager: rack access left the intro by approved design |
| 6 | `DIAGRAM_PRESENT_intro` | intro now uses approved artwork, not `TeachingDiagram` | no `CCF_ONBOARDING_ART_MISSING` in either log | **Stale / superseded** |
| 7–12 | `HELP_WINDOW_VISIBLE`, `HELP_WINDOW_ON_SCREEN`, `HELP_CLOSE_BUTTON_REACHABLE` for `intro` and `intro-more-detail` | `ShowScenarioIntroduction()` hides `menu-help-window` and shows the onboarding | UxPedagogy's own map/workplan Help states PASS; **#359 J-help 23/23 PASS at both sizes** (window on screen, close visible with `[Esc]`, Esc closes only Help, no focus trap) | **Covered** for Help. Onboarding fit not measured |
| 13 | `MORE_DETAIL_OPENS_THE_FULL_TEXT` | onboarding has no More-detail toggle (3 pages instead) | menu-help More detail PASS in UxPedagogy map/workplan states | **Superseded** |
| 14–15 | `TEXT_help-intro`, `TEXT_help-intro-more-detail` ("no visible text") | measured the hidden window | onboarding text size not measured | **Stale.** Onboarding text size **NOT VERIFIED** (minor) |
| 16–17 | `STAGE_FIXTURE_PRESENT_5/7` | env fixtures not supplied. The Oct-23 Files fixtures are **save v20, model-0 (pre-V2)** worlds, not representative of the V2 playtest world, so they were not used | none | **NOT RUN.** An honest V2 Stage 5/7 save needs real multi-year play |
| 18–19 | `WORKPLAN_LOCKED_SECTIONS_AT_STAGE_5/7` | direct consequence of 16–17: the world stayed at stage 0, where `locked=3` is correct. *Note:* UxPedagogy's `…_DISABLED_5/7` and `…_EXPLAIN_5/7` "PASS" lines are **vacuous** (also stage 0) | #359 forced-lesson replacement hit a probe defect | **NOT VERIFIED** |

The objective-card fit checks (16/16) and the other content/overlap checks remain credited only at their exact passed scope.

## 5. Session 1: `Ccf359V2Closure` on exact `045966d2` (one Editor, 294 s, 0 `error CS`)

`CCF359_V2_CLOSURE_FAIL checks=135 shots=28`; 11 FAIL lines; driver exit 1. The only exception in the log is the known QuickSearch `SearchDatabase` noise. The Editor rewrote 8 envelope paths (`.vscode/settings.json` + 4 Deadwood_Fungi + 3 Forb), all matching known generated hashes. They were restored by the guard with the Editor stopped; U3 is tracked-clean.

### 5.1 Results by gate (PASS counts are this run's)

| Gate | Result | What exactly passed or failed |
|---|---|---|
| **J onboarding** | **FAIL: probable minor product defect** | PASS (12): fresh game opens page 1 with modal held; default focus is page-1 **Next**; M cannot open the map behind it; F1 cannot skip; Esc on page 1 does not skip; Right advances; Esc goes back; completion releases the modal; no focus left on a hidden element; W then moves the player 3.59 m. **FAIL:** after Right, focus is `Button#onboarding-back '← Back'`; Enter then went **back** to page 1. Mechanism (inferred from source, not instrumented): the page re-renders in the key handler and destroys the focused Next; UI Toolkit then dispatches the arrow's navigation and focus falls to the first focusable element (Back). Only Next schedules focus. Knock-on fails: page-3 focus and title checks ran on page 2 |
| **J Help (F1)** | **PASS 23/23** (both sizes) | F1 opens an on-screen window; Continue `[Esc]` visible inside it; Help takes no focus; Esc closes Help and nothing else; over the Stand Map one Esc closes only Help and a second closes the map; F1 toggles; no focus left in hidden Help |
| **B surface lifecycle** | **PASS** | exactly one `Scenario One V2 Headland`, active only in V2: active in fresh V2 → **inactive** after a model-0 Enlarged80 load → the **same object** reactivated after V2 reload → **inactive** during the Reference Future v1 Year-50 preview (opened from V2: `began=True`) → the same object active after return. Never a duplicate. *Honest wording:* the object is created once and then deactivated/reactivated, not destroyed and recreated |
| **C map ↔ 3D** | **PASS** (14) | live mesh: spine x −35.4, r 1.3, z −37.7…37.7; one eastward arm only, at z −27.4 (south), **no northern spur**; the feeder end meets the forest path (0 m); the map model equals the mesh place; **Stand Map strokes and pads at the mesh place at both sizes**; negative controls (3 m north shift, mirrored northern feeder) detected; every headland casing drawn before any fill. Paired captures: `c-stand-map-*` and an orthographic `c-3d-top-down-*` (trees hidden for that capture only) |
| **G five-rack figure** | **PASS except one over-scoped check** | ×2 sizes: Objective 1 shows `access-five-rack`; five planned corridors; **six bands**; spacing not uniform; corridors at the **live** suggested routes (z −35.27 … 35.23); frame visible and ≥560 wide; illustrative title present; words inside the frame without overlap; V2 card wording. The `…FIGURE_FLOOR` FAIL (smallest 18) is the **shared lesson caption** (`--fs-body` 18), outside the figure's 20-unit floor: a **probe defect** |
| **D+ planning / confirmed parity** | **PASS** (15) | minimap shows five dashed routes before planning; in planning mode the routes move to the Stand Map **at the same fractions** as the minimap guidance and at the manager's endpoints; five drafts appear only on the Stand Map while the minimap still shows guidance; the plan confirms; after confirmation, at both sizes, each confirmed rack has exactly one band and one line on **each** canvas at the same fractions and at the manager's own endpoints, with no leftover guidance; a 1% moved rack is detected |
| **H old-world guard** | **PARTIAL PASS** | after a pre-V2 (model-0) load: V2 off, surface inactive, no map headland or strokes, Objective 1 shows the original `access` picture, no figure title or V2 wording. During the Reference preview: V2 off, surface inactive, model headland 0, no figure. The preview-stroke FAIL reads a stale canvas (the Stand Map cannot open in preview): **probe defect**. **Legacy40 new game and Legacy40-native preview: NOT RUN** (session 2) |
| **I Work Plan locks** | **NOT VERIFIED** | lesson 0: 3 locked (Planting, Pruning, Nursery), disabled, explained: PASS. Forced 5/7: the injection was discarded (topic stayed 0), a **probe defect**, not a product result |
| **F performance** | **MEASURED, NOT VERIFIED** (no baseline) | Editor play mode, GTX 980M Vulkan, private Xephyr, 300 frames each. HUD still: mean 97.6 ms/frame, p95 104, GC allocated 204 KB/frame, 5 gen-0 collections. Stand Map open: 95.1 ms, 88 KB/frame, 2 collections. Objectives open: 96.8 ms, 187 KB/frame, 5 collections. First-half vs second-half allocation is equal within 0.5%, so allocation is **steady, not growing**. Absolute times include Editor overhead and are **not** player performance. ~200 KB/frame in a still HUD is flagged for #316 review |

### 5.2 Defects in the #359 probe (author's own, disclosed, kept unfixed in `81818e7f`)
1. `ForceStage` injected facts without first syncing `TeachingWorldVersion`, so `TeachingFacts()` discarded them. Fix: read `ui.Learning.CurrentTopicIndex` before injecting.
2. The figure font floor was applied to every label, including the shared caption.
3. The preview-stroke check did not require the Stand Map to be shown and repainted.

None of these hides a product failure. Each makes its sub-check NOT VERIFIED.

## 6. A–O on exact `045966d2` (PASS / FAIL / NOT RUN / NOT VERIFIED)

| Gate | Result | Scope |
|---|---|---|
| A Import/compile | **PASS** | this session 0 `error CS`; #358's 17 inherited |
| B V2 surface lifecycle | **PASS** (#359) | model-0 load and V2 Reference preview transitions; Legacy40 new game NOT RUN |
| C Map vs 3D | **PASS** (#359) | numeric mesh ↔ model ↔ strokes + paired captures; a human visual review of the pair is still the reviewer's |
| D Full vs Mini | **Historic StandMapParity FAIL ×2 retained.** V2 witness (#358) APPROVED at default non-planning scope; **planning + confirmed parity PASS** (#359) | the old gate is not retroactively green |
| E Legend / paths | **NOT VERIFIED** | captures exist (colour + grayscale); no observer comprehension check |
| F Performance | **MEASURED, NOT VERIFIED** | no baseline; Editor numbers only |
| G Five-rack diagram | **PASS** except one over-scoped probe check | live render at both sizes |
| H Old-world guard | **PARTIAL** | pre-V2 load and V2-origin preview PASS; Legacy40 and native preview **NOT RUN** |
| I Card fit / guidance | **UxPedagogy canonical FAIL retained**; card fit 16/16 inherited; Work Plan forced-lesson locks **NOT VERIFIED**; real stage 5/7 **NOT RUN** | |
| J Native controls / focus | **FAIL (probable minor product defect)** in the onboarding; Help close/focus **PASS** | synthetic Input System events, not native OS |
| K Work Plan / MFM boundary | **PASS inherited** (#358 MFM07 106) | not rerun |
| L Workbench retirement | **107/107 in-Editor inherited; strict runner exit 1 = FAIL (unchanged)**; #287 NOT fully qualified | not rerun, not relabelled |
| M CTA / feedback checker | **PASS source-static inherited** | not rerun |
| N Save/load compatibility | **save03b canonical FAIL + diagnostic FAIL retained.** v24 persistence 41, combined 35/46, model-0 21, Legacy40 15, Reference v1 inherited PASS; v23≡v24 hash identity offline PASS (#359 rerun); v24 save-contract replacement **NOT RUN** | |
| O Final integrated regression | **NOT RUN** | not qualified |

## 7. Protection (verified before and after; unchanged)
U3 `045966d2` on its branch, tracked-clean, index unchanged, no `Ccf359*` file or `.meta` left in `Assets/ForestPrototype`; frozen Reference Future v1 `.bytes`+`.meta`, scene, `Packages`, `ProjectVersion` = HEAD blobs; player saves `forest-save.json.bak`, `save-backup-20261104/forest-save.json`, `prefs`, `prefs.bak-…` SHA-256 unchanged (runs used an isolated `XDG_CONFIG_HOME`); licensed pack 27/27; original eight-dirty Workbench status + 8 hashes identical (HEAD `4d95e452`); `Library` 6.6 GiB, not copied or rebuilt; ZX20 36.14 GiB after. The licence copy was deleted by the driver. Primary checkout user changes (`AGENTS.md`, `Docs/Operations/StorageArchitecture.md`) untouched. `active-tasks.md` untouched.

## 8. Blockers, severity and Manager decisions
1. **Onboarding keyboard focus (J): minor severity, probable product defect.** It affects keyboard-only players who mix the Right arrow with Enter; nothing is lost (Back is recoverable). Needs a small product fix (for example, re-assert Next/Begin focus after any page change, or stop arrow navigation moving focus inside the onboarding) by a separately authorised owner, plus a native-input confirmation. **It blocks a "J PASS"**; whether it blocks a personal internal playtest is the Manager's call.
2. **Not run because Unity was stopped:** Legacy40/native-Reference old-world guard (`Ccf359OldWorld`) and the v24 save contract (`Ccf359V24SaveContract`). Both are ready (typechecked). A re-lease should first fix the three probe defects and rerun session 1 once.
3. **Still open regardless:** real V2 Stage 5/7 Work Plan locks (needs an honestly played save), onboarding text-size fit, a performance baseline, and a decision on whether "access racks" should return to the intro.
4. **Strict Workbench runner exit 1 remains FAIL**; any personal playtest needs an explicit Manager risk admission of that documented environment-only residual. No `main` merge, no Player/Windows build, #316 **not** started, personal playtest **not** released.

## 9. Evidence and reproduction
* Evidence (new directory; #358 evidence was not modified): `/media/jer/Files/CCF/Verification/CCF-359-IndependentAuditPlaytestClosure-20261109/`: `phase1/` (TeachingPersistence offline rerun), `triage/` (independently rebuilt protection manifests), `probe/` (staged bytes), `driver/`, `holder.sh`/`gate.sh`/`restore12.sh` (#358 copies, identity/path changes only; the Files-lane admission check additionally allows the uncommitted probe folder), `run/` (holder log, lock id, disk monitor, `v2closure/` log + 29 evidence files, `gates/v2closure/`, `restores/after-v2closure/` before-image + diff), `screens/gray/`, `post/post-release-checks.txt`. SHA-256 manifest: `evidence-manifest.sha256` (its own hash is in the #359 closing comment).
* Offline witness reproduction: `python3 -I Tools/Verification/StandMap/V2Witness/run_v2witness_offline.py --lane <lane> --probe <ABSOLUTE path> --out <dir>`.
