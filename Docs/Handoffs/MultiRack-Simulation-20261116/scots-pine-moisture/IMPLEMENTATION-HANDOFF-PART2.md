# Scots Pine and Site Moisture V1 — Part 2 Implementation Handoff

```text
IMPLEMENTATION HANDOFF
Task: MULTI-2D-SCOTS-PINE-MOISTURE-P2-20261128 (Playable Milestone 2 Part 2, Option A owner-locked, roadmap v1.22)
Worker: Claude Opus 5.5 (claude-opus-5-5), high effort. Owner-authorised fallback; Gemini 4 Neon was unavailable
        (provider credit failure before any work).
Role: Sole implementation writer for this lane
Coordinator: DeepSeek V4.1 Flash (read-only / coordination)
Base: origin/integration/2d-sim @ f4dee8023d3492a25e61451c7e2a49515e507ed3
Protected: origin/main @ bdeb6a324acaf4129c10fb0846faac8b64974360 (not touched)
Part 1 merge: 3beabb2a35e343e5e1eda73b759335385e6dea8f
Feature branch: feat/multi-2d-scots-pine-moisture-part2-v1
Review branch: review/multi-2d-scots-pine-moisture-part2-v1 (same final HEAD as the feature branch)
HEAD: the commit that adds this file. Code under test: 6a68cc19 (all source and test changes; this commit adds only this file).
GITHUB HANDOFF PATH: Docs/Handoffs/MultiRack-Simulation-20261116/scots-pine-moisture/IMPLEMENTATION-HANDOFF-PART2.md
EXTERNAL EVIDENCE: /tmp/scotspine-part2-gate/final/ (local, volatile; summarised below)
```

**State (D-030):** isolated branch implemented and verified; NOT merged to integration/2d-sim or main.

The verification below is the worker's own. Nobody has reviewed it independently yet. No Unity, Blender, or Windows build was run.

## What changed, by workstream

### 1. Live annual wiring (Core only)

- **Site moisture in the yearly loop.** A woodland now has an optional `MultiAreaScotsPineState` on `MultiAreaRoot`. It holds the site setting: either a whole-woodland class, or the Part 1 Dry Ridge & Moist Slope two-zone map split on the stand-bounds midpoint. All three places below look the modifiers up in `SiteMoistureEngine` / `SpeciesMoistureTable` and nowhere else.
  - **Adult growth.** Each species' adult growth is multiplied by its own **growth** modifier at the tree's position (`Phase2MixedDensity.StepRack`). For pure-Sitka racks this goes through `Stage4Options.SiteProductivity`.
  - **Natural establishment.** Each species' requested natural establishment is multiplied by its **establishment** modifier at the cell centre (`Phase2Annual.Step`).
  - **Planted pine.** A planted Scots pine has one first-year establishment check (`ScotsPineAnnual`).
- **Neutral woodlands take the old path.** When the state is null — every existing woodland, every legacy preset, and every Version 2 save — none of the new code runs. NeutralLegacy is exactly (1.0, 1.0). Core test SP2-02 shows that running the new code path on an existing preset with a neutral state gives the same results bit for bit.
- **Scots pine adults.**
  - **Species map.** Pine adults are listed in the Phase 2 species map as `"scots-pine"`. They are never added to `Phase2Options.GenesisSpecies`, which stays Oak/Beech only.
  - **DBH growth.** `ScotsPineEngine.EvaluateGrowth` is called directly, using the tree's age and its rack's start-of-year stocking, basal area and top height. Outside the GROWFOR domain there is no DBH increment.
  - **Crowns.** CrownModel 1 crowns are built from the pine card's crown fit and Ci50 4.75.
  - **Density mortality.** Pine is a non-reference stem in the generic density component. It is never part of the Sitka Comeau subset, and it is excluded from Sitka seed sources.
- **Local-seed regeneration.** `ScotsPineEngine.EvaluateRegeneration` is called once per in-polygon ground cell. Its inputs come from map state:
  - the age of the oldest pine within 30 m of the cell;
  - the seed-year schedule, using the woodland generator seed;
  - the cell's light, used as openness;
  - the larger of the bramble and bracken covers, used as undergrowth;
  - the moisture class at the cell centre.
