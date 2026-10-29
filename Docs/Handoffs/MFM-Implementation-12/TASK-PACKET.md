# MFM-12 — Integrated Verification and Independent Review

**Parent:** #91  
**Spec:** `specs/multiscale-forestry-management-v1/` MFM-12  
**Status:** PREPARED / BLOCKED on MFM-11 and prerequisite feature qualifications  
**Purpose:** qualify Multi-Scale Forestry Management V1 end-to-end against the approved requirements checklist on one exact integrated Scenario One candidate.

## Difficulty and routing

- **Difficulty:** High — end-to-end save/map/3D/Work Plan/completion/rendered/spec-conformance gate.
- **Preferred T3 executor:** Claude Sonnet 5.5 Medium / Anthropic; escalate to Claude Opus 5.5 Low only if a difficult integration diagnosis warrants it.
- **Independent architecture/spec reviewer:** OpenAI 6.1 Sol, cross-provider and independent from the dominant final fixes.

## Start gate / base

Start only when:

- MFM-06…11 are accepted on the active lineage;
- PARITY-09 is approved;
- S1-C and S1-D feature-level qualifications required by the complete loop are green;
- one exact GitHub-resolvable integration SHA contains the intended candidate;
- no active writer owns MFM/save/Work Plan/teaching files.

Suggested branch/worktree: `qual/mfm-v1-integrated-v1` / `/media/jer/Videos/CCF-MFM-12`.

Create claims only when all gates are true.

## Required end-to-end flow

Exercise the complete V1 chain on the exact candidate:

`walk/sample → demonstrate judgement → derive prescription → inspect fit → review coverage → review/confirm candidates in 3D → review proposals → required extrapolated-unit spot-check → approve prescription → ordinary Work Plan → approve work → execute → advance year → Annual Review/reinspect`

Evidence must prove:

- no second authoritative biological state;
- persisted Rack V2 geometry drives sample/coverage;
- sample teaching protections remain intact;
- deterministic prescription and fit disagreement visibility;
- spacing-only candidates require individual player judgement;
- proposals are not Fell marks and cannot leak into Work Plan before prescription approval;
- prescription approval creates pending ordinary work only;
- Work Plan approval remains the commitment gate;
- manual state/management alternative remains authoritative;
- durable v23 prescription/work provenance/history linkage survives save/load;
- Legacy40/pre-feature saves and frozen Reference Future remain valid;
- malformed new save data rejects before mutation;
- Sitka-only quantitative species guard holds;
- extension teaching IDs and guide monotonicity remain stable across save/load;
- calibration-impact gate is green;
- full Scenario One completion remains reachable;
- no hidden automation replaces player judgement.

## Rendered / interaction evidence

Capture material map/3D/candidate/proposal/Work Plan/review states at 1280×720 and 1920×1080. Candidate/proposal presentation must remain distinct from Red Fell, Blue Crop Tree and Orange Rack marks.

At least one candidate confirm/reject and required extrapolated-unit spot-check must be grounded in actual 3D interaction evidence; browser/static evidence alone cannot satisfy this.

## Checklist and stop rule

Complete every applicable item in `specs/multiscale-forestry-management-v1/checklists/requirements.md` with evidence references.

Any failed mandatory item => `NEEDS CHANGES`; do not weaken tests or modify production inside qualification.

No Windows build.

## Handoff / independent review

Publish `Docs/Handoffs/MFM-Implementation-12/HANDOFF.md` with exact candidate SHA, checklist evidence table, save/rendered/interaction/completion gates and deviations.

Independent OpenAI 6.1 Sol review publishes `Docs/Handoffs/MFM-Implementation-12/INDEPENDENT-REVIEW.md` with `APPROVED / NEEDS CHANGES / REJECTED`.

Only after complete evidence may the Manager emit:

`MULTI-SCALE FORESTRY MANAGEMENT V1 — INTEGRATED / READY FOR USER PLAYTEST`

This is not user acceptance and does not authorise a Windows build.

## Copy/paste dispatch line

`Run MFM-12 from Docs/Handoffs/MFM-Implementation-12/TASK-PACKET.md only after MFM-06..11, PARITY-09 and required S1-C/S1-D feature gates are accepted on one exact published integration candidate. Use Claude Sonnet 5.5 Medium for end-to-end T3 qualification and OpenAI 6.1 Sol for independent cross-provider spec review. Execute the full sample→prescription→candidate→proposal→Work Plan→execution→review chain, save/old-save/Reference gates, 3D spot-checks, completion and both rendered resolutions. Do not fix production inside qualification. No Windows build.`

**User action required now:** none; blocked on prerequisite feature gates.
