# Sen-1 #731 — #652 D-091 source-only start admission after physical board refresh

**VERDICT: APPROVED — CUSTODIAN ONE-ROW PHYSICAL BOARD REFRESH.**
**[APPROVED TO START — #652 D-091 SOURCE ONLY, CONDITIONAL ON FRESH ACTOR-SPECIFIC PHYSICAL RECHECK].**
**NOT A UNITY START. NOT A LIVE OPS DEPLOYMENT.** 2026-11-18, Overall Manager #731, OpenAI GPT-6, GitHub-browser.

## 1. Manager review: actual receipt

Read the [complete #652 physical board custodian RESULT](https://github.com/jermur788/CCF/blob/37c4e88bdcb23f56ed873705c85a7e4edfa78130/Docs/Handoffs/ScenarioOne-Operations-Readiness-20261118/Sen-1-652-BOARD-CUSTODIAN-D091-ONE-ROW-ADMISSION-REFRESH-RESULT-20261118.md), [registration 6978422396](https://github.com/jermur788/CCF/issues/652#issuecomment-6978422396), [physical-result receipt 6978468601](https://github.com/jermur788/CCF/issues/652#issuecomment-6978468601), and [blocked source-writer registration 6978301260](https://github.com/jermur788/CCF/issues/652#issuecomment-6978301260). Actual custodian: Claude Opus 5.5, Claude Code in T3 Code on jer-laptop, separate from source author, reporting owner's directly delegated physical board custody. The custodian must not independently review subsequent #652 source changes.

**Accepted as reported physical evidence, not separately re-executed by this GitHub Manager:**
- Under nonblocking exclusive flock on the shared board directory, custodian prepared reversible before bytes, verified unchanged original board SHA and exact source branch/worktree/HEAD, and atomically changed only **existing `CCF-652-OPS-READINESS` row**. No new row or lane assignment.
- Board SHA-256 before `8d3c17e173899562cf508a1114d2b0fa09b79022c93c95688f2d8529f098b6af` to after `d98c31560c7dc19047bb78bd0995045bd4a242adac491b3dab42bd9359fad272`; exact one-row replacement, 42 rows, same order, 41 other rows identical; original unmodified `packet-lint --board` exit0 both candidate and locked live board.
- `lanes.json`, installed Ops code, role docs, README, deployment manifest, environment and Unity guard/history have identical before/after recorded hashes. No physical Unity lock record and no observed source process. The full recovery, diff and log are under `/media/jer/Files/CCF/Verification/Sen1-652-D091-BoardCustody-20261118/`, as reported; Manager has not directly accessed this physical directory.
- Prospective source-only row now names actual writer, exclusive source checkout, existing source state and D-091 scope, explicitly **RESERVED ONLY / SOURCE BLOCKED / NOT RUNNING** until THIS Manager verdict; ownership by explicit release with NO expiry. Source worktree reported clean/remote-equal at time of custodian transaction. Cooperative authority is stated precisely, not cryptographically assured.

**Review result:** **APPROVED board registration and custody correction; prior identified physical-row blocker resolved.** No nonauthor review is needed to repeat this exact already validated custody update. This approval alone does not assert that physical state remains unchanged now; source worker must personally recheck at the first edit.

## 2. Exact binding actor / lane / permitted phase

- **Issue:** [#652](https://github.com/jermur788/CCF/issues/652), original same cumulative Ops source workstream. **Do not create a second #652 source author or branch.**
- **Sole source writer:** actual registered OpenAI/Codex **gpt-6.1-sol medium** native T3 Code session `9bcdfde6-699b-41f7-befb-acc9a7551f09`, host `jer-laptop`. The recommended GPT-6.2 High model is not a hard gate; permission binds the **actual** registered worker, not a fictional substitute.
- **Source branch:** `ops/sen-1-652-unity-lease-readiness-20261116-v1`. **Worktree:** `/media/jer/Files/CCF/Operations/CCF-OpsReadiness-652-20261116`, an existing Ops-only, non-Unity source worktree on Files CCF. The **later Compact30 Unity scene** source worktree remains separately required **on ZX20 SSD** under #718; this non-Unity Ops source repair is NOT that scene worktree.
- **Author-reported pinned source starting revision:** `a9f56d6385d49ac51e35f303de00d77d1e586195`, matching origin at prior check. If local head, remote tip, branch, file inventory or physical owner is no longer as reported, STOP and re-admit deliberately; no automatic rebasing or takeover.
- **Custody:** event-driven owner/worker documented release, **NO expiry or estimated-time cutoff**; one writer for exact source files; no source/Unity lock transfer implied.

### Source edit ceiling for new D-091 feature
1. `CCF-Ops/scripts/unity-lock`: explicit `--no-expiry` new acquisition mode, mutually exclusive with `--eta`, valid `eta:null` read/write in explicitly marked no-expiry mode; retain valid legacy timestamp and overdue-as-advisory, reject missing/corrupt metadata, preserve identity/owner/task/lock-id checks, single machine-wide guard and fail-closed no-reclaim semantics.
2. `CCF-Ops/tests/self-test.py`: complete deterministic isolated tests for null/legacy acquisition/status/second-block/release, concurrency, wrong owner/task/ID, legacy overdue, stale/reused PID, missing/invalid metadata, actual negative storage/device/UUID/fstype/path/symlink/branch/dirty policies; full original suite intact.
3. `CCF-Ops/README.md`: document event-driven explicit release, no expiry, manual safe process verification and no automatic stale reclaim.
4. `CCF-Ops/roles/unity-operator.md` **OPTIONAL**, only direct correction to existing operator procedure if shown needed and already recorded separately in physical row. Narrow fixtures only when justified and within original #652 conditional ceiling.
5. Same **existing cumulative #652 result Markdown** may be updated for source/report handoff under the pre-existing original task; no unapproved new output folder, source file, scripts or lane config.
6. Previously approved #652 ceiling remains available **only** for previously evidenced unrelated #652 defects, not automatic license for broad changes under this D-091 delta.

**NOT granted:** live `/media/jer/ZX20/CCF-Ops` installation/edits, actual physical board/lane/Unity lock changes, `_ccf.py`, `lane-status`, `packet-lint`, `lanes.json`, production/deployment manifests unless a distinct evidenced original #652 correction specifically requires it, game C#/Unity/assets, #690 save, Multi/2D, user Primary, Windows build or a second writer. This is **source-only Ops CLI code**, not Compact30 scene code.

## 3. Required actual-source start protocol

Before writing source or executing source tests, the sole registered worker shall:
1. Refresh main GitHub live coordination, #652 and this exact binding verdict; verify task has not been revoked or reassigned and prior author/custodian not concurrently writing.
2. Re-read actual installed physical #652 row, board and actual Ops locks under legally appropriate non-mutating observation; verify the expected after-board checksum, actual actor/session, D-091 scope and one-writer reservation. If authoritative physical board has legitimately changed since custody result, reconcile by content and lock receipt rather than treating the historical checksum as perpetual. No changes to the board by source author.
3. Inspect exact Files CCF source worktree, proper source store, branch, starting HEAD, remote equality, clean tracked/untracked/ignored user-owned status, no unrelated dirty changes, legal mount/space/identity; check any preflight relevant to this exact **non-Unity Ops source lane** using **original unmodified** validator, require actual acceptance/exit0/no relevant warning. Do not falsely claim `lane-status READY` on a `Lane None` row or substitute #690's preflight. If original validator cannot address a source checkout without a registered lane, document this fact and use the actual source/board custody checks specified by #652; STOP for Manager if this reveals a genuinely missing mandatory configured source lane rather than inventing one.
4. Post fresh **[RUNNING — #652 D-091 SOURCE ONLY]** on #652 naming the same actual session/model/provider/host, branch, worktree, starting revision, physical row readback, preflight/actual admission results and exact file paths. **Only then** change an in-ceiling source file and execute source tests.
5. Run complete original green suite and new positive/negative tests, preserve unrelated work; source error within scope can be repaired without new microtasks. If real owner conflict, required out-of-scope edit, invalid physical board/preflight, unsafe test root or unexpected Unity state: **STOP/BLOCKED**, preserve evidence and request adjudication.
6. Publish cumulative immutable GitHub source/result handoff and detailed tests/hashes. Arrange a **different model-family nonauthor independent review** of exact source candidate; previous review of the older source does not certify new D-091 code. The custodian Claude session above may NOT review its corresponding source delta.

**No additional approval hop for ordinary bounded source/test fixes** after the worker completes the admission checks above. Do not conflate source RUNNING with qualified, independently reviewed or deployed.

## 4. What remains closed to the worker

- Live Ops installation remains **HELD** despite the existing prior source review/141-pass history; the new D-091 source change needs its own full suite, independent review and exact physical deployment packet, exclusive installer/guard, backups/rollback and separate Manager **[APPROVED TO DEPLOY OPS]**.
- Unity's current installed ETA requirement still applies until actual reviewed deployment, so **NO Unity launch** or Compact30 scene creation under D-091.
- #718 **W §12 owner-signed exact seven-path exception** and focused review, actual ZX20 Compact30 lane and single machine-wide Unity lock, separate Manager **[APPROVED TO START — CONTENT BYTES ONLY]**, native import/readback all remain pending.
- Existing #690 Claude save-source and Multi #730 source/physical custody unchanged. Legacy40, Enlarged80/RackV2/MFM, frozen Reference Future v1 and v1–v24 protected.
- No arbitrary worker deadlines. Dashboard/Drive sync UNVERIFIED. No game build or Windows consent.

**Binding return to existing source worker (paste-ready):**
> Manager #731 approves your #652 D-091 SOURCE-ONLY start, conditioned on fresh live-board and exact checkout/ownership rechecks. Read the COMPLETE approval at https://github.com/jermur788/CCF/blob/docs/sen-1-731-chas-manager-intake-20261118-v1/Docs/Handoffs/ScenarioOne-Operations-Readiness-20261118/Sen-1-731-652-D091-SOURCE-ADMISSION-APPROVAL-AFTER-BOARD-REFRESH-20261118.md. Continue as the SAME registered Codex session `9bcdfde6-699b-41f7-befb-acc9a7551f09` on the existing clean Files CCF #652 Ops source worktree and branch. Recheck current physical #652 board row and source starting HEAD/remote, actual no-conflict ownership and original relevant preflight; register fresh [RUNNING — #652 D-091 SOURCE ONLY] with evidence BEFORE edits. Implement explicit `--no-expiry` unity-lock with backward-compatible fail-closed tests/docs within the allowlist; run full original suite and negatives; publish cumulative source result for independent review. No live Ops deployment, Unity, #690/Multi/game source or Windows. STOP on failed admission.
