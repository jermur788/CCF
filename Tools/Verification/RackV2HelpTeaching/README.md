# RACK-V2-10 help / teaching offline check

```
python3 -B Tools/Verification/RackV2HelpTeaching/run_offline.py --out <dir outside the repo> [--base 6d3228c]
```

No Unity process. Nothing is written inside the repository (the generated .NET project lives in `--out`). Needs `dotnet` (SDK 8+). Exit 0 and `RACK_V2_HELP_TEACHING_PASS` on success.

- **Static part (S1–S12)** reads the UI files that cannot compile on plain .NET and checks their wiring:
  - `ScenarioOneUiRoot.HelpContext` unlocks Tree Marks only from the saved `RackPlanningV2Active` and `RackPlanConfirmed` state.
  - Tree Marks is the walking context until it has been seen once, and again while the spray marker is equipped, and the existing context `Introduce` path opens it.
  - The V2 Rack help, the Objective 1 concept, good practice, instruction and lesson texts use the checked pure copy, and only in a V2 world.
  - Since `--base`, only the RACK-V2-10 copy/wiring files changed: no marking, geometry, manager, save/schema, scene, prefab, material, settings, package or `.meta` change, and nothing untracked under `Assets/`.
- **Offline part (`RackV2HelpTeachingOfflineCheck.cs`)** compiles `MenuHelpView.Text.cs`, `LearningObjectivesView.Curriculum.cs` and `ControlsReference.cs` and checks them. It reads the spec's quoted §7, §28 and §31 sentences, so the copy cannot drift from the spec silently. It checks:
  - the help screen names;
  - the Tree Marks entry set (orange, red and blue only; no future categories);
  - the §28 orange convention and first-tree simplification disclosure, verbatim;
  - the unlock truth tables;
  - the §31/§7 Objective 1 sentences;
  - the §30 flow at every plan/marking state;
  - the lesson ids;
  - forbidden claims (`18 m`, `ideal`, `always`, five-rack quota wording, universal-practice wording, legacy `B` authoring) on every RACK-V2-10 surface.

This proves wording, the entry set, the unlock rule and the flow order. It does not prove rendering, modal timing in play or text fit; those belong to the rendered gate (RACK-V2-13).
