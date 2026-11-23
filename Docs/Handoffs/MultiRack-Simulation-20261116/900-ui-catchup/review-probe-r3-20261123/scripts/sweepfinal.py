import json, sys
base = '/mnt/c/Users/hp/AppData/Local/Temp/review-r3/out'
def summarize(recs):
    n = len(recs); bad = [r for r in recs if not r['ok']]
    byb = {}
    for r in recs:
        k = str(r['blocked']); d = byb.setdefault(k, [0, 0]); d[0] += 1; d[1] += (0 if r['ok'] else 1)
    hs = sorted({h for r in recs for h in (r['hMin'], r['hMax']) if h is not None})
    sl = [r['slack'] for r in recs if r['slack'] is not None]; gp = [r['gap'] for r in recs if r['gap'] is not None]
    wb = [r for r in recs if r['blocked'] > 0]; reasons = {}
    for r in recs:
        for w in r['reasons']: reasons[w] = reasons.get(w, 0) + 1
    return dict(trees=n, measured=n - sum(1 for r in recs if r.get('unmeasured')), fiveRows=sum(1 for r in recs if r['n'] == 5), shortLists=sum(1 for r in recs if 0 < r['n'] < 5),
                failing=len(bad), byBlocked={k: v for k, v in sorted(byb.items())}, treesWithBlocked=len(wb), blockedRows=sum(r['blocked'] for r in wb),
                parityFail=sum(1 for r in recs if not r['parity']), reasonCut=sum(r['whyClipped'] for r in recs), metaTailCut=sum(r['metaClipped'] for r in recs),
                rowHeights=hs, minFreeRoom=min(sl) if sl else None, minGap=min(gp) if gp else None, reasons=reasons)
rows = []
for vp, d in (('1280x720', 'fin-sw-1280'), ('1920x1080', 'fin-sw-1920')):
    for label in ('clean', 'queued4', 'approved-crop', 'after-execute', 'year1'):
        j = json.load(open(f'{base}/{d}/sweep-{label}-{vp}.json'))
        s = summarize(j['recs']); rows.append((vp, label, s))
        print(vp, f'[{label}]', ' '.join(f"{k}={v}" for k, v in s.items()))
print()
print('| state | viewport | trees measured | five-row / short | trees with blocked rows (rows) | blocked rows per tree (count: trees) | failing | row heights | min free room below dock | reasons |')
for vp, label, s in rows:
    bb = ', '.join(f"{k}: {v[0]}" for k, v in s['byBlocked'].items() if k != '0')
    print(f"| {label} | {vp} | {s['measured']}/{s['trees']} | {s['fiveRows']} / {s['shortLists']} | {s['treesWithBlocked']} ({s['blockedRows']}) | {bb or '-'} | {s['failing']} | {s['rowHeights']} | {s['minFreeRoom']} | {s['reasons']} |")
# compare to the earlier sw2 runs
print()
for vp, d1, d2 in (('1280x720', 'fin-sw-1280', 'sw2-1280'), ('1920x1080', 'fin-sw-1920', 'sw2-1920')):
    for label in ('clean', 'queued4', 'approved-crop', 'after-execute', 'year1'):
        a = json.load(open(f'{base}/{d1}/sweep-{label}-{vp}.json'))['recs']; b = json.load(open(f'{base}/{d2}/sweep-{label}-{vp}.json'))['recs']
        ka = {r['id']: (r['n'], r['blocked'], r['ok'], r['hMin'], r['hMax']) for r in a}; kb = {r['id']: (r['n'], r['blocked'], r['ok'], r['hMin'], r['hMax']) for r in b}
        print(vp, label, 'per-tree records identical between the two final sweeps:', ka == kb, f'({len(ka)} trees)')
