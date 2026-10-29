#!/usr/bin/env python3
"""Build and run the S1-D T-04 guide-monotonicity offline check on the plain .NET SDK (production ScenarioOneTeachingHistory.cs and
ScenarioOneTeachingSequence.cs). --mutations breaks the helper and requires the check to FAIL each time. Proves logic and source
wiring only; no Unity behaviour. Usage: run_teaching_history_offline.py [--out DIR] [--mutations]"""
import argparse, os, shutil, subprocess, sys, tempfile
HERE = os.path.dirname(os.path.abspath(__file__))
ROOT = os.path.abspath(os.path.join(HERE, "..", "..", ".."))
GAME = os.path.join(ROOT, "Assets", "ForestPrototype")
NONUI = os.path.join(HERE, "..", "ScenarioOneNonUi", "OfflineLogic")
HIST = os.path.join(GAME, "UI", "ScenarioOneTeachingHistory.cs")
SOURCES = ["{hist}", os.path.join(GAME, "UI", "ScenarioOneTeachingSequence.cs"), os.path.join(GAME, "ScenarioOne", "ScenarioOneRackBay.cs"),
    os.path.join(GAME, "ScenarioOne", "CompetitionShare.cs"), os.path.join(GAME, "Racks", "RackGeometry.cs"),
    os.path.join(NONUI, "OfflineStubs.cs"), os.path.join(HERE, "TeachingHistoryOfflineCheck.cs")]
MUTATIONS = [
    ("no review needed", "e.Kind == Kind.Acknowledged && e.Id > both", "e.Kind == Kind.Acknowledged"),
    ("footprint ignored", "if (!e.InFootprint) continue;", ""),
    ("planting before review counts", "if (e.Id <= review) continue;", ""),
    ("goals unrecorded still establish", "!beechRecorded || !oakRecorded", "false"),
    ("same-year review establishes", "if (ack.Year > plantingYear)", "if (ack.Year >= plantingYear)"),
    ("floor drops with later work", "return Transformation;", "return Establishment;"),
]

PROJECT = """<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <LangVersion>9.0</LangVersion>
    <Nullable>disable</Nullable>
    <ImplicitUsings>disable</ImplicitUsings>
    <EnableDefaultCompileItems>false</EnableDefaultCompileItems>
    <StartupObject>TeachingHistoryOfflineCheck</StartupObject>
  </PropertyGroup>
  <ItemGroup>
{items}
  </ItemGroup>
</Project>
"""


def build_and_run(dotnet, work, hist_source):
    os.makedirs(work, exist_ok=True)
    sources = [hist_source if s == "{hist}" else s for s in SOURCES]
    for source in sources:
        if not os.path.isfile(source):
            sys.exit("missing source " + source)
    with open(os.path.join(work, "check.csproj"), "w") as handle:
        handle.write(PROJECT.format(items="\n".join('    <Compile Include="%s" />' % s for s in sources)))
    env = dict(os.environ, DOTNET_CLI_TELEMETRY_OPTOUT="1", DOTNET_NOLOGO="1", CCF_REPO_ROOT=ROOT)
    build = subprocess.run([dotnet, "build", "-nologo", "-v", "q", "-c", "Release"], cwd=work, env=env, capture_output=True, text=True)
    if build.returncode != 0:
        return "BUILD_FAILED", build.stdout + build.stderr
    run = subprocess.run([dotnet, "run", "--no-build", "-c", "Release"], cwd=work, env=env, capture_output=True, text=True)
    return ("OK" if run.returncode == 0 else "CHECK_FAILED"), run.stdout + run.stderr



def main():
    p = argparse.ArgumentParser(); p.add_argument("--out", default=os.path.join(tempfile.gettempdir(), "ccf-s1d-t04-offline")); p.add_argument("--mutations", action="store_true")
    a = p.parse_args(); dotnet = shutil.which("dotnet")
    if not dotnet: sys.exit("dotnet SDK not found")
    status, out = build_and_run(dotnet, a.out, HIST); sys.stdout.write(out)
    if status != "OK": return 1
    if not a.mutations: return 0
    original = open(HIST).read(); survived = []
    for n, (name, find, rep) in enumerate(MUTATIONS):
        if original.count(find) != 1: sys.exit("mutation '%s': expected one match, found %d" % (name, original.count(find)))
        d = os.path.join(a.out, "mutant%d" % n); os.makedirs(d, exist_ok=True); src = os.path.join(d, "ScenarioOneTeachingHistory.cs")
        open(src, "w").write(original.replace(find, rep)); st, o = build_and_run(dotnet, d, src)
        if st == "CHECK_FAILED": print("MUTATION CAUGHT   %s" % name)
        else: print("MUTATION SURVIVED %s (%s)" % (name, st)); survived.append(name)
    print("TEACHING_HISTORY_MUTATIONS_%s caught=%d survived=%d" % ("PASS" if not survived else "FAIL", len(MUTATIONS) - len(survived), len(survived)))
    return 0 if not survived else 1

if __name__ == "__main__": sys.exit(main())
