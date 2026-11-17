# Multi #712 — Ops shared-lock reconciliation and selective lease cleanup: result

**[PHYSICAL #712 LOCKED RECONCILIATION COMPLETE] — PASS. Phase 2 cleanup: NONE (not authorised).** Date 2026-11-17. Claim https://github.com/jermur788/CCF/issues/712. Packet: [#712 complete physical lock reconciliation packet](https://github.com/jermur788/CCF/blob/docs/multi-694-710-mode-repair-verdict-20261117-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-712-COMPLETE-PHYSICAL-LOCK-RECONCILIATION-AND-SELECTIVE-CLEANUP-PACKET-20261117.md) (branch tip `eef3702b`).

## Actor and authority
- **Actor:** Anthropic Claude Opus 5.5 (`claude-opus-5-5`), Claude Code in T3 Code, Linux host `jer-laptop`, OS user `jer`, GitHub `jermur788`.
- **Authority:** the physical owner/custodian, user jermur788, was asked directly in-session on 2026-11-17 at about 22:14Z. They authorised **Phase 1 only**: a locked, read-only reconciliation plus this result. They declined to authorise Phase 2 cleanup at this time. The GitHub claim was not treated as authority.
- **Independence disclosure:** the actor responsible for the #703/#710 incident was also Claude Opus 5.5, in a different session. The custodian was told this before giving authority. This reconciliation verifies the files against receipts and against #707's independent readback, so it does not rely on that actor's own account.

## Lock protocol used
The same exclusive, nonblocking directory flock held by #707 was used: `flock -x -n /media/jer/ZX20/CCF-Ops/board <cmd>` on directory inode `1399769` (exFAT `/dev/sdd2`, UUID `18D6-896E`). One flock call held the lock for the whole capture: stat, hash, copy, parse and re-hash.

| Event | UTC |
|---|---|
| Lock 1 acquired (capture) | 2026-11-17T22:15:05.062898619Z |
| Lock 1 released | 2026-11-17T22:15:06.191411606Z |
| Lock 2 acquired (closing readback) | 2026-11-17T22:16:47.854665934Z |
| Lock 2 released | 2026-11-17T22:16:47.860721315Z |
| Independent release probe | FREE at 2026-11-17T22:16:47Z |

**Board/lanes mutations: NONE.** All analysis ran on the copies taken under the lock.

## Deployed board/lanes, before and after
| File | sha256 (lock 1 pre = lock 1 post = lock 2) | size | mtime |
|---|---|---|---|
| `board/active.md` | `ef2d97720b0939dd59f9ada25fcc82a97794a59eed474856ab5bef6148a273f6` | 84753 | 2026-11-17 21:52:34.12Z |
| `lanes.json` | `1b1cc0054cb7a968abc87dafd9e1044b30fdbfc529de33b4057dc42b34861bc3` | 9108 | 2026-11-17 21:52:34.12Z |

These hashes are byte-identical to the [#707 22:05Z lock-incident readback](https://github.com/jermur788/CCF/issues/707#issuecomment-6962484697). No third party has written to either file since the #710 write at 21:52:34Z.

## Reconciliation against receipts
Receipts used:
- #703: `active.md.before-703.md` `a41cb50e…7c7679`, `lanes.json.before-703.json` `e6cf0742…e3a3`.
- #710: `active.md.before-710.md` `5384b1b6…dc95d`, `lanes.json.before-710.json` `ed79cd5a…d498`.

All four are in each lane's `receipts/` folder.

**lanes.json** (schema 1, keys `board, git_stores, lanes, repository, schema, storage`):
- before-703 → before-710: added `MultiTier2Qual703` and `Sen1SaveA0ZX20NonUnity707`. None removed or changed. Order preserved. Non-lane keys identical.
- before-710 → live: added `MultiP1ModeRepair710` only. None removed or changed. before-710 is an exact prefix of live. Non-lane keys identical.
- Live: 23 lanes, no duplicate names, no duplicate paths. JSON parses.

**board/active.md:**
- before-703 → before-710 has two changes. #707 edited the `CCF-690-SEN1-SAVE-A0` row under its own lock at 21:50:21Z. The #703 lease paragraph was appended at about 21:20Z.
- before-710 → live: only the #710 lease paragraph was appended (lines 117–118).
- The A0 row is byte-identical between before-710 and live. It matches #707's reported edit.
- The official Ops parser `_ccf.board_read` on the locked copy: **OK, 37 rows × 17 fields**. `_ccf.board_conflicts` reports **none**. This matches #707's "37×17 lint PASS".

**Lost or overwritten records: none found.** The #710 read happened after #707's 21:50:21Z board write: the before-710 receipt keeps that mtime. Live content is exactly before-710 plus the #710 additions. For #703, the before-703 receipt keeps the board mtime 20:37:05Z, and every row present then is still present.

**Limitation:** a write landing in the seconds between an actor's receipt copy and its own write would not show up in the receipts. #707's independent hash reproduction found no evidence that this happened.

**Observation (not a defect in another owner's rows):** #703 and #710 are recorded only as prose lease paragraphs plus `lanes.json` entries. Neither has a board table row, so `lane-status` row-based conflict checks cannot see them. This is left for Manager #694 to decide. It was not repaired here.

## Protected state (unchanged before/after)
Filesystems:
- Files: ext4 `/dev/sdb4`, UUID `2283b135-bf37-42d6-921f-2fb861d0a8d4`.
- ZX20: exFAT `/dev/sdd2`, UUID `18D6-896E`.

**#710 R store** `git/CCF-710-ModeRepair.git`:
- Refs: `repair/multi-p1-shell-modes-710-20261117-v1` = `35d26f5e64a00a400c700871927ccc2173370568` and `integrate/multi-p1-h2-f-child-b-20261117-v1` = `719ccbf87f0644a1a3fb9487ce7b991f4b867424`.
- R is a commit whose only parent is C `719ccbf8…`.
- R's diff contains only 4 mode changes, 100644→100755:
  - `build/build-p0-core-ref.sh`
  - `tests/build-output-safety.sh`
  - `tests/cli-safety.sh`
  - `tests/p0-isolation.sh`

**#710 source worktree** `ccf-710-src`: on the R branch, HEAD R, clean (0 status lines).

**Disposable clones:** inspected only, not touched.
- `ccf-710-test`: HEAD R, clean.
- `ccf-703-qual`, `ccf-703-p0`, `ccf-703-diag`: HEAD C, clean.
- None is a symlink.
- Sizes: #703 root 6.4 G, #710 root 4.3 G.

**Processes:** no process has its cwd under either lane root.

**Evidence:**
- `/media/jer/Files/CCF/Verification/CCF-710-ModeRepair-20261117/SHA256SUMS.txt`: 41/41 OK.
- `/media/jer/Files/CCF/Verification/CCF-703-Tier2-Qual-20261117/SHA256SUMS.txt`: 127/127 OK.
- A 177-file manifest of both evidence trees, both receipts folders and the R store refs/HEAD is identical before and after (`protected-manifest-before.sha256` = `protected-manifest-after.sha256`).

**Primary dirty files**, unchanged and not staged:
- `AGENTS.md` `1bf4286b…97fd`
- `Docs/Operations/StorageArchitecture.md` `6bebe495…5a22`

**Origin main** at check: `dd3b9032e42f00d819697855f18c0a5b93dbc81e`.

## Cross-Manager #700 non-overlap (Scenario One #707/#690 A0)
- Board row `CCF-690-SEN1-SAVE-A0` and lane `Sen1SaveA0ZX20NonUnity707` were read only. Both are identical to #707's accepted state.
- Worktree `/media/jer/ZX20/CCF-Sen1-690-A0-20261117` is detached at `55863f1b7719011bba16b3f3ab377e379c0b988a`, clean.
- Branch `integration/sen-1-d073-shared-save-a0-20261117-v1` is absent from the canonical store.
- The owner, custodian and the 2026-11-18T00:00Z advisory cutoff were **not changed or released**.
- No Multi lane path overlaps the A0 path.

## Phase 2 cleanup
**None performed. Cleanup list: none.** The custodian did not authorise selective cleanup. Both lanes stay in place:
- `MultiTier2Qual703`
- `MultiP1ModeRepair710`

Both keep their 2026-11-19T18:00Z expiry. Outstanding action, if later authorised by the custodian under the same flock:
- #703: delete `ccf-703-qual`, `ccf-703-p0`, `ccf-703-diag` and `tmp/` only.
- #710: delete `ccf-710-test` and `tmp/` only.
- Then make the exact, minimal lane/lease updates.
- Never touch the #710 store, R branch, `ccf-710-src`, receipts or evidence.
- Lanes must not be auto-purged after expiry without proof that R has been preserved.

## Verdict and limitations
- **PASS:** the lock was obtained and released cleanly. Board and lanes reconcile exactly with the #703/#710 receipts and the #707 readback. No lost or overwritten records were found, and protected state is unchanged.
- The incident itself remains a genuine protocol breach. This result does not ratify the #710 actor's pledge; that is Manager #694's decision.
- Not run: no source, test, Unity, Windows or native tests. Security L4 FAIL38 and the first-ecological-year HOLD are unchanged.
- Not ratified here: this result grants no deletion or wider source authority. Manager #694 accepts or rejects it separately.

## Evidence
`/media/jer/Files/CCF/Verification/CCF-712-OpsLockRecon-20261117/` contains:
- `locked-capture/` (board/lanes copies plus the flock transcript)
- `flock-closing-readback.txt`
- `lanes-compare-output.txt` and `board-compare-output.txt`, with their scripts
- `official-board-lint-output.txt`
- `protected-state-before.txt`
- `protected-manifest-{before,after}.sha256`
- `evidence-checksum-verify.txt`
- `SHA256SUMS.txt`
