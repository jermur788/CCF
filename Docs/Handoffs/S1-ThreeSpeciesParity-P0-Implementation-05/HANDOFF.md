# PARITY-05-PATCH — Early-Life / Browse Conformance Closure — Handoff

**Task:** `PARITY-05-PATCH` (bounded documentation + verification closure; no production-behaviour change)
**Spec:** `specs/scenario-one-three-species-parity-p0/` — APPROVED
**Claim:** GitHub issue #32 (`jermur788/CCF`)
**Date:** 2026-10-28
**Worker / routing:** Claude Sonnet 5.5 (`claude-sonnet-5-5`, Anthropic) via Claude Code in T3 Code, local session. The packet targets "Claude subscription 2 / strongest available Claude model"; this session cannot verify which account it runs under and is not necessarily the strongest model. The actual executor is recorded here and no claim of the preferred routing is made.
**Worktree:** `/media/jer/ZX20/CCF-Parity05Patch` (new, clean, non-Unity, no `Library`). The dirty `/media/jer/ZX20/CCF-Primary` checkout was not touched.
**Branch:** `feature/scenario-one-three-species-parity-earlylife-conformance-v1`
**Unity:** **NOT RUN. Lock not acquired.** **Windows build:** none.

## Verdict

**`PARITY-05 CONFORMANCE PATCH IMPLEMENTED — READY FOR UNITY QUALIFICATION`**

with one item reported rather than done, as the packet instructs (closure item 2, production header wording — see §6). This does **not** claim `EARLY-LIFE SPECIES PARITY REQUALIFIED`: the strengthened Unity harnesses have never run in Unity.

- **Zero** species asset, scene, prefab, material, package, project-setting, save/schema, production-C#, value, curve or protected-anchor change (§5).
- Manager decisions M1/M2/M3 are preserved exactly: no packet-only early-life table, no `1.0/1.8/2.2` browse multipliers, no protected `0.2` multiplier (§4).

## 1. Identity

| Item | SHA |
|---|---|
| Starting HEAD (= `origin/main` at task start, re-fetched immediately before claiming) | `86bd5c7a55f6733f24f50e0c8ce46eaf5e470b2a` |
| Coordination-only commit (see §8) | `2b3831575112fb68c50139e49411e418ecba945b` — changes only the PARITY-05-PATCH row of `Docs/Project/coordination/active-tasks.md`; lane fast-forwarded to it; no content effect |
| **Patch HEAD (tested)** | `444a11688529d736fa8d984c18aff95a7b968c1c` |
| Final HEAD | the commit that adds this handoff above the patch HEAD (documentation-only) |
| `origin/main` at publication | still `2b38315` (no drift; the patch cannot conflict) |

No other live row owned the allowed files at start (checked in `active-tasks.md` on `origin/main`).

## 2. Changed files (7; all on the packet's allowed list)

| File | Kind | Diff |
|---|---|---|
| `Docs/BeechSpeciesParameters.md` | evidence-label documentation | +juvenile-row class, +section *Early-life response and planting stock (current)* |
| `Assets/ForestPrototype/Docs/SessileOakV1.md` | evidence-label documentation (markdown under `Assets/`; `.meta` untouched) | Beech-fallback sentence, +section *Early-life and browse labels (current)* |
| `Docs/BrowsingProtectionV1.md` | evidence-label documentation | +section *Species susceptibility, protection and Scenario One 0.2 (PARITY-05 clarification)* |
| `Docs/Project/unity-project-overview.md` | factual current-state wording (no live ownership conflict) | +section *Current early-life and browse model (PARITY-05 documentation alignment, 2026-10-28)* |
| `Tools/Verification/JuvenileMortalityFoundationVerification.cs` | verification, additive | +55 / −0 |
| `Tools/Verification/BrowsingProtectionVerification.cs` | verification, additive | +50 / −0 |
| `Tools/Verification/ThreeSpecies/OfflineLogic/check_early_life_conformance.py` | the one optional new focused file | new, read-only static check |

`RegenerationBudget/RegenerationModelVerification.cs` was **not** edited: its existing species-path coverage is unchanged and the shared-calibration/selection assertions fit more cleanly in the juvenile foundation harness.

