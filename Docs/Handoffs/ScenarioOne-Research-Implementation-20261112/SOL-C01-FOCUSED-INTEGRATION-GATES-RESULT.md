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


## Attempt 2 — HANDED-OFF (2026-11-12)

GPT-6.1 Sol, **Medium**, T3 Code / Codex harness, assigned ChatGPT pool A. Attempt 1 above is retained as history; its next action is superseded by the explicit retry instruction. No source/gameplay changes, test weakening, merge, build, registry or active-tasks edits.

| Check | Verdict | Evidence class |
|---|---|---|
| Same authentic saved-world identity | PASS, bounded year-zero fixture | Actual Editor execution: candidate and baseline both `A4C22867F535328B` from identical bytes. |
| Reference Future milestones | PASS comparison | Actual CurrentWorldHash/WorldHash outputs identical at years 0/20/50/100; no archive edits or rebaseline. Not a new full continuation/Legacy40 geometry qualification. |
| Nursery warning visible | FAIL on BOTH; pre-existing verification debt | Identical original overlap predicate false under nonblack 1280×720 conditions. No candidate-only regression. Expectations unchanged. |
| External wind limitation | PASS | Packet branch note unchanged from `2803d3bd`; explicitly states default Scenario One does not simulate wind damage. Documentary evidence only. |
| Optional Oak/Beech first reached | NOT RUN | No genuine success-history fixture; no invented achievements. |
| Final safety | PASS with lease timing caveat below | Exact source returned clean, host inventory unchanged, no Editor, lock free, disk above floor. |

### Fixture and isolation

Authentic disposable fixture generated by `ForestSaveController.CaptureData()` in the isolated candidate Editor, serialized with JsonUtility. No host saves used. SHA-256 `ebc010c102cd8614ebec6f2c712e6da643f6225696500e34562ac536265c9a68`; format24, ecologicalYear0, RNG1, regeneration2, growth2, storm0, Enlarged80 geometry1; 1344 tree records, 6 buildables, zero serialized cell records. This is limited fresh-world evidence, without management history or annual advance. Mature Reference Future milestones supplement it but are separate fixtures. No claim of a managed-save continuation test.

`diff -u run391.py run393.py` confirmed TASK plus the HOME override as the only differences. Removed override; launcher is now byte-identical to #391 after TASK substitution. Real HOME `/home/jer` preserved; XDG_CONFIG_HOME isolated. Before scene load diagnostic guarded and recorded Application.persistentDataPath inside each run's `config/unity3d/DefaultCompany/CCF`; guard exits3 if outside. No terms dialog appeared; none accepted. Temporary copied licences removed by launcher.

### Nursery scope and diagnostic corrections

Each commit's own Tools/Verification/ScenarioOneTeachingCopyVerification.cs was staged (see source SHA receipts); they differ in copy pins. Only the nursery predicate was extracted for execution. Reproduced its preceding real purchase of 25 saplings and cash threshold, Work Plan refresh, ScrollTo and settle sequence; no forced unlock or expectation change. Added capture sanity and rectangle logging. Candidate3 and baseline each had 921600/921600 nonblack pixels, normal Editor exit0, no reaping. Launcher PASS means diagnostic completed, **not** nursery PASS.

Initial candidate diagnostic omitted the earlier purchase prerequisite and threw at ScrollTo(null), then exited normally through window close (exit0, launcher FAIL/missing token). Candidate2 safely recorded the missing element but is NOT regression evidence. Corrected candidate3 and baseline are the comparable results. All preliminary logs retained. Unity also emitted startup ArgumentOutOfRangeException/UI renderer diagnostics; no claim of an error-free broader suite.

Collapsed nursery content leaves warning width/height zero despite display Flex. This explains the overlap failure without changing production or verifier expectations. Original test would still fail.

