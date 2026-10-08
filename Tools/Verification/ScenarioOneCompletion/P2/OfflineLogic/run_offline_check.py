#!/usr/bin/env python3
"""Build and run the P2 offline logic check without the Unity Editor.

1. Type-checks this worktree's Assembly-CSharp sources (non-Editor) with the
   Roslyn compiler bundled with Unity, using the reference list and defines of
   a Unity-generated Assembly-CSharp.csproj from any local worktree of the same
   Unity version (default /home/jer/CCF-main).
2. Compiles CropTreeCompetitionOfflineCheck.cs against that assembly and runs it
   on Unity's bundled .NET runtime.

Lightweight (no Editor, no Library writes in this worktree). Outputs go to a
temporary build directory. Exit code 0 and P2_OFFLINE_PASS on success.

Usage: run_offline_check.py [--csproj PATH] [--unity PATH] [--out DIR]
"""
import argparse
import glob
import json
import os
import re
import shutil
import subprocess
import sys
import tempfile

HERE = os.path.dirname(os.path.abspath(__file__))
ROOT = os.path.abspath(os.path.join(HERE, "..", "..", "..", "..", ".."))
EVIDENCE = os.path.join(HERE, "..", "Evidence")


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--csproj", default="/home/jer/CCF-main/Assembly-CSharp.csproj")
    parser.add_argument("--unity", default="/media/jer/ZX20/Unity/6000.6.0f1/Editor")
    parser.add_argument("--out", default=os.path.join(tempfile.gettempdir(), "ccf-p2-offline"))
    args = parser.parse_args()

    sdk = os.path.join(args.unity, "Data", "DotNetSdk")
    dotnet = os.path.join(sdk, "dotnet")
    csc = glob.glob(os.path.join(sdk, "sdk", "*", "Roslyn", "bincore", "csc.dll"))[0]
    ref_pack = sorted(glob.glob(os.path.join(sdk, "packs", "Microsoft.NETCore.App.Ref", "*", "ref", "net8.0")))[-1]
    runtime_version = os.path.basename(sorted(glob.glob(os.path.join(sdk, "shared", "Microsoft.NETCore.App", "*")))[-1])
    os.makedirs(args.out, exist_ok=True)

    project = open(args.csproj, encoding="utf-8").read()
    base = os.path.dirname(args.csproj)
    refs = [r if os.path.isabs(r) else os.path.join(base, r) for r in re.findall(r"<HintPath>([^<]+)</HintPath>", project)]
    refs = [r for r in refs if not os.path.basename(r).startswith("Assembly-CSharp")]
    defines = re.search(r"<DefineConstants>([^<]+)", project).group(1)
    sources = [f for f in glob.glob(os.path.join(ROOT, "Assets", "**", "*.cs"), recursive=True) if "/Editor/" not in f]
    if not any(f.endswith("CropTreeCompetition.cs") for f in sources):
        sys.exit(f"Assets sources not found under {ROOT}")

    game = os.path.join(args.out, "Assembly-CSharp.dll")
    rsp = os.path.join(args.out, "game.rsp")
    with open(rsp, "w") as handle:
        handle.write("-nologo\n-target:library\n-nostdlib+\n-langversion:9.0\n-nowarn:0169,0649,0414,0618,1701,0219\n")
        handle.write(f"-out:{game}\n-define:{defines}\n")
        handle.writelines(f'-reference:"{r}"\n' for r in refs)
        handle.writelines(f'"{s}"\n' for s in sources)
    run([dotnet, csc, "@" + rsp], "GAME_ASSEMBLY")

    core = next(r for r in refs if os.path.basename(r) == "UnityEngine.CoreModule.dll")
    exe = os.path.join(args.out, "P2OfflineCheck.dll")
    check_rsp = os.path.join(args.out, "check.rsp")
    with open(check_rsp, "w") as handle:
        handle.write(f"-nologo\n-target:exe\n-nostdlib+\n-langversion:9.0\n-out:{exe}\n")
        handle.writelines(f'-reference:"{r}"\n' for r in sorted(glob.glob(os.path.join(ref_pack, "*.dll"))))
        handle.write(f'-reference:"{game}"\n-reference:"{core}"\n')
        handle.write(f'"{os.path.join(HERE, "CropTreeCompetitionOfflineCheck.cs")}"\n')
    run([dotnet, csc, "@" + check_rsp], "CHECK_ASSEMBLY")

    shutil.copy(core, args.out)
    with open(os.path.join(args.out, "P2OfflineCheck.runtimeconfig.json"), "w") as handle:
        json.dump({"runtimeOptions": {"tfm": "net8.0", "rollForward": "Major",
                                      "framework": {"name": "Microsoft.NETCore.App", "version": runtime_version}}}, handle)
    result = subprocess.run([dotnet, exe, EVIDENCE], capture_output=True, text=True)
    sys.stdout.write(result.stdout)
    sys.stderr.write(result.stderr)
    return result.returncode


def run(command, label):
    result = subprocess.run(command, capture_output=True, text=True)
    errors = [line for line in result.stdout.splitlines() if ": error " in line]
    print(f"{label}_{'OK' if result.returncode == 0 else 'FAILED'} errors={len(errors)}")
    for line in errors[:40]:
        print(line)
    if result.returncode != 0:
        sys.exit(result.returncode)


if __name__ == "__main__":
    sys.exit(main())
