# Sen-1 #601 — Independent review of #600 Compact parent + planting source-bridge DRAFT amendments

**Date:** 16 November 2026. **Role:** independent non-author Reviewer. #600's author was OpenAI GPT-6, and this reviewer is not that author or its child.
**Actual reviewer:** Anthropic Claude Opus 5.5 (`claude-opus-5-5`), Claude Code CLI (T3 Code host) on the owner's local Windows machine, git + GitHub REST API (no `gh`). The effort setting is not self-observable.
**Claim:** [#601](https://github.com/jermur788/CCF/issues/601). I created it and posted `[RUNNING]` registration comments on it before the first tracked edit.
**Review branch:** `review/sen-1-601-600-compact-source-bridge-review-20261116-v1`. I pushed it **unmodified** from the reviewed tip before writing.
**Starting HEAD (= reviewed #600 final tip):** `1b50879725c440221c806c28464ca658fc36fd6f`. **#580 base:** `974ae8b9a8f2ca5ef24c068c5506523a30b2f377`.
**Worktree:** a separate sparse local docs-only git worktree. I read production source with `git show origin/main:<path>` (main at `afad222f`). I did not open a Unity project and held no machine or Unity lease.
**Write scope:** this one Markdown file only. I did not change any spec, checklist, C#, scene, Assets, ProjectSettings, save, main/live board or Drive.

## Verdict

**NEEDS CHANGES — DRAFT DOCUMENT / SOURCE-CONTRACT QUALITY ONLY.**
**0 blocker / 2 major / 7 minor / 3 nit.**

This verdict does **not** approve any spec. It does not grant implementation, Unity, scene or save authority, and it is not owner acceptance. All eight files remain **DRAFT / NOT APPROVED**.

The amendments are additive. They are largely accurate against current source, and they correctly protect the historical worlds, the 2 Oak + 2 Beech count, FR-06 STOP and the failed O1/O2 guard envelopes. Two defects should be corrected before #424 routes these drafts onward:

- **M1:** a systematic traceability error that contradicts the signed save policy and the #580 base.
- **M2:** a scene-startup contract that does not match how the session bootstrap actually behaves.

Both are bounded text repairs to the same eight files.

## 1. Administrative / preservation checks (all PASS)

| Check | Method | Result |
| --- | --- | --- |
| Diff scope | `git diff --stat 974ae8b9 1b508797` | 9 files: 8 allowlisted DRAFT spec Markdown + the one result Markdown; **193 insertions, 0 deletions** (`grep -c '^-[^-]'` = 0). No source, Unity, Assets, ProjectSettings or save paths. |
| Parent checklist | Extracted `- [ ] R-NNN` lines at base and tip; `cmp` | **115/115** unique R IDs, 115 unchecked, 0 checked, **byte-identical** to base. |
| Child checklist | Same for `CIP-NNN` | **33/33** unique, 33 unchecked, 0 checked, **byte-identical** to base. |
| New IDs / checkboxes | Diff grep for `^[-+]\s*- \[` | None added or removed. **148 unchecked items preserved.** |
| Status labels | Read all eight additions | Every section says DRAFT / NOT APPROVED / no implementation authority. |

## 2. Source claims independently verified as accurate

All of these were read from `origin/main`:

- `ProjectSettings/EditorBuildSettings.asset`: exactly one enabled scene, `Assets/Scenes/ForestTest.unity`. There is no `ScenarioOneCompact30.unity` asset, so the drafts correctly call it a *proposed* path.
- `UI/StandaloneSessionMenu.cs`:
  - `[RuntimeInitializeOnLoadMethod(AfterSceneLoad)] Bootstrap()` creates a `DontDestroyOnLoad` host.
  - The Editor-only `CCF_TEST_DIRECT_ENTRY` bypass is present.
  - `Restart()` calls `SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex)`.
- `UI/StandaloneSessionMenu.Startup.cs`:
  - `StartNewScenario()` only hands over the world the scene already generated.
  - Continue uses `saves.CanLoad()` then `Load()`.
- `StandGeometryModel.cs`:
  - Only `Legacy40=0` and `Enlarged80=1` exist, and `IsKnown()` covers only those two.
  - `StandGeometryPolicy.NewGameModel = Enlarged80` is a single global value, with an Editor-only override.
  - `ForSave()` treats pre-v20 saves as Legacy40.
- `ForestStartingStand.cs`: `CollectClearancePoints()` scans `Dirt Path*` (1.5 m corridor), `Forest Clearing`, the player and buildables.
- `UI/Map/StandMapSource.cs`:
  - Reads `Dirt Path` planks and `Forest Clearing`.
  - Reads racks from `RackLayoutTracker`/manager state.
  - Reads juveniles and shelters live.
- `ScenarioOneManager.PrescriptionApproval.cs`:
  - Refuses with "Prescriptions need a confirmed Rack Planning V2 plan" unless `RackPlanningV2Active && rackPlanConfirmed`.
  - Uses `ScenarioOneHeadlandGeometry` strip/pad radii.
- `ScenarioOneManager.cs`:
  - `TryDesignateExactPlanting` creates a **Pending** `PlantJuvenile` order, reserves one owned sapling and creates no juvenile.
  - `MarkAnnualReviewSeen()` records `AnnualReviewAcknowledged`.
- `UI/AnnualReviewView.cs`: the "I've read the annual results" button acknowledges only when `!showingEarlierYear`. Browsing past years does not acknowledge.
- `ScenarioOne.asset` serialized values (not just C# defaults) match the drafts:
  - starting cash 1 200 000 c (€12,000);
  - minimum harvest job 250 000 c (€2,500);
  - shelter material 500 c;
  - Oak 550 c and Beech 450 c, each with `plantingMinutes` 10.
- `ForestSaveController.cs`:
  - Uses `forest-save.json`, `.tmp` and `.bak`.
  - `CanLoad()` validates against the current scene.
  - `LoadData()` warns and loads best-effort when `version > CurrentVersion` (24).
- Signed policy: the D-A attestation (`6903e5fc`) of candidate `200add4e`. The drafts correctly keep "signed policy" separate from "implemented" (#544), keep Compact future-schema fail-closed versus historical best-effort, and keep explicit foreign-ID refusal before mutation.

## 3. Findings

### M1 (MAJOR) — "ISO-58" is mislabelled as the pre-mutation profile/scene/slot identity preflight

**Evidence:** #600 adds **12** occurrences of `ISO-58`, and no other ISO ID, across all eight files and the result:

- result §2 SAVE row ("ISO-58 selected profile/content/scene/slot and foreign/unknown/Compact-future refusal");
- parent spec SAVE-1 ("pre-mutation profile+world/scene/definition/content-version validation (ISO-58)");
- plan P0/P4;
- tasks SC-03 ("ISO-58 preflight before mutation") and SC-06 ("Signed ISO-58 three-profile policy implemented");
- parent checklist R-012–024 row;
- child CIP-023 row, child plan step 4 and child CP-S4 ("implements ISO-58 three-profile scene/slot identity isolation").

**What ISO-58 actually is:** in the signed checklist (`specs/scenario-independent-save-isolation-v1/checklists/requirements.md` at `200add4e`, line 104), ISO-58 is "**BLOCKED on CG-TRIG**". It covers compact arrival/assessment/group-designation/review-acknowledgement teaching triggers, per C-26 [M-5]. The #580 base headers in both specs and the parent checklist say this correctly: "ISO-58/CG-TRIG is BLOCKED".

**Why it matters:** #600 now contradicts its own base and the signed policy in the same files. A future save implementer could take either wrong reading:

- treat pre-mutation identity refusal as blocked behind CG-TRIG; or
- treat building the identity router as "satisfying ISO-58" and so unblocking teaching evidence.

**Repair:** relabel each occurrence to the signed criteria that actually cover pre-mutation identity/slot/backup refusal:

- ISO-07, -08, -42, -44, -48, -54, -55;
- ISO-15 for historical geometry;
- FR-07 / W-03 / H-05.

Keep ISO-58 only where CG-TRIG teaching triggers are meant, and always as BLOCKED.

### M2 (MAJOR) — The SCENE-1 startup contract misses how the bootstrap and title screen actually work, leaving the boot-scene decision hidden

**Source facts:**

1. `RuntimeInitializeOnLoadMethod(AfterSceneLoad)` runs **once per application launch**, after the first scene loads. Later `SceneManager.LoadScene` calls (including `Restart()`) do not re-bootstrap. The host persists, and after a restart the title card is not shown again (`startupScreen` becomes false on `Close()`).
2. The title screen **needs a gameplay scene**. `OpenStartupWhenReady()` polls `Open()`, which requires a `ForestPlayer`, `ForestSaveController` and `ScenarioOneUiRoot` in the scene. If they are missing it gives up after 600 frames and logs `CCF_STARTUP_SCREEN_UNAVAILABLE`.
3. Before the title opens, the first scene's `ForestStartingStand.Awake()` has already generated a fresh world under the global `NewGameModelForSession` (Enlarged80). "Start New Scenario" just hands that world over.

**Consequence:** SCENE-1 requires registering Compact "**without silently changing the legacy first scene/index**". Combined with the facts above, profile selection would always happen over an **already-generated E80 ForestTest world**. Every Compact New or Continue would then need a scene switch after the title. That conflicts with the signed policy's W-01/H-06, "validate its authored binding **before destination generation**".

SCENE-1 frames the open review choice only as "host stays vs recreated". The real decision is the **boot/title scene**: a neutral boot/title scene that generates no world, versus the ForestTest-hosted title. That decision has build-index, startup-screen-dependency and generation-order consequences.

The stale-reference concern is narrower than written, because `Open()` re-finds the player, saves and UI each time it opens. The startup card's `forestUi`-dependent members (`ControlRows`, `ShowScenarioIntroduction`) and the "one host" guarantee across a scene switch still need specifying.

**Repair:** name the boot/title-scene choice as an explicit SCENE-1 decision. State which scene is index 0 and whether that scene generates a world. Specify how the title opens without a pre-generated gameplay world, or explicitly accept and justify the pre-generated one. State that `Bootstrap` fires once per launch, and what Restart shows (title or none) for each profile.

### m1 (minor) — A second hard-coded save path is omitted

`StandaloneSessionMenu.cs` has its own `SavePath => Path.Combine(persistentDataPath, "forest-save.json")`. It controls whether the F10 card offers "Load your same-build save" and whether "Save current forest" asks to replace an existing file. This is independent of `ForestSaveController`.

The signed FR-13 already requires "both current filename call sites must use the same registry query". #600 SAVE-1 lists only `ForestSaveController`, so in a Compact world the F10 card would key off the historical file. Add this call site to SAVE-1 and the child save paragraph.

### m2 (minor) — The headland and track-approach injection is not named in WORLD-1

`ForestStartingStand.Awake()` unconditionally adds `ScenarioOneHeadlandPresentation` and sets `ecology.StandApproachExpected = true`. `StandMapSource` keys its scenery cache on `StandApproachPath.Version`.

The drafts forbid headland ramps, the old corridor and machine tracks, but never name these sources. Add them to the WORLD-1 / SC-04 hazard list next to `CollectClearancePoints()`.

### m3 (minor) — Objective and completion calibration will throw or mis-scale for a new geometry

`ScenarioOneDefinition.TargetAreaFactor(geometry)` **throws** `ArgumentOutOfRangeException` for any geometry outside `IsKnown()`. It also rounds the area ratio to an integer: 30 m gives 900/1600 = 0.5625, which rounds to **1**. Compact would therefore inherit Legacy40-sized extensive targets, such as 60 retained original trees and 3 regeneration cells out of 36. `ScenarioOneObjectives.Evaluate` receives the geometry.

LOOP-1 correctly leaves the completion rule undecided, but it should flag this dispatch hazard alongside OPEN-C5/D-074. It matches signed ISO-44's "objective dispatch".

### m4 (minor) — The rack/MFM suppression design overlooks an existing geometry-keyed seam

The existing seam:

- `RackPlanningV2Active` already requires `StandGeometryModelVersion == Enlarged80`.
- `RackFirstStageActive` requires `ScenarioOneRackLayout.AppliesTo(geometry)`.
- Prescriptions refuse cleanly in any non-V2 world.

So a distinct Compact geometry identity already yields "no V2 / no rack-first / no prescriptions" through current code. The drafts instead say these features need "explicitly bypassing / gating only for Compact".

The drafts should cite this seam. Acceptance should then require proof that each consumer honours it: `StandMapSource.ReadRacks()`'s non-V2 path, `RackLayoutTracker` lines, `ControlsReference.RowsFor` and the start guidance. That proof would replace a new Compact-only branch.

### m5 (minor) — Planting shelter and executor choices are batch toggles, and the labour quote is omitted

`SetPlantingShelters(bool)` and `SetPlantingExecution(method)` overwrite **every** open exact `PlantJuvenile` order. The current UI therefore offers all-or-none protection and a single executor, even though `installShelter` is stored per order.

The child spec's "optional protect-or-not" and "up to €20" imply mixed per-tree choices. The drafts should state whether mixed per-tree protection is new behaviour or out of scope.

The drafts also present "€20 stock-only" while omitting the labour estimate: `estimatedCostCents` is minutes × the €45/h contractor rate, so 4 × 10 min is about €30. The alternative is 40 owner minutes. The drafts say actor charge is a separate fact, but should give the source figure.

### m6 (minor) — The profile→scene "stable address" is not reconciled with the signed registry

Signed §4.1 binds a profile only by chapter, profile, world, geometry and content keys, and says "Display labels/ordinals/**scene names** … are never routing keys". #600 adds a profile→scene "stable scene address" without saying two things:

- that the scene is a *destination derived from* the signed binding, not an identity or routing key;
- that adding a scene field to the single registry must go through the one shared-save owner (#544, FR-13), not a second router.

### m7 (minor) — The new SCENE-1 obligations have no acceptance line

The new SCENE-1 obligations are:

- build registration that keeps index 0;
- a single persistent host;
- no fallback to the active build index;
- boot/title behaviour.

None has its own checklist ID; the crosswalk folds them into the save IDs R-012–024. That fits the no-new-IDs constraint for this task, but the approval revision should add explicit acceptance IDs. Otherwise these obligations are untestable as written.

### Nits

- **n1:** The crosswalk ranges are loose. R-074 (performance budget) sits in the owner-acceptance row. R-081/R-082 (evidence/dashboard) sit in the startup/save row.
- **n2:** The O1 0.141 m / O2 0.155 m / B2 0.121 m figures are restated in roughly seven places across the eight files. Referencing one source (#583 / the parent WORLD-1) would reduce drift risk.
- **n3:** The branch history has eight spec commits under two repeated messages (4× "stage…", 4× "align…"). This is harmless, but it makes per-file provenance harder to read.

## 4. Things done well (no change requested)

- The owner-approved look and count are kept separate from the unsigned manifest, geometry and guard envelope.
- The FR-06 STOP is explicit, and implementers have no discretion to shift positions.
- The drafts forbid a fake Rack V2 plan or MFM proposal.
- Exact-planting Pending semantics and the latest-only acknowledgement are stated correctly.
- The signed policy is clearly distinguished from its implementation (#544).
- Historical v1–v24, Legacy40/E80, Rack V2/MFM and frozen Reference Future v1 protections are preserved.
- No new requirement IDs were created.

## 5. Not performed / unverified

I did not run Unity, compile, run tests, check collisions, render, load or save, create scenes, or build for Windows. I made no spec, checklist, source, Assets, ProjectSettings, main/live-board, dashboard or Drive edits. Source statements above come from reading `origin/main` and are not runtime verification. I did not re-verify the #508/#583 geometry figures (189/14/8/175, O1/O2/B2 margins); I checked only that they are carried consistently and labelled as unverified.

## 6. Recommended next step (for #424 to route)

1. Run a bounded author repair on the **same eight DRAFT files**: fix M1 (relabel the 12 ISO-58 occurrences), M2 (the boot/title-scene decision and bootstrap facts) and m1–m7 as text only. Preserve the 115+33 checklist lines byte-for-byte and add no IDs.
2. Run an independent non-author **delta recheck** of the changed lines only. A full re-review is not needed.
3. Spec approval, the world manifest, OPEN-C1/C5/C7, D-083 routing, D-032, #544 implementation, native Unity qualification and the owner playtest all remain separate gates. No Windows build.

**Canonical mapping:** NEEDS CHANGES (DRAFT doc/source-contract quality). No spec status change.
