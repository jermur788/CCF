#!/usr/bin/env python3
"""Build and run the #358 V2 witness offline check on the plain .NET SDK, outside the repository.

Compiles the PRODUCTION map files (no UnityEngine) from a given Unity lane/worktree plus StandMapV2Witness.cs (its UNITY_EDITOR half is
not compiled) and StandMapV2WitnessOfflineCheck.cs. Usage: run_v2witness_offline.py --lane <worktree> --probe <dir> [--out DIR]
"""
import argparse, os, shutil, subprocess, sys, tempfile
ap = argparse.ArgumentParser()
ap.add_argument("--lane", default="/media/jer/ZX20/CCF-U3"); ap.add_argument("--probe", default=os.path.dirname(os.path.abspath(__file__)))
ap.add_argument("--out", default=os.path.join(tempfile.gettempdir(), "ccf358-v2witness-offline"))
a = ap.parse_args()
MAP = os.path.join(a.lane, "Assets", "ForestPrototype", "UI", "Map")
SOURCES = [os.path.join(a.lane, "Assets", "ForestPrototype", "Racks", "RackGeometry.cs")] + [os.path.join(MAP, n) for n in (
    "StandMapTransform.cs", "StandMapModel.cs", "StandMapSymbols.cs", "StandMapGridPlan.cs", "StandMapDrawPlan.cs", "StandMapCanopyRaster.cs", "StandMapOverlay.cs", "StandMapLegend.cs")] + [
    os.path.join(a.probe, "StandMapV2Witness.cs"), os.path.join(a.probe, "StandMapV2WitnessOfflineCheck.cs")]
for s in SOURCES:
    if not os.path.isfile(s): sys.exit("missing source " + s)
os.makedirs(a.out, exist_ok=True)
proj = ('<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net8.0</TargetFramework><LangVersion>9.0</LangVersion>'
        '<Nullable>disable</Nullable><ImplicitUsings>disable</ImplicitUsings><EnableDefaultCompileItems>false</EnableDefaultCompileItems>'
        '<StartupObject>StandMapV2WitnessOfflineCheck</StartupObject></PropertyGroup><ItemGroup>\n%s\n</ItemGroup></Project>' % "\n".join('<Compile Include="%s" />' % s for s in SOURCES))
open(os.path.join(a.out, "check.csproj"), "w").write(proj)
env = dict(os.environ, DOTNET_CLI_TELEMETRY_OPTOUT="1", DOTNET_NOLOGO="1")
b = subprocess.run(["dotnet", "build", "-nologo", "-v", "q", "-c", "Release"], cwd=a.out, env=env, capture_output=True, text=True)
if b.returncode != 0: sys.stdout.write(b.stdout); sys.stderr.write(b.stderr); sys.exit(b.returncode)
r = subprocess.run(["dotnet", "run", "--no-build", "-c", "Release"], cwd=a.out, env=env, capture_output=True, text=True)
sys.stdout.write(r.stdout); sys.stderr.write(r.stderr); sys.exit(r.returncode)
