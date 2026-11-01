# MFM-10 Browser / Static Guard Handoff — 2026-11-01

**Status:** **BROWSER PHASE COMPLETE — READY FOR #213 RESIDUAL OFFLINE QUALIFICATION**  
**Browser/static claim:** #212  
**Local residual claim:** #213  
**Parent:** #91 / browser Manager #200 / local orchestration #203  
**Exact inspected candidate:** `6784ab17cac922a16815f4898a30737482101018`  
**Published integration branch:** `integration/scenario-one-player-test-ready-v1`  
**Final MFM-10 token remains withheld:** `SPECIES GUARD QUALIFIED` may be issued only after #213's residual executable evidence is accepted.

## Purpose and browser boundary

This handoff completes the browser/GitHub-only portion of MFM-10 requested by `BROWSER-SPLIT-HANDOFF-20261101.md`: inspect the exact candidate, accepted authority, current Sitka-only MFM guards, Oak/Beech parity boundaries and existing verification assets; classify every MFM-10 requirement; and leave #213 only the smallest executable matrix that static evidence cannot prove.

No local worktree was used. No executable offline test was run. No Unity/Editor/runtime work was performed. No production code, integration state, test baseline or Windows build was changed. Research Source Consolidation #194 remains **NO ACTION**.

## Exact candidate / lineage result

GitHub branch inspection confirms `integration/scenario-one-player-test-ready-v1` still points exactly at:

`6784ab17cac922a16815f4898a30737482101018`

The MFM-09 integration delta immediately preceding this candidate was also inspected. Relative to its accepted pre-MFM-09 base, the production changes are confined to teaching / Work Plan UI surfaces (`LearningObjectivesView*`, `ScenarioOneTeachingSequence`, `WorkPlanView`) plus MFM-09 verification tooling. It does **not** modify `ScenarioOneCoverage`, `ScenarioOnePrescription`, `ScenarioOneCropTreeCandidates`, `ScenarioOnePrescriptionCalibration`, mixed-density production logic, species assets, save schema/validator, or the MFM-07 approval core. Therefore MFM-09 did not broaden MFM proposal species scope.

## Authority inspected

Primary accepted authority on the exact candidate:

- `specs/multiscale-forestry-management-v1/spec.md`
- `specs/multiscale-forestry-management-v1/tasks.md` — MFM-10
- `specs/multiscale-forestry-management-v1/checklists/requirements.md`
- `specs/scenario-one-three-species-parity-p0/spec.md`

Relevant accepted boundaries:

1. MFM V1 must not silently run Sitka-only proposal calibration against Oak/Beech.
2. When the quantitative calibration is unsupported, MFM may refuse auto-proposal for that unsupported subset rather than inventing broadleaf coefficients.
3. Comparable Sitka/Oak/Beech modelling depth does **not** mean equal empirical precision.
4. Accepted mixed density is ordinal species-aware growing-space occupancy; no DEC, DMC, mixture-productivity bonus or continuous broadleaf self-thinning law is allowed.
5. Pure-Sitka protected arithmetic/anchors remain exact.
6. Proposals are non-committing; prescription approval creates Pending ordinary work; Work Plan approval remains the commitment gate.

Supporting project evidence is consistent with this boundary: the accepted species evidence rejects a universal Sitka-derived density ceiling for Oak/Beech and preserves pure-Sitka compatibility rather than generalising Sitka coefficients.

## Production species-guard inspection

### `ScenarioOnePrescriptionCalibration.cs`

The V1 calibration explicitly identifies its values as **gameplay calibration** measured on the deterministic Scenario One stand/rack layout, not a forestry standard or cross-species coefficient set. No Oak/Beech quantitative calibration exists here.

### `ScenarioOneCoverage.cs`

This is the first fail-closed species boundary.

- V1 quantitative rules are documented as calibrated for **Sitka adults only**.
- Source/sample coverage fails if the sample contains a living non-Sitka adult.
- A target area containing any living non-Sitka adult receives the `NonSitkaAdult` exclusion reason.
- Such a target is `Excluded`, not `Covered`, before the MFM-03 quantitative fit/calibration is used to admit it for proposal generation.

Result: a mixed Sitka/Oak/Beech target area is not silently treated as a Sitka-calibrated area.

### `ScenarioOnePrescription.cs`

This is the second fail-closed boundary.