- **Recruits and promotion.** An eligible cell gains a recruit with probability `0.25 × establishment modifier`, keyed by `RngSalts.ScotsPineRecruitment` + area + cell. Juveniles (planted and natural) grow, die or promote by a deterministic rule. Promotion happens at 3.0 m to `PL-<juvenileId>` or `PN-<recruitId>`.
- **RNG salts.** `RngSalts` registers the Part 1 salt as `ScotsPineGoodSeedYear = 0x1A96C03752AE8D4B` with its value unchanged, so the Part 1 digest still matches. It adds two new prefix-free string families: `SCOTS-PINE-RECRUIT-` and `SCOTS-PINE-ESTABLISH-`.
- **When draws happen.** Rolls go through the existing `Phase2Random.Roll` seam, so tests can observe them. No draw happens when:
  - there is no state;
  - there is no mature local seed tree;
  - it is a poor seed year;
  - a planting site has an establishment modifier of 1.
- **Work actions.**
  - **Planting.** Planting `"scots-pine"` reuses the Phase 2 Plant, Shelter and SpotClear orders. It is allowed on Forest Maker woodlands only, because their state persists in Save v3. The sapling material is the Part 1 EUR 0.45, the shelter lasts 8 years, and the spot clear is about 1 m².
  - **Pruning.** Pine has two lifts, 2.5 m then 5.5 m (inside the 5–6 m target), five years apart. The species-aware prune read-out shows this schedule.
- **Timber grading.** `D062TimberAssortmentEngine.GradeScotsPine` passes stem volume, maturity (40 years or more), pruned height, tight early stocking and the Small Roundwood DBH (14 cm or more) to `ScotsPineEngine.GradeTimber`. The prices are Sawlog EUR 68/t, Small Roundwood 45 and Pulp/Chip 38, and the harvested mass uses 1.02 t/m³. Neither the Sitka price ladder nor its 890 kg/m³ proxy is used. Two enum values were added: `TimberAssortmentKind.SmallRoundwood` and `PulpChip`.

### 2. Forest Maker / New Woodland

- **New preset** `fm-064-scots-pine-sitka-two-zone-v1`, titled "0.64 ha Mixed Scots Pine & Sitka (Dry Ridge & Moist Slope)". It is 80 × 80 m with five 16 × 80 m lanes, age 20, 2,100 stems/ha, giving 1,344 stems.
  - The cohort is 75 % pine on the dry ridge (x below 40 m) and 25 % pine on the moist slope [Calibration], which gives 692 pine at Year 0.
  - The new species mix `ScotsPineSitkaTwoZone` can also be chosen for custom recipes.
  - The canonical bytes and content hashes of the existing recipes are unchanged. The pine count is appended to stocking notes only for the new mix.
- **Picker.** The New Woodland dialog has a Site Moisture selector with NeutralLegacy (the default) plus the five classes. Labels, the default and the bog caution text come from Core (`GET api/multi/woodland/options`). `siteMoisture` is an optional member of the new-woodland request. Built-in stands and the two-zone preset lock the picker.
- **Label (locked rule).** `SiteMoistureLabels.WholeStandLabel` in Core always returns "Two-Zone (Dry Ridge & Moist Slope)" for the two-zone layout. Core test SP2-05 shows that the Part 1 tie rule would pick DryFreeDraining, and that the label does not.

### 3. Version 3 saves (Phase1 kept byte-identical)

- **New block.** Feature block `scotsPine`, version 1, in `src/Core/Saves/ScotsPineFeatureBlockCodec.cs`. It is registered in `FeatureBlockRegistry` and listed in `build/2d-gate/gate-config.json` under `saveV3.requiredCodecs` with `minBlockVersion: 1` and an authority note naming this handoff.
- **Why a separate block.** The frozen v2 validator only accepts Sitka, Oak and Beech, so the inner v2 payload is a pine-free projection (`ScotsPineSaveProjection.ForV2`):
  - pine species entries, pine juveniles, their shelters, work records and pending/approved pine orders are taken out;
  - pine deadwood is written as Sitka.
