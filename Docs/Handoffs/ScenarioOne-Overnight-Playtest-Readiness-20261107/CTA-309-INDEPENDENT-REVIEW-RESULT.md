# #314 — Independent re-review of the #309 CTA verifier RR-1 to RR-5 repair (source scope only)

**Reviewer verdict: NEEDS CHANGES (source scope only).** This is reviewer evidence and opinion for Manager #200. It is **not** Manager acceptance and cannot approve the checker. No source was changed. Claim [#314](https://github.com/jermur788/CCF/issues/314), orchestrator [#313](https://github.com/jermur788/CCF/issues/313), author claim [#309](https://github.com/jermur788/CCF/issues/309).

> **ROUTING NOTE — read this first.** Tonight's OpenAI-family capacity is exhausted (native Codex/ChatGPT account limit until 2026-11-08 21:15; the OpenCode-delivered `openai/gpt-6.1-sol` then also hit its usage limit). **Cross-provider review was UNAVAILABLE.** This review is the **same-provider-family fallback**: Anthropic family like the #309 author, but a different model (Sonnet 5.5 here; the author was Opus 5.5), a different provider instance (Claude B - Pro; the author ran on Claude A) and a fresh session. It is the same disclosed-fallback pattern as #305. A cross-provider reviewer might still find things this model family misses. Manager #200 may discount the independence accordingly.

## Summary

**The repair is real and large, and everything the packet asked me to reproduce did reproduce.** All 81 #305 mutants are rejected, each for a plausible cause (I re-ran them with my own driver from the #305 reviewer's original script, not the author's copy). The 21 R-IDs, the original 20, the #304 author's 35 and the 19 benign controls behave as claimed. The committed suite exits 0 with `rejected=219/219 false_greens=0 wrong_cause=0` and `benign 32/32`. The candidate gives `10 PASS / 0 FAIL / 2 NOT-TESTABLE`, exit 0, on a **trace-proved** .NET SDK 8.0.133 (281 file opens under `sdk/8.0.133`, 0 under `sdk/10.0.114`). The historical baseline gives `6 / 4 / 2`, exit 1, with `baseline-output.txt` untouched. The diff is exactly the five allowed paths. No control was weakened, hidden, skipped or rebaselined.

**It is not yet a closed verifier.** I wrote **176 further cases of my own** (six rounds). Of the 129 I expected to be rejected, 120 are valid C# 9 mutants (Roslyn, 0 errors) and **29 of those 120 are accepted in memory; 26 are still accepted end to end** through the real `check_baseline.py` (`PASS=10 FAIL=0 NOT-TESTABLE=2`, exit 0). They fall into three findings (S-2 has two sub-classes), none of which the 81-mutant corpus or the author's 62 new controls exercises:

| Family | What gets through | Cases (end-to-end green) | Severity (my view) |
|---|---|---|---|
| **S-1** | RR-2 "related state" is still a **count-only signature**: tuple deconstruction and a compensated move defeat the write counts of `help`, `manager`, `map`, `workPlan`, `review`, `rackLayout`, `screen` | 10 | Medium, blocking by the packet's rule |
| **S-2** | **Checker/compiler disagreement (N-4)**: Unicode format characters (ZWJ, ZWSP, soft hyphen) are ignored inside C# identifiers; U+0085/U+2028/U+2029 end a `//` comment for the compiler but not for the lexer. This hides *any* guarded statement, including **a second static primary** | 11 (8 + 3) | Medium, blocking by the packet's rule |
| **S-3** | The pinned routing's **inputs are not pinned** (`NeedsAnnualReview`, `TeachingRacks`, `CurrentTopicIndex`); same effect as the closed X-F2-11 to 15 retargets. Not disclosed; the README sentence "only for an unconfirmed Rack V2 Objective 1" is stronger than what is checked | 5 (4 meaningful) | Medium for accuracy, Low for safety |

Lone CR as a comment terminator is also accepted *in memory* (3 cases) but is neutralised in the real pipeline, because `Path.read_text` decodes with universal newlines. I count it as closed by accident, not as a false green.

