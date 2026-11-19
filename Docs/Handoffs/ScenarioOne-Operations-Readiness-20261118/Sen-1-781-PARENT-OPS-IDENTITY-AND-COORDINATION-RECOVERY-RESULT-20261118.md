# Sen-1 #781 — Parent consolidated result: Ops lane identity and coordination recovery (G0)

**Status:** G0 diagnosis and reconciliation complete; independent review returned **NEEDS CHANGES** and one bounded delta recheck is outstanding (section 7.4). Read-only and documentary only. G1 (isolated candidate source correction) and G2 (live physical mutation) are **NOT GRANTED** and remain HELD.
**Parent:** T3 Code / OpenCode, `opencode-go/deepseek-v4.1-flash` (variant `high`), thread `26ad9ad0-c04f-4125-b4d4-ad79f3b59220`, host `jer-laptop`.
**Packet:** [Sen-1 #781 TG Fork packet](https://github.com/jermur788/CCF/blob/docs/sen-1-731-chas-gpt6-manager-20261118-v2/Docs/Handoffs/ScenarioOne-Operations-Readiness-20261118/Sen-1-781-TG-FORK-OPS-LANE-IDENTITY-AND-COORDINATION-RECOVERY-PACKET-20261118.md).
**Parent claim:** [#781 comment 6996330217](https://github.com/jermur788/CCF/issues/781#issuecomment-6996330217). **Dispatch registration:** [#781 comment 6996384005](https://github.com/jermur788/CCF/issues/781#issuecomment-6996384005). **Capacity reroute record:** [#781 comment 6996404168](https://github.com/jermur788/CCF/issues/781#issuecomment-6996404168).
**Base:** `origin/main` = `a69b3e2e90829505c792d95f5a3eba3e7d8d8132`.
**Branch:** `docs/sen-1-781-parent-ops-recovery-20261118-v1`. **Worktree:** `/media/jer/Files/CCF/Operations/CCF-781-Parent-20261118`.

> This document is a G0 result. It is **not** an installed or runtime verification PASS, not a source approval, and not a permit. Nothing in `CCF-Ops` was installed, deployed, repaired or reconfigured by this programme. Dashboard and Drive mirrors are **NOT VERIFIED** here.

---

## 1. What was asked, and what this cycle actually did

The packet asked for one substantial, identity-safe outcome: restore an independently evidenced way to see which real CCF source lanes and which exclusive Unity lock are available, without breaking physical custody or overwriting user/worker edits. G0 is the executable part: A1 read-only storage/deployed-command diagnosis, A2 November coordination and local-compaction reconciliation, Q independent nonauthor review, and this consolidated result. G1 and G2 were explicitly withheld.

Verified scope of this cycle:

- no change to installed `CCF-Ops`, `lanes.json`, `board/active.md`, `deployment.json` or `state/`;
- no change to `Docs/Project/coordination/active-tasks.md`, its archive, root `AGENTS.md`, `StorageArchitecture.md` or the three untracked November notes;
- no Unity Editor, Player, build, lock acquisition, force release or stale-agent eviction;
- no game source, no main merge, no `#690` save, no `#718` scene, no Multi `#730`/`#789` work, no GPT-6.2 use.

## 2. Identity and routing record (actual, read from runtime)

| Role | Harness / provider | Model / effort | T3 thread (suffix) | Git result | Only writable path |
|---|---|---|---|---|---|
| Parent | T3 Code / OpenCode (`opencode`) | `opencode-go/deepseek-v4.1-flash`, `high` | `26ad9ad0-c04f-4125-b4d4-ad79f3b59220` (this session) | `docs/sen-1-781-parent-ops-recovery-20261118-v1` | `Sen-1-781-PARENT-…-RESULT-20261118.md` |
| A1 | T3 Code / Codex (`codex`) | `gpt-6.1-sol`, `high` (runtime-reported) | `…5fc90470-52cc-4ad3-a14e-053c86d3e37e`; codex thread `01a1f156-cdbe-7d11-bc51-125cff22b786` | `docs/sen-1-781-a1-physical-identity-20261118-v1` @ `2b14d860` | `Sen-1-781-A1-…-ROOT-CAUSE-20261118.md` |
| A2 | T3 Code / Claude (`claudeAgent_claude_b_pro`, reroute) | `claude-sonnet-5-5` (runtime-observed) | `…41d0405f-3914-48d8-a908-712c73620150` | `docs/sen-1-781-a2-coordination-20261118-v1` @ `2b08d661` | `Sen-1-781-A2-…-RECONCILIATION-20261118.md` |
| Q | T3 Code / Claude (`claudeAgent_claude_b_pro`, reroute) | `claude-opus-5-5` | `…d8dbb90e-19bd-4ae9-876b-060e3a4e3e5e` | `review/sen-1-781-q-ops-admission-20261118-v1` | `Sen-1-781-Q-…-REVIEW-20261118.md` |

Recorded deviations, not hidden:

- A2's first dispatch on the `claudeAgent` instance failed with a provider rate limit before doing any work; A2 was rerouted to `claudeAgent_claude_b_pro` with the same model and brief, and recorded the reroute in its own document. Q's first dispatch on `claudeAgent` later failed the same way (5-hour limit, resets 03:30 Europe/Dublin) and Q was rerouted to `claudeAgent_claude_b_pro` with `claude-opus-5-5`. **A2 and Q therefore share the provider instance `claudeAgent_claude_b_pro` and the Anthropic model family.** Q's independence rests on being a different session and a different model (`claude-opus-5-5` vs `claude-sonnet-5-5`), on having authored neither A1 nor A2, and on being given the reviewer role in advance; it does **not** rest on a different provider instance. The earlier claim comment that described Q as "separate from the A2 instance" is superseded by this statement. No gate is satisfied by a reroute.
- Children's first dispatches used the harness's isolated per-task checkout, which landed on the root filesystem. A1 refused to write and A2 was cancelled before writing; both lanes were re-prepared on Files at the packet's candidate paths and re-dispatched behind a mandatory placement gate. See 3.1. This is recorded as an operational defect of the harness default, not as a task failure.
- Token, cost and quota observations are **UNKNOWN**: the harness exposes no per-child token or cost accounting to this parent.

## 3. Storage placement compliance (owner no-root directive)

Owner directive of 2026-11-18: no new development on the system root filesystem; protect ≥10 GiB free on Files; do not use `/home/jer`, `/tmp` or `/var/tmp` as silent scratch.

| Mount | Device | Filesystem | UUID | Free at start | Used by this cycle |
|---|---|---|---|---|---|
| `/media/jer/ZX20` | `/dev/sdc2` | exfat | `18D6-896E` | 343 GiB | read-only inspection only |
| `/media/jer/Files` | `/dev/sdb4` | ext4 | `2283b135-bf37-42d6-921f-2fb861d0a8d4` | 40 GiB (92 %) | parent worktree and child worktrees (text/docs only) |
| `/media/jer/Videos` | `/dev/sdb1` | ext4 | `60e276b9-4abb-4325-a913-f4c43c14294b` | 413 GiB | not used; no CCF lane invented here |
| `/` | `/dev/sda2` | ext4 | `d393b49a-24aa-4f88-b8c4-5121b96c240d` | 35 GiB | no CCF worktree, source or output created |

Scratch for GitHub comment bodies was written to `/dev/shm` (tmpfs, not the root disk). Files free space remained above the 10 GiB floor throughout; verified at the end of the cycle.

### 3.1 Placement incident: the harness default violates the owner directive

The first A1 dispatch used T3's `isolation: "worktree"`, which prepared its checkout at `/home/jer/.t3/worktrees/CCF-Primary/t3-fc2f7127-…`. `findmnt -T .` there reported `/dev/sda2 ext4 … d393b49a-…`, i.e. **the root filesystem**. A1 stopped at its own storage gate and wrote nothing; the second A2 attempt was cancelled for the same reason before it did any work.

This is a systemic finding, not a one-off: T3 per-task checkouts accumulate under `/home/jer/.t3/worktrees/` on this host, all on root. Q counted **23** there at 01:29Z; the parent's earlier "25" was not timestamped and is withdrawn. Any CCF delegated work that accepts the harness default silently breaks the owner's no-root placement rule.

Remediation performed here: the three checkouts created by these dispatches were verified clean at `a42eaadc` with no commits and no user content, then removed with `git worktree remove` (root free space 31 GB → 33 GB); the packet's own candidate worktrees were prepared on Files at the required base and the lanes were re-dispatched behind a mandatory placement gate. No other T3 checkout was touched. Q independently counted 23 checkouts under `/home/jer/.t3/worktrees/CCF-Primary` at 01:29Z; the parent's earlier "25" was not timestamped and is withdrawn in favour of Q's timestamped count.

### 3.2 Prepared lanes (verified before dispatch)

| Actor | Worktree | Branch | Verified |
|---|---|---|---|
| Parent | `/media/jer/Files/CCF/Operations/CCF-781-Parent-20261118` | `docs/sen-1-781-parent-ops-recovery-20261118-v1` | ext4 `/dev/sdb4`, HEAD `a69b3e2e` |
| A1 | `/media/jer/Files/CCF/Operations/CCF-781-A1-PhysicalIdentity-20261118` | `docs/sen-1-781-a1-physical-identity-20261118-v1` | ext4 `/dev/sdb4`, clean, HEAD `a69b3e2e` |
| A2 | `/media/jer/Files/CCF/Operations/CCF-781-A2-Coordination-20261118` | `docs/sen-1-781-a2-coordination-20261118-v1` | ext4 `/dev/sdb4`, clean, HEAD `a69b3e2e` |
| Q | `/media/jer/Files/CCF/Operations/CCF-781-Q-Review-20261118` (prepared at Q dispatch) | `review/sen-1-781-q-ops-admission-20261118-v1` | as above |

## 4. Parent-level verified evidence (independent of A1/A2)

These were observed directly by the parent and are used to cross-check, not to replace, the children's findings.

### 4.1 The kernel device name has changed; no UUID or label change was found

`findmnt` / `lsblk` / `/dev/disk/by-uuid` at 2026-11-19T00:5xZ:

- `/media/jer/ZX20` ← `/dev/sdc2`, exfat, UUID `18D6-896E`, label `ZX20`;
- `/` ← `/dev/sda2`, ext4, UUID `d393b49a-24aa-4f88-b8c4-5121b96c240d`;
- `/dev/disk/by-id/ata-NetacPortableSSD_AA20260702512G231151-part2` → `/dev/sdc2`.

Historical kernel names for the same ZX20 filesystem include `/dev/sda2` (the deployed/storage-architecture record) and `/dev/sdd2` (reported by #778, not independently re-verified here). No evidence of a filesystem UUID or label change was found; the device number moved, the identity did not. A1 reached the same conclusion and states that absence of a historical relabel event is not positively proven.

Consequence: the historical pin `/dev/sda2` now names **the operating system disk**, not ZX20. A guard that compared only the kernel path would be worse than useless. The deployed ZX20 identity check avoids that specific failure because it compares block-device numbers through a persistent by-id path plus UUID, fstype, mount target and unique-visibility. It does **not** pin the Files mount at all (Q-M1, section 4.6), and it does not stop a non-deployed copy of the scripts from acting on live state (Q-H1, section 4.5).

### 4.2 The deployed storage-only check passes; the lock readback is a snapshot from a write-capable command

- `python3 /media/jer/ZX20/CCF-Ops/scripts/lane-status --storage-only` → `READY / PASS | device=/dev/sdc2 rw=True | free=367830237184 bytes (342.57 GiB) threshold=16106127360 bytes (15.00 GiB)`, exit 0. This is a genuine, reproducible result and A1 independently reproduced it.
- `unity-lock status` reported `free` with no Unity Editor PIDs. **Disclosure:** the parent ran the *unmodified* command once. That command is not strictly read-only — with the state directory present it enters `guarded()`, which calls `mkdir(exist_ok=True)` and opens the guard with `a+`. A1 therefore refused to run it unmodified and used a documented write-denying in-memory wrapper; its controlled readback agrees (`status: free`, no Editor PIDs). Q additionally observed the guard file still at 0 bytes with mtime 2026-10-16 and `history.jsonl` unchanged since 2026-11-15, which is *consistent with* no change but is not a before/after proof for the parent's run. The parent does not present the lock readback as a clean read-only PASS.
- The parent also ran one **full** `lane-status` and viewed it through `head`. No complete transcript, exit code or elapsed time was retained, so **completion is parent-attested and unverified**. A1's bounded run timed out at 55 s without a report and was not repeated. The lane count **25** is the `lanes.json` entry count, verified by A1 and by Q — not by the parent's truncated output. The parent **claims** its earlier "20 lanes" was a truncation artefact of that `head`; Q did not confirm that and treated it as plausible and unverified, so it is recorded here as the parent's own claim only.

**Therefore the #778 "ZX20 mount identity mismatch" does not reproduce against the deployed copy as of A1's controlled readback.** It does reproduce against the repository copy — see 4.3.

### 4.3 The repository-side Ops source is stale and fails closed

- `python3 /media/jer/Files/CCF-Primary/CCF-Ops/scripts/lane-status --storage-only` → `ERROR: ZX20 mount identity mismatch: {'source': '/dev/sdc2', 'target': '/media/jer/ZX20', 'fstype': 'exfat', 'uuid': '18D6-896E'}`, exit 2.
- Cause: `CCF-Ops/lanes.json` on `ops/workflow-foundation-v1` still pins `"device": "/dev/sda2"` (with `"repository": "/media/jer/ZX20/CCF-Repository.git"` and four lanes).
- `CCF-Ops/` is **not tracked on `origin/main` at all**; the only repository copy lives on `ops/workflow-foundation-v1`, whose last commits touching it are `3614783e` and `b09031c5`.

So the reported failure was real, but it describes the repository source, not the installed state. Two things follow. The fail-closed behaviour here is **correct**, and the stale pin is a defect. But that same stale pin is **currently the only thing stopping the repository copy's scripts from acting on the live Unity lock state with a stale registry** (Q-H1, section 4.5). It must not be "fixed" in isolation.

### 4.4 Deployed bytes vs the approved #652 source

sha256 of deployed `/media/jer/ZX20/CCF-Ops` readback:

| File | Deployed | Approved #652 source (`CCF-OpsReadiness-652-20261116` @ `4c7acc05`) | Relationship |
|---|---|---|---|
| `scripts/lane-status` | `e7102a83…` | `e7102a83…` | identical |
| `scripts/unity-lock` | `54cf0a95…` | `8fa0c5ad…` | **deployed lacks the D-091 no-expiry feature** |
| `scripts/_ccf.py` | `ab4343ef…` | `24a32146…` | deployed is older; still defaults the helper to `/dev/sda2` while honouring the configured by-id device |
| `lanes.json` | `c1d14719…` (mtime 2026-11-18 17:28) | `f73d1c08…` | deployed registers 25 live lanes including `U3`/`Q10`; the source is a **stale 4-lane fixture** (absent NonUnity path, no U3/Q10), not a registry |
| `deployment.json` | `8c66b290…` | n/a | observation receipt only |

This confirms the board's own statement that installed `unity-lock` still requires the old `--eta` until a separately reviewed deployment. It also shows the deployed configuration changed on 2026-11-18 **after** its own receipt was recorded (2026-11-15T23:18:25Z). The receipt's configuration projection deliberately excludes `/lanes`, so lane edits fall outside its hash **by design**; the non-lane projection `ed642a52…` still matches. That is still not a licence to treat the receipt as a contemporaneous lane/ownership audit.

### 4.5 Cross-copy live-state hazard (Q-H1) — the most serious finding of this cycle

Q read the code rather than running it, and found a hazard that A1's and the parent's first drafts both missed:

**Every copy of the Ops scripts writes the same fixed live state directory**, `/media/jer/ZX20/CCF-Ops/state/unity`, because that path is a constant in `unity-lock`. Each copy, however, checks permissions against **its own** `lanes.json` and its own board.

Consequences:

- The deployed copy is the intended one, and it currently checks the correct configuration.
- The **repository copy R** is blocked only by its stale `/dev/sda2` pin. That stale pin is accidentally the thing preventing a stale registry from authorising operations on live state.
- The **approved #652 source checkout A** appears, on static reading, to *satisfy* `config_read` today: it carries a 4-lane configuration and the 37-line October board, which are internally consistent. Run from there, `lane-status` would report storage OK with October owners, and `release --force` with any free text as the "manager approval" would delete the live `lock.json`.
- CCF-Temp scratch copies and replicas have the same property.

This is why Q rejected the parent's original G1-a framing. "Fix the stale pin" in isolation would remove R's accidental protection and create a second tracked lane list that will itself go stale.

**Required guard, not granted:** before any non-deployed copy's pin or configuration is corrected, the tools must refuse to mutate live state, and must refuse to present their own lanes or board as live, whenever they are run from anywhere other than the deployed root. That needs an explicit negative witness for each of R, A and a CCF-Temp copy.

**Interim operating rule (a recommendation only, not an authorisation):** until that guard exists, invoke only `/media/jer/ZX20/CCF-Ops/scripts/*` for any status or lock operation, and never a source or scratch copy.

### 4.6 The identity guard pins ZX20 only (Q-M1)

The deployed identity check protects ZX20. **It does not pin the Files mount.** Files-backed lanes and the bare repository are admitted on the strength of a resolved path prefix under `/media/jer/Files/CCF`; the deployed board is not — it is still constrained to ZX20. If `/media/jer/Files` were ever unmounted while a real directory `/media/jer/Files/CCF` existed on the root filesystem, that root-backed directory would be accepted for those Files-admitted paths.

The approved #652 source narrows lane admission but **widens board admission too**, to the Files CCF tree, so it does not close this gap and adds a second admission surface. The ZX20 checks are best described as a mistake guard — they stop a wrong device, a duplicate UUID, a renumbered kernel name and a remount — not as an anti-clone or anti-masquerade proof, and they are evaluated once per command.

### 4.7 D-091 compliance of the projected force path is not established (Q-M2)

In the projected no-expiry `unity-lock`, `--force` requires only `--owner`, `--reason` and a free-text `--manager-approval`, and it **skips the Editor and recorded-PID checks**. The tool cannot verify that the approval text refers to any real authorisation. Calling that projection a "D-091 candidate" overstates it: the no-expiry acquisition mode is closer to D-091, but the force path still allows an occupied lock to be cleared on unverifiable text.

This is **not a regression introduced by the projection**: the deployed `unity-lock` carries the same free-text force gate. What the projection adds is no-expiry acquisition, while leaving the force path as it is. The question is therefore an explicit decision item for the #652 source owner, not something the parent design may assume away.

### 4.8 Preserved user work in the primary anchor

`/media/jer/Files/CCF-Primary` was left byte-identical. It remains on `ops/workflow-foundation-v1`, ahead 1 / behind 1, with exactly these uncommitted items:

| Path | sha256 (before and after) |
|---|---|
| `AGENTS.md` | `1bf4286b3485de018759a5b9b8e19d74beb6385844b56315f22f71b7819897fd` |
| `Docs/Operations/StorageArchitecture.md` | `6bebe49511f310c0f97b000f2f5a6c8db6890bb77fd312b9ec47e47c900a5a22` |
| `Docs/Project/coordination/active-tasks.md` | `ec842f269258d64aec3fee6b23f8c56ef1b8fa63822caf2e4472d39611afbc8f` |
| `Docs/Operations/PostStage1CCFWorktreeEvidenceDisposition-20261118.md` | `fc8cff37496aaeca63f66dccb490f5e250c8812909460bb70b08a860145d81be` |
| `Docs/Operations/Stage1StorageRecoveryExecution-20261118.md` | `188fd66dad072912f1266a9f7ac927e5745a8b3c204e69b5346df9af576a9651` |
| `Docs/Operations/StaleWorktreeDiskPressureHandoff-20261118.md` | `c2d9f78fe82f4fd74d4b5552b6c1b1df13145fa96c5f293922ad1c8873e39691` |
| `Docs/Project/coordination/archive/active-tasks-2026-10.md` | `4b9faa3eae8d8a389ddcc8dc0d8a754f545a193a6dd9e6805626778a75510b22` |

## 5. A1 — physical identity and Ops readback root cause

**Actor:** A1 research child, T3 Code / Codex, provider runtime-reported `gpt-6.1-sol` (effort `high`), host `jer-laptop`, codex thread `01a1f156-cdbe-7d11-bc51-125cff22b786`. T3 delegated thread `…delegate-task%3A5fc90470-52cc-4ad3-a14e-053c86d3e37e`.
**Branch:** `docs/sen-1-781-a1-physical-identity-20261118-v1` at `2b14d8601dc5ea13a0e7a26f34c8d698b7b3c997` (pushed).
**Document:** `Sen-1-781-A1-PHYSICAL-IDENTITY-AND-OPS-READBACK-ROOT-CAUSE-20261118.md`, 1,659 lines, sha256 `07e9058e930f66e569461a940302f42ef69207fe50fcc3a255a963ecb74322ba`, one file added, nothing else changed.
**Placement:** its gate passed first: realpath `…/CCF-781-A1-PhysicalIdentity-20261118`, Files `/dev/sdb4` ext4, branch as assigned, HEAD `a69b3e2e`, clean.

A1's root-cause statement, which the parent accepts as consistent with its own readback:

1. Today's ZX20 is the Netac external SSD partition `/dev/sdc2`; `/dev/sda2` is root on the Samsung internal SSD; Files and Videos are **partitions of the same HGST physical HDD**, not separate disks.
2. The **deployed** `lanes.json` already pins the persistent by-id path and the deployed `_ccf.py` compares block-device numbers plus exact mount target, UUID, fstype and unique visible UUID. Its storage check succeeds. The **repository copy** still compares a literal `/dev/sda2` and fails at `storage_check` before its retired repository/NonUnity paths are even reached. A kernel renumbering explains why a literal pin became invalid; it does not authorise substituting another current `/dev/sdX` name.
3. Deployed status sees no `lock.json` and no process whose `argv[0]` basename is exactly `Unity`, so it reports free. That is a lock/process snapshot, not a lane release or an execution permit.
4. Full `lane-status` does sequential recursive `du -sh` per lane and prints only after collecting everything; A1's bounded run hit 55 s without a report and was not repeated.
5. The receipt is old observational provenance, not a deploy authorisation. Non-lane configuration still matches its recorded projection (`ed642a52…`); lane registrations changed legitimately after the receipt (receipt recorded 2026-11-15T23:18:25Z; `lanes.json` mtime 2026-11-18 17:28). The receipt must not be presented as a contemporaneous lane/ownership audit.
6. Board semantics produce misleading labels: `lane-status` matches the exact Lane string and copies free-form Owner text, including `NONE` and `DONE-PENDING-LEDGER`, and does not consult authoritative `main`, issue claims or physical custody. Its ordinary exit 0 means storage ready, not lanes ready.

Additional A1 findings the parent carries forward:

- **`unity-lock status` is not a strictly no-write command.** With the state directory present it enters `guarded()`, which calls `mkdir(exist_ok=True)` and opens the guard with `a+`. A1 therefore did **not** run it unmodified; it used an in-memory write-denying wrapper with an audit hook and reported the same readback (`status: free`, no Editor PIDs). This is recorded as a verification limitation, not an unmodified CLI PASS. The parent had already run the unmodified command once; Q observed the guard still at 0 bytes with mtime 2026-10-16 and `history.jsonl` unchanged since 2026-11-15, which is **consistent with no byte change but is not a before/after proof**. The parent's run is therefore disclosed as a deviation from the read-only intent, not claimed as clean.
- **Deployed vs approved #652 source:** deployed `lane-status` equals the approved source; deployed `unity-lock` equals the *old repository* version and lacks the approved D-091 no-expiry feature; deployed `_ccf.py` is older and still carries an unused `/dev/sda2` default; eight-file candidate `60f97b06…` is `NOT_DEPLOYED`.
- **Staged projections exist but are not installed:** two identical five-file projections with the approved no-expiry `unity-lock` and merged README (manifest `9e23d2fd…`), plus 13 retained base/replica snapshots. None of these is authority to deploy.
- **Owner/coverage gap:** 14 configured lanes have no direct live-board `Lane` match (`Q10`, `U1`, `Integration`, `NonUnity`, `RackV2-09`, `S1Obj`, `IntroAstra243`, `AccessSpineMeasure248`, `UIQual275`, `MainInt277`, `WorkbenchTier2Reg287`, `UIQual376`, `Objective1UX443`, `Sen1SaveA0NonUnity690`). A1 correctly refuses to conclude they are ownerless or reclaimable: several have narrative reservations, open issues or issue-level releases that the table does not show, and `Sen1SaveA0NonUnity690` does not exist on disk while live `main` still names it.
- **The local anchor's `decision-log.md` does not contain D-091**, while live `main` does. Local canonical context is therefore behind the accepted owner policy.
- **25 lanes ≠ 25 slots:** several configured lanes are on Files and not Unity-capable; a free global lock implies nothing about per-lane availability.

A1's bounded correction register (C1–C7) is preserved in its document with file set, source authority, custodian, risk, negative witness and required permit, each marked `NOT GRANTED` / `G1 HELD` / `G2 HELD`. The parent adopts it in section 9 rather than restating every row here.

A1 explicitly declined to claim: completed full inventory, unmodified `unity-lock` PASS, all owners absent, all lanes on ZX20, live D-091 compliance, a no-expiry Unity permit, or any G1/G2 approval.

**Bounded amendment after Q review (A1-1, A1-2, A1-3).** A1 adopted Q-H1 as a first-class finding in its own document: every Ops copy targets the same fixed live `state/unity` directory while checking its own `lanes.json` and board; R is blocked only by its stale pin and **that failure is protective**; the #652 source checkout appears to satisfy its own checks and could create a record that D reads as malformed, or clear live state via force, bypassing Editor/PID checks. A1 also added the Q-M1 Files-identity residual and board-admission widening, corrected its claim that A is "uniformly stricter", and added the "`--force` while a Unity Editor or live recorded PID exists" decision item to C2, each with the required non-deployed-copy negative witness.

Amendment pin: branch `docs/sen-1-781-a1-physical-identity-20261118-v1` at `a7e985ee` (parent `2b14d860`); document sha256 `d2869a5aaf08125b9f5c0ce49a2f2d73cb767d193f60970b6dc87eddfa3f7130`; 27 insertions, 5 deletions, one file. **Provenance note:** the A1 correction session made and committed this amendment, then hit its provider usage limit before pushing. The parent pushed that author commit unchanged — a mechanical publish, not a content edit. Nothing in it was rewritten by the parent.

## 6. A2 — November coordination and local compaction reconciliation

**Actor:** A2 research child, T3 Code / Claude, runtime-observed `claude-sonnet-5-5`, provider instance assigned `claudeAgent_claude_b_pro` (reroute), host `jer-laptop`. Harness scratchpad UUID `1912523c-d2bd-452f-9191-ba6f036788e4`.
**Branch:** `docs/sen-1-781-a2-coordination-20261118-v1` at `2b08d661389329521080746178cd247f73a18c34` (pushed).
**Document:** `Sen-1-781-A2-NOVEMBER-COORDINATION-AND-COMPACTION-RECONCILIATION-20261118.md`, 419 lines, sha256 `1d974d37e07a2d5367b0223e9301c3e41eeab9fae69e35aef5d72f74b8808336`, one file added.

**Verdict carried into the parent:** the local 207→93 trim is **not** a compaction of current coordination. It must not be committed, merged, copied onto `main`, or used as an ownership source; it is read-only evidence.

Verified basis:

- **Divergent base.** The trim derives from the ops-branch file at `46247486` (2026-10-30). The anchor and `main` share only the **2026-10-12 merge-base `c1564b85`**; `main` has 1,849 commits the anchor lacks, the anchor has 62 `main` lacks.
- **Retired designation.** The local header still names the ops branch as the authoritative record. `origin/ops` retired that on 2026-11-15 in `a29c2fa4`; the anchor is one commit behind it and does not carry the retirement.
- **Scale of loss.** `main`'s file is 981 lines / 779,213 bytes (blob `af3ca9a9…`, sha256 `cd38c93a…`); 969 lines are November-authored; 297 lines reference OPEN issues across 53 distinct open issues. The trim keeps **one line in common** with `main` and 2.0 % of its bytes (952 deletions).
- **False statements.** "No November work is recorded" is false for live coordination; the "board equally empty" claim is false for the deployed board (44 task rows, 25 of them November); the "fidelity checks" describe the stale repo-tracked `lanes.json` (4 lanes, `/dev/sda2`), not the deployed 25-lane by-id configuration.
- **Silent ownership edits.** Three of five retained rows had Owner or model fields changed with no recorded authority (one model change, two Owner cells set to `NONE`) — exactly the "silent deletion of ownership" the packet asked Q to test, in miniature.
- **Archive overstatement.** 26 of 115 removed/changed non-blank original lines are not verbatim in the untracked archive, so its "verbatim" claim is wrong.
- **Real downstream harm already caused.** The #777 parent's #778 preflight recorded "Live project coordination: BLOCKED" on the strength of this file.
- **`main` is itself behind reality**: no #781, #789, #785–#788 entries; still describes #777 as "READY, NOT RUNNING". A2 therefore requires a **registration-sync commit first**, then a forward-only compaction from current `main` with keep-by-default tests, a manifest, mechanical proofs and independent review.

A2 disclosed two deviations: it ran read-only `git worktree list` several times despite being told not to use `git worktree` (no add/remove/prune), and it observed a changed `.git` worktree directory mtime while the anchor's `index` and `HEAD` were untouched. It states plainly that it ran no `lane-status`/`unity-lock`, inspected no lock or process state, and could not explain the 20-vs-25 lane count — which the parent **attributes to its own `head` truncation (parent-attested, no retained transcript)**.

## 7. Q — independent nonauthor review

**Reviewer:** T3 Code / Claude, provider instance `claudeAgent_claude_b_pro` (reroute), model `claude-opus-5-5`, host `jer-laptop`. **Branch:** `review/sen-1-781-q-ops-admission-20261118-v1` at `1f89f10f469f0099f4686088cf2ad3faf9b4ffaf`. **Document:** `Sen-1-781-Q-INDEPENDENT-IDENTITY-AND-OWNERSHIP-REVIEW-20261118.md`, 246 lines, sha256 `01dd3b9986ea75516875c37a6352f88456ec5c28b802a871018aa9b4d3be9542`.

### 7.1 Verdict

**NEEDS CHANGES — for diagnostic and recovery design only.**

Q confirmed that A2's conclusion holds and that A1's root-cause diagnosis is correct, while rejecting A1's proposed remedy set and this parent's first draft. The verdict applies to the diagnostic and recovery *design*; it is not an installation, runtime or acceptance verdict.

### 7.2 Findings Q raised

| ID | Severity | Substance |
|---|---|---|
| Q-H1 | HIGH | Any copy of the Ops scripts writes the same live `state/unity` directory while checking its own `lanes.json` and board. The repository copy is protected only by its stale pin; the approved #652 source checkout appears to satisfy its own checks and could force-release the live lock on free text. |
| Q-H2 | HIGH | The parent's first draft overclaimed: an unretained "completed" full inventory, a lock readback presented as a pass from a write-capable command, and an unsupported "did not alter any byte" sentence. |
| Q-M1 | MEDIUM | The identity guard pins ZX20 only; Files lanes, the repository and the Files-side board are admitted on a path prefix, and the #652 source widens board admission to the Files tree. |
| Q-M2 | MEDIUM | The projected no-expiry `unity-lock` keeps a free-text `--force` that skips Editor and PID checks, so calling it D-091 compliant overstates it. |
| Q-M3 | MEDIUM | After the reroutes, A2 and Q share the provider instance; independence is by session and model, not by provider. |
| Q-L1–L4 | LOW | "Relabelled twice" was wrong (the kernel name moved, no UUID/label change found); a commit typo; a checkout-count drift; a stale fixture described as a "proposal"; and A2 listing `git fetch --dry-run` as read-only. |

Q also confirmed, explicitly:

- A1's write-blocking wrapper and its 55 s timeout are **honestly labelled limitations**, not overclaims.
- A2's limitations are honestly stated, including its disclosed read-only `git worktree list`.
- A1's and A2's counts do not conflict: `lanes.json` has 25 entries; the board has 44 rows, of which 25 are November rows and 19 are October rows. The coincidence of two 25s is not a contradiction. The parent **further claims** its own "20" came from `head` truncation; Q recorded that as plausible and unverified, so it is the parent's claim, not a Q finding.
- No proposed step would release occupied Unity custody, take #690/#718/Multi custody, or add expiry/reclaim.

### 7.3 Corrections requested and their disposition

**A1 (three bounded items):** add Q-H1 as a first-class finding with the non-deployed-copy negative witness and the statement that R's failure is protective; add the Files-identity residual and board-admission widening; add "force while an Editor or recorded PID is live" to the C2 decision items. **Status: DONE** — branch `docs/sen-1-781-a1-physical-identity-20261118-v1` now at `a7e985ee`, document sha256 `d2869a5aaf08125b9f5c0ce49a2f2d73cb767d193f60970b6dc87eddfa3f7130`, 27 insertions and 5 deletions in one file. The author session committed and then hit its usage limit before pushing; the parent pushed the author commit unchanged.

**A2 (one bounded item):** reword the `git fetch --dry-run` line so it does not claim strict read-only. **Status: DONE** — branch `docs/sen-1-781-a2-coordination-20261118-v1` now at `21035b27c28af8445ee3efcad2026d8c8fcada1a`, document sha256 `adbb6bb6ff8f9f08567e3c8f9c04101dfd95441d103facaebd464e2b66c9aab7`; the change is three insertions and one deletion in one document, and A2 reports that it alters no conclusion.

**Parent (P-1 to P-15):** applied to this document. The material ones are section 4.1 (kernel name, not relabel), 4.2 (storage check versus write-capable lock readback, and the unverified full run), 4.3 (the stale pin is protective), 4.5–4.7 (new Q-H1/Q-M1/Q-M2 sections), 5 (no-rewrite claim replaced with "consistent with", no before/after proof), 6 (parent-attested attribution), 9.1 (residual risk list instead of a clean bill), 9.2 G1-a (live-state containment first) and G1-d (no-expiry acquisition mode, not D-091 compliance).

### 7.4 Delta recheck

**Reviewer:** same Q reviewer, new session, `claude-opus-5-5`, `claudeAgent_claude_b_pro`, nonauthor. **Branch:** `review/sen-1-781-q-ops-admission-20261118-v1` at `cc53729d`; document sha256 `0226b2c41ccce198fc20f7d659cedfb32aaf95299968432c493655e9f8943508` (334 lines; the commit appends `## 9. Delta recheck` and changes nothing earlier).

**Delta verdict: NEEDS CHANGES — diagnostic and recovery design only.** A1-1, A1-2, A1-3 and A2-1 all **PASS**, and parent corrections P-1 to P-12, P-14 and P-15 **PASS**. Four parent-only LOW wording items remained, none HIGH or MEDIUM:

| ID | Issue | Disposition |
|---|---|---|
| R-1 | §3.1 still stated "25 checkouts" while withdrawing that number two sentences later | Fixed: the unstamped count is removed and Q's timestamped 23 at 01:29Z is the only figure |
| R-2 | §7.2 and §4.2 treated the "20 lanes from `head` truncation" explanation as Q-confirmed | Fixed: recorded as the parent's own claim; Q's plausible-but-unverified treatment is stated |
| R-3 | §4.7 heading said "not D-091 compliant" rather than compliance "not established", and omitted that the deployed copy has the same force gate | Fixed: heading and body corrected; explicitly not a regression introduced by the projection |
| R-4 | §4.6 and §9.1 said the *deployed* check admits a Files-side board | Fixed: the deployed board is ZX20-only; only the #652 source widens board admission |

Q stated that a fresh full review is not needed and recommended only that **someone who wrote none of the documents** compare the changed lines against `c342107a` to confirm nothing else moved. That narrow nonauthor diff check is the final closure step. Q also disclosed two caveats: the harness wrote two text-only tool-result files under `/home/jer/.claude_ccf2/…` on the root filesystem (not its choice, content already committed), and Git attributes all three commits to `jermur788`, so it could not independently verify that the A1 correction session authored `a7e985ee`; it accepted the parent's statement without verification, and the parent repeats it here as a statement rather than an independently proven fact.

## 8. Truthful permission state after G0

| Item | State |
|---|---|
| Installed Ops repaired or reconfigured | NO — not attempted |
| Non-deployed copy given a live-state containment guard | NO — Q-H1 remains open |
| Repository Ops source corrected | NO — not attempted (G1 HELD) |
| Lane registry / board / deployment reconciled | NO — unchanged |
| `active-tasks.md` compacted, trimmed or republished | NO — local uncommitted proposal preserved untouched |
| Unity lock acquired, released or force-released | NO — nothing acquired or released; one unmodified `status` read by the parent is disclosed as write-capable |
| Unity Editor, Player or build run | NO |
| Game source or save touched | NO |
| Main merged or advanced by this work | NO |
| Independent review closed | NO — Q returned NEEDS CHANGES; delta recheck outstanding |
| Drive / dashboard mirrors | NOT VERIFIED |

## 9. What a lawful G1 and G2 would require

Nothing in this section is granted. It converts A1's C1–C7 register and A2's Steps 0–9 into one actor-specific sequence so that Manager #731 can route it without re-deriving it. Every item below is a **proposal requiring its own authorisation**.

### 9.1 No change is needed to make `--storage-only` pass today

The deployed storage check already passes for ZX20. No correction exists to make it succeed, and **no proposal here replaces the persistent by-id device with a current `/dev/sdX` name, weakens the UUID/fstype/mount-target checks, fabricates an expiry, clears a lock, or copies a repository/scratch `lanes.json` over live physical state.**

That is a statement about one check, not a clean bill of health. The residual risks that remain are:

1. **Q-H1 (HIGH)** — any non-deployed copy of the scripts can act on the live Unity lock state using its own stale registry, and the repository copy's stale pin is currently what prevents that.
2. **Q-M1 (MEDIUM)** — the Files mount identity is not pinned; Files lanes and the repository are admitted on a path prefix alone, and the #652 source additionally admits a Files-side board.
3. **Q-M2 (MEDIUM)** — the projected no-expiry `unity-lock` retains a free-text `--force` that skips Editor and PID checks.
4. **A1 C5** — `unity-lock status` is write-capable by design (`mkdir`, guard `a+`), so a status read is not a strict no-write operation.
5. **A1 C6** — full `lane-status` performs per-lane recursive `du` and can exceed a minute.
6. **Stale source and stale documentation** — the repository copy is a stale fixture, the receipt predates the current lane list, and `StorageArchitecture.md` still describes kernel device names from a previous boot.
7. **Stale coordination** — see section 6 and A2's procedure.

### 9.2 G1 — isolated, non-installed source corrections (NOT GRANTED)

Preconditions: Manager #731 plus the actual #652 source custodian authorise an exact file set and a safe, non-root, isolated source lane. #652's existing NO LIVE INSTALL restriction stays in force.

| Order | Scope | Owner | Must include |
|---|---|---|---|
| G1-a | **Live-state containment first**, then repository-side Ops source: a guard in `scripts/unity-lock` and `scripts/lane-status` that refuses live-state mutation, and refuses to present a copy's own lanes/board as live, when run from anywhere other than the deployed root; only then `CCF-Ops/lanes.json`, `CCF-Ops/scripts/_ccf.py`, `CCF-Ops/README.md` on a successor of `ops/workflow-foundation-v1` | existing #652 source custodian | **no tracked copy may become a second live registry**; persistent by-id device, Files repository/NonUnity, no stale `/dev/sdaX` pins; current lane list supplied by the physical custodian rather than invented; negative witness for each of R, A and a CCF-Temp copy; correcting R's storage pin is forbidden until the guard exists, because R's current failure is protective |
| G1-b | Strictly read-only `unity-lock status` (A1 C5): `scripts/unity-lock`, `tests/self-test.py`, `README.md` | #652 source custodian + independent reviewer | status on an absent or malformed state must create nothing and must fail conservatively; acquire/release serialisation unchanged; live Editor never reported available |
| G1-c | Bounded `lane-status` inventory (A1 C6) | #652 source custodian | optional/skippable disk-usage pass that cannot bypass board, store, dirty, identity or branch-conflict checks, and cannot change exit semantics silently |
| G1-d | **No-expiry acquisition-mode candidate** (source half of C2/C4): `scripts/unity-lock`, `tests/self-test.py`, `roles/unity-operator.md`, `README.md`, regenerated `deployment.json` | #652 installer + independent reviewer | exactly the reviewed projection, not the whole approved tree; interrupted-install and old-reader-rollback negative tests. **D-091 compliance of the whole custody route is NOT established**: the projected force path still accepts free-text approval and skips Editor/PID checks (Q-M2). Renaming this a "D-091 candidate" would overstate it. |
| G1-e | Documentation (A1 C7): `Docs/Operations/StorageArchitecture.md` and future `CCF-Ops/README.md` examples | operations docs custodian | dated observed identity instead of dated kernel names; Files and Videos noted as partitions of one disk; never edit the dirty primary file in place |

Required G1 evidence: full existing self-test suite plus the negative witnesses named in A1's register (wrong by-id target, duplicate visible UUID, UUID/fstype/target mismatch, symlink or path escape, wrong Git store, occupied/unclaimed Editor, held/malformed lock, expired ETA never auto-releasing). Cross-family independent source review, then one bounded correction cycle. **An offline PASS on a candidate is not a deployment permit and not a Unity permit.**

### 9.3 G2 — physical custody and live configuration (NOT GRANTED)

Preconditions: the owner/Manager authorises a physical operation in writing, with a named custodian, a tracked backup/rollback, process and occupied-owner checks, and an explicit no-install or installer risk decision. D-091 governs: no expiry, no stale-agent reclaim, no forced clearing.

| Order | Scope | Custodian | Required safeguards |
|---|---|---|---|
| G2-a | A1 C4: refresh `deployment.json` for whatever is actually installed, as its own receipt transaction | #652 provenance custodian | truthful post-install hashes; no false candidate-deployed claim; no hiding drift in `live_state` |
| G2-b | A1 C2/C4 live half: install exactly the reviewed projection and its receipt | named installer + owner + independent reviewer | pre/post mount/device/UUID/path/status readback; rollback proven; refuse rollback when an active no-expiry lock would become malformed |
| G2-c | A1 C3: reconcile `board/active.md` and `lanes.json#/lanes` row by row | physical board/custodian under the board-directory lock | preserve all 25 live entries until each is individually decided; carry #690/#710/#725/#755 and Multi #730 custody; never recreate or delete a lane from a stale record; preserve `Sen1SaveA0NonUnity690`'s stale-path defect as an open item rather than silently dropping it |

### 9.4 Coordination recovery — a separate Manager lane, not a G1/G2 side effect

A2's forward-only procedure (Steps 0–9) belongs to Manager #731 or an assigned integrator, because `active-tasks.md` is single-writer. It must start from current `main`, never from the anchor's dirty file, and it must be preceded by a **registration sync** commit covering #777's closure, #778's current state, #781, #789, #785–#788 and the current Multi lanes. The anchor's dirty file, archive and three notes stay untouched until the owner or a physical custodian authorises a hash-pinned read-only preservation copy.

### 9.5 What is not authorised by any of the above

No Unity Editor, Player or build; no #690 save, #718 scene or Multi source work; no lane deletion, mount change, lock acquisition or force release; no use of GPT-6.2; no Drive/dashboard claim; no main merge by this programme.

## 10. Recommended next step

1. **Complete the review loop.** Run Q's single delta recheck against the corrected A1, A2 and parent documents. Do not hand this result to Manager #731 as review-closed before that verdict exists.
2. **Manager #731 decision gate (no physical work needed):** decide whether to route the existing #652 source custodian into a lawful G1, in this order — **live-state containment guard first** (Q-H1), then the no-expiry acquisition mode with the force path decided explicitly (Q-M2), then the Files identity gap (Q-M1), then the stale repository fixture, then documentation.
3. **Coordination recovery as its own lane:** registration sync commit on `main` (covering #777's closure, #778, #781, #789, #785–#788 and current Multi lanes), then A2's forward-only compaction with a manifest, mechanical proofs and independent review. The anchor's dirty file must not be used as an input.
4. **Operational hygiene, safe now and requiring no permission:** read coordination from `main`, not from the anchor; use only the deployed Ops scripts for any status or lock operation; and do not accept the harness's default isolated checkout on this host, because it lands on the root filesystem.
5. **Still not authorised:** anything in G2, any Unity work, any lane reclamation, and any use of the local trim.
