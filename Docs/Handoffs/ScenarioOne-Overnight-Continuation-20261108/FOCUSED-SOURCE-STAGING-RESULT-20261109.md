# #354 Focused Source Staging Result — 2026-11-09

**Verdict: BLOCKED — candidate staged and published for inspection, but NOT READY for Unity.** One compile defect in already-approved D-072 source was found offline, against the project's own Unity 6000.6.0f1 reference DLLs. Fixing it means changing a file that the headland checker byte-pins, which is outside this task's authority, so this report stops for Manager #200. No other blocker was found. This is not a Unity-qualified, integrated, main-ready or user-accepted candidate, and no integration claim is made.

Binding packet: [Manager #353→#354](https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Overnight-Continuation-20261108/MANAGER-353-TO-354-FOCUSED-SOURCE-STAGING-PACKET-20261109.md). Claim: [#354](https://github.com/jermur788/CCF/issues/354).

## 1. Identity

| Item | Value |
|---|---|
| Executor | Claude Code, Anthropic Claude Sonnet 5.5 (`claude-sonnet-5-5`), run through T3 Code. Reasoning effort was not exposed to the session. The packet suggested GPT-6.1 Sol High; this is the actual executor. |
| Branch | `integration/scenario-one-focused-playtest-20261109-v1` (new; absent locally and on `origin` at preflight) |
| Worktree | `/media/jer/Files/CCF/Operations/CCF-FocusedSourceStage-20261109` (canonical store `/media/jer/Files/CCF/Repository.git`) |
| Initial base | `52bb32a6e36c7e0528f679cf97b3276a83537500` (#353 report-only tip) |
| Unity-tested C2 | `b82cb2405fa245ed49e48eb363b4b664b1d85d5e`, the direct parent of the base. `git diff C2 52bb32a6` is exactly one added Markdown file, so production source at the base equals C2. |
| Source commit 1 | `8fc4576f00975631cfa03f18665936fd8e618f6d`: 23 D-072 paths |
| Source commit 2 | `04d65a56ae0ce6ae8b61e8a0cb0687e177978e64`: 8 CTA checker paths (**final source tip**) |
| Report commit | The branch tip after this file is committed (this file cannot name its own hash). The hash is recorded in the #354 hand-off comment. |
| Tree state | Clean (`git status` empty) at source tip. No untracked, ignored, `Library`, `Temp`, `obj` or `Logs` paths. |
| Durable evidence | `/media/jer/Files/CCF/Verification/FocusedSourceStage-20261109/`. Outputs are outside the repo and on Files, not ZX20. |

Preflight (read-only): live `active-tasks.md` showed #354 READY with no writer or Unity claim. Branch and path were absent. The Files mount was ext4 read-write with 11 GB free. All five pinned commits resolved. The `52bb32a6` successor check and the C2 proof are above. Nothing was stashed, reset, forced or reused, and no existing worktree was touched.

## 2. Transfer method and overlap proof

Method: an explicit file-level allowlist, copied as whole blobs with `git checkout <pin> -- <paths>`. There was no merge, cherry-pick, rebase or history rewrite.

* **D-072** `feature/scenario-one-next-action-guidance-v1` @ `9789369d` is 77 commits ahead of merge-base `50042bbe`. The first 60 of those (up to `e1be15c1`) are decision, coordination, spec and Drive-publication history that was deliberately not merged. The remaining 17 (`ffe7c21c`…`9789369d`) carry the source work and its handoff documents. Allowlist: every `Assets/` and `Tools/` path that differs from the merge-base, which is 11 UI files and 12 verification files.
* **CTA** `990ec43e` (tested; the published `9867144c` adds only one Docs result file). The CTA branch changed **no `Assets/` file** after its merge-base with the D-072 branch (`7f5278bf`), so its delta is the 8 `Tools/Verification/OvernightPlaytestFeedback/` files. PR #300 and its UI ancestry were not touched.
* **Overlap proof** (per path, for all 31 distinct paths): the blob at C2 equals the blob at merge-base `50042bbe`, so C2 and the Workbench/1.1.1 lineage changed none of them. The transfers are pure additions on top of C2, with **no hunk conflicts and no reconciliation needed**. The only D-072 vs CTA content difference is `LearningObjectivesView.cs`. CTA carried the older T2-state copy (`7bbacd00`), and D-072's later copy (`6e5d8465`) was taken as authoritative, as D-072 is the later reviewed work. The CTA checker is then run against that integrated file (section 5).
* **Byte identity:** 31/31 transferred blobs equal their origin blobs (git blob ids). Per-file SHA-256 values are in `transfer-manifest.tsv` in the evidence folder.

### Transferred paths (31)

| Path | Origin pin | Blob | SHA-256 | vs C2 | Disposition |
|---|---|---|---|---|---|
| `Assets/ForestPrototype/UI/LearningObjectivesView.cs` | D-072@9789369d | `6e5d846553` | `fb97d3886fff…` | modified | byte-identical |
| `Assets/ForestPrototype/UI/Map/StandMapDrawPlan.cs` | D-072@9789369d | `7b0345c7ed` | `7c55cd9d3d4e…` | modified | byte-identical |
| `Assets/ForestPrototype/UI/Map/StandMapLegend.cs` | D-072@9789369d | `a092312697` | `a629ad2a17f0…` | modified | byte-identical |
| `Assets/ForestPrototype/UI/Map/StandMapModel.cs` | D-072@9789369d | `d8b68bd174` | `9ba839dfb1a4…` | modified | byte-identical |
| `Assets/ForestPrototype/UI/Map/StandMapSource.cs` | D-072@9789369d | `70f38210d6` | `3f37216d931e…` | modified | byte-identical |
| `Assets/ForestPrototype/UI/Resources/ScenarioOneUi.uss` | D-072@9789369d | `5136ed1a42` | `577f89ddbb61…` | modified | byte-identical |
| `Assets/ForestPrototype/UI/ScenarioOneUiRoot.cs` | D-072@9789369d | `cf5707e336` | `70fb0333f51a…` | modified | byte-identical |
| `Assets/ForestPrototype/UI/StandMapView.cs` | D-072@9789369d | `5a3ffe8966` | `df9256a9e638…` | modified | byte-identical |
| `Assets/ForestPrototype/UI/TeachingDiagram.cs` | D-072@9789369d | `65d4aba342` | `5062dd7dd183…` | modified | byte-identical |
| `Assets/ForestPrototype/UI/TeachingDiagramPlan.cs` | D-072@9789369d | `6e73f9571f` | `967b21b8bfd1…` | modified | byte-identical |
| `Assets/ForestPrototype/UI/WorkPlanView.cs` | D-072@9789369d | `46c919d4ad` | `93d5ab0898c3…` | modified | byte-identical |
| `Tools/Verification/NextActionGuidance/check_sources.py` | D-072@9789369d | `af89768ff5` | `a557f4f491fd…` | new | byte-identical |
| `Tools/Verification/ScenarioOneFiveRackDiagram/expected_assertions.json` | D-072@9789369d | `eff8e5a69f` | `6515df087111…` | new | byte-identical |
| `Tools/Verification/ScenarioOneFiveRackDiagram/FiveRackDiagramCheck.cs` | D-072@9789369d | `04f8a9f08d` | `5cb387f5029f…` | new | byte-identical |
| `Tools/Verification/ScenarioOneFiveRackDiagram/PainterProbe/PainterProbe.cs` | D-072@9789369d | `14b87ecc43` | `8b1b90b6c7b8…` | new | byte-identical |
| `Tools/Verification/ScenarioOneFiveRackDiagram/PainterProbe/RecordingUnityStubs.cs` | D-072@9789369d | `15d203c486` | `9241cd65d4d7…` | new | byte-identical |
| `Tools/Verification/ScenarioOneFiveRackDiagram/run_five_rack_check.py` | D-072@9789369d | `b36b72be08` | `03d927cb4c73…` | new | byte-identical |
| `Tools/Verification/ScenarioOneFiveRackDiagram/test_stage_inventory_guard.py` | D-072@9789369d | `bd789b241d` | `527347491635…` | new | byte-identical |
| `Tools/Verification/StandMap/HeadlandOffline/HeadlandFixture.cs` | D-072@9789369d | `ce94e596d8` | `c59d4ca0527d…` | new | byte-identical |
| `Tools/Verification/StandMap/HeadlandOffline/HeadlandOfflineCheck.cs` | D-072@9789369d | `2eccb68cff` | `1a22930a556d…` | new | byte-identical |
| `Tools/Verification/StandMap/HeadlandOffline/HeadlandSourceStubs.cs` | D-072@9789369d | `2dcf03a9de` | `6c95297d65f9…` | new | byte-identical |
| `Tools/Verification/StandMap/HeadlandOffline/run_headland_check.py` | D-072@9789369d | `818ee8602a` | `73e111497927…` | new | byte-identical |
| `Tools/Verification/StandMap/HeadlandOffline/test_stage_inventory_guard.py` | D-072@9789369d | `818daab5bc` | `5ae498171f48…` | new | byte-identical |
| `Tools/Verification/OvernightPlaytestFeedback/ObjectiveCtaRoslynBinding.cs` | CTA@990ec43e | `2d435d468c` | `8a0020cbb8bd…` | new | byte-identical |
| `Tools/Verification/OvernightPlaytestFeedback/README.md` | CTA@990ec43e | `69e0cfb91a` | `fd49fa72eb1c…` | modified | byte-identical |
| `Tools/Verification/OvernightPlaytestFeedback/check_baseline.py` | CTA@990ec43e | `809d446e29` | `7ced2839e16c…` | modified | byte-identical |
| `Tools/Verification/OvernightPlaytestFeedback/objective_cta_roslyn_binding.py` | CTA@990ec43e | `00c312ef2c` | `566aef38e0b7…` | new | byte-identical |
| `Tools/Verification/OvernightPlaytestFeedback/objective_cta_source_check.py` | CTA@990ec43e | `360122d75d` | `3e62f0086255…` | new | byte-identical |
| `Tools/Verification/OvernightPlaytestFeedback/roslyn-reference-manifest.txt` | CTA@990ec43e | `c95d42e61c` | `978123241d3d…` | new | byte-identical |
| `Tools/Verification/OvernightPlaytestFeedback/test_objective_cta_roslyn_binding.py` | CTA@990ec43e | `90a7ae1107` | `422d1ee14b1c…` | new | byte-identical |
| `Tools/Verification/OvernightPlaytestFeedback/test_objective_cta_source_check.py` | CTA@990ec43e | `1bcb93d16e` | `56cd2c9524ba…` | new | byte-identical |

### Excluded on purpose

* All Docs, `specs/` and coordination history from the D-072 branch (including `Docs/Project/*`). The packet permits only this result file as a new Markdown file.
* The four D-072 handoff documents that the five-rack checker pins (see section 6). Their bytes exist at the pinned commits in the shared store, but adding them is a Manager decision.
* Whole-branch merges of the D-072 and CTA branches, PR #300, and any `.meta`, scene, prefab, package, ProjectSettings or save/reference file.

## 3. What was retained, and what was not touched

* **C2 Reference Preview fix retained verbatim.** `git diff C2 HEAD` over every path C2/Workbench changed (9 production paths plus the C2 Tools paths) is empty.
* **Workbench / 1.1.1 lineage.** Reviewed Workbench source `a4019e6b96b50e03d2b71d92c44fc30a0f7ace29` and the 1.1.1 package proposal commit `46bc1fd6` are both ancestors of `HEAD`. They were verified in place and not re-applied. The package files are exactly C2's: `Packages/manifest.json` blob `9fb01b723a`, `packages-lock.json` blob `c26dfa4f23`. They are **not** adopted as canonical or `main` policy by this staging.
* **No unexpected scene, package or asset changes.** Against C2 there are 0 changes to `.meta`, `ProjectSettings/`, `Packages/`, `.unity`, `.prefab` or `.asset` files. `Assets/Scenes/ForestTest.unity` blob `acbb98cb0d` equals C2.
* **Protected items.** The frozen Reference Future v1 archive and identity, Legacy40, Enlarged80 Rack V2, Work Plan commitment, MFM proposal-vs-Fell distinction, Sitka/Oak/Beech parity and all user saves: no file for any of these is in the 32-file difference from C2. No hash, expected result or baseline was changed or rebaselined. (A name search for protected terms matched only `expected_assertions.json` through its `.json` extension. It is the D-072 five-rack assertion list, byte-identical to origin.)

## 4. Scope of the combined change

`git diff --stat C2 HEAD` is 32 files: the 31 transfers above plus the #353 report that was already in the base. In production terms, relative to the D-072 stage bases, the only differing production paths are 9 Workbench/C2 paths and 11 approved D-072 Assets paths, with **0 unexplained** (`allowed-delta-classification.txt`).

## 5. Offline verification

Everything below ran offline from the candidate worktree (Python 3.12.3, .NET SDK 8.0.133 pinned by an external `global.json` working directory where the checker needs it). Every command's output is in the evidence folder. Nothing was written inside the repo, and `git status` stayed empty after every run.

| # | Command (abridged) | Expected | Actual |
|---|---|---|---|
| 1 | `python3 -I -B Tools/Verification/NextActionGuidance/check_sources.py` | all PASS | exit 1: **37 PASS / 2 FAIL**. Both failures are base-diff inventories (section 6A). |
| 2 | `python3 -I -B …/StandMap/HeadlandOffline/run_headland_check.py --out <new>` | `HEADLAND_SOURCE_AND_OFFLINE_PASS` | exit 1: `HEADLAND_SOURCE_AND_OFFLINE_FAIL` at the stage-2 inventory. The checker fails fast, so later checks did not run (6B). |
| 2d | Same checker, only `check_stage_inventory` stubbed (**DIAGNOSTIC, not a stage PASS**) | rest holds | exit 0: `HEADLAND_OFFLINE_TALLY PASS=92 FAIL=0`. Authority constants parsed, source methods unchanged, protected UI bytes preserved, scene attachment, and the .NET build of the real pure-map code all pass. |
| 3 | `python3 -I -B …/ScenarioOneFiveRackDiagram/run_five_rack_check.py --out <new>` (full, once) | `FIVE_RACK_SUITE_PASS` | exit 1: `FIVE_RACK_SUITE_FAIL source_checks=176 failed=14`. **436 PASS** (165 painter-probe, 162 source, the rest structural/fixture). All 29 shipped falsification controls (15 plan, 14 painter) applied and were caught. The scene's Dirt Path planks and Forest Clearing parse PASS. The 14 failures are classified in 6C. |
| 4 | `check_baseline.py --rotation-state post-fix --instruction-state post-fix` (SDK-pinned cwd) | `10/0/2`, exit 0 | exit 1: **9 PASS / 1 FAIL / 2 NOT-TESTABLE.** Check 08's Roslyn compile stage fails on **`CS0619`**: see section 7. |
| 4d | Same on a scratch clone outside the repo with `StandMapSource.cs:219` changed to an explicit cast (**DIAGNOSTIC**) | — | exit 0: **10/0/2**, including the Objectives `Update()` site and the six real `SnapFar` cases. So that one line is the only CTA obstacle. |
| 5 | `test_objective_cta_source_check.py` (shipped suite, once, 4 min 44 s) | 884/884 rejected, 112/112 benign | exit 0: **884/884 rejected, 0 false greens, 0 wrong causes; 112/112 benign accepted (32+32+48)**; #325 groups 3/3, 5/5, 5/5. Documented accepted-limit and inline-comment false-red probes behaved as documented. Disclosed limits are unchanged and not waived. |

Not run, by packet instruction: Unity, any Editor import, a global lease, Library creation, native tests, a Player build, B3 replay, new adversarial mutation campaigns, any A–O or Workbench Tier-2 gate. The standalone `objective_cta_roslyn_binding.py` and its test file were not run separately, because check 08 already runs the Roslyn stage and it stops at the compile error.

## 6. Stage-specific checker mismatch disposition

No checker, pin, hash, expected value or test was weakened, edited, skipped or rewritten in the repo. The stage verifiers were not reported as globally PASS.

**A. `check_sources.py` (2 failures).** Both compare against a pre-Workbench base (`e1be15c1`). "Production diff restricted to five owned files" sees the 9 Workbench/C2 production paths. "No added F6/save fields" matches one line in `ForestSaveController.cs` (Workbench/C2 lineage). The same regex over the D-072 transfer alone (C2→HEAD, Assets) has 0 hits.

**B. Headland checker.** It stops at the stage-2 inventory because it counts every production path that differs from the #297 anchor. The 9 Workbench/C2 paths are present in this tree and none of them is a D-072 path. The diagnostic run in 2d is the evidence that nothing else is wrong. The stage-1 pin (`StandMapSource.cs` must equal #297 bytes) **passes**, which matters for section 7.

**C. Five-rack checker, 14 failures in three groups:**

1. **8 — pinned handoff documents are absent.** `OBJECTIVE-GUIDANCE-TEACHING-MAP-IMPLEMENTATION-HANDOFF.md`, `D072-FIVE-RACK-DIAGRAM-BOUNDED-REPAIR-HANDOFF.md` and `D072-FIVE-RACK-STAGE-INVENTORY-CHECKER-REPAIR-HANDOFF.md` are each pinned twice (git blob and bytes). Also `OBJECTIVE-GUIDANCE-TEACHING-MAP-HANDOFF.md` ("unchanged") and the "#296 implementation handoff unchanged since e7bda1f2" check. All four exist at `9789369d` and are absent here by scope.
2. **2 — ancestry-pinned stage 3.** "HEAD predates #303, so no #303 verifier path may be changed" fails, and the assertion tally follows (170 ran vs 176 expected, three stage-3 names missing). The #303 files are present and byte-identical, but the checker decides the stage from commit ancestry, and a byte-level transfer cannot give this branch `6a8b261f` as an ancestor. **This cannot pass on any candidate built without merging that history, which the packet forbids.**
3. **4 — Workbench/C2 delta.** `ForestTest.unity` unchanged, no `Packages/` change, "only allowed files changed since 1b68ddcd" (the 9 production paths plus Docs from the main lineage that the D-072 base predates), and "no new Assets file or .meta" (the same Workbench production paths).

**Proposed witness for Manager G4 (a proposal, not applied).** Accept the combined tree against these stage verifiers on three exact, reviewable facts instead of byte-identical stage inventories: (i) the 31-row byte-identity manifest; (ii) the production allowed-delta set is exactly {9 C2/Workbench paths} ∪ {11 D-072 Assets paths} with 0 unexplained (both stage bases), recomputable with `git diff --name-only <base> HEAD -- Assets Packages ProjectSettings`; (iii) the scene, `Packages/` and C2-lineage paths are byte-identical to C2. Separately, Manager may choose either to add the four pinned handoffs byte-identically from the pinned commits, or to record the ancestry-based stage-3 pair as a permanent expected mismatch.

## 7. Blocking finding: Unity 6000.6 compile defect in approved D-072 source

* **Evidence.** The CTA Roslyn stage compiles the real player-like source set at C# 9 against 320 pinned reference DLLs taken from `/media/jer/ZX20/Unity/6000.6.0f1/` (the project's Editor version). It reports one error: `CS0619 Assets/ForestPrototype/UI/Map/StandMapSource.cs:219 'SceneHandle.implicit operator int(SceneHandle)' is obsolete: 'Implicit conversion from SceneHandle to int is deprecated. Use SceneHandle.GetRawData() instead'`. The line is `int scene = eco.gameObject.scene.handle;`. It is the only `.handle` use in `Assets/`.
* **Origin.** It was introduced by D-072/#297 commit `3bc357dc` and is present in the stage-1 anchor, so it was source-reviewed and approved. It was missed because the offline checkers stub the Unity types (the stand-ins let `handle` convert to `int`). C2 has none of this code, so C2's own Unity run did not exercise it.
* **Why I did not fix it.** The obvious replacement is not one token. `GetRawData()` returns `ulong`, which does not assign to the `int` field `headlandScene` (the scratch diagnostic showed `CS0266`). A real fix means choosing a type or comparison for that field and its `-1` sentinel. The headland stage-1 pin also requires `StandMapSource.cs` to equal the #297 bytes, so a repair breaks that pin until Manager approves a new one. That is a substantive decision, not a mechanical reconciliation, so per the packet I stopped.
* **Limits.** This is an offline compile with the SDK-8.0.133 C# 9 compiler against real Unity DLLs. It is strong evidence but not a Unity compile. The diagnostic scratch clone used an unsafe cast purely to learn what else was behind the error. That cast is **not** a proposed fix and is not in the repo.

## 8. Confirmations

* Unity / Editor launched: **no.** Global lease acquired: **no.** Library created: **no.** Packages imported: **no.** Native/Player/B3 run: **no.** Integrated A–O PASS claimed: **no.** Workbench Tier-2 (#287) remains FAILED/NOT QUALIFIED, as before.
* Not touched: `main`, any feature or user-test branch, `/media/jer/ZX20/CCF-U3`, the original dirty Workbench, any Unity lane, `active-tasks.md`, the shared `.vscode/settings.json`, or the user's saves. The only shared-store change is the new branch and worktree record.
* A disposable shared clone and two scratch scripts under the session scratchpad (outside the repo) were used for the diagnostics. The clone is `git clone --shared` from the canonical store, and the branch tip was not modified through it.

## 9. Residual risks

1. The `CS0619` defect above. Any other approved-but-never-Unity-compiled D-072 code could have similar issues; the offline compile found only this one.
2. The CTA checker's disclosed benign inline-comment false-red limit is unchanged.
3. Native rendering of the five-rack figure, headland and next-action cues, and all Workbench Tier-2 behaviour, remain unverified.
4. The ancestry-based five-rack stage-3 mismatch (6C.2) will persist on any history-free candidate.

## 10. Recommended single next step

Manager #200 decides one bounded repair of `StandMapSource.cs:219` (how the scene identity is stored and compared, plus the matching change to the headland stage-1 pin). That would go as a small, separately reviewed commit on this branch. After it, re-run only check 08 (`check_baseline.py` expects `10/0/2`) and the headland checker, then proceed to the G4 source review of the unchanged witness above. Do not start #315 or #316, and do not enter Unity until the compile is clean offline.

**Status: BLOCKED. The staged candidate is published for inspection but is not READY FOR MANAGER G4 SOURCE REVIEW until the decision above is made.**
