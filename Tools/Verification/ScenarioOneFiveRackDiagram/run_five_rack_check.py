#!/usr/bin/env python3
"""Offline (no Unity) source/fixture checks for the Objective 1 illustrative five-rack figure (D-072 FR-T, issue #296).

  python3 -I -B Tools/Verification/ScenarioOneFiveRackDiagram/run_five_rack_check.py [--out DIR] [--base SHA] [--no-mutations]

1. Parses the authority constants (ScenarioOneHeadlandGeometry, StandGeometryModel, RackGeometry, ScenarioOneSuggestedRackLayout ids),
   the ForestTest scene's "Dirt Path" planks and "Forest Clearing", and - when the archive is present - re-reads the five entrance trees from
   the archived Enlarged80 Rack V2 snapshot (read-only, hash-checked).
2. Source guards: the figure is built from the authority constants, reads no save and no live Stand Map, and only allowed files changed.
3. Builds TeachingDiagramPlan.cs + StandApproachRoute.cs + FiveRackDiagramCheck.cs on the plain .NET SDK and runs the structural checks.
4. Proves the introduction and lessons 2-8 pictures are item-for-item unchanged against --base, via the existing check's dump.
5. Falsification: applies deliberate defects to a temporary copy of the plan and requires the checker to FAIL on each.

Nothing is written inside the repository. Prints FIVE_RACK_SUITE_PASS / FIVE_RACK_SUITE_FAIL and exits 0 / 1.
"""
import argparse, hashlib, json, math, os, re, shutil, subprocess, sys
from pathlib import Path

HERE = Path(__file__).resolve().parent
ROOT = HERE.parents[2]
BASE = "1b68ddcd583ab2bddd43b5ce18365171c0bab3b4"
PLAN = Path("Assets/ForestPrototype/UI/TeachingDiagramPlan.cs")
PAINTER = Path("Assets/ForestPrototype/UI/TeachingDiagram.cs")
USS = Path("Assets/ForestPrototype/UI/Resources/ScenarioOneUi.uss")
ROUTE = Path("Assets/ForestPrototype/StandApproachRoute.cs")
SCENE = Path("Assets/Scenes/ForestTest.unity")
ARCHIVE = Path("/media/jer/Files/CCF")
SNAPSHOT_SHA = "e8bb96f483e598e7449f370cf56f928503dc79ec7337307758eff23104b10ac5"
HANDOFF_DIR = "Docs/Handoffs/ScenarioOne-Overnight-Playtest-Readiness-20261106/"
ALLOWED = {str(PLAN), str(PAINTER), HANDOFF_DIR + "OBJECTIVE-GUIDANCE-TEACHING-MAP-IMPLEMENTATION-HANDOFF.md"}
ALLOWED_PREFIX = "Tools/Verification/ScenarioOneFiveRackDiagram/"
PROTECTED = ["Assets/ForestPrototype/UI/Map/StandMapSource.cs", "Assets/ForestPrototype/UI/Map/StandMapModel.cs",
             "Assets/ForestPrototype/UI/Map/StandMapDrawPlan.cs", "Assets/ForestPrototype/UI/Map/StandMapLegend.cs",
             "Assets/ForestPrototype/UI/LearningObjectivesView.cs", "Assets/ForestPrototype/UI/ScenarioOneUiRoot.cs",
             "Assets/ForestPrototype/UI/StandMapView.cs", "Assets/ForestPrototype/UI/WorkPlanView.cs",
             "Assets/ForestPrototype/ScenarioOne/ScenarioOneHeadlandGeometry.cs", "Assets/ForestPrototype/ScenarioOne/ScenarioOneSuggestedRackLayout.cs",
             "Assets/ForestPrototype/StandApproachRoute.cs", "Assets/Scenes/ForestTest.unity",
             HANDOFF_DIR + "OBJECTIVE-GUIDANCE-TEACHING-MAP-HANDOFF.md",
             "Tools/Verification/ScenarioOneUxPedagogy/OfflineLogic/TeachingDiagramPlanCheck.cs",
             "Tools/Verification/ScenarioOneUxPedagogy/OfflineLogic/run_diagram_check.py"]
results = []


def record(ok, name, detail=""):
    results.append(ok)
    print(("PASS " if ok else "FAIL ") + "SOURCE " + name + (" :: " + str(detail) if detail != "" else ""), flush=True)


def git(*args):
    return subprocess.check_output(["git", "-C", str(ROOT), *args], text=True)


def read(path):
    return (ROOT / path).read_text()


