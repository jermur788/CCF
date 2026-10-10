#!/usr/bin/env python3
"""One explicit isolated model2 Unity diagnostic at a time."""
import argparse,fcntl,hashlib,json,os,shutil,subprocess,time
from pathlib import Path
os.environ.setdefault('CCF_STAND_GEOMETRY', '0')  # historical anchors run in the explicit Editor-only Legacy40 override (D-056); export CCF_STAND_GEOMETRY=1 to run them in Enlarged80 deliberately
p=argparse.ArgumentParser();p.add_argument('--gate',choices=['Model2Verification','Model2Matrix','Model2Performance','Model2TargetedEconomy'],default='Model2Verification');args=p.parse_args()
r=Path(__file__).resolve().parents[3];out=r/'Build/UnderstoreyRecruitment/Model2';out.mkdir(parents=True,exist_ok=True)
lock=(r/'Build/UnderstoreyRecruitment/launch.lock').open('w');fcntl.flock(lock,fcntl.LOCK_EX|fcntl.LOCK_NB)
source=Path(__file__).with_name(args.gate+'.cs');target=r/'Assets/ForestPrototype'/source.name
assert not target.exists(),target
shutil.copy2(source,target);unit='ccf-model2-'+str(time.time_ns());log=out/(args.gate+'.log');start=time.monotonic()
try:
 env=dict(os.environ,XDG_CONFIG_HOME=str(r/'Build/UnderstoreyRecruitment/config'),CCF_MODEL2_OUTPUT=str(out/'evidence'),DISPLAY=os.environ.get('DISPLAY',':0'))
 (out/'evidence').mkdir(exist_ok=True)
 cmd=['systemd-run','--user','--scope','--quiet','--unit='+unit,'-p','MemoryMax=8G','-p','MemorySwapMax=256M','/media/jer/ZX20/Unity/6000.6.0f1/Editor/Unity','-projectPath',str(r),'-job-worker-count','2','-executeMethod',args.gate+'.Begin','-batchmode','-nographics','-logFile',str(log)]
 try:code=subprocess.run(cmd,cwd=r,env=env,timeout=3600).returncode
 except subprocess.TimeoutExpired:subprocess.run(['systemctl','--user','stop',unit+'.scope'],check=False);code=124
 text=log.read_text(errors='replace');markers=[x for x in text.splitlines() if '_PASS' in x or '_FAIL' in x or 'error CS' in x or '_ANCHOR' in x]
 status='PASS' if code==0 and args.gate.upper()+'_PASS' in text and not any('_FAIL' in x or 'error CS' in x for x in markers) else 'FAIL'
 result=dict(gate=args.gate,status=status,exit_code=code,seconds=time.monotonic()-start,source_sha256=hashlib.sha256(source.read_bytes()).hexdigest(),markers=markers)
 (out/(args.gate+'.result.json')).write_text(json.dumps(result,indent=2)+'\n');print(json.dumps(result),flush=True)
finally:target.unlink(missing_ok=True);Path(str(target)+'.meta').unlink(missing_ok=True)
