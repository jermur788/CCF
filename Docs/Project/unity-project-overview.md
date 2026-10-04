# Unity Project Overview

## Purpose and baseline

Verified technical/implementation facts and local checking guidance, not design plans. Inspected repository/Unity state outranks planning for implementation questions.

Revision 5 baseline supplied by the Overall Manager:

- Snapshot date: 2026-10-02.
- Repository: `https://github.com/jermur788/CCF.git`.
- Branch: `main`.
- Exact implementation baseline: `0fc92ca989c0df7109e2507b2aab5712f0cf6077`.
- Inspect live worktree status before writes. Mutable branch positions may differ from this historical snapshot.

At the supplied baseline, forestry/survival branches matched main. The separately recorded frozen chain ahead of main is:

| Branch | Tip | Ahead | Summary |
|---|---|---:|---|
| save-hardening-and-growth-cache | 3b71e4b | 1 | Save/load hardening and growth readout |
| rng-versioning | 8aede39 | 2 | Opt-in model 1; legacy model 0 preserved |
| batch-recompute | 457c897 | 3 | Batched rebuilds/shared Hegyi forecast term |
| edge-bias-diagnostic | a31ec62 | 4 | Edge diagnostic |

These changes are not in the pinned baseline. Their integration/acceptance must not be inferred from a branch label. The migration packet freezes these exact commits; it authorises no stack modification or merge. Verify other worktrees live rather than treating this baseline as their current state.

Later state (verified 2026-10-02): all four stack commits are ancestors of `main` (via merge `8ae7a25`), and the combined ecology package is integrated on top — see **Integrated ecology package** below.

## Engine and environment

| Detail | Baseline value |
|---|---|
| Editor | Unity 6000.6.0f1 |
| Pipeline | URP 17.6.0 |
| Input System | 1.20.0 |
| Test Framework | 1.8.0 |
| AI Navigation | 2.0.14 |
| Unity MCP | com.kitwright.unity.mcp from GitHub |
| Serialization | Force Text |
| Metadata | Visible Meta Files |
| Development environment | Linux; VS Code |
| Player controller | Assets/ForestPrototype/ForestPlayer.cs |

The MCP dependency is an unpinned Git URL in `Packages/manifest.json`; the lock records `585ada203ff25b1fbea23821e159afa4a930d481`. There is no `.gitattributes` at baseline, so neither Git LFS nor a Unity YAML merge driver is configured.

## Project-context migration

At main baseline `0fc92ca…`, `Docs/Project/`, root AGENTS/CLAUDE and `Tools/ProjectContext/` did not exist; root `AI_Instructions` existed and was stale. Decision Log/Milestone define the accepted migration target, not an already-integrated baseline fact.

The migration was implemented on `task/project-context-migration` (tip `42db4e9`, based on the baseline SHA). It changes documentation/agent entry points and the stdlib Python generator only.

Later state (verified 2026-10-04): `42db4e9` is an ancestor of `main`, so the migrated context, entry points and generator are integrated. The generator has been run from clean context commits on `main` (outputs in ignored `Build/ProjectContext/`). Remaining setup-milestone gates (worker Unity smoke runs, permanent Drive publication, project attachment refresh) are tracked in the Current Milestone.

## Shared VS Code settings

`.vscode/settings.json` is tracked shared configuration: exclusions/associations, nesting and default `CCF.slnx`, without worktree-specific paths. Keep it unchanged; use VS Code user settings or an external `.code-workspace` for local differences.

## Scenes and code

- `Assets/Scenes/ForestTest.unity`: reference forest/scenario scene.
- `Assets/Scenes/MixedSpeciesTest.unity`.
- `Assets/Scenes/ForestryAssetReview.unity`: walkable art/LOD review.
- `Assets/Scenes/SampleScene.unity`: exists, not the reference scene.

Most code is in `Assets/ForestPrototype/`:

