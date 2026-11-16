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
