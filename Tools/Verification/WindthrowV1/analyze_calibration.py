#!/usr/bin/env python3
"""Combine complete storm worlds, excluding the interrupted partial world.
Never infer a completed world from merely having a year25 row.
"""
from pathlib import Path
import csv,json,re,statistics,collections
r=Path(__file__).resolve().parents[3]
root=r/'Build/WindthrowV1'
worlds={};events=[];provenance=[]
for gate in ('StormLongRunCalibration','StormLongRunResume'):
 out=root/gate;log=out/(gate+'.log')
 if not log.exists():continue
 content=log.read_text(errors='replace')
 done=set(re.findall(r'STORM_LONG_RUN_WORLD_DONE (\S+) year=',content))
 rows=list(csv.DictReader((out/'evidence/storm_long_run_years.csv').open()))
 complete={run:[row for row in rows if row.get('run')==run and row.get('failed_objectives') is not None] for run in done}
 for run,data in complete.items():
  if run in worlds:raise RuntimeError('Duplicate complete world '+run)
  if not data:raise RuntimeError('Completed log world has no complete CSV rows '+run)
  worlds[run]=data
 events.extend(row for row in csv.DictReader((out/'evidence/storm_long_run_events.csv').open()) if row['run'] in done)
 provenance.append(dict(gate=gate,complete_worlds=len(done),status='PASS' if ('STORM_LONG_RUN_RESUME_PASS' in content or 'STORM_LONG_RUN_CALIBRATION_PASS' in content) else 'INTERRUPTED_OR_RUNNING'))
groups=collections.defaultdict(list)
for run,rows in worlds.items():
 last=rows[-1];groups[(round(float(last['frequency']),2),last['weights'],last['managed'])].append(rows)
summary=[]
for (frequency,weights,managed),data in sorted(groups.items()):
 for horizon in (25,50,100):
  reached=[next((row for row in rows if int(row['year'])==horizon),None) for rows in data]
  valid=[row for row in reached if row is not None]
  record=dict(frequency=frequency,weights=weights,managed=managed,horizon=horizon,completed_worlds=len(data),expected_worlds=8,pending_worlds=8-len(data),reached_horizon=len(valid),early_stopped=sum(row is None for row in reached),scenario_completed=sum(row['completed']=='True' for row in valid),current_objectives_all_achieved=sum(not row['failed_objectives'] for row in valid))
  for key in ('cash_cents','minimum_cash_cents','living_trees','original_living','crops','original_crops_lost','canopy','light','regeneration_density','promoted_living','planted_alive','regeneration_cells','bramble','bracken','deadwood_m3','storm_events','windthrow_victims','windthrow_volume_m3'):
   values=[float(row[key]) for row in valid]
   for suffix,fn in (('mean',statistics.mean),('min',min),('max',max)):
    record[key+'_'+suffix]=fn(values) if values else None
  record['no_event_worlds']=sum(int(row['storm_events'])==0 for row in valid)
  summary.append(record)
out=root/'CombinedCalibration';out.mkdir(exist_ok=True)
(out/'summary.json').write_text(json.dumps(dict(planned_worlds=144,complete_worlds=len(worlds),all_worlds_complete=len(worlds)==144,provenance=provenance,groups=summary),indent=2)+'\n')
if summary:
 with (out/'summary.csv').open('w') as stream:
  writer=csv.DictWriter(stream,fieldnames=list(summary[0]));writer.writeheader();writer.writerows(summary)
with (out/'complete_world_events.csv').open('w') as stream:
 if events:writer=csv.DictWriter(stream,fieldnames=list(events[0]));writer.writeheader();writer.writerows(events)
horizons=[row for run in sorted(worlds) for row in worlds[run] if int(row['year']) in (25,50,100)]
with (out/'complete_world_horizons.csv').open('w') as stream:
 if horizons:writer=csv.DictWriter(stream,fieldnames=list(horizons[0]));writer.writeheader();writer.writerows(horizons)
print(json.dumps(dict(complete_worlds=len(worlds),planned_worlds=144,groups=len(groups),all_worlds_complete=len(worlds)==144)))
