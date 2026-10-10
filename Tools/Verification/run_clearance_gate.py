#!/usr/bin/env python3
"""Isolated, sequential clearance/regression gate; disposable staging only."""
import argparse,fcntl,json,os,shutil,subprocess,time
from pathlib import Path
os.environ.setdefault('CCF_STAND_GEOMETRY', '0')  # historical anchors run in the explicit Editor-only Legacy40 override (D-056); export CCF_STAND_GEOMETRY=1 to run them in Enlarged80 deliberately
ROOT=Path(__file__).resolve().parents[2]
def check_anchors(gate, rng, log):
    expected = {
        'ScenarioOneCompletionVerification': ['SCENARIO_ONE_COMPLETION_HASH ' + ('568922E1A6D73CDD' if rng == '0' else '00479F18970F9926')],
        'ScenarioOneInteractionVerification': ['hash=BFC55473C1506067', 'continuedHash=9CDF21A541C5968D'],
        'ScenarioReferenceVerification': ['continuedHash=9CDF21A541C5968D', 'worldHash=7AD177B3CC2F73C7'],
        'BrowsingProtectionVerification': ['neutral=BFC55473C1506067', 'A=3485B6630C9EA448 B=3485B6630C9EA448'],
        'RngModelPolicyVerification': ['neutral=BFC55473C1506067 normal=3485B6630C9EA448',
            'neutral=2A0B8C32AC0DE113 normal=506E8AF6D8514C6C', 'hash=CDC4DE8F8EF471E5', 'hash=872094413305A082'],
    }.get(gate, [])
    return [value for value in expected if value not in log]
def main():
    parser=argparse.ArgumentParser(description=__doc__)
    parser.add_argument('gates',nargs='+');parser.add_argument('--rng',choices=['0','1'])
    parser.add_argument('--graphics',action='store_true');parser.add_argument('--interactive',action='store_true')
    parser.add_argument('--capture',action='store_true');parser.add_argument('--timeout',type=int,default=1800)
    args=parser.parse_args();out=ROOT/'Build/ClearanceVerification';out.mkdir(parents=True,exist_ok=True)
    lock=(out/'launch.lock').open('w');fcntl.flock(lock,fcntl.LOCK_EX|fcntl.LOCK_NB)
    config=out/'config';shutil.copytree(Path.home()/'.config/unity3d/Unity/licenses',config/'unity3d/Unity/licenses',dirs_exist_ok=True)
    for gate in args.gates:
        suffix=('model'+args.rng if args.rng else 'default')+('-rendered' if args.capture else '')
        log=out/(gate+'-'+suffix+'.log');source=ROOT/'Tools/Verification'/(gate+'.cs');target=ROOT/'Assets/ForestPrototype'/source.name
        copied=False
        if source.exists():
            if target.exists():raise RuntimeError('Investigate existing staged harness: '+str(target))
            shutil.copy2(source,target);copied=True
        unit='ccf-clearance-'+str(time.time_ns())
        cmd=['systemd-run','--user','--scope','--quiet','--unit='+unit,'-p','MemoryMax=8G','-p','MemorySwapMax=256M',
             '/media/jer/ZX20/Unity/6000.6.0f1/Editor/Unity','-projectPath',str(ROOT),'-job-worker-count','2','-logFile',str(log)]
        if not args.interactive:cmd+=['-batchmode']
        if not args.graphics and not args.interactive:cmd+=['-nographics']
        cmd+=['-quit'] if gate=='Import' else ['-executeMethod',gate+'.Begin']
        env=dict(os.environ,XDG_CONFIG_HOME=str(config),CCF_ACCEPTANCE_OUTPUT=str(out/'evidence'),DISPLAY=os.environ.get('DISPLAY',':0'))
        if args.rng:env['CCF_RNG_MODEL']=args.rng
        if args.capture:env['CCF_CLEARANCE_CAPTURE']='1'
        start=time.monotonic()
        try:
            try:code=subprocess.run(cmd,cwd=ROOT,env=env,timeout=args.timeout).returncode
            except subprocess.TimeoutExpired:
                subprocess.run(['systemctl','--user','stop',unit+'.scope'],check=False);code=124
            s=log.read_text(errors='replace') if log.exists() else ''
            markers=[l for l in s.splitlines() if any(t in l for t in ['_PASS','_FAIL','_HASH','_ANCHORS','BROWSE_LIFECYCLE','BROWSE_SCENARIO_LIFECYCLE','CANONICAL_LIFECYCLE','error CS'])]
            missing=check_anchors(gate,args.rng,s)
            if missing:markers.append('CANONICAL_ANCHOR_FAIL missing='+repr(missing))
            failed=code!=0 or 'error CS' in s or any('_FAIL' in l for l in markers)
            if gate!='Import' and not any('_PASS' in l for l in markers):failed=True
            result=dict(gate=gate,mode=suffix,exit_code=code,status='FAIL' if failed else 'PASS',seconds=round(time.monotonic()-start,2),markers=markers)
            (out/(gate+'-'+suffix+'.result.json')).write_text(json.dumps(result,indent=2)+'\n');print(json.dumps(result),flush=True)
            if failed:raise RuntimeError('Gate failed: '+str(log))
        finally:
            if copied:target.unlink(missing_ok=True);Path(str(target)+'.meta').unlink(missing_ok=True)
if __name__=='__main__':main()
