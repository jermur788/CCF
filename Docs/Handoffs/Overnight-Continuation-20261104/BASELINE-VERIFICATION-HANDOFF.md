# #260 — Offline baseline verification handoff

**Assessment: ready for independent review of the SOURCE-STATIC baseline only.** No production fix, runtime reproduction, integration or Manager acceptance is claimed. Parent: [#254](https://github.com/jermur788/CCF/issues/254); task: [#260](https://github.com/jermur788/CCF/issues/260).

- Worker: OpenAI / gpt-6.1-sol, medium; assigned test sub-agent, no child delegation.
- Routing journal reference: **EVENT 011**, pool **codex_c75d8114** (dispatch-supplied identity; ledger not edited by this worker).
- Worktree: `/media/jer/Files/CCF/Operations/CCF-Overnight-FeedbackVerify-20261104`.
- Branch: `verify/overnight-playtest-feedback-20261104`.
- Exact inspected production baseline: `72b4efbd1c653fa2231f632a79030a83ff76f833`.
- Candidate context commit: `63e95ba2175f7e239a278c616681b286719821f8` (latest change to canonical context/instruction set on this candidate). Live GitHub coordination and master packet were read separately; their newer status does not replace the candidate's source bytes.
- Initial `git status --short`: empty. `git log --oneline -1`: `72b4efbd map: draw living conifer crowns as pointed whorl stars; smooth Beech outline (#232)`. Git common store verified as `/media/jer/Files/CCF/Repository.git`.
- Changed files: only the new `Tools/Verification/OvernightPlaytestFeedback/` directory (checker, README, raw stdout) and this handoff. No production, existing test, serialized asset, `.meta`, package or project setting changes. No Unity/Editor/Player/Windows execution, merge, rebase, reset, clean, stash or force push.

## Result and interpretation

**Nothing here proves visual layout, input behaviour or rendered correctness.** All entries are SOURCE-STATIC. PASS means only the named bounded source-policy or isolated-math assertion passed. FAIL records feedback that this baseline has not satisfied; it is not a failed Unity run. NOT-TESTABLE means the claimed behaviour requires later observation, even where the source expression can be quoted exactly.

Tally, extracted from the raw output below: **5 PASS / 4 FAIL / 2 NOT-TESTABLE** (11 rows; physical-input and trail observations excluded from PASS; six SnapFar case diagnostics form one aggregate row).

| Check | Result | Bounded finding |
|---|---|---|
| SOURCE-STATIC 01 requested-spacing-presentation-format | FAIL | Requested presentation format, not a functional defect; expected to flip with #255. Require compact 16–20m/16-20m. |
| SOURCE-STATIC 02 tree-marking-heading | FAIL | Require exact heading literal Introducing tree marking; literal presence alone cannot prove display. |
| SOURCE-STATIC 03 source-material-copy | FAIL | No CCF source material phrase in ForestPrototype C# string literals (comments excluded). |
| SOURCE-STATIC 04 orange-entry-is-not-fell | PASS | Bounded legend check: orange rack-removal entry is separate from Red/Fell; no claim about rendered paint. |
| SOURCE-STATIC 05 no-formal-pro-silva-copy | PASS | Conservative lexical guard: no Pro Silva mention in ForestPrototype C# literals; any mention requires human review, not automatic classification as a standard. |
| SOURCE-STATIC 06 round-handle-input | NOT-TESTABLE | BASELINE OBSERVATION only: snap argument is !ShiftHeld(e.modifiers), then forwarded to SnapFar for Reshape. SOURCE-STATIC cannot test physical input. |
| SOURCE-STATIC 06a rotation-snap-policy | PASS | Explicit pre-fix policy matches the real OnPointerMove snap argument for all six gesture/Shift combinations; not #256 fix qualification. |
| SOURCE-STATIC 06b snapfar-offline-cases | PASS | Six direct production SnapFar calls pass on plain .NET; endpoints, flags and returned angles reported. |
| SOURCE-STATIC 07 objective-next-instruction | PASS | Unconfirmed V2 instruction names Stand Map / Plan Racks path; later states are quoted separately. |
| SOURCE-STATIC 08 objective-primary-cta | FAIL | Exactly one primary must invoke ui.ShowMap(), irrespective of label; the baseline primary invokes ui.CloseAll(). Live navigation/focus/prominence unmeasured. |
| SOURCE-STATIC 09 map-minimap-trail-consistency | NOT-TESTABLE | NOT STATICALLY TESTABLE: no demonstrated coordinate mismatch for the same confirmed rack/waypoint. Shared source/draw path is evidence, not rendered parity; unconfirmed racks are deliberately FullOnly. |

The Objective 1 “duration” is spacing in metres. The current concept uses the en dash and a space before the unit, at `Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs:88`. This is an intentional **requested presentation-format failure expected to flip with #255**, not a functional defect or something this worker fixed.

Objective instruction and CTA are separate assertions. The initial V2 instruction already names the Stand Map / Plan Racks path (`LearningObjectivesView.Curriculum.cs:100`), while the primary header button closes the screen (`Assets/ForestPrototype/UI/LearningObjectivesView.cs:75`). Other header buttons exist at lines 73–74. No runtime button focus or navigation was tested.

The orange entry is separate from red Fell (`Assets/ForestPrototype/UI/MenuHelpView.Text.cs:21–22`); the explicit separation sentence is at line 99. No offending orange-is-Fell string was found in this bounded legend check. No Pro Silva mention was found in ForestPrototype C# string tokens; the source comment at `Assets/ForestPrototype/UI/TeachingDiagramPlan.cs:12` is not player copy or a formal-standard claim. This is not a primary-source forestry standards audit or an exhaustive semantic check of all possible displayed text. Exact current strings and expressions are quoted with individual file:line references in the raw output.

The rotation path selects Reshape on the round handle and forwards the negated Shift state into SnapFar. This shows **snapping enabled unless Shift is held**, not that rotation is impossible without Shift: snapping can leave some angles unaffected. No physical gesture was reproduced.

For trails, the shared rack endpoints, waypoint and transform provide no demonstrated coordinate mismatch for the same confirmed data. Source also deliberately hides unlocked unconfirmed racks from the minimap and can show minimap-only suggested guidance. These are presentation-state distinctions, not proof of the reported defect. **NOT STATICALLY TESTABLE** here: no deterministic rendered/input reproduction has been established.

## Re-run and evidence

```bash
python3 Tools/Verification/OvernightPlaytestFeedback/check_baseline.py
```

Baseline exit status: **1**, expected because four source feedback assertions remain unmet. The review-response reruns returned 1, no stderr, and byte-identical stdout. The raw file is `Tools/Verification/OvernightPlaytestFeedback/baseline-output.txt`; the original raw evidence remains in Git at `62f1c1645b63f72125def83838e20847ece04e52`. Production comparison remains MATCH. Checks 02–07 and 09 were compared as complete source blocks against the starting HEAD and are byte-identical. No production fixes were pulled into this lane.

## Review response

This bounded follow-up starts from clean `62f1c1645b63f72125def83838e20847ece04e52` on the same branch/PR #264. Only the same four tooling/documentation files change. No Unity, Player build, production edit, coordination/ledger edit or merge. The only compilation is the explicitly requested plain-.NET SnapFar driver, staged in an automatically removed OS temporary directory outside the repository; no UnityEngine references, SDK/package installation or production runtime build.

1. **Coverage gap — rotation policy and snapping math.** Original 06 remains exactly unchanged as a physical-input observation. New 06a locates the real `Propose` call within `OnPointerMove` (comments/string contents excluded), extracts the snap expression, and evaluates its restricted boolean grammar for Reshape/Create/Move × Shift up/down. Default `pre-fix` expects `[True, False, True, False, True, False]`: the known baseline snaps rotation unless Shift is held. Explicit `--rotation-state post-fix` instead expects `[False, False, True, False, True, False]`: rotation always receives false; create/move retain !ShiftHeld. The expected state is selected explicitly, never auto-accepted from source; each state rejects the other. In-memory controls confirmed both directions, always true/false, reversed Shift, rotation-only snapping and a decoy call outside the method. Unsupported expressions report FAIL. This proves the argument's truth table for this supported source structure, not execution/reachability, actual ShiftHeld input, forwarding correctness, physical pointer response or #256 acceptance. Move receives the argument but its current branch uses MoveAlongHeadland without SnapFar. Original 06's wording is intentionally retained as a pre-fix observation; consult 06a on later sources.

   `RackEditing.cs` and `RackGeometry.cs` are Unity-free and reachable on installed .NET 8. New 06b directly compiles those production files with an embedded six-case driver and calls actual SnapFar; no port/stub. Within/outside angle and boundary windows and disabled near-angle/near-boundary cases all pass endpoint tolerance 0.001 m, flags and parallel ID 0. Raw diagnostics below report endpoints and returned angles. The six cases contribute one tally row. They prove only those isolated inputs; they do not exhaust nearby-rack snapping/ties, threshold edges, geometry validation, downstream gesture integration or real input. The entrance (0,0) is an isolated snapping fixture, not a qualified headland edit.

2. **Weak check 01 — presentation preference.** The predicate is unchanged; row/name/note now explicitly say requested presentation format, not functional defect, expected to flip with #255. It proves compact 16–20m/16-20m copy in this concept, not functional correctness, readability or rendered layout.

3. **Weak check 08 — label-specific assertion.** Replaced literal `Plan Racks` requirement with exactly one `btn-primary` button invoking `ui.ShowMap()`. All button labels/classes/callbacks remain evidence; baseline primary invokes `ui.CloseAll()` and fails. In-memory full-check controls showed an arbitrary label plus ShowMap passes; Plan Racks plus CloseAll fails; ShowWorkPlan fails. This proves only source primary class and map action, not contextual objective-state suitability, focus, prominence or live destination behaviour. Future callback/refactor styles need deliberate checker review.

The original reviewer accepted a source-level baseline, not gameplay qualification; this follow-up likewise awaits independent review/Manager acceptance. No child agents or new PR. Live dashboard/coordination ingestion was not verified; authorised handoff is via the same Git branch/PR, with those records outside this worker's write scope.

Raw stdout (verbatim):

```text
SOURCE-STATIC baseline-source-commit: 72b4efbd1c653fa2231f632a79030a83ff76f833
SOURCE-STATIC tracked-production-vs-baseline: MATCH
SOURCE-STATIC scope: current checkout; no runtime/render/input proof; no automatic baseline update
SOURCE-STATIC FAIL 01 requested-spacing-presentation-format: Requested presentation format, not a functional defect; expected to flip with #255. Spacing in metres, not a duration; require compact 16–20m/16-20m.
  SOURCE-STATIC Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs:87: "Extraction racks allow machinery to reach trees without creating access everywhere across the stand. "
  SOURCE-STATIC Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs:88: "In early CCF transformation, rack spacing may commonly be around 16–20 m, but appropriate spacing depends on the stand, machinery and site. "
  SOURCE-STATIC Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs:89: "Scenario One uses that range as a guide for this plantation. "
  SOURCE-STATIC Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs:90: "This stand needs five extraction racks to create a workable first access layout. "
  SOURCE-STATIC Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs:91: "Plan the racks on the map, then mark each rack entrance in the forest."
SOURCE-STATIC FAIL 02 tree-marking-heading: Require exact heading literal Introducing tree marking; literal presence alone cannot prove display.
  SOURCE-STATIC Assets/ForestPrototype/UI/MenuHelpView.Text.cs:46: case Menu.TreeMarks: return "Tree Marks · what the paint on a tree means";
SOURCE-STATIC FAIL 03 source-material-copy: No CCF source material phrase in ForestPrototype C# string literals (comments excluded).
  SOURCE-STATIC Assets/ForestPrototype/UI/MenuHelpView.Text.cs:28: "Removal trees associated with a machine route can be marked with vertical stripes. The CCF source material describes removal/treatment colours such as orange or pink; Scenario One uses orange."
SOURCE-STATIC PASS 04 orange-entry-is-not-fell: Bounded legend check: orange rack-removal entry is separate from Red/Fell; no claim about rendered paint.
  SOURCE-STATIC Assets/ForestPrototype/UI/MenuHelpView.Text.cs:21: new[] { "Orange vertical stripes", "Rack / machine-path removal" },
  SOURCE-STATIC Assets/ForestPrototype/UI/MenuHelpView.Text.cs:22: new[] { "Red", "Fell" },
  SOURCE-STATIC Assets/ForestPrototype/UI/MenuHelpView.Text.cs:99: + "A tree carries only one of these marks: the spray will not mark a Fell or Crop Tree, and X or C cannot change an orange mark. "
SOURCE-STATIC PASS 05 no-formal-pro-silva-copy: Conservative lexical guard: no Pro Silva mention in ForestPrototype C# literals; any mention requires human review, not automatic classification as a standard.
  SOURCE-STATIC Assets/ForestPrototype/UI/MenuHelpView.Text.cs:29: public const string TreeMarksSimplificationText = "The game asks you to mark the rack entrance tree as a teaching action. Real marking practice may mark additional trees along a route.";
SOURCE-STATIC NOT-TESTABLE 06 round-handle-input: BASELINE OBSERVATION only: snap argument is !ShiftHeld(e.modifiers), then forwarded to SnapFar for Reshape. SOURCE-STATIC cannot test physical input.
  SOURCE-STATIC Assets/ForestPrototype/UI/Map/StandMapRackEditor.cs:190: if (SelectedRackId > 0 && manager.TryGetRack(SelectedRackId, out RackPlanRack selected) && OnHandle(px, selected))
  SOURCE-STATIC Assets/ForestPrototype/UI/Map/StandMapRackEditor.cs:192: next = Gesture.Reshape;
  SOURCE-STATIC Assets/ForestPrototype/UI/Map/StandMapRackEditor.cs:251: private static bool ShiftHeld(EventModifiers modifiers)
  SOURCE-STATIC Assets/ForestPrototype/UI/Map/StandMapRackEditor.cs:252: => (modifiers & EventModifiers.Shift) != 0 || (Keyboard.current != null && Keyboard.current.shiftKey.isPressed);
  SOURCE-STATIC Assets/ForestPrototype/UI/Map/StandMapRackEditor.cs:260: Propose(px, !ShiftHeld(e.modifiers), out RackPoint entrance, out RackPoint far, out RackSnapResult snap, out RackEditValidation check);
  SOURCE-STATIC Assets/ForestPrototype/UI/Map/StandMapRackEditor.cs:325: case Gesture.Reshape:
  SOURCE-STATIC Assets/ForestPrototype/UI/Map/StandMapRackEditor.cs:327: snapResult = RackEditing.SnapFar(bounds, entrance, pointer, suggestedAngles, manager.RackEditOthers(gestureRackId), snap);
  SOURCE-STATIC Assets/ForestPrototype/UI/StandMapView.cs:601: UiKit.Add(card, "Drag a rack to slide it along the headland. Drag its round handle to rotate it. Hold Shift to turn snapping off.", "faint");
SOURCE-STATIC PASS 06a rotation-snap-policy: Expected state=pre-fix; observed Reshape/Create/Move (Shift up,down)=[True, False, True, False, True, False]. Pre-fix requires [True, False, True, False, True, False]; after #256 require [False, False, True, False, True, False] with --rotation-state post-fix. PASS confirms the selected source baseline, not physical input; Move receives the argument but does not call SnapFar.
  SOURCE-STATIC Assets/ForestPrototype/UI/Map/StandMapRackEditor.cs:260: Propose(px, !ShiftHeld(e.modifiers), out RackPoint entrance, out RackPoint far, out RackSnapResult snap, out RackEditValidation check);
SOURCE-STATIC PASS 06b snapfar-offline-cases: Six direct calls to production SnapFar on plain .NET; 0.001m tolerance; no Unity or gesture dispatch proof.
  SOURCE-STATIC Assets/ForestPrototype/Racks/RackEditing.cs:259: public static RackSnapResult SnapFar(RackEditBounds bounds, RackPoint entrance, RackPoint desiredFar,
SOURCE-STATIC PASS SnapFar near-angle enabled=True input=(20.000,0.700) far=(20.012,0.000) angle=0.000 angleSnapped=True boundarySnapped=False
SOURCE-STATIC PASS SnapFar near-angle-disabled enabled=False input=(20.000,0.700) far=(20.000,0.700) angle=2.005 angleSnapped=False boundarySnapped=False
SOURCE-STATIC PASS SnapFar outside-angle-window enabled=True input=(20.000,3.000) far=(20.000,3.000) angle=8.531 angleSnapped=False boundarySnapped=False
SOURCE-STATIC PASS SnapFar near-boundary enabled=True input=(39.000,0.000) far=(40.000,0.000) angle=0.000 angleSnapped=False boundarySnapped=True
SOURCE-STATIC PASS SnapFar near-boundary-disabled enabled=False input=(39.000,0.000) far=(39.000,0.000) angle=0.000 angleSnapped=False boundarySnapped=False
SOURCE-STATIC PASS SnapFar outside-boundary-window enabled=True input=(37.000,0.000) far=(37.000,0.000) angle=0.000 angleSnapped=False boundarySnapped=False
SOURCE-STATIC PASS 07 objective-next-instruction: Unconfirmed V2 instruction names Stand Map / Plan Racks path; later states are quoted separately.
  SOURCE-STATIC Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs:100: if (!confirmed) return counter + ". Plan Racks on the Stand Map [M], then choose Confirm Rack Plan.";
  SOURCE-STATIC Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs:102: return counter + ". Return to the forest and mark the first rack tree nearest the path at each rack entrance: [P] spray marker, aim within 3.5 m, left-click.";
  SOURCE-STATIC Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs:103: return counter + (workPending ? ". Next, review and approve the rack removal work in the Work Plan [Tab], then advance one year."
  SOURCE-STATIC Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs:104: : ". Open the Work Plan [Tab] and advance one year.");
  SOURCE-STATIC Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs:116: case "rack.create": return "Open the Stand Map [M] and choose Plan Racks. Create and edit your first rack on the map; no trees are cut by planning.";
SOURCE-STATIC FAIL 08 objective-primary-cta: Require exactly one primary source button invoking ui.ShowMap(); labels are evidence only. Baseline primary invokes ui.CloseAll(); visual prominence, focus and destination behaviour remain unmeasured.
  SOURCE-STATIC Assets/ForestPrototype/UI/LearningObjectivesView.cs:73: buttons.Add(UiKit.Button("Stand map [M]", () => ui.ShowMap()));
  SOURCE-STATIC Assets/ForestPrototype/UI/LearningObjectivesView.cs:74: buttons.Add(UiKit.Button("Work Plan [Tab]", () => ui.ShowWorkPlan()));
  SOURCE-STATIC Assets/ForestPrototype/UI/LearningObjectivesView.cs:75: buttons.Add(UiKit.Button("Back to forest [O / Esc]", () => ui.CloseAll(), true, "btn-primary"));
SOURCE-STATIC NOT-TESTABLE 09 map-minimap-trail-consistency: NOT STATICALLY TESTABLE: no demonstrated coordinate mismatch for the same confirmed rack/waypoint. Shared source/draw path is evidence, not rendered parity; unconfirmed racks are deliberately FullOnly.
  SOURCE-STATIC Assets/ForestPrototype/UI/HudMinimapView.cs:84: StandMapModel model = ui.MapSource != null ? ui.MapSource.Model : null;
  SOURCE-STATIC Assets/ForestPrototype/UI/HudMinimapView.cs:87: if (show) surface.Sync(model);
  SOURCE-STATIC Assets/ForestPrototype/UI/StandMapView.cs:219: StandMapModel model = force ? ui.MapSource.Rebuild() : ui.MapSource.Model;
  SOURCE-STATIC Assets/ForestPrototype/UI/Map/StandMapSource.cs:279: foreach (RackPlanRack rack in ui.Manager.RackPlan)
  SOURCE-STATIC Assets/ForestPrototype/UI/Map/StandMapSource.cs:282: Start = rack.Entrance,
  SOURCE-STATIC Assets/ForestPrototype/UI/Map/StandMapSource.cs:283: End = rack.Far,
  SOURCE-STATIC Assets/ForestPrototype/UI/Map/StandMapSource.cs:290: FullOnly = !confirmed && !rack.IsLocked,
  SOURCE-STATIC Assets/ForestPrototype/UI/Map/StandMapSource.cs:184: model.HasWaypoint = ui.Map != null && ui.Map.TryGetWaypointTarget(out target);
  SOURCE-STATIC Assets/ForestPrototype/UI/Map/StandMapSource.cs:186: model.Waypoint = new RackPoint(target.x, target.y);
  SOURCE-STATIC Assets/ForestPrototype/UI/Map/StandMapDrawPlan.cs:106: if ((rack.MiniOnly && full) || (rack.FullOnly && !full) || StandMapModel.IsDynamicRack(rack.Symbol) || !StandMapSymbols.IsDrawn(rack.Symbol, detail)) continue;
  SOURCE-STATIC Assets/ForestPrototype/UI/Map/StandMapDrawPlan.cs:136: if ((rack.MiniOnly && full) || (rack.FullOnly && !full) || StandMapModel.IsDynamicRack(rack.Symbol) || !StandMapSymbols.IsDrawn(rack.Symbol, detail)) continue;
  SOURCE-STATIC Assets/ForestPrototype/UI/Map/StandMapDrawPlan.cs:126: foreach (RackPoint p in path.Points) item.Points.Add(t.WorldToPixels(p));
  SOURCE-STATIC Assets/ForestPrototype/UI/Map/StandMapDrawPlan.cs:257: MapPosition c = t.WorldToPixels(model.Waypoint);
  SOURCE-STATIC Assets/ForestPrototype/UI/Map/StandMapTransform.cs:38: return new MapPosition((world.X - MinX) / SizeM, 1f - (world.Z - MinZ) / SizeM);
SOURCE-STATIC TALLY PASS=5 FAIL=4 NOT-TESTABLE=2
SOURCE-STATIC exit=1 means feedback assertions remain unmet; observations do not count as PASS.
```

## Bounded manual Unity reproduction — future authorised worker only

These are **unexecuted manual steps**, not evidence. First obtain a fresh exclusive Unity lease and designated Unity-capable lane; verify exact candidate SHA, ownership, clean status, disk/process admission, Unity **6000.6.0f1** and installed packages. Use an isolated/disposable save and preference profile; preserve the user's real save. Do not use this offline worktree as an Editor lane. No Player or Windows build is requested. Record full commit, profile/setup, resolution, input device and screenshots/video under durable `/media/jer/Files/CCF/` evidence storage. Test 1280×720 and 1920×1080 with the same UI scale; distinguish baseline from any later fix candidate.

1. **Objective concept and next action.** Start a fresh Enlarged80 Scenario One with Rack V2 active and no confirmed rack plan; open Objectives [O]. Capture the concept, instruction and header buttons. Baseline source predicts spaced metre text and a generic primary close button; the feedback target is compact unit text and a clear planning action. Click the existing Stand map button, then choose Plan Racks. Record whether this path works, whether planning mode needs a second action, initial keyboard focus and what Back to forest actually does. Repeat instruction capture after confirming a valid five-rack plan, after partial entrance marking, and after all entrances are marked; compare with the four state-dependent branches quoted above. Do not infer an automatic planning-mode transition from the instruction text.
2. **Tree-mark introduction and paint.** With the isolated V2 plan confirmed and first-use help unseen, return to the forest. Capture Tree Marks first-use and F1 full help, including heading and orange/red/blue legend; record whether the requested introduction and unwanted source phrase appear. Equip P, aim within the displayed 3.5 m limit at an eligible entrance tree, spray once and walk around it. Record the actual stripe colour, orientation and four-side appearance. Inspect a separate disposable red Fell / blue Crop example without executing removals. Do not relabel orange as Fell or infer a formal Pro Silva standard. If first-use suppression prevents the screen, record preference setup and use the existing reset-help option only in the isolated profile.
3. **Round rotation handle.** Select one unlocked draft rack in Plan Racks. Drag the round far-end handle through the same near-parallel/suggested and intermediate angles first without Shift, then with Shift. Capture ghost, final endpoints, displayed bearing and status on release. Baseline source predicts snapping can engage without Shift and is disabled with Shift; actual hit testing, pointer capture and angle response are unmeasured. Repeat body drag to distinguish movement from rotation. After a future #256 fix, compare free handle rotation without Shift while ordinary draw/move snapping remains as specified; use that fix's own acceptance packet rather than guessing it here.
4. **Stand Map / minimap trails.** In the same isolated world record suggested, draft, confirmed and executed states separately. Set a waypoint on an identifiable rack entrance, then on its far end and on a known path bend; capture Stand Map and walking minimap without changing destination or world state. Compare normalized location, north orientation, rack endpoints, path shape and waypoint marker, accounting for map scale, symbols, minimum stroke widths and the intentional draft/guidance visibility differences. Record actual source rack IDs/endpoints and waypoint coordinates if an approved existing diagnostic permits it. A mismatch requires the same data/state and measurable differing positions; different dash styles or hidden draft racks alone do not demonstrate the claimed trail error. Save/reload comparison, if authorised later, must use this disposable profile and be reported separately.

## Explicitly UNMEASURED / remaining gates

- Unity import/compilation, Editor runtime, actual physical keyboard/mouse input, pointer hit/capture/release, snapped/unsnapped angle response and ordinary movement snapping.
- Which text is actually displayed in each first-use/F1/objective state; rendered font/unit/dash/layout/overflow, colour/stripe geometry, visual prominence, keyboard focus, accessibility and button navigation/destination behaviour.
- Same-state Stand Map/minimap/path/rack/waypoint rendered alignment, update timing and post-edit/confirm/execute/reload parity. No static inconsistency was demonstrated and no visual defect is disproved.
- Dynamically constructed, localized, serialized or externally loaded player text outside the lexical inventory; exhaustive orange/Fell semantic contradictions; independent ecological/marking-convention or formal Pro Silva authority.
- Simulation/ecology, harvesting only once, resource/cost awards, construction consumption, regeneration, persistence/save/load, Legacy40/frozen Reference Future and protected-hash runtime regressions. Production source was unchanged; these gates were not rerun.
- Runtime performance, hardware/platform behaviour and any Player/Windows build. No #255/#256 implementation or later combined candidate was tested.
- Independent cross-provider review, Manager acceptance/integration and dashboard synchronization. This worker publishes the PR/handoff; DPC/#200 retain issue claim, routing ledger, live coordination and dashboard ownership. Their final updates were not verified here.

## V2 Orchestration Decision Appendix

| Field | Decision / provenance |
|---|---|
| Dispatch event | **EVENT 011**, pool **codex_c75d8114**, **gpt-6.1-sol medium**, as explicitly supplied in this task. No claim that this worker independently audited the DPC-only journal. |
| Route and scope | Assigned OFFLINE VERIFICATION worker for #260 under #254; Tier 1 source checks, standard-library Python (existing Tools Python precedent inspected). |
| Context / authority | Exact candidate and context SHA above; live GitHub active-tasks and master packet inspected read-only. User's latest owned-path and machine rules control. |
| Capacity / escalation | No child agents, other provider, effort escalation, quota/reset wait or Unity lease used. Provider quota/reset values: UNKNOWN. Parent routing/cost accounting remains with DPC. |
| Alternative rejected | A static PASS for real rotation or trail rendering would overstate evidence. Keep rotation as observation and trail as NOT STATICALLY TESTABLE; hand off bounded manual steps. |
| Numeric provenance | Review-response eleven rows parsed from raw stdout, reconciled to its TALLY: 5 PASS / 4 FAIL / 2 NOT-TESTABLE; sum = 11 (06b case diagnostics counted once). Exit status 1 is a feedback-baseline failure, not a runtime gate or script crash. Two reruns were byte-identical. |
| Source provenance | Each quoted value has the exact candidate-relative path and line in stdout. Git tracked production comparison MATCH; no rebaseline or fixture weakening. |
| Outcome / next | Tooling and honest baseline ready for independent review; DPC arranges cross-provider review and #200 decides acceptance/integration. Later authorised Unity worker performs manual matrix. No protected-branch merge or runtime acceptance. |

This appendix cites EVENT 011 without modifying `ROUTING-LEDGER.md` or `active-tasks.md`. Publication is via the verification branch PR to main, not integration into the protected player-test candidate.
