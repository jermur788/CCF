#!/usr/bin/env python3
"""Build and run the S1-D T-04 guide-monotonicity offline matrix on the plain .NET SDK.

Compiles the PRODUCTION ScenarioOneTeachingSequence.cs with the pure rack-area types, the generated read-only latch
member from ScenarioOneManager.cs and this folder's TeachingSequenceMonotonicityOfflineCheck.cs in a temporary project
OUTSIDE the repository. It also audits the source wiring and asserts that no unowned production file changed against
the exact T-04 start.

The check covers FR-B1/FR-B2/FR-B3/AC-3: pre-latch equivalence with the accepted sequence, ReleaseReviewed without
RackReviewComplete, the authoritative response -> recruitment boundary and one-way write, post-latch Crop Tree
increase/decrease/loss, later in-footprint felling, executed-share drift, save/load/rebuild at lessons 6-8, the missing-
latch conservative default, repeated read purity and all eight stable lesson IDs.

This proves pure rules and source text only. It does not prove Unity Editor import, rendering, input, a real serialized
save or the scheduled Unity qualification.

Usage: run_teaching_sequence_offline.py --out DIR [--base SHA] [--mutations]
Output must be under /media/jer/Files/CCF/Verification. Needs `dotnet` (SDK 8+) and `git`.
"""
import argparse
import json
import os
import shutil
import subprocess
import sys
from pathlib import Path

HERE = Path(__file__).resolve().parent
ROOT = HERE.parents[2]
GAME = ROOT / "Assets" / "ForestPrototype"
START = "bccd2b90b3eafc3728a9f29ac421b2719d042d72"
SEQUENCE = GAME / "UI" / "ScenarioOneTeachingSequence.cs"
OWNED_PRODUCTION = {
    "Assets/ForestPrototype/UI/ScenarioOneTeachingSequence.cs",
    "Assets/ForestPrototype/UI/LearningObjectivesView.Sequence.cs",
}
SOURCES = [
    GAME / "UI" / "ScenarioOneRackBayText.cs",
    GAME / "ScenarioOne" / "ScenarioOneRackBay.cs",
    GAME / "ScenarioOne" / "CompetitionShare.cs",
    GAME / "Racks" / "RackGeometry.cs",
    ROOT / "Tools" / "Verification" / "ScenarioOneNonUi" / "OfflineLogic" / "OfflineStubs.cs",
    HERE / "TeachingSequenceMonotonicityOfflineCheck.cs",
]
MUTATIONS = [
    ("latch floor removed",
     "=> f.FirstGuidedRackReviewComplete ? RecruitmentAndLater(f) : CurrentUnlatched(f);",
     "=> CurrentUnlatched(f);"),
    ("floor drops behind recruitment on Crop Tree drift",
     "? RecruitmentAndLater(f) : CurrentUnlatched(f)",
     "? (f.CropTrees == 3 ? RecruitmentAndLater(f) : 4) : CurrentUnlatched(f)"),
    ("lesson id renamed",
     'new Lesson("access",',
     'new Lesson("access-renamed",'),
    ("session review replaces the durable read",
     "f.FirstGuidedRackReviewComplete ? RecruitmentAndLater(f)",
     "f.RackReviewComplete ? RecruitmentAndLater(f)"),
    ("pre-latch path gets a floor",
     "=> f.FirstGuidedRackReviewComplete ? RecruitmentAndLater(f) : CurrentUnlatched(f);",
     "=> System.Math.Max(RecruitmentAndLater(f), CurrentUnlatched(f));"),
]
PROJECT = """<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <LangVersion>9.0</LangVersion>
    <Nullable>disable</Nullable>
    <ImplicitUsings>disable</ImplicitUsings>
    <EnableDefaultCompileItems>false</EnableDefaultCompileItems>
    <StartupObject>TeachingSequenceMonotonicityOfflineCheck</StartupObject>
  </PropertyGroup>
  <ItemGroup>
{items}
  </ItemGroup>
</Project>
"""


def git(*args):
    return subprocess.run(["git", "-C", str(ROOT), *args], capture_output=True, text=True)


def latch_manager(work):
    text = (GAME / "ScenarioOne" / "ScenarioOneManager.cs").read_text()
    start = text.index("    private bool firstGuidedRackReviewComplete;")
    end = text.index("    public void InitializeNewScenario()", start)
    bridge = work / "LatchManager.cs"
    bridge.write_text("public sealed partial class ScenarioOneManager { private bool referencePreviewActive;\n"
                      + text[start:end] + "}\n")
    return bridge


