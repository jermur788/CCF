# CCF #338 — Exact-dirty-byte Unity lease admission: SOURCE PROPOSAL (not deployed)

**Date:** 8 November 2026
**Status:** SOURCE PROPOSAL, tested offline on disposable fixtures. **Not deployed, not reviewed, not approved.** No lease, Unity, registered-lane or deployed-CCF-Ops change was made.
**Task:** [#338](https://github.com/jermur788/CCF/issues/338), PACKET ONLY, bounded safety-verification tooling repair. Binding packet: `MANAGER-337-SAFE-LEASE-ADMISSION-SOURCE-AND-REVIEW-PACKET-20261108.md`.

## 1. Identity

| Field | Value |
|---|---|
| Worker | Anthropic **Claude Sonnet 5.5** (`claude-sonnet-5-5`), Claude Code CLI under T3 Code. One author, no subagents, no self-review. |
| Reasoning effort | Harness-reported numeric setting **`10`** (scale unspecified). **Not confirmed equal to the packet's preferred "High".** The Manager and reviewer should weigh that. |
| Branch / worktree | `fix/unity-lease-exact-dirty-guard-20261108-v1` / `/media/jer/Files/CCF/Operations/CCF-UnityLeaseExactGuard-20261108` |
| Base (starting HEAD) | `7b9aedd14c04dc28cad061bdef24b2d590b21c61` (= freshly fetched `origin/main`) |
| Context commit | `4790c20918ca2a667b04d80262142d9b4151b60e` (latest change to the canonical context/instruction set; the packet named none) |
| **Tested source commit** | **`949cc0c19bed09ba3fad4c319e235575ac75f6a8`** (the `Tools/Verification/UnityLeaseExactAdmission/` files only) |
| Publication (report-only) commit | reported in the #338 HANDED-OFF comment and the final handoff message (a file cannot contain its own commit SHA) |
| Evidence | `/media/jer/Files/CCF/Verification/CCF-UnityLeaseGuardSource-20261108/` (`SHA256SUMS.txt`) |
| RUNNING receipt | [#338 comment 6720131119](https://github.com/jermur788/CCF/issues/338#issuecomment-6720131119), posted after live preflight and before any source edit |

**Launch-setup correction (disclosed).** The first orchestrator launch used the default home worktree; that author session stopped with no edits and no RUNNING post. The parent then moved only that newly created clean task worktree to the prescribed Files path and repaired its registration. Branch and base were unchanged. It was a launch-setup correction, not a pre-existing conflicting owner. At my start I verified: status clean, HEAD == fresh `origin/main`, no other worktree on this branch, #338 with no assignee and only the Manager READY comment, live `active-tasks.md` byte-identical to the local copy.

## 2. Result

A smallest-change patch to the deployed `unity-lock` is proposed. It adds one **opt-in, acquire-only** `--exact-admission` mode. That mode is hard-wired in code to one lane, one branch, two pinned Git states and the exact eight protected byte hashes. The default path is unchanged: with no `--exact-admission-*` option the single original clean-lane `if` runs exactly as before. The patch removes **one** line and adds the rest; everything after the lock-write (release, status, force-release, `__main__` wrapper), the flock guard and the stale/PID helpers are byte-identical to the deployed source (tested).

- **Pre-transition state: implemented and tested.** Original branch/HEAD `4d95e452…384`, exactly the eight dirty files.
- **Post-transition state: implemented and tested with a fixture-frozen candidate, but UNSUPPORTED in production.** Its candidate identity has not been frozen by the Manager, so the production profile carries `head: None` and every request for that state is refused (tested, and re-confirmed read-only against the real lane). I make **no claim** that any post-transition state passes.
- **Production dry run (read-only, no lease):** the production profile's decision function, run against the real registered lane, returned an ADMIT decision for pre-transition and REFUSED post-transition as unsupported. Real-lane index, eight working files, HEAD and lock directory were byte-identical before and after (`preflight/real-lane-{before,after}-dryrun.txt`, `readonly-production-dryrun.json`). This was a function call, not an acquire; no lock or state file was written.

## 3. Deployed source inspected read-only

| File | SHA256 | mode | mtime |
|---|---|---|---|
| `/media/jer/ZX20/CCF-Ops/scripts/unity-lock` | `54cf0a95561d0a8c350d89adbee83f393990b728d6127b94f69c587260f048e5` | 755 jer:jer, 10079 B | 2026-10-15 23:06:39 +0100 |
| `…/scripts/_ccf.py` (imported helpers) | `ab4343efc7d1b82c5a4aca747d0e09c614779cfd9aa406a0dd681d204f5de001` | 755, 11825 B | 2026-11-05 17:16:51 +0000 |
| `…/lane-status`, `…/packet-lint` (not imported; recorded) | `e7102a83…7173f`, `a776ec42…43e43` | 755 | 11-05 / 10-15 |
| `…/CCF-Ops/lanes.json` | `8f1aade7a140c8abd55e925838fbaa51cf984bcd8a94d9e6861f199dc9655b0b` | | |

Re-checked at the end (`deployed-and-lane-final-check.txt`): all hashes, modes and mtimes identical; ZX20 free space byte-identical (23,216,128,000) so nothing was written there.

**What the deployed acquire does** (relevant semantics, all preserved): `flock(LOCK_EX)` on a permanent `guard` inode around every metadata operation; refuse if `lock.json` exists (held, stale or malformed all require explicit release); storage readiness; configured, Unity-capable lane; real top-level and common Git dir match the configured store; branch/HEAD equal the supplied values; **`git status --porcelain` must be empty** (the only clean-lane gate, the line this proposal conditions); lane registered in a configured store; no other writable checkout of the branch; any running Unity process must be exactly the claimed PID; lock written by temp-file + `os.replace` under the guard. `release` needs owner + task + lock-id; force needs Manager reference. `status`/`release` never consult cleanliness. `lane-status` reports a dirty lane as not READY; it is separate and unchanged.

## 4. The mechanism

### 4.1 Activation (all five options together, `acquire` only, otherwise a usage error)

```
unity-lock acquire --owner OWNER --task-id TASK \
  --lane WorkbenchTier2Reg287 --branch feature/scenario-one-workbench-retirement-20261105 \
  --head 4d95e45245d660e24b91e26754d9c57949bf3384 --eta FUTURE_ISO_WITH_TZ --notes '…' \
  --exact-admission WorkbenchTier2Reg287-exact-dirty-v1 \
  --exact-admission-state pre-transition \
  --exact-admission-digest 90da4135e52e895d0e728229786d6c9f1fe52ce83591a9ae9bba40a94c42ce90 \
  --exact-admission-reason 'one-off diagnostic: …' \
  --exact-admission-receipt 'URL/ref of the Manager approval of THIS lease'
```

The digest is SHA256 over profile id, state, lane name/path, branch, HEAD and the sorted per-path `(working sha, historical-HEAD blob sha, size)` table. The operator must restate it, so the activation *names* the exact approved bytes; a stale or wrong value is refused. The receipt is an auditable cooperative declaration (like `--manager-approval`), not cryptographic proof. Recompute the digest with `python3 -I -B test_exact_admission.py SourceStructure.test_production_digest_pin`.

### 4.2 Why constants in code, not a config/allowlist

Paths, hashes, HEADs, lane, branch, evidence location and the expiry are Python constants in `EXACT_ADMISSION_PROFILES`/`PROTECTED_DIRTY`. A caller can only select an entry by name. No wildcard, no caller-supplied path or hash, no generic `--allow-dirty`, no environment hook. The cost is that changing the policy means redeploying a reviewed file, which is the intent.

### 4.3 What must hold, every time (any failure = refusal, exit 2, lock untouched)

1. Profile known; state known; **state frozen** (post-transition needs a pinned HEAD, pinned package-JSON blob hashes/sizes and a pinned changed-path list); profile not past `valid_until`; digest matches.
2. Configured lane name == `WorkbenchTier2Reg287`; lane path == `/media/jer/ZX20/CCF-WorkbenchTier2-Registered-20261106`; common store == `/media/jer/Files/CCF/Repository.git`; branch and HEAD equal the pinned state (in addition to the existing `--branch/--head` match).
3. **Evidence:** the #318 directory `/media/jer/Files/CCF/Verification/Workbench287MorningClosure-20261107` must be a canonical (non-symlink) directory; `SHA256SUMS.txt` must hash to the pinned `9fb80596…1f74`, parse strictly, list **exactly** the 16 expected files, and each file (opened `O_NOFOLLOW`, regular only) must hash to its listed value. The expected 16 are derived from the pinned table, so code constants and preserved evidence must agree.
4. **Git (admission-only calls, `GIT_*` env stripped, no user/system config, `GIT_OPTIONAL_LOCKS=0`, no index refresh):** `status --porcelain=v1 -z --untracked-files=normal --ignored=traditional --no-renames` may show only ` M` for exactly the pinned dirty set and `!!` only for the pinned ignored list (`Assembly-CSharp-Editor.csproj`, `Assembly-CSharp.csproj`, `CCF-WorkbenchTier2-Registered-20261106.slnx`, `Library/`, `Logs/`, `UserSettings/`). Any `??`, staged, deleted, typechange, unmerged, other ignored entry, non-UTF-8 path or Git error is refused. Then: `diff-index --cached` empty, `diff --name-only` equals the status set, and **`ls-files -v` must show only `H`** (assume-unchanged / skip-worktree flags would let Git status lie).
5. **Bytes, per pinned path:** canonical path (no symlinked component), regular file, SHA256, size, `0o100755` mode all equal the pinned values, and the blob at HEAD hashes to the pinned historical value. exFAT stat/mtime/inode differences are **not** drift when the bytes match (tested); a changed byte, size or mode is never waived.
6. Post-transition additionally: HEAD descends from `46bc1fd6…c7c` (`merge-base --is-ancestor`), changed paths since the old HEAD ⊆ the pinned list, and the two package JSONs in the working tree equal their pinned *tracked* candidate bytes (so they are clean and only the five Materials + `.vscode/settings.json` remain modified).
7. All pre-existing guards still run afterwards, unchanged: lock-exists/stale/malformed, registration, branch-conflict, Editor-PID, atomic lock write under the flock.

### 4.4 Audit, one-time nature, rollback

- On success the lock record gains `exact_admission` (profile, state, digest, lane, branch, head, owner, task, reason, receipt, evidence-manifest hash, per-path before sha/size/mode, timestamp), and `history.jsonl` gets an `exact-admission` event **before** the lock is written; normal release later appends `release` carrying the same record. The deployed `status` and `release` read and release such a record unchanged (tested).
- It is per-invocation: every acquire revalidates everything; the exclusive lock means one lease at a time; after release a new request starts from scratch (tested). It is not persistent policy: the profile carries a proposed hard expiry `valid_until = 2026-11-11T00:00:00+00:00` (**Manager-tunable, my proposal**).
- **Rollback / deactivation:** restore the original file (`54cf0a95…048e5`) from the retained backup, or let `valid_until` pass. Nothing else changes state.
- **R1 is untouched:** this admits a lease only. It does not make the Workbench R1 post-run provenance pass; R1 still judges drift against its own HEAD and a dirty diagnostic run remains an R1 FAIL.

## 5. Files (tested commit `949cc0c1`)

All under `Tools/Verification/UnityLeaseExactAdmission/`:

| File | SHA256 |
|---|---|
| `unity-lock.deployed-54cf0a95.txt` (verbatim deployed baseline) | `54cf0a95561d0a8c350d89adbee83f393990b728d6127b94f69c587260f048e5` |
| `_ccf.deployed-ab4343ef.txt` (verbatim helper baseline, makes tests hermetic) | `ab4343efc7d1b82c5a4aca747d0e09c614779cfd9aa406a0dd681d204f5de001` |
| `unity-lock.proposed` (full proposed file) | `38018c6f34449b97866512de2d322294ed96fa299a6f3dc14191ef50e23d835c` |
| `unity-lock-exact-admission.patch` (`patch -p1` against the deployed file) | `deb541e75e3731248a66c1554935c2e75bb3ff76c1a63dd3ba78d678b299d361` |
| `test_exact_admission.py` | `7b9122f370b723638e0a67d8f5d28e97f21291303e307b8daeb6417cdcc95afd` |

Plus this Markdown. Never treat `unity-lock.proposed` as the deployed script.

## 6. Verification actually performed

Command: `CCF_LEASE_TEST_TMP=<scratch dir on the Files disk> python3 -I -B test_exact_admission.py -v` (Python 3.12.3, Git 2.43.0). **77 tests, all OK**, run twice: on the working tree and on a `git archive` of tested commit `949cc0c1` (`test-run-from-tested-commit-949cc0c1.log`).

| Packet row | Coverage |
|---|---|
| (a) default clean-pass / dirty-deny | Deployed and proposed modules run on identical fixtures with no opt-in: identical result for clean (lock has no new field), dirty, staged, modified, untracked; exact options on a clean lane are refused rather than silently passing. |
| (b) exact states | Eight-file pre-state accepted + audited, lane bytes/index unchanged; fixture-frozen post-state with six dirty accepted; production post-state unsupported; unfrozen head / package pin / path list each refuse; production profile refused on any other lane. |
| (c) one extra / changed | extra modified, untracked file/dir, staged, protected file staged/deleted/reverted/size-changed/**one byte flipped at same size**, unlisted ignored path, `.git/info/exclude` hiding a file, mode change, symlink-to-identical-bytes, symlinked directory. |
| (d) identity / evidence | wrong branch/HEAD/lane name/path/common store (profile and config), missing / trailing-newline-edited / consistently re-forged manifest+backup / edited backup / edited HEAD copy / missing copy / extra manifest entry / symlinked manifest or root, expired, wrong digest, unknown profile/state, partial options, options on `status`/`release`. |
| (e) lease semantics | held, stale (absent PID) and malformed lock blocks, lock untouched; unclaimed Editor; second writable checkout of the branch; lane unregistered; **two forked concurrent acquires over 4 rounds always give exactly one lock + one `exact-admission` event**; revalidation after release. |
| (f) errors never accept | corrupt index; assume-unchanged and skip-worktree flags hiding a real change (precondition asserts Git status itself is fooled); `git` missing from PATH; simulated `git ls-files` and non-zero `status` exit; `GIT_DIR/GIT_WORK_TREE/GIT_INDEX_FILE` not inherited by admission; non-UTF-8 path. |
| Structure | baseline/helper hashes pinned (and equal the live deployed files, read-only); patch reproduces the proposed file byte-for-byte and removes exactly the one original line; text after the lock write, `__main__` wrapper, flock/stale helpers identical to deployed; production constants equal the #337 `protected-before.json` and the real #318 manifest verifies 16/16. |

**Mutation check** (`mutation_check.py` / `mutation-check.log`): 27 deliberate weakenings of the proposed script; **24 caught by behavioural tests**. Three survived, all *redundant defence-in-depth* that other layers already cover: the explicit staged-index check (status X column also refuses), the canonical-path/symlinked-component check (status refuses first), and the HEAD-blob hash re-check (HEAD pin plus evidence cross-check already fix it). They stay in the proposal as belt-and-braces; I could not construct a fixture that isolates them.

## 7. Limitations and what was NOT verified

- **Not deployed and not exercised against the real production lock, flock or ZX20 config.** The test harness stubs exactly: `config_read` / `require_storage` / `on_storage` / `ROOT` (ZX20 mount-identity and free-space checks) and `editors()` (Unity process discovery). Those production paths are therefore untested here; they are byte-identical to the deployed source.
- Fixtures live on the Files disk (ext4), not exFAT. exFAT-specific behaviour is evidenced only by the read-only dry run of the exact `git status … --ignored=traditional` invocation on the real lane, whose output was exactly the eight ` M` entries plus the six pinned ignored entries and a clean `ls-files -v`.
- The deployed `tests/self-test.py` (writes under ZX20/CCF-Temp) was **not** run and is **not** extended. A later authorised deployment should run it, and a dedicated adoption packet should decide whether to carry these tests there.
- The guard is a point-in-time check inside the flock, as the original is. It does not police drift between acquire and Editor launch or during the run. That remains the job of the R1 provenance and the lock's cooperative ownership model.
- The lock is cooperative, not a security boundary; the receipt is a declaration, not proof.
- Strictness that may cause **false refusals** (deliberate, fail-closed): any ignored path outside the six pinned (for example `Temp/`, `obj/`, a stray `.sln`) refuses; a changed file mode refuses; `GIT_CONFIG_GLOBAL=/dev/null` means a safe.directory problem would refuse rather than accept.
- No Unity, no lease, no deployed-file edit, no registered-lane write, no package/material/save change, no integration, PR, build or cleanup. No assumption was made that the post-transition candidate exists.
- **Disk:** Files free was 20.48 GB (19.08 GiB) when I started and 20,483,293,184 B at the end, below the 20 GiB figure. I treated the 20/27 GiB rules as ZX20 Unity-run rules (the packet cites them with the Unity lane); this work wrote under 2 MB to the Files disk and nothing to ZX20. Flagged in case the Manager intended them for Files as well.

## 8. Decisions for Manager #200

1. **Freeze the post-transition candidate**, or leave it unsupported: exact head, the two tracked package-JSON blob hashes/sizes, and the exact changed-path list vs `4d95e452…`. Then the profile, digest and tests must be re-run and re-reviewed. The ancestor `46bc1fd6…c7c` is recorded only as a required ancestor.
2. Confirm or change the proposed `valid_until` (2026-11-11 UTC).
3. Confirm the strict ignored-path list and the exact-mode pin are acceptable, or widen them narrowly in the reviewed source.
4. Decide whether `lane-status --preflight` should report this lane differently (unchanged here; it will still say dirty).
5. Choose the deployment procedure: copy the retained original, apply the patch, verify the result hash equals `38018c6f…835c`, run the deployed self-test, then use the lease. Only after #339 independent review and a separate Manager authorisation.

## 9. Evidence manifest and release

Durable evidence: `/media/jer/Files/CCF/Verification/CCF-UnityLeaseGuardSource-20261108/` — `preflight/` (deployed copies and provenance, live coordination, packet, lanes, real-lane before/after), `readonly-production-dryrun.{py,json}`, `test-run-final.log`, `test-run-from-tested-commit-949cc0c1.log`, `mutation_check.py`, `mutation-check.log`, `tested-source-sha256.txt`, `deployed-and-lane-final-check.txt`, `SHA256SUMS.txt`. The disposable `tmp/` fixture directory was emptied.

Unrelated work preserved: none touched. `active-tasks.md`, Overall-Dev, the deployed CCF-Ops tree and the registered lane were not edited.

DASHBOARD SYNC: not available to this worker / NOT VERIFIED; Manager #200 owns active-tasks and dashboard ingestion.

**HANDED-OFF as a source proposal. Not accepted, not deployed, not independently reviewed. The Files author lane is released: no further writes by this worker.**
