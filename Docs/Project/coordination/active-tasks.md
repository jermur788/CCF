# Active Task Coordination

This file is the **live single-writer ownership and execution-lock authority** outside the project context snapshot. Completed historical work belongs in Git history, handoffs, Manager reviews and `Docs/Project/unity-project-overview.md`, not as live locks here.

**Last reconciled:** 2026-10-29 by Overall Manager / OpenAI GPT-5.6 Sol after independent Manager approval #158. Browser repair #145 is complete/closed; #158 is APPROVED/CLOSED; #143 is explicitly released on exact `c8cfae8c4025cec6e960a050edf4bfde2a981de5` for non-Unity implementation/offline verification, subject to task-start ownership/schema recheck. #124/#126/#128 remain blocked downstream.

## Coordination rules

- Overall Manager or assigned integrator maintains this file.
- Workers refresh this file and their claim at task start and before asserting a lock is absent.
- One writable lane/worktree per owner and one primary writer per subsystem/file set.
- Never stash, reset, discard, clean, overwrite or absorb unrelated dirty work.
- Overlap means stop before editing and obtain Manager ownership/API disposition.
- Claims record execution presence only; they do not grant ownership.
- Heavy Unity is serialized machine-wide. Verify branch/HEAD/ownership before lock acquisition; never bypass another owner or kill its process.
- No Windows build before the user's personal Scenario One playtest and explicit authorisation.

## Current integration truth

- MFM-06 #113: INTEGRATED / VERIFIED for its bounded gate at `b971f4da0540e606bfee9702b0c99f81a0a89355`.
- MFM-07 #125: INTEGRATED / VERIFIED at post-MFM-07 umbrella **`c8cfae8c4025cec6e960a050edf4bfde2a981de5`**, save schema v23. MFM-07 approval, MFM-06 save/load, protected hash dump and interaction regressions passed at that gate.
- MFM-08A #129: sole bounded coverage-world adapter writer on `feature/mfm-v1-coverage-world-adapter-v1` / `/media/jer/Videos/CCF-MFM-08A`. Latest durable worker record names local-only `aff488a5`; offline gates are green. Storage remediation #152 restored ZX20 above the preferred 20 GiB target. User reports #129's Unity task is now running in the cloud-code/local execution lane; fresh machine lock/process state remains authoritative.
- S1-C T-04 #123: final Manager-approved candidate `633775fd8650db3cf3482e550d106a0febd7b9cb`, docs successor `3ea4af8b`; closed/no ownership.
- S1-C T-06 #147: exact merged production candidate `d959e0397c80bc7a9d7a707c8d4f437f946231f4`, docs successor/tip `769ac28a08d47acb7dce727d1188025e56a2b3b8`; offline Manager review APPROVED. It remains sole history/review presentation owner and waits for its rendered/waypoint Unity qualification after the current Unity owner releases the machine lane.
- **D-066 product decision remains accepted.** Browser repair #145 narrowed its implementation contract to exactly one boolean-like monotone historical latch for the existing first guided rack sample/review, written only when the unlatched pre-amendment rack-world sequence genuinely crosses `response → recruitment` after prior lesson guards, `ReleaseReviewed` and actual session `RackReviewComplete`. Old/missing=false; no retrospective reconstruction; individual `[J]/[K]`/session detail remain non-persistent.
- Independent Manager review #158 is **APPROVED/CLOSED**. Independent inspection confirmed the real current-session write boundary and the need to strip the new teaching-only field from every world/reference/protected identity/hash projection, including the non-default v23 prescription-state `HashLayout` branch. Full save round-trip still preserves the latch; no rebaseline.
- #143 is now released from exact integration base `c8cfae8c4025cec6e960a050edf4bfde2a981de5` for implementation/offline verification. That base carries save schema v23; v24 is only the expected next slot if the worker's fresh task-start check still confirms v23 current/free and no competing save/schema/hash writer.
- S1-D T-05 #151 remains READY AFTER #147 but has no ownership while #147 holds the history/review seam.
- MFM-08 #156 remains blocked on #129 Unity qualification + Manager approval + integration/repin and has no production ownership.

## Live / prepared tasks

