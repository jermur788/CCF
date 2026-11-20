# #893 — Seven exact Files worktrees: read-only local safety preflight

**Result status:** **PREFLIGHT COMPLETE — 7 of 7 PREFLIGHT-PASS / OWNER-DELETE-DECISION-PENDING.**
**PRECHECK ONLY — ZERO DELETED — ZERO FREED — NOT A DELETION APPROVAL.**
**Date/time:** 2026-11-20, 22:37–22:50 UTC.
**Worker:** DeepSeek V4.1 Flash (`opencode-go/deepseek-v4.1-flash`) via local T3 Code / OpenCode on `jer-laptop`. One read-only researcher; no Unity, no native Git writer, no #802 remover role claimed.
**Authority:** owner approval 2026-11-20 22:34:19 UTC, exact seven paths only. Issue #893.
**Companion:** `CCF-893-EXACT-SEVEN-PATH-PREFLIGHT-RESULTS.tsv` (7 rows).

> A PASS is an observation at one instant. It is **not** safe-to-delete, not an owner release, and not permission to run `git worktree remove`.

---

## 1. Host and mount proof

| Check | Observed |
|---|---|
| Hostname | `jer-laptop` |
| User | `jer` |
| Files mount | `/dev/sdb4` → `/media/jer/Files`, ext4, `rw,relatime` |
| Files UUID | `2283b135-bf37-42d6-921f-2fb861d0a8d4` — **matches the packet** |
| Capacity at start / end | 498,654,666,752 total; 450,298,675,200 used; **22,950,465,536 free (96 %)** at 22:37Z → 22,950,322,176 free at 22:52Z (Δ 143,360 B, not from this inspection) |
| Realpath / symlink | All seven `realpath` results equal the literal path; `islink = False` |
| Nested mount | None: every path resolves to device `sdb4`, Files UUID above |
| Shared Git dir | `/media/jer/Files/CCF/Repository.git` for all seven; `.git` is a **file**, not an independent clone |
| Worktree registration | All seven present exactly once in `git worktree list --porcelain` |

The seven paths are real directories on the intended Files filesystem, not symlinks, not a separate mount, and not independent clones.

---

## 2. Live ownership refresh

| Source | Result for the seven paths |
|---|---|
| `active-tasks.md` on `main` (`e2226a25`, 1,086 lines) | **0 mentions** of any of the seven worktree names |
| Local CCF-Ops board `board/active.md` | **0 mentions** |
| Unity lease history | No lock held. Last events: `Codex-886` release 20:35:36Z, `Claude-891` release 22:34:10Z. **None** for these paths |
| Unity guard file | Empty (`0` bytes), consistent with no held lock |
| Running processes | No readable process has a `cwd`, open FD or mmap inside any of the seven roots |
| T3 local state | Only historical session logs (13 Nov and 18 Nov) mention these names; the live SQLite WAL hit is current-session text. No live session targets them |
| GitHub issue search | Originating issues are closed: #260, #255, #256, #269/#275/#277, MFM-08 #195/#205, #213 |
| #802 text | **0** references to these paths; they are Operations task worktrees, not Ops `unity-lock` copies or Ops source checkouts |

**Process/FD limitation:** 281 `/proc` FD entries were unreadable (root/kernel-owned). The scan covered every readable process as user `jer`; no hit was found. This is recorded as a bounded limitation, not as a proof that no process anywhere could ever touch the paths.

---

## 3. Per-path results

All seven were observed at the exact inventory HEAD. Allocated bytes reproduced **byte-for-byte** (total **8,624,160,768 B**). Working-tree state was identical across all seven: **0 staged, 0 unstaged, 0 untracked, 0 ignored**.

