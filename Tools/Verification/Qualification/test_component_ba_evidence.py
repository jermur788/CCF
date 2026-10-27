"""Offline/static tests for the density harness's per-component basal-area evidence.

They exercise qualification_results.read_component_ba_evidence (the same reader the qualification runner uses) on synthetic
component-ba-<geo>.csv files, without Unity. They prove the five properties the instrumentation must satisfy:

  * component total == reported total (combinedBaLossM2 == deadBaM2, and the fractions agree);
  * a zero component is reported as zero, and a component that fires is positive;
  * a both-component (Sitka AND generic) row is representable and counted;
  * measurement rows cannot silently disappear (missing, partial, duplicate or extra scenario-year rows fail);
  * malformed/incomplete rows fail downstream analysis (bad schema, non-numeric, non-finite, bad flags, unattributable).

No density threshold is applied. Run:  python3 -m unittest Tools.Verification... or unittest discover -s this directory.
"""
import csv
from pathlib import Path
import re
import subprocess
import tempfile
import unittest

from qualification_results import (COMPONENT_BA_COLUMNS, COMPONENT_BA_FLOAT_SLACK, COMPONENT_BA_QUANTUM,
                                   component_ba_fraction_tolerance, component_ba_row_problems,
                                   component_ba_sum_tolerance, component_ba_total_tolerance, read_component_ba_evidence)

CLI = Path(__file__).resolve().parents[1] / "ThreeSpecies/Qualification/component_ba.py"
HARNESS = Path(__file__).resolve().parents[1] / "ThreeSpecies/Qualification/DensityQualification.cs"


def component_ba_row(scenario="C_MIXED_DENSE", geo="E80", year=1, route="SpeciesAwareOccupancy",
                     living=1000, ba_pre=50.0, sitka_deaths=4, generic_deaths=6, sitka_ba=1.5, generic_ba=0.5,
                     dead_ba=None, exact=1):
    """One schema-complete row. dead_ba defaults to the component total, so the default row is internally consistent."""
    combined = sitka_ba + generic_ba
    if dead_ba is None:
        dead_ba = combined
    deaths = sitka_deaths + generic_deaths
    frac = (lambda value: f"{value / ba_pre:.5f}" if ba_pre > 0 else "0.00000")
    return {
        "scenario": scenario, "geo": geo, "year": str(year), "route": route, "livingBefore": str(living),
        "baPreM2": f"{ba_pre:.5f}", "densityDeaths": str(deaths),
        "sitkaComponentDeaths": str(sitka_deaths), "genericDeaths": str(generic_deaths),
        "sitkaBaLossM2": f"{sitka_ba:.5f}", "genericBaLossM2": f"{generic_ba:.5f}", "combinedBaLossM2": f"{combined:.5f}",
        "sitkaBaFraction": frac(sitka_ba), "genericBaFraction": frac(generic_ba), "combinedBaFraction": frac(combined),
        "deadBaM2": f"{dead_ba:.5f}", "deadBaFraction": frac(dead_ba),
        "sitkaFired": "1" if sitka_deaths > 0 else "0", "genericFired": "1" if generic_deaths > 0 else "0",
        "bothFired": "1" if (sitka_deaths > 0 and generic_deaths > 0) else "0", "attributionExact": str(exact),
    }


def write_csv(path, rows, header=COMPONENT_BA_COLUMNS):
    with Path(path).open("w", newline="") as handle:
        writer = csv.DictWriter(handle, fieldnames=header)
        writer.writeheader()
        for row in rows:
            writer.writerow(row)


def quantised_row(s, g, combined, ba_pre=42.13765, dead=None,
                  sitka_fraction=None, generic_fraction=None, combined_fraction=None, dead_fraction=None):
    """A both-fired row whose fields are supplied as explicit serialised strings, so the 5-decimal representation
    relationship (for example sitka+generic = 0.11794 while combined = 0.11793) can be set exactly. Fractions default to
    five-decimal roundings of the parsed BA over the parsed denominator."""
    sp, gp, cp, bp = float(s), float(g), float(combined), float(ba_pre)
    dp = cp if dead is None else float(dead)
    frac = lambda value: f"{value / bp:.5f}" if bp > 0 else "0.00000"
    return {
        "scenario": "C_MIXED_DENSE", "geo": "E80", "year": "1", "route": "SpeciesAwareOccupancy", "livingBefore": "1000",
        "baPreM2": f"{bp:.5f}", "densityDeaths": "5", "sitkaComponentDeaths": "3", "genericDeaths": "2",
        "sitkaBaLossM2": f"{sp:.5f}", "genericBaLossM2": f"{gp:.5f}", "combinedBaLossM2": f"{cp:.5f}",
        "sitkaBaFraction": frac(sp) if sitka_fraction is None else sitka_fraction,
        "genericBaFraction": frac(gp) if generic_fraction is None else generic_fraction,
        "combinedBaFraction": frac(cp) if combined_fraction is None else combined_fraction,
        "deadBaM2": f"{dp:.5f}", "deadBaFraction": frac(dp) if dead_fraction is None else dead_fraction,
        "sitkaFired": "1", "genericFired": "1", "bothFired": "1", "attributionExact": "1",
    }


