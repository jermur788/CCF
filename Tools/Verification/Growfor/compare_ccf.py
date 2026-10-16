#!/usr/bin/env python3
"""Comparator: CCF aggregate yearly values vs BroadLynch2006_Sitka.

Ingests a CCF pure-Sitka yearly aggregate series (B basal area, N stocking,
H top height, V volume) and projects the published Broad & Lynch 2006 Sitka
stand model forward from the series' first row, reporting per-year divergence.
Comparison/validation only: it never edits CCF production mechanics.

INPUT FORMAT (documented for Tier-3)
------------------------------------
Canonical column/key names (units are part of the name; no ambiguous short
aliases are accepted):

    age              years
    stems_ha         stems/ha
    basal_area_m2_ha m2/ha
    top_height_m     metres
    volume_m3_ha     m3/ha (optional)
    run              optional series id (select with --run)

Accepted unambiguous synonyms: age_years/stand_age; stems_per_ha/trees_ha/
stocking_ha; basal_area; top_height/height_m; volume. Short or unit-ambiguous
names (n, b, h, v, ba, stocking) are REJECTED, as are unknown columns, two
columns mapping to the same dimension, and non-finite values.

JSON:  {"meta": {...optional, "run": <id>...},
        "series": [{"age": <yr>, "stems_ha": <stems/ha>, "basal_area_m2_ha": <m2/ha>,
                    "top_height_m": <m>, "volume_m3_ha": <m3/ha optional>}, ...]}
CSV :  header row using the canonical/synonym names above.

Ages must be strictly increasing with no duplicates and at least two rows.

INITIALIZATION (explicit, not fitted)
-------------------------------------
The site index S is inferred from a SINGLE observed (age, top height) pair - the
first row - by inverting the polymorphic Bertalanffy-Richards height equation;
S is top height at age 30. S is never fitted across the trajectory and the model
is never calibrated to minimise disagreement. Output metadata records the exact
observation, the resulting S, and the method.

OUTPUT
------
<out>/comparison.json and <out>/comparison.md: initialization metadata, per-year
CCF vs model B/N/H/V, % differences (denominator defined explicitly; undefined
percentages reported, never coerced to zero), domain flags for nonphysical model
rows, and a summary (mean/max abs % over defined, in-domain rows only).

Usage:
  compare_ccf.py --input <file.csv|file.json> --out <dir> [--run NAME]
                 [--model thinned|unthinned]
"""
from __future__ import annotations

import argparse
import csv
import json
import math
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import broad_lynch_2006_sitka as bl  # noqa: E402

# Explicit, unit-bearing names only. Each column name maps to exactly one dimension.
CANONICAL_NAMES = {
    "age": {"age", "age_years", "stand_age"},
    "stems_ha": {"stems_ha", "stems_per_ha", "trees_ha", "stocking_ha"},
    "basal_area_m2_ha": {"basal_area_m2_ha", "basal_area"},
    "top_height_m": {"top_height_m", "top_height", "height_m"},
    "volume_m3_ha": {"volume_m3_ha", "volume"},
    "run": {"run", "series_id", "scenario", "series"},
}
REQUIRED = ("age", "stems_ha", "basal_area_m2_ha", "top_height_m")
_NAME_TO_DIMENSION = {name: dim for dim, names in CANONICAL_NAMES.items() for name in names}

# Broad & Lynch 2006 volume equation definition (the source specifies a stand-volume equation with a
# measured small-end convention); this is a DIAGNOSTIC comparator, not CCF biological stem volume.
VOLUME_DEFINITION = ("Broad & Lynch 2006 stand-volume equation (source's own small-end/utilisation "
                     "convention, 7 cm small-end diameter as identified by review); NOT asserted "
                     "identical to CCF's biological stem-volume measure.")
DOMAIN_NOTE = ("The published 2006 volume equation has no protective low-stock asymptote; at very low "
               "stocking it returns nonphysical (often negative) volume. Such rows are flagged "
               "OUTSIDE_USEFUL_DOMAIN and excluded from percentage summaries rather than clamped.")
INITIALIZATION_METHOD = ("Single observed anchor (age, top height) inverted through the polymorphic "
                         "Bertalanffy-Richards height equation: "
                         "b = -ln(1-(H/a)^c)/(age-t0); S = a[1-(1-(H0/a)^c)e^{-b(30-t0)}]^{1/c}.")


