# CCF #346 — Workbench 1.1.1 candidate-transition feasibility result

**Date:** 2026-11-08. **Worker:** Claude Sonnet 5.5 (`claude-sonnet-5-5`), Claude Code via T3 Code, effort UNKNOWN. One local worker, no delegates.
**Lane:** `/media/jer/Files/CCF/Operations/CCF-Workbench111TransitionFeasibility-20261108` (sparse), branch `docs/workbench-111-transition-feasibility-20261108-v1`, started at `origin/main` `d294255a9c5fea3a35119dad847637709b2432ff`. **Evidence:** `/media/jer/Files/CCF/Verification/CCF-Workbench111TransitionFeasibility-20261108/`.
**Scope kept:** read-only Git object inspection plus one disposable fixture. No Unity, lease, lock change, Workbench mutation, cleanup, integration or build.

## Verdict

**NO-GO today for any local transition or lease. The Git mechanics themselves are feasible (PASS in the toy fixture), so this is not a dead end.** Two source gaps must be closed first and neither is closed by anything on `main` or on `46bc1fd6`:

1. The reviewed admission source refuses every post-transition request (`post-transition.head = None`, `clean_pinned = {}`, `allowed_changed_paths = None`).
2. Candidate `46bc1fd6` does not contain the full §4d B3 witness. It only has partial player-position logging (see §1.3).

**Single recommended next action:** see §6.

## Summary table

| Question | Result |
|---|---|
| Candidate `46bc1fd6` descends from old HEAD `4d95e452` | **PASS** — ancestor, exactly 7 commits |
| Registered Workbench still at old HEAD / branch, index == old tree, no hidden index flags | **PASS** — read-only, see §1.2 |
| Eight protected working bytes == #337 table | **PASS 8/8** (hash, size) |
| Candidate touches any of the 6 Material/`.vscode` paths | **PASS (no)** — candidate blob id == old blob id for all 6 |
| The 2 package files: working bytes == **candidate** blob | **PASS** — both, exact (blob id and sha256) |
| Plain `merge --ff-only` / `switch` / `checkout` onto the candidate | **Expected REFUSAL** in fixture (both package files "would be overwritten"); all 8 bytes untouched |
| Non-forced transition that adopts the candidate and preserves all 8 | **PASS in fixture** by two variants (§2); not tried on the real lane |
| B3 evidence-only verifier already present | **NO / PARTIAL** — extra verifier source needed |
| Post-transition lease admission possible with reviewed source | **NO** — profile unfrozen |
| Old-HEAD pre-transition lease can serve the witness | **NO** (§3.3) |
| Real lane safety of the transition (exFAT, stat cache, 5,732 paths, Library) | **UNKNOWN** — not provable by a toy |

## 1. Identity and change inventory (read-only)

### 1.1 Ancestry

