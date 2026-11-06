#!/usr/bin/env python3
"""
DISPOSABLE / EXPERIMENTAL / OFFLINE -- #271 matched comparison of MFM Option A and constrained fallback C.

NOT production code. NOT a test. NOT a policy default. Grants no approval of any spec or change.
Do not copy into Assets/. Delete after the Manager has consumed the handoff.
Run:  python3 -I -B mfm_a_vs_c_offline.py <outdir>      (stdlib only, deterministic, no random numbers)

Independent re-implementation. It does NOT import the #253 script (OptionA-Offline-20261104/optionA_offline_analysis.py).
It ports, from the C# at commit 72b4efbd (git show only; cited as file:line in the handoff):
  ScenarioOneRackSampleGeometry.Derive (axis, ordering, strips, teaching anchor)   -> derive_group()
  ScenarioOneRackBay.Corridor.Contains / Frame.Contains                            -> in_corridor(), in_sample()
  ScenarioOneCoverage.Mask.Contains / Locate precedence / Compare tolerances       -> classify_point(), compare()
FIDELITY LIMIT: this is a Python port. Matching C# numbers on the legacy layout (check V2) shows the ported legacy path agrees
with one recorded C# run; it says nothing about the A or C paths, which have no C# at all.
Competition (Hegyi) is NOT computed: 'covered' below is therefore a CEILING (density + mean DBH tests only).
"""
import json, math, sys, os, hashlib

EVID = "/media/jer/Files/CCF/Verification"
P_TREES = EVID + "/ScenarioOneAccessSpineMeasure-20261103T2110Z/measurements/trees.json"
P_MFM = EVID + "/ScenarioOneAccessSpineMeasure-20261103T2110Z/measurements/mfm.json"
P_SUGG = EVID + "/ScenarioOneAccessSpineMeasure-20261103T2110Z/measurements/suggested_racks.json"
P_DES = EVID + "/ScenarioOneFiveRackReference-20261103T2210Z/measurements/design_candidate.json"
P_MFMC = EVID + "/ScenarioOneFiveRackReference-20261103T2210Z/measurements/mfm_candidate.json"
RAW_SHA = {
    P_TREES: "822959038ffd2f4a0379e1b6c77bfff465ede76ec3141b675ab3035c42a5cbf0",
    P_DES: "f9ca1d1977427ad78f882a58b09088a4f2b7346578b845dca4b40e67fe07975c",
    P_MFMC: "67a24ac0f1288949f0dbfa149ae5538ed25faef465f403f9c1cd32ffc8aa15a2",
}

# ---- constants and their sources (C# at 72b4efbd unless stated) ----------------------------------------------------------
HALF = 40.0                 # stand half extent: 80x80 stand = 6400 m2 (meta.json frame text; #253 SS7)
CORR_W = 2.4                # rack corridor width: RackGeometry.DefaultWidthM, used by design_candidate/suggested_racks (width_m 2.4)
TEACH_HALF_LEN = 12.0       # ScenarioOneRackBay.HalfLengthM (ScenarioOneRackBay.cs:~28)
TEACH_HALF_W = 9.0          # ScenarioOneRackBay.TeachingHalfWidthM
SEG_HALF = 12.0             # ScenarioOnePrescriptionCalibration.V1.SegmentHalfLengthM
DENS_TOL, DBH_TOL = 0.20, 0.15   # V1 calibration (ScenarioOnePrescriptionCalibration.cs:73-75)
MIN_STEMS = 30              # V1 calibration MinimumStems
CAPS = (20.0, 15.0, 12.0, 9.0)   # task brief: reuse #253 reach caps. NOTE: #253 tabulated 9/10/12/16/20; 15 is NOT in #253 (my addition)
CELL = 0.5                  # lattice spacing, same convention as #251/#253 (centres from -39.75)
CROSS_TOL = 1e-3            # CrossingToleranceM
OFFSET_RES = 1000.0         # OffsetResolutionPerM
MIN_RACK_LEN = 5.0          # RackEditing.MinLengthM
EDITOR_CLEARANCE = 2.4      # RackEditing.OverlapClearanceM = DefaultWidthM (RackEditing.cs:102)
HEADLAND_X = -35.4          # raw: design_candidate headland-centreline far x; ScenarioOneHeadlandGeometry x -35.4
WIN_E, WIN_N, WIN_S = 37.5, 37.5, -37.5  # E/N from design_candidate far ends ('east/west-window','north-window'); S is MY symmetric ASSUMPTION
GAP_ASSUMED = 16.0          # #251 SS3C.3 'gaps of at least 16 m' -- provenance unclear (see handoff); carried as a labelled assumption
LOG = []


def log(*a):
    s = " ".join(str(x) for x in a)
    LOG.append(s)
    print(s)


def sha(p):
    h = hashlib.sha256()
    h.update(open(p, "rb").read())
    return h.hexdigest()


# ---- geometry primitives -------------------------------------------------------------------------------------------------
class Rack:
    def __init__(self, rid, e, f, w=CORR_W):
        self.id = rid
        self.ex, self.ez = e
        self.fx, self.fz = f
        self.w = w
        dx, dz = self.fx - self.ex, self.fz - self.ez
        self.L = math.hypot(dx, dz)
        self.ux, self.uz = dx / self.L, dz / self.L

    def contains(self, x, z):      # Corridor.Contains (ScenarioOneRackBay.cs:128-137), epsilon 1e-4
        px, pz = x - self.ex, z - self.ez
        along = px * self.ux + pz * self.uz
        across = -px * self.uz + pz * self.ux
        return -1e-4 <= along <= self.L + 1e-4 and abs(across) <= self.w * 0.5 + 1e-4

    def seg_dist(self, x, z):      # distance to the finite centreline segment
        px, pz = x - self.ex, z - self.ez
        t = max(0.0, min(self.L, px * self.ux + pz * self.uz))
        return math.hypot(px - t * self.ux, pz - t * self.uz)


