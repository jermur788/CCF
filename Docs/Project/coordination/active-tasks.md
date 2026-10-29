# Active Task Coordination

This file is the **live single-writer ownership and execution-lock authority** outside the project context snapshot. Completed historical work belongs in Git history, handoffs, Manager reviews and `Docs/Project/unity-project-overview.md`, not as live locks here.

**Last reconciled:** 2026-10-29 by Overall Manager / OpenAI GPT-5.6 Sol after S1-D T-05 browser readiness #149; T-05 Code #151 is prepared but blocked on #147 history/review ownership.

## Coordination rules

- Overall Manager or assigned integrator maintains this file.
- Every worker with GitHub access refreshes this file and its claim at task start and before asserting a lock is absent.
- One writable lane/worktree per owner and one primary writer per subsystem/file set.
- Never stash, reset, discard, clean, overwrite or absorb unrelated dirty work.
- Overlap means stop before editing and return to Manager for ownership/API disposition.
- A task claim records execution presence only; it **does not grant ownership** or override this file.
- Heavy Unity work is serialized machine-wide. Verify branch/HEAD/ownership before acquiring the Unity lock; never bypass another owner or kill its process.
- No Windows build before the user's personal Scenario One playtest and explicit authorisation.

## Current integration truth

- MFM-06 #113: **INTEGRATED / VERIFIED for its bounded gate** at exact production umbrella `b971f4da0540e606bfee9702b0c99f81a0a89355`.
- Published `integration/scenario-one-player-test-ready-v1` head at browser #127 refresh: `7608f02cb8da1fabd8b78fd5aaf45b0c32086f67`, a documentation-only successor of the MFM-06 qualified production merge.
- MFM-07 #121: **IMPLEMENTED + MANAGER-REVIEWED/APPROVED as a candidate**, exact production candidate `d4188b33e58b0fe67504a87167692d01b226c84e`, docs-only successor `01755c47d537202e9cf479545be527d43121298f`.
- MFM-07 #125: **INTEGRATED / VERIFIED for its bounded gate**. Pre-integration umbrella `7608f02cb8da1fabd8b78fd5aaf45b0c32086f67` + exact input `d4188b33` merged `--no-ff` with no conflicts into **`c8cfae8c4025cec6e960a050edf4bfde2a981de5`**; handoff published at **`6b84f0d9d044ef76fd935b20b8d4546e8162b0ab`**. Post-merge compile, MFM-07 approval 106, MFM-06 save/load 159, 18-line protected hash dump (no rebaseline), and interaction gates all PASS.
- **Post-MFM-07 integrated umbrella SHA: `c8cfae8c4025cec6e960a050edf4bfde2a981de5`** (`integration/scenario-one-player-test-ready-v1`; save schema v23).
- S1-C T-04 #123 is **IMPLEMENTED + UNITY-QUALIFIED + MANAGER-APPROVED as a candidate** at final exact production SHA **`633775fd8650db3cf3482e550d106a0febd7b9cb`**. Pushed successor `3ea4af8b` changes only the final handoff after that production SHA. The earlier `ba2922e...` / `cff79f0...` approval lineage is superseded. #123 remains closed and owns no files.
- Browser #127 disposed the accepted MFM-07 coverage residual: the coverage core already accepts masks/features, but the production prescription-world adapter passes empty arrays. #129 source inspection correctly STOPPED before code because mask/runtime authority was incomplete. Browser source-authority review #146 is prepared to settle Track/Clearing/Site/player-start semantics before #129 resumes.
- **D-066 remains canonical and accepted:** preserve full S1-D FR-B1/AC-3 with the smallest durable historical teaching-sample/review-complete fact; individual `[J]/[K]` choices remain non-persistent; broad T-07 cycle-state persistence remains cancelled.
- Browser amendment #136 is handed off/closed and owns nothing. Browser Manager review #144 found the **amendment implementation contract NEEDS CHANGES** before local production may start: it is too broad (per-cycle keys/counters), does not pin the real response→recruitment write transition including session `RackReviewComplete`, and does not explicitly exclude teaching-only persistence from every world-hash identity path.
- Browser repair #145 is prepared to correct those specification/packet defects. **No S1-D save/schema version is released or owned yet; v24 remains only a conditional expected next version if still free after repaired approval and live refresh.**
- S1-D T-05 readiness #149 inspected the integrated MFM-07 pending/provenance boundary and current S1-C history surface. **Verdict: READY AFTER #147.** Generic readiness grouping can live in `WorkPlanView.cs`, but faithful exact-year Annual Review navigation needs the existing S1-C review presentation seam; #151 is prepared with no ownership until #147 releases it.

