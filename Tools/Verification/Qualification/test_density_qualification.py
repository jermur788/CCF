"""Compile and execute the density harness's actual coverage policy without Unity."""
import json
import os
from pathlib import Path
import re
import subprocess
import tempfile
import unittest

from qualification_results import DENSITY_PHASES, DENSITY_SCENARIOS


class DensityCoverageTests(unittest.TestCase):
    def test_runners_reject_unknown_tokens_before_license_or_unity_setup(self):
        verification = Path(__file__).resolve().parents[1]
        for runner in (verification / "Qualification/run_qualification.py", verification / "ThreeSpecies/Qualification/run_qualification.py"):
            for key, value, message in (("CCF_QUAL_SCENARIOS", "B_BEECH_DENSE", "filter selected nothing"),
                                        ("CCF_QUAL_SCENARIOS", "B,UNKNOWN", "filter selected nothing"),
                                        ("CCF_QUAL_PHASES", "LONGRUN,UNKNOWN", "Unknown CCF_QUAL_PHASES")):
                with self.subTest(runner=runner, value=value), tempfile.TemporaryDirectory(prefix="ccf-invalid-request-") as tmp:
                    out = Path(tmp) / "must-not-be-created"
                    run = subprocess.run(["python3", "-B", str(runner), "--label", "invalid", "--out", str(out),
                                          "--method", "DensityQualification.Begin", "--marker", "DENSITY_QUAL_PASS", "--env", key + "=" + value],
                                         capture_output=True, text=True)
                    self.assertNotEqual(0, run.returncode)
                    self.assertIn(message, run.stderr + run.stdout)
                    self.assertFalse(out.exists())

    def test_actual_selector_and_empty_check_policy(self):
        source = (Path(__file__).resolve().parents[1] / "ThreeSpecies/Qualification/DensityQualification.cs").read_text()
        helper = "public static class DensityQualificationCoverage\n" + source.split(
            "public static class DensityQualificationCoverage\n", 1)[1].split(
                "public sealed class DensityQualificationRunner", 1)[0]
        longrun = source.split("private IEnumerator LongRun()", 1)[1].split("// ---------- driver", 1)[0]
        names = re.findall(r'\("([A-G]1?_[A-Z0-9_]+)", [12],', longrun)
        self.assertEqual(DENSITY_SCENARIOS, names)
        self.assertIn('public const string Phases = "' + ','.join(DENSITY_PHASES) + '";', source)
        ux = (Path(__file__).resolve().parents[1] / "ScenarioOneUx/Qualification/UxQualification.cs").read_text()
        ux_helper = "public static class UxQualificationPhases\n" + ux.split("public static class UxQualificationPhases\n", 1)[1].split("// F10 evidence", 1)[0]
        self.assertIn("string[] selectedPhases = UxQualificationPhases.Select(only);", ux)
        wanted = set(re.findall(r'Want\("([A-Z_0-9]+)"\)', ux))
        known = set(re.search(r'public const string Known = "([^"]+)"', ux_helper).group(1).split(','))
        self.assertEqual(wanted, known)
        self.assertIn("DensityQualificationCoverage.SelectScenarios(", longrun)
        self.assertLess(longrun.index("SelectScenarios("), longrun.index('Csv($"longrun-'))
        self.assertIn("r1.Rows.Count != horizon || r1.Living.Count != horizon", longrun)
        self.assertIn("DensityQualificationCoverage.RequireCoverage(hardPassed, measurementOnly, longRunRows + perfRecords, expectedLongRunRows + expectedPerfRecords)", source)
        program = r'''
using System;
using System.Linq;
public static class DensityCoverageSelfTest
{
    private static int checks;
    private static void Check(bool ok, string name)
    {
        if (!ok) throw new Exception(name);
        checks++;
        Console.WriteLine("DENSITY_COVERAGE_SELF_TEST_PASS " + name);
    }
    private static void Rejected(Action action, string message, string name)
    {
        try { action(); } catch (InvalidOperationException e) { Check(e.Message.Contains(message), name); return; }
        throw new Exception("False pass: " + name);
    }
    public static void Main()
    {
        string[] names = new string[] { NAMES };
        Check(DensityQualificationCoverage.SelectScenarios(names, null).Length == 9, "no filter retains all scenarios");
        Check(DensityQualificationCoverage.SelectScenarios(names, "B,F").SequenceEqual(new[] { "B_BEECH_DENSE", "F_BEECH_YOUNG" }), "valid B,F");
        Check(DensityQualificationCoverage.SelectScenarios(names, "C,G").SequenceEqual(new[] { "C_MIXED_DENSE", "G_MIXED_YOUNG" }), "valid C,G excludes model1 controls");
        Check(DensityQualificationCoverage.SelectScenarios(names, "C1,G1").Length == 2, "valid model1 controls");
        Check(DensityQualificationCoverage.SelectScenarios(names, " B , F,B ").Length == 2, "trim and duplicate short selectors");
        foreach (string filter in new[] { "B_BEECH_DENSE,F_BEECH_YOUNG", "C_MIXED_DENSE,G_MIXED_YOUNG", "UNKNOWN", "B,UNKNOWN", "", " ", "," })
            Rejected(() => DensityQualificationCoverage.SelectScenarios(names, filter), "filter selected nothing", "reject [" + filter + "]");
        Check(DensityQualificationCoverage.SelectPhases("LONGRUN").SequenceEqual(new[] { "LONGRUN" }), "valid phase");
        foreach (string phases in new[] { "LONGRUN,UNKNOWN", "UNKNOWN", "" })
            Rejected(() => DensityQualificationCoverage.SelectPhases(phases), "Unknown CCF_QUAL_PHASES", "unknown density phase [" + phases + "]");
        Rejected(() => DensityQualificationCoverage.RequireCoverage(0, false, 0, 0), "hardChecks=0", "zero checks in check-bearing phase fails");
        Rejected(() => DensityQualificationCoverage.RequireCoverage(-1, false, 0, 0), "hardChecks=0", "negative check count fails");
        DensityQualificationCoverage.RequireCoverage(0, true, 200, 200);
        Check(true, "measurement-only zero checks with expected rows allowed");
        Rejected(() => DensityQualificationCoverage.RequireCoverage(0, true, 0, 200), "incomplete rows", "zero measurement rows fails");
        Rejected(() => DensityQualificationCoverage.RequireCoverage(0, true, 0, 0), "incomplete rows", "zero expected rows fails");
        Rejected(() => DensityQualificationCoverage.RequireCoverage(0, true, 100, 200), "incomplete rows", "partial measurement rows fails");
        DensityQualificationCoverage.RequireCoverage(1, false, 0, 0);
        DensityQualificationCoverage.RequireCoverage(90, false, 0, 0);
        Check(true, "positive hard check counts accepted");
        Check(UxQualificationPhases.Select(null).Length == 14, "default UX phases preserved");
        Check(UxQualificationPhases.Select(" ROLE , F10_INPUT ").Length == 2, "valid UX subset");
        foreach (string phases in new[] { "UNKNOWN", "ROLE,UNKNOWN", "" })
            Rejected(() => UxQualificationPhases.Select(phases), "Unknown CCF_UXQ_PHASES", "unknown UX phase [" + phases + "]");
        Console.WriteLine("DENSITY_COVERAGE_SELF_TEST_PASS checks=" + checks);
    }
}
'''.replace("NAMES", ", ".join(json.dumps(n) for n in names))
        # TMPDIR is set to external project storage by the documented test command.
        with tempfile.TemporaryDirectory(prefix="ccf-density-coverage-") as tmp:
            out = Path(tmp)
            (out / "SelfTest.cs").write_text("using System;\nusing System.Linq;\n" + helper + ux_helper + program[program.index("public static class"):])
            (out / "check.csproj").write_text('<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net8.0</TargetFramework></PropertyGroup></Project>')
            run = subprocess.run(["dotnet", "run", "--project", str(out / "check.csproj"), "-c", "Release"],
                                 env=dict(os.environ, DOTNET_NOLOGO="1", DOTNET_CLI_TELEMETRY_OPTOUT="1"), capture_output=True, text=True)
            self.assertEqual(0, run.returncode, run.stdout + run.stderr)
            self.assertIn("DENSITY_COVERAGE_SELF_TEST_PASS checks=28", run.stdout)
            print(run.stdout, end="")


if __name__ == "__main__":
    unittest.main()
