# #329 — independent exact-candidate source review

**Review execution: COMPLETED, SOURCE-ONLY. CTA: NEEDS CHANGES. Workbench: APPROVED at SOURCE-ONLY review level.** These are independent reviewer recommendations for Manager #200 G2/G3 disposition, not Manager acceptance, G4 approval, integration or Unity qualification. A329-01 below is an in-scope blocker. No competing fix was authored.

Binding packet: [Manager exact-candidate dispatch](https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Morning-Closure-20261107/MANAGER-329-EXACT-CANDIDATE-INDEPENDENT-REVIEW-DISPATCH-20261107.md). [Actual RUNNING receipt](https://github.com/jermur788/CCF/issues/329#issuecomment-6714386782). [Adverse CTA finding receipt](https://github.com/jermur788/CCF/issues/329#issuecomment-6714448086).

## Identity and independence

Reviewer: T3 Code / Codex harness / **OpenAI gpt-6.1-sol, Medium reasoning**, the observable runtime setting. This differs from the packet's recommended High and was disclosed before review. Actual author: Anthropic Claude Sonnet 5.5; this reviewer is a separate, cross-provider session and used no delegated agents. No capacity/quota telemetry was available.

| Item | Exact identity |
|---|---|
| CTA governing #325 base | `c83fae1af4ac1449cba0304e44b0df49b91f8919` |
| CTA tested source | `288ba3863793e3775327fb7322dc06af28f7835a` |
| CTA reviewed publication | `6adb20318b80e8a81560bc58a63c8bdbfa15d7ca`, `fix/cta-roslyn-symbol-binding-20261107-v1` |
| Workbench governing #323 base | `2c4aea1c94a0768c684376bc027ba15c5af5146f` |
| Workbench tested source | `a4019e6b96b50e03d2b71d92c44fc30a0f7ace29` |
| Workbench reviewed publication | `03b484d10b36382ab86d5e345ece9acac96c9620`, `fix/workbench-receipt-finalization-20261107-v1` |
| Reviewer docs base / starting HEAD | freshly verified HTTPS main `4cc3e620b08d1646b9f7b7756a845409df20fc08` |
| CTA source canonical-context snapshot | `81867c8940d7d07c952e8e49ca4a0cb93498e790` |
| Workbench source canonical-context snapshot | `63044dfbbeb869996ceecec8a47619dcba048b0b` |
| Reviewer branch | `docs/verifier-roslyn-receipt-independent-20261107-v1` |
| Reviewer worktree | `/media/jer/Files/CCF/Operations/CCF-VerifierClosureReview-20261107` |
| Common Git store | `/media/jer/Files/CCF/Repository.git` |
| Evidence root (`E/` below) | `/media/jer/Files/CCF/Verification/VerifierRoslynReceiptReview-20261107/` |

Preflight read live GitHub coordination and #329, checked remote source tips, Git common directory/worktree registrations, branch/path uniqueness, tracked/untracked/ignored cleanliness, processes, filesystem and free space. No competing reviewer claim or Unity process was found. The new docs lane started clean. Files is ext4 `/dev/sdb4`, about 32 GiB available on entry and 31 GiB after setup. No ZX20 working set or Library was created. The primary lane's pre-existing `AGENTS.md` and `Docs/Operations/StorageArchitecture.md` edits and ignored context mirrors were preserved. Author source lanes were inspected read-only and clean; the protected registered ZX20 Workbench lane was not accessed.

**Publication identity PASS:** `git diff 288ba386 6adb2031` adds only CTA author handoff Markdown; `git diff a4019e6b 03b484d1` adds only Workbench author handoff Markdown. All tested source bytes are identical at each publication head. Nothing was repinned to main.

## A — CTA decision: NEEDS CHANGES (G2/G3, source-only)

The repair genuinely closes the known name-binding false greens. It does **not** close the required method/site identity boundary: an Objectives presentation update can be moved into an uncalled method while the complete checker remains green. This is a measurement-gate defect, not a demonstrated regression in the unchanged production candidate.

### A329-01 — Objectives visibility site can be relocated out of Update, false green

**Severity: major for this source gate; blocks source approval under dispatch Stage A items 2 and 5.** Exact reviewed source: `6adb20318b80e8a81560bc58a63c8bdbfa15d7ca`. The candidate itself is not altered; this witness exists only in reviewer disposable clone `E/cta-site`.

Minimal mutation in `Assets/ForestPrototype/UI/ScenarioOneUiRoot.cs`:

```diff
@@ Update(), original line 235
-        learning.Root.style.display = !preview && screen == UiScreen.Objectives ? DisplayStyle.Flex : DisplayStyle.None;
+        // reviewer: visibility update moved into an uncalled method
@@ immediately before ShowObjectives(), original line 693
+    private void ReviewUnusedPanel(bool preview) { learning.Root.style.display = !preview && screen == UiScreen.Objectives ? DisplayStyle.Flex : DisplayStyle.None; }
     public void ShowObjectives()
```

Reproduce from a disposable checkout of the pinned CTA publication, apply exactly that mutation, then run:

```bash
python3 -B Tools/Verification/OvernightPlaytestFeedback/check_baseline.py \
  --rotation-state post-fix --instruction-state post-fix
```

**Actual:** process exit **0**, `SOURCE-STATIC TALLY PASS=10 FAIL=0 NOT-TESTABLE=2`; check **08 PASS**; Roslyn evidence says **0 compile errors**, metadata base/bindings correct, and the same **27** sites. **Required:** check 08 rejects the relocated Objectives presentation site. Raw files: `E/out/cta-site-relocation.{patch,stdout,stderr,exit}`. Patch SHA-256: `0dc449573e5e5260b3f15b8557363261a00713b8e6bb97bfd632e817e95f91ba`; stdout SHA-256: `f9f0e3f964044442e1568d89135da2f42b478f05fb29c504e168f41abec6fdf8`. Both are also in the evidence index.

Why this matters: the original code updates the Objectives panel's display every `Update()`. After the mutation, the statement only exists in a private method with no call sites, so this per-frame panel update no longer executes. The exact statement text, receiver symbols and counts survive. The lexical `_ROOT_STATEMENT_SNIPPETS` permits the statement anywhere within the owning root class; the Roslyn reporter walks the entire declaring file and the Python evaluator checks a multiset, without enclosing-method identity. This is a concrete panel-site gap, not an arbitrary C# parsing curiosity. No claim of rendered/runtime reproduction is made.

Required next step: Manager disposition and a bounded author response that protects this actual method/site association. The reviewer did not implement a repair, change the accepted gate or start a further mutation campaign after establishing the finding.

Changed CTA verification paths against #325: `ObjectiveCtaRoslynBinding.cs`, `objective_cta_roslyn_binding.py`, `roslyn-reference-manifest.txt`, `test_objective_cta_roslyn_binding.py`, `check_baseline.py`, `README.md`, all under `Tools/Verification/OvernightPlaytestFeedback/`; plus the author result Markdown.

### Verified positives and preserved boundaries

| Check | Independent result |
|---|---|
| SDK / compiler | SDK **8.0.133** pinned with `global.json`, `rollForward: disable`. Compiler DLL hashes below independently recomputed. Wrong/missing SDK/compiler cases fail closed in the real suite. |
| Reference identity | Manifest SHA-256 independently verified; **320/320** copied references match every row; no reference symlinks; suite rejects missing, extra and corrupt references and altered manifest. |
| Baseline compilation model | **189** player-like Assets C# files (source-set SHA-256 `e539430679f2edb2e67e50b1083e124c3b420b065eadfd6b3896a653a7376297`), C#9 and the declared standalone-Linux/New Input System defines; **0** errors, one global root with metadata `UnityEngine.MonoBehaviour` base. |
| Receiver/member identity | All **5** Time and **22** DisplayStyle sites are semantically bound, not merely name strings: metadata `UnityEngine.Time` properties and metadata `UnityEngine.UIElements.DisplayStyle` fields, with metadata assembly identity. |
| Required known witnesses | Suite reconstructs **A326-N06/N07/N08**, **L326-U01**, **R321-S05/S06/S07**; real full checker rejects all seven as **9/1/2**, only **08**, for intended semantic or historical lexical cause. |
| Separate reviewer reconstruction | Tuple field/property/DisplayStyle and global clock witnesses independently reconstructed in `independent_cta.py`; all four are **valid C#9, 0 compile errors**, and each complete checker run exits **1**, **9/1/2**, semantic check 08 failure. A benign unrelated root alias remains **10/0/2**, exit 0. |
| Additional bounded inspection | Metadata-like source `UnityEngine.Time` namespace collision is compile-clean and rejected semantically; benign alias accepted; actual compile-error fixture rejected. Local-tuple fixture has CS0841 and is explicitly **not counted** as a valid false-green witness. |
| Published Roslyn suite | Exit **0**: 20 synthetic rule negatives; 20 semantic negatives / 8 benign; 18 fail-closed checks; real end-to-end candidate **10/0/2**, seven negatives **9/1/2** only 08, five benign **10/0/2**, historical **6/4/2**. |
| Old lexical suite | Exit **0**, **884/884** negatives rejected, false greens **0**, wrong causes **0**; benign **32 + 32 + 48 = 112/112** accepted. Preserved earlier N-1/N-2 families remain covered by the unchanged suite (including the prior 47+10 witness scope); new-source author groups 21/21 and 12/12 retained. |
| Genuine SDK8 SnapFar | Explicit SDK8-cwd full checker exit **0**, six distinct production `RackEditing.SnapFar` cases PASS, candidate **10/0/2**. Host trace contains three SDK selections, all `/usr/lib/dotnet/sdk/8.0.133/dotnet.dll`. |
| Source comparability | Lexical checker, its old suite and `baseline-output.txt` are byte-identical to #325. Six verification paths changed from #325, plus author report; no production/package/serialized change. `git diff --check` PASS. |

Independent pins:

- Manifest: `978123241d3d6ff18973a0e9d486c0e20f4da30f5c2c7c5d3e2947c6fb3a18f1`.
- `Microsoft.CodeAnalysis.dll`: `124939a14172f874777c37405e9fc9aefa8a5fda301a15f7feea3d96ca822346`.
- `Microsoft.CodeAnalysis.CSharp.dll`: `e91e2d1b62e5cf3b54369cab3a2d4055a7bcf9769c3a3182a65ae6a7e0941977`.

The copied reference directory is an **external machine dependency**, `.../CTA325-CompanionNameRepair-20261107/refs`, not bundled or portable. Its 320 hashes were verified; original provenance remains the disclosed Unity 6000.6.0f1 Editor DLLs plus copied project ScriptAssemblies. No package was installed or downloaded by this review. Source shows no declared NuGet package dependency; network traffic itself was not instrumented. Absence of the external references cannot be called cloud PASS.

The guard models one player-like `Assembly-CSharp`, not Unity's import/assembly graph, generators, analyzers, Editor branches or all target configurations. It rejects explicit unsupported topology, but only root-file symbols are judged. LearningObjectivesView and other production type internals are not semantically certified. The source-set hash is evidence, not an enforced immutable source hash: benign changed sources are intentionally accepted. Counts detect missing/extra sites but do not enforce their method identity; A329-01 demonstrates the material limit. All native UI/runtime behaviour remains **NOT RUN**.

Author provenance disclosure checked: the tested CTA commit adds one test-only `mkdir` correction over `5f304408`; it does not change implementation bytes. The full published suite passed here at source-identical publication head. Superseded author outputs were not used as current test results.

## B — Workbench decision: APPROVED (G2/G3 recommendation, source-only)

No in-scope blocker found in the bounded finalisation repair. The surviving early marker is conspicuously incomplete, including when both JSON publication and fallback append fail. The extra FINAL/helper-authority controls directly cover failure states added by this protocol and are within the review scope.

| Check | Independent result |
|---|---|
| Complete runner suite | Exit **0**, **41/41** tests OK, no skips; includes original 13 mocks, real-Git drift, R1–R5 controls and six FINAL-receipt controls. |
| Ordinary success | Real runner logic under mocked Editor/lease returns **0**; JSON exit 0, empty failures; marker's **first line** exactly FINAL/runner_exit=0 and SHA-256 of those JSON bytes. Helper returns authoritative success. |
| Simulated simultaneous ENOSPC | Independently faulted JSON `os.replace` and fallback append: **exit 1**, no JSON, surviving marker PROVISIONAL/INCOMPLETE, quoted raw Editor PASS/clean only; no bare PASS/clean line. |
| Real permission refusal | After the real provisional publication, reviewer fixture directory chmod **0555**, marker **0444**; genuine `PermissionError`, **exit 1**, no JSON, only provisional marker. Executed as **uid 1000**, not root; permission control was not skipped. |
| Third/final write failure | Independently faulted final rename, with both append-success and append-failure variants: process **1**; earlier on-disk JSON may still say **0**; marker stays provisional and helper rejects it. No cross-file atomicity claim. |
| Receipt authority negatives | Missing/unreadable JSON, corrupt JSON syntax, altered hash/bytes, stale or non-final marker, bare old-style PASS/clean rejected. Published controls also test mismatched JSON bytes and missing marker. Caller must separately require process exit 0. |
| Old R324-01 differential | Same independent ENOSPC shape against #323: **exit 1 but bare Editor PASS and clean-status lines survive**, reproduced in `old_wb_receipt.py`. Current candidate survives only as conspicuously incomplete. |
| R1 / P7 differential | Independent tracked package/material-byte mutation fixture: #318 returns **0**, failures empty; #323 and this #328 candidate return **1**, `postrun_lane_drift:2` and both path/hash diagnostics. Runner suite also exercises a real temporary Git repository. No protected lane is used. |
| R3 | Exact terminal token checks, duplicate/near-miss/failure cases and source-table matching retained in runner suite. Raw Editor PASS remains distinct from runner terminal success. |
| R4/R5 and 28 projections | Exit **0**, **28** real offline C# projection controls, historical schema boundaries, synthetic labels with old-source negative control, **6** teaching controls and wiring checks. Only reviewer clone's output-directory allow-list was temporarily redirected, then restored byte-for-byte. |
| Historical equality | Read-only `compare_projection.py 6b7d2d45`: both `HistoricalLayout` methods, 28-control C# block and historical schema audit identical; **PROJECTION_IDENTITY_PASS**. This is block identity, not whole-harness identity to #318. |
| Test preservation | All **46** pre-existing #323 test-class members unchanged; original 13 #318 mock bodies unchanged. No old test was weakened. |
| Scope | Only `run_workbench_retirement.py` and `test_runner.py` changed from #323, plus author report. Relevant projection/historical sources unchanged from #323; no Assets/Packages/ProjectSettings/.vscode/Reference/save change. |

The provisional file has explicit first and last incomplete lines; all raw log/status lines are prefixed `PROVISIONAL> `. The final form restores the old raw body only after JSON publication, under a new first-line authority record. `authoritative_success()` checks the matching JSON gate, exit 0/no failures and exact first-line hash. It cannot establish the caller's process exit or a particular externally requested run identity on its own; a complete old pair is not proof of a new process invocation. The runner refuses an already existing output directory. No universal replay/transaction/power-loss guarantee is inferred.

Repository and deployed CCF-Ops source searches found no consumer parsing this runner's marker format beyond the runner/tests; other qualification runners own separate formats. Unknown external readers must honour the changed first line and complete process/JSON/marker conjunction. No consumer was changed by this reviewer.

Author's final projection-driver substitution is a one-element tuple; the two disclosed earlier malformed attempts remain superseded. Reviewer reran the actual controls with its own explicit, retained allow-list substitution rather than accepting those outputs. No installed Unity runtime or original registered lane execution is claimed.

## Commands, exits and evidence trust

Everything below ran in reviewer-owned Files evidence clones or fixtures. `run_review.py` records exact suite commands, cwd, exits and durations in `E/out/commands.json`. All Python runs used bytecode suppression or an external cache; temporary directories were under `E/tmp`, shared compiler reuse disabled. No Unity command was executed: mock harnesses replace the Editor/lease/licence boundary with fake fixtures.

| Command / driver | Exit | Main retained evidence |
|---|---|---|
| `python3 -B Tools/Verification/OvernightPlaytestFeedback/test_objective_cta_roslyn_binding.py` | 0 | `out/cta-roslyn-suite.{stdout,stderr,exit}` |
| `python3 -B Tools/Verification/OvernightPlaytestFeedback/test_objective_cta_source_check.py` | 0 | `out/cta-lexical-suite.*` |
| SDK8 cwd: `python3 -B <cta>/Tools/Verification/OvernightPlaytestFeedback/check_baseline.py --rotation-state post-fix --instruction-state post-fix` | 0 | `out/cta-sdk8.*`, `out/sdk8-host-trace.txt` |
| `python3 -B independent_cta.py` | 0 | `out/independent-cta-report.json`, per-witness `out/independent-cta-*.{stdout,stderr,exit}` |
| Same complete checker, site-relocation clone | **0 (false green)** | `out/cta-site-relocation.*`, exact patch above |
| `python3 -I -B Tools/Verification/WorkbenchRetirement/test_runner.py -v` | 0 | `out/wb-suite.*` |
| `python3 -B independent_wb.py` | 0 | `out/independent-wb-*.json`, ordinary/ENOSPC/permissions/final/combined faults |
| `python3 -B old_wb_receipt.py` | 0 (old defect reproduced) | `out/old-wb-receipt.json` |
| `python3 -B p7_compare.py` | 0 | `out/p7-compare.json` |
| `python3 -I -B <wb>/Tools/Verification/RackV2Persistence/test_historical_projection.py --out <E>/projection` | 0 | `out/wb-projection.*`, `out/projection-allowlist-substitution.txt` |
| Read-only prior `compare_projection.py 6b7d2d45`, cwd `E/wb` | 0 | `out/projection-identity.*` |
| Independent SHA/reference and test/body comparability inspection; candidate `git diff --check` | 0 | `out/independent-toolchain-identity.json`, `out/comparability.json` |

Reviewer fixture disclosure: first independently reconstructed global clock lacked the additional Time properties used by other production files and caused compile errors. That attempt stopped before mutating any clone and is retained as `out/superseded-fixture-independent-cta*`; it is **not** used as a compile-clean witness. The corrected fixture has 0 compile errors and full-checker intended-cause failure. An additional local-tuple experiment is compile-invalid and not counted as a valid witness. Routine shell cwd mistakes in invoking the P7/identity drivers were corrected before execution; no author files were touched.

The site-relocation clone intentionally retains its two-line production mutation as review evidence; it is not a task/source worktree or integration candidate. Other normal test clones are clean after their runs. Raw evidence is local durable Files data; the GitHub handoff embeds the blocker and reproducible command so the finding does not depend on access to that disk. Evidence index: `E/out/evidence-index.json`.

## Remaining gates and handback

**Manager #200 decision required:** CTA **NEEDS CHANGES**, Workbench **SOURCE-ONLY APPROVED recommendation**. No G4 integration approval is issued. Overall review execution is complete with an adverse CTA result; #328 is not re-authored or reopened here.

**NOT RUN:** all Unity import/compile/runtime gates, Workbench C–G and native former-site checks at 720p/1080p, frozen Legacy40/Reference save/load, RackV2 runtime/cold-start, Enlarged80 runtime, integrated #315 A–O (15 gates), Windows/Player builds and any player acceptance. Frozen/reference/save anchors were preserved by source identity only, not re-emitted. #287 stays NOT UNITY QUALIFIED; #315 stays blocked. Package/material provenance, protected eight dirty paths, B3 tree replacement and storage/registered-lane admission remain unresolved and unchanged.

**Files published by reviewer:** this handoff only. No production, test, package, project settings, serialized asset or canonical coordination edit. Source lanes remain read-only. Reviewer lane is released after publishing and #329 HANDED-OFF; retained Files evidence is not cleaned.

**Dashboard synchronization: NOT VERIFIED / Manager follow-up required.** Reviewer has no authority to change the Manager-only live coordination record. The result is returned directly to #200 for its authoritative record update, dashboard ingestion/audit advancement and published-state verification. No generated dashboard file was edited and GitHub publication alone is not claimed as dashboard synchronization.
