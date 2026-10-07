#!/usr/bin/env python3
"""Offline response-shape comparison. Synthetic competitors; no production prediction."""
import csv,json,math
from pathlib import Path
out=Path(__file__).resolve().parents[3]/'Docs/Research/UnderstoreyRecruitment/Evidence/Continuation'
out.mkdir(parents=True,exist_ok=True)
def response(form,b,k):
 c=max(b,k)
 if form=='linear':return c
 if form=='saturating':return (1-math.exp(-2*c))/(1-math.exp(-2))
 if form=='threshold_saturation':return (1-math.exp(-2*max(0,c-.25)/.75))/(1-math.exp(-2))
 # Unequal weights are deliberately hypothetical C, not inferred species biology.
 q=(b+.5*k)/1.5
 return (1-math.exp(-2*q))/(1-math.exp(-2))
forms=['linear','saturating','threshold_saturation','type_weighted_saturation']
rows=[]
# Reduced blocks: cover/type shape, browse separation, height window, strength.
cases=[(b,k,0,False,.6,.35) for b,k in [(0,0),(.2,0),(.5,0),(.9,0),(0,.5),(.5,.5),(.9,.9)]]
cases += [(c,c,b,p,.6,.35) for c in [0,.9] for b in [0,.8] for p in [False,True]]
cases += [(.9,.9,0,False,h,s) for h in [.2,.6,1.5,2] for s in [.15,.35,.6]]
for form in forms:
 for species in ['sitka-spruce','sessile-oak','beech']:
  for adapter in ['natural','planted_expected']:
   for case,(b,k,browse,protected,height,strength) in enumerate(cases):
    for light in [.08,.4,.8]:
     # Height window and light/background losses are C illustrations, not adopted rules.
     vulnerability=max(0,min(1,(1.5-height)/1.2))
     vegetation=strength*response(form,b,k)*vulnerability
     after_veg=1-vegetation
     browse_loss=after_veg*(0 if protected else .25*browse)
     other_loss=(after_veg-browse_loss)*(.1*(1-light))
     remaining=after_veg-browse_loss-other_loss
     assert 0<=vegetation<=strength and abs(1-vegetation-browse_loss-other_loss-remaining)<1e-12
     rows.append([form,species,adapter,case,b,k,light,browse,protected,height,strength,vegetation,browse_loss,other_loss,remaining])
with (out/'survival_forms.csv').open('w',newline='') as f:
 w=csv.writer(f,lineterminator='\n');w.writerow(['form','species','adapter','case','bramble_cover','bracken_cover','light','browse','protected','height_m','strength_C','vegetation_loss','browse_loss','other_loss','remaining']);w.writerows(rows)
summary={'rows':len(rows),'forms':forms,'synthetic':True,'stock_balance_tolerance':1e-12,'selected_for_next_trial':'linear','production_adoption':False,'reason':'No empirical curve discriminator; linear has no shape or unequal-type coefficients. Unequal type weights change equal-cover responses but lack support.'}
(out/'survival_forms.json').write_text(json.dumps(summary,indent=2)+'\n')
print(json.dumps(summary))
