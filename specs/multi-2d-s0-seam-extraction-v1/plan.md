# CCF Multi 2D — Pre-2A (S0) Seam Extraction and 2D Integration Trunk — Implementation Plan

- **Spec ID:** `SPEC-multi-2d-s0-seam-extraction-V1`
- **Feature ID:** `multi-2d-s0-seam-extraction-v1`
- **Spec status:** `DRAFT` — this plan authorises **zero** implementation
- **Classification:** `SPEC REQUIRED`
- **Plan owner:** Spec author (draft); appointed 2D integrator after approval
- **Authoring base (context) commit:** `5fe2fabe9073d7b72711f4ab48553c785351d7bc`
- **Created:** 2026-11-25
- **Last updated:** 2026-11-25

**Authority.** This plan is derived from `spec.md` and adds no product requirement. It is subordinate to the latest user instruction, `Docs/Project/decision-log.md`, `Docs/Project/current-milestone.md` and `Docs/Project/game-brief.md`. Implementation truth is the inspected repository. While the spec is `DRAFT`, nothing here may be executed.

**Line-number policy.** Every table below is **keyed by member name or banner text**. Line numbers are informational, measured at Step 2 `adea7af2…` (UI) or at the base `5fe2fabe…` (Core, which is identical at both). The real source is the owner-accepted baseline `T0`, which may differ because Phase 1 Step 2 is still in R2/R3. At packet start the worker regenerates the line spans from `T0` and records them in the move table; a missing or renamed member is a stop condition.

---

## 1. Implementation approach per packet

### 1.1 Pre-S0 (integrator, after approval)

1. **T-00 Trunk creation.** After owner acceptance of the consolidated Phase 1 baseline `T0`: `git push origin <T0>:refs/heads/integration/2d-sim`, then record `git ls-remote origin refs/heads/integration/2d-sim` (must print `T0`). No commit is made on the trunk at creation.
2. **T-00b Baseline capture (read-only at `T0`).** Build and run the existing suites exactly as §3.9 of the spec lists them. Record per-suite pass counts and the exact summary-line formats the gate will parse; the IDs of every payload-budget check in the `ui-smoke-multiarea.sh` log; the Core stdout lines for anchors #7 and #9; the served `/multi` body hashes `P_x` and headers; the protected-listener state; and tool versions (dotnet SDK, node, python3, Edge). These values seed `build/2d-gate/baseline-counts.json` and the identity JSON in S0a.

### 1.2 S0a — mechanical moves + trunk gate

The packet lands as **four separate commits** on one feature branch, so each can be reverted alone after the `--no-ff` merge:

| Commit | Content |
|---|---|
| A-C1 `build(2d-gate): add fail-closed 2D trunk gate and self-test` | `build/2d-gate.sh`, `build/2d-gate/{frozen-anchors.json, protected-paths.txt, determinism-allowlist.txt, baseline-counts.json, selftest.sh}` |
| A-C2 `refactor(ui): extract /multi page verbatim into embedded assets` | `src/Ui/multi/{multi.html, multi.css, multi.js, .gitattributes}`, `build/2d-gate/{raw-literal.py, s0a-multi-asset-identity.json}`, `SitkaP1.Ui.csproj` entries, the three static-initialiser swaps plus `MultiEmbeddedAsset` helper, removal of `MultiAreaPage` |
| A-C3 `refactor(ui): split /multi PlayerApi partial into route files` | `git mv src/Ui/PlayerMultiApi.cs src/Ui/MultiRoutes.cs` followed by member moves into `MultiApi.{View,Commands,Saves,History}.cs`; `build/2d-gate/{move-proof.py, s0a-move-table.json}` |
| A-C4 `refactor(core): split continuation Work Plan commands into partial files` | `MultiAreaContinuation.Commands.{Crop,Fell,Prune,Clear,Planting,Plan}.cs`; member removals in `MultiAreaContinuation.cs`; move-table rows |

**Order matters.** A-C2 is done **before** A-C3, so the raw-literal extraction reads the untouched `T0` file layout and the move proof of A-C3 operates on a file that no longer contains the constants.

