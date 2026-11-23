import json
base = '/mnt/c/Users/hp/AppData/Local/Temp/review-r3/out'
print('--- first (superseded) pass: trees recorded with 0 rows per state')
for vp, d in (('1280x720', 'sweep1280'), ('1920x1080', 'sweep1920')):
    for label in ('clean', 'queued4', 'approved-crop', 'after-execute', 'year1'):
        j = json.load(open(f'{base}/{d}/sweep-{label}-{vp}.json'))['recs']
        print(f"  {vp} {label:<14} trees={len(j)} n==0: {sum(1 for r in j if r['n'] == 0)}")
print('--- final pass (fin-sw): n==0 per state (expect 0) and the smallest real L year 3 B')
for vp, d in (('1280x720', 'fin-sw-1280'), ('1920x1080', 'fin-sw-1920')):
    z = 0
    for label in ('clean', 'queued4', 'approved-crop', 'after-execute', 'year1'):
        j = json.load(open(f'{base}/{d}/sweep-{label}-{vp}.json'))['recs']; z += sum(1 for r in j if r['n'] == 0)
    print(' ', vp, 'n==0 total', z)
for line in open(f'{base}/fin2-ft-1280/run.log', encoding='utf-8', errors='replace'):
    if 'smallest REAL engine light index' in line:
        for y in json.loads(line.split(' :: ', 1)[1]):
            print('  year', y['year'], [(a['area'], round(a['min'], 8)) for a in y['m']])
    if 'L = 1e-200' in line:
        print(line[:330].rstrip())
print('--- the five-blocked tree in approved-crop')
for vp, d in (('1280x720', 'fin-sw-1280'), ('1920x1080', 'fin-sw-1920')):
    j = json.load(open(f'{base}/{d}/sweep-approved-crop-{vp}.json'))['recs']
    f = [(r['id'], r['n'], r['blocked'], r['ok'], r['hMin'], r['hMax']) for r in j if r['blocked'] == 5]
    print(' ', vp, f)
print('--- unfixed page: worst rows-to-dock gap and worst slack (queued state)')
j = json.load(open(f'{base}/fin2-prev-1280/sweep-queued4-1280x720.json'))['recs']
print('  min gap', min(r['gap'] for r in j if r['gap'] is not None), 'min slack', min(r['slack'] for r in j if r['slack'] is not None), 'heights', sorted({h for r in j for h in (r['hMin'], r['hMax']) if h}))
print('  per blocked-count failing:', {k: sum(1 for r in j if str(r['blocked']) == k and not r['ok']) for k in ('0', '1', '2', '3', '4')}, 'of', {k: sum(1 for r in j if str(r['blocked']) == k) for k in ('0', '1', '2', '3', '4')})
