#!/usr/bin/env python3
"""PARITY-08 offline check: saved species identity is validated before load, on the plain .NET SDK. No Unity.

Compiles the working-tree save sources (the same candidate set as MfmSaveProvenance/run_offline_check.py) with
Parity08SpeciesOfflineCheck.cs in a temporary project OUTSIDE the repository and runs it.

Usage: run_offline_check.py [--out DIR]
Exit 0 and the PARITY08_SPECIES_OFFLINE_PASS token on success. Needs `dotnet` (SDK 8+).
"""
import argparse, glob, importlib.util, os, re, shutil, subprocess, sys, tempfile

HERE = os.path.dirname(os.path.abspath(__file__))
MFM = os.path.join(HERE, "..", "MfmSaveProvenance")
spec = importlib.util.spec_from_file_location("mfm_runner", os.path.join(MFM, "run_offline_check.py"))
mfm = importlib.util.module_from_spec(spec)
spec.loader.exec_module(mfm)
FP, ONE, N3 = mfm.FP, mfm.ONE, mfm.N3


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--out", default=os.path.join(tempfile.gettempdir(), "ccf-parity08-offline"))
    args = parser.parse_args()
    if not shutil.which("dotnet"):
        sys.exit("dotnet SDK not found")
    d = args.out
    shutil.rmtree(d, ignore_errors=True)
    os.makedirs(d)
    shim = open(os.path.join(mfm.ROOT, "Tools/Verification/ThreeSpecies/OfflineLogic/UnityShim.cs")).read()
    shim = shim.replace("public static class Mathf\n    {", "public static class Mathf\n    {\n        public static int RoundToInt(float v)=>(int)Math.Round(v);\n"
                        "        public static int CeilToInt(float v)=>(int)Math.Ceiling(v);\n        public static int FloorToInt(float v)=>(int)Math.Floor(v);\n"
                        "        public static float Repeat(float t, float length) => t - (float)Math.Floor(t / length) * length;")
    open(os.path.join(d, "UnityShim.cs"), "w").write(shim)
    open(os.path.join(d, "UiFactsBridge.cs"), "w").write('public static class ScenarioOneUiFacts { public static string SquareLabel(int cellIndex) => "#" + cellIndex; }\n')
    prescription = open(os.path.join(ONE, "ScenarioOnePrescription.cs")).read()
    match = re.search(r"public enum ProposalReason\s*\{[^}]*\}", prescription)
    open(os.path.join(d, "ReasonExtract.cs"), "w").write("public static class ScenarioOnePrescription\n{\n    " + match.group(0) + "\n}\n")
    open(os.path.join(d, "ArchiveBridge.cs"), "w").write(mfm.archive_bridge(open(os.path.join(ONE, "ScenarioReferenceArchive.cs")).read()))
    sources = [os.path.join(ONE, f) for f in ["ScenarioManagementEvent.cs", "ScenarioOneWorkOrder.cs", "ScenarioEcologicalSnapshot.cs", "ScenarioDeadwoodRecord.cs"]] \
        + [os.path.join(FP, f) for f in ["RackPlanningModel.cs", "StandGeometryModel.cs", "RegenerationModel.cs", "SitkaGrowthModel.cs"]] \
        + sorted(glob.glob(os.path.join(FP, "WorkEconomy", "*.cs"))) + [os.path.join(N3, "SaveNeighbours.cs")] \
        + [os.path.join(d, n) for n in ("UnityShim.cs", "UiFactsBridge.cs", "ReasonExtract.cs", "ArchiveBridge.cs")] \
        + [os.path.join(FP, f) for f in ["ForestSaveData.cs", "ForestSaveValidation.cs", "ForestSaveValidation.Geometry.cs",
           "ForestSaveValidation.Storms.cs", "ForestSaveValidation.RackV2.cs"]] \
        + [os.path.join(ONE, f) for f in ["ScenarioOneSaveMigration.cs", "ScenarioOneManager.Prescriptions.cs"]] \
        + [os.path.join(MFM, "ManagerStub.cs"), os.path.join(HERE, "Parity08SpeciesOfflineCheck.cs")]
    open(os.path.join(d, "check.csproj"), "w").write(mfm.project(list(dict.fromkeys(sources)), "Parity08SpeciesOfflineCheck"))
    env = dict(os.environ, DOTNET_CLI_TELEMETRY_OPTOUT="1", DOTNET_NOLOGO="1", CCF_REPO_ROOT=mfm.ROOT)
    build = mfm.dotnet(["build", "-nologo", "-v", "q", "-c", "Release"], d, env)
    if build.returncode != 0:
        sys.stdout.write(build.stdout); sys.stderr.write(build.stderr)
        print("BUILD FAILED")
        return build.returncode
    run = mfm.dotnet(["run", "--no-build", "-c", "Release"], d, env)
    sys.stdout.write(run.stdout); sys.stderr.write(run.stderr)
    return run.returncode


if __name__ == "__main__":
    sys.exit(main())