def method(text, signature):
    start = text.index(signature)
    depth, pos = 0, text.index("{", start)
    while True:
        depth += (text[pos] == "{") - (text[pos] == "}")
        pos += 1
        if depth == 0:
            return text[start:pos]


def authority():
    geo = read("Assets/ForestPrototype/ScenarioOne/ScenarioOneHeadlandGeometry.cs")
    consts = dict(re.findall(r"public const float (\w+) = (-?[\d.]+)f;", geo))
    size = re.search(r"StandSizeMeters\(int model\) => model == Enlarged80 \? ([\d.]+)f", read("Assets/ForestPrototype/StandGeometryModel.cs")).group(1)
    width = re.search(r"public const float DefaultWidthM = ([\d.]+)f;", read("Assets/ForestPrototype/Racks/RackGeometry.cs")).group(1)
    ids = re.search(r"EntranceTreeIds = \{([^}]*)\}", read("Assets/ForestPrototype/ScenarioOne/ScenarioOneSuggestedRackLayout.cs")).group(1)
    ids = ",".join(re.findall(r'"(PO\d{4})"', ids))
    return consts, size, width, ids


def scene():
    text = read(str(SCENE))
    docs = {}
    for chunk in text.split("\n--- !u!"):
        m = re.match(r"(\d+) &(\d+)", chunk)
        if m:
            docs[m.group(2)] = (m.group(1), chunk)

    def transform_of(name):
        for cls, chunk in docs.values():
            if cls == "1" and re.search(r"m_Name: " + re.escape(name) + r"\n", chunk):
                for comp in re.findall(r"component: \{fileID: (\d+)\}", chunk):
                    if docs.get(comp, ("", ""))[0] == "4":
                        t = docs[comp][1]
                        assert re.search(r"m_Father: \{fileID: 0\}", t), name + " must be a root transform"
                        pos = [float(v) for v in re.search(r"m_LocalPosition: \{x: ([-\d.e]+), y: ([-\d.e]+), z: ([-\d.e]+)\}", t).groups()]
                        rot = [float(v) for v in re.search(r"m_LocalRotation: \{x: ([-\d.e]+), y: ([-\d.e]+), z: ([-\d.e]+), w: ([-\d.e]+)\}", t).groups()]
                        scale = [float(v) for v in re.search(r"m_LocalScale: \{x: ([-\d.e]+), y: ([-\d.e]+), z: ([-\d.e]+)\}", t).groups()]
                        return pos, rot, scale
        raise KeyError(name)

    planks = [transform_of("Dirt Path %02d" % i) for i in range(52)]
    names = re.findall(r"m_Name: (Dirt Path[^\n]*)", text)
    record(len(names) == 52, "ForestTest has exactly the 52 authored planks Dirt Path 00..51", len(names))
    rot = planks[0][1]
    record(abs(rot[0]) < 1e-6 and abs(rot[2]) < 1e-6, "plank 00 is rotated about Y only")
    yaw = 2.0 * math.atan2(rot[1], rot[3])
    clearing = transform_of("Forest Clearing")
    xz = ",".join("%r,%r" % (p[0][0], p[0][2]) for p in planks)
    return xz, yaw, clearing


def snapshot():
    path = ARCHIVE / re.search(r'FixtureSnapshotPath = "([^"]+)"', read(str(PLAN))).group(1)
    if not path.exists():
        print("NOT RUN snapshot re-read: archive absent at " + str(path), flush=True)
        return "NOT_RUN"
    data = path.read_bytes()
    record(hashlib.sha256(data).hexdigest() == SNAPSHOT_SHA, "archived snapshot SHA-256 matches", hashlib.sha256(data).hexdigest())
    world = json.loads(data)
    record(world["standGeometryModel"] == 1 and world["scenarioOne"]["rackPlanningModel"] == 1 and world["simulationSeed"] == 20260914
           and len(world["trees"]) == 1344, "snapshot is Enlarged80 + Rack V2, seed 20260914, 1344 tree records")
    zs = []
    for tree_id in ["PO0203", "PO1204", "PO2003", "PO2803", "PO3803"]:
        found = [t for t in world["trees"] if t["treeId"] == tree_id]
        record(len(found) == 1, "snapshot holds entrance tree " + tree_id + " exactly once")
        zs.append(repr(found[0]["position"]["z"]))
    return ",".join(zs)


