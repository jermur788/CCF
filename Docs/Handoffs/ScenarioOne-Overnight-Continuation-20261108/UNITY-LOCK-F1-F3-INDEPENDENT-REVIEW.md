# #341 Independent review of the #340 F1–F3 lease-guard repair — NEEDS CHANGES (SOURCE ONLY)

Date: 2026-11-08. Reviewer: **Anthropic Claude Opus 5.5 (`claude-opus-5-5`)**, Claude Code / Agent SDK harness. Harness-visible reasoning effort: numeric `10`; mapping to a named level is **UNKNOWN**. No subagents or other sessions. No source repair, deployment, real lease, Unity, ZX20 write, integration, PR or build.

**Verdict: NEEDS CHANGES — SOURCE ONLY.** The repair closes all three named #339 false accepts (F1 replace refs, F2 clean-filter/normalisation, F3 stat cache). It also closes nine further variants I constructed. On the old source those variants are red where they apply. Positive, default-clean and lock safeguards are intact. However, a **reviewer-owned bypass of the same canonical-identity class as F1** still acquires a fixture lease on the exact tested source (finding **N-1**). The repair's raw proof trusts `git ls-tree HEAD` to report the canonical tree. Git does not check the hash of subtree objects it reads during that recursive walk, so a forged subtree object makes unapproved working bytes look canonical. The pinned HEAD SHA remains unchanged throughout. The fix is small and bounded (see N-1). Everything else reviewed is acceptable source-only.

## 1. Identity, independence and history

