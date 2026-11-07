# #325 — bounded CTA companion/name rebinding repair

**AUTHOR HANDOFF — ready for independent #326 source review; not accepted or integrated.**

Worker: **T3 Code, Codex harness, gpt-6.1-sol, reasoningEffort high, OpenAI provider family; account/quota UNKNOWN**. Sole author; no delegated authors or independent review claimed.

Source branch: `verify/scenario-one-feedback-cta-anchor-20261106`.
Exact source branch HEAD at verification, and **tested commit: `b040a9929b693f9ad28bc8f9f935f002b91a38b8`** (implementation commits `1f2bb5e44aab96e78ac12b51784c6f1b7f585507` and `b040a9929b693f9ad28bc8f9f935f002b91a38b8`). The publication commit adds only this report; its exact final branch SHA is recorded in the #325 handoff comment, and is the Git commit containing this document. A document cannot contain its own commit hash. No tested Python/source file changes in that publication commit.

Binding authority: [Manager repair packet](https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Morning-Closure-20261107/MANAGER-CTA-321-COMPANION-NAME-REBINDING-REPAIR-PACKET-20261107.md) and the adjacent `MANAGER-322-CLOSURE-DISPOSITION-AND-NEXT-GATES-20261107.md`; latest user assignment/ruling governs the read-only reference copy-out exception below.

## Preflight and scope

Before writing, local HEAD and fresh HTTPS `git ls-remote origin refs/heads/verify/scenario-one-feedback-cta-anchor-20261106` both equalled **`3033dd12fed73136c8f94c82d58bf0d7cacd5380`**. `git status --porcelain=v1 -uall --ignored` was empty, and `git ls-files -v` showed no lowercase/skip-worktree entries. Branch, origin and bare common store `/media/jer/Files/CCF/Repository.git` matched the packet; no Git locks or competing lane writer were found. User assigns the sole author; live coordination/closed predecessor ownership was inspected. Worktree `/media/jer/Files/CCF/Operations/CCF-FeedbackCTARepair-20261106` is on `/dev/sdb4`, Files/ext4, initially 42 GiB available. Context snapshot last changed at `81867c8940d7d07c952e8e49ca4a0cb93498e790`; the binding packet overrides stale lane planning where applicable. No unrelated changes existed or were overwritten.

Exactly five changed paths relative to the packet base:

- `Tools/Verification/OvernightPlaytestFeedback/objective_cta_source_check.py`
- `Tools/Verification/OvernightPlaytestFeedback/test_objective_cta_source_check.py`
- `Tools/Verification/OvernightPlaytestFeedback/check_baseline.py`
- `Tools/Verification/OvernightPlaytestFeedback/README.md`
- `Docs/Handoffs/ScenarioOne-Morning-Closure-20261107/T3-CTA-321-COMPANION-NAME-REBINDING-REPAIR-RESULT.md`

Only the PASS diagnosis string of test 08 changes in `check_baseline.py`. All other checks and its FAIL path are unchanged. No production/game C#, Assets, Unity metadata, package, ProjectSettings, save, frozen Reference Future, PR #300 ancestry, integration/main or Windows build changes. No Editor, Unity lease, new Library, cleanup or source writes to any ZX20 lane. Existing verification evidence is read-only.

The user subsequently authorised **strictly read-only copy-out of exactly manifest-listed DLLs**, with mandatory hash agreement and no other reads/writes in ZX20 lanes. All 320 were copied to `/media/jer/Files/CCF/Verification/CTA325-CompanionNameRepair-20261107/refs/`; **320/320 SHA256 comparisons passed, zero mismatches**. All compiler references use those verified Files copies. No partial Player-build Managed substitute was used. The full per-file verification table follows below.

## Repair and bounded contract

F-1: `_root_reviewed_names()` now includes `_ROOT_HEADER`, including `MonoBehaviour`, and the remaining reviewed root fragments (screen enum, pinned bodies, declarations/statements, Start writes, review-year body and routing input). The existing root alias/type guard therefore rejects `using MonoBehaviour = ZHiddenBase;` before inherited nested `Time` can be treated as an unchanged Unity base. The real header pin remains in force. Unrelated aliases remain accepted.

F-2: `_guard_companion_rebinding()` audits type declarations in the **already collected** companions against that same reviewed-root-name inventory. It rejects `Time`, `MonoBehaviour` and the other derived reviewed names, rather than pinning an arbitrary list containing only the three witness spellings. Existing legitimate global companion declarations have narrow path/name exceptions; view partials already validated by `_scope` remain allowed. Exceptions are not inferred from a mutant's declarations. A file-local UiKit alias cannot change root lookup and is allowed. The collector is unchanged.

This uses the existing bounded structure walk, not a second parser or Roslyn framework. It conservatively rejects protected names even on nested/namespaced companion types where C# lookup might make them harmless. It does not resolve generic arity, namespaces, inheritance, external assemblies or arbitrary C# semantics; it does not pin the internals of legitimate companion definitions. The README explains these limits and the test-08 message stays explicitly lexical.

## Three repaired false greens and source fidelity

The source edits in `rebinding_cases()` were transcribed from read-only `CTA-309-TwoGuardReview-20261107/new_variants.py`. Every generated changed file was compared byte-for-byte with that reviewer's `new-mutants/` export; the generated source maps also equal the original mutation definitions. **3/3 cases, 5/5 files identical**. The originals were never rewritten.

| Case | Roslyn C#9 | Real full checker | Asserted rejection cause |
|---|---|---|---|
| R321-S05 | 0 errors | exit 1, 9/1/2; only FAIL 08 | `collected companion type 'Time' redefines a reviewed root identifier` (`F-2 name binding`) |
| R321-S06 | 0 errors | exit 1, 9/1/2; only FAIL 08 | `using alias 'MonoBehaviour' shadows a name the reviewed text reads (N-2)` |
| R321-S07 | 0 errors | exit 1, 9/1/2; only FAIL 08 | same MonoBehaviour alias/name-binding cause |

Each new negative checks its cause both in the in-memory suite and the real full checker; a crash, file-inventory failure, unrelated diagnosis or extra failed check does not satisfy the assertion. The hinted S06 companion is collected through the real collector.

| Case/file | SHA256 (original = generated) |
|---|---|
| R321-S05 / `Assets/ForestPrototype/UI/UiKit.cs` | `336c914a0079e6593a4ee57cfc634650cdf3bb04a072d51c2a862bf19a011c20` |
| R321-S06 / `Assets/ForestPrototype/UI/ZHiddenBase.cs` | `76e16e5fc28e949512fc8795bc8cea9737177e3dce2369fb003f8b66177970ce` |
| R321-S06 / `Assets/ForestPrototype/UI/ScenarioOneUiRoot.cs` | `9f17d9885de9479078c6b9cde0342e95e39469dd5c7e84b1b6ac403ab3e2dbc8` |
| R321-S07 / `Assets/ForestPrototype/UI/ScenarioOneUiRoot.cs` | `9f17d9885de9479078c6b9cde0342e95e39469dd5c7e84b1b6ac403ab3e2dbc8` |
| R321-S07 / `Assets/ForestPrototype/UI/UiKit.cs` | `b90f8d5dd6c7f0283b6690a73df7dffb9f735d267ffb94df5aa932ea0eabe021` |

## Bounded new controls

Five nearby negatives: alternate base alias target; escaped `@MonoBehaviour` alias with `global::` target; escaped global `@Time` in UiKit.cs; global `Time` in a hinted companion; global `MonoBehaviour` in UiKit.cs with inherited `Time`. **5/5 rejected for their name-binding cause**, in-memory and full-checker exit 1 (only check 08; 9/1/2). The global-base fixture supplies every Time member used by the source set; it was compiled, not assumed valid.

Five benign controls: unrelated root alias; unrelated UiKit class; UiKit-local `using Time = System.Int32`; unrelated hinted base/class; unrelated root class plus alias. **5/5 accepted**, in-memory and full-checker **10/0/2 exit 0**. All 13 new cases compile with zero errors. No unbounded new mutation hunt was performed.

## Preserved gates and actual exits

All rows below were run in this author turn, unless explicitly labelled historical evidence. The final suite uses the tested source above; the older intermediate successful runs were not substituted for it.

| Gate | Result | Exit |
|---|---|---|
| Full repository mutation suite | **884/884 prior negatives**, false greens 0, wrong cause 0; new 3 repaired + 5 nearby reported separately | **0** |
| Prior benign controls | **112/112** = 32 original/#305/#309 + 32 loop-1 + 48 #320; new 5/5 separate | suite 0 |
| Original author controls | 20 original + 14 F1 + 21 F2/F3, all rejected | suite 0 |
| #305 original corpus | **81/81** X-F1/X-F2 rejected; original known **50/50** rejected; 21/21 re-derived R IDs rejected; 13/13 original benign accepted | fidelity/corpus driver 0; suite 0 |
| #309 regressions | RR-1 28/28, RR-2 15/15, RR-3 14/14, N-4 5/5 | suite 0 |
| Loop-1 reviewer negatives | S-1 10/10, S-2 14/14 (including 3 lone-CR), S-3 4/4, L-06 2/2 | suite 0 |
| Loop-1 added negatives | S-1 16/16, S-2 5/5, S-3 2/2, L-06 3/3 | suite 0 |
| #314 complete corpus/source fidelity | **654/654 definitions**, zero changed edits/definitions; the same five previously documented reclassifications retained; required **57 cases / 59 files** byte-identical to original review exports | fidelity/corpus driver 0 |
| Previous N-1/N-2 closure | **47/47 + 10/10** cause assertions in suite and **57/57 real full-checker runs**, each exit 1, 9/1/2, only FAIL 08 for its intended cause | driver 0; each checker 1 |
| #320 additional author negatives | N-1 21/21, N-2 12/12 | suite 0 |
| Prior limitation/over-rejection probes | **9+9** unchanged and **52** newer probes unchanged; no invalid C# relabelled valid | suite 0 |
| New witness/nearby/benign set | 3/3 repaired + 5/5 nearby negatives, 5/5 benign; union 892 negatives / 117 benign | suite 0; e2e driver 0 |
| Python compile | all three Python files | **0** |
| Candidate normal full checker | **10 PASS / 0 FAIL / 2 NOT-TESTABLE** | **0** |
| Historical `72b4efbd1c653fa2231f632a79030a83ff76f833` with current checker | **6 PASS / 4 FAIL / 2 NOT-TESTABLE**, intentional red | **1** |
| `baseline-output.txt` | byte-identical to packet base; blob `db6a8b850dce7efadaac6fe1b87b1dbba469428f`; SHA256 `513cddd9babddaede82212f8e4891ecfcf9823c13a69474a3665f890a7f05dc1` | identity assertions 0 |
| Roslyn compiler/tool build | original reviewer tool/source, C#9, same Player-like defines, exact 320 hash-verified references; **baseline 189 files + 13 cases all errors=0** | build **0**, compiler **0** |

The 654 prior cases include the same 576 counted negatives, 35 benign and reported probes; they are not added to 884 a second time. Original known-50 checks use the reviewer's explicit ID list; no claim is made that every historical exploratory source is compilable. O-15 remains explicitly invalid C#. The old malformed/unsupported controls remain unchanged and are not silently skipped.

## SDK8 trace evidence

The checker inherits its cwd for `dotnet run`; `TMPDIR` and the Python script location do not select the SDK. Pinned cwd: `/media/jer/Files/CCF/Operations/CTA325-Proof-20261107/sdk8/cwd/deeper`; `/media/jer/Files/CCF/Operations/CTA325-Proof-20261107/sdk8/global.json` specifies **8.0.133, rollForward disable**.

- `candidate.hostfxr`: found that global.json, resolved `8.0.133`, used `/usr/lib/dotnet/sdk/8.0.133/dotnet.dll`.
- `candidate.strace`: **278** successful SDK8 opens, **0** SDK10 opens; actual Roslyn/csc DLLs opened under SDK8.
- Unpinned `control-cwd`: hostfxr resolves latest to **10.0.114**; strace **500** successful SDK10 opens, **0** SDK8 opens. Both full checks exit 0 / 10/0/2.
- **Six distinct production `RackEditing.SnapFar` cases PASS**: near-angle, near-angle-disabled, outside-angle-window, near-boundary, near-boundary-disabled, outside-boundary-window. These are isolated .NET math calls, not physical input/Unity tests.

