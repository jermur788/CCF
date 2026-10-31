# Scenario One Combined HUD / Focus Regression Repair 01 (#199) — Handoff

Status: **PARTIAL — implementation of the competitor family complete, gate verification pending; estimate/minimap + FOCUS_04/06/07 STOPPED for Manager disposition.** No Unity was run by the implementer.

- Branch: `fix/scenario-one-combined-hud-focus-regression-v1`
- Exact start: `ce6737146a7aa9ea953d7db0ca93b103f6c2c996`
- Implementation commit: see `git log` on the branch (the commit touching `CombinedHudComposition.cs`); this handoff is committed after it.

## Changed files
- `Tools/Verification/ScenarioOneCombined/CombinedHudComposition.cs` — verification fixture only (`RackOwnedTreeIds()`, `Choppable()` now skips trees owned by a Rack V2 plan). No assertion edited, removed, skipped or loosened.
- No production file under `Assets/` changed. No-production-outside-UI statement: nothing outside `Tools/Verification/` was touched.

## Root cause per family

### 1. Estimate / minimap (`ESTIMATE_MINIMAP_STAYS_VISIBLE`, 1280 `ESTIMATE_PANEL_DOES_NOT_OVERLAP_THE_MINIMAP`) — PRODUCTION CHANGE CONFLICTING WITH PROTECTED CONTRACT, NOT REPAIRED
- Culprit: PARITY-07 commits `c158c95f` / `5cbde15d` (merged via "Integrate PARITY-07 ... rendered qualification"):
  - `Assets/ForestPrototype/UI/ScenarioOneUiRoot.cs:240` `hud.ShowMinimap(!(inspecting && (inspection.ShowsCompetitors || inspection.ShowsEstimate)))` — minimap now hides for every ordinary (non-crop) E-inspection.
  - `TreeInspectionView.cs:19,83` `EstimateWidth = 600` (was 400).
- Evidence this was deliberate: `Tools/Verification/Parity07Unity/HANDOFF.md` lines 19, 25, 37: the new PARITY-07 estimate copy needs ~445–490 ref units of height but only ~245 exist between the status column and the minimap (non-scrolling panel, cursor locked); the worker hid the minimap and explicitly flagged "Minimap is hidden while a non-crop tree is inspected (new, deliberate, presentation-only); reviewer may prefer a different trade-off." No Manager acceptance of that trade-off was found.
- Protected assertions involved (`CombinedHudComposition.cs:370-375`): `ESTIMATE_MINIMAP_STAYS_VISIBLE` ("the minimap only steps aside for the competitor list"), `ESTIMATE_PANEL_DOES_NOT_OVERLAP_THE_MINIMAP`, plus `ESTIMATE_PANEL_STARTS_UNDER_THE_HUD_COLUMN`, `_IS_ON_SCREEN`, `_IS_NOT_CLIPPED`.
- Why not repaired: the two contracts cannot both hold with the current copy by a trivial change. Keeping the minimap visible leaves ~245 units, versus 445–490 needed. Moving the 600-wide estimate panel left of the minimap would satisfy the Combined assertions but would collide with the central aiming prompt and the expanded 600-wide detail column (PARITY-07 `INSPECTION_CLEARS_CENTRAL_PROMPT` / `ESTIMATE_CLEARS_CENTRAL_PROMPT`, 1031/0 baseline). That is a layout/product trade-off I cannot verify without Unity.
- **Manager decision needed** — options: (a) restore minimap during estimate and relayout the estimate beside the minimap (needs PARITY-07 prompt/detail-column re-qualification); (b) shorten/condense estimate copy (touches PARITY-07 wording, out of scope); (c) formally supersede `ESTIMATE_MINIMAP_STAYS_VISIBLE` (and the README row) because PARITY-07 now accepts the minimap stepping aside for any inspection card. The 1280-only overlap failure is not independently explained offline; it is likely a consequence of the same reserved-height/hidden-minimap geometry (`PlaceSidePanel`, `ScenarioOneUiRoot.cs:290-302`) and must be re-read from the next gate log after the disposition.