### 1.3 S0b — fixed engine seams

1. **RngSalts first** (lowest risk, value-identical). Add `src/Core/RngSalts.cs`, substitute the literals (§2.5), add `tests/Core.Tests/RngSaltsTests.cs` plus one registration line, and extend G6.
2. **`IFeatureBlockCodec`** interface file, with zero implementations.
3. **AnnualPipeline attempt** with relocations M1 and M2 only (spec §6.2), then decision point DP-S0b (spec §6.3). On any trigger T1–T6, revert the pipeline routing in `Advance` (the `Advance` member text must return to byte-identity with `T0`) and record `RETAINED (Tn)`.
4. **Contract presence.** The C-V3, C-ECON and C-FENCE text in the approved spec is unchanged (the gate does not implement them; review inspects them).

### 1.4 S0c — native ES modules

1. Write the parity driver (`tests/s0c-parity/*`) and capture the **reference** at the S0b trunk tip, before touching any module.
2. Produce the inventory: section→module assignment (§2.3), layer of each module, the full list of upward/cyclic calls (→ `s0c-hook-inventory.json`), and the external calls into `map/` functions (→ `s0c-renderer-callsite-map.json`).
3. Perform the split using only P1–P6 (spec §7.2). Preserve indentation and add no formatting.
4. Add `module-graph.mjs` and `js-reconstruct.mjs`; update `tests/ui-smoke-multiarea.sh` (M-series module routes, `node --check` per module via scratch `.mjs`, and a J-loader using `vm.SourceTextModule`).
5. Run the gate with `--browser` and the parity comparison.

---

## 2. File-by-file mechanical-move tables

### 2.1 `src/Ui/PlayerMultiApi.cs` (`PlayerApi` partial) → route partial files

Leading XML doc comments, attributes and comment lines directly above a member move with it. All target files declare `namespace SitkaP1.Ui;` and `internal sealed partial class PlayerApi`.

| Member(s) (start line at adea7af) | → Target file |
|---|---|
| File-level `using` lines (1–8); class doc comment (12–18); constants `MaxIdLength` … `MaxHistoryPageSize` (21–28); **all** static readonly fields in original order: `MultiFenceMembers` (30) … `MultiRecoverMembers`, `MultiPageHtml`/`MultiPageCss`/`MultiPageJs` (53–55, initialisers swapped per spec §5.2 (i)), `MultiLabels` (57); instance fields `multiArea`, `multiSession`, `saveSlots`, `savesWritten`; `SavesWritten` (62–68); `AddMultiAreaRoutes` (71); `RefusableMulti` (128); `ScenesOf` (1031); `ReadMultiRequestAsync` (1034); `RequireMember` (1068); `RequireBoundedText` (1080); request records `MultiFenceRequest` (1097) … `MultiPlantingRemoveRequest` (1128); **new** private `MultiEmbeddedAsset` helper | `src/Ui/MultiRoutes.cs` |
| `WriteMultiManifestAsync` (139); `ReadMultiViewAsync` (157); `BoundedMultiSnapshot` (402); `R3`, `R3f` (413–414); `ProjectEcologyForView` (418); both `SerializeBoundedMultiView` overloads (445, 452); `ProjectViewForBudget` (633); `ProjectScenesForBudget` (665); `ProjectStage4ForBudget` (686); both `BoundSnapshotForResponse` overloads (753, 756); `BoundSessionViewForAux` (760); `TailOf` (781); `WriteRawJsonAsync` (791); `ReadMultiCellAsync` (941); `ReadMultiCompetitionAsync` (961); `MultiHitTestAsync` (984); records `MultiViewPayload` (1158), `MultiProjected*Dto` (1163–1247), `MultiCompetitionPayload` (1324), `MultiHitPayload` (1330), `MultiCellPayload` (1335) | `src/Ui/MultiApi.View.cs` |
| `MultiCommandAsync<TRequest>` (1007); record `MultiCommandPayload` (1319) | `src/Ui/MultiApi.Commands.cs` |
| `ReadMultiSavesAsync` (805); `MultiSaveAsync` (851); `MultiLoadAsync` (893); `MultiRecoverAsync` (918); `IsIdentityRefusal` (936); records `MultiSaveRequest` (1132), `MultiLoadRequest` (1134), `MultiSlotRequest` (1136), `MultiSlotInfo` (1138), `MultiSavesPayload` (1141), `MultiSaveOutcome` (1143), `MultiSavePayload` (1146), `MultiLoadInfo` (1148), `MultiLoadPayload` (1150), `MultiRecoverPayload` (1155) | `src/Ui/MultiApi.Saves.cs` |
| `PreviewMultiReviewAsync` (170); `SkipMultiYearsAsync` (197); `ReadMultiHistoryIndexAsync` (243); `ReadMultiHistoryYearAsync` (279); `ReadMultiStructureAsync` (346); `FenceOf` (358); `TryPreviewForView` (361); `DefaultSkipRefusalMessage` (376); `TryParseExactInt` (384); records `MultiReviewPreviewDto` (1249), `MultiPreviewReviewPayload` (1262), `MultiSkipStepDto` (1278), `MultiSkipPayload` (1280), `MultiHistoryYearRefusalPayload` (1302), `MultiStructurePayload` (1307) | `src/Ui/MultiApi.History.cs` |
| `internal static class MultiAreaPage` (1346–7525) | **removed.** Its constant values move to `src/Ui/multi/` per §2.4 |

