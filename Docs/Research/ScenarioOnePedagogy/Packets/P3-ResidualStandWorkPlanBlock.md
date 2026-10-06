```text
TASK PACKET
PROJECT: CCF
MANAGER: ChatGPT Game Dev (Overall Manager, D-026)
BASE: origin/main after P2 is integrated
CONTEXT COMMIT: latest full context SHA at issue time
WORKTREE: dedicated
BRANCH: task/scenario-one-residual-stand-review
ROLE: Primary
GOAL: Before approving a thinning, the Work Plan shows what the player is
      leaving as well as what they take, in a few plain lines and with no score.
AUTHORITATIVE SOURCES:
- Decision Log D-010 (Work Plan reviews, does not design), D-020
- Docs/Research/ScenarioOnePedagogy/ResidualStandDecision.md (Recommended)
- Docs/Research/ScenarioOnePedagogy/MarkingAssessmentFramework.md
IN SCOPE:
- WorkPlanView: a "WHAT YOU ARE LEAVING" block on the thinning card when
  >=1 open FellTree order exists, from UI/ResidualStandSummary.cs:
  Crop Trees released (count >=10 %, mean change); basal area and standing
  volume before -> after; cells touched and largest connected opening (light
  forecast with the production canopy formula); seed-bearing trees removed;
  removals not within 8 m of any Crop Tree.
- One sentence per line; neutral styling (no good/bad colours).
OUT OF SCOPE:
- Notional value of the retained stand (practice mode only), wind band
  (decision #26), any score/grade, any suggestion of trees, economy changes.
OWNED FILES / SUBSYSTEMS:
- Assets/ForestPrototype/UI/WorkPlanView.cs
- Assets/ForestPrototype/UI/ResidualStandSummary.cs (extend)
- Tools/Verification/ScenarioOnePedagogy/ResidualStandReviewVerification.cs NEW
SHARED / LOCKED FILES:
- ScenarioOneManager.cs read-only (public quote/order accessors only).
COMPATIBILITY REQUIREMENTS:
- Work Plan refresh signature must include marks so the block updates.
MUST-NOT-CHANGE HASHES / ANCHORS:
- All.
INTENTIONALLY CHANGED HASHES / ANCHORS:
None.
VERIFICATION:
- Batch: T2 vs T5 fixture (equal volume) -> different spatial lines.
- Batch: block present for any non-empty plan; both harvest money and
  retained capital present; no field named score/grade/rating.
- Batch: block text identical across two processes.
- Interactive: rendered Work Plan at three resolutions; MenuTutorial PASS.
- Completion/interaction anchors unchanged.
STOP AND ASK IF:
The block needs >6 lines to be understandable, or users report it as advice.
```
