# Manager → Sol: Implement integrated Scenario One UI presentation (binding packet)

**Status: DISPATCH READY, NOT RUNNING.** User directed one coherent design implementation, with four authoritative SVG layout images and later Fable review. **Claim [#372](https://github.com/jermur788/CCF/issues/372) — READY / NOT RUNNING.** Do not mark it running before real local preflight. Source implementation is authorised by the approved spec, **not Unity access or release approval**.

## Why / difficulty / model

Purpose: remove clutter and provide consistent decisions across (1) walking/tree inspection HUD, (2) compact Year/Cash header and contextual bottom keycaps, (3) Objectives/Stand Map/Work Plan/Annual Review menu navigation, (4) Work Plan cost/approval presentation and (5) end-of-year Annual Review summary. **HIGH** — crosses stateful UI classes, source-only model truth, locked keyboard input, annual historical data and safe commitment gates. **Best suited: OpenAI Sol / Codex High using T3 local code lane**. Single primary UI writer; do not delegate overlapping UI files.

## Supplemental poster style references

**Additional user-supplied three-poster set:** https://github.com/jermur788/CCF/blob/spec/scenario-one-ui-presentation-20261110-v1/specs/scenario-one-ui-presentation-v1/visuals/POSTER-SUPPLEMENT-AND-ERRATA.md

These are uploaded 2× PNG **visual style references**, not game truth or current Unity renders. The user reports all three local files in `/media/jer/Files/CCF/Verification/CCF-362-FableUXReview-20261110/spec-posters/`. Verify existence and hashes exactly as recorded in the addendum before using them. If absent, follow the four Git-backed SVGs and report poster comparison NOT INSPECTED; do not invent image inspection. The Manager chat also has a downloadable 3-PNG ZIP which the user can pass to Sol.

Adopt styling: unified woodland green/cream, consistent readable typography, aligned financial amounts, clear active states, concise selected-tree card, discrete Year/Cash header, contextual bottom keycaps, descriptive Annual Review cards. **Do not copy content errors** listed in the addendum: `[F]` Fell (actual `[X]`), copied 54%/competitor shares or net/canopy/synthetic outcomes, premature “Continue to Year 6” Review control, nor made-up operations. Fable will audit actual source behavior and real screenshots against these precise constraints.

## Exact inputs

- **Binding approved four-file spec:** https://github.com/jermur788/CCF/blob/spec/scenario-one-ui-presentation-20261110-v1/specs/scenario-one-ui-presentation-v1/spec.md
- **Plan:** https://github.com/jermur788/CCF/blob/spec/scenario-one-ui-presentation-20261110-v1/specs/scenario-one-ui-presentation-v1/plan.md
- **Tasks:** https://github.com/jermur788/CCF/blob/spec/scenario-one-ui-presentation-20261110-v1/specs/scenario-one-ui-presentation-v1/tasks.md
- **Acceptance:** https://github.com/jermur788/CCF/blob/spec/scenario-one-ui-presentation-20261110-v1/specs/scenario-one-ui-presentation-v1/checklists/requirements.md
- **Visual A — HUD/inspection:** https://github.com/jermur788/CCF/blob/spec/scenario-one-ui-presentation-20261110-v1/specs/scenario-one-ui-presentation-v1/visuals/01-hud-inspection.svg
- **Visual B — Work Plan:** https://github.com/jermur788/CCF/blob/spec/scenario-one-ui-presentation-20261110-v1/specs/scenario-one-ui-presentation-v1/visuals/02-work-plan.svg
- **Visual C — Annual Review:** https://github.com/jermur788/CCF/blob/spec/scenario-one-ui-presentation-20261110-v1/specs/scenario-one-ui-presentation-v1/visuals/03-annual-review.svg
- **Visual D — navigation/guards:** https://github.com/jermur788/CCF/blob/spec/scenario-one-ui-presentation-20261110-v1/specs/scenario-one-ui-presentation-v1/visuals/04-navigation-and-guards.svg
- **Earlier Fable design-only proposal:** https://github.com/jermur788/CCF/blob/docs/scenario-one-fable-ui-layout-proposal-20261110-v1/Docs/Handoffs/ScenarioOne-Playtest-Feedback-20261110/FABLE-52-HUD-MENU-REVIEW-LAYOUT-PROPOSAL.md
- **User's latest precise choices (controlling):** Annual Review headings **Work done, Money, Forest changes**, never questions; HUD top-right **Year and Cash only**; **no background browsing indicator, no invented biodiversity figure**; contextual useful bottom button/key prompts; restore full information through `[V]` and detail pages. All numbers in mockups are illustrative.
- **Current source baseline:** `4db486db7a132bd01725442ef33e00f702c4dff9`, `feature/scenario-one-player-feedback-overnight-20261110-v1` lineage. Our source branch pre-created as `feature/scenario-one-ui-presentation-v1-sol` from that candidate, **not** from docs-only spec branch.
- **Exact planned local lane/worktree:** `/media/jer/Files/CCF/Operations/CCF-S1-UI-Presentation-V1-Sol` (must be actually created/checkpointed by Sol, and safe on available disk; if location cannot be admitted, STOP/ask Manager instead of silently using the user's warm U3/Workbench).

## Mandatory preflight

1. Refresh live `Docs/Project/coordination/active-tasks.md` at task start and before claiming a missing lock; inspect #361 user Editor status, #372 and any active UI/Unity writer. Verify actual local path, branch, HEAD, cleanliness including generated/untracked/ignored valued files, source ancestry and disk. Nothing in this packet overrides an existing owner.
2. If no overlap and lane safe, register single-writer UI ownership with Manager/live coordination as required, and update #372 to **[RUNNING]** with actual model/provider, branch, worktree, starting full Git commit ID. If not admitted mark **[BLOCKED]** with the reason and do not mutate production.
3. Inspect actual source, not just Fable's old captures. `WalkingHudView.cs`, `TreeInspectionView.cs`, `WorkPlanView.cs`, `AnnualReviewView.cs`, `ScenarioOneUiRoot.cs`, `LearningObjectivesView.cs`, `ScenarioOneUi.uss`, current plan-effects and in-world marker owner. Look up existing state guards and already-implemented source-toggle, small-job quote and latest-unread review; don't undo them.
4. **No Unity until** separate user machine ownership/release status, real global `unity-lock` lease, actual clean/exact candidate U3 admission, 35GiB-class free disk monitoring, 27 ignored licensed assets / frozen reference + meta / original dirty Workbench preservation, **verified isolated F5/F9/PlayerPrefs and user save protection**. No current status assertion can replace real preflight. If blocked do source work and accurately report Unity NOT RUN.

## Implementation slices — one writer, bounded

1. **HUD/main layout:** objective cue, compact Year + Cash upper right, contextual keycap bar at bottom. Only valid keys; `[X]` and `[C]` absent under Rack-first. Existing `[V]` full facts keyboard accessible in fixed/paged cursor-locked panel. Rendered vs pointer semantics honest.
2. **Inspection:** merge tree and ranked neighbours/default forecast into one responsive compact card, accurate local-competition shares vs potential diameter-growth reduction, top three rows and "others" computed. Distinguish tree/mark/MFM suggestion. Plan effects are draft; wind disabled not safe. Optional on-tree marker shares only with actual legibility go/no-go; card fallback mandatory.
3. **Menu shell:** consistent screen names, navigation, back-to-forest and safe NEXT action; adapt current view roots, not a new global state machine.
4. **Work Plan:** actual current quote, minimum, owner time and group state; clear review → approve → advance hierarchy without changing existing approval/advance warning. **Never pulse Approve**.
5. **Annual Review:** three descriptive sections Work done / Money / Forest changes at top, recorded selected-year facts only; net income-cost result only where supported by YearMoneySummary; no fabricated 0s, biodiversity metric or current forest facts in earlier history. Source chips hidden by default, detailed records retained, historical read-only, only newest unread acknowledges, **Back to forest remains enabled**.

Minimise source boundary to `Assets/ForestPrototype/UI/` plus relevant USS and focused verification; typed presentation adapter outside requires Manager sign-off before editing non-UI gameplay files. Preserve world/rack/growth/save/marking/economy and old-world compatibility. Verify one primary source candidate; do not compare against hardcoded screenshot figures.

## Verification budget and output

Tier 1 typed/source unit tests and targeted meaningful negative guards. One proportionate Tier-2 run only after safe exclusive Unity admission: genuine nonblack 1280×720/1920×1080 colour/grayscale for HUD, Work Plan, Annual Review; keyboard, rack-first, Crops/Fell/MFM, current/old/year-history/ACK, low-value quote, [V] details. Explicit PASS/FAIL/NOT RUN per case; no broad Tier-3 forest/release/calibration rerun. Keep original tests intact.

**Exact mandatory GitHub Markdown result:**
`Docs/Handoffs/ScenarioOne-UI-Presentation-20261110/SOL-UI-PRESENTATION-V1-IMPLEMENTATION-AND-QUALIFICATION-RESULT.md`

Include source branch/commit, diff inventory, model/provider, actual worktree, live claim/lock, test commands and logs, real screenshot paths, meaningful comparison to all four SVGs and known deviations, compatibility/side effects, input/approval/review semantics and next review link. At handoff mark **#372 [HANDED-OFF] and close**, release any owned UI/Unity lock, preserve unrelated dirty files, then return to Manager #200. **Fable #373 is WAITING until this handoff is complete**; do not spawn Fable edits on the same source lane.

## Copy/paste dispatch line

> Sol (Codex High / T3 local code): Execute the Manager binding UI implementation packet at https://github.com/jermur788/CCF/blob/spec/scenario-one-ui-presentation-20261110-v1/Docs/Handoffs/ScenarioOne-UI-Presentation-20261110/MANAGER-SOL-UI-PRESENTATION-V1-IMPLEMENTATION-PACKET.md . Claim #372, branch `feature/scenario-one-ui-presentation-v1-sol`, proposed isolated worktree `/media/jer/Files/CCF/Operations/CCF-S1-UI-Presentation-V1-Sol`. Preflight live owners and protected state; implement all specified UI screens using four SVGs, run only guarded focused checks/one safe Tier2, publish the exact Sol result handoff, close claim, return Manager. Never use Unity/Windows without the required gates.

**User action:** paste the dispatch line to Sol. No new design decision needed to start source work. The user must separately release any personally occupied Unity Editor if guarded qualification would need it. Fable review will be dispatched after Sol's result.
