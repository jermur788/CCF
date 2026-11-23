import re, json
for vp, tag in (('1280x720', 'fin2-ft-1280'), ('1920x1080', 'fin2-ft-1920')):
    p = f'/mnt/c/Users/hp/AppData/Local/Temp/review-r3/out/{tag}/run.log'
    lines = [re.sub(r'/s/[0-9a-f]{64}/', '/s/<c>/', l.rstrip('\n')) for l in open(p, encoding='utf-8', errors='replace')]
    print('=' * 10, vp, 'PASS', sum(1 for l in lines if l.startswith('PASS')), 'FAIL', sum(1 for l in lines if l.startswith('FAIL')), 'INFO', sum(1 for l in lines if l.startswith('INFO')))
    for l in lines:
        if 'ramp on the real layer' in l:
            rows = json.loads(l.split(' :: ', 1)[1]); print('ramp  L -> opacity/fill/meanRGBdelta:', [(r['L'], r['op'], r['fill'], r['meanSummedRgbDelta']) for r in rows])
        elif 'AC01 (attack 1): with Inspect ground ON, EVERY crown selection' in l:
            head, js = l.split(' :: ', 1); v = json.loads(js)
            print(head[:5], 'P6 ground-ON crown selections:', len(v), 'all ok', all(x['ok'] for x in v), 'scrollTop', {x['scrollTop'] for x in v}, 'lastBottom', [x['lastBottom'] for x in v][:3], 'dockTop', [x['dockTop'] for x in v][:3], 'addBottom', [x['addBottom'] for x in v][:3], 'groundTop', [x['groundTop'] for x in v][:3], 'dockBottom', [x['dockBottom'] for x in v][:3])
        elif l.startswith(('PASS', 'FAIL')) and any(k in l for k in ('AC04: Approve approves', 'AC21: Execute fells exactly', 'AC15: with stage4 null', 'AC15: a paired re-read', 'AC15/AC21: a real Execute', 'AC21: Year+1', 'AC01 (LOADED session, plan APPROVED', 'AC20: no CSP', 'AC20: no script', 'AC20: no HTTP', 'AC16: no horizontal page overflow at the end')):
            print(l[:330])
        elif 'smallest REAL engine light index' in l:
            print(l[:700])
        elif 'http' in l and 'HTTP statuses seen' in l:
            print(l[:200])
