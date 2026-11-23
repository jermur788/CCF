# MULTI Stage 5 — Sitka regeneration engine: implementation handoff

**State: IMPLEMENTED ON TASK BRANCH. NOT NUMERICALLY QUALIFIED. NOT REVIEWED. NOT INTEGRATED. NOT OWNER-ACCEPTED.**

**STOP CONDITION TRIGGERED: S5-07.** At 2.5 m the engine produces about 2–3.5 times as many promoted trees per hectare as at 5.0 m. The approved tolerance is `[0.75, 1.33]`. Meeting it needs a change to a locked owner decision (Q5 / spec §4), so no engine change was made to chase it. The `S5-07` test is committed and **fails on purpose** so the unmet gate stays visible. The other 17 new Stage 5 tests and all 184 pre-existing tests pass.

---

## 1. Identity and provenance

| Item | Value |
|---|---|
| Worker / session | Claude Code, running in T3 Code on the owner's Windows host. WSL Ubuntu holds the lane |
| Role | Primary: spec-approval document update, pure-.NET engine implementation and numerical qualification |
| Task class / routing | `SPEC REQUIRED` / `IMPLEMENT — COMPLEX` |
| Spec | `specs/multi-stage5-sitka-regeneration-v1/` (spec, plan, tasks, checklists/requirements). Status **APPROVED**, owner approval 2026-11-23 |
| Spec URL (approved) | https://github.com/jermur788/CCF/blob/85d85a8d9bdbdf3ae867a3225597c5f084d77773/specs/multi-stage5-sitka-regeneration-v1/spec.md |
| Requested model | Claude Opus 5.5 (named in the task packet commit identity) |
| Selected / resolved / executed model | `claude-opus-5-5` (Claude Opus 5.5) for all three, as reported by the harness |
| Reasoning effort | the harness's default for this session. The setting is not visible to the worker, so no value is claimed |
| Subagents | none. The worker did all the work itself |
| Tools used | Claude Code Bash (Git Bash calling `wsl -e …`), one PowerShell call (`wsl -e git status`), and Read/Write/Edit on `\\wsl.localhost\Ubuntu\…` paths; WSL git; `/home/jer/.dotnet/dotnet` 8.0.134; WSL `python3` for one in-place text substitution in a test file; `ss`/`df` for read-only environment checks. Not used: Windows `git.exe`, Unity, a browser, GitHub PR tools |

## 2. Git and links

| Item | SHA / link |
|---|---|
| Base (owner-chosen) | `e7c1a57da519ed1ee03ccfbad240196a66df0905` (`feat/multi-stage4-ui-catchup-20261123-v1`) |
| Docs branch, draft | `docs/multi-stage5-sitka-regeneration-spec-20261123-v1` @ `6c74588e8386d11f5f4208de4b204b8b155d5acb`. Its parent is `bc8829d7`, an older `main` |
| **Docs branch, APPROVED** | `85d85a8d9bdbdf3ae867a3225597c5f084d77773`, pushed to `origin/docs/multi-stage5-sitka-regeneration-spec-20261123-v1` |
| Spec on the feature branch | `a3c3494925f80fd69773fbd52ea00b4170bfa770` (draft quartet) and `7493d66fe63f08cc58ae4d3f2847833faee92241` (approval), both cherry-picked with `-x` |
| Engine commit | `8bafda54e7c5756f279264f81a1eb8203be667fa` |
| Test commit | `0ad634ba62b04ff7a7132dd9efc6ba7123e42362`. The last code commit; every hash and test count below was measured here |
| Handoff commit | the commit that adds this file and the lane admission, on top of `0ad634ba`. The pushed branch head is reported in the push record and the final message |
| Branch | `feat/multi-stage5-sitka-regeneration-20261123-v1`. Only this branch and the docs branch were pushed |
| Compare | https://github.com/jermur788/CCF/compare/e7c1a57da519ed1ee03ccfbad240196a66df0905...feat/multi-stage5-sitka-regeneration-20261123-v1 |
| This handoff | https://github.com/jermur788/CCF/blob/feat/multi-stage5-sitka-regeneration-20261123-v1/Docs/Handoffs/MultiRack-Simulation-20261116/stage5-regeneration/MULTI-STAGE5-SITKA-REGENERATION-IMPLEMENTATION-20261123.md |
| Lane admission | https://github.com/jermur788/CCF/blob/feat/multi-stage5-sitka-regeneration-20261123-v1/Docs/Handoffs/MultiRack-Simulation-20261116/stage5-regeneration/MULTI-STAGE5-SITKA-REGENERATION-D091-LANE-ADMISSION-20261123.md |

