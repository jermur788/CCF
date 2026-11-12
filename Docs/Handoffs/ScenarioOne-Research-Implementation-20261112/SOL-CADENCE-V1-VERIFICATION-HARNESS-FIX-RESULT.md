# Claim #401 harness repair handoff — NOT UNITY VERIFIED

OpenAI GPT-6.1 Sol, medium effort, Codex harness in T3 Code; sole writer, no subagents.
Branch: feature/scenario-one-cadence-dual-advance-v1-20261112.
Worktree: /media/jer/Files/CCF/Worktrees/CCF-CadenceDualAdvance-Source-397.
Starting local and origin HEAD after fetch: 8c1efc744722943f5119d358e5d836b372e5e454; clean.
Last harness commit: f2f3e136d970e17f48444c91b6ad4b14d76ca7df.
Implementation on task branch only; Manager acceptance/integration remains pending.

## Finding dispositions

- F-1 ACCEPTED for bounded source repair, runtime NOT RUN: CadenceParity.cs:77–85 requires exactly N ticks and exact reassessment completion summary. RunStop:121–135 takes mandatory expectedK and expectedStopReason and asserts both. N=5 remains harness-only.
- F-2 ACCEPTED for bounded source repair, runtime NOT RUN: Inputs():20–26 captures all nine production input types, preserving false as well as true enabled states. Completion():27–35 reads existing private screen/review label, requires Review and exact latest-year range/reason. RunStop checks lock and full restoration. Cancel:149–152 expects "Cancelled between years.". RejectWrongStopReason:140–146 is a negative fixture: first proves the actual summary is readable/correct, then must reject wrong text. Removing the summary equality makes that fixture fail. WRITTEN / NOT RUN, not an executed mutation kill.
- F-3 ACCEPTED for bounded source repair, runtime NOT RUN: Run:55–65 opens Work Plan and refreshes teaching on both A legs, checks identical canonical JSON and hash before ticking; :76–80 compares every intermediate tick. B:88–95 still uses genuine UI acknowledgments, normalises only AnnualReviewAcknowledged and resulting event-ID shifts; no fabricated acknowledgment.
- F-4 disclosed: production four-year target means five-tick cap cannot normally bind.
- F-5 IMPLEMENTED / offline PASS: Check.cs:25–28 kills all four requested calculator mutants (actual scratch runs below).
- F-6 NOT WRITTEN / NOT RUN: deterministic TickSkip exception, OnDisable and manager/controller-loss fixtures were not added. These need a scoped disposable Unity runner. OnDisable intentionally restores without Review; manager loss cannot satisfy RunStop's live-manager summary oracle. No production hooks or widened scope.
- F-7/F-8 untouched, outside authorised scope.
- F-9 authentic Crop Tree death snapshot unavailable in this packet; README records actual mortality/disabled-manager/stale-sweep checks. Fixture preparation BLOCKED; runtime NOT RUN.
- F-10 authentic Legacy40 fixture eligibility NOT RUN. README requires genuine computed stage 7 and rejection otherwise; no teaching-credit fabrication.

## Changed files

- Tools/Verification/CadenceDualAdvance/CadenceParity.cs
- Tools/Verification/CadenceDualAdvance/OfflineLogic/Check.cs
- Tools/Verification/CadenceDualAdvance/README.md
- This result Markdown.

## Actual verification and limits

Evidence: /media/jer/Files/CCF/Verification/CadenceHarnessRepair-401-20261112T092000Z/

Commands ran from the assigned worktree. Scratch TMPDIR, DOTNET_CLI_HOME, NUGET_PACKAGES were set by verify.py to evidence subdirectories; MSBuild node reuse disabled. Exact reproducible verification driver is preserved below. It ran twice, including after the final harness edit, exit 0 both times; outputs below are final-run logs.

