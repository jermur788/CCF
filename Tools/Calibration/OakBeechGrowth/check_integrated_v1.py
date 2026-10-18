#!/usr/bin/env python3
"""Integrated source composition guard plus unchanged frozen growth checkpoints. Original branch-specific guard remains check_production_v1.py.

Source integrity is checked on a NORMALIZED form of each protected file: comments and non-semantic
whitespace are ignored, so an accepted comment-only edit (for example the model-2 default note added
to SitkaGrowthModel.cs by 40b2419) no longer suppresses the 360 calibrated checkpoint comparisons.
A meaningful code or calibration change still changes the normalized hash and still fails.
See test_source_normalization.py for the positive/negative tests.
"""
from pathlib import Path
import argparse,csv,hashlib,json,re
import kernel_controls as kernel
HERE=Path(__file__).parent;ROOT=HERE.parents[2]
FIELDS={'Pd':'potentialDbhGrowthCmPerYear','Ph':'potentialHeightGrowthMPerYear','Dmax':'maxDbhCm','Hmax':'maxHeightM','Ci50':'ci50'}

# --- Source-integrity normalization ---------------------------------------------------------
# C# is normalized by removing comments and collapsing whitespace OUTSIDE string/char literals
# (literal text is preserved verbatim, including interpolated-string expressions). Non-C# protected
# files (Unity YAML) only normalize line endings, trailing whitespace and blank lines.
def _read_char(text,i):
    frag=[];n=len(text)
    while i<n:
        c=text[i]
        if c=='\\' and i+1<n:frag.append(c);frag.append(text[i+1]);i+=2;continue
        frag.append(c);i+=1
        if c=="'":break
    return ''.join(frag),i

def _read_string(text,i,verbatim,interpolated):
    """i points just after the opening quote. Returns (fragment_including_closing_quote, next_index)."""
    frag=[];n=len(text)
    while i<n:
        c=text[i]
        if verbatim:
            if c=='"':
                if i+1<n and text[i+1]=='"':frag.append('""');i+=2;continue
                return ''.join(frag)+'"',i+1
            if interpolated and c=='{':
                if i+1<n and text[i+1]=='{':frag.append('{{');i+=2;continue
                expr,i=_norm_code(text,i+1,stop_on_brace=True)
                frag.append('{'+''.join(expr)+'}');i+=1;continue
            if interpolated and c=='}':
                if i+1<n and text[i+1]=='}':frag.append('}}');i+=2;continue
            frag.append(c);i+=1
        else:
            if c=='\\' and i+1<n:frag.append(c);frag.append(text[i+1]);i+=2;continue
            if c=='"':return ''.join(frag)+'"',i+1
            if interpolated and c=='{':
                if i+1<n and text[i+1]=='{':frag.append('{{');i+=2;continue
                expr,i=_norm_code(text,i+1,stop_on_brace=True)
                frag.append('{'+''.join(expr)+'}');i+=1;continue
            if interpolated and c=='}':
                if i+1<n and text[i+1]=='}':frag.append('}}');i+=2;continue
            frag.append(c);i+=1
    return ''.join(frag),i

def _norm_code(text,i,stop_on_brace):
    out=[];n=len(text);pending=False;depth=0
    def flush():
        nonlocal pending
        if pending and out:out.append(' ')
        pending=False
    while i<n:
        c=text[i]
        if stop_on_brace and c=='}' and depth==0:return out,i
        if c in ' \t\r\n\f\v':pending=True;i+=1;continue
        if c=='/' and i+1<n and text[i+1]=='/':
            pending=True;i+=2
            while i<n and text[i] not in '\r\n':i+=1
            continue
        if c=='/' and i+1<n and text[i+1]=='*':
            pending=True;i+=2
            while i+1<n and not (text[i]=='*' and text[i+1]=='/'):i+=1
            i=min(i+2,n);continue
        flush()
        if c=='$' and i+1<n and text[i+1]=='"':
            frag,i=_read_string(text,i+2,False,True);out.append('$"'+frag);continue
        if c=='@' and i+1<n and text[i+1]=='"':
            frag,i=_read_string(text,i+2,True,False);out.append('@"'+frag);continue
        if c=='$' and i+2<n and text[i+1]=='@' and text[i+2]=='"':
            frag,i=_read_string(text,i+3,True,True);out.append('$@"'+frag);continue
        if c=='@' and i+2<n and text[i+1]=='$' and text[i+2]=='"':
            frag,i=_read_string(text,i+3,True,True);out.append('@$"'+frag);continue
        if c=='"':
            frag,i=_read_string(text,i+1,False,False);out.append('"'+frag);continue
        if c=="'":
            frag,i=_read_char(text,i+1);out.append("'"+frag);continue
        if c=='{':depth+=1;out.append('{');i+=1;continue
        if c=='}':depth-=1;out.append('}');i+=1;continue
        out.append(c);i+=1
    return out,i

def normalize_csharp(text):
    out,_=_norm_code(text,0,stop_on_brace=False)
    return ''.join(out).strip()

def normalize_other(text):
    text=text.replace('\r\n','\n').replace('\r','\n')
    lines=[line.rstrip() for line in text.split('\n')]
    return re.sub(r'\n\s*\n+','\n','\n'.join(lines)).strip()

def normalized_text(path):
    text=path.read_text(encoding='utf-8')
    return normalize_csharp(text) if path.suffix=='.cs' else normalize_other(text)

def normalized_sha256(path):
    return hashlib.sha256(normalized_text(path).encode('utf-8')).hexdigest()

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
