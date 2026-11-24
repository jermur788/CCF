# MULTI Phase 2 — delta verification: is mutation m18 now killed?

**Reviewer:** Claude Sonnet 5.5 (`claude-sonnet-5-5`), independent reviewer. I did not write the three new tests. This is a fresh run, not a copy of my earlier review's numbers. This file is the only thing I publish. I did not commit to the feature or docs branch, and I did not touch the parent checkout.
**Scope (narrow, as briefed):** one claim and two sensitivities, plus the spec amendment and the frozen anchors. Nothing else is re-reviewed.

```
DELTA VERIFICATION
Feature branch: feat/multi-phase2-oak-beech-20261124-v1
Verified HEAD:  8894ab866706a500a5fa5ce5d7c6e6e14a1998fc   (confirmed live on GitHub during the run and again just before publishing)
Reviewed code head (earlier review, 4c74533e): 401dab1835128f70d1ecc28f54ae5db8ac68bf69
Docs branch:    docs/multi-phase2-oak-beech-spec-20261124-v1 @ 0bb49dfe3aa714407043a7a9820719a584296b7f
m18:            KILLED  (by P2-06b, and only by P2-06b, for the intended reason)
P2-04b:         NOT VACUOUS (fails when a boundary-sliver point is accepted)
P2-07b:         NOT VACUOUS (fails under three different planted-promotion breaks; one of them no older test sees)
Production change 401dab18..8894ab86: NONE (nothing under src/, Assets/ or ProjectSettings)
Spec amendment: documentation-only as worded; accurate (one precision note, §6)
Build/test:     0 warnings / 0 errors; 245 / 245 twice; identical PASS sets
Frozen anchors and Q6: all reproduced, unchanged
Verdict:        PASS
```

## 1. What changed since the reviewed head

`git diff --name-status 401dab18 8894ab86`:

| File | Change |
|---|---|
| `Docs/Handoffs/MultiRack-Simulation-20261116/phase2-oak-beech/PHASE2-STEPB-HANDOFF-20261124.md` | added, +152 |
| `Tools/MultiRackSitkaP1Lab/tests/Core.Tests/Phase2Tests.cs` | +187 / −0 |

- **Nothing under `src/`.** `git diff --name-only 401dab18 8894ab86 -- Tools/MultiRackSitkaP1Lab/src Assets ProjectSettings` prints no path. The only code-side change is the test file.
- **Pure additions.** `numstat` is `187 0`. The test file has exactly three hunks (`@@ -563,0 +564,74`, `@@ -790,0 +865,48`, `@@ -849,0 +972,65`), which is 74 + 48 + 65 = 187 lines. They are the three new `T.Add` calls (`P2-04b`, `P2-06b`, `P2-07b`). No existing assertion, helper or tolerance moved.
- Three commits: `b565abf0` handoff (docs), `97e0eed2` the three tests, `8894ab86` the `P2-04b` name reword.

## 2. Method

All commands ran in WSL with `~/.dotnet/dotnet` 8.0.134 and `TMPDIR=/tmp/dv/tmp`, outside any repository, `--disable-build-servers --no-incremental`.

- **Clean copy.** A sparse checkout, detached at `8894ab866706a500a5fa5ce5d7c6e6e14a1998fc`, of `Tools/MultiRackSitkaP1Lab/`, `Tools/MultiRackSitkaLab/` (the P0 tree) and the four `Assets/ForestPrototype/` files the lab links or pins, with `.p0ref/` copied in (its sha256 is `9a99f781…`, the P0 pin). It has a real `.git`, because the P0 source-trust tests call `git cat-file` and `git rev-parse HEAD:…`. Its objects are shared with a base clone through `objects/info/alternates`.
- **Mutations.** Each ran on its own fresh copy of that tree: one exact single-site string replacement (asserted to occur exactly once), a clean build, the **full** suite, then the copy was deleted. The feature worktree (`git status` empty, `HEAD 8894ab86` before and after) and the parent checkout were never written to.
- **Controls.** The unmodified copy is the baseline of §3.

## 3. Reproduced build and test counts, and frozen anchors

