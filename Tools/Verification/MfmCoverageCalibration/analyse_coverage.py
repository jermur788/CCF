#!/usr/bin/env python3
"""MFM-03 coverage calibration analysis (measurement only; Python standard library only).

Reads the exporter's per-tree JSON (production geometry + CompetitionShare values for the MFM-02 V2 fixture) and the frozen
infrastructure inputs, then measures every rack-served coverage unit and writes results.json + tables.md to the output directory.

Nothing here is a prescription, a coverage decision used by the game, or a production constant. Every tolerance tested is a
GAMEPLAY CALIBRATION candidate. Candidate-spacing "player judgements" are a deterministic workload simulation keyed on a hash of
the tree ID, NOT a quality model; no DBH, competition or dominance value takes part in candidate ordering or rejection.

Usage: analyse_coverage.py <export.json> <out-dir> <repo-root>
"""
import hashlib
import itertools
import json
import math
import os
import re
import statistics
import sys

HERE = os.path.dirname(os.path.abspath(__file__))
STAND = 40.0
FORMER_EDGE = 19.4          # Legacy40 clamp line preserved inside Enlarged80 (Enlarged80Handoff.md; Unity Project Overview)
HEGYI_REACH = 8.0           # edge-proximity band = the competition neighbourhood truncated by the stand boundary
SAMPLE_CROP_TREES = ["P0810", "P1206", "P1214"]   # MFM-02 live chain Crop Trees (Unity evidence mfm02-v2-measurements.txt)
SAMPLE_RELEASE_BA_SHARE = 0.17898431420326233      # MFM-02 live initial release BA share (same evidence); sample-derived ceiling
SAFEGUARD = 0.40
RELEASE_TARGET = 0.30
STEP = 0.1                  # area integration step (m); convergence is reported

# ----------------------------------------------------------------------------------------------------------- inputs / masks

def headland_constants(root):
    text = open(os.path.join(root, "Assets", "ForestPrototype", "ScenarioOne", "ScenarioOneHeadlandGeometry.cs")).read()
    def c(name):
        m = re.search(r"const float %s = (-?[0-9.]+)f;" % name, text)
        if not m:
            raise SystemExit("headland constant %s not found" % name)
        return float(m.group(1))
    return {k: c(k) for k in ("HeadlandX", "SouthZ", "NorthZ", "FeederZ", "PathX", "Width", "PadRadius", "StemGuard")}


class Masks:
    """Infrastructure no-treatment footprints: headland/feeder (production constants, stem-centre guard), forest track planks and
    the south approach extension (generator path corridor), work clearing (visible art radius, which is wider than the generator's
    no-tree radius), player start and built structures (generator clearing radius). Rack corridors are handled by production
    placement, not here."""

    def __init__(self, infra, head):
        r_path = infra["pathCorridorRadiusM"]
        r_site = infra["clearingRadiusM"]
        self.circles = {"track": [], "clearing": [], "sites": []}
        for p in infra["scenePlanks"]:
            self.circles["track"].append((p["x"], p["z"], r_path))
        for p in infra["approachExtension"]:
            if -STAND - r_path <= p["z"] <= STAND + r_path:
                self.circles["track"].append((p["x"], p["z"], r_path))
        cl = infra["clearing"]
        self.circles["clearing"].append((cl["x"], cl["z"], cl["visualRadiusM"]))
        self.circles["sites"].append((infra["player"]["x"], infra["player"]["z"], r_site))
        for b in infra["buildables"]:
            self.circles["sites"].append((b["x"], b["z"], r_site))
        strip = head["Width"] * 0.5 + head["StemGuard"]
        pad = head["PadRadius"] + head["StemGuard"]
        hx, fz, px = head["HeadlandX"], head["FeederZ"], head["PathX"]
        self.capsules = {"headland": [((hx, head["SouthZ"]), (hx, head["NorthZ"]), strip), ((hx, fz), (px, fz), strip)]}
        self.circles["headland"] = [(hx, fz, pad), (px, fz, pad)]

    @staticmethod
    def _seg_d2(x, z, a, b):
        ax, az = a; bx, bz = b
        dx, dz = bx - ax, bz - az
        L = dx * dx + dz * dz
        t = 0.0 if L <= 0 else max(0.0, min(1.0, ((x - ax) * dx + (z - az) * dz) / L))
        ex, ez = x - (ax + t * dx), z - (az + t * dz)
        return ex * ex + ez * ez

    def kind(self, x, z):
        for k in ("headland", "track", "clearing", "sites"):
            for cx, cz, r in self.circles.get(k, []):
                if (x - cx) ** 2 + (z - cz) ** 2 <= r * r:
                    return k
            for a, b, r in self.capsules.get(k, []):
                if self._seg_d2(x, z, a, b) <= r * r:
                    return k
        return None

    def column_cuts(self, x, z0, z1, kinds=None):
        cuts = []
        for k, items in self.circles.items():
            if kinds and k not in kinds:
                continue
            for cx, cz, r in items:
                if abs(x - cx) < r:
                    h = math.sqrt(r * r - (x - cx) ** 2)
                    cuts.append((cz - h, cz + h))
        for k, items in self.capsules.items():
            if kinds and k not in kinds:
                continue
            for (ax, az), (bx, bz), r in items:
                if ax == bx:     # vertical capsule
                    if abs(x - ax) < r:
                        h = math.sqrt(r * r - (x - ax) ** 2)
                        cuts.append((min(az, bz) - h, max(az, bz) + h))
                else:            # horizontal capsule (feeder)
                    lo, hi = min(ax, bx), max(ax, bx)
                    if lo <= x <= hi:
                        cuts.append((az - r, az + r))
                    else:
                        ex = lo if x < lo else hi
                        if abs(x - ex) < r:
                            h = math.sqrt(r * r - (x - ex) ** 2)
                            cuts.append((az - h, az + h))
        return [(max(z0, a), min(z1, b)) for a, b in cuts if b > z0 and a < z1]


