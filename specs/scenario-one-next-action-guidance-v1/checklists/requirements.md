# Requirements and evidence checklist

**MANAGER G4/G5 APPROVAL ADDENDUM — 2026-11-06.** [Formal exact-head approval](https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Overnight-Playtest-Readiness-20261106/MANAGER-OBJECTIVE-GUIDANCE-SPEC-APPROVAL-20261106.md) after the independent review at `e5fe1761744d880798fd7cc685362f1e28b7c60b` returned **0 blocking findings**. Binding Q1: **mouse/pointer-only NEW Objective 1 guidance controls** for the first focused playtest. Do not implement F6/Shift+F6, globally change `UiKit.Button.focusable` or hijack existing Tab/O/M/Esc/F1; keyboard reach is DEFERRED, not falsely PASS. **No default/restored destructive prompt focus; safe-first if any traversal remains reachable; native safety checks mandatory.** D-08 safe-prompt primary styling, D-09 blocked/dual no-primary fallback, D-10 screen-reader deferral, D-12 measured new-text floor and Q3 Objective 1-only/no-new-HUD are APPROVED. Q2: static five-southern-headland-parallel-rack teaching diagram, **approved reference recipe** using actual `ScenarioOneSuggestedRackLayout.Get`, `ScenarioOneHeadlandGeometry` and `StandMapSymbols`; exact numeric fixture must be derived from genuine deterministic Enlarge80 Rack V2 tree identities before T3 art, no invented data. Q4: Stage B uses fresh current main/integration source in a separate owned lane, NOT this diverged docs branch. Approved Stage B requires separate T2 then T3 claims, handoffs/reviews and later exact-head Unity qualification. All previous DRAFT/"proposed" paragraphs below are retained as **historical design provenance** where they conflict with this overriding Manager decision. No production or Unity evidence was created by this status change.

---


- **Spec ID:** SPEC-SCENARIO-ONE-NEXT-ACTION-GUIDANCE-V1
- **Status:** **APPROVED DESIGN / UNCHECKED IMPLEMENTATION ACCEPTANCE**, per independent 0-blocker review and Manager G4/G5 decision.
- **Source base:** `1e3e159b768dc4b0bfe6e410a7728a412d34e5db`; context `81867c8940d7d07c952e8e49ca4a0cb93498e790`.
- **Implementation under test:** none. Static document/source inspection only. No Unity/tests/runtime/build/captures produced.

Unchecked future implementation boxes mean NOT VERIFIED / NOT RUN, never waived. **Manager design decisions and independent spec review are approved only in the separate release record; all production/Unity acceptance remains unchecked.** Old FIXED claims withdrawn.

## Release decisions and ownership

- [x] Manager disposed Q1–Q4 for scoped first slice in G4/G5 review; exact source-derived numeric geometry fixture still pending before T3.
- [x] D-07 approved **mouse-only new guidance controls**. No F6/Shift+F6 or global UiKit focus reversal; keyboard reach DEFERRED, no keyboard safety test waived.
- [x] D-08 approved safe prompt restyle, T2 assigned under own forthcoming claim; actual behaviour NOT VERIFIED.
- [x] D-09 no-primary Consent ∥ Reset and B1(b)/B2 safe focus approved; runtime NOT VERIFIED, lifecycle fix not authorised.
- [x] D-10 screen-reader deferral explicit; NOT PROVIDED is not PASS.
- [ ] D-11 approved geometry/source/capture reference and numerical/binary diagram rubric.
- [x] D-12 new-text floor and D-13 no-new-HUD first-slice design approved; actual pixel measurements NOT RUN.
- [x] Independent opposite-provider spec re-review PASS (0 blockers), Manager G4/G5 **APPROVED**; production work still requires separate owned task claims and source/Unity gates.
- [ ] T1, T2, T3, T4 if released, and T5 each have own claim, owner, registered lane/branch, exact HEAD and owned files; shared UI/style files single-writer.

## Behaviour / source and native-input gates (future)

