#!/usr/bin/env python3
"""Integrated source composition guard plus unchanged frozen growth checkpoints. Original branch-specific guard (check_production_v1.py) is RETIRED; see that file.

Source integrity is checked on a NORMALIZED form of each protected file, using the shared module
Tools/Verification/source_normalization.py: comments and non-semantic whitespace are ignored, so an
accepted comment-only edit (for example the model-2 default note added to SitkaGrowthModel.cs by
40b2419) no longer suppresses the 360 calibrated checkpoint comparisons. A meaningful code or
calibration change still changes the normalized hash and still fails.
See test_source_normalization.py for the positive/negative tests.
"""
from pathlib import Path
import argparse,csv,hashlib,json,re,sys
import kernel_controls as kernel
HERE=Path(__file__).parent;ROOT=HERE.parents[2]
FIELDS={'Pd':'potentialDbhGrowthCmPerYear','Ph':'potentialHeightGrowthMPerYear','Dmax':'maxDbhCm','Hmax':'maxHeightM','Ci50':'ci50'}

# Shared comment/whitespace-insensitive source normalization (single implementation).
sys.path.insert(0,str(ROOT/'Tools'/'Verification'))
import source_normalization
normalize_csharp=source_normalization.normalize_csharp
normalized_text=source_normalization.normalized_text
normalized_sha256=source_normalization.normalized_sha256

def main():
    parser=argparse.ArgumentParser();parser.add_argument('--out',type=Path,required=True);args=parser.parse_args()
    inputs=json.loads((HERE/'production_v1_inputs.json').read_text())
    contract=json.loads((HERE/'integrated_source_contract.json').read_text())
    assert set(contract)==set(inputs['preserved_files_sha256'])
    for f,entry in contract.items():
        expected=entry.get('normalized_sha256')
        assert expected,'missing normalized_sha256 for '+f
        actual=normalized_sha256(ROOT/f)
        assert actual==expected,f+' normalized source changed: expected '+expected+' got '+actual
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
    report={'scope':'NON-UNITY mathematical controls; no empirical/loader/runtime claim','source_integrity':'normalized (comments and non-semantic whitespace ignored)','checkpoint_count':len(checkpoints),'protected_files_exact':len(inputs['preserved_files_sha256']),'chosen':inputs['chosen'],'results':summaries}
    args.out.parent.mkdir(parents=True,exist_ok=True);args.out.write_text(json.dumps(report,indent=2,sort_keys=True,allow_nan=False)+'\n')
    print(f'INTEGRATED_CALIBRATION_V1_PASS checkpoints={len(checkpoints)} protected_files={len(inputs["preserved_files_sha256"])}')
if __name__=='__main__':main()
