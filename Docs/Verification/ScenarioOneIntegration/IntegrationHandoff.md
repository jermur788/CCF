# Scenario One integration candidate — construction handoff

Branch: `integration/scenario-one-final-candidate-v1`. Base: `c1564b857972ee18917e8a461c457533a54cc3f1`. Authority/context: `c6dc136b7f1f8668d9f67f95f48968d290ad5067`. Checked code head: `930d920cc6126f31441edf3633c38b6008ce18af`; the following evidence-only commit supplies the final candidate HEAD, read with `git rev-parse HEAD` / remote readback. Source authority is the current Manager packet plus A5 `60394d766c7722fded762571baa4058ed4d34828`. This candidate is **constructed, not Unity-qualified**. Only this one integration candidate branch is pushed; the separately requested stable marking publication branch is also pushed. No main merge, canonical publication, Windows build, Unity execution, seed campaign or final-anchor adjudication occurred.

**Tier-1 status is not all green:** five reduced-simulator R2 checks fail with the combined calibrated species rows. No threshold was relaxed and no production value was tuned to remove them. Source ownership and all other listed current construction checks pass. Queue B receives this exact candidate and the unresolved diagnostic risk, not a claim of integrated acceptance.

## Task 1 — marking publication

- Stable branch `perf/scenario1-marking-optimization-v1`, remote HEAD / exact original production commit `678283642aaeb60221eda0fe8cb3f97e27958048`.
- Original parent `130bc36bad3cf09c42092bc176e61ddf645a1ad6`.
- Original containing branch `perf/scenario1-dense-stand-player-v1`, local head `e788b09f97958fd0d416cf4f67d63e7ef599df77`; source worktree clean.
- Exact patch inspected: only `Assets/ForestPrototype/ForestTreeMarkingManager.cs`, 31 additions/9 deletions; cached positions/diameters/marks, wider squared-distance prefilter followed by identical cutoff, Hegyi additions/order retained. No amend/rebase/patch rewrite. Remote HEAD equals the exact original commit, proving ancestry. Production file in the candidate is byte-identical to that accepted source.

## Accepted input provenance / actual order

1. Density-owned 12 production paths imported exactly from `90e3496873da4e3b23038993ca5bd2fec3c2e7f1`, without its inherited timber/history branch. Accepted B evidence `9c736dc52717302b39faf61c75da958c81d8c1af`; evidence-only later commits have no production delta. Density constants Lmax 0.50 / k 1.60 / m 2 retained.
2. Site exclusive commits `435f55c0f4054241a74c97ff0dbf380897bf1691` → `fdbe64fe021acd0cde36cc4b197c3b40b7a81478` → `e49b1955f6eab990d4b1ba7e6e7811e152629037` → `47539847d7e2531824c30f3dfe635d55404699a8`.
3. Timber/economy/history `8d21e27046fdeae4b2cda710938daf19757fbc3d` → `855827f96225003c6387b0da43107c509603391a` → `feb754cbe263c41a8d2f6fe33cd9fc39888c7e60`.
4. Growth production delta `ea52050ea62206a13b36a100bd03ec9d8488da39`, final source `796da99029190ef71e68d5fa722f25240e696318`. New evidence `a0c21e0743212ab270543bcd38549334ff7a0c97` is source qualification only; its production diff from 796da99 is empty.
5. N3 `eb7fd3d8c0baac1d71d43ebfce035591b9e4a8ac`.
6. A1 guidance `a93a7220fd8b4d8246b3613d913360dad06988e8`; correct source smoke `899709f5f968d64cebed3c961b8596e3a2f245a3` (production unchanged from A1), not the older ba48591 smoke.
7. Original UX production commits `9a865db4b3ae35b84f4a2ed97486266b90450e37`, `c89cebcf9863a3ee6ff501ffcef6a40e1fe99e06`, `7203fee21eaae06500cb031bae261efed9dca925`, `9b01a7baf83ca9878dedc38c5305b98d9d08b759`, `58e7a67c3c96288e54df513e280c1fa28c17b8df`, `51e240f4a7998bbb2758f0f5f081d13e3751794b`; follow-up production `5e4d66eabf6005dbeb0f2cb75594b0c514c01c73` from complete accepted source `549eae3318f1bb4d01a2853384b58f282f7f4e73`.
8. Marking production `678283642aaeb60221eda0fe8cb3f97e27958048` only.
9. Disposable tooling foundation from qualified density 90e3496, preserve A1 history tests, UX harness and explicit P3 expectation from 549eae3, U4/A1 smoke harness from 899709f; then fixture delta `4c5a8a4b80e2e1df4d343a2aebc21475d6f1401a`. Source-specific guards adapted explicitly as below.