def union_length(cuts, z0, z1):
    used, end = 0.0, z0
    for a, b in sorted(cuts):
        if b <= end:
            continue
        used += b - max(a, end)
        end = b
    return used


# ------------------------------------------------------------------------------------------------------------ geometry helpers

class World:
    def __init__(self, export, masks):
        self.e = export
        self.masks = masks
        self.racks = export["racks"]
        self.rack_z = [r["entranceZ"] for r in self.racks]
        self.rack_x = (self.racks[0]["entranceX"], self.racks[0]["farX"])
        self.half_w = self.racks[0]["width"] * 0.5
        fp = export["footprint"]
        self.fp_rect = (fp["anchorX"] - fp["halfLength"], fp["anchorX"] + fp["halfLength"], fp["anchorZ"] - fp["halfWidth"], fp["anchorZ"] + fp["halfWidth"])
        self.trees = export["trees"]
        self.by_id = {t["id"]: t for t in self.trees}
        for t in self.trees:
            t["mask"] = masks.kind(t["x"], t["z"])
            t["legacy"] = not t["id"].startswith("PO")
            t["formerEdge"] = t["legacy"] and (abs(abs(t["x"]) - FORMER_EDGE) < 1e-3 or abs(abs(t["z"]) - FORMER_EDGE) < 1e-3)
            t["edgeDist"] = STAND - max(abs(t["x"]), abs(t["z"]))
            t["ba"] = math.pi * (t["dbh"] / 200.0) ** 2

    def corridor_cuts(self, x, z0, z1):
        if not (self.rack_x[0] - 1e-4 <= x <= self.rack_x[1] + 1e-4):
            return []
        return [(max(z0, z - self.half_w), min(z1, z + self.half_w)) for z in self.rack_z if z + self.half_w > z0 and z - self.half_w < z1]

    def net_area(self, rect, exclude_footprint=True, step=STEP, kinds=None, include_masks=True):
        """Midpoint integration in x; exact union of corridor/mask/footprint intervals in z."""
        x0, x1, z0, z1 = rect
        if x1 <= x0 or z1 <= z0:
            return 0.0
        n = max(1, int(math.ceil((x1 - x0) / step - 1e-9)))
        dx = (x1 - x0) / n
        f0, f1, g0, g1 = self.fp_rect
        area = 0.0
        for k in range(n):
            x = x0 + (k + 0.5) * dx
            cuts = self.corridor_cuts(x, z0, z1)
            if include_masks:
                cuts += self.masks.column_cuts(x, z0, z1, kinds)
            if exclude_footprint and f0 <= x <= f1 and g1 > z0 and g0 < z1:
                cuts.append((max(z0, g0), min(z1, g1)))
            area += (z1 - z0 - union_length(cuts, z0, z1)) * dx
        return area


def rel(a, b):
    return (a - b) / b if b else float("nan")


def quantile(values, q):
    v = sorted(values)
    return v[int(round((len(v) - 1) * q))] if v else 0.0     # rounded-index quantile; production must name/version its choice


def metrics(stems, area, source_p90=None, ci_key="ciPost"):
    n = len(stems)
    ci = [t[ci_key] for t in stems]
    dbh = [t["dbh"] for t in stems]
    out = {
        "stems": n,
        "netAreaM2": area,
        "densityPerHa": n / area * 1e4 if area > 0 else 0.0,
        "meanDbhCm": statistics.mean(dbh) if n else 0.0,
        "qmdCm": math.sqrt(sum(d * d for d in dbh) / n) if n else 0.0,
        "basalAreaM2PerHa": sum(t["ba"] for t in stems) / area * 1e4 if area > 0 else 0.0,
        "meanCi": statistics.mean(ci) if n else 0.0,
        "sdCi": statistics.stdev(ci) if n > 1 else 0.0,
        "sdDbh": statistics.stdev(dbh) if n > 1 else 0.0,
        "p90Ci": quantile(ci, 0.9) if n else 0.0,
        "formerEdgeStems": sum(t["formerEdge"] for t in stems),
        "legacyCoreStems": sum(t["legacy"] for t in stems),
        "within8mOfBoundary": sum(t["edgeDist"] < HEGYI_REACH for t in stems),
        "westOfHeadland": sum(t["x"] < -36.85 for t in stems),
    }
    if source_p90 is not None:
        out["aboveSourceP90"] = sum(c > source_p90 for c in ci) / n if n else 0.0
    return out


# ---------------------------------------------------------------------------------------------------------------- the analysis

