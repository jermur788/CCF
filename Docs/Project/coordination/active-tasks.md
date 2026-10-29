# Active Task Coordination

This file is the **live single-writer ownership and execution-lock authority** outside the project context snapshot. Completed historical work belongs in Git history, handoffs, Manager reviews and `Docs/Project/unity-project-overview.md`, not as live locks here.

**Last reconciled:** 2026-10-29 by Overall Manager / OpenAI GPT-5.6 Sol at browser amendment #136 handoff.

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
- Browser #127 disposed the accepted MFM-07 coverage residual: the coverage core already accepts masks/features, but the production prescription-world adapter passes empty arrays. A bounded prerequisite is required before shipped MFM-08 coverage UX; see #129.
- **D-066 is now canonical:** preserve full S1-D FR-B1/AC-3 with the smallest durable aggregate historical teaching-sample/review-complete record; individual `[J]/[K]` choices remain non-persistent; broad T-07 cycle-state persistence remains cancelled.
- Browser amendment #136 has published its amended spec/packet/result and **owns no production or canonical file surface after handoff**. Its prepared local prerequisite is #143, pending independent browser Manager review before release.

## Live / prepared tasks

| Task | Primary / lane | Branch / worktree | Ownership | Restrictions / dependency | Status |
|---|---|---|---|---|---|
| **PLAYER-TEST-OVERNIGHT-01** (#91) | DeepSeek orchestration/integration lead | `orchestration/scenario-one-player-test-ready-v1`; production on child lanes | Orchestration/claims/integration decisions/final assembly | Preserve child ownership; no product invention; no Windows build | **RUNNING** |
| **PLAYER-TEST-T3-CODE-LANE-01** (#106) | Local C#/Unity execution umbrella | child worktrees | Local implementation/qualification through child claims | Exact candidate/handoff; Unity serialized | **RUNNING** |
| **MFM-07 integration** (#125) | T3 Code / `deepseek-v4.1-flash` (DeepSeek, Opencode Go) — integration complete, no active writer | `integration/scenario-one-player-test-ready-v1`; `/media/jer/Videos/CCF-PlayerTestIntegration` | MFM-07 merge/qualification only | No MFM-08/09; no schema/save-validator edit | **INTEGRATED / VERIFIED** — `c8cfae8c4025cec6e960a050edf4bfde2a981de5`; no active writer |
| **S1-C T-04 provenance labelling** (#123) | **Claude Code / Anthropic Claude Sonnet 5.5 Medium** — sole history/review writer | `feature/s1-c-t04-provenance-labels-v1`; `/media/jer/Videos/CCF-S1C-T04` | `AnnualReviewView.cs`, `ManagementHistoryText.cs`, minimum helper | No save/schema/validator, MFM proposal/work-order or simulation edits | **RUNNING / MANAGER REVIEW NEEDS CHANGES** — production ownership remains with #123 until corrected handoff or explicit release |
| **S1-D T-04P minimal teaching persistence** (#143) | OpenAI Sol/Codex local preferred after release | `feature/s1-d-t04-teaching-persistence-v1`; `/media/jer/Videos/CCF-S1D-T04-Persistence` | prospective sole writer for D-066 minimum save/schema/validation slice only | Prepared against exact production umbrella `c8cfae8c...` / v23; v24 expected only after live start refresh; mandatory independent review; no curriculum edits | **PREPARED — MANAGER REVIEW REQUIRED; no ownership yet** |
| **S1-D T-04 guide monotonicity** (#124) | **Claude Sonnet 5.5 / Anthropic** — sole curriculum writer when re-released | `feature/s1-d-t04-guide-monotonicity-v1`; `/media/jer/Videos/CCF-S1D-T04` | curriculum/current-state files only after #143 integration; no save/schema ownership | Rebase/replay `b9d204c` only after exact #143 integrated/qualified lineage; must land before MFM-09 | **BLOCKED ON #143** — `b9d204c` remains unqualified evidence |
| **PARITY-08 save/deterministic compatibility** (#126) | Claude Opus 5.5 Low preferred primary writer when re-released | `verify/parity-08-save-compatibility-v1`; `/media/jer/Videos/CCF-Parity08` | prospective save/validator/hash/reference writer only after S1-D lineage is exact | Must be repinned after #143 persistence + #124 curriculum lineage; do not start on obsolete v23-only basis | **BLOCKED ON S1-D #143/#124 LINEAGE; no ownership** |
| **PARITY-08 independent review** (#128) | independent OpenAI review lane | exact future #126 handed-off candidate | **No production ownership; review-only** | Starts only after repinned #126 hands off exact candidate; never a second save writer | **BLOCKED ON #126 AFTER S1-D LINEAGE** |
| **MFM-08A coverage-world adapter** (#129) | OpenAI Sol/Codex or equivalent local Code lane when dispatched | suggested `feature/mfm-v1-coverage-world-adapter-v1`; `/media/jer/Videos/CCF-MFM-08A` | smallest production adapter surface after exact base pin | Pin post-MFM-07 umbrella `c8cfae8c4025cec6e960a050edf4bfde2a981de5`; no UI/teaching/Work Plan/save/calibration; do not invent infrastructure geometry | **PREPARED FOR DISPATCH — no ownership yet** |
| **PARITY-03 Q10 evidence strengthening** (#74) | waiting verifier | existing verification lane | verification only | non-blocking; Unity gate later | **WAITING** |

No current row above asserts that the Unity lock is free. Each actual worker must refresh lock/process/free-space state immediately before Unity.

## Single-writer boundaries

### #123 S1-C T-04

History/review provenance labels only. No save/load/validation, `ScenarioOneManager` work-order creation, MFM proposal/approval logic or prescription persistence.

### #143 / #124 S1-D T-04

- #143 is the only prepared D-066 save/schema/validation writer. While PREPARED it owns nothing. If released, it owns only the minimum aggregate teaching-completion persistence, its capture/restore/validation and exact authoritative completion write hook.
- #124 owns no save/schema files. It resumes only after #143 is integrated/qualified and then consumes the shared historical-completion API on a rebase/replay of the curriculum candidate.
- Individual `[J]/[K]` choices, inspected-tree/session transcript and broad current readiness state remain non-persistent.
- T-03 and T-04 must share one historical sample-complete meaning; no second general history/state machine.

### #126 / #128 PARITY-08

#126 cannot become the save/validator/hash/reference writer until the exact post-#143/#124 lineage is known and a Manager packet repins it. #128 remains read-only independent review and starts only after #126 hands off.

### #129 MFM-08A

Bridge authoritative live state into the existing coverage masks/features contract only. Known authoritative manager state includes planted juveniles, clearance patches, shelters, deadwood and work orders. Inspect authoritative infrastructure geometry before mapping headland/track/clearing/site masks. If an accepted required mask lacks an authoritative source/API, STOP and report it; do not invent coordinates/radii or silently omit the exclusion.

## Dependency-ready sequence

### S1-C

- T-04 = #123; current Manager review requires corrections.
- T-06 after T-04 is handed off/reviewed/accepted and history ownership is free.
- T-07 after save/identity ownership is free and one exact current integration lineage is pinned.
- T-08 after T-01..T-07 are accepted on one exact candidate.

### S1-D

1. Browser amendment #136 = **HANDED OFF; no ownership**.
2. Independent browser Manager review of #136 decides APPROVED / NEEDS CHANGES / REJECTED.
3. If APPROVED, release **T-04P #143** as the sole save/schema lane from the exact qualified post-MFM-07 base, subject to live v23/free-writer refresh.
4. Integrate/qualify #143, then re-release **T-04 #124** on that exact lineage; rebase/replay the old candidate rather than stacking blindly.
5. T-05 remains separate and reuses S1-C history.
6. T-08 waits for amended required implementation tasks on one lineage; T-09 remains independent spec review after T-08 PASS.

### MFM

- MFM-08A = #129, exact post-MFM-07 base `c8cfae8c4025cec6e960a050edf4bfde2a981de5`; dispatch remains subject to Manager release and live ownership.
- MFM-08 map/3D review UX follows #129 + integrated MFM-07 + free UI ownership.
- MFM-09 follows MFM-08 + accepted S1-D T-04/T-05; teaching ownership stays in MFM-09.
- MFM-10 follows MFM-09 + PARITY-08 and preserves Sitka-only quantitative scope.
- MFM-11 follows stable parity/repeated-intervention/player-facing flow.
- MFM-12 follows MFM-11 plus parity/S1-C/S1-D qualification.

### Parity / endgame

- PARITY-08 #126 + review #128 remain blocked until the exact S1-D #143/#124 save/curriculum lineage is established and repinned.
- PARITY-09 waits for PARITY-08 PASS + #128 APPROVED + MFM-10.
- Final Tier-3 waits for all required Rack/parity/S1-C/S1-D/MFM integrated qualification on one exact candidate.
- User personal playtest follows qualified candidate; Windows build only after explicit post-playtest authorisation.

## Recently released/completed claims

These own no files: #103 S1-D T-03; #104 S1-C T-05; #93 MFM-04; #100 MFM-05; #102 PARITY-07; #111 MFM-06 implementation; #113 MFM-06 integration; #118 browser lane 03; #120 browser lane 04; #121 MFM-07 implementation; #122 browser lane 05; #127 browser lane 06; #125 MFM-07 integration; **#136 S1-D persistence amendment**.

Browser #127 result: `Docs/Handoffs/PlayerTestReadiness-Overnight-01/BROWSER-LANE-06-RESULT.md`.
Browser #136 result: `Docs/Handoffs/S1-D-T04-Persistence-Amendment-20261029/BROWSER-RESULT.md`.

## Non-dispatched backlog

Issue #28, **Verification harness hygiene — standalone body binding and site-seam allow-list**, remains backlog only and blocks no current feature.
