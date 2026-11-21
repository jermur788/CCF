# Multi #885 — Stage B P0 isolation actual execution result

**Formal qualification verdict: NEEDS CHANGES. Stage B FAILED. Return to Manager #885. Stage I remains NEEDS CHANGES; Q1 is NOT AUTHORISED.**

Worker: separate GPT-6.1 Sol / Codex in T3 Code, high-difficulty dispatch, actual configured reasoning effort medium. Qualification executor only, not Manager/custodian/reviewer. One unchanged harness invocation; no repair, retry, waiver, integration, Unity or Q1 execution.

## Authority and identity

Complete Manager handoff and D-091 custodian addendum read:
- https://github.com/jermur788/CCF/blob/94b84ff3c6b09bf5e9f590af5e82473e1d91a759/Docs/Handoffs/MultiRack-Simulation-20261116/885-qualification-programme/Multi-885-MANAGER-STAGE-B-OWNER-STRACE-PREREQUISITE-AND-Q1-SURVIVING-CROP-RULING-20261121.md
- https://github.com/jermur788/CCF/blob/efb188ace4dc5489c7a0c8852d5cf5e323280335/Docs/Handoffs/MultiRack-Simulation-20261116/885-qualification-programme/Multi-885-D091-CUSTODIAN-ADDENDUM-DISPOSABLE-P0-LANE-AND-EPHEMERAL-PORTS-20261121.md

Candidate: 4cecf87339bea4d32907955a8d5625d1f27f96bd, detached HEAD, no source branch created. Candidate context commit identified: 62e39ec621e38a84030c08b35a171e297056c9ed; repository workflow/context inspected, coordination separately fetched live.

Exact disposable path: /home/jer/Development/CCF-HP-Worktrees/CCF-Multi-885-P0Isolation-Disposable-20261121. Verified absent before creation; removed at documented completion.
Evidence: /home/jer/Multi-885-StageB-20261121-evidence.
TMPDIR: evidence root's tmp/ outside repository and outside /tmp; removed after preserving evidence.
Start: 2026-11-21T22:41:47+00:00; final custody check: 2026-11-21T22:48:35+00:00.
Documentation branch: docs/multi-885-stageb-p0-isolation-result-20261121-sol-v1, based on live main 3a67d0987be0cf40d007892739d2ad295c84f0e0; only this new Markdown, no main edit.

## Prerequisites and invocation

Owner installation was reported by the direct user dispatch. Owner's exact install command was not supplied or independently witnessed. Installed executable/package independently verified; no agent install, sudo or substitution:

```text
DESKTOP-DL1BQI7
uid=1000(jer) gid=1000(jer) groups=1000(jer),27(sudo)
6.18.58.1-microsoft-standard-WSL2
strace -- version 6.19
Copyright (c) 1991-2026 The strace developers <https://strace.io>.
This is free software; see the source for copying conditions.  There is NO
warranty; not even for MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.

Optional features enabled: stack-trace=libunwind stack-demangle m32-mpers mx32-mpers
strace	6.19+ds-0ubuntu5
8.0.134
Python 3.14.4
sha256sum (uutils coreutils) 0.10.0
```

Host /etc/os-release: Ubuntu 26.04.1 LTS. /home/jer had 879 GiB free; /tmp is 3.9 GiB tmpfs and was not used as harness scratch. PATH explicitly prepended /home/jer/.dotnet; generated provenance also says SDK 8.0.134.

Live default-branch active-tasks.md and latest #885 comments fetched before creation; board re-fetched after completion and byte-identical. No conflicting claim or revocation observed. The board had not yet registered this new executor dispatch. Execution authority was explicit user dispatch plus exact Manager/custodian grant; no board transfer invented. No physical Ops board or installed Ops state changed.

Invocation from disposable repository root:

```bash
export PATH=/home/jer/.dotnet:$PATH
export TMPDIR=/home/jer/Multi-885-StageB-20261121-evidence/tmp
bash Tools/MultiRackSitkaP1Lab/tests/p0-isolation.sh /home/jer/Multi-885-StageB-20261121-evidence/run
```

Harness, analyzer and builder inspected before execution. Their before/after SHA-256 manifests match exactly:

