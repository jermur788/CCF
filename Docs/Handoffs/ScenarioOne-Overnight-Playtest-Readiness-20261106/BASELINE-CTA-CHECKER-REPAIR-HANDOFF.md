# Historical Objective CTA checker — source repair handoff

**Task:** [#299](https://github.com/jermur788/CCF/issues/299)  
**Date:** 2026-11-06  
**Status:** **SOURCE IMPLEMENTED / NOT YET QUALIFIED / NOT REVIEWED / NOT INTEGRATED**  
**Manager verdict:** **PENDING — do not label PASS/APPROVED**.

## Scope and ownership

- Starting accepted Objective-guidance candidate: `7f5278bfd42997da055da63423b6b4c30c7bb8f3` on `feature/scenario-one-next-action-guidance-v1`. It follows the accepted T2 candidate; only a later historic T3 handoff was present in between.
- Source-only branch: `verify/scenario-one-feedback-cta-anchor-20261106`; GitHub-native bounded changes by ChatGPT GPT-6, recorded by Overall Manager #200. **No local worker worktree was created/claimed; no Unity/Editor lease was taken.**
- All changed paths remain inside `Tools/Verification/OvernightPlaytestFeedback/`. **No** `Assets/ForestPrototype`, map/diagram, save, scene, build or user-checkout changes. No work on #297/#296's reserved feature branch.
- Difficulty **Medium**: source-shape migration needs conditional routing and false-green controls. The separate local verification owner should be capable of Python and .NET 8; DeepSeek can choose the actual provider/effort based on capacity, with an independent reviewer thereafter.
- Live task lock: `Docs/Project/coordination/active-tasks.md`; issue #299 is the execution claim. This handoff does not grant any local worktree or machine lease.

## Implemented delta

1. `check_baseline.py`: test 08 no longer assumes an unconditional `buttons.Add(UiKit.Button(... "btn-primary"...))`. It now calls a bounded structural verifier, reports **SOURCE-STATIC PASS/FAIL** for the real Objective CTA and continues on a failing structural assertion rather than aborting with a missing historical anchor. Other test numbers 01–09 and 06a/b/c are unmodified. In particular 06b still executes six real .NET `SnapFar` cases, and 06 and 09 remain NOT-TESTABLE at source level.
2. `objective_cta_source_check.py`: token-aware comment/literal handling, constructor and method extraction, unique named map CTA with `"Stand map [M]"`, `ui.ShowMap()` callback and actual Objectives header-row binding, Rack V2 and unconfirmed Objective 1 conditional target, allowlisted scope-only primary demotion/restoration, Help/preview/menu/close-frame suspension, visible/enabled candidate ancestry, and dynamic `btn-primary` and `next-action` assignment and removal. Raises on missing/ambiguous source shapes rather than passing from an unrelated substring.
3. `test_objective_cta_source_check.py`: baseline source validation plus **20 intentionally broken in-memory fixture variants** (identity, callback, caption, row, duplicate name, target, Objective state, Rack V2 gate, Help/Reference, enabled/visibility, scoped demotion, restoration, dynamic styling, and eligibility). Production files are not modified.

## Evidence actually obtained in this Manager session

- GitHub ancestry check: verification branch is an exact descendant of the accepted Objective-guidance feature candidate.
- GitHub compare: **three verification files only**, four source commits, no production changes.
- Direct GitHub inspection: all legacy non-CTA source-string anchors queried from the selected feature candidate were still present, including SnapFar source and rotation/copy state anchors.
- Source text inspection: seven critical CTA structural patterns matched exactly once in the actual accepted C# source; every exact replacement anchor for the 20 mutation fixtures was found uniquely in the source.
- Historical rotation/input source is `gesture != Gesture.Reshape && !ShiftHeld(e.modifiers)`; the currently displayed rack-edit sentence is the **post-fix** version. Use the explicit post-fix flags below.

**NOT RUN:** Python module compilation, the full numbered checker, the actual 20 in-memory Python mutation executions, the six .NET SnapFar calls, Unity Editor/native input, Help-close frame runtime state, or 720p/1080p rendered CTA. The current Manager tool environment cannot clone the full repository or execute .NET; source inspection above is **not** a substitute for those tests. There is no fabricated PASS tally.

## Mandatory local qualification — exact commands

In a **separate, clean, Manager-registered verification worktree** checked out at this verification branch (not the #297 feature worktree):

```bash
git status --short --branch
python3 -m py_compile Tools/Verification/OvernightPlaytestFeedback/check_baseline.py Tools/Verification/OvernightPlaytestFeedback/objective_cta_source_check.py Tools/Verification/OvernightPlaytestFeedback/test_objective_cta_source_check.py
python3 Tools/Verification/OvernightPlaytestFeedback/test_objective_cta_source_check.py
python3 Tools/Verification/OvernightPlaytestFeedback/check_baseline.py --rotation-state post-fix --instruction-state post-fix
```

Requires Python 3, Git and .NET 8 SDK. Save **full raw stdout/stderr/exit codes** and the starting/end HEAD and clean state. Expect all 20 negative controls to reject, and a **SOURCE-STATIC** result for the full candidate; if any source anchor drifts or outputs an error, diagnose it rather than changing tests to green. Run the historical baseline comparison with explicit expected source-state mismatch as appropriate; do not weaken original tests, change `BASE`, or rewrite `baseline-output.txt` to disguise history.

If checks fail, repair **only the allowed verification paths** and hand off a new exact candidate; do not mutate production C# to satisfy an old source checker. Preserve all raw failure evidence and ask Manager for new scope if production drift is real.

## Independent source review and Manager gate

Assign a separate reviewer after actual Python/.NET evidence is published. Review the exact verification diff, mutation coverage and any false-green hazards; confirm that original 01–09, 06a/b/c, SnapFar, rotation/help/map-minimap NOT-TESTABLE semantics and historical evidence were not weakened. Publish a severity-coded GitHub Markdown reviewer handoff and request Overall Manager verdict **APPROVED / NEEDS CHANGES / REJECTED**.

**Do not merge yet.** After #297 map and #296 diagram have their own approvals, the integrator must carry only the reviewed verification delta onto the exact new candidate and rerun the checker there. Source PASS must not be reported as Unity runtime PASS. No Windows/Player build is authorised by this task.

## Next task and user action

Next: **local Python + .NET verification, then independent reviewer**, keeping ownership isolated from #297. User action required only to dispatch this bounded local verification task to a capable Code/T3 runner; no product decision is needed.