| ID | Path | HEAD / branch | Allocated | State |
|---|---|---|---:|---|
| `CCF-INV-0128` | `…/CCF-Overnight-FeedbackVerify-20261104` | `6efba8fd…` `verify/overnight-playtest-feedback-20261104` | 1,232,261,120 | PASS |
| `CCF-INV-0129` | `…/CCF-Overnight-ObjectiveCopy-20261104` | `693e24c8…` `feature/overnight-objective-copy-20261104` | 1,232,629,760 | PASS |
| `CCF-INV-0131` | `…/CCF-Overnight-RackHandle-20261104` | `9304d318…` `feature/overnight-rack-handle-20261104` | 1,232,502,784 | PASS |
| `CCF-INV-0132` | `…/CCF-Overnight-UIQualification-20261104` | `d3ba4fce…` `integration/scenario-one-overnight-ui-qualification-v1` | 1,233,047,552 | PASS (variance noted) |
| `CCF-INV-0102` | `…/CCF-MFM08Closure` | `911a8a34…` `verify/mfm-08-tier2-closure-v1` | 1,230,462,976 | PASS |
| `CCF-INV-0103` | `…/CCF-MFM08Qual` | `d88a32f6…` `qual/mfm-08-unity-v1` | 1,231,458,304 | PASS |
| `CCF-INV-0104` | `…/CCF-MFM10Residual-20261101` | `43e26370…` `verify/mfm-10-species-guard-v1` | 1,231,798,272 | PASS |

### Ignored-file evidence

`GIT_OPTIONAL_LOCKS=0 git status --porcelain=v1 --untracked-files=all --ignored=matching` returned **zero lines** for every path. A separate bounded `find -xdev` scan found **no** `Library/`, `Build/`, `Temp/`, `obj/`, `UserSettings/`, save/prefs file, `.bak`, `.zip`, `.tar`, `.7z`, `.log` or `.env`. Each worktree contains ~5,400 files of ordinary Unity source (`Assets/`, `Packages/`, `ProjectSettings/`, `Docs/`, `Tools/`) totalling ~1.216 GB of file bytes; the largest entries are project FBX art (26.5 MB max). No personal save or licensed binary outside the tracked project content was found.

### Reconstructibility

For all seven, the exact HEAD object is present on GitHub, the origin branch exists, and `git rev-list --count <HEAD> --not --remotes=origin` is **0**. Worktree HEAD reflogs contain only ancestors of HEAD — no reflog-only or orphaned commit.

| ID | Remote state | Reconstruction (future, not executed) |
|---|---|---|
| 0128 | origin branch = `6efba8fd` | `git worktree add <newpath> 6efba8fdc35f22098faf205a5f7d14ccf65a3e1a` |
| 0129 | origin branch = `693e24c8` | `git worktree add <newpath> 693e24c8891637470052031d5023883b922e9334` |
| 0131 | origin branch = `9304d318` | `git worktree add <newpath> 9304d318f346b281a19cb805feebc6dc1a95008e` |
| **0132** | origin branch **advanced to `cddf3ed6` (+6)**; HEAD is an ancestor | `git worktree add <newpath> d3ba4fce3994d5c59b51786c6671dd654791e7d1` — **by SHA, not branch** |
| 0102 | origin branch = `911a8a34` | `git worktree add <newpath> 911a8a34ffbaa2004697ffeabdfe21edcb82eb2d` |
| 0103 | origin branch = `d88a32f6` | `git worktree add <newpath> d88a32f6b4ce8d79b71c7f0350ddc9344bdf2727` |
| 0104 | origin branch = `43e26370` | `git worktree add <newpath> 43e263708d8f517337181c92d0d78ed442803950` |

**0132 variance (recorded, not a hold):** the branch received six later commits after this checkout was last used. The local HEAD is a clean ancestor of the remote tip and no local work is unpushed. The exact old tree is recoverable by SHA; reconstructing by branch name would produce different content. This is the only deviation from the inventory snapshot across the seven.

---

## 4. Classification

| Class | Count | Paths |
|---|---:|---|
| PREFLIGHT-PASS / OWNER-DELETE-DECISION-PENDING | **7** | 0128, 0129, 0131, 0132, 0102, 0103, 0104 |
| HOLD | 0 | — |
| UNKNOWN | 0 | — |

