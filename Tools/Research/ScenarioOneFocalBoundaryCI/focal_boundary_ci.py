#!/usr/bin/env python3
"""CCF #484 Tier 1: offline focal Hegyi competition-index boundary sensitivity.

Pure standard-library Python. No Unity, no repository imports, no writes to the
evidence root. Run with:  python3 -I -B focal_boundary_ci.py --out-dir <dir>

Question: for the original #460 scratch 20 x 20 m and 30 x 30 m study forests,
how much does the current 8 m Hegyi competition index (CI) of a few identified
focal Sitka trees change if a *hypothetical*, continuously stocked surrounding
forest is added outside the property, compared with the actual finite forest?

What is measured vs assumed
  MEASURED (read-only from #460 raw JSON): tree IDs, X/Z, DBH, recorded source CI.
  RECOMPUTED offline: the source Hegyi term (ForestEcologyController.cs
      HegyiTerm / UpdateCompetition), in float64 and in a float32 emulation.
  ASSUMED (plainly hypothetical): every external stem. See build_external_stems.

Source formula (Assets/ForestPrototype/ForestEcologyController.cs, blob 0290a7be):
  for each living target i, for each living j != i in ordinal-TreeId order:
      distance = Vector2.Distance(pos_i, pos_j); skip if distance > 8 m
      ci += (dbh_j / max(1, dbh_i)) / max(0.5, distance)
"""
from __future__ import annotations

import argparse
import csv
import hashlib
import json
import math
import os
import platform
import struct
import sys
from dataclasses import dataclass

SCRIPT_VERSION = "1.0.0"

# ---- constants locked to current CCF source -------------------------------
HEGYI_CUTOFF_M = 8.0              # ForestEcologyController.HegyiCutoffMeters
DISTANCE_FLOOR_M = 0.5            # Mathf.Max(0.5f, distance)
DBH_FLOOR_CM = 1.0                # Mathf.Max(1f, targetDiameter)
LATTICE_SPACING_M = 1.9           # ForestStartingStand.latticeSpacingMeters
STARTING_STEMS_PER_HA = 2100.0    # StandGeometryModel.StartingStemsPerHectare
SITKA_CI50 = 5.0                  # unity-project-overview C8; display arithmetic only

# ---- pinned #460 evidence --------------------------------------------------
DEFAULT_EVIDENCE_ROOT = (
    "/media/jer/Files/CCF/Verification/CompactOneRackNativeFeasibility460-20261113T145748Z/"
    "native-20261113T152528Z"
)
PINNED_SHA256 = {  # from the published #460 report
    "native-result.json": "58524ad4f634378a875313909bb5cf42cfb37468abe0e60a61b63ce00a51de40",
    "executed-harness.cs": "3c0ebae88448d27b0aae4d0345b37a28026f8c9cc3b417d13920288853019863",
}
CANDIDATES = (  # id, file, expected property size (m), #460 crop (anchor) IDs
    ("C-A-20", "C-A-20.json", 20.0, ("P0101", "P0604")),
    ("C-B-30", "C-B-30.json", 30.0, ("P0000", "P1504")),
)


class DataGateError(Exception):
    """Raw evidence cannot be read and reconciled. Stop before calculating."""


# ---- float32 emulation -----------------------------------------------------
_F32 = struct.Struct("<f")


def f32(x: float) -> float:
    """Round a Python float to the nearest IEEE-754 binary32 value."""
    return _F32.unpack(_F32.pack(x))[0]


# ---- data model ------------------------------------------------------------
@dataclass(frozen=True)
class Stem:
    id: str
    x: float
    z: float
    dbh: float
    weight: float = 1.0   # 1.0 = a real stem; <1.0 only in the expected-value sensitivity


# ---- Hegyi arithmetic ------------------------------------------------------
def hegyi_term_f64(neighbour_dbh: float, target_dbh: float, distance: float) -> float:
    return (neighbour_dbh / max(DBH_FLOOR_CM, target_dbh)) / max(DISTANCE_FLOOR_M, distance)


def hegyi_term_f32(neighbour_dbh: float, target_dbh: float, distance: float) -> float:
    # Every operand is already a float32 value; each C# float operation rounds once.
    quotient = f32(neighbour_dbh / f32(max(DBH_FLOOR_CM, target_dbh)))
    return f32(quotient / f32(max(DISTANCE_FLOOR_M, distance)))


