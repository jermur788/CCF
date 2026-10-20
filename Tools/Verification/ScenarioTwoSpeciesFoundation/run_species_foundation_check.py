#!/usr/bin/env python3
"""Build and run the offline check for the Scenario Two species-model foundation on the plain .NET SDK.

Two parts, neither needing Unity:

  1. SCOPE / PROTECTION (git). Every protected file is compared with the BASE commit by git blob id, and the set of files that
     differ from BASE (tracked changes plus untracked files) must be exactly the allowed list below. Nothing else may move:
     not the Sitka/Oak/Beech assets, the Sitka growth model, the density model, the save and reference-archive code, scenes,
     packages or project settings.
  2. LOGIC (dotnet). The pure foundation files are compiled WITHOUT UnityEngine, in a temporary project outside the repo, with
     OfflineLogic/ScenarioTwoSpeciesFoundationOfflineCheck.cs, and the check is run against this repository's files.

It proves pure rules, the text of the species assets and the source wiring. It does NOT prove Unity import/compile, that a
ScriptableObject loads, or any runtime behaviour or anchor hash. The Unity-side check is in UnityLogic/ and has NOT been run.

Usage: run_species_foundation_check.py [--base SHA] [--out DIR]
Exit code 0 and SCENARIO_TWO_SPECIES_FOUNDATION_PASS on success. Needs `git` and `dotnet` (SDK 8+).
"""
import argparse, os, shutil, subprocess, sys, tempfile

HERE = os.path.dirname(os.path.abspath(__file__))
ROOT = os.path.abspath(os.path.join(HERE, "..", "..", ".."))
FP = os.path.join(ROOT, "Assets", "ForestPrototype")
DEFAULT_BASE = "8f9ed8025243254fe7378b6d7771a5f1e7efaccc"

# Files this task may add or change (repository-relative). Anything else that differs from BASE fails the check.
ALLOWED = {
    "Assets/ForestPrototype/SpeciesConfigurationPolicy.cs", "Assets/ForestPrototype/SpeciesConfigurationPolicy.cs.meta",
    "Assets/ForestPrototype/SpeciesSiteResponse.cs",
    "Assets/ForestPrototype/Species/ScenarioTwo.meta",
    "Tools/Verification/SiteInfrastructure/run_scenario_two_site_check.py",
    "Tools/Verification/SiteInfrastructure/OfflineLogic/ScenarioTwoSiteRulesOfflineCheck.cs",
    "Assets/ForestPrototype/TreeSpeciesDefinition.cs", "Assets/ForestPrototype/ForestEcologyController.cs",
    "Assets/ForestPrototype/ForestTreeMarkingManager.cs", "Assets/ForestPrototype/ForestTreeSpawner.cs",
    "Assets/ForestPrototype/Species/ScenarioTwo/ScotsPine.asset", "Assets/ForestPrototype/Species/ScenarioTwo/ScotsPine.asset.meta",
    "Assets/ForestPrototype/Species/ScenarioTwo/DownyBirch.asset", "Assets/ForestPrototype/Species/ScenarioTwo/DownyBirch.asset.meta",
    "Assets/ForestPrototype/Species/ScenarioTwo/CommonAlder.asset", "Assets/ForestPrototype/Species/ScenarioTwo/CommonAlder.asset.meta",
    "Docs/ScenarioTwoSpeciesFoundation.md",
}
ALLOWED_PREFIXES = ("Tools/Verification/ScenarioTwoSpeciesFoundation/",)

# Must be byte-identical to BASE (git blob id). Any change here is a stop-and-ask.
PROTECTED = [
    "Assets/ForestPrototype/Species/SitkaSpruce.asset", "Assets/ForestPrototype/Species/SitkaSpruce.asset.meta",
    "Assets/ForestPrototype/Species/SessileOak.asset", "Assets/ForestPrototype/Species/SessileOak.asset.meta",
    "Assets/ForestPrototype/Species/BeechSpecies.asset", "Assets/ForestPrototype/Species/BeechSpecies.asset.meta",
    "Assets/ForestPrototype/SitkaGrowthModel.cs", "Assets/ForestPrototype/MixedDensityModel.cs",
    "Assets/ForestPrototype/MixedDensityModel.Composite.cs", "Assets/ForestPrototype/SitkaSubsetDensitySignal.cs",
    "Assets/ForestPrototype/ForestSiteProfile.cs", "Assets/ForestPrototype/ScenarioSiteProfiles.cs", "Assets/ForestPrototype/JuvenileEcologyRules.cs",
    "Assets/ForestPrototype/RegenerationModel.cs", "Assets/ForestPrototype/SimulationRandom.cs",
    "Assets/ForestPrototype/ForestSaveData.cs", "Assets/ForestPrototype/ForestSaveController.cs",
    "Assets/ForestPrototype/ForestSaveValidation.cs", "Assets/ForestPrototype/ForestSaveValidation.Geometry.cs",
    "Assets/ForestPrototype/ForestSaveValidation.Storms.cs", "Assets/ForestPrototype/ScenarioOne/ScenarioOneSaveMigration.cs",
    "Assets/ForestPrototype/ScenarioOne/ScenarioReferenceArchive.cs", "Assets/ForestPrototype/ScenarioOne/ScenarioReferenceRunner.cs",
    "Assets/ForestPrototype/ScenarioOne/ScenarioReferenceSchedule.cs", "Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.cs",
    "Packages/manifest.json", "Packages/packages-lock.json", "ProjectSettings/ProjectVersion.txt",
]

