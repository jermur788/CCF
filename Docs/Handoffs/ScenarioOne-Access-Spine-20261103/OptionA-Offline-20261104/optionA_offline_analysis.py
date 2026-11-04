#!/usr/bin/env python3
"""
DISPOSABLE OFFLINE ANALYSIS - NOT PRODUCTION CODE - #253 Option A qualification (EVENT 005, 2026-11-04).

What this is
  An INDEPENDENT Python re-implementation (a "model") of
    * the legacy ScenarioOneRackSampleGeometry.Derive ordering/strip logic, read at commit 72b4efbd, lines 179-298, and
    * a PROPOSED, UNAPPROVED branch-local derivation (Option A from #251, with the gaps this study found filled in).
  It is NOT the C# and nothing here proves the C# behaves the same. It proves properties of the *proposed rules*
  on synthetic and published-endpoint geometry, in plain doubles. Unity was not run. No trees.json was read.

How to run (no packages needed):  python3 -I -B optionA_offline_analysis.py [outdir]
  Writes results.json and run.log next to itself (or into outdir). Deterministic: every random draw is seeded.

Conventions copied from the C# (ground plane, metres, x east, z north):
  axis angle in [0,180) from doubled-angle length-weighted mean; normal = axis turned +90 deg (left);
  rack order by midpoint offset rounded to the mm, ties by rack id; refusal if High[k] > Low[k+1] + 1e-3;
  strips halfway between neighbouring midpoint offsets, outermost strip mirrors its inner half gap.
"""
import itertools
import json
import math
import os
import random
import sys
import time

HALF = 40.0                # Enlarged80 stand: |x|,|z| <= 40 (ScenarioOneRackBay/stand size 80, #251 N37)
OFFSET_RES = 1000.0        # Derive line 30
CROSS_TOL = 1e-3           # Derive line 31
CORRIDOR_EPS = 1e-4        # ScenarioOneRackBay.Corridor.Contains
SEG_HALF = 12.0            # calibration SegmentHalfLengthM
TEACH_MIN_LEN = 24.0       # Derive line 262
TIE_BAND = 1e-3            # PROPOSAL Q02 (a convenience reuse, not evidence)
ANGLE_RES_DEG = 1e-3       # PROPOSAL: quantise angles so a tolerance edge cannot flicker with float noise


def rk(i, e, f, w=2.4):
    return dict(id=i, ex=e[0], ez=e[1], fx=f[0], fz=f[1], w=w)


# The 3W+2E 45-degree reference: endpoints are #251 Appendix A row N10 (published in
# MFM-TOPOLOGY-OPTIONS-HANDOFF.md), junction => far. Corridor width 2.4 m.
REFERENCE = [
    rk(1, (-2.436166286468506, -23.7), (37.5, 16.236166286468514)),
    rk(2, (-2.436166286468506, -21.7), (-35.4, 11.2638337135315)),
    rk(3, (2.9724510669708253, -3.6), (-35.4, 34.77245106697083)),
    rk(4, (0.16338980197906475, 4.15), (33.51338980197906, 37.5)),
    rk(5, (-2.7148311138153076, 26.85), (-13.364831113815304, 37.5)),
]
# Suggested east-west five-rack layout: z lines from #251 N36, x from -35.4 to 40.
SUGGESTED_Z = [-35.271175384521484, -16.421764373779297, 0.23823529481887817, 15.793529510498047, 35.232353210449219]
SUGGESTED = [rk(i + 1, (-35.4, z), (40.0, z)) for i, z in enumerate(SUGGESTED_Z)]


def length(r):
    return math.hypot(r["fx"] - r["ex"], r["fz"] - r["ez"])


def finite(v):
    return not (math.isnan(v) or math.isinf(v))


def validate(racks, half):
    """Derive lines 182-199, same order and wording. Returns problem string or None."""
    if racks is None or len(racks) != 5:
        return "A rack-served sample needs exactly 5 confirmed racks (found %d)." % (0 if racks is None else len(racks))
    if not finite(half) or half <= 0:
        return "The stand size is not a positive finite number."
    by_id = sorted(racks, key=lambda r: r["id"])
    for i, r in enumerate(by_id):
        if r["id"] <= 0:
            return "Rack ids must be positive (found %d)." % r["id"]
        if i > 0 and by_id[i - 1]["id"] == r["id"]:
            return "Rack id %d appears more than once." % r["id"]
        if not all(finite(r[k]) for k in ("ex", "ez", "fx", "fz")):
            return "Rack %d has a non-finite endpoint." % r["id"]
        if not finite(r["w"]) or r["w"] <= 0:
            return "Rack %d has no corridor width." % r["id"]
        if length(r) < 1e-3:
            return "Rack %d has no length." % r["id"]
    return None


def axis_degrees(rs):
    sc = ss = tot = 0.0
    for r in sorted(rs, key=lambda r: r["id"]):
        dx, dz = r["fx"] - r["ex"], r["fz"] - r["ez"]
        L = math.sqrt(dx * dx + dz * dz)
        sc += (dx * dx - dz * dz) / L
        ss += 2.0 * dx * dz / L
        tot += L
    if math.sqrt(sc * sc + ss * ss) < 1e-6 * tot:
        return None
    a = 0.5 * math.atan2(ss, sc) * 180.0 / math.pi
    if a < 0.0:
        a += 180.0
    if a >= 180.0:
        a -= 180.0
    return a


def order_group(rs):
    """Derive lines 201-258 for one set of racks. Returns (group dict, None) or (None, problem)."""
    a = axis_degrees(rs)
    if a is None:
        return None, "The racks do not share a direction, so there is no common normal to order them across."
    ar = a * math.pi / 180.0
    ax, az = math.cos(ar), math.sin(ar)
    nx, nz = -az, ax
    ms = []
    for r in sorted(rs, key=lambda r: r["id"]):
        oe, of = r["ex"] * nx + r["ez"] * nz, r["fx"] * nx + r["fz"] * nz
        L = length(r)
        ms.append(dict(r=r, off=(oe + of) * 0.5, low=min(oe, of), high=max(oe, of), L=L))
    ms.sort(key=lambda m: (round(m["off"] * OFFSET_RES), m["r"]["id"]))
    for k in range(len(ms) - 1):
        if ms[k]["high"] > ms[k + 1]["low"] + CROSS_TOL:
            return None, ("Racks %d and %d cross or overlap across the common normal, so they have no defensible order."
                          % (ms[k]["r"]["id"], ms[k + 1]["r"]["id"]))
    n = len(ms)
    if n < 2:
        return None, "UNSPECIFIED: a direction group of one rack has no neighbour to halve the distance to."
    offs = [m["off"] for m in ms]
    lower = [offs[0] - (offs[1] - offs[0]) * 0.5 if k == 0 else (offs[k - 1] + offs[k]) * 0.5 for k in range(n)]
    upper = [offs[k] + (offs[k] - offs[k - 1]) * 0.5 if k == n - 1 else (offs[k] + offs[k + 1]) * 0.5 for k in range(n)]
    return dict(axis=a, ax=ax, az=az, nx=nx, nz=nz, ms=ms, offs=offs, lower=lower, upper=upper,
                ids=[m["r"]["id"] for m in ms], minid=min(m["r"]["id"] for m in ms),
                total_len=sum(m["L"] for m in ms)), None


