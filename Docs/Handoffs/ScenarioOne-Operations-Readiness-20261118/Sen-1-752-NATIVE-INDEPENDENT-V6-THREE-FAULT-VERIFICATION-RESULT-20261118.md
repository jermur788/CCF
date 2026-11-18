# Sen-1 #752 — native independent v6 three-fault verification (result)

**Date:** 2026-11-18. **Reviewer:** OpenCode agent under T3 Code, provider `opencode-go`, model `deepseek-v4.1-flash`, host `jer-laptop`, session `eaea35cb-a7ce-4f07-9988-6d74061630e5`. **Claim:** [#752](https://github.com/jermur788/CCF/issues/752). **Packet:** [#751 GPT-6 browser packet](https://github.com/jermur788/CCF/blob/review/sen-1-751-gpt6-browser-exfat-v6-20261118-v1/Docs/Handoffs/ScenarioOne-Operations-Readiness-20261118/Sen-1-751-GPT6-INDEPENDENT-BROWSER-V6-REVIEW-AND-ONE-NATIVE-VERIFICATION-PACKET-20261118.md). **Branch:** `review/sen-1-752-native-v6-exfat-independent-20261118-v1` from `main` `55d4fb892c83dda3fcca13bf25e099f7e980748a`.

## 1. Verdict

**NEEDS CHANGES — one narrow post-rename booking defect, plus the three named blockers independently confirmed fixed.**

- (a) post-rename receipt readback `KeyboardInterrupt` → **PASS**: 6 booked, 6 `RESTORED`, 50/50 byte-exact, zero `.tmp`.
- (b) one-shot pre-state read `EIO` at index 4 → **PASS**: 3 booked, 3/3 reversed, index-4 path never published, 50/50 exact.
- (c) persistent diagnostic reread `EIO` during recovery → **PASS**: 6 attempts, 5 `RESTORED`, 1 explicit `UNVERIFIABLE`, **not** a baseline PASS; baseline returns only after a separate explicit repair.
- Full unmodified v6 harness on an independently built same-device fixture → **21/21 PASS**.
- **Blocking gap (new, same class, packet item 2):** booking is *not* atomic with publication. A signal delivered between `os.replace` (L93) and the booking record (L94-95) leaves the file published, books nothing, restores nothing, and is reported as `error-before-publication`. A signal between `publish()` returning and `booked.append(rec)` (L155) escapes `batch()` entirely: published file, no recovery, no result. Both reproduced deterministically (§5). This falsifies v6 doc §2.1 ("any later exception — including a signal — cannot leave a published path unbooked"). It does **not** produce a false PASS, and it is not the readback failure #746 named, so (a)-(c) stand as closed.

## 2. Independence, disclosed plainly

Distinct session, no v6 authoring state, no other worker's scratch or artifacts used, subject read fresh from disk. **Material limitation:** this session's provider/model string is identical to the v6 harness author's (`opencode-go/deepseek-v4.1-flash`, same host, same harness). Independence here is by **session and non-involvement**, not by model identity. I state this rather than let it be discovered later; if the Manager requires model-identity independence, this review cannot supply it.

Ordering: registration was posted on #752 at 15:32Z; read-only source inspection began ~15:31Z. No test ran and no subject byte was written before registration.

## 3. Exact subject, verified against the packet's declared hashes

| Artifact | Local path | SHA-256 | Packet-declared |
| --- | --- | --- | --- |
| Harness source | `…/Sen1-741-C1-ExFATQual-20261118/v6/exfat_qual_v6.py` | `d75a218212cedca21f7fa5b93b81181c965aa91487afb782a37b751c61e8eec3` | equal |
| Author result JSON | `…/v6/exfat-qualification-v6.json` | `77e5ed4ef4816f8855067a6b7d91d8eed083964a35c27319161337e8b2eb017c` | equal |
| `SHA256SUMS` in `v6/` | — | agrees with both lines above | — |

Both originals re-hashed **unchanged after** the review. Originals were not copied over, edited or regenerated.

## 4. Fixture and custody (independent of the author's run)

- Scratch: `/media/jer/ZX20/CCF-Temp/Sen1-752-NativeV6-20261118T153253Z`, new and disjoint, `/dev/sdd2` **exFAT**, `st_dev` **2098** = live Ops `st_dev` 2098. `Sen1-741-*` scratch trees untouched and unused as input. The staged projection bytes were read read-only from the approved #741-B input tree (`inputs/source-4c7acc05`, `inputs/README.md.merged-v2`); nothing was written there.
- `base/CCF-Ops` = byte copy of live `/media/jer/ZX20/CCF-Ops` (50 files) taken read-only; verified equal to a fresh live manifest (`bd01a398…`, 50/50).
- Staged projection = approved bytes, hashes matching #741 §5: `8fa0c5ad…` `1bd03f15…` `e44e7eb8…` `24a32146…` `3baf029e…`; receipt fixture `ee13a811…` (`TEST_FIXTURE_ONLY`, never installed).
- Harness replays used a **byte-identical copy** (`d75a…`) of the reviewed script; my targeted drivers load the reviewed file's function definitions directly from the original path (`exec` of the definitions region only, no edits).

Commands (exact):

```
python3 <scratch>/replay/exfat_qual_v6_replay.py <scratch> /media/jer/ZX20/CCF-Ops \
  <scratch>/evidence/live-inventory-before.sha256 <scratch>/staged <scratch>/fixture/deployment.json
python3 <scratch>/replay/targeted_three_fault_replay.py \
  /media/jer/Files/…/v6/exfat_qual_v6.py <scratch> /media/jer/ZX20/CCF-Ops <scratch>/staged <scratch>/fixture/deployment.json
python3 <scratch>/replay/boundary_probes.py \
  /media/jer/Files/…/v6/exfat_qual_v6.py <scratch> /media/jer/ZX20/CCF-Ops <scratch>/staged <scratch>/fixture/deployment.json
```

Environment: Python 3.12.3, Linux 7.0.0-42-generic; mount `exfat rw,relatime,uid=1000,gid=1000,fmask=0022,dmask=0022,iocharset=utf8,errors=remount-ro`.

## 5. Results

### 5.1 Full unmodified harness — 21/21 PASS

Independent fixture, run 15:34:11–15:34:15Z. Summary `{"total": 21, "pass": 21, "fail": 0}`. The three target rows:

| Test | failure | booked | recovered |
| --- | --- | --- | --- |
| `batch.receipt-readback-SIGINT` | `post-rename-error` (readback) | 6 | 6 × `RESTORED` |
| `batch.pre-state-read-eio-does-not-bypass-recovery` | `error-before-publication`, `scripts/_ccf.py`, index 4, `OSError` | 3 | 3 × `RESTORED` |
| `batch.diagnostic-re-read-eio-does-not-stop-recovery` | `post-rename-error` (dir-sync) | 6 attempts | 5 `RESTORED` + `scripts/_ccf.py` `UNVERIFIABLE` |

Result JSON for my run: `d9ff25668f6dbc5e046251cab57c6f1fa06a3f8e223ab6bf28547cfdf0464386` (differs from the author's `77e5…` only through root path, fixtures and timestamps — expected).

### 5.2 Targeted replay — 30 assertions, 0 failures

Each case ran on a **fresh** copy of the 50-file replica, with the pre-state asserted equal to the backup and to the live manifest before injection.

**(a) `KeyboardInterrupt` during post-rename receipt readback** (`interrupt_readback`, index 6)

- booking record for `deployment.json`: `renamed=true`, `observed_sha=null`, `dir_sync_ok=true`, `error={where:readback, type:KeyboardInterrupt, errno:null}` — i.e. the interrupt fired *after* publication and after booking.
- first five booked records carry the exact new hashes → publication really happened.
- recovery: 6/6 `RESTORED`; final inventory **identical to the 50/50 pre-state**; **zero** `.tmp`.

**(b) one-shot pre-state read `EIO` at index 4** (`fail_prestate_at=4`)

- failure exactly `{kind:error-before-publication, at:scripts/_ccf.py, index:4, type:OSError}`.
- booked = 3, each `observed_sha` = new hash, `dir_sync_ok=true`; index-4 path still the live old hash `ab4343ef…` (never published).
- recovery 3/3 `RESTORED`; final inventory identical 50/50; zero `.tmp`.

**(c) persistent diagnostic reread `EIO` during recovery** (`hostile_recovery_path=_ccf.py`)

- recovery attempts = 6 (not aborted); outcomes `5 × RESTORED` (`unity-lock`, `self-test.py`, `unity-operator.md`, `README.md`, `deployment.json`) and `scripts/_ccf.py = UNVERIFIABLE`.
- inventory differs from baseline in **exactly one** path, `scripts/_ccf.py`, holding the published new bytes `24a32146…`.
- with the diagnostic fault removed the same read succeeds at `24a32146…`, proving the classification is conservative, not a real unreadability.
- baseline returns **only** after explicit `shutil.copyfile` repair — i.e. the harness does not present partial state as green.

**(control) unobstructed six-file stage then full rollback:** 6 booked, no failure, 6/6 `RESTORED`, 50/50 exact, zero `.tmp`.

### 5.3 Static control-flow inspection (source line numbers)

- `publish()` L86-112: temp write+fsync L89-92, `os.replace` L93, booking dict L94-95, readback L97-104 under `except BaseException`, dir-sync L105-111 under `except BaseException`; it returns a record and never raises after rename **except** in the window L93→L94.
- `batch()` L129-181: pre-state read L140 inside `try` L137; publish L143 inside `try` L137-154; `booked.append(rec)` L155 **outside** that `try`; failure classification L156-159; recovery loop L168-179 with `except BaseException` per restored path L174.
- `safe_sha()` L53-58 never raises. Recovery continues after a diagnostic failure (L174-179). No live path write: `LIVE_OPS` is used only for `stat()` at L187; the only subprocesses are `findmnt` (L184) and `flock`/`sleep` custody probes (L302-306). No staged script is executed, so no live `unity-lock` state-dir default can be triggered.

## 6. The blocking finding — booking is not atomic with publication

Two deterministic line-boundary injections (`sys.settrace`, no subject edit) model a signal delivered between bytecodes:

| Injection point | Observed |
| --- | --- |
| `publish`, line 94 (immediately after `os.replace` completed, before the record is bound) | `batch` returned `{kind: error-before-publication, at: scripts/unity-lock, index: 1, type: KeyboardInterrupt}`, **booked 0**, **recovered 0**, tree differs by `scripts/unity-lock` = `8fa0c5ad…` (the new bytes), baseline **not** restored |
| `batch`, line 155 (passed `publish()`, before `booked.append`) | `batch` raised `KeyboardInterrupt` out to the caller, **no result**, **booked 0**, **recovered 0**, same single published file, baseline **not** restored |

Why this is a real defect and not a test artefact: CPython runs a pending signal handler at bytecode boundaries, so a `SIGINT` arriving during `os.replace` is normally deferred to the instruction right after it — exactly the L93→L94 window. The window is a handful of bytecodes, so it is unlikely per file, but it is reachable and it is the same failure class as #746's blocker: *published, unbooked, unrestored*. The first probe additionally records a **false** cause (`error-before-publication`), which is the one thing a reconciling operator must not be told.

It is not a false PASS: no path reports success. It is an abandonment with a wrong reason (probe 1) or an uncaught escape (probe 2).

### Required single patch (blocking)

In `batch()`:

1. Treat **any** `BaseException` escaping `publish()` as *may-be-published*. Before breaking, probe `safe_sha(base/rel)`. If it equals the planned `expected` (or differs from `live_before[rel]`), book `{"path": …, "renamed": True, "expected_sha": expected, "observed_sha": None, "dir_sync_ok": None, "error": {...}}` so the recovery loop covers it. Emit `error-before-publication` **only** when the probe proves the destination is unchanged.
2. Move `booked.append(rec)` inside the guarded block, and run the recovery loop in `finally:` so an exception after publication still reaches recovery rather than escaping `batch()`.

Acceptance for the patch: (a) 6/6, (b) 3/3 with index 4 never published, (c) 5 `RESTORED` + 1 `UNVERIFIABLE` all still hold, **plus** the two boundary probes above must end with 1 booked / 1 `RESTORED` and a byte-exact 50/50 tree, and probe 1 must no longer say `error-before-publication`.

## 7. What is NOT established

- **SIGKILL, `SIGSTOP`, kernel panic, power loss:** no in-process handler runs. Not testable here and not claimed. The mitigation remains #741's approved stage dossier: verified **off-volume** backup of all 50 files before the first write, and after any uncatchable interruption, re-observe and reconcile installed hashes against both the backup and the projected set under renewed exclusive custody before continuing. Nothing in this review weakens or replaces that requirement.
- **Real signal timing:** the boundary probes model signal delivery at line boundaries with `sys.settrace`; they are not a delivered OS `SIGINT`. They are deterministic evidence of reachability, not a probability measurement.
- Kernel/filesystem generality: one kernel, one exFAT device. Directory `fsync` success is device- and kernel-specific.
- The live directory's cluster history/free space, live flock/Unity lease, Git worktree behaviour and real load were not exercised.
- No live install, board reservation, lane/ledger change, Unity run, or Windows build was performed or is authorised by this document.

## 8. Live Ops — read-only, unchanged

50/50 manifest `bd01a398fdcf770de932b5e29a5186a05958db22bd40354fe83b07a4331a41bf` identical before (15:33Z) and after (15:37Z). Board `2b99552c…`, lanes `c17903dd…`, receipt `8c66b290…`; no `lock.json`; no Unity process; directory mtime still `2026-11-18 12:52:39Z`. No production file was replaced even temporarily.

## 9. Evidence

Durable copy: `/media/jer/Files/CCF/Verification/Sen1-752-Native-V6-20261118/` with `SHA256SUMS`:

| File | SHA-256 |
| --- | --- |
| `replay-qualification-21.json` | `d9ff25668f6dbc5e046251cab57c6f1fa06a3f8e223ab6bf28547cfdf0464386` |
| `targeted-replay.json` | `e9b8e27298cef2eed013d6065b52ff65c719ad8f87273ca1e26bf65bd4af6999` |
| `boundary-probes.json` | `14a342034c4674a78ea0dbf1c2d211142530daae99ffb8c3ea31276f0e80c789` |
| `full-replay-stdout.log` | `e1a59bd10b835c0e7d6fd5708b942272b2cbe57f47759719f68568c7820b9c3a` |
| `live-inventory-before.sha256` / `-after` | `bd01a398…` (identical) |

Live scratch (authoritative during the run): `/media/jer/ZX20/CCF-Temp/Sen1-752-NativeV6-20261118T153253Z`.

## 10. Status

`NEEDS CHANGES` applies only to the booking/publication atomicity in §6. The three named v5 blockers are closed with reproduced evidence. #652 live install and Unity remain **HELD**; the owner's **NO LIVE INSTALL** order stands; the owner's power-loss risk decision remains separate and outstanding. No merge to `main`, no board/lanes/ledger/guard/Unity/installer/Windows action.
