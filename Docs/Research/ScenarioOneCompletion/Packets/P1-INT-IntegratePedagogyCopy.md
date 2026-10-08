# P1-INT — Integrate pedagogy P0+P1 copy onto main

```text
TASK PACKET
GOAL: Main teaches what CCF is, starts with positive selection, and makes no
      unsupported clearance claim; harness run-mode fixes land on main.
BRANCH: integration/pedagogy-p1 (from current origin/main)
ROLE: Integrator (independent of the P1 author if possible)
```

**Start authority:** Manager approval of `task/scenario-one-pedagogy-p1` @ `1a36ea9` (P1 record: ready for review). Base: current `origin/main` (≥ `a8596df`).

**Scope**

1. Merge or cherry-pick `ff6b887` (P0 gate modes), `db21642` (harness fixes) and `1a36ea9` (P1 copy) onto main. `git merge-tree` at `a8596df` reports no textual conflicts.
2. Update `ScenarioOneTeachingCopyVerification` expectations to save **v17** (`CCF_EXPECTED_SAVE_VERSION=17` or the equivalent).
3. If Model 2 has already integrated (v18): apply the model-dependent clearance sentence (`RegenerationModel2Teaching.md` §4). Otherwise keep P1's Model 1 sentence.

**Owned files:** the P1 file set (`UI/*.cs` listed in the P1 record, `ForestTreeMarkingManager.cs` strings, `ClearancePreview.cs`, `VegetationClearance.cs` labels), `Tools/Verification/{ScenarioOneTeachingCopyVerification, ClearanceVerification, MenuTutorialVerification, ScenarioOneRemovalVerification}.cs`, `Docs/Verification/ScenarioOneGateModes.md`, P1 docs and captures.

**Locked:** `ScenarioOneManager.cs`, save files, `ForestEcologyController.cs`, objectives, economy, scenes, prefabs.

**Exclusions:** no new copy beyond P1; no layout changes.

**Save impact:** none.

**Test plan:** Teaching copy (batch + interactive rendered at 1280/1600/1920); Clearance (interactive); MenuTutorial (interactive); Removal (batch); Completion (batch, `D7C4DDD36B53FCCE` unchanged); Reference (batch); Interaction (batch).

**Manual review:** fresh-profile walkthrough of the five help screens; forecast line readable (expected: still small; P2 fixes it).

**Stop conditions:** any anchor drift; conflict with a concurrent Sol commit in a P1 file; the copy gate needing more than a version expectation change.

**Handoff:** IMPLEMENTATION HANDOFF with gate table and anchors.
