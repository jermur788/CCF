#!/usr/bin/env python3
"""Offline integration checks. All generated data stays in verified ZX20 scratch."""
import sys
sys.dont_write_bytecode=True
from concurrent.futures import ThreadPoolExecutor
from datetime import datetime,timedelta,timezone
import json
import os
from pathlib import Path
import subprocess
import tempfile

OPS=Path(__file__).resolve().parents[1]
sys.path.insert(0,str(OPS/'scripts'))
from _ccf import ROOT, BOARD_FIELDS, board_read, command, fields_read, storage_check, on_storage, storage_health, require_storage

storage_check()
TMP=on_storage(ROOT/'CCF-Temp')
os.environ.update(TMPDIR=str(TMP),TMP=str(TMP),TEMP=str(TMP),XDG_CACHE_HOME=str(TMP/'cache'),PYTHONDONTWRITEBYTECODE='1',GIT_OPTIONAL_LOCKS='0')
SCRATCH=Path(tempfile.mkdtemp(prefix='foundation-test-',dir=TMP))
print('Scratch:',SCRATCH,flush=True)
count=0

def check(condition,label):
    global count
    if not condition: raise AssertionError(label)
    count+=1; print('PASS:',label,flush=True)

def run(args,code=0):
    p=subprocess.run([str(v) for v in args],capture_output=True,text=True,env=os.environ)
    if p.returncode!=code: raise AssertionError(f'expected {code}, got {p.returncode}: {args}\n{p.stdout}\n{p.stderr}')
    return p.stdout

def cli(name,*args,code=0): return run([sys.executable,'-B',OPS/'scripts'/name,*args],code)

# Packet fixtures: fields, draft distinction, explicit Unity conflicts and headings.
f=OPS/'tests/fixtures'
check(cli('packet-lint',f/'valid.md').splitlines()[0]=='PASS','packet valid PASS')
for name in ['nonunity-lane-head','canonical-style']:
    check(cli('packet-lint',f/(name+'.md')).startswith('PASS'),name+' PASS without Unity inference')
for name in ['missing-unity-required','unity-nonunity-lane','stop-placeholder','handoff-placeholder','missing-role-file']:
    check(cli('packet-lint',f/(name+'.md'),code=1).startswith('FAIL'),name+' FAIL')
check(cli('packet-lint','--draft',f/'missing-unity-required.md').startswith('WARN'),'draft missing Unity declaration WARN')
check(cli('packet-lint',f/'valid-unity.md').splitlines()[0]=='PASS','packet valid Unity PASS')
unity_missing_lane=SCRATCH/'unity-missing-lane.md'
unity_missing_lane.write_text('\n'.join(l for l in (f/'valid-unity.md').read_text().splitlines() if not l.startswith('Unity lane:'))+'\n')
check(cli('packet-lint',unity_missing_lane,code=1).startswith('FAIL'),'Unity lane is required')
for name in ['missing-base','missing-verification-tier','conflicting-unity-ownership','missing-stop-condition']:
    check(cli('packet-lint',f/(name+'.md'),code=1).startswith('FAIL'),'packet '+name+' FAIL')
check(cli('packet-lint','--draft',f/'partial.md').startswith('WARN'),'partial draft WARN')
check(cli('packet-lint',f/'partial.md',code=1).startswith('FAIL'),'partial final FAIL')
check(cli('packet-lint','--draft',f/'conflicting-unity-ownership.md',code=1).startswith('FAIL'),'draft does not hide explicit Unity conflict')
packet=SCRATCH/'packet.md'
packet.write_text((f/'valid.md').read_text().replace('Base: c6dc136b7f1f8668d9f67f95f48968d290ad5067','Base: invalid'))
check(cli('packet-lint',packet,code=1).startswith('FAIL'),'malformed SHA FAIL')
packet.write_text('\n'.join('## '+l.split(':',1)[0]+'\n'+l.split(':',1)[1].strip() for l in (f/'valid.md').read_text().splitlines()))
check(cli('packet-lint',packet).startswith('PASS'),'Markdown heading packet PASS')
check(cli('packet-lint','--board',OPS/'board/active.md').startswith('PASS'),'live board schema PASS')