```text
b2b990a3a01f994ffca0f08cc3ffac15639f5293016c92dd12789497121db41f  Tools/MultiRackSitkaP1Lab/tests/p0-isolation.sh
35c549f7f13e8f77ff064af5562ec9ca2bbe5769bf77ed02b342395210ddb7f5  Tools/MultiRackSitkaP1Lab/tests/l4_analyze.py
e4e00479544d98cc2738ff2d96ee30ea74cf4a9f620ed3ef93c0dd25f6f05853  Tools/MultiRackSitkaP1Lab/build/build-p0-core-ref.sh
```

## Actual L0–L4 results

| Gate | Actual result |
|---|---|
| L0 | PASS emitted by harness: no commit touches P0 since a7f41983c2f2f233b8fe999d1a5b10e72bdd2866. |
| L1 | PASS emitted: exact committed P0 tree f212c8f64fb28b83aa870c7b5b7c72e03051e46a. |
| L2 | Harness final check NOT REACHED. Independent post-run identical-command comparison PASS: empty P0 status before/after including ignored/untracked. |
| L3 | Harness final check NOT REACHED. Independent post-run path/size/mtime comparison PASS: zero ignored P0 files before/after. |
| L3+ | Harness final check NOT REACHED. Independent SHA-256 comparison PASS: all 52 tracked P0 files unchanged; disposable tracked status empty. |
| L4 self-test | FAIL: synthetic 23/23 PASS; decoy 10/13 required identities, zero unresolved; read-only control exit 0. |
| L4 full cold build/test/CLI | INCOMPLETE / NOT PASSED: P1 build refused generated reference before tests/CLI; final analyzer and OVERALL lines never reached. |
| Generated DLL pin | FAIL, actual generated SHA-256 differs from mandatory expectation. |
| Harness exit | 1. |

Independent comparisons supplement the aborted script; they do not pretend the script emitted final L2/L3 PASS or completed L4.

## Failure 1 — L4 host incompatibility

```text
mutating calls succeeded=13 failed=0 protected hits=12 unresolved=0 failed protected attempts=0
HIT mkdir ['/home/jer/Multi-885-StageB-20261121-evidence/tmp/ccf-p1-l4decoy.3VuT3VNC/protected/sub']
HIT openat ['/home/jer/Multi-885-StageB-20261121-evidence/tmp/ccf-p1-l4decoy.3VuT3VNC/protected/sub/rel.txt']
HIT mkdir ['/home/jer/Multi-885-StageB-20261121-evidence/tmp/ccf-p1-l4decoy.3VuT3VNC/protected/sub/d']
HIT renameat2 ['/home/jer/Multi-885-StageB-20261121-evidence/tmp/ccf-p1-l4decoy.3VuT3VNC/protected/sub/r2.txt', '/home/jer/Multi-885-StageB-20261121-evidence/tmp/ccf-p1-l4decoy.3VuT3VNC/protected/sub/rel.txt']
HIT linkat ['/home/jer/Multi-885-StageB-20261121-evidence/tmp/ccf-p1-l4decoy.3VuT3VNC/protected/sub/hl', '/home/jer/Multi-885-StageB-20261121-evidence/tmp/ccf-p1-l4decoy.3VuT3VNC/protected/sub/r2.txt']
HIT symlink ['/home/jer/Multi-885-StageB-20261121-evidence/tmp/ccf-p1-l4decoy.3VuT3VNC/protected/sub/sl']
HIT openat ['/home/jer/Multi-885-StageB-20261121-evidence/tmp/ccf-p1-l4decoy.3VuT3VNC/protected/sub/t.txt']
HIT chmod ['/home/jer/Multi-885-StageB-20261121-evidence/tmp/ccf-p1-l4decoy.3VuT3VNC/protected/sub/r2.txt']
HIT openat ['/home/jer/Multi-885-StageB-20261121-evidence/tmp/ccf-p1-l4decoy.3VuT3VNC/protected/sub/tr.txt']
HIT ftruncate ['/home/jer/Multi-885-StageB-20261121-evidence/tmp/ccf-p1-l4decoy.3VuT3VNC/protected/sub/tr.txt']
HIT openat ['/home/jer/Multi-885-StageB-20261121-evidence/tmp/ccf-p1-l4decoy.3VuT3VNC/outside/via/through-link.txt', '/home/jer/Multi-885-StageB-20261121-evidence/tmp/ccf-p1-l4decoy.3VuT3VNC/protected/through-link.txt']
HIT mkdir ['/home/jer/Multi-885-StageB-20261121-evidence/tmp/ccf-p1-l4decoy.3VuT3VNC/protected/pyrel']
expected identities matched=10/13 missing=[('symlinkat', 'sub/sl'), ('utimensat', 'sub/t.txt'), ('fchmodat', 'sub/r2.txt')] unexpected=[('chmod', 'sub/r2.txt'), ('linkat', 'sub/r2.txt'), ('renameat2', 'sub/rel.txt'), ('symlink', 'sub/sl')]
```

