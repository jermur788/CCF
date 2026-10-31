# MFM-08 — Successor Tier-2 (repaired candidate) — claim #156

**Result: `MULTI-SCALE REVIEW UX QUALIFIED — READY FOR MANAGER REVIEW`** — with the red regression gates below disclosed. All of them reproduce identically (or worse) on the pre-MFM-08 parent `cfdc93bf`, so none is caused by the candidate.

- **Provider/model:** Claude Code, Sonnet 5.5.
- **Candidate qualified:** `e5fb424635f16182748efcb0539e8a49539495a9` (detached, clean, `/media/jer/ZX20/CCF-U1`, Unity 6000.6.0f1). No production file changed.
- **Harness lineage:** `qual/mfm-08-unity-v1` `630c6c3d` plus one local addition (below). The two original defect assertions and every other check are untouched.
- **Lock:** normal `unity-lock`, lock_id `c4ae8720d3a94a528f517f7b96b1ffb4`, task `MFM-08-156-TIER2-UNITY`, acquired 2026-10-30T21:55:50Z, released normally (forced: false), final status `free`, no Editor or Xephyr left. `--branch HEAD` was needed because the lane is detached.
- **Evidence (all under `/media/jer/Files/CCF/Verification/`):** `MFM08-Tier2-successor-20261030T215603Z` (run 1), `...-run2-20261030T220340Z` (final MFM-08 gate), `...-regress-20261030T220935Z`, `...-rackv2-20261030T222544Z`, `...-rackv2-diag-20261031T005050Z`, `...-rackv2-base2-20261031T030358Z`, `...-rackv2-final-20261031T032748Z`, plus lock receipts.

## MFM-08 live gate
- Run 1, unmodified harness: `MFM08_UNITY_PASS checks=353 shots=23`. The segment/footprint checks and the two former Defect checks all pass: Low/Centre/High footprints are distinct, the sample is the fixed teaching footprint, tags no longer overlap, Inspect uses the area-specific centre.
- Run 2: `MFM08_UNITY_PASS checks=373 shots=25`. The 20 extra checks are the **open-work conflict** I added, which the old harness lacked: a proposed tree is Fell-marked, moved into the Work Plan through `AddMarkedTreesToWorkPlan()`, and the review is re-read. The tree is held out of the proposals, the hold is shown as `CONFLICT — held, not proposed`, the player's open order is not overwritten or added to, and removing it through `RemovePendingOrder` restores the identical proposal signature. 1280x720 and 1920x1080 captures with `tags=15 overlaps=0`.
- **Note for the Manager:** in both conflict cases production reports `UnitNoLongerCovered` for the unit rather than the per-tree `ManualFellMark` / `OpenWorkOrder` kind, because an open felling order or mark takes the unit out of Covered. The player's state is untouched either way. The per-tree kinds are not reached by this fixture.
- Everything else from the earlier run holds (Previous/Next bounded, real [C]/[N], no bulk confirm, Prepare creates a prescription only, proposals noncommitted, waypoint alone not enough but a real 3D inspect is, approval creates Pending work with provenance, repeated approval refused, in-memory v23 round trip, no save written).

## Unity regressions (candidate)
| Gate | Result |
|---|---|
| MFM-06 compile / save | PASS |
| MFM-07 approval boundary | PASS |
| MFM-08A coverage world | PASS |
| Reference Future v1 (protected hashes) | PASS |
| Save hardening | PASS |
| Scenario One completion | PASS |
| Scenario One interaction | PASS |
| Scenario One progress | gate token `SCENARIO_ONE_PROGRESS_VERIFY_PASS`, but the runner reports `pass: false` only because the gate wrote an isolated `forest-save.json.bak` (the runner allows disk saves only for save/approval/coverage-world); host saves unchanged. Judge accordingly. |
| Rack V2 field-marking / runtime / save | PASS (99 / 14 / 35 checks) |
| Rack V2 Legacy40 rack authoring | PASS (15) |
| Rack V2 confirm / minimap | PASS (213) on rerun |
| Rack V2 combined-v2 | **447 pass / 11 fail**, same on pre-MFM-08 parent `cfdc93bf` (447/11) |
| Rack V2 guided-v2 | **1 fail** ("V2 confirmed rack lesson clearly requests annual advance"), same on `cfdc93bf` |
| Rack V2 combined-legacy | 440 / 8 fail (440/8 on a rerun); `cfdc93bf` is worse (416/32) |

