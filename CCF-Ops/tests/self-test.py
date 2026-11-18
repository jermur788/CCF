#!/usr/bin/env python3
"""Offline integration checks. Isolated ZX20 fixtures; owned temporary Files lane is removed in finally."""
import sys
sys.dont_write_bytecode=True
from concurrent.futures import ThreadPoolExecutor
from datetime import datetime,timedelta,timezone
import json
import os
import stat
import shutil
from pathlib import Path
import subprocess
import tempfile
import runpy
from unittest.mock import patch

OPS=Path(__file__).resolve().parents[1]
sys.path.insert(0,str(OPS/'scripts'))
from _ccf import ROOT, BOARD_FIELDS, board_read, command, fields_read, storage_check, on_storage, storage_health, require_storage, DURABLE_CCF, config_read

storage_check()
DEVICE=command(['findmnt','-n','-o','SOURCE','-T',str(ROOT)])
TMP=on_storage(ROOT/'CCF-Temp')
os.environ.update(TMPDIR=str(TMP),TMP=str(TMP),TEMP=str(TMP),XDG_CACHE_HOME=str(TMP/'cache'),PYTHONDONTWRITEBYTECODE='1',GIT_OPTIONAL_LOCKS='0')
SCRATCH=Path(tempfile.mkdtemp(prefix='foundation-test-',dir=TMP))
print('Scratch:',SCRATCH,flush=True)
count=0

def check(condition,label):
    global count
    if not condition: raise AssertionError(label)
    count+=1; print('PASS:',label,flush=True)

