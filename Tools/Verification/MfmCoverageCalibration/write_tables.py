#!/usr/bin/env python3
"""MFM-03: render results.json (from analyse_coverage.py) as the Markdown tables used in the handoff. Measurement only.

Usage: write_tables.py <out-dir>      reads <out-dir>/results.json, writes <out-dir>/tables.md
Time figures use ASSUMED 15/30/60 s per individual candidate decision and an ASSUMED 4 m/s straight-line walking speed; they are
illustrations, not measured player effort.
"""
import json
import os
import sys

out = sys.argv[1]
R = json.load(open(os.path.join(out, "results.json")))
L = []
P = lambda v: "%+.1f%%" % (100 * v)


def table(head, rows):
    L.append("| " + " | ".join(head) + " |")
    L.append("|" + "|".join("---" for _ in head) + "|")
    for r in rows:
        L.append("| " + " | ".join(str(c) for c in r) + " |")
    L.append("")


s = R["source"]
L.append("## Source (teaching footprint, post-access, pre-treatment)\n")
table(["stems", "net area m2", "density /ha", "mean DBH cm", "QMD cm", "BA m2/ha", "mean CI", "sd CI", "P90 CI", "above own P90"],
      [[s["stems"], "%.1f" % s["netAreaM2"], "%.0f" % s["densityPerHa"], "%.2f" % s["meanDbhCm"], "%.2f" % s["qmdCm"], "%.1f" % s["basalAreaM2PerHa"],
        "%.3f" % s["meanCi"], "%.3f" % s["sdCi"], "%.3f" % s["p90Ci"], "%.1f%%" % (100 * s["aboveSourceP90"])]])

rec = {c["id"]: c for c in R["recommendedClassification"]["anchor-aligned-thirds"]}
hist = {c["id"]: c for c in R["historicalClassification"]["anchor-aligned-thirds"]}
regen = {r["id"]: r for r in R["regenerationHistoricalDiagnostic"]["perRegion"]}
for pname in ("anchor-aligned-thirds", "whole-strip", "equal-thirds"):
    L.append("## Per-unit metrics — %s\n" % pname)
    cls = {c["id"]: c for c in R["recommendedClassification"][pname]}
    rows = []
    for r in R["regions"][pname]:
        c = cls[r["id"]]
        m = r["maskedStems"]
        state = "SOURCE (manual)" if r["footprintStems"] and r["stems"] == 0 else ("covered" if c["covered"] else "not covered")
        rows.append([r["id"], r["stems"], "%.0f" % r["netAreaM2"], "%.0f" % r["densityPerHa"], P(r["dDensity"]) if r["stems"] else "—",
                     "%.2f" % r["meanDbhCm"], P(r["dDbh"]) if r["stems"] else "—", "%.2f" % r["meanCi"], P(r["dCi"]) if r["stems"] else "—",
                     ("±%.1f" % (100 * r["seDCi"])) if r["stems"] else "—", "%.1f%%" % (100 * r["aboveSourceP90"]), r["formerEdgeStems"],
                     r["within8mOfBoundary"], r["legacyCoreStems"], r["rackRemovals"], m["clearing"] + m["track"] + m["headland"] + m["sites"], r["footprintStems"],
                     "%.0f/%.0f/%.0f" % (r["corridorAreaM2"], r["infrastructureAreaM2"], r["footprintAreaM2"]), state, "%.2f" % c["margin"] if r["stems"] else "—"])
    table(["unit", "stems", "net m2", "dens/ha", "Δdens", "DBH", "ΔDBH", "CI", "ΔCI", "SE ΔCI pp", "> src P90", "former-edge", "≤8 m of boundary",
           "core stems", "rack removals", "masked stems", "sample stems", "corridor/infra/sample m2", "recommended rule", "margin"], rows)

L.append("## Robustness variants (anchor-aligned thirds)\n")
table(["variant", "source density", "source CI", "covered (recommended)", "covered (with +10 pp tail guard)"],
      [[v["label"], "%.0f" % v["sourceDensity"], "%.3f" % v["sourceMeanCi"], "%d: %s" % (len(v["coveredRecommended"]), " ".join(v["coveredRecommended"])),
        "%d: %s" % (len(v["coveredWithGuard10"]), " ".join(v["coveredWithGuard10"]))] for v in R["variants"]])

