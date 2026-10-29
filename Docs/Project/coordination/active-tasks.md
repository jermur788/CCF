# Active Task Coordination

This file is the **live single-writer ownership and execution-lock authority** outside the project context snapshot. Completed historical work belongs in Git history, handoffs, Manager reviews and `Docs/Project/unity-project-overview.md`, not as live locks here.

**Last reconciled:** 2026-10-29 by Overall Manager / OpenAI GPT-5.6 Sol under browser claim #159. Storage remediation #152 is handed off/closed; #146 is handed off/closed as an administrative record of the already-binding Manager source-authority disposition on #129; #148 is superseded/closed; #129 has a pushed, exact worker handoff with task-published Unity evidence but is **not Manager-approved or integrated**; #143 remains released for non-Unity/offline work but has no durable `[RUNNING]` start record and therefore no production ownership.

## Coordination rules

- Overall Manager or assigned integrator maintains this file.
- Workers refresh this file and their claim at task start and before asserting a lock is absent.
- One writable lane/worktree per owner and one primary writer per subsystem/file set.
- Never stash, reset, discard, clean, overwrite or absorb unrelated dirty work.
- Overlap means stop before editing and obtain Manager ownership/API disposition.
- Claims record execution presence only; they do not grant ownership.
- Heavy Unity is serialized machine-wide. Verify branch/HEAD/ownership, current free space, lock and Editor-process state immediately before lock acquisition; never infer those facts from an older handoff and never bypass another owner or kill its process.
- No Windows build before the user's personal Scenario One playtest and explicit authorisation.

## Current integration / durable handoff truth

- MFM-06 #113: INTEGRATED / VERIFIED for its bounded gate at `b971f4da0540e606bfee9702b0c99f81a0a89355`.
- MFM-07 #125: INTEGRATED / VERIFIED at post-MFM-07 umbrella **`c8cfae8c4025cec6e960a050edf4bfde2a981de5`**, save schema v23. MFM-07 approval, MFM-06 save/load, protected hash dump and interaction regressions passed at that gate.
- MFM-08A #129: **HANDED-OFF / CLAIM CLOSED; MANAGER REVIEW + INTEGRATION PENDING.** Branch `feature/mfm-v1-coverage-world-adapter-v1` is pushed. Worker handoff names gated production candidate **`716486a21588e76a482d3eaea4fed12069f75ffd`** and docs-only handoff successor **`0b33c3288ebc78ab7361d0cdb3fcee537324dadb`**. Its own published evidence reports offline MfmCoverageWorld/MFM-04/05/07 gates green and Unity compile, coverage-world, approval, MFM-06 save, 18/18 protected-hash comparison and Reference Future PASS. This is worker evidence, **not** Manager approval or integration. #129 owns no production files after handoff.
- Storage remediation #152: **HANDED-OFF / CLOSED.** Durable handoff reports `/media/jer/ZX20` restored from 14.734 GiB to **21.255 GiB free** and the 20 GiB target met by deleting only the inactive clean RackV2-09 Unity `Library` cache. This removes the stale standing storage blocker only; it does **not** prove current free space, current Unity lock state or Editor-process state.
- MFM-08A source authority: direct Manager dispositions on #129 are binding. Track uses authoritative Dirt Path geometry plus the existing `ForestStartingStand` path-corridor setting; Clearing uses `Forest Clearing` plus the existing clearing-radius setting; Site uses durable buildable/structure positions plus that clearing-radius setting; player start/live player position is excluded from runtime MFM Site coverage; minimal read-only accessors are permitted; ProtectedArea polygon sampling is rejected/fail-closed; `ScenarioOneManager.ReadCoverage` is permitted only as a thin side-effect-free production-read façade. #146 now durably records those decisions and is closed; #148 is superseded/closed and made no duplicate semantic decision.
- S1-C T-04 #123: final Manager-approved candidate `633775fd8650db3cf3482e550d106a0febd7b9cb`, docs successor `3ea4af8b`; closed/no ownership.
- S1-C T-06 #147: exact merged production candidate `d959e0397c80bc7a9d7a707c8d4f437f946231f4`, docs successor/tip `769ac28a08d47acb7dce727d1188025e56a2b3b8`; offline Manager review **APPROVED FOR OFFLINE PHASE ONLY**. Required two-resolution rendered/waypoint Unity qualification and affected Unity regressions remain pending. No current machine availability is inferred from #129's earlier handoff; #147 must fresh-preflight before any Unity attempt.
- **D-066 product decision remains accepted.** Browser repair #145 narrowed its implementation contract to exactly one boolean-like monotone historical latch for the existing first guided rack sample/review, written only when the unlatched pre-amendment rack-world sequence genuinely crosses `response → recruitment` after prior lesson guards, `ReleaseReviewed` and actual session `RackReviewComplete`. Old/missing=false; no retrospective reconstruction; individual `[J]/[K]`/session detail remain non-persistent.
- Independent Manager review #158 is **APPROVED/CLOSED**. Independent inspection confirmed the real current-session write boundary and the need to strip the new teaching-only field from every world/reference/protected identity/hash projection, including the non-default v23 prescription-state `HashLayout` branch. Full save round-trip still preserves the latch; no rebaseline.
- #143 is released from exact integration base `c8cfae8c4025cec6e960a050edf4bfde2a981de5` for implementation/offline verification. That base carries save schema v23; v24 is only the expected next slot if the worker's fresh task-start check still confirms v23 current/free and no competing save/schema/hash writer. **No durable `[RUNNING]` worker start has been published after release, so no production ownership is registered.**
- S1-D T-05 #151 remains READY AFTER #147 but has no ownership while #147 holds the history/review seam.
- MFM-08 #156 remains **BLOCKED / NO OWNERSHIP** until the exact #129 worker candidate is Manager-reviewed/APPROVED, integrated into the Scenario One umbrella and #156 is repinned to that exact integrated lineage.
- PARITY-03 Q10 #74 remains a non-blocking evidence-strengthening follow-up. Its static phase at `3383e763e8ccff945dd00250366b7f334fe57331` is Manager-approved; the controller Unity measurement is still incomplete. #152 removes the stale storage-floor blocker, but #74 must fresh-preflight current storage/lock/process/priority before retrying and must not infer a continuity verdict from the interrupted import.