def distance_f64(a: Stem, b: Stem) -> float:
    return math.hypot(a.x - b.x, a.z - b.z)


def distance_f32(a: Stem, b: Stem) -> float:
    dx = f32(f32(a.x) - f32(b.x))
    dz = f32(f32(a.z) - f32(b.z))
    return f32(math.sqrt(f32(f32(dx * dx) + f32(dz * dz))))


ARITHMETIC = {
    "float64": (hegyi_term_f64, distance_f64, lambda v: v),
    "float32": (hegyi_term_f32, distance_f32, f32),
}


def ordinal_sorted(stems):
    """Engine order: string.CompareOrdinal on TreeId (IDs here are ASCII)."""
    return sorted(stems, key=lambda s: s.id)


def contributions(focal: Stem, others, arithmetic: str):
    """Hegyi terms of `others` on `focal`, in the given (already ordered) order.

    Self is excluded by ID. Neighbours farther than 8 m are skipped; exactly 8 m
    is included, matching `if (distance > cutoffMeters) continue;`.
    Returns a list of (id, distance, term, weighted_term).
    """
    term_fn, dist_fn, cast = ARITHMETIC[arithmetic]
    out = []
    for other in others:
        if other.id == focal.id:
            continue
        d = dist_fn(focal, other)
        if d > HEGYI_CUTOFF_M:
            continue
        t = term_fn(cast(other.dbh), cast(focal.dbh), d)
        wt = t if other.weight == 1.0 else cast(other.weight * t)
        out.append((other.id, d, t, wt))
    return out


def accumulate(terms, arithmetic: str, start: float = 0.0) -> float:
    cast = ARITHMETIC[arithmetic][2]
    total = cast(start)
    for _id, _d, _t, wt in terms:
        total = cast(total + wt)
    return total


def finite_ci(focal: Stem, stems, arithmetic: str):
    terms = contributions(focal, ordinal_sorted(stems), arithmetic)
    return accumulate(terms, arithmetic), terms


def context_ci(focal: Stem, stems, external, arithmetic: str):
    """Finite terms (ordinal order) first, then external terms (ordinal order).

    Returns (ci_finite, ci_context, ext_sum, finite_terms, ext_terms). With no
    external stems ci_context is bit-identical to ci_finite by construction.
    """
    f_terms = contributions(focal, ordinal_sorted(stems), arithmetic)
    ci_f = accumulate(f_terms, arithmetic)
    e_terms = contributions(focal, ordinal_sorted(external), arithmetic)
    ci_c = accumulate(e_terms, arithmetic, start=ci_f)
    ext_sum = accumulate(e_terms, arithmetic)
    return ci_f, ci_c, ext_sum, f_terms, e_terms


# ---- property geometry -----------------------------------------------------
def is_outside_property(x: float, z: float, half: float) -> bool:
    """Square property centred on the origin; the boundary line itself is inside."""
    return max(abs(x), abs(z)) > half


def edge_distance_inside(x: float, z: float, half: float) -> float:
    """Same definition as the #460 harness: size/2 - max(|x|,|z|)."""
    return half - max(abs(x), abs(z))


def distance_outside_property(x: float, z: float, half: float) -> float:
    return math.hypot(max(abs(x) - half, 0.0), max(abs(z) - half, 0.0))


# ---- hypothetical external context ----------------------------------------
def fnv1a32(text: str) -> int:
    h = 2166136261
    for byte in text.encode("utf-8"):
        h = ((h ^ byte) * 16777619) & 0xFFFFFFFF
    return h


def mix32(h: int) -> int:
    """murmur3 finaliser, identical to ForestStartingStand.Mix (used there for the Enlarged80 outer ring)."""
    h ^= h >> 16
    h = (h * 0x85EBCA6B) & 0xFFFFFFFF
    h ^= h >> 13
    h = (h * 0xC2B2AE35) & 0xFFFFFFFF
    h ^= h >> 16
    return h


def stocking_fraction() -> float:
    full_lattice_per_ha = 10000.0 / (LATTICE_SPACING_M ** 2)
    return STARTING_STEMS_PER_HA / full_lattice_per_ha


