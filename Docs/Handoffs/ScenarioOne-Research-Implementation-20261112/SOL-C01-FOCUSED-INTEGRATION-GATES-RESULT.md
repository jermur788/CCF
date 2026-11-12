# C-01 focused integration gates — BLOCKED

Worker: GPT-6.1 Sol, **Medium**, T3 Code / Codex harness, ChatGPT pool A (assigned pool; billing not independently inspected). Task #393, 2026-11-12. Verification-only; no integration verdict.

Candidate `a314d308315b5894263d30461a1e8b52ca0f3605`; intended baseline `b62446caf40fc2ac939d55a6bea6e201f41de700`. Report branch `docs/sol-c01-focused-gates-393-20261112`, sole report worktree `/media/jer/Files/CCF/Operations/CCF-393-Report`, based on fetched and remote-readback-verified packet head `2803d3bd15d0abe146f0a590b2c2766690b7d939`.

| Check | Verdict | Evidence class / reason |
|---|---|---|
| Same saved-world CurrentWorldHash | NOT RUN / NOT VERIFIED, BLOCKED | No fixture produced, no hash emitted. No baseline checkout/run. No equality claim from source. |
| Nursery warning baseline/candidate classification | NOT RUN / NOT VERIFIED | Editor never loaded project; neither assertion ran. Cannot classify pre-existing debt or regression. |
| External wind limitation | PASS | Documentary/source inspection: packet-branch EXTERNAL-TEST-LIMITATIONS-C01.md explicitly bolds “The default Scenario One does not simulate wind damage.” Candidate ScenarioOneManager.cs:475 selects StormModel.None. This is not runtime wind verification. |
| Optional Oak/Beech first reached | NOT RUN | No genuine success-history fixture used. |
| Final safety | PASS for observed local custody | Source unchanged, no live Editor, normal lease released, host inventory unchanged, disk above floor. Runtime save isolation remains NOT VERIFIED because executeMethod never ran. |

## Blocking evidence and next action

Unity 6000.6.0f1 launched under an isolated HOME and XDG_CONFIG_HOME. Licence entitlement resolved, but a **Unity Editor Software Terms** dialog (last updated June 30, 2026) required a legal acceptance before project load. The worker did not accept on the user's behalf. Decline was clicked; exact process 182720 exited normally with code 1 after 44 seconds. No kill, no retry, no test failure or product regression inferred. The launch driver's FAIL means missing completion token, not a failed gameplay assertion. Local screenshot `terms.png` is retained in the evidence root; no UI screenshot is presented as game evidence.

Next action: user/Manager supplies or authorises a test HOME with the Unity terms already legitimately accepted, then rerun this same bounded packet on the retained warm lane. Do not copy host saves or Prefs. Create an authentic disposable world in that isolated environment, record format/year/model and byte hash, and hash exactly those bytes on both exact source heads; then run the original nursery overlap predicate under comparable nonblack conditions. Frozen archive/Legacy40 comparisons and runtime paths still require verification. No save format/year/model or before/after world hashes exist from this attempt.

## Commands and reproducibility

Full disposable runner and diagnostic source are in [evidence-393](evidence-393/). Diagnostic was prepared only outside the repo; it was not compiled/executed. It uses CaptureData to generate a fresh year-zero fixture if absent, CurrentWorldHash for the saved data and frozen archive milestones, and the original `warning.worldBound.Overlaps(scroll.contentViewport.worldBound)` predicate. Its visual fixture remains unqualified: no assertion equivalence or test success is claimed.

