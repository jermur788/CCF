#!/usr/bin/env python3
"""Sequential S1-A gates. Uses isolated config, never another worker's Editor.
Stages only Tools/Verification sources; always removes source/meta. Keeps distinct
logs/results for repeats. --phase targeted|focused|regression; default all.
"""
import argparse, fcntl, json, os, re, shutil, subprocess, time
from pathlib import Path
os.environ.setdefault('CCF_STAND_GEOMETRY', '0')  # historical anchors run in the explicit Editor-only Legacy40 override (D-056); export CCF_STAND_GEOMETRY=1 to run them in Enlarged80 deliberately
ROOT = Path(__file__).resolve().parents[2]
OUT = ROOT/'Build/S1A'
UNITY = '/media/jer/ZX20/Unity/6000.6.0f1/Editor/Unity'

def run_gate(gate, name, interactive=False):
    source = ROOT/'Tools/Verification'/(gate+'.cs')
    target = ROOT/'Assets/ForestPrototype'/source.name
    if target.exists(): raise RuntimeError('Existing staged harness: '+str(target))
    config = ROOT/'Build/UnderstoreyRecruitment/config'
    shutil.copytree(Path.home()/'.config/unity3d/Unity/licenses', config/'unity3d/Unity/licenses', dirs_exist_ok=True)
    log = OUT/(name+'.log'); evidence = OUT/'evidence'/name; evidence.mkdir(parents=True, exist_ok=True)
    unit = 'ccf-s1a-'+str(time.time_ns())
    cmd = ['systemd-run','--user','--scope','--quiet','--unit='+unit,'-p','MemoryMax=8G','-p','MemorySwapMax=256M',
           UNITY,'-projectPath',str(ROOT),'-job-worker-count','2','-executeMethod',gate+'.Begin','-logFile',str(log)]
    if not interactive: cmd += ['-batchmode','-nographics']
    env = dict(os.environ, XDG_CONFIG_HOME=str(config), CCF_ACCEPTANCE_OUTPUT=str(evidence), DISPLAY=os.environ.get('DISPLAY',':0'))
    start = time.monotonic(); shutil.copy2(source,target)
    try:
        try: code = subprocess.run(cmd,cwd=ROOT,env=env,timeout=1800).returncode
        except subprocess.TimeoutExpired:
            subprocess.run(['systemctl','--user','stop',unit+'.scope'],check=False); code=124
    finally:
        target.unlink(missing_ok=True); Path(str(target)+'.meta').unlink(missing_ok=True)
    content = log.read_text(errors='replace') if log.exists() else ''
    expected = {'ScenarioOneTeachingCopyVerification':'S1A_TEACHING_VERIFY_PASS','MenuTutorialVerification':'MENU_TUTORIAL_PLAYTHROUGH_PASS',
                'ClearanceVerification':'CLEARANCE_ACCEPTANCE_PASS','ScenarioOneRemovalVerification':'SCENARIO_ONE_REMOVAL_VERIFY_PASS'}[gate]
    markers = [line for line in content.splitlines() if any(x in line for x in ['_PASS','_FAIL','_HASH','error CS'])]
    passed = code == 0 and expected in content and not any('_FAIL' in line or 'error CS' in line for line in markers)
    result = dict(name=name, gate=gate, mode='interactive' if interactive else 'batch', status='PASS' if passed else 'FAIL',
                  exit_code=code, seconds=round(time.monotonic()-start,1), markers=markers)
    (OUT/(name+'.json')).write_text(json.dumps(result,indent=2)+'\n'); print(json.dumps({k:result[k] for k in ['name','status','seconds']}),flush=True)
    if not passed: raise RuntimeError('Gate failed; inspect '+str(log))
    return result

def main():
    parser=argparse.ArgumentParser(); parser.add_argument('--phase',choices=['targeted','focused','regression','all'],default='all'); args=parser.parse_args()
    OUT.mkdir(parents=True,exist_ok=True)
    lock=(OUT/'launch.lock').open('w'); fcntl.flock(lock,fcntl.LOCK_EX|fcntl.LOCK_NB)
    busy=subprocess.run(['pgrep','-f','Editor/Unity '],capture_output=True,text=True).stdout.split()
    if busy: raise RuntimeError('Another Unity Editor is running: '+str(busy))
    results=[]
    if args.phase in ['targeted','all']:
        for name,interactive in [('teaching-batch-1',False),('teaching-batch-2',False),('teaching-rendered',True)]:
            results.append(run_gate('ScenarioOneTeachingCopyVerification',name,interactive))
        hashes=[re.findall(r'S1A_READONLY_HASH ([0-9A-F]{16})','\n'.join(r['markers']))[0] for r in results[:2]]
        if hashes[0]!=hashes[1]: raise RuntimeError('Teaching determinism mismatch: '+str(hashes))
        for name in ['menu-standalone-1','menu-standalone-2']: results.append(run_gate('MenuTutorialVerification',name,True))
        results.append(run_gate('ClearanceVerification','clearance',True))
        results.append(run_gate('ScenarioOneRemovalVerification','removal',False))
    if args.phase in ['focused','all']:
        for phase,anchor in [('P2','F58FB0B1A421D28B'),('P3','F7C2FC966816BBAD')]:
            subprocess.run(['python3',str(ROOT/f'Tools/Verification/ScenarioOneCompletion/{phase}/run_{phase.lower()}_gates.py')],cwd=ROOT,check=True)
            result=json.loads((ROOT/f'Build/{phase}/results.json').read_text())
            if result['status']!='PASS' or any(r['hash']!=anchor for r in result['results']): raise RuntimeError(phase+' anchor/gate failure')
            results.append(dict(name=phase,status='PASS',hash=anchor))
    if args.phase in ['regression','all']:
        subprocess.run(['python3',str(ROOT/'Tools/Verification/WindthrowV1/run_regression.py')],cwd=ROOT,check=True)
        regression=json.loads((ROOT/'Build/WindthrowV1/Regression/results.json').read_text())
        if len(regression)!=24 or any(r['status']!='PASS' for r in regression): raise RuntimeError('Regression is not 24/24 PASS')
        for gate,anchor in [('ScenarioOneCompletionVerification','702766DECE591E21'),('ScenarioReferenceVerification','7AD177B3CC2F73C7'),('ScenarioReferenceVerification','9CDF21A541C5968D')]:
            match=next(r for r in regression if r['gate']==gate)
            if anchor not in '\n'.join(match['markers']): raise RuntimeError('Anchor moved: '+gate+' '+anchor)
        baseline = json.loads((ROOT/'Docs/Verification/StormsWindthrowIntegration/regression-results.json').read_text())
        established = {r['gate']: set(re.findall(r'\b[0-9A-F]{16}\b', '\n'.join(r['markers']))) for r in baseline}
        for result in regression:
            exercised = set(re.findall(r'\b[0-9A-F]{16}\b', '\n'.join(result['markers'])))
            if exercised != established[result['gate']]:
                raise RuntimeError('Established modern anchors moved: '+result['gate'])
        results.append(dict(name='regression',status='PASS',count=24,all_established_anchors_unchanged=True))
    (OUT/(args.phase+'-results.json')).write_text(json.dumps(results,indent=2)+'\n')
    print('S1A_'+args.phase.upper()+'_PASS',flush=True)
if __name__=='__main__': main()
