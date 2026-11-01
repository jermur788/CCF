# PARITY-09 — Independent Cross-Provider Review (claim #219)

**Verdict: APPROVED** (with one non-blocking verification-only follow-up recommended; see §6)

## 1. Identity and independence

- Reviewer: Claude Sonnet 5.5 (Anthropic), review-only sub-agent. The implementation/qualification worker was DeepSeek V4.1 Flash. I did not implement, qualify, or run any gate for this task.
- Candidate: `3acdf9cb635cdca79606335801e97f9d7dcce8b0`, which is an ancestor of `origin/integration/scenario-one-player-test-ready-v1`. It descends from `6784ab17` (MFM-09/MFM-10 lineage).
- Reviewed worktree: `/media/jer/ZX20/CCF-U1` at `aa8eb6ff`. The only change after `3acdf9cb` is the handoff file.
- **I did not edit production, tests, thresholds, baselines, the handoff or the coordination ledger. I did not self-accept the task. I did not run Unity, touch #194, or build for Windows.** The only file I wrote is this one.

## 2. What I inspected

- `HANDOFF.md`, `specs/.../checklists/requirements.md`, and the git diffs `6784ab17..3acdf9cb` and `3acdf9cb..aa8eb6ff`.
- Raw logs and marker files under `/media/jer/Files/CCF/Verification/PARITY-09-20261101/{offline,static,unity}`.
- `check_integrated_v1.py`, `run_pruning_check.py` (its `BASE=72dbee8b`), and the history harness csproj.
- A byte-diff of the pruning check's 17 protected files between its pinned base `72dbee8b` and `3acdf9cb`.

## 3. Candidate and delta (scope item 1) — confirmed

- `6784ab17..3acdf9cb` touches exactly four files:
  - `UI/Resources/ScenarioOneUi.uss`
  - `UI/ScenarioOneUiRoot.cs`
  - `UI/TreeInspectionView.cs`
  - `Tools/Verification/ScenarioOneCombined/CombinedHudComposition.cs` (verification fixture)
- A keyword scan of the added and removed lines for basal, growth, species, save, price, hash, density, prescription, calibration and version found nothing.
- Against the pruning check's pinned base `72dbee8b`, all of these are byte-identical at `3acdf9cb`:
  - `SitkaGrowthModel`, `MixedDensityModel(+Composite)`, `SitkaSubsetDensitySignal`, `ForestEcologyController`
  - the three species assets
  - `TimberYieldCalculator`, `TimberYieldEconomyAdapter`, `ScenarioOneEconomyAdapter`, `ScenarioOne.asset`
  - the Reference Future v1 `.bytes` and schedule JSON
- Only `ForestSaveData`, `ForestSaveController` and `ForestSaveValidation` moved, via accepted save work: S1-C v22, MFM-06 v23, D-066 v24, and PARITY-08 validation.
- The #199 delta is **UI-presentation only**. It contains no parity arithmetic, MFM prescription, coverage or calibration change, no species asset, save or schema change, no ecology or economy change, and no anchor change.

## 4. New Unity PASS evidence (scope item 4) — confirmed from raw markers

| Gate | Observed |
|---|---|
| Adult Legacy40 and Enlarged80 | `BROADLEAF_GROWTH_VERIFY_PASS checks=30` in both. Each marker file holds 30 PASS lines. Oak and Beech constants match 0.8/0.65/120/42.5/5 and 0.9/0.75/120/42.5/6.5, site scalar exactly 1.0, and the Sitka comparator is exact. |
| DensityQualification | `DENSITY_QUAL_PASS geometry=L40 hardChecks=5`. Anchors `7E57B9DAEF5BF4D0` / `35E2BF1C2F55C2DE` match under model 1 and model 2. Dense and moderate pure-Sitka 100-year runs have model 2 exactly equal to model 1. SITKACOMP is 24/24 identical victim sets, 356 victims, `legacyRoute=24`. |
| Completion model 2, Legacy40 | `SCENARIO_ONE_COMPLETION_VERIFY_PASS`. Twelve phase PASS records, P0–P11. Layout `9C77EB4BEE427FB9`. Completed year 25, cash 613,978, y30 living 199, canopy 0.879, regeneration 60 cells, deadwood 19.436. P0 reports save v24. |
| Reference Legacy40 | Archive, previews, continuation and negative control all PASS. Milestone hashes `A564…`, `F7DF…`, `D5E7…`, `7AD1…`, schedule `56C8B99FA1E8DDD1`, continuation `EBD49ADEF18C3F36`. |
| SaveHardening Legacy40 | `SAVE_HARDENING_VERIFY_PASS` |
| MixedDensity Legacy40 | Exactly one FAIL line, `POLICY_SAVE_VERSION_UNCHANGED saveVersion=24`. Every behavioural check passes: Reference/Beech/Oak classes, Sitka < Beech < Oak demand order, new-game growth model 2, pure-Sitka model 2 equals model 1, route switching, overcrowding and self-thinning, deadwood recorded once. |
| PARITY-07 rendered 1031/0/36 | **Not in the evidence root and not re-inspected by me.** It is reused from the #199 review at the same candidate. The delta is UI-only and that review is bound to `3acdf9cb`, so reuse is acceptable. The raw proof lives in the #199 review artifacts, not here. |

## 5. Classification of every red item (scope item 3)

