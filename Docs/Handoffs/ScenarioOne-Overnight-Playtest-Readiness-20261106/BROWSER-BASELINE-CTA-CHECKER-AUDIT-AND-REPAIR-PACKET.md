# Browser Manager audit — historical Objective 1 CTA checker needs a non-weakening repair

**Date:** 2026-11-06. **Status:** **BROWSER SOURCE-AUDIT COMPLETE; CHECKER REPAIR NOT IMPLEMENTED / NOT VERIFIED**. Manager browser-lane [#200](https://github.com/jermur788/CCF/issues/200) inspection only. New child execution claim [#299](https://github.com/jermur788/CCF/issues/299) **READY TO ASSIGN a separate verification source lane / NOT RUNNING**. This is independent of [#297 D-072 map source](https://github.com/jermur788/CCF/issues/297) and does **not** release map, diagram, Unity or integration work.

## Evidence inspected directly in GitHub

`Tools/Verification/OvernightPlaytestFeedback/check_baseline.py` on `main` tests 01..09 (including 06a/06b/06c) and its current test 08 around lines 249–254:
```python
buttons = evidence(VIEW, "buttons.Add(UiKit.Button(")
primary = [r for r in buttons if '"btn-primary"' in r[2]]
primary_map = len(primary) == 1 and bool(re.search(r"\(\)\s*=>\s*ui\.ShowMap\(\)", primary[0][2]))
```
`evidence` raises `ValueError` when exact source anchor absent. Outer handler exits **2**, so the historical checker does **not** reach a PASS/FAIL determination for test 08 on the accepted Objective cue candidate. This is a **source-shape incompatibility**, not proof of correct CTA behaviour.

The [accepted, source-reviewed Objective guidance implementation #295](https://github.com/jermur788/CCF/issues/295) on `feature/scenario-one-next-action-guidance-v1` now constructs:
```csharp
Button map = UiKit.Button("Stand map [M]", () => ui.ShowMap());
map.name = "objective-primary-cta";
buttons.Add(map);
```
The map button's **primary status is dynamic**, not a permanent `btn-primary` constructor argument. In `ScenarioOneUiRoot.RefreshNextActionGuidance`, `target = "objective-primary-cta"` only when Objective 1 conditions and the active Objectives view match. `ApplyNextActionGuidance` excludes Reference Preview, standalone menus, Help and the Help-close frame and checks visible/enabled scope before adding `btn-primary` and `next-action`. For missing/suspended/blocked states, there need not be any eligible primary, and safe modal choices may take precedence. The old test expectation of *exactly one unconditional primary ShowMap constructor* is **no longer the accepted design**.

**Evidence class:** GitHub source inspection only; no script execution, native Unity input, scene render, or PASS claim. The review findings on old checker's source are reproducible by reading the cited lines, but exact candidate test output must come from the actual verification worker.

## Required fix acceptance — preserve the original meaning, not a green tally

1. **Keep the checker a real guard.** Preserve all other baseline assertions 01–09, 06a, 06b, 06c and SnapFar's six true .NET calls. No deleting checks, returning PASS on a missing source anchor, downgrading FAIL to NOT-TESTABLE, opportunistic branch-baseline reset or changing historic user feedback to match implementation. State-specific expectations must be explicit.
2. **Repair test 08 with bounded structural checks:** Source-detect exactly one named Objective map button `objective-primary-cta`, correct `UiKit.Button` callback to `ui.ShowMap()`, that this same button is added to the actual Objectives button row, and that its literal caption stays stable (rather than being overwritten with a “Next” label). Inspect guarded conditional Objective 1 routing of the same named target in `RefreshNextActionGuidance` and the dynamic application/removal of `btn-primary` and `next-action`. Cross-check known safe/blocked cases from approved guidance spec; only claim what a bounded static scanner can demonstrate.
3. **Negative mutation tests:** Intentionally alter temporary **fixture copies** (not working production) of map target, click callback, guard, visibility check, dynamic class assignment and parent-row binding; the checker must FAIL each relevant mutation, not incorrectly PASS because an unrelated substring exists. Parser robustness must be sufficient for source's exact current syntax and reject ambiguous duplicates. Avoid unbounded brittle regex if it would be unreviewably unsafe; favour small tested lexical/token-aware bounded extraction.
4. **Do not overclaim dynamic behaviour:** Record static/structural result separately from stateful Unity Editor/native pointer/keyboard/Help timing and 720p/1080p rendered prominence, which remain **NOT RUN** until an exact integrated candidate. Additional deterministic offline harness state probes may strengthen the proof but must be tied to real production code or explicitly labelled simulation.
5. **Other tests stay intact:** 01 objective spacing copy, 02 heading, 03 source-material phrase, 04 Orange≠Red Fell, 05 Pro Silva copy, 06 input NOT TESTABLE, 06a rotation-snap policy, 06b six real SnapFar .NET cases, 06c instruction copy, 07 next instruction, 09 map/minimap **NOT STATICALLY TESTABLE**. If an unrelated source-anchor fails on current candidate, report as its own failure or separate scope request instead of masking with a false green.
6. **Differential/source evidence:** baseline `main` and accepted #295 feature comparison; exact start/end heads, changed verification-only paths, test command, stdout/stderr/return codes, a clean pass for *the intended new candidate* and expected previous state mismatch. The checker must not require merging the old diverged PR #264 wholesale or altering protected source.
7. **Independent review:** An independently selected capable reviewer inspects code diff, negative cases and preservation of prior assertions, records severity-coded findings in a GitHub Markdown report. Manager verdict on the checker **APPROVED / NEEDS CHANGES / REJECTED** is separate from accepting D-072 #297 or #296. Final exact integrated candidate reruns checker after the map/diagram integration and resolves any additional drift before claiming playtest readiness.

## Ownership and execution routing

- **Difficulty:** **Medium** — Python static checker and source-shape migration, .NET offline verification, negative test design; no Unity Editor required for source repair.
- **Best-suited model/provider:** selected by **DeepSeek orchestrator at actual dispatch**, based on available subscriptions/quotas and task-fit; code/test competent model then distinct independent reviewer. Model names are **not hardcoded**.
- **Exact child claim:** [#299](https://github.com/jermur788/CCF/issues/299).
- **Proposed isolated source branch:** `verify/scenario-one-feedback-cta-anchor-20261106` based on an *inspected accepted Objective guidance source*, not blindly on `main`.
- **Proposed separate worktree:** `/media/jer/Files/CCF/Operations/CCF-FeedbackCTARepair-20261106`. **Not created or certified free/clean.** The actual worker must refresh live coordination and obtain **Manager #200 explicit single-writer reservation/registration before editing**, and verify no existing directory/dirty files, exact HEAD, mount and storage. If branch/base selection would conflict with active D-072 #297, STOP for repin; never share its writable branch/worktree.
- **Strict allowed files:** `Tools/Verification/OvernightPlaytestFeedback/check_baseline.py`, new tightly bounded negative test fixtures/scripts in the **same verification directory** and exact result handoff, no `Assets/ForestPrototype` or any map/diagram/save file changes.
- **Handoff:** `Docs/Handoffs/ScenarioOne-Overnight-Playtest-Readiness-20261106/BASELINE-CTA-CHECKER-REPAIR-HANDOFF.md` on the distinct verification branch, exact code/test evidence and subsequent independent review/Manager disposition.
- **No local/Unity code performed in browser:** publishing this audit/packet is management work, **not** task #299 [RUNNING] or implementation. No Player/Windows build.

## Browser work available alongside #297

Overall Manager can independently (a) maintain this source diagnosis and packet, (b) review incoming #297 code/handoff/diff when published and issue real acceptance decision, (c) prepare exact integrated qualification and protected regression checklist without running Unity, and (d) audit stale D-071 Project/Drive context snapshot #293 and plan deterministic D-072 regeneration. **Do not** re-publish older mirrors as current or treat a free Unity lock as Stage C permission.

## Paste-ready future worker dispatch — only after Manager registers a separate verification lane

> **DeepSeek orchestrator — prepare separate checker repair task #299 (do not interfere with #297).** Read https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Overnight-Playtest-Readiness-20261106/BROWSER-BASELINE-CTA-CHECKER-AUDIT-AND-REPAIR-PACKET.md . Select a capable available Python/C# source-verification model based on actual tools/quotas; difficulty Medium. Ask Manager #200 to register the distinct branch `verify/scenario-one-feedback-cta-anchor-20261106` and isolated `/media/jer/Files/CCF/Operations/CCF-FeedbackCTARepair-20261106` after source-base/ownership preflight; do not start in #297 worktree. Fix stale test 08 while preserving all named tests and negative failure semantics; source-based PASS is not Unity runtime PASS. Run true .NET SnapFar and mutation controls; publish `BASELINE-CTA-CHECKER-REPAIR-HANDOFF.md` in its own branch, then separate independent reviewer/Manager approval. Do not touch production, Unity or Windows build.
