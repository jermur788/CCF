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
