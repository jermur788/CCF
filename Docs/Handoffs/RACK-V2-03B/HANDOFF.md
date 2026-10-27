# RACK-V2-03B — SAVE V21 / WORLD IDENTITY / PERSISTENT RACK FOUNDATION — RUN RECEIPTS

- Task: RACK-V2-03B (packet; spec SPEC-scenario-one-objective-1-rack-planning-field-marking-v2-V1, APPROVED)
- Worker: SOL/CODEX (opencode / mimo-v2.6-pro), Primary role
- Base: `089dbebd4098c71fdf092a0c04fd05a54d8db486` (packet implementation baseline)
- Context commit (docs/spec authority): `2a183c11d8503c550cba30394d5b363cfc0e6d07`
- Branch: `feature/scenario-one-rack-v2-save-foundation-v1`
- Final HEAD: `c6342f79a3e89c040fadd232017a937132779385`
  - `cf84d24` save: persist Rack Planning & Field Marking V2 state as save v21 (D-064)
  - `2f8fca5` test: assert the rack workflow flag alone never moves a world hash
  - `c6342f7` fix: hash the pre-v21 layout with the v20 header the accepted anchors were computed against
- Lane: `/media/jer/ZX20/CCF-U3` (registered Unity lane), detached at `c6342f7` for the gate chain, restored
  afterwards to `feature/scenario-one-player-test-non-ui-remediation-v1` @ `fc8042c37939838a4a525b75e895016698b1cb80`, clean.
- Unity lock: `bedf9882421245b387551429823b4f30` acquired/released normally (chain 1); second lock
  acquired/released normally (chain 2). Free, no Editor PIDs at release.

## Unity gate chain (final HEAD `c6342f7`, all PASS)

| Gate | Verdict | Seconds | Required tokens | Result |
|---|---|---|---|---|
| 01c RackV2SaveVerification (new) | PASS | 69 | RACKV2SAVE_VERIFY_PASS | 35 checks, 0 failures |
| 02b StandGeometryVerification | PASS | 63 | STAND_GEOMETRY_VERIFY_PASS, STAND_GEOMETRY_START_WORLD_HASH 5546B9B3B50673B2 | 10 check groups |
| 03b Enlarged80Verification | PASS | 114 | ENLARGED80_VERIFY_PASS, ENLARGED80_START_WORLD_CURRENT_HASH 94589C749852F359, ENLARGED80_YEAR3_CURRENT_HASH 92F9635566BF6AFE | 12 check groups |
| 04b ScenarioReferenceVerification | PASS | 94 | REFERENCE_FUTURE_V1_CONTRACT_PASS, 7AD177B3CC2F73C7, BFFF66015A6619DA | archive + preview + continuation |
| 05b ScenarioOneCompletionVerification (growth 2) | PASS | 79 | SCENARIO_ONE_COMPLETION_VERIFY_PASS, SCENARIO_ONE_COMPLETION_GROWTH2_LAYOUT_PASS 9C77EB4BEE427FB9 | 12 phases |
| 06b ScenarioOneCompletionVerification (historical model-1 fixture) | PASS | 69 | SCENARIO_ONE_COMPLETION_VERIFY_PASS, 702766DECE591E21 | v18 projection exact |

`gates/<name>/result.json` holds each verdict and per-token presence; `gates/<name>/` holds the full Editor log.

## Protected fingerprints — before / after (NO REBASELINE)

| Fingerprint | Meaning | Before | After (c6342f7) | Status |
|---|---|---|---|---|
| 9C77EB4BEE427FB9 | completion growth-2 world (Legacy40, v19 layout) | 9C77EB4BEE427FB9 | 9C77EB4BEE427FB9 | EXACT |
| 5546B9B3B50673B2 | StandGeometry fresh Legacy40 start (v19 layout) | 5546B9B3B50673B2 | 5546B9B3B50673B2 | EXACT |
| 94589C749852F359 | Enlarged80 fresh start (v20 layout) | 94589C749852F359 | 94589C749852F359 | EXACT |
| 92F9635566BF6AFE | Enlarged80 year 3 (v20 layout) | 92F9635566BF6AFE | 92F9635566BF6AFE | EXACT |
| 7AD177B3CC2F73C7 | Reference Future v1 archive Year 100 | 7AD177B3CC2F73C7 | 7AD177B3CC2F73C7 | EXACT |
| BFFF66015A6619DA | Reference continuation v15 layout | BFFF66015A6619DA | BFFF66015A6619DA | EXACT |
| 702766DECE591E21 | frozen historical v18 growth-model-1 completion | 702766DECE591E21 | 702766DECE591E21 | EXACT |
| EBD49ADEF18C3F36 | Reference continuation diagnostic (informational) | EBD49ADEF18C3F36 | EBD49ADEF18C3F36 | EXACT |

