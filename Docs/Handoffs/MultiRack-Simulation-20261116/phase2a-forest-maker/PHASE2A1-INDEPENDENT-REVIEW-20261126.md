# CCF Multi 2D — Phase 2A-1 Forest Maker Core — Independent Review

- **Date:** 2026-11-26 (gate/selftest runs finished 2026-11-27 00:59 UTC)
- **Reviewer:** Claude Opus 5.5, independent review sub-agent (did not author the change; read-only on the feature worktree)
- **Spec:** `SPEC-multi-2d-phase2a-forest-maker-V1` (`specs/multi-2d-phase2a-forest-maker-v1/`), Step 2A-1 only
- **Candidate branch:** `feat/multi-2d-phase2a1-forest-maker-core-20261126-v1`
- **Candidate HEAD reviewed:** `82bfffcd5c6bda9ea9b9ae23caf34d42215510ab` (remote ref matches)
- **Code commit:** `c335559c9a5e958027dc5290df0407687c013b54` — verified: `git diff --name-only c335559c..82bfffcd` lists only `Docs/Handoffs/MultiRack-Simulation-20261116/phase2a-forest-maker/PHASE2A1-IMPLEMENTATION-HANDOFF-20261126.md`
- **Review base:** `6d72c30538b1fbe30d3cbcf083b5b368733662b8` (`origin/integration/2d-sim` at dispatch)
- **`origin/main`:** `bdeb6a324acaf4129c10fb0846faac8b64974360` (unchanged throughout)
- **Live `origin/integration/2d-sim` at review end:** `c82e4e759c2ceef1c13ddb6a033e034097aef77e` (advanced during the review; see Conditions)
- **Review branch:** `review/multi-2d-phase2a1-forest-maker-core-20261126-v1` (from `6d72c305`; this file only)

---

## 1. Method

1. Read `AGENTS.md`, the approved spec quartet (spec §5.1–§5.5, §9 P2A-01..06, P2A-REG), and the owner's locked 2A-1 outcomes in the dispatch.
2. Read every changed source/test file in full: `ForestMakerRecipe.cs`, `ForestMakerGenerator.cs`, `ForestMakerTests.cs`, plus diffs of `RngSalts.cs`, `MultiAreaExperiment.cs`, `tests/Core.Tests/Program.cs`, `build/2d-gate/baseline-counts.json`. Cross-checked against existing Core: `MultiAreaGenerator.Next`, `Phase2Rules.cs` (`Phase2SeedRain.Maturity`, `Phase2Crown.TargetRadiusM`, seed cutoffs), `Stage5SeedRain.Maturity`, `Stage5Constants`, `Phase2Models.cs` species constants, `Stage4Models.cs` (`Phase2WithEconomy`), `RngSaltsTests.cs`, `selftest.sh` C09.
3. Scope checks with `git diff --name-only` / `--stat` on base..HEAD and on protected paths.
4. Re-ran the full browser gate and the selftest myself on the clean feature worktree, into new reviewer-only evidence directories (implementer evidence untouched).

### Exact commands (run inside WSL, detached runner `review-scratch/review-runner.sh`)

```
export DOTNET_ROOT=$HOME/.dotnet
export PATH=$HOME/.dotnet:$HOME/.local/share/vite-plus/bin:$PATH
export TMPDIR=/home/jer/Development/CCF-Multi-2D-Phase2A1-Evidence-20261126/review-scratch/tmp   # /tmp tmpfs was 93% full from other lanes
cd /home/jer/Development/CCF-HP-Worktrees/CCF-Multi-2D-Phase2A1-20261126/Tools/MultiRackSitkaP1Lab
./build/2d-gate.sh --browser --base 6d72c30538b1fbe30d3cbcf083b5b368733662b8 \
  --p0ref-dir /home/jer/Development/CCF-Multi-2D-Phase2A1-Evidence-20261126/p0ref-seed \
  /home/jer/Development/CCF-Multi-2D-Phase2A1-Evidence-20261126/review-gate
./build/2d-gate/selftest.sh \
  --p0ref-dir /home/jer/Development/CCF-Multi-2D-Phase2A1-Evidence-20261126/p0ref-seed \
  /home/jer/Development/CCF-Multi-2D-Phase2A1-Evidence-20261126/review-selftest
```

`--p0ref-dir` pointed at the external seed, never the worktree `.p0ref`. SDK resolved 8.0.134 under `global.json` latestPatch (8.0.133 not installed); C7/C8 cold-rebuild HOST-NA is the existing gate design.

### Host contention