- V1 prescription derivation states that it removes only Sitka adults.
- A non-Sitka adult in the sample stops derivation with `SampleHasNonSitka` / “V1 is Sitka-only.”
- Release proposal generation only works on `Covered` areas.
- A confirmed Crop Tree that is not Sitka is returned as `NotSitka` and receives no release proposal.
- A proposed competitor must itself be Sitka; non-Sitka competitors are skipped.
- Proposal output remains plain proposal data rather than marks or work orders.

### `ScenarioOneCropTreeCandidates.cs`

Candidate suggestions are generated only from `Covered` areas and additionally require `IsSitka`.

Potential spec tension reviewed: the MFM spec says no species-specific candidate rule should be introduced unless parity authority justifies it. In the current reachable V1 flow, however, coverage cannot be `Covered` when the area contains any living non-Sitka adult. The candidate `IsSitka` predicate therefore acts as redundant defence-in-depth under the already-authorised **refuse unsupported calibration** path; it does not narrow a parity-supported mixed `Covered` set. I do **not** classify this as a current code/spec mismatch. It must be revisited if a later spec allows multi-species MFM quantitative coverage rather than whole-area refusal.

## Existing accepted / verification evidence inventory

### MFM proposal / approval invariants already accepted

`Docs/Handoffs/MFM-07/INTEGRATION-HANDOFF.md` records accepted exact-candidate-lineage evidence that:

- proposals are transient plain data and do not create Fell marks, work orders or persisted proposal state;
- manual Fell marks, open orders and confirmed Crop Tree state win conflicts and are surfaced rather than overwritten;
- prescription approval creates **Pending** ordinary felling work only;
- Work Plan approval remains the separate commitment gate;
- stale/invalid links fail closed before mutation and rollback protects against partial writes;
- the existing save/provenance fields are reused without a schema/validator rewrite;
- protected hash evidence and interaction/save gates passed on the MFM-07 integration candidate.

Those behaviours are inherited by `6784ab17...`; the inspected MFM-09 delta does not modify their production files. They therefore do not need to be rerun merely to answer MFM-10 species scope. They remain part of later Tier-2/Tier-3 integrated qualification.

### Existing MFM species-guard offline checks

`Tools/Verification/MfmPrescriptionCore/OfflineLogic/PrescriptionCoreOfflineCheck.cs` already contains executable assertions for the exact guard classes MFM-10 needs:

- target non-Sitka adult -> area excluded with `NonSitkaAdult`;
- sample non-Sitka adult -> coverage failure with an explicit non-Sitka reason;
- prescription derivation with a non-Sitka sample -> `SampleHasNonSitka`;
- non-Sitka confirmed Crop Tree -> `NotSitka` and no proposal;
- release competitors must be living Sitka and unmarked;
- deterministic proposal output / non-mutation checks.

`Tools/Verification/MfmPrescriptionCore/run_prescription_core_offline.py` compiles the **production** pure MFM sources unchanged with the existing offline check, verifies the frozen MFM-03 fixture hashes, treats warnings as errors, runs the check twice and requires byte-identical output. It writes evidence outside the repository and requires `MFM04_PRESCRIPTION_CORE_PASS` plus repeat determinism for success.

### Existing accepted three-species density checks

`Tools/Verification/ThreeSpecies/OfflineLogic/run_offline_check.py --suite density` compiles production `SitkaGrowthModel.cs` and `MixedDensityModel.cs` plus the accepted density checks, then also runs the density-composite and two arithmetic-isolation suites. The check explicitly covers:

- pure Sitka -> legacy exact Sitka route;
- any Oak/Beech under SpeciesAware model -> species-aware occupancy route;
- Oak and Beech serialized growing-space classes;
- pure Oak, pure Beech and mixed fixtures;
- no biological maximum-density/SDI claim and no broadleaf use of Sitka arithmetic;
- source/arithmetic isolation between the Sitka and mixed-density modules;
- deterministic generic mortality/calibration behaviour.

The primary density suite ends in `THREE_SPECIES_OFFLINE_PASS`; the runner's density group also requires the associated composite/isolation suites to return zero.

## MFM-10 requirement classification

