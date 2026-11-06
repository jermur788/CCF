# Historical Objective CTA checker — local qualification handoff

**Task:** [#301](https://github.com/jermur788/CCF/issues/301) — local qualification of the committed [#299](https://github.com/jermur788/CCF/issues/299) verifier repair (draft [PR #300](https://github.com/jermur788/CCF/pull/300)).
**Date:** 2026-11-06
**Status:** **LOCAL QUALIFICATION COMPLETE — SOURCE-STATIC PASS. NOT INDEPENDENTLY REVIEWED, NOT ACCEPTED, NOT INTEGRATED.**
**Verdict requested from Overall Manager:** APPROVED / NEEDS CHANGES / REJECTED on the exact evidence below.

## Implementation handoff

| Field | Value |
|---|---|
| Task | #301 local Python/.NET qualification of the #299 CTA source-checker repair |
| Worker | T3 Code / OpenCode Go — DeepSeek V4.1 Flash (orchestrator and local executor) |
| Role | Local verification worker (PACKET ONLY); no product, review or acceptance authority |
| Branch | `verify/scenario-one-feedback-cta-anchor-20261106` |
| Base | Accepted Objective-guidance candidate `7f5278bfd42997da055da63423b6b4c30c7bb8f3` (ancestor of HEAD, verified) |
| Context commit | Not pinned by the #301 packet. Live coordination read from `origin/main` `9d4cc223` (2026-11-06T12:07Z). Branch merge-base with `origin/main` = `e1be15c17a403bc78130a7f01a3248d537960fd8`. |
| HEAD start | `39cda9a97393e4d7942ca43e81fc6ab486377700` |
| HEAD end | `39cda9a97393e4d7942ca43e81fc6ab486377700` (unchanged; this handoff is the only new commit) |
| Isolated worktree | `/media/jer/Files/CCF/Operations/CCF-FeedbackCTARepair-20261106` (created for this task; did not exist before) |

## Ownership and preflight

Observed before any run, in the new worktree:

```text
$ git status --short --branch
## verify/scenario-one-feedback-cta-anchor-20261106...origin/verify/scenario-one-feedback-cta-anchor-20261106
```

- Worktree did not exist before this task; `git worktree list` contained no checkout of this branch.
- Branch/HEAD matched `origin/verify/scenario-one-feedback-cta-anchor-20261106` = `39cda9a9`; tracking branch created with `git worktree add --track`.
- Tracked and untracked cleanliness: **0** modified and **0** untracked entries at start and at end.
- Mount/space: `/media/jer/Files` (ext4, durable) with **83 GiB** available; no `Library`, `Temp`, `obj` or `Logs` in this worktree.
- Toolchain: Python 3.12.3, Git 2.43.0, .NET SDK 8.0.133 present (plus 10.0.114), runtime `Microsoft.NETCore.App 8.0.33`.
- **#297 exclusion:** the reserved #297 worktree `/media/jer/ZX20/CCF-NextActionGuidance-T3-20261106` was read once and left untouched at `1b68ddcd583ab2bddd43b5ce18365171c0bab3b4` on `feature/scenario-one-next-action-guidance-v1`, clean. This task did not use it, did not share its branch, and did not touch #296/#297 files.
- **No Unity:** no Editor, Player or build was launched (no Unity process observed before or after). This task never enters `Assets/ForestPrototype`.
- **#200 registration:** this worker did not edit `Docs/Project/coordination/active-tasks.md`; during the two-lane split that file is single-writer to #200. The worktree claim above is recorded here and in the #301 GitHub thread, and the coordination-file registration remains with #200. See "Decisions needed".

## Commands actually run (exact, in order)

All runs used the working directory `/media/jer/Files/CCF/Operations/CCF-FeedbackCTARepair-20261106`.

```bash
git status --short --branch
python3 -m py_compile Tools/Verification/OvernightPlaytestFeedback/check_baseline.py Tools/Verification/OvernightPlaytestFeedback/objective_cta_source_check.py Tools/Verification/OvernightPlaytestFeedback/test_objective_cta_source_check.py
python3 Tools/Verification/OvernightPlaytestFeedback/test_objective_cta_source_check.py
python3 Tools/Verification/OvernightPlaytestFeedback/check_baseline.py --rotation-state post-fix --instruction-state post-fix
# repeat of the last command with the .NET 8 SDK pinned (see "SDK pinning")
TMPDIR=/tmp/ccf-sdk8 python3 Tools/Verification/OvernightPlaytestFeedback/check_baseline.py --rotation-state post-fix --instruction-state post-fix
# same-source old-baseline comparison in a disposable detached worktree at 72b4efbd (see below)
python3 Tools/Verification/OvernightPlaytestFeedback/check_baseline.py --rotation-state pre-fix --instruction-state pre-fix
```

Raw stdout, stderr and exit codes are preserved byte-exact at
`/media/jer/Files/CCF/Verification/ScenarioOneFeedbackCtaQualification-20261106/raw/`
with `../SHA256SUMS.txt`.

## Result summary

| Run | Exit | Result |
|---|---|---|
| `py_compile` (3 modules) | **0** | no stdout, no stderr |
| `test_objective_cta_source_check.py` | **0** | 20/20 mutation controls rejected, 0 false greens |
| `check_baseline.py --rotation-state post-fix --instruction-state post-fix` | **0** | `PASS=10 FAIL=0 NOT-TESTABLE=2` |
| Same run, .NET SDK 8.0.133 pinned | **0** | byte-identical stdout to the run above |
| Old baseline `72b4efbd` with the repaired checker, pre-fix flags | **1** | `PASS=6 FAIL=4 NOT-TESTABLE=2` — reproduces the stored `baseline-output.txt` tally |

**No test was weakened, no expectation was changed, no fixture or `baseline-output.txt` was edited.** Zero source files were modified: the only commit produced by this task is this handoff.

## Named test tally (01–09, 06a/b/c) on the qualified source

Source read: the branch's production tree (accepted Objective-guidance candidate, `7f5278b` family).

| Test | Result | Note |
|---|---|---|
| 01 requested-spacing-presentation-format | PASS | compact `16–20m` present |
| 02 tree-marking-heading | PASS | exact heading literal present |
| 03 source-material-copy | PASS | no "CCF source material" literal |
| 04 orange-entry-is-not-fell | PASS | legend separation intact |
| 05 no-formal-pro-silva-copy | PASS | no Pro Silva literal |
| 06 round-handle-input | **NOT-TESTABLE** | physical input; observation only |
| 06a rotation-snap-policy (post-fix) | PASS | observed `[False, False, True, False, True, False]` |
| 06b snapfar-offline-cases | PASS | six real `SnapFar` calls, all PASS |
| 06c rack-edit-instruction-copy (post-fix) | PASS | exact post-fix sentence |
| 07 objective-next-instruction | PASS | Plan Racks / Stand Map [M] path |
| 08 objective-primary-cta | PASS | new bounded structural proof (see below) |
| 09 map-minimap-trail-consistency | **NOT-TESTABLE** | rendered parity; observation only |

`SOURCE-STATIC tracked-production-vs-baseline: DIFF` is expected and correct: this branch carries the accepted Objective-guidance source, not the `72b4efbd` player-test baseline. It is not a failure.

## Check 08 — the repaired anchor

The historical check 08 looked for an inline `buttons.Add(UiKit.Button(... "btn-primary"))` and aborted with exit 2 when the source changed. The repair replaces it with a bounded structural contract over the real `LearningObjectivesView` constructor and `ScenarioOneUiRoot.RefreshNextActionGuidance` / `ApplyNextActionGuidance` / `ClearNextAction` / `GuidanceVisible`. On this source it reports six anchored evidence rows:

```text
SOURCE-STATIC PASS 08 objective-primary-cta: Bounded source-structure proof: one named Objectives map CTA invokes ui.ShowMap(), is bound to the header row and selected only for unconfirmed Rack V2 Objective 1; scoped primary-class restoration/demotion, Help/preview/visibility guards and dynamic btn-primary/next-action application are checked. Stateful Unity input, focus and rendered prominence remain NOT TESTED.
  SOURCE-STATIC Assets/ForestPrototype/UI/LearningObjectivesView.cs:73: Named Stand map [M] button -> ui.ShowMap(), added to Objectives header row
  SOURCE-STATIC Assets/ForestPrototype/UI/ScenarioOneUiRoot.cs:375: Conditional Rack V2 / unconfirmed Objective 1 target
  SOURCE-STATIC Assets/ForestPrototype/UI/ScenarioOneUiRoot.cs:444: Scoped class demotion with original primary restoration
  SOURCE-STATIC Assets/ForestPrototype/UI/ScenarioOneUiRoot.cs:476: Reference/menus/Help and help-close suspension
  SOURCE-STATIC Assets/ForestPrototype/UI/ScenarioOneUiRoot.cs:480: Enabled, visible, descendant-of-scope eligibility
  SOURCE-STATIC Assets/ForestPrototype/UI/ScenarioOneUiRoot.cs:487: Dynamic primary and next-action class application
```

## 20/20 negative mutation controls

Full stdout (`raw/03-cta-mutations.stdout.txt`, sha256 `77dfd933…83304`, exit 0):

```text
SOURCE-STATIC PASS objective-cta-live-source: 6 anchored evidence rows (UI runtime NOT TESTED)
SOURCE-STATIC PASS mutation-map-button-name: rejected (Objective CTA name must be unique and bound to the ShowMap button)
SOURCE-STATIC PASS mutation-map-click-destination: rejected (named map button with exact caption and ShowMap click: expected one structural match, found 0)
SOURCE-STATIC PASS mutation-map-caption: rejected (named map button with exact caption and ShowMap click: expected one structural match, found 0)
SOURCE-STATIC PASS mutation-map-not-added-to-row: rejected (Objectives constructor/row binding: expected one structural match, found 0)
SOURCE-STATIC PASS mutation-duplicate-map-identity: rejected (Objective CTA name must be unique and bound to the ShowMap button)
SOURCE-STATIC PASS mutation-target-renamed: rejected (RefreshNextActionGuidance/no rack plan yet: expected one structural match, found 0)
SOURCE-STATIC PASS mutation-objective-condition-removed: rejected (RefreshNextActionGuidance/no rack plan yet: expected one structural match, found 0)
SOURCE-STATIC PASS mutation-rack-v2-gate-removed: rejected (RefreshNextActionGuidance/rack teaching gate: expected one structural match, found 0)
SOURCE-STATIC PASS mutation-help-guard-removed: rejected (ApplyNextActionGuidance/preview/menus/help suspension: expected one structural match, found 0)
SOURCE-STATIC PASS mutation-reference-preview-guard-removed: rejected (ApplyNextActionGuidance/preview/menus/help suspension: expected one structural match, found 0)
SOURCE-STATIC PASS mutation-candidate-enabled-check-removed: rejected (ApplyNextActionGuidance/candidate scoped visibility: expected one structural match, found 0)
SOURCE-STATIC PASS mutation-scope-visibility-check-removed: rejected (ApplyNextActionGuidance/candidate scoped visibility: expected one structural match, found 0)
SOURCE-STATIC PASS mutation-hidden-visibility-check-removed: rejected (visibility and display suspension: expected one structural match, found 0)
SOURCE-STATIC PASS mutation-allowlist-primary-dedup-removed: rejected (RefreshNextActionGuidance/bounded class demotion: expected one structural match, found 0)
SOURCE-STATIC PASS mutation-unrelated-global-demotion: rejected (RefreshNextActionGuidance/bounded class demotion: expected one structural match, found 0)
SOURCE-STATIC PASS mutation-primary-restore-removed: rejected (RefreshNextActionGuidance/restore old primary classes: expected one structural match, found 0)
SOURCE-STATIC PASS mutation-primary-class-not-added: rejected (ApplyNextActionGuidance/apply primary: expected one structural match, found 0)
SOURCE-STATIC PASS mutation-next-action-class-not-added: rejected (ApplyNextActionGuidance/apply next-action: expected one structural match, found 0)
SOURCE-STATIC PASS mutation-eligibility-return-removed: rejected (ApplyNextActionGuidance/no styling if ineligible: expected one structural match, found 0)
SOURCE-STATIC PASS mutation-target-not-bound-to-scoped-button: rejected (RefreshNextActionGuidance/candidate resolved in its scope: expected one structural match, found 0)
SOURCE-STATIC CTA MUTATIONS rejected=20/20 false_greens=0
```

Each control mutates an in-memory copy only; `replace_one` first asserts the live anchor occurs exactly once, so a silently missing anchor would fail the control itself rather than pass.

## Six real .NET SnapFar cases (06b)

Real production `RackEditing.SnapFar` / `RackGeometry` compiled from this checkout into an OS temporary directory outside the repository, target framework `net8.0`, six direct calls, 0.001 m tolerance:

```text
SOURCE-STATIC PASS 06b snapfar-offline-cases: Six direct calls to production SnapFar on plain .NET; 0.001m tolerance; no Unity or gesture dispatch proof.
  SOURCE-STATIC Assets/ForestPrototype/Racks/RackEditing.cs:259: public static RackSnapResult SnapFar(RackEditBounds bounds, RackPoint entrance, RackPoint desiredFar,
SOURCE-STATIC PASS SnapFar near-angle enabled=True input=(20.000,0.700) far=(20.012,0.000) angle=0.000 angleSnapped=True boundarySnapped=False
SOURCE-STATIC PASS SnapFar near-angle-disabled enabled=False input=(20.000,0.700) far=(20.000,0.700) angle=2.005 angleSnapped=False boundarySnapped=False
SOURCE-STATIC PASS SnapFar outside-angle-window enabled=True input=(20.000,3.000) far=(20.000,3.000) angle=8.531 angleSnapped=False boundarySnapped=False
SOURCE-STATIC PASS SnapFar near-boundary enabled=True input=(39.000,0.000) far=(40.000,0.000) angle=0.000 angleSnapped=False boundarySnapped=True
SOURCE-STATIC PASS SnapFar near-boundary-disabled enabled=False input=(39.000,0.000) far=(39.000,0.000) angle=0.000 angleSnapped=False boundarySnapped=False
SOURCE-STATIC PASS SnapFar outside-boundary-window enabled=True input=(37.000,0.000) far=(37.000,0.000) angle=0.000 angleSnapped=False boundarySnapped=False
```

## SDK pinning

The harness targets `net8.0` but has no `global.json`, so the host's default SDK (10.0.114) builds it. To prove the required .NET 8 toolchain specifically, the run was repeated with `TMPDIR` pointing at a directory holding a `global.json` that pins `"version": "8.0.133", "rollForward": "disable"`; `tempfile` then creates the driver project beneath it, so SDK resolution selects 8.0.133.

```text
tempdir: /tmp/ccf-sdk8/sdk-probe-ynb4vhrb
dotnet --version in tempdir: 8.0.133
```

The SDK-8 run's stdout is **byte-identical** to the SDK-10 run (`diff` produced no output; both sha256 `3128d28d…2af53a`), so the qualification does not depend on the newer SDK.

## Same-source old-baseline comparison

A disposable detached worktree at `72b4efbd1c653fa2231f632a79030a83ff76f833` (the #260 baseline) was created under `/tmp`, the three repaired checker files were copied in, and the checker was run with pre-fix flags. The worktree was removed afterwards; no repository ref was changed.

Result: `PASS=6 FAIL=4 NOT-TESTABLE=2`, exit **1** — the exact tally stored in `Tools/Verification/OvernightPlaytestFeedback/baseline-output.txt`. The only textual difference is check 08:

```diff
-SOURCE-STATIC FAIL 08 objective-primary-cta: Require exactly one primary source button invoking ui.ShowMap(); labels are evidence only. Baseline primary invokes ui.CloseAll(); visual prominence, focus and destination behaviour remain unmeasured.
-  SOURCE-STATIC Assets/ForestPrototype/UI/LearningObjectivesView.cs:73: buttons.Add(UiKit.Button("Stand map [M]", () => ui.ShowMap()));
-  SOURCE-STATIC Assets/ForestPrototype/UI/LearningObjectivesView.cs:74: buttons.Add(UiKit.Button("Work Plan [Tab]", () => ui.ShowWorkPlan()));
-  SOURCE-STATIC Assets/ForestPrototype/UI/LearningObjectivesView.cs:75: buttons.Add(UiKit.Button("Back to forest [O / Esc]", () => ui.CloseAll(), true, "btn-primary"));
+SOURCE-STATIC FAIL 08 objective-primary-cta: Source-structure contract unmet: named map button with exact caption and ShowMap click: expected one structural match, found 0. This is not a Unity runtime verdict.
```

That is the intended repair behaviour: the old baseline still **fails** check 08, but as a completed tally row instead of an exit-2 abort, and checks 01–07/09 are byte-identical. The repaired checker is not more permissive on the old source.

## Verification tier boundary

- **PROVEN (SOURCE-STATIC / offline):** Python compilation; the full numbered checker on the qualified source; 20/20 negative controls; six real production `SnapFar` calls on .NET 8; the historical baseline comparison; a clean tree at a single unchanged HEAD.
- **NOT TESTED (explicitly out of scope):** Unity Editor and Play Mode, any rendered CTA at 1280×720 or 1920×1080, real pointer/keyboard input, Help-close frame state, focus order, visual prominence of `btn-primary`, map/minimap rendered trail parity, saves, Player or Windows builds. No `Assets/ForestPrototype` file was read for modification or opened in Unity.

## Remaining verification gates

1. Independent source review of the exact #299 delta (`check_baseline.py`, `objective_cta_source_check.py`, `test_objective_cta_source_check.py`) against this evidence.
2. Overall Manager disposition: APPROVED / NEEDS CHANGES / REJECTED.
3. Later integration must cherry-pick only the reviewed verification delta onto the post-#297/#296 candidate and rerun the checker there.
4. Any runtime/rendered CTA claim remains a separate Unity gate.

## Known limitations

- The scanner is a bounded lexical/structural check, not a C# compiler, reachability analysis or UI test.
- The 20 mutation controls prove rejection of the specific seeded defects only.
- Check 06b exercises isolated snapping math; it does not prove gesture dispatch or headland-edit validity.
- `baseline-output.txt` was read and compared, never rewritten.

## Unrelated work preserved

- `/media/jer/Files/CCF-Primary` still carries its pre-existing uncommitted `AGENTS.md` and `Docs/Operations/StorageArchitecture.md` modifications; they were not touched.
- The #297 worktree and branch were left exactly as found.
- The disposable `/tmp` baseline worktree and the `__pycache__` produced by `py_compile` were removed; final `git status --short --branch` shows a clean tree with 0 untracked entries.

## Decisions needed

1. **#200 coordination registration:** the isolated worktree is claimed in this handoff and the #301 thread, but the single-writer coordination file was deliberately not edited by this worker. #200 should record the owner/state or confirm the issue thread is sufficient.
2. Independent reviewer assignment and Manager verdict on this exact evidence.

## Worker assessment

Ready for review: **Yes** (SOURCE-STATIC qualification complete; no repair was needed, no test weakened).

DASHBOARD SYNC: **NOT AVAILABLE** — no dashboard publisher is reachable from this isolated verification worktree, and during the two-lane split publication is owned by the #200 browser lane.
PUBLISHED DASHBOARD REFLECTS THIS TASK: **NO**
DASHBOARD SYNC FOLLOW-UP:
- #200 should publish the #301 qualification result through the normal dashboard ingestion path once the reviewer/Manager verdict exists; this worker cannot write the coordination file or trigger ingestion.

## Recommended next step

Assign a separate reviewer (different provider from the executor where genuinely available) to review the three verification files and this evidence, then return APPROVED / NEEDS CHANGES / REJECTED. **Do not merge; no Unity, Player or Windows build is authorised by this task.**
