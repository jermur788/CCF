#!/usr/bin/env python3
"""Compile the game's runtime C# (Assets/**, excluding Editor folders) against the Unity runtime assemblies of an already-built Player.
A SYNTAX/TYPE gate only: it is not Unity's own compile (no source generators, no Editor assembly, no analyzers) and proves nothing about behaviour."""
import argparse, os, subprocess, sys, shutil
ap = argparse.ArgumentParser()
ap.add_argument('--src', required=True, help='directory that contains Assets/')
ap.add_argument('--out', required=True)
ap.add_argument('--extra', nargs='*', default=[], help='extra .cs files (e.g. disposable harnesses)')
ap.add_argument('--managed', default='/home/jer/ccf-verify-player/CCFVerify_Data/Managed')
a = ap.parse_args()
skip = ('Assembly-CSharp', 'mscorlib', 'netstandard', 'System', 'Mono.', 'Microsoft.', 'Unity.Collections.Tests', 'UnityEngine.UI.Tests')
refs = []
for n in sorted(os.listdir(a.managed)):
    if n.endswith('.dll') and not n.startswith(skip) and n != 'System.dll':
        refs.append(os.path.join(a.managed, n))
srcs = []
for root, dirs, files in os.walk(os.path.join(a.src, 'Assets')):
    dirs[:] = [d for d in dirs if d != 'Editor']
    for f in files:
        if f.endswith('.cs'): srcs.append(os.path.join(root, f))
srcs += [os.path.abspath(x) for x in a.extra]
os.makedirs(a.out, exist_ok=True)
proj = '''<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>netstandard2.1</TargetFramework><LangVersion>9.0</LangVersion><Nullable>disable</Nullable><ImplicitUsings>disable</ImplicitUsings>
    <EnableDefaultCompileItems>false</EnableDefaultCompileItems><AllowUnsafeBlocks>true</AllowUnsafeBlocks><TreatWarningsAsErrors>false</TreatWarningsAsErrors>
    <NoWarn>CS0618;CS0649;CS0169;CS0414;CS0219;CS0162;CS0168;CS1591;CS8632;NU1701;CS0067;CS0108;CS0114;CS0659;CS0661;CS0660</NoWarn>
    <GenerateAssemblyInfo>false</GenerateAssemblyInfo><DisableImplicitFrameworkReferences>false</DisableImplicitFrameworkReferences>
  </PropertyGroup>
  <ItemGroup>
%s
%s
  </ItemGroup>
</Project>
''' % ('\n'.join('    <Compile Include="%s" />' % s for s in srcs), '\n'.join('    <Reference Include="%s" />' % r for r in refs))
open(os.path.join(a.out, 'check.csproj'), 'w').write(proj)
env = dict(os.environ, DOTNET_CLI_TELEMETRY_OPTOUT='1', DOTNET_NOLOGO='1')
r = subprocess.run(['dotnet', 'build', '-nologo', '-v', 'q', '-c', 'Release', '-clp:NoSummary;ErrorsOnly'], cwd=a.out, env=env, capture_output=True, text=True)
lines = sorted(set(l for l in (r.stdout + r.stderr).splitlines() if 'error' in l))
print('sources=%d refs=%d exit=%d errors=%d' % (len(srcs), len(refs), r.returncode, len(lines)))
for l in lines[:60]: print(l.replace(a.src, '<src>')[:300])
