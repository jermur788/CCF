# Active Task Coordination

This file is the **live single-writer ownership and execution-lock authority** outside the project context snapshot. Completed historical work belongs in Git history, handoffs, Manager reviews and `Docs/Project/unity-project-overview.md`, not as live locks here.

**Last reconciled:** 2026-10-29 by Overall Manager / OpenAI GPT-5.6 Sol after browser repair #145. D-066 is repaired to one monotone first-guided-sample/review completion latch, but the repaired contract is **not released** until independent Manager review #158. #143/#124/#126/#128 remain blocked with no save/schema/hash ownership.

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
- MFM-08A #129: sole bounded coverage-world adapter writer on `feature/mfm-v1-coverage-world-adapter-v1` / `/media/jer/Videos/CCF-MFM-08A`. Latest worker record names local-only `aff488a5`; offline gates green, Unity not run, candidate unpushed/unreviewed/not integrated; storage floor blocks qualification.
- S1-C T-04 #123: final Manager-approved candidate `633775fd8650db3cf3482e550d106a0febd7b9cb`, docs successor `3ea4af8b`; closed/no ownership.
- S1-C T-06 #147: preserved candidate merged non-destructively with final T-04; offline-green, rendered qualification blocked by storage. It remains sole history/review presentation owner.
- **D-066 product decision remains accepted.** Browser Manager review #144 found implementation-contract defects. Browser repair #145 has now repaired the proposed authority to exactly one boolean-like monotone historical latch for the existing first guided rack sample/review, written only when the unlatched pre-amendment rack-world sequence genuinely crosses `response → recruitment` after prior lesson guards, `ReleaseReviewed` and actual session `RackReviewComplete`. Old/missing=false; no retrospective reconstruction; individual `[J]/[K]`/session detail remain non-persistent.
- The repaired D-066 contract also states that the teaching latch is saved progression **but never world identity**: every `ScenarioReferenceArchive` world/hash projection must strip/ignore it, including the non-default v23 prescription-state `HashLayout` branch and any minimal/protected projection. Full save round-trip still preserves it. No rebaseline.
- **#158 is the independent browser Manager gate for the repaired #145 contract.** Until #158 APPROVED, #143/#124/#126/#128 remain blocked. No S1-D schema version is reserved; v24 remains only an expected next slot if later live refresh still finds v23 current/free.
- S1-D T-05 #151 remains READY AFTER #147 but has no ownership while #147 holds the history/review seam.
- MFM-08 #156 remains blocked on #129 Unity qualification + Manager approval + integration/repin and has no production ownership.

## Live / prepared tasks

