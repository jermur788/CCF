# MULTI-STANDMAP-2D-SELECTED-UI-V1 — Implementation plan

**Status: APPROVED FOR BOUNDED FRONT-END IMPLEMENTATION by Manager #863, acting on the owner's explicit 2026-11-20 14:38 UTC delegation.** Companion to `spec.md`. This approval grants no individual agent a physical/source lease, merge or protected file changes.

## 1. Preflight and source inventory

Refresh live main coordination board and physical custody. Start exact qualified #849 source `4c42e2012a4c1bee3a422e41a58535343d63ac9f` in **fresh, separately registered clean source lane**; check entire `src/Ui/wwwroot/` and linked `PlayerApi.cs`/`PlayerWorkPlanAdapter.cs` **read-only** to identify front-end contract. Reuse real #849 map scene, exact hit-test/fence/PlanId/Execute semantics. Review prior CSS 200%-zoom regression, independent G2 handoff and planned tests before proposing changes. No protected P0 rebuild or Unity.

## 2. Freeze owner design reference (dependency)

Obtain owner-designated actual generated **comparison image #1** (the 2026-11-20 13:52 comparison, visual selection at 13:56); store a permanent, licensed, SHA-256 logged reference in a separate later reference-asset grant or this spec's `references/` **only after permission**. Store image #3 as a separate **legend-scope-only** supporting reference if made available. State clearly “generated concept, not Unity screenshot”. Use actual file bytes; do not recreate/reinvent an image, silently substitute other screenshots or claim pixel comparisons from prose. If unavailable, mark visual matching **NOT VERIFIED** and leave final reference-fidelity gate OPEN; proceed only with approved text-grounded front-end authoring and do not assert visual fidelity or owner acceptance. Obtaining/hash-freezing the exact reference remains a Manager-controlled follow-up.

## 3. Proposed minimal source file allowlist (subject to writer preflight and approval)

**Primary expected writable UI files, exactly existing from #849:**
- `Tools/MultiRackSitkaP1Lab/src/Ui/wwwroot/index.html` — structure/drawers/semantic controls;
- `Tools/MultiRackSitkaP1Lab/src/Ui/wwwroot/app.css` — theme, responsive desktop grid/zoom/mobile;
- `Tools/MultiRackSitkaP1Lab/src/Ui/wwwroot/app.js` — view rendering, status glyphs, reason dropdown display-only-to-string translation, pan/zoom UI and focus.

**Potential test file** `Tools/MultiRackSitkaP1Lab/tests/ui-smoke.sh` — **read-only by default**; request a separately approved narrow tests-writer grant ONLY if the selected UI changes legitimately require revised DOM bindings or new tests, preserving stronger/previous semantic safety assertions. Never weaken 200% or page safety tests simply for green.

**Explicitly read-only/compile-linked protected:** `src/Ui/PlayerApi.cs`, `src/Ui/Program.cs`, `src/Core/**`, `.p0ref/**`, `src/Core/SitkaP1.Core.csproj`, `fixtures/**`, other Tools/MultiRackSitkaLab/**, Assets/**, ProjectSettings/**, `specs/**` outside this draft and `Docs/Project/**`. Source code privilege does not follow from inspecting an API. A material backend/core change requires STOP and a new approved packet and ownership grant.

**Proposed branch name after approval:** `feat/multi-868-selected-standmap-ui-v1`, **from #849 exact source commit**, not main or either mock-prototype branch. Worktree/host/actor **UNASSIGNED**; this docs packet confers no physical lease.

## 4. Implementation sequence, narrow not broad redesign

1. Inventory exact existing UI bindings and define status/legend map: current engine statuses + selected overlay, clear one-species legend. Write screenshot/layout baseline, no source.
2. Build desktop shell and dominant forest map with compact data rail, bounded side Inspector/Work Plan. Keep original full tree table, history, technical source info in accessible secondary panels; no removal of evidence.
3. Add grouped D-pad/zoom/Fit, preserving existing pan mapping and correct hit-test geometry; keep overlap resolution and a keyboard option.
4. Add predefined reason select + Other; translate to exact existing recorded reason string, preserve current validations, duplicate blocking, approval/execute and re-read fences.
5. Add richer canopy/ground visuals only from existing geometry, deliberate semantic glyphs and truthful two-dimensional legend. Protect target selection, north/scale, stump/canopy gap, 100 Sitka state. If aesthetic cannot match concept without invented data, document bounded deviation.
6. Iteratively render at desktop 1280×720 and 1920×1080; adjust layout and focus. Reflow at real browser 200% zoom, 640/360 px. Use screenshots of *real rendered candidate*, not AI mockups, for side-by-side comparison against frozen design concept. No new dependencies.
7. Tier 1 fast checks during local authoring: JS parsing, layout core actions, exact server API smoke. Perform one **Tier 2 FEATURE QUALIFICATION** on the exact candidate (no full integrated milestone), including regression described in spec §6. Independent nonauthor review must issue APPROVED/NEEDS CHANGES/REJECTED. Do not conflate CSS/DOM tests with actual user-playable behavior.
8. Publish complete durable Markdown handoff to #868 and #863, with source SHA, allowed diff, honest real screenshots and browser/input evidence, P0 frozen hash/fixture preservation, tests, identified deviations, no Unity/build/main merge. If no real browser access, report held visual/owner gates, not PASS.

## 5. Acceptance, integration and stopping rules

**Source author stop:** any requirement for changing `PlayerApi.cs`/core, rebuilding protected P0, new growth, new tree species, new save/finance, new generated fictitious canopy, or conflicting physical/source lane must be reported as a blocker. Stop only that action, continue authorised UI/design work. Main/G4 integration requires separate authority and reviewed exact candidate. No Windows Player build before Scenario One owner playtest and explicit approval.

**Approval resolution:** Owner explicitly delegated four-file approval to Manager #863 on 2026-11-20 14:38 UTC. Manager approved image #1 style direction + image #3 legend semantics, compact desktop/reflow requirements, the exact five dropdown labels in `spec.md`, and this production plan. Manager may now dispatch one local-capable writer *after real lock/lease preflight*, without DeepSeek. The actual generated image asset is not frozen; no worker may claim pixel-specific reference fidelity until a verified reference is archived and independently compared.
