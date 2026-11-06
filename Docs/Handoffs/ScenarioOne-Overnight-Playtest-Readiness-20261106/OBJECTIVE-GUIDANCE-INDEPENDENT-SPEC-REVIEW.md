# Independent spec review — Objective next-action guidance (Stage A2)

- **Claim:** #282 (spec author route) / #294 (overnight umbrella), Stage A2. A review claim was to be created by #200; this reviewer did not create or edit any claim.
- **Reviewer:** Claude Sonnet 5.5, read-only, provider/effort as observed in this session (Anthropic Claude Code; effort not independently displayed — recorded as Medium per dispatch, not verified). Distinct provider and session from the author (GPT-6.1 Sol per amendment default).
- **Date:** 2026-11-06.
- **Reviewed head:** `aade967d937623b51991a2fd65444bbde5cc03f9` (PR #265 branch `docs/overnight-objective-guidance-spec-20261104`, parent `a173192f`).
- **Review lane:** branch `docs/overnight-guidance-review-20261106-v1`, worktree `/media/jer/Files/CCF/Operations/CCF-Overnight-GuidanceReview-20261106`.
- **Base:** main `1dd9650dca066cc87221a71fac1ab56803f680a2`.
- **Line cites:** `spec:N` etc. are line numbers in the files as stored at `aade967d`. `src:` cites are `origin/main` (1dd9650d).

## Verdict: **APPROVE WITH CHANGES**

The revision is a genuine repair, not a relabelling. D-071 is consistent everywhere I looked, and all three #270 High findings are disposed in operative text with owner, decision ID and gate. Prior "FIXED" claims are withdrawn. Two gaps, listed below, make the spec promise or omit behaviour the current UI cannot supply. They are small, text-only fixes. Neither needs a product decision from the user.

**Blocking: 2. Non-blocking: 7.**

> **This review is not Manager approval.** The spec is, and must stay, DRAFT. DRAFT → APPROVED requires an Overall Manager (#200) G4/G5 verdict, which an independent reviewer PASS does not substitute for. Stage B must not start on this document alone.

## Blocking findings (fix before Manager approval)

### B1 — Medium-High — the "safe read-only guidance control" is undefined and does not exist in source
- **Where:** `spec:67` (FR-1 blocked fallback, e.g. "Review rack guidance"), and the FR-3 rows at `spec:88`, `spec:89`, `spec:90`, `spec:96` that rely on it ("safe read-only guidance", "cue read-only rack guidance while manual drawing is required").
- **Problem:** FR-1 says that when progression is blocked the cue falls to "a safe, enabled guidance/navigation control (for example 'Review rack guidance')" that "may reveal the current instruction/reason". `git grep -i "rack guidance"` over `Assets/ForestPrototype/*.cs` on main returns nothing. No such control exists. It is not named in any T2 file list, USS scope, task deliverable or AC row. The blocked states (invalid five, consent pending, planning mode with fewer than five racks, reopened/failed) are exactly the states a player will sit in for a long time. The spec therefore depends on a UI element it never specifies, while presenting the fallback as a normal path. It also duplicates text already visible in Objectives (`src:LearningObjectivesView.cs:73-75`, `CurrentInstruction`).
- **Required fix (choose one, record it as a decision):** either (a) define the control (label, owner screen(s), what it opens, that it is read-only, T2 ownership, and an AC-G row for it); or (b) strike it and make "visible reason + **no eligible primary**" the blocked-state behaviour, retaining a deterministic safe *navigation* target only where an existing control already is one (Stand Map, Plan Racks, Back to forest). Option (b) matches the amendment's "no eligible primary fallback" default and adds no invented UI.

### B2 — Medium — Consent ∥ Reset: the amendment's "explicitly safe selection order" is not specified
- **Where:** `spec:77` (FR-2 dual state), `spec:140` (FR-6 focus), `spec:156` (AC-X).
- **Problem:** the amendment (§Manager defaults, modal/action safety) requires simultaneous Consent/Reset to have "an explicitly safe selection order **and** a no eligible primary fallback". The spec gives the fallback (strongly, `spec:77`, `spec:52`) but not the order. Neither FR-2 nor FR-6 says which control traversal reaches first, whether any focus is placed when both cards render, or that destructive controls ("Cancel approved work", "Reset layout") are never first in traversal and never receive default or restored focus after a rebuild. AC-X only asserts that no cue appears and that stale Enter/Space does not mutate. If D-07 is approved with F6 traversal, an unspecified order can put "Cancel approved work" ahead of "Keep approved work" and a keyboard user is one Enter from the destructive path.
- **Required fix:** add to FR-2/FR-6 and AC-X: in the dual state, no default or restored focus; safe controls (`map-consent-decline`, `map-edit-reset-cancel`) precede destructive ones in traversal; focus restoration after Refresh never lands on a destructive control; and an AC-X native-input check for it. Same rule for the single-prompt cases and the Advance warning.

## Non-blocking findings

- **N1 — existing static primaries contradict "exactly one highlighted action".** Current main has permanent `btn-primary` on `Back to forest` (`src:LearningObjectivesView.cs:75`, `StandMapView.cs:93`), `Advance one year` and `Read first annual results…` (`WorkPlanView.cs:161-163`), `Confirm Rack Plan`, `Reopen Rack Plan`, `Plan Racks` toggle (`StandMapView.cs:263,646,726`). FR-1 says "other enabled actions remain secondary" (`spec:67`) but the spec only assigns restyling for the three rack prompts (D-08). T2 must enumerate which static primaries are demoted or conditioned (e.g. `Advance one year` is primary while Approve Work Plan is the real next step, `spec:93`). Add to the T2 candidate-change list.
- **N2 — the Advance warning already has a safe primary.** `WorkPlanView.cs:66` styles "return" `btn-primary` and leaves `Approve`/`Advance anyway` unstyled, so R-02 does not apply there as it does to the three map prompts. `spec:75` groups them. Harmless, but say so to avoid T2 "fixing" something already correct.
- **N3 — F6/Shift+F6 is a new binding.** I found no F6 use in `Assets/ForestPrototype/*.cs` and none reserved in `specs/` or `Docs/Project/` on main, so no conflict *in source* (not exhaustive across the unmerged #257 draft, which I did not read). Note, as inferred risk only: making UITK buttons `focusable=true` also enables the toolkit's default Tab/arrow navigation, which collides with the global Tab Work Plan handler (`src:ScenarioOneUiRoot.cs:358`, `ScenarioOneManager.cs:586-591`). AC-K already tests Tab reservation; add "default UITK Tab/arrow focus navigation suppressed or harmless" as an explicit sub-check. The Manager still chooses scoped-F6 vs mouse-only first slice; the spec correctly offers both.
- **N4 — 16 physical px needs a stated unit conversion.** Per `src:ScenarioOneUi.uss:43-45`, 1 UI unit = 0.8 px at 720p and 1.2 px at 1080p. A 16 px floor at 720p is ≥20 UI units, above `--fs-button` (17) and `--fs-body` (18). `spec:144` demands measurement, which is right; state the 720p target as ≥20 UI units so a "Next:" badge on a 17-unit button cannot pass.
- **N5 — diagram tolerance vs "schematic, not to scale" tension.** `spec:122` says schematic/no survey precision; `spec:131` imposes ≤1° direction and ≤1% displacement. Clarify that the tolerance applies to topology/relative geometry after the documented uniform transform, not a claim of survey accuracy. Source supports a real fixture: suggested racks run from `HeadlandX=-35.4` at each entrance Z to `farX` (`src:ScenarioOneSuggestedRackLayout.cs:27-54`); the numeric fixture is rightly marked NOT YET APPROVED.
- **N6 — "FAIL 08" is referenced but not defined** in any of the five artifacts (`spec:19`, `spec:150`, `plan:31`, `tasks:48`). It is defined only in the original packet lineage. Add a one-line pointer so a qualifier cannot misread it.
- **N7 — source base is pinned to `1e3e159b`; main is now `1dd9650d`.** `1e3e159b` is an ancestor of main (10 commits behind) and `git diff --stat 1e3e159b origin/main -- Assets/ForestPrototype/{UI,ScenarioOne,Racks}` is empty, so the ledger is still valid. Stage B/D must still re-audit against its actual integration base (amendment: workbench integrated first). Spec Q4 (`spec:175`) already says this.

## Answers to the review questions

**1. D-071 fidelity — PASS.** `spec:15` states the existing southern-headland entrance and FIVE PARALLEL Rack V2 racks and that D-067–D-069 are superseded as Scenario One production/design targets. This is restated in `spec:17`, `plan:17`, `tasks:22/34`, `requirements:42-43`, handoff line 26. A grep for northern/herringbone/diagonal/3+2/spine/extension in all four artifacts finds only negations ("no northern spine…", "zero invented northern/diagonal/spine edges", `spec:130`). I found no residual live-spine, diagonal, single-family-entry or player-extension sentence. Visual Option A = five parallel racks is kept distinct from historical MFM Option A at `spec:15` and `requirements:43`, matching D-071 and current-milestone §"access-network scope". The diagram is clearly illustrative (`spec:17`, `spec:122` required caption "not your saved Stand Map") and `spec:17`/`plan:17`/`tasks:73` disclaim any Stand Map/3D/minimap network change. D-070 is excluded.

**2. #270 High blockers — PASS with B2 caveat.**
- R-01 (`spec:50`, FR-6 `spec:138-142`, D-07, AC-K): `UiKit.Button` `focusable=false` confirmed at `src:UiKit.cs:62-70`; session-menu opt-in confirmed at `StandaloneSessionMenu.cs:109-112`. Disposed as an explicit Manager decision with a mouse-only fallback; marked NOT FIXED. Matches the amendment ("contradictory UiKit.Button behaviour is a STOP + Manager design decision").
- R-02 (`spec:51`, `spec:75`, D-08, AC-P): `btn-primary` on "Cancel approved work", "Reset layout", "Reopen the plan" and ghost on the safe choices confirmed at `src:StandMapView.cs:629-632,685-688,716-719`. Disposed as an explicit T2 restyle and a rule that destructive controls never get a cue. Correct owner, decision and gate. Note N2.
- R-03 (`spec:52`, `spec:77`, D-09, AC-X): co-visibility confirmed — `RequestReset` sets `ResetPending` without clearing `consentRetry` (`src:StandMapRackEditor.cs:496-509`) and the Reset button is gated only on `!ResetPending` (`StandMapView.cs:619`). The no-primary fallback is explicitly load-bearing and is not folded into a hidden lifecycle fix. Missing piece: B2.
- Prior P1/P2/P3 FIXED claims are withdrawn in all four artifacts (`spec:46`, `plan:11`, `tasks:6`, `requirements:8`, handoff line 51).

**3. #270 mediums — PASS (truthfully dispositioned, `spec:53-61`).** Guidance-vs-input ownership, pointer-down prompt clearing (verified `src:StandMapRackEditor.cs:177-187` clears `ResetPending`/`consentRetry` on an eligible left press), first-use Help cue, Help force-close, rack availability gates (`src:ScenarioOneUiRoot.cs:447-464`; `RackPlanProblem`, `RackApprovalProblem`, `CanEditWorkPlan`, `CanAdvanceYearNow`, `NeedsAnnualReview`, `RackPlanningAvailable` all exist), unsaved `rackMode`, 16 px, no screen reader, judgement-only diagram. None is claimed fixed. Screen reader is explicitly deferred (D-10) and "visible text is not a delivery mechanism".

**4. Cue contract — PASS with B1.** One enabled truthful action (`spec:67`), disabled never the cue, reason shown, non-colour "Next:" + outline/weight (`spec:67`, `spec:144`), no auto mark/fell/approve/advance (`spec:69`, `plan:15`), no second progression engine and no new save keys (`spec:21`, `spec:65`, AC-S). The FR-3 sequence (`spec:85-96`) is Objectives → Stand Map → Plan Racks → Confirm → entrance marking → Work Plan approval → advance/annual review, and matches the existing helper (`src:LearningObjectivesView.Curriculum.cs:100-106`) and gates (`WorkPlanView.cs:159-163`, `ScenarioOneUiRoot.cs:455-464`). Defect: B1.

**5. Manager defaults — compatible; one Manager choice remains, no user ambiguity.**
- Scoped focus preserving Tab / no global rewrite: matches (`spec:140-142`; `spec:79` STOP if wider routing).
- Safe modal precedence: matches except B2 (selection order).
- Static labelled diagram, no live inset: matches (`spec:118`, `spec:122`).
- Bounded O1 first slice, later lessons enumerated honestly: matches (`spec:102-114` FR-4, REVIEW NEEDED rows, T4 optional).
- Current integration lineage: acceptable (Q4, N7).
- No new save fields: matches.
Q1 (F6 vs mouse-only), D-08, D-09, D-10, Q2/D-11, D-12, Q3/D-13 and Q4 are all drafted as recommendations. The amendment defaults settle D-08, D-09 (no-primary), D-10 (defer), D-11 (static diagram), D-13 (O1 first, no new HUD) and Q4. **Only Q1's concrete binding (F6/Shift+F6 vs mouse-only) is not settled by the amendment** — a Manager design call, not a user question. No genuine product ambiguity requires a STOP.

**6. Diagram rubric — PASS (measurable, with N5).** `spec:126-136` gives pass/fail rows at 1280×720 and 1920×1080, colour and grayscale, for topology (exactly 5 racks, 5 entrances, no invented edges), direction/displacement/width tolerances, retention in 4 inter-rack and 2 outer bands, state semantics by pattern/shape/text, labels/legend, legibility and 4/4 independent recognition. Plan/confirmed/executed are separated at `spec:124`. Geometry is derived from verified source (`ScenarioOneHeadlandGeometry.cs:7-23`, `ScenarioOneSuggestedRackLayout.cs:22-54`, symbols `StandMapSymbols.cs:86-107`, all exact), and the numeric fixture is explicitly "NOT YET APPROVED" rather than invented.

**7. Evidence integrity — PASS.** No gate weakening, no static FAIL relabelled PASS, no fabricated captures; every AC row is NOT RUN (`spec:150`). Facts (S1–S14), proposals (FR-1–6) and UNKNOWN/REVIEW NEEDED are separated. Status is DRAFT-only in all five artifacts. The handoff's five-file claim matches `git show --stat aade967d` exactly (5 files, no others). N6 is a provenance gap, not an integrity breach.

**8. Source citations — PASS (no mismatches found).** I spot-checked against `origin/main`: S4 (button classes and names), S5 (`StandMapRackEditor.cs:53-58,177-187,487-512`), S7 (`ScenarioOneUiRoot.cs:447-464`), S8 (`UiKit.cs:62-70`, `StandaloneSessionMenu.cs:109-112`), S9 (`TeachingDiagramPlan.cs:207-235`, two rack columns), S10 (`StandMapSymbols.cs` 86/87/90/91/94/95/97/100-104/106-107 all exact), S11 (suggested layout IDs PO0203/PO1204/PO2003/PO2803/PO3803), S12 (`ScenarioOneHeadlandGeometry.cs:7-23`), S14 (`ScenarioOneUi.uss:43-53`). One imprecision: S8 cites `UiKit.cs:29-36` which is `MakeModal`, not the focus setting (that is at 62-70); harmless.

## Evidence classes

- **Verified by reading source:** everything cited above, via `git show`/`git grep` against `origin/main` 1dd9650d and `aade967d`; D-071 and the current-milestone section on main; the amendment; the #270 review text; the 5-file diff of `aade967d`.
- **Inferred, not verified:** UITK default Tab/arrow navigation interaction (N3); that the F6 absence from `*.cs` and `specs/` implies no reservation across unmerged branches; rendered 720p/1080p unit-to-pixel results.
- **NOT verified:** any Unity compile, import, render, input dispatch, focus behaviour, screenshot, grayscale capture, save/load, or the recognition rubric. No Unity process was started. I did not read the original packet's FAIL 08 definition beyond a grep, or #257 (PR #263).

## Manager decision summary

- **Needs a Manager choice:** Q1/D-07 binding (scoped F6/Shift+F6 + Enter/Space, or mouse-only first slice); B1 option (a) or (b); acceptance of the B2 safe-order rule; D-11 numeric fixture and tolerances (before T3); Q4 concrete implementation base/lane registration.
- **Settled by the amendment defaults (Manager need only ratify):** D-08 safe prompt emphasis; D-09 no-primary fallback; D-10 screen-reader deferral; Q2 static labelled diagram of the D-071 five-parallel-rack family; D-12 new-text-only floor (add N4 wording); D-13/Q3 O1-first, no new HUD widget, later objectives as REVIEW NEEDED tasks.
- **Needs the user:** nothing, unless the Manager declines both amendment defaults and F6 and wants a different representation.
- **Recommended path:** author (or Manager) applies B1 and B2 as a small text revision, then Manager issues G4/G5. Stage B stays blocked until the status is actually APPROVED.
