#!/usr/bin/env python3
"""Comparator: CCF aggregate yearly values vs BroadLynch2006_Sitka.

Ingests a CCF pure-Sitka yearly aggregate series (B basal area, N stocking,
H top height, V volume) and projects the published Broad & Lynch 2006 Sitka
stand model forward from the series' first row, reporting per-year divergence.
Comparison/validation only: it never edits CCF production mechanics.

INPUT FORMAT (documented for Tier-3)
------------------------------------
JSON:  {"meta": {...optional...},
        "series": [{"age": <yr>, "N": <stems/ha>, "B": <m2/ha>, "H": <m>, "V": <m3/ha optional>}, ...]}
CSV :  header row; recognised column aliases (case-insensitive):
         age/year/stand_age            -> age (years)
         stems_ha/stocking/n/trees_ha  -> N
         basal_area_m2_ha/basal_area/ba-> B
         top_height_m/top_height/h     -> H
         volume_m3_ha/volume/v         -> V (optional)
         run/series/label              -> optional series id (filter with --run)
Units: age years; N stems/ha; B m2/ha; H m; V m3/ha. Rows are sorted by age.

OUTPUT
------
<out>/comparison.json and <out>/comparison.md: anchor, inferred site index,
per-year CCF vs model B/N/H/V and % differences, and a summary (mean/max abs %).

Usage:
  compare_ccf.py --input <file.csv|file.json> --out <dir> [--run NAME]
                 [--model thinned|unthinned] [--csv]
"""
from __future__ import annotations

import argparse
import csv
import json
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import broad_lynch_2006_sitka as bl  # noqa: E402

ALIASES = {
    "age": {"age", "year", "stand_age", "ccf_year", "sim_year"},
    "N": {"stems_ha", "stocking", "stocking_ha", "n", "trees_ha", "living"},
    "B": {"basal_area_m2_ha", "basal_area", "ba", "b", "basalarea"},
    "H": {"top_height_m", "top_height", "h", "topheight"},
    "V": {"volume_m3_ha", "volume", "v", "vol"},
    "run": {"run", "series", "label", "scenario"},
}


def _canonical(header):
    out = {}
    for col in header:
        key = col.strip().lower()
        for target, names in ALIASES.items():
            if key in names:
                out[target] = col
                break
    return out


def load(path, run=None):
    text = Path(path).read_text()
    if path.endswith(".json") or text.lstrip().startswith("{"):
        data = json.loads(text)
        rows = data.get("series", data.get("rows", []))
        meta = data.get("meta", {})
        series = [{k: float(v) for k, v in r.items() if k in ("age", "N", "B", "H", "V")} for r in rows]
        return meta, series
    reader = csv.DictReader(text.splitlines())
    cols = _canonical(reader.fieldnames or [])
    required = {"age", "N", "B", "H"}
    missing = required - set(cols)
    if missing:
        raise SystemExit("STOP: input missing required columns for: " + ", ".join(sorted(missing))
                         + " (found " + ",".join(reader.fieldnames or []) + ")")
    series = []
    for row in reader:
        if run is not None and cols.get("run") and row.get(cols["run"]) != run:
            continue
        item = {"age": float(row[cols["age"]]), "N": float(row[cols["N"]]),
                "B": float(row[cols["B"]]), "H": float(row[cols["H"]])}
        if "V" in cols and row.get(cols["V"]) not in (None, ""):
            item["V"] = float(row[cols["V"]])
        series.append(item)
    if not series:
        raise SystemExit("STOP: no rows matched (run filter " + repr(run) + ")")
    return {"source": str(path), "run": run}, series


