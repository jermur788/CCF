#!/usr/bin/env python3
"""Read-only protected-call inventory: all prior Check assertions remain, except the approved intro replacements."""
import argparse, collections, re, subprocess
from pathlib import Path
root=Path(__file__).resolve().parents[3]
ap=argparse.ArgumentParser(); ap.add_argument('--base',default='8f8e6f03c65894b0d933aad23a4ad69227fb33e8'); args=ap.parse_args()
paths=['Tools/Verification/ScenarioOneRackUx/GuidedOnboardingVerification.cs',
       'Tools/Verification/SequentialObjectives/SequentialObjectivesVerification.cs',
       'Tools/Verification/ScenarioOneCombined/CombinedHudComposition.cs']
def checks(source):
 result=[]
 for m in re.finditer(r'\bCheck\(',source):
  depth=1; quote=False; escape=False; i=m.end()
  while depth and i<len(source):
   c=source[i]
   if quote:
    if escape: escape=False
    elif c=='\\': escape=True
    elif c=='"': quote=False
   elif c=='"': quote=True
   elif c=='(': depth+=1
   elif c==')': depth-=1
   i+=1
  result.append(re.sub(r'\s+',' ',source[m.start():i]).strip())
 return collections.Counter(result)
failed=[]
for path in paths:
 old=subprocess.check_output(['git','show',args.base+':'+path],cwd=root,text=True)
 current=(root/path).read_text(); missing=checks(old)-checks(current)
 allowed=[]
 if 'GuidedOnboardingVerification' in path:
  allowed=['Check(ui.Help.IsOpen && Text().Contains("Transforming a Sitka Plantation"), "fresh scenario introduction appears")',
           'Check(Text().Contains("next generation") && Text().Contains("one step at a time"), "premise and sequential learning are explained")']
 for call in allowed:
  if missing[call]: missing[call]-=1
 for call,n in missing.items():
  if n>0: failed.append(path+': '+call)
 print(path+': baseline checks='+str(sum(checks(old).values()))+' current checks='+str(sum(checks(current).values())))
if failed: raise SystemExit('PROTECTED_INTEGRITY_FAIL\n'+'\n'.join(failed))
print('PROTECTED_INTEGRITY_PASS all original non-intro Check calls retained')