The worker must classify **every** member found at `T0`. A member present at `T0` but absent from this table is placed in `MultiRoutes.cs` and reported for review; it is not silently dropped.

### 2.2 `src/Core/MultiArea/MultiAreaContinuation.cs` → command partial files

| Member(s) (start line at 5fe2fabe) | → Target file |
|---|---|
| `MarkCrop` (459), `UnmarkCrop` (483) | `MultiAreaContinuation.Commands.Crop.cs` |
| `AddPending` (507), `RemovePending` (533) | `MultiAreaContinuation.Commands.Fell.cs` |
| `AddPrune` (557), `AddScheduledPrune` (589), `RemovePrune` (602) | `MultiAreaContinuation.Commands.Prune.cs` |
| `AddClearUndergrowth` (630), `RemoveClearUndergrowth` (653) | `MultiAreaContinuation.Commands.Clear.cs` |
| `AddPlant` (679), `AddShelter` (683), `AddSpotClear` (687), `AddPlanting` (692), `RemovePlanting` (714) | `MultiAreaContinuation.Commands.Planting.cs` |
| `Approve` (739), `CancelApproved` (782), `Execute` (808–983) | `MultiAreaContinuation.Commands.Plan.cs` |
| Records and `MultiAreaAnnual` at file top (1–65); class header, **all fields**, constructors, reads (207–458), `Advance` (993–1155), Save/Load (`CaptureSave` … `DraftOrderCount`, 1165–1325), `ReadCanopyLight` … `HitTest`, and all helpers (1401–1572) | **stay** in `MultiAreaContinuation.cs` |

A section-banner comment that directly precedes the first member of a moved group moves with that group. Every new file starts with `namespace SitkaP1.Core;` and `using SitkaP1.Core.Phase1;`, matching the root file, then `internal sealed partial class MultiAreaContinuation`.

### 2.3 Embedded JavaScript sections → ES modules (S0c)

Line numbers are within `PlayerMultiApi.cs` at adea7af. After S0a the same text lives in `src/Ui/multi/multi.js` at line `n − 2914`.