Candidate3 actual log:
```
C393_HASH saved=A4C22867F535328B year=0 version=24
C393_REFERENCE year=0 current=321AFE4EC28E90B0 world=ECD870EB5170D1A5
C393_REFERENCE year=20 current=360C2B9EFFB8CBB3 world=89A8DC4ECC99EDFA
C393_REFERENCE year=50 current=F0387C1BE0A9DA80 world=F640255094DB3945
C393_REFERENCE year=100 current=086A3EF3B75DE6F0 world=45919D63C5EEAAD1
C393_SANITY positive=921600 total=921600 size=1280x720
C393_NURSERY nursery warning visible=False warning=(x:240.00, y:776.25, width:0.00, height:0.00) viewport=(x:225.00, y:371.25, width:1131.25, height:405.00) display=Flex
C393_DONE
```
Baseline actual log (identical):
```
C393_HASH saved=A4C22867F535328B year=0 version=24
C393_REFERENCE year=0 current=321AFE4EC28E90B0 world=ECD870EB5170D1A5
C393_REFERENCE year=20 current=360C2B9EFFB8CBB3 world=89A8DC4ECC99EDFA
C393_REFERENCE year=50 current=F0387C1BE0A9DA80 world=F640255094DB3945
C393_REFERENCE year=100 current=086A3EF3B75DE6F0 world=45919D63C5EEAAD1
C393_SANITY positive=921600 total=921600 size=1280x720
C393_NURSERY nursery warning visible=False warning=(x:240.00, y:776.25, width:0.00, height:0.00) viewport=(x:225.00, y:371.25, width:1131.25, height:405.00) display=Flex
C393_DONE
```

### Commands, custody and limits

Full command argument arrays, corrected launcher/diagnostic and receipts are committed in [evidence-393-attempt2](evidence-393-attempt2/). Reproduction launches:
```bash
python3 -I -B /media/jer/Files/CCF/Verification/CCF-393-IntegrationGates-20261112/attempt2/launch.py candidate3
python3 -I -B /media/jer/Files/CCF/Verification/CCF-393-IntegrationGates-20261112/attempt2/launch.py baseline
```
The helper checks the 16106127360-byte floor before every launch and supplies exact lane, files, output, fixture, method and token arguments. Warm Library reused; baseline was detached checkout `b62446caf40fc2ac939d55a6bea6e201f41de700` in the same lane, no new worktree/Library. Candidate lease `b094975d9ddf4b369595a32bcd77b6ef`, baseline lease `0b267aa2edb44da1ae3a7566f0c54461`; acquire owner `GPT-6.1 Sol Medium`, task `CCF-393-FOCUSED-GATES`, exact lane/HEAD, required notes. Baseline branch argument initially DETACHED was rejected; correct argument HEAD acquired normally. All runs checked lock exact HEAD before staging.

Caveat: baseline lease was released after C393_DONE while PID190951 was still finishing shutdown; no subsequent launch or checkout occurred until launcher confirmed normal exit and lock status showed no Editor. Release was not forced. This early release is an operational timing defect, not an exclusive lease held through complete process exit claim.

Editor-only changes to .vscode/settings.json and three Forb materials were preserved as evidence patches and restored to their pre-run tracked bytes after each session; initial lane was clean. No unowned dirty files were removed. Disposable sources/metas removed, leftover lists empty. No user #382 lane writes. Primary's unrelated edits preserved. No Windows/Player build, packages, deletion of assets or save edits. The 27 licensed ignored asset inventory was validated in attempt1; not requalified as gameplay evidence here.

Final state:
```json
{
  "free_bytes": 17659723776,
  "host_files_unchanged": 37,
  "head": "a314d308315b5894263d30461a1e8b52ca0f3605",
  "status": "",
  "lock": {
    "status": "free",
    "metadata": null,
    "unity_editor_pids": []
  }
}
```
All same 37 host inventory SHA-256 values unchanged from attempt1 and attempt2 before/after. Disk observed above floor at each launch (receipts); observed intermediate 17653956608 bytes, no continuous disk-monitoring claim. Previously accepted 157/5 mutation evidence remains inherited and pinned to its historical candidates, not rerun. Legacy40 geometry and broader gameplay/save-load tests NOT RUN in this task.

Recommendation: keep ResearchCopyQual391 warm/registered until Manager integration review and any follow-up nursery debt decision. No registry mutation. Required focused comparisons completed; no integration verdict. Manager #200 owns integration. Dashboard ingestion/audit synchronization NOT VERIFIED and remains Manager operational follow-up.
