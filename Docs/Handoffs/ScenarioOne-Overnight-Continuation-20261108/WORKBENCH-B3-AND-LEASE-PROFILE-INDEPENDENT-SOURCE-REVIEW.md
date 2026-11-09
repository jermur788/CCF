# #348 Independent source-only delta review: B3 candidate C and post-transition lease profile

**Verdict: APPROVED (SOURCE ONLY)**, with four non-blocking findings for the Manager (section 3). This covers source only. Nothing was compiled or run in Unity. The provisional expiry `2026-11-11T00:00:00+00:00` is **not approved**. Deployment of `unity-lock.proposed` is **not approved**. No candidate transition is approved.

- **Reviewer:** Claude Opus 5.5 (Anthropic), model id `claude-opus-5-5`. Effort was **low**, as set by the orchestrator for this session. The #347 author was Claude Sonnet 5.5, so this is the same provider family but a different model and session.
- **Date:** 2026-11-09. **Lane:** `/media/jer/Files/CCF/Operations/CCF-WorkbenchB3PostTransitionReview-20261108`, branch `docs/workbench-b3-post-transition-combined-review-20261108-v1`. It started at freshly fetched `origin/main` `20985face85ba7d29bc36219923e1660692c9b4e`, with a sparse checkout of this directory only.
- **Binding packet:** `MANAGER-346-TO-347-B3-CANDIDATE-AND-LEASE-PROFILE-PACKET-20261108.md`, sections 3 and 4.
- **Receipts:** RUNNING https://github.com/jermur788/CCF/issues/348#issuecomment-6729409487.
- **Evidence:** `/media/jer/Files/CCF/Verification/CCF-WorkbenchB3PostTransitionReview-20261108/`. `SHA256SUMS.txt` has sha256 `e783d0bb63cf2fa9eb299cb0820c509d76227dd2b090e7d1e20090b81d89929b` and covers 10 files: logs, results JSON, identity facts and the reviewer's two scripts. All of it totals 76 KiB. The disposable fixtures and scratch repositories were mine and were deleted after the run.

## 1. Sources reviewed (exact identities)

| Item | SHA | Notes |
|---|---|---|
| B3 candidate **C** | `69c52906e973f6afa926c941cefa2bbc7b2a2412` | Branch `feat/workbench-111-b3-evidence-candidate-20261108-v1` (remote tip equals C). Parent is `34e6db134b50f0a393733145962ca7c9c69a9e36`, whose parent is Option B `46bc1fd618ae5b214d7de296a03ac7efec27cb7c` |
| Lease profile source | `26e32b9cf8579a97e4988dcab5f674ef03c1cdd9` | Parent `995b2553…`, then `11cee0c26c81b916c77a5310370d7ce42f2a203b`. The branch tip `40467cf21c14cf3f58655bb2cebfc11db8aae58b` only adds the author's report (`git diff --name-status 26e32b9c 40467cf2` shows one `A` .md) |
| Historical Workbench HEAD | `4d95e45245d660e24b91e26754d9c57949bf3384` | Read through the shared store only |

## 2. Per-item results

Each result below comes from a command I ran myself. The logs are in the evidence directory.

### 2.1 Candidate C (item 1)

