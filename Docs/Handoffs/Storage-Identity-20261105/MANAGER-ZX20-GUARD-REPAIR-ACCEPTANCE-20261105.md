# Manager acceptance — ZX20 stable-device guard repair

**Date:** 5 November 2026. **Manager:** Overall Manager, browser/GitHub lane #200.
**Source:** [#290 latest worker handoff](https://github.com/jermur788/CCF/blob/docs/zx20-storage-identity-20261105-v1/Docs/Handoffs/Storage-Identity-20261105/T3-ZX20-STORAGE-GUARD-RECONCILIATION-RESULT.md) (latest repair-deployed section); previous [diagnostic verdict](https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/Storage-Identity-20261105/MANAGER-ZX20-DIAGNOSTIC-DISPOSITION-20261105.md) remains historical and is superseded as to the live guard state.

## Formal Manager verdict

**APPROVED — TARGETED OPERATIONAL REPAIR AND STORAGE GATE FOR #289.** The specifically reported `/dev/sda2` → `/dev/sdc2` enumerator-drift failure is repaired to a defensible, fail-closed persistent by-id identity check. This accepts the reported worker evidence after GitHub read-back; **Manager did not personally execute the Linux tests or inspect the deployed filesystem bytes**. It is **NOT** approval of a full CCF-Ops lane inventory, complete legacy self-test suite, general filesystem health or Unity verification.

The user explicitly authorised the bounded stable-device guard correction after Phase A. The worker now reports a successful durable backup on writable `/media/jer/Files`, byte-identical originals and `backup.sha256`; unchanged unrelated deployment artifacts; installation of exactly four files (lanes.json, scripts/_ccf.py, scripts/lane-status, README.md) under existing guard flock, rechecking no Unity lease/process and using per-file atomic replacement. New storage identity compares underlying block-device numbers via a persistent partition by-id path, with visible UUID uniqueness rejection. The target mount, UUID, filesystem, read/write and minimum free-space checks remain. The worker reports before/after SHA-256 values, eight negative staged tests (duplicate UUID, wrong/missing device, read-only and low-space cases), post-install `lane-status --storage-only` READY/PASS at ~33.7 GiB, `unity-lock status` free and `source environment.sh` PASS. No lease was acquired/released/reset, Unity started, playtest edited, saved-world mutated or Windows build initiated.

### Boundaries and evidence gaps

- Full no-flags `lane-status` lane inventory was stopped while scanning large Unity lanes with `du`; **NOT VERIFIED**. Do not make it a mandatory blocking or potentially costly scan for #289; use narrow storage and clone checks.
- `tests/self-test.py` retains literal historical `/dev/sda2` fixtures; complete shipped self-test coverage is **NOT VERIFIED**. Create or schedule a separate bounded CCF-Ops test-fixture alignment task under an exclusive ops writer; do not silently edit outside the four-file scope or weaken negative checks.
- The transient earlier `/media/jer/Files` ext4 read-only state cleared and a durable write/backup succeeded, but the cause of the transient state is **UNKNOWN**. #291 should be treated as a separate **non-blocking observational health investigation** if it remains open; any repeated read-only state stops new writes.
- GitHub handoff has hashes and reported checks, not independently inspected live source diffs; repo/source-vs-explicit-copy provenance remains important on any future redeployment. Do not claim comprehensive deployment audit.

## Release verdicts

**#289 D-071 context publication: READY FOR FRESH SAFE PREFLIGHT, NOT RUNNING.** The earlier storage identity blocker is discharged by the targeted repair. Original [#289 packet](https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ProjectContext-20261105/CONTEXT-PUBLICATION-D071-PACKET.md) remains authoritative. Local worker must refresh live active-tasks, claim and main, confirm `source environment.sh` and `lane-status --storage-only` independently pass now, check Files rw for durable output/backup as applicable, path ownership, disk space and an unused clean isolated clone before starting. If any fail, mark BLOCKED and stop. When truly starting, mark claim [RUNNING] with actual provider, lane, branch/worktree and starting HEAD. Generate one clean-context SHA (not stale fixed SHA), both composites and all stamped mirrors, verify manifest SHA-256/determinism, publish the handoff and update existing permanent Drive mirrors in place only with verified read-back; refresh platform attachments separately. No Unity lease is required for docs-only work.

**#287 workbench Unity feature qualification: STILL WAITING.** The storage predicate may no longer block a future preflight, but the user's personal playtest has not been explicitly released. A past `unity-lock status free` proves no current machine ownership; it does not authorise Editor start. Keep requirement for an explicit personal-playtest release, fresh ownership/branch/HEAD checks and a newly obtained exclusive machine-wide Unity lease. #284 remains source-reviewed only, not integrated. No Windows build before the user's completed personal playtest **and separate explicit permission**.

**D-071:** existing southern-headland five parallel racks for Scenario One; no spine/herringbone/road-building implementation. Frozen Reference Future v1, Legacy40 and old saves protected.

## Publication and next steps

Manager updates #290 closure semantics, #289 READY, #287 WAITING, #291 non-blocking status, live active-tasks and the Overall-Dev dashboard source. Source changes require actual read-back; automated dashboard ingestion remains **UNVERIFIED** unless there is an independent downstream receipt. New context mirrors and attachments remain historical until #289 completes.

**Next executable task:** local T3 Code GPT-6.1 Sol Medium for #289 in the packet's isolated context lane; copy/paste dispatch and GitHub URL supplied separately. User action: dispatch to the worker; no additional product decision or playtest/build permission is required for the docs-only job.