| Check | Result |
|---|---|
| Live refs | feature `8894ab86…` ✓; docs `0bb49dfe…` ✓ (via the GitHub API); the delta branch did not exist |
| Clean rebuild | **0 Warning(s), 0 Error(s)** |
| Suite run 1 | **245 / 245 passed, 0 failed** |
| Suite run 2 | **245 / 245 passed, 0 failed** |
| Run-to-run | the sorted `PASS`/`FAIL` name sets are identical (diff empty); 596 output lines each |
| Composition | 242 at `401dab18` + 3 new = 245 |
| New tests | `P2-04b`, `P2-06b`, `P2-07b` all PASS |

Anchors, read from the lines the suite prints (not from constants in the code), identical in both runs:

| Anchor | Expected | Observed |
|---|---|---|
| Single-area Model 0 | `7e3a2a86…` | `7e3a2a869ec1662458b74875cadee1e4b16cf2b87b0aaa05df2d9a0061311ece` ✓ |
| `0/0/0/0` | `a692a6d5…` | `a692a6d5bc374dd0225153b3bf0e0b0e9467d3c041b0a6b03414f42d3c02233d` ✓ |
| `0/0/1/0` | `814e8165…` | `814e81650b62428510fe60c3142b51d9055537d6014303074f08733bf8d46287` ✓ |
| `1/1/1/0` | `8a926776…` | `8a926776ffa3ca3dd1611c2e4d462f8c82b4d78f29ad5e83e5a8f8ce1d2b87fb` ✓ (printed twice) |
| `1/1/1/1` | `c4b670ed…` | `c4b670ede36ef133fb7ab9b43992319b340f27827ce85025ae68444c63d2dc35` ✓ (also as `1/1/1/1/0`) |
| P0 Core pin | `9a99f781…` | `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3` ✓ |
| Q6 `1/1/1/1/1` vs `1/1/1/1/0` | equal, `776a68df…` | both `776a68df048a640c849801e743db3372d9d6d76808828d0e0623222ae35ce22d` ✓ |
| `MA-1-MIXED` 30-year fingerprint | `93e9abda…` | `93e9abda0670cb7a7f56fda91cc1e96e8bc931b330f57d99ec1b588600b9251f` ✓ |

## 4. The m18 verdict: **KILLED**

**Mutation** (`Phase2Annual.cs`, `AdvancePlantedJuveniles`, the vegetation-loss guard): `if (vegetation > 0f && Phase2Random.Roll(…JuvenileVegetationKey…) < vegetation)` becomes `if (vegetation > float.MaxValue && …)`. `p_veg` is still computed and stored (`LastVegetationLossProbability`), but it can never kill a planted juvenile. This is the intent of m18 in my earlier review; the text of my edit may differ from the earlier one.

| | Unmodified | m18 |
|---|---|---|
| Build | 0 / 0 | 0 / 0 |
| Suite | 245 / 245 | **244 pass, 1 fail** |
| Failing test | none | **`P2-06b` only** |
| `P2-06b` unweeded arm | alive 152/200 (76.0 %); deaths browse 6, **vegetation 42**; account `VegetationDeaths` 42 | alive 193/200 (96.5 %); deaths browse 7, **no vegetation deaths**; account `VegetationDeaths` 0 |
| `P2-06b` spot-cleared arm | alive 177/200 (88.5 %); browse 7, vegetation 16; account 16 | alive 193/200 (96.5 %); browse 7; account 0 |
| 30-year fingerprint | `93e9abda…` | `93e9abda…` (unchanged, as in the earlier review) |

**Failure message, verbatim:** `assertion failed: the unweeded arm records at least one vegetation-attributable death (0)`.

**Intended reason, not an incidental crash.** The suite ran to completion (245 results). The one failure is the first `P2-06b` assertion that the mutation violates: the unweeded arm shows zero `DeathCause = "vegetation"` stems and a `VegetationDeaths` account of 0, so the two arms are identical (193 / 193). Nothing threw, and no other test (`P2-06 S-01`, `P2-07b`, `P2-10`, …) was disturbed. In the unmodified run the same test shows the effect the criterion asks for: 42 vs 16 vegetation deaths, and patched survival 177 > 152.

## 5. Sensitivity of the other two new tests

Each mutation ran alone on its own copy, with the full suite.