| Area | Implementation |
|---|---|
| Individuals | ForestTree: ID/species/state, exclusive marks, pruning history |
| Ecology | ForestEcologyController: competition, canopy/light, growth, crowns, regeneration, seeds/establishment/promotion, diagnostics |
| Juvenile rules | JuvenileEcologyRules: shared juvenile survival/growth/promotion path for natural cohorts and exact planted juveniles |
| Mortality | ForestTree.ApplyMortality(cause, year): explicit biological death, separate from Fell() |
| Natural regeneration | Species-keyed, origin/history-aware cell cohorts |
| Exact planting | Scenario One individual juveniles until promotion |
| Management | ScenarioOneManager: work, economy/history, nursery/planting, treatment patches, retained timber, reference and Work Plan |
| Save/load | ForestSaveController / ForestSaveData |
| Understorey | Provisional functional-group state, not researched causal v1 |
| Habitat/audio | Derived from ecology/understorey/deadwood/juveniles |
| Art | Editor material/LOD/prefab/review-scene tooling |

## Save and interactions

Baseline `ForestSaveData.CurrentVersion = 13`. The ecology package raised it to 14 (biological-mortality cause/year). Current integrated `main`: `ForestSaveData.CurrentVersion = 15` (Scenario 1 execution/protection/economy fields, below); definition still `scenario-one-v13`, display `Scenario One — Sitka Plantation to Continuous-Cover Forest`, execution `ManagementOnly`. v1–v14 saves restore; atomic save hardening is in place.

V13 includes two-type marks, exact juveniles, clearance patches, retained construction timber, management/economy/history, understorey, deadwood, work orders and pruning history.

Marks are None/Fell/CropTree, exclusive. Trees store lifts/crown base/last year; Scenario One batches eligible Crop Tree pruning. Purchased Oak/Beech planting has ground-selected positions, markers, contractor orders, persistent individuals and promotion. Legacy cell/cohort planting remains for historical compatibility.

Felling outcomes: SellAndExtract, RetainAsFallenDeadwood, KeepForUse. Construction timber is tracked separately.

## Annual sequence

1. Competition.
2. Canopy/light.
3. Adult growth.
4. Crown relaxation.
5. Canopy/light refresh.
6. Regeneration growth.
7. Mast.
8. Seed rain.
9. Establishment.
10. Promotion.
11. Establishment suitability.
12. Opening decay.
13. Diagnostics.

At the Revision 5 baseline, browsing and researched three-group causal Understorey v1 were not authoritative systems. Browsing & Protection v1 is now integrated (see **Integrated Scenario 1**); causal Understorey v1 is still not authoritative.

## Reference Future v1

Historical reference, intentionally frozen after the v13 overhaul:

| Field | Value |
|---|---|
| Schedule | reference-future-v1 |
| Definition / save | scenario-one-v12 / v12 |
| Seed | 20260914 |
| Start | 336 Sitka; 2,100 stems/ha |
| Historical canonical lifecycle (pre-calibration) | 7E39B70A14959FAD — superseded for current ecology |
| Initial full world | A564039D9B7CE31D |
| Schedule hash | 56C8B99FA1E8DDD1 |
| Year 20 | F7DF7DAB53B6FD32 |
| Year 50 | D5E75D6D21D631AC |
| Year 100 | 7AD177B3CC2F73C7 |

Contract (D-042, `Docs/ReferenceFutureContract.md`): immutable archive; integrity checked on the original embedded JSON, never by reserialising through current save classes; frozen milestones load/preview under current code; Year-50 historical save continues deterministically under the current ecology and save version (v12 → v14 at the ecology integration, v12 → v15 since Scenario 1). Exact historical biology replay is not required; continuation divergence is diagnostic only. A new model gets a new reference version, not an edit to v1.

Schedule: `Assets/ForestPrototype/ScenarioOne/Resources/ScenarioOneReferenceScheduleV1.json`. Existing scenario/reference/art documentation is in `Assets/ForestPrototype/Docs/`, including `ScenarioOneReferenceFutureV1.md`; other verification/design documents are in root Docs.

## Integrated ecology package

Integrated on `main` 2026-10-02 by cherry-picking `1319c2c`, `2632de6` and `6dc6daf` onto context commit `cd239d7`, with no conflicts. The resulting gameplay integration head is `b1e6c51`. Its code tree is identical to `6dc6daf`; only main's newer canonical-context docs differ.

