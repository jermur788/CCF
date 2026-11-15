# CCF-Ops persistent identity durability result — 2026-11-15

> Latest status: see **Final round — portable self-test device assertions; residual inherited boundary fixture** below. The transient-device defect is fixed; self-test still exits 1 after 33 passes because its existing `/` sentinel conflicts with the required storage boundary. No green self-test or integration is claimed. Earlier rounds remain historical.

Implementation worker under orchestrator #544, claim #548. PACKET ONLY; operations tooling; ready for bounded review, **not integrated, deployed or fully qualified**. Issue #548 remains OPEN for Manager.

- Branch: `ops/unity-lease-persistent-identity-20261115-v1`
- Worktree: `/media/jer/Files/CCF/Operations/CCF-OpsLeaseFix-20261115` (one new sparse Files worktree; no Assets checkout).
- Base: `a29c2fa480f1077e12434dcf6679f2f5d9465cdc`, freshly fetched `origin/ops/workflow-foundation-v1`.
- Context commit: `46247486b37904f6b36825bb40fb6ed7bea35e88` (base's latest canonical-context change).
- Verified source HEAD: `8483e5b844564dcedc47d289c877aed49076cdab`. The final publication HEAD is recorded in the #548 receipt; it adds this handoff/provenance only.
- Runtime: T3 Code / Codex / gpt-6.1-sol high; account/quota label unavailable. No delegates.

## Two-tree root cause

`/media/jer/ZX20/CCF-Ops` is the operational deployment designated by StorageArchitecture. Its persistent pin already resolved to actual mount `/dev/sdc2`, target `/media/jer/ZX20`, exfat, UUID `18D6-896E`; live CLI returned free/zero Editors before changes. `/media/jer/Files/CCF-Primary/CCF-Ops` and the requested remote base instead pin `/dev/sda2` and compare strings, reproducing the false mount mismatch (exit 2). Unity usability needed no live repair. The defect is an unversioned live identity fix and stale deployment provenance. Rehashing confirmed **seven** drifted recorded paths (the packet said six but listed seven): lanes.json, AGENTS.md, README.md, environment.sh, scripts/_ccf.py, scripts/lane-status, board/active.md. The two lease/lint executables still match; no lease implementation rewrite was needed.

## Exact change and excluded semantics

[Exact source patch (JSON-escaped unified diff)](ops-lease-persistent-identity-evidence-20261115/identity.patch.json): three files, 35 insertions / 6 deletions. `lanes.json` changes only the device pin. `_ccf.py` imports stat and adopts live same_device / visible_uuid_devices / require_unique_uuid plus their storage_check/storage_health calls. Only block-device st_rdev equality succeeds; mount target, fstype, UUID, visible-UUID uniqueness, rw and capacity checks remain. The default storage_check argument also uses the persistent pin; live still has a stale `/dev/sda2` default, although its CLI supplies the correct config explicitly. `lane-status` adopts same_device for lane classification. `unity-lock` and `packet-lint` are byte-unchanged.

Additional live semantics were **not** adopted:

1. `_ccf.py`: FILES_ROOT/DURABLE_CCF constants and on_durable_ccf helper; accepting repository paths in the Files CCF tree; accepting non-Unity lane paths in that tree. Unity-lane policy and board-path storage checks remain unchanged in this patch.
2. `lane-status`: DURABLE_CCF import, permitting inventory of Files non-Unity lanes, and the separate off-storage Unity/non-Unity warnings.
3. `lanes.json`: canonical repository/git-store relocation to Files, legacy-store relocation, NonUnity path/purpose relocation, and twelve extra lane registrations: Q10, U3, RackV2-09, S1Obj, IntroAstra243, AccessSpineMeasure248, UIQual275, MainInt277, WorkbenchTier2Reg287, UIQual376, ResearchCopyQual391, Objective1UX443.
4. Live AGENTS.md/README.md/environment.sh/board/state/ledger content was not imported. Manager-owned active-tasks and both boards were not edited.

[Remaining helper diff](ops-lease-persistent-identity-evidence-20261115/scripts-_ccf.py.remaining-live-diff.patch.json), [lane-status diff](ops-lease-persistent-identity-evidence-20261115/scripts-lane-status.remaining-live-diff.patch.json), [config diff](ops-lease-persistent-identity-evidence-20261115/lanes.json.remaining-live-diff.patch.json) explain every remaining byte difference. Therefore the requested whole-file hash equality is deliberately **not met**; importing all live bytes would violate the identity-only scope.

## SHA-256 before / after / live

`CCF-Ops/scripts/_ccf.py`:

- Base: `b12a613379d69c5afa8e4c20ae0fcdcff6b8772c39501f60c01933263cf46923`
- Branch after: `429e35ae7b4333f343334e02b30de17843c40a3dca25fe1feacc2f6df4c2ffb3`
- Live before and after: `ab4343efc7d1b82c5a4aca747d0e09c614779cfd9aa406a0dd681d204f5de001`

`CCF-Ops/lanes.json`:

- Base: `53d3ed9c79adbe032b406d1f1ea4342613532b4c432e6df2c5d354f9719e77f9`
- Branch after: `daf54563c6ddccf7f23fff764093ab44189c3c20e27ab461ce81cf010ea5e2f2`
- Live before and after: `1b6d70eb1a884d21dffc1e9d92db20a57cb728dd7a534aeec52ab8386cecaf21`

`CCF-Ops/scripts/lane-status`:

- Base: `7fca072046997d43caf51a87397d2735c579cc2c1732515cfa088c9dff4b3761`
- Branch after: `a240098779136a0a586956bdbf985624ad79cdd1abb41ad501f3eba380726546`
- Live before and after: `e7102a83ad942cc7365cb22db9a8f79823744ea98bc2cf8a6738a11f1897173f`

[Machine-readable hashes](ops-lease-persistent-identity-evidence-20261115/hashes-after.json) also cover unchanged lease/lint files and unrelated live drift.

`CCF-Ops/deployment.json` did not exist on the requested base. The new receipt names the exact source commit/hash set and branch. `deployed_at: null`, `deployment_status: NOT_DEPLOYED`, and a real `recorded_at` truthfully distinguish candidate capture from deployment. The receipt commit follows the source commit to avoid self-reference. It deliberately does not claim to describe the whole live deployment. Live deployment.json remains byte-identical at SHA-256 `22bb354343c699cc78903d4a5d1b1b13548e126a4a04351f54f25586905195ed`; its old provenance is still stale and requires a separately scoped, reviewed reconciliation.

## Command results

[Recorded commands with real stdout/stderr and exit codes](ops-lease-persistent-identity-evidence-20261115/commands.md). Full unfiltered local evidence is `/media/jer/Files/CCF/Operations/OpsLeaseEvidence-20261115/commands.{md,jsonl}`. Initial read-only reconnaissance predates this recorder; its tool output remains in the T3 session. Source reads/edits were executed through the session's Python runner; the exact resulting patch is published. Raw board text is referenced externally to honour the no-live-board-content-in-Git limit.

- Branch direct storage_check and storage_health: **PASS**, exit 0, physical `/dev/sdc2`, rw, 55,409,115,136 free bytes / 51.60 GiB against 15 GiB floor.
- Live unity-lock status: **PASS**, exit 0, free / metadata null / Editor PIDs [].
- Machine-wide normal acquire → status → release → status: **PASS**, all exit 0; lock `8d77dccc25ca468baf1a69af92fa45f5`, owner `Codex-Ops548-durability`, task `CCF-548-IDENTITY-DURABILITY`, PID 773451 (verification Python process), acquired 2026-11-15T22:44:25.592554Z. Lane UIQual376 was clean at `2aaacad68ed186dbe14531f31ce5bb3a87811441`, branch `qualify/scenario-one-tagged-text-379-20261111`, before/after. This was a metadata-only cycle, with no source/Editor ownership claimed. Complete requested returned metadata is in the command transcript. Released normally with exact owner/task/id; `forced: false`; free again with zero Editor PIDs. Only the CLI's standard lock metadata/release-history effects occurred in live state.
- No Editor launched at any point. During the lock cycle 25 process samples (~50 ms cadence) all saw zero Editors. Every CLI observation saw zero; inspected commands launch no Editor. Sampling is not kernel-wide proof against an unrelated process existing between samples.
- Temporary minimal ops copies under `/tmp/opencode/ccf-ops-lease-20261115/` contain scripts/config only, never board/state/ledger. Wrong UUID `DEAD-BEEF`, absent device `/dev/disk/by-id/CCF-NONEXISTENT-548`, wrong root `/media/jer/Files`, and wrong fstype `ext4`: each **refused with mount-identity error / exit 2** for status, acquire and release, using both branch and live scripts. 24 full-command checks (2 copies × 4 corruptions × 3 operations), plus an earlier 24-operation minimal-argument probe; live config never changed. [Results](ops-lease-persistent-identity-evidence-20261115/negative-results.json).
- Focused source verification: **18 PASS**, including actual by-id/device equivalence, char/regular/missing-device rejection, actual unique UUID, default helper, physical health, low-space/read-only/target rejection, mocked absent/duplicate UUID refusal, and parsing all four Python tools. [Verification script](ops-lease-persistent-identity-evidence-20261115/verify-identity.py). No test thresholds or repository harness were changed.
- Branch `unity-lock status --json`: **BLOCKED**, exit 2, `ERROR: missing repository: /media/jer/ZX20/CCF-Repository.git` after identity succeeds.
- Branch `python3 -B CCF-Ops/tests/self-test.py`: **FAIL**, exit 1, first packet-valid check receives the same config error (expected 0, got 2). It did not reach fixture lease/force/concurrency tests. Its own newly created empty `/media/jer/ZX20/CCF-Temp/foundation-test-6p3l0vst` directory was inspected and removed; no fixture/harness remains.
- Branch `scripts/packet-lint tests/fixtures/valid.md` and `scripts/packet-lint --board board/active.md`: **BLOCKED**, exit 2, same missing repository. No PASS is claimed for the repository self-test or lint.
- A separate read of live board through board_read fails `Unity required? must be Yes/No`; the malformed #460 row is Manager/#553 scope, not repaired here.
- `git diff --check`: **PASS**. Manifest hashes verified against the exact committed source blobs. Live source/config/provenance hashes and Primary's two dirty-file hashes stayed unchanged.
- Current capacity observation: root `/dev/sda2` 25G available (90%), Files `/dev/sdb4` 127G (72%), ZX20 `/dev/sdc2` 52G (90%). Earlier root-full report is not current; no cleanup/pruning/reclamation was attempted.

## Remaining gates and protection

The branch is a durable narrow identity patch, **not a drop-in live deployment**. To obtain a standalone green branch CLI/self-test/lint, the Manager must separately reconcile the pre-existing Files storage migration, old repository/board assumptions, and stale hardcoded self-test device paths. Live provenance reconciliation remains outstanding; do not blanket-copy this candidate onto live. No reboot/hardware renumbering or actual duplicate disk was induced; duplicate UUID behavior was verified with a mock, not physical hotplug. Occupied/foreign-owner/force/concurrency behavior was not re-exercised because the repository self-test stopped first; lock code is unchanged. No independent review, G2/G4 integration, Unity gameplay qualification, dashboard or Drive sync is claimed. Dashboard/Drive publication/readback remains Manager follow-up.

Primary status remains exactly the two pre-existing dirty files: AGENTS.md SHA-256 `1bf4286b3485de018759a5b9b8e19d74beb6385844b56315f22f71b7819897fd`; Docs/Operations/StorageArchitecture.md `6bebe49511f310c0f97b000f2f5a6c8db6890bb77fd312b9ec47e47c900a5a22`. No Primary writes, game source/Assets writes, Unity launch, build, save access, PlayerPrefs/EditorPrefs access, live deployment rewrite, board/coordination edits, lock bypass, force release, deletion of existing assets/worktrees, or package changes occurred.


## Continuation — repository pin and truthful live provenance (2026-11-15)

This section supersedes the earlier outstanding-provenance status above, while
preserving the first turn as history. Direct user continuation under #544/#548;
no delegates. **Provenance reconciliation verified; branch self-test and
packet-lint still FAIL for the inherited admission defect described below.**
No integration, tooling deployment or independent acceptance is claimed.

Implementation/source commit: `a16b66f747138f99378a5eb8f72b88181f94a760`. The following receipt/evidence commit
is the publication head, recorded with its full SHA in the #548 comment. The
required gates were run at this source commit; the same commands are rerun at
the final publication head and their full output is recorded in the #548
comment and durable `/media/jer/Files/CCF/Operations/OpsLeaseEvidence-20261115/continuation-final-gates.json`.
Receipt-only changes do not alter the tested source files.

### Repository field and remaining failure classification

Only `CCF-Ops/lanes.json.repository` changes from
`/media/jer/ZX20/CCF-Repository.git` to `/media/jer/Files/CCF/Repository.git`.
A parsed comparison against `dee4ee5` proves every other field is unchanged.
The four source lanes are retained; live has sixteen registrations. That
lane-list/path/purpose difference is **expected live-state drift (c)**, not a
defect or an instruction to import operational registrations.

[Exact diagnosis and base-line evidence](ops-lease-persistent-identity-evidence-20261115/continuation/failure-diagnosis.json):

- **(b), fixed:** obsolete `repository` storage identity. The migrated bare
  repository exists at the new path; no live configuration was rewritten.
- **(a), current blocking failure:** inherited `config_read` still calls
  `on_storage(cfg['repository'], root)`, which requires the repository under
  ZX20. `config_read` is byte-identical to base
  `a29c2fa480f1077e12434dcf6679f2f5d9465cdc` from
  `ops/workflow-foundation-v1`; its base line 82 contains that exact call.
  The migrated Files repository is therefore rejected with `path escapes
  ZX20`. The self-test stops on its first valid-packet assertion (line 38),
  before exercising any lease fixture. Both lint invocations stop in config
  admission before parsing their requested input. This is inherited validator
  behavior, not a reason to revert the correct repository path or copy live
  board/lane state. No new green claim is made.
- **(a), static downstream limitations, not executed failures:** the same base
  validator requires the config-relative board on ZX20 (base line 111), which
  rejects this authorised Files worktree. Base self-test lines 70, 78–79, 99
  and 110 hardcode `/dev/sda2` and `/dev/sdb2`; those fixture assumptions are
  not portable to the currently observed devices. No validator or existing
  assertion was bypassed or weakened to claim a pass.
- **(b), latent migration drift, deliberately unresolved under the explicit
  “That field only” instruction:** source `git_stores` still names
  `/media/jer/ZX20/CCF-Repository.git` and `/home/jer/CCF/.git`. The new repository
  is absent from that list, so the existing membership check would also fail
  after repository admission was repaired. The evidence records the exact
  values and membership result. Changing those fields would exceed the
  requested one-field config edit; no alias, fallback or automatic remapping
  was added. This source candidate remains unsuitable for blanket deployment.
- **(c), live operational board:** a separate read-only invocation of live
  packet-lint returns exit 1, `FAIL: Unity required? must be Yes/No`.
  [Actual output](ops-lease-persistent-identity-evidence-20261115/continuation/live-board-read-only-lint.json). This is excluded from
  source hash equality by design, but still fails its independent schema gate;
  provenance PASS does not certify the board or conceal that failure. No live
  board contents were imported or edited.

### Actual self-test and lint output at the implementation head

Run from the task worktree's `CCF-Ops/` directory. These are real runs, not
predicted results; stdout followed by stderr below, with exits retained.
[Machine-readable full output](ops-lease-persistent-identity-evidence-20261115/continuation/gates-source-head.json).

`python3 -B tests/self-test.py` — exit **1**:

```text
Scratch: /media/jer/ZX20/CCF-Temp/foundation-test-95s5zvjd
Traceback (most recent call last):
  File "/media/jer/Files/CCF/Operations/CCF-OpsLeaseFix-20261115/CCF-Ops/tests/self-test.py", line 38, in <module>
    check(cli('packet-lint',f/'valid.md').splitlines()[0]=='PASS','packet valid PASS')
          ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
  File "/media/jer/Files/CCF/Operations/CCF-OpsLeaseFix-20261115/CCF-Ops/tests/self-test.py", line 34, in cli
    def cli(name,*args,code=0): return run([sys.executable,'-B',OPS/'scripts'/name,*args],code)
                                       ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
  File "/media/jer/Files/CCF/Operations/CCF-OpsLeaseFix-20261115/CCF-Ops/tests/self-test.py", line 31, in run
    if p.returncode!=code: raise AssertionError(f'expected {code}, got {p.returncode}: {args}\n{p.stdout}\n{p.stderr}')
                           ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
AssertionError: expected 0, got 2: ['/usr/bin/python3', '-B', PosixPath('/media/jer/Files/CCF/Operations/CCF-OpsLeaseFix-20261115/CCF-Ops/scripts/packet-lint'), PosixPath('/media/jer/Files/CCF/Operations/CCF-OpsLeaseFix-20261115/CCF-Ops/tests/fixtures/valid.md')]

ERROR: path escapes ZX20: /media/jer/Files/CCF/Repository.git

```

`python3 -B scripts/packet-lint --board board/active.md` — exit **2**:

```text
ERROR: path escapes ZX20: /media/jer/Files/CCF/Repository.git
```

`python3 -B scripts/packet-lint tests/fixtures/valid.md` — exit **2**:

```text
ERROR: path escapes ZX20: /media/jer/Files/CCF/Repository.git
```

The stopped self-test's own scratch directory was inspected, found empty and
removed with `rmdir`; no guard, lock or history existed there. No force/steal,
production lock acquisition or Unity launch occurred in this continuation.

### Provenance model and proof

The manifest keeps **28** source hashes that actually match its named
historical Git commit and pins **7** immutable deployed overrides with reasons
and SHA-256 hashes, without claiming those overrides came from this branch.
Its explicit `live_state` excludes the board, ledger, machine state, lane
registrations and generated bytecode; a canonical JSON hash still checks every
`lanes.json` key except `lanes`. A separate five-file candidate receipt names
`a16b66f747138f99378a5eb8f72b88181f94a760` as **NOT_DEPLOYED**, so the identical Git/live manifest describes both
trees honestly without pretending their immutable bytes match each other.

The seven pinned overrides are the five explained immutable differences
(`AGENTS.md`, `README.md`, `environment.sh`, `_ccf.py`, `lane-status`) plus two
legacy supplemental deployment documents that already match the old
`immutable_deployed_hashes` but have no established source at its named commit.
[Immutable diffs against the historical source](ops-lease-persistent-identity-evidence-20261115/continuation/immutable-live-diffs.patch.json)
explain the five source differences. This is an observed baseline with retained
previous hashes, not retrospective independent approval or fictitious source
attribution. Every override remains hash-checked; source code cannot be added
to `live_state`. Both original `source_hashes` and `immutable_deployed_hashes`
coverage are accounted for, including the two supplemental documents; no
immutable check was silently dropped.

[Verifier documentation](../../../CCF-Ops/DeploymentProvenance.md),
[verifier](../../../CCF-Ops/scripts/verify-deployment),
[exact Git-blob checks](ops-lease-persistent-identity-evidence-20261115/continuation/source-blob-verification.json),
[repeatable proof script](ops-lease-persistent-identity-evidence-20261115/continuation/prove-provenance.py) and
[all actual probe outputs](ops-lease-persistent-identity-evidence-20261115/continuation/provenance-probes.json).
The verifier also fails on unclassified source additions and rejects any new
source exclusion. `deployment.json` and its timestamped backups are receipt
metadata, excluded only to avoid self-hashing. No dependency or framework was
added. The verifier is run from Git; it was not installed into live tooling.

Run the proof again from the repository root:

```bash
python3 -B Docs/Handoffs/ScenarioOne-Save-Orchestration-20261115/ops-lease-persistent-identity-evidence-20261115/continuation/prove-provenance.py --ops CCF-Ops --output /media/jer/Files/CCF/Operations/OpsLeaseEvidence-20261115/provenance-recheck.json
```

Actual live PASS, deliberate isolated `_ccf.py` drift FAIL and additional probes:

LIVE READ-ONLY AUDIT — exit **0**:

```text
PASS: deployment provenance (28 source hashes, 7 pinned deployed overrides, 1 config projection, 5 reasoned live-state entries)
PASS: candidate provenance (5 source hashes)
```

ISOLATED CHECKED-FILE COPY — exit **0**:

```text
PASS: deployment provenance (28 source hashes, 7 pinned deployed overrides, 1 config projection, 5 reasoned live-state entries)
```

REAL SOURCE DRIFT (_ccf.py) — exit **1**:

```text
FAIL: deployment provenance
  scripts/_ccf.py: expected ab4343efc7d1b82c5a4aca747d0e09c614779cfd9aa406a0dd681d204f5de001, got 8b59365afeaa748d7add5e92239a8b3bcc1f2d216a22aa69e37d73e362150362
```

CONFIG DRIFT OUTSIDE /lanes — exit **1**:

```text
FAIL: deployment provenance
  lanes.json (except /lanes): expected ed642a520d25470f1192cbbd88b4ccede16bebc6259a5bb1db039df45864cca6, got 20e249b3ddcd0448a16cba46c022d3e4571f49eecc8fcfc684a04c72c826bfed
```

EXPECTED OPERATIONAL /lanes DRIFT — exit **0**:

```text
PASS: deployment provenance (28 source hashes, 7 pinned deployed overrides, 1 config projection, 5 reasoned live-state entries)
```

UNCLASSIFIED SOURCE ADDITION — exit **1**:

```text
FAIL: deployment provenance
  unclassified file: scripts/unexpected-source.py
```

SOURCE EXCLUSION REJECTED — exit **2**:

```text
ERROR: live_state must name exactly the documented paths with reasons
```

ISOLATED COPY RESTORED — exit **0**:

```text
PASS: deployment provenance (28 source hashes, 7 pinned deployed overrides, 1 config projection, 5 reasoned live-state entries)
```

Scratch copies contained only checked immutable files and config. No live board,
ledger or state was copied to scratch/Git; all temporary changes were confined
to Files scratch and removed after the probes.

### Authorised live provenance publication and protection

Backup: `/media/jer/ZX20/CCF-Ops/deployment.json.bak-20261115T225732Z`.

- Live manifest before SHA-256: `22bb354343c699cc78903d4a5d1b1b13548e126a4a04351f54f25586905195ed`.
- Backup SHA-256: `22bb354343c699cc78903d4a5d1b1b13548e126a4a04351f54f25586905195ed` (byte-identical to before).
- Live manifest after SHA-256: `bc27de755194bd4c8ff34d24f55583db52376915301bfb22fabbb6405d7ca0e5`.
- Branch/live manifest byte equality: **PASS**.
- Post-publication audit using the live manifest: exit **0**, deployment and
  candidate provenance PASS.
- Whole live-tree before/after comparison: only existing `deployment.json`
  changed; only the required timestamped backup was added; nothing removed.
  All other observed live files, including tooling, config, board, ledger,
  state, guard/history and existing bytecode cache, stayed byte-identical.

[Publication receipt](ops-lease-persistent-identity-evidence-20261115/continuation/live-publication.json),
[before hashes](ops-lease-persistent-identity-evidence-20261115/continuation/live-before-hashes.json),
[after hashes](ops-lease-persistent-identity-evidence-20261115/continuation/live-after-hashes.json) and
[old manifest backup in evidence](ops-lease-persistent-identity-evidence-20261115/continuation/deployment-before.json).
The live `deployed_at` retains the historical baseline date; `recorded_at`
records this audit, and no new tooling deployment date is invented.

Primary remained at exactly its two pre-existing dirty files. Protected hashes
rechecked after the live publication:

- `AGENTS.md`: `1bf4286b3485de018759a5b9b8e19d74beb6385844b56315f22f71b7819897fd`.
- `Docs/Operations/StorageArchitecture.md`: `6bebe49511f310c0f97b000f2f5a6c8db6890bb77fd312b9ec47e47c900a5a22`.

[Protected hashes](ops-lease-persistent-identity-evidence-20261115/continuation/protected-primary-hashes.json). No Primary write,
game source/Assets access, save/PlayerPrefs/EditorPrefs access, board or
active-tasks write, Unity Editor launch, dependency change, forced lock action,
or manual guard/history deletion was performed.

### Explicitly unproven / still open

- The repository self-test and both requested lint forms are **not green**;
  source Files admission, config-relative board admission, the latent
  `git_stores` migration and portable test-device fixtures remain unresolved
  as classified above. No downstream lease/force/concurrency assertion was
  reached by these runs. The accepted earlier identity proof is not a new
  full-suite pass.
- Live board schema still fails its independent gate; operational validity is
  not implied by provenance validity.
- Five live immutable overrides have captured bytes/diffs, but are not
  integrated into this source candidate or independently source-approved.
  The original publication source of the two supplemental deployment documents
  remains unestablished. No historic full-deployment reconstruction is claimed.
- Candidate tools/config and verifier were not deployed to live; only the
  provenance file changed. This is not a drop-in tooling release.
- No reboot, physical UUID duplication/hotplug, Unity/gameplay/save test,
  independent review, Manager acceptance, integration, dashboard or Drive
  synchronization was performed. Receipt hashes detect drift from this
  observed baseline, not hostile coordinated alteration or all future state.
- #548 remains OPEN for the Manager; #544 orchestration and board authority
  are unchanged. DASHBOARD SYNC: NOT PERFORMED (no live board write authority);
  PUBLISHED DASHBOARD REFLECTS THIS TASK: UNVERIFIED, Manager follow-up.


## Round 2 — complete storage semantic repair (2026-11-15)

Direct implementation sub-agent continuation under #548 / orchestrator #544.
**The earlier classification (a) was wrong:** the rejected Files repository
was a missing storage-migration behaviour, not an inherited validator to retain.
This section supersedes the earlier continuation's outstanding storage-admission
conclusions without rewriting its historical evidence. No delegates, acceptance,
integration or tooling deployment. **#548 remains OPEN.**

Start: `4f85ad2aba15c57896fce5a62b7f70fbb0289303`, clean isolated worktree
`/media/jer/Files/CCF/Operations/CCF-OpsLeaseFix-20261115`, branch
`ops/unity-lease-persistent-identity-20261115-v1`. Retained packet context:
`46247486b37904f6b36825bb40fb6ed7bea35e88`. Latest direct user packet is
implementation authority; StorageArchitecture is storage authority. The live
`main` coordination record was read, not edited. Actual runtime is T3 Code /
Codex / gpt-6.1-sol high. No Unity Editor or game/source/save/prefs work.

Implementation commit: `a4a7e6cc3f2ee0563ef4bee38757cc00d3ba32b3`.
The following receipt/evidence commit is the publication head, recorded with
its full SHA in the #548 comment. All eight candidate source hashes are verified
against exact blobs at the implementation commit. Required gates are rerun at
the final publication head; their complete final output and that head are in
#548 and the external `round2/final-head-gates.json` receipt. This avoids a
self-referential commit hash in a file being committed.

### Complete live-versus-start semantic delta inventory

The entire three-script diff was inspected. This table covers every behavioural
hunk; imports/constants are included where they enable behaviour. No row is a
formatting or comment-only difference. [Raw initial script diffs](ops-lease-persistent-identity-evidence-20261115/round2/initial-script-diffs.json)
and [AST node/hash coverage plus complete remaining diff](ops-lease-persistent-identity-evidence-20261115/round2/semantic-delta-inventory.json)
make the surface independently inspectable.

| Script / behaviour at start | Live versus starting branch | Reason | Decision and final outcome |
|---|---|---|---|
| `_ccf.py`: Files root and `on_durable_ccf` | Only live defines the Files CCF boundary and resolves paths before checking containment. | Durable CCF storage moved to Files. | **PORT** constants/helper exactly; resolved siblings and symlink escapes remain rejected. |
| `_ccf.py`: `storage_check` default device | Live helper default is `/dev/sda2`; branch default is the persistent Netac by-id pin. Both config-driven paths use the configured persistent pin. | Kernel enumeration is unstable; this is the defect the durability repair prevents. | **EXCLUDE** live's stale default; retain the previously proved persistent default. No UUID/fstype/device/unique-UUID checks removed. |
| `_ccf.py`: repository admission | Live accepts ZX20 with `on_storage`, or Files CCF with `on_durable_ccf`; branch accepts only ZX20. Live explicitly rejects locations outside both. | The canonical bare repository legitimately moved. | **PORT** the union and explicit outside-both rejection; `on_project_storage` shares this bounded dispatch with the board. |
| `_ccf.py`: lane admission without permission | Live automatically accepts non-Unity lanes under Files CCF even when `permit_off_storage_lanes=False`; starting branch keeps all lanes ZX20-only by default. | Migrated NonUnity registration needs Files; direct packet requires explicit permission for off-ZX20 lanes. | **PORT WITH REQUIRED BOUNDARY** through the existing explicit inventory flag; **exclude** live's automatic default exception. Unity always remains ZX20-only. No registration imported. |
| `lane-status`: durable non-Unity inspection | Live imports `DURABLE_CCF` and inspects an existing Files non-Unity lane with the existing Git/du logic; branch skips all off-ZX20 lanes. | Legitimate non-Unity operations checkouts live on Files. | **PORT**; real Files Git fixture is inspected without changing its Git metadata bytes/mtimes. |
| `lane-status`: off-ZX20 Unity warning | Live says `STOP; writable Unity lane inspection skipped`; branch has a generic off-storage warning. | Unity must remain on ZX20. | **PORT** the warning/skip branch. Candidate config now rejects an off-ZX20 Unity registration before this defensive branch is reached. |
| `lane-status`: other off-storage warning | Live distinguishes an off-storage lane outside durable CCF and skips it; branch uses one generic warning. | Durable non-Unity is the sole intended exception. | **PORT** the defensive warning/skip branch; arbitrary paths now fail candidate config admission first. |
| `unity-lock`: all behaviour | No difference: same SHA-256 `54cf0a95561d0a8c350d89adbee83f393990b728d6127b94f69c587260f048e5`. | Shared storage admission fixes the lock's config failure. | **EXCLUDE changes**: no delta to port, lock algorithm/state universe/force rules unchanged. |

Additional necessary candidate changes, distinguished from live deltas:

- Both starting trees allow arbitrary paths when the inventory flag is true.
  The packet explicitly forbids that escape. Candidate now allows only resolved
  non-Unity paths under ZX20 or Files CCF with that flag. Default callers remain
  ZX20-only; the flag cannot admit Unity on Files, `/`, `/tmp`, a sibling prefix,
  or a symlink escaping either boundary.
- The config-relative board is admitted under the same bounded project-storage
  union, so this authorised Files worktree can run its own lint/lock. No board
  content is copied, changed or bypassed. Outside-both board paths still fail.
- `lanes.json.git_stores` changes only its two migrated `git_dir` values:
  `/media/jer/Files/CCF/Repository.git` and `/media/jer/Files/CCF-Legacy/.git`.
  Both were physically verified. Store names retain their existing values;
  labels do not determine identity. All four source lane registrations and
  every other config value are unchanged from the starting head.
- `lane-status` and `unity-lock` are now each byte-identical to live. Remaining
  `_ccf.py` differences are exactly the retained persistent default, bounded
  explicit lane permission, the shared path helper and board admission. There
  are no unexplained remaining differences in the requested scripts.
- No live board, lane registrations, ledger, state, README/environment or other
  operational content was imported. NonUnity's old source registration is
  deliberately retained, even though its old directory is absent. This is not
  a drop-in live configuration deployment.

### Five regression results after the port

[Repeatable boundary verification script](ops-lease-persistent-identity-evidence-20261115/round2/verify-storage-boundaries.py),
[all 75 checks with real commands/output](ops-lease-persistent-identity-evidence-20261115/round2/regression-probes.json),
and [all 18 retained identity checks](ops-lease-persistent-identity-evidence-20261115/round2/identity-verification.json).

| Regression | Result | Actual evidence / limit |
|---|---|---|
| Wrong UUID | **PASS: rejected** | `DEAD-BEEF`: status/acquire/release exit 2 with mount-identity error in candidate and isolated live-script copies; no test state/guard created. |
| Wrong fstype | **PASS: rejected** | `ext4` against physical exFAT: same 6 CLI refusals, no state created. |
| Nonexistent by-id device | **PASS: rejected** | `/dev/disk/by-id/CCF-NONEXISTENT-548`: same 6 CLI refusals, no state created. Wrong mount root is also rejected in 6 additional operations. |
| Repository and lane boundaries | **PASS** | Repository outside both trees rejected for `/`, `/tmp`, Files sibling prefix and escaping symlink; board escape rejected. Files non-Unity rejected by default, admitted only with explicit inventory permission; Files Unity rejected with either flag value; arbitrary/sibling/symlink lanes rejected with either flag value. Actual `lane-status` rejects `/` even though it enables inventory permission. |
| Positive identity and lock status | **PASS** | Physical by-id equivalence, unique UUID, exFAT, exact mount, rw/free capacity pass; branch and live `unity-lock status --json` each exit 0 with `status: free`, `metadata: null`, `unity_editor_pids: []`. No live acquire/release/force/steal. |

The earlier 18 checks also re-prove character/regular/missing-device refusal,
low space, read-only, wrong target, zero/duplicate UUID refusal (mocked UUID
visibility only), and parsing all four tools. The new boundary proof uses real
mounts and filesystem paths, including a real Files Git fixture. Negative CLI
copies contain only scripts/config, never operational board/ledger/state.

### Required gate output at the implementation head

Real commands from the task worktree's `CCF-Ops/` directory, stdout followed by
stderr without filtering. [Machine-readable commands, output and exits](ops-lease-persistent-identity-evidence-20261115/round2/gates-source-head.json).

`python3 -B tests/self-test.py` — exit **1**:

```text
Scratch: /media/jer/ZX20/CCF-Temp/foundation-test-8nrb0qwc
PASS: packet valid PASS
PASS: nonunity-lane-head PASS without Unity inference
PASS: canonical-style PASS without Unity inference
PASS: missing-unity-required FAIL
PASS: unity-nonunity-lane FAIL
PASS: stop-placeholder FAIL
PASS: handoff-placeholder FAIL
PASS: missing-role-file FAIL
PASS: draft missing Unity declaration WARN
PASS: packet valid Unity PASS
PASS: Unity lane is required
PASS: packet missing-base FAIL
PASS: packet missing-verification-tier FAIL
PASS: packet conflicting-unity-ownership FAIL
PASS: packet missing-stop-condition FAIL
PASS: partial draft WARN
PASS: partial final FAIL
PASS: draft does not hide explicit Unity conflict
PASS: malformed SHA FAIL
PASS: Markdown heading packet PASS
PASS: live board schema PASS
Traceback (most recent call last):
  File "/media/jer/Files/CCF/Operations/CCF-OpsLeaseFix-20261115/CCF-Ops/tests/self-test.py", line 75, in <module>
    rows=json.loads(cli('lane-status','--config',config,'--json'))
                    ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
  File "/media/jer/Files/CCF/Operations/CCF-OpsLeaseFix-20261115/CCF-Ops/tests/self-test.py", line 34, in cli
    def cli(name,*args,code=0): return run([sys.executable,'-B',OPS/'scripts'/name,*args],code)
                                       ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
  File "/media/jer/Files/CCF/Operations/CCF-OpsLeaseFix-20261115/CCF-Ops/tests/self-test.py", line 31, in run
    if p.returncode!=code: raise AssertionError(f'expected {code}, got {p.returncode}: {args}\n{p.stdout}\n{p.stderr}')
                           ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
AssertionError: expected 0, got 2: ['/usr/bin/python3', '-B', PosixPath('/media/jer/Files/CCF/Operations/CCF-OpsLeaseFix-20261115/CCF-Ops/scripts/lane-status'), '--config', PosixPath('/media/jer/ZX20/CCF-Temp/foundation-test-8nrb0qwc/lanes.json'), '--json']

ERROR: ZX20 mount identity mismatch: {'source': '/dev/sdc2', 'target': '/media/jer/ZX20', 'fstype': 'exfat', 'uuid': '18D6-896E'}

```

`python3 -B scripts/packet-lint --board board/active.md` — exit **0**:

```text
PASS: board schema and duplicate checks (15 live tasks)
```

`python3 -B scripts/packet-lint tests/fixtures/valid.md` — exit **0**:

```text
PASS
```

Classification: the storage-migration/config-admission defect is **fixed**.
The unchanged self-test passes **21 assertions**, including its branch-board
schema check, then fails at line 75 because its isolated fixture's line 70
still pins `/dev/sda2`; actual ZX20 is `/dev/sdc2`. This is an **inherited stale
test-device fixture**, not a product identity failure or reason to weaken
validation. The self-test was not edited. No fixture lock/force/concurrency
section was reached, so the packet's no-force limit was preserved.

Static downstream limitations (not executed failures): lines 78–79/110 also
hardcode `/dev/sda2`; line 99 hardcodes `/dev/sdb2`. Its `/` lane sentinel expects
arbitrary off-storage inventory, which conflicts with this packet's required
bounded permission and would need separately reviewed test repair. The source
`environment.sh` still references historical ZX20 output directories; it was
not changed or executed by this worker. The existing preflight readiness rule
still requires ZX20 even for a durable non-Unity inventory row; this worker
ported the live inventory delta, not assignment policy. None is silently green.

The branch board has no inherited `Unity required?` content defect: its
unchanged 15-task table passes. The separate **live** read-only board gate is
still #553/Manager operational state, not imported or edited:

```text
FAIL: Unity required? must be Yes/No
```

Exit **1**. [Exact live lint receipt](ops-lease-persistent-identity-evidence-20261115/round2/live-board-lint.json).
No board schema gate was bypassed. The stopped self-test's own fixture files
were hash-inventoried and removed only after checking that no guard, history
or lock existed; production state and Git metadata were untouched.

### Provenance, publication and protection

The five reasoned live-state exclusions are unchanged. All **28 historical
source hashes**, **7 pinned live immutable overrides**, and the config projection
of every key except `/lanes` are retained. No live immutable hash is refreshed
to hide drift; only explanatory override reasons and the separate candidate
receipt change. Candidate now pins **8 files** (the previous five plus unchanged
unity-lock, packet-lint and self-test) to the implementation commit.
[All 36 exact Git-blob checks](ops-lease-persistent-identity-evidence-20261115/round2/source-blob-verification.json)
pass. The candidate remains **NOT_DEPLOYED**.

Both-way provenance proof, rerun after the port (all eight expected outcomes
match):

LIVE READ-ONLY AUDIT — exit **0**:

```text
PASS: deployment provenance (28 source hashes, 7 pinned deployed overrides, 1 config projection, 5 reasoned live-state entries)
PASS: candidate provenance (8 source hashes)
```

ISOLATED CHECKED-FILE COPY — exit **0**:

```text
PASS: deployment provenance (28 source hashes, 7 pinned deployed overrides, 1 config projection, 5 reasoned live-state entries)
```

REAL SOURCE DRIFT (_ccf.py) — exit **1**:

```text
FAIL: deployment provenance
  scripts/_ccf.py: expected ab4343efc7d1b82c5a4aca747d0e09c614779cfd9aa406a0dd681d204f5de001, got 8b59365afeaa748d7add5e92239a8b3bcc1f2d216a22aa69e37d73e362150362
```

CONFIG DRIFT OUTSIDE /lanes — exit **1**:

```text
FAIL: deployment provenance
  lanes.json (except /lanes): expected ed642a520d25470f1192cbbd88b4ccede16bebc6259a5bb1db039df45864cca6, got 20e249b3ddcd0448a16cba46c022d3e4571f49eecc8fcfc684a04c72c826bfed
```

EXPECTED OPERATIONAL /lanes DRIFT — exit **0**:

```text
PASS: deployment provenance (28 source hashes, 7 pinned deployed overrides, 1 config projection, 5 reasoned live-state entries)
```

UNCLASSIFIED SOURCE ADDITION — exit **1**:

```text
FAIL: deployment provenance
  unclassified file: scripts/unexpected-source.py
```

SOURCE EXCLUSION REJECTED — exit **2**:

```text
ERROR: live_state must name exactly the documented paths with reasons
```

ISOLATED COPY RESTORED — exit **0**:

```text
PASS: deployment provenance (28 source hashes, 7 pinned deployed overrides, 1 config projection, 5 reasoned live-state entries)
```

[All actual provenance probe commands/output](ops-lease-persistent-identity-evidence-20261115/round2/provenance-probes.json).
No source/config mismatch is excused as mutable lane state.

Only `deployment.json` was published to live, with backup:

- Backup: `/media/jer/ZX20/CCF-Ops/deployment.json.bak-20261115T230935Z`.
- Before and byte-identical backup SHA-256: `bc27de755194bd4c8ff34d24f55583db52376915301bfb22fabbb6405d7ca0e5`.
- After SHA-256: `2034c77fac082ec8107deef073605ce9314d594e79568e28f32aa8584dc4f5bc`.
- Whole live-tree hashes: only `deployment.json` changed, only the required
  backup was added, nothing removed; all tooling/config/board/ledger/state,
  permanent guard and history retain their exact bytes.
- Branch/live provenance bytes match; audit using the published live manifest
  passes deployment and all eight candidate hashes.
- Primary's two dirty-file hashes and branch board/active-tasks hashes remain
  identical to the start. Primary was never written to.

[Publication and post-publication audit](ops-lease-persistent-identity-evidence-20261115/round2/live-publication.json),
[whole live tree before](ops-lease-persistent-identity-evidence-20261115/round2/live-before-hashes.json),
[after](ops-lease-persistent-identity-evidence-20261115/round2/live-after-hashes.json),
[protected before](ops-lease-persistent-identity-evidence-20261115/round2/protected-before-hashes.json),
[after](ops-lease-persistent-identity-evidence-20261115/round2/protected-after-hashes.json).
Full operational evidence: `/media/jer/Files/CCF/Operations/OpsLeaseEvidence-20261115/round2/`.

### Exactly what remains unproven / open

- The **full self-test is not passing**: stale device fixtures stop execution
  before lane/storage/lease/concurrency/pilot sections. Their assertions were
  not relaxed, skipped or rewritten. No new occupied/foreign-owner/normal
  acquire/release/concurrent-acquire proof is claimed by this round; earlier
  normal-cycle evidence remains historical. No force or steal occurred.
- The mandatory bounded inventory permission intentionally rejects the
  self-test's later arbitrary-root sentinel. Repairing that harness expectation,
  portable device fixtures, historical environment outputs, and durable-lane
  assignment preflight remains outside this bounded source port and unverified.
- The **live board still fails** its independent content gate (#553). Provenance
  PASS is not board validity. There was no live board/active-tasks write.
- No candidate scripts/config were deployed; applying candidate helpers to
  live registrations is not qualified. Strict default admission would reject
  a Files lane in callers that do not explicitly enable inventory permission;
  operational deployment would need its own reviewed caller/config plan.
- No reboot/re-enumeration/hotplug or physical duplicate UUID was induced.
  Current kernel-name versus by-id equivalence is physical; duplicate UUID
  refusal uses a mock. Editor scans/status snapshots observed zero Editors;
  they are not kernel-wide continuous proof about unrelated processes.
- Independent G2/G3/G4 review, integration, Manager acceptance, Unity/gameplay/
  save qualification, dashboard/Drive sync and historical provenance of the
  two supplemental deployed documents remain unproven. Hashes detect drift
  against the observed baseline, not coordinated hostile receipt alteration.
- **#548 stays OPEN.** DASHBOARD SYNC: NOT PERFORMED (no board write authority).
  PUBLISHED DASHBOARD REFLECTS THIS TASK: UNVERIFIED. Manager/#544 follow-up;
  this source/evidence publication confers no physical ownership or acceptance.


## Final round — portable self-test device assertions; residual inherited boundary fixture

Direct user packet under #544/#548; implementation worker, no delegates.
Implementation commit: `60f97b06bd41f9a89fde4eb775527ebc6947560b`.
Starting publication head: `4d24f3b1535741bcef142d72d162d6f818bc4133`.
**#548 stays OPEN. Self-test remains FAIL; both requested source lint gates PASS.**
No production helper, live script/config, game source, board or active-tasks
change. No Editor, force, steal, production acquisition/release or Primary access.
The publication head follows this evidence update; exact final-head gate output
is preserved externally and posted verbatim on #548 after push/readback.

### Exact bounded test repair and retained coverage

[Exact self-test unified diff](ops-lease-persistent-identity-evidence-20261115/final-round/self-test.patch).
Only `CCF-Ops/tests/self-test.py` changes executable test behavior:

- After the existing persistent-pin `storage_check()` succeeds, derive `DEVICE`
  from `findmnt SOURCE` for the guarded `ROOT`. UUID `18D6-896E`, exfat, mount
  target, visible UUID uniqueness and minimum capacity checks remain mandatory.
- The clean/absent-lane assertions require their existing ZX20 boolean AND a
  real block device whose `st_rdev` equals the guarded root's `st_dev`.
  This independently checks physical identity without assuming `/dev/sdX` text.
- The existing off-storage assertion likewise checks the actual `/` filesystem
  device and explicitly rejects the ZX20 device. Its path, expected success,
  STOP warning and HEAD assertions are unchanged, so its stale acceptance
  expectation is exposed rather than adjusted to fit current production rules.
- The read-only mount fixture uses the same verified live source; its expected
  `READ ONLY` result remains unchanged. Historical documents are untouched.

[Exact-helper negative verification](ops-lease-persistent-identity-evidence-20261115/final-round/verify-fixture-devices.py)
loads only the helper's AST from this self-test, without executing the harness.
**22 checks PASS**: correct ZX20 block accepted; different actual root block,
character/regular/nonexistent devices rejected; root identity independently
verified; storage_health rejects wrong physical device/UUID/fstype/nonexistent
pin. Wrong physical-device status/acquire/release probes on candidate and
isolated live script copies each exit 2 with mount-identity mismatch before
creating any state/guard. Production/live config never changes.

Retained verification also passes **75 boundary checks + 18 identity checks**.
Wrong UUID, wrong fstype, absent by-id and wrong mount root each reject
status/acquire/release on candidate and isolated live copies (exit 2, no state).
Repository/lane sibling and symlink escapes remain rejected; Files non-Unity
admission still needs explicit permission; Files Unity stays forbidden.
Positive physical identity/capacity and candidate/live FREE status, null
metadata, zero Editor PIDs hold. Low space, read-only and mocked absent/duplicate
UUID remain blocked. These are focused results, not full lease qualification.

### Exact residual classification

Self-test now passes **33** assertions, including both repaired lane-device
assertions and Git-metadata byte/mtime preservation. It stops at line **108**:
`off_rows=json.loads(cli('lane-status','--config',config,'--json'))` expects exit
0, receives exit 2 and `ERROR: path is outside the Files CCF tree: /`.
The old fixture at line 106 appends `/` as a non-Unity lane. Round 2's required
bounded inventory rule correctly rejects that path even with permission.

**Classification: inherited test debt (stale arbitrary-root acceptance
expectation), not environment failure or live-board state.** This fixture
already existed at starting head and remains unchanged. The final packet
explicitly requires legitimate residual failures be documented and left failing;
no path substitution, skip, expected-exit relaxation or production admission
change was made. The `/` rejection is separately re-proved by the retained
boundary verifier. Subsequent storage/lease/force/concurrency/pilot self-test
sections are **NOT REACHED**. There is no claim they pass.

The independently read live board still returns exit 1,
`FAIL: Unity required? must be Yes/No` (#553/Manager scope). Source board lint
passes against the unchanged 15-row source snapshot; its self-test label
`live board schema PASS` refers to that source board. Neither board was edited.

### Verbatim required gates at implementation head

Working directory: task worktree `CCF-Ops/`.
[Full stdout/stderr/exits](ops-lease-persistent-identity-evidence-20261115/final-round/gates-source-head.json).

`python3 -B tests/self-test.py` — exit **1**:

```text
Scratch: /media/jer/ZX20/CCF-Temp/foundation-test-ekt27myh
PASS: packet valid PASS
PASS: nonunity-lane-head PASS without Unity inference
PASS: canonical-style PASS without Unity inference
PASS: missing-unity-required FAIL
PASS: unity-nonunity-lane FAIL
PASS: stop-placeholder FAIL
PASS: handoff-placeholder FAIL
PASS: missing-role-file FAIL
PASS: draft missing Unity declaration WARN
PASS: packet valid Unity PASS
PASS: Unity lane is required
PASS: packet missing-base FAIL
PASS: packet missing-verification-tier FAIL
PASS: packet conflicting-unity-ownership FAIL
PASS: packet missing-stop-condition FAIL
PASS: partial draft WARN
PASS: partial final FAIL
PASS: draft does not hide explicit Unity conflict
PASS: malformed SHA FAIL
PASS: Markdown heading packet PASS
PASS: live board schema PASS
PASS: lane clean
PASS: lane ahead/behind local refs
PASS: lane verifies backing device and ZX20 YES
PASS: absent lane reports nearest-parent ZX20 device
PASS: lane dirty is data, exit zero
PASS: lane missing tolerated
PASS: lane-status leaves Git metadata bytes/mtimes unchanged
PASS: malformed duplicate lane config fails clearly
PASS: malformed JSON fails clearly
PASS: malformed config shape fails clearly
PASS: malformed config shape fails clearly
PASS: malformed config shape fails clearly
Traceback (most recent call last):
  File "/media/jer/Files/CCF/Operations/CCF-OpsLeaseFix-20261115/CCF-Ops/tests/self-test.py", line 108, in <module>
    off_rows=json.loads(cli('lane-status','--config',config,'--json'))
                        ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
  File "/media/jer/Files/CCF/Operations/CCF-OpsLeaseFix-20261115/CCF-Ops/tests/self-test.py", line 44, in cli
    def cli(name,*args,code=0): return run([sys.executable,'-B',OPS/'scripts'/name,*args],code)
                                       ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
  File "/media/jer/Files/CCF/Operations/CCF-OpsLeaseFix-20261115/CCF-Ops/tests/self-test.py", line 41, in run
    if p.returncode!=code: raise AssertionError(f'expected {code}, got {p.returncode}: {args}\n{p.stdout}\n{p.stderr}')
                           ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^
AssertionError: expected 0, got 2: ['/usr/bin/python3', '-B', PosixPath('/media/jer/Files/CCF/Operations/CCF-OpsLeaseFix-20261115/CCF-Ops/scripts/lane-status'), '--config', PosixPath('/media/jer/ZX20/CCF-Temp/foundation-test-ekt27myh/lanes.json'), '--json']

ERROR: path is outside the Files CCF tree: /

```

`python3 -B scripts/packet-lint --board board/active.md` — exit **0**:

```text
PASS: board schema and duplicate checks (15 live tasks)
```

`python3 -B scripts/packet-lint tests/fixtures/valid.md` — exit **0**:

```text
PASS
```

### Final provenance audit in both directions

Live read-only audit returns exit 0:

```text
PASS: deployment provenance (28 source hashes, 7 pinned deployed overrides, 1 config projection, 5 reasoned live-state entries)
PASS: candidate provenance (8 source hashes)
```

All eight existing provenance probes matched: pristine live/copy PASS;
deliberate `_ccf.py` source drift FAIL (exit 1); config drift outside `/lanes`
FAIL (1); expected lane-registration drift PASS; unclassified source FAIL (1);
attempted source exclusion rejected (2); restored copy PASS. An additional
isolated candidate `tests/self-test.py` tamper fails (exit 1), proving the
repaired test is still enforced by candidate provenance. All **36** historical
and candidate source hashes match their exact recorded Git blobs.

No immutable live hash or exclusion was changed. The candidate receipt now
pins the eight candidate files at the implementation commit, including the
new self-test hash, and remains `NOT_DEPLOYED`. Only the live provenance file
was published with a byte-identical timestamped backup:

- Backup: `/media/jer/ZX20/CCF-Ops/deployment.json.bak-20261115T231825Z`.
- Before/backup SHA-256: `2034c77fac082ec8107deef073605ce9314d594e79568e28f32aa8584dc4f5bc`.
- After SHA-256: `8c66b290c247708846cb8b59f83626a10a3fa1d052397a3316dc5f23fc157b18`.

[Whole-tree protection evidence](ops-lease-persistent-identity-evidence-20261115/final-round/protection.json)
proves only existing `deployment.json` changed and its backup was added; no
other live file changed or disappeared. Branch board/active-tasks/historical
verification document/config and production scripts remain byte-identical.
Primary and its dirty files were not accessed in this round. The stopped
self-test's own scratch was inventoried, checked for absence of lock/guard/history,
then removed; no other scratch or existing operational data was removed.

### Remaining unproven / handoff

Self-test exit 0 is **NOT achieved**. The legitimate inherited `/` fixture
failure is deliberately left visible as required. Downstream self-test
storage/lease/occupied-owner/concurrency/pilot behavior remains unproven.
No force operation ran. No reboot/hotplug/physical UUID duplication test,
independent acceptance/review/integration, candidate deployment, Unity/game/save
qualification, or dashboard/Drive synchronization is claimed. Live board #553
validity remains a separate FAIL. Original publication source of two supplemental
live documents remains unestablished. Device observations are snapshots, not
continuous process or hotplug monitoring.

DASHBOARD SYNC NOT PERFORMED (no board write authority); published dashboard
state UNVERIFIED, Manager/#544 follow-up. #548 stays OPEN. Final publication-head
receipts and remote readback live at
`/media/jer/Files/CCF/Operations/OpsLeaseEvidence-20261115/final-round/`;
source-head receipts and the exact diff are committed alongside this result.