for pname in ("anchor-aligned-thirds", "whole-strip"):
    for cname, sw in R["sweeps"][pname].items():
        L.append("## One-at-a-time sweep — %s, centre %s\n" % (pname, cname))
        rows = []
        for k in ("density", "dbh", "ci", "guard", "minStems"):
            for t, ids in sw[k].items():
                rows.append([k, t, len(ids), " ".join(ids)])
        table(["varied", "value", "covered", "covered units"], rows)

w = R["windows"]
L.append("## Sample-sized window spread (%d windows; %d clean interior)\n" % (w["count"], w["interiorCleanCount"]))
rows = []
for k in ("dDensity", "dDbh", "dCi"):
    rows.append([k] + ["%.0f%% / %.0f%%" % (100 * w["within"][k][t]["all"], 100 * w["within"][k][t]["interiorNoFormerEdge"]) for t in ("0.05", "0.1", "0.15", "0.2", "0.25", "0.3")]
                + ["%s … %s" % (P(w["range"][k][0]), P(w["range"][k][1]))])
table(["metric", "≤5%", "≤10%", "≤15%", "≤20%", "≤25%", "≤30%", "range (all)"], rows)
L.append("Windows whose crowded tail exceeds source + g: " + ", ".join("+%d pp: %.0f%%" % (round(float(g) * 100), 100 * v) for g, v in w["tailExceedsSourcePlus"].items()) + "\n")

fe = R["formerEdge"]
L.append("## Former-edge clamp-line band\n")
table(["group", "mean CI", "mean DBH", "mean nearest-neighbour m", "> source P90"],
      [[g, "%.3f" % fe["meanCi"].get(g, float("nan")), "%.2f" % fe["meanDbh"].get(g, float("nan")) if g in fe["meanDbh"] else "—",
        "%.3f" % fe["meanNearestNeighbourM"][g] if g in fe["meanNearestNeighbourM"] else "—",
        "%.1f%%" % (100 * fe["fractionAboveSourceP90"][g]) if g in fe["fractionAboveSourceP90"] else "—"] for g in ("formerEdge", "ringNeighboursWithin2m", "coreInterior", "outerPlantation")])
table(["unit", "band stems", "ΔCI with band", "ΔCI without band", "> src P90 with", "> src P90 without"],
      [[u["id"], u["bandStems"], P(u["dCiWith"]), P(u["dCiWithout"]), "%.1f%%" % (100 * u["aboveP90With"]), "%.1f%%" % (100 * u["aboveP90Without"])] for u in fe["perUnit"]])

L.append("## Stand-edge competition truncation (no former-edge stems)\n")
table(["distance to stand boundary m", "stems", "mean CI", "ΔCI vs source"],
      [["%g–%g" % tuple(b["edgeDistanceM"]), b["stems"], "%.2f" % b["meanCi"], P(b["dCiVsSource"])] for b in R["edgeTruncation"]])
table(["distance from rack-corridor edge m", "stems", "mean CI post-access", "mean CI pre-access"],
      [["%g–%g" % tuple(b["distanceFromCorridorEdgeM"]), b["stems"], "%.2f" % b["meanCi"], "%.2f" % b["meanCiPreAccess"]] for b in R["corridorTruncation"]])

L.append("## Outer remainders\n")
rows = [["MFM-02 fixture", " ".join("%.3f" % z["entranceZ"] for z in R["fixture"]["racks"]),
         "%.1f (edge %.3f)" % (R["remainder"]["fixture"][0]["area"], R["remainder"]["fixture"][0]["stripEdge"]),
         "%.1f (edge %.3f)" % (R["remainder"]["fixture"][1]["area"], R["remainder"]["fixture"][1]["stripEdge"]), R["remainder"]["fixtureStems"], "—"]]
