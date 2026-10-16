"""Deterministic offline tests for compare_ccf.py (schema, run filter, input validity, percent
semantics, domain classification, initialization metadata). No Unity, no dependencies.

Run:  python3 Tools/Verification/Growfor/test_compare_ccf.py
Exit 0 = all checks pass.
"""
import json
import math
import sys
import tempfile
import unittest
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import compare_ccf as c  # noqa: E402
import broad_lynch_2006_sitka as bl  # noqa: E402

HEADER = "age,stems_ha,basal_area_m2_ha,top_height_m,volume_m3_ha\n"
TWO_ROWS = "20,2100,41.10,14.63,249.2\n120,256,83.81,31.05,1218.8\n"


def write(tmp, name, text):
    path = Path(tmp) / name
    path.write_text(text)
    return str(path)


class SchemaTests(unittest.TestCase):
    def test_canonical_names_accepted(self):
        with tempfile.TemporaryDirectory() as tmp:
            meta, series = c.load(write(tmp, "s.csv", HEADER + TWO_ROWS))
            self.assertEqual(2, len(series))
            self.assertAlmostEqual(41.10, series[0]["basal_area_m2_ha"])

    def test_unambiguous_synonyms_accepted(self):
        text = "stand_age,stems_per_ha,basal_area,top_height,volume\n20,2100,41.10,14.63,249.2\n120,256,83.81,31.05,1218.8\n"
        with tempfile.TemporaryDirectory() as tmp:
            _, series = c.load(write(tmp, "s.csv", text))
            self.assertEqual(2, len(series))

    def test_ambiguous_short_names_rejected(self):
        for header in ("age,n,b,h,v\n", "age,N,B,H,V\n", "age,stocking,basal_area_m2_ha,top_height_m\n"):
            with self.subTest(header=header):
                with tempfile.TemporaryDirectory() as tmp:
                    with self.assertRaises(SystemExit):
                        c.load(write(tmp, "s.csv", header + "20,2100,41.10,14.63\n"))

    def test_unknown_column_rejected(self):
        with tempfile.TemporaryDirectory() as tmp:
            with self.assertRaises(SystemExit):
                c.load(write(tmp, "s.csv", HEADER.rstrip("\n") + ",mystery\n" + "20,2100,41.10,14.63,249.2,1\n"))

    def test_duplicate_dimension_columns_rejected(self):
        text = "age,stems_ha,basal_area_m2_ha,basal_area,top_height_m\n20,2100,41.10,41.10,14.63\n120,256,83.81,83.81,31.05\n"
        with tempfile.TemporaryDirectory() as tmp:
            with self.assertRaises(SystemExit):
                c.load(write(tmp, "s.csv", text))


class RunFilterTests(unittest.TestCase):
    HEAD = "run,age,stems_ha,basal_area_m2_ha,top_height_m\n"
    ROWS = ("A,20,2100,41.10,14.63\nA,120,256,83.81,31.05\n"
            "B,20,1800,30.00,13.00\nB,120,200,60.00,28.00\n")

    def test_single_run_without_filter(self):
        with tempfile.TemporaryDirectory() as tmp:
            _, series = c.load(write(tmp, "s.csv", self.HEAD + "A,20,2100,41.10,14.63\nA,120,256,83.81,31.05\n"))
            self.assertEqual(2, len(series))

    def test_run_filter_selects_requested_series(self):
        with tempfile.TemporaryDirectory() as tmp:
            _, series = c.load(write(tmp, "s.csv", self.HEAD + self.ROWS), run="B")
            self.assertEqual(2, len(series))
            self.assertAlmostEqual(30.00, series[0]["basal_area_m2_ha"])

    def test_unknown_run_rejected(self):
        with tempfile.TemporaryDirectory() as tmp:
            with self.assertRaises(SystemExit):
                c.load(write(tmp, "s.csv", self.HEAD + self.ROWS), run="C")

    def test_multiple_runs_without_filter_rejected(self):
        with tempfile.TemporaryDirectory() as tmp:
            with self.assertRaises(SystemExit):
                c.load(write(tmp, "s.csv", self.HEAD + self.ROWS))

    def test_run_filter_without_run_column_rejected(self):
        with tempfile.TemporaryDirectory() as tmp:
            with self.assertRaises(SystemExit):
                c.load(write(tmp, "s.csv", HEADER + TWO_ROWS), run="A")

    def test_json_run_filter(self):
        with tempfile.TemporaryDirectory() as tmp:
            payload = {"meta": {"run": "A"}, "series": [
                {"age": 20, "stems_ha": 2100, "basal_area_m2_ha": 41.1, "top_height_m": 14.63},
                {"age": 120, "stems_ha": 256, "basal_area_m2_ha": 83.81, "top_height_m": 31.05}]}
            path = write(tmp, "s.json", json.dumps(payload))
            _, series = c.load(path, run="A")
            self.assertEqual(2, len(series))
            with self.assertRaises(SystemExit):
                c.load(path, run="Z")


