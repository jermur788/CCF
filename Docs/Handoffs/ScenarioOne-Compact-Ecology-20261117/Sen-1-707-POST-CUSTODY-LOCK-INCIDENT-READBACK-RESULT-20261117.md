# Sen-1 #707 — post-custody lock-incident readback (addendum)

**PASS (read-only lock-incident reconciliation). #690 SOURCE START HELD.**

- **No third-party change since #707's result.** Live board and `lanes.json` are byte-identical to the post-#710 state recorded in the [#707 result](https://github.com/jermur788/CCF/blob/0a8a0f5d15c121248d7dade5d55d72efda88614c/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261117/Sen-1-707-690-ZX20-BOOTSTRAP-AND-PHYSICAL-CUSTODY-RESULT-20261117.md).
- **A0 custody intact.** The A0 row and lane match what #707 recorded, and the ZX20 worktree is clean and detached at D-073.
- **Preflight shows only the expected mismatch.** The unmodified mandatory preflight returns exit 1 with exactly one warning, the expected `board branch/HEAD differs: CCF-690-SEN1-SAVE-A0` (F-1). This exit 1 is **not** relabelled as READY.
- **#710 has confirmed lock compliance.** The #710 physical actor confirmed on GitHub that it took no lock for the writes, that every future board/lanes write will take the same board-directory flock, and that it does not overlap A0.
- **Custodian reconfirmed.** The custodian reconfirmed supervision of the advisory expiry, 2026-11-18T00:00:00Z.

Nothing was written to the board, lanes, Ops, the A0 branch, source, Multi rows, Unity, saves or Windows.

## Worker and authority

