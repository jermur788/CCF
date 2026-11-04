#!/usr/bin/env python3
"""Build and run the MFM-07 proposal-state and prescription-approval boundary checks on the plain .NET SDK (no Unity).

Compiles the PRODUCTION pure sources (no UnityEngine) with the existing offline ForestEcologyController stand-in and the checks in
OfflineLogic/, in a build directory OUTSIDE the repository, then runs the checks TWICE and requires byte-identical output (cross-process
determinism). Nothing is written inside the repository.

Fixture: the MFM-03 frozen inputs (fixture-stand.csv, fixture-infrastructure.json). They are read from
Tools/Verification/MfmCoverageCalibration/inputs/ when that tooling is merged, otherwise from the pinned MFM-03 commit in Git, and their
SHA-256 must match the values below either way.

Usage: run_prescription_core_offline.py [--out DIR]    Exit 0 and MFM07_PRESCRIPTION_CORE_PASS on success. Needs `dotnet` (SDK 8+) and git.
"""
import argparse
import datetime
import hashlib
import os
import shutil
import subprocess
import sys

HERE = os.path.dirname(os.path.abspath(__file__))
ROOT = os.path.abspath(os.path.join(HERE, "..", "..", ".."))
GAME = os.path.join(ROOT, "Assets", "ForestPrototype")

PURE_SOURCES = [
    os.path.join(GAME, "ScenarioOne", "ScenarioOneCoverage.cs"),
    os.path.join(GAME, "ScenarioOne", "ScenarioOnePrescription.cs"),
    os.path.join(GAME, "ScenarioOne", "ScenarioOneCropTreeCandidates.cs"),
    os.path.join(GAME, "ScenarioOne", "ScenarioOnePrescriptionCalibration.cs"),
    os.path.join(GAME, "ScenarioOne", "ScenarioOnePrescriptionApproval.cs"),
    os.path.join(GAME, "ScenarioOne", "ScenarioOneRackSampleGeometry.cs"),
    os.path.join(GAME, "ScenarioOne", "ScenarioOneRackBay.cs"),
    os.path.join(GAME, "ScenarioOne", "CompetitionShare.cs"),
    os.path.join(GAME, "Racks", "RackGeometry.cs"),
    # The one impure dependency (Hegyi term); the check compares it with the real ForestEcologyController.cs source text.
    os.path.join(ROOT, "Tools", "Verification", "ScenarioOneNonUi", "OfflineLogic", "OfflineStubs.cs"),
]
CHECK_DIR = os.path.join(HERE, "OfflineLogic")

MFM03_COMMIT = "73356e3"  # research/mfm-coverage-calibration-v1: MFM-03 handoff 60792dc + independent review
MFM03_INPUTS = "Tools/Verification/MfmCoverageCalibration/inputs"
FIXTURES = {
    "fixture-stand.csv": "4b4b9703e65fe01146e57deb2095ea4f2cf3b702243c73075cdc2bf52f7f2433",
    "fixture-infrastructure.json": "c7dd5b811c975c6bf6cd50a201308424c4f2eed6b9dca30f80300dc885644630",
}

PROJECT = """<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <LangVersion>9.0</LangVersion>
    <Nullable>disable</Nullable>
    <ImplicitUsings>disable</ImplicitUsings>
    <EnableDefaultCompileItems>false</EnableDefaultCompileItems>
    <StartupObject>ApprovalBoundaryOfflineCheck</StartupObject>
  </PropertyGroup>
  <ItemGroup>
{items}
  </ItemGroup>
</Project>
"""


def fixture(name, out_dir):
    data = None
    in_tree = os.path.join(ROOT, MFM03_INPUTS, name)
    if os.path.isfile(in_tree):
        with open(in_tree, "rb") as handle:
            data = handle.read()
        origin = in_tree
    else:
        data = subprocess.run(["git", "-C", ROOT, "show", "%s:%s/%s" % (MFM03_COMMIT, MFM03_INPUTS, name)], capture_output=True, check=True).stdout
        origin = "git %s:%s/%s" % (MFM03_COMMIT, MFM03_INPUTS, name)
    digest = hashlib.sha256(data).hexdigest()
    if digest != FIXTURES[name]:
        sys.exit("fixture %s has SHA-256 %s, expected %s" % (origin, digest, FIXTURES[name]))
    path = os.path.join(out_dir, name)
    with open(path, "wb") as handle:
        handle.write(data)
    print("MFM07_FIXTURE %s sha256=%s from %s" % (name, digest, origin))
    return path


