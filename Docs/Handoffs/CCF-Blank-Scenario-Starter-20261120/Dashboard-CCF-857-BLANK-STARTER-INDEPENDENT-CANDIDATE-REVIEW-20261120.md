# CCF #857 independent candidate review

**Verdict: NEEDS CHANGES**

This verdict covers the candidate only and confers no integration, merge or owner-acceptance right.

## Identity and scope

Requested: independent reviewer, different model and session from implementer GPT-6.2 Sol. Executed: OpenCode `openai/gpt-6.1-terra` (GPT-6.1 Terra). This session was assigned as a subagent by the caller; independence is asserted by assignment, not independently proven. The assignment included the author's handoff, so this was not a blind review. No nested reviewer, Unity launch, Unity lock, integration or candidate edit was performed.

Reviewed `cbcb3850c3952a8d74e54606279165b2f4be9add..c5e0547a03d473663510fea5edb8b86b43083c32`; source tested at `fac8a909c7028ef2d9e74cca2bc28511bce2f132`, with only the handoff changed afterwards. Specification bytes: `23da9850a7245a8623244ff2e2e76fbf46c33f0c`. Candidate remote HEAD was rechecked at the stated SHA. Review branch starts at `origin/main` `cba0a905b766ce8fc0356d351ac5bff1c2a9a008`.

## Findings

### R-01 — BLOCKING — required save witness is incomplete

`Tools/Verification/BlankStarter/run-native.sh:53-71` stages only `forest-save.json`; it explicitly asserts `.tmp` and `.bak` are absent. AC-06 requires all three pre-existing files hashed before and after, with bytes and metadata unchanged. The trace catches attempted access to absent paths, and the separate positive control proves each path can be traced, but neither substitutes for the required existing-sidecar witness. The live-path attempt did not complete the scenario. **Correction:** under an authorized isolated exact-path setup, stage distinct existing main, tmp and bak witnesses; capture hashes and metadata before/after; repeat all specified operations with the three-path trace and retain results. Do not rerun against the real host without custody approval.

### R-02 — BLOCKING — required historical determinism qualification not demonstrated

