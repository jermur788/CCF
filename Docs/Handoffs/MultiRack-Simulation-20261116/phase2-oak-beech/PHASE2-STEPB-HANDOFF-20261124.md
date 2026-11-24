# MULTI Phase 2 — Sessile Oak and European Beech: Step B implementation handoff

**Written:** 2026-11-24 07:39 UTC
**Status:** `IMPLEMENTED ON TASK BRANCH + INDEPENDENTLY REVIEWED (G2/G3 PASS, APPROVED) — FOUR OWNER DECISIONS OUTSTANDING`
**Not merged. No PR. Not integrated.**

---

## 1. Identity and authorship

| Field | Value |
|---|---|
| Task class | `SPEC REQUIRED`, two-phase gate |
| Phase A (Step A) | DRAFT spec quartet + read-only pre-flight audit |
| Phase B (Step B) | pure-.NET engine implementation `1/1/1/1/1` + `P2-01`–`P2-10` |
| Spec author | **Claude Opus 5.5** (`claudeAgent`/`claude-opus-5-5`) |
| Implementer | **Claude Opus 5.5** |
| Independent reviewer (`P2-11`) | **Claude Sonnet 5.5** (`claude-sonnet-5-5`), effort `xhigh`, fresh thread |
| Coordinator, pre-flight audit, this handoff | **DeepSeek V4.1 Flash** (`opencode-go/deepseek-v4.1-flash`, variant `high`) — coordination-only; wrote **no** code, tests or implementation docs |
| Model **requested** by packet | capable C#/.NET route (Opus 5.5 or Sonnet 5.5) |
| Model **selected** for coordination | DeepSeek V4.1 Flash, inherited, not chosen by the coordinator |
| Model **resolved** | Opus 5.5 implementer, Sonnet 5.5 reviewer — per owner decisions C1/C2 |

No model family owns code, Unity, assets or testing; the split above is the temporary ownership recorded for this task.

## 2. Refs

| Ref | SHA |
|---|---|
| `feat/multi-phase2-oak-beech-20261124-v1` (implementation) | `401dab1835128f70d1ecc28f54ae5db8ac68bf69` |
| Implementation commits | `cc67361f` (engine), `401dab18` (tests) |
| `docs/multi-phase2-oak-beech-spec-20261124-v1` (APPROVED spec) | `8cec7c4d7d1882056477c18ed9ca7a3de3fdd5b9` |
| Tag `docs/multi-phase2-oak-beech-spec-draft-20261124` (approved DRAFT artifact) | `3a1eca6c6d5aaf2b69230376deeb4f6c3fd15794` |
| `review/multi-phase2-oak-beech-20261124-v1` (P2-11 review) | `4c74533e03ac8a586f64b5dbe4c671e7ffd92662` |
| Base — Stage 5 docs tip | `d1db81b59248c13b4f9682ecdf68e37759b15c65` |
| Stage 5 approved code head | `5b757d9b4a353d07557db95d3af9227c8d841874` |
| `origin/main` | `c1302e64dc9bd55ce9d4be7c2e84d242ec87e38c` |
| `active-tasks.md` blob on `main` | `c2954aaad78fd8a2a0d2814ed3caa15e066ba221` |

Links:
- Spec: https://github.com/jermur788/CCF/blob/docs/multi-phase2-oak-beech-spec-20261124-v1/specs/multi-phase2-oak-beech-v1/spec.md
- Review: https://github.com/jermur788/CCF/blob/review/multi-phase2-oak-beech-20261124-v1/Docs/Handoffs/MultiRack-Simulation-20261116/phase2-oak-beech/MULTI-PHASE2-OAK-BEECH-INDEPENDENT-REVIEW-20261124.md
- Pre-flight audit: https://github.com/jermur788/CCF/blob/docs/multi-phase2-oak-beech-spec-20261124-v1/Docs/Handoffs/MultiRack-Simulation-20261116/phase2-oak-beech/PHASE2-PREFLIGHT-AUDIT-20261124.md

## 3. Git state

```
git diff --shortstat d1db81b5 401dab18
 12 files changed, 3563 insertions(+), 32 deletions(-)
```
```
 .../src/Core/MultiArea/MultiAreaContinuation.cs    |  228 +++-
 .../src/Core/MultiArea/Phase2Annual.cs             |  663 +++
 .../src/Core/MultiArea/Phase2MixedDensity.cs       |  311 +++
 .../src/Core/MultiArea/Phase2Models.cs             |  255 +++
 .../src/Core/MultiArea/Phase2Planting.cs           |  165 +++
 .../src/Core/MultiArea/Phase2Records.cs            |  162 +++
 .../src/Core/MultiArea/Phase2Rules.cs              |  277 +++
 Tools/MultiRackSitkaP1Lab/src/Core/Stage4Models.cs |   28 +-
 .../tests/Core.Tests/Phase2Support.cs              |  235 +++
 .../tests/Core.Tests/Phase2Tests.cs                | 1257 ++++++
 .../tests/Core.Tests/Program.cs                    |    1 +
 .../tests/Core.Tests/Stage5Tests.cs                |   13 +-
```
`git status --short`: **empty**. No mode-only churn; no `Assets/`, `/multi` UI, save-schema, `ProjectSettings/`, Compact30, Legacy40, Enlarged80 or Reference Future v1 change. `tests/Core.Tests/Program.cs` is the **test runner**, not `src/Ui/Program.cs`.

