# Sen-1 #802 — DeepSeek one-orchestrator: Unity-lock containment and safe retirement of obsolete CCF copies

**Parent:** [#802](https://github.com/jermur788/CCF/issues/802) · **Manager:** [#731](https://github.com/jermur788/CCF/issues/731) · **Ops lineage:** [#652](https://github.com/jermur788/CCF/issues/652)
**Binding packet:** `Sen-1-802-ONE-ORCHESTRATOR-UNITY-LOCK-CONTAINMENT-AND-51-COPY-SAFE-RETIREMENT-PACKET-20261119.md`
**Actor:** opencode-go/DeepSeek V4.1 Flash, native T3 Code / OpenCode, host `jer-laptop` (Linux), parent session `d6edf2f1-47cc-4141-a587-abc139628ef1`
**Date:** 19 November 2026

---

## 0. Outcome

**PARTIAL — G0 COMPLETE, G1 NOT ADMITTED, G2 HELD, CLEANUP EXECUTED WITH INTERNAL REVIEW.**

| Measure | Actual |
|---|---|
| Whole Git worktrees removed | **7** (221 → 214 registered) |
| Whole non-Git directories removed | **18** |
| Ops copies eliminated | **10** of 51 |
| Disk reclaimed (kernel-measured) | **7.44 GB** (Files +4.95 GB, Videos +2.49 GB) |
| Retained / unknown | 41 copies; 1 worktree held (#792) |
| Retained non-deployed live-capable | **38** |
| Unity live lock | **UNCHANGED / FREE** |

**Containment is NOT complete and is not claimed.** G1 source authority was never granted, no guard was written, and 38 non-deployed copies still resolve to the live `state/unity` by default. The cleanup reduced reachable surface; it did not eliminate it.

---

## 1. Gate status ladder

| Gate | Result | Basis |
|---|---|---|
| **G0 — host admission / inventory** | **INVENTORIED** | §2; `inventory-before.jsonl`, `live-state-witness-before.txt` |
| **G1 — source admitted** | **NOT ADMITTED** | Board row `CCF-652-OPS-READINESS` = `RESERVED ONLY / NOT RUNNING / SOURCE BLOCKED`; no actor-specific `APPROVED TO START SOURCE`; no physical lease |
| **CONTAINMENT IMPLEMENTED** | **NO** | Correctly blocked at G1; no `CCF-Ops/` file edited |
| **INTERNAL QA VERDICT** | **APPROVED WITH CHANGES — corrections applied** | §5, Claude Opus 5.5 nonauthor child |
| **Deletion** | **EXECUTED** | §6, 25 receipt rows, zero failures |
| **G2 — installed** | **HELD** | No installer reservation, no Manager deploy permit, R-1/D-1 open |
| **Unity live lock** | **UNCHANGED / FREE** | `lock.json` absent; `history.jsonl` `dfb50644…` before **and** after |

---

## 2. G0 — actual read-only host admission

### 2.1 Storage and mount identity (verified, not assumed)

| Target | Source | fstype | UUID | Free at admission |
|---|---|---|---|---|
| `/media/jer/ZX20` | `/dev/sdc2` | exfat | `18D6-896E` | 366.6 GB |
| `/media/jer/Files` | `/dev/sdb4` | ext4 | `2283b135-bf37-42d6-921f-2fb861d0a8d4` | 31.4 GB |
| `/media/jer/Videos` | `/dev/sdb1` | ext4 | `60e276b9-4abb-4325-a913-f4c43c14294b` | 438 GB |

By-id link confirmed: `ata-NetacPortableSSD_AA20260702512G231151-part2 -> ../../sdc2`. Device letters match the #794 snapshot. Files and Videos remain two partitions of **one** physical device — not disaster-recovery separation. Files stayed above the 10 GiB floor throughout.

### 2.2 Live Unity state (read-only; no `unity-lock` command was ever run)

```
state/unity/lock.json     ABSENT          (no occupied lock, before and after)
state/unity/guard         inode 240749  size 0
state/unity/history.jsonl inode 240750  size 299478
                          sha256 dfb506443e1f41b6d5f2be23e2c2b38d0ee76fe78d4d4ba2446fbc962422fffa
```

That hash is **byte-identical to the #794 witness** before the work and **still identical after** it. No Unity Editor process and **zero** open file descriptors on `state/unity` at admission and at deletion time.

### 2.3 Deployed Ops digests (before = after)

| File | sha256 |
|---|---|
| `scripts/unity-lock` | `54cf0a95561d0a8c350d89adbee83f393990b728d6127b94f69c587260f048e5` |
| `scripts/_ccf.py` | `ab4343efc7d1b82c5a4aca747d0e09c614779cfd9aa406a0dd681d204f5de001` |
| `scripts/lane-status` | `e7102a83ad942cc7365cb22db9a8f79823744ea98bc2cf8a6738a11f1897173f` |
| `lanes.json` | `aa3d465832cc3f37e9a6f971480055a3b008d63057adf1b8de2a8bbaf51c2c97` |
| `deployment.json` | `8c66b290c247708846cb8b59f83626a10a3fa1d052397a3316dc5f23fc157b18` |
| `board/active.md` | `4d76f9a3df8f7f92f3cf3b8f7c6e3468b85ddb4e493792cca6f159c2955b6b2e` |

`lanes.json`, `deployment.json`, `unity-lock`, `_ccf.py`, `lane-status` all still match the #794 witness. **`board/active.md` changed** (`0baf348d…` → `4d76f9a3…`), consistent with the 09:20–09:30 UTC owner/manager scope comments; it was **not** written by this orchestrator and its hash was unchanged by the deletion work.

### 2.4 Inventory

All **51** copies from the #794 input were present at admission. Input `live-capable-copies.txt` sha256 `e466d275913a1254aa7d56ed5374e326ed65f86324f3baf5c0feb02ccd1f118b`, unchanged since #794.

Four distinct `unity-lock` byte-identities across the 51:

| Bytes | Copies | Role |
|---|---|---|
| `54cf0a95…` | 38 | deployed image |
| `8fa0c5ad…` | 7 | #652 source tree |
| `e4653034…` | 5 | older variant |
| `da055bcc…` | 1 | older variant |

47 admitted live-capable, 4 rejected by the deployed mount-identity check; 45 resolve by default to live `state/unity` (**1 deployed + 44 non-deployed**).

### 2.5 The G1 blocker (custody and permissions)

The authoritative physical board row reads:

> `CCF-652-OPS-READINESS … RESERVED ONLY / NOT RUNNING / SOURCE BLOCKED … Manager #731 … issues a source-admission verdict naming session 9bcdfde6; the author then refreshes physical state and personally re-registers RUNNING on #652 … before any source edit.`

The [#652 corrected G1 text](https://github.com/jermur788/CCF/issues/652#issuecomment-7004086131) records that a real physical reservation/lease and a custody statement are **still outstanding on the owner/Manager side**.

**Conclusion: no lawful source writer exists.** No Sol/Codex source child was launched, no `CCF-Ops/` file was edited, and no permit was invented. G1 is reported as blocked, not worked around.

---

## 3. Why containment is not claimed

The packet states that updating an installed script does not make old copies safe, and that *"a guard in the newest source alone is INCOMPLETE if any retained 2016/2026 historical copy can still operate live."* With G1 blocked there is no guard at all. The only lawful mitigation available was removal of provably unused copies — which reduces reachable surface but cannot make retained copies fail closed.

This result therefore keeps the required distinction between *"44 historically risky"* and the empirically safe set: **38 non-deployed copies remain live-capable.**

---

## 4. Classification

| Class | Meaning | Count |
|---|---|---|
| `KEEP_DEPLOYED` | installed `/media/jer/ZX20/CCF-Ops` + `state/unity` | 1 copy + state |
| `KEEP_ACTIVE_OWNER` | #652 source checkout, `CCF-OpsLeaseFix-20261115`, `/media/jer/Files/CCF-Primary` (+7 dirty/untracked items) | 3 copies |
| `KEEP_EVIDENCE` | `Sen1-741-ExFATQual*` (11), `Sen1-752-NativeV6`, `Sen1-754-*` (3), `foundation-test-66okebci`, `foundation-test-_f2pd8_q` | 36 copies |
| `DELETE_PROVEN_UNUSED` | 7 registered worktrees + 18 scratch directories | 10 copies |
| `UNKNOWN_RETAIN` | `CCF-792-Admission` worktree; ~200 older registered CCF worktrees | not touched |

The four `#781` worktrees were individually re-evaluated under the owner's 09:20 amendment rather than the older blanket-retention rule.

---

## 5. Internal nonauthor review — and what it changed

**Reviewer:** Claude Opus 5.5, provider `claudeAgent`, distinct from the parent's `opencode-go/deepseek-v4.1-flash`; a child thread of this same parent. Read-only: no `unity-lock`/`lane-status` command, no `environment.sh`, no writes.

**Verdict on the 28-row manifest: `NEEDS CHANGES`.** 22 rows approved; 6 held.

| Held row | Reason | Final disposition |
|---|---|---|
| **D05** `CCF-792-Admission` | #792 is **OPEN** ("G1 SOURCE HELD"); manifest wrongly said custody was closed | **RETAINED** — needs documented custodian release |
| **D13** `foundation-test-66okebci` | Explicitly kept as evidence by the #652 Sonnet-5.5 reviewer; #652 is open | **RETAINED** (KEEP_EVIDENCE) |
| **D18** `foundation-test-_f2pd8_q` | Same | **RETAINED** (KEEP_EVIDENCE) |
| **D09** `CCF-794-Scratch` | Only 14 of 344 files had archived twins; raw QA/QC repro material existed only on Videos | **ARCHIVED then REMOVED** (439 files, 0 mismatches) |
| **D16** `foundation-test-a8o93028` | Unique unpreserved `result.json`, cited in open-#652 logs | **ARCHIVED then REMOVED** (97 files, 0 mismatches) |
| **D20** `foundation-test-jeayef_x` | Same | **ARCHIVED then REMOVED** (137 files, 0 mismatches) |

The reviewer also found that (a) the manifest lacked per-row inode/mount, descendants, archive location and restore receipt — corrected here and in `deletion-receipts.jsonl`; (b) 12 of the 14 `foundation-test-*` dirs contain 3–4 nested single-commit synthetic repos, not "no git" as stated; (c) this report's earlier specimen count of 18 was wrong — there are **15**. All three were accepted and fixed before any deletion.

The reviewer independently confirmed: **no committed work can be lost** by the 8 worktree removals (each HEAD equals its `origin` branch tip, 0 unpushed commits, clean including ignored files); none of the 28 rows contained live lock state, the #652 source, #785–#788 art, #730/#789 outputs or #690 save work; the 10 `history.jsonl` copies inside the rows were synthetic (different inodes, none hashing to `dfb50644…`); and none of the 221 registered worktrees lay under a Batch B root.

---

## 6. Deletion receipts (actual)

Executed with a hardcoded allowlist, no globs, no `--force`, no `git clean`, no `git worktree prune`, no branch deletion. Every row re-checked for path, inode and mount immediately before removal.

### 6.1 Git worktrees — non-force `git worktree remove`

| Path | Branch | HEAD | rc |
|---|---|---|---|
| `…/CCF-781-A1-PhysicalIdentity-20261118` | `docs/sen-1-781-a1-physical-identity-20261118-v1` | `a7e985ee` | 0 |
| `…/CCF-781-A2-Coordination-20261118` | `docs/sen-1-781-a2-coordination-20261118-v1` | `21035b27` | 0 |
| `…/CCF-781-Parent-20261118` | `docs/sen-1-781-parent-ops-recovery-20261118-v1` | `3af8988d` | 0 |
| `…/CCF-781-Q-Review-20261118` | `review/sen-1-781-q-ops-admission-20261118-v1` | `cc53729d` | 0 |
| `/media/jer/Videos/CCF-794-Parent` | `docs/sen-1-794-unified-ops-parent-20261119-v1` | `e7a87f36` | 0 |
| `/media/jer/Videos/CCF-794-QA-Review` | `review/sen-1-794-qa-ops-admission-20261119-v1` | `f3f8c9ed` | 0 |
| `/media/jer/Videos/CCF-794-QC-Review` | `review/sen-1-794-qc-coordination-preauth-20261119-v1` | `94d07fd3` | 0 |

All branches remain on `origin` at these exact SHAs. Worktree registry: **221 → 214**.

### 6.2 Directories — literal exact-root recursive removal

18 directories, all `rc=0`, all confirmed absent afterwards:

`CCF-794-Scratch` (archived first) · `foundation-test-0yh2_igl`, `-1012e2tx`, `-1qpsgar_`, `-6h1iydzh`, `-9x2x0u_q`, `-a8o93028`(archived), `-bezvs_9c`, `-_fcmohm_`, `-jeayef_x`(archived), `-npq3e7x1`, `-oa64u2xw`, `-pz52tiqx` · `ops652-final-source`, `ops652-optiona-source`, `ops652-rehearsal-v62zx56l` · `probe-J6Nd`, `287-registration-stage`

### 6.3 Reclaimed space (kernel-measured, not estimated)

| Mount | Before | After | Delta |
|---|---|---|---|
| `/media/jer/Files` | 31.40 GB free | 36.35 GB free | **+4.95 GB** |
| `/media/jer/Videos` | 438.07 GB free | 440.56 GB free | **+2.49 GB** |
| **Total** | | | **≈ 7.44 GB** |

Apparent-size sums overstate the reclaim because apparent bytes ≠ allocated blocks; the measured `df` delta is the figure of record.

### 6.4 Archival before irreversible removal

`archive-manifest-before-delete.txt` — per-file sha256 for **42,570** files across all deleted rows, sha256 `e6680916496f5a9c34a0bb96cd905d800c8d00bb5a27fc9382ca5eb8dbec636e`.

`archive/D09`, `archive/D16`, `archive/D20` — byte-verified copies, **0 mismatches**.

---

## 7. Policy compliance

- Parent worktree `/media/jer/Videos/CCF-802-Parent-20261119` (non-root, disjoint, new at `origin/main` `159f0ff8`); scratch under `/media/jer/Files/CCF/Verification/Sen1-802-LockContainment-20261119/` (non-root).
- Nothing written to OS root `/`, to `Library/Temp/Logs/obj`, or to the deployed Ops tree.
- `/media/jer/Files/CCF-Primary`'s seven original dirty/untracked items untouched.
- No Unity launch, no Windows build, no Multi/gameplay/save change, no forced lease release, no `git clean -fdx`, no `git worktree prune`, no branch deletion, no package/LFS change.

---

## 8. Residual risk — retained live-capable entrypoints

| Group | Copies | Live-default? |
|---|---|---|
| Deployed `/media/jer/ZX20/CCF-Ops` | 1 | yes (legitimate) |
| #652 source `CCF-OpsReadiness-652-20261116` | 1 | **yes — protected custody** |
| `CCF-OpsLeaseFix-20261115` | 1 | **yes — protected custody** |
| `Sen1-741-ExFATQual*` (11 dirs × 2) | 22 | **yes** |
| `Sen1-752-NativeV6` | 2 | **yes** |
| `Sen1-754-*` (3 dirs) | 10 | **yes** |
| `foundation-test-66okebci`, `-jeayef_x` | 2 | **yes** |
| Rejected / non-default leftovers | 2 | no |
| **Total retained** | **41** | **39 live-default → 38 non-deployed** |

**`RETAINED NONDEPLOYED LIVE-CAPABLE = 38`.** A retained copy is not automatically a defect, but each is a reachable path to the live lock under the same uid. Labelling this PASS would contradict the packet.

---

## 9. What would actually close this

1. **G1 source route** — a real physical lease on `CCF-652-OPS-READINESS`, an actor-specific `APPROVED TO START SOURCE`, an amended file ceiling and a named continuing/transferring custodian. Then implement the fail-closed installed-location + backing-device admission guard, prove the negative tests on isolated synthetic state, and re-audit survivors.
2. **Evidence-retention route** — an owner decision releasing the `Sen1-741/752/754` specimens for retirement. That alone would remove 34 of the 38 remaining live-capable copies, leaving the 2 protected source checkouts plus the deployed copy.
3. **Custodian release for #792** so `CCF-792-Admission` (held) can be retired.

None of these were available. This is the exact next executable gate — not a request to re-run #794.

---

## 10. Outstanding gates

| Gate | Owner | Blocks |
|---|---|---|
| Physical lease + `APPROVED TO START SOURCE` on `CCF-652-OPS-READINESS` | Owner / #731 | Any containment source edit (G1) |
| Custody: continuing vs transferring from session `9bcdfde6` | Owner | Source admission |
| `APPROVED TO DEPLOY THIS EXACT CONTAINMENT BUILD` + installer reservation | #731 + Ops custodian | G2 |
| R-1 interrupted-installer signal exclusion; D-1 temp-file handling | Ops custodian | G2 |
| Release decision on `Sen1-741/752/754` specimens | Owner | Removing 34 residual live-capable copies |
| Custodian release for `#792` worktree | #792 custodian | Retiring the held worktree |
| Individual retirement of ~200 older registered CCF worktrees | Manager | Further Files/root reclamation |

---

## 11. Honest limitations

- **Containment is not achieved.** 38 non-deployed copies still default to live state.
- No guard was written, so no negative-test evidence exists for one.
- Same-uid hostility is out of scope; OS permissions under one Linux user are not a security boundary.
- `board/active.md` changed since #794; not written here, and not independently attributed beyond timestamps and the recorded owner/manager comments.
- The reviewer could not verify processes owned by other OS users (no access).
- Reclaim is reported from `df` deltas; exFAT free-space accounting on ZX20 is coarse, so only the Files/Videos deltas are asserted.
- Dashboard/Drive mirrors: **NOT VERIFIED**.

---

*Published by the #802 parent. §6 counts are filesystem receipts from `deletion-receipts.jsonl`, not proposals. Manifests and archives: `/media/jer/Files/CCF/Verification/Sen1-802-LockContainment-20261119/`.*
