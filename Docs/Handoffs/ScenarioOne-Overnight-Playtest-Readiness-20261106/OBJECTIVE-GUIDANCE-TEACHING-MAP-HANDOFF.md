# Stage B / T3 (#296) — BLOCKED before diagram implementation

**Date:** 2026-11-06. **Parent:** overnight chain #294.
**Worker:** OpenAI `gpt-6.1-sol`, Medium reasoning, Codex harness in T3 Code; primary implementation sub-agent, no further delegation.
**Verdict:** **BLOCKED — actual Stand Map source omits the reference headland and southern feeder.** An authentic numeric fixture exists; the blocker is the map/reference mismatch, not missing coordinates. No diagram implementation, acceptance, integration or playtest-readiness claim.

## Lane, authority and preservation

- Worktree: `/media/jer/ZX20/CCF-NextActionGuidance-T3-20261106`.
- Branch: `feature/scenario-one-next-action-guidance-v1`.
- Starting HEAD: `8bd7416dd71f5ed175a45ead14dd1114ee636ce9`; fetch succeeded, local HEAD equalled origin, and porcelain including untracked files was empty before work.
- Ending production-source HEAD: **unchanged**, `8bd7416dd71f5ed175a45ead14dd1114ee636ce9`. Ending delivery HEAD is the documentation commit containing this handoff, resolved exactly with `git log -1 --format=%H -- Docs/Handoffs/ScenarioOne-Overnight-Playtest-Readiness-20261106/OBJECTIVE-GUIDANCE-TEACHING-MAP-HANDOFF.md`; the final dispatch reply records its full SHA after push. A document cannot embed its own commit hash.
- Read the Stage B packet from GitHub main (not present in this lane), Manager G4/G5 decision, issue #296, approved spec at `c61bbdd92b94ca07ad806bddfa0bb34123f9e89f` including FR-5/AC-D/AC-V, and project authority/context documents.
- Read the T2 implementation handoff on this branch and both independent T2 source reviews on `origin/docs/overnight-guidance-review-20261106-v1` at `bb5bfc1b8f76b72acfea4a80354bf7b33f569b16`. Review 1 required B1–B3 repairs; review 2 inspected this starting HEAD and returned PASS WITH NON-BLOCKING NOTES, zero blockers. No T2 fixes or expectations were changed here.
- The user's latest direct instruction accepts T2 and releases the shared USS to this single writer. The inspected GitHub coordination row still describes T3 as waiting for T2; the direct release is the authority for this dispatch. Live coordination is Manager-owned and was not edited.
- Unrelated coordination-anchor changes in `/media/jer/Files/CCF-Primary`: `AGENTS.md` and `Docs/Operations/StorageArchitecture.md`. Left untouched. No personal save was read or changed; only archived verification evidence was inspected.

## Mandatory first-step reference — authentic input, not invented coordinates

Read-only input snapshot:

`/media/jer/Files/CCF/Verification/RackV2-Combined-20261027T1334Z/unity-combined-write/config/unity3d/DefaultCompany/CCF/forest-save.json`

- SHA-256: `e8bb96f483e598e7449f370cf56f928503dc79ec7337307758eff23104b10ac5`.
- Save version 21; `standGeometryModel=1` (Enlarged80), `scenarioOne.rackPlanningModel=1` (Rack V2), `simulationSeed=20260914`, `ecologicalYear=0`.
- Snapshot has 1,344 tree records, all stage 0. This is input provenance only, **not a proposed retained-tree census** or promise of current save-schema equivalence.
- Adjacent `unity-combined-write/result.json` records `RACKV2COMBINED_WRITE_PASS`, exit 0, historical candidate `96d9abe2b32a53e744b30c79c2ac7b194350dcfc`. The parent historical combined handoff reports other lesson failures; this task does not relabel that whole historical qualification PASS.
- Cross-check: preserved `RackV2-04A-20261027/unity/SuggestedLayoutVerification.log` prints the five actual production getter routes at two-decimal display precision: Z −35.27, −16.42, 0.24, 15.79, 35.23; each starts at X −35.40, ends at X 40.00, width 2.4. No old Unity process was rerun.

### Real production derivation inspected

`ScenarioOneManager.RackEditing.cs:185` snapshots plantation tree IDs and positions (including stumps) and delegates to `ScenarioOneSuggestedRackLayout.Get`. The helper at `ScenarioOneSuggestedRackLayout.cs:28` rejects wrong model identities, missing/duplicate entrance IDs, non-finite Z and Z outside the headland. For each of its five unique source IDs it uses the **tree's actual Z**, not an average planted-row Z:

`Entrance=(ScenarioOneHeadlandGeometry.HeadlandX, tree.Z); Far=(StandGeometryModel.StandSizeMeters(Enlarged80)*0.5f, tree.Z); WidthM=RackGeometry.DefaultWidthM`.