- **Shared juvenile ecology:** `JuvenileEcologyRules` is the single biological path for natural cohorts and exact planted juveniles. Legacy planted cohorts remain compatible, exact planted identity/position/provenance is kept, and promotion is deterministic.
- **Mortality foundation:** `ForestTree.ApplyMortality(cause, year)` sets `biologicallyDead` with cause and year. It is idempotent, excludes the tree from living systems, persists in save v14 and restores without mortality/harvest events. Ecological death is separate from `Fell()` and yields no timber or cash. There is no automatic adult mortality, no storms and no assumed deadwood disposition.
- **C8 growth/competition (Sitka):** Hegyi cutoff 8 m (`ForestEcologyController.HegyiCutoffMeters`; global), Sitka Ci50 5, Sitka potential DBH growth 1.2 cm/yr. The competition equation is unchanged. Beech/Oak keep Ci50 3 and 0.4 cm/yr.
- **k10a10 light:** canopy shade reach = 1.0 × crown radius + half a 5 m cell (2.5 m) (`CanopyShadeReachPerCrownRadius`, shared by the marking forecast). Opacity is an implicit 1.0 and the light equation is unchanged.
- **Calibrated canonical lifecycle (RNG model 0, 80 years):** `BFC55473C1506067`. The calibration measurement hash is `D48A19525E69DD8A`.

Verified on integrated `main` at `b1e6c51` (Unity 6000.6.0f1, batchmode, each gate in its own process with an isolated `XDG_CONFIG_HOME`):

| Gate | Result |
|---|---|
| Import/compile | 0 compiler errors |
| EcologyCalibrationAdoptionVerification | PASS. Hash D48A19525E69DD8A. Interior/whole 0.3566/0.3877. Q/moderate/heavy 1.171/1.558/1.739 (interior 1.844). Edge 1.130. Light at Y5/Y10: control 0.029/0.043, Q 0.045/0.061, moderate 0.148/0.163, heavy 0.254/0.257 |
| JuvenileMortalityFoundationVerification | PASS. Shared juvenile rules (30 combinations), promotion persistence and tree mortality all pass |
| Canonical lifecycle, two separate processes | BFC55473C1506067 both times. The integration harness also reproduced it in A/B runs |
| Save hardening / RNG model / batch recompute | PASS |
| Scenario One interaction / habitat presentation | PASS. Year-0 grass: 4 patches in 3 supported cells; dark-cell negative control detected |
| Reference Future v1 | Archive integrity, archive negative control, preview Y0/20/50/100, current continuation (v12 → v14, deterministic) and contract all PASS. Year 100 = 7AD177B3CC2F73C7 |
| CCFIntegrationVerificationTemp, Beech, Oak, Oak player planting, planting | PASS |

The frozen archive and schedule blobs are byte-identical to the pre-integration main. A rendered ForestTest Year-0 smoke used an offscreen batch-mode render with Ultimate Nature present locally. The stand rendered normally, and grass appeared only at the bright clearing/road edge. This was not an interactive play session.

## Integrated Scenario 1

Fast-forwarded into `main` on 2026-10-04 from `integration/scenario-one-complete` at `dc975e4` (pre-merge main `bb314a4`). Detailed records: `Docs/BrowsingProtectionV1.md`, `Docs/Scenario1EcologyCompletion.md`, `Docs/Scenario1EconomyIntegration.md`, `Docs/Stage1WorkEconomyFoundation.md`, `Docs/TimberAssortmentYieldV1.md`, `Docs/SitkaPlantation02Integration.md`, `Docs/Scenario1StartingStandAssetFix.md`, `Docs/Scenario1CompactBrashFix.md`.