def source_guards():
    painter, plan = read(str(PAINTER)), read(str(PLAN))
    site = method(painter, "private static Plan.Site SiteFromAuthority()")
    wanted = {"StandSizeM": r"StandGeometryModel\.StandSizeMeters\(StandGeometryModel\.Enlarged80\)", "HeadlandX": r"ScenarioOneHeadlandGeometry\.HeadlandX",
              "HeadlandSouthZ": r"ScenarioOneHeadlandGeometry\.SouthZ", "HeadlandNorthZ": r"ScenarioOneHeadlandGeometry\.NorthZ",
              "FeederZ": r"ScenarioOneHeadlandGeometry\.FeederZ", "FeederEndX": r"ScenarioOneHeadlandGeometry\.PathX",
              "PathWidthM": r"ScenarioOneHeadlandGeometry\.Width", "PadRadiusM": r"ScenarioOneHeadlandGeometry\.PadRadius",
              "RackWidthM": r"RackGeometry\.DefaultWidthM"}
    for field, expr in wanted.items():
        record(re.search(field + r"\s*=\s*" + expr + r",", site) is not None, "painter Site." + field + " comes from its C# authority")
    record(re.search(r"stage = lesson == 0 \? Plan\.ObjectiveOneFigure\(SiteFromAuthority\(\)\) : Plan\.For\(lesson\);", painter) is not None,
           "Objective 1 (lesson 0) draws the five-rack figure; every other picture is still Plan.For(lesson)")
    record('name = lesson == 0 ? "rack-lesson-diagram"' in painter, "the Objective 1 picture keeps the element name rack onboarding checks look for")
    figure = plan[plan.index("// ======== Objective 1: illustrative five-rack plan"):]
    record(not re.search(r"(?<![\d.])(35\.4|37\.7|27\.4|2\.6|2\.2|2\.4)f?(?![\d])", figure.replace("2.436166f", "")),
           "plan holds no second copy of headland/feeder/pad/rack-width constants (only the approach fixture's PathX samples)")
    forbidden = ["forest-save", "SaveSystem", "SaveData", "PlayerPrefs", "System.IO", "File.", "persistentDataPath", "JsonUtility", "Resources.Load",
                 "GameObject.Find", "FindObject", "StandMapModel", "StandMapSource", "StandMapDrawPlan", "StandMapLegend", "ScenarioOneManager",
                 "RackPlan", "WorkPlan", "MarkTree", "Fell("]
    # The plan names its fixture's archive path as provenance (a string constant, never opened); that one declaration is exempt.
    provenance = re.compile(r"^\s*public const string FixtureSnapshotPath = \"[^\"]+\";\s*$", re.M)
    record(len(provenance.findall(plan)) == 1, "the fixture archive path appears only as one provenance constant")
    for name, text in (("TeachingDiagram.cs", painter), ("TeachingDiagramPlan.cs", provenance.sub("", plan))):
        hits = [f for f in forbidden if f in text]
        record(not hits, name + " reads no save, scene object, live Stand Map, rack plan or Work Plan", hits)
    record(not re.search(r"^\s*using\s+Unity", plan, re.M), "TeachingDiagramPlan stays Unity-free (offline-checkable)")


def scope_guards(base):
    changed = set(git("diff", "--name-only", base).split()) | set(git("ls-files", "--others", "--exclude-standard").split())
    outside = sorted(p for p in changed if p not in ALLOWED and not p.startswith(ALLOWED_PREFIX))
    record(not outside, "only allowed files changed since " + base[:8], outside)
    for path in PROTECTED:
        record(path not in changed, "unchanged: " + path)
    record(not any(p.endswith(".meta") or (p.startswith("Assets/") and p not in ALLOWED) for p in changed), "no new Assets file or .meta")
    record(str(USS) not in changed, "shared stylesheet (T2-protected bytes) unchanged: the figure's title/notes reuse existing diagram classes")
    painter = read(str(PAINTER))
    record(re.search(r'UiKit\.Text\(value, title \? "diagram-label" : "diagram-caption"\)', painter) is not None
           and "label.style.fontSize = FigureTextSize;" in painter and "private const float FigureTextSize = 20f;" in painter,
           "figure title/notes use existing classes with a 20 UI-unit reading size")
    old = git("show", base + ":" + str(PAINTER))
    new = read(str(PAINTER))
    for sig in ("private void Draw(MeshGenerationContext context)", "private void DrawYoung(Painter2D p, Plan.Item item)",
                "private void DrawMarks(Painter2D p, Plan.Item item)", "private void DrawCycle(Painter2D p, Plan.Item item)",
                "private void Ellipse(", "private void DashedEllipse(", "private void Polygon(", "private void Line("):
        record(method(old, sig) == method(new, sig), "painter method unchanged: " + sig.split("(")[0].split()[-1])


