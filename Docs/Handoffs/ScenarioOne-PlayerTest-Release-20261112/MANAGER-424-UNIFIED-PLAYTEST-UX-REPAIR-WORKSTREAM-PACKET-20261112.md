# Manager #424 — single Scenario One personal-playtest UX repair workstream (all non-Objective-2 feedback)

**12 November 2026. Status: READY FOR DOCS-ONLY TRIAGE AND SPEC PREPARATION; NO SOURCE/UNITY WORK STARTED.** User's direct 22:15 instruction: treat **everything already covered except the full redesign of Objective 2** as **ONE coordinated task**. This is the **single integrated UX stream**, with one accountable primary implementer/integrator when safe. It includes the presentation findings and their linked first-rack/geometry diagnoses; do not quietly split these into an army of overlapping agents or claim one universal cosmetic fix is enough.

**Parent:** [#281 playtest UX feedback](https://github.com/jermur788/CCF/issues/281), [20 pre-redesign findings plus #21 Objective 2 in consolidated report](https://github.com/jermur788/CCF/blob/docs/scenario-one-d073-live-player-feedback-20261112-v1/Docs/Handoffs/ScenarioOne-PlayerTest-Release-20261112/MANAGER-424-D073-LIVE-PERSONAL-PLAYTEST-FEEDBACK-20261112.md), [three wording choice groups](https://github.com/jermur788/CCF/blob/docs/scenario-one-d073-live-player-feedback-20261112-v1/Docs/Handoffs/ScenarioOne-PlayerTest-Release-20261112/MANAGER-424-D073-WORDING-OPTIONS-AND-OBJECTIVE2-FOLLOWUP-20261112.md). **High-priority child of same UX task:** [#438 first-rack entrance guidance/identity triage](https://github.com/jermur788/CCF/issues/438) — **NOT** a second independent active implementation claim. **Exception to source scope:** [#439 full-rack Objective 2 redesign](https://github.com/jermur788/CCF/issues/439) belongs exclusively to parallel Workstream B.

## Assignment

- **Purpose:** reconcile/repair first-time guidance, rack-planning/entrance wayfinding, overlays, typography, interaction affordances, copy and Annual Review wording into **one cohesive Scenario One player-facing pass**.
- **Difficulty HIGH:** multiple panels, UI state/action progress, native pointer, first-rack labelling vs persisted identity, one possible geometry question; requires accepted spec and focused native visual qualification.
- **Best suited for current preparatory pass:** GPT-6 High in browser/GitHub for bounded spec/triage; **later** Sol/Codex gpt-6.1-sol High (or suitable capable local UI/Unity engineer) as one primary production writer, only after Manager approves scope, user releases Editor and live ownership is verified.
- **Docs-only preparation lane:** branch `docs/manager-424-playtest-ux-workstream-20261112-v1`; **GitHub/browser only, no local worktree**. Future source lane **NOT YET ALLOCATED**; absolutely do not invent a current worktree. For future Unity qualification use an isolated verified local worktree and the one machine-wide Unity lease.
- **Task claim:** [#440 — single integrated UX repair workstream](https://github.com/jermur788/CCF/issues/440), **READY / NOT RUNNING**. It is an umbrella/execution-presence claim; live `Docs/Project/coordination/active-tasks.md` remains sole ownership/lock authority maintained by #424.
- **Concurrently allowed:** Objective 2 research/spec Workstream B on a **different docs-only branch**; B owns its mechanics/spec reasoning, not generic UI source. Both may prepare docs concurrently; source writers MUST agree on interface/API boundary before edits to `LearningObjectivesView*`, `ScenarioOneTeachingSequence`, `ScenarioOneUiRoot`, `StandMapView` or `ScenarioOneRackBayText`.

## Required coverage — all pre-Objective-2 user findings

**First-step Objectives and Stand Map (original findings #1–7):**
- After three intro pages, first **Objectives (O)** target should be recognisably highlighted/slowly pulsing; after opening Objectives, already-highlighted **Stand Map** should not also say redundant `Next`.
- Make Objectives title the clearest typographic level; larger tabs and legible instructional text with disciplined hierarchy and dark whitespace.
- Fix the **Objective 1 teaching diagram shifting** after More detail expansion/collapse and Esc/reopen; preserve initial position and avoid forced scrolling.
- Remove Stand Map popup/details heading fragment `Find where to look` and redundant introductory warning sentence per player report; make actual key presses M, Esc, P, Tab recognisable through **one subtle keyboard-key style** (do not style clickable Back to forest as a key).
- On **first guided** Stand Map visit, give the actual **Plan Rack** target a gentle pulse in addition to current highlighting, with a clear non-animated alternative under Reduce motion; do not pulse destructive/approval controls.
- Differentiate right-hand **Plan Racks instructions** from middle overview/legend while retaining map and useful legend as primary working context.

**Rack editing, map and first marked entrance (original #8–9, #14):**
- Rotate handle gets recognisable cursor on hover/drag. A near-suggested-angle snap may be proposed only with a **verified, defensible existing reference angle and reversible drag-away**, not an invented ideal or new geometry policy.
- Investigate potential redundant **lower-left rack segment** beside existing track and intervening narrow tree strips. **This is an open design question**, not permission to trim rack geometry or modify D-064, saved access, tree counts, frozen Reference/Legacy40.
- **Urgent first-entrance reliability gate [#438]:** before first field mark, upper-left direction box and minimap target the **first lower/bottom entrance**; use a gentle first-time pulse; show accessible order after a mark. Player described a far/top entrance labelled Rack 1 only after already painting bottom. Distinguish **presentation order** from stored stable rack IDs and verify intended mark-to-rack association; do not renumber persistent identifiers or fix source before deciding whether genuine wrong tree association exists.
- Make minimap substantially more legible, about **2.5× current perceived size**, with screenshots and actual free viewport/overlap at 720p/1080p. Clarify via design measurements whether 2.5× refers to linear size or overall perceptual target; do not blindly multiply width and height by 2.5.

**Tree marking, Work Plan and review (original #10–13, #15–20):**
- **Introducing Tree Marking** and **Work Plan** first-use/help modal backdrops must substantially suppress background text without making modal/forest unreadable or breaking focus/click interception.
- Simplify **Introducing Tree Marking** copy: user requests removing stretch from `A tree carries only one of these marks` through `change an orange mark`, retaining leading one/two sentences and key safety/work-approval meaning. Replace awkward impersonal spray-paint wording; contextualise post-confirm rack intro instead of `CONFIRMED RACK PLAN`; revise `Marking cuts nothing` naturally. The 3 × 3 alternatives remain for user selection and are not yet accepted. Preserve actual rack mark vs Crop/Fell distinction and **separate Work Plan Approve and Advance**.
- Work Plan main panel deserves larger tabs, clear headings/subheadings/body scale, negative/dark whitespace and possibly larger screen footprint **without clipping at 720p or crowding approvals**.
- After `5/5` entrance marks, upper-left next-action `Tab` uses keycap styling; **5/5 on screen does not prove #438 mark identity**.
- Annual Review's backwards instruction to return to Work Plan `Tab` when already in review must be corrected/suppressed in the appropriate state (not just string deletion). Delete redundant footer instruction `Read WORK DONE, MONEY and FOREST` **but retain results and real acknowledgement gate**. Replace `One thinning does not finish continuous-cover forestry` with a user-selected natural phrase (three alternatives recorded), without incorrectly describing rack clearance as a thinning.

## Discovery / implementation gates

**Phase A — immediately eligible, browser/docs only, may run in parallel with B:** inventory every finding against exact current D-073 source and prior approved Objective guidance/UI specs; identify code owner/file set and cross-screen design system. Prepare one **SPEC REQUIRED** integrated UI/wayfinding plan if cross-screen changes, else a clearly bounded approved-spec amendment. Mark each item `copy-only`, `UI/readability`, `guidance-state`, `risk/needs-decision`, or `requires runtime reproduction`. Do not resolve missing evidence by guessing.

**Phase B — after user safely exits protected personal Editor and Manager validates lane/locks:** a distinct implementer in fresh, isolated registered source branch/worktree, **one primary UI writer**. Before starting mark issue [RUNNING] with model/provider/branch/worktree/starting HEAD; refresh `active-tasks.md` (read-only for workers); do not alter original qualified playtest candidate, #382, main, old saves or licensed assets. For shared files overlapping B, get explicit ownership/API boundary first.

**Phase C — proportionate verification:** source/offline tests for copy/state invariants, **targeted native** 720p/1080p intro → Objectives → Stand Map/Plan Racks → marking → Work Plan → Year Review. Reproduce and fix diagram drift across foldout/reopen, first-time pulse and Reduce motion, opacity/input/focus, keycaps, pointer rotation, exact before-first-mark HUD/minimap, after-first-mark rack association and correct Work Plan proposals. The first #438 wrong-association concern is **a stop gate**. Preserve known prior failing harness/CTA checks as disclosed; never weaken checks, falsely infer source/Unity PASS from documentation, repeat full Tier-3 unnecessarily, or run Windows/Player build.

**Phase D — independent relevant review + Manager verdict and user re-test:** formal APPROVED/NEEDS CHANGES/REJECTED, integrated only when safe. Do not mark playtest ACCEPTED or release for external demo without genuine player/qualification evidence.

## Scope exclusions and boundaries

- **Exclude all Objective 2 content/rule and full-rack sample/extrapolation changes** in [parallel #439](https://github.com/jermur788/CCF/issues/439). Do not rewrite Objective 2 rules as a cosmetic side effect.
- #9 **access layout design question** and #8 any *new* snap-alignment policy need accepted product rulings if geometry, treatment-area maps or save identity change.
- #438 root-cause triage may reveal a genuine event/mark mapping bug; this task carries the ownership but implementation must stop for a specific spec/decision if changing persistent IDs or D-064 semantics.
- No speculative tree-quality automation, rack felling, prescription approval, source/save schema changes, false forestry precision or weakening of required legacy/frozen-state compatibility.
- **No physical Unity work while user's current session runs.** Never seize, kill, force-release, duplicate or interrupt active Editor. No Windows build before explicit post-user-playtest approval. Ops dashboard/Drive mirror UNVERIFIED.

**Output from preparatory phase:** one inventory + scope/acceptance matrix and **one integrated implementation handoff** on this docs-only branch (exact path to be added in claim), not production source; acknowledge any user word choices still pending. On eventual production completion, output a separate GitHub Markdown source/qualification handoff and independent review, not just a chat answer.

**User action:** continue or finish the protected playtest, report exact terminal/integrity exit receipt, and choose wording variants when convenient. **To begin docs-only prep** send the dispatch message from Manager; no Editor shutdown needed for *docs-only* work. For production/Unity, user must finish and release the guarded session first.
