#!/usr/bin/env python3
"""Multi-seed summary of the density v3 pure-stand acceptance (R1'-R4 of CompositeDensityV3Predeclared.md, Trial 2).

Usage:
  analyze_seeds.py --seed default=longrun-L40-300y.csv --seed 101=longrun-L40-seed101.csv ... [--reference E80=longrun-E80-300y.csv]
                   [--md OUT.md] [--json OUT.json]

Each CSV is a DensityQualification LONGRUN file (one geometry). The metric formulas are the ones in analyze_trial2.py, copied
unchanged so the two tools agree to the printed precision; run with --check-against to compare against analyze_trial2.py output.

For every pure fixture (A_OAK_DENSE, B_BEECH_DENSE, E_OAK_YOUNG, F_BEECH_YOUNG) it reports, per metric, the value in every seed,
the median, min and max, how many seeds pass, and for each miss the distance past the limit (absolute and as a fraction of the
limit). D_SITKA_DENSE_MODEL2 (the unchanged legacy route) is reported for the one lumpiness metric as a small-stand baseline when
present. Nothing here changes a threshold.
"""
import argparse
import collections
import csv
import json
import math
import statistics

PURE = ["A_OAK_DENSE", "B_BEECH_DENSE", "E_OAK_YOUNG", "F_BEECH_YOUNG"]
# name -> (description, limit, kind) ; kind "max" = value must be <= limit, "min" = value must be >= limit
METRICS = collections.OrderedDict([
    ("R1a", ("abs occupancy slope Y201-300", 0.0025, "max")),
    ("R1b", ("abs occupancy slope Y151-250", 0.004, "max")),
    ("R2a", ("largest 5-year-block change of annual loss", 0.10, "max")),
    ("R2b", ("largest single-year loss", 0.40, "max")),
    ("R3a", ("worst 25-y dead basal area fraction", 0.04, "max")),
    ("R3b", ("worst 25-y fraction of stems >= 20 cm dead", 0.05, "max")),
    ("R4a", ("worst 25-y basal-area ratio", 0.5, "min")),
    ("R4b", ("basal area Y300/Y100", 0.5, "min")),
])


def slope(ys, a, b):
    xs = list(range(a, b + 1))
    vs = [ys[x - 1] for x in xs]
    n = len(xs)
    mx, my = sum(xs) / n, sum(vs) / n
    den = sum((x - mx) ** 2 for x in xs)
    return sum((x - mx) * (v - my) for x, v in zip(xs, vs)) / den if den else 0.0


def load(path):
    rows = collections.defaultdict(list)
    for r in csv.DictReader(open(path)):
        rows[r["scenario"]].append(r)
    for v in rows.values():
        v.sort(key=lambda r: int(r["year"]))
    return rows


def metrics(v):
    """Same arithmetic as analyze_trial2.py main(), pure-stand branch (all-stem loss)."""
    n = len(v)
    occ = [float(r["occupancyAfter"]) for r in v]
    living = [int(r["livingBefore"]) for r in v]
    deaths = [int(r["densityDeaths"]) for r in v]
    loss = [d / l if l else 0.0 for d, l in zip(deaths, living)]
    s2, s15 = slope(occ, 201, 300), slope(occ, 151, 250)
    blk = max(abs(sum(loss[b + 4:b + 9]) / 5 - sum(loss[b - 1:b + 4]) / 5) for b in range(26, 292, 5))
    maxyr = max(loss[25:])
    wdead = wbig = 0.0
    for st in range(26, 277):
        w = v[st - 1:st + 24]
        wdead = max(wdead, sum(float(r["deadBaFraction"]) for r in w) / 25)
        bb = sum(int(r["bigBefore"]) for r in w)
        wbig = max(wbig, sum(int(r["bigDeaths"]) for r in w) / max(1, bb))
    ba = [float(r["baM2ha"]) for r in v]
    minratio = min(ba[i] / ba[i - 25] for i in range(25, 300) if ba[i - 25] > 0)
    ratio = ba[299] / ba[99]
    init = max(1, living[0])
    extra = dict(
        minLivingFraction=min(int(r["livingAfter"]) for r in v) / init,
        finalLiving=int(v[-1]["livingAfter"]),
        y300Ba=ba[299], y300Occ=occ[299],
        yearsLossOver40=sum(1 for x in loss[25:] if x > 0.40),
        yearsLossOver25=sum(1 for x in loss[25:] if x > 0.25),
        peakLossYear=25 + 1 + max(range(len(loss[25:])), key=lambda i: loss[25:][i]),
    )
    return dict(R1a=abs(s2), R1b=abs(s15), R2a=blk, R2b=maxyr, R3a=wdead, R3b=wbig, R4a=minratio, R4b=ratio,
                s2_signed=s2, s15_signed=s15), extra