## 4. Verification evidence

| Check | Result |
|---|---|
| Clean rebuild, `--no-incremental`, `--disable-build-servers`, external `TMPDIR` | **0 warnings / 0 errors** (implementer, coordinator and reviewer all reproduced) |
| Full Core suite | **242 / 242 passed, 0 failed** — 209 pre-existing + 33 Phase 2 |
| Implementer runs | 242/242 twice (43 s, 42 s) |
| Coordinator independent run | 242/242, exit 0 |
| Reviewer runs | 242/242 twice, identical PASS sets |
| Run-to-run difference | only PS-01 / MA-W10 race counts and MC wall-clock lines |

## 5. Protected hashes — expected vs observed

| Anchor | Value | Observed |
|---|---|---|
| Single-area Model 0 | `7e3a2a869ec1662458b74875cadee1e4b16cf2b87b0aaa05df2d9a0061311ece` | match |
| Multi `0/0/0/0` | `a692a6d5bc374dd0225153b3bf0e0b0e9467d3c041b0a6b03414f42d3c02233d` | match |
| Multi `0/0/1/0` | `814e81650b62428510fe60c3142b51d9055537d6014303074f08733bf8d46287` | match |
| Stage 4 `1/1/1/0` | `8a926776ffa3ca3dd1611c2e4d462f8c82b4d78f29ad5e83e5a8f8ce1d2b87fb` | match |
| Stage 5 `1/1/1/1` | `c4b670ede36ef133fb7ab9b43992319b340f27827ce85025ae68444c63d2dc35` | match; **now pinned** as `Stage5Tests.PinnedStage5FullRunHash` (declared test change, closes pre-existing nit R8) |
| P0 Core binary pin | `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3` | match, from build and from re-hashing the loaded DLL |
| Q6 equivalence `1/1/1/1/1` vs `1/1/1/1/0` | — | identical `776a68df048a640c849801e743db3372d9d6d76808828d0e0623222ae35ce22d` |
| MA-1-MIXED-v1 30-year | — | `93e9abda0670cb7a7f56fda91cc1e96e8bc931b330f57d99ec1b588600b9251f` |

## 6. `P2-01`–`P2-10`

All ten PASS as worded, per the reviewer's per-criterion table. Two carry evidence caveats that are **not** failures and are listed in §8:
- **P2-06** is demonstrated at the `p_veg` level only, never at the survival level (F1).
- **P2-07 / P2-10** use seeded states; the plant → promote lifecycle is never exercised end to end on the shipped fixture (F3).

## 7. Scope confirmations (checklist H1–H5)

- **H1 scope** PASS — all 12 files under `Tools/MultiRackSitkaP1Lab/`.
- **H2 evidence labels** PASS — every constant group carries a class label; none is `IRISH DIRECT`.
- **H3 `0.7 m` rule** PASS — labelled `NEW OWNER RULE` in constants, in `Phase2Evidence.AdultSpacing` and in the refusal text. It is **not** a Unity transfer. The `0.5 m` planting rule **is** a transfer, and its tie-break is Unity's "lower work-order id wins".
- **H4 not-a-port statement** — present in code as `Phase2Evidence.NotResolvePlanting`. Stated here in prose: **`Plant` / `Shelter` / `SpotClear` are not a port of Unity's `ResolvePlanting`.** The engine command borrows Unity's geometry, nursery defaults (age 3, height 0.6 m), `sqrt(1/π)` = 0.5641896 m spot radius and browse rules only. There is no stock, cash, owner-minutes or `stockItemId` offer identity, by owner decision.
- **H5 not-verified list** — §10.

## 8. G3 measurement-integrity notes (honesty section)