- git fetch origin: exit 0.
- git status --porcelain: exit 0, empty at start.
- git rev-parse HEAD origin/feature/scenario-one-cadence-dual-advance-v1-20261112: exit 0, both starting SHA above.
- python3 Tools/Verification/CadenceDualAdvance/run_offline.py: exit 0, 19 assertions.
- Four dotnet run --project scratch/<mutant>/check.csproj invocations: exit 134 each, assertion failures, not compilation failures.
- run_typecheck.py --mode prod with StandMapSource compat and combined calculator/harness replacements: exit 0, six baseline errors, no new errors. Editor guard stripped only in scratch copy.
- Same check with AdvanceYearTypo401 in scratch harness: exit 1, CS1061 seventh error.
- git diff --check: exit 0.
- git diff --stat 8c1efc74..HEAD -- Assets: exit 0, EMPTY. No production changes.
- git add and git commit of harness files: exit 0.

IMPORTANT: the requested real Unity 6000.6 DLL typecheck was NOT RUN. The review's actual documented command uses UnityEngine.Modules 2021.3.33 NuGet reference assemblies and Input/Painter stubs, not real 6000.6 DLLs. I reproduced that review check, with SceneHandle→int scratch compatibility. I did not access the prohibited ZX20 Editor to obtain DLLs. Therefore real-DLL binding remains an unmet verification requirement, not PASS.

No Unity Editor/process/lock, Player or Windows build, merge, save-slot access, Assets edit, active-tasks edit or ZX20 access was performed. All runtime parity, stop, cancellation, negative summary, full input restoration, mortality, Legacy40, frozen reference, rendered UI and save/reload gates remain NOT RUN. No runtime result is called PASS.

Working tree was limited to the three listed harness files before source commit; report is the only subsequent change. Final clean state and GitHub readback are checked after publishing. Dashboard synchronization is left to orchestrator #402; not verified here.

## Exact verification driver
```python
import os,pathlib,subprocess
root=pathlib.Path.cwd(); ev=pathlib.Path(__file__).parent
os.environ.update(TMPDIR=str(ev/'tmp'),DOTNET_CLI_HOME=str(ev/'dotnet'),NUGET_PACKAGES=str(ev/'nuget'),MSBUILDDISABLENODEREUSE='1')
for k in ['tmp','dotnet','nuget']: (ev/k).mkdir(exist_ok=True)
def run(name,args):
 r=subprocess.run(args,text=True,stdout=subprocess.PIPE,stderr=subprocess.STDOUT)
 (ev/(name+'.log')).write_text(r.stdout); print(name,r.returncode,r.stdout[-1800:]); return r
run('offline',['python3','Tools/Verification/CadenceDualAdvance/run_offline.py'])
calc=root/'Assets/ForestPrototype/ScenarioOne/ScenarioManagementCadence.cs'
check=root/'Tools/Verification/CadenceDualAdvance/OfflineLogic/Check.cs'
for name,old in [('year',' || item.year != order.resolvedYear'),('id','item.eventId <= 0 || '),('tree',' || string.IsNullOrEmpty(order.targetTreeId)'),('future',' || order.resolvedYear > current')]:
 d=ev/name;d.mkdir(exist_ok=True)
 (d/'Calculator.cs').write_text(calc.read_text().replace(old,''))
 (d/'check.csproj').write_text(f'<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net8.0</TargetFramework><EnableDefaultCompileItems>false</EnableDefaultCompileItems></PropertyGroup><ItemGroup><Compile Include="{d}/Calculator.cs"/><Compile Include="{check}"/></ItemGroup></Project>')
 r=run('mutant-'+name,['dotnet','run','--project',str(d/'check.csproj')])
 assert r.returncode!=0 and '!=' in r.stdout
compat=ev/'StandMapSource.compat.cs'
compat.write_text((root/'Assets/ForestPrototype/UI/Map/StandMapSource.cs').read_text().replace('SceneHandle','int'))
harness=(root/'Tools/Verification/CadenceDualAdvance/CadenceParity.cs').read_text().replace('#if UNITY_EDITOR','').replace('#endif','')
combined=ev/'CadenceWithHarness.cs'
# Using directives must precede declarations.
usings='\n'.join(x for x in harness.splitlines() if x.startswith('using '))
body='\n'.join(x for x in harness.splitlines() if not x.startswith('using '))
combined.write_text(usings+'\n'+calc.read_text()+'\n'+body)
args=['python3','Tools/Verification/CloudTypeCheck/run_typecheck.py','--mode','prod','--replace','Assets/ForestPrototype/UI/Map/StandMapSource.cs',str(compat),'--replace',str(calc.relative_to(root)),str(combined),'--out',str(ev/'typecheck')]
run('typecheck',args)
combined.write_text(combined.read_text().replace('ui.Manager.AdvanceYear()', 'ui.Manager.AdvanceYearTypo401()'))
args[-1]=str(ev/'typecheck-typo')
run('typecheck-typo',args)

```