Missing identities: symlinkat sub/sl, utimensat sub/t.txt, fchmodat sub/r2.txt.
Selected syscall trace contains symlink and chmod instead of expected corresponding *at calls; utimensat identity absent. Underlying host implementation for the absent timestamp call is not established. This reproduces the anticipated Ubuntu26.04/Python3.14/uutils harness-host incompatibility; it does not establish a product defect. No expectation edited.

## Failure 2 — regenerated binary identity

Required SHA-256:
9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3

Generated SHA-256:
bcbdbf05d1f36334fbbf113da70355272952bd9dc9046a9bb78e858e61d697b0

Builder completed its extracted-input before/after checks and reported SDK 8.0.134. Exact P0 source tree remained pinned. Binary difference cause is UNRESOLVED; Ubuntu alone is not established as its cause. No rebuild, normalization or rebaseline attempted.

Complete captured console:

```text
L0 PASS: no commit in a7f41983c2f2f233b8fe999d1a5b10e72bdd2866..HEAD touches Tools/MultiRackSitkaLab
L1 PASS: HEAD P0 tree f212c8f64fb28b83aa870c7b5b7c72e03051e46a == pin
L4 self-test FAIL: synthetic ok=1; decoy identities 10/13, unresolved 0; read control exit 0
P0 Core reference built: /home/jer/Development/CCF-HP-Worktrees/CCF-Multi-885-P0Isolation-Disposable-20261121/Tools/MultiRackSitkaP1Lab/.p0ref/SitkaLab.Core.dll sha256=bcbdbf05d1f36334fbbf113da70355272952bd9dc9046a9bb78e858e61d697b0
/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-885-P0Isolation-Disposable-20261121/Tools/MultiRackSitkaP1Lab/src/Core/SitkaP1.Core.csproj(36,5): error : P0 Core reference hash BCBDBF05D1F36334FBBF113DA70355272952BD9DC9046A9BB78E858E61D697B0 does not match the expected 9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3. Rebuild it with build/build-p0-core-ref.sh; do not edit P0.

Build FAILED.

/home/jer/Development/CCF-HP-Worktrees/CCF-Multi-885-P0Isolation-Disposable-20261121/Tools/MultiRackSitkaP1Lab/src/Core/SitkaP1.Core.csproj(36,5): error : P0 Core reference hash BCBDBF05D1F36334FBBF113DA70355272952BD9DC9046A9BB78E858E61D697B0 does not match the expected 9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3. Rebuild it with build/build-p0-core-ref.sh; do not edit P0.
    0 Warning(s)
    1 Error(s)

Time Elapsed 00:00:07.03
```

Unchanged script continued from self-test failure into its prescribed cold build. Inner set -e stopped on hash refusal, preventing all P1 tests/CLI and final gates.

## Partial real-trace diagnostic, not qualification

After the failed run, unchanged analyzer was invoked read-only on its partial real trace using the original protected path and initial cwd, with separate diagnostic outputs. Exit 1:

```text
mutating calls succeeded=549 failed=103 protected hits=0 unresolved=12 failed protected attempts=0
UNRESOLVED ftruncate fd argument 0 not decoded to an absolute path | 9</memfd:doublemapper>(deleted), 2199023255552
UNRESOLVED fchmod fd argument 0 not decoded to an absolute path | 10<socket:[378258]>, 0600
UNRESOLVED ftruncate fd argument 0 not decoded to an absolute path | 9</memfd:doublemapper>(deleted), 2199023255552
UNRESOLVED fchmod fd argument 0 not decoded to an absolute path | 10<socket:[377406]>, 0600
UNRESOLVED ftruncate fd argument 0 not decoded to an absolute path | 9</memfd:doublemapper>(deleted), 2199023255552
UNRESOLVED fchmod fd argument 0 not decoded to an absolute path | 10<socket:[379295]>, 0600
UNRESOLVED ftruncate fd argument 0 not decoded to an absolute path | 9</memfd:doublemapper>(deleted), 2199023255552
UNRESOLVED fchmod fd argument 0 not decoded to an absolute path | 10<socket:[381021]>, 0600
UNRESOLVED ftruncate fd argument 0 not decoded to an absolute path | 9</memfd:doublemapper>(deleted), 2199023255552
UNRESOLVED fchmod fd argument 0 not decoded to an absolute path | 10<socket:[377624]>, 0600
UNRESOLVED ftruncate fd argument 0 not decoded to an absolute path | 9</memfd:doublemapper>(deleted), 2199023255552
UNRESOLVED fchmod fd argument 0 not decoded to an absolute path | 10<socket:[377626]>, 0600
```

