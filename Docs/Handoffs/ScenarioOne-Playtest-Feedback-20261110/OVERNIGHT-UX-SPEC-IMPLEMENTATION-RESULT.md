# #363 — Scenario One player feedback implementation result

**COMPLETE source implementation and conditional handoff; NOT Unity-qualified, integrated or accepted.** 2026-11-10. Manager decision requested: **APPROVED / NEEDS CHANGES / REJECTED**. This replaces the partial checkpoint, which remains in Git history. No sub-agents were used.

Worker: OpenAI **gpt-6.1-sol / medium**, Codex harness in T3 Code. One owned source branch `feature/scenario-one-player-feedback-overnight-20261110-v1`, reused clean registered worktree `/media/jer/Files/CCF/Operations/CCF-FocusedSourceStage-20261109`. Resume HEAD `58f694ab4e4308542c17cca5c0eda8fadda23e56`; original chain starts at `8c49460d0a327a21e57b2dfc661d3e5779dd13d1`. Its only difference from tested game/verifier `fb599eecde983552dec4f06fb9efcf8e38397aed` is the prior 91-line report. No stale-main game source used.

**Exact source/tested commit: `bfd8e2a49c19c09ca98057ae4b7bb7c4d23b5e49`.** All source and focused checks below apply to these source bytes. This result is published in a later **report-only commit**, discoverable by `git log -1 --format=%H -- Docs/Handoffs/ScenarioOne-Playtest-Feedback-20261110/OVERNIGHT-UX-SPEC-IMPLEMENTATION-RESULT.md`; that commit is not a new source qualification. Exact source file SHA-256 receipt is in the evidence archive.

