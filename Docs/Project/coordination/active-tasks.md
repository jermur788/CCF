# Active Task Coordination

This file is the **live single-writer ownership and execution-lock authority** outside the project context snapshot. Completed historical work belongs in Git history, handoffs, Manager reviews and `Docs/Project/unity-project-overview.md`, not as live locks here.

**Last reconciled:** 2026-10-29 by Overall Manager / OpenAI GPT-5.6 Sol at browser Manager review #144 handoff.

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
- **D-066 remains canonical and accepted:** preserve full S1-D FR-B1/AC-3 with the smallest durable historical teaching-sample/review-complete fact; individual `[J]/[K]` choices remain non-persistent; broad T-07 cycle-state persistence remains cancelled.
- Browser amendment #136 is handed off/closed and owns nothing. Browser Manager review #144 found the **amendment implementation contract NEEDS CHANGES** before local production may start: it is too broad (per-cycle keys/counters), does not pin the real response→recruitment write transition including session `RackReviewComplete`, and does not explicitly exclude teaching-only persistence from every world-hash identity path.
- Browser repair #145 is prepared to correct those specification/packet defects. **No S1-D save/schema version is released or owned yet; v24 remains only a conditional expected next version if still free after repaired approval and live refresh.**

## Live / prepared tasks

| Task | Primary / lane | Branch / worktree | Ownership | Restrictions / dependency | Status |
|---|---|---|---|---|---|
| **PLAYER-TEST-OVERNIGHT-01** (#91) | DeepSeek orchestration/integration lead | `orchestration/scenario-one-player-test-ready-v1`; production on child lanes | Orchestration/claims/integration decisions/final assembly | Preserve child ownership; no product invention; no Windows build | **RUNNING** |
| **PLAYER-TEST-T3-CODE-LANE-01** (#106) | Local C#/Unity execution umbrella | child worktrees | Local implementation/qualification through child claims | Exact candidate/handoff; Unity serialized | **RUNNING** |
| **MFM-07 integration** (#125) | T3 Code / DeepSeek integration lane — complete, no active writer | `integration/scenario-one-player-test-ready-v1`; `/media/jer/Videos/CCF-PlayerTestIntegration` | MFM-07 merge/qualification only | No MFM-08/09; no schema/save-validator edit | **INTEGRATED / VERIFIED** — `c8cfae8c4025cec6e960a050edf4bfde2a981de5`; no active writer |
| **S1-C T-04 provenance labelling** (#123) | **Claude Code / Anthropic Claude Sonnet 5.5 Medium** — sole history/review writer | `feature/s1-c-t04-provenance-labels-v1`; `/media/jer/Videos/CCF-S1C-T04` | `AnnualReviewView.cs`, `ManagementHistoryText.cs`, minimum helper | No save/schema/validator, MFM proposal/work-order or simulation edits | **RUNNING / MANAGER REVIEW NEEDS CHANGES** — production ownership remains with #123 until corrected handoff or explicit release |
| **S1-D persistence amendment repair** (#145) | OpenAI GPT-5.6 Sol browser lane preferred | canonical `main`; no local worktree | prospective browser-only ownership of S1-D spec/packet/review/coordination repair surfaces when RUNNING | Must narrow D-066 to one authoritative teaching-completion latch, pin write transition/shared API/hash exclusion; no C#/Unity | **PREPARED FOR DISPATCH — no ownership yet** |
| **S1-D T-04P minimal teaching persistence** (#143) | local save/code lane TBD after repaired approval | `feature/s1-d-t04-teaching-persistence-v1`; `/media/jer/Videos/CCF-S1D-T04-Persistence` | **none** until repaired amendment passes Manager review | Existing #143 packet is not released; no save/schema/hash ownership; v24 not reserved | **BLOCKED ON AMENDMENT REPAIR #145** |
| **S1-D T-04 guide monotonicity** (#124) | Claude Sonnet 5.5 / Anthropic — curriculum writer only when eventually re-released | `feature/s1-d-t04-guide-monotonicity-v1`; `/media/jer/Videos/CCF-S1D-T04` | none while blocked; later curriculum/current-state only, no save/schema/hash | `b9d204c` is unqualified evidence; wait for repaired/approved persistence prerequisite then exact integration | **BLOCKED ON AMENDMENT REPAIR #145** |
| **PARITY-08 save/deterministic compatibility** (#126) | Claude Opus 5.5 Low preferred when re-released | `verify/parity-08-save-compatibility-v1`; `/media/jer/Videos/CCF-Parity08` | no save/validator/hash/reference ownership while blocked | Must be repinned after repaired S1-D persistence + curriculum exact lineage; old v23-only basis obsolete | **BLOCKED ON S1-D AMENDMENT REPAIR #145** |
| **PARITY-08 independent review** (#128) | independent OpenAI review lane | exact future #126 handed-off candidate | **No production ownership; review-only** | Starts only after future repinned #126 hands off exact candidate | **BLOCKED ON #126 AFTER S1-D LINEAGE** |
| **MFM-08A coverage-world adapter** (#129) | OpenAI Sol/Codex or equivalent local Code lane when dispatched | `feature/mfm-v1-coverage-world-adapter-v1`; `/media/jer/Videos/CCF-MFM-08A` | smallest production adapter surface after exact base pin | Pin post-MFM-07 umbrella `c8cfae8c4025cec6e960a050edf4bfde2a981de5`; no UI/teaching/Work Plan/save/calibration | **PREPARED FOR DISPATCH — no ownership yet** |
| **PARITY-03 Q10 evidence strengthening** (#74) | waiting verifier | existing verification lane | verification only | non-blocking; Unity gate later | **WAITING** |

No current row above asserts that the Unity lock is free. Each actual worker must refresh lock/process/free-space state immediately before Unity.

## Single-writer boundaries

### #123 S1-C T-04

History/review provenance labels only. No save/load/validation, `ScenarioOneManager` work-order creation, MFM proposal/approval logic or prescription persistence.

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

### #129 MFM-08A

Bridge authoritative live state into the existing coverage masks/features contract only. If an accepted required mask lacks an authoritative source/API, STOP and report it; do not invent coordinates/radii or silently omit the exclusion.

## Dependency-ready sequence

### S1-C

- T-04 = #123; current Manager review requires corrections.
- T-06 after T-04 is handed off/reviewed/accepted and history ownership is free.
- T-07 after save/identity ownership is free and one exact current integration lineage is pinned.
- T-08 after T-01..T-07 are accepted on one exact candidate.

### S1-D

1. #136 amendment = handed off/closed.
2. #144 Manager review = **NEEDS CHANGES**; no local release.
3. Execute browser repair **#145** and return its result to a new/reused Manager gate.
4. Only after repaired amendment is APPROVED may the corrected T-04P local save/schema packet be released; expected v24 only after live v23/free-writer confirmation.
5. Integrate/qualify that persistence prerequisite, then re-release #124 on the exact lineage; rebase/replay the old candidate rather than stacking blindly.
6. T-05 remains separate and reuses S1-C history.
7. T-08 waits for required S1-D tasks on one exact lineage; T-09 remains independent spec review after T-08 PASS.

### MFM

- MFM-08A = #129, exact post-MFM-07 base `c8cfae8c4025cec6e960a050edf4bfde2a981de5`; dispatch remains subject to Manager release/live ownership.
- MFM-08 map/3D review UX follows #129 + integrated MFM-07 + free UI ownership.
- MFM-09 follows MFM-08 + accepted S1-D T-04/T-05; teaching ownership stays in MFM-09.
- MFM-10 follows MFM-09 + PARITY-08 and preserves Sitka-only quantitative scope.
- MFM-11 follows stable parity/repeated-intervention/player-facing flow.
- MFM-12 follows MFM-11 plus parity/S1-C/S1-D qualification.

### Parity / endgame

- PARITY-08 #126 + independent #128 remain blocked through #145 and the repaired S1-D implementation/integration sequence; do not release against obsolete v23-only assumptions.
- PARITY-09 waits for PARITY-08 PASS + #128 APPROVED + MFM-10.
- Final Tier-3 waits for all required Rack/parity/S1-C/S1-D/MFM integrated qualification on one exact candidate.
- User personal playtest follows qualified candidate; Windows build only after explicit post-playtest authorisation.

## Recently released/completed claims

These own no files: #103 S1-D T-03; #104 S1-C T-05; #93 MFM-04; #100 MFM-05; #102 PARITY-07; #111 MFM-06 implementation; #113 MFM-06 integration; #118 browser lane 03; #120 browser lane 04; #121 MFM-07 implementation; #122 browser lane 05; #127 browser lane 06; #125 MFM-07 integration; #136 S1-D persistence amendment; **#144 S1-D persistence amendment Manager review (NEEDS CHANGES)**.

Browser #127 result: `Docs/Handoffs/PlayerTestReadiness-Overnight-01/BROWSER-LANE-06-RESULT.md`.
Browser #136 result: `Docs/Handoffs/S1-D-T04-Persistence-Amendment-20261029/BROWSER-RESULT.md`.
Browser #144 result: `Docs/Handoffs/S1-D-T04-Persistence-Amendment-Manager-Review-20261029/BROWSER-RESULT.md`.
Browser repair packet: `Docs/Handoffs/S1-D-T04-Persistence-Amendment-Repair-20261029/BROWSER-HANDOFF.md`.

## Non-dispatched backlog

Issue #28, **Verification harness hygiene — standalone body binding and site-seam allow-list**, remains backlog only and blocks no current feature.
