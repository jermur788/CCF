"""CCF #899 independent offline crown-model comparison (Claude, 2026-11-23).

Read-only scratch analysis. Not part of the CCF repository; touches no game code, fixtures or saves.
Pure Python (no numpy available on this machine). Deterministic: the only RNG is a seeded random.Random
used for the synthetic Frenchpark-like benchmark stand.

Stands:
  MA-A, MA-B : regenerated from MultiAreaGenerator.cs (generator v1, seed 8,850,001) - integer port.
  C30        : Compact30Roster.json (integration/sen1-873 @ 31fbe46e), 175 Sitka rows.
  FP-*       : synthetic stand shaped like the Frenchpark (Co. Roscommon) Teagasc thinning trial.

Candidates:
  C0  current      r0 = rpot,                         target = rpot / (1 + CI/5)
  C1               r0 = target,                       target = rpot / (1 + CI/5)
  A   (C2)         r0 = target,                       target = rpot / sqrt(1 + CI/5)
  C3a              r0 = target,                       target = rpot / (1 + CI/5)^0.65
  B   directional  8 sectors, C[i,k] = sum max(0,cos dtheta) (Dj/Di)^p exp(-d/L);
                   target_k = rpot exp(-beta C_k); R_k += min(vMax, gamma max(0, target_k - R_k)) (no contraction);
                   r_eq = sqrt(mean R_k^2); Year-0 R_k = target_k. p, L, beta, gamma, vMax are UNFITTED in the
                   proposal; here p = 1, L = 2 m, gamma = 0.15, vMax = none, beta fitted (see FP benchmark).
"""
import json, math, random, sys, hashlib, time

M64 = (1 << 64) - 1
GOLD = 0x9E3779B97F4A7C15
OUT = {}

# ------------------------------------------------------------------ MA-1 generator port

def splitmix(z):
    z = (z + GOLD) & M64
    z = ((z ^ (z >> 30)) * 0xBF58476D1CE4E5B9) & M64
    z = ((z ^ (z >> 27)) * 0x94D049BB133111EB) & M64
    return z ^ (z >> 31)

GEN_SEED = 8_850_001

def nxt(ai, cell, salt):
    z = (GEN_SEED * GOLD) & M64
    z = splitmix(z ^ (ai & 0xFFFFFFFF))
    z = splitmix(z ^ (((cell & 0xFFFFFFFF) << 16) & M64))
    return splitmix(z ^ (((salt & 0xFFFFFFFF) << 40) & M64))

def tdiv(a, b):  # C# long division (truncate toward zero)
    q = abs(a) // abs(b)
    return q if (a >= 0) == (b > 0) else -q

PLANS = [
    dict(area="MA-A", poly=[(2000, 2000), (36000, 2000), (40000, 18000), (24000, 32000), (3000, 26000)],
         pitch=2240, jitter=450, margin=700,
         cohorts=[(100, 24, 222, 75, 90, 360, 17000, 25)]),
    dict(area="MA-B", poly=[(36000, 2000), (90000, 0), (86000, 44000), (40000, 18000)],
         pitch=4300, jitter=900, margin=900,
         cohorts=[(70, 47, 345, 80, 220, 480, 27200, 20), (30, 29, 225, 60, 120, 330, 19900, 25)]),
]

def cross(a, b, p):
    return (b[0] - a[0]) * (p[1] - a[1]) - (b[1] - a[1]) * (p[0] - a[0])

def inside_margin(poly, p, m):
    n = len(poly)
    for i in range(n):
        a, b = poly[i], poly[(i + 1) % n]
        c = cross(a, b, p)
        if c <= 0:
            return False
        dx, dy = b[0] - a[0], b[1] - a[1]
        if c * c < m * m * (dx * dx + dy * dy):
            return False
    return True

def gen_area(plan, ai):
    poly = plan["poly"]
    minx = min(p[0] for p in poly); maxx = max(p[0] for p in poly)
    miny = min(p[1] for p in poly); maxy = max(p[1] for p in poly)
    P, J = plan["pitch"], plan["jitter"]
    rows = (maxy - miny) // P + 1
    cols = (maxx - minx) // P + 1
    trees = []
    for row in range(rows):
        for col in range(cols):
            cell = row * 1000 + col
            x = minx + P // 2 + col * P + (nxt(ai, cell, 1) % (2 * J + 1)) - J
            y = miny + P // 2 + row * P + (nxt(ai, cell, 2) % (2 * J + 1)) - J
            if not inside_margin(poly, (x, y), plan["margin"]):
                continue
            pct = nxt(ai, cell, 3) % 100
            cum = 0
            coh = plan["cohorts"][-1]
            for c in plan["cohorts"]:
                cum += c[0]
                if pct < cum:
                    coh = c
                    break
            s = sum(nxt(ai, cell, 10 + k) % 1001 for k in range(4))
            dbh = coh[2] + tdiv((s - 2000) * coh[3], 1000)
            dbh = max(coh[4], min(coh[5], dbh))
            tid = "%s-%03d" % (plan["area"], len(trees) + 1)
            trees.append(dict(id=tid, x=x / 1000.0, z=y / 1000.0, dbh=dbh / 10.0, age=coh[1], row=row))
    return trees, [(p[0] / 1000.0, p[1] / 1000.0) for p in poly]

