# Sen-1 #701 — #690 owner exception: physical custody verification result

**BLOCKED / UNASSIGNED / NOT RUNNING.** The owner exception is on record and every conflict check passed. Two of the packet's own stop conditions were met, so I assigned no writer, reserved no window and left the physical board unchanged:

1. **Branch-assignment preflight is mandatory and returns BLOCKED for this lane.** The deployed Ops README and the Execution Manager role both require `lane-status --preflight` before a branch is assigned, and the README says not to bypass it. The deployed predicate needs the target to be on ZX20 *and* to exist as a registered worktree. The A0 lane is on Files/ext4 and its checkout cannot exist before assignment. The exception forbids bypassing or changing Ops controls, so assigning the A0 branch now would mean either ignoring a BLOCKED preflight or changing the validator. The packet calls for a separate policy decision in this case.
2. **The deployed controls cannot enforce an expiry on source custody.** The board has 17 fixed columns and none of them is an expiry. The only time field in the toolset is the `unity-lock` `eta`. That tool refuses non-Unity lanes, only *reports* `eta_overdue`, and keeps ownership until someone releases it explicitly. The expiries on the Multi rows are free text in the Locks column. The packet says to stop rather than present advisory text as an enforced lease.

Nothing else blocks. A Manager or owner decision on the two points above is the only remaining gate before custody can be assigned.

## Actual worker and authority

