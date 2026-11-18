# Sen-1 #754 — Booking-gap patch and independent acceptance (consolidated result)

**Date:** 2026-11-18. **Claim:** [#754](https://github.com/jermur788/CCF/issues/754). **Packet:** [Sen-1-731-754 one-cycle handoff](https://github.com/jermur788/CCF/blob/docs/sen-1-731-chas-manager-intake-20261118-v1/Docs/Handoffs/ScenarioOne-Operations-Readiness-20261118/Sen-1-731-754-ONE-CYCLE-BOOKING-GAP-PATCH-AND-INDEPENDENT-VERIFICATION-20261118.md). **Defect source:** [#752 native result](https://github.com/jermur788/CCF/blob/review/sen-1-752-native-v6-exfat-independent-20261118-v1/Docs/Handoffs/ScenarioOne-Operations-Readiness-20261118/Sen-1-752-NATIVE-INDEPENDENT-V6-THREE-FAULT-VERIFICATION-RESULT-20261118.md).

## 1. Outcome

**Candidate for Manager #731: scratch harness `exfat_qual_v7_1.py` (`52d4c6ae…0eab`). Independent review APPROVED (v7). Independent delta re-check APPROVED (v7.1). Both are scoped to the scratch harness only.**

- Both #752 windows are closed. These are (1) between `os.replace()` and booking, and (2) between `publish()` returning and `booked.append`. At every index, including the receipt, an interrupt in either window now ends with the path booked, `RESTORED`, the tree back to 50/50 byte-exact, no `.tmp`, and the failure never labelled `error-before-publication`.
- The original 21/21 harness passes with an unchanged test body. The three earlier cases still hold.
- Unreadable and foreign-drift destinations fail closed.
- **One residual stays OPEN (R-1), with one minor (D-1)** (§6). Neither is one of the two #752 windows. Both must be carried into the real deployer's design.
- **No live install, board, lanes, ledger, guard, Unity, game-source or Windows action.** Status table in §9.

## 2. Identities and independence

| Role | Actual identity | Session | Claim / handoff |
|---|---|---|---|
| Patch author | Claude Opus 5.5 (`claude-opus-5-5`), Anthropic, Claude Code (Agent SDK) in T3 Code, host `jer-laptop` | `05912f35-360b-4806-aec1-4d694ed2a154` | [RUNNING 6985686760](https://github.com/jermur788/CCF/issues/754#issuecomment-6985686760); HANDED-OFF comment follows this commit |
| Independent reviewer (v7) | Claude Sonnet 5.5 (`claude-sonnet-5-5`), same harness and host, separate T3 child thread | `3b5c687d-00bb-4e74-a0c9-19ab9e572fce` | [RUNNING 6985945065](https://github.com/jermur788/CCF/issues/754#issuecomment-6985945065), [HANDED-OFF 6986964692](https://github.com/jermur788/CCF/issues/754#issuecomment-6986964692) |
| Independent delta re-checker (v7.1) | Claude Sonnet 5.5, new separate T3 child thread | (see its claim) | [RUNNING 6987434505](https://github.com/jermur788/CCF/issues/754#issuecomment-6987434505), [HANDED-OFF 6988173372](https://github.com/jermur788/CCF/issues/754#issuecomment-6988173372) |

**Independence limitation (stated plainly):** the reviewers are a different model from the author (Sonnet 5.5 vs Opus 5.5). They are **not** a different model family: all three are Anthropic Claude. The packet's preferred real Sonnet 5.5 was used; Sonnet 6.1 was not available and was not substituted. There are two mitigations:
- The first reviewer wrote its fault-injection driver from scratch, without reading the author's driver. It validated that driver by reproducing the #752 defect on unmodified v6.
- The delta re-checker reused an adapted copy of the first reviewer's driver plus its own new probe, so that driver's blind spots carry over (its own disclosure).

None of the three is the #752 worker or the v6 author.

## 3. Reviews

- **Independent review of v7: APPROVED** (0 blocker, 0 major in scope; R-1 residual; F-2, F-3, F-4 minor; 2 NIT). [`22537de6` Sen-1-754-INDEPENDENT-REVIEW-20261118.md](https://github.com/jermur788/CCF/blob/22537de603272b3e4bba261fcd8b7502b6c2f9c4/Docs/Handoffs/ScenarioOne-Operations-Readiness-20261118/Sen-1-754-INDEPENDENT-REVIEW-20261118.md), doc sha256 `c0163e2b…afcb`.
- **Internal bounded loop:**
  - The author resolved F-2 (an interrupted restore left a `.tmp` that blocked a retry; this was a regression from v7's `O_EXCL`) and F-3 (a `RESTORE-ERROR` entry could carry `rel: None`).
  - The fix went into a new file, v7.1, so the approved v7 stays intact.
  - F-4 is answered by §6 of this document.
- **Independent delta re-check of v7.1: APPROVED** (0 blocker, 0 major; F-3 resolved; F-2 resolved for its tested windows; new minor D-1; 3 NIT; R-1 unchanged and acceptable only as a documented OPEN residual). [`982f8f5b` Sen-1-754-DELTA-RECHECK-20261118.md](https://github.com/jermur788/CCF/blob/982f8f5bdd09c88d49d78430e8d4dd35d810f8f4/Docs/Handoffs/ScenarioOne-Operations-Readiness-20261118/Sen-1-754-DELTA-RECHECK-20261118.md).

No genuinely new unsafe scope and no ownership conflict arose, so no safety STOP was needed.

## 4. Source and patch

| Artifact | SHA-256 | Note |
|---|---|---|
| v6 original `/media/jer/Files/CCF/Verification/Sen1-741-C1-ExFATQual-20261118/v6/exfat_qual_v6.py` | `d75a218212cedca21f7fa5b93b81181c965aa91487afb782a37b751c61e8eec3` | never edited; re-hashed unchanged at start and end |
| v6 original JSON | `77e5ed4ef4816f8855067a6b7d91d8eed083964a35c27319161337e8b2eb017c` | unchanged |
| v7 (reviewed) | `02606febc26f8bda286387010ca2a16f25dc24b78fe3b993122b9be1e74f004c` | 450 lines |
| **v7.1 (candidate)** | `52d4c6aedbecd47c4cf912f8903fe07547cb24c9b743aa9c65f15fdf35c60eab` | v7 plus F-2/F-3 |
| `v6-to-v7.diff` / `v7-to-v7_1.diff` / `v6-to-v7_1.diff` | `c14c0296…649b` / `f1f388b8…68c0` / `77928a9b…1683` | |

**What changed (only `publish()`, the new `classify_interrupted_step()` and `forward_phase()`, `batch()`, the docstring and the output JSON name):**

1. `publish()` writes a stage into a caller-owned `progress` dict. It sets `"replace"` **before** `os.replace()` and `"replaced"` after. An exception escaping while the stage is `replace` or `replaced` therefore means *may be published*.
2. The temp file is created with `O_EXCL`. Only a temp file that this call created is ever removed. In v7.1, `publish()` also removes its own temp file if interrupted before the rename (F-2).
3. Any `BaseException` in a file's step, including one between `publish()` returning and `booked.append`, is handled as follows:
   - A may-be-published path is **booked first**, then re-read with `safe_sha`.
   - It is labelled `error-before-publication` (and un-booked) **only** when the re-read equals the pre-state.
   - Equal to the staged bytes → `interrupted-after-publication`, then restored.
   - Unreadable → `destination-state-unverifiable`: stays booked, recovery reports `UNVERIFIABLE`, and the baseline is not claimed.
   - Other bytes → `destination-foreign-drift`: recovery reports `REFUSED` and never overwrites.
   - If the path was never renamed, the result is `pre-state-drift` or `pre-state-unverifiable`. These go to `uncertain`, not to the booking.
4. The forward loop is its own function, `forward_phase()`. Any exception escaping it surfaces at the covered call in `batch()`, which re-classifies `state["inflight"]` and still runs recovery. This includes exceptions from CPython 3.12 `try:` NOPs, which sit outside the exception table and matter only under settrace models. Partial state is kept in an optional caller-owned `evidence` dict.
5. v7.1: a `RESTORE-ERROR` entry always names its path (F-3).

**Unchanged:**
- The 21-check main body is byte-identical to v6 except the JSON filename (independently confirmed by both reviewers).
- `sha`, `safe_sha`, `inventory`, `fsync_dir`, `restore_one` and `run` are unchanged.
- The five future deploy paths, the D-091 source hashes, the merged README `3baf029e…` and the receipt fixture `ee13a811…` are unchanged. This is a **scratch deployment-harness fix**, not production deploy authority.

## 5. Native evidence (author, same device)

**Custody:**
- Scratch `/media/jer/ZX20/CCF-Temp/Sen1-754-Author-20261118T170536Z/` is exFAT on `/dev/sdd2`. Its `st_dev` is **2098**, the same as live `/media/jer/ZX20/CCF-Ops`.
- Mount: `exfat rw,relatime,…,errors=remount-ro`. Python 3.12.3, Linux 7.0.0-42-generic.
- Replica is a byte copy of live Ops: 50/50, manifest `f45385bf…3301`, the same content as #752's manifest.
- Staged files match the packet: `8fa0c5ad` `1bd03f15` `e44e7eb8` `24a32146` `3baf029e`; receipt `ee13a811`.
- The reviewers each used their own disjoint scratch.

Commands, all `python3 -I -B`; `A="<replica> <live-inventory> <staged> <receipt>"`:

| Command | Exit | Result |
|---|---|---|
| `exfat_qual_v6.py <run> $A` (baseline copy) | 0 | 21/21; stdout `e1a59bd1…` **byte-identical to #752's** full-replay stdout |
| `exfat_qual_v7.py <run> $A` | 0 | 21/21; stdout `c2af8395…` |
| `exfat_qual_v7_1.py <run> $A` | 0 | 21/21; stdout identical to v7 (`c2af8395…`) |
| `boundary_driver.py v6 … --control-v6` | 0 | **reproduces #752 on v6**: L94 → `error-before-publication`, booked 0, tree not restored; L155 → `KeyboardInterrupt` escapes `batch()` (idx 1 and 6) |
| `boundary_driver.py v7_1 …` | 0 | **91/91 checks** |
| `findings_driver.py v7 …` / `v7_1 …` | 1 / 0 | F-2/F-3 control: 7 FAIL on v7; **19/19 on v7.1** |

**Boundary traces on v7.1** (independent driver; `sys.settrace` line-boundary `KeyboardInterrupt`; the subject is never edited):

| Probe | Fired at | Result |
|---|---|---|
| B1 rename→booking, idx 1 | `publish`: `progress["stage"] = "replaced"` (first line after `os.replace`) | `interrupted-after-publication`, destination `8fa0c5ad…` (new bytes), booked 1 (provisional), **1 × RESTORED**, 50/50, no `.tmp` |
| B2 return→append, idx 1 | `forward_phase`: `booked.append(rec)` | same: booked 1, 1 × RESTORED, 50/50 |
| B1 / B2 at idx 6 (receipt `deployment.json`, destination `ee13a811…`) | as above | booked 6, **6 × RESTORED**, 50/50 |
| N1 unreadable after rename (idx 1, idx 4) | as B1, then the destination is made unreadable | `destination-state-unverifiable`, `UNVERIFIABLE`, others RESTORED, **tree differs in exactly that path (new bytes) — baseline not claimed**; 50/50 only after a separate explicit repair |
| N2 foreign drift at the append boundary | as B2, foreign bytes written | `destination-foreign-drift`, `REFUSED`, foreign bytes preserved |
| N3 pre-existing foreign `.tmp` | — | `error-before-publication` with re-read proof; foreign temp untouched and reported |
| N4 persistently unreadable pre-state | — | `pre-state-unverifiable`; nothing booked or written |
| Exhaustive forward sweep | every forward line boundary (**412**) | all pass. 407 returned with a failure recorded, all RESTORED, 50/50, no `.tmp`, and pre-publication only with proof. The other 5 are lines in `batch()` before any write, which escape with nothing booked and a byte-exact tree. **All 48 post-rename points book the path.** |
| Double-signal sweep | second interrupt at every classifier line (27) after B1 | all restored, 50/50, never pre-publication |

**Original three cases on v7.1 (continuity):**
- (a) Receipt readback `KeyboardInterrupt`: 6 booked, 6 RESTORED, 50/50.
- (b) Pre-state EIO at index 4: `error-before-publication` with proof (destination `ab4343ef…` = pre-state); index 4 never published; 3/3 RESTORED.
- (c) Persistent diagnostic re-read EIO: 6 attempts, 5 RESTORED plus `scripts/_ccf.py` UNVERIFIABLE; the tree differs only in `_ccf.py`, so the baseline is not claimed until a separate repair.

**Reviewers' independent replay (their own scratch and drivers):**
- 21/21 on v7 and v7.1.
- Both windows at idx 1–6 / 1, 3, 6.
- Opcode-level sweeps of `publish`/`forward_phase` (836 points).
- 840/840 double-signal points.
- Genuine `os.kill`/`pthread_kill` SIGINT storm: 2000 trials, 0 escapes, 0 dirty.
- Fail-closed negatives pass.
- Details are in their documents (§3).

## 6. Open residuals (NOT closed; carry to deploy design)

- **R-1 (MAJOR-class residual, unchanged from v6, not a #752 window).** Signals that land in `batch()`'s own frame after the forward phase or during rollback can escape. A few instructions there sit outside any guard: the recovery `for`, `if out["failure"]:`, and the `try:` NOPs. In the escape cases, either a fully successful install has its interrupt propagate with no rollback, or a rollback stops part-way. `evidence["booked"]` and the partial `recovered` list are retained.
  - Reviewer counts: 14 of 262 single-signal points and 65 of 1572 rollback points under opcode models. **0 of 2000 genuine SIGINT trials** hit it.
  - **Required for any real deployer:** block SIGINT/SIGTERM/SIGHUP (`signal.pthread_sigmask`, or a deferring handler) from before the first publish until rollback completes. Keep the verified off-volume 50-file backup and the post-interruption full hash reconciliation.
  - This document's sweeps cover the forward step only (answers F-4).
- **D-1 (MINOR, new in the delta re-check).** Six instructions between `os.open` returning and the `try:` in `publish()` can still leave our own `.tmp` during a *restore*. A retry of that restore then fails with `FileExistsError` until the temp is removed manually.
  - This is fail-closed with no data loss.
  - Rate: 6 of 129 restore-phase points, versus 46 in v7; 1.0–3.3 % of restore-targeted real-signal trials, versus 15.3 % in v7.
  - The forward phase is not affected, because the classifier removes such a temp using `tmp_preexisting`.
  - Fix with the same signal mask, or per-call unique temp names.
- **NITs:** N-1, a restore interrupted after its rename is conservatively reported `RESTORE-ERROR` although the file is restored. N-2, a possible fd leak in the same gap. N-3, a second signal during publish's cleanup is swallowed. N-4, the classifier no longer records `tmp_removed` when `publish()` already removed the temp. N-5, the first reviewer's driver anchors on v7's layout; use the delta re-checker's adapted copy.
- **Not testable in-process:** SIGKILL, kernel crash, power loss, SIGSTOP. These remain under the off-volume backup, post-interruption full hash reconciliation and **explicit owner risk acceptance**.
- **Model limits:** the settrace and opcode models are stricter than real delivery (CPython 3.12 runs signal handlers only at eval-breaker checks). The real-signal storm is statistical, not exhaustive. Results cover one kernel and one exFAT device. The TOCTOU between the pre-state read and `os.replace` is unchanged from v6; it relies on exclusive custody.

## 7. Live Ops (read only) and other workers

- Live `/media/jer/ZX20/CCF-Ops` was never written; the harness only `stat()`s it.
- At 17:05Z: 50 files, board `2b99552c…`, lanes `c17903dd…`, receipt `8c66b290…` (the #752 values).
- At 18:47Z: still 50 files, receipt `8c66b290…` unchanged.
- `board/active.md` → `4fa750eb…` and `lanes.json` → `c1d14719…` were rewritten at **17:28:31Z by another worker** (the #755 H0A lanes added under Manager #730), as recorded independently by both reviewers. Every other live file is byte-identical. No Unity process was running.
- No other worker's scratch or evidence was written. The #752 and #741 trees were read only.

## 8. Durable evidence

`/media/jer/Files/CCF/Verification/Sen1-754-BookingGapFix-20261118/`:
- `author/`: `SHA256SUMS` itself has sha256 `2138483c…728f`.
- `review/`: SHA256SUMS + SHA256SUMS-doc, by the first reviewer.
- `delta/`: SHA256SUMS + COMMANDS.txt, by the delta re-checker.

Key author files:

| File | SHA-256 |
|---|---|
| `exfat_qual_v7_1.py` | `52d4c6aedbecd47c4cf912f8903fe07547cb24c9b743aa9c65f15fdf35c60eab` |
| `exfat_qual_v7.py` | `02606febc26f8bda286387010ca2a16f25dc24b78fe3b993122b9be1e74f004c` |
| `boundary_driver.py` | `3a4aea225a3e6ae34da10c7e3707f2ccd35d5bdd6370cec72e6a438275bd1cdd` |
| `findings_driver.py` | `95036b94b67a1e3f3dd78351f491231fb4c91d9a06f094fef0aa1e6ee354a8f5` |
| `boundary-v7_1.json` (91/91) | `bfe119e28efcffb60483b792ed49811375c56087f9c6d64243cc033924e9fec9` |
| `boundary-control-v6.json` | `c01d4c98e3349f2187645cc714ebcf75424476c16d81dd000651c8ebc0568a0b` |
| `findings-v7_1.json` (19/19) / `findings-v7.json` (control) | `0594b111…fa51` / `cf4e073d…0a59` |
| `run21-v7_1-result.json` / `run21-v7_1.stdout` | `d87af245…b5a` / `c2af8395…a2d3` |
| `run21-v6.stdout` (= #752) | `e1a59bd10b835c0e7d6fd5708b942272b2cbe57f47759719f68568c7820b9c3a` |
| `live-inventory-before.sha256` / `-after-v7_1.sha256` | `f45385bf…3301` / `934ccba7…7fd2` (differs only in the foreign board/lanes rows above) |

Scratch, authoritative during the run: `/media/jer/ZX20/CCF-Temp/Sen1-754-Author-20261118T170536Z/`. It is not cleaned up, so the Manager can inspect it.

## 9. Status

| SOURCE | REVIEWED | STAGED | DEPLOYED | UNITY |
|---|---|---|---|---|
| Scratch harness v7.1 `52d4c6ae…` authored (Opus 5.5); D-091 runtime source **unchanged** | v7 **APPROVED** (independent Sonnet 5.5); v7.1 delta **APPROVED** (new session); same Claude family; R-1 and D-1 OPEN | five deploy paths, merged README and receipt fixture **unchanged**; nothing staged live | **NO** — owner NO LIVE INSTALL stands | **NOT RUN**; #718 HELD |

**Still required before any live install:**
- Manager #731 adjudication of this result.
- The owner's explicit restricted no-install release and sudden-power-loss risk decision.
- A real deployer that closes R-1 (signal mask) and keeps the off-volume backup and hash reconciliation.
- A properly scoped physical custodian/installer under the live board flock.
- The final single Unity guard exclusion.
- Fresh storage, board and 50-file checks.
- A separate Manager [APPROVED TO DEPLOY OPS].

#690, Multi #730 and #652 are unaffected. No Windows. D-091 still has no arbitrary worker time limits.
