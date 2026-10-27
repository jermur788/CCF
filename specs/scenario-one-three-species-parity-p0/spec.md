# Scenario One Three-Species Parity P0 Specification

- **Spec ID:** SPEC-scenario-one-three-species-parity-p0-V1
- **Status:** DRAFT
- **Owner:** Overall Manager (ChatGPT Game Dev)
- **Created:** 2026-10-27
- **Last updated:** 2026-10-27
- **Decision log references:** D-059, D-061, D-062; compatibility constraints D-042, D-046–D-050, D-056–D-058
- **Milestone:** Scenario One — Complete-Loop Three-Species Demo Readiness
- **Related evidence:** `Docs/Project/research-index.md`; `Docs/Research/CCF_Growth_Density_and_Species_Quantitative_Evidence_Composite.md`; `Docs/Research/SitkaGrowthMortality/SitkaGrowthModel1.md`; `Docs/Research/SitkaGrowthMortality/GROWFOR_Technical_Evidence_Extraction_v1.md`; project research sources `CCF Species Evidence Pack v1 — Multi-Species Continuous-Cover Forestry Research Report.pdf` and `CCF Academic Paper Consolidation — Comprehensive Evidence Edition.pdf`
- **Supersedes / superseded by:** none

## Product intent

Before the first external Scenario One demo, Sitka spruce, Oak and Beech must be represented to comparable modelling depth and must behave as genuinely different species where the evidence shows material differences. Comparable depth does **not** require equal empirical precision. Sitka may retain stronger quantitative calibration; Oak and Beech may use labelled British/European transfer, ordinal representation or explicit gameplay calibration where Irish quantitative evidence is insufficient.

This spec turns the accepted requirements in D-059, D-061 and D-062 into implementation authority once approved. It does not itself choose unsupported numerical coefficients.

## Problem

Scenario One currently has mature Sitka-specific growth/density/economic behaviour while Oak and Beech are less complete in several species-sensitive systems. Accepted project direction requires parity before the external demo, but parity must not be achieved by silently copying Sitka assumptions or inventing pseudo-empirical broadleaf equations.

Known current-state facts include:

- the shared individual-tree/ecology architecture already represents species explicitly;
- Oak and Beech can exist as planted individuals and participate in shared ecology;
- existing calibrated Sitka growth/density behaviour is protected;
- D-061 records that an accepted mixed-species density representation is still required;
- D-062 records that Oak and Beech are currently not given a complete timber assortment/economic layer comparable to Sitka;
- the current milestone explicitly states species-parity P0 work has not started.

The first implementation task must re-audit current `main` so later implementation does not rely on stale findings.

## Goals

- Preserve the current calibrated pure-Sitka behaviour exactly unless a later explicit decision authorises a change.
- Add an accepted mixed-species density representation using ordinal species-aware growing-space-demand classes.
- Give Oak and Beech species-appropriate adult, canopy/light/site, regeneration, juvenile and browse behaviour wherever material differences are supported.
- Provide Oak and Beech with an appropriate timber assortment and economic layer rather than reusing Sitka boundaries/prices.
- Make species differences visible and understandable in inspection/teaching surfaces.
- Preserve old saves, deterministic/reference contracts and the frozen Reference Future v1.
- Label every empirical, transfer, ordinal/model-derived and gameplay-calibrated element honestly.

## Non-goals

- No Scenario Two implementation.
- No Douglas fir, Scots pine, birch, alder or other roster expansion.
- No invented Irish Oak/Beech maximum-density or self-thinning equations.
- No density-equivalence coefficient (DEC), density-modification coefficient (DMC) or mixture-productivity bonus in P0.
- No attempt to give Oak/Beech the same numerical evidence precision as Sitka where the evidence does not support it.
- No silent re-baselining of Sitka anchors.
- No change to Multi-Scale Forestry Management V1's current Sitka-adult-only quantitative treatment unless that feature's own spec is explicitly amended later.
- No broad redesign of Scenario One teaching beyond the species-parity information needed for honest player decisions.

## User / player workflows

### Inspect and compare species

The player can inspect Sitka, Oak and Beech and receive species-appropriate information about growth context, competition/canopy role, regeneration/browse vulnerability and timber/economic status without one broadleaf species being presented as Sitka under another name.

### Manage a mixed stand

