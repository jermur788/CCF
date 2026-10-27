#!/usr/bin/env python3
"""One-year basal-area loss of the pure stands in a DensityQualification long-run CSV (longrun-L40.csv / longrun-E80.csv).

Reads the harness column `deadBaFraction` (basal area of the stems that died this year / basal area of all living stems before the
step) and reports, for every pure scenario (A_OAK_DENSE, B_BEECH_DENSE, E_OAK_YOUNG, F_BEECH_YOUNG), the worst year from year 26 on,
the stand size in that year and the number of years above the generic step's basal-area budget.

The bound it checks is the planner's own rule (MixedDensityModel.GenericAnnualBasalAreaBudgetShare, 0.125 at the time of writing):
no year may lose more basal area than the budget, except that a stand of fewer than 9 stems is exempt (there one stem can exceed the
budget on its own and the planner takes it so that the generic component never stalls). Years 1-25 are reported separately: the
artificial dense start of the A/B fixtures is spread over several years instead of crashing in year 1, and is held to the same bound.

Mixed scenarios (C_, G_) are skipped: their deadBaFraction also contains the unchanged legacy Sitka component, which is not budgeted.

Usage: max_ba_loss.py <longrun-csv> [<longrun-csv> ...] [--budget 0.125] [--first-year 26]
Exit status 0 when no pure scenario exceeds the budget; 1 otherwise; 2 on unreadable input.
"""
import argparse
import csv
import sys

PURE = ("A_OAK_DENSE", "B_BEECH_DENSE", "E_OAK_YOUNG", "F_BEECH_YOUNG")


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("files", nargs="+")
    parser.add_argument("--budget", type=float, default=0.125)
    parser.add_argument("--first-year", type=int, default=26)
    args = parser.parse_args()
    violations = 0
    for path in args.files:
        try:
            with open(path, newline="") as handle:
                rows = list(csv.DictReader(handle))
        except OSError as error:
            print("cannot read", path, error)
            return 2
        if not rows or "deadBaFraction" not in rows[0]:
            print(path, "has no deadBaFraction column")
            return 2
        print(f"== {path}  budget {args.budget:.3f}")
        print("scenario        geo  worst year>=%d: BA loss (year, stems) | years over budget | worst year 1-%d | Y300 stems/BA/occupancy" % (args.first_year, args.first_year - 1))
        for name in PURE:
            mine = sorted((r for r in rows if r["scenario"] == name), key=lambda r: int(r["year"]))
            if not mine:
                continue
            late = [r for r in mine if int(r["year"]) >= args.first_year]
            early = [r for r in mine if int(r["year"]) < args.first_year]
            worst = max(late, key=lambda r: float(r["deadBaFraction"]))
            over = [r for r in mine if int(r["livingBefore"]) >= 9 and float(r["deadBaFraction"]) > args.budget + 1e-6]
            worst_early = max(early, key=lambda r: float(r["deadBaFraction"])) if early else None
            last = mine[-1]
            violations += len(over)
            print(f"{name:15s} {mine[0]['geo']}  {float(worst['deadBaFraction']):.3f} (Y{worst['year']}, n={worst['livingBefore']}) | {len(over)} | "
                  + (f"{float(worst_early['deadBaFraction']):.3f} (Y{worst_early['year']})" if worst_early else "-")
                  + f" | Y{last['year']}: {last['livingAfter']}/{float(last['baM2ha']):.1f}/{float(last['occupancyAfter']):.2f}")
    print("MAX_BA_LOSS_" + ("PASS" if violations == 0 else "FAIL") + f" yearsOverBudget={violations}")
    return 0 if violations == 0 else 1


if __name__ == "__main__":
    sys.exit(main())
