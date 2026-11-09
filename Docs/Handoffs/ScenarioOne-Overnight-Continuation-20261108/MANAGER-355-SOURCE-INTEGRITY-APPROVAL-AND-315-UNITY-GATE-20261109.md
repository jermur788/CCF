# Manager #200 — #355 source/integrity verdict and single-candidate Unity admission

**Date:** 9 November 2026. **Formal G3/G4 verdict: APPROVED, SOURCE/INTEGRITY ONLY; NOT UNITY QUALIFIED OR USER-ACCEPTED.**   
**Author handoff:** [#355 report](https://github.com/jermur788/CCF/blob/182a1e5d3d4e65c87607ccfbed4d5036db3ef49d/Docs/Handoffs/ScenarioOne-Overnight-Continuation-20261108/SCENE-HANDLE-COMPATIBILITY-AND-INTEGRATED-PIN-RESULT-20261109.md).  
**Previous staging:** [#354](https://github.com/jermur788/CCF/issues/354) was a valid BLOCKED stop; its original compile obstacle is resolved at offline scope by #355.  
**Qualification claim to be repinned:** [#315](https://github.com/jermur788/CCF/issues/315); independent result reviewer remains [#316](https://github.com/jermur788/CCF/issues/316).

## Inspected scope and verdict

GitHub exact comparison from #354's report tip `b63309260ade1ec6f4062b932cc90e285d5656a5` to #355 tested source `7dc327903d62e3ab14a09527cae01ab48e1659a6` shows **one commit and exactly five files**: the Stand Map `StandMapSource.cs` production fix and four `Tools/Verification/StandMap/HeadlandOffline/` files. The following successor `182a1e5d3d4e65c87607ccfbed4d5036db3ef49d` changes exactly one Markdown report. Manager inspected the actual C# and verifier commit patches and report.

The production edit exactly matches the approved **nullable typed Unity `SceneHandle`** policy: import the real namespace, cache a `SceneHandle?` with `null` for uncached, reset to null, read the scene's native handle without implicit integer conversion. The existing manager/ecology/lost-component/scene compare and presentation/eligibility logic is otherwise unchanged. The extracted headland stand-in no longer permits implicit SceneHandle→int; the scene-change fixture tests different handles.

The original `check_stage_inventory()` assertions, historical pin constants and 20 historical mutations were **not rewritten**. New `--integrated` mode is expressly separate: it establishes the original #297 bytes at their original commits, reconstructs exactly four approved C# edits, independently pins the new SHA-256 and Git blob, requires exactly the previously classified **nine Workbench/C2 plus eleven D-072 production paths**, compares every nine C2 path and ten remaining D-072 source paths against approved ancestors, and rejects unauthorized paths. Eight targeted negative cases include an obsolete-int regression, a disabled scene comparison, unexpected tracked/untracked Assets, changed scene/packages and another D-072 file.

**Reported, not Manager-rerun:** the T3 worker used 320 SHA-pinned real Unity 6000.6.0f1 DLLs to compile all 189 non-Editor player-like sources under C#9 with zero errors; CTA actual candidate 10 PASS / 0 FAIL / 2 NOT TESTABLE, 92/92 extracted headland behavioural controls, 8/8 new identity negatives, and original fixture 39 source / 92 behavioural / 20 historical mutations. Local clean state, reference/archive, package, scene, source byte identity and no Unity/lease are worker-reported and subject to fresh local operator checks. Manager's GitHub source comparison establishes the five-file source scope, not execution of the local tests or Editor.

**Decision:** APPROVED G3 bounded pin integrity and G4 source admission **for exactly this candidate**, including the integrated-only stage evidence. This is NOT a claim that the original stage checker passes on the combined tree, and not an override of any failed production/runtime gate.

## Historical five-rack checker dependency: disposition

The original #308 five-rack verification contains byte pins for `run_headland_check.py` and `test_stage_inventory_guard.py`, plus ancestry-specific stage selection and historic handoff paths. By inspection these pins **cannot represent the combined branch's independent approved Workbench/C2 and #355 verification changes**. #354 measured a total of 14 failures on the previous combined source, with historic missing-handoff/ancestry/Workbench classes; #355 did not re-run the checker and correctly labels the additional two-script pin effect a **prediction, not a measured new result**.

Manager retains source-stage verified structural/painter/mutation evidence, original pinned Stage #303/#308 authority, and the new exact integrated identity witness. **Do not rewrite the original five-rack checker, import historic handoff docs merely to produce green, or weaken any old checks**. The integrated candidate still owes *real Unity* five-rack diagram, old-world guard, Full vs Mini and native input/visual evidence. Report historical checker stage mismatch as `NOT APPLICABLE TO INTEGRATED HISTORIC STAGE / NOT A PASS` with explicit links. No extra source-security campaign or repeated full five-rack run without a new observed defect.

## Next gate and narrow test economy

Previous #315 packet required separate #287 Workbench Tier-2 before source integration. The complete reviewed source is now assembled and offline-clean; **Manager supersedes that *order only*** for the focused internal playtest: do **the same mandatory Workbench C–G/save/reference/Legacy40 and UI native tests on the single exact combined candidate** in the **same eligible Unity session**. This does **not waive or mark #287 verified**; #287 remains NEEDS CHANGES/NOT UNITY QUALIFIED unless those actual Workbench obligations pass on the combined exact head and Manager subsequently rules. This is a test-session efficiency decision, not weakening of tests or automatic acceptance.

The one next execution is conditional local integrated qualification under [#315](https://github.com/jermur788/CCF/issues/315), with a new exact binding [single-candidate local Unity packet](https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Overnight-Continuation-20261108/MANAGER-355-TO-315-SINGLE-CANDIDATE-UNITY-QUALIFICATION-PACKET-20261109.md). Only after a fresh registered warm-lane ownership, immutable candidate, safe nonforced checkout, tracked-clean admission, real exclusive global Unity lease, licence and storage/monitor gates may any Editor test run. **The Manager has not inspected live local machine state or granted an already-held lease.** The original eight-dirty warm Workbench, registered CCF-U3, protected ignored licensed Innerverse assets, existing Libraries and frozen Reference Future v1/Legacy40 must be preserved.

#315 must stop on first blocker, report exact PASS/FAIL/NOT RUN across A–O, and avoid unnecessary separate cold import, repeated B3, old 884/112 adversarial suite, historic stage-source reruns or a Windows/Player build. If a conditional local lock, storage or dirty-lane admission fails, publish a BLOCKED receipt **without Unity**.

A subsequent genuinely independent [#316](https://github.com/jermur788/CCF/issues/316) review and Manager integration verdict are still mandatory **after** #315 actually returns. No player handback, main merge or Windows build is authorized here. The user must personally playtest an approved internal candidate and separately authorize Windows later.

**User action now:** paste the specific #315 T3 Code dispatch. No new product decision required.
