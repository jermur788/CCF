from sessions import *
import sys
name=sys.argv[1]
assert json.loads(cmd(['python3','-B',lock,'status','--json']))['status']=='free'
assert not cmd(['git','status','--porcelain','-uall'],cwd=lane).strip();assert cmd(['git','rev-parse','HEAD'],cwd=lane).strip()==head
audit();print(cmd(['findmnt','-T',str(lane),'-o','SOURCE,UUID,FSTYPE,AVAIL']),flush=True)
assert __import__('shutil').disk_usage(lane).free>=15*1024**3
lease=cmd(['python3','-B',lock,'acquire','--owner','Sol381','--task-id','CCF-381-UI-QUALIFICATION','--lane',str(lane),'--branch','qualify/scenario-one-tagged-text-379-20261111','--head',head,'--eta',(datetime.datetime.now(datetime.timezone.utc)+datetime.timedelta(minutes=25)).isoformat(),'--notes','381 '+name+' isolated accepted harness','--json']);(r/(name+'-lease.json')).write_text(lease)
base=['python3','-B',str(r/'run_gate.py'),'--ext-files',str(r/'PathGuard381.cs'),'--out',str(r/name),'--display',':89','--timeout','900']
if name=='reference': args=['--name','Reference','--method','ScenarioReferenceVerification.Begin','--files','ScenarioReferenceVerification.cs','ScenarioOneRepeatedInterventions/TeachingCapGateSetup.cs','--env','CCF_STAND_GEOMETRY=0','CCF_T03_LEGACY=1','--pass-token','REFERENCE_FUTURE_V1_PASS','--fail-token','REFERENCE_FUTURE_V1_FAIL']
elif name=='history': args=['--name','History','--method','ScenarioHistoryVerification.Begin','--files','ThreeSpecies/ScenarioHistoryVerification.cs','ScenarioOneRepeatedInterventions/TeachingCapGateSetup.cs','--env','CCF_STAND_GEOMETRY=0','CCF_T03_LEGACY=1','--pass-token','SCENARIO_HISTORY_VERIFY_PASS','--fail-token','SCENARIO_HISTORY_VERIFY_FAIL']
elif name=='density': args=['--name','Density','--method','MixedDensityVerification.Begin','--files','ThreeSpecies/MixedDensityVerification.cs','--env','CCF_STAND_GEOMETRY=0','--pass-token','MIXED_DENSITY_VERIFY_PASS','--fail-token','MIXED_DENSITY_VERIFY_FAIL']
elif name.startswith('residual'): args=['--name','Residual','--method','Ccf381.Begin','--ext-files',str(r/'Ccf381.cs'),str(r/'ResidualProgress381.cs'),str(r/'PathGuard381.cs'),'--pass-token','C381_SESSION_ENDED','--fail-token','C381_FAIL']
else: raise Exception(name)
code=s.call(base+args,cwd=lane,env=dict(os.environ,CCF_GATE_LANE=str(lane)));finish(name);print('phase',name,'exit',code,flush=True)
