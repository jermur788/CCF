import json, re
base = '/mnt/c/Users/hp/AppData/Local/Temp/review-r3/out'
tot = 0
for vp, d in (('1280x720', 'fin-sw-1280'), ('1920x1080', 'fin-sw-1920')):
    n = 0
    for label in ('clean', 'queued4', 'approved-crop', 'after-execute', 'year1'):
        j = json.load(open(f'{base}/{d}/sweep-{label}-{vp}.json')); n += len(j['recs'])
        if label == 'approved-crop':
            five = [r['id'] for r in j['recs'] if r['blocked'] == 5]
            print(vp, 'trees with 5 blocked rows in approved-crop:', five, 'ok:', [r['ok'] for r in j['recs'] if r['blocked'] == 5], 'rowH', [(r['hMin'], r['hMax']) for r in j['recs'] if r['blocked'] == 5])
    print(vp, 'tree measurements over 5 states:', n)
# 1e-200 text
for tag in ('fin2-ft-1280', 'fin2-ft-1920'):
    for line in open(f'{base}/{tag}/run.log', encoding='utf-8', errors='replace'):
        if 'L = 1e-200' in line and line.startswith(('PASS', 'INFO', 'FAIL')):
            print(tag, line[:420].rstrip())
# an alert after a plain (partial/guard) refusal? read the saved results JSON lines for the partial card: alertShown is not logged, so state what is logged
for line in open(f'{base}/fin2-ft-1280/run.log', encoding='utf-8', errors='replace'):
    if line.startswith('INFO') and ('PARTIAL Add provoked' in line or 'GUARD refusal' in line or 'UNCONFIRMED' in line):
        print('logged keys:', sorted(json.loads(line.split(' :: ', 1)[1]).keys()))
