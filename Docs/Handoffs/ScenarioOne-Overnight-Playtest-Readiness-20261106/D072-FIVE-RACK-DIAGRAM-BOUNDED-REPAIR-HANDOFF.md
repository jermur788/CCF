# D-072 #296: five-rack teaching figure, BOUNDED REPAIR HANDOFF

**Date:** 2026-11-06. **Issue:** [#296](https://github.com/jermur788/CCF/issues/296).

**Authority:**
- [Manager #200 ruling](https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Overnight-Playtest-Readiness-20261106/MANAGER-D072-DIAGRAM-REVIEW-RULING-AND-SERIAL-REPAIR-20261106.md) (Stage A).
- [Independent review](https://github.com/jermur788/CCF/blob/docs/d072-five-rack-diagram-review-20261106-v1/Docs/Handoffs/ScenarioOne-Overnight-Playtest-Readiness-20261106/D072-FIVE-RACK-DIAGRAM-INDEPENDENT-REVIEW.md) (`4165fc0d`).

**Verdict: SOURCE REPAIRED + OFFLINE-CHECKED ONLY.** This repair has not been:
- compiled or imported in Unity, rendered, or measured on screen;
- input-tested or integrated;
- reviewed again by an independent reviewer, or accepted by the Manager or user.

**This is not a self-approval.** The original `OBJECTIVE-GUIDANCE-TEACHING-MAP-IMPLEMENTATION-HANDOFF.md` and the independent review are preserved unchanged. The runner checks that the original handoff still matches `e7bda1f2`.

## Worker, lane and HEADs

| Item | Value |
|---|---|
| Worker | Anthropic **Claude Opus 5.5** (`claude-opus-5-5`), Claude Code harness in T3 Code. Dispatched by the DeepSeek orchestrator as the sole repair writer. |
| Effort | The session's observed reasoning-effort setting was **15 (low band)**. The ruling *preferred* a high-reasoning worker, and I do not claim one. The re-reviewer should weigh that. |
| Branch / worktree | `feature/scenario-one-next-action-guidance-v1` at `/media/jer/ZX20/CCF-NextActionGuidance-T3-20261106` |
| Start HEAD | `e7bda1f2eea4340a0d8eaae351abb43ffaff8202`, equal to origin after fetch. Tracked and untracked porcelain were empty, ZX20 was exFAT rw with 31 G free, and no other worktree was on the branch. |
| Source commit | `316429dc5173e81e6d903ea86531a60952f2d36a` (pushed) |
| End HEAD | The documentation commit that adds this file. A file cannot contain its own hash, so the [HANDED-OFF] comment on #296 records it. |
| Not touched | `active-tasks.md`, `/media/jer/Files/CCF-Primary`, the four #297 Stand Map sources, `Tools/Verification/StandMap/HeadlandOffline/*` (#303), USS, scenes, prefabs, saves, rack/MFM/Work Plan code, Unity, Editor, Player and builds |

## Exact changed files (`e7bda1f2..316429dc`)

| File | Change |
|---|---|
| `Assets/ForestPrototype/UI/LearningObjectivesView.cs` | **One line (161):** the Objective 1 diagram call now passes the existing `rackV2Objective`, computed at line 152. Lines 152–156 and every other statement are unchanged. The runner proves the diff against `1b68ddcd` is exactly this one line. |
| `Assets/ForestPrototype/UI/TeachingDiagram.cs` | New constructor `TeachingDiagram(int, string, bool rackV2World)`. The old `(int, string)` constructor chains to it with `false`, so it fails closed. Selection goes through `Plan.ShowsFiveRackFigure(lesson, rackV2World)`. Planned and draft corridor fills now use real transparency (`Tint`) instead of the opaque pre-blended `Colour()`. The property boundary is a thin solid outline, no longer dashed. |
| `Assets/ForestPrototype/UI/TeachingDiagramPlan.cs` | Adds the pure seam `ShowsFiveRackFigure(lesson, rackV2World) => lesson == 0 && rackV2World`. Key rows become two explicit short lines (`FigureKeyLineMax = 27`). The three notes become one. Comments updated. `For(...)` stages are untouched; the existing check's 862-line lesson dump and the painter probe both show them byte-identical. |
| `Tools/Verification/ScenarioOneFiveRackDiagram/run_five_rack_check.py` | Adds the call-site guard, selection-source guards, the painter-probe stage, 22 new mutations, a non-destructive `--out` and an INCOMPLETE verdict. |
| `Tools/Verification/ScenarioOneFiveRackDiagram/FiveRackDiagramCheck.cs` | Adds selection, M-1 budget, M-3 key, the converse key check (m-1), an independent tree/surface check (m-2) and reverse track fidelity (m-3). Three text assertions were relocated (see below). |
| `Tools/Verification/ScenarioOneFiveRackDiagram/PainterProbe/RecordingUnityStubs.cs`, `PainterProbe.cs` | **New.** Recording stand-ins for the UI Toolkit types the painter uses, and a driver that runs the **real** `TeachingDiagram.cs`. |
| This handoff | New |

## Per-finding disposition

### B-1 (blocking): FIXED. Legacy40 is now truthful.

The five-rack figure now appears only when `LearningObjectivesView` passes `rackV2Objective = TeachingRacks && ui.Manager.RackPlanningV2Active`, which is the same flag that already switches the card's words. All of the following get `Plan.For(0)` (the original two-rack Objective 1 picture) and the original `lesson.Caption`, with no title and no notes:

- Legacy40;
- Reference Future v1;
- pre-v21 saves;
- pre-V2 Enlarged80 continuations;
- any caller using the old two-argument constructor.

Nothing in those worlds says that a western headland, a southern feeder, an 80 m stand, entrances or field-marking exist.

What the B-1 fix does not add:
- no scene lookup in the passive picture (a source guard forbids `RackPlanningV2Active`, `ScenarioOneManager` and `FindObject` in both diagram files);
- no save flag;
- no second eligibility rule.

Intro, lessons 2–8 and the extension (`ExtensionOffered ? 4`) are unchanged.

**Offline proof, run by the painter probe against the real painter source:**
- **Positive selection:** `(0, caption, true)` builds the `access-five-rack` stage, with its title, note and figure commands, at all three widths. Its output differs from the original picture.
- **Negative selection:**
  - `(0, caption, false)` and the two-argument form build the original `access` stage.
  - The probe finds no label containing headland, feeder, five-rack, Stand Map, 80 m, field-marked or entrance.
  - The picture's children are only the frame and the caption.
  - The seam returns false for every other lesson with either flag.
- **Backwards compatibility, byte-identical:**
  - The base painter and plan (`1b68ddcd`, before #296) and the current ones were compiled against the same stubs.
  - The probe compared the element tree, every inline style and every Painter2D command for pictures −1..7 at widths 810, 788 and 900. It covered the two-argument form, `false`, and `true` for every picture except Objective 1: **78 comparisons, 0 differences**. The base dump has 191,904 lines, sha256 `cbe08cc8…a6a3`.
  - Mutations P5, P6, P7, Q5, Q6 and P14 show that the comparison fails when the selection, the old constructor's default, a palette byte or a legacy painter stroke is changed.

### M-1 (card fit): FIXED IN SOURCE; Unity measurement still required

The figure's added text went from a 105-character title plus three notes (582 characters, about 10 lines) to the same mandatory title plus **one 161-character note, about 4 lines**. That is roughly 6 fewer lines and 2 fewer note margins.

These are estimates from the review's measured ~88 characters per line at 20 UI units, not measurements:
- at 1280×720, about 118 px saved against a ~44 px estimated overrun;
- at 1920×1080, about 177 px saved against ~83 px.

The note reads:

> Only Work Plan approval commits the work. 16–20 m between racks (15.6–19.4 m here) is guidance for this plantation, not a hard rule. Approximate scale, north up.

The rest of the old notes moved into the key (see M-3).

**Key rows are now two explicit lines of at most 27 characters.** Previously they were ≤ 36 characters and left to wrap. At the 1280×720 frame (≈ 326 UI units of text width) this keeps every row to two lines, so a row cannot run into the next row 56.7 UI units below.

**Unchanged:**
- the mandatory title wording;
- the 20 UI-unit floor;
- the figure's 1000×520 proportions.

No harness assertion was touched: `GuidedOnboardingVerification`, `UxPedagogyShots` and the #224 overflow rules are unchanged.

The checker now enforces a budget:
- exactly one note, ≤ 175 characters;
- title plus note ≤ 300 characters;
- ≤ 4 estimated lines;
- each key row ≤ 2 explicit lines of ≤ 27 characters.

Mutations Q2 and Q7 show that the budget fails when the old long notes come back or a key row grows.

**Not proven:** real text metrics, wrapping, scrolling and fit at 720p/1080p. These remain the Unity `UxPedagogyShots` `OBJECTIVE_CARD_FITS_THE_SCREEN` / `NEEDS_NO_SCROLLING` gate.

### M-2 (checker blind spot): FIXED for the painter's commands; rendering still unproven

`PainterProbe` compiles the **real** `TeachingDiagram.cs` and `TeachingDiagramPlan.cs` against `RecordingUnityStubs.cs`. It builds the diagram as `LearningObjectivesView` does, fires the frame's real geometry callback, and invokes the real `generateVisualContent`. It also calls the private `DrawItem` per item by reflection.

**It checks 165 assertions across the three widths:**
- Draw issues exactly the plan items' commands in plan order.
- Each planned corridor has one see-through fill (alpha ≤ 90/255) and two continuous solid edges spanning its full length. Its 15–17 standing trees remain visible.
- The draft swatch has short dashed strokes and a fainter fill. The harvested swatch is one opaque strip with no edges.
- Each existing surface is one opaque round-capped band at least the authored width, and casings are drawn before fills.
- Each entrance is an outlined square. The field mark is two vertical stripes. The property boundary is one continuous outline. The north arrow is drawn.
- No Fell colour appears anywhere.
- The seven key rows have pairwise-distinct shape signatures with colour ignored.
- **The title** is the first child, positioned relative above the frame, with the exact wording, at ≥ 20 UI units.
- **The note** follows the frame with the exact text, then the caption, and nothing else is added outside the frame.
- Every key row and the tags A/B/C/N/1–5 are labels inside the frame, all at ≥ 20 UI units.

**What this proves:** the painter's element tree and command stream. **What it does not prove:** that Unity renders them as intended. The stubs have no layout engine, fonts, USS, tessellation or blending, so this is not a Unity or render result.

**Painter defect found by the probe (not in the review):** at `e7bda1f2`, `DrawCorridor` filled planned corridors with `Colour(...)`. That method pre-blends against the panel and always returns alpha 255, and the corridor was drawn after the trees (Z 30 > 20). So the "translucent" fill was **opaque** and hid the tree centres inside every planned rack, which reads as cleared. Only slivers of each 1.5 m disc showed beyond the 2.4 m corridor edges. The review's R10 ("fill made opaque") therefore described the shipped behaviour. The fix uses a real-alpha `Tint`, and mutation P2 restores the old behaviour and is caught by 33 FAIL lines.

**Review gaps strengthened:**
- **R7 / m-2:** an independent tree-to-surface distance check uses the authority feeder and pads, the **real** scene + `StandApproachRoute` track and the scene clearing, without calling `OnExistingSurface`. Mutation Q3 is caught.
- **R8 / m-3:** reverse fidelity checks that all 110 real track points down to the stub end lie within 5 cm of the figure's track (worst 0.01261 m). Mutation Q4 (a three-point track) is caught with a worst distance of 3.27 m.
- **R13:** the probe's legacy comparison includes every colour. Mutation Q5 (one Canopy palette byte) is caught.

**Real plan-source compilation and fixture provenance are preserved.** The checker still compiles the real `TeachingDiagramPlan.cs`. The snapshot hash, model, IDs and float Z are re-read from the archive, and the 862-line lesson dump is unchanged.

### M-3 (semantics): FIXED

The planned key row now reads "Planned = confirmed: solid, / work pending, trees stand".

- "Confirmed" and "pending" appear only in that row, not in the draft, field-marked or harvested rows; a checker assertion enforces this.
- The draft row is "dashed, a proposal only".
- The field-marked row is "stripes; not felled, not a Fell mark".
- The harvested row is "open strip with stumps".
- The note keeps the Work Plan commitment gate: "Only Work Plan approval commits the work."
- The existing-infrastructure row names the existing surfaces truthfully: "Existing: A headland, B feeder, C track, clearing". These surfaces exist in every world where this figure is now shown.
- Shape semantics stay colour-independent at both plan level and painter level. Mutation Q1 is caught.

### The eight minor findings

| ID | Disposition |
|---|---|
| m-1 key/map completeness, dashed boundary | **Addressed.** The boundary is now solid. The clearing is named in the existing row. A new converse check requires every map symbol to be a key shape or a named existing pad/clearing (boundary and arrow are self-labelled). Mutation P11 is caught. |
| m-2 tree-on-surface self-test | **Addressed** with the independent check; Q3 is caught. |
| m-3 one-way approach fidelity | **Addressed** with reverse fidelity; Q4 is caught. |
| m-4 default `--out` deleted earlier evidence | **Addressed.** The default is a new timestamped folder, and a non-empty `--out` is refused. Nothing is deleted. |
| m-5 PASS without the archive | **Addressed.** A run where the snapshot was NOT RUN now ends `FIVE_RACK_SUITE_INCOMPLETE` with exit 2. |
| m-6 lesson purple vs Stand Map amber | **Carried.** No space to label it further; the title already says "not your saved Stand Map". This is for the user-clarity review. |
| m-7 "painter methods unchanged" overstated | **Addressed by evidence.** The constructor, `AddText`, `OnFrameGeometry` and `DrawItem` did change. The probe now proves that their output for every non-V2 picture is byte-identical to `1b68ddcd`, under the stubs. The 8 compared methods are still compared. |
| m-8 mesh cost of 384 discs | **Carried** to the Unity performance check with #297 m-1. |

**Also carried:**
- #297 notes m-1..m-5 (Unity gate);
- the five-to-two-rack inconsistency in lessons 2–8 (the Manager ruled not to change it);
- grayscale stripe contrast;
- one world's seeded positions.

### Relocated assertions (not weakened, stated so a reviewer can judge)

| Old assertion | Now |
|---|---|
| Notes contain "between racks 1 and 2" and "existing western headland / southern feeder / approach track" | The key row names A headland, B feeder and C track as existing. Where the feeder joins is still asserted geometrically (FeederZ between entrances 1 and 2, not on the south edge). The sentence was cut for M-1. |
| Notes contain "pending work only" and "not a Fell mark" | The key contains "confirmed" and "work pending" (planned row only) and "not a Fell mark". The note starts "Only Work Plan approval commits the work." |
| Notes contain "(15.6–19.4 m)" and "not a tree count" | The note contains "(15.6–19.4 m here)", still computed from the fixture. "not a tree count" is in the key's tree row. |
| Key rows ≤ 36 characters | ≤ 2 explicit lines of ≤ 27 characters. This is stricter for rendered fit. |
| `LearningObjectivesView.cs` wholly unchanged since base | It now differs by exactly the authorised line, with the flag line unchanged. Any other change fails. |

## Tests run (actual output)

Every command used `python3 -I -B`, .NET 10.0.114 and output outside the repository. **Evidence:** `/media/jer/Files/CCF/Verification/D072-T3-FiveRackDiagram-Repair-20261106/`.

| # | Command | Result |
|---|---|---|
| 1 | `run_five_rack_check.py` at `316429dc` (default out `run-20261106T142400Z/`, log `suite-at-316429dc.log`) | **PASS** `FIVE_RACK_SUITE_PASS source_checks=131 failed=0`: `FIVE_RACK_PASS checks=109`, `FIVE_RACK_PAINTER_PASS checks=165`, build warnings `[]`, lesson dump byte-identical (862 lines), base painter dump of 81 sections |
| 1a | ↳ 29 mutations: the original 7 plus Q1–Q8 (plan) and P1–P14 (painter) | **All 29 caught**, each by its intended assertion (log lines quoted in the evidence) |
| 2 | `ScenarioOneUxPedagogy/OfflineLogic/run_diagram_check.py` | **PASS** `DIAGRAM_PLAN_PASS checks=1330` (same as before the repair) |
| 3 | The same with `--design …/astra/stand.json` | **FAIL, pre-existing:** `DIAGRAM_DESIGN_MATCH_FAIL lines=762`. Identical before the repair and with the base plan. |
| 4 | `CloudTypeCheck/run_typecheck.py --mode prod` | **PASS** `TYPECHECK_CLEAN`, 6 distinct errors = baseline 6. This approximates Unity 2021.3 plus stubs, **not** Unity 6000.6. |
| 4a | The same with `--replace` of a planted typo (`ShowsFiveRackFigur`) in `TeachingDiagram.cs` | **Caught:** `TYPECHECK_NEW_ERRORS` |
| 5 | `StandMap/OfflineLogic/run_standmap_check.py` | **PASS** `176 passed, 0 failed` |
| 6 | `StandMap/HeadlandOffline/run_headland_check.py` (#297 suite, unmodified, owned by #303) | **FAIL, recorded honestly:** `HEADLAND_SOURCE_AND_OFFLINE_FAIL: only four authorised display files changed…`. This is the temporal four-file inventory guard, now also tripped by the authorised call-site line (its `LearningObjectivesView.cs` protection would trip next). Not repaired here; it is #303's scope. |
| 7 | `NextActionGuidance/check_sources.py` (T2) | **38 PASS / 1 FAIL, pre-existing:** "production diff restricted to five owned source/style files". The same tally as before the repair. Every other T2 assertion still passes with the call-site line. |
| 8 | `git diff --check` | **PASS** |

Tests 2–8 ran on the working tree that became `316429dc`, with no edits in between. Their logs are in `other-gates/`.

## NOT RUN (forbidden or out of scope)

- Unity import or compile in the 6000.6.0f1 Editor, and any actual rendering.
- `UxPedagogyShots`, `GuidedOnboardingVerification`, `SequentialObjectivesVerification`, `StandApproachVerification`, `StandMapParity`.
- 1280×720 and 1920×1080 captures in colour and grayscale; text metrics, wrapping, card fit and no-scroll.
- Legacy40, Reference Future v1 and old-save runtime checks of the selected picture (B-1 is proven offline only).
- Two-reviewer recognition and user clarity; native input; performance and mesh cost.
- #303 headland checker repair; the Player or Windows build.

## Requests

- **#303 may start.** This repair's writing phase is released. #303 must re-preflight and claim separately. Its checker must accept exactly the two diagram files and the one `LearningObjectivesView.cs` line, as the ruling says.
- **Targeted independent re-review.** Re-review B-1, M-1..M-3, the newly found opaque-fill defect, the relocated assertions, the probe's limits and the carried minors on the combined HEAD after #303. Prefer a cross-provider reviewer.