| Item | Value |
|---|---|
| Binding packet | `Docs/Handoffs/ScenarioOne-Overnight-Continuation-20261108/MANAGER-339-ADVERSE-SOURCE-REVIEW-AND-F1-F3-REPAIR-PACKET-20261108.md` at origin/main `05e141821f2b2869436a8e01e24bb77022df65dc` (§3, §4, final clarification read in full) |
| Exact tested source reviewed | **`6b111b123dc5d03258e0481c499b0ca634b11a1d`** |
| Report-only publication head | **`3c5577f666fc3a793e76afe70c9420a2387c4ae8`** (origin branch tip verified via `git ls-remote`; adds only the author report; `Tools/Verification/UnityLeaseExactAdmission` tree id `18ab9c5b…` identical at both commits) |
| Base (prior tested source) | `949cc0c19bed09ba3fad4c319e235575ac75f6a8` |
| Author branch / lane | `fix/unity-lease-f1-f3-raw-byte-guard-20261108-v1` / `/media/jer/Files/CCF/Operations/CCF-UnityLeaseRawByteRepair-20261108` (**not touched**; reviewed from `git archive` of the exact SHA) |
| Review branch / lane | `docs/unity-lease-f1-f3-independent-20261108-v1` from origin/main `05e14182…` / `/media/jer/Files/CCF/Operations/CCF-UnityLeaseRawByteReview-20261108`, common store `/media/jer/Files/CCF/Repository.git` |
| Evidence | `/media/jer/Files/CCF/Verification/CCF-UnityLeaseRawByteReview-20261108/run-opus-20261108T1647Z/` (`SHA256SUMS.txt` sha256 `0195649a7955ff7631149e7568ac754c1da75d04bb88b6bc1fa172880a701c0e`, 30 entries, plus `README.md`) |
| Own RUNNING receipt | [#341 comment 6724229400](https://github.com/jermur788/CCF/issues/341#issuecomment-6724229400), 2026-11-08T16:47:28Z |

**Independence and deviations.**
- **Model independence.** The reviewer model (Opus 5.5) differs from the #340 author model (Claude Sonnet 5.5), but both come from **the same provider (Anthropic)**. The packet preferred a cross-provider reviewer. This reviewer was launched at the user's explicit request after two OpenAI Codex reviewer attempts failed. The first hit a usage limit and did no work. The second was gpt-6.1-sol, which posted a [RUNNING] receipt at 13:49Z and was then terminated mid-review by a provider "cybersecurity" flag.
- **Stale RUNNING.** That Codex RUNNING was stale. It produced no report, commit or verdict (orchestrator note at 13:53Z). My RUNNING receipt explicitly supersedes it.
- **Codex scratch not used.** The Codex scratch files in the parent evidence directory are **UNVALIDATED**. They were neither used as evidence nor modified. All my evidence is in the fresh `run-opus-20261108T1647Z/` subdirectory.
- **Receipt order.** Before RUNNING, I did preflight reads only: the packet, issues, `active-tasks.md`, worktree/branch/status checks, `df` and `git ls-remote`. No author source was read, and nothing was executed before the receipt.
- **Stale coordination file.** Live `active-tasks.md` still shows #340 READY / #341 WAITING, which lags the issue state. I did not edit it (Manager-only).
- **Disk space.** Files disk had 16 GiB available at preflight and at the end.

## 2. Findings

### N-1 — MEDIUM (blocking for a fail-closed contract): a forged subtree object defeats the canonical-identity guarantee

**Defect.** `prove_raw_working_tree` (unity-lock.proposed lines 180–181) takes the canonical blob ids from `git ls-tree -r -l -z --full-tree HEAD`. Git verifies the hash of the commit/root tree it resolves through `parse_object`, but it does **not** re-hash subtree objects read during the recursive tree walk. Nor do `status` or `diff-index` re-hash them.

**Effect.** If the object file for a canonical HEAD subtree is replaced with another tree's content, ls-tree, the status/diff pre-filter and the pinned-path `cat-file blob HEAD:<path>` all describe the forged tree. Meanwhile `rev-parse --verify HEAD^{commit}` still equals the pin, and none of the following exist: replace refs, grafts, shallow history.

**Contradicted claims.** This falsifies the author's claim (§2 and the code comment) that the proof compares against the "canonical HEAD tree … read from tree objects only". It also falls under the packet's F1 rule: "the displayed HEAD SHA alone is not evidence that the inspected tree is canonical … conservatively reject any … altered identity mechanism".

**Reproducer** (disposable fixture; `tools/reviewer_probes.py` probe `f1_forged_loose_tree`, result in `out/probes-new.json`):
1. Fresh fixture (the author's `Fixture` plumbing: bare store, linked worktree lane, the eight pinned dirty files). Positive control: exact admission acquires, then is released.
2. Build a drift tree identical to HEAD's except `Assets/Other.cs` → `class ReviewerForgedTree {}\n`, written with a temporary index plus `hash-object -w`, `update-index --cacheinfo` and `write-tree`.
3. Overwrite the loose object file for the canonical `HEAD:Assets` subtree id with the drift tree's `Assets` subtree object file (`os.replace`).
4. Write the drift bytes to `lane/Assets/Other.cs`, then `git add Assets/Other.cs`, so the index agrees with the forged view.
5. Run exact acquire with the correct profile, state, digest, reason and receipt.

**Result on exact `6b111b12`:** `exit 0`, `lock.json` created, `exact-admission` event recorded (**FALSE ACCEPT**). `ls-tree -r HEAD Assets/Other.cs` reports the drift blob `a343727e…`, while the canonical blob is `1ecc246e…` (`class Other {}`). `git fsck` reports `hash-path mismatch`. The old source `949cc0c1` also accepts, so this gap predates the repair; the repair does not close it.

**Applicability and limits** (`tools/forged_object_followup.py`, `out/forged-followup-new.json`):
- **Already-packed objects resist it.** If the canonical subtree is already in a pack, Git prefers the pack. A forged loose copy (case a) and an extra hand-built forged pack (case b) were both ignored, and acquisition was refused through the pre-filter. So the attack needs the canonical tree object to be loose, or the existing pack to be rewritten.
- **The real store has loose objects.** I made one read-only observation of the shared store (`/media/jer/Files/CCF/Repository.git`, which my own review worktree also uses; **not** the registered lane, and the proposed function was **not** run against it). There, **48 of the 568** commit/tree objects reachable from the pinned production HEAD `4d95e452…` are loose. The precondition therefore exists in the real store today.
- **What the attack requires.** Deliberate low-level writing into the object store, which no Git porcelain command does. That is why I rate it **MEDIUM**, not HIGH. The threat class is the same as F1, though: F1 also requires a write into the common store.

**Bounded fix (suggestion only; I did not modify author source).** Hash-verify every commit and tree object the proof relies on before trusting it. One way:
1. Read HEAD and, recursively, every tree through one `git cat-file --batch` process.
2. Recompute `<type> <len>\0<content>` with the repository's object format, and require it to equal the requested id.
3. Derive the expected `(path, mode, blob id)` list from those verified trees, instead of from `ls-tree`. (Or keep `ls-tree` and require its ids to match the verified walk.)

My demonstration in case (d) detected the forgery: `object content does not hash to its id: 9f8d7bf5…`. In the fixture that meant 12 trees. For the real HEAD it means about 568 objects, all small, so the cost is negligible next to the raw-byte read. `git fsck --connectivity-only` does **not** detect it; a full `git fsck` does, but it is store-wide and costly.

**Same fix covers related variants.** The same verification also covers forged commit objects and `objects/info/alternates` substitution of missing objects. Neither was tested separately (marked NOT RUN below).

**Optional hardening.** Add `-c core.commitGraph=false` to `git_exact`. See O-1.

### O-1 — INFO: a forged commit-graph already fools the old source, and the repair catches it only because of Git 2.43 behaviour

**Setup.** A forged `objects/info/commit-graph` (valid checksum) that maps the pinned commit to a drift root tree.

**Result.** This makes `diff-index --cached HEAD` agree with drift staged in the index, so the old source **accepts** (red). The repair **refuses** it: `ls-tree` on Git 2.43.0 parses the real commit object, and the raw proof then reports `working file differs from the canonical HEAD blob`. PASS today. Running Git with `core.commitGraph=false` would make this independent of Git version.

### O-2 — LOW / disclosed limits (accepted as disclosed; no change requested)

- **Mode/exec bit.** The mode of unrelated tracked files is not compared. Probe: `chmod +x Tools/old.py` is accepted. On exFAT this cannot be represented anyway, and it is disclosed.
- **Empty directories.** Empty untracked directories are tolerated (disclosed). Unity may later create a folder `.meta` for them, but that is post-admission and holds no source bytes.
- **Tracked symlinks.** These are compared by link target; a symlink replacing a regular file is refused.
- **TOCTOU.** The proof certifies bytes only at check time. The guard flock serialises lock metadata and does not freeze the lane, so a writer can change files after the proof. This matches the deployed `git status` semantics and is not a regression.
- **HEAD resolved twice.** `ls-tree … HEAD` re-resolves `HEAD` after `require_canonical_object_graph` has pinned it. Passing the pinned SHA instead would remove a narrow ref-update race (cosmetic).
- **Default path unchanged.** The default (non-exact) path is unchanged and, by its nature, still trusts `git status`; the author discloses this. It is out of this repair's scope by packet rule 3. It is a residual Manager risk for the deployed default gate, not a defect of this proposal.

### O-3 — LOW: no timeout while holding the guard (disclosed)

The raw proof runs inside the exclusive guard flock and has no read timeout.

- **No false accept.** A hung or failed read cannot create a lock or an admission event. The lock record is written only after the proof returns, and every error path exits 2. Probes confirmed this for unreadable directories and unreadable Git objects.
- **Availability impact only.** Other `status`/`release` callers wait. I confirmed that acquisition blocks while another process holds the guard, writes no lock while blocked, and completes after release (4.15 s with a 4 s hold).
- **Bounded by interruption.** Killing the process releases the flock.

I rate this an availability limit (LOW), acceptable for a Manager-gated, operator-attended admission. If the Manager wants it bounded, a wall-clock budget that refuses on expiry would remain fail-closed.

**Performance.** I independently measured warm cache on a disposable 4000-file / 403.6 MB repo: 0.88–1.19 s, about 458 MB/s, versus 0.017 s for `git status`. That is consistent with the author's ~2.1 s warm for 1.14 GiB. I did not measure cold cache (no cache-drop privilege) or the ZX20/exFAT device. The author's estimate of 12–40 s cold on ZX20 is plausible but unverified.

## 3. PASS / FAIL / NOT RUN

All probes use disposable fixtures under the evidence root. "Refuse" means all three of: exit 2, no `lock.json`, and no new history event (in particular no `exact-admission`). Each probe first passes a positive admit-and-release control. Each attack precondition was checked separately: ordinary `git status` was fooled, and the displayed HEAD was still the pin (`precondition_*` fields in the JSON).

Command for every row in this table (run once with `<label>=new` and `src`, once with `<label>=old` and `src-old`):

```
python3 -I -B tools/reviewer_probes.py <src>/Tools/Verification/UnityLeaseExactAdmission <src|src-old>/…/unity-lock.proposed <label> fixtures/full out/probes-<label>.json
```

| # | Probe | Old `949cc0c1` | Repair `6b111b12` |
|---|---|---|---|
| 1 | F1 replace ref: pinned commit → drift commit (staged) | FALSE ACCEPT | **PASS refuse** (replacement refs) |
| 2 | F1 replace ref on the HEAD **tree** object | FALSE ACCEPT | **PASS refuse** |
| 3 | F1 replace refs under caller `GIT_REPLACE_REF_BASE=refs/alt/` | refuse | **PASS refuse** (env stripped; non-discriminating) |
| 4 | F1+ reviewer: forged commit-graph root tree | FALSE ACCEPT | **PASS refuse** (raw bytes); see O-1 |
| 5 | **F1+ reviewer: forged loose `Assets/` subtree object** | FALSE ACCEPT | **FAIL — FALSE ACCEPT (N-1)** |
| 6 | F2 `core.attributesFile` (repo-local) clean filter on `Tools/old.py` | FALSE ACCEPT | **PASS refuse** (raw bytes) |
| 7 | F2 `working-tree-encoding=UTF-16` | FALSE ACCEPT | **PASS refuse** |
| 8 | F2 `text eol=crlf` via `info/attributes` | FALSE ACCEPT | **PASS refuse** |
| 9 | F3 non-racy same-size edit, restored mtime, `trustctime=false` `checkStat=minimal` (different file from #339) | FALSE ACCEPT | **PASS refuse** |
| 10 | F3 fsmonitor-valid bit plus repo fsmonitor hook | refuse | **PASS refuse** (`core.fsmonitor=false` forced) |
| 11 | assume-unchanged / 12 skip-worktree / 13 intent-to-add | refuse | **PASS refuse** |
| 14 | exec-bit flip on unrelated file | accept | accept (**disclosed limit**, O-2) |
| 15 | empty untracked directory | accept | accept (**disclosed tolerance**) |
| 16 | case-variant `assets/other.cs`; 17 NFD-named extra file | refuse | **PASS refuse** |
| 18 | nested `.git/` directory with content | FALSE ACCEPT | **PASS refuse** (untracked) |
| 19 | extra ignored `Temp.csproj` not in allow list | refuse | **PASS refuse** |
| 20 | unreadable tracked directory (`chmod 000 Tools`) | FALSE ACCEPT | **PASS refuse** (raw proof could not read) |
| 21 | genuine Git error: HEAD tree object unreadable | refuse | **PASS refuse** |
| 22 | deleted tracked file; 23 file→directory; 24 pinned parent dir → symlink to identical copy | refuse | **PASS refuse** |
| 25 | positive: exact eight admits; second request `lock exists`; wrong-owner release refused; force without `--manager-approval` refused; status `held`; owner release; events `['exact-admission','release']`; `raw_proof.hashed_files == 3` | n/a | **PASS** |
| 26 | stale (dead PID) and malformed lock block acquisition; status `stale`/`malformed`; plain release of malformed refused | PASS | **PASS** |
| 27 | default path: dirty refused (`Unity lane is dirty`), clean admitted without `exact_admission` field | PASS | **PASS** |
| 28 | one ordinary unrelated raw change refused | PASS | **PASS** |
| 29 | guard flock held by another process: acquire blocks, no lock while held, completes after release | PASS | **PASS** |

Other checks (each run once):

| Check | Command / evidence | Result |
|---|---|---|
| Changed paths vs base | `git diff --name-status 949cc0c1 6b111b12` | **PASS**: only `test_raw_byte_admission.py` (A), `unity-lock-exact-admission.patch` (M), `unity-lock.proposed` (M); publication adds only the report |
| Baseline copies byte-identical | `git diff --quiet 949cc0c1 6b111b12 -- …` | **PASS**: `unity-lock.deployed-54cf0a95.txt`, `_ccf.deployed-ab4343ef.txt`, `test_exact_admission.py` unchanged |
| Patch reproduces proposal | `patch -p1` on a copy of `unity-lock.deployed-54cf0a95.txt` (`patchcheck/`), then `cmp` | **PASS**: byte-identical, sha256 `b0720660…`; the only removed line is the default `status --porcelain` gate, re-added under `if a.exact_admission is None` |
| Deployed files unchanged (read-only sha256) | `sha256sum /media/jer/ZX20/CCF-Ops/scripts/{unity-lock,_ccf.py}` | **PASS**: `54cf0a95…`, `ab4343ef…` |
| Pinned eight hashes / #318 manifest / profile / digest / evidence check / `main()` | text comparison old vs new (`structure-checks.log`) | **PASS**: identical; production digest still `90da4135…` |
| Release / status / flock / PID helpers and `__main__` wrapper | text comparison vs deployed baseline | **PASS**: identical |
| Author targeted suite | `CCF_LEASE_TEST_TMP=… python3 -I -B test_raw_byte_admission.py -v` | **PASS 40/40** (6.6 s) |
| Author baseline suite | `… test_exact_admission.py` (once) | **PASS 77/77** (10.7 s). Reads #318/#337 evidence and deployed hashes read-only |
| Author red reproductions on old source | `CCF_LEASE_TEST_PROPOSED=<src-old>/… test_raw_byte_admission.py FalseAcceptReproductions GraftOnPostFixture` | **Reproduced**: 5 FAIL (genuine false accepts), sentinel ok |
| N-1 applicability follow-up | `tools/forged_object_followup.py` | loose-beside-pack refused; extra forged pack refused; loose-only **accepted**; verified traversal **detects** |
| Performance (warm) | `tools/perf_probe.py` (disposable repo deleted afterwards) | 0.88–1.19 s / 403.6 MB |

**NOT RUN.**
- exFAT or other real-device behaviour, including inode/stat noise and case-insensitivity on exFAT (fixtures are ext4).
- Cold-cache timing.
- Forged commit object, and `objects/info/alternates` substitution (same class as N-1; covered by the same fix).
- Submodule fixture. The parser refusing gitlinks is covered by the author's unit test only.
- Concurrent writer during the proof.
- Mutation campaigns (deliberately not repeated, per packet).
- Anything on the real registered lane, a real lease, Unity, or the post-transition production state (still `head=None`, UNSUPPORTED).

## 4. Assessment of the repair against the packet's acceptance boundary

1. **F1 canonical identity: PARTIAL.** Replace refs, grafts and shallow history are refused or neutralised, and `GIT_*` env is stripped. Tree content, however, is still trusted without hash verification (N-1).
2. **F2/F3 raw tracked bytes: PASS** for filters, eol/encoding normalisation, stat cache, index flags, untracked/ignored extras, symlink/dir/file substitution, deletions, unreadable paths and Git errors. Correctness rests on the expected-id list, which N-1 shows can be forged.
3. **Existing safety semantics: PASS** (verified textually and behaviourally). The proposal keeps the November 11 expiry only as a provisional constant, not adopted as policy here. Post-transition stays unsupported.
4. **Bounded verification: PASS.** Red on old source and refusal on the repair reproduced independently for F1/F2/F3 and my variants.
5. **Cost: PASS as disclosed.** No timeout (O-3, LOW).
6. **No scope widening:** confirmed.

**Required to reach APPROVED (source only).** Close N-1: verify object hashes for HEAD's commit and every tree used to build the expected blob list, or otherwise refuse when tree content cannot be proven canonical. Then add the N-1 fixture (loose canonical subtree overwritten with a drift tree) as a red→refuse test. Optionally also: `core.commitGraph=false` and `ls-tree <pinned sha>` (O-1, O-2). A short focused re-review of that delta is sufficient; the F1/F2/F3 work above need not be repeated.

## 5. Preservation and scope compliance

- Deployed `unity-lock`/`_ccf.py`: sha256 read-only only. No ZX20 writes. Registered lane, its HEAD/index/eight dirty files and Library: not touched. The proposed function was never run against it.
- The only real-store access was a read-only object-existence count in the shared common store (§2 N-1).
- No real lease or lock, no Unity, no cleanup of project storage. I deleted only my own disposable perf repo.
- No package/material edits, PR, merge, integration or build.
- All fake fixture leases were released; no `lock.json` remains anywhere under the evidence root.
- The author lane was read-only (not even read: the source came from `git archive` of the exact SHA in the canonical store).
- The review branch commit changes only this file.

**HANDOFF: NEEDS CHANGES — SOURCE ONLY.** STOP for Manager #200; the Manager decides any repair dispatch, source acceptance and deployment.