At 21:38 UTC `pgrep -af 2d-gate` / `pgrep -af selftest` showed another lane's selftest (Species Wave 2, HEAD `6a003a30`) running. The runner polled every 60 s and started only when no `2d-gate|selftest` process existed: waited 21:41:26 → 23:08:39 UTC (gate run 1), then ran gate run 2 and the selftest on an idle host (both "host idle" immediately).

---

## 2. Observed gate and selftest results

### Gate run 1 — FAIL (reviewer harness artifact, not a candidate defect)

- Evidence: `/home/jer/Development/CCF-Multi-2D-Phase2A1-Evidence-20261126/review-gate-r1-sigign/` and `review-gate-r1-sigign.console.log`
- G0, G1 PASS; Core 298/298; Save 50/50; `ui-smoke-multiarea.sh` **360 passed, 6 failed** → `GATE G2 FAIL`, `VERDICT: FAIL (exit=1)`.
- All 6 failures were Z01 SIGINT-shutdown cases (`Z01-api`, `Z01-mixed`, `Z01-page`: "still running 15 s after SIGINT", exit 137).
- Root cause (verified): I launched the runner as `setsid nohup … &`, so the non-interactive shell started it with SIGINT ignored; `/proc/<runner>/status` showed `SigIgn: 0000000000000007` (SIGINT bit set), inherited by the servers under test. The implementer's gate-r3 had 363/0 on the same checks. My runner also mis-logged `gate exit=0` because `$?` was read after `$(date)`; the gate's own verdict line (exit=1) is authoritative.
- A selftest that had auto-started under the same signal mask was stopped by me after <2 min (my own session only) and archived as `review-selftest-r1-aborted-sigign` — not a result.

### Gate run 2 — PASS (relaunched with `setsid -f env --default-signal=INT`, `SigIgn: 0000000000000004`)

- Evidence: `/home/jer/Development/CCF-Multi-2D-Phase2A1-Evidence-20261126/review-gate/` and `review-gate.console.log`; `gate-summary.json` records `headCommit 82bfffcd…`, `baseCommit 6d72c305…`, `exitCode 0`.
- **`VERDICT: PASS (exit=0; NETWORK ISOLATION: PROVEN; P0 REFERENCE: PRE-SEEDED VERIFIED 9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3)`** (23:13:05 → 23:24:58 UTC)

| Check | Observed |
|---|---|
| G0 / G1 | PASS (3 clean builds) |
| Core.Tests | **298/298 passed, 0 failed** (all 7 `forestmaker` cases PASS) |
| Phase1.Save.Tests | **50/50** |
| ui-smoke-multiarea | **363 passed, 0 failed** |
| ui-smoke | 250 passed, 4 known T0 #885 failures {L01, L02, L06, J31}; G11b PASS |
| cli-safety | **23/0** |
| build-output-safety | PASS (C1–C6+P0; HOST-NA C7-cold-build, C8-rebuild; `.p0ref` restored & re-verified) |
| p0-isolation | PASS (decoy 13/13, protected hits 0) |
| S0c asset identity / module graph / JS reconstruct | PASS (14 modules) |
| G3 anchors | **9/9 frozen SHA-256 anchors verified** |
| G4 Save v2 | v2 golden verified (bytes=24703, SchemaVersion=2, `src/Core/Phase1` unchanged) |
| G5 budgets | **5/5** (M11, W137, W149, W258, M13) |
| G6 determinism | 76 files; F1=0, F2_unlisted=0, RNG_patterns=10, RNG_violations=0 |
| G7 protected paths | PASS (93 paths) |
| Stage B | Save Edge **63/63**, Step 2 Edge **67/67**, S0c parity PASS |

Core log reproduced the handoff values exactly: Anchor 10 content hash `d79fabccf3a9447666c436a08c04895c58934fde19c5b5aaf072103612ae4fde`, Anchor 11 `de0eb3f63d13423336348d3cd4f398b9c4f6ce23d77578e902c25960265a5945`, 5-year hashes `a0001376…dd28` (primary) and `eeb0c378…7501` (pure Sitka).

### Selftest — PASS

- Evidence: `/home/jer/Development/CCF-Multi-2D-Phase2A1-Evidence-20261126/review-selftest/` and `review-selftest.console.log` (23:24:58 → 00:59:42 UTC, exit 0)
- **`OVERALL: PASS (29/29 passed, 0 failed; all exit codes 0,1,2,3,4,5,64 proven)`**
- C09-g2-dropped-test → exit 1 as required (confirms the 298 floor).
- Five cases (C14, C23, C26, C27, C28) passed on attempt 2/3 after the pre-existing frozen NB1c OS-scheduling race (49/50, "expected 1 got 3"); the selftest's existing retry policy, no case exhausted retries.
- Note: `selftest.sh:76` takes `BASE_SHA` from live `origin/integration/2d-sim`, which by then was `c82e4e75`, not `6d72c305`. It still passed 29/29.