def main():
    export_path, out_dir, root = sys.argv[1], sys.argv[2], sys.argv[3]
    export = json.load(open(export_path))
    infra = json.load(open(os.path.join(HERE, "inputs", "fixture-infrastructure.json")))
    regen = json.load(open(os.path.join(HERE, "inputs", "historical-lesson5-regeneration.json")))
    head = headland_constants(root)
    masks = Masks(infra, head)
    W = World(export, masks)
    checks = []

    def check(ok, label, detail=""):
        checks.append({"ok": bool(ok), "label": label, "detail": str(detail)})

    check(export["problems"] == [], "exporter fixture identity / source parity", export["problems"])
    check(abs(export["normal"][0]) < 1e-6 and abs(export["normal"][1] - 1) < 1e-6, "fixture normal is +z (east-west racks); offsets equal z")
    units = export["units"]
    check(len(units) == 5 and all(r["empty"] for r in export["remainders"]), "five units, both explicit remainders empty in the fixture")
    check(abs(sum(u["area"] for u in units) - 6400) < 0.01, "unit areas sum to the 80 x 80 m stand", sum(u["area"] for u in units))

    living = [t for t in W.trees if not t["rackRemoval"]]
    check(len(W.trees) == 1344 and len(living) == 1344 - sum(len(r["removals"]) for r in W.racks), "1344 starting stems; post-access living = 1344 - rack removals")
    # The other two live MFM-02 footprint counts (Unity evidence mfm02-v2-measurements.txt, pre-access world).
    fp_pre = [t for t in W.trees if t["footprint"]]
    check(sum(abs(t["across"]) > 8.00385 for t in fp_pre) == 11, "live: 11 footprint stems between the historical 8.00385 m and the 9.0 m edge")
    check(sum(abs(t["along"]) <= 12 and 9 < abs(t["across"]) <= 10 for t in W.trees) == 8, "live: 8 living stems outside the 9 m band but within 10 m")
    check(sum(abs(t["along"]) <= 12 and abs(t["across"]) <= 9 and not t["footprint"] for t in W.trees) == 10, "live: 10 corridor-excluded stems inside the band")
    rem_stems = [t for t in W.trees if t["place"] == "OuterRemainder"]
    check(len(rem_stems) == 0, "no stem classifies into an outer remainder in the fixture", len(rem_stems))

    # ------------------------------------------------------------------------------------------- source (teaching footprint)
    fp_all = [t for t in living if t["footprint"]]
    fp_masked = [t for t in fp_all if t["mask"]]
    source = [t for t in fp_all if not t["mask"]]
    src_area = W.net_area(W.fp_rect, exclude_footprint=False)
    src_area_gross = W.net_area(W.fp_rect, exclude_footprint=False, include_masks=False)
    src = metrics(source, src_area)
    src_p90 = src["p90Ci"]
    src["aboveSourceP90"] = sum(t["ciPost"] > src_p90 for t in source) / len(source)
    conv = {str(s): W.net_area(W.fp_rect, exclude_footprint=False, step=s) for s in (0.2, 0.1, 0.05)}
    fp_units = {}
    for t in fp_all:
        fp_units[units[t["unit"]]["rackId"]] = fp_units.get(units[t["unit"]]["rackId"], 0) + 1
    crops = [W.by_id[i] for i in SAMPLE_CROP_TREES]
    check(all(c["footprint"] and not c["rackRemoval"] for c in crops), "the MFM-02 live Crop Trees are living footprint stems in the fixture")

    # ----------------------------------------------------------------------------------------------------------- partitions
    def unit_rect(u, x0=-STAND, x1=STAND):
        return (x0, x1, max(-STAND, u["lower"]), min(STAND, u["upper"]))

    def region_stems(u_index, x0, x1):
        return [t for t in living if t["place"] == "CoverageUnit" and t["unit"] == u_index and x0 <= t["x"] < x1 + (1e-9 if x1 >= STAND else 0)
                and not t["footprint"] and not t["mask"]]

    partitions = {
        "whole-strip": [("", -STAND, STAND)],
        "anchor-aligned-thirds": [("W", -STAND, -12.0), ("C", -12.0, 12.0), ("E", 12.0, STAND)],
        "equal-thirds": [("W", -STAND, -STAND / 3), ("C", -STAND / 3, STAND / 3), ("E", STAND / 3, STAND)],
    }
    regions = {}
    for pname, segs in partitions.items():
        rows = []
        for u in units:
            for sname, x0, x1 in segs:
                stems = region_stems(u["index"], x0, x1)
                rect = unit_rect(u, x0, x1)
                area = W.net_area(rect)
                gross = (rect[1] - rect[0]) * (rect[3] - rect[2])
                m = metrics(stems, area, src_p90)
                m.update({
                    "id": "R%d%s" % (u["rackId"], sname), "rackId": u["rackId"], "segment": sname or "all", "grossAreaM2": gross,
                    "corridorAreaM2": gross - W.net_area(rect, exclude_footprint=False, include_masks=False),
                    "infrastructureAreaM2": W.net_area(rect, exclude_footprint=False, include_masks=False) - W.net_area(rect, exclude_footprint=False),
                    "footprintAreaM2": W.net_area(rect, exclude_footprint=False) - area,
                    "rackRemovals": sum(1 for t in W.trees if t["rackRemoval"] and t["unit"] == u["index"] and x0 <= t["x"] < x1 + (1e-9 if x1 >= STAND else 0)),
                    "maskedStems": {k: sum(1 for t in living if t["mask"] == k and t["place"] == "CoverageUnit" and t["unit"] == u["index"] and x0 <= t["x"] < x1 + (1e-9 if x1 >= STAND else 0) and not t["footprint"])
                                    for k in ("headland", "track", "clearing", "sites")},
                    "footprintStems": sum(1 for t in fp_all if t["unit"] == u["index"] and x0 <= t["x"] < x1 + (1e-9 if x1 >= STAND else 0)),
                    "outerEdgeUnit": u["lowerOuter"] or u["upperOuter"],
                    "dDensity": rel(m["densityPerHa"], src["densityPerHa"]),
                    "dDbh": rel(m["meanDbhCm"], src["meanDbhCm"]),
                    "dCi": rel(m["meanCi"], src["meanCi"]),
                    "dBa": rel(m["basalAreaM2PerHa"], src["basalAreaM2PerHa"]),
                    # Standard error of the relative mean differences (two independent samples, normal approximation).
                    "seDCi": math.sqrt((m["sdCi"] ** 2 / max(1, m["stems"])) + (src["sdCi"] ** 2 / src["stems"])) / src["meanCi"] if m["stems"] else float("nan"),
                    "seDDbh": math.sqrt((m["sdDbh"] ** 2 / max(1, m["stems"])) + (src["sdDbh"] ** 2 / src["stems"])) / src["meanDbhCm"] if m["stems"] else float("nan"),
                })
                rows.append(m)
        regions[pname] = rows
        check(sum(r["stems"] for r in rows) + len(fp_all) + sum(sum(r["maskedStems"].values()) for r in rows) == len(living),
              pname + ": every living post-access stem is in exactly one region, the footprint or a mask")
        check(abs(sum(r["grossAreaM2"] for r in rows) - 6400) < 0.01, pname + ": regions tile the stand")

    # -------------------------------------------------------------------------------------- classification rule + sensitivity
    def covered(r, td, tb, tc, g, min_stems=30):
        if r["stems"] < min_stems:
            return False
        if abs(r["dDensity"]) > td or abs(r["dDbh"]) > tb or abs(r["dCi"]) > tc:
            return False
        if g is not None and r["aboveSourceP90"] > src["aboveSourceP90"] + g:
            return False
        return True

    def reasons(r, td, tb, tc, g, min_stems=30):
        out = []
        if r["stems"] < min_stems:
            out.append("too few stems (%d < %d)" % (r["stems"], min_stems))
        if abs(r["dDensity"]) > td:
            out.append("density %+.1f%%" % (100 * r["dDensity"]))
        if abs(r["dDbh"]) > tb:
            out.append("mean DBH %+.1f%%" % (100 * r["dDbh"]))
        if abs(r["dCi"]) > tc:
            out.append("mean CI %+.1f%%" % (100 * r["dCi"]))
        if g is not None and r["aboveSourceP90"] > src["aboveSourceP90"] + g:
            out.append("crowded tail %.1f%% > %.1f%%" % (100 * r["aboveSourceP90"], 100 * (src["aboveSourceP90"] + g)))
        return out

    def margin(r, td, tb, tc, g):
        """Largest normalised use of any tolerance (<=1 fits). Near 1 = boundary unit."""
        parts = [abs(r["dDensity"]) / td, abs(r["dDbh"]) / tb, abs(r["dCi"]) / tc]
        if g is not None:
            parts.append(max(0.0, r["aboveSourceP90"] - src["aboveSourceP90"]) / g)
        return max(parts)

    TD = (0.10, 0.15, 0.20, 0.25, 0.30)
    TB = (0.05, 0.10, 0.15, 0.20)
    TC = (0.10, 0.15, 0.20, 0.25, 0.30)
    TG = (None, 0.05, 0.10, 0.15, 0.20)
    grid = list(itertools.product(TD, TB, TC, TG))
    sensitivity = {}
    for pname, rows in regions.items():
        per_unit = {r["id"]: sum(covered(r, *p) for p in grid) / len(grid) for r in rows}
        sensitivity[pname] = {"coveredFractionAcrossGrid": per_unit}

    # One-at-a-time sweeps around candidate centre points.
    centres = {"recommended-20/15/20-no-guard": (0.20, 0.15, 0.20, None), "with-P90-guard-10pp": (0.20, 0.15, 0.20, 0.10),
               "tight-15/10/15": (0.15, 0.10, 0.15, None)}
    rec = centres["recommended-20/15/20-no-guard"]
    sweeps = {}
    for pname in ("whole-strip", "anchor-aligned-thirds"):
        rows = regions[pname]
        sweeps[pname] = {}
        for cname, (td, tb, tc, g) in centres.items():
            s = {"centre": [td, tb, tc, g], "covered": [r["id"] for r in rows if covered(r, td, tb, tc, g)]}
            s["density"] = {str(v): [r["id"] for r in rows if covered(r, v, tb, tc, g)] for v in TD}
            s["dbh"] = {str(v): [r["id"] for r in rows if covered(r, td, v, tc, g)] for v in TB}
            s["ci"] = {str(v): [r["id"] for r in rows if covered(r, td, tb, v, g)] for v in TC}
            s["guard"] = {str(v): [r["id"] for r in rows if covered(r, td, tb, tc, v)] for v in TG}
            s["minStems"] = {str(v): [r["id"] for r in rows if covered(r, td, tb, tc, g, v)] for v in (10, 20, 30, 40, 60)}
            sweeps[pname][cname] = s

    # --------------------------------------------------------------------------- sample-sized window null distribution
    # The same 24 x 18 m footprint shape placed along every rack: how different is a sample-sized area from the real sample merely
    # because of where it falls? Tolerances tighter than this spread classify position noise, not conditions.
    windows = []
    for ri, rz in enumerate(W.rack_z):
        for c in range(-22, 29, 2):
            rect = (c - 12.0, c + 12.0, rz - 9.0, rz + 9.0)
            if rect[0] < W.rack_x[0] + 0.0 or rect[1] > STAND:
                continue
            stems = [t for t in living if rect[0] <= t["x"] <= rect[1] and rect[2] <= t["z"] <= rect[3] and t["place"] != "RackCorridor" and not t["mask"]]
            area = W.net_area((rect[0], rect[1], max(-STAND, rect[2]), min(STAND, rect[3])), exclude_footprint=False)
            if area < 200 or len(stems) < 20:
                continue
            m = metrics(stems, area, src_p90)
            windows.append({"rackId": W.racks[ri]["id"], "centreX": c, "stems": m["stems"], "dDensity": rel(m["densityPerHa"], src["densityPerHa"]),
                            "dDbh": rel(m["meanDbhCm"], src["meanDbhCm"]), "dCi": rel(m["meanCi"], src["meanCi"]),
                            "aboveSourceP90": m["aboveSourceP90"], "formerEdgeStems": m["formerEdgeStems"], "nearBoundary": m["within8mOfBoundary"],
                            "isSource": ri == 2 and c == 0})
    def frac_within(key, tol, subset):
        return sum(abs(w[key]) <= tol for w in subset) / len(subset) if subset else float("nan")
    win_interior = [w for w in windows if w["formerEdgeStems"] == 0 and w["nearBoundary"] == 0]
    window_summary = {
        "count": len(windows), "interiorCleanCount": len(win_interior),
        "within": {k: {str(t): {"all": frac_within(k, t, windows), "interiorNoFormerEdge": frac_within(k, t, win_interior)} for t in (0.05, 0.10, 0.15, 0.20, 0.25, 0.30)}
                   for k in ("dDensity", "dDbh", "dCi")},
        "range": {k: [min(w[k] for w in windows), max(w[k] for w in windows)] for k in ("dDensity", "dDbh", "dCi")},
        "tailExceedsSourcePlus": {str(g): sum(w["aboveSourceP90"] > src["aboveSourceP90"] + g for w in windows) / len(windows) for g in (0.05, 0.10, 0.15, 0.20)},
        "interiorRange": {k: [min(w[k] for w in win_interior), max(w[k] for w in win_interior)] for k in ("dDensity", "dDbh", "dCi")} if win_interior else {},
    }

    # ------------------------------------------------------------------------------------------------------ former-edge band
    band = [t for t in living if t["formerEdge"]]
    band_all = [t for t in W.trees if t["formerEdge"]]
    core_inner = [t for t in living if t["legacy"] and not t["formerEdge"] and max(abs(t["x"]), abs(t["z"])) < FORMER_EDGE - 2.0 and not t["mask"]]
    outer_near = [t for t in living if not t["legacy"] and FORMER_EDGE + 2.0 < max(abs(t["x"]), abs(t["z"])) < 30 and not t["mask"]]
    ring = [t for t in living if not t["formerEdge"] and FORMER_EDGE - 2.0 <= max(abs(t["x"]), abs(t["z"])) <= FORMER_EDGE + 2.0 and not t["mask"]]
    def nn(t, pool):
        return min(math.hypot(t["x"] - o["x"], t["z"] - o["z"]) for o in pool if o is not t)
    live_pool = living
    band_effect = {
        "formerEdgeStemsStart": len(band_all), "formerEdgeStemsAfterAccess": len(band),
        "removedByRacks": sum(t["rackRemoval"] for t in band_all),
        "meanCi": {"formerEdge": statistics.mean(t["ciPost"] for t in band), "ringNeighboursWithin2m": statistics.mean(t["ciPost"] for t in ring),
                   "coreInterior": statistics.mean(t["ciPost"] for t in core_inner), "outerPlantation": statistics.mean(t["ciPost"] for t in outer_near)},
        "meanDbh": {"formerEdge": statistics.mean(t["dbh"] for t in band), "coreInterior": statistics.mean(t["dbh"] for t in core_inner),
                    "outerPlantation": statistics.mean(t["dbh"] for t in outer_near)},
        "meanNearestNeighbourM": {"formerEdge": statistics.mean(nn(t, live_pool) for t in band), "coreInterior": statistics.mean(nn(t, live_pool) for t in core_inner),
                                  "outerPlantation": statistics.mean(nn(t, live_pool) for t in outer_near)},
        "fractionAboveSourceP90": {"formerEdge": sum(t["ciPost"] > src_p90 for t in band) / len(band),
                                   "coreInterior": sum(t["ciPost"] > src_p90 for t in core_inner) / len(core_inner),
                                   "outerPlantation": sum(t["ciPost"] > src_p90 for t in outer_near) / len(outer_near)},
    }
    # Band density: stems within +-1 m of the clamp lines versus the same-width strips 3-5 m inside and outside them.
    def band_density(lo, hi):
        stems = [t for t in living if not t["mask"] and lo <= max(abs(t["x"]), abs(t["z"])) < hi]
        area = 0.0
        # ring area between square half-extents lo and hi, minus corridors/masks: integrate via net_area on the four sides
        for rect in ((-hi, hi, lo, hi), (-hi, hi, -hi, -lo), (-hi, -lo, -lo, lo), (lo, hi, -lo, lo)):
            area += W.net_area(rect, exclude_footprint=False)
        return len(stems) / area * 1e4 if area else 0.0, len(stems), area
    band_effect["ringDensityPerHa"] = {"band(18.4-20.4)": band_density(18.4, 20.4), "inside(15.4-17.4)": band_density(15.4, 17.4), "outside(21.4-23.4)": band_density(21.4, 23.4)}
    # Per-unit effect of the band on the unit means (with vs without band stems).
    band_units = []
    for r in regions["anchor-aligned-thirds"] + regions["whole-strip"]:
        u = next(u for u in units if u["rackId"] == r["rackId"])
        x0, x1 = {"W": (-STAND, -12.0), "C": (-12.0, 12.0), "E": (12.0, STAND), "all": (-STAND, STAND)}[r["segment"]]
        stems = region_stems(u["index"], x0, x1)
        if not any(t["formerEdge"] for t in stems):
            continue
        no_band = [t for t in stems if not t["formerEdge"]]
        band_units.append({"id": r["id"], "partition": "whole-strip" if r["segment"] == "all" else "anchor-aligned-thirds", "bandStems": r["formerEdgeStems"], "stems": r["stems"],
                           "meanCiWith": r["meanCi"], "meanCiWithout": statistics.mean(t["ciPost"] for t in no_band),
                           "dCiWith": r["dCi"], "dCiWithout": rel(statistics.mean(t["ciPost"] for t in no_band), src["meanCi"]),
                           "aboveP90With": r["aboveSourceP90"], "aboveP90Without": sum(t["ciPost"] > src_p90 for t in no_band) / len(no_band),
                           "densityWith": r["densityPerHa"], "densityWithout": len(no_band) / r["netAreaM2"] * 1e4})
    band_effect["perUnit"] = band_units

    # --------------------------------------------------------------------------------- stand-edge (outer lattice) truncation
    interior = [t for t in living if not t["mask"] and t["edgeDist"] >= HEGYI_REACH and not t["formerEdge"]]
    edge_bins = []
    for lo, hi in ((0, 2), (2, 4), (4, 6), (6, 8), (8, 12), (12, 40)):
        s = [t for t in living if not t["mask"] and lo <= t["edgeDist"] < hi and not t["formerEdge"]]
        edge_bins.append({"edgeDistanceM": [lo, hi], "stems": len(s), "meanCi": statistics.mean(t["ciPost"] for t in s), "meanDbh": statistics.mean(t["dbh"] for t in s),
                          "dCiVsSource": rel(statistics.mean(t["ciPost"] for t in s), src["meanCi"])})
    # Rack corridors also truncate competition for their neighbours (post-access).
    def dist_to_corridor(t):
        return min(abs(t["z"] - z) for z in W.rack_z) - W.half_w
    corridor_bins = []
    for lo, hi in ((0, 1), (1, 2), (2, 4), (4, 8)):
        s = [t for t in living if not t["mask"] and t["place"] == "CoverageUnit" and lo <= dist_to_corridor(t) < hi and t["edgeDist"] >= HEGYI_REACH and not t["formerEdge"]]
        corridor_bins.append({"distanceFromCorridorEdgeM": [lo, hi], "stems": len(s), "meanCi": statistics.mean(t["ciPost"] for t in s), "meanCiPreAccess": statistics.mean(t["ciPre"] for t in s)})

    # ------------------------------------------------------------------------------------------------------- remainders
    remainder = {"fixture": export["remainders"], "fixtureStems": len(rem_stems), "probes": export["remainderProbes"]}

    # ---------------------------------------------------- regeneration / juvenile: historical diagnostic (not the V2 fixture)
    def cell_rect(index):
        row, col = divmod(index, 16)
        return (-STAND + 5 * col, -STAND + 5 * col + 5, -STAND + 5 * row, -STAND + 5 * row + 5)
    regen_units = []
    for r in regions["anchor-aligned-thirds"]:
        u = next(u for u in units if u["rackId"] == r["rackId"])
        x0, x1 = {"W": (-STAND, -12.0), "C": (-12.0, 12.0), "E": (12.0, STAND)}[r["segment"]]
        z0, z1 = max(-STAND, u["lower"]), min(STAND, u["upper"])
        occ, cells_hit, weighted = 0, 0, 0.0
        for c in regen["cells"]:
            a0, a1, b0, b1 = cell_rect(c["index"])
            ov = max(0.0, min(a1, x1) - max(a0, x0)) * max(0.0, min(b1, z1) - max(b0, z0))
            if ov > 0:
                cells_hit += 1
                weighted += c["relativeDensity"] * ov / 25.0
        cells_total = sum(1 for idx in range(256) if (lambda a: max(0.0, min(a[1], x1) - max(a[0], x0)) * max(0.0, min(a[3], z1) - max(a[2], z0)) > 0)(cell_rect(idx)))
        regen_units.append({"id": r["id"], "cellsOverlapped": cells_total, "cellsWithCohorts": cells_hit, "areaWeightedRelativeOccupancy": weighted})

    # ------------------------------------------------------------------------------------------- robustness variants
    # Same anchor-aligned regions, re-measured with alternative masks or competition state, classified by the recommended rule and
    # by the guard alternative. A robust rule should not change classification under these measurement choices.
    def variant(label, infra_variant, ci_key):
        Wv = World(export, Masks(infra_variant, head))
        lv = [t for t in Wv.trees if not t["rackRemoval"]]
        sv_stems = [t for t in lv if t["footprint"] and not t["mask"]]
        sv = metrics(sv_stems, Wv.net_area(Wv.fp_rect, exclude_footprint=False), ci_key=ci_key)
        sv["aboveSourceP90"] = sum(t[ci_key] > sv["p90Ci"] for t in sv_stems) / len(sv_stems)
        rows = []
        for u in units:
            for sname, x0, x1 in partitions["anchor-aligned-thirds"]:
                st = [t for t in lv if t["place"] == "CoverageUnit" and t["unit"] == u["index"] and x0 <= t["x"] < x1 + (1e-9 if x1 >= STAND else 0) and not t["footprint"] and not t["mask"]]
                m = metrics(st, Wv.net_area(unit_rect(u, x0, x1)), sv["p90Ci"], ci_key=ci_key)
                m.update({"id": "R%d%s" % (u["rackId"], sname), "dDensity": rel(m["densityPerHa"], sv["densityPerHa"]), "dDbh": rel(m["meanDbhCm"], sv["meanDbhCm"]),
                          "dCi": rel(m["meanCi"], sv["meanCi"])})
                rows.append(m)
        def cls(p):
            saved = src["aboveSourceP90"]
            src["aboveSourceP90"] = sv["aboveSourceP90"]
            out = [r["id"] for r in rows if covered(r, *p)]
            src["aboveSourceP90"] = saved
            return out
        return {"label": label, "sourceDensity": sv["densityPerHa"], "sourceMeanCi": sv["meanCi"], "sourceNetAreaM2": sv["netAreaM2"],
                "coveredRecommended": cls(rec), "coveredWithGuard10": cls(centres["with-P90-guard-10pp"]),
                "dCi": {r["id"]: r["dCi"] for r in rows}, "dDensity": {r["id"]: r["dDensity"] for r in rows}}
    gen_clearing = json.loads(json.dumps(infra)); gen_clearing["clearing"]["visualRadiusM"] = infra["clearingRadiusM"]
    no_masks = {"pathCorridorRadiusM": 0.0, "clearingRadiusM": 0.0, "scenePlanks": [], "approachExtension": [], "clearing": {"x": 0, "z": 0, "visualRadiusM": 0.0},
                "player": {"x": 0, "z": 0}, "buildables": []}
    head_none = dict(head, Width=0.0, StemGuard=0.0, PadRadius=0.0)
    variants = [variant("baseline (visible clearing 4.5 m, post-access CI)", infra, "ciPost"),
                variant("generator clearing radius 2.2 m", gen_clearing, "ciPost"),
                variant("pre-access competition state", infra, "ciPre")]
    # No-mask variant: headland capsules are given zero radius as well, so only rack corridors are removed from the area.
    head_saved = dict(head)
    head.update(head_none)
    variants.append(variant("no infrastructure masks (gross density)", no_masks, "ciPost"))
    head.clear(); head.update(head_saved)
    # Growth drift: MFM-02 live footprint BA 1.441753 -> 1.517099 m2 over one growth step (Unity evidence). Uniform DBH scale that
    # implies, and the DBH difference it would create if a target were measured N years after the source snapshot.
    live_ba_step = 1.5170994823782642 / 1.441753070295678
    growth = {"liveFootprintBaRatioOneStep": live_ba_step, "impliedDbhRatioPerStep": math.sqrt(live_ba_step),
              "dbhDriftAfterYears": {str(n): math.sqrt(live_ba_step) ** n - 1 for n in (1, 2, 3)}}

    # ------------------------------------------------------------------------------------------------ candidate spacing
    def judge_reject(tree_id, rate):
        if rate <= 0:
            return False
        h = int(hashlib.sha256(("mfm03-workload|" + tree_id).encode()).hexdigest()[:8], 16)
        return (h % 10000) < rate * 10000

    def eligible_pool(region_ids, partition):
        rows = {r["id"]: r for r in regions[partition]}
        segs = dict((s, (a, b)) for s, a, b in partitions[partition])
        pool = []
        for rid in region_ids:
            r = rows[rid]
            u = next(u for u in units if u["rackId"] == r["rackId"])
            x0, x1 = segs["" if r["segment"] == "all" else r["segment"]]
            stems = region_stems(u["index"], x0, x1)
            cx = statistics.mean(t["x"] for t in stems)
            cz = statistics.mean(t["z"] for t in stems)
            pool.append((rid, (cx, cz), stems))
        return pool

    def candidates(pool, spacing, order, rate=0.0, resuggest=False):
        fixed = [(c["x"], c["z"]) for c in crops]
        accepted, shown, rejected = [], [], []
        def ok(t, placed):
            return all((t["x"] - x) ** 2 + (t["z"] - z) ** 2 >= spacing * spacing for x, z in placed)
        if order == "unit-centre-first":
            seq = []
            for rid, (cx, cz), stems in pool:
                seq += [(rid, t) for t in sorted(stems, key=lambda t: ((t["x"] - cx) ** 2 + (t["z"] - cz) ** 2, t["x"], t["z"], t["id"]))]
        else:   # west-to-east scan across the eligible area, then south-to-north, then ID
            seq = sorted(((rid, t) for rid, _, stems in pool for t in stems), key=lambda p: (p[1]["x"], p[1]["z"], p[1]["id"]))
        if not resuggest:
            # Batch: the whole spacing-only suggestion set is generated first, then each one is confirmed or rejected; a rejection
            # leaves that position empty (manual designation remains available) and is not silently replaced.
            placed = list(fixed)
            for rid, t in seq:
                if ok(t, placed):
                    placed.append((t["x"], t["z"]))
                    shown.append((rid, t))
            for rid, t in shown:
                (rejected if judge_reject(t["id"], rate) else accepted).append((rid, t))
        else:
            # Queue with re-suggestion: a rejected stem stops blocking spacing, so a later stem nearby may be suggested instead.
            placed = list(fixed)
            for rid, t in seq:
                if ok(t, placed):
                    shown.append((rid, t))
                    if judge_reject(t["id"], rate):
                        rejected.append((rid, t))
                    else:
                        accepted.append((rid, t))
                        placed.append((t["x"], t["z"]))
        return shown, accepted, rejected

    def tour(points):
        start = (W.e["footprint"]["anchorX"], W.e["footprint"]["anchorZ"])
        left = list(points)
        pos, dist = start, 0.0
        while left:
            nxt = min(left, key=lambda p: (p[0] - pos[0]) ** 2 + (p[1] - pos[1]) ** 2)
            dist += math.hypot(nxt[0] - pos[0], nxt[1] - pos[1])
            pos = nxt
            left.remove(nxt)
        return dist

    def release_indicator(accepted, pool_ids, partition, ceiling):
        """Bounded greedy INDICATOR, not a prescription: around each confirmed candidate remove its largest-Share removable
        neighbours (living, outside masks/footprint/corridors, inside a covered region, not a candidate or Crop Tree) until 30% of its
        current competition is planned; charge each unique removal's BA to its own region; report candidates reaching 30% and the
        heaviest region BA share. Greedy failure is not proof that no plan exists."""
        rows = {r["id"]: r for r in regions[partition]}
        segs = dict((s, (a, b)) for s, a, b in partitions[partition])
        region_of = {}
        region_ba = {}
        for rid in pool_ids:
            r = rows[rid]
            u = next(u for u in units if u["rackId"] == r["rackId"])
            x0, x1 = segs["" if r["segment"] == "all" else r["segment"]]
            stems = region_stems(u["index"], x0, x1)
            region_ba[rid] = sum(t["ba"] for t in stems)
            for t in stems:
                region_of[t["id"]] = rid
        protected = set(c["id"] for c in crops) | set(t["id"] for _, t in accepted)
        removed = set()
        charged = {rid: 0.0 for rid in pool_ids}
        reached, short = 0, []
        for rid, t in sorted(accepted, key=lambda p: p[1]["id"]):
            total = t["ciPost"]
            terms = sorted(((term, nid) for nid, term in t["nb"]), reverse=True)
            got = sum(term for term, nid in terms if nid in removed) / total if total else 1.0
            for term, nid in terms:
                if got >= RELEASE_TARGET:
                    break
                if nid in removed or nid in protected or nid not in region_of:
                    continue
                o = region_of[nid]
                if (charged[o] + W.by_id[nid]["ba"]) / region_ba[o] > ceiling + 1e-12:
                    continue
                removed.add(nid)
                charged[o] += W.by_id[nid]["ba"]
                got += term / total
            if got >= RELEASE_TARGET - 1e-12:
                reached += 1
            else:
                short.append((t["id"], got))
        heaviest = max((charged[r] / region_ba[r] for r in pool_ids), default=0.0)
        return {"reached": reached, "of": len(accepted), "removals": len(removed), "heaviestRegionBaShare": heaviest, "short": short[:10]}

    spacing_results = []
    partition = "anchor-aligned-thirds"
    cov_ids = [r["id"] for r in regions[partition] if covered(r, *rec)]
    all_ids = [r["id"] for r in regions[partition] if r["stems"] >= 30]
    for scope_name, ids in (("recommended-covered", cov_ids), ("all-units-upper-bound", all_ids)):
        pool = eligible_pool(ids, partition)
        net_ha = sum(next(r for r in regions[partition] if r["id"] == rid)["netAreaM2"] for rid in ids) / 1e4
        for spacing in (8, 10, 12, 14, 16, 18, 20):
            for order in ("unit-centre-first", "west-east-scan"):
                row = {"scope": scope_name, "spacingM": spacing, "order": order, "coveredRegions": len(ids), "eligibleStems": sum(len(p[2]) for p in pool), "netCoveredHa": net_ha}
                shown, acc, rej = candidates(pool, spacing, order)
                row["candidates"] = len(shown)
                row["candidateIds"] = [rid + ":" + t["id"] for rid, t in shown]
                row["perRegion"] = {rid: sum(1 for r2, _ in shown if r2 == rid) for rid in ids}
                row["regionsWithoutCandidate"] = [rid for rid, n in row["perRegion"].items() if n == 0]
                row["tourM"] = tour([(t["x"], t["z"]) for _, t in shown])
                row["decisions"] = {}
                for rate in (0.0, 0.25, 0.5):
                    for resuggest in (False, True):
                        s2, a2, r2 = candidates(pool, spacing, order, rate, resuggest)
                        row["decisions"]["reject%d%s" % (int(rate * 100), "-resuggest" if resuggest else "")] = {
                            "shown": len(s2), "confirmed": len(a2), "rejected": len(r2), "tourM": tour([(t["x"], t["z"]) for _, t in s2])}
                if scope_name == "recommended-covered":
                    row["releaseAtSampleCeiling"] = release_indicator(acc, ids, partition, SAMPLE_RELEASE_BA_SHARE)
                    row["releaseAt40"] = release_indicator(acc, ids, partition, SAFEGUARD)
                spacing_results.append(row)

    # ------------------------------------------------------------------------------------------------------------- output
    results = {
        "fixture": {"signature": export["signature"], "footprint": export["footprint"], "racks": [{k: r[k] for k in ("id", "index", "offset", "entranceX", "entranceZ", "farX", "farZ", "width")} | {"removals": len(r["removals"])} for r in W.racks],
                    "units": units, "remainders": export["remainders"], "livingStart": len(W.trees), "livingAfterAccess": len(living)},
        "source": src, "sourceMaskedStems": [t["id"] for t in fp_masked], "sourceGrossNetAreaM2": src_area_gross, "sourceAreaConvergence": conv,
        "footprintStemsByUnitRack": fp_units, "sampleCropTrees": [{"id": c["id"], "x": c["x"], "z": c["z"], "unitRack": units[c["unit"]]["rackId"]} for c in crops],
        "regions": regions, "sensitivity": sensitivity, "sweeps": sweeps, "windows": window_summary, "windowRows": windows,
        "formerEdge": band_effect, "edgeTruncation": edge_bins, "corridorTruncation": corridor_bins, "remainder": remainder,
        "regenerationHistoricalDiagnostic": {"source": regen["source"], "ecologicalYear": regen["ecologicalYear"], "cellsWithCohorts": len(regen["cells"]),
                                             "plantedJuveniles": regen["plantedJuveniles"], "clearancePatches": regen["clearancePatches"], "shelters": regen["shelters"],
                                             "deadwoodRecords": regen["deadwoodRecords"], "perRegion": regen_units},
        "spacing": spacing_results, "variants": variants, "growthDrift": growth, "recommendedRule": {"densityTol": rec[0], "dbhTol": rec[1], "ciTol": rec[2], "p90GuardPp": rec[3], "minStems": 30, "partition": partition},
        "recommendedClassification": {pname: [{"id": r["id"], "covered": covered(r, *rec), "reasons": reasons(r, *rec), "margin": margin(r, *rec)} for r in rows] for pname, rows in regions.items()},
        "historicalClassification": {pname: [{"id": r["id"], "covered": covered(r, 0.20, 0.15, 0.20, None), "reasons": reasons(r, 0.20, 0.15, 0.20, None),
                                              "margin": margin(r, 0.20, 0.15, 0.20, None)} for r in rows] for pname, rows in regions.items()},
        "checks": checks,
    }
    os.makedirs(out_dir, exist_ok=True)
    with open(os.path.join(out_dir, "results.json"), "w") as fh:
        json.dump(results, fh, indent=1, default=lambda o: None)
    failed = [c for c in checks if not c["ok"]]
    for c in checks:
        print(("MFM03_CHECK_PASS " if c["ok"] else "MFM03_CHECK_FAIL ") + c["label"] + ((" :: " + c["detail"]) if not c["ok"] and c["detail"] else ""))
    print(("MFM03_ANALYSIS_PASS" if not failed else "MFM03_ANALYSIS_FAIL") + " checks=%d failed=%d" % (len(checks), len(failed)))
    return 1 if failed else 0


if __name__ == "__main__":
    sys.exit(main())
