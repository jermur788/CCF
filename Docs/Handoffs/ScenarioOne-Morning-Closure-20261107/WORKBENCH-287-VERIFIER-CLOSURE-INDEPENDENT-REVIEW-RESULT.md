# #319 independent review of #318 Workbench verifier closure — result

**Reviewer verdict (SOURCE SCOPE ONLY): NEEDS CHANGES.** The verdict rests on one major runner finding (R1) and three minor ones. The B2/B4 source repairs, the scope, the protection of the eight dirty files and the honest BLOCKED and NOT RUN classifications all pass review.

This is a reviewer opinion with evidence. It is **not** Manager #200 acceptance, not Unity qualification, and not admission of the source/verifier. #287 remains NOT QUALIFIED. Manager #200 alone decides admission, the dirty-package/material policy, any new exact-head Unity lease and future Tier-2 work.

## Review target and identity

| Item | Value |
|---|---|
| Review target branch | `fix/workbench-tier2-verifier-closure-20261107-v1` (origin head re-read: `6b7d2d45a21824f4325bc3c176f7e276dc7299e6`) |
| Source candidate | `fb5eeb98053deab10f922437df7fe5c19fefd880` (ancestor of head: verified) |
| Base | `4d95e45245d660e24b91e26754d9c57949bf3384` (ancestor: verified) |
| Exact reviewed diff | `git diff 4d95e452..6b7d2d45` — full copy at `E/misc/full-review-target.diff`, SHA-256 `1eb0d5ba636a7c676c8974dc01dee3557bc69c365d0db3602028eeddc8b21bcb` |
| #318 author (from its report) | T3 Code / Codex harness, gpt-6.1-sol, high effort, OpenAI |
| Reviewer (observed) | Claude Code harness inside T3 Code; model **Claude Opus 5.5 (`claude-opus-5-5`)**, Anthropic. Named reasoning-effort level: **UNKNOWN** (not exposed to me as a named level). Account/quota/backend routing: UNKNOWN. No subagents used. |
| Independence | Cross-provider (author OpenAI, reviewer Anthropic). I did not author any #318 code. |
| Review lane | `/media/jer/Files/CCF/Operations/CCF-WB287VerifierReview-20261107`, branch `docs/workbench-287-verifier-closure-independent-20261107-v1`, created from `origin/main` `c33a777d6d33f30cff7e32361e48b49866ba1701` |
| Evidence root (`E/`) | `/media/jer/Files/CCF/Verification/Workbench287VerifierReview-20261107/` |
| Date | 2026-11-07 (UTC) |

**Preflight:** I fetched the store. The review branch existed neither locally nor on origin, and the worktree and evidence paths did not exist before I created them. The new worktree had a clean index at `c33a777d`. Files is `/dev/sdb4`, ext4, mounted rw with about 49 GiB free. ZX20 had about 22 GiB free when I read it. The Unity lock was `free`, `unity_editor_pids: []` (read-only `unity-lock status --json`), and no Editor process was running.

**Read-only lanes (not written):**
- The #318 source lane is still at `6b7d2d45` with an empty porcelain status.
- The standalone clone `/media/jer/ZX20/CCF-WorkbenchRetirement-T3-20261105` is clean at `5085e8af`.
- The dirty Unity lane `/media/jer/ZX20/CCF-WorkbenchTier2-Registered-20261106` still has exactly eight modified paths at `4d95e452`.

Disclosure: one plain `git status` in the dirty ZX20 lane ran without `GIT_OPTIONAL_LOCKS=0`. Git may refresh the index stat cache on such a call. No tracked byte or worktree file changed. Every later status read used `GIT_OPTIONAL_LOCKS=0`.

**Executing candidate code:** I copied the six source files of `fb5eeb98` with `git show fb5eeb98:<path>` into an untracked scratch folder, `.review-scratch/candidate-fb5eeb98/`, inside my own worktree. The blob IDs and SHA-256s are in `E/copied-blobs.txt`; for example `run_workbench_retirement.py` is blob `c1c1ec3e`, sha256 `8feb635e…84221`. The scratch folder is not committed. Nothing ran in, or wrote to, the source lane.

There was no Unity Editor, import, lease, Library creation, package/material/scene/save edit, PR, merge or force-push.

