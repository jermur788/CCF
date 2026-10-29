# MFM-06 — Save / provenance foundation — Task Packet

## Task identity

- **Spec:** `specs/multiscale-forestry-management-v1/`
- **Task:** MFM-06 — Save / provenance foundation
- **Parent:** #91 PLAYER-TEST-OVERNIGHT-01
- **Purpose:** add the next available save schema for durable prescription/candidate decisions, prescription provenance, approval/history linkage and generated-work provenance while preserving old saves, Legacy40 and frozen Reference Future v1.
- **Difficulty:** **High** — shared persistence/versioning and compatibility work with protected save/reference/determinism contracts.
- **Default implementation model/provider:** **Claude Sonnet 5.5 Medium / Anthropic** under the user's routing.
- **Mandatory independent review:** **OpenAI 6.1 Sol** before integration because this is high-risk save/reference/determinism work.
- **Suggested branch:** `feature/mfm-06-save-provenance-v1`
- **Suggested worktree:** `/media/jer/Videos/CCF-MFM06`
- **Claim:** do not create/start until the start gate is satisfied. At release create `[RUNNING] MFM-06 — save/provenance foundation` with actual model/provider, branch/worktree and starting HEAD.
- **User action required:** none.

## Start gate — all required

1. MFM-04 and MFM-05 exact current-lineage candidates are **published, Manager-accepted and Unity-compiled/qualified**; local-only `a8d3ccd` / `01a2836` reports are insufficient.
2. #104 S1-C T-05 save v22 closure is fully reviewed, qualified and **published-integrated**, so the actual current save version is known.
3. The GitHub integration ref is refreshed and contains the accepted Rack V2 persistence lineage plus the accepted S1-C save lineage.
4. No other active save/schema/validation writer overlaps this task (including any parity/save lane); refresh live `active-tasks.md` immediately before start.
5. Reference Future v1 and supported legacy/pre-feature save fixtures are inventoried before edits.

If any gate is false, the eventual claim is `[BLOCKED]`; do not reserve a schema number or edit persistence code.

## Save-version rule

Use **the actual next available schema version at dispatch**. The accepted G2 order is Rack v21 → S1-C T-05 v22 → MFM-06 next version. Therefore if and only if the published integrated base is still v22 with no intervening accepted schema owner, MFM-06 takes v23. This packet does **not** reserve `23`; refresh and prove availability first.

Never reuse or collide with another accepted/qualified feature's version.

## Required persisted semantics

Implement the minimum durable state required by the approved MFM spec and the accepted MFM-05 interface. Persist, with stable identity/version semantics:

- stable prescription IDs;
- prescription rule/version identifier;
- source sample / representative rack linkage;
- durable player coverage decisions required by the feature;
- durable per-tree candidate decision/provenance required by the accepted MFM-05 flow, without fabricating biological quality;
- prescription approval state/year where required by the spec;
- work-order provenance linking generated ordinary work to prescription ID/version/source/coverage unit/reason;
- history linkage required by the approved spec.

Do not invent field names or redundant data before inspecting the final published MFM-04/05 interfaces. Persist only what cannot be safely recomputed and what the spec requires.

## Compatibility rules

- missing MFM fields = **no prescription state**;
- do not migrate existing marks/orders into fabricated prescriptions;
- Legacy40 remains unaffected and never enters the MFM path;
- pre-feature Enlarged80 saves remain valid with no fabricated prescriptions;
- frozen Reference Future v1 is never rewritten/rebaselined;
- malformed new MFM save data fails safely **before world mutation**;
- transient proposal visuals should be recomputed where safe rather than redundantly persisted;
- existing Rack/S1-C save semantics remain byte/behaviour compatible except for the explicit new schema fields.

## Hard exclusions

- no proposal/approval behaviour beyond the persistence interfaces required for MFM-07;
- no Work Plan import/commit logic;
- no `TreeMarkType.Fell` proposal state;
- no UI/map/3D work;
- no Oak/Beech quantitative prescription release;
- no hidden quality ranking;
- no migration of old marks/orders into prescription history;
- no Reference Future v1 change;
- no silent protected-hash rebaseline;
- no Windows build.

## Required verification

### Pre-edit fixture audit

Inventory every supported save version/fixture and record:

- version;
- Rack model/state;
- Scenario One / Legacy40 identity;
- Reference Future status;
- whether any field collision/ambiguous default exists.

Stop on ambiguity rather than changing compatibility policy.

### Offline/static

- new-game round-trip;
- pre-feature Enlarged80 load → no prescription state;
- Legacy40 load unchanged/no MFM state;
- MFM state round-trip for each persisted semantic above;
- stable IDs across save/load;
- missing fields default to no prescription;
- malformed/duplicate/invalid new IDs or provenance refuse safely before mutation according to the final schema contract;
- existing marks/orders are not fabricated into MFM provenance;
- version/migration chain from every supported prior schema;
- deterministic serialization/hash-strip behaviour as applicable;
- all existing save-hardening / CanLoad-LoadData parity / cell-index / event-ID gates remain green.

### Unity/runtime

Under the machine-wide lock:

- real save/reload with candidate decisions and prescription state;
- cold-start/load cases from supported old saves;
- refusal/no-world-mutation for malformed new MFM data;
- Legacy40 unchanged;
- Reference Future v1 protected checks;
- import/compile cleanly on exact candidate.

### Independent review — mandatory

OpenAI 6.1 Sol reviews the exact published candidate and migration matrix before Manager integration. Reviewer checks schema ownership, defaults, pre-mutation validation, no fabricated migration, stable provenance, Reference/Legacy protection and test strength.

## Stop conditions

Stop and return to Manager if:

- the next save version is not unambiguously available;
- a legitimate supported fixture is ambiguous under the proposed schema;
- protected compatibility would require rebaselining;
- MFM-04/05 interfaces are not yet stable/published;
- S1-C T-05 v22 is not yet published-integrated;
- another live save/schema writer overlaps the same files.

## Handoff

Publish:

`Docs/Handoffs/MFM-Implementation-06/HANDOFF.md`

Include exact SHA, actual schema version and predecessor chain, field/provenance table, fixture matrix, old-save/Legacy/Reference evidence, hash/determinism evidence, Unity results, independent-review verdict and interfaces exposed to MFM-07/08/09.

Success token:

`PRESCRIPTION SAVE FOUNDATION QUALIFIED — READY FOR MANAGER INTEGRATION REVIEW`

## Copy/paste dispatch line

`Run MFM-06 from Docs/Handoffs/MFM-Implementation-06/TASK-PACKET.md. Refresh published integration, #91, #104, #93, #100 and live active-tasks first. Start only when MFM-04/05 are published+Unity-qualified and S1-C T-05 is published-integrated. Use the actual next available save version; do not pre-reserve 23. Claude Sonnet 5.5 Medium by default, mandatory OpenAI 6.1 Sol independent save/reference review. No Windows build.`
