# RACK-V2-12 — Objective 1 end-to-end functional gate

Spec: `specs/scenario-one-objective-1-rack-planning-field-marking-v2/` (task RACK-V2-12, plan F-10, spec §30/§31/§36/§37, D-064 OPEN-2).

`RackV2Objective1EndToEnd.cs` is a disposable Tier-2 Unity gate that plays the whole Objective 1 flow in **one fresh game, one Editor
process**, from the real startup screen:

| Stage | What is driven | What is asserted |
|---|---|---|
| Start | `Start New Scenario` on the real startup screen; the premise pages | fresh Enlarged80 Rack Planning V2 world, 1344 stems, no racks/work; Objective 1 current; §30 step 1 / §31 copy (purpose, conditional 16–20 m, Plan Racks; no universal 18 m / five-rack claim) |
| Walk + map | native `P` (refused before confirm), native `W` held 1.5 s, native `M` | the player walks; the Stand Map opens; Plan Racks is a tool, not yet active |
| Plan / edit | `Plan Racks`, `Reset` (suggested layout), select + native `Delete`, refused four-rack confirm, `Undo`, one move, `Confirm Rack Plan` | five player racks, no work while drafting; four racks cannot be confirmed and nothing changes; the moved plan differs from the suggestion and stays valid; spacing shown; confirm creates one Pending rack-owned order per frozen tree, fells and marks nothing |
| Forest | native `M` closes the map | Tree Marks help introduces itself; Objective 1 asks for field marking; counter `0 / 5`; cue on the first unmarked rack |
| Approval before marking | native `Tab`, Work Plan | **D-064 / OPEN-2: field marking occurs before approval** — Approve Work Plan is unavailable, `ApproveWorkPlan()` is refused, nothing changes, and the Work Plan says why |
| Spray | native `P`, native left-click on each entrance | each rack resolves its own entrance; unrelated tree, later removal tree and a repeat spray are refused; counter 1..5 from the rack objects; orange paint on each entrance; marking approves nothing; at 5 / 5 the cue is gone, Objective 1 hands over to approval and `rack.layout` completes |
| Approve / execute | Work Plan `Approve Work Plan`, `Advance one year`, Annual Review | approval = exactly the frozen rack trees; `rack.approve`; one year; exactly those trees felled (stumps); cash change = quoted net; all rack work Completed and locked; counters unchanged; `rack.result` |
| Next objective | lesson 2 cue; native `C` in the teaching area | Objective 2 current; the teaching area is the central confirmed rack of the edited player plan and applies; Crop Tree marking works |
| Persistence | real `Save()`, `CanLoad()`, `Load()` | current schema, V2 validation, exact rack/order/lock signature, Objective 2 and 5 / 5 after load |

Captures at 1280×720 and 1920×1080 for twelve states (`c01`..`c12`). They are functional evidence; rendered-appearance acceptance
belongs to RACK-V2-13.

Fixtures (stated, not hidden): the player is placed beside each entrance tree for spraying (a short native walk is asserted
separately); the single map "move" edit is committed through the editor's own pointer-release path (`StandMapRackEditor.Apply`,
`Gesture.Move`) because the real-pointer drag is qualified by the RACK-V2-05/06 editing gate. Map buttons are invoked through their
own `Clickable`, as the guided/combined gates do. Every other key and click is a real OS event on the private display.

`CCF_O1E2E_MODE=diagnostic` measures the approval-before-marking case instead of asserting it (logged as `O1E2E_DEFECT`), restores
the in-memory state if approval went through, and continues, so one baseline run shows every defect. It ends with
`RACKV2_O1E2E_DIAGNOSTIC_DONE` and is never qualification evidence. The default `qualify` mode ends with `RACKV2_O1E2E_PASS`.

## Run

Run only in a clean registered Unity lane at the exact candidate under the machine-wide lock held for task `RACK-V2-12`. Inspect
`run_gate.py` first: it stages the listed sources into `Assets/ForestPrototype/`, removes them and their `.meta` afterwards, uses a
private Xephyr display and an isolated `XDG_CONFIG_HOME` (the player's `forest-save.json` is never touched), deletes the copied licence,
and reaps only its own Editor after that Editor's verdict and `Cleanup mono`.

```bash
cd /media/jer/ZX20/CCF-U1
python3 -I -B Tools/Verification/RackV2Objective1/run_gate.py --name RackV2Objective1EndToEnd --method RackV2Objective1EndToEnd.Begin \
  --files RackV2Objective1/RackV2Objective1EndToEnd.cs --native --normal-startup --timeout 1500 \
  --pass-token RACKV2_O1E2E_PASS --fail-token RACKV2_O1E2E_FAIL --out /media/jer/Files/CCF/Verification/<fresh-dir>
```

The same runner stages any other disposable gate (`--files` relative to the lane's `Tools/Verification`, `--ext-files` absolute),
so the existing combined, regression, field, runtime, save and protected-fingerprint harnesses run unchanged under this task's lock.
Restore the lane's known Editor side effects (`.vscode/settings.json`, Section Five materials) by exact path after each run.

This gate is new coverage. It replaces nothing and weakens nothing.
