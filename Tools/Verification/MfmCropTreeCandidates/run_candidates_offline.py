#!/usr/bin/env python3
"""Compile production pure sources and MFM-05 checks outside the repository; run twice, compare output.

Uses MFM-04's inspected source manifest/project settings. No Unity, saves or Assets writes.
"""
import argparse
import datetime
import importlib.util
import os
from pathlib import Path
import shutil
import subprocess
import sys

HERE = Path(__file__).resolve().parent
spec = importlib.util.spec_from_file_location("core", HERE.parent / "MfmPrescriptionCore/run_prescription_core_offline.py")
core = importlib.util.module_from_spec(spec)
spec.loader.exec_module(core)


def main():
    parser = argparse.ArgumentParser()
    stamp = datetime.datetime.now(datetime.timezone.utc).strftime("%Y%m%dT%H%M%SZ")
    parser.add_argument("--out", default="/media/jer/Files/CCF/Verification/MFM05-Candidates-" + stamp)
    out = Path(parser.parse_args().out).resolve()
    root = Path(core.ROOT)
    if out == root or root in out.parents:
        sys.exit("--out must be outside the repository")
    dotnet = shutil.which("dotnet")
    if not dotnet:
        sys.exit("dotnet SDK not found")
    sources = core.PURE_SOURCES + [str(HERE / "OfflineLogic/CropTreeCandidatesOfflineCheck.cs")]
    build = out / "build"
    build.mkdir(parents=True, exist_ok=True)
    project = core.PROJECT.replace("PrescriptionCoreOfflineCheck", "CropTreeCandidatesOfflineCheck")
    (build / "check.csproj").write_text(project.format(items="\n".join('    <Compile Include="%s" />' % s for s in sources)))
    env = dict(os.environ, DOTNET_CLI_TELEMETRY_OPTOUT="1", DOTNET_NOLOGO="1")
    result = subprocess.run([dotnet, "build", "-nologo", "-v", "q", "-c", "Release", "-warnaserror"], cwd=build, env=env, capture_output=True, text=True)
    (out / "build.log").write_text(result.stdout + result.stderr)
    if result.returncode:
        print(result.stdout + result.stderr)
        return result.returncode
    print("MFM05_BUILD_PASS sources=%d warnings-as-errors" % len(sources))
    outputs = []
    for attempt in (1, 2):
        result = subprocess.run([dotnet, "run", "--no-build", "-c", "Release", "--", str(root)], cwd=build, env=env, capture_output=True, text=True)
        output = result.stdout + result.stderr
        (out / ("run%d.log" % attempt)).write_text(output)
        outputs.append((result.returncode, output))
    print(outputs[0][1], end="")
    same = outputs[0] == outputs[1]
    print("MFM05_REPEAT_%s two processes, byte-identical output: %s" % ("PASS" if same else "FAIL", same))
    shutil.rmtree(build / "bin", ignore_errors=True)
    shutil.rmtree(build / "obj", ignore_errors=True)
    return 0 if same and outputs[0][0] == 0 else 1


if __name__ == "__main__":
    sys.exit(main())