def slice_save_constants(path):
    """Extracts PrescriptionApprovalState / PrescriptionCoverageChoice / PrescriptionReasonCode verbatim from ForestSaveData.cs.

    The approval boundary reads these three closed vocabularies; they live in the Unity-dependent save file, so the offline build compiles
    the exact production text of just those classes (never a copy that could drift) into a file outside the repository."""
    text = open(path).read()
    start = text.index("public static class PrescriptionApprovalState")
    end = text.index("[Serializable]\npublic sealed class PrescriptionCoverageDecisionSaveData")
    os.makedirs(OUT_DIR, exist_ok=True)
    out = os.path.join(OUT_DIR, "SaveConstants.cs")
    with open(out, "w") as handle:
        handle.write("using System;\n" + text[start:end])
    return out


OUT_DIR = None


def main():
    global OUT_DIR
    stamp = datetime.datetime.now(datetime.timezone.utc).strftime("%Y%m%dT%H%M%SZ")
    parser = argparse.ArgumentParser()
    parser.add_argument("--out", default="/media/jer/Files/CCF/Verification/MFM07-ApprovalBoundary-" + stamp)
    args = parser.parse_args()
    out = os.path.abspath(args.out)
    OUT_DIR = out
    if out == ROOT or out.startswith(ROOT + os.sep):
        sys.exit("--out must be outside the repository")
    dotnet = shutil.which("dotnet")
    if not dotnet:
        sys.exit("dotnet SDK not found")
    consts = slice_save_constants(os.path.join(GAME, "ForestSaveData.cs"))
    checks = sorted(os.path.join(CHECK_DIR, name) for name in os.listdir(CHECK_DIR) if name.endswith(".cs"))
    sources = PURE_SOURCES + checks + [consts]
    for source in sources:
        if not os.path.isfile(source):
            sys.exit("missing source " + source)
    build_dir = os.path.join(out, "build")
    os.makedirs(build_dir, exist_ok=True)
    stand = fixture("fixture-stand.csv", out)
    infra = fixture("fixture-infrastructure.json", out)
    with open(os.path.join(build_dir, "check.csproj"), "w") as handle:
        handle.write(PROJECT.format(items="\n".join('    <Compile Include="%s" />' % s for s in sources)))
    env = dict(os.environ, DOTNET_CLI_TELEMETRY_OPTOUT="1", DOTNET_NOLOGO="1")
    build = subprocess.run([dotnet, "build", "-nologo", "-v", "q", "-c", "Release", "-warnaserror"], cwd=build_dir, env=env, capture_output=True, text=True)
    if build.returncode != 0:
        sys.stdout.write(build.stdout)
        sys.stderr.write(build.stderr)
        return build.returncode
    print("MFM07_BUILD_PASS sources=%d warnings-as-errors" % len(sources))
    outputs = []
    for attempt in (1, 2):
        run = subprocess.run([dotnet, "run", "--no-build", "-c", "Release", "--", ROOT, stand, infra], cwd=build_dir, env=env, capture_output=True, text=True)
        with open(os.path.join(out, "run%d.log" % attempt), "w") as handle:
            handle.write(run.stdout + run.stderr)
        outputs.append((run.returncode, run.stdout))
    sys.stdout.write(outputs[0][1])
    same = outputs[0] == outputs[1]
    print("MFM07_REPEAT_%s two processes, byte-identical output: %s" % ("PASS" if same else "FAIL", same))
    shutil.rmtree(os.path.join(build_dir, "bin"), ignore_errors=True)
    shutil.rmtree(os.path.join(build_dir, "obj"), ignore_errors=True)
    return 0 if outputs[0][0] == 0 and same else 1


if __name__ == "__main__":
    sys.exit(main())
