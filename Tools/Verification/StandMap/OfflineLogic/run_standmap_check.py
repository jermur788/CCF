#!/usr/bin/env python3
"""Build and run the shared-map (B5/B8) logic check on the plain .NET SDK.

Compiles the PRODUCTION files RackGeometry.cs (RackPoint) and UI/Map/StandMapTransform.cs, StandMapModel.cs, StandMapSymbols.cs
and StandMapDrawPlan.cs (none uses UnityEngine) with StandMapOfflineCheck.cs in a temporary project OUTSIDE the repository and
runs it. No Unity Editor, no Library, nothing is written inside the repo.

Proves: the one world-to-map transform (north up, round trip, cell grid tiling), that the same world point has the same
fractional position in the full-map and minimap plans, the shared symbol table and Mini/Full filter, heading, and that a change
in the model reaches both plans. It does not prove anything about the Unity canvas, the layout or the live data source.

Usage: run_standmap_check.py [--out DIR]     Exit code 0 and STANDMAP_OFFLINE_PASS on success. Needs `dotnet` (SDK 8+).
"""
import argparse, os, shutil, subprocess, sys, tempfile

HERE = os.path.dirname(os.path.abspath(__file__))
ROOT = os.path.abspath(os.path.join(HERE, "..", "..", "..", ".."))
MAP = os.path.join(ROOT, "Assets", "ForestPrototype", "UI", "Map")
SOURCES = [
    os.path.join(ROOT, "Assets", "ForestPrototype", "Racks", "RackGeometry.cs"),
    os.path.join(MAP, "StandMapTransform.cs"),
    os.path.join(MAP, "StandMapModel.cs"),
    os.path.join(MAP, "StandMapSymbols.cs"),
    os.path.join(MAP, "StandMapDrawPlan.cs"),
    os.path.join(HERE, "StandMapOfflineCheck.cs"),
]
PROJECT = """<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net8.0</TargetFramework><LangVersion>9.0</LangVersion><Nullable>disable</Nullable><ImplicitUsings>disable</ImplicitUsings><EnableDefaultCompileItems>false</EnableDefaultCompileItems><StartupObject>StandMapOfflineCheck</StartupObject></PropertyGroup>
<ItemGroup>
{items}
</ItemGroup></Project>
"""


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--out", default=os.path.join(tempfile.gettempdir(), "ccf-standmap-offline"))
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
        sys.stdout.write(build.stdout)
        sys.stderr.write(build.stderr)
        return build.returncode
    run = subprocess.run([dotnet, "run", "--no-build", "-c", "Release"], cwd=args.out, env=env, capture_output=True, text=True)
    sys.stdout.write(run.stdout)
    sys.stderr.write(run.stderr)
    return run.returncode


if __name__ == "__main__":
    sys.exit(main())