An intermediate run at `cf84d24` (before `c6342f7`) showed the two Enlarged80 markers at
`D0C1B0ACA75320C6` / `081D3AC8664BA727`: the v21 strip removed the rack bytes but not the bumped
version header, which `CurrentWorldHash` includes. `c6342f7` makes the pre-v21 layout the exact v20
byte stream (no rack bytes + v20 header); the two markers are exact again. The Legacy40-family anchors
were already exact at `cf84d24` (those functions set/rewrite their own headers). Retained as evidence:
`gates/03-enlarged80/` (the pre-fix values) and `gates/03b-enlarged80/` (exact).

## Offline verification (final HEAD)

- Runtime source type-compile (player assemblies, 165 sources incl. the new gate): 0 errors.
- N3 offline (save validators + completion rules): N3_OFFLINE_PASS checks=88.
- Site infrastructure offline (includes the save-version source pin): SITE_INFRASTRUCTURE_PASS checks=63.
- Three-species history offline: THREE_SPECIES_HISTORY_PASS checks=74.
- Non-UI offline: NONUI_KEYBOARD_PASS 46, NONUI_RACKBAY_PASS 87, NONUI_APPROACH_PASS 49.

## Pre-existing baseline defects (NOT caused by this task; reproduced on pristine `089dbeb`)

- `Tools/Verification/ScenarioOneIntegration/check_sources.py` — `unowned production change:
  Assets/ForestPrototype/ForestEcologyController.cs` (stale accepted-sources manifest; fails identically
  on the untouched base).
- `Tools/Verification/ScenarioTwoSpeciesFoundation/run_species_foundation_check.py` — `FAIL scope
  problems` (187 on pristine base; 222 with this task's added files; the scope check was already failing).
- `Tools/Verification/ScenarioOnePresentationReview.cs` asserts `CurrentVersion == 17` (already stale
  before this task); left untouched.
- The known `UnityEditor.Search` startup diagnostic appears in every Editor log (documented, pre-existing).

## Cleanliness

- `CCF-RackV2-03B`: clean at `c6342f7` (0 uncommitted files).
- `CCF-U3`: clean including untracked at `fc8042c` on its original branch. Known Editor side effects
  (`.vscode/settings.json` + 11 SectionFive materials, the documented re-import family) occurred in both
  chains; preserved byte-exact as `u3-editor-side-effects.patch` and `u3-editor-side-effects-run2.patch`
  (identical sha256 `ed23273008dfcd5dc0feedbbcda87d3b1aecca0d294ed111280f5f7b6130b1d5`) and restored.
- `CCF-Primary` (coordination anchor): untouched; its four pre-existing entries preserved.
- No harness or generated `.meta` left under `Assets/ForestPrototype/` (gate runner removes staged files
  and metas; U3 verified clean).
- Nothing pushed; no PR; no Windows build; no scene or serialized Unity asset edited.

## Administrative publication and accepted result

The user accepted RACK-V2-03B as **TECHNICAL PASS** on 2026-10-27. This handoff was reconstructed from the retained run receipts and the user's acceptance for retroactive GitHub publication; it does not assert a new implementation or a rerun of the gates.

Save v21 persists the explicit `rackPlanningModel` world identity, authoritative rack records and rack-owned work orders. Existing Enlarged80 saves remain model 0; fresh Enlarged80 worlds select model 1. Malformed V2 saves are rejected before mutation. The six protected fingerprints above remain exact without rebaseline, and Reference Future remains untouched. The implementation checkpoint is `c6342f79a3e89c040fadd232017a937132779385`; the separate commit containing this handoff is administrative only.

External evidence archive: `/media/jer/Files/CCF/Verification/RackV2-03B-SaveFoundation-20261027T011613Z/`. The gate logs and result files there are the primary evidence for the reported results and limitations. The historical “Nothing pushed” statement above describes the original run, not this later publication.

RACK-V2-02B-R and RACK-V2-03C are authorised from the implementation checkpoint subject to registered non-overlapping ownership. MFM-02 waits on the 03C confirmed-rack runtime/API. The two lanes require deliberate integration and combined gates before downstream implementation. No Windows build is authorised.

DASHBOARD SYNC: PENDING. PUBLISHED DASHBOARD REFLECTS RACK-V2-03B: NO, per the user's current status; verify ingestion and published state separately. Publication of this handoff alone does not close that operational issue.
