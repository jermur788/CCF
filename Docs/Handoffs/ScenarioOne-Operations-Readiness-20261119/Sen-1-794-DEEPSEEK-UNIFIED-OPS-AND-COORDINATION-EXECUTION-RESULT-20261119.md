# Sen-1 #794 — Unified Ops containment + coordination execution result

**Status: PARTIAL / HANDOFF — G0 COMPLETE, A0 ADMISSION COMPLETE, G1 SOURCE BLOCKED, #793 NOT REVIEWABLE (CANDIDATE ABSENT), G2 HELD.**
Date: 2026-11-19. Parent: DeepSeek V4.1 Flash (`opencode-go/deepseek-v4.1-flash`), native OpenCode inside T3 Code, session `ses_e0d29d8ddffeYAgIvSXyJwqkSf`, host `jer-laptop`, user `jer` uid 1000.
Parent claim: [#794 comment 7003565542](https://github.com/jermur788/CCF/issues/794#issuecomment-7003565542). Binding packet: [Sen-1-794 execution packet](https://github.com/jermur788/CCF/blob/docs/sen-1-731-chas-gpt6-manager-20261118-v2/Docs/Handoffs/ScenarioOne-Operations-Readiness-20261119/Sen-1-794-UNIFIED-TG-FORK-OPS-CONTAINMENT-AND-COORDINATION-EXECUTION-PACKET-20261119.md).
Branch: `docs/sen-1-794-unified-ops-parent-20261119-v1`, worktree `/media/jer/Videos/CCF-794-Parent`, base `origin/main` `a69b3e2e90829505c792d95f5a3eba3e7d8d8132`. This file is the parent's sole tracked write.

This is **not** #781 restated: it adds executed, reproducible evidence for Q-H1/Q-M1/Q-M2 on non-live copies, a per-file G1 allowlist with a negative matrix, an exact grant request, and the measured status of the #793 single-writer lane. **No source fix was implemented because G1 was not authorised** — a physical board reservation and an explicit Manager `APPROVED TO START SOURCE` do not exist, and the parent cannot sign either.

## 1. G0 — identity, storage and placement (verified this session)

| Fact | Observed value |
| --- | --- |
| System root | `/dev/sda2` ext4 `d393b49a-24aa-4f88-b8c4-5121b96c240d` |
| Durable storage | `/media/jer/Files` = `/dev/sdb4` ext4 `2283b135-bf37-42d6-921f-2fb861d0a8d4`, 38.60 GB free (> 10 GiB floor) |
| Non-root work storage | `/media/jer/Videos` = `/dev/sdb1` ext4 `60e276b9-4abb-4325-a913-f4c43c14294b`, 438 GB free |
| Active-work SSD | `/media/jer/ZX20` = `/dev/sdc2` exFAT `18D6-896E`; by-id `ata-NetacPortableSSD_AA20260702512G231151-part2 → sdc2`, `st_dev=7` matching the deployed pin |
| Coordination anchor | `/media/jer/Files/CCF-Primary`, branch `ops/workflow-foundation-v1` `a42eaadc…`, **seven** pre-existing dirty/untracked entries — read only, untouched |
| Worktrees created | `/media/jer/Videos/CCF-794-Parent`, `…/CCF-792-Admission`, `…/CCF-794-QA-Review`, `…/CCF-794-QC-Review` — all on Videos, none on the T3 `/home/jer/.t3/worktrees/**` root default |
| Scratch | `/media/jer/Videos/CCF-794-Scratch` (~5.4 GB including four checkouts); durable evidence on Files |

Files and Videos are **separate partitions of one physical disk** (sdb), not independent failure domains; this is stated, not treated as redundancy. The deployed Ops is *not* affected by the stale `/dev/sda2` pin — the live `lanes.json` uses the by-id path and passes.

**Disclosed deviation (no waiver claimed).** Harness-internal state remains root-backed and cannot be relocated without a prohibited global `HOME`/XDG migration: `/home/jer/.local/share/opencode/opencode.db` (5.09 GB, pre-existing), `/home/jer/.local/share/opencode/log` (45 MB), `/home/jer/.local/share/opencode/shell/**`, `/home/jer/.t3/userdata` (5.5 GB). Observed root free-space delta for this run: ~18 MB. **Zero CCF work product — checkout, scratch, synthetic copy, test, evidence, log — was written to the OS root.** The #785 harness-log exception was not invoked. This residual is an owner decision, not silently accepted.

**Two further placement defects in this run, disclosed in full:**
1. The parent harness used `/media/jer/Videos` (a real ext4 mount, non-root) as its synthetic `ROOT`, so its synthetic state was created at `/media/jer/Videos/CCF-Ops/state/unity` — outside `CCF-794-Scratch`, at the partition root. The harness also `rm -rf`'d that path on each run. It contained only synthetic records (no live data). Its history was archived to `…/parent/synthetic-state-history.jsonl` (sha256 `be7aee60…`) and the directory was then removed; `/media/jer/Videos/CCF-Ops` no longer exists. No user file was touched.
2. The QA reviewer self-reported one out-of-bounds write: a session memory note under `/home/jer/.claude/…/memory/` which it deleted immediately, having additionally rewritten that directory's `MEMORY.md` in place. Nothing of the reviewer's remains under `/home`. This is recorded as a reviewer deviation against the no-root rule.

## 2. Track A — A0 containment-first admission (executed)

### 2.1 What was inspected (read-only)
Deployed `/media/jer/ZX20/CCF-Ops` (`scripts/_ccf.py` `ab4343ef…`, `scripts/unity-lock` `54cf0a95…`, `tests/self-test.py` `2a12a19d…`, `lanes.json` `aa3d4658…`, `deployment.json` `8c66b290…`) and the admitted #652 source `/media/jer/Files/CCF/Operations/CCF-OpsReadiness-652-20261116` (clean, `ops/sen-1-652-unity-lease-readiness-20261116-v1`, HEAD `4c7acc051e262c3fe301049b51ee7fc48a9f99a2`; `_ccf.py` `24a32146…`, `unity-lock` `8fa0c5ad…`, `self-test.py` `1bd03f15…`). No file under ZX20, the source checkout, the board, the ledger or `state/unity` was written.

### 2.2 Parent independent harness — 28 scripted assertions, all passing (not 28 production tests)
Script and full output: `/media/jer/Files/CCF/Verification/Sen1-794-Ops-Containment-Admission-20261119/parent/{harness.sh,harness-output.log}` (`harness.sh` sha256 `088ee016…`; log `1d017f9f…`). Method: two copies of the scripts (source bytes and deployed bytes) in unrelated directories with **only** the `ROOT` and `FILES_ROOT` constants substituted — the diff is captured in the log; they are *not* byte-identical copies. A synthetic Git store/lane/config on a real ext4 mount and a synthetic state root were used; 28 assertions check expected exit codes and file-state predicates. The first full run had 2 failing assertions caused by harness bugs; the harness was fixed and the whole run re-executed, and the archived log is the final 28-pass run (not a hand-edited result). The live state was hashed before and after: `state/unity/history.jsonl` sha256 `dfb50644…` **identical**, no live `lock.json` present, state dir mtime unchanged (2026-11-15). No live lock command was run.

Findings, each reproduced with exit codes:

1. **Q-H1 confirmed (HIGH).** `unity-lock`'s state is `DEFAULT = ROOT/'CCF-Ops/state/unity'` — a fixed machine-wide path, **independent of which copy executes and of `--config`**. `--test-state-dir` is optional for `release`. A copy in an unrelated directory acquired, force-released and ordinarily released that shared state (all exit 0). Unmodified deployed and source bytes loaded read-only report `ROOT=/media/jer/ZX20`, `DEFAULT=/media/jer/ZX20/CCF-Ops/state/unity`.
2. **The risk is fleet-wide, not a stray copy (HIGH, new).** A read-only `config_read` audit of **every** `unity-lock` copy with a sibling `lanes.json` on the machine found **51 copies, 47 admitted, and 45 whose `DEFAULT` resolves to the live `/media/jer/ZX20/CCF-Ops/state/unity`** — i.e. **44 non-deployed copies can act on live lock state today** (2 on Files: the admitted source checkout `/media/jer/Files/CCF/Operations/CCF-OpsReadiness-652-20261116/CCF-Ops` and `…/CCF-OpsLeaseFix-20261115`; 42 historical copies under `/media/jer/ZX20/CCF-Temp/**`). Only 4 were rejected: 2 by the stale `/dev/sda2` pin (the `CCF-Primary/CCF-Ops` copy and a G4 backup — the pin is protective, exactly as the packet said) and 2 of this parent's own synthetic copies. Evidence: `…/parent/live-capable-copies.txt` (sha256 `e466d275…`), produced with module import + `config_read` only — no state written, no lock command run. **Stop-gap rule until containment is deployed: never run `unity-lock`, not even `status`, and never source `environment.sh`, from any non-deployed copy without an explicit `--test-state-dir` under `ZX20/CCF-Temp`.**
3. **Q-M2 confirmed (HIGH).** `--force` needs only `--owner`, `--reason` and any non-placeholder text in `--manager-approval`; nothing verifies a Manager identity, signature, nonce or external reference. It also does not check the live Editor/PID state and does not require the exact lock ID, so it can evict healthy custody. A fabricated string was accepted and audited verbatim in `history.jsonl` on a healthy lease.
4. **Upgrade/rollback hazard (HIGH, new).** Deployed bytes cannot parse a `--no-expiry` record (`eta:null`), classify an **unexpired** lease as `malformed`, and force-evict it. The 2026-11-16 source adds no-expiry custody but deployed tooling would treat it as corrupt — migration must be explicit, never a silent rollback to old readers.
5. **Ordinary release is not owner-authenticated (MEDIUM).** `owner`/`task_id`/`lock_id` are readable from a 0644 `lock.json`; a foreign same-uid copy reproduced all three and released (exit 0).
6. **Q-M1 confirmed (MEDIUM, scope-limited).** `on_durable_ccf()` is a resolved-prefix test only. A path admitted as “durable CCF” was accepted while physically on Videos (`st_dev 2065`) rather than Files (`st_dev 2068`) — no fstype/UUID/device/mount check. This demonstrates missing device identity; it is **not** a physical unmount/substitution test, which remains **NOT TESTED**. `environment.sh` is also a Files-path caller and is absent from the earlier caller list.
7. **`status` is a write (LOW, documented).** On an existing state directory, `status` creates/opens `guard` with `a+` and takes `flock`. On an absent state dir it returns before creating anything.
8. **Stale-fixture correction (evidence-based).** The admitted source contains **no** `/dev/sda2` literal: `storage_check` defaults to the by-id path and `self-test.py` uses the observed device. The literal survives in the **deployed** `tests/self-test.py` (lines 70, 78, 79, 110) and in historical `Docs`/fixtures. Because deployed files are not G1-editable, A1d is **documentation/receipt hygiene only** — it is not a source fix and must not be presented as one.

### 2.3 #792 author package (independent author, different provider)
Claim [#792 comment 7003602473](https://github.com/jermur788/CCF/issues/792#issuecomment-7003602473); original result [#792 comment 7003706186](https://github.com/jermur788/CCF/issues/792#issuecomment-7003706186); **corrected document** commit `40d1d39a283ea4e9eec7dc5dc6979eae41dc96c8` ([permalink](https://github.com/jermur788/CCF/blob/40d1d39a283ea4e9eec7dc5dc6979eae41dc96c8/Docs/Handoffs/ScenarioOne-Operations-Readiness-20261119/Sen-1-792-CONTAINMENT-FIRST-SOURCE-ADMISSION-AND-TEST-RESULT-20261119.md), [correction comment](https://github.com/jermur788/CCF/issues/792#issuecomment-7004054096)); original commit `23b60fcd…`. Author: OpenAI `gpt-6.1-sol` via Codex, branch `docs/sen-1-792-ops-g1-admission-20261119-v1`, worktree `/media/jer/Videos/CCF-792-Admission`, sole tracked file = that document. Its own synthetic harness reported **22 scripted assertions whose expected exit codes and state predicates were checked**, all passing, with a `findmnt` PATH projection disclosed as scaffolding; the full production suite was **NOT RUN**. Receipt: `/media/jer/Files/CCF/Verification/Sen1-794-Ops-Containment-Admission-20261119/author/`.

The two independent harnesses agree on every substantive finding. Where they differ (the author projects `findmnt` output; the parent uses a real mount), the parent's method is stronger and the conclusion is unchanged.

## 3. Track A — G1 permission status (why no source fix exists)

Physical board row `CCF-652-OPS-READINESS` (read-only, 08:13Z) reads `READY | RESERVED ONLY / NOT RUNNING / SOURCE BLOCKED`, prospective writer `OpenAI/Codex gpt-6.1-sol medium`, session `9bcdfde6…`, **no live lease**. No Manager `APPROVED TO START SOURCE` exists on #652 or #731. Therefore A1 was not started, no candidate branch/worktree was created, and no source byte changed.

**One agreed grant text** (the earlier [#652 comment 7003714806](https://github.com/jermur788/CCF/issues/652#issuecomment-7003714806) is **withdrawn** and superseded after independent review): [#652 comment 7004086131](https://github.com/jermur788/CCF/issues/652#issuecomment-7004086131). It requests: a real physical reservation/lease on the board row to one named writer (or an explicit statement that custody stays with session `9bcdfde6…`); the verbatim `APPROVED TO START SOURCE` line; branch `ops/sen-1-794-containment-first-20261119-v1`, worktree `/media/jer/Videos/CCF-794-Source`, base `4c7acc051e262c3fe301049b51ee7fc48a9f99a2`; the allowlist below with two explicit ceiling amendments; the immediate stop-gap rule; the threat-model acknowledgement; Q-M2 left open; no deploy, no live lock/board/state mutation. The board row's recorded HEAD `a9f56d63` is stale against `4c7acc05` and needs a dated custodian addendum.

| Order | File | Bounded change |
| --- | --- | --- |
| A1a | `CCF-Ops/scripts/_ccf.py`, `CCF-Ops/scripts/unity-lock` | Admit exactly one deployed Ops root/identity for lock-state operations; non-admitted copies fail closed or require explicit scratch; preserve `--test-state-dir` isolation. **Amendment required:** the board covers `_ccf.py` only for distinct earlier defects. |
| A1b | `CCF-Ops/scripts/unity-lock` (release/force) | Separate `--force` from ordinary release; require a verifiable approval artefact; no replay; no occupied-Editor/PID eviction; refuse until the Q-M2 mechanism is decided. |
| A1c | `CCF-Ops/scripts/_ccf.py`, `CCF-Ops/scripts/lane-status` | Guard Files CCF admission by mountpoint + fstype + UUID + device (and uniqueness), not prefix; keep ZX20 pins unweakened. **Amendment required:** `lane-status` is likewise covered only for earlier defects. |
| A1d | *no source edit* | The `/dev/sda2` literal is in the **deployed** `tests/self-test.py` and historical docs/fixtures, not the admitted source; a deployed file is not G1-editable. Record it as a deployment/receipt reconciliation item, not a source change. |
| A1e | `CCF-Ops/README.md`, `CCF-Ops/roles/unity-operator.md` | Truthful commands, no-expiry/migration semantics, status-write disclosure, source-vs-deployed delta. |
| — | **Excluded** | `lanes.json`, `deployment.json`, `board/**`, `ledger/**`, `state/**`, installed ZX20 files, `active-tasks.md`, `roles/execution-manager.md`, `tests/fixtures/**`, any Unity/game/save file. |

**Q-M2 is an open owner decision, and the earlier recommendation is withdrawn.** The suggestion of “GitHub approval comment + signature” cannot work: every actor posts to this repository as the same GitHub account (`jermur788`), so author identity is not verifiable from a comment. Real options: (i) Manager-owned signed one-use artefact binding lock ID, action, actor, nonce and expiry, with a replay store; (ii) a privileged local broker using OS identity and an access-controlled queue; (iii) an out-of-band signed file with short expiry, logged to `history.jsonl`. **No recommendation is offered by this parent; `--force` must refuse with a clear “approval mechanism not implemented” error until the owner decides.**

**Honest limit of any A1a design.** With a single Linux uid and writable copies, binding to “the installed executable/identity” prevents *accidental* cross-copy execution; it is not a hostile same-uid security boundary. The docs and the review must say so; the real control for hostile users is OS permissions on `/media/jer/ZX20/CCF-Ops/state/unity` (see §6 decision D4).

## 4. Track B — #793 coordination lane (observed, not touched)

- Branch `docs/sen-1-793-live-main-registration-sync-20261119-v1` tip = `a69b3e2e90829505c792d95f5a3eba3e7d8d8132` = current `main` tip; **no candidate commit, no diff versus main** (checked twice, 08:10Z and 08:22Z, via `gh api` and `git fetch` of that branch only).
- Therefore **no frozen candidate exists** and no review of one can honestly be issued. The parent did not edit, checkout for write, reset, merge or promote `active-tasks.md`, created no alternate #793 branch, and did not touch the rejected local 93-line compaction.
- **Rejected compaction measured read-only (why it stays rejected):** `main` is 981 lines / 779,213 bytes / sha256 `cd38c93a…` with 380 distinct issue references and 88 dated entries in the 17–19 November window; the dirty local file is 93 lines / 15,555 bytes with **0** issue references and **1** entry in that window (its own diff against its stale base is 51 insertions / 165 deletions). It is not a lossless compaction and must never be promoted.
- `main` is **NOT SYNCED**; Manager #731 remains the sole writer; promotion remains Manager/integrator-only.
- Independent baseline pre-audit (nonauthor, different provider) is recorded in §5 and gives the exact acceptance checklist for the future candidate.

## 5. Independent reviews

| Role | Actor | Output | Verdict |
| --- | --- | --- | --- |
| QA — admission package (not a source candidate) | Anthropic `claude-opus-5-5` | [Sen-1-794-QA-INDEPENDENT-OPS-ADMISSION-REVIEW-20261119.md](https://github.com/jermur788/CCF/blob/f3f8c9ed094b32566b68d7cdf1845f9a81b55ff0/Docs/Handoffs/ScenarioOne-Operations-Readiness-20261119/Sen-1-794-QA-INDEPENDENT-OPS-ADMISSION-REVIEW-20261119.md) (commit `f3f8c9ed…`, branch `review/sen-1-794-qa-ops-admission-20261119-v1`) | **NEEDS CHANGES** — 3 HIGH, 6 MEDIUM, 4 LOW (admission package) |
| QC — coordination baseline / pending state | Anthropic `claude-sonnet-5-5` | [Sen-1-794-QC-COORDINATION-BASELINE-PREAUDIT-20261119.md](https://github.com/jermur788/CCF/blob/94d07fd3a9e21efa6146b3450d146e6ec0293809/Docs/Handoffs/ScenarioOne-Operations-Readiness-20261119/Sen-1-794-QC-COORDINATION-BASELINE-PREAUDIT-20261119.md) (commit `94d07fd3…`, branch `review/sen-1-794-qc-coordination-preauth-20261119-v1`) | **NOT REVIEWABLE — CANDIDATE ABSENT** (baseline pinned) |

**QA claim:** [#794 comment 7003768656](https://github.com/jermur788/CCF/issues/794#issuecomment-7003768656); result [#794 comment 7003955512](https://github.com/jermur788/CCF/issues/794#issuecomment-7003955512). The reviewer re-derived the state constant from both byte sets, ran its own 7/7 reproduction (`…/qa/repro`: foreign force-release with `--manager-approval "ok"`, ordinary release from copied IDs, different-device Files admission), and confirmed live state byte-identical before and after (`dfb50644…`).

QA HIGH findings: (a) the #792 document and the parent grant request disagreed on branch, worktree, base, writer, allowlist and phase labels; (b) the allowlist exceeded the board ceiling (`roles/execution-manager.md`, `tests/fixtures/**`, `Docs/**` wildcard; `_ccf.py` and `lane-status` only covered for earlier defects); (c) the live-capable copies were not listed and there was no stop-gap rule. MEDIUM: the grant text omitted the same-uid threat model; a GitHub-comment approval check cannot verify identity because every actor posts as `jermur788`; “installed identity” must mean location + device, not a file hash; A1d targeted a `/dev/sda2` literal absent from the source tests (it is in the deployed `self-test.py`); no base commit stated; stale board HEAD; and the parent harness wrote outside its scratch with `rm -rf`. LOW: assertion-count and “byte-copies” wording, weaker-than-stated Files evidence, missing `environment.sh` caller, and unstated force-path gaps.

**Corrections applied in response:** the parent grant text is withdrawn and replaced by [#652 comment 7004086131](https://github.com/jermur788/CCF/issues/652#issuecomment-7004086131); the #792 document was corrected by its own author in commit `40d1d39a283ea4e9eec7dc5dc6979eae41dc96c8` ([correction comment](https://github.com/jermur788/CCF/issues/792#issuecomment-7004054096)); this parent document corrects its own assertion wording, copy wording, Files scope, force-path gaps and A1d classification, adds the live-capable inventory, and discloses both placement defects. Q-M2 is left **open with no recommendation**.

QC claim: [#793 comment 7003807590](https://github.com/jermur788/CCF/issues/793#issuecomment-7003807590); result [#793 comment 7003909184](https://github.com/jermur788/CCF/issues/793#issuecomment-7003909184). It re-checked the #793 branch twice (08:24:56Z and 08:29:53Z): tip `a69b3e2e`, 0 commits ahead of main, and no other remote branch touches `active-tasks.md` (all 50 branches checked). It confirmed the Manager #731 RUNNING claim (comment 7003200408, 07:55:22Z) states a base matching current main, while the #793 issue title/body still say `READY` — a documentation lag, not a claim of completion.

QC independent findings (verified by the parent where marked):
- **MISSING from main's `active-tasks.md`:** #781, #785, #786, #787, #788, #789 (Multi, informational only), #792, #793, #794. Parent-verified: the file contains **zero** occurrences of any of those numbers, so the current Sen-1 workstream is entirely unregistered.
- **STALE:** #777 shown READY but closed HANDED-OFF/PARTIAL; #778 shown READY but blocked; #652 has no record of the containment-first G1 route, #792 or the pending grant request. Beyond the packet's list: #773, #774, #767, #775 show READY but are closed.
- **Conflicting sole-writer statements:** one line names #200 as sole writer, another names #425; neither names #731. Needs a dated Manager addendum, never a silent rewrite.
- **Rejected local compaction (deepened):** different lineage (62 ahead / 1,849 behind main), 0 vs 379 issue references, drops 28 of 33 task rows, and silently changes owner/model in retained rows (`deepseek→NONE` ×2, `gpt-6.1-terra→gpt-6.1-sol`). It also asserts a ZX20 mount of `/dev/sdd2` where the observed device is `/dev/sdc2`. QC's checker fails it with 24 findings.
- **Promotion blockers (8):** no frozen candidate commit; no #793 result file; no nonauthor review of a candidate; no recorded compare-and-swap promotion grant; the rejected local file must never be the source; three conflicting sole-writer statements need a dated Manager addendum; #793/#786/#787 titles lag their claims; main stays NOT SYNCED until a real readback compare.
- **#794 non-write check:** main's file blob unchanged, no remote branch touches it, and the three sibling worktrees hold the identical hash (point-in-time).

Neither reviewer authored the reviewed material; neither is GPT-6; neither edited `active-tasks.md` or any Ops file.

## 6. Consolidated verdict and exact owner actions

**Status ladder (do not collapse):** DIAGNOSED — yes. **A0 admission — complete.** CANDIDATE IMPLEMENTED — no (not authorised). SOURCE INDEPENDENTLY REVIEWED — no (no candidate). GITHUB REGISTRATION SYNCED — no (#793 candidate absent). INSTALLED — no. VERIFIED IN DEPLOYED OPS — no.

**Owner action needed (exactly these, in order):**
0. **Apply the stop-gap now, no grant required:** do not run `unity-lock` (not even `status`) and do not source `environment.sh` from any non-deployed copy without an explicit `--test-state-dir`; never pass `--force`. 44 non-deployed copies are live-capable today (§2.2 finding 2).
1. **Grant G1 source** on [#652 comment 7004086131](https://github.com/jermur788/CCF/issues/652#issuecomment-7004086131): physical reservation/lease to one named writer (or an explicit statement that custody stays with `9bcdfde6…`), the two ceiling amendments for `_ccf.py`/`lane-status` plus the result path, and the verbatim `APPROVED TO START SOURCE` line. Nothing else is required to start A1a/A1c.
2. **Decide the Q-M2 approval mechanism** — left open deliberately; the parent offers no recommendation because every actor posts as the same GitHub account. A1b stops at that fork until decided and `--force` must refuse.
3. **Tell Manager #731 to publish and freeze** the #793 candidate on the existing branch; then the QC review runs against the frozen commit and promotion is compare-and-swap by the Manager. Main's coordination file currently registers none of #781, #785–#789, #792–#794.
4. **Decide D4 (optional but decisive):** whether to harden `/media/jer/ZX20/CCF-Ops/state/unity` with OS permissions (for example `0700` plus an authorised operator group) so hostile same-uid access is actually bounded. This is machine configuration, outside any source patch.
5. **Accept or reject the harness-internal root storage residual** (§1), including the reviewer's transient `/home` write.

**Remaining blockers, precise:** no #652 physical lease; no Manager source-start permit; no Q-M2 decision; #793 candidate absent; G2 install, forced release, live lock/board mutation, Unity and main promotion all still separately held.

## 7. Preservation and non-effects

- Live deployed Ops bytes unchanged (hashes in §2.1); live `state/unity` byte-identical before/after (`history.jsonl` `dfb50644…`), no `lock.json`; no install, deploy, forced release, Editor launch, Unity, Windows build or main merge.
- `/media/jer/Files/CCF-Primary` seven dirty/untracked entries untouched; all four #781 worktrees and every other registered worktree preserved; nothing deleted except this task's own synthetic state directory, disclosed in §1.
- Concurrent third-party physical change **observed and attributed**: at 08:13:22Z `lanes.json` gained lane `Multi789Y2Src` (25→26), hash `c1d14719…`→`aa3d4658…`, matching board row `CCF-789-MULTI-YEAR2-10-SOURCE` (Multi custodian, GPT-6.1 Sol session `01a1f2dd…`). This is Multi #789's lawful reservation, not a #794 write; it explains the identity discrepancy flagged by #792 and must be reconciled in any later deployment receipt.
- #785 harness-log exception not invoked; #789/Multi not touched; #690 save writer untouched; frozen worlds and Reference Future v1 untouched.

## 8. Evidence index

| Evidence | Path / link | Hash |
| --- | --- | --- |
| Parent harness script | `/media/jer/Files/CCF/Verification/Sen1-794-Ops-Containment-Admission-20261119/parent/harness.sh` | `088ee016…` |
| Parent harness output (28 scripted assertions, all pass) | `…/parent/harness-output.log` | `1d017f9f…` |
| Storage + identity inventory | `…/parent/storage-identity.txt` | — |
| Ops + live-state witness | `…/parent/ops-and-live-witness.txt` | `history.jsonl` `dfb50644…` |
| Live-capable copy inventory (51 audited, 44 non-deployed live-capable) | `…/parent/live-capable-copies.txt` | `e466d275…` |
| Synthetic state history (archived before removal of the stray folder) | `…/parent/synthetic-state-history.jsonl` | `be7aee60…` |
| #792 author evidence receipt | `…/author/{harness.py,commands.json,receipt.json,live-before.json,live-after.json}` | — |
| #792 document (corrected) | commit `40d1d39a…` (original `23b60fcd…`) | — |
| Grant request (supersedes 7003714806) | #652 comment 7004086131 | — |

Dashboard/Drive mirror ingestion: **UNVERIFIED**.
