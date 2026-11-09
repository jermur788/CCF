# CCF #347 — B3 evidence candidate and PROVISIONAL post-transition lease profile (source only)

**Date:** 2026-11-09. **Author:** Claude Sonnet 5.5 (`claude-sonnet-5-5`), Claude Code via T3 Code, effort UNKNOWN. One local owner, two sequential phases, no delegates.
**Scope kept:** source and offline tests only. No Unity, no lease, no deployed `unity-lock` change, no contact with the registered Workbench (HEAD, index, the eight protected files, Library), no cleanup, no integration, no build, no expiry approval, no deployment.

## 1. The two exact source commits

| | Phase A: B3 evidence candidate **C** | Phase B: lease profile source |
|---|---|---|
| Branch | `feat/workbench-111-b3-evidence-candidate-20261108-v1` | `fix/unity-lease-post-transition-pins-20261108-v1` |
| **Exact tested source commit** | **`69c52906e973f6afa926c941cefa2bbc7b2a2412`** | **`26e32b9cf8579a97e4988dcab5f674ef03c1cdd9`** |
| Based on | `46bc1fd618ae5b214d7de296a03ac7efec27cb7c` (Option B) | `11cee0c26c81b916c77a5310370d7ce42f2a203b` (reviewed lease source) |
| Lane (Files, sparse, left in place) | `/media/jer/Files/CCF/Operations/CCF-Workbench111B3Candidate-20261108` | `/media/jer/Files/CCF/Operations/CCF-UnityLeasePostTransitionPins-20261108` |

The Phase B branch tip is a **report-only successor** commit that adds only this file on top of `26e32b9c`; review `26e32b9c`, not the tip. Its hash is on the #347 handoff.

