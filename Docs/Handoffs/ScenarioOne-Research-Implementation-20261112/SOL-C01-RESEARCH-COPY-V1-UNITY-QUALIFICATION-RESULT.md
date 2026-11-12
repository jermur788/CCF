# #391 Unity qualification — PARTIAL

Operator: GPT-6.1 Sol, medium effort, T3 Code/Codex; test worker, no subagents. No production edits, build, merge, issue or active-tasks changes.

Candidate `868643a1311ad79a759dbe18d4353f1b928ce2a7`; baseline `b62446caf40fc2ac939d55a6bea6e201f41de700`, game source `2aaacad68ed186dbe14531f31ce5bb3a87811441`. Unity 6000.6.0f1. Lane `/media/jer/ZX20/CCF-S1-ResearchCopy-V1-Qual`, branch `qualify/scenario-one-research-copy-v1-20261112`, remains at exact candidate.

**PARTIAL.** Real Editor fresh import/script compilation reached Play Mode with zero `error CS` compiler diagnostics. Teaching verifier failed `System.Exception: nursery warning visible`. All five captured images were individually inspected and are black. No readability PASS is claimed from render markers. This is an execution/rendering qualification failure; no text-patch product defect established. Other requested checks below remain unverified. Bounded attempt ended rather than rerunning an impaired render pipeline or expanding into full qualification.

## Attempt 1

Previous report branch attempt stopped BLOCKED before Unity: operational tool rejected unregistered lane. No lease or Editor existed. Orchestrator subsequently registered ResearchCopyQual391; this worker did not edit operational config. Original attempt report remains in the parent report commit and durable `qualification-report.md`.

## Attempt 2: preparation, commands and results

Durable originals and receipts: `/media/jer/Files/CCF/Verification/CCF-391-ResearchCopy-Qual-20261112/`. Commands below ran successfully except explicitly stated failures. No speculative result is PASS.

- `git -C /media/jer/Files/CCF-Primary fetch origin` and requested `git show origin/report/scenario-one-research-copy-v1-unity-qual-20261112:Docs/Handoffs/ScenarioOne-Research-Implementation-20261112/SOL-C01-RESEARCH-COPY-V1-UNITY-QUALIFICATION-RESULT.md`: exit0, read first. Project workflow/context and live active tasks inspected. Primary has unrelated AGENTS.md and StorageArchitecture.md edits, preserved.
- Read #381 readiness report via `git show b62446ca:Docs/Handoffs/ScenarioOne-Playtest-Finish-20261111/SOL-381-SCENARIO-ONE-PLAYTEST-READINESS-RESULT.md`; inspected its `run_gate.py`, `command.py`, continuation `sessions.py`, `next_phases.py`, `PathGuard381.cs`, custody and logs. Candidate verifier sources inspected before staging.
- Licensed provisioning: enumerated #381 continuation custody manifest, selected its ignored asset paths in protected UIQual lane, copied **27 files** with metadata relationships into new lane using `shutil.copy2`; verified source/destination SHA256 equality for each. Protected lane was read only. Exact paths/hashes: `provisioned.json`. No Library copied.
- `df -B1 /media/jer/ZX20`: before **24676139008 bytes (22.98 GiB)**; monitored during import, never observed below floor. After **17661034496 bytes (16.45 GiB)**. Approximately 6.53 GiB consumed. Mount `/dev/sdc2`, exFAT, operational tool accepted. Library retained.
- Correct operational `python3 -B /media/jer/ZX20/CCF-Ops/scripts/unity-lock status --json`: FREE/no Editor before acquisition. Launcher separately checks exact HEAD, clean status and no Editor before staging.

```bash
python3 -B /media/jer/ZX20/CCF-Ops/scripts/unity-lock acquire --owner Sol391 --task-id CCF-391-UI-QUALIFICATION --lane /media/jer/ZX20/CCF-S1-ResearchCopy-V1-Qual --branch qualify/scenario-one-research-copy-v1-20261112 --head 868643a1311ad79a759dbe18d4353f1b928ce2a7 --eta 2026-11-13T03:00:00+00:00 --notes 'bounded copy qualification' --json
```

Exit0, lease **c72e3e0fce27404b88df38b3f3daa146**. No override/force release.