def poly_area(poly):
    s = 0.0
    for i in range(len(poly)):
        a, b = poly[i], poly[(i + 1) % len(poly)]
        s += a[0] * b[1] - b[0] * a[1]
    return abs(s) / 2.0

# ------------------------------------------------------------------ biology

def rpot(dbh):
    return max(0.1, 0.9415 + 0.07635 * dbh)

def neighbours(trees, cutoff=8.0, period=None):
    """Per tree list of (j, d, bearing). Optional periodic (torus) domain for the benchmark stand."""
    n = len(trees)
    out = [[] for _ in range(n)]
    for i in range(n):
        xi, zi = trees[i]["x"], trees[i]["z"]
        for j in range(i + 1, n):
            dx = trees[j]["x"] - xi
            dz = trees[j]["z"] - zi
            if period:
                dx -= period * round(dx / period)
                dz -= period * round(dz / period)
            if abs(dx) > cutoff or abs(dz) > cutoff:
                continue
            d = math.hypot(dx, dz)
            if d > cutoff:
                continue
            out[i].append((j, d, math.atan2(dz, dx)))
            out[j].append((i, d, math.atan2(-dz, -dx)))
    return out

def hegyi(trees, nb):
    ci = []
    for i, t in enumerate(trees):
        s = 0.0
        for j, d, _ in nb[i]:
            s += (trees[j]["dbh"] / max(1.0, t["dbh"])) / max(0.5, d)
        ci.append(s)
    return ci

def directional(trees, nb, S, p, L):
    out = []
    for i, t in enumerate(trees):
        C = [0.0] * S
        for j, d, th in nb[i]:
            w0 = (trees[j]["dbh"] / t["dbh"]) ** p * math.exp(-d / L)
            for k in range(S):
                c = math.cos(th - 2 * math.pi * k / S)
                if c > 0:
                    C[k] += c * w0
        out.append(C)
    return out

SCALAR = {
    "C0": lambda rp, ci: rp / (1 + ci / 5),
    "C1": lambda rp, ci: rp / (1 + ci / 5),
    "A": lambda rp, ci: rp / math.sqrt(1 + ci / 5),
    "C3a": lambda rp, ci: rp / (1 + ci / 5) ** 0.65,
}

class Model:
    def __init__(self, name, B=None):
        self.name = name
        self.B = B  # dict(S, p, L, beta, gamma, vmax)

    def init(self, trees, nb):
        if self.B:
            C = directional(trees, nb, self.B["S"], self.B["p"], self.B["L"])
            for t, Ck in zip(trees, C):
                rp = rpot(t["dbh"])
                t["R"] = [max(0.1, rp * math.exp(-self.B["beta"] * c)) for c in Ck]
                t["r"] = req(t["R"])
        else:
            ci = hegyi(trees, nb)
            for t, c in zip(trees, ci):
                t["r"] = rpot(t["dbh"]) if self.name == "C0" else max(0.1, SCALAR[self.name](rpot(t["dbh"]), c))

    def step(self, trees, nb, dbh_inc=None):
        """One annual step. Competition from pre-growth state (as AnnualRules.Grow), potential from new DBH."""
        ci = hegyi(trees, nb)
        C = directional(trees, nb, self.B["S"], self.B["p"], self.B["L"]) if self.B else None
        for i, t in enumerate(trees):
            if dbh_inc is None:
                resp = 1.0 / (1.0 + ci[i] / 5.0)
                t["dbh"] = min(200.0, max(1.0, t["dbh"] + 1.2 * 1.0 * min(1.0, max(0.0, 1 - t["dbh"] / 120.0)) * resp))
            else:
                t["dbh"] += dbh_inc
            rp = rpot(t["dbh"])
            if self.B:
                b = self.B
                R = t["R"]
                for k in range(b["S"]):
                    tgt = max(0.1, rp * math.exp(-b["beta"] * C[i][k]))
                    g = b["gamma"] * max(0.0, tgt - R[k])
                    if b["vmax"] is not None:
                        g = min(b["vmax"], g)
                    R[k] += g
                t["r"] = req(R)
            else:
                tgt = max(0.1, SCALAR[self.name](rp, ci[i]))
                t["r"] = max(0.1, t["r"] + (tgt - t["r"]) * 0.15)