| # | Banner (start line) | → Module | Layer |
|---|---|---|---|
| — | Header comments (2915–2917) | `main.js` (top) | L5 |
| — | `'use strict';` (2918), `(function () {` (2920), `})();` (7523) | **removed** (P2) | — |
| 0 | prelude constants, `state`, `byId` (2921–3017) | `store.js` | L0 |
| 1 | small DOM helpers (3018) | `dom.js` | L1 |
| 2 | lookups in the latest atomic composite payload (3093) | `store.js` | L0 |
| 3 | talking to the local session process (3272) | `api.js` | L2 |
| 4 | Phase 1 Save/Load (3379) | `dialogs/saves.js` | L4 |
| 5 | alerts, announcements and the re-read lock (3657) | `api.js` (the re-read lock is the fail-closed request gate) | L2 |
| 6 | Execute confirmation (3823) | `dialogs/execute-confirm.js` | L4 |
| 7 | rendering (3902) | `main.js` | L5 |
| 8 | the map: 1 SVG user unit = 1 metre (4081) | `map/renderer-svg.js` | L3 |
| 9 | status marks (4638) | `map/renderer-svg.js` | L3 |
| 10 | Interactive modes (4793) | `modes/interactive.js` | L4 |
| 11 | grid and legend (5141) | `map/renderer-svg.js` | L3 |
| 12 | Tree inspector, Prune box and Competition (5230) | `panels/inspector.js` | L4 |
| 13 | Competition and Fell draft (5383) | `panels/fell-draft.js` | L4 |
| 14 | Work Plan, Receipt, Results, Trees, Details (5723) | `panels/work-plan.js` | L4 |
| 15 | selection & Escape precedence (6125) | `main.js` | L5 |
| 16 | Work Plan commands (6376) | `commands.js` | L4 |
| 17a | Phase 1 Step 2: Skip / History drawer / Structure (6789–7341) | `panels/time-history.js` | L4 |
| 17b | `function wire()` (7342–7517) and bootstrap `initFloorDefs(); renderLegend(); wire(); reread(false);` (7519–7522) | `main.js` (named P1 exception) | L5 |
| new | Renderer contract (spec §7.3) | `map/contract.js` | L3 |
| new | `svgRenderer` adapter object (P5), appended after section 11 | `map/renderer-svg.js` | L3 |
| new | `export const hooks = Object.seal({ … })` (P4) | `store.js` (end) | L0 |
| new | hook assignments, placed immediately before the bootstrap statements | `main.js` | L5 |

Within each module, sections keep their original relative order (`main.js`: header, imports, 7, 15, 17b-`wire`, hook assignments, bootstrap). **The module names above are the default mapping.** Changing the mapping needs reviewer approval, and splitting a section beyond the 17a/17b exception is a stop condition.

### 2.4 Embedded constants → asset files (S0a)

| Source (at T0) | Value rule | → File | LogicalName | Route (unchanged) | MIME (unchanged) |
|---|---|---|---|---|---|
| `MultiAreaPage.Html` | spec §5.3 | `src/Ui/multi/multi.html` | `SitkaP1.Ui.multi.multi.html` | `multi` | `text/html; charset=utf-8` |
| `MultiAreaPage.Css` | spec §5.3 | `src/Ui/multi/multi.css` | `SitkaP1.Ui.multi.multi.css` | `multi.css` | `text/css; charset=utf-8` |
| `MultiAreaPage.Js` | spec §5.3 | `src/Ui/multi/multi.js` | `SitkaP1.Ui.multi.multi.js` | `multi.js` | `text/javascript; charset=utf-8` |

`SitkaP1.Ui.csproj` gains, in a new `ItemGroup` placed after the existing `wwwroot` group (which stays untouched): `<None Remove="multi/**" />` and three `<EmbeddedResource Include="multi/<file>" LogicalName="…" />` lines. In S0c the `multi.js` line is replaced by one line per module (`LogicalName` `SitkaP1.Ui.multi.<relative path with '/' as '.'>`). The gate fails if two `LogicalName`s collide.

### 2.5 RNG literals → `RngSalts` constants (S0b)

Constant names are proposals and are finalised in S0b review. Each value is **byte-identical** to today's literal.

