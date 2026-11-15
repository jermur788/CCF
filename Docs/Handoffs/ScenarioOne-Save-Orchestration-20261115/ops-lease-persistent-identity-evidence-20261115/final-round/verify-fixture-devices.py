#!/usr/bin/env python3
"""Inspect the exact test helper; prove identity negatives without lock writes."""
import argparse
import ast
from copy import deepcopy
import json
import os
from pathlib import Path
import shutil
import stat
import subprocess
import sys
import tempfile
import uuid

parser=argparse.ArgumentParser(description=__doc__)
parser.add_argument('--ops',type=Path,required=True)
parser.add_argument('--output',type=Path,required=True)
args=parser.parse_args()
ops=args.ops.resolve()
sys.path.insert(0,str(ops/'scripts'))
import _ccf
_ccf.storage_check()
results=[]
def check(condition,label,**details):
    record=dict(label=label,passed=bool(condition),**details)
    results.append(record)
    print(('PASS: ' if condition else 'FAIL: ')+label,flush=True)
    assert condition,record
try:
    tree=ast.parse((ops/'tests/self-test.py').read_text())
    helper=next(n for n in tree.body if isinstance(n,ast.FunctionDef) and n.name=='backing_device_matches')
    namespace=dict(os=os,stat=stat)
    exec(compile(ast.Module(body=[helper],type_ignores=[]),str(ops/'tests/self-test.py'),'exec'),namespace)
    matches=namespace['backing_device_matches']
    device=_ccf.command(['findmnt','-n','-o','SOURCE','-T',str(_ccf.ROOT)])
    other=_ccf.command(['findmnt','-n','-o','SOURCE','-T','/'])
    check(matches(device,_ccf.ROOT),'test helper accepts guarded-root backing device',device=device)
    check(not matches(other,_ccf.ROOT),'test helper rejects a DIFFERENT existing block device',device=other)
    check(matches(other,'/'),'test helper independently identifies root backing device',device=other)
    check(not matches('/dev/null',_ccf.ROOT),'test helper rejects character device')
    check(not matches(ops/'tests/self-test.py',_ccf.ROOT),'test helper rejects regular file')
    check(not matches('/dev/disk/by-id/CCF-NONEXISTENT-548',_ccf.ROOT),'test helper rejects nonexistent device')
    cfg=json.loads((ops/'lanes.json').read_text())
    for label,key,value in [('different block device','device',other),('wrong UUID','uuid','DEAD-BEEF'),('wrong fstype','fstype','ext4'),('nonexistent device','device','/dev/disk/by-id/CCF-NONEXISTENT-548')]:
        storage=deepcopy(cfg['storage']);storage[key]=value
        health=_ccf.storage_health(storage)
        check(health['status']=='WRONG DEVICE' and not health['ready'],'storage_health rejects '+label,health=health)
    with tempfile.TemporaryDirectory(prefix='Ops548FinalIdentity-',dir='/media/jer/Files/CCF/Operations') as temp:
        scratch=Path(temp)
        for label,source in [('candidate',ops),('live-copy',Path('/media/jer/ZX20/CCF-Ops'))]:
            target=scratch/label
            (target/'scripts').mkdir(parents=True)
            for name in ['_ccf.py','unity-lock']:
                shutil.copyfile(source/'scripts'/name,target/'scripts'/name)
            value=json.loads((source/'lanes.json').read_text())
            value['storage']['device']=other
            (target/'lanes.json').write_text(json.dumps(value))
            state=_ccf.ROOT/'CCF-Temp'/('ops548-final-never-created-'+uuid.uuid4().hex)
            for operation in ['status','acquire','release']:
                command=[sys.executable,'-B',str(target/'scripts/unity-lock'),operation,'--test-state-dir',str(state),'--json']
                p=subprocess.run(command,capture_output=True,text=True)
                check(p.returncode==2 and 'ZX20 mount identity mismatch' in p.stderr,label+': different physical device rejects '+operation,command=command,exit_code=p.returncode,stdout=p.stdout,stderr=p.stderr)
                check(not state.exists(),label+': rejected '+operation+' creates no state')
    print('FIXTURE_DEVICE_VERIFICATION_PASS '+str(len(results)),flush=True)
finally:
    args.output.write_text(json.dumps(results,indent=2)+'\n')
