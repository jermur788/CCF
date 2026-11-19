# Sen-1 #781 A1 — physical identity and deployed Ops readback root cause

**G0 diagnosis and documentation only. G1 HELD. G2 HELD. Nothing installed or operationally changed.**

## Actor, placement and authority

A1 research child, T3 Code through the Codex harness, provider Codex/OpenAI, runtime-reported model **gpt-6.1-sol**, reasoning effort **high**. Host independently observed: **jer-laptop**, Linux 7.0.0-42-generic, user jer/uid 1000. Process-visible `CODEX_THREAD_ID=01a1f156-cdbe-7d11-bc51-125cff22b786`; no `T3_THREAD_ID` was exposed. The current T3 delegated-thread link is UNKNOWN here; the parent must register the re-dispatch's actual link rather than reusing the stopped dispatch's identity. Model identity is runtime-provided, not inferred from the task's preferred model. Token, cost and quota consumption: UNKNOWN.

Only writable report: `Docs/Handoffs/ScenarioOne-Operations-Readiness-20261118/Sen-1-781-A1-PHYSICAL-IDENTITY-AND-OPS-READBACK-ROOT-CAUSE-20261118.md`.
Actual checkout: `/media/jer/Files/CCF/Operations/CCF-781-A1-PhysicalIdentity-20261118`.
Branch: `docs/sen-1-781-a1-physical-identity-20261118-v1`.
Starting HEAD/base, local origin/main and freshly queried GitHub main: **a69b3e2e90829505c792d95f5a3eba3e7d8d8132**. HEAD at evidence/prewrite remained that base; the report's eventual commit cannot contain its own commit SHA and is supplied in the handoff.

The mandatory placement command was the first shell invocation. It passed: exact realpath, Files /dev/sdb4 ext4, exact branch/base, clean tree. The failed earlier dispatch is historical and made no output; this dispatch did not create, switch, reset, stash, clean, merge or relocate any checkout. No standalone `git worktree` operation was issued; the requested full lane-status internally invokes read-only Git worktree inventory.