def compare(meta, series, model):
    series = sorted(series, key=lambda r: r["age"])
    anchor = series[0]
    si = bl.site_index_from_height(anchor["age"], anchor["H"])
    b = bl.b_from_site_index(si)
    state0 = [anchor["B"], anchor["N"], anchor["H"]]
    rows = []
    for r in series:
        dt = r["age"] - anchor["age"]
        B, N, H = bl.transition(state0, model, b, dt) if dt else state0
        V = bl.volume(B, N, H)
        bl_h = bl.height(r["age"], si)
        row = {"age": r["age"], "ccf": {"B": r["B"], "N": r["N"], "H": r["H"], "V": r.get("V")},
               "model": {"B": B, "N": N, "H": H, "V": V, "H_height_eq": bl_h}}
        for k in ("B", "N", "H", "V"):
            ccf = r.get(k) if k != "V" else r.get("V")
            mv = V if k == "V" else {"B": B, "N": N, "H": H}[k]
            row[f"pct_diff_{k}"] = (None if not ccf else 100.0 * (mv - ccf) / ccf)
        rows.append(row)
    summary = {}
    for k in ("B", "N", "H", "V"):
        vals = [abs(x[f"pct_diff_{k}"]) for x in rows if x[f"pct_diff_{k}"] is not None and x["age"] != anchor["age"]]
        summary[k] = {"n": len(vals), "mean_abs_pct": (sum(vals) / len(vals) if vals else None),
                      "max_abs_pct": (max(vals) if vals else None)}
    return {"meta": meta, "model": model["name"], "anchor_age": anchor["age"],
            "anchor_state": {"B": anchor["B"], "N": anchor["N"], "H": anchor["H"]},
            "site_index": si, "b": b, "rows": rows, "summary": summary,
            "caveat": "Broad & Lynch 2006 Sitka coefficients (BroadLynch2006_Sitka); not verified equal "
                      "to any later GROWFOR executable. Comparison only; no CCF change."}


def render_md(result):
    lines = [f"# CCF vs BroadLynch2006_Sitka ({result['model']})",
             "", f"- anchor age {result['anchor_age']} B={result['anchor_state']['B']} "
             f"N={result['anchor_state']['N']} H={result['anchor_state']['H']}",
             f"- inferred site index S={result['site_index']:.3f} (top height at age 30), b={result['b']:.6f}",
             "", f"- {result['caveat']}", "",
             "| age | CCF B | BL B | CCF N | BL N | CCF H | BL H | CCF V | BL V | dH% | dV% |",
             "|---|---|---|---|---|---|---|---|---|---|---|"]
    for r in result["rows"]:
        c, m = r["ccf"], r["model"]
        def f(v): return "" if v is None else f"{v:.2f}"
        lines.append(f"| {r['age']:.0f} | {f(c['B'])} | {f(m['B'])} | {f(c['N'])} | {f(m['N'])} | "
                     f"{f(c['H'])} | {f(m['H'])} | {f(c.get('V'))} | {f(m['V'])} | "
                     f"{('' if r['pct_diff_H'] is None else format(r['pct_diff_H'],'+.1f'))} | "
                     f"{('' if r['pct_diff_V'] is None else format(r['pct_diff_V'],'+.1f'))} |")
    s = result["summary"]
    lines += ["", "## Summary (abs % difference, excluding anchor)",
              "", "| var | n | mean | max |", "|---|---|---|---|"]
    for k in ("B", "N", "H", "V"):
        d = s[k]
        lines.append(f"| {k} | {d['n']} | "
                     f"{('-' if d['mean_abs_pct'] is None else format(d['mean_abs_pct'],'.1f'))} | "
                     f"{('-' if d['max_abs_pct'] is None else format(d['max_abs_pct'],'.1f'))} |")
    return "\n".join(lines) + "\n"


def main():
    p = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    p.add_argument("--input", required=True)
    p.add_argument("--out", required=True)
    p.add_argument("--run", default=None)
    p.add_argument("--model", choices=["thinned", "unthinned"], default="unthinned")
    a = p.parse_args()
    out = Path(a.out)
    out.mkdir(parents=True, exist_ok=True)
    meta, series = load(a.input, a.run)
    model = bl.THINNED if a.model == "thinned" else bl.UNTHINNED
    result = compare(meta, series, model)
    (out / "comparison.json").write_text(json.dumps(result, indent=2) + "\n")
    (out / "comparison.md").write_text(render_md(result))
    print(json.dumps({"out": str(out), "model": result["model"], "site_index": round(result["site_index"], 3),
                      "summary": result["summary"]}, indent=2))
    return 0


if __name__ == "__main__":
    sys.exit(main())
