#!/usr/bin/env python3
"""Build one clean committed Windows Mono candidate; no packages/settings upgrades.

Guards shared with build_linux.py, whose first real run needed all three:
  --license-dir      the CURRENTLY VALID Unity licence folder. The default ~/.config/unity3d/Unity/licenses can be stale and
                     Unity then exits 198 "No valid Unity Editor license found".
  TMPDIR socket      Unity's licensing client creates a Unix socket under $TMPDIR; on exFAT (the ZX20 drive) it cannot and the
                     Editor hangs with an unrelated-looking error, so the build stops early with the real reason.
  art-pack preflight the locally acquired "Ultimate Nature - Starter" pack is verified against art-pack-manifest.json before
                     anything is staged (see art_pack.py). Only the required files are installed, so the pack's demo HDR is
                     never imported and with_demo_hdr_quarantined.py is not needed.
--project / --output-root exist so the control flow can be tested against a mock project (test_build_windows.py); the defaults
are this repository and its ignored Build/S1B."""
import argparse, datetime, hashlib, json, os, shutil, socket, subprocess, sys, tempfile, time, zipfile
from pathlib import Path
from zoneinfo import ZoneInfo
sys.path.insert(0,str(Path(__file__).resolve().parent))
import art_pack  # sibling module; explicit local art-pack dependency check
ROOT=Path(__file__).resolve().parents[3]
def git(*args,cwd=None): return subprocess.check_output(['git',*args],cwd=cwd or ROOT,text=True).strip()
def require_socket_capable_tmpdir():
 # Same guard as build_linux.require_socket_capable_tmpdir (kept separate: the two builders do not import each other).
 path=os.path.join(tempfile.gettempdir(),'.ccf-sock-%d'%os.getpid())
 if len(path.encode())>=100:return # cannot probe within the AF_UNIX path limit
 probe=socket.socket(socket.AF_UNIX,socket.SOCK_STREAM)
 try:probe.bind(path)
 except OSError as error:raise SystemExit("STOP: TMPDIR %s cannot create Unix sockets (%s); Unity's licensing client would hang. Set TMPDIR to a socket-capable folder such as /run/user/<uid>/ccf-build."%(tempfile.gettempdir(),error))
 finally:
  probe.close()
  try:os.unlink(path)
  except OSError:pass