SOURCES = [os.path.join(FP, n) for n in ("SpeciesConfigurationPolicy.cs", "MixedDensityModel.cs", "MixedDensityModel.Composite.cs")] + \
          [os.path.join(HERE, "OfflineLogic", "ScenarioTwoSpeciesFoundationOfflineCheck.cs")]

PROJECT = """<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net8.0</TargetFramework><LangVersion>9.0</LangVersion><Nullable>disable</Nullable><ImplicitUsings>disable</ImplicitUsings><EnableDefaultCompileItems>false</EnableDefaultCompileItems><StartupObject>ScenarioTwoSpeciesFoundationOfflineCheck</StartupObject></PropertyGroup>
<ItemGroup>
{items}
</ItemGroup></Project>
"""

def git(*args):
    return subprocess.run(["git", "-C", ROOT] + list(args), capture_output=True, text=True)

def scope_check(base):
    problems = []
    if git("cat-file", "-e", base + "^{commit}").returncode != 0:
        return ["base commit %s not found in this repository" % base], 0
    checked = 0
    for rel in PROTECTED:
        want = git("rev-parse", base + ":" + rel)
        path = os.path.join(ROOT, rel)
        if want.returncode != 0:
            problems.append("protected file missing at BASE: " + rel); continue
        if not os.path.isfile(path):
            problems.append("protected file missing now: " + rel); continue
        have = git("hash-object", path).stdout.strip()
        checked += 1
        if have != want.stdout.strip():
            problems.append("protected file CHANGED: " + rel)
    # SpeciesSiteResponse.cs is the one dormant data file this task extends: the change must be purely additive.
    numstat = git("diff", "--numstat", base, "--", "Assets/ForestPrototype/SpeciesSiteResponse.cs").stdout.split()
    if len(numstat) >= 2 and numstat[1] != "0":
        problems.append("SpeciesSiteResponse.cs has removed or edited lines (%s); the change must be purely additive" % numstat[1])
    changed = set(git("diff", "--name-only", base).stdout.split())
    changed |= set(l[3:].strip().strip('"') for l in git("status", "--porcelain", "--untracked-files=all").stdout.splitlines())
    for rel in sorted(changed):
        if rel in ALLOWED or rel.startswith(ALLOWED_PREFIXES):
            continue
        problems.append("file outside the allowed set differs from BASE: " + rel)
    missing = sorted(r for r in ALLOWED if not os.path.isfile(os.path.join(ROOT, r)))
    for rel in missing:
        problems.append("allowed file expected but absent: " + rel)
    return problems, checked

def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--base", default=DEFAULT_BASE)
    parser.add_argument("--out", default=os.path.join(tempfile.gettempdir(), "ccf-species-foundation-offline"))
    args = parser.parse_args()
    problems, checked = scope_check(args.base)
    for p in problems:
        print("FAIL SCOPE " + p)
    print(("PASS " if not problems else "FAIL ") + "SCOPE_PROTECTED_FILES_BYTE_IDENTICAL_TO_BASE_AND_CHANGE_SET_IS_THE_ALLOWED_LIST  [%d protected files compared with %s]" % (checked, args.base[:7]))
    dotnet = shutil.which("dotnet")
    if not dotnet:
        sys.exit("dotnet SDK not found")
    for source in SOURCES:
        if not os.path.isfile(source):
            sys.exit("missing source " + source)
    os.makedirs(args.out, exist_ok=True)
    with open(os.path.join(args.out, "check.csproj"), "w") as handle:
        handle.write(PROJECT.format(items="\n".join('<Compile Include="%s" />' % s for s in SOURCES)))
    env = dict(os.environ, DOTNET_CLI_TELEMETRY_OPTOUT="1", DOTNET_NOLOGO="1")
    build = subprocess.run([dotnet, "build", "-nologo", "-v", "q", "-c", "Release"], cwd=args.out, env=env, capture_output=True, text=True)
    if build.returncode != 0:
        sys.stdout.write(build.stdout); sys.stderr.write(build.stderr)
        return build.returncode
    run = subprocess.run([dotnet, "run", "--no-build", "-c", "Release", "--", ROOT], cwd=args.out, env=env, capture_output=True, text=True)
    sys.stdout.write(run.stdout); sys.stderr.write(run.stderr)
    if problems:
        print("SCENARIO_TWO_SPECIES_FOUNDATION_FAIL scope problems: %d" % len(problems))
        return 1
    if run.returncode == 0:
        print("SCENARIO_TWO_SPECIES_FOUNDATION_PASS")
    return run.returncode

if __name__ == "__main__":
    sys.exit(main())
