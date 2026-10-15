#!/usr/bin/env python3
"""Integrated source composition guard plus unchanged frozen growth checkpoints. Original branch-specific guard remains check_production_v1.py."""
from pathlib import Path
import argparse,csv,hashlib,json,re
import kernel_controls as kernel
HERE=Path(__file__).parent;ROOT=HERE.parents[2]
FIELDS={'Pd':'potentialDbhGrowthCmPerYear','Ph':'potentialHeightGrowthMPerYear','Dmax':'maxDbhCm','Hmax':'maxHeightM','Ci50':'ci50'}
def main():
    parser=argparse.ArgumentParser();parser.add_argument('--out',type=Path,required=True);args=parser.parse_args()
    inputs=json.loads((HERE/'production_v1_inputs.json').read_text())
    contract=json.loads((HERE/'integrated_source_contract.json').read_text())
    assert set(contract)==set(inputs['preserved_files_sha256'])
    for f,entry in contract.items():assert hashlib.sha256((ROOT/f).read_bytes()).hexdigest()==entry['sha256'],f
    kernel.selfcheck();kernel.source_manifest();summaries={};trajectories={}
    for species,file in [('Oak','SessileOak.asset'),('Beech','BeechSpecies.asset')]:
        text=(ROOT/'Assets/ForestPrototype/Species'/file).read_text();values={k:float(re.search(r'^  '+field+r': ([\d.]+)',text,re.M).group(1)) for k,field in FIELDS.items()}
        assert values==inputs['chosen'][species],(species,values)
        for field in FIELDS.values():assert re.search(r'  # \[C\] Ordinal production calibration v1[^\n]*\n  '+field+':',text)
        result,rows=kernel.assess(species,values);assert result['survives'],result
        summaries[species]=result;trajectories[species]=rows
    checkpoints=list(csv.DictReader((HERE/'production_v1_expected.csv').open()))
    for row in checkpoints:
        d,h,promoted=trajectories[row['species']][(row['fixture'],row['treatment'])][int(row['age'])]
        assert format(d,'.12g')==row['D_cm'] and format(h,'.12g')==row['H_m'] and str(int(promoted))==row['promoted'],row
    report={'scope':'NON-UNITY mathematical controls; no empirical/loader/runtime claim','checkpoint_count':len(checkpoints),'protected_files_exact':len(inputs['preserved_files_sha256']),'chosen':inputs['chosen'],'results':summaries}
    args.out.parent.mkdir(parents=True,exist_ok=True);args.out.write_text(json.dumps(report,indent=2,sort_keys=True,allow_nan=False)+'\n')
    print(f'INTEGRATED_CALIBRATION_V1_PASS checkpoints={len(checkpoints)} protected_files={len(inputs["preserved_files_sha256"])}')
if __name__=='__main__':main()
