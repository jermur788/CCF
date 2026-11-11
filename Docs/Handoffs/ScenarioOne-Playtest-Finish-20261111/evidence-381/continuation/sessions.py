from pathlib import Path
import subprocess as s, json, hashlib,os,datetime
r=Path(__file__).parent; lane=Path('/media/jer/ZX20/CCF-S1-UI-Presentation-V1-Qual'); lock='/media/jer/ZX20/CCF-Ops/scripts/unity-lock';head='2aaacad68ed186dbe14531f31ce5bb3a87811441'
def cmd(a,**kw): return s.check_output(a,text=True,**kw)
def audit():
 old=json.loads((r/'custody-before.json').read_text());now={p:hashlib.sha256(Path(p).read_bytes()).hexdigest() if Path(p).is_file() else None for p in old};assert now==old,'custody changed';return now
def finish(name):
 status=json.loads(cmd(['python3','-B',lock,'status','--json']));assert not status['unity_editor_pids']
 changed=cmd(['git','diff','--name-only'],cwd=lane).splitlines();(r/(name+'-import.patch')).write_text(cmd(['git','diff','--binary'],cwd=lane))
 for f in changed:
  assert f=='.vscode/settings.json' or (f.startswith('Assets/ForestPrototype/Art/SectionFive/') and '/Materials/' in f and f.endswith('.mat')),f
  (lane/f).write_bytes(s.check_output(['git','show','HEAD:'+f],cwd=lane))
 assert not cmd(['git','status','--porcelain','-uall'],cwd=lane).strip()
 (r/(name+'-custody-after.json')).write_text(json.dumps(audit(),indent=2))
 m=status['metadata'];cmd(['python3','-B',lock,'release','--owner','Sol381','--task-id','CCF-381-UI-QUALIFICATION','--lock-id',m['lock_id'],'--reason','session complete','--json'])
 (r/(name+'-released.json')).write_text(cmd(['python3','-B',lock,'status','--json']))
if __name__=='__main__':
 import sys
 finish(sys.argv[1])
