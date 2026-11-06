# #304 — CTA checker F1/F2/F3/F5/F6 repair handoff

**Status: HANDED-OFF for independent re-review. NOT approved.** This is a source/offline verification-tooling repair. It is not Unity, render, input or runtime evidence and carries no approval authority. #299 stays open pending an independent targeted re-review and a Manager #200 verdict.

## Task and authority

- Claim: [#304](https://github.com/jermur788/CCF/issues/304) (parents #299, #301, #302). Binding packet: `MANAGER-CTA-CHECKER-REVIEW-DISPOSITION-AND-REPAIR-20261106.md`. Role: sole Stream B implementation/test writer. I did not edit `active-tasks.md`, did not review or approve my own work and did not commission a reviewer.
- Draft PR #300 is **not** merged and must not be merged wholesale. Only this reviewed checker delta (three Python files plus README) may later transfer to the exact approved combined candidate, where every check must be re-run.
- Worker (actual): **Claude Sonnet 5.5** (`claude-sonnet-5-5`), Anthropic, via Claude Code in T3 Code. Observed reasoning effort: the harness set `reasoning_effort=10`, i.e. **low**. It was not xhigh. The packet recommended a high-effort worker, so the re-reviewer should weigh this.

## Preflight evidence (before [RUNNING])

| Check | Observed |
|---|---|
| Worktree / branch | `/media/jer/Files/CCF/Operations/CCF-FeedbackCTARepair-20261106` on `verify/scenario-one-feedback-cta-anchor-20261106` |
| Start HEAD | `f294fe0769bdb910eb58576dc53751f669982528` = `origin/<branch>` after `git fetch origin` |
| Status | clean including untracked (`git status -uall` empty) |
| Mount / space | ext4 `rw,nosuid,nodev,relatime`; 79 GiB free; write test OK |
| Other writers | `fuser` showed only my own shell; no other python/git on the worktree |
| Lane | not the ZX20 feature worktree; no saves; `/media/jer/ZX20/CCF-NextActionGuidance-T3-20261106` untouched |

## Changed files (exactly the allowed set)

- `Tools/Verification/OvernightPlaytestFeedback/objective_cta_source_check.py` (rewritten scanner)
- `Tools/Verification/OvernightPlaytestFeedback/test_objective_cta_source_check.py` (original 20 retained verbatim, plus new groups)
- `Tools/Verification/OvernightPlaytestFeedback/check_baseline.py` (test-08 failure block and PASS note only; hunks at lines 255–263)
- `Tools/Verification/OvernightPlaytestFeedback/README.md`
- this handoff (new)

Start HEAD `f294fe07…`. Code commit `040e496a420925398fa2e93f23578f66b41e8ee2`. The handoff is committed on top of it; the final pushed HEAD is recorded in the #304 `[HANDED-OFF]` comment because a file cannot contain its own commit hash.

Protected and byte-unchanged (`git diff f294fe07` empty): `Assets/`, `Packages/`, `ProjectSettings/`, `baseline-output.txt` (blob `db6a8b850dce7efadaac6fe1b87b1dbba469428f`, sha256 `513cddd9…c1`), `BASE` (`72b4efbd…`), original tests 01–07/09 and 06a/06b/06c. No old mutation was removed, skipped or converted to a warning. `git diff --check`: clean.

## Finding disposition

| ID | Disposition |
|---|---|
| **F1** (blocking) | **Repaired.** The scanner mechanically enumerates every `btn-primary` source in the Objectives constructor: inline `UiKit.Button(…, "btn-primary")` (class arguments after the `enabled` bool, including the `params` tail), and `AddToClassList` / `EnableInClassList` / `ToggleInClassList`. Each must be assigned to a named local that is declared once and never reassigned, have exactly one plain-literal name, and that name must be in the *parsed* `GuidanceButtonNames` set (which must itself contain only plain, unique literals and be referenced only by its declaration and the demotion loop). The map may never be a primary source. Named arguments, non-literal class arguments, unnamed or inline primaries, and any `btn-primary` literal outside a recognised shape raise an error (fail closed). The existing ordered checks still verify the scoped restore/remove loop. R01, R03, R04, R05 and variants all fail. |
| **F2/F3** | **Repaired for the bounded variants.** Structural matches must start in code, never inside a comment, string or char literal (R09). CTA identity is decoded across ordinary, verbatim and `\u` spellings and must be unique (R07); the map has exactly one name and no other uses (R08, R15); extra writes to `candidate`, `guidanceCandidate`, `target`, extra early returns and unexpected `candidate` members are rejected (R11, R12, R13); `#if`/`#elif`/`#else`/`#endif`/`#define`/`#undef` inside or enclosing a checked method fails (R14); `CloseHelp` must record `helpClosedFrame` unconditionally after `help.Close()` and nothing else may write it (R20); `LateUpdate` must call `ApplyNextActionGuidance()` before any branch or return (R21). R12, R13 and R15 were also made to fail with narrow guards. |
| **F5** | **Repaired.** README now states the conditional CTA contract, the fail-closed shapes and that test-08 failures complete with FAIL / exit 1 (not exit 2). #301 history and its SDK attribution were not rewritten; I added only the observed fact that the SDK is chosen from the cwd's `global.json`. |
| **F6** | **Repaired.** The dead `:101–103` check is gone; the equivalent rule is now the F1 enumeration. A failing 08 prints the unmet predicate plus the constructor/allowlist source rows. Unexpected scanner exceptions become a completed FAIL, not a crash or pass. The old baseline fails as a completed 08 FAIL (exit 1) because its Back to forest button is an unnamed inline primary. |
| F4 | Evidence only: a genuinely cwd-pinned SDK 8.0.133 run is recorded below. |
| F7 | `line_at` replaced by regex-based line lookup (whitespace tolerant). The three checker files must still be copied together. |

### Remaining lexical limitations (not hidden, not reclassified as PASS)

This is a lexical guard, not a C# parser or compiler. It does not analyse interpolated strings, C# 11 raw strings, `partial` members in other files, USS, other classes, or runtime mutation of the buttons from outside the checked methods. A new primary created through a helper that receives only `forest`, or in a method other than the constructor (for example the view's `Refresh`), is outside the constructor audit. The pinned counts (`target` writes = 14, per-method return counts, state-write counts) describe the reviewed routing; an intentional routing change in the combined tree will fail them and needs a deliberate checker update. Nothing here proves rendering, input, focus, Help-close timing, saves or navigation.

## Mutation results (raw: `02-mutations.txt`, exit 0)

True source (live files): **PASS**, 8 evidence rows. Tallies: **original 20/20 rejected; F1 14/14; F2/F3 21/21; total 55/55 rejected, 0 false greens; 4/4 benign accepted; historical `BASE` source rejected with 3 evidence rows.** The last column shows the same fixture run against the *previous* scanner at `f294fe07`: all 35 new fixtures were false greens there, so they are real discriminating tests.

### F1 (14)

| Case | Result | Rejection reason (verbatim) | Old scanner `f294fe07` |
|---|---|---|---|
| `R01-unconditional-map-primary` | REJECTED | map CTA must not be unconditionally primary (AddToClassList) | FALSE GREEN |
| `R01b-map-primary-via-enable-class` | REJECTED | map CTA must not be unconditionally primary (EnableInClassList) | FALSE GREEN |
| `R01c-map-primary-verbatim-spelling` | REJECTED | map CTA must not be unconditionally primary (AddToClassList) | FALSE GREEN |
| `R01d-map-class-non-literal` | REJECTED | AddToClassList in the header uses a non-literal class name (ambiguous) | FALSE GREEN |
| `R03-back-to-forest-removed-from-allowlist` | REJECTED | static primary button 'objective-back-to-forest' is not in the GuidanceButtonNames scoped demotion set | FALSE GREEN |
| `R04-back-to-forest-renamed-out-of-allowlist` | REJECTED | static primary button 'objective-forest' is not in the GuidanceButtonNames scoped demotion set | FALSE GREEN |
| `R05-new-unnamed-closeall-primary` | REJECTED | static primary 'close' must have exactly one literal name; found 0 | FALSE GREEN |
| `R05b-new-closeall-primary-named-outside-allowlist` | REJECTED | static primary button 'objective-close' is not in the GuidanceButtonNames scoped demotion set | FALSE GREEN |
| `R05c-inline-unnamed-primary` | REJECTED | static btn-primary header button is not assigned to a named local (unnamed primary) | FALSE GREEN |
| `R05d-new-closeall-primary-reusing-allowlisted-name` | REJECTED | duplicate element name in the Objectives constructor | FALSE GREEN |
| `R05e-named-argument-class` | REJECTED | named UiKit.Button argument in the header is an unsupported shape | FALSE GREEN |
| `R05f-primary-class-in-unsupported-shape` | REJECTED | btn-primary literal 'btn-primary' in the header is not in a supported UiKit.Button/AddToClassList shape | FALSE GREEN |
| `R05g-forest-primary-reassigned` | REJECTED | static primary 'forest' must be declared once and never reassigned in the header | FALSE GREEN |
| `allowlist-duplicate-name` | REJECTED | static primary button 'objective-back-to-forest' is not in the GuidanceButtonNames scoped demotion set | FALSE GREEN |

### F2/F3 (21)

| Case | Result | Rejection reason (verbatim) | Old scanner `f294fe07` |
|---|---|---|---|
| `R07-verbatim-duplicate-cta-name` | REJECTED | duplicate element name in the Objectives constructor | FALSE GREEN |
| `R07b-escaped-duplicate-cta-name` | REJECTED | duplicate element name in the Objectives constructor | FALSE GREEN |
| `R07c-concatenated-name` | REJECTED | an element name in the Objectives constructor is not a single plain literal | FALSE GREEN |
| `R08-later-map-name-overwrite` | REJECTED | Objective CTA name must be unique and bound to the ShowMap button; found map names ['objective-primary-cta', 'objective-map'] | FALSE GREEN |
| `R09-anchor-spoofed-in-string-literal` | REJECTED | ApplyNextActionGuidance/no styling if ineligible: expected one structural match, found 0 | FALSE GREEN |
| `R09b-view-anchor-spoofed-in-string-literal` | REJECTED | Objectives constructor/row binding: expected one structural match, found 0 | FALSE GREEN |
| `R11-target-override-to-closeall` | REJECTED | target is reassigned after the routing chain | FALSE GREEN |
| `R12-candidate-nulled-in-apply` | REJECTED | ApplyNextActionGuidance writes 'candidate' more than once (override) | FALSE GREEN |
| `R13-unreachable-return-in-refresh` | REJECTED | RefreshNextActionGuidance has an unexpected early return | FALSE GREEN |
| `R14-primary-hidden-behind-unity-editor` | REJECTED | \bprivate\s+void\s+ApplyNextActionGuidance\s*\(\s*\): conditional compilation (#if) in a checked method is unsupported | FALSE GREEN |
| `R14b-cta-name-hidden-behind-conditional` | REJECTED | \bpublic\s+LearningObjectivesView\s*\(\s*ScenarioOneUiRoot\s+ui\s*\): conditional compilation (#if) in a checked method is unsupported | FALSE GREEN |
| `R15-map-disabled-after-binding` | REJECTED | map CTA variable may only be declared, named and added to the header row; other uses are unsupported | FALSE GREEN |
| `R16b-extra-return-true-in-visibility` | REJECTED | GuidanceVisible has an unexpected return | FALSE GREEN |
| `R17b-candidate-disabled-in-apply` | REJECTED | ApplyNextActionGuidance uses an unexpected candidate member: SetEnabled | FALSE GREEN |
| `R20-help-close-frame-setter-removed` | REJECTED | CloseHelp/help-close frame recorded: expected one structural match, found 0 | FALSE GREEN |
| `R20b-help-close-frame-conditional` | REJECTED | helpClosedFrame is set conditionally in CloseHelp | FALSE GREEN |
| `R20c-help-close-frame-reset` | REJECTED | helpClosedFrame must be written only by its initialiser and CloseHelp | FALSE GREEN |
| `R22-candidate-nulled-in-clear` | REJECTED | ClearNextAction may only drop the guide classes, badge and nextAction | FALSE GREEN |
| `R23-candidate-nulled-by-unrelated-handler` | REJECTED | 'guidanceCandidate' must be written only by RefreshNextActionGuidance/ApplyNextActionGuidance/ClearNextAction | FALSE GREEN |
| `R21-late-update-apply-removed` | REJECTED | LateUpdate applies cached guidance: expected one structural match, found 0 | FALSE GREEN |
| `R21b-late-update-apply-after-return` | REJECTED | LateUpdate may only apply guidance unconditionally before any branch or return | FALSE GREEN |

### Original 20 (unchanged, all still rejected)

`map-button-name`, `map-click-destination`, `map-caption`, `map-not-added-to-row`, `duplicate-map-identity`, `target-renamed`, `objective-condition-removed`, `rack-v2-gate-removed`, `help-guard-removed`, `reference-preview-guard-removed`, `candidate-enabled-check-removed`, `scope-visibility-check-removed`, `hidden-visibility-check-removed`, `allowlist-primary-dedup-removed`, `unrelated-global-demotion`, `primary-restore-removed`, `primary-class-not-added`, `next-action-class-not-added`, `eligibility-return-removed`, `target-not-bound-to-scoped-button`.

### Benign controls (must still PASS): `B01-trailing-comment-after-eligibility-return`, `B02-map-constructor-reformatted`, `B03-comment-mentions-primary-and-conditional`, `B04-new-nonprimary-header-button-unnamed` — 4/4 accepted.

## Commands and outcomes (raw files in `/media/jer/Files/CCF/Verification/CTA-F1F3-Repair-304-20261106/`, with `SHA256SUMS.txt`)

All Python ran with `PYTHONDONTWRITEBYTECODE=1 PYTHONPYCACHEPREFIX=<scratch>` and `python3 -B`; no `__pycache__` in the repo. (`-I` cannot be used here because it also removes the script directory from `sys.path`, which `check_baseline.py` needs to import its sibling.)

| # | Command (cwd = worktree unless noted) | Exit | Result |
|---|---|---|---|
| 1 | `python3 -m py_compile` on the three scripts | **0** | no output |
| 2 | `python3 -B …/test_objective_cta_source_check.py` | **0** | 55/55, 4/4 benign, historical rejected |
| 3 | `python3 -B …/check_baseline.py --rotation-state post-fix --instruction-state post-fix` | **0** | `PASS=10 FAIL=0 NOT-TESTABLE=2`, stderr empty (sha256 `8cc51e4e…fd0`) |
| 4 | same script with `--rotation-state pre-fix --instruction-state pre-fix`, run in a scratch `git clone -s` checked out at `72b4efbd…` with the amended three files copied in | **1** | `tracked-production-vs-baseline: MATCH`, `PASS=6 FAIL=4 NOT-TESTABLE=2`, stderr empty. Versus `baseline-output.txt` only line 48 (the test-08 text) differs; the three evidence rows are identical. `baseline-output.txt` itself is unchanged. |
| 5 | as 3, but with cwd in `<scratch>/sdk8/cwd` below a `global.json` pinning `8.0.133` (`rollForward: disable`), script invoked by absolute path, `dotnet` wrapped by a logging shim | **0** | `PASS=10 FAIL=0 NOT-TESTABLE=2`; output byte-identical to 3 |

Six real production `RackEditing.SnapFar` cases: PASS in runs 3, 4 and 5 (six `SOURCE-STATIC PASS SnapFar …` rows each), compiled from the real `RackEditing.cs`/`RackGeometry.cs`; no mock. The generated project lives in the OS temp directory, outside the repo.

### .NET SDK evidence (observed, not implied)

- Same cwd as run 5 (`<scratch>/sdk8/cwd`): `dotnet --version` → **8.0.133**; `dotnet --info` → SDK 8.0.133, commit `62e6bab6ed`, workload `8.0.100-manifests.a1f9a504`, base path `/usr/lib/dotnet/sdk/8.0.133/`.
- The shim log from run 5 records the checker's own `dotnet run --project <tmp>/check.csproj` call executing with cwd `…/sdk8/cwd` and `dotnet --version => 8.0.133`.
- From the repo worktree the default is **10.0.114**; installed SDKs are 8.0.133 and 10.0.114. The six-case output is identical under both. This corrects #301's attribution (its `TMPDIR` pin did not select the SDK); #301's raw logs were not modified.

## Final state

Branch `verify/scenario-one-feedback-cta-anchor-20261106`; start `f294fe07…`; code commit `040e496a…`; tracked and untracked tree clean after commit; only the five allowed paths differ from the start HEAD.

## NOT RUN

Unity (Editor, tests, compile); rendering and screenshots; native pointer/keyboard input; Help-close timing; save/load, Legacy40 and Reference scenarios; Windows/Player build; any integration with `main` or the combined #296/#303 candidate; #296/#303 feature worktree; ecological or forestry claims (none made).

## Requested next step

DeepSeek to open a separate docs-only independent re-review lane for this exact delta (`f294fe07..040e496a` plus this handoff), then Manager #200 to rule APPROVED / NEEDS CHANGES / REJECTED. Re-run everything above on the exact combined candidate before any transfer.
