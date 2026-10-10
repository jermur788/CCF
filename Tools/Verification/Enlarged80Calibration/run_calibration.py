#!/usr/bin/env python3
"""Run one disposable calibration/viability gate, always removing staged source/meta.
Uses an independent Library, isolated config and bounded Editor memory. No saved scenes.
"""
import argparse, json, os, shutil, subprocess, time
from pathlib import Path
ROOT=Path(__file__).resolve().parents[3]
UNITY='/media/jer/ZX20/Unity/6000.6.0f1/Editor/Unity'
p=argparse.ArgumentParser();p.add_argument('--gate',choices=['AreaCalibrationVerification','Enlarged80ViabilityVerification','AreaCalibrationReview'],required=True);p.add_argument('--label',required=True);args=p.parse_args()
busy=subprocess.run(['pgrep','-f','^'+UNITY+' '],capture_output=True,text=True).stdout.split()
if busy:raise SystemExit('Another Unity Editor is running: '+str(busy))
OUT=ROOT/'Build/Enlarged80Calibration';OUT.mkdir(parents=True,exist_ok=True)
config=OUT/'config';shutil.copytree(Path.home()/'.config/unity3d/Unity/licenses',config/'unity3d/Unity/licenses',dirs_exist_ok=True)
target=ROOT/'Assets/ForestPrototype'/(args.gate+'.cs');assert not target.exists()
log=OUT/(args.label+'.log');unit='ccf-e80-calibration-'+str(time.time_ns())
cmd=['systemd-run','--user','--scope','--quiet','--unit='+unit,'-p','MemoryMax=8G','-p','MemorySwapMax=256M',UNITY,'-projectPath',str(ROOT),'-job-worker-count','2','-executeMethod',args.gate+'.Begin','-batchmode','-nographics','-logFile',str(log)]
env=dict(os.environ,XDG_CONFIG_HOME=str(config));env.pop('CCF_STAND_GEOMETRY',None)
if args.gate=='AreaCalibrationReview':
 cmd.remove('-batchmode');cmd.remove('-nographics');env['DISPLAY']=os.environ.get('DISPLAY',':0');env['CCF_ACCEPTANCE_OUTPUT']=str(OUT/(args.label+'-captures'))
start=time.monotonic();shutil.copy2(Path(__file__).with_name(args.gate+'.cs'),target)
try:
 try: code=subprocess.run(cmd,cwd=ROOT,env=env,timeout=1800).returncode
 except subprocess.TimeoutExpired:subprocess.run(['systemctl','--user','stop',unit+'.scope'],check=False);code=124
finally:target.unlink(missing_ok=True);Path(str(target)+'.meta').unlink(missing_ok=True)
text=log.read_text(errors='replace') if log.exists() else '';markers=[l for l in text.splitlines() if any(x in l for x in ['AREA_CALIBRATION','E80_VIABILITY','E80_ECONOMY','error CS'])];expected='AREA_CALIBRATION_VERIFY_PASS' if args.gate=='AreaCalibrationVerification' else 'AREA_CALIBRATION_REVIEW_PASS' if args.gate=='AreaCalibrationReview' else 'E80_VIABILITY_VERIFY_PASS'
result=dict(gate=args.gate,status='PASS' if code==0 and expected in text and not any('_FAIL' in l or 'error CS' in l for l in markers) else 'FAIL',exit_code=code,seconds=round(time.monotonic()-start,2),markers=markers)
(OUT/(args.label+'.json')).write_text(json.dumps(result,indent=2)+'\n');print(json.dumps(result),flush=True);raise SystemExit(0 if result['status']=='PASS' else 1)
