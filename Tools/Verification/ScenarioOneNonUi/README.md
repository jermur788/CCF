# SCENARIO-ONE-PLAYER-TEST-NON-UI-REMEDIATION-V1 verification

Covers the four non-UI player-test items: the forest track's approach (`StandApproachRoute` / `StandApproachPath`), competitor keyboard targeting (`CompetitorKeyboardTarget`), the competitor `Share` explanation, and the rack-area lesson for Objectives 2-5 (`ScenarioOneRackBay`). Design note: `Docs/Design/ScenarioOneRackBayLessonV1.md`.

Nothing here writes into the repository, the player's save or the scene. Three layers, from cheapest to most complete:

## 1. Decision rules on plain .NET (no Unity)

```
python3 -I Tools/Verification/ScenarioOneNonUi/run_nonui_offline.py --out <dir outside the repo>
```

Compiles the production files that hold the rules (none uses `UnityEngine`) with `OfflineLogic/*.cs` and runs them. Expect `NONUI_KEYBOARD_PASS`, `NONUI_RACKBAY_PASS`, `NONUI_APPROACH_PASS` (46 keyboard + 77 rack + 49 approach checks for the V2 amendment and the load-cleanup repair). `OfflineLogic/core-stand-replica.csv` is the 336 Legacy40 core trees reproduced from the starting-stand generator (calibration data, not a verification of the generator itself; made at the code's default 1.9 m spacing, not the scene's 2.0 m, so its positions are about 5% short of live and its derived figures are not live-accurate); `StandApproachOfflineCheck` and `RackBayOfflineCheck` read it.

## 2. Syntax / type compile of the game's runtime C# (no Unity)

```
python3 -I Tools/Verification/ScenarioOneNonUi/compile_runtime_sources.py --src <project root> --out <dir outside the repo> \
  [--extra Tools/Verification/ScenarioOneNonUi/UnityGates/*.cs Tools/Verification/StandGeometry/Enlarged80Verification.cs]
```

Compiles `Assets/**` (Editor folders excluded) with `dotnet` against the runtime assemblies of an already-built Player (`--managed`, default `/home/jer/ccf-verify-player/CCFVerify_Data/Managed`). It is **not** Unity's compile: no Editor assembly, source generators or analyzers, and it proves nothing about behaviour. It does catch missing members and type errors (checked with a deliberate error). The base commit `6bdcdd5` compiles with 0 errors under it.

## 3. Live Unity gates (graphical Editor, under the machine-wide Unity lock, one Editor at a time)

Each is a disposable harness in `UnityGates/`, staged into `Assets/ForestPrototype`, run, then removed together with its `.meta` by `ScenarioOneProfessionalMap/run_unity_harness.py`:

```
python3 Tools/Verification/ScenarioOneProfessionalMap/run_unity_harness.py --name CompetitorKeyboardVerification \
  --method CompetitorKeyboardVerification.Begin --files ScenarioOneNonUi/UnityGates/CompetitorKeyboardVerification.cs \
  --out <evidence dir on /media/jer/Files/CCF/Verification> --private-display --keep-focus
```

| Gate | Token | What it checks live |
|---|---|---|
| `CompetitorKeyboardVerification` | `KEYBOARD_PASS` | Real `1`-`5` / `X` / `C` / `E` key events: each number chooses its row and the highlighted tree is the tree behind it; `X` marks and unmarks that tree with the reticle on the Crop Tree; the same tree stays chosen when the list is rebuilt; switching numbers; the same number again and closing the list let go; `X` then acts on the aimed tree again; a different Crop Tree starts with nothing chosen; `X` refuses a tree still marked as a Crop Tree by keyboard and by mouse, with the same message; planting mode and a save load leave no stale target. |
| `RackLessonVerification` | `LESSON_PASS` | The area is the middle rack's strip; three selections must cover near/middle/far and both sides at 8 m spacing; clustered selections fail; the lesson's planned share equals the competitor panel's; sold / deadwood are counted per tree; derived forest facts survive save/load, while session inspection decisions must be repeated after load. V2 also runs real Work Plan/Annual Review, synthetic keyboard post-release inspection/J-K decisions, retained-stand review, zero-extra-felling completion, over-cap UI rejection and in-memory reload invalidation. Queued InputSystem events are not real OS-input qualification. |
| `StandApproachVerification` | `APPROACH_PASS` | The extension planks, the opening in the south ridge, the cutting and the invisible blocker; no tree within the corridor of any plank; 336 core + 1,008 outer trees with P0009 out and P1200 in; the Stand Map draws the extension to the property edge; Legacy40 is left as authored and the approach comes back identically; an old Enlarged80 save keeps the whole south ridge and no extension, and a new world loads back over it with the approach intact. |

A visual review is still required for the cutting beyond the south ridge (it is scenery the player cannot enter): render the view from inside the property looking south through the opening before calling the path done.

## Protected gates to run on the same head afterwards

The chain used for `SCENARIO-ONE-PROFESSIONAL-MAP-INTEGRATION-V1` (evidence `ScenarioOneProfessionalMapIntegration-20261022T114849Z/gates` on Files): `guided` first and alone, `pmv`, `parity`, `combined-hud`, `menu`, `uxshots`, `completion` (expect world hash `9C77EB4BEE427FB9`), `standgeometry`, `Enlarged80` (verification and review; its core-preservation check now pins the one intended difference, P0009 out / P1200 in), `teaching`, and the regressions ScenarioReference, ScenarioOneEconomyIntegration, ScenarioOneProgress, SaveHardening.

Expected, intended changes of those gates because the Enlarged80 starting stand now differs (outer trees near the new track, and the P0009 / P0600 swap): the Enlarged80 start-world hash (`ENLARGED80_START_WORLD_CURRENT_HASH`) and anything derived from it. Legacy40 results (the Reference Future, `completion`, the economy and save-hardening gates) must NOT change. The swap is P0009 out / P0600 in, measured live and repeated across two Editor sessions (SCENARIO-ONE-APPROACH-ANCHOR-DIAGNOSIS-V1). An earlier offline replica predicted P1200 because it used the code's default 1.9 m lattice spacing instead of the scene's serialized 2.0 m; at 1.9 m it found 41 clearance omissions instead of the live 36, so its mortality cut-off fell one rank lower, on P1200. The route cannot be shifted clear of P0009 without a bend tighter than the authored track, and the core tree must not move (see the design note).

## V2 return-review manual checks (registered Unity lane only)

After access and initial release, acknowledge the Annual Review. Return to all three selected trees and open each real inspection [E]. Confirm its decision with J (retain here) or mark justified in-footprint competitors and use K (release further). Inspect retained woodland in each third on each side, confirming it with J. Expected: three tree decisions and six stand portions; Objective 6 becomes active with no additional felling when all decisions retain. A generic annual acknowledgement, two inspections, a clustered group, an outside-footprint removal or an over-cap footprint cannot finish. Loading the same save must require fresh inspections; already-open inspection must be closed/reopened. Further executed in-footprint work requires a fresh review.

Capture inspection, Objectives, Work Plan and Annual Review at 1280×720 and 1920×1080, including overflow checks for the new contextual inspection text. Verify real OS E/J/K input separately from the synthetic disposable gate. Rendered approach, old-save/new-world approach, CompetitorKeyboardVerification, StandApproachVerification and every protected gate above remain required. U3 is not registered: do not run Unity there or borrow U1 without assignment/lock.
