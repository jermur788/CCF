# Manager #424 → #448: four remaining Objective 2 DRAFT corrections

**13 November 2026 | Status: READY / NOT RUNNING | Documentation only | No Objective 2 source or Unity work authorised.**

## Purpose and evidence

The user has endorsed full representative-rack Crop Tree management followed by extrapolated proposals for the other four rack-served areas. [Research task #441](https://github.com/jermur788/CCF/issues/441) delivered a DRAFT specification, [independent #442](https://github.com/jermur788/CCF/issues/442) found 15 issues, and [GPT-6 revision #445](https://github.com/jermur788/CCF/issues/445) corrected most. [Claude Opus #446 independent delta review](https://github.com/jermur788/CCF/blob/docs/claude-446-objective2-delta-review-20261112-v1/Docs/Handoffs/ScenarioOne-PlayerTest-Release-20261112/CLAUDE-446-OBJECTIVE2-445-INDEPENDENT-DELTA-REVIEW-20261112.md) now returns **NEEDS CHANGES (DRAFT quality only)** for four narrow items **R-1 through R-4**. Neither the review nor this packet decides any of **OPEN-1..OPEN-6** or approves implementation. Existing unmeasured per-rack counts and open #418 provenance gate remain unresolved.

**Objective 1 #443 remains FIRST for all local implementation and Unity**; Objective 2 docs-only revision can proceed concurrently without shared source/Editor ownership.

## Assignment and lane

- **Claim:** [#448](https://github.com/jermur788/CCF/issues/448), READY / NOT RUNNING. Before editing, register actual worker/model/provider, exact branch, starting HEAD, **no worktree** on claim as [RUNNING]. On completion publish GitHub result, mark [HANDED-OFF] and close.
- **Difficulty:** Medium — bounded four-documentary repairs requiring precise intervention windows, legacy lesson IDs, and no unapproved product inference.
- **Recommended:** OpenAI GPT-6 High as docs author, later an independent non-author (Claude Opus 5.5 or equivalent) for four-point check.
- **Exclusive writable branch:** `docs/objective2-446-delta-corrections-20261113-v1`, created from the reviewed #445 output, no local worktree/Unity/lock. Confirm live coordination and HEAD before work.
- **Allowed changes:** `Docs/Handoffs/ScenarioOne-PlayerTest-Release-20261112/OBJECTIVE2-FULL-RACK-RESEARCH-AND-EXTRAPOLATION-EVIDENCE.md`, and the four DRAFT files `specs/scenario-one-objective2-whole-rack-sampling-v1/{spec.md,plan.md,tasks.md,checklists/requirements.md}`, plus **one new final handoff** `Docs/Handoffs/ScenarioOne-PlayerTest-Release-20261112/OBJECTIVE2-446-FOUR-FINDINGS-CORRECTION-HANDOFF-20261113.md`. Do not edit the old #445/#446 handoffs, canonical Decision Log, D-065/D-076, main, active-tasks, source, #443/447, saves, OPS, Unity.
- **Review source and checklist:** #446 report **§3 R-1/R-2/R-3/R-4** is binding. Follow its line-level references and legacy source pointers. Everything already marked PASS in #446 should remain untouched unless cross-references require a bounded change.

## Four corrections

**R-1 (B1, substantive):** Define the destination **pre-intervention basal area denominator's state and year**, which earlier executed removals count toward the cumulative cap, the campaign/intervention window for source N / destination N+1 or deferred M, and **re-evaluation at actual resolution/annual Advance** (not just Work Plan approval). Discuss the consequence of growth/mortality and already-executed manual removals, per destination tile/rack and local neighbourhood. It is acceptable to present well-defined candidate alternatives linked to OPEN-3 and OPEN-6; **do not ratify** a cap statistic, threshold, campaign duration or previously proposed tile calibration by fiat. Add a negative acceptance test for between-approval-and-execution BA change; retain authentic release-only source data, zero-release policy and no automatic Fell marks.

**R-2 (B2, substantive):** Correct the actual old→proposed Objectives 2–5/extension mapping. Existing C-01 code contains `ScenarioOneTeachingSequence.ExtensionId = "extend-treatment"`, presented between response and recruitment and offered only when `Current(f) == 5`, with `LearningObjectivesView.Sequence.ExtensionApplies/ExtensionApproved` (approval of any prescription). Do **not** label all four-target proposal steps "no old equivalent." Explain old Enlarged80 and saved-world compatibility, extension state reuse/retirement and downstream presented numbering of recruitment/establishment/transformation. Label practice keys as *related* rather than exact one-to-one persisted lesson IDs; include the original habitat/deadwood choice and `deadwood.*` keys. Protect D-066 one-way legacy completion latch; no schema invention.

**R-3 (B2, substantive):** For undecided **OPEN-6 option B** (destinations deferred to later year M), distinguish (B-i) reusing year-N derived rule with explicit source staleness/comparability checks at M and (B-ii) performing a fresh source demonstration at M, which may constitute a separate D-076 cycle rather than later execution of the original campaign. Clarify the consequence of D-077 Skip ineligibility while a teaching stage remains incomplete (the user may need explicit Advance Year actions). These remain alternatives, **do not choose A/B/B-i/B-ii** or secretly promise both workflows as one.

**R-4 (M6, terminology):** Correct two lingering “D-073 test candidate” labels in `plan.md` §B and evidence report §2. D-073 is an accepted decision governing annual outcomes/history; identify the protected Scenario One personal Editor test via Manager #424 / actual user test context instead.

## Completion and review

Produce a four-row `finding → edited file/section → resolution → remaining OPEN decision` table, explicit diff paths, actual registered model and source/finished HEAD (internal audit), no-source/Unity claim and link the exact final handoff. Keep all test boxes **unchecked** unless native evidence actually exists (none authorised here), avoid fabricated rack/workload measurements, and retain #418 as separate. When #448 is HANDED-OFF, Manager #424 reviews and dispatches **one short independent non-author R-1..R-4 delta check**. Only if that is satisfactory should #425 and the user be presented the **six OPEN product decisions** for ratification. Formal spec approval and implementation are later, and **Objective 1 source lane must hand off first**.

**User action:** none for this correction. No Windows build. Dashboard/Drive mirror status unverified.

## Copy-paste dispatch

> Take CCF #448 to correct four remaining Objective 2 DRAFT research/spec findings in GitHub documentation only. Full binding handoff: https://github.com/jermur788/CCF/blob/docs/manager-424-objective2-446-disposition-20261113-v1/Docs/Handoffs/ScenarioOne-PlayerTest-Release-20261112/MANAGER-424-OBJECTIVE2-446-FOUR-FINDINGS-CORRECTION-PACKET-20261113.md . Register actual model, docs-only branch and starting HEAD first. Fix #446 R-1..R-4, keep OPEN-1..OPEN-6 undecided and tests unverified, publish the specified GitHub handoff and mark #448 HANDED-OFF. Do not modify source, Unity, Objective 1, coordination or approved specs.
