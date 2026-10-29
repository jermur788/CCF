# S1-C T-06 implementation handoff — qualification pending

Task: #147 / approved S1-C FR-7 and AC-5. Worker: T3 Code / Codex gpt-6.1-sol, medium reasoning, configured Codex provider. Primary implementation only; Manager acceptance and integration are not claimed.

Branch: `feature/s1-c-t06-prior-treatment-guidance-v1`.
Worktree: `/media/jer/Videos/CCF-S1C-T06`.
Original exact start: `cff79f0f35d79bb2db723d0b923658c437ebdc7f`.
Preserved implementation: `5bb3acc0d6c13b06d2d7f159c0fefea2b3ab4529`, pushed unchanged before reconciliation.
Manager-authorized normal merge input: `3ea4af8b9464690f3b04b3d702c268c70a0558f7`, containing final approved T-04 production `633775fd8650db3cf3482e550d106a0febd7b9cb`.
Exact merged production candidate: `d959e0397c80bc7a9d7a707c8d4f437f946231f4`, pushed. Merge was clean; no rebase/amend/reset/stash/discard or conflict resolution. Both the final T-04 DERIVED Management History intervention-count heading and T-06 guidance remain present. Canonical context sources in this lineage last changed at `63e95ba2175f7e239a278c616681b286719821f8`; live ownership was read separately from GitHub.

## Player-facing result

Annual Review offers an expandable **PRIOR TREATMENT · L4 MANAGEMENT GUIDANCE** card for successful recorded treatments in the reviewed year. It names one recorded example per biological treatment, labels its historical source RECORDED, and asks what to inspect now before judging any further work. It does not choose the forestry decision or claim a treatment caused current conditions. Full recorded targets remain in Work done.

A recorded valid event map square offers **Look at recorded map square**, reusing the existing map/HUD waypoint route. Missing event cells show NOT RECORDED and offer no waypoint, even when the current world still contains a matching tree/plant. Position-only records remain readable in the existing drill-down; this optional area navigation does not infer a cell from them.

## Changes and invariants

Only two production files differ from approved pushed base `3ea4af8b`:

- `Assets/ForestPrototype/ScenarioOne/ManagementHistoryText.cs`: read-only L4 copy selection and recorded-cell waypoint guard.
- `Assets/ForestPrototype/UI/AnnualReviewView.cs`: expandable guidance in the existing review; existing history query and waypoint APIs reused.

No serialized Unity file, meta, scene, prefab, material, package, ProjectSettings, save/schema/validator, simulation/ecology/economy, curriculum/lesson or MFM production changes. No assets added/deleted. Every other Assets/Packages/ProjectSettings Git object is identical to `3ea4af8b`; protected file SHA-256 values are in `merged-offline/scope-audit.json`. Frozen Reference Future bytes and deterministic/save code are untouched. No protected anchors are intentionally changed or rebaselined.

No new L2 values are introduced: present condition is left for actual forest inspection. Existing CURRENT/DERIVED/RECORDED/not-recorded distinctions remain intact. The reused map route retains its existing local map-waypoint tutorial preference; no new persistence or teaching ID is added. Review browsing/expansion must not acknowledge the latest review.

Full guidance/source/wording/waypoint matrix: [COPY-AUDIT.md](COPY-AUDIT.md).

## Verification so far

Durable evidence: `/media/jer/Files/CCF/Verification/S1C-T06-Guidance-20261029/`.

Merged production candidate offline results:

- T-06 guidance: **53 PASS**, including six biological treatment types, failed/cancelled/None/null, unknown shelter provenance, missing/invalid/zero cell, no live grid, and read-only checks.
- T-04 provenance: **56 PASS**, including final DERIVED Management History heading assertion.
- T-03 drill-down: **72 PASS**.
- Per-year history query: **55 PASS**.
- Source/copy/scope audit: PASS; no unsupported causal/prescriptive claims or new persistence/simulation change.

Earlier interrupted Unity launch on preserved `5bb3acc0` is **not** qualification evidence. It was stopped on the Manager repin instruction; harness/meta/licence copy removed, known import side effects restored, no disk saves created, U1 restored clean to original detached `c8cfae8c4025cec6e960a050edf4bfde2a981de5`, normal lock released.

## Pending qualification / operational blocker

The complete merged-candidate Unity gate remains required: two-resolution (1280×720 / 1920×1080) felling/planting guidance, faithful event-cell waypoint plus real player/HUD arrival via scripted relocation, missing-location case with no invented waypoint, earlier-year read-only/acknowledgement checks, T-04 provenance rendering and affected T-02/T-03/T-04 regressions. Commands/expected behavior are in [T06-RENDERED.md](../../../Tools/Verification/S1CManagementHistory/T06-RENDERED.md).

As of 2026-10-29T22:03Z, #74 released Unity after its fresh partial import crossed the mandatory SSD floor. Lock is free/no Editors, but ZX20 is about **14.73 GiB free**, below **15 GiB**. #147 did not delete another task's retained cache or bypass the floor. A request is pending to remove only the inactive regenerable `/media/jer/ZX20/CCF-Q10/Library` (5.7 GiB), preserving all sources/evidence, before continuing.

No Windows build. No success token or READY FOR MANAGER REVIEW claim until the required runtime/rendered gates pass. Existing unrelated Primary modifications and other lanes are preserved.

DASHBOARD SYNC: NOT VERIFIED; final publication/readback remains pending with qualification closure. No generated dashboard state edited.
