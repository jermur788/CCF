#!/usr/bin/env python3
"""Build and run the Scenario One UX offline check on the plain .NET SDK.

It compiles the PRODUCTION files that hold the player-facing wording and decision rules (none of them uses
UnityEngine or UI Toolkit) together with OfflineLogic/ScenarioOneUxOfflineCheck.cs, in a temporary project OUTSIDE
the repository, and runs it. No Unity Editor, no Library, nothing written inside the repo.

This proves wording, ordering and decision rules only. It does not prove rendering, modal behaviour, input or
text fit: those need Unity (see Docs/Verification/ScenarioOneUxGap/Handoff.md).

Usage: run_ux_check.py [--out DIR]     Exit code 0 and SCENARIO_ONE_UX_PASS on success. Needs `dotnet` (SDK 8+).
"""
import argparse
import os
import shutil
import subprocess
import sys
import tempfile

HERE = os.path.dirname(os.path.abspath(__file__))
ROOT = os.path.abspath(os.path.join(HERE, "..", "..", ".."))
UI = os.path.join(ROOT, "Assets", "ForestPrototype", "UI")

# Files that must stay free of UnityEngine / UIElements so this check can compile them. Add a file here when it joins the check.
PURE_SOURCES = [
    "MenuHelpView.Text.cs",
    "LearningObjectivesView.Curriculum.cs",
]

PROJECT = """<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <LangVersion>9.0</LangVersion>
    <Nullable>disable</Nullable>
    <ImplicitUsings>disable</ImplicitUsings>
    <EnableDefaultCompileItems>false</EnableDefaultCompileItems>
    <StartupObject>ScenarioOneUxOfflineCheck</StartupObject>
  </PropertyGroup>
  <ItemGroup>
{items}
  </ItemGroup>
</Project>
"""


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--out", default=os.path.join(tempfile.gettempdir(), "ccf-scenario-one-ux-offline"))
    args = parser.parse_args()
    dotnet = shutil.which("dotnet")
    if not dotnet:
        sys.exit("dotnet SDK not found")
    sources = [os.path.join(UI, name) for name in PURE_SOURCES] + [os.path.join(HERE, "OfflineLogic", "ScenarioOneUxOfflineCheck.cs")]
    for source in sources:
        if not os.path.isfile(source):
            sys.exit("missing source " + source)
    os.makedirs(args.out, exist_ok=True)
    with open(os.path.join(args.out, "check.csproj"), "w") as handle:
        handle.write(PROJECT.format(items="\n".join('    <Compile Include="%s" />' % s for s in sources)))
    env = dict(os.environ, DOTNET_CLI_TELEMETRY_OPTOUT="1", DOTNET_NOLOGO="1")
    build = subprocess.run([dotnet, "build", "-nologo", "-v", "q", "-c", "Release"], cwd=args.out, env=env, capture_output=True, text=True)
    if build.returncode != 0:
        sys.stdout.write(build.stdout)
        sys.stderr.write(build.stderr)
        return build.returncode
    run = subprocess.run([dotnet, "run", "--no-build", "-c", "Release"], cwd=args.out, env=env, capture_output=True, text=True)
    sys.stdout.write(run.stdout)
    sys.stderr.write(run.stderr)
    return run.returncode


if __name__ == "__main__":
    sys.exit(main())
