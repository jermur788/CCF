# #326 — independent review of the #325 CTA companion/name rebinding repair

**Reviewer opinion, SOURCE-CHECKER SCOPE ONLY: NEEDS CHANGES — and STOP for a Manager #200 decision (Roslyn semantics vs. explicit bounded risk).** This is not Manager approval, integration, or any native/Unity/visual/A–O claim.

Reviewer: **T3 Code, Claude harness, claude-opus-5-5, Anthropic provider family; reasoning effort not observable by the model (dispatch requested "high"); account/quota UNKNOWN.** Cross-provider from the #325 author (OpenAI gpt-6.1-sol). I did not author any #325 code and edited no checker, test, README or production file.

## Reviewed target (exact)

| Item | SHA |
|---|---|
| Source branch | `verify/scenario-one-feedback-cta-anchor-20261106` (also PR #300's head ref; PR open, unmerged) |
| Diff base (#321-reviewed publication) | `3033dd12fed73136c8f94c82d58bf0d7cacd5380` |
| Implementation commits | `1f2bb5e44aab96e78ac12b51784c6f1b7f585507`, `b040a9929b693f9ad28bc8f9f935f002b91a38b8` |
| **Tested source** | **`b040a9929b693f9ad28bc8f9f935f002b91a38b8`** |
| **Publication head (== origin, fresh `git ls-remote`)** | **`c83fae1af4ac1449cba0304e44b0df49b91f8919`** (adds only the author result doc over `b040a992`) |
| This review's docs branch base | `origin/main` `93f7abc0f290cab61509222c237fab513c6337ea` at lane creation |

All tests ran on reviewer-owned clones of the bare store checked out at `c83fae1a` (Assets identical to the #321 tested `8ff6352b`; `git diff --quiet 8ff6352b c83fae1a -- Assets` = 0). The #325 author lane and #321/#325 evidence were only read.

## Verdict rationale (summary)

1. **The three #321 witnesses are genuinely closed.** R321-S05/S06/S07 reconstructed byte-for-byte (5/5 files), compile with 0 errors under C# 9 against the 320 hash-verified references, Roslyn proves the rebinding, and the real full checker rejects each with exit 1, 9/1/2, only check 08, for the intended name-binding cause.
2. **The new guard is sound for what it claims** (alias of the base header name in the root; type declarations named like a reviewed root identifier in a collected companion): every distinct alias/companion-type variant I built that is valid C# is rejected for the right cause; benign unrelated aliases/types stay accepted; all historical gates hold exactly.
3. **But a valid, compile-clean false green on the same `Time.frameCount` close-frame binding remains in collected source**: a **tuple-typed root member named `Time`** (A326-N06/N07), and the same shape on `DisplayStyle` rebinding all 22 `DisplayStyle.Flex/None` reads, including the pinned `learning.Root.style.display` snippet (A326-N08). Roslyn proves the rebinding; the real checker reports **10/0/2, exit 0**. The cause is the pre-existing #320 lexical member-name scan (`_member_names`), which records "the identifier before the first `(`" — for `private (int frameCount, float unscaledTime) Time …` that identifier is `private`. This gap **pre-dates #325** (same outcome on the `3033dd12` checker), is **not listed** among the README known limits or probes, and contradicts the README's N-2 claim that a member named like an outside type (`private FakeTime Time`) is rejected.
4. Per the governing ruling ("if independent review still uncovers different semantic loopholes, STOP and present a separate Roslyn semantic-analysis versus explicit bounded-risk checker decision"), I do **not** recommend another lexical repair loop. I am not waiving the finding either: the checker is NEEDS CHANGES at source scope until Manager #200 chooses (a) Roslyn semantic binding assertions or (b) an explicit, evidence-labelled bounded-risk acceptance of this and the uncollected-source limit.

## 1. Witness reconstruction and fidelity

Reconstruction (`scripts/cases.py`, `scripts/materialize.py`) copies the R321-S05/S06/S07 strings verbatim from read-only `CTA-309-TwoGuardReview-20261107/new_variants.py` and applies them with the same read semantics (`Path.read_text()`, `write_text(newline='')`) to the live files of `c83fae1a`. Every produced file was compared byte-for-byte with the #321 export `new-mutants/` and the #325 author's `CTA325-Proof-20261107/mutants/`.

| Case / file | SHA256 (reviewer = #321 original = #325 author) |
|---|---|
| R321-S05 `UI/UiKit.cs` | `336c914a0079e6593a4ee57cfc634650cdf3bb04a072d51c2a862bf19a011c20` |
| R321-S06 `UI/ScenarioOneUiRoot.cs` | `9f17d9885de9479078c6b9cde0342e95e39469dd5c7e84b1b6ac403ab3e2dbc8` |
| R321-S06 `UI/ZHiddenBase.cs` | `76e16e5fc28e949512fc8795bc8cea9737177e3dce2369fb003f8b66177970ce` |
| R321-S07 `UI/ScenarioOneUiRoot.cs` | `9f17d9885de9479078c6b9cde0342e95e39469dd5c7e84b1b6ac403ab3e2dbc8` |
| R321-S07 `UI/UiKit.cs` | `b90f8d5dd6c7f0283b6690a73df7dffb9f735d267ffb94df5aa932ea0eabe021` |

**5/5 files identical** (`materialize.py` exit 0, `out/witness-fidelity.json`).

## 2. Reference set (hash verification)

I reused the author's copies at `/media/jer/Files/CCF/Verification/CTA325-CompanionNameRepair-20261107/refs/` and independently SHA256-verified each file against the #321 manifest `roslyn-reference-shas.tsv` (manifest order, `NNN-<basename>`): **320 manifest rows, 320 copies, 320 match, 0 mismatch, 0 extra** (`scripts/verify_refs.py`, exit 0, `out/ref-hash-verification.json`). The compiler received exactly these 320 Files paths. No ZX20 file was read or written by this review.

## 3. Roslyn C# 9 compile and semantic proof

Tool: reviewer copy of the #321 symbol tool (`roslyn-tool/Program.cs`), extended to print the receiver symbol, its kind and its declaring file for every `Time.<x>` and `DisplayStyle.<x>` in `ScenarioOneUiRoot.cs`, plus the root base type. Built and run under SDK **8.0.133** from the pinned cwd (`dotnet --version` = 8.0.133; build exit 0; run exit 0). Source set: all non-Editor `Assets/**/*.cs` (189 files), `LanguageVersion.CSharp9`, defines `UNITY_6000_6_OR_NEWER;UNITY_6000_0_OR_NEWER;UNITY_2023_1_OR_NEWER;UNITY_2022_1_OR_NEWER;UNITY_2021_1_OR_NEWER;UNITY_2020_1_OR_NEWER;ENABLE_INPUT_SYSTEM;UNITY_STANDALONE;UNITY_STANDALONE_LINUX`, allowUnsafe as before.

| Case | Errors | Root base | `Time.frameCount` (x3) / `Time.unscaledTime` (x2) receiver |
|---|---|---|---|
| BASELINE (`c83fae1a`) | 0 | `UnityEngine.MonoBehaviour` | NamedType `UnityEngine.Time` [metadata UnityEngine.CoreModule] |
| R321-S05 | 0 | `UnityEngine.MonoBehaviour` | NamedType global `Time` [`UI/UiKit.cs`] |
| R321-S06 | 0 | **`ZHiddenBase`** | NamedType `ZHiddenBase.Time` [`UI/ZHiddenBase.cs`] |
| R321-S07 | 0 | **`ZHiddenBase`** | NamedType `ZHiddenBase.Time` [`UI/UiKit.cs`] |
| A326-N06 tuple field | 0 | `UnityEngine.MonoBehaviour` | **Field `ScenarioOneUiRoot.Time`** [root], member `(int frameCount, float unscaledTime).frameCount` |
| A326-N07 tuple property | 0 | `UnityEngine.MonoBehaviour` | **Property `ScenarioOneUiRoot.Time`** [root] |
| A326-N08 tuple `DisplayStyle` | 0 | `UnityEngine.MonoBehaviour` | `Time` unchanged; **all 9 `DisplayStyle.Flex` + 13 `DisplayStyle.None` → Field `ScenarioOneUiRoot.DisplayStyle`** (Flex returns `DisplayStyle.None`) |
| L326-U01 uncollected file | 0 | `UnityEngine.MonoBehaviour` | NamedType global `Time` [`Assets/ForestPrototype/ZClock.cs`] |

Full per-case output: `out/roslyn-results.txt` (incl. N01/N02 rebinding and the invalid cases below).

## 4. Real full checker (check 08 cause-asserted)

Command per case, cwd `/media/jer/Files/CCF/Verification/CTA321NameIndependentReview-20261107/sdk8/cwd` (global.json one level up: `8.0.133`, `rollForward: disable`), overlay into the reviewer clone `cand/` then `git checkout -- Assets` + removal of added files, clean-status assertion before and after every case:

`python3 -B …/cand/Tools/Verification/OvernightPlaytestFeedback/check_baseline.py --rotation-state post-fix --instruction-state post-fix`

| Case | Valid C#? | Exit | Tally | Failed | Check-08 cause | Outcome |
|---|---|---|---|---|---|---|
| **R321-S05** | yes (0 err) | 1 | 9/1/2 | 08 only | `UiKit.cs:120: collected companion type 'Time' redefines a reviewed root identifier … (F-2 name binding)` | **PASS (closed, right cause)** |
| **R321-S06** | yes | 1 | 9/1/2 | 08 only | `ScenarioOneUiRoot.cs:4: using alias 'MonoBehaviour' shadows a name the reviewed text reads (N-2)` | **PASS** |
| **R321-S07** | yes | 1 | 9/1/2 | 08 only | same alias cause | **PASS** |

Neither is an inventory failure: the collector, file inventory and checks 01–07, 09–12 all pass (9 PASS); only 08 fails, with the name-binding predicate.

## 5. Guard soundness review and reviewer adversarial variants

**Implementation read.** `_root_reviewed_names()` (checker l.1655) now unions the root header, screen enum, all root pins, member/statement snippets, state declarations, Start writes, `TryShowReviewYear` body and routing input with `_ROOT_FOREIGN_NAMES` (117 names; a strict superset of the old `_guard_root` set, so no prior guard was narrowed). `_guard_root` passes it to the unchanged `_guard_shadowing`, whose alias regex `(?<![\w.])using\s+@?([A-Za-z_]\w*)\s*=` now catches `MonoBehaviour`. `_guard_companion_rebinding()` (l.1672) runs `_structure(_lexed(…))` on **every** collected companion and fails any declared `class/struct/interface/enum/record` whose name is in that set, except path-specific top-level intended declarations and the view partials. It runs inside `_class_wide_guards` for all companions. Owning-type scope is conservative (nested and namespaced types count). The lexer used is the hardened N-4 lexer, so unicode-escaped identifiers, Cf characters and literals spanning `#if` lines fail closed (in-memory probes `scripts/probe_escape.py`: 4/4 rejected, `backslash outside a literal`).

**Variants** (`scripts/cases.py`; in-memory `scripts/inmem.py`; compile above; full checker `out/commands-cases.json`):

| ID | Edit | Valid C#? | Full checker | Assessment |
|---|---|---|---|---|
| A326-N01-alias-Z | root `using  @MonoBehaviour  =  ZOtherBase ;` + base in UiKit.cs | yes; base → `ZOtherBase` | exit 1, 9/1/2, 08: alias `MonoBehaviour` (N-2) | rejected, right cause |
| A326-N02-nested-ns-Time | UiKit.cs: `namespace UnityEngine { public static class Time … }` (source beats metadata, CS0436 warning only) | yes; `Time` → source `UnityEngine.Time` in UiKit.cs | exit 1, 9/1/2, 08: companion type `Time` (F-2) | rejected, right cause |
| A326-N03 | UiKit.cs global `enum DisplayStyle` | **no** (82 errors) | exit 1 (F-2) | not credited as a valid negative |
| A326-N04 / N05 | root `using static ZClockHost;` / `ZStyleHost;` importing a static *property* `Time` / `DisplayStyle` | **no** (CS0229 ambiguity) | exit 1 (pre-existing RR-3 using-static ban) | not credited; route is closed by C# itself |
| **A326-N06-root-tuple-field-Time** | root: `private (int frameCount, float unscaledTime) Time = (-2, 0f);` | **yes**, rebinding proved | **exit 0, 10/0/2, 08 PASS** | **FALSE GREEN** |
| **A326-N07-root-tuple-prop-Time** | root: `private (int frameCount, float unscaledTime) Time => (-2, 0f);` | **yes** | **exit 0, 10/0/2** | **FALSE GREEN** |
| **A326-N08-root-tuple-DisplayStyle** | root: `private static readonly (UnityEngine.UIElements.DisplayStyle Flex, UnityEngine.UIElements.DisplayStyle None) DisplayStyle = (…None, …None);` | **yes** | **exit 0, 10/0/2** | **FALSE GREEN** (Objectives panel can never be shown) |
| L326-U01-uncollected-Time | S05's `Time` stub in a new `Assets/ForestPrototype/ZClock.cs` with no hint text | yes | exit 0, 10/0/2 | documented "uncollected source" limit (packet excludes it); reported, not counted |
| A326-B01 | UiKit.cs `using Z = UnityEngine.Time;` | yes | exit 0, 10/0/2 | benign accepted |
| A326-B03 | UiKit.cs unrelated global `ZClockHost`/`ZFrameSource` types | yes | exit 0, 10/0/2 | benign accepted |
| A326-B04 | hinted `ZNotes.cs` with nested `Clock` | yes | exit 0, 10/0/2 | benign accepted |
| A326-P01 | root `using static UnityEngine.Mathf;` | yes | exit 1 (RR-3) | pre-existing conservative over-rejection, unchanged by #325 |

The five canonical insertion points use the anchor `public void ShowObjectives()` (members) and `using UnityEngine.InputSystem;` (directives), each asserted to occur once.

**Exact minimal witness (NEEDS CHANGES):** in `Assets/ForestPrototype/UI/ScenarioOneUiRoot.cs` at `c83fae1a`, insert immediately before `public void ShowObjectives()` (inside the class body, 4-space indent):

```csharp
private (int frameCount, float unscaledTime) Time = (-2, 0f);
```

Roslyn C# 9 with the 320 verified references: 0 errors; all three `Time.frameCount` (including `helpClosedFrame == Time.frameCount` in the pinned `SetAuxiliaryPanelOpen` statement) resolve to field `ScenarioOneUiRoot.Time`. Real `check_baseline.py --rotation-state post-fix --instruction-state post-fix`: **10 PASS / 0 FAIL / 2 NOT-TESTABLE, exit 0**. Mechanism: `_member_names` (checker l.1230) appends the identifier preceding the first top-level `(` — `private` — then skips to `;`, so the declarator `Time` is never compared with `_ROOT_FOREIGN_NAMES`. The plain-typed control `private ZFrame Time;` is rejected (`member 'Time' of ScenarioOneUiRoot shadows a type …`).

**Pre-existence** (`scripts/preexist.py`, `out/preexisting-vs-candidate.txt`): N06/N07/N08 and L326-U01 are accepted by both the `3033dd12` and `c83fae1a` checkers; S05–S07, N01–N03 change from accepted to rejected. So #325 introduces no regression; the residual belongs to the #320 N-2 member scan that #321 sampled with S01–S04 (comma, initialiser, auto-property, `@Time` shapes) but not with tuple types.

## 6. Historical suite and gates (actual runs on `c83fae1a`)

| Gate | Command | Exit | Result |
|---|---|---|---|
| Committed suite | `python3 -B cand/…/test_objective_cta_source_check.py` (cwd SDK8, `PYTHONDONTWRITEBYTECODE=1`, 275 s) | **0** | **884/884 rejected, false_greens 0, wrong_cause 0**; benign **32 + 32 (loop-1) + 48 (#320) = 112/112**; loop-1 probes **9 limits + 9 over-rejections, changed=0**; #320 probes **52, changed=0**; #325 repaired 3/3, nearby 5/5, benign 5/5; historical BASE rejected; 0 FAIL lines, empty stderr |
| #305 corpus fidelity + outcomes | `scripts/fidelity.py` (original `CTA-F1F3-Review-304-20261106/scripts/review_mutations.py`, sha256 `5e463871…`) | **0** | 115/115 original definitions equal to the suite's; **X-F1/X-F2 81/81 rejected; known-50 50/50 rejected**; R-IDs 21/21 rejected; benign 13/13 accepted; 0 crashes |
| #314 654 corpus fidelity | same driver (original `my_mutations*.py` rounds 1–11) | **0** | **654/654** definitions, **0 mismatches**, the 5 documented reclassifications (W-DEP-05, four A8-L06); required **57 = 47 N-1 + 10 N-2**; **59/59 files byte-identical** to the original `loop1-rerun/out/mine8–11/mutants` exports; 57/57 in-memory rejected for intended cause |
| 47 + 10 real full-checker runs | `scripts/e2e.py req57` | **0** (driver); each checker run **1** | **57/57** (47 N-1 + 10 N-2): exit 1, 9/1/2, only check 08 FAIL, check-08 text contains the intended cause (`inside an interpolation hole (N-1)` ×47, `(N-2 owning class)` ×10); clone restored clean after each (`out/commands-req57.json`) |
| Candidate normal | real checker, SDK8 cwd, traced | **0** | **10 PASS / 0 FAIL / 2 NOT-TESTABLE** |
| Historical `72b4efbd…` + current checker | `hist/…/check_baseline.py --rotation-state pre-fix --instruction-state pre-fix` | **1** | **6 / 4 / 2**, failed 01, 02, 03, 08 (intentional red) |
| `baseline-output.txt` | `git rev-parse`/`sha256sum` at `3033dd12` and `c83fae1a` | — | identical: blob `db6a8b850dce7efadaac6fe1b87b1dbba469428f`, SHA256 `513cddd9babddaede82212f8e4891ecfcf9823c13a69474a3665f890a7f05dc1` |
| Python compile | `PYTHONPYCACHEPREFIX=<review>/pycache python3 -m py_compile` (3 files) | **0** | — |
| `git diff --check 3033dd12 c83fae1a` | — | **0** | — |

**SDK8 proof** (`out/e2e/candidate-normal-sdk8.{hostfxr,strace}`, `out/sdk-summary.json`): hostfxr found `…/CTA321NameIndependentReview-20261107/sdk8/global.json`, "SDK path resolved to `/usr/lib/dotnet/sdk/8.0.133`", using `sdk/8.0.133/dotnet.dll`; strace **278** successful SDK8 opens (13 under `sdk/8.0.133/Roslyn`), **0** SDK10. Control from `control-cwd` (no global.json up the tree): resolved `sdk/10.0.114`, **504** SDK10 opens, **0** SDK8; also 10/0/2 exit 0. Both runs printed **six distinct `RackEditing.SnapFar` cases, all PASS**: near-angle, near-angle-disabled, outside-angle-window, near-boundary, near-boundary-disabled, outside-boundary-window (isolated .NET math calls, not Unity input).

No synthetic echo or accidental FAIL is credited: invalid-C# variants (N03, N04, N05) are listed but excluded from validity counts, and every credited rejection asserts the exact check-08 predicate with all other checks passing.

## 7. Diff-scope audit (`out/scope-audit.txt`)

- `3033dd12..c83fae1a`: exactly **5 paths** — `objective_cta_source_check.py` (+41/−2), `test_objective_cta_source_check.py` (+76/−2), `check_baseline.py` (+1, one string line in the test-08 **PASS** note; FAIL path untouched), `README.md` (+3), author result doc (+471). All allowed by the packet.
- `Assets/`, `Packages/`, `ProjectSettings/`, scenes/prefabs/assets, `baseline-output.txt`: **0 changes**.
- 3 linear commits, 0 merges; `3033dd12`, `8ff6352b` and `b040a992` are ancestors of the head; origin == `c83fae1a` (fast-forward, no rewrite evidence). `c83fae1a` is not in `origin/main`. PR #300 is open and unmerged; its head ref *is* this branch, so the author pushes update PR #300 by design; no PR #300 ancestry was merged anywhere.
- Tests not weakened: the only removed test lines are a docstring sentence (extended) and the return condition (extended with `or rebinding_failed`). The checker's only removed lines replace the old `reviewed` expression with the superset function.
- Cosmetic: the inserted test-08 PASS sentence sits between two parenthetical clauses ("…Objective 1 (reviewed root names…); (unpinned root members…)"). Harmless.

## 8. Limitations and what was NOT verified

- **Uncollected source (documented, out of #325 scope):** any new file without a companion hint can declare global `Time` (L326-U01) or `MonoBehaviour` and rebind the pinned root; the checker reports 10/0/2 exit 0. F-2's protection therefore depends on the collector heuristic.
- **Root member declarator shapes (undocumented, in collected source):** tuple-typed members as above. I did not enumerate every other declarator shape; this is a review, not a parser proof.
- Remaining documented limits are unchanged: reflection, unpinned root callers/members, game/teaching state, object lifetime, styles, exceptions/non-termination, general C# semantics; the internals of the path-excepted companion types are not pinned; F-2 conservatively over-rejects nested/namespaced types named like any of the 117 reviewed identifiers (including lower-case locals such as `name`, `tree`, `entry`).
- The Roslyn compile is a Unity-free C# 9 compile against copied Player-like references; no Unity import, ILPP, domain reload, runtime, rendering, input or A–O gate was run or is claimed.
- The Manager-only `active-tasks.md` and dashboards were not touched.
- Read-only integrity: the #325 source lane stayed at `c83fae1a` with an empty `status --porcelain -uall --ignored`; no file under the #321 evidence, `CTA325-CompanionNameRepair-20261107`, `CTA325-Proof-20261107` or the #305 scripts changed during this review. **Observed, not caused by this review:** at 13:30:46 UTC five new loose Git objects (an `AGENTS.md` blob, other blobs and a tree) appeared in `/media/jer/Files/CCF/Verification/CTA-314-IndependentReview-20261107/source-clone/.git/objects/`; that clone's `HEAD`/`index` are unchanged since 03:26. No reviewer script references that clone (my fidelity driver only imported `loop1-rerun/scripts/*.py` and had exited at 13:27:10; at 13:30 my only process was the req57 checker loop in my own `cand/` clone, which is `--shared` from the bare store). Likely a concurrent agent; flagged for the Manager, not investigated further.

## 9. Roslyn recommendation

Yes — a **narrow Roslyn semantic-binding assertion is now the proportionate next step**, rather than another lexical loop. Three review rounds (#314, #321, #326) each found a new compile-clean way to rebind a pinned simple name; each lexical fix closes the witnesses but not the class. The reviewer tool used here (≈50 lines) already proves the needed facts directly: compile the whole non-Editor source set and assert that the pinned root's base is `UnityEngine.MonoBehaviour` and that every receiver the pinned text reads (`Time`, `DisplayStyle`, `Visibility`, `Button`, `Label`, `VisualElement`, `Dictionary`, the companion types …) binds to its expected symbol. That would close the tuple witnesses and the uncollected-file limit together, without a general parser. Costs for Manager to weigh: a dependency on the 320 Unity reference DLLs (today from a Unity install/`Library`, read-only) and a .NET SDK in the gate, and a second, slower check path. The alternative is to accept this class explicitly as a labelled bounded risk (README limit + accepted-limit probe for N06/N08 + L326-U01). Manager #200 decides; I start nothing.

## 10. Commands and evidence

Evidence root (reviewer-owned): `/media/jer/Files/CCF/Verification/CTA321NameIndependentReview-20261107/` — `scripts/` (verify_refs, cases, inmem, probe_escape, probe_tuple, materialize, fidelity, e2e, preexist), `roslyn-tool/`, `sdk8/global.json`, `mutants/`, `req57/`, `out/` (every stdout/stderr/exit, `commands-gates.json`, `commands-cases.json`, `commands-req57.json`, traces under `out/e2e/`).

```bash
V=/media/jer/Files/CCF/Verification/CTA321NameIndependentReview-20261107
python3 -I -B $V/scripts/verify_refs.py $V/out                       # exit 0: 320/320
python3 -I -B $V/scripts/materialize.py                              # exit 0: 5/5 witness files identical
(cd $V/sdk8/cwd && dotnet build $V/roslyn-tool/RoslynCheck.csproj -o $V/roslyn-tool/bin --verbosity quiet)   # exit 0
(cd $V/sdk8/cwd && dotnet $V/roslyn-tool/bin/RoslynCheck.dll $V/scratch $V/mutants $V/out/refs.txt '<defines above>' $V/out/roslyn-results.txt)  # exit 0
python3 -I -B $V/scripts/e2e.py gates                                # exit 0 (normal 10/0/2 exit0, control, historical 6/4/2 exit1)
python3 -I -B $V/scripts/e2e.py cases                                # exit 0 (driver); per-case exits in commands-cases.json
(cd $V/sdk8/cwd && PYTHONDONTWRITEBYTECODE=1 python3 -B $V/cand/Tools/Verification/OvernightPlaytestFeedback/test_objective_cta_source_check.py)  # exit 0
PYTHONDONTWRITEBYTECODE=1 python3 -B $V/scripts/fidelity.py         # exit 0
python3 -I -B $V/scripts/e2e.py req57                                # exit 0: 57/57 valid cause-asserted rejections
python3 -I -B $V/scripts/preexist.py                                 # base vs candidate outcomes
```

Reviewer opinion only. Manager #200 issues any source approval or the tooling-design decision; no integration, Unity, Windows build or A–O claim follows from this review.
