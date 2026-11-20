# Multi-StandMap-Playable-2D — Parent Orchestration Ledger

**Status:** ADMINISTRATIVE LEDGER / PARENT ADMITTED / PRODUCTION HELD
**Date:** 20 November 2026
**Task issue:** [#849](https://github.com/jermur788/CCF/issues/849)
**Manager issue:** [#730](https://github.com/jermur788/CCF/issues/730)
**Binding packet:** [Multi-StandMap-Playable-2D One-Shot T3 Fork Orchestration Handoff](https://github.com/jermur788/CCF/blob/docs/Multi-StandMap-Playable-2D-OneShot-Orchestration-20261120-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-StandMap-Playable-2D-OneShot-T3-Fork-Orchestration-Handoff-20261120.md)
**This ledger branch:** `docs/multi-standmap-playable2d-ledger-20261120-v1` (base `4bb80973`)

This document is written by the **administrative parent only**. It records admission evidence, allocation, gates and observed state. It does **not** contain production code, tests, UI or substantive specification, and it is not a design approval.

---

## 1. Actors and actual availability

| Role | Model | Availability check | Evidence |
|---|---|---|---|
| Administrative parent (no development) | DeepSeek V4.1 Flash 1017 / OpenCode desktop (T3 Code) | Running | This thread |
| Primary implementer / integrator | **Claude Opus 5.5** (`claudeAgent` / `claude-opus-5-5`) | **Verified present and enabled** | `orchestrator_capabilities`, effort `xhigh` |
| Independent reviewer (Phase IV) | **GPT-6.1 Sol** (`codex_3` / `gpt-6.1-sol`) | **Verified present and enabled** | `orchestrator_capabilities` |
| Bounded basic delegated subtasks | Claude Sonnet 5.5 (`claudeAgent` / `claude-sonnet-5-5`) | Verified present | `orchestrator_capabilities`; not yet used |
| Exceptional stuck-only escalation | Astra (`codex_3` / `gpt-6-astra`) | Verified present as an available model | **Not authorised, not used**; no blocker raised |

No model name was invented. `gpt-6.1-sol` exists exactly as named in the packet, so the "Opus unavailable → Sol becomes author, reviewer must change" contingency did **not** trigger.

## 2. G0 admission checks performed

### 2.1 Orchestration skill (Step 0)

- The packet requires reading the **installed** `orchestrate-work` `SKILL.md` and references on the native host, and reports `SKILL_UNAVAILABLE` if missing.
- **Finding:** `orchestrate-work` is **not installed** on this host. Searched: `C:\Users\hp\.agents\skills`, `C:\Users\hp\.claude\skills`, `C:\Users\hp\.codex\skills`, the WSL home equivalents, and the T3/OpenCode skill roots. Installed CCF-relevant skills are `ccf-orchestration`, `file-preserving` helpers (`file-pr`, `find-skills`, `implement-task`).
- **Owner resolution:** direct owner instruction in this session — use the installed **`ccf-orchestration`** skill when `orchestrate-work` is unavailable. Loaded and applied. This is an **owner-authorised substitution**, not a silent redefinition. No ad-hoc shell "orchestration" was improvised.

### 2.2 Live coordination refresh

- Live `Docs/Project/coordination/active-tasks.md` fetched from `main` (`7758e17c882fad01970d2f2fb8c09586bbcab5c1`, 812 460 bytes) at admission, not from the stale local copy.
- Live branch heads verified by `git ls-remote`:
  - `main` = `7758e17c`
  - `fix/multi-a1-workrevision-fence-20261119-v1` = `4bb809732fb691f15630e1fbc5ea20dca6dbbe2f` (**exact A-1 base, matches packet**)
  - docs packet branch = `7ec4a7a1`
- No `[RUNNING]` writer was found for `Tools/MultiRackSitkaP1Lab/**` in the live coordination file at admission. Historical A-1 lane (#846-era) is not claimed as a live lease; a **new** branch and worktree were created rather than borrowing it.

### 2.3 Source lane actually allocated

- **New exclusive worktree:** `/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-StandMap2D-20261120` — clean, verified `git log -1` = `4bb80973`, `git status --porcelain` empty at creation.
- **Implementer branch:** `feat/multi-standmap-playable2d-20261120-v1`, descended from `origin/fix/multi-a1-workrevision-fence-20261119-v1` (= `4bb80973`) via a prepared T3 worktree.
- **Admin/ledger branch:** this document's branch, same base, no source paths.
- A **separate clean reviewer lane** is intentionally deferred until an exact candidate SHA exists; creating it earlier would risk it drifting from the reviewed commit.

### 2.4 Pre-existing user work (protected, untouched)

- The `main` checkout on this host has ~570 pre-existing uncommitted modifications (art binaries under `Assets/ForestPrototype/Art/**`, verification evidence under `Docs/Verification/**`, scripts under `Tools/Verification/**`). **Not touched, not stashed, not reset.**
- The A-1 receipt worktree shows 568 pre-existing entries. Four of them are inside the lab and are **mode-only** differences (`mode change 100755 => 100644` on `build/build-p0-core-ref.sh`, `tests/build-output-safety.sh`, `tests/cli-safety.sh`, `tests/p0-isolation.sh`) caused by accessing the Linux checkout over a Windows/WSL path. Content diff is **zero lines**. Recorded, not "fixed", because it is not this task's material.
- Work was therefore done in a **new clean worktree**, never in a dirty one.

### 2.5 Toolchain reality (verified)

- `global.json` in the lab pins SDK `8.0.133` with `rollForward: latestPatch`.
- Host `dotnet` on `PATH` (`/usr/bin/dotnet`) is SDK **10.0.115** and alone would fail the pin.
- SDK **8.0.134** is installed at `~/.dotnet/sdk` and satisfies `latestPatch`. `~/.dotnet/dotnet --version` returns `8.0.134` inside the lab directory.
- Consequence: all authorised build/test commands must use `~/.dotnet/dotnet` (or `DOTNET_ROOT=$HOME/.dotnet`). This is an environment note, **not** a source change and **not** a gate waiver.
- `git push` from WSL to `origin` works (`--dry-run` returns success). `gh` is authenticated as `jermur788` with `repo` scope, so GitHub-tracked records are publishable.

### 2.6 Physical Ops reservation — NOT VERIFIED (open)

- The packet requires a genuine, exclusive, supported physical reservation for the source/compile/test lane.
- This host has **no accessible physical Ops board**: there is no Ops board artifact in `Docs/Project/**` on `main`, and no connected board/Drive integration is available in this session.
- Therefore the parent **cannot assert a granted physical lease**. Unity is not required by this slice, so the immediate exposure is limited to source/compile/test contention on this machine.
- **Gate:** owner confirmation that no other agent currently writes or builds in `Tools/MultiRackSitkaP1Lab/**` on this host. Until then the lane is exclusive only by declaration and by the fact that a fresh worktree was created.

## 3. Phase state

| Phase | Content | State |
|---|---|---|
| I | Exact map dependency + architecture proof (option A/B) + bounded spec/plan/tasks/checklist delta + exact writable path list + base build/test proof | **RUNNING — delegated to Opus 5.5** |
| II | Same-lock immutable player session facade on existing `AnnualContinuation` WorkRoot | HELD — needs Phase I spec acceptance |
| III | Actual playable map/editor frontend | HELD — same gate |
| IV | Author Tier1 qualification + independent GPT-6.1 Sol Tier2 on the exact candidate | NOT STARTED |
| V | Single exact candidate branch + consolidated handover to #730 | NOT STARTED |

Phase I is deliberately limited: the packet makes the standalone UI host/map adapter **new architecture**, so production writes require the spec delta to be accepted. Phase I produces the evidence needed for that decision without writing production source.

## 4. Open gates

1. **#730 Manager acceptance of the Phase I spec delta** before any Phase II/III production write (new standalone UI runtime and/or Scenario One map ABI).
2. **Scenario One map read/compile grant** — not requested in advance. If Opus selects option **A** (compile-linked reuse) the exact transitive closure and pinned blobs must be approved by the Scenario One source owner first. Option **B** (minimal local, presentation-compatible renderer written inside the lab) requires no cross-project grant and is the lawful fallback; it must not copy Scenario One code under an implied grant.
3. **Physical Ops reservation** (§2.6).
4. **G5** only if a genuinely material frontend/runtime/scope decision cannot be made inside the approved envelope. Not triggered so far.

## 5. What this ledger does NOT claim

- No production code, test, UI, spec or asset has been written by the parent.
- Nothing is IMPLEMENTED, REVIEWED, INTEGRATED, VERIFIED or ACCEPTED.
- No Unity Editor run, no Unity/Windows build, no `Assets/**` change, no fixture/golden/`.p0ref` change, no main merge.
- No claim of visual parity with Scenario One (no reference render was captured).
- The A-1 / G2 qualified baselines and the ten no-work Year0–10 digests are unchanged by this ledger.

## 6. Next actions

1. Collect the Phase I commit SHA and evidence from the Opus 5.5 lane.
2. Record that SHA here and on [#849](https://github.com/jermur788/CCF/issues/849).
3. Obtain #730 Manager acceptance of the spec delta (and, if option A, the Scenario One grant).
4. Only then dispatch Phase II/III production writes in the same lane, followed by Tier1 and independent Tier2.

---

## 7. Addendum — line-ending hazard in prepared worktrees (found during Phase I)

Observed while Phase I was running, and confirmed by direct inspection:

- The shared repository configuration on this host sets `core.autocrlf=true`. Any checkout performed with that setting materialises CRLF in the working tree.
- The lab's `CcfSourcePins` / `.p0ref` integrity checks compare **bytes/SHA-256**, so a CRLF-mangled checkout can fail a pin check that would pass on an LF checkout. This is an environment property, not a source defect.
- WSL global git has `core.autocrlf` unset (Linux default = false), so a **fresh clone** made from WSL is LF-clean and is the correct reference lane for builds and pin verification.
- The T3-prepared implementer worktree is a Windows-path checkout and therefore carries the hazard; the implementer has been verifying against a pristine LF clone instead. That is acceptable as a **baseline proof**, but the production lane for Phase II/III must be LF-clean (fresh WSL clone, or `core.autocrlf=false` set before checkout), and the pin check is the arbiter — never a hand-edited hash.
- Related caution already sent to the implementer: several hundred `Assets/ForestPrototype/Art/**` files show as modified in **every** checkout on this host without any edit (checkout/filter artifact). Therefore `git add -A`/`commit -a` is forbidden in this task; only explicit paths may be staged, and `Assets/**` must not appear in any commit.
- The WSL side also reports the Windows-created worktree as `prunable`; `git worktree prune` must not be run, because it would delete that admin directory.
---

## 8. Phase I result recorded (parent, administrative)

**Implementer:** Claude Opus 5.5. **Result commit:** `786a86372f87738fdc8d5f93aaab922d3f78a2f3` on `feat/multi-standmap-playable2d-20261120-v1`, one docs-only commit, **5 files / 918 lines**, **zero production source**.

Files:
- `Docs/Handoffs/MultiRack-Simulation-20261116/Multi-StandMap-Playable-2D-PhaseI-Architecture-Proof-20261120.md`
- `specs/multirack-sitka-2d-playable-standmap-v1/{spec.md,plan.md,tasks.md,checklists/requirements.md}`

**Map-path decision: Option B** — minimal local, presentation-compatible layer inside `Tools/MultiRackSitkaP1Lab`. Reasons recorded by the implementer, all specific and checkable against `main` `7758e17c`:

1. the V-01 grant covers only `SitkaGrowthModel.cs` + `SimulationRandom.cs`; the closure is **8 files / 2,030 lines** and needs a new Scenario One grant the implementer cannot self-issue;
2. the closure carries the **forbidden rack domain** (`RackPoint` lives in `RackGeometry.cs` with the full rack-planning model), so linking it would compile rack planning into the Multi assembly;
3. `MapTree` has **no `TreeId`**, so exact selection would need Scenario One edits or the fragile derived/nearest identity the packet forbids;
4. the work-state vocabulary (`MapTreeMark{None,Fell,Crop}`, rack-worded legend) cannot express pending ≠ approved ≠ executed;
5. the canopy raster yields pixels with no per-tree identity or assistive-technology object.

**Environment proof:** in a pristine LF clone the base builds with **0 warnings, 0 errors, 66/66 tests passing and all ten no-work digests matching**. The Windows-path editing lane fails 3 tests for environmental reasons only (2 WSL-git resolution, 1 CRLF byte difference), so **Tier-1 must run on the pushed SHA in a WSL-native LF clone** — consistent with §7 above.

**Two gates now open and not self-grantable by the parent:**

- **#730 Manager acceptance** of the spec delta and its six explicit qualifications Q-1…Q-6 (Q-5 is a G4-class new public ABI; Q-1 would narrowly qualify the "no Host/server/network" prohibition for a loopback presentation process).
- **Owner G5: frontend runtime.** UI-A = single-session loopback-only Kestrel process + embedded page (recommended; ASP.NET Core shared framework already installed, real Windows-browser accessibility testing); UI-B = native GTK 3 window over WSLg (no network, but unverified Windows screen-reader support and no per-tree accessible objects). The implementer correctly refused to self-grant either.

**Still true:** nothing is IMPLEMENTED, REVIEWED, INTEGRATED, VERIFIED or ACCEPTED. No production path was written; the proposed writable list (12 paths, two of them justified modifications to `AnnualContinuation.cs` and a one-line test registration) has not been granted.
---

## 9. Owner G5 decision recorded (parent, administrative)

**20 November 2026 — owner selected UI-A:** one single-session, loopback-only presentation process (`127.0.0.1`, ephemeral port, capability path + Host/Origin guards) serving an embedded HTML/CSS/JS page that calls the in-process adapter. UI-B (native GTK 3 over WSLg) is not selected.

This resolves the **owner** half of the frontend boundary only. It does **not**:

- amend the approved Stage 5 spec — qualification **Q-1** still needs #730 acceptance;
- grant any source right, writable path, or G4-class ABI approval (Q-5 remains open);
- authorise any production write. `src/Ui/**` remains a proposal, unwritten.

Production therefore stays HELD on #730 acceptance of the delta and Q-1…Q-6, plus the physical-lane confirmation in §2.6.
