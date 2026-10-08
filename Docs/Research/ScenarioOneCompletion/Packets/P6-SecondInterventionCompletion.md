# P6 — Second intervention, completion model, failure and recovery

```text
TASK PACKET
GOAL: Scenario One completes when the player has managed the forest through two
      selective cycles while keeping continuous cover, standing capital and
      renewal; nothing fails silently; the second-look trigger fires from the
      forest's state.
BRANCH: task/scenario-one-p6-completion
ROLE: Primary (single writer for manager/objectives/save during the packet);
      independent review required (completion anchor + save).
```

**Start authority:** Storms integrated (or explicitly deferred), with no active Sol writer on the manager/save; decisions **N1, N2, L2, L3, O1, M1** (and O2 if a recovery route is chosen). Design: `SecondInterventionDesign.md`, `ScenarioCompletionDesign.md`, `FailureRecoveryDesign.md`, `ObjectiveRedesign.md`.

**Scope**

1. `ScenarioOneObjectives`: completion model D (D1–D7), each from saved data; display names at render.
2. Second-look trigger (RD ≥ 0.6 / light-limited regeneration / storm / planting loss; floor 5, fallback 12 years) exposed to UI as a reason string; PLACES priority 0.
3. Per-forest stage progress (L2): saved `scenarioProgress` (stage id → first year) in the next free save version; validation; old saves → empty progress.
4. `EvaluateProgress`: remove or reword the cash-zero failure (O1); Year-100 outcome → "Century Review — scenario goals not completed"; old saves with `Failed` displayed accordingly; dead-end detection kept from P3.
5. Completion view in the Annual Review (`ScenarioCompletionDesign.md` §4).
6. (If O2 = standing sale) **separate economy packet**; not in P6.

**Owned files:** `ScenarioOne/ScenarioOneObjectives.cs`, `ScenarioOne/ScenarioOneManager.cs` (EvaluateProgress, progress record capture/restore), `ForestSaveData.cs`/`ForestSaveValidation.cs`/`ForestSaveController.cs` (progress record only), `UI/ScenarioProgress.cs`, `UI/AnnualReviewView.cs` (completion view), `Tools/Verification/ScenarioOneCompletionVerification.cs` (+ new negative-control harness).

**Locked:** ecology, storms, economy, Reference archive, scenes/prefabs.

**Exclusions:** economy recovery route; player-declared goals; sample plots.

**Save impact:** **yes**: one progress record, next free version (after storms). Missing field → empty progress. Reference Future v1 untouched.

**Anchors:** completion anchor **intentionally changes** (authorised). Record the new value. Lifecycle and Reference anchors must not change.

**Test plan**

- New reference plan meeting D → completes; record the anchor; two processes identical.
- Negative controls: unmanaged; one thinning only; second cycle < 5 years; clear-fell (canopy < 0.35 any year); planting with no 5-year survival → not complete, with the correct unmet condition named.
- Trigger calibration: T0–T5-type plans → trigger year 6–20.
- Storm viability matrix (from the storm W2 calibration) ≥ 90 % completion for the reference family.
- Save: v1 → latest load; progress round-trips; old Failed saves display "not completed".
- Full regression suite.

**Manual review:** a play-through of two contrasting paths (conservative vs heavy). Does the completion screen read as a description, not a grade?

**Stop conditions:** any lifecycle/Reference anchor drift; storm viability < 90 % (return to Manager: calibrate storms, not objectives); a save need beyond the progress record.

**Handoff:** IMPLEMENTATION HANDOFF + independent review request.
