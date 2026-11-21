# Multi #885 — D-091 physical admission receipt and integrator scope

**Role:** authorised D-091 physical Ops custodian and coordination-board owner (owner appointment, this session).
**Verdict: ADMISSION PASS** — for one exclusive 2D integration lane and one separate non-overlapping read-only qualification lease.
**INTEGRATED: NO. VERIFIED: NO. OWNER ACCEPTED: NO.** This receipt grants physical/custody admission only. No integration, build, test, engine run or Unity action was performed while issuing it.

Canonical URL: https://github.com/jermur788/CCF/blob/docs/multi-885-d091-admission-20261121-v1/Docs/Handoffs/MultiRack-Simulation-20261116/885-multi-area-spec/Multi-885-D091-PHYSICAL-ADMISSION-RECEIPT-AND-INTEGRATOR-SCOPE-20261121.md

| Item | Value |
|---|---|
| Issued at | 2026-11-21T16:31Z and after (observations below) |
| Issuing actor | opencode-go / DeepSeek V4.1 Flash, T3 Code session `a8d35599-48af-4f1b-a652-26f5ff44da5e`, orchestration/custody only — **not** the integrator and not the author |
| Host | `DESKTOP-DL1BQI7`, Linux 6.18.58.1-microsoft-standard-WSL2 x86_64, user `jer` uid 1000 |
| Authority | Owner appointment as D-091 physical custodian and board owner (this task). Manager handoff: `docs/multi-885-q0-manager-scoped-closure-20261121-v1` → `Multi-885-MANAGER-Q0-SCOPED-APPROVED-CLOSURE-AND-Q1-DEPENDENCIES-20261121.md` |
| Governing policy | **D-091**: no arbitrary time limits on worker/worktree ownership; a lane ends on documented safe completion/release, owner-directed cancellation/handoff, or a genuine conflicting-writer/safety/authority stop. **No expiry date is invented below.** |
| Q0 status | APPROVED and CLOSED (revision 3 `d9ca6e0c`, final reviewer `5239c33f`, scoped APPROVED). Not repeated here. |

## 1. What was refreshed before admission

Live `Docs/Project/coordination/active-tasks.md` (main), issue #885 context as recorded on the board, the Manager Q0-closure handoff, the Manager integration-admission/custodian-request packet, decision-log **D-091** and **D-092**, and the separate integrator preflight result `bcd65d52`.

**No machine-readable physical Ops/lease registry exists on this host.** A bounded search found no lease registry, lock file or Ops board: `~/.ccf-orch` contains only historical task evidence files, and `find /home/jer -maxdepth 3 -iname '*lease*'` returned only unrelated Node.js release-tooling files. The earlier preflight reported the same absence. **This receipt is therefore the authoritative physical record for this lane, issued by the appointed custodian — not a discovered pre-existing board, and not a self-invented registry.**

## 2. Observed physical facts (measured, not assumed)

| Fact | Observation |
|---|---|
| Volume | `/dev/sdd` mounted `/`, 1007 G total, 77 G used, **880 G available**, 8 % |
| Home ownership | `/home/jer` and `/home/jer/Development` owned by `jer`, writable |
| Worktree parent | `/home/jer/Development/CCF-HP-Worktrees` exists, owner `jer`, 28 registered worktrees |
| Local clone | `/home/jer/Development/CCF-HP` at `cbf3a6185754a33f4a75b96f8fe775d0041b72a1`, `git status --porcelain` **0 lines** |
| Candidate reachable | `4cecf873…` = commit; `e11dd519…` = commit; **`e11dd519` is an ancestor of `4cecf873`** (fast-forward possible) |
| Integration branch | `integration/multi-885-ma1-two-area-20261121-v1` — **absent** |
| Integration worktree path | `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-885-Integration-20261121` — **absent** |
| Qualification scratch path | `/home/jer/Development/CCF-885-Qualification-Scratch-20261121` — **absent** |
| SDK for the pinned build | `global.json` at the candidate pins `sdk 8.0.133`, `rollForward latestPatch`; `/home/jer/.dotnet/dotnet` has **8.0.134** with ASP.NET Core/Core runtime **8.0.34**. System `dotnet` on PATH is **10.0.115**. **The integrator must invoke `/home/jer/.dotnet/dotnet` explicitly.** |
| Ephemeral loopback ports | `33811`, `33841`, `33911` all **free** at observation |

**Protected processes re-verified live (must not be killed, restarted, repurposed or force-released):**

