# RACK-V2-11 — Unity persistence qualification result

**Task:** RACK-V2-11, claim #96, umbrella #91.
**Exact qualified candidate:** `a6ad231a13d87fe7b11d3bbef046b92fb9520e84`
(umbrella integration branch after RACK-V2-09 + S1-D T-06 + RACK-V2-11 tooling)
**Unity:** 6000.6.0f1; lane U1 `/media/jer/ZX20/CCF-U1`; machine-wide lock owner `ClaudeA-RACK-V2-11`, task `RACK-V2-11`; released normally.
**Executor:** DeepSeek #91 operator ran the already-authored Claude A harness/runner after provider limits.
**Exit:** `RACK V2 SAVE PASS`

## Gate results on exact `a6ad231`

| Gate | Result | Evidence |
|---|---|---|
| `v2` Enlarged80 new-game / confirm / partial marks / all marks / stable IDs / malformed V2 | **PASS** `RACKV2PERSIST_VERIFY_PASS mode=v2 checks=30`; 7 PASS lines, 21 malformed files refused by Continue and Load, no crash/world change; Reference Y100 `7AD177B3CC2F73C7` unchanged | `RackV2-11-20261029/v2-01/` |
| `coldstart` new Editor process continues a partial-mark save | **PASS** `RACKV2PERSIST_VERIFY_PASS mode=coldstart checks=2`; protected unchanged; clean after | `.../coldstart-01/` |
| `legacy40` old/legacy files load unchanged and never enter V2 | **PASS** `RACKV2PERSIST_VERIFY_PASS mode=legacy40 checks=7`; Legacy40 claiming model 1 or carrying V2 records refused | `.../legacy40-01/` |
| `save03b` unchanged RACK-V2-03B in-memory schema gate | **PASS** `RACKV2SAVE_VERIFY_PASS checks=35` | `.../save03b-01/`, `save03b-run.out` |

All runs show `protected-unchanged=True clean-after=True unexpected=[] leftover=[]`; the player save slot and Reference Future v1 archive were restored/unchanged. `.vscode/settings.json` and the five known Section Five materials were restored; two additional Berry materials observed after the first runs were captured in `editor-side-effects-before-save03b.patch` and restored from HEAD before the final save03b run. U1 ended clean at `a6ad231`.

## Representation / policy

No schema change: `ForestSaveData.CurrentVersion` remains 21; `rackPlanningModel`, `rackPlanConfirmed`, `nextRackId`, `racks[]` (`entranceTreeId` / `entranceMarked`) and `workOrders[].rackId` carry the state. Pre-v21 saves normalize to model 0; V2 activates only for Enlarged80 + model 1; Legacy40 never enters V2; malformed V2 is refused before mutation; Reference Future v1 is not reserialized.

## Residual

- No independent review of this verification-only task is claimed; the harness was authored by Claude A and run by the #91 operator.
- No Windows or Player build.
