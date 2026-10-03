#!/usr/bin/env python3
"""Run one disposable Unity gate with isolated game persistence and cleanup.

Reuses locally cached Unity licensing files without logging their contents.
No real game save is copied or written. No dependencies beyond stdlib/installed Unity.
"""
import argparse
import os
from pathlib import Path
import shutil
import subprocess


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("gate")
    parser.add_argument("--method", default="Begin")
    parser.add_argument("--unity", default="/media/jer/ZX20/Unity/6000.6.0f1/Editor/Unity")
    args = parser.parse_args()
    root = Path(__file__).resolve().parents[2]
    source = root / "Tools" / "Verification" / (args.gate + ".cs")
    target = root / "Assets" / "ForestPrototype" / source.name
    sandbox = Path("/tmp/opencode/ccf-economy-isolated-config")
    licenses = Path.home() / ".config" / "unity3d" / "Unity" / "licenses"
    if licenses.is_dir():
        shutil.copytree(licenses, sandbox / "unity3d" / "Unity" / "licenses", dirs_exist_ok=True)
    output = root / "Build" / "ScenarioOneEconomyGates"
    output.mkdir(parents=True, exist_ok=True)
    log = output / (args.gate + "-" + args.method + ".log")
    copied = False
    if source.is_file():
        if target.exists():
            raise RuntimeError("A gate copy already exists; remove/verify it before running the launcher.")
        shutil.copy2(source, target)
        copied = True
    try:
        environment = dict(os.environ, XDG_CONFIG_HOME=str(sandbox))
        run = subprocess.run([args.unity, "-batchmode", "-nographics", "-projectPath", str(root),
            "-executeMethod", args.gate + "." + args.method, "-logFile", str(log)], env=environment, cwd=root)
        text = log.read_text(encoding="utf-8", errors="replace")
        for line in text.splitlines():
            if "VERIFY_PASS" in line or "VERIFY_FAIL" in line or "ECONOMY_VIABILITY" in line or " error CS" in line or "No valid Unity Editor license" in line:
                print(line)
        if run.returncode != 0 or "VERIFY_FAIL" in text or "No valid Unity Editor license" in text:
            raise RuntimeError(f"Gate failed; inspect {log}")
        if "VERIFY_PASS" not in text and "SCENARIO_ONE_ECONOMY_VIABILITY_PASS" not in text:
            raise RuntimeError(f"Gate returned without a pass marker; inspect {log}")
        print("ISOLATED_GATE_PASS log=" + str(log))
    finally:
        if copied:
            target.unlink(missing_ok=True)
            Path(str(target) + ".meta").unlink(missing_ok=True)


if __name__ == "__main__":
    main()