Raw trace/hostfxr/stdout/stderr/exit files and `sdk-summary.json` are retained under `/media/jer/Files/CCF/Operations/CTA325-Proof-20261107`.

## Exact commands and reproduction

Durable offline proof driver: `/media/jer/Files/CCF/Operations/CTA325-Proof-20261107/proof.py`; reference copy driver: `/media/jer/Files/CCF/Operations/CTA325-Proof-20261107/copy_refs.py`. Commands were launched with `PYTHONDONTWRITEBYTECODE=1`, `PYTHONPYCACHEPREFIX=/media/jer/Files/CCF/Operations/CTA325-Proof-20261107/pycache`, `TMPDIR=/media/jer/Files/CCF/Operations/CTA325-Proof-20261107/tmp`, `UseSharedCompilation=false`, `MSBUILDDISABLENODEREUSE=1`, telemetry disabled and no Unity process. Every child command/cwd/exit is retained in `commands-*.json`; individual stdout/stderr/exit files are separate. The proof driver uses sparse disposable shared clones for full-checker mutations and restores each source overlay afterwards. It never changes the source lane's Assets.

```bash
python3 -B /media/jer/Files/CCF/Operations/CTA325-Proof-20261107/proof.py prepare
python3 -B /media/jer/Files/CCF/Operations/CTA325-Proof-20261107/copy_refs.py
python3 -B /media/jer/Files/CCF/Operations/CTA325-Proof-20261107/proof.py suite
python3 -B /media/jer/Files/CCF/Operations/CTA325-Proof-20261107/proof.py compiler
python3 -B /media/jer/Files/CCF/Operations/CTA325-Proof-20261107/proof.py gates
python3 -B /media/jer/Files/CCF/Operations/CTA325-Proof-20261107/proof.py e2e
python3 -B /media/jer/Files/CCF/Operations/CTA325-Proof-20261107/proof.py prior-e2e
python3 -B /media/jer/Files/CCF/Operations/CTA325-Proof-20261107/proof.py updated-e2e
python3 -B /media/jer/Files/CCF/Operations/CTA325-Proof-20261107/proof.py audit
```

All driver commands exit **0**. `gates` intentionally expects historical checker exit **1**; `e2e`/`prior-e2e` intentionally expect each negative checker exit **1**, with completed tallies and the specific cause. The implementation-level commands they execute are:

```bash
python3 -B /media/jer/Files/CCF/Operations/CCF-FeedbackCTARepair-20261106/Tools/Verification/OvernightPlaytestFeedback/test_objective_cta_source_check.py
python3 -m py_compile /media/jer/Files/CCF/Operations/CCF-FeedbackCTARepair-20261106/Tools/Verification/OvernightPlaytestFeedback/check_baseline.py /media/jer/Files/CCF/Operations/CCF-FeedbackCTARepair-20261106/Tools/Verification/OvernightPlaytestFeedback/objective_cta_source_check.py /media/jer/Files/CCF/Operations/CCF-FeedbackCTARepair-20261106/Tools/Verification/OvernightPlaytestFeedback/test_objective_cta_source_check.py
# cwd = /media/jer/Files/CCF/Operations/CTA325-Proof-20261107/sdk8/cwd/deeper
strace -f -qq -e trace=execve,openat -e signal=none -o /media/jer/Files/CCF/Operations/CTA325-Proof-20261107/candidate.strace python3 -B /media/jer/Files/CCF/Operations/CCF-FeedbackCTARepair-20261106/Tools/Verification/OvernightPlaytestFeedback/check_baseline.py --rotation-state post-fix --instruction-state post-fix
# COREHOST_TRACE=1 COREHOST_TRACE_VERBOSITY=4 COREHOST_TRACEFILE=/media/jer/Files/CCF/Operations/CTA325-Proof-20261107/candidate.hostfxr
# Same traced command from /media/jer/Files/CCF/Operations/CTA325-Proof-20261107/control-cwd, output names control.*
python3 -B /media/jer/Files/CCF/Operations/CTA325-Proof-20261107/historical/Tools/Verification/OvernightPlaytestFeedback/check_baseline.py --rotation-state pre-fix --instruction-state pre-fix
dotnet build /media/jer/Files/CCF/Operations/CTA325-Proof-20261107/roslyn-tool/RoslynCheck.csproj -o /media/jer/Files/CCF/Operations/CTA325-Proof-20261107/roslyn-tool/bin --verbosity quiet
dotnet /media/jer/Files/CCF/Operations/CTA325-Proof-20261107/roslyn-tool/bin/RoslynCheck.dll /media/jer/Files/CCF/Operations/CCF-FeedbackCTARepair-20261106 /media/jer/Files/CCF/Operations/CTA325-Proof-20261107/mutants /media/jer/Files/CCF/Operations/CTA325-Proof-20261107/roslyn-tool/refs.txt 'UNITY_6000_6_OR_NEWER;UNITY_6000_0_OR_NEWER;UNITY_2023_1_OR_NEWER;UNITY_2022_1_OR_NEWER;UNITY_2021_1_OR_NEWER;UNITY_2020_1_OR_NEWER;ENABLE_INPUT_SYSTEM;UNITY_STANDALONE;UNITY_STANDALONE_LINUX' /media/jer/Files/CCF/Operations/CTA325-Proof-20261107/roslyn-results.txt
```

For each full-checker mutation the actual command is `python3 -B /media/jer/Files/CCF/Operations/CTA325-Proof-20261107/candidate/Tools/Verification/OvernightPlaytestFeedback/check_baseline.py --rotation-state post-fix --instruction-state post-fix` from the pinned SDK8 cwd. Raw per-case commands/exits are in `commands-e2e.json`, `commands-updated-e2e.json` (the corrected global-base fixture only) and `commands-prior-e2e.json`. Final restore/production identity assertions and `git diff --check` also exit 0.

The initial fixture import-anchor and historical clone-directory setup errors were corrected before final gates. One attempted scratch cleanliness assertion was made while a mutation overlay was active and therefore failed; the final post-restoration audit is the relevant clean-state gate. No failing mutation, benign control or probe was removed/skipped to obtain green.

## Honest limits and next gate

No **new unrelated valid C# semantic loophole** was discovered during this bounded author work. Existing README limits remain: uncollected source, reflection, unpinned root callers/members, game/teaching state, object lifetime, styles, exceptions/nontermination, and general C# semantics. The guard's canonical-path carve-outs do not verify those types' contents. Related benign aliases that use a protected reviewed root name may still be rejected, even if a semantic compiler would find them equivalent.

**STOP recommendation to Manager #200 if another distinct valid semantic false green is found:** preserve a minimal witness and compiler/full-checker receipts, label the limitation, then decide explicitly between Roslyn semantic analysis and accepting a documented bounded risk. Do not start another parser loop or silently accept a known in-scope defect.

No Unity runtime, gameplay, rendering, input, persistence, production qualification, integration, acceptance or Windows build claim is made. #326 must independently review the exact published source; Manager acceptance/main integration remain separate gates. Manager-only `active-tasks.md` and downstream dashboard/mirror reconciliation were not edited or verified here and remain an operational handoff item.

## Manifest copy verification table

All rows: source SHA256 = copied-file SHA256 = reviewer's `roslyn-reference-shas.tsv`; **PASS**. Original paths below are relative to `/media/jer/ZX20/`. Copy filenames are relative to `/media/jer/Files/CCF/Verification/CTA325-CompanionNameRepair-20261107/refs/`. No other ZX20 lane file was read for this proof. The compiler receives these 320 Files paths in manifest order.

<details>
<summary>320 exact verified reference copies (34 ScriptAssemblies + 286 Unity refs)</summary>