Combined-v2's earlier green record (455/0 at RackV2LayoutRepair) was on an older head, so these three gates are red on the integration lineage independent of MFM-08; they should be raised separately.

## Deviations and caveats
- Three first-attempt runs (legacy40, combined-legacy, confirm-minimap) hung at Editor start with no verdict and timed out at 2400 s; they passed or completed on rerun. The two first baseline runs on `cfdc93bf` hung the same way and were rerun.
- `RackV2FieldMarking/run_gate.py` hard-requires the RACK-V2-09 lock owner. I ran an unmodified-logic copy with only that owner check and the ROOT/HERE paths changed (scratchpad, not committed); gate sources are the lane's own.
- Not run: standalone offline suites (previous green offline MFM-04/05/06/07/08A/08 evidence retained; harness change is Unity-only), `RackV2MapPlanning`, Legacy40 compatibility beyond the Legacy40 rack gate.
- **Clean return:** lane back at `e5fb4246`, clean; Editor side effects (`.vscode/settings.json`, SectionFive materials, `Assets/_Recovery` scene recovery copies) were saved as diffs/copies next to each run and reverted; the player's host `forest-save.json*` is unchanged (runner checks); lock free. No Windows build.

---

# MFM-08 — Stand Map / 3D Review UX — Tier-2 Unity Result (claim #156)

**Result: `BLOCKED — GEOMETRY CONFLICT: area footprints ignore the Low/Centre/High segmentation`**

The success token `MULTI-SCALE REVIEW UX QUALIFIED — READY FOR MANAGER REVIEW` is **not** returned. This was verification-only: no production file was changed.

- **Provider/model:** Claude Code, Sonnet 5.5 (Ultra-routed).
- **Production candidate qualified:** `d55ec86a3f0bf13820f2080197b6b0f43d832e06`. The docs tip `d70cec17` differs from it by `HANDOFF.md` only. It was executed in `/media/jer/ZX20/CCF-U1`, detached and clean at that SHA. The `/media/jer/Videos/CCF-MFM08` worktree is not a registered Unity lane and was not touched.
- **Verification head:** branch `qual/mfm-08-unity-v1` (this commit). It adds only `Tools/Verification/MfmTreatmentReviewUnity/` and this file.
- **Unity:** 6000.6.0f1. The candidate compiled with no error in any candidate file.
- **Lock:** normal `unity-lock`, owner `Claude Code (Sonnet 5.5) MFM-08 Tier-2`, task `MFM-08-156-TIER2-UNITY`, lock_id `3694899195804cd088fd9b9246a07bcf`. Acquired 2026-10-30T20:05:01Z, released normally (forced: False), final status `free`, no Editor processes. The lock tool first had to be repaired under #189 (stale `storage.device` in `CCF-Ops/lanes.json`).
- **Evidence root:** `/media/jer/Files/CCF/Verification/MFM08-Tier2-20261030T203629Z-run8-final/` holds the log, 24 captures, `footprints.txt`, `defects.txt`, `findings.txt`, `notes.txt` and lock records. Earlier runs `run1` to `run7` sit beside it, and the lock receipts are `MFM08-Tier2-unity-lock-*.txt` in the same parent directory.

## The blocker (verified, not repaired)

Accepted semantics (`ScenarioOneCoverage` header comment and `Area.Segment`): each rack-served unit is split along the rack axis at the sample anchor ± `SegmentHalfLengthM` into **three areas, Low / Centre / High** (`R1L`, `R1C`, `R1H`, and so on). The teaching unit's **Centre** area is the sample.

`StandMapTreatmentReview.Footprints()` and `TryAreaCentre` match an area to geometry by `UnitIndex + RackId` only and ignore `Segment`. Measured live (`footprints.txt`): **all 5 units draw all 3 of their areas with one identical polygon**, the whole 80 m strip. For example rack 1 (`R1L=NotCovered | R1C=Covered | R1H=Covered`) is one rectangle, and rack 3 (`R3L=Covered | R3C=Manual/SAMPLE | R3H=Covered`) is one rectangle.

Consequences, visible in the captures and measured:

