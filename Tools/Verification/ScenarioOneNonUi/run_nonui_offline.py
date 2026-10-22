#!/usr/bin/env python3
"""Build and run the SCENARIO-ONE-PLAYER-TEST-NON-UI-REMEDIATION-V1 offline checks on the plain .NET SDK.

It compiles the PRODUCTION files that hold the rules (none of them uses UnityEngine) together with the check sources in OfflineLogic/,
in a temporary project OUTSIDE the repository, and runs it. No Unity Editor, no Library, nothing written inside the repo.

This proves decision rules only (competitor keyboard choice, rack-area teaching analysis, sequence order). It does not prove input,
rendering or that the live game feeds the rules the right data: those are the Unity gates beside it.

Usage: run_nonui_offline.py [--out DIR]     Exit code 0 and every NONUI_*_PASS token on success. Needs `dotnet` (SDK 8+).
"""
import argparse
import os
import shutil
import subprocess
import sys
import tempfile

HERE = os.path.dirname(os.path.abspath(__file__))
ROOT = os.path.abspath(os.path.join(HERE, "..", "..", ".."))
GAME = os.path.join(ROOT, "Assets", "ForestPrototype")

# Production files that must stay free of UnityEngine so this check can compile them. Add a file here when it joins the check.
PURE_SOURCES = [
    os.path.join(GAME, "UI", "CompetitorKeyboardTarget.cs"),
    os.path.join(GAME, "UI", "ScenarioOneTeachingSequence.cs"),
    os.path.join(GAME, "UI", "ScenarioOneRackBayText.cs"),
    os.path.join(GAME, "ScenarioOne", "ScenarioOneRackBay.cs"),
    os.path.join(GAME, "Racks", "RackGeometry.cs"),
    os.path.join(GAME, "ScenarioOne", "ScenarioOneRackReview.cs"),
    os.path.join(GAME, "StandApproachRoute.cs"),
]
CHECK_DIR = os.path.join(HERE, "OfflineLogic")

PROJECT = """<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <LangVersion>9.0</LangVersion>
    <Nullable>disable</Nullable>
    <ImplicitUsings>disable</ImplicitUsings>
    <EnableDefaultCompileItems>false</EnableDefaultCompileItems>
    <StartupObject>NonUiOfflineMain</StartupObject>
  </PropertyGroup>
  <ItemGroup>
{items}
  </ItemGroup>
</Project>
"""


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--out", default="/media/jer/Files/CCF/Verification/ScenarioOneNonUiOffline")
    args = parser.parse_args()
    dotnet = shutil.which("dotnet")
    if not dotnet:
        sys.exit("dotnet SDK not found")
    checks = sorted(os.path.join(CHECK_DIR, name) for name in os.listdir(CHECK_DIR) if name.endswith(".cs"))
    sources = PURE_SOURCES + checks
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
    run = subprocess.run([dotnet, "run", "--no-build", "-c", "Release", "--", ROOT], cwd=args.out, env=env, capture_output=True, text=True)
    sys.stdout.write(run.stdout)
    sys.stderr.write(run.stderr)
    return run.returncode


if __name__ == "__main__":
    sys.exit(main())