def _finite(name, value, where=""):
    try:
        number = float(value)
    except (TypeError, ValueError):
        raise SystemExit(f"STOP: {where}{name} is not a number: {value!r}")
    if not math.isfinite(number):
        raise SystemExit(f"STOP: {where}{name} is not finite: {value!r}")
    return number


def _canonical(header, where=""):
    """Map header names to canonical dimensions; reject unknown, ambiguous or duplicated columns."""
    out = {}
    for column in header:
        key = str(column).strip().lower()
        dimension = _NAME_TO_DIMENSION.get(key)
        if dimension is None:
            raise SystemExit(f"STOP: {where}unrecognised column {column!r}; use explicit names such as "
                             "age, stems_ha, basal_area_m2_ha, top_height_m, volume_m3_ha")
        if dimension in out:
            raise SystemExit(f"STOP: {where}two columns map to {dimension} ({out[dimension]!r} and {column!r})")
        out[dimension] = column
    missing = [name for name in REQUIRED if name not in out]
    if missing:
        raise SystemExit(f"STOP: {where}input missing required columns for: " + ", ".join(missing)
                         + " (found " + ",".join(str(c) for c in header) + ")")
    return out


def _validate_series(series):
    """Reject empty/insufficient series and duplicate or non-monotonic ages. Return sorted-by-input order."""
    if not series:
        raise SystemExit("STOP: empty series (no rows)")
    if len(series) < 2:
        raise SystemExit("STOP: insufficient series: a forward comparison needs the anchor plus at least one later row")
    ages = [row["age"] for row in series]
    for previous, current in zip(ages, ages[1:]):
        if current == previous:
            raise SystemExit(f"STOP: duplicate age {current!r} in series")
        if current < previous:
            raise SystemExit(f"STOP: ages are not strictly increasing ({previous!r} then {current!r}); "
                             "refusing to silently reorder the series")
    return series


def _read_json(text, run):
    try:
        data = json.loads(text)
    except json.JSONDecodeError as error:
        raise SystemExit(f"STOP: invalid JSON input: {error}")
    rows = data.get("series")
    if rows is None:
        raise SystemExit("STOP: JSON input must contain a 'series' array")
    meta = data.get("meta", {}) or {}
    if run is not None:
        meta_run = meta.get("run")
        if meta_run is None:
            raise SystemExit(f"STOP: --run {run!r} requested but the input has no meta.run")
        if str(meta_run) != run:
            raise SystemExit(f"STOP: unknown run {run!r}; input meta.run is {meta_run!r}")
    series = []
    for index, raw in enumerate(rows):
        cols = _canonical(list(raw.keys()), where=f"series[{index}]: ")
        item = {"age": _finite(cols["age"], raw[cols["age"]], f"series[{index}]: "),
                "stems_ha": _finite(cols["stems_ha"], raw[cols["stems_ha"]], f"series[{index}]: "),
                "basal_area_m2_ha": _finite(cols["basal_area_m2_ha"], raw[cols["basal_area_m2_ha"]], f"series[{index}]: "),
                "top_height_m": _finite(cols["top_height_m"], raw[cols["top_height_m"]], f"series[{index}]: ")}
        if "volume_m3_ha" in cols and raw[cols["volume_m3_ha"]] not in (None, ""):
            item["volume_m3_ha"] = _finite(cols["volume_m3_ha"], raw[cols["volume_m3_ha"]], f"series[{index}]: ")
        series.append(item)
    return {"source": "<json>", "run": run if run is not None else meta.get("run")}, series


