# PARITY-06 — Unity timber/economy/pruning qualification

**`BROADLEAF TIMBER ECONOMY AND PRUNING QUALIFIED`**

**Remaining PARITY-06 Unity/G4 prerequisite: PASS. Recommend G4 integration may proceed on the qualified production bytes.** This is bounded feature qualification, not integration itself, whole-parity acceptance or final player-test readiness.

## Identity and boundary

- Claim [#92](https://github.com/jermur788/CCF/issues/92), umbrella #91. Claim remains open for Manager/integrator disposition.
- Actual qualifier: **OpenAI `gpt-6.1-sol`, Codex harness/provider instance `codex`, high reasoning, inside T3 Code**. Selection read back through `t3_thread_configuration`. No child agents or independent reviewers were spawned by this qualifier.
- Exact production candidate tested in every candidate Unity launch: **`6abfb59e81d394e2667694446ac7e333e3488594`** (integration parent `dc4f59b` plus reviewed PARITY-06 branch).
- Candidate/source worktree: `/media/jer/Videos/CCF-Parity06Unity`, branch `qual/parity-06-unity-v1`. Publication adds verification tooling and this handoff only. Production bytes are unchanged from `6abfb59`.
- Supplemental gate commit: **`bd3ce0e5a1bb9e07b4a23aab61ad16a94249b84f`**; its exact file bytes were staged by absolute path into the candidate lane and hashed by the runner. No production overlay was used.
- Context snapshot: `63e95ba2175f7e239a278c616681b286719821f8`. Live GitHub coordination and claims were read separately. Their old queue/storage text predates this explicit Unity dispatch; the current user packet supplies the exact candidate and U1 release.
- Authority inspected: AgentWorkflow, decisions, milestone, game brief, Unity overview, research index, approved three-species spec/plan/PARITY-06 task, implementation handoff and independent review. The review approves these production bytes for qualification and G4 once required gates pass.
- Unity: **6000.6.0f1**, revision `f7f8ed4d1e24`, Linux Editor at `/media/jer/ZX20/Unity/6000.6.0f1/Editor/Unity`.
- Durable evidence: **`/media/jer/Files/CCF/Verification/Parity06-Unity-20261029/`**.
- No production repair, ecology/economy retune, package, schema, asset/YAML, expected-value change, protected-anchor rebaseline, integration, PR or Windows build.

## Preflight and execution safety

Source branch and U1 were clean including untracked files. U1's prior detached HEAD was `a900b7faec3e48f6cfc8be7e3bb3b147fe3a61aa`; it was placed detached at the exact candidate. The normal machine-wide lock was free with no Editor PIDs. Storage was rechecked: ZX20 `/dev/sda2` had approximately **21 GiB free**, above the normal 15 GiB floor; durable Files output is on `/dev/sdc4`.

Normal lock: **`aad5bcdbde9c4ca0b7efd044fca7f544`**, owner **`Sol-PARITY-06`**, task **`PARITY-06`**, lane **U1**, branch **HEAD**, exact candidate. Acquired `2026-10-29T06:42:30.317527+00:00`. No bypass, force release, concurrent Editor, cache sharing or process reaper.

The three-species runner, generic qualification runner and applicable legacy runners/harnesses were inspected before execution. The generic `Tools/Verification/ThreeSpecies/Qualification/run_qualification.py` was used because it supports additional residual files, isolated licence/config/save paths, capped memory, arbitrary existing gate entry points and explicit receipts. It stages disposable files, runs one Editor, removes source and generated `.meta` in `finally`, records source hashes and detects tree changes. Existing in-Assets interaction/habitat gates were invoked without staging duplicates. Assertions and original configurations were retained.

An initial Legacy40 launch selected the runner's old host licence and exited **198** before import/assertions (`No valid Unity Editor license found`). The next launch explicitly selected the newer Hub licence. This operational failure remains under `timber-legacy40/`; it is not a gameplay assertion failure or a passing run. No host licence was edited. Later runs use separate `XDG_CONFIG_HOME` per gate; pruning's disk save and all other state are isolated there.

## Gate results

**14/14 required Unity gate configurations PASS, zero assertion failures, zero compiler errors, every successful Editor exit 0.** There were 15 Editor launches including the initial pre-assertion licence failure described above. The unchanged gates were not retried to conceal a product failure.

| Evidence label / gate | Terminal PASS token | Count emitted / independently counted | Seconds |
|---|---|---|---:|
| `timber-legacy40-licensed` / broadleaf timber, geometry 0 | `BROADLEAF_TIMBER_VERIFY_PASS` | 66 assertions | 69.5 |
| `timber-enlarged80` / broadleaf timber, geometry 1 | `BROADLEAF_TIMBER_VERIFY_PASS` | 66 assertions | 85.0 |
| `residual-legacy40` / additive residual gate | `PARITY06_RESIDUAL_VERIFY_PASS` | 86 checks | 74.7 |
| `pruning` / existing focused pruning | `SCENARIO_ONE_PRUNING_VERIFY_PASS` | Assertion total not emitted | 58.0 |
| `sitka-growth` / existing growth/mortality | `SITKA_GROWTH_MODEL_VERIFY_PASS` | 21 checks | 118.4 |
| `sitka-density` / ANCHORS + SITKACOMP | `DENSITY_QUAL_PASS` | 3 hard checks; 24/24 controller sets | 78.0 |
| `sitka-timber` / existing timber-yield | `TIMBER_YIELD_VERIFY_PASS` | 120 fixtures / 3,007 assertions | 71.0 |
| `legacy-geometry` / existing geometry | `STAND_GEOMETRY_VERIFY_PASS` | 10 check groups | 66.2 |
| `reference` / archive, preview, continuation | `REFERENCE_FUTURE_V1_PASS` | Assertion total not emitted; all contract markers present | 93.0 |
| `interaction` / existing interaction | `SCENARIO_ONE_INTERACTION_VERIFY_PASS` | Assertion total not emitted | 87.6 |
| `habitat` / existing habitat presentation | `SCENARIO_HABITAT_PRESENTATION_VERIFY_PASS` | Assertion total not emitted | 57.6 |
| `completion-model1` / historical growth 1 | `SCENARIO_ONE_COMPLETION_VERIFY_PASS` | 12 phase PASS records | 83.5 |
| `completion-model2` / current growth 2 | `SCENARIO_ONE_COMPLETION_VERIFY_PASS` | 12 phase PASS records | 78.1 |
| `economy-model2` / ordinary economy integration | `SCENARIO_ONE_ECONOMY_INTEGRATION_VERIFY_PASS` | 126 assertions | 73.2 |

`final-gate-summary.json` verifies every successful receipt identifies the full candidate SHA, raw exit 0, no compiler/assertion failures and no unexpected tree change. The economy gate restored its exact original world and emitted `noSaveSlotWrites=true`. Negative paths include duplicate felling prevention, correct ledger/retained-material settlement, affordability, invalid targets, unmarketed legacy broadleaves and ten-year save/load continuation.

Compilation retained pre-existing Unity API deprecation/member-hiding warnings and package/import/platform diagnostics; this is not a claim of warning-free project compilation. No orphan-field warning about `ScenarioOne.asset` was observed, and its explicit load callback recorded none.

## Additional residual observations

The existing `BroadleafTimberEconomyVerification` passed unchanged in explicit Legacy40 and Enlarged80, **66 assertions per run**. It checked manager/batch targets 3.5/6 m, first resolution at year 1 and second at year 4, no third lift, exact saved crown bases/counts/last year, legacy models 0/1, identical Sitka revenue and generic harvest costs, species-specific products, pruning-based Oak grading, unchanged prices, quote re-resolution, single signed-ledger settlement and product-report/save validation.

The additive `Parity06ResidualVerification` passed **86 checks**, without weakening or replacing the original harness. It checked:

- exact Sitka price/revenue/job-cost equality across models 0/1/2, with broadleaf marketing off on 0/1;
- all three Sitka lifts and five-year recovery while the species-aware gate is enabled;
- genuine model-1 pending 2.5 m orders loaded on model 2: ordinary approval rejects the obsolete target, annual advance charges nothing and does not prune; old approved orders fail resolution without charging;
- old Oak/Beech one-/two-/three-lift histories, including former 2.5/5/6.5 m crown bases, exact last year and radius through save/load; a one-lift history proceeds to 6 m, two/three lifts stay capped;
- real Oak/Beech prefab refresh from Unpruned to LowPruned_Recent to MediumPruned_Recent, one active rendered model, immediate 8% radius loss per lift, and real annual ecology relaxation matching the retained `0.92² = 0.8464` crown factor. This is runtime component/prefab evidence in a headless Editor, not a rendered player-review or UI-wording claim.

`ScenarioOne.asset` loaded through `AssetDatabase.LoadAssetAtPath` with a warning/error callback: **zero warnings/errors**, non-null definition and unchanged bytes (`PARITY06_ASSET_LOAD_PASS warnings=0 bytesUnchanged=True`). Its historical orphan flat-value key remains in the asset, unconsumed by runtime code. No asset save was requested. Whole-run byte comparison is reported below.

## G3 completion/economy and attribution

The current **growth-model-2 / RNG-1 / regeneration-2 / storm-0 / Legacy40** completion witness passed all **12 phases**, completed at **year 25**, continued to **year 30**, and retained exact current-layout save/load identity:

- **`9C77EB4BEE427FB9`**, matching the retained model-2 completion fingerprint. No broadleaf-containing completion-anchor movement was observed relative to that retained receipt. This is not a newly run A/B comparison against parent `dc4f59b`.
- Minimum and year-30 cash: **613,978 cents (€6,139.78)**, never negative.
- Year 30: **199 living trees**, canopy **0.879**, **60 regeneration cells**, deadwood **19.436 m³**; second intervention **49 trees**, basal-area fraction **0.203**.
- The historical model-1 reconstruction separately completed at year 25, with minimum/year-30 cash **613,986 cents**, and reproduced the protected v18 projection **`702766DECE591E21`**. Its full current hash **`83096F04350D84F8`** is recorded, not substituted for the frozen projection.

The completion play plants Oak/Beech and checks their survival/recruitment, staged management, ordinary work, accounting, repeat load/continuation, objectives and solvency. It does not itself schedule broadleaf pruning; that surface is exercised by both 66-assertion timber runs and the residual gate. The separate economy integration result is in the gate table. These bounded witnesses do not assert premium-Oak attainment or certify a different future full-loop candidate.

Review N1 is retained: `ScenarioReferenceRunner` uses the legacy target overload for candidate selection. The model-2 manager rejects capped/ineligible broadleaves; frozen Reference runs use the original growth-model configuration. Review N3 is a deliberate generic cost consequence: with the actual definition's base 8 min + 2 min/m and 4,500 cents/hour, former lifts cost 975 + 1,350 + 1,575 = **3,900 cents / 52 min**; the two new broadleaf lifts cost 1,125 + 1,500 = **2,625 cents / 35 min**. That is **1,275 cents and 17 min less per fully pruned broadleaf**, with no species multiplier or price change. This calculation describes different management histories, not a pure-Sitka anchor change.

## Protected anchors and asset bytes

All values below were re-emitted/recomputed on the candidate; no expected value was changed:

| Protection | Result |
|---|---|
| Legacy growth-0 lifecycle | `BFC55473C1506067` / `3485B6630C9EA448`, exact |
| Regeneration-1 / growth-0 lifecycle | `962846D2F517B293` / `FBB8F470D85FF815`, exact |
| Growth-1 and model-2 pure-Sitka lifecycle | `7E57B9DAEF5BF4D0` / `35E2BF1C2F55C2DE`, exact |
| Live Sitka density component | 24/24 exact controller victim sets; 356 victims, including 13 boundary deaths; 3 hard checks PASS |
| Sitka 3,000-stem / 11,592-log fingerprint | `CAA0388524E0D0E5`, exact (integrated offline timber run) |
| Sitka 400-stem operation fingerprint | `2976F3575E0446B3`, exact assertion; sale 169,245,486 cm³ / residual 130,542,728 cm³ / 12 summaries |
| Unity Sitka timber | `TIMBER_YIELD_VERIFY_PASS fixtures=120 assertions=3007`; evidence digest `88FAF806EBE8FD5F2047740AE965823F0EE4286BE23EE5AD1B209CD2A228C4A8` |
| Legacy40 starting world | `5546B9B3B50673B2`, exact |
| Reference previews Y0/Y20/Y50/Y100 | `A564039D9B7CE31D` / `F7DF7DAB53B6FD32` / `D5E75D6D21D631AC` / `7AD177B3CC2F73C7`, exact |
| Reference schedule identity | `56C8B99FA1E8DDD1`, exact |
| Reference continuation | Deterministic; current diagnostic `EBD49ADEF18C3F36`, v15-layout diagnostic `BFFF66015A6619DA`, both retained; archive/preview/continuation contract PASS |
| Historical model-1 completion | Protected v18 projection `702766DECE591E21`, exact; 12 phases PASS |

Byte hashes, also compared directly to candidate Git bytes:

- `ScenarioOne.asset`: **`7703ab7b0f4a4e7ad4099780df52f25027d6f68e7997e9d0e3f92fa44f9a7583`**.
- Frozen `ScenarioOneReferenceFutureV1.bytes`: **`789e3eddcf00d3d4d0fd18d078f7ab37a06d44aa9c8672cff564445b2bb041fc`**.
- Frozen schedule JSON: **`91cf50595e9b3337438323ef09609a70d7de11374569dd9fb3c6359fbdb798f0`**.
- `SitkaGrowthModel.cs`: **`291601041876d83451259990f82219c6ccd3fb704ba55e6dcb8189f13b446463`**.

Reference continuation differs from the frozen future by the established diagnostic difference (494 current continuation trees versus 517 archived trees), not a changed archive or an invented new expected future. Both Reference and interaction gates independently reproduced the retained diagnostic fingerprints. The offline timber suite also passed **73 checks** on the integrated candidate; `integrated-offline-timber/stdout.txt` preserves its full output. Synthetic allocator defect fixtures are not a claim of new living-tree form biology.

## Exact reproduction

Run only in a clean, registered U1 at the exact candidate under a normal owner lock. For the additive gate use the committed tooling file from `bd3ce0e` or a byte-identical path; do not overlay production files.

```bash
python3 -I -B Tools/Verification/ThreeSpecies/Qualification/run_qualification.py \
  --label <LABEL> --out /media/jer/Files/CCF/Verification/<RUN>/<LABEL> \
  --method <CLASS>.Begin --file <SOURCE> --marker <PASS_TOKEN> \
  --env CCF_TEST_DIRECT_ENTRY=1 --env CCF_STAND_GEOMETRY=<0_OR_1> \
  --license-dir /home/jer/.var/app/com.unity.UnityHub/config/unity3d/Unity/licenses \
  --timeout 1200
```

The result table supplies labels, class/pass tokens and modes. Timber source is `Tools/Verification/ThreeSpecies/BroadleafTimberEconomyVerification.cs`; residual source is `Tools/Verification/ThreeSpecies/Parity06ResidualVerification.cs`. Both timber geometries were explicit. All protected gates use **Legacy40 / `CCF_STAND_GEOMETRY=0`**. Density additionally uses `--env CCF_QUAL_PHASES=ANCHORS,SITKACOMP`. Completion explicitly uses `--env CCF_GROWTH_MODEL=1` for historical reconstruction and `=2` for the current witness. Interaction and habitat already live in Assets; omit `--file` for them. Every other source is the existing committed verification file, not a patched temporary variant.

`run-protected.py`, `*-command.json`, per-gate `results-*.json`, full `.log` and `.markers.txt` under the evidence root retain the exact commands, HEADs, source hashes, modes, raw exits, terminal counts and diagnostics. Unity internally runs in an 8 GiB systemd scope with two job workers. Expected observable behaviour is the named terminal PASS, raw exit 0, no assertion/compiler error, preservation of the history/economy/visual state above and no leftover disposable scripts/metas. Gate counts that are not emitted are not invented.

## Cleanup, publication and remaining scope

After the final Editor exited, normal lock status showed no Editor PIDs. The sole tracked side effect was `.vscode/settings.json` (solution name changed to `CCF-U1.slnx`, plus trailing-comma normalization). Its exact bytes and binary Git patch were archived under `audit/` before restoring that one file to the candidate. This was restoration of the observed Editor side effect, not a routine shared-settings edit.

An independent content audit recomputed Git blob hashes for **all 5,260 tracked files** against the exact candidate: **zero differences**. It also checked the asset/archive/schedule/Sitka SHA-256 hashes above, no disposable harness or generated `.meta`, and clean Git status including untracked files. Receipt: `audit/final-lane-audit.json`. ZX20 free space at cleanup: **22,019,702,784 bytes (20.51 GiB)**.

U1 was left clean, detached at **`6abfb59e81d394e2667694446ac7e333e3488594`**. The lock was released **normally, `forced=false`**; `lock-after.json` read back **FREE, metadata null, Editor PIDs empty**. No unrelated worktree, Primary changes, user work, shared cache or real save slot was restored/cleaned. All test side effects stayed in the isolated run configurations; durable evidence was retained on Files.

The source/publication branch adds only the residual verification file and this Markdown handoff, with no diff under Assets, Packages or ProjectSettings. The pushed publication SHA and claim-comment URL are reported to the parent after publication. Claim #92 is left open. No PR was created or worked on.

Review N2 (player-facing Oak form/straightness wording) remains with PARITY-07. This qualifier does not accept UI truth, whole parity, the final combined Tier-3 player-test candidate, native input/performance or a Windows build. No G5/schema decision was needed. Any measured broadleaf-containing anchor movement remains subject to Manager G3 acceptance; this worker does not grant itself acceptance or integration authority.

**DASHBOARD SYNC: NOT AVAILABLE. PUBLISHED DASHBOARD REFLECTS THIS TASK: NOT VERIFIED.** No publisher/ingestion or audit/readback endpoint was available in the inspected task surface. The handoff and claim carry the actual qualification result. Manager-owned live coordination reconciliation and supported dashboard ingestion/audit/published-state readback remain an operational follow-up; no generated dashboard file or unrelated local coordination edit was touched.
