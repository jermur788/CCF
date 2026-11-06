# #305 — Targeted independent re-review of the #304 CTA checker repair

**Reviewer verdict: NEEDS CHANGES.** This is a review result, not an approval. Manager #200 issues the final APPROVED / NEEDS CHANGES / REJECTED. No source was changed.

**Reviewed:** `verify/scenario-one-feedback-cta-anchor-20261106` at `1585ca20df2b921d10e79f9aac89f03202d9258e` (code `040e496a`), delta against `f294fe07`. Claim [#305](https://github.com/jermur788/CCF/issues/305), parent [#304](https://github.com/jermur788/CCF/issues/304).

## Summary

**Everything the packet named passes, and I reproduced it independently.** R01, R03, R04, R05, R07, R08, R09, R11, R12, R13, R14, R15, R20 and R21 are all rejected, each for the intended reason. The original 20 controls are unchanged and still rejected. Candidate tally is 10 PASS / 0 FAIL / 2 NOT-TESTABLE, exit 0. Old baseline is 6 / 4 / 2, exit 1, with `baseline-output.txt` byte-identical. The six real SnapFar cases pass under SDK 8.0.133, and a trace of the checker's own process tree proves that SDK was used. No check was weakened. The repair is a real improvement.

**It does not yet close the blocking F1 defect class.** I wrote 81 new broken-source mutations. The scanner rejected 29 and accepted 52. All 52 compile with Roslyn at C# 9 against the real Unity 6000.6 DLLs. Two of the 52 conform to the packet's wording, which leaves **50 false greens**. These two one-line edits are the clearest examples. Each one gets a full `check_baseline.py` result of 10 / 0 / 2, exit 0:

```csharp
// Back to forest is primary again right after demotion: the original two-primary defect.
learning.Root.Q<Button>("objective-back-to-forest")?.AddToClassList("btn-primary");

// Back to forest's runtime name leaves the allowlist, so it is never demoted: R04 by another spelling.
forest.name += "-x";
```

Three things make this more than the expected limits of a lexical scanner:

1. The false greens include the defect F1 was raised for, reached by ordinary edits, such as a small helper or one extra line.
2. Most are not disclosed, and the README states the opposite of several. For example, it says the map is "never … disabled, renamed or otherwise reused".
3. They do not need a parser. A scratch probe of eight small local guards closes 51 of the 52 and accepts the live source and all 15 benign edits. A three-guard subset with no whole-method pins closes all nine mutants in the blocking static-primary finding (RR-1).

**This is about the checker, not the game.** The live Objective UI source still passes, and these mutations are hypothetical future edits. All the false greens were also accepted by the old scanner at `f294fe07`, so none is a regression.

**Recommendation:** one more bounded repair, then another targeted re-review. See [Recommended repair scope](#recommended-repair-scope).

## Reviewer identity and independence

| Item | Value |
|---|---|
| Provider / model | Anthropic, Claude Opus 5.5 (`claude-opus-5-5`), via Claude Code in T3 Code |
| Observed effort | Harness `reasoning_effort=100` |
| Independence limitation | **Cross-provider review was unavailable** (per the #305 packet: ChatGPT/Codex quotas exhausted, OpenCode lane unusable). I share a provider with the #304 author (Claude Sonnet 5.5, effort 10) and a model with the #302 reviewer (Claude Opus 5.5). I am neither instance: this is a fresh session, and my auto-memory held no #302 or #304 content. A cross-provider reviewer might still catch something this model family misses. |
| What I read first | The #305 and #304 claims, the Manager packet, the repair handoff and the #302 report (for the R-ID definitions). I re-derived every R mutation from the #302 table rather than importing the author's fixtures. |
| Authority | Review only. I did not edit source or `active-tasks.md`, did not merge PR #300 and did not approve anything. |

## Lanes

| Lane | Path | Branch / HEAD | State |
|---|---|---|---|
| Report (writes here only) | `/media/jer/Files/CCF/Operations/CCF-FeedbackCTAF1F3Review-20261106` | `docs/baseline-cta-checker-f1-f3-re-review-20261106-v1`, start `6b12ac0e35156838d335725f866e02bc6db3a551` = `origin/main` | Clean with `-uall`; ext4 rw; 78 GiB free at first check, 73 GiB after the clones |
| Source (read-only, never written) | `/media/jer/Files/CCF/Operations/CCF-FeedbackCTARepair-20261106` | `1585ca20df2b921d10e79f9aac89f03202d9258e` | Clean before and after; equals the remote branch |
| Execution (throwaway clone) | `/media/jer/Files/CCF/Verification/CTA-F1F3-Review-304-20261106/source-clone` | Detached at `1585ca20` | Clean with `-uall` after every run; no `__pycache__` |
| Old baseline (throwaway) | `…/CTA-F1F3-Review-304-20261106/old-baseline-clone` | `72b4efbd` plus the three amended scripts, untracked | Used for item D only |
| End-to-end (throwaway) | `…/CTA-F1F3-Review-304-20261106/e2e-clone` | `1585ca20`, mutated files overlaid per run, restored after | Clean after the last run |

No competing writer: the repair worktree was clean at the handed-off HEAD, #304 was `[HANDED-OFF]`, and the only other `dotnet` work at preflight belonged to a separate D072 lane, alongside idle compile servers. `origin/main` advanced to `c6ef3168` during the review with unrelated #303/#306 coordination documents only. This branch stays based on `6b12ac0e`, and the reviewed candidate branch stayed at `1585ca20` throughout.

## Results by contract item

| Item | Result | Reason |
|---|---|---|
| **A.** R01, R03, R04, R05 fail; true source passes | **PASS** | All four rejected, each for the intended reason. Live source accepted with 8 evidence rows. |
| **A.** Enumerates inline `UiKit.Button` and `AddToClassList`/`EnableInClassList` in the constructor | **PASS** (direct shapes) | Direct shapes work, including `ToggleInClassList`, `if`, lambdas, `@map`, `\u` and `\x` spellings (X-F1-10 to 15). |
| **A.** *Every* static primary source in the constructor | **FAIL** | A helper or method group supplies a primary with no literal in the constructor (X-F1-01, 02, 28). Packet: "fail closed instead of accepting arbitrary future primary constructions". |
| **A.** Exactly one plain-literal name; the *actual* name is in the parsed demotion set | **FAIL** | The set is really parsed and a plain second assignment is caught, but `+=`, a cast or an alias changes the name unseen (X-F1-24, 25, 26). The allowlist can also be overwritten through a qualified reference (X-F1-27). |
| **A.** Rejects unnamed, duplicate, ambiguous and named-argument shapes | **PASS** | X-F1-07, 08, 16 to 20, 22, and the author's R05c to R05g. Interpolation and raw strings fail closed in the direct shape. |
| **A.** Forbids an unconditional map primary | **PASS** (constructor only) | Rejected in every direct shape tried. Accepted from outside the constructor (X-F1-03, disclosed in the handoff) and when hidden from the lexer (X-F1-29, 30). |
| **A.** Verifies the scoped restore/remove loop | **FAIL** | The loop is checked for presence and order, but nothing stops the class being put back straight after it (X-F2-16, 17). |
| **B.** R07, R08, R09, R11, R14, R20, R21 and claimed R12, R13, R15 | **PASS** | All rejected, reasons checked against the intended defect. No unrelated parse error. |
| **B.** Own override, ordering, duplicate and guard mutations | **FAIL** | 46 attempted: 11 rejected, 35 accepted. See RR-2 and RR-3. |
| **C.** Original 20 retained and rejected; no mask; 4 benign accepted | **PASS** | Tuple-for-tuple identical in the same order, by AST comparison. No skip, xfail or warning vocabulary. No scanner predicate weakened: 98 of 112 old string constants survive verbatim, and I read the other 14 (evidence-row text and replaced checks now covered by stronger ones). None of my mutants was rejected by the old scanner and accepted by the candidate. 4/4 benign accepted, plus my own 15/15. |
| **D.** Candidate 10/0/2 exit 0 | **PASS** | `PASS=10 FAIL=0 NOT-TESTABLE=2`, exit 0, stderr empty. Stdout sha256 `8cc51e4e…` equals the author's. |
| **D.** Old baseline 6/4/2 exit 1, test 08 a completed FAIL | **PASS** | `PASS=6 FAIL=4 NOT-TESTABLE=2`, exit 1, `tracked-production-vs-baseline: MATCH`, stderr empty. Test 08 completes as FAIL with three evidence rows; no parse crash. |
| **D.** `baseline-output.txt` byte-unchanged | **PASS** | Blob `db6a8b85…` at both `f294fe07` and `1585ca20`, sha256 `513cddd9…`. Regenerated output differs on line 48 only (test-08 text). |
| **D.** `py_compile` on all three scripts | **PASS** | Exit 0, no output, pycache redirected outside the clone. |
| **E.** Six real SnapFar cases under SDK 8 | **PASS** | **Observed `dotnet --version` from the same cwd: 8.0.133.** Six PASS rows, no mock. The trace proves the checker's own `dotnet` used it: see [SDK evidence](#sdk-evidence). |
| **F.** Diff, whitespace, protected files | **PASS** | Exactly the five allowed paths. `git diff --check` exit 0. Assets, Packages, ProjectSettings, `baseline-output.txt`, `BASE` and tests 01 to 07, 09, 06a to c unchanged. See [Protected-file audit](#protected-file-audit). |
| **F.** README on conditional CTA and exit semantics | **PASS** | Both accurate. Exit semantics re-verified end to end on mutated trees: R03 and a missing constructor both complete as 9/1/2 exit 1. |
| **F.** README on lexical limits | **FAIL** | Overclaims. See RR-4. |
| **F.** #301 history not rewritten | **PASS** | The only handoff change is the new file; the old qualification handoff is untouched. |
| **G.** Author's claimed limitations | **FAIL** | The limits stated are real, but one is mis-stated and the list is materially incomplete. See [Limitations](#limitations-claimed-and-additional). |

## Findings

Severities are my recommendation; the Manager rules. "Runtime effect" below is code reasoning. Nothing was run in Unity.

### RR-1 — Major, recommended blocking: a static primary can still escape demotion

This is the F1 defect class, and the packet's own contract sentences cover it: "every competing static primary button's **actual** name", "fail closed instead of accepting arbitrary future primary constructions", and "verify the existing actual scoped loop restores and removes the class correctly".

| Route | Mutants | Why it passes |
|---|---|---|
| R04 by another spelling | X-F1-24 `forest.name += "-x"`, X-F1-25 `((VisualElement)forest).name = …`, X-F1-26 `Button alias = forest; alias.name = …` | The name regex misses `+=`, and the receiver parse attributes cast and alias writes to something other than `forest`. The "no other uses" rule that protects the map (`objective_cta_source_check.py:219`) is not applied to the static primary local. |
| R05 through a helper | X-F1-01 `MakePrimary(...)`, X-F1-02 `Promote(close)`, X-F1-28 method group plus class-level const | The audit reads only constructor text, so a primary whose literal lives elsewhere is invisible. An unrecognised way of building a header button does not fail closed. |
| R03 at run time | X-F1-27 `ScenarioOneUiRoot.GuidanceButtonNames[2] = …`; also `System.Array.Clear(ScenarioOneUiRoot.GuidanceButtonNames, 0, 3)` | The "referenced only by its declaration and the demotion loop" guard (`:203`) uses `(?<![.\w])`, so a qualified reference is not counted. |
| Demotion undone | X-F2-16 re-add after the loop, X-F2-17 second restore loop with a renamed variable | Nothing limits `btn-primary` elsewhere in the root. The live root has exactly 5 such literals, so a one-line inventory catches this. |

X-F2-16, X-F1-24 and X-F1-01 were each run through the full `check_baseline.py`: 10/0/2, exit 0, test 08 PASS.

### RR-2 — Major, recommended blocking: guards the handoff describes can be bypassed by ordinary syntax

| Handoff / README claim | Bypass | Cause |
|---|---|---|
| Handoff: "`helpClosedFrame` … nothing else may write it"; "extra writes to `candidate`, `guidanceCandidate`, `target` … are rejected" | `this.helpClosedFrame = -1;` (X-F2-28), `this.guidanceCandidate = null;` (X-F2-27). These are the author's R20c and R23 fixtures with `this.` added. Also accepted when placed in `ShowObjectives`, and for `this.nextAction`, so it does not depend on the method chosen. | `_assignments` at `:104` excludes any name preceded by `.`. |
| README: "no later `target` write" | `(target, reason) = ("objective-back-to-forest", reason);` before the binding (X-F2-09): R11 by tuple | The write regex needs `target =`. |
| Handoff: R11 "repaired for the bounded variants" | One write moved into the chain, one elsewhere removed (X-F2-10): the Objective 1 CTA becomes Back to forest with the count still 14 | Count-only pin (`:436-440`); the after-chain check starts at `else return;`. |
| README: "no … extra early return" | First guard replaced by bare `return;` (X-F2-08): the whole method is dead, the return count still 3 | Count-only pin (`:444`); the first guard's text is not pinned. |

### RR-3 — Major, Manager's call whether to close or disclose: named defects survive in other ordinary forms

The packet accepts limits that "need a full C# control-flow interpreter" when they are explicit. These are neither listed nor in need of an interpreter: small local guards close all of them in the probe.

| Named defect | Other form accepted |
|---|---|
| R14 editor-only styling | `[Conditional("UNITY_EDITOR")]` on Apply, no `#if` anywhere (X-F2-21); `if (false) candidate.AddToClassList(...)` (X-F2-20) |
| R21 no late apply | Call wrapped in a never-invoked lambda (X-F2-22); local function shadows the method (X-F2-23) |
| R20 no help-close frame | Inverted early guard (X-F2-29); setter inside a never-invoked lambda (X-F2-30) |
| R13 guidance cancelled | `goto` (X-F2-24), `throw` (X-F2-25) |
| R12 CTA class cancelled | `nextAction` class removed in Apply (X-F2-19) or every frame in LateUpdate (X-F2-18) |
| R09 dead guard | Guard inside a never-invoked lambda (X-F2-26) |
| R08 / R15 map renamed or disabled | Through `buttons[0]` (X-F2-01, 02); row disabled or cleared (X-F2-03, 04); `if (false)` around the row binding or name (X-F2-05, 06) |

X-F2-21 and X-F2-27 were also run end to end: 10/0/2, exit 0.

### RR-4 — Major for accuracy: README and handoff overclaim

These need correcting however RR-1 to RR-3 are resolved, because later readers rely on this text.

| Text | Contradicted by |
|---|---|
| README: "**every** static source of `btn-primary` in that constructor … mechanically enumerated" | X-F1-01, 02, 28 |
| README: map "never assigned a primary class, disabled, renamed or otherwise reused" | X-F2-01, 02, 03; X-F1-03 |
| README: "no later `target` write, extra early return or reset" | X-F2-08, 09, 10, 27 |
| Handoff: allowlist "referenced only by its declaration and the demotion loop" | X-F1-27 |
| Handoff: `LateUpdate` "must call" Apply "before any branch or return" | X-F2-22, 23 |
| Handoff: `CloseHelp` "must record `helpClosedFrame` unconditionally" | X-F2-29, 30 |
| Handoff limit: "a helper that receives only `forest`" | Mis-stated. `Promote(forest)` would be harmless because `forest` is allowlisted. The harmful, undisclosed shape is a helper that builds or promotes a *different* button (X-F1-01, 02). |
| README "Known lexical limits" | Omits same-file helpers and methods outside the constructor (the handoff has these, the README does not), and omits qualified writes, tuples, statement wrappers, lambdas, local functions and attributes |
| README: "uniquely named `objective-primary-cta` in any ordinary/verbatim/escaped spelling" | The *map's own* name must be ordinary; `map.name = @"…"` is rejected. Harmless (fails closed), but the sentence only holds for duplicates. |

### RR-5 — Minor: F6 diagnostics name the wrong defect for allowlist failures

`_verify` (`:370-380`) catches an allowlist error, runs the header audit with an empty set, and re-raises only if that audit passes. With a static primary present it never does, so the real error is replaced. A duplicate allowlist entry reports "static primary button 'objective-back-to-forest' is not in the GuidanceButtonNames scoped demotion set", although that name is in the set twice. The real errors, recovered by calling `_allowlist_values` directly, are "GuidanceButtonNames contains a duplicate name" and "conditional compilation (#if)…". Seen on X-F1-21, X-F1-23 and the author's `allowlist-duplicate-name`, whose handoff row prints this reason without comment. Rejection is still correct; the message would send a maintainer the wrong way. The packet's F6 asks for the specific failing predicate.

### Non-blocking notes

- **N-1, contract question.** X-F1-31 (`work.AddToClassList("btn-primary")`) and X-F1-32 (a new CloseAll primary named `map-back-to-forest`) are accepted, and that conforms to the packet: both names are in the demotion set. By code reading they are demoted in the guided state but show two primaries outside it, where `RefreshNextActionGuidance` returns before the loop (Legacy40, later objectives). I do not count these as false greens. The Manager may want "exactly one static primary".
- **N-2, adjacent logic not pinned.** Routing (X-F2-11 to 15), visibility (X-F2-31, 32), the only Objectives caller of Refresh (X-F2-33, 34), `ShowMap`'s body (X-F2-35), a hidden header (X-F2-07), primaries added outside the constructor (X-F1-03, 04, disclosed in the handoff), and non-map name bindings or a duplicate caption (X-F1-33, 34). Counted as false greens, lower priority than RR-1 to RR-3.
- **N-3, input scope.** The scanner reads two files. `LearningObjectivesView.Sequence.cs` and `.Curriculum.cs` are the same class and invisible to it (X-F1-05). `UiKit` is too (X-F1-06). Both are disclosed. The two partials would be cheap to add.
- **N-4, lexer desync.** `_ = $"{'"'}"; map.AddToClassList("btn-primary"); /*'*/` and an `@$"\"` form make the lexer treat a real statement as literal text (X-F1-29, 30). A Roslyn syntax pass confirms the invocation is real code. The handoff says interpolated strings are not analysed, but not that this can hide arbitrary statements. These are deliberately adversarial and unlikely by accident.
- **N-5, author's evidence.** 11/11 checksums in the author's raw evidence verify, and my stdout hash matches theirs. The claim that all 35 new fixtures were false greens on the old scanner holds (I reproduced 35/35). Every tally the author reported reproduced, and I found no misreported result; the text overclaims are in RR-4. The author flagged low effort and asked for it to be weighed, so I relied on no author result without rerunning it.
- **N-6, later transfer.** Informational only: the candidate scanner accepts the view and root at the #296 tip `69b41605` in memory (root identical, one view line differs outside the constructor). This does not replace the required rerun on the exact combined candidate.

## Feasibility evidence

**Scratch only, not a repair, not to be shipped as written.** `scripts/prototype_feasibility.py` wraps the unchanged candidate and runs extra guards only after it accepts.

| Probe | Guards | Result |
|---|---|---|
| Minimal (`prototype_tier1.py`) | Literal inventory of `btn-primary` across the view class and root; write and reference counts without the `.` blind spot; static primary local has no other uses | Closes **18 of 52**, including all nine RR-1 mutants and X-F2-27, 28. 15/15 benign accepted. |
| Full (`prototype_feasibility.py`) | The above, plus: small checked methods pinned as comment- and whitespace-normalised text; no attribute on checked methods; LateUpdate must start with the apply and nothing may shadow it; header is straight-line and its locals keep a fixed shape; caption-to-name bindings; Objectives caller present once outside `#if` | Closes **51 of 52** (all but X-F1-06, which needs `UiKit` read). 15/15 benign accepted. Live source and the #296 tip accepted. |

Limits of this evidence: I fitted these guards after seeing the mutants, so they show the mutants can be closed locally, not that the result is complete. Nobody has attacked them. Whole-method pins cost maintenance: any deliberate edit to a pinned method needs a checker update. The packet warned about brittle bans, though these are method-local and the existing count pins already force review of routing changes. That trade-off is the Manager's.

## Recommended repair scope

One bounded repair under the same five-path limit, by a writer other than this reviewer:

1. **Close RR-1 and RR-2 in code.** The minimal guards above, plus tuple writes, the first Refresh guard and count-compensated retargeting.
2. **Close RR-3 with small local guards**, using method-text pins where needed. If the Manager rejects that maintenance cost, list each form explicitly as a limit instead. Either way, do not leave it undisclosed.
3. **Fix RR-5** by raising the allowlist error first when it is the cause, while keeping the historical baseline's completed FAIL.
4. **Correct the README and handoff text** (RR-4).
5. **Add the mutants as negative controls.** `scripts/review_mutations.py` holds them as data. Rule on N-1 first, because it decides whether X-F1-31 and X-F1-32 are controls.
6. **Rerun a targeted re-review**, cross-provider if a provider has become available.

## Mutation results

Totals: **author's 55** broken-source plus 4 benign, rerun; **mine 102** broken-source (21 re-derived R-IDs and 81 new) plus 15 benign. Overall 157 broken-source and 19 benign.

| Group | Count | Rejected | Accepted | Accepted by old `f294fe07` scanner |
|---|---|---|---|---|
| Author: original 20 | 20 | 20 | 0 | 0 |
| Author: F1 | 14 | 14 | 0 | 14 |
| Author: F2/F3 | 21 | 21 | 0 | 21 |
| Mine: R-IDs re-derived from #302 | 21 | 21 | 0 | 14 |
| Mine: F1 shapes (X-F1) | 35 | 18 | 17 | 35 |
| Mine: F2/F3 shapes (X-F2) | 46 | 11 | 35 | 43 |
| **False greens** | | | **50**, plus 2 contract-conformant (N-1) | |
| Benign: author | 4 | 0 | 4 | 4 |
| Benign: mine | 15 | 0 | 15 | 15 |

Every mutant was compiled with Roslyn at C# 9 against the real Unity 6000.6 DLLs, with a Player-like define set (`UNITY_EDITOR` absent). The unmodified baseline compiles with 0 errors across 189 files. All 52 accepted mutants compile with 0 errors. Only X-F1-09 (raw strings are not available in C# 9) and X-F2-44 (`#if`'d out under these defines) do not, and the scanner rejects both. This is a compiler check, not a Unity compile.

### R-IDs re-derived from the #302 table

| ID | Mutation | Candidate | Old scanner | Compile errors | Rejection reason (cause matched) |
|---|---|---|---|---|---|
| R01 | map statically primary | REJECTED | accepted | 0 | `map CTA must not be unconditionally primary (AddToClassList)` ✓ |
| R02 | inline primary on the map constructor | REJECTED | rejected | 0 | `map CTA must not be unconditionally primary (UiKit.Button)` ✓ |
| R03 | Back to forest dropped from the allowlist | REJECTED | accepted | 0 | `static primary button 'objective-back-to-forest' is not in the GuidanceButtonNames scoped demotion set` ✓ |
| R04 | Back to forest renamed out of the allowlist | REJECTED | accepted | 0 | `static primary button 'objective-forest' is not in the GuidanceButtonNames scoped demotion set` ✓ |
| R05 | new CloseAll primary | REJECTED | accepted | 0 | `static primary 'close' must have exactly one literal name; found 0` ✓ |
| R06 | duplicate CTA name, no spaces | REJECTED | rejected | 0 | `duplicate element name in the Objectives constructor` ✓ |
| R07 | verbatim duplicate CTA name | REJECTED | accepted | 0 | `duplicate element name in the Objectives constructor` ✓ |
| R08 | later map rename | REJECTED | accepted | 0 | `Objective CTA name must be unique and bound to the ShowMap button; found map names ['objective-primary-cta', 'objective-map']` ✓ |
| R09 | guard moved into a string | REJECTED | accepted | 0 | `ApplyNextActionGuidance/no styling if ineligible: expected one structural match, found 0` ✓ |
| R10 | guard commented out | REJECTED | rejected | 0 | `ApplyNextActionGuidance/no styling if ineligible: expected one structural match, found 0` ✓ |
| R11 | target override to CloseAll before binding | REJECTED | accepted | 0 | `target is reassigned after the routing chain` ✓ |
| R12 | candidate nulled in Apply | REJECTED | accepted | 0 | `ApplyNextActionGuidance writes 'candidate' more than once (override)` ✓ |
| R13 | return after tracking clear | REJECTED | accepted | 0 | `RefreshNextActionGuidance has an unexpected early return` ✓ |
| R14 | primary class behind #if UNITY_EDITOR | REJECTED | accepted | 0 | `Apply: conditional compilation (#if) in a checked method is unsupported` ✓ |
| R15 | map disabled after binding | REJECTED | accepted | 0 | `map CTA variable may only be declared, named and added to the header row; other uses are unsupported` ✓ |
| R16 | final return made true | REJECTED | rejected | 0 | `out-of-scope candidate rejected: expected one structural match, found 0` ✓ |
| R17 | guard/assign swapped | REJECTED | rejected | 0 | `ApplyNextActionGuidance/set current candidate: invalid control-flow order` ✓ |
| R18 | V2 gate negated | REJECTED | rejected | 0 | `RefreshNextActionGuidance/rack teaching gate: expected one structural match, found 0` ✓ |
| R19 | objectives flag widened | REJECTED | rejected | 0 | `RefreshNextActionGuidance/objectives-scoped branch: expected one structural match, found 0` ✓ |
| R20 | help-close setter removed | REJECTED | accepted | 0 | `CloseHelp/help-close frame recorded: expected one structural match, found 0` ✓ |
| R21 | LateUpdate apply removed | REJECTED | accepted | 0 | `LateUpdate applies cached guidance: expected one structural match, found 0` ✓ |

### Reviewer's own F1 shapes

| ID | Mutation | Candidate | Old scanner | Compile errors | Class if accepted / rejection reason |
|---|---|---|---|---|---|
| X-F1-01 | R05 via a helper that builds the primary (literal outside the constructor) | **ACCEPTED** | accepted | 0 | Static primary (RR-1) |
| X-F1-02 | R05 via a helper that promotes an existing button | **ACCEPTED** | accepted | 0 | Static primary (RR-1) |
| X-F1-03 | map made primary from a method outside the constructor, name split to dodge the literal count | **ACCEPTED** | accepted | 0 | Unpinned logic (N-2) |
| X-F1-04 | unnamed extra primary created outside the constructor, appended to the header row | **ACCEPTED** | accepted | 0 | Unpinned logic (N-2) |
| X-F1-05 | partial class in another file promotes the map (scanner never reads that file) | **ACCEPTED** | accepted | 0 | Outside scanner input (N-3) |
| X-F1-06 | UiKit.Button itself makes every button primary (scanner never reads UiKit) | **ACCEPTED** | accepted | 0 | Outside scanner input (N-3) |
| X-F1-07 | interpolated class name on the map | REJECTED | accepted | 0 | `AddToClassList in the header uses a non-literal class name (ambiguous)` |
| X-F1-08 | interpolated class name on an inline primary | REJECTED | accepted | 0 | `UiKit.Button class argument in the header is not a plain string literal (ambiguous)` |
| X-F1-09 | C# 11 raw string class name (not valid at Unity's C# level) | REJECTED | accepted | 1 | `AddToClassList in the header uses a non-literal class name (ambiguous)` |
| X-F1-10 | map primary inside an if | REJECTED | accepted | 0 | `map CTA must not be unconditionally primary (AddToClassList)` |
| X-F1-11 | map primary deferred in a lambda | REJECTED | accepted | 0 | `map CTA must not be unconditionally primary (AddToClassList)` |
| X-F1-12 | map primary by ToggleInClassList | REJECTED | accepted | 0 | `map CTA must not be unconditionally primary (ToggleInClassList)` |
| X-F1-13 | verbatim identifier @map | REJECTED | accepted | 0 | `map CTA must not be unconditionally primary (AddToClassList)` |
| X-F1-14 | unicode-escaped identifier | REJECTED | accepted | 0 | `static primary 'u0061p' must be declared once and never reassigned in the header` |
| X-F1-15 | escaped class literal \x | REJECTED | accepted | 0 | `map CTA must not be unconditionally primary (AddToClassList)` |
| X-F1-16 | new Button primary, no name | REJECTED | accepted | 0 | `static primary 'close' must have exactly one literal name; found 0` |
| X-F1-17 | Q-chained primary | REJECTED | accepted | 0 | `AddToClassList(btn-primary) on a non-simple receiver in the header (unnamed primary)` |
| X-F1-18 | row indexer primary | REJECTED | accepted | 0 | `AddToClassList(btn-primary) on a non-simple receiver in the header (unnamed primary)` |
| X-F1-19 | two-step declaration of a primary | REJECTED | accepted | 0 | `static btn-primary header button is not assigned to a named local (unnamed primary)` |
| X-F1-20 | class assigned to the forest variable after declaration (reassignment) | REJECTED | accepted | 0 | `static primary 'forest' must be declared once and never reassigned in the header` |
| X-F1-21 | duplicate allowlist spelling (ordinary + verbatim) | REJECTED | accepted | 0 | `static primary button 'objective-back-to-forest' is not in the GuidanceButtonNames scoped demotion set` |
| X-F1-22 | allowlist entry commented out (R03 with the text still present) | REJECTED | accepted | 0 | `static primary button 'objective-back-to-forest' is not in the GuidanceButtonNames scoped demotion set` |
| X-F1-23 | allowlist entry behind #if (R03 in a Player build) | REJECTED | accepted | 0 | `static primary button 'objective-back-to-forest' is not in the GuidanceButtonNames scoped demotion set` |
| X-F1-24 | R04 by compound assignment: Back to forest's runtime name leaves the allowlist | **ACCEPTED** | accepted | 0 | Static primary (RR-1) |
| X-F1-25 | R04 by cast: second name write the receiver parse cannot attribute | **ACCEPTED** | accepted | 0 | Static primary (RR-1) |
| X-F1-26 | R04 by alias: second name write through another local | **ACCEPTED** | accepted | 0 | Static primary (RR-1) |
| X-F1-27 | allowlist element overwritten at run time through a qualified reference (R03 at run time) | **ACCEPTED** | accepted | 0 | Static primary (RR-1) |
| X-F1-28 | method-group + class-level const: no call shape and no literal in the constructor | **ACCEPTED** | accepted | 0 | Static primary (RR-1) |
| X-F1-29 | lexer desync: char literal inside an interpolation hole hides a map primary | **ACCEPTED** | accepted | 0 | Lexer desync (N-4) |
| X-F1-30 | lexer desync: @$ verbatim-interpolated backslash hides a map primary | **ACCEPTED** | accepted | 0 | Lexer desync (N-4) |
| X-F1-31 | second static primary on an allowlisted button (demoted in guided state, doubled outside it) | ACCEPTED | accepted | 0 | Contract-conformant, not counted (N-1) |
| X-F1-32 | new CloseAll primary reusing another view's allowlisted name (demoted in guided state, doubled outside it) | ACCEPTED | accepted | 0 | Contract-conformant, not counted (N-1) |
| X-F1-33 | names swapped between Work Plan and Back to forest (both allowlisted): later-state targets route to the wrong… | **ACCEPTED** | accepted | 0 | Unpinned logic (N-2) |
| X-F1-34 | second non-primary button with the map caption that closes the screen | **ACCEPTED** | accepted | 0 | Unpinned logic (N-2) |
| X-F1-35 | spoof captioned map + real map primary (tolerant lookup picks the spoof) | REJECTED | accepted | 0 | `static primary button 'objective-primary-cta' is not in the GuidanceButtonNames scoped demotion set` |

### Reviewer's own F2/F3 shapes

| ID | Mutation | Candidate | Old scanner | Compile errors | Class if accepted / rejection reason |
|---|---|---|---|---|---|
| X-F2-01 | R08 by row indexer: map renamed through buttons[0] | **ACCEPTED** | accepted | 0 | Named defect, other form (RR-3) |
| X-F2-02 | R15 by row indexer: map disabled through buttons[0] | **ACCEPTED** | accepted | 0 | Named defect, other form (RR-3) |
| X-F2-03 | R15 by parent: whole button row disabled | **ACCEPTED** | accepted | 0 | Named defect, other form (RR-3) |
| X-F2-04 | row cleared after binding: Q<Button> finds no map | **ACCEPTED** | accepted | 0 | Named defect, other form (RR-3) |
| X-F2-05 | row binding wrapped in if (false) | **ACCEPTED** | accepted | 0 | Named defect, other form (RR-3) |
| X-F2-06 | map identity wrapped in if (false) | **ACCEPTED** | accepted | 0 | Named defect, other form (RR-3) |
| X-F2-07 | header row hidden | **ACCEPTED** | accepted | 0 | Unpinned logic (N-2) |
| X-F2-08 | R13 count-preserving: first guard replaced by a bare return, whole method dead | **ACCEPTED** | accepted | 0 | Guard bypass (RR-2) |
| X-F2-09 | R11 by tuple assignment | **ACCEPTED** | accepted | 0 | Guard bypass (RR-2) |
| X-F2-10 | R11 count-compensated: retarget inside the chain, one other target write removed | **ACCEPTED** | accepted | 0 | Guard bypass (RR-2) |
| X-F2-11 | objectives flag overwritten after its pinned declaration | **ACCEPTED** | accepted | 0 | Unpinned logic (N-2) |
| X-F2-12 | annual-review branch forced, map target unreachable | **ACCEPTED** | accepted | 0 | Unpinned logic (N-2) |
| X-F2-13 | consent forced true, chain never reaches the map target | **ACCEPTED** | accepted | 0 | Unpinned logic (N-2) |
| X-F2-14 | teaching gate widened after the pinned prefix: map CTA offered beyond Objective 1 | **ACCEPTED** | accepted | 0 | Unpinned logic (N-2) |
| X-F2-15 | routing literal swapped, count unchanged | **ACCEPTED** | accepted | 0 | Unpinned logic (N-2) |
| X-F2-16 | Back to forest re-promoted right after the demotion loop (original two-primary defect) | **ACCEPTED** | accepted | 0 | Static primary (RR-1) |
| X-F2-17 | demotion undone by a second loop with a renamed variable | **ACCEPTED** | accepted | 0 | Static primary (RR-1) |
| X-F2-18 | CTA primary class removed every frame after the LateUpdate apply | **ACCEPTED** | accepted | 0 | Named defect, other form (RR-3) |
| X-F2-19 | CTA primary class removed in Apply through nextAction | **ACCEPTED** | accepted | 0 | Named defect, other form (RR-3) |
| X-F2-20 | R14 by plain if: primary class never applied | **ACCEPTED** | accepted | 0 | Named defect, other form (RR-3) |
| X-F2-21 | R14 by [Conditional]: Apply compiled out of Player builds, no #if anywhere | **ACCEPTED** | accepted | 0 | Named defect, other form (RR-3) |
| X-F2-22 | R21 by lambda: LateUpdate call never invoked | **ACCEPTED** | accepted | 0 | Named defect, other form (RR-3) |
| X-F2-23 | R21 by shadow: local function replaces the method inside LateUpdate | **ACCEPTED** | accepted | 0 | Named defect, other form (RR-3) |
| X-F2-24 | R13 by goto: eligible candidate skips styling | **ACCEPTED** | accepted | 0 | Named defect, other form (RR-3) |
| X-F2-25 | R13 by throw: eligible candidate never styled | **ACCEPTED** | accepted | 0 | Named defect, other form (RR-3) |
| X-F2-26 | R09 by dead lambda: guard present but never executed | **ACCEPTED** | accepted | 0 | Named defect, other form (RR-3) |
| X-F2-27 | R23 with this.: candidate nulled by an unrelated handler | **ACCEPTED** | accepted | 0 | Guard bypass (RR-2) |
| X-F2-28 | R20c with this.: help-close frame reset | **ACCEPTED** | accepted | 0 | Guard bypass (RR-2) |
| X-F2-29 | R20 by inverted early guard: setter unreachable while Help is open | **ACCEPTED** | accepted | 0 | Named defect, other form (RR-3) |
| X-F2-30 | R20 by lambda: setter never invoked | **ACCEPTED** | accepted | 0 | Named defect, other form (RR-3) |
| X-F2-31 | GuidanceVisible parameter overwritten: every candidate reads as visible | **ACCEPTED** | accepted | 0 | Unpinned logic (N-2) |
| X-F2-32 | GuidanceVisible skips the candidate's own visibility | **ACCEPTED** | accepted | 0 | Unpinned logic (N-2) |
| X-F2-33 | only Objectives caller of Refresh removed: guidance never derived | **ACCEPTED** | accepted | 0 | Unpinned logic (N-2) |
| X-F2-34 | only Objectives caller of Refresh behind #if (R14 at the call site) | **ACCEPTED** | accepted | 0 | Unpinned logic (N-2) |
| X-F2-35 | ShowMap gutted: CTA still wired by name, opens nothing | **ACCEPTED** | accepted | 0 | Unpinned logic (N-2) |
| X-F2-36 | ordering: candidate bound after Apply | REJECTED | rejected | 0 | `RefreshNextActionGuidance/apply after binding: expected one structural match, found 2` |
| X-F2-37 | guard in block comment | REJECTED | rejected | 0 | `ApplyNextActionGuidance/no styling if ineligible: expected one structural match, found 0` |
| X-F2-38 | guard in verbatim string | REJECTED | accepted | 0 | `ApplyNextActionGuidance/no styling if ineligible: expected one structural match, found 0` |
| X-F2-39 | guard in interpolated string | REJECTED | accepted | 0 | `ApplyNextActionGuidance/no styling if ineligible: expected one structural match, found 0` |
| X-F2-40 | duplicate CTA name with \x escape | REJECTED | accepted | 0 | `duplicate element name in the Objectives constructor` |
| X-F2-41 | duplicate CTA name in an object initializer | REJECTED | accepted | 0 | `duplicate element name in the Objectives constructor` |
| X-F2-42 | duplicate CTA name via class-level const | REJECTED | accepted | 0 | `an element name in the Objectives constructor is not a single plain literal` |
| X-F2-43 | allowlist shadowed by a local in Refresh | REJECTED | rejected | 0 | `static primary button 'objective-back-to-forest' is not in the GuidanceButtonNames scoped demotion set` |
| X-F2-44 | Apply wrapped in #if UNITY_EDITOR | REJECTED | accepted | 2 | `Apply: declared inside conditional compilation (#if)` |
| X-F2-45 | candidate nulled by ref helper in Apply | REJECTED | accepted | 0 | `ApplyNextActionGuidance writes 'candidate' more than once (override)` |
| X-F2-46 | second Apply overload shadow attempt: call site no longer zero-argument | REJECTED | accepted | 0 | `LateUpdate applies cached guidance: expected one structural match, found 0` |

### Reviewer's benign controls

| ID | Edit | Candidate | Compile errors |
|---|---|---|---|
| B-X-01 | map local renamed consistently | ACCEPTED | 0 |
| B-X-02 | forest local renamed consistently | ACCEPTED | 0 |
| B-X-03 | forest declared with var | ACCEPTED | 0 |
| B-X-04 | block comment inside the identity statement | ACCEPTED | 0 |
| B-X-05 | guard split over two lines | ACCEPTED | 0 |
| B-X-06 | work and forest order swapped in the row | ACCEPTED | 0 |
| B-X-07 | new named non-primary header button | ACCEPTED | 0 |
| B-X-08 | extra harmless class on the static primary | ACCEPTED | 0 |
| B-X-09 | #region in the constructor | ACCEPTED | 0 |
| B-X-10 | comment with apostrophe and quote in the constructor | ACCEPTED | 0 |
| B-X-11 | #if in an unrelated root method | ACCEPTED | 0 |
| B-X-12 | string with // in the constructor | ACCEPTED | 0 |
| B-X-13 | header label containing the word primary | ACCEPTED | 0 |
| B-X-14 | CRLF line endings in both files | ACCEPTED | 0 |
| B-X-15 | UTF-8 BOM at the start of both files | ACCEPTED | 0 |

## Commands and raw tallies

`ROOT=/media/jer/Files/CCF/Verification/CTA-F1F3-Review-304-20261106`, `T=$ROOT/source-clone/Tools/Verification/OvernightPlaytestFeedback`. All Python ran with `PYTHONDONTWRITEBYTECODE=1 PYTHONPYCACHEPREFIX=$ROOT/pycache TMPDIR=$ROOT/tmp`. Raw files are in `$ROOT/evidence/`; `SHA256SUMS.txt` there verifies them and the scripts, except the generated mutant source directories.

| # | Cwd | Command | Exit | Raw result | Evidence |
|---|---|---|---|---|---|
| 1 | clone | `python3 -m py_compile $T/check_baseline.py $T/objective_cta_source_check.py $T/test_objective_cta_source_check.py` | 0 | no output | `01-py_compile.out` |
| 2 | clone | `python3 -B $T/test_objective_cta_source_check.py` | 0 | original 20/20, F1 14/14, F2-F3 21/21, benign 4/4, historical rejected with 3 rows, `rejected=55/55 false_greens=0`, stderr empty | `02-author-suite.*` |
| 3 | clone | `python3 -B $T/check_baseline.py --rotation-state post-fix --instruction-state post-fix` | 0 | `PASS=10 FAIL=0 NOT-TESTABLE=2`, sha256 `8cc51e4e39db0318…` | `03-candidate-default.*` |
| 4 | old-baseline-clone at `72b4efbd` with the three amended scripts copied in | `python3 -B Tools/Verification/OvernightPlaytestFeedback/check_baseline.py --rotation-state pre-fix --instruction-state pre-fix` | 1 | `MATCH`, `PASS=6 FAIL=4 NOT-TESTABLE=2`, stderr empty; `diff` against `baseline-output.txt` is line 48 only | `04-*` |
| 5 | scratch | `python3 -B -I scripts/compare_original_20.py old-f294/test_objective_cta_source_check.py $T/test_objective_cta_source_check.py` | 0 | tuple-for-tuple identical: True; mask vocabulary: none | `05-original-20-compare.txt` |
| 5b | scratch | `python3 -B -I scripts/compare_predicates.py old-f294/objective_cta_source_check.py $T/objective_cta_source_check.py` | 0 | 112 constants, 98 verbatim, 14 reviewed | `05b-predicate-compare.txt` |
| 6 | scratch | `python3 -B scripts/review_mutations.py $ROOT/source-clone $ROOT/old-f294 $ROOT/evidence/mutations` | 0 | see tables above | `06-*`, `mutations/results.json` |
| 7 | `$ROOT/roslyn-sdk8` | `dotnet roslyn-tool/bin/RoslynCheck.dll $ROOT/source-clone evidence/mutations/mutants roslyn-tool/refs.txt "<defines>" evidence/07-roslyn-compile.txt` | 0 | baseline 189 files, 0 errors; 116 of 118 rows at 0 errors (the 2 others listed above) | `07-roslyn-compile.txt` |
| 7b | `$ROOT/roslyn-sdk8` | `dotnet roslyn-truth/bin/RoslynTruth.dll <X-F1-29> <X-F1-30> <live view>` | 0 | `map.AddToClassList("btn-primary")` present as a real invocation in both mutants, absent from live source | `07b-roslyn-desync-oracle.txt` |
| 8 | `$ROOT/sdk8/cwd/deeper` | `dotnet --version`; `dotnet --info`; `dotnet --list-sdks` | 0 | **8.0.133** | `08-sdk-identity.txt` |
| 8a | same | `python3 -B $T/check_baseline.py --rotation-state post-fix --instruction-state post-fix` | 0 | 10/0/2, six SnapFar PASS, stderr empty, byte-identical to #3 | `08a-*` |
| 8b | same | as 8a under `COREHOST_TRACE=1 COREHOST_TRACE_VERBOSITY=4 COREHOST_TRACEFILE=… strace -f -qq -e trace=execve,openat -e signal=none -o …` | 0 | byte-identical; 263 successful opens under `sdk/8.0.133`, 0 under `sdk/10.0.114` | `08b-*` |
| 8c | `$ROOT/control-cwd` (no `global.json`) | as 8b | 0 | `dotnet --version` 10.0.114; 0 opens under SDK 8, 538 under SDK 10; byte-identical | `08c-*` |
| 9 | clone | `git diff` / `git rev-parse` audit, full transcript in the file | 0 | see below | `09-protected-audit.txt` |
| 9b | scratch | AST comparison of `check_baseline.py` | 0 | only the test-08 `try` block differs | `09b-*` |
| 10 | scratch | `python3 -B scripts/prototype_feasibility.py $ROOT/source-clone evidence/mutations` | 0 | 51/52 closed, 15/15 benign | `10-feasibility-probe.txt` |
| 10b | scratch | `python3 -B scripts/prototype_tier1.py $ROOT/source-clone evidence/mutations` | 0 | 18/52 closed, 0 benign rejected | `10b-tier1-probe.txt` |
| 11 | clone | `git fetch origin feature/scenario-one-next-action-guidance-v1`, then in-memory scan of `69b41605` | 0 | accepted, 8 rows | `11-feature-branch-probe.txt` |
| 12 | scratch | inline cross-check of `results.json` and compile rows | 0 | 21/21 intended cause; old-rejected-but-new-accepted: none | `12-cross-checks.txt` |
| 13 | scratch | `runpy` of the candidate test file with `old-f294` first on `sys.path` | 1, expected | original 20/20, new 0/35 rejected, 35 FALSE GREEN lines; then `AttributeError` at the historical step because the old exception has no `.rows` | `13-*` |
| 14 | scratch | inline scanner probes | 0 | non-string input and empty source are completed rejections; `this.` and qualified writes accepted in `ShowObjectives`; real allowlist errors recovered | `14-scanner-behaviours.txt` |
| 15 | clone | full probe on `69b41605`, in memory | 0 | accepted | `15-probe-on-feature-tip.txt` |
| 16 | e2e-clone | `check_baseline.py --rotation-state post-fix --instruction-state post-fix` with mutated files overlaid | see right | control **0** 10/0/2; R03 **1** 9/1/2; no constructor **1** 9/1/2; X-F2-16 **0** 10/0/2; X-F1-24 **0** 10/0/2; X-F1-01 **0** 10/0/2; X-F2-27 **0** 10/0/2; X-F2-21 **0** 10/0/2; all with stderr empty | `16-e2e-*` |

## SDK evidence

The checker was invoked by absolute path from `$ROOT/sdk8/cwd/deeper`, beneath `$ROOT/sdk8/global.json` (`{"sdk":{"version":"8.0.133","rollForward":"disable"}}`).

- **`dotnet --version` from that cwd: 8.0.133.** `dotnet --info`: commit `62e6bab6ed`, base path `/usr/lib/dotnet/sdk/8.0.133/`. Installed: 8.0.133 and 10.0.114.
- **The checker's own `dotnet`, from strace:** `/usr/bin/dotnet run --project …/check.csproj --verbosity quiet`, launched by `check_baseline.py`.
- **hostfxr's trace of that run:** probes `…/sdk8/cwd/deeper/global.json`, then `…/sdk8/cwd/global.json`, then `Found global.json […/sdk8/global.json]`, `Resolving SDKs with version = '8.0.133', rollForward = 'disable'`, `Using .NET SDK dll=[/usr/lib/dotnet/sdk/8.0.133/dotnet.dll]`.
- **Files opened by the process tree:** 263 successful opens under `sdk/8.0.133`, including `dotnet.dll`, `MSBuild.dll`, `Sdk.props`, `Microsoft.NET.Sdk.targets` and the compile task `Roslyn/Microsoft.Build.Tasks.CodeAnalysis.dll`. Zero opens or mentions of `sdk/10.0.114`.
- **Real production code, no mock:** the generated project compiles exactly `RackGeometry.cs` and `RackEditing.cs` from the clone plus the driver (`check_baseline.py:139-144`), and the driver defines no production type.
- **Control proves the trace discriminates:** from a cwd with no `global.json`, `dotnet --version` is 10.0.114, 538 opens under SDK 10 and none under SDK 8.
- **Output** is byte-identical across default, pinned, traced and control runs (sha256 `8cc51e4e39db0318eb584f4b7b1d571684a1a481de4e8f88749f546272f68fd0`).

Limit: the C# compile itself ran in an 8.0.133 `VBCSCompiler` server outside the traced tree, so strace shows the SDK 8 compile task but no compiler opens, and the production-file opens in the trace are Python's literal inventory, not the compiler's. Trace artefact: `strace -f` waits for any compile server it spawns, so the control run sat until I stopped that one server. Its `TracerPid` matched my strace and it started in my run; the checker itself had finished, and no other lane's server was touched.

**Six rows from the pinned run (no mocks):**

```text
SOURCE-STATIC PASS SnapFar near-angle enabled=True input=(20.000,0.700) far=(20.012,0.000) angle=0.000 angleSnapped=True boundarySnapped=False
SOURCE-STATIC PASS SnapFar near-angle-disabled enabled=False input=(20.000,0.700) far=(20.000,0.700) angle=2.005 angleSnapped=False boundarySnapped=False
SOURCE-STATIC PASS SnapFar outside-angle-window enabled=True input=(20.000,3.000) far=(20.000,3.000) angle=8.531 angleSnapped=False boundarySnapped=False
SOURCE-STATIC PASS SnapFar near-boundary enabled=True input=(39.000,0.000) far=(40.000,0.000) angle=0.000 angleSnapped=False boundarySnapped=True
SOURCE-STATIC PASS SnapFar near-boundary-disabled enabled=False input=(39.000,0.000) far=(39.000,0.000) angle=0.000 angleSnapped=False boundarySnapped=False
SOURCE-STATIC PASS SnapFar outside-boundary-window enabled=True input=(37.000,0.000) far=(37.000,0.000) angle=0.000 angleSnapped=False boundarySnapped=False
```

## Protected-file audit

| Check | Observed |
|---|---|
| `git diff --name-status f294fe07..1585ca20` | Exactly five paths: handoff (A); README, `check_baseline.py`, `objective_cta_source_check.py`, `test_objective_cta_source_check.py` (M) |
| `Assets`, `Packages`, `ProjectSettings` | 0 files changed |
| `baseline-output.txt` | 0 changes; blob `db6a8b850dce7efadaac6fe1b87b1dbba469428f`, sha256 `513cddd9babddaede82212f8e4891ecfcf9823c13a69474a3665f890a7f05dc1` |
| `BASE` | `72b4efbd1c653fa2231f632a79030a83ff76f833` unchanged in `check_baseline.py`, same value added in the test file |
| Tests 01 to 07, 09, 06a to c | AST comparison: every helper function identical, module-level statements identical, and of 47 top-level statements in `check()` only the test-08 `try` differs. Diff hunks at lines 255, 260 and 263 only. |
| Whitespace | `git diff --check f294fe07..1585ca20` exit 0 |
| Commits | `040e496a` code, `1585ca20` handoff only |
| README, #301 | README accurate on conditional CTA, exit semantics, SDK and baseline-output; **not** on lexical limits (RR-4). `BASELINE-CTA-CHECKER-QUALIFICATION-HANDOFF.md` not in the diff. |

## Limitations: claimed and additional

**Author's stated limits, checked against the code:**

| Claim | Result |
|---|---|
| Not a C# parser or compiler | True |
| Interpolated and C# 11 raw strings not analysed | True, and fails closed in direct shapes (X-F1-07 to 09). Understated: interpolation can desynchronise the lexer and hide statements (X-F1-29, 30). |
| `partial` members in other files, USS, other classes | True (X-F1-05, 06). USS not tested here. |
| Runtime mutation from outside the checked methods | True, and broader than it sounds: includes methods in the two files the scanner does read. |
| Helper "that receives only `forest`" | Mis-stated (RR-4) |
| Method other than the constructor, e.g. `Refresh` | True (X-F1-03, 04); handoff only, not in the README |
| Pinned counts describe the reviewed routing | True. Additional: counts are not identity, because a compensated edit keeps the count (X-F2-08, 10). |

**Additional limits found, not stated by the author:**

1. Qualified references (`this.x`, `Type.x`) are invisible to write and reference counting.
2. Tuple deconstruction is not counted as a write.
3. Anchors prove text presence and order, not execution: a pinned statement inside `if (false)`, a never-invoked lambda, or after `goto` / `throw` still counts.
4. Method attributes are not examined, so `[Conditional]` removes a method's calls with no `#if`.
5. Local functions can shadow a checked method.
6. Parameters and locals not listed in the write guards can be overwritten (`objectives`, `element`, `consent`).
7. `+=` on `.name` is not seen as a name write, and writes through a cast or alias are not attributed to the real button.
8. Nothing inventories `btn-primary` outside the constructor and the pinned lines.
9. Row-level access (`buttons[0]`, `buttons.Clear()`) reaches the map without using the map's variable.
10. Callers of `RefreshNextActionGuidance` and the body of `ShowMap` are not checked.
11. Allowlist failures are reported as a header failure (RR-5).

## NOT RUN

**Not run, and not claimed:** Unity Editor, tests or compile; rendering and screenshots; native pointer or keyboard input; Help-close timing; save/load, Legacy40 and Reference scenarios; Windows or Player build; the exact combined #296/#303 candidate (only an in-memory scan of its view and root); USS; any integration or merge.

Runtime effects described for mutants are code reasoning. Roslyn compiled them; nothing executed them.

## Final state

Report lane: this one file added on `docs/baseline-cta-checker-f1-f3-re-review-20261106-v1`; the commit is on the pushed branch and its hash is in the #305 review-complete comment, because a file cannot contain its own commit hash. Source lane unchanged at `1585ca20`. Throwaway clones and evidence remain under `/media/jer/Files/CCF/Verification/CTA-F1F3-Review-304-20261106/` (`source-clone` 5.0 GiB, `old-baseline-clone` 1.2 GiB plus `e2e-clone`, evidence about 8 MiB); the clones can be deleted once the Manager has ruled, and the evidence should stay. Unrelated local changes already present in `/media/jer/Files/CCF-Primary` (`AGENTS.md`, `Docs/Operations/StorageArchitecture.md`) were not touched.