- **Browsing & Protection v1:** shared juvenile browsing response for natural cohorts and exact planted juveniles. Scenario One background browse pressure defaults to 0.2 (`ScenarioOneDefinition.backgroundBrowsePressure`; the asset does not override it). Neutral pressure 0 keeps the calibrated lifecycle `BFC55473C1506067`; the Scenario One lifecycle at 0.2 is `3485B6630C9EA448`.
- **Shelters:** tree shelters are coupled to exact planting and share its executor. A live shelter uses `installedYear` = resolution year and `effectiveYears` = 8. v15 restore clears protection, then restores saved shelters, then saved protected areas. `BrowseProtectionGeometry` builds fence-area records only; there is no fencing gameplay.
- **Work and economy:** `WorkEconomy` settlement, contractor or landowner-simulated execution per order (harvest is contractor-only), owner minutes, and `TimberYield` assortment allocation with sell / KeepForUse / retained deadwood outcomes. Economy continuation hash `023F4B627B758284`.
- **Save v15 bounded fields:** order executionMethod/installShelter/harvestJobId; scenario shelters/protectedAreas/ownerMinutesUsedThisYear; report minimum adjustment, owner minutes, retained volume and per-product sold volume/revenue; event executionMethod/ownerMinutes. No browse pressure, browse history or form damage is persisted.
- **Regeneration diagnosis and review:** aimed-ground diagnosis line, shelter visuals and annual review ecology lines.
- **Completion verification:** `Tools/Verification/ScenarioOneCompletionVerification.cs`, completion hash `568922E1A6D73CDD`.
- **Presentation:** Plantation Sitka 02 forms for trees under 20 m (pole/first-thinning), with legacy Sitka families retired from the active stand. Moss uses `Ground_Moss_Shoots_03` cushions, and `Forest Ground.mat` uses the textured soil/needle-litter material. Felling brash is a compact grounded 1.9–2.4 m patch beside the stump [D].

Integration regression (on `integration/scenario-one-complete`, Unity 6000.6.0f1, isolated-config batch processes): 27/27 gates PASS, after one fixture-version fix (`6ab31ce`).

Post-merge verification on `main` at `dc975e4` (2026-10-04):

| Gate | Result |
|---|---|
| Import/compile | 0 compiler errors |
| ScenarioOneCompletionVerification | PASS, `568922E1A6D73CDD` |
| Canonical lifecycle | `BFC55473C1506067`; Scenario One pressure 0.2 `3485B6630C9EA448` |
| BrowsingProtectionVerification | PASS (with shelter visuals, review lines, regeneration diagnosis and fence geometry) |
| Reference Future v1 | Contract PASS; Year 100 `7AD177B3CC2F73C7`; continuation v12 → v15 `9CDF21A541C5968D` |
| Scenario One interaction | PASS, including compact felling residue |
| Habitat presentation | PASS; legacy Sitka 0, one active display per tree at Years 0/20/50/100 |

The user's interactive Scenario 1 smoke (2026-10-04) passed, including the starting-stand and compact-brash presentation corrections. No performance problem was reported. Final asset/presentation acceptance has not run. Astra ForestFloorV1 is an external candidate delivery and has not been adopted.

## Art and storage

Baseline committed presentation includes pruning families, refined Sitka benchmark, brash, logs/stumps/floor detail, bramble/bracken/fern/grass, other habitat, Oak/Beech juveniles, bent/cavity cosmetic variants, grass/rush and walkable LOD specimens. Ring-barked, windthrow root and some defect assets exist without causal spawning because matching state is absent.

Presentation must not invent ecology. Large binaries are ordinary Git objects; no new LFS/storage dependency or history rewrite is authorised.

## Verification

Disposable harnesses are under `Tools/Verification/`. Read `Docs/SuppressionHistory.md` and inspect each harness for side effects.

Documented integration procedure:

1. Use an idle/disposable worktree.
2. Copy `Tools/Verification/CCFIntegrationVerificationTemp.cs` into `Assets/ForestPrototype/`.
3. Run:

```text
<UNITY> -batchmode -nographics -projectPath <checkout> -executeMethod CCFIntegrationVerificationTemp.Begin -logFile <log>
```

4. Remove the copy and generated `.meta`.

This auto-start harness can touch `forest-save.json` and normally backs it up/restores it; assess that risk before running. Never leave it in a playable build. Compare hashes only under the verified Editor/configuration conditions.