| Red item | Classification | Basis |
|---|---|---|
| `check_integrated_v1` stale `ForestSaveData.cs` normalized hash | **Verification debt** | The log shows an assertion on the first contract file. `ForestSaveData` legitimately moved (v24). It aborts before the 360 kernel checkpoint comparisons and the species-asset checks. Those protected inputs are unchanged (§3), and the Unity adult 30/30 run covers the same constants live. |
| `SiteInfrastructure` `G_SAVE_VERSION_IS_STILL_23` | **Verification debt** | 63 checks, 1 failure, and it is exactly this pin. `CurrentVersion = 24`. |
| `run_pruning_check` `protected bytes moved: ForestSaveData.cs` | **Verification debt** | It aborts on the first moved save file. The remaining 14 protected files I diffed myself are byte-identical. Pruning logic and species assets are unchanged. |
| `MixedDensity` `POLICY_SAVE_VERSION_UNCHANGED saveVersion=24` | **Verification debt** | It is the only FAIL, and all density behaviour checks pass. |
| `BroadleafTimberEconomyVerification` abort at `CurrentVersion == 23` | **Verification debt, with coverage gap** | It aborts at its first check, so no timber assertion ran in Unity on this candidate. See §6. |
| ThreeSpecies density `MaxRemovedBasalShare` | **Scanner false positive** | The identifier is the MFM rack/prescription canopy-keep removal-share cap (0.40) in `ScenarioOneRackBay.cs`, `ScenarioOnePrescription.cs` and `ScenarioOneTeachingCap.cs`. It is not a species maximum density or maximum basal area. |
| Three pure-Beech R2 misses (0.423 / 0.407 / 0.417 vs 0.40) | **Accepted limitation** | Logged, with the 0.40 threshold unchanged and the failures preserved. The negative control passes. R1, R3 and R4 hold in all three. This follows the earlier Manager adjudication, which I did not re-derive. |
| ThreeSpecies history `WorkItemDetail` build failure | **Verification debt (harness compile list)** | `WorkItemDetail` is defined in `ScenarioManagementHistory.cs`. The harness csproj lists `ManagementHistoryText.cs` but omits that file. The suite is S1-C history, not parity. The claim that it is identical on the accepted base is plausible, but I did not re-run it there. |
| guided-v2, 11 identical failures | **Pre-existing, accepted with the #199 review** | Not rerun by me, and no log is in this evidence root. This is a residual outside parity scope. |
| First completion run (`completion-model2`) failed with `start is not 336 living Sitka` | **Harness precondition, not a defect** | The geometry was not Legacy40, and the harness expects the 336-tree start. The Legacy40 rerun passed. The handoff does not mention the failed attempt. This is a minor disclosure gap, not an overclaim. |

I found **no new defect** and no production or spec problem that should block PARITY-09.

## 6. Checklist coverage (scope items 2 and 5)

The handoff map covers every applicable checklist section: density/pure-Sitka, adult/site/canopy, regeneration/browse, pruning, timber/economics, player-facing truth, save/compatibility, anchors, evidence vocabulary, the MFM guard, and integrated acceptance. The map is honest about which rows lean on accepted-ancestor evidence. Findings:

1. **Pruning and timber Unity coverage on `3acdf9cb` is indirect.** Unity PARITY-06 timber and pruning assertions (66 per geometry) were not rerun, because the gates abort on the v23 pin. Support comes from three sources:
   - the accepted PARITY-06 evidence;
   - the offline `THREE_SPECIES_TIMBER_PASS checks=73` on this candidate;
   - byte-identical pruning, timber, economy and species sources (§3), plus the UI-only #199 delta.

   I judge this sufficient to support the checklist. It is weaker than a fresh Unity run.
2. **Rows depending on reuse or unrerun gates:** the 720p/1080p rendered row (reused PARITY-07 1031/0/36) and the pruning-lift rows. Neither contradicts anything I saw.
3. **Pending by design:** "Independent reviewer confirms spec conformance" is satisfied by this review. "G2 acceptance", "G4 integration/canonical review" and "G3 where applicable" belong to Manager. G5 is not triggered, because there was no unexpected persistence or schema change.
4. **Handoff §4 wording:** `ScenarioOneCompletion` is listed as "model 2, Legacy40". That is accurate, but Enlarged80 completion was not shown.

## 7. Do the stale pins block the checklist? (scope item 5)

No. Every checklist row is supported by at least one of: a PASS on this exact candidate, a protected-file byte-identity check, or accepted ancestor evidence on an unchanged ancestry. None of the five stale pins hides a fact that the other evidence leaves unsupported. The residual weakness is that no Unity timber assertion ran on `3acdf9cb`.

Recommended **non-blocking** verification-only follow-up (no production change): repin the five v23 assertions to accept v24, and add `ScenarioManagementHistory.cs` to the history csproj compile list. Then rerun `BroadleafTimberEconomyVerification` Legacy40 and Enlarged80, `MixedDensity` and `run_pruning_check` once. If the Manager wants Unity timber and pruning evidence on the final candidate before the token, that repin is the smallest bounded action.

## 8. Cleanup (scope item 6)

- U1 `git status` is clean at `aa8eb6ff` (`3acdf9cb` plus the handoff commit).
- No Unity or Editor process is running. A broad `pgrep -i unity` found only my own shell.
- No harness `.cs` or `.meta` is staged in the tree. The extra `*Verification.cs` files under `Assets/ForestPrototype/` are tracked and `git status` is clean.
- I could not read the machine-wide Unity lock state directly. I am relying on the handoff and the absence of an Editor.

## 9. Token supportability

`THREE-SPECIES PARITY P0 VERIFIED` is **supportable once the Manager accepts**. This review recommends acceptance, with the §7 repin recorded as accepted verification debt or run first at the Manager's choice. This review does not issue the token.