## Live / prepared tasks

| Task | Primary / lane | Branch / worktree | Ownership | Restrictions / dependency | Status |
|---|---|---|---|---|---|
| **PLAYER-TEST-OVERNIGHT-01** (#91) | DeepSeek orchestration/integration lead | `orchestration/scenario-one-player-test-ready-v1`; production on child lanes | Orchestration/claims/integration decisions/final assembly | Preserve child ownership; no product invention; no Windows build | **RUNNING** |
| **PLAYER-TEST-T3-CODE-LANE-01** (#106) | Local C#/Unity execution umbrella | child worktrees | Local implementation/qualification through child claims | Exact candidate/handoff; Unity serialized | **RUNNING** |
| **MFM-07 integration** (#125) | T3 Code / DeepSeek integration lane — complete, no active writer | `integration/scenario-one-player-test-ready-v1`; `/media/jer/Videos/CCF-PlayerTestIntegration` | MFM-07 merge/qualification only | No MFM-08/09; no schema/save-validator edit | **INTEGRATED / VERIFIED** — `c8cfae8c4025cec6e960a050edf4bfde2a981de5`; no active writer |
| **S1-C T-06 prior-treatment guidance** (#147) | **T3 Code / Codex gpt-6.1-sol, medium reasoning** | `feature/s1-c-t06-prior-treatment-guidance-v1`; `/media/jer/Videos/CCF-S1C-T06` | sole bounded history/review presentation writer after safe repin | Worker recorded clean start `cff79f0...`, but final T-04 is direct descendant `633775fd...` with docs successor `3ea4af8b`; must non-destructively repin before further production edits; no persistence/curriculum/simulation/MFM | **RUNNING — history/review presentation owner** |
| **S1-D T-05 previous vs proposed presentation** (#151) | local Code lane TBD after #147 | `feature/s1-d-t05-previous-proposed-v1`; `/media/jer/Videos/CCF-S1D-T05` | **none** while blocked; later `WorkPlanView.cs` + minimal existing Annual Review exact-year hook only | #147 must hand off/release history/review first; Manager must inspect returned API and repin exact combined lineage; no MFM-09 teaching/save/schema | **READY AFTER #147 — BLOCKED, NO OWNERSHIP** |
| **S1-D persistence amendment repair** (#145) | OpenAI GPT-5.6 Sol browser lane preferred | canonical `main`; no local worktree | prospective browser-only ownership of S1-D spec/packet/review/coordination repair surfaces when RUNNING | Must narrow D-066 to one authoritative teaching-completion latch, pin write transition/shared API/hash exclusion; no C#/Unity | **PREPARED FOR DISPATCH — no ownership yet** |
| **S1-D T-04P minimal teaching persistence** (#143) | local save/code lane TBD after repaired approval | `feature/s1-d-t04-teaching-persistence-v1`; `/media/jer/Videos/CCF-S1D-T04-Persistence` | **none** until repaired amendment passes Manager review | Existing #143 packet is not released; no save/schema/hash ownership; v24 not reserved | **BLOCKED ON AMENDMENT REPAIR #145** |
| **S1-D T-04 guide monotonicity** (#124) | Claude Sonnet 5.5 / Anthropic — curriculum writer only when eventually re-released | `feature/s1-d-t04-guide-monotonicity-v1`; `/media/jer/Videos/CCF-S1D-T04` | none while blocked; later curriculum/current-state only, no save/schema/hash | `b9d204c` is unqualified evidence; wait for repaired/approved persistence prerequisite then exact integration | **BLOCKED ON AMENDMENT REPAIR #145** |
| **PARITY-08 save/deterministic compatibility** (#126) | Claude Opus 5.5 Low preferred when re-released | `verify/parity-08-save-compatibility-v1`; `/media/jer/Videos/CCF-Parity08` | no save/validator/hash/reference ownership while blocked | Must be repinned after repaired S1-D persistence + curriculum exact lineage; old v23-only basis obsolete | **BLOCKED ON S1-D AMENDMENT REPAIR #145** |
| **PARITY-08 independent review** (#128) | independent OpenAI review lane | exact future #126 handed-off candidate | **No production ownership; review-only** | Starts only after future repinned #126 hands off exact candidate | **BLOCKED ON #126 AFTER S1-D LINEAGE** |
| **MFM-08A source-authority browser review** (#146) | OpenAI GPT-5.6 Sol browser/GitHub lane preferred | canonical `main`; no local worktree | browser-only MFM authority/spec/packet/coordination reconciliation when RUNNING | Settle Track/Clearing/Site/player-start authority; no production C#/Unity | **PREPARED FOR DISPATCH — no ownership yet** |
| **MFM-08A coverage-world adapter** (#129) | OpenAI Sol/Codex or equivalent local Code lane after #146 | `feature/mfm-v1-coverage-world-adapter-v1`; `/media/jer/Videos/CCF-MFM-08A` | no production ownership while blocked; existing clean worktree may remain in place | Exact base `c8cfae8c4025cec6e960a050edf4bfde2a981de5`; do not invent geometry; no UI/teaching/Work Plan/save/calibration | **BLOCKED ON SOURCE-AUTHORITY REVIEW #146** |
| **PARITY-03 Q10 evidence strengthening** (#74) | waiting verifier | existing verification lane | verification only | non-blocking; Unity gate later | **WAITING** |

No current row above asserts that the Unity lock is free. Each actual worker must refresh lock/process/free-space state immediately before Unity.

## Single-writer boundaries

### #147 / #151 S1-C history and S1-D T-05

#147 is the sole bounded history/review presentation writer while it is running. #151 has no production ownership until #147 hands off and Manager review disposes the returned API/diff. T-05's generic status grouping belongs primarily in `WorkPlanView.cs`; its only permitted S1-C write after release is the smallest exact-year Annual Review opening hook needed to navigate to an already-recorded `HistoryYears` year. If #147 already returns such a public read-only hook, #151 must reuse it and not edit `AnnualReviewView.cs`. No duplicate history store/view, no new persisted date/provenance field, and no inferred past location.

T-05 classification is fixed to existing ordinary order records: Approved = open `status == Approved`; Pending new = `status == Pending && createdYear == CurrentEcologicalYear`; Pending carried over = `status == Pending && createdYear < CurrentEcologicalYear`. MFM prescription approval creates ordinary Pending work and never counts as Work Plan Approved by itself.

### #145 / #143 / #124 S1-D T-04

- #145 is browser-only repair and owns no production files.
- #143 has **no ownership** until the repaired amendment is separately Manager-approved and #143 is explicitly re-released. The existing prepared packet must not be executed as written.
- Repaired authority is expected to persist only one monotone historical latch for the existing first guided sample/review unless fresh inspected evidence proves a broader model strictly necessary; any broader need returns to Manager.
- The latch must be written only at the genuine pre-amendment rack-world `response` → `recruitment` transition after actual session `RackReviewComplete` and preceding lesson conditions pass; the existing T-03 helper alone is insufficient to prove that transition.
- Before the latch, T-03 retains existing in-session/current-world cap semantics. After the latch, T-03 and T-04 consume one durable historical-completion meaning.
- Teaching-only persistence must be excluded from world identity/hash paths with no rebaseline.
- #124 never owns save/schema/hash files and resumes only after the repaired #143 prerequisite is accepted/integrated.

### #126 / #128 PARITY-08

#126 cannot become the save/validator/hash/reference writer until repaired S1-D persistence is implemented/integrated, #124 is on the exact resulting lineage, and a Manager packet repins PARITY-08. #128 remains read-only independent review.

### #146 / #129 MFM-08A

#146 is browser-only and owns no production code. It must settle the authoritative runtime meaning/source for Track/Clearing/Site masks and the moving-player versus durable-player-start ambiguity before #129 resumes. #129 may bridge only authoritative live state into the existing coverage masks/features contract. If an accepted required source remains unavailable after #146, STOP; do not invent coordinates/radii or silently omit an exclusion.

## Dependency-ready sequence

### S1-C

- T-04 = #123: **FINAL MANAGER-APPROVED candidate `633775fd8650db3cf3482e550d106a0febd7b9cb`**, pushed docs-only successor `3ea4af8b`; claim closed, history/review ownership released.
- T-06 = #147: **RUNNING** as the sole bounded history/review presentation writer. T-05 #151 waits for its handoff/release before any shared presentation write.
- T-07 waits until save/identity ownership is free and one exact current integration lineage is pinned.
- T-08 after T-01..T-07 are accepted on one exact candidate.

### S1-D

1. #136 amendment = handed off/closed.
2. #144 Manager review = **NEEDS CHANGES**; no local release.
3. Execute browser repair **#145** and return its result to a new/reused Manager gate.
4. Only after repaired amendment is APPROVED may the corrected T-04P local save/schema packet be released; expected v24 only after live v23/free-writer confirmation.
5. Integrate/qualify that persistence prerequisite, then re-release #124 on the exact lineage; rebase/replay the old candidate rather than stacking blindly.
6. T-05 = #151: packet/API mapping ready, **blocked only on #147 history/review ownership release plus Manager exact-lineage repin**; it reuses S1-C history and does not add persistence.
7. T-08 waits for required S1-D tasks on one exact lineage; T-09 remains independent spec review after T-08 PASS.

### MFM

- MFM-08A source authority = #146, browser-capable and prepared.
- MFM-08A implementation = #129, exact post-MFM-07 base `c8cfae8c4025cec6e960a050edf4bfde2a981de5`, **blocked until #146** republishes faithful source authority/packet.
- MFM-08 map/3D review UX follows #129 + integrated MFM-07 + free UI ownership.
- MFM-09 follows MFM-08 + accepted S1-D T-04/T-05; **no active MFM-09 writer is registered.** T-05 owns only generic Approved/Pending-new/Pending-carried-over readiness/history navigation; MFM-09 retains `Extend your treatment` teaching, prescription-specific Work Plan teaching/grouping and candidate/proposal/map UX.
- MFM-10 follows MFM-09 + PARITY-08 and preserves Sitka-only quantitative scope.
- MFM-11 follows stable parity/repeated-intervention/player-facing flow.
- MFM-12 follows MFM-11 plus parity/S1-C/S1-D qualification.

### Parity / endgame

- PARITY-08 #126 + independent #128 remain blocked through #145 and the repaired S1-D implementation/integration sequence; do not release against obsolete v23-only assumptions.
- PARITY-09 waits for PARITY-08 PASS + #128 APPROVED + MFM-10.
- Final Tier-3 waits for all required Rack/parity/S1-C/S1-D/MFM integrated qualification on one exact candidate.
- User personal playtest follows qualified candidate; Windows build only after explicit post-playtest authorisation.

## Recently released/completed claims

These own no files: #103 S1-D T-03; #104 S1-C T-05; #93 MFM-04; #100 MFM-05; #102 PARITY-07; #111 MFM-06 implementation; #113 MFM-06 integration; #118 browser lane 03; #120 browser lane 04; #121 MFM-07 implementation; #122 browser lane 05; #127 browser lane 06; #125 MFM-07 integration; #136 S1-D persistence amendment; #144 S1-D persistence amendment Manager review (NEEDS CHANGES); **#123 S1-C T-04 provenance labelling (FINAL APPROVED candidate `633775fd`)**; #149 S1-D T-05 browser readiness (**READY AFTER #147**).

Browser #127 result: `Docs/Handoffs/PlayerTestReadiness-Overnight-01/BROWSER-LANE-06-RESULT.md`.
Browser #136 result: `Docs/Handoffs/S1-D-T04-Persistence-Amendment-20261029/BROWSER-RESULT.md`.
Browser #144 result: `Docs/Handoffs/S1-D-T04-Persistence-Amendment-Manager-Review-20261029/BROWSER-RESULT.md`.
Browser #149 result: `Docs/Handoffs/S1-D-T05-Readiness-20261029/BROWSER-RESULT.md`.
Browser repair packet: `Docs/Handoffs/S1-D-T04-Persistence-Amendment-Repair-20261029/BROWSER-HANDOFF.md`.
MFM-08A source-authority packet: `Docs/Handoffs/MFM-08A-Source-Authority-Review-20261029/BROWSER-HANDOFF.md`.

## Non-dispatched backlog

Issue #28, **Verification harness hygiene — standalone body binding and site-seam allow-list**, remains backlog only and blocks no current feature.