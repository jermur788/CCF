# MULTI-003 — Independent engineering and evidence review of the Sitka 2D Lab P0

**Issued:** 14 November 2026  
**Execution claim:** [CCF #497 — MULTI-003 independent P0 review](https://github.com/jermur788/CCF/issues/497) — **READY / NOT RUNNING**; no reviewer is registered or granted a machine/branch worktree lease.  
**Role:** separate, non-author engineering reviewer; **do not use Claude Sonnet 5.5 from the original MULTI-002 implementation session**.  
**Difficulty:** **HIGH** — 50 new source/test/fixture files, Core action semantics and hash-chained replay, ASP.NET local security, file isolation, browser/Core parity, evidence integrity and a prior mutation-test miss.  
**Best-suited reviewer:** **Claude Opus 5.5 / Claude Code on a safely isolated local T3 Code lane**, or **independent Sol/Codex High** with the same source/runtime access; model/provider selection follows available capability, not a fixed discipline.  
**Review phase:** Tier 1 source/headless + targeted Tier 2 browser only; NOT production implementation, Unity, Windows game build, forestry model validation or user acceptance.  
**Sole permitted tracked output:** \`Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-003-SITKA-2D-LAB-P0-INDEPENDENT-REVIEW-VERDICT-20261114.md\` added on own review branch. **Do not edit candidate code or existing tests** during independent verdict. If a fix is needed, recommend a separately claimed author-repair task.

## 1. Candidate, approval and writer-side evidence

**Pinned exact P0 candidate:** branch \`feature/multi-002-sitka-2d-reference-lab-p0-20261114-v1\`, recorded full handoff HEAD \`f33641f85ec8146ae26b91e4d5ea0b89bfba40f0\`; code-and-tests commit \`2a76260e176023ca19e1ff6b56c1258c63320f20\` before handoff publication. The implementation branch was compared with its approved-spec starting point \`c92af201b6366e7e6314d587b6d4071dee352fa3\`; the recorded GitHub diff is **51 ADDED** files: **50 exclusively under** \`Tools/MultiRackSitkaLab/**\` and **one** required MULTI-002 handoff Markdown. The Manager independently read back the handoff and compared the current source branch's 51 files; this is **scope proof**, not runtime verification or acceptance.

- [Exact MULTI-002 writer implementation handoff](https://github.com/jermur788/CCF/blob/feature/multi-002-sitka-2d-reference-lab-p0-20261114-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-002-SITKA-2D-LAB-P0-IMPLEMENTATION-HANDOFF-20261114.md) and [#494 registration/handoff claim](https://github.com/jermur788/CCF/issues/494). **Read its full §§1–9** for source and off-repo evidence names and caveats.
- [Formal MULTI-001 user approval record](https://github.com/jermur788/CCF/blob/docs/multi-001-sitka-lab-p0-approved-spec-20261114-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-001-SITKA-2D-LAB-P0-USER-APPROVAL-RECORD-20261114.md). The user approved **L1** separate research laboratory, **L2** P0 with **NO BIOLOGICAL GROWTH** (P1 later), **L3** standalone .NET Core + loopback browser UI.
- **Authoritative four-file APPROVED P0 spec:** [spec.md](https://github.com/jermur788/CCF/blob/docs/multi-001-sitka-lab-p0-approved-spec-20261114-v1/specs/multirack-sitka-2d-reference-lab-v1/spec.md), [plan.md](https://github.com/jermur788/CCF/blob/docs/multi-001-sitka-lab-p0-approved-spec-20261114-v1/specs/multirack-sitka-2d-reference-lab-v1/plan.md), [tasks.md](https://github.com/jermur788/CCF/blob/docs/multi-001-sitka-lab-p0-approved-spec-20261114-v1/specs/multirack-sitka-2d-reference-lab-v1/tasks.md), [checklists/requirements.md](https://github.com/jermur788/CCF/blob/docs/multi-001-sitka-lab-p0-approved-spec-20261114-v1/specs/multirack-sitka-2d-reference-lab-v1/checklists/requirements.md). The approval is for engineering P0 **only**.

**Writer-side claims (NOT independent PASS):**
- Local Linux laptop \`jer-laptop\`, .NET 8 SDK 8.0.133/runtime 8.0.33, Chromium 156 headless, Node 26; **owner explicitly approved provider/host/worktree substitution** from Sol/Codex-HP WSL to Claude Sonnet 5.5 on the laptop Files drive before implementation.
- Clean build **zero warnings/errors**, Core **55/55**, Host **8/8**, headless Chromium real-input UI **15/15**, 2,000-tree test max **169ms** vs 2s budget; screenshot/log evidence reportedly under \`/media/jer/Files/CCF/Verification/MULTI-002-SitkaLab-P0-20261114/\`.
- Mutation run 1 **16/17 caught**; survivor **M17 queue-state guard** led to added test assertions and **M17-only** re-run caught. **Do not describe a final 17/17 full matrix as re-run.** Original 16 were not re-run after the test edit.
- 162-tree and 2,000-tree fixtures are deterministic *formula-generated* (not individually hand audited). Only 3-tree quarter-hectare basal-area oracle was hand worked.
- Atomic negatives sometimes use \`internal ForTestingUnvalidated\` corruption seam (unreachable via public interface). Browser was **headless Chromium only**, not headed browser, Firefox/Windows/WSL. The source author stopped and left their physical worktree, which **must not be reused without checking ownership and state**.
- Original #494 preflight found no live \`active-tasks.md\` row and registered no board lock. **Do not treat lack of a row as permission to override any current source/host owner or reuse the worker's prior local checkout.**

**Non-goals:** Irish field accuracy, actual marteloscope 2D reconstruction, GROWFOR growth, ecological competition, Oak/Beech, 3D, Multi-Rack transfer, Scenario One source, production CCF saves, player acceptance, Windows/Unity native qualification. If these are absent, do not mark P0 FAIL simply because they are future work.

## 2. Reviewer lane and physical-host preflight

**Created review branch, initially untouched and exactly forked from candidate handoff HEAD:**
\`review/multi-003-sitka-2d-lab-p0-independent-20261114-v1\`.

**Proposed NOT RESERVED review worktree:** \`/media/jer/Files/CCF/Operations/CCF-MULTI-003-SitkaLab-Review-20261114\` on the laptop, only if that exact local host and free worktree are **independently verified** and authorised. Do **not** edit, reuse or clean the Sonnet writer's \`.../CCF-MULTI-002-SitkaLab-P0-20261114\` worktree. If laptop unavailable, propose another separate local T3 Code lane after checking live ownership.  
**Proposed NOT CREATED off-repo review receipts:** \`/media/jer/Files/CCF/Verification/MULTI-003-SitkaLab-Review-20261114/\` (and, where explicitly available, read the prior MULTI-002 logs/screenshots **read-only**). No raw test logs or screenshots need be committed.

Before review edits / local tests:
1. Fresh-read [#497](https://github.com/jermur788/CCF/issues/497), source [#494](https://github.com/jermur788/CCF/issues/494), **live** [coordination/active-tasks.md](https://github.com/jermur788/CCF/blob/main/Docs/Project/coordination/active-tasks.md), and candidate branch HEAD/issue registrations. Inspect exact local \`git worktree list --porcelain\`, current host, disk, running dotnet/Unity/editor use, external Ops owners, unrelated dirty paths. Issue claims give **execution presence**, not ownership locks. Live board is controlled by the authorised board writer; do not edit it and do not grant yourself overlapping host/source ownership.
2. Ensure distinct isolated review worktree and known exact candidate HEAD. Never reset, stash, discard, clean or absorb dirty work. Never kill another worker's process or bypass Unity machine lease. No Unity needed; if machine access is unsafe, mark #497 **BLOCKED** and stop rather than using the author worktree.
3. Before any new review report write or runtime test, register actual reviewer **model/provider, host, isolated worktree, branch, pinned starting full HEAD, source READ-ONLY ownership, allowed phase** as **[RUNNING]** in a #497 issue comment. If capability is GitHub-only, state worktree **NONE** and never claim local test PASS; a limited static review may return an explicitly qualified verdict but cannot claim source had been freshly executed. No unknown owner or silent platform substitution.
4. **Review/qualification must not edit production candidate.** Run dotnet/Chromium/tests in an isolated review checkout; generated build output and ignored disposable \`.lab-data/\` stay in that checkout and are removed only if reviewer owns them. If any edit to tests or source is needed to investigate defects, use a temporary disposable **off-repo scratch copy** and report it; never commit changes to candidate/reviewer branch.

## 3. Source review: mandatory risk-focused matrix (not a code rewrite)

| Area | Focus | Required receipt or classification |
|---|---|---|
| **A. Spec fidelity and source custody** | Compare 51-file scope and exact approved contract: 2D single authoritative tree state, synthetic tags, no growth/real plots/Unity, saved format and data root, actual Core-to-UI values | File-path diff + source-level statements. AC-01, AC-02, AC-09, AC-10 |
| **B. Plan state / all-or-nothing** | \`Engine.cs\` status guards, uniqueness, reservation, data-dependent valid adults; inspect whether a corrupted or stale one-ID plan can partially mutate before validation; consecutive execution, rejected/cancelled/stale plans; plan queue guard (M17) | Source paths/line ranges and targeted test. AC-05, AC-06 |
| **C. Event chain/replay/epochs** | \`LabSession.cs\`, \`CanonicalJson.cs\`, \`LabFile.cs\`: exactly one proper event per snapshot-changing action; \`PlanStale\` cascades; audit rejection unchanged hashes; eventSeq/prev/post SHA-256; approvals read historical epoch from payload on replay; clean roundtrip vs stale-on-load; tamper-detection | Independent test/analysis and any schema contradiction. AC-03, AC-05–07 |
| **D. Safe lab saves and file IO** | \`LabDataStore.cs\`, \`LabFile.cs\`: validate filenames/traversal, allowed root/symlink TOCTOU and header-before-body parse; error handling, unlisted file, read-before-check risks, CCF-looking JSON under lab root; ensure no CCF or frozen game save accessed | Confirm both pre-open path rejection and post-header content rejection. AC-06/07 |
| **E. Host / browser boundary** | \`LabHost.cs\`, \`Program.cs\`, \`app.js\`: loopback-only binding, Host/Origin/CSRF/CORS/method/body guards, HTTP API canonical state/metrics, DOM trust and UI rendering from Core only, source of all BA/preview numbers | Code check + hostile Host/Origin and browser/Core parity. AC-02, AC-07/08 |
| **F. Geometry/fixtures/stand maths** | \`Geometry.cs\`, \`Metrics.cs\`, \`SnapshotValidator.cs\`, \`FixtureCatalog.cs\`: integer mm, simple polygon/bow-tie and on-edge, below-threshold/dead/felled eligibility, uniqueness, N/BA/Dq units, overflow bounds, 3-tree oracle, formula-pattern fixtures and manifest | Hand-recompute small oracle; don't claim 162/2,000 individually hand-measured. AC-03/04 |
| **G. Verification strength and limits** | Examine tests, schema checker, UI driver and mutation controls. Pay special attention to prior **M17 miss**, hidden \`ForTestingUnvalidated\` test seam, GUI actual click coverage (dropdown programmatic), runtime pin, artifact evidence access and test result integrity | Explicit **WRITER CLAIMED / REPRODUCED / SOURCE-INSPECTED / NOT TESTED** for every major test; do not assert no bugs due to 15 browser paths. AC-08/11 |
| **H. Performance and UX** | Core/UI equality and 2,000-tree responsiveness, narrow viewport screenshot, legend/non-colour labels, false “real Irish forest” implications, no growth as experiment-step is advanced | Actual measured review-host timing or writer-only value marked as such. AC-08/09 + REQ-U09 |
| **I. Ownership/version constraints** | Owner-approved provider/laptop substitute; no local board lock; no physical host ownership presently verified; single-writer branch/candidate provenance; no touched Unity or root \`.gitignore\` | No false operational clearance. No source/Unity merge/integration acceptance. AC-01/11 |

Review against **all AC-01 through AC-11** from the approved spec (explicit PASS/FAIL/NOT TESTED and rationale), but keep emphasis on source-backed high-risk paths. P0 candidate may be fully implementable without extra ecology research; that does not prove biological or field accuracy.

## 4. Proportionate independent verification plan

**One bounded independent review run** (not multiple expensive full integrations):

- **Tier 1:** Run clean \`dotnet build MultiRackSitkaLab.sln\`, \`dotnet run --no-build --project tests/Core.Tests\` and \`dotnet run --no-build --project tests/Host.Tests\` on independently checked candidate. Verify schema/fixture manifests. Log exit codes, runtime and stdout/stderr paths. Targeted negative tests for M17 queue-again/queue-after-cancel/executed and known plan rejection, replay and header safety; do not blindly trust writer's test tally.
- **Mutation confidence:** The 16 initial controls were **not rerun** after M17 assertions were added. In an isolated disposable scratch copy, rerun the **17 existing mutation controls once if feasible and without source contamination**. Record exact survivors. Do not weaken tests or mutate the candidate branch. If unavailable/time prohibitive, mark limitation and reason; it is **not automatically a P0 blocker** if independent targeted guard proof is otherwise adequate.
- **Tier 2:** Run one real browser-based UI suite in the isolated review environment with \`EVIDENCE_DIR\` pointing to a reviewer-owned off-repo folder. The writer's Chromium run was **headless only**; a headed smoke/second browser is **useful when supported** but not a blanket P0 blocker. If only headless runs, say so. Confirm no unusual screenshot/tool artefact is called a human visual inspection; inspect one actual screenshot if accessible.
- **Speed:** Check 2,000-tree Core preview/inspector using **actual review-host** timing; if inaccessible, report the author's 169 ms only as WRITER CLAIMED. The accepted target is ≤2 s on the recorded actual host.
- Do **not** run Unity import/compile/Editor or native Windows game build, CAPSIS/Samsara, broad forestry simulation, full release Tier 3 or external data acquisition. No new test dependency installs from untrusted repositories, accounts or contact.
- Keep Core test seam distinct from public API; if unreachable corrupt-states tests pass but public attack paths remain untested, identify coverage gap and severity. The original M17 miss merits particular scrutiny.

**Suggested execution commands** (verify actual available SDK/browser before use):
\`\`\`bash
cd Tools/MultiRackSitkaLab
dotnet build MultiRackSitkaLab.sln
dotnet run --no-build --project tests/Core.Tests
dotnet run --no-build --project tests/Host.Tests
sha256sum -c fixtures/synthetic/MANIFEST.sha256
python3 -I -B tests/check-schema.py fixtures/synthetic/*.sitkalab.json
EVIDENCE_DIR=/path/to/reviewer-owned-off-repo-evidence node tests/Ui/run-browser-tests.mjs
\`\`\`
Check the schema-script invocation from its actual help/source rather than assuming these arguments are correct. The mutation runner operates on **a throwaway copy** only (it edits source files transiently); never run it against the live candidate/reviewer checkout unless the script's safe restore behavior is verified and doing so cannot disrupt other workers.

## 5. Exact result, review verdict and closure

**Review branch (created but not claimed by any worker):** \`review/multi-003-sitka-2d-lab-p0-independent-20261114-v1\`, pinned at initial candidate HEAD \`f33641f85ec8146ae26b91e4d5ea0b89bfba40f0\`.  
**Sole tracked reviewer output (new file):** \`Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-003-SITKA-2D-LAB-P0-INDEPENDENT-REVIEW-VERDICT-20261114.md\`.  
**Review author:** independent Claude Opus 5.5/Claude Code or capable separate Sol/Codex High, **not** Claude Sonnet 5.5 source author.

The handoff must contain:
1. Actual worker identity, registered starting HEAD/worktree/physical host, exact source candidate and reviewed scope/path diff; what independent tests physically ran vs unverified off-repo writer artifacts.
2. Critical/major/minor findings, precise source file/line and reproduction, no unsupported speculation. **Unresolved source defects may justify NEEDS CHANGES** even when writer tests pass.
3. AC-01–AC-11 and LAB-01–LAB-05 verdict matrix, explicit confidence for preflight/board gap, expected method/limits, evidence path hashes or screenshots actually inspected, performance, HEAD/source recheck.
4. A separate finding on M17 and the mutation test coverage (16/17 original vs M17-only correction, optional full rerun) and whether corrupt-state-only testing misses feasible public API cases.
5. A final **APPROVED / NEEDS CHANGES / REJECTED** verdict for **ENGINEERING P0 ONLY**. APPROVED does **not** mean integrated into Scenario One, scientifically accurate Irish forestry, accepted by user or ready for P1 ecology.
6. Clear next action: if APPROVED, manager may accept independent **P0 engineering qualification** while keeping standalone lab isolated; if NEEDS CHANGES, one narrowly scoped author repair packet and focused recheck, not a new complete implementation; if REJECTED, reason/safe rollback (no destructive cleanup).
7. Readback of exact GitHub report and compare review branch against registered candidate starting HEAD, **exactly one added Markdown file** and **zero source/approved-spec/coordination modifications**. Mark [#497](https://github.com/jermur788/CCF/issues/497) **[HANDED-OFF]**, close only after publication, and notify #494/Manager. Dashboard/Drive synchronization **NOT VERIFIED** unless evidence proves otherwise.

**No source edits, Unity, CCF production code, protected save, spec changes, coordination writer assumption, real forest data, account registration, external outreach or Windows game build.**

## 6. Paste-ready independent reviewer dispatch

> **MULTI-003 — CCF #497 independent P0 engineering review.** Read the full binding GitHub packet at https://github.com/jermur788/CCF/blob/docs/multi-003-sitka-lab-p0-independent-review-dispatch-20261114-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-003-SITKA-2D-LAB-P0-INDEPENDENT-ENGINEERING-REVIEW-DISPATCH-20261114.md. Independently inspect the completed **MULTI-002 standalone synthetic Sitka 2D C#/.NET+SVG laboratory** at source candidate \`feature/multi-002-sitka-2d-reference-lab-p0-20261114-v1\` and its detailed #494 writer handoff. You **must not be the original Claude Sonnet 5.5 author**. Recommended model Claude Opus 5.5/Claude Code or independent Sol/Codex High. Register actual provider/model, safe isolated host/worktree, review branch and starting HEAD as RUNNING on [#497](https://github.com/jermur788/CCF/issues/497) **before local tests or report edits**, after checking fresh live board/physical ownership. Do not assume absence of a board row authorises collision, and do not reuse/alter the author's worktree. Focus **Engine plan/queue/atomic rejection (especially old M17)**, snapshot hash/replay and load epochs, lab-root path/symlink/header security, loopback Host/Origin/CSRF, Core/UI parity, fixture provenance and actual writer-side vs independently observed Tier1/Tier2 evidence. Run proportionate independent headless tests and one browser path if safely possible; full mutation matrix once in isolated scratch is useful but not an automatic requirement. Return **APPROVED / NEEDS CHANGES / REJECTED FOR ENGINEERING P0 ONLY**, with AC-01–11 evidence and limitations. Publish only \`Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-003-SITKA-2D-LAB-P0-INDEPENDENT-REVIEW-VERDICT-20261114.md\` on \`review/multi-003-sitka-2d-lab-p0-independent-20261114-v1\`; read back, verify one-file diff, close #497 HANDED-OFF and return to Manager. **No source edits, P1 biology, real forest data, Unity, saved game, live coordination writes or Windows build.**
