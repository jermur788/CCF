#!/usr/bin/env python3
"""Sequential baseline verification on unchanged production. Exact source/mode recorded."""
import fcntl,hashlib,json,os,shutil,subprocess,time
from pathlib import Path
ROOT=Path(__file__).resolve().parents[3];out=ROOT/'Build/UnderstoreyRecruitment/regression';out.mkdir(parents=True,exist_ok=True)
config=ROOT/'Build/UnderstoreyRecruitment/config';lock=(ROOT/'Build/UnderstoreyRecruitment/launch.lock').open('w');fcntl.flock(lock,fcntl.LOCK_EX|fcntl.LOCK_NB)
# P0 mode corrections are used as disposable sources, not ported into owned UI files.
P0='1a36ea97c724b3a026e76a0b5432349b46e6a298'
GATES=[
 ('SitkaGrowthModelVerification','Tools/Verification/SitkaGrowthMortality/SitkaGrowthModelVerification.cs',False,None),
 ('RegenerationModelVerification','Tools/Verification/RegenerationBudget/RegenerationModelVerification.cs',False,None),
 ('ScenarioReferenceVerification','Tools/Verification/ScenarioReferenceVerification.cs',False,None),
 ('ScenarioOneCompletionVerification','Tools/Verification/ScenarioOneCompletionVerification.cs',False,None),
 ('ScenarioOneEconomyIntegrationVerification','Tools/Verification/ScenarioOneEconomyIntegrationVerification.cs',False,None),
 ('BrowsingProtectionVerification','Tools/Verification/BrowsingProtectionVerification.cs',False,None),
 ('ScenarioOnePlantingVerification','Tools/Verification/ScenarioOnePlantingVerification.cs',False,None),
 ('ScenarioOnePruningVerification','Tools/Verification/ScenarioOnePruningVerification.cs',False,None),
 ('ScenarioOneDeadwoodVerification','Tools/Verification/ScenarioOneDeadwoodVerification.cs',False,None),
 ('ScenarioOneProgressVerification','Tools/Verification/ScenarioOneProgressVerification.cs',False,None),
 ('SaveHardeningVerification','Tools/Verification/SaveHardeningVerification.cs',False,None),
 ('RngModelVerification','Tools/Verification/RngModelVerification.cs',False,None),
 ('RngModelPolicyVerification','Tools/Verification/RngModelPolicyVerification.cs',False,None),
 ('ScenarioOneInteractionVerification','Assets/ForestPrototype/ScenarioOneInteractionVerification.cs',False,None),
 ('EcologyCalibrationAdoptionVerification','Tools/Verification/EcologyCalibrationAdoptionVerification.cs',False,None),
 ('CCFIntegrationVerificationTemp','Tools/Verification/CCFIntegrationVerificationTemp.cs',False,P0),
 ('ScenarioOneRemovalVerification','Tools/Verification/ScenarioOneRemovalVerification.cs',False,P0),
 ('ClearanceVerification','Tools/Verification/ClearanceVerification.cs',True,P0),
 ('MenuTutorialVerification','Tools/Verification/MenuTutorialVerification.cs',True,P0)]
