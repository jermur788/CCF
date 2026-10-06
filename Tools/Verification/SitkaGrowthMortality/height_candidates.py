"""Offline height-candidate evaluation (Phase 7). Height does not feed CI, crowns
or adult light (light saturates at 8 m), so candidate heights can be applied to
the production DBH trajectories exactly. Planted trees (P*) take candidate
heights; regeneration recruits keep production heights (they never enter the
top-height set)."""
import csv, math, sys, json
src = sys.argv[1]; out = sys.argv[2]
T = {'I': (0.024, 1.247, 27.6), 'II': (0.025, 1.247, 23.3), 'III': (0.042, 1.563, 20.4), 'IV': (0.051, 2.134, 17.4), 'V': (0.060, 2.619, 16.0)}
def cr(cls):
    b2, b3, a30 = T[cls]; b0 = a30 / (1 - math.exp(-b2 * 30)) ** b3
    return lambda t: b0 * (1 - math.exp(-b2 * t)) ** b3
def mitscherlich(h0, a0, g, hmax):
    return lambda t: hmax - (hmax - h0) * math.exp(-g / hmax * (t - a0))
START_TOP = 14.63
# C: current architecture (dH/dt = g (1 - H/Hmax)) re-tuned so that top height
# goes from the authored start (14.63 m at 20) to the class III anchor at 30,
# with the class III anchor-corrected asymptote.
hmax3 = 20.4 / (1 - math.exp(-0.042 * 30)) ** 1.563
g3 = -math.log((hmax3 - 20.4) / (hmax3 - START_TOP)) * hmax3 / 10
CANDIDATES = {
    'production_current': None,
    'A_irish_CR_class_III': cr('III'),
    'A_irish_CR_class_II': cr('II'),
    'A_irish_CR_class_IV': cr('IV'),
    'C_retuned_mitscherlich_III': ('mit', g3, hmax3),
}
rows = list(csv.DictReader(open(src + '/tree_snapshots.csv')))
start = {r['tree_id']: float(r['height_m']) for r in rows if r['run'] == 'start'}
def cand_height(name, tid, age, hprod):
    c = CANDIDATES[name]
    if c is None or not tid.startswith('P') or tid not in start: return hprod
    h0 = start[tid]
    if isinstance(c, tuple):   # per-tree Mitscherlich from its own start height
        _, g, hmax = c
        return hmax - (hmax - h0) * math.exp(-g / hmax * (age - 20))
    return h0 * c(age) / c(20)  # proportional: keeps each tree's relative height
runs = sorted({r['run'] for r in rows if r['run'].startswith(('E_none/', 'A_suppression/', 'D_suppression_plus_cap/', 'C_self_thinning/'))})
result = []
for run in runs:
    for age in [20, 30, 40, 50, 60, 80, 100, 120]:
        trees = [r for r in rows if r['run'] == run and int(r['stand_age']) == age]
        if not trees: continue
        for name in CANDIDATES:
            hs = [(float(t['dbh_cm']), cand_height(name, t['tree_id'], age, float(t['height_m'])), t['tree_id']) for t in trees]
            hs.sort(key=lambda x: (-x[0], x[2]))
            top = hs[:16]
            topH = sum(h for _, h, _ in top) / len(top)
            meanH = sum(h for _, h, _ in hs) / len(hs)
            vol = sum(d * d * 0.00007854 * h * 0.5 for d, h, _ in hs) / 0.16
            hd = sorted(h * 100 / d for d, h, _ in hs)
            hdtop = sum(h * 100 / d for d, h, _ in top) / len(top)
            result.append(dict(run=run, age=age, candidate=name, top_height=round(topH, 2), mean_height=round(meanH, 2),
                               volume_m3_ha=round(vol, 1), hd_mean=round(sum(hd) / len(hd), 1), hd_p90=round(hd[int(0.9 * (len(hd) - 1))], 1), hd_top=round(hdtop, 1)))
with open(out, 'w', newline='') as f:
    w = csv.DictWriter(f, fieldnames=list(result[0].keys())); w.writeheader(); w.writerows(result)
print(json.dumps({'g3': round(g3, 4), 'hmax3': round(hmax3, 2)}))
