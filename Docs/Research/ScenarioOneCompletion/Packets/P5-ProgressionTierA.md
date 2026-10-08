# P5 — Progression restructure (Tier A, UI only) + Model 2 teaching copy

```text
TASK PACKET
GOAL: One "Scenario progress" panel guides the player through stages S1–S12 that
      complete on observed decisions; lessons follow play; clearance teaching
      matches the active regeneration model.
BRANCH: task/scenario-one-p5-progression
```

**Start authority:** P4 integrated; H1 findings reviewed; decisions L1 (Tier A), J3 (preview behind U), J2 (M2 copy, if M2 is integrated). Design: `ObjectiveRedesign.md`, `RegenerationModel2Teaching.md`.

**Scope**

1. New `UI/ScenarioProgress.cs`: stage definitions S1–S12 with evidence rules (`ObjectiveRedesign.md` §3). Event-derived stages (S4, S8, S9, S11, S12) are read from saved data. UI-observed stages (S1, S2, S3, S5, S6, S7, S10) are session-scoped and shown as "seen this session" (Tier A limitation, stated in help).
2. HUD: "Next: <stage>" replaces "Next lesson". O opens **Scenario progress** (current forest objectives listed with plain names, plus stages); lessons become per-stage help.
3. `MenuHelpView`: shorter first-use texts (≤ 3 paragraphs); a controls card under F1.
4. If J3 is accepted: `ForestPlayer` shows the clearance preview only after U (clearance mode); update `ClearanceVerification`.
5. If Model 2 is integrated: model-dependent clearance copy, ground "Why" vegetation lines, Annual Review vegetation-loss line (current year, M2 ledger).
6. **Completion is unchanged** (forest objectives still evaluated by `ScenarioOneObjectives`).

**Owned files:** `UI/ScenarioOneUiRoot.cs`, `UI/WalkingHudView.cs`, `UI/LearningObjectivesView.cs`, `UI/MenuHelpView.cs`, new `UI/ScenarioProgress.cs`, `ForestPlayer.cs` (preview trigger only, if J3), `ClearancePreview.cs`/`VegetationClearance.cs` (copy), `Tools/Verification/{MenuTutorialVerification, ClearanceVerification}.cs`, new harness.

**Locked:** manager, objectives, save, ecology.

**Exclusions:** per-forest saved progress (L2, P6); completion changes; storm stages.

**Save impact:** none.

**Test plan**

- Interactive: no stage completes from inaction (scripted 10 advances with no action → only "time" stages); S4 completes only when a felled tree was within 8 m of a Crop Tree; S8 completes only after 5-year survival.
- MenuTutorial gate updated; Clearance gate updated if J3.
- String gates per regeneration model (M1 sentence vs M2 sentence; no growth/seed/light/browse claim for clearance).
- Anchors unchanged.

**Manual review:** fresh profile, first 30 minutes. Is "Next" always relevant to what the player is doing?

**Stop conditions:** needing a save field; any objective/completion change.

**Handoff:** IMPLEMENTATION HANDOFF.
