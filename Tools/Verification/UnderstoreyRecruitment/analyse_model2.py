#!/usr/bin/env python3
"""Summarize actual model2 evidence; never turn relative abundance into stems."""
import csv,json,statistics
from collections import defaultdict
from pathlib import Path
r=Path(__file__).resolve().parents[3];p=r/'Build/UnderstoreyRecruitment/Model2/evidence'
rows=list(csv.DictReader((p/'model2_horizons.csv').open()))
if (p/'model2_targeted_horizons.csv').exists():rows+=list(csv.DictReader((p/'model2_targeted_horizons.csv').open()))
groups=defaultdict(list)
for row in rows:groups[(row['run'],int(row['repeat']),int(row['year']))].append(row)
summary=[]
for (run,repeat,year),part in groups.items():
 assert len(part)==3
 item={'run':run,'repeat':repeat,'year':year}
 for field in ['natural_remaining','vegetation_loss','light_loss','browse_loss','cumulative_natural_promotions','planted_alive','planted_promoted','planted_vegetation_deaths','planted_light_deaths','planted_browse_deaths']:item[field]=sum(float(x[field]) for x in part)
 for field in ['mean_bramble','mean_bracken','clearance_removals_relative','clearance_removed_planted','clearance_count','clearance_cost_cents','planting_count','shelter_count','cash_cents','living_adults','total_basal_area_m2']:item[field]=float(part[0][field])
 item['world_hash']=part[0]['world_hash'];summary.append(item)
with (p/'model2_stand_horizons.csv').open('w',newline='') as f:
 w=csv.DictWriter(f,fieldnames=list(summary[0]),lineterminator='\n');w.writeheader();w.writerows(summary)
baseline={(x['run'],x['year']):x for x in summary if x['repeat']==0}
pairs=[]
for prefix in ['thin0','thin0.2','thin0.4','thin0.6','dense-opening','lowcompetition','moderate-targeted','dense-opening-targeted','zero-competition-targeted']:
 for year in [10,25,50,100]:
  a=baseline[(prefix+'-clear0',year)];b=baseline[(prefix+'-clear1',year)]
  pairs.append({'pair':prefix,'year':year,'delta_natural_promotions':b['cumulative_natural_promotions']-a['cumulative_natural_promotions'],'delta_remaining_relative':b['natural_remaining']-a['natural_remaining'],'delta_planted_promotions':b['planted_promoted']-a['planted_promoted'],'clearance_cost_cents':b['clearance_cost_cents'],'delta_cash_cents':b['cash_cents']-a['cash_cents'],'delta_living_adults':b['living_adults']-a['living_adults'],'delta_basal_area_m2':b['total_basal_area_m2']-a['total_basal_area_m2']})
(p/'clearance_pairs.json').write_text(json.dumps(pairs,indent=2)+'\n')
if (p/'model2_annual_performance.csv').exists():
 perf=list(csv.DictReader((p/'model2_annual_performance.csv').open()));times=defaultdict(list)
 for row in perf:
  if row['repeat']=='0':continue # explicit warm-up exclusion, both models
  times[(int(row['adults']),int(row['patches']),int(row['model']))].append(float(row['annual_ms']))
 output=[]
 for adults in [336,1300,3000,5000]:
  for patches in [0,12]:
   a=times[(adults,patches,1)];b=times[(adults,patches,2)];ma=statistics.median(a);mb=statistics.median(b)
   output.append({'adults':adults,'patches':patches,'model1_median_ms':ma,'model2_median_ms':mb,'median_delta_ms':mb-ma,'median_delta_percent':100*(mb/ma-1),'model1_range_ms':[min(a),max(a)],'model2_range_ms':[min(b),max(b)],'replicates_after_warmup':len(a)})
 (p/'annual_performance_summary.json').write_text(json.dumps(output,indent=2)+'\n')
print(json.dumps({'worlds':len({(x['run'],x['repeat']) for x in summary}),'horizons':len(summary),'clearance_pairs_at100':[x for x in pairs if x['year']==100]},indent=2))
