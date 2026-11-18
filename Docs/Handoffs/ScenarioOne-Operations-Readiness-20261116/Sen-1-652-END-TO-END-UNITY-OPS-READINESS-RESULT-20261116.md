# Sen-1 #652 — cumulative source readiness result

## Current D-091 source continuation — 2026-11-18

**SOURCE INDEPENDENTLY REVIEWED — APPROVED. NOT DEPLOYED. NATIVE UNITY NOT RUN.** The new different-model-family verdict covers exact source `4d03f5b5b15cdb8aa4f7ff6ca23688bb6c83fd78` only; Manager acceptance, integration and deployment are separate gates. Earlier historical approvals below cover older bytes only.

### Identity, authority and fresh admission

Actual author: OpenAI/Codex **gpt-6.1-sol medium**, native T3 Code, host **jer-laptop**, same session **9bcdfde6-699b-41f7-befb-acc9a7551f09**. Source worktree `/media/jer/Files/CCF/Operations/CCF-OpsReadiness-652-20261116`; branch `ops/sen-1-652-unity-lease-readiness-20261116-v1`; starting local/remote HEAD `a9f56d6385d49ac51e35f303de00d77d1e586195`. It was clean including ignored/untracked files. Original `343ae9909b223b105f962a39058df1e9a544323a` is an ancestor; the canonical store has exactly this branch checkout and the legacy store has none. Prior author explicitly handed off. Current all-project active T3 registry showed no competing CCF source writer. Source context in this historical lane is `9f06398c068cbaed5b307229241c3640c5646d6d`; current owner/Manager D-091 and storage instructions take precedence over obsolete source context wording.