| ID | Mutation (single site) | Result | Failing tests | Reason |
|---|---|---|---|---|
| `s04` | `Phase2Planting.cs:54`: `if (locate(order.XM, order.ZM) is null)` becomes `… is null && order.OrderId.Length < 0` (the `Phase2PointWithoutGroundCell` refusal never fires, so a sliver point is accepted) | 244 / 1 | **`P2-04b` only** | `expected refusal Phase2PointWithoutGroundCell, but the call succeeded` |
| `s07a` | `Phase2Annual.cs`: `if (Phase2Juvenile.CanPromote(species, height, cellLight))` gets `&& height < 0f` (planted promotion unreachable) | 242 / 3 | `P2-07 P-01`, `P2-07 P-02`, **`P2-07b`** | `P2-07b`: `the planted Oak is alive in year 24 (died of light)`. The juvenile passes 3.5 m, never promotes, and eventually dies, so the test fails before its "promotes within 40 years" line |
| `s07b` | `Phase2Annual.cs`: `PL-*` placed at its 2.5 m **cell centre** instead of its exact X/Z | 244 / 1 | **`P2-07b` only** | `PL-* at the exact planted X/Z`. **`P-01` and `P-02` pass** because they plant at 3.75 and 6.25, which are already cell centres. This is the evidence that `P2-07b` adds sensitivity the older tests cannot supply |
| `s07c` | `Phase2Annual.cs`: a planted promotion consumes 1 of the natural 5 m block credit of its block | 243 / 2 | `P2-07 P-01`, **`P2-07b`** | `P2-07b`: `natural 5 m block credit equals the unplanted twin, year 9` (year 9 is the promotion year) |

Disclosure: my first `s07c` edit had an extra `)` and did not compile (0 tests ran). That was my typo, not a result. The row above is the corrected rerun.

`P2-07b`'s printed outcome in the unmodified run: `PJ0001 promoted to PL-PJ0001 in ecological year 9 at H 3.750 m, age 12, cell light 0.683`. Year 9 is the year my earlier probe predicted for a bright Area B cell.

## 6. The spec amendment (`0bb49dfe` against `8cec7c4d`)

The docs branch has one commit on top of the approved spec: `0bb49dfe docs(multi-phase2): Step B follow-up amendment record (…)`. It changes two files.

| Check | Result |
|---|---|
| Files changed | `specs/multi-phase2-oak-beech-v1/spec.md` (+17 / −0) and `specs/multi-phase2-oak-beech-v1/checklists/requirements.md` (+4 / −1). `plan.md` and `tasks.md` untouched |
| `spec.md` | one hunk, `@@ -23,0 +24,17 @@`: a pure insertion of the "Amendment record" appended after the approval record. **The spec body above it is byte-unchanged** (zero lines removed) |
| Checklist | the single deleted line is G05 `[ ]` → `[x]`; three rows added (`P2-04b`, `P2-06b`, `P2-07b`). No existing criterion text edited |
| Rules, constants, tolerances, populations, hashes | **none changed.** The added lines contain no 40/64-digit hash and no new numeric constant; the numbers they quote are observations (1236 / 32608, 152 / 177 of 200, year 9) |
| "No production code" | holds: §1 above shows the only code-side change is the three tests |

**`Phase2PointWithoutGroundCell` description against the code.** The amendment says a `Plant`, `Shelter` or `SpotClear` point that is strictly inside the polygon but in no in-polygon 2.5 m ground cell is refused, and the whole batch with it. The code (`Phase2Planting.cs:52–56`) checks `StrictlyInside` first (`Phase2PointOutsideArea`), then `locate(x, z) is null` → `P1Refusal("Phase2PointWithoutGroundCell", …)`, inside the per-order loop that runs **before** the `switch` on order kind, so it applies to every kind and the throw refuses the batch. `Phase2Annual.Locator` is "the lattice cell floor(x/cell), floor(z/cell) of whichever area holds it … or null when no area has that cell", which is the amendment's "the point's 2.5 m lattice cell is not an in-polygon cell of any area". The stated reason (planted-juvenile step and patch recovery undefined) matches the code: `AdvancePlantedJuveniles` would `continue` for such a juvenile (`Phase2Annual.cs:497`) and patch recovery throws `Phase2PatchOutsideGround`. The cited spec sections exist (`### 7.2 Validation`, `### 7.3 Planted juveniles`, `### 7.4 Selective ~1 m² circular spot clearance`). **Match.**