Launcher is evidence-only copy of #381 launcher with task ID changed to CCF-391-UI-QUALIFICATION; PathGuard copy changes only required evidence-root predicate. No historical gate assertion edited.

```bash
CCF_GATE_LANE=/media/jer/ZX20/CCF-S1-ResearchCopy-V1-Qual python3 -B /media/jer/Files/CCF/Verification/CCF-391-ResearchCopy-Qual-20261112/run391.py --name Teaching --method ScenarioOneTeachingCopyVerification.Begin --files ScenarioOneTeachingCopyVerification.cs --ext-files /media/jer/Files/CCF/Verification/CCF-391-ResearchCopy-Qual-20261112/PathGuard381.cs --out /media/jer/Files/CCF/Verification/CCF-391-ResearchCopy-Qual-20261112/teaching --display :89 --timeout 1800 --pass-token S1A_TEACHING_VERIFY_PASS --fail-token S1A_TEACHING_VERIFY_FAIL
```

Runner exit1, 671 seconds; Editor exit -9 after controlled shutdown hang handling. Actual Editor command: Unity path specified above, `-projectPath <lane> -job-worker-count 2 -logFile <root>/teaching/Teaching.log -executeMethod ScenarioOneTeachingCopyVerification.Begin`. No batchmode or Player build. `CCF_TEST_DIRECT_ENTRY=1`, TMPDIR=/tmp/, private Xephyr :89, fresh XDG_CONFIG_HOME; operational environment.sh not sourced. Freshest installed licence copied by inspected launcher and removed after exit (`teaching/licence-copy-removed.json`, SHA prefixes only). No licence retained in evidence.

Log has S1A_NURSERY_PASS and five S1A_RENDERED_PASS markers, followed by S1A_TEACHING_VERIFY_FAIL `nursery warning visible`. UI Toolkit render-chain exceptions also retained. Those markers do not establish usable captures. Gate uses historical cash mutation for nursery testing; no resulting injected state is credited as authentic progression.

