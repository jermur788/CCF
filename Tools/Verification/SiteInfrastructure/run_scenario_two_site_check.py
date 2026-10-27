#!/usr/bin/env python3
"""Build and run the offline check of the Scenario Two species site rules on the plain .NET SDK.

Compiles ONLY ForestSiteProfile.cs and SpeciesSiteResponse.cs (no UnityEngine, no species asset code) with
OfflineLogic/ScenarioTwoSiteRulesOfflineCheck.cs in a temporary project OUTSIDE the repository. It lives in this folder because
the dormant site-infrastructure checks forbid any other verification tool from naming the site types.

Usage: run_scenario_two_site_check.py [--out DIR]
Exit code 0 and SCENARIO_TWO_SITE_RULES_OFFLINE_PASS on success. Needs `dotnet` (SDK 8+).
"""
import argparse, os, shutil, subprocess, sys, tempfile

HERE = os.path.dirname(os.path.abspath(__file__))
ROOT = os.path.abspath(os.path.join(HERE, "..", "..", ".."))
FP = os.path.join(ROOT, "Assets", "ForestPrototype")
SOURCES = [os.path.join(FP, "ForestSiteProfile.cs"), os.path.join(FP, "SpeciesSiteResponse.cs"),
           os.path.join(HERE, "OfflineLogic", "ScenarioTwoSiteRulesOfflineCheck.cs")]
PROJECT = """<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net8.0</TargetFramework><LangVersion>9.0</LangVersion><Nullable>disable</Nullable><ImplicitUsings>disable</ImplicitUsings><EnableDefaultCompileItems>false</EnableDefaultCompileItems><StartupObject>ScenarioTwoSiteRulesOfflineCheck</StartupObject></PropertyGroup>
<ItemGroup>
{items}
</ItemGroup></Project>
"""

def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--out", default=os.path.join(tempfile.gettempdir(), "ccf-s2-site-offline"))
    args = parser.parse_args()
    dotnet = shutil.which("dotnet")
    if not dotnet:
        sys.exit("dotnet SDK not found")
    for source in SOURCES:
        if not os.path.isfile(source):
            sys.exit("missing source " + source)
    os.makedirs(args.out, exist_ok=True)
    with open(os.path.join(args.out, "check.csproj"), "w") as handle:
        handle.write(PROJECT.format(items="\n".join('<Compile Include="%s" />' % s for s in SOURCES)))
    env = dict(os.environ, DOTNET_CLI_TELEMETRY_OPTOUT="1", DOTNET_NOLOGO="1")
    build = subprocess.run([dotnet, "build", "-nologo", "-v", "q", "-c", "Release"], cwd=args.out, env=env, capture_output=True, text=True)
    if build.returncode != 0:
        sys.stdout.write(build.stdout); sys.stderr.write(build.stderr)
        return build.returncode
    run = subprocess.run([dotnet, "run", "--no-build", "-c", "Release", "--", ROOT], cwd=args.out, env=env, capture_output=True, text=True)
    sys.stdout.write(run.stdout); sys.stderr.write(run.stderr)
    return run.returncode

if __name__ == "__main__":
    sys.exit(main())
