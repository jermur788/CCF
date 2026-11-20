# CCF #858 — independent blank-starter specification delta review

**Verdict: NEEDS CHANGES — DOCUMENT QUALITY ONLY.**

This is not implementation authorization and confers no G4/G5 approval, source grant, integration permission, or owner acceptance. The specification remains **DRAFT / NOT APPROVED / NO IMPLEMENTATION AUTHORITY**.

## Identity and scope

- Requested preferred reviewer: **GPT-6.1 Sol**, unavailable in this session. Recorded substitute actually executed: **GPT-6.1 Terra**, OpenAI, OpenCode.
- Separately assigned review session; the requester states it was spawned by another session. I did not author the correction and had no implementation role. Correction author was identified in the packet as Claude Sonnet 5.5. The harness origin and author independence beyond the supplied assignment were not independently audited; no nested reviewers were used.
- Reviewed correction branch `docs/ccf-856-blank-starter-review-corrections-20261120-v1`, parent **`4ea5a10db061b9fe69601118649bb77b698877cf`**, candidate **`c4d75f8e301fa1d755105531e3411588f6779b9d`**. Direct diff confirms exactly four specification files, +39/−25. GitHub branch API confirmed candidate SHA.
- Review publication branch started at `origin/main` **`cba0a905b766ce8fc0356d351ac5bff1c2a9a008`**, in an independent worktree. `S/` below means `specs/ccf-blank-scenario-starter-v1/`; all line numbers refer to candidate `c4d75f8e301fa1d755105531e3411588f6779b9d`. Review is limited to this delta and its necessary local context, not a fresh full-system review.
- Prior review: [#858 independent review](https://github.com/jermur788/CCF/blob/da2c023f6b21221173ffd71e15b09c5cc2e7a437/Docs/Handoffs/CCF-Blank-Scenario-Starter-20261120/Dashboard-CCF-858-BLANK-STARTER-SPEC-INDEPENDENT-REVIEW-20261120.md).

## Prior findings

| ID | Status | Evidence and disposition |
|---|---|---|
| F-01 | **PARTIALLY RESOLVED** | `S/spec.md:62,83`, `S/plan.md:20`, `S/tasks.md:29,33`, `S/checklists/requirements.md:15,35` consistently remove the undefined in-task production gate, defer it to #473, and prohibit a second editor/resolver. FR-09/AC-08 IDs remain. The visible marker plus preview indicator is observable, and AC-08 also asks for nonempty text. However, the checklist screenshot alone does not establish that each marker follows its actual configurable slot rather than being a static decoration; see D-02. #473 is a deferred DRAFT ownership reference, not proof of a working production validator. |
| F-02 | **PARTIALLY RESOLVED** | The substantive clauses now mandate candidate **and evidence** review independent of implementation author/session before acceptance or integration: `S/spec.md:85,93`, `S/plan.md:31,59`, `S/tasks.md:55,59,63`, `S/checklists/requirements.md:50`. Search found no remaining active conditional “if mandated”, “as required”, “where needed” or “when required” review clause; `S/spec.md:112` quotes historical wording. Nevertheless the future handoff still links the original uncorrected package (D-01), so a worker following its purported complete package can encounter the old optional-review language. |
| F-03 | **RESOLVED** | `S/tasks.md:27,29` distinguishes unconditional `RackLayoutTracker`/`StandMapSource` from manager-conditional `RackTool`/`PlannedFellMarkers` and requires empty behavior proof. |
| F-04 | **RESOLVED** | `S/spec.md:24,78` identifies cleared Racks/Trees/Juveniles/Shelters and player/waypoint flags, retained Paths/Landmarks/MiniGuidance and cached bounds/grid, and requires transition testing if reachable. `S/checklists/requirements.md:11,38` carries layer checks without asserting an observed rendered ghost. |
| F-05 | **RESOLVED** | `S/spec.md:54,76,82` and `S/checklists/requirements.md:7,16,37` cover positive finite X/Z, invalid input, both footprint spawn checks and measured dimensions with proposed ±0.05 m tolerance. The newly chosen margin and invalid-input policy still need clear proposal labeling (D-03), not a reversion of the coverage. |

## New findings

### D-01 — stale future worker package links — BLOCKER

`S/tasks.md:63` describes its four links as the “Complete source-backed package” but they point to `docs/ccf-blank-starter-856-gpt6-g0-20261120-v1`, the pre-correction branch. The originally reviewed `4ea5a10` documents contain the undefined production gate and conditional independent-review language. A later G1 worker following these links can therefore execute against the very text this correction is meant to replace, despite the corrected mandatory review sentence in the surrounding handoff. This is a new consequence of publishing corrections on a separate branch, not a claim that those links were newly added.

**Correction:** bind all four handoff links to the same explicitly approved corrected spec revision, or make the packet require the future approved full SHA and remove or unmistakably mark these links as historical, non-authoritative references. Do not point a future worker at a moving or unapproved revision as if already accepted.

### D-02 — empty-slot witness does not prove binding — SHOULD

`S/spec.md:62,83` requires each slot to show an empty marker, and a nonempty slot to render its text; `S/checklists/requirements.md:15,35` asks for a running screenshot but specifies neither a controlled edit of each underlying authoring slot nor a negative control. A fixed marker/preview banner can satisfy the screenshot while failing to reflect actual empty/nonempty authoring data. A separate generic nonempty display would not prove the correct slot is bound.

**Correction:** for intro, objective and guidance separately, set the actual authoring data empty, observe marker plus indicator; set a distinctive nonempty value, observe that same slot change to that value; clear it and observe the marker return. Preserve the exact candidate and capture inputs and output. This is preview behavior only, not a production validator or new editor.

### D-03 — new choices not explicitly labeled as proposals — SHOULD

`S/spec.md:54` states controller-radius margin and reject-not-clamp as mandatory rules, including that a clamp alternative needs G4; `S/plan.md:21`, `S/tasks.md:33`, and `S/checklists/requirements.md:7` repeat them. The documents are DRAFT, but unlike the tolerance at `S/spec.md:76`, these specific choices are not explicitly identified as proposed choices for G1 setup rather than previously established facts/decisions. A future approver could mistake the unqualified margin or refusal policy for already settled owner intent.

**Correction:** label controller-radius margin, rejection instead of clamping, and ±0.05 m as proposed G1 test/design choices pending the relevant approval and pinning; retain their explicit candidate acceptance assertions and safe invalid-input behavior until an authorized decision changes them.

## Consistency and limits

- Four-file static diff and checklist mapping inspected: no FR or AC ID was dropped. No other weakened requirement or scope expansion was identified in this delta. Save isolation, original-world protection and no runtime JSON resolver remain stated. This is not a proof that future implementation can satisfy them.
- Section 10 (`S/spec.md:107–115`) accurately describes the written textual changes, but its F-02 “uniformly mandatory” summary is incomplete in practical handoff terms because of D-01; F-01's observable test needs D-02. It records no approval.
- `S/tasks.md:55` refers to tests after **candidate composition** and then review. Candidate composition for qualification is not assumed to be integration into shared/main; clarify in the eventual packet if this terminology is used operationally. Explicit before-acceptance/integration gate remains in T7.
- Proposed tolerance is explicitly labeled in the spec and setup; D-03 concerns consistent status labeling of all three new choices. No claim is made that controller geometry was measured.
- Worktree was clean at review start; unrelated modified/untracked work in `/media/jer/Files/CCF-Primary` was preserved. No existing document or spec was edited.

## NOT CHECKED / NOT RUN

- Unity, compiler, Player, scene, save/access trace, runtime, graphics, historical regression and integration tests; no Unity lock or lease was acquired.
- Full re-review of unchanged original design, all underlying source, full parent packet/issue history, #473 implementation or production-publication readiness. No G1/G2/G3/G4/G5 decision or acceptance is inferred.
- Dashboard ingestion/synchronization and issue notification were not part of this publication; Manager follow-up is required to synchronize the authoritative operational record and verify the published dashboard.
- The parent session's actual identity and its relationship to any prior implementation session were not independently established. Independence from the correction author is based on this separate assigned session and supplied identity.

**Next:** bounded documentation corrections for D-01–D-03 and a delta re-review; retain all existing approval and implementation gates.
