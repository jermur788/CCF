# PARITY-04-PATCH — Adult Species Docs / Verification Alignment — Handoff

**Task:** `PARITY-04-PATCH` (bounded adult species docs / verification patch)
**Spec:** `specs/scenario-one-three-species-parity-p0/` (APPROVED)
**Claim:** GitHub issue #17 (`jermur788/CCF`)
**Date:** 2026-10-27
**Worker / routing:** Claude Sonnet 5.5 (`claude-sonnet-5-5`, Anthropic) via Claude Code in T3 Code. This is **not** the packet's preferred Claude Opus (the packet accepts Sonnet if the actual model is recorded) and it is a **local** session, not Claude Cloud.
**Worktree:** `/media/jer/ZX20/CCF-Parity04Patch` (new, clean; the dirty Primary checkout `/media/jer/ZX20/CCF-Primary` was not touched; no Unity `Library`)
**Branch:** `feature/scenario-one-three-species-parity-adult-docverify-v1`
**Unity:** **NOT RUN. Lock not acquired.** **Windows build:** none.

## Verdict

**`PARITY-04 BOUNDED PATCH IMPLEMENTED — READY FOR UNITY QUALIFICATION`**

This implements the bounded patch only. It does **not** claim `ADULT SPECIES PARITY REQUALIFIED`: that still needs PARITY-03 Unity qualification (#10) and a later focused Unity run of the strengthened adult gate (§8). The strengthened gate itself has **not been run in Unity**; it is type-checked only (§6).

- **Zero** species asset, scene, prefab, material, package, project-setting, save-field, production-arithmetic or biological-value change. The one production `.cs` file touched differs in comment lines only; its executable code is identical by the repo's own normalizer (§7).
- The Manager-authorised contract refresh touched **exactly** the three preflight-identified files, no fourth mismatch existed, and the **unchanged** gate now passes: `INTEGRATED_CALIBRATION_V1_PASS checkpoints=360 protected_files=7` (§4, §6).
- No protected fingerprint or expected value was changed (§7).

---

## 1. Identity

| Item | SHA |
|---|---|
| Starting HEAD (= `origin/main` at task start) | `0a313fad1e39d1c13487f5e71aeee5ee234c53a9` — descends from Rack V2 integration `f23549b`; includes the preflight Manager review (`a4ab1f3`) and the FR-3 clarification (`636f8f4`) |
| **Patch HEAD (tested)** | `17aa738c58b474476ce9c6d1f6163d42c63ae5d5` |
| Final HEAD | the commit that adds this handoff above the patch HEAD (documentation-only; read back from the remote after push) |
| `origin/main` at publication | `c9ab5a6bc8f0286b72cd9911434ebcdc3744f8c0` — moved during the task (S1-C T-02 integration, RACK-V2-04A / MFM-02 packets, then a docs-only MFM-01 canonicalization). **No overlap** with the seven changed files; `git merge-tree` reports a clean merge. The refreshed contract was re-checked against all seven pinned sources on `5bb9da5` (the last head with production changes since the base; `c9ab5a6` adds one handoff document only), and they match (§4). No rebase was needed or done |

Live coordination was refreshed from `origin/main` before claiming (row `PARITY-04-PATCH` lists this task as owner of the surface; no other live row owns the allowed files). `Docs/Project/coordination/active-tasks.md` was **not** edited.

## 2. Changed files (7; all on the packet's allowed list)

| File | Kind | Diff |
|---|---|---|
| `Docs/Project/unity-project-overview.md` | current-state documentation (Manager-owned canonical; M-2 assigned it to this patch) | +/− edit at the stale line, one pointer sentence, one new section |
| `Docs/BeechSpeciesParameters.md` | current-state documentation | rewritten adult rows and site text |
| `Assets/ForestPrototype/Docs/SessileOakV1.md` | current-state documentation (markdown under `Assets/`; `.meta` untouched) | stale adult bullet replaced, new section |
| `Tools/Verification/ThreeSpecies/BroadleafAdultGrowthVerification.cs` | verification tooling | +229 / −5 (the five removed lines are stale header/report text; no assertion removed) |
| `Tools/Calibration/OakBeechGrowth/integrated_source_contract.json` | verification contract (Manager-authorised G3 change) | 6 lines: 3 hashes + 3 `source` values |
| `Tools/Verification/SiteInfrastructure/run_site_check.py` | docstring only | save-version text |
| `Assets/ForestPrototype/ScenarioSiteProfiles.cs` | **comment only** | save-version text |

Not changed (deliberately): `check_integrated_v1.py` (unnecessary; no reporting change needed), `Docs/EcologyCalibrationAdoption.md` (a dated 2026-10-02 adoption record; not on the allowed list; the overview now states it is historical), all species assets, scenes, prefabs, `TreeSpeciesDefinition.cs`, `ForestEcologyController.cs`, `ForestSaveData.cs`.

## 3. Documentation corrections (before → after)

**`Docs/Project/unity-project-overview.md`**

- Line 135 (inside the *dated* 2026-10-02 "Integrated ecology package" section).
  *Before:* "…The competition equation is unchanged. Beech/Oak keep Ci50 3 and 0.4 cm/yr."
  *After:* the same sentence frame, then "At this 2026-10-02 integration Beech/Oak still carried their initial Ci50 3 and 0.4 cm/yr; that is history and is **not current**. The current accepted adult calibration is Oak Pd 0.8 cm/yr, Ph 0.65 m/yr, Dmax 120 cm, Hmax 42.5 m, Ci50 5 and Beech Pd 0.9 cm/yr, Ph 0.75 m/yr, Dmax 120 cm, Hmax 42.5 m, Ci50 6.5, all `GAMEPLAY CALIBRATION` (not empirical Irish broadleaf growth constants)…". The historical context is kept and labelled.
- The dated 2026-10-12 species-parity audit section is **left as history**; one sentence now says its facts are dated at `c1564b8` and points to the current state.
- **New section "Current Oak/Beech adult model (PARITY-04 documentation alignment, 2026-10-27)":** the constants table (Oak, Beech, Sitka protected comparator), evidence class (`GAMEPLAY CALIBRATION`; class weights calibration; ordering `ORDINAL REPRESENTATION`, Oak > Beech weak/provisional), the shared adult growth law, neutral/dormant site response, shared canopy/light, shared 8 m Hegyi, `Ci50` roles scoped to growth model 2 per M-4, the accepted Beech height-band miss, and an explicit "static documentation alignment, not Unity qualification" status line. No mirror or attachment was regenerated.

**`Docs/BeechSpeciesParameters.md`** — the adult rows were Pd 0.40, Ph 0.40, **Hmax 40 tagged A/B** ("typical mature height"), Ci50 3.0.
Now: Pd 0.9, Dmax 120, Ph 0.75, Hmax 42.5, Ci50 6.5, each `GAMEPLAY CALIBRATION` (search-grid centre, candidate Beech-122); the superseded values and the A/B tag are stated as history; a growing-space-class row; older-tag rows (crown, reproduction, dispersal, juvenile light) kept unchanged and marked "older tag". New sections: shared adult growth law, **Accepted height-band miss** (Beech 23.37 / 30.24 / 34.65 m at 50 / 75 / 100; 1.63 m and 0.35 m below the provisional 25–30 and 35–40 m bands; accepted limitation per PARITY-02 U-11; do not retune), and *Site, canopy and competition (current)*. The old line "drought and waterlogging should both be strong penalties" is replaced: the direction exists only as dormant ordinal data, **no drought or waterlogging penalty is modelled**, and a neutral multiplier does not make waterlogged sites favourable.

**`Assets/ForestPrototype/Docs/SessileOakV1.md`** — *before:* "adult potential is 40 m height, 120 cm soft DBH scale, and 0.4 cm/year favourable DBH increment". *After:* that bullet defers to a new *Adult growth (current)* section: Pd 0.8, Ph 0.65, Dmax 120, Hmax 42.5, Ci50 5, class High (weight 2.0), `GAMEPLAY CALIBRATION` (candidate Oak-122), shared law, neutral site, shared canopy/competition, heights 21.45 / 28.18 / 32.76 m at 50 / 75 / 100 (inside the provisional bands at 50 and 100). The still-current juvenile, reproduction, mast, dispersal and crown values were not touched.

**Comment-only:** `ScenarioSiteProfiles.cs:5-6` ("version stays 20") now says the file does not change the save version (21 on the current base); `run_site_check.py:15` ("still 20") now says 21.

## 4. Source-contract refresh (Manager-authorised, attributed)

**Steps 1–3, completed before any edit to the contract** (evidence: `post/contract-drift-attribution.txt`):

1. Normalized hashes recomputed on the exact patch base `0a313fa` with the repo's own `Tools/Verification/source_normalization.py` (a sanity check confirmed my per-commit snapshots normalize identically to in-place files).
2. Mismatch set is **exactly** the three preflight files: `ForestEcologyController.cs`, `TreeSpeciesDefinition.cs`, `ForestSaveData.cs`. The other four pinned files match. **No fourth mismatch.**
3. Each drift was re-attributed by computing the normalized hash at every commit that touched the file since `8f9ed80` (where all seven matched), and the function bodies were compared.

| File | Old normalized SHA-256 | New normalized SHA-256 | Old → new `source` | Introduced by |
|---|---|---|---|---|
| `ForestEcologyController.cs` | `fdcfec2a478f6a046d3956a051dee16429080df18d201a28a268b78a28c627c4` | `ea1a204324f3c39972af1d6b6b69d747768f6c81eafe2a7c9bed497af95a8639` | `90e3496…` → `4fc05e11bec5e00cd4dc9b32e380ba1babcf44cf` | four commits, each flipping the hash: `ac4f34c`, `95e582e`, `fadc0c7`, `4fc05e1` |
| `TreeSpeciesDefinition.cs` | `63c9078e7be818d4594978e1377485b1b1648ecb48dedd445766e58e6a011bfd` | `346647ef4b3f4b5cb5a75f7392b2ab6c1a6c58f8e9cd9a1917403dd9b660531a` | `90e3496…` → `ac4f34c415daa2dfc05539bac30b092e682eb10a` | `ac4f34c` |
| `ForestSaveData.cs` | `224173ce7b7fd0eae426c4a18fe037f37a5cbef39a38df163ad47efd059f24c7` | `4734c9d733bf8f8453102b79f96ad3bb8aad5b037140ed0b7e6b65e502c20d73` | `c1564b8…` → `cf84d24c684b4317f5493f9e06d6a65df16e9fb2` | `cf84d24` |

(Consistent with the preflight's attribution. The per-commit run shows `95e582e` flips the controller hash as well, because it restores `GrowAdults`' original local layout after `ac4f34c` restructured it.)

**Why this is adult-arithmetic-neutral (step 4):**

- **Controller.** Net of the four commits against `8f9ed80`, `UpdateCompetition`, `RelaxCrowns`, `RecomputeCanopy`, `HegyiCutoffMeters` and `GetSiteProductivity` are **identical** (comments/blank lines ignored). `GrowAdults` differs by exactly two inserted lines: `if (treeSpecies.HasGrowthAgePhases) ApplyGrowthAgeFactors(…)`, which is skipped for Sitka, Oak and Beech because none of their assets serializes `growthAgePhases` (empty table → `HasPhases` false). The remaining controller deltas are the forest-track stand approach (`fadc0c7`, `4fc05e1`) and the `DispersesSeedNaturally` guard in `ComputeSpeciesSeedRain` (`ac4f34c`; regeneration, outside the adult path).
- **`TreeSpeciesDefinition.cs`.** The only *modified* (non-added) line is the `growingSpaceDemand` tooltip text; everything else is added Scenario Two configuration members. All adult accessors and the serialized declarations (with defaults) of the five adult fields, `growingSpaceDemand` and the crown fields are identical.
- **`ForestSaveData.cs`.** The only *modified* line is `CurrentVersion` 20 → 21; the additions are Rack V2 plan fields. No species, growth, site, canopy or Hegyi field was added.

The refresh changed **only** `normalized_sha256` and `source` of these three entries (file set, keys and the other four entries identical). After it, against the **current** `origin/main` (`5bb9da5`) all seven pinned sources still match, so the contract will not re-drift on merge.

## 5. Adult verification strengthening (`BroadleafAdultGrowthVerification`)

Strictly additive around the lines `check_fixture_controls.py` pins: `Step()`, the single `e.AdvanceOneYear();`, the `fixtureIds` initializer and the competition/law assertions are untouched; `Grid` and `Law` are untouched. Fourteen new check executions per run (§8 gives the expected total):

| Packet item | Implementation (all from the live loaded assets / running ecology) |
|---|---|
| 1. Accepted constants | `{sitka-spruce, sessile-oak, beech}_ACCEPTED_ADULT_CONSTANTS_EXACT`: exact Pd, Ph, Dmax, Hmax, Ci50, growing-space class and **no age-phase table**. Sitka included as the protected comparator. Labelled `GAMEPLAY CALIBRATION`, not empirical |
| 2. Site neutrality | `SITE_SCALAR_EXACTLY_1_*` (3 executions: scene start and after each species' fixture runs): every ecology cell's `SiteProductivity == 1f` **and** `GetSiteProductivity` at every cell centre `== 1f`. See the deviation note below |
| 3. Canopy species-neutral | `CANOPY_LIGHT_IDENTICAL_ACROSS_SPECIES_FOR_IDENTICAL_GEOMETRY`: same ids, positions, DBH, height and crown for Sitka/Oak/Beech → canopy and light of every cell compared **bit for bit**; `CANOPY_PROBE_IS_SENSITIVE_TO_CROWN_GEOMETRY` (a +1 m crown must change the result, so the identity is not vacuous); `CROWN_GEOMETRY_REPORT` (informational: the only permitted species differences) |
| 4. Hegyi | `HEGYI_CUTOFF_EXACTLY_8_M`; all 9 target×neighbour species pairs at 7.99 m (counted, equal to `HegyiTerm`, bit-identical across pairs) and 8.01 m (exactly 0) |
| 5. Height diagnostics | `ADULT_HEIGHT_DIAGNOSTIC` log lines for Oak and Beech at ages 50 / 75 / 100 from a promoted 3.5 m stem (age 10), each with the provisional band and, for Beech below band, `ACCEPTED_PROVISIONAL_HEIGHT_BAND_MISS (PARITY-02 U-11)`. **Not pass/fail targets.** The only assertion is `*_HEIGHT_DIAGNOSTIC_MATCHES_SPECIES_LAW` (the printed engine value equals the restated law, tolerance 1e-3 as in the existing law check) |

Existing assertions are all kept (no coverage reduced). `OAK_BEECH_ADULT_PARAMETERS_REPORT` still reports (now both species' values; its stale "share every parameter" text is gone) and is complemented by the new assertion.

**Deviation to flag (item 2, seam wording).** The packet asks that Scenario One site response "resolves to exactly 1.0 for Sitka/Oak/Beech at the reference site". The harness asserts the *effective* site response growth actually uses (the live scalar above). It does **not** call the dormant species-site seam, because `run_site_check.py` contains `H_NO_OTHER_VERIFICATION_TOOL_REFERENCES_THE_NEW_TYPES`, which fails if any other `Tools/Verification` file mentions those type names **anywhere in its raw text, comments and strings included**. Referencing the seam from this harness would therefore turn that check red, and building the names from fragments to slip past the scan would defeat the guard, which I did not do. The seam's own assertions already cover the requirement and were run on the patch HEAD: `C_EVERY_SPECIES_SITE_COMBINATION_RETURNS_EXACTLY_1` (8,750 combinations) and `E_SCENARIO_ONE_SITE_RESOLVES_TO_1_FOR_EVERY_SPECIES`. The harness header says this in words that avoid the guarded names. If the Manager wants the Unity gate to call the seam as well, that needs the harness allow-listed in `SiteInfrastructureOfflineCheck.cs`, which is outside this packet's allowed files.

## 6. Non-Unity verification, exact commands and results

Run from the worktree. `python3 -I -B` for the runners; `python3 -B` for the two that import sibling modules (`-I` would drop the script directory from the path). All outputs went outside the worktree, which stayed clean (`git status --short --ignored` empty after every run).

| # | Command (args abbreviated) | Pre-edit (base `0a313fa`) | **Patch HEAD `17aa738`** |
|---|---|---|---|
| 1 | `python3 -B Tools/Calibration/OakBeechGrowth/check_integrated_v1.py --out <EV>/post/growth-integrated.json` | exit 1: `AssertionError: …ForestEcologyController.cs normalized source changed` | **exit 0: `INTEGRATED_CALIBRATION_V1_PASS checkpoints=360 protected_files=7`** (gate, normalizer, kernel, 360 expected rows and chosen inputs byte-identical to base) |
| 2 | `python3 -B Tools/Calibration/OakBeechGrowth/test_source_normalization.py` | not run | `SOURCE_NORMALIZATION_PASS` |
| 3 | `python3 -I -B Tools/Verification/SiteInfrastructure/run_site_check.py --out <EV>/post/site` | `SITE_INFRASTRUCTURE_PASS checks=63 siteProfiles=1250 registeredSpecies=3 isolatedCompile=true` | **identical**; includes `H_NO_OTHER_VERIFICATION_TOOL_REFERENCES_THE_NEW_TYPES` (131 tool files scanned), `H_ONLY_NUMERIC_LITERAL_…IS_THE_NEUTRAL_1F`, `G_SAVE_VERSION_IS_STILL_21` |
| 4 | `python3 -I -B Tools/Verification/ThreeSpecies/OfflineLogic/check_fixture_controls.py --out <EV>/post/fixture` | `FIXTURE_CONTROLS_PASS checks=9 static-contracts=7 NOT_UNITY_VERIFIED` | **identical** (it statically pins text of the harness I changed) |
| 5 | `python3 -I -B Tools/Verification/CloudTypeCheck/run_typecheck.py --mode tools --out <EV>/…` (the repo's tool, unmodified) | exit 1: 4 distinct errors vs baseline 43 | **identical output** (see limitation) |
| 6 | scratch type-check (below) | 36 distinct errors, **0** in the harness | **identical error set, 0 in the harness** |

**Type-check limitation (important).** Row 5 cannot validate this harness, and a "clean" would have been meaningless. The 4 errors are all `Painter2D` (a Unity 6 UI API absent from the tool's 2021.3 reference assemblies) in UI files I did not touch. They are *declaration-level*, and `csc` then skips binding every method body, which is why the tool's 43 baseline body-level errors all show as "GONE". I proved this with a negative control: injecting a type error into the pristine harness was **not** reported by the repo tool. To get a real compile check I built a scratch variant **outside the repo** (`<EV>/tools/`): the repo tool's own project and file set plus a scratch `Painter2D` stub and `LangVersion=preview`, which makes body binding work again. Its negative control on a scratch copy of the harness reported both injected errors (`CS0029`, `CS1061` against the real `TreeSpeciesDefinition`). On the patch HEAD it reports **0 errors in `BroadleafAdultGrowthVerification.cs`** and an error set identical to the pre-edit run (36 unrelated errors, stale Input System stubs against newer Rack/UI code). This is an approximation: not Unity's compiler, and the 2021.3 API surface differs from Unity 6000.6.

## 7. Proof of zero production / asset / scene / arithmetic diff

Full output: `post/zero-diff-proof.txt`.

- `git diff --name-status 0a313fa..17aa738` is exactly the seven files in §2.
- Serialized/binary/settings surfaces changed: **none** (`.asset`, `.unity`, `.prefab`, `.mat`, `.meta`, images, `ProjectSettings/`, `Packages/`, `Assets/Scenes/`, `Assets/ForestPrototype/Species/`).
- `git diff --quiet` between base and patch HEAD: **identical** for `Assets/ForestPrototype/Species`, `Assets/Scenes`, `ProjectSettings`, `Packages`.
- The single production `.cs` file in the diff is `ScenarioSiteProfiles.cs`; its raw diff is three comment lines, and its normalized (executable) hash is `7919c99af9b12403` at both base and HEAD. **Production `.cs` files with changed executable code: 0.**
- **No protected value moved:** no 16-hex fingerprint token appears on any added/removed line outside the contract JSON; `production_v1_expected.csv`, `production_v1_inputs.json`, `kernel_controls.py`, `check_integrated_v1.py`, `source_normalization.py` and `MixedDensityVerification.cs` are byte-identical to base; the contract diff is three `normalized_sha256` + three `source` lines.

## 8. What remains for Unity qualification

Nothing here takes the Unity lock. Sequence per the live table: S1-C-IMPL-02 → PARITY-03 Unity (#10) → the PARITY-04 Unity continuation.

1. **Clean import/compile** of the exact integrated candidate (0 `error CS`).
2. **Run the strengthened `BroadleafAdultGrowthVerification`** (copy → run → remove per its header; `CCF_STAND_GEOMETRY=0` for Legacy40, and the Enlarged80 value for the second geometry; confirm the env convention with the lane owner). **Expected marker: `BROADLEAF_GROWTH_VERIFY_PASS checks=30`** (30 check executions by control-flow count: 26 asserting + 4 informational: the `OAK_BEECH` report, two `FIXTURE_RECRUITS` reports, `CROWN_GEOMETRY_REPORT`). The old "14/14" is not comparable. Collect the six `ADULT_HEIGHT_DIAGNOSTIC` lines; arithmetic expectation: Oak ≈ 21.45 / 28.18 / 32.76 m, Beech ≈ 23.37 / 30.24 / 34.65 m with the Beech misses labelled.
3. **Consume, do not duplicate,** PARITY-03's protected pure-Sitka / model-0/1 / species-neutral density evidence (#10).
4. Re-run the §6 non-Unity set on the final integrated HEAD.

**First-run risks in the new code (it has never executed in Unity):** (a) the canopy and Hegyi probes assume `manager.InitializeNewScenario()` leaves no trees behind, the same assumption `Grid` already relies on, and they `Check` for exactly two living trees so a violation fails loudly; (b) the Hegyi probe moves a spawned tree's `transform.position` and relies on `UpdateCompetition` re-reading positions each call (it does, by source); (c) exact float equality of the YAML-loaded constants with C# literals (`0.8f`, `6.5f`, …) is expected but unproven at runtime, and the unchanged offline gate checks the asset *text*; (d) added wall-clock cost: about 180 extra annual steps for the height diagnostics plus nine probe-world resets.

## 9. Blockers, limitations, decisions

- **No blocker.** No measured adult defect (`BLOCKED — MEASURED ADULT DEFECT` not triggered); no drift outside the authorised set (`BLOCKED — SOURCE CONTRACT DRIFT OUTSIDE AUTHORISED SET` not triggered).
- **Decision for the Manager — seam coverage in the Unity gate** (§5): accept the live-scalar assertion plus the offline seam assertions, or authorise allow-listing the adult harness in the site check.
- **Recommendation (not done; outside the allowed files) — repo type-check hygiene:** the CloudTypeCheck approximation is blind to body-level errors until the `Painter2D` gap and the stale baselines are fixed. A separate verification-hygiene task should refresh its stubs/baselines.
- **Noted, no action:** `Tools/Verification/ScenarioTwoSpeciesFoundation/run_species_foundation_check.py` (default base `8f9ed80`, referenced nowhere else) already cannot pass on current `main` because `ForestSaveData.cs` and others have moved since; my comment edit to `ScenarioSiteProfiles.cs` does not change that. `Docs/Verification/ScenarioOneIntegration/ConstructionManifest.json` pins raw hashes of files this patch edited; it is a historical record no gate consumes.
- **Context mirrors:** the overview is a project-context source. No mirror or attachment was regenerated; the queued `CONTEXT-REFRESH-20261027-S1CD-PARITY` will pick the change up.
- **Worktree left in place** (`/media/jer/ZX20/CCF-Parity04Patch`, about 1.1 GB of tracked files; ZX20 is about 96% full). It has no uncommitted work once the branch is pushed and can be removed when no longer needed.

## 10. Task-claim lifecycle

#17 `[DISPATCHED]` → **`[RUNNING]`** (before any substantive edit; comment records actual model/provider, worktree, branch and starting HEAD) → `[HANDED-OFF]` and closed at publication, per the claim's instructions. `active-tasks.md` remains the ownership/lock authority and was not edited.

## 11. Evidence

`/media/jer/Files/CCF/Verification/Parity04Patch-20261027T200424Z/` — `pre/` (baselines and the type-check negative controls), `post/` (gate outputs, `zero-diff-proof.txt`, `contract-drift-attribution.txt`), `tools/` (the scratch type-check, its stub and the negative-control file, with a README).

## 12. Standard handoff block

```text
IMPLEMENTATION HANDOFF
Task: PARITY-04-PATCH — adult species docs / verification alignment
Worker: Claude Sonnet 5.5 (claude-sonnet-5-5) via Claude Code in T3 Code, local session (not Claude Cloud, not Opus)
Role: Primary (docs + verification tooling only)
Branch: feature/scenario-one-three-species-parity-adult-docverify-v1
Base: 0a313fad1e39d1c13487f5e71aeee5ee234c53a9
Context commit: not applicable (no context regeneration)
HEAD: patch HEAD 17aa738c58b474476ce9c6d1f6163d42c63ae5d5; final HEAD = the handoff commit above it
GITHUB HANDOFF PATH: Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-04/HANDOFF.md
GITHUB HANDOFF COMMIT: read back from the remote after push
EXTERNAL EVIDENCE ARCHIVE: /media/jer/Files/CCF/Verification/Parity04Patch-20261027T200424Z/
WHAT CHANGED: Oak/Beech adult docs corrected; adult gate strengthened; three-entry attributed contract refresh; two comment-only save-version fixes
FILES CHANGED: 7 (section 2)
SERIALIZED UNITY FILES TOUCHED: none
PLAYER-FACING RESULT: none
SIMULATION / SAVE IMPACT: none
ASSETS ADDED OR MODIFIED: none (one markdown doc under Assets/ForestPrototype/Docs)
MUST-NOT-CHANGE HASHES / ANCHORS: not touched (section 7)
INTENTIONALLY CHANGED HASHES / ANCHORS: three normalized source hashes in integrated_source_contract.json (Manager-authorised M-1; section 4)
VERIFICATION RUN: section 6
VERIFICATION RESULTS: check_integrated_v1 PASS (360/7); site check PASS (63); fixture controls PASS (9/7); normalization tests PASS; repo typecheck unchanged vs baseline (blind, see limitation); scratch typecheck 0 errors in the harness
SPEC CONFORMANCE: NOT VERIFIED at runtime (static/offline only)
REMAINING VERIFICATION GATES: section 8
KNOWN LIMITATIONS: strengthened gate never run in Unity; repo typecheck cannot bind method bodies; seam not called from the Unity gate (section 5)
UNRELATED WORK PRESERVED: yes (Primary checkout untouched; no other lane's files edited)
DECISIONS NEEDED: section 9 (seam coverage; typecheck hygiene task)
WORKER ASSESSMENT: Ready for review: Yes
RECOMMENDED NEXT STEP: Manager review; integrate when ready; then run the PARITY-04 Unity continuation after PARITY-03 Unity
```

`DASHBOARD SYNC: NOT PERFORMED` — no dashboard publisher in this session. `Published dashboard reflects this task: NO` (outstanding operational item for the Manager).

**Exit:** `PARITY-04 BOUNDED PATCH IMPLEMENTED — READY FOR UNITY QUALIFICATION`