`Docs/Handoffs/CCF-Blank-Scenario-Starter-20261120/Dashboard-CCF-857-BLANK-STARTER-IMPLEMENTATION-RESULT-20261120.md`, qualification table and frozen-reference paragraph: AC-09 requires targeted Legacy40, Enlarged80 and frozen Reference Future/**determinism** checks satisfying existing anchors. The historical run checks Legacy40 starting counts, loads an Enlarged80 save and tests a round trip; offline archive hashes and `ReportFrozen` check archive integrity. These are useful but do not demonstrate the required historical deterministic simulation anchors. The handoff itself says full long-horizon deterministic simulation was not tested, while labeling AC-09 PASS for targeted regression. **Correction:** run and retain the specified applicable deterministic anchor gates on the exact integrated candidate, or mark AC-09 NOT TESTED pending that gate; do not substitute archive integrity for simulation.

### R-03 — BLOCKING — exact allowlist internally inconsistent

The assignment states exactly 26 paths and **three** files under `Tools/Verification/BlankStarter/`; the actual diff has 26 paths but **four** there: `BlankStarterQualification.cs`, `run-native.sh`, `save_witness.py`, and `shadow-save-dir.py`. All other 22 paths match the enumerated scene, source and handoff categories; no prohibited path was changed. A count of three verification files would yield 25 total, so the requested exact list cannot be satisfied literally without clarification. `run-native.sh:51-52` relies on the fourth helper. **Correction:** obtain an explicit authoritative four-file/26-path allowlist clarification, or submit a candidate that conforms to an explicitly resolved list; do not silently remove a needed helper.

### R-04 — NON-BLOCKING — host-incident explanation needs precision

`Assets/ForestPrototype/UI/LearningObjectivesView.cs:91-114` and `Assets/ForestPrototype/UI/MenuHelpView.cs:64-71,154-155` do contain production `PlayerPrefs.DeleteKey`, `SetInt`, and `Save` calls. The blanket assertion “no production code writes PlayerPrefs” is false. In the inspected preview paths the learning constructor skips reload, `Record` and UI reset guard the preview, and the help view is not created; the harness itself uses only `HasKey` (`BlankStarterQualification.cs:171`). This supports no *identified* learning/help write in the exercised blank path, not a byte-level proof that no owner key changed: prior `prefs` bytes were not preserved. **Correction:** state that the relevant production write paths exist but were not observed reachable in the tested preview, and retain the residual uncertainty and the incident's explicit not-restored status.

## Scope, design and evidence observations

- The candidate has 26 changed paths, including four verification files, as described in R-03. No `ProjectSettings`, `Packages`, `ForestTest.unity`, `ForestSaveData.cs`, `StandGeometryModel.cs`, `ForestEcologyController.cs`, or `ScenarioOneManager*` changes. Scene diff is only ground scale, name, width/depth and test-only flag; shared script references remain the same. New `.meta` files and scene references were inspected; no GUID defect identified, not a repository-wide GUID collision audit.
- `BlankStarterPreview.BlocksHistoricalSave` is a single predicate shared by menu and save controller. `Save`, `Load`, `CanLoad` guard before capture or `File.Exists`; F5/F9 call those guarded public methods. F10 builds the blank-only menu before its historical `File.Exists`; startup Continue/refresh and start-new check the predicate before probing; `SaveAndClose` and `Restart` guard first. Restart correctly uses the marker even when Unity assigns the unlisted scene build index 1. No earlier access was identified in the inspected paths. Guards refuse by menu message or omission; direct `Restart`/`SaveAndClose` return silently while the existing F10 refusal remains visible.
- No preview manager/ecology is created to fill nulls. Exact three empty-state messages are in the views; no help view is created and no rack tool/markers are installed. `ForestPlayer.OnGUI` returns for a cached preview or an existing manager; the old manager-present behavior is preserved. Inputs in the harness are synthesized through Unity's Input System, not physical keyboard/mouse.
- Rectangle projection uses `max(width,depth)` once in `StandMapTransform`, centered and north-up; minimap and full blank map share that transform. The 45 × 60 screenshot shows the expected 3:4 boundary. Historical renderer and saved geometry files were not rewritten; the existing 176/176 offline square-map log and targeted historical screenshots/log support bounded compatibility, not all historical states.
- `isolation-retry/exact-shadow-save-access.strace` is actually zero bytes; `run-native.sh:62-68` uses `strace -f` with three literal `-P` arguments, and `exact-trace-positive-control.strace` records open/read of each. `isolated-final` is not zero bytes and was correctly not used as the clean trace. `isolation-retry/exact-shadow-console.log` reports 109 checks; `historical-final/historical-console.log` reports 17. Examined the rectangle screenshot and code/log claims, not every screenshot pixel or all runtime assertions independently.
- `native-r1/save-prior.json` and `save-restored.json` record the save slot absent, backup unchanged, no tmp; `host-side-effect-summary.txt` records changed prefs/analytics/insights. The handoff honestly says the host directory was not restored byte-for-byte and the first run stopped early. The isolated reruns compare against the **post-incident** host state only. Owner adjudication that Unity housekeeping caused the changed keys is plausible but cannot be established from before/after hashes alone; R-04 qualifies the reasoning.

## Requirement and acceptance matrix

PASS means supported for the inspected candidate and cited bounded evidence, not independently rerun. PARTIAL means useful evidence exists but full requirement remains unproven. NOT TESTED is an unrun required gate.

| ID | Verdict | Basis / limitation |
|---|---|---|
| FR-01 | PASS | measured collider, spawn, walking and invalid input in blank run |
| FR-02 | PASS | shared scripts, scene diff and GUID references inspected |
| FR-03 | PASS | no active manager/ecology; negative controls exercised |
| FR-04 | PASS | shared UI headings and preview navigation |
| FR-05 | PASS | empty layers, rectangle transform, screenshot and historical transition check |
| FR-06 | PASS | exact plan message; safe controls and key checks |
| FR-07 | PASS | exact review/objective states and guarded progress |
| FR-08 | PARTIAL | source fence and three-path trace support no access; required existing sidecars not staged, R-01 |
| FR-09 | PASS | individual empty/value/clear slot checks and visible preview indicator |
| FR-10 | PASS | two scene diff and common GUIDs; no copied C# |
| FR-11 | PASS | unlisted-scene entry and marker-based restart refusal |
| FR-12 | PARTIAL | targeted legacy/80 and archive evidence; deterministic simulation anchors outstanding, R-02 |
| FR-13 | PASS | preview/unsaved banner and test-only copy; no Windows build claimed |
| AC-01 | PASS | compile, bounds, spawn, synthetic input, invalid cases |
| AC-02 | PASS | UI, inspection and shared sources |
| AC-03 | PASS | seven layers, rack negatives and transition check |
| AC-04 | PASS | plan message, tab and safe actions |
| AC-05 | PASS | review/objective messages and no lesson progress |
| AC-06 | PARTIAL | clean trace and positive control, but existing sidecar witnesses missing; original host restoration failed, R-01 |
| AC-07 | PASS | measured scene/map rectangle, scene-only difference |
| AC-08 | PASS | three actual slots exercised separately with banner |
| AC-09 | NOT TESTED | required integrated-candidate deterministic anchor qualification not demonstrated; R-02 |
| AC-10 | PARTIAL | this candidate review performed; exact allowlist unresolved R-03, integration/owner custody not decided |

## NOT CHECKED / NOT TESTED

No Unity run or new compilation by reviewer; no physical input, Windows build, exhaustive historical v1–v24 replay, long-horizon determinism, exact integrated candidate, full screenshot-by-screenshot visual audit, or recovery of unavailable host preference bytes. No review verdict can grant authority to integrate. Required next step: resolve R-01–R-03 with the authorized owner and arrange a fresh bounded review of the resulting candidate and evidence.