# Independent tiny test repository; no fixture refs/index/config on SSD.
seed=SCRATCH/'seed'; run(['git','init','-b','test/seed',seed])
for key,value in [('user.name','CCF Offline Test'),('user.email','test@invalid.local'),('commit.gpgSign','false'),('core.hooksPath','/dev/null'),('core.filemode','false')]: run(['git','-C',seed,'config',key,value])
(seed/'fixture.txt').write_text('clean\n'); run(['git','-C',seed,'add','fixture.txt']); run(['git','-C',seed,'commit','-m','isolated test fixture'])
clean=SCRATCH/'clean'; dirty=SCRATCH/'dirty'; missing=SCRATCH/'missing'
run(['git','-C',seed,'worktree','add','-b','test/clean',clean]); run(['git','-C',seed,'worktree','add','-b','test/dirty',dirty]); (dirty/'fixture.txt').write_text('dirty\n')
# Add a local upstream for cheap ahead/behind; no fetch/network.
run(['git','-C',clean,'branch','--set-upstream-to=test/seed'])
board=SCRATCH/'board.md'; board.write_text('| '+' | '.join(BOARD_FIELDS)+' |\n|'+'---|'*len(BOARD_FIELDS)+'\n')
config=SCRATCH/'lanes.json'
cfg={'schema':1,'storage':{'root':str(ROOT),'uuid':'18D6-896E','fstype':'exfat','device':'/dev/sda2','minimum_free_bytes':15*1024**3},'repository':str(seed/'.git'),'git_stores':[{'name':'fixture','git_dir':str(seed/'.git')}],'board':'board.md','lanes':[{'lane':n,'path':str(p),'purpose':'isolated test','unity':n=='U1'} for n,p in [('U1',clean),('dirty',dirty),('missing',missing),('NonUnity',seed)]]}
config.write_text(json.dumps(cfg))
# Byte content AND mtimes of fixture Git metadata must not change under inventory.
metadata=list((seed/'.git').rglob('*'))
before={str(p):(p.stat().st_mtime_ns,p.read_bytes()) for p in metadata if p.is_file()}
rows=json.loads(cli('lane-status','--config',config,'--json'))
check(rows[0]['exists'] and rows[0]['registered_worktree'] and rows[0]['dirty'] is False,'lane clean')
check(rows[0]['ahead_behind']==['0','0'],'lane ahead/behind local refs')
check(rows[0]['zx20'] is True and rows[0]['backing_device']=='/dev/sda2','lane verifies backing device and ZX20 YES')
check(rows[2]['zx20'] is True and rows[2]['backing_device']=='/dev/sda2','absent lane reports nearest-parent ZX20 device')
check(rows[1]['dirty'] is True,'lane dirty is data, exit zero')
check(not rows[2]['exists'] and not rows[2]['registered_worktree'],'lane missing tolerated')
check(before=={str(p):(p.stat().st_mtime_ns,p.read_bytes()) for p in metadata if p.is_file()},'lane-status leaves Git metadata bytes/mtimes unchanged')
bad=json.loads(json.dumps(cfg)); bad['lanes'].append(bad['lanes'][0]); config.write_text(json.dumps(bad))
p=subprocess.run([sys.executable,'-B',str(OPS/'scripts/lane-status'),'--config',str(config)],capture_output=True,text=True)
check(p.returncode==2 and 'duplicate' in p.stderr,'malformed duplicate lane config fails clearly')
config.write_text('{not-json')
p=subprocess.run([sys.executable,'-B',str(OPS/'scripts/lane-status'),'--config',str(config)],capture_output=True,text=True)
check(p.returncode==2 and 'ERROR:' in p.stderr,'malformed JSON fails clearly')
config.write_text(json.dumps(cfg))
for malformed in [[], {'schema':1,'storage':[],'lanes':[{}]}, dict(cfg,lanes=[42])]:
    config.write_text(json.dumps(malformed))
    p=subprocess.run([sys.executable,'-B',str(OPS/'scripts/lane-status'),'--config',str(config)],capture_output=True,text=True)
    check(p.returncode==2 and p.stderr.startswith('ERROR:') and 'Traceback' not in p.stderr,'malformed config shape fails clearly')
