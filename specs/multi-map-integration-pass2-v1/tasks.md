# MULTI Map Integration Pass 2 — Bounded Tasks

- **Spec ID:** SPEC-multi-map-integration-pass2-V1
- **Spec status:** IMPLEMENTING
- **Task list owner:** Primary Implementer (Gemini 4 Neon)
- **Last updated:** 2026-11-24

## T-01 — Commit 1: Pass 2 spec quartet
- **Goal:** Author self-contained `spec.md`, `plan.md`, `tasks.md`, and unticked `checklists/requirements.md` under `specs/multi-map-integration-pass2-v1/`.
- **Scope:** `specs/multi-map-integration-pass2-v1/**`.
- **Verification:** Zero `TBD`; all checklist rows (`MI-P2-01`..`MI-P2-06`, `MI-REG`, `MI-P2-REVIEW`) unchecked `[ ]`.

## T-02 — Commit 2: Six fenced HTTP write endpoints
- **Goal:** Wire `POST api/multi/{clear/add, clear/remove, plant/add, shelter/add, spot-clear/add, planting/remove}` in `src/Ui/` against the single shared `MultiAreaContinuation` session with zero edits to `src/Core/MultiArea/**`.
- **Scope:** `Tools/MultiRackSitkaP1Lab/src/Ui/{Program,PlayerApi,PlayerMultiApi}.cs`.
- **Verification:** Clean build (`0/0`) of `.sln` and `SitkaP1.Ui.csproj`; strict camelCase (`xM`, `zM`, `speciesId`, `orderId`, `column`, `row`), `16 KiB` cap, `400`/`409`/`500` semantics with fresh `view + stage4 + ecology`.

## T-03 — Commit 3: Interactive modes, drawers, Work Plan panel, map previews & Escape order
- **Goal:** Implement the 4 modes (`Select Tree`, `Inspect Cell`, `Plant`, `Spot Clear`), provisional `Draft` previews (snap OFF), `PJ` sapling `Queue 8-year Shelter`, `Inspect Cell` clear/remove buttons, Planting Drawer with sequential `Plant → Shelter → SpotClear` fence-chaining and honest `409` partial disclosure + one-click remove, `Spot Clear` drawer, full 6-verb Work Plan panel + Execute/Consent dialogs, pending/approved/executed map rendering (keeping N-6 stump `X`), and `Escape` order preserving draft Fell ticks.
- **Scope:** `Tools/MultiRackSitkaP1Lab/src/Ui/PlayerMultiApi.cs`.
- **Verification:** Clean build (`0/0`); `node --check multi.js`; zero `data:image/webp`; zero client-side forestry maths; single-row `.workspace-head` preserving R3 fold at `1280×720`.

## T-04 — Commit 4: Core/API tests & smoke witness (with Pass 1 N-3/N-4 restoration)
- **Goal:** Add `MapIntegrationPass2Tests.cs` covering commitment-gate proofs, all refusal codes, sequential queueing, and pending/post-Execute payload budgets; extend `ui-smoke-multiarea.sh` with HTTP + FakeDOM Pass 2 coverage and restore Pass 1 `N-3`/`N-4` assertions.
- **Scope:** `Tools/MultiRackSitkaP1Lab/tests/**`.
- **Verification:** Clean builds (`0/0`); Core suite (`≥ 251` baseline + new tests, 0 failures, identical on 2 runs); all 9 frozen anchors unchanged; `ui-smoke-multiarea.sh` passing.

## T-05 — Commit 5: Real Edge verification matrix & durable handoff
- **Goal:** Run clean-gate builds (`--no-incremental --disable-build-servers`), 2 identical Core suite runs, 9 anchor verifications, raw byte payload measurements (pending & executed, Y0 & Y+5, both experiments), real headless Edge CDP matrix (`1920×1080` and `1280×720`, `ma-1` and `ma-1-mixed`) including visual inspection of post-Execute `PJ` sapling, active 8-year shelter, and `0.564 m` spot-clear ring; verify server stop, protected listeners, clean `git status --short`; commit durable handoff and push task branch.
- **Scope:** `Docs/Handoffs/MultiRack-Simulation-20261116/map-integration-pass2/MULTI-MAP-INTEGRATION-PASS2-NEON-IMPLEMENTATION-20261124.md`; raw evidence archived externally in `/home/jer/Development/CCF-Multi-MapIntegration-Pass2-Evidence-20261124` (mode 700).
- **Verification:** Clean `git status --short`; task branch pushed only; zero PR/merge/force-push/tag; zero checklist rows ticked.