### offline output

```text
CADENCE CALCULATOR PASS 19 assertions (minimal record stubs; no runtime parity claim)

```

### mutant-year output

```text
Unhandled exception. System.Exception: 13 != 15
   at Check.Equal(Int32 actual, Int32 expected) in /media/jer/Files/CCF/Worktrees/CCF-CadenceDualAdvance-Source-397/Tools/Verification/CadenceDualAdvance/OfflineLogic/Check.cs:line 11
   at Check.Main() in /media/jer/Files/CCF/Worktrees/CCF-CadenceDualAdvance-Source-397/Tools/Verification/CadenceDualAdvance/OfflineLogic/Check.cs:line 25

```

### mutant-id output

```text
Unhandled exception. System.Exception: 14 != 15
   at Check.Equal(Int32 actual, Int32 expected) in /media/jer/Files/CCF/Worktrees/CCF-CadenceDualAdvance-Source-397/Tools/Verification/CadenceDualAdvance/OfflineLogic/Check.cs:line 11
   at Check.Main() in /media/jer/Files/CCF/Worktrees/CCF-CadenceDualAdvance-Source-397/Tools/Verification/CadenceDualAdvance/OfflineLogic/Check.cs:line 26

```

### mutant-tree output

```text
Unhandled exception. System.Exception: 14 != 15
   at Check.Equal(Int32 actual, Int32 expected) in /media/jer/Files/CCF/Worktrees/CCF-CadenceDualAdvance-Source-397/Tools/Verification/CadenceDualAdvance/OfflineLogic/Check.cs:line 11
   at Check.Main() in /media/jer/Files/CCF/Worktrees/CCF-CadenceDualAdvance-Source-397/Tools/Verification/CadenceDualAdvance/OfflineLogic/Check.cs:line 27

```

### mutant-future output

```text
Unhandled exception. System.Exception: 16 != 15
   at Check.Equal(Int32 actual, Int32 expected) in /media/jer/Files/CCF/Worktrees/CCF-CadenceDualAdvance-Source-397/Tools/Verification/CadenceDualAdvance/OfflineLogic/Check.cs:line 11
   at Check.Main() in /media/jer/Files/CCF/Worktrees/CCF-CadenceDualAdvance-Source-397/Tools/Verification/CadenceDualAdvance/OfflineLogic/Check.cs:line 28

```

### typecheck output

```text
[prod] distinct errors: 6 (baseline 6)
TYPECHECK_CLEAN mode=prod

```

### typecheck-typo output

```text
[prod] distinct errors: 7 (baseline 6)
NEW  Assets/ForestPrototype/ScenarioOne/ScenarioManagementCadence.cs: error CS1061: 'ScenarioOneManager' does not contain a definition for 'AdvanceYearTypo401' and no accessible extension method 'AdvanceYearTypo401' accepting a first argument of type 'ScenarioOneManager' could be found (are you missing a using directive or an assembly reference?)
TYPECHECK_NEW_ERRORS mode=prod

```