12 unresolved records involve /memfd:doublemapper and socket descriptors. Zero protected hits or failed protected attempts in this partial scope cannot establish L4. No whitelist or syscall edit. Documented untraced-syscall, external-process, legacy-symlink and per-thread-cwd limits still apply.

## Preservation and cleanup

WSL Git primary was clean at cbf3a6185754a33f4a75b96f8fe775d0041b72a1. Windows Git initially reported widespread differences on this same filesystem; WSL Git used for Linux custody decisions. Primary untouched.

Protected snapshots cover primary, integration, author, reviewer, #877 owner and #882 launch: HEAD/status, every P0 file hash/size/mtime-ns and existing P1 reference-file hashes. Initial snapshot collected while disposable creation/execution was starting; identical after run and final cleanup. This is a bounded witness, not a hash of every file in every foreign lane.

Integration 4cecf87339bea4d32907955a8d5625d1f27f96bd, author ac4c7b51582283fb125f6b747a76b31282451696, reviewer 58950a9aa74067f22c9fe322b27f42e036ae2e01; clean statuses preserved. Complete registered worktree list before creation and after removal byte-identical.

#877 PID1750123 and #882 PID1806361 preserved alive with original starts/cwds and listeners 127.0.0.1:33767 / 127.0.0.1:43649. #867 PID390578 absent at preflight, untouched. No new server, listener, proxy or Q1 session.

Failed build left compiler/MSBuild descendants holding own strace open. SIGTERM sent only to own PIDs 2459649, 2459754, 2459755 after verifying each TracerPid 2459540 (this run), command and trace membership. No global build-server shutdown or broad kill. Early attempted cleanup guard refused because their cwd was the SDK directory; it performed no termination. Subsequent tracer-based verification established ownership.

Only exact granted disposable lane removed with git worktree remove --force after empty tracked status, exact HEAD/path validation and evidence copy; force needed for its own ignored outputs. No worktree prune or foreign cleanup. Own temporary scratch removed; retained evidence about 3.8 MiB includes generated DLL/provenance. No tests, source, fixtures, Assets, Packages, ProjectSettings, saves, goldens or protected reference changed. No Unity or multi-year simulation.

## Formal disposition

NEEDS CHANGES is executor qualification verdict, not Manager G4 acceptance. Return both actual failures and partial-trace unresolved records to Manager #885. Any harness correction or expectation change requires a separate approved packet and independent review. Do not waive checks or follow generic build-message rebuild advice under this dispatch.

Stage I remains NEEDS CHANGES. N1 naturally-empty served UI remains NOT OBSERVED. Q1 blocked until Stage I disposition, independent harness review, separate Q1 authorisation and physical lease. Owner G2 pending. No owner action requested here.

Dashboard/coordination synchronization NOT VERIFIED: outstanding operational handoff to authorised Manager/custodian. No takeover of single-writer board or generated dashboard. GitHub publication proves durable evidence availability, not dashboard ingestion or acceptance.

## Retained evidence manifest

Root: /home/jer/Multi-885-StageB-20261121-evidence. SHA-256 below; result/publication receipts excluded to avoid self-reference.