1. **Only two existing test files changed.** `Stage5Tests.cs` has exactly three hunks (one constant, one assertion, `FullRun()` split into an overload); `tests/Core.Tests/Program.cs` gained one registration line. Nothing else moved.
2. **One oracle changed** during implementation: the browse taper width uses Unity's float expression `1.8f − 1.2f`, not the literal `0.6f`. This is a fidelity correction, not a loosened criterion.
3. **Five new tests failed on their first run** and were repaired as test-construction errors. The reviewer **could not verify this claim** — there is a single test commit, so the history is not inspectable. Final oracles show no weakened criterion. This is disclosed rather than asserted.
4. **A test-only seam exists in production code:** a `[ThreadStatic] DrawLog` on `Phase2Random`, used by `P2-03.1`. Reviewed as acceptable, inert by default, and limited in reach — it cannot observe draws made directly through `SimulationRandom`.
5. **Mutation coverage is partial, and the survivors are the important part.** The reviewer ran 36 single-site mutations on `/tmp` copies (35 valid; one equivalent and void). **27 caught, 8 survived:** six unasserted constants/readings (m10, m21, m22, m29, m30, m31), the vegetation-death case (m18), and the direct-draw case that bypasses the seam (m27). Consequence: a mutation that stops vegetation ever killing a planted stem **still passes all 242 tests**. This is exactly F1.
6. **Six implementation ambiguities were self-resolved** and were all checked against the Unity source by the reviewer; one (generic occupancy computed over all living post-growth stems rather than the spec's literal "stems still standing") deviates from the approved wording in favour of Unity's `PlanComponents`, which the spec labels a TRANSFER.
7. **Species imbalance is real and explained, not a bug.** 30-year deadwood was Beech 173 / Oak 37 / Sitka 191; `P2-09 K-01` promotions Beech 10 / Oak 1 / Sitka 1. The reviewer traced it to the ratified light tables: Oak growth at light 0.35 is 0.70 against Beech's 1.00, so **no Oak recruit can promote inside the 14-year window**, and over 30 years Oak promotes roughly 5× less than Beech. This follows directly from the accepted Oak distinct-light tables and is not a tuning error.
8. **Planting is viable but only off the recipe's dark rack.** On the shipped `MA-1-MIXED-v1`, all four planted juveniles in the `P2-10` recipe die from low light. The reviewer's paired probe at full light shows 66% unweeded survival against 95% with a spot patch, and Oak/Beech with `Shelter` + `SpotClear` promote in year 9 in bright cells and managed gaps.

## 9. Owner decisions outstanding (reviewer's F-items; none made here)

| # | Decision | Recommendation |
|---|---|---|
| **F1** | Accept `P2-06` at `p_veg` level as the criterion is worded, **or** require one paired survival test before calling Phase 2 qualified. | **Add the test.** The reviewer already has the paired-probe evidence (66% vs 95%); the surviving m18 mutation proves the current suite cannot detect a broken vegetation-death path. |
| **F2** | Ratify `Phase2PointWithoutGroundCell` as a documented, tested refusal, or choose another treatment for points in boundary slivers. It refuses ~3% of points the spec calls valid, and was not in the spec. | **Ratify and document** via a spec amendment record, and add a test. A point with no in-polygon 2.5 m cell has no light or cover to read; refusing is safer than inventing data. |
| **F8/F9** | Decide how the spec branch and the feature branch integrate. | Keep separate for now per your C6 decision; merge neither to `main`. See §11. |
| **F10** | Acknowledge that Oak is a rare recruit and promoter under the ratified tables, so the 197-vs-40-style split is not later read as a bug. | **Acknowledge.** It follows from the accepted Oak light tables, not from a defect. |

Optional, test-only, no production change: a lifecycle test in a bright cell (F3); asset pins and literal-value assertions for the Oak/Beech constants (F4); a Sitka-with-nearby-Oak Hegyi case (F5); a near-`Dmax` case for the 0.125 budget (F7).

## 10. What was not verified

- **Nothing was run in Unity.** No `Assets/` behaviour, no `/multi` UI, no browser, no save/load, no protected port (`:33767`, `:43649`, `:46593`), no rendered check. Unity source was **read** only, to confirm constants and rule semantics.
- **Test history** — one commit, so first-run failures and the "fixture fixed before measurement" claim are not provable from git.
- **Determinism beyond one host** — two suite runs and several probes, no CI or second machine.
- **Mutation coverage is not completeness** — 35 valid single-site mutations only.
- **The owner's own words for Q1–Q6 and audit correction A-1** were seen only as rendered in the spec amendment record.
- **Reviewer probes are not committed** — throw-away runs on `/tmp`; methods described, sources not published.

## 11. Topology (deferred C6 decision, now concrete)

Three branches carry this task and they touch disjoint files:

| Branch | Carries | Head |
|---|---|---|
| `docs/multi-phase2-oak-beech-spec-20261124-v1` | the APPROVED quartet | `8cec7c4d` |
| `feat/multi-phase2-oak-beech-20261124-v1` | engine + tests + this handoff | `401dab18` (plus this docs commit) |
| `review/multi-phase2-oak-beech-20261124-v1` | the P2-11 review | `4c74533e` |

The feature branch **contains no copy of the spec quartet** — it was cut from `d1db81b5`. The reviewer trial-merged the spec and feature branches read-only and reports they merge cleanly. All three remain unmerged, stacked on the unmerged Stage 5 lineage, per your C6 decision. Nothing reached `main`.

## 12. Canonical status

`IMPLEMENTED ON TASK BRANCH + INDEPENDENTLY REVIEWED (G2/G3 PASS, APPROVED) — FOUR OWNER DECISIONS OUTSTANDING (F1, F2, F8/F9, F10).`

Not integrated. Not merged. No PR. Phase 3 scope (cash, prices, contractor fees, timber assortments) and all other deferred items remain untouched.
