# #251 — Overall Manager disposition: proceed with conditional Option A specification, not implementation

**Date:** 2026-11-04. **Verdict:** **APPROVED AS CORRECTED READ-ONLY RESEARCH; OPTION A SELECTED FOR DRAFT SPEC/QUALIFICATION ONLY.** **No production approval**, no changed rack/MFM validity in the current game and no personal playtest or Windows-build acceptance.

## Evidence inspected

- [#251 MFM topology options handoff](MFM-TOPOLOGY-OPTIONS-HANDOFF.md), independently reviewed and included with the live V2 ledger through **merged, docs-only [PR #252](https://github.com/jermur788/CCF/pull/252)**.
- The parent [#250 STOP](MANAGER-FIVE-RACK-REVIEW-20261103.md), [#248 measurement](UNITY-GEOMETRY-HANDOFF.md), accepted Rack V2/D-064 and MFM V1/D-065.
- Current player-test candidate source: `ScenarioOneRackSampleGeometry.cs` (`Derive`), `ScenarioOneCoverage.cs` (`Input.Geometry`, masks and classification), plus #251's line-referenced inspection of Confirm, `UpdateRack`, rack editor, save validation, prescription/version and archive-hash paths.
- Browser Manager inspected the published research and source snippets; no local Unity or raw-harness execution was performed by Manager.

The first #251 draft received **MATERIAL ERROR** in independent review: **40 claims = 28 PASS + 8 PASS-WITH-CORRECTION + 4 FAIL**. The four failures (Confirm-only gate incomplete; hash stability inferred incorrectly from absence of new fields; stable IDs confused with stable unit meanings; curved-boundary argument overstated) and corrections are transparently retained in Appendix C. This is credible corrected research, **not a 40/40 qualification PASS**.

## Accepted findings

1. The specific five-rack 3+2, 45° reference contains **two mirrored orientation families 90° apart**; the global common normal follows a length difference of approximately 12.304 m out of 219.588 m total, with cross-group ordering overlaps. Current `Derive` returns `Valid=false`. This is **a particular topology failure**, not proof every branched arrangement fails.
2. **Existing lifecycle hazard independent of A/B/C:** Rack Confirm, authoritative confirmed-plan `UpdateRack`, and persisted rack validation do not test whether MFM rack-sample derivation is valid. An invalid plan may reach executed/locked work, potentially preventing a straightforward later repair. No end-to-end irreversible-user reproduction has been demonstrated; record as source-backed risk, not proven catastrophic failure.
3. Option **A** adds a **side/branch-local derivation** while keeping legacy `Derive` unchanged and selected for plans it already accepts. Option **B** generalizes to graph/curved partitions at materially higher specification and verification cost. Option **C** preserves the existing parallel model but cannot meet the 45° five-rack target on this studied spine; its near-56° constraint is an approximation, not a complete layout proof.
4. A cannot safely consist of two independent strip passes. The study calculated **1,575 m²** of overlapping side ownership without disambiguation, and proposes a deterministic **single-owner** partition. It still lacks actual mask-adjusted calibration, rendering, old-save round trips and performance evidence.
5. The sample-to-both-sides idea is permissible under the **current three scalar comparability checks**, but its *forestry/teaching validity* has not been shown. The proposed reference has substantial area far from any rack (study-derived **2,246 m²** over 9 m, including **830 m²** over 20 m). The existing 9 m teaching half-width is **not** an accepted rack-access reach limit.
6. No new serialized field for A would avoid a **schema-layout** change but **does not guarantee unchanged hashes or unit meaning**. Existing rules/calibration text and persisted prescriptions require version-aware, load-safe compatibility checks. Legacy40 and frozen Reference Future v1 remain protected.

## Manager decisions — subordinate to approved Rack V2/MFM specs

**A: CONDITIONAL DRAFT DIRECTION.** Draft an **additive, deterministic branch-local geometry/coverage model** as the first option to qualify. The legacy parallel `Derive` must remain byte-identical and authoritative for plans it already accepts; do not select the new model opportunistically to improve poor fit. Keep a constrained parallel Rack V2 fallback if the new model fails calibration. Do not undertake the graph/persisted-network option at five-rack scope without a new product decision.

**Validity must guard the whole lifecycle, not just Confirm.** DRAFT policy: allow poor/disconnected/ambiguous *proposals* with clear consequences, but **do not confirm** geometry neither accepted derivation can support. Every authoritative mutation of a confirmed plan must be prevalidated atomically **or** force a proper reopen-and-reconfirm workflow, with existing executed work protected. For previously saved, confirmed invalid plans, prefer **load successfully, expose the error, suspend affected new MFM proposals/work and enable safe repair without erasing executed history**; exact legacy-safe behaviour needs tests. Never reject all old save loads or silently rewrite rack geometry to turn a red plan green.

**One player sample:** use one demonstrated 3D teaching sample for both branch groups as an **initial research hypothesis only**. Compare conservatively by the existing stand metrics; show explicitly which group supplied the sample, and require independent forestry/gameplay calibration before making it default. A second sample remains a potential future feature, not a hidden requirement.

**Group tolerance/limit and reach policy are OPEN, not arbitrary constants.** A two-family cap and deterministic tie-break are reasonable investigation bounds, but the exact angle tolerance, chaining behaviour, remainder ownership and non-overlap proofs need offline tests. **Do not approve unbounded reach**: cells/trees outside verified comparable-access range must conservatively remain NotCovered/Manual until a reach rule is approved. Neither 9 m teaching half-width nor the measured 41.18 m maximum is automatically a valid reach threshold.

**Separate Rack V2 design blockers:** legal spine entrance trees, yard/workbench bypass, corridor overlaps, practical turning, complete layout and honest proposal-vs-cleared mask timing need independent specification/qualification. D-064/D-065 remain controlling. No automatic Fell marking; Work Plan approval remains the commitment gate.

## Next work and stop conditions

**#253** — DPC-orchestrated, source/artefact-backed **offline feasibility and qualification plan** for Option A and lifecycle validity: establish robust single-owner area partition, sample/segments, mask-adjusted reach and coverage sensitivity, group-angle edge cases and legacy/branched version/round-trip invariants; independently review. It is **not production implementation** and should require no Unity. Report whether A's apparent advantage survives conservative access/reach constraints. Publish a durable GitHub handoff and V2 orchestration ledger events; Manager then refines the four-file DRAFT spec and seeks any product choices needed before implementation approval.

Keep the original personal-playtest candidate unchanged. Do not alter Rack V2/MFM production, tests, saves or Unity; no Windows/Player build until the user finishes personal playtest and explicitly authorizes it afterward.