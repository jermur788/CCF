# S1-C T-03 rendered qualification

Use Unity 6000.6.0f1, a clean detached U1 candidate and the normal machine-wide
lock (`Sol-S1C-T03`, task `S1-C-T03`). The runner refuses a mismatched lock,
HEAD, dirty lane, occupied private display or another Editor. No Windows build.

```bash
python3 -B Tools/Verification/S1CManagementHistory/run_t03_gate.py \
  --project /media/jer/ZX20/CCF-U1 --expected-head FULL_SHA --lock-id LOCK_ID \
  --out /media/jer/Files/CCF/Verification/RUN/t03-candidate
```

The disposable fixture runs real new-game/work/advance/cancellation APIs. It
replaces two approved target IDs in memory to exercise the manager's actual
failure resolution, then blanks one resulting event reason to represent a
missing historical field. It never writes a disk save. Five completed years
cover 12 grouped fellings, owner/contractor planting, cancellation, two failures,
and no work. Assertions cover named controls, year/reopen presentation state,
recorded money/reasons, scroll reach and unchanged world/scenario-save records,
counts and acknowledgement. The newest acknowledgement is exercised separately.

The renderer captures both required resolutions, including the bottom of the
expanded large harvest. Inspect every capture for wrapping, clipping, hierarchy,
chips and settlement wording. Callback-driven UI checks do not establish
OS-native mouse targeting. This fixture does not qualify other S1-C tasks.

`--gate t02` stages and executes the **unchanged** protected T-02 harness. Run it
on both the pre-T-03 integration and candidate, under separate exact-HEAD locks;
use candidate mode on both so all 246 checks run. Do not reuse another task's
lock, change a locked checkout or rebaseline a protected expected value.

```bash
python3 -B Tools/Verification/S1CManagementHistory/compare_t03_reviews.py \
  --baseline /media/jer/Files/CCF/Verification/RUN/t02-control \
  --candidate /media/jer/Files/CCF/Verification/RUN/t02-candidate \
  --out /media/jer/Files/CCF/Verification/RUN/t02-comparison \
  --repo /path/to/qualification/worktree --base-rev FULL_CONTROL_SHA --head-rev FULL_CANDIDATE_SHA
```

Comparison retains raw evidence/diffs. A separate evidence copy removes only
the approved T-03 additions (detail toggle, grouped settlement and its note,
cancellation aggregate, grouped-money note), then calls the unchanged protected
`compare_reviews.py`. World hashes, gate states, footer predicate and all
remaining text must match. Unexpected movement fails. No source evidence or
protected baseline is rewritten.

The launcher removes its harness/meta and copied licence and restores only the
known Editor import side effects: shared `.vscode/settings.json` and three Forb
materials, with diffs retained. Other lane changes or any created disk saves
fail the process receipt. Inspect status and release the lock after all gates.
The first 2026-10-29 T-02 run preceded the automatic side-effect restoration;
its same restoration was performed explicitly and recorded in the handoff.