When management changes stand composition, density pressure and ecological responses account for species identity using the accepted P0 representation. A pure-Sitka stand must continue through the existing calibrated Sitka path exactly.

### Plant and observe broadleaves

Oak and Beech planting, juvenile development, browsing/competition and later adult behaviour remain internally consistent across the same authoritative ecology rather than switching to unrelated special-case simulation systems.

### Harvest and value broadleaves

When Oak or Beech reaches eligible timber states, the game uses a broadleaf-appropriate assortment/value path with evidence/transfer/calibration distinctions. It must not silently use Sitka assortment boundaries or Sitka prices.

## Functional requirements

- **FR-1 — Pure-Sitka preservation.** A stand whose relevant composition is 100% Sitka MUST route through the existing exact calibrated Sitka density/self-thinning implementation and existing Sitka arithmetic. No parity task may silently alter protected Sitka lifecycle/reference anchors.
- **FR-2 — Mixed density representation.** Mixed stands MUST use ordinal species-aware growing-space-demand classes. Oak and Beech class values MUST be presented internally/documentationally as ordinal/transfer/calibrated modelling unless stronger evidence is explicitly accepted. P0 MUST NOT introduce DEC, DMC, a mixture-productivity bonus, or invented Oak/Beech SDI/maximum-density equations.
- **FR-3 — Species-sensitive adult traits.** Adult growth/site/canopy/light/competition behaviour MUST use species-specific behaviour where current evidence shows a material difference. Shared mechanics MAY remain shared when evidence and model intent support sharing. Any Oak/Beech quantitative transfer or calibration MUST be labelled.
- **FR-4 — Regeneration and juvenile traits.** Establishment, juvenile growth/survival, promotion and other species-sensitive regeneration behaviour MUST distinguish Sitka, Oak and Beech where evidence supports material differences. The implementation MUST preserve the single authoritative ecology/cohort/individual architecture.
- **FR-5 — Browsing/protection.** Species/stage browse response and protection-facing information MUST not assume Sitka response for Oak/Beech where accepted evidence supports a difference. Existing shared pressure architecture may remain shared.
- **FR-6 — Timber assortment.** Oak and Beech MUST have an appropriate timber assortment eligibility model before the external demo. It MUST NOT simply reuse Sitka assortment boundaries. Size, quality/form and management history SHOULD affect eligibility/value where the authoritative state can support them without fabricated precision.
- **FR-7 — Economic layer.** Oak and Beech MUST have an explicit economic layer. Values MUST distinguish empirical evidence, transfer and gameplay calibration. A simple species price multiplier on Sitka is insufficient.
- **FR-8 — Player-facing truth.** Inspection, teaching and relevant Work Plan/timber UI MUST explain species-specific outcomes honestly and MUST NOT imply empirical precision or standard practice beyond the evidence.
- **FR-9 — Compatibility.** Existing/missing-field saves, Legacy40 and Reference Future v1 MUST remain compatible. Reference Future v1 MUST remain the frozen historical archive under D-042.
- **FR-10 — Determinism/reference protection.** Any new species-aware state or arithmetic MUST preserve required deterministic contracts and MUST have separately labelled new anchors when a legitimately changed mixed-species model requires them. Existing protected Sitka/reference anchors may not be silently rebaselined.
- **FR-11 — Evidence traceability.** Every new species parameter or ordinal class MUST have a traceable evidence classification: `DIRECT`, `TRANSFER`, `MODEL DERIVATION`, `ORDINAL REPRESENTATION`, `GAMEPLAY CALIBRATION`, `FALLBACK-CONSERVATIVE`, or `UNSUPPORTED — NEEDS DECISION`.
- **FR-12 — Multi-Scale Forestry Management guard.** The existing Multi-Scale Forestry Management V1 quantitative treatment remains Sitka-adult-only unless that spec is separately amended. Parity implementation MUST prevent Oak/Beech from accidentally entering Sitka-only prescription rules.

## Authority / source of truth

- **Accepted product intent:** latest user instruction; D-059, D-061 and D-062; `Docs/Project/current-milestone.md`; `Docs/Project/game-brief.md`.
- **Implementation facts:** inspected repository/current Unity state first; `Docs/Project/unity-project-overview.md` second.
- **Supporting evidence:** `Docs/Project/research-index.md` and the species/growth/ecology/economics sources it indexes. Research is evidence, not decision authority.
- **Spec authority:** when `APPROVED`, this becomes task-level implementation authority for three-species parity P0, subordinate to the project authority hierarchy above.

