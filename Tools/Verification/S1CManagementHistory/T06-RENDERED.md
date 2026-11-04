# S1-C T-06 guidance qualification

Run from the T-06 checkout with a clean detached candidate on U1 and a normally acquired machine-wide Unity lock owned by `Codex-S1C-T06`, task `S1-C-T06`. The runner validates the lock, lane, clean HEAD and absence of another Editor before staging. Unity version is 6000.6.0f1. No Windows build.

```bash
python3 -B Tools/Verification/S1CManagementHistory/run_t06_gate.py \
  --project /media/jer/ZX20/CCF-U1 --expected-head FULL_SHA --lock-id LOCK_ID \
  --out /media/jer/Files/CCF/Verification/RUN/t06-render \
  --t05-fixtures /media/jer/Files/CCF/Verification/S1C-T05-Unity-20261029/fixtures \
  --t05-prior-evidence /media/jer/Files/CCF/Verification/S1C-T05-Unity-20261029/02-objective/evidence
```

The default sizes are 1280x720 and 1920x1080. Repeat with `--gate t04`, `--gate t03` and `--gate t02` and distinct output folders to run the existing regression harnesses on the same candidate. Those harnesses retain the final T-04 production assertions. The T-06 fixture creates real work through production new-game/designation/approval/advance APIs; it blanks one historical planting event's cell and position **in memory** before recording its read-only comparison baseline. Live planted objects remain present, proving they do not fill historical location gaps. Failed/cancelled/no-work years cannot claim successful prior treatment.

Actions drive actual UI Toolkit callbacks and rendering. The look action must open the existing map at the event's recorded cell centre, and scripted player relocation verifies the actual HUD's arrival text there. This proves destination fidelity and runtime arrival handling, not OS-native walking/mouse input. World hash, scenario-save JSON, counts, cash, year and review acknowledgement must remain unchanged after browsing/expansion/navigation. No disk save is written. Existing map tutorial preferences are isolated by the runner's private config.

The runner copies the disposable selected harness into Assets/ForestPrototype, runs it, then removes it with its generated .meta. It uses isolated saves/config, removes its licence copy, and records/restores only the documented Editor settings/three-material import side effects; other drift fails the receipt. Never leave the harness in a playable build. Inspect every guidance and waypoint capture, both sizes, alongside process-result.json and evidence/t06-result.txt. Release the normal lock after Editor exit and clean lane inspection.

Offline commands (each accepts `--out` outside the repository):

```bash
python3 -B Tools/Verification/S1CManagementHistory/run_t06_offline_check.py --out /media/jer/Files/CCF/Verification/RUN/t06-offline
python3 -B Tools/Verification/S1CManagementHistory/run_t04_offline_check.py --out /media/jer/Files/CCF/Verification/RUN/t04-offline
python3 -B Tools/Verification/S1CManagementHistory/run_t03_offline_check.py --out /media/jer/Files/CCF/Verification/RUN/t03-offline
python3 -B Tools/Verification/S1CManagementHistory/run_offline_check.py --out /media/jer/Files/CCF/Verification/RUN/history-offline
```
