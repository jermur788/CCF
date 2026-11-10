#!/usr/bin/env python3
"""Run only the #363/#365 focused assertions against a compiled runtime assembly. No Unity or save access.
Usage: run_checks.py <check.dll> <out dir> [source root]. The source root (default: this repository) is read for the #365
HUD source contract, which runs first and fails closed. CCF_FEEDBACK_SCRATCH chooses where the pixel-witness plans are written."""
import pathlib, subprocess, sys
here=pathlib.Path(__file__).resolve().parent
sys.dont_write_bytecode=True  # never leave __pycache__ in a source lane
sys.path.insert(0,str(here))
import hud_contract
assembly=pathlib.Path(sys.argv[1]).resolve(); out=pathlib.Path(sys.argv[2]).resolve();out.mkdir(parents=True,exist_ok=True)
source=pathlib.Path(sys.argv[3]).resolve() if len(sys.argv)>3 else here.parents[2]
found=hud_contract.problems(hud_contract.read(source))
for f in found: print('HUD SOURCE CONTRACT FAIL: '+f)
if found: sys.exit(1)
print('HUD source contract PASS (source text only, not a rendered HUD)')
src=here/'Checks.cs'
(out/'checks.csproj').write_text(f"""<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net8.0</TargetFramework><EnableDefaultCompileItems>false</EnableDefaultCompileItems></PropertyGroup><ItemGroup><Compile Include="{src}"/><Reference Include="{assembly}"/></ItemGroup></Project>""")
subprocess.run(['dotnet','run','--project',str(out/'checks.csproj'),'-v','q'],check=True)