| # | Original manifest path | Files copy | SHA256 | Result |
|---|---|---|---|---|
| 000 | `Unity/6000.6.0f1/Editor/Data/NetStandard/ref/2.1.0/netstandard.dll` | `000-netstandard.dll` | `79a49049c360a3ff282e747c311fcd567834efde564a96689872564164756e84` | PASS |
| 001 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Xml.XmlDocument.dll` | `001-System.Xml.XmlDocument.dll` | `e683ff6c95f117954a616d304f3b89cb273e8b00b42efef7fe99a48713800645` | PASS |
| 002 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Text.RegularExpressions.dll` | `002-System.Text.RegularExpressions.dll` | `d8839d80664bcaffafada263aded6bda805885fc8c1af0a38eb784f8786a3d2e` | PASS |
| 003 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.ComponentModel.Primitives.dll` | `003-System.ComponentModel.Primitives.dll` | `6522dfefdddfbf58ccc4f3ba6f41121e16ee5b20dd8f325c6983b4ac8c3befd8` | PASS |
| 004 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.IO.dll` | `004-System.IO.dll` | `855c8b76c1dd99ecd97f079b073eff83292b1ff90b9c5426cbe36b76c39a34d1` | PASS |
| 005 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Drawing.Primitives.dll` | `005-System.Drawing.Primitives.dll` | `da2afcd4237dc4f8e14f66bd9d2983143881e97c3f3c1738620438df94d030e4` | PASS |
| 006 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Threading.ThreadPool.dll` | `006-System.Threading.ThreadPool.dll` | `cdb2e88636ec5e827305d93e008e69cd205675fed0e19115a03288624769e853` | PASS |
| 007 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Net.Http.dll` | `007-System.Net.Http.dll` | `bba894cb713ef6a2f48776240ee55122c38839af88c72742294b4583f8d2a66e` | PASS |
| 008 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Security.Cryptography.Primitives.dll` | `008-System.Security.Cryptography.Primitives.dll` | `4ce2892fa454e46f7d99411caab18db1595bb3650f8fcbe303851731d889f4cd` | PASS |
| 009 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Threading.dll` | `009-System.Threading.dll` | `23c49cba7193aa07fea9c840254433461601ad3b7e9d77d97c889a8b671dee62` | PASS |
| 010 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.ComponentModel.EventBasedAsync.dll` | `010-System.ComponentModel.EventBasedAsync.dll` | `04a7ea907580d9d41ca1d8d67abfe81d61af8fc59125e1383a9502bfd4f9134f` | PASS |
| 011 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Runtime.Numerics.dll` | `011-System.Runtime.Numerics.dll` | `e0727d00a0dda8280716df9a2019d4e53cf3da0b6339b1ad4d407ba2dd3396fd` | PASS |
| 012 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Reflection.Emit.ILGeneration.dll` | `012-System.Reflection.Emit.ILGeneration.dll` | `125c6aaab34dec8c7799fc99074ab01a42f67e549187f50cad9da2184bd35d32` | PASS |
| 013 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Security.SecureString.dll` | `013-System.Security.SecureString.dll` | `fd85a43436a9ed3667df457c6a92721052c29405eba2232b867d3797ca72cb8b` | PASS |
| 014 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/Microsoft.Win32.Primitives.dll` | `014-Microsoft.Win32.Primitives.dll` | `2522cb85ef5379a4f716a035e11549b6814f4d426e758649250738358a71c85f` | PASS |
| 015 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Xml.XDocument.dll` | `015-System.Xml.XDocument.dll` | `729c42931e8812e1a993727a0f32c832ebfc3abac2223a5a30c2a8ac1cc9964d` | PASS |
| 016 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.IO.Compression.dll` | `016-System.IO.Compression.dll` | `dd36080142c15c7cf005e78e3237cd85a6d9e241b979072c0b82e0a40f6848b3` | PASS |
| 017 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Collections.Specialized.dll` | `017-System.Collections.Specialized.dll` | `9572743bf2fa4fd7a5c5e8d2c8a9b44424f38ff67042c833789282dbe241cb88` | PASS |
| 018 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Reflection.Emit.Lightweight.dll` | `018-System.Reflection.Emit.Lightweight.dll` | `5e24ed2d22416fca721f54243e4cbc693e6f293904660947067ba1c0288e79fe` | PASS |
| 019 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Runtime.Serialization.Formatters.dll` | `019-System.Runtime.Serialization.Formatters.dll` | `54c2621c4476473fe95be77a8c91a153c63d6d0cf4912683f6dbc98aae0db85a` | PASS |
| 020 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Resources.Writer.dll` | `020-System.Resources.Writer.dll` | `49e3aaa0b5e6d87c3f1d038877063582811f78ae4fb6d6a8f2cb8920f3782d26` | PASS |
| 021 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Security.Cryptography.X509Certificates.dll` | `021-System.Security.Cryptography.X509Certificates.dll` | `af66cdf9b9e2ee26174a56aae0bee557edd9a7a61b27a4b99509b338cf68c4b4` | PASS |
| 022 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Reflection.Primitives.dll` | `022-System.Reflection.Primitives.dll` | `ebfd6f084c8741e359358711ae6da4b0a06db2369be8c9defb4d6eb17bc8ee14` | PASS |
| 023 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Net.WebSockets.dll` | `023-System.Net.WebSockets.dll` | `538827c5993417c9aba603171200b0f6f096f399e6e7178cf2028735f1b909d7` | PASS |
| 024 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Threading.Tasks.dll` | `024-System.Threading.Tasks.dll` | `30984a07e22928be7fbea6fc3518ef0171537652402d5622497f8dd5afcb16f4` | PASS |
| 025 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Diagnostics.Contracts.dll` | `025-System.Diagnostics.Contracts.dll` | `250ba880b970a355cf626e7ad60f736aab379c7ba06f68ffdebb6f2b852c25cb` | PASS |
| 026 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Runtime.CompilerServices.VisualC.dll` | `026-System.Runtime.CompilerServices.VisualC.dll` | `a75c6fa50f10ad888663ab694ca2cb45b8379070065e9d558d4c19086be9f28b` | PASS |
| 027 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Buffers.dll` | `027-System.Buffers.dll` | `d0648fc3d820c8b225fe991d55d506f3210b698cc02beaf8aa31d990cd990772` | PASS |
| 028 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Threading.Thread.dll` | `028-System.Threading.Thread.dll` | `6822f2724465e7c973ae1f2d5a2e7a5755a816b6847498a855fe9c8d0ed69a0b` | PASS |
| 029 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.ComponentModel.TypeConverter.dll` | `029-System.ComponentModel.TypeConverter.dll` | `399642afed43a9c150650eeb37cd407c2d14ab005e58d6f2784b726e0be72f53` | PASS |
| 030 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Globalization.dll` | `030-System.Globalization.dll` | `a8bb8be1079c4220b202bc873b7ffe39a2e306cd5280843ad5c1aed8376db2d3` | PASS |
| 031 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Runtime.Serialization.Json.dll` | `031-System.Runtime.Serialization.Json.dll` | `8c50999e83e5ef46a479498e2010425283b04eb674cf6e9a691f6e1ea39e3b47` | PASS |
| 032 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.IO.FileSystem.Watcher.dll` | `032-System.IO.FileSystem.Watcher.dll` | `c75a0339b10aedcddff7c3acc85f23bfc95c63244abc7edbaec8606b426a6556` | PASS |
| 033 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Xml.ReaderWriter.dll` | `033-System.Xml.ReaderWriter.dll` | `ae8db24c60c075fc0cd7c30e9aecc7f7de52fa05048d1d09e3ee35653f4c54d5` | PASS |
| 034 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Collections.Concurrent.dll` | `034-System.Collections.Concurrent.dll` | `c882ff60ae9e26a23d149a11aa8bcc546652e1bd2b58656a7c3b11a8f5e227a7` | PASS |
| 035 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Security.Cryptography.Encoding.dll` | `035-System.Security.Cryptography.Encoding.dll` | `2327c771b6f00d194816c7e142ba054b8ee120018295c2d61289dfbf822fbbf7` | PASS |
| 036 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.IO.FileSystem.DriveInfo.dll` | `036-System.IO.FileSystem.DriveInfo.dll` | `25e4ed8748fdbd9cf2fd77014d087ed9ed24dbde2e694e966a68a9d72371f51d` | PASS |
| 037 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Globalization.Extensions.dll` | `037-System.Globalization.Extensions.dll` | `744c044a966cde69a0362d7f7eadba7f4b89e28c95cb47ab6022740d381969c0` | PASS |
| 038 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Reflection.DispatchProxy.dll` | `038-System.Reflection.DispatchProxy.dll` | `69833afb991f90cf22590b9de9aa95ac3aae9bed2b186321ff73fa5790efe08f` | PASS |
| 039 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Net.Sockets.dll` | `039-System.Net.Sockets.dll` | `cb5996625a8ae3b82a2e06d8b27b00e6a4ad480b6dde96492e1a245583275d68` | PASS |
| 040 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Net.NameResolution.dll` | `040-System.Net.NameResolution.dll` | `2308c9e1f674c3b854320507d41ba633802c8db7d10ec8dbb3ef9d86acca44fe` | PASS |
| 041 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Runtime.InteropServices.dll` | `041-System.Runtime.InteropServices.dll` | `7e97622025bcbda146408eecbb750345bd373ab9454fb118f11377f215f56400` | PASS |
| 042 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Resources.ResourceManager.dll` | `042-System.Resources.ResourceManager.dll` | `99ed45baff87bd6723ef8dac8d04d21075627141f7525a1ace95ba1dcba9a964` | PASS |
| 043 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Runtime.Serialization.Primitives.dll` | `043-System.Runtime.Serialization.Primitives.dll` | `f35293ebdd0b318f02341da4bc7bb236cf0c8e14e1c7ec8e3cba9cc4db2d2aee` | PASS |
| 044 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Diagnostics.Tools.dll` | `044-System.Diagnostics.Tools.dll` | `25d1dec703daa1e68dea27c6f85f4a0c55d24a56d1c597388c6c596e0ff9f856` | PASS |
| 045 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Globalization.Calendars.dll` | `045-System.Globalization.Calendars.dll` | `03d14391a91d831f15050cd6255cd1097f546760b46f9fd2c0479069735d264b` | PASS |
| 046 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Collections.dll` | `046-System.Collections.dll` | `969da19309c9d0aa44d4ab9c03ad9f85a614fe0c285eaa68ee0bb5dc4dc6e47a` | PASS |
| 047 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.IO.IsolatedStorage.dll` | `047-System.IO.IsolatedStorage.dll` | `7b514b9dfd4ad5b514b54aa9a12f4a9cd02de97a724b21ecaefdd97c2f94b31f` | PASS |
| 048 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Net.Primitives.dll` | `048-System.Net.Primitives.dll` | `0a3b1ad27d640a56a55ca750861768271edd211ca1c494ae36b5184ab8e26780` | PASS |
| 049 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Threading.Timer.dll` | `049-System.Threading.Timer.dll` | `e9aa691cef3ec721b26e7d0efc1a960ec7fc5661eb1921e06a0ff0afb52f41b9` | PASS |
| 050 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Runtime.Extensions.dll` | `050-System.Runtime.Extensions.dll` | `3cb31bf3acb245a0e409b2e6160e8dfbbe7c26fbd9eeabcc8bf2bb81535cbab9` | PASS |
| 051 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Net.Ping.dll` | `051-System.Net.Ping.dll` | `a17b78f469c6622970d1eb6ee5105220235c70d2908c856de6742797c7d82c32` | PASS |
| 052 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Threading.Tasks.Extensions.dll` | `052-System.Threading.Tasks.Extensions.dll` | `a159ada6a533b86727178129915d53c9c7554b0e55e117fce27b8661f8366f62` | PASS |
| 053 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.IO.MemoryMappedFiles.dll` | `053-System.IO.MemoryMappedFiles.dll` | `a7bcea398169e5f8b7bf01ce75d23065590db0d1a62e67df6a5b0894236e46b1` | PASS |
| 054 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Linq.dll` | `054-System.Linq.dll` | `0f94f7b2e726b32509e628a8514c6ce5ae903d07c99f99a61933289f13756ca4` | PASS |
| 055 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Net.NetworkInformation.dll` | `055-System.Net.NetworkInformation.dll` | `8fb1051323d2fc34c59f8aeaa3f17cd37907eeaf7d0c274323b12b14aa7027e9` | PASS |
| 056 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Dynamic.Runtime.dll` | `056-System.Dynamic.Runtime.dll` | `f34cb31b138244d902cc4220500a60a7ffe6e5cf0874f48678145aa2f42318ea` | PASS |
| 057 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Threading.Overlapped.dll` | `057-System.Threading.Overlapped.dll` | `fb48baf7cb0693c44e907b89a8c792cb4b90de258cd9735bef67d9b4dd85e2f1` | PASS |
| 058 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.ComponentModel.dll` | `058-System.ComponentModel.dll` | `a488ab63c97df6d45f8dc01ecdd56c4ece383fc88231869caca1a74c29ee1c12` | PASS |
| 059 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Security.Principal.dll` | `059-System.Security.Principal.dll` | `f7edd00a5f69766356638b9c8692fa7d8ad2465239c5aeb14d9f071f3832a64b` | PASS |
| 060 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Diagnostics.FileVersionInfo.dll` | `060-System.Diagnostics.FileVersionInfo.dll` | `3f6ce9c7991ab252c5c704ff91e1c99d21b5fe99937d27132b3b56f8b981a045` | PASS |
| 061 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Data.Common.dll` | `061-System.Data.Common.dll` | `cfbbe345a3af5c2820492779ce4c9e1d9c7e4c9e714f222a15ab524cf827af52` | PASS |
| 062 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Xml.XmlSerializer.dll` | `062-System.Xml.XmlSerializer.dll` | `6650b822e73bc40af1f242a6d96c2f25397189b8fa778d22ac6a8c5fa2ea58f7` | PASS |
| 063 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Memory.dll` | `063-System.Memory.dll` | `34abf197fbf191fb83e983571a6f3a6fc460fa80f32038c94d54e96966678f88` | PASS |
| 064 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Threading.Tasks.Parallel.dll` | `064-System.Threading.Tasks.Parallel.dll` | `69efa79ae26cc823964170b3a23f69c8c2346206f3c2670d8d07f60cd4c19a94` | PASS |
| 065 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Security.Cryptography.Algorithms.dll` | `065-System.Security.Cryptography.Algorithms.dll` | `39bd261f8befb2334ab3e720c312ead1d584408c29717f8b439371491d0946ae` | PASS |
| 066 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Runtime.dll` | `066-System.Runtime.dll` | `81be9e3c2938390b15445969128e65e5ff14b80193fc531bce4e9e28a368273a` | PASS |
| 067 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Security.Cryptography.Csp.dll` | `067-System.Security.Cryptography.Csp.dll` | `30f289b46935daa1770373812ac97fe2a3d14806051ba28f900cd305967a0ef9` | PASS |
| 068 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Net.WebHeaderCollection.dll` | `068-System.Net.WebHeaderCollection.dll` | `edfc6c63a5f6374134bc9b25349978acbc4c2b02ae7de2fe7935e898044e6ff7` | PASS |
| 069 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Reflection.Emit.dll` | `069-System.Reflection.Emit.dll` | `b42bfcd22d815a9ab3d2b2e8a425ecc91e50a6e2aa7e3c5cfa2d5b9b9e127c92` | PASS |
| 070 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Diagnostics.Tracing.dll` | `070-System.Diagnostics.Tracing.dll` | `469d5ab04348ab7db62f71d757586e46692695b5c4604a26a01899baceb4b47d` | PASS |
| 071 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Reflection.dll` | `071-System.Reflection.dll` | `00d69622de91ce2e2aeee6c1215debaa721a120d8f59b08e7f4a70796ecf9b3f` | PASS |
| 072 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Diagnostics.Debug.dll` | `072-System.Diagnostics.Debug.dll` | `3964a2da6b23e89d076faf6a91cf8b2fe6d196d4dea2f954e0c5e1f570ef85b1` | PASS |
| 073 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.ValueTuple.dll` | `073-System.ValueTuple.dll` | `1b0ee8e45405c0b4917327b1e44454ffffc63750b7df479a7f6ffbda2a1f92a2` | PASS |
| 074 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Security.Claims.dll` | `074-System.Security.Claims.dll` | `ea04a2840cede1e5ee2281442540f5df410c562cf5943277ab938abf30b386e8` | PASS |
| 075 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.AppContext.dll` | `075-System.AppContext.dll` | `643ec79d998df16995c4270bf2dc8ba881796179c03fa55d343ee9c43ba074d7` | PASS |
| 076 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.ObjectModel.dll` | `076-System.ObjectModel.dll` | `a5d377dbf15d2abe6b00a65ae81766b237354738b275c7a0e2d9466f093cbc20` | PASS |
| 077 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Linq.Parallel.dll` | `077-System.Linq.Parallel.dll` | `f6d2eba7f641721e3a41ee29000430659f0f987ec1b33fc467f715c9d186e58c` | PASS |
| 078 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.IO.Compression.ZipFile.dll` | `078-System.IO.Compression.ZipFile.dll` | `f721f7c90e0e1a8b04a62ce570eb7f7c238db9136ac213a6c04970e94a737e82` | PASS |
| 079 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Diagnostics.TextWriterTraceListener.dll` | `079-System.Diagnostics.TextWriterTraceListener.dll` | `d85c7a6c1d9e848b69aa5a8ffb4a9755e8d84fc957ae9c455ead30fd8b688869` | PASS |
| 080 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Xml.XPath.dll` | `080-System.Xml.XPath.dll` | `ddcb1eb78a567a323b47dc18a9dd63c066000e3cdbb90a4559607dae69ab4be0` | PASS |
| 081 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Linq.Queryable.dll` | `081-System.Linq.Queryable.dll` | `6c5b11651156aedfbacacadfbe4a388062cbae74a2a43004b590990a0b941a26` | PASS |
| 082 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Runtime.Handles.dll` | `082-System.Runtime.Handles.dll` | `aa5fc7ca286d170bfe32dd15663d508bb94e297a52797984a9aa7d8ffddf5ec5` | PASS |
| 083 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Runtime.InteropServices.RuntimeInformation.dll` | `083-System.Runtime.InteropServices.RuntimeInformation.dll` | `1ae138913f54d55cea0f40807ee363fdd1ceaf2037f4a029c1d2aca445146183` | PASS |
| 084 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.IO.FileSystem.Primitives.dll` | `084-System.IO.FileSystem.Primitives.dll` | `4e96df37c5713fb21e30c767aa216e1f10878f30e4a89c6d7657b86637a62647` | PASS |
| 085 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Net.Requests.dll` | `085-System.Net.Requests.dll` | `5f2a63d0ec2fd4ccc45ca2ba3dcfd7a4cf72931e97fd41bd36f00ca2f18dab06` | PASS |
| 086 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Text.Encoding.Extensions.dll` | `086-System.Text.Encoding.Extensions.dll` | `7d88331223deb5e7226b56fe02a2967c8770b7120d3f23a043192a2396d207d3` | PASS |
| 087 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.IO.FileSystem.dll` | `087-System.IO.FileSystem.dll` | `3e6b283d0ad07f2d3e33eb1588e0279f52e684d638fe3dcfa6441deefe799c13` | PASS |
| 088 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Collections.NonGeneric.dll` | `088-System.Collections.NonGeneric.dll` | `17221b8767355654f5c97b53a4fd8dd413f95e2faf36dd2d28ea863ca7536f4a` | PASS |
| 089 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Diagnostics.StackTrace.dll` | `089-System.Diagnostics.StackTrace.dll` | `2b91c8a72755071086c0fe4ab8019480eeabf9ae5243cbf58942761abd1013a5` | PASS |
| 090 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Diagnostics.Process.dll` | `090-System.Diagnostics.Process.dll` | `29f7e89a609dcbcec72a4adf094a458718b23940721cebae0440ad7a0f522921` | PASS |
| 091 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Runtime.Serialization.Xml.dll` | `091-System.Runtime.Serialization.Xml.dll` | `baac69344e578e064dcc277d93bbe1082a72e5d0c21f04b137a3c343c75ef405` | PASS |
| 092 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Xml.XPath.XDocument.dll` | `092-System.Xml.XPath.XDocument.dll` | `857621dc44cff6ac7388d828a5a083d99f149901f2bec74993d87ef3cd541b35` | PASS |
| 093 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Diagnostics.TraceSource.dll` | `093-System.Diagnostics.TraceSource.dll` | `10ae31d5bdd373bbca96dc83cad65a32d1e788d8f95fb1555b371026025b2643` | PASS |
| 094 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Resources.Reader.dll` | `094-System.Resources.Reader.dll` | `f73fa653ba6ceb7e7c1d5eeb82d09c77d4101424422ec74f7313a7845cc882c6` | PASS |
| 095 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.IO.UnmanagedMemoryStream.dll` | `095-System.IO.UnmanagedMemoryStream.dll` | `52df0618cf4bc35ac97150c1a999220b5e6107b7272042cdd49e2adbc41e8adf` | PASS |
| 096 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Linq.Expressions.dll` | `096-System.Linq.Expressions.dll` | `1a9d4766cc13b2e27d6295d707c3af5d57f11d63f4cb4a9ffd47d9917edacd7c` | PASS |
| 097 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Text.Encoding.dll` | `097-System.Text.Encoding.dll` | `968665d2b5b1d7a2786b8e35d9df599ac40d45aedc86b3a2ea21ad35bf02f071` | PASS |
| 098 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Net.Security.dll` | `098-System.Net.Security.dll` | `ca922585b98ea8bc9b2c684dc11cf0a0946211cecec6b3f59c9c9b784a41ba56` | PASS |
| 099 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Numerics.Vectors.dll` | `099-System.Numerics.Vectors.dll` | `4c6ba8896ea9d26ff732d5afaccb3b13ffa4097799b10b3260bf7f64bc8b09c1` | PASS |
| 100 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Net.WebSockets.Client.dll` | `100-System.Net.WebSockets.Client.dll` | `c212a3431d18b2c678daeb0d1c0a839ea0eb02fb2029ecda32b09e00aed00aa5` | PASS |
| 101 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.IO.Pipes.dll` | `101-System.IO.Pipes.dll` | `c988c381f5ed6e2e8ff84a85f71e859f47c19ac149c8ee8230dc3c315100cad4` | PASS |
| 102 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Console.dll` | `102-System.Console.dll` | `98b2c46fda1c10418cddd36e3950058a7d188a48c482074ec77c3daca1dbad04` | PASS |
| 103 | `Unity/6000.6.0f1/Editor/Data/NetStandard/compat/2.1.0/shims/netstandard/System.Reflection.Extensions.dll` | `103-System.Reflection.Extensions.dll` | `59ec997a2dd047441fbf886691dffe2fe9eff0e549dd5d6241400aa2241c7c23` | PASS |
| 104 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/AsmDefToCSProjLib.dll` | `104-AsmDefToCSProjLib.dll` | `51d355bb5fa5a2f3849d17bdff37eabf4ef05e1a65d9f2fa9c9b01e6c173abe2` | PASS |
| 105 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/Bee.BeeDriver2.dll` | `105-Bee.BeeDriver2.dll` | `12c6fd6562a0fbba97a2bdf4b472e77e0c8d9182de856321dcc51432b1c2fd72` | PASS |
| 106 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/Bee.BinLog.dll` | `106-Bee.BinLog.dll` | `75df95272ade09465150bc70c5cdc440ed024682eca71b6e19d1d847a492c973` | PASS |
| 107 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/BeeBuildProgramCommon.Data.dll` | `107-BeeBuildProgramCommon.Data.dll` | `aedac1a4a2bb0327d3a01ffb089d767ae744b940ec92f9053d1b20dd686295f3` | PASS |
| 108 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/Bee.TinyProfiler2.dll` | `108-Bee.TinyProfiler2.dll` | `f7db7c18c47cc3d7c117e929e30372a97e4ed95c642449b9034acc5077ef75b2` | PASS |
| 109 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/ExCSS.Unity.dll` | `109-ExCSS.Unity.dll` | `07860534a9a2a91f273d96e8491ddfa065da699b5539423df8eaa1bf5307036d` | PASS |
| 110 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/Google.Protobuf.dll` | `110-Google.Protobuf.dll` | `5502aab0aac10f558e659dde5eacf8abcc24ad4b937a1c157b2b3c83d50f0514` | PASS |
| 111 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/Grpc.Core.Api.dll` | `111-Grpc.Core.Api.dll` | `0c4e14b08b418a0fda47282cbf36607c6f72ff0ebbb986eeff5b2fd9e2408d55` | PASS |
| 112 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/Grpc.Net.Client.dll` | `112-Grpc.Net.Client.dll` | `4327477a190afaaf4da8db8814f518fa1684f217aceedaa206bedd72ef8a85df` | PASS |
| 113 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/Grpc.Net.Common.dll` | `113-Grpc.Net.Common.dll` | `c277f136c53efd1fc0ec5acc8d8aa8667f124f728bb9a2f299ad00c0b931d8f2` | PASS |
| 114 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/Newtonsoft.Json.dll` | `114-Newtonsoft.Json.dll` | `8c1dd5c184b4e2e7ead06971ff3ebcb46783be972292d1deb1061744369b4d80` | PASS |
| 115 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/PlayerBuildProgramLibrary.Data.dll` | `115-PlayerBuildProgramLibrary.Data.dll` | `a3ee04c6930acf2230455b6a4d789fa50c14e5edca194990d44077185bb52263` | PASS |
| 116 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/ScriptCompilationBuildProgram.Data.dll` | `116-ScriptCompilationBuildProgram.Data.dll` | `93af1b2a23c21d2ec2bb2825a23c3c0e429f590cc8658a83b33b5ee03d7d47ed` | PASS |
| 117 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/StandardSocketsHttpHandler.dll` | `117-StandardSocketsHttpHandler.dll` | `659c784c861fbe5b79ff5c24cf4de0106db27a37e4265023a91ece53d6c4e0b7` | PASS |
| 118 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/System.CodeDom.dll` | `118-System.CodeDom.dll` | `3772e9c431fe84eb53580225ba8fe1dca549e76037375f5ba2be0903733fb716` | PASS |
| 119 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/Unity.BuildService.dll` | `119-Unity.BuildService.dll` | `87cf03d479f27755d731db4d98c3c30853aec574df6b9c09752b6334363192e7` | PASS |
| 120 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/Unity.Cecil.dll` | `120-Unity.Cecil.dll` | `27a8e20b84f9b6aef707d7ea026eed9e5b93f5d93b09362d61281194dee0f2a4` | PASS |
| 121 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/Unity.Cecil.Mdb.dll` | `121-Unity.Cecil.Mdb.dll` | `08db5b40b2610f31ff66dfc00b774cfee455dddd8e73f12a06f030517970f02f` | PASS |
| 122 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/Unity.Cecil.Pdb.dll` | `122-Unity.Cecil.Pdb.dll` | `ed6895a9cbe6aafb357b8c386fb9086fcd6e56899d34d4e7c15bc124ef2ccfa2` | PASS |
| 123 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/Unity.Cecil.Rocks.dll` | `123-Unity.Cecil.Rocks.dll` | `7ca9ebcef1a3ae21ea4f0d9a98337c0441bcf582571982e5ae29c49f48538e3b` | PASS |
| 124 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/Unity.CecilTools.dll` | `124-Unity.CecilTools.dll` | `1fb9871e56f58d90312ab57a5f7f008624f5bf314f4ffc783eeff176435c2729` | PASS |
| 125 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/Unity.CompilationPipeline.Common.dll` | `125-Unity.CompilationPipeline.Common.dll` | `4723691d7055334fe6df3f4bd8fe92e51697d67fec3455cd8be6aedaaac8cbaa` | PASS |
| 126 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEditor.AccessibilityModule.dll` | `126-UnityEditor.AccessibilityModule.dll` | `34d7834f87149e44371a1654e2bb753eb117738e11eca897797dd48381ee3bd4` | PASS |
| 127 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEditor.AdaptivePerformanceModule.dll` | `127-UnityEditor.AdaptivePerformanceModule.dll` | `12f2c11f4b8c024145a0e19915d118ec204e68e993028cb9861dc7edbf1bba4c` | PASS |
| 128 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEditor.AIModule.dll` | `128-UnityEditor.AIModule.dll` | `21b4b9ecaf478129a2018364e5b56fb50acb4d964aa9a15fc86055c70b3b6d65` | PASS |
| 129 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEditor.AnimationWindowModule.dll` | `129-UnityEditor.AnimationWindowModule.dll` | `58eb39d4fbd26e04b53b45e06e4b538d00ed116783f951b32f3f0ebcad01ff12` | PASS |
| 130 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEditor.AssetComplianceModule.dll` | `130-UnityEditor.AssetComplianceModule.dll` | `49fd41a007d37f9b7df2effadb3a82bffe2ab15532519c4b0ffac3681c7d8820` | PASS |
| 131 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEditor.BuildAnalysisModule.dll` | `131-UnityEditor.BuildAnalysisModule.dll` | `57103f737e5419d1f8d5cf103bfec20fe8749b6f2bc592e4abe38473e6723904` | PASS |
| 132 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEditor.BuildProfileModule.dll` | `132-UnityEditor.BuildProfileModule.dll` | `5aa3a772d54bacfef5e968489246bc496a32f59537bae543f3017ac35a7ca86a` | PASS |
| 133 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEditor.BurstModule.dll` | `133-UnityEditor.BurstModule.dll` | `049fd85bfc8d36b52918cb9ab7650b27332b92415d3e7259c09fcf32a5ab5973` | PASS |
| 134 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEditor.ClothModule.dll` | `134-UnityEditor.ClothModule.dll` | `a4620f297e46f5f599e40700802f15306ce6c4764bce8083ac84757b22e8ad19` | PASS |
| 135 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEditor.ContentLoadModule.dll` | `135-UnityEditor.ContentLoadModule.dll` | `5e19096557e1a8e9502e40d6816387195f2e5795ce646112c1c8e52d17a4926f` | PASS |
| 136 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEditor.CoreBusinessMetricsModule.dll` | `136-UnityEditor.CoreBusinessMetricsModule.dll` | `e6dd26d5059345f89f635a4efdbc23baaa91b295b8b601d8931c2b7279448ab2` | PASS |
| 137 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEditor.CoreModule.dll` | `137-UnityEditor.CoreModule.dll` | `5bd289388437461d6e81a19ef05f34ace600b687c3fd15c9299263e8f8afcab8` | PASS |
| 138 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEditor.DeviceSimulatorModule.dll` | `138-UnityEditor.DeviceSimulatorModule.dll` | `084af8781f76b637dab8ddfc88f1838328a899679ca2f2220a082ff8eebef054` | PASS |
| 139 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEditor.DiagnosticsModule.dll` | `139-UnityEditor.DiagnosticsModule.dll` | `e5488672e853c16fbdabd47cb0d4bb26bcd996d238780d08e1fd7eb04ab416aa` | PASS |
| 140 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEditor.dll` | `140-UnityEditor.dll` | `e58aa65076f4213e94c5417dfda081ffcfc24858a93dc0a23ff96df8ce0cfda5` | PASS |
| 141 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEditor.EditorToolbarModule.dll` | `141-UnityEditor.EditorToolbarModule.dll` | `7e42bcc4e5aeea49f56cabf0c494c1a7cf44f7651906c34b10f1b1a1d76f4036` | PASS |
| 142 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEditor.EmbreeModule.dll` | `142-UnityEditor.EmbreeModule.dll` | `4bd39fb5dc98025826c0d926449160427470d303ae56abe7f52435c71e7f1de7` | PASS |
| 143 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEditor.GenericRemoteModule.dll` | `143-UnityEditor.GenericRemoteModule.dll` | `ec5bc688d9c299d3e9aa8b46f57b70e88aad46a799f694e686f45a37145da7a8` | PASS |
| 144 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEditor.GIModule.dll` | `144-UnityEditor.GIModule.dll` | `8e041e56802ca1be199294e4792593c37122c07e55a0e6c7e286726c7d45411f` | PASS |
| 145 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEditor.GraphicsStateCollectionSerializerModule.dll` | `145-UnityEditor.GraphicsStateCollectionSerializerModule.dll` | `73089a04e77c523a5c28cc428411166b97518cbfc902dc7b8668734560bb3008` | PASS |
| 146 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEditor.GraphToolkitModule.dll` | `146-UnityEditor.GraphToolkitModule.dll` | `d2f748018f8a785c5ac2a9f8a3a17f4a2b6150b601444b797800f57c26fa99b2` | PASS |
| 147 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEditor.GraphViewModule.dll` | `147-UnityEditor.GraphViewModule.dll` | `98a7e7b1067829305ffac879f8bc7a8f91791aa61bf7242b54c593a938b18af8` | PASS |
| 148 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEditor.GridAndSnapModule.dll` | `148-UnityEditor.GridAndSnapModule.dll` | `67e01cb94beeb9ede020b58c5a471589feb8b0c4be559bdedd5e2478a843af29` | PASS |
| 149 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEditor.GridModule.dll` | `149-UnityEditor.GridModule.dll` | `512e0632e50025ff6649ee7a15868caf0ec8c2ce71c8dbd12c7e97a589e52663` | PASS |
| 150 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEditor.HierarchyModule.dll` | `150-UnityEditor.HierarchyModule.dll` | `0d3d389a469fe1122cd1d094761eb46ffc33918e6f71b1479c6798324b7ec2d1` | PASS |
| 151 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEditor.LightingModule.dll` | `151-UnityEditor.LightingModule.dll` | `2988e27817f07b04690f89e44f8eb19b5ff0f9e09732116d45e04dace83ddbd5` | PASS |
| 152 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEditor.MathematicsModule.dll` | `152-UnityEditor.MathematicsModule.dll` | `f1d0ce5ab42f20e4b4ba2d8eae0d0346f9a0a01514527c9c6a0a36dc40fcd424` | PASS |
| 153 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEditor.MediaModule.dll` | `153-UnityEditor.MediaModule.dll` | `b4d7bcbab9c073487553e3f776caf43cd5b31676ec2a4ee24cc4a69899404304` | PASS |
| 154 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEditor.MSBuild.dll` | `154-UnityEditor.MSBuild.dll` | `9e6ed1c69c84c9030644d4b2fc6a8082a059f4b1082f49acc7b9f5ac35468e99` | PASS |
| 155 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEditor.MultiplayerModule.dll` | `155-UnityEditor.MultiplayerModule.dll` | `b80bb9a6a5f337807f66fb5d7d8924097562c2ba615b651025edfa2157793ec3` | PASS |
| 156 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEditor.Physics2DModule.dll` | `156-UnityEditor.Physics2DModule.dll` | `13d767bc01a234bb8976870e6e6504d3296c0c021031d76559ee377d06e02080` | PASS |
| 157 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEditor.PhysicsCore2DModule.dll` | `157-UnityEditor.PhysicsCore2DModule.dll` | `8af8f653d2e1fbcf1f7ee18e40df3a3e5f60e1ed392d7915390d5f4eea667893` | PASS |
| 158 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEditor.PhysicsModule.dll` | `158-UnityEditor.PhysicsModule.dll` | `70df158ce553d6b04692b4f9b55541c171e21f0b57b0275eb109dddc75d9ea07` | PASS |
| 159 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEditor.PlayModeModule.dll` | `159-UnityEditor.PlayModeModule.dll` | `5deb07c086e866bec2326f957f8fa22baeba609e620ed3b708af385274914656` | PASS |
| 160 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEditor.PresetsUIModule.dll` | `160-UnityEditor.PresetsUIModule.dll` | `f04de5d12a95f58d31ded6eaf6ddaa0b43036056ac5cd834f9a08b2811a4b8fa` | PASS |
| 161 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEditor.ProjectAuditorModule.dll` | `161-UnityEditor.ProjectAuditorModule.dll` | `8269e3bc830a83dd7e76334aefa2aed5b4a1717fb0c568f6e94b85c280e7a0db` | PASS |
| 162 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEditor.PropertiesModule.dll` | `162-UnityEditor.PropertiesModule.dll` | `fc46c81dc9fd8e0623b33da8495d328f92b6d645c750ab81d6d648120a6c1515` | PASS |
| 163 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEditor.QuickInstallModule.dll` | `163-UnityEditor.QuickInstallModule.dll` | `98db0334974b79317909efefdb76c946356968267b4961ff39e3bb53af20d390` | PASS |
| 164 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEditor.QuickSearchModule.dll` | `164-UnityEditor.QuickSearchModule.dll` | `d125fd42f9ad01d8e9ce6d3757f1a641479ea145bd99f015cd51358ae8fbcf53` | PASS |
| 165 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEditor.SafeModeModule.dll` | `165-UnityEditor.SafeModeModule.dll` | `b88e73c89dfbf086df362780e052fc81c18e78840d2b4e124da0996a9eadbb41` | PASS |
| 166 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEditor.SceneTemplateModule.dll` | `166-UnityEditor.SceneTemplateModule.dll` | `8c895976dcbd32f39ce82b7197d042a03c1a383714dcc6b84cb64fa6aaa9f291` | PASS |
| 167 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEditor.SceneViewModule.dll` | `167-UnityEditor.SceneViewModule.dll` | `3e7f4bf16b9df73ddc346a441b89e54646e30492536f061f3d77bbb0d9ff007f` | PASS |
| 168 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEditor.ShaderBuildSettingsModule.dll` | `168-UnityEditor.ShaderBuildSettingsModule.dll` | `14c0a171e3121415e3dd9cec637ebd73a03a76f2834738df81abb14cb0a5c4be` | PASS |
| 169 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEditor.ShaderCompilationModule.dll` | `169-UnityEditor.ShaderCompilationModule.dll` | `6bb5f4ae20f82387bf95509733ab3fd1762530f7eb663511c3920565a3eeb988` | PASS |
| 170 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEditor.ShaderFoundryModule.dll` | `170-UnityEditor.ShaderFoundryModule.dll` | `7f6adac759fd0349054dd39c613ac911957875c4505a4cb5b76821401d7d6579` | PASS |
| 171 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEditor.ShaderPreprocessorModule.dll` | `171-UnityEditor.ShaderPreprocessorModule.dll` | `14ab1b7c9516d534dbe28d5101de3a43f9a52d4a53b3713f2a0a3c903ca9d9f1` | PASS |
| 172 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEditor.SketchUpModule.dll` | `172-UnityEditor.SketchUpModule.dll` | `c92c086bbe3a7113622e0535c66fde62bc44da22276d85f82b354843680eb69d` | PASS |
| 173 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEditor.SpriteMaskModule.dll` | `173-UnityEditor.SpriteMaskModule.dll` | `d0880c8598d2587d584c318867d64355d37f3b3f05995f5435a5a6075566e1fc` | PASS |
| 174 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEditor.SpriteShapeModule.dll` | `174-UnityEditor.SpriteShapeModule.dll` | `fe8cba0c5b299b29860729485f892d1b6da065ac155fd66a7bc028305f08d522` | PASS |
| 175 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEditor.SubstanceModule.dll` | `175-UnityEditor.SubstanceModule.dll` | `353ab01d6d34aa88de1925bdacec8844859c6c175b29d6197db2badea87dbd51` | PASS |
| 176 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEditor.SyntaxTreeModule.dll` | `176-UnityEditor.SyntaxTreeModule.dll` | `29e4dcf493a23e12cd33bbb16cda2616016c93119e3ff63ed86f03a6febf4b2a` | PASS |
| 177 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEditor.TerrainModule.dll` | `177-UnityEditor.TerrainModule.dll` | `158597214f028c5eabe015706b945ccfd9e461c79d05d2868d90487277e3f632` | PASS |
| 178 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEditor.TextCoreFontEngineModule.dll` | `178-UnityEditor.TextCoreFontEngineModule.dll` | `b0ce1c0cf8fdadb25783da7bf96704703df1108b0a3091271be84b0561317386` | PASS |
| 179 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEditor.TextCoreTextEngineModule.dll` | `179-UnityEditor.TextCoreTextEngineModule.dll` | `b42c939c95be9b1855a91ef7ae2a89c1c311a5e7b9d17ec12d1d151ae95734a6` | PASS |
| 180 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEditor.TextRenderingModule.dll` | `180-UnityEditor.TextRenderingModule.dll` | `d7258805f289988abbae70d97191687594bc777869fba8c3f8d12954a7a3ea60` | PASS |
| 181 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEditor.TilemapModule.dll` | `181-UnityEditor.TilemapModule.dll` | `756117c32e2c963fd5333f5718429cd3e6b068508c07f170c33faf680dff4ec1` | PASS |
| 182 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEditor.TimelineFoundationModule.dll` | `182-UnityEditor.TimelineFoundationModule.dll` | `f53e815866229e647f928f40e47de72909394e6e8f4b7a84c1c386b2c5c197c5` | PASS |
| 183 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEditor.TreeModule.dll` | `183-UnityEditor.TreeModule.dll` | `5d9baaa50dbab86b313c24a69e108d4ed1fe9f79e719c7e07f24ce8c4af276d5` | PASS |
| 184 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEditor.U2DModule.dll` | `184-UnityEditor.U2DModule.dll` | `425f4b92453b551a7bacfd9755e4e17656116b11f7d7c09d0cec02560d57b3bb` | PASS |
| 185 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEditor.UIAutomationModule.dll` | `185-UnityEditor.UIAutomationModule.dll` | `98c73f1b0e430869dbf327a96f359ec5a0df6038314fa39f3bda101aeeec8b82` | PASS |
| 186 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEditor.UIBuilderModule.dll` | `186-UnityEditor.UIBuilderModule.dll` | `9d90304bef9ae2d14880ac72338aac83d962c769d06b9b4f67aa58218456d5f8` | PASS |
| 187 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEditor.UIElementsModule.dll` | `187-UnityEditor.UIElementsModule.dll` | `6ae3491cb41fb6ac6cbc30862c6c0a1caf0beec91fef2487e11463223332227e` | PASS |
| 188 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEditor.UIElementsSamplesModule.dll` | `188-UnityEditor.UIElementsSamplesModule.dll` | `f2ab51f63b8e511ab4bd976de5368c688043e7f9cae96ea851f83571b0b630a6` | PASS |
| 189 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEditor.UIToolkitAuthoringModule.dll` | `189-UnityEditor.UIToolkitAuthoringModule.dll` | `b8fc40127c9fcfeb694196412a5fbe8b5d1ceff2ff987adc88c6eba7104e8c87` | PASS |
| 190 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEditor.UmbraModule.dll` | `190-UnityEditor.UmbraModule.dll` | `75350ed88efccdb77b1876f303fed498297a337bc01084a80eea287486407830` | PASS |
| 191 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEditor.UnityConnectModule.dll` | `191-UnityEditor.UnityConnectModule.dll` | `99894130f643cc0c2584ea73258b8bcd7bf1eeafb87c080755f3e310799cc79e` | PASS |
| 192 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEditor.VectorGraphicsModule.dll` | `192-UnityEditor.VectorGraphicsModule.dll` | `d5e87b580e2245ffce4ff5e6ad7adacddf137f01dfed052445bb9102932eb38c` | PASS |
| 193 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEditor.VFXModule.dll` | `193-UnityEditor.VFXModule.dll` | `7e14fb1fa32d88fb7f2854399e81ed6f84b63b7d85ada966f3afce273b1e0eb4` | PASS |
| 194 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEditor.VideoModule.dll` | `194-UnityEditor.VideoModule.dll` | `8b81e550f4d1829bd59e8fb92037b97b0d7077a9b12c693eed8cf1c05f010534` | PASS |
| 195 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEditor.XRModule.dll` | `195-UnityEditor.XRModule.dll` | `d36117c2dc828b659b70017e1f69f74569e0d500b61d2b65b286d0c2e965ac21` | PASS |
| 196 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.AccessibilityModule.dll` | `196-UnityEngine.AccessibilityModule.dll` | `f4426203fe39be0b5826a741eba5183f38ec1597a6b07e0e6ccb62add5d00593` | PASS |
| 197 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.AdaptivePerformanceModule.dll` | `197-UnityEngine.AdaptivePerformanceModule.dll` | `7ed8156f7d33da61f1579ebe8d36c342087e8f48393dc768d40cc756afb8e01f` | PASS |
| 198 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.AIModule.dll` | `198-UnityEngine.AIModule.dll` | `bb502d4f17ae51c37fe7c71a8471ea9092c9caf433a5828d253429b217bba05a` | PASS |
| 199 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.AMDModule.dll` | `199-UnityEngine.AMDModule.dll` | `d34f253000584f5bfb8497ad3736d70efc85c7392f30f8d01cbc7d5161d2d717` | PASS |
| 200 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.AndroidJNIModule.dll` | `200-UnityEngine.AndroidJNIModule.dll` | `761d609f5a4f051d6a67c340a3cdac281efd71fc69544ceb8ab9f6f3e62e8999` | PASS |
| 201 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.AnimationModule.dll` | `201-UnityEngine.AnimationModule.dll` | `f08ff80eda60383ddb8e15e52366cdab163ea0097033f2ad0fed6c93c9d1ef9c` | PASS |
| 202 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.ARModule.dll` | `202-UnityEngine.ARModule.dll` | `3af308904f72b4353f3b970f0a2fe1a5876e1c287b5050cac4064b57956be8eb` | PASS |
| 203 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.AssetBundleModule.dll` | `203-UnityEngine.AssetBundleModule.dll` | `3f8f4806ec592e1e07296247bfb23e55b9d2c38e258f06168254239feae68724` | PASS |
| 204 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.AudioModule.dll` | `204-UnityEngine.AudioModule.dll` | `8c4e42c460b02cdd98d9f9800cd6a58eeebc096f3be8ed786b6e18a0e8d45219` | PASS |
| 205 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.BurstModule.dll` | `205-UnityEngine.BurstModule.dll` | `c1faeef721f1c2e17a9382c4968297d8f93a1f4e5605a7912a8cc970beaaba92` | PASS |
| 206 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.ClothModule.dll` | `206-UnityEngine.ClothModule.dll` | `a32c50930bdd8686f69e1faafeec882097226f2d718edfb4a389680a21c071a7` | PASS |
| 207 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.ClusterInputModule.dll` | `207-UnityEngine.ClusterInputModule.dll` | `c275d7713c7554b37570f5a391c3cf72a26b8b93c09b5af40134375958e1dc0b` | PASS |
| 208 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.ClusterRendererModule.dll` | `208-UnityEngine.ClusterRendererModule.dll` | `7b679dd0b8d6450d29f8e3eaf37ca154dc697c0766fde81aea6fd4fa26eeb3f5` | PASS |
| 209 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.ContentLoadModule.dll` | `209-UnityEngine.ContentLoadModule.dll` | `030ff21abdbaed47d53ff9f28f61e51cb0b70454282fd5feb2cd9ba08faebd6e` | PASS |
| 210 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.CoreModule.dll` | `210-UnityEngine.CoreModule.dll` | `dcdf14b9eb8e09fe7bed5fac9da39406ed74f89a8dc7c99dbbd53755ce4c5ea5` | PASS |
| 211 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.CrashReportingModule.dll` | `211-UnityEngine.CrashReportingModule.dll` | `06f577e2090ff74f0456d3a29ff151a6c159b1a3a836d7dbe0f034ebe2216d59` | PASS |
| 212 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.DirectorModule.dll` | `212-UnityEngine.DirectorModule.dll` | `576a464dd356928246e0dfb44ca6c07b9ae8673c73fc908a394e83526733d327` | PASS |
| 213 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.dll` | `213-UnityEngine.dll` | `001e8ef92fe33d45ba2bfeeed4694648dd2ebc3004863aca09f4c1c4f1276410` | PASS |
| 214 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.DSPGraphModule.dll` | `214-UnityEngine.DSPGraphModule.dll` | `2d3c776f018d4341f53e3bac1ed0f7eb01585f4cc1b63c880bd4c7a680a0269f` | PASS |
| 215 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.GameCenterModule.dll` | `215-UnityEngine.GameCenterModule.dll` | `f6e05a1ce380b7db7c4a60bff80c6e094482f89ac3177c96c934e428a5712adf` | PASS |
| 216 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.GIModule.dll` | `216-UnityEngine.GIModule.dll` | `f4acd33adf98bbd79bc515e50cfb115b23ca2f63668ce6098cb25dc0e2464a3d` | PASS |
| 217 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.GraphicsStateCollectionSerializerModule.dll` | `217-UnityEngine.GraphicsStateCollectionSerializerModule.dll` | `9f72b4dbdef96b286721752f55b6732bfdf65f58586e2b88626e7ea77eb7f9a4` | PASS |
| 218 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.GridModule.dll` | `218-UnityEngine.GridModule.dll` | `1010a415f83ea6fcc3d90205686964d92d206ab7bf1aa627c864798587a8474f` | PASS |
| 219 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.HierarchyCoreModule.dll` | `219-UnityEngine.HierarchyCoreModule.dll` | `dce448bdd276062a72527777253debc12c311dd85c82ac5f653b391d16735dac` | PASS |
| 220 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.HierarchyModule.dll` | `220-UnityEngine.HierarchyModule.dll` | `713611947fc4f391bf0bc1da93af93c346399b4656273ab9427e135249a1b12f` | PASS |
| 221 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.HotReloadModule.dll` | `221-UnityEngine.HotReloadModule.dll` | `fc27f447adaa679095ff48c04fb7dd96069aa8b66a0c3c83a63a8c59d2279192` | PASS |
| 222 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.IdentifiersModule.dll` | `222-UnityEngine.IdentifiersModule.dll` | `37eac2763b7a8fdb23b161b7ad7eb9561b2b79a17a858a2c3f6d99c5a898df26` | PASS |
| 223 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.ImageConversionModule.dll` | `223-UnityEngine.ImageConversionModule.dll` | `abbb1f6bf4321513607d97b306337a79335046db52207bffca98fe99a26c044b` | PASS |
| 224 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.IMGUIModule.dll` | `224-UnityEngine.IMGUIModule.dll` | `f7b7a89beb3063e194f3d2db43b7f9e94fa685538bc533292e6941ed4f4c4d42` | PASS |
| 225 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.InputForUIModule.dll` | `225-UnityEngine.InputForUIModule.dll` | `e458ed47b66c30af3c4b06169f7c1443b9cbf146c05f21c013edf84602d0cb9f` | PASS |
| 226 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.InputLegacyModule.dll` | `226-UnityEngine.InputLegacyModule.dll` | `a027861977018d0666e3d5b13b9191de29eee2c8c8fe4901c554da693ded16cb` | PASS |
| 227 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.InputModule.dll` | `227-UnityEngine.InputModule.dll` | `28063570ebb872b70eecee26f0adfa0e75dc8c987455333249d40531b4236702` | PASS |
| 228 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.InsightsModule.dll` | `228-UnityEngine.InsightsModule.dll` | `90c166d7a8c614b93e6487c175b21069f3c1389fa701dada413d283607c5d0bc` | PASS |
| 229 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.JSONSerializeModule.dll` | `229-UnityEngine.JSONSerializeModule.dll` | `82755ada0c349e902dcb94eba922e7f48c63fea6bcf93ff4144de4e3e335df25` | PASS |
| 230 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.LocalizationModule.dll` | `230-UnityEngine.LocalizationModule.dll` | `9a2352c42307c0c8fb77d9bc7f936cdf0f76d7c0740a992890e21eb4a4cbef9e` | PASS |
| 231 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.ManagedKernelModule.dll` | `231-UnityEngine.ManagedKernelModule.dll` | `9f13c4e4a37805713686d349aaab8f45b957f590719ca7aa52a33b2902437570` | PASS |
| 232 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.MarshallingModule.dll` | `232-UnityEngine.MarshallingModule.dll` | `a5e0e137e789ee863062b36afa97253ea80029a508566206105bdf9cdbe805e0` | PASS |
| 233 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.MathematicsModule.dll` | `233-UnityEngine.MathematicsModule.dll` | `1b0dfffa66e2c904e17d0e86ecc71b6dfd77dead3e0b0f314831317b9f8126bc` | PASS |
| 234 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.MultiplayerModule.dll` | `234-UnityEngine.MultiplayerModule.dll` | `941eee5d251ee90fa1a47fdf91924f18797229fef0428f9d2207bc214a571a8e` | PASS |
| 235 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.NVIDIAModule.dll` | `235-UnityEngine.NVIDIAModule.dll` | `1f7d89edb4fa139b417bbdbfbfa536fc309cce4c85f91e75732b2ebb106c2d10` | PASS |
| 236 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.ParticleSystemModule.dll` | `236-UnityEngine.ParticleSystemModule.dll` | `d53d66294b5b51d17b7389a3cd7e6a9749150300e849d213a3fd17b3e27e87b9` | PASS |
| 237 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.PerformanceReportingModule.dll` | `237-UnityEngine.PerformanceReportingModule.dll` | `1936a6470f98b5322537d00878cd429f550c0c55623e1727b527b55c6ccf90e5` | PASS |
| 238 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.Physics2DModule.dll` | `238-UnityEngine.Physics2DModule.dll` | `192108e10887feeeafaed6a61e2f5ebb607454706f73730b481af18f4359edaf` | PASS |
| 239 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.PhysicsBackendPhysXModule.dll` | `239-UnityEngine.PhysicsBackendPhysXModule.dll` | `83939d369ee2ff41a26c1bb3a19d8dd7316a8ef0a08b507c4592d96d11a7c6f8` | PASS |
| 240 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.PhysicsCore2DModule.dll` | `240-UnityEngine.PhysicsCore2DModule.dll` | `06569024cbc7fe64bf31944677967f071a485efb2514b3316c9ded9a731bbcc9` | PASS |
| 241 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.PhysicsModule.dll` | `241-UnityEngine.PhysicsModule.dll` | `934a3d22f246949379cb3b24bcdaaafe3e071b9f2eb6441ee9076aacd965b6db` | PASS |
| 242 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.PropertiesModule.dll` | `242-UnityEngine.PropertiesModule.dll` | `a142780bb83bb277ada65fd2c4ff79ec2a3a8b1644a19a51b8e0d38e87c28b9d` | PASS |
| 243 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.RenderAs2DModule.dll` | `243-UnityEngine.RenderAs2DModule.dll` | `34281ef84a5d601872adeffa6cc5546fbd7954f2387f97c190c7ac1828ef4ee0` | PASS |
| 244 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.RuntimeInitializeOnLoadManagerInitializerModule.dll` | `244-UnityEngine.RuntimeInitializeOnLoadManagerInitializerModule.dll` | `8464fe4c1d93936357c5b0e989fd35a4cc1e29494ebb48b55d757db6b1985d78` | PASS |
| 245 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.ScreenCaptureModule.dll` | `245-UnityEngine.ScreenCaptureModule.dll` | `fbd93fe0eae6d7c14354a191ea488a9c378838350d907973972a4e4fad64192b` | PASS |
| 246 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.ScriptingModule.dll` | `246-UnityEngine.ScriptingModule.dll` | `93f5bb339ed617d5e113c5b79da6f231794bcb0875e0052cab945e907444ff58` | PASS |
| 247 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.ShaderVariantAnalyticsModule.dll` | `247-UnityEngine.ShaderVariantAnalyticsModule.dll` | `ef3ed69a5b316205732b18aa855af3296de03a2aa7c39038d689aea1101028af` | PASS |
| 248 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.SpriteMaskModule.dll` | `248-UnityEngine.SpriteMaskModule.dll` | `5c724e6d0eb8e5bb3d8929a533b2bc2cf69d854ebb774f3cc69c03a98cd36b03` | PASS |
| 249 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.SpriteShapeModule.dll` | `249-UnityEngine.SpriteShapeModule.dll` | `34b9c1b4e879c17187e3b7fd01bc95d26c746c83eb34f99d175b69891a25a4c5` | PASS |
| 250 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.StreamingModule.dll` | `250-UnityEngine.StreamingModule.dll` | `da2288dfe91f105ce914b96e388ab68ec5c04dbeb7e083b78a5530e2355e7513` | PASS |
| 251 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.SubstanceModule.dll` | `251-UnityEngine.SubstanceModule.dll` | `32123252842dba0739c03bbc6457c887c118e3670e11a56649a9ed861b33c397` | PASS |
| 252 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.SubsystemsModule.dll` | `252-UnityEngine.SubsystemsModule.dll` | `8b5393ac569af7e01def6fd17e705e48e33f5f7f2fb1db39c41e94d14ce0c6a4` | PASS |
| 253 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.TerrainModule.dll` | `253-UnityEngine.TerrainModule.dll` | `7a26a1926c861a46e273b1cc00d257ef91c77fb33f73bb4b090565dd56d62f13` | PASS |
| 254 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.TerrainPhysicsModule.dll` | `254-UnityEngine.TerrainPhysicsModule.dll` | `1eccd606a0f013c86ac2e28d76aa14c1de03ddf580b139abb403cf6ce63d9c5f` | PASS |
| 255 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.TetgenModule.dll` | `255-UnityEngine.TetgenModule.dll` | `a4d8110c90f1f8eedc5abcd51a45d4bf255b4115c8f0aceb390041a639381c73` | PASS |
| 256 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.TextCoreFontEngineModule.dll` | `256-UnityEngine.TextCoreFontEngineModule.dll` | `7aa7969f5407dde3b5d11656c535bbd3d465f4915141f4a6fc152323bf4d1ed8` | PASS |
| 257 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.TextCoreTextEngineModule.dll` | `257-UnityEngine.TextCoreTextEngineModule.dll` | `adf5fc60dd7348d87ed2cbd7e7cc51eeeec2fe963dbd9b6c3a0c1b36a67cfcc8` | PASS |
| 258 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.TextRenderingModule.dll` | `258-UnityEngine.TextRenderingModule.dll` | `12931fdee139c30a2eb3c455b49f9216e5c175ab279ac07cf9be58df4698f915` | PASS |
| 259 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.TilemapModule.dll` | `259-UnityEngine.TilemapModule.dll` | `8a96558c2b12ef36d5160f104ff5c71f198243a835bc6dcaa11a6f97988f7a30` | PASS |
| 260 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.TimelineFoundationModule.dll` | `260-UnityEngine.TimelineFoundationModule.dll` | `d5910a9eaee6dc10e9f7ecb94796841bfc468d6a172864840f207383254ffc8f` | PASS |
| 261 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.TLSModule.dll` | `261-UnityEngine.TLSModule.dll` | `0382040112f47d7d1985f73fa6b7ed452b18b5ca8a1d84d214c54244ec9cef7e` | PASS |
| 262 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.U2DRuntimeModule.dll` | `262-UnityEngine.U2DRuntimeModule.dll` | `1d170259b835188c6b13a885f97abba5ce77afc12f0a58fdb7eb48d27c41e090` | PASS |
| 263 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.UIElementsModule.dll` | `263-UnityEngine.UIElementsModule.dll` | `54f0c86c6a09cd2456f4206136c4d32d83a85e829cb122dbdbc8bbd4cbb9bb4a` | PASS |
| 264 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.UIModule.dll` | `264-UnityEngine.UIModule.dll` | `d31d79853e64f3f475d7d20a107b644774718f60b6b9113e6af7d99a81ac9f6e` | PASS |
| 265 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.UmbraModule.dll` | `265-UnityEngine.UmbraModule.dll` | `fc6a7d3849b1d5263ec40bf03c1f51afb03ee414999de9b8bcdcd3a9d577fa81` | PASS |
| 266 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.UnityAnalyticsCommonModule.dll` | `266-UnityEngine.UnityAnalyticsCommonModule.dll` | `0f5295d61d41736c30037521890155953c6f4068e430346abb6df41889244001` | PASS |
| 267 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.UnityAnalyticsModule.dll` | `267-UnityEngine.UnityAnalyticsModule.dll` | `e85108ccc10ff733c49f303d746c24a934304b9e0cd73be535eabd1330411e35` | PASS |
| 268 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.UnityConnectModule.dll` | `268-UnityEngine.UnityConnectModule.dll` | `36ada91983520546e460ff7bc3ac4062aa804bcfe93f3561d0a7f089f4d31f2d` | PASS |
| 269 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.UnityConsentModule.dll` | `269-UnityEngine.UnityConsentModule.dll` | `0ea15078b1fe11d2370e16c699c64e9ef60436aaa250ded74cac2108f1aa9bf3` | PASS |
| 270 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.UnityCurlModule.dll` | `270-UnityEngine.UnityCurlModule.dll` | `5fa8cf36c65e6376c8f846d6439120a9e271f9143f0124ea13d65392af03eb57` | PASS |
| 271 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.UnityWebRequestAssetBundleModule.dll` | `271-UnityEngine.UnityWebRequestAssetBundleModule.dll` | `7f1bed3527d6cc22156a35fca0a9d3873841b66ad5fa52fddc144f8aa0bd83c8` | PASS |
| 272 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.UnityWebRequestAudioModule.dll` | `272-UnityEngine.UnityWebRequestAudioModule.dll` | `36b44a35e9fa10ac2cb1af0b145fa2d0d27e2b044cbda395b2f2fbc69f6adfe5` | PASS |
| 273 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.UnityWebRequestModule.dll` | `273-UnityEngine.UnityWebRequestModule.dll` | `973e23ec7079d00d34d5b037854fa4f1c37f8ccdfe8efde11ac0f3d5e36af572` | PASS |
| 274 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.UnityWebRequestTextureModule.dll` | `274-UnityEngine.UnityWebRequestTextureModule.dll` | `2e705aa886992adc75edcdcfc3022fbca9f15d9ba5a8bb8a73d37d2af7297e90` | PASS |
| 275 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.UnityWebRequestWWWModule.dll` | `275-UnityEngine.UnityWebRequestWWWModule.dll` | `c3c0ca3c814072fde866ddde49dd884f2af4b9142930d41ab90efa16aa2e975d` | PASS |
| 276 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.VectorGraphicsModule.dll` | `276-UnityEngine.VectorGraphicsModule.dll` | `dfe078c8b948a0e05660333849e76ea268301d98d1bc0a7dc2dc35e9f92b9f65` | PASS |
| 277 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.VehiclesModule.dll` | `277-UnityEngine.VehiclesModule.dll` | `18120cb7770a84f7465f33eaac4977f7624e5845fd05c449f1fa92d1ded7bf1c` | PASS |
| 278 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.VFXModule.dll` | `278-UnityEngine.VFXModule.dll` | `e686d969e4f4fe80ceb8241f5c48ec2a422b776fe5cf071a1381a81986080b4e` | PASS |
| 279 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.VideoModule.dll` | `279-UnityEngine.VideoModule.dll` | `f909de18d14d63cef89c2df7fd388cb27e623af05d40885ea657ec5bb302bf70` | PASS |
| 280 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.VirtualTexturingModule.dll` | `280-UnityEngine.VirtualTexturingModule.dll` | `7b613f7d57efe48a2bd0ad1e1ca4050a59e163e5e89f5cd9efa4134dd248605e` | PASS |
| 281 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.WindModule.dll` | `281-UnityEngine.WindModule.dll` | `e9bc4efe54737b3634b6ddbc84ff4f2bc600bd4d63d6f53457ca9ada7399bda9` | PASS |
| 282 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/UnityEngine.XRModule.dll` | `282-UnityEngine.XRModule.dll` | `309fbb1a163c37460f730c2a7fde4e3c1297a99c7981a6b2b828b7296fc6e718` | PASS |
| 283 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/Unity.FileUtilities.dll` | `283-Unity.FileUtilities.dll` | `6ff7bb3de59e53dcbf306808ad358957649318d3110191ec64e894c52be79547` | PASS |
| 284 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/Unity.Scripting.dll` | `284-Unity.Scripting.dll` | `2e68e737dc33b33b013cdaf0fc40940ff320121133316a64845c2d4bb639f0b5` | PASS |
| 285 | `Unity/6000.6.0f1/Editor/Data/Managed/UnityEngine/Unity.SerializationLogic.dll` | `285-Unity.SerializationLogic.dll` | `ec65456692ea706b542f050f335a55a49f2a97b9d6096c6c1f27ec9e590ae613` | PASS |
| 286 | `CCF-MainIntegration-20261104/Library/ScriptAssemblies/Unity.AI.Navigation.dll` | `286-Unity.AI.Navigation.dll` | `cd362a3c1a4c5c9f035e24a50f79025648fcb61fe3c7228f446674a7f7f513d6` | PASS |
| 287 | `CCF-MainIntegration-20261104/Library/ScriptAssemblies/Unity.AI.Navigation.Updater.dll` | `287-Unity.AI.Navigation.Updater.dll` | `65700eb5524bb01b2831a72285039a8cf90cd9e56b0c35723d63912a4c6bbc46` | PASS |
| 288 | `CCF-MainIntegration-20261104/Library/ScriptAssemblies/Unity.Burst.dll` | `288-Unity.Burst.dll` | `67ff1175b833fcb2480baa06f0f457c626d85f5acc30d7d4b9f46201cfea2a1a` | PASS |
| 289 | `CCF-MainIntegration-20261104/Library/ScriptAssemblies/Unity.Collections.dll` | `289-Unity.Collections.dll` | `04872bee974d7ea82238e6b12ae883d071c0fbf1d1e1907075bd6fc4a923e786` | PASS |
| 290 | `CCF-MainIntegration-20261104/Library/ScriptAssemblies/UnityEngine.UI.dll` | `290-UnityEngine.UI.dll` | `38716739cc5750d361adf8951a1a4c89ce2fffdafe8d171a46dadf5964f5cda6` | PASS |
| 291 | `CCF-MainIntegration-20261104/Library/ScriptAssemblies/Unity.InputSystem.dll` | `291-Unity.InputSystem.dll` | `886b9f30405cd78ce49853066f17107c7956613fda8c12b4e895f8188c4ee48f` | PASS |
| 292 | `CCF-MainIntegration-20261104/Library/ScriptAssemblies/Unity.InputSystem.ForUI.dll` | `292-Unity.InputSystem.ForUI.dll` | `34ad7b37e90f9ebc77235b32c3c2feb937e7bdc3e43f66e15e3f16487ba5c168` | PASS |
| 293 | `CCF-MainIntegration-20261104/Library/ScriptAssemblies/Unity.InternalAPIEngineBridge.004.dll` | `293-Unity.InternalAPIEngineBridge.004.dll` | `0b7b16b4c84a13b05c40691106575be4c75c86532147a9dec4a3e680ae08bd4a` | PASS |
| 294 | `CCF-MainIntegration-20261104/Library/ScriptAssemblies/Unity.InternalAPIEngineBridge.ObjectDispatcher.Runtime.dll` | `294-Unity.InternalAPIEngineBridge.ObjectDispatcher.Runtime.dll` | `e150792cfa5820cdd538880b6434138736b68d990c9ed204c926256128434cfd` | PASS |
| 295 | `CCF-MainIntegration-20261104/Library/ScriptAssemblies/Unity.InternalAPIEngineBridge.RenderPipelines.Core.Runtime.Shared.dll` | `295-Unity.InternalAPIEngineBridge.RenderPipelines.Core.Runtime.Shared.dll` | `58c4a3a64e89e86cd80c1212b3940417d9eabf5e049c652270e276ad532d9a8f` | PASS |
| 296 | `CCF-MainIntegration-20261104/Library/ScriptAssemblies/Unity.PathTracing.Runtime.dll` | `296-Unity.PathTracing.Runtime.dll` | `e169a09b0b9419858c2ee10375e23fe3b099872bb6ae8c1b28ca469bb257f33b` | PASS |
| 297 | `CCF-MainIntegration-20261104/Library/ScriptAssemblies/Unity.Profiling.Core.dll` | `297-Unity.Profiling.Core.dll` | `ac6944098a1a075b21650643e60c6f9ac38e32d1e1f568e39f92e02cbef63489` | PASS |
| 298 | `CCF-MainIntegration-20261104/Library/ScriptAssemblies/Unity.RenderPipelines.Core.Runtime.dll` | `298-Unity.RenderPipelines.Core.Runtime.dll` | `f01422cba5bdc721f51402735bf2c92bed0bf55fc8c80ffd8733be3e5de9abfa` | PASS |
| 299 | `CCF-MainIntegration-20261104/Library/ScriptAssemblies/Unity.RenderPipelines.Core.Runtime.Shared.dll` | `299-Unity.RenderPipelines.Core.Runtime.Shared.dll` | `5aeeaaa80f57fc177b89209a6b6728e812bba0674c5c0cd3e79951da4c81cb7d` | PASS |
| 300 | `CCF-MainIntegration-20261104/Library/ScriptAssemblies/Unity.RenderPipelines.Core.ShaderLibrary.dll` | `300-Unity.RenderPipelines.Core.ShaderLibrary.dll` | `08f566a79bc2c5b7121d1ded56c6f41738f01672aff6501944cd56f6bc938bc2` | PASS |
| 301 | `CCF-MainIntegration-20261104/Library/ScriptAssemblies/Unity.RenderPipelines.GPUDriven.Runtime.dll` | `301-Unity.RenderPipelines.GPUDriven.Runtime.dll` | `516c4b8c5ca4069948b849b1b175190eafc11745f4d6d9c470aa915c92154566` | PASS |
| 302 | `CCF-MainIntegration-20261104/Library/ScriptAssemblies/Unity.RenderPipelines.ShaderGraph.ShaderGraphLibrary.dll` | `302-Unity.RenderPipelines.ShaderGraph.ShaderGraphLibrary.dll` | `ea058406a96813645e68704c957a812915ad5667928210284643bc5aac735013` | PASS |
| 303 | `CCF-MainIntegration-20261104/Library/ScriptAssemblies/Unity.RenderPipelines.Universal.2D.Runtime.dll` | `303-Unity.RenderPipelines.Universal.2D.Runtime.dll` | `dbaa31e6c915d4fdd50acdfa54bb088eeac195f81d705e9e84c96608df6042b5` | PASS |
| 304 | `CCF-MainIntegration-20261104/Library/ScriptAssemblies/Unity.RenderPipelines.Universal.Config.Runtime.dll` | `304-Unity.RenderPipelines.Universal.Config.Runtime.dll` | `20fca8f197e413c392c612d6636a59a11dc361a3dc884b711e55a917fee8c165` | PASS |
| 305 | `CCF-MainIntegration-20261104/Library/ScriptAssemblies/Unity.RenderPipelines.Universal.Runtime.dll` | `305-Unity.RenderPipelines.Universal.Runtime.dll` | `739b3833014209259429142a874d99063b10d0ae5216df625773f14e3fdeecb7` | PASS |
| 306 | `CCF-MainIntegration-20261104/Library/ScriptAssemblies/Unity.RenderPipelines.Universal.Shaders.dll` | `306-Unity.RenderPipelines.Universal.Shaders.dll` | `df257df03f12bd7edcf0107101623f53040cf0f903c3ba955089b242043cd97c` | PASS |
| 307 | `CCF-MainIntegration-20261104/Library/ScriptAssemblies/Unity.RenderPipeline.Universal.ShaderLibrary.dll` | `307-Unity.RenderPipeline.Universal.ShaderLibrary.dll` | `be2027d6329ef92db68129f86cb7902d7eb2857d6a3883fef504fd7fb966a76b` | PASS |
| 308 | `CCF-MainIntegration-20261104/Library/ScriptAssemblies/Unity.ShaderGraph.Utilities.dll` | `308-Unity.ShaderGraph.Utilities.dll` | `5c33da3103c3f3404a0d2940ecc33746106b7333da410cd976cff9a18cef906f` | PASS |
| 309 | `CCF-MainIntegration-20261104/Library/ScriptAssemblies/Unity.SurfaceCache.Runtime.dll` | `309-Unity.SurfaceCache.Runtime.dll` | `076a64a193d42b0d1149521b86d59f27dd8491e9a509eea81acfe8c1e0083b8e` | PASS |
| 310 | `CCF-MainIntegration-20261104/Library/ScriptAssemblies/Unity.Sysroot.Linux_arm64.dll` | `310-Unity.Sysroot.Linux_arm64.dll` | `97cf19705086e61b9a3880e0c40da1f88128aab70809d7b17623def8d2210b5e` | PASS |
| 311 | `CCF-MainIntegration-20261104/Library/ScriptAssemblies/Unity.Sysroot.Linux_x86_64.dll` | `311-Unity.Sysroot.Linux_x86_64.dll` | `7a8e9580256ebad5130ba37908c16c1415ca7b15dd65f74797c256b8e472f938` | PASS |
| 312 | `CCF-MainIntegration-20261104/Library/ScriptAssemblies/Unity.TextMeshPro.dll` | `312-Unity.TextMeshPro.dll` | `f85fff4db7ccf21d1fa9402f5a9c27498e944ab2e46660929ae6ad70347b98c3` | PASS |
| 313 | `CCF-MainIntegration-20261104/Library/ScriptAssemblies/Unity.Timeline.dll` | `313-Unity.Timeline.dll` | `ca049c8f2a99a990ace87b8cbc3b861cf903eb0557b378a97f3d4a03afff5370` | PASS |
| 314 | `CCF-MainIntegration-20261104/Library/ScriptAssemblies/Unity.Toolchain.Linux-x86_64-Linux.dll` | `314-Unity.Toolchain.Linux-x86_64-Linux.dll` | `6280feb7f7075b577a219e1915fa88b15872ef55ecb0e024c53c0db4d1f4e256` | PASS |
| 315 | `CCF-MainIntegration-20261104/Library/ScriptAssemblies/Unity.UnifiedRayTracing.Runtime.dll` | `315-Unity.UnifiedRayTracing.Runtime.dll` | `e2ace68000e8058023be312252ff59697d6e205c91325e72241912cda373b5e7` | PASS |
| 316 | `CCF-MainIntegration-20261104/Library/ScriptAssemblies/Unity.VisualScripting.Core.dll` | `316-Unity.VisualScripting.Core.dll` | `f89e8cfda34af65f81e06d809ec8f58db3ce00b6a0dbce98267823062fee788b` | PASS |
| 317 | `CCF-MainIntegration-20261104/Library/ScriptAssemblies/Unity.VisualScripting.DocCodeExamples.dll` | `317-Unity.VisualScripting.DocCodeExamples.dll` | `a570f2dfce3d832704b9b747249c16045985e4de2e1a82e127487517e944d13d` | PASS |
| 318 | `CCF-MainIntegration-20261104/Library/ScriptAssemblies/Unity.VisualScripting.Flow.dll` | `318-Unity.VisualScripting.Flow.dll` | `049a926075ac0021b9a18ddaa742edd22783ccd33667905402da809162baaece` | PASS |
| 319 | `CCF-MainIntegration-20261104/Library/ScriptAssemblies/Unity.VisualScripting.State.dll` | `319-Unity.VisualScripting.State.dll` | `b5d96a94ec8f4beb07f91a899158295deb79280533896d6605b741af48faaeab` | PASS |

</details>