Binding authorities read in full: [original same-workstream packet](https://github.com/jermur788/CCF/blob/docs/sen-1-652-end-to-end-ops-readiness-packet-20261116-v1/Docs/Handoffs/ScenarioOne-Operations-Readiness-20261116/Sen-1-652-END-TO-END-UNITY-OPS-READINESS-AND-CONTROLLED-DEPLOYMENT-PACKET-20261116.md), [D-091 amendment](https://github.com/jermur788/CCF/blob/docs/sen-1-731-chas-manager-intake-20261118-v1/Docs/Handoffs/ScenarioOne-Operations-Readiness-20261118/Sen-1-731-MANAGER-733-REVIEW-AND-652-NO-EXPIRY-OPS-SOURCE-CONTINUATION-20261118.md) (blob `886bea5dd8a0cc2ddc4d518608a414d9280a1ac7`), and [actor-specific admission approval](https://github.com/jermur788/CCF/blob/docs/sen-1-731-chas-manager-intake-20261118-v1/Docs/Handoffs/ScenarioOne-Operations-Readiness-20261118/Sen-1-731-652-D091-SOURCE-ADMISSION-APPROVAL-AFTER-BOARD-REFRESH-20261118.md) (blob `fc29ffd2d54062d2e5b87ebbbbcf3cc82c2fe04d`). Current main coordination and #652 comments were refreshed; no revocation/reassignment found.

Physical existing `CCF-652-OPS-READINESS` row now names this actual session, path, exact base, bounded new feature and explicit-release/no-expiry custody. Fresh board SHA-256 `d98c31560c7dc19047bb78bd0995045bd4a242adac491b3dab42bd9359fad272` matched the separate custodian's [one-row receipt](https://github.com/jermur788/CCF/issues/652#issuecomment-6978468601). Original installed `packet-lint /media/jer/ZX20/CCF-Ops/board/active.md --board`: **PASS42 / exit0**. The source author made no physical board edit or maintenance-lock change. `Lane None` has no configured lane for original `lane-status --preflight`; per admission §3.3, exact source/store/board/custody checks are used here. **No lane READY is fabricated and #690's preflight is not substituted.** Original `lane-status --storage-only --json`: PASS/exit0, real `/dev/sdd2` matching persistent ZX20 pin, 53,086,126,080 free bytes vs 16,106,127,360 threshold. Files source is ext4 UUID `2283b135-bf37-42d6-921f-2fb861d0a8d4` on `/dev/sdb4`, ~41.8GB free at admission. Raw production lock absent. No production lock CLI invoked.

Actual [RUNNING registration](https://github.com/jermur788/CCF/issues/652#issuecomment-6978622210) preceded every source edit and source test. The earlier BLOCKED state was correctly retained until the custodian refresh and Manager approval.

### Pinned change and semantics

[Source commit **4d03f5b5b15cdb8aa4f7ff6ca23688bb6c83fd78**](https://github.com/jermur788/CCF/commit/4d03f5b5b15cdb8aa4f7ff6ca23688bb6c83fd78), pushed to the existing branch. Exactly four source files changed against the starting revision, plus this existing cumulative report at publication:

- `CCF-Ops/scripts/unity-lock`: explicit mutually exclusive `--no-expiry` / legacy `--eta`. No mode is an error. Null creation writes `eta:null` and boolean `no_expiry:true`; missing ETA, unmarked null, nonboolean marker and marker/timestamp disagreement are malformed and remain blocking. Legacy records without the new key retain their timestamp shape and are read without migration. `eta_overdue` is false for indefinite mode and advisory for old timestamps; no elapsed-time release/reclaim/kill exists.
- Same script: normal matching owner/task/lock-id release additionally refuses any detected live Unity Editor or any still-existing recorded PID (including reused PID). PID boot/start identity inspection remains. Unknown PID still requires manual operator verification. Force-release semantics and approval/audit requirements are unchanged.
- `CCF-Ops/tests/self-test.py`: add 92 meaningful checks while retaining all prior 141 checks. Tests corrupt only isolated fixture JSON, simulate Editor detection without launching any Editor, let their own Python fixture process exit naturally, and race actual CLI subprocesses under the real isolated guard.
- `CCF-Ops/README.md` and `CCF-Ops/roles/unity-operator.md`: direct mode/release procedure, actual worker identity, manual verification, no stale reclaim and rollback incompatibility warning. No broader storage-role cleanup was performed.

`_ccf.py`, `lane-status`, `packet-lint`, lanes/config, environment and deployment manifest unchanged. No game/serialized Unity files, assets, packages, saves, old worlds or other worker paths changed. Existing source `deployment.json` is historical provenance and was deliberately not restamped as a deployed or independently accepted D-091 candidate.

Trade-off: explicit mode marker distinguishes legitimate null from an accidentally truncated estimate; old readers cannot consume the new record. No live JSON migration is performed. A still-existing recorded operator PID now conservatively blocks normal release even if it is not an Editor; finish that process, or obtain the pre-existing explicit Manager force-release ruling after investigation. This prevents a matching ID from transferring custody while recorded execution remains live.

### Qualification actually run

At exact committed source bytes above:

| Gate | Result |
|---|---|
| `python3 -B CCF-Ops/tests/self-test.py` from the Files worktree | **233 PASS, exit0** |
| `python3 -B <owned ZX20 source copy>/CCF-Ops/tests/self-test.py` | **233 PASS, exit0**, all source files SHA-256 identical before and after |
| Focused storage identity campaign using inspected `_ccf` helpers | **10 PASS, exit0**; real mounted positive, explicitly mocked wrong mount source/target/fstype/missing+wrong UUID, missing+duplicate visible UUID, unique UUID positive, capacity inflation refusal |
| Python AST parse, `git diff --check`, source scope | PASS |
| Full live copied Ops inventory bytes+mode comparison | **50/50 identical**, before vs after qualification |

Original storage/device/UUID/fstype/root/sibling/symlink/default-reader/NonUnity/foreign-store/dual-store/owner/force/concurrency/pilot tests all reached. New tests include null acquire/status/second-block/matching release and complete prior-record history; required mode and mutual exclusion; invalid/elapsed estimates; dirty/branch/HEAD/dual-store/real low-space refusal; all required-key omissions, unmarked/inconsistent/corrupt mode and metadata; wrong owner/task/lock ID; old timestamp format and overdue held state; actual live recorded process release refusal; simulated Editor release refusal with unchanged history; PID start/boot mismatch and naturally exited PID without reclaim; and a second six-process acquisition race for no-expiry.

Files full result identifies source HEAD `4d03f5b5…`. The copy has no Git metadata, so its existing self-test reports historical manifest source HEAD `dfe445e…`; **that field does not identify the new copied source**. Exact copy identity is instead recorded by `source-copy-manifest.json` (all copied file hashes tied to `4d03f5b5…`) and checked before/after. No provenance file was edited to manufacture a newer stamp.

Durable evidence: `/media/jer/Files/CCF/Verification/ScenarioOne652Readiness-20261116/D091-20261118-9bcdfde6`. Full logs, result JSON, pilot receipts, source diff and hash manifests retained there. Both author-owned `foundation-test-9_7ur954` / `foundation-test-an44l36g` fixture trees and the byte-identical source copy were removed after preserving results/pilot evidence; no inactive test Git caches archived. The self-test's own unique Files fixture/symlinks were removed in its existing finally block. Shared cache directories and other workers' scratch were not removed.

Candidate hashes:

| Path | SHA-256 |
|---|---|
| CCF-Ops/scripts/unity-lock | 8fa0c5ada367f109c9437b05bd37b42c7ce78472d6f630634df8745fcb220ac8 |
| CCF-Ops/tests/self-test.py | 1bd03f1520e21db6f6bd00b963e09437267ebf4604ad6772c8e5fffffa9e6e50 |
| CCF-Ops/README.md | a1911b01ece918d4eb52bfde230fe197e34f839a8c41d79f67555d9a560598e4 |
| CCF-Ops/roles/unity-operator.md | e44e7eb89d33a82d4e3dc5987572565e394d12a60ec82733d3dfd4bb103206db |

Evidence hashes:

| File under evidence root | SHA-256 |
|---|---|
| files-full-1.log | b9c5950772541d778d9dd9e0c1fdf77770fdbdb7b5bab3abe81e82c8bdc3648d |
| zx20-full-1.log | f013f0da5d5e8edefdd7ca1b11409d092e5841d73b5514d5ffb7ca3c3bb20f35 |
| focused-identity.log | 99863e2eceed30da1d0fade2fd71a40b3266f1bf819cf51ba4299265e8e9d3d4 |
| live-before.json | 055a9689a66519ff778ead3e7bf664984af2ac0860e2f29cd880125f886ace62 |
| live-after.json | 055a9689a66519ff778ead3e7bf664984af2ac0860e2f29cd880125f886ace62 |
| source-copy-manifest.json | 6dbd01b6ed397feb7da2357244f018c5aac998b0bfcdb67bfe2f0b2430745a54 |
| source.diff | 7d09fe9c41edd6dd8d7e8e465cbb36e70ff066e58cfb0012669f7b33e6a7d9f0 |

The live inventory includes physical board/lanes, all deployed scripts/docs/config, guard and history; all 50 bytes+mode entries match. This is content/mode evidence, not a global forensic claim about access times. Initial Primary unrelated modifications (AGENTS.md, StorageArchitecture.md and the two pre-existing Ops handoff documents) were preserved. No native Unity/Player/Windows test, production acquire/status/release, deployment, backup/rollback rehearsal or new installed-provenance PASS is claimed in this D-091 round. Earlier installer rehearsals below remain historical.

### Independent review completion and later physical drift

The author fetched and read the actual GitHub reviewer registration/verdict and local `REVIEW-VERDICT.md`. Reviewer reports two independent full-suite runs, each **233 PASS / exit0**, 27 direct parser records without mismatch, old-reader fail-closed confirmation and direct no-expiry storage negatives. Its 25 test mutants yielded 18 detected and seven surviving; some survivors are equivalent/masked, while three identify nonblocking test gaps. These counts are reviewer evidence, not author reruns. Evidence stays in the reviewer's separately owned `reviewer-claude-sonnet55-5b89dd08/` folder; author did not alter or clean it.

Disposition of nonblocking findings: M-1 unclaimed-live-Editor acquire coverage, M-2 mode flags on status/release, and M-3 non-string ETA suite coverage remain optional follow-up tests; code was found correct, including direct reviewer parser probes. Preserve the approved source freeze. M-4 operability is an explicit deployment/operator constraint: record the **Editor PID**, not a long-lived shell PID; normal release will refuse any still-existing recorded PID, including reuse, and the original Manager-approved force path is the only investigated exception. Note also that passing `--eta`/`--no-expiry` to status/release is rejected now; a future installer must inspect callers for this compatibility detail. Existing deployment-manifest hashes are stale by design in this source-only scope; regenerate exact installed provenance under the separate reviewed deployment packet. None of these observations grants an installation or force-release permit.

After the author's earlier qualification snapshots, the reviewer observed an external board/lanes update at reported **12:41:21Z**. Fresh author read-only comparison confirms only these two live inventory entries differ from the author's admission snapshot:

- `board/active.md`: `d98c31560c7dc19047bb78bd0995045bd4a242adac491b3dab42bd9359fad272` → `ae80e1908956a228ac77cc29858585361a2ecaffa70dd0337992bdb733d43cda`.
- `lanes.json`: `b16bfcad2fbb0be09c8d44fa0acdc87841d458e784c8befee2668a3650eacd61` → `51850d741cd05e7125f093b8357c6bb74ee2ed9c424fcdad087ad2344125e9c4`.

Fresh original board lint remains **PASS42 / exit0**. Current #652 row still names the same Codex session, source branch/base/scope and no-expiry reservation; its historical prospective status/base text needs Manager/custodian reconciliation to this source result. All other inventory entries, including installed code, guard/history and deployment manifest, match the original snapshot. All four reviewed source hashes match, worktree clean before this report-only update. The author did not perform the external writes and has not adjudicated their authority; custodians/Manager must reconcile them before any future physical action. The earlier 50/50 match is a time-bounded qualification observation, not a claim that live state stayed frozen throughout review. The reviewer reran its suite successfully after the drift; this does not replace fresh installer admission or permission.

### Separate review and handoff

Fresh independent nonauthor review assigned through T3 to **Anthropic Claude Sonnet 5.5 high**, a new reviewer session distinct from author and physical custodian. Review target is exact `4d03f5b5…` vs `a9f56d63…`, read-only source and separate owned evidence only; reviewer registers and publishes its own source-only verdict on #652. Task ID `node:delegated-task:command%3Amcp%3A3714ac9a-b5de-45d4-bd5a-23be713506c2%3Adelegate-task%3Asen1-652-d091-independent-review-4d03f5b5`. **[APPROVED — SOURCE ONLY](https://github.com/jermur788/CCF/issues/652#issuecomment-6979064889), zero blocking / zero major / four minor findings.** Actual reviewer session `5b89dd08-bf18-407b-bce2-8ea19d9aef34`, [registration](https://github.com/jermur788/CCF/issues/652#issuecomment-6978790004). Previous older-source approval is not applied to this delta. The reviewer did not approve the report text or installation.

Worker assessment: SOURCE INDEPENDENTLY APPROVED; ready for Manager source acceptance and separate deployment planning, no self-acceptance. Source changes are committed and remote-published; this report-only followup does not change tested source. Custody is retained pending documented safe handoff/Manager disposition, not a timer. Physical board remains custodian-owned and has not been reconciled by the source author. `HANDED-OFF`/`RELEASED` are receipt/ledger wording, not new table states.

Completed review gate: nonauthor changed-lines APPROVED for exact source `4d03f5b5…`. Remaining gates: Manager acceptance of source result; a distinct exact physical installer permit and reviewed current provenance/backup/readback/rollback plan before any deployment; only later separate Unity/content-start grants. Never copy source board/lanes/ledger/state onto live. Null-record rollback cannot use an old reader while indefinite ownership is active: first verify process exit and explicit safe release, or obtain independently reviewed compatible fallback. No #690/Multi lease rewrite or transfer.

**DASHBOARD SYNC: NOT AVAILABLE / NOT VERIFIED. Published dashboard reflection: NOT VERIFIED.** This author cannot edit the physical board or canonical Manager coordination; authorised Manager/custodian ingestion and dashboard readback remain follow-up. No Drive mirror refresh claimed. No live Ops installation or Unity execution is authorised by these results.

---

## Historical cumulative record — 2026-11-16 (older source only)

# #652 cumulative Option A result — SOURCE APPROVED; NOT DEPLOYED

**Current outcome:** approved, qualified source candidate returned to Overall Manager #640 for a **separate deployment decision**. Latest direct user instruction prohibits live installation and supersedes the original packet's conditional install phase. No new task, issue or implementation lane. Earlier policy blockers/results below are retained as historical evidence and are superseded only where the owner Option A amendment applies.

## Exact source, authority and independent review

- **Runtime:** `0ea3ca4c4cc2570f869ca05fe7bfd0bae2203ccd`.
- **Reviewed source/receipt:** `926547016ee1cbaec3b8d51ba9afebf4a0992306`, same branch `ops/sen-1-652-unity-lease-readiness-20261116-v1`. Runtime→reviewed tip changes only candidate deployment.json. The final handoff commit changes only this cumulative report; all ten reviewed candidate file hashes remain unchanged.
- **Owner authority:** [Option A signed amendment](https://github.com/jermur788/CCF/blob/docs/sen-1-652-owner-option-a-amendment-20261116-v1/Docs/Handoffs/ScenarioOne-Operations-Readiness-20261116/Sen-1-652-OWNER-APPROVED-OPTION-A-NONUNITY-DEFAULT-ADMISSION-AMENDMENT-20261116.md), [updated full workstream](https://github.com/jermur788/CCF/blob/docs/sen-1-652-end-to-end-ops-readiness-packet-20261116-v1/Docs/Handoffs/ScenarioOne-Operations-Readiness-20261116/Sen-1-652-END-TO-END-UNITY-OPS-READINESS-AND-CONTROLLED-DEPLOYMENT-PACKET-20261116.md), [Manager acceptance](https://github.com/jermur788/CCF/issues/652#issuecomment-6932109117).
- **Independent nonauthor source verdict:** [APPROVED, zero blockers/should-fix](https://github.com/jermur788/CCF/issues/652#issuecomment-6932456697), separate Anthropic Claude Sonnet5.5 high / Claude Code through T3 Code. [Reviewer registration](https://github.com/jermur788/CCF/issues/652#issuecomment-6932442317) discloses reading began before the comment. Actual reviewer differs from Codex author despite shared GitHub login; reviewer explicitly did not verify independence beyond nonauthorship/different provider and receipt of task/docs/source only. Source approval does not grant a deployment permit.

## Admission and source change

Actual author remains OpenAI gpt-6.1-sol **medium**, configured Codex provider / Codex via T3 Code, jer-laptop. Clean existing source worktree `/media/jer/Files/CCF/Operations/CCF-OpsReadiness-652-20261116`, local+remote starting HEAD `3f4f64f08802445d4382967e78e14d15112cdece` rechecked. Fresh main prospective reservation, both binding documents, relevant issues, all-project active registry and both configured Git stores inspected. Other running docs/read-only evidence worker was disjoint. No Editor or production lease at admission. [Actual pre-edit RUNNING](https://github.com/jermur788/CCF/issues/652#issuecomment-6932162644).

Existing physical #652 row already covered every intended source path. Read back under actual nonblocking exclusive board-directory maintenance flock and original unchanged packet-lint PASS32; **no physical row edit necessary or performed this round**. Lock released normally. Board remains `4138a62787d9546e425d632480c6f4030a2dd8cf52a4c020ca7636a86fc8bf29`. Prospective row is RESERVED ONLY, not a false current RUNNING claim. No live write or live deployment permit inferred from custody. Source worker ends HANDED-OFF; custodian/Manager retains coordination reconciliation.

Small semantic delta: `_ccf.config_read` now uses bounded resolved project-storage admission for listed rows with exact `unity:false`, independent of the legacy inventory flag. Unity rows still use strict verified ZX20 `on_storage`. Existing default consumers remain guarded by unchanged config/storage/repository/board checks. `unity-lock` adds an early explicit non-Unity selection rejection before creating state/guard; the guarded Unity acquisition still performs all existing ownership/store/branch/HEAD/PID/storage checks. No writable Files preflight or Unity route added.

Touched in this round: scripts/_ccf.py, scripts/unity-lock, tests/self-test.py, README.md, candidate deployment.json and this SAME result. Other production consumers/config unchanged. DeploymentProvenance.md is explicitly outside the ceiling and remains unchanged; owner amendment and README supersede only its old default-reader sentence. Candidate provenance is source-only, live observed receipt is unchanged.

## Fresh qualification and independently reproduced checks

**Author full original suites:** **141 PASS / exit0 from Files source**, **141 PASS / exit0 from byte-identical ZX20 source copy**. Initial diagnostic run134 PASS preceded seven final additional witnesses; it is not substituted for the final suites. Historical90+90 and earlier red attempts are not Option A evidence. Every original assertion remains meaningful, with the changed root-error cause reflecting the owner-approved boundary; full ownership, force-release auditing, stale/malformed records, dual-store conflicts, concurrency and pilot/ledger closure all reached.

New real-shaped default-consumer tests copy exact live lanes.json and board bytes into an isolated candidate tree; CLI config options are omitted. `unity-lock status --test-state-dir` returns0/free with no state; packet-lint returns0/PASS; source environment.sh returns0/PASS; `acquire --lane NonUnity --test-state-dir` returns2/non-Unity-specific rejection without state or guard. Invalid mixed configs fail with the specific root-boundary cause (status/linter2, sourced environment1). The actual live config is also read by the new default loader with no mutation. Bounded Files inventory remains ZX20:NO, writable preflight BLOCKED. Unity-at-Files, arbitrary root, siblings, parent/symlink escapes, malformed/nonabsolute/type/duplicate lane/store/repository configs reject in both default and inventory modes. Valid Unity fixtures still acquire/release isolated leases.

**Focused10 PASS / exit0:** real mounted identity positive plus explicitly simulated wrong mount source/target/fstype/UUID, missing/duplicate visible UUID and capacity-inflation refusal. Earlier security negatives are retained in full tests. **Cleanup:** repeated normal runs remove the exact mkdtemp-owned Files worktree and escape symlinks in finally; deliberate in-memory RuntimeError injection after both symlinks exist confirms exception propagation and finally cleanup. This injection is a separate failure-path diagnostic, not a claimed full-suite pass. Three historical fixtures named by the reviewer were additionally removed after verifying exact filenames, clean fixture bytes, clean Git status and disposable seed store ownership; they contained clean `.git`/fixture.txt entries rather than being empty, so safe `git worktree remove` plus parent rmdir was used.

**Independent reviewer actually ran:** full original suite from exact-tip ZX20 archive, **141 PASS / exit0**, matching label sequence; default consumer/blocked preflight probes, roughly100 adverse config outcomes, and mutants proving the old loader fails positives, removing early reject creates guard, and loosening Unity bounds is caught. Verified source/copy10 hashes and source/live provenance. Reviewer read author Files/focused/cleanup/rollback evidence but did not independently rerun those; a slow all16-lane inventory was terminated and a three-lane projection used. Copied-suite `source_head=dfe445e5...` remains the documented historical receipt fallback without `.git`, NOT runtime provenance; ten exact byte hashes identify0ea3ca4c runtime.

[Published original complete logs and initial scratch rollback evidence](https://github.com/jermur788/CCF/issues/652#issuecomment-6932252503). Final audits: live **28 source hashes +7 pinned overrides +1 config projection PASS**, candidate **10 hashes PASS**, unchanged original physical validator **PASS32**. `git diff --check` passes. Full source file bytes and approved ten-file set unchanged after review.

## Rollback evidence for separate Manager decision

No live installation. All rehearsed replacements were inside `/media/jer/Files/CCF/Verification/ScenarioOne652OptionA-20261116/rollback-lab` or `receipt-rollback-lab-final`. Live tree was only read/copied; real board-directory lock was acquired/released for contention sensitivity without mutation. Production Unity guard/history/lease untouched.

Initial rehearsal: verified full50-file backup including modes; atomic compare-before-write+fsync five-file scratch projection (`scripts/_ccf.py`, `scripts/unity-lock`, environment.sh, tests/self-test.py, README.md); default consumers succeed; ten protected state/config paths unchanged; foreign-drift precheck refuses rollback before any restore write; exact complete bytes/modes restored. Independently reviewed as code/evidence, not independently rerun.

Reviewer D-1/D-2 prompted **author supplementary** rehearsal, not a new runtime change or independently cleared deployment gate. It adds candidate installed-projection attribution in the SCRATCH receipt, moves exactly changed immutable hashes out of obsolete overrides, and verifies projected provenance **31 source+4 overrides+1 config projection and10 candidate PASS**. Exact original receipt rollback verifies baseline **28+7+1 PASS**. Injected failure after three of five file replacements restores exactly the applied subset and original complete tree. Second descriptor on the actual live board directory correctly receives contention rejection; normal release and unchanged live bytes/modes confirmed. All50 backup/restore file bytes+modes and ten protected paths pass. Initial supplementary audit incorrectly compared the restored historical candidate metadata against current Option A source; that diagnostic failure is preserved and audit scope corrected in a fresh lab. No source, rollback byte equality or live state failure occurred.

Future Manager must choose exact per-file projection and correct installed-only receipt attribution, refresh all live inventory/modes/custody/storage/Editor/lease gates, review supplemental evidence, and explicitly permit deployment. This report grants no installation authority. No blanket recopy, live lanes rewrite, board/state replacement or false deployment timestamp. A drifted installed file must block restore rather than overwrite another owner's bytes. The rehearsal scripts refuse lab reuse and are evidence, not a production installer.

## Evidence, remaining limitations and final readback

Durable root: `/media/jer/Files/CCF/Verification/ScenarioOne652OptionA-20261116`. Contains physical-readback.json, live-before/final.json, protected-anchors.json, full-files.log/full-zx20.log, copy-hashes.json, full-*-receipts/default-caller-matrix.json and pilot/result receipts, focused.py/log, cleanup_failure_probe.py/json and log, legacy-fixture-cleanup.json, rollback_rehearsal.py/result, rollback_receipt_rehearsal.py, rollback-receipt-result.json, preserved initial supplementary diagnostic, exact source delta/replay and reviewer snapshots. Earlier roots remain cumulative historical evidence.

Remaining nonblocking source nits accepted by reviewer: exactly Files/CCF root itself is readable as nonUnity; symlink loops fail closed with traceback/exit1 rather than clean2; legacy permit flag is now a compatibility no-op and its tests duplicate modes; machine-specific live NonUnity fixture assumptions. No runtime edit made after approval to remove these. Current test cleanup and historical fixture cleanup completed. Supplementary D-1/D-2 evidence is author-run after review and awaits Manager evaluation; it is not independent live qualification.

**Final statuses:** SOURCE **APPROVED/QUALIFIED**; INDEPENDENT REVIEW **APPROVED for exact runtime0ea3ca4c/reviewed receipt92654701**; LIVE DEPLOYMENT **NOT DEPLOYED — separate Manager decision/permit required**; NATIVE UNITY **NOT TESTED**. All50 live file hashes unchanged throughout Option A, including original board/lanes/receipt/ledger/state/guard/history; both original dirty anchor files match protected hashes. #548 left open. No Unity Editor/import/runtime, real lease acquisition/force/release, game source, saves/Prefs, main merge or Windows build.

Dashboard ingestion/audit/published state, Drive/context mirror synchronization **NOT VERIFIED**, outstanding authorised-custodian follow-up. No generated dashboard state edited. This is the completed requested source-only handoff within existing #652, not a claim of live readiness.

---

# Historical pre-Option-A cumulative handoffs (superseded current-policy status)

# #652 cumulative result — BLOCKED deployment; source APPROVED and full self-test GREEN

Updated 2026-11-16 after the Manager continuation. **NOT DEPLOYED.** Same issue, same branch/worktree, same primary worker; no new implementation issue. The first red result below is historical, superseded by these current source/review results. Live/native readiness is still NOT qualified.

## Current disposition and exact unresolved gate

Source runtime **83c17c027e6427ebf0fa42728ac7dd170510c1f8**, reviewed source/receipt **9ee40f1d567df441de8d0c8349b0fd1ae7071ed9**. Full original self-test **90 PASS, exit0 from Files**, and **90 PASS, exit0 from safe ZX20 copy** on the same ten candidate file hashes. Focused security witnesses **10 PASS, exit0**. Separate Anthropic Claude Sonnet5.5 high reviewer [APPROVED the exact source](https://github.com/jermur788/CCF/issues/652#issuecomment-6931560520) but explicitly **BLOCKED deployment pending Manager policy ruling**. [Reviewer identity registration](https://github.com/jermur788/CCF/issues/652#issuecomment-6931549330); actual provider/worker distinct from author despite shared GitHub login. Reviewer independently ran provenance and bounded admission probes, read full logs, and explicitly did not rerun the full suites. This resolves the earlier NEEDS CHANGES source verdict, not the deployment gate.

Exact failing read-only trial: candidate `_ccf.config_read('/media/jer/ZX20/CCF-Ops/lanes.json')` rejects `path escapes ZX20: /media/jer/Files/CCF/Operations/CCF-NonUnity`. The retained live non-Unity lane is on bounded Files CCF. Candidate intentionally restricts default callers to ZX20 while inventory alone opts into bounded Files admission. Live helper admits bounded Files non-Unity by default. Installing the candidate helper would break default callers (`unity-lock`, `packet-lint`, `environment.sh`); retaining the old helper makes the candidate environment/test imports incompatible. Rewriting live lane registrations is forbidden. Reviewer independently confirmed this is a genuine documented admission-policy gate, not another ordinary fixture defect.

**Manager ruling needed within this same #652 workstream:** may default callers admit bounded Files CCF non-Unity lanes, while Unity remains ZX20-only and arbitrary/sibling/symlink escapes remain rejected? Reviewer recommends yes, but that recommendation is NOT accepted policy or implemented source. Do not dispatch a native gate or claim machine-wide lease readiness. If accepted, a bounded shared-helper correction plus explicit default-caller positive/negative tests, proportionate qualification, exact delta review and renewed deployment gates remain necessary. No new implementation issue is requested.

## Resumed admission and physical custody

Read complete [amended binding packet](https://github.com/jermur788/CCF/blob/docs/sen-1-652-end-to-end-ops-readiness-packet-20261116-v1/Docs/Handoffs/ScenarioOne-Operations-Readiness-20261116/Sen-1-652-END-TO-END-UNITY-OPS-READINESS-AND-CONTROLLED-DEPLOYMENT-PACKET-20261116.md), refreshed main prospective reservation and #548/#553/#640/#647/#648/#652. Registry showed only this active worker; both configured Git stores had no competing target-branch checkout. Existing clean isolated worktree and remote/local starting HEAD723767de rechecked. Actual author remains OpenAI gpt-6.1-sol medium / configured Codex provider / Codex via T3 Code, jer-laptop. [Resumed pre-edit RUNNING](https://github.com/jermur788/CCF/issues/652#issuecomment-6931262852). Known dirty anchor files preserved.

As expressly authorised by continuation, amended **only the existing #652 row** to add environment.sh to the exact ceiling. Actual board-directory exclusive nonblocking flock, second-descriptor contention rejection, exact recovery bytes, original unchanged packet-lint PASS32 before/after, atomic compare-before-replace and independent byte readback. Normal lock release in finally. No second row or unrelated row edit. Before board `c9eb74c397691d07d5527a2d66ca86811e05cd54c39a2aaff19b581ffbcfa057`; after **4138a62787d9546e425d632480c6f4030a2dd8cf52a4c020ca7636a86fc8bf29**. Final full live hash-set comparison confirms this one-row amendment is the only live-file change. lanes.json, scripts, deployment.json, ledger and lease state remain byte-identical. Original packet-lint final PASS32. Prospective row remains READY/RESERVED ONLY, not a false RUNNING claim; final source worker is handed off. Physical custodian/Manager must reconcile that reservation before future mutation.

## Source corrections and complete verification

Cumulative source delta since #548 parent343ae990: tests/self-test.py, README.md, environment.sh, candidate deployment.json and this SAME named result only. No production `_ccf.py`, lane-status, packet-lint or unity-lock changes. The four earlier test findings are repaired: actual state-writing CLI rejects out-of-bounds config before guard creation, paired with a valid-config positive control reaching guard; bounded Files fixture is an actual separate registered worktree regardless of source location; root/sibling/parent/symlink/ZX20-sibling negatives; explicit blocked Files writable preflight. Additional source controls cover wrong/missing UUID, fstype, device pin, mount root, foreign Git store and mismatched task/lease release.

Source environment now retains scratch on ZX20 and checks existing bounded Files CCF Verification/Builds outputs; compares inspected already-live migration semantics and strengthens resolved durable-root admission. UUID/fstype/mount/capacity checks unchanged. No retired output directory created. README reconciles storage facts and admission semantics. Provenance candidate identifies ten exact semantic/config/document file hashes at83c17c02 without claiming installation. The final report/metadata-only commit corrects the stale note about Git/live receipt equality; runtime and all ten reviewed candidate hashes stay unchanged.

Final commands, both exit0:

```bash
cd /media/jer/Files/CCF/Operations/CCF-OpsReadiness-652-20261116
python3 -B CCF-Ops/tests/self-test.py
python3 -B /media/jer/ZX20/CCF-Temp/ops652-final-source/tests/self-test.py
python3 -B /media/jer/Files/CCF/Verification/ScenarioOne652Resume-20261116/focused.py
python3 -B CCF-Ops/scripts/verify-deployment --root /media/jer/ZX20/CCF-Ops --manifest CCF-Ops/deployment.json --candidate-root CCF-Ops
```

Both original FULL90-check runs include previously unreached dual-store collision, duplicate lane/branch, ownership, stale/malformed record, audited force release, concurrent acquisition (exactly one winner), isolated lock release and complete pilot/ledger closure. No skip/silence/rebaseline. Initial resumed full87-check run passed before adding three extra foreign-store/task/lease witnesses; final source was then committed and both final90-check runs executed. Focused10 includes real mounted identity positive plus explicitly simulated missing/duplicate visible UUID, wrong mount source/target/fstype/UUID and prohibited capacity inflation. Simulation labels distinguish these from actual-device checks. Source-versus-live immutable audit **PASS28 source hashes,7 pinned overrides,1 config projection,5 reasoned live-state entries**; candidate **PASS10 hashes**. Reviewer independently rehashed copy10/10.

The ZX20 copied test has no `.git`, so its emitted `source_head=dfe445e5...` is the historical receipt fallback, NOT runtime provenance. Exact copy byte hashes and source candidate receipt prove83c17c02 runtime; this mismatch is explicitly disclosed rather than misrepresented. Prior failing runs remain historical evidence in the earlier result and issue comments.

## Deployment inventory, recovery and limitations

[Published full passing logs, focused output and migration plan](https://github.com/jermur788/CCF/issues/652#issuecomment-6931450891). Durable root `/media/jer/Files/CCF/Verification/ScenarioOne652Resume-20261116`: full-files-final.log, full-zx20-final.log, focused.py/log, zx20-copy.json, live-before.json/live-final.json, physical-proof.json/physical.diff, board-recovery.md, migration-inventory.json (all50 live files), deployment-plan.md, issue/review snapshots and copied pilot/result receipts. Earlier cumulative evidence remains in ScenarioOne652Readiness-20261116.

No deployment passed gate(f); no install attempted, no live script backup/rollback needed. Complete hash inventory classifies identical files, immutable overrides and protected state/config. Candidate helper/environment/tests/docs projection is incompatible until admission ruling. Prospective rollback plan requires timestamped verified Files backup of all live bytes, exclusive board-directory flock, compare-before-write for each reviewed file, atomic replace/fsync, installed-only provenance, original validator/status/identity and protected-state equality; on failure restore exact backup only if installed bytes still match expected new bytes and exclusivity remains. No blanket recopy/lanes/board/lease overwrite. Reviewer considers recipe reasonable but requests scratch-copy dry run before any future install; it has NOT been exercised. Fresh no-Editor/lease deployment gate not reached and must be repeated, never inferred from source tests.

Reviewer nonblocking findings: own durable `escape -> /` fixture links from three runs were removed exactly and recorded in fixture-symlink-cleanup.json; future test still creates such a disposable negative fixture. Stale receipt equality note corrected; this cumulative report replaces old RED current status. Default-caller Files config witness remains explicitly absent from the passing suite and is the exposed policy gate, not a claimed tested capability.

Final readback: original32-row validator PASS, live provenance unchanged, all protected anchor hashes match the historical section. No Unity Editor/import/runtime, production game source, saves/Prefs, native Player, main merge, Windows build or frozen reference change. No production Unity lease acquired/forced/released. #548 remains open. Dashboard audit/ingestion/published dashboard/Drive **NOT VERIFIED**, outstanding authorised-custodian follow-up. No generated dashboard file edited. Source verified/reviewed; physical deployment BLOCKED; native Unity unverified.

---

# Historical first result (superseded source status; retained cumulative evidence)

# #652 End-to-end Unity Operations Readiness — NEEDS CHANGES

2026-11-16. **NOT DEPLOYED. Full original self-test is RED.** Returned to Overall Manager #640 at the mandatory outside-ceiling defect and independent review gates. No native Unity readiness claimed.

## Authority and actual worker

Binding [complete packet](https://github.com/jermur788/CCF/blob/docs/sen-1-652-end-to-end-ops-readiness-packet-20261116-v1/Docs/Handoffs/ScenarioOne-Operations-Readiness-20261116/Sen-1-652-END-TO-END-UNITY-OPS-READINESS-AND-CONTROLLED-DEPLOYMENT-PACKET-20261116.md), main prospective reservation and #548/#553/#640/#647/#648/#652 refreshed. Actual author OpenAI gpt-6.1-sol **medium**, configured Codex provider / Codex harness through T3 Code, jer-laptop. Not the preferred High effort identity. [Pre-source RUNNING](https://github.com/jermur788/CCF/issues/652#issuecomment-6930697112).

Isolated source worktree `/media/jer/Files/CCF/Operations/CCF-OpsReadiness-652-20261116`; branch `ops/sen-1-652-unity-lease-readiness-20261116-v1`; clean starting SHA `343ae9909b223b105f962a39058df1e9a544323a`. Retained context `46247486b37904f6b36825bb40fb6ed7bea35e88`. Existing #548 worktree untouched. Registry had only this running worker; both configured stores checked for branch conflicts. Files ext4 UUID `2283b135-bf37-42d6-921f-2fb861d0a8d4`, ~66 GiB free; ZX20 exFAT UUID `18D6-896E`, ~52 GiB free, original storage validator passed.

## Phase 0 — physical reservation PASS

Actual live copied Ops tree `/media/jer/ZX20/CCF-Ops`, not Git. Acquired exclusive nonblocking Linux flock on existing board directory descriptor; independent second descriptor correctly rejected. Saved exact recovery bytes; original deployed validator checked candidate before atomic same-directory compare-before-replace and physical readback. Original unchanged packet-lint PASS **32 records**, previously31. Exactly one schema-valid prospective #652 READY / RESERVED ONLY / NOT RUNNING row added with actual worker, branch, checkout and entire exact source ceiling. No configured lane borrowed. Source RUNNING separately registered above only after clean actual checkout existed. Maintenance lock normally released in finally.

Board before `a5db6cd0ccce04b92778b2fdc5ae71fb8c08fb952d78d814ce4e600c4cfaacdc`; after `c9eb74c397691d07d5527a2d66ca86811e05cd54c39a2aaff19b581ffbcfa057`. Lanes unchanged `1b6d70eb1a884d21dffc1e9d92db20a57cb728dd7a534aeec52ab8386cecaf21`. Full live file inventory/hash-set comparison before/final: only board/active.md changed. Existing rows, scripts, lanes, deployment, ledger and lease state retained byte-exact. Board remains a prospective reservation, not a current RUNNING claim; no further physical row edit attempted under one-addition authority. Manager/physical custodian must reconcile final stop state before redispatch.

## Source and verification — FAIL

Pushed source-only commit **`5b7489be838effc4174c0382090c351a251d96d9`**, exactly tests/self-test.py and README.md (+16/-5); no production script/config change. Root `/` remains rejected by unchanged validator, cause-specific exit2 assertion added; bounded Files non-Unity inventory and Files Unity rejection exercised. README describes current admission semantics. `git diff --check` passed.

Full original invocation `cd CCF-Ops && python3 -B tests/self-test.py`: inherited candidate **33 PASS then uncaught assertion / exit1** at original line108 (expected0, actual2 for `/`). First edited attempt reached37 PASS then an incorrect new STOP-warning expectation failed; code inspection showed admitted bounded Files inventory instead retains repository identity checks. Corrected that test assumption before final attempt. Final original-suite invocation **44 PASS then exit1** at line135: source environment.sh requires missing `/media/jer/ZX20/CCF-Builds`. It also still requires retired ZX20/CCF-Verification. Did not create retired paths or skip/environment-mock the check. environment.sh is outside the explicit source ceiling: mandatory STOP. Live environment.sh already uses migrated Files CCF output roots; this is stale source, not a demonstrated live environment defect.

[Published exact original and final failing logs](https://github.com/jermur788/CCF/issues/652#issuecomment-6930729333). No green full run exists. Later dual-store, ownership, force-release, concurrency and pilot sections were not reached. Full focused wrong-UUID/device/fstype/root/symlink/security campaign not completed. No assertion that safeguards are qualified by partial passes.

## Independent review — NEEDS CHANGES

Distinct cross-provider Claude Sonnet5.5 high reviewer [registered](https://github.com/jermur788/CCF/issues/652#issuecomment-6930761514) and [reviewed exact source](https://github.com/jermur788/CCF/issues/652#issuecomment-6930771035). Shared GitHub login does not conflate actual worker identities. Reviewer confirms safe stop and outside-ceiling stale environment blocker; **NEEDS CHANGES**, no deployment approval.

Unresolved review findings: new no-lock-state assertion is vacuous for lane-status; bounded Files test uses OPS.parent and is not portable to live/ZX20 test locations; sibling/symlink/ZX20-sibling negatives absent; README writable-preflight claim lacks test. These remain known defects in this published candidate, not accepted tests. No source-ready verdict. A corrected candidate needs the environment ceiling resolved, meaningful portable regression checks, full suite and final negatives, and fresh exact-source independent approval before deployment.

## Deployment and recovery

**No deployment attempted or authorised by evidence.** Source/runtime migration plan and immutable provenance gate not qualified. Reviewer notes live _ccf.py differs, verify-deployment absent live and provenance needs regeneration; no blanket recopy is safe. No source candidate hashes stamped into live manifest. No production lease acquired/forced/released; no Unity Editor/import, game source, saves/Prefs, main merge or Windows build. #548 left open.

Recovery bytes: `/media/jer/Files/CCF/Verification/ScenarioOne652Readiness-20261116/recovery-board.md`; exact one-row diff `physical.diff`, proof `physical-proof.json`, complete live before/final inventories, source.diff, replay.txt, original.log/full.log, review.json and issue snapshots in same directory. Board recovery would require renewed physical custody+directory flock and current-byte comparison to the after hash; never blindly restore over future owners. No live script rollback needed because no deployment occurred.

Protected anchor hashes unchanged: AGENTS.md `1bf4286b3485de018759a5b9b8e19d74beb6385844b56315f22f71b7819897fd`; StorageArchitecture.md `6bebe49511f310c0f97b000f2f5a6c8db6890bb77fd312b9ec47e47c900a5a22`. Their original dirty edits preserved. Final original physical validator readback PASS32. Source result adds only this named Markdown beyond the two-file candidate.

Dashboard ingestion/audit/published dashboard/Drive synchronization **NOT VERIFIED**, outstanding operational follow-up to authorised custodians. No generated dashboard state edited. Overall Manager must resolve source environment.sh permission within a refreshed broader ceiling and coordinate future physical reservation; no new native gate or product acceptance follows from this result.