| Requirement | Browser/static disposition | Evidence / reason | Residual |
|---|---|---|---|
| **10a — proposal feature is disabled or safely limited for unsupported species combinations** | **STATICALLY SATISFIED / EXISTING ACCEPTED EVIDENCE** | Coverage rejects non-Sitka samples and excludes target areas containing living non-Sitka adults before quantitative fit; prescription/release then re-guards non-Sitka Crop Trees and competitors. MFM authority explicitly permits refusal where calibration support is absent. | Re-execute existing MFM prescription-core guard suite on exact `6784ab17...` as executable confirmation; no new fixture/test design required. |
| **10b — no Sitka-only coefficient is silently applied to Oak/Beech** | **STATICALLY SATISFIED / EXISTING ACCEPTED EVIDENCE** | The only inspected MFM quantitative calibration is labelled Sitka-only/gameplay calibration; mixed target areas cannot reach `Covered`; Oak/Beech density remains governed by the separately accepted species-aware parity model rather than MFM Sitka calibration. No new Oak/Beech coefficient is introduced. | Same MFM prescription-core execution plus accepted density suite execution; no production edit. |
| **10c — mixed-species tests prove accepted ordinal species-aware density while refusing unsupported calibration assumptions** | **REQUIRES LOCAL OFFLINE EXECUTION** | Both halves already have authoritative committed offline assets: MFM prescription core tests the refusal path, and ThreeSpecies density tests the accepted Oak/Beech/mixed ordinal occupancy path. Browser inspection cannot truthfully claim these executables pass at `6784ab17...`. | Run the two exact commands below and record their receipts. |
| `requirements.md`: Sitka/Oak/Beech scope is honest | **STATICALLY SATISFIED / EXISTING ACCEPTED EVIDENCE** | V1 messages/statuses explicitly say Sitka-only / non-Sitka excluded; parity spec labels asymmetric evidence honestly. | Covered by command 1 execution receipt. |
| `requirements.md`: Sitka-only calibration is not silently generalised | **STATICALLY SATISFIED / EXISTING ACCEPTED EVIDENCE** | Fail-closed coverage + prescription/release guards prevent Oak/Beech from entering MFM Sitka-calibrated proposal generation. | Covered by command 1 + command 2 receipts. |
| `requirements.md`: mixed-species case is covered by tests | **REQUIRES LOCAL OFFLINE EXECUTION** | Test sources are present and specifically cover mixed/non-Sitka MFM refusal plus Oak/Beech/mixed parity density, but this browser phase did not execute them. | Commands 1 and 2. |

### Unity/runtime classification

**No MFM-10 requirement is classified `REQUIRES UNITY/RUNTIME` after this audit.** The species applicability/refusal logic and parity density arithmetic are pure-source paths with authoritative offline harnesses. MFM-10's own task wording says focused Unity behaviour is only needed *if needed*; the browser audit found no runtime-only species branch requiring it.

This does **not** waive later MFM-12/Tier-2/Tier-3 runtime qualification. It means #213 should not acquire/start Unity merely to close MFM-10 if the exact offline matrix below passes.

### Blocker classification

**No MFM-10 requirement is classified `BLOCKED — CODE/SPEC MISMATCH`.**

If either offline runner exposes a real production defect, #213 must stop and request a separate repair claim. It has no production repair authority. If the desired product behaviour changes from fail-closed Sitka-only MFM proposal generation to quantitative Oak/Beech/mixed MFM treatment, that is a spec/product amendment, not an MFM-10 test repair.

## Smallest exact residual local matrix for #213

Run only after fresh local preflight proves the worktree is on exact `6784ab17cac922a16815f4898a30737482101018`. The browser phase makes no local cleanliness, free-space, Editor-process or lock claim.

| # | Class | Exact command / fixture | Required evidence | PASS condition | STOP / failure condition |
|---|---|---|---|---|---|
| **1** | Local offline | `python3 -B Tools/Verification/MfmPrescriptionCore/run_prescription_core_offline.py --out /media/jer/Files/CCF/Verification/MFM10-SpeciesGuard-20261101/01-prescription-core` | Preserve command/HEAD receipt plus `run1.log`, `run2.log` and runner stdout in the final `Docs/Handoffs/MFM-10-Species-Parity-Guard/HANDOFF.md`. Runner itself validates the frozen MFM-03 fixture SHA-256 values. | exit 0; `MFM04_PRESCRIPTION_CORE_PASS`; `MFM04_REPEAT_PASS ... byte-identical output: True`; species guard assertions remain PASS. | Any non-zero exit, fixture hash mismatch, guard assertion FAIL, or non-deterministic repeat. Do not edit production. |
| **2** | Local offline | `python3 -B Tools/Verification/ThreeSpecies/OfflineLogic/run_offline_check.py --suite density --out /media/jer/Files/CCF/Verification/MFM10-SpeciesGuard-20261101/02-three-species-density` | Preserve command/HEAD receipt and complete stdout/stderr in the final handoff/evidence root. | exit 0; primary `THREE_SPECIES_OFFLINE_PASS`; density-composite + density-isolated + density-sitka-isolated sub-suites all return zero; Oak/Beech/mixed route/asset assertions remain PASS. | Any non-zero suite, missing PASS token, changed legacy fingerprint, broadleaf/Sitka arithmetic isolation failure, or mixed-route assertion failure. Do not rebaseline. |