def lattice_geometry(per_axis: int):
    span = (per_axis - 1) * LATTICE_SPACING_M
    return -span * 0.5, span


def build_external_stems(per_axis: int, half: float, ext_dbh: float, mode: str):
    """ONE plainly hypothetical continuation of the source planting lattice.

    The source lattice (origin -span/2 on both axes, 1.9 m spacing, x from the
    column index c and z from the row index r) is extended to every node outside
    the property square. No jitter. Every external stem is Sitka, age 20, with
    the same assumed DBH `ext_dbh`.

    mode "primary":  keep a node if mix32(fnv1a32(id))/2^32 < p, where p is the source
        2,100 stems/ha target divided by the full-lattice density. Deterministic
        thinning to the source target stocking; NOT the source omission algorithm.
    mode "expected": keep every node with weight p (sensitivity S1; fractional
        stems, realisation-free).
    IDs start with "X_" so they can never collide with source "Pxxxx" IDs.
    """
    p = stocking_fraction()
    origin, _span = lattice_geometry(per_axis)
    margin = int(math.ceil(HEGYI_CUTOFF_M / LATTICE_SPACING_M)) + 3
    out = []
    for r in range(-margin, per_axis + margin):
        for c in range(-margin, per_axis + margin):
            x = origin + c * LATTICE_SPACING_M
            z = origin + r * LATTICE_SPACING_M
            if not is_outside_property(x, z, half):
                continue
            sid = f"X_r{r:+03d}_c{c:+03d}"
            if mode == "primary":
                if mix32(fnv1a32(sid)) / 4294967296.0 < p:
                    out.append(Stem(sid, x, z, ext_dbh))
            elif mode == "expected":
                out.append(Stem(sid, x, z, ext_dbh, weight=p))
            else:
                raise ValueError(mode)
    return out


# ---- reading and reconciling the raw evidence ------------------------------
def sha256_file(path: str) -> str:
    h = hashlib.sha256()
    with open(path, "rb") as fh:
        for block in iter(lambda: fh.read(1 << 20), b""):
            h.update(block)
    return h.hexdigest()


def read_text(path: str) -> str:
    with open(path, encoding="utf-8") as fh:
        return fh.read()


def load_json(path: str):
    return json.loads(read_text(path))


def metadata_snapshot(root: str):
    snap = {}
    for dirpath, _dirs, files in os.walk(root):
        for name in files:
            full = os.path.join(dirpath, name)
            st = os.stat(full)
            snap[os.path.relpath(full, root)] = (st.st_size, st.st_mtime_ns)
    return snap


def _finite_number(value, label):
    if isinstance(value, bool) or not isinstance(value, (int, float)) or not math.isfinite(value):
        raise DataGateError(f"non-finite or non-numeric {label}: {value!r}")
    return float(value)


def stems_from_rows(rows, label: str):
    """Validate raw tree rows and return Stems. Raises DataGateError on any defect."""
    if not isinstance(rows, list) or not rows:
        raise DataGateError(f"{label}: trees[] missing or empty")
    seen = set()
    stems = []
    for i, row in enumerate(rows):
        for key in ("id", "species", "x", "z", "dbh", "ci", "edge", "side", "age"):
            if key not in row:
                raise DataGateError(f"{label}: row {i} missing field {key!r}")
        tid = row["id"]
        if not isinstance(tid, str) or not tid:
            raise DataGateError(f"{label}: row {i} has no usable id")
        if tid in seen:
            raise DataGateError(f"{label}: duplicate tree id {tid}")
        seen.add(tid)
        x = _finite_number(row["x"], f"{label}/{tid}.x")
        z = _finite_number(row["z"], f"{label}/{tid}.z")
        dbh = _finite_number(row["dbh"], f"{label}/{tid}.dbh")
        _finite_number(row["ci"], f"{label}/{tid}.ci")
        _finite_number(row["edge"], f"{label}/{tid}.edge")
        if dbh <= 0:
            raise DataGateError(f"{label}: {tid} has non-positive DBH {dbh}")
        stems.append(Stem(tid, x, z, dbh))
    return stems


