# CCF Multi 2D — S0b Independent Review (RG-S0b)

## Review metadata

| Item | Value |
|---|---|
| Reviewed candidate (exact HEAD) | `6835e27407b4dc5da0ce782832fcf21206094d7e` (`feat/multi-2d-s0b-engine-seams-20261125-v1`, reviewed detached in a clean worktree) |
| Base | `2af3fb1ca3c97fe85735522ca4a4aa6d7b60cb88` (= `origin/integration/2d-sim` after S0a); `git merge-base HEAD 2af3fb1c` = `2af3fb1c` |
| T0 | `47b27a8d3873b32726f315ce1182118da38c50b1` |
| Spec | `SPEC-multi-2d-s0-seam-extraction-V1` (`specs/multi-2d-s0-seam-extraction-v1/`, APPROVED) — §4.2–§4.6, §6, §8 S0b, §9 S0-06/07/08/REG, §11, §12, §16; `tasks.md` T-B1..T-B4, RG-S0b; `checklists/requirements.md` |
| Reviewer | Claude Opus 5.5 (`claude-opus-5-5`) via Claude Code, acting as the RG-S0b review sub-agent for coordinator thread `f011851b-…`; effort **high** (cap respected, not escalated) |
| Date (UTC) | 2026-11-26 |
| Independence | The reviewer did not author, edit, or advise on any S0b commit. The author was Gemini 4 Neon (XHigh) on a different thread. Every author claim was treated as unverified. Nothing on the feature branch was edited. This file is the reviewer's only Git write. |
| Review worktree | `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-2D-S0b-Review-20261125` (clean, detached at HEAD during every run) |
| Raw evidence (external, not in Git) | `/home/jer/Development/CCF-Multi-2D-S0b-Evidence-20261125/{review-gate, review-logs, review-selftest-run2, review-g6, review-mp-mutant, review-mutants}` |
| P0 seed | `CCF_P0REF_DIR=/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-2D-S0b-20261125/Tools/MultiRackSitkaP1Lab/.p0ref`; `sha256sum SitkaLab.Core.dll` = `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3` (verified before the gate, and again after it in both the seed and the review worktree's `.p0ref`) |

## 1. Diff scope

`git diff --name-only 2af3fb1c..6835e274` lists 20 paths. Every one is under `Tools/MultiRackSitkaP1Lab/`, `Docs/` or `specs/multi-2d-`. The out-of-scope filter (`grep -v -E '^(Tools/MultiRackSitkaP1Lab/|Docs/|specs/multi-2d-)'`) returned **NONE**. `git diff --stat 2af3fb1c..HEAD -- Tools/MultiRackSitkaP1Lab/src/Core/Phase1` is **empty**. There are no `Assets/`, `Packages/`, `ProjectSettings/`, `src/Ui/wwwroot/**`, `fixtures/**`, `Docs/Project/**`, `/multi` JS, UI or test-assertion changes. Test-source changes are limited to the new `tests/Core.Tests/RngSaltsTests.cs` (+219) and one `RngSaltsTests.Register();` line in `tests/Core.Tests/Program.cs`. There is no merge to `main` and no merge commit on the branch.

Commits (9): `93735e5a` RngSalts, `2e2a027d` IFeatureBlockCodec, `3bc9bd67` AnnualPipeline routing, `1c662ae3` MIN-2 selftest, `c10c5806` MIN-1 spec wording and frozen contracts, `16852075` NB1c selftest retry, `f6467f70` handoff, `a5648122` TMPDIR scoping, `6835e274` handoff update.

## 2. Criterion-by-criterion results

| ID | Verdict | Exact command(s) | Observed output |
|---|---|---|---|
| **Gate on exact HEAD** | **PASS** | `Tools/MultiRackSitkaP1Lab/build/2d-gate.sh --base 2af3fb1ca3c97fe85735522ca4a4aa6d7b60cb88 /home/jer/Development/CCF-Multi-2D-S0b-Evidence-20261125/review-gate` (01:17:03–01:21:05Z) | **exit 0**. `GATE G0 PASS`; `GATE G1 PASS (P0 REFERENCE: PRE-SEEDED VERIFIED 9a99f781…)`; `GATE G1 PASS`; `GATE G2 PASS`; `NETWORK ISOLATION: PROVEN`; `GATE G3 PASS`; `GATE G4 PASS`; `GATE G5 PASS`; `GATE G6 PASS`; `GATE G7 PASS`; `GATE B NOT RUN`; `VERDICT: PASS (exit=0; …)` |
| G2 suite counts | PASS | same run | Core **282/282, 0 failed** (incl. 5 `S0b RngSalts` tests and `PASS P1-TH-02`); Save **50/50**; ui-smoke-multiarea **363 passed / 0 failed**; ui-smoke **250 passed / 4 failed**, `FAIL` IDs exactly `L01, L02, L06, J31`, `G11b PASS`; cli-safety **23/0**; build-output-safety `C1-C6+P0 PASS; HOST-NA: C7-cold-build,C8-rebuild; .p0ref restored & re-verified`; p0-isolation `PASS (decoy=13/13, protected hits=0, unresolved=0)`; S0a `/multi` asset identity PASS; move-proof (`--part all`) PASS |
| G3 / G4 / G5 / G6 / G7 | PASS | same run | `9/9 frozen SHA-256 anchors verified`; `v2 golden verified (bytes=24703, fileSha=c0ea8a460d9e..., checksum=9629ac1648fa..., SchemaVersion=2, src/Core/Phase1 unchanged)`; `5/5 PASS: M11, W137, W149, W258, M13`; `68 files scanned; F1=0, F2_allowed=4, F2_unlisted=0, RNG_patterns=9, RNG_violations=0`; `48 paths checked; wwwroot tree=dabdbd7fa07c...; P0 tree=f212c8f64fb2...` |
| **S0-06** (EXTRACTED; pipeline order; only M1/M2; anchors, golden and P1-TH-02 unchanged; reviewer-confirmed record) | **PASS** (judged by the reviewer's own equivalence analysis; see §3 and finding NB-1 on the tool) | `python3 Tools/MultiRackSitkaP1Lab/build/2d-gate/move-proof.py --base 2af3fb1c… --head HEAD --member MultiAreaContinuation.Advance --allow M1,M2`; `python3 …/move-proof.py --base 47b27a8d… --head HEAD --part all`; reviewer's normalised statement-multiset diff (`rv-eq.py`, §3.2); manual reading of `AnnualPipeline.cs` and `Advance` | `MOVE-PROOF MEMBER PASS: MultiAreaContinuation.Advance mode=extracted-m1-m2 relocations=M1,M2 steps=6` (rc 0); `MOVE-PROOF PASS: ui(5 files, 14 blocks, 1246 non-empty lines); core(7 files, 8 blocks, 1337 non-empty lines)` (rc 0). The multiset diff leaves only scaffolding, line-wrap and the M1/M2 moves. Gate: anchors 9/9, golden, `PASS P1-TH-02`. |
| **S0-07** (RngSalts byte-identical; no stray literal; test asserts frozen/distinct/prefix/collision/new-family) | **PASS** | `git diff 2af3fb1c..HEAD -- src/Core/{RngSalts.cs,AnnualRules.cs,MultiArea/Phase2MixedDensity.cs,MultiArea/Phase2Rules.cs,MultiArea/Stage5Mast.cs}`; gate G2/G6; extracted-G6 probes (`rv-g6.sh`); RngSalts mutants (§4) | 9 constants equal the base literals byte-for-byte. Exactly 10 changed lines (AnnualRules 1, Phase2MixedDensity 1, Phase2Rules 6, Stage5Mast 2), and each changes only the literal → constant. 5/5 `S0b RngSalts` tests PASS. G6 `RNG_violations=0`. The reviewer re-ran both mutants and both were killed (§4): the duplicate value gave `279/282 passed, 3 failed`, and the non-allow-listed prefix gave `280/282 passed, 2 failed`. |
| **S0-08** (C-V3 unchanged; zero codec implementations; SchemaVersion 2; Phase1 unchanged; no v3 code) | **PASS** | `grep -rn IFeatureBlockCodec src tests --include=*.cs` (outside the interface file); `grep -rn "SchemaVersion = 3" src tests --include=*.cs`; `git diff --stat 2af3fb1c..HEAD -- src/Core/Phase1`; `git diff --word-diff 2af3fb1c..HEAD -- spec.md`; gate G4 | 0 references, 0 implementations, 0 `SchemaVersion = 3` hits. Phase1 diff is empty. G4: `bytes=24703, fileSha=c0ea8a460d9e..., checksum=9629ac1648fa..., SchemaVersion=2`. The new G4 scanner also enforces zero implementations and zero `SchemaVersion = 3` (`2d-gate.sh:1216-1237`). The §6.6 text has no diff hunk. |
| **S0-REG** (S0b slice: zero behaviour change) | **PASS (headless)** | gate run above; diff review | No gameplay, biology, constant, RNG-stream, UI or save-format change found. Anchors, golden, counts (≥ baseline), budgets and P1-TH-02 all hold. The Edge tier B was not run; it is not required for S0b. |
| Listeners (spec §4.3) | **PASS for the gate run; see OBS-1** | `ss -ltnp` before the gate; gate `listeners-before.txt` / `listeners-after.txt` | `:33767`, `:43649`, `:46593` were listening before the gate and in the gate's before/after records (01:17–01:21Z). After that, an unrelated WSL VM restart (boot 01:35:17Z) removed them. See OBS-1. |
| **Fail-closed selftest** | **PASS** | `Tools/MultiRackSitkaP1Lab/build/2d-gate/selftest.sh /home/jer/Development/CCF-Multi-2D-S0b-Evidence-20261125/review-selftest-run2` (run 2, 01:37:58–02:25:19Z, clean detached HEAD; run 1 was killed at C10 by the VM restart, OBS-1) | **exit 0**. `Covered exit codes: [0, 1, 2, 3, 4, 5, 64]`; `OVERALL: PASS (26/26 passed, 0 failed; all exit codes 0,1,2,3,4,5,64 proven)`. There were three NB1c retries: `C14-missing-marker` needed attempts 2/3 **and 3/3**, and `C25-g7-denylist` needed attempt 2/3 (see 7a). The real worktree status was empty afterwards. |
| **MIN-1** (wording-only reconciliation) | **PASS** | `git diff --word-diff=plain 2af3fb1c..HEAD -- specs/multi-2d-s0-seam-extraction-v1/spec.md` | Changes touch only the header provenance sentence, §4.5 (self-test description), §9 S0-04, §11 "Protected paths", and §16 (relabelled columns plus a new "Editorial Reconciliation" row). S0-04 and §11 now list the same allowed roots, forbidden roots and deny-list as §4.2 G7 and §16, so they are mutually consistent. No section is renumbered. §6.6 (C-V3, C-ECON, C-FENCE) is untouched. `frozen-contracts-s0b.md` carries an explicit NON-AMENDMENT NOTICE, and its restatement matches §6.5/§6.6/§12/§13 in substance (it cites `MultiFenceMembers` at its post-S0a home `src/Ui/MultiRoutes.cs:30`, which is correct). |
| **MIN-2 C16** seed symlink → 3 | **PASS** | selftest run 2; `case-C16-*/gate.log` | rc 3. `ERROR [G1]: invalid P0 seed (…/seed-symlink): seed file is a symlink: …/SitkaLab.Core.dll`; `GATE G1 FAIL` |
| **MIN-2 C17** wrong-SHA seed → 3 | **PASS** | same | rc 3. `seed SitkaLab.Core.dll sha256=ec0ac50d… != expected 9a99f781…`; `GATE G1 FAIL` |
| **MIN-2 C18** missing provenance → 3 | **PASS** | same | rc 3. `seed missing file: …/seed-missing-prov/provenance.txt`; `GATE G1 FAIL` |
| **MIN-2 C19** invalid provenance → 3 | **PASS** | same | rc 3. `seed provenance.txt missing exact line: p0QualifiedTree=f212c8f6…`; `GATE G1 FAIL` |
| **MIN-2 C20** 5th ui-smoke failure → 1 | **PASS** | same | rc 1. `ui-smoke.sh unexpected failure IDs: ['Z99']`; `GATE G2 FAIL`, G3–G7 `NOT RUN` |
| **MIN-2 C21** asset identity → 1 | **PASS** | same | rc 1. ui-smoke `PASS-BASELINE-DRIFT`, then `ERROR [G2]: S0a /multi asset identity failed`; `GATE G2 FAIL` |
| **MIN-2 C22** move-proof block tamper → 1 | **PASS** | same | rc 1. `ERROR [G2]: S0a move-proof failed (see g2-move-proof.log)`; `GATE G2 FAIL` |
| **MIN-2 C23** G5 budget → 1 | **PASS** | same | rc 1. `GATE G2 PASS`, `G3 PASS`, `G4 PASS`, then `GATE G5 FAIL`. The planted fault reached G5 as claimed. The `ERROR [G5]:` reason text is empty (OBS-6). |
| **MIN-2 C24** G6 stray RNG literal → 1 | **PASS** | same; `case-C24-*/g6-determinism.json` | rc 1. G2–G5 PASS, then `GATE G6 FAIL`. `f1Violations=[]`, `f2Violations=[]`, `rngLiteralViolations=[{file: src/Core/MultiArea/SelftestRngLiteral.cs, line 5, literal 'ADULT-MORT-'}]`, so the failure comes from the RNG check alone. |
| **MIN-2 C25** G7 deny-list → 4 | **PASS** | same | rc 4. G2–G6 PASS, then `PROTECTED-PATH VIOLATION (1): [{'path': 'Docs/Project/selftest-denylist-violation.txt', 'reason': 'DENY_PREFIX'}]`; `GATE G7 FAIL` |
| **MIN-2 C26** clean exact-four control → 0 | **PASS** | same | rc 0. G0–G7 PASS in a fresh sparse clone; `ui-smoke.sh PASS-BASELINE-DRIFT (250 passed, 4 known T0 #885 failures: L01,L02,L06,J31; G11b PASS)`; `NETWORK ISOLATION: PROVEN` |
| MIN-2 completeness versus the S0a reviewer's list | PARTIAL (NB-3) | read `selftest.sh` against the S0A review's MIN-2 list | Hard-link seed, no-seed cold-build mismatch, and build-output-safety / p0-isolation tolerance cases are still not covered. |

## 3. DP-S0b confirmation (trigger T5)

**EXTRACTED is CONFIRMED. T5 is NOT FIRED.** The reviewer accepts the M1/M2 equivalence argument, on the following grounds.

### 3.1 Reading `Advance` (HEAD) against base `Advance` (`2af3fb1c`)

- **Guards** are unchanged and still run before any pipeline work: `RequireFence(fence)`, `WorkPendingConsentRequired`, and `ContinuationNoLivingStems`, all inside `lock (sync)`.
- **The single atomic commit is preserved.** There is exactly one `root = root with { … }`. Its field set is identical: `EcologicalYear`, `AnnualRevision`, `Areas`, `AnnualHistory`, `Pending`, `Approved`, `PlanStateRevision`, `Stage4` (`PendingPrunes` / `ApprovedPrunes` / `Deadwood` / `NextDeadwoodNumber`), `Stage5`, and `Phase2`. It still comes after `beforeCommit?.Invoke()`. The only change is that the locals are now read from `progress.*`.
- **`AnnualPipeline.cs` has no other authority.** It contains no `lock`, no `root =`, no reflection, no DI and no registration API. Steps receive `YearInProgress`, which holds the start root read-only plus scratch accumulators.
- **`YearInProgress` initialisers match the base locals.** Its constructor evaluates the same expressions in the same order: `revision`, the three arrays, `woodland` (fully qualified as `SitkaP1.Core.CrownModel.Woodland` only because the property `CrownModel` shadows the type), `stage4`, `deadwood`, `nextDeadwood`, `phase2Start`, `density`. `lostCrop`, `nextStage5` and `nextPhase2` start at their defaults (`false` / `null`), which equal the base initialisers.
- **`areaSpecs` copies the same inputs.** It projects the immutable `IReadOnlyList<AreaInfo> areas` field (`MultiAreaContinuation.cs:85`) to `(AreaId, Seed, AreaHa)` in the same index order.

**M1 (deadwood hoist) is exact.** `DeadwoodStep` runs after `AdultGrowthStep` and the empty Phase 3 slot, and strictly before `RegenerationStep`. So `progress.NextAreas[i].State` is exactly the base `stepped` object (`next = stepped`; promotions are not merged yet). The loop:

- iterates `i = 0..Count-1`, the same area index order;
- skips `Reports[i].Report is null`, which is exactly the base naturally-empty `else` branch (`MultiAreaAreaReport(area.AreaId, null)`);
- iterates `report.Deaths` in report order;
- uses the same `IdFor(NextDeadwood++)` counter, which starts at the same `Stage4?.NextDeadwoodNumber ?? 1`;
- passes the same `FallenDeadwoodRecord` arguments.

`deadwood` and `nextDeadwood` were never read elsewhere in the base loop. The only observable difference is the one the spec already records in §6.2: the test-only `afterArea` invocation count when the unreachable `byId[death.TreeId]` invariant fails. Nothing is committed in that case.

**M2 (year-record placement) is exact.** `YearRecordStep` builds `new MultiAreaYearRecord(Year, checked(Year + 1), Revision, Start.WorkRevision, Array.AsReadOnly(Records), HasWork)`. None of these inputs are written by `DeadwoodStep` or `RegenerationStep`: regeneration reassigns only `NextAreas[i]` entries, `NextStage5` and `NextPhase2`, and `Records` is never mutated after `AdultGrowthStep`.

**The reserved steps are true no-ops.** `ReservedPhase3DisturbanceStep.Run(YearInProgress year) { }` and `ReservedPhase2BYearEndSummaryStep.Run(YearInProgress year) { }` have empty bodies and no fields beyond the singleton `Instance`. T6 is not fired.

**RNG draw order is unchanged.** The only RNG consumers are inside `MultiAreaAnnual.Step` / `StepStage4`, `Phase2MixedDensity.StepRack`, `Phase2Annual.Step` and `Stage5Annual.Step`. They are called with the same arguments, in the same per-area order, then in the same single regeneration call. The keyed rolls are stateless (`SimulationRandom.Roll` / FNV-keyed streams). The deadwood and year-record code draws nothing. The lambdas `treeId => stage4.PruningOf(treeId).Lifts` capture the same start-of-year `Stage4`.

### 3.2 Mechanical cross-check (reviewer's own tool, not the author's)

`rv-eq.py` does the following:

1. It takes the base `Advance` statements from `int revision = …` up to `// Test-only fault seam`, and every statement in HEAD `AnnualPipeline.cs` from `internal YearInProgress(` onward.
2. It maps `progress.X` and the property names back to the base local names.
3. It strips declarations and keywords and compares the two statement multisets.

**Residual "only in base":** declaration-initialisers (now in the constructor), `if (stage4 is not null)` (the M1 guard, now `if (stage4 is null) return;`), the `yearRecord` construction (M2, wrapped differently), `nextStage5 = null` / `nextPhase2 = null` (now property defaults), and two `Stage5Inputs(…)` calls that differ only by line wrap.

**Residual "only in head":** class, property and singleton scaffolding; the constructor; `Steps`; the M1 lookup lines (`report = reports[i].Report; if (report is null) continue; stepped = nextAreas[i].State;`); the re-wrapped calls; and local aliases.

No logic statement of the adult step, Crop-mark pruning, regeneration, promotions or consented-cancellation clears is missing or added.

### 3.3 Behavioural cross-check

The gate on this exact HEAD shows anchors 9/9 (including RNG-dependent #4, #5, #7 and #9), the v2 golden, `PASS P1-TH-02` (skip-vs-manual full-root parity on MA-1 and MA-1-MIXED, pre- and post-Fell), and Core 282/282.

The reviewer did **not** tick the confirmation slot inside `S0B-SAFETY-VALVE-DECISION-20261125.md`. The brief permits only this one file as the reviewer's Git write. The integrator should transcribe this confirmation into §5 of that record at T-B5 (OBS-3).

## 4. RngSalts mutant kills (re-run by the reviewer)

**Script:** `rv-mut.sh`. **Evidence:** `review-mutants/`.

**Procedure.** Mutants were run in the clean review worktree *after* the gate and selftest had finished. Each mutant adds one constant and registers it in `Families`. The steps were:

1. build `tests/Core.Tests/SitkaP1.Core.Tests.csproj` (`--disable-build-servers`);
2. run `dotnet tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll`;
3. restore with `git checkout -- src/Core/RngSalts.cs`.

| Mutant | Build | Core result | Failing tests (verbatim) |
|---|---|---|---|
| Duplicate value: `Stage5MastDuplicate = "STAGE5-MAST"` | 0 W / 0 E | **rc 1, `279/282 passed, 3 failed`** | `… pairwise distinct … — assertion failed: duplicate RNG salt (ordinal): STAGE5-MAST`; `… no registered value is a prefix … — forbidden prefix pair in RngSalts.Families: "STAGE5-MAST" is a prefix of "STAGE5-MAST"`; `… validation rules … — duplicate RNG salt (ordinal): STAGE5-MAST` |
| New non-allow-listed prefix: `Phase2MastExtra = "PHASE2-MAST-EXTRA-"` | 0 W / 0 E | **rc 1, `280/282 passed, 2 failed`** | `… no registered value is a prefix … — forbidden prefix pair in RngSalts.Families: "PHASE2-MAST-" is a prefix of "PHASE2-MAST-EXTRA-"`; `… validation rules … — forbidden prefix pair …` |

**Restore.** After `git checkout`, `git status --porcelain` was empty. A rebuild plus re-run gave `282/282 passed, 0 failed`.

**Test content, verified by reading `RngSaltsTests.cs`:**

- (a) The 9 frozen literals are duplicated in the test, and the call-site helper outputs are checked (`STAGE5-PROMO-MA-A-3-7-12`, `PHASE2-PJ-PJ-001/vegetation`, …). A reflection check confirms every `const string` on `RngSalts` is in `Families`.
- (b) Distinctness is checked both ordinal and `OrdinalIgnoreCase`, and empty, whitespace and untrimmed values are rejected.
- (c) Prefix freedom uses `OrdinalIgnoreCase`. The allow-list has exactly the three ADULT-MORT pairs, and the test asserts both `allow-list length == 3` and `observed pairs == allow-list`.
- (d) The collision check uses real 30-year MA-1 (Phase 2 selection) and MA-1-MIXED runs with bright-Area-B planting. It asserts at least one `PL-` promotion, no `GENERIC-` / `QUOTA-` tree IDs and no `QUOTA-` area IDs, and that the three generated ADULT-MORT key sets are disjoint.
- (e) A clean new family (`PHASE3-STORM-`) is accepted, and empty, whitespace, untrimmed, exact-duplicate, case-duplicate and new-prefix entries are all rejected.

`T.True` / `T.Eq` throw (`tests/Core.Tests/Runner.cs:20-24`), so the `ExpectThrows` checks are meaningful.

## 5. Adversarial checks 7a–7d

### 7a — Selftest NB1c retry (`16852075`, `selftest.sh:291-316`): **ACCEPTABLE (NON-BLOCKING NB-2)**

- **When it retries.** The retry fires only when all of these hold: the case did **not** already PASS; `attempt < 3`; `$case_ev/g2-save-tests.log` exists; and that log contains both `^49/50 passed, 1 failed$` and `^FAIL NB1c: .* expected 1, got 3$`. A 49/50 Save log can exist only when G2 failed at the Save suite, and the gate stops there with exit 1. So the condition effectively means "exit 1 at G2, NB1c the sole Save failure, exact message". It does not explicitly test the exit code or a `GATE G2 FAIL` marker.
- **Why it cannot mask a regression.** A retry never converts a failure into a pass by itself. Every attempt re-runs the unmodified gate and re-asserts the case's exact expected exit code and diagnostic pattern. A deterministic regression that happened to look like NB1c would fail all three attempts and be reported FAIL. `2d-gate.sh` itself has no retry and still requires Save 50/50.
- **Consistency with MIN-3.** This matches the recorded interim rule: re-run only an *isolated* NB1c "expected 1, got 3". The script cannot know whether the host is "otherwise idle", so that part of the rule remains the operator's responsibility.
- **Weaknesses.** `rm -rf -- "$case_ev"` (`selftest.sh:296`) deletes the evidence of the failed attempt, so after a retry only the `NOTE [selftest]: retrying …` line in `selftest.log` remains. That is exactly what happened in this review's run 2: the failed attempts' Save logs for C14 and C25 no longer exist. A failed attempt's logs should be preserved, for example as `case-<id>.attempt<N>`. The condition should also assert the gate's rc 1 and the `GATE G2 FAIL` marker explicitly.
- **Observed flake rate.** In run 2, 3 of the ~16 full gate invocations hit NB1c, and C14 hit it twice in a row and passed only on its last allowed attempt. The run was otherwise idle apart from the reviewer's light Python-only probes. That is above the ~7.5% the S0a review measured. A third consecutive NB1c would have failed the selftest. This strengthens the MIN-3 recommendation: an owner- or Manager-authorised test-only repair of `NB1c` should precede any trunk dependence on freeze-on-red.

### 7b — TMPDIR scoping (`a5648122`) and the `2d-gate.sh` diff: **NO WEAKENING FOUND**

- **TMPDIR.** The G0 check that `TMPDIR` and the evidence directory are outside the repository still runs on the caller's `TMPDIR` *before* the change (`2d-gate.sh:326-347`). Only afterwards is the child `TMPDIR` set to `$SCRATCH_DIR/tmp` (`:355-356`, netns worker `:1096-1097`). `$SCRATCH_DIR` is created under the validated base and removed by the `EXIT` trap (`:218`). The same pattern is used in `selftest.sh:66-67`.
- **Other diff hunks.** The rest of the diff adds three things:
  - duplicate-`FAIL`-ID rejection for ui-smoke (`:698-699`), which is stricter;
  - the G4 zero-implementation / no-`SchemaVersion = 3` scan (`:1216-1237`), which is stricter;
  - the G6 rewrite.
- **The G6 rewrite is stronger than base.** Previously it matched only exact `"<lit>"`. Now it matches the content of every string literal (plain, verbatim, interpolated, and `$@`/`@$`) by prefix, suffix or exact key. It also fails if `RngSalts.cs` defines zero constants, if the manifest is empty, or if the manifest lists a pattern missing from `RngSalts.cs`. The F1/F2 loop is unchanged.
- **Nothing relaxed.** No allow-list was broadened (`determinism-allowlist.txt` and `protected-paths.txt` have no diff), no baseline count was lowered, and no stage marker was made optional. The selftest coverage gate tightened from 15 cases / codes `{1,2,3,4,5,64}` to exactly 26 cases / codes `{0,1,2,3,4,5,64}`.

### 7c — `baseline-counts.json` core 277 → 282: **LEGITIMATE**

The only test-source changes are the new `RngSaltsTests.cs`, which registers exactly 5 `T.Add` cases, and its one registration line. No other test file appears in the diff. The base gate (S0a review) recorded Core 277/277, and this HEAD shows 282/282 with the 5 new names visible in `g2-core-tests.log:640-644`. Raising `minPassed` is strictly stronger.

### 7d — G6 RNG-literal scanner: **GENUINELY FAILS CLOSED; scope correct**

The reviewer extracted the G6 Python from `2d-gate.sh` and ran it against copies of `src/Core` (`rv-g6.sh`; evidence `review-g6/`):

| Probe | Result |
|---|---|
| clean | rc 0 |
| `"ADULT-MORT-" + i` | rc 1 |
| `$"STAGE5-PROMO-{i}-1"` | rc 1 |
| `@"PHASE2-PJ-" + i` | rc 1 |
| `i + "/browse"` | rc 1 |
| `const string M = "STAGE5-MAST"` | rc 1 |
| literal inside a `//` comment | rc 0 (correctly ignored) |
| literals planted in `src/Core/bin/`, `src/Core/obj/` and `tests/` | rc 0 (correctly scoped out: the walk is `src/Core` only, with `bin`/`obj` pruned at `:1482`) |

Selftest C24 checks the same thing end-to-end (**PASS**, rc 1 at G6 with only the RNG violation). Known evasions that are **not** caught: split literals (`"ADULT-MORT" + "-"`) and differently-cased literals. A differently-cased key is a different, non-colliding stream. A split literal would be a deliberate evasion outside the spec's "quoted RNG key-prefix literal" wording (OBS-2).

## 6. Findings

| ID | Class | Finding | Evidence (file:line) |
|---|---|---|---|
| **NB-1** | NON-BLOCKING (significant; fix before the next packet that touches `Advance`) | **In extracted mode, `move-proof.py` does not prove "only M1/M2".** `verify_advance_extraction` checks: a list of required snippets in `Advance`; exactly one `root =`; the `Steps` order; empty reserved steps; that a few marker strings exist in `DeadwoodStep`, `YearRecordStep`, `AdultGrowthStep` and `RegenerationStep`; and that the lines outside `Advance` are identical to base. It does **not** compare step bodies with the base `Advance` statements. `--part all` now excludes T0 lines 985–1155 from the S0a multiset (`move-proof.py:250-254`), so `Advance` is no longer covered there either. The reviewer proved this with planted mutants in a throwaway sparse clone (`rv-mpmut.sh`; evidence `review-mp-mutant/`). Mutant A changed `checked(year + 1)` to `checked(year + 2)` in a `RegenerationStep` promotion: member mode **rc 0 PASS**, `--part all` **rc 0 PASS**. Mutant B reversed the `DeadwoodStep` area loop, a direct M1 violation: member mode **rc 0 PASS**. Mutant C put a statement in a reserved step: rc 1, correctly rejected. The safety-valve record's claims "T2 … verified by … move-proof.py: strictly ONLY M1 … M2" and "enforced statically by move-proof.py" (T6 is correct; T2 is overstated) are therefore not supported by the tool. The S0-06 verdict above rests on the reviewer's independent equivalence analysis (§3) and the behavioural gates, not on this tool. **Recommendation:** freeze the normalised statement content of each step body (or the whole `AnnualPipeline.cs` blob id plus the `Advance` span text) in `s0a-move-table.json`, and fail on any drift, so future trunk merges cannot silently alter `Advance` logic. Whether mutants A and B would be caught by anchors or Core tests was **NOT TESTED**. | `build/2d-gate/move-proof.py:61-221`, `:250-254`; `S0B-SAFETY-VALVE-DECISION-20261125.md:34,38` |
| **NB-2** | NON-BLOCKING | The NB1c retry deletes the failed attempt's evidence and does not assert rc 1 / `GATE G2 FAIL` explicitly (see 7a). | `build/2d-gate/selftest.sh:294-316` |
| **NB-3** | NON-BLOCKING | MIN-2 is only partly closed against the S0a reviewer's list. Not covered: a hard-linked seed (`nlink=2`); the no-seed cold-build mismatch (exit 3); and the build-output-safety and p0-isolation tolerance paths. §4.5 now says the self-test covers "invalid/symlink/wrong-SHA/missing-or-invalid-provenance seeds", which matches what is implemented, so the spec is accurate. | `build/2d-gate/selftest.sh:203-246`; S0A review MIN-2 item 2 |
| **NB-4** | NON-BLOCKING | **The file set exceeds spec §8 S0b "Owned files (exact)" and tasks T-B1..T-B4.** The extra files are `move-proof.py`, `selftest.sh`, `baseline-counts.json`, the new `rng-salts-patterns.json` (instead of `determinism-allowlist.txt`), `spec.md` and `frozen-contracts-s0b.md`. The S0a review's MIN-1/MIN-2/NIT-4 follow-ups and the S0b packet brief authorise these, and all are inside G7's allowed roots. However, the handoff says "Spec deviations: None" and does not list the file-set expansion. | `S0B-IMPLEMENTATION-HANDOFF-20261125.md:193`; spec §8 S0b |
| **NB-5** | NON-BLOCKING | `YearInProgress` carries inputs beyond "exactly the locals of today's Advance body": `HasWork`, `CrownModel`, `Stage4Options`, `Areas`, `BuildStage5Inputs` and `AfterArea`, plus a new `AnnualAreaSpec` record struct. These are read-only copies of `Advance`'s parameters and fields, needed because the steps cannot reach `this`. They add no state, ordering or draw. Recorded as a literal deviation from the §6.2 comment, which the spec marks "illustrative". | `src/Core/MultiArea/AnnualPipeline.cs:15,23-64` |
| OBS-1 | OBSERVATION | **The protected listeners disappeared because of an unrelated WSL VM restart.** `:33767`, `:43649` and `:46593` were listening before the gate (PIDs 1750123, 1806361, 839989) and in the gate's `listeners-before.txt` / `listeners-after.txt`. The WSL VM then rebooted (`uptime -s` = `2026-11-26 01:35:17`), which also killed the first selftest run partway through (at C10). After the reboot none of the three listen and the PIDs do not exist. The reviewer neither bound nor stopped them and did not restart them, since they are not the reviewer's processes. Selftest run 2 and the mutant runs therefore had no protected listeners to observe. **The owner should restart those sessions if they are still needed.** | `review-gate/listeners-*.txt`; `uptime -s` |
| OBS-2 | OBSERVATION | The G6 scanner does not catch split or differently-cased key literals (see 7d). | `build/2d-gate.sh:1473-1479` |
| OBS-3 | OBSERVATION | The reviewer confirmation slot in the safety-valve record is still unticked (the one-file write limit). The integrator should transcribe §3's confirmation at T-B5. | `S0B-SAFETY-VALVE-DECISION-20261125.md:62-71` |
| OBS-4 | OBSERVATION | The handoff's reproduction block points `CCF_P0REF_DIR` at a different worktree (`…Phase1-TimeHistoryUI…/.p0ref`). The reviewer used the S0b worktree's seed, which hashes to the same `9a99f781…`. | `S0B-IMPLEMENTATION-HANDOFF-20261125.md:201` |
| OBS-6 | OBSERVATION | When G5 fails, the gate prints `ERROR [G5]: ` with an empty reason (seen in selftest C23), so the missing or failed budget ID has to be found in the multiarea log. Still fail-closed; it is a diagnosability nit. | `review-selftest-run2/case-C23-g5-budget/gate.log` |
| OBS-5 | OBSERVATION | `RngSaltsTests` collision test (d) checks the three ADULT-MORT key families for disjointness and the `GENERIC-` / `QUOTA-` ID-prefix rule over 30-year MA-1 and MA-1-MIXED runs with `PL-` promotions, as §6.4(d) requires. It does not check cross-family disjointness for `PHASE2-*` / `STAGE5-*` generated keys. Those are covered structurally by the strict prefix-freedom test (c). | `tests/Core.Tests/RngSaltsTests.cs:75-154` |

**Blocking findings: none.** No gameplay, biology, constant, RNG, UI or save-format change was found. There are no edits outside the allowed roots, no `src/Core/Phase1` change, no v3 encoder, decoder or registry, no `/multi` JS split, and no merge to `main`.

## 7. Checklist items not verified by this review (NOT TESTED)

- Browser tier B (`--browser`, the Edge suites): **NOT TESTED**. Not required for S0b; required for S0c and S0 closure.
- Cold P0 rebuild reproducing anchor #8: **HOST-NA** on this host (SDK 8.0.134 / Ubuntu 26.04, spec §4.6). The verified pre-seed path was used.
- Whether the NB-1 mutants A and B would be caught by the Core tests or anchors: **NOT TESTED**.
- Listener continuity across selftest run 2 and the mutant runs: **NOT TESTABLE** (OBS-1).
- OQ-5 / OQ-6 approval records: not re-read. The reviewer relied on spec §16 "adopted recommended OQ-1..OQ-6 defaults".
- `requirements.md` rows outside S0b (S0-01..05, S0-09, Locked decisions A/B/D, S0 closure): out of scope.
- The "S0b merged … unlocks Phase 2B" row: not applicable until the integrator merges.
- The `requirements.md` checkboxes themselves were **not** ticked. Only this file was written.

## 8. Commands run by the reviewer (all via WSL, from the review worktree unless noted)

```bash
export DOTNET_ROOT=/home/jer/.dotnet
export PATH=/home/jer/.dotnet:/home/jer/.local/share/vite-plus/bin:/usr/local/sbin:/usr/local/bin:/usr/sbin:/usr/bin:/sbin:/bin
export CCF_P0REF_DIR=/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-2D-S0b-20261125/Tools/MultiRackSitkaP1Lab/.p0ref
sha256sum "$CCF_P0REF_DIR/SitkaLab.Core.dll"                       # 9a99f781…
git rev-parse HEAD; git status --porcelain                          # 6835e274…, empty
git diff --stat 2af3fb1c..HEAD; git diff --name-only 2af3fb1c..HEAD | grep -v -E '^(Tools/MultiRackSitkaP1Lab/|Docs/|specs/multi-2d-)'   # NONE
git diff --stat 2af3fb1c..HEAD -- Tools/MultiRackSitkaP1Lab/src/Core/Phase1                                                           # empty
ss -ltnp | grep -E ':(33767|43649|46593)\b'
Tools/MultiRackSitkaP1Lab/build/2d-gate.sh --base 2af3fb1ca3c97fe85735522ca4a4aa6d7b60cb88 /home/jer/Development/CCF-Multi-2D-S0b-Evidence-20261125/review-gate   # exit 0
python3 Tools/MultiRackSitkaP1Lab/build/2d-gate/move-proof.py --base 2af3fb1ca3c97fe85735522ca4a4aa6d7b60cb88 --head HEAD --member MultiAreaContinuation.Advance --allow M1,M2   # rc 0
python3 Tools/MultiRackSitkaP1Lab/build/2d-gate/move-proof.py --base 47b27a8d3873b32726f315ce1182118da38c50b1 --head HEAD --part all                                          # rc 0
Tools/MultiRackSitkaP1Lab/build/2d-gate/selftest.sh /home/jer/Development/CCF-Multi-2D-S0b-Evidence-20261125/review-selftest   # run 1, killed by VM restart at C10
Tools/MultiRackSitkaP1Lab/build/2d-gate/selftest.sh /home/jer/Development/CCF-Multi-2D-S0b-Evidence-20261125/review-selftest-run2   # exit 0, 26/26
git diff 2af3fb1c..HEAD -- <RngSalts.cs, AnnualRules.cs, Phase2MixedDensity.cs, Phase2Rules.cs, Stage5Mast.cs, IFeatureBlockCodec.cs, Program.cs, baseline-counts.json, rng-salts-patterns.json, 2d-gate.sh, selftest.sh, move-proof.py, s0a-move-table.json, MultiAreaContinuation.cs>
git diff --word-diff=plain 2af3fb1c..HEAD -- specs/multi-2d-s0-seam-extraction-v1/spec.md
grep -rn IFeatureBlockCodec src tests (excluding the interface file); grep -rn 'SchemaVersion = 3' src tests          # 0 .cs hits
python3 -I rv-eq.py        # normalised statement-multiset diff, base Advance vs AnnualPipeline.cs
bash rv-g6.sh              # extracted G6 scanner vs 10 planted-literal copies of src/Core (evidence review-g6/)
bash rv-mpmut.sh           # move-proof mutants A/B/C in a throwaway sparse --shared clone (evidence review-mp-mutant/; clone removed)
bash rv-mut.sh             # RngSalts duplicate / prefix mutants, build + Core run, restore, rebuild, 282/282 (evidence review-mutants/)
uptime -s                  # 2026-11-26 01:35:17 (VM restart; OBS-1)
```

The helper scripts live outside the repository (`/mnt/c/Users/hp/AppData/Local/Temp/opencode/`), and their logs are in the external evidence directory.

## INTEGRATION DECISION

**APPROVED**

On exact HEAD `6835e27407b4dc5da0ce782832fcf21206094d7e`, the reviewer independently found:

- **Gate:** `2d-gate.sh --base 2af3fb1c…` exited **0** with every G0–G7 PASS marker. Network isolation was proven, anchors 9/9, golden 24,703 B / `c0ea8a46…` / `9629ac16…`, Core 282/282 including the 5 RngSalts tests and P1-TH-02, budgets 5/5.
- **Selftest:** exited **0**, 26/26, covering exit codes {0, 1, 2, 3, 4, 5, 64}. Every new MIN-2 case was checked to fail at the claimed stage, for the claimed reason, with the claimed exit code.
- **DP-S0b:** EXTRACTED is **CONFIRMED** (T5 not fired). The reviewer's own reading and statement-multiset comparison show that `Advance` now differs from base only by the two permitted relocations M1 and M2. The reserved steps are true no-ops, the guards and the single `root = root with` commit are intact, and no RNG draw changed.
- **RngSalts:** byte-identical, with exactly 10 literal-only substitutions. Its test kills both re-run mutants.
- **IFeatureBlockCodec:** zero implementations, no `SchemaVersion = 3`, and `src/Core/Phase1` has a zero diff.
- **Spec:** §6.6 is untouched, and the MIN-1 edits are wording-only and consistent with G7.

No blocking issue was found. The most important non-blocking item is NB-1: `move-proof.py`'s member mode does not mechanically enforce "only M1/M2", as the reviewer's planted mutants demonstrate. It should be hardened before any later packet touches `Advance`. It does not invalidate this candidate, whose equivalence the reviewer established independently.

The integrator should also:

- transcribe the DP-S0b confirmation into the safety-valve record (OBS-3);
- treat the NB1c flake rate observed here as a reason to prioritise the MIN-3 test repair;
- note that the protected listeners were lost to an unrelated VM restart after the gate run (OBS-1) and must be restored by their owner.

This approval covers RG-S0b only. It is not an integration or merge. Browser tier B was not run (not required for S0b), and the cold P0 rebuild is HOST-NA on this host.
