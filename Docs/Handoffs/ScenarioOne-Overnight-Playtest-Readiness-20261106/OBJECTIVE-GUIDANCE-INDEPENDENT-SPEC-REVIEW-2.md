# Independent spec re-review — Objective next-action guidance (Stage A2 re-check)

- **Claim:** #282 (spec author route) / #294 (overnight umbrella), Stage A2 re-check. This reviewer created and edited no claim.
- **Reviewer:** Claude Sonnet 5.5, read-only, Anthropic Claude Code; effort not independently displayed (recorded as dispatched, not verified). Distinct provider/session from the author.
- **Date:** 2026-11-06.
- **New reviewed head:** `e5fe1761744d880798fd7cc685362f1e28b7c60b` (PR #265, branch `docs/overnight-objective-guidance-spec-20261104`).
- **Prior reviewed head:** `aade967d937623b51991a2fd65444bbde5cc03f9`. Prior review: `OBJECTIVE-GUIDANCE-INDEPENDENT-SPEC-REVIEW.md` (commit `9f6d4c97`), unmodified.
- **Review branch:** `docs/overnight-guidance-review-20261106-v1`.
- **Method:** `git diff aade967d e5fe1761` limited to the five artifacts (handoff, spec, plan, tasks, requirements checklist), plus grep of all five at `e5fe1761`, plus spot-checks against `origin/main`. `git diff --check` is clean. Only those five files changed.

## Verdict: **PASS WITH NON-BLOCKING NOTES**

**Blocking findings: 0.**

> **This is not Manager approval.** The spec stays DRAFT. DRAFT → APPROVED still needs an Overall Manager (#200) G4/G5 verdict, which this independent review does not replace. Stage B must not start on this review alone.

## Re-check answers

### 1. B1 — resolved
- A grep of all five artifacts at `e5fe1761` for "rack guidance", "Review rack", "read-only guidance", "safe read-only", "guidance control" and "reveal" finds no live promise of a control. The only hits are negations: spec FR-1 (`spec.md:69`, "Do not add a guidance-reveal control"), plan `:17`, tasks `:34`, checklist `:33` ("no new guidance control") and D-09.
- The old "Review rack guidance" label is gone. The corrective "read-only guidance" wording in FR-3 (`spec.md:106`) is replaced by "visible current instruction/validator reason + no eligible primary… T2 creates no additional correction or read-only guidance control".
- Decision (b) is stated the same way in FR-1 (`spec.md:69`), the FR-3 rows (0–4 not planning, rackMode active, invalid/consent-pending five, reopened/failed) and the FR-3 selection paragraph (`:106`). It is also in D-09 (`:182`), plan `:17`, T2 inventory (tasks `:34`), AC-G (`:166`) and checklist `:33`. I found no wording that disagrees.
- The permitted cue targets (Stand Map, Plan Racks *entry* but not the already-active toggle, Back to forest) all exist in source. `LearningObjectivesView.cs:73-75` and `StandMapView.cs` carry them. The existing FR-3 helper steps still point to them.
- The spec states the decision as the author's text choice following the binding default, with "Manager ratification" still required.

### 2. B2 — resolved
- FR-2 (`spec.md` safe-selection-order paragraph, ~`:77`) now requires:
  - no default or restored focus on entry or after any refresh/rebuild;
  - forward traversal `map-consent-decline` then `map-edit-reset-cancel` before any destructive control, with reverse order reversed;
  - no restoring of focus to a destructive control after a rebuild;
  - destructive controls not opted into guidance traversal;
  - a safe primary as visual emphasis only, not autofocus.
- Both control names exist in source: `map-consent-decline` at `StandMapView.cs:689` and `map-edit-reset-cancel` at `:633` on main.
- FR-6 (`:150`) adds a prompt exception to general focus preservation: clear focus rather than restore. It covers dual and single consent/reset/reopen prompts and the Advance warning.
- AC-X (`:~170`) requires a native-input check of all of this, repeated for each single prompt and the Advance warning, and says the checks apply even if Q1 is mouse-only. Keyboard reach not provided is "deferred, not PASS".
- The Advance warning's Return (safe) button is first in source (`WorkPlanView.cs:66` area), which is consistent with the safe-first rule.

### 3. Regression check — none found
- **D-071:** the repair adds no network, herringbone, diagonal or spine text. Diff review shows only B1/B2/N-fixes. The southern-headland entrance and five-parallel-rack statements are untouched.
- **#270 Highs:** R-01 and R-03 are unchanged and still NOT FIXED / gated. The R-02 row is refined, not weakened. The three rack-prompt restyles stay as T2 under D-08. The Advance warning is correctly carved out because it is already safe (`WorkPlanView.cs:66`, re-verified).
- **Cue contract:** one enabled truthful action, disabled never the cue, reason visible, non-colour "Next:", no auto operations and no new save keys all remain. The change tightens the exception: blocked means no primary.
- **Diagram rubric:** the only change is the N5 clarification. Tolerances apply to topology and relative geometry after the documented uniform transform, not survey accuracy. This removes the "schematic vs ≤1°" tension without loosening the numbers.
- **Evidence integrity:** every gate is still NOT RUN. The historical FAIL 08 is preserved verbatim and not reinterpreted. The repair handoff claims only text checks and "No independent re-review of this pass is claimed". Spec line `:162` still bars treating FAIL 08 as PASS.
- **New claims spot-checked against source:**
  - the FAIL 08 definition (`objective-primary-cta`, primary invokes `ui.CloseAll()` instead of `ui.ShowMap()`) matches `BASELINE-VERIFICATION-HANDOFF.md:133` and `LearningObjectivesView.cs:75`;
  - the "Phases B/C" pointer matches the packet's Phase B and Phase C lines;
  - the 0.8 / 1.2 px-per-unit figures are as in my prior review's source cite (`ScenarioOneUi.uss:43-45`), not independently re-measured here.

### 4. Non-blocking N1–N6 (and N7) — coherently addressed
- **N1:** the static-primary inventory (Back to forest on both screens, Advance one year, Read first annual results, Confirm/Reopen Rack Plan, Plan Racks toggle) is in FR-1, T2, plan, checklist and AC-G ("check every inventory item is conditioned/demoted"). It is integrated, not just appended.
- **N2:** the Advance-warning carve-out is in R-02, FR-2, T2 and the checklist.
- **N3:** AC-K now requires "default UITK Tab/arrow navigation suppressed or harmless". It is worded as a risk to test, not as observed behaviour.
- **N4:** ≥20 UI units at 720p is carried in FR-6, D-12, AC-V, tasks and checklist.
- **N5:** done (see 3).
- **N6:** the FAIL 08 definition is in the spec Authority section (`:21`), with pointers from plan, tasks and checklist.
- **N7:** the source-base ledger is retained. Q4/D-Q4 now say Stage B and Stage D each re-audit against their actual exact integration base, Workbench-first. No new contradiction found.

### 5. Residual risk — nothing blocks Manager APPROVED status
Genuine spec defects: none remain at blocking level.

Manager design choices, not spec defects:
- Q1: scoped F6 versus a mouse-only first slice. The spec keeps both open correctly, and the safety rules hold under either.
- D-07/D-08/D-09/D-10/D-12 ratification.
- The FR-5 numeric reference fixture, still marked NOT YET APPROVED.
- Which fresh implementation base and lane to register.

Minor wording observations, not requiring a repair pass (the Manager may fold them into approval):
- **O1:** the safe-first rule for the Advance warning names "safe return" first but does not state which of Approve or Advance anyway counts as the consequential choice. Source order is Return, Approve, Advance anyway. T1 can pin that.
- **O2:** "Back to forest" is both a demoted static primary and a conditionally eligible cue. This is consistent (conditioned), but T2 should have one explicit eligibility predicate for it.
- **O3:** under Q1 mouse-only, `UiKit.Button` stays `focusable=false`, so some B2 traversal checks become trivial or N/A. The spec already says "deferred, not PASS". A qualifier should record that as N/A and not as PASS.

## Verified vs UNKNOWN
**Verified (static, git/text only):**
- the new head and the five-file scope;
- the whitespace check;
- the full B1/B2/N diff;
- the grep for the removed control across all five artifacts;
- that the control IDs and the `WorkPlanView.cs:66` Advance warning exist on `origin/main`;
- the FAIL 08 provenance lines;
- DRAFT status and NOT RUN gates unchanged.

**UNKNOWN (no Unity or runtime used):**
- whether T2 can implement conditioning of the static primaries without side effects;
- real UITK focus, Tab and arrow behaviour;
- the Consent ∥ Reset live repro;
- actual 720p/1080p text scale and clipping;
- the diagram fixture accuracy;
- any rendered or Unity behaviour.
I did not re-read the #257 draft or any source beyond the cited lines.
