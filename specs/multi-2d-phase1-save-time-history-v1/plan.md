# 2D Phase 1 — Plan

**Status: APPROVED PLAN — owner 2026-11-24 13:53 UTC.** Execution is allowed only under phase-specific task packets, single-writer permissions and physical Ops locks; the plan itself is not an occupied source lane.

## Provenance and sequencing

- Engine baseline: `feat/multi-phase2-oak-beech-20261124-v1` @ `cf6fe7a52aa096e824827c568f43fe05724dcce9`, Phase2 independent review/qualifications reported PASS, not yet merged to main.
- Concurrent map: `feat/multi-map-integration-pass1-20261124-v1` descendant of Phase2, 5 commits ahead at this inspection. Independent Opus review of `1f131826` reports APPROVED (G2/G3 PASS), still not integrated/merged; later passes or local writes may exist outside the branches inspected. Inspect live coordination and worktree locks again at actual dispatch.
- No source writer or physical lease created by these documents. Source branch/HEAD is chosen only after owner approval and Ops admission.
- Standard test forest / simple forest maker / finances / labour are later phases; do not pull them into this work.

## Phase 1A0 — independent parallel slice (no map/shared source changes)

1. Confirm approved spec, active-task ownership, compile-linked file read scope, physical Ops lease and clean distinct worktree. Freeze exact Core source SHA as input. Do not start if map writer's actual exclusive write scope includes any proposed file.
2. Author under NEW `src/Core/Phase1/` paths only: versioned save envelope, typed state contract, strict deterministic codec, in-memory consistency validator, size/corruption limits, safe file-slot abstractions. Reuse existing `StrictJson` contract where appropriate without editing it; use only authorised read paths. Include explicit types for Phase2 polymorphic planting orders and optional Stage4/Stage5/Phase2 state.
3. Under NEW test/evidence paths only, prepare deterministic exhaustive sample state round-trip and malformed-input/corruption checks. If existing test runner registration or `csproj` edits would be needed, stop at the boundary: no claim of runnable tests until after join, or obtain a new tested standalone runner grant.
4. Independent source review for correctness, missing save fields and compile compatibility. No integration with the running map; publish a GitHub Markdown handoff and code SHA. Do not assert player-visible save/load yet.

## Phase 1A1 — engine join, gated after map closure and fresh collision check

1. Rebase/cherry-pick only reviewed A0 pieces onto exact approved map + Phase2 integration candidate. One owner for changed shared files.
2. Add single-lock capture and restore into the authoritative `MultiAreaContinuation`, including derived session reset, full root validation and new fence; wire safe save/load commands through adapter/server.
3. Add yearly history-summary recording at annual commit and a read-only paged history query. Preserve existing event/account provenance and immutable annual sequence.
4. Add bounded multi-year controller that calls the actual annual command for each year; exact pause/stop matrix signed before writing. Never bypass approval or use work-cancel consent implicitly.
5. Run Tier 1 then affected Tier 2 tests: save/load, negative corruption, annual parity, token staleness, untouched frozen anchors. Independent review before integration.

## Phase 1B — map/screen presentation, later and exclusive

1. Add minimal `Save`, `Load`, `Advance 1 Year`, `Skip to Review` and `History` controls to current `/multi`, using engine/API data only.
2. Make action previews and refusals clear; keep year-by-year history paged. Check map map/cell/selection source fences and existing payload budget at 1280×720; require no extra engine calculations in presentation code.
3. Independent UI/API review, tests, qualified local runtime test, owner playtest and G4 integration separately. Do not infer acceptance from compilation.

## Stop conditions and risk

Stop only the affected task if there is an overlapping writer/file, unsigned model/save option, missing lock, changed integration baseline, hidden alteration of official ecology, incomplete stored root, corrupted save, missing annual parity or unauthorised old-save/Unity exposure. Report blocker and resume nonconflicting docs/review. No Windows build.

**Current permission:** user-approved Phase 1 end-to-end scope and six A choices. 1A0 is eligible for admission after exact source packet, physical lane and fresh active-tasks inspection. 1A1/1B are HELD until map/shared-file ownership clears, review and separate integration approval. No source task has yet been admitted by this document.
