# Multi #654 — Independent #651 controlled-runtime L4 security review

**Date:** 16 November 2026  
**Review verdict: APPROVED — narrowly, for the #649 source-level fail-closed correction and the #651 submitted *configuration-specific* F0 evidence record.**  
**Not approved:** default-runtime L4, unqualified production/P0 write isolation, fixture/trust promotion, a first ecological year, Unity, or a Windows build. This independent security review is **not** Manager #641's separate F0 acceptance.

## 1. Reviewer identity, custody and actual work

- **Reviewer:** OpenAI ChatGPT **GPT-6**, non-author browser/GitHub reviewer; High effort requested by owner, product effort setting not independently attested. The implementation and local verification author is separately recorded as Anthropic Claude Code Opus 5.5 on native Ubuntu; no self-review.
- **Claim:** [#654](https://github.com/jermur788/CCF/issues/654), actual [RUNNING registration](https://github.com/jermur788/CCF/issues/654#issuecomment-6931328600) made **before** this Markdown was written.
- **Dedicated branch:** `review/multi-p1-654-controlled-runtime-l4-delta-20261116-v1`, created from exact starting source/result head `0cf9fea7dc4c4056b938929e455c7bf766d14957`. No local worktree; GitHub/browser only.
- **Live ownership checked:** current `main:Docs/Project/coordination/active-tasks.md`, especially the existing #639 single-writer row (near the end of the live file). #639's Claude Ubuntu owner exclusively retains `Tools/MultiRackSitkaP1Lab/**` and the #639 result. #654 owns **only this one new review Markdown** on a separate branch. Refreshed claims [#639](https://github.com/jermur788/CCF/issues/639), [#644](https://github.com/jermur788/CCF/issues/644), [#646](https://github.com/jermur788/CCF/issues/646), [#649](https://github.com/jermur788/CCF/issues/649), [#651](https://github.com/jermur788/CCF/issues/651), [#654](https://github.com/jermur788/CCF/issues/654) and Manager [#641](https://github.com/jermur788/CCF/issues/641). Issue titles on older claims are sometimes stale; actual comments/handoff and Git comparison control.
- **Native actions NOT RUN:** no Ubuntu .NET execution, `strace`, shell wrapper replay, independent local trace or checksum readback, production source/fixture edit, physical worktree/lease, P0 modification, ecological year, Unity, save, host/UI or Windows build.

### Exact source comparison

| Edge | Inspected GitHub result | Changed paths |
|---|---|---|
| #646 predecessor `33c0506990eefb6bea4277a553d2fe38408bb353` → #649 repair `9fc3a88b378b0cfff55d87afdd74f28a2dbf411c` | One commit, +331/−121 | **Only** `Tools/MultiRackSitkaP1Lab/tests/l4_analyze.py` and `tests/p0-isolation.sh` |
| #651 source candidate `342cce6d8db16d8ec0acb11d0a5d7805b5541cc4` → author's §10 head `0cf9fea7dc4c4056b938929e455c7bf766d14957` | One commit, +110/−0 | **Only** already reserved `Docs/Handoffs/MultiRack-Simulation-20261116/Multi-Sitka-P1-639-F0-THIRTY-METRE-FOUNDATION-IMPLEMENTATION-RESULT-20261116.md` |
| Author branch at reviewer start | Exact published `0cf9fea7dc4c4056b938929e455c7bf766d14957` | No observed floating-head drift at registration |

The reviewer fetched the final analyzer and harness blobs, and verified the compiled trust-table source still lists **three original-P0 records and one P1 derived catalog as `PendingIndependentReview`**, without recorded acceptance identifiers: [TrustTables.cs](https://github.com/jermur788/CCF/blob/0cf9fea7dc4c4056b938929e455c7bf766d14957/Tools/MultiRackSitkaP1Lab/src/Core/TrustTables.cs#L52-L96). This is a focused status check, **not** a fresh re-audit of the frozen three P0 values, 100 trees or the entire trust catalog.

## 2. #646 blocker and #649 source correction — independent static judgment

The prior independent [#646 NEEDS CHANGES review](https://github.com/jermur788/CCF/blob/review/multi-p1-646-644-file-safety-delta-20261116-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-P1-646-INDEPENDENT-644-FILE-SAFETY-DELTA-RESULT-20261116.md#L46-L64) established a genuine false negative: the old analyzer could count `ftruncate(9, 0) = 0` as a mutation without generating a protected HIT or UNRESOLVED record. The new [explicit `ARG_SPEC`](https://github.com/jermur788/CCF/blob/9fc3a88b378b0cfff55d87afdd74f28a2dbf411c/Tools/MultiRackSitkaP1Lab/tests/l4_analyze.py#L41-L66), [top-level argument splitter and FD decoding](https://github.com/jermur788/CCF/blob/9fc3a88b378b0cfff55d87afdd74f28a2dbf411c/Tools/MultiRackSitkaP1Lab/tests/l4_analyze.py#L83-L175), and [classification/exit policy](https://github.com/jermur788/CCF/blob/9fc3a88b378b0cfff55d87afdd74f28a2dbf411c/Tools/MultiRackSitkaP1Lab/tests/l4_analyze.py#L178-L242) address this exact code path:

1. **FD-only and zero-path fallback — PASS at source level.** `ftruncate`, `fallocate`, `fchmod`, `fchown`, `fsetxattr`, `fremovexattr` and the `utimensat(fd, NULL)` form use explicitly positioned FD arguments. Bare or non-absolute undecoded descriptors cause a reason, and a successful mutation without any target causes UNRESOLVED. Both are fatal. Protected decoded absolute targets become HIT; unknown return `= ?` is conservatively not classified as a failed operation.
2. **Relative and multi-target mutations — PASS at source level.** Legacy path operations use tracked process cwd, `*at` operands are paired with their respective dirfds, and a relative path with unknown cwd/dirfd generates UNRESOLVED. Both source and destination of rename/link are examined. Successful write-intent opens additionally use the returned FD's decoded target, catching a simple symlink routing case. The trace parser handles unfinished/resumed syscall pairs. This is lexical path tracking, not universal kernel path-resolution proof.
3. **Failed protected attempts — PASS at source level.** A failed mutation is not falsely counted as an executed write, but if its resolved target is protected it enters `failedProtectedAttempts`, which is fatal. Failed calls elsewhere are not treated as successful writes.
4. **Read-only exclusions — PASS at source level.** Nonmutating read calls are ignored; `open`/`openat`/`openat2` without the specified write-intent flags are ignored. This is intentionally a syscall-set monitor, not a read/write sandbox.
5. **Regressions/decoy identity — PASS as static test design, author-native outcome NOT RUN by reviewer.** [Self-test cases](https://github.com/jermur788/CCF/blob/9fc3a88b378b0cfff55d87afdd74f28a2dbf411c/Tools/MultiRackSitkaP1Lab/tests/l4_analyze.py#L266-L320) contain 23 assertions (including bare FD, decoded protected, failed protected, read-only, relative dirfd, resumed and duplicate-sighting negatives); [`--expect`](https://github.com/jermur788/CCF/blob/9fc3a88b378b0cfff55d87afdd74f28a2dbf411c/Tools/MultiRackSitkaP1Lab/tests/l4_analyze.py#L245-L263) matches operation identity, not a raw count. The [harness](https://github.com/jermur788/CCF/blob/9fc3a88b378b0cfff55d87afdd74f28a2dbf411c/Tools/MultiRackSitkaP1Lab/tests/p0-isolation.sh#L46-L91) now requires 13 **distinct** expected decoy `(syscall,target)` identities, zero unresolved and a clean read-only control. One operation repeated cannot satisfy a *different* expected identity.

**Source interpretation:** the old proven bare-FD fail-open has been corrected, not whitelisted. The corrected analyzer is genuinely stricter. These are code-flow findings from GitHub; the reported 23/23, 13/13 and real kernel traces are author-run evidence, not reviewer-run assertions.

## 3. #651 controlled .NET verification — submitted evidence, not a native replay

[Author's §10](https://github.com/jermur788/CCF/blob/0cf9fea7dc4c4056b938929e455c7bf766d14957/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-Sitka-P1-639-F0-THIRTY-METRE-FOUNDATION-IMPLEMENTATION-RESULT-20261116.md#L335-L436) and the [Manager's binding Option A packet](https://github.com/jermur788/CCF/blob/docs/multi-p1-651-controlled-runtime-trace-option-a-20261116-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-P1-651-CONTROLLED-DOTNET-TRACE-OPTION-A-VERIFICATION-PACKET-20261116.md) agree in scope. The published Git compare proves **no analyzer, harness, P1 source or launch configuration was committed in #651**. Author-reported commands:

- **A, ordinary/default:** `env -u DOTNET_EnableWriteXorExecute -u DOTNET_EnableDiagnostics tests/p0-isolation.sh …`
- **B, isolated verification subprocess:** `env -u … DOTNET_EnableWriteXorExecute=0 DOTNET_EnableDiagnostics=0 tests/p0-isolation.sh …`

The external `run651-wrapper.sh` is **not committed**. The author says its shell had no switches before or after, the altered settings were only inherited by B's child process tree, and `dotnet-env-seen-by-subprocess.txt` showed only one other pre-existing variable, `DOTNET_BUNDLE_EXTRACT_BASE_DIR`, identical between runs. **Browser conclusion:** the reported scoping is consistent with the stated shell-command structure, and the source did not change. It is **not independently proved** that the external wrapper actually ran those commands or that the ambient environment remained clean; `scope-check.txt` and raw environment receipts were inaccessible here.

### A/B comparison: do not collapse these columns

| Gate / signal | A — default .NET | B — verification-only switches | Provenance |
|---|---|---|---|
| **L4** | **FAIL:** 38 UNRESOLVED = 19 `ftruncate` on `memfd:doublemapper` and 19 `fchmod` on diagnostic sockets | **PASS in traced scope only:** 0 UNRESOLVED | Author's §9–§10 raw-trace summary; no independent native replay |
| Protected tree | 0 P0 HIT and 0 failed protected attempts reported | Same | Author's JSON/trace summary only |
| L0/L1/L2/L3/L3+ | PASS | PASS | Author's receipts; Git independently confirms committed P0 source/tree status, not local state |
| P0 tracked + ignored | 52 tracked digests identical, ignored set empty, L2 unchanged | Same | Author-reported local hashes; no reviewer physical filesystem inspection |
| Analyzer negative cases | 23/23; 13/13 decoy | 23/23; 13/13 decoy; bare FD exit 1, protected FD exit 1, benign exit 0 | Code inspected; run outcome author-reported |
| `generate-manifest --check` / `--stdout` | Identical fixture bytes/hash | MATCH | Author-reported comparison |
| `verify-derived`, three original inspections and pending refusals, three domain `annual` refusals | Recorded and refused appropriately | MATCH | Author-reported; **no annual ecological year executed** |
| Core tests and built binaries | 33/33; binaries hashed | MATCH; same 33/33 and byte-identical binaries | Author-reported |
| Host, saves, broad P0 catalog re-audit, ecological year, Unity, Windows | NOT RUN | NOT RUN | Explicit in result |

The A/B matching output/binary and refused-command evidence supports **a bounded inference** that these particular F0 smoke operations behaved equivalently under the two environments; it cannot establish global .NET configuration equivalence, absolute absence of writes, production/default L4 qualification, race freedom, or simulation readiness.

**114 successful-mutation delta:** §10 reports A 926 versus B 812 and attributes differences to 19 diagnostic-socket `fchmod`, 19 W^X `memfd` `ftruncate`, 38 debug-FIFO creation and 38 removals, 16 socket unlinks, two thread-name writes, a small MSBuild shared-memory difference and 20 NuGet cache entries rewritten in B only. This is a useful author trace breakdown, **not a separately reproduced causal attribution**. The NuGet cache timing explanation is explicitly **unproven**; neither run proves that a later cache or scheduling state will always produce identical other writes. All reported differences are outside the protected P0 tree; independent native inspection of the raw trace was not possible.

## 4. Required security crosswalk and findings

| Finding | Review status | Severity / gate |
|---|---|---|
| **B-01** arbitrary `generate-manifest` output removed | **PASS inherited** from independent #646: stream-only stdout/derived check; no source change since | Previously blocking, closed at source level |
| **M-01** P0 Core reference builder output location | **PASS inherited** for trusted single-user, non-racing host; same-user pathname TOCTOU explicitly remains | Residual boundary, not general race-free protection |
| **M-02** bounded, no-follow derived fixture verification | **PASS inherited** from #646, unchanged | Previously major, closed at source level |
| **m-01** duplicate tree XY parser refusal | **PASS inherited** from #646, unchanged | Minor closed |
| **m-02** bare-FD/unknown-path L4 false-negative + weak decoy count | **PASS now at source level:** explicit argument coverage, successful zero-target UNRESOLVED, protected attempted writes, 23-case tests and 13 distinct identity contract | Previously #646 MAJOR; changed-lines issue addressed |
| **R-01** unreviewed external wrapper/native receipts | **LIMITATION, not a source defect.** Raw local evidence inaccessible via this browser/GitHub-only task | Do not claim independently witnessed .NET trace or env scoping |
| **R-02** default-runtime L4 remains red | **FAIL remains open by design:** 38 reported unresolved, despite no reported P0 HIT | May not be reported as default-runtime security PASS |
| **R-03** trace and path resolution limitations | **DISCLOSED:** traced syscall set and process descendants only; `fchmodat2` is modelled by analyzer but not included in current harness capture; no deletion mutators in decoy; legacy relative paths lexical, thread-cwd/symlink caveats; prior same-user builder race persists | No universal write-proof claim |
| **R-04** unusual FD annotation robustness | **LOW-priority static observation:** `decoded_abs` treats angle-bracket text beginning `/` as a path when it matches the FD grammar. The reported `/memfd:doublemapper` examples have a trailing `(deleted)` decoration, which fails the full FD pattern and is therefore UNRESOLVED, as desired. A hypothetical undecorated `fd</memfd:name>` would instead look like an absolute path. No evidence that this alternative annotation occurred in the submitted runs; revisit if reusing the analyzer on another strace format. | No supported F0 protected-path regression, but not a generic annotation classifier |

**No new blocker or major finding supported within the exact #654 F0 source/record scope.** The missing local raw files restrict *confidence/provenance*, not the source correction verdict. The default-runtime L4 failure is a known qualification limitation, not something this reviewer silently accepted as PASS.

## 5. Evidence provenance / not-run matrix

| Claim | Evidence independently inspected | Status |
|---|---|---|
| #646 predecessor bug and #649 repair coverage | Exact before/after GitHub Python and shell source; source line references above | **DIRECT SOURCE INSPECTION** |
| #649 commit path ceiling, #651 no source changes | GitHub compare metadata + fetched relevant files | **DIRECT GIT EVIDENCE** |
| Live single-writer ownership, branch/head, four pending catalog records | Live main board, issue comments, current source branch ref, compiled catalog source | **DIRECT GIT/RECORD INSPECTION** |
| Default 38 and switched 0, 33/33, 52 P0 digests, 13/13, produced binaries | Published author §9/§10 and #651 issue handoff | **AUTHOR-REPORTED, NOT INDEPENDENTLY REPLAYED** |
| Actual wrapper/environment receipt and full `strace` JSON, checksum manifest | Reported external `/media/jer/Files/CCF/Verification/CCF-651-P1-F0-ControlledDotnetTrace-20261116/` is not mounted or GitHub-committed | **NOT INSPECTED / NOT RUN** |
| Runtime/JIT equivalence beyond specific F0 CLI comparisons | No evidence submitted for general equivalence | **UNSUPPORTED; NOT CLAIMED** |
| Full forest, original P0 trust hash review, first biological year, Host/saves, Unity, Windows | Outside instruction and not executed | **NOT RUN** |

## 6. Exact gate and proportionate native follow-up

**APPROVED for this constrained interpretation:** the #646 fail-open in the traced mutator classification has been repaired in committed source, #651 did not weaken the analyzer or production defaults, and the submitted A/B report is internally coherent as **verification-only, configuration-specific** evidence for this bounded F0 candidate. Keep the published default L4 as **FAIL / 38 UNRESOLVED**. Neither source review nor author-run swapped-runtime trace can establish an unqualified "P0 writes impossible" guarantee.

**Is additional native verification indispensable for this exact source-level #654 verdict?** **No.** The two-file correction and nonmutating #651 Git changes are directly reviewable. **Is additional native evidence indispensable before anyone claims the raw subprocess scope/counts were independently witnessed?** **Yes**—those claims must remain author-reported unless a separate Linux reader validates the external evidence. If Manager #641 requires native-independent confirmation for a stronger F0 acceptance, request **one read-only witness** by a different worker of precisely `run651-wrapper.sh`, `scope-check.txt`, A/B `dotnet-env-seen-by-subprocess.txt`, `inputs.txt`, `exit-code.txt`, per-run `iso/L4-analysis.json`, `tracked-before/after.txt` and `SHA256SUMS`; establish (a) switched env only for B and no leakage, (b) identical strict analyzer and input pins, (c) A=38/B=0 unresolved and both zero P0 hits/failed protected attempts, (d) matching 52 P0 file hashes. In the evidence root, `sha256sum -c SHA256SUMS` is the minimal integrity command; inspect listed files as read-only and report their exact actual paths before any compare. **Do not rerun the build/test/forest suite or first ecological year.**

**Mandatory next step:** Manager [#641](https://github.com/jermur788/CCF/issues/641) inspects this independent result and issues its **separate** F0 APPROVED / NEEDS CHANGES / REJECTED decision, with explicit status of the default-run failure and provenance confidence. The four `PendingIndependentReview` records must remain pending unless a **separately reviewed acceptance-status source commit** is specifically authorised and completed. First ecological year remains on **HOLD**; the annual adapter is not implemented. No P1/P0 source modification, `main` merge, Unity or Windows build is authorized by #654.

**Publication boundary:** One new Markdown on the independent review branch only; source author still owns P1. GitHub handoff and issue notifications are required; dashboard ingestion/readback, Drive and physical Ops synchronization are **NOT VERIFIED**.