Current source gives `HeadlandX=-35.4`, Enlarged80 size 80 and width 2.4. The following table is a **source-derived reconstruction of that exact algebra on the authentic saved input**, not a fresh invocation of C#/Unity. Z digits expose saved float provenance, **not survey precision**. X/width constants below use their C# decimal spelling.

| Source entrance identity | Source tree X | Entrance (X, Z) | Far (X, Z) | WidthM |
|---|---:|---|---|---:|
| PO0203 | -33.81470489501953 | (-35.4, -35.271175384521484) | (40, -35.271175384521484) | 2.4 |
| PO1204 | -31.602941513061523 | (-35.4, -16.421764373779297) | (40, -16.421764373779297) | 2.4 |
| PO2003 | -33.05235290527344 | (-35.4, 0.23823529481887817) | (40, 0.23823529481887817) | 2.4 |
| PO2803 | -33.25 | (-35.4, 15.793529510498047) | (40, 15.793529510498047) | 2.4 |
| PO3803 | -33.78293991088867 | (-35.4, 35.23235321044922) | (40, 35.23235321044922) | 2.4 |

Validated by Python standard-library extraction: each identity appears exactly once; entrances are strictly ordered south-to-north within headland endpoints; all five direction vectors are (75.4, 0), hence parallel. Consecutive centreline gaps are approximately **18.849411, 16.660000, 15.555294, 19.438824 m**. In particular, do not replace the third gap with 16 m or spread the racks evenly. The spec's **16–20 m site guidance is not a hard rule or an assertion that every measured gap is in that interval**.

Important rejected shortcut: `Tools/Verification/RackV2SuggestedLayout/OfflineCheck.cs` carries older qualified **row-centre** Z values (−35.403477, −16.015210, 0.014705888, 16.022398, 35.436646). Those are synthetic helper-test inputs, not this live-world snapshot's actual entrance positions. Likewise, the old Enlarged80 CSV rounds tree values to three decimals. Neither was substituted for the authentic Rack V2 save.

### Infrastructure and retention relationship

From `ScenarioOneHeadlandGeometry.cs:7` and `ScenarioOneHeadlandPresentation.cs:37`:

- Existing longitudinal western headland: **(-35.4, -37.7) → (-35.4, 37.7)**, usable width **2.6 m**.
- Existing southern feeder: **(-35.4, -27.4) → (-2.436166, -27.4)**, width **2.6 m**, with authored junction pads of radius 2.2 m.
- The feeder meets the headland **between the first and second entrances**, not on the property's southern boundary Z=-40. It links toward the existing approach; it is not a newly invented southern road.
- No northern connector/spine exists in the inspected headland geometry/presentation. No route or surface has been added here.
- Between rack edges and property limits there are six nonempty Z bands: **[-40, -36.471175385]**, **[-34.071175385, -17.621764374]**, **[-15.221764374, -0.961764705]**, **[1.438235295, 14.593529510]**, **[16.993529510, 34.032353210]**, **[36.432353210, 40]**. A source-only scan found living tree records east of the headland's east edge in each band. This supports schematic retained texture in four interior and two exterior bands, not exact crown positions, counts or row claims.

Inspected `StandMapSymbols.cs:81` onward: suggested/proposal routes are dashed; confirmed planned routes are solid amber/pending; harvested routes are pale; the map entrance is a blue square. Orange field entrance/removal-tree meaning must not become a cleared-route indication. Future teaching annotations must be labelled as such and must not redefine map symbols.

## STOP evidence — materially different actual Stand Map source

1. `Assets/ForestPrototype/UI/Map/StandMapSource.cs:45` defines `PathPlankPrefix = "Dirt Path"`.
2. Its only path-population site is `ReadScenery`, at `StandMapSource.cs:342`: clear `model.Paths`, collect scene transforms whose names start with that prefix, group those planks into `MapPath` records. No `ScenarioOneHeadlandGeometry` or headland/feeder insertion exists there.
3. `Assets/ForestPrototype/ScenarioOne/ScenarioOneHeadlandPresentation.cs:48` instead creates **one mesh object named `Scenario One V2 Headland`** containing the headland, feeder and pads. It does not create additional `Dirt Path` transforms. Its name does not match the map's prefix.
4. `Assets/ForestPrototype/UI/Map/StandMapDrawPlan.cs:116` draws path bands from `model.Paths`. No separate headland/feeder rendering exists in this draw plan. Suggested racks are supplied separately by `StandMapSource.cs:275`; showing suggested rack lines does not supply their missing connected track.

**Conclusion (static source inference):** the prescribed real 3D headland/feeder topology is absent from the actual Stand Map path model/drawing. A source-faithful teaching picture containing those connections would therefore materially differ from the current Stand Map. This is not a rendered observation or proof of an unseen current screenshot. It is enough to trigger the dispatch's explicit **“actual map differs materially: STOP”** condition before diagram code.

