#!/usr/bin/env python3
"""Run task Unity operations with only the Manager-approved demo HDR held outside Assets.

Legacy S1-B helper for a project that has the FULL Ultimate Nature pack installed (its 8k demo HDR OOM-killed Editor imports).
Not needed for builds made with build_windows.py / build_linux.py: art_pack.py installs only the six required files, so there is
no HDR to quarantine (and this script stops with "missing source" when it is absent)."""
import hashlib,json,re,shutil,signal,subprocess,sys
from pathlib import Path
ROOT=Path(__file__).resolve().parents[3]
GUID='83d5c6c30d68b7e4498cfef8c6e2a35a'
HDR=Path('Assets/InnerverseInteractive/Ultimate Nature – Starter/Environment/Sky/Skyboxes/UNS_HDRI.hdr')
def sha(p):return hashlib.sha256(p.read_bytes()).hexdigest()
def editor_pattern(root):
 # pgrep -f (extended regex) for any Unity Editor whose -projectPath is this checkout; no machine-specific path.
 return r'(^|/)Unity .*-projectPath '+re.sub(r'([.^$*+?()\[\]{}|\\])',r'\\\1',str(root))+'( |$)'
def stop(signum,frame):raise SystemExit(128+signum)
def main():
 if len(sys.argv)<2:raise SystemExit('Usage: with_demo_hdr_quarantined.py COMMAND [ARGS]')
 signal.signal(signal.SIGTERM,stop);signal.signal(signal.SIGINT,stop)
 out=ROOT/'Build/S1B';quarantine=out/'demo-hdr-quarantine';quarantine.mkdir(parents=True,exist_ok=True)
 # Check committed CCF content, not the licensed pack's own demonstration assets.
 matches=subprocess.run(['git','grep','-l',GUID,'HEAD','--','Assets/Scenes','Assets/ForestPrototype','ProjectSettings'],cwd=ROOT,capture_output=True,text=True)
 if matches.returncode not in [0,1] or matches.stdout.strip():raise SystemExit('STOP: tracked CCF reference or failed dependency search: '+matches.stdout+matches.stderr)
 if re.search(r'^guid: (.+)$',(ROOT/Path(str(HDR)+'.meta')).read_text(),re.M).group(1)!=GUID:raise SystemExit('STOP: HDR meta GUID changed')
 records=[]
 for rel in [HDR,Path(str(HDR)+'.meta')]:
  source=ROOT/rel;dest=quarantine/rel.name
  if dest.exists() or not source.is_file():raise SystemExit('STOP: missing source or pre-existing quarantine: '+str(rel))
  records.append(dict(original=str(rel),quarantine=str(dest),sha256=sha(source),bytes=source.stat().st_size))
 report=dict(guid=GUID,tracked_ccf_reference_matches=0,files=records,restored=False)
 (out/'demo-hdr-quarantine-record.json').write_text(json.dumps(report,indent=2)+'\n')
 moved=[]
 try:
  for rec in records:
   shutil.move(ROOT/rec['original'],rec['quarantine']);moved.append(rec)
   assert sha(Path(rec['quarantine']))==rec['sha256']
  code=subprocess.run(sys.argv[1:],cwd=ROOT).returncode
 finally:
  # Do not restore a source while an interrupted Editor could reimport it.
  running=subprocess.run(['pgrep','-af',editor_pattern(ROOT)],capture_output=True,text=True).stdout
  if running.strip():raise RuntimeError('STOP: an Editor is still running on this project; preserved quarantine pending safe restoration: '+running)
  for rec in reversed(moved):
   dest=ROOT/rec['original'];assert not dest.exists(),dest
   shutil.move(rec['quarantine'],dest);assert sha(dest)==rec['sha256']
  report['restored']=True;(out/'demo-hdr-quarantine-record.json').write_text(json.dumps(report,indent=2)+'\n')
 raise SystemExit(code)
if __name__=='__main__':main()
