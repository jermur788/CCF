# #391 Unity qualification — BLOCKED

Operator: GPT-6.1 Sol, medium effort, T3 Code/Codex. Test worker only; no subagents.

Candidate: `868643a1311ad79a759dbe18d4353f1b928ce2a7`, source commit `99af87ee`, feature branch `feature/scenario-one-research-copy-v1-20261112`. Requested baseline: `b62446caf40fc2ac939d55a6bea6e201f41de700`, game source `2aaacad68ed186dbe14531f31ce5bb3a87811441`. No integration or production changes.

**Verdict: BLOCKED.** The required machine-wide lease could not be acquired because the newly authorised lane is not configured in the operational lock tool. User instruction requires stopping at a safety failure. No attempt to register a lane, bypass configuration, override a lock or start an Editor followed this rejection. No Unity result is claimed.

## Safety observations and commands

Commands below were executed on 2026-11-12. Inspection commands succeeded unless otherwise stated. Shell commands were read-only except the explicitly authorised worktree, evidence and report creation.

- `git status --short` in Primary: existing modifications to `AGENTS.md` and `Docs/Operations/StorageArchitecture.md`; preserved.
- `git show 868643a1:Docs/Project/AgentWorkflow.md` and `git show 868643a1:AGENTS.md`: read.
- `df -B1 /media/jer/ZX20`: available **26,788,233,216 bytes** before worktree creation (24.95 GiB), above 15 GiB reserve and sufficient for expected 6–7 GB import.
- `findmnt /media/jer/ZX20`: `/dev/sdc2`, exFAT, mounted read/write. UUID reported by the operational storage check: `18D6-896E`.
- `ps -eo pid,args | rg '[/]Editor/Unity|[U]nity -'`: no Editor match.
- `git worktree list`: existing protected lane and Files author checkout identified, no mutation to protected lanes.
- Inspected #381 durable `run_gate.py`, `command.py`, continuation `sessions.py`, `PathGuard381.cs`, custody manifest and readiness report at `b62446ca`. The launcher names `/media/jer/ZX20/CCF-Ops/scripts/unity-lock`.
- Initial `python3 -B CCF-Ops/scripts/unity-lock status --json` from Primary used its older tool and rejected mount identity (`/dev/sdc2`, UUID above). This was an operator path mistake, corrected before any launch; no configuration changed.
- Correct `python3 -B /media/jer/ZX20/CCF-Ops/scripts/unity-lock status --json`: **FREE**, metadata null, Editor PID list empty.
- `git -C /media/jer/Files/CCF/Operations/CCF-S1-ResearchCopy-V1-Sol status --short`: empty; `rev-parse HEAD`: exact candidate.
- `git worktree add -b qualify/scenario-one-research-copy-v1-20261112 /media/jer/ZX20/CCF-S1-ResearchCopy-V1-Qual 868643a1311ad79a759dbe18d4353f1b928ce2a7`: exit0, new lane at candidate.

Required lease command:

```bash
python3 -B /media/jer/ZX20/CCF-Ops/scripts/unity-lock acquire --owner Sol391 --task-id CCF-391-UI-QUALIFICATION --lane /media/jer/ZX20/CCF-S1-ResearchCopy-V1-Qual --branch qualify/scenario-one-research-copy-v1-20261112 --head 868643a1311ad79a759dbe18d4353f1b928ce2a7 --eta 2026-11-12T23:59:00+00:00 --notes 'User-authorised issue391 bounded copy verification fresh lane' --json
```

**Exit2:** `ERROR: lane is not configured: /media/jer/ZX20/CCF-S1-ResearchCopy-V1-Qual`.

No lease ID exists. Final correct-tool `status --json`: **FREE**, metadata null, `unity_editor_pids: []`. No release needed or attempted. Operational lane registration is required before qualification can resume; this worker did not modify operational configuration.

Final `df -B1 /media/jer/ZX20`: **24,676,139,008 bytes** available (22.98 GiB). No import ran; after-import disk measurement is NOT RUN. Fresh lane `git status --porcelain=v1 -uall`: empty. Lane remains at candidate; no Library copied, created or deleted.

## Qualification matrix