- **Worker:** Anthropic Claude Opus 5.5 (`claude-opus-5-5`), Claude Code in T3 Code, session `fe9ea65b-60aa-4b0f-9a28-fdd50c27421d`, Linux `jer-laptop`, OS user `jer`, GitHub `jermur788`. This session is also the named A0 writer, which is not exercised here.
- **Registration:** re-registered read-only on #707 before any check: [registration](https://github.com/jermur788/CCF/issues/707#issuecomment-6962317803). Start ref was live main `f52a0838…`; main at publication was `d7014d4c1ae80c332c89bf5d6689ce51546307cc`.
- **Authority:** [Manager #700 narrow lock-incident packet](https://github.com/jermur788/CCF/blob/docs/sen-1-700-manager-intake-20261117-v1/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261117/Sen-1-707-MANAGER-PHYSICAL-LOCK-INCIDENT-NARROW-RECONCILIATION-PACKET-20261117.md) (tip `a9c89709`).

## 1. Exclusive read-only board snapshot

- **Lock:** exclusive non-blocking flock on `/media/jer/ZX20/CCF-Ops/board` (inode 1399769), held 22:05:24.007Z–22:05:24.457Z (PID 804299). An independent exclusive probe acquired it at 22:05:24.461Z (PID 804425, exit 0).
- **Snapshot:**

| Item | Observed |
| --- | --- |
| `board/active.md` | `ef2d97720b0939dd59f9ada25fcc82a97794a59eed474856ab5bef6148a273f6`, 84 753 B, mtime 21:52:34.12Z (the #710 write) |
| `lanes.json` | `1b1cc0054cb7a968abc87dafd9e1044b30fdbfc529de33b4057dc42b34861bc3`, 9 108 B, mtime 21:52:34.12Z |
| Ops software | `deployment.json` `8c66b290…7b18`, `lane-status` `e7102a83…173f`, `_ccf.py` `ab4343ef…de001`, `packet-lint` `a776ec42…3e43`, `unity-lock` `54cf0a95…48e5` — all unchanged |
| Validator | deployed `packet-lint --board` PASS, **37 rows × 17 columns**, no duplicate Lane/Branch conflicts |
| Lanes | 23, all names unique; strict mutation-mode parser accepts the config |
| A0 row | exactly one `CCF-690-SEN1-SAVE-A0`, field-identical to #707's accepted edit. State `BLOCKED`, owner `EXCLUSIVE ASSIGNED SOURCE WRITER: Anthropic Claude Opus 5.5, T3 Code / Claude Code session fe9ea65b … SOURCE START HELD / AWAIT MANAGER #700 APPROVED TO START; NOT RUNNING`, lane `Sen1SaveA0ZX20NonUnity707`, branch `integration/sen-1-d073-shared-save-a0-20261117-v1`, HEAD `55863f1b…`, advisory expiry text and custodian jermur788 present |
| Comparison with #707 post-#710 snapshot (21:57Z) | board bytes identical, lanes JSON identical; 0 table rows and 0 lanes added, removed or changed. **No unknown third-party change** |
| #707 vs #710 delta | #707: one A0 row edit and one appended lane. #710: one non-table paragraph (`2026-11-17 PHYSICAL OPS — #710 …`, which the parser ignores) and one appended lane `MultiP1ModeRepair710`. Both are additive and disjoint; neither was altered by this readback |

The same hashes were re-read after the preflight at 22:11Z and were unchanged.

## 2. Source, worktree, ownership and deadline

- **Worktree** `/media/jer/ZX20/CCF-Sen1-690-A0-20261117`: HEAD `55863f1b7719011bba16b3f3ab377e379c0b988a`, **detached**, common dir `/media/jer/Files/CCF/Repository.git`. 0 tracked/untracked/ignored entries, no Library. Registered in the canonical worktree list as detached.
- **A0 branch** `integration/sen-1-d073-shared-save-a0-20261117-v1`: 0 refs and 0 checkouts in the canonical and legacy stores, 0 on GitHub (rechecked 22:11Z).
- **Pins:** canonical and GitHub agree on D-073 `55863f1b…` and donor `209a08e9f220841aaec81385d4cb7b437fa5089f`. The D-073 checkout `/media/jer/ZX20/CCF-S1-ResearchCopy-V1-Qual` and donor checkout `/media/jer/Files/CCF/Operations/CCF-Scen1-SharedSave-T01-20261115` are clean at their pins. Primary is unchanged (2 pre-existing user modifications, untouched).
- **One current writer:** only the named Claude Opus 5.5 session `fe9ea65b`, still NOT RUNNING. No other row or claim names the A0 lane, branch or path.
- **#710 scope:** its lane and board record cover the isolated store `/media/jer/Files/CCF/Operations/CCF-710-ModeRepair-20261117/git/CCF-710-ModeRepair.git`, branch `repair/multi-p1-shell-modes-710-20261117-v1` and a 4-file Multi P1 `.sh` mode-only repair. Its record explicitly lists the canonical/legacy stores, Scenario One and Unity lanes as protected read-only. No overlap with A0.
- **Unity lock:** free, no Editor.
- **Deadline:** checked at 22:05–22:11Z, before the 2026-11-18T00:00:00Z expiry. The user, jermur788, the named custodian, reconfirmed directly in this session at about 22:07Z: **"Yes, I'll supervise it."** They will release the row to BLOCKED / UNASSIGNED at or before 00:00Z unless a new supervised renewal is recorded under the physical lock. The expiry remains **ADVISORY / MANUALLY SUPERVISED — NOT ENFORCED BY TOOL**.

## 3. Unmodified mandatory preflight (read-only)

`lane-status --preflight --lane Sen1SaveA0ZX20NonUnity707 --branch integration/sen-1-d073-shared-save-a0-20261117-v1`, deployed and unmodified, 22:05:55Z–22:10:56Z:

```
Storage: READY / PASS | free=53086126080 bytes (49.44 GiB) | threshold=16106127360 bytes (15.00 GiB)
Sen1SaveA0ZX20NonUnity707 | /media/jer/ZX20/CCF-Sen1-690-A0-20261117 | device=/dev/sdd2 | ZX20: YES | unity=False exists=True registered=True
  branch=HEAD HEAD=55863f1b7719011bba16b3f3ab377e379c0b988a dirty=False ahead/behind=None Library=False disk~=2.0G owner=EXCLUSIVE ASSIGNED SOURCE WRITER: … (CCF-690-SEN1-SAVE-A0)
  warnings: board branch/HEAD differs: CCF-690-SEN1-SAVE-A0
BLOCKED: resolve assignment conflicts before dispatch
preflight exit=1
```

**Exit 1, not READY.** The only warning is the expected F-1 mismatch: the row names the A0 branch while the worktree stays detached until source is authorised. There are no new ownership, dirty, dual-store branch, duplicate-board, storage or registration warnings. The full 23-lane output is retained locally. #707 created no branch.

## 4. #710 lock-compliance confirmation

- **How it was obtained:** this worker messaged the local #710 session directly and asked for a GitHub confirmation. The #710 physical actor (Claude Opus 5.5, Claude Code in T3 Code, `jer-laptop`) posted it at 22:06:11Z: [#710 confirmation](https://github.com/jermur788/CCF/issues/710#issuecomment-6962349394). It answers [Manager #700's request](https://github.com/jermur788/CCF/issues/710) of 22:02Z and the [#707 note](https://github.com/jermur788/CCF/issues/710#issuecomment-6962197878).
- **What it states:**
  1. **No lock was taken** for the 21:52:34Z write. `lanes.json` was replaced via temp file plus `os.replace`, and `board/active.md` was appended. The actor acknowledges this broke #707's exclusive directory flock. The earlier #703 lane write at about 21:20Z was **also made without the lock**. Pre-write backups are in `/media/jer/Files/CCF/Operations/CCF-710-ModeRepair-20261117/receipts/`.
  2. **From now on**, every #710 and #703 board/lanes write first runs `flock -x -n /media/jer/ZX20/CCF-Ops/board <cmd>` and stops if the lock is held.
  3. **No overlap** with lane `Sen1SaveA0ZX20NonUnity707`, its path, branch `integration/sen-1-d073-shared-save-a0-20261117-v1`, or row `CCF-690-SEN1-SAVE-A0`, and it will not change them.
- **Not obtained:** Manager #694 has not separately confirmed. The packet accepts either the #710 actor or #694; #694 may still want to ratify the protocol for all Multi custodians.
- **Not contacted:** this worker did not touch #710's row or lane and did not speak for Manager #694.

## Result and next gate

| Packet item | Result |
| --- | --- |
| 1. Exclusive snapshot, validator, lanes, A0 row, third-party diff, lock release | PASS |
| 2. Source, worktree, single writer, #710 non-overlap, deadline and custodian | PASS (custodian reconfirmed; before expiry) |
| 3. Unmodified preflight | Exit 1, expected F-1 mismatch only; no new warnings (not READY) |
| 4. #710 lock-compliance confirmation | PASS (#710 actor, GitHub 22:06:11Z) |

Manager #700 may now evaluate a separate **[APPROVED TO START]**. It must stay inside the current advisory window, which ends **2026-11-18T00:00:00Z**, or be preceded by a supervised renewal under the physical lock. Required writer sequence:
1. Post `[RUNNING — A0 SOURCE]` on #690, refreshing pins, ownership and expiry.
2. Run `git -C /media/jer/ZX20/CCF-Sen1-690-A0-20261117 switch -c integration/sen-1-d073-shared-save-a0-20261117-v1 55863f1b7719011bba16b3f3ab377e379c0b988a`. This is the first source operation.
3. Run the unmodified preflight, which must return **READY / exit 0** before any merge. Otherwise STOP.

#707 stays OPEN. Evidence (locked snapshot copies, logs, full preflight output, readback script) is in `/media/jer/Files/CCF/Verification/Sen1-707-690-ZX20-Custody-20261117/lock-incident-recheck/`.