def dotnet_project(directory, files, startup):
    directory.mkdir(parents=True, exist_ok=True)
    items = "".join('<Compile Include="%s" />' % f for f in files)
    (directory / "check.csproj").write_text(
        '<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework>'
        '<LangVersion>9.0</LangVersion><Nullable>disable</Nullable><ImplicitUsings>disable</ImplicitUsings>'
        '<EnableDefaultCompileItems>false</EnableDefaultCompileItems><TreatWarningsAsErrors>false</TreatWarningsAsErrors>'
        '<StartupObject>%s</StartupObject></PropertyGroup><ItemGroup>%s</ItemGroup></Project>' % (startup, items))
    env = dict(os.environ, DOTNET_CLI_TELEMETRY_OPTOUT="1", DOTNET_NOLOGO="1")
    build = subprocess.run(["dotnet", "build", "-nologo", "-v", "q", "-c", "Release", "-o", str(directory / "bin")], cwd=directory, env=env,
                           capture_output=True, text=True)
    (directory / "build.log").write_text(build.stdout + build.stderr)
    warnings = sorted(set(re.findall(r"warning (CS\d+): ([^\[]*)", build.stdout)))
    return build.returncode, warnings, directory / "bin" / "check.dll", env


def run(dll, env, *args):
    return subprocess.run(["dotnet", str(dll), *args], capture_output=True, text=True, env=env)


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--out", type=Path, default=ARCHIVE / "Verification/D072-T3-FiveRackDiagram-20261106/offline")
    parser.add_argument("--base", default=BASE)
    parser.add_argument("--no-mutations", action="store_true")
    a = parser.parse_args()
    out = a.out.resolve()
    record(not str(out).startswith(str(ROOT)), "all generated projects and logs are outside the repository", out)
    if out.exists():
        shutil.rmtree(out)
    out.mkdir(parents=True)

    consts, size, width, ids = authority()
    print("AUTHORITY", json.dumps({**consts, "StandSizeEnlarged80": size, "DefaultWidthM": width, "EntranceTreeIds": ids}), flush=True)
    planks, yaw, clearing = scene()
    snap = snapshot()
    source_guards()
    scope_guards(a.base)
    inputs = dict(consts)
    inputs.update({"StandSizeEnlarged80": size, "DefaultWidthM": width, "SourceEntranceIds": ids, "ExpectedSnapshotSha256": SNAPSHOT_SHA,
                   "SnapshotZ": snap, "ScenePlanksXZ": planks, "Plank00YawRadians": repr(yaw),
                   "ClearingX": repr(clearing[0][0]), "ClearingZ": repr(clearing[0][2]), "ClearingScaleX": repr(clearing[2][0])})
    (out / "inputs.txt").write_text("".join("%s=%s\n" % kv for kv in inputs.items()))

    def build_check(plan_path, name):
        return dotnet_project(out / name, [plan_path, ROOT / ROUTE, HERE / "FiveRackDiagramCheck.cs"], "FiveRackDiagramCheck")

    code, warnings, dll, env = build_check(ROOT / PLAN, "five-rack")
    record(code == 0, "five-rack checker builds on .NET 10 with the real TeachingDiagramPlan.cs and StandApproachRoute.cs")
    if code:
        print((out / "five-rack" / "build.log").read_text()[-3000:])
        return finish()
    print("BUILD WARNINGS", warnings, flush=True)
    result = run(dll, env, str(out / "inputs.txt"))
    print(result.stdout.rstrip(), flush=True)
    (out / "five-rack-run.log").write_text(result.stdout + result.stderr)
    record(result.returncode == 0 and "FIVE_RACK_PASS" in result.stdout, "five-rack structural checks")
    dumps = [run(dll, env, str(out / "inputs.txt"), "dump").stdout for _ in range(2)]
    (out / "five-rack-dump.txt").write_text(dumps[0])
    digest = hashlib.sha256(dumps[0].encode()).hexdigest()
    record(dumps[0] == dumps[1] and len(dumps[0]) > 1000, "deterministic figure dump across separate processes", "sha256 " + digest)

    # The introduction and lessons 2-8 are item-for-item unchanged: the existing check's own dump, base plan versus current plan.
    existing = ROOT / "Tools/Verification/ScenarioOneUxPedagogy/OfflineLogic/TeachingDiagramPlanCheck.cs"
    (out / "base-plan").mkdir()
    base_plan = out / "base-plan" / "TeachingDiagramPlan.cs"
    base_plan.write_text(git("show", a.base + ":" + str(PLAN)))
    lesson_dumps = []
    for plan_path, name in ((base_plan, "lessons-base"), (ROOT / PLAN, "lessons-current")):
        code, _, ldll, lenv = dotnet_project(out / name, [plan_path, existing], "TeachingDiagramPlanCheck")
        record(code == 0, "existing lesson check builds with " + name.split("-")[1] + " plan")
        lesson_dumps.append(run(ldll, lenv, "dump").stdout)
        structural = run(ldll, lenv)
        print(name + ": " + structural.stdout.strip().splitlines()[-1], flush=True)
        record(structural.returncode == 0, "existing DIAGRAM_PLAN structural check passes with " + name.split("-")[1] + " plan")
    record(lesson_dumps[0] == lesson_dumps[1] and len(lesson_dumps[0]) > 1000, "introduction and all eight lesson stages dump byte-identically to base",
           "%d lines" % len(lesson_dumps[0].splitlines()))

    if not a.no_mutations:
        mutations(out)
    return finish()


