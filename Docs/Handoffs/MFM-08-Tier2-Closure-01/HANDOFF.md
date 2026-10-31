# MFM-08 Tier-2 Closure 01 — Handoff (claim #195)

**Result: BLOCKED — `RackV2MapPlanning` FAILS on the MFM-08 candidate; the three Rack gates are still RED on current integration.**
`MFM-08 TIER-2 CLOSURE PASS — READY FOR MANAGER REVIEW` is **not** returned.

- **Worker:** Claude Code, Sonnet 5.5. Verification only; no production, test, scene or schema file changed.
- **Branch/worktree:** `verify/mfm-08-tier2-closure-v1` (docs only) at `/media/jer/Files/CCF/Operations/CCF-MFM08Closure`, from origin/main `b7c86313`. **Lane:** `/media/jer/ZX20/CCF-U1`.
- **Lock:** normal `unity-lock`, lock_id `8b1f957b6dd04c5d8af1c8f6368a3237`, task `MFM-08-195-CLOSURE-UNITY`, released normally (forced: false), status `free`, no Editor/Xephyr left.
- **Exact refs executed:** candidate `e5fb424635f16182748efcb0539e8a49539495a9`; feature start `cfdc93bf3943125d21570bc5749a9338f52d1cce`; integration `ce6737146a7aa9ea953d7db0ca93b103f6c2c996` (cfdc93bf is its ancestor, 29 commits back). Lane returned to `e5fb4246`, clean.
- **Evidence root:** `/media/jer/Files/CCF/Verification/MFM08-Closure01-20261031T072158Z/` (lock receipts `MFM08-Closure01-unity-lock-*.json` beside it).

## A. RackV2MapPlanning on e5fb4246 — FAIL
Command (from the README, lane at the candidate, lock held):
`python3 -B Tools/Verification/ScenarioOneProfessionalMap/run_unity_harness.py --name RackV2MapPlanningVerification --method RackV2MapPlanningVerification.Begin --files RackV2MapPlanning/RackV2MapPlanningVerification.cs --out <fresh> --private-display --keep-focus --screen 1920x1080 --sizes 1280x720,1920x1080 --timeout 1800 --env CCF_TEST_DIRECT_ENTRY=1`

- Result: `RACK_V2_MAP_PLANNING_UNITY_FAIL checks=17 shots=0 exception=System.InvalidOperationException: five navigation tabs remain navigation` (`A-mapplanning/`).
- **Cause (read from source, no production change):** the gate asserts `tabsNow.Count == 5` (`RackV2MapPlanningVerification.cs:318`). MFM-08 changed `StandMapView.cs` to add a sixth layer button, "Treatment review", whenever `RackPlanningV2Active` (enum `Layer` gains `Review`; `AddLayerButton("Treatment review", Layer.Review)`). The test predates that intended feature.
- **Comparison on the exact feature start `cfdc93bf`** (no production change, checkout only): `RACK_V2_MAP_PLANNING_UNITY_PASS checks=142 shots=8` (`A2-mapplanning-at-feature-start/`).
- So the failure is introduced by MFM-08 and is an expectation conflict, not a crash: the assertion needs a Manager-authorised update to six tabs (or an exemption for the Review tab). I did not weaken it. Because the gate stopped at check 17, **the rest of RackV2MapPlanning's 142 checks (planning mode, dashed layer, legend, save/hash unchanged by suggestions) were not exercised on the candidate.**

## B. Diagnostic on current integration ce673714 — still RED
| Gate | ce673714 | Candidate e5fb4246 (earlier) | cfdc93bf (earlier) |
|---|---|---|---|
| combined-v2 | 447 pass / 11 fail | 447 / 11 | 447 / 11 |
| combined-legacy | 440 / 8 fail | 440 / 8 | 416 / 32 |
| guided-v2 | FAIL (annual-advance lesson plus the "fresh rack review" sequence checks: expected lesson index 4 got 6/7; "three future trees and six stand portions reviewed [0 0]") | FAIL (same annual-advance assertion) | FAIL (same) |

combined-v2 failing checks (identical set on integration): `ESTIMATE_MINIMAP_STAYS_VISIBLE`, `ESTIMATE_PANEL_DOES_NOT_OVERLAP_THE_MINIMAP` (1280), `COMPETITOR_PANEL_IS_SHOWN` / `COMPETITOR_TAGS_ARE_SHOWN` (1920), `FOCUS_04/06/07` at both sizes. combined-legacy: `ESTIMATE_MINIMAP_STAYS_VISIBLE`, `FOCUS_04/06/07` at both sizes. The Rack failures therefore **exist on the live integration lineage, independent of MFM-08**; I did not repair them (separate Rack V2 claim per the packet). I did not look for a lineage explanation since nothing went green.

## C. MFM-08 evidence
Accepted gate (353 / 373 checks on e5fb4246) not rerun; nothing new made it ambiguous.

## Changed-file boundary and return
No `Assets/`, Rack, MFM, save, scene or settings file changed. Editor side effects (`.vscode`, material noise, `Assets/_Recovery`) were saved as `*.editor-normalisation.diff` / `*.recovery` in the evidence root and reverted; lane clean at `e5fb4246`; host saves untouched by the runners.

## Can #156 be upgraded to Manager APPROVED?
Not by this evidence. Needs: (1) a Manager decision on the stale five-tab assertion in RackV2MapPlanning (a small test-expectation update to six tabs in V2 worlds, then a rerun on the candidate to exercise the remaining 125 checks); (2) a separate Rack V2 repair claim for guided-v2 / combined-v2 / combined-legacy, red on integration.