**Why two cherry-picks and not one.** The base `e7c1a57d` does not contain the draft quartet. Cherry-picking only the approval commit (`85d85a8d`) would have been a modify/delete conflict. Both spec-only commits were therefore cherry-picked: the draft add and the approval. Together they touch only the four quartet files. No merge or rebase onto the docs branch or `main` was done.

## 3. Live coordination snapshot

| Item | Value |
|---|---|
| `active-tasks.md` | `origin/main:Docs/Project/coordination/active-tasks.md`, blob `cf44a0750ddacc94d6592f4ab4c39ed137b2dafd`. It was last changed by `37ceb264b5431fe254d67e5702af39bf66cddc1d` (2026-11-23T18:23:42Z) |
| Fetch times (UTC) | 2026-11-23T20:23:49Z at start and 2026-11-23T20:57:27Z before the handoff. No change between them; `origin/main` was `37ceb264` both times |
| Overlapping writer | none on `Tools/MultiRackSitkaP1Lab/src/Core/**`. The Owner UX lanes are UI-only and exclude `src/Core/**` |
| Lane | `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-Stage5-SitkaRegen-20261123` |
| Unity lock | untouched. No Unity process, Editor, project file or lock file was opened, started or edited by this worker |
| Protected listeners | `127.0.0.1:33767`, `:43649` and `:46593` were observed listening (`ss -ltn`) and were not touched. No server or listener was started; the UI project was compiled but not run |

## 4. Scope and cleanliness

### What changed, in plain English

- **Version switch.** There is a fourth model switch, `MultiAreaRegenerationModelVersion { Off = 0, SitkaCohortCompetitionBrowse = 1 }`, with selection order Adult / Mortality / Crown / Regeneration.
  - Supported selections: `0/0/0/0`, `0/0/1/0`, `1/1/1/0`, `1/1/1/1`.
  - Every other selection is refused with a typed code: `Stage5ModelIncomplete`, `RegenerationModelUnknown` or `Stage5ModelRequired`.
  - The default stays `0/0/0/0`. `/multi` stays on `1/1/1/0`; no UI file changed.
- **Regeneration `0` is byte-identical** to the base on every earlier selection. The tests prove this with the pinned hashes.
- **The `1/1/1/1` annual year** runs the unchanged Stage 4 adult step, then spec steps 5a–12 as one pure function, `Stage5Annual.Step`. Its inputs are the start-of-year Stage 5 state and the whole post-adult-step woodland, so area order cannot change any result. The steps are:
  1. Bramble and bracken move toward a light target.
  2. Existing bands grow and survive: light, browse (pre-growth height) and the bramble/bracken penalty.
  3. One woodland mast roll is made on `STAGE5-MAST`.
  4. Cross-rack seed rain falls from every living mature Sitka. The outer grassland adds nothing.
  5. New bands establish, using the §4 `(cell/5 m)²` scaling.
  6. Bands promote under the aligned 5 m block cap. Each new tree gets the CrownModel 1 radius `r_0` and a position strictly inside its polygon, from a `STAGE5-PROMO-{area}-{col}-{row}-{year}` stream.
  7. Site suitability is a neutral 1.0, and read-only accounts are written.
- **Adult-empty racks** keep advancing and regenerating. Year+1 is refused only when no adult is alive anywhere *and* no juvenile abundance remains.
- **`ClearUndergrowth`** is a new fenced order: Add/Remove, then Approve, then Execute.
  - It is selective: bramble and bracken go to 0 and every Sitka band is kept.
  - The engine mints the key, replay is idempotent, and the whole batch is revalidated atomically.
  - The calendar does not advance, and nothing changes before Execute.
- **State.** `MultiAreaStage5State` is one optional immutable member of `MultiAreaRoot`. It is `null` on every earlier root and is published by the existing single reference assignment. It is session-only and nothing is saved.

### `git diff --stat e7c1a57d..0ad634ba` (WSL-native)

