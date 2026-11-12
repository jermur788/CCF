# Unified UX Repair V1 — proposed implementation and qualification plan

**DRAFT; no source or Unity authorisation. Owner #440 docs preparation.**

## Sequence

**0. Gate and baseline.** Manager reviews this DRAFT and user wording choices. The protected user personal Editor playtest remains in progress, so no physical Unity lock, local worktree, source writer or build may start. After the user provides a safe terminal exit/lease receipt, the Manager checks active-tasks.md live, exact branch/source ancestry, #441 overlap, Ops board, one machine-wide Unity lease and isolated disposable test profiles. A future **single primary UI writer/integrator** gets its own newly allocated branch/worktree and execution claim; do not invent these now.

**1. Safety-first diagnostic (no production edit).** On an authorised **copy** of exact protected D-073 candidate: capture before first paint, after painting the lower entrance, after next paint, five-of-five, and Work Plan pending rack removals. Capture world entrance tree IDs, rack saved stable IDs, actual ordered rack geometry/physical north-south positions, Spray target tree, HUD rack label, minimap marker, work-order rackId/treeId, and bounds. Categorise #438: (a) absent/late presentation only, (b) visit-order labels only, (c) wrong association, (d) uncertain. (c)/(d) is **STOP**, do not auto-fix/approve. Separately measure #9 existing-path/lower-left gap geometry without changing it. Read-only Unity witness may run before code only once user releases protected Editor.

**2. Source-level design:** exactly one shared UI change set with the existing USS/tokens and explicit screen-state table. Confirm #441's single-file/shared-UI contracts before touching LearningObjectivesView, ScenarioOneTeachingSequence, ScenarioOneUiRoot, StandMapView or ScenarioOneRackBayText. #441 owns Objective 2 rule text and calculation. #440 may adjust only generic guidance/review prompts with an agreed API boundary. No new save schema or unrelated menu framework.

**3. Focused UX edits in one coherent sequence.** Implement Objective first-action + Plan Rack safe emphasis (Reduce motion static parity); redundant badge suppression; title/tab/body hierarchy; stable diagram+scroll layout; map instruction/legend contrast; genuine keyboard-keycap component/style; rotate cursor (snap only if explicitly approved); resized minimap plus coherent prepaint/next rack HUD/map waypoint; local modal backdrop/focus; Work Plan hierarchy; copy and Annual Review state-dependent instruction. Prefer short, reviewable commits owned by the same writer, not isolated competing subsystem branches.

**4. Proportionate offline checks:** pure/state tests for stage transition, pulse eligibility, reduced-motion availability, modal state, absent redundant "Next", correct key/button semantics, exact copy, no false thinning wording, source case guards. First-rack mapping/ordering pure assertions compare physical south-to-north entrance order to stable IDs without assuming they are equal. Do not weaken existing CTA or reference/compatibility tests; disclose pre-existing harness failures separately.

**5. Targeted native Tier-2 UX witness:** 720p and 1080p, mouse and keyboard. Walk third intro page → initial O → Objectives → Stand Map → Plan Racks map/legend/rotate → confirmed rack → Tree Marking help → lower-first spray and subsequent entrance → 5/5 → Work Plan review/Approve → Advance → Annual Review read/acknowledge → Objectives. Exercise full and Reduce motion separately, More detail expand/collapse/reopen, resize no text clipping, clicks blocked behind modal, keycaps distinguished, map/HUD cross-target, wrong order/identity gate, click/focus containment, actual work order comparison. Take before/after captures of each high-risk UI, content height, rendered bounding boxes, exact target counters. Reuse existing verified regression suite judiciously; **do not run Windows/Player build**.

**6. Independent relevant review, Manager verdict and user retest.** Reviewer checks a bounded source diff and raw native captures/logs, not claim-only summaries, with APPROVED/NEEDS CHANGES/REJECTED. Manager separately determines safe integration. Human re-test required for subjective hierarchy and acceptable first-entrance navigation; no external-demo acceptance follows automatically.

## File-set routing (inspection only; later writer needs approved lock)

| Area | Candidate files | Scope and overlap |
|---|---|---|
| HUD first target, next-action emphasis | ScenarioOneUiRoot.cs, WalkingHudView.cs, ActiveObjectiveHudWidget.cs, ScenarioOneTeachingSequence.cs | #1, #2, #6, #14, #15, #18; **share UI-contract boundary with #441** |
| Objective layout and diagram | LearningObjectivesView.cs, LearningObjectivesView.Sequence.cs, TeachingDiagram.cs, ScenarioOneUi.uss | #3, #4; Sequence is read-only unless separately agreed |
| Map instructions and rack editing | StandMapView.cs, Map/StandMapRackEditor.cs, Map/StandMapSource.cs, HudMinimapView.cs | #5–9, #14; map source should only reflect confirmed authoritative targets |
| Modal/help/copy | MenuHelpView.cs, MenuHelpView.Text.cs, UiKit.cs, ScenarioOneUi.uss | #5, #10–13, #16, #19–20 |
| Work Plan/review | WorkPlanView.cs, AnnualReviewView.cs, ScenarioOneRackBayText.cs, ScenarioOneUiRoot.cs | #15–20; exact ReviewFirst state matters; #441 owns Objective 2 teaching rules |
| Rack identity source (diagnose only) | ForestTreeMarkingManager.RackEntrances.cs, ScenarioOne/Rack editing/planning and work-order code | #14 STOP; no mutation/renumber/schema without decision |

## Qualifier evidence expected

- One concise evidence manifest: candidate revision, worker and lane, current branch/HEAD/clean state, Unity lease, copy-only profile, safeguards for old user saves, test command/results, screenshots by resolution and stage, measured viewport clearance, true state transitions, unresolved issues, raw evidence references.
- Reference integrity: Legacy40, pre-V2 and frozen Reference Future v1 behaviour at proportionate source/offline regression level, plus any targeted Unity witness necessary for touched surfaces; preserve existing anchors, do not refresh them.
- Statuses separated: source IMPLEMENTED does not mean Unity VERIFIED, integrated does not mean personally ACCEPTED. Claims close only with durable GitHub Markdown handoff.
