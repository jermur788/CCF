#!/usr/bin/env python3
"""D-066 offline qualification. Uses production sources, the old sequence as an independent oracle,
and pre-v24 schema/archive from the exact implementation base. Output must be durable on Files.
The serializer here is a stand-in; Unity JsonUtility/controller qualification remains mandatory.
"""
import argparse, glob, importlib.util, os, re, subprocess, sys
sys.dont_write_bytecode = True
from pathlib import Path
import xml.etree.ElementTree as ET
ROOT = Path(__file__).resolve().parents[3]
HERE = Path(__file__).resolve().parent
BASE = 'c8cfae8c4025cec6e960a050edf4bfde2a981de5'
spec = importlib.util.spec_from_file_location('mfm', ROOT/'Tools/Verification/MfmSaveProvenance/run_offline_check.py')
mfm = importlib.util.module_from_spec(spec); spec.loader.exec_module(mfm)

def main():
 p=argparse.ArgumentParser(); p.add_argument('--out',required=True,type=Path); args=p.parse_args()
 out=args.out.resolve()
 assert out.is_relative_to(Path('/media/jer/Files/CCF/Verification'))
 out.mkdir(parents=True,exist_ok=True)
 # First build the existing MFM suite, then reuse its fully typed save neighbours and shims.
 project=ROOT/'Tools/Verification/MfmSaveProvenance/run_offline_check.py'
 result=subprocess.run(['python3',str(project),'--out',str(out/'mfm')],text=True,capture_output=True)
 (out/'mfm.log').write_text(result.stdout+result.stderr)
 if result.returncode: print(result.stdout+result.stderr); return result.returncode
 directory=out/'checks';directory.mkdir(exist_ok=True)
 tree=ET.parse(out/'mfm/candidate/candidate.csproj')
 group=tree.getroot().find('ItemGroup')
 for node in list(group):
  if node.tag=='Compile' and Path(node.attrib['Include']).name=='PrescriptionSaveOfflineCheck.cs': group.remove(node)
 extras=[HERE/'PersistenceOfflineCheck.cs',ROOT/'Assets/ForestPrototype/UI/ScenarioOneTeachingSequence.cs',ROOT/'Assets/ForestPrototype/UI/ScenarioOneTeachingCap.cs',ROOT/'Assets/ForestPrototype/UI/ScenarioOneRackBayText.cs',ROOT/'Assets/ForestPrototype/ScenarioOne/ScenarioOneRackReview.cs',ROOT/'Assets/ForestPrototype/ScenarioOne/ScenarioOneRackBay.cs',ROOT/'Assets/ForestPrototype/ScenarioOne/CompetitionShare.cs',ROOT/'Assets/ForestPrototype/Racks/RackGeometry.cs',ROOT/'Tools/Verification/ScenarioOneNonUi/OfflineLogic/OfflineStubs.cs',ROOT/'Tools/Verification/ScenarioOneRepeatedInterventions/TeachingCapOfflineCheck.cs']
 old=mfm.git_show(BASE,'Assets/ForestPrototype/UI/ScenarioOneTeachingSequence.cs').replace('ScenarioOneTeachingSequence','OldTeachingSequence')
 (directory/'OldTeachingSequence.cs').write_text(old);extras.append(directory/'OldTeachingSequence.cs')
 # Compile the actual latch storage/property/write method verbatim. Capture/restore are checked in Unity.
 manager=(ROOT/'Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.cs').read_text()
 start=manager.index('    private bool firstGuidedRackReviewComplete;'); end=manager.index('    public void InitializeNewScenario()',start)
 (directory/'LatchManager.cs').write_text('public sealed partial class ScenarioOneManager { private bool referencePreviewActive;\n'+manager[start:end]+'}\n');extras.append(directory/'LatchManager.cs')
 for source in extras: ET.SubElement(group,'Compile',Include=str(source))
 tree.getroot().find('PropertyGroup/StartupObject').text='PersistenceOfflineCheck'
 tree.write(directory/'checks.csproj')
 env=dict(os.environ,CCF_REPO_ROOT=str(ROOT),DOTNET_CLI_TELEMETRY_OPTOUT='1',DOTNET_NOLOGO='1')
 for command in (['dotnet','build','-v','q','-c','Release'],['dotnet','run','--no-build','-c','Release']):
  run=subprocess.run(command,cwd=directory,env=env,text=True,capture_output=True); print(run.stdout+run.stderr)
  if run.returncode:return run.returncode
 # Recompile the old v23 DTO/hash path against the same non-default prescription fixtures.
 baseline=out/'v23';baseline.mkdir(exist_ok=True)
 for name,path in [('ForestSaveData.cs','Assets/ForestPrototype/ForestSaveData.cs'),('ArchiveBridge.cs','Assets/ForestPrototype/ScenarioOne/ScenarioReferenceArchive.cs')]:
  text=mfm.git_show(BASE,path)
  (baseline/name).write_text(mfm.archive_bridge(text) if name=='ArchiveBridge.cs' else text)
 fixture=HERE/'PersistenceHashFixtures.cs'
 srcs=[n.attrib['Include'] for n in group if n.tag=='Compile']
 srcs=[s for s in srcs if Path(s).name in ('ScenarioManagementEvent.cs','ScenarioOneWorkOrder.cs','ScenarioEcologicalSnapshot.cs','ScenarioDeadwoodRecord.cs','RackPlanningModel.cs','StandGeometryModel.cs','RegenerationModel.cs','SitkaGrowthModel.cs','SaveNeighbours.cs','UnityShim.cs','UiFactsBridge.cs') or '/WorkEconomy/' in s]
 for label,folder,dto,archive in [('v23',baseline,baseline/'ForestSaveData.cs',baseline/'ArchiveBridge.cs'),('v24',out/'v24',ROOT/'Assets/ForestPrototype/ForestSaveData.cs',out/'mfm/candidate/ArchiveBridge.cs')]:
  folder.mkdir(exist_ok=True)
  sources=srcs+[str(dto),str(archive),str(fixture)]
  (folder/'hash.csproj').write_text(mfm.project(sources,'PersistenceHashFixtures'))
  for command in (['dotnet','build','-v','q','-c','Release'],['dotnet','run','--no-build','-c','Release']):
   run=subprocess.run(command,cwd=folder,env=env,text=True,capture_output=True)
   if run.returncode: print(run.stdout+run.stderr);return run.returncode
  (out/(label+'-hashes.txt')).write_text(run.stdout)
 assert (out/'v23-hashes.txt').read_bytes()==(out/'v24-hashes.txt').read_bytes(),'v23 identity mismatch (no rebaseline permitted)'
 print('PERSISTENCE_V23_ALL_HASH_BASELINE_PASS '+str(len(run.stdout.splitlines()))+' fixture rows')
 protected=['Assets/ForestPrototype/ScenarioOne/ScenarioOneCompletionRules.cs','Assets/ForestPrototype/ScenarioOne/ScenarioOneObjectives.cs','Assets/ForestPrototype/ScenarioOne/Resources','Assets/ForestPrototype/Racks','Assets/ForestPrototype/WorkEconomy','Assets/Scenes','Packages','ProjectSettings']
 diff=subprocess.check_output(['git','-C',str(ROOT),'diff','--name-only',BASE,'--']+protected,text=True)
 assert not diff,diff
 print('PERSISTENCE_PROTECTED_SOURCES_UNCHANGED '+BASE)
 return 0
if __name__=='__main__':raise SystemExit(main())
