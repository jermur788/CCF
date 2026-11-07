#!/usr/bin/env python3
"""Run the actual C# historical-layout methods offline and audit their field boundaries against Git history."""
import argparse
import json
import os
from pathlib import Path
import re
import subprocess
import sys

ROOT = Path(__file__).resolve().parents[3]
FILES = [ROOT / 'Tools/Verification/RackV2Persistence/RackV2PersistenceVerification.cs',
         ROOT / 'Tools/Verification/StandGeometry/Enlarged80Verification.cs']


def method(text):
    start = text.index('    private static string HistoricalLayout(')
    opening = text.index('{', start)
    depth = 1
    end = opening + 1
    while depth:
        depth += (text[end] == '{') - (text[end] == '}')
        end += 1
    return text[start:end]


def named_method(text, signature):
    start = text.index(signature)
    end = text.index(';', start) + 1  # expression-bodied helper
    return text[start:end]


# R4: every message literal naming v19/v20/v21 must say "synthetic" unless it states a projection rule, a
# field requirement, a relabelled-header refusal, or an explicitly "projected" diagnostic.
LABEL_ALLOWED = {
    'pre-v21 projection requires ordinary legacy orders',
    'pre-v21 projection requires an empty rack plan',
    'pre-v20 projection requires Legacy40',
    'current v24 file missing nextRackId (required since v21)',
    'current v24 file missing racks (required since v21)',
    'current v24 V2 file relabelled with a pre-v21 header',
    'Legacy40 upgraded re-save byte-identical to projected v21 (diagnostic): ',
}


def misleading_labels(text):
    code = '\n'.join(line for line in text.splitlines() if not line.lstrip().startswith('//'))
    return [s for s in re.findall(r'"((?:[^"\\\n]|\\.)*)"', code)
            if re.search(r'\bv(19|20|21)\b', s) and 'synthetic' not in s and s not in LABEL_ALLOWED]


def label_and_teaching_source_checks():
    rack, enlarged = (p.read_text() for p in FILES)
    for path, text in zip(FILES, (rack, enlarged)):
        assert misleading_labels(text) == [], (path.name, misleading_labels(text))
    # Negative control: the same scan must find the misleading labels #319 R4 reported in the #318 bytes.
    old = [subprocess.check_output(['git', 'show', '6b7d2d45:' + str(p.relative_to(ROOT))], cwd=ROOT, text=True) for p in FILES]
    old_rack, old_enlarged = misleading_labels(old[0]), misleading_labels(old[1])
    for label in ('the v20 save keeps its Enlarged80 geometry', 'the Legacy40 v21 save loads unchanged', 'v21 missing nextRackId',
                  'Legacy40 v21 save: Continue loads it unchanged (geometry, signature, world hash) and it never enters V2',
                  'old save with legacy rack progress (v20 and v21 model 0): orders and marks kept as ordinary work, never V2'):
        assert label in old_rack, label
    assert old_enlarged == ['v19 Legacy40 save into an Enlarged80 world: Legacy40 grid and ground restored, 336 trees, cell 4 at (2.5, -17.5)'], old_enlarged
    assert 'Pass("synthetic v19-layout Legacy40 save into an Enlarged80 world' in enlarged
    print('SYNTHETIC_LABELS_PASS rack=0 enlarged80=0 misleading; negative control #318 rack=%d enlarged80=%d found' % (len(old_rack), len(old_enlarged)))
    # R5 wiring: the absent flag goes through the real disk path after the session held true, and cold start loads
    # both flag values from disk; the in-memory-only absent check and its overclaiming label are gone.
    policy = rack[rack.index('private IEnumerator TeachingHistoryPolicy('):rack.index('private IEnumerator RunV2(')]
    cold = rack[rack.index('private IEnumerator RunColdStart('):]
    assert 'LoadInMemory(absent' not in policy and 'absent teaching history stays false' not in rack
    order = [policy.index(x) for x in ('"positive control: a v24 disk file recording true loads true through Continue"',
                                       'bool sessionBefore = manager.FirstGuidedRackReviewComplete;',
                                       'ContinueFromDisk("synthetic v24 disk file with an absent teaching flag")',
                                       'TeachingLoadDiscriminates(sessionBefore, false, manager.FirstGuidedRackReviewComplete)')]
    assert order == sorted(order), order
    assert cold.count('TeachingLoadDiscriminates(sessionBefore, ') == 2 and '!seededValue' in cold
    assert 'seededFlag.Type == JTokenType.Boolean' in cold
    print('TEACHING_CONTROL_WIRING_PASS disk-absent-after-true and cold-start both-polarity controls present')