def load(path, run=None):
    text = Path(path).read_text()
    is_json = path.endswith(".json") or text.lstrip().startswith("{")
    if is_json:
        meta, series = _read_json(text, run)
        meta["source"] = str(path)
        return meta, _validate_series(series)

    reader = csv.DictReader(text.splitlines())
    where = ""
    cols = _canonical(reader.fieldnames or [], where=where)
    rows = list(reader)
    distinct_runs = None
    if "run" in cols:
        distinct_runs = {row[cols["run"]] for row in rows}
        if run is not None:
            if run not in distinct_runs:
                raise SystemExit(f"STOP: unknown run {run!r}; input has {sorted(distinct_runs)!r}")
        elif len(distinct_runs) > 1:
            raise SystemExit("STOP: input contains multiple runs " + repr(sorted(distinct_runs))
                             + "; select one with --run")
    elif run is not None:
        raise SystemExit(f"STOP: --run {run!r} requested but the input has no run column")
    series = []
    for index, row in enumerate(rows):
        if "run" in cols and run is not None and row[cols["run"]] != run:
            continue
        item = {"age": _finite(cols["age"], row[cols["age"]], f"row {index}: "),
                "stems_ha": _finite(cols["stems_ha"], row[cols["stems_ha"]], f"row {index}: "),
                "basal_area_m2_ha": _finite(cols["basal_area_m2_ha"], row[cols["basal_area_m2_ha"]], f"row {index}: "),
                "top_height_m": _finite(cols["top_height_m"], row[cols["top_height_m"]], f"row {index}: ")}
        if "volume_m3_ha" in cols and row.get(cols["volume_m3_ha"]) not in (None, ""):
            item["volume_m3_ha"] = _finite(cols["volume_m3_ha"], row[cols["volume_m3_ha"]], f"row {index}: ")
        series.append(item)
    return {"source": str(path), "run": run}, _validate_series(series)


def _pct(ccf_value, model_value):
    """Percentage difference with an explicit denominator. Returns (value, reason); undefined -> (None, reason)."""
    if ccf_value is None:
        return None, "no_ccf_observation"
    if not math.isfinite(ccf_value):
        return None, "ccf_nonfinite"
    if ccf_value == 0.0:
        return None, "ccf_denominator_zero"
    if not math.isfinite(model_value):
        return None, "model_nonfinite"
    return 100.0 * (model_value - ccf_value) / ccf_value, None


def compare(meta, series, model):
    anchor = series[0]
    si = bl.site_index_from_height(anchor["age"], anchor["top_height_m"])
    b = bl.b_from_site_index(si)
    state0 = [anchor["basal_area_m2_ha"], anchor["stems_ha"], anchor["top_height_m"]]
    rows = []
    for r in series:
        dt = r["age"] - anchor["age"]
        B, N, H = bl.transition(state0, model, b, dt) if dt else state0
        V = bl.volume(B, N, H)
        finite_model = all(math.isfinite(v) for v in (B, N, H, V))
        domain = "ok" if (finite_model and B > 0.0 and N > 0.0 and V >= 0.0) else "OUTSIDE_USEFUL_DOMAIN"
        row = {"age": r["age"], "domain": domain,
               "ccf": {"B": r["basal_area_m2_ha"], "N": r["stems_ha"], "H": r["top_height_m"], "V": r.get("volume_m3_ha")},
               "model": {"B": B, "N": N, "H": H, "V": V, "H_height_eq": bl.height(r["age"], si)}}
        for key, ccf_value, model_value in (("B", r["basal_area_m2_ha"], B), ("N", r["stems_ha"], N),
                                            ("H", r["top_height_m"], H), ("V", r.get("volume_m3_ha"), V)):
            value, reason = _pct(ccf_value, model_value)
            row[f"pct_diff_{key}"] = value
            row[f"pct_reason_{key}"] = reason
        rows.append(row)

    summary = {}
    for key in ("B", "N", "H", "V"):
        values, undefined = [], 0
        for row in rows:
            if row["age"] == anchor["age"]:
                continue
            if row["domain"] != "ok":
                continue
            if row[f"pct_diff_{key}"] is None:
                undefined += 1
            else:
                values.append(abs(row[f"pct_diff_{key}"]))
        summary[key] = {"n_defined": len(values), "n_undefined": undefined,
                        "mean_abs_pct": (sum(values) / len(values) if values else None),
                        "max_abs_pct": (max(values) if values else None)}
    nonphysical = [row["age"] for row in rows if row["domain"] != "ok"]
    undefined_total = {key: sum(1 for row in rows if row["age"] != anchor["age"] and row[f"pct_reason_{key}"] is not None)
                       for key in ("B", "N", "H", "V")}
    return {
        "meta": meta, "model": model["name"],
        "initialization": {"anchor_age": anchor["age"], "anchor_top_height_m": anchor["top_height_m"],
                           "reference_age": bl.SITE_REFERENCE_AGE, "site_index_S": si, "b": b,
                           "method": INITIALIZATION_METHOD,
                           "note": "S inferred from one observation; not fitted across the trajectory; model not calibrated to CCF"},
        "anchor_age": anchor["age"],
        "anchor_state": {"B": anchor["basal_area_m2_ha"], "N": anchor["stems_ha"], "H": anchor["top_height_m"]},
        "site_index": si, "b": b, "rows": rows, "summary": summary,
        "undefined_percentages": undefined_total,
        "nonphysical_rows": nonphysical,
        "percent_denominator": "CCF observed value for the same age and variable; undefined where the CCF value is absent, non-finite or zero",
        "volume_definition": VOLUME_DEFINITION,
        "domain_note": DOMAIN_NOTE,
        "caveats": [
            "Broad & Lynch 2006 Sitka coefficients only; not verified equal to any later GROWFOR executable.",
            "No Oak/Beech coefficient sets, no mixed-species validation, no empirical irregular/CCF validation established here.",
            "No supported numeric B/N/H validity box is published; projections are adequate only within the source fit range.",
            VOLUME_DEFINITION,
            DOMAIN_NOTE,
            "Diagnostic envelope, not a production engine and not an acceptance ground truth.",
        ],
    }