| Task | Primary / lane | Branch / worktree | Ownership | Restrictions / dependency | Status |
|---|---|---|---|---|---|
| **PLAYER-TEST-OVERNIGHT-01** (#91) | orchestration/integration lead | orchestration + child lanes | orchestration/claims/integration decisions | preserve child ownership; no product invention; no Windows build | **RUNNING** |
| **PLAYER-TEST-T3-CODE-LANE-01** (#106) | local C#/Unity umbrella | child worktrees | local implementation/qualification through child claims | exact candidate/handoff; Unity serialized | **RUNNING** |
| **MFM-08A coverage-world adapter** (#129) | Claude Sonnet 5.5 / Claude Code | `feature/mfm-v1-coverage-world-adapter-v1`; `/media/jer/Videos/CCF-MFM-08A` | sole bounded adapter writer | no MFM-08 UI/teaching/Work Plan/save/calibration; Unity serialized | **RUNNING — UNITY QUALIFICATION CURRENT PRIORITY** |
| **S1-C T-06 prior-treatment guidance** (#147) | T3 Code / Codex gpt-6.1-sol | `feature/s1-c-t06-prior-treatment-guidance-v1`; `/media/jer/Videos/CCF-S1C-T06` | sole bounded history/review presentation writer | no persistence/curriculum/simulation/MFM; offline approved; Unity waits on #129 | **OFFLINE APPROVED — UNITY WAITING** |
| **S1-D T-05 previous vs proposed** (#151) | local Code lane after #147 | `feature/s1-d-t05-previous-proposed-v1`; `/media/jer/Videos/CCF-S1D-T05` | **none while blocked** | wait for #147 handoff + Manager exact-lineage repin; no MFM-09/save/schema | **BLOCKED / READY AFTER #147 — NO OWNERSHIP** |
| **S1-D D-066 repair** (#145) | OpenAI GPT-5.6 Sol browser/GitHub | canonical `main`; no local worktree | none; browser result complete | no production ownership | **HANDED-OFF / CLOSED** |
| **D-066 repaired-contract Manager review** (#158) | Overall Manager browser/GitHub | canonical `main`; no local worktree | none; review complete | verdict recorded in durable result | **APPROVED / CLOSED** |
| **S1-D T-04P one-way teaching latch** (#143) | local save/code lane to be claimed | `feature/s1-d-t04-teaching-persistence-v1`; `/media/jer/Videos/CCF-S1D-T04-Persistence` | **released but no production owner until worker records RUNNING**; then sole bounded save/schema/hash writer for one latch | start exact `c8cfae8...`; fresh v23/free-writer check; non-Unity/offline may start now; Unity serialized | **READY FOR T3 OFFLINE — UNITY WAITS** |
| **S1-D T-04 guide monotonicity** (#124) | existing curriculum lane when re-released | `feature/s1-d-t04-guide-monotonicity-v1`; `/media/jer/Videos/CCF-S1D-T04` | **none while blocked**; later curriculum/read-only consumption only | wait for accepted/integrated #143 exact lineage; `b9d204c` evidence only | **BLOCKED ON #143 — NO OWNERSHIP** |
| **PARITY-08 save/deterministic compatibility** (#126) | local verifier TBD after S1-D | `verify/parity-08-save-compatibility-v1`; `/media/jer/Videos/CCF-Parity08` | **none while blocked** | repin after exact post-#143/#124 lineage | **BLOCKED ON S1-D LINEAGE** |
| **PARITY-08 independent review** (#128) | independent OpenAI review lane | exact future #126 candidate | review-only; no production ownership | starts only after repinned #126 handoff | **BLOCKED ON FUTURE #126** |
| **MFM-08A source-authority review** (#146) | browser/GitHub | canonical `main` | no production ownership | semantic decisions already recorded on #129; admin closure only | **SEMANTIC BLOCKER RESOLVED — ADMIN CLOSURE PENDING** |
| **MFM-08 Stand Map / 3D review UX** (#156) | local SOL/Codex UI/Unity lane when released | `feature/mfm-08-map-3d-review-ux-v1`; `/media/jer/Videos/CCF-MFM08` | **none while blocked** | wait for #129 Manager-approved/integrated exact lineage; avoid #147/#151/MFM-09 seams | **BLOCKED ON #129 — NO OWNERSHIP** |
| **PARITY-03 Q10 evidence strengthening** (#74) | verifier | existing verification lane | verification only | non-blocking; Unity only after critical-path owners | **WAITING** |

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

#147 remains the sole history/review presentation writer while running. #151 has no ownership until #147 releases the seam and Manager repins it. T-05 generic Work Plan grouping/history must not become MFM-09 teaching.

### #129 / #156 / MFM-09

#129 is the sole coverage-world adapter writer. #156 has no production ownership until #129 is Unity-qualified, Manager-approved, integrated and repinned. #156 never owns Annual Review/history, S1-D generic Work Plan history, save/schema or MFM-09 curriculum/Work Plan teaching.

## Dependency-ready sequence

### S1-D / parity

1. #145 repair = **COMPLETE/CLOSED**.
2. #158 independent Manager review = **APPROVED/CLOSED**.
3. #143 = **RELEASED NOW** for non-Unity implementation/offline work from exact `c8cfae8...`; worker must fresh-check v23/free ownership before edits. Unity qualification waits for the machine lane.
4. #143 implements/qualifies/independent-reviews one-way latch + all-hash exclusion; Manager reviews/integrates exact candidate.
5. Manager repins/releases #124 on exact #143 integration; #124 curriculum/read-only replay/qualification/review.
6. Only then repin/release #126 PARITY-08 on exact combined lineage.
7. #128 independent review follows #126.
8. T-08/T-09 follow the complete accepted S1-D implementation lineage.

### S1-C / S1-D T-05

- #147 completes rendered qualification/handoff first.
- Manager inspects/reconciles #147 API/lineage, then repins/releases #151 if no ownership conflict.

### MFM

- #129 qualification → Manager approval → integration/repin.
- #156 MFM-08 implementation only after that exact lineage and free UI ownership.
- MFM-09 follows accepted MFM-08 + accepted S1-D T-04/T-05; no active MFM-09 writer is registered.

### Endgame

- PARITY-09 waits for PARITY-08 + independent review + MFM-10.
- Final Tier-3 waits for all required Rack/parity/S1-C/S1-D/MFM integrated qualification on one exact candidate.
- User personal playtest follows qualified Scenario One candidate; Windows build only after explicit post-playtest authorisation.

## Durable browser handoffs

- #145 repair result: `Docs/Handoffs/S1-D-T04-Persistence-Amendment-Repair-20261029/BROWSER-RESULT.md`
- #158 Manager review result: `Docs/Handoffs/S1-D-T04-Persistence-Amendment-Repair-Manager-Review-20261029/BROWSER-RESULT.md`
- #153 MFM-08 readiness: `Docs/Handoffs/MFM-08-UX-Readiness-20261029/BROWSER-RESULT.md`

Issue #28 verification-harness hygiene remains backlog-only and blocks no current feature.