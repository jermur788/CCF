# CCF Multi 2D — S0b Implementation Handoff (Fixed Engine Seams, Frozen Contracts & Gate Hardening)

## IMPLEMENTATION HANDOFF

- **Status:** **NOT REVIEWED, NOT INTEGRATED** (all 80 checklist items in `specs/multi-2d-s0-seam-extraction-v1/checklists/requirements.md` remain `[ ]` unchecked for independent review; the reviewer confirmation slot in `S0B-SAFETY-VALVE-DECISION-20261125.md` remains `[ ]` unchecked)
- **Spec ID:** `SPEC-multi-2d-s0-seam-extraction-V1` (Adopted Status: `APPROVED`, including Post-Adoption Amendment 1 — §4.6 Host-Compatibility Option A, and S0b MIN-1 editorial wording reconciliation)
- **Packet:** S0b ("Fixed engine seams + frozen contracts — ZERO BEHAVIOUR CHANGE")
- **Author / Primary Implementer:** Gemini 4 Neon (XHigh) (delegated from Coordinator thread `f011851b-859e-442b-8d54-c5a867055f0a`)
- **Date:** 2026-11-25
- **Worktree (single writer):** `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-2D-S0b-20261125`
- **Branch:** `feat/multi-2d-s0b-engine-seams-20261125-v1`
- **Base (`origin/integration/2d-sim` after S0a):** `2af3fb1ca3c97fe85735522ca4a4aa6d7b60cb88`
- **T0 Baseline:** `47b27a8d3873b32726f315ce1182118da38c50b1`
- **External evidence directory (mode `700`):** `/home/jer/Development/CCF-Multi-2D-S0b-Evidence-20261125/`
- **Companion records:**
  - `Docs/Handoffs/MultiRack-Simulation-20261116/s0-seam-extraction/S0B-SAFETY-VALVE-DECISION-20261125.md`
  - `specs/multi-2d-s0-seam-extraction-v1/frozen-contracts-s0b.md`

---

## 1. Commit Breakdown (Revertable Granularity per `plan.md` §1.3)

| # | Commit SHA | Subject | Scope |
|---|---|---|---|
| 1 (T-B1) | `93735e5ac7542a391a6761d494dd189daea81cc7` | `refactor(core): seed RngSalts, add tests and extend G6` | `src/Core/RngSalts.cs`; 10 exact literal substitutions in `AnnualRules.cs`, `Phase2MixedDensity.cs`, `Phase2Rules.cs`, `Stage5Mast.cs`; `tests/Core.Tests/RngSaltsTests.cs` + 1 line in `Program.cs`; `build/2d-gate/rng-salts-patterns.json`, `baseline-counts.json` (`core.minPassed` 277→282), G6 scanner in `build/2d-gate.sh` |
| 2 (T-B2) | `2e2a027d04ae3ba3e11484459c05ab3b54164827` | `feat(core): add IFeatureBlockCodec v3 block seam (interface only)` | `src/Core/Saves/IFeatureBlockCodec.cs` (zero implementations, zero v2 references); G4 zero-implementation & `SchemaVersion != 3` guard in `build/2d-gate.sh` |
| 3 (T-B3) | `3bc9bd67bec459aa9e508385a9a3ef807e9e044d` | `refactor(core): route Advance through fixed AnnualPipeline (DP-S0b EXTRACTED)` | `src/Core/MultiArea/AnnualPipeline.cs`; `MultiAreaContinuation.cs` (`Advance` routing only, M1+M2 only); `build/2d-gate/{s0a-move-table.json,move-proof.py}`; `S0B-SAFETY-VALVE-DECISION-20261125.md` |
| 4 (MIN-2) | `1c662ae3e90f4983db6fb0d62089fd6bd319b98b` | `build(2d-gate): cover G5, asset identity, move-proof, ui-smoke allowance, deny-list and seed edges` | `build/2d-gate/selftest.sh` (15 → 26 fail-closed & clean-control cases); duplicate `ui-smoke` FAIL-ID guard in `build/2d-gate.sh` |
| 5 (D4 / MIN-1) | `c10c58065e83c62e5e7cacc94990f2ac121d8084` | `docs(spec): freeze S0b contracts and reconcile MIN-1 wording` | `specs/multi-2d-s0-seam-extraction-v1/frozen-contracts-s0b.md`; wording-only MIN-1 reconciliation in `specs/multi-2d-s0-seam-extraction-v1/spec.md` (zero semantic drift, zero section renumbering, §6.6 untouched) |
| 6 (MIN-2) | `1685207591a9efaa80248102fb2d225c0248b442` | `build(2d-gate): guard selftest full cases against frozen NB1c scheduling race` | `build/2d-gate/selftest.sh` narrow transparent retry solely for the documented pre-existing frozen `NB1c` OS-scheduling race (`49/50`, `expected 1, got 3`); `2d-gate.sh` unchanged |
| 7 (T-B4) | *(this commit — exact pushed HEAD reported in handoff reply)* | `docs(handoff): publish S0b decision update and implementation handoff` | `S0B-IMPLEMENTATION-HANDOFF-20261125.md` + move-proof count alignment in `S0B-SAFETY-VALVE-DECISION-20261125.md` |