MIXED_ENV = {"CCF_QUAL_PHASES": "LONGRUN", "CCF_QUAL_REPLAY": "0", "CCF_QUAL_SCENARIOS": "C", "CCF_STAND_GEOMETRY": "1"}
MIXED_LOG = "QUAL_START geometry=E80 phases=LONGRUN"


class ComponentBaEvidenceTests(unittest.TestCase):
    def test_project_total_equals_reported_total_and_both_components_representable(self):
        with tempfile.TemporaryDirectory(prefix="ccf-component-ba-") as tmp:
            rows = [component_ba_row(year=year) for year in range(1, 101)]
            write_csv(Path(tmp) / "component-ba-E80.csv", rows)
            evidence = read_component_ba_evidence(tmp, MIXED_ENV, MIXED_LOG)
            self.assertTrue(evidence["complete"], evidence["error"])
            self.assertEqual(100, evidence["rows"])
            self.assertEqual(100, evidence["both_fired_rows"])
            self.assertEqual(0, evidence["attribution_mismatch_rows"])

    def test_component_total_mismatch_with_reported_total_fails(self):
        with tempfile.TemporaryDirectory(prefix="ccf-component-ba-") as tmp:
            rows = [component_ba_row(year=year) for year in range(1, 101)]
            rows[0]["combinedBaLossM2"] = "9.50000"          # sum no longer matches the parts
            write_csv(Path(tmp) / "component-ba-E80.csv", rows)
            evidence = read_component_ba_evidence(tmp, MIXED_ENV, MIXED_LOG)
            self.assertFalse(evidence["complete"])
            self.assertIn("combinedBaLossM2", evidence["error"])

    def test_zero_components_are_reported_as_zero_and_fired_components_positive(self):
        env = dict(MIXED_ENV, CCF_QUAL_SCENARIOS="A,D")     # A pure Oak (generic only), D pure Sitka model 2 (Sitka only)
        with tempfile.TemporaryDirectory(prefix="ccf-component-ba-") as tmp:
            rows = [component_ba_row("A_OAK_DENSE", year=year, sitka_deaths=0, sitka_ba=0.0, generic_deaths=6, generic_ba=0.5) for year in range(1, 101)]
            rows += [component_ba_row("D_SITKA_DENSE_MODEL2", year=year, route="LegacySitkaExact", sitka_deaths=8, sitka_ba=2.0, generic_deaths=0, generic_ba=0.0) for year in range(1, 101)]
            write_csv(Path(tmp) / "component-ba-E80.csv", rows)
            evidence = read_component_ba_evidence(tmp, env, MIXED_LOG)
            self.assertTrue(evidence["complete"], evidence["error"])
            self.assertEqual(100, evidence["generic_fired_rows"])
            self.assertEqual(100, evidence["sitka_fired_rows"])
            self.assertEqual(0, evidence["both_fired_rows"])

    def test_component_claimed_not_fired_but_nonzero_fails(self):
        with tempfile.TemporaryDirectory(prefix="ccf-component-ba-") as tmp:
            rows = [component_ba_row(year=year) for year in range(1, 101)]
            rows[3]["genericFired"] = "0"                      # generic still killed 0.5 m2
            write_csv(Path(tmp) / "component-ba-E80.csv", rows)
            evidence = read_component_ba_evidence(tmp, MIXED_ENV, MIXED_LOG)
            self.assertFalse(evidence["complete"])
            self.assertIn("generic", evidence["error"])

    def test_missing_partial_duplicate_or_extra_rows_cannot_disappear(self):
        with tempfile.TemporaryDirectory(prefix="ccf-component-ba-") as tmp:
            valid = [component_ba_row(year=year) for year in range(1, 101)]
            for bad in ([], valid[:99], valid + [valid[0]], [dict(row, scenario="G_MIXED_YOUNG") for row in valid]):
                with self.subTest(rows=len(bad)):
                    write_csv(Path(tmp) / "component-ba-E80.csv", bad)
                    evidence = read_component_ba_evidence(tmp, MIXED_ENV, MIXED_LOG)
                    self.assertFalse(evidence["complete"])

    def test_unattributable_row_fails(self):
        with tempfile.TemporaryDirectory(prefix="ccf-component-ba-") as tmp:
            rows = [component_ba_row(year=year) for year in range(1, 101)]
            rows[7]["attributionExact"] = "0"
            write_csv(Path(tmp) / "component-ba-E80.csv", rows)
            evidence = read_component_ba_evidence(tmp, MIXED_ENV, MIXED_LOG)
            self.assertFalse(evidence["complete"])
            self.assertEqual(1, evidence["attribution_mismatch_rows"])

    def test_malformed_and_incomplete_rows_fail(self):
        with tempfile.TemporaryDirectory(prefix="ccf-component-ba-") as tmp:
            path = Path(tmp) / "component-ba-E80.csv"
            valid = [component_ba_row(year=year) for year in range(1, 101)]
            cases = {
                "non-numeric": {8: ("deadBaM2", "not-a-number")},
                "non-finite": {8: ("deadBaM2", "nan")},
                "bad-flag": {8: ("sitkaFired", "2")},
                "count-mismatch": {8: ("densityDeaths", "99")},
            }
            for label, edits in cases.items():
                with self.subTest(case=label):
                    rows = [dict(row) for row in valid]
                    for index, (field, value) in edits.items():
                        rows[index][field] = value
                    write_csv(path, rows)
                    self.assertFalse(read_component_ba_evidence(tmp, MIXED_ENV, MIXED_LOG)["complete"], label)
            # wrong header (schema mismatch): raw text, so the reader rejects the file rather than the writer
            path.write_text("scenario,geo,year,not-the-real-schema\nC_MIXED_DENSE,E80,1,0\n")
            evidence = read_component_ba_evidence(tmp, MIXED_ENV, MIXED_LOG)
            self.assertFalse(evidence["complete"])
            self.assertIn("schema", evidence["error"])
            # missing file
            path.unlink()
            self.assertFalse(read_component_ba_evidence(tmp, MIXED_ENV, MIXED_LOG)["complete"])


    def test_harness_header_matches_the_python_schema_and_is_wired(self):
        source = HARNESS.read_text()
        match = re.search(r'private const string ComponentBaHeader = "([^"]+)";', source)
        self.assertIsNotNone(match, "the harness must declare the component-ba schema")
        self.assertEqual(match.group(1), ",".join(COMPONENT_BA_COLUMNS))
        # The harness must write the file, record a row per year, and refuse to pass on short output.
        self.assertIn('Csv($"component-ba-{geo}.csv", ComponentBaHeader)', source)
        self.assertIn("result.ComponentRows.Add(", source)
        self.assertIn("Incomplete component-ba measurements", source)

    def test_quantisation_tolerances_are_derived_from_the_five_decimal_quantum(self):
        self.assertEqual(1e-5, COMPONENT_BA_QUANTUM)
        # three independently rounded BA fields; two independently rounded totals; fraction plus rounded numerator;
        # each with only a tiny floating-point slack on top of the quantum term.
        self.assertAlmostEqual(1.5 * COMPONENT_BA_QUANTUM + COMPONENT_BA_FLOAT_SLACK, component_ba_sum_tolerance(), places=15)
        self.assertAlmostEqual(COMPONENT_BA_QUANTUM + COMPONENT_BA_FLOAT_SLACK, component_ba_total_tolerance(), places=15)
        self.assertAlmostEqual(0.5 * COMPONENT_BA_QUANTUM + COMPONENT_BA_FLOAT_SLACK + 0.5 * COMPONENT_BA_QUANTUM / 42.0,
                               component_ba_fraction_tolerance(42.0), places=15)
        self.assertAlmostEqual(0.5 * COMPONENT_BA_QUANTUM + COMPONENT_BA_FLOAT_SLACK,
                               component_ba_fraction_tolerance(0.0), places=15)
        # The tolerances must still be far below any material discrepancy.
        self.assertLess(component_ba_sum_tolerance(), 1e-4)
        self.assertLess(component_ba_total_tolerance(), 1e-4)

    def test_A_valid_quantised_both_fired_row_is_accepted(self):
        # The review's example: sitka+generic independently round to 0.11794 while combined rounds to 0.11793, and the
        # underlying unrounded values are self-consistent.
        row = quantised_row("0.04532", "0.07261", "0.11794")
        self.assertEqual(0.11793, float(row["sitkaBaLossM2"]) + float(row["genericBaLossM2"]))
        self.assertEqual("0.11794", row["combinedBaLossM2"])
        self.assertEqual([], component_ba_row_problems(row))

    def test_B_discrepancy_at_the_representation_bound_is_accepted(self):
        row = quantised_row("0.04532", "0.07261", "0.11793")
        row["combinedBaLossM2"] = "0.117915"       # |0.117915 - 0.11793| = 1.5e-5, exactly the derived bound
        row["deadBaM2"] = "0.117915"
        row["combinedBaFraction"] = f"{0.117915 / float(row['baPreM2']):.5f}"
        row["deadBaFraction"] = row["combinedBaFraction"]
        self.assertEqual([], component_ba_row_problems(row))

    def test_C_discrepancy_beyond_the_representation_bound_is_rejected(self):
        row = quantised_row("0.04532", "0.07261", "0.11793")
        row["combinedBaLossM2"] = "0.11803"        # 1e-4, materially beyond 1.5e-5
        row["deadBaM2"] = "0.11803"
        row["combinedBaFraction"] = f"{0.11803 / float(row['baPreM2']):.5f}"
        row["deadBaFraction"] = row["combinedBaFraction"]
        problems = component_ba_row_problems(row)
        self.assertTrue(problems)
        self.assertTrue(any("combinedBaLossM2" in problem for problem in problems), problems)

    def test_D_fraction_rounding_within_the_derived_bound_is_accepted(self):
        # Small stand basal area makes the numerator-rounding term visible: the derived bound is 1.05e-4, well above the
        # old flat 1e-5. combined/baPreM2 = 0.6 and the stored fraction is 5e-5 away.
        row = quantised_row("0.01000", "0.02000", "0.03000", ba_pre=0.05000,
                            combined_fraction="0.60005", dead_fraction="0.60005")
        self.assertLess(abs(0.60005 - 0.03 / 0.05), component_ba_fraction_tolerance(0.05))
        self.assertEqual([], component_ba_row_problems(row))

    def test_E_fraction_mismatch_beyond_the_derived_bound_is_rejected(self):
        row = quantised_row("0.01000", "0.02000", "0.03000", ba_pre=0.05000,
                            combined_fraction="0.65000", dead_fraction="0.65000")
        problems = component_ba_row_problems(row)
        self.assertTrue(problems)
        self.assertTrue(any("combinedBaFraction" in problem for problem in problems), problems)

    def test_full_quantised_both_fired_evidence_file_is_accepted(self):
        # Regression for the review's blocker: 100 both-fired years with the 0.11794/0.11793 representation relationship
        # must read as complete evidence, not be rejected by over-tight tolerances.
        with tempfile.TemporaryDirectory(prefix="ccf-component-ba-") as tmp:
            rows = [dict(quantised_row("0.04532", "0.07261", "0.11794"), year=str(year), scenario="C_MIXED_DENSE")
                    for year in range(1, 101)]
            write_csv(Path(tmp) / "component-ba-E80.csv", rows)
            evidence = read_component_ba_evidence(tmp, MIXED_ENV, MIXED_LOG)
            self.assertTrue(evidence["complete"], evidence["error"])
            self.assertEqual(100, evidence["both_fired_rows"])
            # The CLI must accept the same file (it shares component_ba_row_problems).
            run = subprocess.run(["python3", "-B", str(CLI), str(Path(tmp) / "component-ba-E80.csv")],
                                 capture_output=True, text=True)
            self.assertEqual(0, run.returncode, run.stdout + run.stderr)
            self.assertIn("COMPONENT_BA_PASS", run.stdout)

    def test_downstream_cli_accepts_complete_and_rejects_incomplete(self):
        with tempfile.TemporaryDirectory(prefix="ccf-component-ba-") as tmp:
            good = Path(tmp) / "component-ba-E80.csv"
            write_csv(good, [component_ba_row(year=year) for year in range(1, 101)])
            run = subprocess.run(["python3", "-B", str(CLI), str(good)], capture_output=True, text=True)
            self.assertEqual(0, run.returncode, run.stderr)
            self.assertIn("COMPONENT_BA_PASS", run.stdout)
            self.assertIn("C_MIXED_DENSE", run.stdout)
            self.assertIn("bothFiredYears=", run.stdout)
            bad = Path(tmp) / "component-ba-E80.csv"
            rows = [component_ba_row(year=year) for year in range(1, 101)]
            rows[0]["genericBaLossM2"] = "0.70000"          # breaks the component total
            write_csv(bad, rows)
            run = subprocess.run(["python3", "-B", str(CLI), str(bad)], capture_output=True, text=True)
            self.assertEqual(1, run.returncode)
            self.assertIn("COMPONENT_BA_FAIL", run.stdout)
            run = subprocess.run(["python3", "-B", str(CLI), str(Path(tmp) / "absent.csv")], capture_output=True, text=True)
            self.assertEqual(1, run.returncode)
            self.assertIn("COMPONENT_BA_FAIL", run.stdout)


if __name__ == "__main__":
    unittest.main()
