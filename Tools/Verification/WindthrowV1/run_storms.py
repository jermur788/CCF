#!/usr/bin/env python3
"""One explicit disposable storm diagnostic with isolated configuration."""
import argparse,fcntl,hashlib,json,os,shutil,subprocess,time
from pathlib import Path
os.environ.setdefault('CCF_STAND_GEOMETRY', '0')  # historical anchors run in the explicit Editor-only Legacy40 override (D-056); export CCF_STAND_GEOMETRY=1 to run them in Enlarged80 deliberately
r=Path(__file__).resolve().parents[3]
p=argparse.ArgumentParser();p.add_argument('--gate',choices=['WindDisplayAudit','StormCoreVerification','WindthrowVisualVerification','StormForcedMatrix','StormSalvageVerification','StormLongRunCalibration','StormMathVerification','StormPerformanceVerification','StormReplayVerification','StormRecruitmentVerification','StormLongRunResume','StormUiVerification','StormRngWrite','StormRngReplay'],default='WindDisplayAudit');p.add_argument('--recent',action='store_true',help='Apply thinning immediately before each forced checkpoint');p.add_argument('--pilot',action='store_true');args=p.parse_args()
out=r/'Build/WindthrowV1'/(args.gate+('Recent' if args.recent else '')+('Pilot' if args.pilot else ''));out.mkdir(parents=True,exist_ok=True)
lock=(r/'Build/UnderstoreyRecruitment/launch.lock').open('w');fcntl.flock(lock,fcntl.LOCK_EX|fcntl.LOCK_NB)
source=Path(__file__).with_name(args.gate+'.cs');target=r/'Assets/ForestPrototype'/source.name;assert not target.exists(),target
def production_manifest():
 files=[path for path in (r/'Assets/ForestPrototype').rglob('*.cs') if path!=target]
 files += [Path(str(path)+'.meta') for path in list(files) if Path(str(path)+'.meta').exists()]
 files += list((r/'Assets/ForestPrototype/ScenarioOne/Resources').glob('WindthrowVisualCatalog.asset*'))
 return {str(path.relative_to(r)):hashlib.sha256(path.read_bytes()).hexdigest() for path in sorted(files)}
launch_manifest=production_manifest()
launch_head=subprocess.check_output(['git','rev-parse','HEAD'],cwd=r).decode().strip()
(out/'production_at_launch.json').write_text(json.dumps(launch_manifest,indent=2)+'\n')
shutil.copy2(source,target);unit='ccf-storm-diagnostic-'+str(time.time_ns());log=out/(args.gate+'.log');start=time.monotonic()
try:
 env=dict(os.environ,XDG_CONFIG_HOME=str(r/'Build/UnderstoreyRecruitment/config'),CCF_STORM_OUTPUT=str(out/'evidence'),DISPLAY=os.environ.get('DISPLAY',':0'),CCF_STORM_RECENT='1' if args.recent else '0',CCF_STORM_PILOT='1' if args.pilot else '0')
 (out/'evidence').mkdir(exist_ok=True)
 cmd=['systemd-run','--user','--scope','--quiet','--unit='+unit,'-p','MemoryMax=8G','-p','MemorySwapMax=256M','/media/jer/ZX20/Unity/6000.6.0f1/Editor/Unity','-projectPath',str(r),'-job-worker-count','2','-executeMethod',args.gate+'.Begin',*(['-batchmode','-nographics'] if args.gate not in ('WindthrowVisualVerification','StormUiVerification') else []),'-logFile',str(log)]
 try:code=subprocess.run(cmd,cwd=r,env=env,timeout=7200).returncode
 except subprocess.TimeoutExpired:subprocess.run(['systemctl','--user','stop',unit+'.scope'],check=False);code=124
 text=log.read_text(errors='replace') if log.exists() else ''
 markers=[line for line in text.splitlines() if any(key in line for key in ['_PASS','_FAIL','_DISTRIBUTION','error CS'])]
 marker={'WindDisplayAudit':'WIND_DISPLAY_AUDIT_PASS','StormCoreVerification':'STORM_CORE_VERIFICATION_PASS','WindthrowVisualVerification':'WINDTHROW_VISUAL_VERIFICATION_PASS','StormForcedMatrix':'STORM_FORCED_MATRIX_PASS','StormSalvageVerification':'STORM_SALVAGE_VERIFICATION_PASS','StormLongRunCalibration':'STORM_LONG_RUN_CALIBRATION_PASS','StormMathVerification':'STORM_MATH_VERIFICATION_PASS','StormPerformanceVerification':'STORM_PERFORMANCE_VERIFICATION_PASS','StormReplayVerification':'STORM_REPLAY_VERIFICATION_PASS','StormRecruitmentVerification':'STORM_RECRUITMENT_VERIFICATION_PASS','StormLongRunResume':'STORM_LONG_RUN_RESUME_PASS','StormUiVerification':'STORM_UI_VERIFICATION_PASS','StormRngWrite':'STORM_RNG_WRITE_PASS','StormRngReplay':'STORM_RNG_REPLAY_PASS'}[args.gate]
 final_manifest=production_manifest()
 (out/'production_at_finish.json').write_text(json.dumps(final_manifest,indent=2)+'\n')
 source_unchanged=launch_manifest==final_manifest
 passed=code==0 and marker in text and source_unchanged and not any('_FAIL' in line or 'error CS' in line for line in markers)
 result=dict(launch_head=launch_head,production_source_unchanged=source_unchanged,gate=args.gate,treatment_timing='recent' if args.recent else 'early-established',status='PASS' if passed else 'FAIL',exit_code=code,seconds=round(time.monotonic()-start,2),source_sha256=hashlib.sha256(source.read_bytes()).hexdigest(),run_head=subprocess.check_output(['git','rev-parse','HEAD'],cwd=r).decode().strip(),run_branch=subprocess.check_output(['git','branch','--show-current'],cwd=r).decode().strip(),markers=markers)
 (out/'result.json').write_text(json.dumps(result,indent=2)+'\n');print(json.dumps(result),flush=True)
finally:target.unlink(missing_ok=True);Path(str(target)+'.meta').unlink(missing_ok=True)
