# CCF-Ops persistent identity durability result — 2026-11-15

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
