# Multi #821 — Independent nonauthor review of the #817 one-area Stage 5 four-file DRAFT correction

**19 November 2026 · Verdict: NEEDS CHANGES — DOCUMENT QUALITY ONLY · HANDED-OFF to Multi Manager [#730](https://github.com/jermur788/CCF/issues/730)**

## 0. Identity, scope and evidence boundary

- **Reviewer:** Anthropic Claude Opus 5.5 (`claude-opus-5-5`), Claude Code session launched by the owner. This session did not write #812, #816 or #817, and it is a different provider from both GPT-6 sessions (#816 and #817).
- **Claim:** [#821 RUNNING receipt](https://github.com/jermur788/CCF/issues/821#issuecomment-7016556432), posted before this file was written.
- **Lane:** git fetch (read-only) plus GitHub REST. This review branch was built with git plumbing directly from fetched `main`: no checkout, no edits in any local worktree, no T3/.NET/Unity, and no physical Ops board or lease. The one write is this Markdown file. Branch `review/multi-821-817-stage5-onearea-independent-20261119-v1` comes from observed `main` `159f0ff8b075e275045ddd1ab0925cb447364b77`.
- **Exact inputs, read in full:**
  - Candidate `docs/multi-817-stage5-one-undivided-area-correction-20261119-v1` @ `9df2470d5226d874c7e6b527c5137ace9850ee32`, all four files: `spec.md` 119 lines, `plan.md` 65, `tasks.md` 84, `checklists/requirements.md` 85.
  - #817 author handoff.
  - Base #812 `docs/multi-812-stage5-intervention-spec-20261119-v1` @ `300c5589eef5f00ffef42d500bc32c0bc56b896b`.
  - [#816 review](https://github.com/jermur788/CCF/blob/docs/multi-816-independent-stage5-one-rack-review-20261119-v1/Docs/Handoffs/MultiRack-Simulation-20261116/816-orchestration/Multi-816-INDEPENDENT-STAGE5-ONE-RACK-SPEC-AND-OWNER-DECISION-REVIEW-20261119.md).
  - [One-undivided-area owner clarification](https://github.com/jermur788/CCF/blob/docs/multi-730-817-owner-one-area-clarification-20261119-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-730-817-OWNER-CLARIFICATION-ONE-UNDIVIDED-AREA-NO-RACK-GEOMETRY-20261119.md).
  - [Owner strategic correction #819](https://github.com/jermur788/CCF/issues/819#issuecomment-7015429935), posted 2026-11-19T16:35:51Z.
  - The #821 dispatch handoff and the #821 issue thread.
- **Compare verified:** `git diff --stat 300c5589..9df2470d` shows exactly 5 paths: the four modified spec Markdown files plus the added #817 handoff. +374/−297 lines, no source, fixture, engine, Host, Unity or coordination paths. The merge base is #812 exactly. The candidate is 5 commits ahead, timestamped 16:44:14Z–16:49:54Z.
- **Checklist machine check:** in the candidate `requirements.md`, `- [ ] **R-` appears **26** times (R-01…R-26 sequential), `- [ ] **X-` appears **24** times (X-01…X-24 sequential), and `[x]`/`[X]` appears **0** times. Result: **26 + 24 = 50 unchecked, 0 checked.**
- **NOT verified (not claimed):** any Multi source/engine code, the G1 or G2 seam, native/.NET tests, browser/accessibility behaviour, Unity, the physical Ops board, the live `active-tasks.md` lock state beyond the #821 issue thread, and Dashboard/Drive mirrors. This is a text and provenance review only. It does not review an implementation or source contract, and it accepts no spec. #730 owns the formal spec decision.

## 1. Verdict

**NEEDS CHANGES — DOCUMENT QUALITY ONLY.**

The correction does its main job well. All four files consistently treat the 30 × 30 m / 100-Sitka P1 stand as **one undivided management area with no rack mechanism**. Every remaining use of "rack" is an exclusion, a historical label or a future-scope note. I found no live rack Confirm, entrance, corridor, served-tree, clearance or geometry-lock requirement.

The decision chain is coherent and consistent across the four files: manual/sample → optional G1 proposal → PendingWork → sole whole-batch Work Plan approval → separate G2 Execute → genuine Year+1. G2 is a mandatory STOP, G3 is deferred to Stage 7, G4 shows UNAVAILABLE (never zero), and the 26/24 checklist is fully unchecked. The #816 findings that the owner superseded (P0-1, P0-2, P1-4) were dropped correctly. Its G1/G2, plan-versus-actual, atomicity, G3/G4 and evidence cautions were kept, and in places strengthened (zero-live, stale handling, Year0–10 oracle).

It is **not approvable unchanged** because of one major finding: the owner's later **#819 product-purpose correction** appears nowhere in the four files. #817 was committed 9–14 minutes after that comment and cites only the earlier G0 clarification. The text still frames multi-area work as indefinitely "LATER / future separately approved scope" and G1 as merely "optional". That leaves the documents open to being read as if a one-area Stage 5 completes Multi's purpose. The owner explicitly rejected that reading, and the #821 dispatch made it an explicit review criterion (§3).

The fix is small and text-only. It needs no new R/X items, no rack restoration and no change to Stage 5 implementation scope. The other findings are minor clarity defects that can go into the same correction.

## 2. Ranked findings

### Critical

None. No file revives rack mechanics, lets a candidate or proposal Fell, adds a second commitment gate, claims G2 proof, claims save durability, invents economics, or checks a box.

### Major

**M-1 — Owner #819 strategic purpose and the near-term multi-area critical path are missing; multi-area is framed as indefinite "LATER".**

Where the text falls short:

- `spec.md` §1 (L13–15): states only the P1 one-area purpose. It says "A later larger stand *may* have multiple management/rack-served areas".
- `spec.md` FR-03 (L62): "Future multi-area is separately specified."
- `spec.md` §7 G0 (L111): "future separately approved scope, not a code dependency now".
- `spec.md` §7 G1 (L112): "OPEN, optional".
- `plan.md` P0.1/P0.4 (L10, L13): no follow-on milestone.
- `tasks.md` L84: "**Multi-area … separate LATER work**".
- `tasks.md` T-02 (L29): G1 is "a separate optional subpacket".
- `checklists/requirements.md` R-01/R-07/R-25: no follow-on.

None of the four files cites #819.

Why it matters: the owner said a one-area Stage 5 is "an intermediate dependency, not Multi's purpose achieved". The owner also said G1 is "a priority proof obligation, not a perpetual optional deferred embellishment", and that #730 "must now bring an explicit near-term cross-area G1/coverage/extrapolation milestone" into the roadmap. The #821 dispatch requires the four files to record this as a separate near-term critical-path requirement. The current wording invites exactly the indefinite Stage 8+ deferral the owner ruled out. This is a defect in the documents. It is not an open source gate.

Required correction:

- **`spec.md` §1:** add one short paragraph that cites #819. It should say:
  - Multi's ultimate purpose is a representative sample in one area, then source-defensible conservative proposed treatment in genuinely comparable **multiple** management areas, followed by explicit player review, an ordinary approved Work Plan, an actual authoritative Fell and genuine per-area subsequent years.
  - Stage 5 is the one-area manual foundation and does not complete that purpose.
- **`spec.md` §7:** re-label G1 along these lines: "OPEN — optional *within the Stage 5 one-area slice* (fail-closed, manual-only still valid); a **priority near-term proof obligation** for the Multi multi-area milestone, not an indefinite deferral". Add a **near-term follow-on** bullet:
  - a separate spec covering a larger independently provenance-checked fixture with at least one comparable and one different/unknown area;
  - comparable/noncomparable/unknown coverage and review;
  - its own approval;
  - protected P1 100 IDs and Year0–10 golden unchanged;
  - prepared in parallel with the G2 audit, not as a Stage 5 code dependency.
- **`plan.md` P0 and P4:** add the same follow-on as a planned parallel documentary track.
- **`tasks.md` final section:** replace "separate LATER work" with "separate **near-term** follow-on spec (not a first-pass T-03/T-04/T-05 task)". Optionally add a FUTURE/UNCLAIMED T-09 stub.
- **`checklists/requirements.md`:** update the R-01 and R-25 wording only. Keep **26 R / 24 X, all unchecked**.
- **Do not** restore rack geometry, change P1 IDs, or make Stage 5 depend on G1.

### Minor

**m-1 — The prescription state wording is ambiguous.**

`spec.md` §3.1 (L52) says "PRESCRIPTION_REVIEWED / PRESCRIPTION_APPROVED (**optional for proposals**; only reconciles pending orders)". This can be read as "proposals may skip prescription approval". That would contradict §3 step 5 (L44), FR-07 (L66), R-11, X-09 and plan Slice C, which all require separate prescription approval before a proposal becomes PendingWork.

Fix: "required for any proposal to become PendingWork; not used for manual work". Also restore #812's sentence: "new edits invalidate prescription approval until reviewed again".

**m-2 — The spec does not say what happens to PendingWork or an approved-but-unexecuted batch when the player advances the year.**

What the text does say:

- `spec.md` §3 step 6 (L45): advancing never approves or executes.
- §3.1 (L54): "Stale edits invalidate pending/approval".
- R-13 and X-12: execution refuses a wrong year.

What none of the four files says is whether year advance is **blocked while an approved batch exists**, **allowed with a warning and automatic invalidation/cancellation**, or something else. #812 §3.1 had flagged a "safe policy for already-approved-but-not-executed work". It is still undefined. This is a small product decision, not a source gate.

Fix: state one rule. Recommended conservative default: advancing with pending or approved unexecuted work shows a warning, needs consent, and marks those orders STALE/CANCELLED (zero Fell); they must be re-reviewed against the new year. Reflect the rule in FR-10/FR-11 and the X-15 or X-17 negative wording, without changing the counts.

**m-3 — The validation boundaries do not match across files.**

- FR-08 (L67) lists prescription materialisation, Work Plan approval and execution.
- R-13 says "proposal/pending/approval/execution".
- X-08 tests stale or duplicate **manual** orders.
- FR-08 omits the manual "Add to Work Plan" boundary.

Fix: add "manual Add to Work Plan" to FR-08 so the spec matches R-13 and X-08.

**m-4 — There is no rule for qualifying G1-conditional cases while G1 is disabled.**

X-07 correctly says "remains NOT RUN if G1 absent, not a fabricated PASS". X-09 depends on G1 in the same way. But T-08 (L76) and plan Slice F require evidence for "every R-01–R-26 / X-01–X-24". As written, a G1-disabled Stage 5 either can never pass qualification or tempts someone to fabricate a pass.

Fix: add one sentence to the checklist preamble or T-08. With G1 disabled:

- R-07/R-08 and X-07/X-09 are dispositioned **NOT APPLICABLE — G1 DISABLED**, backed by positive evidence that extension is disabled and the Manual/NotCovered path works (X-05);
- #730 decides whether to accept that disposition;
- such a box is never ticked as PASS.

**m-5 — The proposal and sample records are not shaped for the multi-area seam the owner asked for.**

The owner clarification asks contracts to "scale to multiple areas without requiring a future layout". `spec.md` §5:

- puts a management scope on `PendingWorkOrder` (L86);
- puts none on `SampleRecord` (L84);
- has no source-scope versus target-scope pair on `ProposalRecord` (L85).

Cross-area proposals (sample in area A, targets in area B) are exactly the #819 target.

Fix: add a conceptual "sample management scope" field to `SampleRecord`, and "sample scope → target scope" to `ProposalRecord`. Each still has a single V1 value. Do not add topology.

### Nits

- **n-1:** `tasks.md` T-07 (L68) says "previous-versus-**proposed** view read-only", but `spec.md` FR-12 (L71) says "previous-versus-**current**". Align them.
- **n-2:** `spec.md` §3 step 8 (L47) no longer states the explicit pre/post live count / BA / tree-level result review that #812 step 8 had. It survives only in FR-09 and the §5 `ExecutedWorkEvent` row. Consider restating it in step 8 as the player-facing review. It is also the per-area outcome review that #819 point 4 anticipates.
- **n-3:** the #817 handoff filename still says "ONE-RACK" (`…STAGE5-ONE-RACK-FOUR-FILE-SPEC-CORRECTION-RESULT…`). Its content is correct. Leave the path as it is for provenance. Just do not read the filename as a rack requirement.

## 3. Criterion-by-criterion matrix (dispatch §"Review criteria")

| # | Criterion | Result | Evidence |
|---|---|---|---|
| 1 | G0 / separation | **PASS** | spec §1 L13–15, FR-03, AC-03, §7 G0; plan P0.1, Slice A; tasks header and T-03 exclusions; R-01/R-02, X-03, X-23 negatives. Not all trees selected: spec §2 L34, R-05, X-04. Exact 100 case-sensitive IDs and single committed source: §2, FR-01, R-03, X-01/X-02. Distinct worlds: §2 L31, R-02 |
| 2 | Player decision chain | **PASS with m-1, m-3** | spec §3 steps 2–7, FR-05/07; R-10–R-12; X-06, X-08–X-12. Work Plan approval is the sole gate (L45, FR-07, R-12) and separate Execute is required (L46). Draft, candidate, proposal and pending stay at zero biology (L43, FR-05, X preamble) |
| 3 | G1 | **Stage 5 slice: PASS; strategic framing: FAIL → M-1, m-5** | Same Sitka/age/area is not a proxy: L42, FR-04, R-07, X-05/X-07. DISABLED/Manual/NotCovered fallback with manual path still valid: AC-05, plan P0.4, T-04. The near-term multi-area critical-path note is missing |
| 4 | G2 mandatory STOP | **PASS** | spec §3 step 7, FR-09/FR-14, §5 closing paragraph, §7 G2; plan P0.5, P1, Slice D; T-02/T-06; R-14–R-16, R-20; X-12–X-14, X-19. No G2 proof claimed anywhere. No UI deletion or relabelled year: L36, L47, X-16. No #789 rebaseline: FR-16, X-17, X-22 |
| 5 | G3 / G4 | **PASS** | FR-12/FR-13, §7 G3/G4; plan P0.6; R-21, R-23; X-20, X-23. No affordability gate: §5 `WorkPlanRevision` L87. UNAVAILABLE, never zero/free, in all four files |
| 6 | Negative / evidence matrix | **PASS with m-4** | 26 R / 24 X / 0 checked, machine-verified. Covers stale, case-shifted, missing, dead and duplicate IDs (X-01/X-04/X-08/X-12); zero-live (X-19); repeated work (X-14/X-18); no-work (X-17); accessibility (X-21). "No checking by checkbox alone" in the preamble and plan P3 L54 |
| 7 | Internal consistency | **Minor defects only** | m-1 to m-5 and n-1/n-2 are actual text defects. Deliberately open gates, **not** defects: G1 comparator proof, G2 seam, the zero-live terminal policy (FR-14 correctly returns it to source G2 / #730 policy), source ownership and physical grants |

## 4. Superseded versus live #816 findings

| #816 finding | Status in #817 | Assessment |
|---|---|---|
| P0-1 no-rack contradicts G0; P0-2 rack contract; P1-4 rack lifecycle | Removed and replaced by one-area G0 | **Correctly superseded** by the owner clarification. Not reintroduced |
| P0-3 G2 open | Kept as a mandatory STOP | **Correctly kept** |
| P1-5 G1 and manual fallback | Kept | **Correct for the slice**, but see M-1 for the strategic priority |
| P1-6 G3 Stage 7; P1-7 G4 deferred; affordability removed | Kept | **Correct** |
| P1-8 acceptance matrix rewrite, 50 unchecked | Done without rack cases | **Correct** (see m-4) |
| P2-9 historical labelling | spec §1.1 non-transfer table | **Correct** |

## 5. Bounded correction instruction for #730

One docs-only correction pass on the same four files at `9df2470d` is enough. It should:

1. fix M-1, citing #819;
2. fix m-1 to m-5, and optionally n-1/n-2;
3. keep **26 R / 24 X / 0 checked**;
4. keep DRAFT status;
5. add no rack mechanics, source, fixture or P1 ID changes.

A short **focused re-check of only those deltas** by a nonauthor (not a full re-review) should then precede the #730 formal spec decision. After that, follow the #821 dispatch sequence: G2 audit, #804 provenance, authorised implementation. Prepare the separate near-term multi-area / G1 feasibility spec in parallel.

## 6. Handoff

- **Status:** **HANDED-OFF to #730.** This review does not approve the spec. The verdict is document quality only: **NEEDS CHANGES (1 major, 5 minor, 3 nits; 0 critical)**.
- **Not done:** no source, native, Unity, physical Ops, spec edits, `active-tasks.md` edits or main merge.
- **Dashboard/Drive:** NOT VERIFIED.
- **User action:** none, unless #730 wants an owner ruling on the m-2 year-advance policy. A conservative default is proposed above.