1. **The source-sample footprint is wrong.** The sample is drawn as the whole 80.0 m strip of an 80.0 m unit. This is the explicit Tier-2 item "authoritative source-sample footprint alignment".
2. **Per-area state is not spatially readable.** The one NOT COVERED area (R1L) and the SAMPLE are painted under two other areas of the same strip. All 15 printed tags sit at only 5 positions (`findings.txt`: 15 of 15 tags overlap at identical rectangles). The SAMPLE tag is hidden under COVERED, and NOT COVERED is hidden under COVERED (see `01-review-card-states-*.png`, bottom strip).
3. **"Inspect in forest" for Low / Centre / High of one unit goes to the same unit centroid**, so it does not take the player to the area it names. That also affects where the 3D spot-check is naturally performed.
4. **My first containment check was too weak.** Candidate-glyph containment passes only because the polygons overlap. An independent transform fit (candidate glyph pixels against tree world positions) found 0.00 m deviation from the *unit* polygon. That confirms the footprints equal the unit polygon exactly, so the segment split was never applied.

Fixing it needs a production change: derive per-segment polygons from the anchor and `SegmentHalfLengthM` (or expose them from the coverage layer) and use them in `Footprints`, `TryAreaCentre` and the tags. That is outside verification authority under #156, so it is returned to the Manager for separate ownership. No test was weakened or skipped. The gate fails on the two `Defect` checks and nothing else.

## What passed on the exact candidate

The final run had 351 checks pass. Run 7, before the segment checks were added, ended `MFM08_UNITY_PASS checks=351`.

Everything below went through the real Stand Map UI (button invocation) and real OS keyboard events from the private-display driver (`xdotool` in Xephyr/openbox). The world is the Rack V2 Enlarged80 world built with production APIs.

- **Rendered at 1280x720 and 1920x1080.** The game view size and capture size were asserted. The review card scrolls inside the window (`maxHeight` bounded to the map frame) and the layer is painted. 12 states were captured at both sizes (`captures.txt`). On visual review of the 1280x720 captures, the card, legend and map are legible; the tag overlap above is the visible defect.
- **Review card:**
  - The sample row is labelled as the source, not a target.
  - 13 Covered, 1 Not covered, and 2 remainders each listed with a reason.
  - The legend states "shape and pattern, not colour alone".
  - "Covered means comparable, not committed".
  - Include / Leave manual are offered only on Covered areas, and are a draft choice that writes no record, order or mark.
- **Form, not colour:** each area kind has its own outline/hatch pattern. A candidate is a square (4 points) and a proposal is a triangle (3 points).
- **Candidates:**
  - They come from real spacing candidates on living trees.
  - Next walks each open candidate once and stops at the last. Previous is disabled before selection and at the first, so there is no wrap.
  - Map navigation changed no mark, order, record or hash.
  - No confirm or confirm-all control exists. Inspect sets the existing waypoint only.
  - The candidate is presented as an inspection prompt.
- **3D with real keys:**
  - [E] inspects, and the chip "CANDIDATE — inspection prompt" is shown.
  - Real **[C]** confirms through the ordinary Crop Tree mark (exactly +1 Crop Tree, and the state derives from the live mark).
  - Real **[N]** rejects with no mark, order, record or hash change. [N] again reopens. [N] on a non-candidate does nothing.
  - [C] toggles back, and Esc ends inspection.
- **Prepare:** creates exactly one Accepted prescription and **no** order, event or Fell mark. The card says "prepared, not approved".
- **Proposals:** each is labelled "PROPOSAL — not a Fell mark and not yet work", and none is worded as a Fell mark. The preview is read-only.
- **Conflict:** a manual Fell mark on a proposed tree is held back and shown as `CONFLICT — held, not proposed` (production reports `UnitNoLongerCovered`). The player's own mark is untouched, no work is created, and removing the mark restores the identical proposals. The open-work conflict was not separately exercised.
- **Spot-check:**
  - A map waypoint alone leaves "3D spot-check not done" and Approve disabled.
  - The approval handler re-checks the gate (invoked directly, nothing happens).
  - A real [E] inspection of a proposed tree in an accepted Covered unit (fresh `ReadCoverage`) completes it, and Approve becomes available.
  - The proposal chip appears in 3D, and no Fell mark is created.