def backing_device_matches(device,path):
    # Compare a block device with the guarded filesystem, not its kernel name.
    try:
        info=os.stat(device)
        return stat.S_ISBLK(info.st_mode) and info.st_rdev==os.stat(path).st_dev
    except OSError:
        return False

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
cfg={'schema':1,'storage':{'root':str(ROOT),'uuid':'18D6-896E','fstype':'exfat','device':DEVICE,'minimum_free_bytes':15*1024**3},'repository':str(seed/'.git'),'git_stores':[{'name':'fixture','git_dir':str(seed/'.git')}],'board':'board.md','lanes':[{'lane':n,'path':str(p),'purpose':'isolated test','unity':n=='U1'} for n,p in [('U1',clean),('dirty',dirty),('missing',missing),('NonUnity',seed)]]}
config.write_text(json.dumps(cfg))
# Byte content AND mtimes of fixture Git metadata must not change under inventory.
metadata=list((seed/'.git').rglob('*'))
before={str(p):(p.stat().st_mtime_ns,p.read_bytes()) for p in metadata if p.is_file()}
rows=json.loads(cli('lane-status','--config',config,'--json'))
check(rows[0]['exists'] and rows[0]['registered_worktree'] and rows[0]['dirty'] is False,'lane clean')
check(rows[0]['ahead_behind']==['0','0'],'lane ahead/behind local refs')
check(rows[0]['zx20'] is True and backing_device_matches(rows[0]['backing_device'],ROOT),'lane verifies backing device and ZX20 YES')
check(rows[2]['zx20'] is True and backing_device_matches(rows[2]['backing_device'],ROOT),'absent lane reports nearest-parent ZX20 device')
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
# Out-of-bounds inventory must fail at admission, before writable state exists.
off=json.loads(json.dumps(cfg)); off['lanes'].append({'lane':'off-storage','path':'/','purpose':'read-only negative sentinel','unity':False})
config.write_text(json.dumps(off))
p=subprocess.run([sys.executable,'-B',str(OPS/'scripts/lane-status'),'--config',str(config),'--json'],capture_output=True,text=True)
check(p.returncode==2 and 'outside the Files CCF tree: /' in p.stderr and not p.stdout,'arbitrary root rejected for the specific path boundary')
# Actual state-writing CLI: invalid config must fail before guard creation.
rejected_state=SCRATCH/'rejected-config-state'
p=subprocess.run([sys.executable,'-B',str(OPS/'scripts/unity-lock'),'acquire','--config',str(config),'--test-state-dir',str(rejected_state)],capture_output=True,text=True)
check(p.returncode==2 and 'outside the Files CCF tree: /' in p.stderr and not rejected_state.exists(),'out-of-bounds lock config creates no state or guard')
config.write_text(json.dumps(cfg))
p=subprocess.run([sys.executable,'-B',str(OPS/'scripts/unity-lock'),'acquire','--config',str(config),'--test-state-dir',str(rejected_state)],capture_output=True,text=True)
check(p.returncode==2 and (rejected_state/'guard').exists(),'positive control reaches guard creation with valid config')
# Explicit read-only inventory admits a bounded Files CCF non-Unity lane.
files_fixture=Path(tempfile.mkdtemp(prefix='ops652-files-fixture-',dir=DURABLE_CCF/'Verification'))
files_lane=files_fixture/'lane'
try:
    run(['git','-C',seed,'worktree','add','-b','test/files',files_lane])
    off['lanes'][-1]['path']=str(files_lane)
    config.write_text(json.dumps(off))
    off_rows=json.loads(cli('lane-status','--config',config,'--json'))
    check(off_rows[-1]['zx20'] is False and backing_device_matches(off_rows[-1]['backing_device'],files_lane),'bounded Files non-Unity reports actual device and ZX20 NO')
    check('ZX20: NO' in cli('lane-status','--config',config),'bounded Files human inventory clearly says ZX20 NO')
    check(off_rows[-1]['HEAD']==command(['git','-C',str(seed),'rev-parse','HEAD']) and off_rows[-1]['registered_worktree'] and off_rows[-1]['dirty'] is False,'bounded Files inventory checks registered clean repository identity')
    check('BLOCKED' in cli('lane-status','--config',config,'--preflight','--lane','off-storage','--branch','test/files',code=1),'bounded Files writable preflight remains blocked')
    # Default readers recognise the same bounded non-Unity registration.
    loaded,_=config_read(config)
    check(loaded['lanes'][-1]['path']==str(files_lane),'default loader admits registered bounded Files non-Unity')
    # Copy candidate tools byte-for-byte, then project untouched live config/board.
    # Default consumer CLI config arguments remain omitted; only lock state is isolated.
    consumer=SCRATCH/'default-consumers';shutil.copytree(OPS,consumer)
    shutil.copyfile(ROOT/'CCF-Ops/lanes.json',consumer/'lanes.json')
    shutil.copyfile(ROOT/'CCF-Ops/board/active.md',consumer/'board/active.md')
    consumer_before={str(p.relative_to(consumer)):p.read_bytes() for p in consumer.rglob('*') if p.is_file()}
    consumer_state=SCRATCH/'default-consumer-state'
    matrix=[]
    for args,label,code in [
        ([sys.executable,'-B',consumer/'scripts/unity-lock','status','--test-state-dir',consumer_state,'--json'],'default unity-lock status',0),
        ([sys.executable,'-B',consumer/'scripts/packet-lint',f/'valid.md'],'default packet-lint',0),
        (['bash','-c','source "$1"','ops652',consumer/'environment.sh'],'default environment',0),
        ([sys.executable,'-B',consumer/'scripts/unity-lock','acquire','--lane','NonUnity','--test-state-dir',consumer_state],'default NonUnity acquire rejects before guard',2)]:
        p=subprocess.run([str(v) for v in args],capture_output=True,text=True)
        matrix.append({'command':[str(v) for v in args],'exit':p.returncode,'stdout':p.stdout,'stderr':p.stderr})
        expected=('"status": "free"' in p.stdout if 'status' in label else 'not Unity-capable' in p.stderr if code==2 else p.stdout.startswith('PASS'))
        check(p.returncode==code and expected,label)
    check(not consumer_state.exists(),'default NonUnity/status consumers create no state or guard')
    check(consumer_before=={str(p.relative_to(consumer)):p.read_bytes() for p in consumer.rglob('*') if p.is_file()},'default consumers leave projected config/tool bytes unchanged')
    (SCRATCH/'default-caller-matrix.json').write_text(json.dumps(matrix,indent=2)+'\n')
    # Real live configuration is read only; no production lock command is run.
    actual,_=config_read(ROOT/'CCF-Ops/lanes.json')
    check(any(l['lane']=='NonUnity' and l['unity'] is False and l['path']=='/media/jer/Files/CCF/Operations/CCF-NonUnity' for l in actual['lanes']),'default loader reads actual untouched live NonUnity registration')
    # Invalid mixed config must fail in the real default consumers as well.
    consumer_config=consumer/'lanes.json';consumer_original=consumer_config.read_bytes()
    invalid_consumer=json.loads(consumer_original)
    next(l for l in invalid_consumer['lanes'] if l['lane']=='NonUnity')['path']='/'
    try:
        consumer_config.write_text(json.dumps(invalid_consumer))
        for args,expected_code in [
            ([sys.executable,'-B',consumer/'scripts/unity-lock','status','--test-state-dir',consumer_state],2),
            ([sys.executable,'-B',consumer/'scripts/packet-lint',f/'valid.md'],2),
            (['bash','-c','source "$1"','ops652',consumer/'environment.sh'],1)]:
            p=subprocess.run([str(v) for v in args],capture_output=True,text=True)
            check(p.returncode==expected_code and 'outside the Files CCF tree: /' in p.stderr,'default consumer rejects invalid mixed config with cause-specific error')
        check(not consumer_state.exists(),'invalid default consumers create no state or guard')
    finally:
        consumer_config.write_bytes(consumer_original)
    # The same Files path never becomes a Unity lane.
    off['lanes'][-1]['unity']=True
    config.write_text(json.dumps(off))
    p=subprocess.run([sys.executable,'-B',str(OPS/'scripts/lane-status'),'--config',str(config)],capture_output=True,text=True)
    check(p.returncode==2 and 'escapes ZX20' in p.stderr,'Files Unity lane rejected')
    for permit in [False,True]:
        try:config_read(config,permit_off_storage_lanes=permit)
        except ValueError as e:check('escapes ZX20' in str(e),'default/inventory Files Unity rejected')
        else:raise AssertionError('Files Unity admitted')
    unity_rejected_state=SCRATCH/'rejected-files-unity'
    p=subprocess.run([sys.executable,'-B',str(OPS/'scripts/unity-lock'),'acquire','--config',str(config),'--test-state-dir',str(unity_rejected_state),'--lane','off-storage'],capture_output=True,text=True)
    check(p.returncode==2 and 'escapes ZX20' in p.stderr and not unity_rejected_state.exists(),'Files Unity rejected before lock state/guard')
    config.write_text(json.dumps(cfg))
    # Real resolved path escapes; symlink fixture is on ext4.
    escape=files_fixture/'escape';escape.symlink_to('/')
    sibling_escape=files_fixture/'sibling-escape';sibling_escape.symlink_to(str(DURABLE_CCF)+'-sibling')
    for invalid in ['/',str(DURABLE_CCF)+'-sibling/lane',str(ROOT)+'-sibling/lane',str(DURABLE_CCF/'../outside-ccf'),str(escape),str(sibling_escape)]:
        bad=json.loads(json.dumps(cfg));bad['lanes'].append({'lane':'escape','path':invalid,'purpose':'negative boundary','unity':False});config.write_text(json.dumps(bad))
        for permit in [False,True]:
            try:config_read(config,permit_off_storage_lanes=permit)
            except ValueError as e:check('outside the Files CCF tree:' in str(e),'default/inventory resolved escape rejected: '+invalid)
            else:raise AssertionError('out-of-bounds configuration admitted')
        p=subprocess.run([sys.executable,'-B',str(OPS/'scripts/lane-status'),'--config',str(config)],capture_output=True,text=True)
        check(p.returncode==2 and 'outside the Files CCF tree:' in p.stderr,'resolved inventory path escape rejected: '+invalid)
    config.write_text(json.dumps(cfg))
    for key,value in [('uuid',''),('uuid','0000-0000'),('fstype','ext4'),('device','/dev/null'),('root',str(ROOT/'CCF-Temp'))]:
        bad=json.loads(json.dumps(cfg));bad['storage'][key]=value;config.write_text(json.dumps(bad))
        p=subprocess.run([sys.executable,'-B',str(OPS/'scripts/lane-status'),'--config',str(config)],capture_output=True,text=True)
        check(p.returncode==2 and 'mount identity mismatch' in p.stderr,'wrong storage '+key+' rejected: '+value)
    config.write_text(json.dumps(cfg))
    # Malformed configuration remains rejected with either caller mode.
    invalid_configs=[]
    for path in ['relative/lane','/']:
        bad=json.loads(json.dumps(cfg));bad['lanes'][0]['path']=path;invalid_configs.append((bad,'lane path must be absolute' if path!='/' else 'escapes ZX20'))
    for value in ['false',0,None]:
        bad=json.loads(json.dumps(cfg));bad['lanes'][0]['unity']=value;invalid_configs.append((bad,'unity: true/false'))
    bad=json.loads(json.dumps(cfg));bad['lanes'].append(dict(bad['lanes'][0]));invalid_configs.append((bad,'duplicate lane'))
    bad=json.loads(json.dumps(cfg));bad['lanes'].append(dict(bad['lanes'][0],lane='same-path'));invalid_configs.append((bad,'duplicate lane'))
    for repo in ['/',str(SCRATCH/'missing-repo')]:
        bad=json.loads(json.dumps(cfg));bad['repository']=repo;invalid_configs.append((bad,'repository must be' if repo=='/' else 'missing repository'))
    bad=json.loads(json.dumps(cfg));bad['git_stores'][0]['git_dir']='relative';invalid_configs.append((bad,'absolute git_dir'))
    bad=json.loads(json.dumps(cfg));bad['git_stores'].append(dict(bad['git_stores'][0]));invalid_configs.append((bad,'duplicate Git store'))
    for bad,message in invalid_configs:
        config.write_text(json.dumps(bad))
        for permit in [False,True]:
            try:config_read(config,permit_off_storage_lanes=permit)
            except ValueError as e:check(message in str(e),'default/inventory malformed config rejected: '+message)
            else:raise AssertionError('malformed configuration admitted')
    config.write_text(json.dumps(cfg))
    broken_board=SCRATCH/'broken-board.md'; broken_board.write_text('| ID | State |\n|---|---|\n')
    check(cli('packet-lint','--board',broken_board,code=1).startswith('FAIL'),'board missing required fields FAIL')

    # Storage admission and board checks.
    health=storage_health(cfg['storage'])
    check(health['ready'] and health['writable'] and health['free_bytes']>=15*1024**3,'actual ZX20 rw/free PASS')
    check(storage_health(cfg['storage'],free_bytes=0)['status']=='LOW SPACE','shared low-space admission BLOCK')
    ro={'source':DEVICE,'target':str(ROOT),'fstype':'exfat','uuid':'18D6-896E','options':'ro,nosuid'}
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
    # A nonempty path from another Git store cannot be acquired as our Unity lane.
    foreign=json.loads(json.dumps(cfg));foreign['lanes'][0]['path']=str(second);config.write_text(json.dumps(foreign))
    p=subprocess.run([sys.executable,'-B',str(OPS/'scripts/unity-lock'),*acquire,'--lane',str(second)],capture_output=True,text=True)
    check(p.returncode==2 and 'unexpected lane Git store' in p.stderr and not (state/'lock.json').exists(),'foreign Git store lane rejected without lease')
    config.write_text(json.dumps(cfg))
    held=json.loads(lock(*acquire)); check(held['status']=='held','lock acquire')
    check(json.loads(lock('status',*base,'--json'))['metadata']['owner']=='test-owner','lock status metadata')
    lock(*acquire,'--owner','second-owner',code=2); check(True,'lock second owner rejected')
    lock('release',*base,'--owner','wrong-owner','--task-id','TEST','--lock-id',held['metadata']['lock_id'],code=2); check((state/'lock.json').exists(),'lock wrong owner cannot release')
    for task,lock_id in [('WRONG-TASK',held['metadata']['lock_id']),('TEST','wrong-lock-id')]:
        p=subprocess.run([sys.executable,'-B',str(OPS/'scripts/unity-lock'),'release',*base,'--owner','test-owner','--task-id',task,'--lock-id',lock_id],capture_output=True,text=True)
        check(p.returncode==2 and (state/'lock.json').exists(),'mismatched task or lease identity cannot release')
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

    # D-091: an explicit mode, not an absent timestamp or distant estimate.
    indefinite=[v for v in acquire]
    eta_index=indefinite.index('--eta'); del indefinite[eta_index:eta_index+2]
    indefinite.append('--no-expiry')
    without_mode=indefinite[:-1]
    def rejected_acquire(args,cause,label):
        before={p.name:p.read_bytes() for p in state.iterdir() if p.is_file()}
        result=subprocess.run([sys.executable,'-B',str(OPS/'scripts/unity-lock'),*args],capture_output=True,text=True)
        after={p.name:p.read_bytes() for p in state.iterdir() if p.is_file()}
        check(result.returncode==2 and cause in result.stderr and before==after,label)
    rejected_acquire(without_mode,'requires --no-expiry or --eta','acquire cannot silently default to indefinite custody')
    rejected_acquire([*indefinite,'--eta',eta],'not allowed with argument','ETA and no-expiry are mutually exclusive')
    for invalid in ['null','',datetime.now(timezone.utc).isoformat(),'2020-01-01T00:00:00+00:00','2099-01-01T00:00:00']:
        rejected_acquire([*without_mode,'--eta',invalid],'isoformat' if invalid in ['null',''] else 'future ISO','invalid or elapsed legacy estimate rejected: '+repr(invalid))
    rejected_acquire([*indefinite,'--branch','wrong-branch'],'branch/HEAD mismatch','no-expiry preserves exact branch gate')
    rejected_acquire([*indefinite,'--head','0'*40],'branch/HEAD mismatch','no-expiry preserves exact HEAD gate')
    (clean/'fixture.txt').write_text('temporarily dirty for D-091 negative\n')
    try:
        rejected_acquire(indefinite,'lane is dirty','no-expiry preserves clean-worktree gate')
    finally:
        (clean/'fixture.txt').write_text('clean\n')
    run(['git','-C',second,'checkout','test/clean']); config.write_text(json.dumps(dual))
    try:
        rejected_acquire(indefinite,'another writable checkout','no-expiry preserves two-store branch conflict gate')
    finally:
        run(['git','-C',second,'checkout','test/seed']); config.write_text(json.dumps(cfg))
    low_cfg=json.loads(json.dumps(cfg));low_cfg['storage']['minimum_free_bytes']=os.statvfs(ROOT).f_bavail*os.statvfs(ROOT).f_frsize+1024**3
    config.write_text(json.dumps(low_cfg))
    try:
        rejected_acquire(indefinite,'LOW SPACE','no-expiry cannot bypass real capacity gate')
    finally:
        config.write_text(json.dumps(cfg))
    held=json.loads(lock(*indefinite)); valid=held['metadata']
    check(held['status']=='held' and valid['eta'] is None and valid['no_expiry'] is True and not held['eta_overdue'],'explicit no-expiry acquire persists marked null without overdue')
    check(json.loads(lock('status',*base,'--json'))['metadata']==valid,'no-expiry status reads exact custody after acquiring CLI exits')
    rejected_acquire(indefinite,'lock exists','second no-expiry acquisition cannot replace owner')
    rejected_acquire(acquire,'lock exists','legacy acquisition cannot replace no-expiry owner')
    release=['release',*base,'--owner','test-owner','--task-id','TEST','--lock-id',valid['lock_id']]
    for field,bad_value in [('--owner','wrong'),('--task-id','wrong'),('--lock-id','wrong')]:
        rejected_acquire([*release,field,bad_value],'matching --owner --task-id --lock-id','no-expiry release refuses wrong '+field)
    # No Unity process is launched: simulate detection inside the actual guarded
    # release function, retaining all real isolated config/record/history paths.
    operator=runpy.run_path(str(OPS/'scripts/unity-lock'))
    release_main=operator['main']; before=(state/'lock.json').read_bytes(); history_before=(state/'history.jsonl').read_bytes()
    with patch.dict(release_main.__globals__,editors=lambda:[12345]), patch.object(sys,'argv',['unity-lock',*release]):
        try:
            release_main()
            raise AssertionError('release accepted a simulated live Editor')
        except ValueError as error:
            check('Editor still running' in str(error) and (state/'lock.json').read_bytes()==before and (state/'history.jsonl').read_bytes()==history_before,'normal release refuses simulated live Editor without mutating custody/history')
    # Mutations here are deliberately corrupt *isolated fixture* records, not
    # migrations. Every malformed row must remain blocking and unmodified.
    corrupt=[]
    for field in valid:
        if field=='no_expiry': continue
        item=dict(valid);del item[field];corrupt.append(('missing '+field,item))
    for label,updates in [('unmarked null',{'no_expiry':False}),('invalid mode marker',{'no_expiry':'true'}),('null mode plus timestamp',{'eta':eta}),('invalid timestamp',{'eta':'garbage','no_expiry':False}),('naive timestamp',{'eta':'2026-01-01T00:00:00','no_expiry':False}),('wrong owner type',{'owner':42}),('wrong PID type',{'pid':True}),('wrong process identity',{'pid':1,'process_identity':{}})]:
        corrupt.append((label,dict(valid,**updates)))
    unmarked=dict(valid);del unmarked['no_expiry'];corrupt.append(('null without mode marker',unmarked))
    for label,item in corrupt:
        (state/'lock.json').write_text(json.dumps(item))
        check(json.loads(lock('status',*base,'--json'))['status']=='malformed','malformed no-expiry metadata: '+label)
        rejected_acquire(indefinite,'lock exists','malformed custody never reclaimed: '+label)
        rejected_acquire(release,'malformed record','malformed custody cannot normally release: '+label)
    (state/'lock.json').write_text(json.dumps(valid))
    lock(*release)
    event_row=json.loads((state/'history.jsonl').read_text().splitlines()[-1])
    check(not (state/'lock.json').exists() and event_row['event']=='release' and event_row['previous']==valid,'no-expiry matching release audits complete prior metadata')
    # Legacy on-disk records have no mode marker. An overdue ETA remains held.
    held=json.loads(lock(*acquire));legacy=held['metadata']
    check('no_expiry' not in legacy and legacy['eta']==eta,'legacy acquisition preserves timestamp record shape')
    legacy['eta']='2000-01-01T00:00:00+00:00';(state/'lock.json').write_text(json.dumps(legacy))
    overdue=json.loads(lock('status',*base,'--json'))
    check(overdue['status']=='held' and overdue['eta_overdue'] is True,'legacy overdue custody remains held and advisory')
    rejected_acquire(indefinite,'lock exists','no-expiry does not reclaim legacy overdue custody')
    lock('release',*base,'--owner','test-owner','--task-id','TEST','--lock-id',legacy['lock_id'])
    # The recorded PID may be a live operator: it cannot release until safe exit.
    proc=subprocess.Popen([sys.executable,'-B','-c','import sys; sys.stdin.read()'],stdin=subprocess.PIPE)
    try:
        held=json.loads(lock(*indefinite,'--pid',str(proc.pid)));identity=held['metadata']
        release=['release',*base,'--owner','test-owner','--task-id','TEST','--lock-id',identity['lock_id']]
        rejected_acquire(release,'recorded PID still exists','normal release refuses actual live recorded process')
        identity['process_identity']['start_ticks']='0';(state/'lock.json').write_text(json.dumps(identity))
        reused=json.loads(lock('status',*base,'--json'))
        check(reused['status']=='stale' and 'reused' in reused['inspection'],'PID reuse/start-ticks mismatch reported as stale')
        rejected_acquire(indefinite,'lock exists','PID reuse cannot silently reclaim no-expiry custody')
        rejected_acquire(release,'recorded PID still exists','PID reuse cannot bypass normal release safety')
        identity['process_identity']['boot_id']='00000000-0000-0000-0000-000000000000';(state/'lock.json').write_text(json.dumps(identity))
        check(json.loads(lock('status',*base,'--json'))['status']=='stale','boot identity mismatch remains stale evidence')
    finally:
        proc.stdin.close();proc.wait(timeout=5)
    check(json.loads(lock('status',*base,'--json'))['status']=='stale','exited no-expiry recorded PID remains stale until explicit release')
    rejected_acquire(indefinite,'lock exists','exited PID cannot silently reclaim no-expiry custody')
    lock(*release)
    with ThreadPoolExecutor(max_workers=6) as pool:
        attempts=list(pool.map(lambda _:subprocess.run([sys.executable,'-B',str(OPS/'scripts/unity-lock'),*indefinite],capture_output=True,text=True),range(6)))
    check(sum(p.returncode==0 for p in attempts)==1 and sum(p.returncode==2 for p in attempts)==5,'no-expiry concurrent processes: exactly one guarded winner')
    held=json.loads(lock('status',*base,'--json'))
    lock('release',*base,'--owner','test-owner','--task-id','TEST','--lock-id',held['metadata']['lock_id'])

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
finally:
    # Only this mkdtemp-owned directory is removed, even after a failed assertion.
    if (files_lane/'.git').is_file():
        run(['git','-C',seed,'worktree','remove',files_lane])
    shutil.rmtree(files_fixture)
    check(not files_fixture.exists(),'owned Files lane and escape symlinks cleaned in finally')
result={'result':'PASS','checks':count,'scratch':str(SCRATCH),'pilot_evidence':str(pilot),'source_head':source_head,'unity_launched':False,'production_lock_acquired':False}
(SCRATCH/'result.json').write_text(json.dumps(result,indent=2)+'\n')
print(json.dumps(result,indent=2),flush=True)