def build_and_run(dotnet, work, sequence):
    work.mkdir(parents=True, exist_ok=True)
    sources = [sequence] + SOURCES + [latch_manager(work)]
    for source in sources:
        if not Path(source).is_file():
            sys.exit("missing source " + str(source))
    items = "\n".join('    <Compile Include="%s" />' % source for source in sources)
    (work / "check.csproj").write_text(PROJECT.format(items=items))
    env = dict(os.environ, DOTNET_CLI_TELEMETRY_OPTOUT="1", DOTNET_NOLOGO="1", CCF_REPO_ROOT=str(ROOT))
    build = subprocess.run([dotnet, "build", "-nologo", "-v", "q", "-c", "Release"], cwd=work, env=env,
                           capture_output=True, text=True)
    if build.returncode != 0:
        return "BUILD_FAILED", build.stdout + build.stderr
    run = subprocess.run([dotnet, "run", "--no-build", "-c", "Release", "--", str(ROOT)], cwd=work, env=env,
                         capture_output=True, text=True)
    return ("OK" if run.returncode == 0 else "CHECK_FAILED"), run.stdout + run.stderr


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--out", required=True, type=Path)
    parser.add_argument("--base", default=START)
    parser.add_argument("--mutations", action="store_true")
    args = parser.parse_args()
    out = args.out.resolve()
    if not out.is_relative_to(Path("/media/jer/Files/CCF/Verification")):
        sys.exit("durable evidence must be under /media/jer/Files/CCF/Verification")
    dotnet = shutil.which("dotnet")
    if not dotnet:
        sys.exit("dotnet SDK not found")
    if git("cat-file", "-e", args.base + "^{commit}").returncode:
        sys.exit("base commit not found: " + args.base)
    status = git("status", "--porcelain", "--", "Assets/ForestPrototype")
    if status.returncode or status.stdout.strip():
        sys.exit("production tree is not clean:\n" + status.stdout + status.stderr)
    changed = [line for line in git("diff", "--name-only", args.base, "--", "Assets/ForestPrototype").stdout.splitlines() if line]
    unowned = sorted(set(changed) - OWNED_PRODUCTION)
    if unowned:
        sys.exit("unowned production changed against " + args.base + ":\n" + "\n".join(unowned))

    out.mkdir(parents=True, exist_ok=True)
    status, output = build_and_run(dotnet, out / "candidate", SEQUENCE)
    (out / "check.log").write_text(output)
    (out / "result.txt").write_text(output.splitlines()[-1] + "\n" if output.splitlines() else "")
    (out / "production-diff.txt").write_text(
        "base=" + args.base + "\n" + "\n".join(changed) + ("\n" if changed else "PRODUCTION_UNCHANGED\n"))
    (out / "command.json").write_text(json.dumps({
        "command": [str(Path(__file__)), "--out", str(out), "--base", args.base],
        "root": str(ROOT), "head": git("rev-parse", "HEAD").stdout.strip(),
        "base": args.base, "production_changed": changed,
    }, indent=2) + "\n")
    sys.stdout.write(output)
    if status != "OK":
        print("TEACHING_SEQUENCE_OFFLINE_FAIL")
        return 1
    if changed:
        print("T04_OWNED_PRODUCTION_CHANGED " + ",".join(changed))
    else:
        print("T04_PRODUCTION_UNCHANGED base=" + args.base)
    if not args.mutations:
        return 0

    original = SEQUENCE.read_text()
    survived = []
    for index, (name, find, replace) in enumerate(MUTATIONS):
        if original.count(find) != 1:
            sys.exit("mutation '%s': expected exactly one match, found %d" % (name, original.count(find)))
        mutant_dir = out / ("mutant%d" % index)
        mutant_dir.mkdir(parents=True, exist_ok=True)
        mutant = mutant_dir / "ScenarioOneTeachingSequence.cs"
        mutant.write_text(original.replace(find, replace))
        mutant_status, mutant_output = build_and_run(dotnet, mutant_dir, mutant)
        failing = [line for line in mutant_output.splitlines() if line.startswith("FAIL")]
        if mutant_status == "CHECK_FAILED":
            print("MUTATION CAUGHT   %-42s %s" % (name, failing[0][:110] if failing else "?"))
        else:
            print("MUTATION SURVIVED %-42s (%s)" % (name, mutant_status))
            survived.append(name)
    print("TEACHING_SEQUENCE_MUTATIONS_%s caught=%d survived=%d"
          % ("PASS" if not survived else "FAIL", len(MUTATIONS) - len(survived), len(survived)))
    return 0 if not survived else 1


if __name__ == "__main__":
    sys.exit(main())