def block_peak(v):
    """Where the largest 5-year-block change of the annual loss fraction (R2a) happens: (change, first year of the earlier block, mean loss before, after, living at the later block)."""
    living = [int(r["livingBefore"]) for r in v]
    loss = [int(r["densityDeaths"]) / l if l else 0.0 for r, l in zip(v, living)]
    best = None
    for b in range(26, 292, 5):
        before, after = sum(loss[b - 1:b + 4]) / 5, sum(loss[b + 4:b + 9]) / 5
        if best is None or abs(after - before) > best[0]:
            best = (abs(after - before), b, before, after, living[b + 4])
    return best


def passes(name, value):
    _, limit, kind = METRICS[name]
    return value <= limit if kind == "max" else value >= limit


def miss(name, value):
    """Distance past the limit in the failing direction; 0 when passing."""
    _, limit, kind = METRICS[name]
    d = value - limit if kind == "max" else limit - value
    return max(0.0, d)


def fmt(name, x):
    if name in ("R1a", "R1b"):
        return f"{x:.4f}"
    if name in ("R3a", "R3b"):
        return f"{x * 100:.2f}%"
    return f"{x:.3f}"


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument("--seed", action="append", default=[], help="LABEL=csv (Legacy40 or any single geometry)")
    ap.add_argument("--reference", action="append", default=[], help="LABEL=csv shown for comparison only")
    ap.add_argument("--baseline", action="append", default=[], help="LABEL=csv holding D_SITKA_DENSE_MODEL2 for that seed (legacy route, small-stand baseline)")
    ap.add_argument("--md")
    ap.add_argument("--json")
    a = ap.parse_args()

    seeds = [s.split("=", 1) for s in a.seed]
    refs = [s.split("=", 1) for s in a.reference]
    data = {lab: load(p) for lab, p in seeds}
    refdata = {lab: load(p) for lab, p in refs}

    out, js = [], {}
    npass_all = collections.OrderedDict()
    for sc in PURE:
        per = collections.OrderedDict()
        for lab, rows in data.items():
            if sc not in rows or len(rows[sc]) < 300:
                continue
            per[lab] = metrics(rows[sc])
        if not per:
            continue
        verdicts = {lab: all(passes(m, v[0][m]) for m in METRICS) for lab, v in per.items()}
        npass_all[sc] = (sum(verdicts.values()), len(verdicts))
        js[sc] = dict(seeds={lab: dict(values=v[0], extra=v[1], pass_all=verdicts[lab]) for lab, v in per.items()})
        out.append(f"### {sc}: {npass_all[sc][0]}/{npass_all[sc][1]} seeds pass every R1'-R4 limit")
        out.append("")
        out.append("| metric (limit) | " + " | ".join(per.keys()) + " | median | min | max | seeds passing |")
        out.append("|---|" + "---|" * (len(per) + 4))
        for m, (desc, limit, kind) in METRICS.items():
            vals = [per[l][0][m] for l in per]
            ok = [passes(m, x) for x in vals]
            cells = [fmt(m, x) + ("" if o else " **x**") for x, o in zip(vals, ok)]
            op = "<=" if kind == "max" else ">="
            out.append(f"| {m} {desc} ({op} {fmt(m, limit)}) | " + " | ".join(cells) +
                       f" | {fmt(m, statistics.median(vals))} | {fmt(m, min(vals))} | {fmt(m, max(vals))} | {sum(ok)}/{len(ok)} |")
            js[sc].setdefault("metrics", {})[m] = dict(values=dict(zip(per.keys(), vals)), median=statistics.median(vals), min=min(vals),
                                                        max=max(vals), limit=limit, kind=kind, seeds_passing=sum(ok), seeds=len(ok),
                                                        median_passes=passes(m, statistics.median(vals)),
                                                        misses={l: miss(m, per[l][0][m]) for l in per if not passes(m, per[l][0][m])},
                                                        misses_fraction_of_limit={l: miss(m, per[l][0][m]) / limit for l in per if not passes(m, per[l][0][m])})
        out.append("")
        failing = [(m, l) for m in METRICS for l in per if not passes(m, per[l][0][m])]
        if failing:
            out.append("Misses (value past the limit): " + "; ".join(
                f"{m} seed {l}: {fmt(m, per[l][0][m])} vs {fmt(m, METRICS[m][1])} (+{fmt(m, miss(m, per[l][0][m]))}, {miss(m, per[l][0][m]) / METRICS[m][1] * 100:.0f}% of limit)" for m, l in failing))
        else:
            out.append("No misses.")
        out.append("")
        out.append("Trajectory sanity: " + "; ".join(
            f"{l}: min living {per[l][1]['minLivingFraction']:.2f} of start, Y300 {per[l][1]['finalLiving']} stems / BA {per[l][1]['y300Ba']:.0f} / occ {per[l][1]['y300Occ']:.2f}, "
            f"years with loss>40%: {per[l][1]['yearsLossOver40']}, >25%: {per[l][1]['yearsLossOver25']}" for l in per))
        out.append("")
        for rl, rrows in refdata.items():
            if sc in rrows and len(rrows[sc]) >= 300:
                rm, rx = metrics(rrows[sc])
                out.append(f"Reference {rl}: " + ", ".join(f"{m} {fmt(m, rm[m])}" for m in METRICS) +
                           f"; Y300 {rx['finalLiving']} stems / BA {rx['y300Ba']:.0f} / occ {rx['y300Occ']:.2f}")
                out.append("")
                js[sc].setdefault("reference", {})[rl] = dict(values={m: rm[m] for m in METRICS}, extra=rx)

    # pooled summary across the four fixtures
    out.append("### Pooled")
    out.append("")
    tot_p = sum(p for p, _ in npass_all.values())
    tot_n = sum(n for _, n in npass_all.values())
    out.append(f"* fixture-seed runs passing every limit: {tot_p}/{tot_n}")
    for m in METRICS:
        miss_n = sum(1 for sc in js for l in js[sc]["seeds"] if not passes(m, js[sc]["seeds"][l]["values"][m]))
        out.append(f"* {m} ({METRICS[m][0]}): {tot_n - miss_n}/{tot_n} pass")
    js["_pooled"] = dict(pass_all=tot_p, total=tot_n)

    out.append("")
    out.append("### When and where the R2' maximum happens (largest 5-year-block change of the all-stem loss fraction)")
    out.append("")
    out.append("| fixture | run | max change | blocks (years) | mean loss before -> after | living stems at the later block | blocks over 0.10 |")
    out.append("|---|---|---|---|---|---|---|")
    for sc in PURE:
        runs = [(lab, rows[sc]) for lab, rows in data.items() if sc in rows and len(rows[sc]) >= 300] + \
               [(rl, rrows[sc]) for rl, rrows in refdata.items() if sc in rrows and len(rrows[sc]) >= 300]
        for lab, v in runs:
            ch, b, bef, aft, n = block_peak(v)
            living = [int(r["livingBefore"]) for r in v]
            loss = [int(r["densityDeaths"]) / l if l else 0.0 for r, l in zip(v, living)]
            over = sum(1 for bb in range(26, 292, 5) if abs(sum(loss[bb + 4:bb + 9]) / 5 - sum(loss[bb - 1:bb + 4]) / 5) > 0.10)
            out.append(f"| {sc} | {lab} | {ch:.3f} | Y{b}-{b + 4} -> Y{b + 5}-{b + 9} | {bef:.3f} -> {aft:.3f} | {n} | {over} |")
            js.setdefault(sc, {}).setdefault("r2_peak", {})[lab] = dict(change=ch, firstYear=b, before=bef, after=aft, living=n, blocksOver=over)

    # lumpiness baseline: the unchanged legacy Sitka route on the same stand
    base = collections.OrderedDict()
    for lab, rows in data.items():
        if "D_SITKA_DENSE_MODEL2" in rows and len(rows["D_SITKA_DENSE_MODEL2"]) >= 300:
            base[lab] = metrics(rows["D_SITKA_DENSE_MODEL2"])[0]
    for lab, path in (b.split("=", 1) for b in a.baseline):
        rows = load(path)
        if "D_SITKA_DENSE_MODEL2" in rows and len(rows["D_SITKA_DENSE_MODEL2"]) >= 300:
            base[lab] = metrics(rows["D_SITKA_DENSE_MODEL2"])[0]
    if base:
        out.append("")
        out.append("### Small-stand baseline: D_SITKA_DENSE_MODEL2 (legacy route, unchanged by v3), same geometry")
        out.append("")
        out.append("| seed | R2a | R2b | R1a | R1b | R3a | R3b |")
        out.append("|---|---|---|---|---|---|---|")
        for lab, mm in base.items():
            out.append(f"| {lab} | {fmt('R2a', mm['R2a'])} | {fmt('R2b', mm['R2b'])} | {fmt('R1a', mm['R1a'])} | {fmt('R1b', mm['R1b'])} | {fmt('R3a', mm['R3a'])} | {fmt('R3b', mm['R3b'])} |")
        js["_baseline_D"] = {lab: mm for lab, mm in base.items()}
        for rl, rrows in refdata.items():
            if "D_SITKA_DENSE_MODEL2" in rrows and len(rrows["D_SITKA_DENSE_MODEL2"]) >= 300:
                mm = metrics(rrows["D_SITKA_DENSE_MODEL2"])[0]
                out.append(f"| (reference {rl}) | {fmt('R2a', mm['R2a'])} | {fmt('R2b', mm['R2b'])} | {fmt('R1a', mm['R1a'])} | {fmt('R1b', mm['R1b'])} | {fmt('R3a', mm['R3a'])} | {fmt('R3b', mm['R3b'])} |")

    text = "\n".join(out)
    print(text)
    if a.md:
        open(a.md, "w").write(text + "\n")
    if a.json:
        json.dump(js, open(a.json, "w"), indent=1, default=float)


if __name__ == "__main__":
    main()