## Itemised verdict

| # | Contract item | Reviewer result | Severity of open findings |
|---|---|---|---|
| 1 | Diff scope, eight dirty files preserved, no false equivalence | **PASS** | — |
| 2 | B2 lifecycle: 4 met / 2 unmet, real scene reload, null detail, C–G retained | **PASS (source)**; runtime **NOT RUN** | Note N6 |
| 3 | B3 tree substitution honestly BLOCKED, no rebaseline/equality loss | **PASS (honest accounting)**; B3 itself remains **BLOCKED / unresolved** | — |
| 4 | B4 v24 pins, historical projection, teaching policy, the 28 controls | **PASS (source)** with minor labelling/strength findings; runtime **NOT RUN** | Minor R4, R5; Note N4 |
| 5 | M1 runner false-green, cleanup, receipts, identity, dirt | **FAIL → NEEDS CHANGES.** N1–N3 and identity checks PASS, but in-run lane drift is not detected and one receipt path can be false-green | **Major R1**; Minor R2, R3; Notes N1, N2, N3, N5 |
| 6 | M2 native 720p/1080p witness | **PASS classification**: correctly NOT RUN, no RackV2 screenshot counted | — |
| 7 | Reconciled verdict | **NEEDS CHANGES (source scope only)** | — |

## 1. Scope and protected dirty lane — PASS

- `git diff --name-status 4d95e452..6b7d2d45` lists **exactly the seven claimed files**:
  - four modified verifier/runner files: `WorkbenchRetirementVerification.cs`, `run_workbench_retirement.py`, `Enlarged80Verification.cs`, `RackV2PersistenceVerification.cs`;
  - three new files: `test_runner.py`, `test_historical_projection.py` and the result document.
- Stat: +790/−80. `git diff 4d95e452 6b7d2d45 -- Assets Packages ProjectSettings .vscode` is **0 lines**. There is no scene, save, fixture, frozen-Reference or production change.
- `git diff --check`: exit 0 (`E/misc/diffcheck.*`).
- `sha256sum -c` of `/media/jer/Files/CCF/Verification/Workbench287MorningClosure-20261107/SHA256SUMS.txt` gives **16/16 OK**, exit 0. The manifest file's own SHA-256 is `9fb80596…961f74`, as claimed.
- Live readback (`E/dirty-lane-readback/readback.txt`): all **eight** current lane files equal their preserved dirty bytes, all differ from Git `4d95e452`, and each Git blob equals its preserved `head-versions` copy. The lane's porcelain status still lists exactly those eight paths. Nothing was cleaned, committed or restored.
- The package attribution was independently re-diffed (`E/dirty-lane-readback/package-material-diff.txt`):
  - three direct entries go from 1.1.0 to 1.1.1;
  - `com.unity.sysroot.base` goes from 1.1.0 to 1.1.1 as the transitive lock entry and dependency edges;
  - the read-only `Library/PackageCache` `package.json` reports 1.1.1 for all four packages.
- The material hunks (MainTex texture binding, `_AlphaToMask` 0→1, float re-encoding of `_Color`) agree with the report. The report calls them Editor property rewrites with possible rendering effects, **not formatting**, and issues **no** equivalence waiver. I find **no false equivalence claim**. The proposed restoration is explicitly "NOT APPLIED" and left for a Manager decision.

## 2. B2 lifecycle repair — PASS at source level, runtime NOT RUN

Verified in `WorkbenchRetirementVerification.cs@fb5eeb98`:

- **Dependency check (lines 203–216):**
  - The verifier reads the real serialized private `requiredBuildable` (production `ForestBuildable.cs:19`) by reflection.
  - It asserts the **exact** sorted workbench set `log-rack-build-01,saw-pit-01,shelter-01,timber-sledge-01`, and that all four are met.
  - It asserts the exact Saw Pit set `grinding-stone-01,log-rack-build-02`, that `saw-pit-01` is unbuilt and that both dependents are **unmet**.
  - Only the historical workbench entry is added to the new-game capture. Nothing else is built.
  - These sets match #317's scene-file mapping.
  - Minor gap, not counted as a finding: "every other site unbuilt" appears in a label but is not separately asserted. The unmet Saw Pit dependents and the unbuilt Saw Pit cover the part that matters.
