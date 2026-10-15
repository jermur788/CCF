#!/usr/bin/env python3
"""Apply the density v3 acceptance (Docs/Verification/ThreeSpeciesProgramme/CompositeDensityV3Predeclared.md, Trial 1 and Trial 2)
to a Unity long-run CSV written by DensityQualification (phase LONGRUN).

Usage: analyze_trial2.py longrun-<geo>.csv [--md OUT.md]

Pure scenarios (A_OAK, B_BEECH, E_OAK_YOUNG, F_BEECH_YOUNG) are judged by R1' - R4. Mixed scenarios (C, G) by R2' on the GENERIC
component (addendum A4), R3', R4 and R5' (Sitka subset held at its own line). D (pure Sitka) is shown for comparison.
Trial 1 values (all-stem loss, occupancy ratio, whole-stand R2) are printed beside the Trial 2 verdicts so nothing is hidden.
"""
import csv
import collections
import sys


def slope(ys, a, b):
    xs = list(range(a, b + 1))
    vs = [ys[x - 1] for x in xs]
    n = len(xs)
    mx, my = sum(xs) / n, sum(vs) / n
    den = sum((x - mx) ** 2 for x in xs)
    return sum((x - mx) * (v - my) for x, v in zip(xs, vs)) / den if den else 0.0


def main():
    path = sys.argv[1]
    md = sys.argv[sys.argv.index("--md") + 1] if "--md" in sys.argv else None
    rows = collections.defaultdict(list)
    for r in csv.DictReader(open(path)):
        rows[r["scenario"]].append(r)
    out = []
    geo = None
    for sc, v in rows.items():
        v.sort(key=lambda r: int(r["year"]))
        geo = v[0]["geo"]
        n = len(v)
        if n < 300:
            out.append(f"## {sc}: only {n} years, skipped")
            continue
        occ = [float(r["occupancyAfter"]) for r in v]
        living = [int(r["livingBefore"]) for r in v]
        deaths = [int(r["densityDeaths"]) for r in v]
        gen = [int(r["genericDeaths"]) for r in v]
        mixed = sc.startswith(("C_", "G_"))
        loss = [d / l if l else 0.0 for d, l in zip(deaths, living)]
        loss2 = [g / l if l else 0.0 for g, l in zip(gen, living)] if mixed else loss
        s2, s15 = slope(occ, 201, 300), slope(occ, 151, 250)
        blk = max(abs(sum(loss2[b + 4:b + 9]) / 5 - sum(loss2[b - 1:b + 4]) / 5) for b in range(26, 292, 5))
        maxyr = max(loss2[25:])
        first = loss[0]
        wdead = wbig = wall = 0.0
        for st in range(26, 277):
            w = v[st - 1:st + 24]
            wdead = max(wdead, sum(float(r["deadBaFraction"]) for r in w) / 25)
            bb = sum(int(r["bigBefore"]) for r in w)
            wbig = max(wbig, sum(int(r["bigDeaths"]) for r in w) / max(1, bb))
            wall = max(wall, sum(int(r["densityDeaths"]) for r in w) / max(1, sum(int(r["livingBefore"]) for r in w)))
        ba = [float(r["baM2ha"]) for r in v]
        minratio = min(ba[i] / ba[i - 25] for i in range(25, 300) if ba[i - 25] > 0)
        ratio300_100 = ba[299] / ba[99]
        rd = [float(r["sitkaRdBeforeStep"]) for r in v[200:]]
        meanrd = sum(rd) / len(rd)
        recruits = sum(int(r["netOther"]) for r in v[200:]) / 100
        R1 = abs(s2) <= 0.0025 and abs(s15) <= 0.004
        R2 = blk <= 0.10 and maxyr <= 0.40
        R3 = wdead <= 0.04 and wbig <= 0.05
        R4 = minratio >= 0.5 and ratio300_100 >= 0.5
        verdict = ("R2'-R4 " + ("PASS" if (R2 and R3 and R4) else "FAIL") + f", R5' {'PASS' if meanrd <= 1.02 else 'FAIL'}") if mixed else \
                  ("R1'-R4 " + ("PASS" if (R1 and R2 and R3 and R4) else "FAIL")) if not sc.startswith("D_") else "comparator"
        out.append(f"### {geo} {sc}: {verdict}")
        out.append("")
        out.append("| year | living | BA m2/ha | occupancy | Dq cm | density deaths | loss | Sitka/Beech/Oak living |")
        out.append("|---|---|---|---|---|---|---|---|")
        for y in (25, 50, 100, 150, 200, 250, 300):
            r = v[y - 1]
            out.append(f"| {y} | {r['livingAfter']} | {float(r['baM2ha']):.1f} | {float(r['occupancyAfter']):.2f} | {float(r['dq']):.1f} | {r['densityDeaths']} | {int(r['densityDeaths'])/max(1,int(r['livingBefore'])):.3f} | {r['nSitka']}/{r['nBeech']}/{r['nOak']} |")
        out.append("")
        out.append(f"* R1' slope Y201-300 {s2:+.4f} (limit 0.0025), Y151-250 {s15:+.4f} (limit 0.004): {'PASS' if R1 else 'FAIL'}" + ("  [not required for mixed stands]" if mixed else ""))
        out.append(f"* R2' (years 26-300, {'generic component' if mixed else 'all stems'}): 5-year-block change {blk:.3f} (limit 0.10), largest year {maxyr:.3f} (limit 0.40): {'PASS' if R2 else 'FAIL'}; first-year loss {first:.3f} (reported)")
        out.append(f"* R3' worst 25-year window: dead basal area {wdead*100:.2f} % (limit 4), stems >= 20 cm {wbig*100:.2f} % (limit 5): {'PASS' if R3 else 'FAIL'}; all-stem loss {wall*100:.1f} % (reported, not tested); recruits per year Y201-300 about {recruits:.1f}")
        out.append(f"* R4 worst 25-year basal-area ratio {minratio:.2f}, Y300/Y100 {ratio300_100:.2f} (limits 0.5): {'PASS' if R4 else 'FAIL'}")
        if mixed or sc.startswith("D_"):
            out.append(f"* R5' mean Sitka-subset relative density at the start of the step, Y201-300: {meanrd:.3f} (limit 1.02): {'PASS' if meanrd <= 1.02 else 'FAIL'}")
        out.append(f"* Trial 1 values for the record: occupancy Y300/Y200 {occ[299]/occ[199]:.2f} (R1 limit 1.10)")
        out.append("")
    text = "\n".join(out)
    print(text)
    if md:
        open(md, "w").write(text + "\n")


if __name__ == "__main__":
    main()
