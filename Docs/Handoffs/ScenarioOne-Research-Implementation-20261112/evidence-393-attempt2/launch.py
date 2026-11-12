import subprocess,os,shutil,sys,json
from pathlib import Path
r=Path(__file__).parent;lane=Path('/media/jer/ZX20/CCF-S1-ResearchCopy-V1-Qual')
assert shutil.disk_usage(lane).free>=16106127360
(r/(sys.argv[1]+'-disk.json')).write_text(json.dumps({'free_bytes':shutil.disk_usage(lane).free}))
cmd=['python3','-I','-B',str(r.parent/'run393.py'),'--name','focused','--method','Ccf393.Begin','--files','ScenarioOneTeachingCopyVerification.cs','--ext-files',str(r/'Ccf393.cs'),'--out',str(r/sys.argv[1]),'--env','CCF393_FIXTURE='+str(r/'fixture.json'),'--pass-token','C393_DONE','--fail-token','C393_BLACK','--timeout','600']
(r/(sys.argv[1]+'-command.json')).write_text(json.dumps(cmd))
sys.exit(subprocess.call(cmd,env=dict(os.environ,CCF_GATE_LANE=str(lane))))
