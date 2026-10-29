# RACK-V2-11 Rack V2 persistence gate (disk save path)

Verification only. It changes no production save class, schema/version (v21), validation rule, migration
policy or Reference Future v1. Inspect `run_gate.py` and `RackV2PersistenceVerification.cs` before running.

Run only in a **registered** Unity lane (`/media/jer/ZX20/CCF-Ops/lanes.json`), at a clean committed
candidate, under a machine-wide Unity lock with owner `ClaudeA-RACK-V2-11`, task `RACK-V2-11`, naming that
exact HEAD and lane. Evidence goes under `/media/jer/Files/CCF/Verification/`, never ZX20.

```
E=/media/jer/Files/CCF/Verification/RackV2-11-<date>
python3 -B Tools/Verification/RackV2Persistence/run_gate.py --gate v2        --out $E/v2-01
python3 -B Tools/Verification/RackV2Persistence/run_gate.py --gate coldstart --out $E/coldstart-01 --seed-from $E/v2-01
python3 -B Tools/Verification/RackV2Persistence/run_gate.py --gate legacy40  --out $E/legacy40-01
python3 -B Tools/Verification/RackV2Persistence/run_gate.py --gate save03b   --out $E/save03b-01
```

| Gate | Pass marker | What it proves |
|---|---|---|
| `v2` | `RACKV2PERSIST_VERIFY_PASS mode=v2` | Through the real `Save()` / `CanLoad()` / `Load()` disk path: new game; draft before confirmation with non-contiguous stable ids (delete + re-add) and `nextRackId`; confirmation; partial marks (2/5, incl. the re-added id); all marks plus a second save cycle; 21 malformed V2 files refused by Continue and Load with no exception and no world change; v20 Enlarged80 save before planning loads model 0 and re-saves as explicit model 0; v20 and v21 model-0 saves with legacy `ConfirmRack` rack progress keep every order/mark as ordinary rackId-0 work and never enter V2; Reference Future v1 archive verified. Exports the partial-mark save + expectation for `coldstart`. |
| `coldstart` | `RACKV2PERSIST_VERIFY_PASS mode=coldstart` | A **new Editor process** continues the v2 run's partial-mark save: exact rack plan, ids, allocator, orders, marks, orange projection and world hash; Continue does not rewrite the file. |
| `legacy40` | `RACKV2PERSIST_VERIFY_PASS mode=legacy40` | A fresh Legacy40 game (`CCF_STAND_GEOMETRY=0`): v21 and v20 Legacy40 saves, with and without legacy rack progress, load unchanged (geometry, signature, world hash) and never enter V2; Legacy40 files claiming V2 are refused without a crash. |
| `save03b` | `RACKV2SAVE_VERIFY_PASS checks=35` | The unchanged RACK-V2-03B in-memory v21 schema/validation/hash gate. |

Save safety: each run uses a fresh `XDG_CONFIG_HOME`, so `forest-save.json` lives only in the run's evidence
directory; the harness additionally backs up and restores whatever is in that slot. The runner SHA-256s the
player's real `~/.config/unity3d/DefaultCompany/CCF/forest-save.json(.bak)` and the Reference Future v1
resource before and after and fails if any changed. The harness is staged into `Assets/ForestPrototype` and
removed with its `.meta` in the runner's `finally` path; only the known Editor rewrites (`.vscode/settings.json`
and five Section Five materials) are captured as a patch and restored from HEAD, and any other changed path
fails the run. No scene save, reference write, ecological year advance, Player or Windows build.