def reconcile_candidate(cand_id, expected_size, data, harness_text):
    """Hard data-gate checks. Returns a list of (name, passed, detail)."""
    checks = []

    def add(name, ok, detail):
        checks.append((name, bool(ok), detail))

    rows = data["trees"]
    n = len(rows)
    add("candidate id matches file", data.get("id") == cand_id, data.get("id"))
    add("property size matches expected", data.get("size") == expected_size, data.get("size"))
    add("rows == recorded initial count == Year0 living", n == data.get("initial") == data["states"][0]["living"] and data["states"][0]["year"] == 0,
        f"rows={n} initial={data.get('initial')} year0_living={data['states'][0]['living']}")
    add("unique tree ids", len({r["id"] for r in rows}) == n, n)
    add("all species sitka-spruce", all(r["species"] == "sitka-spruce" for r in rows), "")
    add("all age 20 (generator canonical age)", all(r["age"] == 20 for r in rows), "")
    half = expected_size / 2.0
    worst = max(abs(r["edge"] - edge_distance_inside(r["x"], r["z"], half)) for r in rows)
    add("recorded edge == size/2 - max(|x|,|z|) (tol 1e-4)", worst <= 1e-4, f"max diff {worst:.3g}")
    add("all stems inside the property polygon", all(not is_outside_property(r["x"], r["z"], half) for r in rows), "")
    add("living-only list by construction (harness Trees() filters t.IsLiving)",
        "Where(t=>t.IsLiving)" in harness_text, "executed-harness.cs line 57")
    add("tree rows captured by MeasureInitial after Generate, before access felling",
        "MeasureInitial(c); c.beforeAccess=WalkSurvey(c);" in harness_text
        and "Harvest(c,\"access-clearance\"" in harness_text, "executed-harness.cs lines 85-87")
    add("recorded CI came from eco.GetCompetitionIndex(t)", "ci=eco.GetCompetitionIndex(t)" in harness_text, "executed-harness.cs line 162")
    return checks


# ---- focal selection -------------------------------------------------------
def select_focal(rows, anchor_ids):
    """Predeclared, delta-blind rule (uses geometry only, never any external result).

      1. The #460 crop anchors (P0101/P0604 or P0000/P1504), if present and retained.
      2. The retained tree with the greatest edge distance (most interior; tie: lowest id).
      3. The retained tree whose edge distance is closest to the median edge distance of
         retained trees ("typical"; tie: lowest id), not already chosen.
    "Retained" = side north or south of the #460 access corridor (i.e. not removed at Year 1).
    Returns [(id, reason)] with 4 entries.
    """
    retained = {r["id"]: r for r in rows if r["side"] in ("north", "south")}
    chosen, reasons = [], {}
    for aid in anchor_ids:
        if aid not in retained:
            raise DataGateError(f"#460 anchor {aid} missing or not a retained stem")
        chosen.append(aid)
        reasons[aid] = "#460 native crop anchor"
    pool = sorted((i for i in retained if i not in chosen))
    interior = min(pool, key=lambda i: (-retained[i]["edge"], i))
    chosen.append(interior)
    reasons[interior] = "most interior retained stem (max edge distance)"
    edges = sorted(r["edge"] for r in retained.values())
    mid = len(edges) // 2
    median = edges[mid] if len(edges) % 2 else (edges[mid - 1] + edges[mid]) / 2.0
    pool = sorted(i for i in retained if i not in chosen)
    typical = min(pool, key=lambda i: (abs(retained[i]["edge"] - median), i))
    chosen.append(typical)
    reasons[typical] = f"retained stem nearest the median retained edge distance ({median:.3f} m)"
    return [(i, reasons[i]) for i in chosen]


# ---- per-candidate analysis ------------------------------------------------
def band_summary(rows, half):
    bands = [(0, 2), (2, 4), (4, 8), (8, 1e9)]
    out = []
    for lo, hi in bands:
        sel = [r for r in rows if lo <= r["edge"] < hi]
        out.append({
            "edge_band_m": f"{lo}-{hi if hi < 1e8 else 'inf'}",
            "n": len(sel),
            "mean_recorded_ci": (sum(r["ci"] for r in sel) / len(sel)) if sel else None,
        })
    return out


