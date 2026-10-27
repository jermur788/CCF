#!/usr/bin/env python3
"""Validate and summarise the density harness's per-component basal-area evidence (component-ba-<geo>.csv).

DensityQualification writes one machine-readable row per scenario-year during the LONGRUN phase: the basal area the Sitka
component and the generic component each removed, the combined total, and whether each fired. This reader:

  * rejects a malformed or incomplete file (wrong schema, missing/partial/duplicate scenario-year rows, non-numeric values,
    flags that are not 0/1, a component total that does not match the reported total, a zero component reported non-zero,
    a row the instrumentation could not attribute);
  * reports, per scenario, how many years each component fired, how many years both fired, and the total basal area each
    component removed, so Tier-3 can read the mixed-stand Sitka/generic split directly.

This is measurement, not an acceptance rule. The existing basal-area budget check (0.125, GenericAnnualBasalAreaBudgetShare)
is unchanged and stays in max_ba_loss.py.

Usage: component_ba.py <component-ba-<geo>.csv> [<component-ba-<geo>.csv> ...]
Exit status 0 when every file is complete and consistent; 1 on a failed or unreadable check.
"""
import argparse
import csv
import sys
from collections import OrderedDict
from pathlib import Path

# The shared receipt/evidence module is one directory up under Verification/Qualification.
sys.path.insert(0, str(Path(__file__).resolve().parents[2] / "Qualification"))
from qualification_results import COMPONENT_BA_COLUMNS, component_ba_row_problems  # noqa: E402


def check_file(path):
    """Return (rows, problems). rows is the parsed list; problems is a list of human-readable strings."""
    problems = []
    try:
        with Path(path).open(newline="") as handle:
            reader = csv.DictReader(handle)
            if reader.fieldnames != COMPONENT_BA_COLUMNS:
                return [], [f"schema mismatch: {reader.fieldnames}"]
            rows = list(reader)
    except OSError as error:
        return [], [f"cannot read: {error}"]
    if not rows:
        return rows, ["no component-ba rows"]
    seen = set()
    for row in rows:
        key = (row["scenario"], row["year"])
        if key in seen:
            problems.append(f"duplicate row {key}")
        seen.add(key)
        problems.extend(component_ba_row_problems(row))
        if len(problems) > 40:
            break
    # Per scenario the years must be a contiguous run starting at 1, with no silent gaps.
    years = OrderedDict()
    for row in rows:
        try:
            years.setdefault(row["scenario"], []).append(int(row["year"]))
        except (TypeError, ValueError):
            problems.append(f"{row['scenario']} Y{row['year']}: year is not an integer")
    for scenario, values in years.items():
        if sorted(values) != list(range(1, len(values) + 1)):
            problems.append(f"{scenario}: non-contiguous years {sorted(values)[:5]}... ({len(values)} rows)")
    return rows, problems


def summarise(rows):
    totals = OrderedDict()
    for row in rows:
        entry = totals.setdefault(row["scenario"], {"rows": 0, "sitkaFired": 0, "genericFired": 0, "bothFired": 0,
                                                    "sitkaBaLossM2": 0.0, "genericBaLossM2": 0.0, "combinedBaLossM2": 0.0})
        entry["rows"] += 1
        entry["sitkaFired"] += 1 if row["sitkaFired"] == "1" else 0
        entry["genericFired"] += 1 if row["genericFired"] == "1" else 0
        entry["bothFired"] += 1 if row["bothFired"] == "1" else 0
        entry["sitkaBaLossM2"] += float(row["sitkaBaLossM2"])
        entry["genericBaLossM2"] += float(row["genericBaLossM2"])
        entry["combinedBaLossM2"] += float(row["combinedBaLossM2"])
    return totals


def main():
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    parser.add_argument("files", nargs="+")
    args = parser.parse_args()
    failures = 0
    total_rows = 0
    for path in args.files:
        rows, problems = check_file(path)
        total_rows += len(rows)
        print(f"== {path}  rows={len(rows)}")
        if not problems:
            for scenario, entry in summarise(rows).items():
                print(f"   {scenario:22s} rows={entry['rows']:4d} bothFiredYears={entry['bothFired']:4d} "
                      f"sitkaFiredYears={entry['sitkaFired']:4d} genericFiredYears={entry['genericFired']:4d} "
                      f"baSitkaM2={entry['sitkaBaLossM2']:.3f} baGenericM2={entry['genericBaLossM2']:.3f} "
                      f"baCombinedM2={entry['combinedBaLossM2']:.3f}")
        else:
            failures += 1
            for problem in problems[:20]:
                print("   FAIL " + problem)
    print("COMPONENT_BA_" + ("PASS" if failures == 0 else "FAIL") + f" files={len(args.files)} rows={total_rows} failedFiles={failures}")
    return 0 if failures == 0 else 1


if __name__ == "__main__":
    sys.exit(main())
