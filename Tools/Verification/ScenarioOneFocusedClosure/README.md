# #359 verification-only probes: Scenario One focused personal-playtest gate closure

Disposable, **verification-only** Unity probes written by CCF #359 for the game source `045966d2f8e26368e30a79f14a1e34167952b95e`. Nothing
here is game code or compiled into a build. They do **not** edit, re-pin, replace or excuse any earlier harness: the historic
`RackV2Save/RackV2SaveVerification.cs` (save03b), `ScenarioOneUxPedagogy/UxPedagogyShots.cs`, `StandMap/StandMapParity.cs`, the
Workbench strict runner and the #358 `StandMap/V2Witness/` files stay byte-identical, and their FAIL results stay FAIL.

The author of these probes also ran them, so they are **author evidence**, not independently reviewed. The #316 reviewer should review them.

## Run status in #359 (read before reusing)

* `Ccf359V2Closure.cs` was run **once**, exactly these bytes (SHA-256 `ac6b19fbdbd17586a41a3fe972c9316012232c1825be4131d9449373a5975655`):
  `CCF359_V2_CLOSURE_FAIL checks=135 shots=28`, 11 FAIL lines. Four of them are a **probable product finding** (onboarding keyboard focus,
  see the #359 result). The other seven come from **three defects in this probe**, which are kept unfixed so the committed bytes stay the tested bytes:
  1. `I_workplan_forced_locks`: `ForceStage` injects facts without first letting `LearningObjectivesView.TeachingFacts()` sync
     `TeachingWorldVersion`, so the injection is discarded (topic stays 0). A fix must read `ui.Learning.CurrentTopicIndex` before injecting,
     as UxPedagogy does by opening Objectives first. The forced 5/7 lock results are therefore **NOT VERIFIED**, not product failures.
  2. `G_five_rack_figure.FIGURE_WORDS_ARE_AT_LEAST_THE_FIGURE_FLOOR` measures every label, including the shared lesson caption
     (`.diagram-caption` = `--fs-body` 18). The 20-unit floor applies only to the figure's own title, notes and labels. The check is over-scoped.
  3. `BH_transitions.REFERENCE_PREVIEW_STAND_MAP_HEADLAND_STROKES_ONLY_IF_V2` reads `StandMapCanvas.LastPlan` while the Stand Map cannot
     open during a preview, so it reads a stale paint. It must require `CurrentScreen == Map` and `PaintedVersion == Model.Version`.
* `Ccf359OldWorld.cs` and `Ccf359V24SaveContract.cs` were **authored and typechecked only** (Roslyn against the candidate's compiled
  `Assembly-CSharp.dll` and the Unity 6000.6 DLLs, with a negative compile control) and were **NOT run**: #359 stopped Unity after the onboarding finding, as its packet requires.

| File | Session | Covers (only behaviour no passing gate on `045966d2` covered) |
|---|---|---|
| `Ccf359V2Closure.cs` | graphical Editor, V2 new game | **J**: onboarding default focus and keys, F1 Help open/close and focus, via queued New Input System keyboard events (synthetic Editor input, not an OS-native/Windows test). **B**: one V2 headland surface, deactivated by a model-0 load and a Reference Preview, the same object reactivated. **C**: the live 3D mesh, the Stand Map model and the Stand Map's own headland strokes/pads agree (the #358 witness checked those strokes by count only), the feeder meets the forest path, and there is no northern spur, plus negative controls. **G**: the live five-rack figure. **H** (pre-V2 load and preview part). **D+**: planning-mode and confirmed-rack Mini/Full parity against the manager's endpoints, with a negative control. **I**: Work Plan locks at *forced* lessons 0/5/7 (forced facts, NOT real stage-5/7 progression). **F**: frame time and GC allocation, **measured only**: there is no baseline, so no limit is asserted |
| `Ccf359OldWorld.cs` | graphical Editor, `CCF_STAND_GEOMETRY=0` | **H**: in a fresh Legacy40 game and inside a frozen Reference Future v1 preview: no V2 surface, Stand Map headland, guidance, five-rack figure or V2 wording |
| `Ccf359V24SaveContract.cs` | batch is enough | The save03b assertions with no current-version counterpart: hash compatibility at v24 against an **independent** strip that includes `firstGuidedRackReviewComplete`, plus a **provenance witness** showing that the historic save03b strip does not match (this reproduces the #358 diagnostic failure); the latch never moves an identity; the nine semantic rejections that `RackV2Persistence` does not cover; `RestoreSaveData(null)` stays model 0; V2 is never active on Legacy40 |

## Run (clean registered lane, machine-wide Unity lock held, evidence outside the repository)

Stage one file into `Assets/ForestPrototype/`, run, and remove it with its generated `.meta` (the #357/#358 `run_gate.py` driver does
this with `--ext-files`):

```bash
# session 1
CCF_TEST_DIRECT_ENTRY=1 CCF_DIAG_DIR=<files dir> CCF_UI_SIZES=1920x1080,1280x720 Unity -projectPath . -executeMethod Ccf359V2Closure.Begin       # CCF359_V2_CLOSURE_PASS
# session 2
CCF_TEST_DIRECT_ENTRY=1 CCF_STAND_GEOMETRY=0 CCF_DIAG_DIR=<files dir> Unity -projectPath . -executeMethod Ccf359OldWorld.Begin                 # CCF359_OLDWORLD_PASS
# session 3
CCF_TEST_DIRECT_ENTRY=1 Unity -projectPath . -executeMethod Ccf359V24SaveContract.Begin                                                        # CCF359_V24SAVE_PASS
```

No probe reads or writes a save file, saves a scene, advances a year or writes an asset. Session 1 temporarily disables tree renderers for one
top-down capture and re-enables them. It also confirms a five-rack plan in the session's in-memory world, then reloads the fresh world.
