#!/usr/bin/env python3
"""Build and run the S1-C T-05 (save v22 objective history) offline check on the plain .NET SDK. No Unity, nothing written
inside the repository.

Two builds in a temporary project OUTSIDE the repo:
  1. BASELINE: ForestSaveData.cs and the hash path of ScenarioReferenceArchive.cs exactly as they were at the pre-v22 base
     commit (aca74ac, save v21). It prints the world-identity hashes of the shared fixture worlds.
  2. CANDIDATE: the working-tree production sources (v22 schema, validators, migration, hash path, history query,
     ScenarioOneManager.ObjectiveHistory.cs). It must reproduce every baseline hash and pass every rule check.

Usage: run_offline_check.py [--out DIR] [--base SHA]
Exit 0 and the S1C_T05_OFFLINE_PASS token on success. Needs `dotnet` (SDK 8+) and git.
"""
import argparse, glob, os, re, shutil, subprocess, sys, tempfile

HERE = os.path.dirname(os.path.abspath(__file__))
ROOT = os.path.abspath(os.path.join(HERE, "..", "..", ".."))
FP = os.path.join(ROOT, "Assets", "ForestPrototype")
ONE = os.path.join(FP, "ScenarioOne")
N3 = os.path.join(ROOT, "Tools", "Verification", "ScenarioOneCompletion", "N3", "OfflineLogic")
BASE_DEFAULT = "aca74ac"


def git_show(ref, path):
    return subprocess.run(["git", "-C", ROOT, "show", ref + ":" + path], capture_output=True, text=True, check=True).stdout


def archive_bridge(source):
    """The pure hash/canonical part of ScenarioReferenceArchive (everything from Canonicalize on), wrapped as the class."""
    tail = source[source.index("public static void Canonicalize(ForestSaveData data)"):]
    return "using System;\nusing System.Collections.Generic;\nusing System.Text;\nusing UnityEngine;\npublic static class ScenarioReferenceArchive\n{\n    " + tail


def project(sources, startup):
    items = "".join('<Compile Include="%s" />' % s for s in sources)
    return ('<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><TargetFramework>net8.0</TargetFramework><OutputType>Exe</OutputType>'
            '<LangVersion>9.0</LangVersion><Nullable>disable</Nullable><EnableDefaultCompileItems>false</EnableDefaultCompileItems>'
            '<ImplicitUsings>disable</ImplicitUsings><StartupObject>%s</StartupObject></PropertyGroup><ItemGroup>'
            '<PackageReference Include="Newtonsoft.Json" Version="13.0.3" />%s</ItemGroup></Project>' % (startup, items))


