# TASK PACKET — S1-C Review / Management History — Implementation 02

**Project:** CCF  
**Manager:** ChatGPT Game Dev  
**Task:** `S1-C-IMPL-02 — Annual Review year browsing requalification and query-layer wiring`  
**Task class:** APPROVED SPEC IMPLEMENTATION  
**Routing:** dedicated Game Dev implementation chat; DeepSeek preferred for bounded Unity/UI implementation and Tier-2 evidence; Claude acceptable as independent reviewer  
**Branch:** `feature/s1-c-history-ui-v1`  
**Base:** `808d8165d242da7632f37fea5c1d7355fa59279c` (T-01 implementation plus published handoff)  
**Approved spec:** `specs/scenario-one-review-history-v1/` on canonical `main` — APPROVED

## Manager verdict on dependency T-01

`S1-C-IMPL-01 — per-year history query foundation` is **APPROVED FOR CONTINUATION** at implementation HEAD `3f0f94fc3af48ea70863e62c1ae38f9b57a58ac7` / published branch HEAD `808d8165d242da7632f37fea5c1d7355fa59279c`.

This verdict is task-level only. T-01 is not separately integrated or feature-qualified; this branch continues from it so the complete S1-C slice can be reviewed and integrated coherently.

## Why this packet exists

The inherited lineage already contains partial Annual Review year navigation in `AnnualReviewView.cs` (Earlier year / Later year controls and `viewYear`). Do **not** rebuild that UI greenfield.

However, the existing view still reconstructs historical relationships directly from `AnnualReports`, `ManagementEvents`, `WorkOrders` and `EcologicalSnapshots`. T-01 now provides the approved read-only per-year contract through `ScenarioOneManager.HistoryYears` / `ScenarioManagementHistory`.

T-02 therefore means:

**adopt + requalify the inherited year browser, wire it to the T-01 query layer, and close only the approved T-02 gaps.**

## Goal

The player can open any completed year's Annual Review from the existing Annual Review surface, with the latest year shown by default, while the acknowledgement gate continues to apply only to the current/latest review.

The UI must consume the T-01 query result rather than independently rejoining raw historical records.

## Preflight

Before editing:

1. refresh live `Docs/Project/coordination/active-tasks.md` from `main`;
2. verify exact branch/HEAD/worktree and preserve unrelated dirty work;
3. confirm no current writer owns `AnnualReviewView.cs` or any other file you need;
4. do not touch Rack V2-owned files, species assets, save schema or ecology/economy mechanics;
5. acquire the machine-wide Unity lock before any Unity execution and release it afterwards.

If current live ownership conflicts with this packet, stop with `OWNERSHIP CONFLICT` before editing.

## Existing implementation to preserve where valid

Inspect and retain compatible behaviour in at least:

- `Assets/ForestPrototype/UI/AnnualReviewView.cs`;
- `Assets/ForestPrototype/UI/ScenarioOneUiRoot.cs` only if wiring is genuinely required;
- `ScenarioOneManager.History.cs` / `ScenarioManagementHistory.cs` from T-01;
- current `annualReviewSeen`, `NeedsAnnualReview`, completion-review and teaching-review acknowledgement semantics;
- existing completion, first-intervention, storm, trend and objective presentation unless T-02 wiring requires a narrow compatibility change.

The existing buttons and `viewYear` state are candidate baseline, not proof T-02 is complete.

## Required implementation

### 1. HistoryYears is the year-navigation authority

Use `ScenarioOneManager.HistoryYears` as the completed-year list and selected-year record source.

The UI must not reconstruct the selected year's event/order/snapshot relationships by querying raw lists again where `YearHistoryView` already supplies them.

At minimum:

- navigation years come from `HistoryYears`;
- selected annual report comes from the selected `YearHistoryView.Report`;
- selected resolved-work/event/snapshot context consumed by the T-02 cards comes from that same `YearHistoryView` where available;
- historical selection remains deterministic when a defensive/missing-record case occurs.

Do not expand into T-03 intervention drill-down or T-04 full provenance labelling.

### 2. Latest-year-first behaviour