```bash
git fetch origin docs/manager-c01-integration-gates-20261112-v1
git show origin/docs/manager-c01-integration-gates-20261112-v1:Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/MANAGER-C01-FOCUSED-INTEGRATION-GATES-PACKET.md
git ls-remote origin refs/heads/docs/manager-c01-integration-gates-20261112-v1
git worktree add -b docs/sol-c01-focused-gates-393-20261112 /media/jer/Files/CCF/Operations/CCF-393-Report 2803d3bd15d0abe146f0a590b2c2766690b7d939
python3 -B /media/jer/ZX20/CCF-Ops/scripts/unity-lock acquire --owner 'GPT-6.1 Sol Medium' --task-id CCF-393-FOCUSED-GATES --lane /media/jer/ZX20/CCF-S1-ResearchCopy-V1-Qual --branch qualify/scenario-one-research-copy-v1-20261112 --head a314d308315b5894263d30461a1e8b52ca0f3605 --eta 2026-11-12T07:00:00Z --notes 'User authorised #393 focused isolated verification only' --json
CCF_GATE_LANE=/media/jer/ZX20/CCF-S1-ResearchCopy-V1-Qual python3 -I -B /media/jer/Files/CCF/Verification/CCF-393-IntegrationGates-20261112/run393.py --name focused --method Ccf393.Begin --files ScenarioOneTeachingCopyVerification.cs --ext-files /media/jer/Files/CCF/Verification/CCF-393-IntegrationGates-20261112/Ccf393.cs --out /media/jer/Files/CCF/Verification/CCF-393-IntegrationGates-20261112/candidate --env CCF393_FIXTURE=/media/jer/Files/CCF/Verification/CCF-393-IntegrationGates-20261112/fixture.json --pass-token C393_DONE --fail-token C393_BLACK --timeout 300
python3 -B /media/jer/ZX20/CCF-Ops/scripts/unity-lock release --owner 'GPT-6.1 Sol Medium' --task-id CCF-393-FOCUSED-GATES --lock-id fbd18ecb751c4d0c83556e3a073cce8d --reason '#393 blocked at legal terms; Editor exited and staged files removed' --json
```

Initial acquire omitted required --notes and was rejected without acquiring; initial release omitted matching owner/task and was rejected; corrected commands above succeeded. No force used.

## Custody and scope

Mount verified `/dev/sdc2`, exFAT, matching registered ResearchCopyQual391 lane path. Existing 27 ignored licensed asset files matched the #391 attempt4 hash manifest. Remaining ignored entries were generated projects, Library, Logs and UserSettings; none cleaned or manually edited. Existing warm Library retained. Tracked/untracked status was empty before/after. Disposable Ccf393.cs and ScenarioOneTeachingCopyVerification.cs plus their generated .meta paths were removed by the launcher, result leftover list empty. No gameplay/source/test edits, test weakening, build, merge, packages or registry changes. Candidate stayed on `qualify/scenario-one-research-copy-v1-20261112` throughout; baseline never checked out.

Machine-wide lease `fbd18ecb751c4d0c83556e3a073cce8d` acquired and released normally; final JSON records FREE and no Editor. Initial free bytes 17659592704, during launch 17659199488, final 17659461632 versus floor 16106127360; observed minimum stayed above floor. No claim of continuous byte-level disk monitoring.

All 37 pre-existing host inventory files match before/after SHA-256, including host saves and Prefs; full receipts committed alongside. Host contents were read for hashing only, never used as fixtures. User #382 worktree was not written to. Primary's unrelated AGENTS.md and Docs/Operations/StorageArchitecture.md modifications preserved; no active-tasks edit. Previously accepted 157 passes / 5 mutation controls and #391 Unity evidence are inherited historical evidence pinned to their recorded candidates, not rerun or upgraded by this attempt.

Recommendation: **keep** ResearchCopyQual391 registration and warm lane pending these two unresolved gates. Manager alone may retire it after successful closure; worker made no registry mutation. Manager #200 retains integration authority. Dashboard sync / published audit: NOT VERIFIED; Manager must reconcile blocked #393 and verify dashboard ingestion. This report does not claim integration readiness.

Durable evidence root: `/media/jer/Files/CCF/Verification/CCF-393-IntegrationGates-20261112/`. No licence bytes committed; temporary licence copies removed by launcher.
