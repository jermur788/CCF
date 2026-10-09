#!/usr/bin/env python3
"""Build one clean committed Windows Mono candidate; no packages/settings upgrades."""
import argparse, datetime, hashlib, json, os, shutil, subprocess, time, zipfile
from pathlib import Path
from zoneinfo import ZoneInfo
ROOT=Path(__file__).resolve().parents[3]
def git(*args): return subprocess.check_output(['git',*args],cwd=ROOT,text=True).strip()
def main():
 p=argparse.ArgumentParser();p.add_argument('--smoke',action='store_true',help='Separate diagnostic Player, never tester package');p.add_argument('--unity',default='/media/jer/ZX20/Unity/6000.6.0f1/Editor/Unity');p.add_argument('--date',default=datetime.datetime.now(ZoneInfo('Europe/Dublin')).strftime('%Y%m%d'));p.add_argument('--sequence',default='B01');args=p.parse_args()
 if git('status','--porcelain'):raise SystemExit('STOP: candidate source must be committed and clean')
 if not args.date.isdigit() or len(args.date)!=8 or not (args.sequence.startswith('B') and args.sequence[1:].isdigit()):raise SystemExit('Invalid date/sequence')
 sha=git('rev-parse','HEAD');build_id=f'CCF-S1-{args.date}-{args.sequence}-{sha[:7]}'
 if args.smoke: build_id+='-SMOKE'
 out=ROOT/'Build/S1B'/f'{build_id}-Windows-x64';out.mkdir(parents=True,exist_ok=False)
 identity=dict(buildId=build_id,gitSha=sha,unity='6000.6.0f1',platform='Windows-x64',saveContract='same-build only',nativeWindowsVerified=False)
 target=ROOT/'Assets/ForestPrototype/Editor/WindowsCandidateBuilder.cs';stamp=ROOT/'Assets/ForestPrototype/UI/Resources/CCFBuildIdentity.json'
 if target.exists() or stamp.exists():raise SystemExit('STOP: pre-existing disposable build source/identity')
 for source in (ROOT/'Assets/ForestPrototype').glob('*Verification*.cs'):
  if not subprocess.run(['git','cat-file','-e',sha+':'+str(source.relative_to(ROOT))],cwd=ROOT,capture_output=True).returncode==0:raise SystemExit('STOP: disposable verification source: '+str(source))
 config=ROOT/'Build/S1B/config';shutil.copytree(Path.home()/'.config/unity3d/Unity/licenses',config/'unity3d/Unity/licenses',dirs_exist_ok=True)
 env=dict(os.environ,XDG_CONFIG_HOME=str(config),CCF_BUILD_OUTPUT=str(out),CCF_BUILD_IDENTITY=json.dumps(identity))
 smoke=ROOT/'Assets/ForestPrototype/S1BStandalonePlayerSmoke.cs'
 if smoke.exists():raise SystemExit('STOP: pre-existing smoke source')
 if args.smoke:smoke.write_bytes((ROOT/'Tools/Verification/S1B/StandalonePlayerSmoke.cs').read_bytes())
 target.write_bytes((ROOT/'Tools/Build/S1B/WindowsCandidateBuilder.cs').read_bytes());log=out.parent/(build_id+'-build.log')
 # These files are rewritten by Unity/URP build callbacks, not S1-B source edits.
 preserved=['.vscode/settings.json','Assets/Settings/DefaultVolumeProfile.asset','Assets/Settings/PC_RPAsset.asset','Assets/Settings/UniversalRenderPipelineGlobalSettings.asset','ProjectSettings/GraphicsSettings.asset','ProjectSettings/ProjectSettings.asset','ProjectSettings/UnityConnectSettings.asset']
 preserved += [str(f.relative_to(ROOT)) for f in (ROOT/'Assets/ForestPrototype/Art/SectionFive').rglob('*.mat')]
 original={rel:(ROOT/rel).read_bytes() for rel in preserved}

 try:
  code=subprocess.run([args.unity,'-batchmode','-nographics','-projectPath',str(ROOT),'-job-worker-count','2','-executeMethod','WindowsCandidateBuilder.Build','-logFile',str(log)],cwd=ROOT,env=env,timeout=5400).returncode
 finally:
  for f in [target,Path(str(target)+'.meta'),stamp,Path(str(stamp)+'.meta'),smoke,Path(str(smoke)+'.meta')]:f.unlink(missing_ok=True)
  changed=[rel for rel,data in original.items() if (ROOT/rel).read_bytes()!=data]
  if changed:
   (out.parent/(build_id+'-source-normalisation.diff')).write_bytes(subprocess.check_output(['git','diff','--',*changed],cwd=ROOT))
   for rel in changed:(ROOT/rel).write_bytes(original[rel])
  if git('status','--porcelain'):raise SystemExit('STOP: unexpected source drift after build; inspect Git diff')

 if code:raise SystemExit('Build failed; inspect '+str(log))
 if 'S1B_WINDOWS_BUILD_PASS' not in log.read_text(errors='replace'):raise SystemExit('Missing build success marker')
 # Retain Unity's diagnostic backups beside the candidate, never inside it.
 debug=out.parent/(build_id+'-developer-backups')
 for directory in list(out.iterdir()):
  if directory.is_dir() and any(word in directory.name.lower() for word in ['donotship','dontship','backupthisfolder']):
   debug.mkdir(exist_ok=True);shutil.move(str(directory),str(debug/directory.name))
 (out/'build-identity.json').write_text(json.dumps(identity,indent=2)+'\n')
 (out/'README.txt').write_text((ROOT/'Docs/Verification/S1B/TesterREADME.md').read_text().replace('@BUILD_ID@',build_id).replace('@GIT_SHA@',sha))
 if args.smoke:
  print(json.dumps(dict(status='PASS',diagnosticOnly=True,buildId=build_id,gitSha=sha,path=str(out))))
  return
 notices=out/'Notices';notices.mkdir()
 shutil.copy2(ROOT/'Tools/Build/S1B/Notices/UnityPlayer-Windows-Mono-6000.6.0f1.pdf',notices/'UnityPlayer-Windows-Mono-6000.6.0f1.pdf')
 # Preserve notices supplied by the installed packages. Editor-only packages are
 # included conservatively, without claiming their code ships in the Player.
 for package in sorted((ROOT/'Library/PackageCache').iterdir()):
  if not package.is_dir():continue
  for name in ['LICENSE.md','LICENSE','Third Party Notices.md','Third Party Notices.txt']:
   source=package/name
   if source.is_file():dest=notices/package.name;dest.mkdir(exist_ok=True);shutil.copy2(source,dest/name)
 shutil.copy2(ROOT/'Docs/Verification/S1B/RightsAudit.md',notices/'RightsAudit.md')
 (notices/'AssetCredits.txt').write_text('Ultimate Nature – Starter by Innerverse Interactive: locally licensed Unity Asset Store pack, embedded only. Project procedural/generated artwork: owner-confirmed private compiled-build rights. See RightsAudit.md and supplied notices.\n')
 files={str(f.relative_to(out)):dict(bytes=f.stat().st_size,sha256=hashlib.sha256(f.read_bytes()).hexdigest()) for f in sorted(out.rglob('*')) if f.is_file()}
 forbidden=[p for p in files if p.endswith(('.cs','.unitypackage','.ulf','.lic','.save')) or 'forest-save' in p.lower() or 'verification' in p.lower() or any(word in p.lower() for word in ['donotship','dontship','backupthisfolder','performancetestrun']) or p.endswith('.pdb')]
 if forbidden:raise SystemExit('STOP: forbidden tester payload: '+str(forbidden))
 assert (out/'CCF.exe').is_file() and (out/'UnityPlayer.dll').is_file() and (out/'CCF_Data').is_dir()
 (out/'package-manifest.json').write_text(json.dumps(dict(identity=identity,files=files,looseSourceOrSecretsOrSavesFound=False),indent=2)+'\n')
 archive=out.with_suffix('.zip')
 with zipfile.ZipFile(archive,'x',compression=zipfile.ZIP_DEFLATED,compresslevel=6) as z:
  for f in sorted(out.rglob('*')):
   if f.is_file():z.write(f,str(Path(out.name)/f.relative_to(out)))
 print(json.dumps(dict(status='PASS',buildId=build_id,gitSha=sha,path=str(out),archive=str(archive),archive_sha256=hashlib.sha256(archive.read_bytes()).hexdigest()),indent=2))
if __name__=='__main__':main()
