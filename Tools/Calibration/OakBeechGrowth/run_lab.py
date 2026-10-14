#!/usr/bin/env python3
"""Deterministic, disposable Kernel A lab. No Unity, dependencies or source edits."""
import argparse,csv,hashlib,itertools,json,math
from collections import Counter
from pathlib import Path
ROOT=Path(__file__).resolve().parents[3]
RANGES={'Oak':{'Pd':[.4,.8,1.2],'Ph':[.4,.65,.9],'Dmax':[80,120,160],'Hmax':[35,42.5,50],'Ci50':[2,5,8]},'Beech':{'Pd':[.4,.9,1.4],'Ph':[.4,.75,1.1],'Dmax':[80,120,160],'Hmax':[35,42.5,50],'Ci50':[3,6.5,10]}}
TREATMENTS={'dense':8.,'moderate':3.,'strong':1.}
CHECKPOINTS=[5,10,20,25,30,40,50,75,100,125,150,200]
def clamp(x,a,b):return min(b,max(a,x))
def step(d,h,p,ci,site=1.):
    dd=p['Pd']*site*clamp(1-d/p['Dmax'],0,1)/(1+ci/p['Ci50'])
    dh=p['Ph']*site*clamp(1-h/p['Hmax'],0,1)
    return clamp(d+dd,1,200),clamp(h+dh,.1,60)
def trajectory(p,ci,initial=None,release=False,generic=False):
    age,d,h=(50,*initial) if initial else (0,2.,.5) if generic else (0,0.,.15)
    promoted=initial is not None or generic
    rows={age:(d,h,promoted)}
    while age<200:
        age+=1
        if promoted:d,h=step(d,h,p,1. if release and age>50 else ci)
        else:
            h+=.35 # shipped full-light/site1, zero-browse surviving-stock control
            if h>=3.5:d=clamp(h*1.5,2,20);promoted=True
        rows[age]=(d,h,promoted)
    return rows

def selfcheck():
    p={'Pd':.8,'Ph':.65,'Dmax':120.,'Hmax':42.5,'Ci50':5.}
    d,h=20.,15.; a=p['Pd']/(1+3/p['Ci50'])/p['Dmax'];b=p['Ph']/p['Hmax']
    for year in range(1,151):
        d,h=step(d,h,p,3)
        assert abs(d-(p['Dmax']-(p['Dmax']-20)*(1-a)**year))<1e-10
        assert abs(h-(p['Hmax']-(p['Hmax']-15)*(1-b)**year))<1e-10
    assert step(160,50,p,3)==(160,50)
    assert trajectory(p,3)[9][2] is False and trajectory(p,3)[10][2] is True
    # Genuine negative controls of scoring properties, not production candidates.
    assert step(20,15,p,1)[0]>step(20,15,p,8)[0]
    assert step(20,15,p,1)[1]==step(20,15,p,8)[1]

def source_manifest():
    paths=['Assets/ForestPrototype/ForestEcologyController.cs','Assets/ForestPrototype/ForestTree.cs','Assets/ForestPrototype/JuvenileEcologyRules.cs','Assets/ForestPrototype/TreeSpeciesDefinition.cs','Assets/ForestPrototype/Species/SessileOak.asset','Assets/ForestPrototype/Species/BeechSpecies.asset']
    hashes={f:hashlib.sha256((ROOT/f).read_bytes()).hexdigest() for f in paths}
    ec=(ROOT/paths[0]).read_text();tree=(ROOT/paths[1]).read_text();juv=(ROOT/paths[2]).read_text()
    for expression in ['Mathf.Clamp01(1f - tree.Diameter / treeSpecies.MaxDbhCm)','1f / (1f + ci / treeSpecies.Ci50)','Mathf.Clamp01(1f - tree.Height / treeSpecies.MaxHeightM)']:assert expression in ec,expression
    assert 'Mathf.Clamp(diameterCm + dbhDeltaCm, 1f, 200f)' in tree
    assert 'Mathf.Clamp(heightMeters + heightDeltaM, 0.1f, 60f)' in tree
    assert 'Mathf.Clamp(height * 1.5f, 2f, 20f)' in juv
    for f in paths[-2:]:
        s=(ROOT/f).read_text()
        for field in ['regenHeightGrowthMPerYear: 0.35','promotionHeightM: 3.5']:assert field in s
    return hashes