Read before writing, in the requested order: binding packet in full via GitHub contents API/ref, primary AGENTS and AgentWorkflow, local Decision Log, StorageArchitecture, then #781 parent/child/dispatch comments. The placement-correction comment supersedes the initial claim's unused-candidate-path wording:
- [Binding G0 packet](https://github.com/jermur788/CCF/blob/docs/sen-1-731-chas-gpt6-manager-20261118-v2/Docs/Handoffs/ScenarioOne-Operations-Readiness-20261118/Sen-1-781-TG-FORK-OPS-LANE-IDENTITY-AND-COORDINATION-RECOVERY-PACKET-20261118.md).
- [Parent claim](https://github.com/jermur788/CCF/issues/781#issuecomment-6996330217), [A1 claim](https://github.com/jermur788/CCF/issues/781#issuecomment-6996339761), [initial dispatch](https://github.com/jermur788/CCF/issues/781#issuecomment-6996384005), [placement correction/re-dispatch](https://github.com/jermur788/CCF/issues/781#issuecomment-6996473911).
- Fresh main coordination and D-091, #731/#777/#778/#652 refreshed; local project scope/vision/overview read. Local primary's older Decision Log does not contain D-091; live main does. The newer direct owner policy and packet control.
- [Owner no-root directive](https://github.com/jermur788/CCF/blob/docs/sen-1-731-chas-gpt6-manager-20261118-v2/Docs/Handoffs/ScenarioOne-Operations-Readiness-20261118/Sen-1-731-OWNER-NO-ROOT-FILESYSTEM-DEVELOPMENT-STORAGE-DIRECTIVE-20261118.md) read in full. No root scratch was used. Python ran with -B (the inline owner parser also imports under -B). No cache relocation, credentials copy, global settings change or extra scratch directory was made.

The primary checkout is **ops/workflow-foundation-v1 @ a42eaadc42bda6d474365f39b67a78a9b14bb043**, ahead 1/behind 1 of its branch upstream. Its three modified files and four untracked status entries (archive directory included) are unrelated preserved user work, listed verbatim below. The seven protected file hashes match the parent's published prefixes. No gameplay, architecture, package, scene, asset, save or Unity change is proposed for this lane. The smallest change is this report alone.

## Root-cause statement

The reported mismatch combines **different copies and different time-bound observations**. It is not evidence that today's ZX20 mount is wrong.

1. Today's mounted ZX20 is the Netac external SSD partition **/dev/sdc2**, exfat, label ZX20, UUID **18D6-896E**. Its persistent by-id pin resolves to that same block device. **/dev/sda2 is root**, on the Samsung internal SSD; Files and Videos are separately mounted ext4 partitions on the **same HGST physical HDD**, not separate physical disks.
2. The **deployed** lanes.json already pins the Netac by-id path and the **deployed** _ccf.py compares block-device numbers, exact mount target, UUID, fstype and unique visible UUID. Its storage-only command succeeds. The **primary repository copy** still compares literal /dev/sda2 and contains retired repository/NonUnity/legacy paths. It fails at storage_check before those later paths are evaluated. A kernel /dev/sdX naming change explains why a literal historical pin becomes invalid; it does not authorise changing that pin to another current /dev/sdX name.
3. Deployed status sees no lock.json and no process whose argv[0] basename is exactly Unity, so its read logic reports free. That is a lock/process snapshot, **not** a lane reservation release or execution permit. The unmodified status command can open/create a guard with write access, so A1 did **not** execute it unmodified. A write-denying in-memory wrapper reproduced the readback without changing installed source/state. This is a deliberate verification limitation, not a literal unmodified CLI PASS.
4. Full lane-status has sequential recursive `du -sh` calls and gathers all lane rows before printing. One bounded run reached **55 seconds without a report**; not repeated. The parent's completed full run is reported evidence only. A1 confirms the expensive code path and observed delay, **not completion** or a measured per-lane time breakdown.
5. The receipt is an old **observational provenance** record, not a deploy authorisation. Its non-lane configuration projection still matches. Current lanes have **25 entries**, not the dispatch's reported 20 or the receipt's descriptive 16. The board has **44 table rows**. These are independent observations, not grounds to remove entries. Current lane edits are outside the receipt's lane-excluding hash by design.
6. Board semantics explain misleading ownership labels: lane-status uses exact Lane-string matches and returns free-form Owner text even for completed/blocked rows; no match prints unassigned. It does not search authoritative main, issue comments, narrative board reservations or physical process custody. Its ordinary exit 0 means storage ready, not every lane ready.

## Current physical storage

All facts are timestamped 2026-11-19 UTC. Exact command output follows in the evidence appendix.

| Role | Actual mount/device | Identity | Available bytes at 01:07Z | Interpretation |
|---|---|---|---:|---|
| root | / on /dev/sda2 | Samsung MZNLN256HCHP-00000, S1ZWNAAH302842; ext4; d393b49a-24aa-4f88-b8c4-5121b96c240d; no label | 36,594,888,704 | OS root; prohibited for new CCF development |
| ZX20 | /media/jer/ZX20 on /dev/sdc2 | NetacPortableSSD, AA20260702512G231151; exfat; 18D6-896E; ZX20 | 367,830,237,184 | Active-work SSD; live Ops threshold 16,106,127,360 bytes |
| Files | /media/jer/Files on /dev/sdb4 | HGST HTS721010A9E630, JR10004M1U3BGE; ext4; 2283b135-bf37-42d6-921f-2fb861d0a8d4; Files | 39,880,065,024 | Durable CCF HDD partition; above 10 GiB floor |
| Videos | /media/jer/Videos on /dev/sdb1 | Same HGST disk as Files; ext4; 60e276b9-4abb-4325-a913-f4c43c14294b; Videos | 443,045,425,152 | Verified non-root separate mount; no CCF lane allocated |

No mount, filesystem repair, label change, sudo or storage mutation was attempted. Historical StorageArchitecture device names (/dev/sda2 ZX20, /dev/sdc4 Files) are dated projections, not today's device truth. Current hardware identity supports kernel enumeration change as the explanation for the old pin; the exact boot/cable/enumeration history and actor responsible for each past config change remain UNKNOWN. No evidence of an actual filesystem UUID relabel was found; absence of such a historical event is not proven.

## Deployed/source/scratch provenance

Aliases used throughout the evidence:
- **D** = /media/jer/ZX20/CCF-Ops, installed and authoritative for currently executing bytes only.
- **R** = /media/jer/Files/CCF-Primary/CCF-Ops, the old tracked ops/workflow-foundation-v1 copy, read-only.
- **A** = /media/jer/Files/CCF/Operations/CCF-OpsReadiness-652-20261116/CCF-Ops, clean source branch ops/sen-1-652-unity-lease-readiness-20261116-v1, HEAD **4c7acc051e262c3fe301049b51ee7fc48a9f99a2**. Source approval applies to exact reviewed source, not installation.
- **F/O** = /media/jer/ZX20/CCF-Temp/ops652-final-source and ops652-optiona-source, scratch variants, not deployment authority.

D's repository is /media/jer/Files/CCF/Repository.git; git_stores are that Files-canonical store plus /media/jer/Files/CCF-Legacy/.git (legacy-SSD is its historical name, not today's physical medium). Board resolves to /media/jer/ZX20/CCF-Ops/board/active.md. The full deployed storage block, all 25 paths/purposes/unity flags, both stores and the receipt are reproduced below. CCF-Ops is tracked on the primary's ops branch but **not tracked at all on the freshly checked origin/main** (empty ls-tree output). Do not describe R as main's installed Ops source.

Every D file read/hash-inspected is pinned in E06, including derived bytecode, operational state and receipt backups. All **50** D files were rehashed unchanged at 01:11:44Z; sorted compact-JSON manifest SHA256 **47a610882a895c9ec59ecd3c1f4bd19c4e16eec353b157a63588fc9dc110bf12**. Individual critical pins:
- lanes.json: **c1d147196e1b8d57008e44545db3ba7f3fecea0ae12094dbf3e05f1c782b7d9e**
- deployment.json: **8c66b290c247708846cb8b59f83626a10a3fa1d052397a3316dc5f23fc157b18**
- scripts/_ccf.py: **ab4343efc7d1b82c5a4aca747d0e09c614779cfd9aa406a0dd681d204f5de001**
- scripts/lane-status: **e7102a83ad942cc7365cb22db9a8f79823744ea98bc2cf8a6738a11f1897173f**
- scripts/unity-lock: **54cf0a95561d0a8c350d89adbee83f393990b728d6127b94f69c587260f048e5**
- board/active.md: **4fa750eba64a8a072feb305a7db2cce17ddedfa62dbd276305116c03ab853994**
- state/unity/history.jsonl: **dfb506443e1f41b6d5f2be23e2c2b38d0ee76fe78d4d4ba2446fbc962422fffa**

E06 lists **every** differing file D↔R, D↔A, R↔A, A↔F and A↔O, with both sides' full hashes or ABSENT. Operational-only files and derived caches are included, not silently hidden. F's name “final” is not proof it is the approved current source: six files differ from A, including its old unity-lock and _ccf.py. O differs from A in README, unity-operator role, unity-lock and tests. D lane-status equals A's lane-status; D unity-lock equals R's old unity-lock, not A's no-expiry version.

E15 separately inventories the two source scratch copies, thirteen retained base/replica snapshots and two five-file staged projections. Their exact locations, manifest hashes and D differences are recorded. The two staged five-file projections are identical (manifest **9e23d2fd8dfec36dc2cad8df6434797c160bea8cdd8bc43b3e0637a5d3bc2702**), with approved no-expiry unity-lock and Option A _ccf.py plus merged README. They are **not installed**. The 50-file base/replicas share manifest **268cfdf6a894b49274755fe83c4b5d96ee18b57b85cb82a63b1cd91bd0ba0c13** and differ from D only in board/lanes.

A broader read-only depth-six scratch scan also encountered intentionally partial/mutant rollback-test trees under #754 work/cases. Tool output was truncated, so no exhaustive all-scratch provenance or acceptance claim is made from it; it was not repeated. The focused retained-source/projection inventory is complete for its stated glob scope. Deeper/unmatched scratch content is UNKNOWN, not an alternative source to install.

### What schema-2 deployment.json proves and does not prove

Receipt source_head **dfe445e54d099a0e20c9c7d5a9e3e6f195968b4e**, source branch ops/workflow-foundation-v1 and deployed_at 2026-10-15T00:20:00+00:00 are historical baseline fields. recorded_at is **2026-11-15T23:18:25.883045+00:00** and provenance_status is **RECONCILED_OBSERVATION_NOT_SOURCE_ACCEPTANCE**. Seven deployed_overrides pin observed divergent docs/environment/_ccf.py/lane-status with reasons; they do not attribute those bytes to an accepted source commit or invent a deployment date.

configuration_hashes hashes canonical compact sorted JSON after deleting only /lanes. Actual **ed642a520d25470f1192cbbd88b4ccede16bebc6259a5bb1db039df45864cca6** matches. Thus current storage/repository/git_stores/board and all other non-lane keys match the recorded projection. **No unrecorded non-lane configuration change is demonstrated.** Whole-file lanes hash/mtime can change while this passes. D lanes.json mtime is **2026-11-18 17:28:31.510000000 +0000**, receipt mtime **2026-11-15 23:18:25.880000000 +0000**: it is incorrect to present that old receipt as a contemporaneous complete lane registry/ownership audit. Changed lane registrations since the receipt are expected live state, but every transaction's actor/permit is not established by the hash.

candidate.source_head **60f97b06bd41f9a89fde4eb775527ebc6947560b**, worktree CCF-OpsLeaseFix-20261115, branch ops/unity-lease-persistent-identity-20261115-v1, status **NOT_DEPLOYED**, deployed_at null, is a separate eight-file candidate snapshot. It is neither A's current 4c7acc05 source nor proof D has those candidate bytes. Candidate checking is optional in verify-deployment; this run deliberately audited the live root with the live receipt, not the stale candidate block.

The read-only verifier is present in A and absent in D. Its E07 run returned PASS (28 source hashes, 7 pinned overrides, 1 config projection, 5 reasoned live-state entries). It verifies recorded bytes/classification, **not** owner authorisation, historical provenance beyond the receipt, board correctness, runtime behaviour, D-091 compliance or no-install risk acceptance. The live_state exclusions explicitly include board, ledger, state, lane registrations and pycache; these are not validated live custody by this PASS. Receipt metadata is not self-hashed.

## Code paths, writes and ownership interpretation

D _ccf.py: command() disables optional Git index locks and sets PYTHONDONTWRITEBYTECODE for subprocesses; same_device() compares st_rdev of block devices. storage_check() enforces source/target/fstype/UUID and unique UUID. storage_health() also checks rw and capacity. config_read() validates those before status, admits durable Files non-Unity lanes, and constrains board to ZX20. Its unused default device still says /dev/sda2; config_read passes the correct config pin explicitly. A already changes that default to persistent by-id and has stricter project-path admission.

D lane-status lines 20–22 return storage-only before board/store/lane inventory. Lines 23–61 parse board, inventory Git stores, then run sequential du/Git inspection per lane. Line 34 uses only exact Lane matches; line 35 copies Owner text without filtering DONE-PENDING-LEDGER or NONE. Lines 62–68 print after the loop. Ordinary line 74 exit depends on storage only; --preflight adds assignment warnings but is not an external source-start permit. No mkdir, board/state write or cache creation path exists in these read-only status branches. Git subprocesses have GIT_OPTIONAL_LOCKS=0.

D unity-lock: editors() scans /proc cmdline for basename Unity (not Unity Hub, arbitrary renamed executables or other hosts). status on a missing state directory returns without creating it. Here the directory exists, so line 107 enters guarded(): mkdir(exist_ok=True), guard.open('a+'), exclusive flock. It can create missing guard even for status, and it is not a strict no-write interface. On status it does not append history, write lock.json or write the board; acquire/release have separate metadata/history writes. A1 replaced only mkdir with an existing-directory check and guard a+ with read-only r **in memory**, and installed an audit hook rejecting writable opens/mutations. The wrapper still uses the original guard inode/flock and original status/config/process/read logic. The exact wrapper/output is recorded, so this cannot be confused with unmodified CLI verification.

Both entry scripts set sys.dont_write_bytecode=True **before importing _ccf**; all A1 invocations additionally used python3 -B. The existing scripts/__pycache__/_ccf.cpython-312.pyc was present before status (SHA256 **900d02566195f6a0e1487edd34dd11f9fc6347a6869b87f4a19f106934545da1**) and unchanged. Running these entry points as inspected should not create pycache; directly importing _ccf without -B/PYTHONDONTWRITEBYTECODE or importing before setting the flag can create it. Its mere preexistence does not identify a responsible actor.

D requires a future ETA on acquisition and does not support A's --no-expiry/null-ETA mode. It reports ETA overdue without auto-release. A stale PID is inspection data; lock remains until explicit matching release or separately authorised force path. **D-091 prohibits arbitrary expiry/reclaim/forced clearing; none was performed.** No acquisition, release, force, source suite, installation or Unity process was run.

### Board and owner readback

At 01:09:03Z state/unity contains only guard/history.jsonl; lock.json is absent. Read-only wrapped status at 01:08:35Z: status free, metadata null, unity_editor_pids []. History contains **415 records**, 2026-10-16 through **2026-11-15T22:44:25.912374+00:00**; last is normal release by **Codex-Ops548-durability**. E10 lists all distinct historical actors and all 44 board Owner fields; history is not a live owner list.

Direct board matches exist for U2, U3, ResearchCopyQual391 and the eight later Multi/save reserved/custodial lanes shown in E10. Not every match is a worker: U3 explicitly says NONE/test complete, U2 no active implementation writer, ResearchCopyQual391 is USER conditional/not started, #674 reserved/not running, #687 retained for review, #697 blocked, #690 prospective Claude source start held, #710 user custody/no writer, #725 immutable user custody, H0A core/viewer Manager #730 custody/no assigned writer. Exact strings below control over this summary.

No direct board Lane match exists for **Q10, U1, Integration, NonUnity, RackV2-09, S1Obj, IntroAstra243, AccessSpineMeasure248, UIQual275, MainInt277, WorkbenchTier2Reg287, UIQual376, Objective1UX443, Sen1SaveA0NonUnity690**. This answers which registrations lack a corresponding direct live-board owner row; it does **not** prove they have no owner anywhere:
- Q10's #74 is still open, WAITING/non-blocking/fresh-preflight-required. Its latest comment grants no production ownership or Unity availability.
- U1 has historical narrative reservations/releases that exact table matching misses; latest actual custody requires reconciliation.
- U3 retains a NONE/completed board row, but live main still records its protected older test state; #91 retains programme history. Do not erase it as stale.
- #287 is open NEEDS CHANGES/dirty lane; #443 is open with a claimed lineage and later Manager hold overlapping save custody. Their absent direct rows cannot release those paths.
- #224/#243/#248/#275/#277/#376/#391 have closed/handed-off issue states, and #243's title explicitly says lease released. Issue closure alone does not prove current physical lane is clean or unreserved.
- Sen1SaveA0NonUnity690 is absent on disk; live main still mentions it while current board routes #690 to the ZX20 NonUnity707 lane. That is a stale path projection requiring custodian reconciliation, not permission to recreate or delete a lane.

**Global “no live owner anywhere” is UNKNOWN for unmatched lanes. No lane is declared safely reclaimable.** The evidence covers current board rows/narratives, main exact-path searches, primary record and selected live issues; it is not omniscient about unpublished sessions or every remote claim. Source/branch cleanliness for all registered lanes was not established because full inventory timed out. Neither 25 configured lanes nor a free global lock implies 25 active writers, 25 available slots or 25 ZX20-backed lanes: several configured non-Unity lanes are on Files.

## Verified, unverified and UNKNOWN

**Verified:** placement/base/main; physical mounts/device IDs/UUID/fs/labels/free bytes; D configuration and scripts; storage PASS vs R fail; read-only wrapped Unity readback; absent lock and bounded Editor scan; live receipt byte/projection PASS; all D file hashes stable over the measured interval; exact D/R/A/F/O file differences; A clean at 4c7acc05; scoped scratch identities; current 25 lanes/44 board rows; full-status timeout; primary's unrelated status and protected pins.

**Not directly verified:** unmodified unity-lock status (write-capable guard); completion of full lane-status; per-lane du timings; source/self-test PASS claims, installer failure injection or rollback; game/Unity behaviour, project compilation, Editor launch, Windows, all-lane cleanliness/owners, future no-expiry acquisition compatibility on live Ops. No tests requiring writes were attempted.

**UNKNOWN:** exact historical kernel renumbering sequence; original actor/date/source commit for each deployed override; complete permit chain for every lane registration; global absence of undisclosed owners; current delegated T3 link; root-backed harness-internal logging/cache behaviour and incidental OS atime/audit writes. Process TMP/XDG variables were unset when inspected; -B suppresses Python bytecode, not every possible tool-internal write. No claim of zero root-system writes is made without tracing. The owner's #785-only root-log exception does not extend to #781. No further build/render/source/cache/rehearsal process is authorised here; parent must resolve any harness-placement uncertainty before new development. The only authored output is this non-root report and its requested Git commit/push.

## Complete proposed-correction register — none granted

These are recommendations for a later named actor, **not an executable repair instruction**. “G1/G2” here are the #781 packet's source/physical phases, not a waiver of AgentWorkflow review gates. No other correction, lane deletion, expiry, mount change or guard weakening is proposed.

| ID / exact file set | Correction and source authority | Required custodian | Risk | Negative witness that must remain failing | Required permit / current status |
|---|---|---|---|---|---|
| C1: isolated #652 source CCF-Ops/scripts/_ccf.py, CCF-Ops/lanes.json, CCF-Ops/README.md; eventual D files separately | Use existing reviewed persistent-device/path-admission source; resolve dispatch examples to the correct D executable. Preserve current live registrations; never copy R's stale config. Authority: inspected D/A, StorageArchitecture, #652/#731 approved source. No need to pin /dev/sdc2. | Existing #652 source custodian + Manager #731; D config only actual physical custodian/owner | Wrong disk or retired store admitted; regression from copying source fixtures over live config | Wrong by-id target, duplicate UUID, UUID/fstype/target mismatch, symlink/path escape or wrong Git store must still refuse; correct bytes on wrong device must fail | New isolated source edits: NOT GRANTED / G1 HELD. Any D config/write: NOT GRANTED / G2 HELD |
| C2: D scripts/_ccf.py, scripts/unity-lock, tests/self-test.py, roles/unity-operator.md, README.md and newly generated deployment.json only | If explicitly permitted, install exactly the separately reviewed five-file projection + new receipt, implementing accepted D-091 no-expiry. Authority: #652 4c7acc05 and #731 exact projection/risk gates; merged staged README, not wholesale A tree. Preserve D environment.sh/AGENTS/config/board/state/ledger. | Named #652 installer + owner jermur788 + Manager #731; independent nonauthor reviewer | Interrupted multi-file install; active-null lock becomes malformed on old-reader rollback; source approval mistaken for deployment permit | Occupied/unclaimed Editor, held/malformed lock, wrong mount, unrelated-file delta or signal publication gap must stop; expired ETA must never auto-release; old-reader rollback with active no-expiry must be refused | NOT GRANTED / G2 HELD. Owner NO LIVE INSTALL still binding; R-1 signal exclusion, owner risk decision, exact installer custody and Manager deploy grant still required |
| C3: D board/active.md and lanes.json#/lanes; authoritative Docs/Project/coordination/active-tasks.md only in separately assigned manager lane | Reconcile each disputed row/path/claim from current main and explicit releases, preserving 25 current entries until individually decided. Carry #690/#710/#725/#755 and Multi #730 custody; include non-running reservations. A2 owns coordination diagnosis. | Physical board/config custodian under board-directory lock; Scenario One #731 and independent Multi #730 for their own rows; coordination maintainer | Silent stale-agent reclaim, duplicate writers, deleting protected November work | Missing direct row or elapsed ETA must not make occupied/reserved Q10/U3/#690/Multi lane READY; two competing owners and dirty primary must block | NOT GRANTED / G2 HELD; coordination edit separately NOT GRANTED (outside A1/G1) |
| C4: D deployment.json only as part of a separately approved receipt transaction; isolated source CCF-Ops/deployment.json if requested | Regenerate reviewed truthful post-install receipt; distinguish historical source, actual observed overrides, current candidate and lane-excluded config. Update stale descriptive count only with timestamped observation. Authority: schema-2 verifier and #731 projection decision. | #652 provenance/installer custodian + independent reviewer | False candidate-deployed claim; hiding code drift in live_state; falsely blaming legitimate live lane changes | Any immutable tool/hash or non-lane config mutation must fail; changed lane array alone may pass provenance but must not pass custody admission automatically | Source receipt work NOT GRANTED / G1 HELD; live receipt NOT GRANTED / G2 HELD |
| C5: isolated CCF-Ops/scripts/unity-lock, tests/self-test.py, README.md | Consider a strictly read-only status path using existing guard inode with preserved writer coordination; absent/malformed state must fail/return conservatively without creating files. Authority: observed mkdir/a+ side effect and G0 no-write requirement. This is a new optional source proposal, not part of accepted four-file D-091 source. | Existing #652 source custodian + Manager #731 + independent reviewer | Inconsistent snapshot or weakened lock exclusion if read-only path is designed carelessly | Status on missing guard must create nothing; concurrent writer, malformed record and live Editor must never be reported safely available; acquire/release still serialize correctly | NOT GRANTED / G1 HELD; any deployment NOT GRANTED / G2 HELD |
| C6: isolated CCF-Ops/scripts/lane-status, tests/self-test.py, README.md | Consider explicit optional disk-usage inventory or bounded per-lane reporting while retaining all safety admission checks; explain ordinary storage exit versus assignment readiness. Authority: observed 55s timeout and sequential du/owner code. No optimisation implemented. | Existing Ops source custodian + Manager #731 | Fast mode accidentally bypasses board/store/dirty/identity checks or changes exit semantics silently | Wrong identity, dual checkout, reserved owner and dirty assigned lane must still block dispatch even with usage disabled; missing owner must not equal free | NOT GRANTED / G1 HELD; deployment NOT GRANTED / G2 HELD |
| C7: Docs/Operations/StorageArchitecture.md in a newly assigned clean docs lane; future command examples in CCF-Ops/README.md | Replace dated kernel-name facts with dated observed persistent identity and clarify Files/Videos share one disk; retain storage policy and path translations. Authority: E03 today's physical evidence, owner policy, Git canonical workflow. Never edit the dirty primary file in place. | Operations docs custodian/owner + Manager; existing primary-edit owner preserved | Turning one boot's /dev/sdX into permanent authority; overwriting user's uncommitted storage edits | A future kernel renumbering must not make documented policy select root; any diff dropping user's pending changes or creating root-backed outputs must fail review | NOT GRANTED / G1 HELD for any Ops source follow-on; docs edit outside this A1 allowlist NOT GRANTED; live action G2 HELD |

No source correction is required merely to make D --storage-only pass today: it already passes. No “fix” should replace persistent by-id with /dev/sdc2, weaken UUID/fstype/mount checks, fabricate an expiry, clear locks or overwrite a live board with a repository/scratch snapshot.

## Handoff and remaining gates

Only this report is authored. **Nothing was installed or changed in deployed Ops, lane registry, board, deployment receipt, Unity state/history, primary user work, source, mounts or processes by an operational repair.** Read-only status wrapper acquired/released only the original guard's flock without writing bytes. The bounded full-status timeout stopped its own inspection child as explicitly requested; no arbitrary user/worker process was killed or signalled. A later process-name read showed no residual du/lane-status child. No temporary script, .meta, extra evidence file or root scratch was created.

The parent must not claim an installed fix, unmodified unity-lock PASS, completed full inventory, all owners absent, all lanes on ZX20, D-091 live compliance, a no-expiry Unity permit, source/test/Unity/Windows qualification, Manager acceptance, G1/G2 approval or dashboard/Drive synchronization. Independent Q review of this diagnostic design and A2 reconciliation remain required. Dashboard/Drive **NOT VERIFIED**; no synchronization write is in A1's allowlist, so parent/Manager must handle authorised publication/coordination follow-up.

## Timestamped evidence table and exact transcripts

Outputs below are verbatim apart from stripped trailing line padding; selected long outputs are explicitly trimmed. Terminal trailing spaces in commands are also omitted. Commands are exact, including in-memory Python payloads. No stored external evidence file is required to interpret this report. The initial gate preceded timestamp capture; its time is bounded before the first live-main evidence at 01:06:43Z.

| Evidence | UTC / command | Exact output excerpt (trimmed) | Full command/output |
|---|---|---|---|
| E01: placement-gate | gate before 01:06:43Z; `realpath .` | `/media/jer/Files/CCF/Operations/CCF-781-A1-PhysicalIdentity-20261118` | [E01](#e01) |
| E02: live-main | 2026-11-19T01:06:43Z; `gh api repos/jermur788/CCF/commits/main --jq .sha` | `a69b3e2e90829505c792d95f5a3eba3e7d8d8132` | [E02](#e02) |
| E03: physical | 2026-11-19T01:07:00Z; `hostname` | `jer-laptop` | [E03](#e03) |
| E04: deployed-config | 2026-11-19T01:07:00Z; `cat /media/jer/ZX20/CCF-Ops/lanes.json` | `{` | [E04](#e04) |
| E05: source-identity | 2026-11-19T01:07:29Z; `GIT_OPTIONAL_LOCKS=0 git -C /media/jer/Files/CCF-Primary status --porcelain --branch` | `## ops/workflow-foundation-v1...origin/ops/workflow-foundation-v1 [ahead 1, behind 1]` | [E05](#e05) |
| E06: all-comparisons | 2026-11-19T01:07:49Z; `python3 -B - <<'PY'` | `ROOT D /media/jer/ZX20/CCF-Ops` | [E06](#e06) |
| E07: storage-status | 2026-11-19T01:08:34Z; `python3 -B /media/jer/ZX20/CCF-Ops/scripts/lane-status --storage-only` | `READY / PASS \| device=/dev/sdc2 rw=True \| free=367830237184 bytes (342.57 GiB) threshold=16106127360 bytes (15.00 GiB)` | [E07](#e07) |
| E08: unity-readonly | 2026-11-19T01:08:35Z; `python3 -B - <<'PY'` | `{` | [E08](#e08) |
| E09: full-status | 2026-11-19T01:08:44Z; `python3 -B - <<'PY'` | `TIMEOUT 55 seconds; not repeated` | [E09](#e09) |
| E10: owners-summary | 2026-11-19T01:09:03Z; `python3 -B - <<'PY'` | `MAIN_COORD_SHA256 cd38c93a0729f624abf34ba31ac1861e1a2b46b0a063a3ee2af759cad3c1672d lines 981` | [E10](#e10) |
| E11: lane-issues | 2026-11-19T01:10:40Z; `python3 -B - <<'PY'` | `#74 open updated=2026-10-30T16:00:41Z title=[WAITING — NON-BLOCKING UNITY FOLLOW-UP / FRESH PREFLIGHT REQUIRED] PARITY-03 Q10 — controller continuity measuremen` | [E11](#e11) |
| E12: retained-claims | 2026-11-19T01:10:57Z; `python3 -B - <<'PY'` | `ISSUE #74 comments=10 canonical_json_sha256=207a0fae89aee9b69ae854615202c89b782848becda8652b74e9cac9992e8737` | [E12](#e12) |
| E13: protected-and-paths | 2026-11-19T01:11:20Z; `python3 -B - <<'PY'` | `1bf4286b3485de018759a5b9b8e19d74beb6385844b56315f22f71b7819897fd AGENTS.md` | [E13](#e13) |
| E14: prewrite-preservation | 2026-11-19T01:11:44Z; `python3 -B - <<'PY'` | `DEPLOYED_FILES 50` | [E14](#e14) |
| E15: scratch-scoped | 2026-11-19T01:12:20Z; `python3 -B - <<'PY'` | `/media/jer/ZX20/CCF-Temp/ops652-final-source files=42 manifest_sha256=bd66de8a4bc879cb66548170907d79cae0835db25cc44bd7308bcd4433827968` | [E15](#e15) |
| E16: claims-tail | 2026-11-19T01:02:30Z; `gh api --paginate repos/jermur788/CCF/issues/781/comments --jq '.[] | select(.id > 6996384005) | "COMMENT \(.id) \(.created_at)\n\(.body)\n"'` | `COMMENT 6996404168 2026-11-19T01:02:30Z` | [E16](#e16) |

### E01

placement-gate; shell exit 0.

Command:

```bash
cd /media/jer/Files/CCF/Operations/CCF-781-A1-PhysicalIdentity-20261118
realpath .
findmnt -T . -o TARGET,SOURCE,FSTYPE -n
git rev-parse --abbrev-ref HEAD
git rev-parse HEAD
git status --porcelain --branch
```

Exact output:

```text
/media/jer/Files/CCF/Operations/CCF-781-A1-PhysicalIdentity-20261118
/media/jer/Files /dev/sdb4 ext4
docs/sen-1-781-a1-physical-identity-20261118-v1
a69b3e2e90829505c792d95f5a3eba3e7d8d8132
## docs/sen-1-781-a1-physical-identity-20261118-v1...origin/main
```

### E02

live-main; shell exit 0.

Command:

```bash
date -u +%Y-%m-%dT%H:%M:%SZ
gh api repos/jermur788/CCF/commits/main --jq .sha
git rev-parse origin/main
gh api 'repos/jermur788/CCF/contents/Docs/Project/decision-log.md?ref=main' --jq .content | base64 -d | rg 'D-091'
```

Exact output:

```text
2026-11-19T01:06:43Z
a69b3e2e90829505c792d95f5a3eba3e7d8d8132
a69b3e2e90829505c792d95f5a3eba3e7d8d8132
| D-091 | 2026-11-18 | **No arbitrary time limits on any CCF worker/worktree/Unity ownership** | **CONFIRMED LATEST DIRECT OWNER GLOBAL WORKFLOW POLICY — IMPLEMENTATION/OPS COMPATIBILITY PENDING** | At 11:49 UTC the owner expressly rejected arbitrary permission expiration for Unity **or any other worktree**, stated that they personally track agents working in Unity, and required another method. An active worker's exclusive ownership continues until **documented safe completion/release, owner-directed cancellation/handoff, or a genuine conflicting-writer/safety/authority stop**, and is **not terminated merely by exceeded estimates, elapsed hours or stale heartbeat**. Preserve a named worker/session, single writable lane, explicit scope, locked physical Ops/Unity custody, status visibility, mandatory original preflight and process verification. No silent cleanup, lease transfer, fabricated distant date, backdating, timer-based stop or automatic stale-agent reclaim. | **Supersedes** the proposed 4/8/12-hour Tier S window and any planned fresh arbitrary expiry. Existing physical Ops/leases (including #690's earlier recorded advisory cutoff and separately managed Multi leases) are **not mechanically changed by this decision**; their actual custodians must safely reconcile current installed schema and preflight under the physical board lock and independent review as needed. #652 live Ops installation remains HELD; no source/Unity start until a compliant no-expiry custody route is actually verified and separately approved. No broad waiver of exclusive Unity lock or source clean/custody gates. [Manager updated draft](https://github.com/jermur788/CCF/blob/docs/sen-1-731-chas-manager-intake-20261118-v1/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261118/Sen-1-731-COMPACT30-TIER-S-W12-OWNER-GRANT-FILLED-SELECTIONS-PENDING-SIGNATURE-20261118.md). |
```

### E03

physical; shell exit 0.

Command:

```bash
date -u +%Y-%m-%dT%H:%M:%SZ
hostname
id
uname -a
for p in / /media/jer/ZX20 /media/jer/Files /media/jer/Videos; do realpath "$p"; findmnt -T "$p" -o TARGET,SOURCE,FSTYPE,UUID,LABEL,OPTIONS -n; findmnt -J -T "$p" -o TARGET,SOURCE,FSTYPE,UUID,LABEL; done
lsblk -o NAME,PATH,MODEL,SERIAL,SIZE,FSTYPE,LABEL,UUID,MOUNTPOINTS
ls -l /dev/disk/by-uuid /dev/disk/by-id
df -B1 / /media/jer/ZX20 /media/jer/Files /media/jer/Videos
```

Exact output:

```text
2026-11-19T01:07:00Z
jer-laptop
uid=1000(jer) gid=1000(jer) groups=1000(jer),4(adm),24(cdrom),27(sudo),30(dip),46(plugdev),100(users),105(lpadmin),125(sambashare)
Linux jer-laptop 7.0.0-42-generic #42~24.04.1-Ubuntu SMP PREEMPT_DYNAMIC Mon Oct 19 14:43:01 UTC 2 x86_64 x86_64 x86_64 GNU/Linux
/
/      /dev/sda2 ext4   d393b49a-24aa-4f88-b8c4-5121b96c240d       rw,relatime,errors=remount-ro
{
   "filesystems": [
      {
         "target": "/",
         "source": "/dev/sda2",
         "fstype": "ext4",
         "uuid": "d393b49a-24aa-4f88-b8c4-5121b96c240d",
         "label": null
      }
   ]
}
/media/jer/ZX20
/media/jer/ZX20 /dev/sdc2 exfat  18D6-896E                            ZX20  rw,relatime,uid=1000,gid=1000,fmask=0022,dmask=0022,iocharset=utf8,errors=remount-ro
{
   "filesystems": [
      {
         "target": "/media/jer/ZX20",
         "source": "/dev/sdc2",
         "fstype": "exfat",
         "uuid": "18D6-896E",
         "label": "ZX20"
      }
   ]
}
/media/jer/Files
/media/jer/Files /dev/sdb4 ext4   2283b135-bf37-42d6-921f-2fb861d0a8d4 Files rw,relatime
{
   "filesystems": [
      {
         "target": "/media/jer/Files",
         "source": "/dev/sdb4",
         "fstype": "ext4",
         "uuid": "2283b135-bf37-42d6-921f-2fb861d0a8d4",
         "label": "Files"
      }
   ]
}
/media/jer/Videos
/media/jer/Videos /dev/sdb1 ext4   60e276b9-4abb-4325-a913-f4c43c14294b Videos rw,relatime
{
   "filesystems": [
      {
         "target": "/media/jer/Videos",
         "source": "/dev/sdb1",
         "fstype": "ext4",
         "uuid": "60e276b9-4abb-4325-a913-f4c43c14294b",
         "label": "Videos"
      }
   ]
}
NAME   PATH      MODEL                      SERIAL                 SIZE FSTYPE LABEL  UUID                                 MOUNTPOINTS
sda    /dev/sda  SAMSUNG MZNLN256HCHP-00000 S1ZWNAAH302842       238.5G
├─sda1 /dev/sda1                                                   512M vfat          7753-0B64                            /boot/efi
└─sda2 /dev/sda2                                                   238G ext4          d393b49a-24aa-4f88-b8c4-5121b96c240d /
sdb    /dev/sdb  HGST HTS721010A9E630       JR10004M1U3BGE       931.5G
├─sdb1 /dev/sdb1                                                 443.2G ext4   Videos 60e276b9-4abb-4325-a913-f4c43c14294b /media/jer/Videos
├─sdb2 /dev/sdb2                                                   488M vfat          F624-7937
├─sdb3 /dev/sdb3                                                  14.9G swap          28cc8d37-1817-4993-ae4e-0599a2983a43
└─sdb4 /dev/sdb4                                                 472.9G ext4   Files  2283b135-bf37-42d6-921f-2fb861d0a8d4 /media/jer/Files
sdc    /dev/sdc  NetacPortableSSD           AA20260702512G231151 476.9G
├─sdc1 /dev/sdc1                                                    16M
└─sdc2 /dev/sdc2                                                 476.9G exfat  ZX20   18D6-896E                            /media/jer/ZX20
sdd    /dev/sdd  SD/MMC                     20120501030900000        0B
sr0    /dev/sr0  HL-DT-ST DVDRAM GUE1N      KX4G35G4506           1024M
/dev/disk/by-id:
total 0
lrwxrwxrwx 1 root root  9 Nov 19 00:24 ata-HGST_HTS721010A9E630_JR10004M1U3BGE -> ../../sdb
lrwxrwxrwx 1 root root 10 Nov 19 00:24 ata-HGST_HTS721010A9E630_JR10004M1U3BGE-part1 -> ../../sdb1
lrwxrwxrwx 1 root root 10 Nov 19 00:24 ata-HGST_HTS721010A9E630_JR10004M1U3BGE-part2 -> ../../sdb2
lrwxrwxrwx 1 root root 10 Nov 19 00:24 ata-HGST_HTS721010A9E630_JR10004M1U3BGE-part3 -> ../../sdb3
lrwxrwxrwx 1 root root 10 Nov 19 00:24 ata-HGST_HTS721010A9E630_JR10004M1U3BGE-part4 -> ../../sdb4
lrwxrwxrwx 1 root root  9 Nov 19 00:24 ata-HL-DT-ST_DVDRAM_GUE1N_KX4G35G4506 -> ../../sr0
lrwxrwxrwx 1 root root  9 Nov 19 00:24 ata-NetacPortableSSD_AA20260702512G231151 -> ../../sdc
lrwxrwxrwx 1 root root 10 Nov 19 00:24 ata-NetacPortableSSD_AA20260702512G231151-part1 -> ../../sdc1
lrwxrwxrwx 1 root root 10 Nov 19 00:24 ata-NetacPortableSSD_AA20260702512G231151-part2 -> ../../sdc2
lrwxrwxrwx 1 root root  9 Nov 19 00:24 ata-SAMSUNG_MZNLN256HCHP-00000_S1ZWNAAH302842 -> ../../sda
lrwxrwxrwx 1 root root 10 Nov 19 00:24 ata-SAMSUNG_MZNLN256HCHP-00000_S1ZWNAAH302842-part1 -> ../../sda1
lrwxrwxrwx 1 root root 10 Nov 19 00:24 ata-SAMSUNG_MZNLN256HCHP-00000_S1ZWNAAH302842-part2 -> ../../sda2
lrwxrwxrwx 1 root root  9 Nov 19 00:25 usb-Generic-_SD_MMC_20120501030900000-0:0 -> ../../sdd
lrwxrwxrwx 1 root root  9 Nov 19 00:24 usb-NetacPortableSSD_NetacPortableSSD_AA20260702512G231151-0:0 -> ../../sdc
lrwxrwxrwx 1 root root 10 Nov 19 00:24 usb-NetacPortableSSD_NetacPortableSSD_AA20260702512G231151-0:0-part1 -> ../../sdc1
lrwxrwxrwx 1 root root 10 Nov 19 00:24 usb-NetacPortableSSD_NetacPortableSSD_AA20260702512G231151-0:0-part2 -> ../../sdc2
lrwxrwxrwx 1 root root  9 Nov 19 00:24 wwn-0x5000cca8a8d9816e -> ../../sdb
lrwxrwxrwx 1 root root 10 Nov 19 00:24 wwn-0x5000cca8a8d9816e-part1 -> ../../sdb1
lrwxrwxrwx 1 root root 10 Nov 19 00:24 wwn-0x5000cca8a8d9816e-part2 -> ../../sdb2
lrwxrwxrwx 1 root root 10 Nov 19 00:24 wwn-0x5000cca8a8d9816e-part3 -> ../../sdb3
lrwxrwxrwx 1 root root 10 Nov 19 00:24 wwn-0x5000cca8a8d9816e-part4 -> ../../sdb4
lrwxrwxrwx 1 root root  9 Nov 19 00:24 wwn-0x5001480000000000 -> ../../sr0
lrwxrwxrwx 1 root root  9 Nov 19 00:24 wwn-0x5002538d000825dd -> ../../sda
lrwxrwxrwx 1 root root 10 Nov 19 00:24 wwn-0x5002538d000825dd-part1 -> ../../sda1
lrwxrwxrwx 1 root root 10 Nov 19 00:24 wwn-0x5002538d000825dd-part2 -> ../../sda2

/dev/disk/by-uuid:
total 0
lrwxrwxrwx 1 root root 10 Nov 19 00:24 18D6-896E -> ../../sdc2
lrwxrwxrwx 1 root root 10 Nov 19 00:24 2283b135-bf37-42d6-921f-2fb861d0a8d4 -> ../../sdb4
lrwxrwxrwx 1 root root 10 Nov 19 00:24 28cc8d37-1817-4993-ae4e-0599a2983a43 -> ../../sdb3
lrwxrwxrwx 1 root root 10 Nov 19 00:24 60e276b9-4abb-4325-a913-f4c43c14294b -> ../../sdb1
lrwxrwxrwx 1 root root 10 Nov 19 00:24 7753-0B64 -> ../../sda1
lrwxrwxrwx 1 root root 10 Nov 19 00:24 F624-7937 -> ../../sdb2
lrwxrwxrwx 1 root root 10 Nov 19 00:24 d393b49a-24aa-4f88-b8c4-5121b96c240d -> ../../sda2
Filesystem        1B-blocks         Used    Available Use% Mounted on
/dev/sda2      250375106560 200987328512  36594888704  85% /
/dev/sdc2      512075235328 144244998144 367830237184  29% /media/jer/ZX20
/dev/sdb4      498654666752 433369075712  39880065024  92% /media/jer/Files
/dev/sdb1      467298177024    440119296 443045425152   1% /media/jer/Videos
```

### E04

deployed-config; shell exit 0.

Command:

```bash
date -u +%Y-%m-%dT%H:%M:%SZ
cat /media/jer/ZX20/CCF-Ops/lanes.json
cat /media/jer/ZX20/CCF-Ops/deployment.json
stat -c '%n | size=%s | mtime=%y' /media/jer/ZX20/CCF-Ops/lanes.json /media/jer/ZX20/CCF-Ops/deployment.json
sha256sum /media/jer/ZX20/CCF-Ops/lanes.json /media/jer/ZX20/CCF-Ops/deployment.json
rg --files -g '!__pycache__/**' /media/jer/ZX20/CCF-Ops/scripts
```

Exact output:

```text
2026-11-19T01:07:00Z
{
  "schema": 1,
  "storage": {
    "root": "/media/jer/ZX20",
    "uuid": "18D6-896E",
    "fstype": "exfat",
    "device": "/dev/disk/by-id/ata-NetacPortableSSD_AA20260702512G231151-part2",
    "minimum_free_bytes": 16106127360
  },
  "repository": "/media/jer/Files/CCF/Repository.git",
  "board": "board/active.md",
  "lanes": [
    {
      "lane": "Q10",
      "path": "/media/jer/ZX20/CCF-Q10",
      "purpose": "Claim #74 controller SeamTransition verification continuation on the existing assigned lane",
      "unity": true
    },
    {
      "lane": "U1",
      "path": "/media/jer/ZX20/CCF-U1",
      "purpose": "primary Unity; Library warm-up deferred",
      "unity": true
    },
    {
      "lane": "U2",
      "path": "/media/jer/ZX20/CCF-U2",
      "purpose": "secondary Unity; Library warm-up deferred",
      "unity": true
    },
    {
      "lane": "U3",
      "path": "/media/jer/ZX20/CCF-U3",
      "purpose": "temporary Scenario One non-UI checkpoint qualification; own fresh Library; U2 remains reserved/cold",
      "unity": true
    },
    {
      "lane": "Integration",
      "path": "/media/jer/ZX20/CCF-Integration",
      "purpose": "clean integration; serialized single writer",
      "unity": false
    },
    {
      "lane": "NonUnity",
      "path": "/media/jer/Files/CCF/Operations/CCF-NonUnity",
      "purpose": "offline tools/tests/docs on Files HDD; never launch Unity",
      "unity": false
    },
    {
      "lane": "RackV2-09",
      "path": "/media/jer/ZX20/CCF-RackV2-09",
      "purpose": "RACK-V2-09 field-marking Unity lane; registered by #91 for the packet-named worktree",
      "unity": true
    },
    {
      "lane": "S1Obj",
      "path": "/media/jer/ZX20/CCF-S1ObjOverflow",
      "purpose": "#224 Scenario One Objectives-window overflow bounded UI repair; registered by #203 for the dedicated clean worktree",
      "unity": true
    },
    {
      "lane": "IntroAstra243",
      "path": "/media/jer/ZX20/CCF-Intro-Astra-20261103",
      "purpose": "#243 authorised isolated onboarding visual verification",
      "unity": true
    },
    {
      "lane": "AccessSpineMeasure248",
      "path": "/media/jer/ZX20/CCF-AccessSpine-Measure-20261103",
      "purpose": "#248 read-only access-spine geometry + MFM mask measurement; Manager-reserved isolated worktree, registered by #248 preflight",
      "unity": true
    },
    {
      "lane": "UIQual275",
      "path": "/media/jer/ZX20/CCF-UIQualification-20261104",
      "purpose": "#275 focused Tier-2 Unity qualification of the isolated overnight UI staging (same exact commit d3ba4fce); registered by #275 preflight because the machine-wide lease admits ZX20 lanes only",
      "unity": true
    },
    {
      "lane": "MainInt277",
      "path": "/media/jer/ZX20/CCF-MainIntegration-20261104",
      "purpose": "main playtest worktree after user-directed #277/#243 integration; warm Library",
      "unity": true
    },
    {
      "lane": "WorkbenchTier2Reg287",
      "path": "/media/jer/ZX20/CCF-WorkbenchTier2-Registered-20261106",
      "purpose": "#287 workbench-retirement Tier-2 qualification; real worktree of the canonical store, registered by #312 for the packet-named path",
      "unity": true
    },
    {
      "lane": "UIQual376",
      "path": "/media/jer/ZX20/CCF-S1-UI-Presentation-V1-Qual",
      "purpose": "#376 user-authorised separate exact-source UI qualification",
      "unity": true
    },
    {
      "lane": "ResearchCopyQual391",
      "path": "/media/jer/ZX20/CCF-S1-ResearchCopy-V1-Qual",
      "purpose": "#391 user-authorised new exact-candidate lane for research-copy V1 Unity qualification",
      "unity": true
    },
    {
      "lane": "Objective1UX443",
      "path": "/media/jer/ZX20/CCF-S1-Objective1UX-20261112",
      "purpose": "#443 Objective 1 first repair (and #438 witness) primary lane; user-chosen Claude substitute for Sol",
      "unity": true
    },
    {
      "lane": "MultiP0Proof674",
      "path": "/media/jer/Files/CCF/Operations/CCF-MultiP0-AncestryProof-20261117/ccf-multi-p0-674",
      "purpose": "#674 isolated genuine P0 ancestry scratch proof (Git only). Worktree path allocated by physical custodian; store is the task-local isolated bare repo /media/jer/Files/CCF/Operations/CCF-MultiP0-AncestryProof-20261117/git/CCF-MultiP0-AncestryProof.git deliberately NOT the shared canonical store. No Unity. Custodian reservation expires 2026-11-18T18:00:00Z.",
      "unity": false
    },
    {
      "lane": "MultiP0P1Integ687",
      "path": "/media/jer/Files/CCF/Operations/CCF-687-P0P1-Integration-20261117/ccf-687-child-a",
      "purpose": "#687 conditional G2 child A isolated P0 current-main ancestry source lane (Git only), RESERVED / NOT RUNNING. Path is the ALLOCATED but UNCREATED child A checkout; the store is the task-local isolated bare repo /media/jer/Files/CCF/Operations/CCF-687-P0P1-Integration-20261117/git/CCF-687-Integration.git deliberately NOT the shared canonical or legacy store. No Unity. Requires Manager #641 readback ratification, immutable M and a distinct registered child A claim before any Git operation. Custodian reservation expires 2026-11-19T18:00:00Z. ASSIGNMENT 2026-11-17: exclusive source writer OpenAI Codex GPT-6.2 Sol Medium (jer-laptop, #692; direct user custody reassignment); assignment only, worker must register [RUNNING] on #692 before any Git operation. Expiry unchanged at 2026-11-19T18:00:00Z.",
      "unity": false
    },
    {
      "lane": "Sen1SaveA0NonUnity690",
      "path": "/media/jer/Files/CCF/Worktrees/CCF-Sen1-690-A0-20261117",
      "purpose": "#695 prospective #690 A0 source-only reservation, canonical Files bare store. Unassigned / NOT RUNNING. Separate Manager source-start approval and exclusive writer required. No Unity.",
      "unity": false
    },
    {
      "lane": "MultiP1ChildB697",
      "path": "/media/jer/Files/CCF/Operations/CCF-697-P1-Child-B-20261117/ccf-697-child-b",
      "unity": false,
      "purpose": "#697 EXCLUSIVE RESERVED: OpenAI Codex GPT-6.2 Sol Medium (this T3 Code / Codex session on jer-laptop); source not started. Isolated bare store /media/jer/Files/CCF/Operations/CCF-697-P1-Child-B-20261117/git/CCF-697-Child-B.git; branch integrate/multi-p1-h2-f-child-b-20261117-v1; evidence /media/jer/Files/CCF/Verification/CCF-697-P1-Child-B-20261117; expiry 2026-11-19T18:00:00Z. Worktree uncreated. Separate Manager #694 start permit and writer RUNNING required. No Unity."
    },
    {
      "lane": "Sen1SaveA0ZX20NonUnity707",
      "path": "/media/jer/ZX20/CCF-Sen1-690-A0-20261117",
      "unity": false,
      "purpose": "#707 ZX20 NonUnity isolated #690 A0 Git-only source lane (owner Option B 2026-11-17 21:32Z). Canonical-store worktree bootstrapped DETACHED at D-073 55863f1b7719011bba16b3f3ab377e379c0b988a; A0 branch integration/sen-1-d073-shared-save-a0-20261117-v1 not created by #707; created and attached at 55863f1b by #714 (local only). Never launch Unity here. Custody per board row CCF-690-SEN1-SAVE-A0; prospective writer session 8b161722-5c96-4e4f-aa9b-1cd9df4c0dee (SOURCE START HELD) assigned by #714; old 2026-11-18T00:00:00Z expiry released; expiry 2026-11-19T00:00:00Z ADVISORY / MANUALLY SUPERVISED - NOT ENFORCED BY TOOL, custodian user jermur788. Source start requires separate Manager #700 APPROVED TO START."
    },
    {
      "lane": "MultiP1ModeRepair710",
      "path": "/media/jer/Files/CCF/Operations/CCF-710-ModeRepair-20261117/ccf-710-src",
      "unity": false,
      "purpose": "#710 RETAINED R SOURCE CUSTODY (no active writer). Custodian user jermur788; #710 writer Claude Opus 5.5 finished. Preserves isolated bare store /media/jer/Files/CCF/Operations/CCF-710-ModeRepair-20261117/git/CCF-710-ModeRepair.git, branch repair/multi-p1-shell-modes-710-20261117-v1 at local R 35d26f5e64a00a400c700871927ccc2173370568 (single parent C 719ccbf87f0644a1a3fb9487ce7b991f4b867424; 4 P1 .sh 100644->100755 only) and clean source worktree ccf-710-src; receipts/ and evidence /media/jer/Files/CCF/Verification/CCF-710-ModeRepair-20261117 retained. Disposable ccf-710-test and tmp/ removed 2026-11-17 under #712 Phase 2. Board row CCF-710-MULTI-P1-R-CUSTODY. No source writes, push, main merge, L4 waiver, Unity, Windows or ecology year. Expiry EXTENDED to 2026-11-23T18:00:00Z by owner/custodian receipt #732 (was 2026-11-19T18:00:00Z); custodian-managed; no automatic purge without R preservation proof; release only after Manager #694 R disposition."
    },
    {
      "lane": "MultiAnnualF0Child725",
      "path": "/media/jer/Files/CCF/Operations/CCF-725-F0Annual-20261118/ccf-725-src",
      "purpose": "#725 EXCLUSIVE RESERVED / NOT RUNNING: OpenAI Codex GPT-6.2 Sol High (native Linux T3 Code, jer-laptop); physical lease granted by custodian jermur788; Manager #694 source start required. Isolated bare store git/CCF-725-F0Annual.git (--no-hardlinks clone of R, origin removed); worktree ccf-725-src; branch feature/multi-p1-f0-annual-725-20261118-v1 at R 35d26f5e64a00a400c700871927ccc2173370568; home/nuget/tmp/artifacts/receipts inside lane; evidence /media/jer/Files/CCF/Verification/CCF-725-F0Annual-20261118; expiry 2026-11-27T18:00:00Z. No Unity. P0/Assets/fixture/catalog/AnnualGate/Host/saves and all other lanes read-only. #735 IMMUTABLE RETENTION ONLY; no source writer; owner jermur788; no auto-purge; original lease preserved, see /media/jer/Files/CCF/Verification/CCF-735-SourceRetention-20261118/RETENTION.md.",
      "unity": false
    },
    {
      "lane": "MultiH0ACore",
      "path": "/media/jer/Files/CCF/Operations/CCF-H0A-Year0Viewer-20261118/ccf-h0a-core",
      "unity": false,
      "purpose": "#755 Phase1 H0-A Core read-adapter / strict fixture admission / f32 codec lane: EMPTY sparse worktree (three tracked root files only), RESERVED by Manager #730 / NOT RUNNING, no source or fixture, no writer assigned, no G2. Isolated REMOTE-LESS bare store /media/jer/Files/CCF/Operations/CCF-H0A-Year0Viewer-20261118/git/CCF-H0A-Year0Viewer.git (shallow base 34185b8b71ba280c89091290345e537ef107bdf4); branch feature/multi-h0a-core-year0-admission-20261118-v1; push denied (blank credential helper, push.default=nothing, reject-all pre-push, five GitHub pushInsteadOf rewrites); artifacts dir /media/jer/Files/CCF/Operations/CCF-H0A-Year0Viewer-20261118/artifacts; evidence /media/jer/Files/CCF/Verification/CCF-H0A-Year0Viewer-20261118. NO EXPIRY (D-091): ownership continues until documented safe release. Separate Manager-granted writer claim and actor-specific G2 grant required. Created by temporary G1 setup custodian Claude Sonnet 5.5 (T3 Code, jer-laptop) under Manager #730 grant https://github.com/jermur788/CCF/issues/755#issuecomment-6986127467. No Unity."
    },
    {
      "lane": "MultiH0AViewer",
      "path": "/media/jer/Files/CCF/Operations/CCF-H0A-Year0Viewer-20261118/ccf-h0a-viewer",
      "unity": false,
      "purpose": "#755 Phase1 H0-A A renderer / camera / hit test / inspector / single UI shell lane: EMPTY sparse worktree (three tracked root files only), RESERVED by Manager #730 / NOT RUNNING, no source or fixture, no writer assigned, no G2. Isolated REMOTE-LESS bare store /media/jer/Files/CCF/Operations/CCF-H0A-Year0Viewer-20261118/git/CCF-H0A-Year0Viewer.git (shallow base 34185b8b71ba280c89091290345e537ef107bdf4); branch feature/multi-h0a-a-year0-viewer-20261118-v1; push denied (blank credential helper, push.default=nothing, reject-all pre-push, five GitHub pushInsteadOf rewrites); artifacts dir /media/jer/Files/CCF/Operations/CCF-H0A-Year0Viewer-20261118/artifacts; evidence /media/jer/Files/CCF/Verification/CCF-H0A-Year0Viewer-20261118. NO EXPIRY (D-091): ownership continues until documented safe release. Separate Manager-granted writer claim and actor-specific G2 grant required. Created by temporary G1 setup custodian Claude Sonnet 5.5 (T3 Code, jer-laptop) under Manager #730 grant https://github.com/jermur788/CCF/issues/755#issuecomment-6986127467. No Unity."
    }
  ],
  "git_stores": [
    {
      "name": "Files-canonical",
      "git_dir": "/media/jer/Files/CCF/Repository.git"
    },
    {
      "name": "legacy-SSD",
      "git_dir": "/media/jer/Files/CCF-Legacy/.git"
    }
  ]
}
{
  "schema": 2,
  "record_type": "observed-deployment-with-separate-source-candidate",
  "source_repository": "/media/jer/Files/CCF/Repository.git",
  "source_branch": "ops/workflow-foundation-v1",
  "source_head": "dfe445e54d099a0e20c9c7d5a9e3e6f195968b4e",
  "deployed_at": "2026-10-15T00:20:00+00:00",
  "deployment_target": "/media/jer/ZX20/CCF-Ops",
  "recorded_at": "2026-11-15T23:18:25.883045+00:00",
  "provenance_status": "RECONCILED_OBSERVATION_NOT_SOURCE_ACCEPTANCE",
  "previous_manifest_sha256": "2034c77fac082ec8107deef073605ce9314d594e79568e28f32aa8584dc4f5bc",
  "source_hashes": {
    "ActiveTasksTransition.md": "e2fe59102b8b408a313f48807323c5d1befb6e89407e8a50dfdf169cd8e7a68d",
    "CLAUDE.md": "625065153e9b69634723b6a6f678fa2371350cb7f436393461a62a07a28e2335",
    "G4BCorrections.md": "5fc54e196e1905a39a5f99bf83ca1a0b8033e2717b2e6a924a861dc54cfb9486",
    "WorkflowFoundationVerification.md": "05d1f8cc3df50180e226e0da16994bd0cc3c6b77d025acb7b18fa5889763b60d",
    "pilot/README.md": "a0813687f28334369cea95796131edc999cf6a24cc8cc65ef4fc93cd279ff6b7",
    "pilot/manager-packet.md": "7c65c61adec2c4ffa30b95529da5edee9f930241e0c84c66c67aa5bd6b2dfb29",
    "pilot/t2-pilot-packet.md": "93aafd766e02e31ceee103bc8615d866995f5c2f8a94c2ba7ae53ebe2df1bd35",
    "pilot/unity-gate-example.md": "4b1de83798691c697141f6a8e2b21561a27a3a741ef179f2932d2e38c37703a3",
    "roles/execution-manager.md": "0fb67b1283052008a42fe40dbeef6dea5f6710ca5b71c7272d99bf581b3843f0",
    "roles/reviewer.md": "58bafc0dee42158816687a71becb73f8a2cf4d1f9dac192bfbe4fe3a56207d6a",
    "roles/unity-operator.md": "120f1deb006aa56a9d4b9643917f855548b5da80432a7166442ecc6163eee815",
    "scripts/packet-lint": "a776ec42a13224936a5c7c6a68ec2ed7ddc4e2cf21a96e287e6843a31e243e43",
    "scripts/unity-lock": "54cf0a95561d0a8c350d89adbee83f393990b728d6127b94f69c587260f048e5",
    "tests/fixtures/canonical-style.md": "e063274c82b1c38a8fc714ccc1ef3b19fb21d04494dca18a56a17760c60eb77a",
    "tests/fixtures/conflicting-unity-ownership.md": "ca6a89ad0b4422575aab3234d5293672ccf756e92f952782d4474f8fcd1fee24",
    "tests/fixtures/handoff-placeholder.md": "8f82d136a6ced39892a03ea03902d93c94fb270aedd1a80abec09d3d5b84edb9",
    "tests/fixtures/missing-base.md": "e7e54f12e455304b501ed5d1f6b6c5a789b20e5cf3413aaf60d2304cffcd44d3",
    "tests/fixtures/missing-role-file.md": "35c9831ff0d67a02151c99c03fa5ee01e7372430be49b3432cb6f0bf8c536856",
    "tests/fixtures/missing-stop-condition.md": "719066818259794ffcab48073682af45e968cbae2cca2c341ccca568deb92171",
    "tests/fixtures/missing-unity-required.md": "9e1110ce0a78241a64508de7269d3c92e9e13ea2a168521263eaba151346c7bb",
    "tests/fixtures/missing-verification-tier.md": "16e0e3ce718d132b17eb24812a60ed2efd97fc492dd3562acec5a362c0f139e2",
    "tests/fixtures/nonunity-lane-head.md": "b90081a1573de40a1deb64ed4e7bdf3907ec258ba9f1142313bb02ebb21c6113",
    "tests/fixtures/partial.md": "7d6bb96b2602b70db57084099e8dcb3870f8328d6b41597619de9b8266ce6be9",
    "tests/fixtures/stop-placeholder.md": "9c7c15857dab00f42ec9644136964d650916498b1053f6d9867c3958f2b23dfe",
    "tests/fixtures/unity-nonunity-lane.md": "0bb1fa102a8f9c11cc025f0b84b36953d31cf7e2af79a875490b1fa9d066cb21",
    "tests/fixtures/valid-unity.md": "281f9c52f9d168c5b75cd6fecfa7ac96cd0e044e7a44dd3508e4f273f416c00b",
    "tests/fixtures/valid.md": "7c65c61adec2c4ffa30b95529da5edee9f930241e0c84c66c67aa5bd6b2dfb29",
    "tests/self-test.py": "2a12a19d848ae789a1404c7ebd5285457e061b0a7142b91de95ed0b79cdeeaf3"
  },
  "deployed_overrides": {
    "AGENTS.md": {
      "sha256": "d1ce3ad775e16d80fea6779c91bfa66adbd2e019edfbbbb9280be563be60e08f",
      "reason": "Live storage-migration instructions differ from historical source; captured, not imported or independently accepted.",
      "previous_source_sha256": "a7d8aa3236ebdd8b9e1d0c24090ceba61353ab56d8fba35b024078a0cbfe0dbe",
      "previous_deployed_sha256": "a7d8aa3236ebdd8b9e1d0c24090ceba61353ab56d8fba35b024078a0cbfe0dbe"
    },
    "G4WorkflowFoundationReviewPacket.md": {
      "sha256": "0219dfb2666a8c444c6c289782e383d6656d44f1af34d0fd55606a725d61e62c",
      "reason": "Supplemental deployed review document absent at historical source_head; matches prior immutable_deployed_hashes, original publication source not established.",
      "previous_deployed_sha256": "0219dfb2666a8c444c6c289782e383d6656d44f1af34d0fd55606a725d61e62c"
    },
    "README.md": {
      "sha256": "249be391e172e9d80f7d3d3419af961850b84c40fb2f15ba7b1d0469649eb517",
      "reason": "Live durable-storage paths differ from historical source; captured, not imported or independently accepted.",
      "previous_source_sha256": "af82c798610906fd8516cdedd8c1407e6a0bde821e6b8108c710283407117401",
      "previous_deployed_sha256": "af82c798610906fd8516cdedd8c1407e6a0bde821e6b8108c710283407117401"
    },
    "WorkflowFoundationHandoff.md": {
      "sha256": "a9077b50dc25523cbf8d0711b23a9ad392c2d27e38858e8e073ed8277125a018",
      "reason": "Supplemental deployed handoff absent at historical source_head; matches prior immutable_deployed_hashes, original publication source not established.",
      "previous_deployed_sha256": "a9077b50dc25523cbf8d0711b23a9ad392c2d27e38858e8e073ed8277125a018"
    },
    "environment.sh": {
      "sha256": "3775cba7ed446c61ae6fa2977da5cf0984ff7d888213ad9fa33396b1aa11c954",
      "reason": "Live Files output-directory admission and exports differ from historical source; captured, not imported or independently accepted.",
      "previous_source_sha256": "0dde119a47f668d03f93445cc606a054465bf6eea49155dc0f71eb2eb41d89d4",
      "previous_deployed_sha256": "0dde119a47f668d03f93445cc606a054465bf6eea49155dc0f71eb2eb41d89d4"
    },
    "scripts/_ccf.py": {
      "sha256": "ab4343efc7d1b82c5a4aca747d0e09c614779cfd9aa406a0dd681d204f5de001",
      "reason": "Observed live persistent-device identity and Files admission differ from historical source. Round 2 ports repository admission; candidate retains the persistent default pin, restricts explicit lane permission to durable non-Unity paths and admits config-relative boards on project storage. Live bytes remain pinned, not deployed from candidate or independently accepted.",
      "previous_source_sha256": "b12a613379d69c5afa8e4c20ae0fcdcff6b8772c39501f60c01933263cf46923",
      "previous_deployed_sha256": "b12a613379d69c5afa8e4c20ae0fcdcff6b8772c39501f60c01933263cf46923"
    },
    "scripts/lane-status": {
      "sha256": "e7102a83ad942cc7365cb22db9a8f79823744ea98bc2cf8a6738a11f1897173f",
      "reason": "Observed live persistent-device comparison and durable non-Unity inventory differ from historical source. Round 2 candidate ports these inventory semantics, but its shared config admission has stricter explicit path boundaries. Live bytes remain pinned; no candidate deployment or independent acceptance claimed.",
      "previous_source_sha256": "7fca072046997d43caf51a87397d2735c579cc2c1732515cfa088c9dff4b3761",
      "previous_deployed_sha256": "7fca072046997d43caf51a87397d2735c579cc2c1732515cfa088c9dff4b3761"
    }
  },
  "configuration_hashes": {
    "lanes.json": {
      "sha256_without_lanes": "ed642a520d25470f1192cbbd88b4ccede16bebc6259a5bb1db039df45864cca6",
      "reason": "Observed live storage-migration configuration; all keys except operational /lanes remain pinned, including repository, git_stores and storage identity. Not a claim of byte equality with candidate config."
    }
  },
  "live_state": {
    "board/active.md": "Manager-owned operational board; changes during task coordination, not deployed source.",
    "ledger/": "Operational handoffs and closed-task records, including README and historic fixtures; not deployed tool source.",
    "state/": "Machine-wide ownership records, guard and append-only history; never overwritten by provenance or source deployment.",
    "lanes.json#/lanes": "Live lane registrations and lane paths/purposes (16 entries) differ from the 4-entry source configuration by design; not imported.",
    "scripts/__pycache__/": "Derived Python interpreter cache, not source; -B prevents this verifier from creating it."
  },
  "audit_metadata": "deployment.json and deployment.json.bak-YYYYMMDDTHHMMSSZ are receipt/backup metadata; no self-hash. Every other file must be hashed or explicitly classified.",
  "candidate": {
    "source_repository": "/media/jer/Files/CCF/Repository.git",
    "source_worktree": "/media/jer/Files/CCF/Operations/CCF-OpsLeaseFix-20261115",
    "source_branch": "ops/unity-lease-persistent-identity-20261115-v1",
    "source_head": "60f97b06bd41f9a89fde4eb775527ebc6947560b",
    "deployment_status": "NOT_DEPLOYED",
    "deployed_at": null,
    "source_hashes": {
      "scripts/_ccf.py": "1295def55a4821ccd00423f65ab58e64a3be916dc18b127ed99ae1099c3e7677",
      "scripts/lane-status": "e7102a83ad942cc7365cb22db9a8f79823744ea98bc2cf8a6738a11f1897173f",
      "lanes.json": "f73d1c08556ebb5a2befeeb7dd9fee7cde08184c935a2af73aded2c8dcefe477",
      "scripts/verify-deployment": "ee001216fb020ae33ed54f4c317e2c8e19ce4bdb407f4e6bb8ae457248a02386",
      "DeploymentProvenance.md": "a08fb8a5d38e82ab3dad1b6b1e0df7280cf28e1751fd1359b61cc2947b288db4",
      "scripts/unity-lock": "54cf0a95561d0a8c350d89adbee83f393990b728d6127b94f69c587260f048e5",
      "scripts/packet-lint": "a776ec42a13224936a5c7c6a68ec2ed7ddc4e2cf21a96e287e6843a31e243e43",
      "tests/self-test.py": "6bba90d82e6ce2297295998c3e7035f3360ada04af338e2c25c167a99abf1a62"
    },
    "hash_scope": "Eight bounded candidate files at the implementation commit; self-test uses verified live backing-device numbers. Receipt/evidence commit follows. Live tooling/config remains unchanged."
  },
  "notes": [
    "This receipt is identical in Git and live, describes existing live bytes separately from candidate bytes, and does not claim the candidate was deployed.",
    "source_head/deployed_at retain the historical deployment baseline; observed overrides have no falsely attributed source commit or deployment date.",
    "No source mismatch was excluded as operational state: all immutable live overrides retain enforced SHA-256 comparisons.",
    "Legacy mutable_paths and immutable_deployed_hashes were replaced by non-overlapping source_hashes, deployed_overrides, configuration_hashes and reasoned live_state.",
    "Only deployment.json is published to live after a timestamped backup; no live tooling, lane registrations, board, ledger or state is rewritten."
  ]
}
/media/jer/ZX20/CCF-Ops/lanes.json | size=12179 | mtime=2026-11-18 17:28:31.510000000 +0000
/media/jer/ZX20/CCF-Ops/deployment.json | size=10462 | mtime=2026-11-15 23:18:25.880000000 +0000
c1d147196e1b8d57008e44545db3ba7f3fecea0ae12094dbf3e05f1c782b7d9e  /media/jer/ZX20/CCF-Ops/lanes.json
8c66b290c247708846cb8b59f83626a10a3fa1d052397a3316dc5f23fc157b18  /media/jer/ZX20/CCF-Ops/deployment.json
/media/jer/ZX20/CCF-Ops/scripts/lane-status
/media/jer/ZX20/CCF-Ops/scripts/unity-lock
/media/jer/ZX20/CCF-Ops/scripts/packet-lint
/media/jer/ZX20/CCF-Ops/scripts/_ccf.py
/media/jer/ZX20/CCF-Ops/scripts/__pycache__/_ccf.cpython-312.pyc
```

### E05

source-identity; shell exit 0.

Command:

```bash
date -u +%Y-%m-%dT%H:%M:%SZ
GIT_OPTIONAL_LOCKS=0 git -C /media/jer/Files/CCF-Primary status --porcelain --branch
git -C /media/jer/Files/CCF-Primary rev-parse HEAD
git ls-tree -r --name-only origin/main -- CCF-Ops
git -C /media/jer/Files/CCF/Operations/CCF-OpsReadiness-652-20261116 rev-parse --abbrev-ref HEAD
git -C /media/jer/Files/CCF/Operations/CCF-OpsReadiness-652-20261116 rev-parse HEAD
GIT_OPTIONAL_LOCKS=0 git -C /media/jer/Files/CCF/Operations/CCF-OpsReadiness-652-20261116 status --porcelain --branch
find /media/jer/ZX20/CCF-Temp -maxdepth 3 -type d -iname '*ops*' -print
```

Exact output:

```text
2026-11-19T01:07:29Z
## ops/workflow-foundation-v1...origin/ops/workflow-foundation-v1 [ahead 1, behind 1]
 M AGENTS.md
 M Docs/Operations/StorageArchitecture.md
 M Docs/Project/coordination/active-tasks.md
?? Docs/Operations/PostStage1CCFWorktreeEvidenceDisposition-20261118.md
?? Docs/Operations/Stage1StorageRecoveryExecution-20261118.md
?? Docs/Operations/StaleWorktreeDiskPressureHandoff-20261118.md
?? Docs/Project/coordination/archive/
a42eaadc42bda6d474365f39b67a78a9b14bb043
ops/sen-1-652-unity-lease-readiness-20261116-v1
4c7acc051e262c3fe301049b51ee7fc48a9f99a2
## ops/sen-1-652-unity-lease-readiness-20261116-v1...origin/ops/sen-1-652-unity-lease-readiness-20261116-v1
/media/jer/ZX20/CCF-Temp/ops652-final-source
/media/jer/ZX20/CCF-Temp/ops652-optiona-source
/media/jer/ZX20/CCF-Temp/ops652-rehearsal-v62zx56l
/media/jer/ZX20/CCF-Temp/Sen1-741-ExFATQual-20261118T142444Z/base/CCF-Ops
/media/jer/ZX20/CCF-Temp/Sen1-741-ExFATQual-20261118T142541Z/base/CCF-Ops
/media/jer/ZX20/CCF-Temp/Sen1-741-ExFATQual-20261118T142559Z/base/CCF-Ops
/media/jer/ZX20/CCF-Temp/Sen1-741-ExFATQualV2-20261118T143503Z/base/CCF-Ops
/media/jer/ZX20/CCF-Temp/Sen1-741-ExFATQualV3-20261118T144412Z/base/CCF-Ops
/media/jer/ZX20/CCF-Temp/Sen1-741-ExFATQualV4-20261118T145233Z/base/CCF-Ops
/media/jer/ZX20/CCF-Temp/Sen1-741-ExFATQualV4-20261118T145242Z/base/CCF-Ops
/media/jer/ZX20/CCF-Temp/Sen1-741-ExFATQualV4-20261118T145302Z/base/CCF-Ops
/media/jer/ZX20/CCF-Temp/Sen1-741-ExFATQualV5-20261118T145959Z/base/CCF-Ops
/media/jer/ZX20/CCF-Temp/Sen1-741-ExFATQualV6-20261118T150720Z/base/CCF-Ops
/media/jer/ZX20/CCF-Temp/Sen1-741-ExFATQualV6-20261118T150753Z/base/CCF-Ops
/media/jer/ZX20/CCF-Temp/Sen1-752-NativeV6-20261118T153253Z/base/CCF-Ops
/media/jer/ZX20/CCF-Temp/Sen1-752-NativeV6-20261118T153253Z/staged/CCF-Ops
/media/jer/ZX20/CCF-Temp/Sen1-754-Author-20261118T170536Z/staged/CCF-Ops
/media/jer/ZX20/CCF-Temp/Sen1-754-Author-20261118T170536Z/replica/CCF-Ops
```

### E06

all-comparisons; shell exit 0.

Command:

```bash
date -u +%Y-%m-%dT%H:%M:%SZ
python3 -B - <<'PY'
from pathlib import Path
import hashlib,json,subprocess
roots={'D':Path('/media/jer/ZX20/CCF-Ops'),'R':Path('/media/jer/Files/CCF-Primary/CCF-Ops'),'A':Path('/media/jer/Files/CCF/Operations/CCF-OpsReadiness-652-20261116/CCF-Ops'),'F':Path('/media/jer/ZX20/CCF-Temp/ops652-final-source'),'O':Path('/media/jer/ZX20/CCF-Temp/ops652-optiona-source')}
for k,p in roots.items():
 print('ROOT',k,p)
 print('CONFIG',k,(p/'lanes.json').read_text() if k=='R' else '')
files={k:{str(f.relative_to(p)):hashlib.sha256(f.read_bytes()).hexdigest() for f in p.rglob('*') if f.is_file()} for k,p in roots.items()}
print('DEPLOYED SHA256 EVERY FILE')
for f,h in sorted(files['D'].items()):print(h,f)
for left,right in [('D','R'),('D','A'),('R','A'),('A','F'),('A','O')]:
 print('DIFFERENCES',left,right)
 for f in sorted(files[left].keys()|files[right].keys()):
  if files[left].get(f)!=files[right].get(f):print(f,files[left].get(f,'ABSENT'),files[right].get(f,'ABSENT'))
d=json.loads((roots['D']/'deployment.json').read_text());cfg=json.loads((roots['D']/'lanes.json').read_text());cfg.pop('lanes')
print('CONFIG_HASH_COMPACT_SORTED',hashlib.sha256(json.dumps(cfg,sort_keys=True,separators=(',',':')).encode()).hexdigest())
print('ACTUAL_LANES',len(json.loads((roots['D']/'lanes.json').read_text())['lanes']))
PY
```

Exact output:

```text
2026-11-19T01:07:49Z
ROOT D /media/jer/ZX20/CCF-Ops
CONFIG D
ROOT R /media/jer/Files/CCF-Primary/CCF-Ops
CONFIG R {
  "schema": 1,
  "storage": {
    "root": "/media/jer/ZX20",
    "uuid": "18D6-896E",
    "fstype": "exfat",
    "device": "/dev/sda2",
    "minimum_free_bytes": 16106127360
  },
  "repository": "/media/jer/ZX20/CCF-Repository.git",
  "board": "board/active.md",
  "lanes": [
    {
      "lane": "U1",
      "path": "/media/jer/ZX20/CCF-U1",
      "purpose": "primary Unity; Library warm-up deferred",
      "unity": true
    },
    {
      "lane": "U2",
      "path": "/media/jer/ZX20/CCF-U2",
      "purpose": "secondary Unity; Library warm-up deferred",
      "unity": true
    },
    {
      "lane": "Integration",
      "path": "/media/jer/ZX20/CCF-Integration",
      "purpose": "clean integration; serialized single writer",
      "unity": false
    },
    {
      "lane": "NonUnity",
      "path": "/media/jer/ZX20/CCF-NonUnity",
      "purpose": "offline tools/tests/docs; never launch Unity",
      "unity": false
    }
  ],
  "git_stores": [
    {
      "name": "ZX20",
      "git_dir": "/media/jer/ZX20/CCF-Repository.git"
    },
    {
      "name": "legacy-SSD",
      "git_dir": "/home/jer/CCF/.git"
    }
  ]
}

ROOT A /media/jer/Files/CCF/Operations/CCF-OpsReadiness-652-20261116/CCF-Ops
CONFIG A
ROOT F /media/jer/ZX20/CCF-Temp/ops652-final-source
CONFIG F
ROOT O /media/jer/ZX20/CCF-Temp/ops652-optiona-source
CONFIG O
DEPLOYED SHA256 EVERY FILE
d1ce3ad775e16d80fea6779c91bfa66adbd2e019edfbbbb9280be563be60e08f AGENTS.md
e2fe59102b8b408a313f48807323c5d1befb6e89407e8a50dfdf169cd8e7a68d ActiveTasksTransition.md
625065153e9b69634723b6a6f678fa2371350cb7f436393461a62a07a28e2335 CLAUDE.md
5fc54e196e1905a39a5f99bf83ca1a0b8033e2717b2e6a924a861dc54cfb9486 G4BCorrections.md
0219dfb2666a8c444c6c289782e383d6656d44f1af34d0fd55606a725d61e62c G4WorkflowFoundationReviewPacket.md
249be391e172e9d80f7d3d3419af961850b84c40fb2f15ba7b1d0469649eb517 README.md
a9077b50dc25523cbf8d0711b23a9ad392c2d27e38858e8e073ed8277125a018 WorkflowFoundationHandoff.md
05d1f8cc3df50180e226e0da16994bd0cc3c6b77d025acb7b18fa5889763b60d WorkflowFoundationVerification.md
4fa750eba64a8a072feb305a7db2cce17ddedfa62dbd276305116c03ab853994 board/active.md
8c66b290c247708846cb8b59f83626a10a3fa1d052397a3316dc5f23fc157b18 deployment.json
22bb354343c699cc78903d4a5d1b1b13548e126a4a04351f54f25586905195ed deployment.json.bak-20261115T225732Z
bc27de755194bd4c8ff34d24f55583db52376915301bfb22fabbb6405d7ca0e5 deployment.json.bak-20261115T230935Z
2034c77fac082ec8107deef073605ce9314d594e79568e28f32aa8584dc4f5bc deployment.json.bak-20261115T231825Z
3775cba7ed446c61ae6fa2977da5cf0984ff7d888213ad9fa33396b1aa11c954 environment.sh
c1d147196e1b8d57008e44545db3ba7f3fecea0ae12094dbf3e05f1c782b7d9e lanes.json
4237f949dd16a8d77060a1673e8e4eadab6c3f6e4d34ad408da4badd0583e553 ledger/2026-10-15-CCF-CANONICAL-WORKFLOW.md
168584c1900f5639eccfe637a43d7f3b32f9dab286db85864d9b903d1c6b97de ledger/2026-10-15-CCF-DUMMY-T1.md
2f221c80123261d71999f772937a668ea63221bea8211bf7a1a3d2ef41143bb5 ledger/2026-10-15-T2-S1-CANDIDATE-V2-PREFLIGHT.md
9420b762200fa8599746f5e3901c4924275ef089958c178261f151c3053e575b ledger/2026-11-12-CCF-412-D073-FR10-ONE-GO.md
23bf09b303aec430d8e6b8f1a76a0d7d34a055e04fd364423df09ae3e12048ba ledger/README.md
a0813687f28334369cea95796131edc999cf6a24cc8cc65ef4fc93cd279ff6b7 pilot/README.md
7c65c61adec2c4ffa30b95529da5edee9f930241e0c84c66c67aa5bd6b2dfb29 pilot/manager-packet.md
93aafd766e02e31ceee103bc8615d866995f5c2f8a94c2ba7ae53ebe2df1bd35 pilot/t2-pilot-packet.md
4b1de83798691c697141f6a8e2b21561a27a3a741ef179f2932d2e38c37703a3 pilot/unity-gate-example.md
0fb67b1283052008a42fe40dbeef6dea5f6710ca5b71c7272d99bf581b3843f0 roles/execution-manager.md
58bafc0dee42158816687a71becb73f8a2cf4d1f9dac192bfbe4fe3a56207d6a roles/reviewer.md
120f1deb006aa56a9d4b9643917f855548b5da80432a7166442ecc6163eee815 roles/unity-operator.md
900d02566195f6a0e1487edd34dd11f9fc6347a6869b87f4a19f106934545da1 scripts/__pycache__/_ccf.cpython-312.pyc
ab4343efc7d1b82c5a4aca747d0e09c614779cfd9aa406a0dd681d204f5de001 scripts/_ccf.py
e7102a83ad942cc7365cb22db9a8f79823744ea98bc2cf8a6738a11f1897173f scripts/lane-status
a776ec42a13224936a5c7c6a68ec2ed7ddc4e2cf21a96e287e6843a31e243e43 scripts/packet-lint
54cf0a95561d0a8c350d89adbee83f393990b728d6127b94f69c587260f048e5 scripts/unity-lock
2875466cf1d309cb2aa65ff34699d7fa3bb2eb6595211c525d42d6c1a4ceba19 state/board-history.jsonl
e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855 state/unity/guard
dfb506443e1f41b6d5f2be23e2c2b38d0ee76fe78d4d4ba2446fbc962422fffa state/unity/history.jsonl
e063274c82b1c38a8fc714ccc1ef3b19fb21d04494dca18a56a17760c60eb77a tests/fixtures/canonical-style.md
ca6a89ad0b4422575aab3234d5293672ccf756e92f952782d4474f8fcd1fee24 tests/fixtures/conflicting-unity-ownership.md
8f82d136a6ced39892a03ea03902d93c94fb270aedd1a80abec09d3d5b84edb9 tests/fixtures/handoff-placeholder.md
e7e54f12e455304b501ed5d1f6b6c5a789b20e5cf3413aaf60d2304cffcd44d3 tests/fixtures/missing-base.md
35c9831ff0d67a02151c99c03fa5ee01e7372430be49b3432cb6f0bf8c536856 tests/fixtures/missing-role-file.md
719066818259794ffcab48073682af45e968cbae2cca2c341ccca568deb92171 tests/fixtures/missing-stop-condition.md
9e1110ce0a78241a64508de7269d3c92e9e13ea2a168521263eaba151346c7bb tests/fixtures/missing-unity-required.md
16e0e3ce718d132b17eb24812a60ed2efd97fc492dd3562acec5a362c0f139e2 tests/fixtures/missing-verification-tier.md
b90081a1573de40a1deb64ed4e7bdf3907ec258ba9f1142313bb02ebb21c6113 tests/fixtures/nonunity-lane-head.md
7d6bb96b2602b70db57084099e8dcb3870f8328d6b41597619de9b8266ce6be9 tests/fixtures/partial.md
9c7c15857dab00f42ec9644136964d650916498b1053f6d9867c3958f2b23dfe tests/fixtures/stop-placeholder.md
0bb1fa102a8f9c11cc025f0b84b36953d31cf7e2af79a875490b1fa9d066cb21 tests/fixtures/unity-nonunity-lane.md
281f9c52f9d168c5b75cd6fecfa7ac96cd0e044e7a44dd3508e4f273f416c00b tests/fixtures/valid-unity.md
7c65c61adec2c4ffa30b95529da5edee9f930241e0c84c66c67aa5bd6b2dfb29 tests/fixtures/valid.md
2a12a19d848ae789a1404c7ebd5285457e061b0a7142b91de95ed0b79cdeeaf3 tests/self-test.py
DIFFERENCES D R
AGENTS.md d1ce3ad775e16d80fea6779c91bfa66adbd2e019edfbbbb9280be563be60e08f a7d8aa3236ebdd8b9e1d0c24090ceba61353ab56d8fba35b024078a0cbfe0dbe
G4WorkflowFoundationReviewPacket.md 0219dfb2666a8c444c6c289782e383d6656d44f1af34d0fd55606a725d61e62c ABSENT
README.md 249be391e172e9d80f7d3d3419af961850b84c40fb2f15ba7b1d0469649eb517 af82c798610906fd8516cdedd8c1407e6a0bde821e6b8108c710283407117401
WorkflowFoundationHandoff.md a9077b50dc25523cbf8d0711b23a9ad392c2d27e38858e8e073ed8277125a018 ABSENT
board/active.md 4fa750eba64a8a072feb305a7db2cce17ddedfa62dbd276305116c03ab853994 cb6bcc40e10a4668168bed9b90e5a7f9e3bb167997f0b5248e6fd850d3e2b24c
deployment.json 8c66b290c247708846cb8b59f83626a10a3fa1d052397a3316dc5f23fc157b18 ABSENT
deployment.json.bak-20261115T225732Z 22bb354343c699cc78903d4a5d1b1b13548e126a4a04351f54f25586905195ed ABSENT
deployment.json.bak-20261115T230935Z bc27de755194bd4c8ff34d24f55583db52376915301bfb22fabbb6405d7ca0e5 ABSENT
deployment.json.bak-20261115T231825Z 2034c77fac082ec8107deef073605ce9314d594e79568e28f32aa8584dc4f5bc ABSENT
environment.sh 3775cba7ed446c61ae6fa2977da5cf0984ff7d888213ad9fa33396b1aa11c954 0dde119a47f668d03f93445cc606a054465bf6eea49155dc0f71eb2eb41d89d4
lanes.json c1d147196e1b8d57008e44545db3ba7f3fecea0ae12094dbf3e05f1c782b7d9e 53d3ed9c79adbe032b406d1f1ea4342613532b4c432e6df2c5d354f9719e77f9
ledger/2026-11-12-CCF-412-D073-FR10-ONE-GO.md 9420b762200fa8599746f5e3901c4924275ef089958c178261f151c3053e575b ABSENT
scripts/__pycache__/_ccf.cpython-312.pyc 900d02566195f6a0e1487edd34dd11f9fc6347a6869b87f4a19f106934545da1 ABSENT
scripts/_ccf.py ab4343efc7d1b82c5a4aca747d0e09c614779cfd9aa406a0dd681d204f5de001 b12a613379d69c5afa8e4c20ae0fcdcff6b8772c39501f60c01933263cf46923
scripts/lane-status e7102a83ad942cc7365cb22db9a8f79823744ea98bc2cf8a6738a11f1897173f 7fca072046997d43caf51a87397d2735c579cc2c1732515cfa088c9dff4b3761
state/board-history.jsonl 2875466cf1d309cb2aa65ff34699d7fa3bb2eb6595211c525d42d6c1a4ceba19 ABSENT
state/unity/guard e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855 ABSENT
state/unity/history.jsonl dfb506443e1f41b6d5f2be23e2c2b38d0ee76fe78d4d4ba2446fbc962422fffa ABSENT
DIFFERENCES D A
AGENTS.md d1ce3ad775e16d80fea6779c91bfa66adbd2e019edfbbbb9280be563be60e08f a7d8aa3236ebdd8b9e1d0c24090ceba61353ab56d8fba35b024078a0cbfe0dbe
DeploymentProvenance.md ABSENT a08fb8a5d38e82ab3dad1b6b1e0df7280cf28e1751fd1359b61cc2947b288db4
G4WorkflowFoundationReviewPacket.md 0219dfb2666a8c444c6c289782e383d6656d44f1af34d0fd55606a725d61e62c ABSENT
README.md 249be391e172e9d80f7d3d3419af961850b84c40fb2f15ba7b1d0469649eb517 a1911b01ece918d4eb52bfde230fe197e34f839a8c41d79f67555d9a560598e4
WorkflowFoundationHandoff.md a9077b50dc25523cbf8d0711b23a9ad392c2d27e38858e8e073ed8277125a018 ABSENT
board/active.md 4fa750eba64a8a072feb305a7db2cce17ddedfa62dbd276305116c03ab853994 cb6bcc40e10a4668168bed9b90e5a7f9e3bb167997f0b5248e6fd850d3e2b24c
deployment.json 8c66b290c247708846cb8b59f83626a10a3fa1d052397a3316dc5f23fc157b18 be9fbcd04a2eadefde65b4aad791cbc30eb4633b57725732645672da9f113b34
deployment.json.bak-20261115T225732Z 22bb354343c699cc78903d4a5d1b1b13548e126a4a04351f54f25586905195ed ABSENT
deployment.json.bak-20261115T230935Z bc27de755194bd4c8ff34d24f55583db52376915301bfb22fabbb6405d7ca0e5 ABSENT
deployment.json.bak-20261115T231825Z 2034c77fac082ec8107deef073605ce9314d594e79568e28f32aa8584dc4f5bc ABSENT
environment.sh 3775cba7ed446c61ae6fa2977da5cf0984ff7d888213ad9fa33396b1aa11c954 fc1744b25c82fd136c176c8f678a3245dd718d7e98985a1ac5a9f86de51c3409
lanes.json c1d147196e1b8d57008e44545db3ba7f3fecea0ae12094dbf3e05f1c782b7d9e f73d1c08556ebb5a2befeeb7dd9fee7cde08184c935a2af73aded2c8dcefe477
ledger/2026-11-12-CCF-412-D073-FR10-ONE-GO.md 9420b762200fa8599746f5e3901c4924275ef089958c178261f151c3053e575b ABSENT
roles/unity-operator.md 120f1deb006aa56a9d4b9643917f855548b5da80432a7166442ecc6163eee815 e44e7eb89d33a82d4e3dc5987572565e394d12a60ec82733d3dfd4bb103206db
scripts/__pycache__/_ccf.cpython-312.pyc 900d02566195f6a0e1487edd34dd11f9fc6347a6869b87f4a19f106934545da1 ABSENT
scripts/_ccf.py ab4343efc7d1b82c5a4aca747d0e09c614779cfd9aa406a0dd681d204f5de001 24a321466e6dae61ba950c4cbc4f918e12f7d6be8a6af628341c5aa8efe0b435
scripts/unity-lock 54cf0a95561d0a8c350d89adbee83f393990b728d6127b94f69c587260f048e5 8fa0c5ada367f109c9437b05bd37b42c7ce78472d6f630634df8745fcb220ac8
scripts/verify-deployment ABSENT ee001216fb020ae33ed54f4c317e2c8e19ce4bdb407f4e6bb8ae457248a02386
state/board-history.jsonl 2875466cf1d309cb2aa65ff34699d7fa3bb2eb6595211c525d42d6c1a4ceba19 ABSENT
state/unity/guard e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855 ABSENT
state/unity/history.jsonl dfb506443e1f41b6d5f2be23e2c2b38d0ee76fe78d4d4ba2446fbc962422fffa ABSENT
tests/self-test.py 2a12a19d848ae789a1404c7ebd5285457e061b0a7142b91de95ed0b79cdeeaf3 1bd03f1520e21db6f6bd00b963e09437267ebf4604ad6772c8e5fffffa9e6e50
DIFFERENCES R A
DeploymentProvenance.md ABSENT a08fb8a5d38e82ab3dad1b6b1e0df7280cf28e1751fd1359b61cc2947b288db4
README.md af82c798610906fd8516cdedd8c1407e6a0bde821e6b8108c710283407117401 a1911b01ece918d4eb52bfde230fe197e34f839a8c41d79f67555d9a560598e4
deployment.json ABSENT be9fbcd04a2eadefde65b4aad791cbc30eb4633b57725732645672da9f113b34
environment.sh 0dde119a47f668d03f93445cc606a054465bf6eea49155dc0f71eb2eb41d89d4 fc1744b25c82fd136c176c8f678a3245dd718d7e98985a1ac5a9f86de51c3409
lanes.json 53d3ed9c79adbe032b406d1f1ea4342613532b4c432e6df2c5d354f9719e77f9 f73d1c08556ebb5a2befeeb7dd9fee7cde08184c935a2af73aded2c8dcefe477
roles/unity-operator.md 120f1deb006aa56a9d4b9643917f855548b5da80432a7166442ecc6163eee815 e44e7eb89d33a82d4e3dc5987572565e394d12a60ec82733d3dfd4bb103206db
scripts/_ccf.py b12a613379d69c5afa8e4c20ae0fcdcff6b8772c39501f60c01933263cf46923 24a321466e6dae61ba950c4cbc4f918e12f7d6be8a6af628341c5aa8efe0b435
scripts/lane-status 7fca072046997d43caf51a87397d2735c579cc2c1732515cfa088c9dff4b3761 e7102a83ad942cc7365cb22db9a8f79823744ea98bc2cf8a6738a11f1897173f
scripts/unity-lock 54cf0a95561d0a8c350d89adbee83f393990b728d6127b94f69c587260f048e5 8fa0c5ada367f109c9437b05bd37b42c7ce78472d6f630634df8745fcb220ac8
scripts/verify-deployment ABSENT ee001216fb020ae33ed54f4c317e2c8e19ce4bdb407f4e6bb8ae457248a02386
tests/self-test.py 2a12a19d848ae789a1404c7ebd5285457e061b0a7142b91de95ed0b79cdeeaf3 1bd03f1520e21db6f6bd00b963e09437267ebf4604ad6772c8e5fffffa9e6e50
DIFFERENCES A F
README.md a1911b01ece918d4eb52bfde230fe197e34f839a8c41d79f67555d9a560598e4 d21387d54694feae0e89129684975f5f295b343b43d37f23224efbc8a73095e3
deployment.json be9fbcd04a2eadefde65b4aad791cbc30eb4633b57725732645672da9f113b34 0ff48c916d78787a182ea8ffe0ab77aec8b2a02c1f7c0eab7734a08fbf552a3d
roles/unity-operator.md e44e7eb89d33a82d4e3dc5987572565e394d12a60ec82733d3dfd4bb103206db 120f1deb006aa56a9d4b9643917f855548b5da80432a7166442ecc6163eee815
scripts/_ccf.py 24a321466e6dae61ba950c4cbc4f918e12f7d6be8a6af628341c5aa8efe0b435 1295def55a4821ccd00423f65ab58e64a3be916dc18b127ed99ae1099c3e7677
scripts/unity-lock 8fa0c5ada367f109c9437b05bd37b42c7ce78472d6f630634df8745fcb220ac8 54cf0a95561d0a8c350d89adbee83f393990b728d6127b94f69c587260f048e5
tests/self-test.py 1bd03f1520e21db6f6bd00b963e09437267ebf4604ad6772c8e5fffffa9e6e50 a25be4f899da5d5e6711956556c118375ea6581b397d005cce87bfd10654e340
DIFFERENCES A O
README.md a1911b01ece918d4eb52bfde230fe197e34f839a8c41d79f67555d9a560598e4 5b7abb83b746fe0c0219a926ec9cf6fc682b7acc1ed7d9d5ad70e69d3628db07
roles/unity-operator.md e44e7eb89d33a82d4e3dc5987572565e394d12a60ec82733d3dfd4bb103206db 120f1deb006aa56a9d4b9643917f855548b5da80432a7166442ecc6163eee815
scripts/unity-lock 8fa0c5ada367f109c9437b05bd37b42c7ce78472d6f630634df8745fcb220ac8 e465303493e1044fac9926c12a89b0f64f05b2aba786469de6f3645f78e41d3e
tests/self-test.py 1bd03f1520e21db6f6bd00b963e09437267ebf4604ad6772c8e5fffffa9e6e50 d4ebd9225b4cde8e2c27950d24f7d97068e89ec6930f34c83b035288cbaab4f9
CONFIG_HASH_COMPACT_SORTED ed642a520d25470f1192cbbd88b4ccede16bebc6259a5bb1db039df45864cca6
ACTUAL_LANES 25
```

### E07

storage-status; shell exit 0.

Command:

```bash
date -u +%Y-%m-%dT%H:%M:%SZ
python3 -B /media/jer/ZX20/CCF-Ops/scripts/lane-status --storage-only
printf 'deployed_exit=%s\n' "$?"
python3 -B /media/jer/Files/CCF-Primary/CCF-Ops/scripts/lane-status --storage-only
printf 'repository_exit=%s\n' "$?"
python3 -B /media/jer/Files/CCF/Operations/CCF-OpsReadiness-652-20261116/CCF-Ops/scripts/verify-deployment --root /media/jer/ZX20/CCF-Ops --manifest /media/jer/ZX20/CCF-Ops/deployment.json
printf 'live_receipt_exit=%s\n' "$?"
```

Exact output:

```text
2026-11-19T01:08:34Z
READY / PASS | device=/dev/sdc2 rw=True | free=367830237184 bytes (342.57 GiB) threshold=16106127360 bytes (15.00 GiB)
deployed_exit=0
ERROR: ZX20 mount identity mismatch: {'source': '/dev/sdc2', 'target': '/media/jer/ZX20', 'fstype': 'exfat', 'uuid': '18D6-896E'}
repository_exit=2
PASS: deployment provenance (28 source hashes, 7 pinned deployed overrides, 1 config projection, 5 reasoned live-state entries)
live_receipt_exit=0
```

### E08

unity-readonly; shell exit 0.

Command:

```bash
date -u +%Y-%m-%dT%H:%M:%SZ
python3 -B - <<'PY'
import sys,runpy,os
from pathlib import Path
p=Path('/media/jer/ZX20/CCF-Ops/scripts/unity-lock')
# No installed code changed: replace only mkdir/open's potentially writable guard effects in memory.
original_open=Path.open
state=p.parent.parent/'state/unity'
def existing_mkdir(self,*args,**kwargs):
 if self != state or not self.is_dir(): raise RuntimeError('mkdir blocked: '+str(self))
def readonly_open(self,mode='r',*args,**kwargs):
 if self==state/'guard' and mode=='a+': return original_open(self,'r',*args,**kwargs)
 if any(c in mode for c in 'wax+'): raise RuntimeError('write open blocked: '+str(self))
 return original_open(self,mode,*args,**kwargs)
def audit(event,args):
 if event=='open' and args[2] & (os.O_WRONLY|os.O_RDWR|os.O_CREAT|os.O_TRUNC|os.O_APPEND): raise RuntimeError('write audit blocked: '+str(args[0]))
 if event in {'os.mkdir','os.remove','os.rename','os.rmdir','os.truncate','os.chmod','os.chown','os.utime'}: raise RuntimeError('mutation audit blocked: '+event)
Path.mkdir=existing_mkdir
Path.open=readonly_open
sys.addaudithook(audit)
sys.path.insert(0,str(p.parent))
sys.argv=[str(p),'status','--json']
runpy.run_path(str(p),run_name='__main__')
PY
```

Exact output:

```text
2026-11-19T01:08:35Z
{
  "status": "free",
  "metadata": null,
  "unity_editor_pids": []
}
```

### E09

full-status; shell exit 0.

Command:

```bash
date -u +%Y-%m-%dT%H:%M:%SZ
python3 -B - <<'PY'
import subprocess,time
cmd=['python3','-B','/media/jer/ZX20/CCF-Ops/scripts/lane-status']
t=time.monotonic()
try:
 p=subprocess.run(cmd,capture_output=True,text=True,timeout=55)
 print(p.stdout,end='');print(p.stderr,end='');print('exit=',p.returncode,'elapsed_seconds=',round(time.monotonic()-t,3))
except subprocess.TimeoutExpired as e:
 print('TIMEOUT 55 seconds; not repeated');print((e.stdout or b'').decode() if isinstance(e.stdout,bytes) else (e.stdout or ''))
 print('elapsed_seconds=',round(time.monotonic()-t,3))
PY
```

Exact output:

```text
2026-11-19T01:08:44Z
TIMEOUT 55 seconds; not repeated

elapsed_seconds= 55.062
```

### E10

owners-summary; shell exit 0.

Command:

```bash
date -u +%Y-%m-%dT%H:%M:%SZ
python3 -B - <<'PY'
from pathlib import Path
import hashlib,json,sys,subprocess,base64
p=Path('/media/jer/ZX20/CCF-Ops');sys.path.insert(0,str(p/'scripts'));import _ccf
cfg=json.loads((p/'lanes.json').read_text());rows=_ccf.board_read(p/'board/active.md')
main=base64.b64decode(json.loads(subprocess.check_output(['gh','api','repos/jermur788/CCF/contents/Docs/Project/coordination/active-tasks.md?ref=main']))['content']).decode()
local=Path('/media/jer/Files/CCF-Primary/Docs/Project/coordination/active-tasks.md').read_text()
print('MAIN_COORD_SHA256',hashlib.sha256(main.encode()).hexdigest(),'lines',len(main.splitlines()))
print('BOARD_ROWS',len(rows));print('GIT_STORES',json.dumps(cfg['git_stores'],sort_keys=True))
print('BOARD OWNERS: Task ID | State | Lane | Owner')
for r in rows:print(r['Task ID'],'|',r['State'],'|',r['Lane'],'|',r['Owner'])
print('LANE_MATCHES: lane | exists | direct_board_task_ids | main_lines | local_lines')
for l in cfg['lanes']:
 terms=[l['path'],l['lane'],'CCF-'+l['lane']]
 match=[r['Task ID'] for r in rows if r['Lane'] in terms]
 ml=[str(i) for i,line in enumerate(main.splitlines(),1) if l['path'] in line or ('| '+l['lane']+' |') in line or ('CCF-'+l['lane']+'`') in line]
 ll=[str(i) for i,line in enumerate(local.splitlines(),1) if l['path'] in line or ('| '+l['lane']+' |') in line]
 print(l['lane'],'|',Path(l['path']).exists(),'|',','.join(match) or 'NONE','|',','.join(ml) or 'NONE','|',','.join(ll) or 'NONE')
state=p/'state/unity';print('STATE_ENTRIES',sorted(x.name for x in state.iterdir()));print('LOCK_EXISTS',(state/'lock.json').exists())
history=[json.loads(x) for x in (state/'history.jsonl').read_text().splitlines()]
print('HISTORY_RECORDS',len(history),'FIRST',history[0]['timestamp'],'LAST',history[-1]['timestamp'])
print('HISTORY DISTINCT ACTORS')
for x in sorted(set(str(h.get('actor')) for h in history)):print(x)
print('LAST_HISTORY_EXACT',json.dumps(history[-1],sort_keys=True))
print('BYTECODE',[(str(x),x.stat().st_mtime_ns,hashlib.sha256(x.read_bytes()).hexdigest()) for x in (p/'scripts').rglob('*.pyc')])
PY
```

Exact output:

```text
2026-11-19T01:09:03Z
MAIN_COORD_SHA256 cd38c93a0729f624abf34ba31ac1861e1a2b46b0a063a3ee2af759cad3c1672d lines 981
BOARD_ROWS 44
GIT_STORES [{"git_dir": "/media/jer/Files/CCF/Repository.git", "name": "Files-canonical"}, {"git_dir": "/media/jer/Files/CCF-Legacy/.git", "name": "legacy-SSD"}]
BOARD OWNERS: Task ID | State | Lane | Owner
CCF-652-OPS-READINESS | READY | None | RESERVED ONLY / NOT RUNNING / SOURCE BLOCKED: sole prospective #652 source-only writer OpenAI/Codex gpt-6.1-sol medium, native T3 Code, session 9bcdfde6-699b-41f7-befb-acc9a7551f09, host jer-laptop (registered https://github.com/jermur788/CCF/issues/652#issuecomment-6978301260); predecessor HANDED-OFF (#652 comment 6932553469). Row refreshed 2026-11-18 by separate physical board custodian USER jermur788 via Claude Opus 5.5 session 81a16309-bfa0-4b74-9c76-0765d38a1926 (custodian-only, not a source writer) per Manager #731 packet b53216ab
CCF-647-SHARED-SAVE-FIXTURE-INCORPORATION | DONE-PENDING-LEDGER | None | NONE active (released by #693); historical owner record: RESERVED ONLY: existing OpenAI gpt-6.1-sol High / Codex Linux #647 worker; NOT RUNNING
CCF-544-SHARED-SAVE-SOURCE-CUSTODY | DONE-PENDING-LEDGER | None | NONE active (HANDED-OFF)
CCF-629-L1-GEOMETRY-DISPATCH | DONE-PENDING-LEDGER | None | NONE (source author HANDED-OFF/CLOSED)
CCF-633-L1-SOURCE-REVIEW | DONE-PENDING-LEDGER | None | NONE (independent reviewer HANDED-OFF/CLOSED)
CCF-635-L1-ADMISSION-DECISION | DONE-PENDING-LEDGER | None | NONE (historic #424 decision complete; #425 sole current Manager)
CCF-638-L1-SOURCE-INCORPORATION | DONE-PENDING-LEDGER | None | NONE active (released by #693); historical owner record: RESERVED ONLY: existing registered OpenAI GPT-6.1 Sol High Linux #638 worker; NOT RUNNING
CCF-460-COMPACT-NATIVE-STUDY | DONE-PENDING-LEDGER | None | NONE (study completed; no source writer)
CCF-412-PREDATOR-FABLE-A-D073 | WAITING | ResearchCopyQual391 | USER (jermur788) is the named owner of the personal Editor playtest on this lane, NOT STARTED, valid only after Manager conditional GO. No agent writer or Unity worker active: Fable #412 parent HANDED-OFF; children #422 #423 #426 #427 #435 #437 all HANDED-OFF/CLOSED
CCF-413-HP-FABLE-B-D074-D076-SPEC-REMOTE | DONE-PENDING-LEDGER | None | NONE (remote HP #413 finished; GitHub #413 HANDED-OFF / MANAGER DOC REVIEW APPROVED and CLOSED, read 2026-11-12; no active writer)
CCF-409-HP-D073-SPEC-REMOTE | DONE-PENDING-LEDGER | None | NONE (remote HP docs-only worker finished; GitHub #409 HANDED-OFF/CLOSED; no active writer)
CCF-406-MANAGEMENT-READOUT | DONE-PENDING-LEDGER | None | NONE (source qualification complete; Manager review pending)
CCF-403-CADENCE-V1-UNITY-QUAL | DONE-PENDING-LEDGER | None | NONE (Fable #402 orchestration complete; no active writer or Unity worker)
SCENARIO-ONE-UI-VISUAL-SYSTEM-V2 | READY | None | NONE (qualified; no active writer)
SCENARIO-ONE-UI-VISUAL-SYSTEM-V1 | READY | None | NONE (UI qualification and integration completed by SCENARIO-ONE-UI-NONUI-INTEGRATION-RECONCILIATION-V1, Claude Sonnet 5.5 in T3 thread 363f2c2a-fc38-4707-8293-901f31825bdf; no active writer)
SCENARIO-ONE-UI-NONUI-INTEGRATION-RECONCILIATION-V1 | READY | None | NONE (integrator thread 363f2c2a-fc38-4707-8293-901f31825bdf finished; no active writer)
SCENARIO-ONE-PLAYER-TEST-NON-UI-REMEDIATION-V1 | READY | U3 | NONE; SOL/Codex V5 continuation-3 test complete
SCENARIO-TWO-SPECIES-QUANTITATIVE-CLOSURE-AND-RUNTIME-V1 | WAITING | U2 | T3 orchestrator c1ff2d8a; no active implementation writer
CCF-S1-SEQUENTIAL-OBJECTIVES-IMPLEMENTATION-V1 | DONE-PENDING-LEDGER | None | NONE (independent review and integration complete; no active writer)
CCF-S1-GUIDED-ONBOARDING-RACK-UX-FIX-1 | DONE-PENDING-LEDGER | None | NONE (implementation, focused re-review and integration complete)
CCF-S1-GUIDED-ONBOARDING-RACK-UX-V1 | DONE-PENDING-LEDGER | None | NONE (implementation, review, fixes and integration complete)
CCF-S1-GUIDED-ONBOARDING-RACK-UX-REVIEW-V1 | DONE-PENDING-LEDGER | None | NONE (independent review completed)
SCENARIO-TWO-SPECIES-MODEL-FOUNDATION-COMPLETE-V1 | DONE-PENDING-LEDGER | None | NONE (integration complete; no active writer)
CCF-DENSITY-INTEGRATION-INPUT | READY | None | NONE
CCF-CANDIDATE-V2 | READY | None | NONE
CCF-T2-OPERATIONAL-PILOT | WAITING | None | NONE
CCF-COMPONENT-BA | READY | CCF-DensityInstrumentation | NONE
CCF-GROWFOR | READY | CCF-Growfor | NONE
CCF-TIER3-PREFLIGHT | WAITING | CCF-Tier3Preflight | NONE
CCF-TIER3-REPIN | BLOCKED | None | Claude Code session (Sonnet 5.5) for Overall Manager
CCF-MODEL1-HISTORICAL-FIXTURE | DONE-PENDING-LEDGER | None | NONE
CCF-MODEL2-LANE | WAITING | None | NONE
CCF-674-MULTI-P0-ANCESTRY | READY | MultiP0Proof674 | RESERVED ONLY: opencode-go DeepSeek V4.1 Flash, owner-assigned; NOT RUNNING
CCF-683-MULTI-P0-H-REVIEW | READY | None | READ-ONLY REVIEWER ONLY: OpenAI Codex gpt-6.1-sol High (thread 942b9ca2); no writer; #674 owner retained
CCF-687-P0P1-INTEGRATION | VERIFY | MultiP0P1Integ687 | OpenAI Codex GPT-6.2 Sol Medium (sole Child A writer; local proof complete; retained for independent review)
CCF-697-MULTI-P1-CHILD-B | BLOCKED | MultiP1ChildB697 | EXCLUSIVE RESERVED: OpenAI Codex GPT-6.2 Sol Medium (this T3 Code / Codex session on jer-laptop); source not started
CCF-690-SEN1-SAVE-A0 | BLOCKED | Sen1SaveA0ZX20NonUnity707 | PROSPECTIVE SOURCE WRITER ASSIGNED, SOURCE START HELD / NOT RUNNING: Anthropic Claude Opus 5.5 (claude-opus-5-5), Claude Code in T3 Code, jer-laptop, session 8b161722-5c96-4e4f-aa9b-1cd9df4c0dee (READINESS-ONLY receipt https://github.com/jermur788/CCF/issues/690#issuecomment-6963560917); assigned 2026-11-17 by #714 assignment custodian USER jermur788 (actor session 68e4e51d) per Manager #700 packet fa297f00. Former writer fe9ea65b REVOKED AND RELEASED via #713; its void approval does not transfer
CCF-710-MULTI-P1-R-CUSTODY | BLOCKED | MultiP1ModeRepair710 | CUSTODY RETAINED: user jermur788 (physical custodian); NO ACTIVE WRITER; #710 writer Claude Opus 5.5 finished; NOT RUNNING
CCF-715-MULTI-P1-L4-READONLY | DONE-PENDING-LEDGER | None | NONE (one-file review handed off by OpenAI Codex GPT-6.2 Sol High, jer-laptop, session 840d9ed1-6323-499b-83b8-990a749587af)
CCF-720-MULTI-P1-L4-LIFECYCLE-PILOT | DONE-PENDING-LEDGER | None | NONE (sole worker Claude Opus 5.5, Claude Code in T3 Code, jer-laptop, handed off; lease released 2026-11-18T00:45:38Z)
CCF-725-MULTI-F0-ANNUAL | DONE-PENDING-LEDGER | MultiAnnualF0Child725 | EXCLUSIVE IMMUTABLE SOURCE CUSTODY: owner jermur788 via NEW #735 native T3 Codex gpt-6.1-sol medium, session 01a1eeb5-2429-78e1-9783-3923470314f0; no source writer
CCF-732-MULTI-YEAR1-NATIVE-QUAL | DONE-PENDING-LEDGER | None | NONE (qualification complete: resumed worker Claude Opus 5.5 session 32188791 handed off; Manager #730 APPROVED focused independent native Year-1 qualification; GitHub #732 closed; lease released 2026-11-18T12:41:21Z by #734 custodian USER jermur788 via Claude Opus 5.5 session dd9143ff-82d8-4eb7-a74d-8b413009cda2, custodian only, not a worker)
CCF-755-H0A-CORE-LANE | READY | MultiH0ACore | RESERVED BY Manager #730 / NOT RUNNING / NO ACTIVE WRITER (writer UNASSIGNED): empty lane created by temporary G1 physical setup custodian Claude Sonnet 5.5 (Claude Code via T3 Code, jer-laptop) under Manager grant https://github.com/jermur788/CCF/issues/755#issuecomment-6986127467; lane custody stays with Manager #730 and the owner, handed back at #755 Phase1 result
CCF-755-H0A-VIEWER-LANE | READY | MultiH0AViewer | RESERVED BY Manager #730 / NOT RUNNING / NO ACTIVE WRITER (writer UNASSIGNED): empty lane created by temporary G1 physical setup custodian Claude Sonnet 5.5 (Claude Code via T3 Code, jer-laptop) under Manager grant https://github.com/jermur788/CCF/issues/755#issuecomment-6986127467; lane custody stays with Manager #730 and the owner, handed back at #755 Phase1 result
LANE_MATCHES: lane | exists | direct_board_task_ids | main_lines | local_lines
Q10 | True | NONE | NONE | NONE
U1 | True | NONE | NONE | 50
U2 | True | SCENARIO-TWO-SPECIES-QUANTITATIVE-CLOSURE-AND-RUNTIME-V1 | NONE | 23,51
U3 | True | SCENARIO-ONE-PLAYER-TEST-NON-UI-REMEDIATION-V1 | 461,528,536,542,550,554,556,558,560 | 52
Integration | True | NONE | NONE | 53
NonUnity | True | NONE | NONE | 63
RackV2-09 | True | NONE | NONE | NONE
S1Obj | True | NONE | NONE | NONE
IntroAstra243 | True | NONE | NONE | NONE
AccessSpineMeasure248 | True | NONE | 841 | NONE
UIQual275 | True | NONE | NONE | NONE
MainInt277 | True | NONE | 659 | NONE
WorkbenchTier2Reg287 | True | NONE | 590,624,627,709 | NONE
UIQual376 | True | NONE | 446,461,475,477,481,483,487 | NONE
ResearchCopyQual391 | True | CCF-412-PREDATOR-FABLE-A-D073 | 372,419,446 | NONE
Objective1UX443 | True | NONE | 328,330 | NONE
MultiP0Proof674 | True | CCF-674-MULTI-P0-ANCESTRY | NONE | NONE
MultiP0P1Integ687 | True | CCF-687-P0P1-INTEGRATION | NONE | NONE
Sen1SaveA0NonUnity690 | False | NONE | 128,131 | NONE
MultiP1ChildB697 | True | CCF-697-MULTI-P1-CHILD-B | NONE | NONE
Sen1SaveA0ZX20NonUnity707 | True | CCF-690-SEN1-SAVE-A0 | NONE | NONE
MultiP1ModeRepair710 | True | CCF-710-MULTI-P1-R-CUSTODY | NONE | NONE
MultiAnnualF0Child725 | True | CCF-725-MULTI-F0-ANNUAL | NONE | NONE
MultiH0ACore | True | CCF-755-H0A-CORE-LANE | NONE | NONE
MultiH0AViewer | True | CCF-755-H0A-VIEWER-LANE | NONE | NONE
STATE_ENTRIES ['guard', 'history.jsonl']
LOCK_EXISTS False
HISTORY_RECORDS 415 FIRST 2026-10-16T01:14:14.819683+00:00 LAST 2026-11-15T22:44:25.912374+00:00
HISTORY DISTINCT ACTORS
Anthropic Claude Sonnet 5.5 (Claude A) #427
Astra-243
CCF Unity Operator
Claude Code (Opus 5.5) via T3 Code #359
Claude Code (Sonnet 5.5) MFM-07
Claude Code (Sonnet 5.5) MFM-08 Closure 01
Claude Code (Sonnet 5.5) MFM-08 Tier-2
Claude Code (Sonnet 5.5) MFM-08 repair smoke
Claude Code (Sonnet 5.5) MFM-08 successor Tier-2
Claude Code (Sonnet 5.5) RACK-V2 combined integrator
Claude Code (Sonnet 5.5) via T3 Code #315
Claude Code (Sonnet 5.5) via T3 Code #351
Claude Code (Sonnet 5.5) via T3 Code #353
Claude Code (Sonnet 5.5) via T3 Code #356
Claude Code (Sonnet 5.5) via T3 Code #357
Claude Code (Sonnet 5.5) via T3 Code #358
Claude Code (Sonnet 5.5) via T3 Code #360
Claude Code / Sonnet 5.5 (U2 operator)
Claude Code Sonnet 5.5 S2 missing-prefab recovery operator
Claude Code session (Sonnet 5.5) independent reviewer
Claude Opus 5.5 (T3 Code; user-chosen substitute for Sol)
Claude Opus 5.5 scen1-490
Claude-24gate-sweep
Claude-MFM08A
Claude-MFM09-B
Claude-PARITY-08
Claude-PARITY-08-BASE
Claude-PARITY-08-RESIDUAL
Claude-S1C-T04
Claude-S1C-T08
Claude-S1D-164
Claude-S1D-GateRecon2
Claude-S1D-T04
Claude-S1D-T05
Claude-S1D-T08
Claude-Sonnet-5.5-CCF370
Claude-Sonnet-5.5-ui-nonui-integration
Claude-delta-sweep
Claude-integrator-rackfirst
Claude-material-diagnosis
Claude-model2-adoption (ccf-primary-2e)
Claude-rack-first-ux
Claude-rack-modal-fix
Claude-s1-combined-integration
Claude-s1-professional-map
Claude-s1-professional-map-integration
Claude-s1-ux-pedagogy
Claude-tier3-repin
ClaudeA-RACK-V2-11
ClaudeB-RACK-V2-09
ClaudeB-RACK-V2-12
ClaudeB-S1D-T06
Codex gpt-6.1-sol HIGH #417
Codex-Ops548-durability
Codex-Q10
Codex-RACK-V2-09
Codex-S1-sequential
Codex-S1C-T06
Codex-S1D-T04P
Codex-Sol-CCF369
Codex-Sol-CCF376
Codex-Sol-CCF377
Codex-Sol-CCF378
Codex-Sol-CCF379
Codex-Sol-CCF381
Codex-model2-continuation
Codex-model2-followup
Codex-s1-ui-runtime-recovery
Codex-verification-debt
DPC-248
DPC-250
DeepSeek orchestrator #203 (independent review)
DeepSeek-201
DeepSeek-PARITY-05
DeepSeek-PARITY-09
DeepSeek-PostMerge-T06
DeepSeek-RACK-V2-12
DeepSeek-S1D-T06
GPT-6.1 Sol High CCF460
GPT-6.1 Sol Medium
Mimo 2.6 Pro worker for DeepSeek orchestrator #203
Mimo-MFM-11
OPENCODE-startup-requalification-r2
OpenAI Codex gpt-6.1-sol HIGH #427
OpenAI GPT-6.1 Sol task443
OpenAI GPT-6.1 Sol task450
OpenCode-deepseek-v4.1-flash-MFM02
S232-Sonnet55
SOL-CODEX-guided-qualification-r1
SOL-CODEX-startup-reconciliation-r1
SOL-CODEX-ui-visual-r2
SOL/Codex
SOL/Codex V5 continuation-2 test worker
Sol-PARITY-06
Sol-S1C-T03
Sol-S1D-T03
Sol-onboarding-fixup
Sol381
Sol391
T3 Claude Opus 5.5 (SCENARIO-ONE-APPROACH-ANCHOR-DIAGNOSIS-V1)
T3 Claude Sonnet 5.5 requalification (packet routed SOL/CODEX)
T3 Code / Claude A claude-sonnet-5-5 (RACK-V2-05 #43)
T3 Code / Claude B Pro claude-sonnet-5-5 (RACK-V2-07 #47)
T3 Code / Claude B claude-sonnet-5-5 xhigh (RACK-V2-05 #43)
T3 Code / Claude Sonnet 5.5 (S1-C-IMPL-02 implementer)
T3 Code / Claude Sonnet 5.5 RACK-V2 combined repair
T3 Code / Claude Sonnet 5.5 S1-C-IMPL-02
T3 Code / Codex (U2 operator)
T3 Code / Codex FIX-1 implementer
T3 Code / Codex RACK-V2 combined integrator
T3 Code / Codex SOL V5 Unity Operator
T3 Code / Codex gpt-6.1-sol (S1-C-IMPL-02 continuation)
T3 Code / Codex gpt-6.1-terra xhigh
T3 Code / Codex gpt-6.1-terra xhigh (RACK-V2-05 #43)
T3 Code / Codex primary worker
T3 Code / OpenCode gemini-4-argon-contributor independent reviewer
T3 Codex SOL
T3 Codex gpt-6.1-sol medium #368
T3 DeepSeek MFM-07-125
T3 SOL Codex S2 qualification operator
T3 SOL d9bba504
T3-Claude-RACK-V2-08
T3-Code-Claude-Sonnet-5.5-MFM02
T3-Code-Claude-Sonnet-5.5-Parity04Unity
T3-Code-Codex-gpt-6.1-sol-Parity03
T3-Code-Codex-gpt-6.1-sol-RackV2-04A
T3-Code-deepseek-v4.1-flash
T3-Code-deepseek-v4.1-flash-RACK-V2-04V2
T3-Codex-RACK-V2-08
T3-Codex-Sol-NonUi-V3
T3-Codex-Sol-NonUi-V4
T3-DeepSeek-Guided-Reconciliation
T3-DeepSeek-Guided-Retry
T3-DeepSeek-Menu-Fixture
T3-DeepSeek-NonUi-P0714
T3-DeepSeek-RACK-V2-08-LAYOUT-REPAIR-01
T3-DeepSeek-UxShots-Fixture
T3-FinalTier3
T3-MFM0405
T3-MFM06
T3-Parity07
T3-S1C-T05
T3-Sol-MFM06-113
T3-Terra-onboarding-polish
T3-UI-V2
UI-V2-LATER-STAGES-Go
USER personal playtest #412
claude-sonnet-5-5 (failover resume)
claude-sonnet-5-5 (licence resume)
codex-gpt-6.1-sol-MFM-12
codex-gpt-6.1-sol-MFM-12-FIT
codex-rack-v2-02b-r
mimo-287-claude-sonnet-5-5
onboarding-0c55a504
opencode-deepseek-hud-colour
opencode-deepseek-v4.1-flash
opencode-deepseek-v4.1-flash-MFM-12
opencode-go/deepseek-v4.1-flash
parity05-codex
rack-v2-03b-save-foundation
rack-v2-03c-runtime
s1-ui-visual-system-v1-implementation-r1
LAST_HISTORY_EXACT {"actor": "Codex-Ops548-durability", "event": "release", "inspection": null, "manager_approval": null, "previous": {"branch": "qualify/scenario-one-tagged-text-379-20261111", "eta": "2026-11-15T22:54:24.460684+00:00", "head": "2aaacad68ed186dbe14531f31ce5bb3a87811441", "lane": "/media/jer/ZX20/CCF-S1-UI-Presentation-V1-Qual", "lock_id": "8d77dccc25ca468baf1a69af92fa45f5", "notes": "User-authorised metadata-only acquire/release proof; no Editor, lane/source/save/prefs writes", "owner": "Codex-Ops548-durability", "pid": 773451, "process_identity": {"boot_id": "e496e620-b788-49cf-91d7-ab6a3679840d", "start_ticks": "13915970"}, "start_timestamp": "2026-11-15T22:44:25.592554+00:00", "task_id": "CCF-548-IDENTITY-DURABILITY"}, "reason": "Completed metadata-only durability verification; no Editor launched", "timestamp": "2026-11-15T22:44:25.912374+00:00"}
BYTECODE [('/media/jer/ZX20/CCF-Ops/scripts/__pycache__/_ccf.cpython-312.pyc', 1794355206990000000, '900d02566195f6a0e1487edd34dd11f9fc6347a6869b87f4a19f106934545da1')]
```

### E11

lane-issues; shell exit 0.

Command:

```bash
date -u +%Y-%m-%dT%H:%M:%SZ
python3 -B - <<'PY'
import concurrent.futures,subprocess,json
ids=[74,91,224,243,248,275,277,287,376,391,443,652,777,778]
def check(n):
 d=json.loads(subprocess.check_output(['gh','api',f'repos/jermur788/CCF/issues/{n}']))
 return f"#{n} {d['state']} updated={d['updated_at']} title={d['title']}"
with concurrent.futures.ThreadPoolExecutor(max_workers=4) as pool:
 for out in pool.map(check,ids):print(out)
PY
```

Exact output:

```text
2026-11-19T01:10:40Z
#74 open updated=2026-10-30T16:00:41Z title=[WAITING — NON-BLOCKING UNITY FOLLOW-UP / FRESH PREFLIGHT REQUIRED] PARITY-03 Q10 — controller continuity measurement
#91 open updated=2026-11-11T07:31:34Z title=[RUNNING] Scenario One player-test readiness — two-lane continuation
#224 closed updated=2026-11-02T19:58:58Z title=[CLOSED][APPROVED] Scenario One Objectives-window overflow — bounded UI repair before MFM-12
#243 closed updated=2026-11-03T21:12:14Z title=[APPROVED / STAGED / LEASE RELEASED — CLOSED] Scenario One introduction refresh
#248 closed updated=2026-11-03T23:11:37Z title=[APPROVED / CLOSED — RESEARCH] Corrected access-spine geometry/MFM measurement
#275 closed updated=2026-11-04T08:36:55Z title=[HANDED-OFF / CLOSED] Focused UI Unity qualification — test repair outstanding
#277 closed updated=2026-11-04T20:08:07Z title=[APPROVED / CLOSED — TEST ONLY] Repair committed rack-editing handle-rotation expectations
#287 open updated=2026-11-09T22:42:51Z title=[NEEDS CHANGES — NOT UNITY QUALIFIED / DIRTY LANE] Workbench Tier-2
#376 closed updated=2026-11-10T22:39:57Z title=[HANDED-OFF / NEEDS CHANGES] UI qualification — initial inspection clips at both sizes
#391 closed updated=2026-11-12T03:00:52Z title=[HANDED-OFF] C-01 research-learning V1 truthful copy — a314d308 reviewed and Unity-checked, not integrated
#443 open updated=2026-11-15T15:49:46Z title=[RUNNING CLAIM / G3 APPROVED — G4 NOT YET VERIFIED] Objective 1 rack-access UX repair
#652 open updated=2026-11-18T18:51:18Z title=[SOURCE APPROVED — DEPLOYMENT HELD / FINAL EXFAT V6 REVIEW OPEN] Sen-1 #652 Ops
#777 closed updated=2026-11-18T21:23:05Z title=[HANDED-OFF / PARTIAL — SOURCE AND UNITY GATED] Sen-1 Compact30 vertical-slice orchestration
#778 open updated=2026-11-18T22:06:10Z title=[READY — TEST-ONLY / CONDITIONAL PHYSICAL PREFLIGHT] Sen-1 Compact30 signed-invariant offline verification scaffold
```

### E12

retained-claims; shell exit 0.

Command:

```bash
date -u +%Y-%m-%dT%H:%M:%SZ
python3 -B - <<'PY'
import concurrent.futures,subprocess,json,hashlib
ids=[74,91,287,443]
def check(n):
 d=json.loads(subprocess.check_output(['gh','api','--paginate',f'repos/jermur788/CCF/issues/{n}/comments']))
 text=json.dumps(d,sort_keys=True,separators=(',',':'))
 tail=d[-1]
 return f"ISSUE #{n} comments={len(d)} canonical_json_sha256={hashlib.sha256(text.encode()).hexdigest()}\nLAST {tail['html_url']} {tail['created_at']}\n{tail['body']}"
with concurrent.futures.ThreadPoolExecutor(max_workers=4) as pool:
 for out in pool.map(check,ids):print(out)
PY
```

Exact output:

```text
2026-11-19T01:10:57Z
ISSUE #74 comments=10 canonical_json_sha256=207a0fae89aee9b69ae854615202c89b782848becda8652b74e9cac9992e8737
LAST https://github.com/jermur788/CCF/issues/74#issuecomment-6527236578 2026-10-30T16:00:41Z
[BROWSER STATUS RECONCILIATION — OBSOLETE RACK WAITING WORDING SUPERSEDED]

Rack V2 feature-scope qualification is complete; the old instruction in this claim body to wait specifically behind RACK-V2-08 #49 is historical and no longer the live dependency.

Current status remains **NON-BLOCKING**. Q10 is evidence-strengthening only and does not gate accepted PARITY-03 or the current Scenario One critical path. Any future Q10 work must refresh live `Docs/Project/coordination/active-tasks.md`, this claim and current integration truth before starting. Static/offline verification may use a dedicated verification lane only if there is no path-owner conflict; the controller-level Unity measurement remains machine-serialized and must not preempt #151/#156 or later critical-path Unity gates without Manager rerouting.

No production ownership, no rebaseline, no Windows build. This comment changes queue wording only; it does not claim Q10 execution or Unity availability.
ISSUE #91 comments=67 canonical_json_sha256=5dc9a4973dbc90f6e7f0e74875ed487a61a2fd92e4bac3a7e4fb358a8bc7afa6
LAST https://github.com/jermur788/CCF/issues/91#issuecomment-6789912834 2026-11-11T07:31:34Z
**#381 returns limited user-facing release, NOT full programme G4:** Manager #200 has [conditionally approved a protected personal Unity Editor Scenario One playtest](https://github.com/jermur788/CCF/blob/docs/manager-381-conditional-personal-playtest-20261111-v1/Docs/Handoffs/ScenarioOne-Playtest-Finish-20261111/MANAGER-381-CONDITIONAL-GO-PERSONAL-EDITOR-PLAYTEST-20261111.md) after inspecting source diff and exact-candidate author qualification (natural full loop, MFM, Reference/Legacy scope, rendered current UI). User #382 is CONDITIONAL GO / NOT STARTED, requires exclusive user Editor lease and actual isolated save/Prefs. Known source/offline verifier RED, incomplete idle/stale/post-completion/native pointer and reviewer debt remain; not accepted/external demo/Windows. #361 protected older U3 test unaccepted. Dashboard downstream sync unverified.
ISSUE #287 comments=21 canonical_json_sha256=30ba61596f4d9e1a01c99ce0f55c09f77829215e773571eea009c39ac07c3e4a
LAST https://github.com/jermur788/CCF/issues/287#issuecomment-6753446719 2026-11-09T22:42:51Z
**Manager #200 after #360:** original Workbench #287 still **NOT FULLY QUALIFIED**; in-Editor 107/107 previous combined candidate PASS, original strict FINAL runner exit1 FAIL remains on known U3 settings rewrite, not waived or relabelled. #360 fixed one onboarding UI focus issue on successor exact source and passed two other narrow Unity tests, did NOT repeat Workbench; #316 independent evidence review is now READY under [binding https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Overnight-Playtest-Readiness-20261109/MANAGER-360-TO-316-INDEPENDENT-INTERNAL-EDITOR-PLAYTEST-READINESS-REVIEW.md](https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Overnight-Playtest-Readiness-20261109/MANAGER-360-TO-316-INDEPENDENT-INTERNAL-EDITOR-PLAYTEST-READINESS-REVIEW.md) to advise whether this environmental-only strict fail can be an explicitly admitted limited internal Editor playtest residual. Pre-external-demo strict qualification remains separate.
ISSUE #443 comments=17 canonical_json_sha256=cd5034c2bf33cb604e6952412feae0beb4966c5104c59da3219741b47745f1af
LAST https://github.com/jermur788/CCF/issues/443#issuecomment-6897133771 2026-11-15T15:49:46Z
**Manager #424 integration ruling — HOLD pending historical-save compatibility (15 November 2026).** The earlier #450 independent UX delta APPROVED result remains recognised, but its candidate is **not merged to main or accepted as a historical save baseline**. T-00 found an unguarded `ForestSaveController.cs` absent-buildable=>retired branch in the cumulative #443 lineage, potentially changing genuine old worlds, plus starting clearing/obstacle effects; no genuine fixture regression was demonstrated, so this is a compatibility *risk*, not a claimed observed failure. Also preserve Reference preview pose repair in eventual all-or-nothing integration, review annual-review/control lifecycle and global ReduceMotion pref. The live board now reserves the 15 overlapping source paths for sequential save T-01..T-04 under #544/#549; **no new Objective 1 source writes or merge during that exclusive reservation.** Future Manager integration packet must reconcile on a reviewed exact candidate and prove protected old-save/Reference and new save routing with targeted tests, keeping UX changes where safe. Full disposition https://github.com/jermur788/CCF/blob/docs/scen1-manager-424-save-t01-ownership-ruling-20261115-v1/Docs/Handoffs/ScenarioOne-Save-Orchestration-20261115/scen1-MANAGER-T00-ACCEPTANCE-T01-BASE-OWNERSHIP-OPS-RULING-20261115.md. No source/Unity edits made here.
```

### E13

protected-and-paths; shell exit 0.

Command:

```bash
date -u +%Y-%m-%dT%H:%M:%SZ
python3 -B - <<'PY'
from pathlib import Path
import hashlib,json,subprocess,os
paths=['AGENTS.md','Docs/Operations/StorageArchitecture.md','Docs/Project/coordination/active-tasks.md','Docs/Operations/PostStage1CCFWorktreeEvidenceDisposition-20261118.md','Docs/Operations/Stage1StorageRecoveryExecution-20261118.md','Docs/Operations/StaleWorktreeDiskPressureHandoff-20261118.md','Docs/Project/coordination/archive/active-tasks-2026-10.md']
root=Path('/media/jer/Files/CCF-Primary')
for n in paths:print(hashlib.sha256((root/n).read_bytes()).hexdigest(),n)
print('PROCESS_ENV',{n:os.getenv(n) for n in ['TMPDIR','TMP','TEMP','XDG_CACHE_HOME','XDG_STATE_HOME','XDG_DATA_HOME','PYTHONDONTWRITEBYTECODE','CODEX_THREAD_ID','T3_THREAD_ID']})
print('OWN_OUTPUT_PARENT',oct(Path.cwd().stat().st_mode & 0o777),os.getuid())
PY
rg -n 'dont_write|du|active=|owner|return 0|guarded|mkdir|open\(|operation.*status|eta|event\(' /media/jer/ZX20/CCF-Ops/scripts/lane-status /media/jer/ZX20/CCF-Ops/scripts/unity-lock
sha256sum /media/jer/Files/CCF-Primary/AGENTS.md /media/jer/Files/CCF-Primary/Docs/Project/AgentWorkflow.md /media/jer/Files/CCF-Primary/Docs/Project/decision-log.md /media/jer/Files/CCF-Primary/Docs/Operations/StorageArchitecture.md
```

Exact output:

```text
2026-11-19T01:11:20Z
1bf4286b3485de018759a5b9b8e19d74beb6385844b56315f22f71b7819897fd AGENTS.md
6bebe49511f310c0f97b000f2f5a6c8db6890bb77fd312b9ec47e47c900a5a22 Docs/Operations/StorageArchitecture.md
ec842f269258d64aec3fee6b23f8c56ef1b8fa63822caf2e4472d39611afbc8f Docs/Project/coordination/active-tasks.md
fc8cff37496aaeca63f66dccb490f5e250c8812909460bb70b08a860145d81be Docs/Operations/PostStage1CCFWorktreeEvidenceDisposition-20261118.md
188fd66dad072912f1266a9f7ac927e5745a8b3c204e69b5346df9af576a9651 Docs/Operations/Stage1StorageRecoveryExecution-20261118.md
c2d9f78fe82f4fd74d4b5552b6c1b1df13145fa96c5f293922ad1c8873e39691 Docs/Operations/StaleWorktreeDiskPressureHandoff-20261118.md
4b9faa3eae8d8a389ddcc8dc0d8a754f545a193a6dd9e6805626778a75510b22 Docs/Project/coordination/archive/active-tasks-2026-10.md
PROCESS_ENV {'TMPDIR': None, 'TMP': None, 'TEMP': None, 'XDG_CACHE_HOME': None, 'XDG_STATE_HOME': None, 'XDG_DATA_HOME': None, 'PYTHONDONTWRITEBYTECODE': None, 'CODEX_THREAD_ID': '01a1f156-cdbe-7d11-bc51-125cff22b786', 'T3_THREAD_ID': None}
OWN_OUTPUT_PARENT 0o755 1000
/media/jer/ZX20/CCF-Ops/scripts/unity-lock:2:"""One persistent machine-wide ownership record, not a process supervisor."""
/media/jer/ZX20/CCF-Ops/scripts/unity-lock:4:sys.dont_write_bytecode = True
/media/jer/ZX20/CCF-Ops/scripts/unity-lock:38:def guarded(state):
/media/jer/ZX20/CCF-Ops/scripts/unity-lock:39:    state.mkdir(parents=True,exist_ok=True)
/media/jer/ZX20/CCF-Ops/scripts/unity-lock:40:    # Permanent guard inode; never unlink it. Metadata operations serialize here.
/media/jer/ZX20/CCF-Ops/scripts/unity-lock:41:    with (state/'guard').open('a+') as guard:
/media/jer/ZX20/CCF-Ops/scripts/unity-lock:52:        required={'lock_id','owner','task_id','lane','branch','head','pid','process_identity','start_timestamp','eta','notes'}
/media/jer/ZX20/CCF-Ops/scripts/unity-lock:53:        if not isinstance(data,dict) or not required.issubset(data): raise ValueError('missing metadata fields')
/media/jer/ZX20/CCF-Ops/scripts/unity-lock:54:        if not all(isinstance(data[k],str) and data[k] for k in required-{'pid','process_identity'}): raise ValueError('invalid string metadata')
/media/jer/ZX20/CCF-Ops/scripts/unity-lock:59:        if datetime.fromisoformat(data['eta']).tzinfo is None: raise ValueError('ETA needs timezone')
/media/jer/ZX20/CCF-Ops/scripts/unity-lock:65:    if not data: return {'status':'free','metadata':None,'unity_editor_pids':editors()}
/media/jer/ZX20/CCF-Ops/scripts/unity-lock:71:    return {'status':state,'metadata':data,'inspection':reason,
/media/jer/ZX20/CCF-Ops/scripts/unity-lock:72:            'eta_overdue':datetime.fromisoformat(data['eta']) < datetime.now(timezone.utc),
/media/jer/ZX20/CCF-Ops/scripts/unity-lock:75:def event(state,kind,actor,reason,previous,inspection=None,manager_approval=None):
/media/jer/ZX20/CCF-Ops/scripts/unity-lock:76:    with (state/'history.jsonl').open('a') as f:
/media/jer/ZX20/CCF-Ops/scripts/unity-lock:77:        f.write(json.dumps({'timestamp':now(),'event':kind,'actor':actor,'reason':reason,'previous':previous,'inspection':inspection,'manager_approval':manager_approval})+'\n')
/media/jer/ZX20/CCF-Ops/scripts/unity-lock:81:    if machine: print(json.dumps(value,indent=2))
/media/jer/ZX20/CCF-Ops/scripts/unity-lock:85:            if k!='status': print(f'  {k}: {json.dumps(v,sort_keys=True) if isinstance(v,(dict,list)) else v}')
/media/jer/ZX20/CCF-Ops/scripts/unity-lock:89:    p.add_argument('operation',choices=['status','acquire','release'])
/media/jer/ZX20/CCF-Ops/scripts/unity-lock:94:    for key in ['owner','task-id','lane','branch','head','eta','notes','lock-id','reason']: p.add_argument('--'+key)
/media/jer/ZX20/CCF-Ops/scripts/unity-lock:105:    if a.operation=='status' and not state.exists():
/media/jer/ZX20/CCF-Ops/scripts/unity-lock:106:        emit(inspect(None,None),a.json); return 0
/media/jer/ZX20/CCF-Ops/scripts/unity-lock:107:    with guarded(state):
/media/jer/ZX20/CCF-Ops/scripts/unity-lock:109:        if a.operation=='status': emit(inspect(data,malformed),a.json); return 0
/media/jer/ZX20/CCF-Ops/scripts/unity-lock:111:            for k in ['owner','task_id','lane','branch','head','eta','notes']:
/media/jer/ZX20/CCF-Ops/scripts/unity-lock:112:                if not (getattr(a,k) or '').strip(): p.error('--'+k.replace('_','-')+' is required')
/media/jer/ZX20/CCF-Ops/scripts/unity-lock:115:            eta=datetime.fromisoformat(a.eta)
/media/jer/ZX20/CCF-Ops/scripts/unity-lock:116:            if eta.tzinfo is None or eta<=datetime.now(timezone.utc): p.error('--eta must be a future ISO timestamp with timezone')
/media/jer/ZX20/CCF-Ops/scripts/unity-lock:131:            if branch_conflicts(checkouts,actual_branch,lane): raise ValueError('branch has another writable checkout; dual-store/lane conflict')
/media/jer/ZX20/CCF-Ops/scripts/unity-lock:136:            data={'lock_id':uuid.uuid4().hex,'owner':a.owner,'task_id':a.task_id,'lane':str(lane),
/media/jer/ZX20/CCF-Ops/scripts/unity-lock:138:                  'start_timestamp':now(),'eta':a.eta,'notes':a.notes}
/media/jer/ZX20/CCF-Ops/scripts/unity-lock:140:            tmp=state/('metadata-'+data['lock_id']+'.tmp')
/media/jer/ZX20/CCF-Ops/scripts/unity-lock:142:                with tmp.open('x') as f:
/media/jer/ZX20/CCF-Ops/scripts/unity-lock:143:                    json.dump(data,f,indent=2); f.write('\n'); f.flush(); os.fsync(f.fileno())
/media/jer/ZX20/CCF-Ops/scripts/unity-lock:147:            emit(inspect(data,None),a.json); return 0
/media/jer/ZX20/CCF-Ops/scripts/unity-lock:150:            if not (a.owner or '').strip() or not (a.reason or '').strip(): p.error('force release requires --owner (actor) and --reason')
/media/jer/ZX20/CCF-Ops/scripts/unity-lock:153:            event(state,'force-release',a.owner,a.reason,data or malformed,inspect(data,malformed),a.manager_approval)
/media/jer/ZX20/CCF-Ops/scripts/unity-lock:155:            if malformed: raise ValueError('malformed record requires explicit --force --owner --reason')
/media/jer/ZX20/CCF-Ops/scripts/unity-lock:156:            if (a.owner,a.task_id,a.lock_id)!=(data['owner'],data['task_id'],data['lock_id']): raise ValueError('release requires matching --owner --task-id --lock-id')
/media/jer/ZX20/CCF-Ops/scripts/unity-lock:157:            event(state,'release',a.owner,a.reason or 'owner release',data)
/media/jer/ZX20/CCF-Ops/scripts/unity-lock:160:        return 0
/media/jer/ZX20/CCF-Ops/scripts/lane-status:4:sys.dont_write_bytecode = True
/media/jer/ZX20/CCF-Ops/scripts/lane-status:21:        print(json.dumps(health,indent=2) if a.json else f"{'READY' if health['ready'] else 'BLOCK'} / {health['status']} | device={health['backing_device']} rw={health['writable']} | free={health['free_bytes']} bytes ({health['free_gib']:.2f} GiB) threshold={health['minimum_free_bytes']} bytes ({health['minimum_free_gib']:.2f} GiB)")
/media/jer/ZX20/CCF-Ops/scripts/lane-status:22:        return 0 if health['ready'] else 1
/media/jer/ZX20/CCF-Ops/scripts/lane-status:23:    tasks=board_read(board);duplicates=board_conflicts(tasks,cfg);checkouts=store_checkouts(cfg)
/media/jer/ZX20/CCF-Ops/scripts/lane-status:33:        row=dict(lane=item['lane'],path=str(path),resolved_path=str(resolved),backing_device=mount['source'],zx20=zx20,unity_capable=item['unity'],exists=exists,registered_worktree=registered,branch=None,HEAD=None,dirty=None,ahead_behind=None,unity_library=(path/'Library').is_dir(),approx_disk_usage=None,owner=[],conflict_warning=list(duplicates),storage=health,dual_store_conflicts=[])
/media/jer/ZX20/CCF-Ops/scripts/lane-status:34:        active=[t for t in tasks if t['Lane'] in {item['lane'],'CCF-'+item['lane'],str(path)}]
/media/jer/ZX20/CCF-Ops/scripts/lane-status:35:        row['owner']=[t['Owner']+' ('+t['Task ID']+')' for t in active]
/media/jer/ZX20/CCF-Ops/scripts/lane-status:37:        off_durable=(not zx20) and (not item['unity']) and resolved.is_relative_to(DURABLE_CCF.resolve())
/media/jer/ZX20/CCF-Ops/scripts/lane-status:39:        elif not zx20 and not off_durable:row['conflict_warning'].append('ZX20: NO — STOP; off-storage lane is not in the durable Files CCF tree')
/media/jer/ZX20/CCF-Ops/scripts/lane-status:41:            row['approx_disk_usage']=command(['du','-sh',str(path)]).split('\t')[0]
/media/jer/ZX20/CCF-Ops/scripts/lane-status:51:                    if row['dirty']:row['conflict_warning'].append('dirty: inspect ownership before writes')
/media/jer/ZX20/CCF-Ops/scripts/lane-status:55:                    conflicts=branch_conflicts(checkouts,assignment,path);row['dual_store_conflicts']=conflicts
/media/jer/ZX20/CCF-Ops/scripts/lane-status:62:    if a.json:print(json.dumps(result,indent=2))
/media/jer/ZX20/CCF-Ops/scripts/lane-status:67:            print(f"  branch={r['branch']} HEAD={r['HEAD']} dirty={r['dirty']} ahead/behind={r['ahead_behind']} Library={r['unity_library']} disk~={r['approx_disk_usage']} owner={', '.join(r['owner']) or 'unassigned'}")
/media/jer/ZX20/CCF-Ops/scripts/lane-status:73:        return 0 if ready else 1
/media/jer/ZX20/CCF-Ops/scripts/lane-status:74:    return 0 if health['ready'] else 1
1bf4286b3485de018759a5b9b8e19d74beb6385844b56315f22f71b7819897fd  /media/jer/Files/CCF-Primary/AGENTS.md
6aa823401ff3b75883b482bbaf558182192586dc3a22a2078a61715d9109ed5f  /media/jer/Files/CCF-Primary/Docs/Project/AgentWorkflow.md
1d9662588961a579ea35f9dd083a670d2c8a67465f651f0b9cbda18885083dc3  /media/jer/Files/CCF-Primary/Docs/Project/decision-log.md
6bebe49511f310c0f97b000f2f5a6c8db6890bb77fd312b9ec47e47c900a5a22  /media/jer/Files/CCF-Primary/Docs/Operations/StorageArchitecture.md
```

### E14

prewrite-preservation; shell exit 0.

Command:

```bash
date -u +%Y-%m-%dT%H:%M:%SZ
python3 -B - <<'PY'
from pathlib import Path
import hashlib,json
expected={"AGENTS.md":"d1ce3ad775e16d80fea6779c91bfa66adbd2e019edfbbbb9280be563be60e08f","ActiveTasksTransition.md":"e2fe59102b8b408a313f48807323c5d1befb6e89407e8a50dfdf169cd8e7a68d","CLAUDE.md":"625065153e9b69634723b6a6f678fa2371350cb7f436393461a62a07a28e2335","G4BCorrections.md":"5fc54e196e1905a39a5f99bf83ca1a0b8033e2717b2e6a924a861dc54cfb9486","G4WorkflowFoundationReviewPacket.md":"0219dfb2666a8c444c6c289782e383d6656d44f1af34d0fd55606a725d61e62c","README.md":"249be391e172e9d80f7d3d3419af961850b84c40fb2f15ba7b1d0469649eb517","WorkflowFoundationHandoff.md":"a9077b50dc25523cbf8d0711b23a9ad392c2d27e38858e8e073ed8277125a018","WorkflowFoundationVerification.md":"05d1f8cc3df50180e226e0da16994bd0cc3c6b77d025acb7b18fa5889763b60d","board/active.md":"4fa750eba64a8a072feb305a7db2cce17ddedfa62dbd276305116c03ab853994","deployment.json":"8c66b290c247708846cb8b59f83626a10a3fa1d052397a3316dc5f23fc157b18","deployment.json.bak-20261115T225732Z":"22bb354343c699cc78903d4a5d1b1b13548e126a4a04351f54f25586905195ed","deployment.json.bak-20261115T230935Z":"bc27de755194bd4c8ff34d24f55583db52376915301bfb22fabbb6405d7ca0e5","deployment.json.bak-20261115T231825Z":"2034c77fac082ec8107deef073605ce9314d594e79568e28f32aa8584dc4f5bc","environment.sh":"3775cba7ed446c61ae6fa2977da5cf0984ff7d888213ad9fa33396b1aa11c954","lanes.json":"c1d147196e1b8d57008e44545db3ba7f3fecea0ae12094dbf3e05f1c782b7d9e","ledger/2026-10-15-CCF-CANONICAL-WORKFLOW.md":"4237f949dd16a8d77060a1673e8e4eadab6c3f6e4d34ad408da4badd0583e553","ledger/2026-10-15-CCF-DUMMY-T1.md":"168584c1900f5639eccfe637a43d7f3b32f9dab286db85864d9b903d1c6b97de","ledger/2026-10-15-T2-S1-CANDIDATE-V2-PREFLIGHT.md":"2f221c80123261d71999f772937a668ea63221bea8211bf7a1a3d2ef41143bb5","ledger/2026-11-12-CCF-412-D073-FR10-ONE-GO.md":"9420b762200fa8599746f5e3901c4924275ef089958c178261f151c3053e575b","ledger/README.md":"23bf09b303aec430d8e6b8f1a76a0d7d34a055e04fd364423df09ae3e12048ba","pilot/README.md":"a0813687f28334369cea95796131edc999cf6a24cc8cc65ef4fc93cd279ff6b7","pilot/manager-packet.md":"7c65c61adec2c4ffa30b95529da5edee9f930241e0c84c66c67aa5bd6b2dfb29","pilot/t2-pilot-packet.md":"93aafd766e02e31ceee103bc8615d866995f5c2f8a94c2ba7ae53ebe2df1bd35","pilot/unity-gate-example.md":"4b1de83798691c697141f6a8e2b21561a27a3a741ef179f2932d2e38c37703a3","roles/execution-manager.md":"0fb67b1283052008a42fe40dbeef6dea5f6710ca5b71c7272d99bf581b3843f0","roles/reviewer.md":"58bafc0dee42158816687a71becb73f8a2cf4d1f9dac192bfbe4fe3a56207d6a","roles/unity-operator.md":"120f1deb006aa56a9d4b9643917f855548b5da80432a7166442ecc6163eee815","scripts/__pycache__/_ccf.cpython-312.pyc":"900d02566195f6a0e1487edd34dd11f9fc6347a6869b87f4a19f106934545da1","scripts/_ccf.py":"ab4343efc7d1b82c5a4aca747d0e09c614779cfd9aa406a0dd681d204f5de001","scripts/lane-status":"e7102a83ad942cc7365cb22db9a8f79823744ea98bc2cf8a6738a11f1897173f","scripts/packet-lint":"a776ec42a13224936a5c7c6a68ec2ed7ddc4e2cf21a96e287e6843a31e243e43","scripts/unity-lock":"54cf0a95561d0a8c350d89adbee83f393990b728d6127b94f69c587260f048e5","state/board-history.jsonl":"2875466cf1d309cb2aa65ff34699d7fa3bb2eb6595211c525d42d6c1a4ceba19","state/unity/guard":"e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855","state/unity/history.jsonl":"dfb506443e1f41b6d5f2be23e2c2b38d0ee76fe78d4d4ba2446fbc962422fffa","tests/fixtures/canonical-style.md":"e063274c82b1c38a8fc714ccc1ef3b19fb21d04494dca18a56a17760c60eb77a","tests/fixtures/conflicting-unity-ownership.md":"ca6a89ad0b4422575aab3234d5293672ccf756e92f952782d4474f8fcd1fee24","tests/fixtures/handoff-placeholder.md":"8f82d136a6ced39892a03ea03902d93c94fb270aedd1a80abec09d3d5b84edb9","tests/fixtures/missing-base.md":"e7e54f12e455304b501ed5d1f6b6c5a789b20e5cf3413aaf60d2304cffcd44d3","tests/fixtures/missing-role-file.md":"35c9831ff0d67a02151c99c03fa5ee01e7372430be49b3432cb6f0bf8c536856","tests/fixtures/missing-stop-condition.md":"719066818259794ffcab48073682af45e968cbae2cca2c341ccca568deb92171","tests/fixtures/missing-unity-required.md":"9e1110ce0a78241a64508de7269d3c92e9e13ea2a168521263eaba151346c7bb","tests/fixtures/missing-verification-tier.md":"16e0e3ce718d132b17eb24812a60ed2efd97fc492dd3562acec5a362c0f139e2","tests/fixtures/nonunity-lane-head.md":"b90081a1573de40a1deb64ed4e7bdf3907ec258ba9f1142313bb02ebb21c6113","tests/fixtures/partial.md":"7d6bb96b2602b70db57084099e8dcb3870f8328d6b41597619de9b8266ce6be9","tests/fixtures/stop-placeholder.md":"9c7c15857dab00f42ec9644136964d650916498b1053f6d9867c3958f2b23dfe","tests/fixtures/unity-nonunity-lane.md":"0bb1fa102a8f9c11cc025f0b84b36953d31cf7e2af79a875490b1fa9d066cb21","tests/fixtures/valid-unity.md":"281f9c52f9d168c5b75cd6fecfa7ac96cd0e044e7a44dd3508e4f273f416c00b","tests/fixtures/valid.md":"7c65c61adec2c4ffa30b95529da5edee9f930241e0c84c66c67aa5bd6b2dfb29","tests/self-test.py":"2a12a19d848ae789a1404c7ebd5285457e061b0a7142b91de95ed0b79cdeeaf3"}
p=Path('/media/jer/ZX20/CCF-Ops')
actual={str(f.relative_to(p)):hashlib.sha256(f.read_bytes()).hexdigest() for f in p.rglob('*') if f.is_file()}
print('DEPLOYED_FILES',len(actual))
print('CHANGED_OR_ADDED_OR_REMOVED',[(f,expected.get(f),actual.get(f)) for f in sorted(expected.keys()|actual.keys()) if expected.get(f)!=actual.get(f)])
print('DEPLOYED_MANIFEST_SHA256',hashlib.sha256(json.dumps(actual,sort_keys=True,separators=(',',':')).encode()).hexdigest())
print('LOCK_EXISTS',(p/'state/unity/lock.json').exists())
PY
git status --porcelain --branch
git log -1 --format='%H %s'
git -C /media/jer/Files/CCF-Primary ls-files CCF-Ops/lanes.json
df -B1 /media/jer/Files
```

Exact output:

```text
2026-11-19T01:11:44Z
DEPLOYED_FILES 50
CHANGED_OR_ADDED_OR_REMOVED []
DEPLOYED_MANIFEST_SHA256 47a610882a895c9ec59ecd3c1f4bd19c4e16eec353b157a63588fc9dc110bf12
LOCK_EXISTS False
## docs/sen-1-781-a1-physical-identity-20261118-v1...origin/main
a69b3e2e90829505c792d95f5a3eba3e7d8d8132 docs(coordination): add larger gated Sen-1 Compact30 vertical slice umbrella and test-only lane
CCF-Ops/lanes.json
Filesystem        1B-blocks         Used   Available Use% Mounted on
/dev/sdb4      498654666752 433378807808 39870332928  92% /media/jer/Files
```

### E15

scratch-scoped; shell exit 0.

Command:

```bash
date -u +%Y-%m-%dT%H:%M:%SZ
python3 -B - <<'PY'
from pathlib import Path
import json,hashlib
root=Path('/media/jer/ZX20/CCF-Temp')
refs={'D':Path('/media/jer/ZX20/CCF-Ops'),'R':Path('/media/jer/Files/CCF-Primary/CCF-Ops'),'A':Path('/media/jer/Files/CCF/Operations/CCF-OpsReadiness-652-20261116/CCF-Ops')}
def manifest(p):return {str(f.relative_to(p)):hashlib.sha256(f.read_bytes()).hexdigest() for f in p.rglob('*') if f.is_file()}
refs={k:manifest(p) for k,p in refs.items()}
candidates=[root/'ops652-final-source',root/'ops652-optiona-source']
candidates += sorted(root.glob('*/base/CCF-Ops'))+sorted(root.glob('*/staged/CCF-Ops'))+sorted(root.glob('*/replica/CCF-Ops'))
for p in candidates:
 m=manifest(p);print(str(p),'files='+str(len(m)),'manifest_sha256='+hashlib.sha256(json.dumps(m,sort_keys=True,separators=(',',':')).encode()).hexdigest())
 print('DIFF D',','.join(n for n in sorted(m.keys()|refs['D'].keys()) if m.get(n)!=refs['D'].get(n)))
 print('KEYS',json.dumps({k:m.get(k) for k in ['scripts/_ccf.py','scripts/unity-lock','README.md','deployment.json']},sort_keys=True))
PY
```

Exact output:

```text
2026-11-19T01:12:20Z
/media/jer/ZX20/CCF-Temp/ops652-final-source files=42 manifest_sha256=bd66de8a4bc879cb66548170907d79cae0835db25cc44bd7308bcd4433827968
DIFF D AGENTS.md,DeploymentProvenance.md,G4WorkflowFoundationReviewPacket.md,README.md,WorkflowFoundationHandoff.md,board/active.md,deployment.json,deployment.json.bak-20261115T225732Z,deployment.json.bak-20261115T230935Z,deployment.json.bak-20261115T231825Z,environment.sh,lanes.json,ledger/2026-11-12-CCF-412-D073-FR10-ONE-GO.md,scripts/__pycache__/_ccf.cpython-312.pyc,scripts/_ccf.py,scripts/verify-deployment,state/board-history.jsonl,state/unity/guard,state/unity/history.jsonl,tests/self-test.py
KEYS {"README.md": "d21387d54694feae0e89129684975f5f295b343b43d37f23224efbc8a73095e3", "deployment.json": "0ff48c916d78787a182ea8ffe0ab77aec8b2a02c1f7c0eab7734a08fbf552a3d", "scripts/_ccf.py": "1295def55a4821ccd00423f65ab58e64a3be916dc18b127ed99ae1099c3e7677", "scripts/unity-lock": "54cf0a95561d0a8c350d89adbee83f393990b728d6127b94f69c587260f048e5"}
/media/jer/ZX20/CCF-Temp/ops652-optiona-source files=42 manifest_sha256=72a3a5b70801ee72297bf079bec6179b4e3c2757652b9adf7a1564cdd1ff939a
DIFF D AGENTS.md,DeploymentProvenance.md,G4WorkflowFoundationReviewPacket.md,README.md,WorkflowFoundationHandoff.md,board/active.md,deployment.json,deployment.json.bak-20261115T225732Z,deployment.json.bak-20261115T230935Z,deployment.json.bak-20261115T231825Z,environment.sh,lanes.json,ledger/2026-11-12-CCF-412-D073-FR10-ONE-GO.md,scripts/__pycache__/_ccf.cpython-312.pyc,scripts/_ccf.py,scripts/unity-lock,scripts/verify-deployment,state/board-history.jsonl,state/unity/guard,state/unity/history.jsonl,tests/self-test.py
KEYS {"README.md": "5b7abb83b746fe0c0219a926ec9cf6fc682b7acc1ed7d9d5ad70e69d3628db07", "deployment.json": "be9fbcd04a2eadefde65b4aad791cbc30eb4633b57725732645672da9f113b34", "scripts/_ccf.py": "24a321466e6dae61ba950c4cbc4f918e12f7d6be8a6af628341c5aa8efe0b435", "scripts/unity-lock": "e465303493e1044fac9926c12a89b0f64f05b2aba786469de6f3645f78e41d3e"}
/media/jer/ZX20/CCF-Temp/Sen1-741-ExFATQual-20261118T142444Z/base/CCF-Ops files=50 manifest_sha256=268cfdf6a894b49274755fe83c4b5d96ee18b57b85cb82a63b1cd91bd0ba0c13
DIFF D board/active.md,lanes.json
KEYS {"README.md": "249be391e172e9d80f7d3d3419af961850b84c40fb2f15ba7b1d0469649eb517", "deployment.json": "8c66b290c247708846cb8b59f83626a10a3fa1d052397a3316dc5f23fc157b18", "scripts/_ccf.py": "ab4343efc7d1b82c5a4aca747d0e09c614779cfd9aa406a0dd681d204f5de001", "scripts/unity-lock": "54cf0a95561d0a8c350d89adbee83f393990b728d6127b94f69c587260f048e5"}
/media/jer/ZX20/CCF-Temp/Sen1-741-ExFATQual-20261118T142541Z/base/CCF-Ops files=50 manifest_sha256=268cfdf6a894b49274755fe83c4b5d96ee18b57b85cb82a63b1cd91bd0ba0c13
DIFF D board/active.md,lanes.json
KEYS {"README.md": "249be391e172e9d80f7d3d3419af961850b84c40fb2f15ba7b1d0469649eb517", "deployment.json": "8c66b290c247708846cb8b59f83626a10a3fa1d052397a3316dc5f23fc157b18", "scripts/_ccf.py": "ab4343efc7d1b82c5a4aca747d0e09c614779cfd9aa406a0dd681d204f5de001", "scripts/unity-lock": "54cf0a95561d0a8c350d89adbee83f393990b728d6127b94f69c587260f048e5"}
/media/jer/ZX20/CCF-Temp/Sen1-741-ExFATQual-20261118T142559Z/base/CCF-Ops files=50 manifest_sha256=268cfdf6a894b49274755fe83c4b5d96ee18b57b85cb82a63b1cd91bd0ba0c13
DIFF D board/active.md,lanes.json
KEYS {"README.md": "249be391e172e9d80f7d3d3419af961850b84c40fb2f15ba7b1d0469649eb517", "deployment.json": "8c66b290c247708846cb8b59f83626a10a3fa1d052397a3316dc5f23fc157b18", "scripts/_ccf.py": "ab4343efc7d1b82c5a4aca747d0e09c614779cfd9aa406a0dd681d204f5de001", "scripts/unity-lock": "54cf0a95561d0a8c350d89adbee83f393990b728d6127b94f69c587260f048e5"}
/media/jer/ZX20/CCF-Temp/Sen1-741-ExFATQualV2-20261118T143503Z/base/CCF-Ops files=50 manifest_sha256=268cfdf6a894b49274755fe83c4b5d96ee18b57b85cb82a63b1cd91bd0ba0c13
DIFF D board/active.md,lanes.json
KEYS {"README.md": "249be391e172e9d80f7d3d3419af961850b84c40fb2f15ba7b1d0469649eb517", "deployment.json": "8c66b290c247708846cb8b59f83626a10a3fa1d052397a3316dc5f23fc157b18", "scripts/_ccf.py": "ab4343efc7d1b82c5a4aca747d0e09c614779cfd9aa406a0dd681d204f5de001", "scripts/unity-lock": "54cf0a95561d0a8c350d89adbee83f393990b728d6127b94f69c587260f048e5"}
/media/jer/ZX20/CCF-Temp/Sen1-741-ExFATQualV3-20261118T144412Z/base/CCF-Ops files=50 manifest_sha256=268cfdf6a894b49274755fe83c4b5d96ee18b57b85cb82a63b1cd91bd0ba0c13
DIFF D board/active.md,lanes.json
KEYS {"README.md": "249be391e172e9d80f7d3d3419af961850b84c40fb2f15ba7b1d0469649eb517", "deployment.json": "8c66b290c247708846cb8b59f83626a10a3fa1d052397a3316dc5f23fc157b18", "scripts/_ccf.py": "ab4343efc7d1b82c5a4aca747d0e09c614779cfd9aa406a0dd681d204f5de001", "scripts/unity-lock": "54cf0a95561d0a8c350d89adbee83f393990b728d6127b94f69c587260f048e5"}
/media/jer/ZX20/CCF-Temp/Sen1-741-ExFATQualV4-20261118T145233Z/base/CCF-Ops files=50 manifest_sha256=268cfdf6a894b49274755fe83c4b5d96ee18b57b85cb82a63b1cd91bd0ba0c13
DIFF D board/active.md,lanes.json
KEYS {"README.md": "249be391e172e9d80f7d3d3419af961850b84c40fb2f15ba7b1d0469649eb517", "deployment.json": "8c66b290c247708846cb8b59f83626a10a3fa1d052397a3316dc5f23fc157b18", "scripts/_ccf.py": "ab4343efc7d1b82c5a4aca747d0e09c614779cfd9aa406a0dd681d204f5de001", "scripts/unity-lock": "54cf0a95561d0a8c350d89adbee83f393990b728d6127b94f69c587260f048e5"}
/media/jer/ZX20/CCF-Temp/Sen1-741-ExFATQualV4-20261118T145242Z/base/CCF-Ops files=50 manifest_sha256=268cfdf6a894b49274755fe83c4b5d96ee18b57b85cb82a63b1cd91bd0ba0c13
DIFF D board/active.md,lanes.json
KEYS {"README.md": "249be391e172e9d80f7d3d3419af961850b84c40fb2f15ba7b1d0469649eb517", "deployment.json": "8c66b290c247708846cb8b59f83626a10a3fa1d052397a3316dc5f23fc157b18", "scripts/_ccf.py": "ab4343efc7d1b82c5a4aca747d0e09c614779cfd9aa406a0dd681d204f5de001", "scripts/unity-lock": "54cf0a95561d0a8c350d89adbee83f393990b728d6127b94f69c587260f048e5"}
/media/jer/ZX20/CCF-Temp/Sen1-741-ExFATQualV4-20261118T145302Z/base/CCF-Ops files=50 manifest_sha256=268cfdf6a894b49274755fe83c4b5d96ee18b57b85cb82a63b1cd91bd0ba0c13
DIFF D board/active.md,lanes.json
KEYS {"README.md": "249be391e172e9d80f7d3d3419af961850b84c40fb2f15ba7b1d0469649eb517", "deployment.json": "8c66b290c247708846cb8b59f83626a10a3fa1d052397a3316dc5f23fc157b18", "scripts/_ccf.py": "ab4343efc7d1b82c5a4aca747d0e09c614779cfd9aa406a0dd681d204f5de001", "scripts/unity-lock": "54cf0a95561d0a8c350d89adbee83f393990b728d6127b94f69c587260f048e5"}
/media/jer/ZX20/CCF-Temp/Sen1-741-ExFATQualV5-20261118T145959Z/base/CCF-Ops files=50 manifest_sha256=268cfdf6a894b49274755fe83c4b5d96ee18b57b85cb82a63b1cd91bd0ba0c13
DIFF D board/active.md,lanes.json
KEYS {"README.md": "249be391e172e9d80f7d3d3419af961850b84c40fb2f15ba7b1d0469649eb517", "deployment.json": "8c66b290c247708846cb8b59f83626a10a3fa1d052397a3316dc5f23fc157b18", "scripts/_ccf.py": "ab4343efc7d1b82c5a4aca747d0e09c614779cfd9aa406a0dd681d204f5de001", "scripts/unity-lock": "54cf0a95561d0a8c350d89adbee83f393990b728d6127b94f69c587260f048e5"}
/media/jer/ZX20/CCF-Temp/Sen1-741-ExFATQualV6-20261118T150720Z/base/CCF-Ops files=50 manifest_sha256=268cfdf6a894b49274755fe83c4b5d96ee18b57b85cb82a63b1cd91bd0ba0c13
DIFF D board/active.md,lanes.json
KEYS {"README.md": "249be391e172e9d80f7d3d3419af961850b84c40fb2f15ba7b1d0469649eb517", "deployment.json": "8c66b290c247708846cb8b59f83626a10a3fa1d052397a3316dc5f23fc157b18", "scripts/_ccf.py": "ab4343efc7d1b82c5a4aca747d0e09c614779cfd9aa406a0dd681d204f5de001", "scripts/unity-lock": "54cf0a95561d0a8c350d89adbee83f393990b728d6127b94f69c587260f048e5"}
/media/jer/ZX20/CCF-Temp/Sen1-741-ExFATQualV6-20261118T150753Z/base/CCF-Ops files=50 manifest_sha256=268cfdf6a894b49274755fe83c4b5d96ee18b57b85cb82a63b1cd91bd0ba0c13
DIFF D board/active.md,lanes.json
KEYS {"README.md": "249be391e172e9d80f7d3d3419af961850b84c40fb2f15ba7b1d0469649eb517", "deployment.json": "8c66b290c247708846cb8b59f83626a10a3fa1d052397a3316dc5f23fc157b18", "scripts/_ccf.py": "ab4343efc7d1b82c5a4aca747d0e09c614779cfd9aa406a0dd681d204f5de001", "scripts/unity-lock": "54cf0a95561d0a8c350d89adbee83f393990b728d6127b94f69c587260f048e5"}
/media/jer/ZX20/CCF-Temp/Sen1-752-NativeV6-20261118T153253Z/base/CCF-Ops files=50 manifest_sha256=268cfdf6a894b49274755fe83c4b5d96ee18b57b85cb82a63b1cd91bd0ba0c13
DIFF D board/active.md,lanes.json
KEYS {"README.md": "249be391e172e9d80f7d3d3419af961850b84c40fb2f15ba7b1d0469649eb517", "deployment.json": "8c66b290c247708846cb8b59f83626a10a3fa1d052397a3316dc5f23fc157b18", "scripts/_ccf.py": "ab4343efc7d1b82c5a4aca747d0e09c614779cfd9aa406a0dd681d204f5de001", "scripts/unity-lock": "54cf0a95561d0a8c350d89adbee83f393990b728d6127b94f69c587260f048e5"}
/media/jer/ZX20/CCF-Temp/Sen1-752-NativeV6-20261118T153253Z/staged/CCF-Ops files=5 manifest_sha256=9e23d2fd8dfec36dc2cad8df6434797c160bea8cdd8bc43b3e0637a5d3bc2702
DIFF D AGENTS.md,ActiveTasksTransition.md,CLAUDE.md,G4BCorrections.md,G4WorkflowFoundationReviewPacket.md,README.md,WorkflowFoundationHandoff.md,WorkflowFoundationVerification.md,board/active.md,deployment.json,deployment.json.bak-20261115T225732Z,deployment.json.bak-20261115T230935Z,deployment.json.bak-20261115T231825Z,environment.sh,lanes.json,ledger/2026-10-15-CCF-CANONICAL-WORKFLOW.md,ledger/2026-10-15-CCF-DUMMY-T1.md,ledger/2026-10-15-T2-S1-CANDIDATE-V2-PREFLIGHT.md,ledger/2026-11-12-CCF-412-D073-FR10-ONE-GO.md,ledger/README.md,pilot/README.md,pilot/manager-packet.md,pilot/t2-pilot-packet.md,pilot/unity-gate-example.md,roles/execution-manager.md,roles/reviewer.md,roles/unity-operator.md,scripts/__pycache__/_ccf.cpython-312.pyc,scripts/_ccf.py,scripts/lane-status,scripts/packet-lint,scripts/unity-lock,state/board-history.jsonl,state/unity/guard,state/unity/history.jsonl,tests/fixtures/canonical-style.md,tests/fixtures/conflicting-unity-ownership.md,tests/fixtures/handoff-placeholder.md,tests/fixtures/missing-base.md,tests/fixtures/missing-role-file.md,tests/fixtures/missing-stop-condition.md,tests/fixtures/missing-unity-required.md,tests/fixtures/missing-verification-tier.md,tests/fixtures/nonunity-lane-head.md,tests/fixtures/partial.md,tests/fixtures/stop-placeholder.md,tests/fixtures/unity-nonunity-lane.md,tests/fixtures/valid-unity.md,tests/fixtures/valid.md,tests/self-test.py
KEYS {"README.md": "3baf029ef6da792c3129ce5e0e91b3097d69ee7c881f389ad0bb9a377de41123", "deployment.json": null, "scripts/_ccf.py": "24a321466e6dae61ba950c4cbc4f918e12f7d6be8a6af628341c5aa8efe0b435", "scripts/unity-lock": "8fa0c5ada367f109c9437b05bd37b42c7ce78472d6f630634df8745fcb220ac8"}
/media/jer/ZX20/CCF-Temp/Sen1-754-Author-20261118T170536Z/staged/CCF-Ops files=5 manifest_sha256=9e23d2fd8dfec36dc2cad8df6434797c160bea8cdd8bc43b3e0637a5d3bc2702
DIFF D AGENTS.md,ActiveTasksTransition.md,CLAUDE.md,G4BCorrections.md,G4WorkflowFoundationReviewPacket.md,README.md,WorkflowFoundationHandoff.md,WorkflowFoundationVerification.md,board/active.md,deployment.json,deployment.json.bak-20261115T225732Z,deployment.json.bak-20261115T230935Z,deployment.json.bak-20261115T231825Z,environment.sh,lanes.json,ledger/2026-10-15-CCF-CANONICAL-WORKFLOW.md,ledger/2026-10-15-CCF-DUMMY-T1.md,ledger/2026-10-15-T2-S1-CANDIDATE-V2-PREFLIGHT.md,ledger/2026-11-12-CCF-412-D073-FR10-ONE-GO.md,ledger/README.md,pilot/README.md,pilot/manager-packet.md,pilot/t2-pilot-packet.md,pilot/unity-gate-example.md,roles/execution-manager.md,roles/reviewer.md,roles/unity-operator.md,scripts/__pycache__/_ccf.cpython-312.pyc,scripts/_ccf.py,scripts/lane-status,scripts/packet-lint,scripts/unity-lock,state/board-history.jsonl,state/unity/guard,state/unity/history.jsonl,tests/fixtures/canonical-style.md,tests/fixtures/conflicting-unity-ownership.md,tests/fixtures/handoff-placeholder.md,tests/fixtures/missing-base.md,tests/fixtures/missing-role-file.md,tests/fixtures/missing-stop-condition.md,tests/fixtures/missing-unity-required.md,tests/fixtures/missing-verification-tier.md,tests/fixtures/nonunity-lane-head.md,tests/fixtures/partial.md,tests/fixtures/stop-placeholder.md,tests/fixtures/unity-nonunity-lane.md,tests/fixtures/valid-unity.md,tests/fixtures/valid.md,tests/self-test.py
KEYS {"README.md": "3baf029ef6da792c3129ce5e0e91b3097d69ee7c881f389ad0bb9a377de41123", "deployment.json": null, "scripts/_ccf.py": "24a321466e6dae61ba950c4cbc4f918e12f7d6be8a6af628341c5aa8efe0b435", "scripts/unity-lock": "8fa0c5ada367f109c9437b05bd37b42c7ce78472d6f630634df8745fcb220ac8"}
/media/jer/ZX20/CCF-Temp/Sen1-754-Author-20261118T170536Z/replica/CCF-Ops files=50 manifest_sha256=268cfdf6a894b49274755fe83c4b5d96ee18b57b85cb82a63b1cd91bd0ba0c13
DIFF D board/active.md,lanes.json
KEYS {"README.md": "249be391e172e9d80f7d3d3419af961850b84c40fb2f15ba7b1d0469649eb517", "deployment.json": "8c66b290c247708846cb8b59f83626a10a3fa1d052397a3316dc5f23fc157b18", "scripts/_ccf.py": "ab4343efc7d1b82c5a4aca747d0e09c614779cfd9aa406a0dd681d204f5de001", "scripts/unity-lock": "54cf0a95561d0a8c350d89adbee83f393990b728d6127b94f69c587260f048e5"}
```

### E16

claims-tail; shell exit 0.

Command:

```bash
gh api --paginate repos/jermur788/CCF/issues/781/comments --jq '.[] | select(.id > 6996384005) | "COMMENT \(.id) \(.created_at)\n\(.body)\n"'
```

Exact output:

```text
COMMENT 6996404168 2026-11-19T01:02:30Z
**#781 CAPACITY NOTE — A2 REROUTED (RECORDED, NOT SILENT).** The first A2 dispatch (`claudeAgent` / `claude-sonnet-5-5`) failed before any work with a Claude API rate limit. It made no edits and published nothing. A2 was rerouted to the separate `claudeAgent_claude_b_pro` provider instance with the same `claude-sonnet-5-5` model and the same bounded brief, and instructed to record the reroute in its own document. This is a quota-source reroute, not a scope, evidence or review-gate change: no gate is treated as satisfied by the reroute. A1 (`codex` / `gpt-6.1-sol`) is unaffected.


COMMENT 6996473911 2026-11-19T01:06:16Z
**#781 PLACEMENT CORRECTION — T3 ISOLATED CHECKOUTS LANDED ON ROOT; LANES RE-PREPARED ON FILES.** Recorded 2026-11-19T01:0xZ, before any A1/A2 write.

A1 correctly **stopped at its own storage gate** and produced nothing. Evidence it returned: its T3-prepared checkout resolved to `/home/jer/.t3/worktrees/CCF-Primary/t3-fc2f7127-b3fe632b8d2b9a9d`, and `findmnt -T .` showed `/dev/sda2 ext4 … d393b49a-…` — that is the **root filesystem**, not `/media/jer/Files`. Files had 42.4 GB free, and the shared bare Git store is on Files, but the checkout itself was on root. Under the binding owner no-root directive of 2026-11-18 that is not acceptable, so A1 refused to create a branch or write. That is the correct behaviour, not a failure of the task.

Confirmed systemic finding: T3's default `isolation: "worktree"` places per-task checkouts under `/home/jer/.t3/worktrees/`, which is on the root filesystem. There are 25 such checkouts on this host (2026-11-12 → 2026-11-19). Any CCF delegated work using the harness default therefore violates the owner no-root placement rule unless the checkout is prepared elsewhere.

Actions taken (all reversible, no user work touched):
- Cancelled the second A2 attempt before it did work, for the same placement reason.
- Removed the three T3 checkouts created by these two failed/cancelled dispatches (`t3-fc2f7127…`, `t3-f03f9a90…`, `t3-1e5375b5…`). All three were clean, at `a42eaadc`, holding no commits and no user content. Root free space went 31 GB → 33 GB. No other T3 checkout was touched.
- Prepared the packet's own candidate worktrees on Files at the exact base `a69b3e2e…`:
  - A1: `/media/jer/Files/CCF/Operations/CCF-781-A1-PhysicalIdentity-20261118` on `docs/sen-1-781-a1-physical-identity-20261118-v1`
  - A2: `/media/jer/Files/CCF/Operations/CCF-781-A2-Coordination-20261118` on `docs/sen-1-781-a2-coordination-20261118-v1`
  Both verified `ext4 /dev/sdb4`, clean, at the required HEAD.
- Re-dispatched A1 (`codex` / `gpt-6.1-sol` high) and A2 (`claudeAgent_claude_b_pro` / `claude-sonnet-5-5`) with a mandatory first-step placement gate that must show `/media/jer/Files` on `/dev/sdb4` before any read or write.

The packet's candidate paths are therefore now genuinely in use rather than reserved, and the earlier registration comment's note that they were unused is superseded by this one. Parent worktree remains `/media/jer/Files/CCF/Operations/CCF-781-Parent-20261118`. Files free space after preparation: ~38 GB, still above the 10 GiB floor. No installed Ops, configuration, board, lock, coordination file or source was changed.
```
