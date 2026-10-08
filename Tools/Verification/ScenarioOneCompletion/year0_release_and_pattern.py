#!/usr/bin/env python3
"""Offline Year-0 crop-tree release and spatial-pattern prototype.

Read-only design evidence for Docs/Research/ScenarioOneCompletion. Not a Unity
gate and not production code. Standard library only.

Inputs (copied with provenance into Docs/Research/ScenarioOneCompletion/Evidence):
  input-y0-trees.csv        Year-0 tree table exported by the pedagogy harness
  input-residual-stand.json treatment definitions T0-T5 and Unity immediate results

What it does:
  1. Recomputes each tree's Hegyi competition index (CI) from positions and DBH
     with the production formula and 8 m cutoff, and checks it against the
     Unity-exported CI (validation of this offline reproduction).
  2. For the 16 fixture Crop Trees, lists ranked neighbours (share of CI).
  3. For each treatment, computes the proposed crop-tree release metric and
     checks crop CI before/after against the Unity harness.
  4. Computes candidate spatial-pattern descriptors and classifies each
     treatment under a grid of thresholds to test robustness.

Why Year 0 is still valid on main a8596df: growth model 1 (D-048) changes
annual height growth and adds density mortality; it does not change the
authored starting stand, the Hegyi formula/cutoff, the light model or prices.
Later-year numbers from the pedagogy harness are NOT reused.
"""
import csv
import json
import math
import os
import sys
from collections import defaultdict

HERE = os.path.dirname(os.path.abspath(__file__))
ROOT = os.path.abspath(os.path.join(HERE, "..", "..", ".."))
EVID = os.path.join(ROOT, "Docs", "Research", "ScenarioOneCompletion", "Evidence")

CUTOFF_M = 8.0          # ForestEcologyController.HegyiCutoffMeters
CELL_M = 5.0            # Scenario One ecology cell size
CELLS_PER_AXIS = 8      # 40 m x 40 m property
ORIGIN = -20.0          # stand spans -20..20 m


def hegyi(neighbour_dbh, target_dbh, distance):
    # ForestEcologyController.HegyiTerm
    return (neighbour_dbh / max(1.0, target_dbh)) / max(0.5, distance)


