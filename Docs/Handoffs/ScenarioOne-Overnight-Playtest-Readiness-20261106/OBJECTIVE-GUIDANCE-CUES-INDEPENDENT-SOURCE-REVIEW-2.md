# Objective Guidance Cues — Independent Source Re-Review (T2 / #295)

- Reviewer: independent source reviewer sub-agent (read-only; no implementation, no Unity)
- Re-checked head: `8bd7416dd71f5ed175a45ead14dd1114ee636ce9` (branch `feature/scenario-one-next-action-guidance-v1`)
- Prior reviewed head: `df6415c9` (implementation `ffe7c21c`); prior review `8284fcb5` (NEEDS CHANGES, B1–B3)
- Base: `e1be15c1`. Authority: approved spec `c61bbdd9`, Manager G4/G5, Stage B packet, #295.
- Method: `git diff df6415c9..8bd7416d` and `e1be15c1..8bd7416d` of the five UI files plus the checker; full read of the current guidance code. I ran `check_sources.py` from a throw-away shared clone at `8bd7416d` (outside the lane): **39 PASS / 0 FAIL, source-only** (matches the author's claim).

## Verdict: **PASS WITH NON-BLOCKING NOTES** — 0 blocking findings

**Not Manager acceptance. No Unity, runtime, render, input, save/load or performance verification was performed or is claimed.** All findings are static source reading.

## Re-check answers

### 1. B1 — button text no longer mutated: RESOLVED
- No assignment to any button's `.text` remains in the guidance path (`ScenarioOneUiRoot.cs` `RefreshNextActionGuidance`/`ApplyNextActionGuidance`/`ClearNextAction`). The old `"Next: " + candidate.text` and `Substring(6)` are removed.
- "Next:" is a separate reused `Label nextBadge` (`pickingMode = Ignore`, class `next-action-badge`, USS 20-unit bold) inserted as a **sibling** before the candidate (`candidate.parent.Insert(IndexOf(candidate), nextBadge)`) and removed by `ClearNextAction`. Candidate gains only classes `btn-primary` + `next-action`.
- Protected harness expectations remain satisfiable: `b.text == "Advance one year"` (MenuTutorialVerification.cs:295/306) and `BtnStarts("Advance one year")` (RackV2Combined) match the unchanged label; "Plan Racks" is still the unmutated label of `map-plan-rack`.
- Detached/stale candidate is safe: `GuidanceVisible` walks to the scope and returns false for a detached node, so the badge insert is never reached on a stale button.

### 2. B2 — expensive lookups off the tick: RESOLVED (cadence documented below)
- `LateUpdate` now calls only `ApplyNextActionGuidance()`: suspension flags, `enabledInHierarchy`, an ancestor display/visibility walk. No `TryGetRackEntranceTarget`, `GetHarvestQuote`, validators or `Query<>` (the checker asserts this on the `Apply` body).
- **Exact cadence.** `RefreshNextActionGuidance()` (full derivation incl. `TryGetRackEntranceTarget` ×(≤ racks, `break` on first markable) and, in Work Plan scope only, `GetHarvestQuote(false)`) runs only:
  1. at the end of an actual `LearningObjectivesView.Refresh` rebuild (moved after the `!force && unchanged` early return, so the 4 Hz tick no longer reaches it);
  2. at the end of an actual `StandMapView.Refresh` rebuild (after the `key == shownKey` early return);
  3. at the end of an actual `WorkPlanView.Refresh` rebuild (after the broad signature early return: cash, feedback, orders, year, reports, learning topic, rack/approval problems);
  4. once in `SetScreen` when entering Review (and `AcknowledgeAnnualReview` → `learning.Refresh(true)`).
  View entry paths (`ShowMap/ShowObjectives/ShowWorkPlan/SetScreen`) all call forced refreshes, so entry always re-derives.
- Per-frame: cheap apply only. Suspension (Reference preview, session menu, Help open, same-frame Help close) is evaluated every frame, so it is immediate.
- **Stale cue after view/plan/mark/load change:** view switches and load (`SetScreen(None)` then re-open) hide the previous root (`display:none` set each `Update`), and `GuidanceVisible` rejects hidden/detached scopes, so a stale cue cannot display; re-entry forces a fresh derive. Plan confirm, marked-entrance count, rack-layout version, editor state/status version and reopen flag are in the map key; Work Plan signature is broad. I found no concrete path that leaves a wrong cue visible while a modal is open and the world is paused.

### 3. B3 — explicit allowlist: RESOLVED
- `GuidanceButtonNames` is a named list of 19 controls; `view.Query<Button>(className:"btn-primary")` sweep is removed. All 19 names exist in source (verified by grep of `.name =`), and every possible `target` string is in the list.
- StandMapTreatmentReview primaries (Inspect in forest, Prepare/Approve prescription), "Set waypoint", "Add rack to Work Plan" and "Walk the forest [Esc]" are **not** touched.
- Restoration: each `Refresh` first `ClearNextAction()`, then restores every recorded button via `EnableInClassList("btn-primary", original)`, clears the map, and only then re-records/demotes. I traced a repeated refresh on a non-rebuilt button (annual-review-acknowledge via `AcknowledgeAnnualReview`): original is recorded after the previous cue is cleared and restored, so it is consistent. A true target that is also an original primary (e.g. `map-back-to-forest`, `workplan-advance`, `workplan-advance-return`) is restored/re-applied correctly.
- Later objectives / Legacy40 hit `else return;` **after** restore and **before** demotion, so the allowlist is not demoted there and originals return (modulo note N1).

### 4. Regression
- Cue contract: single cue reference; heavy border, separate badge, visible reason ≥ 20-unit USS, enabled+visible gate, never enables a disabled control. Held.
- Prompt safety: single prompts cue the safe Keep/Decline/Cancel; dual prompts give an instruction and no target; Confirm disabled under either question; Advance warning keeps its safe Return primary; `ClearPromptFocus` retained. Held.
- No-primary blocked states: `target = null` paths (invalid plan, quote/cash/order validation, unavailable entrance) show the reason with no candidate. Held.
- Mouse-only: no F6, no `focusable = true`, no Tab/global focus change (checker regex on the whole Assets diff). Held.
- Scope: production diff vs base is exactly the five owned UI files; no TeachingDiagram, save, geometry or test/checker weakening (the historical `check_baseline.py` is untouched).
- Checker honesty: labelled `[SOURCE ONLY]`, tally states "runtime/render/input NOT RUN", reproduces 39/0. See N5 for limits.

### 5. Residual — no new blocking defect
I looked specifically for: allowlist missing a controlled button (none — every selectable target is listed); restore racing a refresh (restore precedes re-demote within one synchronous call; no async in the path); cached visibility wrong (visibility recomputed every frame from live styles; only the *target choice* is cached). None blocks.

## Non-blocking notes (acceptable for Manager T2 consideration; none escalates to blocking)

- **N1 — Prompt restyle is unconditional.** `Reset layout`, `Cancel approved work`, `Reopen the plan` lost `btn-primary` and the Keep/decline buttons lost `btn-ghost` in their constructors, so these prompts look different in *all* contexts, including later objectives, and "restore original primary" returns the constructor state (non-primary) for those three. This is conservative for safety and matches the prompt-safety intent, but it is a visible change outside Objective 1. Manager should confirm it is within the approved spec. The checker row "baseline primaries restored" is therefore slightly overstated for these three.
- **N2 — Second primary on Review.** `Walk the forest [Esc]` stays `btn-primary` beside the cued Acknowledge, so "one primary" is not strictly true on that view (cue still distinguished by badge/border). Same for Set waypoint on the map. Consistent with "unrelated primaries untouched".
- **N3 — Cadence gaps to observe natively.** (a) Review has no rebuild hook other than entry/ack, so any later in-review state change would not re-derive until re-entry; (b) whether pointer `Dragging`/`Ghost` changes bump `StatusVersion` (and therefore the map key) was not confirmable statically — if not, the Confirm cue could persist during a drag until the next rebuild; (c) objective transition while the map stays open relies on the map key, not the learning topic. Include in AC-G/P/H gate runs.
- **N4 — Layout risk.** The badge is a sibling in header/tool/card rows (`buttons`, `toolButtons`, `card`); at narrow widths it may wrap or push controls. Combined-HUD overflow already fails on main (memory/handoff). Needs AC-V at 720p/1080p.
- **N5 — Checker limits.** Substring/source-order checks only; no negative tests, no behavioural coverage of restore ordering. Acceptable as a bounded source gate if not presented as behavioural evidence.
- **N6 — Dual-consent layout change.** The consent accept is now moved into its own `card/rack-panel` with `ConsentMessage`, and Confirm Rack Plan is also disabled under Reset. Small functional/layout edits in `StandMapView`; need Unity observation.
- **N7 — Handoff text.** Body lines (e.g. ~33, ~104) still describe the superseded 0.25 s derivation; a supersession banner at line 5 covers it. Cosmetic.
- The 7 NOT RUN matrix rows (native input, rendering, Unity import/runtime, Legacy40/Reference regression, screen reader, keyboard reach) remain open for T3/Stage D as already stated.

## Disposition
B1, B2, B3 resolved at `8bd7416d`; no new blocking findings. Recommend Manager T2 consideration with N1 explicitly decided and N3/N4/N6 carried into the native gates. This review is **not Manager acceptance** and contains **no Unity/runtime verification**; no issue/PR comments were made.