def assess(species,p):
    reasons=set(); trajectories={}; score=0.;first={};juvenile={}
    for treatment,ci in TREATMENTS.items():
        for fixture,initial,generic,release in [('stock',None,False,False),('generic_control',None,True,False),('release50',None,False,True)]+[(f'adult_D{d}_H{h}',(d,h),False,False) for d in [20,40] for h in [15,25]]:
            tr=trajectory(p,ci,initial,release,generic);trajectories[(fixture,treatment)]=tr
            previous=None
            for age,(d,h,promoted) in tr.items():
                if not math.isfinite(d+h) or min(d,h)<0:reasons.add('nonfinite_or_negative')
                if previous and (d<previous[0]-1e-10 or h<previous[1]-1e-10):reasons.add('decline')
                if promoted and (d>=200 or h>=60):reasons.add('storage_clamp')
                if promoted and (d>p['Dmax']+1e-10 or h>p['Hmax']+1e-10):reasons.add('overshoot')
                if fixture=='stock' and age<50 and (d>=.95*p['Dmax'] or h>=.95*p['Hmax']):reasons.add('early_saturation')
                previous=(d,h)
            if fixture=='stock':
                juvenile[treatment]=tr[25][0]
                if tr[25][0]>=40:reasons.add('new_stock_large_at25')
                bands=[(50,20,25),(100,30,35)] if species=='Oak' else [(50,25,30),(100,35,40)]
                for age,lo,hi in bands:score+=max(lo-tr[age][1],0,tr[age][1]-hi)/(hi-lo)/6
            if fixture in ['stock','release50']:
                first[fixture+'/'+treatment]=next((a for a,(d,h,promoted) in tr.items() if d>=60),None)
                if species=='Oak' and first[fixture+'/'+treatment] is not None and first[fixture+'/'+treatment]<80:reasons.add('oak_proxy_before80')
    for fixture in ['stock','generic_control']+[f'adult_D{d}_H{h}' for d in [20,40] for h in [15,25]]:
        ds=[trajectories[(fixture,t)][100][0] for t in TREATMENTS]
        hs=[trajectories[(fixture,t)][100][1] for t in TREATMENTS]
        if not ds[2]>ds[1]>ds[0]:reasons.add('no_positive_release')
        if max(hs)-min(hs)>1e-10:reasons.add('height_release_invented')
    return dict(species=species,parameters=p,survives=not reasons,reasons=sorted(reasons),height_design_distance=score,stock_D25=juvenile,oak_D60_proxy_ages=first if species=='Oak' else {},strong_dense_D100_ratio=trajectories[('stock','strong')][100][0]/trajectories[('stock','dense')][100][0]),trajectories

def main():
    parser=argparse.ArgumentParser();parser.add_argument('--out',type=Path,default=Path(__file__).parent/'Results');args=parser.parse_args();args.out.mkdir(parents=True,exist_ok=True)
    selfcheck();sources=source_manifest(); results=[];data=[]
    for species,ranges in RANGES.items():
        for i,values in enumerate(itertools.product(*ranges.values()),1):
            p=dict(zip(ranges,values));result,tr=assess(species,p);result['id']=f'{species}-{i:03d}';results.append(result)
            for (fixture,treatment),states in tr.items():
                for age in CHECKPOINTS:
                    if age in states:
                        d,h,promoted=states[age];data.append([result['id'],species,fixture,treatment,age,format(d,'.12g'),format(h,'.12g'),int(promoted)])
    summary={'classification':'B','reason':'Many bounded families survive; targets identify release direction, not unique empirical rates; height bands are provisional [C].','candidate_count':len(results),'sources_sha256':sources,'preregistration_sha256':hashlib.sha256((ROOT/'Docs/Research/OakBeechAdultParity/OakBeechCalibrationPreregistration_v1.md').read_bytes()).hexdigest(),'species':{},'sensitivity':{},'baseline':{}}
    for species,ranges in RANGES.items():
        subset=[r for r in results if r['species']==species];surv=[r for r in subset if r['survives']];rejected=[r for r in subset if not r['survives']]
        summary['species'][species]={'survivors':len(surv),'rejected':len(rejected),'rejection_counts':dict(Counter(reason for r in rejected for reason in r['reasons'])),'survivor_parameter_support':{key:sorted(set(r['parameters'][key] for r in surv)) for key in ranges},'lowest_diagnostic_distance':min((r['height_design_distance'] for r in surv),default=None),'lowest_distance_examples':[r for r in sorted(surv,key=lambda r:(r['height_design_distance'],r['id']))[:6]],'D25_range':[min(r['stock_D25'][t] for r in surv for t in TREATMENTS),max(r['stock_D25'][t] for r in surv for t in TREATMENTS)] if surv else None}
        summary['sensitivity'][species]={key:[{'value':value,'survival_fraction':sum(r['survives'] for r in subset if r['parameters'][key]==value)/81,'mean_design_distance':sum(r['height_design_distance'] for r in subset if r['parameters'][key]==value)/81} for value in vals] for key,vals in ranges.items()}
        baseline=dict(Pd=.4,Ph=.4,Dmax=120,Hmax=40,Ci50=3);summary['baseline'][species]=assess(species,baseline)[0]
    if any(summary['species'][s]['survivors']==0 for s in RANGES):summary['classification']='C'
    for name,obj in [('candidates.json',results),('summary.json',summary)]: (args.out/name).write_text(json.dumps(obj,indent=2,sort_keys=True,allow_nan=False)+'\n')
    with (args.out/'trajectories.csv').open('w',newline='') as f:
        w=csv.writer(f,lineterminator='\n');w.writerow(['candidate','species','fixture','treatment','age','D_cm','H_m','promoted']);w.writerows(data)
    print(json.dumps({'classification':summary['classification'],'candidates':len(results),'species':{s:{k:v for k,v in d.items() if k in ['survivors','rejected','rejection_counts','D25_range','lowest_diagnostic_distance']} for s,d in summary['species'].items()},'checkpoints':len(data)}))
if __name__=='__main__':main()