def req(R):
    return math.sqrt(sum(x * x for x in R) / len(R))

# ------------------------------------------------------------------ raster geometry

H = 0.125  # m per cell

def point_in_poly(poly, x, y):
    inside = False
    n = len(poly)
    for i in range(n):
        x1, y1 = poly[i]; x2, y2 = poly[(i + 1) % n]
        if (y1 > y) != (y2 > y):
            xi = x1 + (y - y1) * (x2 - x1) / (y2 - y1)
            if x < xi:
                inside = not inside
    return inside

class Raster:
    def __init__(self, poly):
        self.poly = poly
        self.x0 = min(p[0] for p in poly) - 2 * H
        self.y0 = min(p[1] for p in poly) - 2 * H
        self.W = int(math.ceil((max(p[0] for p in poly) + 2 * H - self.x0) / H))
        self.Hn = int(math.ceil((max(p[1] for p in poly) + 2 * H - self.y0) / H))
        self.mask = []
        for j in range(self.Hn):
            y = self.y0 + (j + 0.5) * H
            m = 0
            for i in range(self.W):
                if point_in_poly(poly, self.x0 + (i + 0.5) * H, y):
                    m |= 1 << i
            self.mask.append(m)
        self.inside = sum(m.bit_count() for m in self.mask)
        self.area = poly_area(poly)

    def cover_circles(self, trees):
        rows = [0] * self.Hn
        for t in trees:
            r = t["r"]
            cx = (t["x"] - self.x0) / H - 0.5
            cy = (t["z"] - self.y0) / H - 0.5
            rc = r / H
            j0 = max(0, int(math.ceil(cy - rc))); j1 = min(self.Hn - 1, int(math.floor(cy + rc)))
            for j in range(j0, j1 + 1):
                dy = j - cy
                hw = rc * rc - dy * dy
                if hw < 0:
                    continue
                hw = math.sqrt(hw)
                i0 = max(0, int(math.ceil(cx - hw))); i1 = min(self.W - 1, int(math.floor(cx + hw)))
                if i1 >= i0:
                    rows[j] |= ((1 << (i1 - i0 + 1)) - 1) << i0
        return rows

    def cover_sectors(self, trees, S):
        rows = [0] * self.Hn
        half = math.pi / S
        for t in trees:
            R = t["R"]
            rmax = max(R)
            cx = (t["x"] - self.x0) / H - 0.5
            cy = (t["z"] - self.y0) / H - 0.5
            rc = rmax / H
            j0 = max(0, int(math.ceil(cy - rc))); j1 = min(self.Hn - 1, int(math.floor(cy + rc)))
            i0 = max(0, int(math.ceil(cx - rc))); i1 = min(self.W - 1, int(math.floor(cx + rc)))
            for j in range(j0, j1 + 1):
                dy = j - cy
                bits = 0
                for i in range(i0, i1 + 1):
                    dx = i - cx
                    d2 = dx * dx + dy * dy
                    if d2 > rc * rc:
                        continue
                    k = int(round(math.atan2(dy, dx) / (2 * half))) % S
                    if d2 * H * H <= R[k] * R[k]:
                        bits |= 1 << i
                rows[j] |= bits
        return rows

    def metrics(self, rows, gaps=True):
        cov = sum((c & m).bit_count() for c, m in zip(rows, self.mask))
        res = dict(cover=cov / self.inside, bare_m2=(self.inside - cov) * H * H)
        if not gaps:
            return res
        W, Hn = self.W, self.Hn
        g = []
        for c, m in zip(rows, self.mask):
            v = m & ~c
            s = bin(v)[2:].zfill(W)[::-1]
            g.append(bytearray(1 if ch == "1" else 0 for ch in s[:W]))
        # largest 4-connected gap component
        seen = [bytearray(W) for _ in range(Hn)]
        best = 0
        for j in range(Hn):
            gj = g[j]
            for i in range(W):
                if gj[i] and not seen[j][i]:
                    stack = [(j, i)]; seen[j][i] = 1; n = 0
                    while stack:
                        a, b = stack.pop(); n += 1
                        for a2, b2 in ((a + 1, b), (a - 1, b), (a, b + 1), (a, b - 1)):
                            if 0 <= a2 < Hn and 0 <= b2 < W and g[a2][b2] and not seen[a2][b2]:
                                seen[a2][b2] = 1; stack.append((a2, b2))
                    best = max(best, n)
        res["largest_gap_m2"] = best * H * H
        # largest inscribed circle in gaps (squared EDT, Felzenszwalb-Huttenlocher)
        BIG = 1e12
        f = [[BIG if g[j][i] else 0.0 for i in range(W)] for j in range(Hn)]
        for i in range(W):
            col = edt1d([f[j][i] for j in range(Hn)])
            for j in range(Hn):
                f[j][i] = col[j]
        mx = 0.0
        for j in range(Hn):
            row = edt1d(f[j])
            m = max(row)
            if m > mx:
                mx = m
        res["gap_width_m"] = 2 * math.sqrt(mx) * H if mx < BIG else float("nan")
        return res