class InputValidityTests(unittest.TestCase):
    def test_non_finite_values_rejected(self):
        for value in ("nan", "inf", "-inf"):
            with self.subTest(value=value):
                text = HEADER + f"20,2100,41.10,{value},249.2\n120,256,83.81,31.05,1218.8\n"
                with tempfile.TemporaryDirectory() as tmp:
                    with self.assertRaises(SystemExit):
                        c.load(write(tmp, "s.csv", text))

    def test_duplicate_age_rejected(self):
        text = HEADER + "20,2100,41.10,14.63,249.2\n20,1900,45.0,17.0,300.0\n"
        with tempfile.TemporaryDirectory() as tmp:
            with self.assertRaises(SystemExit):
                c.load(write(tmp, "s.csv", text))

    def test_non_monotonic_ages_rejected(self):
        text = HEADER + "40,1706,75.87,24.14,803.1\n20,2100,41.10,14.63,249.2\n"
        with tempfile.TemporaryDirectory() as tmp:
            with self.assertRaises(SystemExit):
                c.load(write(tmp, "s.csv", text))

    def test_vacuous_and_insufficient_series_rejected(self):
        with tempfile.TemporaryDirectory() as tmp:
            with self.assertRaises(SystemExit):
                c.load(write(tmp, "empty.csv", HEADER))
            with self.assertRaises(SystemExit):
                c.load(write(tmp, "one.csv", HEADER + "20,2100,41.10,14.63,249.2\n"))


class CompareSemanticsTests(unittest.TestCase):
    def _series(self):
        return [
            {"age": 20.0, "stems_ha": 2100.0, "basal_area_m2_ha": 41.10, "top_height_m": 14.63, "volume_m3_ha": 249.2},
            {"age": 120.0, "stems_ha": 256.0, "basal_area_m2_ha": 83.81, "top_height_m": 31.05, "volume_m3_ha": 1218.8},
        ]

    def test_initialization_metadata_is_explicit(self):
        result = c.compare({"source": "x"}, self._series(), bl.UNTHINNED)
        init = result["initialization"]
        self.assertEqual(20.0, init["anchor_age"])
        self.assertEqual(14.63, init["anchor_top_height_m"])
        self.assertAlmostEqual(22.19334, init["site_index_S"], places=4)
        self.assertAlmostEqual(init["site_index_S"], bl.site_index_from_height(20.0, 14.63), places=12)
        self.assertIn("30", init["method"])
        self.assertIn("not fitted", init["note"])

    def test_zero_denominator_percentage_is_undefined_not_zero(self):
        series = [
            {"age": 20.0, "stems_ha": 2100.0, "basal_area_m2_ha": 41.10, "top_height_m": 14.63, "volume_m3_ha": 0.0},
            {"age": 40.0, "stems_ha": 1706.0, "basal_area_m2_ha": 75.87, "top_height_m": 24.14, "volume_m3_ha": 0.0},
        ]
        result = c.compare({"source": "x"}, series, bl.UNTHINNED)
        later = result["rows"][1]
        self.assertIsNone(later["pct_diff_V"])
        self.assertEqual("ccf_denominator_zero", later["pct_reason_V"])
        self.assertGreaterEqual(result["undefined_percentages"]["V"], 1)
        self.assertIsNone(result["summary"]["V"]["mean_abs_pct"])

    def test_negative_model_volume_flagged_not_clamped(self):
        result = c.compare({"source": "x"}, self._series(), bl.UNTHINNED)
        last = result["rows"][-1]
        self.assertEqual("OUTSIDE_USEFUL_DOMAIN", last["domain"])
        self.assertLess(last["model"]["V"], 0.0)          # raw prediction retained
        self.assertIn(120.0, result["nonphysical_rows"])
        self.assertIn("asymptote", result["domain_note"])
        self.assertIn("7 cm", result["volume_definition"])
        self.assertGreaterEqual(len(result["caveats"]), 5)


if __name__ == "__main__":
    unittest.main(verbosity=2)