- **Approval boundary:**
  - Approve creates exactly the previewed orders, all **Pending** `FellTree`, each with prescription, coverage-unit, Crop Tree and closed-reason provenance.
  - No order is Approved, and there is no cash, year or tree change and no Fell mark.
  - The card states Pending, not approved for execution.
  - **Repeated approval is refused** (both the UI handler and the manager) and adds nothing.
  - Opening the Work Plan approves nothing. Ordinary Work Plan approval remains the commitment step and then approves the orders. Approved is not executed.
- **Save/world:** in-memory v23 validation and reload preserve the world hash. No save file was written and the original world was restored (isolated `XDG_CONFIG_HOME`).
- **Declared fixture shortcut:** after the real [C]/[N] steps, the remaining spacing candidates were confirmed through `ForestTreeMarkingManager.Mark` (the production path), only so that a prescription could exist. The UI itself never bulk-confirms.

## Regression and protected compatibility

- **Offline, all green on the candidate tree** (`/media/jer/Files/CCF/Verification/MFM08-Tier2-offline-regression/`):
  - MFM-04 core 240/0.
  - MFM-05 candidates 95/0.
  - MFM-06 save provenance 172 pass.
  - MFM-07 approval boundary 125/0.
  - MFM-08A coverage 102/0.
  - MFM-08 treatment review 106/0.
  - Each has a byte-identical repeat.

  The MFM-08 `BOUNDARY_FAIL` printed there is only the offline check seeing this verification harness in the worktree. The candidate's own 12-file diff from `cfdc93bf` is entirely inside the owned surface.
- **Unity regressions NOT run** (Rack V2 map/runtime/field, MFM-06/07 Unity, Reference Future v1 protected hashes, UI gate). I deliberately did not spend lock time on a candidate being returned for a production fix; they must be run on the repaired candidate. Nothing in this run touched save, schema, hash, reference or Work Plan files.

## Commands

- `python3 -B Tools/Verification/MfmTreatmentReviewUnity/run_gate.py --out <fresh evidence dir>` is the runner. It requires the held lock and the clean exact SHA, stages the gate, runs it, and removes the staged files.
- `python3 -B Tools/Verification/<Mfm suite>/run_*_offline.py` runs the offline regressions.

Run history:

| Run | Outcome |
|---|---|
| run1 | harness compile error (my own typo) |
| run2 | harness assumption: [E] while another tree was inspected |
| run3, run4, run7 | gate pass (347 / 347 / 351 checks) |
| run8 (final) | adds the segment/footprint checks: 351 pass, 2 verified defects |

## Changed-file boundary and clean return

- **Production:** none. Added only `Tools/Verification/MfmTreatmentReviewUnity/{MfmTreatmentReviewUnityGate.cs,run_gate.py}` and this file on `qual/mfm-08-unity-v1`. The harness is staged into the lane at run time and removed. Verified: lane clean at `d55ec86a`, no leftover staged files, no `mono_crash`, no Editor, no Xephyr.
- **Editor normalisation:** after each run, `.vscode/settings.json` and 5 SectionFive materials (`_Color` noise) were saved as diffs in `MFM08-Tier2-editor-normalisation/` and reverted.
- **Lock** released normally, status `free`. No Windows build.

---

# MFM-08 — Stand Map / 3D Review UX — Offline Candidate Handoff

**Claim:** #156 · **Parent:** #91 / #106 · **Mode:** offline/static only (no Unity run in this phase)
**Provider/model:** Claude Code, Sonnet 5.5 (medium)
**Branch:** `feature/mfm-08-map-3d-review-ux-v1` · **Worktree:** `/media/jer/Videos/CCF-MFM08`
**Exact start (Manager repin `6511551513`):** `cfdc93bf3943125d21570bc5749a9338f52d1cce`
**Production candidate SHA (all code):** `d55ec86a3f0bf13820f2080197b6b0f43d832e06` (first slice `4c1e25f2`). This HANDOFF is a docs-only successor commit on top; its SHA is the branch tip and is posted on #156.

## Verdict
Offline/static portion **passes**. The candidate is **not Unity-verified**: no compile in the Editor, no rendering, no interaction was run. Unity Tier-2 qualification (1280×720 and 1920×1080, interaction, regressions) is still required and is scheduled separately by #91.