FINAL_MARKERS={'SitkaGrowthModelVerification': 'SITKA_GROWTH_MODEL_VERIFY_PASS', 'RegenerationModelVerification': 'REGENERATION_MODEL_VERIFY_PASS', 'ScenarioReferenceVerification': 'REFERENCE_FUTURE_V1_PASS', 'ScenarioOneCompletionVerification': 'SCENARIO_ONE_COMPLETION_VERIFY_PASS', 'ScenarioOneEconomyIntegrationVerification': 'SCENARIO_ONE_ECONOMY_INTEGRATION_VERIFY_PASS', 'BrowsingProtectionVerification': 'BROWSING_PROTECTION_VERIFY_PASS', 'ScenarioOnePlantingVerification': 'SCENARIO_ONE_PLANTING_VERIFY_PASS', 'ScenarioOnePruningVerification': 'SCENARIO_ONE_PRUNING_VERIFY_PASS', 'ScenarioOneDeadwoodVerification': 'SCENARIO_ONE_DEADWOOD_VERIFY_PASS', 'ScenarioOneProgressVerification': 'SCENARIO_ONE_PROGRESS_VERIFY_PASS', 'SaveHardeningVerification': 'SAVE_HARDENING_VERIFY_PASS', 'RngModelVerification': 'RNG_MODEL_VERIFY_PASS', 'RngModelPolicyVerification': 'RNG_MODEL_POLICY_VERIFY_PASS', 'ScenarioOneInteractionVerification': 'SCENARIO_ONE_INTERACTION_VERIFY_PASS', 'EcologyCalibrationAdoptionVerification': 'ECOLOGY_CALIBRATION_ADOPTION_VERIFY_PASS', 'CCFIntegrationVerificationTemp': 'INTEGRATION_VERIFY_PASS', 'ScenarioOneRemovalVerification': 'SCENARIO_ONE_REMOVAL_VERIFY_PASS', 'ClearanceVerification': 'CLEARANCE_ACCEPTANCE_PASS', 'MenuTutorialVerification': 'MENU_TUTORIAL_PLAYTHROUGH_PASS'}
results=[]
for gate,path,interactive,ref in GATES:
 target=ROOT/'Assets/ForestPrototype'/(gate+'.cs');staged=not path.startswith('Assets/');log=out/(gate+'.log')
 if staged and target.exists():raise RuntimeError('Existing staged source '+str(target))
 data=subprocess.check_output(['git','show',ref+':'+path],cwd=ROOT) if ref else (ROOT/path).read_bytes()
 if staged:target.write_bytes(data)
 unit='ccf-understorey-regression-'+str(time.time_ns());start=time.monotonic()
 try:
  cmd=['systemd-run','--user','--scope','--quiet','--unit='+unit,'-p','MemoryMax=8G','-p','MemorySwapMax=256M','/media/jer/ZX20/Unity/6000.6.0f1/Editor/Unity','-projectPath',str(ROOT),'-job-worker-count','2','-executeMethod',gate+'.Begin','-logFile',str(log)]
  if not interactive:cmd+=['-batchmode','-nographics']
  env=dict(os.environ,XDG_CONFIG_HOME=str(config),CCF_DIAG_DIR=str(out/'evidence'),CCF_ACCEPTANCE_OUTPUT=str(out/'evidence'),DISPLAY=os.environ.get('DISPLAY',':0'))
  (out/'evidence').mkdir(exist_ok=True)
  try:code=subprocess.run(cmd,cwd=ROOT,env=env,timeout=1200).returncode
  except subprocess.TimeoutExpired:subprocess.run(['systemctl','--user','stop',unit+'.scope'],check=False);code=124
  content=log.read_text(errors='replace') if log.exists() else ''
  markers=[s for s in content.splitlines() if any(x in s for x in ['_PASS','_FAIL','_HASH','error CS'])]
  passed=code==0 and FINAL_MARKERS[gate] in content and not any('_FAIL' in s or 'error CS' in s for s in markers)
  # Explicit immutable Reference anchors; stop immediately if the archive/replay differs.
  if gate=='ScenarioReferenceVerification':passed=passed and '7AD177B3CC2F73C7' in content and '9CDF21A541C5968D' in content
  result=dict(gate=gate,status='PASS' if passed else 'FAIL',exit_code=code,seconds=round(time.monotonic()-start,2),mode='interactive' if interactive else 'batch',source_ref=ref or 'a8596df',source_sha256=hashlib.sha256(data).hexdigest(),markers=markers)
  results.append(result);(out/'results.json').write_text(json.dumps(results,indent=2)+'\n');print(json.dumps({k:result[k] for k in ['gate','status','seconds','exit_code']}),flush=True)
  if gate=='ScenarioReferenceVerification' and not passed:raise RuntimeError('STOP: unexpected Reference failure. See '+str(log))
 finally:
  if staged:target.unlink(missing_ok=True);Path(str(target)+'.meta').unlink(missing_ok=True)
