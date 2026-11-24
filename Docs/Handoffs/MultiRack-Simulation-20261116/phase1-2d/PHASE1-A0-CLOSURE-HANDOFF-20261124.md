# 2D Phase 1 — A0 Closure Handoff

**Issued by:** DeepSeek V4.1 Flash orchestrator, 2026-11-24.
**Approved candidate:** `8db5e09538474cec01374066be08d73f240de82c` on `feat/multi-2d-phase1-a0-20261124-v1` (base `cf6fe7a52aa096e824827c568f43fe05724dcce9`).
**Implementer:** Claude Opus 5.5. **Independent reviewer:** Claude Sonnet 5.5. **Verdict:** **APPROVED**.
**Status line:** `IMPLEMENTED: yes (contract level)` · `TESTED: contract level, independently reproduced` · `REVIEWED: APPROVED` · `INTEGRATED: no` · `VERIFIED: no — A1 not built` · `ACCEPTED: no`.

This document closes **A0 only**. It is not a save/load feature. A player cannot yet save or restore a running forest.

## 1. What A0 actually is

A strict, versioned, deterministic save **contract** for the 2D multi-area lab: envelope, complete typed state DTO, strict codec, consistency validator, engine-lineage header, and a path-confined named-slot store with atomic publish, prior-good backup and recovery. It runs as new files only and is deliberately **not wired into the engine**.

| File | Purpose |
|---|---|
| `src/Core/Phase1/Phase1SaveFormat.cs` | Format marker, schema **v2**, limits, value tags, model vector, genesis identity, `Phase1EngineLineage` |
| `src/Core/Phase1/Phase1SaveState.cs` | Complete root DTO; approved plan reduced to orders, no authority fields |
| `src/Core/Phase1/Phase1SaveEnvelope.cs` | Header (label, year, generation, models, genesis, lineage) + state |
| `src/Core/Phase1/Phase1SaveCodec.cs` | Deterministic writer; UTF-8 gate; strict reader |
| `src/Core/Phase1/Phase1SaveValidator.cs` | Cross-field consistency; refuses states the engine cannot produce |
| `src/Core/Phase1/Phase1SaveSlots.cs` | Named slots; `renameat2(RENAME_NOREPLACE)`, `link(2)` fallback, backup/recovery |
| `src/Core/Phase1/Phase1RootMapping.cs` | Internal A1 seam; compile-verified only |
| `tests/Phase1.Save.Tests/*` | Standalone 44-test suite |

## 2. Delivery history (all rounds recorded)

| Round | Artifact | Result |
|---|---|---|
| Implementation | `21df07c2` | 29/29 contract tests, additions-only diff |
| Independent review R1 | Sonnet 5.5 | **NEEDS CHANGES** — 5 blocking findings |
| Bounded repair R1 | `8db5e095` (`09a6ded6` code, `8db5e095` docs) | all five closed, 15 new tests |
| Focused re-review | Sonnet 5.5 | **APPROVED**, no new blocking findings |

Reviewer arbitration of the two judgement calls:
- **F2 deviation accepted.** A confirmed save may now replace an unreadable primary (the defect was that a normal save could not). The corrupt file is never promoted to backup. Typed refusals still hold for `Decode`/`Load`/`Inspect`.
- **F4f year-0 check correctly dropped.** Under ruling B1 the persisted plan carries only orders, so there is no authority field left to check. A1 must present it as a draft.

False-refusal check: the reviewer drove the real engine through 83 states (Phase 2, Stage 4, Stage 5, model 0; crop marks, pending/approved/cancelled/executed work, year-cancelled work, up to 28 years) through capture/encode/decode/restore — **0 refused**.

## 3. Verified gates (orchestrator-run, not self-reported)

| Gate | Command | Result |
|---|---|---|
| Core build | `dotnet build src/Core/SitkaP1.Core.csproj` | 0 warnings, 0 errors |
| A0 suite | `dotnet run --project tests/Phase1.Save.Tests` | **44/44 passed**, scratch removed (reviewer also ran Release: 44/44) |
| Regression compile | `dotnet build tests/Core.Tests/SitkaP1.Core.Tests.csproj` | 0 warnings, 0 errors |
| Diff vs base | `git diff --name-status cf6fe7a5..HEAD` | 12 additions, 0 non-`A` rows |
| Diff vs prior candidate | `git diff --name-status 21df07c2..HEAD` | 11 `M`, inside the A0 file set only |
| Boundary | path filter | no `Assets/**`, no `Tools/MultiRackSitkaLab`, no `src/Ui`, no `tests/Core.Tests`, no `Docs/Project`, no csproj/sln edits |