## Changed files (12 vs start; nothing else)
Production:
- `Assets/ForestPrototype/UI/ScenarioOneTreatmentReview.cs` (new, pure, no UnityEngine): wording, closed-reason text, area rows, `Session` (draft choices, rejections, candidate cursor, active prescription, 3D spot-check latch), approval gate, 3D tree context.
- `Assets/ForestPrototype/UI/StandMapTreatmentReview.cs` (new): the Treatment review side card, map-layer feed, and the 3D companion (`OnInspected`, `ContextFor`).
- `Assets/ForestPrototype/UI/Map/StandMapReviewPlan.cs` (new, pure): per-state outline/hatch patterns, candidate and proposal glyph shapes.
- `Assets/ForestPrototype/UI/Map/StandMapReviewLayer.cs` (new): its own element over the map surface; paints the plan and prints an area tag. The shared canvas, draw plan, model and HUD minimap are untouched.
- `Assets/ForestPrototype/UI/StandMapView.cs`: `Layer.Review` tab "Treatment review" (only when `RackPlanningV2Active`), layer element, side-card branch, `SetWaypointAtWorld`, state key.
- `Assets/ForestPrototype/UI/TreeInspectionView.cs`: calls `OnInspected`, adds a candidate/proposal chip and line, and the contextual key hint.
- four `.meta` files for the new scripts.
Verification: `Tools/Verification/MfmTreatmentReview/` (runner + `OfflineLogic/TreatmentReviewOfflineCheck.cs`).
**Not touched:** `ScenarioOneUiRoot.cs`, `AnnualReviewView.cs`/history, `ScenarioOneManager*`, coverage/calibration/world adapter, save/schema/validator/hash/reference, Work Plan code, learning/curriculum, USS, scenes/prefabs, Windows build.

## APIs consumed
- Coverage: `ScenarioOneManager.ReadCoverage(out string problem, IReadOnlyCollection<string> manualUnitIds = null)` returning `ScenarioOneCoverage.Result`. Called with **no** manual ids, so classification stays pure and the Include/Leave-manual choice is a separate draft (otherwise "Leave manual" would turn Covered into Manual). Two call sites: the review read and the once-per-newly-inspected-tree spot-check read. No `Classify`, masks or coverage-world code in MFM-08 files (asserted).
- Candidates: `ScenarioOneCropTreeCandidates.Suggest`, `TryGetInspectionTarget`, `TryMoveInspectionTarget`.
- Prescription: `CreatePrescription`, `PreviewPrescription`, `ApprovePrescription` (each called once). `CheckPrescriptionLink` is not needed and not used.

## Implemented contract
- **Source sample vs targets:** sample row "Sample — demonstrated treatment", never a target; map pattern is a heavy outline with crossed diagonal hatch.
- **Four states, readable reasons:** `COVERED`/`NOT COVERED`/`EXCLUDED`/`MANUAL` with plain-language text for every closed `Reason`; `NaturalRegeneration`/`PlayerCropTrees` are informational notes, not exclusions. Remainders listed as Manual.
- **Covered = comparable, not committed:** meaning line "Comparable — available to include" plus a note. Classification and the draft choice ("Apply sample treatment here" / "Leave manual") are separate; only Covered targets are offered the choice (the model also refuses it for others).
- **Wording:** "Apply your sample treatment to comparable areas." "operator selection" is not used.
- **Candidates:** card with "CANDIDATE — inspection prompt" and "Spacing identifies where to inspect. Judge the tree in 3D before confirming it as a Crop Tree." Previous/Next (non-wrapping; Previous only from a selected candidate), Inspect in forest (sets the existing waypoint; no teleport). The map has no confirm and there is no bulk action. In 3D the inspection card shows the candidate chip; **[C]** is the normal Crop Tree mark (confirmation), **[N]** rejects/reopens (session-only, no mark). Confirmed state is derived from the live Crop Tree mark.
- **Proposals:** from read-only `PreviewPrescription`; each labelled "PROPOSAL — not a Fell mark and not yet work" with tree, unit, served Crop Tree, reason and share/basal area; conflicts ("CONFLICT — held, not proposed") use the production conflict kinds. Plain data only, never marks.
- **Prepare / approve:** "Prepare prescription for review" calls `CreatePrescription` (snapshot only). "Approve prescription" is enabled only with an active snapshot, a valid preview with proposals, and a 3D spot-check; the gate is re-evaluated in the click handler before `ApprovePrescription`. Success text: "Prescription approved. Pending tasks were added to the Work Plan. They are not approved for execution yet." The Work Plan is never opened or approved from here.
- **3D spot-check:** recorded only in `OnInspected` when the player actually inspects a tree standing in an accepted, still Covered unit of the active prescription. Waypoint, selection and map movement never set it. Not persisted; reset when the active snapshot is replaced.
- **Form, not colour:** areas differ by outline dash/weight and hatch (sample cross-diagonal; covered single diagonal; not covered dashed, no hatch; excluded square grid; manual dotted) and carry a printed tag. Candidate = hollow **square**, proposal = hollow **triangle** (conflict adds a bar); existing marks are rings on the map and paint bands in the forest. Violet/pink are bounded presentation choices.
- **Fail closed:** an invalid `ReadCoverage` shows the production problem and builds no action.

