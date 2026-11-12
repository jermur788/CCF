#!/usr/bin/env python3
"""Guard-removal checks for the year-comparison text mapper.

Each mutation is a copy of the mapper with ONE rule removed, written to a new directory outside the repository.
The copy must still compile, and the fixture must then FAIL on the assertion that guards that rule. A mutant that
passes means the fixture does not really test the rule. The production source is never rewritten.

Run as: python3 -I -B Tools/Verification/YearComparison/run_mutations.py --out <new directory outside the repo>
"""
import argparse
import difflib
import pathlib
import shutil
import subprocess
import sys

# (name, text to find exactly once in the mapper, replacement, fixture group to run, assertion expected to fail)
MUTATIONS = [
    ("year-filter",
     "            if (work == null || work.Year != year) continue;",
     "            if (work == null) continue;",
     "yearfilter", "FAILED only viewed-year rows are counted"),
    ("ambiguous-exclusion",
     "            if (work.Link == Readout.OrderLink.AmbiguousIdentity) { ambiguous++; continue; }\n",
     "",
     "ambiguous", "FAILED ambiguous rows are excluded from every count"),
    ("ambiguous-before-unresolved",
     "            if (work.Link == Readout.OrderLink.AmbiguousIdentity) { ambiguous++; continue; }\n"
     "            if (work.Result == Readout.Resolution.Unresolved) { unresolved++; continue; }\n",
     "            if (work.Result == Readout.Resolution.Unresolved) { unresolved++; continue; }\n"
     "            if (work.Link == Readout.OrderLink.AmbiguousIdentity) { ambiguous++; continue; }\n",
     "ambiguous", "FAILED a row that is ambiguous and unresolved is reported in the ambiguous group"),
    ("cannot-compare-precedence",
     "        if (result.ComparisonState == Readout.ObservationState.CannotCompare)\n",
     "        if (result.ComparisonState == Readout.ObservationState.CannotCompare && selectedRecorded && latestRecorded)\n",
     "precedence", "FAILED CannotCompare beats a missing snapshot"),
    ("canopy-points",
     "Signed(change.MeanCanopyChange * 100f, \"0.0\")",
     "Signed(change.MeanCanopyChange, \"0.0\")",
     "units", "FAILED canopy change is shown in percentage points with its sign"),
    ("event-only-warning",
     "            if (work.Link == Readout.OrderLink.MissingOrder || work.Link == Readout.OrderLink.InconsistentOrder) eventOnly++;\n",
     "",
     "linkage", "FAILED expected exactly one row 'work-event-only', found 0"),
]


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--out", required=True, type=pathlib.Path)
    out = parser.parse_args().out.resolve()
    here = pathlib.Path(__file__).resolve().parent
    root = here.parents[2].resolve()
    if out == root or root in out.parents or out.exists():
        parser.error("--out must be a NEW directory outside this repository")
    exe = shutil.which("dotnet")
    if exe is None:
        parser.error("dotnet SDK required")
    out.mkdir(parents=True)
    source = root / "Assets/ForestPrototype/ScenarioOne/ScenarioOneYearComparisonText.cs"
    original = source.read_text(encoding="utf-8")
    others = [root / "Assets/ForestPrototype/ScenarioOne/ScenarioOneManagementFollowonReadout.cs",
              here / "YearComparisonVerification.cs", here / "OfflineProvenanceTypes.cs",
              root / "Tools/Verification/ManagementFollowonReadout/OfflineContractTypes.cs"]
    failures = 0
    for name, before, after, group, expected in MUTATIONS:
        if original.count(before) != 1:
            print(name + ": ERROR the guarded text was not found exactly once; review the mutation definition")
            failures += 1
            continue
        work = out / name
        work.mkdir()
        mutated = original.replace(before, after, 1)
        mutant = work / source.name
        mutant.write_text(mutated, encoding="utf-8")
        (work / "mutation.diff").write_text("".join(difflib.unified_diff(
            original.splitlines(True), mutated.splitlines(True), fromfile="mapper", tofile=name, n=0)), encoding="utf-8")
        items = "\n".join('    <Compile Include="' + str(p) + '" />' for p in [mutant] + others)
        project = work / "Mutation.csproj"
        project.write_text('<Project Sdk="Microsoft.NET.Sdk">\n'
                           '  <PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net8.0</TargetFramework>'
                           '<LangVersion>9.0</LangVersion><Nullable>disable</Nullable>'
                           '<ImplicitUsings>disable</ImplicitUsings><EnableDefaultCompileItems>false</EnableDefaultCompileItems>'
                           '</PropertyGroup>\n  <ItemGroup>\n' + items + '\n  </ItemGroup>\n</Project>\n', encoding="utf-8")
        build = subprocess.run([exe, "build", str(project), "-c", "Release", "-v", "q", "-nologo"],
                               cwd=work, capture_output=True, text=True)
        (work / "build.log").write_text("exit=%d\n" % build.returncode + build.stdout + build.stderr, encoding="utf-8")
        if build.returncode != 0:
            print(name + ": ERROR the mutant did not compile (build exit=%d), so it proves nothing" % build.returncode)
            failures += 1
            continue
        run = subprocess.run([exe, str(work / "bin/Release/net8.0/Mutation.dll"), group], cwd=work, capture_output=True, text=True)
        (work / "run.log").write_text("exit=%d\n" % run.returncode + run.stdout + run.stderr, encoding="utf-8")
        if run.returncode != 0 and expected in run.stderr:
            print(name + ": KILLED; build exit=0, fixture exit=%d (group %s); %s" % (run.returncode, group, expected))
        else:
            print(name + ": SURVIVED or failed for another reason; build exit=0, fixture exit=%d (group %s); expected: %s"
                  % (run.returncode, group, expected))
            failures += 1
    if source.read_text(encoding="utf-8") != original:
        print("ERROR: the production mapper changed during the run")
        failures += 1
    print("mutations: %d of %d killed" % (len(MUTATIONS) - failures, len(MUTATIONS)))
    return 1 if failures else 0


if __name__ == "__main__":
    sys.exit(main())
