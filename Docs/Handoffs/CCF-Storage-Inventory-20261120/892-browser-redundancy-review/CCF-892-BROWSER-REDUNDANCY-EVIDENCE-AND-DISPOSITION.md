# #892 — Browser-only redundancy evidence and disposition queue

**Status:** RESEARCH COMPLETE / LOCAL CHECKS PENDING / **NO CLEANUP EXECUTED** / **NO DELETION APPROVED**.
**Date:** 2026-11-20, 22:05–22:50 UTC.
**Source snapshot:** report blob `1f19c1d88d9f1bf6f8ea9baf108a7d12f0d04f43`; register blob `6f6615877ea242ec07930ab87b5b9e41f6aa98ef` (306 rows, unchanged).
**Live base:** `main` = `e2226a252ff9b0ad016b4aa4c27552a1b3af1d9`.
**Companion:** `CCF-892-BROWSER-REDUNDANCY-DECISIONS.tsv` — 306 rows, one per source `id`, preserving the input IDs.

---

## 0. Provider and method disclosure

The packet suggested a separately appointed GitHub-browser GPT-6 researcher. **The actual worker is DeepSeek V4.1 Flash** (`opencode-go/deepseek-v4.1-flash`) via the T3 Code / OpenCode GitHub connector. No browser-GPT-6 session was started, and none is claimed.

The packet forbids local filesystem probes. This audit therefore makes **no local claim at all**: every disposition is derived from (a) the 306-row register snapshot, (b) live GitHub issue state, refs and commit objects, and (c) live `main` coordination text. All filesystem, process, FD, ignored-file, mount-identity and Unity-lock facts remain **UNKNOWN** and are listed as required local checks. `gh` was used only as the GitHub API client.

**No deletion, move, archive, worktree operation, merge, source edit or Unity access occurred.**

---

## 1. Executive findings