- **What the block carries.** The site setting, the pine tree IDs, tight-stocking IDs, pine deadwood IDs, the removed list items with their exact list positions, the natural recruits and the annual accounts. It ends with a `contentHash` (SHA-256 of the block body, 64 lowercase hex characters).
- **Decoding.** Decoding is fail-closed: exact keys, version gate, canonical bytes, hash, and consistency rules. Saved approved pine orders come back as Pending drafts, which is load policy A.
- **When the block is written.** Only when the woodland has Scots pine or site-moisture state, so every other v3 save is unchanged.
- **Absent block** gives NeutralLegacy. For the two-zone preset it gives that recipe's own genesis site and pine instead. Neither case is ever a refusal. Version 2 saves load unchanged.

### 4. Five-lens /multi UI and map sprites

- **Ecology data.** The ecology DTO has a new nullable `scotsPine` section that is filled in on Forest Maker woodlands only. It carries the site, zones, the separate modifiers per zone and species, active recruits, accounts, the plant option and the timber book. Every other payload — MA-1, MA-1-MIXED — is byte-for-byte unchanged.
- **Inspector data.** New route `POST api/multi/scots-pine/tree`, registered in `MultiRoutes.cs`, serves the inspector's per-tree detail from Core.
- **Modules.** No new JS module. The existing 16 modules were extended and the module graph is unchanged and acyclic. There is no 4th column and the dark theme is kept.
- **Lenses.** Woodland shows the site label and the bog caution. The lens slots that were reserved for this feature now show live Core values:
  - Trees: pine profile and growth-domain counts;
  - Regeneration: the separate pine and Sitka modifiers per zone, seed-year status and recruits;
  - Harvesting: the 68/45/38 EUR/t grades at 1.02 t/m³;
  - Habitat: the bog caution or "no bog", and the pine deadwood count.
- **Map.** New `#ccf-pine` (circular, straight-down, transparent gaps) and `#ccf-sapling-pine` sprites, with a stable rotation per tree ID. Recruits are drawn with dashed rings, the two-zone layout gets a ground tint and a dashed midpoint line, and the minimap colours pine separately.
- **Legend.** The pine and moisture legend entries appear only when the woodland carries them, so the default legend still has 16 items.
- **Other readouts.**
  - Plant menu: a Scots pine option with the EUR 0.45 note.
  - Inspector: site, modifiers, GROWFOR status, pruning and grade. A neutral site shows "NeutralLegacy — no moisture modifier".
  - History: a "Scots pine — Year N → N+1" card.
  - Economy labels: Scots pine sapling, Small roundwood, Pulp / chip.

### 5. Carried Part 1 test closure

`GROWTH-03` in `tests/Core/ScotsPine` shows that WetMovingAlluvial growth uses the growth modifier 0.35 and not the establishment modifier 0.70. The ScotsPine suite is now 27/27, and the Part 1 digest `96d214ce…1748` is unchanged.

## Player-facing result

- **New woodland.** The player picks a whole-woodland moisture class (a bog shows a habitat-protection caution) or the Dry Ridge & Moist Slope preset. That preset starts with mature pine leading on the dry ridge and Sitka on the moist slope.
- **Moisture effects.** Dry sites slow Sitka to 35 % of its growth. Pine grows inside its GROWFOR domain and stops growing DBH outside it.
- **Planting.** Pine can be planted with a shelter and a spot clear. Planted under a closed canopy it dies of light — the browser run planted in the brightest free interior cell, which had light 0.092, and the pine died in year 1.
- **Regeneration.** Natural pine appears only in good seed years, in bright gaps with low undergrowth and a mature pine within 30 m.
- **Value.** Pruning to 5.5 m is what lets a mature pine reach Sawlog (EUR 68/t).
- **Persistence.** Everything above survives Save v3 and Load.

## Simulation / save impact

