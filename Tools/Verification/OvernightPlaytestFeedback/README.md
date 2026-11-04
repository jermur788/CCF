# Overnight playtest feedback — source baseline (#260)

Written against published player-test source commit `72b4efbd1c653fa2231f632a79030a83ff76f833`, independently of fixes #255/#256. Python 3 standard library, Git and the installed .NET 8 SDK are required; no packages are added.

From this checkout's root:

```bash
python3 Tools/Verification/OvernightPlaytestFeedback/check_baseline.py
```

Every result is **SOURCE-STATIC**. The checker reads the current checkout, quotes exact C# literal fragments / expressions with file and line, and compares tracked production files against the recorded commit. It never changes source, existing tests, preferences, saves or Unity assets, and never launches Unity. Check 06b compiles/runs only `RackEditing.cs`, `RackGeometry.cs` and an embedded six-case driver with plain .NET, in an automatically removed OS temporary directory outside the repository. No Unity or Player build runs. The SOURCE-STATIC prefix describes the source/offline evidence tier; 06b executes isolated math rather than merely scanning text. It is independent of the working directory. Output has no timestamps or changing verification-branch HEAD, so identical source produces identical output.

Exit 1 means at least one feedback assertion is unmet, including the deliberately retained Objective 1 baseline failure; exit 0 means the selected source-policy and isolated-math assertions pass, not runtime qualification. NOT-TESTABLE observations never count as PASS. Exit 2 means missing source/anchor or another read/parse failure: inspect before interpreting any partial tally. An added/untracked production file is outside the tracked Git comparison; the C# inventory still reads it.

Checks cover the V2 Objective 1 concept's compact metre formatting, exact requested heading literal, source-material phrase, the Tree Marks legend's separate orange rack-removal and red Fell entries, Pro Silva mentions, rotation snap expressions, state-selected rack-edit instruction sentence, next-instruction path, primary CTA source and shared map/minimap source paths. The requested “duration” is actually rack spacing in metres. Both ASCII hyphen and the candidate's en dash are accepted; rejecting a space before m is a requested presentation format, not a functional defect, and is expected to flip with #255.

The phrase inventory scans ordinary/verbatim C# string tokens under `Assets/ForestPrototype/`, ignoring comments. It is deliberately conservative: a Pro Silva mention fails for human review rather than guessing whether it asserts a formal standard. It is not a full C# parser, evaluates no expressions/conditional compilation/interpolation, joins no dynamically composed text, and scans no serialized assets, localisation, external resources or binaries. The orange/Fell assertion is specifically the existing legend and separation sentence; it is not an exhaustive semantic language detector. Missing expected anchors fail with exit 2 rather than silently passing; checks 06a and 06c report a missing/unsupported policy expression or missing/changed/ambiguous instruction sentence as FAIL instead (exit 1, with a completed tally). Any later refactor needs deliberate checker review; do not relax it to hide a baseline failure.

**Nothing here proves visual layout, input behaviour or rendered correctness.** Exact heading presence alone cannot prove a heading is displayed. A shared map source cannot prove trail parity. Source `btn-primary` is not a measurement of user attention or keyboard focus. No forestry convention, Pro Silva standard, compile gate, simulation, persistence or platform qualification is established.

`baseline-output.txt` is the raw stdout of this bounded review-response run on the same pre-fix production baseline (exit 1): `PASS=6 FAIL=4 NOT-TESTABLE=2`, twelve tally rows (one new passing 06c; all previous results unchanged). The six 06b case diagnostics contribute one aggregate tally row. Two executions produced byte-identical output and no stderr. The original nine-row evidence is retained in Git at `62f1c1645b63f72125def83838e20847ece04e52`. Later production-state runs should be saved separately inside an owned evidence path. Manual Unity steps and the full limitations are in `Docs/Handoffs/Overnight-Continuation-20261104/BASELINE-VERIFICATION-HANDOFF.md`.

## Review-response checks

Check 06 retains its physical-input observation; its instruction evidence now comes from the state-aware literal inventory so changed copy cannot abort the run. New 06a reads the snap argument from the sole `Propose(px, ..., out RackPoint entrance...)` call inside the actual `OnPointerMove` method, with comments/strings excluded. It evaluates a restricted boolean grammar for Reshape/Create/Move with Shift up and down; unsupported expressions fail for deliberate review. This is not a general C# interpreter or data-flow proof. It assumes `ShiftHeld` supplies the Shift state and does not execute the event handler or prove downstream forwarding. Existing 06 quotes that forwarding as evidence.

