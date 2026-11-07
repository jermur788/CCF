# #321 — Independent N-1/N-2 CTA checker closure review

**Reviewer opinion: NEEDS CHANGES — SOURCE SCOPE ONLY.** The original 47 N-1 and 10 N-2 repairs reproduce, but three new compile-clean name-shadowing variants survive both the scanner and the real full checker (`10/0/2`, exit 0 each). See findings F-1/F-2 and the final execution table below. These are defects in the verification boundary demonstrated by hypothetical mutations; this review does not establish a defect in the unchanged production UI.

**Reviewer PASS is not Manager #200 acceptance and cannot release integrated A–O.** I cannot approve the code, grant a lease or make a Manager ruling. No Unity Editor/Player, native input, rendering, runtime, save or integrated-candidate evidence is claimed.

## Identity, authority and exact target

| Item | Observed value |
|---|---|
| Reviewer | OpenAI `gpt-6.1-sol`, **high** reasoning effort, Codex harness through T3 Code, as identified by this session's runtime instructions. Account/subscription/quota identity: **UNKNOWN**. No helpers or further delegates used. |
| Independence | Separate reviewer session and cross-provider family from the author, identified by the task and author handoff as Anthropic Claude Opus 5.5 via Claude B. I did not author or change the checker. Author account/effort are not independently observable here. |
| Dispatch | Attempt **2**. The user reports that attempt 1 on another OpenAI/Codex instance hit a usage limit and produced no work. That account incident was not directly observed by this reviewer. |
| Binding authority | [#321 review packet](https://github.com/jermur788/CCF/blob/4ad68c27f2b5a9bda74b5567a741813fb018b80b/Docs/Handoffs/ScenarioOne-Morning-Closure-20261107/MANAGER-CTA-309-TWO-GUARD-INDEPENDENT-REVIEW-PACKET.md), [#320 repair packet](https://github.com/jermur788/CCF/blob/4ad68c27f2b5a9bda74b5567a741813fb018b80b/Docs/Handoffs/ScenarioOne-Morning-Closure-20261107/MANAGER-CTA-309-TWO-GUARD-CLOSURE-PACKET.md), #309 contract and [#314 final review](https://github.com/jermur788/CCF/blob/9911793352c9236828f17380c4a5603cb7e3422b/Docs/Handoffs/ScenarioOne-Overnight-Playtest-Readiness-20261107/CTA-309-INDEPENDENT-REVIEW-RESULT.md). The user's direct packet authorises this publication. |
| Exact source head | `3033dd12fed73136c8f94c82d58bf0d7cacd5380`, branch `verify/scenario-one-feedback-cta-anchor-20261106`. |
| Tested code | `8ff6352b34a0fcbff7d0606b1d9ab28c6dd67a4c`. Verification files are byte-identical between this commit and `3033dd12`. |
| Comparison base | `753143e62a5cf1d7436874e731766c4893a4c7c7`. Historical intentionally red source: `72b4efbd1c653fa2231f632a79030a83ff76f833`. |
| Docs lane | `/media/jer/Files/CCF/Operations/CCF-CTA309TwoGuardReview-20261107`, new branch `docs/cta-309-two-guard-independent-20261107-v1`, from fetched `origin/main` **`4ad68c27f2b5a9bda74b5567a741813fb018b80b`**. Current canonical context commit: `4790c20918ca2a667b04d80262142d9b4151b60e`. |
| Preflight | New path/local branch/remote branch unused; clean tracked/index state before scratch; no unrelated orchestrator changes observed. Shared store `/media/jer/Files/CCF/Repository.git`; Files `/dev/sdb4`, ext4, mounted read/write, about 47 GiB free. No Unity lane or lease used. |
| Evidence root | **`/media/jer/Files/CCF/Verification/CTA-309-TwoGuardReview-20261107/`**, abbreviated **`$E`** below. Scratch clones are only under my docs lane's `scratch/`. Logs, scripts and compiler inputs are under `$E`. |

[RUNNING receipt](https://github.com/jermur788/CCF/issues/321#issuecomment-6705415211). The source lane, Unity lane, orchestrator worktree and `active-tasks.md` were not edited. Only this result document is published from my branch. No PR was created, pushed, merged or approved; no main integration, rebaseline, package, version, save or Reference change was made.

## Findings, ranked by severity

### F-1 — Medium, blocking reviewer opinion: alias bypasses the new base-class/shadow guard

**Cases `R321-S06` and `R321-S07`.** Add this directive to the root file:

```csharp
using MonoBehaviour = ZHiddenBase;
```

Supply this base in an included companion file:

```csharp
public abstract class ZHiddenBase : UnityEngine.MonoBehaviour
{
    protected static class Time
    {
        public static int frameCount => -2;
        public static float unscaledTime => 0f;
    }
}
```

`R321-S06` uses a new `UI/ZHiddenBase.cs` with an explicit companion hint. `R321-S07` appends the base to the existing **`UiKit.cs` companion**, so there is no question about input collection. Both leave the pinned root header `public sealed class ScenarioOneUiRoot : MonoBehaviour` unchanged.

The scanner accepts both. C# 9 compilation has zero errors. Roslyn's semantic model proves the root now derives from `ZHiddenBase` and the three `Time.frameCount` reads resolve to **`ZHiddenBase.Time.frameCount`** in the injected source. Consequently the unchanged pinned help-close frame test reads a constant `-2`; after `CloseHelp` writes that value, the equality guard continues to suspend guidance rather than suppressing it just for the real close frame. This is a source-semantic consequence, not an observed Unity execution.

**Cause:** `objective_cta_source_check.py:2000` pins `_ROOT_HEADER` text, but the `reviewed` identifier set at line 2001 is assembled without that header. `_guard_shadowing` at line 1281 rejects only aliases whose names appear in that set. `MonoBehaviour` is absent. A lexical base-name pin therefore does not bind the base type, despite the newly advertised new-base-class protection. These variants defeat the same inherited-name mechanism as the author's rejected `N2-09`.

**Evidence:** `$E/new_variants.py`; `$E/new-mutants/R321-S06/` and `R321-S07/`; `$E/new-variants.json`; `$E/companion-proof.json`; `$E/symbol-results.txt`; `$E/roslyn-results.txt`; real-checker logs `$E/e2e/R321-S06.*` and `R321-S07.*`.

**Requested disposition:** repair or return this explicit residual to the Manager; do not call base/shadow closure complete. A narrow check for rebinding the pinned base name is a possible direction. No implementation or scope expansion is proposed as reviewer work.

### F-2 — Medium, blocking reviewer opinion: a collected companion can redefine a name read by the pins

**Case `R321-S05`.** Append a global static `Time` type to the existing `UiKit.cs`, exposing the time properties used by the candidate and returning constant `frameCount = -2`. No pinned method or declaration text changes.

The scanner accepts it. The entire 189-file production C# set compiles at C# 9 with zero errors. Roslyn resolves the root's pinned `Time.frameCount` references to **`Time.frameCount` declared in `UiKit.cs`**, instead of `UnityEngine.Time`. The help-close equality becomes permanently true after close, as above. Other time reads are affected too; the case is deliberately defective.

**Cause:** `_guard_shadowing` scans the root file and the view partial files, while `_class_wide_guards` at line 1700 sends `UiKit.cs` only through `_guard_uikit`. The latter pins six builders and primary-class text, without auditing additional types that can change simple-name lookup in the root. The file is already collected and inspected, so this is not the disclosed L-01 case of an uncollected source/constructed element name.

**Evidence:** `$E/new-mutants/R321-S05/Assets/ForestPrototype/UI/UiKit.cs`; `$E/new_variants.py`; `$E/symbol-results.txt`; `$E/roslyn-results.txt`; `$E/e2e/R321-S05.*`.

**Requested disposition:** bound the foreign-name protection across the actual collected input, or obtain a Manager decision accepting a specifically disclosed residual. The reviewer does not require a universal C# parser.

### F-3 — Informational: trace counts label an exec as an open

The SDK-selection evidence is valid. The author's counting command counts every non-ENOENT trace line containing `sdk/8.0.133`, rather than just successful `openat` calls. Its **281** includes **280 successful opens plus one compiler `execve`**. My independent trace produces the same 281/280 distinction and zero SDK10 opens. The unpinned control selects SDK10. This is a minor reporting imprecision, not evidence of an SDK substitution or fabricated test. `$E/trace-recount.json` includes the exact lines and recount of both original and independent traces.

## Packet outcomes

| Packet item / class | Outcome | Independent evidence / practical limit |
|---|---|---|
| 1 — original N-1 holes | **PASS** | **47/47**, original exported source identity, C# 9 zero errors, real test 08 alone FAIL, intended N-1 cause, `9/1/2`, exit **1** each. `$E/required57.json`, `required57-source-fidelity.json`, `roslyn-results.txt`, `e2e/`, `audit-results.txt`. |
| 1 — original N-2 decoys | **PASS** | **10/10**, same validity/process requirements; intended owning-class cause, `9/1/2`, exit **1** each. Same proof set. |
| 1 — fresh adversarial N-1/decoy/member variants | **PASS for tested valid shapes** | Six new interpolation writes, one new decoy and four member-shadow shapes reject in memory and real test 08; all 11 compile cleanly. No claim from the two invalid write probes. `$E/new-variants.json`, `roslyn-results.txt`, `e2e/`. |
| 1 / 4 — fresh alias/cross-file name-shadowing | **FAIL** | **R321-S05/S06/S07**: C# 9 zero errors, actual semantic rebindings, real `10/0/2`, exit **0**, no FAIL or stderr. Findings F-1/F-2. |
| 2 — #305 original 81 / known 50, plus 21 R-controls | **PASS** | All **102/102** reject; old scanner's 52 accepts reconcile to 50 defects plus X-F1-31/32. `$E/corpus305.*`, `original50-ids.json`, suite group logs. |
| 2 — all previous 275 author/reviewer negatives | **PASS** | Exact preserved groups all reject; incorporated into the **884/884**, wrong-cause **0**, suite exit **0**. `$E/suite.*`. |
| 2 — requested 35 reviewer + 56 prior benign | **PASS** | All accepted, exact ID reconciliation in `$E/requested-benign-ids.json`; no new benign regression. Full suite benign invocations **32 + 32 + 48 = 112**, all accepted. |
| 2 — original #314 176, expanded 654 | **PASS as disclosed** | Independently loaded original definitions, zero crashes. Original six rounds: 128 rejected negatives, W-DEP-05 disclosed accepted limit, 21 benign accepted, probes retained. All rounds: 576 counted negatives reject, 35 benign accept; four pre-existing L-06 over-rejections remain disclosed. `$E/independent-rounds.*`, `fidelity.*`. |
| 2 — accepted-limit / over-rejection controls | **PASS as disclosed** | Original **9 + 9** probes unchanged; new group **52** probes unchanged, including O-10…O-16 and L-09/L-10. O-15 is explicitly invalid C#; the other six new O-probes compile. `$E/suite.stdout`, `roslyn-results.txt`. |
| 2 — unchanged regression controls / positive / historical | **PASS** | Positive **10/0/2 exit 0**; historical **6/4/2 exit 1**; original numbered checker controls preserved. Baseline fixture byte-identical. `$E/candidate-traced.*`, `historical.*`, `scope.json`. |
| 3 — actual six SnapFar tests, cwd-pinned SDK8 | **PASS** | Six distinct PASS lines; actual SDK/compiler paths traced to **8.0.133**; unpinned control resolves **10.0.114**. `$E/candidate.strace`, `.hostfxr`, `control.strace`, `.hostfxr`, `trace-recount.json`. F-3 is count wording only. |
| 3 — Python / independent C# compiler gates | **PASS with invalid probes separated** | Three Python files compile, exit **0**. Rebuilt Roslyn scaffold baseline **189/0 errors**; all original 57 and three new false greens compile cleanly. Compiler-only evidence, **not Unity**. `$E/pycompile.*`, `roslyn-results.txt`, `symbol-results.txt`. |
| 4 — exact five-path scope / protected assets / no PR import | **PASS** | Exact full diff, only two single-parent commits after base; code same at tested/report commits. No Assets/UI, dependency, version/save/Reference or main change. `$E/scope.json`. |
| 4 — hole rules and direct owner/brace binding | **PASS for reproduced boundaries** | Original 57 and valid fresh hole/decoy cases close correctly, unsupported tested shapes reject, unrelated types/read-only interpolation remain accepted. Name-identity residuals above prevent a blanket closure PASS. |
| 4 — README / diagnostic scope | **FAIL for new residual disclosure; otherwise reproduced** | Existing L/W/O limitations are explicit and accurate. The alias/base and collected-companion shadow cases require a specific disposition; neither is established closed by local text pins. |
| 5 — evidence / independence / reviewer-only verdict | **PASS** | Cross-provider reviewer, exact target, individual reasons/exits, invalid probes and limitations disclosed. **NEEDS CHANGES**, source scope only; no Manager acceptance or A–O release. |
| Unity / native / integrated A–O / saves / renders | **NOT RUN** | Prohibited and outside this review. No lease, Unity process or acceptance claim. |
| Serial old/new suite timing comparison; dashboard publication readback | **NOT RUN** | Candidate elapsed time recorded; no before/after benchmark or dashboard synchronization claim. |

The final audit asserts **75/75 real end-to-end invocations**, all with empty stderr: the 57 required negatives, 11 valid new rejected negatives, three valid new false greens, one valid new benign, and three invalid exploratory probes explicitly excluded from validity claims. `$E/audit.exit` is **0** and `$E/audit-results.txt` records the assertions. The e2e scratch clone ended clean.

## Independent reproduction method and raw proof

I inspected the earlier harnesses before using their data. I did **not** run their hard-coded writing/overlay scripts against their original lanes. My driver `$E/independent.py` loads unchanged **original #314 mutation definitions**, supplies only the file-name constants they import, applies each edit to exact candidate source in memory, filters companions with the real collector predicate, and treats crashes separately from rejection. It exports changed source into my evidence directory. The old artifact's own accepted verdicts select the 57 required cases, independent of the candidate's current result.

`$E/required57.json` contains all exact IDs: A8 decoys ×6, A9 decoys ×4, A10 holes ×28, A11 holes ×19. `$E/required57-source-fidelity.json` proves **59 changed files across those 57 cases are byte-identical to the #314 review artifact's exported sources**. `$E/fidelity.stdout` independently compares all **654/654** original definitions with the candidate's transcription, with zero edit/why/group/transform mismatches. The only five expectation changes are the disclosed W-DEP-05 → limit and four pre-existing L-06 benign-labelled over-rejections → over. No reclassification of a new reviewer defect was made.

`$E/e2e.py` overlays one case at a time **only in my separate `scratch/e2e` clone**, invokes the exact unedited committed `check_baseline.py` in a subprocess with post/post flags and SDK8-pinned cwd, captures stdout/stderr/exit, and restores every file in `finally`. This runs all six actual SnapFar calls as part of each full command. A counted closure requires exactly one FAIL, test 08, its intended cause, `9/1/2` and exit 1. Raw logs are `$E/e2e/<case-id>.stdout`, `.stderr`, `.exit`; aggregate `$E/e2e-summary.json`. Newly accepted mutants require full `10/0/2`, exit 0, not just a scanner row count.

For #305, `$E/corpus305.py` reads the **original** `CTA-F1F3-Review-304-20261106/scripts/review_mutations.py` definitions. It independently runs 81 X-cases, 21 R-cases and 13 benign edits. The original `1585ca20` scanner accepts 52 X-cases: the original #305 contract excluded `X-F1-31` and `X-F1-32`, leaving the recorded 50 defects. The current candidate rejects all 102, including both subsequently forbidden additional primaries. CRLF/BOM benign controls are additionally covered by the committed suite and the original #314 replay.

The original #314 six rounds contain **176** cases: 128 expected negatives reject, W-DEP-05 stays accepted as the disclosed game-progress limit, 21 benign controls accept, and the remaining probes retain their documented outcomes. All 11 rounds contain **654** cases: 576 `rej` cases reject; W-DEP-05 alone among original `rej` labels accepts; 35 benign cases accept; four benign-labelled L-06 controls remain rejected as documented. The remaining 38 limit/exploratory cases have their recorded outcomes. Full individual reasons: `$E/independent-rounds.json` and `.stdout`; no scanner crashes.

The requested **35 reviewer + 56 prior-suite benign controls all accept**. The 56 are exactly the four groups selected in the original `fix_feasibility_l1c.py` (`loop1_reviewer_benign`, `loop1_benign`, `benign`, `new_benign`); their IDs and the 35 reviewer IDs are in `$E/requested-benign-ids.json`, audited against actual accepted log lines. These sets overlap other groups; 112 is the suite’s benign invocation count, not a claim of 112 unique source files.

The committed suite is independently run from the exact candidate scratch clone. Its 275 earlier negatives are preserved; original 20, #304 F1 14/F2-F3 21, #305 R 21/X-F1 35/X-F2 46, #309 RR-1 28/RR-2 15/RR-3 14/N-4 5, and loop-1 56 all remain exercised. The additional original reviewer rounds and author's new variants are exercised without replacing those groups. See `$E/suite.stdout`, `.stderr`, `.exit`. No skip/xfail/rebaseline or weakened default was introduced in the diff.

## Toolchain and historical controls

`$E/gates.py` sets `TMPDIR` and Python caches under `$E`, disables shared compiler reuse, and runs from **`$E/sdk8/cwd/deeper`**, whose ancestor `$E/sdk8/global.json` pins SDK `8.0.133`, `rollForward: disable`.

- Python compile: the exact three edited Python files, `$E/pycompile.*`.
- Positive full checker: `$E/candidate-traced.stdout`, `.stderr`, `.exit`; six distinct `SOURCE-STATIC PASS SnapFar` lines, `10/0/2`, exit 0.
- Actual process trace: `$E/candidate.strace` and `$E/candidate.hostfxr` resolve `/usr/lib/dotnet/sdk/8.0.133/dotnet.dll` and open **`8.0.133/Roslyn/bincore/csc.dll` and `Microsoft.CodeAnalysis.CSharp.dll`**. No SDK10 opens. The compiler is actually invoked, not inferred from `dotnet --version`.
- Unpinned control: `$E/control.strace`, `$E/control.hostfxr`, `$E/control-traced.*` resolve SDK **10.0.114**, run the same six tests and produce byte-identical checker stdout. SHA-256 of both stdout files: `69fd54fa60065af189e72580e79fa14d99d7876c13fa18a92e3c2b8935634b36`.
- Historical full checker: exact old source with only the three candidate Python scripts copied into my historical clone, pre/pre flags, `$E/historical.*`: `6/4/2`, exit 1. `$E/historical-vs-baseline.diff` contains just the known changed test-08 diagnostic; the frozen fixture was not rewritten.
- `baseline-output.txt`: blob **`db6a8b850dce7efadaac6fe1b87b1dbba469428f`** at both base and target; SHA-256 **`513cddd9babddaede82212f8e4891ecfcf9823c13a69474a3665f890a7f05dc1`**. Exact path diff is empty.

`$E/compile.py` rebuilds a copied, inspected reviewer Roslyn scaffold using SDK8. It compiles the candidate's non-Editor C# sources with **C# 9**, real Unity 6000.6 reference DLLs and the original Player-like defines. Reference hashes are recorded in `$E/roslyn-reference-shas.tsv`. This invokes no Unity process, importer, ILPP or domain reload. `$E/symbol-tool/Program.cs` additionally asks the semantic model what the mutant's root base and `Time.frameCount` actually resolve to; `$E/symbol-results.txt` is the direct symbol proof for F-1/F-2.

Independent Roslyn run: **189-file baseline, 320 references, zero errors**; all **57 required original mutants compile with zero errors**. All 55 author variants/benign/limit/over-rejection sources compile cleanly except deliberately invalid O-15 (one CS0103). `A7-x-teachinghijack` has **24 errors**, confirming the author's disclosure. Total: 131 mutant directories checked, exit 0 for the scaffold (per-case errors are reported individually, not hidden by that aggregate exit).

Of my 18 fresh probes, 15 are compile-clean: six additional interpolation writes (`R321-H03`…`H08`), four member-shadowing shapes (`S01`…`S04`), one decoy (`D01`), three surviving shadows (`S05`…`S07`), and one benign nested-lambda/read-only interpolation (`B02`). The 11 valid new negatives other than the three survivors reject, and B02 accepts. Three exploratory probes I wrote are **invalid**, explicitly excluded from meaningful negative/benign counts: H01 (CS8598, null-suppression operator in an assignment target), H02 (two CS0103 errors from malformed extra tuple parentheses), and B01 (two CS0518 errors because the positional record needs `IsExternalInit`, absent from these references). Their raw results remain visible. This is not a benign regression or proof of valid-C# rejection for those three probes.

## Source inspection, documentation and limitations

The full `753143e6..3033dd12` path diff is exactly:

1. `Tools/Verification/OvernightPlaytestFeedback/objective_cta_source_check.py`
2. `Tools/Verification/OvernightPlaytestFeedback/test_objective_cta_source_check.py`
3. `Tools/Verification/OvernightPlaytestFeedback/check_baseline.py`
4. `Tools/Verification/OvernightPlaytestFeedback/README.md`
5. `Docs/Handoffs/ScenarioOne-Morning-Closure-20261107/T3-CTA-309-TWO-GUARD-CLOSURE-RESULT.md`

`$E/scope.json` records the path list, two single-parent commits and the actual checker-note diff. There is **no merge commit** in this range, no PR #300 ancestor import beyond the already-reviewed base, and no Assets/Packages/ProjectSettings/version/save/Reference change. The test-file deletions are its docstring update and final return expression expanded to include the new group; existing cases/assertions stay intact. `check_baseline.py` changes only test-08 PASS note text. Checks 01–07, 09, SnapFar, error handling and FAIL reporting are unchanged. The report's approximate line-addition figures are not used as integrity evidence; actual diff is authoritative.

N-1 uses the existing lexer's hole spans, expands nested holes into code tokens and applies the same write/receiver rules as statements. It runs before write counts, so the required cases fail for their cause. Assignment/compound operators, tuple nesting, parentheses, ref/out/in, increment and calls/lambdas are covered by the reproduced cases. Unsupported `#` and `::` holes fail closed; nested literal ambiguities continue to fail N-4. I found no new surviving interpolation-write case in my tested shapes. This is bounded lexical checking, not general C# semantics.

N-2 now binds the original decoys correctly by type stack and direct-member depth, requires reviewed methods/constructor to be members, rejects extra modifiers/attributes and ambiguous conditional brace depth, and permits unrelated types. That is a real improvement. However, the new shadowing guard's file/name boundary leaves F-1/F-2, so it does not justify a complete fail-closed claim for base or name rebinding.

README correctly narrows “every write” to enumerated lexical shapes and names reflection L-04; it describes direct owning-class pins instead of arbitrary text matches. W-DEP-05, L-01 aliases/constructed names outside the companion set, unpinned root callers, `ecology`/`nextBadge`/rack-problem inputs and over-rejections are explicitly disclosed. Those disclosures agree with the reproduced probes. They do not specifically disclose the new base-name alias or shadow type **inside an already collected companion**. These cases need an explicit disposition, not a statement that the original 57 passing tests prove the class closed.

Unicode handling remains conservative: Cf outside comments/literals, non-leading BOM, unsupported controls, U+0085/U+2028/U+2029, lone CR in memory, Unicode escapes/raw strings and preprocessor/literal ambiguity fail closed. Existing Unicode cases reproduce; benign ordinary Unicode and CRLF/BOM controls remain accepted. Python/Roslyn Unicode-table equivalence for arbitrary future code points remains unproved, as disclosed. I did not attempt universal parser equivalence.

The disclosed deviations were checked as follows:

- **Y-B-17 CRLF:** original transform is verified and accepted in memory. `Path.read_text()` normalises disk CRLF, so an on-disk run would not exercise those original bytes. In-memory coverage is valid; no disk-preservation claim is made.
- **A7-x-teachinghijack:** now rejected by shadowing guard. Independent Roslyn diagnostics establish invalid C#; it is not counted here as a new benign regression or a newly closed valid negative.
- **Suite duration:** the enlarged suite genuinely executes the additional rounds; its runtime is material. My run took **520.0 seconds (8 min 40 s)** while the independent replay, Roslyn and e2e work also ran on four logical CPUs. The author’s approximate 1-to-5-minute comparison is plausible, but I did not rerun the old suite as a serial timing benchmark; that before/after measurement is **NOT RUN** here. No before/after performance equivalence is claimed.

## Reproduction and handoff boundary

The scripts and source hashes in `$E` make the review reproducible. The following commands run only in reviewer-owned storage; do not substitute an author or Unity lane for their scratch paths:

```bash
python3 -B "$E/independent.py"       # original reviewer definitions, 654 cases
python3 -B "$E/corpus305.py"         # original #305 definitions
python3 -B "$E/fidelity.py"          # original-to-repo/source-export identity
python3 -B "$E/new_variants.py"      # fresh adversarial cases
python3 -B "$E/gates.py"             # actual checker, SDK traces, old control
python3 -B "$E/compile.py"           # compiler-only validity checks
```

`e2e.py` creates a new scratch clone at a fixed unused path, so inspect it and supply a fresh reviewer-owned clone path before rerunning; do not overwrite the retained clone. Likewise preserve existing raw evidence before rerunning commands that write fixed log names. The exact candidate file-copy manifest is `$E/copied-shas.tsv` and `$E/copied-git-blobs.tsv`; final evidence hashes are `$E/SHA256SUMS.txt`.

**Manager action requested:** assess F-1/F-2 and issue the source-only ruling. This reviewer recommends **NEEDS CHANGES**, without changing code or prescribing acceptance. Integrated A–O, Unity, player review, protected save/reference qualification and any build permission remain **NOT RUN / NOT RELEASED by this review**. Dashboard ingestion/publication readback was not performed; synchronization is an outstanding operational handoff item, not a fabricated closure receipt.
