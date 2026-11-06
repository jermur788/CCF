#!/usr/bin/env python3
"""D-072 source-only contracts, real pure-map code, and extracted source methods with explicit Unity stand-ins.

No Editor, world, save, mesh, native input or rendered UI execution. Builds outside the checkout with .NET 10.
The seven geometry constants are parsed from their authority; the Unity geometry class is never compiled.
"""
import argparse
import hashlib
import os
from pathlib import Path
import re
import subprocess
import sys

HERE = Path(__file__).resolve().parent
ROOT = HERE.parents[3]
MAP = Path("Assets/ForestPrototype/UI/Map")
BASE = "7f5278bfd42997da055da63423b6b4c30c7bb8f3"
PURE = [Path("Assets/ForestPrototype/Racks/RackGeometry.cs")] + [MAP / name for name in (
    "StandMapTransform.cs", "StandMapModel.cs", "StandMapSymbols.cs", "StandMapGridPlan.cs",
    "StandMapDrawPlan.cs", "StandMapCanopyRaster.cs", "StandMapOverlay.cs", "StandMapLegend.cs")]


def git(*arguments):
    return subprocess.check_output(["git", "-C", str(ROOT), *arguments], text=True)


def method(text, signature):
    start = text.index(signature)
    opening = text.index("{", start)
    depth = 1
    position = opening + 1
    while depth:
        depth += (text[position] == "{") - (text[position] == "}")
        position += 1
    return text[start:position]


def require(condition, name):
    if not condition:
        raise AssertionError(name)
    print("PASS SOURCE " + name, flush=True)


