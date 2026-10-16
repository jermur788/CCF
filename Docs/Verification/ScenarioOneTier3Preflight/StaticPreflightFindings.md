# Tier-3 static preflight findings

Read-only audit of every verification/build tool that participates in the
upcoming Tier-3 qualification, at candidate
`971c5f8cc75e571a159a5f38415d0005fdf30ae4`. **Unity was not run and no
production file was changed.** Static suites were executed; their results are in
§4 and in `PreflightManifest.json`.

Verified environment: Python 3.12.3; .NET SDK 10.0.112 (`dotnet`); no `mono`
(the offline C# suites compile with `dotnet`); Unity editor
`/media/jer/ZX20/Unity/6000.6.0f1/Editor/Unity` present; both licence trees
present (`/home/jer/.var/app/com.unity.UnityHub/config/unity3d/Unity/licenses`
and `/home/jer/.config/unity3d/Unity/licenses`).

---

## 1. Scenario selector / phase token audit — CLEAN

Canonical lists (code):

- density phases (`DensityQualification.cs:45`, mirrored in
  `qualification_results.py:6`): `RATIOS,ROLLS,ANCHORS,SITKA100,SITKACOMP,PARITY,CURVE,FAIRNESS,FAIRNESSHIGH,CONTINUITY,FINALBROADLEAF,LATEMIX,SAVELOAD,PERF,LONGRUN`
- short selectors (`DensityQualification.cs:46`): `A,B,C,D,E,F,G,C1,G1`
- UX phases (`UxQualification.cs:86`), 14 tokens incl. `F10_INPUT`

Every `CCF_QUAL_PHASES`, `CCF_QUAL_SCENARIOS` and `CCF_UXQ_PHASES` occurrence in
tools, tests and live documented commands is a member of its canonical list.
No obsolete token. The historical full-name selector defect is already corrected
in docs (`CanopyPulseRootCause.md`: `B,F` / `C,G`); the reduced-model offline
CLI still requires full fixture names by design and warns against copying them
into `CCF_QUAL_SCENARIOS`.

**Empty values fail closed:**

- Density: `CCF_QUAL_PHASES=` / empty `CCF_QUAL_SCENARIOS` are rejected
  **before Unity launch** (`run_qualification.py` preflight → `density_request`
  → `ValueError`). Covered by unit tests.
- UX: an empty/unknown `CCF_UXQ_PHASES` is rejected **after launch** by
  `UxQualificationPhases.Select` throwing in the C# harness. It still fails the
  run, but the Phase-2 handoff wording "fail explicitly before Unity launch in
  the runners" is **true only for the density runners** — the UX runner has no
  pre-launch phase validation. (Finding G1; documentation/gap, not a wrong
  token.)

---

## 2. Hardcoded hash / candidate-SHA audit

### Correct and current

- `DensityQualification.cs:90` Legacy40 anchors `7E57B9DAEF5BF4D0` /
  `35E2BF1C2F55C2DE` (Enlarged80 measured, not an accepted anchor).
- `MixedDensityVerification.cs:54` same Legacy40 pair; `:57` Enlarged80 pair
  explicitly labelled *observed, not accepted*.
- `N3/run_n3_offline.py:15` SHA-256 of `ScenarioOneCompletionRules.cs`
  (`862eda3ffd…`) — matches the base and passes.
- `MixedDensityOfflineCheck.cs:49-51` Sitka class / legacy decision block /
  stand relative-density fingerprints — pass.
- `Tooling` receipts: `qualification_results.py` `DENSITY_SCENARIOS` matches the
  C# long-run fixture names (self-test asserts this).

### Retired / observation-only (no action)

- `572025058B227E92` (obsolete expected mixed-world hash) appears **only** in the
  removal note `Phase2ToolingHandoff.md:10`; no code/assertion anywhere.
- `009588E5ACCB4F8F` (observed U4 Y25 mixed hash) is **never** an expectation;
  three doc mentions say it is observation-only.

### Stale / documentation-only (Finding G2, low risk)

- `F7C2FC966816BBAD` is the **pre-UX-patch** P3 hash, superseded by
  `AF7405918195F0DE` in the active `run_s1a_gates.py:62`. It still appears as if
  current in `Docs/Project/unity-project-overview.md:330,343,368,379`,
  `Docs/Verification/Enlarged80Calibration/ImplementationHandoff.md:75`,
  `…/evidence-manifest.json:130`, and several historical `p3-results.json`.
  These are documentation/historical records, not Tier-3 gate inputs. `run_s1a_gates.py`
  itself is an S1-A regression driver, not part of the Tier-3 gate set.

### Scripts that assume an old candidate (Finding B4)

- `Tools/Verification/ScenarioOneIntegration/check_sources.py:6` pins
  `BASE=c1564b85…` and a fixed accepted-source list (`:11`) that predates the
  candidate-v2 canopy fix. At the candidate SHA it **fails**:
  `AssertionError: ('unowned production change', 'Assets/ForestPrototype/MixedDensityModel.Composite.cs')`
  (that file's canopy fix `639ae2c` has no owner in the list). Do **not** use it
  as a Tier-3 gate.
- `Tools/Calibration/OakBeechGrowth/check_production_v1.py` is the **retired**
  pre-density check and fails on `ForestEcologyController.cs` (documented in
  `IntegrationHandoff.md:62`). Superseded by `check_integrated_v1.py` (passes).
- `Tools/Verification/ThreeSpecies/Qualification/run_v3_remaining.sh` edits two
  production constants in `MixedDensityModel.Composite.cs` and restores them with
  `git checkout`. Do **not** run it on the clean candidate.

---

## 3. Stale paths, main-SSD outputs, licence handling, duplication

### Stale paths (Finding B5 / G4)

- `Tools/Build/S1B/with_demo_hdr_quarantined.py:33` hardcodes
  `pgrep … 'projectPath /home/jer/CCF-s1b'`. It is the wrong worktree; the guard
  can neither detect the real Editor nor (if an Editor exists on that old path)
  *abort* a run. Used by `run_v3_remaining.sh` and `run_u4.sh`, **not** by
  `build_linux.py`.
- `Tools/Verification/ScenarioOneCompletion/{P2,P3}/OfflineLogic/run_offline_check.py:33`
  default `--csproj /home/jer/CCF-main/Assembly-CSharp.csproj` (main SSD,
  different worktree). Overridable; the runbook passes `--unity` instead.
- `Tools/Verification/inspect_final_presentation_inputs.py:15` sources a
  dependency from `/home/jer/CCF-main/…` (stage-local only). Not a Tier-3 gate.

### Main-SSD output defaults (Finding G5)

`/tmp` (main OS SSD) is the default scratch for the offline runners and
type-check (`tempfile.gettempdir()`), and `/tmp/opencode/…` for
`run_final_presentation.py`, `inspect_final_presentation_inputs.py`,
`run_scenario_one_economy_gate.py` and two `.cs` harnesses. The runbook sets
`TMPDIR` under ZX20 and always passes explicit `--out` to keep evidence on ZX20.
`build_linux.py`, `run_linux_player_smoke.py` and the three `run_qualification.py`
copies default their outputs to ZX20/explicit `--out`.

### Licence-dir handling (Finding B3)

- ThreeSpecies and UX `run_qualification.py` copy `--license-dir` into the
  isolated config (default `~/.config/unity3d/Unity/licenses`).
- The base `Tools/Verification/Qualification/run_qualification.py` (used by
  `run_u4.sh`) has **no** `--license-dir`; `run_u4.sh` relies on a prior `compile`
  step to seed `$OUT/run/config`. The runbook avoids the base runner.
- `run_unity_gates.py:74`, `run_model2.py:13`, `WindthrowV1/run_regression.py:7`,
  `UnderstoreyRecruitment/run_regression.py:7` set `XDG_CONFIG_HOME` but **never
  copy a licence**; run cold, Unity can fail with "No valid Unity Editor license".
  Pre-seed the config or use the ThreeSpecies/UX runner.
- Documentation passes the Flatpak path
  (`…/.var/app/com.unity.UnityHub/…`) while runner defaults use
  `…/.config/unity3d/…`. Both exist here; the runbook fixes one explicit value.

### Accidental Tier-3 duplication (Finding G6)

Three near-identical `run_qualification.py` copies have drifted:
`Tools/Verification/Qualification/` (no `--license-dir`, project-scoped
`pgrep`), `ThreeSpecies/Qualification/` (has `--license-dir`, project-scoped
`pgrep`), `ScenarioOneUx/Qualification/` (has `--license-dir`, **machine-wide**
`pgrep`). Also two U4 harnesses exist: `Tools/Verification/U4/U4Qualification.cs`
(used by `run_u4.sh`, env-supplied expected hash) and
`Tools/Verification/U4Guidance/U4GuidanceSmoke.cs` (used by the Phase-2 focused
rerun and this runbook). The candidate's
`Tools/Build/S1B/` tree is byte-identical between the candidate SHA and the
`tooling/linux-player-tier3` tip, so there is no Linux-build duplication.

### Vacuous-PASS risks (Finding G3 / G7)

- `evaluate_receipt` enforces `hardChecks >= 1` only for the density marker in a
  check-bearing phase. The UX runner calls `evaluate_receipt` **without**
  `--min-hard-checks` support, so a `UX_QUAL_DONE_MARKER`-terminated run with
  `hardChecks=0` would be a vacuous PASS. Normal runs report many checks.
- The broad `UxQualification` harness does **not** hard-assert that the Game view
  is really 1280×720 (it only hard-checks clipping *at* the requested width);
  `U4GuidanceSmoke` does assert actual size. Evidence PNG dimensions must be
  checked post-run (runbook §7).

### Evidence-manifest inputs

`PreflightManifest.json` records the participant tool SHA-256 list. The Tier-3
run must additionally hash every `*.log`, `results-*.json`, `markers.txt`,
`evidence/**`, `package-manifest.json`, `build-identity.json`,
`smoke-summary.json` and screenshot (runbook §12). The Phase-2 manifests are
correct patterns to follow.

---

## 4. Static (Non-Unity) test results

All run at the candidate SHA, outputs under
`/media/jer/ZX20/CCF-Temp/tier3-preflight/tests/` (raw evidence). The candidate's
worktree contains a *full* `Assets`/`Packages`/`ProjectSettings` checkout for
these runs.

| Suite | Command (abbrev) | Result |
|---|---|---|
| Runner unit tests | `python3 -B -m unittest discover -s Tools/Verification/Qualification -p 'test_*.py'` | **22/22 PASS** (incl. 28 C# policy cases) |
| F10 classifier self-test | `run_f10_self_test.py` | **13/13 PASS** + 2 flow wiring PASS |
| Three-species offline | `run_offline_check.py --suite all` | density **91 PASS**; composite density **107 checks, 3 FAIL**; timber **71 PASS**; history **74 PASS**; 2 isolated compiles **PASS** |
| N3 offline | `run_n3_offline.py` | **88/88 PASS** |
| UX offline | `run_ux_check.py` | **176/176 PASS** |
| Fixture controls | `check_fixture_controls.py` | **9 PASS + 7 static contracts** |
| Type-check prod | `run_typecheck.py --mode prod` | **CLEAN** 7 distinct errors = baseline 7 |
| Type-check tools | `run_typecheck.py --mode tools` | **CLEAN** 13 distinct errors vs baseline 43, 0 new |
| Linux build tests | `test_build_linux.py` | **22/22 PASS** (README says 20) |
| Growth integrated | `check_integrated_v1.py` | **360 checkpoints / 7 protected files PASS** |
| Site infrastructure | `run_site_check.py` | **63 PASS**, 1250 profiles neutral |
| P2 offline | `P2/…/run_offline_check.py --unity …` | **P2_OFFLINE_PASS** |
| P3 offline | `P3/…/run_offline_check.py --unity …` | **P3_OFFLINE_PASS** |
| Composite mutations | `run_composite_mutations.py` | all observed mutants **KILLED**; control reproduces the same **3 known R2 failures** |
| `check_sources.py` | static ownership | **FAIL** — stale candidate-v1 list (B4) |
| `check_production_v1.py` | retired check | **FAIL** as documented (`ForestEcologyController.cs`) |

### The 3 composite-density offline failures — ACCEPTED KNOWN C-LEVEL LIMITATION

**Manager adjudication (recorded): ACCEPTED KNOWN C-LEVEL LIMITATION.**

`COMPOSITE_DENSITY_FAIL checks=107 failures=3` on the reduced simulator:

| fixture | failed quantity | observed | criterion |
|---|---|---|---|
| Legacy40 `B_BEECH_DENSE` | max annual all-stem loss after Y25 | 0.423 | 0.40 |
| Legacy40 `F_BEECH_YOUNG` | max annual all-stem loss after Y25 | 0.407 | 0.40 |
| Enlarged80 `F_BEECH_YOUNG` | max annual all-stem loss after Y25 | 0.417 | 0.40 |

The criterion remains **0.40**. The three results remain **failures /
non-passes in evidence**; the numbers are recorded exactly as observed and no
threshold, calibration or production value was changed to remove them.

(candidate-v2's canopy fix removed the earlier L40 young-Oak and E80 dense-Beech
failures: Phase-1 recorded 5, now 3.) R1/R3/R4 pass; all mixed checks and
pure-Sitka identities pass. `THREE_SPECIES_OFFLINE` and the composite mutation run confirm the
oracle still detects real regressions.

These are deterministic **reduced-model** screening results using an external
recruitment series, not Unity runtime behaviour. They are carried explicitly and
are **not** silently folded into the accepted dense-Beech variability caveat.
Adjudicated and accepted as a known C-level limitation; still reported as
non-passes wherever the reduced-model evidence is cited.

---

## 5. Accepted limitations vs blockers

### Accepted limitations (carry forward, do not re-litigate)

- Native F10 and native Windows input remain UNVERIFIED; Linux Editor synthetic
  F10 is `PASS_WITH_UNVERIFIED` by design.
- U4 `V1_world_hash_Y25_OBSERVED` is a measurement only; final current-model hash
  adjudication is the Tier-3 decision, not a new anchor.
- Linux Player smoke is bounded: same-build save/load, ~600 s timeout, no FPS
  benchmark, no native-Windows/distribution claim.
- Type-check is an approximate 2021.3-surface compile, not Unity's compiler.
- `F7C2FC966816BBAD` remains in historical documentation (G2); it is not a gate.
- Single non-reproducible post-verdict shutdown crash is annotated, not waived;
  the raw non-zero exit is retained.

### Blockers / prerequisites (resolve or accept before a one-shot run)

- **B1 — full worktree.** Tier-3 must run in a full checkout at the candidate
  SHA. In the default sparse Non-Unity worktree (`Docs`, `Tools`, `CCF-Ops`),
  `Assets/` is absent: the offline C# suites cannot compile production sources,
  the build/Unity runners refuse to start, and `run_typecheck.py --mode tools`
  produces a **false** NEW_ERRORS cascade (354 spurious `CS0246`). All static
  results above were produced after materialising `Assets/Packages/ProjectSettings`.
- **B2 — machine-wide lock.** `build_linux.py` has no Editor guard; the other
  runners guard only per-project (or per-install for UX). Hold an explicit lock.
- **B3 — licence seeding.** Every XDG-isolated runner must have a licence
  (explicit `--license-dir`, or pre-seeded config).
- **B4 — EXCLUDED tools.** `check_sources.py`, `check_production_v1.py` and
  `with_demo_hdr_quarantined.py` are **EXCLUDED from Tier-3** (see §6).
- **B5 —** do not wrap Tier-3 steps with `with_demo_hdr_quarantined.py`; it is
  excluded and targets `/home/jer/CCF-s1b`.
- **B6 — model-2 new-game gate pending.** `feature/scenario-one-model2-default`
  is the same SHA as the candidate; §9 cannot pass until that branch lands and
  `RngModelPolicyVerification`/new-game anchors are updated together.
- **B7 — reduced-model R2 misses.** **Adjudicated: ACCEPTED KNOWN C-LEVEL
  LIMITATION** (see §4). Recorded values 0.423 / 0.407 / 0.417 against the
  unchanged 0.40 criterion; they remain failures/non-passes in evidence.

### 6. Tools EXCLUDED from Tier-3 (explicit)

The following are **not** Tier-3 gates and must not be used in a Tier-3 run:

| Tool | Reason |
|---|---|
| `Tools/Verification/ScenarioOneIntegration/check_sources.py` | stale candidate-v1 ownership list; fails on `MixedDensityModel.Composite.cs` |
| `Tools/Calibration/OakBeechGrowth/check_production_v1.py` | retired; superseded by `check_integrated_v1.py` |
| `Tools/Build/S1B/with_demo_hdr_quarantined.py` | wrong worktree `/home/jer/CCF-s1b`; can abort a wrapped run |

`run_v3_remaining.sh` is also not part of Tier-3 (it mutates production
constants).

### Bounded tooling bugs

Documented, not auto-fixed (each encodes an acceptance/ownership decision or a
documentation judgement, so they are not "purely mechanical" and none is on the
critical Tier-3 path):

- G1 UX runner lacks pre-launch `CCF_UXQ_PHASES` validation (fails closed post-launch).
- G2 stale `F7C2FC966816BBAD` in project documentation.
- G3 broad UX harness does not hard-assert actual 1280×720.
- G4 stale `/home/jer/CCF-s1b` path in `with_demo_hdr_quarantined.py:33`.
- G5 main-SSD `/tmp` default outputs across many runners.
- G6 three drifted `run_qualification.py` copies.
- G7 UX runner cannot enforce a minimum hard-check count.

None of these changes candidate behaviour or thresholds; the runbook works
around all of them.
