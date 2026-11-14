# MULTI-005 — Independent focused delta review of Sitka Lab P0 security hardening

**Date:** 14 November 2026  
**Claim:** [CCF #504](https://github.com/jermur788/CCF/issues/504), **READY / NOT RUNNING**; no reviewer, host or worktree is yet registered.  
**Difficulty:** **HIGH, narrowly scoped**. The changed code is small, but correctness hinges on Linux file descriptors, symlink/regular-file races, atomic writes, HTTP error mapping, and adversarial test coverage.  
**Best-suited independent reviewer:** **Claude Opus 5.5 in local T3 Code / Claude Code** or a separate **Sol/Codex High** with direct local tests. **Not the MULTI-004 Claude Sonnet 5.5 author.**  
**Purpose:** Determine whether the F-1…F-5 repairs genuinely close the independent MULTI-003 findings without regressing the already approved P0 engine. This is **NOT** another full P0/Unity/release review. Do not change the reviewed source.  
**Final verdict:** **APPROVED / NEEDS CHANGES / REJECTED**, explicitly **for the MULTI-004 engineering P0 hardening delta only**.

## 1. Pinned authority and exact candidate

**Previously accepted P0 engineering:** [MULTI-003 independent review verdict](https://github.com/jermur788/CCF/blob/review/multi-003-sitka-2d-lab-p0-independent-20261114-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-003-SITKA-2D-LAB-P0-INDEPENDENT-REVIEW-VERDICT-20261114.md). The reviewer independently ran clean build, Core **55/55**, Host **8/8**, Chromium **15/15**, original mutation **17/17**, and timed a 2,000-tree interaction at **157 ms maximum**. There were six MINOR findings, zero CRITICAL or MAJOR. The Manager **accepted the independent engineering P0 qualification**, without merging, P1, Unity, Irish field claims or user acceptance.

**MULTI-004 author task:** [#502, closed HANDED-OFF](https://github.com/jermur788/CCF/issues/502), with [complete source/test handoff](https://github.com/jermur788/CCF/blob/feature/multi-004-sitka-lab-p0-file-safety-hardening-20261114-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-004-SITKA-LAB-P0-HARDENING-IMPLEMENTATION-HANDOFF-20261114.md). The repair worker was Claude Sonnet 5.5 on `jer-laptop` in a fresh worktree, registered before edits; the reader must distinguish **author claims from independently reproduced evidence**.

**Pinned source:** `feature/multi-004-sitka-lab-p0-file-safety-hardening-20261114-v1`, **final handoff HEAD** `9ba88b6e3fff3d2ebff10fc88797bf73c0ebc5cc`; **author-tested source** `e9b3b73095139894a3502ce89111937983572dd1` followed by one Markdown handoff commit. **Baseline:** independently qualified MULTI-003 candidate `f33641f85ec8146ae26b91e4d5ea0b89bfba40f0`.

The Manager independently retrieved the full source handoff and compared GitHub commits: **exactly 10 changed paths**, **nine under `Tools/MultiRackSitkaLab/**`** (seven modified, two newly added) and the one new implementation handoff Markdown. No changes to approved spec, fixtures, schema, UI, root configuration, main, Unity, coordination, production saves or Scenario One.

The **independent review branch**, created at the precise final handoff HEAD and initially untouched:

`review/multi-005-sitka-lab-p0-hardening-delta-20261114-v1`

The **sole allowed tracked result file**, not yet created:

`Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-005-SITKA-LAB-P0-HARDENING-INDEPENDENT-DELTA-REVIEW-20261114.md`

The reviewer must not commit fixes or new source tests on this review branch. For exploratory mutant or attack probes, use **disposable off-repo scratch copies**; do not alter even temporarily another worker's worktree.

## 2. Worker/host/coordination preflight — mandatory

**Proposed separate worktree only; not created or reserved:**
`/media/jer/Files/CCF/Operations/CCF-MULTI-005-SitkaLab-DeltaReview-20261114`

**Proposed evidence directory only; not created:**
`/media/jer/Files/CCF/Verification/MULTI-005-SitkaLab-DeltaReview-20261114/`

Before local tests or writing the review report:

1. Fresh-read this full packet, [#504](https://github.com/jermur788/CCF/issues/504), source [#502](https://github.com/jermur788/CCF/issues/502), prior [#497](https://github.com/jermur788/CCF/issues/497), reviewed branch HEAD and the **live** `Docs/Project/coordination/active-tasks.md` (main), not a context snapshot. Check existing branch ownership and `git worktree list --porcelain`; check physical host and any shared processes or Unity Editor/OS locks. Previous workers reported no MULTI board row, but **absence of a row is not automatic ownership authority**.
2. Ensure that your worktree and evidence folders are **distinct and unused**; never reset, clean, stash, discard, absorb or modify unrelated dirty work or prior Sonnet/Opus worktrees. Do not kill another worker's processes or bypass an Editor machine lease. This task does **not** need Unity. If host/toolchain/physical ownership or board control conflicts, mark #504 **[BLOCKED]**, do not run further.
3. With safe preflight, register the **actual provider/model, host, worktree, review branch, full starting HEAD, source read-only scope, allowed phase** on #504 as **[RUNNING] before source inspection/tests or writing**. The claim tracks presence only; source/host ownership remains subject to the authorised live owner. A GitHub-only reviewer may do a limited static audit (worktree NONE), but **must not claim independent runtime PASS**.
4. At handoff stop your host processes, keep external evidence separately, and verify the final source candidate has not changed. Do not conflate an unchanged GitHub branch with physical cleanup of another person's worktree.

## 3. Focused review matrix: F-1…F-5 and residual risks

| Finding | Exact review requirement | Severity handling |
|---|---|---|
| **F-1: FIFO host hang** | Inspect `NoFollowFiles.cs`, `LabDataStore.cs`, `LabHost.cs`. Put an actual FIFO with a valid lab filename directly in a disposable `.lab-data`; ensure it is neither listed nor opened, load refuses **promptly**, and concurrent `GET /api/state` stays responsive. Confirm `O_NONBLOCK` plus descriptor type check prevents a race-time FIFO hang. | A proven hang in normal supported Linux conditions is a **real defect**, not merely a test issue. |
| **F-2: symlink/open race** | Check the `openat` relative-to-directory-fd `O_NOFOLLOW` path; `statx` on the opened descriptor; regular file type, `nlink > 1`, size-limit enforcement and clean fd closure. Attack after the final pre-check with symlink→outside **valid lab JSON**, FIFO, directory, hard link, symlinked root and aggressive atomic swaps. Confirm **no outside bytes opened/loaded**, not merely a rejected HTTP response after outside bytes were read. Examine real Linux permissions and `statx` layout/ABI assumptions. | An unblocked out-of-root read or FIFO stall may be **MAJOR or blocking** even if a same-user local actor is required. Do not inherit F-2's prior minor severity automatically. |
| **F-3: falsely named test** | Verify the old misnamed test was renamed and new test hooks really fire **after both listing and per-entry classification, immediately before open**; disable the authoritative guard in scratch to prove the test fails. Check M23 and new hook mechanics. | Test must exercise the danger window it claims, not only root classification. |
| **F-4: layered-guard regression coverage** | Verify fresh separate tests for X01/X06/X08/X16/X22 (now M18–M22), including hash-consistent forged history and validator cases, plus old M17. Do a full **30 mutation-control** run in an isolated scratch copy if practical. Confirm the original 17 remain effective and 13 new controls are caught; don't manufacture tests for unreachable X12/X20. Confirm no production guards were removed or weakened. | Disclose surviving mutants; distinguish genuinely independent inner-guard checks from outer guard masking. |
| **F-5: atomic save/error handling** | Check a squatted folder, FIFO and symlink; failed `File.Move`/permission case; removal of only the task's own temp file, no decoy/adjacent-file loss; HTTP 409/500 machine-readable responses and 405/413/415 error-code alignment. Check that errors do not leak physical paths or stack traces. | Data overwrite or orphaned temp file is a source finding, not a browser limitation. |
| **F-6: fixture wording** | Confirm unchanged approved spec and unchanged three fixtures. Only 3-tree oracle manually audited; 162 and 2,000 stems formula-generated, pinned and independently recomputed. The Manager has not authorised editing the APPROVED spec in this task. | Document accepted documentary limitation, **not** failure to model biological growth. |

**Additional required deliberate risk assessment** (not blanket scope expansion):

- **Save-directory swap during *writes***: the worker admits writes remain path-based; another same-user process can swap the lab directory during a save. Determine whether this can violate the **approved lab-only persistence boundary**, write outside the root, corrupt/overwrite a protected file, or merely affect same-user-controlled temporary files. If the actual write isolation guarantee is false, assign severity and **NEEDS CHANGES** where justified; the packet does **not** waive that risk. Do not assume F-2's read protection automatically protects writes.
- **Size growth after descriptor `statx`**: the second read-loop limit has no independently effective test. Use a controlled descriptor/file growth probe if practical, or identify this as an explicit evidence limitation. Do not deliberately hang the shared host or exhaust disk.
- **Platform restriction**: Native Windows/macOS (and unsupported Linux architectures) now **fail closed with HTTP 501 for lab file access**; Linux arm64 is claimed supported by headers/self-test but was **not run**. Judge engineering correctness on **Linux x64 actually tested**, make arm64 conditional/not verified, and flag any material **product portability decision** to the Manager. A Linux-only working lab does **not** establish cross-platform P0. Do not require a Windows game/Unity build for this review.
- **Distinguish untested from failed**: headed browser, Firefox, WSL, macOS and P1 are NOT TESTED, not presumed failures. Synthetic fixed fixtures are never measured Irish inventory. Do not perform field-data research or interpret this as forestry accuracy qualification.

## 4. Bounded independent qualification

**Tier 1:** On a clean review checkout, run the author-noted commands after verifying local toolchain:

```bash
cd Tools/MultiRackSitkaLab
dotnet build MultiRackSitkaLab.sln --no-incremental
dotnet run --no-build --project tests/Core.Tests
dotnet run --no-build --project tests/Host.Tests
sha256sum -c fixtures/synthetic/MANIFEST.sha256
python3 -I -B tests/check-schema.py fixtures/synthetic/*.sitkalab.json
```

Expected **writer-side** values: build 0 warnings/errors, Core **77/77**, Host **12/12**, original mutation **17/17** and expanded **30/30**. Record actual independently executed values and commands separately; don't retroactively call worker evidence REPRODUCED.

**Mutation controls:** run `python3 -I -B tests/run-mutation-controls.py <scratch-copy> <results-json>` from a disposable **copy** (the script edits source within its scratch); inspect the current script's exact invocation and existing mutation patterns before running. Supplement with bounded reviewer-owned attack probes, particularly the write-root swap and read growth tests above. Never mutate source on the reviewer branch.

**Tier 2 targeted:** one real local loopback host/API adversarial smoke, including FIFO load + concurrent normal GET and symlink/valid-outside-content race; a single headless Chromium UI run is sufficient to guard save/load and UI state regression. The full previously reviewed P0 test matrix need not be reconstructed. A headed or second browser is useful but **not mandatory**. Check the off-repo MULTI-004 proof directory **read-only if it physically exists**:
`/media/jer/Files/CCF/Verification/MULTI-004-SitkaLab-Hardening-20261114/`
Verify its SHA-256 manifest (37 files) if available; if missing, state not accessible instead of inventing a pass.

**Performance:** if browser suite runs, record measured 2,000-tree action time; otherwise the author's max **162 ms** remains writer-claimed only. Test target ≤2 s on a documented host, not a release-wide universal guarantee.

**No Tier 3 Unity, Windows game build, P1 biological modelling, real GBIF/marteloscope import, production saves, player release or physical CCF forest data.** No new third-party dependencies without separately recorded approval.

## 5. Expected review result and closure

Publish **exactly one new Markdown** on the dedicated review branch:

`Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-005-SITKA-LAB-P0-HARDENING-INDEPENDENT-DELTA-REVIEW-20261114.md`

It must include:

1. Actual registered worker/host/branch/worktree/starting HEAD, board and physical owner preflight; source diff and candidate unchanged attestation. Review branch file diff **exactly one added report**.
2. **F-1…F-5 explicit PASS/FAIL/PARTIAL** with path/line citations, observed negative proof and an assessment of F-6's separate accepted wording discrepancy.
3. **Security verdict on residual path-based *writes*** under root-folder swaps, read-size-growth bound and Linux-only fail-closed/arm64 not tested. Classify threat model and any **material portability decision** without misrepresenting supported platforms.
4. Tests actually executed with logs/hashes/screenshots or off-repo evidence directory, compared separately against author claims; original 17 and expanded 30 mutation controls, isolated probe methodology.
5. Engineering P0 hardening **APPROVED / NEEDS CHANGES / REJECTED** with defects classified CRITICAL / MAJOR / MINOR. APPROVED means **only that the bounded repair passes** on the tested target, not integrated, Irish forestry validated, P1 approved, cross-platform, Unity qualified or user-accepted.
6. Actionable next step: if APPROVED, Manager may mark MULTI-004 P0 security-hardening independently qualified, then separately decide cross-platform/later spec follow-up. If NEEDS CHANGES, Manager issues a **narrow repair** for verified findings followed by a **focused** recheck, not a new full review. If REJECTED, explain why and preserve all work, no destructive cleanup.
7. Fetch/read back published report from GitHub and compare against registered branch starting HEAD, **one new Markdown only**, no source changes. Mark [#504](https://github.com/jermur788/CCF/issues/504) **[HANDED-OFF]**, close with report URL and notify #502/Manager. Dashboard/Drive mirror **NOT VERIFIED** absent evidence.

**Prohibited:** production fixes by reviewer, broad refactor, approved spec edits, live coordination edits without delegation, Unity/Scenario One source, P1, real forestry measurements/credentials, protected save access, Windows game builds, source merges, claims of user acceptance.

## 6. Copy/paste independent reviewer dispatch

> Take ownership of **MULTI-005 / CCF #504 — focused independent Sitka Lab P0 hardening delta review**. Read the complete binding packet at https://github.com/jermur788/CCF/blob/docs/multi-005-sitka-lab-p0-independent-delta-review-packet-20261114-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-005-SITKA-LAB-P0-INDEPENDENT-HARDENING-DELTA-REVIEW-PACKET-20261114.md and the MULTI-004 source handoff linked there. Use **Claude Opus 5.5 or an independent Sol/Codex High**, **not** the author Sonnet 5.5. Start on created `review/multi-005-sitka-lab-p0-hardening-delta-20261114-v1` from pinned MULTI-004 candidate. Proposed (not reserved) distinct worktree: `/media/jer/Files/CCF/Operations/CCF-MULTI-005-SitkaLab-DeltaReview-20261114`. Fresh-check live coordination/physical host/worktree/source before local tests and register actual worker, provider/model, branch, worktree and starting HEAD as RUNNING on #504; BLOCKED if conflict. Review ONLY F-1 to F-5 repairs and regression, especially FIFO/host responsiveness, handle-relative symlink race and descriptor checks, mutation 30/30, save temp cleanup/status codes. **Explicitly assess residual path-based write-directory swap and Linux-only portability**; do not assume acceptance. Run proportionate independent Tier1 and targeted Tier2, no Unity/full requalification. Publish only `Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-005-SITKA-LAB-P0-HARDENING-INDEPENDENT-DELTA-REVIEW-20261114.md` on the review branch, verdict APPROVED / NEEDS CHANGES / REJECTED for engineering P0 hardening, readback/one-file proof, close #504 HANDED-OFF and return to Manager. No source/spec/coordination edits, protected saves, P1 ecology or Windows build.
