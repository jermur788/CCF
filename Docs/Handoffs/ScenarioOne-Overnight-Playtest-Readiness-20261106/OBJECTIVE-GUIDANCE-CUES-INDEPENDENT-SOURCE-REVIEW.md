# Independent source review — Stage B / T2 Objective 1 guidance cues (#295, overnight #294)

- **Reviewer:** Anthropic Claude Sonnet 5.5 (`claude-sonnet-5-5`), Claude Code harness in T3 Code. Effort: Medium per the Stage B packet; the harness does not expose the setting, so this is the dispatched value, not a measurement. Independent of the OpenAI `gpt-6.1-sol` implementer.
- **Date:** 2026-11-06.
- **Implementation HEAD reviewed:** `ffe7c21c19cead30d2d2bce1b6fb58fbabf04ef7`
- **Delivery HEAD (handoff only):** `df6415c9b4845f4f9567fb5cd297add5e8f797f4`
- **Base:** `e1be15c17a403bc78130a7f01a3248d537960fd8`
- **Lane:** `/media/jer/Files/CCF/Operations/CCF-Overnight-GuidanceReview-20261106`, branch `docs/overnight-guidance-review-20261106-v1`. It was clean and level with its remote branch at start.

## Verdict: **NEEDS CHANGES** — 3 blocking findings, 6 non-blocking notes

The design is sound. Q1 mouse-only, prompt safe-first ordering, the dual Consent/Reset no-primary rule, the Advance-warning Return and the scope inventory all hold in source. Three defects in the shipped source need repair before Manager T2 acceptance and before T3 reuses the branch.

**This is source review only.** No Unity, Editor, Player, native input, rendering or runtime verification was done or claimed. It is **not** Manager T2 acceptance, which only #200 can grant.

## Blocking findings

### B1. The cue rewrites `Button.text`, which breaks existing exact-label gates, and the handoff does not disclose it
- **Where:** `Assets/ForestPrototype/UI/ScenarioOneUiRoot.cs:442-446` sets `candidate.text = "Next: " + candidate.text`. The strip is at `:331-335`.
- **Evidence in unmodified harnesses at `ffe7c21c`:**
  - `Tools/Verification/RackV2MapPlanning/RackV2MapPlanningVerification.cs:316` asserts `plan.text == "Plan Racks"`.
    - It runs in a V2 world with the map open, plan mode off and `map-plan-rack` present.
    - That is the state where the selector's `map-plan-rack` target is chosen (`ScenarioOneUiRoot.cs:381`), after `StandMapView.Refresh` calls the hook at `StandMapView.cs:253` (hook after `:252`).
    - Whether it fails depends on teaching topic and plan-confirmed state in that harness, so this is likely, not certain.
  - `Tools/Verification/MenuTutorialVerification.cs:295` and `:306` match `b.text == "Advance one year"`.
  - `Tools/Verification/S1CManagementHistory/AnnualReview*Verification.cs` use `FindButton(text)` with `==`.
    - These conflict only when that button is the cue target, so this is conditional.
  - `RackV2Objective1EndToEnd.cs:109` uses `StartsWith` and is safe.
- **Why it matters:** the handoff says "No existing test was modified" and does not say that its own cue mutates the label those tests find.
  - A Stage D run would fail on a gate the T2 worker never looked at.
  - The 32-check source checker cannot see this.
- **Required fix:** pick one.
  - (a) Do not mutate `.text`. Render the "Next:" word in a separate sibling label or a `::before`-style child, so the button text stays byte-identical.
  - (b) Keep the text mutation. Then (i) disclose every affected harness with file:line in the handoff, (ii) give each a prefix-tolerant comparison, and (iii) get Manager acceptance that this is an authorised test edit, not a weakening.
  - Option (a) is preferred.

### B2. Per-tick scene-wide `FindObjectsByType` scans run from the selector, and the handoff calls them bounded
- **Where:**
  - `ScenarioOneUiRoot.cs:394` calls `manager.TryGetRackEntranceTarget` once per unmarked rack, up to 5 times per refresh.
  - `ScenarioOneManager.RackPlan.cs:371` makes each call run `LivingTreesById()`.
  - `ScenarioOneManager.cs:2291-2299` implements that as `FindObjectsByType<ForestTree>` plus a new `Dictionary`.
  - `ScenarioOneUiRoot.cs:412` calls `GetHarvestQuote(false)` for Work Plan approval. It rebuilds `LivingTreesById` plus LINQ lists before it consults its cache (`ScenarioOneManager.cs:843-846`).
- **Cadence:**
  - The hook fires on every card rebuild, and from `LateUpdate` at 4 Hz (`:314`) whenever a modal or Help state is live.
  - Objectives, Map and Work Plan all trigger it.
  - In the marking phase that is up to 5 scene-wide scans every 250 ms on an 80 ha forest.
