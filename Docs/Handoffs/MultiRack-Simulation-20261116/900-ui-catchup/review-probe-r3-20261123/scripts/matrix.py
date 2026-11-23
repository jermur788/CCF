import json, re
for vp, tag in (('1280x720', 'fin2-ft-1280'), ('1920x1080', 'fin2-ft-1920')):
    p = f'/mnt/c/Users/hp/AppData/Local/Temp/review-r3/out/{tag}/run.log'
    txt = open(p, encoding='utf-8', errors='replace').read().splitlines()
    print('=' * 8, vp)
    for line in txt:
        if line.startswith('PASS') or line.startswith('FAIL') or line.startswith('INFO'):
            if 'DEFECT-2: for ok / refused / guard / transport' in line:
                head, js = line.split(' :: ', 1)
                v = json.loads(js)
                print(head[:6], 'matrix cases:', len(v), 'all ok:', all(x.get('ok') for x in v))
                print(f"  {'answer':<10}{'interference':<9}{'ok':<6}{'insp':<6}{'alert':<6}{'announce':<9}{'noWrite':<8}{'serverSame':<11}requests-after-release")
                for x in v:
                    print(f"  {x['kind']:<10}{x['action']:<9}{str(x['ok']):<6}{str(x['okInsp']):<6}{str(x['okAlert']):<6}{str(x['okAnn']):<9}{str(x['noWrite']):<8}{str(x['serverUnchanged']):<11}{x['reqs']}")
            elif 'DEFECT-2 control' in line:
                head, js = line.split(' :: ', 1)
                print(head[:300])
                for c in json.loads(js):
                    print(f"  control {c['kind']:<10} selected={c['insp']:<10} expectedTopmost={c['expectHit']:<10} alertShown={c['alertShown']} alert={c['alert'][:70]!r}")
            elif ('DEFECT-2' in line and ('stale REFUSED' in line)) or 'EXPLORE' in line or 'F-05 (D' in line or 'stale-REFUSED side effect' in line:
                print(line[:1000])