Editor PID **113621** logged Cleanup mono, compiler/physics/input/licensing shutdown, but remained running. `ps -p 113621` confirmed it; `kill -KILL 113621` exit0. Only recorded owned PID killed. No Scene Backup dialog encountered. Runner then removed both disposable scripts and generated metas. Known import diff preserved in `teaching-import.patch`; each changed path checked against .vscode/settings.json or SectionFive Materials/*.mat whitelist, then restored byte-exact from clean HEAD via `git show HEAD:<path>`. Final `git status --porcelain -uall` empty. No git clean.

```bash
python3 -B /media/jer/ZX20/CCF-Ops/scripts/unity-lock release --owner Sol391 --task-id CCF-391-UI-QUALIFICATION --lock-id c72e3e0fce27404b88df38b3f3daa146 --reason 'teaching failed; shutdown hung recorded PID killed' --json
```

Exit0 normal release; final status FREE, metadata null, unity_editor_pids empty (`attempt2-final-lock.json`).

## Qualification matrix

| Check | 1280×720 | 1920×1080 | Reading / cause |
|---|---|---|---|
| Import/compile | PASS (Editor) | same compilation | Real Play Mode reached, zero C# compiler errors; runtime render errors retained |
| Help competition/Share | NOT VERIFIED | NOT RUN | s1a-help-1280.png black |
| Help wind limitation | NOT VERIFIED | NOT RUN | Same black image |
| Help canopy/gap/open | NOT VERIFIED | NOT RUN | Same black image |
| Learning curriculum topic 2 | NOT VERIFIED | NOT RUN | s1a-learning-1280.png black; harness selects topic 1 |
| Learning map.regeneration | NOT VERIFIED | NOT RUN | No requested step capture |
| Teaching why it matters | NOT VERIFIED | NOT RUN | No readable capture |
| Objectives Oak/Beech lines | NOT VERIFIED | NOT RUN | s1a-objectives-1280.png black |
| Objectives Areas with young trees | NOT VERIFIED | NOT RUN | Same black image |
| Work Plan natural young-tree groups | NOT VERIFIED | NOT RUN | s1a-workplan-1280.png black; clearance state not reached |
| Annual removal jobs completed | NOT VERIFIED | NOT RUN | Gate stopped before annual review |
| Annual Objective first reached | NOT VERIFIED | NOT RUN | Authentic reaching state not witnessed |

Also inspected s1a-inspection-1280.png: black. Every captured image is unusable; no visibility/overlap/cut-off judgement possible. All originals retained, five modest PNG copies embedded below.

- ScenarioOneTeachingCopyVerification: **FAIL**, nursery-warning assertion, runtime rendering faults; not a product PASS.
- WorkPlanResidualVerification: **NOT RUN**, bounded attempt ended after failed teaching/capture route. Requires independent lease/session.
- Baseline fixture world identity: **NOT RUN**, no candidate value. #381 loop records WorldHash `014F5F543BC0B328` and save/load equality, but no retained loop forest-save fixture found; only first-run save found, without establishing a matching recorded identity. No substitution or rebaseline.
- Frozen Reference/Legacy40: **NOT RUN**. Cheap accepted `ScenarioReferenceVerification.Begin` route identified in #381 next_phases.py, not executed in this attempt. Historical anchors are evidence only.

## Save/Prefs custody and remaining work

Actual guard output: persistent=`<root>/teaching/config/unity3d/DefaultCompany/CCF`; F5/F9=`<persistent>/forest-save.json`; config=`<root>/teaching/config`. Guard wrote/saved isolated PlayerPrefs and verified `<persistent>/prefs` exists. Full actual paths in teaching/evidence/process-paths.txt. No host slot used.

Before first Editor/after last: enumerated host files containing prefs or forest-save under both host Unity config roots. All **9 files unchanged**, SHA256 below. No protected worktree writes; no U3, frozen Reference, Legacy40 mutation. Library kept; no Editor; final FREE. Publication documentation only, feature branch receives no commit. Dashboard synchronization NOT VERIFIED; issues and active-tasks intentionally untouched per direct packet.

| Host file | Before = after SHA256 |
|---|---|
| `/home/jer/.config/unity3d/DefaultCompany/CCF/prefs` | `c16ff426930f16c4c74b52a2b0ecd259142599943484b44de1164841f560d71e` |
| `/home/jer/.config/unity3d/DefaultCompany/CCF/forest-save.json.bak` | `c0169790c8bba169ffd27c486e6a83bf67d6c938e170da58d42031bbcdf862a3` |
| `/home/jer/.config/unity3d/DefaultCompany/CCF/prefs.bak-20261017-before-resolution-fix` | `2af15cebc666fa5c5bccd103c8dd4f6e367653c060e5c710f9c20b53a23b5307` |
| `/home/jer/.config/unity3d/DefaultCompany/CCF/save-backup-20261104/forest-save.json` | `7effe9b72c741d372efbb8acc1115d7be57a3514300da1d3d2de42801e565075` |
| `/home/jer/.var/app/com.unity.UnityHub/config/unity3d/prefs` | `4c6dcd791400d33826c1f2761a34b59778f786bc334d78b816c66d7d16e50154` |
| `/home/jer/.var/app/com.unity.UnityHub/config/unity3d/DefaultCompany/CCF/prefs` | `bc21c9ae8cac040981b99c6e989546343ae1443ab45d9cb34de79061b034e708` |
| `/home/jer/.var/app/com.unity.UnityHub/config/unity3d/DefaultCompany/CCF/forest-save.json` | `ccb01a0125d4d55174b073cc7a0b45221b4e9e2f17bce6f8f0c4820914c5d303` |
| `/home/jer/.var/app/com.unity.UnityHub/config/unity3d/DefaultCompany/Setup Guide In-Editor Tutorial/prefs` | `176d7a3fc2179a959633b91ac3cd7268715a418d6082dac49d070491f0541984` |
| `/home/jer/.var/app/com.unity.UnityHub/config/unity3d/DefaultCompany/My project/prefs` | `ec704d4085921e975a254821be98a99c4b4284ac8185e212258d15be6b7aa47b` |

![Inspected: s1a-help-1280.png, black / unusable](evidence-391-unity/s1a-help-1280.png)

![Inspected: s1a-inspection-1280.png, black / unusable](evidence-391-unity/s1a-inspection-1280.png)

![Inspected: s1a-learning-1280.png, black / unusable](evidence-391-unity/s1a-learning-1280.png)

![Inspected: s1a-objectives-1280.png, black / unusable](evidence-391-unity/s1a-objectives-1280.png)

![Inspected: s1a-workplan-1280.png, black / unusable](evidence-391-unity/s1a-workplan-1280.png)