| Lane | PID | Listener | Working directory | State |
|---|---|---|---|---|
| #877 owner review | 1750123 | `127.0.0.1:33767` | `/home/jer/Development/CCF-Review-877/wt-owner/Tools/MultiRackSitkaP1Lab` | ALIVE, untouched |
| #882 private playtest | 1806361 | `127.0.0.1:43649` | `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-882-Launch/Tools/MultiRackSitkaP1Lab` | ALIVE, untouched |
| #867 incident | 390578 | — | — | ABSENT (unchanged; belongs to its owner) |

Existing 885 worktrees, both **clean** and preserved: `CCF-Multi-885-MA1` at `ac4c7b51` (author lane, branch `feat/multi-885-ma1-two-area-workplan-20261121-v1`) and `CCF-Multi-885-Review-20261121` at `58950a9a` (reviewer, detached HEAD).

## 3. Source-ownership disposition

- **#882 remains an existing, protected source lane.** Its branch `feat/multi-882-competition-unity-ui-firstpass-v1` is at `32a3ddc46e032c92431d113325376adfcbdc0cd0`; its live worktree `CCF-Multi-882-Launch` is at `e11dd519` with a **clean** status and a running private listener. **No release is inferred and none is granted here** (D-091). The board's #882 writer reservation is left intact.
- **#885 author lane is complete for integration purposes, not released.** Author head `ac4c7b51` = candidate `4cecf873` **plus exactly one documentation file** (`Multi-885-MA1-INDEPENDENT-REVIEW-RESULT-20261121.md`). The integrator therefore fast-forwards to `4cecf873` and **must not** take `ac4c7b51`; the review document is linked, not merged. The author worktree keeps its branch under D-091 until documented completion.
- **One exclusive integrator lane is created by this receipt**, disjoint in branch ref, worktree path, build outputs and port from every lane above.
- **Conflict analysis (recorded, not assumed):** the committed candidate delta is **16 paths** (15 code/test files plus the author handoff Markdown — see §5). The #882 branch delta versus the same base is **1 documentation file**, and the intersection between the two deltas is **empty**. A *future* merge of #882 UI work onto the integrated lane is not analysed here and remains a Manager decision.

## 4. Exact SHAs and target

| Role | SHA |
|---|---|
| Integration start target | `e11dd5192d7a89abf859613ce92814bdd6338ea2` |
| Approved code candidate (fast-forward destination) | `4cecf87339bea4d32907955a8d5625d1f27f96bd` |
| Author/docs head (linked only, NOT integrated) | `ac4c7b51582283fb125f6b747a76b31282451696` |
| Integrated HEAD | **NONE** until the integrator publishes it |

Target branch: `integration/multi-885-ma1-two-area-20261121-v1`, created at `e11dd519` and fast-forwarded **only** to `4cecf873`. No merge commit, no cherry-pick, no reconstruction, no main merge.

## 5. Physical grant — permitted paths and resources

**Integration lane (exclusive writer = the integrator, during its run):**

| Resource | Granted |
|---|---|
| Worktree | `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-885-Integration-20261121` (absent at issue; integrator creates it) |
| Branch | `integration/multi-885-ma1-two-area-20261121-v1` |
| Writable source scope | **exactly** the candidate's own 15 code/test paths — no source edit is authorised; they must remain byte-identical to `4cecf873`: `src/Core/MultiArea/{MultiAreaContinuation,MultiAreaContracts,MultiAreaExperiment,MultiAreaGenerator,MultiAreaGeometry,MultiAreaProjection,MultiAreaWorkPlanAdapter}.cs`, `src/Ui/{PlayerApi.cs,PlayerMultiApi.cs,Program.cs,wwwroot/app.js}`, `tests/Core.Tests/{MultiAreaTests.cs,Program.cs}`, `tests/ui-smoke-multiarea.sh`, `tests/ui-smoke.sh` (all under `Tools/MultiRackSitkaP1Lab/`) |
| Writable generated output | that worktree's own `bin/`, `obj/` and test/evidence output **only** |
| Ephemeral listener | `127.0.0.1:33811` (verified free) — one listener, integrator-owned, stopped at completion |
| Build command | pinned SDK invoked as `/home/jer/.dotnet/dotnet` (8.0.134) |
| Read-only inputs | `Assets/ForestPrototype/{SitkaGrowthModel,SimulationRandom,TreeSpeciesDefinition}.cs`, `Assets/ForestPrototype/Species/SitkaSpruce.asset`, immutable fixture `fixtures/derived/p1d-30m-sitka-adult-v1.sitkap1fx.json`, `build/build-p0-core-ref.sh`, and every other existing worktree |
| Result document | one new Markdown on a **new** narrow docs branch — not this branch, not `main` |