## 3. Evidence-label changes (before → after)

- **Beech label (closure 1).** `BeechSpeciesParameters.md` juvenile light-response row now reads "the **combined** establishment/survival use of it is `FALLBACK-CONSERVATIVE`" and the new section states the exact rule: `useDistinctJuvenileLightResponses` is off, establishment follows the juvenile light curve, annual survival is exactly 1.0 at or above `regenPoorLightThreshold` (0.15) and `1 − regenMortalityUnderPoorLight` (0.8) below. No Beech curve was invented. `SessileOakV1.md` previously said "legacy combined response path" without a class; it now names the class for Beech and states that Oak keeps its distinct curves.
- **Shared calibration.** 0.35 m/yr, 3.5 m, 0.5, 1.5, 0.2, 0.15 are labelled `GAMEPLAY CALIBRATION`, shared, not species evidence; species-specific items listed separately (initial height 0.15/0.18, Oak 0.2 promotion-light floor, seed rules).
- **Broadleaf stock (closure 2, documentation part).** Planting stock (age 3, 0.6 m, density 1) is labelled **broadleaf stock abstraction v1**, species-neutral, in the Beech, Oak and overview documents. No document previously carried Beech-only stock wording; the only stale text is in a production file (§6).
- **Browse truth (closure 3).** `BrowsingProtectionV1.md` and the overview state Sitka 0.15 / Beech 0.5 / Oak 1.0, ordering Oak > Beech > Sitka (`TRANSFER` ordering, gameplay-calibrated numbers), absolute `[0,1]` inputs not multipliers, and that Scenario One `0.2` is `ScenarioOneDefinition.backgroundBrowsePressure`, not a protection factor.
- **Protection truth (closure 4).** Documents state binary access (intact fence / effective shelter 0; breached / expired / failed / outside 1; cohorts get the geometric unfenced fraction) and browse-free-baseline equivalence for protected outcomes.
- **Unknown species (closure 7).** Recorded in the overview as a PARITY-08 follow-up only; not touched.

## 4. Verification assertions added (all additive; none ever run in Unity)

**`BrowsingProtectionVerification` — `VerifyEarlyLifeBrowseConformance` (called right after the existing palatability check):**

- strict ordering `Oak > Beech > Sitka` on the loaded palatabilities;
- `Scenario One pressure == 0.2`;
- per species at a height below every full-vulnerability height: unprotected probability is **exactly** `0.2 × palatability` with reason `Exposed`, and strictly decreases Oak → Beech → Sitka;
- protected (`access 0`, intact fence) probability is **exactly 0** with reason `Protected`, at pressure 0.2 **and** at pressure 1 — i.e. 0.2 is not a residual protected multiplier;
- a 21-year × 4-point scan of real `BrowsingConditions.ProtectionAt` over a fence and a shelter: `access ∈ {0,1}` always, and `access == 0` exactly when the state is intact fence / effective shelter; both outcomes are reached.
- **Hash safety:** the block logs with `Debug.Log`, never `Emit`, because `Emit` feeds `BROWSE_MEASUREMENT_HASH`. The static check fails if `Emit(` appears in the block. Marker: `BROWSE_EARLY_LIFE_CONFORMANCE_PASS`.

**`JuvenileMortalityFoundationVerification` — `VerifyEarlyLifeConformance` (static, read-only, first call in `Verify`):**

- selection: Oak `UsesDistinctJuvenileLightResponses`, Beech and Sitka not;
- exact shared calibration for all three (0.35 / 3.5 / 0.5 / 1.5 / 0.2 / 0.15, `SupportsRegeneration`), initial heights 0.15 / 0.18 / 0.15 and promotion-light floors 0 / 0.2 / 0;
- Beech and Sitka at nine light values: `SurvivalResponse` equals the combined fallback rule **exactly**, and `JuvenileEstablishmentResponse` equals the growth light response;
- Oak: its survival and establishment curves **differ** from the combined fallback at some light (guards against a relabelled copy);
- shared planting-stock values `3 / 0.6 / 1` on the controller. Marker: `EARLY_LIFE_CONFORMANCE_VERIFY_PASS`.
- It runs only in `Verify()`, not in `VerifyCanonical()`, and mutates nothing, so the canonical lifecycle anchors are unaffected.