**The 3.79 % figure.** The amendment quotes "1236 of 32608 points (3.79 %)". My run prints `P2-04b sliver points 1236 of 32608 otherwise-valid scan points (3.79 %)`, in both suite runs and in every mutation run that executed. **Exact.** The amendment's "about 3 % (reviewer)" is my earlier 3.07 % and 3.42 % over a slightly different population (every strictly-inside 0.25 m point, with no ≥ 0.7 m adult filter); the two are consistent, and the 3.79 % is the one tied to a stated population.

**Other numbers in the checklist rows reproduce.** `P2-06b` "152/200 vs 177/200 over 6 years" and `P2-07b` "PJ0001 → PL-PJ0001 in ecological year 9" are both what I observed.

**Precision note (not a defect).** "Documentation only" means no code, constant, tolerance, population or hash moves, and the engine behaviour it ratifies already exists at `cc67361f`. It does still add text to the spec: a ratified `§7.2` row and three acceptance criteria. That is a spec change, owner-approved and appended after the body, not an edit of an existing rule. The wording is accurate as long as "rule" is read as "behaviour of the engine".

## 7. The eight survivors of the earlier review

| ID | What it was | Now |
|---|---|---|
| **m18** | `p_veg` can never kill a planted juvenile | **KILLED** by `P2-06b` (§4) |
| m10 | Hegyi CI over Sitka stems only | still survives: 245 / 0 |
| m21 | Oak seed cutoff 80 → 20 m | still survives: 245 / 0 |
| m22 | all species share the `STAGE5-MAST` stream | still survives: 245 / 0 |
| m29 | Oak growing-space weight 2.0 → 1.0 | still survives: 245 / 0 |
| m30 | Beech growing-space weight 1.5 → 1.0 | still survives: 245 / 0 |
| m31 | Oak survival-table entry 0.75 → 0.5 | still survives: 245 / 0 |
| m27 | same draw made directly through `SimulationRandom` | **not re-run** (see §8) |

For m10, m21, m22, m29, m30 and m31 I rebuilt a single-site equivalent from the earlier review's description. None is equivalent to no change: each moves the 30-year fingerprint away from `93e9abda…` (to `d590d6ba…`, `32a1af55…`, `8b955c7e…`, `740a8e03…`, `8a382338…` and `8192b21b…` in that order), so the suite really ran a different engine and still passed. **No survivor other than m18 is killed by the new tests.** That is expected: the three tests assert survival, refusal and promotion, not the growth, dispersal or mast constants.

## 8. What I did not verify

- **m27** (a draw made directly through `SimulationRandom`). I could not reconstruct that site from the earlier description with confidence, and the three new tests have no way to see it (the seam only sees routed draws). I report it as unchanged, not as re-run.
- **My six re-run survivors are reconstructions.** Each is a single-site edit of the described kind, not the earlier review's byte-identical edit. Only m18's intent is the same by construction; the other five confirm "still survives" for the described error.
- **Mutation coverage.** Four sensitivity edits and one claim edit show sensitivity to those specific errors, not completeness of the three tests. I did not mutate the `P2-04b` batch-injection (Execute) path separately, only the validator it shares.
- **Test robustness.** `P2-06b` is deterministic over 200 fixed area seeds with the default pressure; I did not vary the seeds, the horizon or the pressure.
- **The handoff `b565abf0`.** I confirmed it exists on the feature branch with a §5 (the target of checklist G05) and that it is the only non-test file added. I did not review its content, and I did not check its four open owner decisions.
- **The spec beyond the amendment**, `plan.md`, `tasks.md`, `Docs/Project/**`, and the earlier review's findings F4–F10, which this delta does not address.
- **Anything in Unity, the `/multi` UI, a browser, a save, or any protected port**, and cross-OS or CI determinism. Two suite runs and eleven mutation runs on one host.
- **Whether the owner accepts the amendment or ticks the checklist rows.** That is theirs. I ticked nothing.
- **Other processes on the host.** About 25 `sitkap1-ui` processes from earlier days were already running on this machine. None started during my runs and I did not touch them.

reviewed head 8894ab866706a500a5fa5ce5d7c6e6e14a1998fc, m18 KILLED, verdict PASS