| Check / changed panel | 1280×720 | 1920×1080 | Evidence / reading |
|---|---|---|---|
| Editor import/compile, 0 errors | NOT RUN | NOT RUN | Lease blocked before first Editor |
| Help: competition/Share paragraph | NOT VERIFIED | NOT VERIFIED | No image |
| Help: wind limitation | NOT VERIFIED | NOT VERIFIED | No image |
| Help: canopy/gap/open | NOT VERIFIED | NOT VERIFIED | No image |
| Learning Objectives topic 2 | NOT VERIFIED | NOT VERIFIED | No image |
| Learning Objectives map.regeneration step | NOT VERIFIED | NOT VERIFIED | No image |
| Teaching Lesson why it matters | NOT VERIFIED | NOT VERIFIED | No image |
| Objectives Oak/Beech Plant, with species present | NOT VERIFIED | NOT VERIFIED | No image |
| Objectives Areas with young trees | NOT VERIFIED | NOT VERIFIED | No image |
| Work Plan clearance natural young-tree groups | NOT VERIFIED | NOT VERIFIED | No image |
| Annual Review removal jobs completed | NOT VERIFIED | NOT VERIFIED | No authentic state reached |
| Annual Review Objective first reached | NOT VERIFIED | NOT VERIFIED | No authentic state reached |

`ScenarioOneTeachingCopyVerification.cs`: **NOT RUN**. `ScenarioOneCompletion/P3/WorkPlanResidualVerification.cs`: **NOT RUN**. AC-6 baseline fixture load / CurrentWorldHash comparison: **NOT RUN**, no candidate value. Reference Future / Legacy40 anchor gate: **NOT RUN**. No rebaseline. AC-7 readability: **NOT VERIFIED**. No screenshot exists; no screenshot directory populated. Harness failure is not a product PASS; here no harness ran.

## Custody and limits

Durable evidence: `/media/jer/Files/CCF/Verification/CCF-391-ResearchCopy-Qual-20261112/` (`host-custody-before.json`, `host-custody-after.json`, `final-lock.json`). Hashes below are two consecutive closing snapshots, not a claim of an Editor-bracketing experiment: no Editor ever started. All 37 enumerated host files match. No save/Prefs write occurred. Licensed ignored assets were **not provisioned** because the lease safety check stopped work first. No licence copied; disposable process save/Prefs paths NOT VERIFIED because no process launched. No harness staged, no generated harness metadata, no Editor import side effects to restore, no Editor killed. Protected source lane, U3, frozen Reference, Legacy40 and other worktrees were not modified. Author and both-review full reading/spec acceptance investigation was not completed before the safety stop; no conformance assessment claimed.

Publication is documentation only from exact candidate on `report/scenario-one-research-copy-v1-unity-qual-20261112`; author worktree is returned to its original feature branch clean. No GitHub issue, active-tasks or dashboard write. Dashboard synchronization NOT VERIFIED.

Host save/Prefs closing custody (SHA-256; before = after):

| Path | Before = after |
|---|---|
| `/home/jer/.config/unity3d/DefaultCompany/CCF/forest-save.json.bak` | `c0169790c8bba169ffd27c486e6a83bf67d6c938e170da58d42031bbcdf862a3` |
| `/home/jer/.config/unity3d/DefaultCompany/CCF/prefs` | `c16ff426930f16c4c74b52a2b0ecd259142599943484b44de1164841f560d71e` |
| `/home/jer/.config/unity3d/DefaultCompany/CCF/save-backup-20261104/forest-save.json` | `7effe9b72c741d372efbb8acc1115d7be57a3514300da1d3d2de42801e565075` |
| `/home/jer/.var/app/com.unity.UnityHub/config/unity3d/DefaultCompany/CCF/forest-save.json` | `ccb01a0125d4d55174b073cc7a0b45221b4e9e2f17bce6f8f0c4820914c5d303` |
| `/home/jer/.var/app/com.unity.UnityHub/config/unity3d/DefaultCompany/CCF/prefs` | `bc21c9ae8cac040981b99c6e989546343ae1443ab45d9cb34de79061b034e708` |
| `/home/jer/.var/app/com.unity.UnityHub/config/unity3d/DefaultCompany/My project/prefs` | `ec704d4085921e975a254821be98a99c4b4284ac8185e212258d15be6b7aa47b` |
| `/home/jer/.var/app/com.unity.UnityHub/config/unity3d/DefaultCompany/Setup Guide In-Editor Tutorial/prefs` | `176d7a3fc2179a959633b91ac3cd7268715a418d6082dac49d070491f0541984` |
| `/home/jer/.var/app/com.unity.UnityHub/config/unity3d/prefs` | `4c6dcd791400d33826c1f2761a34b59778f786bc334d78b816c66d7d16e50154` |
