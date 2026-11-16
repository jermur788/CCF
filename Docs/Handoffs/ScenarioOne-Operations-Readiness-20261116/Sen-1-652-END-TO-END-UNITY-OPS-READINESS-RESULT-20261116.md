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