- Woodlands without Scots pine or moisture state: no change to results, tokens, lineage or saves (anchors below; SP2-02; existing v3 economy and genesis tests pass unchanged).
- New state is opt-in: a non-neutral class at creation, the two-zone recipe, or the first pine Plant order (which opens a neutral state).
- Save v3 gains the `scotsPine` block. `src/Core/Phase1/**` is byte-identical: `git diff --stat f4dee802..HEAD -- src/Core/Phase1` is empty.

## Anchors

- **Must not change (all 9 asserted by Core.Tests and unchanged):**

  | Anchor | Value |
  | --- | --- |
  | 1 | `7e3a2a86…1ece` |
  | 2 | `a692a6d5…233d` |
  | 3 | `814e8165…6287` |
  | 4 | `8a926776…87fb` |
  | 5 | `c4b670ed…dc35` |
  | 6 | `c67edc6b…53d8` |
  | 7 | `776a68df…e22d` (1/1/1/1/0 equals 1/1/1/1/1) |
  | 8 | P0 binary `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3`. Re-hashed with `sha256sum .p0ref/SitkaLab.Core.dll` and asserted by P2-01. |
  | 9 | `93e9abda…251f` |

  The Part 1 digest `96d214ce52d58a6daf4db8e1d99ca05b6d61b43e769dfa00e5317cf636ba1748` is also unchanged.
- **Intentionally changed:** none.
- **New pins:** none. Pine behaviour is checked by assertion and determinism tests (SP2-06, SP2-10, SP2-12, SP2-S1), not by new hashes.

## Verification (from `Tools/MultiRackSitkaP1Lab`, WSL2, `dotnet --version` = 8.0.134, node v26.13.1)

Raw logs are in `/tmp/scotspine-part2-gate/final/`, run at HEAD `abcc5ff8`. The only later code commit is `6a68cc19`, a browser-driver fix; gate 7 was then re-run at that state.