| # | Check | Result | How |
|---|---|---|---|
| 1a | C descends from Option B `46bc1fd6` and from `4d95e452` | **PASS** | `git merge-base --is-ancestor` exit 0 for both. `git log 46bc1fd6..C` shows exactly 2 commits |
| 1b | B→C touches only `Tools/Verification/StandGeometry/{Enlarged80Verification.cs M, run_enlarged80.py M, test_b3_witness.py A}` | **PASS** | `git diff --name-status 46bc1fd6 C`. A filter for paths outside `Tools/Verification/` returned none |
| 1c | No gameplay, Assets, ProjectSettings, Packages, scene, material or save diff B→C | **PASS** | Same diff (3 paths, all under `Tools/Verification/`) |
| 1d | Package bytes at C equal Option B byte for byte | **PASS** | Blob ids are equal: `Packages/manifest.json` `9fb01b72…` (sha256 `f2ff5f95…c2c1c`, 2297 B) and `Packages/packages-lock.json` `c26dfa4f…` (sha256 `f43d9894…52b1`, 14223 B) |
| 1e | 4d95→C inventory | **PASS** | 13 paths: **M** 2 Packages + 5 `Tools/Verification` (RackV2 .cs, Enlarged80 .cs, run_enlarged80.py, WorkbenchRetirement .cs, run_workbench_retirement.py); **A** 3 `Tools/Verification` tests + 3 Morning-Closure .md. 4d95→Option B alone has 4 modified and 2 added verification files, which matches the #346 correction. C then adds 1 more modified file (`run_enlarged80.py`) and 1 more added file (`test_b3_witness.py`) |
| 1f | Original production call order in `ResetWorld` unchanged | **PASS** (read) | Diff hunk at `Enlarged80Verification.cs:82-103`. Only `B3*` calls are inserted. The order destroy → `ApplyStandGeometry` → `ResetForDeterministicRun` → `Generate` → `InitializeNewScenario` → `InvalidateCompetition` is intact. The only other edited production-path line is `TryBeginReferencePreview(100)`, whose result now goes into a local before the identical `Check`. That is the same single call with the same semantics |
| 1g | The witness block's only transform write is the authored-pin control | **PASS** (read) | The only `transform.position =`, `cc.enabled =` and `Physics.SyncTransforms()` are in `B3PinPlayer` (`:606-617`), reached only when `b3Mode == "authored-pin"`. **Caveat, finding F2:** `saves.CaptureData()` is not strictly read-only |
| 1h | Exceptions cannot escape `B3Finish` | **PASS** (read) | Its whole body after the `b3Started` guard is in `try/catch`. `B3Pose`, `B3Identity` and `B3ArchiveState` each catch internally. A bad `CCF_B3_MODE` throws inside `Run()`, the existing loop catches it as a harness failure, and `b3Started` stays false |
| 1i | The author makes no compile or runtime claim | **PASS** | The author's report §3 says "the C# was never compiled by Unity". It claims only a `csc` syntax check and a compile against hand-written stubs. I did not repeat the stub compile |
| 1j | C# compiles in the Unity 6000.6 Editor | **NOT RUN** | No Unity. By reading, I cross-checked every production symbol the witness uses against C's `Assets/`: `ForestStartingStand` private fields `latticePerAxis/latticeSpacingMeters/standWidthMeters/standDepthMeters` and `static Vector2 FnvJitter(string)`, public `PathCorridorRadiusMeters/ClearingRadiusMeters`, `SimulationSeed/RngModelVersion`, `ScenarioReferenceArchive.{ResourceName, Load, Canonicalize, Hash, startingStandHash, milestones}`, `ForestSaveData.trees[].treeId`, `ForestBuildable.{BuildId, IsRetiredFromStartingWorld}`. All exist with compatible types. I found no scoping conflict (lambda `t` and foreach `t` are siblings). This is not compile evidence |
| 1k | Author's `test_b3_witness.py` (9 tests) | **PASS** | Run from a private sparse scratch repository at C that uses the shared store as alternates (no object writes): 9/9 OK |

