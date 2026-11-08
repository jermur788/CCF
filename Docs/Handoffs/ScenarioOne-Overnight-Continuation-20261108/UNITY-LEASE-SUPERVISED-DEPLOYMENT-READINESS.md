# #345 — Supervised deployment readiness: exact-dirty Unity lease guard

**Readiness verdict: CONDITIONAL GO for a one-window, operator-attended deployment. NO-GO for permanent/global adoption.** Nothing was deployed, acquired, run or changed.
**Author:** Anthropic Claude Sonnet 5.5, effort numeric UNKNOWN. Read-only. Lane `/media/jer/Files/CCF/Operations/CCF-UnityLeaseSupervisedReadiness-20261108`, branch `docs/unity-lease-supervised-readiness-20261108-v1`, start HEAD `37657fca02f273d23466756232e02bcdb99d7d37`.
**Reviewed source (unchanged):** tested `11cee0c26c81b916c77a5310370d7ce42f2a203b`; reports `24e0de290bfdfa9d5a584e1c9e6d87bd8c18bb1c` (author), `9e9ae1dfffaedf7213740567ba170f190e1213b7` (#344, same provider). Evidence: `/media/jer/Files/CCF/Verification/CCF-UnityLeaseSupervisedReadiness-20261108/` (`observations.txt`, `SHA256SUMS.txt`).

## 1. What was inspected
| Item | Result |
|---|---|
| Deployed `unity-lock` / `_ccf.py` hashes | **PASS** — equal the pinned baseline (`54cf0a95…`) and helper (`ab4343ef…`) copies |
| Live lease | **PASS** — no `lock.json`; `guard` present; last event release 2026-11-07 01:49Z (#287); no Unity or `unity-lock` process |
| Warm Workbench HEAD/branch | **PASS** — `4d95e452…` on `feature/scenario-one-workbench-retirement-20261105` = pre-transition pin; registered in `lanes.json` (`WorkbenchTier2Reg287`, `unity:true`), store `/media/jer/Files/CCF/Repository.git` |
| 8 protected modified files | **PASS** — sha256 + size equal the table in the source, mode 0100755 each (read by plain file read; no Git status) |
| #318 evidence manifest | **PASS** — `9fb80596…` equals the profile pin |
| Library | PASS (exists, 6.6 G); contents not scanned |
| Untracked/ignored inventory, index flags, package diff | **UNKNOWN** — needs `git status`/index, which the packet forbids on the protected lane; the guard checks them at admission and will refuse if wrong |
| ZX20 free space | **22 G**: above the 20-GiB STOP, **below the 27-GiB cold-entry gate**, above the 15-GiB floor. Gate that applies to a warm entry is UNKNOWN; Unity entry itself is out of scope here |
| Files free space | 13 G (98 % used), root 7.1 G — tight but sufficient for this report/evidence |
| System I/O pressure | **WARNING** — `some avg10=60%` at inspection; machine was busy (other agents). Not a safe moment to measure or run the scan |
| Real exFAT/ZX20 scan time | **UNKNOWN** — never measured; no live scan was run |
| Source/test status | accepted by Manager (#343/#344); not re-tested here, as instructed |

## 2. Is supervised use practical? (O-3 liveness)
Facts from code: `guarded()` takes `flock(LOCK_EX)` on `state/unity/guard` with **no timeout** and holds it for the whole of `acquire`, including `admit_exact_dirty` (status, ls-tree walk, then reading 5,732 tracked files / 1.138 GiB). `status` and `release` take the same flock, so during the scan they wait. `lock.json` is written only **after** admission, and the kernel drops a flock when its process exits.

Consequences:
- A slow scan is a bounded, attended pause (recorded: warm ≈2 s, cache-evicted ext4 22–36 s; exFAT-on-USB unknown). Fine for an operator who is watching.
- A *stalled* USB read could leave the acquire process uninterruptible and block `status`/`release` for everyone until the device answers. Killing the acquire process (once it can die) is safe for state — no `lock.json` exists yet and the flock is released — but **do not kill anyone else's process, remove `guard`, or hand-edit/force-release `lock.json`** (`release --force` needs Manager approval).
- The pre-existing `git status --porcelain --ignored` over the whole lane (including the 6.6 G Library) also runs inside the lock; its cost is UNKNOWN and not new (the baseline guard ran `git status` there too).

**Acceptable under this explicit operator policy** (no code change, so no new source review): one human present for the whole call; no other agent expected to use the lease in that window; I/O pressure calm first; cache pre-warmed *outside* the lock (below); a wall-clock stop rule. If any condition cannot be met → NO-GO for that attempt. Permanent adoption without a real timeout is NO-GO; the one minimum mitigation if the Manager wants unattended use is a separate, reviewed source change adding `LOCK_NB` retry with a deadline plus a read-progress deadline in the raw proof — not requested now.

## 3. Open limitations and my recommendation on each
1. **Expiry (needs Manager decision).** The source constant `valid_until = 2026-11-11T00:00:00+00:00` is an unapproved author placeholder. Recommend the Manager freeze **deployment time + 6 h** for one witness (extend only by a new decision), set by a single-line edit of that constant on the staged copy, recorded with its new hash. The expiry is not part of the admission digest, so the operator's restated digest (`90da4135e52e895d0e728229786d6c9f1fe52ce83591a9ae9bba40a94c42ce90`, pre-transition) is unaffected. Because this makes the deployed bytes differ from the reviewed bytes by exactly that one line, the staging step verifies "diff vs reviewed source is exactly that line".
2. **Post-transition: UNSUPPORTED** (`head=None`, no frozen package blobs/path list). Any request for it is refused by code. Do not move the warm lane's HEAD/index/dirty files to make the lock work; a transition needs its own witness and Manager packet.
3. **Default strict-clean path** still relies on `git status` (not hardened). Deploying for a window changes this for every lane while installed; for the trusted single operator this window it is the same risk as today. This is the main reason to reject permanent global adoption without review.
4. **R-1** (HEAD re-resolved at admission): operator keeps sole ownership of the lane; no other writer for the window.
5. **Independence:** #344 was same-provider; cross-provider attempt was refused by the provider filter and not retried.
6. Option B 1.1.1 B3 / Workbench C–G / integrated A–O (0/15) / ZX20 27-GiB cold-entry gate remain separate and unaffected.

## 4. Unexecuted procedure (every step needs operator consent; stop at any FAIL)
**Gate 0 — Manager authorises a supervised deployment packet naming: expiry (e.g. +6 h), operator, window, and rollback owner.** Without it, stop.

**A. Pre-checks (read-only)**
1. `sha256sum /media/jer/ZX20/CCF-Ops/scripts/{unity-lock,_ccf.py}` must equal `54cf0a95…` / `ab4343ef…`; `unity-lock status --json` must show no lease; no Unity process.
2. Workbench: `GIT_OPTIONAL_LOCKS=0 git -C <lane> rev-parse HEAD` = `4d95e452…`; re-hash the 8 files against the source table (as done here).
3. `cat /proc/pressure/io` calm (`some avg10` < 10); ZX20 free ≥ gate the Manager names; Files free ≥ 5 G.
4. Isolated fixture sanity (never the real lane): from a Files scratch dir, `CCF_LEASE_TEST_TMP=<scratch> python3 -I -B test_tree_object_integrity.py` on the extracted tested source — 13 tests, one run.

**B. Stage (same filesystem as the target, nothing live yet)**
5. Back up: `cp -p` deployed `unity-lock` to `/media/jer/Files/CCF/Verification/<deploy-dir>/unity-lock.baseline-54cf0a95` and a second copy `scripts/unity-lock.rollback-54cf0a95` on ZX20; check both sha256 = `54cf0a95…`.
6. `git archive 11cee0c2` `unity-lock.proposed` → `scripts/.unity-lock.staged`; check blob/sha256 equals the reviewed file; apply the Manager's one-line `valid_until` edit; record `diff` = exactly that line and the final sha256 in the evidence.
7. `chmod 755`, `python3 -I -B -m py_compile` into a scratch path (not beside the script; no `__pycache__` written there).

**C. Install (operator present, window open)**
8. `mv -f scripts/.unity-lock.staged scripts/unity-lock` (rename over the old file on the same fs). exFAT has no journal, so this is replace-atomic in the kernel but not power-loss-safe; the ZX20 rollback copy covers that.
9. Verify `sha256sum scripts/unity-lock` = staged hash and `scripts/unity-lock status --json` returns normally.

**D. Supervised use** (a separate Manager-authorised Unity packet; not part of this readiness report)
10. Pre-warm outside the lock: `cat` the 5,732 tracked files to `/dev/null` (read-only), then immediately run `timeout --signal=INT 180 scripts/unity-lock acquire … --exact-admission <profile> --exact-admission-state pre-transition --exact-admission-digest 90da4135… --exact-admission-reason … --exact-admission-receipt …`. In a second terminal only `timeout 20 scripts/unity-lock status --json`.
11. Success = exit 0 and a `lock.json` with `exact_admission.raw_proof` (`verified_trees` ≈ 567, `hashed_files` ≈ 5,724, `pinned_skipped` 8). Failure/timeout = no `lock.json`; do not retry in a loop; record and stop.
12. After the Unity work: normal `release --owner … --lock-id …`; confirm no lease, no Editor.

**E. Rollback (any failure, or end of window/expiry)**
13. Only when `status` shows no lease and no Editor: `cp -p scripts/unity-lock.rollback-54cf0a95 scripts/.unity-lock.restore && mv -f scripts/.unity-lock.restore scripts/unity-lock`; check sha256 = `54cf0a95…`; `status` works.
14. Re-hash the 8 protected files, confirm Workbench HEAD unchanged; keep the Files evidence copy; delete the ZX20 staged/rollback copies only after Manager closes the packet.
15. If a lease exists or the process is stuck: do not restore over a script mid-call, do not kill foreign processes, do not delete `guard`/`lock.json`; escalate to Manager (`release --force` needs a Manager reference).

## 5. Next operator action
Manager #200 decides one of: (a) authorise the supervised deployment packet above with a frozen expiry (recommended only if a Unity witness is actually needed now and the machine is quiet); (b) decline deployment and keep the baseline guard; (c) commission the one liveness change (deadline on the flock wait and the raw read) as a separate source + review cycle. No other downstream work follows automatically.