def run_project(directory, files, startup):
    directory.mkdir(parents=True, exist_ok=True)
    items = "\n".join(f'<Compile Include="{path}" />' for path in files)
    project = ('<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType>'
               '<TargetFramework>net10.0</TargetFramework><LangVersion>9.0</LangVersion>'
               '<Nullable>disable</Nullable><ImplicitUsings>disable</ImplicitUsings>'
               '<EnableDefaultCompileItems>false</EnableDefaultCompileItems>'
               f'<StartupObject>{startup}</StartupObject></PropertyGroup><ItemGroup>{items}</ItemGroup></Project>')
    (directory / "check.csproj").write_text(project)
    environment = dict(os.environ, DOTNET_CLI_TELEMETRY_OPTOUT="1", DOTNET_NOLOGO="1")
    build = subprocess.run(["dotnet", "build", "-nologo", "-v", "q", "-c", "Release"],
                           cwd=directory, env=environment, capture_output=True, text=True)
    (directory / "build.log").write_text(build.stdout + build.stderr)
    if build.returncode:
        raise RuntimeError(build.stdout + build.stderr)
    result = subprocess.run(["dotnet", "run", "--no-build", "-c", "Release"],
                            cwd=directory, env=environment, capture_output=True, text=True)
    (directory / "run.log").write_text(result.stdout + result.stderr)
    if result.returncode:
        raise RuntimeError(result.stdout + result.stderr)
    return result.stdout


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--out", type=Path, default=Path("/media/jer/Files/CCF/Verification/D072-StandMapHeadland-20261106/headland-dotnet"))
    arguments = parser.parse_args()
    output = arguments.out.resolve()
    require(not output.is_relative_to(ROOT), "all generated projects/output outside repository")
    source = (ROOT / MAP / "StandMapSource.cs").read_text()
    original = git("show", f"{BASE}:{MAP}/StandMapSource.cs")
    for signature in ("private void ReadScenery()", "private void ReadRacks()", "private void ReadTrees()",
                      "private void ReadSmallLayers(ForestEcologyController eco)", "public StandMapModel Rebuild()"):
        require(method(source, signature) == method(original, signature), "unchanged " + signature)
    headland = method(source, "private void ClearExistingHeadland()") + "\n" + method(source, "private void ReadExistingHeadland(ForestEcologyController eco)")
    require(not re.search(r"FindObjects|FindFirstObject|GameObject.Find|ReadScenery|ReadTrees|sharedMesh|GetVertices", headland),
            "cache/projection/invalidation adds no scene discovery or mesh reads")
    require(not re.search(r"model\.Paths|SetActive|Destroy\(|PlayerPrefs|SaveData|\.Save\(|\.position\s*=", headland),
            "projection cannot mutate Paths, save, world or geometry")
    require("headlandLookupAttempts < HeadlandLookupLimit" in headland and "1 << headlandLookupAttempts" in headland,
            "attached lookup budget and exponential backoff remain in actual source")
    refresh = method(source, "private void Refresh()")
    require(refresh.count("ReadExistingHeadland(eco);") == 1 and "nextHeadlandRead = now + RackRefreshSeconds;" in headland,
            "actual Refresh calls separately throttled source seam once")
    require(re.search(r"RackRefreshSeconds = 0\.25f;", source) is not None, "actual source cadence is 0.25 seconds")
    production = set(git("diff", BASE, "--name-only", "--", "Assets", "Packages", "ProjectSettings").splitlines())
    allowed = {str(MAP / name) for name in ("StandMapSource.cs", "StandMapDrawPlan.cs", "StandMapModel.cs", "StandMapLegend.cs")}
    require(production == allowed, "only four authorised display files changed; world/save/racks/Reference/Mini consumers untouched")
    protected = ["Tools/Verification/NextActionGuidance/check_sources.py", "Tools/Verification/StandMap/OfflineLogic",
                 "Tools/Verification/StandMap/StandMapParity.cs", "Tools/Verification/StandApproach",
                 "Assets/ForestPrototype/UI/StandMapView.cs", "Assets/ForestPrototype/UI/HudMinimapView.cs",
                 "Assets/ForestPrototype/UI/ScenarioOneUiRoot.cs", "Assets/ForestPrototype/UI/LearningObjectivesView.cs",
                 "Assets/ForestPrototype/UI/WorkPlanView.cs", "Assets/ForestPrototype/UI/Resources/ScenarioOneUi.uss"]
    require(not git("diff", BASE, "--name-only", "--", *protected).strip(),
            "original verification assertions and Stage T2 UI bytes preserved")
    require(not git("ls-files", "--others", "--exclude-standard", "--", "Assets").strip(),
            "no disposable Assets script or generated metadata")
    pure_text = "\n".join((ROOT / path).read_text() for path in PURE)
    require(not re.search(r"using UnityEngine\b|UnityEngine\.", pure_text), "production map types and predicate stay Unity-free")
    presentation = (ROOT / "Assets/ForestPrototype/ScenarioOne/ScenarioOneHeadlandPresentation.cs").read_text()
    manager = (ROOT / "Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.RackPlan.cs").read_text()
    require(all(token in presentation for token in ('scenario.RackPlanningV2Active', 'GameObject.Find("Dirt Path 00")',
            'template.GetComponent<Renderer>() == null', 'new GameObject("Scenario One V2 Headland")',
            'root.AddComponent<MeshRenderer>()', 'surface.SetActive(active)'))
            and 'ecology.StandGeometryModelVersion == StandGeometryModel.Enlarged80' in manager
            and 'manager.RackPlanningV2Active' in headland,
            "gate parity with actual manager and successful physical template/surface conditions")
    require(re.search(r"bool active = scenario != null && scenario\.RackPlanningV2Active;", presentation) is not None,
            "physical Reconcile active condition still exactly manager V2")
    physical = re.sub(r"\s+", "", presentation)
    physical_contract = (
        "floatx=ScenarioOneHeadlandGeometry.HeadlandX;",
        "floatsouth=ScenarioOneHeadlandGeometry.SouthZ,north=ScenarioOneHeadlandGeometry.NorthZ;",
        "floatfeeder=ScenarioOneHeadlandGeometry.FeederZ,path=ScenarioOneHeadlandGeometry.PathX;",
        "floatradius=ScenarioOneHeadlandGeometry.Width*0.5f;",
        "AddRectangle(vertices,uv,triangles,x-radius,x+radius,south,north,y);",
        "AddRectangle(vertices,uv,triangles,x,path,feeder-radius,feeder+radius,y+0.0002f);",
        "AddDisc(vertices,uv,triangles,x,feeder,ScenarioOneHeadlandGeometry.PadRadius,y+0.0004f);",
        "AddDisc(vertices,uv,triangles,path,feeder,ScenarioOneHeadlandGeometry.PadRadius,y+0.0004f);")
    require(all(token in physical for token in physical_contract),
            "source-to-3D mapping parity: rectangles, width alias, and exactly authored feeder pads")
    scene = (ROOT / "Assets/Scenes/ForestTest.unity").read_text()
    attachment_ids = []
    for script in ("ForestStartingStand", "ForestEcologyController"):
        guid = re.search(r"guid: (\w+)", (ROOT / f"Assets/ForestPrototype/{script}.cs.meta").read_text()).group(1)
        blocks = [block for block in scene.split("--- !u!") if "guid: " + guid in block]
        require(len(blocks) == 1, "single scene source for " + script)
        attachment_ids.append(re.search(r"m_GameObject: \{fileID: (\d+)\}", blocks[0]).group(1))
    require(attachment_ids[0] == attachment_ids[1], "attached presentation lookup matches actual starting stand/ecology scene object")
    require('if (full)\n            foreach (MapHeadlandSurface surface in model.ExistingHeadlandSurfaces)' in
            (ROOT / MAP / "StandMapDrawPlan.cs").read_text(), "new collection explicitly Full-only")

    output.mkdir(parents=True, exist_ok=True)
    constants_text = (ROOT / "Assets/ForestPrototype/ScenarioOne/ScenarioOneHeadlandGeometry.cs").read_text()
    names = ("HeadlandX", "SouthZ", "NorthZ", "FeederZ", "PathX", "Width", "PadRadius")
    constants = {}
    for name in names:
        matches = re.findall(r"public const float " + name + r"\s*=\s*(-?[0-9.]+)f;", constants_text)
        require(len(matches) == 1, "unique authority constant " + name)
        constants[name] = matches[0]
    constant_file = output / "GeometryConstants.cs"
    constant_file.write_text("public static class ScenarioOneHeadlandGeometry {\n" + "\n".join(
        f"public const float {name} = {value}f;" for name, value in constants.items()) + "\n}\n")
    print("AUTHORITY_CONSTANTS " + str(constants), flush=True)
    fields_start = source.index("    private ForestEcologyController headlandEcology;")
    counter = "    public int HeadlandProjectionReads { get; private set; }"
    fields = source[fields_start:source.index(counter) + len(counter)]
    cadence = re.search(r"private const float RackRefreshSeconds = [0-9.]+f;", source).group()
    no_property_start = source.index("            ClearExistingHeadland();", source.index("if (!model.HasProperty)"))
    no_property = source[no_property_start:source.index("            model.Racks.Clear();", no_property_start)]
    seam = output / "HeadlandSourceSeam.cs"
    seam.write_text("using UnityEngine;\npublic sealed class HeadlandSourceSeam {\n"
        + "private readonly ScenarioOneUiRoot ui; private readonly StandMapModel model = new StandMapModel();\n"
        + cadence + "\n" + fields + "\n" + headland + "\n"
        + "public HeadlandSourceSeam(ScenarioOneUiRoot ui) { this.ui = ui; }\n"
        + "public System.Collections.Generic.List<float> LookupTimes => ui.Ecology.gameObject.LookupTimes;\n"
        + "public StandMapModel Read(bool property, float now) { Time.unscaledTime = now; model.HasProperty = property;\n"
        + "if (!property) {\n" + no_property + "} else ReadExistingHeadland(ui.Ecology); model.Restamp(); return model; }\n}\n")
    old_files = []
    for path in PURE:
        destination = output / "baseline-source" / path.name
        destination.parent.mkdir(parents=True, exist_ok=True)
        destination.write_text(git("show", f"{BASE}:{path}"))
        old_files.append(destination)
    fixture = HERE / "HeadlandFixture.cs"
    baseline = run_project(output / "baseline-fingerprint", old_files + [fixture], "HeadlandFingerprint")
    current = run_project(output / "current-fingerprint", [ROOT / path for path in PURE] + [fixture], "HeadlandFingerprint")
    require(baseline == current, "baseline/current Full+Mini draw lists, legend and model hash byte-identical without surface (40 plans)")
    print("BASELINE_CURRENT_FINGERPRINT_SHA256 " + hashlib.sha256(current.encode()).hexdigest(), flush=True)
    result = run_project(output / "headland-check", [ROOT / path for path in PURE] + [fixture, constant_file, seam,
        HERE / "HeadlandSourceStubs.cs", HERE / "HeadlandOfflineCheck.cs"], "HeadlandOfflineCheck")
    print(result, end="")
    print("HEADLAND_SOURCE_AND_OFFLINE_PASS; Unity compile/live linkage/render/input/build NOT RUN")
    return 0


if __name__ == "__main__":
    try:
        sys.exit(main())
    except (AssertionError, RuntimeError, subprocess.CalledProcessError) as error:
        print("HEADLAND_SOURCE_AND_OFFLINE_FAIL: " + str(error), file=sys.stderr)
        sys.exit(1)
