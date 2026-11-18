# Multi #755 Phase 1 — H0-A local-only G1 physical lane setup: result (G1 SETUP PASS; EMPTY LANES HANDED BACK TO MANAGER)

**18 November 2026 · custodian: Anthropic Claude Sonnet 5.5 (`claude-sonnet-5-5`), Claude Code / Agent SDK through T3 Code, native host `jer-laptop` (same chat and session `7616b58c-c5de-48ab-9127-4deb32300921` as the #755 Phase 0 registration) · Manager: #730 · claim: [#755](https://github.com/jermur788/CCF/issues/755).**
**Authority:** [Manager Phase 1 grant document](https://github.com/jermur788/CCF/blob/docs/multi-730-755-phase1-grant-20261118-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-755-MANAGER-G1-PHASE0-VERDICT-READBACK-EXCEPTION-AND-ACTOR-SPECIFIC-PHASE1-GRANT-20261118.md) and [grant comment 6986127467](https://github.com/jermur788/CCF/issues/755#issuecomment-6986127467).
**Registered [RUNNING — #755 PHASE1]:** [comment 6986186296](https://github.com/jermur788/CCF/issues/755#issuecomment-6986186296), 2026-11-18T17:25:15Z, before any mutation. Pinned base: GitHub main `34185b8b71ba280c89091290345e537ef107bdf4`, identical to the canonical store's `refs/remotes/origin/main`.

## 0. Outcome

**G1 physical setup: PASS** against all four D1 substitute-evidence items and the grant's mutation ceiling. **No stop condition fired.** The transaction ran 17:27:58-17:28:32Z. Two empty Core/Viewer lanes now exist in an isolated, remote-less Git store on Files ext4, with two H0-only entries appended to live `lanes.json` and two H0-only rows appended to the live board. The installed `lane-status` is recorded as **EXPECTED BLOCKED / UNRESOLVED FOR THAT TOOL**, not suppressed. **I do not own or admit any writer**: the lanes are handed back to Manager #730, and no Core/A writer may start from this report.

| D1 item | Result |
| --- | --- |
| 1. Exclusive `flock -x -n` on the live `board` for the whole transaction; fresh pre-hashes; durable before-images | **PASS.** Lock held 17:27:58-end; `/proc/locks` shows it released afterwards. Hashes in §2. |
| 2. Two clean, distinct, isolated sparse worktrees; remote-less; push safeguards; read back from BOTH | **PASS.** 18/18 readback checks per worktree (§3). |
| 3. Two entries in `lanes.json`, two rows on the board; everything else byte-identical; validators; `deployment.json` unchanged | **PASS.** §4. |
| 4. Pre/post hashes, atomic temp+`fsync`+replace, final readback, custody receipt, no protected access, no unrelated change, no expiry | **PASS.** §2, §6, §7. |
| 5. Stop conditions | **None triggered.** |

## 1. Fresh physical pre-state (17:24-17:28Z, before mutation)

Board, `lanes.json` and `deployment.json` were unchanged since #753/#755 Phase 0 (both operational files last written 12:52:39Z); nothing in Ops was newer than 12:53Z; no lock on `board` or `state/unity/guard`; no Unity/.NET/Ops-tool process; installed `packet-lint --board` PASS (42 rows); ZX20 storage READY (47.99 GiB free against a 15.00 GiB floor); Files had 34-35 GB free; both target paths and any `*H0A*` directory were absent; no new Manager or owner instruction after the grant; GitHub main still `34185b8b`.

## 2. Hashes (SHA-256), before and after

| File | Before | After | Change |
| --- | --- | --- | --- |
| `ZX20/CCF-Ops/board/active.md` | `2b99552cc50026d8b322b393030f46b22b86424c20398240ff8f86d016f29842` | `4fa750eba64a8a072feb305a7db2cce17ddedfa62dbd276305116c03ab853994` | **+2 lines** (0 removed, 0 changed hunks); 42 -> 44 rows |
| `ZX20/CCF-Ops/lanes.json` | `c17903ddbf9157ce092cb091d350822f6b8574cfaad7ba6c79f1040350885331` | `c1d147196e1b8d57008e44545db3ba7f3fecea0ae12094dbf3e05f1c782b7d9e` | **+12 lines** (0 removed, 0 changed hunks); 23 -> 25 lane entries |
| `ZX20/CCF-Ops/deployment.json` | `8c66b290c247708846cb8b59f83626a10a3fa1d052397a3316dc5f23fc157b18` | **identical** | none |

The live bytes equal the validated candidate bytes exactly (candidate hashes equal the "after" hashes). Both files were written with temp file + `fsync` + `os.replace` + directory `fsync` and read back equal; both mtimes are 17:28:31Z.

## 3. Isolated store, worktrees and push containment (all on Files ext4; root `/` unused)

| Item | Value |
| --- | --- |
| Lane root `R` | `/media/jer/Files/CCF/Operations/CCF-H0A-Year0Viewer-20261118/` |
| Bare store | `R/git/CCF-H0A-Year0Viewer.git`; **no remote**, shallow (`shallow` = `34185b8b`), 965-967 MB, base ref `local/h0a-base` = `34185b8b71ba280c89091290345e537ef107bdf4` |
| Seed | one-shot `git fetch --no-tags --depth=1 file:///media/jer/Files/CCF/Repository.git refs/remotes/origin/main:refs/heads/local/h0a-base` (local path, no network), run **after** containment was applied; canonical store refs hash identical before and after (`466dee15f1d184d4...`) |
| Core worktree | `R/ccf-h0a-core`, branch `feature/multi-h0a-core-year0-admission-20261118-v1`, HEAD `34185b8b`, clean |
| Viewer worktree | `R/ccf-h0a-viewer`, branch `feature/multi-h0a-a-year0-viewer-20261118-v1`, HEAD `34185b8b`, clean |
| Hook | `R/hooks/pre-push`, mode `0555`, prints "push denied" and exits 1 |
| Artifacts | `R/artifacts/` created, **empty** |
| Sink path | `R/.no-push-sink` **does not exist** (never create it) |

**Read back from BOTH worktrees (18 PASS / 0 FAIL each, plus an independent re-check from outside the script):** correct branch and HEAD; clean status; no remotes; no upstream; `core.hooksPath` = `R/hooks`; blank `credential.helper`; `push.default=nothing`; exactly the five `url.file://R/.no-push-sink/.pushInsteadOf` rules for `https://github.com/`, `http://github.com/`, `git@github.com:`, `ssh://git@github.com/`, `git://github.com/`; no token/password/credential strings in the store config; `Assets/` absent; only the files below on disk; the hook blocks a push to a RAM-only local bare repo (which stayed ref-free); and explicit-URL pushes to GitHub (https, `git@`, `ssh://`) are rewritten to the non-existent local sink and fail locally (run with a dead proxy as an extra guard, and the errors never name GitHub).

**What is in each worktree:** only the three tracked root files from the base tree, `.gitignore`, `AGENTS.md` and `CLAUDE.md` (28 KB each). That is the rehearsed cone-sparse layout I described in the #755 Phase 0 report and in the RUNNING comment. There is **no** H0, viewer, fixture, application, test or build file and no `Assets/` or protected source. **Manager: if you read "EMPTY" as zero files, tell me and the custodian can switch to no-cone sparse; I made no such change without instruction.**

**Residual risk (unchanged from Phase 0 and from grant D6):** hooks and Git config can be bypassed (`--no-verify`, `-c`, editing config), and `gh api`, `curl` or SSH can still publish; the host has a `gh` token and a set `SSH_AUTH_SOCK`. Writer task packets must prohibit pushes, uploads and overrides, and reviewers must check GitHub for new viewer paths.

## 4. Validators (installed Ops code, run read-only after the replace)

| Validator | Result |
| --- | --- |
| `_ccf.config_read` on live `lanes.json` | OK; 25 lanes (the 23 originals compare equal, all non-lane keys such as `storage`, `repository`, `git_stores` equal) |
| `_ccf.board_read` on live board | OK; **44 rows**; the 42 originals compare equal |
| `_ccf.board_conflicts` | **none** (no duplicate lane or branch) |
| `packet-lint --board` | `PASS: board schema and duplicate checks (44 live tasks)`, exit 0 |
| `deployment.json` | byte-identical |
| Expiry text (D-091) | none: neither new row nor new lane entry carries a date or expiry; `purpose`/`Locks` say "NO EXPIRY (D-091): ownership continues until documented safe release" |

**New lane entries** (`unity:false`, keys `lane`, `path`, `purpose`, `unity` only): `MultiH0ACore` -> `R/ccf-h0a-core`, `MultiH0AViewer` -> `R/ccf-h0a-viewer`.
**New board rows** (appended after the last existing `|` line; the live table was already split into three blocks by earlier custodians, which `board_read` accepts): `CCF-755-H0A-CORE-LANE` and `CCF-755-H0A-VIEWER-LANE`, State `READY` meaning reserved, Owner text "RESERVED BY Manager #730 / NOT RUNNING / NO ACTIVE WRITER", `Unity required?` `No`, `HEAD` `34185b8b…`, Open gate "Manager #730 review ...; separate writer claim, actor-specific G2 grant, frozen read contract, D5 baseline; L4 FAIL38 UNWAIVED".

## 5. Installed `lane-status`: EXPECTED BLOCKED (recorded, not suppressed)

Run with a config holding only the two H0 lanes (so it does not walk the protected #710/#725 lanes), for each lane:

```
MultiH0ACore | .../ccf-h0a-core | device=/dev/sdb4 | ZX20: NO | unity=False exists=True registered=False
  warnings: worktree belongs to unexpected repository; not registered in configured repository
BLOCKED: resolve assignment conflicts before dispatch     (exit 1)
```

The Viewer lane returns the same result (exit 1). This matches the documented tool limitation (it requires a ZX20 lane that is a worktree of the canonical store). **It is unresolved for that tool**: I did not patch, weaken or work around it. The substitute evidence above is the grant's D1 replacement for this one H0 G1 case only. Full text: `validation/lane-status-EXPECTED-BLOCKED.txt`.

## 6. Unchanged and affected, and rollback

**Affected (only):** the new lane tree under `R`; the new evidence tree; `lanes.json` (+2 entries); `board/active.md` (+2 rows). Parent-directory mtimes also moved.
**Unchanged (verified):** the 42 original board rows and 23 original lane entries (byte-identical); `deployment.json`; `state/unity` (no lock, history untouched); the canonical store (refs hash equal; used read-only as the fetch source); lanes #674 (`5aa7b6ef`, still 8 dirty lines, advisory 18:00Z date left alone per D-091), #697 (`719ccbf8`), #690/#707 (`55863f1b`), #652 checkout (`4c7acc05`). The protected #710/#725 stores were **not opened or inspected**; no change to any other lane.
**Transient files:** the transaction wrote `lanes.json.755cand` (validation copy) and `*.755.tmp` (atomic-replace temps) inside the Ops directories per precedent; a post-run sweep found no leftovers.
**Rollback:** none was needed. If the Manager wants the setup undone: restore the before-images (hash-verified, same board lock) and remove exactly the paths in §7, only after a Manager release (D-091: no silent cleanup).

## 7. Path inventory (local only; nothing below was or may be pushed)

- `/media/jer/Files/CCF/Operations/CCF-H0A-Year0Viewer-20261118/` (`git/CCF-H0A-Year0Viewer.git` with 41 files, `hooks/pre-push`, `ccf-h0a-core/`, `ccf-h0a-viewer/`, `artifacts/` empty).
- `/media/jer/Files/CCF/Verification/CCF-H0A-Year0Viewer-20261118/custody/` (476 KB, 18 hashed files, `SHA256SUMS` verifies 18/18): `before/` (original `active.md`, `lanes.json`, `deployment.json`, `SHA256SUMS`, candidate board and lanes), `after/` (live copies plus sums), `validation/` (`readback-ccf-h0a-core.txt`, `readback-ccf-h0a-viewer.txt`, `packet-lint-board.txt`, `lane-status-EXPECTED-BLOCKED.txt`, `worktree-list.txt`, `lane-inventory.txt`, `canonical-refs-hash.txt`), `lane-status-record/lanes.json`, `transaction.log`, `RECEIPT.md`, and the task-local script **`txn755_phase1.py`** (sha256 `bd64ed851ae888eee18d98747273777fa4acecd2ee70d5e29771cb7f9ee85868`, 259 lines). The script is **left for independent audit, is not an installed Ops tool and is not published to GitHub.**
- RAM scratch (`/dev/shm`) held the grant copy, the draft and a hook-test bare repo; all removed.

## 8. Deviations, disclosures and caveats

- **Worktree contents:** three tracked root files rather than zero files (see §3).
- **Script defects fixed before the run:** a dry-run in RAM found that the live board's `|` lines are not one contiguous block (my first assertion would have aborted after the local directories were created) and the `ls-files` check would have counted skip-worktree entries; both were corrected, re-dry-run, and only then was the transaction executed. No operative file was touched before that.
- **The installed tooling was used read-only** (`config_read`, `board_read`, `board_conflicts`, `packet-lint`, `lane-status` on a two-lane config); I wrote no Ops tool or installed script.
- **Not done:** any viewer source, fixture import, package assembly, application test or build, browser, Unity, Windows, Host, `dotnet`, `node`; any read of #710/#725; any GitHub write other than the #755/#730 comments, this branch and this one file; main edit; public push; G2 start; L4 waiver (**L4 FAIL38 UNWAIVED**).

## 9. Custody, safe state and what the Manager must still do

- **Custody:** the lanes are **RESERVED by Manager #730, NOT RUNNING, no writer assigned, no expiry (D-091).** My temporary custodian role ends with this handoff and I retain no write claim to them.
- **Safe state:** the stores and worktrees are clean; the board lock is released; Ops files validated; nothing is queued or running.
- **Still open before any source:** separate named Core and Viewer writer claims with actor-specific G2 non-release grants and fresh readback; the frozen Core-to-Viewer contract; **D5** (named protected-path baseline *outside* the sparse tree, with a lawful read grant) because empty protected paths inside the sparse tree prove nothing; a Manager-granted read of the canonical store for the Core writer's byte-verified fixture copy (blob `8bf81a43…`, 32,567 B, D3); reviewer selection; G3/G4 and L4 FAIL38 remain blocked; reconciliation of the stale #710/#725/#732/#674/#652 coordination records by their authorised owners (D7); canonical Multi decision ID/crosswalk.
- **Notes for writer packets:** the lanes are not registered in `git_stores`, so Ops performs no dual-store branch-conflict check for these branch names; the worktrees' `.git` files hold absolute paths (do not move the lane root); the store is shallow (no history beyond `34185b8b`).

**NO VIEWER SOURCE / NO FIXTURE IMPORT / NO APPLICATION TESTS OR BUILDS / NO UNITY / NO WINDOWS / NO HOST / NO PROTECTED-SOURCE ACCESS / NO PUBLIC PUSH / NO MAIN INTEGRATION / NO G2 / NO L4 WAIVER.** Dashboard/Drive sync: **NOT VERIFIED.** Issue #755 stays OPEN for Manager #730 review of this physical receipt.
