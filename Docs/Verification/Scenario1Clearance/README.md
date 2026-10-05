# Scenario 1 clearance acceptance evidence

Branch: `task/scenario-one-clearance-preview`. Base: UI candidate `83574c8`. Locked context: `691dd18`. Task implementation only; no merge or integration approval. Human smoke review remains pending.

## Gate results

| Gate / mode | Result | Seconds |
| --- | --- | ---: |
| Import (default) | PASS | 270.09 |
| ClearanceVerification (default-rendered) | PASS | 65.17 |
| ScenarioOneInteractionVerification (default) | PASS | 76.55 |
| ScenarioOnePlantingVerification (default) | PASS | 48.31 |
| SaveHardeningVerification (default) | PASS | 43.08 |
| ScenarioOneEconomyIntegrationVerification (default) | PASS | 55.93 |
| ScenarioOneCompletionVerification (default) | PASS | 59.17 |
| ScenarioOneCompletionVerification (model0) | PASS | 68.47 |
| BrowsingProtectionVerification (default) | PASS | 137.63 |
| ScenarioReferenceVerification (default) | PASS | 67.7 |
| RngModelPolicyVerification (default) | PASS | 73.5 |
| ScenarioOnePresentationReview (default-rendered) | PASS | 172.32 |

The final rendered clearance fixture reports `CLEARANCE_ACCEPTANCE_PASS`: A–J all pass, with 3 species cohorts, 1 exact inside juvenile and 12 competing ground patches. It checks protected crop trees, just-outside targets, preview/execution agreement, safe cancellation, save/load, duplicate order prevention and one settlement. The real walking ground-ray path invokes the shared query. A separate normal annual resolution confirms completed work, one debit and treatment-year display suppression.

A 64-cell matrix confirms identical same-year planting spots return no targets and leave density and saved history unchanged. Distinct overlapping circles retain the existing union-area calculation. A small centre circle does not block subsequent whole-cell clearance.

Rendered previews and planting controls pass at 1280×720, 1600×900 and 1920×1080. The final focused fixture asserts real size labels and no overlap between planting controls and ground report. The broader review produced 56 UI captures, covering inspection, Work Plan, Annual Review, Map, planting, save reconstruction and Reference return; contact sheets and key full-size screens were inspected. That broader review preceded the final HUD-only placement correction, which the focused final fixture verifies.

## Canonical anchors (unchanged)

| Anchor | Value |
| --- | --- |
| Model 1 completion | `00479F18970F9926` |
| Model 0 completion | `568922E1A6D73CDD` |
| Model 0 neutral / normal lifecycle | `BFC55473C1506067` / `3485B6630C9EA448` |
| Model 1 neutral / normal lifecycle | `2A0B8C32AC0DE113` / `506E8AF6D8514C6C` |
| Historical continuation | `9CDF21A541C5968D` |
| Model 0 / 1 policy continuation | `CDC4DE8F8EF471E5` / `872094413305A082` |
| Reference schedule / archive FNV | `56C8B99FA1E8DDD1` / `1A42C7BD0439E72F` |
| Reference year 0 | `A564039D9B7CE31D` |
| Reference year 20 | `F7DF7DAB53B6FD32` |
| Reference year 50 | `D5E75D6D21D631AC` |
| Reference year 100 | `7AD177B3CC2F73C7` |

The launcher stops on missing pinned anchors; no new lifecycle anchor is accepted. [Machine-readable results](results.json) preserve gate markers, source hashes, log hashes and screenshot hashes.

## Selected smoke evidence

The mixed patch is controlled fixture cell D4, with a protected crop tree, Sitka/oak/beech cohorts, exact inside/outside saplings and competing cover. Before/after images show the isolated production work effect without annual seed rain; production habitat rebuild is called. The separate annual fixture covers normal execution. The camera is at walking height; surviving standing-tree branches can occlude the ground boundary. Moss/litter and outside plant foliage are deliberately retained.

- [Before](before.png)
- [Exact square preview and affected markers](preview-ui.png)
- [After clearance](after.png)
- [After save/load](loaded.png)
- [Preview at 1280×720](preview-1280-ui.png)
- [Preview at 1920×1080](preview-1920-ui.png)
- [Planting circle and controls](planting-circle-ui.png)
- [Planting circle at 1280×720](planting-circle-1280-ui.png)
- [Work Plan confirmation](work-plan-ui.png)

These nine PNGs total 21.7 MiB. Full rendered captures, contact sheets, current logs and failed diagnostic attempts remain locally under ignored `Build/ClearanceVerification/`. The original user screenshot was not supplied, so its individual mesh is unconfirmed. Inspected bramble/bilberry habitat owners and natural/planted regeneration owners are covered; no asset is deleted globally.

## Reproduction and boundaries

Use this dedicated worktree with Unity 6000.6.0f1. The launcher uses an exclusive worktree lock, isolated configuration/save slot, two workers, bounded 8 GiB memory, and disposable harness staging. It removes the staged script/meta on exit. Read the relevant harness before running it.

```sh
python3 Tools/Verification/run_clearance_gate.py ClearanceVerification --interactive --capture
python3 Tools/Verification/run_clearance_gate.py ScenarioOneCompletionVerification --rng 0
```

Existing base GUID closure assets (six ignored stump/mushroom assets) were present locally for rendering; no package or dependency was added. Graphics used GTX 980M/Vulkan. No shared Library was used. Own Editor-generated tracked material/settings normalization was restored; recovery/crash artifacts were preserved under ignored build attempts. Some diagnostic graphical sessions logged an Editor Search indexing exception; successful runtime assertions and rendered inspection do not imply an error-free Editor subsystem.

The [implementation record and manual smoke steps](../../Scenario1ClearanceCorrection.md) describe controls, ownership and trade-offs. Save schema, Reference Future v1, ecology calibration and contractor rates remain unchanged. Clearance suppression lasts through the treatment year; subsequent recolonisation follows existing ecology. Confirmation uses the existing Work Plan task approval, not a new immediate world action.