def dotnet(args, cwd, env):
    return subprocess.run(["dotnet"] + args, cwd=cwd, env=env, capture_output=True, text=True)


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--out", default=os.path.join(tempfile.gettempdir(), "ccf-s1c-t05-offline"))
    parser.add_argument("--base", default=BASE_DEFAULT)
    args = parser.parse_args()
    if not shutil.which("dotnet"):
        sys.exit("dotnet SDK not found")
    out = args.out
    shutil.rmtree(out, ignore_errors=True)
    base_dir, cand_dir = os.path.join(out, "baseline"), os.path.join(out, "candidate")
    os.makedirs(base_dir); os.makedirs(cand_dir)

    shim = open(os.path.join(ROOT, "Tools/Verification/ThreeSpecies/OfflineLogic/UnityShim.cs")).read()
    shim = shim.replace("public static class Mathf\n    {", "public static class Mathf\n    {\n        public static int RoundToInt(float v)=>(int)Math.Round(v);\n"
                        "        public static int CeilToInt(float v)=>(int)Math.Ceiling(v);\n        public static int FloorToInt(float v)=>(int)Math.Floor(v);\n"
                        "        public static float Repeat(float t, float length) => t - (float)Math.Floor(t / length) * length;")
    facts = open(os.path.join(FP, "UI", "ScenarioOneUiFacts.cs")).read()
    if "public static string SquareLabel(int cellIndex)" not in facts or '"#" + cellIndex' not in facts:
        sys.exit("ScenarioOneUiFacts.SquareLabel changed; update the offline shim")
    for d in (base_dir, cand_dir):
        open(os.path.join(d, "UnityShim.cs"), "w").write(shim)
        open(os.path.join(d, "UiFactsBridge.cs"), "w").write('public static class ScenarioOneUiFacts { public static string SquareLabel(int cellIndex) => "#" + cellIndex; }\n')

    shared = [os.path.join(ONE, f) for f in ["ScenarioManagementEvent.cs", "ScenarioOneWorkOrder.cs", "ScenarioEcologicalSnapshot.cs", "ScenarioDeadwoodRecord.cs"]] \
        + [os.path.join(FP, f) for f in ["RackPlanningModel.cs", "StandGeometryModel.cs", "RegenerationModel.cs", "SitkaGrowthModel.cs"]] \
        + sorted(glob.glob(os.path.join(FP, "WorkEconomy", "*.cs"))) + [os.path.join(N3, "SaveNeighbours.cs"),
          os.path.join(HERE, "HashFixtures.cs")]
    def shims(directory):
        return [os.path.join(directory, "UnityShim.cs"), os.path.join(directory, "UiFactsBridge.cs")]

    # ---- baseline (pre-v22 sources from git) ----
    open(os.path.join(base_dir, "ForestSaveData.cs"), "w").write(git_show(args.base, "Assets/ForestPrototype/ForestSaveData.cs"))
    open(os.path.join(base_dir, "ScenarioReferenceArchive.cs"), "w").write(archive_bridge(git_show(args.base, "Assets/ForestPrototype/ScenarioOne/ScenarioReferenceArchive.cs")))
    base_sources = shared + shims(base_dir) + [os.path.join(base_dir, n) for n in ("ForestSaveData.cs", "ScenarioReferenceArchive.cs")] + [os.path.join(HERE, "OldDumpMain.cs")]
    base_sources = list(dict.fromkeys(base_sources))
    open(os.path.join(base_dir, "baseline.csproj"), "w").write(project(base_sources, "OldDumpMain"))

    # ---- candidate (working tree) ----
    objectives = open(os.path.join(ONE, "ScenarioOneObjectives.cs")).read()
    match = re.search(r"\[Serializable\]\s*public sealed class ScenarioObjectiveResult\s*\{.*?\n\}", objectives, re.S)
    if not match:
        sys.exit("ScenarioObjectiveResult not found in ScenarioOneObjectives.cs")
    open(os.path.join(cand_dir, "ObjectiveResultExtract.cs"), "w").write("using System;\n" + match.group(0) + "\n")
    open(os.path.join(cand_dir, "ArchiveBridge.cs"), "w").write(archive_bridge(open(os.path.join(ONE, "ScenarioReferenceArchive.cs")).read()))
    cand_sources = shared + shims(cand_dir) + [os.path.join(FP, f) for f in ["ForestSaveData.cs", "ForestSaveValidation.cs", "ForestSaveValidation.Geometry.cs",
        "ForestSaveValidation.Storms.cs", "ForestSaveValidation.RackV2.cs"]] \
        + [os.path.join(ONE, f) for f in ["ScenarioOneSaveMigration.cs", "ScenarioManagementHistory.cs", "ScenarioInterventionHistory.cs", "ManagementHistoryText.cs",
           "ScenarioOneManager.ObjectiveHistory.cs"]] \
        + [os.path.join(cand_dir, n) for n in ("ObjectiveResultExtract.cs", "ArchiveBridge.cs")] \
        + [os.path.join(HERE, "ManagerStub.cs"), os.path.join(HERE, "ObjectiveHistoryOfflineCheck.cs")]
    cand_sources = list(dict.fromkeys(cand_sources))
    open(os.path.join(cand_dir, "candidate.csproj"), "w").write(project(cand_sources, "ObjectiveHistoryOfflineCheck"))

    env = dict(os.environ, DOTNET_CLI_TELEMETRY_OPTOUT="1", DOTNET_NOLOGO="1", CCF_REPO_ROOT=ROOT)
    for label, directory in (("baseline", base_dir), ("candidate", cand_dir)):
        build = dotnet(["build", "-nologo", "-v", "q", "-c", "Release"], directory, env)
        if build.returncode != 0:
            sys.stdout.write(build.stdout); sys.stderr.write(build.stderr)
            print("BUILD FAILED: " + label)
            return build.returncode
    dump = os.path.join(out, "baseline-hashes.txt")
    run = dotnet(["run", "--no-build", "-c", "Release", "--", dump], base_dir, env)
    sys.stdout.write(run.stdout)
    if run.returncode != 0 or "version=21" not in run.stdout:
        sys.stderr.write(run.stderr)
        print("BASELINE FAILED (expected a save v21 baseline)")
        return run.returncode or 1
    run = dotnet(["run", "--no-build", "-c", "Release", "--", dump], cand_dir, env)
    sys.stdout.write(run.stdout); sys.stderr.write(run.stderr)
    if run.returncode != 0:
        return run.returncode
    # Reference Future v1 and every other embedded resource must be byte-identical to the base commit.
    resources = "Assets/ForestPrototype/ScenarioOne/Resources"
    changed = subprocess.run(["git", "-C", ROOT, "diff", "--name-only", args.base, "--", resources], capture_output=True, text=True).stdout.strip()
    untracked = subprocess.run(["git", "-C", ROOT, "ls-files", "--others", "--exclude-standard", "--", resources], capture_output=True, text=True).stdout.strip()
    if changed or untracked:
        print("REFERENCE_RESOURCES_CHANGED: " + changed + untracked)
        return 1
    print("REFERENCE_FUTURE_V1_RESOURCES_UNCHANGED_VS_" + args.base)
    return 0


if __name__ == "__main__":
    sys.exit(main())
