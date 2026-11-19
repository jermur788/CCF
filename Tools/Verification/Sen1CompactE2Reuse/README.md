# Sen-1 #815 — Compact30 E2 reuse verification

Disposable, verification-only directory. It is **not** the reserved `Sen1Compact30Preflight` path and it writes no save.

## What is here

| File | Purpose |
|---|---|
| `Compact30RulesOfflineCheck.cs` | Plain .NET checks for the candidate roster/site fixtures and every site-validation positive/negative rule. No Unity types. |
| `run_offline.py` | Compiles `Compact30SiteRules.cs` + the offline check in a temporary project outside the repository and runs it. |
| `Sen1CompactE2ReuseVerification.cs` | Disposable Unity Editor gate: opens `ScenarioOneCompact30`, enters play, binds the runtime world, performs the teaching gates, four G1 designations, approval, annual resolution, CT-3 hold, covering review and T1. |

## Actual results in this source-only parent

**RUN — PASS**

```bash
python3 Tools/Verification/Sen1CompactE2Reuse/run_offline.py
# checks=36 failures=0
```

- Roster: 175 rows, unique ascending Sitka ids, exact #676 candidate SHA-256
  `dac06a3b7775f884fac9f8df6b95178cbaf9d912b44687082a161eaf0426cca1`.
- Four sites: correct 6×6 cells, path clearance non-negative and matching the recorded E1 value within 0.05 m, outside H1, legal at designation.
- Negative witnesses: wrong species/stock, site on the path, site in H1, stem intrusion, duplicate position, clearance drift, edited roster digest, duplicate roster id, short roster.

**RUN — PASS (not a Unity run)**

The complete runtime `Assets/**/*.cs` set (excluding `Editor/`) was compiled against the actual
Unity 6000.6.0f1 managed assemblies with 0 errors and 0 warnings. The disposable Editor harness was
also compiled against Unity 6000.6.0f1 + `UnityEditor.dll` with 0 errors. This proves API/type
compatibility, not a Unity import, scene bind, GUI run or gameplay result.

`Tools/Verification/CloudTypeCheck/run_typecheck.py --mode prod`: `TYPECHECK_CLEAN` (6 baseline
errors, 0 new).

**NOT RUN — Unity Editor/native gameplay**

No lawful machine-wide Unity lease route was available. The deployed `unity-lock` tool is absent from
the current Operations tree and the recorded lane configuration is stale; #815 forbids reopening that
investigation and forbids using a known unsafe lock. No Editor was started and no other owner's
process was touched.

## Mutation witness (proves the negative checks are live)

A scratch copy of `Compact30SiteRules.cs` had the path-clearance refusal disabled
(`if (false && clearance < 0f)`). The same 36-check run then failed exactly the intended witness:

```
FAIL site on the path refused
checks=36 failures=1
```

The unmutated source passes 36/36. This is a real negative-mutation demonstration for the site rules;
it does not cover runtime/scene behaviour.

## What the disposable Editor harness covers

It checks: Compact session/geometry/36 cells; 175 distinct roster trees and no implicit marks; no
rack tool/plan; old geometry sizes and admissions; no save controller; three real Crop Trees without
progress credit; arrival refused outside H1; assessment refused until opened; free exact placement
refused; four fixed Pending orders; a Pending cancellation and replacement; three-year-1 plantings
then a later-year fourth planting; approval and bulk-setter refusal; linked juveniles and visible
neutral representations; Y* = the later resolution year; non-covering review without credit; CT-3 on
two advance calls in the same frame; completed-site replacement refusal; covering review release;
no historical two-cycle completion; and a deliberate no-work year after T1.

It still does not cover a Failed-order replacement, old-world runtime suites, save/load, rendered
composition, performance or player feel.

## How to run the Unity gate later (disposable workflow)

1. Copy `Sen1CompactE2ReuseVerification.cs` into `Assets/ForestPrototype/`.
2. In the Editor, call `Sen1CompactE2ReuseVerification.Begin()` (or add a temporary menu item).
   The scene opens and play mode starts; the runner exits play mode and the Editor with code 0/1.
3. Collect `COMPACT30_E2_VERIFY_PASS checks=<n>` or `COMPACT30_E2_VERIFY_FAIL <reason>` from the log.
4. Remove the copied file and its generated `.meta` from `Assets/ForestPrototype/`. Confirm
   `git status` is clean apart from the intended source branch.

The harness writes no save and no scene. It does not exercise save/load (HELD), full Compact
completion (unsigned), player feel, rendered composition or performance.

## Copy/paste result block

```
SEN1_COMPACT30_E2_REUSE
offline_rules: PASS (36/36)
unity6000_assembly_compile: PASS (0 errors, 0 warnings)
editor_native_gate: NOT RUN (no lawful lease)
save_integration: HELD
compact_full_completion: NOT IMPLEMENTED (M-A/OD-16 unsigned)
```