- Worker: **Anthropic Claude Opus 5.5 (`claude-opus-5-5`)**, Claude Code harness in T3 Code, Linux `jer-laptop`, OS user `jer`, GitHub `jermur788`, Claude config dir `~/.claude_ccf2` (account pool not independently verified). Registered before any readback: [#701 RUNNING](https://github.com/jermur788/CCF/issues/701#issuecomment-6961037023).
- Authority: direct user dispatch plus the [Manager #700 packet](https://github.com/jermur788/CCF/blob/docs/sen-1-700-manager-intake-20261117-v1/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261117/Sen-1-701-690-OWNER-EXCEPTION-AND-PHYSICAL-CUSTODY-VERIFICATION-PACKET-20261117.md) (branch tip `b84cb905`).
- Owner exception: recorded in live main `Docs/Project/coordination/active-tasks.md` (commit `93abe68b`, entry dated 2026-11-17 20:55 UTC) and in the #690 body. It covers a one-off direct Git-only Files operation for #690, limited to the registered isolated Files worktree and the canonical store, with one exclusive time-limited writer and **no bypass or modification of Ops/Unity safeguards**. I verified the record. I did not verify the owner's words independently of Manager #700's transcription.
- Live main at start and at publication base: `8f6bca0b71782081835e0482a7025dab2f657b8b`.

## Physical readback under the board-directory flock

Board directory inode 1399769. Shared flock held 21:07:55.516Z–21:07:55.795Z (PID 772641). An independent exclusive non-blocking probe acquired the lock at 21:07:55.799Z (PID 772741, exit 0), which proves no holder remained. No exclusive maintenance lock was taken because no board edit was justified.

| Item | Observed |
| --- | --- |
| `board/active.md` SHA-256 | `a41cb50edc2c43c22bfd881649124b990eccab5e3448e7b38f5f99b50a7c7679` (81 844 B, mtime 20:37:05Z) — unchanged before and after |
| `lanes.json` SHA-256 | `e6cf074231d16620a9a680eaf719dbc4fc2d8eafe77ca3074695c55c3e7ae3a3` — unchanged |
| `deployment.json` | `8c66b290c247708846cb8b59f83626a10a3fa1d052397a3316dc5f23fc157b18` |
| `lane-status` / `_ccf.py` | `e7102a83ad942cc7365cb22db9a8f79823744ea98bc2cf8a6738a11f1897173f` / `ab4343efc7d1b82c5a4aca747d0e09c614779cfd9aa406a0dd681d204f5de001` |
| `packet-lint` / `unity-lock` | `a776ec42a13224936a5c7c6a68ec2ed7ddc4e2cf21a96e287e6843a31e243e43` / `54cf0a95561d0a8c350d89adbee83f393990b728d6127b94f69c587260f048e5` |
| Board validation | deployed `packet-lint --board` PASS, **37 rows × 17 columns**, zero duplicate Lane/Branch conflicts; 20 configured lanes |
| A0 row | exactly one `CCF-690-SEN1-SAVE-A0`, `BLOCKED`, owner `UNASSIGNED / NOT RUNNING`, lane `Sen1SaveA0NonUnity690`, branch `integration/sen-1-d073-shared-save-a0-20261117-v1`, HEAD `55863f1b…`; no other row names the A0 lane, branch or path |
| A0 lane | `/media/jer/Files/CCF/Worktrees/CCF-Sen1-690-A0-20261117`, `unity:false` |
| Live rows in ACTIVE/VERIFY/INTEGRATE | only `CCF-687-P0P1-INTEGRATION` (VERIFY; isolated Multi store; disjoint) |
| Save/D-073 rows | #544, #638, #647, #403 `DONE-PENDING-LEDGER`; #412 `WAITING` (user personal Editor lane, no agent writer) |
| Unity | `unity-lock status`: free, no metadata, no Editor PIDs; no Unity process observed |
| Storage | ZX20 `/dev/sdd2` exFAT `18D6-896E` rw, 51.41 GiB free vs 15 GiB (PASS). Files `/dev/sdb4` ext4 `2283b135-bf37-42d6-921f-2fb861d0a8d4` rw, about 47.7 GiB and 28.5 M inodes free |

### Deployed preflight for the exact A0 assignment

`lane-status --preflight --lane Sen1SaveA0NonUnity690 --branch integration/sen-1-d073-shared-save-a0-20261117-v1` (deployed, unmodified, read-only), 21:08:02Z–21:13:39Z: `Storage: READY / PASS`, then **`BLOCKED: resolve assignment conflicts before dispatch`, exit 1**. Board and config hashes re-read after the run were unchanged.

The A0 target row shows ZX20: NO, exists=False, registered=False and the warning `lane absent; no creation performed`. Storage is READY. There are no board duplicates and no dual-store branch conflict. So the only failing terms are the structural `zx20` and `exists`/`registered` predicates, not an ownership conflict. That detail matters for the decision below. It does not make the result a PASS.

## Source and store readback (read-only, `GIT_OPTIONAL_LOCKS=0`)

- Intended checkout absent. Its ancestors and `/media/jer/Files/CCF/Repository.git` are real directories, not symlinks. `Worktrees/` is writable.
- The A0 branch has 0 refs in the canonical store, 0 in the legacy store, 0 on GitHub, and appears in no worktree list (canonical 181 entries, legacy 45).
- D-073 first parent `55863f1b7719011bba16b3f3ab377e379c0b988a` and V2 donor second parent `209a08e9f220841aaec81385d4cb7b437fa5089f` exist as commits. The canonical branch refs match GitHub. Merge base is `50042bbe97734b031210ddebb603b5ec00ff459d`.
- Checkouts are clean, with zero tracked or untracked entries: `/media/jer/ZX20/CCF-S1-ResearchCopy-V1-Qual` at D-073 and `/media/jer/Files/CCF/Operations/CCF-Scen1-SharedSave-T01-20261115` at the donor. Both were preserved.
- Donor-side delta is 15 Assets, 2 Tools, 136 Docs and 12 specs. The 15 Assets and 2 Tools paths exactly match the Manager's 17-path ceiling. Files changed on both sides: `ForestSaveController.cs`, `ForestStartingStand.cs`, `ScenarioOne/ScenarioOneManager.cs`.
- Live coordination (main `8f6bca0b`) names no current writer on the A0 branch, path or the 17 files. #544 is handed off; #443 is on integration hold and excluded from A0. Multi #687/#697 (and closed #674/#683) use isolated task-local stores and disjoint `Tools/MultiRack*` scopes. The A0 writer would need to be a session other than those Multi custodians. No writer was selected, so this check never had to be applied.

## Why each gate stops here

| Gate | Result |
| --- | --- |
| Owner exception exists and is narrow | PASS (recorded by Manager #700; see above) |
| No conflicting writer on file set, branch, path, stores or Unity | PASS |
| A0 path and branch absent; pins clean | PASS |
| Direct Git-only route usable **without bypassing a mandatory control** | **BLOCKED**: README §"Dual Git stores and assignment" and `roles/execution-manager.md` require `lane-status --preflight` before branch assignment, and it returns BLOCKED for this lane |
| Enforced expiry or renewal under existing controls | **BLOCKED**: no supported mechanism (board has no expiry field; `unity-lock` is Unity-only and its ETA is advisory) |
| One-row board edit | NOT PERFORMED (conditional on the gates above) |

I did not create a substitute enforcement mechanism such as a timer, Git hook, permission change or validator patch. Each of those would modify Ops controls or the shared store, which the exception forbids.

## Decision needed (Manager #700, owner if required)

Pick one:

- **(A) Policy clarification for the exception.** State in writing that, for #690 only, the owner exception displaces the README/role requirement for a READY `lane-status --preflight` before branch assignment. The A0 check would then require all of the preflight's conflict terms (storage, board duplicates, dual-store branch conflicts) to be clean, and they are clean today. Then accept **advisory, custodian-attested expiry** for this one operation: an exact UTC deadline in the A0 row's Locks/Next action text, the worker's own RUNNING/STOP comments on #690, and a custodian readback and release at or before the deadline. That is the same model the Multi #674/#687/#697 rows use. It must be described as advisory, not enforced.
- **(B) ZX20 alternative.** Approve and register a distinct ZX20 NonUnity source lane whose worktree a custodian creates and registers before assignment, so the unmodified preflight can return READY. Expiry would still be advisory under (B), so Manager #700 must also accept advisory expiry or the gate stays blocked.

After either decision, a fresh custodian edits only the existing A0 row under an exclusive maintenance flock, with before/after hashes. Manager #700 then issues **[APPROVED TO START]**, and only after that does the A0 writer register [RUNNING — A0 SOURCE].

## Negative checks / what this session did not do

No physical board or `lanes.json` write (hashes unchanged). No Ops script, config or validator change. No #652 install. No source worktree or branch, fetch into the canonical store, merge, C# edit, compile or test. No Unity, saves, Prefs or Windows. The pre-existing modified `AGENTS.md` and `Docs/Operations/StorageArchitecture.md` in `/media/jer/Files/CCF-Primary` were left untouched. Local evidence (readback logs and copies of the locked board and config) is under `/media/jer/Files/CCF/Verification/Sen1-701-690-Custody-20261117/`. This result was published as one new Markdown file through the GitHub API, with no local checkout.