**Needed from Manager:** resolve this source mismatch explicitly. Either authorise a separate, narrowly owned map-presentation repair and repin/review its candidate, or explicitly decide that T3 may illustrate the existing 3D infrastructure despite the current Stand Map omission, with truthful explanatory text and an amended comparison gate. This worker does not select that product/scope exception. No world geometry, map model, minimap, save, progression, MFM or T2 change was attempted.

## Changed paths and checks

Only changed repository path: this handoff. No `.meta`, temporary Assets harness, new package, generated-project output, diagram code, stylesheet or saved fixture was added. Because the task stopped before diagram code, **in-code reference data/origin comment is NOT IMPLEMENTED**; the provenance table above preserves the validated input for an authorised continuation.

| Check | Result / scope |
|---|---|
| Fetch + exact branch/HEAD/origin + initial porcelain | PASS; clean and exact pinned starting HEAD |
| Authentic fixture identity/hash and five unique source entrances | PASS; read-only archived save |
| Python standard-library source/reference audit | **33 assertions PASS**; snapshot hash/identity, production algebra/constants, exact saved Z values, six occupied bands, feeder ordering, map omission and docs-only inventory; run as an inline read-only script, no persistent verifier added |
| Source-derived endpoint/width/order/parallel/headland relationship | PASS; algebra reconstruction, not fresh C# execution |
| Four interior + two exterior occupied retained bands | PASS; existence check only, no retained census |
| Existing feeder/no invented northern-spine source audit | PASS for production world infrastructure |
| Required correspondence with actual Stand Map | **FAIL / BLOCKED**; headland and feeder absent from map path source |
| T2 preservation; owned-path/forbidden-change inventory | PASS; documentation-only delta, no T2 or protected code changes |
| `git diff --check` | PASS |
| New diagram, label/legend and in-code reference implementation | NOT RUN / NOT IMPLEMENTED; stopped before editing |
| Unity/Editor/Player/build/compiler/runtime/save/load | **NOT RUN**; explicitly forbidden in this source-only task |

No Unity, Editor, Player, build, native input, screenshot generation or new rendered review was performed. No save writes, GitHub comments, specification status changes, merge or integration occurred.

## Future Stage D/T5 acceptance — all NOT RUN

After a Manager disposition and completed/reviewed T3 implementation, test **each row separately** at **1280×720 and 1920×1080**, in **colour and grayscale**:

| Gate | Required observation | Status |
|---|---|---|
| Topology | Five parallel planned corridors, five headland entrance squares, real longitudinal headland and southern feeder; no north/diagonal/spine edges; real-map/source overlay and count sheet | NOT RUN |
| Relative geometry | Uniform transform documented; direction ≤1°, displacement ≤1% stand width, width deviation ≤5% reference scaled width; no survey-accuracy claim | NOT RUN |
| Retention | Crown/row texture in four interior and two outer bands; no false clearance or census; planned overlay retains standing-tree meaning | NOT RUN |
| State meanings | Proposed/dashed, confirmed/outlined pending, executed/pale plus stump only in separate key; orange marking not clearance; colour-independent recognition | NOT RUN |
| Labels/scale | Illustrative/not saved-map/not cleared caption; headland, southern entrance, five racks, retained stand; site-specific 16–20 m guidance, orientation and scale caveat | NOT RUN |
| Typography/contrast | New words ≥20 UI units and ≥16 physical px at measured panel scale, equivalent-role non-regression, distinguishable pattern gaps/strokes | NOT RUN |
| Reflow/clipping | All caption/legend content scrolls into view without overlap, clipping or obscuring CTA/reason at both resolutions | NOT RUN |
| Recognition | Two independent reviewers, one opposite provider family, each 4/4 on headland/five parallel racks/retained stand/not executed in each size/mode; separate user clarity review | NOT RUN |
| Native input/T2 regression | Existing O/M/Tab/Esc/F1, prompt safe-first/stale Enter/Space safety, cue badges/reasons and later-objective scope; no automatic work | NOT RUN |

Exact future procedure: once authorised, open the fresh Enlarged80 Rack V2 Objective 1 lesson and Stand Map on the same candidate; capture both supported sizes and grayscale variants; compare to the reference table/topology and measure text at actual panel scale. Traverse/scroll the entire diagram and return to map/planning/marking/Work Plan with native controls. Observe five pending routes, standing retained bands, safe cues and no automatic marking/approval/execution. These are instructions and expected observations, **not performed tests**.

## Review disposition

Independent review of the **blocker/reference report** can start. Independent **full-Stage-B completion review cannot start as a completed T2+T3 candidate**: T2 is preserved/accepted per dispatch, but T3 has no implementation. Manager disposition of the map mismatch is required first; no self-approval or waiver is implied.