**`check_early_life_conformance.py` (runs now, offline, 34 checks):** asset contract (selection, shared calibration, exact Oak anchors, Beech curve, palatabilities, ordering, escape heights); production source contract (survival selection, browse constants and product formula, `ProtectionAt` assigns only `0f`/`1f`, Scenario One `0.2` only copied to background pressure, stock defaults and scene values); absence of the rejected packet-only concepts from production; documentation labels; harness presence and hash safety; and, with `--base`, the diff proof (allowed files only, no asset/scene/production-C# change, harness edits additive, no 16-hex fingerprint token touched).

No harness assertion was weakened: the diff has **zero removed lines** in every harness file.

## 5. Commands and results (non-Unity; patch HEAD `444a116`)

Evidence: `/media/jer/Files/CCF/Verification/Parity05Patch-20261028T203337Z/` (`post/`, `tools/`). All outputs went outside the worktree, which stayed clean (`git status --short --ignored` empty after every run); runners used `python3 -I -B` (`-B` only for the two that import siblings).

| # | Command | Result |
|---|---|---|
| 1 | `python3 -I -B Tools/Verification/ThreeSpecies/OfflineLogic/check_early_life_conformance.py --out <EV>/post/early-life-at-patch-head --base 2b38315` | **`EARLY_LIFE_CONFORMANCE_STATIC_PASS checks=34 NOT_UNITY_VERIFIED`** |
| 2 | 10 mutation negative controls against that checker (`tools/negative_controls.py`) | **`ALL_NEGATIVE_CONTROLS_OK`** — each of: Beech palatability 0.6, Oak below Beech, Beech given the distinct flag, retuned shared growth, fractional protection access, browse formula dropping access, Scenario One pressure changed, Beech label removed, `Emit` added to the browse block, an `establishmentRatePerYear` scalar introduced, is detected by the named check |
| 3 | scratch type-check, Assets + the two harnesses only (`tools/scratch_typecheck2.py`) | **0 errors in both harnesses**; 8 pre-existing 2021.3-approximation errors in unrelated `Assets/` files; body-level negative controls (`int x = "string"` injected into each new block) are reported as `CS0029` |
| 4 | `python3 -B Tools/Calibration/OakBeechGrowth/check_integrated_v1.py` | `INTEGRATED_CALIBRATION_V1_PASS checkpoints=360 protected_files=7` |
| 5 | `python3 -B Tools/Calibration/OakBeechGrowth/test_source_normalization.py` | `SOURCE_NORMALIZATION_PASS` |
| 6 | `python3 -I -B Tools/Verification/SiteInfrastructure/run_site_check.py` | `SITE_INFRASTRUCTURE_PASS checks=63 siteProfiles=1250 registeredSpecies=3 isolatedCompile=true` |
| 7 | `python3 -I -B Tools/Verification/ThreeSpecies/OfflineLogic/check_fixture_controls.py` | `FIXTURE_CONTROLS_PASS checks=9 static-contracts=7 NOT_UNITY_VERIFIED` |
| 8 | `python3 -I -B Tools/Verification/ThreeSpecies/OfflineLogic/run_offline_check.py --suite all` | **exit 1 on the patch and on the untouched base `2b38315` with byte-identical PASS/FAIL output** (see below) |

**Row 8, pre-existing failures (not caused by this patch, not fixed here).** `NO_PRODUCTION_IDENTIFIER_OR_STRING_EXPRESSES_A_SPECIES_MAXIMUM_DENSITY_OR_MAX_BASAL_AREA` fails on `ScenarioOneRackBay.cs: identifier MaxRemovedBasalShare` (`THREE_SPECIES_OFFLINE_FAIL failures=1 checks=91`), and three long-run density cases fail R2 (`LONG_RUN_L40_B_BEECH_DENSE`, `LONG_RUN_L40_F_BEECH_YOUNG`, `LONG_RUN_E80_F_BEECH_YOUNG`; `COMPOSITE_DENSITY_FAIL checks=107 failures=3`). The same suite on a clean detached checkout of `2b38315` (removed afterwards) produced identical output. They belong to the density (PARITY-03) and rack-bay surfaces. Passing on both: `MIXED_DENSITY_ISOLATED_PASS`, `SITKA_MODEL_ISOLATED_PASS`, `THREE_SPECIES_TIMBER_PASS checks=71`, `THREE_SPECIES_HISTORY_PASS checks=74`.

**Type-check limitation.** The repo's own `CloudTypeCheck` cannot bind method bodies on current `main`: compiling all of `Tools/Verification` together raises duplicate-type declaration errors across unrelated suites, and `csc` then skips bodies. The scratch project compiles only the harnesses against `Assets`, which does bind bodies (proved by the negative controls). It type-checks against a Unity 2021.3 reference approximation, so a clean result means "type-correct", not "passes in Unity 6000.6".

## 6. Items reported, not done

1. **Stale production wording (closure 2, production part — STOP and report per the packet).** `Assets/ForestPrototype/ForestEcologyController.cs:29-35` still has `[Header("Planting (Beech v1)")]` and tooltips saying "nursery-stock Beech" / "planted Beech juvenile" on the three shared stock fields, which every planted species uses. Correcting it needs a production-file edit (a `[Header]`/tooltip string change; no behaviour, but it is serialized-inspector text), so I did not do it. Proposed follow-up: a comment/tooltip-only packet. The documentation now states the abstraction and the stale header explicitly.
2. **Unknown species on load** (`ForestSaveController` falls back to the default species for trees; `RestoreCellState` skips unknown cohorts; `ForestSaveValidation` has no known-species check) — PARITY-08, untouched.

## 7. Preservation proof

Full output: `post/zero-diff-proof.txt`. Against `2b38315`:

- changed files are exactly the seven in §2;
- no `.asset .unity .prefab .mat .meta` or image/model/shader/asmdef file changed; `git diff --quiet` is **identical** for `Assets/ForestPrototype/Species`, `Assets/Scenes`, `ProjectSettings`, `Packages`, `Assets/ForestPrototype/Racks`, `Assets/ForestPrototype/ScenarioOne`;
- **no `Assets/**/*.cs` file changed**; git blob ids of `JuvenileEcologyRules`, `BrowsingConditions`, `TreeSpeciesDefinition`, `ForestEcologyController`, `ScenarioOneManager`, `ScenarioOneDefinition` and the three species assets are identical to base;
- no 16-hex fingerprint token appears on any added or removed line; the documented anchors (`BFC55473C1506067`, `3485B6630C9EA448`, `BROWSE_MATRIX_HASH BEC549ED9EBA18B7`, `BROWSE_MEASUREMENT_HASH 32224555690DD824`) are untouched;
- `check_integrated_v1.py` still pins all seven protected sources as unchanged.

## 8. Coordination note (disclosure)

At task start I set the PARITY-05-PATCH row in `Docs/Project/coordination/active-tasks.md` to `[RUNNING]` and pushed it straight to `origin/main` as `2b38315`, then marked the GitHub claim. `active-tasks.md` is Overall-Manager-maintained live state, and earlier workers recorded only the GitHub claim. That row edit therefore went beyond what the packet strictly asks. It contains only this task's own facts (executor, worktree, base). The patch itself is proven against `2b38315`, not `86bd5c7`, so the proof excludes it by construction. The Manager may keep or revert that row.

## 9. What remains for Unity qualification (separate task; obeys the machine-wide queue)

Nothing here takes the Unity lock. Scope proposal (Tier 2, focused; no regression expansion):

1. Clean import/compile of the exact integrated candidate (0 `error CS`).
2. Run `JuvenileMortalityFoundationVerification` (`Begin`): expect `SHARED_JUVENILE_RULES_VERIFY_PASS`, **`EARLY_LIFE_CONFORMANCE_VERIFY_PASS`** and `JUVENILE_MORTALITY_FOUNDATION_VERIFY_PASS`; then `BeginCanonical`: expect `JUVENILE_CANONICAL_VERIFY_PASS` with the lifecycle hashes `BFC55473C1506067` (neutral) and `3485B6630C9EA448` (Scenario One 0.2) **unchanged**.
3. Run `BrowsingProtectionVerification`: expect **`BROWSE_EARLY_LIFE_CONFORMANCE_PASS`** and `BROWSING_PROTECTION_VERIFY_PASS`, with `BROWSE_MATRIX_HASH BEC549ED9EBA18B7` and `BROWSE_MEASUREMENT_HASH 32224555690DD824` **unchanged**.
4. Copy → run → remove per each harness header (the harnesses are Editor-only disposable runners). Confirm no temporary script or generated `.meta` remains under `Assets/ForestPrototype/`. `JuvenileMortalityFoundationVerification` and `BrowsingProtectionVerification` do not write the save slot.
5. Any hash movement is a failed gate and is reported, not rebaselined.

**First-run risks in the new code (never executed in Unity):** float equality `open.Probability == pressure × palatability` relies on every other term being exactly `1f` at the chosen 0.3 m height (derived by hand from `AssessBrowse`, not run); the shelter in the scan relies on `BrowseShelter.IsEffective` semantics already used by the existing shelter checks. A failure there would indicate a harness arithmetic mistake, not a production change.

## 10. Standard handoff block

```text
IMPLEMENTATION HANDOFF
Task: PARITY-05-PATCH — early-life/browse conformance closure
Worker: Claude Sonnet 5.5 (claude-sonnet-5-5) via Claude Code in T3 Code, local session
Role: Primary (docs + verification only)
Branch: feature/scenario-one-three-species-parity-earlylife-conformance-v1
Base: 86bd5c7a55f6733f24f50e0c8ce46eaf5e470b2a (patch proven against coordination-only 2b3831575112fb68c50139e49411e418ecba945b)
Context commit: not applicable (no context regeneration; overview is a context source — queued context refresh will pick it up)
HEAD: patch HEAD 444a11688529d736fa8d984c18aff95a7b968c1c; final HEAD = the handoff commit above it
GITHUB HANDOFF PATH: Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-05/HANDOFF.md
EXTERNAL EVIDENCE ARCHIVE: /media/jer/Files/CCF/Verification/Parity05Patch-20261028T203337Z/
WHAT CHANGED: FALLBACK-CONSERVATIVE Beech label; broadleaf stock label; browse values/ordering/binary-protection/0.2-is-background documented; additive harness assertions; offline static checker
FILES CHANGED: 7 (section 2)
SERIALIZED UNITY FILES TOUCHED: none
PLAYER-FACING RESULT: none
SIMULATION / SAVE IMPACT: none
ASSETS ADDED OR MODIFIED: none (two markdown docs, one under Assets/ForestPrototype/Docs)
MUST-NOT-CHANGE HASHES / ANCHORS: untouched (section 7)
INTENTIONALLY CHANGED HASHES / ANCHORS: none
VERIFICATION RUN: section 5
VERIFICATION RESULTS: static checker PASS 34; 10/10 negative controls; harness type-check 0 errors (body-binding proven); protected-source/site/fixture/normalization suites PASS; run_offline_check --suite all fails identically on patch and untouched base (pre-existing)
SPEC CONFORMANCE: NOT VERIFIED at runtime (static/offline only)
REMAINING VERIFICATION GATES: section 9 (focused Unity qualification)
KNOWN LIMITATIONS: strengthened harnesses never run in Unity; stale production Header "Planting (Beech v1)" not edited (section 6); pre-existing offline density/rack-bay failures (section 5 row 8); type-check is a 2021.3 approximation
UNRELATED WORK PRESERVED: yes (Primary checkout untouched; no other lane's files edited)
DECISIONS NEEDED: (a) authorise a comment/tooltip-only follow-up for the stale production Header; (b) keep or revert the active-tasks.md row edit (section 8); (c) triage the pre-existing offline failures separately
WORKER ASSESSMENT:
Ready for review: Yes
DASHBOARD SYNC: NOT AVAILABLE
PUBLISHED DASHBOARD REFLECTS THIS TASK: NO
DASHBOARD SYNC FOLLOW-UP:
- no dashboard publisher in this session; Manager to run or allow ingestion
RECOMMENDED NEXT STEP:
- Manager review; queue the focused Unity qualification (section 9) when the machine-wide Unity lane is free
```

**Exit:** `PARITY-05 CONFORMANCE PATCH IMPLEMENTED — READY FOR UNITY QUALIFICATION`
