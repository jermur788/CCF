```text
TASK PACKET
PROJECT: CCF
MANAGER: ChatGPT Game Dev (Overall Manager, D-026)
BASE: origin/main at issue time (>= 3e4ee40)
CONTEXT COMMIT: latest full context SHA at issue time (must include any
      accepted entries from CanonicalUpdateProposal.md)
WORKTREE: dedicated (e.g. /home/jer/CCF-teaching-copy)
BRANCH: task/scenario-one-teaching-copy
ROLE: Primary
GOAL: A first-time player is told what CCF is, why they start by choosing
      trees to keep, and what each tool really does — with no change to
      simulation, save or objectives.
AUTHORITATIVE SOURCES:
- Game Brief, Decision Log (D-003, D-004, D-010, D-013, D-020, D-021),
  Current Milestone, Overview, AgentWorkflow
- Docs/Research/ScenarioOnePedagogy/TutorialCopyBank.md (approved subset only)
- Docs/Research/ScenarioOnePedagogy/ForestryTerminologyAudit.md
- Docs/Research/ScenarioOnePedagogy/CurrentTutorialAudit.md
IN SCOPE (strings and presentation only):
- An opening card (S0) shown once per profile (reuse the MenuHelpView
  first-use mechanism): CCF definition, what the player owns, "start with the
  trees you want to keep".
- Rewrite of the 5 menu introductions per the copy bank (shorter HUD intro;
  F5/F9 save/load mentioned).
- Learning-objective step texts revised for positive selection and honest
  clearance; rename the checklist "Lessons" in UI labels (internal ids and
  PlayerPrefs keys UNCHANGED).
- Ground "Why" line names seed limitation where every regenerating
  species' seed rain is zero (derivable; ScenarioOneUiFacts).
- Clearance preview label and Work Plan clearance card name the young trees
  affected; Annual Review "regeneration cohort(s) removed" -> plain wording.
- Marking messages use "to fell" (not "harvest"); species display names
  instead of ids in the objectives list.
- One explanatory line on the thinning card about early thinnings not paying
  under the contractor minimum (factual; no change to the economy).
- One line explaining why both broadleaves must be planted (no seed source on
  the property) beside the objective.
OUT OF SCOPE:
- Objective logic, ScenarioOneManager (including the dead TutorialHint text,
  which stays until Sol's manager changes are integrated), ForestPlayer
  behaviour (preview-on-U is decision #9 and a separate change), new state,
  save fields, economy values, ecology, scenes, prefabs.
OWNED FILES / SUBSYSTEMS:
- Assets/ForestPrototype/UI/MenuHelpView.cs, LearningObjectivesView.cs (text),
  WalkingHudView.cs, TreeInspectionView.cs, StandMapView.cs, WorkPlanView.cs,
  AnnualReviewView.cs, ScenarioOneUiFacts.cs
- Assets/ForestPrototype/ForestTreeMarkingManager.cs (message strings only)
- Assets/ForestPrototype/ScenarioOne/ClearancePreview.cs (Label string only)
- Tools/Verification/MenuTutorialVerification.cs (string expectations, if any)
SHARED / LOCKED FILES:
- Do not touch ScenarioOneManager.cs, save files, ForestEcologyController.cs
  (Sol's lock).
COMPATIBILITY REQUIREMENTS:
- PlayerPrefs keys CCF.MenuHelp.v1.* and CCF.Learning.v1.* unchanged.
- Save v16/v17 unaffected. Reference Future v1 untouched.
MUST-NOT-CHANGE HASHES / ANCHORS:
- All lifecycle, completion and Reference anchors recorded in the Overview.
INTENTIONALLY CHANGED HASHES / ANCHORS:
None.
VERIFICATION:
- Import/compile 0 errors.
- MenuTutorialVerification INTERACTIVE PASS (all markers), with any updated
  string expectations.
- ClearanceVerification INTERACTIVE PASS.
- ScenarioOneCompletionVerification batch PASS with unchanged anchor.
- String tests (new disposable harness or added to MenuTutorialVerification):
  the opening card contains the CCF definition; the clearance label names young
  trees; no clearance text claims a seedling benefit; no prescriptive phrase.
- Rendered capture review at 1280x720, 1600x900 and 1920x1080 (text fits).
STOP AND ASK IF:
Any requested wording requires a new mechanic or claims ecology the
simulation lacks, or the user has not approved the copy subset.
```

**Expected player-visible result:** on a fresh profile the opening card explains CCF in three sentences. Prompts, help and lessons start with keeping trees. Clearance text names the young trees it removes. The dark-ground "Why" explains that there is no seed yet, as well as the darkness.