- **Null detail (line 49):** `detail = detail ?? ""` runs before both the failure path and the success path, so `CanLoad(out problem)` with a null problem can no longer throw. This repairs #317's exact defect.
- **Real restart (lines 100–117, 219):**
  - `ReloadFreshScene` keeps only the runner via `DontDestroyOnLoad`, calls `SceneManager.LoadScene("ForestTest", Single)` and waits 5 frames.
  - It then re-acquires the new scene's components and asserts the workbench is retired, unbuilt and hidden (0 renderers, 0 colliders), V2 is active and there are 1,344 trees.
  - This is the same mechanism as production `StandaloneSessionMenu.Restart` (`:146–148`, `LoadScene(activeScene.buildIndex)`). `ForestTest` is the only enabled scene in `EditorBuildSettings`.
  - Production `ForestBuildable.Awake:40–49` recomputes `retiredFromStartingWorld` and `isBuilt` from serialized scene state, so the assertion genuinely witnesses Awake. It is not two field assignments.
  - The `[RuntimeInitializeOnLoadMethod]` runner install and the session-menu bootstrap fire once per Play session, so the reload does not duplicate the runner.
  - `ResetWorld` is now commented and labelled as a model-0 scratch fixture, and section A's label was corrected. Reset inputs (player pose, workbench state) are logged.
- **C–G retained:** the removed lines in the whole file are only the old section-A comment and label, the old comment and the faulty `HasPrerequisite` check. Check-call count went 50 → 60. Sections C (disk Save/CanLoad/LoadData/re-save), D (empty/null lists), E (Reference 0/20/50/100 enter/exit), F (Year-0/Year-3 resumed vs continuous) and G (Legacy40 accounting and return) are textually unchanged apart from the inserted reload and fixture lines.
- **Runtime:** NOT RUN by #318 or by me.

## 3. B3 tree substitution — honest BLOCKED; no rebaseline

- I independently recomputed from the raw #287 captures (`E/tree-recompute/compare.py`, exit 0):
  - run4 start/final SHA-256 `9b5e9f0d…` / `3fbdc138…`; run5 `6905220c…` / `9d8acf02…`, all matching #318's forensics;
  - 1,344/1,344 trees, 1,343 shared, all 1,343 identical;
  - **lost P0604** at (−12.1465, −8.0724), **gained P1209** at (−2.0547, 3.9100);
  - top-level differences are only `trees` and `scenarioOne`, and the only differing `scenarioOne` key is `understoreyCells`;
  - **the captures contain no player-pose field**, which confirms #318's evidence-gap statement.
- The cited mechanism is real source: `ForestStartingStand.CollectClearancePoints:325–343` adds the live `ForestPlayer` position and every non-retired buildable as clearance anchors. The #318 report says plainly that this "establishes plausible input sensitivity, **not the cause**" and marks B3 **BLOCKED**. I judge that separation of mechanism from cause **honest**.
- The Enlarged80 final equality `Check(CurrentHash() == startAnchor, …)` (line 462) is unchanged and absent from the diff's removed lines. There is no tree, omission, hash, fixture or determinism change.
- B3 itself remains unresolved. Closing it needs a separately authorised runtime investigation using the new `ENLARGED80_RESET_INPUT` and `WORKBENCH_RESET_INPUT` logs.

## 4. B4 save versions and historical projection — PASS at source level, with minor findings

- **Stale pins:**
  - Rack: the four v23 sites (174, 329, 476, 527 at base) now pin **v24**. Production `ForestSaveData.CurrentVersion = 24` at `4d95e452`.
  - Enlarged80 pins v24.
  - The string-replace `ToV20` that only recognised `"version":23` is gone.
