# #331 — Independent #330 Objectives Update-site source review

**REVIEWER OPINION: NEEDS CHANGES (SOURCE-ONLY).** A329-01 is genuinely closed for its method-ancestry cause. One new, bounded benign-source regression, **A331-01**, prevents approval under packet item 2: an inline comment in the assignment target makes the real checker reject the otherwise identical, correctly bound statement in the actual `ScenarioOneUiRoot.Update()`.

This is independent review evidence, not Manager G2/G3 acceptance, integration, Unity qualification or player/runtime evidence. The reviewer made no repair and approves neither their own review nor the author's implementation as Manager.

## Identity, authority and exact candidate

| Item | Observed value |
|---|---|
| Reviewer | **OpenAI GPT-6.1 Sol**, provider instance **ChatGPT - OpenAI B** (`codex_c75d8114-7118-4224-8bf2-b9b62637e1fb`), **reasoningEffort high**, T3 Code through the Codex harness; actual selection independently read through `t3_thread_configure` and `orchestrator_capabilities` |
| Independence | This reviewer did not author #330. Author is the separate **Xiaomi MiMo-V2.6-Pro** session (`xiaomi-token-plan-ams/mimo-v2.6-pro`); its effort was not exposed and is not inferred. Cross-provider independence is genuine. |
| Account | `gh api user --jq .login` returned `jermur788` |
| Clock | Host `date -u`; first inspection **2026-11-08T00:46:37Z**, lane admission / RUNNING **2026-11-08T00:48:41Z**; completion recorded below |
| Binding packet | `MANAGER-330-CTA-UPDATE-SITE-INDEPENDENT-REVIEW-PACKET-20261107.md`, read completely from fetched main; retained as `E/out/binding-review-packet.md` |
| Binding adverse finding | A329-01 in `VERIFIER-CLOSURE-INDEPENDENT-REVIEW-RESULT.md` at `2b608ff3dd5a1fd94c6860aaf47bacbf1c76098b` |
| Candidate branch | `fix/cta-objectives-update-site-20261107-v1` — read only |
| Tested source | `990ec43ec6bf5ec7063fb530f80b82e37501c582` |
| Published candidate | `9867144c30d059433fb124aa016af6a5b20e1085` |
| #328 base | `6adb20318b80e8a81560bc58a63c8bdbfa15d7ca` |
| #325 base | `c83fae1af4ac1449cba0304e44b0df49b91f8919` |
| Candidate canonical context | `81867c8940d7d07c952e8e49ca4a0cb93498e790` |
| Reviewer docs branch | `docs/cta-update-site-independent-20261107-v1` |
| Reviewer docs base | Fetched `origin/main` **`c0513d6ac84dce4750747596bd9dacfddfd2da36`**, not a substitute candidate |
| Reviewer docs canonical context | `4790c20918ca2a667b04d80262142d9b4151b60e` |
| Reviewer lane | `/media/jer/Files/CCF/Operations/CCF-CTAUpdateSiteReview-20261107` |
| Common store | `/media/jer/Files/CCF/Repository.git` |
| Evidence root (`E`) | `/media/jer/Files/CCF/Verification/CTAUpdateSiteReview-20261107/` |

No quota or unexposed author effort/timing is invented. The author's handoff contains an imprecise `00:3xZ` clock endpoint; it is not used as a measurement. Git reports source/publication commit timestamps **2026-11-08T00:44:29+00:00** / **00:44:36+00:00**; the live author HANDED-OFF receipt was **00:45:11Z**.

## Admission and isolation

Fetched the canonical store before admission; refreshed live coordination and issues #330/#331. #330 was actually HANDED-OFF; #331 was open with no competing comment. Checked worktree registrations, source branch/head, author clean status, branch/path uniqueness, authenticated account, mount and capacity. Both proposed reviewer branch/path and evidence root were absent; nothing was reset, cleaned, force-pushed or reused. Created exactly the authorised reviewer worktree with:

```bash
git -C /media/jer/Files/CCF/Repository.git fetch origin
git -C /media/jer/Files/CCF/Repository.git worktree add \
  /media/jer/Files/CCF/Operations/CCF-CTAUpdateSiteReview-20261107 \
  -b docs/cta-update-site-independent-20261107-v1 \
  c0513d6ac84dce4750747596bd9dacfddfd2da36
```

The new lane started clean. Files was verified as **ext4 `/dev/sdb4`**, approximately **27 GiB available** at entry; all scratch/evidence remained on Files. The harness-provided starting checkout was clean. The pre-existing protected dirty files/lane were not cleaned or modified. The author worktree and other named lanes were not written; `active-tasks.md` remains Manager-owned and untouched.