```text
 .../src/Core/CanopyLightEvaluator.cs               |  21 +
 .../src/Core/MultiArea/MultiAreaContinuation.cs    | 175 +++++-
 .../src/Core/MultiArea/Stage5Annual.cs             | 239 ++++++++
 .../src/Core/MultiArea/Stage5Bands.cs              | 218 +++++++
 .../src/Core/MultiArea/Stage5Clearing.cs           |  52 ++
 .../src/Core/MultiArea/Stage5Juvenile.cs           |  94 +++
 .../src/Core/MultiArea/Stage5Mast.cs               |  48 ++
 .../src/Core/MultiArea/Stage5Models.cs             | 164 ++++++
 .../src/Core/MultiArea/Stage5Promotion.cs          | 109 ++++
 .../src/Core/MultiArea/Stage5Records.cs            |  92 +++
 .../src/Core/MultiArea/Stage5SeedRain.cs           |  58 ++
 .../src/Core/MultiArea/Stage5Undergrowth.cs        |  42 ++
 Tools/MultiRackSitkaP1Lab/src/Core/Stage4Models.cs |  31 +-
 .../tests/Core.Tests/Program.cs                    |   1 +
 .../tests/Core.Tests/Stage4BehaviourTests.cs       |   9 +-
 .../tests/Core.Tests/Stage5GridStudy.cs            | 105 ++++
 .../tests/Core.Tests/Stage5Support.cs              | 159 +++++
 .../tests/Core.Tests/Stage5Tests.cs                | 647 +++++++++++++++++++++
 .../checklists/requirements.md                     |  31 +
 specs/multi-stage5-sitka-regeneration-v1/plan.md   |  36 ++
 specs/multi-stage5-sitka-regeneration-v1/spec.md   | 159 +++++
 specs/multi-stage5-sitka-regeneration-v1/tasks.md  |  21 +
 22 files changed, 2488 insertions(+), 23 deletions(-)
```

The handoff commit adds only the two Markdown files in `Docs/Handoffs/MultiRack-Simulation-20261116/stage5-regeneration/`.

- **`git status --short` (WSL)** returned 0 lines after the test commit. `.p0ref/` is ignored (`Tools/MultiRackSitkaP1Lab/.gitignore:8`).
- **Allowlist check:** 0 changed paths under `Assets/`, `Packages/`, `ProjectSettings/`, `*.meta`, `src/Ui/`, `*.sln` or any save schema.
- **Not changed:** UI (`PlayerMultiApi.cs`, `PlayerApi.cs`, `Program.cs`, wwwroot), Unity files, save files, fixtures, Compact30/Legacy40/Enlarged80/Reference Future v1, and the `.p0ref` contents.
- **Two edits outside new files**, both small and additive:
  - `CanopyLightEvaluator.LightFields` is a new internal Layer-C-only helper. It reuses the existing private `Light` routine, and `Evaluate` is unchanged.
  - `Stage4BehaviourTests.RunFingerprint` is now `internal` and takes an optional selection. With no selection it runs the unchanged Stage 4 recipe.

## 5. Hashes and verification

### Commands (all from the lane, WSL)

```bash
cd /home/jer/Development/CCF-HP-Worktrees/CCF-Multi-Stage5-SitkaRegen-20261123/Tools/MultiRackSitkaP1Lab
sha256sum .p0ref/SitkaLab.Core.dll
TMPDIR=/tmp/stage5-tmp /home/jer/.dotnet/dotnet build MultiRackSitkaP1Lab.sln --disable-build-servers [--no-incremental]
TMPDIR=/tmp/stage5-tmp /home/jer/.dotnet/dotnet tests/Core.Tests/bin/Debug/net8.0/SitkaP1.Core.Tests.dll
TMPDIR=/tmp/stage5-tmp /home/jer/.dotnet/dotnet build src/Ui/SitkaP1.Ui.csproj --disable-build-servers   # compile only, not run
```

### Build and tests

| Gate | Base `e7c1a57d` (Step 2, before any edit) | Code head `0ad634ba` |
|---|---|---|
| Solution build | 0 warnings, 0 errors (`TreatWarningsAsErrors` on) | 0 warnings, 0 errors (incremental and `--no-incremental`) |
| UI project build (not in `.sln`) | not measured | 0 warnings, 0 errors. Compiled only, not run |
| Core tests | **184/184** passed | **201/202** passed: all 184 pre-existing pass; 17/18 new Stage 5 tests pass; **`S5-07 G-01` fails** (§5.3) |
| Suite runtime | about 5 s | about 40 s; the S5-07 study takes about 33 s of that |

### Protected anchors

| Anchor | Before (base) | After (`0ad634ba`) |
|---|---|---|
| Single-area Model 0 | `7e3a2a869ec1662458b74875cadee1e4b16cf2b87b0aaa05df2d9a0061311ece` | `7e3a2a869ec1662458b74875cadee1e4b16cf2b87b0aaa05df2d9a0061311ece` |
| Multi-area Model 0 | `a692a6d5bc374dd0225153b3bf0e0b0e9467d3c041b0a6b03414f42d3c02233d` | identical (default ctor, explicit `0/0/0`, explicit `0/0/0/0`) |
| Stage 3 C2 `0/0/1` | `814e81650b62428510fe60c3142b51d9055537d6014303074f08733bf8d46287` | identical (crown ctor, explicit `0/0/1`, explicit `0/0/1/0`) |
| Multi-area experiment | `c66d566c9b930ea1e4e46b2fe9a4f213cf212d787fdc752141107f5c4c0f1f95` | identical |
| P0 Core pin | `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3` | identical (`sha256sum` and the csproj pin) |
| `1/1/1/0` full run, S4 R-02 recipe (measured in Step 2) | `8a926776ffa3ca3dd1611c2e4d462f8c82b4d78f29ad5e83e5a8f8ce1d2b87fb` | identical by the default recipe and by explicit `1/1/1/0`. The same recipe on `1/1/1/1` over its 10 years, where no promotion can occur, also reproduces it exactly |
| **`1/1/1/1` full run (new)** | — | **`b48aa8fca670170adff0b26c0f701e4fa12d206ee170b787b60b9134f7cda01c`** |