| Site (at 5fe2fabe) | Before | After |
|---|---|---|
| `src/Core/AnnualRules.cs:87` | `"ADULT-MORT-" + before[i].TreeId` | `RngSalts.AdultMortality + before[i].TreeId` |
| `src/Core/MultiArea/Phase2MixedDensity.cs:108` | `"ADULT-MORT-" + before[i].TreeId` | `RngSalts.AdultMortality + before[i].TreeId` |
| `src/Core/MultiArea/Phase2Rules.cs:158` | `"PHASE2-MAST-" + speciesId` | `RngSalts.Phase2Mast + speciesId` |
| `src/Core/MultiArea/Phase2Rules.cs:159` | `"ADULT-MORT-GENERIC-" + treeId` | `RngSalts.AdultMortalityGeneric + treeId` |
| `src/Core/MultiArea/Phase2Rules.cs:160` | `"ADULT-MORT-GENERIC-QUOTA-" + areaId` | `RngSalts.AdultMortalityGenericQuota + areaId` |
| `src/Core/MultiArea/Phase2Rules.cs:161` | `"PHASE2-PJ-" + juvenileId` | `RngSalts.Phase2Juvenile + juvenileId` |
| `src/Core/MultiArea/Phase2Rules.cs:162` | `JuvenileKey(juvenileId) + "/vegetation"` | `JuvenileKey(juvenileId) + RngSalts.Phase2JuvenileVegetationSuffix` |
| `src/Core/MultiArea/Phase2Rules.cs:163` | `JuvenileKey(juvenileId) + "/browse"` | `JuvenileKey(juvenileId) + RngSalts.Phase2JuvenileBrowseSuffix` |
| `src/Core/MultiArea/Stage5Mast.cs:11` | `public const string MastKey = "STAGE5-MAST";` | `public const string MastKey = RngSalts.Stage5Mast;` |
| `src/Core/MultiArea/Stage5Mast.cs:15` | `$"STAGE5-PROMO-{areaId}-{column}-{row}-{year}"` | `$"{RngSalts.Stage5Promotion}{areaId}-{column}-{row}-{year}"` (same `InvariantCulture`) |

No other token on those lines changes. If OQ-6 (b) is answered "no", these files stay untouched, and `determinism-allowlist.txt` lists exactly these literal sites as allowed duplicates of `RngSalts` values.

---

## 3. Branch and merge procedure

### 3.1 Branches

| Branch | Base | Writer | Lifetime |
|---|---|---|---|
| `integration/2d-sim` | `T0` (owner-accepted consolidated Phase 1 baseline) | integrator only (merges) | long-lived 2D trunk; **never** merged to `main` |
| `feat/multi-2d-s0a-mechanical-moves-<YYYYMMDD>-v1` | trunk tip | S0a single writer | until merged |
| `feat/multi-2d-s0b-engine-seams-<YYYYMMDD>-v1` | trunk tip after S0a | S0b single writer | until merged |
| `feat/multi-2d-s0c-es-modules-<YYYYMMDD>-v1` | trunk tip after S0b | S0c single writer | until merged |

### 3.2 Merge procedure (one branch at a time)

1. **Trunk precondition:** the last gate run on the trunk tip returned 0 and the trunk is not FROZEN.
2. **Author:** pushes the feature branch and runs `build/2d-gate.sh` on its HEAD `H1` (author evidence only).
3. **Independent review:** a non-author route reviews `H1` against the spec and the packet review gate (RG-S0a/b/c) and returns APPROVED or NEEDS CHANGES.
4. **Integrator rebase:** `git fetch origin --prune`, then rebase the feature branch onto `origin/integration/2d-sim`, giving `H2`. If `H2 ≠ H1`, `git range-diff <old-base>..H1 origin/integration/2d-sim..H2` must show only patch-identical (`=`) entries. Otherwise the reviewer re-reviews `H2`.
5. **Gate:** `build/2d-gate.sh <evidence-dir>` on `H2` (add `--browser` for S0c and for any later UI-touching branch if the owner adopts that rule). It must exit 0.
6. **Merge:** on an up-to-date local trunk (`git merge --ff-only origin/integration/2d-sim`), run `git merge --no-ff <feature> -m "merge(2d-sim): <packet> — SPEC-multi-2d-s0-seam-extraction-V1; review <route/verdict ref>; gate <evidence id>"`.
7. **Post-merge gate:** run `build/2d-gate.sh` on the merge commit `M`. On non-zero, **do not push**; discard the local merge and return the branch to its author.
8. **Push:** `git push origin integration/2d-sim` (fast-forward only; never `--force`).
9. **Record:** the integrator writes the AgentWorkflow `INTEGRATION DECISION` block into the packet's GitHub-tracked handoff. The Manager updates `Docs/Project/coordination/active-tasks.md` (not part of any S0 branch).

