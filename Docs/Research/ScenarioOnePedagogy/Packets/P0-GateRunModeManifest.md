```text
TASK PACKET
PROJECT: CCF
MANAGER: ChatGPT Game Dev (Overall Manager, D-026)
BASE: origin/main at issue time (>= 3e4ee40), plus the harness fixes on
      task/scenario-one-pedagogy-overnight once reviewed
CONTEXT COMMIT: latest full context SHA at issue time
WORKTREE: dedicated (e.g. /home/jer/CCF-gate-modes)
BRANCH: task/verification-gate-run-modes
ROLE: Primary
GOAL: Integration regressions run every Unity gate in the mode it needs, so
      interactive-only gates are never reported red because of batch mode.
AUTHORITATIVE SOURCES:
- AgentWorkflow, Unity Project Overview (Verification section)
- Docs/Research/ScenarioOnePedagogy/BaselineRedGateInvestigation.md
IN SCOPE:
- A small machine-readable manifest of gate -> mode (batch | interactive |
  rendered-capture), read by Tools/Verification/run_clearance_gate.py (or a
  thin wrapper), which refuses to run a gate in the wrong mode.
- Initial entries: ClearanceVerification=interactive,
  MenuTutorialVerification=interactive, ScenarioOnePresentationReview=rendered,
  all other current Tools/Verification gates=batch.
- Document the modes in the Overview's Verification section (proposed text
  in CanonicalUpdateProposal.md; Manager applies it).
OUT OF SCOPE:
- Any Assets/ change; any gate logic change beyond reading the manifest.
OWNED FILES / SUBSYSTEMS:
- Tools/Verification/run_clearance_gate.py, new Tools/Verification/gate-modes.json
SHARED / LOCKED FILES:
- None in Assets. Coordinate with whoever runs integration regressions.
COMPATIBILITY REQUIREMENTS:
- Existing anchor checks in the launcher unchanged.
MUST-NOT-CHANGE HASHES / ANCHORS:
- All (no simulation change).
INTENTIONALLY CHANGED HASHES / ANCHORS:
None.
VERIFICATION:
- Launch ClearanceVerification in batch -> launcher refuses with a clear message.
- Launch ClearanceVerification and MenuTutorialVerification interactive -> PASS.
- Launch ScenarioOneRemovalVerification batch -> PASS.
STOP AND ASK IF:
The launcher is also used by another active worker in a way the manifest
would break.
```

**Why now:** three gates were reported "pre-existing red" during the Regeneration Model 1 integration. Two of them were batch-mode artefacts. A permanently red gate hides real regressions.