for p in R["remainder"]["probes"]:
    rows.append([p["name"] + " (hypothetical)", " ".join("%g" % z for z in p["z"]), "%.1f" % p["remainders"][0]["area"], "%.1f" % p["remainders"][1]["area"], "", "%d / %d" % (p["rightStems"], p["leftStems"])])
table(["layout", "rack z", "right (south) remainder m2", "left (north) remainder m2", "fixture stems", "probe stems right/left"], rows)

L.append("## Regeneration — historical Lesson-5 diagnostic (pre-V2 save, year 2; NOT the V2 fixture)\n")
d = R["regenerationHistoricalDiagnostic"]
L.append("Cells with natural cohorts: %d; planted juveniles %d; clearance patches %d; shelters %d; deadwood records %d.\n"
         % (d["cellsWithCohorts"], d["plantedJuveniles"], d["clearancePatches"], d["shelters"], d["deadwoodRecords"]))
table(["unit", "5 m cells overlapped", "cells with cohorts", "area-weighted relative occupancy"],
      [[r["id"], r["cellsOverlapped"], r["cellsWithCohorts"], "%.3f" % r["areaWeightedRelativeOccupancy"]] for r in d["perRegion"]])

L.append("## Candidate spacing and confirmation workload\n")
L.append("Decisions shown/confirmed/rejected. `batch` = suggestion set generated once, rejection leaves the position empty; `queue` = a rejection stops blocking spacing so a nearby stem may be suggested next. Rejections are a deterministic hash-of-ID simulation, not a quality model. Time = decisions × 15/30/60 s (ASSUMED) + straight nearest-neighbour tour at 4 m/s (ASSUMED).\n")
for scope in ("recommended-covered", "all-units-upper-bound"):
    rows = []
    for r in R["spacing"]:
        if r["scope"] != scope:
            continue
        d = r["decisions"]
        def f(k):
            v = d[k]
            return "%d/%d/%d" % (v["shown"], v["confirmed"], v["rejected"])
        t = lambda sec: (r["candidates"] * sec + r["tourM"] / 4.0) / 60.0
        rel = r.get("releaseAtSampleCeiling")
        rel40 = r.get("releaseAt40")
        rows.append([r["spacingM"], r["order"], r["candidates"], "%.1f" % (r["candidates"] / r["netCoveredHa"]), len(r["regionsWithoutCandidate"]), "%.0f" % r["tourM"],
                     "%.1f / %.1f / %.1f" % (t(15), t(30), t(60)), f("reject25"), f("reject25-resuggest"), f("reject50"), f("reject50-resuggest"),
                     ("%d/%d, %.1f%%" % (rel["reached"], rel["of"], 100 * rel["heaviestRegionBaShare"])) if rel else "—",
                     ("%d/%d, %.1f%%" % (rel40["reached"], rel40["of"], 100 * rel40["heaviestRegionBaShare"])) if rel40 else "—"])
    L.append("### Scope: %s (%d regions, %d eligible stems, %.3f ha net)\n" % (scope, R["spacing"][0 if scope == "recommended-covered" else -1]["coveredRegions"],
             [r for r in R["spacing"] if r["scope"] == scope][0]["eligibleStems"], [r for r in R["spacing"] if r["scope"] == scope][0]["netCoveredHa"]))
    table(["spacing m", "order", "candidates (= decisions at 0% rejection)", "per net ha", "regions w/o candidate", "tour m", "minutes @15/30/60 s",
           "25% reject batch", "25% reject queue", "50% reject batch", "50% reject queue", "greedy 30% @ sample ceiling 17.9%", "greedy 30% @ 40%"], rows)

g = R["growthDrift"]
L.append("Live MFM-02 growth step: footprint BA ×%.4f ⇒ uniform DBH ×%.4f per step; DBH drift after 1/2/3 steps: %s.\n"
         % (g["liveFootprintBaRatioOneStep"], g["impliedDbhRatioPerStep"], " / ".join(P(v) for v in g["dbhDriftAfterYears"].values())))
open(os.path.join(out, "tables.md"), "w").write("\n".join(L))
print("MFM03_TABLES_WRITTEN " + os.path.join(out, "tables.md"))
