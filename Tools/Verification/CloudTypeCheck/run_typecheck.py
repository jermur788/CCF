#!/usr/bin/env python3
"""Type-check CCF C# sources without Unity, for code-only (cloud) sessions.

WHAT THIS IS: a compile of the runtime sources against community reference
assemblies for UnityEngine 2021.3 (NuGet `UnityEngine.Modules`) plus two
checked-in stubs (InputSystemStub.cs, UnityEditorStub.cs). It catches syntax
errors, missing members and wrong types in code that is edited without Unity.

WHAT THIS IS NOT: it never runs the game, it is not Unity's compiler, and the
2021.3 API surface differs from Unity 6000.6. The known differences appear as a
constant set of errors recorded in baseline-*.txt. Only errors that are NOT in
the baseline are meaningful; a clean diff means "no new compile errors against
this approximation", never "Unity compiles it".

Modes:
  prod   production sources under Assets/ (Editor folders excluded).
  tools  + UNITY_EDITOR defined and the Tools/Verification harnesses, so the
         disposable in-Editor gates can be checked too.

Needs the .NET SDK (8+) and, the first time, network access to nuget.org.
The generated project lives in a temporary directory outside the repository.

Usage: run_typecheck.py [--mode prod|tools] [--out DIR] [--update-baseline]
Exit status 0 when there are no errors beyond the baseline.
"""
import argparse
import os
import re
import shutil
import subprocess
import sys
import tempfile

HERE = os.path.dirname(os.path.abspath(__file__))
ROOT = os.path.abspath(os.path.join(HERE, "..", "..", ".."))

# Harnesses that inject low-level input need InputSystem.LowLevel, which is not stubbed.
TOOLS_EXCLUDE = [
    "Tools/Verification/**/OfflineLogic/**/*.cs",
    "Tools/Verification/MenuTutorialVerification.cs",
    "Tools/Verification/Startup/StartupScreenVerification.cs",
    "Tools/Verification/WindthrowV1/StormUiVerification.cs",
    # SCENARIO-ONE-UX-PEDAGOGY-V1 disposable Editor harnesses: they use Input System and Editor APIs this 2021.3 approximation does not stub
    # (InputSystem.QueueDeltaStateEvent / settings, Mouse.WarpCursorPosition, EditorWindow.position / maximized). Their real compile check is
    # Unity's own compiler against the project's references (Tools/Verification/ScenarioOneUxPedagogy/README.md).
    "Tools/Verification/ScenarioOneUxPedagogy/CameraLookDiagnosis.cs",
    "Tools/Verification/ScenarioOneUxPedagogy/UxPedagogyShots.cs",
    # SCENARIO-ONE-PLAYER-TEST-REMEDIATION-V1 combined-integration harness: Editor window reflection and Unity.Profiling recorders; checked by Unity's own compiler.
    "Tools/Verification/ScenarioOneCombined/CombinedHudComposition.cs",
]

PROJECT = """<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <OutputType>Library</OutputType>
    <LangVersion>9.0</LangVersion>
    <Nullable>disable</Nullable>
    <ImplicitUsings>disable</ImplicitUsings>
    <EnableDefaultCompileItems>false</EnableDefaultCompileItems>
    <GenerateAssemblyInfo>false</GenerateAssemblyInfo>
    <NoWarn>CS0169;CS0649;CS0414;CS0618;CS1701;CS0219;CS0168;CS0067;CS0108;CS0105;CS8981;NU1701</NoWarn>
    {define}
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="UnityEngine.Modules" Version="2021.3.33" />
    <PackageReference Include="Newtonsoft.Json" Version="13.0.3" />
  </ItemGroup>
  <ItemGroup>
{items}
  </ItemGroup>
</Project>
"""


def build_items(mode):
    items = ['    <Compile Include="%s" />' % os.path.join(HERE, "InputSystemStub.cs")]
    if mode == "tools":
        items.append('    <Compile Include="%s" />' % os.path.join(HERE, "UnityEditorStub.cs"))
    exclude = ";".join(os.path.join(ROOT, p) for p in ["Assets/**/Editor/**/*.cs"])
    items.append('    <Compile Include="%s" Exclude="%s" />' % (os.path.join(ROOT, "Assets/**/*.cs"), exclude))
    if mode == "tools":
        tool_exclude = ";".join(os.path.join(ROOT, p) for p in TOOLS_EXCLUDE)
        items.append('    <Compile Include="%s" Exclude="%s" />' % (os.path.join(ROOT, "Tools/Verification/**/*.cs"), tool_exclude))
    return "\n".join(items)


def normalise(line):
    """path(line,col): error CSnnnn: message [project]  ->  relpath: error CSnnnn: message"""
    match = re.match(r"^(.*?)\(\d+,\d+\): (error CS\d+: .*?)(?: \[[^\]]*\])?\s*$", line.strip())
    if not match:
        return None
    path = match.group(1)
    if path.startswith(ROOT + os.sep):
        path = path[len(ROOT) + 1:]
    return "%s: %s" % (path, match.group(2))


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--mode", choices=["prod", "tools"], default="prod")
    parser.add_argument("--out", default=os.path.join(tempfile.gettempdir(), "ccf-typecheck"))
    parser.add_argument("--update-baseline", action="store_true")
    args = parser.parse_args()
    dotnet = shutil.which("dotnet")
    if not dotnet:
        sys.exit("dotnet SDK not found")
    work = os.path.join(args.out, args.mode)
    os.makedirs(work, exist_ok=True)
    define = "<DefineConstants>$(DefineConstants);UNITY_EDITOR</DefineConstants>" if args.mode == "tools" else ""
    with open(os.path.join(work, "typecheck.csproj"), "w") as handle:
        handle.write(PROJECT.format(items=build_items(args.mode), define=define))
    env = dict(os.environ, DOTNET_CLI_TELEMETRY_OPTOUT="1", DOTNET_NOLOGO="1")
    result = subprocess.run([dotnet, "build", "-nologo", "-v", "q", "-o", os.path.join(work, "out")],
                            cwd=work, env=env, capture_output=True, text=True)
    output = result.stdout + result.stderr
    errors = sorted({n for n in (normalise(l) for l in output.splitlines() if " error CS" in l) if n})
    if not errors and result.returncode != 0:
        sys.stdout.write(output)
        sys.exit("build failed without compiler errors (restore/network problem?)")
    baseline_path = os.path.join(HERE, "baseline-%s.txt" % args.mode)
    if args.update_baseline:
        with open(baseline_path, "w") as handle:
            handle.write("\n".join(errors) + ("\n" if errors else ""))
        print("baseline updated: %d errors -> %s" % (len(errors), baseline_path))
        return 0
    baseline = set()
    if os.path.isfile(baseline_path):
        baseline = {l.rstrip("\n") for l in open(baseline_path) if l.strip()}
    new = [e for e in errors if e not in baseline]
    resolved = sorted(baseline - set(errors))
    print("[%s] distinct errors: %d (baseline %d)" % (args.mode, len(errors), len(baseline)))
    for line in new:
        print("NEW  " + line)
    for line in resolved:
        print("GONE " + line)
    print("TYPECHECK_%s mode=%s" % ("CLEAN" if not new else "NEW_ERRORS", args.mode))
    return 0 if not new else 1


if __name__ == "__main__":
    sys.exit(main())
