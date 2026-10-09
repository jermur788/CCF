# S1-A Teaching Readiness — Manager handoff

Date: 2026-10-09. Primary implementation: Codex. Branch: `task/s1-a-teaching-readiness`. Dedicated worktree: `/home/jer/CCF-s1a`. Authoritative base and context: `2c0f7cdbde7bb2ce982fda7a6963be48e01278aa`.

Implementation commit: 3ca6b17d1ef2b8116783a4a86e8c92d7432fb650. Final handoff commit is reported by the worker with the final clean status. No push or integration has been performed.

## Outcome and scope

The first-cycle Help/Learning/Work Plan/Review now teach repeated selective continuous-cover management: begin with trees worth keeping, inspect their competitors, do not treat suppression as a removal instruction, consider the residual stand, and return to inspect and reassess. Existing P2 competitor presentation and P3 residual-stand summary are retained. No new competition graphics or permanent HUD panel were added.

Natural regeneration is described as an opportunity requiring seed and suitable conditions, never a guaranteed consequence of thinning. Ground and map diagnosis use the authoritative current cell seed-rain state to distinguish an empty site with no incoming seed. Natural regeneration is not presented as necessary for every useful management outcome.

The accepted Model2 clearance explanation was already correct and is retained verbatim: “Dense bramble or bracken can reduce the survival of small young trees. Clearing can reduce that competition, but it also removes young trees already growing inside the treatment area, and the vegetation can return.” The existing legacy explanation remains conditional on the accepted model. No compulsory clearance or repeat interval was added.

Pruning is described as a retained-tree timber-quality treatment; Scenario One records it without applying a timber-price premium.

Learning (O) exposes all eight current scenario success objectives in the existing modal, independently of optional learning practice. It reads authoritative progress/targets and refreshes their display; IDs, count, thresholds, achievements and recorded labels are unchanged. Species names are rendered through existing definitions, with human-readable fallback names; raw IDs remain intact in state and lookup keys. Annual Review, Reference comparison labels and regeneration review text use display formatting only.

Nursery buttons show quantity and exact purchase total. Before purchase, the card explains that stock is not planted, shows cash and uncommitted cash after purchase, and derives its warning from the existing harvest quotes, minimum charge and CashOutlook/WarningText. It distinguishes contractor/shelter costs from owner-time unsheltered planting (no external labour charge). The warning is advisory: an affordable but unwise purchase is still permitted by the unchanged purchase API. Inventory/execution/economic rules remain unchanged.

Supporting text changed from 13 to 16 pixels (muted) and 12 to 15 pixels (faint/stat labels). Existing modal backgrounds are opaque to keep forest texture from obscuring Work Plan and Annual Review. No general UI redesign was performed.

## Exact production files

- `Assets/ForestPrototype/UI/MenuHelpView.cs` — management reasoning and menu orientation.
- `Assets/ForestPrototype/UI/LearningObjectivesView.cs` — teaching copy and current objective display.
- `Assets/ForestPrototype/UI/ScenarioOneUiFacts.cs` — seed-absence diagnosis, species/objective display formatting.
- `Assets/ForestPrototype/UI/WalkingHudView.cs` — supplies authoritative state to ground diagnosis.
- `Assets/ForestPrototype/UI/StandMapView.cs` — supplies authoritative state to map diagnosis.
- `Assets/ForestPrototype/UI/WorkPlanView.cs` — execution/pruning/nursery teaching and existing-cost advisory.
- `Assets/ForestPrototype/UI/AnnualReviewView.cs` — return/reassess teaching and readable objective labels.
- `Assets/ForestPrototype/ScenarioEcologyReviewLines.cs` — readable species names in formatted review text.
- `Assets/ForestPrototype/UI/Resources/ScenarioOneUi.uss` — supporting font sizes and modal contrast.

## Reusable verification files

- `Tools/Verification/ScenarioOneTeachingCopyVerification.cs` — new batch and rendered teaching gate.
- `Tools/Verification/run_s1a_gates.py` — sequential isolated targeted/focused/current-regression runner, repeated batch hash check and baseline anchor comparison.
- `Tools/Verification/S1A_README.md` — reproducible commands, contracts and cleanup.
- `Tools/Verification/ClearanceVerification.cs` — selected historical interactive-only/first-help/preference corrections reconciled with current Model2.
- `Tools/Verification/MenuTutorialVerification.cs` — selected interactive guard plus real arrival/aim settling and accepted E-inspection alternative for obstructed ground rays; no fabricated learning credit.
- `Tools/Verification/ScenarioOneRemovalVerification.cs` — current speciesless U-area clearance planning, no immediate removal/cash charge; legacy API coverage retained.
- `Tools/Verification/WindthrowV1/run_regression.py` — stages the three reconciled current fixtures rather than old P0 copies. The other modern suite fixtures and existing historical integration/version-5 adjustment are retained.

Historical `1a36ea97c724b3a026e76a0b5432349b46e6a298` was inspected as reference, never wholesale cherry-picked. One early Menu repetition missed real site-inspection credit because the aim ray was obstructed; its failed evidence is retained locally. The fixture correction waits for real state and uses the existing keyboard inspection path when necessary. Both final standalone repetitions and the current regression run use the corrected fixture.

## Verification

Unity import/compile: PASS, zero C# compiler errors. Supplemental offline P2/P3 checks also passed.

