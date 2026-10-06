```text
TASK PACKET
PROJECT: CCF
MANAGER: ChatGPT Game Dev (Overall Manager, D-026)
BASE: origin/main after P1 is integrated (may run in parallel with P2/P3 only
      if WalkingHudView edits are sequenced; otherwise after P3)
CONTEXT COMMIT: latest full context SHA at issue time
WORKTREE: dedicated
BRANCH: task/scenario-one-history-phase1
ROLE: Primary
GOAL: The player can see what changed since their last intervention, where to
      look next, and the history of a place — all from data already saved.
AUTHORITATIVE SOURCES:
- Decision Log D-004, D-020
- Docs/Research/ScenarioOnePedagogy/ForestDiaryConcept.md (Phase 1, causality
  rules §4)
- Docs/Research/ScenarioOnePedagogy/AnnualReviewHistoryIntegration.md
IN SCOPE:
- New read-only helper UI/ForestHistory.cs over ScenarioEcologicalSnapshot,
  ScenarioAnnualReport and ScenarioManagementEvent lists.
- AnnualReviewView: "What changed since your last intervention", "What to
  inspect next" (<=3 items; place + fact; deterministic order), History tab
  (timeline of intervention years + every 5th year; compare two years).
- WalkingHudView ground report and StandMapView side panel: one "History of
  this place" line/expander (events by cell).
OUT OF SCOPE:
- Any save field (Crop-Tree growth since designation, cell regeneration
  history, purpose notes, plots = later packets), charts beyond existing bars,
  causal claims outside the allowed list.
OWNED FILES / SUBSYSTEMS:
- Assets/ForestPrototype/UI/ForestHistory.cs (+ .meta) NEW
- Assets/ForestPrototype/UI/AnnualReviewView.cs, StandMapView.cs,
  WalkingHudView.cs (place line only)
- Tools/Verification/ScenarioOnePedagogy/HistoryVerification.cs NEW
SHARED / LOCKED FILES:
- ScenarioOneManager.cs read-only.
COMPATIBILITY REQUIREMENTS:
- Must tolerate v1-v16 (and v17) saves with missing fields; Reference preview
  shows no player history.
MUST-NOT-CHANGE HASHES / ANCHORS:
- All.
INTENTIONALLY CHANGED HASHES / ANCHORS:
None.
VERIFICATION:
- Batch: scripted 10-year run with two interventions -> history identical
  before/after save/load; place history lists exactly the events for that cell.
- Batch: deny-list (cut/fell/plant/clear/remove/thin as imperatives) absent
  from "inspect next"; forbidden causal patterns absent.
- Batch: legacy v15 save renders a timeline.
- Interactive: MenuTutorial PASS (annual gate unchanged); rendered review at
  three resolutions.
- Anchors unchanged.
STOP AND ASK IF:
A desired history item needs data that is not saved (record it for Phase 2
instead of approximating).
```
