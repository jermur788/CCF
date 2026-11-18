# Multi #723 — first deterministic Sitka year: implementation readiness and overnight result

**Status: STAGE A COMPLETE — STAGE B NOT DISPATCHED. 3 genuine blockers. No annual source, no ecological year, no P0/P1/R mutation.**
**Date:** 2026-11-18. **Claim:** [#723](https://github.com/jermur788/CCF/issues/723). **Manager:** [#694](https://github.com/jermur788/CCF/issues/694).
**Orchestrator (actual):** DeepSeek V4.1 Flash (`deepseek-v4.1-flash`), provider `opencode-go`, OpenCode in T3 Code, host `jer-laptop` (Linux 7.0.0-42-generic). Effort/variant not observable and not claimed.
**Parent docs branch:** `docs/multi-deepseek-firstyear-overnight-result-20261118-v1`, from full main HEAD `aaa6254b7eb3a61ef65aafc2ad61ac821119a592`.
**Ledger:** [overnight orchestration ledger](Multi-2D-SITKA-723-OVERNIGHT-ORCHESTRATION-LEDGER-20261118.md).

**Status labels used strictly:** PROPOSED · APPROVED SPEC · IMPLEMENTED · REVIEWED · INTEGRATED · VERIFIED · ACCEPTED. Nothing here is IMPLEMENTED, VERIFIED or ACCEPTED code.

---

## 1. Overnight outcome in one paragraph

The authority, source and oracle work is complete and stronger than the packet assumed: the four-file P1 specification **is owner-approved**, the 30 m/100-tree derived fixture identity **is accepted**, and a source-faithful Year-1 arithmetic oracle can now be stated exactly. The reason nothing was implemented is narrower than “the spec is a draft”: the **approved** F0 packet already contains a one-year annual source-rule step, but a fresh source write requires three things that are not in place tonight — an explicit written policy allowing isolated development while default .NET L4 remains **FAIL38**, a newly assigned physical exclusive source/test lane, and a Manager `[APPROVED TO START]` on #723. Adding annual code under an unwritten security policy would be exactly the waiver the packet forbids. The dossier therefore pins the specification, the fixture, the exact Year-1 expected result, the negative/refusal matrix and the qualification plan, so the next authorised writer starts from a decision, not a rediscovery.

**No source was written. No `dotnet` build or test was run. No ecological year was executed. No Unity, Windows, save, HTTP or P0/P1 file was touched. The retained R candidate and its custody are unchanged.**

---

## 2. Authority reconciliation (what is actually approved)

| Item | Exact evidence | Verdict |
|---|---|---|
| Latest direct owner instruction | This overnight dispatch for #723; Manager #694 preparation comment 2026-11-18T00:58Z | Authorises **orchestration and Stage A only** until the four Stage B checks hold. |
| Owner four-file technical spec approval | [`90583811c4fb88508a4bf05c14365732e24a21ac`](https://github.com/jermur788/CCF/blob/90583811c4fb88508a4bf05c14365732e24a21ac/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-Sitka-P1-OWNER-FOUR-FILE-APPROVAL-AND-639-F0-LOCAL-IMPLEMENTATION-PACKET-20261116.md); owner answer “They're all approved.” at 2026-11-16 18:56 UTC; spec status **APPROVED FOR BOUNDED P1 IMPLEMENTATION PACKETS** | **APPROVED SPEC.** Not IMPLEMENTING/VERIFIED/ACCEPTED. |
| Bounded F0 annual scope | Same F0 packet: “deterministic one-year source-rule annual 30m **once manifest gate passes**, simple repeat-year semantics and per-ID audit”; `plan.md` G2-F0 “Synthetic 30m/0.09ha experimental annual source-rule model … NOT Unity parity” | A one-year adult annual step is **inside the approved F0 scope**. It is not a new specification. |
| 30 m fixture identity | `p1d-30m-sitka-adult-v1`, SHA-256 `bcaaa5c3…757c4`, content hash `b7dee8a3…6627`; independent review #661 and Manager #641 acceptance | **ACCEPTED — identity only.** Not ecological calibration, not Unity parity, not permission to run a year by itself. |
| Fixture trust status in R | `TrustTables.cs` records the derived fixture as `ReviewStatus.Accepted`; `AnnualGate.Check` returns `AnnualEngineNotImplemented` | Trust gate **passes**; engine gate **blocks** by design. |
| F0 task state | #639 OPEN / board-reserved / no active worker | The annual deliverable was never implemented; a new bounded packet or explicit revival is required. |
| Main canonical decision log | `Docs/Project/decision-log.md` on main: D-082 (standalone Sitka experiment, DRAFT spec), D-084 = **Compact30 shelter choices** | **Documentation collision, not an approval:** main D-084 is a _different_ decision. The Multi P1 D-084 exists only on `docs/multi-p1-owner-g2-approval-20261116-v1`. |
| P1 spec merge state | Four-file spec and Multi D-084 are **not merged to main** | Readers must cite the branch document, never main D-084, for P1 authority. Flagged as a real integration hazard. |
| Default .NET L4 | #715 and #720 evidence; Manager #720 verdict; #722 OPEN/BLOCKED | **FAIL38 remains, strict analyzer not relaxed.** No waiver. |
| Physical R custody | Ops row `CCF-710-MULTI-P1-R-CUSTODY`, lane `MultiP1ModeRepair710`, custodian user `jermur788`, **NO ACTIVE WRITER**, expiry 2026-11-19 18:00 UTC | Preserved. No lane was inherited, borrowed or cleaned. |
| Other owners | #724 separate Compact30 docs mission; #722 blocked; Unity, canonical/legacy stores, Scenario One, user-dirty Primary `AGENTS.md` and `Docs/Operations/StorageArchitecture.md` | Untouched. |

**Consequence:** Stage B check **A** (approved current first-year spec) is satisfied for the bounded F0 annual step, with the caveat that the Manager should record `IMPLEMENTING` against `90583811` when a writer starts. Checks **B**, **C** and **D** are not satisfied.

---

## 3. Stage B gate matrix — why no writer was dispatched

| Gate | Required evidence | Actual state (2026-11-18 01:26Z) | Verdict |
|---|---|---|---|
| **A. Approved current spec** | Manager/owner-approved exact first-year spec with implementation status APPROVED/IMPLEMENTING | Four-file spec owner-approved on `90583811`; F0 packet explicitly includes the one-year annual after manifest acceptance; derived fixture identity accepted (#661/#641). Spec **not merged to main**; Multi D-084 only on a docs branch; #639 still reserved. | **PASS with caveat.** Bounded F0 annual is authorised by the existing approved packet. Manager should record the implementing status and the exact allowlist for the new writer. |
| **B. Security development policy** | Explicit confirmation that isolated development may proceed while default L4 FAIL38 stays unwaived and release acceptance is not downgraded | Default L4 FAIL38 is accepted as unresolved (#715, #720). #722 is the whole-role qualification and is **OPEN/BLOCKED**; its packet forbids an annual year inside that task. No document says “isolated development is permitted under FAIL38”. Precedent (#703/#710/#720 ran builds/tests while reporting FAIL38) is **not** written policy. | **FAIL — decisive.** Proceeding would be an inferred waiver. |
| **C. Physical ownership** | New exclusive source/test lane on Files/ext4, fresh branch/worktree, sole writer/model/host, allowed file set, outputs, lease; R preserved | No #723 lane exists. R custody is retained with **NO ACTIVE WRITER** and an owner-managed deadline. No custodian assignment was issued for a new writer. | **FAIL.** |
| **D. Manager start permit** | Manager #694 publishes explicit `[APPROVED TO START]` (or recorded equivalent direct owner authority), then the actual worker posts `[RUNNING]` with branch/worktree/HEAD before any clone/build/write | #723 contains the Manager's preparation handoff only. No start permit. | **FAIL.** |

Per the packet: **“If any check fails, DO NOT dispatch native implementation.”** Stage B was not entered. No substitute worker, no “quick” run, no L4 whitelist, no P0 mutation.

---

## 4. Implementation-readiness matrix

Legend: **READY** = authority + oracle + file group are sufficient for a bounded writer; **READY WITH TEST** = mechanism is specified but the frozen fixture does not exercise its branch, so a dedicated test/derived fixture is mandatory; **HOLD** = not authorised or not specified for this slice.

| # | First-year mechanism | Accepted authority / source line | Status | Required test oracle | Code owner / file group |
|---|---|---|---|---|---|
| 1 | Domain tag and experiment identity | Spec §0 B01; `Domain.cs`; fixture `domain=DerivedP1ThirtyMetreExperimentV1` | READY | Exact case-sensitive tag; wrong/missing tag refuses `UnknownDomain`; never infer from size/ID/name | P1 writer — `src/Core/Domain.cs` (extend only if needed) |
| 2 | Genesis manifest and trust | Spec §4.1/§5.1; `DerivedManifest.cs`; `TrustTables.cs`; accepted fixture | READY | 100 unique IDs, 100 unique positions, all live, MatureEquivalent/Reference/Sitka/unpruned, 12 below 170 mm; altered bytes/ID/area/bits refuse before any year | P1 writer — manifest loader unchanged; new annual engine reads it |
| 3 | State schema for an in-memory year | Spec §5 (per-experiment, per-ID, per-stand, per-event fields) | **READY (in memory)**; C-1 **HOLD** for persistence | Before/after per-ID DBH/H/crown/age/status; pre-growth CI; N, BA, Dq, RD, P; cause/year; no duplicate death; no victims in Year 1 | P1 writer — new `src/Core/AnnualEngine.cs` + state DTOs |
| 4 | Work/felling boundary | Spec §5.2; R has no work phase | **HOLD for this slice** (no work, no felling) | Must refuse work/plan commands or omit them; a mark/proposal must never change N/BA/CI | Not in this packet |
| 5 | Ecological-year increment | Spec §5.2; source `AdvanceOneYear` increments once before CI/RNG | READY | Year 0→1 exactly once; failure leaves year 0; no second increment on retry | `AnnualEngine` |
| 6 | Pre-growth Hegyi CI | Source `UpdateCompetition`; inclusive 8 m, ordinal ID order, inline float32 term, 0.5 m distance floor, `max(1, D_target)` | READY WITH TEST | 1,622 directed pairs in the frozen fixture; max CI `8.432238578796387` (P30-053); exact 8 m boundary and >8 m cases need a **dedicated edge fixture** (frozen fixture's nearest gap is 7.9914/8.0048 m) | `AnnualEngine` + competition helper |
| 7 | DBH growth | Source `GrowAdults`; Sitka asset `1.2`, `120`, `Ci50 5`; pre-growth CI; clamps `[1,200]` | READY | Per-ID post-DBH table (Appendix A); 20 cm CI=0 → +1.0 cm; DBH ≥120 → 0; clamp probe | `AnnualEngine` |
| 8 | Height | Source `SitkaGrowthModel.NextHeight`; Class III `H=34.362…(1−e^(−0.042a))^1.563` anchored at 20.4 m at age 30; relative height; clamp `[0.1,60]` | READY | Age 30→31 per-ID values; age 30 H=20.4 → next relative height; 60 m clamp; **not** GROWFOR/SI30 | `AnnualEngine` + linked `SitkaGrowthModel` |
| 9 | Crown | Source `PotentialCrownRadiusM` (linear branch, `usePowerLawCrown:0`), 15 % relaxation, 0.1 m floor; post-growth DBH, pre-growth CI | READY | Per-ID crown table; CI=0 and CI=5 probes; floor probe; initial crown is synthetic zero-competition, not measured | `AnnualEngine` |
| 10 | Density and pressure | Source `StandRelativeDensity` + `SitkaGrowthModel`; all **living** trees; post-growth DBH; declared f32 area `0.09` | READY | Initial RD `0.609795828454288`; post-growth RD `0.6375836886879149`; P `0.09395922171978738`; all-living denominator includes the 12 below threshold | `AnnualEngine` |
| 11 | Hazard mortality | Source `ApplyAdultDensityMortality`; `S=1−1/(1+CI/5)` from **pre-growth** CI; `p=min(0.5,0.08·P²·V/meanV)`; conditional roll | READY WITH TEST | Exact 100 rolls and p values (Appendix A); **Year 1 has zero victims**, so a **separate approved higher-pressure derived fixture** must exercise the branch, plus a fixed victim-set check | `AnnualEngine` |
| 12 | Boundary mortality | Source survivor RD>1 loop; S desc / ID asc; running squared-DBJ; last survivor preserved | READY WITH TEST | Needs a dedicated synthetic high-RD or singleton probe; must prove hazard/boundary disjointness, one death per ID, `self-thinning` cause/year | `AnnualEngine` |
| 13 | Deterministic RNG | `SimulationRandom.Roll` model 1, SplitMix64 finaliser, key `"ADULT-MORT-"+TreeId`, **post-increment** year, integer seed | READY | Exact `P30-001` roll `0.165713369846344` (numerator 2780209); model-0 control `0.8582888245582581` (14399697); whole 100-ID vector stable across two clean process invocations | `AnnualEngine` + linked unchanged `SimulationRandom` |
| 14 | Chronology | Spec §3; source order: year → CI → growth → crown → density death; no second growth after death | READY | Victims (when present) must have grown and aged first; no survivor re-growth; event order observable in the annual record | `AnnualEngine` |
| 15 | Atomicity | Spec §5.2 | READY for in-memory commit | Injected failure leaves year, trees, RNG state and mode unchanged; retry is not a second biology | `AnnualEngine` (no persistence) |
| 16 | Save/load/replay | Spec §5.2/§9; C-1 OPEN; F0 has **no save** | **HOLD — out of this overnight slice** | Do not implement or claim; a future C-1-reviewed packet owns it | Not in this packet |
| 17 | Refusal semantics | `Domain.cs`, `FixtureAccess.AnnualGate`, CLI exit codes | READY | Exact codes and exit 2, no mutation: `AnnualEngineNotImplemented` (30 m today), `OriginalAnnualRefused`, `DomainUnavailable`, `UnknownDerivedFixture`, `UnknownDomain`, `ManifestNotAccepted` (only if trust regresses) | `src/Cli/Program.cs` + gate |
| 18 | Numerical reference class | Spec §3 R-1; C-2a/C-2b | **HOLD for parity claims** | Label every output `source-derived, not Unity parity`; C-2b only after a legal 40/80 Unity capture; C-6 owner GO absent | Writer must not claim parity |
| 19 | Zero-growth P0 protection | P0 spec; `Tools/MultiRackSitkaLab/**`; P0 98 tests; L0–L4 | READY as an obligation | P0 52-file subtree `f212c8f6…` unchanged; 82 + 16 tests; fixture hashes; L0–L3 PASS; **L4 FAIL38 reported, never waived** | Writer must run and report |
| 20 | UI observation | F0 CLI only | **HOLD** (out of scope) | No UI in this overnight slice | Not in this packet |

**File group for an authorised writer (proposal, not a grant):** new `Tools/MultiRackSitkaP1Lab/src/Core/Annual*.cs` (+ DTOs), a narrow `src/Cli/Program.cs` `annual` path replacing the refusal, and `tests/Core.Tests/**` annual tests. **Read-only:** `Tools/MultiRackSitkaLab/**`, `Assets/**`, `Packages/**`, `ProjectSettings/**`, `Docs/Project/**`, `specs/**`, the accepted fixture bytes, all saves/references, and the four pinned CCF blobs. Do not edit P0, the fixture, the catalog digest or the trust table.

---

## 5. Deterministic Year 1 oracle (independent source transcription)

**What this is.** An independent transcription of the pinned source's adult-only path over the frozen 100-tree fixture, computed in this session from the source text with explicit binary32 rounding where the C# expression rounds to `float` and binary64 where it uses `double`. It reproduces four quantities that were independently derived elsewhere (#643 initial metrics; #653 G10/G11 rolls), which is the validation that the transcription is not arbitrary.

**What this is not.** It is **not** a compiled-C# output, **not** a Unity/Mono observation, **not** a C-2b parity result and **not** an accepted golden until a compiled candidate reproduces it under a declared numeric policy. `Math.Pow`/`Math.Exp`/`Math.Sqrt` last-bit behaviour is runtime/library dependent; the discrete outcomes below have wide margins and are the robust part.

### 5.1 Pinned inputs

| Item | Value |
|---|---|
| Fixture | `p1d-30m-sitka-adult-v1.sitkap1fx.json`, 32,567 bytes, SHA-256 `bcaaa5c3d9b3d36ad8319ffcc361a6225a02b90281cef8435de7c52f8ff757c4`, content hash `b7dee8a34b4e66b4cc443b533604b5962f5fe4c1d2f28832270a0c2b02ba6627` |
| Trees | 100 live `P30-001`…`P30-100`, age 30, all MatureEquivalent/Sitka/Reference, unpruned, site productivity 1.0, positions inside `[−15,+15)` m |
| Threshold | 170 mm; **12** stems below it (`P30-018`, `-026`, `-031`, `-032`, `-038`, `-040`, `-047`, `-053`, `-055`, `-080`, `-092`, `-098`) — included in CI/RD/growth/mortality |
| Year / seed / models | ecological year 0 → 1; simulation seed `20261116`; growth model 2 (pure-Sitka Reference route); RNG model 1; algorithm `p1-f0-adult-sitka-30m-v1` |
| Area | declared 900 m² = 0.09 ha, f32 bits `3db851ec` (not a Unity `standSize²` expression) |
| Source pins | `SimulationRandom.cs bb83d7f8`, `SitkaGrowthModel.cs e4c73402`, `SitkaSpruce.asset e638e28e`, `TreeSpeciesDefinition.cs 51faaa0d` |
| Excluded | storm, canopy/light, regeneration, recruitment, juveniles, mixed species, browsing, pruning, work/felling, save |

### 5.2 Exact Year-0 and Year-1 aggregates

| Quantity | Value | Note |
|---|---|---|
| Initial N/ha | `1111.111083984375` | `float(100 / 0.09f)` widened to double; matches #643 |
| Initial Dq | `25.303288374529917` cm | double sum of f32 DBH²; matches #643 |
| Initial RD | `0.609795828454288` | `N/ha·(Dq/25)^2.063/1868`; matches #643 |
| Initial BA/ha | `55.872911409` m²/ha | matches #653 reviewer arithmetic |
| Post-growth Dq | `25.855791791844318` cm | after the DBH law with pre-growth CI |
| Post-growth RD | `0.6375836886879149` | post-growth DBH, same 100 living stems |
| Density pressure P | `0.09395922171978738` | `max(0,(RD−0.6)/0.4)` |
| Mean vulnerability | `0.035410183953663125` | mean of `S⁴`, pre-growth CI. Independent check: `…3112`/`…3115` (2 binary64 ULPs; sequential vs compensated sum) |
| Max p | `0.0030975518881867846` | `P30-053` (also max CI `8.432238578796387`). Independent check: `…67859` (3 ULPs, same cause) |
| Expected deaths | `0.07062668276950532` | sum of p; not a count prediction |
| Actual victims | **none** | 100 rolls evaluated, all `roll ≥ p`; no boundary loop |
| Survivor RD | `0.6375836886879149` | unchanged; boundary not triggered |
| Closest non-victim margin | `0.02139606510860858` | `P30-032`: p `0.0022366401030308254`, roll `0.023632705211639404` — a ~10.6× p increase would be needed to flip it |

### 5.3 Deterministic RNG vectors (exact integer)

| Case | Key | Result |
|---|---|---|
| G10 | model 1, `ADULT-MORT-P30-001`, year 1, seed 20261116 | numerator `2780209` / 16777216 = `0.165713369846344` (matches #653 exactly) |
| G11 | model 0, same key | numerator `14399697` / 16777216 = `0.8582888245582581` (matches #653 exactly) |

Model 0 is a **negative/fallback control only**; it must never be the P1 configuration.

### 5.4 Discrete assertions a compiled candidate must satisfy

1. Exactly 100 living stems enter the year; the 12 below-threshold stems are in CI, RD, growth and mortality, not filtered.
2. 1,622 directed neighbour pairs are included by the inclusive 8 m rule; the largest CI is `8.432238578796387` at `P30-053`.
3. Every stem's DBH increases, age becomes 31, height follows the Class III relative-height law, and crown relaxes toward its post-growth, pre-growth-CI target.
4. Post-growth RD is about `0.63758` (> 0.6, so the hazard branch runs) and no hazard victim is selected; the boundary loop does not run.
5. No death record, no `self-thinning` cause, no deadwood, no timber and no plan change are produced in Year 1.
6. Repeating the same clean process invocation produces byte-identical annual output; changing the seed, year or one DBH changes it.
7. The `annual` command still refuses today with `AnnualEngineNotImplemented` (exit 2) until the engine exists; after implementation it must refuse only in the specified negative cases.

### 5.5 Known test gaps in the frozen fixture (must not be hidden)

- **Mortality branch is not exercised** in Year 1 (zero victims). A separate approved higher-pressure derived fixture is required to test hazard selection, and a separate boundary/singleton probe for RD>1.
- **8 m boundary is not exercised at the exact edge**: the closest pairs are 7.9914 m (included) and 8.0048 m (excluded); the fixture has no pair at exactly 8.000000 m.
- **No work/felling**, so the post-work pre-growth population step is untested.
- **No save/replay**, so Mode A/Mode B equivalence is untested and out of scope until C-1.
- **No Unity parity**: the 30 m domain is deliberately non-Unity-parity; C-2b remains OPEN.
- **Float last bits** are runtime dependent; the Appendix A table is a candidate reference, not a bit oracle.

### 5.6 Independent cross-check (completed)

A second, independent implementation was delegated to **OpenAI GPT-6.2 Sol** (provider Codex, High effort, host `jer-laptop`) with **read-only** instructions: no writes, no build, no tests, no Unity. It recomputed the five key items from the same pinned source and fixture and returned:

| Item | Independent result | Agreement |
|---|---|---|
| G10 roll `ADULT-MORT-P30-001` / 1 / 20261116 | numerator `2780209`; `0.165713369846343994140625`; FNV32 `0x5986DA0C`, mixed state `0x2A6C3177AD653A89` | **Exact** |
| Initial N/ha, Dq, RD | `1111.111083984375`, `25.303288374529917`, `0.60979582845428804` | **Exact** |
| 1,622 directed pairs; `P30-001` CI; max CI | `1,622`; `1.1999388933181763`; `P30-053 = 8.4322385787963867` (21 neighbours) | **Exact** |
| Post-growth Dq / RD / P | `25.855791791844318` / `0.63758368868791493` / `0.093959221719787378` | **Exact** |
| MeanV / max p | `0.035410183953663112` / `0.0030975518881867859` | **2–3 binary64 ULPs**, attributed to sequential vs compensated summation; compensated summation reproduced this document's values |
| Year-1 outcome | `victims = []`; boundary not triggered; smallest `roll − p = 0.021396065108608579` at `P30-032` | **Exact to 9 significant figures** |

The independent worker also confirmed the growth path: `P30-001` DBH gain `0.70242631435394287 cm`, height gain `0.58995056152343750 m`, `H(31)=20.921672761019323 m`. This is a genuine second implementation, not a restatement: it independently derived the FNV/Mix intermediate words. The only divergence is last-bit floating-point accumulation, which is exactly why §5.5 forbids treating this table as a bit oracle and why a compiled candidate needs a **written numeric policy** before any golden comparison.

---

## 6. Deterministic test plan (for the future authorised writer)

**Tier 0 — preconditions (no code before these):**
1. Manager `[APPROVED TO START]` and a named exclusive lane (branch, worktree, Files/ext4, lease, allowed files).
2. Registered writer `[RUNNING]` with actual provider/model/host and starting HEAD.
3. Clean R clone at `35d26f5e…`; P0 subtree `f212c8f6…` and P1 subtree `fd0861ad…` read back; fixture SHA-256 read back.
4. Isolated `HOME`, `NUGET_PACKAGES`, `TMPDIR` on Files/ext4 (root `/tmp` was full in prior runs); SDK 8.0.133; no shared build server left running.
5. Written L4 development policy (Gate B).

**Tier 1 FAST — after the bounded annual writer returns:**
1. Existing P1 suite must stay **33/33 PASS**, unchanged.
2. Build: 0 warnings / 0 errors; P0 reference DLL hash remains `9a99f781…34b3`.
3. Scalar probes G01–G09 and G12 from #653 (pressure onset/cap, CI edge/floor, DBH law/clamp, height at 30, crown CI 0/5, clamp, last-survivor) — labelled source/adapter, not Unity.
4. RNG: exact G10/G11 numerators, then the whole 100-ID roll vector.
5. Year-1 golden: the aggregates in §5.2 and the per-ID table in Appendix A, within a written numeric policy; discrete outcomes must be exact.
6. Determinism: two clean process invocations byte-identical (stdout and annual record); one controlled perturbation changes it.
7. Refusals: each exact code and exit 2, with pre/post state identical; no dead ID, wrong domain, wrong year, wrong model, duplicate ID or altered fixture may advance.
8. P0 protection: subtree unchanged, P0 82 + 16 tests, original fixture hashes, L0–L3 PASS; **L4 recorded as FAIL38** (not waived, not skipped silently).
9. Atomicity: an injected failure leaves year 0 and the tree array unchanged; retry is not a second biology.
10. Independent changed-lines review by a **different provider** with a bounded read-only lane; author does not review own work.

**Tier 2 FEATURE QUALIFICATION — only after Tier 1 and any approved fixture additions:**
1. One immutable Year 0→1 candidate and repeat run from the same genesis, with a full per-ID record and state/ledger digests.
2. A separately approved **higher-pressure** fixture proving hazard victims, one death per ID, cause `self-thinning`, death year 1, survivors-only boundary, no timber.
3. An **8 m edge** fixture (exactly 8.000000 m, just above, 0.25 m floor) and an **ID-order/tie** fixture.
4. A boundary RD>1 case, including the last-survivor rule and hazard/boundary disjointness.
5. Only if C-1 is separately approved: save/replay Mode A/Mode B and failed-year atomicity. Not in this slice.
6. Unity 40/80 capture (C-6) and C-2b only under a separate owner GO; never inferred from the 30 m experiment.

**Do not**: weaken a failing test to get green; run L4 under a modified analyzer; treat a helper roll as an observed controller branch; call the 30 m result Unity parity; use the P0 inventory year 2026 or generator seed 6390001 as ecological inputs.

---

## 7. Genuine blockers (only these) and the single next action

| ID | Blocker | Why it is genuine | Exact unblock |
|---|---|---|---|
| **B-1** | No written limited-development security policy for default L4 FAIL38 | The packet forbids inferring a waiver. #715/#720 prove the 38 events are runtime-internal, but the strict analyzer stays fail-closed. #722 (whole-role proof) is blocked and nowhere permits annual development. | Owner/Manager writes one sentence: isolated local development and test execution on the exact R descendant is permitted while default L4 is reported **FAIL38**, provided no analyzer change, no protect-path write and no release/security claim. |
| **B-2** | No new physical exclusive source/test lane | R custody is retained with no active writer; a GitHub task does not grant physical permission. | Custodian `jermur788` assigns one new lane (store/worktree/branch/lease/allowed files) under the Ops board flock, preserving R. |
| **B-3** | No Manager start permit | Stage B check D; #723 has only the preparation comment. | Manager #694 posts `[APPROVED TO START #723]` naming the exact writer/lane and the allowlist. |
| **Caveat (not a blocker)** | Main canonical decision log has a **different D-084** and the P1 spec is not merged to main | Future readers can cite the wrong decision; it does not invalidate the owner approval. | When a writer starts, cite `90583811` and the Multi D-084 branch document; merge the spec/canonical row in a separate docs task. |

**One executable next action — paste this to the owner/Manager (it covers B-1, B-2, B-3 in one decision):**

> `[APPROVED TO START #723 — BOUNDED F0 ANNUAL]` I authorise one isolated local writer on a new exclusive Files/ext4 lane (R `35d26f5e` descendant, P1 `Tools/MultiRackSitkaP1Lab/**` + its tests only, P0/Assets/specs/fixture read-only) to implement the approved one-year adult annual step and its Tier-1 tests. Isolated development and test execution may proceed while default .NET L4 is reported **FAIL38**; the strict analyzer, P0 immutability, no-release claim and no-Unity-parity rules are not waived. Custodian assigns the lane; writer registers `[RUNNING]` before any write.

After that, the next step is one writer (prefer OpenAI Codex GPT-6.2 Sol High) and one independent reviewer (different provider, read-only), with the file group and Tier-1 plan above. Nothing else is required.

---

## 8. Status ledger (what may and may not be claimed)

| Item | Status |
|---|---|
| Four-file P1 specification | **APPROVED SPEC** (owner, 2026-11-16 18:56 UTC) — not merged to main |
| Bounded F0 one-year annual | **APPROVED IN SCOPE**, **NOT IMPLEMENTED** |
| 30 m fixture identity | **ACCEPTED (identity only)** |
| Year-1 oracle in this document | **PROPOSED reference** — independent transcription, not compiled/Unity verified |
| Annual engine | **NOT IMPLEMENTED / NOT REVIEWED / NOT VERIFIED** |
| Default L4 | **FAIL38 / SECURITY BLOCKED / NO WAIVER** |
| P0 | **UNCHANGED** (52 files, 98 tests, L0–L3 previously PASS, L4 FAIL38) |
| Unity / Windows / save / HTTP | **NOT TOUCHED / NOT AUTHORISED** |
| Manager integration / main merge | **NOT REQUESTED** |

---

## 9. Custody, preservation and evidence

- **R preserved:** `35d26f5e64a00a400c700871927ccc2173370568`, tree `33f84db2e7fdac527c343a758775757f49687c42`, clean, P0 `f212c8f6…` 52 files, P1 `fd0861ad…` 27 files, fixture `bcaaa5c3…757c4`, four CCF blobs pinned. No clone, build, test or write was performed against it.
- **Not touched:** #697/#692/#703/#713/#720 stores and evidence, #722 (blocked), #724 (separate mission), canonical/legacy stores, `CCF-Primary` user-dirty `AGENTS.md` and `Docs/Operations/StorageArchitecture.md`, all Unity lanes, Scenario One, saves, Reference Future v1.
- **Physical Ops:** no board mutation was made. R row remains `BLOCKED / CUSTODY RETAINED / NO ACTIVE WRITER`, advisory expiry 2026-11-19 18:00 UTC; do not auto-delete or silently inherit.
- **Evidence root for this Stage A:** the two Markdown files on this branch. No raw local artifacts are claimed as durable evidence; every number is reproducible from the pinned source and fixture described in §5.1.
- **No dashboard/Drive sync is claimed.**

---

## Appendix A — candidate per-ID Year 1 reference (100 rows)

Format: `DBH0`/`DBH1` cm, `H1` m, `crown1` m, `CIpre`, `S`, `p`, `roll`. Values are the independent transcription of §5; 6 decimal places are display only. The pipe-delimited canonical table (`id|age0|age1|dbh0_cm|…|victim`, `%.9g`) used for the transcription artifact has SHA-256 `79d63de470d982bcb01c180035cbdf594f0edd2992c884bdaf48213d1bb59dc6`; it is an identifier for this transcription, **not** an acceptance vector.

table sha256 79d63de470d982bcb01c180035cbdf594f0edd2992c884bdaf48213d1bb59dc6 lines 101
| Tree | age0→1 | DBH0 (cm) | DBH1 (cm) | H1 (m) | crown1 (m) | CIpre | S | p | roll | victim |
|---|---|---|---|---|---|---|---|---|---|---|
| P30-001 | 30→31 | 32.900002 | 33.602428 | 23.659950 | 3.359647 | 1.199939 | 0.193540 | 2.79851e-05 | 0.165713 | — |
| P30-002 | 30→31 | 18.200001 | 18.778030 | 19.137177 | 2.183709 | 3.805781 | 0.432191 | 0.000695896 | 0.558678 | — |
| P30-003 | 30→31 | 32.200001 | 32.843811 | 23.444580 | 3.269346 | 1.818760 | 0.266729 | 0.000100953 | 0.720913 | — |
| P30-004 | 30→31 | 23.900000 | 24.512579 | 20.890907 | 2.620296 | 2.843878 | 0.362560 | 0.000344636 | 0.575026 | — |
| P30-005 | 30→31 | 29.900000 | 30.512012 | 22.736937 | 3.073999 | 2.360945 | 0.320739 | 0.000211081 | 0.304164 | — |
| P30-006 | 30→31 | 19.299999 | 19.902529 | 19.475616 | 2.273679 | 3.356443 | 0.401659 | 0.000519125 | 0.581356 | — |
| P30-007 | 30→31 | 24.100000 | 24.730093 | 20.952440 | 2.643179 | 2.609989 | 0.342969 | 0.000275968 | 0.517415 | — |
| P30-008 | 30→31 | 18.900000 | 19.512577 | 19.352549 | 2.247809 | 3.252028 | 0.394088 | 0.000481077 | 0.135457 | — |
| P30-009 | 30→31 | 30.299999 | 30.961885 | 22.860004 | 3.132526 | 1.776093 | 0.262112 | 9.41425e-05 | 0.747573 | — |
| P30-010 | 30→31 | 20.299999 | 20.982830 | 19.783289 | 2.378999 | 2.300482 | 0.315114 | 0.000196657 | 0.770880 | — |
| P30-011 | 30→31 | 21.700001 | 22.337521 | 20.214027 | 2.466053 | 2.709565 | 0.351455 | 0.000304312 | 0.103333 | — |
| P30-012 | 30→31 | 28.600000 | 29.174656 | 22.336964 | 2.955207 | 2.952597 | 0.371275 | 0.000378984 | 0.116986 | — |
| P30-013 | 30→31 | 23.100000 | 23.623663 | 20.644768 | 2.521937 | 4.252145 | 0.459585 | 0.000889822 | 0.570917 | — |
| P30-014 | 30→31 | 28.000000 | 28.532000 | 22.152359 | 2.888024 | 3.646631 | 0.421740 | 0.000630987 | 0.435887 | — |
| P30-015 | 30→31 | 31.299999 | 31.853006 | 23.167677 | 3.147050 | 3.019795 | 0.376543 | 0.000400957 | 0.698417 | — |
| P30-016 | 30→31 | 26.799999 | 27.354891 | 21.783154 | 2.810131 | 3.398046 | 0.404623 | 0.000534619 | 0.210012 | — |
| P30-017 | 30→31 | 29.500000 | 30.063293 | 22.613867 | 3.016954 | 3.033108 | 0.377576 | 0.000405376 | 0.991600 | — |
| P30-018 | 30→31 | 15.200000 | 15.676293 | 18.214163 | 1.932495 | 6.001620 | 0.545521 | 0.0017664 | 0.293442 | — |
| P30-019 | 30→31 | 23.500000 | 24.074806 | 20.767838 | 2.573719 | 3.394143 | 0.404347 | 0.000533158 | 0.263040 | — |
| P30-020 | 30→31 | 17.200001 | 17.807184 | 18.829506 | 2.120380 | 3.465311 | 0.409354 | 0.000560064 | 0.731471 | — |
| P30-021 | 30→31 | 28.700001 | 29.346119 | 22.367729 | 3.000620 | 2.065275 | 0.292313 | 0.000145625 | 0.562122 | — |
| P30-022 | 30→31 | 29.200001 | 29.764416 | 22.521564 | 2.994958 | 3.043713 | 0.378397 | 0.000408911 | 0.770897 | — |
| P30-023 | 30→31 | 24.400000 | 24.897705 | 21.044743 | 2.605746 | 4.604061 | 0.479387 | 0.00105338 | 0.084387 | — |
| P30-024 | 30→31 | 29.700001 | 30.208199 | 22.675402 | 3.001912 | 3.884351 | 0.437213 | 0.000728806 | 0.272705 | — |
| P30-025 | 30→31 | 26.900000 | 27.410488 | 21.813921 | 2.795583 | 4.118723 | 0.451678 | 0.000830147 | 0.385045 | — |
| P30-026 | 30→31 | 13.800000 | 14.207016 | 17.783422 | 1.812343 | 8.046181 | 0.616746 | 0.0028858 | 0.387661 | — |
| P30-027 | 30→31 | 26.200001 | 26.718296 | 21.598551 | 2.747701 | 4.048885 | 0.447446 | 0.00079947 | 0.568504 | — |
| P30-028 | 30→31 | 20.900000 | 21.399662 | 19.967890 | 2.351407 | 4.916713 | 0.495801 | 0.00120523 | 0.152582 | — |
| P30-029 | 30→31 | 31.700001 | 32.270481 | 23.290743 | 3.187541 | 2.739083 | 0.353929 | 0.000312971 | 0.874587 | — |
| P30-030 | 30→31 | 29.400000 | 30.033937 | 22.583099 | 3.047753 | 2.145817 | 0.300290 | 0.000162182 | 0.683349 | — |
| P30-031 | 30→31 | 13.100000 | 13.622574 | 17.568050 | 1.795735 | 5.228222 | 0.511156 | 0.00136162 | 0.802347 | — |
| P30-032 | 30→31 | 14.700000 | 15.143649 | 18.060328 | 1.886840 | 6.867479 | 0.578681 | 0.00223664 | 0.023633 | — |
| P30-033 | 30→31 | 29.400000 | 29.924698 | 22.583099 | 2.988528 | 3.633531 | 0.420863 | 0.000625752 | 0.808652 | — |
| P30-034 | 30→31 | 27.799999 | 28.308266 | 22.090826 | 2.860999 | 4.070028 | 0.448734 | 0.000808715 | 0.843144 | — |
| P30-035 | 30→31 | 28.700001 | 29.210148 | 22.367729 | 2.928665 | 3.948411 | 0.441242 | 0.000756043 | 0.108125 | — |
| P30-036 | 30→31 | 27.900000 | 28.416466 | 22.121592 | 2.872606 | 3.916349 | 0.439232 | 0.000742366 | 0.696800 | — |
| P30-037 | 30→31 | 28.600000 | 29.143490 | 22.336964 | 2.938786 | 3.408622 | 0.405372 | 0.000538588 | 0.520113 | — |
| P30-038 | 30→31 | 15.100000 | 15.540845 | 18.183395 | 1.914375 | 6.897608 | 0.579747 | 0.00225318 | 0.099854 | — |
| P30-039 | 30→31 | 32.200001 | 32.771530 | 23.444580 | 3.226213 | 2.681163 | 0.349057 | 0.000296091 | 0.148775 | — |
| P30-040 | 30→31 | 13.800000 | 14.352091 | 17.783422 | 1.854725 | 4.617994 | 0.480141 | 0.00106002 | 0.967155 | — |
| P30-041 | 30→31 | 28.000000 | 28.633558 | 22.152359 | 2.940486 | 2.260580 | 0.311350 | 0.000187428 | 0.808980 | — |
| P30-042 | 30→31 | 30.100000 | 30.665405 | 22.798470 | 3.063386 | 2.950048 | 0.371073 | 0.000378162 | 0.296111 | — |
| P30-043 | 30→31 | 27.600000 | 28.115229 | 22.029291 | 2.849737 | 3.966906 | 0.442394 | 0.000763973 | 0.863004 | — |
| P30-044 | 30→31 | 34.000000 | 34.518299 | 23.998388 | 3.330152 | 3.296377 | 0.397327 | 0.000497089 | 0.923272 | — |
| P30-045 | 30→31 | 26.000000 | 26.496117 | 21.537016 | 2.722301 | 4.473570 | 0.472216 | 0.000991752 | 0.727353 | — |
| P30-046 | 30→31 | 30.900000 | 31.439085 | 23.044607 | 3.108900 | 3.263998 | 0.394966 | 0.000485377 | 0.560398 | — |
| P30-047 | 30→31 | 15.000000 | 15.425760 | 18.152628 | 1.902637 | 7.330884 | 0.594514 | 0.00249166 | 0.895257 | — |
| P30-048 | 30→31 | 23.900000 | 24.419003 | 20.890907 | 2.578630 | 4.258151 | 0.459935 | 0.00089254 | 0.341720 | — |
| P30-049 | 30→31 | 23.700001 | 24.248631 | 20.829370 | 2.577015 | 3.776422 | 0.430292 | 0.000683743 | 0.771871 | — |
| P30-050 | 30→31 | 26.900000 | 27.549753 | 21.813921 | 2.864780 | 2.164259 | 0.302091 | 0.000166109 | 0.778465 | — |
| P30-051 | 30→31 | 28.299999 | 28.917103 | 22.244661 | 2.954779 | 2.429877 | 0.327041 | 0.000228166 | 0.995709 | — |
| P30-052 | 30→31 | 26.799999 | 27.348787 | 21.783154 | 2.807113 | 3.491436 | 0.411171 | 0.000570076 | 0.755875 | — |
| P30-053 | 30→31 | 13.100000 | 13.497924 | 17.568050 | 1.760544 | 8.432239 | 0.627761 | 0.00309755 | 0.285513 | — |
| P30-054 | 30→31 | 24.200001 | 24.709692 | 20.983206 | 2.596491 | 4.397861 | 0.467964 | 0.000956512 | 0.521218 | — |
| P30-055 | 30→31 | 16.400000 | 16.854542 | 18.583368 | 2.011246 | 6.396095 | 0.561253 | 0.00197914 | 0.153535 | — |
| P30-056 | 30→31 | 33.599998 | 34.143135 | 23.875320 | 3.315420 | 2.953776 | 0.371368 | 0.000379365 | 0.729886 | — |
| P30-057 | 30→31 | 30.200001 | 30.734560 | 22.829237 | 3.053778 | 3.399450 | 0.404723 | 0.000535145 | 0.349905 | — |
| P30-058 | 30→31 | 19.799999 | 20.293243 | 19.629452 | 2.269170 | 5.157222 | 0.507739 | 0.00132557 | 0.855814 | — |
| P30-059 | 30→31 | 20.900000 | 21.441294 | 19.967890 | 2.367897 | 4.153980 | 0.453790 | 0.000845782 | 0.431994 | — |
| P30-060 | 30→31 | 24.600000 | 25.230766 | 21.106276 | 2.681180 | 2.562244 | 0.338821 | 0.000262857 | 0.884969 | — |
| P30-061 | 30→31 | 26.200001 | 26.822617 | 21.598551 | 2.798232 | 2.532727 | 0.336230 | 0.000254909 | 0.391321 | — |
| P30-062 | 30→31 | 23.700001 | 24.245106 | 20.829370 | 2.575459 | 3.833155 | 0.433951 | 0.000707299 | 0.042558 | — |
| P30-063 | 30→31 | 21.799999 | 22.307453 | 20.244795 | 2.420038 | 4.675752 | 0.483244 | 0.0010877 | 0.277337 | — |
| P30-064 | 30→31 | 19.200001 | 19.694084 | 19.444847 | 2.226085 | 5.200689 | 0.509837 | 0.00134762 | 0.622556 | — |
| P30-065 | 30→31 | 24.000000 | 24.515680 | 20.921673 | 2.584495 | 4.308087 | 0.462833 | 0.000915244 | 0.732312 | — |
| P30-066 | 30→31 | 21.700001 | 22.203444 | 20.214027 | 2.411111 | 4.762770 | 0.487850 | 0.00112976 | 0.793348 | — |
| P30-067 | 30→31 | 17.000000 | 17.464909 | 18.767971 | 2.057558 | 6.077431 | 0.548632 | 0.00180703 | 0.516998 | — |
| P30-068 | 30→31 | 34.900002 | 35.442181 | 24.275293 | 3.413777 | 2.847944 | 0.362890 | 0.000345894 | 0.085170 | — |
| P30-069 | 30→31 | 25.400000 | 25.948624 | 21.352413 | 2.702919 | 3.621564 | 0.420059 | 0.000620985 | 0.351563 | — |
| P30-070 | 30→31 | 22.799999 | 23.444008 | 20.552467 | 2.551400 | 2.546493 | 0.337441 | 0.000258601 | 0.383804 | — |
| P30-071 | 30→31 | 22.500000 | 23.117971 | 20.460167 | 2.517788 | 2.888711 | 0.366183 | 0.000358619 | 0.883549 | — |
| P30-072 | 30→31 | 25.200001 | 25.762772 | 21.290878 | 2.694681 | 3.422619 | 0.406360 | 0.000543859 | 0.838852 | — |
| P30-073 | 30→31 | 27.900000 | 28.440224 | 22.121592 | 2.884803 | 3.524233 | 0.413437 | 0.000582744 | 0.428466 | — |
| P30-074 | 30→31 | 24.000000 | 24.531260 | 20.921673 | 2.591441 | 4.035139 | 0.446605 | 0.000793479 | 0.397315 | — |
| P30-075 | 30→31 | 18.400000 | 18.875656 | 19.198711 | 2.161711 | 5.679985 | 0.531835 | 0.00159568 | 0.922318 | — |
| P30-076 | 30→31 | 33.299999 | 33.835484 | 23.783020 | 3.287917 | 3.095498 | 0.382373 | 0.000426372 | 0.705294 | — |
| P30-077 | 30→31 | 28.299999 | 28.812332 | 22.244661 | 2.900136 | 3.949245 | 0.441294 | 0.0007564 | 0.179122 | — |
| P30-078 | 30→31 | 33.200001 | 33.735439 | 23.752253 | 3.280317 | 3.105518 | 0.383136 | 0.000429788 | 0.176166 | — |
| P30-079 | 30→31 | 22.200001 | 22.740459 | 20.367865 | 2.462964 | 4.047862 | 0.447383 | 0.000799023 | 0.822326 | — |
| P30-080 | 30→31 | 16.799999 | 17.367735 | 18.706436 | 2.077668 | 4.088746 | 0.449869 | 0.000816931 | 0.383811 | — |
| P30-081 | 30→31 | 28.200001 | 28.880934 | 22.213894 | 2.980482 | 1.740759 | 0.258244 | 8.87075e-05 | 0.072335 | — |
| P30-082 | 30→31 | 18.500000 | 19.042339 | 19.229479 | 2.192866 | 4.357595 | 0.465675 | 0.000937932 | 0.033681 | — |
| P30-083 | 30→31 | 24.100000 | 24.653574 | 20.952440 | 2.608806 | 3.661913 | 0.422760 | 0.000637115 | 0.592821 | — |
| P30-084 | 30→31 | 25.500000 | 26.059107 | 21.383181 | 2.715289 | 3.450985 | 0.408353 | 0.000554605 | 0.666210 | — |
| P30-085 | 30→31 | 27.000000 | 27.543089 | 21.844687 | 2.819183 | 3.562125 | 0.416033 | 0.000597518 | 0.876979 | — |
| P30-086 | 30→31 | 22.900000 | 23.428194 | 20.583233 | 2.509203 | 4.191679 | 0.456030 | 0.000862608 | 0.755918 | — |
| P30-087 | 30→31 | 33.299999 | 33.849834 | 23.783020 | 3.296774 | 2.884188 | 0.365819 | 0.000357196 | 0.399777 | — |
| P30-088 | 30→31 | 26.600000 | 27.139421 | 21.721619 | 2.787619 | 3.657428 | 0.422461 | 0.000635314 | 0.255225 | — |
| P30-089 | 30→31 | 24.600000 | 25.177944 | 21.106276 | 2.656995 | 3.253411 | 0.394190 | 0.000481573 | 0.905924 | — |
| P30-090 | 30→31 | 26.900000 | 27.587429 | 21.813921 | 2.883582 | 1.771608 | 0.261623 | 9.34424e-05 | 0.728886 | — |
| P30-091 | 30→31 | 23.000000 | 23.764727 | 20.614002 | 2.618826 | 1.342129 | 0.211621 | 4.00016e-05 | 0.706070 | — |
| P30-092 | 30→31 | 14.200000 | 14.786740 | 17.906490 | 1.894054 | 4.015912 | 0.445425 | 0.000785125 | 0.482508 | — |
| P30-093 | 30→31 | 25.000000 | 25.632736 | 21.229345 | 2.712295 | 2.507088 | 0.333963 | 0.000248104 | 0.373229 | — |
| P30-094 | 30→31 | 27.100000 | 27.743162 | 21.875454 | 2.876738 | 2.222138 | 0.307684 | 0.000178756 | 0.485518 | — |
| P30-095 | 30→31 | 26.900000 | 27.527664 | 21.813921 | 2.853773 | 2.416383 | 0.325817 | 0.000224768 | 0.377894 | — |
| P30-096 | 30→31 | 30.500000 | 31.116640 | 22.921539 | 3.122478 | 2.257078 | 0.311018 | 0.000186629 | 0.233388 | — |
| P30-097 | 30→31 | 27.799999 | 28.420069 | 22.090826 | 2.918298 | 2.434641 | 0.327473 | 0.000229372 | 0.225818 | — |
| P30-098 | 30→31 | 15.600000 | 16.119665 | 18.337231 | 1.974865 | 5.044948 | 0.502237 | 0.00126904 | 0.227153 | — |
| P30-099 | 30→31 | 22.000000 | 22.637512 | 20.306328 | 2.488542 | 2.686133 | 0.349478 | 0.000297522 | 0.684470 | — |
| P30-100 | 30→31 | 21.700001 | 22.451191 | 20.214027 | 2.512960 | 1.542949 | 0.235819 | 6.16812e-05 | 0.387251 | — |
