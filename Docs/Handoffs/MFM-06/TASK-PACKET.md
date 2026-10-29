# MFM-06 — Save / Provenance Foundation

## Task identity

- **Parent umbrella:** #91 `PLAYER-TEST-OVERNIGHT-01`
- **Local execution lane:** #106
- **Implementation claim:** #111
- **Independent review claim:** #112
- **Integration gate claim:** #113
- **Approved authority:** `specs/multiscale-forestry-management-v1/` — MFM-06 and checklist K/J/I6; D-065 remains controlling product authority.
- **Observed umbrella lineage:** `integration/scenario-one-player-test-ready-v1` reported locally at `ac876a64969752a3864034521f42559671264b30` after integration of S1-D T-03, S1-C T-05 v22, MFM-04/05 and PARITY-07.
- **Important visibility caveat:** parent ledger reports `ac876a6` as clean/local but **unpushed** at handoff time. Treat local repository state as implementation truth and publish the exact candidate before claiming remote inspectability.

## Purpose

Add the next available save-schema foundation for Multi-Scale Forestry Management V1 prescription state and durable provenance, without implementing the later proposal/approval UX or changing the authoritative biological model.

This is a high-risk save/persistence boundary. Keep implementation, independent review and integration distinct.

## Model routing

- **Implementation + local/Unity qualification:** OpenAI **6.1 Sol**. Reason: the approved MFM-06 task is routed to SOL/CODEX and this slice touches save/schema compatibility.
- **Independent review:** Claude **Opus 5.5 Low** under claim #112, after the implementation handoff is published. The reviewer must be independent of the implementing thread.
- **Later/lower-risk work:** Sonnet 5.5 Medium remains the programme default unless escalation is justified.

## Start conditions

Before edits:

1. Refresh #91, #106, #111, live `Docs/Project/coordination/active-tasks.md`, this packet, and the approved MFM spec/checklist.
2. Inspect the actual local `integration/scenario-one-player-test-ready-v1` head and confirm it is the reported `ac876a6` or a proven descendant that changes no MFM-06 dependency assumptions.
3. Confirm all relevant worktrees are clean and no active writer owns the same save/persistence files.
4. Confirm the exact current save version on that candidate. Expected from S1-C T-05: **v22**.
5. If current save version is not 22, **STOP** and report the real version/dependency conflict. Do not guess the next schema.
6. Mark #111 `[RUNNING]` with actual model/provider, branch/worktree and starting HEAD before production edits.

If v22 is confirmed, this task owns **v23**.

## Suggested lane

- Branch: `feature/mfm-v1-save-provenance-v23`
- Worktree: `/media/jer/Videos/CCF-MFM-06`
- Root: exact refreshed umbrella candidate / proven docs-only descendant.

Do not work on the parent orchestration branch directly.

## Required persisted contract

Persist only what MFM-06 requires for durable prescription/provenance state:

- stable prescription ID(s);
- deterministic rule/version identity;
- source sample / teaching-rack linkage sufficient for provenance;
- accepted player coverage decisions/state required by the spec;
- prescription approval state and approval year where defined by current accepted domain state;
- work-order provenance fields required so later generated ordinary work can identify the prescription/source;
- durable history linkage required by the approved spec.

Use the smallest schema/domain surface that supports the accepted contract. Do not invent MFM-07 proposal/approval behaviour merely to fill fields.

## Compatibility requirements

The implementation must satisfy the approved checklist, especially K1–K10 and related J/I6 requirements:

- **K1:** use the next available schema after the integrated v22 lineage;
- **K2:** old/missing fields mean **no prescription state**;
- **K3:** never migrate existing marks/orders into fabricated prescriptions;
- **K4:** stable prescription IDs persist;
- **K5:** approval/history/provenance required by the spec persists;
- **K6:** transient proposal visuals/state should be recomputed where safe rather than redundantly persisted;
- **K7:** Legacy40 unaffected;
- **K8:** pre-feature Enlarged80 saves remain valid with no fabricated prescription;
- **K9:** frozen Reference Future remains untouched;
- **K10:** malformed new save data fails safely **before world mutation**.

Also preserve:

- existing v22 S1-C objective/progression history and its validated compatibility semantics;
- management-event ID save hardening;
- Rack V2 persisted geometry/world identity;
- protected hashes/fingerprints unless deliberately versioned by accepted authority — no silent rebaseline;
- deterministic loading and validation ordering.

## Explicit exclusions

Do **not** implement in MFM-06:

- MFM-07 proposal state or approval boundary behaviour;
- Work Plan import/commit semantics beyond the provenance storage foundation;
- MFM-08 map/3D UX;
- MFM-09 teaching-step flow;
- broadleaf quantitative prescription behaviour;
- bulk Crop Tree confirmation;
- new forestry calibration values;
- Reference Future changes;
- a second authoritative stand/biological state.

No Windows build.

## Verification required before handoff

### Static/offline

Add focused deterministic tests covering at minimum:

1. v23 round-trip of each new persisted field;
2. absent MFM fields on v22/pre-feature saves => no prescription state;
3. existing marks/orders remain ordinary existing state and are **not** assigned fabricated prescription IDs/provenance;
4. stable prescription IDs survive repeated save/load;
5. malformed IDs/version/source/coverage/provenance structures are rejected or fail safely before mutation;
6. validation order/no-mutation checks for invalid new data;
7. v22 S1-C history state survives upgrade unchanged;
8. Legacy40/pre-feature Enlarged80 compatibility;
9. Reference Future protected resources/hashes unchanged;
10. deterministic serialized output where the existing save architecture supports deterministic comparison.

Use mutation/negative controls where practical. Do not weaken an existing gate to make v23 pass.

### Compatibility/hash evidence

Record exact before/after evidence for the accepted pre-v23 compatibility contract. In particular:

- identify which historical save bytes/hashes are expected to remain byte-identical and which legitimately differ only due to the explicit v23 schema;
- prove pre-feature/v22 loads do not fabricate MFM state;
- preserve any established hash-strip/comparator rules from T-05 rather than silently rewriting them.

### Unity/local gate

On the exact candidate:

- clean Unity import/compile;
- save/load round-trip for v23;
- pre-v23 load compatibility;
- invalid-v23 pre-mutation rejection/no-mutation observation;
- no Reference/Legacy regression in applicable focused gates;
- exact working tree cleanliness after verification.

Heavy Unity remains serialized under the normal lock.

## Independent review — claim #112

After implementation/qualification is handed off, Claude Opus 5.5 Low independently inspects:

- exact diff and candidate SHA;
- schema-number ownership and migration semantics;
- K1–K10 plus relevant J1/J2/J3/I6 provenance requirements;
- validation-before-mutation ordering;
- whether old marks/orders can accidentally acquire fabricated provenance;
- v22 S1-C history preservation;
- Rack/Legacy/Reference compatibility;
- protected hash/comparator handling;
- whether Unity/save evidence was run on the exact candidate.

Verdict must be `APPROVED`, `NEEDS CHANGES`, or `REJECTED` in:

`Docs/Handoffs/MFM-06/INDEPENDENT-REVIEW.md`

The reviewer does not integrate or modify production under #112.

## Handoff

Publish:

`Docs/Handoffs/MFM-06/HANDOFF.md`

Include:

- actual model/provider;
- branch/worktree;
- starting and ending SHA;
- confirmed starting save version and resulting schema version;
- exact changed files;
- schema/domain-field table;
- compatibility/migration rules;
- static/offline results and negative/mutation evidence;
- Unity/save/no-mutation results;
- protected hash/Reference/Legacy evidence;
- repo/worktree/Unity-lock cleanliness;
- unresolved residuals, if any.

Success token:

`PRESCRIPTION SAVE FOUNDATION QUALIFIED — READY FOR MANAGER INTEGRATION REVIEW`

Do not use the token if independent review is still required for the intended integration gate; instead state implementation/qualification complete and #112 pending.

## Integration — claim #113

Do not integrate until all are true:

- #111 exact implementation candidate is published and clean;
- required local/Unity/save gates pass;
- #112 independent Claude Opus 5.5 Low review returns `APPROVED`;
- no ownership overlap exists;
- current umbrella head is refreshed and integration impact is understood.

After integration, run a bounded post-merge save/compile/no-mutation check before marking MFM-06 integrated/verified. MFM-07 may start only after this foundation is accepted on the umbrella lineage.