## Evidence classification

The implementation must distinguish:

- **DIRECT:** directly supported by relevant primary/authoritative evidence;
- **TRANSFER:** British/European or other non-Irish evidence applied with explicit limits;
- **MODEL DERIVATION:** deterministic value/result derived from an accepted model rather than directly measured;
- **ORDINAL REPRESENTATION:** ranked relative behaviour used because a defensible quantitative equation is unavailable;
- **GAMEPLAY CALIBRATION:** chosen to produce clear/viable game behaviour and labelled as such;
- **FALLBACK-CONSERVATIVE:** deliberately cautious default pending better evidence;
- **UNSUPPORTED — NEEDS DECISION:** implementation must stop rather than invent precision.

The accepted Broad & Lynch/GROWFOR evidence remains an aggregate Sitka benchmark, not proof of individual-tree CCF behaviour and not evidence for Oak/Beech equations.

## Save / backward compatibility

The exact schema impact is not yet fixed. The implementation audit must determine whether existing species fields/configuration can carry all P0 behaviour without persisted additions.

If new persisted state is required:

- use the next available save schema/version;
- missing fields must preserve old behaviour or use an explicitly accepted compatibility default;
- do not fabricate historical Oak/Beech state from old saves;
- preserve Legacy40 semantics;
- do not reserialize Reference Future v1 as a migration mechanism;
- add explicit save/load and deterministic continuation tests.

A schema change requires G5 review before implementation.

## Security / write-back

No external security boundary is introduced. Save validation remains a write-back/trust concern: new enum/class/parameter IDs and persisted species state must reject malformed values safely rather than mutating unrelated world state.

## Acceptance criteria

- **AC-1:** Pure-Sitka verification proves byte/arithmetic-equivalent routing through the existing calibrated Sitka density implementation and all protected Sitka/reference anchors required by the implementation packet remain exact.
- **AC-2:** A mixed Sitka/Oak/Beech fixture demonstrates the accepted ordinal growing-space-demand representation without DEC, DMC, mixture-productivity bonus or invented broadleaf maximum-density equations.
- **AC-3:** Oak and Beech adult/regeneration/juvenile/browse behaviour is species-aware for every material difference accepted by the evidence matrix, with no unlabelled Sitka inheritance.
- **AC-4:** Oak and Beech have non-Sitka-copy timber assortment rules and an explicit economic layer with evidence/transfer/calibration labels.
- **AC-5:** Player inspection/teaching communicates material species differences and evidence limits without pseudo-empirical wording.
- **AC-6:** Old saves, Legacy40 and Reference Future v1 pass compatibility gates; no historical archive is rewritten.
- **AC-7:** Integrated Scenario One completion remains reachable and solvent under the qualified parity candidate; material changes to canopy/economy/completion are measured and reviewed rather than silently accepted.
- **AC-8:** Multi-Scale Forestry Management V1 continues to exclude Oak/Beech from its Sitka-only quantitative prescription unless separately amended.
- **AC-9:** Independent review confirms evidence classification and spec conformance before parity is marked VERIFIED.

## Stop conditions

- A task requires changing protected pure-Sitka arithmetic rather than routing around/preserving it.
- A worker proposes an Oak/Beech quantitative equation or coefficient without evidence/classification sufficient for the spec.
- A density design requires DEC, DMC or a mixture-productivity bonus.
- A save-format/history change lacks G5 approval.
- A required player-facing claim would overstate Irish/UK practice or empirical support.
- A change would modify Reference Future v1 rather than preserving it as a frozen archive.
- Current repository inspection materially contradicts a premise of this draft spec.

## Open product decisions

These must be resolved from evidence/calibration and accepted by the Manager before the corresponding production task begins:

1. **Oak/Beech ordinal growing-space-demand class values** within the D-061 architecture.
2. **Oak/Beech adult trait parameter set** where the evidence establishes direction but not an accepted numeric value.
3. **Oak/Beech timber assortment thresholds/quality rules** and which existing form/history signals are sufficiently authoritative to use in P0.
4. **Oak/Beech economic values** where Irish market evidence is incomplete; any selected calibrated values require explicit labels.
5. **Save-schema need** after current-main implementation inspection.

No implementation task may silently resolve these by convenience.