#!/usr/bin/env python3
"""Offline causal candidates, deliberately not a reproduction of Unity ecology.
All coefficients are C (calibration) or I (inference); outputs are relative
response multipliers for a virtual patch, never predicted physical stems.
"""
import csv,itertools,json,math
from pathlib import Path
ROOT=Path(__file__).resolve().parents[3]
OUT=ROOT/'Docs/Research/UnderstoreyRecruitment/Evidence';OUT.mkdir(parents=True,exist_ok=True)
def shape(form,cover,light):
 if form=='linear':return cover
 if form=='saturating':return cover/(.3+cover)*1.3
 if form=='threshold_saturation':return max(0,cover-.2)/(.3+max(0,cover-.2))*1.3
 if form=='type_weighted':return cover # Requires independently measured competitor cover, not total vegetation.
 if form=='light_cover':return cover*(.4+.6*light)
 raise ValueError(form)
rows=[]
for form,strength,process,light,cover,browse,protected,recruitment,clearance in itertools.product(
 ['linear','saturating','threshold_saturation','type_weighted','light_cover'],[.15,.35,.6],['establishment','survival','growth','establishment_growth','survival_growth'],[.08,.4,.8],[0,.3,.7,1],[0,.8],[False,True],[0,1],['none','once','repeat5']):
 # Virtual cover recovers toward initial cover at the current placeholder colonisation rate.
 c=cover;pop=1.;growth=0.;est=0.;veg_loss=0.;browse_loss=0.;cost_events=0
 for year in range(1,21):
  if clearance=='once' and year==1 or clearance=='repeat5' and (year==1 or year%5==0):c=0.;cost_events+=1
  effect=min(.95,strength*shape(form,c,light));factor=1-effect
  accepted=recruitment*max(0,light)* (factor if 'establishment' in process else 1)
  est+=accepted
  # Light mortality is not invented here: these are incremental vegetation/browse multipliers only.
  vloss=pop*effect if 'survival' in process else 0
  pop-=vloss;veg_loss+=vloss
  bloss=pop*(0 if protected else browse*.04);pop-=bloss;browse_loss+=bloss
  pop+=accepted
  growth+=light*(factor if 'growth' in process else 1)*(1-(0 if protected else browse)*.9)
  c+=.3*(cover-c)
  assert pop>=0 and (recruitment!=0 or est==0)
  if year in [10,20]:rows.append([form,strength,'C',process,light,cover,browse,protected,recruitment,clearance,year,pop,growth,est,veg_loss,browse_loss,c,cost_events])
with (OUT/'candidate_sensitivity.csv').open('w',newline='') as f:
 w=csv.writer(f,lineterminator='\n');w.writerow(['form','strength','grade','process','light','initial_cover','browse','protected','recruitment_source','clearance','year','virtual_abundance','relative_growth_integral','accepted_virtual_recruitment','vegetation_loss','browse_loss','recovered_cover','treatment_events']);w.writerows(rows)
# Existing recurrence, with fixed environment (analytical check only; confirm via Unity next).
with (OUT/'recolonisation_analytical.csv').open('w',newline='') as f:
 w=csv.writer(f,lineterminator='\n');w.writerow(['light','group','year','target','after_clearance','fraction_recovered','coefficient_grade'])
 for light in [.08,.4,.8]:
  clamp=lambda x:max(0,min(1,x))
  targets={'ferns':clamp((light-.06)/.24)*clamp((.75-light)/.45)*.75,'grasses':clamp((light-.4)/.4)*.6,'forbs':clamp((light-.32)/.38)*.55,'shrubs':clamp((light-.5)/.4)*.7}
  for group,target in targets.items():
   for year in [1,3,5,10,20]:w.writerow([light,group,year,target,target*(1-.7**year),1-.7**year,'D existing placeholder'])
summary={'rows':len(rows),'scope':'virtual response multipliers, not Unity trajectories','coefficients':'C/I; no empirical Irish universal coefficient','no_seed_checked':True,'models':5,'processes':5,'status':'candidate comparison only; no production selection'}
(OUT/'candidate_summary.json').write_text(json.dumps(summary,indent=2)+'\n');print(json.dumps(summary))