### Explicitly not in #213's residual matrix

- no broad source/spec re-review;
- no new Oak/Beech MFM coefficient or quantitative prescription logic;
- no test rewrite unless an existing verification asset itself is demonstrably broken, in which case stop and report before changing it;
- no Unity/Editor gate if both offline commands pass;
- no save-schema/reference migration;
- no Frozen Reference v1 regeneration/rebaseline;
- no integration;
- no Windows build;
- no action on #194.

## #213 completion rule

If both commands pass on exact `6784ab17...`, and local preflight/cleanup evidence is sound, #213 may publish:

`Docs/Handoffs/MFM-10-Species-Parity-Guard/HANDOFF.md`

The final handoff should combine these executable receipts with this browser/static handoff and may return exactly:

`SPECIES GUARD QUALIFIED`

No Unity run is required for that MFM-10 token unless #213 discovers evidence that contradicts the static call boundary documented here. If such contradictory evidence appears, stop and return it to #200 instead of silently expanding the matrix.

## Dispatch for #213

**Plain-English purpose:** execute only the two residual offline guard suites that browser/static evidence cannot prove, then publish the final MFM-10 evidence handoff.  
**Difficulty:** **2/5 (Low/Medium)** — existing deterministic runners, no production design or implementation expected.  
**Best-suited provider/model:** **DeepSeek orchestrator/default worker** if capable, per current routing; escalate only for a demonstrated capability failure.  
**Exact lane:** local-only #203 lane; fresh dedicated worktree/branch from exact `6784ab17cac922a16815f4898a30737482101018`; suggested branch `verify/mfm-10-species-guard-v1`. Unity lane is **not required** by this handoff.  
**Claim:** #213 — **READY after publication of this handoff**.  
**Final handoff:** `Docs/Handoffs/MFM-10-Species-Parity-Guard/HANDOFF.md`.  
**User action required:** none for browser closure; local orchestrator can continue under #213.

Copy/paste dispatch:

`Run MFM-10 residual local qualification under claim #213 from exact candidate 6784ab17cac922a16815f4898a30737482101018 using Docs/Handoffs/MFM-10-Species-Parity-Guard/BROWSER-HANDOFF-20261101.md. Do not repeat browser/spec/source inspection. After fresh local preflight, run only: (1) python3 -B Tools/Verification/MfmPrescriptionCore/run_prescription_core_offline.py --out /media/jer/Files/CCF/Verification/MFM10-SpeciesGuard-20261101/01-prescription-core and (2) python3 -B Tools/Verification/ThreeSpecies/OfflineLogic/run_offline_check.py --suite density --out /media/jer/Files/CCF/Verification/MFM10-SpeciesGuard-20261101/02-three-species-density. If both pass on the exact candidate, publish Docs/Handoffs/MFM-10-Species-Parity-Guard/HANDOFF.md combining those receipts with the browser evidence and return SPECIES GUARD QUALIFIED. Do not run Unity unless contradictory evidence proves it is required. No production repair, integration, Windows build or #194 action.`

## Browser verdict

**APPROVED TO CONTINUE TO #213 RESIDUAL OFFLINE QUALIFICATION.**

Static/accepted evidence already establishes the intended fail-closed Sitka-only MFM calibration boundary and the separate accepted Oak/Beech/mixed parity model. The only MFM-10 evidence not truthfully provable in a browser is that the two already-committed deterministic offline suites pass on exact candidate `6784ab17...`.

## Dashboard / publication status

CCF Git is canonical. This browser handoff is the durable result for #212. Overall Dev/dashboard publication must be synchronized by the Manager workflow; actual Dashboard ingestion/rendering cannot be verified from this browser audit alone.