def edt1d(f):
    n = len(f)
    v = [0] * n; z = [0.0] * (n + 1); d = [0.0] * n
    k = 0; z[0] = -1e30; z[1] = 1e30
    for q in range(1, n):
        s = ((f[q] + q * q) - (f[v[k]] + v[k] * v[k])) / (2 * q - 2 * v[k])
        while s <= z[k]:
            k -= 1
            s = ((f[q] + q * q) - (f[v[k]] + v[k] * v[k])) / (2 * q - 2 * v[k])
        k += 1; v[k] = q; z[k] = s; z[k + 1] = 1e30
    k = 0
    for q in range(n):
        while z[k + 1] < q:
            k += 1
        d[q] = (q - v[k]) ** 2 + f[v[k]]
    return d

# ------------------------------------------------------------------ helpers

def clone(trees):
    out = []
    for t in trees:
        c = dict(t)
        if "R" in t:
            c["R"] = list(t["R"])
        out.append(c)
    return out

def stand_stats(trees, area):
    n = len(trees)
    ba = sum(math.pi * (t["dbh"] / 200.0) ** 2 for t in trees) / (area / 10000.0)
    dq = math.sqrt(sum(t["dbh"] ** 2 for t in trees) / n)
    sph = n / (area / 10000.0)
    rd = sph * (dq / 25.0) ** 2.063 / 1868.0
    return dict(n=n, sph=round(sph), ba=round(ba, 1), dq=round(dq, 2), relative_density=round(rd, 3))

def q(vals, p):
    s = sorted(vals)
    if not s:
        return float("nan")
    k = (len(s) - 1) * p
    lo = int(math.floor(k)); hi = int(math.ceil(k))
    return s[lo] + (s[hi] - s[lo]) * (k - lo)

def rsum(trees, area):
    return sum(math.pi * t["r"] ** 2 for t in trees) / area

def dist_to_poly_edge(poly, x, y):
    best = 1e9
    for i in range(len(poly)):
        (x1, y1), (x2, y2) = poly[i], poly[(i + 1) % len(poly)]
        dx, dy = x2 - x1, y2 - y1
        t = max(0.0, min(1.0, ((x - x1) * dx + (y - y1) * dy) / (dx * dx + dy * dy)))
        best = min(best, math.hypot(x - x1 - t * dx, y - y1 - t * dy))
    return best

# ------------------------------------------------------------------ Frenchpark-like benchmark

# Fernandez (2017) UCD MSc, Frenchpark Co. Roscommon, Sitka planted 1995 at 2 x 2 m, YC 24.
# First thinning 2010 (age 15), Table 2 (stems/ha, mean DBH cm after first thinning):
FP_T2 = {"unthinned": (2133, 16.2), "light": (1500, 17.2), "medium": (1230, 17.7), "heavy": (1015, 18.5)}
# Measured 5 seasons later on mean-DBH trees (4 per plot x 3 plots, 2 next to drains + 2 away). PRIMARY reading:
# thesis Table 12 (p.36) per-group means, drain/no-drain averaged: DBH cm and maximum crown width m of the felled crown.
FP_OBS = {"unthinned": ((19.12 + 19.78) / 2, (2.44 + 2.68) / 2), "light": ((20.78 + 20.88) / 2, (2.55 + 3.24) / 2),
          "medium": ((21.55 + 22.85) / 2, (3.16 + 3.24) / 2), "heavy": ((22.02 + 22.17) / 2, (3.13 + 3.39) / 2)}
# SENSITIVITY reading: thesis text p.16 sample DBH and Fig. 11 rounded crown widths (heavy DBH 23.5 conflicts with Table 12).
FP_OBS_TEXT = {"unthinned": (19.0, 2.6), "light": (20.5, 2.9), "medium": (22.0, 3.2), "heavy": (23.5, 3.3)}

