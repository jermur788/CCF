import sys,json,ast
from pathlib import Path
from unittest.mock import patch
sys.path.insert(0,sys.argv[1]);import _ccf
cfg=json.loads(Path(sys.argv[2]).read_text());s=cfg['storage'];n=0
def check(value,label):
 global n
 assert value,label
 n+=1;print('PASS:',label)
info=json.loads(_ccf.command(['findmnt','-J','-T',s['root'],'-o','SOURCE,TARGET,FSTYPE,UUID,OPTIONS']))['filesystems'][0]
check(_ccf.same_device(info['source'],s['device']),'enumerated source and persistent pin share block device number')
check(not _ccf.same_device('/dev/null','/dev/null'),'character devices rejected')
check(not _ccf.same_device(sys.argv[2],sys.argv[2]),'regular files rejected even with matching rdev')
check(not _ccf.same_device(info['source'],'/dev/disk/by-id/CCF-NONEXISTENT-548'),'absent pin rejected')
check(_ccf.visible_uuid_devices(s['uuid'])==[info['source']],'exactly one physically visible UUID')
check(str(_ccf.storage_check())==s['root'],'default storage_check uses persistent pin')
check(_ccf.storage_health(s)['status']=='PASS','physical storage health PASS')
check(_ccf.storage_health(s,free_bytes=0)['status']=='LOW SPACE','low capacity remains blocked')
ro=dict(info,options='ro,nosuid')
check(_ccf.storage_health(s,mount_info=ro)['status']=='READ ONLY','read-only remains blocked')
check(_ccf.storage_health(s,mount_info=dict(info,target='/tmp'))['status']=='WRONG DEVICE','mount target remains bound')
for devices in [[],[info['source'],'/dev/CCF-DUPLICATE-FIXTURE']]:
 with patch.object(_ccf,'visible_uuid_devices',return_value=devices):
  try:_ccf.storage_check()
  except ValueError as e:check('exactly one device' in str(e),'zero/duplicate UUID storage_check rejected: '+repr(devices))
  else:raise AssertionError('UUID ambiguity accepted')
  check(_ccf.storage_health(s)['status']=='WRONG DEVICE','zero/duplicate UUID health blocked')
for name in ['_ccf.py','lane-status','unity-lock','packet-lint']:
 ast.parse((Path(sys.argv[1])/name).read_text());check(True,'Python parse: '+name)
print('IDENTITY_VERIFICATION_PASS',n)