| Gate | Final result |
|---|---|
| Teaching batch 1 / batch 2 | PASS / PASS, 20 checks each, identical read-only hash `EBE7A228F630AE41` |
| Teaching interactive | PASS, 57 checks and 14 rendered captures |
| MenuTutorial standalone 1 / 2 | PASS / PASS (101.2 s / 61.4 s) |
| Clearance targeted | PASS under current Model2 |
| Removal targeted | PASS under current controls |
| P2 focused | PASS twice batch + once rendered, exact required hash on all three |
| P3 focused | PASS twice batch + once rendered, exact required hash on all three |
| Full current regression | **24/24 PASS**, all production source manifests unchanged during gates |
| Every exercised modern hash vs committed baseline | PASS, all equal |

See `verification-results.json` for every individual gate, exact markers, source provenance and elapsed times. Gates ran on uncommitted implementation bytes before the implementation commit; `source-manifest.json` identifies those exact bytes. The regression launcher was already running when the additional all-hash validator was added, so that validator was applied to the completed 24-gate evidence as a separate final check. The committed runner includes the same validation for future runs.

Required compatibility anchors:

| Contract | Result hash |
|---|---|
| P2 | `F58FB0B1A421D28B` |
| P3 | `F7C2FC966816BBAD` |
| Model2 completion compatibility (v18 projection) | `702766DECE591E21` |
| Reference Future v1 year 100 | `7AD177B3CC2F73C7` |
| Reference continuation legacy v15 layout | `9CDF21A541C5968D` |
| Current save-19 completion | `84CD51EB6A951D9E` |
| Current save-19 Reference continuation | `5CFEB0C6A01A670B` |

The packet compatibility anchors and the modern save-19 values are distinct projections of the accepted current fixtures; both must remain unchanged. Every exercised 16-digit hash in all 24 gates is compared with the committed modern regression record, not merely the five named packet anchors.

## Rendered evidence and reproducibility

Run the commands in `Tools/Verification/S1A_README.md` with Unity 6000.6.0f1, one Editor at a time. Isolated settings/licence/saves are under `Build/UnderstoreyRecruitment/config`; each worktree uses its own Library.

Full-resolution teaching evidence: `/home/jer/CCF-s1a/Build/S1A/evidence/teaching-rendered/`. Fourteen PNGs cover Help, Learning, current objectives, Crop Tree Inspection, Work Plan, Nursery/pre-purchase warning and Annual Review, each at 1280×720 and 1920×1080. Filename pattern: `s1a-{help,learning,objectives,inspection,workplan,nursery,review}-{1280,1920}.png`. Automated bounds/control checks and visual inspection found no important clipping, hidden required controls, material overlap or unreadable supporting text. Objective/species labels are human-readable; purchase warnings and Buy controls remain visible. Screens are scrollable existing modals.

Additional P2/P3 rendered evidence: `/home/jer/CCF-s1a/Build/P2/evidence/` and `/home/jer/CCF-s1a/Build/P3/evidence/` (1280×720, 1600×900 and 1920×1080). Logs/results remain in ignored `Build/S1A`, `Build/P2`, `Build/P3`, and `Build/WindthrowV1/Regression`. Portable results and SHA-256 evidence/source manifests are committed beside this handoff; large logs/PNGs are local ignored evidence, not Git payload.

## Diff audit, preservation and status

Final diff is restricted to the nine listed production files, seven reusable verification files and this verification handoff/results/manifest directory. No simulation equations, schema/persistence, objective achievement/completion logic, economy values, P2/P3 calculations, storms or Reference archive changed. Accepted stack remains save 19 / RNG 1 / regeneration 2 / growth 1 / storm 0, with StormModel1 dormant. Serialized assets touched by the committed change: **none** (no scenes, prefabs, materials, .asset, .meta, packages or settings).

Unity generated settings/material normalisation in this initially clean task checkout during rendered gates. The exact diff is retained in ignored `Build/S1A/editor-normalisation.diff`; only those task-generated changes (one settings file and eleven materials) were restored to the base after all Editors exited. All disposable Assets verification sources/generated .meta files were removed by their launchers. Final whitespace audit passed; worktree is clean after handoff commit. Authoritative GitHub main was rechecked at the same `2c0f7cd` revision before final handoff.

The new branch started directly at the exact authorised base with a clean status. Live S1-A ownership was recorded through the initially clean `/media/jer/ZX20/CCF-post-p3-context-refresh` integration/coordination checkout, in local coordination-only commits `20d6c8d` and `abece51`. No canonical context was regenerated. That checkout was not used for implementation. `/home/jer/CCF-main` at `402a2b41108dc9aa91e225db9047956b0a91a97e` and its unrelated settings, materials, recovery files and dirty state were preserved; no checkout/reset/stash/clean/update was performed there.

## Limitations and recommendation

These are Unity Editor gates and controlled rendered first-cycle fixtures. They establish teaching/presentation and compatibility readiness, not standalone Player certification, a human beginner pilot, universal resolution/scaling coverage, or later S1-D interventions. Nursery advice reflects current authoritative quotes; future work can cost more and must still be reviewed.

Manager should review this branch and evidence, then assign an integrator to incorporate the implementation/tooling/handoff commits onto the accepted main and rerun required post-integration gates. Keep local coordination-only commits out of gameplay integration. Proceed to S1-B/Stage-A beginner pilot preparation only after Manager acceptance. This worker has not self-integrated.
