# RACK-V2-06 — spacing teaching handoff

**SPACING TEACHING READY — READY FOR MANAGER REVIEW**

Starting `origin/main`: `ead50223b79fd653533822b609be7e7ce2cc5cbf` (descends from RACK-V2-05 PR #44 merge `0ec6bc4d473975e9938040026737e70a33432c48`). Executor: T3 Code / OpenAI Codex `gpt-6.1-terra` xhigh. Branch `feature/scenario-one-rack-v2-spacing-teaching-v1`, authoring worktree `/media/jer/ZX20/CCF-RackV2-06`. Production commit `43137f4dacf0f44a2731b53de36b512c6af10d95`; fixture-only correction `5f7ff19aa57476a53d81085752993d8b006a236d`. **Exact Unity-tested candidate:** `5f7ff19aa57476a53d81085752993d8b006a236d`. Final tip is the documentation-only commit containing this handoff; `git log -1 -- Docs/Handoffs/RACK-V2-06/HANDOFF.md` identifies it. No production or verification bytes changed after the tested candidate.

## Implementation and evidence boundary

- `Racks/RackEditing.cs` adds a pure nearest-centre-line query using the existing segment-distance geometry. Equal distances choose the lowest rack ID. It does not alter geometry or editing rules. Its inclusive 16 and 20 m thresholds are Scenario One **GAMEPLAY CALIBRATION INFORMED BY DIRECT SOURCE GUIDANCE** (D-064), not a universal CCF rule.
- `UI/Map/StandMapRackEditor.cs` derives the selected rack or live proposal from the existing authoritative plan/ghost and compares it with `RackEditOthers`, which reads player-plan racks and excludes the edited rack. Dashed suggestions are never counted. The readout explicitly says “closest centre-line distance”; it can change during rotation and reports no neighbouring player rack honestly.
- `UI/StandMapView.cs` displays the site-specific 16–20 m advisory and below/within/above wording, refreshing during pointer movement. Existing invalid-plan reason and soft snapping remain in place; off-band placement remains possible.
- `Tools/Verification/RackV2Spacing/` contains focused pure checks and a disposable graphical gate adapted from RACK-V2-05, plus run instructions. No scene, prefab, material, save schema, package or project setting was changed in the implementation.

RACK-V2-02's reconstruction (`/media/jer/Files/CCF/Verification/RackV2-02-Geometry-20261026T223940Z/HANDOFF.md` §7) measured approximately nine planting-row intervals for 18 m, but its final verdict was a geometry stop and explicitly left review/acceptance to the Manager. We did not establish that a player-facing row equivalent was accepted for the revised stand, so the optional row count is omitted. This avoids presenting an unaccepted **MODEL/SCENE DERIVATION** as settled.

## Verification

Durable evidence root: `/media/jer/Files/CCF/Verification/RackV2-06/`. The offline checks below were run against `43137f4` production bytes, identical at `5f7ff19`:

- `python3 Tools/Verification/RackV2Spacing/run_offline.py --out <evidence>/offline-spacing`: 11 PASS, including deterministic ties, no neighbour, both inclusive boundaries, below/inside/above, convergence and unchanged inputs.
- Static inspection/check: player plan only, own ID excluded, no suggestion or plan-mutation calls from spacing display; site-specific advisory copy; no row-equivalent output.
- `RackV2Editing/run_offline.py`: 61 PASS. `StandMap/OfflineLogic/run_standmap_check.py`: 165 PASS, including dashed/solid distinctions. `RackV2SuggestedLayout/run_offline.py`: 147 PASS and read-only static PASS. Runtime source type compile against Unity 6000.6 reference assemblies: 0 errors.
- Unity 6000.6.0f1 graphical Editor, U1 clean/detached at exact `5f7ff19`, isolated config and real desktop pointer: `RackV2Spacing/run_gate.py --gate editing` PASS **344 checks / 25 captures**, exit 0; `--gate map` PASS **142 checks / 8 captures**, exit 0. Both 1280×720 and 1920×1080 exercised no neighbour, within/below/above, live move/rotate, free off-band placement, invalid reason, and retained edit behaviour. The editing gate checks the full save around refusals and unchanged world outside rack-plan changes; the map gate confirms display work does not mutate its fixture. Captures and logs are in `unity-spacing-run2/` and `unity-map/`. Representative within, below, above, no-neighbour and rotation images were inspected for readability at the supported sizes. These are automated Editor captures, not a human acceptance or Player-build review.
- First Unity run at `43137f4` stopped at the added below-band assertion after 189 checks: the rotated fixture remained within band. The diagnostic is retained in `unity-spacing-run1/`. `5f7ff19` changes only the fixture: it explicitly moves a rack close and undoes the move, and puts the within fixture away from the 20 m boundary. The complete gate was rerun rather than carrying forward partial results.
- Machine-wide lock `c8723885a3de468293ba944905b1f95a` (first run) and `dae8a47a98eb403897449b6b874fae11` (final gates) acquired and released normally. U1 began and ended clean; generated scripts and `.meta` removed. Unity-generated changes to four initially clean settings/material files were captured as evidence patches and restored to the candidate between runs and after the last run. No Editor remains. MFM-03 remained independently dispatched; the lock was free when acquired.

## Manual check

In Unity 6000.6.0f1, open the project on this branch, start a new Enlarged80 Scenario One, open Stand Map and choose **Plan Racks**. Draw one rack: expect “No neighbouring player rack yet” despite dashed suggestions. Draw a second roughly parallel rack about 18 m away: expect a live distance and “within” text beside the advisory band. Select and drag it closer than 16 m, then wider than 20 m, and rotate its handle: expect the distance and status to update while dragging; release at valid off-band positions and expect the rack to remain there. Drag into an invalid overlap: expect a reason and no change on release. Repeat at 1280×720 and 1920×1080.

## Limits and coordination

No whole Objective-1 qualification, final appearance acceptance, integration, user acceptance, Player/Windows build or further Rack V2 tasks are claimed. Live coordination was edited only under the amendment: 04, 05 and 06 rows; unrelated rows preserved. Claim #45 was marked RUNNING before production edits. The original dirty Primary worktree was untouched. Dashboard ingestion/audit/publication readback was not available or verified in this session and remains an operational follow-up; no generated dashboard state was edited.