## Live / prepared tasks

| Task | Primary / lane | Branch / worktree | Ownership | Restrictions / dependency | Status |
|---|---|---|---|---|---|
| **PLAYER-TEST-OVERNIGHT-01** (#91) | orchestration/integration lead | orchestration + child lanes | orchestration/claims/integration decisions | preserve child ownership; no product invention; no Windows build | **RUNNING** |
| **PLAYER-TEST-T3-CODE-LANE-01** (#106) | local C#/Unity umbrella | child worktrees | local implementation/qualification through child claims | exact candidate/handoff; Unity serialized | **RUNNING** |
| **S1-C T-06 prior-treatment guidance** (#147) | T3 Code / Codex gpt-6.1-sol | `feature/s1-c-t06-prior-treatment-guidance-v1`; `/media/jer/Videos/CCF-S1C-T06` | sole bounded history/review presentation writer while claim remains active | no persistence/curriculum/simulation/MFM; offline approved; fresh Unity preflight required | **OFFLINE APPROVED — UNITY QUALIFICATION WAITING** |
| **S1-D T-05 previous vs proposed** (#151) | local Code lane after #147 | `feature/s1-d-t05-previous-proposed-v1`; `/media/jer/Videos/CCF-S1D-T05` | **none while blocked** | wait for #147 final handoff + Manager exact-lineage repin; no MFM-09/save/schema | **BLOCKED / READY AFTER #147 — NO OWNERSHIP** |
| **S1-D T-04P one-way teaching latch** (#143) | local save/code lane to be claimed | `feature/s1-d-t04-teaching-persistence-v1`; `/media/jer/Videos/CCF-S1D-T04-Persistence` | **no production owner until worker records `[RUNNING]` after fresh task-start checks**; then sole bounded save/schema/hash writer for one latch | start exact `c8cfae8...`; fresh v23/free-writer check; non-Unity/offline may start; Unity serialized | **READY FOR T3 OFFLINE — NOT RUNNING** |
| **S1-D T-04 guide monotonicity** (#124) | existing curriculum lane when re-released | `feature/s1-d-t04-guide-monotonicity-v1`; `/media/jer/Videos/CCF-S1D-T04` | **none while blocked**; later curriculum/read-only consumption only | wait for accepted/integrated #143 exact lineage; `b9d204c` evidence only | **BLOCKED ON #143 — NO OWNERSHIP** |
| **PARITY-08 save/deterministic compatibility** (#126) | local verifier TBD after S1-D | `verify/parity-08-save-compatibility-v1`; `/media/jer/Videos/CCF-Parity08` | **none while blocked** | repin after exact post-#143/#124 lineage | **BLOCKED ON S1-D LINEAGE** |
| **PARITY-08 independent review** (#128) | independent OpenAI review lane | exact future #126 candidate | review-only; no production ownership | starts only after repinned #126 handoff | **BLOCKED ON FUTURE #126** |
| **MFM-08 Stand Map / 3D review UX** (#156) | local SOL/Codex UI/Unity lane when released | `feature/mfm-08-map-3d-review-ux-v1`; `/media/jer/Videos/CCF-MFM08` | **none while blocked** | wait for #129 Manager approval + integration + exact repin; avoid #147/#151/MFM-09 seams | **BLOCKED — NO OWNERSHIP** |
| **PARITY-03 Q10 evidence strengthening** (#74) | verifier | `verify/parity-03-density-continuity-q10-v1`; `/media/jer/ZX20/CCF-Q10` | verification only; no production ownership | static phase approved; controller Unity measurement incomplete; non-blocking; fresh preflight/priority required | **WAITING — NON-BLOCKING UNITY FOLLOW-UP** |

No row by itself proves that the Unity lock is free. Every Unity worker must refresh lock/process/free-space immediately before use.

## Single-writer boundaries

### #143 / #124

- #145 repair and #158 Manager review are complete/closed and own no production code.
- #143 is explicitly released, but production ownership begins only when its worker refreshes this ledger and records `[RUNNING]` with actual model/provider/branch/worktree/start SHA.
- #143 may persist **one boolean-like first-guided-sample/review historical completion latch only**, with minimum version/capture/restore/hash compatibility plumbing. No per-cycle key/list/counters; no J/K/session detail.
- #143's latch writes false→true only at the genuine unlatched `response → recruitment` transition after prior lesson guards, `ReleaseReviewed` and actual session `RackReviewComplete`. T-03's current helper alone is insufficient.
- Before latch, T-03 retains accepted current-world/session cap semantics. After latch, T-03 and T-04 consume one shared read-only historical completion meaning.
- Teaching latch round-trips in saves but is excluded from **all** world/reference/protected hash identity layouts, including non-default prescription worlds; no rebaseline.
- #124 never owns save/schema/hash. It resumes only after exact accepted/integrated #143 and Manager repin; it replays curriculum/read-only consumption against that lineage.

### #126 / #128

#126 cannot become the save/validator/hash/reference writer until #143 is accepted/integrated, #124 is accepted on the exact resulting lineage, and Manager republishes/repins PARITY-08. #128 remains read-only independent review after #126 handoff.

### #147 / #151

#147 remains the sole history/review presentation writer while active. #151 has no ownership until #147 releases the seam and Manager repins it. T-05 generic Work Plan grouping/history must not become MFM-09 teaching.

### #129 / #156 / MFM-09

#129 has handed off and owns no production files. Its exact pushed candidate is evidence awaiting Manager implementation review/integration. #156 has no production ownership until that exact lineage is Manager-approved, integrated and repinned. #156 never owns Annual Review/history, S1-D generic Work Plan history, save/schema or MFM-09 curriculum/Work Plan teaching. No active MFM adapter writer is registered.

## Dependency-ready sequence

### S1-D / parity

1. #145 repair = **COMPLETE/CLOSED**.
2. #158 independent Manager review = **APPROVED/CLOSED**.
3. #143 = **RELEASED** for non-Unity implementation/offline work from exact `c8cfae8...`; worker must fresh-check v23/free ownership and publish `[RUNNING]` before edits. No worker start is durably recorded yet.
4. #143 implements/qualifies/independent-reviews one-way latch + all-hash exclusion; Manager reviews/integrates exact candidate.
5. Manager repins/releases #124 on exact #143 integration; #124 curriculum/read-only replay/qualification/review.
6. Only then repin/release #126 PARITY-08 on exact combined lineage.
7. #128 independent review follows #126.
8. T-08/T-09 follow the complete accepted S1-D implementation lineage.

### S1-C / S1-D T-05

- #147 completes the exact rendered/waypoint Unity qualification and publishes the final handoff.
- Manager inspects/reconciles #147 API/lineage and integration state, then repins/releases #151 if no ownership conflict.

### MFM

- #129 worker implementation/qualification = **HANDED-OFF** on exact pushed candidate `716486a2...` with docs-only successor `0b33c328...`.
- Next: Manager implementation review of that exact candidate; if APPROVED, integrate/repin the Scenario One umbrella.
- #156 MFM-08 implementation starts only after that exact integrated lineage and free UI ownership.
- MFM-09 follows accepted MFM-08 + accepted S1-D T-04/T-05; no active MFM-09 writer is registered.

### Evidence follow-up

- #74 Q10 remains non-blocking. Storage remediation #152 is complete, but the Unity controller measurement still needs a fresh preflight and should run only when current critical-path ownership/priority permits.

### Endgame

- PARITY-09 waits for PARITY-08 + independent review + MFM-10.
- Final Tier-3 waits for all required Rack/parity/S1-C/S1-D/MFM integrated qualification on one exact candidate.
- User personal playtest follows qualified Scenario One candidate; Windows build only after explicit post-playtest authorisation.

## Durable handoffs / reconciliation records

- #145 repair result: `Docs/Handoffs/S1-D-T04-Persistence-Amendment-Repair-20261029/BROWSER-RESULT.md`
- #158 Manager review result: `Docs/Handoffs/S1-D-T04-Persistence-Amendment-Repair-Manager-Review-20261029/BROWSER-RESULT.md`
- #146 administrative source-authority result: `Docs/Handoffs/MFM-08A-Source-Authority-Review-20261029/BROWSER-RESULT.md`
- #148 superseded review closure: `Docs/Handoffs/MFM-08A-Source-Authority-Manager-Review-20261029/BROWSER-RESULT.md`
- #152 storage handoff: `Docs/Handoffs/Unity-SSD-Headroom-Remediation-20261029/HANDOFF.md`
- #129 worker handoff: `Docs/Handoffs/MFM-08A-Coverage-World-Adapter/HANDOFF.md` on `feature/mfm-v1-coverage-world-adapter-v1`
- #153 MFM-08 readiness: `Docs/Handoffs/MFM-08-UX-Readiness-20261029/BROWSER-RESULT.md`
- #159 coordination reconciliation: `Docs/Handoffs/Browser-Coordination-Reconciliation-20261029/BROWSER-RESULT.md`

Issue #28 verification-harness hygiene remains backlog-only and blocks no current feature.
