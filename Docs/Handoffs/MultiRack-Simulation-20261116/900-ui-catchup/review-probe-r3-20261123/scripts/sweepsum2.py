import json
base = '/mnt/c/Users/hp/AppData/Local/Temp/review-r3/out'
def summarize(recs):
    n = len(recs)
    bad = [r for r in recs if not r['ok']]
    byb = {}
    for r in recs:
        k = str(r['blocked']); d = byb.setdefault(k, [0, 0]); d[0] += 1; d[1] += (0 if r['ok'] else 1)
    hs = sorted({h for r in recs for h in (r['hMin'], r['hMax']) if h is not None})
    sl = [r['slack'] for r in recs if r['slack'] is not None]
    gp = [r['gap'] for r in recs if r['gap'] is not None]
    wb = [r for r in recs if r['blocked'] > 0]
    reasons = {}
    for r in recs:
        for w in r['reasons']:
            reasons[w] = reasons.get(w, 0) + 1
    return dict(trees=n, fiveRows=sum(1 for r in recs if r['n'] == 5), shortLists=sum(1 for r in recs if 0 < r['n'] < 5),
                zeroLegit=sum(1 for r in recs if r.get('zeroLegit')), UNMEASURED=sum(1 for r in recs if r.get('unmeasured')), failing=len(bad),
                byBlocked={k: dict(trees=v[0], fail=v[1]) for k, v in sorted(byb.items())}, treesWithBlocked=len(wb), blockedRows=sum(r['blocked'] for r in wb),
                parityFail=sum(1 for r in recs if not r['parity']), reasonCut=sum(r['whyClipped'] for r in recs), metaTailCut=sum(r['metaClipped'] for r in recs),
                rowHeights=hs, minFreeRoomBelowDock=min(sl) if sl else None, minRowsToDockGap=min(gp) if gp else None, reasons=reasons)
for vp, d in (('1280x720', 'sw2-1280'), ('1920x1080', 'sw2-1920')):
    print('=' * 14, vp)
    for label in ('clean', 'queued4', 'approved-crop', 'after-execute', 'year1'):
        j = json.load(open(f'{base}/{d}/sweep-{label}-{vp}.json'))
        s = summarize(j['recs'])
        print(f"[{label}]", ' '.join(f"{k}={v}" for k, v in s.items()))
