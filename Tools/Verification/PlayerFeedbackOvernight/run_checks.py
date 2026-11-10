#!/usr/bin/env python3
"""Run only #363 focused assertions against a compiled runtime assembly. No Unity or save access."""
import pathlib, subprocess, sys
assembly=pathlib.Path(sys.argv[1]).resolve(); out=pathlib.Path(sys.argv[2]).resolve();out.mkdir(parents=True,exist_ok=True)
src=pathlib.Path(__file__).with_name('Checks.cs').resolve()
(out/'checks.csproj').write_text(f'''<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net8.0</TargetFramework><EnableDefaultCompileItems>false</EnableDefaultCompileItems></PropertyGroup><ItemGroup><Compile Include="{src}"/><Reference Include="{assembly}"/></ItemGroup></Project>''')
subprocess.run(['dotnet','run','--project',str(out/'checks.csproj'),'-v','q'],check=True)