Reproduce from any checkout of the branch:
```
cd Tools/MultiRackSitkaP1Lab
export PATH=/home/jer/.dotnet:$PATH
dotnet build src/Core/SitkaP1.Core.csproj -v q --nologo
dotnet run --project tests/Phase1.Save.Tests -c Debug
```

## 4. Requirement state at A0 scope (approved by the reviewer)

| Req | Result | Basis |
|---|---|---|
| P1-01 full mutable root | PASS WITH LIMITS | Every root field modelled; real-engine probe parity; A0's own tests synthetic, mapping compile-only |
| P1-02 strict compatibility | PASS WITH LIMITS | Strict decode incl. UTF-8, duplicate keys, enum, overflow, canonical form, checksum; lineage now exact |
| P1-03 durable safe writes | PASS WITH LIMITS | Fault injection at every stage; no-clobber publish; Linux-x64 only, fail-closed elsewhere |
| P1-10 protected boundaries | PASS WITH LIMITS | Additions-only, no protected paths; frozen anchors NOT TESTED in this scope |

## 5. Known limits carried into A1 (mandatory, not optional)

1. **NB1 — residual clobber window.** `replaceExisting:true` on a slot that did *not* exist at the check still publishes with plain `rename`; a slot created in that window could be overwritten without a backup. **A1 must use no-replace whenever `!exists`, and must serialize slot operations.**
2. **NB2 — deferred order rules.** The validator does not reject a Fell order naming a Crop-marked tree, nor a tree holding both Fell and Prune orders. **A1 must revalidate every restored order against the restored root and refuse the load if any cannot be preserved.**
3. `link(2)` fallback path is untested (filesystem lacks `RENAME_NOREPLACE` on the test host).
4. `Phase1RootMapping` including the approval→draft split is compile-verified only; no runtime round-trip of every field.
5. Real power loss and the `ReplacedDurabilityUnconfirmed` path are untested.
6. Validator rules were derived from engine source at `cf6fe7a5`; **A1 must recheck them against the reviewed map candidate**.
7. Documentation defect NB3: the worker's handoff §1 still describes schema 1 and `^…$`; its §8 supersedes §1. This closure document is authoritative where they disagree.

## 6. B2 — what A1 must guarantee for load/session safety (reviewer ruling)

- Fresh random session id per load, never derived from the file.
- Executed idempotency map restored verbatim.
- Old-session fence refused for every command, including Execute-replay; a retry across a restart returns a stale-session refusal, not a replayed receipt.
- New session ids cannot collide with any prefix in loaded plan ids.
- `PlanStateRevision` restored exactly.
- Approved Fell/Prune/Clear/Plant orders returned to their pending lists and revalidated; the load fails closed if any cannot be preserved.
- Parity tests ignore session-prefixed ids.
- A1's UI must show a replaced-but-unreadable save plainly before overwrite (a `.corrupt-<time>` quarantine copy is a cheap optional addition).

## 7. B5 — Phase B copy obligations (map-owning writer's files, not A0)

Before any build exposes saving, these must stop claiming nothing is saved: `MultiSessionOnly`, `MultiChipSessionOnly`, "saving: NOT MODELLED" in `MultiAreaContracts.cs`; `PlayerMultiApi.cs`; `Ui/Program.cs`; `README.md`; and the tests pinning those strings.

## 8. A1 admission status

**A1 IS NOT ADMITTED.** It requires the shared `MultiAreaContinuation` / adapters / `PlayerMultiApi` / `Program.cs` / test registration, all owned by the live map lane (Pass 2 last seen at `9aaebeba`, committed locally, **not pushed, not independently reviewed**). Admission needs, in order:

1. Map Pass 2 closure or an explicit scope release.
2. A fresh collision check of branches, worktrees, HEAD and dirty state.
3. An owner/Manager **G4 shared-file integration grant** naming the exact reviewed candidate.
4. A new single-writer branch/worktree for A1.

Until then, no A1 or B work will be started, and no merge, PR or main write will occur.

## 9. What is explicitly NOT proven

A player cannot save, load, skip or review history. No root capture under the engine lock, no session fence, no load policy A, no annual skip controller, no paged history, no API/UI, no map-branch compatibility, no owner playtest, no acceptance.

**Owner action at this point:** none required for A0. The next owner decision is the A1 admission grant once the map lane closes.