Other documented entry points include `CCFBeechVerification.Begin`, `CCFOakVerification.Begin`, `CCFOakPlayerPlantingVerification.Begin`, `CCFPlantingVerification.Begin`, ScenarioOne Planting/Pruning/Removal/Deadwood/Progress verification `.Begin` methods and `ScenarioReferenceVerification.Begin`.

Minimum worker smoke: identify path/branch/HEAD/status; check shared settings; own Library; open Unity 6000.6.0f1 and ForestTest; check compile/console; run selected appropriate harness; remove script/meta; exit without unrelated serialized saves; account for final Git changes.

The migration generator uses Python 3.10+ standard library, not Unity. Run `python3 Tools/ProjectContext/generate_project_instructions.py` only after committing clean sources. Outputs are ignored in Build/ProjectContext.

## Local worktrees

The final integration/OpenAI/Claude worker layout and their independent Unity smoke gates are not established by this document. Target separate writable integration, OpenAI and Claude worktrees; preserve existing names if renaming would risk work. Check current paths/branches with `git worktree list` and record verified setup results through a separate assigned task.

Observed 2026-10-04 (`git worktree list`): `/home/jer/CCF-main` is the integration/main worktree (now on `main`). `/home/jer/CCF` (`feature/forestry-ecology`) holds uncommitted user ecology/art work that must be preserved. Task worktrees `/home/jer/CCF-claude`, `/home/jer/CCF-openai`, `/home/jer/CCF-opencode-economy`, `/home/jer/CCF-context-sync`, `/home/jer/CCF-project-context-migration` and `/home/jer/CCF-review-ecology` remain on their task branches.

## Revision 5 provenance

Source pack: Drive folder `1fUyti4nHlbKKwjWnvWsVvSbvFN-4mhm0` (Claude Review Pack — Refreshed Drafts); review request identifies Revision 5 and D-035–D-040 acceptance. Native Google Docs were read through authenticated read-only Drive access, not changed.

| Repository destination | Source Google document ID |
|---|---|
| Docs/Project/game-brief.md | 1Rkvo9nUrU99g8wz1XU2CSibo-WWZpoPK8u_DSKVOS9w |
| Docs/Project/decision-log.md | 1nODkvCnHD-89NziaRY4qreaba61yYn6JqvdwpyYLZ04 |
| Docs/Project/current-milestone.md | 10ksKpEkmMllHTZq9aCRwGTurv4S0TG9SjXBlCFcNgik |
| Docs/Project/unity-project-overview.md | 1WSjjXXGgA7f9Pyppo2PQdJ2pjXXW5xbqHzMh2cm7c1o |
| Docs/Project/AgentWorkflow.md | 1vh_C5dKZ6FAlucRV4K_KvMRp5vvjWwrRJ6LMBw7jMR0 |
| Docs/Project/research-index.md | 12pyVVmHpAcdyuxQsOEXQVkO2qFO6borwsRsrN61FpTE |
| Docs/Project/instructions/shared-project-instructions.md | 1t03jEuGVJxfYAXPAilLkgJWI_7a_RuzWBuASIfE-PIw |
| Docs/Project/instructions/chatgpt-role.md | 11_myY8N1taegsf-M8tQSXiZ2rjFzpCkop5w33TSf7AI |
| Docs/Project/instructions/claude-role.md | 1VH8V-t9xqDg02yv5c_Ui65BrfhbQCf_Ok3PwNoN64WU |
| Docs/Project/coordination/active-tasks.md | 1AUdhc-HnKTkWo5sbkwPOTtLGEIkhkgSShofv8x0Konk |
| AGENTS.md | 1QKEsZKCPblDEZUOWHTaSMYAV2f9jmWpGKboJd3EaKqo |
| CLAUDE.md | 1G-RaRphkDLoWSG_sXosgAuLy4K3OAbRzdxsV955oLlg |
| Tools/ProjectContext/generate_project_instructions.py | 1Q_Htq4yJ3xgfphDdCdYYz26VMccjoFOV850TMI-Cs_U |

Migration normalises text layout/encoding, ports useful legacy rules, records the task scope/baseline, and makes the supplied generator executable with clear error handling. It does not advance gameplay design or incorporate the frozen stack.