- **Why it matters:**
  - The handoff says the cadence avoids "repeatedly scanning forest/validators". It is bounded in frequency but not in cost.
  - Nothing the player can do while the modal is open changes entrance trees, so the repeat scans add no information.
  - Hitch and GC risk is unmeasured and plausible.
- **Required fix:**
  - Run the entrance-target and quote lookups only on a view-driven refresh (`Refresh`, open, rebuild).
  - Alternatively cache the result keyed on `MarkedRackEntranceCount` plus plan version and clear it on the existing refresh events.
  - The 4 Hz tick should only re-run cheap predicates and suspension checks.
  - Add a note in the handoff that frame cost is unmeasured and is a Stage D gate.

### B3. Demotion strips every `btn-primary` under four whole roots, including controls the cue never selects
- **Where:** `ScenarioOneUiRoot.cs:336-338` runs `view.Query<Button>(className: "btn-primary").ForEach(RemoveFromClassList)` over `learning.Root`, `map.Root`, `workPlan.Root` and `review.Root`.
- **Collateral:**
  - `map.Root` contains `StandMapTreatmentReview` cards built through `StandMapView.cs:816`.
  - Their `btn-primary` controls are `Inspect in forest` (`StandMapTreatmentReview.cs:305`), `Prepare prescription for review` (`:341`) and `Approve prescription` (`:480`).
  - Nothing in the selector maps these, so they are stripped on every refresh and never restored.
  - Each rebuild re-creates them primary, then they are stripped again within 250 ms, which gives a flicker.
  - This is the D-065 MFM proposal path, outside the five-view Objective 1 scope. The packet allows demoting only "Back to forest, map toggles, Confirm/Reopen, advance and read-results".
- **Required fix:** demote by explicit name (or an explicit owned list), covering only the buttons T2 names and the Plan Racks, Reopen, Confirm and prompt buttons. Do not demote by class across whole subtrees. The checker (`check_sources.py`, "competing static primaries removed") should assert that list, not the sweep.

## Non-blocking notes

- **N1. Missing #270 disposition.** The handoff never mentions #270. Source does address R-01 (still `focusable=false`, mouse-only per Manager Q1), R-02 (destructive primary removed) and R-03 (dual → no primary).
  - Add a short R-01..R-03 mapping to the handoff.
  - Under R-03, `map-edit-reset` stays enabled during Consent (`StandMapView.cs:~625`). The dual state is therefore still reachable and is handled only by the selector's no-primary rule plus ordering.
- **N2. Dual-prompt layout.** In the dual state `map-consent-accept` is removed from its own panel and appended bare under the Reset panel (`StandMapView.cs:~646-651`).
  - It is separated from its question text.
  - Review the visual/ordering at the Unity gate. The label is self-describing and the button is non-focusable, so I do not rate it unsafe.
- **N3. Reason text may be unrelated to the cue.** In the marking branch (`ScenarioOneUiRoot.cs:391-400`) `reason = problem` is set for any rack that fails the entrance target, even when another rack is markable and `objective-back-to-forest` becomes the cue. The displayed reason can describe a different rack.
- **N4. Cue pop-in on open.** `GuidanceVisible` reads `resolvedStyle` (`:464-472`). That can lag one panel update after a screen opens, so the cue can appear up to about 250 ms late.
  - The same applies to the Advance-warning Return primary, which the handoff says is "retained".
  - Runtime-only; check it in native review.
- **N5. Per-frame work during suspension.** `LateUpdate` calls the full selector every frame while Help, the session menu or Reference Preview is active (`:314-316`). That includes an array allocation and four `Query<Button>` sweeps each frame.
  - The suspension flag could short-circuit the demotion sweep after the first pass.
  - Fold this into B2's fix.
- **N6. Non-O1 contexts lose all primaries.** The selector returns early for later objectives and Legacy40 (`:439`).
  - Every primary under the four roots has already been demoted, including Annual Review acknowledgement and Advance one year there.
  - The handoff discloses this.
  - It is visually inconsistent with the protected Legacy40 path, so Manager should acknowledge it explicitly.
- **N7. Historical checker.** `check_baseline.py` now aborts with an anchor error (exit 2) instead of completing with one FAIL. That blocks its other checks (01-07, 09) from running.
  - The handoff is honest about this and did not weaken the checker.
  - It needs the independently authorised successor the handoff names, before Stage D relies on it.

## Review questions

