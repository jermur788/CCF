# #344 — Independent delta review of the N-1 tree-object integrity repair (source only)

**Verdict (SOURCE ONLY): APPROVED** for the N-1 delta. This is **not** deployment approval. O-3 (global flock held during the 1.138-GiB raw scan with no read timeout), the unapproved 2026-11-11 expiry, `post-transition` UNSUPPORTED and the strict-clean default path's reliance on Git status all remain separate Manager gates.

## Identity and independence

| Item | Value |
|---|---|
| Reviewed tested source | `11cee0c26c81b916c77a5310370d7ce42f2a203b` (base `6b111b123dc5d03258e0481c499b0ca634b11a1d`) |
| Author report-only successor | `24e0de290bfdfa9d5a584e1c9e6d87bd8c18bb1c` (adds one path: `UNITY-LOCK-N1-TREE-INTEGRITY-SOURCE-REPAIR.md`) |
| Reviewer branch / lane | `docs/unity-lease-n1-integrity-review-20261108-v1`, `/media/jer/Files/CCF/Operations/CCF-UnityLeaseN1TreeIntegrityReview-20261108` (new; created from freshly fetched `origin/main` `9c881200803f4027a0eb12f4a60c996bea2e40f7`) |
| Reviewer commit | the commit that adds this file (published in the #344 HANDED-OFF comment); it changes this one path only |
| External evidence | `/media/jer/Files/CCF/Verification/CCF-UnityLeaseN1TreeIntegrityReview-20261108/`, `SHA256SUMS.txt` (48 entries) sha256 `37186ed821af01deccd964d82c5376c396ec0f39e0e7c23a51b51bf66c613a1c` |
| Reviewer | Anthropic **Claude Opus 5.5** (`claude-opus-5-5`), reasoning-effort setting 15 (numeric; any High/Medium label UNKNOWN) |
| Author | Anthropic Claude Sonnet 5.5 |

**Same-provider limitation:** the reviewer is a different model and a different session from the author, but **the same provider (Anthropic)**. This is **not** a cross-provider review. The preferred cross-provider reviewer (Codex) was refused by that provider's filter, and the user chose this fallback. Model-family blind spots shared between author and reviewer cannot be ruled out.

Source was read only from independent `git archive` extracts of `11cee0c2` and `6b111b12` in the evidence directory (`src-11cee0c2/`, `src-6b111b12/`). Every script ran with `python3 -I -B`, never with an extract as cwd, `CCF_LEASE_TEST_TMP=<evidence>/tmp`. The author's evidence was treated as claims. Its `SHA256SUMS.txt` verifies (7/7 OK), and I reproduced the claims independently.

## Findings

| ID | Severity | Status | Summary |
|---|---|---|---|
| N-1 | MEDIUM (release-blocking at #341) | **RESOLVED** | A forged non-root subtree now refuses (exit 2, no `lock.json`, no new history event) across 6 independent forgery shapes, including packed and alternates. Each shape false-accepts on the old source. |
| R-1 | LOW, pre-existing, non-blocking | open (recommendation) | `prove_raw_working_tree` resolves `HEAD^{commit}` again instead of using the pinned `st['head']`. |
| R-2 | INFO | open (recommendation) | The `ls-tree` vs verified-walk comparison is redundant defence-in-depth, and no test fails if it is removed. |
| R-3 | INFO | accepted | Fail-closed refusal of hash-valid but legacy trees (`100664`, `040000`, nested `.Git`). The real pinned HEAD has none of these (checked read-only). |
| R-4 | INFO, pre-existing | accepted | The CLI `--head` regex is 40-hex, so a sha256 repository cannot reach admission through `main()`. The new walk itself handles sha256 correctly (function level). |
| R-5 | INFO | out of scope | `cat-file --batch` reads have no timeout (O-3 class). Verification uses plain hashlib SHA-1, so forging an existing object needs a SHA-1 second preimage (infeasible). |

**R-1 detail.** `main()` checks `actual_head == a.head`, `admit_exact_dirty` checks `head == st['head']`, and `require_canonical_object_graph` checks `HEAD^{commit} == want_head`. `prove_raw_working_tree` then calls `rev-parse HEAD^{commit}` again and verifies whatever commit that returns. Suppose a concurrent writer moves the lane HEAD inside that window to a commit X that matches the working tree. The raw proof is then of X while the audit record says `head` = the pin. `raw_proof.verified_commit` would record X, but nothing refuses on it. The old source had the same window (`ls-tree … HEAD`), so this is **not a regression**. It also needs live write access to the lane, and such a writer could change bytes right after a lease anyway. Suggested one-line hardening for a later revision: pass `st['head']` into `prove_raw_working_tree`, or refuse unless `raw_proof['verified_commit'] == st['head']`. **Status: inspected only. I did not run a race test.**

**R-2 detail.** Mutant m2, which disables the comparison, still passes the author's `ForgedObjects` suite. Mutant m1, which disables the hash check, fails 2 tests and false-accepts my depth-1 case. So the hash check is the load-bearing control and the tests do test it. The comparison only guards a narrow case: an object swapped between the walk and the `ls-tree` call. For that case it is sound, because downstream uses `ls-tree` entries only when they equal the verified inventory. Note that this means the author's "concurrent writer between walk and ls-tree is not defended" understates the code: that particular swap is refused. The broader file-read window remains.

## 1. N-1 reproduction (reviewer-written forgery code, `scripts/n1_review.py`)

Harness reused from source: `Fixture`, `run` and `load_script` from `test_exact_admission.py`. These provide the disposable bare store, registered worktree lane, fake lock dir and stubbed ZX20 identity. The forgery code is my own. Each case runs as follows:

1. Positive control on the honest fixture for both scripts: exit 0, lock, 1 `exact-admission` event, then release.
2. Forge the object.
3. Assert Git is fooled: HEAD is still the pin, `diff-index --cached HEAD` is empty and status shows no non-pinned entries.
4. Run exact acquire with old `6b111b12` and new `11cee0c2`.

| Case (`logs/case-*.jsonl`) | Forgery | Old `6b111b12` | New `11cee0c2` |
|---|---|---|---|
| `depth1` | Loose `Assets` tree (non-root) overwritten in place under its own id, `Other.cs` → drifted blob, index updated | **exit 0, lock, +`exact-admission`** (FALSE ACCEPT) | exit 2 "Git object 9f8d7bf5… content does not match its object id (corrupt or forged tree)", no lock, no new event |
| `depth3_inject` | Depth-3 tree `Assets/ForestPrototype/Art` gains a new tracked entry `Injected.cs` that exists only in the working tree | **FALSE ACCEPT** | exit 2 (hash mismatch), no lock, no event |
| `symlink_mode` | Forged `Tools` tree changes `old.py` from mode 100644 to **120000**; the working tree holds the matching symlink | **FALSE ACCEPT** | exit 2, no lock, no event |
| `alternates` | Genuine loose `Assets` tree **deleted** from the store; forged copy under the same id served from `objects/info/alternates` | **FALSE ACCEPT** | exit 2, no lock, no event |
| `packed` | **Pack forgery:** a reviewer-written pack v2 + idx v2 writer repacks every object with correct CRC32, pack trailer and idx checksum, except that the `Assets` entry holds forged tree bytes under the genuine id. No loose objects remain. (`git verify-pack` itself reports a validation error, rc 1, but normal reads do not check.) | **FALSE ACCEPT** | exit 2, no lock, no event |
| `commit_graph` | Only `objects/info/commit-graph` forged: the pinned commit's CDAT root-tree slot points at a genuine alternative root tree; checksum recomputed; all objects honest | exit 2: Git status uses the graph, so the pre-filter is fooled, but `ls-tree` reads the real commit, so the raw proof refuses ("working file differs … Assets/Other.cs") | exit 2 at pre-filter ("unexpected Git status entry 'M  Assets/Other.cs'") because `core.commitGraph=false` |

The `sha256_*` CLI runs (`case-sha256_*.jsonl`) end at argparse for *both* scripts, including the honest control, because of R-4. They are therefore not evidence of forgery handling and are superseded by the function-level runs below.

**Function level (`scripts/n1_funcs.py`, `logs/funcs.jsonl`), on sha256-format fixtures:**
- Honest: new `prove_raw_working_tree` passes (12 verified trees).
- Depth-1 forgery: old returns a proof (false accept), new refuses with a hash mismatch.
- Depth-3 inject: old returns a proof with 12 tracked files (the injected file was admitted), new refuses.

**Author cross-check (`logs/author-tree-test-*.txt`):**
- New source: `test_tree_object_integrity.py` 13/13 OK.
- Old source (`CCF_LEASE_TEST_PROPOSED`), `ForgedObjects` only: both sub-root forgeries FAIL (= false accept), the stats test ERRORs because the field is missing, and the root-tree/commit forgeries pass. This matches the author's claim that Git's `parse_object` already re-hashes the root tree and commit.

## 2. Algorithm inspection (`unity-lock.proposed` at `11cee0c2`)

**Hashing.**
- `_BatchObjectReader.read` refuses a malformed id (wrong hex length for the repository format), `missing`, a header whose echoed id differs from the request, a non-digit size, the wrong type and size above 64 MiB. It refuses these *before* reading the payload.
- It also refuses truncation and a missing trailing LF.
- It then hashes `kind + b' %d\0' % size + payload` and compares the result with the **requested** `oid` argument. Git's type and size labels feed the hash, so any mislabel changes the digest. Git is never trusted for identity.
- I checked: short id refused, missing commit refused, mistyped id refused (author test), size bound refused, commit without a leading `tree` line refused.

**Process handling.**
- One request is in flight at a time. Without `--buffer`, `cat-file --batch` flushes after each object, so there is no pipe deadlock.
- stderr goes to DEVNULL.
- `close()` runs `kill()`+`wait()` in `finally` on every success and exception path.
- After 40 failing walks the process had no remaining child processes (`children: []`), and no `cat-file --batch` process was left on the host.
- Spawn failure maps to `ValueError`, which becomes exit 2.
- A hung Git process would block without a timeout (R-5, O-3 class).

**Walk.**
- Iterative, deterministic and bounded. Each distinct tree is hashed *before* it is parsed. Shared subtrees are expanded at every path, which is the correct inventory semantics.
- Bounds: 2 M objects, 5 M entries (inventory + stack) and 64 MiB per object. These are sensible fail-safes, because once hashing is enforced only genuine pinned trees can be parsed.

**Tree parsing.**
- Refused: truncation or trailing garbage, modes other than 100644/100755/120000/40000 (so 160000 submodules, legacy `100664` and `040000` are refused), empty, `.`, `..` and slash-containing names, `.git` in any case at any depth, non-UTF-8 names and repeated names (including a file and a directory with the same name).
- Accepted: unsorted but hash-valid entries, and names containing `\n`, `\t` or `\\`. Both are safe: the listing comparison is order-independent and `ls-tree -z` handles those names.
- Escape: the walk opens files with `O_NOFOLLOW` and symlink entries are compared as target bytes, so no tree name escapes the lane.

**Listing comparison.** It is complete: a dict equality over the full path set with (mode, blob id) values. `ls-tree` now runs on the resolved commit id. Blobs are not re-hashed in the walk, which is correct: blob ids are only compared with hashes of raw working bytes. The pinned-dirty `HEAD:path` sha256 check is independent of blob store content.

**Commit-graph / alternates / replace.**
- `core.commitGraph=false` applies to every admission Git process. The walk reads the commit object directly, so a forged graph cannot redirect it (case `commit_graph`).
- Alternates are not refused. Objects served from them are hash-verified like any other (case `alternates`).
- Replace refs, grafts and shallow history are still refused, and `GIT_NO_REPLACE_OBJECTS` / `core.useReplaceRefs=false` are kept.

**Read-only production sanity (`scripts/real_head_readonly_walk.py`, `logs/real-head-readonly-walk.json`).**
- Method: my own walker read the pinned HEAD `4d95e452…` from `/media/jer/Files/CCF/Repository.git` with `cat-file --batch` only, `GIT_OPTIONAL_LOCKS=0`, writing nothing.
- Result: 567 distinct trees (377,511 bytes), all hash-valid. Modes are only `100644`/`100755`/`40000`, with no unsafe or non-UTF-8 names. The walk took 0.80 s on Files ext4 (not exFAT or ZX20).
- So the repair's stricter tree rules (R-3) would not refuse the current production pin.
- The shared store has `objects/info/commit-graph` (neutralised by the repair), no alternates file, no `info/grafts` and 0 `refs/replace`.
- This does not run the proposed script on any real lane.

## 3. Regression controls on `11cee0c2` (existing tests by name, one pass)

- `test_raw_byte_admission.py`, 8/8 OK (`logs/regress-raw.txt`): F1 replace ref, F1 info/grafts, F2 local clean filter, F3 same-size edit with restored mtime, sentinel plain unrelated change, sentinel default path not using the new proof, sentinel exact-eight positive then exclusive lock blocks a second request, sentinel held lock checked before raw work.
- `test_exact_admission.py`, 11/11 OK (`logs/regress-exact.txt`): exact-eight accepted and audited, re-acquire blocked by the exclusive lock, default clean passes identically, default dirty denied identically to deployed, held lock blocks, malformed lock blocks, patch reproduces proposed, baseline/helper pinned bytes (including a read-only hash of the live deployed files), release/status/main wrapper unchanged, production profile one-lane and post-transition unfrozen/refused.

The full 77+40 suites were **not** repeated, because no new issue required it.

**Identity:**
- Base→tested diff is exactly three paths (`logs/diff-name-status.txt`): `unity-lock.proposed` M, `unity-lock-exact-admission.patch` M, `test_tree_object_integrity.py` A. The successor `24e0de29` adds only the author report.
- Deployed-baseline copies are byte-identical at base and tested source: `unity-lock.deployed-54cf0a95.txt` `54cf0a95…`, `_ccf.deployed-ab4343ef.txt` `ab4343ef…`. The original test files are unchanged too: `test_exact_admission.py` `7b9122f3…`, `test_raw_byte_admission.py` `2b612105…`.
- The live deployed `/media/jer/ZX20/CCF-Ops/scripts/unity-lock` and `_ccf.py` still hash to `54cf0a95…` / `ab4343ef…` (read-only).
- Independent `patch -p1` of the baseline copy with the tested patch is `cmp`-identical to `unity-lock.proposed` (sha256 `62a97b45…`; patch `4b93c900…`; new test `16f8127d…`).

## Commands (from `<E>/logs`, `E=/media/jer/Files/CCF/Verification/CCF-UnityLeaseN1TreeIntegrityReview-20261108`, `S=$E/src-11cee0c2/Tools/Verification/UnityLeaseExactAdmission`, `O=$E/src-6b111b12/...`, `CCF_LEASE_TEST_TMP=$E/tmp`)

```
git -C /media/jer/Files/CCF/Repository.git archive <sha> Tools/Verification/UnityLeaseExactAdmission | tar -x -C $E/src-<sha8>
python3 -I -B $E/scripts/n1_review.py $O/unity-lock.proposed $S/unity-lock.proposed $S <depth1|depth3_inject|symlink_mode|alternates|packed|commit_graph>
python3 -I -B $E/scripts/n1_funcs.py $O/unity-lock.proposed $S/unity-lock.proposed $S $E/scripts
python3 -I -B $S/test_tree_object_integrity.py -v                                   # new: 13 OK
CCF_LEASE_TEST_PROPOSED=$O/unity-lock.proposed python3 -I -B $S/test_tree_object_integrity.py -v ForgedObjects   # old: 2 FAIL, 1 ERROR
CCF_LEASE_TEST_PROPOSED=<mutant> python3 -I -B $S/test_tree_object_integrity.py ForgedObjects   # m1: 2 FAIL; m2: OK (mutation-diffs/)
python3 -I -B $S/test_raw_byte_admission.py -v -k test_F1_replace_ref -k test_F2_local_clean_filter -k test_F3_same_size_edit -k test_F1_info_grafts -k test_sentinel_
python3 -I -B $S/test_exact_admission.py -v -k test_exact_eight_file_state_accepted -k test_default_clean_lane_passes -k test_default_dirty_lane_denied -k test_held_lock_blocks -k test_malformed_lock_blocks -k test_reaccepting_second_time -k test_patch_reproduces -k test_baseline_and_helper -k test_release_status_and_main_wrapper -k test_production_profile
python3 -I -B $E/scripts/real_head_readonly_walk.py /media/jer/Files/CCF/Repository.git 4d95e45245d660e24b91e26754d9c57949bf3384
```

## Not done / limits

- No race test for R-1, no test of a hung `cat-file`, no exFAT/ZX20 timing, no SHA-1 collision case.
- I did not test a forged *blob*. Blobs are never trusted by design, and blob content is checked through raw working-byte hashes.
- Mutant files were deleted after the run; only their one-line diffs are kept in `mutation-diffs/`.
- All fixtures, `tmp/` and the mutants were removed.
- Untouched: source, deployed `unity-lock`/helpers/config, real lock/lease, Unity, ZX20 (one read-only hash of two deployed files only), protected or warm worktrees, shared-store objects (read-only `cat-file`/`for-each-ref`/`ls` only), gc/repack/cleanup, integration, build and the historical #340/#341/#343 lanes and evidence.