def cell_of(x, z):
    cx = min(CELLS_PER_AXIS - 1, max(0, int((x - ORIGIN) // CELL_M)))
    cz = min(CELLS_PER_AXIS - 1, max(0, int((z - ORIGIN) // CELL_M)))
    return cz * CELLS_PER_AXIS + cx


def label(cell):
    return "ABCDEFGH"[cell % CELLS_PER_AXIS] + str(cell // CELLS_PER_AXIS + 1)


def load():
    trees = {}
    with open(os.path.join(EVID, "input-y0-trees.csv"), newline="") as handle:
        for row in csv.DictReader(handle):
            trees[row["tree"]] = {
                "x": float(row["x_m"]), "z": float(row["z_m"]), "dbh": float(row["dbh_cm"]),
                "h": float(row["height_m"]), "ci": float(row["ci"]), "cell": int(row["cell"]),
                "vol": float(row["stem_vol_m3"]), "hd": float(row["hd_ratio"]),
            }
    with open(os.path.join(EVID, "input-residual-stand.json")) as handle:
        study = json.load(handle)
    return trees, study


def ci_map(trees, removed):
    living = {k: v for k, v in trees.items() if k not in removed}
    result = {}
    for tid, t in living.items():
        total = 0.0
        for nid, n in living.items():
            if nid == tid:
                continue
            d = math.hypot(t["x"] - n["x"], t["z"] - n["z"])
            if d <= CUTOFF_M:
                total += hegyi(n["dbh"], t["dbh"], d)
        result[tid] = total
    return result


def neighbours(trees, tid, removed=frozenset()):
    t = trees[tid]
    rows = []
    for nid, n in trees.items():
        if nid == tid or nid in removed:
            continue
        d = math.hypot(t["x"] - n["x"], t["z"] - n["z"])
        if d <= CUTOFF_M:
            rows.append((nid, d, n["dbh"], hegyi(n["dbh"], t["dbh"], d)))
    total = sum(r[3] for r in rows)
    rows.sort(key=lambda r: (-r[3], r[0]))
    return total, [(nid, d, dbh, term, term / total if total else 0.0) for nid, d, dbh, term in rows]


def basal_area_m2(dbh_cm):
    return math.pi * (dbh_cm / 200.0) ** 2


def components(cells):
    """4-connected components of a set of cell indices."""
    seen, out = set(), []
    for start in sorted(cells):
        if start in seen:
            continue
        stack, comp = [start], []
        seen.add(start)
        while stack:
            c = stack.pop()
            comp.append(c)
            x, z = c % CELLS_PER_AXIS, c // CELLS_PER_AXIS
            for dx, dz in ((1, 0), (-1, 0), (0, 1), (0, -1)):
                nx, nz = x + dx, z + dz
                if 0 <= nx < CELLS_PER_AXIS and 0 <= nz < CELLS_PER_AXIS:
                    nc = nz * CELLS_PER_AXIS + nx
                    if nc in cells and nc not in seen:
                        seen.add(nc)
                        stack.append(nc)
        out.append(sorted(comp))
    return out


def pattern(trees, removed, opened_share, cluster_cells, cluster_mass):
    ba_cell = defaultdict(float)
    rem_cell = defaultdict(float)
    for tid, t in trees.items():
        ba_cell[t["cell"]] += basal_area_m2(t["dbh"])
        if tid in removed:
            rem_cell[t["cell"]] += basal_area_m2(t["dbh"])
    total_removed = sum(rem_cell.values())
    if total_removed <= 0:
        return "NO REMOVAL", {}
    opened = {c for c, r in rem_cell.items() if ba_cell[c] > 0 and r / ba_cell[c] >= opened_share}
    comps = components(opened)
    largest = max((len(c) for c in comps), default=0)
    mass_in_largest = max((sum(rem_cell[c] for c in comp) for comp in comps), default=0.0) / total_removed
    mass_in_opened = sum(rem_cell[c] for c in opened) / total_removed
    if largest >= cluster_cells and mass_in_largest >= cluster_mass:
        verdict = "CONCENTRATED GAP"
    elif opened and mass_in_opened >= cluster_mass:
        verdict = "SMALL GROUP OPENINGS"
    else:
        verdict = "DISTRIBUTED REMOVAL"
    return verdict, {
        "cells_touched": len(rem_cell), "opened_cells": len(opened), "opened_groups": len(comps),
        "largest_group_cells": largest, "share_in_largest_group": round(mass_in_largest, 3),
        "share_in_opened_cells": round(mass_in_opened, 3),
        "max_cell_share_removed": round(max(r / ba_cell[c] for c, r in rem_cell.items()), 3),
    }


def main():
    trees, study = load()
    crop = study["cropTrees"]
    out_lines = []

    # 1. Validate the offline CI reproduction against Unity's exported CI.
    ci = ci_map(trees, frozenset())
    worst = max(abs(ci[t] - trees[t]["ci"]) for t in trees)
    out_lines.append(f"CI_REPRODUCTION max_abs_error={worst:.4f} trees={len(trees)}")

    # 2. Ranked neighbours for each fixture crop tree (top 5).
    rows = []
    for tid in crop:
        total, ranked = neighbours(trees, tid)
        for rank, (nid, d, dbh, term, share) in enumerate(ranked, 1):
            rows.append({"crop_tree": tid, "crop_dbh_cm": trees[tid]["dbh"], "crop_ci": round(total, 4),
                         "rank": rank, "neighbour": nid, "neighbour_dbh_cm": dbh, "distance_m": round(d, 2),
                         "hegyi_term": round(term, 4), "share_of_ci": round(share, 4),
                         "neighbour_is_crop": int(nid in crop)})
    with open(os.path.join(EVID, "crop-neighbour-ranking.csv"), "w", newline="") as handle:
        writer = csv.DictWriter(handle, fieldnames=list(rows[0].keys()))
        writer.writeheader()
        writer.writerows(rows)
    shares = defaultdict(list)
    for r in rows:
        shares[r["crop_tree"]].append(r["share_of_ci"])
    top1 = sorted(v[0] for v in shares.values())
    top3 = sorted(sum(v[:3]) for v in shares.values())
    counts = sorted(len(v) for v in shares.values())
    out_lines.append(f"NEIGHBOURS_PER_CROP min={counts[0]} median={counts[len(counts)//2]} max={counts[-1]}")
    out_lines.append(f"TOP1_SHARE min={top1[0]:.3f} median={top1[len(top1)//2]:.3f} max={top1[-1]:.3f}")
    out_lines.append(f"TOP3_SHARE min={top3[0]:.3f} median={top3[len(top3)//2]:.3f} max={top3[-1]:.3f}")
    strong = sorted(sum(1 for s in v if s >= 0.10) for v in shares.values())
    out_lines.append(f"NEIGHBOURS_GE_10PCT min={strong[0]} median={strong[len(strong)//2]} max={strong[-1]}")

    # 3. Release metric per treatment; validate against Unity's crop CI before/after.
    unity = {row["treatment"]: row for row in study["immediate"] if row["context"] == "Y0"}
    release_rows = []
    for treatment in study["treatments"]:
        removed = frozenset(treatment["removed"])
        after = ci_map(trees, removed)
        before_sum = sum(ci[t] for t in crop)
        after_sum = sum(after[t] for t in crop)
        meaningful = 0
        released = 0
        for tid in crop:
            total, ranked = neighbours(trees, tid)
            hit = [r for r in ranked if r[0] in removed and r[4] >= 0.10]
            meaningful += len(hit)
            if total > 0 and (total - after[tid]) / total >= 0.10:
                released += 1
        far = sum(1 for r in removed if all(
            math.hypot(trees[r]["x"] - trees[c]["x"], trees[r]["z"] - trees[c]["z"]) > CUTOFF_M for c in crop))
        u = unity[treatment["id"]]["crop_release"]
        release_rows.append({
            "treatment": treatment["id"], "removed_n": len(removed),
            "crop_ci_before_mean": round(before_sum / len(crop), 4), "crop_ci_after_mean": round(after_sum / len(crop), 4),
            "unity_ci_before": u["ci_before"], "unity_ci_after": u["ci_after"],
            "crop_ci_change_pct": round((after_sum - before_sum) / before_sum * 100, 2),
            "crop_trees_released_ge10pct": released, "meaningful_competitors_removed": meaningful,
            "removals_beyond_8m_of_any_crop": far,
        })
    with open(os.path.join(EVID, "release-metric.csv"), "w", newline="") as handle:
        writer = csv.DictWriter(handle, fieldnames=list(release_rows[0].keys()))
        writer.writeheader()
        writer.writerows(release_rows)
    for r in release_rows:
        out_lines.append("RELEASE " + " ".join(f"{k}={v}" for k, v in r.items()))

    # 4. Spatial descriptors under a threshold grid.
    grid = [(s, c, m) for s in (0.25, 0.30, 0.40) for c in (3, 4, 5) for m in (0.4, 0.5, 0.6)]
    pattern_rows = []
    for treatment in study["treatments"]:
        removed = frozenset(treatment["removed"])
        verdicts = defaultdict(int)
        central = None
        for s, c, m in grid:
            verdict, facts = pattern(trees, removed, s, c, m)
            verdicts[verdict] += 1
            if (s, c, m) == (0.30, 4, 0.5):
                central = (verdict, facts)
        row = {"treatment": treatment["id"], "description": treatment["description"],
               "central_verdict": central[0]}
        row.update(central[1])
        row["grid_agreement"] = f"{max(verdicts.values())}/{len(grid)}"
        row["grid_verdicts"] = "; ".join(f"{k}:{v}" for k, v in sorted(verdicts.items()))
        pattern_rows.append(row)
        out_lines.append("PATTERN " + " ".join(f"{k}={v}" for k, v in row.items() if k != "description"))
    keys = sorted({k for r in pattern_rows for k in r}, key=lambda k: list(pattern_rows[1].keys()).index(k)
                  if k in pattern_rows[1] else 99)
    with open(os.path.join(EVID, "spatial-pattern.csv"), "w", newline="") as handle:
        writer = csv.DictWriter(handle, fieldnames=keys)
        writer.writeheader()
        writer.writerows(pattern_rows)

    with open(os.path.join(EVID, "year0-prototype-output.txt"), "w") as handle:
        handle.write("\n".join(out_lines) + "\n")
    print("\n".join(out_lines))
    return 0


if __name__ == "__main__":
    sys.exit(main())