def main():
 p=argparse.ArgumentParser(description=__doc__,formatter_class=argparse.RawDescriptionHelpFormatter);p.add_argument('--smoke',action='store_true',help='Separate diagnostic Player, never tester package');p.add_argument('--unity',default='/media/jer/ZX20/Unity/6000.6.0f1/Editor/Unity');p.add_argument('--date',default=datetime.datetime.now(ZoneInfo('Europe/Dublin')).strftime('%Y%m%d'));p.add_argument('--sequence',default='B01')
 p.add_argument('--project',default=str(ROOT),help='project root (default: this repo)');p.add_argument('--output-root',default=None,help='build root (default: <project>/Build/S1B, ignored by Git)')
 p.add_argument('--license-dir',default=None,help='folder holding the CURRENTLY VALID Unity licence (Hub-managed). Default: ~/.config/unity3d/Unity/licenses, which can be stale; Unity then stops with "No valid Unity Editor license found"')
 p.add_argument('--art-pack',default=os.environ.get('CCF_ART_PACK_DIR'),help='folder of the locally acquired Ultimate Nature - Starter pack (InnerverseInteractive, or its parent) (default: $CCF_ART_PACK_DIR; else the pack must already be installed in the project)')
 p.add_argument('--no-art-pack-check',action='store_true',help='TEST ONLY (mock projects): skip the art-pack check; recorded as unverified in the build identity')
 args=p.parse_args()
 root=Path(args.project).resolve();output_root=Path(args.output_root).resolve() if args.output_root else root/'Build/S1B'
 if not (root/'Assets/ForestPrototype').is_dir():raise SystemExit('STOP: '+str(root/'Assets/ForestPrototype')+' missing; run in a full project worktree')
 if git('status','--porcelain',cwd=root):raise SystemExit('STOP: candidate source must be committed and clean')
 if not args.date.isdigit() or len(args.date)!=8 or not (args.sequence.startswith('B') and args.sequence[1:].isdigit()):raise SystemExit('Invalid date/sequence')
 require_socket_capable_tmpdir()
 licences=Path(args.license_dir) if args.license_dir else Path.home()/'.config/unity3d/Unity/licenses'
 if args.license_dir and not licences.is_dir():raise SystemExit('STOP: --license-dir folder not found: '+str(licences))
 sha=git('rev-parse','HEAD',cwd=root);build_id=f'CCF-S1-{args.date}-{args.sequence}-{sha[:7]}'
 if args.smoke: build_id+='-SMOKE'
 out=output_root/f'{build_id}-Windows-x64'
 identity=dict(buildId=build_id,gitSha=sha,unity='6000.6.0f1',platform='Windows-x64',saveContract='same-build only',nativeWindowsVerified=False)
 if args.no_art_pack_check:
  print('WARNING: --no-art-pack-check: the art pack is NOT verified (test use only)',file=sys.stderr);identity['artPack']=dict(verified=False,reason='--no-art-pack-check')
 else:
  try:identity['artPack']=art_pack.prepare(root,args.art_pack,root/'Tools/Build/S1B/art-pack-manifest.json')
  except art_pack.ArtPackError as error:raise SystemExit('STOP: '+str(error))
  full_content=identity['artPack'].get('fullContent')
  if isinstance(full_content,dict) and full_content.get('matchesManifest') is False:print("WARNING: the pack's whole-content checksum differs from the manifest; the required files do match",file=sys.stderr)
 target=root/'Assets/ForestPrototype/Editor/WindowsCandidateBuilder.cs';stamp=root/'Assets/ForestPrototype/UI/Resources/CCFBuildIdentity.json'
 if target.exists() or stamp.exists():raise SystemExit('STOP: pre-existing disposable build source/identity')
 for source in (root/'Assets/ForestPrototype').glob('*Verification*.cs'):
  if not subprocess.run(['git','cat-file','-e',sha+':'+str(source.relative_to(root))],cwd=root,capture_output=True).returncode==0:raise SystemExit('STOP: disposable verification source: '+str(source))
 smoke=root/'Assets/ForestPrototype/S1BStandalonePlayerSmoke.cs'
 if smoke.exists():raise SystemExit('STOP: pre-existing smoke source')
 if out.exists():raise SystemExit('STOP: output already exists; refusing to overwrite: '+str(out))
 out.mkdir(parents=True)
 config=output_root/'config'
 if licences.is_dir():shutil.copytree(licences,config/'unity3d/Unity/licenses',dirs_exist_ok=True)
 env=dict(os.environ,XDG_CONFIG_HOME=str(config),CCF_BUILD_OUTPUT=str(out),CCF_BUILD_IDENTITY=json.dumps(identity))
 if args.smoke:smoke.write_bytes((root/'Tools/Verification/S1B/StandalonePlayerSmoke.cs').read_bytes())
 target.write_bytes((root/'Tools/Build/S1B/WindowsCandidateBuilder.cs').read_bytes());log=out.parent/(build_id+'-build.log')
 # These files are rewritten by Unity/URP build callbacks, not S1-B source edits.
 preserved=['.vscode/settings.json','Assets/Settings/DefaultVolumeProfile.asset','Assets/Settings/PC_RPAsset.asset','Assets/Settings/UniversalRenderPipelineGlobalSettings.asset','ProjectSettings/GraphicsSettings.asset','ProjectSettings/ProjectSettings.asset','ProjectSettings/UnityConnectSettings.asset']
 preserved=[rel for rel in preserved if (root/rel).exists()]
 preserved += [str(f.relative_to(root)) for f in (root/'Assets/ForestPrototype/Art/SectionFive').rglob('*.mat')]
 original={rel:(root/rel).read_bytes() for rel in preserved}

 try:
  code=subprocess.run([args.unity,'-batchmode','-nographics','-projectPath',str(root),'-job-worker-count','2','-executeMethod','WindowsCandidateBuilder.Build','-logFile',str(log)],cwd=root,env=env,timeout=5400).returncode
 finally:
  for f in [target,Path(str(target)+'.meta'),stamp,Path(str(stamp)+'.meta'),smoke,Path(str(smoke)+'.meta')]:f.unlink(missing_ok=True)
  changed=[rel for rel,data in original.items() if (root/rel).read_bytes()!=data]
  if changed:
   (out.parent/(build_id+'-source-normalisation.diff')).write_bytes(subprocess.check_output(['git','diff','--',*changed],cwd=root))
   for rel in changed:(root/rel).write_bytes(original[rel])
  if git('status','--porcelain',cwd=root):raise SystemExit('STOP: unexpected source drift after build; inspect Git diff')

 if code:raise SystemExit('Build failed; inspect '+str(log))
 if 'S1B_WINDOWS_BUILD_PASS' not in log.read_text(errors='replace'):raise SystemExit('Missing build success marker')
 # Retain Unity's diagnostic backups beside the candidate, never inside it.
 debug=out.parent/(build_id+'-developer-backups')
 for directory in list(out.iterdir()):
  if directory.is_dir() and any(word in directory.name.lower() for word in ['donotship','dontship','backupthisfolder']):
   debug.mkdir(exist_ok=True);shutil.move(str(directory),str(debug/directory.name))
 (out/'build-identity.json').write_text(json.dumps(identity,indent=2)+'\n')
 (out/'README.txt').write_text((root/'Docs/Verification/S1B/TesterREADME.md').read_text().replace('@BUILD_ID@',build_id).replace('@GIT_SHA@',sha))
 if args.smoke:
  print(json.dumps(dict(status='PASS',diagnosticOnly=True,buildId=build_id,gitSha=sha,path=str(out))))
  return
 notices=out/'Notices';notices.mkdir()
 shutil.copy2(root/'Tools/Build/S1B/Notices/UnityPlayer-Windows-Mono-6000.6.0f1.pdf',notices/'UnityPlayer-Windows-Mono-6000.6.0f1.pdf')
 # Preserve notices supplied by the installed packages. Editor-only packages are
 # included conservatively, without claiming their code ships in the Player.
 for package in sorted((root/'Library/PackageCache').iterdir()):
  if not package.is_dir():continue
  for name in ['LICENSE.md','LICENSE','Third Party Notices.md','Third Party Notices.txt']:
   source=package/name
   if source.is_file():dest=notices/package.name;dest.mkdir(exist_ok=True);shutil.copy2(source,dest/name)
 shutil.copy2(root/'Docs/Verification/S1B/RightsAudit.md',notices/'RightsAudit.md')
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