def render_md(result):
    init = result["initialization"]
    lines = [f"# CCF vs BroadLynch2006_Sitka ({result['model']})", "",
             f"- anchor age {init['anchor_age']} top-height {init['anchor_top_height_m']:.2f} m "
             f"B={result['anchor_state']['B']} N={result['anchor_state']['N']}",
             f"- inferred site index S={init['site_index_S']:.5f} (top height at age {init['reference_age']:.0f}), "
             f"b={init['b']:.6f}",
             f"- initialization: {init['method']}",
             f"- {init['note']}", "",
             f"- percentage denominator: {result['percent_denominator']}", "",
             f"- volume definition: {result['volume_definition']}", "",
             f"- domain: {result['domain_note']}", "",
             "| age | CCF B | BL B | CCF N | BL N | CCF H | BL H | CCF V | BL V | dH% | dV% | domain |",
             "|---|---|---|---|---|---|---|---|---|---|---|---|"]
    def f(v):
        return "" if v is None else f"{v:.2f}"
    def p(v):
        return "" if v is None else format(v, "+.1f")
    for r in result["rows"]:
        c, mo = r["ccf"], r["model"]
        lines.append(f"| {r['age']:.0f} | {f(c['B'])} | {f(mo['B'])} | {f(c['N'])} | {f(mo['N'])} | "
                     f"{f(c['H'])} | {f(mo['H'])} | {f(c.get('V'))} | {f(mo['V'])} | "
                     f"{p(r['pct_diff_H'])} | {p(r['pct_diff_V'])} | {r['domain']} |")
    s = result["summary"]
    lines += ["", "## Summary (abs % over defined, in-domain rows, excluding anchor)",
              "", "| var | n defined | n undefined | mean | max |", "|---|---|---|---|---|"]
    for k in ("B", "N", "H", "V"):
        d = s[k]
        lines.append(f"| {k} | {d['n_defined']} | {d['n_undefined']} | "
                     f"{('-' if d['mean_abs_pct'] is None else format(d['mean_abs_pct'],'.1f'))} | "
                     f"{('-' if d['max_abs_pct'] is None else format(d['max_abs_pct'],'.1f'))} |")
    if result["nonphysical_rows"]:
        lines += ["", "## Rows outside the useful domain (raw model output, not clamped)",
                  "", "ages: " + ", ".join(f"{a:.0f}" for a in result["nonphysical_rows"])]
    lines += ["", "## Caveats", ""] + [f"- {c}" for c in result["caveats"]]
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
    print(json.dumps({"out": str(out), "model": result["model"], "site_index": round(result["site_index"], 5),
                      "nonphysical_rows": len(result["nonphysical_rows"]), "summary": result["summary"]}, indent=2))
    return 0


if __name__ == "__main__":
    sys.exit(main())