**Intermediate commits, superseded, kept on the branches:** Phase A `34e6db13…` (first witness; I announced it as the frozen candidate on #347 before noticing it lacked per-tree record hashes, §3 item 4). `69c52906` adds only that and is the final candidate **C**. Phase B `995b2553…` pinned `34e6db13`; `26e32b9c` re-pins to `69c52906`. Both Phase B source commits touch the same four files.

Evidence (hash-indexed): `/media/jer/Files/CCF/Verification/CCF-WorkbenchB3PostTransitionSources-20261108/`.

## 2. Candidate C: what changed

`C` is two commits past `46bc1fd6` and changes only `Tools/Verification/StandGeometry/`:

| Path | Kind | Blob |
|---|---|---|
| `Enlarged80Verification.cs` | modified | see `phaseA-candidate-freeze.txt` |
| `run_enlarged80.py` | modified | see freeze file |
| `test_b3_witness.py` | **new**, offline | see freeze file |

`Packages/manifest.json` and `Packages/packages-lock.json` in C are byte-identical to `46bc1fd6` (`9fb01b72…`, `c26dfa4f…`). No `Assets/`, `ProjectSettings/`, `Packages/`, scene, material, save, `.vscode/` or Reference Future v1 change. Old HEAD `4d95e452` → C changes **13 paths** (the 11 from #346 plus `run_enlarged80.py` modified and `test_b3_witness.py` added), listed in §5.

### Why the Enlarged80 harness
#333 §4d and its mechanism analysis are written against `Enlarged80Verification.ResetWorld` (reset, `TryBeginReferencePreview`, `EndReferencePreview`, later resets). I extended that harness and its runner. `WorkbenchRetirementVerification.cs` was **not** touched: it has the same reset/preview pattern but logs only `player=` at reset. If the Manager wants the witness hosted in the Workbench harness instead (#337's lane runs that gate), that is a different, follow-up source change. **This is a Manager decision.**

### How the witness works (observation only)
- Every witness line starts `ENLARGED80_B3W seq=… frame=… mode=… label=… kind=…`, so the existing runner's marker filter keeps them. No file is written; no production call is added or reordered (a test checks the order of `ApplyStandGeometry` → `ResetForDeterministicRun` → `Generate` → `InitializeNewScenario` → `InvalidateCompetition`).
- Anything unreadable is logged `kind=UNAVAILABLE what=… reason=…`, never guessed. A reflection read of private generator settings is used for the core-slot model; it is **self-checked** against the real trees after every `Generate` (`SLOT_MODEL_CHECK positionMismatches=`).
- `CCF_B3_MODE=authored-pin` (runner `--b3-mode authored-pin`) is the one harness-side action: before each `ResetWorld` it puts the player back on the authored start captured at harness start (controller off, move, controller back, `Physics.SyncTransforms()`) and logs pose before and after. Default `plain` changes nothing.
- The runner adds a `b3` object to its JSON: `EVIDENCE_COMPLETE` or `EVIDENCE_INCOMPLETE` plus missing record kinds, and always `b3_verdict=NOT_DECIDED_BY_HARNESS`. The harness's own PASS/FAIL rule is unchanged. An archive change makes the harness fail.

## 3. §4d coverage (static only; every runtime value is UNKNOWN UNTIL UNITY)

| # | Required observation | Status | Where / limits |
|---|---|---|---|
| 1 | Identity pre/post: repo HEAD, candidate SHA, scene path + Git blob, Editor version, both package JSON sha256, PackageCache for the 4 Linux packages, UTC | **Source present** | HEAD and candidate come from runner env (`git rev-parse HEAD`; `--candidate-sha` optional, else `UNAVAILABLE`). PackageCache uses folder name plus a **listing hash of relative paths and sizes**, not a content hash of ~0.37 GiB: a deliberate narrowing, stated here |
| 2 | Pose at start, each reset entry, pre-Generate, pre/post `TryBeginReferencePreview`, post-`EndReferencePreview` +0/+1/+2; position `R`, controller enabled/center/bounds, `autoSyncTransforms` | **Source present** | `autoSyncTransforms` is read by reflection and reads `UNAVAILABLE` if 6000.6 lacks it. Preview is logged only in section G (once), as in the original harness |
| 3 | Clearance anchors as `CollectClearancePoints()` sees them: (kind, name, x, z, radius), list sha256, core slots inside each | **Source present** | Recomputed read-only with the same Find calls, using the generator's public radii. Core slot positions come from reflection plus the generator's own arithmetic, checked at runtime |
| 4 | Tree identity: count, id sha256, per-record hash, world hash; core clearance-omitted and hash-omitted sets | **Source present (added in `69c52906`)** | `CurrentWorldHash` hashes one canonical JSON, not records. The per-record value is the same FNV-64 `ScenarioReferenceArchive.Hash` applied to each tree record (plus the named P0604, P1209, P0600, P0009). "Hash-filled" is logged as `hashBudgetOmitted` (absent core slots not in a clearance zone) |
| 5 | `simulationSeed`, `rngModelVersion`, readable static RNG state | **Source present** | Unity `Random.state` hashed via `JsonUtility`; `UNAVAILABLE` if it cannot serialise. No other static RNG is read |
| 6 | Numbered reset steps with frame count | **Source present** | 5 steps per reset; the common prefix carries `frame=` |
| 7 | Reference Future v1 and Legacy40 fixtures hashed before/after; change fails | **Source present, with an interpretation** | The tracked tree has **no separate Legacy40 fixture files**. Reference Future v1 is permanently Legacy40, so its anchors are logged: `.bytes` on disk (sha256 + Git blob id), the `Resources` bytes, and `startingStandHash` plus every milestone `worldHash`. Before ≠ after fails the run |
| 8 | Two plain runs + one authored-start control | **Support only** | Two plain runs = the runner invoked twice with different labels; control = `--b3-mode authored-pin`. No results exist offline |
| 9 | "Evidence complete", never "B3 passed" | **Source present** | `witness=EVIDENCE_COMPLETE/INCOMPLETE`, `b3Verdict=NOT_DECIDED_BY_HARNESS` |

**Not confirmed:** the C# was never compiled by Unity. Checks done: `csc` found no syntax error in the full file; the witness block was **compiled against hand-written Unity stubs** (stub compile, exit 0; stubs are my guess of API shapes, so this proves syntax and typing against those guesses only); 9 offline Python tests pass (witness hooks per §4d item, generation-call order unchanged, witness block has no file write/save/load/state change and exactly one transform write, scope is `Tools/Verification/` only, runner status logic, argument refusal). A one-hook deletion control made the hook test fail.

**Known limit for the Manager:** in a plain run the harness's own later checks may fail (that is the B3 symptom). The witness still counts as complete if everything was captured through the end of the first Reference preview (the first `post-EndReferencePreview+2`). If the harness dies earlier, the witness is INCOMPLETE.

## 4. Phase B: what changed

Edits in `Tools/Verification/UnityLeaseExactAdmission/`, nothing else:

1. `unity-lock.proposed`: the `post-transition` block and one comment. The default-clean path, lock/status/release, the canonical raw-byte proof, F1–F3/N-1 and every function body are byte-identical to `11cee0c2` (a test removes the post block and the expiry comment from both texts and compares the rest).
2. `unity-lock-exact-admission.patch`: regenerated with `diff -u` against the unchanged deployed baseline; applying it reproduces `unity-lock.proposed` exactly.
3. `test_exact_admission.py`: **two** tests that asserted the old unfrozen state were updated (`…post_state_unfrozen` → `…frozen_to_candidate_c`; `…post_state_unsupported` → `…in_both_states`). Both now assert the frozen values and that the production profile is refused on any other lane with a correct digest.
4. `test_post_transition_profile.py`: new, 12 targeted tests.

### Post-transition profile (PROVISIONAL, NOT ACCEPTED, NOT DEPLOYED)
| Field | Value |
|---|---|
| `head` | `69c52906e973f6afa926c941cefa2bbc7b2a2412` (C) |
| `must_descend_from` | `46bc1fd618ae5b214d7de296a03ac7efec27cb7c` (unchanged) |
| `dirty` (6) | `.vscode/settings.json` + 5 `.mat`, original (working sha256, HEAD-blob sha256, size); unchanged from the pre-transition tuples; C leaves all six blobs equal to old HEAD |
| `clean_pinned` (2) | `Packages/manifest.json` → `f2ff5f95f9e93716947391a8818dbe642d81f0b49068516441a58def966c2c1c`, 2297 B; `Packages/packages-lock.json` → `f43d9894af3c6dc054e86e98335dbd248a9fb97527b49c7b5952d96e037e52b1`, 14223 B. Equal to the C blobs **and** to the #337 working bytes |
| `allowed_changed_paths` | the 13 paths of `git diff --name-only 4d95e452 C` (§5) |
| Evidence | #318 manifest `9fb80596…961f74` (16 files), unchanged and reused for both states |
| **Digest** `post-transition` | **`eda2bfe31b870f92c3760a654d620d76bbdf6d915dff6b79123240a2ad28d7cd`** — **PROVISIONAL, NOT ACCEPTED**. The pre-transition digest is unchanged `90da4135…42ce90` |
| `valid_until` | `2026-11-11T00:00:00+00:00`, the unchanged author placeholder, now labelled `AUTHOR PLACEHOLDER, NOT APPROVED, NOT ADOPTED`. **Not approved by this task** |

Derivation was independent: the tests read blobs and diffs from Git objects of the shared store and compare them with the profile, and recompute the digest with their own implementation.

## 5. Changed-path inventory, old HEAD `4d95e452` → C `69c52906` (13)

Added (6): `Docs/Handoffs/ScenarioOne-Morning-Closure-20261107/T3-WORKBENCH-287-VERIFIER-CLOSURE-RESULT.md`, `…/WORKBENCH-RECEIPT-R2-FINALIZATION-RESULT.md`, `…/WORKBENCH-RUNNER-R1-R5-CORRECTION-RESULT.md`, `Tools/Verification/RackV2Persistence/test_historical_projection.py`, `Tools/Verification/StandGeometry/test_b3_witness.py`, `Tools/Verification/WorkbenchRetirement/test_runner.py`.
Modified (7): `Packages/manifest.json`, `Packages/packages-lock.json`, `Tools/Verification/RackV2Persistence/RackV2PersistenceVerification.cs`, `Tools/Verification/StandGeometry/Enlarged80Verification.cs`, `Tools/Verification/StandGeometry/run_enlarged80.py`, `Tools/Verification/WorkbenchRetirement/WorkbenchRetirementVerification.cs`, `Tools/Verification/WorkbenchRetirement/run_workbench_retirement.py`.

Old eight dirty → six expected after the transition: the two package files stop being dirty; the five Materials and `.vscode/settings.json` stay dirty, byte-for-byte.

## 6. Miniature transition replay and controls (disposable fixture only)

One miniature repository per run, built from exact old and C blobs of the 21 relevant paths plus the preserved **#318 `dirty-backup/`** bytes (so the registered lane was not read at all). Results:

- Start state: exactly eight dirty paths with the pinned working hashes and sizes.
- Plain `merge --ff-only`, `switch --detach`, `checkout` onto the target: **all refused**, naming both package files; HEAD, status and bytes unchanged.
- `git add` of **only** the two package files, then `merge --ff-only`: succeeds; HEAD = target; status = exactly the six dirty paths; nothing staged; no hidden index flags; the six keep their pinned bytes; the two packages equal the profile pins and the fixture HEAD blobs; the old→target changed-path list equals `allowed_changed_paths` exactly.
- Not-yet-transitioned fixture (wrong HEAD): HEAD package blobs are the old bytes, so the post pins cannot match.
- Targeted existing controls, **24 tests, OK**: `SourceStructure` (baseline/patch/unchanged-text checks), `ExactPostTransition` (fixture positive; wrong state name; edited/dirty packages; non-descendant HEAD; unlisted changed path; each unfrozen piece unsupported), `ExactPreTransition`, `DefaultStrictClean`. Every rejected request asserts no lock file exists, so no fake lease or event is created on a negative.
- Five mutation controls on the profile (wrong head, package size +1, dropped allowed path, removed head check, changed expiry) each made the targeted tests fail.
- **Not run:** the full 77+40+13 suites, any broad mutation campaign, admission on the real lane.

## 7. Limitations and handoff notes

- A fixture proves Git and profile semantics only, not safety on the live 5,732-file exFAT worktree (stat noise, Unity-written files, untracked/ignored set, Library). The staging step is **not authorised** on the real lane; it changes its index and needs a separate Manager decision.
- Source tests are not Editor compile or runtime evidence. B3 witness values are UNKNOWN until a Unity run, which needs the lease, the deployment decision and the disk gates that #345 left NO-GO.
- Two existing tests (production-profile ones) hit the `valid_until` check before the digest check, so they fail on or after 2026-11-11. The pre-transition half already did this; I did not change that behaviour.
- The deployed script was only read for hashing (existing `SourceStructure` test); not modified. No expiry was approved or deployed.
- Review focus suggested for #348: (a) the B3 diff is observation-only and its scope; (b) every §4d item above; (c) pins derived from C and the #318 evidence; (d) the "nothing else changed" claim for the lease script; (e) the two edited existing tests are the only weakenings of old assertions, and both only reflect the intended unfreezing.

**Status:** #347 HANDED-OFF. Not accepted, integrated or Unity-qualified.