1. **Approved T2 behaviour — holds, with B1 and B3 caveats.**
   - There is one `nextAction` field, assigned once (`ScenarioOneUiRoot.cs:54`, `:442`).
   - Eligibility requires `enabledInHierarchy` and ancestor visibility (`:441`, `:464`). Disabled candidates never get a cue (`:448-455`).
   - The order is Objectives → Map → Plan Racks → Confirm → entrance marking → Work Plan → Advance/Review (`:364-441`).
   - Blocked, manual-drag and ghost states set only `reason` (`:381-384`). Dual Consent/Reset sets no target (`:358-359`).
   - The selector calls no world actions and invokes no callbacks. No new control, no progression state and no auto-execution were found.
2. **Q1 mouse-only — holds.**
   - No `F6`, `Shift+F6` or `focusable = true` was added.
   - `HandleKeys` is untouched. `UiKit.Button` keeps `focusable = false` (`UiKit.cs:62`).
   - `Blur()` is called only on a focused descendant (`:476-479`). It does not grant focus.
3. **Prompt safety — holds.**
   - Consent and Reset target the safe `…-decline` / `…-cancel` (`:359-361`).
   - The safe choice is added first (`StandMapView.cs:~635-641`, `:698-706`, `:732-738`).
   - Destructive buttons lose primary. Confirm is disabled under either question (`:662`).
   - Rebuild calls `ClearPromptFocus` (`StandMapView.cs:251`, `WorkPlanView.cs:184`).
   - The Advance Return keeps `btn-primary` at construction (`WorkPlanView.cs:68`) and is re-selected as the cue (`:363`). It is briefly demoted during Help (N4).
   - No path was found where a destructive control becomes primary. See N2 for the dual layout.
4. **Static-primary demotion — fails on scope.** Back to forest, map toggles, Confirm, Reopen, Advance and Read-results are demoted and restored by the cue as intended. B3 shows over-reach and N6 the non-O1 behaviour.
5. **State derivation and cadence — correct facts, heavy cost.**
   - The selector reads `RackPlanProblem`, `RackApprovalProblem`, `CanConfirmRackPlan`, `CanAdvanceYearNow`, `NeedsAnnualReview`, `rackMode`, the editor flags and the Help, session and reference suspension. Null guards are adequate: construction order puts `review` before `map` (`ScenarioOneUiRoot.cs:119-120`).
   - A stale cue cannot survive a load or view change. Each call strips the previous cue first (`:329-339`) and rederives.
   - Costs are in B2 and N5.
6. **Scope discipline — holds, with B3.**
   - `git diff --stat e1be15c1..df6415c9` shows exactly the five owned production files, the new checker and the handoff. No save, preferences, TeachingDiagram, geometry, marking, world, scene, prefab or asset changes.
   - The checker only asserts, and I ran it at `ffe7c21c` with 32 of 32 passing. It changes no existing gate.
7. **Evidence honesty — acceptable, with gaps.**
   - The 26 PASS rows are labelled "static/source only". Spot-checked rows 6, 8, 9, 11, 14, 17-21, 26 against source and they hold.
   - NOT RUN rows are not claimed.
   - Gaps: the B1 test-label impact is not mentioned, #270 has no disposition (N1), and "bounded" is overstated (B2).
8. **Regression risk beyond the questions above.**
   - `StandMapView.Refresh` calls the hook after a rebuild. `AnnualReview` is only demoted through the root and keeps its callbacks.
   - Help close is covered by the same-frame guard.
   - I found no routing change that Unity-free reading would break apart from B1 and B3.

## Verified vs not verified

**Verified (source and static only):**
- The diff `e1be15c1..df6415c9` and per-file `ffe7c21c` source.
- Adjacent paths: `ScenarioOneUiRoot` constructor, screen routing and `LateUpdate`; `StandMapView` Refresh and prompt builders; `WorkPlanView`; `LearningObjectivesView`; `AnnualReviewView`; `UiKit`; `StandMapRackEditor` flags; `ScenarioOneManager.TryGetRackEntranceTarget`, `LivingTreesById` and `GetHarvestQuote`.
- Existing Tools/Verification label dependencies, by grep.
- `check_sources.py` run at `ffe7c21c`: 32 of 32 passed.
- The Manager approval, the Stage B packet and the spec addendum.

**Not verified:**
- Anything that compiles, instantiates UITK, renders or accepts input.
- Whether the B1 harnesses actually fail, which needs a Unity run.
- Frame cost for B2.
- Hit-testing, layout and prominence.
- The typecheck output and the Help offline 97-check run claimed in the handoff. I did not rerun them.
- Live coordination state, the issue text of #295 and the full text of the spec beyond its addendum. `gh issue view 295` returned nothing usable in this lane; scope came from the packet and the Manager approval.

## Required next step

Repair B1-B3 on `feature/scenario-one-next-action-guidance-v1`, add the #270 mapping, then re-review. T3 (#296) should not start until that re-review and Manager acceptance.