- [ ] FR-1 / AC-G: current helper/counters/teaching facts reused; no second progression source, no new save/settings keys.
- [ ] FR-2 / AC-P: guidance scope distinct from input ownership; visible enabled safe target only, occluded/background primary suppressed.
- [ ] R-01 / AC-K: approved keyboard reach/focus/traversal/Enter/Space actually implemented and native-input tested; Tab stays Work Plan; gameplay keys never activate buttons.
- [ ] R-02 / AC-P: safe consent/reset/reopen/advance-warning emphasis; no destructive default cue/focus/activation.
- [ ] R-03 / AC-X: real Consent ∥ Reset cross unconfirmed/invalid-five/valid-five/confirmed backgrounds → **no eligible primary**, visible reason, safe secondary recovery; source and Unity outcomes recorded separately. No default/restored focus; forward traversal map-consent-decline then map-edit-reset-cancel before any reachable destructive control; no destructive focus restored after refresh/rebuild. Repeat no-focus/safe-first native-input checks for single consent/reset/reopen prompts and Advance warning, even with Q1 mouse-only (keyboard reach remains deferred, not PASS).
- [ ] AC-H: first-use Help/onboarding owns first cue; F1 over each rack/advance prompt; Esc/Continue/context/navigation close → fresh cue, no fall-through.
- [ ] AC-P/X: eligible map editing pointer-down clears prompts as current source does; no false “until answered” lifecycle or stale target.
- [ ] FR-3 / AC-G: Objectives → Stand Map → Plan Racks → explicit drawing → Confirm → forest entrances → Work Plan approval → annual review, all truthful and deliberate.
- [ ] RackPlanningAvailable, CanConfirmRackPlan, RackPlanProblem, RackApprovalProblem, CanEditWorkPlan, CanAdvanceYearNow and NeedsAnnualReview actually gate cue/availability; disabled reason visible.
- [ ] B1(b) / FR-1, FR-3, D-09, AC-G: blocked/manual-drawing state shows existing reason/instruction + no eligible primary; no new guidance control. Only truthful existing Stand Map / Plan Racks entry / Back to forest navigation may be cued; Undo/Delete secondary.
- [ ] T2 conditions/demotes existing static btn-primary on Back to forest (Objectives/Map), Advance one year, Read first annual results, Confirm/Reopen Rack Plan and Plan Racks toggle; only eligible target primary, none in dual prompt.
- [ ] R-02: preserve Advance warning's already-safe return primary; it is not the same restyle as the three rack prompts. AC-K: default UITK Tab/arrow focus navigation suppressed or harmless; Q1 F6 vs mouse-only remains Manager choice.
- [ ] WorkPending=false not mistaken for approved/resolved; rack-owned work and failures determine state; no false “cleared route”.
- [ ] Forest HUD consistent with shared instruction; no new highlighted widget claimed unless separately approved.
- [ ] AC-S: load reconstructs cue from persisted facts plus transient view/session state; rackMode not assumed saved; marks, harvested locks, Step IDs, Legacy40 and frozen Reference Future unchanged.
- [ ] No automatic plan/confirm/mark/approve/advance, duplicated world actions or protected-test weakening; future regression checks match actual changed scope.
- [ ] FR-4 / AC-L if released: later lessons mapped to actual validators/handlers, every unknown REVIEW NEEDED; no whole-product completion from O1 alone.

## Diagram / visual gates (future)

- [ ] D-071 exact family: existing southern entrance/headland, five parallel racks, five entrance squares, retained bands; D-067–D-069 not live alternatives.
- [ ] Visual Option A distinguished from unrelated historical two-family MFM Option A; no northern/diagonal/herringbone/spine-extension network; D-070 excluded.
- [ ] FR-5 reference endpoints/transform approved; direction within 1°, displacement ≤1% stand width, width ≤5% reference deviation, or Manager-approved stricter replacement documented before implementation. Tolerances preserve topology/relative geometry after documented uniform transform, not survey accuracy.
- [ ] Retained crowns in 4 inter-rack and 2 outer bands; no invented real row count/census.
- [ ] Illustrative/not-live/not-cleared labels, scale/orientation caveat, site-specific 16–20 m explanation and planned/confirmed/executed/marked key visible.
- [ ] Current StandMapSymbols meaning preserved; non-colour pattern/shape/text recognition and Next/focus distinction at both resolutions.
- [ ] FR-5 every rubric row passes separately at 720p and 1080p, colour and grayscale; independent 4/4 recognition per reviewer/mode/size, no coaching or judgement-only waiver.
- [ ] FR-6 / AC-V new-text target ≥20 UI units at pinned 720p scale (1 unit = 0.8 px); actual panel-scale floor ≥16 physical pixels AND equivalent-role non-regression; no clipping/overlap, usable scroll/reflow; source USS values not substituted for measurements.

## Evidence and handoff gates

- [ ] Exact-head compiler/bounded offline and Tier-2 native-input/rendered results for all applicable AC rows; no existing FAIL reinterpreted as PASS.
- [ ] FAIL 08 definition/provenance read from spec Authority section (objective-primary-cta: CloseAll instead of ShowMap; original qualification packet/baseline); historical FAIL retained.
- [ ] Pinned design source-base note retained; Stage B and Stage D re-audit against actual exact integration bases, respecting amendment's Workbench-first integration order.
- [ ] T5 exclusive lane/lease and disposable saves; no user playtest state touched; durable evidence on Files CCF tree.
- [ ] Independent cross-provider review verdict and unresolved findings retained; Manager/user acceptance separate.
- [ ] #281 remains open until player-facing implementation/qualification/acceptance; docs handoff does not close expectation gap.

Current design result: **APPROVED SCOPED OBJECTIVE 1 SPEC** by Manager G4/G5. **NOT IMPLEMENTED / NOT UNITY-VERIFIED / NOT INTEGRATED**; checklist presence does not satisfy any unchecked gate.
