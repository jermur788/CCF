#!/usr/bin/env python3
"""Verify accepted production ownership, independent manager/UI merge composition and protected invariants. No Unity."""
from pathlib import Path
import argparse,subprocess,json
P=Path(__file__).resolve().parents[3]
BASE='c1564b857972ee18917e8a461c457533a54cc3f1'
parser=argparse.ArgumentParser();parser.add_argument('--out',type=Path,required=True);args=parser.parse_args()
OUT=args.out;OUT.mkdir(parents=True,exist_ok=True)
def git(*args):return subprocess.check_output(['git','-C',str(P),*args],text=True).strip()
import hashlib,tempfile
sources=['90e3496873da4e3b23038993ca5bd2fec3c2e7f1','47539847d7e2531824c30f3dfe635d55404699a8','796da99029190ef71e68d5fa722f25240e696318','eb7fd3d8c0baac1d71d43ebfce035591b9e4a8ac','a93a7220fd8b4d8246b3613d913360dad06988e8','549eae3318f1bb4d01a2853384b58f282f7f4e73','678283642aaeb60221eda0fe8cb3f97e27958048','8d21e27046fdeae4b2cda710938daf19757fbc3d','855827f96225003c6387b0da43107c509603391a','feb754cbe263c41a8d2f6fe33cd9fc39888c7e60']
changed=git('diff',BASE,'--name-only','--','Assets','Packages','ProjectSettings').splitlines();results={}
merged={'Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.cs':('eb7fd3d8c0baac1d71d43ebfce035591b9e4a8ac','b4894ccf7966cfa5fa4d9c0df653d99b1846fa5a','549eae3318f1bb4d01a2853384b58f282f7f4e73'),'Assets/ForestPrototype/UI/AnnualReviewView.cs':('a93a7220fd8b4d8246b3613d913360dad06988e8','b4894ccf7966cfa5fa4d9c0df653d99b1846fa5a','549eae3318f1bb4d01a2853384b58f282f7f4e73')}
for path in changed:
 assert not path.startswith(('Packages/','ProjectSettings/')) and not path.endswith(('.unity','.prefab','.mat')),path
 actual=(P/path).read_bytes() if (P/path).exists() else subprocess.check_output(['git','-C',str(P),'show',':'+path])
 if path in merged:
  with tempfile.TemporaryDirectory() as td:
   paths=[]
   for n,h in enumerate(merged[path]):
    q=Path(td)/str(n);q.write_bytes(subprocess.check_output(['git','-C',str(P),'show',h+':'+path]));paths.append(str(q))
   r=subprocess.run(['git','merge-file','-p',*paths],capture_output=True);assert r.returncode==0,(path,r.stderr);assert actual==r.stdout,path
  results[path]={'owners':list(merged[path]),'composition':'independent clean three-way merge','sha256':hashlib.sha256(actual).hexdigest()}
 else:
  matches=[]
  for h in sources:
   r=subprocess.run(['git','-C',str(P),'show',h+':'+path],capture_output=True)
   if r.returncode==0 and r.stdout==actual:matches.append(h)
  assert matches,('unowned production change',path);results[path]={'exact_accepted_sources':matches,'sha256':hashlib.sha256(actual).hexdigest()}
for path in ['Assets/ForestPrototype/ForestSaveData.cs','Assets/ForestPrototype/ForestSaveController.cs','Assets/ForestPrototype/ForestSaveValidation.cs','Assets/ForestPrototype/ScenarioOne/ScenarioOneObjectives.cs','Assets/ForestPrototype/ScenarioOne/ScenarioOneDefinition.cs','Assets/ForestPrototype/Species/SitkaSpruce.asset']:
 assert (P/path).read_bytes()==subprocess.check_output(['git','-C',str(P),'show',BASE+':'+path]),path
manager=(P/'Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.cs').read_text();assert 'public const int NewGameGrowthModel = GrowthModel.SiteClassDensity;' in manager
assert not git('diff',BASE,'--name-only','--','Docs/Project')
(OUT/'accepted-production-composition.json').write_text(json.dumps(results,indent=2)+'\n')
print('ACCEPTED_SOURCE_COMPOSITION_PASS files='+str(len(results))+' merged_owners=2 schema_objectives_default_sitka_and_canonical_preserved=true')
