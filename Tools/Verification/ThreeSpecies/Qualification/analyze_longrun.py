#!/usr/bin/env python3
"""Summarise DensityQualification LONGRUN csv files into the tables the density qualification asks for.

    analyze_longrun.py <longrun-*.csv> [--compare <older longrun-*.csv>] > tables.md

Per scenario: stems, basal area, occupancy, Dq, annual density mortality and species composition at the checkpoint years; and the three
failure modes the previous qualification (a395e3f, Result C) found, each as a number rather than a judgement:
  RUNAWAY      slope of occupancy over the last 50 and last 100 years (still rising?)
  OSCILLATION  years with an annual density-death fraction over 15%; largest single-year jump in that fraction after year 25
  ROUTE CLIFF  for mixed stands, the year the last broadleaf died and the annual losses around it
It recommends nothing and imposes no forestry threshold.
"""
import csv
import statistics
import sys
from collections import defaultdict

CHECKPOINTS = [25, 50, 100, 150, 200, 250, 300]


def load(path):
    scenarios = defaultdict(list)
    with open(path) as f:
        for row in csv.DictReader(f):
            scenarios[row["scenario"]].append(row)
    for rows in scenarios.values():
        rows.sort(key=lambda r: int(r["year"]))
    return scenarios


def slope(values):
    n = len(values)
    if n < 3:
        return 0.0
    xs = list(range(n))
    mx, my = sum(xs) / n, sum(values) / n
    den = sum((x - mx) ** 2 for x in xs)
    return sum((x - mx) * (v - my) for x, v in zip(xs, values)) / den if den else 0.0


def f(row, key):
    return float(row[key])


def table(name, rows):
    out = [f"### {name}", "", "| year | living stems | BA m²/ha | occupancy | Dq cm | density deaths that year | death fraction | route | Sitka / Beech / Oak living |", "|---|---|---|---|---|---|---|---|---|"]
    for y in CHECKPOINTS:
        if y > len(rows):
            continue
        r = rows[y - 1]
        before = f(r, "livingBefore")
        frac = f(r, "densityDeaths") / before if before else 0.0
        out.append(f"| {y} | {r['livingAfter']} | {f(r, 'baM2ha'):.1f} | {f(r, 'occupancyAfter'):.2f} | {f(r, 'dq'):.1f} | {r['densityDeaths']} | {frac:.3f} | {r['route']} | {r['nSitka']} / {r['nBeech']} / {r['nOak']} |")
    return "\n".join(out)


def failure_modes(name, rows):
    n = len(rows)
    occ = [f(r, "occupancyAfter") for r in rows]
    ba = [f(r, "baM2ha") for r in rows]
    frac = [f(r, "densityDeaths") / f(r, "livingBefore") if f(r, "livingBefore") else 0.0 for r in rows]
    lines = [f"* **{name}** ({n} years)"]
    lines.append(f"  * RUNAWAY: occupancy slope per year over the last 50 years {slope(occ[-50:]):+.4f}, over the last 100 {slope(occ[-100:]):+.4f}; "
                 f"occupancy Y{n} = {occ[-1]:.2f}, max {max(occ):.2f} (year {occ.index(max(occ)) + 1}); BA Y{n} = {ba[-1]:.0f} m²/ha, max {max(ba):.0f}")
    jumps = [(abs(frac[i] - frac[i - 1]), i + 1) for i in range(25, n)]
    big = max(jumps) if jumps else (0.0, 0)
    lines.append(f"  * OSCILLATION: years with density-death fraction over 15%: {sum(1 for v in frac if v >= 0.15)}; "
                 f"largest year-to-year change in that fraction after Y25: {big[0]:.3f} (into year {big[1]}); mean fraction over the last 50 years {statistics.mean(frac[-50:]):.3f}")
    broadleaf = [int(r["nBeech"]) + int(r["nOak"]) for r in rows]
    started_mixed = broadleaf[0] > 0
    if started_mixed:
        last_year = max((i + 1 for i, v in enumerate(broadleaf) if v > 0), default=0)
        if last_year < n:
            around = ", ".join(f"Y{y}: {int(rows[y - 1]['densityDeaths'])} of {rows[y - 1]['livingBefore']} ({frac[y - 1]:.3f}, {rows[y - 1]['route']})" for y in range(max(1, last_year - 2), min(n, last_year + 4) + 1))
            lines.append(f"  * ROUTE CLIFF: the last broadleaf is alive through year {last_year}; losses around it: {around}")
        else:
            lines.append(f"  * ROUTE CLIFF: broadleaves survive to Y{n} (no route flip occurs); final broadleaf living: {broadleaf[-1]}")
    return "\n".join(lines)


def main():
    args = [a for a in sys.argv[1:] if not a.startswith("--")]
    compare = sys.argv[sys.argv.index("--compare") + 1] if "--compare" in sys.argv else None
    if compare:
        args.remove(compare)
    data = load(args[0])
    older = load(compare) if compare else {}
    for name, rows in data.items():
        print(table(name, rows) + "\n")
        print(failure_modes(name, rows) + "\n")
        if name in older:
            print("  * for comparison, the same scenario in the older file:\n" + failure_modes(name + " (older)", older[name]).replace("* **", "    * **", 1) + "\n")


if __name__ == "__main__":
    main()
