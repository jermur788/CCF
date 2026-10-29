#!/usr/bin/env python3
"""Build and run the S1-D T-03 teaching-cap offline check on the plain .NET SDK.

Compiles PRODUCTION ScenarioOneTeachingCap.cs with ScenarioOneRackBay.cs / ScenarioOneRackBayText.cs (UnityEngine-free) and this
folder's TeachingCapOfflineCheck.cs in a temporary project OUTSIDE the repository. Also asserts, from Git, that the protected files
(save schema, completion rules, objectives, rack analysis/lifecycle, economy) have no diff against the T-03 base.

--mutations compiles deliberately broken copies of ScenarioOneTeachingCap.cs and requires the check to FAIL for each.
Usage: run_teaching_cap_offline.py [--out DIR] [--mutations] [--base SHA]   Needs `dotnet` (SDK 8+) and `git`.
Proves the decision table and call-site source text only; it does not prove any Unity behaviour.
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
CAP_SOURCE = os.path.join(GAME, "UI", "ScenarioOneTeachingCap.cs")
NONUI = os.path.join(HERE, "..", "ScenarioOneNonUi", "OfflineLogic")
DEFAULT_BASE = "23fe4b8"

SOURCES = [
    "{cap}",
    os.path.join(GAME, "UI", "ScenarioOneTeachingSequence.cs"),
    os.path.join(GAME, "UI", "ScenarioOneRackBayText.cs"),
    os.path.join(GAME, "ScenarioOne", "ScenarioOneRackBay.cs"),
    os.path.join(GAME, "ScenarioOne", "CompetitionShare.cs"),
    os.path.join(GAME, "Racks", "RackGeometry.cs"),
    os.path.join(NONUI, "OfflineStubs.cs"),
    os.path.join(HERE, "TeachingCapOfflineCheck.cs"),
]

# Paths T-03 must not change (no save diff, completion/protected anchors, rack analysis, economy/ecology, Rack/MFM/S1-C UI).
PROTECTED = [
    "Assets/ForestPrototype/ForestSaveData.cs",
    "Assets/ForestPrototype/ForestSaveValidation.cs",
    "Assets/ForestPrototype/ScenarioOne/ScenarioOneCompletionRules.cs",
    "Assets/ForestPrototype/ScenarioOne/ScenarioOneObjectives.cs",
    "Assets/ForestPrototype/ScenarioOne/ScenarioOneRackBay.cs",
    "Assets/ForestPrototype/ScenarioOne/ScenarioOneRackReview.cs",
    "Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.RackPlan.cs",
    "Assets/ForestPrototype/ScenarioOne/ScenarioInterventionHistory.cs",
    "Assets/ForestPrototype/ScenarioOne/ScenarioCycleState.cs",
    "Assets/ForestPrototype/UI/AnnualReviewView.cs",
    "Assets/ForestPrototype/UI/ScenarioOneTeachingSequence.cs",
    "Assets/ForestPrototype/UI/ScenarioOneRackBayText.cs",
    "Assets/ForestPrototype/Racks",
    "Assets/ForestPrototype/WorkEconomy",
    "Assets/Scenes",
    "ProjectSettings",
    "Packages",
]

MUTATIONS = [
    ("cap never relaxes (always Hard)",
     "return SampleHistoricallyComplete(bay, releaseReviewed) ? Phase.Advisory : Phase.Hard;", "return Phase.Hard;"),
    ("advisory before completion",
     "return SampleHistoricallyComplete(bay, releaseReviewed) ? Phase.Advisory : Phase.Hard;", "return Phase.Advisory;"),
    ("completion depends on the cap",
     "bay.Applies && releaseReviewed && bay.ReleaseYear >= 0", "bay.Applies && bay.CanopyKept && releaseReviewed && bay.ReleaseYear >= 0"),
    ("results need not be read",
     "bay.Applies && releaseReviewed && bay.ReleaseYear >= 0", "bay.Applies && bay.ReleaseYear >= 0"),
    ("hard message loses the quantity",
     'Percent(bay.RemovedBasalShare) + " of its trunk area removed or planned for removal', '"some of its trunk area removed or planned for removal'),
    ("advisory turns into an instruction",
     "It does not block Approve Work Plan or Advance;", "Cancel removals."),
]

PROJECT = """<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <LangVersion>9.0</LangVersion>
    <Nullable>disable</Nullable>
    <ImplicitUsings>disable</ImplicitUsings>
    <EnableDefaultCompileItems>false</EnableDefaultCompileItems>
    <StartupObject>TeachingCapOfflineCheck</StartupObject>
  </PropertyGroup>
  <ItemGroup>
{items}
  </ItemGroup>
</Project>
"""


def build_and_run(dotnet, work, cap_source):
    os.makedirs(work, exist_ok=True)
    sources = [cap_source if s == "{cap}" else s for s in SOURCES]
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


def protected_diff(base):
    out = subprocess.run(["git", "-C", ROOT, "diff", "--stat", base, "--"] + PROTECTED, capture_output=True, text=True)
    if out.returncode != 0:
        sys.exit("git diff failed: " + out.stderr)
    return out.stdout.strip()


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--out", default=os.path.join(tempfile.gettempdir(), "ccf-s1d-teaching-cap-offline"))
    parser.add_argument("--mutations", action="store_true")
    parser.add_argument("--base", default=DEFAULT_BASE)
    args = parser.parse_args()
    dotnet = shutil.which("dotnet")
    if not dotnet:
        sys.exit("dotnet SDK not found")

    status, output = build_and_run(dotnet, args.out, CAP_SOURCE)
    sys.stdout.write(output)
    if status != "OK":
        return 1
    diff = protected_diff(args.base)
    if diff:
        print("TEACHING_CAP_PROTECTED_DIFF_FAIL\n" + diff)
        return 1
    print("TEACHING_CAP_PROTECTED_DIFF_PASS base=%s paths=%d (no save/completion/rack-analysis/economy/scene/package diff)" % (args.base, len(PROTECTED)))
    if not args.mutations:
        return 0

    with open(CAP_SOURCE) as handle:
        original = handle.read()
    survived = []
    for index, (name, find, replace) in enumerate(MUTATIONS):
        if original.count(find) != 1:
            sys.exit("mutation '%s': expected exactly one match, found %d" % (name, original.count(find)))
        mutant_dir = os.path.join(args.out, "mutant%d" % index)
        os.makedirs(mutant_dir, exist_ok=True)
        mutant_source = os.path.join(mutant_dir, "ScenarioOneTeachingCap.cs")
        with open(mutant_source, "w") as handle:
            handle.write(original.replace(find, replace))
        mutant_status, mutant_output = build_and_run(dotnet, mutant_dir, mutant_source)
        failing = [line for line in mutant_output.splitlines() if line.startswith("FAIL")]
        if mutant_status == "CHECK_FAILED":
            print("MUTATION CAUGHT   %-36s %d failing check(s), e.g. %s" % (name, len(failing), failing[0][:100] if failing else "?"))
        else:
            print("MUTATION SURVIVED %-36s (%s)" % (name, mutant_status))
            survived.append(name)
    print("TEACHING_CAP_MUTATIONS_%s caught=%d survived=%d" % ("PASS" if not survived else "FAIL", len(MUTATIONS) - len(survived), len(survived)))
    return 0 if not survived else 1


if __name__ == "__main__":
    sys.exit(main())