**Recommendation:** one more strictly bounded serial repair under the same five-path limit, then an exact-head re-review. A scratch wrapper of five small guards (never shipped, not a repair) closed **25 of the 26** end-to-end cases (the 26th, `W-DEP-05`, is game-progress logic outside any reasonable scope), rejected none of my 21 benign cases or the author's 24 benign controls, and reopened none of the author's 164 mutation controls. See [Is another repair warranted?](#is-another-repair-warranted). If the Manager prefers disclosure to code for S-2, S-1 and S-3 still need either a guard or an explicit README limit; today the README says the opposite.

**This is about the checker, not the game.** The live Objective UI source still passes, nothing here shows a production defect, and every mutant is a hypothetical future edit. Nothing in this review is Unity, render, input, focus or runtime evidence.

## Reviewer identity and provenance

| Item | Value |
|---|---|
| Role | T3 delegate reviewer for #314 |
| Provider / model / effort | `claudeAgent_claude_b_pro` (Claude B - Pro), `claude-sonnet-5-5`, effort **XHIGH** (set by the Mimo orchestrator) |
| Independence limitation | Same provider family as the #309 author (Opus 5.5 on Claude A) and as the #305 reviewer; see the routing note above |
| What I read before writing any case | The #309 contract, the review packet, the Manager ruling (incl. the N-1 ruling), the #305 report, the author's handoff, the candidate checker, test file and README in full |
| How my cases were made | Written after reading the checker source (white-box), in six rounds (`my_mutations*.py`). I had not read the *bodies* of the author's `probe_extra.py` when I wrote round 1; I read its anchor header and later its result log |
| Authority | Review only. I edited no checker, test, fixture or production file. I hold **no Unity lease and ran no Unity** |

## Lanes, pins and preflight

In this report `$E` means `/media/jer/Files/CCF/Verification/CTA-314-IndependentReview-20261107`.

| Item | Observed |
|---|---|
| Candidate | `verify/scenario-one-feedback-cta-anchor-20261106` at `b4d47d3d45717be9da159f0368407b10c0c3e6fe` (docs-only handoff commit); code commit `84d0ba92c763f4b1e2542f4e1b31240882960187`; parent `1585ca20df2b921d10e79f9aac89f03202d9258e` (the #305-reviewed candidate). `refs/heads/verify/...` in the bare store and the author lane's `HEAD` both read `b4d47d3d` at start and at end |
| Execution lane (mine, throwaway) | `$E/source-clone`, detached `b4d47d3d`; mutated files are overlaid only in memory or in `$E/e2e-clone`; both ended clean (one stray `.pyc` from a scratch import appeared in `source-clone` and was removed; tracked files were never modified) |
| Historical baseline lane (mine) | `$E/old-baseline-clone`, sparse shared clone detached at `72b4efbd`, the three amended scripts copied in untracked (the same method as #305 and #309) |
| Docs lane (mine) | `/media/jer/Files/CCF/Operations/CCF-MimoCta314Review-20261107`, new branch `docs/mimo-cta-309-independent-review-20261107-v1` from `origin/main` `b5c89dfc`. The branch and path did not exist at preflight |
| Read-only to me | `/media/jer/Files/CCF/Operations/CCF-FeedbackCTARepair-20261106` (`rev-parse` only; I deliberately did not run `git status` there), the author's evidence dir, the #305 evidence dir, the Mimo worktree |
| One disclosed touch outside my lanes | At preflight I ran `git fetch origin` once from `/media/jer/Files/CCF-Primary` (updates remote-tracking refs only; no working-tree change). I touched nothing else there |
| Storage | `$E = /media/jer/Files/CCF/Verification/CTA-314-IndependentReview-20261107` on Files (ext4), about 52 GiB free at start; all logs and matrices are there |
| Toolchain | Python 3.12.3; `dotnet` SDKs 8.0.133 and 10.0.114 both installed; `strace` present; no `dotnet` or Unity process of anyone else observed at start |

## Results by review-packet area

| # | Packet area | Result | Basis |
|---|---|---|---|
| 1 | Diff reconciliation, byte-identity of preserved controls, `check_baseline.py` and README justification | **PASS** | Section [Diff and preserved controls](#diff-and-preserved-controls) |
| 2 | Re-run of the 81 #305 mutants, the older controls and benign sets (reproduction) | **PASS** | 102/102 broken rejected, 15/15 benign accepted; reasons checked |
| 2 | **My own independent mutations** (the core of the review) | **FAIL** | 26 compile-clean defects accepted end to end: S-1, S-2, S-3 |
| 3 | Real execution tallies, SDK 8.0.133 trace, `py_compile`, reporter tallies | **PASS** | Section [SDK evidence](#sdk-evidence) |
| 4 | UI contract from checker logic | **PASS with one caveat** | Section [UI contract](#ui-contract-verified-from-the-checker-logic); the caveat is S-2's hidden second primary |
| 5 | Overclaim audit of handoff and README | **PARTIAL FAIL** | Section [Overclaim audit](#overclaim-audit) |
| 6 | Old baseline red preserved, scope integrity, checker unedited during review | **PASS** | `evidence/11-final-integrity.txt` |

## Findings (severity-ranked)

Severities are my recommendation; the Manager rules. "Blocking by the packet's rule" means the packet says any surviving covered mutation is a blocking finding. Every case ID below is in the [tables](#my-176-cases). All case sources are in `$E/scripts/my_mutations*.py`; per-case results are in `$E/mine*/results.json`; C# 9 compile results are in `$E/evidence/04-roslyn-*`.

### S-1 — Medium, blocking by the packet's rule: RR-2 related state is still a count-only signature

- **Where.** `Tools/Verification/OvernightPlaytestFeedback/objective_cta_source_check.py:107-114` (`_assignments` counts `x =`, `x op=`, `x++` and `ref`/`out x`; a deconstruction target is not a match) and `:1610-1616` (the loop that pins `help`, `manager`, `learning`, `map`, `workPlan`, `review`, `rackLayout` to 1 write and `screen` to 3).
- **Cases (all valid C# 9, all end-to-end `10/0/2`, exit 0).**
  - `Z-RR2-T-help`, `-screen`, `-rackLayout`, `-map`, `-workPlan`, `-review`, `U-RR2-T-manager`: `(help, workPlanWasOpen) = (null, false);` appended to `ShowObjectives`. For `help`, the guidance then reads `help == null` and is suspended forever (the effect of the rejected `RR2-10`, reached by one tuple).
  - `Z-RR2-C-screen`: TryShowReviewYear's `screen = UiScreen.Review;` removed and `ShowObjectives` made to end with `screen = UiScreen.Map;`. The count stays 3, and the Objectives screen now resolves guidance against `map.Root`.
  - `Z-RR2-C-help`: `help = new MenuHelpView(this)` becomes `help = null` (count still 1). `Y-RR2-08`: `screen = UiScreen.Review` becomes `screen = UiScreen.None`.
- **Contract link.** #309 RR-2 names "tuple reassignment, count-compensated retargeting" and says "not count-only signatures". The author closed both for the guarded names (`U-RR2-T-helpClosedFrame-ctl` and `U-RR2-T-learning-ctl` are rejected) and for the pinned methods, but these seven fields keep only a number.
- **Suggested narrow fix (same file).** Treat each reviewed write as a pinned snippet exactly as `_ROOT_SNIPPETS` already does (`screen = UiScreen.Review;`, `help = new MenuHelpView(this);`, and so on) and require every `name =` position to lie inside a pinned body or a snippet; add a deconstruction regex `\((?:[^()]*,)?\s*(?:this\s*\.\s*)?NAME\s*(?:,[^()]*)?\)\s*=(?![=>])`. Scratch result: closes all ten cases.

### S-2 — Medium, blocking by the packet's rule: the lexer and the C# compiler disagree on two kinds of character (N-4)

Both sub-classes are valid C# 9 (Roslyn 0 errors) and defeat every guard that reads identifiers, positions or comments.

- **(a) Unicode format characters (category Cf) are ignored inside identifiers by the C# compiler**, so `fo<U+200D>rest` *is* `forest`. The checker's identifier handling (`\w` regexes, `_TOKEN` at `:857`, `_positions` at `:978-981`) sees a different token. Cases: `Y-N-11` (`forest.name += "-x"`), `Z-N-SHY` (soft hyphen), `Z-N-CF-buttons` (header row cleared), `Z-N-CF-allowlist-clear` (`Array.Clear` on the allowlist), `Y-N-14`, `Y-N-15` (guarded-state writes), and **`Z-N-CF-secondprimary`**: `scroll.Add(Ui<ZWJ>Kit.Button("Close", () => ui.CloseAll(), true, "btn-" + "primary"))` is **a second static primary**, the N-1/RR-1 defect itself, and gets `10/0/2` exit 0. Its control without the character (`Z-N-CF-secondprimary-ctl`) is rejected. `Y-N-12`: a `partial class Learning<ZWJ>ObjectivesView` file is not even collected as a companion, because `_COMPANION_HINT` (`:596-597`) searches the raw text.
- **(b) U+0085, U+2028 and U+2029 end a `//` comment (and a directive line) for the compiler, but the lexer looks for `\n` only** (`:658`, `:669`). Cases: `Y-N-10`, `Y-N-13`, `Z-N-PS` hide `Root.SetEnabled(false)` after a comment. A lone CR does the same, but `Path.read_text` converts it to `\n` before lexing, so `Y-N-09`, `Z-N-CR-region` and `Z-N-CR-root` are rejected end to end even though the in-memory API accepts them. That is closed by an accident of the file reader, not by a rule.
- **Likelihood and materiality.** These need invisible characters and are very unlikely by accident, which is why I rank them Medium and not higher. They still matter for three reasons. The packet counts any surviving covered mutation as blocking. The README and handoff say that `\u`-escaped identifiers, interpolation holes and raw strings fail closed, so a reader would reasonably believe identifier obfuscation is closed. And the handoff's own "Not proved: that the lexer matches Roslyn on every input" names the general class but not these two concrete ones; the README is silent.
- **Suggested narrow fix.** In `_lexed` (the one entry point for every checked file): reject U+0085, U+2028 and U+2029 anywhere, and reject any Cf character in code (outside strings and comments), allowing a leading BOM. In `collect_companions`, test `_COMPANION_HINT` on the text with Cf stripped. I scanned all 198 live `Assets/**/*.cs` files: **none contains a Cf character or any non-ASCII character in code** (`evidence/08-lexer-live-assets.txt`), so this cannot reject today's source.

### S-3 — Medium for accuracy, Low for safety: the pinned routing reads unpinned inputs, and the README does not say so

- **Cases (end-to-end `10/0/2`, exit 0).** `W-DEP-01`: `public bool NeedsAnnualReview => true;` (`ScenarioOneUiRoot.cs:49`). `W-DEP-02`: `TeachingRacks => false` (`LearningObjectivesView.cs:24`). `W-DEP-03`: `CurrentTopicIndex => 1` (`:23`). `W-DEP-04`: `NeedsAnnualReview` routed through a helper that returns true. Each sends the pinned chain down another branch. The identical effect written *inside* the chain is rejected (X-F2-12 "annual-review branch forced, map target unreachable", X-F2-13, X-F2-14). `W-DEP-05` (`ReloadProgress` marks every lesson done) is game-progress logic and I do not count it as a checker defect.
- **Adjacent unpinned routing.** `Y-RR3-17` (`ShowObjectives` opens the map) and `Y-RR3-18` (`CloseAll` no longer clears the screen) are accepted. This is the #305 "N-2 unpinned logic" class; the Manager's ruling asks that anything unproved be *stated*, and the README does not.
- **Overclaim.** The README says the map is "selected as target exactly once, only for an unconfirmed Rack V2 Objective 1". That holds for the text of the routing chain, not for the inputs it reads.
- **Suggested narrow fix.** Three one-line `_snippet_spans` pins (`NeedsAnnualReview`, `TeachingRacks`, `CurrentTopicIndex`), or an explicit README limit naming them and `ShowObjectives`/`CloseAll`.

### S-4 — Low: the limit probes are not in the repository

The README says each known limit "has an accepted probe in the #309 handoff" and cites probe IDs L-01 to L-07 and O-01 to O-04. The probes are real: I re-ran my own versions (`Y-N-22` to `Y-N-25` for L-01/L-02, `V-L03` for L-03, `V-L04` for L-04, `V-L06-1/2` for L-06, `V-L07` for L-07, `V-L01-1`) and every limit behaves as documented (accepted, compile-clean). But the committed suite contains **no** limit or over-rejection probe (`grep` finds none in `test_objective_cta_source_check.py`), the handoff holds only a one-line table row per probe, and the probe source lives in the author's off-repo evidence (`.../CTA-RR1RR4-Repair-309-20261107/scripts/probe_extra.py`). A later maintainer cannot re-run them from the repo, and nothing asserts that a limit stays disclosed. Suggest committing them as a separate "accepted limits" group that is *reported but not counted as mutations*, or rewording the README.

### S-5 — Info: disclosed limit L-06 is the most ordinary-looking one

`V-L06-1/2`: a companion class that writes `var v = ui.Learning; v.Root.Query<Button>().ForEach(b => b.AddToClassList("btn-primary"))` (or `SetEnabled(false)`) is accepted, because the companion rule only matches the literal text `Learning.Root`. The author discloses it as L-06, and I could not find a lexical rule that closes it without banning legitimate `ui.Learning.X` uses (the walking HUD calls `ui.Learning.HudModel()`). I record it so the Manager sees it is one local variable, not an exotic shape.

### S-6 — Info: maintenance cost and message quality

By design, any edit to a pinned method, a reviewed caption or a pinned UiKit member fails check 08 until the scanner is updated. I confirmed the documented over-rejections (O-01 to O-04) and added: a caption copy tweak (`Z-B-20`, `Z-B-21`: message "header button '...' must be declared exactly once; found 0", which does not say "caption changed"), a reason-text edit inside a pinned method (`Z-B-22`), any `btn-primary` text in a `Debug.Log` in the view class (`Y-B-10`), `.Q<>` in a view partial (`Y-B-11`) and an editor test that names a header button (`Y-B-12`). All fail closed, which is the stated trade-off ("any edit to a pinned method fails until its reviewed text … is updated"; the three reviewed captions are described in the README's header-grammar bullet). I found **no undisclosed over-rejection among my 21 benign cases** (21/21 accepted). Informational: the candidate accepts the in-flight feature tip `9789369d` and #296's `69b41605` in memory (`evidence/07-tip-probe.txt`), so the pinned text does not obviously block the next integration.

## Is another repair warranted?

**Yes, one, strictly bounded, then an exact-head re-review.** The evidence that it is small and safe is a scratch wrapper (`$E/scripts/fix_feasibility.py`; reviewer scratch, **not approved code**, not a repair) that runs after the unchanged candidate:

| Guard in the scratch wrapper | Closes |
|---|---|
| Reject U+0085/U+2028/U+2029 anywhere; reject Cf characters in code; strip Cf before the companion-hint test | S-2 (11 end-to-end cases) |
| Deconstruction-write regex plus pinned write sites for the seven fields | S-1 (10 cases) |
| Three one-line routing-input pins | S-3 (`W-DEP-01` to `04`) |

Result (`evidence/09-fix-feasibility-v3.txt`, `-manager.txt`): live source accepted; **24 of the 28 in-memory false greens closed**, the remaining four being the three lone-CR cases (already closed in the real pipeline) and `W-DEP-05` (out of scope); `U-RR2-T-manager` (added later) is also closed, so **25 of the 26 end-to-end cases are closed**; **0 of my 21 benign cases rejected** (the first version, which banned all non-ASCII in code, wrongly rejected a no-break space and `größe`; the Cf-only version does not); **0 of the author's 24 benign controls rejected**; **0 of the author's 164 mutation controls reopened**. The wrapper does not close S-5 (L-06). Caveat: I fitted it after seeing the cases, so it shows the cases can be closed locally, not that the result is complete; nobody has attacked it.

If the Manager would rather not spend a repair, S-2 could be accepted as a *disclosed* residual (it needs invisible characters), but S-1 and S-3 then still need either a guard or an explicit README limit, because today the README states the opposite. Whichever route is taken, I would add my cases as regression controls (the sources are in `$E/scripts/my_mutations*.py`) and require them to be rejected *for their stated cause*, as #309 did for the #305 corpus.

## UI contract verified from the checker logic

Read from the code, not the README. Each item names the rule and a case that exercises it.

| Contract | Rule in `objective_cta_source_check.py` | Exercised by |
|---|---|---|
| Exactly one named **Back to forest** static primary in the reviewed default header | `_KNOWN_HEADER_BUTTONS` third tuple (`:1261-1265`) is the only `primary=True`; `_guard_header` (`:1529-1546`) requires exactly one class containing `btn-primary`, `ui.CloseAll()`, enabled `true`, name `objective-back-to-forest`, added to the row once; `_guard_view_class` (`:1403-1406`) requires exactly **one** `btn-primary` literal across the view class and every partial | `RR1-10` (none left), `R05`, `RR1-08`, `X-F1-31`, `X-F1-32`, `Y-RR1-10` to `13`, `Z-RR1-07` to `09` (a second one) all rejected |
| The Stand Map CTA is never statically primary | The map button must carry **no** classes at all (`not info["classes"]`, `:1536`), plus the legacy `_audit_header` map rule (`:308-309`) and the class-list API ban across the view class (`_VIEW_BANS`, `:1270-1278`) | `R01`, `R02`, `Y-RR1-28`, `X-F1-10` to `15` rejected |
| Its name stays demoted through the scoped `GuidanceButtonNames` | `_allowlist_values` (`:198-216`) requires the CTA name exactly once, no duplicates, only plain literals, and exactly two references; `_guard_header` requires the Back to forest name to be a member (`:1543-1545`); the demotion loop is part of the pinned `RefreshNextActionGuidance` text | `R03`, `R04`, `X-F1-27`, `Y-RR1-14` to `16`, `Y-RR4-01`, `02`, `04`, `05`, `06` rejected |
| The map is primary only dynamically | `ApplyNextActionGuidance` and the routing chain are token-pinned (`_ROOT_PINS`, `:992-1179`) | `X-F2-08` to `26` rejected |
| No new static primary by implementation accident | Every `UiKit` call must be `UiKit.Member` with plain-literal class arguments and `btn-primary` only at the one reviewed call (`_check_uikit_calls`, `:1337-1371`); extras in the header must be non-primary, added once, with a new caption and a name outside the allowlist (`:1547-1558`) | `Y-RR1-10` to `13`, `Z-RR1-07` to `09` rejected. **Caveat: `Z-N-CF-secondprimary` gets through (S-2)** |

**The N-1 ruling is enforced, not replaced by an invented policy.** The ruling asks for exactly one existing named static primary and no new competing named static primaries, even when named in the demotion list. The checker implements exactly that: `X-F1-31` and `X-F1-32` are now rejected, and extra *non-primary* header buttons are still accepted (`B-X-07`, `Y-B-18`, `Z-B-24`). It is slightly stricter than the ruling in two documented ways (it pins the caption, click target and enabled argument of all three reviewed buttons, and it counts `btn-primary` across the whole view class, not only the header), both consistent with "freeze the accepted current UI". The README says so ("it is not a UI policy").

**Not claimed, correctly.** Player focus, consent, native click, rendered prominence, Help-close timing and input remain future Unity tests. The `check_baseline.py` PASS note says "Stateful Unity input, focus and rendered prominence remain NOT TESTED", and the handoff lists them under NOT RUN. I found no place where a source check is relabelled as runtime or Unity evidence.

## Overclaim audit

| Statement (where) | Verdict | Evidence |
|---|---|---|
| Handoff: "All 81 broken-source mutations … are now rejected … each for a stated cause" | **True** | My driver: 81/81 (and 102/102 with the R-IDs) |
| Handoff: "Candidate 10/0/2 exit 0; historical 6/4/2 exit 1; six SnapFar on traced, cwd-pinned SDK 8.0.133" | **True** | [SDK evidence](#sdk-evidence) |
| Handoff: "original 20, #304 55 and 4 benign byte-identical"; "#305 corpus byte-identical" | **True** | [Diff and preserved controls](#diff-and-preserved-controls) |
| Handoff: "55 of my 62 new mutations were also accepted by `1585ca20`" | **True** | I re-ran the author's 62 controls against the `1585ca20` scanner: RR-1 26/28, RR-2 11/15, RR-3 13/14, N-4 5/5 = 55 of 62 |
| Handoff: "The 198 live files lex without error" | **True** | `evidence/08-lexer-live-assets.txt`: 198 OK, 0 failed |
| Handoff: "It fails closed on raw strings, `\u` identifiers (any backslash in code) …" | True as written, but implies identifier obfuscation is closed | S-2(a) |
| Handoff: "Not proved: that the lexer matches Roslyn on every input" | **True, honest, but generic** | S-2 gives two concrete classes |
| README: "A qualified, tuple, `ref`/`out`, lambda, shadowing, `nameof` or count-compensated reference therefore fails" | True for the nine guarded identifiers; **not** for the seven related-state fields, which "keep their reviewed number of writes" | S-1 |
| README: "the map selected as target exactly once, only for an unconfirmed Rack V2 Objective 1" | **Overstated**: true of the chain text only | S-3 |
| README: "Known limits … each has an accepted probe in the #309 handoff" | **Partly**: the probes ran and are described, but they are not in the repository or the suite | S-4 |
| README/handoff: lexer "separates comments, preprocessor lines and string/char literals" | True for the C# lexical grammar the checker implements; omits the Cf/line-separator mismatch | S-2 |
| Handoff limits L-01, L-02, L-03, L-04, L-06, L-07 and O-01 to O-04 | **Real**: each reproduced as documented (L-xx accepted, O-xx rejected) | `Y-N-22` to `25`, `V-L03`, `V-L04`, `V-L06-1/2`, `V-L07`, `Z-N-nested-interp-view`, `Z-B-22` |
| Anywhere: source checks relabelled as runtime/Unity | **Not found** | handoff NOT RUN list; PASS note wording |

Is a remaining false-green class material? In my judgment S-1 and S-2 are *narrow, cheap and in contract scope* but need unusual syntax or invisible characters, so the practical risk from an accidental edit is low; they are blocking under the packet's own rule and because the docs imply they are closed. S-3 is the more realistic gap for a future refactor and the only one the README actively contradicts.

## Diff and preserved controls

| Check | Observed (`evidence/01-diff-reconciliation.txt`, `01b-checkbaseline-readme-diff.txt`) |
|---|---|
| Changed paths `1585ca20..b4d47d3d` | Exactly five: the new handoff (A) and `README.md`, `check_baseline.py`, `objective_cta_source_check.py`, `test_objective_cta_source_check.py` (M). Numstat 511/0, 10/7, 4/4, 1120/14, 593/20 matches the handoff |
| Commits | `84d0ba92` (code) and `b4d47d3d` (handoff only); `1585ca20` is the parent; no merge; no PR #300 ancestry |
| `Assets`, `Packages`, `ProjectSettings` | 0 files changed. Scenes, saves and fixtures not touched |
| `baseline-output.txt` | Blob `db6a8b850dce7efadaac6fe1b87b1dbba469428f`, sha256 `513cddd9babddaede82212f8e4891ecfcf9823c13a69474a3665f890a7f05dc1` at `1585ca20`, at `b4d47d3d` and in my working copy |
| `BASE` | `72b4efbd1c653fa2231f632a79030a83ff76f833` unchanged in `check_baseline.py` |
| `git diff --check` | exit 0 |
| `check_baseline.py` (+4/−4) | Justified. The hunks are the import line, the `collect_companions(ROOT)` argument to the call, and the two lines of the PASS note that stopped claiming the superseded "every static source in the demotion set" rule. Checks 01 to 07, 09 and 06a to c are untouched |
| Original 20, #304 F1 14, F2/F3 21, benign 4 | Lines 28 to 198 of the `1585ca20` test file occur **verbatim, once**, in the new file (sha256 prefix `6119b5fa`). The driver around them changed only to pass `collect_companions`; the original and #304 cases still pass if *any* rejection occurs (as before), so I re-ran them myself from the old file: **55/55 rejected, 4/4 benign accepted** (`evidence/05-orig55.stdout`) |
| #305 corpus | Lines 33 to 295 of the #305 `review_mutations.py` occur verbatim, once (sha256 prefix `9a7a8fd2`) |
| Skip, xfail, widened defaults | None found. `grep` shows "skip" only inside C# mutation text. The driver counts a wrong-cause rejection as a failure |
| README rewrite | Matches the mechanically verified scope except the points in the [overclaim audit](#overclaim-audit) |

## Mutation results

Totals. **Reproduction:** #305 corpus 102 broken (21 R-IDs, 35 X-F1, 46 X-F2) and 15 benign; original/#304 55 broken and 4 benign. **Mine:** 176 cases. The 1585ca20 scanner accepts 52 of the 81 (X-F1 17, X-F2 35): the **50 true false greens plus the 2 N-1 cases** `X-F1-31`/`X-F1-32`, which the Manager's N-1 ruling now requires to be rejected; the candidate rejects all 81. "Exit" below is the in-process verdict (a rejection is the completed FAIL of check 08); real process exits are in the end-to-end table.

| Group | Count | Rejected | Accepted | Notes |
|---|---|---|---|---|
| #305 R-IDs | 21 | 21 | 0 | 14 of them were accepted by 1585ca20 |
| #305 X-F1 | 35 | 35 | 0 | 17 accepted by 1585ca20 |
| #305 X-F2 | 46 | 46 | 0 | 35 accepted by 1585ca20 |
| #305 benign (incl. CRLF, BOM) | 15 | 0 | 15 | |
| Original 20 + #304 F1 14 + F2/F3 21 | 55 | 55 | 0 | |
| #304 benign | 4 | 0 | 4 | |
| **Mine: expected rejected** | 129 | 100 | **29** | 120 compile at C# 9; of those 29 of 120 are accepted |
| Mine: expected benign | 21 | 0 | 21 | 0 over-rejections |
| Mine: disclosed-limit probes | 14 | 1 | 13 | the one rejection (`Y-RR3-11`) is my first probe's own artefact: a `for (;;) {}` block joins the next statement and is rejected; `V-L03` (do/while) is accepted as disclosed |
| Mine: exploratory | 12 | 11 | 1 | fail-closed documented behaviours |

Roslyn (C# 9, the real Unity 6000.6 DLLs, Player-like defines, SDK 8.0.133 pinned by cwd, my own build of the #305 harness): baseline 189 files, 0 errors. #305 corpus: 117 rows, all 0 errors except `X-F1-09` (raw strings, not C# 9) and `X-F2-44` (`#if`'d out), exactly as #305 recorded. Original set: 59 rows, all 0 errors except `map-not-added-to-row` (CS0841, a pre-existing immutable control). Mine: 164 of 176 compile; the 12 that do not are my own authoring errors fixed in later rounds (`Y-RR1-07/08/09` became `Z-RR1-07/08/09`; `Y-N-04` became `Z-N-04`; `Y-B-11`, `Z-B-11`; `Y-N-21` dropped), and five deliberately broken diagnostic probes (`Y-RR4-03/07/08/09/10`). **All 29 false greens compile with 0 errors.** This is a compiler check, not a Unity compile.

### End-to-end through the real `check_baseline.py` (pinned SDK 8.0.133 cwd)

All 26 surviving cases, three lone-CR cases, a rejected control set and the diagnostics cases (`evidence/06-*`, `evidence/10-e2e-table.txt`, raw output in `$E/e2e-out/`):

| Case | check_baseline exit | Tally |
|---|---|---|
| R03 | 1 | PASS=9 FAIL=1 NOT-TESTABLE=2 |
| U-RR2-T-manager | 0 | PASS=10 FAIL=0 NOT-TESTABLE=2 |
| W-DEP-01 | 0 | PASS=10 FAIL=0 NOT-TESTABLE=2 |
| W-DEP-02 | 0 | PASS=10 FAIL=0 NOT-TESTABLE=2 |
| W-DEP-03 | 0 | PASS=10 FAIL=0 NOT-TESTABLE=2 |
| W-DEP-04 | 0 | PASS=10 FAIL=0 NOT-TESTABLE=2 |
| W-DEP-05 | 0 | PASS=10 FAIL=0 NOT-TESTABLE=2 |
| X-F1-24 | 1 | PASS=9 FAIL=1 NOT-TESTABLE=2 |
| X-F2-16 | 1 | PASS=9 FAIL=1 NOT-TESTABLE=2 |
| Y-N-09 | 1 | PASS=9 FAIL=1 NOT-TESTABLE=2 |
| Y-N-10 | 0 | PASS=10 FAIL=0 NOT-TESTABLE=2 |
| Y-N-11 | 0 | PASS=10 FAIL=0 NOT-TESTABLE=2 |
| Y-N-12 | 0 | PASS=10 FAIL=0 NOT-TESTABLE=2 |
| Y-N-13 | 0 | PASS=10 FAIL=0 NOT-TESTABLE=2 |
| Y-N-14 | 0 | PASS=10 FAIL=0 NOT-TESTABLE=2 |
| Y-N-15 | 0 | PASS=10 FAIL=0 NOT-TESTABLE=2 |
| Y-RR2-08 | 0 | PASS=10 FAIL=0 NOT-TESTABLE=2 |
| Y-RR4-05 | 1 | PASS=9 FAIL=1 NOT-TESTABLE=2 |
| Y-RR4-09 | 1 | PASS=9 FAIL=1 NOT-TESTABLE=2 |
| Z-N-CF-allowlist-clear | 0 | PASS=10 FAIL=0 NOT-TESTABLE=2 |
| Z-N-CF-buttons | 0 | PASS=10 FAIL=0 NOT-TESTABLE=2 |
| Z-N-CF-secondprimary | 0 | PASS=10 FAIL=0 NOT-TESTABLE=2 |
| Z-N-CF-secondprimary-ctl | 1 | PASS=9 FAIL=1 NOT-TESTABLE=2 |
| Z-N-CR-region | 1 | PASS=9 FAIL=1 NOT-TESTABLE=2 |
| Z-N-CR-root | 1 | PASS=9 FAIL=1 NOT-TESTABLE=2 |
| Z-N-PS | 0 | PASS=10 FAIL=0 NOT-TESTABLE=2 |
| Z-N-SHY | 0 | PASS=10 FAIL=0 NOT-TESTABLE=2 |
| Z-RR2-C-help | 0 | PASS=10 FAIL=0 NOT-TESTABLE=2 |
| Z-RR2-C-screen | 0 | PASS=10 FAIL=0 NOT-TESTABLE=2 |
| Z-RR2-T-help | 0 | PASS=10 FAIL=0 NOT-TESTABLE=2 |
| Z-RR2-T-map | 0 | PASS=10 FAIL=0 NOT-TESTABLE=2 |
| Z-RR2-T-rackLayout | 0 | PASS=10 FAIL=0 NOT-TESTABLE=2 |
| Z-RR2-T-review | 0 | PASS=10 FAIL=0 NOT-TESTABLE=2 |
| Z-RR2-T-screen | 0 | PASS=10 FAIL=0 NOT-TESTABLE=2 |
| Z-RR2-T-workPlan | 0 | PASS=10 FAIL=0 NOT-TESTABLE=2 |

### #305 R-IDs re-derived from #302 (21)

| ID | Mutation | 1585ca20 | b4d47d3d | C# 9 errors | Candidate rejection cause (abridged) |
|---|---|---|---|---|---|
| R01 | map statically primary | rejected | **REJECTED** | 0 | map CTA must not be unconditionally primary (AddToClassList) |
| R02 | inline primary on the map constructor | rejected | **REJECTED** | 0 | map CTA must not be unconditionally primary (UiKit.Button) |
| R03 | Back to forest dropped from the allowlist | rejected | **REJECTED** | 0 | static primary button 'objective-back-to-forest' is not in the GuidanceButtonNames scoped demotion set |
| R04 | Back to forest renamed out of the allowlist | rejected | **REJECTED** | 0 | static primary button 'objective-forest' is not in the GuidanceButtonNames scoped demotion set |
| R05 | new CloseAll primary | rejected | **REJECTED** | 0 | static primary 'close' must have exactly one literal name; found 0 |
| R06 | duplicate CTA name, no spaces | rejected | **REJECTED** | 0 | duplicate element name in the Objectives constructor |
| R07 | verbatim duplicate CTA name | rejected | **REJECTED** | 0 | duplicate element name in the Objectives constructor |
| R08 | later map rename | rejected | **REJECTED** | 0 | Objective CTA name must be unique and bound to the ShowMap button; found map names ['objective-primary-c… |
| R09 | guard moved into a string | rejected | **REJECTED** | 0 | ApplyNextActionGuidance/no styling if ineligible: expected one structural match, found 0 |
| R10 | guard commented out | rejected | **REJECTED** | 0 | ApplyNextActionGuidance/no styling if ineligible: expected one structural match, found 0 |
| R11 | target override to CloseAll before binding | rejected | **REJECTED** | 0 | target is reassigned after the routing chain |
| R12 | candidate nulled in Apply | rejected | **REJECTED** | 0 | ApplyNextActionGuidance writes 'candidate' more than once (override) |
| R13 | return after tracking clear | rejected | **REJECTED** | 0 | RefreshNextActionGuidance has an unexpected early return |
| R14 | primary class behind #if UNITY_EDITOR | rejected | **REJECTED** | 0 | \bprivate\s+void\s+ApplyNextActionGuidance\s*\(\s*\): conditional compilation (#if) in a checked method … |
| R15 | map disabled after binding | rejected | **REJECTED** | 0 | map CTA variable may only be declared, named and added to the header row; other uses are unsupported |
| R16 | final return made true | rejected | **REJECTED** | 0 | out-of-scope candidate rejected: expected one structural match, found 0 |
| R17 | guard/assign swapped | rejected | **REJECTED** | 0 | ApplyNextActionGuidance/set current candidate: invalid control-flow order |
| R18 | V2 gate negated | rejected | **REJECTED** | 0 | RefreshNextActionGuidance/rack teaching gate: expected one structural match, found 0 |
| R19 | objectives flag widened | rejected | **REJECTED** | 0 | RefreshNextActionGuidance/objectives-scoped branch: expected one structural match, found 0 |
| R20 | help-close setter removed | rejected | **REJECTED** | 0 | CloseHelp/help-close frame recorded: expected one structural match, found 0 |
| R21 | LateUpdate apply removed | rejected | **REJECTED** | 0 | LateUpdate applies cached guidance: expected one structural match, found 0 |

### #305 X-F1 (35)

`X-F1-31` and `X-F1-32` are the two N-1 cases (accepted by 1585ca20, now rejected by the ruling).

| ID | Mutation | 1585ca20 | b4d47d3d | C# 9 errors | Candidate rejection cause (abridged) |
|---|---|---|---|---|---|
| X-F1-01 | R05 via a helper that builds the primary (literal outside the constructor) | accepted | **REJECTED** | 0 | LearningObjectivesView.cs:83: header row addition of a button that is not a reviewed UiKit.Button declar… |
| X-F1-02 | R05 via a helper that promotes an existing button | accepted | **REJECTED** | 0 | LearningObjectivesView.cs:83: header local 'close' is used outside the reviewed straight-line header sha… |
| X-F1-03 | map made primary from a method outside the constructor, name split to dodge t… | accepted | **REJECTED** | 0 | LearningObjectivesView.cs:194: a class-list API (only the reviewed header may style a button) in the Obj… |
| X-F1-04 | unnamed extra primary created outside the constructor, appended to the header… | accepted | **REJECTED** | 0 | LearningObjectivesView.cs:194: element-tree traversal or query in the Objectives view class is unsupport… |
| X-F1-05 | partial class in another file promotes the map (scanner never reads that file) | accepted | **REJECTED** | 0 | LearningObjectivesView.Sequence.cs:9: a class-list API (only the reviewed header may style a button) in … |
| X-F1-06 | UiKit.Button itself makes every button primary (scanner never reads UiKit) | accepted | **REJECTED** | 0 | UiKit.cs:66: UiKit must not attach btn-primary itself (RR-1) |
| X-F1-07 | interpolated class name on the map | rejected | **REJECTED** | 0 | AddToClassList in the header uses a non-literal class name (ambiguous) |
| X-F1-08 | interpolated class name on an inline primary | rejected | **REJECTED** | 0 | UiKit.Button class argument in the header is not a plain string literal (ambiguous) |
| X-F1-09 | C# 11 raw string class name (not valid at Unity's C# level) | rejected | **REJECTED** | 1 (CS8773) | LearningObjectivesView.cs:75: the lexer cannot follow this source (raw string literal (C# 11) is unsuppo… |
| X-F1-10 | map primary inside an if | rejected | **REJECTED** | 0 | map CTA must not be unconditionally primary (AddToClassList) |
| X-F1-11 | map primary deferred in a lambda | rejected | **REJECTED** | 0 | map CTA must not be unconditionally primary (AddToClassList) |
| X-F1-12 | map primary by ToggleInClassList | rejected | **REJECTED** | 0 | map CTA must not be unconditionally primary (ToggleInClassList) |
| X-F1-13 | verbatim identifier @map | rejected | **REJECTED** | 0 | map CTA must not be unconditionally primary (AddToClassList) |
| X-F1-14 | unicode-escaped identifier | rejected | **REJECTED** | 0 | LearningObjectivesView.cs:75: the lexer cannot follow this source (backslash outside a literal (Unicode-… |
| X-F1-15 | escaped class literal \x | rejected | **REJECTED** | 0 | map CTA must not be unconditionally primary (AddToClassList) |
| X-F1-16 | new Button primary, no name | rejected | **REJECTED** | 0 | static primary 'close' must have exactly one literal name; found 0 |
| X-F1-17 | Q-chained primary | rejected | **REJECTED** | 0 | AddToClassList(btn-primary) on a non-simple receiver in the header (unnamed primary) |
| X-F1-18 | row indexer primary | rejected | **REJECTED** | 0 | AddToClassList(btn-primary) on a non-simple receiver in the header (unnamed primary) |
| X-F1-19 | two-step declaration of a primary | rejected | **REJECTED** | 0 | static btn-primary header button is not assigned to a named local (unnamed primary) |
| X-F1-20 | class assigned to the forest variable after declaration (reassignment) | rejected | **REJECTED** | 0 | static primary 'forest' must be declared once and never reassigned in the header |
| X-F1-21 | duplicate allowlist spelling (ordinary + verbatim) | rejected | **REJECTED** | 0 | GuidanceButtonNames contains a duplicate name |
| X-F1-22 | allowlist entry commented out (R03 with the text still present) | rejected | **REJECTED** | 0 | static primary button 'objective-back-to-forest' is not in the GuidanceButtonNames scoped demotion set |
| X-F1-23 | allowlist entry behind #if (R03 in a Player build) | rejected | **REJECTED** | 0 | GuidanceButtonNames: conditional compilation (#if) in a checked method or declaration is unsupported |
| X-F1-24 | R04 by compound assignment: Back to forest's runtime name leaves the allowlist | accepted | **REJECTED** | 0 | LearningObjectivesView.cs:81: header local 'forest' is used outside the reviewed straight-line header sh… |
| X-F1-25 | R04 by cast: second name write the receiver parse cannot attribute | accepted | **REJECTED** | 0 | LearningObjectivesView.cs:81: header local 'forest' is used outside the reviewed straight-line header sh… |
| X-F1-26 | R04 by alias: second name write through another local | accepted | **REJECTED** | 0 | LearningObjectivesView.cs:81: header local 'forest' is used outside the reviewed straight-line header sh… |
| X-F1-27 | allowlist element overwritten at run time through a qualified reference (R03 … | accepted | **REJECTED** | 0 | GuidanceButtonNames must be used only by its declaration and the demotion loop; found 3 references |
| X-F1-28 | method-group + class-level const: no call shape and no literal in the constru… | accepted | **REJECTED** | 0 | LearningObjectivesView.cs:83: header local 'close' is used outside the reviewed straight-line header sha… |
| X-F1-29 | lexer desync: char literal inside an interpolation hole hides a map primary | accepted | **REJECTED** | 0 | LearningObjectivesView.cs:75: literal/comment boundaries are ambiguous (for example a literal nested in … |
| X-F1-30 | lexer desync: @$ verbatim-interpolated backslash hides a map primary | accepted | **REJECTED** | 0 | LearningObjectivesView.cs:75: literal/comment boundaries are ambiguous (for example a literal nested in … |
| X-F1-31 | second static primary on an allowlisted button (demoted in guided state, doub… | accepted | **REJECTED** | 0 | LearningObjectivesView.cs:78: header local 'work' is used outside the reviewed straight-line header shap… |
| X-F1-32 | new CloseAll primary reusing another view's allowlisted name (demoted in guid… | accepted | **REJECTED** | 0 | LearningObjectivesView.cs:82: extra header button 'close' must be non-primary, added once and not use a … |
| X-F1-33 | names swapped between Work Plan and Back to forest (both allowlisted): later-… | accepted | **REJECTED** | 0 | LearningObjectivesView.cs:76: header button 'Work Plan [Tab]' must be ui.ShowWorkPlan(), without classes… |
| X-F1-34 | second non-primary button with the map caption that closes the screen | accepted | **REJECTED** | 0 | header button 'Stand map [M]' must be declared exactly once; found 2 (RR-1) |
| X-F1-35 | spoof captioned map + real map primary (tolerant lookup picks the spoof) | rejected | **REJECTED** | 0 | static primary button 'objective-primary-cta' is not in the GuidanceButtonNames scoped demotion set |

### #305 X-F2 (46)

| ID | Mutation | 1585ca20 | b4d47d3d | C# 9 errors | Candidate rejection cause (abridged) |
|---|---|---|---|---|---|
| X-F2-01 | R08 by row indexer: map renamed through buttons[0] | accepted | **REJECTED** | 0 | LearningObjectivesView.cs:76: header local 'buttons' is used outside the reviewed straight-line header s… |
| X-F2-02 | R15 by row indexer: map disabled through buttons[0] | accepted | **REJECTED** | 0 | LearningObjectivesView.cs:76: header local 'buttons' is used outside the reviewed straight-line header s… |
| X-F2-03 | R15 by parent: whole button row disabled | accepted | **REJECTED** | 0 | LearningObjectivesView.cs:82: header local 'buttons' is used outside the reviewed straight-line header s… |
| X-F2-04 | row cleared after binding: Q<Button> finds no map | accepted | **REJECTED** | 0 | LearningObjectivesView.cs:82: header local 'buttons' is used outside the reviewed straight-line header s… |
| X-F2-05 | row binding wrapped in if (false) | accepted | **REJECTED** | 0 | LearningObjectivesView.cs:75: header local 'buttons' is used outside the reviewed straight-line header s… |
| X-F2-06 | map identity wrapped in if (false) | accepted | **REJECTED** | 0 | LearningObjectivesView.cs:74: header local 'map' is used outside the reviewed straight-line header shape… |
| X-F2-07 | header row hidden | accepted | **REJECTED** | 0 | LearningObjectivesView.cs:83: header local 'header' is used outside the reviewed straight-line header sh… |
| X-F2-08 | R13 count-preserving: first guard replaced by a bare return, whole method dead | accepted | **REJECTED** | 0 | ScenarioOneUiRoot.cs:337: pinned method RefreshNextActionGuidance changed (expected 'if', found 'return'… |
| X-F2-09 | R11 by tuple assignment | accepted | **REJECTED** | 0 | ScenarioOneUiRoot.cs:452: pinned method RefreshNextActionGuidance changed (expected 'guidanceCandidate',… |
| X-F2-10 | R11 count-compensated: retarget inside the chain, one other target write remo… | accepted | **REJECTED** | 0 | ScenarioOneUiRoot.cs:361: pinned method RefreshNextActionGuidance changed (expected 'target', found 'rea… |
| X-F2-11 | objectives flag overwritten after its pinned declaration | accepted | **REJECTED** | 0 | ScenarioOneUiRoot.cs:369: pinned method RefreshNextActionGuidance changed (expected 'bool', found 'objec… |
| X-F2-12 | annual-review branch forced, map target unreachable | accepted | **REJECTED** | 0 | ScenarioOneUiRoot.cs:370: pinned method RefreshNextActionGuidance changed (expected 'NeedsAnnualReview',… |
| X-F2-13 | consent forced true, chain never reaches the map target | accepted | **REJECTED** | 0 | ScenarioOneUiRoot.cs:356: pinned method RefreshNextActionGuidance changed (expected 'mapQuestions', foun… |
| X-F2-14 | teaching gate widened after the pinned prefix: map CTA offered beyond Objecti… | accepted | **REJECTED** | 0 | ScenarioOneUiRoot.cs:364: pinned method RefreshNextActionGuidance changed (expected '(', found 'true'); … |
| X-F2-15 | routing literal swapped, count unchanged | accepted | **REJECTED** | 0 | ScenarioOneUiRoot.cs:381: pinned method RefreshNextActionGuidance changed (expected '"map-plan-rack"', f… |
| X-F2-16 | Back to forest re-promoted right after the demotion loop (original two-primar… | accepted | **REJECTED** | 0 | ScenarioOneUiRoot.cs:451: pinned method RefreshNextActionGuidance changed (expected 'guidanceScope', fou… |
| X-F2-17 | demotion undone by a second loop with a renamed variable | accepted | **REJECTED** | 0 | ScenarioOneUiRoot.cs:451: pinned method RefreshNextActionGuidance changed (expected 'guidanceScope', fou… |
| X-F2-18 | CTA primary class removed every frame after the LateUpdate apply | accepted | **REJECTED** | 0 | ScenarioOneUiRoot.cs:328: 'nextAction' is referenced outside the reviewed methods (qualified, tuple, ref… |
| X-F2-19 | CTA primary class removed in Apply through nextAction | accepted | **REJECTED** | 0 | ScenarioOneUiRoot.cs:489: pinned method ApplyNextActionGuidance changed (expected 'end of body', found '… |
| X-F2-20 | R14 by plain if: primary class never applied | accepted | **REJECTED** | 0 | ScenarioOneUiRoot.cs:487: pinned method ApplyNextActionGuidance changed (expected 'candidate', found 'if… |
| X-F2-21 | R14 by [Conditional]: Apply compiled out of Player builds, no #if anywhere | accepted | **REJECTED** | 0 | ScenarioOneUiRoot.cs:474: a [Conditional] attribute in ScenarioOneUiRoot is unsupported (RR-3) |
| X-F2-22 | R21 by lambda: LateUpdate call never invoked | accepted | **REJECTED** | 0 | ScenarioOneUiRoot.cs:1: ScenarioOneUiRoot: expected exactly one reviewed occurrence of 'private void Lat… |
| X-F2-23 | R21 by shadow: local function replaces the method inside LateUpdate | accepted | **REJECTED** | 0 | ScenarioOneUiRoot.cs:332: 'ApplyNextActionGuidance' is referenced outside the reviewed methods (qualifie… |
| X-F2-24 | R13 by goto: eligible candidate skips styling | accepted | **REJECTED** | 0 | ScenarioOneUiRoot.cs:482: pinned method ApplyNextActionGuidance changed (expected 'ClearNextAction', fou… |
| X-F2-25 | R13 by throw: eligible candidate never styled | accepted | **REJECTED** | 0 | ScenarioOneUiRoot.cs:482: pinned method ApplyNextActionGuidance changed (expected 'ClearNextAction', fou… |
| X-F2-26 | R09 by dead lambda: guard present but never executed | accepted | **REJECTED** | 0 | ScenarioOneUiRoot.cs:483: pinned method ApplyNextActionGuidance changed (expected 'if', found 'System');… |
| X-F2-27 | R23 with this.: candidate nulled by an unrelated handler | accepted | **REJECTED** | 0 | 'guidanceCandidate' must be written only by RefreshNextActionGuidance/ApplyNextActionGuidance/ClearNextA… |
| X-F2-28 | R20c with this.: help-close frame reset | accepted | **REJECTED** | 0 | helpClosedFrame must be written only by its initialiser and CloseHelp |
| X-F2-29 | R20 by inverted early guard: setter unreachable while Help is open | accepted | **REJECTED** | 0 | ScenarioOneUiRoot.cs:611: pinned method CloseHelp changed (expected '!', found 'help'); a reviewed metho… |
| X-F2-30 | R20 by lambda: setter never invoked | accepted | **REJECTED** | 0 | ScenarioOneUiRoot.cs:614: pinned method CloseHelp changed (expected 'helpClosedFrame', found 'System'); … |
| X-F2-31 | GuidanceVisible parameter overwritten: every candidate reads as visible | accepted | **REJECTED** | 0 | ScenarioOneUiRoot.cs:493: pinned method GuidanceVisible changed (expected 'for', found 'element'); a rev… |
| X-F2-32 | GuidanceVisible skips the candidate's own visibility | accepted | **REJECTED** | 0 | ScenarioOneUiRoot.cs:495: pinned method GuidanceVisible changed (expected '.', found '=='); a reviewed m… |
| X-F2-33 | only Objectives caller of Refresh removed: guidance never derived | accepted | **REJECTED** | 0 | RefreshNextActionGuidance must be called exactly once by the view class, from Refresh(bool); found 0 (RR… |
| X-F2-34 | only Objectives caller of Refresh behind #if (R14 at the call site) | accepted | **REJECTED** | 0 | \bpublic\s+void\s+Refresh\s*\(\s*bool\s+force\s*\): conditional compilation (#if) in a checked method or… |
| X-F2-35 | ShowMap gutted: CTA still wired by name, opens nothing | accepted | **REJECTED** | 0 | ScenarioOneUiRoot.cs:690: pinned method ShowMap changed (expected 'Map', found 'None'); a reviewed metho… |
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

### #305 benign (15)

| ID | Mutation | 1585ca20 | b4d47d3d | C# 9 errors | Candidate rejection cause (abridged) |
|---|---|---|---|---|---|
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
| B-X-14 | CRLF line endings in all files | accepted | **ACCEPTED** | 0 |  |
| B-X-15 | UTF-8 BOM at the start of all files | accepted | **ACCEPTED** | 0 |  |

### Original 20, #304 F1 14, F2/F3 21 and benign 4 (run from the 1585ca20 test file)

| ID | Group | 1585ca20 | b4d47d3d | C# 9 errors | Cause (abridged) |
|---|---|---|---|---|---|
| map-button-name | original | rejected | **REJECTED** | 0 | Objectives constructor/map identity: expected one structural match, found 0 |
| map-click-destination | original | rejected | **REJECTED** | 0 | named map button with exact caption and ShowMap click: expected one structural match, found 0 |
| map-caption | original | rejected | **REJECTED** | 0 | named map button with exact caption and ShowMap click: expected one structural match, found 0 |
| map-not-added-to-row | original | rejected | **REJECTED** | 1 (CS0841) | Objectives constructor/row binding: expected one structural match, found 0 |
| duplicate-map-identity | original | rejected | **REJECTED** | 0 | duplicate element name in the Objectives constructor |
| target-renamed | original | rejected | **REJECTED** | 0 | RefreshNextActionGuidance/no rack plan yet: expected one structural match, found 0 |
| objective-condition-removed | original | rejected | **REJECTED** | 0 | RefreshNextActionGuidance/no rack plan yet: expected one structural match, found 0 |
| rack-v2-gate-removed | original | rejected | **REJECTED** | 0 | RefreshNextActionGuidance/rack teaching gate: expected one structural match, found 0 |
| help-guard-removed | original | rejected | **REJECTED** | 0 | ApplyNextActionGuidance/preview/menus/help suspension: expected one structural match, found 0 |
| reference-preview-guard-removed | original | rejected | **REJECTED** | 0 | ApplyNextActionGuidance/preview/menus/help suspension: expected one structural match, found 0 |
| candidate-enabled-check-removed | original | rejected | **REJECTED** | 0 | ApplyNextActionGuidance/candidate scoped visibility: expected one structural match, found 0 |
| scope-visibility-check-removed | original | rejected | **REJECTED** | 0 | ApplyNextActionGuidance/candidate scoped visibility: expected one structural match, found 0 |
| hidden-visibility-check-removed | original | rejected | **REJECTED** | 0 | visibility and display suspension: expected one structural match, found 0 |
| allowlist-primary-dedup-removed | original | rejected | **REJECTED** | 0 | RefreshNextActionGuidance/bounded class demotion: expected one structural match, found 0 |
| unrelated-global-demotion | original | rejected | **REJECTED** | 0 | RefreshNextActionGuidance/bounded class demotion: expected one structural match, found 0 |
| primary-restore-removed | original | rejected | **REJECTED** | 0 | RefreshNextActionGuidance/restore old primary classes: expected one structural match, found 0 |
| primary-class-not-added | original | rejected | **REJECTED** | 0 | ApplyNextActionGuidance/apply primary: expected one structural match, found 0 |
| next-action-class-not-added | original | rejected | **REJECTED** | 0 | ApplyNextActionGuidance/apply next-action: expected one structural match, found 0 |
| eligibility-return-removed | original | rejected | **REJECTED** | 0 | ApplyNextActionGuidance/no styling if ineligible: expected one structural match, found 0 |
| target-not-bound-to-scoped-button | original | rejected | **REJECTED** | 0 | RefreshNextActionGuidance/candidate resolved in its scope: expected one structural match, found 0 |
| R01-unconditional-map-primary | F1 | rejected | **REJECTED** | 0 | map CTA must not be unconditionally primary (AddToClassList) |
| R01b-map-primary-via-enable-class | F1 | rejected | **REJECTED** | 0 | map CTA must not be unconditionally primary (EnableInClassList) |
| R01c-map-primary-verbatim-spelling | F1 | rejected | **REJECTED** | 0 | map CTA must not be unconditionally primary (AddToClassList) |
| R01d-map-class-non-literal | F1 | rejected | **REJECTED** | 0 | AddToClassList in the header uses a non-literal class name (ambiguous) |
| R03-back-to-forest-removed-from-allowlist | F1 | rejected | **REJECTED** | 0 | static primary button 'objective-back-to-forest' is not in the GuidanceButtonNames scoped demotion set |
| R04-back-to-forest-renamed-out-of-allowlist | F1 | rejected | **REJECTED** | 0 | static primary button 'objective-forest' is not in the GuidanceButtonNames scoped demotion set |
| R05-new-unnamed-closeall-primary | F1 | rejected | **REJECTED** | 0 | static primary 'close' must have exactly one literal name; found 0 |
| R05b-new-closeall-primary-named-outside-allowlist | F1 | rejected | **REJECTED** | 0 | static primary button 'objective-close' is not in the GuidanceButtonNames scoped demotion set |
| R05c-inline-unnamed-primary | F1 | rejected | **REJECTED** | 0 | static btn-primary header button is not assigned to a named local (unnamed primary) |
| R05d-new-closeall-primary-reusing-allowlisted-name | F1 | rejected | **REJECTED** | 0 | duplicate element name in the Objectives constructor |
| R05e-named-argument-class | F1 | rejected | **REJECTED** | 0 | named UiKit.Button argument in the header is an unsupported shape |
| R05f-primary-class-in-unsupported-shape | F1 | rejected | **REJECTED** | 0 | btn-primary literal 'btn-primary' in the header is not in a supported UiKit.Button/AddToClassList shape |
| R05g-forest-primary-reassigned | F1 | rejected | **REJECTED** | 0 | static primary 'forest' must be declared once and never reassigned in the header |
| allowlist-duplicate-name | F1 | rejected | **REJECTED** | 0 | GuidanceButtonNames contains a duplicate name |
| R07-verbatim-duplicate-cta-name | F2-F3 | rejected | **REJECTED** | 0 | duplicate element name in the Objectives constructor |
| R07b-escaped-duplicate-cta-name | F2-F3 | rejected | **REJECTED** | 0 | duplicate element name in the Objectives constructor |
| R07c-concatenated-name | F2-F3 | rejected | **REJECTED** | 0 | an element name in the Objectives constructor is not a single plain literal |
| R08-later-map-name-overwrite | F2-F3 | rejected | **REJECTED** | 0 | Objective CTA name must be unique and bound to the ShowMap button; found map names ['objective-primary-c… |
| R09-anchor-spoofed-in-string-literal | F2-F3 | rejected | **REJECTED** | 0 | ApplyNextActionGuidance/no styling if ineligible: expected one structural match, found 0 |
| R09b-view-anchor-spoofed-in-string-literal | F2-F3 | rejected | **REJECTED** | 0 | Objectives constructor/row binding: expected one structural match, found 0 |
| R11-target-override-to-closeall | F2-F3 | rejected | **REJECTED** | 0 | target is reassigned after the routing chain |
| R12-candidate-nulled-in-apply | F2-F3 | rejected | **REJECTED** | 0 | ApplyNextActionGuidance writes 'candidate' more than once (override) |
| R13-unreachable-return-in-refresh | F2-F3 | rejected | **REJECTED** | 0 | RefreshNextActionGuidance has an unexpected early return |
| R14-primary-hidden-behind-unity-editor | F2-F3 | rejected | **REJECTED** | 0 | \bprivate\s+void\s+ApplyNextActionGuidance\s*\(\s*\): conditional compilation (#if) in a checked method … |
| R14b-cta-name-hidden-behind-conditional | F2-F3 | rejected | **REJECTED** | 0 | \bpublic\s+LearningObjectivesView\s*\(\s*ScenarioOneUiRoot\s+ui\s*\): conditional compilation (#if) in a… |
| R15-map-disabled-after-binding | F2-F3 | rejected | **REJECTED** | 0 | map CTA variable may only be declared, named and added to the header row; other uses are unsupported |
| R16b-extra-return-true-in-visibility | F2-F3 | rejected | **REJECTED** | 0 | GuidanceVisible has an unexpected return |
| R17b-candidate-disabled-in-apply | F2-F3 | rejected | **REJECTED** | 0 | ApplyNextActionGuidance uses an unexpected candidate member: SetEnabled |
| R20-help-close-frame-setter-removed | F2-F3 | rejected | **REJECTED** | 0 | CloseHelp/help-close frame recorded: expected one structural match, found 0 |
| R20b-help-close-frame-conditional | F2-F3 | rejected | **REJECTED** | 0 | helpClosedFrame is set conditionally in CloseHelp |
| R20c-help-close-frame-reset | F2-F3 | rejected | **REJECTED** | 0 | helpClosedFrame must be written only by its initialiser and CloseHelp |
| R22-candidate-nulled-in-clear | F2-F3 | rejected | **REJECTED** | 0 | ClearNextAction may only drop the guide classes, badge and nextAction |
| R23-candidate-nulled-by-unrelated-handler | F2-F3 | rejected | **REJECTED** | 0 | 'guidanceCandidate' must be written only by RefreshNextActionGuidance/ApplyNextActionGuidance/ClearNextA… |
| R21-late-update-apply-removed | F2-F3 | rejected | **REJECTED** | 0 | LateUpdate applies cached guidance: expected one structural match, found 0 |
| R21b-late-update-apply-after-return | F2-F3 | rejected | **REJECTED** | 0 | LateUpdate may only apply guidance unconditionally before any branch or return |
| B01-trailing-comment-after-eligibility-return | benign4 | accepted | **ACCEPTED** | 0 |  |
| B02-map-constructor-reformatted | benign4 | accepted | **ACCEPTED** | 0 |  |
| B03-comment-mentions-primary-and-conditional | benign4 | accepted | **ACCEPTED** | 0 |  |
| B04-new-nonprimary-header-button-unnamed | benign4 | accepted | **ACCEPTED** | 0 |  |

### My 176 cases

Legend: Expect `rej` = should be rejected; `ben` = benign; `lim` = disclosed limit (accepted is expected); `unk` = exploratory. Fam = failure family S-1/S-2/S-3 above (C = S-1, A and B = S-2, D = S-3). e2e exit = real `check_baseline.py` exit where I ran it.

| ID | Group | Mutation | Expect | Result | C# 9 errors | Fam | e2e exit | Cause / note (abridged) |
|---|---|---|---|---|---|---|---|---|
| Y-RR1-01 | RR-1 | forest.name rewritten from itself (forest.name = forest.name + suffix) | rej | rejected | 0 |  |  | an element name in the Objectives constructor is not a single plain literal |
| Y-RR1-02 | RR-1 | forest name built by Concat (statement not a plain literal binding) | rej | rejected | 0 |  |  | an element name in the Objectives constructor is not a single plain literal |
| Y-RR1-03 | RR-1 | names set through a tuple deconstruction | rej | rejected | 0 |  |  | LearningObjectivesView.cs:82: header local 'work' is used outside the reviewed straight-line header… |
| Y-RR1-04 | RR-1 | unreviewed inline new Button appended to the header row | rej | rejected | 0 |  |  | LearningObjectivesView.cs:82: header row addition of a button that is not a reviewed UiKit.Button d… |
| Y-RR1-05 | RR-1 | Back to forest disabled through the enabled argument | rej | rejected | 0 |  |  | LearningObjectivesView.cs:79: header button 'Back to forest [O / Esc]' must be ui.CloseAll(), the o… |
| Y-RR1-06 | RR-1 | map made static primary via a const class argument | rej | rejected | 0 |  |  | UiKit.Button class argument in the header is not a plain string literal (ambiguous) |
| Y-RR1-07 | RR-1 | second static primary in the Curriculum partial | rej | rejected | 1 (CS0246) |  |  | LearningObjectivesView.Curriculum.cs:10: btn-primary attached by UiKit.Button outside the reviewed … |
| Y-RR1-08 | RR-1 | primary class through UiKit.Box in a partial | rej | rejected | 1 (CS0246) |  |  | LearningObjectivesView.cs:79: the Objectives view class must contain exactly one btn-primary litera… |
| Y-RR1-09 | RR-1 | primary class through UiKit.Add in a partial | rej | rejected | 2 (CS0246) |  |  | LearningObjectivesView.Curriculum.cs:10: btn-primary attached by UiKit.Add outside the reviewed sit… |
| Y-RR1-10 | RR-1 | a second Back to forest primary placed in the scroll area | rej | rejected | 0 |  |  | LearningObjectivesView.cs:192: btn-primary attached by UiKit.Button outside the reviewed site (RR-1) |
| Y-RR1-11 | RR-1 | verbatim spelling of the class on Work Plan | rej | rejected | 0 |  |  | LearningObjectivesView.cs:76: header button 'Work Plan [Tab]' must be ui.ShowWorkPlan(), without cl… |
| Y-RR1-12 | RR-1 | escaped spelling of the class on Work Plan | rej | rejected | 0 |  |  | LearningObjectivesView.cs:76: header button 'Work Plan [Tab]' must be ui.ShowWorkPlan(), without cl… |
| Y-RR1-13 | RR-1 | interpolated class on Work Plan | rej | rejected | 0 |  |  | LearningObjectivesView.cs:76: literal/comment boundaries are ambiguous (for example a literal neste… |
| Y-RR1-14 | RR-1 | allowlist copied with Clone and edited | rej | rejected | 0 |  |  | GuidanceButtonNames must be used only by its declaration and the demotion loop; found 3 references |
| Y-RR1-15 | RR-1 | allowlist aliased by a second static field | rej | rejected | 0 |  |  | GuidanceButtonNames must be used only by its declaration and the demotion loop; found 3 references |
| Y-RR1-16 | RR-1 | allowlist sorted in place at runtime | rej | rejected | 0 |  |  | GuidanceButtonNames must be used only by its declaration and the demotion loop; found 3 references |
| Y-RR1-17 | RR-1 | Back to forest retargeted to ShowObjectives (still primary) | rej | rejected | 0 |  |  | LearningObjectivesView.cs:79: header button 'Back to forest [O / Esc]' must be ui.CloseAll(), the o… |
| Y-RR1-18 | RR-1 | header statement hidden behind #if | rej | rejected | 0 |  |  | \bpublic\s+LearningObjectivesView\s*\(\s*ScenarioOneUiRoot\s+ui\s*\): conditional compilation (#if)… |
| Y-RR1-19 | RR-1 | forest aliased by var | rej | rejected | 0 |  |  | LearningObjectivesView.cs:82: header local 'forest' is used outside the reviewed straight-line head… |
| Y-RR1-20 | RR-1 | local function capturing forest | rej | rejected | 0 |  |  | LearningObjectivesView.cs:82: header local 'forest' is used outside the reviewed straight-line head… |
| Y-RR1-21 | RR-1 | forest callback registered | rej | rejected | 0 |  |  | LearningObjectivesView.cs:82: header local 'forest' is used outside the reviewed straight-line head… |
| Y-RR1-22 | RR-1 | this.Root used in a method | rej | rejected | 0 |  |  | LearningObjectivesView.cs:102: the Objectives Root may be used only by its declaration and the cons… |
| Y-RR1-23 | RR-1 | Root restyled inside the constructor | rej | rejected | 0 |  |  | LearningObjectivesView.cs:83: header local 'Root' is used outside the reviewed straight-line header… |
| Y-RR1-24 | RR-1 | second constructor overload | rej | rejected | 0 |  |  | LearningObjectivesView must have exactly one constructor; found 2 |
| Y-RR1-25 | RR-1 | static constructor | rej | rejected | 0 |  |  | LearningObjectivesView must have exactly one constructor; found 2 |
| Y-RR1-26 | RR-1 | a second class in the view file tints a button | rej | rejected | 0 |  |  | LearningObjectivesView.cs:8: a class-list API (only the reviewed header may style a button) in the … |
| Y-RR1-27 | RR-1 | UiKit overload with default argument | rej | rejected | 0 |  |  | UiKit must declare exactly one Button, Box, Row, Text, Add and MakeModal; found ['Add', 'Box', 'But… |
| Y-RR1-28 | RR-1 | Stand map CTA primary via UiKit class param in the ctor (R02 variant, enabled f… | rej | rejected | 0 |  |  | map CTA must not be unconditionally primary (UiKit.Button) |
| Y-RR1-29 | RR-1 | Work Plan renamed to a name outside the allowlist | rej | rejected | 0 |  |  | LearningObjectivesView.cs:76: header button 'Work Plan [Tab]' must be ui.ShowWorkPlan(), without cl… |
| Y-RR1-30 | RR-1 | extra header button given the Back to forest name | rej | rejected | 0 |  |  | duplicate element name in the Objectives constructor |
| Y-RR1-31 | RR-1 | header button class supplied through params array | rej | rejected | 0 |  |  | UiKit.Button class argument in the header is not a plain string literal (ambiguous) |
| Y-RR1-32 | RR-1 | map CTA primary via named argument | rej | rejected | 0 |  |  | named UiKit.Button argument in the header is an unsupported shape |
| Y-RR1-33 | RR-1 | nameof(forest) used after attach | rej | rejected | 0 |  |  | LearningObjectivesView.cs:82: header local 'forest' is used outside the reviewed straight-line head… |
| Y-RR1-34 | RR-1 | foreach over the row children in the ctor | rej | rejected | 0 |  |  | LearningObjectivesView.cs:83: header local 'buttons' is used outside the reviewed straight-line hea… |
| Y-RR1-35 | RR-1 | ui field reassigned through this | rej | rejected | 0 |  |  | LearningObjectivesView.cs:83: header local 'ui' is used outside the reviewed straight-line header s… |
| Y-RR1-36 | RR-1 | map CTA primary restored from another view-class method via ui.RootElement | rej | rejected | 0 |  |  | LearningObjectivesView.cs:102: a class-list API (only the reviewed header may style a button) in th… |
| Y-RR2-01 | RR-2 | this.guidanceScope = null in ShowObjectives | rej | rejected | 0 |  |  | 'guidanceScope' must be written only by RefreshNextActionGuidance/ApplyNextActionGuidance/ClearNext… |
| Y-RR2-02 | RR-2 | guidanceCandidate property wrapper | rej | rejected | 0 |  |  | 'guidanceCandidate' must be written only by RefreshNextActionGuidance/ApplyNextActionGuidance/Clear… |
| Y-RR2-03 | RR-2 | Interlocked.Exchange(ref this.guidanceCandidate) | rej | rejected | 0 |  |  | ScenarioOneUiRoot.cs:702: 'guidanceCandidate' is referenced outside the reviewed methods (qualified… |
| Y-RR2-04 | RR-2 | helpClosedFrame += 1 outside CloseHelp | rej | rejected | 0 |  |  | helpClosedFrame must be written only by its initialiser and CloseHelp |
| Y-RR2-05 | RR-2 | helpClosedFrame++ outside CloseHelp | rej | rejected | 0 |  |  | helpClosedFrame must be written only by its initialiser and CloseHelp |
| Y-RR2-06 | RR-2 | dead first guard: // true | rej | rejected | 0 |  |  | ScenarioOneUiRoot.cs:337: pinned method RefreshNextActionGuidance changed (expected ')', found '//'… |
| Y-RR2-07 | RR-2 | ClearNextAction also nulls the candidate | rej | rejected | 0 |  |  | ClearNextAction may only drop the guide classes, badge and nextAction |
| Y-RR2-08 | RR-2 | screen retargeted inside TryShowReviewYear (count unchanged) | rej | **ACCEPTED (false green)** | 0 | C | 0 |  |
| Y-RR2-09 | RR-2 | tuple swap of guidance fields in an unpinned method | rej | rejected | 0 |  |  | ScenarioOneUiRoot.cs:696: 'nextAction' is referenced outside the reviewed methods (qualified, tuple… |
| Y-RR2-10 | RR-2 | guidanceScope declared again as a local in another method (shadow) | rej | rejected | 0 |  |  | 'guidanceScope' must be written only by RefreshNextActionGuidance/ApplyNextActionGuidance/ClearNext… |
| Y-RR2-11 | RR-2 | guidancePrimaries replaced by reflection-free new dictionary assignment | rej | rejected | 0 |  |  | ScenarioOneUiRoot.cs:696: 'guidancePrimaries' is referenced outside the reviewed methods (qualified… |
| Y-RR2-12 | RR-2 | ref local aliasing helpClosedFrame | rej | rejected | 0 |  |  | helpClosedFrame must be written only by its initialiser and CloseHelp |
| Y-RR2-13 | RR-2 | out-style aliasing via local function writing helpClosedFrame | rej | rejected | 0 |  |  | helpClosedFrame must be written only by its initialiser and CloseHelp |
| Y-RR2-14 | RR-2 | nameof used as string key for guarded state (reflection prep) | rej | rejected | 0 |  |  | ScenarioOneUiRoot.cs:696: 'guidanceCandidate' is referenced outside the reviewed methods (qualified… |
| Y-RR2-15 | RR-2 | target literal swapped between two pinned write sites (count unchanged) | rej | rejected | 0 |  |  | ScenarioOneUiRoot.cs:409: pinned method RefreshNextActionGuidance changed (expected '"objective-wor… |
| Y-RR2-16 | RR-2 | scope condition flipped (screen == Objectives ? map.Root : learning.Root) | rej | rejected | 0 |  |  | RefreshNextActionGuidance/screen scope: expected one structural match, found 0 |
| Y-RR2-17 | RR-2 | ApplyNextActionGuidance suspended flag forced | rej | rejected | 0 |  |  | ApplyNextActionGuidance/preview/menus/help suspension: expected one structural match, found 0 |
| Y-RR3-01 | RR-3 | [Conditional] on ClearNextAction | rej | rejected | 0 |  |  | ScenarioOneUiRoot.cs:462: a [Conditional] attribute in ScenarioOneUiRoot is unsupported (RR-3) |
| Y-RR3-02 | RR-3 | [Conditional] on GuidanceVisible-like new helper called from Apply? (unpinned a… | rej | rejected | 0 |  |  | ScenarioOneUiRoot.cs:323: a [Conditional] attribute in ScenarioOneUiRoot is unsupported (RR-3) |
| Y-RR3-03 | RR-3 | local function shadows ApplyNextActionGuidance in Start | rej | rejected | 0 |  |  | ScenarioOneUiRoot.cs:92: 'ApplyNextActionGuidance' is referenced outside the reviewed methods (qual… |
| Y-RR3-04 | RR-3 | late removal of the CTA primary after Apply in LateUpdate | rej | rejected | 0 |  |  | ScenarioOneUiRoot.cs:328: 'nextAction' is referenced outside the reviewed methods (qualified, tuple… |
| Y-RR3-05 | RR-3 | row cleared at the end of the ctor | rej | rejected | 0 |  |  | LearningObjectivesView.cs:83: header local 'buttons' is used outside the reviewed straight-line hea… |
| Y-RR3-06 | RR-3 | row disabled | rej | rejected | 0 |  |  | LearningObjectivesView.cs:83: header local 'buttons' is used outside the reviewed straight-line hea… |
| Y-RR3-07 | RR-3 | row indexer rename | rej | rejected | 0 |  |  | LearningObjectivesView.cs:83: header local 'buttons' is used outside the reviewed straight-line hea… |
| Y-RR3-08 | RR-3 | caller moved to BuildTopic | rej | rejected | 0 |  |  | RefreshNextActionGuidance must be called exactly once by the view class, from Refresh(bool); found … |
| Y-RR3-09 | RR-3 | caller inside zero-iteration foreach | rej | rejected | 0 |  |  | LearningObjectivesView.cs:193: the Objectives RefreshNextActionGuidance call must be an uncondition… |
| Y-RR3-10 | RR-3 | caller inside while(false)-like if on a constant | rej | rejected | 0 |  |  | LearningObjectivesView.cs:193: the Objectives RefreshNextActionGuidance call must be an uncondition… |
| Y-RR3-11 | RR-3 | caller preceded by infinite loop (disclosed L-03 class) | lim | rejected (stricter than disclosed) | 0 |  |  | LearningObjectivesView.cs:194: the Objectives RefreshNextActionGuidance call must be an uncondition… |
| Y-RR3-12 | RR-3 | duplicate Refresh(bool) under #if | rej | rejected | 0 |  |  | expected exactly one method matching '\\bpublic\\s+void\\s+Refresh\\s*\\(\\s*bool\\s+force\\s*\\)';… |
| Y-RR3-13 | RR-3 | view Refresh early return before caller via expression | rej | rejected | 0 |  |  | LearningObjectivesView.cs:194: the Objectives RefreshNextActionGuidance call must be an uncondition… |
| Y-RR3-14 | RR-3 | SetScreen no longer refreshes Objectives (pinned) | rej | rejected | 0 |  |  | ScenarioOneUiRoot.cs:582: pinned method SetScreen changed (expected 'true', found 'false'); a revie… |
| Y-RR3-15 | RR-3 | LateUpdate apply wrapped in try/catch swallowing nothing but moved after return | rej | rejected | 0 |  |  | LateUpdate may only apply guidance unconditionally before any branch or return |
| Y-RR3-16 | RR-3 | ShowMap pinned: SetScreen(None) | rej | rejected | 0 |  |  | ScenarioOneUiRoot.cs:690: pinned method ShowMap changed (expected 'Map', found 'None'); a reviewed … |
| Y-RR3-17 | RR-3 | ShowObjectives opens Map instead (unpinned routing; N-2 class) | lim | accepted (disclosed limit) | 0 |  |  |  |
| Y-RR3-18 | RR-3 | CloseAll no longer closes Objectives (unpinned routing; N-2 class) | lim | accepted (disclosed limit) | 0 |  |  |  |
| Y-RR4-01 | RR-4 | allowlist element is a non-literal expression | rej | rejected | 0 |  |  | GuidanceButtonNames must contain only plain string literals |
| Y-RR4-02 | RR-4 | allowlist missing the map CTA | rej | rejected | 0 |  |  | map CTA missing or duplicated in scoped allowlist |
| Y-RR4-03 | RR-4 | allowlist declared twice | rej | rejected | 2 (CS0102) |  |  | GuidanceButtonNames allowlist: expected one structural match, found 2 |
| Y-RR4-04 | RR-4 | allowlist entry with interpolation | rej | rejected | 0 |  |  | ScenarioOneUiRoot.cs:62: literal/comment boundaries are ambiguous (for example a literal nested in … |
| Y-RR4-05 | RR-4 | allowlist duplicate (ordinary + escaped spelling) | rej | rejected | 0 |  | 1 | GuidanceButtonNames contains a duplicate name |
| Y-RR4-06 | RR-4 | allowlist missing forest AND forest has wrong name | rej | rejected | 0 |  |  | static primary button 'objective-forest' is not in the GuidanceButtonNames scoped demotion set |
| Y-RR4-07 | RR-4 | unterminated block comment in the root | rej | rejected | 8 (CS1513) |  |  | ScenarioOneUiRoot.cs:696: the lexer cannot follow this source (unterminated block comment); unsuppo… |
| Y-RR4-08 | RR-4 | unbalanced brace in the view ctor | rej | rejected | 26 (CS0106) |  |  | UiKit.Button class argument in the header is not a plain string literal (ambiguous) |
| Y-RR4-09 | RR-4 | constructor missing entirely (renamed) | rej | rejected | 1 (CS1520) |  | 1 | expected exactly one method matching '\\bpublic\\s+LearningObjectivesView\\s*\\(\\s*ScenarioOneUiRo… |
| Y-RR4-10 | RR-4 | empty-ish companion: UiKit.cs deleted from companions | rej | rejected | 792 (CS0103) |  |  | UiKit must be declared exactly once, non-partial, in UiKit.cs; found [] (N-3) |
| Y-N-01 | novel | interpolation: braces in nested string inside a hole, then class API | rej | rejected | 0 |  |  | LearningObjectivesView.cs:102: literal/comment boundaries are ambiguous (for example a literal nest… |
| Y-N-02 | novel | interpolation: escaped braces then hidden call | rej | rejected | 0 |  |  | LearningObjectivesView.Sequence.cs:9: the Objectives Root may be used only by its declaration and t… |
| Y-N-03 | novel | interpolation: nested interpolated string in a hole + trailing call | rej | rejected | 0 |  |  | LearningObjectivesView.Sequence.cs:9: the Objectives Root may be used only by its declaration and t… |
| Y-N-04 | novel | verbatim interpolated with doubled quotes in hole then call | rej | rejected | 4 (CS1073) |  |  | LearningObjectivesView.Sequence.cs:9: the Objectives Root may be used only by its declaration and t… |
| Y-N-05 | novel | format clause with colon then hidden call | rej | rejected | 0 |  |  | LearningObjectivesView.Sequence.cs:9: the Objectives Root may be used only by its declaration and t… |
| Y-N-06 | novel | inactive #if region with unbalanced apostrophe prose then real call | rej | rejected | 0 |  |  | LearningObjectivesView.Sequence.cs:11: the lexer cannot follow this source (unsupported character l… |
| Y-N-07 | novel | inactive #if region containing a class-list call (conservative: treated as code) | unk | rejected (documented fail-closed) | 0 |  |  | LearningObjectivesView.Sequence.cs:10: a class-list API (only the reviewed header may style a butto… |
| Y-N-08 | novel | #region text with quotes and slashes | ben | accepted | 0 |  |  |  |
| Y-N-09 | novel | lone CR ends a // comment: hidden call after it | rej | **ACCEPTED (false green)** | 0 | A | 1 |  |
| Y-N-10 | novel | U+2028 ends a // comment: hidden call after it | rej | **ACCEPTED (false green)** | 0 | A | 0 |  |
| Y-N-11 | novel | zero-width joiner inside the 'forest' identifier (Cf ignored by the C# compiler) | rej | **ACCEPTED (false green)** | 0 | B | 0 |  |
| Y-N-12 | novel | zero-width joiner inside a partial class name hides the companion | rej | **ACCEPTED (false green)** | 0 | B | 0 |  |
| Y-N-13 | novel | NEL (U+0085) ends a // comment: hidden call after it | rej | **ACCEPTED (false green)** | 0 | A | 0 |  |
| Y-N-14 | novel | Cf char inside GuidanceButtonNames reference outside the pins | rej | **ACCEPTED (false green)** | 0 | B | 0 |  |
| Y-N-15 | novel | Cf char inside guarded helpClosedFrame write | rej | **ACCEPTED (false green)** | 0 | B | 0 |  |
| Y-N-16 | novel | BOM / NBSP between tokens in the header (benign whitespace) | ben | accepted | 0 |  |  |  |
| Y-N-17 | novel | unicode letter identifiers elsewhere (benign) | ben | accepted | 0 |  |  |  |
| Y-N-18 | novel | char literal containing a quote then code | rej | rejected | 0 |  |  | LearningObjectivesView.cs:102: the Objectives Root may be used only by its declaration and the cons… |
| Y-N-19 | novel | string containing // then hidden call | rej | rejected | 0 |  |  | LearningObjectivesView.cs:102: the Objectives Root may be used only by its declaration and the cons… |
| Y-N-20 | novel | attribute with string containing bracket on a pinned method | rej | rejected | 0 |  |  | ScenarioOneUiRoot.cs:463: an attribute on a checked method is unsupported (e.g. [Conditional]) |
| Y-N-21 | novel | global-namespace type hijacks Button in a new file that mentions no hint (docum… | lim | accepted (disclosed limit) | 3 (CS1729) |  |  |  |
| Y-N-22 | novel | non-companion file disables the map CTA via constructed name (disclosed L-01) | lim | accepted (disclosed limit) | 0 |  |  |  |
| Y-N-23 | novel | non-companion file re-promotes Work Plan via constructed names (class re-add fr… | lim | accepted (disclosed limit) | 0 |  |  |  |
| Y-N-24 | novel | root unpinned method disables all buttons of its document (disclosed L-02) | lim | accepted (disclosed limit) | 0 |  |  |  |
| Y-N-25 | novel | root unpinned method hides the CTA by constructed name (disclosed L-01/L-02 cla… | lim | accepted (disclosed limit) | 0 |  |  |  |
| Y-B-01 | benign | new unrelated root method querying another label | ben | accepted | 0 |  |  |  |
| Y-B-02 | benign | new static readonly field in the view | ben | accepted | 0 |  |  |  |
| Y-B-03 | benign | long lesson text edit in Curriculum partial | ben | accepted | 0 |  |  |  |
| Y-B-04 | benign | extra using directive in root and view | ben | accepted | 0 |  |  |  |
| Y-B-05 | benign | whitespace/tab and trailing-space changes inside pinned ClearNextAction | ben | accepted | 0 |  |  |  |
| Y-B-06 | benign | XML doc comment with quotes before a root method | ben | accepted | 0 |  |  |  |
| Y-B-07 | benign | [SerializeField] field in root | ben | accepted | 0 |  |  |  |
| Y-B-08 | benign | expression-bodied root property reading learning | ben | accepted | 0 |  |  |  |
| Y-B-09 | benign | root method passing learning to a helper (README documents this as rejected) | unk | rejected (documented fail-closed) | 0 |  |  | ScenarioOneUiRoot.cs:697: the Objectives view field is reassigned, aliased or passed on (RR-1/RR-2) |
| Y-B-10 | benign | string with btn-primary text in a log call in the view (README: any btn-primary… | unk | rejected (documented fail-closed) | 0 |  |  | LearningObjectivesView.cs:102: the Objectives view class must contain exactly one btn-primary liter… |
| Y-B-11 | benign | new added unrelated view-class method using Q<> (README documents Q as banned) | unk | rejected (documented fail-closed) | 3 (CS0246) |  |  | LearningObjectivesView.Curriculum.cs:10: element-tree traversal or query in the Objectives view cla… |
| Y-B-12 | benign | new editor test mentioning header name (companion rule) | unk | rejected (documented fail-closed) | 0 |  |  | Editor/ObjectiveTests.cs:1: another class names an Objectives header button (RR-1) |
| Y-B-13 | benign | unrelated new MonoBehaviour that mentions UiKit | ben | accepted | 0 |  |  |  |
| Y-B-14 | benign | pinned Refresh: comment block inserted mid-body | ben | accepted | 0 |  |  |  |
| Y-B-15 | benign | header: region and extra comment lines | ben | accepted | 0 |  |  |  |
| Y-B-16 | benign | allowlist extended with an extra name and reordered | ben | accepted | 0 |  |  |  |
| Y-B-17 | benign | CRLF line endings in every checked file | ben | accepted | 0 |  |  |  |
| Y-B-18 | benign | new Objectives header extra button, non-primary, new caption | ben | accepted | 0 |  |  |  |
| Z-RR1-07 | RR-1 | second static primary in the Curriculum partial (qualified types) | rej | rejected | 0 |  |  | LearningObjectivesView.Curriculum.cs:10: btn-primary attached by UiKit.Button outside the reviewed … |
| Z-RR1-08 | RR-1 | primary class through UiKit.Box in a partial (qualified) | rej | rejected | 0 |  |  | LearningObjectivesView.cs:79: the Objectives view class must contain exactly one btn-primary litera… |
| Z-RR1-09 | RR-1 | primary class through UiKit.Add in a partial (qualified) | rej | rejected | 0 |  |  | LearningObjectivesView.Curriculum.cs:10: btn-primary attached by UiKit.Add outside the reviewed sit… |
| Z-N-04 | novel | verbatim interpolated with nested string in hole then call (valid C#) | rej | rejected | 0 |  |  | LearningObjectivesView.Sequence.cs:9: the Objectives Root may be used only by its declaration and t… |
| Z-B-11 | benign | new view-class method using Q<> (README documents Q as banned) | unk | rejected (documented fail-closed) | 1 (CS1061) |  |  | LearningObjectivesView.Curriculum.cs:10: element-tree traversal or query in the Objectives view cla… |
| Z-RR2-T-help | RR-2 | tuple deconstruction writes help in an unpinned method | rej | **ACCEPTED (false green)** | 0 | C | 0 |  |
| Z-RR2-T-screen | RR-2 | tuple deconstruction writes screen in an unpinned method | rej | **ACCEPTED (false green)** | 0 | C | 0 |  |
| Z-RR2-T-rackLayout | RR-2 | tuple deconstruction writes rackLayout in an unpinned method | rej | **ACCEPTED (false green)** | 0 | C | 0 |  |
| Z-RR2-T-map | RR-2 | tuple deconstruction writes map in an unpinned method | rej | **ACCEPTED (false green)** | 0 | C | 0 |  |
| Z-RR2-T-workPlan | RR-2 | tuple deconstruction writes workPlan in an unpinned method | rej | **ACCEPTED (false green)** | 0 | C | 0 |  |
| Z-RR2-T-review | RR-2 | tuple deconstruction writes review in an unpinned method | rej | **ACCEPTED (false green)** | 0 | C | 0 |  |
| Z-RR2-C-screen | RR-2 | count-compensated: TryShowReviewYear write removed, ShowObjectives retargets sc… | rej | **ACCEPTED (false green)** | 0 | C | 0 |  |
| Z-RR2-C-help | RR-2 | count-compensated: Start's help initialiser replaced by null (one write, value … | rej | **ACCEPTED (false green)** | 0 | C | 0 |  |
| Z-RR2-P-help | RR-2 | help gets a property alias writer (property setter) | rej | rejected | 0 |  |  | ScenarioOneUiRoot must assign 'help' exactly 1 time(s) (RR-2 related state) |
| Z-N-CR-region | novel | lone CR ends a #region line: hidden call after it | rej | **ACCEPTED (false green)** | 0 | A | 1 |  |
| Z-N-PS | novel | U+2029 ends a // comment: hidden call after it | rej | **ACCEPTED (false green)** | 0 | A | 0 |  |
| Z-N-CR-root | novel | lone CR hides a guarded-state write in the root | rej | **ACCEPTED (false green)** | 0 | A | 1 |  |
| Z-N-CF-buttons | novel | ZWJ identifier clears the header row in the ctor | rej | **ACCEPTED (false green)** | 0 | B | 0 |  |
| Z-N-CF-secondprimary | novel | second STATIC PRIMARY in the view via ZWJ-split UiKit and fragment class (N-1 b… | rej | **ACCEPTED (false green)** | 0 | B | 0 |  |
| Z-N-CF-secondprimary-ctl | novel | control: same second primary WITHOUT the ZWJ | rej | rejected | 0 |  | 1 | LearningObjectivesView.cs:192: UiKit.Button class argument is not a plain string literal |
| Z-N-CF-allowlist-clear | novel | ZWJ-split Array.Clear of the allowlist | rej | **ACCEPTED (false green)** | 0 | B | 0 |  |
| Z-N-CF-ctl-allowlist | novel | control: Array.Clear of the allowlist WITHOUT ZWJ | rej | rejected | 0 |  |  | GuidanceButtonNames must be used only by its declaration and the demotion loop; found 3 references |
| Z-N-SHY | novel | soft hyphen (Cf) in forest identifier | rej | **ACCEPTED (false green)** | 0 | B | 0 |  |
| Z-N-CF-ctl-forest | novel | control: forest.name += without Cf | rej | rejected | 0 |  |  | LearningObjectivesView.cs:82: header local 'forest' is used outside the reviewed straight-line head… |
| Z-N-NBSP-ctl | novel | control: NBSP inside identifier is a different (undeclared) identifier -> not v… | ben | accepted | 0 |  |  |  |
| Z-N-else | novel | #if/#else with the hostile code only in the #else branch | rej | rejected | 0 |  |  | LearningObjectivesView.Sequence.cs:12: the Objectives Root may be used only by its declaration and … |
| Z-N-nested-interp-view | novel | benign nested-literal interpolation in LearningObjectivesView.cs (documented O-… | unk | rejected (documented fail-closed) | 0 |  |  | LearningObjectivesView.cs:102: literal/comment boundaries are ambiguous (for example a literal nest… |
| Z-N-nested-interp-seq | novel | benign nested-literal interpolation in a partial (precise lexer) | ben | accepted | 0 |  |  |  |
| Z-N-verbatim-hash | novel | verbatim string with a line starting # (preprocessor-like) | unk | rejected (documented fail-closed) | 0 |  |  | LearningObjectivesView.Sequence.cs:9: a multi-line literal or comment spans a preprocessor line; fa… |
| Z-N-cdata | novel | string ending in backslash then code: "a\\" then call | rej | rejected | 0 |  |  | LearningObjectivesView.Sequence.cs:9: the Objectives Root may be used only by its declaration and t… |
| Z-N-chars | novel | char literals of quote/brace/backslash then hidden call | rej | rejected | 0 |  |  | LearningObjectivesView.Sequence.cs:9: the Objectives Root may be used only by its declaration and t… |
| Z-RR4-12 | RR-4 | allowlist contains null | rej | rejected | 0 |  |  | GuidanceButtonNames must contain only plain string literals |
| Z-RR4-13 | RR-4 | allowlist behind #if with an unrelated static primary also present | unk | accepted | 0 |  |  |  |
| Z-B-20 | benign | Back to forest caption copy tweak (plausible product text edit) | unk | rejected (documented fail-closed) | 0 |  |  | header button 'Back to forest [O / Esc]' must be declared exactly once; found 0 (RR-1) |
| Z-B-21 | benign | Work Plan caption copy tweak | unk | rejected (documented fail-closed) | 0 |  |  | header button 'Work Plan [Tab]' must be declared exactly once; found 0 (RR-1) |
| Z-B-22 | benign | an unrelated literal tweak inside pinned Refresh (reason text) | unk | rejected (documented fail-closed) | 0 |  |  | ScenarioOneUiRoot.cs:359: pinned method RefreshNextActionGuidance changed (expected '"Two rack ques… |
| Z-B-23 | benign | unrelated new method in root using document.rootVisualElement and SetEnabled | ben | accepted | 0 |  |  |  |
| Z-B-24 | benign | header extra button with enabled=false | ben | accepted | 0 |  |  |  |
| W-DEP-01 | dep | NeedsAnnualReview forced true: pinned chain always diverts to the annual-review… | rej | **ACCEPTED (false green)** | 0 | D | 0 |  |
| W-DEP-02 | dep | TeachingRacks forced false: Objective 1 never offered a guidance target (X-F2-1… | rej | **ACCEPTED (false green)** | 0 | D | 0 |  |
| W-DEP-03 | dep | CurrentTopicIndex forced to 1 | rej | **ACCEPTED (false green)** | 0 | D | 0 |  |
| W-DEP-04 | dep | NeedsAnnualReview routed through a helper returning true | rej | **ACCEPTED (false green)** | 0 | D | 0 |  |
| W-DEP-05 | dep | Objective guidance spot: ReloadProgress marks everything done (lesson stage jum… | rej | **ACCEPTED (false green)** | 0 | D | 0 |  |
| V-L06-1 | limit | companion aliases ui.Learning in a local then restyles every Objectives button … | lim | accepted (disclosed limit) | 0 |  |  |  |
| V-L06-2 | limit | existing companion (WalkingHudView) gets the same alias route | lim | accepted (disclosed limit) | 0 |  |  |  |
| V-L01-1 | limit | companion re-primaries every button of the document (no header names, no Learni… | lim | accepted (disclosed limit) | 0 |  |  |  |
| U-RR2-T-manager | RR-2 | tuple deconstruction writes manager (null) in an unpinned method | rej | **ACCEPTED (false green)** | 0 | C | 0 |  |
| U-RR2-T-learning-ctl | RR-2 | control: tuple deconstruction writes learning (caught by the alias rule) | rej | rejected | 0 |  |  | ScenarioOneUiRoot.cs:696: the Objectives view field is reassigned, aliased or passed on (RR-1/RR-2) |
| U-RR2-T-helpClosedFrame-ctl | RR-2 | control: tuple deconstruction writes helpClosedFrame (guarded name) | rej | rejected | 0 |  |  | ScenarioOneUiRoot.cs:696: 'helpClosedFrame' is referenced outside the reviewed methods (qualified, … |
| V-L03 | limit | non-terminating do/while before the Objectives caller (disclosed L-03) | lim | accepted (disclosed limit) | 0 |  |  |  |
| V-L04 | limit | reflection writes guarded state by string name (disclosed L-04) | lim | accepted (disclosed limit) | 0 |  |  |  |
| V-L07 | limit | exception thrown by an expression before the Objectives caller (disclosed L-07) | lim | accepted (disclosed limit) | 0 |  |  |  |

## Commands and raw tallies

`E=/media/jer/Files/CCF/Verification/CTA-314-IndependentReview-20261107`. All Python ran with `PYTHONDONTWRITEBYTECODE=1 PYTHONPYCACHEPREFIX=$E/pycache TMPDIR=$E/tmp`. Python 3.12.3.

| # | What | Result | Raw |
|---|---|---|---|
| 1 | `python3 -m py_compile` on the three scripts | exit 0, no output | `final/01-py_compile.out` |
| 2 | Author suite `python3 -B test_objective_cta_source_check.py` | exit 0; `rejected=219/219 false_greens=0 wrong_cause=0`; `benign 32/32`; historical rejected with 3 rows; stderr empty | `final/02-suite.*` |
| 3 | Candidate `check_baseline.py --rotation-state post-fix --instruction-state post-fix`, cwd pinned to SDK 8.0.133 | `PASS=10 FAIL=0 NOT-TESTABLE=2`, exit 0, stderr empty; stdout sha256 `1facb1ff4202c072…` | `final/05-candidate.*` |
| 4 | Same, traced (`COREHOST_TRACE=1`, `strace -f -e trace=execve,openat`, in-process compiler) | byte-identical stdout, exit 0 | `final/06-*` |
| 5 | Same from a cwd with no `global.json` (control) | byte-identical stdout, exit 0, but SDK 10.0.114 | `final/07-*` |
| 6 | Historical `72b4efbd` plus the three amended scripts (untracked) | `PASS=6 FAIL=4 NOT-TESTABLE=2`, exit 1, `tracked-production-vs-baseline: MATCH`; differs from `baseline-output.txt` at **line 48 only** (check 08's text); byte-identical to #305's regenerated output | `final/08-*` |
| 7 | #305 corpus with my driver (`run_corpus.py`) | 102/102 rejected, 15/15 benign accepted, 1585ca20 accepts 52 | `evidence/02-corpus305.stdout`, `corpus305/results.json` |
| 8 | Original/#304 controls from the 1585ca20 file (`run_orig55.py`) | 55/55 rejected, 4/4 accepted | `evidence/05-orig55.stdout`, `orig55/results.json` |
| 9 | My six rounds (`run_my.py`) | see the table above | `evidence/03-mine-round1..6.stdout`, `mine*/results.json`, `evidence/10-mine-tally.txt` |
| 10 | Roslyn C# 9 compile of every case (`roslyn.sh`) | see above | `evidence/04-roslyn-*` |
| 11 | End-to-end runs (`e2e.sh`) | 35 runs through the real `check_baseline.py` | `evidence/06-*`, `evidence/10-e2e-table.txt`, `e2e-out/` |
| 12 | Live-source lexing and Cf scan | 198/198 files lex; no Cf or non-ASCII code | `evidence/08-lexer-live-assets.txt` |
| 13 | In-flight tips in memory (`tip_probe.py`) | accepted at `b4d47d3d`, `69b41605`, `9789369d`, feature tip | `evidence/07-tip-probe.txt` |
| 14 | Scratch fix feasibility (`fix_feasibility.py`) | see above | `evidence/09-fix-feasibility-*.txt` |
| 15 | Final integrity re-hash | all five files identical to the pinned blobs; remote tip `b4d47d3d` | `evidence/11-final-integrity.txt` |

## SDK evidence

The checker's `dotnet run` inherits the Python process's working directory, and the SDK is chosen from that directory's `global.json`. A bare `dotnet --version` is not proof, so I traced the checker's own process tree.

- Runs 3 and 4 started from `$E/sdk8/cwd/deeper`, beneath `$E/sdk8/global.json` = `{ "sdk": { "version": "8.0.133", "rollForward": "disable" } }`. `TMPDIR` was `$E/tmp`, which has no `global.json`. `dotnet --version` there: **8.0.133**; from `$E/control-cwd`: **10.0.114**. Both are installed (`dotnet --list-sdks`).
- **Process trace (run 4).** `strace` shows the checker (`python3 … check_baseline.py`) exec `/usr/bin/dotnet run --project <tmp>/check.csproj --verbosity quiet`, and, in the same traced tree, `/usr/lib/dotnet/dotnet exec /usr/lib/dotnet/sdk/8.0.133/Roslyn/…/csc.dll`. **281 successful opens under `sdk/8.0.133`, 0 under `sdk/10.0.114`.** `hostfxr` probes `…/sdk8/cwd/deeper/global.json`, then `…/sdk8/cwd/global.json`, then `Found global.json […/sdk8/global.json]`, `Resolving SDKs with version = '8.0.133', rollForward = 'disable'`, `Using .NET SDK dll=[/usr/lib/dotnet/sdk/8.0.133/dotnet.dll]`.
- **Control (run 5).** No `global.json`: `version = 'latest'` resolves to `sdk/10.0.114/dotnet.dll`; **504 opens under `sdk/10.0.114`, 0 under `sdk/8.0.133`**. The trace therefore discriminates.
- The output is byte-identical across the default, pinned, traced and control runs (sha256 `1facb1ff4202c072fa2df0b795b53a0457a5f3aef58cf0e0eafcbba3b2e3b011`), which also equals the sha256 the author reported.
- The generated project compiles exactly `RackGeometry.cs` and `RackEditing.cs` plus the six-case driver: real production code on plain .NET, no mock, no Unity.

```text
SOURCE-STATIC PASS SnapFar near-angle enabled=True input=(20.000,0.700) far=(20.012,0.000) angle=0.000 angleSnapped=True boundarySnapped=False
SOURCE-STATIC PASS SnapFar near-angle-disabled enabled=False input=(20.000,0.700) far=(20.000,0.700) angle=2.005 angleSnapped=False boundarySnapped=False
SOURCE-STATIC PASS SnapFar outside-angle-window enabled=True input=(20.000,3.000) far=(20.000,3.000) angle=8.531 angleSnapped=False boundarySnapped=False
SOURCE-STATIC PASS SnapFar near-boundary enabled=True input=(39.000,0.000) far=(40.000,0.000) angle=0.000 angleSnapped=False boundarySnapped=True
SOURCE-STATIC PASS SnapFar near-boundary-disabled enabled=False input=(39.000,0.000) far=(39.000,0.000) angle=0.000 angleSnapped=False boundarySnapped=False
SOURCE-STATIC PASS SnapFar outside-boundary-window enabled=True input=(37.000,0.000) far=(37.000,0.000) angle=0.000 angleSnapped=False boundarySnapped=False
```

## What remains unproved

**Not run, and not claimed:** Unity Editor compile or tests; rendering and screenshots; native pointer or keyboard input; player focus and consent; Help-close timing; save/load, Legacy40 and Reference scenarios; Windows or Player build; USS and other style sources; the exact integrated candidate (only an in-memory scan of the feature tips); any merge. Runtime effects described for mutants are code reasoning plus a Roslyn compile; nothing executed them.

**Unproved by this review.** That my three findings are the *only* remaining ones (I fitted my cases after reading the code, and a different model family might find others). That the lexer matches Roslyn on every input (I tested the 198 live files, the corpus and the shapes above; I did not test every Unicode category). That the stored pinned text is the *right* behaviour (it is the reviewed `1585ca20` source). That the scratch guards are complete. Behaviour of classes outside the companion set, reflection, aliasing in companions, non-termination and exceptions (the disclosed L-01 to L-07).

## Final state

- Checker, test, README, `check_baseline.py` and `baseline-output.txt` re-hashed at the end: **identical** to the pinned blobs and to my pre-run hashes (`evidence/11-final-integrity.txt`). Remote branch tip and the author lane's `HEAD` still `b4d47d3d`. I edited nothing under review.
- Evidence stays at `$E` (`source-clone` about 5 GiB, plus small clones); the clones may be deleted after the Manager rules, the evidence should stay. `SHA256SUMS.txt` there covers the scripts and evidence.
- This report is the only file published from the docs lane. The hash of the commit that adds it is in the #314 `[HANDED-OFF]` comment, because a file cannot contain its own commit hash.

---

# Exact-head re-review after repair loop 1 (head `753143e6`, appended 2026-11-07)

> **Everything above this line is my first review of `b4d47d3d` and is kept as written.** Where this section shows the first review was wrong or too cautious, the [corrections](#corrections-to-the-first-review-above) say so explicitly.

**Reviewer verdict on the exact head `753143e62a5cf1d7436874e731766c4893a4c7c7` (code `e29e3e1b`): NEEDS CHANGES — source scope only, and narrowly.** This is reviewer evidence and opinion for Manager #200. It is **not** Manager acceptance and cannot approve the checker. I changed no source.

> **The one authoring loop budget is now SPENT.** No further repair will be scheduled tonight, whatever this verdict says. Every residual defect below goes to Manager #200 for morning disposition. A precisely scoped future-fix note is in [Future-fix note](#future-fix-note-for-manager-200-not-a-repair).

> **Independence status, restated.** Cross-provider review was unavailable tonight (OpenAI quota exhausted). This re-review is again the **same-provider-family fallback**: T3 delegate reviewer, provider `claudeAgent_claude_b_pro` (Claude B - Pro), model `claude-sonnet-5-5`, effort **XHIGH**, against an author on `claude-opus-5-5` (Claude A). I am the same reviewer as in the first review, so I am not independent of my own earlier cases (I reused them) and I wrote the new attacks below after reading the loop-1 diff. A reviewer from another model family might find more, or fewer, than I did.

## Re-review summary

**The repair does what it says for the thing I found, and my own earlier finding list is closed.** Observed, with my own drivers and fresh runs (not the author's tallies):

- All **25** in-scope end-to-end survivors of the first review are now rejected through the real `check_baseline.py` (exit 1, `PASS=9 FAIL=1 NOT-TESTABLE=2`), each for its intended cause: **S-1 10/10, S-2 11/11, S-3 4/4**. The 26th, `W-DEP-05` (`ReloadProgress` marks every lesson done), stays accepted (`10/0/2`, exit 0) and is a documented out-of-scope limit. My 3 lone-CR cases are now rejected **by a rule** (U+000D not followed by LF), not by the file reader.
- Of the 29 in-memory false greens of the first review, **28** are rejected; the 112 cases that were rejected before are still rejected; all 21 of my benign cases are still accepted.
- The gates reproduce exactly: suite `rejected=275/275 false_greens=0 wrong_cause=0`, benign 32/32 + 32/32, candidate `10/0/2` exit 0, historical `6/4/2` exit 1, six SnapFar on a trace-proved SDK 8.0.133. The diff is exactly the five allowed paths.
- The author's new claims hold on every point I could re-run: 26 new mutations all rejected (and 25 of them accepted by `b4d47d3d`), 54/54 transcriptions byte-identical, all 106 loop-1 suite cases give the expected exit through the real pipeline, the NBSP-indented `#endif` desync is real (Roslyn differential) and now closed.

**It is still not a closed verifier.** I wrote **478 new cases** (rounds 7 to 11, below) after reading the new code. All compile at C# 9 against the real Unity 6000.6 DLLs unless stated. **57 of them are accepted by the repaired checker, in two classes the loop-1 text says are closed:**

| ID | What gets through | Cases (valid C# 9, accepted in memory) | End to end (`10/0/2`, exit 0) | Severity (my view) |
|---|---|---|---|---|
| **N-1** | **S-1 is closed for statements, not for interpolation holes.** `_tokens` emits a whole interpolated string as one token, so the new write-site rule never sees code inside `{ … }`, and the older count rule only matches `x =`, `x op=`, `x++`, `ref|out x`. A **tuple, parenthesised or `out|ref this.x` write of any of the 7 related-state fields, or `++screen`, hidden in a hole, is accepted.** The original `Z-RR2-T-help` effect (`help` becomes null, guidance suspended for good) is reachable by wrapping it as `Debug.Log($"{((help, workPlanWasOpen) = (null, false))}");` | 47 (A10: 28, A11: 19) | 11 of 11 run | Medium (blocking by the packet's rule); needs a deliberately written hole, no accidental route |
| **N-2** | **The new exact-text pins are not tied to the member they pin ("decoy").** The reviewed text is searched "exactly once anywhere in the file". A nested class, a nested struct or a separate top-level class holding the reviewed text lets the real member change. Defeats the new **S-1 declaration pins** (`help`, `screen`, `manager` become properties that read null or `Map`) and the new **S-3 routing-input pins** (`NeedsAnnualReview => true`, `TeachingRacks => true`) | 10 | 10 of 10 run | Medium for accuracy (README and handoff claim the opposite), Low for safety (needs a deliberately built unused type) |

Both classes were also accepted by `b4d47d3d`, so neither is a regression from the repair. They are residuals the repair either did not reach (N-1) or claims to have closed (N-2: "A field cannot become a property that drops writes", handoff; "Each field declaration is pinned", README). **Neither has an accidental route**: each needs a hostile or at least deliberately odd edit. That is why I rank them Medium and not higher, and why I think the checker is already fit as a guard against *accidental* regressions. The packet's own rule ("any surviving covered mutation is blocking") and the README's explicit claims are why the verdict is still NEEDS CHANGES.

Everything is **source and offline-tool evidence**. Not run, and not claimed: Unity compile or tests, rendering, native input, focus, Help-close timing, saves, Legacy40/Reference, Player builds, the integrated candidate, any merge.

## Re-review identity, lanes and method

| Item | Observed |
|---|---|
| Role / identity | T3 delegate reviewer for #314, same session family as the first review; provider `claudeAgent_claude_b_pro` (Claude B - Pro), `claude-sonnet-5-5`, effort XHIGH (set by the Mimo orchestrator). Authority: review only. No checker, test, fixture or production file was edited. No Unity lease, no Unity |
| Candidate (verified at start and end) | `verify/scenario-one-feedback-cta-anchor-20261106` = `753143e62a5cf1d7436874e731766c4893a4c7c7` (docs commit, author `jermur788`), parent code commit `e29e3e1bfb943980a2fe7710449c0052ced90823`, grandparent `b4d47d3d45717be9da159f0368407b10c0c3e6fe`. `git ls-remote` on the bare store and a `rev-parse` of the author lane (the only command I ran there) both read `753143e6` at the end |
| Re-review lanes (all mine, throwaway) | `$L = $E/loop1-rerun` with `src` (sparse shared clone, detached `753143e6`), `e2e` (same, overlaid and restored per run), `old` (sparse clone at `72b4efbd` plus the three amended scripts copied in untracked), `ovl` (overlay clone in which I edited the **test** file on purpose to prove the probe tripwire; never committed). The first review's `source-clone` (still `b4d47d3d`) served as the "before" scanner. `E=/media/jer/Files/CCF/Verification/CTA-314-IndependentReview-20261107` |
| Read-only to me | `/media/jer/Files/CCF/Operations/CCF-FeedbackCTARepair-20261106` (one `rev-parse HEAD`), the author's loop-1 evidence dir (not opened; I read the handoff addendum from the pushed commit instead), `/media/jer/Files/CCF-Primary` (shell cwd only; I ran no git write there this time and no `git fetch` there) |
| Writes outside my lanes | None. Stray `__pycache__/objective_cta_source_check.cpython-312.pyc` files created by a scratch import in two of my own clones were removed. The Roslyn build server my end-to-end runs started (pid 2158094, SDK 8.0.133, started at 05:04) was stopped; no `dotnet` server of mine was left (`ps` empty at the end) |
| Toolchain | Python 3.12.3; .NET SDKs 8.0.133 (pinned by cwd `global.json`) and 10.0.114; Roslyn C# 9 tool built in the first review (`$E/roslyn-tool`) against the 320 real Unity 6000.6 and project DLLs, Player-like defines, baseline 189 files / 0 errors |
| Cases | My six first-review case files were copied **unchanged** (`cmp` identical) and re-run at the new head, with the `b4d47d3d` scanner as the "before" column: 176/176. New rounds 7 to 11 are in `$L/scripts/my_mutations7..11.py` |
| Claim receipts | #314 `[RUNNING — rerun]` posted at start; the `[HANDED-OFF — exact-head re-review]` comment carries the hash of the commit that adds this section |

## Results by re-review area

| # | Mandatory area | Result | Basis |
|---|---|---|---|
| 1 | Diff scope, earlier blocks, `baseline-output.txt` | **PASS** | [Diff scope](#1-diff-scope) |
| 2 | My 176 cases, full re-run (26 e2e survivors, 3 lone CR, 112 stay rejected, 21 benign) | **PASS** | [Reconciliation](#2-reconciliation-of-the-first-reviews-survivors-with-the-authors-accounting) |
| 3 | Author's new claims, fresh runs | **PASS** (every number reproduced) | [Author claims](#3-the-authors-new-claims-fresh-runs) |
| 4 | Gates (suite, candidate, historical, SDK trace, `py_compile`, Roslyn) | **PASS** | [Gates](#4-gates) |
| 5 | Over-rejection audit O-05 to O-09 and live Assets | **PASS** (disclosed, fail-closed only, no live source rejected) | [Audit](#5-over-rejection-audit) |
| 6 | Limits honesty | **PARTIAL FAIL** (two overclaims: N-1, N-2) | [Limits](#6-limits-honesty) |
| 7 | Integrity | **PASS** | [Integrity](#7-integrity) |
| — | **My own new attacks on the repaired code (rounds 7 to 11)** | **FAIL**: 57 accepted | [New attacks](#new-attacks-written-after-reading-the-loop-1-diff-rounds-7-to-11) |

## Findings after repair loop 1 (severity-ranked)

Severities are my recommendation; the Manager rules. In this section `$L` is `$E/loop1-rerun`. Case sources are in `$L/scripts/my_mutations7..11.py`, per-case results in `$L/out/mine7..11/results.json`, compile results in `$L/evidence/04-roslyn-l1-mine*.txt`, end-to-end output in `$L/e2e-out/`.

### N-1 — Medium, blocking by the packet's rule: S-1 is not closed for writes inside interpolation holes

- **Where.** `objective_cta_source_check.py` `_tokens` (`:949-974`) turns an interpolated string into **one** token (`istr`); `_guard_related_state` (`:1861-1913`) runs only over those tokens, so a write inside `$"{ … }"` is invisible to the new site rule. The older count rule `_assignments` (`:110-117`) still runs first and sees holes, but only for `x =`, `x op=`, `x++` and `ref|out x`. Everything the loop-1 text promises beyond those shapes is therefore not enforced inside a hole.
- **Cases (all valid C# 9, Roslyn 0 errors except the one invalid case, which is rejected anyway).** Seven related-state fields (`help`, `manager`, `map`, `workPlan`, `review`, `rackLayout`, `screen`) × four shapes in an interpolated string in an unpinned root method: tuple target `$"{((F, _) = (null, 0))}"`, tuple target in second position, parenthesised target `$"{((F) = null)}"`, and the same tuple inside `$@"…"` = **28 accepted** (`A10-hole-{tuple,tuple2,paren,verbatim}-{field}`). Plus `ZOut(out this.F)` and `ZRef(ref this.F, null)` in a hole for 6 fields (12), `++screen` and `--screen` in a hole (2), and five **called-site** variants appended to `ShowObjectives` exactly like the original `Z-RR2-T-*` survivors (`A11-called-hole-help`, `-screen`, `-manager`, `-paren-help`, `A11-called-lambda-hole-help`) = **19 accepted** in round 11. The control, the identical tuple as a plain statement (`A11-called-stmt-ctl-help`), is rejected for the intended cause.
- **End to end.** 11 representative cases run through the real `check_baseline.py` from the SDK-8.0.133-pinned cwd: `A10-hole-tuple-help`, `A10-hole-paren-screen`, `A10-hole-verbatim-manager`, `A11-called-hole-help`, `-called-hole-screen`, `-called-hole-manager`, `-called-hole-paren-help`, `-called-lambda-hole-help`, `-hole-outthis-help`, `-hole-prefixinc-screen`, `-hole-refthis-rackLayout`: all `PASS=10 FAIL=0 NOT-TESTABLE=2`, exit 0, stderr empty (`$L/evidence/06-e2e-l1-round10-11.txt`).
- **What it would do (code reasoning, not run).** `A11-called-hole-help` appends `Debug.Log($"{((help, workPlanWasOpen) = (null, false))}");` to `ShowObjectives`. Opening the Objectives screen then sets `help` to null, and the pinned `ApplyNextActionGuidance` reads `help == null` as "suspended" for good: the effect of the rejected `RR2-10`, and of the first review's `Z-RR2-T-help`. `A11-called-hole-screen` sets `screen = Map` right after `SetScreen(Objectives)`, so the Objectives screen resolves its guidance against `map.Root`.
- **Why it matters / contract link.** It is the S-1 class the loop was meant to close ("tuple, parenthesised … `ref`/`out` … whatever the count", README) with the same effect, behind one more layer of syntax. Existing guarded names (`guidanceCandidate`, `helpClosedFrame`, `nextAction`) **are** protected in holes (position-based rules over the masked code: `A11-hole-tuple-guarded`, `-helpClosedFrame`, `-this-assign-nextAction` rejected), so the gap is specific to the new token rule.
- **Not a regression.** `b4d47d3d` accepts all of them too (my driver, `b4d` column). It is a residual that the repair's own README sentence claims to have removed.
- **How I found it.** I first neutralised the old count rule in memory (patching `_assignments` to return the reviewed counts for the 8 field names; `scripts/r7_site_rule_alone.py`) to test the new rule on its own: it rejected 340 of 347 round-7 attacks alone, and the **only seven it missed were the seven `interp` cases**, which is what pointed at holes. I report this because it also shows the new site rule itself is good: apart from holes, no other write shape in my 25-shape matrix gets past it.
- **Suggested narrow fix (same file; scratch result below).** Apply `_state_write` to the tokens of each interpolation hole too (the lexer already keeps `(start, close, inner)` per hole), or ban related-state names inside holes outside the pinned bodies.

### N-2 — Medium for accuracy, Low for safety: exact-text pins are not tied to the member they pin ("decoy")

- **Where.** `_snippet_spans` (`:1066-1085`) requires the reviewed token sequence to occur **exactly once in the file**. It does not check what encloses it. The new S-1 declaration pins (`_STATE_DECLARATIONS`) and S-3 routing-input pins (`_ROUTING_INPUT_ROOT`, `_ROUTING_INPUTS_VIEW`) use it. A second type in the same file that carries the reviewed text satisfies the pin while the real member changes.
- **Cases (all valid C# 9, all accepted in memory and end to end `10/0/2`, exit 0).**

| ID | Mutation (real member) | Decoy that keeps the reviewed text once |
|---|---|---|
| `A8-D1-help-prop-decoy` | `private MenuHelpView help { get => null; set { } }` (guidance suspended permanently) | nested `class ZDecoyHelp { private MenuHelpView help; }` |
| `A8-D2-screen-prop-decoy` | `private UiScreen screen { get => UiScreen.Map; set { } }` | nested class with `private UiScreen screen = UiScreen.None;` |
| `A8-D3-manager-prop-decoy` | `private ScenarioOneManager manager { get => null; set { } }` | nested class with the declaration |
| `A8-D4-nar-decoy` | `public bool NeedsAnnualReview => true;` | nested class with its own `manager` field and the reviewed expression |
| `A8-D5-teaching-decoy` | `TeachingRacks => true; CurrentTopicIndex => 0` | nested class in the view with both reviewed members |
| `A8-D5b-teaching-decoy-simple` | `public bool TeachingRacks => true;` | nested class with `TeachingRacks` text (control `A8-D5c`, no decoy: **rejected**, so the pin works against the plain edit) |
| `A9-D7-toplevel-decoy-root` | `NeedsAnnualReview => true` | a **separate top-level class** in the root file |
| `A9-D8-struct-decoy-view` | `TeachingRacks => true` | nested **struct** in the view |
| `A9-D9-help-decoy-struct` | `help` property | nested struct |
| `A9-D10-help-realistic` | `help` returns null only while `screen == None` (backing field `helpBacking`) | nested class |

- **Contract link / overclaim.** README: "Each field declaration is pinned"; handoff: "A field cannot become a property that drops writes"; README and the check-08 PASS note: the routing is selected "as far as the pinned routing text and its pinned UI inputs decide". For a decoy, the pins hold while the input is not the reviewed one. The older snippet pin for `learning` is not affected (`A9-D11-learning-decoy` rejected by the alias rule).
- **Extent.** Method pins (`_pin`) are not decoyable in a practical way (the decoy would have to replicate every field the body uses and not trip the "exactly one method with that signature" rule); I did not find a route. Only short declaration or one-line-member pins are affected.
- **Not a regression**, as for N-1 (`b4d47d3d` accepts all of them; the control shows the repair's S-3 pin does close the plain edit).

### N-3 — Info: the "unpinned screen entry points" limit names two methods; the class is every unpinned root method

README: "The unpinned screen entry points `ShowObjectives` and `CloseAll` can route elsewhere (probes Y-RR3-17, Y-RR3-18)". `A7-x-update-setscreen` (an `if (screen == Objectives) SetScreen(Map);` at the top of `Update()`, valid C# 9) is accepted too. It is the same documented N-2 (#305) class, so the limit is real but under-described. Suggest "any unpinned root method that calls `SetScreen`, `ShowMap`, `ShowObjectives`, or `CloseAll`".

### N-4 — Info: undisclosed-by-name over-rejections of the L-06 closure

The README states the rule exactly (`.Learning` may only be followed by a member access other than `Root` or by `== null`/`!= null`), and O-08 names `null != ui.Learning`. Four idiomatic benign companion reads are rejected by that rule but are not in the O list: `ui.Learning is null`, `ui.Learning is not null` (the C# 9 forms), `ui.Learning ?? fallback`, and a local alias used only for reading (`var l = ui.Learning; … l.CurrentTopicIndex`) (`A8-L06-isnull`, `-isnotnull`, `-coalesce`, `-localalias-read`). All fail closed, none touches today's source, and the README sentence covers them. Cost: a future false red with a message that says "alias or pass-on". I rate it an accepted trade-off.

### N-5 — Info: a non-UTF-8 `.cs` file under `Assets/` stops the whole script with `ERROR`, exit 2

`LATIN1-comment` (a Latin-1 byte in a comment of a new file; Roslyn accepts it) gives `SOURCE-STATIC ERROR: 'utf-8' codec can't decode byte 0xe9 …`, exit 2. Pre-existing (`Path.read_text(encoding="utf-8")` in `collect_companions` and the other checks), fail-closed (never green), and the message does not name the file. No action needed for tonight.

## Closure of the first review's findings

| First-review finding | Status at `753143e6` | Evidence |
|---|---|---|
| **S-1** count-only RR-2 signature (10 e2e cases) | **Closed for statements, 10/10.** Open inside interpolation holes (**N-1**) | e2e table; rounds 7, 10, 11 |
| **S-2** Cf identifiers and line-separator comment terminators (11 e2e cases + 3 lone CR) | **Closed, 11/11 and 3/3.** My differential probe shows the same hole existed for six more whitespace classes in directives (below); all are closed now | e2e table; [probe](#the-nbsp-indented-endif-desync-my-own-differential-probe) |
| **S-3** routing inputs unpinned (`W-DEP-01` to `04`) | **Closed 4/4 against the plain edit.** Decoy-bypassable (**N-2**). `W-DEP-05` is a documented out-of-scope limit | e2e table; N-2 |
| **S-4** limit probes not in the repo | **Closed.** 9 accepted-limit + 9 over-rejection probes are in the suite, reported and not counted; changing an outcome fails the run (I proved it with an overlay, below) | `evidence/13-probe-overlay.*` |
| **S-5** L-06 (alias of `ui.Learning`) | **Closed for the alias and pass-on shapes.** I tried 7 new evasions (comment/newline separators, `@Learning`, `?.`, tuple deconstruction, lambda argument, generic argument, switch expression) on top of my 2 first-review probes and the author's 3; none passed. See correction below | `A8-L06-*` |
| **S-6** maintenance cost / message quality | Unchanged by design; now more fail-closed rules (O-05 to O-09, N-4) | audit below |

### Corrections to the first review above

1. **Lone CR.** I wrote that the three lone-CR cases were "closed by accident, not as a false green". They are now closed by a rule (`_FOREIGN_LINE_BREAK`, `\r(?!\n)`), and the in-memory API rejects them with "U+000D is a line break for the C# compiler …". The first review's description of the pre-repair state stands.
2. **S-5.** I wrote that I "could not find a lexical rule that closes [L-06] without banning legitimate `ui.Learning.X` uses". The author found one (a follow-set after `.Learning`), and I could not beat it with 7 new evasions plus my 2 earlier probes. My statement was too pessimistic. The rule does over-reject a few idiomatic reads (N-4).
3. **"112 previously rejected."** The packet's figure is reproduced exactly: of my 176 cases, **112** were rejected by `b4d47d3d` (100 expected-rejected, 11 exploratory, 1 limit probe `Y-RR3-11`) and all 112 are still rejected.

## 1. Diff scope

| Check | Observed (`$L/evidence/01-diff-scope.txt`, `09-preserved-blocks.txt`) |
|---|---|
| Changed paths `b4d47d3d..753143e6` | Exactly five: `check_baseline.py` (+2/−1), `objective_cta_source_check.py` (+281/−13), `test_objective_cta_source_check.py` (+545/−4), `README.md` (+7/−6) and the handoff (+191, appended). By commit: code `e29e3e1b` touches the README and the three scripts; the docs commit `753143e6` touches the handoff and one README sentence |
| Against `1585ca20` | The same five paths (handoff added). 0 merge commits. `Assets`, `Packages`, `ProjectSettings`: **0 files changed**. `git diff --check` exit 0 |
| `baseline-output.txt` | Blob `db6a8b850dce7efadaac6fe1b87b1dbba469428f` at `1585ca20`, `b4d47d3d`, `e29e3e1b` and `753143e6`; sha256 `513cddd9babddaede82212f8e4891ecfcf9823c13a69474a3665f890a7f05dc1` in the tree. Byte-preserved |
| `check_baseline.py` | One hunk: the test-08 PASS note, reworded from "selected only for unconfirmed Rack V2 Objective 1" to "as far as the pinned routing text and its pinned UI inputs decide (the game state they read is not checked)". Checks 01 to 07 and 09 untouched. The new wording is accurate except for N-2 |
| Earlier test blocks | Lines 28 to 198 of the `1585ca20` test file (sha256 `6119b5fa…`) occur verbatim, once, in the new file. Lines 33 to 295 of the #305 reviewer's script (sha256 `9a7a8fd2…`) occur verbatim, once. The `rr1`, `rr2`, `rr3`, `n4` and `new_benign` tables are identical to `b4d47d3d`. A line diff shows exactly **4** removed lines in the test file: two docstring lines, one import line and the exit condition (now also fails on a changed probe outcome) |
| README | Reworked for S-1, S-2, S-3, L-06 and the probe list; no unrelated section changed. Overclaims are listed in [Limits honesty](#6-limits-honesty) |

## 2. Reconciliation of the first review's survivors with the author's accounting

Author: "S-1 10/10, S-2 11/11, S-3 4/4, with `W-DEP-05` kept as a documented out-of-scope probe, plus 3 lone-CR cases rejected by a rule". Mine, from my own case list (first-review report, [end-to-end table](#end-to-end-through-the-real-check_baselinepy-pinned-sdk-8133-cwd)) re-run at `753143e6` through the real pipeline:

| Family (first review) | In my list | Now rejected e2e (exit 1, `9/1/2`) | Still green | Agrees with author |
|---|---|---|---|---|
| S-1 (`Y-RR2-08`, `Z-RR2-T-{help,screen,rackLayout,map,workPlan,review}`, `U-RR2-T-manager`, `Z-RR2-C-{screen,help}`) | 10 | **10** | 0 | yes |
| S-2 (`Y-N-10` to `15`, `Z-N-CF-{buttons,secondprimary,allowlist-clear}`, `Z-N-SHY`, `Z-N-PS`) | 11 | **11** | 0 | yes |
| S-3 (`W-DEP-01` to `04`) | 4 | **4** | 0 | yes |
| S-3 `W-DEP-05` | 1 | 0 | **1** (exit 0, `10/0/2`) | yes: documented out-of-scope, kept as an in-suite accepted-limit probe |
| Lone CR (`Y-N-09`, `Z-N-CR-region`, `Z-N-CR-root`) | 3 | 3 (e2e: through the universal-newline read) and **3/3 in memory by rule** | 0 | yes |
| **Total** | 29 | **28** (25 in scope + 3 lone CR) | **1** (`W-DEP-05`) | yes |

Of my 29 in-memory false greens: **28 rejected**, **1 accepted (`W-DEP-05`)**; of my 5 S-3 cases, 4 are rejected and the one that is not is the documented one, so **no S-3 case other than the documented out-of-scope one is green**. All 29 compile with 0 errors (first review), and every other class is unchanged:

| My 176 cases | At `b4d47d3d` (reproduced with my driver) | At `753143e6` |
|---|---|---|
| Expected rejected: 129 | 100 rejected, **29 accepted** (identical to the first review) | 128 rejected, **1 accepted** (`W-DEP-05`) |
| Previously rejected, all classes: 112 (100 + 11 exploratory + `Y-RR3-11`) | 112 | **112 still rejected, 0 regressions** |
| Expected benign: 21 | 21 accepted | **21 accepted**, 0 over-rejections |
| Disclosed-limit probes: 14 | 13 accepted | 11 accepted, `V-L06-1` and `V-L06-2` now rejected (L-06 closed), `Y-RR3-11` as before |
| Exploratory: 12 | 11 rejected, `Z-RR4-13` accepted | unchanged |

Also re-run with my own drivers: the #305 corpus (102 broken: **102 rejected**, 15 benign: **15 accepted**) and the original/#304 set (55 rejected, 4 benign accepted). The columns labelled `old1585` in `evidence/02-corpus305.stdout` are, in this rerun, the `b4d47d3d` scanner.

The 29 former in-memory false greens, with the cause at the new head:

| ID | Family | b4d47d3d | 753143e6 | Rejection cause at 753143e6 (abridged) |
|---|---|---|---|---|
| Y-RR2-08 | S-1 | accepted | **REJECTED** | ScenarioOneUiRoot.cs:681: pinned method TryShowReviewYear (RR-2 related state 'screen') changed (expected 'Review', found 'None'); a reviewed method m |
| Y-N-09 | lone CR | accepted | **REJECTED** | LearningObjectivesView.Sequence.cs:9: U+000D is a line break for the C# compiler but not for this lexer (it can end a // comment or a preprocessor lin |
| Y-N-10 | S-2 | accepted | **REJECTED** | LearningObjectivesView.Sequence.cs:9: U+2028 is a line break for the C# compiler but not for this lexer (it can end a // comment or a preprocessor lin |
| Y-N-11 | S-2 | accepted | **REJECTED** | LearningObjectivesView.cs:82: character U+200D (category Cf) in code: the C# compiler reads it differently from this lexer (a format character inside  |
| Y-N-12 | S-2 | accepted | **REJECTED** | LearningObjectivesView.Cf.cs:1: character U+200D (category Cf) in code: the C# compiler reads it differently from this lexer (a format character insid |
| Y-N-13 | S-2 | accepted | **REJECTED** | LearningObjectivesView.Sequence.cs:9: U+0085 is a line break for the C# compiler but not for this lexer (it can end a // comment or a preprocessor lin |
| Y-N-14 | S-2 | accepted | **REJECTED** | ScenarioOneUiRoot.cs:696: character U+200D (category Cf) in code: the C# compiler reads it differently from this lexer (a format character inside an i |
| Y-N-15 | S-2 | accepted | **REJECTED** | ScenarioOneUiRoot.cs:696: character U+200D (category Cf) in code: the C# compiler reads it differently from this lexer (a format character inside an i |
| Z-RR2-T-help | S-1 | accepted | **REJECTED** | ScenarioOneUiRoot.cs:696: 'help' is written outside its reviewed write site (tuple or deconstruction target): tuple, ref/out, qualified, compound, mov |
| Z-RR2-T-screen | S-1 | accepted | **REJECTED** | ScenarioOneUiRoot.cs:696: 'screen' is written outside its reviewed write site (tuple or deconstruction target): tuple, ref/out, qualified, compound, m |
| Z-RR2-T-rackLayout | S-1 | accepted | **REJECTED** | ScenarioOneUiRoot.cs:696: 'rackLayout' is written outside its reviewed write site (tuple or deconstruction target): tuple, ref/out, qualified, compoun |
| Z-RR2-T-map | S-1 | accepted | **REJECTED** | ScenarioOneUiRoot.cs:696: 'map' is written outside its reviewed write site (tuple or deconstruction target): tuple, ref/out, qualified, compound, move |
| Z-RR2-T-workPlan | S-1 | accepted | **REJECTED** | ScenarioOneUiRoot.cs:696: 'workPlan' is written outside its reviewed write site (tuple or deconstruction target): tuple, ref/out, qualified, compound, |
| Z-RR2-T-review | S-1 | accepted | **REJECTED** | ScenarioOneUiRoot.cs:696: 'review' is written outside its reviewed write site (tuple or deconstruction target): tuple, ref/out, qualified, compound, m |
| Z-RR2-C-screen | S-1 | accepted | **REJECTED** | ScenarioOneUiRoot.cs:681: pinned method TryShowReviewYear (RR-2 related state 'screen') changed (expected 'screen', found 'CloseHelp'); a reviewed met |
| Z-RR2-C-help | S-1 | accepted | **REJECTED** | ScenarioOneUiRoot.cs:1: RR-2 related state 'help': expected exactly one reviewed occurrence of `help = new MenuHelpView(this);` (missing, changed or d |
| Z-N-CR-region | lone CR | accepted | **REJECTED** | LearningObjectivesView.Sequence.cs:9: U+000D is a line break for the C# compiler but not for this lexer (it can end a // comment or a preprocessor lin |
| Z-N-PS | S-2 | accepted | **REJECTED** | LearningObjectivesView.Sequence.cs:9: U+2029 is a line break for the C# compiler but not for this lexer (it can end a // comment or a preprocessor lin |
| Z-N-CR-root | lone CR | accepted | **REJECTED** | ScenarioOneUiRoot.cs:696: U+000D is a line break for the C# compiler but not for this lexer (it can end a // comment or a preprocessor line); fail clo |
| Z-N-CF-buttons | S-2 | accepted | **REJECTED** | LearningObjectivesView.cs:83: character U+200D (category Cf) in code: the C# compiler reads it differently from this lexer (a format character inside  |
| Z-N-CF-secondprimary | S-2 | accepted | **REJECTED** | LearningObjectivesView.cs:192: character U+200D (category Cf) in code: the C# compiler reads it differently from this lexer (a format character inside |
| Z-N-CF-allowlist-clear | S-2 | accepted | **REJECTED** | ScenarioOneUiRoot.cs:696: character U+200B (category Cf) in code: the C# compiler reads it differently from this lexer (a format character inside an i |
| Z-N-SHY | S-2 | accepted | **REJECTED** | LearningObjectivesView.cs:82: character U+00AD (category Cf) in code: the C# compiler reads it differently from this lexer (a format character inside  |
| W-DEP-01 | S-3 | accepted | **REJECTED** | ScenarioOneUiRoot.cs:1: RR-3 routing input 'NeedsAnnualReview': expected exactly one reviewed occurrence of `public bool NeedsAnnualReview => manager  |
| W-DEP-02 | S-3 | accepted | **REJECTED** | LearningObjectivesView.cs:1: RR-3 routing input 'TeachingRacks': expected exactly one reviewed occurrence of `public bool TeachingRacks => CurrentTopi |
| W-DEP-03 | S-3 | accepted | **REJECTED** | LearningObjectivesView.cs:1: RR-3 routing input 'CurrentTopicIndex': expected exactly one reviewed occurrence of `public int CurrentTopicIndex => Scen |
| W-DEP-04 | S-3 | accepted | **REJECTED** | ScenarioOneUiRoot.cs:1: RR-3 routing input 'NeedsAnnualReview': expected exactly one reviewed occurrence of `public bool NeedsAnnualReview => manager  |
| W-DEP-05 | S-3 (out of scope) | accepted | **ACCEPTED** | documented out-of-scope limit (game/teaching progress) |
| U-RR2-T-manager | S-1 | accepted | **REJECTED** | ScenarioOneUiRoot.cs:696: 'manager' is written outside its reviewed write site (tuple or deconstruction target): tuple, ref/out, qualified, compound,  |

## 3. The author's new claims, fresh runs

| Author claim (handoff, "Repair loop 1") | Observed (own run) | Verdict |
|---|---|---|
| Suite `rejected=275/275 false_greens=0 wrong_cause=0`, benign 32 + 32 = 64, 18 probes unchanged | Same, exit 0, stderr empty (`final/02-suite.stdout`). Group lines: original 20, F1 14, F2-F3 21, R-IDs 21, X-F1 35, X-F2 46, RR-1 28, RR-2 15, RR-3 14, N-4 5, reviewer S-1 10, S-2 14, S-3 4, L-06 2, loop-1 S-1 16, S-2 5, S-3 2, L-06 3 | **True** |
| The 26 new loop-1 mutations are all rejected | 26/26 rejected in the suite, in my own memory driver and through the real pipeline (106-case e2e run below) | **True** |
| "At `b4d47d3d`, all 30 transcribed reviewer mutations and 25 of the 26 new mutations were accepted; `L1-S2-06` was already rejected" | My driver on the exported suite cases: 30/30 accepted, 25/26 accepted, `L1-S2-06` rejected; 9 limits accepted; the 9 over-rejection probes: 5 already rejected (O-01 to O-04, O-07), 4 new (**O-05, O-06, O-08, O-09**) (`evidence/11-b4d-on-suite.txt`) | **True** |
| 54/54 transcribed reviewer cases are identical to the originals | My own independent comparison builds the **whole mutated file map** from my original definition and from the suite's transcription and compares them byte for byte, plus group and description: 54/54 identical (25 survivors + 3 lone CR + 2 L-06 + 21 benign, and `W-DEP-05`, `Y-RR3-17`, `Y-RR3-18` as the 3 limit probes) (`evidence/07-fidelity-l1.txt`) | **True** |
| End-to-end 40-run matrix: control and `W-DEP-05` `10/0/2` exit 0, the 38 mutated `9/1/2` exit 1 | I ran **all 106** loop-1 suite cases through the real `check_baseline.py` (SDK-pinned cwd): 56 mutations (30 transcribed reviewer + 26 new, including the 10 the author ran) → all exit 1 `9/1/2`; 32 benign + 9 limits → all exit 0 `10/0/2`; 9 over-rejections → all exit 1; **stderr empty in all 106**. With my 36 earlier runs, 137 distinct runs overall (`evidence/06-e2e-l1-suite106.txt`, `14-e2e-all.json`) | **True** (and wider) |
| `L1-S2-04`: an NBSP-indented `#endif` in an inactive verbatim-string opener makes hidden code **active** for Roslyn while the old lexer saw a string | Reproduced with my own differential probe (below), and it is wider than one character | **True** |
| All three scripts ASCII-only, apart from three pre-existing en dashes in `check_baseline.py`; invisible test characters written as escapes | `grep -P '[^\x00-\x7F]'` finds only `check_baseline.py` (3 lines); the test file writes every invisible character as an escape | **True** |
| 198 live files lex; only two BOMs; companions 29 | My scan (`evidence/08-live-scan-l1.txt`): 198 OK, 0 rejected; BOM at offset 0 in `Assets/TutorialInfo/Readme.cs` and `Editor/ReadmeEditor.cs` only (neither a companion); no U+0085/2028/2029, no lone CR, no control character, no Cf character; 29 companions, the same set as `b4d47d3d` | **True** |
| In-suite probes fail the run if an outcome changes | Overlay (`ovl`): I replaced the `L-03` limit probe by an edit the checker rejects; the suite then printed `FAIL accepted-limit-L-03: now rejected, documented accepted; update the README known limits`, `changed=1`, and exited **1** (`evidence/13-probe-overlay.*`) | **True** |

### The NBSP-indented `#endif` desync: my own differential probe

Setup (`$L/out/diffprobe2`, `evidence/05-diffprobe2-*.txt`): a type error (`int bad = nosuchname;`) sits after a `#endif` that follows an inactive `@"` opener. If Roslyn treats that line as a directive the code is **active** and compiles with `CS0103`; if not, it stays inside the string and compiles clean. The lexer verdicts are the old (`b4d47d3d`) and new (`753143e6`) `_lex_uncached`.

| Indent before `#endif` and `#if` | Roslyn (C# 9, real Unity DLLs, SDK 8.0.133) | `b4d47d3d` lexer | `753143e6` lexer |
|---|---|---|---|
| none (control) | **active** (CS0103) | rejects ("spans a preprocessor line") | rejects |
| two spaces, tab | active | rejects | rejects |
| NBSP U+00A0 | **active** | **accepts (string hides it)** | rejects |
| form feed U+000C | **active** | **accepts** | rejects |
| vertical tab U+000B | **active** | **accepts** | rejects |
| SUB U+001A | **active** | **accepts** | rejects |
| BOM U+FEFF mid-file | **active** | **accepts** | rejects |
| EM SPACE U+2003 | **active** | **accepts** | rejects |
| `#` followed by NBSP, `# endif` | active | rejects | rejects |
| block comment before the directive, `/* c */ #endif` | **not active** (clean) | accepts | accepts (correct) |
| no directive at all (string control) | clean | accepts | accepts |

So the author's case is real and the defect was wider than one character; the repair closes every row I could build. Roslyn does **not** honour a comment-prefixed directive, and the repaired lexer correctly keeps treating that as string text.

## 4. Gates

All from clean clones at `753143e6`, Python 3.12.3. `final/` and `evidence/` are under `$L`.

| Gate | Result | Raw |
|---|---|---|
| `python3 -m py_compile` on the three scripts | exit 0, no output | `final/01-py_compile.out` |
| Suite `python3 -B test_objective_cta_source_check.py` (cwd = clone) | **exit 0**; `rejected=275/275 false_greens=0 wrong_cause=0`; benign 32/32 and 32/32; `limits=9 over-rejections=9 changed=0`; historical baseline rejected ("unnamed primary", 3 evidence rows); stderr empty | `final/02-suite.*` |
| Candidate `check_baseline.py --rotation-state post-fix --instruction-state post-fix`, cwd under `$E/sdk8` (`global.json` 8.0.133, `rollForward: disable`) | **`PASS=10 FAIL=0 NOT-TESTABLE=2`, exit 0**, stderr empty, six `SnapFar` `PASS` rows (values as in the first review). stdout sha256 `47fc3d40b1352085acf8fd57a36b6a431c1011481c5af80388b9e37ea8ee343d` | `final/05-candidate.*` |
| Same, traced (`COREHOST_TRACE=1`, `strace -f -e execve,openat`, `UseSharedCompilation=false`) | Byte-identical stdout; hostfxr: `Found global.json […/sdk8/global.json]`, `Resolving SDKs with version = '8.0.133', rollForward = 'disable'`, `Using .NET SDK dll=[/usr/lib/dotnet/sdk/8.0.133/dotnet.dll]`. **281 successful opens under `sdk/8.0.133`, 0 under `sdk/10.0.114`**; one `Microsoft.CodeAnalysis.CSharp.dll` open, from 8.0.133 | `final/06-*`, `10-sdk-summary.txt` |
| Control: cwd without `global.json` | `dotnet --version` **10.0.114**; `version = 'latest'`; **504 opens under `sdk/10.0.114`, 0 under `sdk/8.0.133`**; same stdout bytes. The trace discriminates | `final/07-*` |
| Historical `72b4efbd` + the three amended scripts (untracked copies) | **`PASS=6 FAIL=4 NOT-TESTABLE=2`, exit 1**, `tracked-production-vs-baseline: MATCH`, stderr empty. Differs from `baseline-output.txt` at **line 48 only** (check 08's text). **Byte-identical** to my first-review historical output at `b4d47d3d` (`cmp`) | `final/08-*` |
| Roslyn C# 9 compile, real Unity 6000.6 DLLs (320 refs), Player-like defines, SDK 8.0.133 | Baseline 189 files, 0 errors. The author's **106 loop-1 case folders: 106 compile with 0 errors** (I exported them from the suite's own tables with the suite's own `build` semantics). My 176 first-review cases: identical results to the first review (164 compile; the same 12 invalid). #305 corpus 117: identical (2 invalid, as #305 recorded). Original 59: identical | `evidence/04-roslyn-l1-*.txt` |
| In-flight source (in memory only) | `753143e6`, `69b41605`, `9789369d` and `origin/feature/scenario-one-next-action-guidance-v1` (= `9789369d`) are all **accepted** by the repaired scanner (companions 29 → 30), so the new rules do not block the next integration's text | `evidence/07-tip-probe-l1.txt` |

## 5. Over-rejection audit

| Behaviour (README name) | Disclosed in the README? | Direction | Live `Assets` source | My verdict |
|---|---|---|---|---|
| **O-05** any `#if` inside `Start` | yes (known limits) | fail-closed only | live `Start` has none; live accepted | accepted false-red trade-off |
| **O-06** U+2028 even inside a verbatim string | yes | fail-closed only | none of the 198 files contains U+0085/2028/2029 | accepted trade-off |
| **O-07** a local or parameter named like a related-state field in the root | yes (pre-existing through the count rule) | fail-closed only | none | accepted |
| **O-08** `null != ui.Learning` | yes | fail-closed only | all live `.Learning` uses pass | accepted |
| **O-09** `screen` passed whole to any method (`Debug.Log(screen)`) and `screen` as a member receiver (`screen.ToString()`) | yes (receiver and whole-argument both named) | fail-closed only | live passes | accepted |
| Form feed, vertical tab, other control or format character in code | yes ("documented, not probed") | fail-closed only | none | accepted |
| Any `return`/`throw`/`goto`/`yield` in `Start` before the last reviewed write, even inside a lambda | yes | fail-closed only | none today | accepted |
| `ui.Learning is null`, `is not null`, `??`, local alias read in a companion (**N-4**) | by rule only | fail-closed only | none | accepted, Info |
| A line of a multi-line comment or verbatim string starting with whitespace and `#` | yes | fail-closed only | none in live files | accepted |
| Non-ASCII letters, combining marks and `Zs` in code; BOM, CRLF; Cf/NBSP/`€` in strings and comments; `#` in strings and chars; bidi override in comments | n/a | accepted (benign) | — | no over-rejection |

Evidence: the suite's own INFO lines (all nine over-rejections "rejected as documented"), my 21 first-review benign cases (21/21 accepted), my round-8 hunt (13 of 17 benign candidates accepted; the 4 rejected are N-4) and the 32 + 24 suite benign cases. The only defects in this area are **in the permissive direction** (N-1, N-2), not here.

## 6. Limits honesty

| Statement | Verdict | Basis |
|---|---|---|
| W-DEP-05 out-of-scope documentation | **Honest.** README names `ReloadProgress` marking every lesson done, and the case is an in-suite accepted-limit probe | suite INFO line; e2e `10/0/2` |
| L-01 residual route (a view member returning the view or an element) | **Honest, and currently theoretical**: the only element-returning public member of the live view is `Root`, which is banned for companions | grep of the live view |
| In-suite limit probes, reported not counted, run fails on change | **True**, proved with the overlay | `evidence/13-probe-overlay.*` |
| Corrected README scope sentence ("as far as that text and its pinned UI inputs decide (the game state they read is not checked)") | **Better, still overstated by N-2** (a decoy defeats the "pinned UI inputs") | N-2 |
| README: "Any other write fails, whatever the count: an assignment …, a parenthesised, tuple or nested-tuple target …" | **Overstated.** True for statements; false inside an interpolation hole | **N-1** |
| README: "Each field declaration is pinned" / handoff: "A field cannot become a property that drops writes" | **Overstated** | **N-2** |
| README: unpinned screen entry points are `ShowObjectives` and `CloseAll` | True but under-described | N-3 |
| README: lexer separates comments, preprocessor lines and literals; Cf, line separators, directive whitespace fail closed | **True**, and stronger than the author's single probe (differential table above) | §3 |

## 7. Integrity

`evidence/16-final-integrity.txt`, taken at the end:

- The five files in my `src` clone are byte-identical to the blobs in the bare store at `753143e6`: `check_baseline.py` sha256 `787497d8…bb587`, `objective_cta_source_check.py` `9654a974…c6b66`, `test_objective_cta_source_check.py` `925b9e8c…2f6c`, `README.md` `04fea8e7…3d` (blob `5861f56d`), `baseline-output.txt` `513cddd9…c1` (blob `db6a8b85`). The handoff blob sha256 is `994be7b1…0222`. The gates script's before and after hash lists (`final/00-` and `final/99-sha256-*`) are identical.
- Remote tip and the author lane's `HEAD`: `753143e6` at start and at end. I never wrote to the author lane or the bare store.
- I did not edit any checker, test or fixture in a clone either, except the **`ovl` overlay clone's test file**, on purpose, to prove the tripwire. It is uncommitted and separate from the clones used for results.

## New attacks written after reading the loop-1 diff (rounds 7 to 11)

These are **my own** cases, written after reading the new code (white-box), so they test the repair rather than reproduce my first list. Method: every case is an in-memory edit of the live files run against the repaired scanner by `run_my_l1.py` (the `b4d47d3d` scanner alongside as the "before" column), the whole set compiled with Roslyn C# 9 against the real Unity DLLs, and the accepted ones run through the real `check_baseline.py`.

| Round | Content | Cases | Valid C# 9 | Rejected | Accepted | of which false greens |
|---|---|---|---|---|---|---|
| 7 | **Write-shape matrix** on the new S-1 rule: 26 shapes (plain, `this.`, parenthesised, tuple, second-position tuple, `this.` tuple, nested tuple, lambda, `ref`/`out` through helpers, `ref` local, alias, cast, `@`, comment/newline/NBSP between tokens, chained, `for`, `if`, local function, **interpolation hole**, `Unsafe.AsRef`, `__makeref`, `??=`) × 8 related-state fields in a new method; 6 shapes × 8 fields at the tail of `Start` and the head of `OnDestroy`; 7 `screen` compound and `++` forms; 21 class-scope shapes (setter, `init`, expression body, nested class, nested `ref`, static, `ref` return); 16 guarded-name shapes; 3 probes | 350 | 338 (invalid: 3 `init` accessors, 8 `Unsafe.AsRef`, 1 type-hijack case) | **347** (336 valid) | 3 (`A7-x-update-setscreen` and `A7-x-enabled-false`, valid, in a disclosed class; `A7-x-teachinghijack`, not valid C#) | 0 |
| 8 | Decoy/shadow pins (8, incl. one no-decoy control), Start manipulations (3), L-06 evasions and reads (12), benign over-rejection hunt (16), lexer extras (4) | 43 | 41 | 20 | 23 (13 benign rightly accepted, 3 disclosed limits, 1 exploratory, **6 false greens**) | **6** |
| 9 | Decoy placements (nested class, nested struct, separate top-level class), one control | 6 | 6 | 1 | 5 (1 benign control, **4 false greens**) | **4** |
| 10 | Writes inside interpolation holes: 7 fields × 8 shapes | 56 | 55 | 28 | 28 | **28** |
| 11 | More hole shapes (`out|ref this.x`, `++|--`), five **called-site** variants, controls | 23 | 23 | 4 | 19 | **19** |
| **Total** | | **478** | | | | **57** (10 decoy + 47 hole), all valid C# 9 |

Counts are from `out/mine7..11/results.json`; "accepted" includes cases that were rightly accepted (benign controls, disclosed limits). Per-case outcomes and rejection reasons are in `$L/out/mine*/results.json` and `$L/evidence/03-mine-round*.stdout`.

**What the matrix says about the repaired rule.** Apart from interpolation holes, the new S-1 site rule withstood everything: with the old count rule neutralised in a scratch patch, the remaining rules (chiefly the new one) still reject 340 of the 347 round-7 attacks, each for its intended cause ("written outside its reviewed write site (assignment | tuple or deconstruction target | ref operand | out operand | in operand | ++ operand)", "reached through a receiver other than `this`", "`__makeref` … unsupported"). The `screen` receiver/argument rule, the top-level-statement rule for the reviewed `Start` writes, the early-`return` rule and the "reviewed text lies inside `#if`" rule rejected every variant I built. A random mutation fuzz (500 seeded truncations, deleted lines/characters/brackets, odd-character insertions in the root, the view and a partial) gave **0 crashes** (60 rejected of 91 truncations, 44 of 90 insertions; no traceback), so "never a parse crash" holds on that sample (`evidence/15-fuzz-crash.txt`).

**Round 8 (decoys and benign hunt), per case.**

| ID | Intent | Expect | b4d47d3d | 753143e6 | Result | Cause at head (abridged) |
|---|---|---|---|---|---|---|
| A8-D1-help-prop-decoy | help becomes a property with a no-op setter (always null); the reviewed field text survives once in a nes | must reject | accepted | accepted | **FALSE GREEN** |  |
| A8-D2-screen-prop-decoy | screen becomes a property that always reads Map (no-op setter); the reviewed initialiser survives once in | must reject | accepted | accepted | **FALSE GREEN** |  |
| A8-D3-manager-prop-decoy | manager becomes a property with a no-op setter; reviewed declaration survives once in a nested decoy | must reject | accepted | accepted | **FALSE GREEN** |  |
| A8-D4-nar-decoy | NeedsAnnualReview forced true; the reviewed expression survives once in a nested decoy class with its own | must reject | accepted | accepted | **FALSE GREEN** |  |
| A8-D5-teaching-decoy | TeachingRacks forced true; both reviewed members survive once in a nested decoy class of the view | must reject | accepted | accepted | **FALSE GREEN** |  |
| A8-D5b-teaching-decoy-simple | TeachingRacks forced true; only the TeachingRacks text survives in a nested decoy (CurrentTopicIndex stay | must reject | accepted | accepted | **FALSE GREEN** |  |
| A8-D5c-teaching-decoy-ctl | control for D5b: TeachingRacks forced true and NO decoy | must reject | accepted | rejected | ok | LearningObjectivesView.cs:1: RR-3 routing input 'TeachingRacks': expected exactly one reviewed occurrence |
| A8-D6-ctor-count | decoy nested class declares a second LearningObjectivesView-like text (sanity: nested class with a field  | exploratory | rejected | rejected | info | LearningObjectivesView.cs:25: the Objectives Root may be used only by its declaration and the constructor |
| A8-S-reorder | Start: map and workPlan constructions swapped (order of reviewed writes is not pinned; disclosed) | disclosed limit | accepted | accepted | limit as disclosed |  |
| A8-S-deadwrite | Start: infinite loop before the help write (non-termination, disclosed L-03/L-08 class) | disclosed limit | accepted | accepted | limit as disclosed |  |
| A8-S-neverrun | Start: help write after an unbraced if-guard (conditional) | must reject | accepted | rejected | ok | ScenarioOneUiRoot.cs:153: RR-2 related state: `help = new MenuHelpView(this);` must be a top-level statem |
| A8-L06-isnull | companion: ui.Learning is null (C# 9 idiom) - benign read | benign | accepted | rejected | **over-rejected** | StandMapView.cs:27: another class reaches the Objectives view other than by a member access or null test  |
| A8-L06-isnotnull | companion: ui.Learning is not null (C# 9 idiom) - benign read | benign | accepted | rejected | **over-rejected** | StandMapView.cs:27: another class reaches the Objectives view other than by a member access or null test  |
| A8-L06-localalias-read | companion: local alias of ui.Learning used only for a read | benign | accepted | rejected | **over-rejected** | StandMapView.cs:27: another class reaches the Objectives view other than by a member access or null test  |
| A8-L06-coalesce | companion: ui.Learning ?? x null-coalescing read | benign | accepted | rejected | **over-rejected** | StandMapView.cs:27: another class reaches the Objectives view other than by a member access or null test  |
| A8-L06-elementtraversal | companion reaches Objectives elements only through a public member of the view (documented residual) | disclosed limit | accepted | accepted | limit as disclosed |  |
| A8-L06-separator | companion: ui . Learning with comments/newline before Root | must reject | rejected | rejected | ok | StandMapView.cs:28: another class reaches the Objectives Root (RR-1) |
| A8-L06-verbatim | companion: ui.@Learning.@Root | must reject | rejected | rejected | ok | StandMapView.cs:27: another class reaches the Objectives Root (RR-1) |
| A8-L06-nullcond | companion: ui?.Learning?.Root | must reject | rejected | rejected | ok | StandMapView.cs:27: another class reaches the Objectives Root (RR-1) |
| A8-L06-tuple | companion: tuple deconstruction alias of ui.Learning | must reject | accepted | rejected | ok | StandMapView.cs:27: another class reaches the Objectives view other than by a member access or null test  |
| A8-L06-arg | companion: ui.Learning passed to an Action<LearningObjectivesView> lambda | must reject | accepted | rejected | ok | StandMapView.cs:27: another class reaches the Objectives view other than by a member access or null test  |
| A8-L06-generic | companion: generic helper receives ui.Learning (type inferred) | must reject | accepted | rejected | ok | StandMapView.cs:28: another class reaches the Objectives view other than by a member access or null test  |
| A8-L06-switch | companion: switch expression over ui.Learning | must reject | accepted | rejected | ok | StandMapView.cs:27: another class reaches the Objectives view other than by a member access or null test  |
| A8-B-interp-screen | root: Debug.Log($"screen={screen}") in an unpinned method (interpolation hole, not a whole argument) | benign | accepted | accepted | ok |  |
| A8-B-screen-tostring | root: screen.ToString() in an unpinned method (member receiver) | exploratory | accepted | rejected | info | ScenarioOneUiRoot.cs:696: 'screen' is used as a member receiver or passed whole as an argument; a ref/in  |
| A8-B-screen-eq | root: screen comparisons and ternary only | benign | accepted | accepted | ok |  |
| A8-B-screen-switchexpr | root: screen switch expression | benign | accepted | accepted | ok |  |
| A8-B-help-reads | root: reads of help/manager/map with ?. and is-pattern in an unpinned method | benign | accepted | accepted | ok |  |
| A8-B-start-swap-region | root: #region wrapped around part of Start's constructions (no #if) | benign | accepted | accepted | ok |  |
| A8-B-start-try | root: try/catch around an unrelated Start statement before the reviewed writes | benign | accepted | accepted | ok |  |
| A8-B-nonascii-ident | partial: non-ASCII letters in a new identifier and in comments/strings (precomposed e-acute, sharp s) | benign | accepted | accepted | ok |  |
| A8-B-ctrlz-comment | partial: form feed and SUB inside a // comment (page-break artefact) | benign | accepted | accepted | ok |  |
| A8-B-ff-line | partial: a form-feed-only line between members (old GNU style page break) - control char in code | exploratory | accepted | rejected | info | LearningObjectivesView.Sequence.cs:9: character U+000C (category Cc) in code: the C# compiler reads it di |
| A8-B-lone-bom-comment | partial: U+FEFF inside a comment and a string | benign | accepted | accepted | ok |  |
| A8-B-crlf-all | CRLF endings in the new root and a new partial (CR followed by LF is fine) | benign | accepted | accepted | ok |  |
| A8-B-hash-in-string | partial: '#' inside an ordinary string and a char literal | benign | accepted | accepted | ok |  |
| A8-B-multiline-verbatim-indented-hash | partial: verbatim string whose second line starts with whitespace and '#' (e.g. markdown heading) - docum | exploratory | rejected | rejected | info | LearningObjectivesView.Sequence.cs:9: a multi-line literal or comment spans a preprocessor line; fail clo |
| A8-B-multiline-comment-hash | partial: block comment with an indented '#' line (e.g. commented-out #if) | exploratory | rejected | rejected | info | LearningObjectivesView.Sequence.cs:9: a multi-line literal or comment spans a preprocessor line; fail clo |
| A8-B-xml-doc-hash | partial: XML doc comment (///) lines containing an indented '#' | exploratory | accepted | accepted | info |  |
| A8-L-nul-code | NUL char (U+0000) inside code of the view partial hiding nothing but desyncing Python | must reject | accepted | rejected | ok | LearningObjectivesView.Sequence.cs:9: character U+0000 (category Cc) in code: the C# compiler reads it di |
| A8-L-hangul-filler | partial: Hangul filler (Lo) in an identifier - a different identifier for Roslyn and for the checker | benign | accepted | accepted | ok |  |
| A8-L-astral-cf | astral Cf (U+E0001 language tag) in code of the partial | must reject | accepted | rejected | ok | LearningObjectivesView.Sequence.cs:9: character U+E0001 (category Cf) in code: the C# compiler reads it d |
| A8-L-bidi-comment | Bidi override U+202E inside a comment and string (display-only; not a parse change) | benign | accepted | accepted | ok |  |

### End-to-end through the real `check_baseline.py` (SDK 8.0.133 cwd): my earlier cases

| Case | Set | check_baseline exit | Tally | Test-08 cause (abridged) |
|---|---|---|---|---|
| CONTROL-live | control | 0 | `10 / 0 / 2` | PASS (accepted) |
| Y-RR2-08 | S-1 | 1 | `9 / 1 / 2` | ScenarioOneUiRoot.cs:681: pinned method TryShowReviewYear (RR-2 related state 'screen') changed (expected 'Review', found 'None'); |
| Z-RR2-T-help | S-1 | 1 | `9 / 1 / 2` | ScenarioOneUiRoot.cs:696: 'help' is written outside its reviewed write site (tuple or deconstruction target): tuple, ref/out, qual |
| Z-RR2-T-screen | S-1 | 1 | `9 / 1 / 2` | ScenarioOneUiRoot.cs:696: 'screen' is written outside its reviewed write site (tuple or deconstruction target): tuple, ref/out, qu |
| Z-RR2-T-rackLayout | S-1 | 1 | `9 / 1 / 2` | ScenarioOneUiRoot.cs:696: 'rackLayout' is written outside its reviewed write site (tuple or deconstruction target): tuple, ref/out |
| Z-RR2-T-map | S-1 | 1 | `9 / 1 / 2` | ScenarioOneUiRoot.cs:696: 'map' is written outside its reviewed write site (tuple or deconstruction target): tuple, ref/out, quali |
| Z-RR2-T-workPlan | S-1 | 1 | `9 / 1 / 2` | ScenarioOneUiRoot.cs:696: 'workPlan' is written outside its reviewed write site (tuple or deconstruction target): tuple, ref/out,  |
| Z-RR2-T-review | S-1 | 1 | `9 / 1 / 2` | ScenarioOneUiRoot.cs:696: 'review' is written outside its reviewed write site (tuple or deconstruction target): tuple, ref/out, qu |
| U-RR2-T-manager | S-1 | 1 | `9 / 1 / 2` | ScenarioOneUiRoot.cs:696: 'manager' is written outside its reviewed write site (tuple or deconstruction target): tuple, ref/out, q |
| Z-RR2-C-screen | S-1 | 1 | `9 / 1 / 2` | ScenarioOneUiRoot.cs:681: pinned method TryShowReviewYear (RR-2 related state 'screen') changed (expected 'screen', found 'CloseHe |
| Z-RR2-C-help | S-1 | 1 | `9 / 1 / 2` | ScenarioOneUiRoot.cs:1: RR-2 related state 'help': expected exactly one reviewed occurrence of `help = new MenuHelpView(this);` (m |
| Y-N-10 | S-2 | 1 | `9 / 1 / 2` | LearningObjectivesView.Sequence.cs:9: U+2028 is a line break for the C# compiler but not for this lexer (it can end a // comment o |
| Y-N-11 | S-2 | 1 | `9 / 1 / 2` | LearningObjectivesView.cs:82: character U+200D (category Cf) in code: the C# compiler reads it differently from this lexer (a form |
| Y-N-12 | S-2 | 1 | `9 / 1 / 2` | LearningObjectivesView.Cf.cs:1: character U+200D (category Cf) in code: the C# compiler reads it differently from this lexer (a fo |
| Y-N-13 | S-2 | 1 | `9 / 1 / 2` | LearningObjectivesView.Sequence.cs:9: U+0085 is a line break for the C# compiler but not for this lexer (it can end a // comment o |
| Y-N-14 | S-2 | 1 | `9 / 1 / 2` | ScenarioOneUiRoot.cs:696: character U+200D (category Cf) in code: the C# compiler reads it differently from this lexer (a format c |
| Y-N-15 | S-2 | 1 | `9 / 1 / 2` | ScenarioOneUiRoot.cs:696: character U+200D (category Cf) in code: the C# compiler reads it differently from this lexer (a format c |
| Z-N-CF-buttons | S-2 | 1 | `9 / 1 / 2` | LearningObjectivesView.cs:83: character U+200D (category Cf) in code: the C# compiler reads it differently from this lexer (a form |
| Z-N-CF-secondprimary | S-2 | 1 | `9 / 1 / 2` | LearningObjectivesView.cs:192: character U+200D (category Cf) in code: the C# compiler reads it differently from this lexer (a for |
| Z-N-CF-allowlist-clear | S-2 | 1 | `9 / 1 / 2` | ScenarioOneUiRoot.cs:696: character U+200B (category Cf) in code: the C# compiler reads it differently from this lexer (a format c |
| Z-N-SHY | S-2 | 1 | `9 / 1 / 2` | LearningObjectivesView.cs:82: character U+00AD (category Cf) in code: the C# compiler reads it differently from this lexer (a form |
| Z-N-PS | S-2 | 1 | `9 / 1 / 2` | LearningObjectivesView.Sequence.cs:9: U+2029 is a line break for the C# compiler but not for this lexer (it can end a // comment o |
| W-DEP-01 | S-3 | 1 | `9 / 1 / 2` | ScenarioOneUiRoot.cs:1: RR-3 routing input 'NeedsAnnualReview': expected exactly one reviewed occurrence of `public bool NeedsAnnu |
| W-DEP-02 | S-3 | 1 | `9 / 1 / 2` | LearningObjectivesView.cs:1: RR-3 routing input 'TeachingRacks': expected exactly one reviewed occurrence of `public bool Teaching |
| W-DEP-03 | S-3 | 1 | `9 / 1 / 2` | LearningObjectivesView.cs:1: RR-3 routing input 'CurrentTopicIndex': expected exactly one reviewed occurrence of `public int Curre |
| W-DEP-04 | S-3 | 1 | `9 / 1 / 2` | ScenarioOneUiRoot.cs:1: RR-3 routing input 'NeedsAnnualReview': expected exactly one reviewed occurrence of `public bool NeedsAnnu |
| W-DEP-05 | S-3 | 0 | `10 / 0 / 2` | PASS (accepted) |
| Y-N-09 | lone CR (3) | 1 | `9 / 1 / 2` | LearningObjectivesView.Sequence.cs:10: the Objectives Root may be used only by its declaration and the constructor (RR-1). Predica |
| Z-N-CR-region | lone CR (3) | 1 | `9 / 1 / 2` | LearningObjectivesView.Sequence.cs:10: the Objectives Root may be used only by its declaration and the constructor (RR-1). Predica |
| Z-N-CR-root | lone CR (3) | 1 | `9 / 1 / 2` | 'guidanceCandidate' must be written only by RefreshNextActionGuidance/ApplyNextActionGuidance/ClearNextAction. This is not a Unity |
| R03 | rejected control | 1 | `9 / 1 / 2` | static primary button 'objective-back-to-forest' is not in the GuidanceButtonNames scoped demotion set. This is not a Unity runtim |
| X-F1-24 | rejected control | 1 | `9 / 1 / 2` | LearningObjectivesView.cs:81: header local 'forest' is used outside the reviewed straight-line header shapes (RR-1/RR-3). Predicat |
| X-F2-16 | rejected control | 1 | `9 / 1 / 2` | ScenarioOneUiRoot.cs:451: pinned method RefreshNextActionGuidance changed (expected 'guidanceScope', found 'learning'); a reviewed |
| Y-RR4-05 | rejected control | 1 | `9 / 1 / 2` | GuidanceButtonNames contains a duplicate name. This is not a Unity runtime verdict. |
| Y-RR4-09 | rejected control | 1 | `9 / 1 / 2` | expected exactly one method matching '\\bpublic\\s+LearningObjectivesView\\s*\\(\\s*ScenarioOneUiRoot\\s+ui\\s*\\)'; found 0. Pred |
| Z-N-CF-secondprimary-ctl | rejected control | 1 | `9 / 1 / 2` | LearningObjectivesView.cs:192: UiKit.Button class argument is not a plain string literal. Predicate: UiKit.Button class argument i |

All 36: stderr empty. Two exit 0 (`CONTROL-live`, `W-DEP-05`), 34 exit 1 with `9/1/2`. The three lone-CR cases are rejected here through the universal-newline read, as in the first review; the in-memory API now rejects them by rule.

### End-to-end: the decoy and hole cases (new findings N-1 and N-2)

| Case | check_baseline exit | Tally | Test-08 result |
|---|---|---|---|
| A8-D1-help-prop-decoy | 0 | `10 / 0 / 2` | PASS (accepted) |
| A8-D2-screen-prop-decoy | 0 | `10 / 0 / 2` | PASS (accepted) |
| A8-D3-manager-prop-decoy | 0 | `10 / 0 / 2` | PASS (accepted) |
| A8-D4-nar-decoy | 0 | `10 / 0 / 2` | PASS (accepted) |
| A8-D5-teaching-decoy | 0 | `10 / 0 / 2` | PASS (accepted) |
| A8-D5b-teaching-decoy-simple | 0 | `10 / 0 / 2` | PASS (accepted) |
| A8-D5c-teaching-decoy-ctl | 1 | `9 / 1 / 2` | LearningObjectivesView.cs:1: RR-3 routing input 'TeachingRacks': expected exactly one reviewed occurrence of ` |
| A9-D7-toplevel-decoy-root | 0 | `10 / 0 / 2` | PASS (accepted) |
| A9-D8-struct-decoy-view | 0 | `10 / 0 / 2` | PASS (accepted) |
| A9-D9-help-decoy-struct | 0 | `10 / 0 / 2` | PASS (accepted) |
| A9-D10-help-realistic | 0 | `10 / 0 / 2` | PASS (accepted) |
| A10-hole-tuple-help | 0 | `10 / 0 / 2` | PASS (accepted) |
| A10-hole-paren-screen | 0 | `10 / 0 / 2` | PASS (accepted) |
| A10-hole-verbatim-manager | 0 | `10 / 0 / 2` | PASS (accepted) |
| A11-called-hole-help | 0 | `10 / 0 / 2` | PASS (accepted) |
| A11-called-hole-screen | 0 | `10 / 0 / 2` | PASS (accepted) |
| A11-called-hole-manager | 0 | `10 / 0 / 2` | PASS (accepted) |
| A11-called-hole-paren-help | 0 | `10 / 0 / 2` | PASS (accepted) |
| A11-called-lambda-hole-help | 0 | `10 / 0 / 2` | PASS (accepted) |
| A11-hole-outthis-help | 0 | `10 / 0 / 2` | PASS (accepted) |
| A11-hole-prefixinc-screen | 0 | `10 / 0 / 2` | PASS (accepted) |
| A11-hole-refthis-rackLayout | 0 | `10 / 0 / 2` | PASS (accepted) |
| A11-called-stmt-ctl-help | 1 | `9 / 1 / 2` | ScenarioOneUiRoot.cs:696: 'help' is written outside its reviewed write site (tuple or deconstruction target):  |

### Future-fix note for Manager #200 (not a repair)

Both new classes look cheap and local. I tested the idea with a **reviewer scratch wrapper** (`$L/scripts/fix_feasibility_l1c.py`, run after the unchanged scanner, **never shipped, not approved code**, fitted after seeing the cases):

1. **Decoy guard.** In `LearningObjectivesView.cs` and `ScenarioOneUiRoot.cs`, the masked code may declare exactly one `class`/`struct`/`interface`/`record` (the class itself; the root's `enum UiScreen` is untouched). The live view and root each declare one. Equivalent: require each pinned snippet to lie at the class-body brace depth.
2. **Hole guard.** Apply the checker's own `_state_write` to the tokens of every interpolation hole (the lexer already keeps `(start, close, inner)` per hole) and reject a write or a receiver other than `this` there.

Scratch results (`evidence/12c-fix-feasibility-decoy-hole.txt`, `12-`, `12b-`): live source accepted; **all 57 accepted false greens of rounds 8 to 11 rejected** (10 decoy + 47 hole), the only expected-rejected case still accepted being `W-DEP-05`; **0 of my 35 accepted benign cases and 0 of the 56 suite benign cases newly rejected**, CRLF and BOM variants still accepted; the 9 suite accepted-limit probes keep their outcome; no verdict among the 575 cases of rounds 1 to 9 changed other than the ten decoys. Caveat: I fitted it after seeing the cases, so it shows they can be closed locally, not that nothing else exists. If the Manager prefers disclosure to code, the README sentences listed in the [limits table](#6-limits-honesty) would have to be reworded and the decoy and hole shapes named as accepted limits with in-suite probes (the suite already has the machinery).

### Is anything further warranted tonight?

**No repair.** The one authoring loop is spent. My recommendation to Manager #200 is a disposition choice, not a schedule:

- **(a)** Accept the checker as a bounded guard against *accidental* regression of the Objectives CTA, record N-1 and N-2 as accepted limits (reword the two README claims and add two probes), and carry the two guards above as a future-fix note; or
- **(b)** hold the checker as NEEDS CHANGES until the two guards are written and re-reviewed.

Either way the first review's three findings are closed, which is the substantial part of the loop's value.

## What remains unproved

**Not run, and not claimed** (unchanged from the first review): Unity Editor compile or tests; rendering and screenshots; native pointer or keyboard input; player focus and consent; Help-close timing; save/load, Legacy40 and Reference scenarios; Windows or Player build; USS and other style sources; the exact integrated candidate (only an in-memory scan of the feature tips); any merge. Runtime effects described for mutants are code reasoning plus a Roslyn compile; nothing executed them.

**Unproved by this re-review.** That N-1 and N-2 are the only remaining classes (I wrote the cases after reading the diff, am the same model family as the author, and found these only in rounds 8 and 10 after round 7 came out clean). That the lexer matches Roslyn on every input (I tested directive whitespace, line separators, format characters and the 198 live files, not every Unicode category; Python's Unicode database and the compiler's can differ for newly assigned code points, though the checker fails closed on any category outside its short allow list). That the scratch guards are complete. That the stored pinned texts are the right behaviour (they are the reviewed `1585ca20` source). Anything in the game state the routing reads (`ScenarioOneManager`, teaching state): disclosed out of scope.

## Commands and raw tallies

`E=/media/jer/Files/CCF/Verification/CTA-314-IndependentReview-20261107`, `L=$E/loop1-rerun`. Python ran with `PYTHONDONTWRITEBYTECODE=1`, `PYTHONPYCACHEPREFIX=$E/pycache`, `TMPDIR=$E/tmp`.

| # | What | Result | Raw |
|---|---|---|---|
| 1 | Diff scope vs `b4d47d3d`, `1585ca20`; blobs; earlier blocks | five paths; blocks verbatim | `evidence/01-diff-scope.txt`, `09-preserved-blocks.txt` |
| 2 | Gates (`scripts/gates_l1.sh`): py_compile, suite, pinned/traced/control candidate, historical, SDK summary | as in §4 | `final/` |
| 3 | My six first-review case files unchanged (`cmp`), `run_my_l1.py` on the new head and on `b4d47d3d` | 176 cases: 28/29 former false greens rejected, 112/112 stay rejected, 21/21 benign accepted | `evidence/03-mine-round1..6.stdout`, `10-mine-tally-l1.txt` |
| 4 | #305 corpus and original/#304 set | 102/102, 15/15; 55/55, 4/4 | `evidence/02-`, `05-` |
| 5 | End-to-end: my 36 + the author's 106 suite cases (+ new) | 137 distinct runs, 0 stderr | `evidence/06-*`, `14-e2e-all.json`, `e2e-out/` |
| 6 | Fidelity of the 54 transcribed cases (whole file maps) | 54/54 identical | `evidence/07-fidelity-l1.txt` |
| 7 | `b4d47d3d` vs head on the 106 suite cases | author's claims reproduce | `evidence/11-b4d-on-suite.txt` |
| 8 | Roslyn C# 9 of all case folders | see §4 | `evidence/04-roslyn-l1-*.txt` |
| 9 | Differential directive probes | table in §3 | `evidence/05-diffprobe2-*.txt` |
| 10 | Live scan, in-flight tips | 198 OK; tips accepted | `evidence/08-`, `07-tip-probe-l1.txt` |
| 11 | New rounds 7 to 11 | 478 cases, 57 false greens | `evidence/03-mine-round7..11.stdout` |
| 12 | Site rule alone (count rule neutralised) | 340/347; 7 missed = holes | `evidence/17-site-rule-alone.txt` |
| 13 | Probe tripwire overlay | exit 1 on a changed probe | `evidence/13-probe-overlay.*` |
| 14 | Crash fuzz, 500 seeded mutants | 0 crashes | `evidence/15-fuzz-crash.txt` |
| 15 | Scratch guards | 57/57 closed, 0 benign lost | `evidence/12*-fix-feasibility*.txt` |
| 16 | Final integrity re-hash | identical | `evidence/16-final-integrity.txt` |

## Final state (re-review)

- Checker, test, README, `check_baseline.py` and `baseline-output.txt` at `753143e6` re-hashed at the end: **identical** to the pinned blobs (above). Remote tip and the author lane's `HEAD` still `753143e6`. I edited nothing under review.
- Evidence stays at `$L` (about 71 MiB; the first review's 5 GiB `source-clone` is beside it); `SHA256SUMS.txt` there covers the re-review scripts and evidence. Clones may be deleted after the Manager rules; the evidence should stay.
- This section is appended to the same file as the first review and is the only file published from the docs lane. The hash of the commit that adds it is in the #314 `[HANDED-OFF — exact-head re-review]` comment, because a file cannot contain its own commit hash.
