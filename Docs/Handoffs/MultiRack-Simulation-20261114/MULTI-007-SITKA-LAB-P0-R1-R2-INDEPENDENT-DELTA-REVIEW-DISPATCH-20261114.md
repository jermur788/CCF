# MULTI-007 — Independent focused R-1/R-2 recheck of Sitka Lab P0 write-safety repair

**Issued:** 14 November 2026  
**Binding reviewer claim:** [CCF #511](https://github.com/jermur788/CCF/issues/511), **READY / NOT RUNNING**. No reviewer has started, and no physical worktree, host or resource lease has been assigned.  
**Verdict expected:** **APPROVED / NEEDS CHANGES / REJECTED — MULTI-006 engineering P0 repair delta only**.  
**Difficulty:** **MEDIUM–HIGH**, focused native Linux file-handle/TOCTOU security verification and regression validity; not an end-to-end forestry game review.  
**Best-suited independent worker:** **Sol/Codex High in a fresh local T3 Code session** or another independently capable **non-Opus** code reviewer. **Do not use Claude Opus 5.5**, the actual MULTI-006 source author. Avoid any reviewer who authored this patch.  
**User action:** send the paste-ready line in §6 to the independent local reviewer. Do not run another process against the writer's active physical worktree.

## 1. Exact evidence and source authority

**Approved P0 product boundary:** standalone C#/.NET + loopback browser research laboratory, synthetic Sitka fixture, **NO BIOLOGICAL GROWTH**, no Unity, no measured Irish marteloscope or production save. [Approved four-file spec](https://github.com/jermur788/CCF/blob/docs/multi-001-sitka-lab-p0-approved-spec-20261114-v1/specs/multirack-sitka-2d-reference-lab-v1/spec.md). MULTI-003 was independently APPROVED for the **original engineering P0 candidate**, not for later unreviewed edits.

**MULTI-005 independent [NEEDS CHANGES report](https://github.com/jermur788/CCF/blob/review/multi-005-sitka-lab-p0-hardening-delta-20261114-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-005-SITKA-LAB-P0-HARDENING-INDEPENDENT-DELTA-REVIEW-20261114.md):** one **MAJOR R-1** (same-user directory exchange during a path-based save overwrites valid files outside the lab root, leaves temp orphans, misreports outcomes) and one **MINOR R-2** (working post-\`statx\` read-length safeguard has no failing regression test). Earlier FIFO/read-side/guard findings F-1–F-4 independently passed and F-5 remained partial solely because of R-1. The Manager **did not waive R-1**; it required a narrow fix.

**MULTI-006 implementation [#510](https://github.com/jermur788/CCF/issues/510) HANDED-OFF/CLOSED:** [writer's complete GitHub handoff](https://github.com/jermur788/CCF/blob/feature/multi-006-sitka-lab-p0-handle-anchored-writes-20261114-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-006-SITKA-LAB-P0-WRITE-SAFETY-REPAIR-HANDOFF-20261114.md).

- Actual source worker: **Claude Opus 5.5** on \`jer-laptop\`, Linux Mint x64, local T3 Code, registered [RUNNING on #510](https://github.com/jermur788/CCF/issues/510#issuecomment-6883008705). The source worker and independent reviewer must differ.
- **Final candidate HEAD (includes handoff):** \`c445f8700610a7e6a835fef9ecec39eb9432128d\`, branch \`feature/multi-006-sitka-lab-p0-handle-anchored-writes-20261114-v1\`.
- **Source commit writer actually tested:** \`ae05c50a11e1d8cd5145043c3a6341ffd0d9ddb1\`; the next commit adds exactly the writer handoff.
- **Prior MULTI-004 head:** \`9ba88b6e3fff3d2ebff10fc88797bf73c0ebc5cc\`. Manager independently fetched the handoff and verified GitHub comparison: **seven changed paths only** (six lab source/tests/README files modified and exactly one added handoff Markdown). No \`specs/\`, schemas, fixtures, UI, host routing, \`Assets/\`, \`Docs/Project/\`, root ignore, Unity or production save changes.

**Writer-side results only (NOT reviewer confirmation):** clean build zero warnings/errors; Core **82/82**, Host **13/13**, fixture manifest **3/3**, schema pass; **35/35** mutation controls caught (17 originals plus 18 additional); Chromium headless **15/15**, 2,000-tree max **186 ms** vs 2s budget. Writer reran the MULTI-005 independent store-write swap probe: baseline **100 out-of-folder overwrites and 862 orphan temps**, repair **0 overwrites/0 orphan temps**; independent live-host smoke **21/23 baseline vs 23/23 repair twice**. Evidence on laptop Files drive at \`/media/jer/Files/CCF/Verification/MULTI-006-SitkaLab-WriteRepair-20261114/\`; verify its \`MANIFEST.sha256\` (37 entries) **read-only if accessible**. The Manager has **not** run these tests.

## 2. Review branch, physical preflight and no-edit rule

**Created independent review branch (initially unchanged from full candidate HEAD):** \`review/multi-007-sitka-lab-p0-write-safety-delta-20261114-v1\` at \`c445f8700610a7e6a835fef9ecec39eb9432128d\`.

**Proposed, NOT CREATED/RESERVED, separate worktree:** \`/media/jer/Files/CCF/Operations/CCF-MULTI-007-SitkaLab-WriteDeltaReview-20261114\`.

**Proposed, NOT CREATED, off-repo evidence folder:** \`/media/jer/Files/CCF/Verification/MULTI-007-SitkaLab-WriteDeltaReview-20261114/\`.

**Sole allowed tracked output:**
\`Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-007-SITKA-LAB-P0-R1-R2-INDEPENDENT-DELTA-REVIEW-20261114.md\` added on the review branch. All implementation files are **read-only**. Exploratory attack/mutation scripts may be created in a disposable **off-repo scratch copy** only, never changed in the candidate/reviewer branch.

**Before source inspection, local test or report edit:**
1. Fresh-read [#511](https://github.com/jermur788/CCF/issues/511), this packet, the pinned writer handoff, independent MULTI-005 report, and the **live** \`Docs/Project/coordination/active-tasks.md\` on \`main\`. Check the exact local git branch and full HEAD against the pinned candidate, \`git worktree list --porcelain\`, existing worktree identities/dirty paths, machine host, disk capacity and active \`dotnet\`, Chromium or Unity processes. Past reports of no MULTI board row **do not grant ownership**. Source claim tracks execution presence; active board and actual physical owner govern any collision.
2. Never reuse, reset, clean, stash, absorb or modify the MULTI-002 through MULTI-006 author/review worktrees or any unrelated dirty work. Never kill another owner's process, take an ungranted Unity lease or write live coordination. If the proposed laptop/path is unavailable or ownership conflicts, **mark #511 BLOCKED and stop** rather than quietly choosing a different machine/worktree.
3. When safe, use an isolated review worktree and post actual **model/provider, host, worktree path, review branch, starting full HEAD and read-only source scope as [RUNNING] on #511 BEFORE tests or writing**. If only GitHub/static evidence is available, state worktree NONE and do not claim runtime repro. No source edit/merge permitted for this reviewer.
4. At handoff stop only processes you started; do not remove older \`/tmp\` folders or protected saves. Record what is actually clean and what you could not verify.

## 3. Focused R-1 MAJOR test matrix — actual write isolation

Inspect \`src/Core/NoFollowFiles.cs\` and \`src/Core/LabDataStore.cs\` against MULTI-005's precise failure and the user's approved save-root contract.

- **One trusted FD per save:** \`OpenDirectory\` with no-follow + directory flags retains its FD from the first access until all action/cleanup ends; \`openat(dfd,tmp,O_WRONLY|O_CREAT|O_EXCL|O_NOFOLLOW|O_CLOEXEC,0600)\`, write-all (partial-write, \`EINTR\`, zero-progress), close (including deferred error handling), **\`renameat(dfd,tmp,dfd,final)\`** and, on failure, **\`unlinkat(dfd,tmp,0)\`** use **that same dfd**. Check safe FD closure, error paths, exact accepted filenames, and that no path-based \`FileStream\`, \`File.Move\`, \`File.Delete\` or AT_FDCWD sneaks into the actual transaction. Confirm late symlink/FIFO/folder squat outcomes and errors remain controlled.
- **Deterministic stage-hook attacks:** independently test replacement of \`.lab-data\` with a symlink **after the directory FD opens** and again **between temp write and rename**, plus a deliberately failed rename/cleanup while folder path is swapped. Verify the original trusted directory inode gets the intended save or retains its original contents after failure, the **alternate directory files remain identical**, decoy/unrelated files untouched, and **zero temp orphans** in either location when the test has settled. Check truthful result status: the handler must not claim failure after a write escaped elsewhere.
- **Concurrent exchange attack:** use the *MULTI-005 reviewer-owned* \`renameat2(RENAME_EXCHANGE)\` store-level \`writeswap\` and live HTTP \`smoke.py\` modes from \`/media/jer/Files/CCF/Verification/MULTI-005-SitkaLab-DeltaReview-20261114/\` **read-only if available**; or independently construct equivalent probes in a scratch lab copy. Compare to known baseline counterexamples. Assert **zero out-of-root alternate-folder overwrites, zero leftover temps and no misleading HTTP success/error** under bounded timing. Do not permit attacks to target game saves, Reference Future or a real user's files. The smoke script may leave a \`swaplink\` after running: each attempt needs a **fresh scratch root**, and cleanup only of reviewer-owned artefacts.
- **Regression coverage:** review targeted \`FolderSwappedForALink_…\` tests and new host test; prove they exercise the dangerous interval and can fail if \`openat\`, \`renameat\` or \`unlinkat\` is individually made path-based. Examine **M31–M34**, plus revised M28; explain any surviving or build-broken mutant rather than counting it as caught.
- **Material residual distinction — real directory substitution BEFORE open:** The author notes that a same-user process can replace \`.lab-data\` with a **different actual directory** before \`OpenDirectory\`. A save then correctly acts on the opened substitute directory: this is not the original **symlink-while-saving** exploit. The reviewer must **classify** whether this defeats P0's explicit fixed-root security guarantee sufficiently to merit **NEEDS CHANGES** or is an honest same-user/local-adversary threat limitation acceptable with a documented caveat. Explain difference between inode-anchored save integrity during one transaction and persistent directory identity across separate sessions; do **not** silently claim the latter. Any new persistent inode pinning is a *separate Manager design decision*, not authorised here.
- **Other bounds:** new save permissions \`0600\` (check actual mode/umask), Linux x64 file ABI actually run, Linux arm64 **untested** and native Windows/macOS save/list/load deliberately return **HTTP 501**. Don't claim cross-platform functionality or require a Windows/Unity build.

**R-1 verdict requirement:** PASS / FAIL / PARTIAL and explicit explanation of **what data/location can actually be written under same-user directory exchanges**, not only whether tests report green.

## 4. Focused R-2 MINOR test matrix — file growth after statx

- Inspect \`NoFollowFiles.ReadAll\` second cumulative \`FileTooLarge\` check. The first check uses the descriptor's \`statx\` size; the second must enforce a byte bound if a regular file supplies content despite an earlier reported size 0 or grows after inspection.
- Check the new test reading **\`/proc/<numeric current process PID>/status\`**, not \`/proc/self\` (symlink). It should assert \`statx\` reports size=0, content is more than 10 bytes with a large limit, and max 10 throws \`FileTooLarge\` **rather than passing vacuously**. If procfs is unavailable on a claimed supported Linux x64 test host, record a genuine qualification limitation, not a silent skip.
- Mutant **M35** must remove the second guard, **still compile**, and be **CAUGHT by the new test**; the original first size test must still PASS. If its removal is caught only by another error or compilation, that is not effective coverage.

**R-2 verdict:** PASS / FAIL / PARTIAL and source + actual test receipt.

## 5. Bounded qualification, publication and next decision

**Suggested actual commands**, from verified isolated review worktree \`Tools/MultiRackSitkaLab\` (adjust based on source help and actual installed tools):

\`\`\`bash
dotnet build MultiRackSitkaLab.sln --no-incremental
dotnet run --no-build --project tests/Core.Tests
dotnet run --no-build --project tests/Host.Tests
(cd fixtures/synthetic && sha256sum -c MANIFEST.sha256)
python3 -I -B tests/check-schema.py fixtures/synthetic/*.sitkalab.json
# The mutation runner accepts a scratch-copy root and optional result JSON.
# Run these only on disposable copies, never the review branch:
MUT_ONLY=M28,M31,M32,M33,M34,M35 python3 -I -B tests/run-mutation-controls.py "$SCRATCH/mut" "$EV/mutation-focus.json"
# Full 35 once if practical to detect regression, not a full P0 requalification.
EVIDENCE_DIR="$EV/browser" node tests/Ui/run-browser-tests.mjs
\`\`\`

The focused recheck need not repeat additional 22 independent novel mutants, broader forestry research, full P0 proof, cross-browser/Windows, Unity import, Tier-3 release qualification or player testing. A single bounded Core+Host test run, focused six mutations with optional 35-control regression, safe store/host swap probes and one browser regression are proportionate. Verify candidate SHA remains unchanged at handoff.

**Output report** \`Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-007-SITKA-LAB-P0-R1-R2-INDEPENDENT-DELTA-REVIEW-20261114.md\` must contain:
1. Actual registered independent non-Opus provider/model, host/worktree/starting HEAD and clean candidate/source-unchanged attestation; report GitHub branch compared against pinned candidate **one added Markdown only**, no code/tests/board/Unity/source edits.
2. **R-1 and R-2 each PASS/FAIL/PARTIAL**, evidence and exact code locations; independent test exit codes, commands, timing and post-attack saved-file/orphan counts versus writer's claims.
3. Discussion of real-folder substitution before open (whether separate design decision needed), Linux-only target, platform 501 behaviour by source (unless separately run), temp mode 0600, future portability not approved.
4. Mutation coverage M28/M31–M35; any full 35/35 if run. State **REPRODUCED**, **SOURCE INSPECTED**, **WRITER ONLY** or **NOT TESTED** accurately. Off-repo logs/screenshots/hashes actually accessed, not presumed.
5. Final **APPROVED / NEEDS CHANGES / REJECTED** for **MULTI-006 P0 write-safety repair only**. If APPROVED, Manager may accept that narrow hardening and record a separate decision on persistent folder identity and Linux-only platform boundaries. It is NOT a source merge, Scenario One/Unity integration, field correctness, P1 ecology, Windows game build or user acceptance. If NEEDS CHANGES, list smallest demonstrable repair and one focused review; if REJECTED, preserve work and explain why.
6. Publish report on review branch, fetch/read back from GitHub, verify **exactly one new Markdown** changed, mark [#511](https://github.com/jermur788/CCF/issues/511) **[HANDED-OFF]** and close only after successful publication, notify #510 and Manager. Dashboard/Drive sync **NOT VERIFIED** absent direct proof.

**Do not edit** implementation/spec, live coordination, \`main\`, production saves, Unity/game code, real forestry data, P1 model, archived Reference Future, or execute Windows/Player builds.

## 6. Copy/paste independent reviewer dispatch

> Take ownership of **MULTI-007 / CCF #511 — independent focused Sitka Lab P0 write-safety recheck**. Read the full binding GitHub packet at https://github.com/jermur788/CCF/blob/docs/multi-007-sitka-lab-p0-write-safety-delta-dispatch-20261114-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-007-SITKA-LAB-P0-R1-R2-INDEPENDENT-DELTA-REVIEW-DISPATCH-20261114.md. **Use Sol/Codex High or a genuinely independent non-Opus local reviewer, NOT the Claude Opus 5.5 MULTI-006 implementer.** Pinned candidate is \`feature/multi-006-sitka-lab-p0-handle-anchored-writes-20261114-v1\` at full handoff HEAD documented in the packet; created reviewer branch \`review/multi-007-sitka-lab-p0-write-safety-delta-20261114-v1\` starts exactly there. Proposed separate uncreated worktree \`/media/jer/Files/CCF/Operations/CCF-MULTI-007-SitkaLab-WriteDeltaReview-20261114\`. Fresh-check live board, physical host, process/branch/worktree ownership and register actual provider/host/worktree/start HEAD **RUNNING on #511 before code inspection, tests or writing**; if conflict mark BLOCKED and stop. Independently recheck MULTI-005 **R-1 MAJOR** handle-anchored save \`openat\` temp/write/\`renameat\`/\`unlinkat\` against directory/symlink swapping, outside victim zero overwrite/temp orphan, truthfully reported HTTP results; and **R-2 MINOR** procfs post-statx growth guard with M35. Run focused Tier1, M28/M31–M35 and bounded HTTP/browser/real-folder identity probes in safe scratch only. **Explicitly classify** substitution of a different REAL directory before open as a separate same-user/identity policy question; don't silently equate it with the repaired mid-save symlink race. Return APPROVED / NEEDS CHANGES / REJECTED for the narrow P0 engineering delta, publish ONLY \`Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-007-SITKA-LAB-P0-R1-R2-INDEPENDENT-DELTA-REVIEW-20261114.md\` on the untouched review branch, verify GitHub one-file diff, close #511 HANDED-OFF and notify Manager. No code/spec/coordination writes, Unity, P1, CCF saves, real field data or Windows builds.