No A3/A4/A5/A6 branch was merged. Only feature handoffs/docs from the selected source commits, narrowly required density source documentation and this construction evidence were carried. Canonical Docs/Project remains untouched; the Manager's c6 context remains authority, not a new publication claim about this main-based checkout.

## Conflicts and semantic resolutions

- **Site/timber offline runner:** Git inferred a site-runner rename from the omitted tool ancestry, despite disabled directory-rename handling. Retained site runner exactly from site HEAD and installed timber `ThreeSpecies/OfflineLogic/run_offline_check.py` in its actual namespace. No runner combined or production conflict hidden.
- **Economy README:** modify/delete due omitted ancestor docs. Retained exact feature source README as supporting evidence only.
- **Broadleaf growth fixture:** v3 had an R-prefix recruit cleanup; A2 has explicit nine-fixture-ID Step control for every annual loop. Reviewed both; retained the stronger A2 identity-based control, all original comparisons/thresholds, no production change.
- **MixedDensityVerification:** v3 already had model1/model2 measured equality and geometry-aware v16 handling. Retained its component/Generic assertions outside the overlap; applied A2 explicit per-geometry table, independent measured equality, no unknown-geometry fallback and v16 refusal/session-growth check. No wholesale older gate replacement. Enlarged80 lifecycle table remains labelled observed, not Manager-accepted replacement anchors.
- **ForestEcologyController and density/species definitions:** exact accepted v3 bytes. Broadleaf assets exactly equal growth source 796da99, including preserved density demand fields. No density/growth textual conflict in the real density-first order.
- **ScenarioOneManager:** no textual conflict; independently reconstructed as a clean three-way merge of N3 eb7fd3d and UX 549eae3 against b4894cc, and compared byte-for-byte. Economic gating, resolved work/history and N3 event/completion boundaries coexist with UX flows. New game still uses SiteClassDensity (1).
- **AnnualReviewView:** independently reconstructed as A1 a93a722 + UX 549eae3 against b4894cc; exact match. Footer visibility/derived wording and historical drill-down preserved; default first-review prompt comes from accepted MenuHelpView.
- **History, timber/economy, remaining UI, marking:** production ownership check requires each changed file to match an accepted source exactly; the two manager/review compositions above are the only composed files. All 59 changed production files pass. No mechanic resolved by branch preference.

## Tier-1 results

| check | result / limits |
|---|---|
| Accepted source composition | PASS, 59 production files; two independently composed files; save/objectives/default/Sitka/canonical invariants preserved |
| Approximate production type-check | PASS zero new errors, 7 existing reference-surface errors |
| Approximate tool type-check | PASS zero new errors, 13 remaining baseline errors (baseline 43); compile-only API stubs extended for accepted UX input/reflection signatures, baseline not updated. This is not a Unity compile. |
| Core density suite | PASS 91 checks, legacy-route fingerprints/module-source protections |
| Composite density suite | **FAIL 5/61 checks**, only reduced-simulator R2; exact identities/continuity/allocation and all other checks pass; mixed replay/pure-Sitka equivalence pass |
| Density isolation | PASS both arithmetic-module isolation compiles |
| Timber | PASS 71 |
| History | PASS 74 |
| N3/guidance | PASS 88, actual protected rules hash, enum/chronology negatives, boundary round trips and legacy terminal behaviour |
| UX offline | PASS 176; no rendered/input claim |
| Growth integrated guard + frozen lab checkpoints | PASS 360 checkpoints / 7 exact source contracts, chosen parameters/release ordering/height independence/negative controls unchanged |
| Fixture controls | PASS 9 extracted-control checks plus 7 static contract groups |
| Site | PASS 63, all 1250 profiles neutral, isolated compile |
| git diff --check | PASS across candidate delta; no scene/prefab/material/package/settings drift |

Commands are the existing runners under Tools/Verification/CloudTypeCheck, ThreeSpecies/OfflineLogic, ScenarioOneCompletion/N3, ScenarioOneUx, SiteInfrastructure and Calibration/OakBeechGrowth; all output directories were external. Reproduce ownership with `python3 Tools/Verification/ScenarioOneIntegration/check_sources.py --out <external-dir>`, growth with `python3 Tools/Calibration/OakBeechGrowth/check_integrated_v1.py --out <external.json>`. Full receipts/logs are in ConstructionEvidence (published log copies have trailing whitespace trimmed only; raw logs remain in the external construction directory); source/object hashes are in ConstructionManifest.json. Heavy .NET build outputs remain outside the repo.

## Explicit source-specific tooling adaptations

