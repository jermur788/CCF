# Independent source review — #299 Objective 1 CTA checker repair (#302)

**Date:** 2026-11-06
**Claim:** [#302](https://github.com/jermur788/CCF/issues/302), reviewing the [#299](https://github.com/jermur788/CCF/issues/299) repair as qualified by [#301](https://github.com/jermur788/CCF/issues/301) (draft [PR #300](https://github.com/jermur788/CCF/pull/300)).
**Status:** **SOURCE-STATIC REVIEW COMPLETE. Not Manager approval, not an integration, not Unity proof.**

## Verdict

**NEEDS CHANGES. One blocking Major finding (F1).**

There are **no blocker-class findings** of the kind the dispatch names. No numbered test was removed, waived or relabelled. `BASE` and `baseline-output.txt` are unchanged. 06b still compiles and calls the real production `SnapFar` on .NET. The old baseline still fails.

Every result the #301 worker reported reproduced byte-for-byte when I ran it myself:

- 20/20 seeded mutations rejected.
- `PASS=10 FAIL=0 NOT-TESTABLE=2`, exit 0.
- The old baseline gives `PASS=6 FAIL=4 NOT-TESTABLE=2`, exit 1.
- The six real SnapFar cases pass.

The blocking problem is in what the new test 08 checks. It proves the Stand map button is made primary at the right time, but it does **not** prove that a competing "primary" button is turned off. The checker still passes when the static `btn-primary` **Back to forest [O / Esc]** button (which calls `ui.CloseAll()`) is no longer demoted, or when another static primary button is added (findings R03, R04 and R05). In those cases the Objective 1 header shows Back to forest as a second primary button, next to the map button. That is exactly the defect the original test 08 caught ("Baseline primary invokes ui.CloseAll()"). On this point the new test is more lenient than the old one. The fix is small and stays inside the verification files. I prototyped it: it still passes the live source and rejects all four of those mutations.

| ID | Severity | Blocking | Summary |
|---|---|---|---|
| F1 | **Major** | **Yes** | Test 08 does not list competing or unconditional `btn-primary` sources in the Objectives header. False greens: R01, R03, R04, R05. |
| F2 | Major (dispatch floor) | No | Matching only checks that anchors exist and are in order, so an added statement can override or cancel the checked path. False greens: R08, R09, R11–R15, R20, R21. |
| F3 | Minor | No | The CTA-name uniqueness check only recognises one way of writing the string. A verbatim-string duplicate passes (R07). |
| F4 | Minor (evidence) | No | #301's "SDK 8.0.133 pinned" run did not actually pin the SDK; it used 10.0.114. The conclusion is still true; my own correctly pinned run is byte-identical. |
| F5 | Minor (docs) | No | `README.md:34` (and the exit-2 note at :17) still describe the old test-08 contract. |
| F6 | Minor | No | When test 08 fails it prints no evidence and stops at the first unmet check. The dead check at `objective_cta_source_check.py:101-103` (see F1) suggests coverage that isn't there. |
| F7 | Nit | No | `line_at` relies on exact spacing. A lone `check_baseline.py` transfer would crash on import. |

## Reviewer and independence

| Field | Value |
|---|---|
| Reviewer | **Claude Opus 5.5** (`claude-opus-5-5`), provider **Claude A** (`claudeAgent`), configured effort **xhigh** |
| Dispatch | DeepSeek V4.1 Flash orchestrator (dispatch only, not the reviewer) |
| Independence | Different provider and model from the ChatGPT GPT-6 source author (#299) and the DeepSeek V4.1 Flash qualifier (#301). I wrote no #299/#301 code, did not edit PR #300 or the verify branch, and ran every command below myself. |
| Role | Independent source reviewer. Writes only this report. Has no approval, merge or integration authority. |

## Lane, starting HEAD and target

| Item | Value |
|---|---|
| Review worktree / branch | `/media/jer/Files/CCF/Operations/CCF-FeedbackCTAIndependentReview-20261106`, `docs/feedback-cta-checker-independent-review-20261106-v1` |
| Review starting HEAD | `4780ae00409489b79e0ca50188c3da96f4436fdb` (= `origin/main`; 2 docs-only commits ahead of remote docs branch `eb93a9ba`) |
| Read-only target | `/media/jer/Files/CCF/Operations/CCF-FeedbackCTARepair-20261106`, `verify/scenario-one-feedback-cta-anchor-20261106` at **`f294fe0769bdb910eb58576dc53751f669982528`** (= remote; clean before and after, 0 tracked or untracked changes) |
| Review delta | **`7f5278bfd42997da055da63423b6b4c30c7bb8f3..f294fe07`** (accepted Objective-guidance candidate → repair head). `72b4efbd1c653fa2231f632a79030a83ff76f833` is only the historical `BASE`. |
| File written | Only this report |

## Environment and toolchain (observed)

- Linux 7.0.0-42-generic x86_64. Python 3.12.3. Git 2.43.0.
- .NET SDKs 8.0.133 and 10.0.114; the default is 10.0.114. Runtimes Microsoft.NETCore.App 8.0.33 and 10.0.14.
- No Unity process before or during the review. No Unity, Editor, Player, save, scene or Windows build was involved.
- All Python ran with `PYTHONDONTWRITEBYTECODE=1 PYTHONPYCACHEPREFIX=<scratch>`. No `__pycache__` was left in the target; I checked with `find` afterwards.
- Scratch outputs went under `/tmp/claude-1000/.../scratchpad/out/`. These are ephemeral. The evidence that matters is quoted or hashed in this report.

## Commands actually run by this reviewer (exit codes observed)

`$V` = target worktree, `$D` = `Tools/Verification/OvernightPlaytestFeedback`, `$S` = review scratch dir under `/tmp`.

| # | Command (abridged; cwd = `$V` unless stated) | Exit | Observed |
|---|---|---|---|
| 1 | `git -C $V diff --stat 7f5278b..f294fe07`; `--name-only`; `git diff 7f5278b..f294fe07 -- Assets \| wc -l` | 0 | 5 files: 3 verification + 2 handoffs. **0 lines under `Assets/`.** |
| 2 | `python3 -m py_compile $D/check_baseline.py $D/objective_cta_source_check.py $D/test_objective_cta_source_check.py` | **0** | Empty stdout and stderr |
| 3 | `python3 $D/test_objective_cta_source_check.py` | **0** | `rejected=20/20 false_greens=0`. stdout sha256 `77dfd933…c83304` (= raw `03`) |
| 4 | `python3 $D/check_baseline.py --rotation-state post-fix --instruction-state post-fix` (default SDK) | **0** | `PASS=10 FAIL=0 NOT-TESTABLE=2`, stderr empty. stdout sha256 `3128d28d…2af53a` (= raw `04`) |
| 5 | Same as 4 with `TMPDIR=$S/sdk8` (`global.json` pins `8.0.133`, `rollForward: disable`) — the #301 method | **0** | Byte-identical to 4. **The shim (7) shows the SDK used was 10.0.114**, not 8. |
| 6 | Same as 4, run **from cwd `$S/sdk8/cwd`** (below that `global.json`), with `TMPDIR=$S/sdk8` | **0** | Byte-identical to 4 (`3128d28d…`), and to raw `05` |
| 7 | Runs 5 and 6 repeated with a logging `dotnet` shim on `PATH`. It logs cwd and SDK, captures the generated driver project, then `exec`s the real `dotnet` with unchanged args. | 0 / 0 | Run 5: muxer `10.0.114`, `NETCoreSdkVersion 10.0.114`, MSBuild 18.0.36. Run 6: muxer **`8.0.133`**, `NETCoreSdkVersion 8.0.133`, MSBuild **17.8.60**. `TargetFramework net8.0` both times. stdout unchanged. |
| 8 | `git -C $V worktree add --detach $S/old-72b4efbd 72b4efbd…`; copy the three f294fe07 checker files in (sha256 matched); `python3 $D/check_baseline.py --rotation-state pre-fix --instruction-state pre-fix` (cwd = old worktree) | 0 / **1** | `tracked-production-vs-baseline: MATCH`; `PASS=6 FAIL=4 NOT-TESTABLE=2`. stdout sha256 `3e826a59…f42f1` (= raw `07`) |
| 9 | `diff baseline-output.txt <run 8 stdout>` | 1 | Only the test-08 block differs (quoted below) |
| 10 | `git -C $V worktree remove --force $S/old-72b4efbd && git -C $V worktree prune` | 0 | Removed. Its only content was the untracked copied checker folder. |
| 11 | `cd …/ScenarioOneFeedbackCtaQualification-20261106/raw && sha256sum -c ../SHA256SUMS.txt` | **0** | 19/19 OK |
| 12 | `python3 $S/independent_mutations.py $V` (my in-memory mutations, R01–R21 and B01–B02) | 0 | Rejected 7, **false-green 14**, benign accepted 2, unexpected reject 0. stdout sha256 `53636af0…f81f237` |
| 13 | `python3 $S/prototype_f1_guard.py $V` (proposed F1 guard, scratch only) | 0 | Live source passes. R01, R03, R04, R05 rejected. |
| 14 | `git -C $V fetch -q origin main`, then ancestry and diff queries for PR #300 | 0 | `origin/main` was already `4780ae00`, so the remote-tracking ref did not change. No working tree changed. |
| 15 | `git -C $V status --short --branch --untracked-files=all` before and after | 0 | Clean both times. HEAD = `f294fe07`. |

## Branch-diff scope and PR #300 warning

**Executed:**

- `7f5278b` is an ancestor of `f294fe07`.
- `git diff --name-only 7f5278b..f294fe07` lists exactly these files:
  - `Tools/Verification/OvernightPlaytestFeedback/check_baseline.py` (+13/−6: one import line and the test-08 block)
  - `Tools/Verification/OvernightPlaytestFeedback/objective_cta_source_check.py` (new, 195 lines)
  - `Tools/Verification/OvernightPlaytestFeedback/test_objective_cta_source_check.py` (new, 105 lines)
  - `Docs/Handoffs/…/BASELINE-CTA-CHECKER-REPAIR-HANDOFF.md` and `…QUALIFICATION-HANDOFF.md` (docs)
- **No production C#, USS, scene, prefab, ProjectSettings or package change.** The `Assets/` diff is 0 lines.

**PR #300 ancestry (executed):**

- PR #300 is draft and open. Head: `verify/…` at `f294fe07`. Base: `feature/scenario-one-next-action-guidance-v1`, currently `1b68ddcd`, which is 2 commits ahead of the verify branch.
- Against `origin/main` (`4780ae00`), the verify branch's merge base is `e1be15c1`. The branch carries **12 commits not on main**. Two of them are historical Objective UI production commits: `ffe7c21c feat(ui): derive safe Objective One next-action cues` and `065feb12 Fix T2 cue labels, refresh cost and primary styling scope`.
- Those two commits change `LearningObjectivesView.cs`, `ScenarioOneUiRoot.cs`, `StandMapView.cs`, `WorkPlanView.cs` and `ScenarioOneUi.uss`. `7f5278b` is not on main.

> **PR #300 MUST NOT be merged wholesale, and must not be retargeted to main or merged as a branch.** Only the reviewed three-file verification delta (amended per F1) may later be carried onto the exact integrated candidate and **rerun there**: py_compile, the mutations, the full checker and the six .NET SnapFar cases. The two handoff documents are history, not code.

## Original numbered tests 01–09 and 06a/b/c

**Code comparison (executed):**

- `check_baseline.py` has blob `2dcacb2a` at both `a61e643b` (where the checker was staged) and `7f5278b`, and blob `9f752dc1` at `f294fe07`.
- The diff touches only line 13 (the import) and the test-08 block (lines 250-261).
- `BASE = "72b4efbd…"` is unchanged at line 16.
- `baseline-output.txt` has blob `db6a8b85` at both `7f5278b` and `f294fe07`. The only commit that ever touched it is `a61e643b`.
- `README.md` is unchanged (blob `ef4b07ca`); see F5.

"Old src" below means the run on 72b4efbd with the repaired checker and pre-fix flags.

| Test | Code change 7f5278b→f294fe07 | Candidate post-fix (I ran it) | Old src (I ran it) | Stored `baseline-output.txt` | Note |
|---|---|---|---|---|---|
| 01 requested-spacing | none | PASS | FAIL | FAIL | |
| 02 tree-marking-heading | none | PASS | FAIL | FAIL | |
| 03 source-material-copy | none | PASS | FAIL | FAIL | |
| 04 orange-entry-is-not-fell | none | PASS | PASS | PASS | |
| 05 no-formal-pro-silva | none | PASS | PASS | PASS | |
| 06 round-handle-input | none | **NOT-TESTABLE** | NOT-TESTABLE | NOT-TESTABLE | Observation only |
| 06a rotation-snap-policy | none | PASS `[F,F,T,F,T,F]` | PASS `[T,F,T,F,T,F]` | PASS | State selected by flag |
| 06b snapfar-offline-cases | none | PASS, 6 real cases | PASS, 6 real cases | PASS | Real `RackEditing.cs`/`RackGeometry.cs` compiled (captured csproj below). No mock. |
| 06c rack-edit-instruction | none | PASS (post-fix sentence) | PASS (pre-fix) | PASS | |
| 07 objective-next-instruction | none | PASS | PASS | PASS | |
| 08 objective-primary-cta | **replaced** | PASS (bounded structural) | **FAIL** (completed row, exit 1) | FAIL | Old baseline still fails; see F1/F6 |
| 09 map-minimap-trail | none | **NOT-TESTABLE** | NOT-TESTABLE | NOT-TESTABLE | Observation only |
| **Tally / exit** | | **10/0/2, exit 0** | **6/4/2, exit 1** | 6/4/2 | |

On whether any test was weakened:

- Tests 01–07 and 09 are byte-unchanged in code.
- Test 08 replaced a static contract that the accepted design deliberately invalidated: Back to forest is still created with `btn-primary`, and the map is made primary only conditionally, at runtime. That replacement is legitimate in principle. F1 records the one property it dropped.

## Test 08 and scanner audit (from reading the source; behaviour confirmed by the mutations)

What the scanner does verify on the live source (`LearningObjectivesView.cs:62-90`, `ScenarioOneUiRoot.cs:61-67, 335-500`):

| Contract element | Scanner location | Assessment |
|---|---|---|
| Named map button, exact caption, `() => ui.ShowMap()` click | `:81-86` | Correct. Requires the exact one-argument form, so any extra arguments are rejected (R02). |
| Identity `"objective-primary-cta"` on the map variable only | `:87-91` | Correct for plain string literals. Verbatim-string duplicates and later overwrites get through (F3, F2/R08). |
| Header-row binding: `Root.Add(modal)`, header, `UiKit.Row(header)`, button, name, `buttons.Add(map)`, `modal.Add(header)` | `:92-100` | Order-only (F2), but it does bind the map to the header row. |
| "Not unconditionally primary" | `:101-103` | **Effectively dead**: the regex at `:81-85` already forbids extra arguments. A separate `map.AddToClassList("btn-primary")` is not seen (F1/R01). |
| Allowlist contains the CTA exactly once | `:105-107` | Correct, but does not check that the **competing** `objective-back-to-forest` is listed (F1/R03). |
| Refresh: clear, restore originals, clear tracking, screen scope, no-scope return, consent precedence, Rack V2 gate, objectives flag, `!RackPlanConfirmed` → CTA target, `else return`, scoped demotion loop, scope-bound candidate, apply | `:109-136` | Every anchor must appear exactly once, in order. Negating or widening a condition is rejected (R18, R19 and the seeded cases). The target is selected exactly once (`:137-138`). There is exactly one `owned.RemoveFromClassList` (`:139-140`). |
| ClearNextAction removes both classes and the reference | `:142-148` | Correct |
| Apply: preview/menu/Help/help-close suspension, enabled + visible + in-scope eligibility, clear, `if (!eligible) return`, assign, add `btn-primary` and `next-action` | `:150-164` | Exact expression match, so dropping any guard term is rejected. Order is enforced (R17). The **setter** of `helpClosedFrame` (`CloseHelp`, `:614`) and the `LateUpdate` re-apply call (`:327`) are not checked (F2/R20, R21). |
| GuidanceVisible: ancestry walk, display/visibility, `current == scope`, final `return false` anchored with `\s*$` | `:165-176` | Correct. The final-return anchor works (R16 rejected). |
| Lexer `_LEX` | `:14-17` | Handles `//`, `/* */`, verbatim `@"…"` with `""`, regular strings with escapes, and char literals. Leftmost matching correctly protects `//` inside strings. Comments are blanked in extracted bodies. **Literals are kept in extracted bodies**, so code-shaped text inside a string can match a literal-free anchor (R09). No awareness of `#if` (R14). C# 11 raw strings are not handled; Unity 6000.6's C# level makes that moot in practice (inferred). |
| `_method` brace extraction | `:35-53` | Matches the signature exactly once on the literal-masked text. Braces are counted on the masked copy, and the slice is taken from the original (same length). Sound. |
| `_one` | `:56-60` | Exactly-one, with `re.S`. Duplicates fail safe, e.g. a literal duplicating an anchor raises "found 2". |
| `_ordered` | `:63-69` | **Order and existence only.** Does not reject added statements that contradict or cancel the anchored path (F2). |

## Mutation results

### 20 seeded controls (`test_objective_cta_source_check.py`) — ran myself, exit 0

All 20 were rejected with 0 false greens. My stdout is byte-identical to the worker's raw `03-cta-mutations.stdout.txt` (sha256 `77dfd933…c83304`). The cases: map-button-name, map-click-destination, map-caption, map-not-added-to-row, duplicate-map-identity, target-renamed, objective-condition-removed, rack-v2-gate-removed, help-guard-removed, reference-preview-guard-removed, candidate-enabled-check-removed, scope-visibility-check-removed, hidden-visibility-check-removed, allowlist-primary-dedup-removed, unrelated-global-demotion, primary-restore-removed, primary-class-not-added, next-action-class-not-added, eligibility-return-removed, target-not-bound-to-scoped-button. `replace_one` asserts each anchor occurs exactly once, so a drifted anchor makes the control raise rather than pass.

### Independent reviewer mutations — ran myself (`$S/independent_mutations.py`, scratch, not committed)

All mutations are in-memory text edits to the real production C#, passed to the **unchanged** `verify_objective_cta`. None of the mutated C# was compiled. "Would compile" is inferred from C# rules: R13 produces unreachable-code warning CS0162, not an error.

| ID | Cat. | Mutation (old → new) | Outcome | Functional impact if it shipped (inferred from source) |
|---|---|---|---|---|
| R01 | a | add `map.AddToClassList("btn-primary");` after `map.name = …` | **FALSE-GREEN** | Map is primary outside the T2 slice (later objectives and Legacy40, where `RefreshNextActionGuidance` returns before demotion), next to Back to forest. Within the slice, the allowlist still demotes it. The conditional CTA becomes unconditional. |
| R02 | a | `UiKit.Button("Stand map [M]", () => ui.ShowMap(), true, "btn-primary")` | REJECTED | — (caught by the constructor regex, not by `:101-103`) |
| R03 | allowlist | drop `"objective-back-to-forest"` from `GuidanceButtonNames` | **FALSE-GREEN** | During unconfirmed Objective 1, the static `btn-primary` Back to forest (`CloseAll`) is never demoted. **Two primaries; the original #260 test-08 defect returns.** |
| R04 | allowlist | `forest.name = "objective-forest"` | **FALSE-GREEN** | Same as R03 |
| R05 | allowlist | add `Button close = UiKit.Button("Close", () => ui.CloseAll(), true, "btn-primary"); buttons.Add(close);` | **FALSE-GREEN** | A new competing CloseAll primary in the header. The old test 08 would have failed this shape. |
| R06 | b | `work.name="objective-primary-cta";` (no spaces) | REJECTED | — |
| R07 | b | `work.name = @"objective-primary-cta";` | **FALSE-GREEN** | Duplicate CTA id. The map is still found first (UQuery depth-first order), but `objective-work-plan` routing (annual review and approval states) is lost. |
| R08 | b | add `map.name = "objective-map";` after `buttons.Add(map);` | **FALSE-GREEN** | `Q<Button>("objective-primary-cta")` returns null, so the **Objective 1 CTA is never shown.** |
| R09 | c | `if (!eligible) return;` → `UnityEngine.Debug.Log("if (!eligible) return;");` | **FALSE-GREEN** | Ineligible or null candidates get styled; a null candidate throws. |
| R10 | c | `// if (!eligible) return;` | REJECTED | — (comments masked) |
| R11 | d | insert `if (scope == learning.Root) target = "objective-back-to-forest";` before the candidate binding | **FALSE-GREEN** | **Objective 1 CTA becomes Back to forest / `CloseAll`, i.e. the original defect.** |
| R12 | d | `candidate = null;` after `Button candidate = guidanceCandidate;` | **FALSE-GREEN** | CTA never styled |
| R13 | d | `return;` after `guidancePrimaries.Clear();` | **FALSE-GREEN** | Guidance never derived |
| R14 | d | wrap `candidate.AddToClassList("btn-primary");` in `#if UNITY_EDITOR … #endif` | **FALSE-GREEN** | Player builds lack the primary class |
| R15 | d | `map.SetEnabled(false);` after row binding | **FALSE-GREEN** | Map button is disabled and the CTA is ineligible |
| R16 | anchor | final `return false;` → `return true;` in `GuidanceVisible` | REJECTED | — |
| R17 | order | swap `if (!eligible) return;` and `nextAction = candidate;` | REJECTED (order) | — |
| R18 | cond | `!manager.RackPlanningV2Active` | REJECTED | — |
| R19 | cond | `bool objectives = scope == learning.Root \|\| true;` | REJECTED | — |
| R20 | guard | remove `helpClosedFrame = Time.frameCount;` from `CloseHelp` | **FALSE-GREEN** | The help-close-frame suspension can never fire |
| R21 | guard | remove the `ApplyNextActionGuidance();` call from `LateUpdate` | **FALSE-GREEN** | Cached guidance is no longer re-applied after layout or Help |
| B01 | benign | trailing comment after `if (!eligible) return;` | ACCEPTED-OK | — |
| B02 | benign | whitespace-only map-constructor reformat | ACCEPTED-OK | — |

**Tally:** 7 rejected, **14 false greens**, 2 benign accepted, 0 unexpected rejections.

## False-green analysis

1. **Competing or unconditional primary (F1: R01, R03, R04, R05)** is a real gap, not a pathological one. All four are ordinary edits: trimming an allowlist, renaming a button, adding a header button, or adding a style class.
   - The accepted design leaves Back to forest statically `btn-primary` (`LearningObjectivesView.cs:79`) and relies on `GuidanceButtonNames` (`ScenarioOneUiRoot.cs:61-67`) plus the demotion loop (`:444-450`) to remove it during the guided window.
   - The scanner checks the loop mechanism but not that the competing button is inside its scope.
   - The 08 PASS note ("scoped primary-class restoration/demotion … are checked") therefore overstates what is proved.
   - #297's current feature head `1b68ddcd` does **not** touch this region: it changes 4 Map files only. The risk is a plausible future edit vector, not something already in the tree.
2. **Override or neutralise statements (F2)** need an extra statement placed next to an untouched anchor. Bounded lexical scanners share this weakness, and the handoffs disclose it ("not a C# compiler, reachability analysis or UI test"). R08 and R11 are the most natural of these; each is one line.
3. **Mask choices:**
   - Comments are masked correctly (R10).
   - Literals are deliberately kept, because anchors match `"btn-primary"` and similar. As a result, literal-free anchors can be satisfied by code-like text inside a string (R09).
   - Duplicating an anchor inside a literal fails safe: `_one` reports "found 2".
4. **Regex anchoring is sound where it matters:**
   - `\b` word boundaries hold.
   - The `\s*$` anchor on the final return holds (R16).
   - Exact suspension and eligibility expressions are checked, so dropping or negating any term is rejected.
   - The prefix-only screen-scope ternary and the `(learning.TeachingRacks\b` gate are intentionally partial.

## Old-baseline comparison (ran myself)

`diff baseline-output.txt <72b4efbd pre/pre stdout>` changes only the test-08 block:

```diff
48,51c48
< SOURCE-STATIC FAIL 08 objective-primary-cta: Require exactly one primary source button invoking ui.ShowMap(); labels are evidence only. Baseline primary invokes ui.CloseAll(); visual prominence, focus and destination behaviour remain unmeasured.
<   SOURCE-STATIC Assets/ForestPrototype/UI/LearningObjectivesView.cs:73: buttons.Add(UiKit.Button("Stand map [M]", () => ui.ShowMap()));
<   SOURCE-STATIC Assets/ForestPrototype/UI/LearningObjectivesView.cs:74: buttons.Add(UiKit.Button("Work Plan [Tab]", () => ui.ShowWorkPlan()));
<   SOURCE-STATIC Assets/ForestPrototype/UI/LearningObjectivesView.cs:75: buttons.Add(UiKit.Button("Back to forest [O / Esc]", () => ui.CloseAll(), true, "btn-primary"));
---
> SOURCE-STATIC FAIL 08 objective-primary-cta: Source-structure contract unmet: named map button with exact caption and ShowMap click: expected one structural match, found 0. This is not a Unity runtime verdict.
```

- The genuine old-baseline failure is preserved: 08 still FAILs, with a completed tally and exit 1.
- Every other line, including `tracked-production-vs-baseline: MATCH` and the six SnapFar lines, is byte-identical.
- This matches the worker's raw `07` and `08` files.
- Caveat (F6): the old source now fails on the first structural precondition (the map is not a named variable). It no longer fails because the scanner detected a CloseAll primary, and it prints no evidence rows.

## .NET 8 pinning and the six SnapFar receipts

**SDK resolution (ran myself, via the logging shim):**

- The checker runs `dotnet run --project <tmp>/check.csproj` with **cwd = the repository** (`subprocess.run` has no `cwd`).
- The `dotnet` muxer looks for `global.json` from the **current working directory**, not from the project directory.
- With the #301 method (`TMPDIR` holding `global.json`, cwd = repo), the shim logged:
  - muxer `10.0.114`
  - `dotnet msbuild <project> -getProperty:NETCoreSdkVersion` → `10.0.114`, MSBuildBinPath `/usr/lib/dotnet/sdk/10.0.114`, MSBuild `18.0.36`
  - The probe the worker used (`dotnet --version` run *inside the temp project directory*) reports `8.0.133`, which is not the SDK the checker actually uses. See F4.
- With cwd moved below that `global.json` (run 6), the shim logged muxer `8.0.133`, `NETCoreSdkVersion 8.0.133`, MSBuildBinPath `/usr/lib/dotnet/sdk/8.0.133`, MSBuild `17.8.60`.
- **That genuinely SDK-8 run's stdout is byte-identical (`3128d28d…2af53a`)** to the SDK-10 run and to the worker's raw `04` and `05`. The conclusion "qualification does not depend on the newer SDK" therefore holds on my evidence, not on #301's.

**Captured driver project (ran myself):**

```xml
<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net8.0</TargetFramework><EnableDefaultCompileItems>false</EnableDefaultCompileItems></PropertyGroup><ItemGroup><Compile Include="/media/jer/Files/CCF/Operations/CCF-FeedbackCTARepair-20261106/Assets/ForestPrototype/Racks/RackGeometry.cs" /><Compile Include="/media/jer/Files/CCF/Operations/CCF-FeedbackCTARepair-20261106/Assets/ForestPrototype/Racks/RackEditing.cs" /><Compile Include="SnapCheck.cs" /></ItemGroup></Project>
```

`SnapCheck.cs` calls `RackEditing.SnapFar(...)` and `RackEditing.DirectedAngleDegrees(...)` directly; there is no mock or stub. The driver targets `net8.0`, so it runs on the 8.0.x runtime (8.0.33 installed; inferred from the default roll-forward policy, not logged).

**Six receipts (ran myself; identical under SDK 10 and SDK 8, and on the old baseline):**

```text
SOURCE-STATIC PASS SnapFar near-angle enabled=True input=(20.000,0.700) far=(20.012,0.000) angle=0.000 angleSnapped=True boundarySnapped=False
SOURCE-STATIC PASS SnapFar near-angle-disabled enabled=False input=(20.000,0.700) far=(20.000,0.700) angle=2.005 angleSnapped=False boundarySnapped=False
SOURCE-STATIC PASS SnapFar outside-angle-window enabled=True input=(20.000,3.000) far=(20.000,3.000) angle=8.531 angleSnapped=False boundarySnapped=False
SOURCE-STATIC PASS SnapFar near-boundary enabled=True input=(39.000,0.000) far=(40.000,0.000) angle=0.000 angleSnapped=False boundarySnapped=True
SOURCE-STATIC PASS SnapFar near-boundary-disabled enabled=False input=(39.000,0.000) far=(39.000,0.000) angle=0.000 angleSnapped=False boundarySnapped=False
SOURCE-STATIC PASS SnapFar outside-boundary-window enabled=True input=(37.000,0.000) far=(37.000,0.000) angle=0.000 angleSnapped=False boundarySnapped=False
```

## Published raw evidence (#301)

- `sha256sum -c ../SHA256SUMS.txt` in `/media/jer/Files/CCF/Verification/ScenarioOneFeedbackCtaQualification-20261106/raw/` gives **19/19 OK, exit 0**. `SHA256SUMS.txt` sits in the parent directory with relative names.
- Raw exit files: `02=0`, `03=0`, `04=0`, `05=0`, `07=1`. Every stderr file is empty.
- Tallies: `04` and `05` are 10/0/2; `07` is 6/4/2. Each matches my own runs byte-for-byte (`03` ↔ my run 3, `04` ↔ run 4, `05` ↔ run 6, `07` ↔ run 8).
- `00-environment.txt` itself records `PWD=<repo worktree>` and `DOTNET_SDK_ACTIVE=10.0.114`. `06-sdk8-pin-proof.txt` records a `dotnet --version` probe run *in the tempdir*. Both are consistent with F4.

## Evidence classification

| Claim | Class |
|---|---|
| Delta is 3 verification files + 2 docs, 0 `Assets/` lines; 7f5278b is an ancestor | **(a)** executed now |
| PR #300 branch carries `ffe7c21c`/`065feb12` production UI commits not on main | **(a)** executed now (git ancestry and diff) |
| 01–07/09 code unchanged; `BASE` unchanged; `baseline-output.txt` blob unchanged | **(a)** executed now |
| py_compile 0; 20/20 rejected; 10/0/2 exit 0; 6/4/2 exit 1; six SnapFar PASS | **(a)** executed now, and byte-identical to the worker's raw files |
| Real production `RackEditing.cs`/`RackGeometry.cs` compiled, no mock | **(a)** executed now (captured csproj and driver) |
| SDK 8.0.133 run byte-identical | **(a)** executed now with a genuine cwd pin; **(b)** the #301 claim of having done so is not supported by its own method (F4) |
| Driver runs on runtime 8.0.33 | **(c)** inferred (net8.0 + default roll-forward); not logged |
| Raw #301 logs untampered relative to their checksum file | **(a)** executed now. Who wrote the files and their timestamps is **(b)** worker-reported. |
| #301 worker's clean preflight, start/end HEAD `39cda9a9`, #297 worktree untouched | **(b)** worker-reported only (I saw clean status at `f294fe07` now) |
| 14 independent false greens and their outcomes | **(a)** executed now (lexical only; mutated C# **not compiled**) |
| Functional impact of each mutation (two primaries, CTA lost, etc.) | **(c)** inferred from source; **not run in Unity** |
| F1 prototype guard accepts live source and rejects R01/R03/R04/R05 | **(a)** executed now (scratch prototype; not a source change) |
| Rendered CTA prominence, focus, native pointer/keyboard, Help-close runtime frame, 720p/1080p, map/minimap parity, saves, Player/Windows build | **(d)** NOT RUN / Unity — none attempted |

## Findings

### F1 — Major, blocking: competing and unconditional `btn-primary` sources are not checked

**Where:** `objective_cta_source_check.py:101-107`; the test-08 PASS note at `check_baseline.py:252-257`.

**What happens:** test 08 passes in all of these cases:

- Back to forest is dropped from the scoped demotion allowlist (R03).
- Back to forest is renamed out of it (R04).
- A new unnamed static `btn-primary` CloseAll button is added to the header (R05).
- The map gets `btn-primary` through a separate statement (R01).

The dedicated "must not be unconditionally primary" check (`:101-103`) can never fire, because the constructor regex already forbids extra arguments.

**Impact (inferred from source):**

- For R03, R04 and R05: during unconfirmed Rack V2 Objective 1, the Objectives header shows a CloseAll button as `btn-primary` next to the guided map CTA. That brings back the defect the original test 08 rejected.
- For R01: the map is primary outside the guided window, which breaks the accepted conditional design.
- The old test 08 would have failed the static equivalents of R03, R04 and R05. On this axis, the new test is more lenient than the one it replaces.

**Requested change (verification files only):**

1. In the Objectives constructor body, collect every button that receives `btn-primary`, whether as an inline `UiKit.Button` argument or through `AddToClassList` / `EnableInClassList`. Require:
   - none of them is the map variable;
   - no static-primary button is unnamed;
   - each one has exactly one plain-literal `.name` that is a member of `GuidanceButtonNames`.
2. Remove the dead check at `:101-103`, or fold it into step 1.
3. Add seeded fixtures equivalent to R01, R03, R04 and R05, making 24 or more seeded mutations in total.
4. Update the 08 PASS note.

My prototype (`$S/prototype_f1_guard.py`, about 25 lines) passes the live source (static primaries `['forest']`) and rejects all four mutations.

### F2 — Major by the dispatch floor, non-blocking: presence-and-order matching accepts overriding or cancelling statements

**Where:** `_ordered` and `_one` (`:56-69`), literal-kept extraction (`:52`), no preprocessor awareness, and no coverage of the `CloseHelp` setter or the `LateUpdate` call.

**Confirmed false greens:**

- R08: the map's name is overwritten, so the CTA is never shown.
- R11: the Objectives target is overridden to Back to forest / CloseAll.
- R12, R13, R15: the CTA is never styled.
- R09: code-like text hidden in a string literal.
- R14: `#if UNITY_EDITOR`.
- R20, R21: the help-close and LateUpdate guards are bypassed.

**Why the floor applies:** these could hide the accepted conditional-CTA regression, so the dispatch floor makes them Major.

**Why not blocking:** each needs an unusual added statement that contradicts an intact, checked statement. They are inherent to the bounded lexical design, which the handoffs disclose.

**Suggested cheap hardening (optional; Manager's call):**

- Exactly one `.name =` assignment on the map variable (R08).
- Exactly one `target =` assignment after the Rack V2 branch's `else return;` (R11).
- Reject `#if` / `#endif` inside the extracted methods (R14).
- Match literal-free anchors against literal-masked text (R09).
- Anchor the `CloseHelp` setter and the `LateUpdate` call (R20, R21).

Everything else should be recorded as a documented limitation.

### F3 — Minor: CTA-name uniqueness check only recognises one spelling

**Where:** `:87`.

`work.name = @"objective-primary-cta"` passes (R07). The Objective 1 CTA is still the map, because the map comes first in UQuery's depth-first order (inferred). But Work Plan CTA routing for later T2 states is lost.

**Suggestion:** require the substring `objective-primary-cta` to occur exactly once across all literals in the constructor.

### F4 — Minor (qualification evidence): the #301 "SDK 8.0.133 pinned" run used SDK 10.0.114

**Where:** `BASELINE-CTA-CHECKER-QUALIFICATION-HANDOFF.md` § SDK pinning, and raw `06-sdk8-pin-proof.txt`.

The handoff says `tempfile` "creates the driver project beneath it, so SDK resolution selects 8.0.133". That is incorrect for this checker: the muxer resolves the SDK from the cwd, which is the repo. My reproduction of that exact method logged SDK 10.0.114 and MSBuild 18.0.36.

The substantive claim survives: my genuinely pinned run (cwd under the `global.json`; SDK 8.0.133, MSBuild 17.8.60) is byte-identical.

**Requested:**

- The Manager record should cite this review's run, not #301's, for SDK-8 evidence.
- The integrated-candidate rerun should pin by running the checker **from a cwd below the pinning `global.json`**, and log `dotnet --version` from that cwd.

### F5 — Minor (docs drift): README describes the old test 08

**Where:** `README.md:34`, plus the exit-2 statement at `:17`.

- `:34` still says test 08 "requires exactly one button carrying `btn-primary` whose callback invokes `ui.ShowMap()` … The current primary button invokes `ui.CloseAll()` and fails".
- `:17` says missing anchors give exit 2; test 08 now reports a FAIL row with exit 1 instead.

README was outside the #299/#301 allowed files. Updating it needs Manager scope, and should travel with the transferred delta.

### F6 — Minor: test 08 FAIL output is hard to diagnose

**Where:** `check_baseline.py:258-261`; `objective_cta_source_check.py:101-103`.

- A FAIL prints `[]` evidence rows and only the first unmet assertion.
- On the old baseline it reports "named map button … found 0" instead of the substantive "primary invokes CloseAll".
- Regression sensitivity is preserved; the problem is diagnosis.
- The dead check at `:101-103` implies coverage that does not exist; it is fixed together with F1.

### F7 — Nit

- `line_at` (`:178-180`) searches for exact-spacing fragments, so a reformat that still matches the regex would print evidence line `:0`.
- `check_baseline.py:13` imports the sibling module at the top level. Transferring `check_baseline.py` without `objective_cta_source_check.py` would crash with an `ImportError` traceback (exit 1, not the documented exit 2). Keep the three files together.

## Limits and NOT RUN

- **NOT RUN:**
  - Unity Editor or Play Mode.
  - Native pointer/keyboard input and focus.
  - The Help-close runtime frame.
  - Rendered CTA prominence at 1280×720 or 1920×1080.
  - Map/minimap rendered trail parity.
  - New/old saves, reference or legacy worlds.
  - Player or Windows builds.
  - Compiling any mutated C#.
  - The checker on the future integrated (post-#296/#297) candidate.
- All mutation outcomes are lexical scanner verdicts. Their functional impacts are inferred from source.
- This is a source-static review. A PASS of test 08 is a bounded structure check, not a UI verdict.
- I wrote no production, verification or coordination file. The only write in the review lane is this report.
- `active-tasks.md`, PR #300, the #296/#297 worktrees and `/media/jer/Files/CCF-Primary`'s uncommitted files were not touched.

## Manager #200 decision requested

Please issue **APPROVED / NEEDS CHANGES / REJECTED** for the #299/#301 source-static repair. That decision is the Manager's, not this reviewer's.

My recommendation is **NEEDS CHANGES**:

- Scope a bounded verification-only amendment for **F1**: the extra assertion, 4 new seeded fixtures and the updated PASS note.
- Optionally fold in the F2 and F3 hardening.
- Allow a README update (F5).
- Then require a rerun of py_compile, the mutations, the full checker (post/post), the old baseline (pre/pre) and a genuine cwd-pinned SDK-8 run.
- Then require a short re-review of only that diff.

If the Manager instead accepts F1 as a documented limitation, the Manager record should state plainly that test 08 does not prove the Back to forest primary is demoted.

In every case:

- **Do not merge PR #300 wholesale.**
- Transfer only the reviewed verification delta to the exact integrated candidate and rerun it there.
- Keep Unity and rendered CTA qualification as a separate gate.
