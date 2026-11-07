# #309 — CTA verifier RR-1 to RR-5 third-pass repair (worker handoff)

**Status: HANDED-OFF for a new independent re-review (#314). NOT approved.** This is worker evidence for source/offline verification tooling only. It is **not** an independent review, **not** Manager #200 acceptance, and not Unity, render, input or runtime evidence. #299 stays NEEDS CHANGES until the #314 review and a Manager verdict. #287 Unity is not released by this result.

**Worker result.** All 81 broken-source mutations from the #305 corpus are now rejected. That covers the 50 true false greens and the 2 N-1 cases (X-F1-31, X-F1-32), which the Manager's N-1 ruling now requires to be rejected. Each is rejected for a stated cause. The 21 re-derived R-IDs, the original 20 and the #304 author's 35 further mutations stay rejected. The 15 #305 benign edits and the author's 4 stay accepted. I added 62 new RR-1/RR-2/RR-3/N-4 regression mutations, all rejected, and 13 new benign controls, all accepted. The candidate gives **10 PASS / 0 FAIL / 2 NOT-TESTABLE, exit 0**. The historical baseline gives **6 PASS / 4 FAIL / 2 NOT-TESTABLE, exit 1**, `baseline-output.txt` is byte-preserved, and the six real `RackEditing.SnapFar` cases pass on a traced, cwd-pinned .NET SDK **8.0.133**. **No case is BLOCKED.** The coverage is bounded, not universal. The known limits are listed in [Limits](#limits-not-covered-and-what-remains-unproved), each with an accepted probe, as the contract requires.

## Task, authority and provenance

| Item | Value |
|---|---|
| Claim | [#309](https://github.com/jermur788/CCF/issues/309). Ruling: `MANAGER-D072-STREAM-A-B-REVIEW-RULING-20261106.md` (Stream B). Mimo #313 overnight child order #3. |
| Worker | T3 delegate worker, provider **claudeAgent (Claude A)**, model **claude-opus-5-5**, effort **XHIGH** (set by Mimo orchestrator). A fresh session: not the #304 author (Claude Sonnet 5.5, low effort) and not the #305 reviewer session. |
| Inputs read first | #309 body, the Manager ruling, the #305 report (`docs/baseline-cta-checker-f1-f3-re-review-20261106-v1` @ `94168e3c`) and its scripts/evidence, read-only, under `/media/jer/Files/CCF/Verification/CTA-F1F3-Review-304-20261106/` |
| Not done | No production C#/UI/scene/asset edit, no Unity, no `unity-lock`, no build, no PR #300 merge, no `active-tasks.md`/Manager-doc/ledger edit, no stash/reset/clean, no other lane touched. Old handoffs and `baseline-output.txt` unchanged. |

## Preflight (recorded before any edit; [RUNNING] comment posted 2026-11-07)

| Check | Observed |
|---|---|
| Lane / branch | `/media/jer/Files/CCF/Operations/CCF-FeedbackCTARepair-20261106`, `verify/scenario-one-feedback-cta-anchor-20261106` |
| Start HEAD | `1585ca20df2b921d10e79f9aac89f03202d9258e` = `origin/verify/scenario-one-feedback-cta-anchor-20261106` after `git fetch` (0 ahead / 0 behind) |
| Clean | `git status --porcelain --untracked-files=all --ignored` empty; no assume-unchanged/skip-worktree flags |
| Owners | no other [RUNNING] on #309; #287 handed off 01:51Z, #317 handed off 02:08Z; no `dotnet`/Unity process at start |
| Storage | Files `/dev/sdb4` ext4 rw, 54 GiB free. Evidence: `/media/jer/Files/CCF/Verification/CTA-RR1RR4-Repair-309-20261107/` (Files, not a Git lane) |

## Exact diff

Code commit **`84d0ba92c763f4b1e2542f4e1b31240882960187`** (parent `1585ca20`), then this handoff as a docs-only commit. The final pushed HEAD is in the #309 [HANDED-OFF] comment, because a file cannot contain its own hash.

| File | + | − | Change |
|---|---|---|---|
| `Tools/Verification/OvernightPlaytestFeedback/objective_cta_source_check.py` | 1120 | 14 | New #309 section (lexer, pins, header grammar, inventories, `collect_companions`); small RR-2/RR-5 edits to existing predicates (below) |
| `Tools/Verification/OvernightPlaytestFeedback/test_objective_cta_source_check.py` | 593 | 20 | Companions passed; #305 corpus and #309 controls added; cause checks; original 20 + #304 55 + 4 benign kept byte-identical |
| `Tools/Verification/OvernightPlaytestFeedback/check_baseline.py` | 4 | 4 | Test-08 only: import of `collect_companions`, the call passes it, and the PASS note states the new rule |
| `Tools/Verification/OvernightPlaytestFeedback/README.md` | 10 | 7 | Check-08 scope, known limits and control counts rewritten to match what is checked |
| this handoff | new | | |

`git diff --check` exit 0. Changed paths against `1585ca20`: exactly the four files above, plus this handoff. `Assets/`, `Packages/` and `ProjectSettings/` have 0 changed files. `baseline-output.txt` blob `db6a8b850dce7efadaac6fe1b87b1dbba469428f` (sha256 `513cddd9…c1`) is unchanged. `BASE` `72b4efbd…` is unchanged. Checks 01–07, 09 and 06a/b/c are unchanged. The `check_baseline.py` change is justified because test 08 must now receive the companion sources (N-3); without them the scanner fails closed by design. The PASS note had to stop claiming the superseded "every static source … in the demotion set" rule (RR-4).

**Edits to pre-existing predicates (all strengthen or only reword).**
- `_assignments(..., qualified=True)` also counts `this.x =`. It is used only at the two root-wide write checks: guidance state and `helpClosedFrame`.
- The `GuidanceButtonNames` reference count now includes qualified references.
- RR-5: when the allowlist itself is invalid, the header audit skips only the membership predicate. The allowlist's own error is then raised, so it is never replaced by a membership message. Rejection is unchanged in every case.
- Two messages reworded: the allowlist label, and "checked method or declaration".
- `_verify` first runs the N-4 lexical-soundness check.

No predicate was removed or weakened. The historical rejection reason is unchanged.

## Implementation notes

The design adds three bounded techniques, all lexical and all failing closed with a `path:line` message and the RR/N tag. It is still not a C# parser. The #305 scratch prototype was not reused: I wrote and tested this code independently, using only the reviewer's mutation data.

**N-4: a lexer it can trust.** `_lexed` is a small precise lexer for comments, preprocessor lines, character literals and regular, verbatim and interpolated strings. It recurses into interpolation holes, whose code counts as code; format clauses count as text. It fails closed on raw strings, `\u` identifiers (any backslash in code), malformed character literals, line breaks in regular strings, unbalanced interpolation, and any literal or comment that runs across a preprocessor line. The last rule keeps inactive `#if` text from desynchronising it (N4-03, N4-04). It lexes all 198 `.cs` files under `Assets/` without error. In `LearningObjectivesView.cs` and `ScenarioOneUiRoot.cs`, which the older regex-based per-statement checks still read, `_lexically_sound` also requires both lexers to agree on every literal/comment boundary. X-F1-29 and X-F1-30 now fail there, so the older checks can no longer be fed a desynchronised view. The live `LearningObjectivesView.Sequence.cs` partial has a nested-literal interpolation, `$"Beech: {(… ? "present" : …)}"`. It is lexed precisely, and only the new guards read it.

**RR-1 and N-1: exactly one static primary.**
- *Header grammar* (`_guard_header`). The constructor is split into top-level statements, and each header statement must be one of the reviewed shapes. The three reviewed buttons are bound by caption, click (`() => ui.Method()`), enabled argument, classes and literal name: map → `ShowMap`, no classes, `objective-primary-cta`; Work Plan → `ShowWorkPlan`, no classes, `objective-work-plan`; Back to forest → `CloseAll`, `true`, exactly one `"btn-primary"`, `objective-back-to-forest`, which must be in the parsed `GuidanceButtonNames`. Extra header buttons are allowed only if non-primary, added once, with a new caption and a name outside the allowlist; B04 and B-X-07 still pass. Every occurrence of `header`, `buttons`, `modal`, `Root`, the constructor's `ui` and each button local, found in hole-visible masked code, must sit at a token position recorded from a matched shape. That closes `+=`, cast, alias, `nameof`, lambda, hole, `if`/`goto` wrappers, indexers, `SetEnabled`/`Clear` and later restyling.
- *Class-wide inventories* (`_guard_view_class`), over `LearningObjectivesView.cs` and every partial. Exactly one `btn-primary` literal (any spelling, interpolated text included), no class-list API identifier, no element-tree traversal or query API, `Root` only in its declaration and the constructor, one constructor, no `Conditional*` or `using static`. Each Objectives header name and caption appears once. Every `UiKit` use must be `UiKit.Member`; class arguments must be plain literals, and `btn-primary` is allowed only at the Back to forest call. Unknown members, aliases, method groups and named class arguments fail. This closes helpers, method groups, class constants (X-F1-01, 02, 28, RR1-03 to RR1-07), the second static primaries of N-1 (X-F1-31, X-F1-32, RR1-08) and partial-file routes (X-F1-05, RR1-22).
- *Allowlist*: a qualified reference, `Array.Clear` or an element write fails, through the strengthened count and the root occurrence inventory (X-F1-27, RR1-11, RR1-12).

**RR-2 and RR-3: pinned reviewed root code** (`_guard_root`).
- `RefreshNextActionGuidance`, `ClearNextAction`, `ApplyNextActionGuidance`, `GuidanceVisible`, `CloseHelp`, `SetScreen` and `ShowMap` are compared token by token with the reviewed bodies stored in the scanner. Comments and layout are ignored, the operators are munched as C# lexes them, and a literal is one token. Each must carry no attribute. The stored text equals `1585ca20`, comments removed. This closes tuple writes, count-compensated retargets, the dead first guard, `if (false)` styling, `goto`/`throw`, dead lambdas, late removal and demotion undone after the loop (X-F2-08 to 17, 19, 20, 24 to 26, 29 to 32, 35).
- `LateUpdate` must begin with `ApplyNextActionGuidance();` (X-F2-22).
- *Occurrence inventory.* Outside the pinned bodies, `helpClosedFrame`, `nextAction`, `guidanceCandidate`, `guidanceScope`, `guidancePrimaries`, `GuidanceButtonNames`, `ApplyNextActionGuidance`, `ClearNextAction`, `GuidanceVisible` and `learning.Root`/`Learning.Root` may appear only inside 16 reviewed declaration/statement snippets, each present exactly once. Qualification, tuple, `ref`/`out`, lambda, property wrapper, local-function shadow or an extra use anywhere in the root therefore fails (X-F2-18, 23, 27, 28; RR2-01 to RR2-06, RR2-13, RR2-14). The `learning` field may not be reassigned, aliased or passed on. `help`, `manager`, `learning`, `map`, `workPlan`, `review`, `rackLayout` and `screen` keep their reviewed write counts (RR2-10 to RR2-12, RR2-15).
- Outside the pins, a class-list call needs a plain non-primary literal (`ClearClassList` fails). All 5 root `btn-primary` literals must lie inside the pins. Objectives header names may appear only in the allowlist and pinned routing. `Conditional*` and `using static` are banned, which covers X-F2-21 and alias forms such as RR1-27.
- *Reachable caller* (`_guard_refresh_caller`, X-F2-33, X-F2-34, RR3-04 to RR3-09). `ui.RefreshNextActionGuidance();` appears once in the whole view class, as a top-level statement of `Refresh(bool)` after the single pinned early return. That method has no other `return`, no `throw`, `goto` or `yield`, no attribute and no `#if`.

**N-2 and N-3: scope.**
- `collect_companions(repo)` reads every `.cs` under `Assets/` that mentions `LearningObjectivesView`, `ScenarioOneUiRoot`, `UiKit`, an Objectives header name, or a `\u`/`\x` escape: 29 files at this HEAD. `verify_objective_cta(view, root, companions)` without companions now fails closed.
- `ScenarioOneUiRoot` and `UiKit` must each be one non-partial declaration in their reviewed file (RR1-21). Every `LearningObjectivesView` partial gets the class-wide rules.
- No other companion may name an Objectives header button or reach `ui.Learning.Root` (RR1-17, RR1-18).
- `UiKit.Button`, `Box`, `Row`, `Text`, `Add` and `MakeModal`, every member the Objectives constructor calls, are pinned and declared once (X-F1-06, RR1-19, RR1-20, RR1-28). `UiKit.cs` contains no `btn-primary`.

**RR-5.** Allowlist errors are no longer masked. X-F1-21 now reports "GuidanceButtonNames contains a duplicate name" and the author's `allowlist-duplicate-name` reports the same. X-F1-23 reports "conditional compilation (#if) … declaration". X-F2-43 reports "GuidanceButtonNames allowlist: expected one structural match, found 2". The suite checks these causes.

**RR-4.** The README's check-08 section now lists exactly the guards above and the known limits with their probe IDs, and no longer claims "every static source … mechanically enumerated" in the old sense. This handoff is new. The #301/#304 handoffs are untouched.

**Test-file mechanics.** The original 20 / F1 14 / F2-F3 21 / benign 4 block is `1585ca20` lines 28–198, byte-identical (sha256 `6119b5fa…`, found once). The #305 corpus is `review_mutations.py` lines 33–295, byte-identical (sha256 `9a7a8fd2…`, found once). Every #305 and #309 mutation must be rejected **for its stated cause**: a wrong-cause rejection counts as a failure (`wrong_cause`), never as a pass. There is no skip, xfail or rebaseline; the word "skip" appears only inside mutation C# text, as `goto skip` and `skipped`. Exit 0 requires every mutation rejected, every benign edit accepted and the historical source rejected.

**Maintenance cost (explicit).** Any deliberate edit to a pinned method, a snippet, the header shape or a pinned UiKit member fails check 08 until the scanner's reviewed text is updated in the same reviewed change. That is the intended fail-closed trade-off. Informational only: the scanner accepts the view, root and companions in memory at the in-flight tips `9789369d` (#308 feature branch) and `69b41605` (#296). This does not replace the required rerun on the exact integrated candidate.

## Results

All gates ran on code commit `84d0ba92` with Python 3.12.3. Raw files are under `$E=/media/jer/Files/CCF/Verification/CTA-RR1RR4-Repair-309-20261107/final/`. All Python ran with `PYTHONDONTWRITEBYTECODE=1 PYTHONPYCACHEPREFIX=$E/pycache` and the lane stayed clean throughout.

| Gate | Result | Evidence |
|---|---|---|
| `python3 -m py_compile` on the three scripts | exit 0, no output | `01-py_compile.out` |
| Suite `python3 -B …/test_objective_cta_source_check.py` | **exit 0**. Live source accepted (10 evidence rows). original 20/20, F1 14/14, F2-F3 21/21, #305 R-IDs 21/21, #305 X-F1 35/35, #305 X-F2 46/46, #309 RR-1 28/28, RR-2 15/15, RR-3 14/14, N-4 5/5. Benign 32/32 accepted. Historical baseline rejected ("unnamed primary", 3 rows). `rejected=219/219 false_greens=0 wrong_cause=0`. stderr empty. | `02-suite.*` |
| #305 corpus vs the reviewed scanner `1585ca20` and the candidate | Reproduction exact: at `1585ca20`, X-F1 17 and X-F2 35 accepted (**52**), every other result matches the #305 `results.json`. Candidate: **102/102 rejected** (21 R-IDs, 81 X-cases), 15/15 benign accepted, every `intended` cause matched | `03-corpus.stdout`, `$E/mutations/corpus/results.json` |
| Extra probes | 16/16 extra attacks rejected, 6/6 limitation probes accepted as documented, 4/4 over-rejections rejected as documented | `04-probes.stdout`, `$E/mutations/probes/results.json` |
| Candidate `check_baseline.py --rotation-state post-fix --instruction-state post-fix`, cwd pinned to SDK 8.0.133 | **`PASS=10 FAIL=0 NOT-TESTABLE=2`, exit 0**, stderr empty, six SnapFar PASS rows. sha256 `1facb1ff4202c072fa2df0b795b53a0457a5f3aef58cf0e0eafcbba3b2e3b011` | `05-candidate.*` |
| Same, traced | Byte-identical stdout, exit 0 | `06-*` |
| Same from a cwd without `global.json` (control, SDK 10.0.114) | Byte-identical stdout, exit 0 | `07-*` |
| Historical `72b4efbd` with the three amended scripts copied in, untracked | **`PASS=6 FAIL=4 NOT-TESTABLE=2`, exit 1**, `tracked-production-vs-baseline: MATCH`, stderr empty. Differs from `baseline-output.txt` at line 48 only (test-08 text, as before). **Byte-identical to #305's regenerated output** (`cmp` exit 0, sha256 `ba4f7104…`) | `08-*` |
| `baseline-output.txt` | Blob `db6a8b85…` at `1585ca20` and in the tree; sha256 `513cddd9…c1`; never regenerated in place | `09-protected-audit.txt` |
| Roslyn C# 9 compile of every control and probe | Baseline 189 files, 0 errors. 248 of 251 control directories and all 26 probes compile with 0 errors; the 3 exceptions are listed below | `12-roslyn-*` |

The historical clone is a `git clone --shared --no-checkout` of the bare store with sparse checkout `/Assets/**/*.cs` and the checker folder, detached at `72b4efbd`. It is in the evidence folder, and nothing was written to the bare store. `check_baseline.py` reads only `.cs` files and Git, and the production comparison printed MATCH.

The 1585ca20 column of the matrix below shows that **55 of my 62 new #309 mutations were also accepted by the reviewed `1585ca20` scanner** (RR-1 26/28, RR-2 11/15, RR-3 13/14, N-4 5/5), so they are new coverage, not duplicates. The other 7 are hardening controls that the older checks already reject.

## SDK 8.0.133 evidence (cwd-pinned, traced)

The checker's `dotnet run` inherits the Python process's working directory, and the SDK is selected from that directory's `global.json`, not from `TMPDIR` or the script location. Runs 05 and 06 were started from `$E/sdk8/cwd/deeper`, beneath `$E/sdk8/global.json` = `{ "sdk": { "version": "8.0.133", "rollForward": "disable" } }`. `TMPDIR` was `$E/tmp`, which has no `global.json`.

- From that cwd, `dotnet --version` gives **8.0.133**; from the control cwd it gives **10.0.114**. Both SDKs are installed (`dotnet --list-sdks`).
- **hostfxr trace of run 06** (`COREHOST_TRACE=1`, verbosity 4): it probes `…/sdk8/cwd/deeper/global.json`, then `…/sdk8/cwd/global.json`, then reports `Found global.json [$E/sdk8/global.json]`, `Resolving SDKs with version = '8.0.133', rollForward = 'disable'` and `Using .NET SDK dll=[/usr/lib/dotnet/sdk/8.0.133/dotnet.dll]`.
- **strace of the checker's whole process tree** (`strace -f -e trace=execve,openat`). The checker itself executes `/usr/bin/dotnet run --project <tmp>/check.csproj --verbosity quiet`. The C# compiler runs inside the traced tree as `/usr/lib/dotnet/dotnet exec /usr/lib/dotnet/sdk/8.0.133/Roslyn/bincore/csc.dll …`, and it opens `sdk/8.0.133/Roslyn/bincore/Microsoft.CodeAnalysis.CSharp.dll`. In total there are **281 successful opens under `sdk/8.0.133` and 0 under `sdk/10.0.114`**. The trace used `UseSharedCompilation=false` and `MSBUILDDISABLENODEREUSE=1`, which keep the compiler and MSBuild nodes inside the tree; this closes the #305 limitation where the compile ran in a server outside the trace. Run 05, without these settings, produced byte-identical output.
- **The control shows that the trace discriminates.** Run 07, from a cwd with no `global.json`, resolves `version = 'latest'` → `sdk/10.0.114/dotnet.dll`, runs `sdk/10.0.114/Roslyn/bincore/csc`, and has 504 opens under `sdk/10.0.114` and 0 under `sdk/8.0.133`.
- All six cases are real production code with no mock: the generated project compiles exactly `RackGeometry.cs` and `RackEditing.cs` plus the six-case driver.

```text
SOURCE-STATIC PASS SnapFar near-angle enabled=True input=(20.000,0.700) far=(20.012,0.000) angle=0.000 angleSnapped=True boundarySnapped=False
SOURCE-STATIC PASS SnapFar near-angle-disabled enabled=False input=(20.000,0.700) far=(20.000,0.700) angle=2.005 angleSnapped=False boundarySnapped=False
SOURCE-STATIC PASS SnapFar outside-angle-window enabled=True input=(20.000,3.000) far=(20.000,3.000) angle=8.531 angleSnapped=False boundarySnapped=False
SOURCE-STATIC PASS SnapFar near-boundary enabled=True input=(39.000,0.000) far=(40.000,0.000) angle=0.000 angleSnapped=False boundarySnapped=True
SOURCE-STATIC PASS SnapFar near-boundary-disabled enabled=False input=(39.000,0.000) far=(39.000,0.000) angle=0.000 angleSnapped=False boundarySnapped=False
SOURCE-STATIC PASS SnapFar outside-boundary-window enabled=True input=(37.000,0.000) far=(37.000,0.000) angle=0.000 angleSnapped=False boundarySnapped=False
```

## Roslyn reality check

The tool is my copy of the #305 harness, `$E/sdk8/roslyn-tool-309/`, built with SDK 8.0.133. Its one change from the reviewer's `Program.cs` is that a mutant may add a new file, which RR1-22 needs. It compiles the Assembly-CSharp source set of the lane (`Assets/**/*.cs` minus `Editor/`) with Roslyn at C# 9 against the same 320 Unity 6000.6 and project DLLs as #305, which are read only. The defines are Player-like, with no `UNITY_EDITOR`. The unmodified tree has 189 files and 0 errors. This is a compiler check, not a Unity compile.

| Set | Directories | 0 errors | Not compiling |
|---|---|---|---|
| Author original 20 + F1 14 + F2/F3 21 + benign 4 | 59 | 58 | `map-not-added-to-row`: CS0841, `work` used before its declaration. This original control is pre-existing and immutable, preserved unchanged, and still rejected |
| #305 R-IDs 21, X-F1 35, X-F2 46, benign 13 | 115 | 113 | X-F1-09 (CS8773, raw strings are not C# 9) and X-F2-44 (CS0103, `#if`'d out), the same two #305 recorded; both still rejected |
| #309 RR-1 28, RR-2 15, RR-3 14, N-4 5, benign 11 | 73 | **73** | none |
| Whole-file transforms B-X-14, B-X-15, NB-10, NB-11 | 4 | 4 | none |
| Evidence probes (16 attacks, 6 limits, 4 over-rejections) | 26 | **26** | none |

So every #309 mutation is a compilable C# 9 defect under the Player-like defines, every limitation probe is a compilable defect the scanner accepts, and every benign control compiles. During development two of my controls did not compile (RR1-03 used an unqualified `Button` in a partial; RR3-05 reused the local name `later`). I fixed both before the code commit, and the final run above is on the committed text.

## Limits (not covered) and what remains unproved

These are stated limits, not passes. Each L-probe is a compilable defective edit that the scanner **accepts**; it is recorded in `04-probes.stdout` and is not counted as a test.

| Probe | Not covered | Why it is out of bounded lexical scope |
|---|---|---|
| L-01 | A class outside the companion set (it never mentions the hint names) disables a header button by a constructed name from `FindAnyObjectByType<UIDocument>()` | Classes other than the view, its partials, the root and UiKit are not analysed beyond the companion name/`Learning.Root` inventory; string construction is unbounded |
| L-06 | A companion aliases `ui.Learning` before reaching `.Root` | Aliasing in other classes is not tracked |
| L-02 | Unpinned root code disables every button of its own document (no class change) | The root legitimately queries its own document; non-class mutations outside the pins are not inventoried |
| L-03, L-07 | An infinite loop or a throwing expression before the Objectives caller | Non-termination and exceptions are not analysed (no control-flow interpreter) |
| L-04 | Reflection writes guarded state by a string name | Strings are not resolved to members |
| — | USS/style sheets and other style sources; runtime layout, focus, input and timing | Not read; not runtime evidence |
| — | UiKit members other than Button/Box/Row/Text/Add/MakeModal | Not pinned (not called by the Objectives constructor) |
| — | Same-named types in other namespaces, extension-method tricks and other novel C# shapes | Not claimed; fail closed only where a guard sees them |

**Documented over-rejections**, where benign edits fail closed: O-01, a literal nested in an interpolation hole in `LearningObjectivesView.cs` or `ScenarioOneUiRoot.cs` (lexer agreement); O-02, apostrophes in two preprocessor lines of those files; O-03, any change inside a pinned method, including spacing in an interpolation hole; O-04, `ui?.RefreshNextActionGuidance()`. Renaming `header`/`buttons` or reformatting the pinned view `Refresh` guard also fails. Each needs a deliberate checker update.

**Not proved.** That the stored pinned text is the *right* behaviour: it is the reviewed `1585ca20` source, accepted as-is. That the guard set is complete: the guards were written after seeing the #305 corpus, the #309 attacks are my own, and nobody independent has attacked this version yet. That the lexer matches Roslyn on every input: it is verified only on the 198 live `Assets/` files, the controls and the compile check above. Unity Editor compile, native input, rendering, Help-close timing, saves, Legacy40/Reference, Windows/Player, and the exact integrated candidate: **NOT RUN**.

## Commands

`E=/media/jer/Files/CCF/Verification/CTA-RR1RR4-Repair-309-20261107`, lane `L=/media/jer/Files/CCF/Operations/CCF-FeedbackCTARepair-20261106`. The scripts below are in `$E/scripts/` and reproducible; `SHA256SUMS.txt` in `$E` covers the scripts and logs.

| # | Command (cwd) | What |
|---|---|---|
| 1 | `bash $E/scripts/final_gates.sh "<label>"` | Steps 01–10 above: py_compile, suite, corpus, probes, pinned, traced and control candidate runs, historical run, audit, SDK summary |
| 2 | `python3 -I -B scripts/run_corpus.py $L scripts/reviewer-305-review_mutations.py.copy scripts/scanner_1585ca20.py <#305 results.json> <out>` (`$E`) | #305 corpus against `1585ca20` and the candidate. The corpus block is sha-checked, and the reviewer's script copy has sha256 `5e463871…` |
| 3 | `python3 -I -B scripts/probe_extra.py $L <out>` (`$E`) | Extra attacks, limitation probes and over-rejections |
| 4 | `python3 -B scripts/export_mutants.py $L scripts/test_1585ca20.py <out>`, then `dotnet $E/sdk8/roslyn-tool-309/bin/RoslynCheck.dll $L <out> refs.txt "<defines>" <log>` (`$E/sdk8/cwd/deeper`) | C# 9 compile of every control and probe |
| 5 | `python3 -B scripts/build_matrix.py $L $E` (`$E`) | The matrix below, from the final logs |

## Final state

The lane is clean after the commits. The candidate branch was published fast-forward only from `1585ca20`, with no force-push or rebase. Evidence, clones and the Roslyn copy remain under `$E`. The `old-baseline-clone` is a small sparse shared clone and can be deleted after the Manager rules. Build servers that my own runs started were stopped. No other lane's process, file or lock was touched.

## Full per-case matrix

"1585ca20 scanner" is the #305-reviewed scanner, called with the 2-argument API on the same in-memory mutated text. "#309 scanner" is the committed result from `02-suite.stdout`. "C# 9 compile errors" comes from `12-roslyn-*`. Reasons are abridged; full text is in `02-suite.stdout`. There is one suite process with exit 0; per-case results are in-process verdicts.

For the four whole-file benign transforms, the 1585ca20 column is not a fresh 2-argument run. For B-X-14 and B-X-15 it is the corpus run in `03-corpus.stdout`. For NB-10 and NB-11 it is trivially true, because the old scanner never reads the partials or UiKit.

### Original 20 (unchanged)

| ID | Mutation | 1585ca20 scanner | #309 scanner | C# 9 compile errors | #309 rejection reason (abridged) |
|---|---|---|---|---|---|
| map-button-name | map-button-name | rejected | **REJECTED** | 0 | Objectives constructor/map identity: expected one structural match, found 0 |
| map-click-destination | map-click-destination | rejected | **REJECTED** | 0 | named map button with exact caption and ShowMap click: expected one structural match, found 0 |
| map-caption | map-caption | rejected | **REJECTED** | 0 | named map button with exact caption and ShowMap click: expected one structural match, found 0 |
| map-not-added-to-row | map-not-added-to-row | rejected | **REJECTED** | 1 (CS0841) | Objectives constructor/row binding: expected one structural match, found 0 |
| duplicate-map-identity | duplicate-map-identity | rejected | **REJECTED** | 0 | duplicate element name in the Objectives constructor |
| target-renamed | target-renamed | rejected | **REJECTED** | 0 | RefreshNextActionGuidance/no rack plan yet: expected one structural match, found 0 |
| objective-condition-removed | objective-condition-removed | rejected | **REJECTED** | 0 | RefreshNextActionGuidance/no rack plan yet: expected one structural match, found 0 |
| rack-v2-gate-removed | rack-v2-gate-removed | rejected | **REJECTED** | 0 | RefreshNextActionGuidance/rack teaching gate: expected one structural match, found 0 |
| help-guard-removed | help-guard-removed | rejected | **REJECTED** | 0 | ApplyNextActionGuidance/preview/menus/help suspension: expected one structural match, found 0 |
| reference-preview-guard-removed | reference-preview-guard-removed | rejected | **REJECTED** | 0 | ApplyNextActionGuidance/preview/menus/help suspension: expected one structural match, found 0 |
| candidate-enabled-check-removed | candidate-enabled-check-removed | rejected | **REJECTED** | 0 | ApplyNextActionGuidance/candidate scoped visibility: expected one structural match, found 0 |
| scope-visibility-check-removed | scope-visibility-check-removed | rejected | **REJECTED** | 0 | ApplyNextActionGuidance/candidate scoped visibility: expected one structural match, found 0 |
| hidden-visibility-check-removed | hidden-visibility-check-removed | rejected | **REJECTED** | 0 | visibility and display suspension: expected one structural match, found 0 |
| allowlist-primary-dedup-removed | allowlist-primary-dedup-removed | rejected | **REJECTED** | 0 | RefreshNextActionGuidance/bounded class demotion: expected one structural match, found 0 |
| unrelated-global-demotion | unrelated-global-demotion | rejected | **REJECTED** | 0 | RefreshNextActionGuidance/bounded class demotion: expected one structural match, found 0 |
| primary-restore-removed | primary-restore-removed | rejected | **REJECTED** | 0 | RefreshNextActionGuidance/restore old primary classes: expected one structural match, found 0 |
| primary-class-not-added | primary-class-not-added | rejected | **REJECTED** | 0 | ApplyNextActionGuidance/apply primary: expected one structural match, found 0 |
| next-action-class-not-added | next-action-class-not-added | rejected | **REJECTED** | 0 | ApplyNextActionGuidance/apply next-action: expected one structural match, found 0 |
| eligibility-return-removed | eligibility-return-removed | rejected | **REJECTED** | 0 | ApplyNextActionGuidance/no styling if ineligible: expected one structural match, found 0 |
| target-not-bound-to-scoped-button | target-not-bound-to-scoped-button | rejected | **REJECTED** | 0 | RefreshNextActionGuidance/candidate resolved in its scope: expected one structural match, found 0 |

### #304 author F1 (14, unchanged)

| ID | Mutation | 1585ca20 scanner | #309 scanner | C# 9 compile errors | #309 rejection reason (abridged) |
|---|---|---|---|---|---|
| R01-unconditional-map-primary | R01-unconditional-map-primary | rejected | **REJECTED** | 0 | map CTA must not be unconditionally primary (AddToClassList) |
| R01b-map-primary-via-enable-class | R01b-map-primary-via-enable-class | rejected | **REJECTED** | 0 | map CTA must not be unconditionally primary (EnableInClassList) |
| R01c-map-primary-verbatim-spelling | R01c-map-primary-verbatim-spelling | rejected | **REJECTED** | 0 | map CTA must not be unconditionally primary (AddToClassList) |
| R01d-map-class-non-literal | R01d-map-class-non-literal | rejected | **REJECTED** | 0 | AddToClassList in the header uses a non-literal class name (ambiguous) |
| R03-back-to-forest-removed-from-allowlist | R03-back-to-forest-removed-from-allowlist | rejected | **REJECTED** | 0 | static primary button 'objective-back-to-forest' is not in the GuidanceButtonNames scoped demotion set |
| R04-back-to-forest-renamed-out-of-allowlist | R04-back-to-forest-renamed-out-of-allowlist | rejected | **REJECTED** | 0 | static primary button 'objective-forest' is not in the GuidanceButtonNames scoped demotion set |
| R05-new-unnamed-closeall-primary | R05-new-unnamed-closeall-primary | rejected | **REJECTED** | 0 | static primary 'close' must have exactly one literal name; found 0 |
| R05b-new-closeall-primary-named-outside-allowlist | R05b-new-closeall-primary-named-outside-allowlist | rejected | **REJECTED** | 0 | static primary button 'objective-close' is not in the GuidanceButtonNames scoped demotion set |
| R05c-inline-unnamed-primary | R05c-inline-unnamed-primary | rejected | **REJECTED** | 0 | static btn-primary header button is not assigned to a named local (unnamed primary) |
| R05d-new-closeall-primary-reusing-allowlisted-name | R05d-new-closeall-primary-reusing-allowlisted-name | rejected | **REJECTED** | 0 | duplicate element name in the Objectives constructor |
| R05e-named-argument-class | R05e-named-argument-class | rejected | **REJECTED** | 0 | named UiKit.Button argument in the header is an unsupported shape |
| R05f-primary-class-in-unsupported-shape | R05f-primary-class-in-unsupported-shape | rejected | **REJECTED** | 0 | btn-primary literal 'btn-primary' in the header is not in a supported UiKit.Button/AddToClassList shape |
| R05g-forest-primary-reassigned | R05g-forest-primary-reassigned | rejected | **REJECTED** | 0 | static primary 'forest' must be declared once and never reassigned in the header |
| allowlist-duplicate-name | allowlist-duplicate-name | rejected | **REJECTED** | 0 | GuidanceButtonNames contains a duplicate name |

### #304 author F2/F3 (21, unchanged)

| ID | Mutation | 1585ca20 scanner | #309 scanner | C# 9 compile errors | #309 rejection reason (abridged) |
|---|---|---|---|---|---|
| R07-verbatim-duplicate-cta-name | R07-verbatim-duplicate-cta-name | rejected | **REJECTED** | 0 | duplicate element name in the Objectives constructor |
| R07b-escaped-duplicate-cta-name | R07b-escaped-duplicate-cta-name | rejected | **REJECTED** | 0 | duplicate element name in the Objectives constructor |
| R07c-concatenated-name | R07c-concatenated-name | rejected | **REJECTED** | 0 | an element name in the Objectives constructor is not a single plain literal |
| R08-later-map-name-overwrite | R08-later-map-name-overwrite | rejected | **REJECTED** | 0 | Objective CTA name must be unique and bound to the ShowMap button; found map names ['objective-primary-cta', 'objective-map'] |
| R09-anchor-spoofed-in-string-literal | R09-anchor-spoofed-in-string-literal | rejected | **REJECTED** | 0 | ApplyNextActionGuidance/no styling if ineligible: expected one structural match, found 0 |
| R09b-view-anchor-spoofed-in-string-literal | R09b-view-anchor-spoofed-in-string-literal | rejected | **REJECTED** | 0 | Objectives constructor/row binding: expected one structural match, found 0 |
| R11-target-override-to-closeall | R11-target-override-to-closeall | rejected | **REJECTED** | 0 | target is reassigned after the routing chain |
| R12-candidate-nulled-in-apply | R12-candidate-nulled-in-apply | rejected | **REJECTED** | 0 | ApplyNextActionGuidance writes 'candidate' more than once (override) |
| R13-unreachable-return-in-refresh | R13-unreachable-return-in-refresh | rejected | **REJECTED** | 0 | RefreshNextActionGuidance has an unexpected early return |
| R14-primary-hidden-behind-unity-editor | R14-primary-hidden-behind-unity-editor | rejected | **REJECTED** | 0 | \bprivate\s+void\s+ApplyNextActionGuidance\s*\(\s*\): conditional compilation (#if) in a checked method or declaration is unsupported |
| R14b-cta-name-hidden-behind-conditional | R14b-cta-name-hidden-behind-conditional | rejected | **REJECTED** | 0 | \bpublic\s+LearningObjectivesView\s*\(\s*ScenarioOneUiRoot\s+ui\s*\): conditional compilation (#if) in a checked method or declaration is unsupported |
| R15-map-disabled-after-binding | R15-map-disabled-after-binding | rejected | **REJECTED** | 0 | map CTA variable may only be declared, named and added to the header row; other uses are unsupported |
| R16b-extra-return-true-in-visibility | R16b-extra-return-true-in-visibility | rejected | **REJECTED** | 0 | GuidanceVisible has an unexpected return |
| R17b-candidate-disabled-in-apply | R17b-candidate-disabled-in-apply | rejected | **REJECTED** | 0 | ApplyNextActionGuidance uses an unexpected candidate member: SetEnabled |
| R20-help-close-frame-setter-removed | R20-help-close-frame-setter-removed | rejected | **REJECTED** | 0 | CloseHelp/help-close frame recorded: expected one structural match, found 0 |
| R20b-help-close-frame-conditional | R20b-help-close-frame-conditional | rejected | **REJECTED** | 0 | helpClosedFrame is set conditionally in CloseHelp |
| R20c-help-close-frame-reset | R20c-help-close-frame-reset | rejected | **REJECTED** | 0 | helpClosedFrame must be written only by its initialiser and CloseHelp |
| R22-candidate-nulled-in-clear | R22-candidate-nulled-in-clear | rejected | **REJECTED** | 0 | ClearNextAction may only drop the guide classes, badge and nextAction |
| R23-candidate-nulled-by-unrelated-handler | R23-candidate-nulled-by-unrelated-handler | rejected | **REJECTED** | 0 | 'guidanceCandidate' must be written only by RefreshNextActionGuidance/ApplyNextActionGuidance/ClearNextAction |
| R21-late-update-apply-removed | R21-late-update-apply-removed | rejected | **REJECTED** | 0 | LateUpdate applies cached guidance: expected one structural match, found 0 |
| R21b-late-update-apply-after-return | R21b-late-update-apply-after-return | rejected | **REJECTED** | 0 | LateUpdate may only apply guidance unconditionally before any branch or return |

### #305 reviewer: R-IDs re-derived from #302 (21)

| ID | Mutation | 1585ca20 scanner | #309 scanner | C# 9 compile errors | #309 rejection reason (abridged) |
|---|---|---|---|---|---|
| R01 | map statically primary | rejected | **REJECTED** | 0 | map CTA must not be unconditionally primary (AddToClassList) |
| R02 | inline primary on the map constructor | rejected | **REJECTED** | 0 | map CTA must not be unconditionally primary (UiKit.Button) |
| R03 | Back to forest dropped from the allowlist | rejected | **REJECTED** | 0 | static primary button 'objective-back-to-forest' is not in the GuidanceButtonNames scoped demotion set |
| R04 | Back to forest renamed out of the allowlist | rejected | **REJECTED** | 0 | static primary button 'objective-forest' is not in the GuidanceButtonNames scoped demotion set |
| R05 | new CloseAll primary | rejected | **REJECTED** | 0 | static primary 'close' must have exactly one literal name; found 0 |
| R06 | duplicate CTA name, no spaces | rejected | **REJECTED** | 0 | duplicate element name in the Objectives constructor |
| R07 | verbatim duplicate CTA name | rejected | **REJECTED** | 0 | duplicate element name in the Objectives constructor |
| R08 | later map rename | rejected | **REJECTED** | 0 | Objective CTA name must be unique and bound to the ShowMap button; found map names ['objective-primary-cta', 'objective-map'] |
| R09 | guard moved into a string | rejected | **REJECTED** | 0 | ApplyNextActionGuidance/no styling if ineligible: expected one structural match, found 0 |
| R10 | guard commented out | rejected | **REJECTED** | 0 | ApplyNextActionGuidance/no styling if ineligible: expected one structural match, found 0 |
| R11 | target override to CloseAll before binding | rejected | **REJECTED** | 0 | target is reassigned after the routing chain |
| R12 | candidate nulled in Apply | rejected | **REJECTED** | 0 | ApplyNextActionGuidance writes 'candidate' more than once (override) |
| R13 | return after tracking clear | rejected | **REJECTED** | 0 | RefreshNextActionGuidance has an unexpected early return |
| R14 | primary class behind #if UNITY_EDITOR | rejected | **REJECTED** | 0 | \bprivate\s+void\s+ApplyNextActionGuidance\s*\(\s*\): conditional compilation (#if) in a checked method or declaration is unsupported |
| R15 | map disabled after binding | rejected | **REJECTED** | 0 | map CTA variable may only be declared, named and added to the header row; other uses are unsupported |
| R16 | final return made true | rejected | **REJECTED** | 0 | out-of-scope candidate rejected: expected one structural match, found 0 |
| R17 | guard/assign swapped | rejected | **REJECTED** | 0 | ApplyNextActionGuidance/set current candidate: invalid control-flow order |
| R18 | V2 gate negated | rejected | **REJECTED** | 0 | RefreshNextActionGuidance/rack teaching gate: expected one structural match, found 0 |
| R19 | objectives flag widened | rejected | **REJECTED** | 0 | RefreshNextActionGuidance/objectives-scoped branch: expected one structural match, found 0 |
| R20 | help-close setter removed | rejected | **REJECTED** | 0 | CloseHelp/help-close frame recorded: expected one structural match, found 0 |
| R21 | LateUpdate apply removed | rejected | **REJECTED** | 0 | LateUpdate applies cached guidance: expected one structural match, found 0 |

### #305 reviewer: X-F1 (35)

| ID | Mutation | 1585ca20 scanner | #309 scanner | C# 9 compile errors | #309 rejection reason (abridged) |
|---|---|---|---|---|---|
| X-F1-01 | R05 via a helper that builds the primary (literal outside the constructor) | accepted | **REJECTED** | 0 | LearningObjectivesView.cs:83: header row addition of a button that is not a reviewed UiKit.Button declaration (RR-1) |
| X-F1-02 | R05 via a helper that promotes an existing button | accepted | **REJECTED** | 0 | LearningObjectivesView.cs:83: header local 'close' is used outside the reviewed straight-line header shapes (RR-1/RR-3) |
| X-F1-03 | map made primary from a method outside the constructor, name split to dodge the literal count | accepted | **REJECTED** | 0 | LearningObjectivesView.cs:194: a class-list API (only the reviewed header may style a button) in the Objectives view class is unsupported (RR-1/RR-3) |
| X-F1-04 | unnamed extra primary created outside the constructor, appended to the header row | accepted | **REJECTED** | 0 | LearningObjectivesView.cs:194: element-tree traversal or query in the Objectives view class is unsupported (RR-1/RR-3) |
| X-F1-05 | partial class in another file promotes the map (scanner never reads that file) | accepted | **REJECTED** | 0 | LearningObjectivesView.Sequence.cs:9: a class-list API (only the reviewed header may style a button) in the Objectives view class is unsupported (RR-… |
| X-F1-06 | UiKit.Button itself makes every button primary (scanner never reads UiKit) | accepted | **REJECTED** | 0 | UiKit.cs:66: UiKit must not attach btn-primary itself (RR-1) |
| X-F1-07 | interpolated class name on the map | rejected | **REJECTED** | 0 | AddToClassList in the header uses a non-literal class name (ambiguous) |
| X-F1-08 | interpolated class name on an inline primary | rejected | **REJECTED** | 0 | UiKit.Button class argument in the header is not a plain string literal (ambiguous) |
| X-F1-09 | C# 11 raw string class name (not valid at Unity's C# level) | rejected | **REJECTED** | 1 (CS8773) | LearningObjectivesView.cs:75: the lexer cannot follow this source (raw string literal (C# 11) is unsupported); unsupported literal/comment shapes fai… |
| X-F1-10 | map primary inside an if | rejected | **REJECTED** | 0 | map CTA must not be unconditionally primary (AddToClassList) |
| X-F1-11 | map primary deferred in a lambda | rejected | **REJECTED** | 0 | map CTA must not be unconditionally primary (AddToClassList) |
| X-F1-12 | map primary by ToggleInClassList | rejected | **REJECTED** | 0 | map CTA must not be unconditionally primary (ToggleInClassList) |
| X-F1-13 | verbatim identifier @map | rejected | **REJECTED** | 0 | map CTA must not be unconditionally primary (AddToClassList) |
| X-F1-14 | unicode-escaped identifier | rejected | **REJECTED** | 0 | LearningObjectivesView.cs:75: the lexer cannot follow this source (backslash outside a literal (Unicode-escaped identifier)); unsupported literal/com… |
| X-F1-15 | escaped class literal \x | rejected | **REJECTED** | 0 | map CTA must not be unconditionally primary (AddToClassList) |
| X-F1-16 | new Button primary, no name | rejected | **REJECTED** | 0 | static primary 'close' must have exactly one literal name; found 0 |
| X-F1-17 | Q-chained primary | rejected | **REJECTED** | 0 | AddToClassList(btn-primary) on a non-simple receiver in the header (unnamed primary) |
| X-F1-18 | row indexer primary | rejected | **REJECTED** | 0 | AddToClassList(btn-primary) on a non-simple receiver in the header (unnamed primary) |
| X-F1-19 | two-step declaration of a primary | rejected | **REJECTED** | 0 | static btn-primary header button is not assigned to a named local (unnamed primary) |
| X-F1-20 | class assigned to the forest variable after declaration (reassignment) | rejected | **REJECTED** | 0 | static primary 'forest' must be declared once and never reassigned in the header |
| X-F1-21 | duplicate allowlist spelling (ordinary + verbatim) | rejected | **REJECTED** | 0 | GuidanceButtonNames contains a duplicate name |
| X-F1-22 | allowlist entry commented out (R03 with the text still present) | rejected | **REJECTED** | 0 | static primary button 'objective-back-to-forest' is not in the GuidanceButtonNames scoped demotion set |
| X-F1-23 | allowlist entry behind #if (R03 in a Player build) | rejected | **REJECTED** | 0 | GuidanceButtonNames: conditional compilation (#if) in a checked method or declaration is unsupported |
| X-F1-24 | R04 by compound assignment: Back to forest's runtime name leaves the allowlist | accepted | **REJECTED** | 0 | LearningObjectivesView.cs:81: header local 'forest' is used outside the reviewed straight-line header shapes (RR-1/RR-3) |
| X-F1-25 | R04 by cast: second name write the receiver parse cannot attribute | accepted | **REJECTED** | 0 | LearningObjectivesView.cs:81: header local 'forest' is used outside the reviewed straight-line header shapes (RR-1/RR-3) |
| X-F1-26 | R04 by alias: second name write through another local | accepted | **REJECTED** | 0 | LearningObjectivesView.cs:81: header local 'forest' is used outside the reviewed straight-line header shapes (RR-1/RR-3) |
| X-F1-27 | allowlist element overwritten at run time through a qualified reference (R03 at run time) | accepted | **REJECTED** | 0 | GuidanceButtonNames must be used only by its declaration and the demotion loop; found 3 references |
| X-F1-28 | method-group + class-level const: no call shape and no literal in the constructor | accepted | **REJECTED** | 0 | LearningObjectivesView.cs:83: header local 'close' is used outside the reviewed straight-line header shapes (RR-1/RR-3) |
| X-F1-29 | lexer desync: char literal inside an interpolation hole hides a map primary | accepted | **REJECTED** | 0 | LearningObjectivesView.cs:75: literal/comment boundaries are ambiguous (for example a literal nested in an interpolation hole, or a quote in a prepro… |
| X-F1-30 | lexer desync: @$ verbatim-interpolated backslash hides a map primary | accepted | **REJECTED** | 0 | LearningObjectivesView.cs:75: literal/comment boundaries are ambiguous (for example a literal nested in an interpolation hole, or a quote in a prepro… |
| X-F1-31 | second static primary on an allowlisted button (demoted in guided state, doubled outside it) | accepted | **REJECTED** | 0 | LearningObjectivesView.cs:78: header local 'work' is used outside the reviewed straight-line header shapes (RR-1/RR-3) |
| X-F1-32 | new CloseAll primary reusing another view's allowlisted name (demoted in guided state, doubled outside it) | accepted | **REJECTED** | 0 | LearningObjectivesView.cs:82: extra header button 'close' must be non-primary, added once and not use a guidance name; only Back to forest is statica… |
| X-F1-33 | names swapped between Work Plan and Back to forest (both allowlisted): later-state targets route to the wrong button | accepted | **REJECTED** | 0 | LearningObjectivesView.cs:76: header button 'Work Plan [Tab]' must be ui.ShowWorkPlan(), without classes, named 'objective-work-plan' and added to th… |
| X-F1-34 | second non-primary button with the map caption that closes the screen | accepted | **REJECTED** | 0 | header button 'Stand map [M]' must be declared exactly once; found 2 (RR-1) |
| X-F1-35 | spoof captioned map + real map primary (tolerant lookup picks the spoof) | rejected | **REJECTED** | 0 | static primary button 'objective-primary-cta' is not in the GuidanceButtonNames scoped demotion set |

### #305 reviewer: X-F2 (46)

| ID | Mutation | 1585ca20 scanner | #309 scanner | C# 9 compile errors | #309 rejection reason (abridged) |
|---|---|---|---|---|---|
| X-F2-01 | R08 by row indexer: map renamed through buttons[0] | accepted | **REJECTED** | 0 | LearningObjectivesView.cs:76: header local 'buttons' is used outside the reviewed straight-line header shapes (RR-1/RR-3) |
| X-F2-02 | R15 by row indexer: map disabled through buttons[0] | accepted | **REJECTED** | 0 | LearningObjectivesView.cs:76: header local 'buttons' is used outside the reviewed straight-line header shapes (RR-1/RR-3) |
| X-F2-03 | R15 by parent: whole button row disabled | accepted | **REJECTED** | 0 | LearningObjectivesView.cs:82: header local 'buttons' is used outside the reviewed straight-line header shapes (RR-1/RR-3) |
| X-F2-04 | row cleared after binding: Q<Button> finds no map | accepted | **REJECTED** | 0 | LearningObjectivesView.cs:82: header local 'buttons' is used outside the reviewed straight-line header shapes (RR-1/RR-3) |
| X-F2-05 | row binding wrapped in if (false) | accepted | **REJECTED** | 0 | LearningObjectivesView.cs:75: header local 'buttons' is used outside the reviewed straight-line header shapes (RR-1/RR-3) |
| X-F2-06 | map identity wrapped in if (false) | accepted | **REJECTED** | 0 | LearningObjectivesView.cs:74: header local 'map' is used outside the reviewed straight-line header shapes (RR-1/RR-3) |
| X-F2-07 | header row hidden | accepted | **REJECTED** | 0 | LearningObjectivesView.cs:83: header local 'header' is used outside the reviewed straight-line header shapes (RR-1/RR-3) |
| X-F2-08 | R13 count-preserving: first guard replaced by a bare return, whole method dead | accepted | **REJECTED** | 0 | ScenarioOneUiRoot.cs:337: pinned method RefreshNextActionGuidance changed (expected 'if', found 'return'); a reviewed method may change only together… |
| X-F2-09 | R11 by tuple assignment | accepted | **REJECTED** | 0 | ScenarioOneUiRoot.cs:452: pinned method RefreshNextActionGuidance changed (expected 'guidanceCandidate', found '('); a reviewed method may change onl… |
| X-F2-10 | R11 count-compensated: retarget inside the chain, one other target write removed | accepted | **REJECTED** | 0 | ScenarioOneUiRoot.cs:361: pinned method RefreshNextActionGuidance changed (expected 'target', found 'reason'); a reviewed method may change only toge… |
| X-F2-11 | objectives flag overwritten after its pinned declaration | accepted | **REJECTED** | 0 | ScenarioOneUiRoot.cs:369: pinned method RefreshNextActionGuidance changed (expected 'bool', found 'objectives'); a reviewed method may change only to… |
| X-F2-12 | annual-review branch forced, map target unreachable | accepted | **REJECTED** | 0 | ScenarioOneUiRoot.cs:370: pinned method RefreshNextActionGuidance changed (expected 'NeedsAnnualReview', found 'true'); a reviewed method may change … |
| X-F2-13 | consent forced true, chain never reaches the map target | accepted | **REJECTED** | 0 | ScenarioOneUiRoot.cs:356: pinned method RefreshNextActionGuidance changed (expected 'mapQuestions', found 'true'); a reviewed method may change only … |
| X-F2-14 | teaching gate widened after the pinned prefix: map CTA offered beyond Objective 1 | accepted | **REJECTED** | 0 | ScenarioOneUiRoot.cs:364: pinned method RefreshNextActionGuidance changed (expected '(', found 'true'); a reviewed method may change only together wi… |
| X-F2-15 | routing literal swapped, count unchanged | accepted | **REJECTED** | 0 | ScenarioOneUiRoot.cs:381: pinned method RefreshNextActionGuidance changed (expected '"map-plan-rack"', found '"map-back-to-forest"'); a reviewed meth… |
| X-F2-16 | Back to forest re-promoted right after the demotion loop (original two-primary defect) | accepted | **REJECTED** | 0 | ScenarioOneUiRoot.cs:451: pinned method RefreshNextActionGuidance changed (expected 'guidanceScope', found 'learning'); a reviewed method may change … |
| X-F2-17 | demotion undone by a second loop with a renamed variable | accepted | **REJECTED** | 0 | ScenarioOneUiRoot.cs:451: pinned method RefreshNextActionGuidance changed (expected 'guidanceScope', found 'foreach'); a reviewed method may change o… |
| X-F2-18 | CTA primary class removed every frame after the LateUpdate apply | accepted | **REJECTED** | 0 | ScenarioOneUiRoot.cs:328: 'nextAction' is referenced outside the reviewed methods (qualified, tuple, ref, lambda, shadow or extra use) (RR-2/RR-3) |
| X-F2-19 | CTA primary class removed in Apply through nextAction | accepted | **REJECTED** | 0 | ScenarioOneUiRoot.cs:489: pinned method ApplyNextActionGuidance changed (expected 'end of body', found 'nextAction'); a reviewed method may change on… |
| X-F2-20 | R14 by plain if: primary class never applied | accepted | **REJECTED** | 0 | ScenarioOneUiRoot.cs:487: pinned method ApplyNextActionGuidance changed (expected 'candidate', found 'if'); a reviewed method may change only togethe… |
| X-F2-21 | R14 by [Conditional]: Apply compiled out of Player builds, no #if anywhere | accepted | **REJECTED** | 0 | ScenarioOneUiRoot.cs:474: a [Conditional] attribute in ScenarioOneUiRoot is unsupported (RR-3) |
| X-F2-22 | R21 by lambda: LateUpdate call never invoked | accepted | **REJECTED** | 0 | ScenarioOneUiRoot.cs:1: ScenarioOneUiRoot: expected exactly one reviewed occurrence of `private void LateUpdate() { ApplyNextActionGuidance();` (miss… |
| X-F2-23 | R21 by shadow: local function replaces the method inside LateUpdate | accepted | **REJECTED** | 0 | ScenarioOneUiRoot.cs:332: 'ApplyNextActionGuidance' is referenced outside the reviewed methods (qualified, tuple, ref, lambda, shadow or extra use) (… |
| X-F2-24 | R13 by goto: eligible candidate skips styling | accepted | **REJECTED** | 0 | ScenarioOneUiRoot.cs:482: pinned method ApplyNextActionGuidance changed (expected 'ClearNextAction', found 'if'); a reviewed method may change only t… |
| X-F2-25 | R13 by throw: eligible candidate never styled | accepted | **REJECTED** | 0 | ScenarioOneUiRoot.cs:482: pinned method ApplyNextActionGuidance changed (expected 'ClearNextAction', found 'if'); a reviewed method may change only t… |
| X-F2-26 | R09 by dead lambda: guard present but never executed | accepted | **REJECTED** | 0 | ScenarioOneUiRoot.cs:483: pinned method ApplyNextActionGuidance changed (expected 'if', found 'System'); a reviewed method may change only together w… |
| X-F2-27 | R23 with this.: candidate nulled by an unrelated handler | accepted | **REJECTED** | 0 | 'guidanceCandidate' must be written only by RefreshNextActionGuidance/ApplyNextActionGuidance/ClearNextAction |
| X-F2-28 | R20c with this.: help-close frame reset | accepted | **REJECTED** | 0 | helpClosedFrame must be written only by its initialiser and CloseHelp |
| X-F2-29 | R20 by inverted early guard: setter unreachable while Help is open | accepted | **REJECTED** | 0 | ScenarioOneUiRoot.cs:611: pinned method CloseHelp changed (expected '!', found 'help'); a reviewed method may change only together with this checker … |
| X-F2-30 | R20 by lambda: setter never invoked | accepted | **REJECTED** | 0 | ScenarioOneUiRoot.cs:614: pinned method CloseHelp changed (expected 'helpClosedFrame', found 'System'); a reviewed method may change only together wi… |
| X-F2-31 | GuidanceVisible parameter overwritten: every candidate reads as visible | accepted | **REJECTED** | 0 | ScenarioOneUiRoot.cs:493: pinned method GuidanceVisible changed (expected 'for', found 'element'); a reviewed method may change only together with th… |
| X-F2-32 | GuidanceVisible skips the candidate's own visibility | accepted | **REJECTED** | 0 | ScenarioOneUiRoot.cs:495: pinned method GuidanceVisible changed (expected '.', found '=='); a reviewed method may change only together with this chec… |
| X-F2-33 | only Objectives caller of Refresh removed: guidance never derived | accepted | **REJECTED** | 0 | RefreshNextActionGuidance must be called exactly once by the view class, from Refresh(bool); found 0 (RR-3 missing/extra caller) |
| X-F2-34 | only Objectives caller of Refresh behind #if (R14 at the call site) | accepted | **REJECTED** | 0 | \bpublic\s+void\s+Refresh\s*\(\s*bool\s+force\s*\): conditional compilation (#if) in a checked method or declaration is unsupported |
| X-F2-35 | ShowMap gutted: CTA still wired by name, opens nothing | accepted | **REJECTED** | 0 | ScenarioOneUiRoot.cs:690: pinned method ShowMap changed (expected 'Map', found 'None'); a reviewed method may change only together with this checker … |
| X-F2-36 | ordering: candidate bound after Apply | rejected | **REJECTED** | 0 | RefreshNextActionGuidance/apply after binding: expected one structural match, found 2 |
| X-F2-37 | guard in block comment | rejected | **REJECTED** | 0 | ApplyNextActionGuidance/no styling if ineligible: expected one structural match, found 0 |
| X-F2-38 | guard in verbatim string | rejected | **REJECTED** | 0 | ApplyNextActionGuidance/no styling if ineligible: expected one structural match, found 0 |
| X-F2-39 | guard in interpolated string | rejected | **REJECTED** | 0 | ApplyNextActionGuidance/no styling if ineligible: expected one structural match, found 0 |
| X-F2-40 | duplicate CTA name with \x escape | rejected | **REJECTED** | 0 | duplicate element name in the Objectives constructor |
| X-F2-41 | duplicate CTA name in an object initializer | rejected | **REJECTED** | 0 | duplicate element name in the Objectives constructor |
| X-F2-42 | duplicate CTA name via class-level const | rejected | **REJECTED** | 0 | an element name in the Objectives constructor is not a single plain literal |
| X-F2-43 | allowlist shadowed by a local in Refresh | rejected | **REJECTED** | 0 | GuidanceButtonNames allowlist: expected one structural match, found 2 |
| X-F2-44 | Apply wrapped in #if UNITY_EDITOR | rejected | **REJECTED** | 2 (CS0103) | \bprivate\s+void\s+ApplyNextActionGuidance\s*\(\s*\): declared inside conditional compilation (#if) |
| X-F2-45 | candidate nulled by ref helper in Apply | rejected | **REJECTED** | 0 | ApplyNextActionGuidance writes 'candidate' more than once (override) |
| X-F2-46 | second Apply overload shadow attempt: call site no longer zero-argument | rejected | **REJECTED** | 0 | LateUpdate applies cached guidance: expected one structural match, found 0 |

### #309 RR-1 (28)

| ID | Mutation | 1585ca20 scanner | #309 scanner | C# 9 compile errors | #309 rejection reason (abridged) |
|---|---|---|---|---|---|
| RR1-01 | static primary on a lesson card button outside the header | accepted | **REJECTED** | 0 | LearningObjectivesView.cs:212: btn-primary attached by UiKit.Button outside the reviewed site (RR-1) |
| RR1-02 | lesson card button made primary from string fragments | accepted | **REJECTED** | 0 | LearningObjectivesView.cs:212: UiKit.Button class argument is not a plain string literal |
| RR1-03 | primary-building helper in the Curriculum partial | accepted | **REJECTED** | 0 | LearningObjectivesView.Curriculum.cs:10: btn-primary attached by UiKit.Button outside the reviewed site (RR-1) |
| RR1-04 | class-list helper in the Sequence partial | accepted | **REJECTED** | 0 | LearningObjectivesView.Sequence.cs:9: a class-list API (only the reviewed header may style a button) in the Objectives view class is unsupported (RR-… |
| RR1-05 | Back to forest primary class supplied by a class constant | rejected | **REJECTED** | 0 | UiKit.Button class argument in the header is not a plain string literal (ambiguous) |
| RR1-06 | UiKit.Button taken as a method group | accepted | **REJECTED** | 0 | LearningObjectivesView.cs:102: UiKit.Button used as a method group |
| RR1-07 | UiKit aliased by a using directive | accepted | **REJECTED** | 0 | LearningObjectivesView.cs:5: UiKit used other than as UiKit.Member (alias or method group) |
| RR1-08 | Work Plan made a second static primary (N-1) | accepted | **REJECTED** | 0 | LearningObjectivesView.cs:76: header button 'Work Plan [Tab]' must be ui.ShowWorkPlan(), without classes, named 'objective-work-plan' and added to th… |
| RR1-09 | Back to forest retargeted to the map | accepted | **REJECTED** | 0 | LearningObjectivesView.cs:79: header button 'Back to forest [O / Esc]' must be ui.CloseAll(), the one static primary, named 'objective-back-to-forest… |
| RR1-10 | Back to forest no longer the static primary (no static primary left) | accepted | **REJECTED** | 0 | LearningObjectivesView.cs:79: header button 'Back to forest [O / Esc]' must be ui.CloseAll(), the one static primary, named 'objective-back-to-forest… |
| RR1-11 | allowlist cleared with Array.Clear from an unpinned method | rejected | **REJECTED** | 0 | GuidanceButtonNames must be used only by its declaration and the demotion loop; found 3 references |
| RR1-12 | allowlist element overwritten through a qualified reference in CloseAll | accepted | **REJECTED** | 0 | GuidanceButtonNames must be used only by its declaration and the demotion loop; found 3 references |
| RR1-13 | Back to forest re-promoted from an unpinned root method | accepted | **REJECTED** | 0 | ScenarioOneUiRoot.cs:696: the Objectives Root is used outside the reviewed root code (RR-1) |
| RR1-14 | re-promotion from the document root with name and class fragments | accepted | **REJECTED** | 0 | ScenarioOneUiRoot.cs:622: class-list call outside the reviewed methods must name a plain, non-primary class literal (RR-1) |
| RR1-15 | root aliases the view object and disables every Objectives button | accepted | **REJECTED** | 0 | ScenarioOneUiRoot.cs:622: the Objectives view field is reassigned, aliased or passed on (RR-1/RR-2) |
| RR1-16 | Objectives Root reached through the public Learning property | accepted | **REJECTED** | 0 | ScenarioOneUiRoot.cs:702: the Objectives Root is used outside the reviewed root code (RR-1) |
| RR1-17 | another class disables the map CTA by name | accepted | **REJECTED** | 0 | WalkingHudView.cs:183: another class names an Objectives header button (RR-1) |
| RR1-18 | another class reaches the Objectives Root through ui.Learning | accepted | **REJECTED** | 0 | StandMapView.cs:27: another class reaches the Objectives Root (RR-1) |
| RR1-19 | UiKit.Button overload makes two-argument buttons primary from fragments | accepted | **REJECTED** | 0 | UiKit must declare exactly one Button, Box, Row, Text, Add and MakeModal; found ['Add', 'Box', 'Button', 'Button', 'MakeModal', 'Row', 'Text'] (N-3) |
| RR1-20 | UiKit.MakeModal disables the whole modal | accepted | **REJECTED** | 0 | UiKit.cs:34: pinned method UiKit.MakeModal changed (expected 'RegisterCallback', found 'SetEnabled'); a reviewed method may change only together with… |
| RR1-28 | UiKit.Text (called for the header title) disables its parent | accepted | **REJECTED** | 0 | UiKit.cs:43: pinned method UiKit.Text changed (expected 'if', found 'label'); a reviewed method may change only together with this checker (#309 RR-2… |
| RR1-21 | ScenarioOneUiRoot made partial (a second file could extend it) | accepted | **REJECTED** | 0 | ScenarioOneUiRoot must be declared exactly once, non-partial, in Assets/ForestPrototype/UI/ScenarioOneUiRoot.cs; found [('Assets/ForestPrototype/UI/S… |
| RR1-22 | new LearningObjectivesView partial file disables the Objectives Root | accepted | **REJECTED** | 0 | LearningObjectivesView.Extra.cs:3: the Objectives Root may be used only by its declaration and the constructor (RR-1) |
| RR1-23 | constructor ui parameter reassigned after the click lambdas capture it | accepted | **REJECTED** | 0 | LearningObjectivesView.cs:82: header local 'ui' is used outside the reviewed straight-line header shapes (RR-1/RR-3) |
| RR1-24 | header local passed to a helper from inside an interpolation hole | accepted | **REJECTED** | 0 | LearningObjectivesView.cs:84: header local 'forest' is used outside the reviewed straight-line header shapes (RR-1/RR-3) |
| RR1-25 | modal cleared after the header is attached | accepted | **REJECTED** | 0 | LearningObjectivesView.cs:83: header local 'modal' is used outside the reviewed straight-line header shapes (RR-1/RR-3) |
| RR1-26 | upward element-tree traversal from the view | accepted | **REJECTED** | 0 | LearningObjectivesView.cs:102: element-tree traversal or query in the Objectives view class is unsupported (RR-1/RR-3) |
| RR1-27 | [Conditional] applied through a ConditionalAttribute alias | accepted | **REJECTED** | 0 | ScenarioOneUiRoot.cs:4: a [Conditional] attribute in ScenarioOneUiRoot is unsupported (RR-3) |

### #309 RR-2 (15)

| ID | Mutation | 1585ca20 scanner | #309 scanner | C# 9 compile errors | #309 rejection reason (abridged) |
|---|---|---|---|---|---|
| RR2-01 | this.helpClosedFrame reset in an unpinned method | accepted | **REJECTED** | 0 | helpClosedFrame must be written only by its initialiser and CloseHelp |
| RR2-02 | this.guidanceCandidate nulled in CloseAll | accepted | **REJECTED** | 0 | 'guidanceCandidate' must be written only by RefreshNextActionGuidance/ApplyNextActionGuidance/ClearNextAction |
| RR2-03 | this.nextAction nulled in TryShowReviewYear | accepted | **REJECTED** | 0 | 'nextAction' must be written only by RefreshNextActionGuidance/ApplyNextActionGuidance/ClearNextAction |
| RR2-04 | tuple reset of the guidance state | accepted | **REJECTED** | 0 | ScenarioOneUiRoot.cs:702: 'guidanceCandidate' is referenced outside the reviewed methods (qualified, tuple, ref, lambda, shadow or extra use) (RR-2/R… |
| RR2-05 | help-close frame reset through a ref argument | rejected | **REJECTED** | 0 | helpClosedFrame must be written only by its initialiser and CloseHelp |
| RR2-06 | primary tracking cleared outside Refresh (restoration lost) | accepted | **REJECTED** | 0 | ScenarioOneUiRoot.cs:696: 'guidancePrimaries' is referenced outside the reviewed methods (qualified, tuple, ref, lambda, shadow or extra use) (RR-2/R… |
| RR2-07 | dead first guard: if (true) return | accepted | **REJECTED** | 0 | ScenarioOneUiRoot.cs:337: pinned method RefreshNextActionGuidance changed (expected 'learning', found 'true'); a reviewed method may change only toge… |
| RR2-08 | target retargeted through a ref helper before binding | rejected | **REJECTED** | 0 | RefreshNextActionGuidance writes target 15 times; expected 14 (routing change needs checker review) |
| RR2-09 | map and Back to forest targets swapped (count unchanged) | rejected | **REJECTED** | 0 | RefreshNextActionGuidance/no rack plan yet: expected one structural match, found 0 |
| RR2-10 | Help reference reset: guidance suspended forever | accepted | **REJECTED** | 0 | ScenarioOneUiRoot must assign 'help' exactly 1 time(s) (RR-2 related state) |
| RR2-11 | manager reference reset through this. | accepted | **REJECTED** | 0 | ScenarioOneUiRoot must assign 'manager' exactly 1 time(s) (RR-2 related state) |
| RR2-12 | Objectives view reference reset: Refresh returns at its first guard | accepted | **REJECTED** | 0 | ScenarioOneUiRoot.cs:702: the Objectives view field is reassigned, aliased or passed on (RR-1/RR-2) |
| RR2-13 | help-close frame initialiser changed | accepted | **REJECTED** | 0 | ScenarioOneUiRoot.cs:1: ScenarioOneUiRoot: expected exactly one reviewed occurrence of `private int helpClosedFrame = -1;` (missing, changed or dupli… |
| RR2-15 | screen state reset right after Objectives opens (no Objectives guidance scope) | accepted | **REJECTED** | 0 | ScenarioOneUiRoot must assign 'screen' exactly 3 time(s) (RR-2 related state) |
| RR2-14 | guidance scope reset inside a lambda | rejected | **REJECTED** | 0 | 'guidanceScope' must be written only by RefreshNextActionGuidance/ApplyNextActionGuidance/ClearNextAction |

### #309 RR-3 (14)

| ID | Mutation | 1585ca20 scanner | #309 scanner | C# 9 compile errors | #309 rejection reason (abridged) |
|---|---|---|---|---|---|
| RR3-01 | [Conditional] on the view Refresh (the caller vanishes from Player builds) | accepted | **REJECTED** | 0 | LearningObjectivesView.cs:125: a [Conditional] attribute in the Objectives view class is unsupported (RR-1/RR-3) |
| RR3-02 | [Conditional] on SetScreen | accepted | **REJECTED** | 0 | ScenarioOneUiRoot.cs:562: a [Conditional] attribute in ScenarioOneUiRoot is unsupported (RR-3) |
| RR3-03 | unreviewed attribute on ApplyNextActionGuidance | accepted | **REJECTED** | 0 | ScenarioOneUiRoot.cs:475: an attribute on a checked method is unsupported (e.g. [Conditional]) |
| RR3-04 | Objectives caller behind if (false) | accepted | **REJECTED** | 0 | LearningObjectivesView.cs:193: the Objectives RefreshNextActionGuidance call must be an unconditional top-level statement after the single reviewed e… |
| RR3-05 | Objectives caller inside a never-invoked lambda | accepted | **REJECTED** | 0 | LearningObjectivesView.cs:193: the Objectives RefreshNextActionGuidance call must be an unconditional top-level statement after the single reviewed e… |
| RR3-06 | early return before the Objectives caller | accepted | **REJECTED** | 0 | LearningObjectivesView.cs:194: the Objectives RefreshNextActionGuidance call must be an unconditional top-level statement after the single reviewed e… |
| RR3-07 | throw before the Objectives caller | accepted | **REJECTED** | 0 | LearningObjectivesView.cs:194: the Objectives RefreshNextActionGuidance call must be an unconditional top-level statement after the single reviewed e… |
| RR3-08 | goto over the Objectives caller | accepted | **REJECTED** | 0 | LearningObjectivesView.cs:194: the Objectives RefreshNextActionGuidance call must be an unconditional top-level statement after the single reviewed e… |
| RR3-09 | local function named like the guidance method in the view Refresh | accepted | **REJECTED** | 0 | RefreshNextActionGuidance must be called exactly once by the view class, from Refresh(bool); found 2 (RR-3 missing/extra caller) |
| RR3-10 | SetScreen no longer refreshes the Objectives view | accepted | **REJECTED** | 0 | ScenarioOneUiRoot.cs:582: pinned method SetScreen changed (expected 'Refresh', found 'Record'); a reviewed method may change only together with this … |
| RR3-11 | LateUpdate apply behind if (false) | rejected | **REJECTED** | 0 | LateUpdate may only apply guidance unconditionally before any branch or return |
| RR3-12 | CTA primary class removed late from CloseAll | accepted | **REJECTED** | 0 | ScenarioOneUiRoot.cs:702: 'nextAction' is referenced outside the reviewed methods (qualified, tuple, ref, lambda, shadow or extra use) (RR-2/RR-3) |
| RR3-13 | map CTA primary removed from an unpinned method with class fragments | accepted | **REJECTED** | 0 | ScenarioOneUiRoot.cs:622: the Objectives Root is used outside the reviewed root code (RR-1) |
| RR3-14 | header row hidden through a Root query in the constructor | accepted | **REJECTED** | 0 | LearningObjectivesView.cs:83: header local 'Root' is used outside the reviewed straight-line header shapes (RR-1/RR-3) |

### #309 N-4 (5)

| ID | Mutation | 1585ca20 scanner | #309 scanner | C# 9 compile errors | #309 rejection reason (abridged) |
|---|---|---|---|---|---|
| N4-01 | char literal in an interpolation hole hides a root write | accepted | **REJECTED** | 0 | ScenarioOneUiRoot.cs:702: literal/comment boundaries are ambiguous (for example a literal nested in an interpolation hole, or a quote in a preprocess… |
| N4-02 | @$ verbatim interpolation in a partial (lexed precisely, hidden code found) | accepted | **REJECTED** | 0 | LearningObjectivesView.Sequence.cs:9: the Objectives Root may be used only by its declaration and the constructor (RR-1) |
| N4-03 | verbatim string opened in an inactive #if region hides active code | accepted | **REJECTED** | 0 | LearningObjectivesView.cs:103: a multi-line literal or comment spans a preprocessor line; fail closed (N-4) |
| N4-04 | block comment opened in an inactive #if region hides active code | accepted | **REJECTED** | 0 | ScenarioOneUiRoot.cs:703: a multi-line literal or comment spans a preprocessor line; fail closed (N-4) |
| N4-05 | Unicode-escaped identifier writes guarded state | accepted | **REJECTED** | 0 | ScenarioOneUiRoot.cs:702: the lexer cannot follow this source (backslash outside a literal (Unicode-escaped identifier)); unsupported literal/comment… |

### Benign controls (32)

| ID | Benign edit | 1585ca20 scanner | #309 scanner | C# 9 compile errors | |
|---|---|---|---|---|---|
| B-B01-trailing-comment-after-eligibility-return | B01-trailing-comment-after-eligibility-return | accepted | **ACCEPTED** | 0 |  |
| B-B02-map-constructor-reformatted | B02-map-constructor-reformatted | accepted | **ACCEPTED** | 0 |  |
| B-B03-comment-mentions-primary-and-conditional | B03-comment-mentions-primary-and-conditional | accepted | **ACCEPTED** | 0 |  |
| B-B04-new-nonprimary-header-button-unnamed | B04-new-nonprimary-header-button-unnamed | accepted | **ACCEPTED** | 0 |  |
| B-X-01 | map local renamed consistently | accepted | **ACCEPTED** | 0 |  |
| B-X-02 | forest local renamed consistently | accepted | **ACCEPTED** | 0 |  |
| B-X-03 | forest declared with var | accepted | **ACCEPTED** | 0 |  |
| B-X-04 | block comment inside the identity statement | accepted | **ACCEPTED** | 0 |  |
| B-X-05 | guard split over two lines | accepted | **ACCEPTED** | 0 |  |
| B-X-06 | work and forest order swapped in the row | accepted | **ACCEPTED** | 0 |  |
| B-X-07 | new named non-primary header button | accepted | **ACCEPTED** | 0 |  |
| B-X-08 | extra harmless class on the static primary | accepted | **ACCEPTED** | 0 |  |
| B-X-09 | #region in the constructor | accepted | **ACCEPTED** | 0 |  |
| B-X-10 | comment with apostrophe and quote in the constructor | accepted | **ACCEPTED** | 0 |  |
| B-X-11 | #if in an unrelated root method | accepted | **ACCEPTED** | 0 |  |
| B-X-12 | string with // in the constructor | accepted | **ACCEPTED** | 0 |  |
| B-X-13 | header label containing the word primary | accepted | **ACCEPTED** | 0 |  |
| NB-01 | pinned Refresh guard reformatted with a trailing comment | accepted | **ACCEPTED** | 0 |  |
| NB-02 | comments added inside pinned ApplyNextActionGuidance | accepted | **ACCEPTED** | 0 |  |
| NB-03 | new interpolated string with nested literals in a partial (precise lexer) | accepted | **ACCEPTED** | 0 |  |
| NB-04 | additional root caller of RefreshNextActionGuidance | accepted | **ACCEPTED** | 0 |  |
| NB-05 | #region around LateUpdate | accepted | **ACCEPTED** | 0 |  |
| NB-06 | non-primary class on a lesson card button | accepted | **ACCEPTED** | 0 |  |
| NB-07 | unrelated new UiKit helper | accepted | **ACCEPTED** | 0 |  |
| NB-08 | plain non-primary class literal added in an unpinned root method | accepted | **ACCEPTED** | 0 |  |
| NB-12 | interpolation with an alignment comma in a lesson label | accepted | **ACCEPTED** | 0 |  |
| NB-13 | #if/#endif with trailing comments in an unpinned root method | accepted | **ACCEPTED** | 0 |  |
| NB-09 | pinned ShowMap re-spaced with a comment | accepted | **ACCEPTED** | 0 |  |
| B-X-14 | CRLF line endings in view and root | accepted | **ACCEPTED** | 0 |  |
| B-X-15 | UTF-8 BOM at the start of view and root | accepted | **ACCEPTED** | 0 |  |
| NB-10 | CRLF in both partials and UiKit | accepted | **ACCEPTED** | 0 |  |
| NB-11 | UTF-8 BOM in both partials and UiKit | accepted | **ACCEPTED** | 0 |  |

### Evidence-only probes (not counted as tests)

| ID | Expectation | Probe | #309 scanner | As expected | C# 9 compile errors | Reason (abridged) |
|---|---|---|---|---|---|---|
| P-A01 | REJECT | Back to forest gets btn-primary twice | REJECTED | True | 0 | LearningObjectivesView.cs:79: header button 'Back to forest [O / Esc]' must be ui.CloseAll(), the one static primary, named 'objective-back-to-forest… |
| P-A02 | REJECT | Back to forest class literal 'btn-primary next-action' (not the btn-primary class) | REJECTED | True | 0 | LearningObjectivesView.cs:79: header button 'Back to forest [O / Esc]' must be ui.CloseAll(), the one static primary, named 'objective-back-to-forest… |
| P-A03 | REJECT | map inserted with Insert instead of Add | REJECTED | True | 0 | Objectives constructor/row binding: expected one structural match, found 0 |
| P-A04 | REJECT | root clears the guidance candidate's classes from CloseAll | REJECTED | True | 0 | ScenarioOneUiRoot.cs:702: 'ClearNextAction' is referenced outside the reviewed methods (qualified, tuple, ref, lambda, shadow or extra use) (RR-2/RR-… |
| P-A05 | REJECT | static constructor added in a partial | REJECTED | True | 0 | LearningObjectivesView must have exactly one constructor; found 2 |
| P-A06 | REJECT | field initializer builds a primary button in a partial | REJECTED | True | 0 | LearningObjectivesView.Sequence.cs:9: btn-primary attached by UiKit.Button outside the reviewed site (RR-1) |
| P-A07 | REJECT | property wraps the guidance candidate | REJECTED | True | 0 | 'guidanceCandidate' must be written only by RefreshNextActionGuidance/ApplyNextActionGuidance/ClearNextAction |
| P-A08 | REJECT | ClearClassList on the guided button from an unpinned method | REJECTED | True | 0 | ScenarioOneUiRoot.cs:702: class-list call outside the reviewed methods must name a plain, non-primary class literal (RR-1) |
| P-A09 | REJECT | allowlist loses readonly | REJECTED | True | 0 | ScenarioOneUiRoot.cs:1: ScenarioOneUiRoot: expected exactly one reviewed occurrence of `private static readonly string[] GuidanceButtonNames = {` (mi… |
| P-A10 | REJECT | Refresh signature gains an optional parameter | REJECTED | True | 0 | expected exactly one method matching '\\bpublic\\s+void\\s+RefreshNextActionGuidance\\s*\\(\\s*\\)'; found 0 |
| P-A11 | REJECT | constructor ui parameter aliased | REJECTED | True | 0 | LearningObjectivesView.cs:82: header local 'ui' is used outside the reviewed straight-line header shapes (RR-1/RR-3) |
| P-A12 | REJECT | nameof on a header local (conservative) | REJECTED | True | 0 | LearningObjectivesView.cs:82: header local 'forest' is used outside the reviewed straight-line header shapes (RR-1/RR-3) |
| P-A13 | REJECT | modal indexer disables the header | REJECTED | True | 0 | LearningObjectivesView.cs:83: header local 'modal' is used outside the reviewed straight-line header shapes (RR-1/RR-3) |
| P-A14 | REJECT | Root used from a view method through this. | REJECTED | True | 0 | LearningObjectivesView.cs:140: the Objectives Root may be used only by its declaration and the constructor (RR-1) |
| P-A15 | REJECT | #if/#else swaps the help-close initialiser | REJECTED | True | 0 | helpClosedFrame must be written only by its initialiser and CloseHelp |
| L-01 | LIMIT | a class outside the companion set disables the Work Plan button by a constructed name | ACCEPTED | True | 0 |  |
| L-02 | LIMIT | root disables every button of its own document from an unpinned method (no class change) | ACCEPTED | True | 0 |  |
| L-03 | LIMIT | non-terminating loop before the Objectives caller | ACCEPTED | True | 0 |  |
| L-04 | LIMIT | reflection writes guarded state by a string name | ACCEPTED | True | 0 |  |
| P-A16 | REJECT | UiKit.Text disables the parent of every label (the header holds the title label) | REJECTED | True | 0 | UiKit.cs:43: pinned method UiKit.Text changed (expected 'if', found 'label'); a reviewed method may change only together with this checker (#309 RR-2… |
| L-06 | LIMIT | another class aliases ui.Learning before reaching Root | ACCEPTED | True | 0 |  |
| L-07 | LIMIT | exception thrown by an expression before the Objectives caller | ACCEPTED | True | 0 |  |
| O-01 | OVER | literal nested in an interpolation hole in LearningObjectivesView.cs (legacy-lexer files fail closed) | REJECTED | True | 0 | LearningObjectivesView.cs:139: literal/comment boundaries are ambiguous (for example a literal nested in an interpolation hole, or a quote in a prepr… |
| O-02 | OVER | apostrophes in two preprocessor lines of ScenarioOneUiRoot.cs (legacy regex pairs them) | REJECTED | True | 0 | ScenarioOneUiRoot.cs:693: literal/comment boundaries are ambiguous (for example a literal nested in an interpolation hole, or a quote in a preprocess… |
| O-03 | OVER | spaces added inside a pinned interpolation hole | REJECTED | True | 0 | ScenarioOneUiRoot.cs:418: pinned method RefreshNextActionGuidance changed (expected '$"Approval needs {UiKit.Money(cost)} including approved work; av… |
| O-04 | OVER | null-conditional call of the Objectives guidance refresh | REJECTED | True | 0 | LearningObjectivesView.cs:193: the Objectives RefreshNextActionGuidance call must be an unconditional top-level statement after the single reviewed e… |

---

## Repair loop 1 (post-#314)

**Status: HANDED-OFF for an exact-head independent re-review. NOT approved.** This section is worker evidence for source/offline verification tooling only. It is not an independent review, not Manager #200 acceptance, and not Unity, render, input or runtime evidence. Sections above this one are the original #309 handoff (code `84d0ba92`, handoff `b4d47d3d`) and are kept as written; where this loop makes one of their statements wrong, the [correction notes](#loop-1-correction-notes-to-the-sections-above) say so.

**Loop-1 result.** All 25 in-scope end-to-end false greens from the #314 independent review are now rejected, each for its RR-2, N-4 or RR-3 cause: S-1 10/10, S-2 11/11, S-3 4/4. The 26th case, `W-DEP-05` (game-progress logic), stays accepted and is a stated out-of-scope limit. The reviewer's 3 lone-CR cases are now rejected by a rule, not by the file reader. The checker also rejects 25 further false greens that this loop found in its own attack set (16 S-1, 4 S-2, 2 S-3, 3 L-06). All prior controls still hold: the suite gives `rejected=275/275 false_greens=0 wrong_cause=0`, all 64 benign edits are accepted, and the 18 limit and over-rejection probes keep their documented outcomes. The candidate gives **10 PASS / 0 FAIL / 2 NOT-TESTABLE, exit 0** on a traced, cwd-pinned .NET SDK **8.0.133**. The historical baseline gives **6 / 4 / 2, exit 1**, with `baseline-output.txt` byte-preserved. **No case is BLOCKED.** The coverage is still bounded, not universal; the [loop-1 limits](#loop-1-stated-limits-and-over-rejections) list what is not covered.

### Loop-1 provenance, preflight and scope

| Item | Value |
|---|---|
| Worker | T3 delegate worker, provider **claudeAgent (Claude A)**, model **claude-opus-5-5**, effort **XHIGH** (set by Mimo orchestrator). I am the original #309 author, doing the one bounded repair loop that the orchestration packet allows. |
| Input | #314 independent review `CTA-309-INDEPENDENT-REVIEW-RESULT.md` at `6f39262784f489e813c6eb3738173225a8bc223a` (NEEDS CHANGES: S-1 10, S-2 11, S-3 5 end-to-end cases, plus Low/Info S-4 to S-6). The reviewer's scripts and evidence under `/media/jer/Files/CCF/Verification/CTA-314-IndependentReview-20261107/` were read only. Its scratch five-guard wrapper `fix_feasibility.py` was not reused; I wrote the guards below independently. |
| Preflight (2026-11-07T04:05:38Z, before any edit) | Lane `/media/jer/Files/CCF/Operations/CCF-FeedbackCTARepair-20261106`, branch `verify/scenario-one-feedback-cta-anchor-20261106`, HEAD `b4d47d3d45717be9da159f0368407b10c0c3e6fe` = origin after `git fetch` (0 ahead, 0 behind). `git status --porcelain --untracked-files=all --ignored` empty, no assume-unchanged or skip-worktree flags, no other [RUNNING] on #309 after my [HANDED-OFF] at 03:18Z, no `dotnet` or Unity process. Files ext4 had 51 GiB free. [RUNNING] comment: https://github.com/jermur788/CCF/issues/309#issuecomment-6702030959 |
| Scope | The same five paths as the contract: `objective_cta_source_check.py`, `test_objective_cta_source_check.py`, `check_baseline.py` (one test-08 PASS-note line), the checker `README.md`, and this appended section. No production C#, Assets, saves, Unity, `unity-lock`, PR #300, Manager docs, ledger or `active-tasks.md`; no other lane touched. |
| Evidence | `$L1 = /media/jer/Files/CCF/Verification/CTA-RR1RR4-Repair-309-20261107/loop1/`; the original evidence beside it is untouched |

### Loop-1 exact diff

Code commit **`e29e3e1bfb943980a2fe7710449c0052ced90823`** (parent `b4d47d3d`), then a documentation commit with this addendum and one README sentence (no code). The final pushed HEAD is in the #309 [HANDED-OFF] comment.

| File | + | − | Change |
|---|---|---|---|
| `objective_cta_source_check.py` | 281 | 13 | S-1 related-state write sites, S-2 lexer alignment, S-3 routing-input pins, L-06 companion rule, `#if` check for reviewed snippets, `is_companion_source`, a whole-file token cache (performance only) |
| `test_objective_cta_source_check.py` | 545 | 4 | New loop-1 groups and runner. The 4 changed lines are the docstring, one import line and the exit condition (which now also fails on a changed probe outcome). The `1585ca20` control block (lines 28–198, sha256 `6119b5fa…`) and the #305 corpus block (sha256 `9a7a8fd2…`) still occur verbatim, once each |
| `check_baseline.py` | 2 | 1 | Test-08 PASS note only: the "selected only for unconfirmed Rack V2 Objective 1" overclaim now says it holds as far as the pinned routing text and its pinned UI inputs decide. Justified by RR-4: the PASS note repeated the README sentence that #314 S-3 found overstated |
| `README.md` | 6 + 1 | 5 + 1 | Check-08 text for S-1, S-2, S-3 and L-06; overclaim fixed; known limits rewritten around in-suite probes; control counts. One more sentence (the companion-selection rule now names `.Learning` and the Cf-stripped match) is in the docs commit with this addendum; no gate reads the README |
| this handoff | appended | | This section only |

`git diff --check b4d47d3d HEAD` exit 0. `Assets/`, `Packages/` and `ProjectSettings/` have 0 changed files since `1585ca20`. `baseline-output.txt` blob `db6a8b850dce7efadaac6fe1b87b1dbba469428f` is unchanged. `BASE` `72b4efbd…` is unchanged. All three Python files are ASCII-only, apart from three pre-existing en dashes in `check_baseline.py`; every invisible test character is written as an escape.

### Loop-1 changes by finding

**S-1 (RR-2 related state): write sites, not write counts.** The count rule stays in place and still runs first, so RR2-10, RR2-11 and RR2-15 keep their causes. After it, `_guard_related_state` enforces:
- **Pinned declarations.** `private ScenarioOneManager manager;`, `WorkPlanView workPlan`, `AnnualReviewView review`, `StandMapView map`, `RackLayoutTracker rackLayout`, `MenuHelpView help` and `private UiScreen screen = UiScreen.None;` (plus the existing `learning` declaration). A field cannot become a property that drops writes.
- **Reviewed write sites.** Each reference is written by one reviewed statement (`manager = GetComponent<…>() ?? FindFirstObjectByType<…>();`, `rackLayout = new RackLayoutTracker(this);`, `learning = …`, `workPlan = …`, `review = …`, `map = …`, `help = new MenuHelpView(this);`). Each must be a top-level statement of `Start`: inside `Start`, at brace and paren depth 0, after `;`, `{` or `}`, and not inside `#if`. `Start` may contain no `return`, `throw`, `goto` or `yield` before the last of them. `screen` is written only by its initialiser and the pinned bodies of `SetScreen` and of `TryShowReviewYear`, which is now pinned too.
- **Write shapes.** Over the root's tokens, an occurrence of one of the eight names is a write when it is followed by any assignment operator (including `>>=`), `++` or `--`; when it is preceded by `++`, `--`, `ref`, `out` or an argument `in`; when it is parenthesised (`(help) = x`); or when it is an element of a tuple or nested-tuple group that is assigned or deconstructed. A write outside the sites above fails, whatever the count. Any member access to these names through a receiver other than `this` fails. `screen`, a value type, may be neither a member receiver nor a whole call or indexer argument, which closes `ref this` extensions and `in`-parameter writers such as `UnsafeUtilityExtensions.AsRef`. `unsafe`, `__makeref`, `__refvalue`, `__reftype` and `__arglist` are banned in the root.
- **Snippets and `#if`.** `_snippet_spans` now rejects any reviewed snippet that lies inside `#if`. This covers all 16 existing root snippets, the view `Root` declaration and the new pins.

**S-2 (N-4): lexer aligned with the C# compiler.** All in `_lexed`, the one entry point for every checked file.
- U+0085, U+2028, U+2029 and a lone CR fail anywhere. Roslyn ends a line at each of them.
- Outside comments and literals, code may contain only printable ASCII, tab, LF, CR in CRLF, a leading BOM, and non-ASCII letters, digits, combining and connecting marks and space separators. A Cf character (U+200D, U+200B, U+00AD, or a U+FEFF that is not leading) or a control character in code fails, because Roslyn ignores Cf inside an identifier. The free text of `#region`, `#endregion`, `#warning` and `#error` lines counts as text.
- A `#` must start a plain directive line: spaces and tabs before it (and a BOM on line 1) and spaces and tabs after it. The differential compile probes below show that Roslyn also honours a form feed, a vertical tab or a no-break space in those places.
- A multi-line literal or comment that contains a line starting with any whitespace and `#` fails. Before this loop only spaces and tabs were checked. The differential probe D4 shows that an NBSP-indented `#endif` inside an inactive verbatim-string opener makes the following code **active** for Roslyn while the old lexer saw a string; that was a real false green (`L1-S2-04`).
- `collect_companions` (through the new `is_companion_source`) also tests the hint on the text with Cf removed, so `Learning<U+200D>ObjectivesView` is collected (`Y-N-12`). The hint also matches `.Learning`; this adds no file at this HEAD (still 29 companions).
- `_COND` accepts a leading BOM, as the precise lexer does. `_row` splits on LF only.

**Live-source claim, re-verified.** All 198 `Assets/**/*.cs` files lex with the repaired lexer (198 OK, 0 failed). No file contains U+0085, U+2028, U+2029, a lone CR (raw bytes) or a control character. The only Cf characters are two leading BOMs at offset 0 (`Assets/TutorialInfo/Readme.cs` and `Editor/ReadmeEditor.cs`, not companions). So the new rules do not reject today's source (`$L1/final/12-lex-live.stdout`).

**S-3 (RR-3): routing inputs pinned.** `public bool NeedsAnnualReview => manager != null && manager.AnnualReports.Count > 0 && !manager.AnnualReviewSeen;` in the root, and `CurrentTopicIndex => ScenarioOneTeachingSequence.Current(TeachingFacts());` and `TeachingRacks => CurrentTopicIndex == 0;` in `LearningObjectivesView.cs`, must each occur exactly once, outside `#if`. The game and teaching state they read is not pinned (limits below). The README sentence "only for an unconfirmed Rack V2 Objective 1" and the matching check-08 PASS note now say exactly that.

**Low/Info items.**
- (a) S-4: the limit probes are now in the suite as a reported group. `accepted-limit` covers L-01, L-02, L-03, L-04, L-07, the new L-08, and the reviewer's `W-DEP-05`, `Y-RR3-17` and `Y-RR3-18`; `over-rejection` covers O-01 to O-09. They are not counted as mutations. If a probe's documented outcome changes, the run fails, so the README cannot silently go stale.
- (b) S-5, L-06: **closed for the alias and pass-on shapes.** In another companion, `.Learning` may only be followed by a member access other than `Root`, or by `== null` or `!= null`. Every live use passes (`HudModel()`, `TeachingRacks`, `CurrentTopicIndex`, `Record`, `Summary`, `!= null` and others, in the UI companions and in `ScenarioOneManager.cs`). `V-L06-1`, `V-L06-2` and my former L-06 probe are rejected. Residual, still disclosed: a view-class member that returned the view itself or one of its elements would allow traversal, which is the same class as L-01.
- (c) Lone CR: closed **by rule** now (U+000D not followed by LF fails in memory). `Path.read_text` also turns it into LF when files are read from disk, which is why #314 counted the three cases as closed in practice.
- The 26th case, `W-DEP-05` (`ReloadProgress` marks every lesson done): **out-of-scope stated limit**. It is game and teaching-progress logic, not checker scope. It is accepted by design and kept as an in-suite accepted-limit probe.

### Loop-1 results

All gates ran on the committed tree at `e29e3e1b` with Python 3.12.3. Raw files are in `$L1/final/`. Python ran with `PYTHONDONTWRITEBYTECODE=1 PYTHONPYCACHEPREFIX=$L1/pycache` and `TMPDIR=$L1/tmp`. The lane was clean before and after.

| Gate | Result | Evidence (`$L1/final/`) |
|---|---|---|
| `python3 -m py_compile` on the three scripts | exit 0, no output | `01-py_compile.out` |
| Suite `python3 -B …/test_objective_cta_source_check.py` | **exit 0**, stderr empty. Live source accepted (10 rows). original 20/20, F1 14/14, F2-F3 21/21, #305 R-IDs 21/21, X-F1 35/35, X-F2 46/46, #309 RR-1 28/28, RR-2 15/15, RR-3 14/14, N-4 5/5 (all as before); **#314 reviewer S-1 10/10, S-2 14/14 (11 + 3 lone CR), S-3 4/4, L-06 2/2; loop-1 S-1 16/16, S-2 5/5, S-3 2/2, L-06 3/3**. Benign 32/32 and loop-1 benign 32/32 (21 reviewer + 11 new). Probes: 9 accepted limits and 9 over-rejections as documented, 0 changed. Historical baseline rejected ("unnamed primary", 3 rows). **`rejected=275/275 false_greens=0 wrong_cause=0`** | `02-suite.*` |
| #305 corpus vs `1585ca20` and the candidate (the #309 script, unchanged) | Reproduction exact (`1585ca20` accepts X-F1 17 + X-F2 35 = 52). Candidate **102/102 rejected**, 15/15 benign accepted | `03-corpus305.*` |
| #309 evidence probes (the #309 script, unchanged) | 16/16 attacks rejected, 4/4 over-rejections rejected, 5 of 6 limits accepted. The one change is **L-06, now rejected (closed)**, as intended | `04-probes309.*` |
| Candidate `check_baseline.py --rotation-state post-fix --instruction-state post-fix`, cwd pinned to SDK 8.0.133 | **`PASS=10 FAIL=0 NOT-TESTABLE=2`, exit 0**, stderr empty, six SnapFar PASS rows. sha256 `47fc3d40b1352085acf8fd57a36b6a431c1011481c5af80388b9e37ea8ee343d` (changed from `1facb1ff…` only by the reworded 08 PASS note) | `05-candidate.*` |
| Same, traced (`COREHOST_TRACE`, `strace -f -e execve,openat`, `UseSharedCompilation=false`, `MSBUILDDISABLENODEREUSE=1`) | Byte-identical stdout, exit 0. hostfxr: `Found global.json [$E/sdk8/global.json]`, `version = '8.0.133', rollForward = 'disable'`, `Using .NET SDK dll=[/usr/lib/dotnet/sdk/8.0.133/dotnet.dll]`. **281 successful opens under `sdk/8.0.133`, 0 under `sdk/10.0.114`**; the C# compiler (`Microsoft.CodeAnalysis.CSharp.dll`) is opened from `sdk/8.0.133` | `06-*`, `10-sdk-summary.txt` |
| Same from a cwd without `global.json` (control) | `dotnet --version` **10.0.114**; `version = 'latest'` → `sdk/10.0.114`; **504 opens under `sdk/10.0.114`, 0 under `sdk/8.0.133`**; byte-identical stdout, exit 0. The trace discriminates | `07-*`, `10-sdk-summary.txt` |
| Historical `72b4efbd` with the three amended scripts copied in, untracked | **`PASS=6 FAIL=4 NOT-TESTABLE=2`, exit 1**, `tracked-production-vs-baseline: MATCH`, stderr empty. Differs from `baseline-output.txt` at line 48 only (test-08 text, as before). **Byte-identical to the `b4d47d3d` historical output** (`cmp` exit 0) | `08-*` |
| `baseline-output.txt` | Blob `db6a8b850dce7efadaac6fe1b87b1dbba469428f` at `b4d47d3d`, at `e29e3e1b` and in the tree; sha256 `513cddd9…c1`; never regenerated | `09-protected-audit.txt` |
| #314 corpus, 176 cases | See below | `11-reviewer314.*` |
| Every loop-1 case under the `b4d47d3d` scanner and the repaired one | Every mutation rejected for its `intended` cause at HEAD. At `b4d47d3d`, all 30 transcribed reviewer mutations and **25 of the 26 new loop-1 mutations were accepted** (new false greens), and `L1-S2-06` was already rejected. All 32 benign accepted by both. The 9 limits are accepted by both; the 9 over-rejections are rejected at HEAD, 4 of them new in this loop (O-05, O-06, O-08, O-09) | `12-compare-loop1.*` |
| Live source | 198/198 `Assets/**/*.cs` lex; 29 companions, unchanged | `12-lex-live.stdout` |
| Lane | Clean before and after the gates (`00-lane-status.txt`, `99-lane-status-end.txt`: 0 lines) | |

**#314 reviewer corpus (176 cases, the reviewer's own case definitions, run with my driver `run_reviewer314.py`).** The reviewer's `harness` module is not executed: a shim with the same path constants is used, and edits apply exactly as in its `apply_case`.
- All **112** cases the reviewer's run had rejected at `b4d47d3d` are still rejected.
- Of the **29** in-memory false greens (26 end-to-end plus the 3 lone-CR cases), **28** are now rejected with the intended cause: S-1 "RR-2 related state", S-2 "(N-4)", S-3 "RR-3 routing input". The only one still accepted is `W-DEP-05` (out of scope).
- 21/21 benign cases are accepted. 11 of the 14 limit probes are accepted; `V-L06-1` and `V-L06-2` are now rejected (L-06 closed), and `Y-RR3-11` is rejected as before. The 12 exploratory cases are unchanged (11 rejected, `Z-RR4-13` accepted).
- Verdicts are identical between a run during development and the final run (`$L1/final/11-reviewer314.stdout`, `$L1/reviewer314-final/results.json`).

**Transcription.** The 54 reviewer cases copied into the suite (25 survivors, 3 lone-CR, 2 L-06, 21 benign, 3 limits) are proved identical to the reviewer's originals: same id, group, description, edit list and transform (`$L1/final/12-transcription.stdout`: 54/54 identical).

**End-to-end through the real `check_baseline.py`.** Run in a sparse shared clone at `e29e3e1b` (`$L1/e2e-clone`), from the SDK 8.0.133-pinned cwd, with each case's files overlaid and then removed exactly:

**Tally: 40 runs.** The no-edit control and `W-DEP-05` (documented out-of-scope limit) give `PASS=10 FAIL=0 NOT-TESTABLE=2`, exit 0. All other 38 give `PASS=9 FAIL=1 NOT-TESTABLE=2`, exit 1, failing only test 08 for the stated cause: the 25 in-scope #314 survivors, its 3 lone-CR cases and 10 new loop-1 mutations. stderr is empty everywhere, and the clone was verified clean after each run. Note that `Y-N-12` is rejected because `collect_companions` now reads the new partial file from disk, which proves the Cf-stripped companion match in the real pipeline (`$L1/logs/13-e2e.txt`, `$L1/e2e-out/`).

| Case | Origin | `check_baseline.py` exit | Tally | Test-08 cause (abridged) |
|---|---|---|---|---|
| NOEDIT-control | control | 0 | `PASS=10 FAIL=0 NOT-TESTABLE=2` | PASS (accepted) |
| Z-RR2-T-help | #314 reviewer | 1 | `PASS=9 FAIL=1 NOT-TESTABLE=2` | ScenarioOneUiRoot.cs:696: 'help' is written outside its reviewed write site (tuple or deconstruction target): tuple, ref/out, qualified, compound, mov |
| Z-RR2-T-screen | #314 reviewer | 1 | `PASS=9 FAIL=1 NOT-TESTABLE=2` | ScenarioOneUiRoot.cs:696: 'screen' is written outside its reviewed write site (tuple or deconstruction target): tuple, ref/out, qualified, compound, m |
| Z-RR2-T-rackLayout | #314 reviewer | 1 | `PASS=9 FAIL=1 NOT-TESTABLE=2` | ScenarioOneUiRoot.cs:696: 'rackLayout' is written outside its reviewed write site (tuple or deconstruction target): tuple, ref/out, qualified, compoun |
| Z-RR2-T-map | #314 reviewer | 1 | `PASS=9 FAIL=1 NOT-TESTABLE=2` | ScenarioOneUiRoot.cs:696: 'map' is written outside its reviewed write site (tuple or deconstruction target): tuple, ref/out, qualified, compound, move |
| Z-RR2-T-workPlan | #314 reviewer | 1 | `PASS=9 FAIL=1 NOT-TESTABLE=2` | ScenarioOneUiRoot.cs:696: 'workPlan' is written outside its reviewed write site (tuple or deconstruction target): tuple, ref/out, qualified, compound, |
| Z-RR2-T-review | #314 reviewer | 1 | `PASS=9 FAIL=1 NOT-TESTABLE=2` | ScenarioOneUiRoot.cs:696: 'review' is written outside its reviewed write site (tuple or deconstruction target): tuple, ref/out, qualified, compound, m |
| U-RR2-T-manager | #314 reviewer | 1 | `PASS=9 FAIL=1 NOT-TESTABLE=2` | ScenarioOneUiRoot.cs:696: 'manager' is written outside its reviewed write site (tuple or deconstruction target): tuple, ref/out, qualified, compound,  |
| Z-RR2-C-screen | #314 reviewer | 1 | `PASS=9 FAIL=1 NOT-TESTABLE=2` | ScenarioOneUiRoot.cs:681: pinned method TryShowReviewYear (RR-2 related state 'screen') changed (expected 'screen', found 'CloseHelp'); a reviewed met |
| Z-RR2-C-help | #314 reviewer | 1 | `PASS=9 FAIL=1 NOT-TESTABLE=2` | ScenarioOneUiRoot.cs:1: RR-2 related state 'help': expected exactly one reviewed occurrence of `help = new MenuHelpView(this);` (missing, changed or d |
| Y-RR2-08 | #314 reviewer | 1 | `PASS=9 FAIL=1 NOT-TESTABLE=2` | ScenarioOneUiRoot.cs:681: pinned method TryShowReviewYear (RR-2 related state 'screen') changed (expected 'Review', found 'None'); a reviewed method m |
| Y-N-10 | #314 reviewer | 1 | `PASS=9 FAIL=1 NOT-TESTABLE=2` | LearningObjectivesView.Sequence.cs:9: U+2028 is a line break for the C# compiler but not for this lexer (it can end a // comment or a preprocessor lin |
| Y-N-11 | #314 reviewer | 1 | `PASS=9 FAIL=1 NOT-TESTABLE=2` | LearningObjectivesView.cs:82: character U+200D (category Cf) in code: the C# compiler reads it differently from this lexer (a format character inside  |
| Y-N-12 | #314 reviewer | 1 | `PASS=9 FAIL=1 NOT-TESTABLE=2` | LearningObjectivesView.Cf.cs:1: character U+200D (category Cf) in code: the C# compiler reads it differently from this lexer (a format character insid |
| Y-N-13 | #314 reviewer | 1 | `PASS=9 FAIL=1 NOT-TESTABLE=2` | LearningObjectivesView.Sequence.cs:9: U+0085 is a line break for the C# compiler but not for this lexer (it can end a // comment or a preprocessor lin |
| Y-N-14 | #314 reviewer | 1 | `PASS=9 FAIL=1 NOT-TESTABLE=2` | ScenarioOneUiRoot.cs:696: character U+200D (category Cf) in code: the C# compiler reads it differently from this lexer (a format character inside an i |
| Y-N-15 | #314 reviewer | 1 | `PASS=9 FAIL=1 NOT-TESTABLE=2` | ScenarioOneUiRoot.cs:696: character U+200D (category Cf) in code: the C# compiler reads it differently from this lexer (a format character inside an i |
| Z-N-PS | #314 reviewer | 1 | `PASS=9 FAIL=1 NOT-TESTABLE=2` | LearningObjectivesView.Sequence.cs:9: U+2029 is a line break for the C# compiler but not for this lexer (it can end a // comment or a preprocessor lin |
| Z-N-CF-buttons | #314 reviewer | 1 | `PASS=9 FAIL=1 NOT-TESTABLE=2` | LearningObjectivesView.cs:83: character U+200D (category Cf) in code: the C# compiler reads it differently from this lexer (a format character inside  |
| Z-N-CF-secondprimary | #314 reviewer | 1 | `PASS=9 FAIL=1 NOT-TESTABLE=2` | LearningObjectivesView.cs:192: character U+200D (category Cf) in code: the C# compiler reads it differently from this lexer (a format character inside |
| Z-N-CF-allowlist-clear | #314 reviewer | 1 | `PASS=9 FAIL=1 NOT-TESTABLE=2` | ScenarioOneUiRoot.cs:696: character U+200B (category Cf) in code: the C# compiler reads it differently from this lexer (a format character inside an i |
| Z-N-SHY | #314 reviewer | 1 | `PASS=9 FAIL=1 NOT-TESTABLE=2` | LearningObjectivesView.cs:82: character U+00AD (category Cf) in code: the C# compiler reads it differently from this lexer (a format character inside  |
| W-DEP-01 | #314 reviewer | 1 | `PASS=9 FAIL=1 NOT-TESTABLE=2` | ScenarioOneUiRoot.cs:1: RR-3 routing input 'NeedsAnnualReview': expected exactly one reviewed occurrence of `public bool NeedsAnnualReview => manager  |
| W-DEP-02 | #314 reviewer | 1 | `PASS=9 FAIL=1 NOT-TESTABLE=2` | LearningObjectivesView.cs:1: RR-3 routing input 'TeachingRacks': expected exactly one reviewed occurrence of `public bool TeachingRacks => CurrentTopi |
| W-DEP-03 | #314 reviewer | 1 | `PASS=9 FAIL=1 NOT-TESTABLE=2` | LearningObjectivesView.cs:1: RR-3 routing input 'CurrentTopicIndex': expected exactly one reviewed occurrence of `public int CurrentTopicIndex => Scen |
| W-DEP-04 | #314 reviewer | 1 | `PASS=9 FAIL=1 NOT-TESTABLE=2` | ScenarioOneUiRoot.cs:1: RR-3 routing input 'NeedsAnnualReview': expected exactly one reviewed occurrence of `public bool NeedsAnnualReview => manager  |
| W-DEP-05 | #314 reviewer | 0 | `PASS=10 FAIL=0 NOT-TESTABLE=2` | PASS (accepted) |
| Y-N-09 | #314 reviewer | 1 | `PASS=9 FAIL=1 NOT-TESTABLE=2` | LearningObjectivesView.Sequence.cs:10: the Objectives Root may be used only by its declaration and the constructor (RR-1) |
| Z-N-CR-region | #314 reviewer | 1 | `PASS=9 FAIL=1 NOT-TESTABLE=2` | LearningObjectivesView.Sequence.cs:10: the Objectives Root may be used only by its declaration and the constructor (RR-1) |
| Z-N-CR-root | #314 reviewer | 1 | `PASS=9 FAIL=1 NOT-TESTABLE=2` | 'guidanceCandidate' must be written only by RefreshNextActionGuidance/ApplyNextActionGuidance/ClearNextAction |
| L1-S1-01 | loop 1 (new) | 1 | `PASS=9 FAIL=1 NOT-TESTABLE=2` | ScenarioOneUiRoot.cs:696: 'help' is written outside its reviewed write site (assignment): tuple, ref/out, qualified, compound, moved or compensated wr |
| L1-S1-08 | loop 1 (new) | 1 | `PASS=9 FAIL=1 NOT-TESTABLE=2` | ScenarioOneUiRoot.cs:153: RR-2 related state: Start may not return, throw, goto or yield before its last reviewed related-state write |
| L1-S1-11 | loop 1 (new) | 1 | `PASS=9 FAIL=1 NOT-TESTABLE=2` | ScenarioOneUiRoot.cs:126: RR-2 related state: `learning = new LearningObjectivesView(this);` must be a top-level statement of Start (not moved, wrappe |
| L1-S1-14 | loop 1 (new) | 1 | `PASS=9 FAIL=1 NOT-TESTABLE=2` | ScenarioOneUiRoot.cs:697: 'screen' is used as a member receiver or passed whole as an argument; a ref/in extension or parameter could write it (RR-2 r |
| L1-S1-16 | loop 1 (new) | 1 | `PASS=9 FAIL=1 NOT-TESTABLE=2` | \bprivate\s+void\s+Start\s*\(\s*\): conditional compilation (#if) in a checked method or declaration is unsupported |
| L1-S2-01 | loop 1 (new) | 1 | `PASS=9 FAIL=1 NOT-TESTABLE=2` | ScenarioOneUiRoot.cs:128: the lexer cannot follow this source ('#' that does not start a plain preprocessor line (other whitespace before it, or not a |
| L1-S2-04 | loop 1 (new) | 1 | `PASS=9 FAIL=1 NOT-TESTABLE=2` | LearningObjectivesView.Sequence.cs:10: a multi-line literal or comment spans a preprocessor line; fail closed (N-4) |
| L1-S2-05 | loop 1 (new) | 1 | `PASS=9 FAIL=1 NOT-TESTABLE=2` | LearningObjectivesView.cs:82: character U+FEFF (category Cf) in code: the C# compiler reads it differently from this lexer (a format character inside  |
| L1-S3-01 | loop 1 (new) | 1 | `PASS=9 FAIL=1 NOT-TESTABLE=2` | ScenarioOneUiRoot.cs:51: RR-3 routing input 'NeedsAnnualReview': reviewed text `public bool NeedsAnnualReview => manager != null && manager.AnnualRepo |
| L1-L06-03 | loop 1 (new) | 1 | `PASS=9 FAIL=1 NOT-TESTABLE=2` | StandMapView.cs:27: another class reaches the Objectives view other than by a member access or null test on .Learning (alias or pass-on, L-06) (RR-1) |

### Loop-1 Roslyn reality check

The same tool as above (`$E/sdk8/roslyn-tool-309/`, C# 9, the same 320 Unity 6000.6 and project DLLs, Player-like defines without `UNITY_EDITOR`). The unmodified tree has 189 files and 0 errors.
- **All 106 loop-1 case directories compile with 0 errors**: 30 transcribed reviewer mutations, 26 new loop-1 mutations, 21 + 11 benign, 9 limits and 9 over-rejections (`$L1/final/13-roslyn-loop1.txt`). One draft case, `#i<U+200D>f` (a format character inside a directive keyword), failed with CS1024, so it is **not** a valid mutant; it was dropped and is not counted.
- **Differential probes** (`$L1/logs/06-roslyn-diffprobes.txt`) put a type error where the directive decides whether code is active. Form-feed-indented `#if`, a no-break space after `#`, and vertical-tab-indented `#if` give 0 errors, so Roslyn honours them and the code is inactive, like the plain `#if` control. The NBSP-indented `#endif` inside an inactive verbatim-string opener gives CS0029, so the "hidden" code is **active**, like the active control.

### Loop-1 stated limits and over-rejections

Accepted-limit probes (compilable defective edits the checker accepts; in the suite, reported, not counted):

| Probe | Not covered | Why |
|---|---|---|
| L-01 | A class outside the companion set disables a header button by a constructed name | Classes other than the view, its partials, the root, UiKit and companions are not analysed; string construction is unbounded |
| L-06 residual | A view-class member returning the view or one of its elements, then tree traversal from a companion | Same class as L-01: element-tree reachability. The alias and pass-on shapes are closed |
| L-02 | Unpinned root code disables every button of its own document (no class change) | The root queries its own document legitimately |
| L-03, L-07, **L-08** | An infinite loop or a throwing expression before the Objectives caller, or (new) a throwing expression in `Start` before a reviewed related-state write | Non-termination and exceptions are not analysed. The order of the reviewed writes among themselves in `Start` is not pinned |
| L-04 | Reflection by string name | Strings are not resolved to members |
| `W-DEP-05` (#314 26th case) | Game and teaching progress (`ReloadProgress`, `done`, `TeachingFacts`, `ScenarioOneTeachingSequence`, `ScenarioOneManager` state) that the pinned routing inputs read | Game logic, out of checker scope |
| `Y-RR3-17`, `Y-RR3-18` | The unpinned entry points `ShowObjectives` and `CloseAll` routing elsewhere | Unpinned UI logic (#305 N-2 class), now named in the README |
| — | Members of other classes that the chain reads (`StandMapView`, `WorkPlanView`, `RackLayoutTracker`); object lifetime and component state at run time (for example destroying the manager); USS; UiKit members other than the six pinned | Not read or not analysed |

Over-rejection probes (benign edits that fail closed; in the suite): O-01 to O-04 as before. New in this loop:
- O-05: any `#if` inside `Start`, which now holds reviewed writes.
- O-06: U+2028 even inside a verbatim string.
- O-07: a local named like a related-state field (pre-existing through the count rule).
- O-08: `null != ui.Learning` in a companion.
- O-09: `screen` passed whole to any method, even `Debug.Log(screen)`.
- Not probed but documented: any `return`, `throw`, `goto` or `yield` in `Start` before the last reviewed write, even inside a lambda; a form feed, vertical tab or other control or format character in code.

**Not proved.** That the stored pinned text and the reviewed write sites are the *right* behaviour (they are the reviewed `1585ca20` source). That the guard set is complete: the S-1 to S-3 guards were written after reading #314's cases; the 26 extra loop-1 attacks are mine, and nobody independent has attacked this head yet. That the lexer matches Roslyn on every input: it is now aligned on the line-break, format-character and directive-whitespace classes, verified by the cases and probes above and on the 198 live files, not on every Unicode category. Unity Editor compile, native input, rendering, Help-close timing, saves, Legacy40/Reference, Windows/Player and the exact integrated candidate: **NOT RUN**.

### Loop-1 correction notes to the sections above

- "Implementation notes", RR-2 bullet: "`help`, `manager`, `learning`, `map`, `workPlan`, `review`, `rackLayout` and `screen` keep their reviewed write counts". **That count-only rule was insufficient (#314 S-1).** These fields now also keep pinned declarations and write sites (above).
- "Implementation notes", N-4: "It fails closed on raw strings, `\u` identifiers …" and "It lexes all 198 `.cs` files". Both were true, but the lexer did **not** handle format characters in identifiers, the Unicode line separators or exotic directive whitespace (#314 S-2 and `L1-S2-04`). It does now.
- "Limits" table, L-06 row ("Aliasing in other classes is not tracked"), and the L-06 row of "Evidence-only probes" (ACCEPTED): **superseded**. The alias and pass-on shapes are closed; only the residual above remains.
- "Limits" intro, "Each L-probe … is recorded in `04-probes.stdout` and is not counted as a test" and the README's "each has an accepted probe in the #309 handoff": the probes now also live **in the suite** (reported, not counted). The README says so.
- "Exact diff", `check_baseline.py` "+4/−4, test-08 only": this loop changes one more test-08 PASS-note line (+2/−1). The candidate stdout sha256 therefore changes from `1facb1ff…` to ``47fc3d40…``.
- The README sentence "the map selected as target exactly once, only for an unconfirmed Rack V2 Objective 1" was overstated (#314 S-3) and has been reworded.
- "Results", suite row (`rejected=219/219`, benign 32/32): at `e29e3e1b` the suite has `rejected=275/275`, benign 32/32 + 32/32, and 18 probes.

### Loop-1 commands

All scripts are in `$L1/scripts/`. `$L1/SHA256SUMS.txt` covers the scripts and logs.

| # | Command | What |
|---|---|---|
| 1 | `bash $L1/scripts/final_gates_loop1.sh <label>` | py_compile, suite, #305 corpus, #309 probes, pinned/traced/control candidate runs, historical run, audit, SDK summary, #314 corpus, b4d47d3d-vs-HEAD comparison, transcription identity, live lex, export and Roslyn compile |
| 2 | `python3 -I -B scripts/run_reviewer314.py <lane> <out>` (`$L1`) | The #314 reviewer's 176 cases against the lane checker |
| 3 | `python3 -B scripts/compare_loop1.py <lane> scripts/scanner_b4d47d3d.py <out.json>` | Every loop-1 case under the `b4d47d3d` scanner and the repaired one |
| 4 | `python3 -B scripts/verify_transcription.py <lane>` | Transcribed cases equal the reviewer's originals |
| 5 | `python3 -I -B scripts/diff_probes.py <lane> <out>`, then RoslynCheck | Differential directive probes |
| 6 | `bash scripts/e2e_loop1.sh <id> <case-dir>` | One end-to-end `check_baseline.py` run on the e2e clone |

### Loop-1 final state

The lane is clean after the commits. The branch was published fast-forward only from `b4d47d3d`, with no force-push or rebase. Build servers that my own runs started were stopped. The e2e and historical clones are throwaway clones in the evidence folder and can be deleted after the Manager rules. The Roslyn references read Unity and package DLLs read-only, as before, including `/media/jer/ZX20/CCF-MainIntegration-20261104/Library/ScriptAssemblies`. No other lane's file, process or lock was changed.