def analyse_candidate(cand_id, size, data, anchor_ids):
    half = size / 2.0
    rows = data["trees"]
    stems = stems_from_rows(rows, cand_id)
    by_id = {s.id: s for s in stems}
    rec = {r["id"]: r for r in rows}
    per_axis = int(data["lattice"])
    origin, span = lattice_geometry(per_axis)
    ext_dbh = round(sum(s.dbh for s in stems) / len(stems), 4)

    # Whole-stand recreation check: recompute every recorded CI (validation, not a focal result).
    diffs64, diffs32, exact32 = [], [], 0
    for s in stems:
        c64, _ = finite_ci(s, stems, "float64")
        c32, _ = finite_ci(s, stems, "float32")
        diffs64.append(abs(c64 - rec[s.id]["ci"]))
        diffs32.append(abs(c32 - rec[s.id]["ci"]))
        exact32 += int(c32 == f32(rec[s.id]["ci"]))
    recreation = {
        "stems_checked": len(stems),
        "max_abs_diff_float64_vs_recorded": max(diffs64),
        "max_abs_diff_float32_emulation_vs_recorded": max(diffs32),
        "float32_emulation_bit_equal_to_recorded": exact32,
        "note": "Python emulation; not a claim of Unity bit-equality (no native comparison run).",
    }

    primary = build_external_stems(per_axis, half, ext_dbh, "primary")
    expected = build_external_stems(per_axis, half, ext_dbh, "expected")
    for e in primary + expected:
        if e.id in by_id:
            raise DataGateError(f"external id collides with source id: {e.id}")

    focal_list = select_focal(rows, anchor_ids)
    focal_out, used_ext = [], {}
    for fid, reason in focal_list:
        f = by_id[fid]
        r = rec[fid]
        row = {
            "candidate": cand_id, "stage": "Year 0 post-Generate, pre-access, all living",
            "focal_id": fid, "species": r["species"], "side": r["side"], "reason": reason,
            "x": f.x, "z": f.z, "dbh_cm": f.dbh, "edge_distance_m": r["edge"],
            "recorded_native_ci": r["ci"],
        }
        for arith in ("float64", "float32"):
            ci_f, ci_c, ext_sum, f_terms, e_terms = context_ci(f, stems, primary, arith)
            _, _, ext_sum_s1, _, s1_terms = context_ci(f, stems, expected, arith)
            # negative control 1: zero external stems must reproduce the finite CI exactly
            z_f, z_c, z_sum, _, z_terms = context_ci(f, stems, [], arith)
            tag = "f64" if arith == "float64" else "f32"
            row[f"ci_finite_{tag}"] = ci_f
            row[f"external_contribution_{tag}"] = ext_sum
            row[f"ci_context_{tag}"] = ci_c
            row[f"delta_ci_{tag}"] = ci_c - ci_f
            row[f"ratio_{tag}"] = (ci_c / ci_f) if ci_f > 0 else None
            row[f"s1_expected_external_contribution_{tag}"] = ext_sum_s1
            row[f"s1_expected_ci_context_{tag}"] = ci_f + ext_sum_s1
            row[f"negctl_zero_external_equal_{tag}"] = (z_c == z_f == ci_f and z_sum == 0.0 and not z_terms)
            if arith == "float64":
                row["finite_neighbour_count"] = len(f_terms)
                row["finite_neighbour_ids"] = [t[0] for t in f_terms]
                row["external_neighbour_count"] = len(e_terms)
                row["external_neighbour_ids"] = [t[0] for t in e_terms]
                row["s1_external_node_count_in_reach"] = len(s1_terms)
                for t in e_terms:
                    used_ext.setdefault(t[0], {})[fid] = (t[1], t[2])
        row["abs_diff_recomputed_f64_vs_recorded"] = abs(row["ci_finite_f64"] - r["ci"])
        row["abs_diff_recomputed_f32_vs_recorded"] = abs(row["ci_finite_f32"] - r["ci"])
        row["abs_diff_f32_vs_f64_context"] = abs(row["ci_context_f32"] - row["ci_context_f64"])
        row["source_ci50_multiplier_finite"] = 1.0 / (1.0 + row["ci_finite_f64"] / SITKA_CI50)
        row["source_ci50_multiplier_context"] = 1.0 / (1.0 + row["ci_context_f64"] / SITKA_CI50)
        focal_out.append(row)

    ext_rows = []
    pmap = {e.id: e for e in primary}
    for eid in sorted(used_ext):
        e = pmap[eid]
        ext_rows.append({
            "candidate": cand_id, "external_id": eid, "x": e.x, "z": e.z,
            "distance_outside_property_m": distance_outside_property(e.x, e.z, half),
            "assumed_species": "sitka-spruce", "assumed_age": 20, "assumed_dbh_cm": e.dbh,
            "provenance": "HYPOTHETICAL continuation of source lattice, not measured",
            "per_focal": {fid: {"distance_m": d, "hegyi_term": t} for fid, (d, t) in sorted(used_ext[eid].items())},
        })

    # negative control 2: an external-only world with a stem placed past the cutoff contributes nothing
    far = Stem("X_FAR", stems[0].x + HEGYI_CUTOFF_M + 0.5, stems[0].z, ext_dbh)
    far_ci_f, far_ci_c, far_sum, _, _ = context_ci(stems[0], [stems[0]], [far], "float64")

    return {
        "candidate_id": cand_id, "property_size_m": size, "half_size_m": half,
        "lattice_per_axis": per_axis, "lattice_origin_m": origin, "lattice_span_m": span,
        "living_stems": len(stems), "mean_dbh_cm": sum(s.dbh for s in stems) / len(stems),
        "assumed_external_dbh_cm": ext_dbh, "stocking_fraction_p": stocking_fraction(),
        "external_nodes_outside_property_considered": len(expected),
        "external_primary_stems_kept": len(primary),
        "finite_recreation_check": recreation,
        "ci_by_edge_band_recorded": band_summary(rows, half),
        "focal": focal_out, "external_stems_in_reach_of_focal": ext_rows,
        "negative_control_beyond_cutoff": {"ci_finite": far_ci_f, "ci_context": far_ci_c, "external_sum": far_sum},
    }


