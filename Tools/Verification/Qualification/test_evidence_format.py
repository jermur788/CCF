"""Regression tests for the explicit LONGRUN evidence-format contract (CURRENT vs PRE-COMPONENT-BA).

CURRENT is the default and requires component-ba rows. PRE-COMPONENT-BA is the historical format produced before
component-BA instrumentation: ordinary LONGRUN evidence is still required and must be complete/valid, but component-ba is
not expected. Historical mode must be requested explicitly and is never inferred from a missing file.

No Unity: these exercise qualification_results.evaluate_receipt / read_longrun_evidence and the runner CLI (argument
parsing only; a bad/empty --evidence-format must be rejected before any Editor could launch).
"""
import os
import subprocess
import sys
import tempfile
import unittest
from pathlib import Path

from qualification_results import (COMPONENT_BA_COLUMNS, EVIDENCE_FORMAT_CURRENT, EVIDENCE_FORMAT_PRE_COMPONENT_BA,
                                   evaluate_receipt, read_component_ba_evidence, read_longrun_evidence)

ROOT = Path(__file__).resolve().parents[3]
RUNNER = ROOT / "Tools/Verification/ThreeSpecies/Qualification/run_qualification.py"

ENV = {"CCF_QUAL_PHASES": "LONGRUN", "CCF_QUAL_REPLAY": "0", "CCF_QUAL_SCENARIOS": "B,F", "CCF_STAND_GEOMETRY": "1"}
LOG = "QUAL_START geometry=E80 phases=LONGRUN\nDENSITY_QUAL_PASS hardChecks=0"
SCENARIOS = ("B_BEECH_DENSE", "F_BEECH_YOUNG")
YEARS = range(1, 101)
# livingBefore,baPreM2,densityDeaths,sitkaComponentDeaths,genericDeaths,sitkaBaLossM2,genericBaLossM2,combinedBaLossM2,
# sitkaBaFraction,genericBaFraction,combinedBaFraction,deadBaM2,deadBaFraction,sitkaFired,genericFired,bothFired,attributionExact
NO_DEATH_ROW = "100,10.00000,0,0,0,0.00000,0.00000,0.00000,0.00000,0.00000,0.00000,0.00000,0.00000,0,0,0,1"


def longrun_rows():
    return [f"{scenario},E80,{year}\n" for scenario in SCENARIOS for year in YEARS]


def write_longrun(tmp, rows):
    (Path(tmp) / "longrun-E80.csv").write_text("scenario,geo,year\n" + "".join(rows))


def write_component_ba(tmp, body=None, text=None):
    path = Path(tmp) / "component-ba-E80.csv"
    if text is not None:
        path.write_text(text)
        return path
    rows = body if body is not None else [f"{scenario},E80,{year},SpeciesAwareOccupancy,{NO_DEATH_ROW}\n"
                                          for scenario in SCENARIOS for year in YEARS]
    path.write_text(",".join(COMPONENT_BA_COLUMNS) + "\n" + "".join(rows))
    return path


def decision(tmp, evidence_format=None):
    evidence = read_longrun_evidence(tmp, ENV, LOG)
    return evaluate_receipt(0, "DENSITY_QUAL_PASS", LOG, [], env=ENV, measurement_evidence=evidence,
                            evidence_format=evidence_format)


