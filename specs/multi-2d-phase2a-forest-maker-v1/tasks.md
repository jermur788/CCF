# CCF Multi 2D — Phase 2A: Forest Maker — Tasks

- **Spec ID:** `SPEC-multi-2d-phase2a-forest-maker-V1`
- **Spec status:** `DRAFT` (no task may start until the owner approves the spec and a D-091 lane is admitted)
- **Created:** 2026-11-25

Rules: one single-writer lane per step. Commit after each task with a passing build. Never amend or rewrite history. Never tick `checklists/requirements.md` (that is the reviewer's job). Record evidence in the step handoff.

## Step 2A-1 — Core forest maker (lane 1)

- **T-2A1-00 Preflight:** confirm the base SHA, clean worktree, 9 anchors and `.p0ref` hash. Record the baseline Core/Save test counts and protected listeners.
- **T-2A1-01 Recipe:** add `src/Core/MultiArea/ForestMakerRecipe.cs` (record, enum, bounds constants, `RequireValid`, `TotalStems`, canonical bytes, hash, decode, presets). **Accept:** P2A-01.
- **T-2A1-02 Generator:** add `src/Core/MultiArea/ForestMakerGenerator.cs` implementing spec §5.2 rules 1–10 and §5.3. **Accept:** P2A-02.
- **T-2A1-03 Manifest note override:** an additive `GeneratorNoteOverride` in `MultiAreaExperiment.cs`. **Accept:** MA-1 / MA-1-MIXED manifest bytes and anchors 1–9 unchanged.
- **T-2A1-04 Presets:** assert both presets exactly. Capture their content hashes as anchors 10 and 11. **Accept:** P2A-03, P2A-04.
- **T-2A1-05 Species and cross-lane proofs:** constants-driven maturity/crown assertions, Hegyi cross-lane CI, per-lane RD, MA-B broadleaf seed input vs pure-Sitka control, 15-year run on both presets. **Accept:** P2A-05, P2A-06.
- **T-2A1-06 Regression and handoff:** full builds/tests, anchors, scope diff (zero `src/Ui/**`, `src/Core/Phase1/**`, biology). Write the 2A-1 implementation handoff (GitHub-tracked). **Accept:** P2A-REG (2A-1 part).

## Step 2A-2 — UI, Save Schema v3, speed (lane 2, after 2A-1 is accepted)

- **T-2A2-00 Preflight:** base = accepted 2A-1 HEAD (itself on the accepted Phase 1 Step 2 HEAD). Same baseline captures as T-2A1-00.
- **T-2A2-01 M0 measurement harness and table:** record the spec §8 measurements for both presets at Year 0 and Year 15 **before** changing budget code. **Accept:** M0 table in the handoff.
- **T-2A2-02 v3 codec:** `Phase1SaveFormat`, `Phase1SaveEnvelope`, `Phase1SaveCodec`, `Phase1SaveValidator` per plan §3.1. Add the v2 before/after SHA-256 regression and new v3 positive/negative cases. **Accept:** P2A-07 (codec part). All 50 existing Save tests still pass.
- **T-2A2-03 Core v3 capture and load:** `MultiAreaContinuation` writes v3 for recipe sessions. `BadgeOverride`. v3 self-contained genesis verification (spec §6.3). **Accept:** P2A-07.
- **T-2A2-04 Session host:** `src/Ui/MultiSessionHost.cs`, `PlayerApi` / `PlayerMultiApi` / `Program` wiring, `api/multi/woodland/{options,preview,new}`, v3 load replacement, unsaved-progress guard. **Accept:** P2A-08.
- **T-2A2-05 New Woodland modal:** header button, accessible dialog, Core preview, verbatim refusals, discard confirmation. **Accept:** P2A-09 (modal part).
- **T-2A2-06 Lane view:** active lane + neighbours (2 or 3), mini-map, 1-lane toggle, lane strip, ≥ 3 areas only. Two-area layout unchanged. **Accept:** P2A-09.
- **T-2A2-07 Payload fix and scaled budget:** typed `MultiViewPayloadBudgetExceeded` replaces the silent fallback, `MultiViewBudget.For`, forced-overflow test, frozen budget constant from M0. **Accept:** P2A-10.
- **T-2A2-08 Speed qualification:** re-run M0 after the changes, report against the Q5 targets. A static/dynamic split only if spec §8 conditions hold. **Accept:** P2A-10.
- **T-2A2-09 Regression and handoff:** full builds/tests, `ui-smoke-multiarea.sh` (ephemeral port), 1280×720 and 1920×1080 gates, anchors 1–11, scope diff, protected listeners. Write the 2A-2 implementation handoff (GitHub-tracked). **Accept:** P2A-REG.

## Dependencies

`Owner approval of this spec` → `Phase 1 Step 2 accepted` → `D-091 admission 2A-1` → T-2A1-* → independent review → owner acceptance → `D-091 admission 2A-2` → T-2A2-* → independent review → owner acceptance.
