# #320 — CTA checker residual N-1/N-2 two-guard closure: author result

> **UNQUALIFIED SOURCE PUBLICATION, pending independent review [#321](https://github.com/jermur788/CCF/issues/321).** This is the author's self-report. It is **not** a review and **not** Manager acceptance. Independent review #321 and then Manager #200 decide. Everything below is static/lexical source checking plus an isolated plain-.NET SnapFar run. **No Unity, native, render, input or save result is claimed.**

| | |
|---|---|
| Claim | [#320](https://github.com/jermur788/CCF/issues/320), under the Manager #200 binding packet `Docs/Handoffs/ScenarioOne-Morning-Closure-20261107/MANAGER-CTA-309-TWO-GUARD-CLOSURE-PACKET.md` (on `main`), D3 option (b). This is a new, separately approved repair budget, not the spent #309 overnight loop |
| Worker | Claude Opus 5.5 (model id `claude-opus-5-5`, as stated by the session) running through T3 Code on the Claude Code harness. Dispatched by the Mimo orchestrator (thread 1872ce0f). Provider: Anthropic. Account/instance: **UNKNOWN** (not observable from inside the session). Reasoning effort: **UNKNOWN** (not observable) |
| Lane | `/media/jer/Files/CCF/Operations/CCF-FeedbackCTARepair-20261106`, branch `verify/scenario-one-feedback-cta-anchor-20261106` |
| Start head | `753143e62a5cf1d7436874e731766c4893a4c7c7` == origin. Preflight: tracked, untracked, ignored and skip-worktree all clean |
| **Code commit (tested)** | **`8ff6352b34a0fcbff7d0606b1d9ab28c6dd67a4c`**: checker, tests, README and the check-08 note |
| Docs commit | the commit that adds this file (child of `8ff6352b`; see the branch log). It changes only this file |
| Evidence | `/media/jer/Files/CCF/Verification/CTA-320-TwoGuard-20261107/` (`SHA256SUMS.txt`: 716 files, sha256 `6ff91cefb5ec32e7740f7cbe138e188fafbb7b14eadb0a4f1d6011e3fbdd5bcd`) |

## Changed files (code commit `8ff6352b`, vs `753143e6`)

| Path | Change |
|---|---|
| `Tools/Verification/OvernightPlaytestFeedback/objective_cta_source_check.py` | +394/−41 lines: the N-1 hole guard, the N-2 owning-class walk and shadow guard, and owner binding of every existing pin |
| `Tools/Verification/OvernightPlaytestFeedback/test_objective_cta_source_check.py` | +674 lines: the #320 groups. **No existing case, group or assertion was removed or changed** |
| `Tools/Verification/OvernightPlaytestFeedback/README.md` | Claims revised (see [Docs truthfulness](#docs-truthfulness)) |
| `Tools/Verification/OvernightPlaytestFeedback/check_baseline.py` | Test-08 PASS note text only (one `report("PASS", "08 …")` string). Checks 01–07 and 09 and the FAIL path are untouched |
| `Docs/Handoffs/ScenarioOne-Morning-Closure-20261107/T3-CTA-309-TWO-GUARD-CLOSURE-RESULT.md` | New (docs commit) |

Not touched: `baseline-output.txt` (blob `db6a8b850dce7efadaac6fe1b87b1dbba469428f` at both `753143e6` and `8ff6352b`; sha256 `513cddd9babddaede82212f8e4891ecfcf9823c13a69474a3665f890a7f05dc1`), and all of the following: `Assets/`, `Packages/`, scenes, saves, other checkers, `main`, PR #300, `active-tasks.md`.

## Contract items

| # | Item | Result |
|---|---|---|
| 1 | N-1: reject the 47 interpolation-hole writes, each for its intended reason, end to end | **PASS (author).** 47/47 rejected in the suite with `… inside an interpolation hole (N-1)`. 47/47 end to end through the real `check_baseline.py`: exit 1, `9/1/2`, the only FAIL is 08 and it names that cause. 0 benign regressions. 21 further author variants also rejected |
| 2 | N-2: reject the 10 decoy cases, owner-bound, fail closed when ambiguous | **PASS (author).** 10/10 rejected with `… (N-2 owning class)`, in the suite and end to end. 12 further author variants (interface, namespace, record, `#if`-brace trick, modifier, name shadowing) also rejected |
| 3 | Truthful README, report and test-08 wording | **PASS (author)**; see [Docs truthfulness](#docs-truthfulness) |
| 4 | In-repo regression tests and all controls, with real exits | **PASS (author)**: suite `rejected=884/884 false_greens=0 wrong_cause=0`, benign 112/112, 52 probes unchanged, exit 0. Every gate is below. Nothing was NOT RUN |
| 5 | Independence from the reviewer scratch | **PASS (author)**; see [Independence](#independence-from-the-reviewer-scratch) |

## What the two guards do

**N-1 (interpolation holes).** The lexer already recorded each hole's `(start, close, inner spans)`, but `_tokens` keeps a whole interpolated string as one token. That is right for the pinned text comparison, but it made the #314 S-1 write-site rule blind to hole code. `_deep_tokens` now opens every string. Each hole, at any nesting, becomes ordinary tokens between its own `{`/`}`. Format clauses and literal text are dropped. `_guard_interpolation_holes` runs the **same** per-token rule as statements (`_check_state_token`, factored out of `_guard_related_state` unchanged): any write of the eight related-state names, any receiver other than `this`, and the `screen` member-receiver/whole-argument rule. It covers every hole in `ScenarioOneUiRoot.cs` outside the pinned bodies. It runs before the write-count rule, so the error names the real cause. Fail closed: a `#` token in a hole, or a format clause beginning `::`. (Roslyn treats `{global::X}` as `global` plus a format clause and gives CS0103, so that shape is not valid C# anyway; probe O-15.) Statement-level behaviour is unchanged.

**N-2 (owning class).** `_structure` is a bounded walk over the comment-free tokens. It records, for every token, the stack of open braces (namespace, type `class`/`struct`/`interface`/`enum`/`record`, or block) and every type declared in the file. It fails closed on unbalanced braces, on `#if`/`#elif`/`#else`/`#endif` branches that change the brace depth, and on a file-scoped namespace. Comments are not tokens and a directive line is one opaque token, so neither can stand in for a declaration. The guards built on it:
- `_bind_to_owner`: every reviewed declaration must be a **direct member** of the top-level owning class body, with no attribute or extra modifier before it. That covers the related-state fields, the guarded fields, `GuidanceButtonNames`, `Learning`, `NeedsAnnualReview`, `CurrentTopicIndex`, `TeachingRacks`, the view `Root` and `UiScreen`. The other reviewed statements must lie in the class and not in a nested type. Every pinned method, plus the Objectives constructor, `Refresh(bool)` and `Start`, must be a direct member of its class.
- `_guard_shadowing`: in the root and in every part of the view, it rejects:
  - a nested or added type named like a simple name the reviewed text looks up (`Time`, `StandaloneSessionMenu`, `ScenarioOneTeachingSequence`, …);
  - a class member named like an outside type the reviewed text reads (`private FakeTime Time`);
  - a `using` alias with such a name.
- The root header `public sealed class ScenarioOneUiRoot : MonoBehaviour` is pinned, and no part of `LearningObjectivesView` may declare a base type, because base-class members would hide those names too.

Unrelated nested and additional types stay accepted (NB320-01/02/03/04/12/13, A9-D12).

## Test matrix (in-repo suite at `8ff6352b`, `final/02-suite.stdout`, exit 0, stderr empty)

| Group | Result |
|---|---|
| Original 20, #304 F1 14, F2/F3 21 | 20/20, 14/14, 21/21 |
| #305 corpus: 21 R-IDs, 35 X-F1, 46 X-F2 (the 81 includes the known 50 false greens) | 21/21, 35/35, 46/46 |
| #309 RR-1 28, RR-2 15, RR-3 14, N-4 5 | all rejected |
| #314 S-1 10, S-2 14, S-3 4, L-06 2; #309 loop-1 S-1 16, S-2 5, S-3 2, L-06 3 | all rejected |
| **Existing total** | **275/275** (unchanged) |
| #314 reviewer, all 11 rounds (654 cases; 576 counted mutations) | rounds 1–6: 94/94, 27/27, 4/4, 0/0, 3/3, 0/0; round 7: 347/347; rounds 8–11: 17/17, 5/5, **56/56, 23/23** |
| #320 N-1 author variants | 21/21 |
| #320 N-2 author variants | 12/12 |
| **All mutations** | **884/884, false_greens=0, wrong_cause=0** (a scanner crash never counts as a rejection) |
| Benign: original/#305/#309 32, loop-1 32, #320 48 (35 reviewer + 13 author) | **32/32, 32/32, 48/48: zero benign regressions** |
| Probes, reported and not counted: loop-1 9 limits + 9 over-rejections; #320 52 (43 reviewer lim/unk/over, O-10..O-16, L-09, L-10) | all keep their documented outcome (`changed=0`) |
| Historical old baseline `72b4efbd` | rejected (expected FAIL), 3 evidence rows |

The "35 reviewer benign" are the 35 benign cases the #314 reviewer's rounds accepted at `753143e6`. The "56 author benign" are the suite's 32 + 24 existing benign groups that the #314 report counts. All of them are still accepted. Four reviewer "ben" cases (`A8-L06-isnull`, `-isnotnull`, `-localalias-read`, `-coalesce`) were **already rejected at `753143e6`** (L-06 over-rejections). They are carried as documented over-rejection probes, not counted as benign, and their outcome is unchanged.

**Transcription fidelity** (`post/fidelity.txt`): 654/654 in-repo reviewer cases are identical to the originals in `my_mutations*.py` (id, why, edits, transform), and each source file's sha256 is recorded in the suite. There are 5 documented reclassifications:
- `W-DEP-05`: rej → lim. It is an out-of-scope limit and stays accepted.
- The four L-06 cases: ben → over.

**Pre-change comparison** (`pre/rounds.stdout` vs `post/rounds-v1.stdout`): at `753143e6` exactly the 57 N-1/N-2 cases plus `W-DEP-05` were accepted false greens. After the change, only `W-DEP-05` stays accepted. The only other verdict change across the 654 is `A7-x-teachinghijack` (reviewer "lim", accepted → now rejected by the N-2 shadow guard). Note that this case is not valid C# (24 Roslyn errors in the reviewer's own compile).

## End to end (real `check_baseline.py`, pinned SDK 8.0.133 cwd)

Method: a throwaway sparse `--shared` clone at `8ff6352b` (`e2e-clone`). For each case its files were overlaid, the real `check_baseline.py --rotation-state post-fix --instruction-state post-fix` was run from `sdk8/cwd/deeper` with `UseSharedCompilation=false`, and the clone was restored. The clone was clean before every case and at the end. Raw stdout/stderr/exit per case are in `e2e-out/`; the tally is `e2e-tally.txt`.

| Set | Cases | Outcome |
|---|---|---|
| #314 N-1 (A10 ×28, A11 ×19) | 47 | all exit 1, `PASS=9 FAIL=1 NOT-TESTABLE=2`, the only FAIL is 08 and it names `inside an interpolation hole (N-1)` |
| #314 N-2 (A8-D1/D2/D3/D4/D5/D5b, A9-D7/D8/D9/D10) | 10 | all exit 1, `9/1/2`, the 08 line names `(N-2 owning class)` |
| #320 author N-1 / N-2 | 21 / 12 | all exit 1, `9/1/2`, intended cause in the 08 line |
| #320 over-rejection probes O-10..O-16 | 7 | exit 1 as documented |
| #314 reviewer benign (35 minus `Y-B-17`) | 34 | all exit 0, `10/0/2` |
| #320 author benign / limit probes L-09, L-10 | 13 / 2 | all exit 0, `10/0/2` |
| **Total** | **146** | **146/146 as expected, stderr empty in every run** |

`Y-B-17` (CRLF in every file) is checked in memory only. `Path.read_text` turns CRLF back into LF, so an on-disk run cannot test it.

## Gates (`final/`, run by `scripts/gates.sh` at lane HEAD `8ff6352b`)

| Gate | Result |
|---|---|
| `python3 -m py_compile` of all three Python files (cache redirected to the evidence dir) | exit 0 |
| In-repo suite | exit 0 (above) |
| Candidate `check_baseline.py` (post-fix/post-fix) from the SDK-pinned cwd | **`PASS=10 FAIL=0 NOT-TESTABLE=2`, exit 0**, stderr empty |
| Traced candidate (`strace -f` execve/openat + `COREHOST_TRACE`) | Same stdout (sha256 `69fd54fa…4b36`). hostfxr: `Found global.json [.../sdk8/global.json]`, `Resolving SDKs with version = '8.0.133', rollForward = 'disable'`, `Using .NET SDK dll=[/usr/lib/dotnet/sdk/8.0.133/dotnet.dll]`. 281 successful opens under `sdk/8.0.133`, 0 under `sdk/10.0.114`; Roslyn opened from `sdk/8.0.133/Roslyn`. The six SnapFar cases: 6 PASS lines |
| Traced control (cwd without `global.json`) | Resolves `latest` → `sdk/10.0.114` (504 opens, 0 under 8.0.133), identical stdout. This proves the cwd, not the script, selects the SDK |
| SDK trace pointer | `final/06-strace.txt`, `final/06-corehost-trace.txt`, summary `final/10-sdk-summary.txt` |
| Historical old baseline `72b4efbd` + the three committed scripts (untracked copies in a throwaway clone), pre-fix/pre-fix | **`PASS=6 FAIL=4 NOT-TESTABLE=2`, exit 1**. The diff against `baseline-output.txt` is the single, already documented 08 diagnostic line (README) |
| `baseline-output.txt` | Unchanged: same blob at `753143e6` and `8ff6352b`, sha256 identical before and after the gates |
| Lane integrity | Status clean before and after; no skip-worktree or assume-unchanged entries; the five-file sha256 set is identical before and after the gates |
| Roslyn C# 9 (reviewer-built `RoslynCheck.dll`, real Unity 6000.6 reference DLLs, Player-like defines, SDK 8.0.133 pinned by cwd; a compiler check, **not** a Unity compile) | Baseline 189 files, 0 errors. All 55 author cases compile with 0 errors except O-15, which is invalid C# on purpose and documented (`roslyn/result.txt`) |

A VBCSCompiler server (pid 2457957) was left by my untraced gate-05 run. Its TMPDIR was in my evidence dir and its start time matched gate 05. I stopped that pid only; `dotnet build-server shutdown` was not used.

## Newly discovered false greens (at `753143e6`, now closed)

Found by my own variants (`post/author-old-vs-new.txt`). All are valid C# 9 (Roslyn 0 errors).
- **N-1 class, more shapes (12):** `((this.help) = null)`, the `@$` prefix order, multi-line verbatim holes, a statement lambda in a hole, a ref-`this` extension receiver `screen.ZTouch()` in a hole, an `in screen` argument, alignment after a tuple write, a verbatim hole with an empty nested literal `""` or `$""` (both lexers agree, so N-4 does not catch it), an expression-bodied property, comments inside a hole, and a `this.screen` tuple.
- **N-2 class, more decoy shapes (5):**
  - nested `interface` decoy;
  - decoy inside a `namespace` appended to the root file;
  - nested `record` decoy;
  - `#if ZNEVER` braces that make a nested decoy look like a class member;
  - `static` in front of the reviewed field.
- **New sub-class: name-shadowing hijack (4).** A nested type `StandaloneSessionMenu { static bool IsOpen => true; }` suspends guidance permanently (N2-06). A member `private ZFrame Time` (N2-07), a `using Time = ZTimeStub;` alias (N2-08), or a new base class with a nested `Time` (N2-09) breaks the pinned help-close-frame test. The pinned text is unchanged in all four; only what its names resolve to changes.

No further false-green class is known to me. That is not a proof that none exists.

## Docs truthfulness

- README, check 08: it says outright that the checker is static and lexical: nothing is compiled or run, and there is no Unity, native, compile or render result. "proves" became "checks".
- README: "Each field declaration is pinned" became "pinned as a direct member of the `ScenarioOneUiRoot` class body". "Any other write fails, whatever the count" is now limited to the listed lexical shapes, "in ordinary code and (since #320 N-1) inside interpolation holes", and reflection (L-04) is named as not covered.
- New README bullets for interpolation holes (N-1) and the owning class (N-2), each with its own limits.
- Known limits, newly stated:
  - unpinned root callers: any unpinned root method calling `SetScreen`, `ShowMap`, `ShowObjectives` or `CloseAll` (A7-x-update-setscreen, Y-RR3-17/18);
  - unpinned root members that the pinned routing reads: `ecology`, `nextBadge`, `RackPlanProblem`, `RackApprovalProblem` (probes L-09, L-10);
  - W-DEP-05 game-teaching progress;
  - L-01 including aliases and constructed names outside the companion set;
  - over-rejections O-10..O-16, plus the reviewer's four L-06 over-rejections.
- The #314 S-4 gap is closed: the reviewer's limit and exploratory probes are now in the repo as reported-but-not-counted probes.
- Check-08 PASS note: "Static, lexical source-structure check (nothing compiled or run)", "pinned inputs (bound to their owning class)", "unpinned root members and callers and the game/teaching state are not checked".

## Disclosed limitations (new with #320)

- **Lexical only.** Both new guards read tokens. They are not a C# parser or semantic model. The member-name scan takes the identifier before `(`, `=`, `=>`, `{`, `;` or a top-level `,` in a member header. Unusual declarations, such as a conversion operator named like a type, may be misread; they are over-rejected rather than under-rejected where I could tell.
- **Type detection** keys on `class`/`struct`/`interface`/`enum`/`record` (with `record` checked contextually), skipping a keyword after `:`, `,` or `.` (generic constraints). C# 10+ shapes such as file-scoped namespaces and `record struct` are outside C# 9. A file-scoped namespace fails closed.
- **The hole guard covers `ScenarioOneUiRoot.cs` only.** The related-state fields are private to the root, and other files cannot name them. Hole code in other files remains covered by the existing position-based rules (guarded names, header locals, `.Learning`), as before.
- **New over-rejections O-10..O-16:**
  - an attribute in front of a reviewed field;
  - `#if`/`#else` branches that each change brace depth;
  - a harmless nested type named like a local of the reviewed text;
  - `nameof(ScenarioOneUiRoot.map)` or `screen.ToString()` inside a hole (consistent with the statement rule);
  - `::` in a hole;
  - an empty base type on a view part.
- **Unicode:** no change. The N-4 rules still apply (Cf, foreign line breaks and other control characters fail closed). Python's and Roslyn's Unicode tables could still differ for new code points.
- **Not covered at all**, as before: Unity Editor or Player compile, runtime, rendering, input, focus, Help-close timing, save/load, USS, and the integrated candidate.

## Independence from the reviewer scratch

I read `fix_feasibility_l1c.py` and the 12* evidence as a description of the gap, and did not import or adapt that code. The differences are deliberate:

| | Reviewer scratch | This repair |
|---|---|---|
| N-1 | Applied `_state_write` to each hole's tokens in isolation | One token stream for the whole file, with holes expanded in place, so bracket and tuple context carries across nesting. The same receiver and `screen` rules apply. `#` and `::` fail closed |
| N-2 | "Exactly one type declaration per pinned file", which would ban legitimate unrelated classes | Binds each pin to its owning class body and brace depth. Adds the `#if` brace-depth rule, the name-shadow guard, the root-header pin and the view base-type rule. Unrelated types stay allowed |

The packet asked me not to ban legitimate unrelated classes, and the scratch's per-file rule would have. Every new guard was reviewed against its own author variants and benign controls, and checked for validity with Roslyn.

## Deviations and notes

- None of the contract's gates was skipped or reduced. Round 7 (347 generated cases) is in the repo as its generator, not as literal data, and its fidelity is proved like the other rounds.
- The in-repo suite now takes about 5 minutes (it was about 1 minute) because it runs all 654 reviewer cases.
- No Unity, lease or save was used. PR #300 was not touched. Nothing was merged. No Manager acceptance is claimed.

**Mandatory next:** independent re-review [#321](https://github.com/jermur788/CCF/issues/321) at the exact pushed head, then a Manager #200 ruling.