**Qualification lane (separate, read-only, reserved now, unused until a Q1 packet exists):** scratch `/home/jer/Development/CCF-885-Qualification-Scratch-20261121` (absent), port `127.0.0.1:33841` (free), **read-only** against the future integrated SHA, no source writes, no Unity, no Windows build.

**Prohibited in both lanes:** any merge to `main`; any change to forestry equations, `AnnualRules.cs`, `AnnualEngine.cs`, `Domain.cs`, `WorkContract.cs`, the fixture or goldens; rebuilding protected P0 references except through the pinned `build-p0-core-ref.sh`; Unity; Windows build; touching #877, #882, #867 or the two existing 885 worktrees; force-release of any lease; self-granting of anything beyond this table.

**Lease term:** open-ended under D-091 (`rollForward`-style expiry is explicitly forbidden). It ends on (a) the integrator's documented completion/release, (b) owner-directed cancellation or handoff, or (c) a genuine conflicting-writer/safety/authority stop. **No fabricated expiry date is recorded.**

## 6. Corrections to earlier assumptions (recorded honestly)

1. The preflight's allowlist says **15** files; the actual candidate delta is **16 paths** — the 15 code/test files plus `Multi-885-MA1-TWO-AREA-SIMULATOR-SOURCE-IMPLEMENTATION-HANDOFF-20261121.md`. The extra path is documentation carried by the fast-forward and is not a source write.
2. `.p0ref` is **not a tracked path** at `4cecf873`; the P0 core reference is a **generated** artifact produced by `build/build-p0-core-ref.sh` and checked by `tests/p0-isolation.sh`. The integrator must hash the generated reference from the pinned script output, not look for a tracked directory.
3. The proposed preflight worktree name is accepted, but **no** preflight reservation existed — this receipt is the first physical grant.

## 7. Integrator scope (hand this to the separate integrator)

The integrator is a **separate local actor** (proposed: GPT-6.1 Sol / Codex on this host), never the branch author (Claude Opus 5.5) and never this custodian.

Required sequence, all inside the granted lane:
1. Fresh preflight: confirm worktree path absent, branch absent, base/candidate reachable, `main` untouched, #877/#882 listeners still as recorded here.
2. Create the worktree and branch at `e11dd519`; fast-forward to `4cecf873`; assert no merge commit and that `ac4c7b51` is **not** included.
3. Byte-compare the full product/test/dependency tree against `4cecf873` (`git diff 4cecf873 <head> -- Tools/MultiRackSitkaP1Lab` must be empty).
4. Run with `/home/jer/.dotnet/dotnet`: pinned SDK build, **Core 135/135**, **single-area smoke 287/287**, **multi-area smoke 127/127**, and the P0 isolation check produced by the pinned script.
5. Execute the **naturally-empty-area runtime history wording** check (populated-only branch and carried-empty branch) against the actually served candidate on `127.0.0.1:33811`.
6. Record exact hashes: the four pinned source files, fixture, generated P0 reference, and the 15 product/test files.
7. Stop the listener; leave the worktree in its final state; publish one integration result Markdown with the exact integrated SHA and a verdict of APPROVED / NEEDS CHANGES / REJECTED.

**Stop conditions:** any byte difference from `4cecf873`; any need to edit a protected file; a conflicting live process on the granted port; any missing pinned SDK; any temptation to fix a failure in the product code (that requires a new bounded source packet).

## 8. Remaining blockers and next authorised action

| Blockers | Status after this receipt |
|---|---|
| `blk-a1d10762dfdf7bee` (custody) | **RESOLVED BY THIS RECEIPT** — physical custody established at §2/§5 |
| `blk-8a73ce26bb0951d9` (lease) | **RESOLVED BY THIS RECEIPT** — one exclusive integration lane plus one separate read-only qualification lease |
| Source-ownership reconciliation | **RECORDED** in §3; the live `active-tasks.md` update is a separate bounded board write and was **not** performed here |
| Integrated/verified SHA | **STILL MISSING** — no integration has been executed |
| Q1 execution packet and harness bytes | **STILL MISSING** — a separate bounded packet after the integrated SHA exists |

**Next authorised action:** dispatch the separate integrator under §7. Then, and only after a verified integrated SHA exists, author the distinct Q1 qualification execution packet (harness bytes, seeds, logging, cases) against that SHA.

**Not claimed:** INTEGRATED, VERIFIED, OWNER ACCEPTED, MODEL CALIBRATED, G1. G2 owner playtest remains separate and pending.