**§4d witness coverage (#333 at `f1a501b1`, section 4d).** "Source" means the observation is present in source and I checked it by reading. Every runtime value is **UNKNOWN until a Unity run**.

| §4d | Observation | Source-observable? | Runtime |
|---|---|---|---|
| 1 | Identity pre/post: HEAD, candidate, scene path and blob, Editor, two package sha256, PackageCache for 4 Linux packages, UTC | **Source present, two disclosed narrowings.** PackageCache is hashed as a listing of paths and sizes, not contents. Candidate SHA is optional and becomes `UNAVAILABLE` (finding F1) | UNKNOWN |
| 2 | Pose and CharacterController at harness start, each reset entry, pre-Generate, pre/post `TryBeginReferencePreview`, post-End +0/+1/+2; `autoSyncTransforms` | **Source present** (`B3Pose`, `:589-603`). `autoSyncTransforms` is read by reflection, and if missing it is logged as an inline `UNAVAILABLE` (§4d allows UNAVAILABLE). The harness has only one Reference preview call site (`:413`), and it is instrumented | UNKNOWN |
| 3 | Clearance anchors (kind, name, x, z, radius), sha256, core slots per anchor | **Source present.** `B3Anchors` mirrors `CollectClearancePoints` (`ForestStartingStand.cs:325-344`) Find for Find. The slot model copies the core arithmetic of `GenerateEnlarged80` (`:181-209`) and is self-checked against the live trees | UNKNOWN |
| 4 | Tree count, id sha256, per-record hash, clearance-omitted and hash-filled sets | **Source present, with an interpretation.** `CurrentWorldHash` has no per-record hash, so the per-record value applies FNV `Hash` to each canonical `TreeSaveData` JSON. The omitted sets are derived from the slot model and are invalid if `slotModelValid=False` | UNKNOWN |
| 5 | `simulationSeed`, `rngModelVersion`, static RNG or UNAVAILABLE | **Source present** (adds a `UnityEngine.Random.state` hash) | UNKNOWN |
| 6 | Numbered reset steps with frame | **Source present** (5 `STEP` lines per reset, `frame=` in the prefix) | UNKNOWN |
| 7 | Reference Future v1 and Legacy40 fixture hashes before/after; change fails | **Source present, with an interpretation.** `ls-tree C` has no Legacy40 fixture besides docs evidence `startup-legacy40.json`, so the archive's own Legacy40 anchors stand in. The disk sha256 of the `.bytes` is always compared. A change converts a PASS into FAIL (`:817-818`) | UNKNOWN |
| 8 | Two plain runs plus authored-pin control | **Support only** (runner `--b3-mode`, labels). There are no offline results, correctly | UNKNOWN |
| 9 | Result is "evidence complete", never "B3 passed" | **Source present** (`b3Verdict=NOT_DECIDED_BY_HARNESS`) | — |

I found no required observation that was **quietly** narrowed. Each narrowing (PackageCache listing hash, optional candidate, per-record and Legacy40 interpretations) is stated in the author's report §3.

### 2.2 Lease profile (item 2)

| # | Check | Result | How |
|---|---|---|---|
| 2a | 11cee0c2→26e32b9c changes only 4 files: proposed, patch, `test_exact_admission.py`, new `test_post_transition_profile.py` | **PASS** | `git diff --name-only`. The blob ids of `unity-lock.deployed-54cf0a95.txt`, `_ccf.deployed-ab4343ef.txt`, `test_raw_byte_admission.py` and `test_tree_object_integrity.py` are identical at both commits (`logs/patch-reproduction.txt`) |
| 2b | In `unity-lock.proposed`, only the post-transition block and the expiry **comment** change. Expiry value unchanged | **PASS** | Full diff read: 2 hunks. The `valid_until` value is still `'2026-11-11T00:00:00+00:00'` and the comment now says "AUTHOR PLACEHOLDER, NOT APPROVED, NOT ADOPTED". No function changed, so F1–F3/N-1, default-clean, lock/status/release, the raw proof and the digest formula are all untouched |
| 2c | Regenerated patch reproduces the proposed file from the deployed-baseline copy | **PASS** | `patch -p1` applied to `unity-lock.deployed-54cf0a95.txt` gives sha256 `434e59d4…6316`, equal to `unity-lock.proposed` (`cmp` ok) |
| 2d | The two edited old tests are the only changed assertions and reflect only the unfreezing | **PASS** | `test_production_profile_is_one_lane_and_post_state_frozen_to_candidate_c` now asserts head = C, clean pins = 2 packages, 13 allowed paths; the old `assertIsNone(head)` was removed. `test_production_profile_refused_on_any_other_lane_in_both_states` now expects the lane refusal instead of "not frozen". Unfrozen fail-closed coverage remains in `ExactPostTransition.test_unfrozen_pieces_each_make_state_unsupported`, which passed |
| 2e | `post.head == C`; `must_descend_from == [46bc1fd6]` | **PASS** | Loaded the proposed module and compared |
| 2f | Six dirty pins equal the original five Materials plus `.vscode/settings.json`, with unchanged hash, size and HEAD blob | **PASS** | `post.dirty` equals `pre.dirty` minus the packages. Each of the 8 pre pins equals `/media/jer/Files/CCF/Verification/CCF-Workbench111GenvB3-20261108/protected-before.json` (sha256 `d664f53e…7ac2`, read only): working, head, size, mode `0o100755`. For the six, the blob at C equals the blob at 4d95 and its sha256 equals the pinned HEAD hash |
| 2g | Two clean package pins equal the C blobs and the 1.1.1 working bytes | **PASS** | sha256 and size of the C blobs (`f2ff5f95…`/2297, `f43d9894…`/14223) equal both the pins and the `#318 dirty-backup/Packages/*` bytes |
| 2h | `allowed_changed_paths == git diff --name-only 4d95e452 C` | **PASS** | 13 paths, sorted and equal; name-status as in 1e |
| 2i | Digest reproducible | **PASS** | An independent recomputation of the `admission_digest` formula gives post `eda2bfe31b870f92c3760a654d620d76bbdf6d915dff6b79123240a2ad28d7cd`, equal to the module's value. Pre digest is unchanged at `90da4135…ce90`. **This digest is PROVISIONAL, NOT ACCEPTED** |
| 2j | Still marked PROVISIONAL NOT ACCEPTED | **PASS** | Block comment: "PROVISIONAL (#347 Phase B) … NOT ACCEPTED and NOT DEPLOYED until the Manager approves" |
| 2k | #318 evidence intact | **PASS** | `Workbench287MorningClosure-20261107/SHA256SUMS.txt` sha256 is `9fb805968899cb6cd369a6d627ef1c01a7afece2368ff9d2093db1bfa9961f74`, it has 16 entries, and `sha256sum -c` passed for all of them (read only) |
| 2l | Author's `test_post_transition_profile.py` (12) and targeted `test_exact_admission.py` classes SourceStructure, ExactPostTransition, ExactPreTransition and DefaultStrictClean (24) | **PASS** | 12/12 and 24/24 OK, with `CCF_LEASE_TEST_TMP` set to my own Files evidence `tmp/` (since deleted) |

### 2.3 My own disposable fixture (item 3)

I wrote this independently in `scripts/review348.py`. It is one miniature repository with the commit chain old (4d95 blobs) → optB (Option B blobs) → target (C blobs) → wrong (target plus an unlisted `Assets/` file). It uses the 21 relevant paths plus the preserved `#318 dirty-backup` bytes. Each lane copy gets its own git dir and a `.git` pointer file. I called the **proposed** `admit_exact_dirty()` directly. Profile copies were patched only for lane, path, store, branch, fixture heads and mode `0o100644`. The real 8/6/2 pins, the real allowed list and the real #318 evidence root were left unpatched. No lock or lease event exists on this path. 49/49 checks matched expectations (`logs/reviewer-own-fixture.log`, `logs/review348-results.json`).

| Check | Result |
|---|---|
| Plain `merge --ff-only`, `switch --detach` and `checkout` onto the target are refused, name both packages, and leave HEAD and the 8-dirty status unchanged | **PASS** |
| Stage only the two packages, then `merge --ff-only`: HEAD = target, exactly the six dirty paths, nothing staged, all six working bytes, sizes and HEAD blobs match the pins, both packages equal the clean pins | **PASS** |
| Pre-transition eight-file state ADMITTED by the proposed code (real 8 pins) | **PASS** |
| Post-transition six-dirty state ADMITTED (real 6+2 pins, real 13-path list) | **PASS** |
| Wrong target (lane fast-forwarded to prior Option B, profile head = target) REFUSED | **PASS** |
| Post state on an un-transitioned lane REFUSED; pre state on a transitioned lane REFUSED | **PASS** |
| Unfrozen: `head=None` REFUSED "not frozen"; `allowed_changed_paths=None` REFUSED "not frozen" | **PASS** |
| Unpatched production profile on the fixture REFUSED (lane mismatch) | **PASS** |
| **My mutations (not in the author's five):** staging only ONE package then ff-only is still refused; profile head equals the lane but the candidate changes an unlisted `Assets/` path, REFUSED "outside the frozen candidate list"; allowed list missing `test_b3_witness.py` REFUSED; lock-file pin size +1 REFUSED on that path and the digest moves; transitioned lane with the manifest re-dirtied by one byte REFUSED | **PASS** (all fail closed) |
| **B3 mutations (`scripts/b3mut.py`):** BM3, swapping `ApplyStandGeometry`/`ResetForDeterministicRun`, is caught by the author's order test | **PASS** |
| BM1: inline `candidate=UNAVAILABLE repoHead=UNAVAILABLE` plus archive resource/anchor `UNAVAILABLE` still yields `EVIDENCE_COMPLETE` | **Observed defect, finding F1** |
| BM2: a `SetPositionAndRotation` write in `B3Pose`, and BM4: an extra `stand.Generate()` in `ResetWorld`, both pass the author's static tests | **Observed test gap, finding F4.** My own reading confirms C itself contains neither |

## 3. Findings (ranked; none blocks SOURCE-ONLY approval)

**F1 (Medium, evidence-completeness overclaim).** `Enlarged80Verification.cs:541`, `:774`, `:811-813` and `run_enlarged80.py:48`. Some inline fields are rendered as `UNAVAILABLE` without calling `B3Missing`, so neither the harness `complete` flag nor the runner's `kind=UNAVAILABLE` scan counts them. The affected fields are `repoHead` and `candidate` (§4d item 1), `resourceBytesSha256` and `legacy40AnchorsSha256` (item 7), `autoSyncTransforms` and `unityRandomStateSha256`. **Scenario:** a run started as `run_enlarged80.py plain-1`, the documented default with no `--candidate-sha`, logs `candidate=UNAVAILABLE` and still reports `b3.status = EVIDENCE_COMPLETE`. I reproduced this offline (BM1). Likewise, if `Resources.Load` returns null both before and after, `archiveSame` is true and the run is COMPLETE. The disk sha256 of the `.bytes` is still compared, which limits the impact for item 7. **Mitigation without changing C:** the runner JSON carries `b3.candidate` next to `b3.status`, and the author disclosed that the candidate is optional. The Manager should require `--candidate-sha <C>` in any qualification run and read the inline fields. A future source fix should count required-field `UNAVAILABLE` (candidate, HEAD, archive parts) as incomplete. Any change to C means re-pinning the lease profile.

**F2 (Low, observer effect; Unity behaviour UNKNOWN).** `Enlarged80Verification.cs:728-733` (`CurrentHash()` and `saves.CaptureData()` in `B3PostGenerate`, between `Generate` and `InitializeNewScenario`). `ForestSaveController.CaptureData` calls `ScenarioOneManager.CaptureSaveData`, which calls `EnsureUnderstoreyGrid()` (`ScenarioOneManager.cs:1399`, `:2510-2534`) when the regeneration model is Competition. That **writes** manager state at a point where the original harness did not. I believe it is neutralised: `InitializeNewScenario` immediately clears `understoreyCells` and calls `InvalidateCompetition` (`:638`, `:653`), and the understorey initialisers contain no RNG use (grep). So the claim "changes no production state" is true in net effect but not strictly. A duplicate-run comparison cannot detect this because every run has it.

**F3 (Low, interpretation for the Manager to accept explicitly).** The PackageCache identity is a path-and-size listing hash, not a content hash (`:575`). The "authored start" for the pin control is the player position captured at harness start after 5 frames, not the scene-authored transform (`:795`, `:613`); it is logged, so it can be audited. The per-record tree hash and the Legacy40 stand-ins are as in §2.1. All of these are disclosed, but they are the Manager's call.

**F4 (Low, offline test gaps; not a source defect in C).** `test_b3_witness.py`. `test_witness_block_only_observes` matches only `transform.position =`. `test_generation_calls_keep_their_original_order` checks first-occurrence order only, so an inserted duplicate production call passes (BM2/BM4). The runner also raises a raw `IndexError` if `--b3-mode` is given without a value (`run_enlarged80.py:57`). This is cosmetic.

No defect was found in the lease profile delta.

## 4. Not verified

- **Unity:** Editor compile, runtime witness values, CharacterController/physics observer effects, F2 in practice and the duplicate-run outcomes are all **NOT RUN / UNKNOWN**. No Unity Editor was started.
- **Real lane:** I did not inspect or touch the registered Workbench `/media/jer/ZX20/CCF-WorkbenchTier2-Registered-20261106` (not even `git status`). Admission on the real 5,732-file exFAT lane, a real transition and real staging are **NOT RUN**. The fixture is ext4 with mode 644, and the exFAT 755 behaviour is not reproduced.
- **Deployed script:** `/media/jer/ZX20/CCF-Ops` was not used or modified. No real lease, lock or event was created.
- **Suites:** I did not run the full 77+40+13 suites, `test_raw_byte_admission.py`, `test_tree_object_integrity.py`, the non-targeted `test_exact_admission.py` classes, or any broad mutation campaign. I did not repeat the stub C# compile.
- Shared store: I wrote only my own branch ref and worktree registration. My scratch repositories used read-only alternates and were deleted. I made no writes in either author lane.

## 5. Independence and limitations

I am Claude Opus 5.5, from the same provider family (Anthropic) as the #347 author, Claude Sonnet 5.5. This is not a cross-provider review, and shared blind spots are possible. Before forming my findings I had seen the Manager packet and the short #347 HANDED-OFF issue comment, which summarises the commits, pins, digest and test counts. I read the author's full report only **after** my own checks and findings were complete. Comparing against it afterwards, I found its disclosure of the optional candidate and the PackageCache narrowing, but it does not state the F1 consequence (`EVIDENCE_COMPLETE` with missing fields). I did not use the author's fixture code for my own fixture. I did run the author's tests as targeted controls, as allowed. Effort was low and the scope was bounded.

**Explicitly:** the provisional expiry `2026-11-11T00:00:00+00:00` is NOT approved. Deployment of the proposed lease script is NOT approved. The post-transition digest `eda2bfe3…28d7cd` remains PROVISIONAL, NOT ACCEPTED. No candidate transition, Unity run, integration or build is approved, and nothing was run on Unity. The Manager (#200) decides what happens next.