Why PASS for all seven: exact path, mount and UUID verified; exact HEAD and branch reproduced; registration confirmed; no tracked, untracked or ignored changes; no live process, lease or claim; originating tasks closed; exact commit recoverable from GitHub; no unique local data found.

Why that is still not safety: the check is instantaneous, ignored-file proof depends on `.gitignore` completeness, root-owned process state is partly unreadable, and no owner has released the paths.

---

## 5. Owner decision queue (exact paths only)

Ranked by evidence strength; all seven are equal on current evidence, so the ranking is operational.

| Rank | Exact path | Bytes | Notes |
|---|---|---:|---|
| 1 | `/media/jer/Files/CCF/Operations/CCF-MFM08Closure` | 1,230,462,976 | Oldest task stream; zero variance |
| 2 | `/media/jer/Files/CCF/Operations/CCF-MFM08Qual` | 1,231,458,304 | Zero variance |
| 3 | `/media/jer/Files/CCF/Operations/CCF-MFM10Residual-20261101` | 1,231,798,272 | Zero variance |
| 4 | `/media/jer/Files/CCF/Operations/CCF-Overnight-FeedbackVerify-20261104` | 1,232,261,120 | Zero variance |
| 5 | `/media/jer/Files/CCF/Operations/CCF-Overnight-ObjectiveCopy-20261104` | 1,232,629,760 | Zero variance |
| 6 | `/media/jer/Files/CCF/Operations/CCF-Overnight-RackHandle-20261104` | 1,232,502,784 | Zero variance |
| 7 | `/media/jer/Files/CCF/Operations/CCF-Overnight-UIQualification-20261104` | 1,233,047,552 | Branch tip +6; reconstruct by SHA |

**Total if and only if later approved: 8,624,160,768 B (8.62 GB decimal).** This is a possible recovery figure, not freed space. Files free space was 22,950,465,536 B at 22:37Z and 22,950,322,176 B at 22:52Z — a 143,360 B difference attributable to concurrent filesystem activity. This inspection wrote nothing to Files.

**Next gate:** the owner approves or rejects **each exact path**. Only then may #425 route one native writer to run a fresh final preflight and, for approved paths only, non-forced `git worktree remove <exact path>`. That execution is a separate packet; it is not granted here.

---

## 6. Limitations and unknowns

1. Ignored-file proof is `.gitignore`-dependent; an unlisted generated file pattern would not appear.
2. 281 `/proc` FD entries were unreadable (root/kernel), so process absence is proven only for readable processes.
3. No Git fetch was performed; remote state is the local `origin/*` ref plus live GitHub API reads.
4. Reflog inspection covered HEAD reflogs; repository-wide stashes were not re-audited and none is known to belong to these paths.
5. File contents were not hashed; cleanliness relies on Git index/status and the tracked tree.
6. Access times may have changed due to reads. No literal zero-write claim is made.

---

## 7. Non-mutation attestation

**Permitted operations used:** `hostname`, `findmnt`, `lsblk`, `df`, `du -x`, `stat`, `find -xdev`, `/proc` symlink reads, bounded `grep` of T3 logs, GitHub API reads, and read-only Git (`rev-parse`, `symbolic-ref`, `status`, `worktree list`, `reflog`, `rev-list`, `merge-base`, `for-each-ref`) with **`GIT_OPTIONAL_LOCKS=0`**.

**Not performed:** no `worktree add/remove`, `rm`, `mv`, archive, compression, `clean`, `prune`, `gc`, `repack`, `fetch`, `push`, checkout, reset, stash, branch/tag/ref change, permission change, mount/remount, Unity Editor or Hub operation, Unity lock acquire/release, source/scene/asset/save/Prefs write, merge or Windows build. No file was written inside any audited root. No shared Git metadata was modified.

**Result:** zero deleted, zero moved, zero archived, zero bytes freed, zero Unity or source changes.

---

*Handoff: `HANDED OFF / READY FOR OWNER DECISION`. The read-only inspection role is released. Any deletion remains a separate, exact-path, owner-approved execution phase.*
