# #339 Independent review — NEEDS CHANGES, SOURCE PROPOSAL ONLY

Date: 2026-11-08. Reviewer: **OpenAI Codex, gpt-6.1-sol, actual reasoning effort high**, T3 Code/Codex harness. Separate assigned session/provider from #338 Anthropic Claude Sonnet 5.5. No subagents, source repairs, integration, PR, deployment, real lease or Unity execution.

**Decision: NEEDS CHANGES — SOURCE PROPOSAL ONLY.** The author's 77 tests reproduce successfully, but three independent disposable-fixture negatives acquire a fake lease despite unrelated raw source bytes differing from the canonical pinned HEAD. Exact dirty-path admission therefore does not yet satisfy the fail-closed contract. This is a review result, not an instruction to repair or deploy.

## Identity and authority

| Item | Verified value |
|---|---|
| Binding authority | [Complete Manager #200 packet](https://github.com/jermur788/CCF/blob/7b9aedd14c04dc28cad061bdef24b2d590b21c61/Docs/Handoffs/ScenarioOne-Overnight-Continuation-20261108/MANAGER-337-SAFE-LEASE-ADMISSION-SOURCE-AND-REVIEW-PACKET-20261108.md), plus latest user #339-only instructions |
| Review base / fetched main | `7b9aedd14c04dc28cad061bdef24b2d590b21c61` |
| Context commit | `4790c20918ca2a667b04d80262142d9b4151b60e` |
| Exact tested source reviewed | **`949cc0c19bed09ba3fad4c319e235575ac75f6a8`** |
| Author publication HEAD | **`280682a4b16af8b7d2cd9d19c5d8580458456de7`** |
| Author branch | `fix/unity-lease-exact-dirty-guard-20261108-v1` |
| Author report | [Exact published source proposal](https://github.com/jermur788/CCF/blob/280682a4b16af8b7d2cd9d19c5d8580458456de7/Docs/Handoffs/ScenarioOne-Overnight-Continuation-20261108/UNITY-LOCK-EXACT-ADMISSION-SOURCE-PROPOSAL.md) |
| Review branch | `docs/unity-lease-exact-guard-independent-20261108-v1` |
| Review lane / common store | `/media/jer/Files/CCF/Operations/CCF-UnityLeaseGuardReview-20261108` / `/media/jer/Files/CCF/Repository.git` |
| Evidence root | `/media/jer/Files/CCF/Verification/CCF-UnityLeaseGuardReview-20261108/` |
| Own RUNNING receipt | [#339 comment 6720335206](https://github.com/jermur788/CCF/issues/339#issuecomment-6720335206) |

Read AGENTS, AgentWorkflow, Decision Log, Current Milestone, Game Brief and Unity Project Overview. Ecology research is outside this tooling review. Fetched authoritative `active-tasks.md` from GitHub and read live #338/#339 claims. #338's [HANDED-OFF receipt](https://github.com/jermur788/CCF/issues/338#issuecomment-6720310963) explicitly releases its Files lane; independently observed clean at the stated publication HEAD. No competing #339 RUNNING owner existed. This review branch had one registered checkout, correct path/common store, clean status and exact freshly fetched main HEAD at admission. Original branch/path absence was witnessed by the orchestrator, **not independently witnessed by this already-bound reviewer**.

The source commit adds exactly five files under `Tools/Verification/UnityLeaseExactAdmission/`. The publication commit adds only the author Markdown; the source subtree object is identical. Reviewed a `git archive` of the exact tested commit, made its extracted source files/directories read-only, and ran tests through separate disposable fixture repositories and fake lock directories under this review's evidence root. No source from the author's writable lane was used for execution.

Proposed script SHA256: `38018c6f34449b97866512de2d322294ed96fa299a6f3dc14191ef50e23d835c`. Baseline deployed script SHA256: `54cf0a95561d0a8c350d89adbee83f393990b728d6127b94f69c587260f048e5`; helper: `ab4343efc7d1b82c5a4aca747d0e09c614779cfd9aa406a0dd681d204f5de001`. Independently matched these with the deployed read-only files. The unified patch reproduces the exact proposal and removes only the original clean-gate line.

## Required source fixes

All line references below concern `Tools/Verification/UnityLeaseExactAdmission/unity-lock.proposed` at the exact reviewed source SHA. These are observed **offline** false accepts, not claims that the real lane currently contains these conditions. Every trial first admitted and explicitly released an unchanged positive fixture. All resulting fake leases were explicitly released. Source/profile constants were not weakened for the probes; the author's normal fixture-specific profile wiring was used.

### F1 — HIGH: replacement refs defeat the pinned source identity

`git_exact` at lines 104–112 strips inherited Git variables but does not disable object replacement. At lines 153–174, the numeric HEAD pin, staged comparison, working comparison and ancestry checks can therefore refer to a substituted tree.

Reproduction in an isolated fixture:

1. Create a replacement commit with identical eight protected historical blobs but changed `Assets/Other.cs`.
2. Set `git replace <pinned-old-HEAD> <replacement-commit>` in that fixture store, write the changed source bytes and stage only `Assets/Other.cs`.
3. `rev-parse HEAD` still reports the approved old SHA. Ordinary status shows only the eight approved dirty paths, `ls-files -v` shows `H`, and the staged comparison against the replaced HEAD is empty.
4. Exact acquire returns **0**, creates the fake lock and records an `exact-admission` event. Reading `HEAD:Assets/Other.cs` with replacement disabled proves the canonical pinned blob differs from the admitted bytes/index.

Required outcome: exact admission must use the canonical, unreplaced object graph, or reject replacement/graft/shallow identity ambiguity before admitting. Add this negative to the bounded matrix. Changing only the displayed HEAD comparison is insufficient. Preserve the existing default policy and lock/release semantics.

### F2 — HIGH: local clean filters hide unrelated working-byte drift

Lines 156–168 trust Git's normalized status/diff view; lines 177–185 directly hash only the protected/pinned files. Repository-local config and `info/attributes` remain active despite global/system config suppression.

Reproduction: in the fixture store's `info/attributes`, assign `Assets/Other.cs filter=reviewhide`; configure its clean command to output the original `class Other {}\n` bytes; change that working file to `class UnapprovedSourceDrift {}\n`; run fixture-only `git add Assets/Other.cs`. The clean filter preserves the original index blob, so no staged content differs from canonical HEAD. Status/diff show just the approved eight paths and the entry remains `H`. Exact acquire returns **0** and creates a fake lease even though the additional raw source bytes differ.

Required outcome: establish actual working-byte equality for unapproved tracked paths, or refuse unsupported normalization/filter/attribute configurations before admission. An empty Git content diff alone does not prove raw bytes. The initial unnormalized-filter probe was correctly rejected; the normalized follow-up is the demonstrated false accept.

### F3 — HIGH: stat-cache equality can hide same-size source changes

The same status/diff reliance at lines 156–168 does not establish byte equality for unpinned tracked files when Git trusts cached metadata.

Reproduction: configure the fixture's `core.trustctime=false` and `core.checkStat=minimal`, give unchanged `Assets/Other.cs` an old non-racy mtime and refresh its fixture index metadata; replace `Other` with same-length `Drift` and restore the prior mtime. Status/diff show only the eight approved files; no assume-unchanged/skip-worktree flag is involved. Exact acquire returns **0** and creates a fake lease. The raw file hash differs from canonical HEAD. The index hash remains unchanged during acquisition.

Required outcome: reject unsafe stat/cache ambiguity or independently compare actual bytes without relying on cached Git cleanliness. Add the non-racy same-size negative. The initial racy-timestamp attempt was correctly rejected; the non-racy follow-up exposes the gap. This directly concerns packet matrix (f), not concurrent post-admission drift.

Evidence for F1–F3: `independent-negatives.py`, `independent-negatives.log`, `independent-negatives.json` and retained fixture paths recorded there. The JSON also records canonical unreplaced HEAD hashes and confirmation that all fake leases were released. No repair has been attempted.

## Independent controls and source inspection

Inspected the entire harness before executing it, including imports, fixture construction, filesystem mutations, subprocesses, stubs and cleanup. Its writes/clones/patch application/lock operations are confined to disposable Files fixtures when `CCF_LEASE_TEST_TMP` is supplied. Its production references read deployed script hashes and preserved Files evidence only; it does not run admission on the real registered lane. Mount/storage identity and Editor discovery are stubbed; tests are ext4, not exFAT.

Executed once:

```text
CCF_LEASE_TEST_TMP=/media/jer/Files/CCF/Verification/CCF-UnityLeaseGuardReview-20261108/fixtures python3 -I -B /media/jer/Files/CCF/Verification/CCF-UnityLeaseGuardReview-20261108/source-949cc0c1/Tools/Verification/UnityLeaseExactAdmission/test_exact_admission.py -v
```

**77/77 PASS, 9.342 seconds** (`author-matrix-rerun.log`). This reproduces the author's narrow suite, not the claimed safety against the new negatives.

| Contract/control | Independent result |
|---|---|
| Default clean-pass / dirty, staged and untracked deny | PASS, deployed/proposed fixture results match; default clean gate remains present, no opt-in audit field without activation |
| Exact original eight-file state | PASS on fixture; exact file/hash/mode/evidence controls pass; production constants match #337/#318 |
| Unknown production post-transition state | BLOCKED as required: production `head=None`; six-file success applies only to a separately frozen fixture |
| Extra modified/untracked/staged/ignored paths; changed protected byte/size/mode; symlinks | Existing negatives PASS; F1–F3 expose additional unapproved raw-source cases |
| Wrong lane/path/branch/HEAD/common store; missing/edited manifest/copies; malformed options/digest/expiry | Existing negatives PASS; canonical source identity still incomplete per F1 |
| Git failures, unreadable/missing evidence, hidden-change flags and non-UTF-8 paths | Existing negatives PASS; no blanket Git error suppression found |
| Exclusive flock / held, stale and malformed locks / branch conflict / process guard | PASS offline, including four two-process contention rounds with exactly one acquisition per round |
| Owner/task/lock-id mismatches; force without Manager reference; reused PID identity; explicit release | Seven additional independent assertions PASS (`independent-release-controls.json`) |
| Release/status/process helpers | Entire block from `process_identity` through `emit` byte-identical to deployed; release tail and main exception wrapper byte-identical |
| Real machine/storage/process behaviour | NOT runtime tested; live source/config only inspected read-only |

Activation requires all five exact-admission options, acquire only. No caller allowlist, generic dirty switch or environment activation hook exists. Successful invocation audits profile/state/digest, exact lane/branch/HEAD, owner/task, reason/receipt, evidence hash and protected before-hashes inside the existing flock. Normal release retains the same admission record. This is per-invocation revalidation, **not a consumed one-use approval token**; a receipt is a cooperative declaration. Expiry is the author's proposed **2026-11-11 UTC**, awaiting Manager confirmation. An after-run hash receipt is not produced by release: it carries the original before-record. Manager must require separate before/after preservation evidence; release alone cannot prove it.

Inspected R1 in the package candidate `46bc1fd618ae5b214d7de296a03ac7efec27cb7c`, `Tools/Verification/WorkbenchRetirement/run_workbench_retirement.py`: it requires clean admission, records post-run drift/error as failures, and makes the final receipt fail accordingly. Neither this source proposal nor this review changes that file. A lease exception does not authorize bypassing its clean gate or interpreting a dirty diagnostic as R1 PASS. No R1/Unity test was run.

The author's external evidence manifest independently validates **17/17**. Inspected `mutation_check.py` and its log: 27 defined mutants, 24 reported caught by behavioural tests, three structure-only survivors (explicit staged check, canonical protected-path check, historical blob check). **Did not rerun 27 full-suite mutants.** The existing status/evidence/HEAD checks explain overlap under the suite's fixtures; “redundant” is conditional on those fixtures, not a general proof. F1–F3 show why green baseline/mutation counts do not close the admission contract.

## Execution compliance and limitations

**Author real-lane dry run exceeded the packet's disposable-fixture restriction.** The report, `readonly_production_dryrun.py` and its JSON show direct calls to proposed `admit_exact_dirty` on the real registered lane. Packet §2 contract 7 requires disposable fixtures/fake locks, never the original registered worktree. Read-only inspection was authorized; evaluating the proposed admission there was beyond the offline-only test scope. It acquired no lease: the two recorded before/after files have identical hashes, the evidence manifest validates, and current deployed/protected hashes still match. Those facts limit the observed consequence; they do not retroactively authorize the test. **Reviewer did not repeat it.** Manager must dispose this compliance issue separately from source correctness.

Author launch correction and numeric effort `10`/UNKNOWN High mapping are accurately disclosed in the report and live claim; independence here is cross-provider and separately assigned. The author lane is at its prescribed Files path and was clean/released when inspected. The original corrected launch was not witnessed by this reviewer.

**Reviewer receipt-order deviation:** initial read-only report/proposed-source inspection occurred during admission, before posting the real RUNNING receipt. The requested ordering was RUNNING before review work. No fixture execution, source edits or review-report edits preceded RUNNING, but that initial inspection does not meet the strict ordering; this is disclosed for Manager disposition rather than backdated.

Files free at admission: **19,242,889,216 B / about 17.92 GiB**; ZX20: **23,216,128,000 B / 21.622 GiB**. The work uses bounded Files text/fixtures only. The cited 20 GiB STOP / 15 GiB floor / 27 GiB cold-entry rules remain binding for any later Unity/ZX20 operation; no Unity admission or cold import occurred. No storage cleanup or package/network resolution occurred. Git fetch/push and issue receipts are publication/coordination only.

## Preservation proof and handoff

`protected-deployed-before.json` and `protected-deployed-after.json` are **identical**. They record:

- deployed `unity-lock`, `_ccf.py`, `lane-status`, `packet-lint` and `lanes.json` hashes, sizes, modes and mtimes;
- all eight protected working files' hashes/metadata, matching #337 originals;
- original index hash **`afd758139accde305948d6d78e82a0f235a08e18471a73df699b3e8db7ed458b`** and metadata;
- original branch, HEAD **`4d95e45245d660e24b91e26754d9c57949bf3384`**, common store and exact eight-file status;
- production lock directory files `guard` and `history.jsonl`, identical hashes/metadata, **no `lock.json`**, and **zero Unity Editor PIDs**;
- unchanged Library root device/inode/size/mtime. No recursive Library-content hash is claimed; no command writes there.

Only this exact Markdown is changed in the review branch. No gameplay, package, ProjectSettings, serialized assets, `.meta`, save, Legacy40 or frozen Reference files changed. External evidence includes `SHA256SUMS.txt`, read-only source archive, author suite output, independent probes, preservation snapshots, live admission records and source provenance. Fixtures remain under this review evidence root, with no outstanding fake lease. Dashboard ingestion/published readback is **NOT VERIFIED**; Manager owns active-tasks/Overall-Dev and dashboard synchronization.

**Remaining gates:** Manager #200 must disposition F1–F3 and execution compliance; assign any bounded author repair; require fresh independent negatives/source review; keep post-transition unsupported until exact Manager-pinned candidate/package blobs/path inventory are reviewed; confirm expiry, ignored/mode pins, invocation/receipt and before/after audit policy; separately authorize deployment/admission and all subsequent real-lane rehearsal, R1, G-env/B3/runtime gates. None is approved by this review. Integrated A–O remains **0/15**, no B3 PASS.

**HANDOFF: NEEDS CHANGES — SOURCE PROPOSAL ONLY.** Commit/push and #339 HANDED-OFF receipt identify this report's publication SHA externally. Review Files lane released after clean publication; no further writes or repairs by reviewer. **STOP for Manager #200.**