def fp_stand(seed=899, side=60.0, sd=4.0):
    rng = random.Random(seed)
    trees = []
    n = int(side / 2.0)
    for row in range(n):
        for col in range(n):
            trees.append(dict(id="FP-%02d-%02d" % (row, col), row=row,
                              x=1.0 + 2.0 * col + rng.uniform(-0.25, 0.25),
                              z=1.0 + 2.0 * row + rng.uniform(-0.25, 0.25),
                              dbh=max(5.0, min(30.0, rng.gauss(16.2, sd)))))
    keep = int(round(2133 * side * side / 10000.0))
    rng.shuffle(trees)
    trees = sorted(trees[:keep], key=lambda t: t["id"])
    # rescale so the stand mean DBH is exactly 16.2 cm at age 15 (Table 2 unthinned)
    m = sum(t["dbh"] for t in trees) / len(trees)
    for t in trees:
        t["dbh"] += 16.2 - m
    return trees

def fp_thin(trees, target_sph, side):
    if target_sph >= 2133:
        return trees
    keep_n = int(round(target_sph * side * side / 10000.0))
    t2 = [t for t in trees if t["row"] % 7 != 3]  # extraction rack: 1 line in 7
    t2.sort(key=lambda t: t["dbh"])           # low selection between racks
    return sorted(t2[len(t2) - keep_n:], key=lambda t: t["id"]) if len(t2) > keep_n else t2

_FP_CACHE = {}

def fp_run(model, side=60.0, sd=4.0, seed=899, obs=None):
    obs = obs or FP_OBS
    res = {}
    key = (seed, side, sd)
    if key not in _FP_CACHE:
        base = fp_stand(seed, side, sd)
        nbs = {}
        for trt, (sph, _) in FP_T2.items():
            kept = fp_thin(clone(base), sph, side)
            nbs[trt] = ([t["id"] for t in kept], neighbours(kept, period=side))
        _FP_CACHE[key] = (base, neighbours(base, period=side), nbs)
    base, nb0, nbs = _FP_CACHE[key]
    pre = clone(base)
    model.init(pre, nb0)
    for trt, (sph, mean_dbh) in FP_T2.items():
        ids, nb = nbs[trt]
        byid = {t["id"]: t for t in pre}
        trees = clone([byid[i] for i in ids])
        m = sum(t["dbh"] for t in trees) / len(trees)
        obs_dbh, obs_w = obs[trt]
        inc = (obs_dbh - m) / 5.0  # force the observed mean-tree DBH at the measurement (crown model test only)
        for _ in range(5):
            model.step(trees, nb, dbh_inc=inc)
        mean_now = sum(t["dbh"] for t in trees) / len(trees)
        sample = [t for t in trees if abs(t["dbh"] - mean_now) <= 1.5]
        r = sum(t["r"] for t in sample) / len(sample)
        rp = sum(rpot(t["dbh"]) for t in sample) / len(sample)
        res[trt] = dict(n_stand=len(trees), n_sample=len(sample), post_thin_mean_dbh=round(m, 2),
                        pred_r=round(r, 3), pred_ratio=round(r / rp, 3),
                        obs_r_upper=round(obs_w / 2, 3), obs_ratio=round(obs_w / 2 / rpot(obs_dbh), 3))
    return res

def fit_beta(target_ratio, S=8, p=1.0, L=2.0, side=60.0, obs=None):
    lo, hi = 0.0, 2.0
    for _ in range(18):
        mid = (lo + hi) / 2
        m = Model("B", dict(S=S, p=p, L=L, beta=mid, gamma=0.15, vmax=None))
        r = fp_run(m, side, obs=obs)["unthinned"]["pred_ratio"]
        if r > target_ratio:
            lo = mid
        else:
            hi = mid
    return (lo + hi) / 2

# ------------------------------------------------------------------ main experiments