def derive_group(racks, half=HALF):
    """Legacy Derive for ONE direction family (ScenarioOneRackSampleGeometry.cs:179-298). Returns dict or {'valid':False,'problem':..}."""
    byid = sorted(racks, key=lambda r: r.id)
    if len(byid) < 2:
        return {"valid": False, "problem": "group of one rack: strips UNSPECIFIED (legacy needs offsets[1])"}
    sc = ss = tl = 0.0
    for r in byid:
        dx, dz = r.fx - r.ex, r.fz - r.ez
        sc += (dx * dx - dz * dz) / r.L
        ss += 2.0 * dx * dz / r.L
        tl += r.L
    if math.hypot(sc, ss) < 1e-6 * tl:
        return {"valid": False, "problem": "The racks do not share a direction"}
    ang = 0.5 * math.atan2(ss, sc) * 180 / math.pi
    if ang < 0: ang += 180
    if ang >= 180: ang -= 180
    a = ang * math.pi / 180
    ax, az = math.cos(a), math.sin(a)
    nx, nz = -az, ax
    m = []
    for r in byid:
        oe, of = r.ex * nx + r.ez * nz, r.fx * nx + r.fz * nz
        m.append({"r": r, "off": (oe + of) / 2, "low": min(oe, of), "high": max(oe, of)})
    m.sort(key=lambda d: (round(d["off"] * OFFSET_RES), d["r"].id))
    for k in range(len(m) - 1):
        if m[k]["high"] > m[k + 1]["low"] + CROSS_TOL:
            return {"valid": False, "problem": "Racks %d and %d cross or overlap across the common normal" % (m[k]["r"].id, m[k + 1]["r"].id),
                    "ids": (m[k]["r"].id, m[k + 1]["r"].id)}
    offs = [d["off"] for d in m]
    n = len(offs)
    for k in range(n):
        m[k]["lower"] = offs[0] - (offs[1] - offs[0]) * 0.5 if k == 0 else (offs[k - 1] + offs[k]) * 0.5
        m[k]["upper"] = offs[k] + (offs[k] - offs[k - 1]) * 0.5 if k == n - 1 else (offs[k] + offs[k + 1]) * 0.5
    return {"valid": True, "axis_deg": ang, "ax": ax, "az": az, "nx": nx, "nz": nz, "ordered": m,
            "name": byid[0].id, "ids": [d["r"].id for d in m]}


def teaching_anchor(rack):
    """Closest point to the stand origin on the teaching rack, clamped to keep the 24 m footprint on the rack (Derive :260-271)."""
    along = (0 - rack.ex) * rack.ux + (0 - rack.ez) * rack.uz
    cl = min(max(along, TEACH_HALF_LEN), rack.L - TEACH_HALF_LEN)
    return rack.ex + rack.ux * cl, rack.ez + rack.uz * cl, along, cl