### 3.3 Freeze-on-red

If any gate run on the trunk tip returns non-zero, the trunk is **FROZEN** (recorded by the Manager in coordination). While frozen, only (a) a repair branch or (b) `git revert -m 1 <merge>` on a revert branch may merge, each through the full procedure above. The trunk unfreezes only when a gate run on the new tip returns 0. History is never rewritten.

---

## 4. Gate wiring

- **Entry point:** `Tools/MultiRackSitkaP1Lab/build/2d-gate.sh [--base <ref>] [--browser] [--headless-only] <evidence-dir>`. Exit codes and stage order are in spec §4.2–§4.3.
- **Stage skeleton:** each stage is a bash function writing `GATE <stage> PASS|FAIL|NOT RUN` to stdout and to `<evidence-dir>/gate-summary.json`. The final verdict is computed from markers; any missing marker counts as FAIL.
- **Data inputs (committed, integrator-owned after S0a):** `build/2d-gate/frozen-anchors.json`, `protected-paths.txt`, `determinism-allowlist.txt`, `baseline-counts.json`, `s0a-multi-asset-identity.json` (field `mode`: `s0a-full` until S0c, then `s0c-css-plus-html-delta`), `s0a-move-table.json`; from S0c also `s0c-hook-inventory.json` and `s0c-renderer-callsite-map.json`.
- **Helper tools (committed, no packages):** `raw-literal.py`, `move-proof.py` (python3 standard library); `module-graph.mjs`, `js-reconstruct.mjs` (Node built-ins only); `selftest.sh` (bash, local clone into `TMPDIR`).
- **Existing suites invoked:** exactly the commands in spec §3.9, each with an evidence sub-directory under `<evidence-dir>`. The gate never edits these scripts, except that S0c owns the listed `ui-smoke-multiarea.sh` changes.
- **Environment:** `DOTNET_CLI_TELEMETRY_OPTOUT=1`, `DOTNET_NOLOGO=1`, `TMPDIR` outside the repository; `unshare -rn` around G1–G2 when available (spec §4.4). The protected listeners are recorded before and after with `ss -tln`.

---

## 5. Evidence and artifact paths

| Artifact | Path | Committed? |
|---|---|---|
| Gate script and data/tools | `Tools/MultiRackSitkaP1Lab/build/2d-gate.sh`, `Tools/MultiRackSitkaP1Lab/build/2d-gate/**` | yes |
| `/multi` byte-identity proof (gate input) | `Tools/MultiRackSitkaP1Lab/build/2d-gate/s0a-multi-asset-identity.json` | yes |
| Move tables / reports | `build/2d-gate/s0a-move-table.json` (committed); move-proof report summarised in the handoff, full report external | table yes; full report off-repo |
| S0c inventories | `build/2d-gate/s0c-hook-inventory.json`, `build/2d-gate/s0c-renderer-callsite-map.json` | yes |
| Trunk creation record | `Docs/Handoffs/MultiRack-Simulation-20261116/s0-seam-extraction/S0A-TRUNK-CREATION-RECORD-<YYYYMMDD>.md` | yes |
| Packet handoffs (author) | `Docs/Handoffs/MultiRack-Simulation-20261116/s0-seam-extraction/S0{A,B,C}-IMPLEMENTATION-HANDOFF-<YYYYMMDD>.md` | yes |
| Independent reviews | `Docs/Handoffs/MultiRack-Simulation-20261116/s0-seam-extraction/S0{A,B,C}-INDEPENDENT-REVIEW-<YYYYMMDD>.md` | yes |
| Safety-valve decision | `Docs/Handoffs/MultiRack-Simulation-20261116/s0-seam-extraction/S0B-SAFETY-VALVE-DECISION-<YYYYMMDD>.md` | yes |
| S0 closure | `Docs/Handoffs/MultiRack-Simulation-20261116/s0-seam-extraction/S0-CLOSURE-<YYYYMMDD>.md` | yes |
| Raw gate logs, screenshots, parity diffs, HAR/network logs | `/media/jer/Files/CCF/Verification/MultiRack-2D-S0/<packet>/<UTC stamp>/` (per `evidence/README.md`) | no (off-repo; referenced from the handoffs) |
| Local scratch | `Tools/MultiRackSitkaP1Lab/evidence/` (git-ignored) or `TMPDIR` | no |