def main():
    t_start = time.time()
    # ---- stands
    stands = {}
    for ai, plan in enumerate(PLANS):
        trees, poly = gen_area(plan, ai)
        stands[plan["area"]] = (trees, poly)
    roster = json.load(open(sys.argv[1]))
    c30 = [dict(id=r["id"], x=r["x_m"], z=r["z_m"], dbh=r["dbh_cm"], age=r["age_y"], crown_file=r["crown_m"])
           for r in roster["rows"]]
    stands["C30"] = (c30, [(-15.0, -15.0), (15.0, -15.0), (15.0, 15.0), (-15.0, 15.0)])
    OUT["stands"] = {}
    for k, (trees, poly) in stands.items():
        a = poly_area(poly)
        st = stand_stats(trees, a)
        st["area_m2"] = round(a, 1)
        nb = neighbours(trees)
        ci = hegyi(trees, nb)
        st["ci_mean"] = round(sum(ci) / len(ci), 3)
        st["ci_q10_q50_q90"] = [round(q(ci, x), 2) for x in (0.1, 0.5, 0.9)]
        edge = [dist_to_poly_edge(poly, t["x"], t["z"]) < 8.0 for t in trees]
        st["share_within_8m_of_edge"] = round(sum(edge) / len(trees), 3)
        OUT["stands"][k] = st
    # Compact30 file crowns equal open-grown potential?
    OUT["c30_file_crown_equals_rpot_maxabs"] = round(max(abs(t["crown_file"] - rpot(t["dbh"])) for t in c30), 4)
    print("stands", json.dumps(OUT["stands"]), flush=True)

    # ---- Frenchpark benchmark: scalar candidates (no fitting) and B (beta fitted to unthinned)
    fp = {}
    for name in ("C0", "C1", "A", "C3a"):
        fp[name] = fp_run(Model(name))
    obs_unthinned_ratio = fp["A"]["unthinned"]["obs_ratio"]
    beta8 = fit_beta(obs_unthinned_ratio)
    beta8_L3 = fit_beta(obs_unthinned_ratio, L=3.0)
    beta16 = fit_beta(obs_unthinned_ratio, S=16)
    # B matched to A's unthinned prediction instead (same-size control, isolates the directional structure)
    beta8_matchA = fit_beta(fp["A"]["unthinned"]["pred_ratio"])
    BPAR = dict(S=8, p=1.0, L=2.0, beta=beta8, gamma=0.15, vmax=None)
    BVAR = {
        "B": BPAR,
        "B_L3": dict(BPAR, L=3.0, beta=beta8_L3),
        "B16": dict(BPAR, S=16, beta=beta16),
        "B_matchA": dict(BPAR, beta=beta8_matchA),
    }
    for name, par in BVAR.items():
        fp[name] = fp_run(Model("B", par))
    # DBH-spread sensitivity for A and B
    fp["A_sd3"] = fp_run(Model("A"), sd=3.0)
    fp["A_sd5"] = fp_run(Model("A"), sd=5.0)
    fp["B_sd3"] = fp_run(Model("B", BPAR), sd=3.0)
    fp["B_sd5"] = fp_run(Model("B", BPAR), sd=5.0)
    OUT["frenchpark"] = fp
    # sensitivity: thesis-text reading of sample DBH / rounded widths; B refitted to that reading's unthinned ratio
    fpt = {name: fp_run(Model(name), obs=FP_OBS_TEXT) for name in ("C1", "A", "C3a")}
    beta_text = fit_beta(fpt["A"]["unthinned"]["obs_ratio"], obs=FP_OBS_TEXT)
    fpt["B"] = fp_run(Model("B", dict(BPAR, beta=beta_text)), obs=FP_OBS_TEXT)
    fpt["B_beta"] = round(beta_text, 4)
    OUT["frenchpark_text_reading"] = fpt
    OUT["B_params"] = {k: {kk: (round(vv, 4) if isinstance(vv, float) else vv) for kk, vv in v.items()} for k, v in BVAR.items()}
    print("frenchpark done", round(time.time() - t_start), "s", flush=True)
    print(json.dumps(fp, indent=0)[:3000], flush=True)

    # ---- Year-0 table on the game stands
    rasters = {k: Raster(poly) for k, (trees, poly) in stands.items()}
    y0 = {}
    models_y0 = ["C0", "C1", "A", "C3a", "B", "B_matchA", "B_L3", "B16"]
    for sk, (trees, poly) in stands.items():
        nb = neighbours(trees)
        y0[sk] = {}
        for mn in models_y0:
            m = Model("B", BVAR[mn]) if mn.startswith("B") else Model(mn)
            tr = clone(trees)
            m.init(tr, nb)
            rs = [t["r"] for t in tr]
            row = dict(r_mean=round(sum(rs) / len(rs), 3), r_q10=round(q(rs, 0.1), 2), r_q50=round(q(rs, 0.5), 2),
                       r_q90=round(q(rs, 0.9), 2), sum_disk_over_area=round(rsum(tr, rasters[sk].area), 3))
            row.update({k2: round(v, 4) for k2, v in rasters[sk].metrics(rasters[sk].cover_circles(tr)).items()})
            if mn.startswith("B"):
                sec = rasters[sk].metrics(rasters[sk].cover_sectors(tr, BVAR[mn]["S"]))
                row["sector_geometry"] = {k2: round(v, 4) for k2, v in sec.items()}
            edge = [dist_to_poly_edge(poly, t["x"], t["z"]) < 8.0 for t in tr]
            e = [t["r"] for t, f in zip(tr, edge) if f]; ii = [t["r"] for t, f in zip(tr, edge) if not f]
            row["r_edge_mean"] = round(sum(e) / len(e), 3) if e else None
            row["r_interior_mean"] = round(sum(ii) / len(ii), 3) if ii else None
            y0[sk][mn] = row
        print("y0", sk, round(time.time() - t_start), "s", flush=True)
    OUT["year0"] = y0

    # ---- Fell scenarios
    def nearest_to(trees, px, pz, n):
        return [t["id"] for t in sorted(trees, key=lambda t: ((t["x"] - px) ** 2 + (t["z"] - pz) ** 2, t["id"]))[:n]]

    A_trees, A_poly = stands["MA-A"]
    B_trees, B_poly = stands["MA-B"]
    cxA = sum(p[0] for p in A_poly) / len(A_poly); czA = sum(p[1] for p in A_poly) / len(A_poly)
    cxB = sum(p[0] for p in B_poly) / len(B_poly); czB = sum(p[1] for p in B_poly) / len(B_poly)
    # A-T20: one extraction rack (generator row 6) + low selection until >= 20% of basal area is removed
    baA = sum(t["dbh"] ** 2 for t in A_trees)
    t20 = [t["id"] for t in A_trees if t["row"] == 6]
    rem = sum(t["dbh"] ** 2 for t in A_trees if t["id"] in t20)
    for t in sorted([t for t in A_trees if t["id"] not in t20], key=lambda t: (t["dbh"], t["id"])):
        if rem >= 0.2 * baA:
            break
        t20.append(t["id"]); rem += t["dbh"] ** 2
    c30_trees = stands["C30"][0]
    scen = {
        "A-control": ("MA-A", []),
        "A-G10": ("MA-A", nearest_to(A_trees, cxA, czA, 10)),
        "A-D10": ("MA-A", [t["id"] for i, t in enumerate(A_trees) if i % 17 == 8][:10]),
        "A-T20": ("MA-A", sorted(t20)),
        "B-control": ("MA-B", []),
        "B-G10": ("MA-B", nearest_to(B_trees, cxB, czB, 10)),
        "C30-control": ("C30", []),
        "C30-G10": ("C30", nearest_to(c30_trees, 6.0, 6.0, 10)),
    }
    OUT["fell_ids"] = {k: v[1] for k, v in scen.items()}
    OUT["fell_ba_share"] = {}
    for k, (sk, ids) in scen.items():
        tr = stands[sk][0]
        OUT["fell_ba_share"][k] = round(sum(t["dbh"] ** 2 for t in tr if t["id"] in ids) / sum(t["dbh"] ** 2 for t in tr), 3)
    YEARS = [1, 3, 10]
    traj = {}
    radius_tracks = {}
    for mn in ["C0", "A", "B", "B_matchA"]:
        traj[mn] = {}
        radius_tracks[mn] = {}
        for sc, (sk, ids) in scen.items():
            trees0, poly = stands[sk]
            ras = rasters[sk]
            m = Model("B", BVAR[mn]) if mn.startswith("B") else Model(mn)
            tr = clone(trees0)
            m.init(tr, neighbours(tr))
            tr = [t for t in tr if t["id"] not in ids]  # fell at Year 0 after genesis
            nb = neighbours(tr)
            rec = {}
            def snap(label):
                r = {k2: round(v, 4) for k2, v in ras.metrics(ras.cover_circles(tr)).items()}
                r["r_mean"] = round(sum(t["r"] for t in tr) / len(tr), 3)
                r["sum_disk_over_area"] = round(rsum(tr, ras.area), 3)
                if mn.startswith("B"):
                    r["sector_geometry"] = {k2: round(v, 4) for k2, v in ras.metrics(ras.cover_sectors(tr, BVAR[mn]["S"])).items()}
                rec[label] = r
                radius_tracks[mn].setdefault(sc, {})[label] = {t["id"]: t["r"] for t in tr}
                if mn.startswith("B"):
                    radius_tracks[mn][sc][label + "_R"] = {t["id"]: list(t["R"]) for t in tr}
            snap("Y0post")
            y = 0
            for target in YEARS:
                while y < target:
                    m.step(tr, nb); y += 1
                snap("Y%d" % target)
            traj[mn][sc] = rec
            print("traj", mn, sc, round(time.time() - t_start), "s", flush=True)
    OUT["trajectories"] = traj

    # gap-edge neighbours vs same trees in control
    edge_resp = {}
    for mn in radius_tracks:
        edge_resp[mn] = {}
        for sc, (sk, ids) in scen.items():
            if not ids:
                continue
            ctrl = sk.replace("MA-", "")
            ctrl = {"MA-A": "A-control", "MA-B": "B-control", "C30": "C30-control"}[sk]
            trees0 = stands[sk][0]
            felled = [t for t in trees0 if t["id"] in ids]
            adj = [t["id"] for t in trees0 if t["id"] not in ids and
                   min(math.hypot(t["x"] - f["x"], t["z"] - f["z"]) for f in felled) <= 3.5]
            rt, rc = radius_tracks[mn][sc], radius_tracks[mn][ctrl]
            row = dict(n_adjacent=len(adj))
            for lab in ("Y1", "Y3", "Y10"):
                d_f = sum(rt[lab][i] - rt["Y0post"][i] for i in adj) / len(adj)
                d_c = sum(rc[lab][i] - rc["Y0post"][i] for i in adj) / len(adj)
                row[lab] = dict(dr_fell=round(d_f, 3), dr_control=round(d_c, 3), release=round(d_f - d_c, 3))
            if mn.startswith("B"):
                # directional asymmetry: growth of the sector facing the nearest felled stem vs the opposite sector
                S = BVAR[mn]["S"]
                face, opp = [], []
                for i in adj:
                    t = next(tt for tt in trees0 if tt["id"] == i)
                    f = min(felled, key=lambda f: math.hypot(t["x"] - f["x"], t["z"] - f["z"]))
                    k = int(round(math.atan2(f["z"] - t["z"], f["x"] - t["x"]) / (2 * math.pi / S))) % S
                    R0, R3 = rt["Y0post_R"][i], rt["Y3_R"][i]
                    face.append(R3[k] - R0[k]); opp.append(R3[(k + S // 2) % S] - R0[(k + S // 2) % S])
                row["Y3_gap_facing_sector_dr"] = round(sum(face) / len(face), 3)
                row["Y3_opposite_sector_dr"] = round(sum(opp) / len(opp), 3)
            edge_resp[mn][sc] = row
    OUT["gap_edge_response"] = edge_resp

    # untouched 10-year drift (does B ratchet?) and annual max expansion
    drift = {}
    for mn in ("A", "B"):
        drift[mn] = {}
        for sk in ("MA-A", "MA-B", "C30"):
            sc = {"MA-A": "A-control", "MA-B": "B-control", "C30": "C30-control"}[sk]
            tr0 = radius_tracks[mn][sc]["Y0post"]; tr10 = radius_tracks[mn][sc]["Y10"]
            shrink = sum(1 for i in tr0 if tr10[i] < tr0[i] - 1e-6)
            drift[mn][sk] = dict(r_mean_Y0=round(sum(tr0.values()) / len(tr0), 3),
                                 r_mean_Y10=round(sum(tr10.values()) / len(tr10), 3),
                                 trees_shrinking=shrink, n=len(tr0))
    OUT["untouched_drift"] = drift

    # density sweep: equal 15 cm DBH on a periodic square grid (Year-0 equilibrium radius / spacing)
    sweep = []
    for s in (0.8, 1.0, 1.5, 2.0, 2.24, 3.0, 4.0, 6.0, 8.0):
        n = max(6, int(round(24 / s))); side = n * s
        grid = [dict(id="%d-%d" % (i, j), x=(i + 0.5) * s, z=(j + 0.5) * s, dbh=15.0) for i in range(n) for j in range(n)]
        nbg = neighbours(grid, period=side)
        ci0 = hegyi(grid, nbg)[0]
        C0k = directional(grid, nbg, BPAR["S"], BPAR["p"], BPAR["L"])[0]
        rp = rpot(15.0)
        a = rp / math.sqrt(1 + ci0 / 5)
        b = req([max(0.1, rp * math.exp(-BPAR["beta"] * x)) for x in C0k])
        sweep.append(dict(spacing_m=s, sph=round(10000 / s / s), ci=round(ci0, 2), A_r_over_spacing=round(a / s, 3),
                          B_r_over_spacing=round(b / s, 3), A_ratio=round(a / rp, 3), B_ratio=round(b / rp, 3),
                          C1_ratio=round(1 / (1 + ci0 / 5), 3)))
    OUT["density_sweep"] = sweep
    OUT["runtime_s"] = round(time.time() - t_start, 1)
    OUT["script_sha256"] = hashlib.sha256(open(__file__, "rb").read()).hexdigest()
    OUT["roster_sha256_file"] = hashlib.sha256(open(sys.argv[1], "rb").read()).hexdigest()
    json.dump(OUT, open(sys.argv[2], "w"), indent=1)
    print("done", OUT["runtime_s"], "s")

if __name__ == "__main__":
    main()
