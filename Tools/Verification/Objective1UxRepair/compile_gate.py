#!/usr/bin/env python3
"""Offline syntax/type gate against installed 6000.6.0f1 assemblies, including disposable Editor harness.
Does not import, launch Unity, or prove rendering/behaviour. Writes only to --out on Files.
"""
import argparse
import os
from pathlib import Path
import subprocess
from xml.sax.saxutils import escape
ap=argparse.ArgumentParser(); ap.add_argument('--out',type=Path,required=True); a=ap.parse_args()
root=Path(__file__).resolve().parents[3]; a.out.mkdir(parents=True,exist_ok=True)
managed=Path('/media/jer/ZX20/Unity/6000.6.0f1/Editor/Data/Managed')
refs=list((managed/'UnityEngine').glob('*.dll'))
refs += list((root/'Library/ScriptAssemblies').glob('*.dll'))
# Use package assemblies from the warm Library, excluding the project being compiled and Editor-only package wrappers.
refs=[p for p in refs if p.name not in ('Assembly-CSharp.dll','Assembly-CSharp-Editor.dll')]
refs=list({p.name:p for p in refs}.values())
sources=[p for p in (root/'Assets').rglob('*.cs') if 'Editor' not in p.parts]+[Path(__file__).with_name('Objective1UxNative.cs')]
project='<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><TargetFramework>netstandard2.1</TargetFramework><LangVersion>9.0</LangVersion><DefineConstants>UNITY_EDITOR</DefineConstants><EnableDefaultCompileItems>false</EnableDefaultCompileItems><AllowUnsafeBlocks>true</AllowUnsafeBlocks><NoWarn>CS0618;CS0649;CS0169;CS0414;CS0067</NoWarn></PropertyGroup><ItemGroup>'
project+=''.join('<Compile Include="'+escape(str(p))+'" />' for p in sources)
project+=''.join('<Reference Include="'+escape(p.stem)+'"><HintPath>'+escape(str(p))+'</HintPath></Reference>' for p in refs)
project+='</ItemGroup></Project>'; (a.out/'gate.csproj').write_text(project)
r=subprocess.run(['dotnet','build',str(a.out/'gate.csproj'),'-c','Release','-v','q','-clp:ErrorsOnly;NoSummary'],env=dict(os.environ,DOTNET_CLI_TELEMETRY_OPTOUT='1',DOTNET_NOLOGO='1'),text=True,stdout=subprocess.PIPE,stderr=subprocess.STDOUT)
print(r.stdout); print('OBJECTIVE1_UX_OFFLINE_COMPILE_'+('PASS' if r.returncode==0 else 'FAIL')+' sources='+str(len(sources))+' refs='+str(len(refs))); raise SystemExit(r.returncode)
