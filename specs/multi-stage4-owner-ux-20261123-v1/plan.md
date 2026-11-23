# CCF Multi Stage 4 Owner UX Repair — Implementation Plan
**Status:** DRAFT, subject to approval with `spec.md` and the checklist.  
**Coordinator:** DeepSeek orchestration; **Writer:** Claude Opus 5.5; **Reviewer:** GPT-6.1 Sol independently.

## P0 — Source and locks
Read exact candidate `e7c1a57d`, current `origin/main` coordination board, D-091 grants and attached continuation handoff `db24a542`. Confirm current candidate HEAD, UI entry points and outstanding F-2. DeepSeek should use a fresh named implementation branch/worktree off `e7c1a57d`. No writing in the reviewed candidate lane or independent reviewer lane. Record exact read allowlist, write allowlist, author identity, physical lease, stop conditions and branch in the actual packet.

## P1 — Convert this pack into canonical approved spec quartet
Create canonical four-file spec under a unique non-colliding `specs/multi-stage4-owner-ux-20261123-v1/` path and corresponding task handoff (different from ecology #900). Link to the owner-supplied screenshots/descriptions and this full artifact. Approve through actual owner G5/authorised spec process. Phase A approval must not implicitly include phase B simulation history changes.

## P2 — One small UI-only production increment (Phase A)
Opus 5.5 implements UX-01–UX-09 as a coherent interaction-flow repair in `MultiAreaPage` HTML/CSS/JS in `PlayerMultiApi.cs`. Prioritize **tree selection and no scroll first**: compact inspector, map ranks, Escape/×, then nav and disclosure. Next implement a continuously calibrated *visual* coral-peach layer from unaltered engine light values, then ground inspector/mode. Include F-2 truthful unavailable state within approved UI scope. Do not create new Core functions or touch `PlayerApi.cs` / `Program.cs`.

A practical presentation mapping is `(lightIndex ∈ 0..1) → [opacity, colour/brightness]`, e.g. `alpha = min(0.65, 0.65 × L^1.2)` and muted coral→pale peach interpolation; **this is only an example for visual tuning, not a numerical simulation law**. Ensure a strictly positive (however tiny) alpha for any `L>0`, and zero at `L=0`, no artificial cutoff. Test low intensities at L=0, 0.01, 0.05, 0.5, 1 and verify forest readability. Do not modify numeric light values.

## P3 — Targeted qualification
- Static source review of event propagation, ESC precedence, accessible focus, SVG hit precedence, IDs unaffected by artistic repositioning, Work Plan draft vs queued state, area/zoom preservation and F-2 409.
- Existing solution/UI build using pinned .NET 8.0.134 and external TMPDIR, five frozen anchors and `.p0ref` hash; no unapproved compiler/task expansion.
- Existing tests and narrow new UI interaction tests; compare baseline triaged six multi-area smoke failures and four single-area failures without silently changing them.
- Real OS-assigned loopback server and real Edge CDP with no protected listener takeover; both **1280×720 and 1920×1080**, map legend shown, A/B, Light ON/OFF, normal trees plus all 5 rank toggles, Escape, prune disclosure, Work Plan approval/Execute unaffected.
- Screenshots of initial map, selected competitor selection above fold, several map ranks chosen and matched inspector, closed inspection, Light Guide low/high gradient, ground inspection. Two viewport sets where useful; keep existing AC-9 per-image/set cap if matching same acceptance artifact policy. Check no horizontal page overflow, no CSP/console errors, server 5xx, token exposure, anchor drift.
- `APPROVED/NEEDS CHANGES/REJECTED` independent GPT-6.1 Sol review of exact candidate. Sol is NOT a helper in the Opus writer thread and cannot approve its own work.

## P4 — Owner playtest before closure
Provide a fresh capability URL privately (never commit it). Owner personally tests full five-rank Fell flow, Escape and compact controls; checks coral-peach overlay aesthetic. Treat owner feedback as acceptance or change request. Reviewer approval does not equal owner acceptance or integration.

## P5 — Phase B separate decision and scope
DeepSeek authors a focused extension to the approved quartet or separate approved spec for UX-10 (actual pre/post Fell and yearly light comparisons). Capture policy, model-version invalidation, identical cell-grid alignment, multiple years/interventions, failure/409, no-work comparator if relevant, memory/performance and save contract. Request owner G5 approval for the exact snapshot/data approach and separate D-091 writer grants, then hand to **Opus 5.5 as single implementer**, independent **Sol 6.1 review**, owner playtest. Prefer B1 session-local actual before/after first; B2 counterfactual only as separately approved feature. If Phase B is not approved, close Phase A honestly and keep B blocked, not claimed delivered.

## Stop conditions
Need to modify Core, PlayerApi, Program, historical saves, fixtures, Unity, `main`, protected references; conflicts with live physical owner or missing lease; any anchor/P0 drift; owner design conflict; real browser unavailable for required tests; Phase B requires unapproved history/persistence. Stop blocked operation, publish blocker, continue authorised nonconflicting work.
