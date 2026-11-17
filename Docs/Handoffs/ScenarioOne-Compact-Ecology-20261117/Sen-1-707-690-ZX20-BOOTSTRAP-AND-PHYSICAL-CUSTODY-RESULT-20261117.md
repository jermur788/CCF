# Sen-1 #707 — #690 ZX20 NonUnity bootstrap and physical custody result

**PHYSICAL PREP PASS — CUSTODY ASSIGNED, SOURCE START HELD.**

- **Lane and worktree.** A new ZX20 `unity:false` lane `Sen1SaveA0ZX20NonUnity707` is registered. Its canonical-store worktree is `/media/jer/ZX20/CCF-Sen1-690-A0-20261117`, detached at D-073 `55863f1b7719011bba16b3f3ab377e379c0b988a` and clean.
- **Mandatory preflight.** The unmodified `lane-status --preflight` for the exact A0 branch returned **READY, exit 0, with no warnings** before assignment.
- **Board.** Exactly one board row, `CCF-690-SEN1-SAVE-A0`, now names **one exclusive source writer**. It carries a named-custodian expiry of **2026-11-18T00:00:00Z — ADVISORY / MANUALLY SUPERVISED — NOT ENFORCED BY TOOL**.
- **Not done.** The A0 branch was not created. No merge, C# edit, test, Unity, #652 install, saves, Prefs or Windows. #690 stays **BLOCKED / SOURCE START HELD** until Manager #700 issues a separate **[APPROVED TO START]**.

Read these two findings before approving: **F-1**, the expected post-assignment preflight mismatch and the writer's first step, and **F-2**, a concurrent board/lanes write by #710 made *inside* this session's exclusive flock window.

## Identity, authority and decisions

