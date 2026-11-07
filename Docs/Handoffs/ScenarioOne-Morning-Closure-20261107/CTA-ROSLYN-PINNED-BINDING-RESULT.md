# #328 Stage A — CTA Roslyn pinned symbol binding — author result

**AUTHOR HANDOFF — SOURCE-ONLY. Not approved, not integrated, not Unity-verified.** Independent review is #329. This worker does not accept its own work.

Binding authority: [Manager packet](https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Morning-Closure-20261107/MANAGER-327-NEXT-SOURCE-CLOSURE-PACKET-20261107.md) and [ruling A](https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Morning-Closure-20261107/MANAGER-327-DISPOSITION-AND-VERIFIER-ARCHITECTURE-20261107.md). Claim: [#328 RUNNING receipt](https://github.com/jermur788/CCF/issues/328#issuecomment-6710053902).

## Identity

| Item | Value |
|---|---|
| Branch (new) | `fix/cta-roslyn-symbol-binding-20261107-v1` |
| Base (exact published #325 head) | `c83fae1af4ac1449cba0304e44b0df49b91f8919` |
| Implementation commit | `5f3044085268a7ed0377c18a89ecb7a14c8f0362` |
| **Tested commit** | **`288ba3863793e3775327fb7322dc06af28f7835a`** (= implementation + one test-only line, below) |
| Published head | the commit that adds this document; it changes no tested byte (`git diff 288ba386 <head>` is this file only) |
| Worktree | `/media/jer/Files/CCF/Operations/CCF-CTARoslyn-20261107` (new, sole author; canonical store `/media/jer/Files/CCF/Repository.git`) |
| Evidence (`E/`) | `/media/jer/Files/CCF/Verification/CTARoslynBinding-328-20261107/` |
| Worker | T3 Code / Claude Code harness, model **`claude-sonnet-5-5` (Sonnet 5.5)**, Anthropic provider; reasoning effort not observable by me. This is **not** the packet's recommended GPT-6.1 Sol High: it is the model the user dispatched. Treat the independent review accordingly. |

**Preflight (before any edit, read-only):** live `active-tasks.md` read from `origin/main` `a37089e1` (no owner of the #325/#323 lanes); `git ls-remote` showed `verify/scenario-one-feedback-cta-anchor-20261106` at `c83fae1a` (= packet); target branch and path did not exist; no Git lock files; no Unity or Git writer process; Files 35 GiB free. The worktree was clean (tracked, untracked, ignored) at start and at the end of every gate. The coordination anchor `/media/jer/Files/CCF-Primary` has two pre-existing dirty tracked files (`AGENTS.md`, `Docs/Operations/StorageArchitecture.md`); they are not mine and were not touched or used.

## What changed (exact inventory vs `c83fae1a`)

`git diff --stat c83fae1a 288ba386`: 6 files, +1391/−1, **all under `Tools/Verification/OvernightPlaytestFeedback/`**.

| Path | Role | SHA-256 |
|---|---|---|
| `ObjectiveCtaRoslynBinding.cs` (new, 233 lines) | The bounded Roslyn entrypoint. Reports facts only: compile errors, every `ScenarioOneUiRoot` type with declaring files and resolved base type, and each `Time.*` / `DisplayStyle.*` member access with the semantic symbol of its receiver and member. | `aa8579b9…2b864` |
| `objective_cta_roslyn_binding.py` (new) | Owns the rules and the plumbing: SDK pin, compiler and reference hash verification, build of the reporter, pass/fail rules, fail-closed handling. | `d0e460bc…b36a` |
| `roslyn-reference-manifest.txt` (new) | The 320 SHA-256 reference pins, byte-identical to the #325/#326 manifest. | `97812324…18f1` |
| `check_baseline.py` | Check 08 runs the new stage after the lexical stage accepts. One import, two lines, and the PASS note text. The FAIL path and every other check are unchanged. | `129c36aa…7161` |
| `test_objective_cta_roslyn_binding.py` (new) | Four-section test suite, below. | `023b36a7…c988` |
| `README.md` | New section "Roslyn symbol binding (#328)": rules, pins, needs, fail-closed list, limits. | `53ea4de5…1633` |

**Unchanged, verified by Git blob id between `c83fae1a` and `288ba386`:** `objective_cta_source_check.py` (`360122d7…`), `test_objective_cta_source_check.py` (`1bcb93d1…`), `baseline-output.txt` (`db6a8b85…`). `git diff c83fae1a 288ba386 -- Assets Packages ProjectSettings .vscode` is empty. No production, Unity, package, save, Reference Future v1, Legacy40, PR #300 or `main` change. The one-line difference between the implementation and tested commits is test-only (see "Process disclosures").

## Design (one guarded semantic check, not a framework)

Check 08 is now two stages. **Stage 1** is the existing lexical checker, unchanged. **Stage 2** runs only on source that stage 1 accepted, so every earlier cause assertion and the historical red are untouched. Stage 2:

1. Pins the SDK: a `global.json` (8.0.133, `rollForward: disable`) in a temporary folder; every `dotnet` call runs there, so the caller's working directory and the SDK 10 default do not matter. Requires `dotnet --version` == `8.0.133`.
2. Verifies the compiler: SHA-256 of the SDK's `Microsoft.CodeAnalysis.dll` and `Microsoft.CodeAnalysis.CSharp.dll`.
3. Verifies the references: the manifest's own SHA-256, then all 320 files by SHA-256, no symlinks, no extra files in the folder.
4. Builds the reporter offline in a temporary folder against the SDK's own Roslyn DLLs. **No NuGet package, nothing downloaded.**
5. Compiles the player-like source set (every `Assets/**/*.cs` outside `Editor` and dot/`~` folders; C# 9; the Player defines) and judges the reported symbols.

**Rules, all pinned** (`objective_cta_roslyn_binding.py`): 0 compile errors; exactly one type named `ScenarioOneUiRoot`, the global class declared only in `ScenarioOneUiRoot.cs`, base type the **metadata** type `UnityEngine.MonoBehaviour` in `UnityEngine.CoreModule`; every member access whose receiver is spelled `Time` has a receiver that is the metadata type `UnityEngine.Time` and a member that is a property of it; every `DisplayStyle` receiver is the metadata enum `UnityEngine.UIElements.DisplayStyle` (`UnityEngine.UIElementsModule`) and every member is a field of it; and the site multiset is exactly `Time.frameCount` ×3, `Time.unscaledTime` ×2, `DisplayStyle.Flex` ×9, `DisplayStyle.None` ×13 (27 sites). Any other member on those receivers, or any `Time` / `DisplayStyle` identifier outside such an access (using directives aside), fails. A source type, local, field, property, tuple element, alias to a source type, or ambiguous name fails with a message such as ``receiver `Time` binds to Field ScenarioOneUiRoot.Time (source in this project)``.

**Baseline fact** (Roslyn 4.8.0.0 on .NET 8.0.33): 189 files, source-set SHA-256 `e5394306…6297`, 0 errors, one root type, base `UnityEngine.MonoBehaviour` from metadata, and the 27 sites above all bind to `UnityEngine.Time` / `UnityEngine.UIElements.DisplayStyle`.

**Fails closed** (each has a test, section C): `dotnet` absent; SDK ≠ 8.0.133; compiler DLL hash mismatch; reference folder missing; one reference corrupted, missing or extra; manifest hash mismatch; topology the single-assembly model cannot describe (an `.asmdef`/`.asmref`/`.rsp` outside Editor, or C# in `Plugins`/`Standard Assets` folders); reporter build or run failure; reporter output absent or not JSON; an unexpected exception anywhere in the stage. Each is a completed FAIL of check 08 (`ObjectiveCtaBindingError`), never a pass and never a crash.

## Results at the tested commit `288ba386`

All commands ran with `python3 -B`, `PYTHONPYCACHEPREFIX` and `TMPDIR` under `E/`, `UseSharedCompilation=false`, `MSBUILDDISABLENODEREUSE=1`, telemetry off, no Unity. Raw output: `E/out/` (`proof/` and `final-*.txt`). A first full set at `5f304408` is retained under `E/out/superseded-5f304408/` (see disclosures).

### Required results

| # | Requirement | Result |
|---|---|---|
| 1 | `ScenarioOneUiRoot` derives from `UnityEngine.MonoBehaviour` | Baseline PASS (metadata, `UnityEngine.CoreModule`). Wrong or source base fails for `[base type]` (S06, S07, X328-N01 negatives). |
| 2 | Every reviewed `Time.frameCount` / `Time.unscaledTime` receiver is `UnityEngine.Time` | 5/5 sites bind to the metadata type. Tuple field/property, source class (any file), nested type, alias to a source class, ambiguity: all fail `[receiver]`. |
| 3 | Every reviewed `DisplayStyle.Flex` / `.None` receiver is the `UnityEngine.UIElements.DisplayStyle` enum | 22/22 sites bind to the metadata enum. Tuple field, source enum, source alias: fail `[receiver]`. |
| 4 | Expected sites/counts; fail closed | 27 pinned sites. Silent missing site (X328-N11) fails `[site count]`; extra member (X328-N12) fails; compile error (X328-N13) fails; second root type (X328-N09) fails `[root]`; ambiguity (X328-N10) fails `[compile]` + `unresolved or ambiguous`; 18 degradation checks in section C all fail closed. |
| 5 | A326-N06/N07/N08, L326-U01 and S05/S06/S07 fail the full checker for the intended reason | See differential table. |
| 5 | All former 884 negatives / 112 benign, 47 N-1 + 10 N-2, historical 6/4/2, candidate 10/0/2, genuine .NET 8 SnapFar | See preserved gates. |
| 6 | No new lexical parser, no production redesign, no speculative framework | `objective_cta_source_check.py` is byte-identical. The only new C# is the one 233-line reporter. |

### Differential: the published #325 checker vs this checker, through the REAL `check_baseline.py` (post-fix states, disposable sparse clones, each case restored and the clone re-verified clean)

| Case | #325 `c83fae1a` | This branch `288ba386` | Intended cause (new) |
|---|---|---|---|
| **A326-N06** root tuple FIELD `Time` | **exit 0, 10/0/2 (FALSE GREEN)** | **exit 1, 9/1/2, only FAIL 08** | ``receiver `Time` binds to Field ScenarioOneUiRoot.Time (source in this project)`` |
| **A326-N07** root tuple PROPERTY `Time` | **exit 0, 10/0/2 (FALSE GREEN)** | **exit 1, 9/1/2, only FAIL 08** | ``… binds to Property ScenarioOneUiRoot.Time …`` |
| **A326-N08** root tuple `DisplayStyle` | **exit 0, 10/0/2 (FALSE GREEN)** | **exit 1, 9/1/2, only FAIL 08** | ``receiver `DisplayStyle` binds to Field ScenarioOneUiRoot.DisplayStyle …`` |
| **L326-U01** uncollected `ZClock.cs` global `Time` | **exit 0, 10/0/2 (FALSE GREEN)** | **exit 1, 9/1/2, only FAIL 08** | ``receiver `Time` binds to NamedType Time (source in this project)`` |
| R321-S05 / S06 / S07 | exit 1, 9/1/2, only 08 | exit 1, 9/1/2, only 08 — **lexical cause preserved** (`F-2 name binding`; `using alias 'MonoBehaviour' shadows …`) | stage 1 still rejects first |
| Benign A326-B01, A326-B03, A326-B04, X328-B05 (unrelated root tuple member `Clock`), X328-B06 (unrelated new file) | exit 0, 10/0/2 | **exit 0, 10/0/2** (no over-rejection) | — |

The old checker reproduces all four #326 false greens, so these negatives can fail. S05/S06/S07 are also rejected by Roslyn **on its own** (section B), proving the semantic stage is an independent second line and not just riding the lexical one. The five witness files are **byte-identical to the #321 originals** (5/5, `E/out/proof/proof.json` → `witness_fidelity`).

### The new test suite — `test_objective_cta_roslyn_binding.py`, exit 0, "ALL CHECKS PASSED" (~4 min)

| Section | Content | Result |
|---|---|---|
| A rules (no .NET) | `evaluate()` on synthetic reporter output; each rule has a failing and a passing input | 20 negatives, each rejected for its own tag; expected report accepted |
| B semantic (real Roslyn) | overlays on the real source set: 3 witnesses, 4 #326 cases, 13 further rebinding/tooling negatives | **20 negatives each rejected for its intended cause; 8 benign overlays accepted** (incl. `UnityEngine.Time.frameCount` fully qualified, `using Time = UnityEngine.Time;`, `using static UnityEngine.Mathf;`) |
| C fail-closed | the degradations listed above, with real tampered reference copies and a real wrong SDK pin | 18 checks, all completed failures |
| D end-to-end | the real `check_baseline.py` in a disposable clone | candidate 10/0/2 exit 0; **7 negatives** exit 1, 9/1/2, only 08, intended cause; **5 benign** 10/0/2; historical 6/4/2 |

### Preserved gates

| Gate | Result | Exit |
|---|---|---|
| Unchanged `test_objective_cta_source_check.py` | **884/884 negatives rejected, false greens 0, wrong cause 0**; benign **32 + 32 + 48 = 112/112**; N-1 author 21/21 and N-2 author 12/12 (the 47 N-1 and 10 N-2 witnesses are inside the 884 groups); probes 9+9 and 52 unchanged; #325 repaired 3/3, nearby 5/5, benign 5/5 | 0 |
| `py_compile` of all five Python files | clean | 0 |
| Candidate normal checker, default cwd (SDK 10 default for the SnapFar step) | **10 PASS / 0 FAIL / 2 NOT-TESTABLE** | 0 |
| Candidate, cwd pinned to SDK 8.0.133 with `COREHOST_TRACE` | 10/0/2; stdout byte-identical to the default-cwd run; hostfxr trace: all 3 `Using .NET SDK dll` lines are `/usr/lib/dotnet/sdk/8.0.133/dotnet.dll`, zero `sdk/10.0.114/dotnet.dll`. **Six distinct production `RackEditing.SnapFar` cases PASS** | 0 |
| Candidate, repeat run | stdout byte-identical (deterministic) | 0 |
| Historical `72b4efbd…` with this checker, pre-fix states | **6 PASS / 4 FAIL (01, 02, 03, 08) / 2 NOT-TESTABLE**; stdout **byte-identical to the published #325 checker's** on the same source; stage 2 never reached ("Roslyn" absent) | 1 (intended) |
| `baseline-output.txt` | unchanged (blob `db6a8b85…`) | — |

## Root and reference identity

| Item | Value |
|---|---|
| .NET SDK | `8.0.133`, pinned by `global.json` (`rollForward: disable`) in a temp folder; hostfxr-traced |
| `Microsoft.CodeAnalysis.dll` | `124939a14172f874777c37405e9fc9aefa8a5fda301a15f7feea3d96ca822346` (4.8.0.0) |
| `Microsoft.CodeAnalysis.CSharp.dll` | `e91e2d1b62e5cf3b54369cab3a2d4055a7bcf9769c3a3182a65ae6a7e0941977` |
| Reference manifest | `978123241d3d6ff18973a0e9d486c0e20f4da30f5c2c7c5d3e2947c6fb3a18f1`, 320 rows |
| Reference copies | `/media/jer/Files/CCF/Verification/CTA325-CompanionNameRepair-20261107/refs` (`NNN-<file>`); **320/320 SHA-256 match** on every run; override with `CCF_CTA_ROSLYN_REFERENCES` |
| Reference origin | 182 `UnityEngine` + 103 NetStandard shims + 1 netstandard under Unity 6000.6.0f1 `Editor/Data/`, and 34 ZX20 `CCF-MainIntegration-20261104/Library/ScriptAssemblies` project DLLs. **Read-only copies only; nothing was read from or written to ZX20 by this task.** |
| Language / defines | C# 9; `UNITY_6000_6_OR_NEWER;…;ENABLE_INPUT_SYSTEM;UNITY_STANDALONE;UNITY_STANDALONE_LINUX` |

## Baseline limits (read before relying on this)

- **Not Unity compilation and not native-UI proof.** It is the SDK 8.0.133 C# 9 compiler on one `Assembly-CSharp` model. Unity's compiler, analyzers, source generators, asmdef graph and import pipeline are not run. A real Unity compile of this candidate is **NOT RUN**.
- Only `ScenarioOneUiRoot.cs` is judged. The same names in `LearningObjectivesView*.cs` and elsewhere are checked by neither stage. Pinned counts check how many reviewed accesses exist, not that each sits in the intended method.
- `#if` regions follow the module's defines only (Player-like, no `UNITY_EDITOR`).
- A hash-identical reference set is trusted. A different Unity version needs a new manifest. The check is **machine-dependent** by design: it fails closed where the SDK, compiler or reference folder is absent, so a cloud/code-only worker cannot pass check 08.
- Pins (27 sites, base type, hashes) fail on a deliberate edit until updated, like every other pin in this checker.

## Process disclosures

1. **Model:** Sonnet 5.5, not the recommended Sol High (above).
2. **Test bug found by re-running on the committed tree.** My first full set ran at `5f304408`. Re-running the suites on the committed files exposed a test-only defect: the historical step needs the tool folder to exist after checking out the baseline SHA, which is only true while the tool files are untracked. I added the one `mkdir` line (commit `288ba386`; `test_objective_cta_roslyn_binding.py` only; product files byte-identical) and **re-ran everything at `288ba386`**. The earlier outputs are kept, labelled superseded.
3. **One stray file:** a bug in one of my own test fixtures wrote an 8-byte file named like the defines string into the tool folder once during development. I removed it before the first commit; it was never staged.
4. **One leftover compiler server:** the pre-existing SnapFar step (`dotnet run`, shared compilation) left one `VBCSCompiler` (pid 3136202, parent 1, started 19 s earlier by my own run). I killed exactly that PID. All later runs set `UseSharedCompilation=false` and left no process.
5. **A326-N04 / N05 (`using static` with a `Time` / `DisplayStyle` property):** with this SDK these produce `CS0229` ambiguity compile errors, so they are rejected as `[compile]` plus `unresolved or ambiguous`, not as valid-C# false greens. Reported as observed; no claim about other compilers.
6. No `strace` is installed here, so SDK evidence is the hostfxr trace, not the #325 syscall trace.
7. Disposable sparse clones used by the proof driver are under `E/clones/` (130 MiB for all of `E/`); no cleanup of any other path was done.

## Reproduce

```bash
cd /media/jer/Files/CCF/Operations/CCF-CTARoslyn-20261107          # at 288ba386 (or any checkout of this branch)
export UseSharedCompilation=false MSBUILDDISABLENODEREUSE=1 DOTNET_CLI_TELEMETRY_OPTOUT=1 PYTHONPYCACHEPREFIX=<outside the worktree> TMPDIR=<outside the worktree>
python3 -B Tools/Verification/OvernightPlaytestFeedback/check_baseline.py --rotation-state post-fix --instruction-state post-fix   # 10/0/2, exit 0
python3 -B Tools/Verification/OvernightPlaytestFeedback/objective_cta_roslyn_binding.py     # toolchain + per-site evidence, exit 0
python3 -B Tools/Verification/OvernightPlaytestFeedback/test_objective_cta_roslyn_binding.py   # all four sections, ~4 min
python3 -B Tools/Verification/OvernightPlaytestFeedback/test_objective_cta_source_check.py     # 884/884 and 112 benign, ~4.5 min
```

The proof driver (`E/proof_stage_a.py <sha>`) builds the old/new/historical disposable clones and runs the differential; it is evidence tooling, not part of the repository.

## Status

**Author claim: SOURCE-ONLY candidate, qualified by the evidence above at `288ba386`.** The two #326 false-green families (tuple-typed root member, uncollected file) are closed by symbol binding, not lexical guessing; the three #321 witnesses and all earlier controls are preserved. Not approved. Not Unity-verified. Independent review (#329) and the Manager's G2/G3 verdict are required. Stage B (Workbench R324-01) is a separate branch and worktree and is reported in its own document.
