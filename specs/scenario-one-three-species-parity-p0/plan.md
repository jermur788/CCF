# Scenario One Three-Species Parity P0 Implementation Plan

- **Spec ID:** SPEC-scenario-one-three-species-parity-p0-V1
- **Spec status:** DRAFT
- **Plan owner:** Overall Manager (ChatGPT Game Dev)
- **Created:** 2026-10-27
- **Last updated:** 2026-10-27

## Architecture / current-state findings

Verified project-level findings from the current canonical overview/decisions:

- Species identity already exists in the shared individual-tree/ecology architecture.
- Oak and Beech can exist as planted individuals and participate in shared regeneration/juvenile systems.
- Existing Sitka growth/density behaviour is calibrated and protected by deterministic/reference gates.
- Historical integrated ecology records Sitka-specific competition/growth parameters and lower Oak/Beech values, but those historical constants must be re-audited on current `main` before implementation.
- D-061 requires a new mixed-species density representation using ordinal species-aware growing-space-demand classes while preserving the exact pure-Sitka path.
- D-062 records the current broadleaf timber/economic layer as incomplete relative to Sitka.
- Current milestone states species-parity P0 production work has not started.

Because implementation has continued since the original parity audit, T-01 performs a fresh code-level audit before any production edit.

## Proposed technical approach

Use the existing shared simulation architecture and add species-aware policy/data at the narrowest authoritative layers rather than creating parallel broadleaf simulation systems.

1. Preserve a hard pure-Sitka fast path into the existing calibrated Sitka density/self-thinning implementation.
2. For mixed stands, introduce a small explicit ordinal growing-space-demand classification for Sitka/Oak/Beech. Apply it only to the mixed density signal required by D-061; do not invent Oak/Beech SDI equations.
3. Audit all species-sensitive adult, canopy/light/site, regeneration, juvenile and browse traits. Keep genuinely shared mechanics shared; replace inappropriate Sitka defaults with evidence-backed/transfer/ordinal/calibrated species values.
4. Add broadleaf timber assortment as its own eligibility specification rather than species multipliers on Sitka products.
5. Add broadleaf economic values through explicit species/assortment records with evidence classification.
6. Update inspection/teaching copy so the player sees real species distinctions and evidence limits.
7. Add save fields only where an implementation requirement cannot be derived from existing authoritative state.

## Integration boundaries

May touch, after spec approval and task assignment:

- species trait/configuration definitions;
- mixed-density/self-thinning routing;
- species-specific regeneration/juvenile/browse policy;
- timber assortment/economy data and eligibility;
- inspection/teaching surfaces required for parity;
- focused verification tooling and save schema if separately approved.

Must not touch without separate authority:

- Rack V2 planning/runtime/field-marking implementation;
- Multi-Scale Forestry Management prescription behaviour except its species exclusion guard;
- Reference Future v1 archive bytes;
- Scenario Two roster/infrastructure;
- storm activation/default policy;
- unrelated UI redesign;
- Windows build/distribution.

## Affected systems / files

Exact files are deliberately deferred to T-01 current-main audit. Expected areas include:

- `ForestTree` / species definitions;
- `ForestEcologyController` and species growth/density configuration;
- regeneration/juvenile rules;
- browsing/protection rules;
- Scenario One inspection UI;
- timber assortment/yield/economy code and data;
- save validation if new persisted state is accepted;
- focused verification harnesses.

The implementation packet must replace this expected list with exact current paths before writing.

## Ownership / concurrency considerations

- One primary writer per species-parity subsystem/file set.
- Do not overlap active Rack V2 lanes.
- Multi-Scale Forestry Management production remains gated independently; parity may run in parallel only where files/subsystems do not overlap.
- Serialized Unity assets are single-writer. Prefer code/data definitions over unnecessary serialized edits.
- Heavy Unity runs require the machine-wide Unity lock.
- DeepSeek is suitable for bounded implementation tasks; Claude should provide independent review for high-risk ecology/calibration/economy tasks. MiMo may assist read-only inspection/evidence mapping.

## Migration / save implications

T-01 must determine whether parity can be expressed entirely through code/data derived from existing species/state. Prefer no schema change where persisted state is unnecessary.

If a new schema is required:

- stop for G5 approval;
- use the next available schema version on the integration lineage;
- missing fields preserve accepted old-save semantics;
- do not fabricate historical broadleaf state;
- Legacy40 and Reference Future v1 remain compatible;
- add malformed-data validation and deterministic continuation gates.

## Verification strategy

- **Implementation correctness:** focused static/offline tests during each task; representative Unity runtime fixtures for mixed and pure stands; save/load only when persistence is relevant.
- **Spec conformance:** requirements checklist plus independent review of evidence classification, pure-Sitka preservation, broadleaf non-Sitka inheritance and player-facing wording.
- **Verification tier:** Tier 1 during implementation; Tier 2 per bounded feature slice; Tier 3 only once parity is integrated into the full Scenario One candidate.

Mandatory gates:

- G2 acceptance for species-sensitive behaviour;
- G3 measurement integrity for density/calibration/expected-value changes;
- G4 before shared integration/canonical acceptance;
- G5 for any save/history/product-scope change.

Protected behaviour must not be rebaselined merely to obtain green tests.

## Risks

- Hidden Sitka defaults may exist in systems not captured by the old audit.
- Broadleaf evidence may support relative/ordinal behaviour but not precise coefficients.
- Timber economics may be the largest false-precision risk because Irish broadleaf price evidence is incomplete.
- Mixed-density changes could accidentally alter pure-Sitka arithmetic.
- Broadleaf changes may affect long-run canopy/completion/economy enough to require gameplay calibration review.
- Save state may currently encode species-related assumptions indirectly; T-01 must identify these before editing.

## Implementation sequence

1. **T-01 Current-main species-parity audit** — inspect all species-sensitive systems/files and produce an exact implementation inventory.
2. **T-02 Evidence/parameter closure** — build the Oak/Beech evidence matrix and resolve the open values through Manager acceptance before production coding that depends on them.
3. **T-03 Mixed density representation** — implement ordinal species-aware mixed density with exact pure-Sitka preservation.
4. **T-04 Adult/canopy/site parity** — implement accepted adult species differences and associated inspection diagnostics.
5. **T-05 Regeneration/juvenile/browse parity** — close accepted species-sensitive early-life behaviour gaps.
6. **T-06 Timber assortment and economics** — implement Oak/Beech product eligibility and values with explicit evidence labels.
7. **T-07 Player-facing parity** — inspection/teaching copy and UI required to explain species behaviour honestly.
8. **T-08 Save/determinism compatibility** — land any approved persistence changes and compatibility tests; otherwise explicitly verify no schema change is needed.
9. **T-09 Integrated parity qualification** — combined mixed/pure fixtures, Scenario One completion/economy, protected anchors, rendered review and independent spec-conformance review.

No T-03+ production task may be dispatched while the spec is DRAFT or while T-02 has unresolved values required by that task.