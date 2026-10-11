from pathlib import Path
import subprocess,os,json,time,shutil,hashlib,re,sys
r=Path(__file__).resolve().parents[3];out=r/'Build/SaveCellIndexHardening';out.mkdir(parents=True,exist_ok=True);unity='/media/jer/ZX20/Unity/6000.6.0f1/Editor/Unity'
config=out/'config';shutil.copytree(Path.home()/'.config/unity3d/Unity/licenses',config/'unity3d/Unity/licenses',dirs_exist_ok=True);env=dict(os.environ,XDG_CONFIG_HOME=str(config));
for k in ['CCF_STAND_GEOMETRY','CCF_STARTUP_SCREEN','CCF_STARTUP_INPUT','CCF_STARTUP_BOUNDARY']:env.pop(k,None)
def digest(p):return hashlib.sha256(p.read_bytes()).hexdigest() if p.exists() else None
saved={str(p):digest(p) for p in [Path.home()/'.config/unity3d/DefaultCompany/CCF'/n for n in ['forest-save.json','forest-save.json.bak','forest-save.json.tmp']]}
(out/'original-saves.json').write_text(json.dumps(saved,indent=2))
results=[]
def audit(log):
 text=log.read_text(errors='replace')
 assert not re.search(r'error CS\d+|Failed to import|Import failed|Importer.*(?:error|failed)',text),str(log)
 signature=json.loads((r/'Docs/Verification/Enlarged80Calibration/editor-search-baseline.json').read_text())['signature']
 for m in re.finditer(r'^(?:[\w.]+Exception):.*$',text,re.M):
  block='\n'.join(text[m.start():].splitlines()[:10]);assert block==signature,('Unexpected exception',str(log),block)
 return text

def gate(label):
 assert not subprocess.run(['pgrep','-f','^'+unity+' '],capture_output=True,text=True).stdout.strip()
 target=r/'Assets/ForestPrototype/SaveCellIndexHardeningVerification.cs';assert not target.exists()
 source=r/'Tools/Verification/SaveCellIndexHardening/SaveCellIndexHardeningVerification.cs';shutil.copy2(source,target)
 log=out/(label+'.log');start=time.monotonic();unit='ccf-cell-'+str(time.time_ns());print('START '+label,flush=True)
 try:
  cmd=['systemd-run','--user','--scope','--quiet','--unit='+unit,'-p','MemoryMax=8G','-p','MemorySwapMax=256M',unity,'-projectPath',str(r),'-job-worker-count','2','-executeMethod','SaveCellIndexHardeningVerification.Begin','-batchmode','-nographics','-logFile',str(log)]
  try:code=subprocess.run(cmd,cwd=r,env=env,timeout=1800).returncode
  except subprocess.TimeoutExpired:subprocess.run(['systemctl','--user','stop',unit+'.scope']);raise
 finally:target.unlink(missing_ok=True);Path(str(target)+'.meta').unlink(missing_ok=True)
 text=audit(log);passed=code==0 and 'SAVE_CELL_INDEX_HARDENING_VERIFY_PASS' in text and 'SAVE_CELL_STARTUP_PASS' in text and 'VERIFY_FAIL' not in text
 result=dict(gate=label,exit_code=code,status='PASS' if passed else 'FAIL',seconds=round(time.monotonic()-start,2),source_sha256=digest(source),markers=[l for l in text.splitlines() if l.startswith(('SAVE_CELL_','SAVE_CELL_INDEX_'))]);results.append(result);(out/'new-gate-results.json').write_text(json.dumps(results,indent=2))
 print(result['status']+' '+label,flush=True);assert passed,('Gate failed',label)
 assert all(digest(Path(p))==h for p,h in saved.items()),'Original saves changed'
for label in ['run-1','run-2']:gate(label)
print('SAVE_CELL_NEW_GATE_PASS_X2',flush=True)