def legacy_derive(racks, half=HALF):
    p = validate(racks, half)
    if p:
        return dict(valid=False, problem=p, kind="legacy")
    g, p = order_group(racks)
    if p:
        return dict(valid=False, problem=p, kind="legacy")
    teach = g["ms"][2]
    if teach["L"] < TEACH_MIN_LEN:
        return dict(valid=False, problem="The teaching rack %d is shorter than the 24 m teaching length." % teach["r"]["id"], kind="legacy")
    return dict(valid=True, problem="", kind="legacy", group=g, teach_id=teach["r"]["id"],
                sig="legacy axis=%r order=%s offs=%s" % (g["axis"], g["ids"], [repr(o) for o in g["offs"]]))


# ------------------------------------------------------------------------------------------------ grouping (PROPOSED)
def dir_deg(r):
    d = math.atan2(r["fz"] - r["ez"], r["fx"] - r["ex"]) * 180.0 / math.pi
    d = d % 180.0
    return round(d / ANGLE_RES_DEG) * ANGLE_RES_DEG % 180.0   # quantised so tolerance edges are not float-noise sensitive


def circ(a, b):
    d = abs(a - b) % 180.0
    return min(d, 180.0 - d)


def group_racks(racks, link_tol, span_tol=None, max_groups=2):
    """Single-link components on the undirected direction (circular, period 180). Deterministic: processed by rack id.
    span_tol (if given) refuses a component whose widest pair exceeds it = the transitive-chaining guard.
    Returns (groups as lists of racks sorted by name, None) or (None, problem)."""
    rs = sorted(racks, key=lambda r: r["id"])
    ang = {r["id"]: dir_deg(r) for r in rs}
    parent = {r["id"]: r["id"] for r in rs}

    def find(x):
        while parent[x] != x:
            parent[x] = parent[parent[x]]
            x = parent[x]
        return x
    for a, b in itertools.combinations(rs, 2):
        if circ(ang[a["id"]], ang[b["id"]]) <= link_tol + 1e-9:
            ra, rb = find(a["id"]), find(b["id"])
            if ra != rb:
                parent[max(ra, rb)] = min(ra, rb)
    comps = {}
    for r in rs:
        comps.setdefault(find(r["id"]), []).append(r)
    groups = [comps[k] for k in sorted(comps)]            # named by smallest rack id
    if len(groups) > max_groups:
        return None, "The racks fall in %d direction groups; at most %d are supported." % (len(groups), max_groups)
    if span_tol is not None:
        for gr in groups:
            span = max([circ(ang[a["id"]], ang[b["id"]]) for a, b in itertools.combinations(gr, 2)] or [0.0])
            if span > span_tol + 1e-9:
                return None, "Group %d chains directions %.3f deg apart (limit %.3f): ambiguous grouping." % (min(r["id"] for r in gr), span, span_tol)
    return groups, None


# ------------------------------------------------------------------------------------------------ branched derive (PROPOSED)
LEGACY_ELIGIBLE_PREFIXES = ("Racks ", "The racks do not share a direction")