config.write_text(json.dumps(cfg))
# Read-only negative config: observe existing /; create no files or worktree there.
off=json.loads(json.dumps(cfg)); off['lanes'].append({'lane':'off-storage','path':'/','purpose':'read-only negative sentinel','unity':False})
config.write_text(json.dumps(off))
off_rows=json.loads(cli('lane-status','--config',config,'--json'))
check(off_rows[-1]['zx20'] is False and off_rows[-1]['backing_device']=='/dev/sdb2','off-storage reports actual root device and ZX20 NO')
check('ZX20: NO' in cli('lane-status','--config',config),'off-storage human output clearly says ZX20: NO')
check(off_rows[-1]['HEAD'] is None and any('STOP' in w for w in off_rows[-1]['conflict_warning']),'off-storage warning stops writable-lane inspection')
config.write_text(json.dumps(cfg))
broken_board=SCRATCH/'broken-board.md'; broken_board.write_text('| ID | State |\n|---|---|\n')
check(cli('packet-lint','--board',broken_board,code=1).startswith('FAIL'),'board missing required fields FAIL')

# Storage admission and board checks.
health=storage_health(cfg['storage'])
check(health['ready'] and health['writable'] and health['free_bytes']>=15*1024**3,'actual ZX20 rw/free PASS')
check(storage_health(cfg['storage'],free_bytes=0)['status']=='LOW SPACE','shared low-space admission BLOCK')
ro={'source':'/dev/sda2','target':str(ROOT),'fstype':'exfat','uuid':'18D6-896E','options':'ro,nosuid'}
check(storage_health(cfg['storage'],mount_info=ro)['status']=='READ ONLY','read-only mount admission BLOCK')
low=json.loads(cli('lane-status','--storage-only','--json','--config',config,'--test-free-bytes','0',code=1))
check(low['status']=='LOW SPACE' and not low['ready'] and low['minimum_free_bytes']==15*1024**3,'lane-status low-space hard stop')
check('PASS:' in run(['bash','-c','source "$1"','ccf-test',OPS/'environment.sh']),'environment.sh actual rw/free PASS')
# No fixture store or branch is production data.
second=SCRATCH/'second-store'
run(['git','clone','--no-hardlinks',seed,second])
run(['git','-C',second,'checkout','-b','test/clean'])
dual=json.loads(json.dumps(cfg));dual['git_stores'].append({'name':'second','git_dir':str(second/'.git')});config.write_text(json.dumps(dual))
blocked=cli('lane-status','--config',config,'--preflight','--lane','U1','--branch','test/clean',code=1)
check('BLOCKED' in blocked and 'another writable checkout' in blocked,'dual-store branch collision blocks preflight')
run(['git','-C',second,'checkout','test/seed'])
check(cli('lane-status','--config',config,'--preflight','--lane','U1','--branch','test/clean').rstrip().endswith('READY'),'nonconflicting assignment READY')
config.write_text(json.dumps(cfg))
# Duplicate board claims fail mechanically; no board authority is promoted.
row=['DUP-1','ACTIVE','fixture','Codex','test','test','U1','test/clean',command(['git','-C',str(seed),'rev-parse','HEAD']),'fixture','fixture only','No','None','Tier 1','fixture','None','test']
for duplicate_field in ['Lane','Branch']:
    other=list(row);other[0]='DUP-2'
    other[BOARD_FIELDS.index('Branch' if duplicate_field=='Lane' else 'Lane')]='other'
    board.write_text('| '+' | '.join(BOARD_FIELDS)+' |\n|'+'---|'*len(BOARD_FIELDS)+'\n| '+' | '.join(row)+' |\n| '+' | '.join(other)+' |\n')
    check('duplicate live '+duplicate_field in cli('packet-lint','--config',config,'--board',board,code=1),'duplicate board '+duplicate_field+' FAIL')
board.write_text('| '+' | '.join(BOARD_FIELDS)+' |\n|'+'---|'*len(BOARD_FIELDS)+'\n')