- Old registered HEAD `4d95e45245d660e24b91e26754d9c57949bf3384` on `feature/scenario-one-workbench-retirement-20261105`. `HEAD` of the registered worktree and the branch ref in the shared store agree.
- Candidate `46bc1fd618ae5b214d7de296a03ac7efec27cb7c` (`proposal/linux-sdk-111-provenance-20261108-v1`): `merge-base --is-ancestor` true, `rev-list --count` = 7. Neither commit is on `origin/main`.
- Commits: `fb5eeb98`, `6b7d2d45`, `e9426769`, `2c4aea1c`, `a4019e6b`, `03b484d1`, `46bc1fd6`.
- `07b6060a` (the #337 BLOCKED report) is one docs commit on top of `46bc1fd6` and adds only one Markdown file.

### 1.2 Changed tracked paths, old → candidate (11 paths)

| Kind | Path |
|---|---|
| Package (M) | `Packages/manifest.json` (`ca552487…`→`9fb01b72…`), `Packages/packages-lock.json` (`8c0c0978…`→`c26dfa4f…`) |
| Verification, modified (M) ×4 | `Tools/Verification/RackV2Persistence/RackV2PersistenceVerification.cs`, `…/StandGeometry/Enlarged80Verification.cs`, `…/WorkbenchRetirement/WorkbenchRetirementVerification.cs`, `…/WorkbenchRetirement/run_workbench_retirement.py` |
| Verification, new (A) ×2 | `Tools/Verification/RackV2Persistence/test_historical_projection.py`, `Tools/Verification/WorkbenchRetirement/test_runner.py` |
| Docs, new (A) ×3 | `T3-WORKBENCH-287-VERIFIER-CLOSURE-RESULT.md`, `WORKBENCH-RECEIPT-R2-FINALIZATION-RESULT.md`, `WORKBENCH-RUNNER-R1-R5-CORRECTION-RESULT.md` (under `Docs/Handoffs/ScenarioOne-Morning-Closure-20261107/`) |

**Packet discrepancy:** the Manager packet says "four already authored verification scripts, three new". Git shows **four modified and two new** verification scripts. The total is still 11. Nothing under `Assets/`, `ProjectSettings/` or `.vscode/` changes.

### 1.3 B3 instrumentation status

- A search of `Tools/Verification/**` at `46bc1fd6` finds no §4d witness (no CharacterController-state capture at Reset/Generate/Reference-preview/end/+1–2 frames, no clearance-anchor capture, no RNG-version record, no duplicate-run or authored-start pose-pin mode).
- The only related code is existing `*_RESET_INPUT … player=<position>` logging in `WorkbenchRetirementVerification.cs` and `Enlarged80Verification.cs`.
- **Conclusion:** the candidate is a package-only candidate plus the earlier runner repairs. A usable B3 witness needs **minimal additional verification-only source**, which changes the candidate HEAD. I did not write or approve it. (Based on text search, not a compile; a reviewer should confirm.)

### 1.4 Eight protected paths: old vs candidate vs working

All eight working files were re-hashed now (read-only; `git hash-object` without `-w`, `sha256sum`). Working sha256/size equal the #337 table (`61d36e47…`, `106f237b…`, `1fd08425…`, `39385672…`, `e2d88892…`, `b32b7b5f…`, `f2ff5f95…`, `f43d9894…`).

| Path | Working == old HEAD blob | Working == candidate blob | Old blob == candidate blob |
|---|---|---|---|
| `.vscode/settings.json` | no | no | **yes** (unchanged) |
| 5 Material `.mat` files | no | no | **yes** (unchanged) |
| `Packages/manifest.json` (2297 B) | no | **YES** (`9fb01b72…`; sha256 `f2ff5f95…`) | no |
| `Packages/packages-lock.json` (14223 B) | no | **YES** (`c26dfa4f…`; sha256 `f43d9894…`) | no |

So **six paths remain dirty after adoption; the two package files stop being dirty** (they become equal to the new HEAD). This is the "eight → six" contraction.

### 1.5 Registered lane read-only state (observed, may drift)

- Index file md5 `decae789…` before and after my inspection (I read it only through `ls-files`/`diff-index --cached` with `GIT_OPTIONAL_LOCKS=0`).
- 5,732 index entries = 5,732 tree entries at old HEAD. `diff-index --cached` against old HEAD is empty (nothing staged). `ls-files -v` shows no non-`H` flag (no assume-unchanged/skip-worktree).
- No Unity process. ZX20 free ≈ 22 GiB (above 20 GiB STOP, ~2 GiB headroom); Files free ≈ 12 GiB (98 % used).
- **Not examined:** untracked/ignored inventory, warm Library state, real exFAT scan time, I/O pressure.

## 2. Git transition semantics, in miniature

**Fixture.** One disposable repo in scratch (`fixture.sh`, copied to evidence). It holds only exact old/candidate blobs of the 11 changed paths plus the 8 protected paths (read with `cat-file` from the shared store), two commits `old`/`target`, `core.filemode=false`, no alternates, isolated config, and the 8 real working-file bytes overlaid. `git status` in the base shows exactly 8 modified. Each variant ran in a fresh copy.

| Variant | Command | Result | 8 bytes intact | Dirty after |
|---|---|---|---|---|
| A | `git merge --ff-only target` | **Refused**: "local changes … would be overwritten": both package files | yes | 8 |
| B | `git switch --detach target` | **Refused**, same two files | yes | 8 |
| C | `git checkout target` | **Refused**, same two files | yes | 8 |
| D | `git switch -m --detach target` | **Succeeded**, HEAD → target | yes | **6**, 0 staged |
| E | `git add` the two package files, then `git merge --ff-only target` | **Succeeded** (true fast-forward) | yes | **6**, 0 staged |
| F | `git add` the two package files, then `git checkout target` | **Succeeded** | yes | **6**, 0 staged |
| G | variant E on a named branch | **Succeeded**, branch advanced | yes | **6**, 0 staged |

Why: Git compares each file to the **index**, which still holds the old blob. Even when the working file equals the target blob it counts as "locally modified" and is refused. Staging the two package files first (E/F/G), or letting `-m` do a content-level merge (D), removes that objection. Candidate bytes equal the working bytes, so no merge conflict or rewrite occurs.

**Interpretation, not overstated.**
- A plain "non-forced fast-forward/switch" **will be refused** on the real lane. The packet's step must be specified as E/G (explicit two-file stage + `--ff-only`) or D (`switch -m`). Both mutate the real index, which this task did not do and which needs explicit Manager authorisation.
- The fixture proves Git semantics for these files **only**. It does not prove safety on the live exFAT worktree: stat/mtime noise on 5,732 entries, timing, Unity-written files, the untracked/ignored set, an unchanged Library, or the behaviour of `git switch -m` over the real index. D especially rests on a 3-way content merge that is trivially clean here but is a wider mechanism than E; **E/G is the narrower choice**.
- The fixture tree is 868 KB in scratch; nothing was written under the shared store or the registered lane.

## 3. Admission and provenance consistency

Source examined: `Tools/Verification/UnityLeaseExactAdmission/unity-lock.proposed` at `24e0de29` (reviewed tested source `11cee0c2…`). Not run.

### 3.1 Can the reviewed source admit a post-transition candidate? **NO.**

`admit_exact_dirty` raises "not frozen by the Manager; unsupported" while `head is None` or `clean_pinned`/`allowed_changed_paths` are unset. The state already encodes the right *shape* (6 dirty paths, `must_descend_from = ['46bc1fd618ae5b214d7de296a03ac7efec27cb7c']`), so the gap is data plus a code edit, not new logic.

### 3.2 Concrete values a separately approved post-transition profile needs

| Field | Value / status |
|---|---|
| `head` | **TBD** — the frozen candidate commit `C`. It can be `46bc1fd6` only if B3 verifier code is not needed; per §1.3 it will be a descendant `C` of `46bc1fd6` |
| `must_descend_from` | `46bc1fd618ae5b214d7de296a03ac7efec27cb7c` (already set) |
| `clean_pinned` | `Packages/manifest.json` → `(f2ff5f95f9e93716947391a8818dbe642d81f0b49068516441a58def966c2c1c, 2297)`; `Packages/packages-lock.json` → `(f43d9894af3c6dc054e86e98335dbd248a9fb97527b49c7b5952d96e037e52b1, 14223)`. Blob == working bytes verified now |
| `dirty` (6) | `.vscode/settings.json` and the 5 `.mat` paths; existing tuples stay valid because candidate blob == old blob (verified by git id). Sizes/modes unchanged |
| `allowed_changed_paths` | the 11 paths in §1.2 **plus** any new `Tools/Verification/**` paths in `C`; nothing outside `Tools/Verification/**`, `Docs/**`, and the two package files |
| Backups / evidence | `evidence_manifest_sha256 = 9fb805968899cb6cd369a6d627ef1c01a7afece2368ff9d2093db1bfa9961f74` (16 files: 8 dirty backups + 8 head-versions); `/media/jer/Files/CCF/Verification/Workbench287MorningClosure-20261107`. The code reuses the pre-transition list for post state, so no change needed |
| Retention of 5 Materials + `.vscode` | They stay as ordinary dirty working files (6 paths) and remain in the #318 backups; transition must not touch them. They remain the reason R1 fails closed |
| `valid_until` | **Unresolved**; neither 2026-11-11 nor "+6 h" is approved (#345 ruling §4) |
| `admission_digest` | must be recomputed for state `post-transition` with the real `head`, and restated by the operator |

Anything that fills `head`/`clean_pinned`/`allowed_changed_paths` edits executable bytes of the reviewed script, so it needs a **delta review** (the same pattern as #343/#344) before deployment. Deployment itself stays **NO-GO NOW** under #345.

### 3.3 Can an old-HEAD pre-transition lease serve the witness? **No.**

- The lock record stores `head` given at acquire (`--head`, checked against the real HEAD). The reviewed script has no mid-lease re-pin; moving HEAD under a held lease leaves a record that no longer names the code Unity runs. This is a provenance break, and #345 ruling item 2 already forbids it. I do **not** read a historical-HEAD lease as authorising any HEAD change.
- A pre-transition lease is not needed for the Git step itself: no Unity is running during it, so a Manager-controlled "no other writer" sequence (transition → verify → separate post-transition admission) is the only coherent order. But both lease states still need the new script deployed, because the **deployed** lock refuses any dirty lane.

## 4. Options compared (no bypass of the global lock; no cold Library)

| Path | Verdict |
|---|---|
| **A. Disposable rehearsal only** (done here) | Done. Settles Git semantics only |
| **B. Source-policy freeze: author candidate `C` (B3 verifier additions only) + fill post-transition profile + delta review** | **Smallest meaningful step.** Needs no Unity, no lease, no lane mutation, no deployment |
| C. Supervised lease now (global lock + unbounded `flock` + raw-scan 1.138 GiB on slow I/O) | Not ready: profile unfrozen, no B3 code, #345 NO-GO stands, 2 GiB disk headroom |
| D. Commit the 6 Editor-written files onto a local lane branch to make the lane "clean" for the default lock | **Rejected here**: it moves the HEAD identity off the reviewed candidate, hides diagnostic drift R1 must keep failing on, and needs Manager approval. Listed only so it is not rediscovered as a shortcut |
| E. A different clean registered lane | Needs a cold Library import, which the disk thresholds forbid (22 GiB < 27 GiB) |

No genuinely simpler existing registered-lane workflow was found.

## 5. Limitations and non-claims

- Fixture ≠ live lane. exFAT stat behaviour, the real 869 KB index, untracked/ignored sets, Library and Editor-written files are untested.
- B3 "missing" is a text search of `Tools/Verification/**` at `46bc1fd6`, not a compile.
- I read `unity-lock.proposed`; I did not run it or its tests, nor re-review its security.
- Not verified: reliable 20-second STOP monitor, licence, I/O pressure, whether UPM re-resolves 1.1.1 on open.
- Shared-store writes by this task: `git fetch origin` (refs/objects, needed to verify `origin/main`) and `git worktree add` for my own docs lane (new branch ref + `worktrees/` metadata). No GC/repack, no change to the registered worktree's metadata (HEAD/index md5 re-checked unchanged) and no Unity.
- Unrelated pre-existing local edits in the coordination anchor (`AGENTS.md`, `Docs/Operations/StorageArchitecture.md`) were left alone.

## 6. Single recommended next action

**Open one source-only task (new Manager packet, Files lane, no Unity) to produce and freeze candidate `C` and its post-transition profile values, followed by a delta review.** Concretely:

1. On a fresh Files lane from `46bc1fd6`, add only the minimal §4d B3 evidence-only verification code under `Tools/Verification/**`; commit; record the exact `C`.
2. Fill the post-transition profile (§3.2) with `C`, the two pinned package hashes/sizes and the exact `allowed_changed_paths`; recompute the digest; keep expiry as a separate Manager decision.
3. Re-run the fixture with `C` (`fixture.sh` + `trials.sh` take the revs as variables) and specify the transition as variant **E/G** (stage the two package files, then `--ff-only`), with a post-transition 8/8 → 6/6 hash check.
4. Independent delta review of the script diff. Only then may a separate Manager decision reconsider deployment/lease.

**Why:** it resolves the only two hard blockers (unfrozen profile, absent B3 code) with zero Unity, lease, lock or lane risk, and it lets the Manager judge the exact bytes before anyone touches the warm Workbench.

**Status:** #346 HANDED-OFF. STOP for Manager #200. Not accepted, integrated or Unity-qualified.