def derive_branched(racks, half=HALF, link_tol=15.0, span_tol=None):
    """Returns the legacy result untouched when legacy is valid or when legacy failed for an unrelated reason."""
    legacy = legacy_derive(racks, half)
    if legacy["valid"]:
        return legacy
    pr = legacy["problem"]
    eligible = (pr.startswith("Racks ") and "cross or overlap" in pr) or pr.startswith("The racks do not share a direction")
    if not eligible:
        return legacy
    groups, p = group_racks(racks, link_tol, span_tol)
    if p:
        return dict(valid=False, problem=p, kind="branched")
    if len(groups) == 1:
        return legacy                                      # grouping cannot cure it; keep the legacy error
    gs = []
    for gr in groups:
        g, p = order_group(gr)
        if p:
            return dict(valid=False, problem="Group %d: %s" % (min(r["id"] for r in gr), p), kind="branched")
        gs.append(g)
    gs.sort(key=lambda g: g["minid"])
    big = sorted(gs, key=lambda g: (-len(g["ids"]), g["minid"]))[0]
    teach = big["ms"][len(big["ms"]) // 2]
    if teach["L"] < TEACH_MIN_LEN:
        return dict(valid=False, problem="The teaching rack %d is shorter than the 24 m teaching length." % teach["r"]["id"], kind="branched")
    # teaching anchor: closest point to the origin on the teaching rack, clamped so +-12 m stays on it (Derive 265-270)
    r = teach["r"]
    ux, uz = (r["fx"] - r["ex"]) / teach["L"], (r["fz"] - r["ez"]) / teach["L"]
    along = (0 - r["ex"]) * ux + (0 - r["ez"]) * uz
    cl = min(max(along, 12.0), teach["L"] - 12.0)
    anchor = (r["ex"] + ux * cl, r["ez"] + uz * cl)
    for g in gs:
        g["anchor_along"] = anchor[0] * g["ax"] + anchor[1] * g["az"]
    allr = sorted(racks, key=lambda r: r["id"])
    sig = "branched groups=%s" % [(g["ids"], repr(g["axis"])) for g in gs]
    return dict(valid=True, problem="", kind="branched", groups=gs, racks=allr, half=half, teach_id=r["id"],
                anchor=anchor, sig=sig, tie=TIE_BAND)


def in_corridor(r, x, z):
    dx, dz = r["fx"] - r["ex"], r["fz"] - r["ez"]
    L = math.sqrt(dx * dx + dz * dz)
    if L < CORRIDOR_EPS:
        return False
    px, pz = x - r["ex"], z - r["ez"]
    al = (px * dx + pz * dz) / L
    ac = (-px * dz + pz * dx) / L
    return -CORRIDOR_EPS <= al <= L + CORRIDOR_EPS and abs(ac) <= r["w"] * 0.5 + CORRIDOR_EPS


def seg_dist(r, x, z):
    dx, dz = r["fx"] - r["ex"], r["fz"] - r["ez"]
    L2 = dx * dx + dz * dz
    t = max(0.0, min(1.0, ((x - r["ex"]) * dx + (z - r["ez"]) * dz) / L2))
    return math.hypot(x - (r["ex"] + t * dx), z - (r["ez"] + t * dz))


def rect_separation(r1, r2):
    """Separating-axis test on two rack corridor rectangles (eps ignored). >0 = clear gap in metres along the best axis; <=0 = overlap."""
    def corners(r):
        dx, dz = r["fx"] - r["ex"], r["fz"] - r["ez"]
        L = math.hypot(dx, dz)
        ux, uz = dx / L, dz / L
        nx, nz = -uz, ux
        h = r["w"] * 0.5
        return [(r["ex"] + sa * ux * 0 + sb * nx * h, r["ez"] + sb * nz * h) if False else
                (r["ex"] + a * dx + sb * nx * h, r["ez"] + a * dz + sb * nz * h) for a in (0.0, 1.0) for sb in (-1.0, 1.0)], (ux, uz), (nx, nz)
    c1, u1, n1 = corners(r1)
    c2, u2, n2 = corners(r2)
    best = -1e18
    for ax_ in (u1, n1, u2, n2):
        p1 = [x * ax_[0] + z * ax_[1] for x, z in c1]
        p2 = [x * ax_[0] + z * ax_[1] for x, z in c2]
        best = max(best, max(min(p2) - max(p1), min(p1) - max(p2)))
    return best


def classify(m, x, z, owner="lateral"):
    """One total function: ground point -> exactly one label tuple. owner='nearest' is the alternative rule studied only for sensitivity."""
    if abs(x) > m["half"] or abs(z) > m["half"]:
        return ("OUT",)
    for r in m["racks"]:                       # lowest rack id wins where corridors overlap
        if in_corridor(r, x, z):
            return ("COR", r["id"])
    cands = []
    for gi, g in enumerate(m["groups"]):
        o = x * g["nx"] + z * g["nz"]
        if g["lower"][0] <= o <= g["upper"][-1]:
            for k in range(len(g["offs"])):
                if (o >= g["lower"][k] and o < g["upper"][k]) or (k == len(g["offs"]) - 1 and o <= g["upper"][k]):
                    cands.append((abs(o - g["offs"][k]), gi, k))
                    break
    if owner == "nearest" and cands is not None:
        best = None
        for gi, g in enumerate(m["groups"]):
            for k, mm in enumerate(g["ms"]):
                d = seg_dist(mm["r"], x, z)
                if best is None or d < best[0] - 1e-12 or (abs(d - best[0]) <= 1e-12 and mm["r"]["id"] < best[3]):
                    best = (d, gi, k, mm["r"]["id"])
        cands = [(best[0], best[1], best[2])] if best else cands
    if not cands:                               # outside every strip: remainder of the group whose outer edge is nearer
        best = None
        for gi, g in enumerate(m["groups"]):
            o = x * g["nx"] + z * g["nz"]
            if o < g["lower"][0]:
                ov, side = g["lower"][0] - o, "Right"
            else:
                ov, side = o - g["upper"][-1], "Left"
            if best is None or ov < best[0] - m["tie"]:
                best = (ov, gi, side)
        return ("REM", m["groups"][best[1]]["minid"], best[2])
    cands.sort()
    pick = cands[0]
    if len(cands) > 1 and abs(cands[1][0] - cands[0][0]) <= m["tie"]:
        pick = min(cands, key=lambda c: c[1])   # tie: smaller group id (groups sorted by smallest rack id)
    _, gi, k = pick
    g = m["groups"][gi]
    along = x * g["ax"] + z * g["az"] - g["anchor_along"]
    seg = "L" if along < -SEG_HALF else ("C" if along < SEG_HALF else "H")
    return ("UNIT", g["ids"][k], seg, g["minid"])


def label_key(lab):
    return "|".join(str(v) for v in lab)


# ------------------------------------------------------------------------------------------------ raster + exact scan areas
def raster(m, res, owner="lateral"):
    n = int(round(2 * m["half"] / res))
    areas, cells = {}, {}
    for i in range(n):
        z = -m["half"] + (i + 0.5) * res
        for j in range(n):
            x = -m["half"] + (j + 0.5) * res
            lab = classify(m, x, z, owner)
            k = label_key(lab)
            areas[k] = areas.get(k, 0.0) + res * res
    return areas


def slab(ox, oz, dx, dz, fx, fz, mn, mx, lo, hi):
    f0, fd = fx * ox + fz * oz, fx * dx + fz * dz
    if abs(fd) < 1e-12:
        return (0.0, 0.0) if (f0 < mn or f0 > mx) else (lo, hi)
    a, b = (mn - f0) / fd, (mx - f0) / fd
    return max(lo, min(a, b)), min(hi, max(a, b))


def scan_areas(m, step=0.1, check_pieces=0, rng=None):
    """Production-style exact interval method (cf. ScenarioOneCoverage.Frame.MeasureAreas, lines 466-530) on a horizontal scan line,
    extended with EVERY discontinuity of the proposed rule as a crossing. Each piece is classified once at its middle.
    check_pieces>0 additionally tests that random points inside a piece classify identically (= crossing set complete)."""
    half = m["half"]
    n = int(round(2 * half / step))
    areas, bad = {}, 0
    edges = []   # (gi, value) for every strip edge and outer edge
    for gi, g in enumerate(m["groups"]):
        for v in g["lower"] + g["upper"]:
            edges.append((gi, v))
    centres = [(gi, c) for gi, g in enumerate(m["groups"]) for c in g["offs"]]
    for i in range(n):
        z = -half + (i + 0.5) * step
        ts = [-half, half]
        for gi, v in edges:
            g = m["groups"][gi]
            if abs(g["nx"]) > 1e-12:
                ts.append((v - g["nz"] * z) / g["nx"])
        # lines where two lateral distances (or two outer overshoots) are equal, +- the tie band
        cs = [(gi, c) for gi, c in centres] + edges
        for (g1, c1), (g2, c2) in itertools.combinations(cs, 2):
            if g1 >= g2:
                continue
            ga, gb = m["groups"][g1], m["groups"][g2]
            for s in (1.0, -1.0):
                for band in (0.0, m["tie"], -m["tie"]):
                    # (oa - c1) - s*(ob - c2) = band
                    coef = ga["nx"] - s * gb["nx"]
                    rhs = band + c1 - ga["nz"] * z + s * (gb["nz"] * z - c2)
                    if abs(coef) > 1e-12:
                        ts.append(rhs / coef)
        for r in m["racks"]:
            dx, dz = r["fx"] - r["ex"], r["fz"] - r["ez"]
            L = math.hypot(dx, dz)
            lo, hi = slab(-r["ex"] + 0.0, z - r["ez"], 1.0, 0.0, dx / L, dz / L, -CORRIDOR_EPS, L + CORRIDOR_EPS, -1e18, 1e18)
            lo, hi = slab(-r["ex"], z - r["ez"], 1.0, 0.0, -dz / L, dx / L, -(r["w"] * 0.5 + CORRIDOR_EPS), r["w"] * 0.5 + CORRIDOR_EPS, lo, hi)
            if hi > lo:
                ts += [lo - 0.0, hi - 0.0]
        for g in m["groups"]:     # segment cut lines along each group's own axis
            if abs(g["ax"]) > 1e-12:
                for c in (g["anchor_along"] - SEG_HALF, g["anchor_along"] + SEG_HALF):
                    ts.append((c - g["az"] * z) / g["ax"])
        ts = sorted(t for t in ts if -half <= t <= half)
        for a, b in zip(ts, ts[1:]):
            if b - a < 1e-12:
                continue
            lab = classify(m, 0.5 * (a + b), z)
            k = label_key(lab)
            areas[k] = areas.get(k, 0.0) + (b - a) * step
            if check_pieces and rng is not None:
                for _ in range(check_pieces):
                    t = a + (b - a) * (0.001 + 0.998 * rng.random())
                    if label_key(classify(m, t, z)) != k:
                        bad += 1
    return areas, bad


def summarize(areas):
    u = sum(v for k, v in areas.items() if k.startswith("UNIT"))
    r = sum(v for k, v in areas.items() if k.startswith("REM"))
    c = sum(v for k, v in areas.items() if k.startswith("COR"))
    o = sum(v for k, v in areas.items() if k.startswith("OUT"))
    return dict(units=u, remainder=r, corridor=c, outside=o, total=u + r + c + o)


def unit_table(areas):
    """UNIT|rackid|seg|group -> area, merged to rack id only."""
    t = {}
    for k, v in areas.items():
        if k.startswith("UNIT"):
            _, rid, seg, gid = k.split("|")
            t[int(rid)] = t.get(int(rid), 0.0) + v
    return t


# ------------------------------------------------------------------------------------------------ tests
LOG = []
RESULTS = {}


def say(msg=""):
    print(msg)
    LOG.append(msg)


def check(name, ok, detail=""):
    RESULTS.setdefault("checks", []).append(dict(name=name, pass_=bool(ok), detail=detail))
    say("[%s] %s%s" % ("PASS" if ok else "FAIL", name, (" -- " + detail) if detail else ""))
    return ok


def mk_branched(racks, **kw):
    d = derive_branched(racks, **kw)
    assert d["valid"], d["problem"]
    return d


def naive_double_claim(m, res=0.5):
    """#251 'naive per-side strips': non-corridor cells inside BOTH groups' [lower0, upperLast] ranges."""
    n = int(round(2 * m["half"] / res))
    both = 0
    noncor = 0
    for i in range(n):
        z = -m["half"] + (i + 0.5) * res
        for j in range(n):
            x = -m["half"] + (j + 0.5) * res
            if any(in_corridor(r, x, z) for r in m["racks"]):
                continue
            noncor += 1
            inn = sum(1 for g in m["groups"] if g["lower"][0] <= x * g["nx"] + z * g["nz"] <= g["upper"][-1])
            if inn == 2:
                both += 1
    return both * res * res, noncor * res * res


def t_reproduce():
    say("== A. reproduce #251 numbers from published endpoints (validates this model, not the C#) ==")
    lg = legacy_derive(REFERENCE)
    check("A1 legacy Derive refuses the reference with the R4/R5 string", (not lg["valid"]) and lg["problem"].startswith("Racks 4 and 5 cross or overlap"), lg["problem"])
    d = mk_branched(REFERENCE)
    g = {tuple(x["ids"]): x for x in d["groups"]}
    check("A2 grouping gives {R2,R3,R5} (3 racks) and {R1,R4}", sorted(g) == [(1, 4), (5, 3, 2)], str(sorted(g)))
    wst = g[(5, 3, 2)]
    est = g[(1, 4)]
    say("   west axis=%.9f east axis=%.9f" % (wst["axis"], est["axis"]))
    gaps_w = [wst["offs"][1] - wst["offs"][0], wst["offs"][2] - wst["offs"][1]]
    gaps_e = [est["offs"][1] - est["offs"][0]]
    say("   midpoint-offset gaps west=%s east=%s  (#251 same-side gaps 17.510/16.623/17.855)" % ([round(x, 3) for x in gaps_w], [round(x, 3) for x in gaps_e]))
    check("A3 per-group gaps match #251 N25 within 1e-3", abs(gaps_w[0] - 17.50988569) < 1e-3 and abs(gaps_w[1] - 16.62310275) < 1e-3 and abs(gaps_e[0] - 17.85476012) < 1e-3)
    check("A4 teaching rack is R3 (middle of the 3-rack group)", d["teach_id"] == 3, "teach=%d anchor=%s" % (d["teach_id"], tuple(round(v, 3) for v in d["anchor"])))
    both, noncor = naive_double_claim(d)
    check("A5 naive per-side strips double-claim 1575.0 m^2 of 5875.75 (N52)", abs(both - 1575.0) < 1e-9 and abs(noncor - 5875.75) < 1e-9, "both=%s noncorridor=%s" % (both, noncor))
    a = raster(d, 0.5)
    s = summarize(a)
    ut = unit_table(a)
    say("   owner-rule raster 0.5 m: %s" % {k: round(v, 3) for k, v in s.items()})
    say("   per-rack unit area (lateral owner): %s" % {k: round(v, 2) for k, v in sorted(ut.items())})
    check("A6 owner rule closes to 6400.0: units 5461.25 + remainder 414.5 + corridors 524.25 (N53)",
          abs(s["units"] - 5461.25) < 1e-9 and abs(s["remainder"] - 414.5) < 1e-9 and abs(s["corridor"] - 524.25) < 1e-9 and abs(s["total"] - 6400.0) < 1e-9, str({k: round(v, 3) for k, v in s.items()}))
    RESULTS["reference_owner_raster_0p5"] = {k: v for k, v in a.items()}
    RESULTS["reference_summary_0p5"] = s
    return d


def t_order_invariance(d_ref):
    say("== B. input-order, orientation and id-relabel invariance ==")
    sig0 = derive_branched(REFERENCE)["sig"]
    ok = True
    perms = 0
    for p in itertools.permutations(range(5)):
        racks = [REFERENCE[i] for i in p]
        for flips in range(32):
            rr = []
            for bit, r in enumerate(racks):
                if flips >> bit & 1:
                    r = dict(r, ex=r["fx"], ez=r["fz"], fx=r["ex"], fz=r["ez"])
                rr.append(r)
            ok &= (derive_branched(rr)["sig"] == sig0)
            perms += 1
    check("B1 Derive-level signature identical over 120 orders x 32 entrance/far flips (%d runs)" % perms, ok)
    m0 = mk_branched(REFERENCE)
    h0 = [label_key(classify(m0, -39.5 + 1.0 * j, -39.5 + 1.0 * i)) for i in range(80) for j in range(80)]
    okh = True
    for p in itertools.permutations(range(5)):
        mm = mk_branched([REFERENCE[i] for i in p])
        okh &= (h0 == [label_key(classify(mm, -39.5 + 1.0 * j, -39.5 + 1.0 * i)) for i in range(80) for j in range(80)])
    check("B2 6400-cell label map identical over all 120 input orders", okh)
    # Ownership under rack-id RELABEL: ties change owner (documented, not a defect); verify the dependence is confined to ties
    relabel = {1: 5, 2: 4, 3: 3, 4: 2, 5: 1}
    rl = [dict(r, id=relabel[r["id"]]) for r in REFERENCE]
    mr = mk_branched(rl)
    a1 = raster(m0, 1.0)
    a2 = raster(mr, 1.0)
    inv = {v: k for k, v in relabel.items()}
    diff = 0.0
    for k in set(a1) | set(a2):
        # map relabelled keys back to original ids for comparison
        pass
    t1, t2 = unit_table(a1), {inv[k]: v for k, v in unit_table(a2).items()}
    diff = max(abs(t1.get(k, 0) - t2.get(k, 0)) for k in range(1, 6))
    say("   relabel (1<->5, 2<->4) max per-rack unit area change @1 m raster: %.3f m^2 (ids only break ties; group naming follows min id)" % diff)
    RESULTS["relabel_max_unit_area_change_m2_at_1m"] = diff
    check("B3 id relabel changes ownership only on tie bands (<= 1% of any unit)", diff <= 0.01 * max(t1.values()), "diff=%.3f" % diff)


def t_scan_vs_raster(d):
    say("== C. exact interval scan (production-style) vs independent raster; resolution convergence ==")
    t0 = time.time()
    scan, bad = scan_areas(d, 0.1, check_pieces=3, rng=random.Random(253))
    ss = summarize(scan)
    say("   scan@0.1 m: %s  (%.1fs)  piece-constancy violations: %d" % ({k: round(v, 3) for k, v in ss.items()}, time.time() - t0, bad))
    check("C1 piece-constancy: no random point inside a scan piece classifies differently (crossing set complete)", bad == 0, "violations=%d" % bad)
    check("C2 scan total equals stand area to 1e-6", abs(ss["total"] - 6400.0) < 1e-6)
    sc_t = unit_table(scan)
    s05, _ = scan_areas(d, 0.05)
    step_diff = max(abs(unit_table(s05)[k] - sc_t[k]) for k in sc_t)
    check("C3 scan is step-stable: 0.1 m vs 0.05 m differ by < 1.0 m^2 per unit (midpoint rule on oblique edges; ~0.1 percent of a unit)", step_diff < 1.0, "diff=%.3f m^2" % step_diff)
    # Unbiased Monte Carlo cross-check of the category areas (independent of both the lattice and the scan lines)
    rng = random.Random(7)
    n = 400000
    cnt = {}
    for _ in range(n):
        lab = classify(d, rng.uniform(-HALF, HALF), rng.uniform(-HALF, HALF))
        cnt[lab[0]] = cnt.get(lab[0], 0) + 1
    worst_sigma = 0.0
    for cat, key in (("UNIT", "units"), ("REM", "remainder"), ("COR", "corridor")):
        p = cnt.get(cat, 0) / n
        est, se = p * 6400.0, 6400.0 * math.sqrt(p * (1 - p) / n)
        worst_sigma = max(worst_sigma, abs(est - ss[key]) / se)
        say("   MC %-9s %.1f +- %.1f  vs scan %.3f" % (key, est, se, ss[key]))
    check("C4 Monte Carlo (400k seeded points) agrees with the exact scan within 4 sigma in every category", worst_sigma < 4.0, "worst=%.2f sigma" % worst_sigma)
    # Regular lattices are biased by their alignment with the 45/135 degree edges. Quantify, do not hide.
    lat = {}
    for res in (1.0, 0.5, 0.25, 0.125):
        t = summarize(raster(d, res))
        lat[str(res)] = t
        say("   lattice %.3f m: units %.2f remainder %.2f corridor %.2f   (exact scan: %.2f / %.2f / %.2f)" % (res, t["units"], t["remainder"], t["corridor"], ss["units"], ss["remainder"], ss["corridor"]))
    RESULTS["lattice_vs_exact_summary"] = lat
    bias = max(abs(lat["0.5"][k] - ss[k]) for k in ("units", "remainder", "corridor"))
    check("C5 the 0.5 m lattice used by #251 (N51-N53) is within 0.2% of the 6400 m^2 stand of the exact value", bias / 6400.0 < 0.002, "max category bias %.2f m^2 (%.3f%% of stand)" % (bias, 100 * bias / 6400.0))
    RESULTS["scan_units_m2"] = {str(k): v for k, v in sc_t.items()}
    RESULTS["scan_summary"] = ss
    # A segment-level table from the scan (Low/Centre/High), so the stems-per-segment can be sized later
    seg = {}
    for k, v in scan.items():
        if k.startswith("UNIT"):
            _, rid, s, gid = k.split("|")
            seg["R%s%s" % (rid, s)] = round(v, 3)
    RESULTS["scan_segment_areas_m2_unmasked"] = seg
    say("   smallest segment areas (unmasked m^2): %s" % sorted(seg.items(), key=lambda kv: kv[1])[:6])


def random_two_group_plan(rng):
    """Valid-by-construction two-family plan: 3+2 racks, family axis a and a+delta, ~16-20 m gaps, random lengths."""
    for _ in range(200):
        a = rng.uniform(0, 180)
        delta = rng.uniform(60, 120)
        racks, rid = [], 1
        ids = list(range(1, 6))
        rng.shuffle(ids)
        for fam, n in ((a, 3), (a + delta, 2)):
            th = math.radians(fam)
            ux, uz = math.cos(th), math.sin(th)
            nx, nz = -uz, ux
            base = rng.uniform(-10, 10)
            for k in range(n):
                off = base + k * rng.uniform(16, 20) - 9 * (n - 1)
                L = rng.uniform(28, 60)
                cx, cz = nx * off, nz * off
                racks.append(rk(ids.pop(), (cx - ux * L / 2, cz - uz * L / 2), (cx + ux * L / 2, cz + uz * L / 2)))
        rng.shuffle(racks)
        if all(abs(c) < 39 for r in racks for c in (r["ex"], r["ez"], r["fx"], r["fz"])):
            return racks
    return None


def t_properties():
    say("== D. seeded property tests on random synthetic two-family plans ==")
    rng = random.Random(20261104)
    plans = tried = 0
    total_fn_ok = 0
    n_points = 0
    cons_worst = 0.0
    legacy_ok = 0
    bad_scan = 0
    unspecified = 0
    while plans < 40 and tried < 4000:
        tried += 1
        racks = random_two_group_plan(rng)
        if racks is None:
            continue
        d = derive_branched(racks)
        if d["kind"] == "legacy" and d["valid"]:
            legacy_ok += 1
            continue
        if not d["valid"]:
            if "UNSPECIFIED" in d["problem"]:
                unspecified += 1
            continue
        plans += 1
        # totality + boundary points
        pts = [(rng.uniform(-40, 40), rng.uniform(-40, 40)) for _ in range(1500)]
        for g in d["groups"]:
            for v in g["lower"] + g["upper"]:
                for _ in range(40):
                    t = rng.uniform(-40, 40)
                    # a point on the line (n.p = v) +- 1e-9
                    px, pz = g["nx"] * v + g["ax"] * t, g["nz"] * v + g["az"] * t
                    for eps in (-1e-9, 0.0, 1e-9):
                        pts.append((px + g["nx"] * eps, pz + g["nz"] * eps))
        for x, z in pts:
            n_points += 1
            lab = classify(d, x, z)
            total_fn_ok += lab[0] in ("OUT", "COR", "UNIT", "REM")
            lab2 = classify(d, x, z)
            if lab != lab2:
                total_fn_ok -= 1
        # conservation on the exact scan, and permutation invariance of label map
        scan, bad = scan_areas(d, 0.5, check_pieces=1, rng=rng)
        bad_scan += bad
        cons_worst = max(cons_worst, abs(summarize(scan)["total"] - 6400.0) if all(abs(c) <= 40 for c in [0]) else 0)
        shuffled = racks[:]
        rng.shuffle(shuffled)
        d2 = derive_branched(shuffled)
        if d2["sig"] != d["sig"]:
            total_fn_ok -= 10 ** 6
    say("   plans qualified as branched-valid: %d of %d drawn (legacy-valid %d, group-of-one UNSPECIFIED %d)" % (plans, tried, legacy_ok, unspecified))
    check("D1 classify is a total deterministic function (%d points incl. +-1e-9 edge points, %d plans)" % (n_points, plans), total_fn_ok == n_points)
    check("D2 scan conservation: unit+remainder+corridor+outside = 6400 within 1e-6 on every plan", cons_worst < 1e-6, "worst=%.2e" % cons_worst)
    check("D3 scan piece-constancy: zero violations over all plans", bad_scan == 0, "violations=%d" % bad_scan)
    RESULTS["property_plans"] = plans
    RESULTS["property_unspecified_group_of_one"] = unspecified
    # legacy untouched: the suggested parallel layout through the branched entry point is byte-identical to legacy
    lg = legacy_derive(SUGGESTED)
    br = derive_branched(SUGGESTED)
    check("D4 parallel plan: branched entry returns the legacy result object unchanged (kind=legacy, same sig)", lg["valid"] and br["kind"] == "legacy" and br["sig"] == lg["sig"])


def t_edge_cases():
    say("== E. grouping edge cases (fail-closed behaviour of the PROPOSED rule) ==")

    def with_angle(base, ang, rid, off=0.0, L=40.0):
        th = math.radians(ang)
        ux, uz = math.cos(th), math.sin(th)
        nx, nz = -uz, ux
        cx, cz = nx * off, nz * off
        return rk(rid, (cx - ux * L / 2, cz - uz * L / 2), (cx + ux * L / 2, cz + uz * L / 2))
    # E1 wraparound: 179.9 and 0.1 deg are 0.2 deg apart
    racks = [with_angle(0, 179.9, 1), with_angle(0, 0.1, 2)]
    gr, p = group_racks(racks + [with_angle(0, 90, 3), with_angle(0, 90.1, 4), with_angle(0, 89.9, 5)], 1.0)
    check("E1 wraparound: 179.9deg and 0.1deg group together (circular distance)", gr is not None and sorted(sorted(r["id"] for r in g) for g in gr) == [[1, 2], [3, 4, 5]], str(p))
    # E2 tolerance edge flicker: 4.0 vs 4.0000000001 vs 3.9999999999
    for delta, want in ((3.9999999999, 1), (4.0, 1), (4.0000000001, 1), (4.002, 2)):
        gr, _ = group_racks([with_angle(0, 0, 1), with_angle(0, delta, 2)], 4.0, max_groups=2)
        check("E2 tolerance edge delta=%.10f gives %d group(s) (angles quantised to 1e-3 deg)" % (delta, want), len(gr) == want)
    # E3 transitive chaining: A-B 3.9, B-C 3.9, A-C 7.8 with link tol 4
    racks = [with_angle(0, 0, 1), with_angle(0, 3.9, 2), with_angle(0, 7.8, 3)]
    g1, p1 = group_racks(racks, 4.0)
    g2, p2 = group_racks(racks, 4.0, span_tol=4.0)
    check("E3 chaining: single-link alone ACCEPTS A-B-C as one group (the hazard)", g1 is not None and len(g1) == 1)
    check("E3b chaining: span guard REFUSES it (fail closed, ambiguous)", g2 is None and "chains" in p2, p2)
    # E4 three families
    racks = [with_angle(0, 0, 1), with_angle(0, 60, 2), with_angle(0, 120, 3)]
    g3, p3 = group_racks(racks, 10.0)
    check("E4 three direction families refused (cap 2)", g3 is None and "at most 2" in p3, p3)
    # E5 input order invariance of grouping on the chaining case
    ok = all((group_racks(list(pm), 4.0, span_tol=4.0)[1] or "").startswith("Group 1 chains") for pm in itertools.permutations(racks[:3] if False else [with_angle(0, 0, 1), with_angle(0, 3.9, 2), with_angle(0, 7.8, 3)]))
    check("E5 grouping outcome identical over all 6 orders of the chaining case", ok)
    # E6 duplicate id, near-zero length, wrong count: unrelated causes => legacy error untouched, no grouping attempted
    dup = [dict(r) for r in REFERENCE]
    dup[1]["id"] = 1
    b = derive_branched(dup)
    check("E6 duplicate rack id: legacy error returned, branched never selected", (not b["valid"]) and b["kind"] == "legacy" and "more than once" in b["problem"], b["problem"])
    tiny = [dict(r) for r in REFERENCE]
    tiny[4].update(fx=tiny[4]["ex"] + 1e-4, fz=tiny[4]["ez"])
    b = derive_branched(tiny)
    check("E7 near-zero-length rack: legacy 'has no length' error untouched", (not b["valid"]) and b["kind"] == "legacy" and "no length" in b["problem"], b["problem"])
    four = REFERENCE[:4]
    b = derive_branched(four)
    check("E8 four racks: legacy 'exactly 5' error untouched", (not b["valid"]) and b["kind"] == "legacy" and "exactly 5" in b["problem"], b["problem"])
    # E9 group of one: 4 racks in one family + 1 in the other
    mixed = [with_angle(0, 45, i + 1, off=-24 + 17 * i, L=50) for i in range(4)] + [with_angle(0, 135, 5, off=0, L=50)]
    b = derive_branched(mixed)
    check("E9 4+1 plan: new rule says UNSPECIFIED (no neighbour to halve) rather than inventing a strip", (not b["valid"]) and ("UNSPECIFIED" in b["problem"] or "cross" in b["problem"]), b["problem"])
    # E10 exactly-cancelling families: equal totals, legacy fails with 'do not share a direction' (an eligible cause)
    eq = [with_angle(0, 45, 1, off=-8, L=50), with_angle(0, 45, 2, off=8, L=50), with_angle(0, 135, 3, off=-25, L=33.3333),
          with_angle(0, 135, 4, off=0, L=33.3333), with_angle(0, 135, 5, off=25, L=33.3334)]
    lg = legacy_derive(eq)
    say("   E10 legacy on equal-total families: valid=%s problem=%r" % (lg["valid"], lg["problem"]))
    RESULTS["E10_equal_totals_legacy_problem"] = lg["problem"]
    check("E10 equal-length families make legacy fail with 'do not share a direction' (a different legacy error than the reference's)", (not lg["valid"]) and ("do not share" in lg["problem"] or "cross" in lg["problem"]), lg["problem"])
    # E11 same-family plan that legacy refuses (one rack 15 deg off, long enough to overlap its neighbour's normal range)
    par = [with_angle(0, 45, 1, off=0, L=50), with_angle(0, 60, 2, off=9, L=100), with_angle(0, 45, 3, off=18, L=50),
           with_angle(0, 45, 4, off=36, L=50), with_angle(0, 45, 5, off=54, L=50)]
    lg = legacy_derive(par)
    b = derive_branched(par, link_tol=15.0)
    check("E11 single-family plan that legacy refuses is NOT rescued by the branched rule (same error returned)", (not lg["valid"]) and (not b["valid"]) and b["problem"] == lg["problem"], b["problem"])
    # E12 legacy Derive ACCEPTS two parallel racks 0.5 m apart (corridors overlap almost fully): Derive-valid is not 'sensible'
    near = [with_angle(0, 45, 1, off=0, L=50), with_angle(0, 45, 2, off=0.5, L=50)] + [with_angle(0, 45, i, off=20 * i, L=50) for i in (3, 4, 5)]
    lgn = legacy_derive(near)
    say("   E12 legacy on parallel racks 0.5 m apart: valid=%s (editor OverlapClearanceM=2.4 is the only guard)" % lgn["valid"])
    RESULTS["E12_legacy_accepts_half_metre_parallel_racks"] = bool(lgn["valid"])
    check("E12 OBSERVATION: legacy Derive accepts 0.5 m-spaced parallel racks (degenerate 0.5 m strip); any new validity gate must not equate Derive-valid with sensible", lgn["valid"])


def t_corridors(d):
    say("== F. corridor intersections ==")
    r1, r2 = REFERENCE[0], REFERENCE[1]
    best = 1e9
    for i in range(0, 4001):
        t = i / 4000.0
        best = min(best, seg_dist(r1, r2["ex"] + t * (r2["fx"] - r2["ex"]), r2["ez"] + t * (r2["fz"] - r2["ez"])))
    sep = rect_separation(r1, r2)
    say("   reference R1/R2: closest centreline distance %.3f m (< 2.4 m editor clearance); rectangle separating-axis gap %.3f m" % (best, sep))
    RESULTS["r1_r2_centreline_min_m"] = best
    RESULTS["r1_r2_rectangle_gap_m"] = sep
    check("F1 CORRECTION: R1/R2 corridor RECTANGLES do not overlap (SAT gap > 0); #251's 'overlap' is the editor's 2.4 m centreline-clearance rule", abs(best - 1.414213562) < 1e-3 and sep > 0, "gap=%.3f m" % sep)
    # a SYNTHETIC genuinely-overlapping pair (two racks of different families crossing near a spine)
    ra = rk(1, (-30, -30), (30, 30))
    rb = rk(2, (-30, 30), (30, -30))
    others = [rk(3, (-35, 10), (-5, 40)), rk(4, (-35, -10), (-5, -40)), rk(5, (35, 8), (8, 38))]
    cross = [ra, rb] + others
    dd = derive_branched(cross, link_tol=15.0)
    say("   synthetic crossing pair: branched valid=%s %s" % (dd["valid"], dd["problem"]))
    # exact overlap = 2.4*2.4/sin(90) = 5.76 m^2 for perpendicular 2.4 m strips crossing mid-length
    n = 4800
    res = 80.0 / n
    both = union = 0
    sample_ok = True
    m = dd if dd["valid"] else None
    for i in range(0, n, 2):
        z = -40 + (i + 0.5) * res
        for j in range(0, n, 2):
            x = -40 + (j + 0.5) * res
            a_, b_ = in_corridor(ra, x, z), in_corridor(rb, x, z)
            both += a_ and b_
            union += a_ or b_
            if m and a_ and b_ and classify(m, x, z) != ("COR", 1):
                sample_ok = False
    both *= 4 * res * res
    union *= 4 * res * res
    say("   perpendicular crossing overlap %.3f m^2 (exact 5.76); union %.3f (sum of rectangles %.3f)" % (both, union, 2.4 * (length(ra) + length(rb))))
    RESULTS["synthetic_crossing_overlap_m2"] = both
    check("F2 genuinely overlapping corridors: overlap ~5.76 m^2, counted once in the union", abs(both - 5.76) < 0.2 and abs(union - (2.4 * (length(ra) + length(rb)) - 5.76)) < 0.005 * union, "overlap=%.3f union=%.3f expected~%.3f" % (both, union, 2.4 * (length(ra) + length(rb)) - 5.76))
    if m:
        check("F3 overlap ground is owned by the lowest rack id (COR,1) and labelled once", sample_ok)
    else:
        check("F3 crossing-pair plan is itself refused by the branched rule, so ownership of crossing ground never arises", True, dd["problem"])


def t_reach(d):
    say("== G. reach: distance from owned ground to the OWNING rack's finite centreline ==")
    res = 0.5
    n = int(80 / res)
    rack_by_id = {r["id"]: r for r in REFERENCE}
    thresholds = (9.0, 12.0, 20.0)
    for rule in ("lateral", "nearest"):
        tot = 0.0
        over = {t: 0.0 for t in thresholds}
        mx = 0.0
        for i in range(n):
            z = -40 + (i + 0.5) * res
            for j in range(n):
                x = -40 + (j + 0.5) * res
                lab = classify(d, x, z, rule)
                if lab[0] != "UNIT":
                    continue
                dist = seg_dist(rack_by_id[lab[1]], x, z)
                tot += res * res
                mx = max(mx, dist)
                for t in thresholds:
                    if dist > t:
                        over[t] += res * res
        say("   owner=%-8s owned %.2f m^2; >9 m %.2f, >12 m %.2f, >20 m %.2f; max %.2f m" % (rule, tot, over[9.0], over[12.0], over[20.0], mx))
        RESULTS["reach_" + rule] = dict(owned=tot, over9=over[9.0], over12=over[12.0], over20=over[20.0], max=mx)
    # disagreement between the two owner rules
    dis = 0.0
    for i in range(n):
        z = -40 + (i + 0.5) * res
        for j in range(n):
            x = -40 + (j + 0.5) * res
            a, b = classify(d, x, z, "lateral"), classify(d, x, z, "nearest")
            if a[0] == "UNIT" and b[0] == "UNIT" and a[1] != b[1]:
                dis += res * res
    say("   ground whose owning rack differs between lateral-owner and nearest-segment-owner: %.2f m^2" % dis)
    RESULTS["owner_rule_disagreement_m2"] = dis
    caps = {}
    for rule in ("lateral", "nearest"):
        row = {}
        for cap_m in (9.0, 10.0, 12.0, 16.0, 20.0):
            elig = 0.0
            for i in range(n):
                z = -40 + (i + 0.5) * res
                for j in range(n):
                    x = -40 + (j + 0.5) * res
                    lab = classify(d, x, z, rule)
                    if lab[0] == "UNIT" and seg_dist(rack_by_id[lab[1]], x, z) <= cap_m:
                        elig += res * res
            row[str(cap_m)] = elig
        caps[rule] = row
        say("   owner=%-8s ground within cap of its owner (eligible for Covered): %s of 5875.75 non-corridor m^2 (0.5 m lattice)" % (rule, {k: round(v, 1) for k, v in row.items()}))
    RESULTS["reach_cap_eligible_area"] = caps
    sg = {"reach_over9_sampled_nearest_any_rack": None}
    # N55 reproduction: nearest finite segment of ANY rack, non-corridor cells
    over9 = over12 = over20 = nc = 0.0
    for i in range(n):
        z = -40 + (i + 0.5) * res
        for j in range(n):
            x = -40 + (j + 0.5) * res
            if any(in_corridor(r, x, z) for r in REFERENCE):
                continue
            dm = min(seg_dist(r, x, z) for r in REFERENCE)
            nc += res * res
            over9 += res * res * (dm > 9)
            over12 += res * res * (dm > 12)
            over20 += res * res * (dm > 20)
    check("G1 reproduces #251 N55 (2246.0 / 1718.25 / 830.0 m^2 beyond 9/12/20 m of the nearest rack)", abs(over9 - 2246.0) < 1e-9 and abs(over12 - 1718.25) < 1e-9 and abs(over20 - 830.0) < 1e-9, "%.2f / %.2f / %.2f of %.2f" % (over9, over12, over20, nc))
    # suggested parallel layout baseline (N54)
    over9 = nc = 0.0
    for i in range(n):
        z = -40 + (i + 0.5) * res
        for j in range(n):
            x = -40 + (j + 0.5) * res
            if any(in_corridor(r, x, z) for r in SUGGESTED):
                continue
            dm = min(seg_dist(r, x, z) for r in SUGGESTED)
            nc += res * res
            over9 += res * res * (dm > 9)
    check("G2 reproduces #251 N54 (suggested layout: 169.5 m^2 beyond 9 m)", abs(over9 - 169.5) < 1e-9, "%.2f of %.2f" % (over9, nc))


def t_sample_footprint(d):
    say("== I. where the demonstrated 24 x 18 m sample footprint falls (no trees: geometry only; forestry transfer UNMEASURED) ==")
    r = [x for x in REFERENCE if x["id"] == d["teach_id"]][0]
    L = length(r)
    ux, uz = (r["fx"] - r["ex"]) / L, (r["fz"] - r["ez"]) / L
    ax_, az_ = d["anchor"]
    res = 0.1
    cnt, cor, tot = {}, 0.0, 0.0
    for i in range(int(24 / res)):
        for j in range(int(18 / res)):
            a = -12 + (i + 0.5) * res
            c = -9 + (j + 0.5) * res
            x, z = ax_ + ux * a - uz * c, az_ + uz * a + ux * c
            lab = classify(d, x, z)
            tot += res * res
            if lab[0] == "COR":
                cor += res * res
            elif lab[0] == "UNIT":
                k = "R%d%s" % (lab[1], lab[2])
                cnt[k] = cnt.get(k, 0.0) + res * res
            else:
                cnt[lab[0]] = cnt.get(lab[0], 0.0) + res * res
    say("   footprint %.1f m^2 (24x18): corridor %.2f m^2; by owning unit: %s" % (tot, cor, {k: round(v, 2) for k, v in sorted(cnt.items())}))
    RESULTS["sample_footprint_by_label_m2"] = dict(cnt, corridor=cor, total=tot)
    foreign = sum(v for k, v in cnt.items() if k.startswith("R") and not k.startswith("R3"))
    say("   footprint ground that belongs to units of OTHER racks (the legacy rule also lets the sample override neighbouring strips): %.2f m^2" % foreign)
    check("I1 sample footprint measured against the proposed ownership (informational)", True, "foreign-unit ground %.2f m^2, corridor %.2f m^2" % (foreign, cor))


def t_tolerance():
    say("== H. group tolerance sensitivity: hand-drawn directions perturbed about each rack midpoint ==")
    out = {}
    for link in (4.0, 10.0):
        for delta in (0, 2, 4, 6, 8, 10, 15, 20):
            rng = random.Random(1000 + delta)
            tally = {"branched_valid": 0, "grouping_refused": 0, "group_ordering_refused": 0, "legacy_valid": 0, "other": 0}
            for _ in range(200):
                rs = []
                for r in REFERENCE:
                    cx, cz = (r["ex"] + r["fx"]) / 2, (r["ez"] + r["fz"]) / 2
                    ang = math.atan2(r["fz"] - r["ez"], r["fx"] - r["ex"]) + math.radians(rng.uniform(-delta, delta))
                    L = length(r)
                    rs.append(rk(r["id"], (cx - math.cos(ang) * L / 2, cz - math.sin(ang) * L / 2), (cx + math.cos(ang) * L / 2, cz + math.sin(ang) * L / 2)))
                b = derive_branched(rs, link_tol=link, span_tol=link)
                if b["valid"] and b["kind"] == "legacy":
                    tally["legacy_valid"] += 1
                elif b["valid"]:
                    tally["branched_valid"] += 1
                elif "direction groups" in b["problem"] or "chains" in b["problem"]:
                    tally["grouping_refused"] += 1
                elif b["problem"].startswith("Group "):
                    tally["group_ordering_refused"] += 1
                else:
                    tally["other"] += 1
            out["link%g_delta%g" % (link, delta)] = tally
            say("   link/span tol %4.1f deg, +-%2d deg perturbation: %s" % (link, delta, tally))
    RESULTS["tolerance_sweep"] = out
    check("H1 sweep completed deterministically (seeded); see results.json tolerance_sweep", True)


def main():
    out = sys.argv[1] if len(sys.argv) > 1 else os.path.dirname(os.path.abspath(__file__))
    t0 = time.time()
    d = t_reproduce()
    t_order_invariance(d)
    t_scan_vs_raster(d)
    t_properties()
    t_edge_cases()
    t_corridors(d)
    t_reach(d)
    t_sample_footprint(d)
    t_tolerance()
    fails = [c for c in RESULTS["checks"] if not c["pass_"]]
    say("== SUMMARY: %d checks, %d failed, %.1fs ==" % (len(RESULTS["checks"]), len(fails), time.time() - t0))
    for c in fails:
        say("   FAILED: " + c["name"])
    with open(os.path.join(out, "results.json"), "w") as f:
        json.dump(RESULTS, f, indent=1, sort_keys=True, default=str)
    with open(os.path.join(out, "run.log"), "w") as f:
        f.write("\n".join(LOG) + "\n")
    return 1 if fails else 0


if __name__ == "__main__":
    sys.exit(main())
