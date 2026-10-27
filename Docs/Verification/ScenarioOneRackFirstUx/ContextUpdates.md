# Proposed canonical-document updates (corrected; not applied)

Replaces the earlier draft of this file, whose pins (`cd0692a`, "96 rack checks", "not integrated") were stale. Written by the integrator for the Overall Manager after integration and verification. **The canonical documents are not edited by this task, and no context snapshot is published.** The Manager applies the text below through the normal canonical-doc commit and context-refresh process. The decision number is a placeholder: D-063 is the next free number at context commit `36cd791b329189e0ae38931d06ae51af1a1173ce`; the Manager confirms it at publication.

## 0. Pins and facts (all values below are derived from these)

| Item | Value |
|---|---|
| Baseline (tooling-only successor candidate) | `8942b5c3c2adb66164573f0c12a92b31adfcb0cf` |
| Feature branch / reviewed head | `feature/scenario-one-rack-first-ux-v1` @ `a2021b5ad970bdd3084ef8ba56f1318fbe21fd12` (independent review 2026-10-18: APPROVE WITH NON-BLOCKING NOTES) |
| Player-tested code head (implementer) | `2d9e3fb24eee1fcc87cce3a8be58138d36f91ccf` (`a2021b5` adds only `Docs/Verification` markdown) |
| Integrated squash | `f4d108afd567890b5ae3bc3a6ae7db7242c854ef` on `integration/scenario-one-tier3-candidate`; tree `70fac330ba241a984f9f0747fa56a2a260736c09`, identical to the `a2021b5` tree (and to each of `Assets/`, `Tools/`, `Packages/`, `ProjectSettings/`) |
| Wording-only commit (final tested **code** head) | `aad4e6f1f49f1d30d0b4eb38e80c8a6a12f903eb` |
| This file | in a docs-only commit after `aad4e6f`; production tree unchanged by it |
| Context commit at task start | `36cd791b329189e0ae38931d06ae51af1a1173ce`. The code line (`8942b5c`) is not an ancestor of it; the two lines meet at `c1564b8` |
| Publication state | **Local only.** Neither the feature branch nor the integration commits are pushed (origin `integration/scenario-one-tier3-candidate` is still `8942b5c`). Incremental bundle of both branches: `/media/jer/ZX20/CCF-Verification/ScenarioOneRackFirstUxIntegration-20261018T075433Z/rack-first-integration-incremental.bundle` (requires `8942b5c`) |
| Integration evidence root | `/media/jer/ZX20/CCF-Verification/ScenarioOneRackFirstUxIntegration-20261018T075433Z/` |
| Implementation evidence root | `/media/jer/ZX20/CCF-Verification/ScenarioOneRackFirstUx-20261017/` |

## 1. `Docs/Project/decision-log.md`

Add one row to the decision table:

| D-063 | 2026-10-18 | Scenario One teaches extraction racks before selective marking | Confirmed (Overall Manager integration packet); implemented, integrated locally and verified; not pushed | Scenario One's opening lesson is the creation of **extraction racks**: permanent straight corridors of felled trees across the planted rows, so machinery can reach the stand now and in future thinnings. Individual selective marking (inspection, Crop Trees, X/C) is introduced afterwards, once the first intervention has been resolved. Racks are planned by a **hybrid of an in-world tool (B) and a Stand Map "Plan rack" tool**, both calling one backend (`RackPlanner` / `RackGeometry`: corridor width default 2.4 m, trees chosen from tree positions, Crop Trees and already-planned trees skipped, deterministic order) and one confirm path (`ScenarioOneManager.ConfirmRack`), so a rack means the same wherever it was planned. A rack is a set of ordinary Pending `FellTree` orders in the draft Work Plan; approval, harvest, cost and timber value are the existing ones. Scenario One racks run boundary to boundary of the stand; the geometry also accepts explicit start and end points, which Scenario One does not use. The authored Scenario One plan is five racks across the 41 planted rows with seven rows left standing between racks; this is the scenario's own layout and carries no general forestry-spacing claim (see the wording commit in section 3). **No save-schema change (v20), no ecology, regeneration, browsing, density, calibration, price or economy change; Reference Future untouched.** The teaching gate is soft (prompts, cue, status line, lesson order): X and C still work from the first frame. Rack grouping (the Work Plan group cards and violet markers) is session state; after a load the rack trees are ordinary draft felling jobs and none is hidden. Source: `a2021b5` (reviewed), integrated as `f4d108a`, final tested code head `aad4e6f`. |