- **Worker and physical operator.** Anthropic Claude Opus 5.5 (`claude-opus-5-5`), Claude Code in T3 Code, session `fe9ea65b-60aa-4b0f-9a28-fdd50c27421d`, Linux `jer-laptop`, OS user `jer`, GitHub `jermur788`, Claude config `~/.claude_ccf2` (account pool not verified). This worker also produced #701. Before any physical write it posted [RUNNING](https://github.com/jermur788/CCF/issues/707#issuecomment-6961756979) with the start state: main `b3d963c8`, board `a3127cbb…`, lanes `4a390900…`, intended lane, path and branch, and the permitted state changes.
- **Authority.** The owner approved Option B at 21:32 UTC, per the [#707 packet](https://github.com/jermur788/CCF/blob/docs/sen-1-707-zx20-physical-preflight-dispatch-20261117-v1/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261117/Sen-1-707-690-ZX20-NONUNITY-CUSTODY-AND-PREFLIGHT-BOOTSTRAP-PACKET-20261117.md) (tip `c4955f26`), plus the direct user dispatch.
- **Owner/custodian decisions,** answered directly by the user in this session at about 21:41 UTC and [recorded on #707](https://github.com/jermur788/CCF/issues/707#issuecomment-6961785379) before any write:
  - **Custodian: USER jermur788.** They personally monitor the cutoff and stop/release the writer at or before it.
  - **Sole A0 source writer: this Claude Opus 5.5 session.** It holds no Multi lane. Multi #703 and #710 are held by *other* Claude Opus 5.5 sessions on the same host; their T3 session ids differ.
  - **Expiry:** 2026-11-18T00:00:00Z, advisory.

## Why ZX20 Integration was not reused, and why bootstrap was lawful

- The Ops README says "No per-task worktree creation. Reuse lanes only after checking ownership/dirty state."
- The only existing ZX20 `unity:false` lane, `Integration` (`/media/jer/ZX20/CCF-Integration`), is unusable. It sits on `integration/scenario-one-tier3-candidate` at `8f9ed802` with **799 untracked entries** (a `Scenario1V1/` project with Library) belonging to another owner. Reusing it would require switching or cleaning someone else's checkout, which is forbidden.
- Main records a Manager-approved procedure for exactly this case, [MANAGER-287-REGISTERED-WORKTREE-MIGRATION-PACKET](https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/UnityLock-20261106/MANAGER-287-REGISTERED-WORKTREE-MIGRATION-PACKET.md): create a new canonical-store ZX20 worktree with plain `git worktree add`, then register only that verified path as one new `lanes.json` entry under the Ops guard, with scripts and other lanes byte-identical. #707 followed it under owner and Manager #700 authority.
- The bootstrap did **not** assign or create the A0 branch. It used `--detach` at the pinned D-073 commit.
- The canonical store is already configured for exFAT (`core.filemode=false`, `core.symlinks=false`, `core.ignorecase=true`) and has no hooks. The checkout is 6 276 files, about 2.0 G on disk.
- Main has no ZX20 storage rule against NonUnity worktrees. The storage policy text in Primary's uncommitted `AGENTS.md` is user work in progress and not authority; it was left untouched.

## Physical sequence (exclusive flock on board directory inode 1399769)

**Run 1, 21:42:38.128Z–21:43:16.143Z (PID 788948).**
- Phase 0 PASS.
- `git --git-dir=/media/jer/Files/CCF/Repository.git worktree add --detach /media/jer/ZX20/CCF-Sen1-690-A0-20261117 55863f1b…` succeeded: clean, detached, registered.
- My own whole-store "refs unchanged" self-check then failed. Concurrent registered docs reviewer [#709](https://github.com/jermur788/CCF/issues/709) created `refs/heads/review/sen-1-709-world-attestation-nonopenai-20261117-v1` at 21:42:43Z for its own `/tmp` worktree. That branch is disjoint from A0 and was not caused by the bootstrap.
- The script stopped before touching `lanes.json` or the board. Their hashes were unchanged, and an independent exclusive probe at 21:43:16.148Z proved the lock released.
- This was disclosed in an [interim comment](https://github.com/jermur788/CCF/issues/707#issuecomment-6961859119) before resuming. The resumed ref gate covers the A0, D-073 and donor refs and A0-branch absence; other concurrent refs are logged.

**Run 2, 21:44:22.014Z–21:56:47.961Z (PID 791022).**

| Phase | Result |
| --- | --- |
| 0 — locked readback | PASS. Board `a3127cbb46132b47bdd77f22cad31c528af9952887c8435ca1b3566025f5809a` (identical to run 1), 37×17, `packet-lint --board` PASS, no duplicates. A0 row `BLOCKED / UNASSIGNED / NOT RUNNING`; no other row names the A0 lane, branch or path. 21 lanes, no name or path clash. Only VERIFY row is Multi #687 (disjoint). A0 branch absent in canonical store, legacy store and GitHub. D-073 checkout `/media/jer/ZX20/CCF-S1-ResearchCopy-V1-Qual` and donor checkout `/media/jer/Files/CCF/Operations/CCF-Scen1-SharedSave-T01-20261115` clean at `55863f1b` / `209a08e9`. Unity lock free, no Editor. ZX20 storage PASS. |
| 1 — bootstrap verify | Worktree registered and detached in the canonical list; HEAD `55863f1b`; common dir `/media/jer/Files/CCF/Repository.git`; 0 tracked/untracked/ignored entries; no Library; `/dev/sdd2` exFAT `18D6-896E`. |
| 1b — lane registration | One appended `lanes.json` object, a purely additive 6-line diff. Staged copy accepted by the deployed parser in strict mutation mode (22 lanes). An unknown lane name was rejected as a negative probe. Installed by same-directory replace; bytes verified; `lanes.json` → `ed79cd5a6d5da6d20bbb4aa8930335d821ac214a35ec688d8f3da1a3efe3d498`. |
| 2a — **mandatory preflight** | Deployed `lane-status` `e7102a83…173f` and `_ccf.py` `ab4343ef…de001`, unmodified. `lane-status --preflight --lane Sen1SaveA0ZX20NonUnity707 --branch integration/sen-1-d073-shared-save-a0-20261117-v1`, 21:44:38–21:50:21Z → **`READY`, exit 0**. Target terms: `ZX20: YES`, `unity=False`, `exists=True`, `registered=True`, `dirty=False`, `owner=unassigned`, **`warnings: none`**. Storage READY / PASS, 49.44 GiB free vs 15 GiB. |
| 2b — one-row custody edit | Staged board lint PASS. Diff is exactly 1 old and 1 new line (row `CCF-690-SEN1-SAVE-A0`). Installed by replace; bytes verified; live lint PASS 37×17. In-lock board → `5384b1b67abf3d8da29a6ce096b9a998f00d9ec8cedf5d85086fb679666dc95d`. Field-level comparison: only the A0 row changed (Owner, Harness, Model, Quota source, Lane, Locks, Open gate, Dependency, Next action). State stays `BLOCKED`; Branch, HEAD and Subsystem are unchanged. |
| 2c — post-edit | Inventory taken. Post-edit preflight exit 1, see **F-1**. Lock released 21:56:47.961Z; independent exclusive probe acquired 21:56:47.977Z (PID 799557, exit 0). |

**New A0 row, recorded as written:**
- **Owner:** `EXCLUSIVE ASSIGNED SOURCE WRITER: Anthropic Claude Opus 5.5, T3 Code / Claude Code session fe9ea65b (the #701/#707 worker) on jer-laptop; owner-named 2026-11-17 #707; SOURCE START HELD / AWAIT MANAGER #700 APPROVED TO START; NOT RUNNING`
- **Lane:** `Sen1SaveA0ZX20NonUnity707`. **Branch:** `integration/sen-1-d073-shared-save-a0-20261117-v1`. **HEAD:** `55863f1b…`.
- **Locks:** the ZX20 path and canonical store; "bootstrapped DETACHED at 55863f1b; A0 branch NOT yet created"; the 15 Assets + 2 reviewed Tools scope link; donor Docs/specs excluded; `EXPIRY 2026-11-18T00:00:00Z ADVISORY / MANUALLY SUPERVISED - NOT ENFORCED BY TOOL`; named custodian USER jermur788; the release procedure (writer posts STOPPED/RELEASED on #690 and the custodian re-reads the row); the old Files lane `Sen1SaveA0NonUnity690` superseded for #690, its path never created.
- **Open gate:** separate Manager #700 APPROVED TO START; writer then registers RUNNING, refreshes expiry and ownership, and creates the A0 branch in this worktree.
- **Next action:** if not started or finished by expiry, the custodian returns the row to `BLOCKED / UNASSIGNED`.

## Findings for Manager #700

**F-1 — post-assignment preflight shows the expected branch-name mismatch (not a conflict).**
- After the row edit, the same unmodified preflight returned exit 1 with exactly one warning: `board branch/HEAD differs: CCF-690-SEN1-SAVE-A0`.
- This is by design. The row names the intended A0 branch, but #707 was forbidden to create it, so the worktree stays detached (`branch=HEAD`). Every conflict term remains clean: storage, duplicates, dual-store branch conflicts and dirty state.
- Required writer sequence after APPROVED TO START:
  1. Post `[RUNNING — A0 SOURCE]` on #690 and re-read the row, expiry and live coordination.
  2. Run `git -C /media/jer/ZX20/CCF-Sen1-690-A0-20261117 switch -c integration/sen-1-d073-shared-save-a0-20261117-v1 55863f1b7719011bba16b3f3ab377e379c0b988a`. This branch-creation step is the first source operation.
  3. Re-run the unmodified preflight. The row's branch and HEAD will then match, so READY / exit 0 is expected. **Stop if it is not READY.** Only then begin the merge.

**F-2 — concurrent #710 write inside this session's exclusive flock window.**
- At **21:52:34Z** both live files were rewritten by the [#710](https://github.com/jermur788/CCF/issues/710) Multi custodian, another Claude Opus 5.5 session.
- That is inside run 2's exclusive board-directory flock, so that writer either did not take this flock or did not wait for it.
- **Nothing of #707 was lost.** Live board `ef2d97720b0939dd59f9ada25fcc82a97794a59eed474856ab5bef6148a273f6` with the appended `2026-11-17 PHYSICAL OPS — #710 …` paragraph removed reproduces my in-lock hash `5384b1b6…` exactly. Live `lanes.json` `1b1cc0054cb7a968abc87dafd9e1044b30fdbfc529de33b4057dc42b34861bc3` with the `MultiP1ModeRepair710` lane removed reproduces `ed79cd5a…` exactly.
- Live board still lints at 37×17, and live `lanes.json` holds 23 lanes.
- Manager #694 and #700 should remind custodians to use the shared board-directory flock.

## Before/after and negative checks

- **Canonical worktree list** (run 1 pre-bootstrap → run 2 after): only `+ /media/jer/ZX20/CCF-Sen1-690-A0-20261117  HEAD 55863f1b  detached`.
- **Canonical refs:** A0, D-073 and donor refs are unchanged (`55863f1b` and `209a08e9`, local and origin). There is no A0 ref anywhere; GitHub has 0. The 7 other ref changes logged during run 2 were disjoint concurrent activity: #708/#709 review branches and T3 checkpoint refs.
- **Legacy store** refs and worktrees are unchanged.
- **Checkouts:** D-073, donor and Primary are unchanged (Primary keeps 2 pre-existing user modifications).
- **Other paths:** the Files path `/media/jer/Files/CCF/Worktrees/CCF-Sen1-690-A0-20261117` was never created, and the ZX20 staging dir was removed.
- **Unity lock** is free with no Editor.
- **Ops software unchanged:** `deployment.json` and all scripts have the same hashes. No validator, README or role edit; no #652 install.
- **No source work:** no branch creation, merge, C#, compile, test, Unity, saves, Prefs or Windows.
- **Evidence and logs** (both run scripts, locked copies, diffs, preflight outputs, inventories): `/media/jer/Files/CCF/Verification/Sen1-707-690-ZX20-Custody-20261117/`.

## Status

- **#707 HANDED-OFF and left open** for the Manager #700 verdict.
- **#690 is not RUNNING or APPROVED.** Custody expires **2026-11-18T00:00:00Z** (advisory). If Manager approval and the source work do not fit in that window, the custodian (jermur788) returns the row to BLOCKED / UNASSIGNED, or a fresh custody renewal is recorded.