- **Version commits verified:** each claimed commit's `CurrentVersion` equals its label and its parent's equals the label minus one: `4c40b0ea`=19, `b0246334`=20, `cf84d24c`=21, `cd7b5183`=22, `03790ab7`=23, `129d0ace`=24.
- **`HistoricalLayout` behaviour:** byte-identical in both harnesses (checked by both the author's and my extraction).
  - It accepts only targets 19/20/21 and only v24 input.
  - It **refuses** to discard: a true or missing teaching flag; objective history; prescriptions or `nextPrescriptionId`; any work-order or event provenance; a non-empty, confirmed or advanced rack plan before v21; linked rack orders before v21; and non-Legacy40 geometry before v20.
- **My independent schema audit** (`E/reviewer-schema-audit/`, exit 0):
  - I compared public instance fields of all 80 `[Serializable]` classes at `4d95e452` with the **last** commit of each version (`b0246334^`, `cf84d24c^`, `cd7b5183^`) and with the introduction commits.
  - The only save-relevant additions are exactly what the projection removes: root `standGeometryModel` (v20); scenario `rackPlanningModel/rackPlanConfirmed/nextRackId/racks` and order `rackId` (v21); `objectiveAchievements` (v22); prescription fields (v23); `firstGuidedRackReviewComplete` (v24).
  - The one other delta, `AssortmentSpecification.MinimumMidDiameterMm`, belongs to timber-yield configuration and is not referenced by save data.
  - The author's audit covered only three classes and omitted root `ForestSaveData`. Mine covers that gap and agrees.
- **Teaching-history policy matches production.** `ScenarioOneManager.cs:1484` is `firstGuidedRackReviewComplete = saveVersion >= 24 && data.firstGuidedRackReviewComplete`. The new controls test v24 true/false round-trip and a stray true flag on v23 being ignored. **No schema or save-policy decision is hidden in the diff.**
- **The 28 C# controls are real.** I re-ran the author's script from my scratch copy. The only change was its hard-coded output-path assertion (`E/author-projection-path-only.diff`, one line). Result: exit 0, 28 `PROJECTION_PASS` plus `HISTORICAL_PROJECTION_PASS checks=28`, compiled by Roslyn against the actual extracted method. The 28 are 3 versions × 6 boundary tests, 9 refusals and 1 v21 V2 preservation.
- **My 14 extra controls on the same extracted method** (`E/reviewer-projection/run/`, build exit 0, run exit 2 by design):
  - 13 behaved as expected: refusals for advanced `nextPrescriptionId`; each single order-provenance field; advanced `nextRackId`; an unconfirmed draft rack before v21; a missing or wrongly-typed teaching flag; targets 22 and 18; v23 input. Allowances for a confirmed plan at v21 and plain Legacy40 to v19.
  - 1 did not refuse where I first expected it to: a pre-v21 projection of an **empty-plan `rackPlanningModel=1`** removes the model. This is the rule the base `ToV20` already applied deliberately (its second `.Replace` for model 1 with an empty plan). An empty V2 plan carries no player history, and the v2-mode section 7 depends on it. Classified **Note N4**, not a defect.
- **Synthetic vs genuine:** code comments say "Synthetic layout projection, NOT an archived historical capture", and the Enlarged80 check labels now say "synthetic". Neither harness claims a genuine archived save. See minor R4 for the remaining log labels.
- **Runtime** (v2, Legacy40, coldstart, Enlarged80 F) is NOT RUN.

## 5. M1 runner — independent mocks; NEEDS CHANGES

### 5a. Author's 13 mocks, re-run from my scratch copy — PASS

`CCF_TEST_SCRATCH=E/author-runner-mocks/tmp python3 -I -B Tools/Verification/WorkbenchRetirement/test_runner.py` gives **exit 0, 13/13 ok**. Receipts are in `E/author-runner-mocks/{stdout,stderr,exit}.txt`.

| Control | Result |
|---|---|
| N1: exit 0 without terminal PASS | 1 |
| N2: FAIL marker plus PASS | 1 |
| N3: real nested `packages/` licence directory | 0, cleanup done, 2 hash prefixes |
| Injected `IsADirectoryError` | 1, licence removed, both receipts written |
| Cleanup refusal | 1 |
| Compiler error | 1 |
| Nonzero exit | 1 |
| Dirty lane | 1, not launched |
| Wrong branch / HEAD / lease | 1 each, not launched |
| Post-run lease change | 1 |

### 5b. Reviewer adversarial probes

`E/reviewer-probes/probe_runner.py`: 35 probes, script exit 0. Raw output is in `E/reviewer-probes/stdout.txt`; the separately reproduced receipt detail is in `p4-detail.txt`. My first attempt crashed because of a bug in my own harness (it read a directory as a receipt). It is retained as `attempt1-*` and was fixed before the real run.

**Passed (fail closed, as intended):**
- **Identity:** near-miss branch `candidate-x`; detached HEAD (`""`); stale lease; free lease; Editor PID before the run; Editor PID surviving the run; task 288; owner mismatch; lease lane elsewhere; 12-character HEAD in the lease; an untracked file; an untracked source. All exit 1 and none launched the Editor, except the surviving-PID probe, which by design is detected after the run.
- **Look-alikes:** `…_PASSED`, `…_PASS_X`, an indented token and a prefixed token are all rejected.
- **Other cases:** a missing log fails; a timeout gives 124 and fails; a pre-existing staged file is refused **and preserved**; `--env HOME=` is refused.
- **Symlinks:**
  - A symlink loop in the source licence folder makes `copytree` fail → admission error (exit 1). The partial copy is still deleted and the outside target survives.
  - A selected licence folder that is itself a symlink: copy deleted, source survives.
  - A symlink to an outside folder created inside the licence copy during the run: copy deleted, outside folder **survives** (`rmtree` does not follow it).

**Findings:** see R1–R3 and N1–N3 below.

The owner, HEAD, branch and lease checks match the real `unity-lock` schema: I read `/media/jer/ZX20/CCF-Ops/scripts/unity-lock:52–73` read-only, and `task_id` is a string. The **pre-run** dirt check (`git status --porcelain=v1 -uall`, any output fails) is new and stricter than base, which had none. It was **not loosened**.

## 6. M2 native witness — correctly NOT RUN

The #318 table records Phase 1.5 as **NOT RUN** and gives a detailed future protocol (1280×720 and 1920×1080, real input, approach/aim/use/traverse, repeated after save/load, after a real new-game reload and after a Reference enter/exit). It does **not** count RackV2/O1E2E screenshots or source assertions as a workbench-native PASS, and it explicitly narrows the Year-3 stripped-record claim (#317 m1). The Year-0/Year-3 hashes stay labelled as context-dependent diagnostics.

I classify every #318 result row as follows. None is overclaimed:
- B1 attribution: PASS-attribution / NOT RUN requalification;
- B2: source PASS / runtime NOT RUN;
- B3: FAIL-BLOCKED;
- B4: source PASS / runtime NOT RUN;
- M1: mock PASS, but see R1/R2;
- M2: NOT RUN.

One wording issue: "both marker and JSON receipts are attempted regardless" and "receipt-writing failure also forces nonzero exit" are each true. Together they imply durable receipts that agree with the exit code, which R2 shows can fail.

## Severity-ranked findings

### R1 — MAJOR (contract item 5): in-run lane drift is neither detected nor recorded

**Evidence:** probe P7. The mocked Editor makes `git status` report ` M Packages/manifest.json` and ` M Packages/packages-lock.json` after the run. The runner returns **0**, with `failures: []` and green receipts.

**Cause:**
- Admission checks dirt only before staging (`verify_identity(args, clean=True)`, line 95).
- After the Editor exits, `verify_identity(args, clean=False)` (line 125) checks only lease, branch and HEAD.
- There is no status check after harness/meta cleanup, and receipts record no package resolution.

**Why it matters:** this is exactly the #317 **B1** mechanism. The 2026-11-07 Editor run rewrote `Packages/*` to 1.1.1 and five materials inside a lane that was clean at admission. A future runner would report a clean-looking PASS for bytes that are no longer the reviewed HEAD. It is not an admission bypass, but it is a quiet dirty-lane path *during* the gate. #317 B1 asked that a future run "record … per-phase drift rather than allow arbitrary contents at known dirty paths".

**Required change (not applied by me):** after removing the harness and `.meta`, run `git status --porcelain=v1 -uall`. If it shows anything, fail and record the drifted paths and their hashes in both receipts. Add a mock control for this.

### R2 — MINOR: the JSON receipt can contradict the actual exit code

**Evidence:** `E/reviewer-probes/p4-detail.txt`. When the `.markers.txt` write fails, the process returns 1 and the printed JSON says `exit_code: 1`, but the **on-disk JSON receipt says `exit_code: 0`, `failures: []`**.

**Cause:** in lines 158–164 the tuple of `(path, body)` pairs is built, including `json.dumps(result)`, *before* the loop runs. A markers failure therefore never reaches the JSON body.

**Impact:** a durable false-green receipt in a rare filesystem-failure case.

**Fix:** serialize the JSON after attempting the markers write.

### R3 — MINOR: `--pass-token` is only enforced for the Workbench gate

**Evidence:** probe P8. `RackV2PersistenceVerification` with `--pass-token RACKV2PERSIST_CHECK` (a per-group intermediate marker) and a log holding only that marker returns **0**.

The #318 protocol tells operators to supply the actual terminal token, but the code does not enforce it. Enlarged80 (`ENLARGED80_VERIFY_PASS`) and Rack (`RACKV2PERSIST_VERIFY_PASS`) have known terminal tokens. Suggest an allow-list mapping each gate to its token, or refusing unknown gates.

### R4 — MINOR (B4 labelling): some log lines still read as genuine historical saves

Rack v2/Legacy40 log lines are the evidence a future Tier-2 run will cite:
- "the v20 save keeps its Enlarged80 geometry";
- "the Legacy40 v21 save loads unchanged";
- `Pass("Legacy40 v21 save: Continue loads it unchanged …")`;
- `Pass("old save with legacy rack progress (v20 and v21 model 0) …")`;
- the pre-existing refusal labels "v21 missing nextRackId", which act on v24 files.

The Enlarged80 Pass "v19 Legacy40 save into an Enlarged80 world" also lacks "synthetic", although its check labels have it.

These fixtures are now synthetic projections; the code comments say so, the log text does not. This is an improvement on base, which used v23 captures mislabelled v21, but it is not yet fully truthful in runtime evidence. Prefix them with "synthetic".

### R5 — MINOR (strength): two teaching-history controls cannot fail

- "absent teaching history stays false" goes through `JsonUtility.FromJson` into memory, where a missing bool defaults to false. It never reaches production's disk/JSON validation path.
- The cold-start check `FirstGuidedRackReviewComplete == seeded flag` cannot tell anything apart while the seeded partial save has `false`.

These are not false claims, but they are weak. The v24 true/false round-trip and the v23-stray-flag controls are the ones that actually discriminate.

### Notes (no change required for source admission; for Manager awareness)

- **N1:** the FAIL regex `\b(?:[A-Z][A-Z0-9_]*_)?FAIL\b` misses `…_FAILED`, `…_FAILURE`, lowercase forms, and uncaught `NullReferenceException` / `Assertion failed` lines when a PASS token is present (probe P1: exit 0). There is **no current false-green**: all three gates print a mutually exclusive terminal `*_VERIFY_PASS` / `*_VERIFY_FAIL`, and `_VERIFY_FAIL` is matched. An unrelated production exception during a passing gate would not fail the runner.
- **N2:** the Editor's stdout/stderr are neither captured nor checked (P3b). Only `-logFile` is inspected.
- **N3:** the runner inherits `CCF_TEST_DIRECT_ENTRY` from the caller and does not set it. The #318 future protocol does not mention it. Without it, `StandaloneSessionMenu.Bootstrap` shows the startup screen during the gate. Base had the same behaviour. The future operator should pass `--env CCF_TEST_DIRECT_ENTRY=1` deliberately and record it.
- **N4:** a pre-v21 projection deliberately drops an empty-plan `rackPlanningModel=1`. This is the retained base rule; it should be documented in the projection comment and given a positive control.
- **N5:** `test_runner.py` defaults its scratch folder to the #318 evidence folder `Workbench287MorningClosure-20261107/additions` when `CCF_TEST_SCRATCH` is unset. I set the variable so nothing was written there.
- **N6 (runtime risk, B3-related):** after the real reload, section C regenerates with `ResetWorld` and requires `Hash()==newStartHash && TreeSig==newStartTrees`, both captured before the reload. `CollectClearancePoints` uses live player pose, so any difference between the authored start pose and the earlier pose would give a **false red**, never a false green. This can only be judged at runtime.
- **N7:** Roslyn reports `CS0436`. The dirty lane's generated `Library/ScriptAssemblies/Assembly-CSharp.dll` (01:46Z) still contains a compiled `Enlarged80VerificationRunner` from the night's disposable staging. This is generated cache, not source, and needs no action. Treat it as a reason not to reuse that Library as evidence of the tracked tree.

## Raw proof, commands and exits

All paths are under `E/` = `/media/jer/Files/CCF/Verification/Workbench287VerifierReview-20261107/`. Sources were the scratch copies of `fb5eeb98` (`E/copied-blobs.txt`).

| Check | Command (summary) | Exit | Receipt |
|---|---|---|---|
| Changed-file scope | `git diff --name-status/--stat 4d95e452..6b7d2d45`; production path diff | n/a; 0 lines | `E/misc/diffstat.txt`, `E/misc/full-review-target.diff` |
| Whitespace | `git diff --check 4d95e452 6b7d2d45` | 0 | `E/misc/diffcheck.*` |
| Python compile | `python3 -I -m py_compile` (3 files, scratch) | 0 | `E/misc/pycompile.*` |
| Dirty-lane preservation | `sha256sum -c SHA256SUMS.txt`, per-file lane/preserved/Git compare | 0, 16/16 OK; 8/8 equal | `E/dirty-lane-readback/readback.txt`, `package-material-diff.txt` |
| Author runner mocks | `CCF_TEST_SCRATCH=… python3 -I -B test_runner.py` | **0**, 13/13 | `E/author-runner-mocks/` |
| Reviewer runner probes | `python3 -I -B probe_runner.py <runner copy> <tmp>` | 0 (8 expectation mismatches: 5 = N1, 1 = R1, 1 = R3, 1 = a symlink loop that fails closed, which is acceptable; N2 is documented by P3b, recorded as expected) | `E/reviewer-probes/stdout.txt`, `p4-detail.txt`, `attempt1-*` |
| Author projection controls | `python3 -I -B test_historical_projection_reviewer_outpath.py --out …/run` (only the out-path assertion changed) | **0**, 28 controls | `E/author-projection/` |
| Reviewer projection controls | `python3 -I -B build.py <scratch> …/run` | build 0, run **2** (13 expected, 1 = Note N4) | `E/reviewer-projection/run/` |
| Schema audit | `python3 -I -B audit.py <review worktree>` | 0 | `E/reviewer-schema-audit/result.json` |
| Tree recomputation | `python3 -I -B compare.py <#287 evidence> run4 run5` | 0 | `E/tree-recompute/stdout.txt` |
| Roslyn type check (3 harnesses, `UNITY_EDITOR`, read-only DLLs) | `python3 -I -B compile.py <scratch> …/positive` | **0**, 0 errors, 45 warnings (44 CS0618, 1 CS0436, 1 CS1701) | `E/reviewer-roslyn/positive/` |
| Roslyn negative control | same, plus a file with an undefined name | **1**, CS0103 | `E/reviewer-roslyn/negative-control/` |
| Lock / Editor | `python3 -B unity-lock status --json`; `pgrep` | free, `[]`; none | read-only |

## Honest limitations

- **No Unity at any point.** Scene reload, sections C–G, all Rack modes, Enlarged80 F, the native witness and rendered material equivalence are **NOT RUN**. A Roslyn type check against the dirty lane's 1.1.1-built DLLs is not Editor compilation or behaviour.
- The runner probes replace `subprocess`, `git` and lock calls with mocks. They test the Python orchestration logic only, not a real Editor or lease.
- My schema audit uses a regex over public fields. It does not parse C# semantically, so properties and serialization attributes beyond the public-field convention were not evaluated.
- B3's cause stays unproven. I added no runtime evidence.

## Reconciled verdict

**NEEDS CHANGES — source scope only.**

- The B2 lifecycle repair, the B4 version pins and projection, scope control, dirty-file preservation and the honesty of the B3, M2 and Unity NOT RUN classifications are sound.
- **R1 (major)** should be fixed before this runner is used for an admission-grade Tier-2 run. **R2–R5 (minor)** should be fixed in the same small follow-up. All are verifier/runner-only changes inside the #318 file scope.
- No production, package or save-policy change is implied. No Manager-only policy question was found beyond the existing package and dirty-file disposition, which #318 correctly leaves to Manager.

This reviewer opinion is not Manager #200 acceptance and confers no qualification.
