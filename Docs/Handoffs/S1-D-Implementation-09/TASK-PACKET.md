# S1-D T-09 — Independent spec-conformance review and hand-off — Task Packet

## Task identity

- **Spec:** `specs/scenario-one-repeated-interventions-v1/`
- **Task:** T-09 — Independent spec-conformance review and hand-off
- **Parent:** #91 PLAYER-TEST-OVERNIGHT-01
- **Purpose:** independently close the S1-D requirements checklist after the integrated multi-cycle witness, confirming the repeated-management feature is non-prescriptive, compatible and free of hidden automation/unlabelled coefficient changes.
- **Difficulty:** **Medium–High** — review-only, but requires cross-system evidence synthesis across T-02..T-08 and protected contracts.
- **Default reviewer:** **Claude Sonnet 5.5 Medium / Anthropic**, independent from the recommended OpenAI 6.1 Sol T-08 executor.
- **Escalation:** Claude Opus 5.5 Low only with a recorded reason such as contradictory evidence, unresolved architecture ambiguity or a material protected-contract question.
- **Mode:** read-only/browser/repository evidence review; no implementation.
- **Suggested review branch if a handoff doc is authored locally:** `review/s1-d-spec-conformance-v1`
- **Suggested worktree:** `/media/jer/Videos/CCF-S1D-T09` or browser-only if every required artifact is published.
- **Claim:** create only after T-08 publishes PASS: `[RUNNING] S1-D T-09 — independent spec-conformance review` with reviewer/provider and exact candidate SHA.
- **User action required:** none unless the review discovers a genuine unresolved product decision.

## Hard start gate

Do not start until:

1. T-08 has published `S1-D T-08 MULTI-CYCLE WITNESS PASS — READY FOR INDEPENDENT SPEC REVIEW` on an exact GitHub-resolvable candidate.
2. T-02..T-06 and their accepted handoffs/reviews are published and traceable from that candidate.
3. T-04 has its independent guide-change review.
4. T-05 rendered evidence is published.
5. T-06 independent save/load-path review and Unity qualification are traceable.
6. The current `specs/scenario-one-repeated-interventions-v1/checklists/requirements.md` and all Manager decisions/amendments are refreshed.

If any mandatory evidence is missing, verdict is `NEEDS CHANGES` with the exact evidence gap; do not infer PASS.

## Review scope

Review only. No source/verification edits except the review/handoff Markdown itself.

Confirm every applicable requirement/acceptance criterion and specifically test the following review themes:

### Repeated-management semantics

- second, third and later cycles use the ordinary management/work-order machinery rather than a hidden cycle state machine;
- no fixed intervention interval is introduced;
- readiness remains advisory/descriptive rather than a treatment recommendation;
- completion remains reachable and management remains possible after completion.

### Teaching / guide

- the teaching-footprint cap is hard only within its owning active lesson/sample condition and cannot lifetime-block later Approve/Advance;
- the guide is monotone only where faithful persisted history proves completion;
- no new persisted teaching authority, `PlayerPrefs` authority or `[J]/[K]` persistence was introduced;
- stable teaching IDs are not renamed/reused.

### Previous / proposed / history

- S1-D reuses the authoritative S1-C history surface;
- no duplicate history database/query/UI exists;
- Approved / Pending new / Pending carried-over presentation is derived from authoritative order/history facts;
- no fabricated Crop Tree designation date or unsupported causal history is shown.

### Save / chronology integrity

- event IDs are positive/unique and invalid saves refuse before mutation;
- no repair/renumbering/reinterpretation of ambiguous history occurs;
- old saves/Legacy40/Reference Future v1 remain within accepted compatibility policy;
- no S1-D V1 schema field was added.

### No hidden automation / coefficients

Confirm AC-10 / AC-11 / AC-13 and related requirements:

- no hidden treatment automation replaces player judgement;
- no duplicate derived-history authority or competing state store was introduced;
- no unlabelled coefficient/threshold/economy/ecology change was smuggled into S1-D;
- N3 completion-rule normalised hash/protected objective rules remain unchanged unless an explicit accepted decision says otherwise.

### Verification quality

- T-08 witness exercised three real cycles, idle years, stale pending work, save/load, completion on the third cycle and continued management afterward;
- D-057 solvent witness is evidence-backed and no coefficients were changed merely to pass;
- mandatory narrow regressions/protected anchors are green or have an accepted, explicit disposition;
- verification changes did not weaken assertions/thresholds.

## Evidence matrix

Produce a table covering each mandatory FR/AC/RD/checklist item with:

- requirement;
- exact implementation evidence SHA/file;
- exact verification evidence/handoff;
- review finding: PASS / GAP / CONFLICT;
- notes on evidence classification where relevant.

A checklist line with no concrete evidence is a GAP, not an assumed PASS.

## Verdicts

Use exactly one:

- **APPROVED** — all mandatory requirements evidenced; no unresolved conflict.
- **NEEDS CHANGES** — bounded implementation/evidence gaps remain and can be returned without changing accepted product direction.
- **REJECTED** — candidate conflicts with accepted S1-D product intent or protected contracts in a material way.

Do not promote spec status yourself unless the Manager assigns that action; the review supplies the verdict for Manager promotion.

## Stop / escalation conditions

Escalate to Claude Opus 5.5 Low with a recorded reason only if:

- two accepted evidence sources materially conflict;
- a protected save/reference/determinism interpretation is ambiguous;
- implementation appears to require a product decision outside the approved spec;
- the review cannot distinguish a real defect from stale evidence at Sonnet Medium.

If a genuine product decision is required, stop and return it to Overall Manager/user; do not invent resolution.

## Handoff

Publish:

`Docs/Handoffs/S1-D-Implementation-09/HANDOFF.md`

Include exact reviewed candidate SHA, complete requirements matrix, evidence gaps/conflicts, independence statement, final verdict and Manager next action.

Success token:

`S1-D T-09 INDEPENDENT REVIEW APPROVED — READY FOR MANAGER SPEC VERIFICATION`

## Copy/paste dispatch line

`Run S1-D T-09 from Docs/Handoffs/S1-D-Implementation-09/TASK-PACKET.md only after T-08 publishes PASS on an exact GitHub-resolvable candidate. Reviewer: Claude Sonnet 5.5 Medium / Anthropic, independent from the T-08 executor; escalate to Opus 5.5 Low only with a recorded reason. Review only: fully evidence the S1-D checklist and issue APPROVED / NEEDS CHANGES / REJECTED. No implementation, no Windows build.`