The same revision also repairs the playtest UX items (Help and first-use windows as modals that own the keyboard and pointer, tree inspection card and world indicator, plain-language terminology, Stand Map modal, single "Approve Work Plan" action, HUD buttons responding to a real click); these need no separate decision. Recommended companion record (Manager's choice): the first-year consequence of the authored plan below.

## 2. `Docs/Project/current-milestone.md`

Under "In scope", add:

- Scenario One playtest UX revision and rack-first tutorial: integrated locally as `f4d108a` (tree identical to reviewed `a2021b5`) with a wording-only commit `aad4e6f`; verified in the Editor and, for `f4d108a`, in a Linux Player (compile, build, save/load smoke, pointer smoke, modal keyboard smoke, rack-first Year 1). **Not pushed. Native Windows UNVERIFIED.**

Preserve these open items; none is closed by this work:

- native Windows smoke;
- native Windows owner retest of the previously reported menu/tab symptoms ("More detail does nothing", Stand Map top-tab hitbox offset; neither was reproduced on Linux);
- verification-debt cleanup;
- distribution asset-pack entitlement/licensing;
- final release review / G4.

Do not change the external-demo "NOT READY" wording on account of this work.

## 3. `Docs/Project/unity-project-overview.md`

Add a section after "Integrated malformed cell-index save hardening":

> ## Integrated Scenario One rack-first tutorial and playtest UX revision (2026-10-18, D-063)
>
> - **Source and identity.** Reviewed `a2021b5` (independent review APPROVE WITH NON-BLOCKING NOTES) was squash-integrated onto `8942b5c` as `f4d108a` on `integration/scenario-one-tier3-candidate`; its tree is identical to `a2021b5`. One wording-only commit, `aad4e6f`, follows it. Local only; not pushed.
> - **Racks.** `Racks/RackGeometry.cs` (pure geometry: boundary clipping to the stand, explicit start/end support, default corridor width 2.4 m, deterministic selection ordered along the line then by tree id, Crop Trees and already-planned trees skipped and counted), `Racks/RackPlanner.cs` (row detection from tree positions, `Propose`/`ProposeBetween`, the harvest quote through `ScenarioOneManager.QuoteTrees`, which calls the same `ScenarioOneEconomyAdapter.QuoteHarvest` as the Work Plan), `Racks/RackTool.cs` (in-world tool: B, aim at a planted row, mouse wheel turns it in 15° steps, click or Enter adds it, Esc or B cancels with no change), `Racks/PlannedFellMarkers.cs` (violet rack markers, red planned-fell markers) and `ScenarioOneManager.Racks.cs` (`ConfirmRack`, `RemoveRackGroup`, the session-only grouping and the derived `RackFirstStageActive`). The Stand Map has a "Plan rack" mode on the same planner. No rack object is persisted.
> - **Authored layout** (`ScenarioOneRackLayout`, Enlarged80 only): five racks on rows 5, 13, 21, 29 and 37 of 41, seven rows left standing between them; 157 of 1,344 living trees (11.7%), 17.89 m³ stem volume, 15.08 m³ sold, timber income €752.59 against the shared €2,500 contractor-visit minimum: **net −€1,747.41 in year one** (cash €12,000.00 → €10,252.59). A single rack on its own previews at about −€2.3k because it carries the whole visit minimum. This is the cash consequence of the authored plan, not a calibration change.
> - **Teaching order.** Lessons: racks, approval, advance a year, Annual Review; then inspection, Crop Trees and selective felling. The gate is soft. The learning cue follows lesson progress, not the stage, so "Next lesson: Create your first rack" also shows on a Legacy40 stand and on a loaded game that has not done the rack lessons.
> - **Work Plan.** Racks are grouped at the top ("Extraction racks · n racks, m trees", DRAFT/APPROVED, Remove rack / Cancel approval, Show trees); the one approval action is "Approve Work Plan" (the same all-or-nothing approval); "Add marked trees" and the "What are you doing?" card are gone. Marks still convert to Pending orders when the plan opens or on approval (unchanged behaviour; `AddMarkedTreesToWorkPlan` is retained for harnesses).
> - **Modal input rule.** While the help and first-use window is open (`ScenarioOneManager.ModalOpen`, set by `MenuHelpView`) it owns the keyboard: Tab, M and O do nothing; Continue, Esc and F1 close it and the shortcuts work again at once. A screen that opens with its own first-use window needs that window dismissed before its key closes it again.
> - **HUD pointer.** A press on a HUD button with the mouse free stays with the button (`ScenarioOneUiRoot.IsOverButton`), so Map, Objectives and Help respond to a real click; world clicks still retake the mouse.
> - **Wording commit `aad4e6f`** (wording and comments only): the regeneration objective line reads "Young trees present in N areas (at least M needed)" (short name "Areas with young trees"), because the calculation counts the map squares that hold any young tree (`ForestEcologyCell.HasRegeneration`) and does not measure establishment or success; the rack Help and lesson text teach racks as permanent extraction access and describe the authored rack plan, and no longer present rack spacing as a general rule (the "seventh to ninth row, common in Irish thinning" claim had no source in the research index); the fresh-game start footer in the Work Plan gives rack guidance while `RackFirstStageActive` and the previous marking guidance otherwise.
> - **Verification.** Reviewed head: rack harness 111 hard checks, rendered harness 119, UX gate 198 (all with 0 unverified), Menu, U4 guidance, Startup, P2, P3 and Interaction PASS, offline checks `RACK_OFFLINE_PASS checks=23` and `SCENARIO_ONE_UX_PASS checks=188`. Integrated `f4d108a` (Unity 6000.6.0f1, lane U1, detached, Unity lock): compile/import 0 `error CS`; Linux Player `CCF-S1-20261018-L04-f4d108a-SMOKE`; save/load smoke PASS on the second launch (the first launch of a freshly built Player fails the smoke's fixed 3-second startup check; the same happened on the first launches of the `8942b5c` and `f6c5d3a` builds); human-speed pointer smoke (HUD Map, Objectives and Help buttons, every Stand Map tab, a map square) PASS; modal keyboard smoke (Tab/M/O ignored over the first-use and rack windows, shortcuts resume after Continue) PASS; rack-first Year 1 in the Player (33-tree rack, approve, advance, Annual Review, marking line introduced) PASS. Wording commit `aad4e6f`, Editor only (the Player was not rebuilt): compile 0 `error CS`, offline UX check 188, wording readback 24 hard checks (rack stage) and 23 (Legacy40), rack harness 111, rendered harness 119, UX gate 198, Menu gate PASS. **Not run:** Tier-3 (inherited evidence stands: no simulation, economy, save or Reference Future file changed), the Startup, Interaction, P2, P3 and U4 gates after `aad4e6f`, native Windows.
> - **Unchanged.** `ForestSaveData.CurrentVersion` 20; growth, regeneration, browsing, density, Model 2 calibration, objective thresholds, timber prices, harvest economics, the contractor minimum, Reference Future v1, historical hashes, Scenario Two, Asset Store files.
> - **Known residuals (not fixed here).** `ScenarioInterventionHistory.NextStepHint` still says "No intervention yet. Inspect trees, mark them with X, import the marks into the Work Plan…" before the first intervention; `ScenarioHistoryVerification` asserts that text, so changing it needs a gate edit and a decision. A few player-facing strings still say "cell" (`ForestPlayer.cs` planting-mode prompt, `ForestEcologyController` regeneration messages, the storm text in `AnnualReviewView`). The Linux Player smoke's fixed 3-second startup check fails on the first launch of a fresh build.

## 4. Proposed `active-tasks.md` row (Manager edits that file)

| CCF-S1-RACKFIRST-INTEGRATION | DONE (awaiting Manager) | Claude Code session (Sonnet 5.5) for Overall Manager | CCF-Integration (`integration/scenario-one-tier3-candidate` @ `aad4e6f`, unpushed), CCF-U1 (detached `aad4e6f`, clean) | Unity lock released | Squash `f4d108a` (tree == `a2021b5`), wording commit `aad4e6f`; open release items preserved (section 2) |

## 5. Facts recorded for provenance

- Storage was below the 15 GiB minimum when the task began and the Unity lock refused to issue. With Manager approval (asked in-session) two superseded intermediate Player builds of this feature, `CCF-S1-20261018-L01-678f111-SMOKE-Linux-x64` and `L02-cd0692a-SMOKE-Linux-x64` (about 2.4 GB), were deleted; their build logs, the kept `L03-2d9e3fb` build and the `8942b5c` build remain. After the new `L04` build the volume is again below the minimum.
- The Unity lock record names `f4d108a`; U1 was advanced to `aad4e6f` inside the same hold for the wording verification, because a new acquire is refused while storage is below the minimum. The lock was held past its ETA and released with its own lock id after the last gate.
