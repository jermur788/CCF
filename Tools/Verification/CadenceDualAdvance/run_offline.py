#!/usr/bin/env python3
import pathlib, subprocess, tempfile
root=pathlib.Path(__file__).resolve().parents[3]
with tempfile.TemporaryDirectory(prefix='ccf-cadence-') as d:
 p=pathlib.Path(d)
 (p/'check.csproj').write_text('<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net8.0</TargetFramework><EnableDefaultCompileItems>false</EnableDefaultCompileItems></PropertyGroup><ItemGroup><Compile Include="'+str(root/'Assets/ForestPrototype/ScenarioOne/ScenarioManagementCadence.cs')+'"/><Compile Include="'+str(pathlib.Path(__file__).parent/'OfflineLogic/Check.cs')+'"/></ItemGroup></Project>')
 subprocess.run(['dotnet','run','--project',str(p/'check.csproj')],check=True)