MUTATIONS = [
    ("regularised spacing", 'public static readonly float[] FixtureEntranceZ = {',
     'public static readonly float[] FixtureEntranceZ = { -35.271175f, -17.645f, -0.019f, 17.607f, 35.23235f }; private static readonly float[] Unused = {',
     "centreline gaps are the real nonuniform source gaps"),
    ("sixth corridor", "                Y1 = FigureY(site, rackZ[rack]), Width = site.RackWidthM * FigurePxPerM, Z = 30 });\n",
     "                Y1 = FigureY(site, rackZ[rack]), Width = site.RackWidthM * FigurePxPerM, Z = 30 });\n        items.Add(new Item { Id = \"rack6\", Kind = Kind.Corridor, Role = Role.Rack, State = CorridorState.Planned, X = FigureX(site, site.HeadlandX), Y = FigureY(site, 25f), X1 = FigureX(site, half), Y1 = FigureY(site, 25f), Width = site.RackWidthM * FigurePxPerM, Z = 30 });\n",
     "exactly five rack corridors on the map"),
    ("cleared corridors", 'AddTreeRow(site, items, "rack" + (rack + 1) + "-tree", rackZ[rack], false);', '{ }',
     "still has standing trees along it"),
    ("feeder on the south edge", "float[] feeder = { FigureX(site, site.HeadlandX), FigureY(site, site.FeederZ), FigureX(site, site.FeederEndX), FigureY(site, site.FeederZ) };",
     "float[] feeder = { FigureX(site, site.HeadlandX), FigureY(site, -half), FigureX(site, site.FeederEndX), FigureY(site, -half) };",
     "single southern feeder is"),
    ("draft told apart by colour only", 'State = row == 1 ? CorridorState.Draft : CorridorState.Planned', 'State = CorridorState.Planned',
     "every key state has its own shape/pattern"),
    ("outer south band missing", "for (int band = 0; band <= rackZ.Length; band++)", "for (int band = 1; band <= rackZ.Length; band++)",
     "retained band 1 (outer south"),
    ("harvested rack on the map", "State = CorridorState.Planned, X = FigureX(site, site.HeadlandX)", "State = CorridorState.Harvested, X = FigureX(site, site.HeadlandX)",
     "every map corridor is planned"),
]


def mutations(out):
    plan = read(str(PLAN))
    for index, (name, old, new, expected) in enumerate(MUTATIONS):
        record(plan.count(old) == 1, "mutation '" + name + "' applies to exactly one place")
        if plan.count(old) != 1:
            continue
        directory = out / ("mutation-%d" % index)
        directory.mkdir()
        mutated = directory / "TeachingDiagramPlan.cs"
        mutated.write_text(plan.replace(old, new))
        code, _, dll, env = dotnet_project(directory / "build", [mutated, ROOT / ROUTE, HERE / "FiveRackDiagramCheck.cs"], "FiveRackDiagramCheck")
        if code:
            record(False, "mutation '" + name + "' builds", (directory / "build" / "build.log").read_text()[-600:])
            continue
        result = run(dll, env, str(out / "inputs.txt"))
        (directory / "run.log").write_text(result.stdout + result.stderr)
        failed = [l for l in result.stdout.splitlines() if l.startswith("FAIL ")]
        record(result.returncode != 0 and any(expected in l for l in failed), "mutation '" + name + "' is caught", failed[:2])


def finish():
    ok = all(results)
    print(("FIVE_RACK_SUITE_PASS" if ok else "FIVE_RACK_SUITE_FAIL") + " source_checks=%d failed=%d" % (len(results), results.count(False)), flush=True)
    return 0 if ok else 1


if __name__ == "__main__":
    sys.exit(main())
