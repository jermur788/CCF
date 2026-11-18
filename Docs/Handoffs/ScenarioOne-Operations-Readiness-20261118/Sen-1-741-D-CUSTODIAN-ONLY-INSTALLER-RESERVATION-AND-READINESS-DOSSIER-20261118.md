# Sen-1 #741-D — custodian-only future installer reservation packet and live readiness dossier

**STATUS: DOCUMENTATION/PLAN ONLY. NO RESERVATION TAKEN. NO LIVE OPS WRITE. NO BOARD OR LANES EDIT. NO UNITY. NO INSTALL PERMISSION GRANTED BY THIS DOCUMENT.**

**Date:** 2026-11-18, observation window 13:39:54–13:44 UTC.
**Author:** DeepSeek V4.1 Flash 1017 (`opencode-go/deepseek-v4.1-flash`), provider instance `opencode`, harness T3 Code / OpenCode, host `jer-laptop`, Linux user `jer` uid 1000. Orchestrator for [#741](https://github.com/jermur788/CCF/issues/741); **not** a source author, **not** the independent reviewer, **not** the installer, **not** a physical board custodian.
**Subtask:** packet §3.D of `Sen-1-741-DEEPSEEK-ONE-CYCLE-OPS-DEPLOYMENT-AND-COMPACT-UNITY-READINESS-ORCHESTRATION-20261118.md`.

> The earlier direct owner order **NO LIVE INSTALL** remains operative and is not released by anything here. This dossier prepares what a separately authorised custodian/installer would need; it asserts no present physical ownership of the board, no installer lease and no deploy approval.

## 1. Purpose and authority boundary

This document does three things and no more:

1. Re-establishes, from fresh read-only observation, whether the physical preconditions recorded in the [2026-11-18 #652 preflight](https://github.com/jermur788/CCF/blob/8506db60b4ecf4f42d212c4c0a2bd749cf622571/Docs/Handoffs/ScenarioOne-Operations-Readiness-20261118/Sen-1-652-D091-LIVE-DEPLOYMENT-PREFLIGHT-AND-CANDIDATE-PROJECTION-RESULT-20261118.md) still hold.
2. States the **exact** future live mutation set, so a later installer cannot widen it by interpretation.
3. Hands the actual [already-authorised physical board custodian](https://github.com/jermur788/CCF/issues/731) a paste-ready reservation/correction packet for the **existing** `CCF-652-OPS-READINESS` row.

It does **not** reserve anything. A reservation exists only once a genuinely authorised physical custodian writes it under the live board-directory flock with its own before/after receipt.

## 2. Fresh read-only observation (executed, not inherited)

All commands were reads. No `flock` was taken on live Ops, no file was created under `/media/jer/ZX20/CCF-Ops`, and no repository, board, lane, lease or Unity state was touched. Evidence: `/media/jer/Files/CCF/Verification/Sen1-741-DeepSeek-Orchestrator-20261118/d-custody/`.

| Check | Fresh result 2026-11-18 13:39–13:44 UTC | Versus preflight 13:12Z |
| --- | --- | --- |
| ZX20 device / fstype / mount | `/dev/sdd2`, `exfat`, `rw`, `uid=1000,gid=1000,fmask=0022` | identical |
| ZX20 UUID | `18D6-896E` | identical |
| ZX20 free space | 53,034,745,856 B (90% used) | byte-identical free space |
| Files device / fstype | `/dev/sdb4`, `ext4`, `rw` | identical |
| Files UUID | `2283b135-bf37-42d6-921f-2fb861d0a8d4` | identical |
| Files free space | 40,342,376,448 B (92% used) | was 44,068,823,040 B (91%) — see §3 |
| Installed Ops files | `sha256sum -c` against the preflight `live-inventory.sha256`: **50/50 OK, 0 mismatch** | identical |
| `board/active.md` | `2b99552c…` (mtime 12:52:39Z) | identical |
| `lanes.json` | `c17903dd…` (mtime 12:52:39Z) | identical |
| `deployment.json` | `8c66b290…` | identical |
| Live receipt audit (source `verify-deployment --root /media/jer/ZX20/CCF-Ops --manifest <live deployment.json>`) | **PASS exit 0** — 28 source hashes, 7 pinned overrides, 1 config projection, 5 reasoned live-state entries | identical verdict |
| Original board lint (source `packet-lint --board` run on a **byte-identical copy** of the live board in my own evidence folder) | **PASS exit 0 — 42 live tasks** | identical verdict |
| Live board-directory flock | none: no entry in `/proc/locks` for device `08:32`; board inode 1399769 and Unity guard inode 1598961 unheld | identical |
| Unity Editor process | none (no `Unity` argv[0] process) | identical |
| `state/unity/lock.json` | **absent** | identical |
| `state/unity/guard` | empty, `e3b0c442…` | identical |
| `state/unity/history.jsonl` | 299,478 B, `dfb50644…` | identical |
| Live file mtimes newer than 12:53Z | **none** | — |
| Accepted source branch | local = `origin` = `4c7acc05`, one checkout only (`/media/jer/Files/CCF/Operations/CCF-OpsReadiness-652-20261116`), clean | identical |
| `CCF-412` personal Editor playtest row | `WAITING`, **NOT STARTED** | identical |

**Reading of the above:** the projection, the receipt, the board, the lanes and the Unity guard are still exactly what the preflight recorded. Nothing is occupying the machine-wide Unity lock. No competing Ops writer was observed at any moment in this window.

## 3. Detected changes since the 12:58–13:16Z preflight

| Change | Assessment |
| --- | --- |
| `board/active.md` unchanged since `2b99552c…` (12:52:39Z) | #734 and #735 drift predates the preflight inventory; **no new drift**. Their rows and lanes remain protected and are not reverted, restated or reinterpreted here. |
| `lanes.json` unchanged since `c17903dd…` | as above; still 23 lanes; no `#652` lane and no installer reservation exists. |
| Files filesystem free space fell ~3.73 GB (44,068,823,040 → 40,342,376,448 B, 91% → 92%) | **Non-blocking for this projection.** The full 50-file backup is well under 1 MB; durable evidence is still writable. Flagged so a later installer does not treat the preflight capacity figure as current. ZX20 capacity (the actual 16.1 GB minimum gate) is unchanged. |
| New evidence directories under `/media/jer/Files/CCF/Verification/` (mine, and the #741 children's disjoint lanes) | Expected; Files only; no effect on Ops. |
| Nothing else | No Ops source, board, lanes, ledger, guard, history, script, config or Unity state changed. |

**No Multi #730 custody action, and none of #734/#735's changes, was touched, reverted or reinterpreted.**

## 4. The immutable future live mutation set (cannot be widened here)

Exactly **five file replacements plus one receipt write**. Anything else is out of scope and a STOP.

| # | Live path (under `/media/jer/ZX20/CCF-Ops/`) | New bytes SHA-256 | Current live SHA-256 | Note |
| --- | --- | --- | --- | --- |
| 1 | `scripts/unity-lock` | `8fa0c5ada367f109c9437b05bd37b42c7ce78472d6f630634df8745fcb220ac8` | `54cf0a95561d0a8c350d89adbee83f393990b728d6127b94f69c587260f048e5` | reviewed D-091 |
| 2 | `tests/self-test.py` | `1bd03f1520e21db6f6bd00b963e09437267ebf4604ad6772c8e5fffffa9e6e50` | `2a12a19d848ae789a1404c7ebd5285457e061b0a7142b91de95ed0b79cdeeaf3` | reviewed D-091 233-check suite |
| 3 | `roles/unity-operator.md` | `e44e7eb89d33a82d4e3dc5987572565e394d12a60ec82733d3dfd4bb103206db` | `120f1deb006aa56a9d4b9643917f855548b5da80432a7166442ecc6163eee815` | reviewed D-091 |
| 4 | `scripts/_ccf.py` | `24a321466e6dae61ba950c4cbc4f918e12f7d6be8a6af628341c5aa8efe0b435` | `ab4343efc7d1b82c5a4aca747d0e09c614779cfd9aa406a0dd681d204f5de001` | approved Option A origin, required by the D-091 suite |
| 5 | `README.md` | `3baf029ef6da792c3129ce5e0e91b3097d69ee7c881f389ad0bb9a377de41123` | `249be391e172e9d80f7d3d3419af961850b84c40fb2f15ba7b1d0469649eb517` | deployment-only merged document (subtask A v2 + orchestrator repair R-1), **not** the reviewed source README `a1911b01…` |
| 6 | `deployment.json` | `ee13a811304c4cbfced23c4969b2a94d970d22fc5d424b72fa24dd2a943ad216` | `8c66b290c247708846cb8b59f83626a10a3fa1d052397a3316dc5f23fc157b18` | prospective receipt (`TEST_FIXTURE_ONLY`, `NOT_DEPLOYED`, `deployed_at: null`), **not** a source-code copy; previous receipt kept as `deployment.json.bak-<UTC>` |

The README and receipt bytes above are **proposed**, produced and rehearsed in isolated scratch under subtask B. They are not installed and are not installed provenance.

**Explicitly NOT mutated (must be byte-identical before and after):** `environment.sh` (`3775cba7…`), `AGENTS.md` (`d1ce3ad7…`), `lanes.json` (`c17903dd…`), `board/active.md` (`2b99552c…`), all `ledger/*`, all `state/*` including `state/unity/{guard,history.jsonl}` and any `lock.json`, `scripts/lane-status`, `scripts/packet-lint`, and the remaining installed files.

**Not installed at all:** `scripts/verify-deployment` and `DeploymentProvenance.md` (both source-only). Source `deployment.json` bytes (`be9fbcd0…`) must never be copied — they are stale for D-091 and already FAIL the verifier.

**Known physical constraints:** ZX20 is exFAT mounted with `fmask=0022`; every file is effectively `0755 jer:jer` and `chmod`/`chown` cannot express per-file modes. Git modes are not representable on the target. The receipt must therefore record bytes and ownership readback, not mode expectations.

## 5. Who may do what (no implied ownership)

| Step | Required actor | Current state |
| --- | --- | --- |
| Stage, rehearse, prepare merged README + prospective receipt | #741-A (docs) and #741-B (native staging) under this orchestration | **in progress** |
| Independent nonauthor review of staged projection/README/receipt/rollback | #741-C separate session | **not started** |
| Correct the stale `CCF-652` row and reserve an exclusive installer scope | **an actually authorised physical board custodian**, under the live board-directory flock, with its own receipt | **NOT DONE — no reservation exists** |
| Release the owner's **NO LIVE INSTALL** order and issues `[APPROVED TO DEPLOY OPS — EXACT FIVE FILES + RECEIPT]` | Owner, then Manager #731, bound to a named installer/session | **NOT ISSUED** |
| Perform the install | that named installer, in the reserved scope | **not authorised, not scheduled** |

**I am none of these actors.** I hold no lease, no lane, no editor and no board row.

## 6. Required fresh readback before any reservation is written

A custodian must repeat, at reservation time, not reuse from here: ZX20 identity/UUID/mount/free space; full 50-file hash set; `board/active.md`, `lanes.json` and `deployment.json` hashes; `/proc/locks` free for device `08:32`; no `Unity` process; no `state/unity/lock.json`; `CCF-412` still not started; and the accepted source branch tip/cleanliness. **Any mismatch with §2/§4 is a STOP, not a value to "update".**

## 7. Paste-ready custodian packet — reservation and row correction (NOT EXECUTED)

> **For the already-authorised physical board custodian only.** Do not run this as a worker with a general task. It is a physical custody action, not a source, Unity or install action.

**7.1 Preconditions.** Owner/Manager direction for this specific row correction; common Ops board-directory flock available; current bytes captured; the accepted source branch tip confirmed at `4c7acc05` (or a later reviewed tip); no installer or Unity holder.

**7.2 Take the exclusive board-directory flock** (non-blocking; abort if held):
`flock -x -n /media/jer/ZX20/CCF-Ops/board -c '<steps>'` — abort with no write if the lock is not obtained immediately.

**7.3 Write a recoverable pre-state copy** of the exact `board/active.md` bytes and `lanes.json` bytes into a new, disjoint custodian receipt folder under `/media/jer/Files/CCF/Verification/`, with SHA-256 of both, before writing anything.

**7.4 Change exactly one row** — `CCF-652-OPS-READINESS` — and nothing else. Required content, stated as facts the row currently gets wrong:

| Field | Current (stale) | Required correction |
| --- | --- | --- |
| State/owner text | `READY | RESERVED ONLY / NOT RUNNING / SOURCE BLOCKED: sole prospective #652 source-only writer … session 9bcdfde6…` | Source is **APPROVED** (independent nonauthor review; 233/233 author ×2 and reviewer ×2). The workstream is now **staging/deployment readiness under #741**. Record the actual pause reason: live install HELD by owner NO-INSTALL order + Manager gate, not "source blocked". |
| HEAD | `a9f56d6385d49ac51e35f303de00d77d1e586195` | **Stale.** `a9f56d63` is an ancestor of the reviewed tip and **predates the D-091 source commits**. Any installer reservation naming it would point at a commit that does not contain the reviewed bytes. Use the exact reviewed branch tip `4c7acc051e262c3fe301049b51ee7fc48a9f99a2` (verify it is still the tip at write time). |
| Locks | `None` | Unchanged — **no installer lease exists**. Do not invent one in the row. |
| Open gate | `Manager #731 assesses this custodian receipt and issues a source-admission verdict naming session 9bcdfde6…` | Superseded. Replace with: staged five-file projection + merged README + prospective receipt + independent nonauthor review + owner release of NO-INSTALL + Manager deploy permit bound to a named installer. |
| Next action | `Source writer stays BLOCKED / NOT RUNNING until Manager #731 source-admission verdict…` | Replace with: staging/rehearsal/review under #741; then custodian reservation for the exact five-file + receipt scope; then owner release; then Manager deploy permit. |

**7.5 After the write:** re-read the row, re-run the original `packet-lint --board` (expect 42 live tasks, exit 0), recompute `board/active.md` and `lanes.json` hashes, release the flock, and publish a one-row-readback receipt with before/after hashes and proof that the other 41 rows and 23 lanes are unchanged.

**7.6 Do not, in the same action:** touch any other row, add or remove a lane, create an installer lane, write `deployment.json`, copy any Ops file, launch Unity, or claim the install has been scheduled.

## 8. Deploy-day sequence an installer would follow (future, unauthorised)

1. Re-run every check in §6 in the reserved window; STOP on any drift.
2. Full verified backup of all 50 installed files (bytes + SHA-256 + owner) into a new Files verification folder; record board/lanes/state hashes but **never restore them from backup**.
3. For each of the five files, in the approved order: confirm current bytes equal the §4 "current live" hash → write `NAME.tmp` in the same directory → `fsync` → `rename` → re-hash. Any path whose current bytes differ is a **foreign-drift STOP for that path**, not a silent overwrite.
4. Write the reviewed prospective `deployment.json`, first preserving the old receipt as `deployment.json.bak-<UTC>`.
5. Verify: source `verify-deployment` against the new receipt (**PASS**, expected 33 hashes + 1 config projection + 5 live-state entries after moving the five into source hashes), original `packet-lint --board` PASS 42, and byte-identity of every protected path in §4.
6. **Rollback** is compare-before-write and only while exclusivity is still held: restore a path only if its current bytes equal the *new* hash; refuse on drift. Restoration must return every pre-state byte exactly.
7. **Null-lock rollback prohibition:** before restoring the old `unity-lock`, read `state/unity/lock.json`. Absent → safe. Legacy ISO record → safe (old reader reads it; preserve the record unchanged). `eta:null` + `no_expiry:true` → **DO NOT restore the old script**: the old reader reports it malformed, so acquire would be refused and normal release impossible. Instead keep the candidate script, verify Editor and recorded PID have exited, and have the owner release normally. Never delete or migrate a held lock record to make a rollback pass.
8. Publish a physical installed-provenance receipt; do not claim installed PASS from source PASS.

### 8.1 Rehearsed facts and hazards the installer must carry

The five-file plus receipt projection was rehearsed by subtask B entirely in isolated scratch (evidence `/media/jer/Files/CCF/Verification/Sen1-741-B-StageRehearsal-20261118/`). Facts that change installer behaviour, all independently relevant:

- **`unity-lock` defaults to the live state directory.** Its default `state/unity` path is hard-coded to `/media/jer/ZX20/CCF-Ops/state/unity`. Running *any staged clone* of the tool without `--test-state-dir` therefore acts on **live** ownership state. Rehearsals and audits must pass an explicit test state dir; the installer must never point a staged copy at live by accident.
- **The check count is data-dependent.** The suite runs one check per ledger file, so the installed tree reported **234** passes where the accepted source tree reported **233** — the live ledger has one entry the source tree lacks and the other 233 labels match. Treat "233" as the source-qualification figure, not a fixed installed contract; a future count differs legitimately as the ledger grows. Do not "fix" the difference.
- **exFAT was not rehearsed.** The replica lived on ext4. Atomic `tmp` → fsync → rename semantics on the live exFAT mount are therefore **unproven for this projection**, and the deploy-day sequence must re-verify each replace and each rollback on the real filesystem while exclusivity is held.
- **Old tooling refuses non-ZX20 paths.** The installed (old) `_ccf` will not operate on an ext4 replica, so old-tool board lint and an old-tool legacy-lock release could not be exercised there. The old reader was still shown to read a legacy record. At deploy time the installed tools operate on their real paths, so this limits rehearsal fidelity, not the live contract.
- **Two inherited documentation debt items ship with this projection** and are accepted deliberately rather than silently: `roles/unity-operator.md` still names retired `ZX20/CCF-Verification` / `ZX20/CCF-Builds` directories, and the merged README still says the self-test "creates fixtures only under ZX20/CCF-Temp" although it also writes Files verification fixtures. Both are pre-existing, present in the reviewed source and/or the live baseline, and correcting either would change an approved hash and require new qualification. Record them; do not "fix" them inside the install window.

## 9. What this dossier does NOT claim

- Not that an installer owns the board, the board row, a lease or a lane.
- Not that the staged projection is reviewed: subtasks A–C were **in progress / not started** when this was written.
- Not that the owner has released **NO LIVE INSTALL**, and not that any Manager deploy permit exists.
- Not that the merged README and prospective receipt are approved: B rehearsed them in scratch and C's independent review was still in progress when this line was written. Treat their presence in §4 as proposed bytes, not as a reviewed install candidate.
- Not that ZX20/Files capacity or the board will be identical at deploy time — they must be re-observed.
- Not any Unity, game-source, save, Multi #730, #690 or Windows status.
- Dashboard / Drive synchronisation: **NOT VERIFIED**.

## 10. Next safe actions

1. ~~A merged README → B staging/rehearsal → C review~~ **Done to the point of review:** A produced the merged README (v2 after orchestrator repair R-1), B staged and rehearsed the exact five-plus-receipt projection in scratch with all eleven procedures passing, and C's independent nonauthor review is the outstanding step.
2. After C's verdict and any bounded correction: hand this §7 packet to the authorised physical custodian with the now-known staged hashes filled in (`README.md` `3baf029e…`, prospective `deployment.json` `ee13a811…`).
3. Owner decides on releasing NO-INSTALL; Manager #731 issues the deploy permit bound to a named installer. Only then does §8 run, on the real exFAT mount, re-observed from scratch.