| # | Command | Result |
| --- | --- | --- |
| 1 | `dotnet clean MultiRackSitkaP1Lab.sln`, then `dotnet restore MultiRackSitkaP1Lab.sln`, then `dotnet build MultiRackSitkaP1Lab.sln --no-restore` | **0 Warning(s), 0 Error(s)**, exit 0 |
| 2 | `dotnet run --project tests/Core/ScotsPine/SitkaP1.ScotsPine.Tests.csproj --no-build --no-restore` (the project is not in the .sln, so it was restored and built explicitly first: 0/0) | **TOTAL 27/27 passed, 0 failed** (Determinism 1, Biology 10, Timber 5, SiteMoisture 11); digest `96d214ce…1748` |
| 3 | `dotnet run --project tests/Core.Tests/SitkaP1.Core.Tests.csproj --no-build --no-restore` | **326/326 passed, 0 failed** (314 base + SP2-01..12). The 9 anchors are printed and asserted. |
| 4 | `dotnet run --project tests/Phase1.Save.Tests/Phase1Save.Tests.csproj --no-build --no-restore` (the packet's `Phase1.Save.Tests.csproj` is actually named `Phase1Save.Tests.csproj`; not in the .sln, built explicitly: 0/0) | **56/56 passed, 0 failed** (52 base + SP2-S1..S4). `git diff --stat f4dee802..HEAD -- src/Core/Phase1` is empty. |
| 5 | `node build/2d-gate/module-graph.mjs` | `MODULE-GRAPH PASS: 16 modules, acyclic, all reachable from main.js, 1 module script tag` |
| 6 | `dotnet build src/Ui/SitkaP1.Ui.csproj --no-restore` (after `dotnet restore src/Ui/SitkaP1.Ui.csproj`; the UI is not in the .sln), then `bash tests/ui-smoke-multiarea.sh` | Build **0/0**; smoke **OVERALL: PASS (363 passed, 0 failed)** |
| 7 | `bash tests/scots-pine-part2-ui/run-scots-pine-part2-edge.sh /tmp/scotspine-part2-gate/final/07-browser` (real Edge, headless, over CDP) | **1920x1080: 29 passed, 0 failed. 1280x720: 29 passed, 0 failed. OVERALL: PASS (71 passed, 0 failed)**, including Z01–Z04 for 4 server processes |

**Gate 7 coverage, per viewport.** 25 checks run in the main phase and 4 in the load phase. They cover:

- the picker default and the bog caution;
- the two-zone preview and Year 0 (692 pine, 1,344 stems);
- pine sprites, zones and legend;
- the five live lenses;
- the listener-leak check (10 → 10) and the render-time check;
- the pine inspector;
- the Plant menu: Plant, Shelter and SpotClear queued, quote "Scots pine sapling (€0.45)";
- Fell as deadwood, then Execute and Advance;
- the History card;
- Save v3, then SIGINT, then a new process, then Load: pine adults, the planted pine juvenile record, pine deadwood DW0001, recruits, accounts and the two-zone site all persist;
- zero console errors and zero 404/5xx;
- clean SIGINT stops with empty stderr, and git status unchanged.

**First attempt of gate 7.** It was kept in `07-browser-attempt1/` and recorded 65 passed, 2 failed. At 1280x720 the History drawer overlapped the Saves button; the second failure was the dependent load phase. The driver was fixed in commit `6a68cc19` to close the drawer with Escape, and the run above is the re-run after that fix.

**Skipped as instructed:** `build/2d-gate/selftest.sh`, the full `2d-gate.sh --browser`, `phase1-time-history-ui/run-step2-edge.sh`, Unity, Windows builds, `ui-smoke.sh`, `p0-isolation.sh`, `cli-safety.sh`, `build-output-safety.sh`. None of my changes needed them. Also not re-run: `tests/map-first-lenses-ui`, which expects a 16-item legend; that is unchanged for its MA-1-MIXED woodland.

## AC-7 mapping

| AC-7 element | Evidence |
| --- | --- |
| Annual growth in the integrated loop | SP2-03 (dry Sitka at 0.35), SP2-06 (pine DBH = Part 1 site-adjusted increment; moist Sitka bit-equal to neutral), SP2-12 (deterministic 10-year run) |
| Regeneration | SP2-10 (good years only, eligible gaps, determinism, zero draws without seed tree or in poor years), SP2-11 (juvenile rule), SP2-07 (planted pine assessed) |
| Work / economy outcomes | SP2-07 (EUR 0.45 sapling, shelter, spot clear, Forest Maker only), SP2-08 (2.5 / 5.5 m pruning), SP2-09 (68/45/38 EUR/t at 1.02 t/m³, both Sawlog routes), browser SP2-UI-19/20 (quote and settlement) |
| Woodland generation | SP2-04 (preset), SP2-05 (label), browser SP2-UI-06..10 |
| New and old save compatibility | SP2-S1 (cross-process v3 round trip, identical next year), SP2-S2 (whole class, absent block → neutral or genesis), SP2-S3 (v2 loads neutral), SP2-S4 (tamper refusals), browser SP2-UI-23..28 |
| Feedback in /multi | browser SP2-UI-02..22; smoke 363/363 |
| NeutralLegacy identity | SP2-02, the 9 anchors, the existing v3 economy and genesis tests |

## Known limitations and [Calibration] disclosures

- **Calibration values.** Every number below is a provisional [Calibration] teaching value, not an Irish prediction (D-059).
  - Live-loop values added in Part 2:
    - 30 m seed-tree search radius;
    - recruits: 25 % chance per eligible cell before the establishment modifier, starting at 0.10 m;
    - juveniles: grow 0.40 m/yr × min(1, light/0.60) × growth modifier; die at light below 0.20, or when smothered (exposure 0.70 or more while under 1.0 m); browse escape at 1.5 m; promotion at 3.0 m;
    - tight early stocking at 2,500 stems/ha;
    - maturity at 40 years;
    - Small Roundwood minimum DBH 14 cm;
    - pruning lifts 2.5 / 5.5 m;
    - generic growing-space weight 1.0;
    - 75 % / 25 % pine share in the preset.
  - Values inherited from Part 1: the growth shape, the gap thresholds, the seed-year blocks, the prices and the sapling cost.
- **Height growth.** Pine height follows the shared Stage 4 height step scaled by the moisture growth modifier, because Part 1 supplies DBH only. The pine C2 relaxation rate uses the Sitka reference value of 0.15, because the card has none.
- **No growth outside the GROWFOR domain.** Following Part 1, a pine outside the domain gets no DBH increment. This happens to young promoted pine under 10 years old, and to racks whose basal area is above 100 m²/ha after years without thinning. The inspector says so.
- **Light-demanding juveniles die under closed canopy.** A planted or natural pine juvenile dies in deep shade. At Year 0 the brightest interior cell of the preset is about 0.09, so planting pine under that canopy fails, and natural regeneration needs real gaps (openness of at least 0.60, undergrowth of at most 0.30).
- **Planted Oak and Beech ignore moisture.** The planted Oak and Beech juvenile rules are unchanged. Moisture affects their adult growth and their natural establishment only.
- **Whole-stem pine grading.** Pine is graded as one log; there is no log bucking. The harvested mass for the contractor cost uses 1.02 t/m³.
- **Inherited Part 1 notes.** Two pine prices equal Sitka figures (68 = Sitka pallet, 38 = Sitka pulp), but they are declared separately. The pine sapling (EUR 0.45) is ten times cheaper than Beech and Oak.
- **How I read "Unknown/absent block → neutral defaults, never a refusal".** An *absent* `scotsPine` block loads as neutral, or as the two-zone preset's own genesis. An *unknown block key* is still refused by the existing fail-closed C-V3 registry; I did not change that. *Unknown members inside* the block are refused, as the packet's tamper list requires.
- **P0 reference DLL.** This machine has SDK 8.0.134 only, and building the P0 DLL with it gives `bcbdbf05…` instead of the pinned `9a99f781…`. I installed the verified 8.0.133-built `.p0ref` copy (dll, sha256 and provenance) from the CCF-Multi-2D-Year30Compaction worktree; it is the same qualified P0 commit and tree. `.p0ref` is git-ignored and the P0 sources were not touched.
- **Test projects outside the .sln.** The ScotsPine and Phase1Save test projects and the UI project are not in the .sln. They need an explicit restore and build before the `--no-build` commands in the packet will run.
- **Recruits in lane-filtered payloads.** The recruit list in the ecology section covers the whole woodland. It is small, and the page filters it by area.

## Files changed

`git diff --stat f4dee802..HEAD` at code HEAD 6a68cc19: 53 files changed, 3561 insertions(+), 129 deletions(-). This handoff commit adds 1 file.

All files are inside `Tools/MultiRackSitkaP1Lab/` (src/Core, src/Ui, tests, and `build/2d-gate/{gate-config.json, rng-salts-patterns.json}`), `specs/multi-2d-scots-pine-moisture-v1/` and this handoff.

Not touched: `src/Core/Phase1/**`, `src/Ui/wwwroot/**`, `src/Ui/MultiViewBudget.cs`, `Assets/**`, `Packages/**`, `ProjectSettings/**`, `Docs/Project/**`, the .sln, any package, LFS or `.p0ref` binary in git, Downy birch/Common alder wiring, Real-World Inspection, Jay and Windstorm UI, and deep-history compaction.

- **Serialized Unity files touched:** none. **Assets added:** none. Two inline SVG symbols were added to `multi.html`.

`git status --short` before this commit: clean. Build output and evidence live outside the repo or in ignored paths.

## Unrelated work preserved

The worktree was clean at f4dee802. No other branch, worktree or merge was touched. Step 0 repaired the worktree metadata as instructed: the `.git` gitfile and the main repository's `worktrees/.../gitdir`. I did not move the worktree or rewrite history.

## Decisions needed

1. Confirm or retune the live-loop [Calibration] values, especially the juvenile light death threshold of 0.20 and the 30 m seed-tree search radius.
2. Should pine keep "no DBH growth outside the GROWFOR domain" in live play, or should a bounded fallback be specified?
3. Confirm my reading of "unknown block → neutral". Unknown block keys are still refused.
4. Confirm the EUR 0.45 sapling scale.
5. Optionally, update `tests/map-first-lenses-ui` if its 16-item legend count should cover pine woodlands.

## Worker assessment

Ready for independent review: **Yes**, for the isolated branch. Every gate in the verification diet passes, and these were run: the 9 anchors, Phase1 byte-identity, smoke 363/363, and the Edge suite at both viewports. Nothing is merged.

## Recommended next step

A non-author independent review (GPT-6.2 Sol, read-only) of `review/multi-2d-scots-pine-moisture-part2-v1` at this HEAD, against the spec quartet, this handoff and the packet's fences. After that review, the coordinator decides on integration into `integration/2d-sim`.

## Repair cycle 1 (independent review follow-up)

**D-030 state: repaired on the isolated branch `feat/multi-2d-scots-pine-moisture-part2-v1`. NOT merged to `integration/2d-sim` or `main`. An independent delta review is pending.**

Source: `SOL-INDEPENDENT-REVIEW-PART2-20261128.md` on `review/multi-2d-scots-pine-moisture-part2-v1` @ `beb0f61695f20955144dac9c151d8dfa5064d1ce`, which returned NEEDS CHANGES for `cbc369030827d715ac02d2e4c45238ed6a6783b7`. Repair implementer: Claude Opus 5.5.

**Repair commit: `954b2778ddb97a26af554dc976d760b38f8e2c6a`** (source and tests), parent `cbc36903`. This section is in a separate docs commit on top of it, because a commit cannot record its own SHA.

### Findings, root cause and fix

| # | Finding | Root cause | Fix |
| --- | --- | --- | --- |
| 1 | **Blocking.** Save v3 refused an approved plan whose only orders were Scots pine Plant orders, with `SaveInconsistent` and "an approved plan holds at least one order". | `ScotsPineSaveProjection.ForV2` stripped every pine Plant order from `Phase2.PendingOrders` and `Phase2.ApprovedOrders`, but it kept `MultiAreaRoot.Approved`. The frozen `Phase1SaveValidator` therefore received an approval with no orders and refused it before the `scotsPine` block was written. | `ForV2` sets `Approved = null` in the v2 projection, and only there, when all four approved lists are empty after stripping: `Approved.Orders`, `Stage4.ApprovedPrunes`, `Stage5.ApprovedClears` and `Phase2.ApprovedOrders`. The live root is unchanged. The pine orders still travel in `blocks.scotsPine.approvedOrders`, and `Apply` already restores them as Pending drafts. Load policy A is unchanged: no approval is restored, and the new session ID gives any fresh approval a new PlanId. |
| 2 | **Non-blocking.** On load, `DraftOrderCount` left out approved pine Plant orders. An approved pine Plant + Shelter plan reported 1 instead of 2. | `RestoreVerifiedV3IntoFreshSession` counted only the inner v2 `SavedApproval`. Approved pine Plant orders come from the block instead. | The new `ScotsPineSaveProjection.RestoredApprovedPlantCount(blocks)` re-reads the already verified `scotsPine` block and returns `approvedOrders.Count`, or 0 when the block is absent. The load result reports `DraftOrderCount(SavedApproval)` plus that number. Nothing is double counted: pine Shelter and SpotClear orders are in the v2 payload, and only pine Plant orders are in the block. The count is computed after the load, so `SaveV3Codec.Decode` and its placeholder-root block validation are unchanged. |

`DraftOrderCount` keeps its documented meaning: the number of formerly approved, unexecuted orders that came back as drafts. A plan that was pending, not approved, when saved still reports 0. That matches the v2 path and `Phase1SaveIntegrationTests`.

### Files changed (`cbc36903..954b2778`)

| File | Reason |
| --- | --- |
| `Tools/MultiRackSitkaP1Lab/src/Core/Saves/ScotsPineFeatureBlockCodec.cs` | `ForV2` clears an approval left empty by the projection. Adds `RestoredApprovedPlantCount`. |
| `Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/MultiAreaContinuation.cs` | The v3 load result counts restored approved pine Plant drafts. |
| `Tools/MultiRackSitkaP1Lab/tests/Phase1.Save.Tests/ScotsPineSaveTests.cs` | Adds the SP2-S5 and SP2-S6 regression tests. No existing test was changed. |

`src/Core/Phase1/**` is byte-identical, with tree `fe970e3f78362310821156de0d34bea990387aee`. No UI, Assets, Packages, ProjectSettings, Docs/Project or P0 file changed.

### New tests (Phase1Save.Tests)

- **SP2-S5** is the approved pine-only plan. It plants one Scots pine in `MA-E` with Economy off, approves, and saves before Execute. The save is schema 3, and the inner v2 state has `Approved == null` and no Phase 2 orders. `RestoreFromV3` succeeds with `Approved == null`, and the restored Pending list equals the original order: same ID, area, species and point. `DraftOrderCount == 1`. Executing with the pre-save PlanId and execution key refuses with `WorkPlanNotApproved`. A fresh approval mints a different PlanId. A re-save of the restored session is schema 3 and restores the same Pending draft, with `DraftOrderCount == 0` because the plan is now pending. Its `scotsPine` block is byte-identical across another save → load → save.
- **SP2-S6** is an approved pine Plant + Shelter plan at the same point. The save succeeds, and the inner v2 approval stays non-empty with one Shelter. The load restores both orders as Pending drafts, a pine `MultiAreaPlantOrder` and a `MultiAreaShelterOrder`, with `Approved == null` and `DraftOrderCount == 2`.

RED before the fix, from the same build with the new tests and the old source:

```
FAIL SP2-S5 ... — inconsistent save: an approved plan holds at least one order
FAIL SP2-S6 ... — two formerly approved orders restored as drafts: expected 2, got 1
56/58 passed, 2 failed
```

The validator raises "inconsistent save: …" as `P1Refusal("SaveInconsistent", …)` (`Phase1SaveValidator.cs:513`).

### Gate results (from `Tools/MultiRackSitkaP1Lab`, WSL2, `~/.dotnet/dotnet`, TMPDIR outside the repo)

| Gate | Result |
| --- | --- |
| `~/.dotnet/dotnet --version` | 8.0.134 |
| build `MultiRackSitkaP1Lab.sln` | 0 Warning(s), 0 Error(s) |
| build `tests/Core/ScotsPine/SitkaP1.ScotsPine.Tests.csproj` | 0 Warning(s), 0 Error(s) |
| build `tests/Phase1.Save.Tests/Phase1Save.Tests.csproj` | 0 Warning(s), 0 Error(s) |
| build `src/Ui/SitkaP1.Ui.csproj` | 0 Warning(s), 0 Error(s) |
| `SitkaP1.Core.Tests.dll` | 326/326 passed, 0 failed. All nine frozen anchor SHA-256 values appear in the output. |
| `Phase1Save.Tests.dll` | 58/58 passed, 0 failed (56 prior + SP2-S5 + SP2-S6). Includes `golden: pinned SHA-256 of the fixed Phase 2 approved sample encoding` and SP2-S4. |
| `SitkaP1.Core.ScotsPine.Tests.dll` | TOTAL: 27/27 passed, 0 failed. Digest `96d214ce52d58a6daf4db8e1d99ca05b6d61b43e769dfa00e5317cf636ba1748` is unchanged. |

The packet named the Scots pine test assembly `SitkaP1.ScotsPine.Tests.dll`. The csproj builds `SitkaP1.Core.ScotsPine.Tests.dll`, so that file was run. `.p0ref/SitkaLab.Core.dll` still has SHA-256 `9a99f781…34b3`.

Skipped per the packet: the Edge/browser suites, `tests/ui-smoke*.sh`, `build/2d-gate*`, `run-step2-edge.sh`, any npm/node UI build, and Unity. No UI code changed. The Saves dialog shows the corrected `draftOrderCount`, but no rendered check was run.