---

## 6. Rollback plan per packet

All rollbacks use new commits on a revert or repair branch, merged through §3.2. No history rewrite and no force-push.

- **S0a.** Because S0a lands as commits A-C1…A-C4 inside one `--no-ff` merge, a defect in one part is rolled back with `git revert <A-Cn>` alone. For example, reverting A-C3 restores `PlayerMultiApi.cs` as one file while keeping the gate and the extracted assets. A full rollback is `git revert -m 1 <S0a merge>`, which returns the trunk to `T0` content and **removes the gate**. In that case the trunk stays FROZEN and interim checks use the T-00b baseline commands manually until S0a re-lands.
- **S0b.** `git revert -m 1 <S0b merge>`. The RngSalts substitutions return to the same-valued literals, `Advance` becomes inline again (or is unchanged if RETAINED), and the interface file is removed. The contract text stays in the spec. Phase 2B engine lanes started under the S0b unlock **pause** until S0b re-lands and then rebase.
- **S0c.** `git revert -m 1 <S0c merge>`. This restores `multi.js`, the classic script tag, the old asset route and the old harness lines. The identity JSON's `mode` returns to `s0a-full`, so the S0a byte-identity check applies again automatically. Phase 2A and 2B UI lanes pause until S0c re-lands.

---

## 7. Dependency and unlock note

```text
E-0 (spec APPROVED + OQ answers + T0 owner-accepted + T-00 trunk + Manager lane registration)
  └─► S0a ──► S0b ──► S0c
              │        └─► unlocks Phase 2A UI and Phase 2B UI coding (parallel lanes)
              └─► unlocks Phase 2B Economy ENGINE coding (parallel lane)
```

Unlocks are permissions for *starting* parallel coding under each phase's own approved spec and lane admission. They are not approvals of that work. Phase 2B persistence additionally waits for Phase 2A's C-V3 implementation (spec §12 item 4).

---

## 8. Risks

| # | Risk | Mitigation |
|---|---|---|
| R1 | Windows checkouts with `core.autocrlf=true` change line endings and break byte identity | Reference hashes come from Git blobs, never the working tree; `src/Ui/multi/.gitattributes` `* -text`; the gate checks LF-only, no BOM and no final newline for the three S0a files; the authoritative gate host is the WSL2/Linux checkout |
| R2 | Static initialiser order across partial files | All `/multi` static fields stay in `MultiRoutes.cs` in original order (spec §5.4); the move proof enforces it |
| R3 | M1/M2 equivalence subtleties in `Advance` | Only two pre-identified relocations; triggers T1–T6; automatic RETAIN with a reviewed record (spec §6.3) |
| R4 | ES-module cycles or TDZ at module evaluation (an upward `const` reference) | Layering plus the `hooks` registry; the only top-level statements are the bootstrap; any cross-module `const` initialiser reference that points upward is a stop condition |
| R5 | `vm.SourceTextModule` needs `--experimental-vm-modules` and may vary by Node version | The Node version is recorded at T-00b; if unavailable, stop and ask (no package may be added) |
| R6 | The browser tier depends on a Windows host (Edge + Windows node via WSL2) | Tier B is a separate, explicit stage; when absent the gate returns 3 or 5, never PASS; S0c cannot merge without it |
| R7 | `T0` differs from adea7af (Step 2 R2/R3 repairs) | All tables are keyed by member or banner names; line spans are regenerated at `T0`; a missing member is a stop condition |
| R8 | Module requests carrying `Origin` | The existing guard accepts the exact admitted origin; the S0c headless and Edge runs assert 200 on every module route (no 403) |
| R9 | `p0-isolation.sh` needs `strace`; long gate runtime | Prerequisite check in G0 (exit 3); runtime is accepted cost; the gate never touches the protected listeners |
