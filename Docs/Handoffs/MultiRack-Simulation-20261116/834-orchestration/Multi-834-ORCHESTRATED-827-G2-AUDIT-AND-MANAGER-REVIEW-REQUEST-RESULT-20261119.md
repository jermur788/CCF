# Multi #834 — Orchestrated #827 authoritative Fell / Year+1 source audit, returned to Manager #730

**Date:** 19 November 2026
**Parent:** [#834](https://github.com/jermur788/CCF/issues/834) · **Audited claim:** [#827](https://github.com/jermur788/CCF/issues/827) (now HANDED-OFF / CLOSED) · **Manager of record:** [#730](https://github.com/jermur788/CCF/issues/730)
**Result type:** ORCHESTRATION RESULT. No source, fixture, test, Unity, native or physical action. No implementation approval, no V-01/V-02 waiver, no L4 change, no merge.

## 1. Headline for Manager #730

**G2 is BLOCKED at source: the inspected #789 Year2–10 candidate contains no player Fell of any kind.** There is no Fell status transition, no Fell cause value, no Work Plan or work order, no work-event record, no execution identity and no post-Fell year input. Natural mortality is real, typed and separable. The annual continuation step is a genuinely atomic compute-then-publish transaction under one lock with a proven pre-publish fault seam, and is therefore a credible *base* for a future Fell seam — but that seam does not exist today.

Full audit (the reviewable evidence): [Multi-827-READONLY-AUTHORITATIVE-FELL-YEAR-SEAM-AND-G1-FIELDS-SOURCE-AUDIT-RESULT-20261119.md](https://github.com/jermur788/CCF/blob/review/multi-827-legal-fell-year-and-g1-field-audit-20261119-v1/Docs/Handoffs/MultiRack-Simulation-20261116/827-orchestration/Multi-827-READONLY-AUTHORITATIVE-FELL-YEAR-SEAM-AND-G1-FIELDS-SOURCE-AUDIT-RESULT-20261119.md)

**Decision requested:** one scoped **APPROVED / NEEDS CHANGES / REJECTED** on *evidence sufficiency* — not implementation acceptance. No second documentary review of the audit was commissioned, and none is needed.

## 2. Actors, provenance and publication integrity

| Item | Value |
|---|---|
| Orchestrator (parent #834) | `opencode-go / DeepSeek V4.1 Flash (high)` in the T3 Code → OpenCode harness; owner Windows 11 + WSL2; GitHub via `gh` as `jermur788`. Orchestration only. |
| Independent inspector (#827) | **Claude Opus 5.5 (high)** via the T3 `claudeAgent` pool — distinct session, distinct model family, nonauthor of #789/#811/#814, no subagents. |
| Inspector RUNNING | [#827 comment 7020107710](https://github.com/jermur788/CCF/issues/827#issuecomment-7020107710), posted before any result write. |
| Inspector handoff | [#827 comment 7020320704](https://github.com/jermur788/CCF/issues/827#issuecomment-7020320704); #827 CLOSED. |
| Audit branch | `review/multi-827-legal-fell-year-and-g1-field-audit-20261119-v1`, commit `39370daf7e12732dffe37ea54dbe3dbb561acd57`, created from live main `852304d8a668d6ee2b04547aaec751be6b462c53`. |
| Audit diff | exactly **one added Markdown**, +190/−0, no source/spec/coordination/test change. Verified by GitHub compare. Main has since advanced to `24716d67` (`Docs/Project/coordination/active-tasks.md` only, unrelated path), so the branch is 1 ahead / 1 behind current main with no file overlap. |
| Readback | file re-fetched through the API, byte-identical to the inspector's draft; `sha256 c022f3358d852ad3f14f21acdcbd33e91f789d2a9d1fde0b1db15cb63d09b108`, 190 lines / 32,409 bytes. |
| Orchestrator spot-check | 20+ decisive citations re-read independently at the same worktree HEADs (see §3). All matched; no overstated claim found. |

Inspected source provenance (all read-only, none modified):

| Role | Path | Branch @ HEAD | Dirty |
|---|---|---|---|
| #789 Year2–10 engine | `/home/jer/CCF-Dev/Multi-20261119/y2engine` | `feature/multi-789-year2-10-engine-20261119-v1` @ `6f9d2e99…` | clean |
| H0A Year0 core | `…/core` | `feature/multi-h0a-core-year0-admission-20261118-v1` @ `c4124b63…` | pre-existing untracked `Tools/MultiRackSitkaYear0Viewer/fixtures/` only |
| H0A viewer | `…/viewer` | `feature/multi-h0a-a-year0-viewer-20261118-v1` @ `f3537fe3…` | clean |
| #811 history viewer | `…/_811/history` | `feature/multi-811-offline-history-viewer-20261119-v1` @ `89613237…` | clean |
| #814 marking | `…/_814/marking` | `feature/multi-814-local-candidate-marking-20261119-v1` @ `e8e1d376…` | clean |

## 3. What is PROVED (and re-verified by the orchestrator)

- **Genesis identity, locally:** fixture `p1d-30m-sitka-adult-v1.sitkap1fx.json` is 32,567 bytes, `sha256 bcaaa5c3…`, `treeCount: 100`, IDs `P30-001`…`P30-100`, `contentHash b7dee8a3…`; per-tree values are float32 bit words (`F32.cs`), stand aggregates are `double`. Re-verified directly.
- **Admission:** catalog trust is checked before the file is opened (`FixtureAccess.cs`), with size/hash/strict-parse/ordinal-unique-ID rules (`DerivedManifest.cs`).
- **Atomic annual step:** `AnnualContinuation.Advance` validates `(expectedYear, expectedRevision)`, refuses when no live stem remains, computes the whole next snapshot, then publishes it in one assignment under a lock, with a test-only fault seam *before* publish (`AnnualContinuation.cs:28–46`). Re-verified.
- **Natural mortality is separate and typed:** `AnnualDeath(TreeId, EcologicalYear, Cause, Mechanism)`, `"self-thinning"` with `hazard`/`boundary`, dead rows frozen.
- **RNG is survivor-safe by construction:** `SimulationRandom.Roll(model, "ADULT-MORT-"+TreeId, year, seed)` — keyed by tree ID, year and seed, not draw order.
- **Zero-live refusal** for the annual advance (`ContinuationNoLivingStems`).
- **Protected Year0–10 no-work oracle exists as recorded source digests** (Year 1 `df2d44fd…` … Year 10 `10d31913…`) plus sequential repeat-identical checks. **Not re-executed** — see §8.
- **Reusable Unity core maths is already compile-linked unchanged:** `Assets/ForestPrototype/SitkaGrowthModel.cs` (`e4c73402`) and `SimulationRandom.cs` (`bb83d7f8`), blob IDs identical on main and to the pins. Re-verified.

## 4. What is GAP / CONTRADICTED

1. **No player Fell anywhere in the engine.** CLI exposes only `generate-manifest`, `verify-derived`, `inspect-original`, `annual`. The only write paths publish ecology. The import path explicitly refuses felled trees (`"felled trees are unsupported in an original import"`), and `P1Disclosure` states "ANNUAL ADVANCE AND WORK REFUSED". Re-verified verbatim.
2. **No Work Plan / batch / order / event.** Whole-batch validation, duplicate/missing/dead/stale-ID refusal and roster+count+BA+event ordering have no implementation; only analogues exist (annual step; P0's clone-and-commit; #814's non-authoritative JS validation).
3. **No execution identity.** A repeated advance is *refused* (`ContinuationYearMismatch`), not replayed as an identical-key no-op. Spec R-15 / X-14 are unimplemented.
4. **No current-source "AfterMortality" frame.** Multi-year state is `internal` (`ContinuationSnapshot`), reachable only by `InternalsVisibleTo`/source-compiling exporters; the public engine advances exactly one year and refuses afterwards.
5. **No intervention→Year+1 path.** The ecology step consumes only living survivors; nothing carries a Fell into it. The ecology half is ready; the intervention half is absent.
6. **`revision == year` invariant.** Tests and the #811 exporter assume `Revision` equals the year count, which a same-year Fell revision would break.
7. **V-01 / V-02 unresolved and unwaived.** The candidate compiles two files out of `Assets/ForestPrototype/` via `$(CcfRepoRoot)` while tracking **no `Assets/` directory** (32 Tools files only), so it cannot build standalone; the P0 `SitkaLab.Core.dll` referenced for ABI identity is git-ignored and local-only. #804 remains **PARK FOR DESKTOP / MAIN BLOCKED** with **L4 FAIL38 unwaived**.
8. **No candidate is on GitHub.** All five inspected branches return 404; `Tools/MultiRack*` is absent from main. A GitHub-only reviewer could not have inspected any of this — the audit's reachability comes from the owner's local read-only worktrees.
9. **G1 cannot support multi-area comparability.** One 30 m square, single species, single cohort age, synthetic crown radius, fixed site productivity 1.0, no terrain/wind, **no management-area identity**. #828 remains DRAFT; no comparator or fixture was invented.
10. **Minor contradiction:** stale comments in `TrustTables.cs` / `FixtureAccess.cs` still claim "no ecological year runs", while `AnnualEngine.Advance` runs one. Documentation-only; no behavioural risk identified.

## 5. Reuse recommendation (source-similarity only — not an ABI/compile proof)

| main type | Verdict | Reason |
|---|---|---|
| `SitkaGrowthModel.cs`, `SimulationRandom.cs` (real path `Assets/ForestPrototype/`, **not** `WorkEconomy/`) | **REUSE** | Pure, already compile-linked unchanged; survivor-independent rolls. Any continued reuse must resolve V-01 as an approved explicit build-input scope. |
| `ScenarioOneWorkOrder.cs`, `ScenarioManagementEvent.cs` | **ADAPT (concepts)** | Right vocabulary (`FellTree`, felled treatment, status split) but Unity `Vector3`/3D world position and extraction-economy fields must not transfer into 2D/Stage 5. |
| `ScenarioOnePrescriptionApproval.cs` | **ADAPT** | Closest match to proposal → approval → PendingWork with player precedence (D-065); its rack/coverage-unit conflict model is forbidden by G0. Only relevant once G1 exists. |
| `ScenarioManagementHistory.cs`, `ScenarioCycleState.cs`, `ForestryWorkDomain.cs`, `ForestryWorkCalculator.cs` | **EXCLUDE for Stage 5** | Economy/settlement coupling (G4 deferred) or descriptive-only state, not an authority. |
| P0 `Tools/MultiRackSitkaLab` plan/Tx code (`a7f41983`) | **ADAPT (pattern only)** | Best precedent for atomic whole-plan Fell + event append, but a different domain with a calendar-only year step; P1 refuses its felled state. Do not link it in. |

Consequence for #730/#822: **do not design a second Work Plan / history domain blind.** Reuse the two pure maths files, adapt the order/event/approval *vocabulary*, and exclude the economy layer.

## 6. Manager decision surface (design choices, not approvals)

Before any G2 code, #730 must settle: (a) Fell transaction placement beside `AnnualContinuation.Advance` under the same lock and publish pattern; (b) a distinct Fell cause type rather than a free string; (c) whether a same-year Fell bumps `Revision` (breaking `revision == year`) or a separate work-revision counter leaves the no-work oracle untouched; (d) session-local execution key → payload digest → result map for identical-key no-op vs conflicting-payload refusal; (e) whether the fixture's `actionEligibleDbhThresholdMm = 170` binds Stage 5 (open product question — no threshold invented here); (f) an approved public continuation contract instead of `internal` + source-compiling; (g) a V1 management-scope ID on work and event records.

## 7. Lawful access, custody and what was preserved

- #827's original browser-only wording was explicitly amended by Manager #730 for **permitted desktop read-only inspection**. The owner-directed #834 orchestration authorised exactly that, and nothing more. The inspector used reads, `git` read-only commands, `gh` API reads and one API file write; no code was executed.
- **No source, fixture, test, spec, coordination, main or Ops artefact was changed.** All five worktree HEADs and dirty counts are unchanged; `core` still carries only its pre-existing untracked fixtures directory. The archive checksums verify 9/9; bundles were not unpacked.
- No lease, lock, board row, Editor or machine-wide Unity state was acquired or altered, and none is claimed. `/media/jer/{ZX20,Files}` was not mounted and was not needed.

## 8. Not verified (explicit)

No test, build, .NET, Unity, browser or game run occurred. All "recorded test" statements are **source text**, not executed evidence. Year0–10 no-work digests and the fault-seam tests were **not re-run**. L4 FAIL38 was taken from the #804 Manager verdict and not re-evaluated. `plan.md`/`tasks.md` were fetched but not line-cited. **Nothing here proves Unity import, native compile, runtime or rendered behaviour.**

## 9. Downstream gate — what a G2 implementation would still need

1. Manager #730 verdict on this audit (APPROVED / NEEDS CHANGES / REJECTED) and, if it chooses to proceed, a bounded G2 source-contract revision settling the seven design choices.
2. A genuinely approved spec state for that revision (Stage 5 is design-approved, not implementation-released).
3. An approved public source lineage — public/committed candidate custody or an explicit, reviewed V-01 build-input scope — before any branch is treated as reviewable source. **No automatic V-01/V-02 waiver and no L4 downgrade.**
4. A real source/physical reservation: one exclusive writer lane, single-writer file allowlist, and the applicable Ops/custody grant. None exists today.
5. Then, and only then, implementation by a single writer followed by a **separate nonauthor C# source and exact-candidate feature qualification**: atomic whole-batch Fell with rollback, once-only same-process identity, Fell→Year+1 ecology on survivors, and the Year0–10 no-work oracle unchanged, at Tier 1/Tier 2.

**User/physical authorisation still required:** anything that writes source (lane + writer + Ops/custody), any public/committed custody change for the local candidates, and any eventual integration or merge. This orchestration and the audit grant none of these.

## 10. Orchestration closure

- #834 [RUNNING] registered at [comment 7020054724](https://github.com/jermur788/CCF/issues/834#issuecomment-7020054724); #827 dispatched to one nonauthor inspector, audited, handed off and closed; this single consolidated result published on `docs/multi-834-orchestrator-result-20261119-v1` as exactly one new Markdown; #834 marked HANDED-OFF / CLOSED.
- No redundant review of the audit was created. `Docs/Project/coordination/active-tasks.md` was **not** edited: Multi coordination remains Manager #730's single-writer call.
