# #656 independent native receipt witness — #658 follow-up

**Verdict: APPROVED, strictly for the #651/#658 receipt packaging, present-time integrity and report erratum.** The earlier checksum/report blockers in [the independent NEEDS CHANGES findings](https://github.com/jermur788/CCF/issues/656#issuecomment-6931891738) are addressed. This is an independent read of recorded evidence; **replay NOT RUN**. No trust record is promoted and no ecological year is authorised. All four records remain `PendingIndependentReview`; first year HOLD. Default-runtime L4 remains unaccepted.

## Reviewer, scope and custody

- **OpenAI Codex `gpt-6.1-sol`, High reasoning effort, T3 Code**, independently assigned non-author; reviewed material authored by Anthropic Claude Opus 5.5. No delegated reviewer.
- Actual `uname -a`: `Linux jer-laptop 7.0.0-42-generic #42~24.04.1-Ubuntu SMP PREEMPT_DYNAMIC Mon Oct 19 14:43:01 UTC 2 x86_64 x86_64 x86_64 GNU/Linux`.
- Separate worktree `/home/jer/.t3/worktrees/CCF-Primary/t3-33b62d28-756a122f6037b4e6`; initial branch `t3/native-evidence-witness-review`, HEAD `46247486b37904f6b36825bb40fb6ed7bea35e88`, clean, no unrelated changes.
- Branch `review/multi-p1-656-native-receipts-20261116-v1`, created from fetched **origin/main start `cfbf0ca98466964c26800651b4bb9067ebb6b352`**. Remote branch absent before creation. Project context snapshot `a5541b8a193883792db290539b7a3b877b589ff2`; live coordination read separately.
- [Resumed RUNNING](https://github.com/jermur788/CCF/issues/656#issuecomment-6932300197) preceded native evidence inspection and tracked writing. Direct user dispatch authorises this separate worktree and read-only evidence access; no physical execution/Unity lease requested or acquired.
- Gate C exists on current main in **`Docs/Project/coordination/active-tasks.md`**, retained from [97ed6a2f3a51a3e91e5d98e1cff503093e7e77b0](https://github.com/jermur788/CCF/commit/97ed6a2f3a51a3e91e5d98e1cff503093e7e77b0). It reserves exactly this file/branch. The only later board commit through the start SHA concerns #659; no later #640 comment amended/superseded C at preflight. The supplied plural `Docs/Projects/...` is incorrect; the singular path was checked.

Binding scope: [#656 packet](https://github.com/jermur788/CCF/blob/docs/multi-p1-656-native-evidence-witness-packet-20261116-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-P1-656-INDEPENDENT-651-LOCAL-EVIDENCE-READ-ONLY-WITNESS-PACKET-20261116.md), [#658 packet](https://github.com/jermur788/CCF/blob/docs/multi-p1-658-evidence-addenda-report-erratum-packet-20261116-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-P1-658-CORRECTIVE-EVIDENCE-MANIFEST-ADDENDA-AND-651-ERRATUM-PACKET-20261116.md), [Manager dispatch Phase 2](https://github.com/jermur788/CCF/blob/docs/multi-p1-658-manager-executable-dispatch-20261116-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-P1-658-MANAGER-IMPLEMENTATION-DISPATCH-20261116.md), [author handoff](https://github.com/jermur788/CCF/issues/658#issuecomment-6932260531). Reviewed erratum HEAD **`72efdedc8673186a197756b0fe905157ecae9ae7`**, versus `0cf9fea7dc4c4056b938929e455c7bf766d14957`.

## PASS / PARTIAL / FAIL matrix

R = `/media/jer/Files/CCF/Verification/`. Root names are literal children of R. Each root contains the new `SHA256SUMS-CORRECTED-20261116` and `MANIFEST-CORRECTION-20261116.md`.

| Check | Independently read recorded evidence / result | Status | Severity / implication |
|---|---|---|---|
| #639 `CCF-639-P1-F0-20261116/` | Corrected **15 OK, exit 0**; 17 regular files. Historical parent `isolation-run-1.SHA256SUMS` remains in place: **14 unreadable relative paths, exit 1**. All 14 original digests match files under `isolation-run-1/` when interpreted there, without moving anything. | PASS repair; historical FAIL preserved | Original layout defect addressed by sidecar |
| #644 `CCF-644-P1-F0-FileSafety-20261116/` | Corrected **76 OK, exit 0**; 78 regular files. Historical `SHA256SUMS`: **75 OK, only self-entry FAILED, exit 1**. | PASS repair; historical FAIL preserved | No non-manifest payload failure |
| #649 `CCF-649-P1-F0-L4-FailClosed-20261116/` | Corrected **52 OK, exit 0**; 54 regular files plus two symlinks. Historical `SHA256SUMS`: **51 OK, only self-entry FAILED, exit 1**. | PASS repair; historical FAIL preserved | No non-manifest payload failure |
| #651 `CCF-651-P1-F0-ControlledDotnetTrace-20261116/` | Corrected **72 OK, exit 0**; 74 regular files. Historical `SHA256SUMS`: **71 OK, only self-entry FAILED, exit 1**; its SHA remains exactly the earlier #656 value. | PASS repair; historical FAIL preserved | Earlier medium provenance blocker addressed for present-time integrity |
| Inventories and paths, all four | Every regular file minus exactly the two sidecars equals corrected inventory; no duplicates, missing/extra entries, self/note entries, unsafe payload paths or unsupported objects. Historical manifest included as payload. Paths sorted by byte value. Exactly two extra regular files versus each historical payload list plus its manifest. | PASS | No unsafe/outside-root hashed target |
| #649 decoys | Both symlinks and literal targets documented in note, excluded from hashing and reviewer target-content reads; hard-link `sub/hl` is a regular payload. | PASS | Existing outside-root link documented below; excluded from manifests |
| Notes, all four | Authority/creator/time, old manifest path/digest, defect/result, inventory/exclusions, full new-manifest SHA and provenance boundary present. Manifest digests match actual files. | PASS content | Nonblocking caveats below |
| Erratum / source ceiling | Exact Git diff: **one Markdown, +59/−0**; four-line introductory pointer plus 55-line §10.5. Removing only these additions reproduces the prior report byte-for-byte; original §10 unchanged. No source/test/trust change in this Git delta. | PASS | Documentation-only reviewed delta |
| `comm` correction | Narrow raw `openat` parsing, joining relevant unfinished/resumed records, reproduces **A 117 / B 116 / Δ+1**; resumed contributions 3 / 4. §10.5 calls these successful write-intent opens, not proven `write` syscalls. | PASS | Earlier counting/wording defect addressed |
| Total and causal wording | Re-read JSON totals **926 / 812**. **926−812 = 19+19+38+38+16+1+3−20 = 114**. Erratum downgrades “fully attributed” to count-reconciled, causation partly unproven. Other category counts were independently established in the earlier witness; no broad repeat parser run. | PASS count; PARTIAL causation | Cache, diagnostic identity, MSBuild/thread causes remain unproven |
| Default A L4 | Re-read JSON: **38 unresolved = 19 memfd ftruncate + 19 socket fchmod**, 0 protectedHits/failedProtectedAttempts; recorded exit 1. | **FAIL, preserved** | Existing default-runtime qualification blocker; no waiver |
| Verification-only B L4 | Re-read JSON: **0 unresolved**, 0 protectedHits/failedProtectedAttempts; recorded exit 0. Erratum retains configuration/traced-scope limit. | PASS recorded configuration only | No default-runtime/general-security acceptance |
| Reviewer no-follow method | Initial safe inventory used `lstat`; later inventory predicate attempted target metadata stat before excluding symlinks. No target content read/hash or traversal. | PARTIAL | Medium process deviation; see below |
| Original execution / custody history / replay | Current payloads match historical payload digests; #651 prior-witness manifest SHA unchanged. No contemporaneous independent execution attestation or uninterrupted-custody proof. **Replay NOT RUN**. | PARTIAL / NOT RUN | Scope limit, not a fabricated pass |

**215 corrected entries total**, including four historical manifests, all passed. The three self-referential historical manifests retain their original failure. No underlying non-manifest payload digest failed, including #639's 14 payloads checked against their historical digests.

## Digest pins and symlink boundary

| Root | Actual corrected-manifest SHA-256 (matched to note) | Actual historical-manifest SHA-256 |
|---|---|---|
| #639 | `0ce1bae654adaaf508244dd332e6d8910d7331d64d6c2186634cb881fac7f214` | `64edbead8ff4d8694eaaa8c57a7532956e322b4a0df7e79c1bf120deb89f6a67` |
| #644 | `d398062e9475c54843f96f0e09cf56f39f08e17a4cec16055270855658329cc5` | `9fcbfe0f1a6b358bad40485d6d8011be2ee279623575b0606c29af6b6f2e8fce` |
| #649 | `e61085e158a4ba1e4ab37f7aa69a3f20b6c67057a8946d3208ee1304fbb8a8d7` | `efd90d6f4b3b0d770a582231cfc21f58ecb17c2038bed9f4638a6679b44ca765` |
| #651 | `8723a7d6deb2c424d36034d095ca80c3a93c1bcd2391c28e0cc2680e4958bf2d` | `27d5827aaebf0261bb2328e9d2fa300e7e5e76387446309fd5f0e866665b67a8` |

#649 `./decoy-precheck/outside/via` has literal target `/tmp/claude-1000/-media-jer-Files-CCF-Primary/63485931-a682-48a4-8756-7a63539f7492/scratchpad/decoy649.Hpww/protected` (outside root). `./decoy-precheck/protected/sub/sl` has literal target `r2.txt`. The initial safety inventory used `lstat`/`readlink`; a later inventory predicate also attempted target metadata checks (review-process deviation below). Neither target's file contents were opened or hashed. No outside-root target appears in the corrected manifest.

## Nonblocking findings and unverified properties

1. **Low — approximate time wording.** §10.5 says “about 23:2x UTC”, and the board heading says “~23:20”. Actual board commit timestamp **23:12:39Z**, author RUNNING **23:13:01Z**, and notes **23:14:02Z** (#644/#649/#651) / **23:14:03Z** (#639), consistent with sidecar mtimes, show prospective recorded ordering. The approximate minute labels are inaccurate; use exact times above for chronology. This does not obstruct verification or authenticate historical execution.
2. **Low — generation mechanism unproven.** Notes say the old self-entry was “hashed while being written”. Self-inclusion and mismatch are verified; the original creation/rewrite mechanism remains author-reported. The author script's identity and exclusive-create/no-overwrite execution were not independently observed. Current inventory/digests establish the bounded result, not every historical filesystem action.
3. **Medium — reviewer process deviation, no payload impact.** A later read-only inventory comprehension evaluated `Path.is_file()` before `is_symlink()`, attempting symlink-target metadata checks, including the documented outside-root decoy. This did not open target contents, hash targets, traverse their directories or modify anything. It exceeded the intended no-follow inspection method and is disclosed rather than claimed compliant. The initial `lstat` inventory and manifest safety checks independently establish that no symlink target is included in checksum coverage. This limits the reviewer-process claim, without changing the receipt-integrity result.

No blocking receipt change requested for this bounded acceptance. Earlier limits remain: no initial-parent/full non-DOTNET environment equality proof, global no-leak guarantee, actual binary rehash in the author's worktree, fresh P0 source recomputation, replay of CLI/tests/negative probes, general .NET/JIT equivalence, coverage of untraced calls/unrelated processes or same-user race safety. Analyzer limitations explicitly include untraced syscalls/pre-opened FDs, non-descendants, lexical intermediate-path resolution and per-thread `chdir` tracking. All native evidence file-content reads stayed within the four allowed roots; the target-metadata deviation above is the exception to the no-follow method. Repository/GitHub context was read separately. No author-worktree, protected P0/P1, trust catalog or board write occurred.

## Commands and files checked

Preflight used `pwd`, `uname -a`, `git status --short`, `git branch --show-current`, `git remote -v`, `git rev-parse origin/main`, `git rev-parse HEAD`, and `git ls-remote --heads origin review/multi-p1-656-native-receipts-20261116-v1`. Main, all three packet branches and `feature/multi-p1-639-f0-30m-foundation-20261116-v1` were fetched. Packets and live board were read with `git show <ref>:<path>`. `git log` checked later board changes. Issue bodies/comments used `gh api repos/jermur788/CCF/issues/<n>` and `.../comments` for #656/#658/#640; direct `issues/comments/6931891738`, `6932218665`, `6932260531` were also read. RUNNING used `gh issue comment 656 --repo jermur788/CCF --body-file /tmp/ccf-656-resumed-running.md`.

Exact checksum commands, with working directory set to the corresponding root:

```text
# In each of the four R/<root>/ directories:
sha256sum --strict -c SHA256SUMS-CORRECTED-20261116
# In R/CCF-639-P1-F0-20261116/:
sha256sum -c isolation-run-1.SHA256SUMS
# In each #644 / #649 / #651 root:
sha256sum -c SHA256SUMS
```

Only checksum verification executed inside evidence roots. Each corrected manifest was checked alongside its historical comparator, then separately to capture its own exit code (all 0; historical all 1). Other native checks used **`python3 -I -B - <<'PY'`** inline read-only programs from the reviewer worktree: `os.walk(..., followlinks=False)`, `lstat`/`readlink`, manifest regex `([0-9a-f]{64}) ([ *])(.+)`, safe root-relative regular-file checks, inventory set/duplicate/sort comparisons, `hashlib.sha256`, note reads and JSON parsing. #639 historical names were interpreted under `isolation-run-1/` for digest comparison, without modifying the manifest or payloads. Relevant raw `openat` records were selected by quoted `/proc/.../comm` path, joined by PID if unfinished/resumed, and counted only with `O_WRONLY`/`O_RDWR` and successful FD return. No evidence harness/script was executed.

Directly parsed native files: eight sidecars, four historical manifests, both #651 `A-default/iso/L4-analysis.json` and `B-verification-switches/iso/L4-analysis.json`, their `exit-code.txt` and `iso/L4-strace.txt` (narrow `comm` count only). Checksum commands additionally read every manifest payload entry. No broad raw-trace accounting or negative-probe replay repeated.

Exact erratum checks (P is `Docs/Handoffs/MultiRack-Simulation-20261116/Multi-Sitka-P1-639-F0-THIRTY-METRE-FOUNDATION-IMPLEMENTATION-RESULT-20261116.md`):

```text
git diff --stat 0cf9fea7dc4c4056b938929e455c7bf766d14957 72efdedc8673186a197756b0fe905157ecae9ae7
git diff --numstat 0cf9fea7dc4c4056b938929e455c7bf766d14957 72efdedc8673186a197756b0fe905157ecae9ae7
git diff --unified=5 0cf9fea7dc4c4056b938929e455c7bf766d14957 72efdedc8673186a197756b0fe905157ecae9ae7 -- <P>
```

Read-only Python compared both `git show <SHA>:<P>` byte strings. An initial whole-file-prefix hypothesis was false because of the introductory pointer; inspecting the diff and removing only the two additions proved full original-text preservation, including exact original §10 bytes. This was a document comparison, not a payload-checksum failure.

## Handoff boundary

**G2/G3 scoped decision: APPROVED** for the evidence addenda and #651 erratum only. No integration performed. The review branch must differ from its pinned start by **this one new Markdown only**; publication rechecks live gate C and commit/readback. Manager #641 retains the separate acceptance/status-promotion decision; this review neither edits nor authorises promotion of trust records or execution of the first ecological year. Default L4 **FAIL 38** stays separate from switched-only recorded **PASS 0**.

Dashboard ingestion/published state, Drive mirrors and physical Ops synchronization **NOT VERIFIED**; reviewer has no board write authority. The #656 handoff and #641 notification request Manager coordination/synchronization follow-up. No generated dashboard state edited. No dotnet, strace, build, tests, Unity, ecological year, clean/stash/reset/kill or asset deletion run.
