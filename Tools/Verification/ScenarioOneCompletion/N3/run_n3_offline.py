#!/usr/bin/env python3
"""Compile actual N3 rules/save validators and extracted manager boundaries without Unity."""
from pathlib import Path
import argparse,hashlib,json,subprocess,sys
HERE=Path(__file__).resolve().parent;ROOT=HERE.parents[3];FP=ROOT/'Assets/ForestPrototype'
sys.path.insert(0,str(ROOT/'Tools'/'Verification'))
import source_normalization
# Normalized (comment/whitespace-insensitive) sha256 of the accepted N3 completion-rules source.
# Replaces the previous raw whole-file hash 862eda3ffd4f0ca3a857474773bc54b04abb8d01d5f894bdf482e0c2851dbe3f,
# which suppressed all 88 N3 checks on any comment/whitespace edit. See Tools/Verification/source_normalization.py.
N3_RULES_NORMALIZED_SHA256='8166775d17827112282a668f8d0859d6e31c6868fa4724c77c13dbf6d6f80648'
def method(text,signature):
    start=text.index(signature);brace=text.index('{',start);depth=1;end=brace+1
    while depth:
        if text[end]=='{':depth+=1
        elif text[end]=='}':depth-=1
        end+=1
    return text[start:end]
def main():
    parser=argparse.ArgumentParser();parser.add_argument('--out',type=Path,required=True);args=parser.parse_args();args.out.mkdir(parents=True,exist_ok=True)
    assert source_normalization.normalized_sha256(FP/'ScenarioOne/ScenarioOneCompletionRules.cs') == N3_RULES_NORMALIZED_SHA256
    manager=(FP/'ScenarioOne/ScenarioOneManager.cs').read_text();enums=(FP/'ScenarioOne/ScenarioManagementEvent.cs').read_text()
    assert 'AnnualReviewAcknowledged = 9' in enums
    # Frozen pre-extension values from f4d0a1 compatibility contract.
    import re
    body=re.search(r'enum ScenarioManagementEventType\s*\{([^}]+)\}',enums,re.S).group(1)
    tokens=re.sub(r'//[^\n]*','',body).split(',');value=-1;actual={}
    for token in tokens:
        token=token.strip()
        if not token:continue
        pair=token.split('=');value=int(pair[1]) if len(pair)>1 else value+1;actual[pair[0].strip()]=value
    expected=dict(zip(['StockPurchased','OrderCreated','OrderApproved','OrderCancelled','WorkResolved','YearAdvanced','ScenarioCompleted','ScenarioFailed','CenturyReviewed','AnnualReviewAcknowledged'],range(10)))
    assert actual==expected,(actual,expected)
    assert 'if (outcome == ScenarioOneOutcome.Active)' in method(manager,'private void EvaluateProgress()')
    # Guidance is presentation only: it must not appear in completion evaluation, restore or the acknowledgement handler.
    for name in ['private void EvaluateProgress()','public void RestoreSaveData(ScenarioOneSaveData data, int saveVersion)','public void MarkAnnualReviewSeen()']:
        assert 'Guidance' not in method(manager,name),name
    history=(FP/'ScenarioOne/ScenarioOneManager.History.cs').read_text()
    assert 'GuidanceText(CompletionGuidance)' in history and 'CompletionGuidanceState.None' in history
    assert 'CompletionGuidanceState.AcknowledgeReview' in (FP/'UI/AnnualReviewView.cs').read_text()
    pred='objectives.Count > 0 && objectives.All(result => result.achieved)\n                && ScenarioOneCompletionRules.HasReviewedTwoCycleSequence(managementEvents, workOrders)'
    assert pred in manager
    restore=method(manager,'public void RestoreSaveData(ScenarioOneSaveData data, int saveVersion)')
    assert 'annualReviewSeen = data.annualReviewSeen;' in restore and 'AnnualReviewAcknowledged' not in restore
    counter=restore[restore.index('nextManagementEventId = Mathf.Max(1, data.nextManagementEventId);'):restore.index('// JsonUtility')]
    bridge='''using System;using System.Collections.Generic;using System.Linq;using UnityEngine;
public sealed class ManagerBoundaryBridge {
 public int CurrentYear;public long cashCents;public bool annualReviewSeen;
 public List<ScenarioAnnualReport> annualReports=new List<ScenarioAnnualReport>();
 public List<ScenarioManagementEvent> managementEvents=new List<ScenarioManagementEvent>();
 public List<ScenarioOneWorkOrder> workOrders=new List<ScenarioOneWorkOrder>();
 public int nextManagementEventId=1;public ScenarioOneOutcome outcome;public int outcomeYear;public string outcomeReason;
'''+method(manager,'public void MarkAnnualReviewSeen()')+'\n'+method(manager,'private void RecordEvent(ScenarioManagementEvent entry)')+'\n'+method(manager,'private static List<ScenarioManagementEvent> CloneEvents(List<ScenarioManagementEvent> source)')+'''
 public void RestoreTerminalAndCounter(ScenarioOneSaveData data) {
 managementEvents=CloneEvents(data.managementEvents);
'''+counter+'''
 }
 public bool CanComplete(bool allObjectivesReached) {
 if (outcome != ScenarioOneOutcome.Active) return outcome == ScenarioOneOutcome.Completed;
 return '''+pred.replace('objectives.Count > 0 && objectives.All(result => result.achieved)','allObjectivesReached')+''';
 }
}
'''
    (args.out/'ManagerBoundaryBridge.cs').write_text(bridge)
    # Post-rack: ScenarioOneWorkOrder.cs labels map squares through ScenarioOneUiFacts.SquareLabel, which needs a live
    # ForestEcologyController and UI Toolkit. With no ecology production returns its "#N" fallback, and an offline run has
    # none, so shim exactly that. The assert keeps the shim honest: it fails if production changes the signature or fallback.
    facts=(FP/'UI/ScenarioOneUiFacts.cs').read_text()
    assert 'public static string SquareLabel(int cellIndex)' in facts and '"#" + cellIndex' in facts
    (args.out/'UiFactsBridge.cs').write_text('public static class ScenarioOneUiFacts { public static string SquareLabel(int cellIndex) => "#" + cellIndex; }\n')
    shim=(ROOT/'Tools/Verification/ThreeSpecies/OfflineLogic/UnityShim.cs').read_text()
    shim=shim.replace('public static class Mathf\n    {','public static class Mathf\n    {\n        public static int RoundToInt(float v)=>(int)Math.Round(v);\n        public static int CeilToInt(float v)=>(int)Math.Ceiling(v);\n        public static int FloorToInt(float v)=>(int)Math.Floor(v);')
    (args.out/'UnityShim.cs').write_text(shim)
    sources=[FP/f for f in ['ScenarioOne/ScenarioOneCompletionRules.cs','ScenarioOne/ScenarioOneCompletionGuidance.cs','ScenarioOne/ScenarioManagementEvent.cs','ScenarioOne/ScenarioOneWorkOrder.cs','ScenarioOne/ScenarioEcologicalSnapshot.cs','ScenarioOne/ScenarioDeadwoodRecord.cs','ForestSaveData.cs','ForestSaveValidation.cs','ForestSaveValidation.Geometry.cs','ForestSaveValidation.Storms.cs','StandGeometryModel.cs','RegenerationModel.cs','SitkaGrowthModel.cs']]+sorted((FP/'WorkEconomy').glob('*.cs'))+sorted((HERE/'OfflineLogic').glob('*.cs'))+[args.out/'ManagerBoundaryBridge.cs',args.out/'UiFactsBridge.cs',args.out/'UnityShim.cs']
    items=''.join('<Compile Include="'+str(f)+'" />' for f in sources)
    project='<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><TargetFramework>net8.0</TargetFramework><OutputType>Exe</OutputType><EnableDefaultCompileItems>false</EnableDefaultCompileItems><ImplicitUsings>disable</ImplicitUsings><StartupObject>N3OfflineCheck</StartupObject></PropertyGroup><ItemGroup><PackageReference Include="Newtonsoft.Json" Version="13.0.3" />'+items+'</ItemGroup></Project>'
    (args.out/'n3.csproj').write_text(project)
    build=subprocess.run(['dotnet','build','-v','q','-c','Release'],cwd=args.out,capture_output=True,text=True)
    if build.returncode:print(build.stdout+build.stderr);return build.returncode
    run=subprocess.run(['dotnet','run','--no-build','-c','Release'],cwd=args.out,capture_output=True,text=True);print(run.stdout+run.stderr,end='')
    (args.out/'result.txt').write_text(run.stdout+run.stderr)
    (args.out/'source-manifest.json').write_text(json.dumps({str(f.relative_to(ROOT)) if f.is_relative_to(ROOT) else f.name:hashlib.sha256(f.read_bytes()).hexdigest() for f in sources},indent=2)+'\n')
    return run.returncode
if __name__=='__main__':sys.exit(main())