## Offline evidence (all run on this candidate)
- `python3 -B Tools/Verification/MfmTreatmentReview/run_treatment_review_offline.py` → `MFM08_BUILD_PASS` (14 sources, warnings as errors), `MFM08_TREATMENT_REVIEW_PASS checks=106 failed=0`, `MFM08_REPEAT_PASS` (two processes, byte-identical), `MFM08_BOUNDARY_PASS` (only owned files changed since the start).
- Regression, unchanged: MFM-04 core `240/0`, MFM-05 candidates `95/0`, MFM-07 approval boundary `125/0`, each with repeat PASS.
- Type-check of the Unity-facing files: `Tools/Verification/CloudTypeCheck/run_typecheck.py` (2021.3 approximation) with a throwaway `Painter2D` stub outside the repo, bodies confirmed to bind by a negative control. No errors in `StandMapTreatmentReview`, `ScenarioOneTreatmentReview`, `StandMapView`, `TreeInspectionView` or the plan. The one new entry vs baseline is the same `MeshGenerationContext.painter2D` 2021.3 gap `StandMapCanvas` and others already have (Painter2D exists in Unity 6). **This is not Unity's compiler.**
- Evidence logs: `/media/jer/Files/CCF/Verification/MFM08-TreatmentReview-final/`, `MFM08-regress-*`.

## Decisions to review
1. **Footprints and candidate stems are drawn from public pure data, not a coverage adapter.** Area polygons come from `ScenarioOneRackSampleGeometry.Derive(ConfirmedRacks, half)` (the same pure function production uses; matched to coverage areas by `UnitIndex`+`RackId`, otherwise not drawn). The candidate batch needs a stem list, so the card reads living `ForestTree`s (position, species, Crop/Fell mark). Coverage classification still comes only from `ReadCoverage`. If the Manager prefers a thin manager accessor for these, that is a separate API request.
2. **[N]** for reject/reopen: free of existing bindings; it is in the contextual hint only, not a global shortcut.
3. The tab is shown only when `RackPlanningV2Active`.

## Unity-readiness checklist (for the separately scheduled gate)
1. Import/compile clean in Unity 6000.6.0f1 (new files have `.meta`; Painter2D/`ScrollViewMode` usage untested).
2. Render Treatment review at 1280×720 and 1920×1080: sample + four states + tags + legend text; the card scrolls rather than overflowing (height capped at `MapSidePx − 40`); confirm area tags and hatching are legible and do not hide the map squares.
3. Covered vs Include/Manual is distinct on screen.
4. Candidate Previous/Next/Inspect; walk; confirm one with [C]; reject another with [N]; no bulk action; reopen works.
5. Candidate/proposal glyphs distinguishable from Fell/Crop/Rack marks by shape/text (rings, bands, rack lines).
6. Prepare prescription creates no order or mark; preview shows proposals; inspect one in 3D (chip + line, no Fell mark).
7. Waypoint alone leaves "3D spot-check not done"; actually inspecting a tree in an accepted unit completes it; Approve stays disabled before.
8. Approve creates ordinary Pending work with provenance and the Work Plan stays unapproved; double-approve blocked.
9. Conflict/held presentation via fixture (manual Fell mark, open work, unit no longer covered).
10. Rerun Stand Map/Rack V2, MFM-04/05/06/07 and UI regressions; Reference Future v1 hashes unchanged; no Windows build.
Watch for: `maxHeight`-bounded `ScrollView` inside the `map-side` card; keyboard focus (buttons are `focusable=false` via `UiKit`); the review layer sitting under the square buttons.

## Next action
#91 schedules the Unity Tier-2 gate on the exact SHA posted on #156 (fresh lock/process/free-space/priority preflight). Do not integrate before then.