The expected policy is explicit, never inferred from the observed expression:

- Default `--rotation-state pre-fix`: require `[True, False, True, False, True, False]` for Reshape/Create/Move, each with Shift up/down. PASS confirms the known pre-fix source structure, including snapping during rotation; it does not mean #256 is fixed.
- After #256, run `python3 Tools/Verification/OvernightPlaytestFeedback/check_baseline.py --rotation-state post-fix --instruction-state post-fix`: require `[False, False, True, False, True, False]`. Rotation must receive snap disabled regardless of Shift; create/move arguments must stay enabled unless Shift is held. A fixed expression fails pre-fix mode, and the old expression fails post-fix mode. Always-on, always-off, reversed Shift and rotation-only snapping also fail. Move currently ignores its snap argument and uses `MoveAlongHeadland`; no claim of move snapping is made.

Check 06b invokes the actual production `SnapFar`, not a Python port or a UnityEngine stub. Six cases cover a suggested angle within/outside the snap window, a boundary within/outside its window, and disabled snapping for both near cases. It asserts endpoints to 0.001 m, angle/boundary flags and zero parallel-rack ID, and prints returned endpoints and angles. Bounds have half-extent 40 m and entrance (0,0); these are isolated snapping inputs, not a valid headland-edit qualification. It does not cover nearby-rack tie-breaking, all threshold edges, validation, or gesture integration. Missing .NET or execution problems cause exit 2; no install/fallback silently converts this to PASS.

Check 08 requires exactly one button carrying `btn-primary` whose callback invokes `ui.ShowMap()`. Its label is evidence only: any wording can satisfy this navigation assertion. The current primary button invokes `ui.CloseAll()` and fails. This measures source class/action, not a contextual objective-state policy, focus, rendered prominence or successful live navigation. The prior review kept checks 02–07 and 09 byte-for-byte unchanged; this union-defect follow-up changes only 06's instruction evidence lookup and adds 06c; the original 06 observation remains baseline-specific when inspecting later sources.

## Combined-tree instruction-copy coupling

DPC's object-level union of #255/#256 (`b9f4d047`) exposed a real harness defect: 06 hard-coded `Hold Shift to turn snapping off.` and aborted on #256's rewritten sentence before reaching the later checks. The checker is now coupled to source in **two places: the rotation expression (06a) and the instruction sentence (06c)**. Anyone merging #255/#256 must pass the matching state flags for the tree being read. Both default to `pre-fix`; neither is inferred from source.

```bash
# Original production baseline:
python3 Tools/Verification/OvernightPlaytestFeedback/check_baseline.py --rotation-state pre-fix --instruction-state pre-fix
# Tree containing #256 (including the #255/#256 union):
python3 Tools/Verification/OvernightPlaytestFeedback/check_baseline.py --rotation-state post-fix --instruction-state post-fix
```

06c requires exactly one ordinary C# literal starting `Drag a rack to slide it along the headland.` in `StandMapView.cs`, equal to the selected full sentence:

- `pre-fix`: “Drag a rack to slide it along the headland. Drag its round handle to rotate it. Hold Shift to turn snapping off.”
- `post-fix`: “Drag a rack to slide it along the headland. Its round handle rotates freely. Hold Shift while drawing to turn snapping off.”

The wrong known state fails; unknown, missing or ambiguous copy reports observed state `neither` and FAIL, without throwing a missing-anchor ERROR. Comments are excluded. This is a literal-copy assertion, not proof of display or physical input. Rotation's existing truth-table check is unchanged. The single `SOURCE-STATIC selected-states: rotation=… instruction=…` line records the requested combination; 06a/06c and the final tally record whether it passed.

Own-branch pre/pre run: **6 PASS / 4 FAIL / 2 NOT-TESTABLE**, exit 1, production MATCH. Actual local union tree `b9f4d047`, post/post: **9 PASS / 1 FAIL / 2 NOT-TESTABLE**, exit 1; only the retained primary CTA check 08 fails. The union run read Git tree blobs through an in-memory `read` adapter (and compared that tree to BASE); its C# path inventory was confirmed identical. It used the checkout's byte-identical Unity-free RackEditing/RackGeometry files for the unchanged 06b offline driver. No production file, branch or checkout was changed. Raw union output: `/media/jer/Files/CCF/Verification/OvernightFeedbackUnion-b9f4d047-output.txt`. This remains source/offline evidence, not integrated runtime qualification.