### 2. Competitor panel/tags (1920, V2 only: `COMPETITOR_PANEL_IS_SHOWN`, `COMPETITOR_TAGS_ARE_SHOWN`) — STALE FIXTURE ORDERING, repaired in the harness
- `Main()` loops sizes in one session (`CombinedHudComposition.cs` ~line 113-133): at 1280 the `focus` section confirms the Rack V2 plan (`FOCUS_06_V2_AUTHORITATIVE_PLAN_CONFIRMED`). At 1920 `CompetitorState` marks `DenseSpot().Tree` as Crop Tree; RACK-V2 changes (`ForestTreeMarkingManager.cs` Mark(): `IsRackRemovalTree`/`HasRackEntranceMark`, `ForestTreeMarkingManager.RackEntrances.cs:33-50`) correctly refuse Crop/Fell marks on trees owned by a confirmed rack. The mark is silently refused, `tree.IsCropTree` is false, `TreeInspectionView.Refresh` never sets `ShowsCompetitors` (`TreeInspectionView.cs:114`), so no panel/tags.
- Consistent with the evidence: V2 only (legacy has no V2 plan), 1920 only (second size), and the refusal is the accepted save-v21/D-064 rule, not a UI defect.
- Fix: `Choppable()` (used only by `DenseSpot()`) excludes trees in any rack's `removalTreeIds` or `entranceTreeId`. Assertion text/conditions unchanged; first-size and legacy behaviour unchanged (no racks ⇒ empty set). This is an inference from reading code, not run: verify at 1920 V2.

### 3. FOCUS_04 / 06 / 07 (both sizes, both modes) — same root cause as family 1
- `FocusTransitions()` calls `Inspect(spot)` (FOCUS_01) and never ends the inspection; `isInspecting` stays true across Objectives/Map (`ForestPlayer.cs:61,898`; player disabled so nothing clears it). After closing a panel the walking HUD is shown with the tree still inspected, so the PARITY-07 rule at `ScenarioOneUiRoot.cs:240` hides the minimap.
- Protected assertions: `FOCUS_04_MINIMAP_AND_OBJECTIVE_ARE_BACK` (line ~517), `FOCUS_06_MINIMAP_SHOWS_THE_NEW_RACK_AFTER_CONFIRMATION` (~561), `FOCUS_07_MINIMAP_IS_BACK` (~616). Before PARITY-07 the minimap hid only for competitor lists, so these passed.
- Not repaired separately: the fix is the family-1 disposition. Ending the inspection in the harness before FOCUS_04 would hide a real player-visible case (close a panel while a tree is still inspected), so I did not do it.

## Why this is minimal / does not weaken tests
Only the fixture's tree selection changed, to avoid trees that production deliberately protects. No threshold, expected value, section or check was changed.

## Gate commands for the orchestrator (serialized Unity lane, clean locked lane)
```
python3 -I -B Tools/Verification/RackV2NoCompetingAuthor/run_gate.py --gate combined-v2
python3 -I -B Tools/Verification/RackV2NoCompetingAuthor/run_gate.py --gate combined-legacy
```
with `CCF_UI_SIZES=1280x720,1920x1080` (default) and `CCF_COMBINED_SECTIONS` unset. Affected regressions if production is later changed: `Tools/Verification/Parity07Unity/run_gate.py`, Stand Map/minimap and guided-v2 (post-#198) gates.

## Expected results from this commit alone
- `COMPETITOR_PANEL_IS_SHOWN` / `COMPETITOR_TAGS_ARE_SHOWN` at 1920 V2: expected to pass. 
- Still failing, by design pending disposition: `ESTIMATE_MINIMAP_STAYS_VISIBLE` (all runs), 1280 V2 estimate overlap, `FOCUS_04/06/07` (both sizes, both modes). Expected remaining count: V2 ≈ 9 fails, legacy 8.

## Unverified
Everything: no Unity run. The 1920 competitor diagnosis is code inference; the 1280 overlap cause is unconfirmed; whether the Manager accepted the PARITY-07 minimap trade-off is not recorded in the repo.
