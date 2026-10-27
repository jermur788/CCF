# RACK-V2-COMBINED-INTEGRATION gate

`RackV2CombinedVerification.cs` is the disposable Tier-2 gate for behaviour that neither Rack V2 lane could
test alone: the 02B-R fresh-world headland underneath the 03C authoritative rack runtime, a real cold-start
Continue from a persisted V2 save, and the Scenario One progression after the rack stage.

It has three modes, each run in its own Editor process, selected by `CCF_RV2C_MODE`:

| Mode | Process | What it proves | Pass token |
|---|---|---|---|
| `write` | fresh game, direct entry (`CCF_TEST_DIRECT_ENTRY=1`) | V2 world has the headland; five suggested-layout racks derive exactly the 02B-R qualified entrance trees; Confirm creates Pending rack work only; two entrances marked; a real disk save | `RACKV2COMBINED_WRITE_PASS` |
| `continue` | cold Editor on the **real startup screen** (no direct entry) | Continue against that file restores ids, geometry, frozen sets, marks, order ids/status and the world hash exactly; approve through the Work Plan UI; warn-on-edit; execute; locks; `rack.approve` / `rack.result`; Stage 2; a second disk round trip after execution | `RACKV2COMBINED_CONTINUE_PASS` |
| `probe` | fresh game, direct entry | **Informational.** V2 racks that are not on the authored layout lines, to measure what the legacy `RackLayoutTracker` does with them | `RACKV2COMBINED_PROBE_DONE` |

Environment: `CCF_RV2C_MODE`, `CCF_RV2C_STATE` (file shared between `write` and `continue`), `CCF_ACCEPTANCE_OUTPUT`
(capture folder). `continue` needs the `forest-save.json` that `write` produced copied into its own isolated
config folder. Run under the machine-wide Unity lock using the repo's copy -> run -> remove workflow.

Save safety: every mode refuses to run unless `Application.persistentDataPath` is inside `XDG_CONFIG_HOME`
(the runner's isolated config), so the player's own `forest-save.json` is never read or written.

Run it with `run_gate.py --private-display --keep-focus --env CCF_RV2C_MODE=write CCF_RV2C_STATE=...`
(`--normal-startup` for `continue`), 1280x720 and 1920x1080 captures are written as `c01..c07-*.png`.

This gate is new coverage. It replaces nothing and weakens nothing; the legacy `RackUxRack` gate is unchanged
(see the combined handoff for its retained / replaced / retired mapping).

## RACK-V2-COMBINED-REPAIR-01: model-0 / Legacy40 progression regression

`RackV2LegacyProgressionRegression.cs` is a second disposable gate, added with the lesson/progression repair. The repair made the
rack lesson predicates read the persisted V2 rack objects and the work they own; this gate shows model-0 worlds, which have no such
objects, still use the original authored-layout predicates. Select the mode with `CCF_RV2L_MODE`:

| Mode | World | What it proves | Pass token |
|---|---|---|---|
| `model0` | historical model-0 Enlarged80 (fresh game regenerated with the rack plan cleared) | five authored lines planned through the legacy `ConfirmRack`, approved in the real Work Plan, executed: `rack.create/layout/approve/result` follow the authored lines, orders carry no rack id, the next lesson opens, a disk round trip keeps model 0 and the lessons re-derive | `RV2L_MODEL0_PASS` |
| `legacy40` | fresh Legacy40 game (`CCF_STAND_GEOMETRY=0`) | no authored layout, so only `rack.create` records and the next lesson opens from the first completed felling | `RV2L_LEGACY40_PASS` |

Every step prints an `RV2L_TRACE` line with no timestamps or ids. The gate reads only members that exist on the pre-repair
production bytes, so running it on the pre-repair HEAD and on the repaired HEAD and diffing the `RV2L_TRACE` lines shows whether the
repair changed model-0 behaviour. Same copy -> run -> remove workflow, under the machine-wide Unity lock, with the isolated config guard.