```text
611d0f0bdd899f97f76e95728012fdb23cc98e0ff0a36b6d8016a6f443b4a7f7  885-comments-start.json
e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855  P0-ignored-after-independent.txt
e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855  P0-status-after-independent.txt
9e218ca7f29c73895ad4a66dc6ca90cf1a844126bd74e417e0d8cdc0489e30e8  P0-tracked-after-independent.txt
6faaee333771584c9493ad3ff518df8169a2563494da323ef1d7f89c4ce34273  active-tasks-final.md
6faaee333771584c9493ad3ff518df8169a2563494da323ef1d7f89c4ce34273  active-tasks-start.md
ce2f4037286f22ac433a5f4eb591f4db0964de2b6f4943c4f3707d3b9bb38e1d  console.txt
8ed0076141b1a7ad387138b5f4836e0598254acfdf09188f11cf619f3e9a7ab1  custodian.md
e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855  disposable-after.txt
af19b915fbeb3d0343046bca03706b7b9b61c3f4ada0e0b75d092e8835be1552  end-time.txt
b59db1205da78886a4e58e7e2f43689c39d05039c88e72a96ddedc9aa4ffb842  environment.txt
4355a46b19d348dc2f57c046f8ef63d4538ebb936000f3c9ee954a27460dd865  exit-code.txt
bcbdbf05d1f36334fbbf113da70355272952bd9dc9046a9bb78e858e61d697b0  generated-SitkaLab.Core.dll
2c9f341c4fca44957d9be8f8ab77f9c1687c2daf8a01692e5a138f51ae38592e  generated-p0-provenance.txt
ecfaa6fc0fbfcc1d32bbb5cf4eec1011b224052ddc7fa5c60c51145aea24328c  generated-p0.sha256
c82d9eea330277102239189f3cc341ca7e7b6bac3d3e2c03fb19404b9131beb0  harness-after.sha256
c82d9eea330277102239189f3cc341ca7e7b6bac3d3e2c03fb19404b9131beb0  harness-before.sha256
f3cbd7e060f9ac68a99e35b756f976ea7753a165784fbf6a965072b1895569d5  own-build-server-cleanup.json
4355a46b19d348dc2f57c046f8ef63d4538ebb936000f3c9ee954a27460dd865  partial-trace-diagnostic-exit.txt
46c67f7303110e1f46e8fa5d50431a41e1dc012edfe79b717266ea368f48fefb  partial-trace-diagnostic.json
89dee5487314676aee97d0cee8b0eeb2a879edb596ef12168db205e4a5a6d322  partial-trace-diagnostic.txt
e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855  primary-before.txt
a57e3ffc6fdf9868d1f4a0b9f20aa2e8791f42f3518fe40415c18611692ad92e  protected-after.json
a57e3ffc6fdf9868d1f4a0b9f20aa2e8791f42f3518fe40415c18611692ad92e  protected-before.json
a57e3ffc6fdf9868d1f4a0b9f20aa2e8791f42f3518fe40415c18611692ad92e  protected-final.json
1ed679acc6ab23c2874075fd41947cbbe009f70fa76cd5a6dbf50ff10596a252  protected-listeners-final.txt
20f011cd46f59c890cc8130abc189bfb436571111ef20a8465f22a8c11986cf4  protected-processes-final.txt
e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855  run/L2-before.txt
e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855  run/L3-before.txt
b9fdfc39201d34b4ae5eff4a5c7f0e1a8db811c432135ff18409e311ac569268  run/L4-selftest-expected.txt
e18cb8baaf72a8ebe181cca63db940076323298638a69ecfb40506434bc672d7  run/L4-selftest-read.trace
712b7602dd62ada01a163e098826790841739e7940262c3b068016d8c8c7d272  run/L4-selftest-read.txt
004cbe99ead6f69f2e4bc2aed911b5afe89f75de3d878836e97f12a434c903b3  run/L4-selftest-synthetic.txt
655a7cb3820e16a3830a32f415019fa2a3b56e58ca69f7a325e3364859966275  run/L4-selftest-write.json
e50eec5e11f0f509eaa4680a7decf60529c0ba0faa29f2bc796219ac1903196c  run/L4-selftest-write.trace
73ac01015f25f06f66cc90957eef358074861524b13d48c8dd3610b14c89629b  run/L4-selftest-write.txt
7234238190d26106a8cbdaa87a98b34a52e94d8781e86342eab9d0a5972c68b0  run/L4-strace.txt
e518f92aed2119b64e949892f6f7b926c1a4211545a621d50c6488a237cca846  run/summary.txt
9e218ca7f29c73895ad4a66dc6ca90cf1a844126bd74e417e0d8cdc0489e30e8  run/tracked-before.txt
8307fa2530aa77380acf8d710ee4cc6af5cbe548f7610db4e21858ebe132229b  snapshot.py
07614735f6a96159d39e5844c1bfaff710ad9fd487e89026f7a0ecbd1236a6cc  start-time.txt
0154f8da0a942bf493e8c9053313fa20c1b9bb0b3cfc42adea18e28a27604e86  worktrees-after.txt
0154f8da0a942bf493e8c9053313fa20c1b9bb0b3cfc42adea18e28a27604e86  worktrees-before.txt
```
