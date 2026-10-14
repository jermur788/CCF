#!/usr/bin/env python3
"""Read-only review of exact UX source; independently recompute presentation hashes."""
from pathlib import Path
import argparse,hashlib,json,re,subprocess
HERE=Path(__file__).resolve().parent;ROOT=HERE.parents[3]
SOURCE='549eae3318f1bb4d01a2853384b58f282f7f4e73';BASE='84e881aa6d0e96c5752f09492d5db37d2cc8925e'
ALLOWED={'Assets/ForestPrototype/UI/Resources/ScenarioOneUi.uss','Assets/ForestPrototype/UI/ScenarioOneUiRoot.cs','Assets/ForestPrototype/UI/TreeInspectionView.cs','Assets/ForestPrototype/UI/UiKit.cs','Assets/ForestPrototype/UI/WorkPlanOverview.cs'}
def git(*args):return subprocess.check_output(['git','-C',str(ROOT),*args],text=True)
def subtitle(source):
    text=git('show',source+':Assets/ForestPrototype/UI/WorkPlanOverview.cs')
    start=text.index('UiKit.Add(card, "WHAT YOU ARE LEAVING"');end=text.index('ResidualStandResult stand',start)
    strings=re.findall(r'"([^"]*)"',text[start:end]);return ''.join(strings[2:-1])
def stable_hash(text):
    h=1469598103934665603;raw=text.encode('utf-16-le')
    for i in range(0,len(raw),2):h=((h^int.from_bytes(raw[i:i+2],'little'))*1099511628211)&((1<<64)-1)
    return f'{h:016X}'
def main():
    parser=argparse.ArgumentParser();parser.add_argument('--out',type=Path,required=True);args=parser.parse_args()
    assert subprocess.run(['git','-C',str(ROOT),'merge-base','--is-ancestor',BASE,SOURCE]).returncode==0
    paths=git('diff','--name-only',BASE,SOURCE).splitlines();full=git('diff',BASE,SOURCE)
    assert all(f in ALLOWED or f.startswith(('Docs/Verification/','Tools/Verification/')) for f in paths),'RESULT C unrelated changed path'
    assert {f for f in paths if f.startswith('Assets/')}==ALLOWED
    manifest={f:hashlib.sha256(git('show',SOURCE+':'+f).encode()).hexdigest() for f in paths}
    text=(ROOT/'Docs/Verification/ScenarioOneUxReview/Evidence/p3-hashed-text.txt').read_text()
    new=subtitle(SOURCE);old=subtitle('b4894ccf7966cfa5fa4d9c0df653d99b1846fa5a');assert new==subtitle(BASE)
    assert text.count(new)==1
    newhash=stable_hash(text);oldhash=stable_hash(text.replace(new,old));assert newhash=='AF7405918195F0DE' and oldhash=='F7C2FC966816BBAD',(newhash,oldhash)
    assert 'StableHash(leaving + "\\n" + money)' in git('show',SOURCE+':Tools/Verification/ScenarioOneCompletion/P3/WorkPlanResidualVerification.cs')
    diagnostic=git('show',SOURCE+':Tools/Verification/ScenarioOneUx/Qualification/make_startup_diagnostic.py')
    startup=git('show',SOURCE+':Tools/Verification/Startup/StartupScreenVerification.cs')
    stale='no key opened a forest panel or advanced time' not in startup
    result={'result':'B','source':SOURCE,'base':BASE,'ancestry_verified':True,'complete_diff_sha256':hashlib.sha256(full.encode()).hexdigest(),'files':manifest,'production_paths':sorted(ALLOWED),'new_p3_text_hash':newhash,'old_p3_text_hash_with_only_subtitle_reverted':oldhash,'unchanged_recorded_world_hash':'EBE7A228F630AE41','diagnostic_generator_stale_after_startup_correction':stale,'scope':'Source and retained-log review only; no Unity execution'}
    args.out.parent.mkdir(parents=True,exist_ok=True);args.out.write_text(json.dumps(result,indent=2,sort_keys=True)+'\n')
    print('UX_FOLLOWUP_SOURCE_REVIEW_PASS result=B files='+str(len(paths))+' old='+oldhash+' new='+newhash+' no_simulation_save_ecology_delta=True')
if __name__=='__main__':main()