class EvidenceFormatTests(unittest.TestCase):
    def test_01_current_complete_longrun_and_component_ba_passes(self):
        with tempfile.TemporaryDirectory(prefix="ccf-format-") as tmp:
            write_longrun(tmp, longrun_rows())
            write_component_ba(tmp)
            result = decision(tmp, EVIDENCE_FORMAT_CURRENT)
            self.assertEqual("PASS", result["status"], result["coverage_errors"])
            self.assertEqual(EVIDENCE_FORMAT_CURRENT, result["evidence_format"])

    def test_02_current_complete_longrun_without_component_ba_fails(self):
        with tempfile.TemporaryDirectory(prefix="ccf-format-") as tmp:
            write_longrun(tmp, longrun_rows())          # no component-ba file
            result = decision(tmp, EVIDENCE_FORMAT_CURRENT)
            self.assertEqual("FAIL", result["status"])
            self.assertTrue(any("component-ba" in error for error in result["coverage_errors"]), result["coverage_errors"])

    def test_03_historical_complete_longrun_without_component_ba_passes(self):
        with tempfile.TemporaryDirectory(prefix="ccf-format-") as tmp:
            write_longrun(tmp, longrun_rows())
            result = decision(tmp, EVIDENCE_FORMAT_PRE_COMPONENT_BA)
            self.assertEqual("PASS", result["status"], result["coverage_errors"])
            self.assertEqual(EVIDENCE_FORMAT_PRE_COMPONENT_BA, result["evidence_format"])

    def test_04_historical_missing_or_partial_longrun_fails(self):
        with tempfile.TemporaryDirectory(prefix="ccf-format-") as tmp:
            # missing longrun file entirely
            self.assertEqual("FAIL", decision(tmp, EVIDENCE_FORMAT_PRE_COMPONENT_BA)["status"])
            # partial rows
            write_longrun(tmp, longrun_rows()[:100])
            self.assertEqual("FAIL", decision(tmp, EVIDENCE_FORMAT_PRE_COMPONENT_BA)["status"])

    def test_05_historical_malformed_longrun_fails(self):
        with tempfile.TemporaryDirectory(prefix="ccf-format-") as tmp:
            (Path(tmp) / "longrun-E80.csv").write_text("scenario,geo,year\nB_BEECH_DENSE,E80,not-a-year\n")
            result = decision(tmp, EVIDENCE_FORMAT_PRE_COMPONENT_BA)
            self.assertEqual("FAIL", result["status"])

    def test_06_historical_with_valid_component_ba_still_passes(self):
        with tempfile.TemporaryDirectory(prefix="ccf-format-") as tmp:
            write_longrun(tmp, longrun_rows())
            write_component_ba(tmp)
            self.assertEqual("PASS", decision(tmp, EVIDENCE_FORMAT_PRE_COMPONENT_BA)["status"])

    def test_07_historical_with_malformed_component_ba_fails_not_ignored(self):
        with tempfile.TemporaryDirectory(prefix="ccf-format-") as tmp:
            write_longrun(tmp, longrun_rows())
            write_component_ba(tmp, text="wrong,schema\n1,2\n")     # present but malformed
            result = decision(tmp, EVIDENCE_FORMAT_PRE_COMPONENT_BA)
            self.assertEqual("FAIL", result["status"])
            self.assertTrue(any("component-ba" in error for error in result["coverage_errors"]), result["coverage_errors"])

    def test_08_unknown_evidence_format_is_rejected(self):
        with tempfile.TemporaryDirectory(prefix="ccf-format-") as tmp:
            write_longrun(tmp, longrun_rows())
            write_component_ba(tmp)                                 # even fully valid evidence must not accept a bad format
            result = decision(tmp, "component-ba")
            self.assertEqual("FAIL", result["status"])
            self.assertTrue(any("Unknown evidence format" in error for error in result["assertion_failures"]),
                            result["assertion_failures"])
        # The format guard is independent of the marker.
        self.assertEqual("FAIL", evaluate_receipt(0, "GATE_PASS", "GATE_PASS checks=1", [], evidence_format="bogus")["status"])

    def test_09_explicit_empty_evidence_format_is_rejected_through_the_cli(self):
        with tempfile.TemporaryDirectory(prefix="ccf-format-") as tmp:
            for value in ("", "component-ba"):
                run = subprocess.run([sys.executable, "-B", str(RUNNER),
                                      "--label", "format-check", "--out", str(Path(tmp) / "out"),
                                      "--method", "DensityQualification.Begin", "--marker", "DENSITY_QUAL_PASS",
                                      "--evidence-format", value],
                                     capture_output=True, text=True)
                self.assertNotEqual(0, run.returncode, "empty/unknown --evidence-format must be rejected before launch")
                self.assertFalse((Path(tmp) / "out" / "evidence").exists(), "no run directory may be created")

    def test_10_historical_directory_fails_by_default_and_passes_only_when_declared(self):
        with tempfile.TemporaryDirectory(prefix="ccf-format-") as tmp:
            write_longrun(tmp, longrun_rows())          # a historical receipt: no component-ba
            default = decision(tmp)                     # omitted -> CURRENT
            self.assertEqual(EVIDENCE_FORMAT_CURRENT, default["evidence_format"])
            self.assertEqual("FAIL", default["status"])
            declared = decision(tmp, EVIDENCE_FORMAT_PRE_COMPONENT_BA)
            self.assertEqual("PASS", declared["status"])
            # The absence of component-ba must never itself select historical mode.
            self.assertNotEqual(default["evidence_format"], EVIDENCE_FORMAT_PRE_COMPONENT_BA)

    # --- present-vs-absent polarity: historical mode tolerates ONLY explicit physical absence ---

    def test_11_absence_is_explicit_and_never_inferred_from_an_error(self):
        with tempfile.TemporaryDirectory(prefix="ccf-format-") as tmp:
            write_longrun(tmp, longrun_rows())
            self.assertIs(True, read_component_ba_evidence(tmp, ENV, LOG)["absent"])          # genuinely missing
            write_component_ba(tmp, text="wrong,schema\n1,2\n")                               # present but malformed
            self.assertIs(False, read_component_ba_evidence(tmp, ENV, LOG)["absent"])
            write_component_ba(tmp, text="")                                                  # present but empty
            self.assertIs(False, read_component_ba_evidence(tmp, ENV, LOG)["absent"])

    def test_12_historical_bad_year_component_ba_fails(self):
        with tempfile.TemporaryDirectory(prefix="ccf-format-") as tmp:
            write_longrun(tmp, longrun_rows())
            rows = [f"{scenario},E80,{year},SpeciesAwareOccupancy,{NO_DEATH_ROW}\n" for scenario in SCENARIOS for year in YEARS]
            rows[0] = "B_BEECH_DENSE,E80,not-a-year,SpeciesAwareOccupancy," + NO_DEATH_ROW + "\n"
            write_component_ba(tmp, body=rows)
            self.assertIs(False, read_component_ba_evidence(tmp, ENV, LOG)["absent"])
            self.assertEqual("FAIL", decision(tmp, EVIDENCE_FORMAT_PRE_COMPONENT_BA)["status"])
            self.assertEqual("FAIL", decision(tmp, EVIDENCE_FORMAT_CURRENT)["status"])

    def test_13_historical_invalid_utf8_component_ba_fails(self):
        with tempfile.TemporaryDirectory(prefix="ccf-format-") as tmp:
            write_longrun(tmp, longrun_rows())
            (Path(tmp) / "component-ba-E80.csv").write_bytes(b"\xff\xfe\x00 not utf-8 \x80\x81")
            self.assertIs(False, read_component_ba_evidence(tmp, ENV, LOG)["absent"])
            self.assertEqual("FAIL", decision(tmp, EVIDENCE_FORMAT_PRE_COMPONENT_BA)["status"])

    def test_14_historical_unreadable_component_ba_fails_or_skips(self):
        with tempfile.TemporaryDirectory(prefix="ccf-format-") as tmp:
            write_longrun(tmp, longrun_rows())
            path = write_component_ba(tmp)
            os.chmod(path, 0)
            try:
                if os.geteuid() == 0:
                    self.skipTest("running as root; chmod 000 does not deny read")
                try:
                    path.read_bytes()
                except PermissionError:
                    pass
                else:
                    self.skipTest("platform does not enforce unreadability for this user")
                self.assertIs(False, read_component_ba_evidence(tmp, ENV, LOG)["absent"])
                self.assertEqual("FAIL", decision(tmp, EVIDENCE_FORMAT_PRE_COMPONENT_BA)["status"])
            finally:
                os.chmod(path, 0o644)

    def test_15_historical_short_row_fails_gracefully_without_traceback(self):
        with tempfile.TemporaryDirectory(prefix="ccf-format-") as tmp:
            write_longrun(tmp, longrun_rows())
            # A row with a missing year: int(None) raises TypeError unless it is handled explicitly.
            write_component_ba(tmp, text=",".join(COMPONENT_BA_COLUMNS) + "\nB_BEECH_DENSE\n")
            result = decision(tmp, EVIDENCE_FORMAT_PRE_COMPONENT_BA)
            self.assertEqual("FAIL", result["status"])


if __name__ == "__main__":
    unittest.main()