# Lock exercises only isolated test state; never acquire production lock.
state=SCRATCH/'lock-state'
base=['--test-state-dir',str(state),'--config',str(config)]
head=command(['git','-C',str(seed),'rev-parse','HEAD'])
eta=(datetime.now(timezone.utc)+timedelta(minutes=20)).isoformat()
acquire=['acquire',*base,'--owner','test-owner','--task-id','TEST','--lane',str(clean),'--branch','test/clean','--head',head,'--eta',eta,'--notes','isolated offline self-test','--json']
def lock(*args,code=0): return cli('unity-lock',*args,code=code)
check(json.loads(lock('status',*base,'--json'))['status']=='free','lock initial status free')
for args,label in [(['--lane','NonUnity'],'NonUnity lane rejected'),(['--branch','wrong-branch'],'wrong branch rejected'),(['--head','0'*40],'wrong HEAD rejected')]:
    lock(*acquire,*args,code=2);check(True,label)
held=json.loads(lock(*acquire)); check(held['status']=='held','lock acquire')
check(json.loads(lock('status',*base,'--json'))['metadata']['owner']=='test-owner','lock status metadata')
lock(*acquire,'--owner','second-owner',code=2); check(True,'lock second owner rejected')
lock('release',*base,'--owner','wrong-owner','--task-id','TEST','--lock-id',held['metadata']['lock_id'],code=2); check((state/'lock.json').exists(),'lock wrong owner cannot release')
lock('release',*base,'--owner','test-owner','--task-id','TEST','--lock-id',held['metadata']['lock_id'])
check(json.loads(lock('status',*base,'--json'))['status']=='free','lock owner release')

# Start a fixture process and let it exit naturally; never kill any process.
proc=subprocess.Popen([sys.executable,'-B','-c','import sys; sys.stdin.read()'],stdin=subprocess.PIPE)
try:
    held=json.loads(lock(*acquire,'--pid',str(proc.pid)))
finally:
    proc.stdin.close(); proc.wait(timeout=5)
stale=json.loads(lock('status',*base,'--json'))
check(stale['status']=='stale' and stale['metadata']['pid']==proc.pid,'lock stale PID inspection')
lock(*acquire,code=2); check(True,'stale record still rejects acquire')
lock('release',*base,'--force','--owner','test-inspector',code=2); check((state/'lock.json').exists(),'force release requires reason')
lock('release',*base,'--force','--owner','test-inspector','--reason','fixture exited',code=2)
check((state/'lock.json').exists(),'force without Manager approval reference rejected')
lock('release',*base,'--force','--owner','test-inspector','--reason','fixture PID exited; no Unity was launched','--manager-approval','user-packet:isolated-force-test')
events=[json.loads(l) for l in (state/'history.jsonl').read_text().splitlines()]
check(events[-1]['event']=='force-release' and events[-1]['previous']==stale['metadata'] and events[-1]['inspection']['status']=='stale' and events[-1]['manager_approval']=='user-packet:isolated-force-test','force release audits complete previous state')
(state/'lock.json').write_text('{broken fixture metadata')
check(json.loads(lock('status',*base,'--json'))['status']=='malformed','malformed stale metadata inspectable')
lock(*acquire,code=2)
lock('release',*base,'--force','--owner','test-inspector','--reason','isolated malformed fixture','--manager-approval','user-packet:isolated-force-test')
check(json.loads((state/'history.jsonl').read_text().splitlines()[-1])['previous']['raw']=='{broken fixture metadata','force release preserves malformed raw state')

with ThreadPoolExecutor(max_workers=6) as pool:
    attempts=list(pool.map(lambda _:subprocess.run([sys.executable,'-B',str(OPS/'scripts/unity-lock'),*acquire],capture_output=True,text=True),range(6)))