[RUNNING receipt](https://github.com/jermur788/CCF/issues/331#issuecomment-6715245496). Retained preflight and live coordination: `out/preflight.txt`, `out/live-coordination-at-admission.md`, `out/context-identities.txt`.

Reviewer-owned sparse/shared disposable clones: `E/scratch/candidate` at exact tested source, `E/scratch/mutation` at the same source, `E/scratch/old` at exact #328 base. Only these disposables received C# mutations. Global SDK pins were placed in `E/global.json` and `E/sdk8/global.json`; nested suite clones/temporary directories inherit the SDK pin. No Unity process, lease, import, new Library or playable-build script was started.

## A329-01 transition — PASS, exact witness now rejected

Applied #329's retained patch verbatim to both old and new disposable sources. `git diff` of the applied mutation has SHA-256:

`0dc449573e5e5260b3f15b8557363261a00713b8e6bb97bfd632e817e95f91ba`

This is byte-identical to the binding witness, not merely similar mutation text. It replaces the statement at original line 235 with the review comment and inserts the unchanged statement in uncalled `private void ReviewUnusedPanel(bool preview)` before `ShowObjectives()`.

| Independently executed source/tool combination | Exit | Actual result |
|---|---:|---|
| #328 checker + exact A329-01 source | 0 | **10/0/2**, check 08 PASS; old false green reproduced |
| #330 checker + exact A329-01 source | 1 | **9/1/2**, only check 08 FAIL; `[update site]` identifies `MethodDeclaration ReviewUnusedPanel of ScenarioOneUiRoot` rather than the actual Update |
| #330 checker + pristine tested source | 0 | **10/0/2**, correct Update-site evidence; all six actual SnapFar controls PASS |

Old false-green stdout SHA-256 independently reproduced exactly:

`f9f0e3f964044442e1568d89135da2f42b478f05fb29c504e168f41abec6fdf8`

The new raw Roslyn report proves **CSharp9**, **320 references**, **189 source files**, **0 compile errors**, and unchanged **27 sites**: `Time.frameCount=3`, `Time.unscaledTime=2`, `DisplayStyle.Flex=9`, `DisplayStyle.None=13`. For A329-01, the sole unmet rule is the Update-site ancestry rule; no inventory mismatch, syntax error or missing reference causes the rejection. Baseline's callable is the real `ScenarioOneUiRoot.Update()`; relocated callable/method is `ScenarioOneUiRoot.ReviewUnusedPanel(bool)`.

Source transition: #328's lexical check pinned statement content somewhere in the root class and its semantic check accepted a file-wide symbol multiset. #330 additionally reports candidate assignments, the root's actual Update members and the nearest callable/method; the bridge checks receiver binding, candidate uniqueness and method/callable ancestry. The same symbol inventory can no longer rescue an uncalled-method relocation. This does not prove general control-flow reachability or that Unity invoked a method.

Raw proof: `out/A329-01.patch`, `out/A329-{old,new}-checker.{stdout,stderr,exit}`, `out/independent-reporter.json`, `out/independent-summary.json`.

## A331-01 — Inline comment causes an in-scope false red

**Severity: moderate; source-review blocker under required benign whitespace/comment variations.** This is a false rejection introduced by the repair, not another A329-01 false green and not a demonstrated gameplay defect.

Minimal single-statement witness in `Assets/ForestPrototype/UI/ScenarioOneUiRoot.cs:235`:

```diff
-        learning.Root.style.display = !preview && screen == UiScreen.Objectives ? DisplayStyle.Flex : DisplayStyle.None;
+        learning./* review trivia only */Root.style.display = !preview && screen == UiScreen.Objectives ? DisplayStyle.Flex : DisplayStyle.None;
```

The comment is C# trivia only: no executable token, method, receiver, condition, symbol, call or count changes. Independent Roslyn results show **0 errors / 189 files / 27 sites**, the same metadata symbols and the same `ScenarioOneUiRoot.learning` field, with the assignment still directly enclosed by the actual `ScenarioOneUiRoot.Update()` method. The unchanged lexical checker separately accepts it.

**End-to-end results:** repaired checker **exit 1, 9/1/2, only check 08 FAIL** for the retained-comment target mismatch; old #328 checker on identical witness source **exit 0, 10/0/2**. The new invocation ran **2026-11-08T00:58:03.591624Z–00:58:25.015573Z**, and the old invocation **00:58:25.017887Z–00:58:52.520251Z**, from the recorded SDK8 cwd. Applicable witness patch SHA-256 is **`aa16a7fcda992d23934f60ca88d1ceef783fecaab1f392cf5568d4b711a5ad7d`**; independent `git apply --check` returned **0** on pristine tested source.

Root cause is narrowly identified at candidate `ObjectiveCtaRoslynBinding.cs:254`: `NormalizedLeft` uses `expression.ToString()` and removes whitespace/`@`, but does not remove comment trivia. Its reported left becomes `learning./*reviewtriviaonly*/Root.style.display`. The bridge's `objective_cta_roslyn_binding.py:276` compares that text against `learning.Root.style.display` and rejects before its otherwise-correct source-field/method ancestry can satisfy the guard. The conditional still finds the candidate, so this is not missing-candidate detection or a compile failure.

The existing benign X330-B11 tests a comment outside the assignment; it does not cover trivia within the target. Whitespace-only target variation and a comment in the conditional both pass, which further localises the new boundary.

**Return to Manager:** retain this exact witness and request a bounded author response preserving benign target trivia while retaining all ancestry and symbol controls. Reviewer supplies evidence only, not a patch or Manager acceptance. No general C# fuzz campaign, additional semantic-gap hunt or source repair loop was started after this finding.

Witness source: `E/overlays/R331-B04-comment-in-target/Assets/ForestPrototype/UI/ScenarioOneUiRoot.cs`; canonical applicable patch and exact checker logs: `out/R331-B04-comment-in-target.git.patch`, `out/R331-B04-comment-in-target-{checker,old-checker}.{stdout,stderr,exit}`. Reproduction from a new sparse disposable candidate clone:

```bash
git apply "$E/out/R331-B04-comment-in-target.git.patch"
cd "$E/sdk8"
TMPDIR="$E/tmp" PYTHONDONTWRITEBYTECODE=1 python3 -B \
  /absolute/path/to/disposable/Tools/Verification/OvernightPlaytestFeedback/check_baseline.py \
  --rotation-state post-fix --instruction-state post-fix
```

The external immutable reference directory remains `/media/jer/Files/CCF/Verification/CTA325-CompanionNameRepair-20261107/refs`.

## Bounded-invariant challenge matrix

All independently authored overlays below were compiled by the actual repaired reporter; every case has **0 C#9 errors**. Unless stated otherwise the exact 27 symbol sites survive. Full-checker outcomes are retained separately, including cases rejected earlier by the unchanged lexical gate.

| Case | Mutation / expected boundary | Observed semantic rule | Real checker |
|---|---|---|---|
| R331-N01 | Exact A329-01 uncalled private method | Sole method-ancestry failure | 9/1/2, exit 1, only 08 |
| R331-N02 | Additional `Update(int)` member, no extra sites | Two Update members rejected, not textual-name satisfaction | 9/1/2, exit 1, only 08 |
| R331-N03 | Uncalled local function inside real Update | Nearest callable `LocalFunction ReviewUnusedLocal` rejected | 9/1/2, exit 1, only 08 |
| R331-N04 | Uncalled anonymous function inside real Update | `AnonymousFunction` rejected | 9/1/2, exit 1, only 08 |
| R331-N05 | Wrong target `map.Root.style.display` | Wrong receiver rejected | 9/1/2, exit 1, only 08 |
| R331-N06 | Same-count statement in another class's `Update()` | `ReviewDecoy.learning` / `ReviewDecoy.Update()` rejected | 9/1/2, exit 1, only 08 |
| R331-N07 | Relocated statement plus comment spelling the correct Update | Comment produces no site; private-method ancestry rejected | 9/1/2, exit 1, only 08 |
| R331-N08 | Clone assignment in an uncalled helper, leaving original | Two candidates rejected; also 29 instead of 27 sites | 9/1/2, exit 1, only 08; lexical duplication cause |
| R331-B01 | Whitespace/newline in assignment target | Accepted | 10/0/2, exit 0 |
| R331-B02 | Newline/escaped `@Update ( )` signature, nested ordinary block | Accepted as actual Update | 10/0/2, exit 0 |
| R331-B03 | Benign unrelated uncalled helper | Accepted | 10/0/2, exit 0 |
| R331-B04 | Inline comment inside assignment target | **False red A331-01** despite correct symbols/Update | 9/1/2, exit 1, only 08; old checker 10/0/2 exit 0 |
| R331-B05 | Inline comment in condition | Accepted | 10/0/2, exit 0 |

Thus the ancestry repair is substantive, not a diagnostic-only change. Same method-name text in comments/another class and same symbol counts elsewhere do not satisfy it. Duplicate identical parameterless declarations would be illegal C#; the valid overloaded-member case challenges member uniqueness without hiding behind a compiler error. The duplicated-assignment case is explicitly not used to prove ancestry-only rejection.

## Preservation and comparability

**Both complete candidate suites independently rerun, exit 0.** No author-reported test is treated as an independent pass. The prior patch/stdout are reused only as immutable comparison inputs, and both were independently reconstructed. No old run substitutes for an affected test:

- **Lexical suite**, **2026-11-08T00:52:37Z–00:59:55Z**: **884/884 negatives rejected, 0 false greens, 0 wrong causes**, all original families retained (including the binding 47+10 families); **112/112 benign** (32+32+48), plus #325 **5/5 benign**, **5/5 nearby** and **3/3 repaired** controls. Historical source rejected. Existing documented lexical over-rejections/limits remain reported by the unchanged suite; A331-01 is new, in the Roslyn path, not one of those grandfathered probes.
- **Roslyn suite**, **2026-11-08T00:49:24Z–00:59:09Z**: A **30** synthetic negatives rejected; B **25** real negatives rejected for intended cause and **10** benign accepted; C **18** fail-closed degradation controls; D actual checker baseline **10/0/2 exit 0**, **11** negatives **9/1/2 only 08**, **7** benign **10/0/2**, historical **6/4/2 exit 1**, Roslyn stage not reached. Final line **ALL CHECKS PASSED**. The named A326/L326/R321 controls all executed in this independent run.

These shipped suites do not contain the new inline-target-comment witness; their green exits therefore do not override A331-01.

Independent `audit_comparability.py` completed **exit 0**. Its exact-file/AST evidence is retained in `out/comparability.json`:

- Lexical checker SHA-256 **`3e62f00862555fd0c63a776c2fac8035e76ba1198cfd0e2b98942a54b1988b3b`**, lexical suite **`56cd2c9524ba0e40d1557bd81f8300838f5a788eecafb11cbd07b1838239dc28`**: identical across #325, #328, #330 tested/publication and #329's retained reviewer clone.
- Manifest **`978123241d3d6ff18973a0e9d486c0e20f4da30f5c2c7c5d3e2947c6fb3a18f1`**: identical across #328/#329/#330. It did not exist at #325; the reference-copy directory's historical name does not imply otherwise.
- Independently hashed all **320** actual reference copies against manifest rows; no symlink accepted. Raw per-file hashes: `out/independent-reference-hashes.json`.
- Original **20** synthetic negative-case ASTs and all **28** old semantic-overlay definitions are unchanged, including `A326-N06/N07/N08`, `L326-U01`, `R321-S05/S06/S07`. Original fail-closed and end-to-end function ASTs are unchanged.
- SDK/compiler hashes, manifest/reference-count, defines, root/base/receiver types and all expected symbol-count pin ASTs remain unchanged. `reference_files`, `build_toolchain`, `topology_problems`, `_is_symbol` function ASTs remain identical; no reference or identity checks were weakened.
- Original root C# SHA-256 **`70fb0333f51a6fd8da4de907b5d6613bb0c4dfd4276ea82ef448d9ba9669536d`**, recorded historical baseline, production `RackEditing.cs` and `RackGeometry.cs` are byte-identical across those applicable source pins.

Actual SDK selection was **8.0.133**; 10.0.114 is also installed but was not substituted. The reporter independently hashes SDK8 compiler DLLs and uses `LanguageVersion.CSharp9`. `out/sdk.txt`, `out/toolchain-facts.json`, retained reporter build/global.json and raw `language/referenceCount` fields establish this path. This is real compilation against trusted copied references, not Unity's compiler, import or Editor state.

Six direct production SnapFar cases were actually executed: near-angle enabled/disabled, outside-angle-window, near-boundary enabled/disabled, outside-boundary-window. All PASS in pristine `out/candidate-checker.stdout`; this proves the bounded offline numeric cases only, not native Shift/input or rendered geometry.

## Scoped-diff audit

`git diff --name-only 6adb2031 990ec43e` is exactly these five allowed paths under `Tools/Verification/OvernightPlaytestFeedback/` (**388 insertions, 11 deletions**):

- `ObjectiveCtaRoslynBinding.cs`: new reporting only — candidate assignment, nearest callable/method, root Update members.
- `objective_cta_roslyn_binding.py`: additional bounded Update-site rules and evidence; original bindings/pins preserved.
- `test_objective_cta_roslyn_binding.py`: additive fixtures/cases; original negative definitions unchanged.
- `check_baseline.py`: **two diagnostic lines only** in check 08; no SnapFar logic change.
- `README.md`: new bounded assertion and correction of the old broad method-identity limitation.

Publication `990ec43e..9867144c` adds **only** `CTA-OBJECTIVES-UPDATE-SITE-REPAIR-RESULT.md` (82 lines); no tested tool bytes change. Parents are exactly publication → tested source → #328 published base, not repinned main or a merge. Both source and publication `git diff --check` returned **0**. Full source diff SHA-256: **`e3175cbe8ce44d6c27e5d78e9c71e8ccf589623cece6f743326a63e7e6ca35cd`** (`out/source-diff.patch`).

No candidate changes to Assets, Packages, ProjectSettings, scenes/prefabs/materials, save data, frozen Reference Future v1, Legacy40, Workbench or PR #300. No integration/merge/main mutation was performed. Reviewer committed only this report on the reviewer docs branch; protected dirty ZX20 files and all named other lanes remain outside the write set.

## Commands, exits and evidence interpretation

All checker commands used `--rotation-state post-fix --instruction-state post-fix`, except the suite's intentionally historical `pre-fix` case. Absolute argv, cwd, UTC start/end and process exits for individual checker invocations are in `out/independent-commands.json`.

| Command / action | Exit | Evidence |
|---|---:|---|
| Exact witness `git apply` on old/new independent clones | 0 | Applied patch hash equals #329 pin |
| Old/new/pristine `python3 -B .../check_baseline.py` | 0 / 1 / 0 | A329 transition table and raw outputs |
| `python3 -B .../test_objective_cta_roslyn_binding.py` | 0 | `out/roslyn-suite.{stdout,stderr,exit,started,ended}` |
| `python3 -B .../test_objective_cta_source_check.py` | 0 | `out/lexical-suite.{stdout,stderr,exit,started,ended}` |
| `python3 -B E/audit_comparability.py` | 0 | Exact-file, original-case/function AST, reference and scope assertions |
| Direct unchanged lexical check on A331-01 | 0 | `out/comment-witness-lexical.*` |
| `python3 -B E/run_independent.py` initial collection | 1 | Reviewer driver used wrong JSON key `receiverText` rather than `receiverName` after storing the complete raw report; **not a candidate failure** |
| `python3 -B E/run_independent.py --resume` | 0 | Corrected evidence summariser only; resumes from retained raw report, then runs actual checkers; no source/tool repair |

The collection-driver exit alone does not certify expectations: A331-01 is a failed benign challenge even though the collector can finish successfully. The original driver error/stderr is retained without laundering it into a test pass. Candidate/author source was never edited to resolve it.

## Verdict and remaining authority

**NEEDS CHANGES (SOURCE-ONLY), reviewer opinion.** A329-01 method relocation is repaired and preserved controls remain intact, but required benign inline-comment variation fails because textual normalisation retains C# trivia. Manager should disposition **A331-01** and route a bounded author response/review rather than treating this opinion as Manager approval. No fix was made by this reviewer.

**NOT RUN:** Unity Editor/import/compilation, licence/lease, native input/focus, rendered UI, player behaviour, saves/Reference/Legacy40 runtime, performance, Workbench Unity C–G, Windows builds and **all integrated A–O gates**. Offline C#9 reporter/SnapFar evidence cannot waive those gates or imply integration/acceptance. #287 is not made Unity-qualified by this review.

Verification completed **2026-11-08T00:59:55Z**; postflight recorded with host UTC clock **2026-11-08T01:01:29Z** in `out/postflight.txt`. Author worktree remains clean and its local/remote source refs remain exactly `9867144c30d059433fb124aa016af6a5b20e1085`; the harness-provided starting checkout remains clean. Files has approximately **25 GiB** available. Evidence is retained, indexed by `E/out/SHA256SUMS`; publication/issue receipts are additionally retained under `E/publication/` after committing this report. The docs lane is released read-only on final #331 HANDED-OFF; no integration is requested or implied.

Coordination/dashboard closure remains Manager-owned; this reviewer does not edit `active-tasks.md` or claim a dashboard publication that was not verified. Manager should synchronise the new NEEDS CHANGES/A331-01 disposition; dashboard synchronisation is an outstanding operational handoff item, not a source-test result.
