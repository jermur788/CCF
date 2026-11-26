# CCF Multi 2D — Phase 2B-2a Economy UI & `/multi` Routes — Task Checklist

- [x] **T-00** Verify worktree branch (`feat/multi-2d-phase2b2a-economy-ui-20261126-v1`), base commit (`6d72c30538b1fbe30d3cbcf083b5b368733662b8`), and clean status.
- [x] **T-01** Inspect approved Phase 2B-1 engine contracts, `/multi` HTTP layer, existing 14 ES modules, and existing Edge/smoke test invariants.
- [x] **T-02** Author four-file spec quartet under `specs/multi-2d-phase2b-economy-ui-v1/`.
- [x] **T-03** Implement lock-safe D2a session swap and server-authoritative `/multi` economy routes & DTO formatters in `PlayerApi.cs`, `MultiRoutes.cs`, `MultiApi.View.cs`, `MultiApi.Commands.cs`, `MultiApi.History.cs`, and `MultiApi.Saves.cs`.
- [x] **T-04** Implement Economy UI in `multi.html`, `multi.css`, and the existing 14 ES modules (zero new JS modules, zero JS economy math, single History drawer, contractor-only Fell, €2,500 floor highlight, upfront-cash block, Save v3 disclosure).
- [x] **T-05** Author new headless Edge economy suite (`tests/phase2b2a-economy-ui/run-economy-edge.sh` and `drive-economy.mjs`) covering 1920×1080 and 1280×720.
- [x] **T-06** Verify solution/UI builds (0 W / 0 E), Core (`291/291`), Save (`50/50`), `ui-smoke-multiarea.sh` (`363/0`), `ui-smoke.sh` (`250/4 {L01,L02,L06,J31}`), `run-edge-e2e.sh` (`63/63`), `run-step2-edge.sh` (`67/67`), new Economy Edge suite (both viewports), and `build/2d-gate.sh --headless-only` (exit 0).
- [x] **T-07** Author implementation handoff at `Docs/Handoffs/MultiRack-Simulation-20261116/phase2b-economy/PHASE2B2A-IMPLEMENTATION-HANDOFF-20261126.md`.
- [x] **T-08** Commit cleanly, re-run browser suites + canonical gate against the final commit, verify forbidden paths & tokens, and push feature branch to `origin` (zero merges, zero PRs).
