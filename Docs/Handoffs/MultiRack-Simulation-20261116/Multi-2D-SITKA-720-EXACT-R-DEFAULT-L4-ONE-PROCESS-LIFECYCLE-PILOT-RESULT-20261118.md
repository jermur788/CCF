# Multi #720 — Exact-R default .NET L4: one-process fd lifecycle pilot (result)

**Proposed outcome: PILOT PROVEN (one-process pilot only), with residual gaps listed below. Default L4 stays FAIL38.** Date 2026-11-18. Claim [#720](https://github.com/jermur788/CCF/issues/720), Manager [#694](https://github.com/jermur788/CCF/issues/694). Worker: Anthropic Claude Opus 5.5 (`claude-opus-5-5`), Claude Code in T3 Code, host `jer-laptop`. Manager #694 decides APPROVED / NEEDS CHANGES / REJECTED for the pilot evidence only.

In one default-runtime `verify-derived` process on exact R, the kernel trace links both historical mutator kinds from creation to teardown:

- **memfd.** `memfd_create("doublemapper", MFD_CLOEXEC)` returns fd 8. The `ftruncate(8, 2 TiB)` follows 4.1 ms later. The fd is then used only for 152 `MAP_SHARED` mappings by two threads of the same fd table. It is never duplicated, passed or explicitly closed. It ends with the process.
- **Socket.** `socket(AF_UNIX, SOCK_STREAM|SOCK_CLOEXEC)` returns fd 9 (inode 4658923). Then `fchmod(9, 0600)` → `bind` to the full path `/media/jer/Files/CCF/T720/dotnet-diagnostic-960791-14199553-socket` → `listen(255)`. The path is unlinked at `coreclr_shutdown_2`, and the process exits.
- **Stacks.** Every creator and mutator stack is in `libcoreclr.so` 8.0.33 under `coreclr_initialize`.
- **No protected impact.** No protected path appears anywhere in the trace. No accept was observed during this test.

This covers only this one process. It does not retroactively settle the 38 historical events.

## Authorisation and lane

| Item | Record |
|---|---|
| Owner grant + start approval | Owner/custodian jermur788 granted the start directly in session, in place of a separate Manager permit (#703/#710 precedent). The Manager keeps the verdict. [Grant comment](https://github.com/jermur788/CCF/issues/720#issuecomment-6965292800) · [RUNNING](https://github.com/jermur788/CCF/issues/720#issuecomment-6965311805) |
| Board | Row `CCF-720-MULTI-P1-L4-LIFECYCLE-PILOT` (ACTIVE) and lane `MultiL4Pilot720`. Written under `flock -x -n /media/jer/ZX20/CCF-Ops/board` (inode 1399769). First write used `RUNNING`. The parser rejects that, so it was corrected to `ACTIVE` under the same lock protocol. Post-write parse: 40 rows, conflicts `[]`. Pre-write copies in `/media/jer/Files/CCF/Operations/CCF-720-L4Pilot-20261118/receipts/`. No other row or lane changed. |
| Lane paths | Clone `…/CCF-720-L4Pilot-20261118/ccf-720-pilot` (now removed). Build caches `home/`, `nuget/`, `btmp/` (now removed). TMPDIR `/media/jer/Files/CCF/T720`: mode 0700, ext4 `/dev/sdb4`, realpath unchanged, empty before and after; now removed. Evidence `/media/jer/Files/CCF/Verification/CCF-720-L4LifecyclePilot-20261118/` is retained. |
| Environment | Linux 7.0.0-42-generic; strace 6.8 (`6.8-0ubuntu2`, stack-trace=libunwind); SDK 8.0.133 (P1 `global.json`); runtime Microsoft.NETCore.App 8.0.33. The `dotnet` muxer loads hostfxr **10.0.14** (the highest installed), which rolls `net8.0` forward to 8.0.33. That is default host behaviour and is recorded, not changed. `/` was 100 % full, so root `/tmp` was not used. |

## Candidate, build and integrity

- **Clone.** `git clone --no-hardlinks` of `/media/jer/Files/CCF/Operations/CCF-710-ModeRepair-20261117/git/CCF-710-ModeRepair.git`, detached at R `35d26f5e64a00a400c700871927ccc2173370568`, origin removed. Tree `33f84db2…`, P0 subtree `f212c8f6…`, P1 subtree `fd0861ad…`. 0 object files had extra hard links.
- **Build.** One pinned build inside the clone: `build/build-p0-core-ref.sh`, then `dotnet build src/Cli/SitkaP1.Cli.csproj -nodeReuse:false -p:UseSharedCompilation=false`, with `HOME`/`DOTNET_CLI_HOME`/`NUGET_PACKAGES`/`TMPDIR` inside the lane. 0 warnings, 0 errors.
  - The P0 core reference build started a lane-owned `VBCSCompiler` (its env showed the lane HOME/TMPDIR). `dotnet build-server shutdown` did not stop it, so I sent it SIGTERM before the trace. No other process was touched.
- **Binary hashes:**
  - `sitkap1.dll` `43179186ab29ea0479b35f938310bec108a500c5625e376bc82186a6392b8d28`, same before and after the run
  - `SitkaP1.Core.dll` `3e0a67f1…327b`
  - `SitkaLab.Core.dll` `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3`, matching the pin in `SitkaP1.Core.csproj`
  - `libcoreclr.so` `60e1c3d4a68c72eafb91c154285ea9d62d98edb8a4311b3af7004b6b6240a79c`
  - `dotnet` `5ac7511c…cb3d`
- **Pre/post guards** (`guard-pre.txt` = `guard-post.txt`, SHA-256 `d96e84a1…e269`):
  - #710 store refs unchanged, and `ccf-710-src` clean at R
  - clone tracked status clean
  - **P0 52** tracked files, content digest `9e218ca7…30e8`, the same value as #703
  - **P1 27** tracked blob identities unchanged
  - the 4 CCF pins (SimulationRandom `bb83d7f8`, SitkaGrowthModel `e4c73402`, SitkaSpruce.asset `e638e28e`, TreeSpeciesDefinition `51faaa0d`) match in both tree and worktree
  - fixture `bcaaa5c3d9b3d36ad8319ffcc361a6225a02b90281cef8435de7c52f8ff757c4` unchanged
- **CLI result:** rc 0, stderr empty, `trust=TRUSTED trees=100 belowActionThreshold=12 contentHash=b7dee8a3…6627 derivedCatalogDigest=143a253a…a6f7`. This matches #703.

## Exact executed command (run once)

```bash
LIST=execve,execveat,clone,clone3,fork,vfork,exit,exit_group,open,openat,openat2,creat,close,close_range,dup,dup2,dup3,fcntl,unshare,memfd_create,ftruncate,mmap,mprotect,munmap,socket,socketpair,bind,listen,accept,accept4,connect,getsockname,getpeername,getsockopt,fchmod,recvmsg,recvmmsg,sendmsg,sendmmsg,pidfd_getfd,unlink,unlinkat
cd /media/jer/Files/CCF/Operations/CCF-720-L4Pilot-20261118
timeout -k 10 600 env -i PATH=/usr/bin:/bin HOME="$OP/home" TMPDIR=/media/jer/Files/CCF/T720 LANG=C.UTF-8 DOTNET_CLI_TELEMETRY_OPTOUT=1 \
  env -u DOTNET_EnableWriteXorExecute -u DOTNET_EnableDiagnostics \
  strace -f -ttt -yy -k -s 256 -o "$EV/run/strace.txt" -e "trace=$LIST" \
  /bin/sh -c 'env | LC_ALL=C sort > "$1"; ls -l /proc/$$/fd > "$2"; exec /usr/lib/dotnet/dotnet "$3" verify-derived p1d-30m-sitka-adult-v1 "$4"' \
  sh "$EV/run/launch-env.txt" "$EV/run/initial-fds.txt" "$DLL" "$FIX" < /dev/null > "$EV/run/stdout.txt" 2> "$EV/run/stderr.txt"
```

- **Syscall list.** This is the packet's full list, unchanged. Before the run, strace accepted it and `-k` on `/bin/true`.
- **Launch environment.** `launch-env.txt` holds exactly `DOTNET_CLI_TELEMETRY_OPTOUT, HOME, LANG, PATH, PWD, TMPDIR`. Neither `DOTNET_EnableWriteXorExecute` nor `DOTNET_EnableDiagnostics` is present, so both runtime defaults applied.
- **fds at exec.** The `/bin/sh` wrapper only records the environment and fd table, then `exec`s dotnet in the same PID 960791. At the dotnet `execve` (strace.txt line 2855) the fds were 0 `/dev/null`, 1 `stdout.txt`, 2 `stderr.txt`. The dash save-fd 10 was closed at line 2850. Wall time was 25.5 s. 13 TIDs ran: the leader 960791 (sh, then dotnet), 3 short-lived wrapper children (`env`, `sort`, `ls`) and 9 runtime threads. All exited 0.

## Event table (raw `strace.txt` line numbers, leader PID/TID 960791)

| Line | Time (s) | Event | Note |
|---|---|---|---|
| 2855 | …466.046859 | `execve(/usr/lib/dotnet/dotnet …)` | Start of the default runtime. fds 0–2 only. |
| 8696 | …467.522032 | `close(8</sys/fs/cgroup/…/cpu.max>)` | Earlier occupant of number 8, closed first |
| **8721** | …467.527589 | **`memfd_create("doublemapper", MFD_CLOEXEC) = 8</memfd:doublemapper>(deleted)`** | **Creator** |
| **8741** | …467.531730 | **`ftruncate(8</memfd:doublemapper>(deleted), 2199023255552) = 0`** | **Mutator 1**: same TID, same fd, 4.1 ms later, nothing between that touches fd 8 |
| 8844 / 8892 | …467.5575 / .5687 | `close(9</dev/urandom>)`, `close(9</proc/960791/stat>)` | Earlier occupants of number 9 |
| **8918** | …467.574567 | **`socket(AF_UNIX, SOCK_STREAM\|SOCK_CLOEXEC, 0) = 9<UNIX-STREAM:[4658923]>`** | **Creator** |
| **8940** | …467.579608 | **`fchmod(9<UNIX-STREAM:[4658923]>, 0600) = 0`** | **Mutator 2**: same inode |
| 8962 | …467.584525 | `bind(9<…[4658923]>, sun_path="/media/jer/Files/CCF/T720/dotnet-diagnostic-960791-14199553-socket") = 0` | Full 66-byte path, not abbreviated |
| 8984 | …467.589647 | `listen(9<UNIX-STREAM:[4658923,"…-socket"]>, 255) = 0` | Kernel annotation ties the bound path to the same inode |
| 54294 | …490.748679 | `unlink("…/dotnet-diagnostic-960791-14199553-socket") = 0` | Teardown, stack in `coreclr_shutdown_2` |
| 54435 / 54446 | …490.856150 / .860200 | `exit_group(0)`, `+++ exited with 0 +++` | Last thread of the shared fd table exits, and the kernel releases fd 8 and fd 9 |

The full stacks for these lines are in `run/mutator-stacks.txt`.

- **memfd and socket stacks.** `memfd_create`/`ftruncate` and `socket`/`fchmod`/`bind`/`listen` are all `libcoreclr.so(GetCLRRuntimeHost+offset)` frames under `coreclr_initialize` ← `libhostpolicy.so corehost_main` ← `libhostfxr.so` 10.0.14 ← `dotnet`.
  - `memfd_create`/`ftruncate` are adjacent return addresses `+0x40da3f`/`+0x40da5b` in one function.
  - `socket`/`fchmod`/`bind` are `+0x32f99b`/`+0x32f9bd`/`+0x32fac8` in one function, and share the same caller chain `+0x3eb553`.
- **Symbol caveat.** Without runtime debug symbols, libunwind can name only the nearest *exported* symbol. Internal function names, such as the double-mapper or diagnostic-server IPC creation code, are an inference from runtime source as reviewed in #715. The trace does not show them.

## fd lifecycle proof

| Requirement | memfd (fd 8) | Diagnostics socket (fd 9, inode 4658923) |
|---|---|---|
| Creator observed in trace | Yes: line 8721, TID 960791 | Yes: line 8918, TID 960791 |
| Created after strace attach, not inherited | Yes. Not present at execve; number 8 previously held cpu.max and was closed | Yes. Not present at execve; number 9 previously held urandom and stat |
| Mutator links to the same object | `ftruncate` on `8</memfd:doublemapper>` = 2 TiB, return 0 | `fchmod` on `9<UNIX-STREAM:[4658923]>`, mode 0600, return 0 |
| Duplication / transfer | None. Only 5 `dup2` and 22 `fcntl F_DUPFD*` calls in the trace, none on fd 8/9. No `close_range`, `pidfd_getfd`, or `sendmsg`/`recvmsg` at all (so no SCM_RIGHTS passing). No non-thread `fork`/`clone` after exec, and `CLOEXEC` is set | Same |
| Shared fd group | 9 runtime threads via `clone3(CLONE_VM\|CLONE_FILES\|CLONE_THREAD…)`, one fd table. 149 lines on TID 960791 and 9 `mmap` lines on TID 960814 use fd 8, all `MAP_SHARED` mappings of the same memfd | No fd 9 use by other TIDs in traced syscalls |
| Binding / peer | N/A | Bound to the full diagnostics path and listening. **No accept observed during this test** (`accept`/`accept4` traced: 0). `poll`/`epoll` were outside the requested list |
| Closure | No explicit `close(8)`. Released at `exit_group(0)` of the shared fd group | Path unlinked at shutdown (line 54294). No explicit `close(9)`. Released at `exit_group(0)` |
| Protected-file target | None (anonymous memfd) | None (socket inode, path in the lane TMPDIR) |

**Protected-path check.** I searched the whole trace for protected roots: `CCF-710-ModeRepair`, `CCF-Primary`, `CCF-Legacy`, `/media/jer/ZX20`, `Repository.git`, and `CCF-697`/`692`/`639`/`651`/`649`/`703`/`712`. There were **0 hits**.

- **Successful write-capable opens:** only the wrapper's own `launch-env.txt`/`initial-fds.txt` in the evidence dir, and two `/proc/self/task/<tid>/comm` (thread naming).
- **Failed write open:** one `O_RDWR /dev/shm/sem.clrst…`, which returned ENOENT.
- **Clone paths:** read-only opens of the CLI's `bin/` files and the fixture directory, plus the runtime's failed probes for `libc`/`liblibc.so` in `bin/`.
- **Other sockets:** `/var/run/nscd/socket` (from `ls`), `/var/run/lttng/lttng-ust-sock-8` and `$HOME/.lttng/…`. All are failed `connect` calls with ENOENT.

## Gaps and limits (stated, not hidden)

1. **Implicit closure.** strace cannot print the kernel's fd release at exit. Closure is inferred from `exit_group(0)` plus all 9 runtime TIDs reporting `+++ exited`. No explicit close was observed.
2. **Peers outside the subtree.** A process outside the traced subtree could `connect` to the listening path while it existed (0600, same uid), and this trace could not see the connect itself. The server side shows no `accept`, so no accepted peer session existed in this process during this test. `poll`/`epoll_wait` were not traced.
3. **Debug pipes.** The runtime also created and removed `clr-debug-pipe-960791-14199553-in/out` in TMPDIR. Creation (`mknodat`/`mkfifo`) is outside the packet's syscall list; only the unlinks are visible. They are not historical mutator targets. A future whole-L4 capture should add `mknod,mknodat` if pipe provenance matters.
4. **Stack names.** As explained under the event table, internal function names come from source inference, not from the trace.
5. **Scope.** This is one CLI process, not the 19 historical L4 processes. Their fd numbers (8 memfd / 9 socket), size and mode match this pilot's pattern exactly, but that is consistent evidence, not proof.

## Recommendation

- Treat this as **PILOT PROVEN**. Keep **default L4 FAIL38** and the strict analyzer.
- If the Manager accepts, the next step is the packet's separate bounded step: one instrumented whole-L4 role run on exact R using this command's syscall scope plus `mknod,mknodat`, followed by independent review, before any targeted analyzer classification.
- Any launch-setting change (diagnostics or W^X) remains a separate owner security decision.

## Evidence (retained, Files/ext4)

`/media/jer/Files/CCF/Verification/CCF-720-L4LifecyclePilot-20261118/` — `SHA256SUMS.txt` (circular self-entry excluded):

```
c9ede092f535c245d660dd355394fc11e62e4a58fed912e3d1bc4ce563be97f1  ./build.log
9937052a50a1dc098d4e2eb1f7226c9f9aecd97d9abea39aefd93cbc14c40756  ./env-binaries.txt
d96e84a187d24d9259569a0e5d000111bf140beb9fb66cf84c7675f6954ee269  ./guard-post.txt
d96e84a187d24d9259569a0e5d000111bf140beb9fb66cf84c7675f6954ee269  ./guard-pre.txt
d9754304c4e05d14fbf64b84df3a9b608d6ad2d69359e68d6ecb1373d280207d  ./run/initial-fds.txt
fcd06a1a95f843403155fc7a42e02399d6d85b242a50f1f12fe988164cd09c43  ./run/launch-env.txt
6f9861c68c4be144167f472b811171b74f16c0c73ae76b0ec561f9440df53e73  ./run/lifecycle-extract.txt
404c4da787599cf2de00c8d50515ba5060a357108b4400a1c5254a81e7f7e29c  ./run/mutator-stacks.txt
93ff7811a209e2a8479230bbb9b6bc19f7f311d3af383ec350c1db2a7e7d5494  ./run/rc.txt
e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855  ./run/stderr.txt
8d8061020c4e6372d1b7a5ba67c820772270abc87b92601f06f06016e34377a0  ./run/stdout.txt
ac9f891f0045ba51b7b9d29f6eb2302cbf19ce9877da547b6bddd2d4399e9e4a  ./run/strace.txt
1728aac37befd47218ab19583f42bedd128eea30c0ee0479b2983dc712aae2bd  ./run/times.txt
e4a51f2611736ca5719cce4c45a66f170ff6accd467bdef660aaa6021766ab34  ./run/tmpdir-after.txt
ae0870a194cb85a2de3c7ed07cfdc02070b7426f55f5d85cc0cdf94ca05c483c  ./run/tmpdir-before.txt
```

Run and guard scripts, plus the board/lanes pre-write copies, are in `/media/jer/Files/CCF/Operations/CCF-720-L4Pilot-20261118/receipts/`. The disposable clone, build caches and TMPDIR were removed after the evidence was sealed. The #710 store (`repair/…` still at R), `ccf-710-src` and its custody row are untouched.

## Not done (out of scope)

Not done: annual engine, 131-test suite, 19-process isolation suite, analyzer patch/whitelist, alternate launch flags, production service, Unity, Windows, main merge, and any L4 waiver.