check(sum(p.returncode==0 for p in attempts)==1 and sum(p.returncode==2 for p in attempts)==5,'atomic concurrent acquisition: exactly one winner')
held=json.loads(lock('status',*base,'--json'))
lock('release',*base,'--owner','test-owner','--task-id','TEST','--lock-id',held['metadata']['lock_id'])
check(json.loads(lock('status',*base,'--json'))['status']=='free','isolated lock free after tests')
p=subprocess.run([sys.executable,'-B',str(OPS/'scripts/unity-lock'),'status','--test-state-dir',str(ROOT/'CCF-Verification/invalid-test-lock')],capture_output=True,text=True)
check(p.returncode==2,'test lock override cannot escape scratch')

# A tiny dummy workflow; every stage is explicitly simulated.
pilot=SCRATCH/'pilot'; pilot.mkdir(); board=pilot/'active.md'
header='| '+' | '.join(BOARD_FIELDS)+' |\n|'+'---|'*len(BOARD_FIELDS)+'\n'
source_repo=OPS.parent
if (source_repo/'.git').exists():
    source_head=command(['git','-C',str(source_repo),'rev-parse','HEAD'])
    source_branch=command(['git','-C',str(source_repo),'rev-parse','--abbrev-ref','HEAD'])
else:
    deployment=json.loads((OPS/'deployment.json').read_text())
    source_head=deployment['source_head']; source_branch=deployment['source_branch']
packet_text=(OPS/'pilot/manager-packet.md').read_text()
check(cli('packet-lint',OPS/'pilot/manager-packet.md').startswith('PASS'),'pilot manager packet lint')
# Production lane inventory is read-only and archived in the pilot receipt.
(pilot/'lane-status.json').write_text(cli('lane-status','--json'))
values=['CCF-DUMMY-T1','ACTIVE','simulated worker','Codex','GPT-6.1 Sol','OpenAI allowance','NonUnity',source_branch,source_head,'operations dummy','isolated fixture only','No','None','Tier 1','mock handoff','None','produce simulated handoff']
def snapshot(state,action):
    values[1]=state; values[-1]=action
    board.write_text(header+'| '+' | '.join(values)+' |\n'); board_read(board)
    (pilot/(state+'.md')).write_text(board.read_text())
snapshot('ACTIVE','produce simulated handoff')
(pilot/'worker-handoff.md').write_text(f'SIMULATED WORKER HANDOFF\nBranch: {source_branch}\nBase: c6dc136b7f1f8668d9f67f95f48968d290ad5067\nContext: c6dc136b7f1f8668d9f67f95f48968d290ad5067\nExact HEAD: {source_head}\nResult: no implementation; fixture-only handoff\n')
snapshot('VERIFY','mock verify packet fields and fixture handoff')
check((pilot/'worker-handoff.md').exists() and len(board_read(board))==1,'pilot ACTIVE → handoff → VERIFY')
ledger=f'''Task ID: CCF-DUMMY-T1
Purpose: Prove offline board/handoff/ledger mechanics using a simulation.
Worker route: Codex / GPT-6.1 Sol / OpenAI allowance
Base: c6dc136b7f1f8668d9f67f95f48968d290ad5067
Final HEAD: {source_head}
Verification: Packet lint; read-only lane status; board schema; fixture handoff presence; mock verdict.
Verdict: SIMULATED PASS; no independent gate or production acceptance.
Integrated?: Simulated
Canonical impact: None
Evidence path: {pilot}
Date: {datetime.now(timezone.utc).date()}
'''
(pilot/'ledger.md').write_text(ledger)
required={'task id','purpose','worker route','base','final head','verification','verdict','integrated?','canonical impact','evidence path','date'}
check(required.issubset(fields_read(ledger)),'pilot closed ledger required fields')
for entry in (OPS/'ledger').glob('*.md'):
    if entry.name!='README.md': check(required.issubset(fields_read(entry.read_text())),'stored ledger schema: '+entry.name)
board.write_text(header)
check(board_read(board)==[] and (pilot/'ledger.md').exists(),'pilot ledger exists before board close')
result={'result':'PASS','checks':count,'scratch':str(SCRATCH),'pilot_evidence':str(pilot),'source_head':source_head,'unity_launched':False,'production_lock_acquired':False}
(SCRATCH/'result.json').write_text(json.dumps(result,indent=2)+'\n')
print(json.dumps(result,indent=2),flush=True)