Authority: [approved four-file spec](https://github.com/jermur788/CCF/tree/main/specs/scenario-one-player-feedback-overnight-v1), #361 feedback, original Fable #362 report and Manager addendum. No PNG was accessed or claimed as rendered evidence. The approved successor explicitly amends S1-C FR4 source-chip visibility and D-072 minimap presentation only. Underlying history provenance, sample scoring and save policies remain unchanged.

## Delivered behavior and requirement status

**IMPLEMENTED** means task-branch source exists. **OFFLINE VERIFIED** means the stated focused assertions/typed compile ran. **RENDERED VERIFIED: none. Unity/native-input/save-reload qualification: NOT RUN for every row.** Source inspection and synthetic draw-plan pixels are not Unity evidence or user acceptance.

| FR | Source status | Evidence / qualification limit |
|---|---|---|
| Z1 | IMPLEMENTED / OFFLINE VERIFIED | TeachingBay reads the existing cached Derive/third confirmed rack analysis; rotated frame and valid/invalid plan checks. No new scoring geometry. |
| Z2 | IMPLEMENTED / OFFLINE VERIFIED | Full Access/Marks dashed footprint, two separators, Near/headland, Middle, Far labels; descriptive legend and user-invoked existing cell-waypoint API. Actual layout/native waypoint use NOT RUN. |
| Z3 | IMPLEMENTED | Walking HUD at existing 4Hz cadence: inside/outside, current third, left/right looking into rack, checkboxes, n/3, sides and spacing. Corridor is explicitly no-Crop-credit. HUD overflow/render NOT RUN. |
| Z4 | IMPLEMENTED / OFFLINE VERIFIED | Specific outside, duplicate-third, extra fourth, absent side, too-close ID and aimed offending pair/distance feedback, Fell-not-Crop. Player marks remain applied; scoring code untouched. Focused negative selection checks pass. |
| Z5 | IMPLEMENTED / OFFLINE VERIFIED | V2 Objective2 figure uses the same actual frame, three thirds, illustrative three trees on both sides and explicit 8m text. Legacy picture retained. Live orientation/readability NOT RUN. |
| N1 | IMPLEMENTED | Explicit Review-the-year transition and HUD title at existing acknowledgement boundaries; later guidance prioritises review. No stage-index, completion predicate or acknowledgement write change. Real progression NOT RUN. |
| N2 | IMPLEMENTED / OFFLINE VERIFIED | One navigation allowlist; gentle 1.2s border tint, persistent Next cue, Reduce motion default true. Commitment/modal/Help targets excluded from animation; unsafe-approval mutant killed. Disabled/hidden/help/paused/preview guards inspected, actual UI behavior NOT RUN. |
| N3 | IMPLEMENTED | Walking objective container uses the same single animation owner; no fake clickable action or focus movement. Physical key instructions remain visible. Runtime animation NOT RUN. |
| N4 | IMPLEMENTED | Extension checklist separates map, reading, covered/manual choice, individual inspection, proposals, prescription gate and Work Plan gate. Only an actually stored Accepted prescription earns its tick; unobservable judgement steps say player action. Actual Treatment flow NOT RUN. |
| N5 | IMPLEMENTED | Review Walk primary only when acknowledgement absent; latest pending ACK primary, historical ACK hidden/callback guarded. Visible Objective5 session-only warning. Native focus/Enter NOT RUN. |
| R1 | IMPLEMENTED | Show data sources toggle; provenance legend and chips default hidden. All chips/metadata retained. UI toggle NOT RUN. |
| R2 | IMPLEMENTED / OFFLINE VERIFIED | Plain qualifiers remain independently visible: calculated, estimated/reconstructed, session-only, not recorded, now. Qualifier assertions pass. Toggle changes only view state/classes, not historical data. Live year browsing NOT RUN. |
| R3 | IMPLEMENTED | Existing report content retained, columns ordered Work done / Forest changes / Cash, existing detail/history maintained. Extensive new report redesign omitted under the spec's bounded-layout option. Render NOT RUN. |
| C1 | IMPLEMENTED | Audit found preapproval BuildHarvest already reads authoritative current quote, base harvesting cost and minimum adjustment. Added explicit total quoted contractor charge and all-jobs commitment scope. No hardcoded minimum or economic calculation change. Quote/cash runtime NOT RUN. |
| C2 | IMPLEMENTED | Both UI occurrences of nearest-the-path corrected to first designated removal tree at headland-side entrance. Legacy source outside those V2 branches unchanged. |
| C3 | IMPLEMENTED | Current HUD action/key on card; Concept, Practice and full instructions under More detail; session warning, safeguard and extension checklist remain visible. 720p layout NOT RUN. |
| C4 | IMPLEMENTED | Review-year action, unchanged acknowledgement gates, session warning, clearing rename and primary semantics as above. |
| M1 | IMPLEMENTED / OFFLINE VERIFIED | Source-backed feeder distinct cased band and dashed centre above path. Endpoint projection and content-bearing offline pixel checks at both sizes pass, removed-feeder mutant fails. Actual scene/cache/crop and 3D rendered parity NOT RUN. |
| M2 | IMPLEMENTED / OFFLINE VERIFIED | Already on the ground / Your plan / Done groups; tan forest track, dotted old-path section, pale headland, dashed-centre link with distinct swatches. Existing clearing, no player-facing V2 legend. Old path splitting is paint-only at real feeder junction. |
| M3 | IMPLEMENTED / OFFLINE VERIFIED | Source read-only headland/link on minimap, stage-gated sample outline; hollow suggestion/entrance targets and filled actual marks, separate harvested state. Hollow/filled state tests pass. Minimap native layout NOT RUN. |
| M4 | IMPLEMENTED / OFFLINE VERIFIED | No geometry, planks, masks, scene, MFM, saves or Reference changes. Protected hashes agree outside allowlisted UI. Optional 3D trail restyle omitted: no admitted Unity proof of visual-only safety. |
| D1 | IMPLEMENTED | Full-width Objective1 figure with taller scrollable content, source schematic tree bands and existing access. Actual 720p/1080p legibility NOT RUN. |
| D2 | IMPLEMENTED | Suggested rack locations title; direct Headland/Link track/Forest track/Old path/Existing clearing labels, tan track and inherited-infrastructure sentence. Label overlap/render NOT RUN. |
| D3 | IMPLEMENTED / OFFLINE VERIFIED | Existing source-fixture five unequal positions retained; all five Draft/dashed, hollow entrances, original six retained bands, actual ~15.6–19.4m range and approximate 16–20m guidance. Five-state/source identity checks pass. |
| D4 | IMPLEMENTED / OFFLINE VERIFIED | Grouped key and four-stage Suggested → Planned → Entrance marked → Harvested strip, north/20m, deliberate execution wording and old-world guard. Render NOT RUN. |
| D5 | IMPLEMENTED | Figure explicitly illustrative, not saved Stand Map or Unity evidence. No black historical capture reused. Current Unity captures NOT RUN. |

## Feeder investigation and numeric evidence

Read `StandMapSource.ReadExistingHeadland`, `ReadScenery`, `StandMapDrawPlan`, symbol table/legend and physical `ScenarioOneHeadlandPresentation.CreateSurface`.

- The baseline source already supplies headland and feeder from the same geometry constants as the physical mesh; both are painted on Full, only excluded from Mini. It is incorrect to claim the baseline model lacks the feeder.
- Headland: **(-35.4, -37.7) → (-35.4, 37.7)**, length **75.4m**. Feeder: **(-35.4, -27.4) → (-2.436166, -27.4)**, length **32.963834m**. Both **2.6m** wide; two junction pads radius **2.2m**. Geometry source files are byte-identical to baseline.
- Baseline: 4 segment strokes (two casings/two fills) plus pad paints, all with Path/PathCasing; later scene Dirt Path strokes use the identical style and can overpaint the track junction. Legend also repeats the same Path swatch. This establishes **same-style visual indistinctness and junction overpaint risk**, not observed runtime absence.
- New plan: original path first, then two access casing strokes, two pale fills, two casing/two fill pads and one dashed link centre. Separate source-backed style passes do not create new route geometry. Old path receives dotted paint only on its actual section north of the feeder junction.
- At map sides **480/800px** (offline fixtures labelled for 720/1080 screens), true 2.6m width projects to **15.6/26px**; feeder centreline length **197.783/329.638px**. Focused inverse-transform endpoint error <0.0001m. Mini uses the identical model/transform.
- Offline raster witness (Pillow painter of the actual C# draw plan, synthetic fixture, NOT a Unity screenshot): feeder/background mean luminance difference **188.32 / 96.89**; removed-feeder negative control **0.00 / 0.00**. Both exceed the 80 threshold; grayscale artifacts included. A first darker link styling measured 71.98 in one witness; the source palette was improved before the final passing test. No test threshold was reduced.
- Eligibility/cache remains the existing strict V2+Enlarged80+active presentation gate. A capped delayed component lookup is a residual lifecycle hypothesis if a future live map is empty. No actual scene/cache or crop/zoom defect is proven here; verify under a legitimate Unity lease before accepting visuals. The source-only fix does not claim to resolve every possible runtime absence cause.

## Verification and evidence

Raw archive: **`/media/jer/Files/CCF/Verification/ScenarioOnePlayerFeedbackOvernight-20261110/`**.

| Check | Result | Receipt |
|---|---|---|
| Full runtime typed C# compile against existing Player Unity assemblies | OFFLINE VERIFIED, **189 sources / 116 references / exit 0 / 0 errors** | `compile-phase1.txt` through `compile-phase4.txt` |
| Focused positive/negative assertions | OFFLINE VERIFIED, **67 checks** | `checks-phase4.txt`, repository `Tools/Verification/PlayerFeedbackOvernight/Checks.cs` |
| Typed mutation tests | OFFLINE VERIFIED, **3/3 killed** | `mutations.txt`; unsafe approval pulse, absent teaching overlay, absent feeder style |
| Content-bearing pixel witness plus removed-feeder negative | OFFLINE VERIFIED, both sizes | `pixels.txt`, `plan-480.json`, `plan-800.json`, `offline-feeder-1280x720*.png`, `offline-feeder-1920x1080*.png` |
| Current Unity import/render/native interaction/save reload | **NOT RUN** | Admission failures below; no new capture claimed |
| Independent review | **NOT RUN** | Same worker performed checks, no independent reviewer |

Reproduce from repository root (all outputs disposable, no saves):

```sh
python3 Tools/Verification/ScenarioOneNonUi/compile_runtime_sources.py --src "$PWD" --out /tmp/ccf363-compile-phase4
python3 Tools/Verification/PlayerFeedbackOvernight/run_checks.py /tmp/ccf363-compile-phase4/bin/Release/netstandard2.1/check.dll /tmp/ccf363-checks
python3 Tools/Verification/PlayerFeedbackOvernight/pixel_check.py
python3 Tools/Verification/PlayerFeedbackOvernight/mutations.py
```

The existing compiler helper needs absolute `--src`; the initial relative-path invocation failed to locate files and was corrected. This offline compile does not run Unity source generators/analyzers or prove Unity behavior. Tests use small plain-data fixtures, not a real progressed saved world. No existing tests were weakened, deleted, skipped or rebaselined. No broad historic campaign was rerun.

Historic results remain unchanged: **strict Workbench runner exit 1 FAIL** despite original Editor 107/107; **StandMapParity FAIL ×2**; **UxPedagogy 38 FAIL**; **#360 171 PASS / 2 FAIL**; **save03b historic FAIL**. The #360 **26 black captures** remain invalid rendered evidence. Existing broad checker expectations may flag intentional presentation changes; their historical outcomes remain preserved and have not been relabelled green.

## Protection, ownership and Unity admission

Frozen baseline has **17,350 SHA-256 entries**: all tracked files in the source, warm U3 and original registered Workbench; all **27 ignored U3 Assets**; Unity save/preferences directories present on this machine. `protected-before.json` SHA-256 **4cc126cde6a4cf0e726527c776d5cd1b85bd1899317852b61e4235a2e339234a**. `protected-comparison.json` SHA-256 **8f81ccab68add588825247cbd16ac2d9daa940f16ba2959e531cfd2f0f226324**.

Final comparison: **17,330 identical, 20 changed, zero missing, zero changes outside source UI**. The 20 differences are exactly the allowlisted production files below. U3 ignored pack **27/27 unchanged**, original Workbench/dirty bytes, frozen Reference archive/meta, all scenes/planks, tree masks, packages and MFM files in those tracked trees are unchanged. Save/preferences files in the captured directories are unchanged. The manifest is a byte-protection inventory, not proof that every possible external save location is isolated; no save was loaded or written in this run.

#361 was refreshed before final qualification: feedback received, no explicit human release and no proven save-profile isolation. `command -v unity-lock` finds no executable; no real exclusive owner lock was acquired. No Editor was identified in the process checks; Hub/licensing client alone does not constitute an Editor or admission. Therefore **Unity NOT RUN**. Licence qualification, cold import, monitor, save-reload and rendered checks were not attempted. No foreign process/lock was killed or released.

U3 remains at **fb599eec** with its original settings plus seven material dirty paths, unmodified. No stash/reset/clean/new checkout/new Library. Final sampled free disk: Files **8,375,304,192 bytes (~7.80GiB)**; ZX20 **38,575,669,248 bytes (~35.93GiB)**. No 20s disk monitor was started because Unity was never admitted. No owned Unity lease remains to release. Source worktree was clean after source push; subsequent report is the only final edit. Manager-owned `active-tasks.md` untouched. Dashboard synchronization **NOT RUN**, left to Manager.

## Serial commits and changed files

- `b7ced0e3`: Phase1 teaching area, waypoint, HUD, orientation and warning.
- `ec4abdbd`: Phase2 safe guidance, motion preference and checklist/disclosure.
- `f5f21748`: Phase3 Annual Review sources/primary and quote transparency.
- `cbf218e7`: Phase4 feeder/map/legend/schematic and verification.
- `bfd8e2a4`: focused verification corrections for entrance swatch parity and corridor feedback; exact tested source.

Production changes, all under `Assets/ForestPrototype/UI/`:

```text
AnnualReviewView.cs
LearningObjectivesView.Curriculum.cs
LearningObjectivesView.Sequence.cs
LearningObjectivesView.cs
Map/StandMapCanvas.cs
Map/StandMapDrawPlan.cs
Map/StandMapLegend.cs
Map/StandMapLegendView.cs
Map/StandMapModel.cs
Map/StandMapSource.cs
Map/StandMapSymbols.cs
MenuHelpView.Text.cs
Resources/ScenarioOneUi.uss
ScenarioOneRackBayText.cs
ScenarioOneTeachingSequence.cs
ScenarioOneUiRoot.cs
StandMapView.cs
TeachingDiagram.cs
TeachingDiagramPlan.cs
WorkPlanView.cs
```

New verification files: `Tools/Verification/PlayerFeedbackOvernight/{SOURCE-ALLOWLIST.md,Checks.cs,run_checks.py,pixel_check.py,mutations.py}`. Report: this file. No serialized Unity assets or new asset GUIDs. No main merge, PR, Windows/Player build, automatic Fell/approval, new road, contractor price change or save-schema change.

## Excluded items and research-only recommendations

- **Access topology / west gateway C1 / construction:** retain the inherited access for existing saves. Commission a separate topology spec with explicit old-world discrimination, tree-ID/mask and MFM impact studies before any physical change. No new gate, northern spur, trail deletion or road cost is implemented.
- **Optional 3D old-trail restyle:** deliberately unchanged because a genuine graphical/physical safety check was unavailable. Map-only section styling changes no planks or exclusions. A later visual-only proposal should verify collision, masks and actual rendered width before approval.
- **Combined rack + initial release visit:** current first access and later sample-release decisions have different teaching prerequisites; existing quote groups eligible harvest/salvage work per visit. Evaluate a separately specified combined pedagogy and actual current quotes before promising savings. This run adds fee transparency, not a new work sequence or price. Real-world operational recommendations/cost research NOT RUN.
- **Objective5 partial persistence:** retain current session-only inspections with the visible warning. A future persistence spec needs migration, stale tree/dead-tree identity, valid review evidence and latch tests. No new schema or saved per-tree input implemented.

Manager should review source and arrange one guarded warm Tier2 graphical qualification after genuine #361 release and complete lock/licence/save/disk admission. Specifically inspect 720p HUD overflow, figure label wrapping, Annual Review qualifier density and primary focus, pulse suppression, live feeder visibility/cache and 3D endpoint parity, and disposable save/reload/legacy worlds. **Independent review NOT RUN.** No self-acceptance or external-demo GO is asserted.
