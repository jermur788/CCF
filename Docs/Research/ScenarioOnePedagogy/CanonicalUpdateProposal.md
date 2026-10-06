# Canonical update proposal (for Manager review)

**This file changes nothing.** It lists exact text the Overall Manager may apply to canonical files after review. Decision IDs are placeholders (`D-NEW-n`), because other workers (e.g. Sol's `CanonicalDecisionProposal.md`) may propose decisions concurrently. The Manager assigns real numbers.

---

## 1. `Docs/Project/research-index.md` — add a section

```markdown
## European Pro Silva CCF Practice Synthesis

`European_Pro_Silva_CCF_Practice_Synthesis.md` (6 October 2026, including the
priority-source deep dive; local copy `~/Documents/CCF Game/`, SHA-256 prefix
`e8c961081c5df035`; not committed to Git). **Supporting practitioner evidence,
not decision authority.** Practitioner guidance is kept distinct from
empirical ecology: where they conflict, the CCF Primary Literature Synthesis v1
and the dedicated Irish reports take precedence for biological calibration.
Numeric thresholds from Finnish, Swiss, French, Walloon and Spanish practice
are not Scenario One parameters; Irish Pro Silva guidance has the highest
operational relevance but is not biological calibration.

Used by the Scenario One pedagogy research
(`Docs/Research/ScenarioOnePedagogy/`) for: positive selection (quality tree
first, then competitors), the "do not clean up" lesson, repeated light
interventions, marteloscope-style training, residual-stand evaluation,
permanent monitoring with recorded interventions, and the Bio Tree concept.
```

## 2. `Docs/Project/decision-log.md` — proposed rows (Status: Proposed or Open)

| ID | Date | Topic | Status | Decision or position | Implication / notes |
|---|---|---|---|---|---|
| D-NEW-1 | 2026-10-07 | Teaching principle: positive selection | Proposed | Scenario One teaching starts marking with the trees to keep (Crop Trees), then their real competitors; suppression alone is not presented as a reason to remove a tree. | Research: `Docs/Research/ScenarioOnePedagogy/PositiveSelectionTeaching.md`. No mechanic change. |
| D-NEW-2 | 2026-10-07 | Tutorial and marking assessment | Proposed | No universal CCF/sustainability score. Reviews show economic, silvicultural, ecological and stability dimensions separately; lessons complete on observed actions, not on "correct" answers. | Consistent with the game brief ("no universally correct prescription"). `DecisionMatrix.md` §2.5. |
| D-NEW-3 | 2026-10-07 | Learning progress scope | Open | Per-forest stage progress (save) vs current per-device PlayerPrefs. | Save-schema change; sequence after the growth-model save version. |
| D-NEW-4 | 2026-10-07 | Practice (marteloscope) mode | Open | Recommended: a reusable practice sandbox, Year-0 copy first, then a copy of the current forest; no save writes; no score. | `MarteloscopeDecision.md`. Touches manager/save; after the growth-model integration. |
| D-NEW-5 | 2026-10-07 | Bio Tree designation | Deferred | No new mark until at least one habitat-tree state exists. | `BioTreeDecision.md`. |
| D-NEW-6 | 2026-10-07 | Forest Diary | Proposed (Phase 1) / Open (Phase 2) | Phase 1: history derived from existing saved snapshots, reports and events (no save change). Phase 2: optional permanent sample plots (save). | `MonitoringDecision.md`. |
| D-NEW-7 | 2026-10-07 | Gate run modes | Proposed | ClearanceVerification and MenuTutorialVerification require an interactive Editor; the integration launcher records and enforces each gate's mode. | `BaselineRedGateInvestigation.md`. |
| D-NEW-8 | 2026-10-07 | Clearance teaching honesty | Proposed | Until causal understorey competition exists, player text must not claim that clearance helps seedlings; clearance text names the young trees it removes. | D-020, D-024, D-047 open item. |

## 3. `Docs/Project/current-milestone.md`

In **Regeneration Model 1 › Open/deferred**, replace the line

> - pre-existing harness failures (Clearance, MenuTutorial, Removal) reproduce identically on `402a2b4`, recorded separately.

with (only after the harness commits are reviewed and integrated):

> - The three pre-existing red gates were diagnosed (`Docs/Research/ScenarioOnePedagogy/BaselineRedGateInvestigation.md`). Removal was stale against the accepted area-clearance contract and now passes in batch. Clearance was stale against the first-use HUD introduction and needs an interactive Editor; it passes interactively. MenuTutorial needs an interactive Editor; it passes interactively. No product defect was found.

Add a short note:

> **Scenario One pedagogy research** (`task/scenario-one-pedagogy-overnight`): audit, learning architecture, practice-mode and residual-stand designs, Forest Diary design, gate diagnosis and implementation packets. Proposals only; awaiting Manager review. Not a change to current scope.

## 4. `Docs/Project/unity-project-overview.md` — Verification section additions

```markdown
Gate modes (verified 2026-10-07 at `3e4ee40` + harness fixes):
ClearanceVerification and MenuTutorialVerification need an interactive
(rendered) Editor: in -batchmode `Cursor.lockState` cannot become Locked and
keyboard input does not reach play mode. Both harnesses fail fast with that
message in batch mode. ScenarioOneRemovalVerification runs in batch.

In-memory restore timing: `ForestSaveController.CaptureData` enumerates trees
including inactive ones, and `LoadData` destroys trees absent from the save at
frame end. Harnesses (and any future practice/sandbox mode) must restore from
a serialized copy and let at least one frame pass before capturing or
enumerating. `Tools/Verification/ScenarioOnePedagogy/ResidualStandEvaluation`
demonstrates the pattern; its two-process outputs are byte-identical.

Presentation-calibration observations at `3e4ee40` (RNG 1, regeneration 1):
at Year 0 every plantation tree reads "high" wind exposure (diagnostic 17–30
against thresholds 5/12) and 328 of 336 read "crowded" competition. The whole
stand's notional roadside value (~€1,702) is below the €2,500 harvest
minimum.
```

## 5. `Docs/Project/coordination/active-tasks.md` (Manager-maintained, live)

Suggested row:

| Task | Primary | Branch | Worktree | Owned files/subsystems | Shared restriction | Status |
|---|---|---|---|---|---|---|
| Scenario One pedagogy / marteloscope research (overnight) | Claude | task/scenario-one-pedagogy-overnight | /home/jer/CCF-pedagogy | Docs/Research/ScenarioOnePedagogy, Tools/Verification/ScenarioOnePedagogy, harness fixes to Clearance/MenuTutorial/Removal verification | No production, save, ecology or Reference changes | Complete; pushed; awaiting Manager review |

## 6. Not proposed

- No change to `game-brief.md`. The work strengthens existing principles ("make sustainability understandable", "preserve multiple credible strategies", "no universally correct prescription") without changing direction.
