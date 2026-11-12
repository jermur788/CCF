# Overall Manager decision — HP #409 D-073 Annual Review comparison, 12 November 2026

**Verdict: APPROVED — BOUNDED D-073 SPECIFICATION ONLY.** The [four approved files](https://github.com/jermur788/CCF/tree/docs/manager-d073-approved-scope-20261112-v1/specs/scenario-one-multiyear-decision-outcome-feedback-v2) are the current task-level authority for this feature, on **this review branch**. This does not make any game code implemented, Unity-verified, integrated, merged, user-tested or accepted. No implementation or machine lease is granted by this approval.

## Sources reviewed and worker disposition

- [#409](https://github.com/jermur788/CCF/issues/409) has HANDED-OFF/CLOSED, with an actual worker [five-file result](https://github.com/jermur788/CCF/blob/docs/d073-annual-review-spec-hp-20261112-v1/Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/HP-D073-ANNUAL-REVIEW-SPEC-REFINEMENT-RESULT.md). Reviewed actual four DRAFT files on its branch, their exact compare with the #406 offline-readout base (two commits ahead, five touched paths only), the source of the approved [#406 helper](https://github.com/jermur788/CCF/blob/qualify/scenario-one-management-followon-readout-20261112-v1/Assets/ForestPrototype/ScenarioOne/ScenarioOneManagementFollowonReadout.cs), and the existing `AnnualReviewView.BuildWork`/canopy formatting code. The user-reported command/log and WSL proof are worker evidence; Manager did not re-execute scripts or access HP disk.
- #409's doc checks reported clean diff/whitespace, 10 valid relative links and consistent requirement/acceptance/task references. The Manager read current four-file contents and verified that FR-1–14, AC-1–12 and T-00–06 remain defined and in scope after amendments.
- The worker actually ran **Claude Fable 5.2**, not the packet's preferred Sonnet. T3 shell was Windows host invoking Ubuntu through `wsl.exe`, but all Git/source operations were on the Ubuntu WSL filesystem, and the worker disclosed the route. Neither changed project source or a protected lane; no re-execution is claimed. It read D-073 from the newer `main` decision authority because the pinned helper base's Decision Log predates it; correct accepted-intent precedence. No regenerated instruction snapshot/context mirror is asserted.
- Physical board #409 row was previously verified READY/NOT RUNNING by [#411](https://github.com/jermur788/CCF/issues/411), and GitHub Manager released the HP task. At #409 handoff, the worker closed its claim; **no current task writer**. HP did not update physical board; the Predator board still needs a later **DONE-PENDING-LEDGER** or equivalent closure by an authorised integrator. Do not start a second writer under the stale READY row. Preserve Fable #402/#408 and user #382 lanes.

## The six Manager product decisions

1. **Approve Option B** — one small read-only comparison card in the existing Annual Review, not a new timeline.
2. **Latest year** — one muted discovery line, no misleading A→B comparison with the same year.
3. **Felling wording** — neutral **felled** for the new card; existing Work column's **thinned** also covers rack-access work and is not a precise treatment classification.
4. **Work outcomes** — two short **DERIVED** recorded-event outcome lines, not newly verified historical tree work; missing or ambiguous source identity must be called out separately. Do not reclassify FellTree as an accepted major intervention.
5. **Defer extra measures** — mean DBH/light, deadwood item count, regeneration density, species basal area excluded from first card.
6. **Defer per-unit/candidate history** — no source rack, group-treatment attribution or candidate major-intervention year; those belong to separately approved D-076/D-074 features.

## Corrections made before approval

The HP document was a good basis, but several points could generate a false player claim or a perpetually failing test. Rather than demand a second short revision handoff, Manager branched from the immutable #409 handoff and amended the four spec files directly:

- **Canopy units (FR-7)**: helper returns a fraction; convert A and B ×100 to percent and the difference ×100 to **percentage points**, following the existing view code. The old wording `StandComparison values unmodified` was incompatible with the proposed percentage-point UI.
- **Species gap (FR-8)**: a union species with `NotRecorded` is absent on **one** side, not necessarily both. Never present a one-sided absence as zero.
- **Truthful event provenance (FR-4/5/10)**: counted outcomes are derived from recorded events, not direct RECORDED counts or verified retained-order targets. Ambiguous/unresolved rows are excluded with nonduplicated warnings; missing/inconsistent retained-order linkage is labelled unverified even when the event outcome is recorded. No inferred tree treatment.
- **Meaningful acceptance (AC-12)**: existing `BuildWork` counts event-derived work and sometimes report-derived regeneration removals. Equal counts on **every** fixture, including malformed/ambiguous records, is unsound. Compare like-for-like categories with complete valid event provenance and explicitly surface unsupported/ambiguous mismatches.
- **State priority (FR-9)**: no-years/latest/ambiguous/missing snapshot ordering is defined so duplicate-year records do not accidentally display a false simpler missing state.
- The four-file spec, plan, tasks and checklist are now marked **APPROVED** consistently. **T-00 COMPLETE**; T-01 remains BLOCKED on qualified cadence release and exact integrated base. All implementation acceptance checkboxes remain unchecked.

## Implementation sequence and dependencies (not yet dispatched)

1. **Wait for the current Predator Fable cadence #402 / exact Unity verifier #408 handoff**; do not launch a competing `AnnualReviewView.cs` owner. Review and accept/repair the source and release its ownership.
2. **T-01**: Manager/integrator chooses an isolated exact base carrying the qualified cadence result **and** offline-approved #406 helper; check actual blob/source and clean branch/worktree/locks. No direct merge into main or protected #382.
3. **T-02/T-03**: a separately claimed worker (HP is well suited) reruns the 53 readout assertions on exact base and builds a small pure mapper + focused fixture, with actual type-bind exit and adversarial source-truth checks. No Unity or UI source ownership is implied until registered.
4. **T-04**: one owner wires one card only in existing `AnnualReviewView.cs`. The update belongs in the Predator UI lane after Fable releases it.
5. **T-05**: separate Unity-qualified worker executes proportionate exact-head Tier 2, including 720p/1080p, old-world/Legacy40/Reference Future and one-year/long-advance comparisons, then hands off for **T-06 independent review and Manager integration decision**. No Windows/Player build before user personally playtests and explicitly authorises it.

**Next logical goal:** a two-machine Fable orchestration plan is prepared separately but not dispatched; Predator owns Unity-qualified UI integration, HP can own offline Forestry management objectives and spec work on disjoint source paths, pending D-074 approval and fresh ownership. No cross-machine claim/Unity lock is inferred from GitHub source state.

**Coordination and dashboard:** Manager to mark #409 HANDED-OFF/CLOSED in authoritative GitHub `active-tasks.md`. Physical board row is worker-reported still READY and must be transitioned by its physical owner as part of next Ops reconciliation (not silently overwritten remotely). Dashboard auto-ingest/receipt and Drive mirror are NOT VERIFIED. User action for this spec review: none.
