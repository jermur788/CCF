#!/usr/bin/env python3
"""MFM-08 offline/static gate: compile the pure review model + map plan against the production MFM-04/05/07 pure sources, run the checks twice,
then verify the changed-file boundary against the pinned start commit. No Unity, saves or Assets writes; build output goes outside the repo."""
import argparse
import datetime
import importlib.util
import os
from pathlib import Path
import shutil
import subprocess
import sys

sys.dont_write_bytecode = True  # never leave __pycache__ inside the repository
HERE = Path(__file__).resolve().parent


def load(rel, name):
    spec = importlib.util.spec_from_file_location(name, HERE.parent / rel)
    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    return module


core = load("MfmPrescriptionCore/run_prescription_core_offline.py", "core")
approval = load("MfmApprovalBoundary/run_approval_boundary_offline.py", "approval")
START = "cfdc93bf3943125d21570bc5749a9338f52d1cce"
ALLOWED = (
    "Assets/ForestPrototype/UI/StandMapView.cs",
    "Assets/ForestPrototype/UI/TreeInspectionView.cs",
    "Assets/ForestPrototype/UI/ScenarioOneTreatmentReview.cs",
    "Assets/ForestPrototype/UI/StandMapTreatmentReview.cs",
    "Assets/ForestPrototype/UI/Map/StandMapReviewPlan.cs",
    "Assets/ForestPrototype/UI/Map/StandMapReviewLayer.cs",
    "Assets/ForestPrototype/UI/Map/StandMapAreaFootprints.cs",  # Geometry Repair 01: the one shared area-footprint derivation
    "Tools/Verification/MfmTreatmentReview/",
    "Docs/Handoffs/MFM-08-Map-3D-Review-UX/",
    "Docs/Handoffs/MFM-08-Geometry-Repair-01/",
)


def boundary(root):
    """Every file changed since START (committed or not) must be an allowed MFM-08 file, a .meta of one, or a Docs handoff."""
    names = subprocess.run(["git", "-C", str(root), "diff", "--name-only", START], capture_output=True, text=True, check=True).stdout.split()
    names += subprocess.run(["git", "-C", str(root), "ls-files", "--others", "--exclude-standard"], capture_output=True, text=True, check=True).stdout.split()
    bad = []
    for name in sorted(set(names)):
        base = name[:-5] if name.endswith(".meta") else name
        if not any(base == a or (a.endswith("/") and base.startswith(a)) for a in ALLOWED):
            bad.append(name)
    return sorted(set(names)), bad


def main():
    parser = argparse.ArgumentParser()
    stamp = datetime.datetime.now(datetime.timezone.utc).strftime("%Y%m%dT%H%M%SZ")
    parser.add_argument("--out", default="/media/jer/Files/CCF/Verification/MFM08-TreatmentReview-" + stamp)
    out = Path(parser.parse_args().out).resolve()
    root = Path(core.ROOT)
    if out == root or root in out.parents:
        sys.exit("--out must be outside the repository")
    dotnet = shutil.which("dotnet")
    if not dotnet:
        sys.exit("dotnet SDK not found")
    game = Path(core.GAME)
    approval.OUT_DIR = str(out)
    consts = approval.slice_save_constants(str(game / "ForestSaveData.cs"))
    pure = list(approval.PURE_SOURCES) + [consts, str(game / "UI/ScenarioOneTreatmentReview.cs"), str(game / "UI/Map/StandMapReviewPlan.cs"), str(game / "UI/Map/StandMapAreaFootprints.cs")]
    sources = pure + [str(HERE / "OfflineLogic/TreatmentReviewOfflineCheck.cs"), str(HERE / "OfflineLogic/AreaFootprintChecks.cs")]
    for s in sources:
        if not os.path.isfile(s):
            sys.exit("missing source " + s)
    build = out / "build"
    build.mkdir(parents=True, exist_ok=True)
    project = core.PROJECT.replace("PrescriptionCoreOfflineCheck", "TreatmentReviewOfflineCheck")
    (build / "check.csproj").write_text(project.format(items="\n".join('    <Compile Include="%s" />' % s for s in sources)))
    env = dict(os.environ, DOTNET_CLI_TELEMETRY_OPTOUT="1", DOTNET_NOLOGO="1")
    result = subprocess.run([dotnet, "build", "-nologo", "-v", "q", "-c", "Release", "-warnaserror"], cwd=build, env=env, capture_output=True, text=True)
    (out / "build.log").write_text(result.stdout + result.stderr)
    if result.returncode:
        print(result.stdout + result.stderr)
        return result.returncode
    print("MFM08_BUILD_PASS sources=%d warnings-as-errors" % len(sources))
    outputs = []
    for attempt in (1, 2):
        run = subprocess.run([dotnet, "run", "--no-build", "-c", "Release", "--", str(root)], cwd=build, env=env, capture_output=True, text=True)
        text = run.stdout + run.stderr
        (out / ("run%d.log" % attempt)).write_text(text)
        outputs.append((run.returncode, text))
    print(outputs[0][1], end="")
    same = outputs[0] == outputs[1]
    print("MFM08_REPEAT_%s two processes, byte-identical output: %s" % ("PASS" if same else "FAIL", same))
    changed, bad = boundary(root)
    print("MFM08_BOUNDARY_%s changed=%d outside-owned-surface=%s" % ("PASS" if not bad else "FAIL", len(changed), bad))
    shutil.rmtree(build / "bin", ignore_errors=True)
    shutil.rmtree(build / "obj", ignore_errors=True)
    return 0 if same and outputs[0][0] == 0 and not bad else 1


if __name__ == "__main__":
    sys.exit(main())