### Post-run state

- Feature worktree `git status --short` empty and HEAD `82bfffcd` before gate, after gate and after selftest.
- P0 seed `p0ref-seed` SHA-256 still `9a99f781…34b3`.
- `origin/main` = `bdeb6a32…` at start and end. Remote feature ref = `82bfffcd`.

---

## 3. Criteria verification

| Area | Verdict | Basis |
|---|---|---|
| **A. Scope / forbidden paths** | PASS | base..HEAD touches exactly the 12 allowed files (quartet ×4, `ForestMaker/*.cs` ×2, `RngSalts.cs`, `MultiAreaExperiment.cs`, `ForestMakerTests.cs`, `Program.cs`, `baseline-counts.json`, handoff). `git diff --stat` empty for `src/Ui`, `src/Core/Phase1`, `Assets/`, `Packages/`, `ProjectSettings/`, `Docs/Project/`, `build/2d-gate/frozen-anchors.json`, `MultiAreaContinuation.cs`, `MultiAreaGenerator.cs`, `MultiAreaEcology.cs`, `Tools/MultiRackSitkaLab/`. No biology/AnnualPipeline/economy-math file in the diff. `MultiAreaExperiment.cs` change is the null-preserving `GeneratorNoteOverride` only (MA-1 hash `c66d566c…1f95` asserted unchanged). |
| **B. Recipe / validation** | PASS | `ForestMakerRecipe.cs:101-168` checks in spec §5.1 order, no coercion; cap uses `Int128` (`:91-95`) and is checked before any tree/candidate allocation; 3,000 accepted / 3,001 refused tested (`ForestMakerTests.cs:96-117`); every bound at min/max/min−1/max+1; canonical bytes/hash stable; whitespace, trailing newline, reordered keys refused (`:157-163`). Generator is integer-only: the only `float`/`double` are summary outputs (`ForestMakerGenerator.cs:10,24,142`) copying existing `AreaHa`; `ISqrt` uses `Math.Sqrt` with exact integer correction loops. No `DateTime`/`Guid`/`Random`. Determinism and seed sensitivity tested (`:229-244`), 5-year hash repeat (`:277-279`). |
| **C. Preset geometry / counts** | PASS | Hard-coded expectations (`ForestMakerTests.cs:298-425`): lanes x=0/16/32/48/64/80 m, `"1280"` m² per lane, 1,344 = 1,336 S20 + 4 O60 + 4 B60, per-lane A 265+2+2, B/C/D 269, E 264+2+2; pure 269/269/269/269/268, empty `GenesisSpecies`; broadleaf coordinates (1,250 / 78,750 mm; y 10/30/50/70 m) and alternating species; ≥2,000 mm exclusion over all pairs; 1,024 cells 192/224/192/224/192 for both presets; strictly-inside + ≥500 mm margin, unique positions, ordinal `D4` IDs, unique lane seeds ≠ generator seed. |
| **D. Species / biology** | PASS | Starting sizes are the spec §5.3 Q1/Q2 accepted defaults (TRANSFER / GAMEPLAY CALIBRATION / RESEARCH REQUIRED, disclosed in the manifest); no new biology equation. Maturity asserted from constants: `Phase2SeedRain.Maturity(Oak/Beech, MaturityFullYears)==1` and `EdgeBroadleafAgeYears == MaturityFullYears`; `Stage5SeedRain.Maturity(MaturityOnsetYears)==0` and preset age == `MaturityOnsetYears` (=20f). Genesis crowns bit-equal `Phase2Crown.TargetRadiusM` over the connected woodland for all 8 broadleaves. |
| **E. Cross-lane ecology** | PASS (see Minor-1) | (a) MA-B border Sitka: woodland CI > lane-only CI and more neighbours (`:485-495`); (b) per-lane RD equals own-0.128 ha RD and differs from pooled by >0.1; per-lane density route (A/E mixed, B/C/D pure); (c) non-zero Oak and Beech `SeedArrival` in MA-B within 5 years; pure-Sitka control exactly 0.0 and zero bands; no stems/cells outside 80×80 m and light 1.0 outside the perimeter; (d) both presets advance 15 years. |
| **F. Economy interop** | PASS | Real generated primary preset via `CreateSession(…, MultiAreaModelSelection.Phase2WithEconomy)` (= `Economy = EconomyModelVersion.Enabled`, `Stage4Models.cs:83-84`); opening 1,200,000 cents; `EconomyFellContractorOnly` on both executor paths; quote total = `MinimumHarvestJobCents` = 250,000; approve → execute → settle 250,000; 1-year advance to Year 1 with one year-end summary; EUR 2,000 opening → `EconomyInsufficientCash` on quote and approve, cash unchanged. |
| **G. Gate honesty** | PASS | `baseline-counts.json` change is `minPassed` 291→298 only (upward); note text updated; `maxFailed`, regex, other suites untouched. Requirement confirmed by my selftest C09 (dropped test → exit 1). No other gate/selftest file changed. Core 298 > 291. |
| **RNG salt uniqueness** | PASS | `FOREST-MAKER-` appended to `RngSalts.Families`; no existing value changed; prefix-free against all entries; `RngSaltsTests` reflection/prefix checks pass; G6 RNG_violations=0. |
| **Anchors 1–9** | PASS | 9/9 verified; `frozen-anchors.json` unchanged. Anchors 10–11 pinned as test constants only, per dispatch item I. |
| **Save v2 50/50** | PASS | Observed 50/50 in gate run 2 (and run 1). |
| **I. Not accepted items** | None present | No UI, Save v3 or 2A-2 work; no weakened checks; no anchors added to `frozen-anchors.json`; preset tests are not tautological. |