1. **The Tier A "12 versus 19" discrepancy is fully explained and is not a defect.** It is entirely a **date-boundary difference**: the strict Tier A cutoff is `< 2026-11-06` (14 days before the audit), while the naive prefilter used `< 2026-11-07`. The seven extra rows are exactly the 6 November worktrees, totalling **8.67 GB**. All seven are now classified as Tier A-adjacent and are otherwise eligible.
2. **The strict Files Tier A 12 is not uniformly safe.** Cross-checking live coordination removes five of them: `CCF-NonUnity` (a documented lane), `CCF-OpsUnityLock01` (Ops/lock governance), `CCF-ContextSync` (#429 open), and `CCF-S1D-Spec`/`CCF-S1D-T02` (S1-D remains pre-demo scope). **Only 7 of the 12 survive as research-cleared** (8.62 GB).
3. **The most important new finding is theme adjacency, not age.** **22** worktrees that the register marked `POSSIBLE REDUNDANT` — clean, pushed and apparently old — belong to workstreams whose controlling issue is **still open** (Workbench #287, Unity lease/guard #652, Scenario One UI presentation #382, Fable #447, U3 #361). Including already-protected rows, the adjacency hold covers **28 rows / 29.69 GB**. These were silently "candidate-safe" under a pure age/cleanliness filter and are now **HOLD**.
4. **Research-cleared ceiling: 62.74 GB** — Files **45 paths / 51.27 GB**, ZX20 6 paths / 11.48 GB. Only the Files figure relieves the urgent volume (24.3 GB free, 95 % used). ZX20 has 280.8 GB free, so its 11.48 GB is housekeeping, not relief.
5. **The published register contains three spurious open-issue links** caused by issue-number regex matching inside hex UUID fragments (e.g. `a7ea36bc-649a…` → "#649"). All three errors made rows *more* protected, never less. They are corrected in the decision TSV and disclosed in §8.
6. **Nothing in the 51-path shortlist is approved.** Each path still requires an exact owner approval and a native preflight covering ignored files, live processes/FDs, lease/registry state, mount identity and reconstruction proof.

---

## 2. Baseline validation of the 306-row register

| Check | Result |
|---|---|
| Rows | 306 |
| Unique `id` | 306 |
| Unique `absolute_path` | **305** |
| Duplicate path | `/media/jer/Files/CCF-Legacy` appears twice (`CCF-INV-0240` worktree row and `CCF-INV-0305` aggregate row), both 5,476,270,080 B |
| Status counts | 225 POSSIBLE / 55 HOLD / 11 PROTECT / 8 ACTIVE / 6 ARCHIVE / 1 NOT INSPECTED |
| Snapshot blobs | report `1f19c1d…`, register `6f661587…` — both match the packet |

**Double-count rule.** Any sum must exclude the duplicate legacy row and must never add a parent aggregate to its children. This report's ceilings use worktree rows only (`common_git_dir` = canonical or legacy store) and exclude all `SEE CHILDREN` composite rows, the duplicate legacy aggregate, and nested parent/child pairs. The register's naive total (≈1.23 TB) is **not** a physical footprint.

---

## 3. Tier A 12-versus-19 reconciliation — exact

Filter used for the naive count: mount `Files` + status `POSSIBLE REDUNDANT COPY` + `dirty_tracked = 0` + `untracked = 0` + HEAD reachable from `origin/*` + owner `NONE OBSERVED` + last-listed date `< 2026-11-07`.

- Naive result: **19 paths / 23.34 GB**
- Report §10 Tier A: **12 paths / 14.67 GB** (cutoff `< 2026-11-06`)
- Difference: **7 paths / 8.67 GB — all dated exactly 2026-11-06**

| Extra row | Path | Allocated | Branch | Current disposition |
|---|---|---:|---|---|
| `CCF-INV-0087` | `CCF/Operations/CCF-D072-308-IndependentReview-20261106` | 1.239 GB | `docs/d072-308-independent-combined-review-20261106-v1` | Cleared (Tier A-adjacent) |
| `CCF-INV-0089` | `CCF/Operations/CCF-D072-FiveRackReReview-20261106` | 1.239 GB | `docs/d072-five-rack-diagram-targeted-re-review-20261106-v1` | Cleared (Tier A-adjacent) |
| `CCF-INV-0090` | `CCF/Operations/CCF-D072-FiveRackReview-20261106` | 1.239 GB | `docs/d072-five-rack-diagram-review-20261106-v1` | Cleared (Tier A-adjacent) |
| `CCF-INV-0091` | `CCF/Operations/CCF-D072-MapSourceReview-20261106` | 1.239 GB | `docs/d072-stand-map-source-review-20261106-v1` | Cleared (Tier A-adjacent) |
| `CCF-INV-0095` | `CCF/Operations/CCF-FeedbackCTAF1F3Review-20261106` | 1.239 GB | `docs/baseline-cta-checker-f1-f3-re-review-20261106-v1` | Cleared (Tier A-adjacent) |
| `CCF-INV-0096` | `CCF/Operations/CCF-FeedbackCTAIndependentReview-20261106` | 1.239 GB | `docs/feedback-cta-checker-independent-review-20261106-v1` | Cleared (Tier A-adjacent) |
| `CCF-INV-0130` | `CCF/Operations/CCF-Overnight-Orchestrator-20261106` | 1.238 GB | `docs/deepseek-orchestration-20261105-v1` | Cleared (Tier A-adjacent) |

**Conclusion:** the 12 reported IDs are exactly the strict 14-day set; the 19 are that set plus the 6 November cohort. Neither number is wrong. `Tier A exact membership` is now recorded per ID in the decision TSV.

---

## 4. Strict Tier A (12) reclassification with live evidence

| Row | Path | Size | Live evidence | Disposition |
|---|---|---:|---|---|
| `CCF-INV-0128` | `CCF-Overnight-FeedbackVerify-20261104` | 1.232 GB | Overnight stream; #254/#269/#313/#332/#363 all CLOSED | **Cleared** |
| `CCF-INV-0129` | `CCF-Overnight-ObjectiveCopy-20261104` | 1.233 GB | same | **Cleared** |
| `CCF-INV-0131` | `CCF-Overnight-RackHandle-20261104` | 1.233 GB | same | **Cleared** |
| `CCF-INV-0132` | `CCF-Overnight-UIQualification-20261104` | 1.233 GB | same | **Cleared** |
| `CCF-INV-0102` | `CCF-MFM08Closure` | 1.230 GB | MFM-08 #195/#205 CLOSED | **Cleared** |
| `CCF-INV-0103` | `CCF-MFM08Qual` | 1.231 GB | same | **Cleared** |
| `CCF-INV-0104` | `CCF-MFM10Residual-20261101` | 1.232 GB | MFM-10 #213 CLOSED | **Cleared** |
| `CCF-INV-0081` | `CCF-ContextSync` | 1.196 GB | #429 context-sync remains **OPEN** | HOLD |
| `CCF-INV-0123` | `CCF-NonUnity` | 1.197 GB | Documented non-Unity lane; #690 coordination references it | HOLD |
| `CCF-INV-0127` | `CCF-OpsUnityLock01` | 1.230 GB | Ops/Unity-lock governance; #287/#652/#802 adjacency | HOLD |
| `CCF-INV-0145` | `CCF-S1D-Spec` | 1.196 GB | S1-D is pre-demo scope in the current milestone | OWNER-DECISION |
| `CCF-INV-0146` | `CCF-S1D-T02` | 1.228 GB | same | OWNER-DECISION |

**Strict Tier A cleared: 7 paths / 8.62 GB, not 12 / 14.7 GB.** The difference is not lost space; it is correctly reclassified protection.

---

## 5. Ranked Files-first owner decision queue

All rows below are clean, pushed, unclaimed by issue number, older than the Tier B ceiling, and have their exact HEAD object confirmed present on GitHub (`GET /commits/{sha}` returned the same SHA). **None is approved.**

### Rank 1 — Tier A (7 paths, 8.62 GB) — highest confidence

| # | Path | Size | Branch @ HEAD |
|---|---|---:|---|
| 1 | `CCF/Operations/CCF-Overnight-UIQualification-20261104` | 1.233 GB | `integration/scenario-one-overnight-ui-qualification-v1` @ `d3ba4fce39` |
| 2 | `CCF/Operations/CCF-Overnight-ObjectiveCopy-20261104` | 1.233 GB | `feature/overnight-objective-copy-20261104` @ `693e24c889` |
| 3 | `CCF/Operations/CCF-Overnight-RackHandle-20261104` | 1.233 GB | `feature/overnight-rack-handle-20261104` @ `9304d318f3` |
| 4 | `CCF/Operations/CCF-Overnight-FeedbackVerify-20261104` | 1.232 GB | `verify/overnight-playtest-feedback-20261104` @ `6efba8fdc3` |
| 5 | `CCF/Operations/CCF-MFM10Residual-20261101` | 1.232 GB | `verify/mfm-10-species-guard-v1` @ `43e263708d` |
| 6 | `CCF/Operations/CCF-MFM08Qual` | 1.231 GB | `qual/mfm-08-unity-v1` @ `d88a32f6b4` |
| 7 | `CCF/Operations/CCF-MFM08Closure` | 1.230 GB | `verify/mfm-08-tier2-closure-v1` @ `911a8a34ff` |

Recovery command for any one: `git worktree add <path> <branch>` at the recorded HEAD (native worker, separate authorisation only).

### Rank 2 — Tier A-adjacent, 6 Nov (7 paths, 8.67 GB)

The seven rows in §3. Same evidence class as Rank 1; the only difference is 24 hours of age. Recommend treating them as a single batch with Rank 1 if approved.

### Rank 3 — 7–9 November Files (15 paths, 16.11 GB)

Old review/orchestration checkouts. Confidence is lower because several have no issue number in path or branch, so closure rests on workstream naming plus absence of any live claim. Full list: decision TSV, `browser_disposition = RESEARCH-CLEARED`, filter `latest_verified_use` 2026-11-07…09.

### Rank 4 — 10–12 November Files (16 paths, 17.86 GB)

Most recent cleared cohort. Several are S1/CTA/D073 reviews whose issues are closed, but the one-week age means recent conversation context cannot be excluded from GitHub alone. Recommend the owner review these individually rather than as a batch.

### Rank 5 — ZX20, no warm Library (6 paths, 11.48 GB)

`CCF-AccessSpine-Measure-20261103` (1.92), `CCF-Intro-Astra-20261103` (1.92), `CCF-NextActionGuidance-T3-20261106` (1.97), `CCF-RackV2-05` (1.86), `CCF-RackV2-09` (1.89), `CCF-S1ObjOverflow` (1.92). None carries a `Library/`, so no reimport cost. **Low priority:** ZX20 has 280.8 GB free.

**Queue totals:** 51 paths / **62.74 GB** (Files 45 / 51.27 GB; ZX20 6 / 11.48 GB). Rank 1+2 alone = 14 paths / 17.29 GB and is the cleanest single approval unit.

---

## 6. Workstream-adjacency holds (the main safety correction)

These rows pass every age/cleanliness/push test but belong to **open** workstreams. A pure register filter would have promoted them. They are HOLD:

| Workstream | Paths | Controlling open issue | Why the hold |
|---|---:|---|---|
| Workbench / `CCF-WB*` | 10 | **#287** Workbench Tier-2 — NEEDS CHANGES / dirty lane | The lane is explicitly not clean and not released |
| Unity lease / guard / readiness | 9 | **#652** Ops deployment held | The tooling under audit is the subject of an open deployment gate |
| Scenario One UI presentation | 2 (+ the protected ZX20 #382 qualifier) | **#382** personal UI playtest | #382 is an owner-held personal qualifier |
| Fable | 3 | **#447** Fable orchestration open | Orchestration ownership unresolved |
| U3 clean witness | 1 | **#361** personal playtest | Personal lineage |
| Named Ops/lock lanes (`CCF-NonUnity`, `CCF-OpsUnityLock01`, `CCF-OpsReadiness-652`, `CCF-UnityLock-287-Registration`, `CCF-Ops287Migration-Docs`, `CCF-OpsLeaseFix`) | 6 | #287 / #652 / #802 | Named lanes and Ops change control; `overlap_802 = YES` |
| D-073 / personal-playtest lineage | 4 | #412/#417/#437 closed | Chain-of-custody unproven from GitHub |

This is why the naive "old + clean + pushed" set is not a deletion set.

---

## 7. Full 306-row disposition summary

| Disposition | Rows | Meaning |
|---|---:|---|
| RESEARCH-CLEARED / LOCAL-PREFLIGHT-REQUIRED | **51** | Eligible for an exact-path owner approval after native preflight |
| NEEDS-GITHUB-FOLLOWUP | **129** | 37 legacy-store worktrees (owner/closure unproven) + 86 worktrees younger than 13 Nov |
| HOLD | **72** | Active/protected/dirty/local-only/open-issue/Ops/#802/adjacent workstream |
| OWNER-DECISION | **37** | 23 T3 root sessions (separate programme), 6 warm ZX20 Libraries, 2 S1-D, Ops/build/T3 archive streams |
| NOT-ASSESSED | **17** | Composite/parent rows; decision belongs to children |
| **Total** | **306** | |

No row carries a `SAFE-TO-DELETE` value, by design.

---

## 8. Register self-corrections

Three register rows carry an `OPEN ISSUE REFERENCE` produced by matching a three-digit number inside a hex UUID. They are artefacts, not ownership:

| Row | Path | Register said | Actual |
|---|---|---|---|
| `CCF-INV-0013` | `.t3/…/t3-a7ea36bc-649a837498219d69` | #649 | `649` is part of the UUID; no issue token |
| `CCF-INV-0018` | `.t3/…/t3-d41dc361-17da144c075f7223` | #361 | `361` is part of the UUID; no issue token |
| `CCF-INV-0024` | `.t3/…/t3-f9f6d97f-5059d832ef463a85` | #832 | `832` is part of the UUID; branch is #676 (closed) |

The decision TSV records these as `REGISTER FLAG CORRECTED`. Effect: strictly protective (each row remains HOLD/OWNER-DECISION for other reasons). No candidate was promoted by the correction.

---

## 9. Cross-checks with #383, #385 and #802

- **#383** remains OPEN/PARTIAL with approved cleanup = 0. Its Z1/Z2 Libraries are still owner-unconfirmed; the inventory shows both unchanged. Nothing here revives or extends #383's conditional rulings.
- **#385**: all ten B1 paths were checked against the 306 register. **None reappears**, so no previously removed worktree is being proposed again. The old #385 approval is not reused for any new path.
- **#802**: five rows are flagged `overlap_802 = YES` (Ops lock/readiness/registration checkouts) and all are HOLD. `CCF/Builds` is `ADJACENT`. No #802 custody, count, script or deployment obligation is revised here, and no deletion candidate is duplicated into #802's lane.
- **#890/#891/#885/#886** are open; no candidate path intersects them after the workstream holds above.

---

## 10. Reclaim ceiling and what it is not

| Stream | Research ceiling | Status |
|---|---:|---|
| Files worktree queue (Ranks 1–4) | **51.27 GB** | Conditional; no approval |
| ZX20 no-Library worktrees | **11.48 GB** | Conditional; low urgency |
| **Total research-cleared** | **62.74 GB** | **Not freed, not approved** |
| Build archive stream (19 Build dirs + `CCF/Builds`) | 22.2 GB | Owner/evidence decision, not counted |
| `CCF/Verification` | 120.3 GB | Evidence store; no whole-store deletion |
| Legacy store `.git` | 4.0 GB | Requires stash `7b1c808d` + `prototype-baseline` tag preservation first |
| T3 root sessions / `~/dev` | ~58 GB | Separate programme; excluded by packet §4D |

The ceilings are non-overlapping worktree rows only. **No claim is made that any of this is "freed" space.** Files would move from 24.3 GB free to roughly 75 GB free *if* the entire Files queue were approved and executed — a hypothetical, not a result.

---

## 11. Mandatory local preflight (not executed here)

For every future exact-path candidate, a native worker must prove:

1. No task, agent, thread or user owns the path; live coordination and the physical Ops board agree.
2. No process has a cwd, open file descriptor, socket, Git lock or Unity lease in the path.
3. Exact mount UUID/device and realpath, with no symlink traversal or nested mount.
4. Branch and full HEAD match the recorded value; the exact commit is reachable from the canonical remote.
5. Clean staged/unstaged/untracked **and ignored** inventory, including nested saves, `.meta`, licensed binaries, caches and evidence.
6. No unique local stash, reflog-only commit or unpushed branch depends on it.
7. Recorded reconstruction command and a verified backup of anything not reproducible.
8. Explicit owner grant for the exact subset and single-writer control of the shared Git registry.
9. Non-forced `git worktree remove <exact path>` only; no wildcards, no `--force`, no glob cleanup.
10. Before/after free-space evidence and an execution handoff.

Any failed check means RETAIN.

---

## 12. Protected locations (excluded from every queue)

#361 `CCF-U3` and #382 `CCF-S1-UI-Presentation-V1-Qual`; U1/U2/`CCF-Integration`; U2's local-only HEAD `e26516f`; `CCF-Primary` and `Repository.git`; `CCF-Ops`; all warm ZX20 Libraries absent owner sign-off; every dirty/untracked worktree; the three legacy worktrees with local-only commits; all builds and `CCF/Verification`; the legacy stash and annotated tag; offline/unmounted material; and every path owned by an open issue.

---

## 13. Acceptance

- Browser-side research: **COMPLETE**.
- Cleanup executed: **ZERO**.
- Deletion/archive/merge/Unity authority granted: **NONE**.
- Local verification: **PENDING** and cannot be satisfied by this chat.
- Next step: #425/owner review of the ranked queue, then **exact-path approval only** for the chosen subset. Native execution is separately routed and must not conflict with #802.

*This report is decision support. It is not an approval, a work order, or evidence that any folder is unused.*
