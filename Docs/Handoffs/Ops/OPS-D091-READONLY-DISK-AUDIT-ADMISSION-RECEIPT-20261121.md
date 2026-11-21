# Ops — D-091 read-only disk-audit admission receipt

**ADMITTED — READ-ONLY.** One audit session may inventory and measure CCF worktrees and storage and propose a ranked deletion list. **Zero deletion authority.** No inventory was performed by the custodian; issuing this receipt is the whole action.

Issued **2026-11-21T23:48Z** (observed clock `2026-11-21T23:48:16+00:00`).

Canonical URL: https://github.com/jermur788/CCF/blob/docs/ops-20261121-disk-audit-readonly-admission-v1/Docs/Handoffs/Ops/OPS-D091-READONLY-DISK-AUDIT-ADMISSION-RECEIPT-20261121.md

## 1. Issuer and authority chain

| Item | Value |
|---|---|
| Issuer | existing D-091 physical custodian — opencode-go / DeepSeek V4.1 Flash, T3 session `a8d35599-48af-4f1b-a652-26f5ff44da5e` |
| Host | `DESKTOP-DL1BQI7`, Linux 6.18.58.1-microsoft-standard-WSL2, `jer` uid 1000 |
| Packet | [`OPS-OWNER-WORKTREE-DISK-RECLAMATION-AUDIT-NO-DELETION-20261121.md`](https://github.com/jermur788/CCF/blob/docs/ops-20261121-worktree-disk-audit-owner-prune-v1/Docs/Handoffs/Ops/OPS-OWNER-WORKTREE-DISK-RECLAMATION-AUDIT-NO-DELETION-20261121.md) — inventory and recommendation only, zero deletion |
| Authority basis | The packet names this custodian as provider **"or another explicit owner-appointed local Ops inventory agent under custodian's read-only admission"**. The owner has appointed the audit worker named below; this receipt is the custodian's read-only admission under that clause. Owner instruction in this session authorises issuing it |
| Custodian role status | Unrevoked. Prior custodian records `b77f48fb`, `efb188ac`, `32a5b3e9` stand |

## 2. Audit worker identity

| Field | Value |
|---|---|
| Thread | `e908af52-016d-489c-8682-d5e4a7cc1fb2` — "Worktree Disk Reclamation Audit" |
| Provider / model | `codex_3` / **gpt-6.1-terra** |
| Host / directory | `DESKTOP-DL1BQI7`, project checkout `/home/jer/Development/CCF-HP` (thread reports `worktreePath: null`, branch `main`) |
| Status at issue | `running`, created by `user` at `23:40:42Z`, last updated `23:47:23Z`, parent thread null |
| **How the id was obtained** | The message did **not** carry a readable `senderThreadId` field. The id above was matched from the live T3 thread registry by title, provider, model, project and running state. **Nothing was invented**; if the intended sender is a different thread, this grant binds to the named identity and the worker must state its own thread id in its result |
| Scope | read-only inventory, measurements and a ranked proposal only |

## 3. Live verification at issue (measured by me)

| Check | Result |
|---|---|
| `/` extent | `/dev/sdd` **ext4**, 1007 G total, 78 G used, **879 G available** (WSL2 virtual disk) |
| Windows host disk | `C:` via 9p at `/mnt/c`, 238 G total, 202 G used, **37 G free (85 %)** — the actual pressure, and **not** directly relieved by deleting Linux files |
| `/media/jer/ZX20` | **ABSENT / not mounted** — any ZX20 target is `UNKNOWN`, not "empty" |
| CCF roots | `/home/jer/Development`, `/home/jer/Development/CCF-HP`, `/home/jer/Development/CCF-HP-Worktrees` exist |
| Primary checkout | `/home/jer/Development/CCF-HP`, branch `main`, `cbf3a618`, `git status` 0 lines; git common dir `.git` (same volume) |
| Registered worktrees | **29** |
| Live CCF processes | #877 PID **1750123** alive (`/home/jer/Development/CCF-Review-877/wt-owner/…`), #882 PID **1806361** alive (`…/CCF-Multi-882-Launch/…`); no other CCF-cwd process |
| Loopback listeners | only `127.0.0.1:33767` (#877) and `127.0.0.1:43649` (#882) |
| #867 | PID 390578 absent (owner's incident; not free to reuse) |
| Demo lane | `CCF-Multi-885-Demo-Playable-20261121` and `CCF-885-Demo-Evidence-20261121` **absent at issue**; if either appears later it is an admitted live lane |
| Board | live `active-tasks.md` refreshed; no audit or demo lane registered there yet |

## 4. Permitted locations (read-only)

`/home/jer/Development/**` (CCF checkouts, registered worktrees, any unregistered CCF copies), `/home/jer/Development/CCF-HP-Worktrees/**`, `/home/jer/Development/CCF-HP/.git` metadata, `/home/jer/Development/CCF-Review-877/**`, `/mnt/c/**` **only** for capacity/accounting facts (no scan of unrelated Windows user data), `/tmp` state that belongs to CCF runs, and process metadata under `/proc`.

Explicitly **out of scope**: `/media/jer/ZX20` (unmounted — report `UNKNOWN`, do not infer), any path outside CCF/dev roots, personal files, credentials, `~/.ssh`, browser profiles, unrelated repositories.

## 5. Permitted actions (read-only)

`date`, `hostname`, `df -hT`, `lsblk -f`, `mount`, `stat`, `du -sx -B1`, `find` bounded to CCF roots with metadata output only, `git worktree list --porcelain`, `git rev-parse`, `git log`, `git status --porcelain=v1 --untracked-files=all`, `git reflog` read, `git ls-remote`/`gh` **read-only** to check whether commits exist on a durable remote, `ss -ltnp`, `/proc/<pid>/cwd|cwd` and `cmdline` reads, bounded `lsof` if installed, `wsl.exe --list --verbose` for capacity facts only.

**One writable location**, outside every worktree: `/home/jer/Development/CCF-Ops-DiskAudit-20261121/` for local logs and raw command output. Everything else is read-only. Local logs are supplementary; the GitHub result document is the deliverable.

## 6. Prohibited (any occurrence is an immediate STOP)

No `rm`, `rmdir`, `trash`, `unlink`, `shred`, `truncate`, `dd`, `mv` of lane content, no `git gc`/`prune`/`worktree prune`/`clean`/`checkout`/`reset`/`commit`/`stash`/`fetch --prune`, no `dotnet build`/`clean`, no test runs, no Unity, no Windows build, no `chmod`/`chown`, no process kill or service restart (including #877/#882/#867), no cache or evidence deletion, no VHDX compaction, `wsl --shutdown`, or Windows disk-management command, no package install, no writes into any worktree, save, fixture, golden, `.p0ref` or source file, no publication of tokens or capability secrets, no ownership transfer or release of any lane, and **no performing of any proposed deletion**.

## 7. Unresolved locks — classify C/HOLD, never propose

| Lane | Status at issue |
|---|---|
| `CCF-Multi-885-Integration-20261121` @ `4cecf873` | preserved integration lane — **C** |
| `CCF-Multi-885-MA1` @ `ac4c7b51` | author lane — **C** |
| `CCF-Multi-885-Review-20261121` @ `58950a9a` | reviewer lane — **C** |
| #877 PID 1750123 / `:33767` | live owner review — **C**, process is protected |
| #882 PID 1806361 / `:43649` | live private playtest — **C**, process is protected |
| #867 (PID 390578 absent) | owner's incident; absence of a process is **not** a release — **HOLD** |
| Primary checkout `CCF-HP` @ `cbf3a618` | **C** |
| #885 demo lane (if created) | **C** once present |
| Any directory whose owner cannot be verified | **HOLD**, never A |

## 8. Deliverable

One new Markdown on branch **`docs/ops-20261121-worktree-disk-reclamation-result-v1`** at
`Docs/Handoffs/Ops/OPS-WORKTREE-DISK-RECLAMATION-READONLY-INVENTORY-RESULT-20261121.md`,
containing the mount/usage table, every registered and orphan CCF worktree with path/size/branch/SHA/recency/dirty state/owner/process/class, the ranked A/B/C proposal with conservative reclaimable GiB, the owner-release checklist, the two-pass future plan shown as **commands not executed**, and an explicit `UNKNOWN` list (ZX20 unmounted, C: vs WSL accounting). Read back the pushed blob and report its exact URL and sha256.

## 9. Non-claims

No deletion, pruning, cleanup, process termination, build, Git change, VHDX compaction or ownership transfer is authorised or performed. No lane is released. No owner acceptance, no Stage I verdict, no Q1 authorisation, no claim that any directory is disposable. Owner approval of an exact path list remains required before any removal.
