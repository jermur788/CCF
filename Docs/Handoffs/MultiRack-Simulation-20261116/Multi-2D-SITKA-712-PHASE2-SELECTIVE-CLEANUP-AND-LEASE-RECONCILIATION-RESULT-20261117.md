# Multi #712 Phase 2 — selective cleanup and lease reconciliation: result

**[PHYSICAL #712 PHASE 2 COMPLETE] — PASS.** Date 2026-11-17. Claim https://github.com/jermur788/CCF/issues/712.

Inputs:
- Manager #694's Phase 1 approval and conditional Phase 2 scope: https://github.com/jermur788/CCF/issues/712#issuecomment-6962704470
- The RUNNING notice for this phase: https://github.com/jermur788/CCF/issues/712#issuecomment-6962777968
- The Phase 1 result: [Phase 1 result `3eaa061e`](https://github.com/jermur788/CCF/blob/3eaa061e7c080fc47761769697e792251987bc31/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-712-OPS-LOCK-RECONCILIATION-AND-SELECTIVE-LEASE-CLEANUP-RESULT-20261117.md)

## Custodian and actor
- **Physical custodian:** user jermur788. At about 22:23Z they directly authorised, in the session, the exact plan that was later posted in the RUNNING notice. No custody was inferred from the Manager comment.
- **Actor:** Anthropic Claude Opus 5.5 (`claude-opus-5-5`), Claude Code in T3 Code, `jer-laptop`, OS user `jer`.
- **Disclosure:** the actor responsible for the #703/#710 incident was also Claude Opus 5.5, in a different session.

## Live preflight under the lock (before deleting anything)
- `flock -x -n /media/jer/ZX20/CCF-Ops/board` was held 22:24:40.990–22:24:40.994Z. Board `ef2d9772…` and lanes `1b1cc005…` were unchanged since Phase 1.
- No process had a cwd or open file under either lane root.
- Every target:
  - resolved to itself and was not a symlink;
  - was on Files ext4 (`/dev/sdb4`, dev 2068) and owned by `jer`;
  - had no foreign-owned files and no mounts below it.
- Clones were clean, with 0 stashes. They have no remotes. Every ref tip in them is present in the source store: the #697 store for the #703 clones, the #710 store for `ccf-710-test`.
- The 8 symlinks inside `tmp/` all point within `tmp/`.
- Evidence files: `preflight-lock.txt`, `preflight-paths.txt`, `preflight-source-preservation.txt`.

## Deleted paths (exactly these, nothing else)
`rm -rf --one-file-system` ran on each allowlisted path after a per-path identity re-check, 22:25:10–22:25:12Z (`deletion-log.txt`).

| Path | Size before |
|---|---|
| `/media/jer/Files/CCF/Operations/CCF-703-Tier2-Qual-20261117/ccf-703-qual` | 2.2G |
| `/media/jer/Files/CCF/Operations/CCF-703-Tier2-Qual-20261117/ccf-703-p0` | 2.2G |
| `/media/jer/Files/CCF/Operations/CCF-703-Tier2-Qual-20261117/ccf-703-diag` | 2.2G |
| `/media/jer/Files/CCF/Operations/CCF-703-Tier2-Qual-20261117/tmp` | 66M |
| `/media/jer/Files/CCF/Operations/CCF-710-ModeRepair-20261117/ccf-710-test` | 2.2G |
| `/media/jer/Files/CCF/Operations/CCF-710-ModeRepair-20261117/tmp` | 156K |

**What remains:**
- The #703 root is now 3.2M: `out/` and `receipts/`. `out/` was not in the allowlist, so it was kept.
- The #710 root is now 2.2G: `git/`, `ccf-710-src` and `receipts/`.
- Files free space went from 36G to 45G.

## Board/lanes transaction (one flock, read-modify-write-readback)
Script: `board_lanes_txn.py`. Transcript: `txn-transcript.txt`.
- The lock was held 22:26:02.157–22:26:02.292Z. An independent probe found it free at 22:26:02Z.
- The script refused to write unless the live hashes matched the preflight hashes, and they did.
- Backups were written first: `active.md.before-712p2.md` and `lanes.json.before-712p2.json`.
- The candidate was linted before writing. Each file was written to a temporary file in the same directory, fsynced, and then swapped in with `os.replace`. Both files were then read back byte-for-byte.

| File | Before | After |
|---|---|---|
| `board/active.md` | `ef2d97720b0939dd59f9ada25fcc82a97794a59eed474856ab5bef6148a273f6` | `7b062b2853fd06bbeca73d5dcd4ace841ce62b50339cddebc9f6528f8a67c792` |
| `lanes.json` | `1b1cc0054cb7a968abc87dafd9e1044b30fdbfc529de33b4057dc42b34861bc3` | `75df505feacfab8a6e06f331b2c4a7ef57d16deb0ed898b6f844b2a926ff3627` |

**lanes.json** (23 → 22 lanes):
- Removed `MultiTier2Qual703`. Its path `ccf-703-qual` no longer exists.
- Changed the `MultiP1ModeRepair710` purpose text only. Its path stays `ccf-710-src`. The new text describes retained R source custody with no active writer: custodian jermur788, store/branch/R/worktree/evidence preserved, test/tmp removed, expiry 2026-11-19T18:00Z custodian-managed, no automatic purge without proof that R is preserved, release only after Manager #694 decides what happens to R.
- No other lane and no non-lane key changed.

**board/active.md** (official `_ccf.board_read`: 37 → **38 rows × 17 fields**; `board_conflicts` reports **none**):
- Inserted one structured row right after the A0 row:
  `| CCF-710-MULTI-P1-R-CUSTODY | BLOCKED | CUSTODY RETAINED: user jermur788 (physical custodian); NO ACTIVE WRITER; #710 writer Claude Opus 5.5 finished; NOT RUNNING | None | None | None | MultiP1ModeRepair710 | repair/multi-p1-shell-modes-710-20261117-v1 | 35d26f5e64a00a400c700871927ccc2173370568 | Multi P1 Tools local four-mode repair R (source preservation only) | … expiry 2026-11-19T18:00:00Z custodian-managed; no automatic purge without R preservation proof | No | None | Tier 1 Git source proof | Manager #694 R disposition (integrate or transfer); default L4 FAIL38 open | #710 Manager approval of local mode correction; #712 Phase 2 | Manager #694 decides R route; custodian releases this row and lane only after R is preserved elsewhere or expressly abandoned |`
- Appended one prose paragraph, "2026-11-17 PHYSICAL OPS — #712 PHASE 2 …". It records:
  - the #703 release;
  - the #710 disposal plus retained custody;
  - that the earlier breach is not ratified;
  - the evidence path.
- Every old line is preserved in order. The `CCF-690-SEN1-SAVE-A0` row is byte-identical.

This gives #710 R custody a structured board row, so `board_conflicts` can now enforce it. The two prose notes from before remain. No reservation was invented: the row records the existing #710 custody. #703 has no row because its lease is released.

## R source and evidence preservation (after cleanup)
**R store:**
- Refs unchanged: R branch = `35d26f5e64a00a400c700871927ccc2173370568`, `integrate/multi-p1-h2-f-child-b-20261117-v1` = `719ccbf8…`.
- R's only parent is C `719ccbf8…`.
- `git fsck --no-dangling` rc 0.
- Worktrees: the bare store plus `ccf-710-src` only.

**Source worktree:** `ccf-710-src` HEAD R, on the R branch, 0 status lines.

**Evidence:**
- `/media/jer/Files/CCF/Verification/CCF-710-ModeRepair-20261117/SHA256SUMS.txt`: 41/41 OK.
- `/media/jer/Files/CCF/Verification/CCF-703-Tier2-Qual-20261117/SHA256SUMS.txt`: 127/127 OK.
- The 177-file protected manifest (both evidence trees, both receipts folders, R store refs/HEAD) is identical to the Phase 1 `protected-manifest-before.sha256`.

**Other protected state:**
- #697 store present, with its branch still at `719ccbf8…`.
- Primary dirty files unchanged: `AGENTS.md` `1bf4286b…` and `StorageArchitecture.md` `6bebe495…`.
- Scenario One A0 worktree: detached at `55863f1b`, clean. Its branch is absent, and its row and lane are unchanged.
- Origin main at the post-check: `0c553b6d2d3ff337d769ea0dd3bf9c358c6bd856`.

## Owners notified
Manager #694 and Overall Manager #700, through #712 and short pointer comments. #700 is told for information only, since nothing it owns changed.

## Verdict and limitations
**PASS.**
- Only the six allowlisted disposable paths were deleted.
- The #703 reservation was released.
- #710 R custody is retained and now tracked as a structured row.
- Board/lanes were changed in one flock transaction with readback.
- Protected state is intact.

The earlier unflocked writes remain a protocol breach and are not ratified here.

Not done or unchanged:
- No source, test, Unity, Windows or native run.
- No push or main merge.
- Default L4 FAIL38 and the first-ecological-year HOLD remain open.
- No `lane-status --preflight` was run, because no lane was started.

Manager #694 may now accept this result and close #712.

## Evidence
`/media/jer/Files/CCF/Verification/CCF-712-OpsLockRecon-20261117/phase2/` contains:
- `preflight-*.txt`
- `deletion-log.txt`
- `board_lanes_txn.py` and `txn-transcript.txt`
- `active.md.before-712p2.md`, `lanes.json.before-712p2.json` and `active.md.candidate.md`
- `post-cleanup-protected.txt`
- `protected-manifest-after-phase2.sha256`
- `SHA256SUMS.txt`