Opening Annual Review shows the latest completed year.

When a new completed year appears, the view returns to latest rather than silently remaining on an older year.

Earlier/later controls may remain the existing button design if they satisfy the approved spec. Do not create a second history panel.

### 3. Acknowledgement gate must not change

Historical browsing is read-only presentation.

- `annualReviewSeen` / current review acknowledgement semantics remain unchanged;
- browsing an older year must not acknowledge the current year;
- acknowledgement controls must not appear as an actionable gate for an older year;
- completion-review and teaching-review acknowledgement behaviour must remain compatible.

If satisfying T-02 appears to require changing gate semantics, stop with `SPEC BOUNDARY`.

### 4. Same Annual Review structure

Historical years use the existing Annual Review surface/card hierarchy rather than a new analytics screen.

Keep trends as the existing simple compact strips. No map overlay or analytical table is authorised by T-02.

Historical cards must not show today's live-only state under an old year. Preserve the existing guard that omits live browse/protection/current-state-only details from historical years, or replace it only with a more accurate query-layer equivalent.

## Owned files / allowed changes

Primary ownership:

- `Assets/ForestPrototype/UI/AnnualReviewView.cs`;
- `Assets/ForestPrototype/UI/ScenarioOneUiRoot.cs` only for narrow required wiring;
- bounded T-02 verification under an appropriate `Tools/Verification/S1CManagementHistory/` or UI verification path;
- `Docs/Handoffs/S1-C-Implementation-02/HANDOFF.md`.

Small history-query compatibility edits are allowed only if a demonstrated T-02 defect requires them and they preserve the T-01 contract. Document any such edit explicitly.

## Must not edit

Do not edit:

- Rack Planning & Field Marking V2 files;
- save schema/version or `ForestSaveData.cs`;
- objective achievement persistence (T-05);
- ecology, growth, regeneration, browsing or storm mechanics;
- economy calculators/prices;
- species assets or serialized scene/prefab YAML except a narrowly necessary UI document change explicitly justified in the handoff;
- curriculum/lesson sequencing;
- Multi-Scale Forestry Management;
- Reference Future archive/content/hashes;
- Windows build.

Do not implement T-03 intervention drill-down, T-04 provenance pass, T-05 objective-year persistence, T-06 guidance, or T-07 contract work in this packet.

## Verification

Use proportionate Tier 2 feature verification for this bounded UI slice.

At minimum prove on the exact tested HEAD:

1. a session/fixture with at least three completed years exposes all three through the Annual Review year browser;
2. Annual Review opens on the latest year by default;
3. navigate to an earlier year and back to latest without changing simulation/save state;
4. selected historical content is sourced through `HistoryYears` / `YearHistoryView`, not independently rejoined by UI code;
5. an older-year view does not show/trigger the current-year acknowledgement action;
6. current/latest review acknowledgement still blocks advancement exactly as before until acknowledged;
7. completion-review and teaching-review acknowledgement behaviour is not regressed;
8. no-work and work years both render coherently;
9. rendered Annual Review is legible at 1280×720 and 1920×1080;
10. compiler/import is clean and no unrelated verification baseline is weakened.

Re-run the T-01 offline history check after any history-query compatibility edit. If T-02 touches only UI code, record that T-01 source was unchanged and retain the existing T-01 evidence.

## Handoff

Publish:

`Docs/Handoffs/S1-C-Implementation-02/HANDOFF.md`

Include:

- exact base and final HEAD;
- files/symbols changed;
- which inherited year-browser behaviour was adopted versus changed;
- how `HistoryYears` / `YearHistoryView` became the historical data source;
- acknowledgement-gate proof;
- verification commands, Unity lock ownership and results;
- 1280×720 and 1920×1080 rendered evidence locations;
- failures/unresolved blockers;
- recommendation for T-03 and T-07 continuation.

## Exit

`S1-C YEAR BROWSING IMPLEMENTED / REQUALIFIED — READY FOR MANAGER REVIEW`

Do not claim full S1-C feature qualification, integration, spec verification or completion. This packet closes T-02 only.