| Task | Primary / lane | Branch / worktree | Ownership | Restrictions / dependency | Status |
|---|---|---|---|---|---|
| **PLAYER-TEST-OVERNIGHT-01** (#91) | orchestration/integration lead | orchestration + child lanes | orchestration/claims/integration decisions | preserve child ownership; no product invention; no Windows build | **RUNNING** |
| **PLAYER-TEST-T3-CODE-LANE-01** (#106) | local C#/Unity umbrella | child worktrees | local implementation/qualification through child claims | exact candidate/handoff; Unity serialized | **RUNNING** |
| **S1-C T-06 prior-treatment guidance** (#147) | T3 Code / Codex gpt-6.1-sol | `feature/s1-c-t06-prior-treatment-guidance-v1`; `/media/jer/Videos/CCF-S1C-T06` | sole bounded history/review presentation writer | no persistence/curriculum/simulation/MFM; rendered gate pending storage | **RUNNING — UNITY BLOCKED BY STORAGE** |
| **S1-D T-05 previous vs proposed** (#151) | local Code lane after #147 | `feature/s1-d-t05-previous-proposed-v1`; `/media/jer/Videos/CCF-S1D-T05` | **none while blocked** | wait for #147 handoff + Manager exact-lineage repin; no MFM-09/save/schema | **BLOCKED / READY AFTER #147 — NO OWNERSHIP** |
| **S1-D D-066 repair** (#145) | OpenAI GPT-5.6 Sol browser/GitHub | canonical `main`; no local worktree | browser spec/packet/result only | no C#/Unity/save-schema implementation | **HANDED-OFF — CLOSE AFTER RESULT LINK** |
| **D-066 repaired-contract Manager review** (#158) | independent browser reviewer | canonical `main`; no local worktree | review only; no production files | review #145 result; verdict APPROVED / NEEDS CHANGES / REJECTED | **PREPARED — NEXT S1-D GATE** |
| **S1-D T-04P one-way teaching latch** (#143) | local save/code lane TBD after approval | `feature/s1-d-t04-teaching-persistence-v1`; `/media/jer/Videos/CCF-S1D-T04-Persistence` | **none while blocked**; later sole bounded save/schema/hash writer for one latch | must wait for #158 APPROVED + explicit Manager repin/release; v24 not reserved | **BLOCKED ON #158 — NO OWNERSHIP** |
| **S1-D T-04 guide monotonicity** (#124) | existing curriculum lane when re-released | `feature/s1-d-t04-guide-monotonicity-v1`; `/media/jer/Videos/CCF-S1D-T04` | **none while blocked**; later curriculum/read-only consumption only | wait for #158 then accepted/integrated #143 exact lineage; `b9d204c` evidence only | **BLOCKED ON #158 / #143 — NO OWNERSHIP** |
| **PARITY-08 save/deterministic compatibility** (#126) | local verifier TBD after S1-D | `verify/parity-08-save-compatibility-v1`; `/media/jer/Videos/CCF-Parity08` | **none while blocked** | old v23-only basis obsolete; repin after exact post-#143/#124 lineage | **BLOCKED ON #158 + S1-D LINEAGE** |
| **PARITY-08 independent review** (#128) | independent OpenAI review lane | exact future #126 candidate | review-only; no production ownership | starts only after repinned #126 handoff | **BLOCKED ON FUTURE #126** |
| **MFM-08A source-authority review** (#146) | browser/GitHub | canonical `main` | no production ownership | semantic decisions already recorded on #129; admin closure only | **SEMANTIC BLOCKER RESOLVED — ADMIN CLOSURE PENDING** |
| **MFM-08A coverage-world adapter** (#129) | Claude Sonnet 5.5 / Claude Code | `feature/mfm-v1-coverage-world-adapter-v1`; `/media/jer/Videos/CCF-MFM-08A` | sole bounded adapter writer | no MFM-08 UI/teaching/Work Plan/save/calibration; Unity/storage gate | **RUNNING — UNITY BLOCKED BY STORAGE** |
| **MFM-08 Stand Map / 3D review UX** (#156) | local SOL/Codex UI/Unity lane when released | `feature/mfm-08-map-3d-review-ux-v1`; `/media/jer/Videos/CCF-MFM08` | **none while blocked** | wait for #129 Manager-approved/integrated exact lineage; avoid #147/#151/MFM-09 seams | **BLOCKED ON #129 — NO OWNERSHIP** |
| **PARITY-03 Q10 evidence strengthening** (#74) | verifier | existing verification lane | verification only | separate Unity/storage scheduling | **WAITING** |

No row asserts that the Unity lock is free. Every Unity worker must refresh lock/process/free-space immediately before use.

## Single-writer boundaries

### #145 / #158 / #143 / #124

- #145 is browser-only and owns no production code. Its repair result is `Docs/Handoffs/S1-D-T04-Persistence-Amendment-Repair-20261029/BROWSER-RESULT.md`.
- #158 is independent browser review only. It may approve/reject the repaired contract but does not implement it.
- #143 has **no ownership** until #158 APPROVED and Manager explicitly releases it after fresh live schema/ownership inspection.
- If released, #143 may persist **one boolean-like first-guided-sample/review historical completion latch only**, with minimum version/capture/restore/hash compatibility plumbing. No per-cycle key/list/counters; no J/K/session detail.
- #143's latch writes false→true only at the genuine unlatched `response → recruitment` transition after prior lesson guards, `ReleaseReviewed` and actual session `RackReviewComplete`. T-03's current helper alone is insufficient.
- Before latch, T-03 retains accepted current-world/session cap semantics. After latch, T-03 and T-04 consume one shared read-only historical completion meaning.
- Teaching latch round-trips in saves but is excluded from **all** world/reference/protected hash identity layouts; no rebaseline.
- #124 never owns save/schema/hash. It resumes only after exact accepted/integrated #143 and Manager repin; it replays curriculum/read-only consumption against that lineage.

### #126 / #128

#126 cannot become the save/validator/hash/reference writer until #158 approves, #143 is accepted/integrated, #124 is accepted on the exact resulting lineage, and Manager republishes/repins PARITY-08. #128 remains read-only independent review after #126 handoff.

### #147 / #151

#147 remains the sole history/review presentation writer while running. #151 has no ownership until #147 releases the seam and Manager repins it. T-05 generic Work Plan grouping/history must not become MFM-09 teaching.

### #129 / #156 / MFM-09

#129 is the sole coverage-world adapter writer. #156 has no production ownership until #129 is Unity-qualified, Manager-approved, integrated and repinned. #156 never owns Annual Review/history, S1-D generic Work Plan history, save/schema or MFM-09 curriculum/Work Plan teaching.

## Dependency-ready sequence

### S1-D / parity

1. #145 = **HANDED-OFF** repaired contract.
2. Run browser Manager review **#158**.
3. If #158 APPROVED, Manager refreshes live schema/ownership and repins/releases #143; otherwise keep downstream blocked.
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
- #158 review packet: `Docs/Handoffs/S1-D-T04-Persistence-Amendment-Repair-Manager-Review-20261029/BROWSER-HANDOFF.md`
- #153 MFM-08 readiness: `Docs/Handoffs/MFM-08-UX-Readiness-20261029/BROWSER-RESULT.md`

Issue #28 verification-harness hygiene remains backlog-only and blocks no current feature.