The original `check_production_v1.py` is retained unchanged and correctly fails on its b489-pinned controller hash after density integration. The separate `check_integrated_v1.py` keeps exactly the same chosen values, [C] checks, kernel/source expression guards and 360 frozen numerical expectations; its explicit integrated_source_contract pins density-owned files to 90e3496 and unchanged Sitka asset/objectives/save bytes to the production base. No numeric expectation or historical anchor was replaced.

The density reduced simulator initially failed its asset guards because its copied .4/.4 / Ci50 3 rows no longer matched the accepted species assets. Its two broadleaf input rows now exactly match 796da99 (.8 / Ci50 5 Oak; .9 / Ci50 6.5 Beech); Sitka stays unchanged. Asset equality remains enforced. Thresholds, production calibration and external recruitment series are unchanged. Historical source-v3 screening is reproduced at 90e3496, not under the newly combined growth inputs. Diagnostic output now prints the individual failed rule, maximum single-year loss and minimum BA ratio; this changes no predicate. Compile-only InputSystem/Editor stubs add signatures for the accepted UX harness instead of excluding it or refreshing the baseline.

## New reduced-model R2 risk — not waived

| fixture | failed quantity | observed | unchanged limit |
|---|---|---|---|
| Legacy40 dense Beech | maximum annual all-stem loss after Y25 | 0.423 | 0.40 |
| Legacy40 young Oak | maximum adjacent-five-year loss-fraction change | 0.106 | 0.10 |
| Legacy40 young Beech | maximum annual all-stem loss after Y25 | 0.407 | 0.40 |
| Enlarged80 dense Beech | maximum annual all-stem loss after Y25 | 0.421 | 0.40 |
| Enlarged80 young Beech | maximum annual all-stem loss after Y25 | 0.417 | 0.40 |

In all five, R1 flattening, R3 dead-BA/big-stem protection and R4 BA retention pass. All mixed checks and pure-Sitka identities pass. These are deterministic **reduced-model** screening results using an external recruitment series measured under an earlier runtime lineage; they do not establish current Unity behaviour. They are additional to the Manager-accepted dense-Beech five-year variability caveat and must not be silently included in that acceptance. No fresh search, extra seeds, production retuning or threshold waiver was performed. Queue B must receive this open risk with the candidate; a green integrated density verdict is not claimed here.

## Preserved contracts / accepted caveats

- Save20 unchanged; event0–8 retained, review event9; current year/monotonic IDs, no fabricated active-legacy chronology. Completed and Failed stay terminal. Eight objective IDs/conditions and definition source bytes unchanged. N3 requires matching genuinely resolved work in both distinct-year plans and acknowledgement strictly between their event boundaries. Second intervention remains non-prescriptive.
- Frozen Reference Future v1 files and pure-Sitka contract untouched; no DEC, DMC, mixture-productivity bonus, numerical site activation or default-model change. No final expected completion/live-continuation hash established. Existing observed source hash movements remain evidence only; final adjudication deferred.
- P3 presentation expectation **AF7405918195F0DE** carried from accepted UX, including its original tooling expectation change; no new simulation anchor edit.
- Density source B accepted by Manager: dense Beech can marginally exceed the self-registered five-year all-stem variability diagnostic; known [C] calibration limitation, not empirical validation. New reduced R2 failures above remain separately open.
- Growth remains Kernel A, weakly identifiable ordinal [C], no empirical coefficient claim. Existing saves adopt current species rows; juvenile promotion/lab-stock caveats retained from source qualification.
- U4 states 1 and 3 place guidance at the bottom of scrollable Annual Review; state2 has an immediate footer. Accepted for integration; verify actual placement at 1280×720 in Queue B.
- 1280×720 minimum external demo, 1024×768 compatibility-only. No rendered/input/Player/native-Windows verification by this worker.

## Files / unexpected changes / next

59 production files (full paths + exact owner hashes in accepted-production-composition.json): density/model/species definitions/assets, dormant site types/metas, timber/economy, history/N3/guidance, accepted UX partials/layout and marking. Remaining changes are bounded verification tools/compile-only stubs, selected feature documentation and this evidence; ConstructionManifest lists every committed pre-handoff file. No unexplained production change, no canonical implementation-status update, no settings/package/scene/prefab/material drift. The new reduced-model R2 failures are the unexpected verification finding.

**NEXT:** Queue B Tier-2 integrated Unity smoke on the exact pushed HEAD, including acknowledged guidance placement, source/runtime parameter equivalence and explicit triage of the reduced R2 risk. This handoff neither waives that risk nor authorises full campaigns, final anchor adjudication, Windows builds or main publication. Stop this construction worker after push/readback.