The new `1/1/1/1` fingerprint recipe is `Stage5Tests.FullRun`:

- MA-1 at the default 2.5 m grid, pressure 0.2 and the generator mast seed.
- A year-0 thinning of every second stem, then one ClearUndergrowth in year 5.
- 30 years.
- Fingerprinted: every adult report, the final rosters and the full Stage 5 read-out (cells, bands, cover, clear log and accounts).

Two runs gave identical hashes. The value is recorded, not pinned.

### Qualification per criterion

| Criterion | Test(s) | Observed | Result |
|---|---|---|---|
| **S5-01** frozen paths | `S5-01 F-01/F-02/F-03`, plus the pre-existing suite | All anchors above are identical. Tokens for `0/0/0`, `0/0/1` and `1/1/1` are unchanged, and `1/1/1/1` = `ma1-connected-woodland-stage4-adult-mortality-v1+regeneration-v1` (Q3). Stage 5 state is `null` on earlier roots. Partial selections, an unknown version, bad options and Stage 5 calls on `1/1/1/0` are typed refusals. Build 0/0 | PASS |
| **S5-02** dark interior vs managed gap | `S5-02 G-01` | Area A, a 7.5 m gap felled around (20, 15), 12 cells read in both sessions:<br>• Year-1 light: dense 0.003, gap 0.980.<br>• Dense 15-year mean light 0.102 (LR 0.105, below the 0.15 poor-light threshold); it never reaches 3.5 m and never promotes.<br>• Year 12, gap vs dense: abundance 4.500 (full capacity) vs 1.569; tallest band 3.24 m vs 1.74 m. | PASS (see G3-1) |
| **S5-03** bramble/bracken | `S5-03 U-01/U-02` | All 383 cells start at exactly `0.25·T`, bramble = bracken, bit for bit. Year 1 follows gain 0.30 exactly in all 383 cells, and a pure step proves the loss 0.45 branch. `p_veg` is exact at 0.2 and 0.3 m, half at 0.9 m, and 0 at 1.5 m and above. It enters band survival as spec §3.2 states | PASS |
| **S5-04** ClearUndergrowth | `S5-04 C-01/C-02` | Pending and Approve change no cell (same reference) and not the year. Execute zeroes bramble and bracken on the target only, keeps the same band list, and does not move the year or the annual revision (work revision +1). Replay returns a receipt and publishes nothing. Unknown area, out-of-polygon or non-existent cell, other-area cell, duplicate and stale fence are all refused. A drifted batch (4 kinds) is refused whole and the root reference is unchanged. Consented Year+1 cancels clears. Next-year regrowth is exactly `0.30·T` | PASS |
| **S5-05** browsing | `S5-05 B-01` | Pressure 0 is bit-identical to the light-only height and survival paths at 21 light values, and whole sessions at pressure 0 record no browse loss. At 0.2: p = `0.2f·0.15f` = 0.030000001 (see G3-2); the height increment falls by `0.90·p` and survival by `0.04·p`. Bands at 0.9 m are bit-identical under 0 and 0.2 | PASS |
| **S5-06** cross-rack seed rain | `S5-06 S-01/S-02` | A seeds B and B seeds A; the engine total matches a double oracle over both racks to 1e-5 relative. With only immature stems, seed and bands are zero. One mast roll per year, equal to `STAGE5-MAST` with the Q1 thresholds (12 years). A Stage 5 year with reversed area order gives an identical result hash and identical promotions | PASS |
| **S5-07** grid consistency | `S5-07 G-01` | Accepted abundance: in tolerance, 0/80 seed checks outside. Multi-year promotions: **80/80 seed checks outside `[0.75, 1.33]`**. Zero out-of-polygon trees, exact. See §5.3 | **FAIL — stop condition** |
| **S5-08** promotion | `S5-08 P-01/P-02/P-03` | Oldest represented band ≥ 3.5 m; no light gate (Q2); the accumulator never promotes. Age = max(1, y − e); DBH = clamp(1.5·H, 2, 20); `r_0` = `r_pot/√(1+CI/5)`, checked against an oracle with CI > 0, so `r_0 < r_pot`. Block cap: one per block per year; the winner is oldest, then tallest, then (Column, Row) (Q5); deferred cells promote in the following years. Placement is deterministic and strictly in-polygon. The new tree is in the roster, in the canopy/light woodland and in the next adult step. IDs are unique | PASS |
| **S5-09** adult-empty rack | `S5-09 E-01` | Rack B has 0 living adults (its only stem's death is injected; D-093 forbids emptying a rack by Work Plan). Its adult step is skipped, yet its own ecological year advances each year. It receives seed from A, establishes, its undergrowth updates, and it **promotes in year 14**. The new adult runs the adult step the next year | PASS |
| **S5-10** determinism and atomicity | `S5-10 D-01/D-02` | Two 30-year `1/1/1/1` runs are identical. The `1/1/1/1` adult output equals `8a926776…` before any promotion, so Stage 5 never draws from or reorders `ADULT-MORT-*`. A forced pre-commit failure in Execute (with a clear) and in Year+1 leaves the same root reference. The retried year equals an uninterrupted twin | PASS |
| S5-11 independent review | — | not run | PENDING |

### 5.3 S5-07 — measured result (the triggered stop condition)

**Setup.**

- Woodland: MA-1, 30 years, mast seeds 1–20.
- Two scenarios: a year-0 thinning of every second stem ("managed thinning"), and "no work".
- Each seed runs at 5.0 m, at 2.5 m with model light, and at 2.5 m with the shared-light control. In the control, every 2.5 m cell reads the light at its aligned 5 m block centre. That is exactly the 5.0 m light field, which the test proves for seed 1.
- Lattice: 2.5 m has 383 cells in 113 aligned 5 m blocks, 36 of them edge blocks (< 4 in-polygon cells). 5.0 m has 101 cells.

| Scenario | Metric | Light | 2.5 / 5.0 ratio over 20 seeds (min – max, mean, pooled) | Seeds outside `[0.75, 1.33]` |
|---|---|---|---|---|
| managed thinning | accepted abundance | model | 0.815 – 0.836, 0.827, 0.827 | 0/20 |
| managed thinning | accepted abundance | shared | 0.830 – 0.859, 0.846, 0.846 | 0/20 |
| managed thinning | promotions | model | **1.923 – 2.211**, 2.074, pooled **2.072** (3170 vs 1530) | **20/20** |
| managed thinning | promotions | shared | **2.506 – 2.959**, 2.655, pooled **2.652** (4058 vs 1530) | **20/20** |
| no work | accepted abundance | model | 0.842 – 0.870, 0.860, 0.860 | 0/20 |
| no work | accepted abundance | shared | 0.904 – 0.956, 0.922, 0.921 | 0/20 |
| no work | promotions | model | **2.057 – 3.217**, 2.436, pooled **2.412** (1440 vs 597) | **20/20** |
| no work | promotions | shared | **3.000 – 4.304**, 3.502, pooled **3.467** (2070 vs 597) | **20/20** |

**Edge-block effect, reported honestly.**

- 38.3 % of the 2.5 m promotions (managed thinning, 1213 of 3170) fall in the 36 edge blocks, and 46.8 % in the no-work runs (674 of 1440).
- The edge blocks do not explain the excess. Even if every edge-block promotion is removed, the 2.5 m totals are still about 1.3–2.3 times the 5.0 m totals.
- At 2.5 m the standing abundance at year 30 is lower: about 0.52–0.77 of the 5.0 m value. More abundance has been exported into trees.

**Why this fails. The cause is the approved policy, not a code defect.**

- A Unity/5.0 m band holds the abundance of a whole 5 m cell and promotes into **one** tree.
- At 2.5 m the same abundance is split across four cells' bands. Growth is intensive, so those bands reach 3.5 m together.
- The aligned-block cap limits promotions to one per block **per year**. But spec §4 and Q5 say "non-winners stay and can promote next year". So the deferred bands promote in later years, and up to four trees come from the abundance that makes one tree at 5.0 m.
- The shared-light control removes light-field differences and makes the ratio **worse** (up to 4.30), which isolates the scaling/promotion policy as the cause.
- Accepted abundance, which is what the §4 scaling governs directly, is consistent (0.82–0.96).

**Decision needed (owner).** The options below are not implemented or tested; they only frame the choice. The locked decisions are unchanged.

- **(a) Block-level export.** When a block promotes, also remove (export into the same tree) the other cells' qualifying bands of that block's winning cohort. One 5 m block then exports one tree per cohort, as one 5 m cell does. **The implementer's recommendation.** It is the closest analogue of Unity at 5 m, and the existing S5-07 study is a ready acceptance gate.
- **(b) Fractional-promotion accumulator.** The alternative already named in spec §9 Q5.
- **(c) Accept or record a different tolerance** for multi-year promotions.
- **(d) Run Stage 5 regeneration on the 5.0 m grid only.**

Any of (a), (b) or (d) changes Q5 or §4. Any of them would need a spec revision before a new implementation pass.

### G3 measurement-integrity disclosures

1. **S5-02 criterion replaced after a failed first run.**
   - First run: the self-authored sub-criterion "the dense interior never exceeds 1.5 m in 15 years" failed. The dense interior's tallest band reached 2.48 m as Area A self-thinned and its interior light rose from 0.003 to a 15-year mean of 0.102.
   - The 1.5 m cap is not a spec value. It was replaced by the spec's own reading of "stays suppressed": never reaches promotion height or promotes, sits in the poor-light band, and is shorter and less abundant than the gap at the same year. All observed values are printed by the test.
2. **S5-05 float tolerance.** "p ≤ 0.03" is asserted as `p ≤ 0.03 + 1e-7`. The reason: Unity's float expression `0.2f × 0.15f` gives 0.030000001; the spec's 0.03 is the real-number product.
3. **S5-07 horizon is 30 years.** The draft's Q4 text mentioned 10 years. The binding owner resolution has no horizon. In 10 years no band can reach 3.5 m (0.15 + 10 × 0.315 = 3.30 m), so a 10-year promotion ratio would be 0/0.
4. **Stage4BehaviourTests.RunFingerprint** became `internal` with an optional selection. Its default path is unchanged, and it still reproduces `8a926776…`.
5. No other pre-existing test, threshold or harness was changed.

### Spec interpretation choices (documented in code)

- **Light for steps 5a–10.** The refreshed light of the post-adult-step woodland, as in Unity's annual order (`RecomputeCanopy` after mortality, before regeneration). Seed rain uses the same post-adult-step trees, with ages already incremented.
- **Survival.** One double product `light · (1 − 0.04·p) · (1 − p_veg)`, as spec §3.2 states. Unity's competition model applies the vegetation loss as a separate float multiplication first. The two differ only at float rounding; the spec governs.
- **Mast stream.**
  - The stream is a `System.Random` seeded by `SimulationRandom.Create(1, mastSeed, year, salt)`, with `salt` the FNV-1a hash of `"STAGE5-MAST"` (the same pattern as Unity `SpeciesRandom`).
  - The default mast seed is the experiment generator seed (8 850 001 for MA-1).
  - Thresholds use the asset's float values (`0.15f`, `0.15f + 0.35f`).
- **Block cap.** It is woodland-wide: a 5 m block may straddle the A/B boundary. Column and row come from the shared world lattice, so the tie-break is unique.
- **`r_0` competition.** The CI for `r_0` is evaluated against the post-adult-step woodland. Same-year promotions are not neighbours of each other, which keeps the result independent of order.
- **Placement.** Up to 8 jitter attempts inside the cell (Unity offset form), then the cell centre.
- **Year+1 refusal at Regeneration 1.** Year+1 is refused only when no adult is alive anywhere *and* no juvenile abundance remains. Earlier selections are unchanged.

### Spec conformance

| Spec part | Result |
|---|---|
| §2 version switch, supported set, typed refusals, Regen 0 byte-identical | PASS |
| §3 annual steps 5a–12, adult-empty continuation, one mast roll, cross-rack seed, neutral site | PASS |
| §3.1 / §3.2 bramble/bracken, juvenile growth, survival, browse; pressure 0 light-only | PASS |
| §3.3 promotion into CrownModel 1 (`r_0` not `r_pot`), unique ID, in-polygon | PASS |
| §4 area-consistent scaling: accepted abundance | PASS (0.82–0.96) |
| §4 / Q4 multi-year promotions within `[0.75, 1.33]` | **FAIL — stop condition** |
| §5 ClearUndergrowth lifecycle | PASS |
| §6 RNG isolation | PASS |
| §7 atomic immutable state | PASS |
| §10 exclusions | PASS: no excluded file or feature touched |
| H2 constants tagged | PASS: every Stage 5 constant is in `Stage5Constants` under a `TRANSFER` / `GAMEPLAY CALIBRATION` / `SIMULATION ABSTRACTION` header |

## 6. Honest state

- **State words:**
  - `IMPLEMENTED ON TASK BRANCH`: yes.
  - `NUMERICALLY QUALIFIED`: **no**. S5-07 fails, and the stop condition is triggered.
  - `REVIEWED`: no.
  - `INTEGRATED`, `VERIFIED`, `ACCEPTED`: not claimed.
- **Triggered gate:** the task packet's §6 stop condition "S5-07 cannot reach tolerance without changing a locked owner decision". An owner decision is required on Q5 / §4. No other stop condition was triggered: Unity checks agree with the spec, there is no overlapping writer, the frozen hashes and P0 pin are unchanged, no UI/Unity/save change was needed, `.p0ref` matched, and the build is 0/0.
- **Readiness for independent review:** the code and tests are ready for an independent read-only review (S5-11), including whether the S5-07 measurement design is fair. Owner acceptance must wait for the S5-07 decision.
- **Known limitations:**
  - The S5-07 failure itself.
  - The suite exits with code 1 while S5-07 fails.
  - `/multi` cannot show Stage 5; this is by design for this stage.
  - The `1/1/1/1` fingerprint is recorded, not pinned.
  - Stage 5 adds about 30 s to the Core suite.
- **Not verified:**
  - Independent review.
  - Any Unity behaviour or Unity numerical parity of the ported rules. They were read at `5e79e986` and ported, not executed side by side.
  - UI runtime: the UI project compiled; no smoke script or browser ran.
  - Ecological validity: all constants are gameplay or transfer values, and light is not calibrated PAR. Planting or regeneration is not claimed to be beneficial.
  - Windows `git.exe` view of the lane (not used, by instruction).
  - GitHub rendering of these documents.
- **Commit identity:** per-command `-c user.name="Claude Opus 5.5 (Stage 5 implementer)" -c user.email=jermur788@users.noreply.github.com`. Every commit body ends with `Co-Authored-By: Claude Opus 5.5 <noreply@anthropic.com>`. The repository config was not edited.
- **Not done:** no PR, merge, tag, force push, history rewrite or branch deletion.

## Appendix — S5-07 per-seed output (verbatim from the test run at `0ad634ba`)

```text
managed thinning seed  1: accepted 2.5/5.0 0.824 (shared 0.838); promotions 159/76 = 2.092 (shared 207/76 = 2.724); ending abundance 0.569; occupancy 0.601
managed thinning seed  2: accepted 2.5/5.0 0.823 (shared 0.845); promotions 159/79 = 2.013 (shared 205/79 = 2.595); ending abundance 0.579; occupancy 0.611
managed thinning seed  3: accepted 2.5/5.0 0.816 (shared 0.830); promotions 163/81 = 2.012 (shared 205/81 = 2.531); ending abundance 0.547; occupancy 0.577
managed thinning seed  4: accepted 2.5/5.0 0.836 (shared 0.854); promotions 160/74 = 2.162 (shared 189/74 = 2.554); ending abundance 0.588; occupancy 0.620
managed thinning seed  5: accepted 2.5/5.0 0.815 (shared 0.837); promotions 165/79 = 2.089 (shared 199/79 = 2.519); ending abundance 0.565; occupancy 0.596
managed thinning seed  6: accepted 2.5/5.0 0.829 (shared 0.859); promotions 161/76 = 2.118 (shared 214/76 = 2.816); ending abundance 0.574; occupancy 0.606
managed thinning seed  7: accepted 2.5/5.0 0.828 (shared 0.851); promotions 160/74 = 2.162 (shared 219/74 = 2.959); ending abundance 0.523; occupancy 0.551
managed thinning seed  8: accepted 2.5/5.0 0.833 (shared 0.845); promotions 154/72 = 2.139 (shared 189/72 = 2.625); ending abundance 0.585; occupancy 0.617
managed thinning seed  9: accepted 2.5/5.0 0.828 (shared 0.855); promotions 168/78 = 2.154 (shared 216/78 = 2.769); ending abundance 0.536; occupancy 0.566
managed thinning seed 10: accepted 2.5/5.0 0.824 (shared 0.848); promotions 157/71 = 2.211 (shared 208/71 = 2.930); ending abundance 0.535; occupancy 0.564
managed thinning seed 11: accepted 2.5/5.0 0.832 (shared 0.848); promotions 150/78 = 1.923 (shared 203/78 = 2.603); ending abundance 0.599; occupancy 0.631
managed thinning seed 12: accepted 2.5/5.0 0.826 (shared 0.840); promotions 155/73 = 2.123 (shared 187/73 = 2.562); ending abundance 0.587; occupancy 0.619
managed thinning seed 13: accepted 2.5/5.0 0.825 (shared 0.853); promotions 159/75 = 2.120 (shared 210/75 = 2.800); ending abundance 0.558; occupancy 0.589
managed thinning seed 14: accepted 2.5/5.0 0.823 (shared 0.847); promotions 157/80 = 1.962 (shared 209/80 = 2.612); ending abundance 0.545; occupancy 0.575
managed thinning seed 15: accepted 2.5/5.0 0.823 (shared 0.839); promotions 157/80 = 1.962 (shared 208/80 = 2.600); ending abundance 0.583; occupancy 0.615
managed thinning seed 16: accepted 2.5/5.0 0.835 (shared 0.854); promotions 149/72 = 2.069 (shared 184/72 = 2.556); ending abundance 0.583; occupancy 0.615
managed thinning seed 17: accepted 2.5/5.0 0.825 (shared 0.835); promotions 162/78 = 2.077 (shared 202/78 = 2.590); ending abundance 0.571; occupancy 0.602
managed thinning seed 18: accepted 2.5/5.0 0.834 (shared 0.849); promotions 152/72 = 2.111 (shared 190/72 = 2.639); ending abundance 0.590; occupancy 0.623
managed thinning seed 19: accepted 2.5/5.0 0.825 (shared 0.844); promotions 168/83 = 2.024 (shared 216/83 = 2.602); ending abundance 0.599; occupancy 0.632
managed thinning seed 20: accepted 2.5/5.0 0.833 (shared 0.854); promotions 155/79 = 1.962 (shared 198/79 = 2.506); ending abundance 0.573; occupancy 0.605
no work seed  1: accepted 2.5/5.0 0.866 (shared 0.914); promotions 74/30 = 2.467 (shared 108/30 = 3.600); ending abundance 0.727; occupancy 0.766
no work seed  2: accepted 2.5/5.0 0.862 (shared 0.928); promotions 76/27 = 2.815 (shared 103/27 = 3.815); ending abundance 0.711; occupancy 0.750
no work seed  3: accepted 2.5/5.0 0.849 (shared 0.912); promotions 75/30 = 2.500 (shared 107/30 = 3.567); ending abundance 0.679; occupancy 0.716
no work seed  4: accepted 2.5/5.0 0.853 (shared 0.911); promotions 76/33 = 2.303 (shared 106/33 = 3.212); ending abundance 0.691; occupancy 0.729
no work seed  5: accepted 2.5/5.0 0.869 (shared 0.904); promotions 72/35 = 2.057 (shared 105/35 = 3.000); ending abundance 0.768; occupancy 0.810
no work seed  6: accepted 2.5/5.0 0.865 (shared 0.932); promotions 72/28 = 2.571 (shared 104/28 = 3.714); ending abundance 0.718; occupancy 0.757
no work seed  7: accepted 2.5/5.0 0.867 (shared 0.956); promotions 67/27 = 2.481 (shared 110/27 = 4.074); ending abundance 0.766; occupancy 0.808
no work seed  8: accepted 2.5/5.0 0.861 (shared 0.936); promotions 71/33 = 2.152 (shared 101/33 = 3.061); ending abundance 0.750; occupancy 0.791
no work seed  9: accepted 2.5/5.0 0.870 (shared 0.941); promotions 69/30 = 2.300 (shared 104/30 = 3.467); ending abundance 0.769; occupancy 0.811
no work seed 10: accepted 2.5/5.0 0.865 (shared 0.917); promotions 74/23 = 3.217 (shared 99/23 = 4.304); ending abundance 0.672; occupancy 0.709
no work seed 11: accepted 2.5/5.0 0.862 (shared 0.912); promotions 74/34 = 2.176 (shared 103/34 = 3.029); ending abundance 0.736; occupancy 0.776
no work seed 12: accepted 2.5/5.0 0.867 (shared 0.923); promotions 73/32 = 2.281 (shared 102/32 = 3.188); ending abundance 0.768; occupancy 0.810
no work seed 13: accepted 2.5/5.0 0.859 (shared 0.909); promotions 73/32 = 2.281 (shared 102/32 = 3.188); ending abundance 0.717; occupancy 0.756
no work seed 14: accepted 2.5/5.0 0.866 (shared 0.944); promotions 63/27 = 2.333 (shared 99/27 = 3.667); ending abundance 0.763; occupancy 0.805
no work seed 15: accepted 2.5/5.0 0.842 (shared 0.908); promotions 72/30 = 2.400 (shared 106/30 = 3.533); ending abundance 0.675; occupancy 0.712
no work seed 16: accepted 2.5/5.0 0.847 (shared 0.919); promotions 72/31 = 2.323 (shared 105/31 = 3.387); ending abundance 0.689; occupancy 0.726
no work seed 17: accepted 2.5/5.0 0.855 (shared 0.917); promotions 69/31 = 2.226 (shared 96/31 = 3.097); ending abundance 0.722; occupancy 0.761
no work seed 18: accepted 2.5/5.0 0.866 (shared 0.905); promotions 73/30 = 2.433 (shared 106/30 = 3.533); ending abundance 0.729; occupancy 0.769
no work seed 19: accepted 2.5/5.0 0.857 (shared 0.922); promotions 73/25 = 2.920 (shared 103/25 = 4.120); ending abundance 0.704; occupancy 0.743
no work seed 20: accepted 2.5/5.0 0.848 (shared 0.919); promotions 72/29 = 2.483 (shared 101/29 = 3.483); ending abundance 0.695; occupancy 0.733
```
