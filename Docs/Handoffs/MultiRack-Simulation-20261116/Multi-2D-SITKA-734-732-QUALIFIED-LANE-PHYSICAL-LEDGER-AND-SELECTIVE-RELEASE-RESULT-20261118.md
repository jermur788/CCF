# Multi 2D Sitka — #734 physical ledger reconciliation and selective release of the qualified #732 lane

**Result: PASS.** The #732 board row is reconciled. The disposable #732 qualification lane is released using an exact allowlist. All source, Git stores and evidence that had to be preserved are preserved and verified. The network-stop cache files are kept physically as byte-identical archived copies.

**Date:** 2026-11-18 UTC. **Claim:** [#734](https://github.com/jermur788/CCF/issues/734). **Packet:** Manager #730 `dea7d9021a0b896356dc0162de241ddb8f4fa7ec`, on branch `docs/multi-730-732-pass-734-custody-20261118-v1`. **Result branch:** `ops/multi-734-732-qual-custody-release-20261118-v1`, from main `5f395c37e153db9a3868f694a352db6cc025225a`. The only change on it is this file.

## 1. Actor

| Field | Value |
|---|---|
| Model | Anthropic Claude Opus 5.5 (`claude-opus-5-5`) |
| Harness | Claude Code in T3 Code, native Linux |
| Session | `dd9143ff-82d8-4eb7-a74d-8b413009cda2` (new) |
| Host | `jer-laptop`, Linux 7.0.0-42-generic x86_64, uid 1000 |
| Authority | Owner jermur788 dispatched this session directly as physical custodian only |
| Registration | [#734 comment 6978756366](https://github.com/jermur788/CCF/issues/734#issuecomment-6978756366), 12:37Z, before any physical write |
| Excluded sessions — none reused | #725 author `01a1ed7e…`; original #732 custodian `c33954eb…`; original #732 worker and amend-1 custodian `76502b90…`; #732 resumed worker `32188791…` |

This session was not a qualification, test or source worker. It ran no dotnet, builds, tests, Unity or Windows. It made no network calls from the lane and no source or code edits. It did not merge to main and did not waive L4. **Default L4 FAIL38 remains UNWAIVED.**

## 2. Lock and transaction

- The board was re-read live. The prehash was not reused: CCF-652 had changed since #732. The values pinned at preflight were:
  - board `d98c31560c7dc19047bb78bd0995045bd4a242adac491b3dab42bd9359fad272` (42 rows);
  - `lanes.json` `b16bfcad2fbb0be09c8d44fa0acdc87841d458e784c8befee2668a3650eacd61` (24 lanes, 2 git_stores).
- The dry run and the real run each held their own nonblocking exclusive `flock -x -n /media/jer/ZX20/CCF-Ops/board` for the whole run. The real transaction started at 12:41:19Z. Each run covered pinned-hash verification, target-row identity, physical re-checks, candidate lint, deletion, the atomic replace and the byte readback. The lock was free both times.
- Script: `txn734.py` (sha `858d3765…`). Log: `txn.log` (sha `93b02068…`). The dry run passed first, logged in `txn-dry.log`.
- **Candidate lint:**
  - `_ccf.board_read` returned 42 rows, the same count as before.
  - `_ccf.board_conflicts` returned `[]` both before and after.
  - `_ccf.config_read` returned OK. It was run on a temporary `lanes.json.734cand` placed beside the live config inside the lock, then removed.
  - lanes went from 24 to 23.
  - The only row that differed was `CCF-732-MULTI-YEAR1-NATIVE-QUAL`.
- **Atomic replace:** each file was written to a `.734.tmp`, fsynced and moved into place with `os.replace`. The byte readback matched both candidates. Post-write values:
  - board `ae80e1908956a228ac77cc29858585361a2ecaffa70dd0337992bdb733d43cda`, 42 rows, 0 conflicts;
  - `lanes.json` `51850d741cd05e7125f093b8357c6bb74ee2ed9c424fcdad087ad2344125e9c4`, 23 lanes, git_stores unchanged.
  - A later independent `cmp` found the live files equal to `active.md.after-734.md` and `lanes.json.after-734.json`. No `.tmp` or candidate files were left in CCF-Ops.

## 3. Row and lane before and after

**Board row `CCF-732-MULTI-YEAR1-NATIVE-QUAL`:**
- **Before:** `WAITING`, "LEASE ISSUED, NOT RUNNING", lane `MultiYear1Qual732`, model "TBD at RUNNING". The full row is in `active.md.before-734.md`, line 119.
- **After:** `DONE-PENDING-LEDGER`. This is the closest value in the supported `_ccf.STATES` vocabulary to "qualified, awaiting integration and ledger". The parser schema was not changed. The other columns now read:

| Column | After |
|---|---|
| Owner | `NONE` (qualification complete, Manager #730 APPROVED, #732 closed; lease released 2026-11-18T12:41:21Z by #734 custodian jermur788 through this session) |
| Model | `claude-opus-5-5` |
| Lane | `None` |
| Branch | `qualify/multi-732-year1-native-20261118-v1` (result `88e82cd0`) |
| HEAD | `df467dcc302eca564343e6da40f2fb88529a5d16` |
| Subsystem | "FOCUSED INDEPENDENT NATIVE-QUALIFIED" |
| Locks | A "Released: …" statement that names the archive and evidence locations and the #725 and #710 retention dates |
| Open gate | Not merged or integrated, not Unity-verified, not release-qualified, default L4 FAIL38 UNWAIVED, #729 M-1 fix required before Year-2 |
| Next action | Manager #730 inspects this result; any integration needs a separate proposal |

One dated `PHYSICAL OPS — #734` paragraph was appended to the board, at line 137.

**`lanes.json`:** the entry `MultiYear1Qual732` (path `…/CCF-732-Year1Qual-20261118/ccf-732-qual`) was removed. This follows the #720 precedent ("lane MultiL4Pilot720 removed"). No other lane or store was touched.

**Rows and lanes not changed:**
- The #725 row `CCF-725-MULTI-F0-ANNUAL`, DONE-PENDING-LEDGER, with exclusive source custody retained.
- The #710 row `CCF-710-MULTI-P1-R-CUSTODY`, BLOCKED, with custody retained.
- CCF-652 and every other row.
- Lanes `MultiAnnualF0Child725` and `MultiP1ModeRepair710`.

## 4. Proof before release (`guard734.sh pre` → `guard-pre/`)

**Source and stores:**
- **Qualified candidate is still available.** The #725 bare store `CCF-725-F0Annual.git` has `df467dcc` as a `commit`. The #725 clean worktree `ccf-725-src` is at `df467dcc`, tree `4237ece5`, branch `feature/multi-p1-f0-annual-725-20261118-v1`, with an empty porcelain status.
- **The qual store held nothing unique.**
  - Its refs were a subset of the #725 store refs: `df467dcc` / `719ccbf8` / `35d26f5e`.
  - All **20,211** objects reachable from those refs are present in the #725 store. The `cat-file --batch-check` check found 0 missing.
  - There was no `objects/info/alternates` file and no hard-linked files (count 0). The local config is bare only, with no remote.
- **R is preserved.** `#710` `CCF-710-ModeRepair.git` `refs/heads/repair/multi-p1-shell-modes-710-20261117-v1` = `35d26f5e64a00a400c700871927ccc2173370568`. The `ccf-710-src` worktree is clean at tree `33f84db2`.

**Qualification lane:**
- The worktree was detached at `df467dcc`, tree `4237ece5`.
- It had no tracked or untracked changes. The only ignored paths were `.p0ref/` and the `bin/` and `obj/` folders of Cli, Core and Core.Tests.
- No user files were found.

**Processes and links:**
- No processes had a cwd, exe, root or fd inside the lane.
- No `dotnet` or `VBCSCompiler` processes were running (`pgrep -x`).
- The lane contained 0 symlinks.

**Disjointness:**
- Only the `MultiYear1Qual732` lane entry referenced the lane root, and no git_store did.
- Board references were only the #732 row and its own history paragraph.
- The #725 lane root (inode 9874089) and the #710 lane root (inode 9874117) are siblings of the #732 lane root (inode 9874159), not ancestors or descendants. All three are on the same Files ext4 device (2068).

**Primary:** the two user-dirty files matched `custody/pre/primary.sha256` (exit 0).

## 5. Evidence preservation (all verified before and after release; all exit 0)

| Set | Files | Manifest | SHA256SUMS sha |
|---|---|---|---|
| `worker/` (original #732 worker, BLOCKED evidence incl. `stop/`) | 48 + manifest = 49 | 48 lines, `sha256sum -c` OK | `c9bad9c12f6ec8b975355cc16da7656130343fa95452a4bc11ce80ff405d67a1` |
| `worker-resume/` (offline resume PASS) | **90 checksummed + the manifest itself = 91** | 90 lines, OK | `50e7ba42e60f9b4245e8854e69bdc123e07d4f078573c27ffae586be496c4d1e` |
| `custody/` top level (GRANT.md, pre/post snapshots, candidates, txn) | 31 checksummed | 31 lines, OK | `9cbf0060b9d4ccbd9639fedb68c717b728cc4f876e55ea9eef393481e5d8c5c3` |
| `custody/amend-1/` | 29 + manifest | 29 lines, OK | `eec37483fd29579c245c4215a285b1db7867dc7f5777455f589b34368a28a694` |

Notes on the evidence sets:
- Two `custody/` files are not listed in any manifest: `amend-1.comment.url` (written after the 11:50 manifest) and `amend-1/SHA256SUMS`. Both are retained.
- The #725 evidence and receipts passed `sha256sum -c` against `custody/pre/e725-*.sha256`. The #710 evidence and receipts passed against `custody/pre/r710-*.sha256`. Both checks were run before and after release.
- `guard-post/` shows the #725 HEAD and status, refs and worktree index hash, and the #710 HEAD and refs, all **byte-identical** to `guard-pre/`.
- The published GitHub result `88e82cd0cc3e60dcc777720332200a88231d4b8e` is the live tip of `qualify/multi-732-year1-native-20261118-v1`, and GitHub #732 is closed `[APPROVED / COMPLETED]`.
- Pinned values still recorded in evidence: DLL `9a99f781…` (`amend-1/p0ref.sha256`), golden `574d8746…` (`worker-resume/`), and the network-cache hashes (`worker/stop/http-cache.sha256`, `amend-1/sdk-advertising.sha256`).

The evidence directory `/media/jer/Files/CCF/Verification/CCF-732-Year1Qual-20261118/` was **not** deleted or altered. The only change there was the new `custody/release/` folder.

## 6. Network-stop cache files: retention assessment

**Question:** do the 5 NuGet HTTP-cache files and 66 SDK advertising files have to stay physically as network-stop evidence?

**Assessment:**
- The STOP itself is already established by three things:
  - Manager #730's verdict ("STOP was real and complied with");
  - the hashes in `worker/stop/http-cache.sha256` and `amend-1/sdk-advertising.sha256`;
  - the timestamped listings in `worker/stop/lane-cache-listing.txt` and `amend-1/lane-cache-listing.txt`.
- The raw bytes are still the only direct proof of what was downloaded: the `api.nuget.org` service index, the vulnerability data and the workload manifests. Hashes alone cannot be re-inspected.
- The #725 author lane's own identical copies (from 07:33Z) belong to #725 and fall under its separate expiry, so they cannot be relied on beyond 20 Nov.

**Custodian decision: RETAIN PHYSICALLY, but not in the disposable lane.**
- Before deletion, the whole lane `home/` (79 files, including the 5 http-cache files, the 66 sdk-advertising files, the sentinels and the NuGet migration and config files) was copied byte-for-byte with `cp -a`, which keeps mtimes. It went to `custody/release/archive/lane-home/`. The total is about 1.08 MB.
- Two other items were copied the same way:
  - the `.p0ref/` folder (DLL `9a99f781`, `SitkaLab.Core.dll.sha256`, provenance) to `archive/p0ref/`;
  - `offline/NuGet.Config` (`fa1d75e1`) to `archive/offline/`.
- Verification:
  - the 5 http-cache files passed `sha256sum -c` against `worker/stop/http-cache.sha256`;
  - the 66 sdk-advertising files passed against `amend-1/sdk-advertising.sha256`;
  - the `.p0ref` and offline config passed against the amend-1 hashes;
  - `diff -r` showed the copies identical to the lane originals.
- `archive.SHA256SUMS` (83 files, sha `99269c48…`) was re-checked inside the lock immediately before deletion.
- The lane copies were deleted only after the archive was proven.

## 7. Deletions (exact allowlist) and retentions

Before deletion, the full lane manifest was saved as `lane-pre-manifest.txt`: 6,924 entries, sha `36ac3a2c…`, with type, mode, size and mtime for each path. About 2.25 GB were freed (`du -sb` total 2,246,172,539 B).

| Path under `/media/jer/Files/CCF/Operations/CCF-732-Year1Qual-20261118/` | Action | Why safe |
|---|---|---|
| `ccf-732-qual/` (detached worktree, about 1.23 GB, including ignored bin/obj and `.p0ref`) | `rmtree` | Clean at `df467dcc`, which lives in the #725 store and worktree. `.p0ref` archived. |
| `git/CCF-732-Year1Qual.git/` (bare clone, about 1.02 GB) | `rmtree` | All 20,211 reachable objects are in the #725 store. No alternates or hard links. |
| `home/` | `rmtree` | Archived byte-identical, including the network-stop caches. |
| `tmp/` (MSBuild and NuGet scratch, plus the tamper copy) | `rmtree` | Build scratch. The tamper refusal result is recorded in `worker-resume/`. |
| `nuget/`, `artifacts/`, `receipts/` | `rmtree` | Empty. |
| `offline/` (`NuGet.Config`) | `chmod u+w` dir, then `rmtree` | Archived byte-identical. |
| `git/`, then the lane root | `rmdir` (non-recursive) | Empty after the above, so a non-recursive `rmdir` would have failed if anything unexpected was left. |

Every allowlisted path was checked inside the lock before deletion: it was a real directory, not a symlink, had `realpath == path`, was on device 2068 and was inside the lane root. The top-level entries had to equal exactly `{artifacts, ccf-732-qual, git, home, nuget, offline, receipts, tmp}`.

**Not deleted, and never in scope:**
- the #725 lane, store, worktree, evidence and receipts;
- the #710 lane, store, worktree, evidence and receipts;
- `Repository.git` and Primary, including its user-dirty files;
- Scenario One and Unity, the frozen fixture, and the P0 and CCF pins;
- all #732 evidence.

No `git clean`, stash, reset or broad cleanup was used.

## 8. Remaining leases and deadlines

| Item | State | Deadline | Owner |
|---|---|---|---|
| #725 original author source, store, evidence (79 files) and receipts (16 files) | Retained, unchanged; row DONE-PENDING-LEDGER | **2026-11-20T18:00:00Z**; custodian-managed, no automatic purge | Not #734's to release |
| #710 repaired R `35d26f5e`, store, evidence and receipts | Retained, unchanged; row BLOCKED, custody retained | **2026-11-23T18:00:00Z**; release only after Manager #694 R disposition | Not #734's to release |
| #732 qualification lane `MultiYear1Qual732` | **RELEASED 2026-11-18T12:41:21Z**; lane entry removed | n/a | — |
| #732 evidence, including `custody/release/` | Retained permanently in Files CCF | n/a | — |

## 9. Receipts

All receipts are in `/media/jer/Files/CCF/Verification/CCF-732-Year1Qual-20261118/custody/release/`:
- **Scripts:** `guard734.sh` and `txn734.py`.
- **Logs:** `txn-dry.log` and `txn.log`.
- **Guard snapshots:** `guard-pre/` and `guard-post/`.
- **Board and lanes copies:** `active.md.{before,candidate,after}-734.md` and `lanes.json.{before,candidate,after}-734.json`.
- **Lane and archive:** `lane-pre-manifest.txt`, `archive/` and `archive.SHA256SUMS`.
- **This result:** a copy of this file as `RESULT-734.md`.
- **Release manifest:** `SHA256SUMS`, 148 entries, sha `780f29e2a8fe465c14664d0433c3ade927591b61efead8bf400485ba175be8e4`, written before `RESULT-734.md` was added.

**Trap noted:** a `pgrep -af '[V]BCSCompiler'` run from inside a `bash -c` whose own command line contains the word "VBCSCompiler" matches the invoking shell. `guard-pre/procs-dotnet.txt` therefore lists the guard's own parent shell (pid 1278659). That line is a false positive: the `pgrep -x` results were empty both times.

## 10. Status (unchanged by this custody action)

- #725: **LOCALLY IMPLEMENTED**.
- #729: **INDEPENDENTLY SOURCE/NUMERIC REVIEWED**.
- #732: **FOCUSED INDEPENDENT NATIVE-QUALIFIED**.
- None of these is merged, integrated, Unity-verified, broadly release-qualified, L4-safe, product-accepted or authorised for Windows.
- Default L4 FAIL38 remains **UNWAIVED**.
- The #729 M-1 dead-tree fix is required before any Year-2 work.
- A successful cleanup is not integration. Manager #730 should inspect this result, give a verdict on #734, and treat any integration as a separate proposal.
