#!/usr/bin/env python3
"""Offline verification for the Compact30 E2 candidate roster/site rules.

Compiles Assets/ForestPrototype/ScenarioOne/Compact30/Compact30SiteRules.cs (no UnityEngine
dependency) together with Compact30RulesOfflineCheck.cs, then runs the checks against the checked-in
candidate fixture JSON. The generated project lives outside the repository.

This is a SOURCE-ONLY gate. It does not compile Unity code, bind the scene or run gameplay.

Usage: run_offline.py
Exit 0 = all checks passed; 1 = failures; 2 = fixture/setup problem.
"""
import os
import shutil
import subprocess
import sys
import tempfile

HERE = os.path.dirname(os.path.abspath(__file__))
ROOT = os.path.abspath(os.path.join(HERE, "..", "..", ".."))
RULES = os.path.join(ROOT, "Assets/ForestPrototype/ScenarioOne/Compact30/Compact30SiteRules.cs")
CHECK = os.path.join(HERE, "Compact30RulesOfflineCheck.cs")
FIXTURES = os.path.join(ROOT, "Assets/ForestPrototype/ScenarioOne/Compact30/Resources")

PROJECT = """<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <LangVersion>9.0</LangVersion>
    <Nullable>disable</Nullable>
    <ImplicitUsings>disable</ImplicitUsings>
    <EnableDefaultCompileItems>false</EnableDefaultCompileItems>
    <GenerateAssemblyInfo>false</GenerateAssemblyInfo>
    <AssemblyName>Compact30RulesOfflineCheck</AssemblyName>
  </PropertyGroup>
  <ItemGroup>
    <Compile Include="{rules}" />
    <Compile Include="{check}" />
  </ItemGroup>
</Project>
"""


def main():
    for path in (RULES, CHECK, os.path.join(FIXTURES, "Compact30Roster.json"),
                 os.path.join(FIXTURES, "Compact30Sites.json")):
        if not os.path.exists(path):
            print("missing input: " + path, file=sys.stderr)
            return 2
    work = tempfile.mkdtemp(prefix="ccf-compact30-offline-")
    try:
        with open(os.path.join(work, "Check.csproj"), "w", encoding="utf-8") as handle:
            handle.write(PROJECT.format(rules=RULES, check=CHECK))
        result = subprocess.run(["dotnet", "run", "--project", work, "--", FIXTURES],
                                cwd=work, text=True)
        return result.returncode
    finally:
        shutil.rmtree(work, ignore_errors=True)


if __name__ == "__main__":
    sys.exit(main())