class Layout:
    """One option under test: a list of racks split into direction groups, each derived by the unchanged legacy logic."""

    def __init__(self, name, racks, groups_ids, masks):
        self.name = name
        self.racks = racks
        self.byid = {r.id: r for r in racks}
        self.masks = masks
        self.groups = []
        self.problem = None
        for gid in groups_ids:
            g = derive_group([self.byid[i] for i in gid])
            if not g["valid"]:
                self.problem = g["problem"]
                return
            self.groups.append(g)
        # teaching rule (A.1 item 4, generalised): rack at index n//2 of the LARGEST group; single group of five -> index 2 (= legacy).
        big = max(self.groups, key=lambda g: (len(g["ids"]), -g["name"]))
        self.teach_group = big
        self.teach = big["ordered"][len(big["ordered"]) // 2]["r"]
        if self.teach.L < 2 * TEACH_HALF_LEN:
            self.problem = "teaching rack %d shorter than 24 m" % self.teach.id
            return
        self.anchor = teaching_anchor(self.teach)
        self.rack_group = {}
        for gi, g in enumerate(self.groups):
            for d in g["ordered"]:
                self.rack_group[d["r"].id] = gi
        # anchor projection on each group's own axis (A.1 item 5)
        for g in self.groups:
            g["anchor_along"] = self.anchor[0] * g["ax"] + self.anchor[1] * g["az"]

    def in_corridor(self, x, z):
        for r in self.racks:
            if r.contains(x, z):
                return True
        return False

    def in_sample(self, x, z):    # Frame.Contains: |along|<=12, |across|<=9, not in any corridor (ScenarioOneRackBay.cs:~166-168)
        ax_, az_, _, _ = self.anchor
        t = self.teach
        px, pz = x - ax_, z - az_
        along = px * t.ux + pz * t.uz
        across = -px * t.uz + pz * t.ux
        return abs(along) <= TEACH_HALF_LEN and abs(across) <= TEACH_HALF_W and not self.in_corridor(x, z)

    def masked(self, x, z):
        for (ax, az, bx, bz, rad) in self.masks:
            dx, dz = bx - ax, bz - az
            ls = dx * dx + dz * dz
            t = 0.0 if ls <= 0 else max(0.0, min(1.0, ((x - ax) * dx + (z - az) * dz) / ls))
            ex, ez = x - (ax + t * dx), z - (az + t * dz)
            if ex * ex + ez * ez <= rad * rad:
                return True
        return False

    def strip_claims(self, x, z):
        """Per group: (group_index, unit_rack, lateral_distance) if the point lies in that group's strip, using [lower,upper) with last closed."""
        out = []
        for gi, g in enumerate(self.groups):
            o = x * g["nx"] + z * g["nz"]
            n = len(g["ordered"])
            for k, d in enumerate(g["ordered"]):
                if (o >= d["lower"] and o < d["upper"]) or (k == n - 1 and o <= d["upper"] and o >= d["lower"]):
                    out.append((gi, d["r"], abs(o - d["off"])))
                    break
        return out

    def owner(self, x, z, rule):
        """Return (rack|None, group_index|None). rule 'lateral' = #251/#253 owner rule (remainder if no strip); 'nearest' = nearest finite centreline."""
        if rule == "lateral":
            c = self.strip_claims(x, z)
            if not c:
                return None, None
            best = c[0]
            for cc in c[1:]:
                if cc[2] < best[2] - CROSS_TOL or (abs(cc[2] - best[2]) <= CROSS_TOL and self.groups[cc[0]]["name"] < self.groups[best[0]]["name"]):
                    best = cc
            return best[1], best[0]
        best = None
        for r in sorted(self.racks, key=lambda r: r.id):
            d = r.seg_dist(x, z)
            if best is None or d < best[0] - 1e-12:
                best = (d, r)
        r = best[1]
        return r, self.rack_group[r.id]

    def segment(self, x, z, gi):
        g = self.groups[gi]
        along = x * g["ax"] + z * g["az"] - g["anchor_along"]
        return 0 if along < -SEG_HALF else (1 if along < SEG_HALF else 2)


SEGN = "LCH"


def lattice():
    n = int(round(2 * HALF / CELL))
    return [(-HALF + CELL * (i + 0.5), -HALF + CELL * (j + 0.5)) for i in range(n) for j in range(n)]


def compare(src, tgt):
    """ScenarioOneCoverage.Compare, density and mean DBH only. Competition NOT computed (CEILING)."""
    dd = (tgt["dens"] - src["dens"]) / src["dens"]
    db = (tgt["dbh"] - src["dbh"]) / src["dbh"]
    return abs(dd) <= DENS_TOL and abs(db) <= DBH_TOL, dd, db


# ---- evaluation ----------------------------------------------------------------------------------------------------------
def evaluate(lay, rule, stems, pts, use_masks=True):
    """Classify every lattice cell and stem once (production precedence: corridor > remainder > mask > sample > area).
    Returns per-item label rows; caps are applied afterwards."""
    cell_area = CELL * CELL

    def label(x, z):
        if lay.in_corridor(x, z):
            return ("COR", None, None, 0.0)
        r, gi = lay.owner(x, z, rule)
        if r is None:
            return ("REM", None, None, 0.0)
        d = r.seg_dist(x, z)
        if use_masks and lay.masked(x, z):
            return ("MASK", r.id, None, d)
        if lay.in_sample(x, z):
            return ("SAMPLE", r.id, None, d)
        return ("UNIT", r.id, lay.segment(x, z, gi), d)

    cells = [label(x, z) + (x, z) for (x, z) in pts]
    st = [label(s["x"], s["z"]) + (s["dbhCm"],) for s in stems]
    return cells, st, cell_area


def metrics_for(items_area, items_stems):
    stems = len(items_stems)
    area = items_area
    if area <= 0 or stems == 0:
        return {"stems": stems, "area": area, "dens": 0.0, "dbh": 0.0}
    return {"stems": stems, "area": area, "dens": stems / (area / 10000.0), "dbh": sum(items_stems) / stems}


def apply_cap(lay, cells, st, cell_area, cap):
    """Matched policy: ground farther than `cap` from its OWNING rack's finite centreline is Manual/NotCovered ('beyond reach')."""
    sa = sum(cell_area for c in cells if c[0] == "SAMPLE")
    ss = [s[4] for s in st if s[0] == "SAMPLE"]
    src = metrics_for(sa, ss)
    src_key = (lay.teach.id, 1)
    units = {(r.id, sg): {"area_all": 0.0, "area_in": 0.0} for r in lay.racks for sg in range(3)}   # production always has rack x {L,C,H}
    for c in cells:
        if c[0] == "UNIT":
            u = units.setdefault((c[1], c[2]), {"area_all": 0.0, "area_in": 0.0})
            u["area_all"] += cell_area
            if c[3] <= cap:
                u["area_in"] += cell_area
    return src, units, src_key


def run_option(lay, rule, stems, pts, caps, masks_on=True):
    cells, st, cell_area = evaluate(lay, rule, stems, pts, masks_on)   # stem tuple: (kind, rack, seg, dist, dbh)
    res = {}
    # geometry-only (no masks / no sample precedence) numbers for #253 comparability
    gcells, _, _ = evaluate(lay, rule, [], pts, use_masks=False)
    nonc = [c for c in gcells if c[0] != "COR"]
    ground = len(nonc) * cell_area
    rem = sum(1 for c in nonc if c[0] == "REM") * cell_area
    own = [c for c in nonc if c[0] != "REM"]
    geom = {"cells_total_m2": len(gcells) * cell_area, "corridor_union_m2": (len(gcells) - len(nonc)) * cell_area,
            "non_corridor_m2": ground, "remainder_m2": rem, "owned_m2": len(own) * cell_area,
            "max_owner_dist_m": max([c[3] for c in own]) if own else 0.0}
    for cap in caps:
        within = sum(1 for c in own if c[3] <= cap) * cell_area
        beyond = ground - within - rem
        geom["cap%g" % cap] = {"within_m2": within, "within_share": within / ground, "beyond_owner_m2": beyond,
                              "remainder_m2": rem, "uncovered_share": 1 - within / ground}
    res["geometry"] = geom
    # unit evaluation with masks + sample precedence
    out = {}
    for cap in caps:
        src, units, src_key = apply_cap(lay, cells, st, cell_area, cap)
        cats = {"corridor": 0, "remainder_manual": 0, "masked": 0, "sample": 0, "beyond_reach_manual": 0,
                "unit_source_manual": 0, "unit_too_few": 0, "unit_differs": 0, "unit_covered_ceiling": 0}
        ustate = {}
        # re-collect stems per unit under the cap with real dbh values
        per = {}
        for t in st:
            if t[0] == "UNIT" and t[3] <= cap:
                per.setdefault((t[1], t[2]), []).append(t[4])
        for k in sorted(units):
            u = units[k]
            ds = per.get(k, [])
            m = metrics_for(u["area_in"], ds)
            if k == src_key:
                ustate[k] = ("SourceManual", m)
            elif m["stems"] < MIN_STEMS:
                ustate[k] = ("NotCovered/TooFewStems", m)
            else:
                ok, dd, db = compare(src, m)
                ustate[k] = ("CoveredCeiling" if ok else "NotCovered/Differs", m)
        for t in st:
            if t[0] == "COR": cats["corridor"] += 1
            elif t[0] == "REM": cats["remainder_manual"] += 1
            elif t[0] == "MASK": cats["masked"] += 1
            elif t[0] == "SAMPLE": cats["sample"] += 1
            else:
                if t[3] > cap: cats["beyond_reach_manual"] += 1
                else:
                    s = ustate[(t[1], t[2])][0]
                    cats[{"SourceManual": "unit_source_manual", "NotCovered/TooFewStems": "unit_too_few",
                          "NotCovered/Differs": "unit_differs", "CoveredCeiling": "unit_covered_ceiling"}[s]] += 1
        treatable = len(st) - cats["corridor"] - cats["masked"] - cats["sample"]
        non_source = [k for k in ustate if k != src_key]
        cov_units = [k for k in non_source if ustate[k][0] == "CoveredCeiling"]
        cov_area = sum(ustate[k][1]["area"] for k in cov_units)
        treat_area = sum(u["area_all"] for k, u in units.items()) + 0.0
        # area of treatable ground = unit ground (all) + remainder ground that is neither masked nor corridor nor sample
        rem_area = sum(1 for c in cells if c[0] == "REM") * cell_area
        denom_area = treat_area + rem_area
        out["cap%g" % cap] = {
            "source": {"stems": src["stems"], "net_area_m2": src["area"], "density_per_ha": src["dens"], "mean_dbh_cm": src["dbh"]},
            "units_total": len(units), "units_non_source": len(non_source), "units_covered_ceiling": len(cov_units),
            "units_covered_share_ceiling": len(cov_units) / len(non_source) if non_source else 0.0,
            "stem_categories": cats, "treatable_stems": treatable,
            "stems_covered_ceiling": cats["unit_covered_ceiling"],
            "stems_covered_share_ceiling": cats["unit_covered_ceiling"] / treatable if treatable else 0.0,
            "stems_manual_or_notcovered": treatable - cats["unit_covered_ceiling"],
            "area_covered_ceiling_m2": cov_area, "area_treatable_m2": denom_area,
            "area_covered_share_ceiling": cov_area / denom_area if denom_area else 0.0,
            "unit_states": {"R%d%s" % (k[0], SEGN[k[1]]): [ustate[k][0], ustate[k][1]["stems"], round(ustate[k][1]["area"], 3)] for k in sorted(ustate)},
        }
    res["policy"] = out
    res["cells"] = cells
    res["stem_labels"] = st
    return res


# ---- layouts -------------------------------------------------------------------------------------------------------------
def clip_line(px, pz, dx, dz, xmin, xmax, zmin, zmax):
    """Chord of the infinite line through (px,pz) direction (dx,dz) inside the window (slab method). Returns (t0,t1) or None."""
    t0, t1 = -1e9, 1e9
    for p, d, lo, hi in ((px, dx, xmin, xmax), (pz, dz, zmin, zmax)):
        if abs(d) < 1e-12:
            if p < lo or p > hi: return None
        else:
            a, b = (lo - p) / d, (hi - p) / d
            if a > b: a, b = b, a
            t0, t1 = max(t0, a), min(t1, b)
    return (t0, t1) if t1 - t0 > 1e-9 else None


def build_cs(theta_deg, spine_s, spine_n, name):
    """C-S(theta): ONE parallel family (bearing theta from north), five FULL CHORDS of the window, whose offsets are equally spaced across
    the offset range the spine spans (maximum-spread: the best any single family can do while each line still crosses the spine).
    MY CONSTRUCTION (assumption): 'one junction per line on the spine, spread over its whole z-extent' is the #251 SS3C.3 bound applied."""
    th = math.radians(theta_deg)
    dx, dz = math.sin(th), math.cos(th)
    nx, nz = math.cos(th), -math.sin(th)
    os_, on_ = spine_s[0] * nx + spine_s[1] * nz, spine_n[0] * nx + spine_n[1] * nz
    racks, info = [], []
    for k in range(5):
        o = os_ + (on_ - os_) * k / 4.0
        px, pz = o * nx, o * nz
        ch = clip_line(px, pz, dx, dz, HEADLAND_X, WIN_E, WIN_S, WIN_N)
        if ch is None:
            info.append({"k": k, "offset": o, "chord": None}); continue
        t0, t1 = ch
        e = (px + dx * t0, pz + dz * t0); f = (px + dx * t1, pz + dz * t1)
        info.append({"k": k, "offset": o, "len": (t1 - t0), "entrance": e, "far": f,
                     "entrance_on_headland": abs(e[0] - HEADLAND_X) <= 0.01})
        racks.append(Rack(k + 1, e, f))
    gap = abs(on_ - os_) / 4.0
    return racks, info, gap


def build_ch(theta_deg, name):
    """C-H(theta): ONE parallel family whose entrances are ON THE HEADLAND (legal under RackEditing.EntranceOnHeadland today), bearing theta,
    entrance z equally spaced from the south window to the last z that still gives a >=5 m rack (RackEditing.MinLengthM). Max-spread; MY CONSTRUCTION."""
    th = math.radians(theta_deg)
    dx, dz = math.sin(th), math.cos(th)

    def chord_len(z0):
        ch = clip_line(HEADLAND_X, z0, dx, dz, HEADLAND_X, WIN_E, WIN_S, WIN_N)
        return 0.0 if ch is None else ch[1] - max(ch[0], 0.0)
    lo, hi = WIN_S, WIN_N
    # largest z0 with chord >= MIN_RACK_LEN (chord length decreases with z0 for a north-east bearing)
    for _ in range(80):
        mid = (lo + hi) / 2
        if chord_len(mid) >= MIN_RACK_LEN: lo = mid
        else: hi = mid
    zmax = lo
    racks, info = [], []
    for k in range(5):
        z0 = WIN_S + (zmax - WIN_S) * k / 4.0
        ch = clip_line(HEADLAND_X, z0, dx, dz, HEADLAND_X, WIN_E, WIN_S, WIN_N)
        t1 = ch[1]
        racks.append(Rack(k + 1, (HEADLAND_X, z0), (HEADLAND_X + dx * t1, z0 + dz * t1)))
        info.append({"k": k, "entrance_z": z0, "len": t1})
    gap = (zmax - WIN_S) / 4.0 * math.cos(th)   # perpendicular gap for a north-east bearing between headland entrances
    return racks, info, gap


def build_cj(theta_deg, des):
    """C-J(theta): the LEAST-INVENTED fallback C. Uses the reference's own five junction points and its own east/west sides (raw design_candidate);
    the only change is direction: every rack gets the SAME bearing theta (east-side racks run +dir, west-side racks run -dir), ends clipped to the
    same windows. At 45 deg the east racks (already 45 deg NE) are unchanged; the three west racks are mirrored from NW to SW. No junction is moved."""
    th = math.radians(theta_deg)
    dx, dz = math.sin(th), math.cos(th)
    racks, info = [], []
    for r in des["racks"]:
        sgn = 1.0 if r["side"] == "E" else -1.0
        jx, jz = r["junction"]["x"], r["junction"]["z"]
        ch = clip_line(jx, jz, dx * sgn, dz * sgn, HEADLAND_X, WIN_E, WIN_S, WIN_N)
        t1 = ch[1]
        racks.append(Rack(int(r["id"][1:]), (jx, jz), (jx + dx * sgn * t1, jz + dz * sgn * t1)))
        info.append({"id": r["id"], "side": r["side"], "len": t1})
    return racks, info


def perp_gaps(racks):
    g = derive_group(racks)
    if not g["valid"]: return None
    o = [d["off"] for d in g["ordered"]]
    return [o[i + 1] - o[i] for i in range(len(o) - 1)]


def min_centreline_dist(racks):
    best = 1e9
    for i in range(len(racks)):
        for j in range(i + 1, len(racks)):
            a, b = racks[i], racks[j]
            for _ in (0,):
                # segment-segment distance by dense sampling of endpoints against segments (parallel racks -> exact at endpoints)
                for (px, pz) in ((a.ex, a.ez), (a.fx, a.fz)):
                    best = min(best, b.seg_dist(px, pz))
                for (px, pz) in ((b.ex, b.ez), (b.fx, b.fz)):
                    best = min(best, a.seg_dist(px, pz))
    return best


def main(outdir):
    os.makedirs(outdir, exist_ok=True)
    raw_ok = {}
    for p, h in RAW_SHA.items():
        raw_ok[os.path.basename(p)] = (sha(p) == h)
    log("RAW FIXTURE SHA256 matches those recorded in #251 App.A:", json.dumps(raw_ok))
    assert all(raw_ok.values()), "raw fixture hash mismatch"

    trees = [t for t in json.load(open(P_TREES))["trees"] if t["living"] and not t["stump"]]
    log("living stems:", len(trees), "| outside +-40 m:", sum(1 for t in trees if abs(t["x"]) > HALF or abs(t["z"]) > HALF))
    mf = json.load(open(P_MFM))
    masks = [(m["ax"], m["az"], m["bx"], m["bz"], m["r"]) for m in mf["baselineMaskList"]]
    log("baseline masks (production, fresh Enlarged80 world, existing Dirt Path/Clearing/Site/Headland):", len(masks))
    des = json.load(open(P_DES))
    cand = [Rack(int(r["id"][1:]), (r["junction"]["x"], r["junction"]["z"]), (r["far"]["x"], r["far"]["z"])) for r in des["racks"]]
    sg = json.load(open(P_SUGG))["racks"]
    sugg = [Rack(i + 1, (r["entranceX"], r["entranceZ"]), (r["farX"], r["farZ"])) for i, r in enumerate(sg)]
    sp = des["spine"]
    spine_s = (sp["existing_polyline"][0][0], sp["existing_polyline"][0][1])
    spine_n = (sp["continuation_end"]["x"], sp["continuation_end"]["z"])
    log("spine ends (design_candidate): south", spine_s, "north", spine_n, "z-extent", round(spine_n[1] - spine_s[1], 6))
    pts = lattice()
    log("lattice cells:", len(pts), "cell", CELL, "m; total area", len(pts) * CELL * CELL)

    R = {}
    # --- legacy validity of each candidate arrangement through the UNCHANGED single-family Derive -------------------
    leg_cand = derive_group(cand)
    log("V0 legacy Derive on the 3+2 reference (all five racks as one family):", "VALID" if leg_cand["valid"] else "INVALID: " + leg_cand["problem"])
    R["legacy_derive_on_reference"] = {"valid": leg_cand["valid"], "problem": leg_cand.get("problem")}
    # Option A grouping: undirected direction, 4 deg link tolerance (ASSUMPTION; irrelevant here: racks are exactly 45/135 deg)
    angs = {}
    for r in cand:
        a = math.degrees(math.atan2(r.fz - r.ez, r.fx - r.ex)) % 180.0
        angs[r.id] = a
    log("rack undirected angles (deg):", {k: round(v, 6) for k, v in angs.items()})
    groups = []       # greedy single-link on the undirected angle, 4 deg link (ASSUMPTION; irrelevant for exactly-45/135 racks)
    for rid, a in sorted(angs.items()):
        for g in groups:
            if min(abs(a - g[0]), 180 - abs(a - g[0])) <= 4.0:
                g[1].append(rid); break
        else:
            groups.append([a, [rid]])
    gl = [sorted(g[1]) for g in groups]
    log("Option A groups:", gl)

    layouts = {}
    layouts["LEGACY_PARALLEL_E-W (suggested layout, headland entrances)"] = Layout("legacy", sugg, [[r.id for r in sugg]], masks)
    layouts["A: 3+2 herringbone 45deg (reference candidate), two families"] = Layout("A", cand, gl, masks)
    cinfo = {}
    for th in (45.0, 55.68, 60.0, 70.0):
        racks, info, gap = build_cs(th, spine_s, spine_n, "CS")
        cinfo["C-S(%g)" % th] = {"gap_m": gap, "racks": info,
                                 "entrances_on_headland": sum(1 for i in info if i.get("entrance_on_headland")),
                                 "min_centreline_dist_m": min_centreline_dist(racks) if len(racks) == 5 else None,
                                 "chords": len(racks)}
        layouts["C-S %g deg: spine-crossing parallel family (max spread)" % th] = Layout("CS%g" % th, racks, [[r.id for r in racks]], masks)
    racks, info = build_cj(45.0, des)
    cinfo["C-J(45)"] = {"racks": info, "gap_m": None, "entrances_on_headland": 0, "chords": 5, "perp_gaps": perp_gaps(racks),
                        "min_centreline_dist_m": min_centreline_dist(racks)}
    layouts["C-J 45 deg: reference junctions+sides kept, ONE parallel family (least invented)"] = Layout("CJ45", racks, [[r.id for r in racks]], masks)
    racks, info, gap = build_ch(45.0, "CH")
    cinfo["C-H(45)"] = {"gap_m": gap, "racks": info, "entrances_on_headland": 5, "min_centreline_dist_m": min_centreline_dist(racks), "chords": 5,
                        "perp_gaps": perp_gaps(racks)}
    layouts["C-H 45 deg: headland-entrance parallel family (max spread, legal entrances)"] = Layout("CH45", racks, [[r.id for r in racks]], masks)
    R["c_construction"] = cinfo
    for k, v in cinfo.items():
        log("C construction", k, "| gap %s m | racks %d | entrances on headland %s | min centreline dist %s | perp gaps %s" % (
            None if v["gap_m"] is None else round(v["gap_m"], 3), v["chords"], v["entrances_on_headland"], None if v["min_centreline_dist_m"] is None else round(v["min_centreline_dist_m"], 3), v.get("perp_gaps")))

    R["layouts"] = {}
    full = {}
    for lname, lay in layouts.items():
        if lay.problem:
            log("LAYOUT INVALID", lname, lay.problem)
            R["layouts"][lname] = {"valid": False, "problem": lay.problem}
            continue
        entry = {"valid": True, "teach_rack": lay.teach.id, "anchor": [round(lay.anchor[0], 6), round(lay.anchor[1], 6)],
                 "anchor_unclamped_along": round(lay.anchor[2], 6), "anchor_along": round(lay.anchor[3], 6), "groups": [g["ids"] for g in lay.groups],
                 "rack_lengths": {r.id: round(r.L, 3) for r in lay.racks}}
        for rule in ("lateral", "nearest"):
            res = run_option(lay, rule, trees, pts, CAPS)
            entry[rule] = {"geometry": res["geometry"], "policy": res["policy"]}
            full[(lname, rule)] = res
        # disputed ground: area claimed by BOTH groups' strips; and lateral-vs-nearest owner disagreement
        dbl = 0; dbl_w = {}; disagree = 0
        gcl, _, ca = evaluate(lay, "lateral", [], pts, use_masks=False)
        gcn, _, _ = evaluate(lay, "nearest", [], pts, use_masks=False)
        for (x, z), cl, cn in zip(pts, gcl, gcn):
            if cl[0] == "COR": continue
            if len(lay.strip_claims(x, z)) >= 2:
                dbl += 1
                dbl_w["lateral:grp%d" % lay.rack_group[cl[1]]] = dbl_w.get("lateral:grp%d" % lay.rack_group[cl[1]], 0) + 1
                dbl_w["nearest:grp%d" % lay.rack_group[cn[1]]] = dbl_w.get("nearest:grp%d" % lay.rack_group[cn[1]], 0) + 1
            if cl[0] == "REM" or cn[1] != cl[1]:
                disagree += 1
        entry["disputed"] = {"double_claim_m2": dbl * ca, "double_claim_owner_split_m2": {k: v * ca for k, v in dbl_w.items()},
                             "lateral_vs_nearest_disagree_or_remainder_m2": disagree * ca}
        # stems whose owning rack differs between rules
        sl = full[(lname, "lateral")]["stem_labels"]; sn = full[(lname, "nearest")]["stem_labels"]
        entry["disputed"]["stems_owner_differs_lateral_vs_nearest"] = sum(1 for a, b in zip(sl, sn) if a[0] in ("UNIT", "REM") and (a[1] != b[1] or a[2] != b[2] or a[0] != b[0]))
        R["layouts"][lname] = entry

    # --- pairwise: stems whose category/owner differs between A and legacy-like C ---------------------------------
    ka = "A: 3+2 herringbone 45deg (reference candidate), two families"
    kl = "LEGACY_PARALLEL_E-W (suggested layout, headland entrances)"
    for kc in [k for k in layouts if k.startswith("C-")]:
        for rule in ("lateral", "nearest"):
            if layouts[kc].problem: continue
            a = full[(ka, rule)]["stem_labels"]; c = full[(kc, rule)]["stem_labels"]
            diff = sum(1 for x, y in zip(a, c) if (x[0], x[1], x[2]) != (y[0], y[1], y[2]))
            R["layouts"][kc].setdefault("stems_label_differs_vs_A", {})[rule] = diff

    # --- feasibility arithmetic for the 45-degree question ---------------------------------------------------------
    z_ext = spine_n[1] - spine_s[1]
    feas = {"spine_z_extent_m": z_ext}
    for g in (EDITOR_CLEARANCE, 8.0, 12.0, 13.70, 16.0, 20.0):
        feas["max_parallel_racks_at_45deg_gap_%g" % g] = int(math.floor(z_ext * math.sin(math.radians(45)) / g + 1e-9)) + 1
    feas["spine_length_needed_45deg_gap16_m"] = 64 / math.sin(math.radians(45))
    feas["theta_floor_deg_gap16"] = math.degrees(math.asin(64 / z_ext))
    feas["max_equal_gap_at_45deg_m"] = z_ext * math.sin(math.radians(45)) / 4
    feas["min_theta_deg_for_gap_%g" % EDITOR_CLEARANCE] = math.degrees(math.asin(4 * EDITOR_CLEARANCE / z_ext))
    R["feasibility_45deg"] = feas
    log("FEAS", json.dumps(feas))

    # --- validation V1: re-derive #253/#251 headline numbers on A with this independent code ---------------------
    A = R["layouts"][ka]
    v1 = {
        "corridor_union_m2": A["lateral"]["geometry"]["corridor_union_m2"],
        "non_corridor_m2": A["lateral"]["geometry"]["non_corridor_m2"],
        "double_claim_m2": A["disputed"]["double_claim_m2"],
        "lateral_owned_m2": A["lateral"]["geometry"]["owned_m2"],
        "lateral_remainder_m2": A["lateral"]["geometry"]["remainder_m2"],
        "lateral_max_owner_dist_m": A["lateral"]["geometry"]["max_owner_dist_m"],
        "nearest_beyond_9_m2": A["nearest"]["geometry"]["cap9"]["beyond_owner_m2"],
        "nearest_beyond_12_m2": A["nearest"]["geometry"]["cap12"]["beyond_owner_m2"],
        "nearest_beyond_20_m2": A["nearest"]["geometry"]["cap20"]["beyond_owner_m2"],
        "nearest_max_owner_dist_m": A["nearest"]["geometry"]["max_owner_dist_m"],
    }
    R["V1_rederived_vs_published"] = v1
    pub = {"corridor_union_m2": 524.25, "non_corridor_m2": 5875.75, "double_claim_m2": 1575.0, "lateral_owned_m2": 5461.25,
           "lateral_remainder_m2": 414.5, "nearest_beyond_9_m2": 2246.0, "nearest_beyond_12_m2": 1718.25, "nearest_beyond_20_m2": 830.0}
    for k, p in pub.items():
        log("V1 %-26s mine=%10.4f published(#251/#253)=%10.4f  %s" % (k, v1[k], p, "MATCH" if abs(v1[k] - p) < 1e-9 else "DIFF"))
    log("V1 lateral_max_owner_dist_m mine=%.4f published=69.17 ; nearest_max mine=%.4f published=41.18" % (v1["lateral_max_owner_dist_m"], v1["nearest_max_owner_dist_m"]))

    # --- validation V2: legacy port vs recorded C# production run (baseline of mfm_candidate.json) --------------------
    base = json.load(open(P_MFMC))["baseline"]
    L = full[(kl, "lateral")]
    cells, st = L["cells"], L["stem_labels"]
    mine = {}
    for t in st:
        if t[0] == "UNIT":
            mine["R%d%s" % (t[1], SEGN[t[2]])] = mine.get("R%d%s" % (t[1], SEGN[t[2]]), 0) + 1
    prod = {a["id"]: a["stems"] for a in base["areas"]}
    rows, allmatch = [], True
    for k in sorted(prod):
        mm = mine.get(k, 0)
        rows.append((k, prod[k], mm)); allmatch &= (prod[k] == mm)
    log("V2 legacy-port per-area stems vs recorded C# (production) baseline [area, C#, port]:", rows)
    src_mine = sum(1 for t in st if t[0] == "SAMPLE")
    log("V2 sample stems port=%d C#=%d | all 15 areas match: %s | sample net area lattice=%.3f C#(interval scan)=%.3f" % (
        src_mine, base["sourceStems"], allmatch, sum(1 for c in cells if c[0] == "SAMPLE") * CELL * CELL, base["sourceNetAreaM2"]))
    R["V2_legacy_port_vs_cs_run"] = {"rows": rows, "all_area_stems_match": allmatch, "sample_stems_port": src_mine, "sample_stems_cs": base["sourceStems"],
                                      "cs_covered": base["coveredCount"], "cs_nonsource": base["nonSourceCount"],
                                      "port_unit_states_cap_inf_not_run": "see policy tables; port covered-ceiling at cap 20 differs by design"}
    # port's own Covered at effectively unlimited reach, for comparison with C# 12/14 (ceiling excludes competition so port >= C#)
    big = run_option(layouts[kl], "lateral", trees, pts, (1e9,))["policy"]["cap1e+09"]
    log("V2 port covered-ceiling (no reach cap, density+DBH only): %d/%d ; C# covered (incl. competition): %d/%d" % (
        big["units_covered_ceiling"], big["units_non_source"], base["coveredCount"], base["nonSourceCount"]))
    R["V2_legacy_port_vs_cs_run"]["port_covered_no_cap"] = [big["units_covered_ceiling"], big["units_non_source"]]
    R["V2_treatable"] = {"port_treatable_stems_no_cap": big["treatable_stems"]}

    # --- lattice bias spot check (0.25 m) on the A corridor union and non-corridor ground -----------------------------
    n2 = int(round(2 * HALF / 0.25))
    fine = [(-HALF + 0.25 * (i + 0.5), -HALF + 0.25 * (j + 0.5)) for i in range(n2) for j in range(n2)]
    la = layouts[ka]
    cor = sum(1 for (x, z) in fine if la.in_corridor(x, z)) * 0.0625
    R["lattice_check_0.25m"] = {"corridor_union_m2_A": cor, "corridor_union_m2_0.5m": v1["corridor_union_m2"]}
    log("lattice 0.25 m corridor union (A):", cor, "vs 0.5 m:", v1["corridor_union_m2"])

    # --- compact headline table -----------------------------------------------------------------------------------
    log("")
    log("=== MATCHED-POLICY TABLE (lattice 0.5 m; reach = distance to OWNING rack finite centreline; uncovered = 1 - within-reach share of non-corridor ground) ===")
    hdr = "%-78s %-8s" % ("layout", "owner") + "".join("  unc@%-4g" % c for c in CAPS)
    log(hdr)
    rows_out = []
    for lname in layouts:
        e = R["layouts"][lname]
        if not e["valid"]:
            log("%-78s INVALID: %s" % (lname[:78], e["problem"])); continue
        for rule in ("lateral", "nearest"):
            u = [e[rule]["geometry"]["cap%g" % c]["uncovered_share"] for c in CAPS]
            log("%-78s %-8s" % (lname[:78], rule) + "".join("  %6.2f%%" % (100 * x) for x in u))
    log("")
    log("=== UNIT/STEM POLICY (masks + sample precedence; CEILING: density+DBH only, competition not computed) ===")
    for lname in layouts:
        e = R["layouts"][lname]
        if not e["valid"]: continue
        for rule in ("lateral", "nearest"):
            for c in CAPS:
                p = e[rule]["policy"]["cap%g" % c]
                log("%-60s %-8s cap%-3g units %2d/%2d  stems covered<= %4d / treatable %4d (%5.1f%%)  manual/notcov stems %4d  cats %s" % (
                    lname[:60], rule, c, p["units_covered_ceiling"], p["units_non_source"], p["stems_covered_ceiling"], p["treatable_stems"],
                    100 * p["stems_covered_share_ceiling"], p["stems_manual_or_notcovered"], json.dumps(p["stem_categories"])))
    for lname in layouts:
        e = R["layouts"][lname]
        if e["valid"]:
            log("DISPUTED", lname[:60], json.dumps(e["disputed"]), "| differs-vs-A", json.dumps(e.get("stems_label_differs_vs_A")))

    R["meta"] = {"constants": {"HALF": HALF, "CELL": CELL, "CAPS": CAPS, "SEG_HALF": SEG_HALF, "DENS_TOL": DENS_TOL, "DBH_TOL": DBH_TOL,
                               "MIN_STEMS": MIN_STEMS, "TEACH_HALF_LEN": TEACH_HALF_LEN, "TEACH_HALF_W": TEACH_HALF_W, "GAP_ASSUMED": GAP_ASSUMED,
                               "EDITOR_CLEARANCE": EDITOR_CLEARANCE, "WIN": [HEADLAND_X, WIN_E, WIN_S, WIN_N]},
                 "raw_sha256": RAW_SHA, "script": "mfm_a_vs_c_offline.py"}
    for lname in R["layouts"]:
        pass
    # strip bulky per-cell data (not stored); results contain only aggregates
    with open(os.path.join(outdir, "results.json"), "w") as f:
        json.dump(R, f, indent=1, sort_keys=True, default=str)
    with open(os.path.join(outdir, "run.log"), "w") as f:
        f.write("\n".join(LOG) + "\n")


if __name__ == "__main__":
    main(sys.argv[1] if len(sys.argv) > 1 else "out")
