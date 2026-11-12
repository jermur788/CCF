#!/usr/bin/env python3
"""Compile and run the REAL year-comparison text mapper and the REAL read-only readout helper with focused fixtures.

Uses TEST-ONLY doubles of the history types, so a PASS proves the mapper's wording and rules in isolation. It does not
compile all of Assets, does not start Unity and reads no saved world. Real production type binding is a separate
check: Tools/Verification/ScenarioOneNonUi/compile_runtime_sources.py.

The build and the run are two separate dotnet processes so each exit code is reported on its own line.
Requires the dotnet SDK with the net8.0 runtime. Run as: python3 -I -B Tools/Verification/YearComparison/run_offline.py
"""
import argparse
import pathlib
import shutil
import subprocess
import sys
import tempfile

here = pathlib.Path(__file__).resolve().parent
root = here.parents[2]
mapper = root / "Assets/ForestPrototype/ScenarioOne/ScenarioOneYearComparisonText.cs"
helper = root / "Assets/ForestPrototype/ScenarioOne/ScenarioOneManagementFollowonReadout.cs"
fixture = here / "YearComparisonVerification.cs"
provenance_double = here / "OfflineProvenanceTypes.cs"
# Shared with the helper's own fixture; referenced here, never edited here.
history_doubles = root / "Tools/Verification/ManagementFollowonReadout/OfflineContractTypes.cs"
sources = (mapper, helper, fixture, provenance_double, history_doubles)


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--out", type=pathlib.Path, help="directory outside the repository to keep the project, logs and binaries in")
    parser.add_argument("--group", default="all", help="run one group of checks only (default: all)")
    args = parser.parse_args()
    missing = [str(p) for p in sources if not p.is_file()]
    if missing:
        print("ERROR: missing source: " + ", ".join(missing), file=sys.stderr)
        return 2
    exe = shutil.which("dotnet")
    if exe is None:
        print("NOT RUN: dotnet SDK not installed; no pass claimed", file=sys.stderr)
        return 2
    if args.out:
        dest = args.out.resolve()
        if dest == root.resolve() or root.resolve() in dest.parents:
            print("ERROR: --out must not be inside the repository", file=sys.stderr)
            return 2
        dest.mkdir(parents=True, exist_ok=True)
        return run(exe, dest, args.group)
    with tempfile.TemporaryDirectory(prefix="ccf-year-comparison-") as tmp:
        return run(exe, pathlib.Path(tmp), args.group)


def write_project(work, compile_sources, name="YearComparisonCheck"):
    items = "\n".join('    <Compile Include="' + str(p) + '" />' for p in compile_sources)
    project = work / (name + ".csproj")
    project.write_text('<Project Sdk="Microsoft.NET.Sdk">\n'
                       '  <PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net8.0</TargetFramework>'
                       '<LangVersion>9.0</LangVersion><Nullable>disable</Nullable>'
                       '<ImplicitUsings>disable</ImplicitUsings><EnableDefaultCompileItems>false</EnableDefaultCompileItems>'
                       '<TreatWarningsAsErrors>true</TreatWarningsAsErrors>'
                       '</PropertyGroup>\n  <ItemGroup>\n' + items + '\n  </ItemGroup>\n</Project>\n', encoding="utf-8")
    return project


def run(exe, work, group):
    project = write_project(work, sources)
    build = subprocess.run([exe, "build", str(project), "-c", "Release", "-v", "minimal", "-nologo"],
                           cwd=work, text=True, capture_output=True)
    (work / "build.log").write_text("exit=%d\n" % build.returncode + build.stdout + build.stderr, encoding="utf-8")
    print("dotnet build exit=%d" % build.returncode)
    if build.returncode != 0:
        print(build.stdout, end="")
        print(build.stderr, end="", file=sys.stderr)
        print("NOT RUN: the fixture did not compile; no pass claimed", file=sys.stderr)
        return build.returncode
    program = work / "bin/Release/net8.0/YearComparisonCheck.dll"
    result = subprocess.run([exe, str(program), group], cwd=work, text=True, capture_output=True)
    (work / "run.log").write_text("exit=%d\n" % result.returncode + result.stdout + result.stderr, encoding="utf-8")
    print(result.stdout, end="")
    print(result.stderr, end="", file=sys.stderr)
    print("fixture run exit=%d" % result.returncode)
    return result.returncode


if __name__ == "__main__":
    sys.exit(main())
