#!/usr/bin/env python3
"""Sequential isolated Unity diagnostics; stage exactly one disposable source."""
import argparse,fcntl,json,os,shutil,subprocess,time
from pathlib import Path
os.environ.setdefault('CCF_STAND_GEOMETRY', '0')  # historical anchors run in the explicit Editor-only Legacy40 override (D-056); export CCF_STAND_GEOMETRY=1 to run them in Enlarged80 deliberately
parser=argparse.ArgumentParser();parser.add_argument('--visual',action='store_true');parser.add_argument('--skip-inventory',action='store_true');parser.add_argument('--fixtures',action='store_true');parser.add_argument('--performance-only',action='store_true');args=parser.parse_args()
gate='UnderstoreyVisualAudit' if args.visual else 'UnderstoreyFixtures' if args.fixtures or args.performance_only else 'UnderstoreyDiagnostics'
ROOT=Path(__file__).resolve().parents[3]
out=ROOT/'Build/UnderstoreyRecruitment';out.mkdir(parents=True,exist_ok=True)
lock=(out/'launch.lock').open('w');fcntl.flock(lock,fcntl.LOCK_EX|fcntl.LOCK_NB)
config=out/'config';shutil.copytree(Path.home()/'.config/unity3d/Unity/licenses',config/'unity3d/Unity/licenses',dirs_exist_ok=True)
source=Path(__file__).with_name(gate+'.cs');target=ROOT/'Assets/ForestPrototype'/source.name
if target.exists():raise RuntimeError('Existing disposable script: '+str(target))
shutil.copy2(source,target)
unit='ccf-understorey-'+str(time.time_ns());log=out/(gate+'.log');start=time.monotonic()
try:
 env=dict(os.environ,XDG_CONFIG_HOME=str(config),CCF_UNDERSTOREY_OUTPUT=str(out/'evidence'),DISPLAY=os.environ.get('DISPLAY',':0'))
 if args.skip_inventory:env['CCF_SKIP_INVENTORY']='1'
 if args.performance_only:env['CCF_FIXTURE_PERFORMANCE_ONLY']='1'
 cmd=['systemd-run','--user','--scope','--quiet','--unit='+unit,'-p','MemoryMax=8G','-p','MemorySwapMax=256M','/media/jer/ZX20/Unity/6000.6.0f1/Editor/Unity','-projectPath',str(ROOT),'-job-worker-count','2','-executeMethod',gate+'.Begin','-logFile',str(log)]
 if not args.visual:cmd+=['-batchmode','-nographics']
 try:code=subprocess.run(cmd,cwd=ROOT,env=env,timeout=3600).returncode
 except subprocess.TimeoutExpired:
  subprocess.run(['systemctl','--user','stop',unit+'.scope'],check=False);code=124
 text=log.read_text(errors='replace') if log.exists() else '';markers=[l for l in text.splitlines() if 'UNDERSTOREY_' in l and ('_PASS' in l or '_FAIL' in l) or 'error CS' in l]
 result=dict(exit_code=code,seconds=round(time.monotonic()-start,2),markers=markers,status='PASS' if code==0 and ('UNDERSTOREY_VISUAL_AUDIT_PASS' if args.visual else 'UNDERSTOREY_FIXTURES_PASS' if gate=='UnderstoreyFixtures' else 'UNDERSTOREY_DIAGNOSTICS_PASS') in text and '_FAIL' not in '\n'.join(markers) and 'error CS' not in text else 'FAIL')
 (out/(gate+'.result.json')).write_text(json.dumps(result,indent=2)+'\n');print(json.dumps(result),flush=True)
finally:
 target.unlink(missing_ok=True);Path(str(target)+'.meta').unlink(missing_ok=True)