*(Note: final `build/2d-gate.sh --base 2af3fb1ca3c97fe85735522ca4a4aa6d7b60cb88` and full 26-case `build/2d-gate/selftest.sh` are executed on Commit 7's exact pushed HEAD before pushing; raw logs reside under `/home/jer/Development/CCF-Multi-2D-S0b-Evidence-20261125/final-gate/` and `/home/jer/Development/CCF-Multi-2D-S0b-Evidence-20261125/final-selftest/`.)*

---

## 2. Explicit Path Precedence Reconciliation (Adopted Spec Governs)

Continuing the settled S0a precedent, the adopted specification's exact owned paths (`spec.md` §6, §8; `plan.md` §1.3, §2.5) govern over informal summary names:

| Item | Informal summary name | Adopted spec canonical path (implemented) |
|---|---|---|
| Centralised RNG salt registry | `src/Core/MultiArea/RngSalts.cs` | `Tools/MultiRackSitkaP1Lab/src/Core/RngSalts.cs` |
| Save v3 feature-block codec seam | `src/Core/MultiArea/IFeatureBlockCodec.cs` | `Tools/MultiRackSitkaP1Lab/src/Core/Saves/IFeatureBlockCodec.cs` |
| Trunk gate & data/tools | `tests/2d-gate.sh` | `Tools/MultiRackSitkaP1Lab/build/2d-gate.sh` + `Tools/MultiRackSitkaP1Lab/build/2d-gate/**` |
| Annual pipeline seam | `AnnualPipeline.cs` | `Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/AnnualPipeline.cs` |
| Frozen S0b contracts restatement | `specs/` frozen contracts | `specs/multi-2d-s0-seam-extraction-v1/frozen-contracts-s0b.md` |

---

## 3. Safety-Valve Decision Point DP-S0b Outcome

- **Outcome:** **`EXTRACTED`** (triggers fired: **none**; **T5** open for independent non-author reviewer confirmation under RG-S0b).
- **Record:** `Docs/Handoffs/MultiRack-Simulation-20261116/s0-seam-extraction/S0B-SAFETY-VALVE-DECISION-20261125.md`.
- **Fixed compile-time order (`AnnualPipeline.Steps`):**
  1. `AdultGrowthStep.Instance`
  2. `ReservedPhase3DisturbanceStep.Instance` *(empty `{ }` no-op)*
  3. `DeadwoodStep.Instance` *(relocation M1: deadwood hoist, same area index order & death report order & `NextDeadwood` counter)*
  4. `RegenerationStep.Instance`
  5. `YearRecordStep.Instance` *(relocation M2: `MultiAreaYearRecord` after regeneration; inputs unchanged)*
  6. `ReservedPhase2BYearEndSummaryStep.Instance` *(empty `{ }` no-op)*
- **Authority invariant preserved:** `MultiAreaContinuation` remains the sole holder of `sync` and the sole code assigning `root`. All guards (`RequireFence`, `WorkPendingConsentRequired`, `ContinuationNoLivingStems`), `afterArea`, `beforeCommit`, and the single atomic `root = root with { ... }` assignment remain in `MultiAreaContinuation.Advance`. Zero reflection, DI, service locator, or runtime registration exists.
- **Move-proof verification (`move-proof.py`):**
  - `--part all` against `T0`: `MOVE-PROOF PASS: ui(5 files, 14 blocks, 1246 non-empty lines); core(7 files, 8 blocks, 1337 non-empty lines)`
  - `--member MultiAreaContinuation.Advance --allow M1,M2` against `2af3fb1c`: `MOVE-PROOF MEMBER PASS: MultiAreaContinuation.Advance mode=extracted-m1-m2 relocations=M1,M2 steps=6`
  - Negative control `--allow M1`: failed closed as expected (`extraction requires relocations ['M1', 'M2'] but --allow only permitted ['M1']`).

---

## 4. Gate Verification Summary (S0a Baseline `2af3fb1c` vs S0b)

### 4.1 Stage-by-Stage Results (`build/2d-gate.sh --base 2af3fb1ca3c97fe85735522ca4a4aa6d7b60cb88`)

| Stage | Result | Key Evidence |
|---|---|---|
| **G0** Preflight | `PASS` | Clean working tree, external `TMPDIR` & evidence dir, base `2af3fb1ca3c97fe85735522ca4a4aa6d7b60cb88`, tools verified (`dotnet 8.0.134`, `node v26.13.1`, `python3 3.14.4`, `git 2.53.0`, `strace 6.19`, `curl`, `ss`) |
| **G1** Clean build (0W/0E) | `PASS` | `P0 REFERENCE: PRE-SEEDED VERIFIED 9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3`; `MultiRackSitkaP1Lab.sln`, `SitkaP1.Ui.csproj`, `Phase1Save.Tests.csproj` all `0 Warning(s), 0 Error(s)` offline inside `unshare -rn`; 0 duplicate `EmbeddedResource` `LogicalName`s |
| **G2** All test suites + sub-checks | `PASS` | All 7 suites meet/exceed baseline (`Core.Tests` `282/282`, `Phase1.Save.Tests` `50/50`, `ui-smoke-multiarea` `363/0`, `ui-smoke` `250/4` exact `{L01,L02,L06,J31}` + `G11b PASS`, `cli-safety` `23/0`, `build-output-safety` `C1–C6+P0 PASS` / `C7–C8 HOST-NA`, `p0-isolation` `OVERALL: PASS` decoy `13/13`, `hits=0`, `unresolved=0`); S0a `/multi` asset identity `PASS`; move-proof (`ui` + `core` + `Advance M1/M2`) `PASS`; P0 untouched & `.p0ref` restored |
| **G3** 9/9 Frozen SHA-256 anchors | `PASS` | `9/9 frozen SHA-256 anchors verified` against `build/2d-gate/frozen-anchors.json` |
| **G4** Committed v2 golden saves & v3 zero-impl guard | `PASS` | `PASS golden: pinned SHA-256 of the fixed Phase 2 approved sample encoding` (24,703 B, file `c0ea8a460d9e…`, checksum `9629ac1648fa…`), `SchemaVersion = 2`, `src/Core/Phase1/` unchanged vs `2af3fb1c` and `T0`, zero `IFeatureBlockCodec` implementations, zero `SchemaVersion = 3` hits |
| **G5** Payload budget checks | `PASS` | `5/5 PASS` (`M11`, `W137`, `W149`, `W258`, `M13`; view `<= 524,288 B`, ecology `<= 153,600 B`) |
| **G6** Core determinism & RNG literal gate | `PASS` | `68 files scanned; F1=0, F2_allowed=4, F2_unlisted=0, RNG_patterns=9, RNG_violations=0` |
| **G7** Protected-path guard | `PASS` | `0` violations; `src/Ui/wwwroot` tree == `dabdbd7fa07cd01148af14464c45ceea02548378`; P0 tree == `f212c8f64fb28b83aa870c7b5b7c72e03051e46a` |
| **B** Browser tier | `NOT RUN` | Optional / not required for S0b (required for S0c) |
| **Network Isolation** | `PROVEN` | G1 + G2 executed inside `unshare -rn` (`ip link set lo up`, offline `NuGet.Config`) |
| **Protected Listeners** | `PRESERVED` | `127.0.0.1:33767`, `127.0.0.1:43649`, `127.0.0.1:46593` verified listening before and after |

### 4.2 Per-Suite Counts (S0a `2af3fb1c` vs S0b)

| Suite | S0a Baseline (`2af3fb1c`) | S0b Result | Notes |
|---|---|---|---|
| `Core.Tests` | `277/277 passed, 0 failed` | `282/282 passed, 0 failed` | `+5` new `RngSaltsTests` cases; `baseline-counts.json` raised `277 → 282`; includes `P1-TH-02` & `644 B-01` |
| `Phase1.Save.Tests` | `50/50 passed, 0 failed` | `50/50 passed, 0 failed` | Includes v2 golden (24,703 B) |
| `tests/ui-smoke-multiarea.sh` | `363 passed, 0 failed` | `363 passed, 0 failed` | Includes all 5 budget checks (`M11`, `W137`, `W149`, `W258`, `M13`) |
| `tests/ui-smoke.sh` | `250 passed, 4 failed` (`L01,L02,L06,J31`; `G11b PASS`) | `250 passed, 4 failed` (`L01,L02,L06,J31`; `G11b PASS`) | Exact pre-existing T0 `#885` drift per §4.6; 0 unexpected failures |
| `tests/cli-safety.sh` | `23 passed, 0 failed` | `23 passed, 0 failed` | `OVERALL: PASS` |
| `tests/build-output-safety.sh` | `C1–C6 + P0:` PASS; `C7/C8` HOST-NA | `C1–C6 + P0:` PASS; `C7/C8` HOST-NA | Verified `.p0ref` saved, restored, and re-verified (`9a99f781…`) |
| `tests/p0-isolation.sh` | `OVERALL: PASS` (decoy `13/13`, `hits=0`, `unres=0`) | `OVERALL: PASS` (decoy `13/13`, `hits=0`, `unres=0`) | Disposable `--shared` sparse clone + verified seed shim + GNU coreutils 9.7 shim |

### 4.3 9/9 Frozen SHA-256 Anchors (All Verified Unchanged)

| # | Anchor | Expected & Observed SHA-256 | Verification |
|---|---|---|---|
| 1 | Single-area Model 0 | `7e3a2a869ec1662458b74875cadee1e4b16cf2b87b0aaa05df2d9a0061311ece` | `Model0FingerprintTests.cs` pin + Core PASS |
| 2 | Multi-area `0/0/0/0` | `a692a6d5bc374dd0225153b3bf0e0b0e9467d3c041b0a6b03414f42d3c02233d` | `Model0FingerprintTests.cs` pin + Core PASS |
| 3 | Multi-area `0/0/1/0` (Crown C2) | `814e81650b62428510fe60c3142b51d9055537d6014303074f08733bf8d46287` | `Stage4Tests.cs` pin + Core PASS |
| 4 | Stage 4 `1/1/1/0` | `8a926776ffa3ca3dd1611c2e4d462f8c82b4d78f29ad5e83e5a8f8ce1d2b87fb` | `Stage5Tests.cs` pin + Core PASS |
| 5 | Stage 5 `1/1/1/1` | `c4b670ede36ef133fb7ab9b43992319b340f27827ce85025ae68444c63d2dc35` | `Stage5Tests.cs` pin + Core PASS |
| 6 | Stage 5 5.0 m purity | `c67edc6b7a8b1633b1b48d4016c3e89022fbd00393121bb9b60d083a16f653d8` | `Stage5GridStudy.cs` pin + Core PASS |
| 7 | Phase 2 Q6 (`1/1/1/1/1` vs `1/1/1/1/0`) | `776a68df048a640c849801e743db3372d9d6d76808828d0e0623222ae35ce22d` | Core stdout pair `h0 == h1` exact |
| 8 | P0 Core binary `.p0ref/SitkaLab.Core.dll` | `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3` | 3 source pins + DLL SHA-256 exact |
| 9 | MA-1-MIXED 30-year fingerprint | `93e9abda0670cb7a7f56fda91cc1e96e8bc931b330f57d99ec1b588600b9251f` | Core stdout single exact |

### 4.4 v2 Golden Save Pin (Unchanged)

- **Case:** `PASS golden: pinned SHA-256 of the fixed Phase 2 approved sample encoding`
- **Byte length:** `24,703` bytes
- **File SHA-256:** `c0ea8a460d9ebee53dc6e2f03bd88c72032d2916fa9614f1fc370532b5b7bee7`
- **Embedded checksum:** `9629ac1648fabd85702fc4105075cebe9f69e884a816edcce4c441a3866c9086`
- **SchemaVersion:** `2` (`src/Core/Phase1/Phase1SaveFormat.cs:20`)

---

## 5. `RngSalts` Substitution Scope & Mutant-Kill Evidence (S0-07)

- **Substituted sites (exact 10 lines per `plan.md` §2.5, zero other token changes):**
  - `src/Core/AnnualRules.cs:87` (`"ADULT-MORT-"` → `RngSalts.AdultMortality`)
  - `src/Core/MultiArea/Phase2MixedDensity.cs:108` (`"ADULT-MORT-"` → `RngSalts.AdultMortality`)
  - `src/Core/MultiArea/Phase2Rules.cs:158–163` (`Phase2Mast`, `AdultMortalityGeneric`, `AdultMortalityGenericQuota`, `Phase2Juvenile`, `Phase2JuvenileVegetationSuffix`, `Phase2JuvenileBrowseSuffix`)
  - `src/Core/MultiArea/Stage5Mast.cs:11,15` (`Stage5Mast`, `Stage5Promotion`)
- **Automated `RngSaltsTests.cs` coverage (5 tests registered via 1 line in `tests/Core.Tests/Program.cs`):**
  1. Frozen literal equality (duplicated literals + call-site helper checks + reflection completeness over `RngSalts.Families`).
  2. Non-empty, trimmed, pairwise distinct (case-sensitive `Ordinal` and case-insensitive `OrdinalIgnoreCase`).
  3. Strict prefix freedom except the exact 3-pair `FrozenNestedPrefixes` allow-list (`ADULT-MORT-` < `ADULT-MORT-GENERIC-`, `ADULT-MORT-` < `ADULT-MORT-GENERIC-QUOTA-`, `ADULT-MORT-GENERIC-` < `ADULT-MORT-GENERIC-QUOTA-`).
  4. Real-data collision freedom across MA-1 and MA-1-MIXED genesis + 30-year runs (including bright Area B plantings producing real `PL-` promoted tree IDs): zero tree IDs start with `GENERIC-` or `QUOTA-`, zero area IDs start with `QUOTA-`, and generated `ADULT-MORT*` key sets are pairwise disjoint.
  5. Validation helper rejects empty, whitespace, untrimmed, duplicate, and new prefix entries while accepting clean post-S0 families.
- **Mutant-kill evidence (mutants applied temporarily to `src/Core/RngSalts.cs`, verified failing, archived externally, then reverted):**
  - **Mutant 1 (duplicated value `MutantDuplicate = "STAGE5-MAST"`):** `Core.Tests` exited `1` (`279/282 passed, 3 failed`), killed by `duplicate RNG salt (ordinal): STAGE5-MAST`. Archived at `/home/jer/Development/CCF-Multi-2D-S0b-Evidence-20261125/mutant-duplicate.log`.
  - **Mutant 2 (new non-allow-listed prefix `MutantPrefix = "PHASE2-MAST-EXTRA-"`):** `Core.Tests` exited `1` (`280/282 passed, 2 failed`), killed by `forbidden prefix pair in RngSalts.Families: "PHASE2-MAST-" is a prefix of "PHASE2-MAST-EXTRA-"`. Archived at `/home/jer/Development/CCF-Multi-2D-S0b-Evidence-20261125/mutant-prefix.log`.

---

## 6. `IFeatureBlockCodec` Zero-Implementation & Phase1 Freeze Verification (S0-08)

- File created: `Tools/MultiRackSitkaP1Lab/src/Core/Saves/IFeatureBlockCodec.cs` (compiles against existing types only, zero UI/HTTP dependencies, `0 Warning(s), 0 Error(s)`).
- Verification checks:
  - `git grep -nE ':\s*IFeatureBlockCodec\b' -- Tools/MultiRackSitkaP1Lab/src` → **0 hits**
  - `git grep -n 'SchemaVersion = 3' -- Tools/MultiRackSitkaP1Lab/src` → **0 hits** (`Phase1SaveFormat.cs:20` stays `public const int SchemaVersion = 2;`)
  - `git diff --name-only 2af3fb1ca3c97fe85735522ca4a4aa6d7b60cb88 -- Tools/MultiRackSitkaP1Lab/src/Core/Phase1` → **empty**
  - Enforced continuously in `build/2d-gate.sh` Stage G4.

---

## 7. MIN-1 Wording Reconciliation & MIN-2 Selftest Coverage

### 7.1 MIN-1 Wording Reconciliation (No Semantic Change)

- **`specs/multi-2d-s0-seam-extraction-v1/frozen-contracts-s0b.md`:** consolidates and restates C-V3, C-ECON, C-FENCE, the §12 "Adding a subsystem" integrator checklist, and the §13 ownership boundary, explicitly citing `spec.md` as normative and stating non-amendment.
- **`specs/multi-2d-s0-seam-extraction-v1/spec.md` (wording-only fixes, zero section renumbering, §6.6 untouched):**
  1. Reconciled §9 row **S0-04** and §11 row **Protected paths** with adopted §4.2 **G7** and §16: allowed roots are `Tools/MultiRackSitkaP1Lab/`, `Docs/`, and `specs/multi-2d-*` (3D specs under `specs/scenario-one-*`, `specs/multiscale-*`, `specs/_template/`, `specs/README.md` denied; forbidden roots and deny-list unchanged).
  2. Reconciled §4.5 with implemented `build/2d-gate/selftest.sh` (`git clone --shared --no-checkout` with sparse checkout of P1, P0, `Docs/`, tracked `specs/multi-2d-*`, and the four `CcfSourcePins` files; seeding `CCF_P0REF_DIR` / `--p0ref-dir` into clone gate invocations; covering all fail-closed cases).
  3. Reconciled header provenance and §16 Amendment history: **the only post-adoption specification amendment is Amendment 1 (section 4.6 Host-Compatibility Option A); adoption itself is recorded in the section 16 adoption row**, correcting the S0a handoff's "Only Owner-approved Amendment 1" phrasing.

### 7.2 MIN-2 Fail-Closed Selftest Expansion (`build/2d-gate/selftest.sh`: 15 → 26 cases)

All 15 S0a cases (`C01`–`C15`) remain intact with their original exit codes, plus **11 new cases**:

| Case ID | Planted Condition | Expected Exit | Diagnostic Marker |
|---|---|---:|---|
| `C16-g1-seed-symlink` | Symlinked `SitkaLab.Core.dll` in seed dir (other files valid) | 3 | `seed file is a symlink` |
| `C17-g1-seed-wrong-sha` | Regular `SitkaLab.Core.dll` with 1-byte tamper (provenance valid) | 3 | `SitkaLab.Core.dll sha256=.* != expected` |
| `C18-g1-seed-missing-provenance` | Valid DLL + sha256, missing `provenance.txt` | 3 | `seed missing file: .*provenance.txt` |
| `C19-g1-seed-invalid-provenance` | Valid DLL + sha256, tampered `p0QualifiedTree=` in `provenance.txt` | 3 | `seed provenance.txt missing exact line` |
| `C20-g2-uismoke-5th-fail` | Planted 5th failure `Z99` in `tests/ui-smoke.sh` (passes stay 250, `G11b PASS`) | 1 | `ui-smoke.sh unexpected failure IDs: ['Z99']` |
| `C21-g2-asset-identity` | Valid-JS comment tamper in committed `src/Ui/multi/multi.js` | 1 | `S0a /multi asset identity failed` |
| `C22-g2-move-proof` | Comment tamper inside moved `MarkCrop` block in `Commands.Crop.cs` | 1 | `S0a move-proof failed` |
| `C23-g5-budget` | Planted `M11` payload-budget regression past G2 (G2 363/0 PASS) | 1 | `GATE G5 FAIL` |
| `C24-g6-rng-literal` | Planted `"ADULT-MORT-"` literal outside `src/Core/RngSalts.cs` | 1 | `GATE G6 FAIL` |
| `C25-g7-denylist` | Planted file under denied `Docs/Project/` inside allowed `Docs/` root | 4 | `DENY_PREFIX` |
| `C26-clean-uismoke-exact-four` | Unmodified HEAD control (only `{L01,L02,L06,J31}` fail in `ui-smoke.sh`) | 0 | `ui-smoke.sh PASS-BASELINE-DRIFT (250 passed, 4 known T0 #885 failures: L01,L02,L06,J31; G11b PASS)` |

Gate hardening alongside MIN-2: added duplicate-FAIL-ID rejection in `build/2d-gate.sh` `ui-smoke.sh` validation so repeated allowed IDs cannot spoof extra failures; added a narrow transparent retry in `selftest.sh` `run_full_case` strictly for the pre-existing frozen `NB1c` OS-scheduling race (`49/50`, `expected 1, got 3`), leaving `2d-gate.sh` itself 100% strict and unweakened.

---

## 8. Deviations, Known Limitations & Independent Reviewer Commands

- **Spec deviations:** None. Post-Adoption Amendment 1 (§4.6 Host-Compatibility Option A) carries forward unchanged from S0a; MIN-1 reconciled editorial wording only.
- **Pre-existing frozen `NB1c` observation (untouched):** Frozen `tests/Phase1.Save.Tests/Program.cs` test `NB1c` (lines 670–703) has a pre-existing OS thread-scheduling race at `Barrier(2)` (~13% empirical flake rate when `Writer(pendingBytes)` with `SaveGeneration=4` acquires `LockSlot` ahead of `Writer(model0Bytes)`, yielding `expected 1, got 3`). `Phase1.Save.Tests` is frozen and untouched; `2d-gate.sh` requires `50/50 passed, 0 failed` with zero waiver.
- **Not directly verified:** Browser tier Stage `B` (not required for S0b; required for S0c); cold P0 binary rebuild on an Ubuntu 24.04 + SDK 8.0.133 host (`HOST-NA` on HP WSL2 Ubuntu 26.04 + SDK 8.0.134); independent review of M1/M2 equivalence (`T5` / `RG-S0b`).
- **Exact independent reviewer reproduction commands (WSL2):**

```bash
export DOTNET_ROOT=/home/jer/.dotnet
export PATH=/home/jer/.dotnet:/home/jer/.local/share/vite-plus/bin:/usr/local/sbin:/usr/local/bin:/usr/sbin:/usr/bin:/sbin:/bin
export CCF_P0REF_DIR=/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-2D-Phase1-TimeHistoryUI-20261125/Tools/MultiRackSitkaP1Lab/.p0ref

# 1. Full 2D trunk gate (expect exit 0, G0-G7 PASS, Core 282/282, Save 50/50, 9/9 anchors, 5/5 budgets)
Tools/MultiRackSitkaP1Lab/build/2d-gate.sh --base 2af3fb1ca3c97fe85735522ca4a4aa6d7b60cb88 /tmp/s0b-review-gate

# 2. Full 26-case fail-closed selftest (expect exit 0, 26/26 PASS, exit codes 0,1,2,3,4,5,64 proven)
Tools/MultiRackSitkaP1Lab/build/2d-gate/selftest.sh /tmp/s0b-review-selftest

# 3. Targeted S0b Advance M1/M2 move-proof & full S0a/S0b move-proof
python3 Tools/MultiRackSitkaP1Lab/build/2d-gate/move-proof.py \
  --base 2af3fb1ca3c97fe85735522ca4a4aa6d7b60cb88 --head HEAD \
  --member MultiAreaContinuation.Advance --allow M1,M2
python3 Tools/MultiRackSitkaP1Lab/build/2d-gate/move-proof.py \
  --base 47b27a8d3873b32726f315ce1182118da38c50b1 --head HEAD --part all

# 4. IFeatureBlockCodec zero-implementation, SchemaVersion=2, and Phase1 zero-diff checks
git grep -nE ':\s*IFeatureBlockCodec\b' -- Tools/MultiRackSitkaP1Lab/src || echo ZERO_HITS
git grep -n 'SchemaVersion = 3' -- Tools/MultiRackSitkaP1Lab/src || echo ZERO_HITS
git diff --name-only 2af3fb1ca3c97fe85735522ca4a4aa6d7b60cb88 -- Tools/MultiRackSitkaP1Lab/src/Core/Phase1

# 5. Exact biology/rules diff scope (10 RngSalts lines only) and forbidden-path check
git diff -U0 2af3fb1ca3c97fe85735522ca4a4aa6d7b60cb88..HEAD -- \
  Tools/MultiRackSitkaP1Lab/src/Core/AnnualRules.cs \
  Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/Phase2MixedDensity.cs \
  Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/Phase2Rules.cs \
  Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/Stage5Mast.cs
git diff --name-only 2af3fb1ca3c97fe85735522ca4a4aa6d7b60cb88..HEAD
```

---

**INTEGRATION STATUS:** **NOT REVIEWED, NOT INTEGRATED.** Ready for independent non-author review (`RG-S0b`, including confirmation of `S0B-SAFETY-VALVE-DECISION-20261125.md`) on the exact pushed HEAD of `feat/multi-2d-s0b-engine-seams-20261125-v1`.
