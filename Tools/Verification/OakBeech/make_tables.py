#!/usr/bin/env python3
"""Turn rows-trajectories.csv (written by OakBeechQualification phase 'rows') into the 2B trajectory tables.

Per species / growth model / regime (the centre tree of each 5 x 5 grid): DBH, height, 5-year mean annual DBH and height
increment, competition index and DBH/height ratio at the preregistered ages. No empirical curve is compared; the lab
checkpoints (Tools/Calibration/OakBeechGrowth/production_v1_expected.csv) are only quoted where the fixture is the lab's.
"""
import csv, sys
from collections import defaultdict
AGES = [10, 20, 30, 40, 50, 75, 100, 125]
def main(path, out):
    rows = defaultdict(dict)
    for r in csv.DictReader(open(path)):
        if r['tree'] != '2-2':
            continue
        rows[(r['species'], r['model'], r['regime'])][int(r['age'])] = (float(r['D_cm']), float(r['H_m']), float(r['CI']), float(r['D_over_H']))
    lines = []
    for (species, model, regime), series in sorted(rows.items(), key=lambda kv: (kv[0][0], kv[0][1], ['dense', 'moderate', 'strong'].index(kv[0][2]))):
        lines.append(f"\n### {species}, growth model {model}, regime {regime}\n")
        lines.append("| age (y) | DBH cm | height m | dDBH cm/y (5-y mean) | dH m/y (5-y mean) | CI | DBH/H cm per m |")
        lines.append("|---:|---:|---:|---:|---:|---:|---:|")
        for a in AGES:
            if a not in series:
                continue
            d, h, ci, ratio = series[a]
            p = series.get(a - 5)
            dd = f"{(d - p[0]) / 5:.3f}" if p else "n/a"
            dh = f"{(h - p[1]) / 5:.3f}" if p else "n/a"
            lines.append(f"| {a} | {d:.2f} | {h:.2f} | {dd} | {dh} | {ci:.2f} | {ratio:.3f} |")
    open(out, 'w').write("\n".join(lines) + "\n")
    print(f"wrote {out} ({len(rows)} series)")
if __name__ == '__main__':
    main(sys.argv[1], sys.argv[2])