# ---- output ----------------------------------------------------------------
FOCAL_CSV_FIELDS = [
    "candidate", "focal_id", "species", "side", "reason", "x", "z", "dbh_cm", "edge_distance_m",
    "recorded_native_ci", "finite_neighbour_count", "ci_finite_f64", "ci_finite_f32",
    "external_neighbour_count", "external_contribution_f64", "ci_context_f64", "delta_ci_f64", "ratio_f64",
    "external_contribution_f32", "ci_context_f32", "s1_expected_external_contribution_f64",
    "s1_expected_ci_context_f64", "abs_diff_recomputed_f64_vs_recorded", "abs_diff_recomputed_f32_vs_recorded",
    "abs_diff_f32_vs_f64_context", "negctl_zero_external_equal_f64", "negctl_zero_external_equal_f32",
    "source_ci50_multiplier_finite", "source_ci50_multiplier_context",
]


def write_outputs(out_dir, results):
    os.makedirs(out_dir, exist_ok=True)
    with open(os.path.join(out_dir, "results.json"), "w", encoding="utf-8", newline="\n") as fh:
        json.dump(results, fh, indent=1, sort_keys=True)
        fh.write("\n")
    with open(os.path.join(out_dir, "focal-rows.csv"), "w", encoding="utf-8", newline="") as fh:
        w = csv.DictWriter(fh, fieldnames=FOCAL_CSV_FIELDS, extrasaction="ignore", lineterminator="\n")
        w.writeheader()
        for cand in results["candidates"]:
            for row in cand["focal"]:
                w.writerow(row)
    with open(os.path.join(out_dir, "external-stems.csv"), "w", encoding="utf-8", newline="") as fh:
        w = csv.writer(fh, lineterminator="\n")
        w.writerow(["candidate", "external_id", "x", "z", "distance_outside_property_m", "assumed_species",
                    "assumed_age", "assumed_dbh_cm", "focal_id", "distance_to_focal_m", "hegyi_term"])
        for cand in results["candidates"]:
            for e in cand["external_stems_in_reach_of_focal"]:
                for fid, v in e["per_focal"].items():
                    w.writerow([e["candidate"], e["external_id"], e["x"], e["z"], e["distance_outside_property_m"],
                                e["assumed_species"], e["assumed_age"], e["assumed_dbh_cm"], fid,
                                v["distance_m"], v["hegyi_term"]])


