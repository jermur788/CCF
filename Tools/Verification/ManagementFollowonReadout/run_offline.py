#!/usr/bin/env python3
"""Compile/run the REAL production readout plus a TEST-ONLY signature adapter and focused fixtures.
Does not compile all of Assets, does not invoke Unity or access saved worlds. Requires dotnet SDK.
For real production binding separately run ScenarioOneNonUi/compile_runtime_sources.py on a safe lane.
"""
import argparse
import pathlib
import shutil
import subprocess
import sys
import tempfile

here = pathlib.Path(__file__).resolve().parent
root = here.parents[2]
actual = root / "Assets/ForestPrototype/ScenarioOne/ScenarioOneManagementFollowonReadout.cs"
fixture = here / "ManagementFollowonReadoutVerification.cs"
stubs = here / "OfflineContractTypes.cs"

def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--out", type=pathlib.Path, help="optional throwaway directory outside the source repo")
    args = parser.parse_args()
    if not all(p.is_file() for p in (actual, fixture, stubs)):
        print("ERROR: missing production helper or test source", file=sys.stderr)
        return 2
    exe = shutil.which("dotnet")
    if exe is None:
        print("NOT RUN: dotnet SDK not installed; no pass claimed", file=sys.stderr)
        return 2
    if args.out:
        dest = args.out.resolve()
        if str(dest).startswith(str(root.resolve()) + "/"):
            print("ERROR: --out must not be inside repository", file=sys.stderr)
            return 2
        dest.mkdir(parents=True, exist_ok=True)
        return run(exe, dest)
    with tempfile.TemporaryDirectory(prefix="ccf-management-readout-") as tmp:
        return run(exe, pathlib.Path(tmp))

def run(exe, work):
    sources = (actual, fixture, stubs)
    items = "\n".join('    <Compile Include="' + str(p) + '" />' for p in sources)
    project = ('<Project Sdk="Microsoft.NET.Sdk">\n'
               '  <PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net8.0</TargetFramework>'
               '<LangVersion>9.0</LangVersion><Nullable>disable</Nullable>'
               '<ImplicitUsings>disable</ImplicitUsings><EnableDefaultCompileItems>false</EnableDefaultCompileItems>'
               '</PropertyGroup>\n  <ItemGroup>\n' + items + '\n  </ItemGroup>\n</Project>\n')
    (work / "ManagementReadoutCheck.csproj").write_text(project, encoding="utf-8")
    result = subprocess.run([exe, "run", "--project", str(work / "ManagementReadoutCheck.csproj"),
                             "-c", "Release"], cwd=work, text=True, capture_output=True)
    print(result.stdout, end="")
    print(result.stderr, end="", file=sys.stderr)
    return result.returncode

if __name__ == "__main__":
    sys.exit(main())