def fields(commit, path, cls):
    text = subprocess.check_output(['git', 'show', commit + ':' + path], cwd=ROOT, text=True)
    body = text.split('class ' + cls + '\n', 1)[1].split('\n}', 1)[0]
    return set(re.findall(r'^    public (?!const\b|static\b)[\w<>]+ (\w+)(?:\s*=(?!>)|;)', body, re.M))


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument('--out', type=Path, required=True)
    args = ap.parse_args()
    out = args.out.resolve()
    assert any(out.is_relative_to(Path('/media/jer/Files/CCF/Verification') / allowed) for allowed in
               ('Workbench287MorningClosure-20261107/additions', 'WorkbenchRunnerR1R5-20261107'))
    out.mkdir(parents=True, exist_ok=False)
    # Historical schema evidence: audit all relevant public field additions, not version headers alone.
    paths = [('Assets/ForestPrototype/ForestSaveData.cs', 'ScenarioOneSaveData'),
             ('Assets/ForestPrototype/ScenarioOne/ScenarioOneWorkOrder.cs', 'ScenarioOneWorkOrder'),
             ('Assets/ForestPrototype/ScenarioOne/ScenarioManagementEvent.cs', 'ScenarioManagementEvent')]
    expected = [set('firstGuidedRackReviewComplete objectiveAchievements nextPrescriptionId prescriptions'.split()),
                set('prescriptionId prescriptionCoverageUnitId prescriptionCropTreeId prescriptionReason'.split()), {'prescriptionId'}]
    evidence = {}
    for version, commit in [(19, '4c40b0ea'), (20, 'b0246334'), (21, 'cf84d24c')]:
        evidence[str(version)] = {}
        for (path, cls), added in zip(paths, expected):
            actual = fields('4d95e452', path, cls) - fields(commit, path, cls)
            if version < 21:
                added = added | (set('rackPlanningModel rackPlanConfirmed nextRackId racks'.split()) if cls == 'ScenarioOneSaveData' else {'rackId'} if cls == 'ScenarioOneWorkOrder' else set())
            assert actual == added, (version, cls, actual, added)
            evidence[str(version)][cls] = sorted(actual)
    (out / 'historical-schema-fields.json').write_text(json.dumps(evidence, indent=2) + '\n')
    print('HISTORICAL_SCHEMA_FIELD_BOUNDARIES_PASS v19/v20/v21 against pinned production schema')
    label_and_teaching_source_checks()
    methods = [method(p.read_text()) for p in FILES]
    teaching = named_method(FILES[0].read_text(), '    private static bool TeachingLoadDiscriminates(')
    assert methods[0] == methods[1], 'the two self-contained disposable gates must use the same audited projection'
    source = '''using System; using System.Linq; using Newtonsoft.Json.Linq;
static class StandGeometryModel { public const int Legacy40 = 0; }
static class Controls {
static int checks;
static int teachingChecks;
static void Check(bool ok, string message) { if (!ok) throw new InvalidOperationException(message); }
''' + methods[0] + '\n' + teaching + '''
static JObject Fixture() => JObject.Parse(@"{'version':24,'standGeometryModel':0,'trees':[{'treeId':'sentinel','diameterCm':17}],
 'scenarioOne':{'firstGuidedRackReviewComplete':false,'objectiveAchievements':[], 'nextPrescriptionId':1,'prescriptions':[],
 'rackPlanningModel':0,'rackPlanConfirmed':false,'nextRackId':1,'racks':[],
 'workOrders':[{'rackId':0,'prescriptionId':0,'prescriptionCoverageUnitId':'','prescriptionCropTreeId':'','prescriptionReason':0,'targetTreeId':'sentinel'}],
 'managementEvents':[{'prescriptionId':0,'year':7}]}}");
static JObject Scenario(JObject root) => (JObject)root["scenarioOne"];
static void Test(bool ok, string name) { Check(ok, name); checks++; Console.WriteLine("PROJECTION_PASS " + name); }
static void Refuse(Action<JObject> mutate, int version, string name) {
 var root=Fixture(); mutate(root); bool refused=false;
 try { HistoricalLayout(root.ToString(),version); } catch(InvalidOperationException) { refused=true; }
 Test(refused,name);
}
static void Main() {
 foreach(int version in new[]{19,20,21}) {
  var root=Fixture(); var projected=JObject.Parse(HistoricalLayout(root.ToString(),version));
  Test((int)projected["version"]==version,"explicit header "+version);
  Test(JToken.DeepEquals(root["trees"],projected["trees"]),"tree records unchanged "+version);
  Test((string)Scenario(projected)["workOrders"][0]["targetTreeId"]=="sentinel" && (int)Scenario(projected)["managementEvents"][0]["year"]==7,"legacy progress kept "+version);
  Test(Scenario(projected)["firstGuidedRackReviewComplete"]==null && Scenario(projected)["objectiveAchievements"]==null && Scenario(projected)["prescriptions"]==null,"no later history "+version);
  Test((projected["standGeometryModel"]!=null)==(version>=20),"geometry boundary "+version);
  Test((Scenario(projected)["rackPlanningModel"]!=null)==(version>=21) && (Scenario(projected)["workOrders"][0]["rackId"]!=null)==(version>=21),"rack boundary "+version);
 }
 Refuse(r=>Scenario(r)["firstGuidedRackReviewComplete"]=true,20,"do not discard completed review");
 Refuse(r=>((JArray)Scenario(r)["objectiveAchievements"]).Add(new JObject()),21,"do not discard objective history");
 Refuse(r=>((JArray)Scenario(r)["prescriptions"]).Add(new JObject()),20,"do not discard prescription");
 Refuse(r=>Scenario(r)["workOrders"][0]["prescriptionId"]=1,21,"do not discard order provenance");
 Refuse(r=>Scenario(r)["managementEvents"][0]["prescriptionId"]=1,20,"do not discard event provenance");
 Refuse(r=>Scenario(r)["rackPlanConfirmed"]=true,20,"do not discard confirmed rack plan");
 Refuse(r=>Scenario(r)["workOrders"][0]["rackId"]=3,20,"do not discard linked rack order");
 Refuse(r=>r["standGeometryModel"]=1,19,"do not disguise Enlarged80 as pre-v20");
 Refuse(r=>r["version"]=25,20,"unaudited source schema refused");
 var v2=Fixture(); Scenario(v2)["rackPlanConfirmed"]=true; Scenario(v2)["workOrders"][0]["rackId"]=3;
 ((JArray)Scenario(v2)["racks"]).Add(new JObject{["rackId"]=3});
 var v21=JObject.Parse(HistoricalLayout(v2.ToString(),21));
 Test(JToken.DeepEquals(Scenario(v2)["racks"],Scenario(v21)["racks"]) && (int)Scenario(v21)["workOrders"][0]["rackId"]==3,"v21 preserves V2 records and links");
 Console.WriteLine("HISTORICAL_PROJECTION_PASS checks="+checks);
 // R5: the actual TeachingLoadDiscriminates helper. Positive controls must pass; trivial and ignored loads must not.
 Teach(TeachingLoadDiscriminates(true,false,false),"absent flag on disk applied over a true session");
 Teach(TeachingLoadDiscriminates(false,true,true),"cold start applies the opposite (true) flag");
 Teach(!TeachingLoadDiscriminates(false,false,false),"false-seeded comparison with a false session is rejected as trivial");
 Teach(!TeachingLoadDiscriminates(true,true,true),"true-seeded comparison with a true session is rejected as trivial");
 Teach(!TeachingLoadDiscriminates(true,false,true),"a load that ignored the absent-flag file is caught");
 Teach(!TeachingLoadDiscriminates(false,true,false),"a load that ignored the opposite-flag file is caught");
 Console.WriteLine("TEACHING_CONTROLS_PASS checks="+teachingChecks);
}
static void Teach(bool ok, string name) { Check(ok, name); teachingChecks++; Console.WriteLine("TEACHING_CONTROL_PASS " + name); }
}'''
    (out / 'Controls.cs').write_text(source)
    sdk = subprocess.check_output(['dotnet', '--list-sdks'], text=True).splitlines()[-1]
    version, directory = sdk.split(' ', 1)
    newtonsoft = Path(directory.strip('[]')) / version / 'Newtonsoft.Json.dll'
    assert newtonsoft.is_file()
    # Direct Roslyn invocation uses installed framework references; no restore, network, or Unity process.
    refs = sorted(Path('/usr/lib/dotnet/packs/Microsoft.NETCore.App.Ref').glob('8.*/ref/net8.0/*.dll'))
    assert refs
    compiler = Path(directory.strip('[]')) / version / 'Roslyn/bincore/csc.dll'
    (out / 'Controls.runtimeconfig.json').write_text(json.dumps({'runtimeOptions': {'tfm': 'net8.0', 'framework': {'name': 'Microsoft.NETCore.App', 'version': '8.0.0'}}}))
    compile_cmd = ['dotnet', str(compiler), '-nologo', '-target:exe', '-out:' + str(out / 'Controls.dll'),
                   '-r:' + str(newtonsoft)] + ['-r:' + str(p) for p in refs] + [str(out / 'Controls.cs')]
    import shutil
    shutil.copy2(newtonsoft, out / 'Newtonsoft.Json.dll')
    env = dict(os.environ, DOTNET_CLI_HOME=str(out / 'dotnet-home'), DOTNET_CLI_TELEMETRY_OPTOUT='1', DOTNET_NOLOGO='1')
    for label, command in [('build', compile_cmd), ('run', ['dotnet', str(out / 'Controls.dll')])]:
        (out / (label + '.command.json')).write_text(json.dumps(command) + '\n')
        result = subprocess.run(command, cwd=out, env=env, capture_output=True, text=True, timeout=90)
        (out / (label + '.stdout')).write_text(result.stdout)
        (out / (label + '.stderr')).write_text(result.stderr)
        (out / (label + '.exit')).write_text(str(result.returncode) + '\n')
        print(result.stdout + result.stderr)
        if result.returncode: return result.returncode
    return 0


if __name__ == '__main__':
    sys.exit(main())