def main(argv=None):
    ap = argparse.ArgumentParser(description=__doc__.splitlines()[0])
    ap.add_argument("--evidence-root", default=DEFAULT_EVIDENCE_ROOT)
    ap.add_argument("--out-dir", required=True)
    args = ap.parse_args(argv)

    root = os.path.realpath(args.evidence_root)
    out_dir = os.path.realpath(args.out_dir)
    if out_dir == root or out_dir.startswith(root + os.sep):
        print("REFUSED: --out-dir must not be inside the read-only evidence root", file=sys.stderr)
        return 1

    try:
        before_meta = metadata_snapshot(root)
        manifest = load_json(os.path.join(root, "evidence-manifest.json"))
        needed = ["native-result.json", "executed-harness.cs"] + [c[1] for c in CANDIDATES]
        hashes_before = {}
        for name in needed:
            path = os.path.join(root, name)
            if not os.path.isfile(path):
                raise DataGateError(f"missing evidence file {name}")
            digest = sha256_file(path)
            hashes_before[name] = digest
            if manifest.get(name) != digest:
                raise DataGateError(f"{name}: sha256 {digest} != evidence-manifest {manifest.get(name)}")
            if name in PINNED_SHA256 and PINNED_SHA256[name] != digest:
                raise DataGateError(f"{name}: sha256 {digest} != #460 report pin {PINNED_SHA256[name]}")
        harness_text = read_text(os.path.join(root, "executed-harness.cs"))
        native = load_json(os.path.join(root, "native-result.json"))
        native_by_id = {c["id"]: c for c in native["candidates"]}

        results = {"script": "focal_boundary_ci.py", "script_version": SCRIPT_VERSION,
                   "stage": "Year 0, immediately after unmodified ForestStartingStand.Generate, before access felling",
                   "evidence_sha256": hashes_before, "constants": {
                       "hegyi_cutoff_m": HEGYI_CUTOFF_M, "distance_floor_m": DISTANCE_FLOOR_M,
                       "dbh_floor_cm": DBH_FLOOR_CM, "lattice_spacing_m": LATTICE_SPACING_M,
                       "starting_stems_per_ha": STARTING_STEMS_PER_HA, "sitka_ci50_display_only": SITKA_CI50},
                   "reconciliation": {}, "candidates": []}
        for cand_id, fname, size, anchors in CANDIDATES:
            data = load_json(os.path.join(root, fname))
            checks = reconcile_candidate(cand_id, size, data, harness_text)
            checks.append(("per-candidate file == its copy inside native-result.json", data == native_by_id.get(cand_id), ""))
            results["reconciliation"][cand_id] = [{"check": n, "pass": ok, "detail": d} for n, ok, d in checks]
            failed = [n for n, ok, _ in checks if not ok]
            if failed:
                raise DataGateError(f"{cand_id}: reconciliation failed: {failed}")
            cand = analyse_candidate(cand_id, size, data, anchors)
            rc = cand["finite_recreation_check"]
            if rc["max_abs_diff_float64_vs_recorded"] > 1e-4:
                raise DataGateError(f"{cand_id}: recomputed finite CI does not reproduce recorded CI "
                                    f"(max diff {rc['max_abs_diff_float64_vs_recorded']}); stage mismatch?")
            results["candidates"].append(cand)

        hashes_after = {n: sha256_file(os.path.join(root, n)) for n in needed}
        after_meta = metadata_snapshot(root)
        custody = {"files_in_root": len(before_meta), "metadata_unchanged": before_meta == after_meta,
                   "key_file_hashes_unchanged": hashes_before == hashes_after}
        if not (custody["metadata_unchanged"] and custody["key_file_hashes_unchanged"]):
            raise DataGateError("evidence root changed while reading")
        results["custody"] = custody
        write_outputs(out_dir, results)
        meta = {"python": platform.python_version(), "platform": platform.platform(),
                "script_sha256": sha256_file(os.path.abspath(__file__)), "evidence_root": root}
        with open(os.path.join(out_dir, "run-meta.json"), "w", encoding="utf-8", newline="\n") as fh:
            json.dump(meta, fh, indent=1, sort_keys=True)
            fh.write("\n")
        print(f"OK: wrote results.json, focal-rows.csv, external-stems.csv, run-meta.json to {out_dir}")
        return 0
    except DataGateError as exc:
        print(f"BLOCKED / DATA GATE: {exc}", file=sys.stderr)
        return 2


if __name__ == "__main__":
    sys.exit(main())