---

## 4. Findings

### Critical — 0

### Major — 0

### Minor — 2

- **Minor-1 — Cross-lane seed attribution not isolated to MA-A.** `Tools/MultiRackSitkaP1Lab/tests/Core.Tests/ForestMakerTests.cs:528-552`. The MA-B seed checks sum `SeedArrival` for MA-B and build `oakSources`/`beechSources` from *all* broadleaves, including the four MA-E trees at x=78.75 m. Oak cutoff is 80 m and Beech 50 m (`Phase2Models.cs:128,150`), so MA-E Oak can reach MA-B cells too. The assertion message "from MA-A" is therefore not strictly proven. The spec requirement (P2A-06c, "non-zero oak or beech seed input from an MA-A edge broadleaf") is still met in substance: MA-A broadleaves at x=1.25 m are 16–31 m from every MA-B cell centre, well within both cutoffs, and the pure-Sitka control proves zero leakage when no broadleaves exist. *Suggested follow-up (non-blocking):* filter sources to `AreaId == "MA-A"` in the per-cell check.
- **Minor-2 — Corner matrix accepts any `P1Refusal`.** `ForestMakerTests.cs:184-197`. The 64-case loop counts any typed refusal as handled and only asserts `generated > 0 && refused > 0`, so a regression that turned a valid corner into the wrong refusal code would not fail. Individual codes are covered by P2A-01a, so this is a test-strength gap, not a defect. *Suggested follow-up:* assert the expected outcome or code per corner.

### Nit — 3

- **Nit-1** — `specs/multi-2d-phase2a-forest-maker-v1/spec.md:109,209` name `Phase2Rules.Maturity`; the method is `Phase2SeedRain.Maturity` in `src/Core/MultiArea/Phase2Rules.cs:200`. The implementation and tests use the correct symbol.
- **Nit-2** — `src/Core/RngSalts.cs:28` / `ForestMakerGenerator.cs:70`: `FOREST-MAKER-` is registered, but generation draws from integer `(seed, lane, cell, salt)` SplitMix and `StreamNamespaceKey` is referenced only by a test. The registration is a reservation, not a live string stream; the code comment discloses this. Harmless.
- **Nit-3** — `specs/multi-2d-phase2a-forest-maker-v1/checklists/requirements.md` boxes remain unchecked. I did not edit them (this review publishes one file only); §3 above is the reviewer's checklist evidence.

---

## 5. Conditions (for the integrator, non-blocking for this decision)

1. **Integration base drift.** `origin/integration/2d-sim` advanced during the review from `6d72c305` to `c82e4e75` (Species Wave 2 `6a003a30` and Woodland Importer). `6d72c305` is an ancestor and the two sides change disjoint files, but this review's gate ran against base `6d72c305`. Re-run `2d-gate.sh --browser` on the actual merge result before pushing `integration/2d-sim`; the Core floor 298 remains valid as a floor.
2. Anchors 10–11 stay test-pinned in `ForestMakerTests.cs`; promoting them into `frozen-anchors.json` needs separate owner authority.
3. Minor-1/Minor-2 may be folded into Phase 2A-2 or a later test-hardening task.

---

## 6. Not verified by this review

- Unity Editor import, compile or play mode (none in scope; this is the pure-.NET lab).
- Cold rebuild of the 8.0.133 `.p0ref` pin (HOST-NA by gate design; pre-seeded verified mode used).
- The gate/selftest on the merge result with `c82e4e75` (see Condition 1).
- Ecological realism of the Q1/Q2 starting sizes (spec-accepted defaults, flagged RESEARCH REQUIRED).
- Performance/payload for the presets in the UI (Phase 2A-2 scope).

---

INTEGRATION DECISION: APPROVED
