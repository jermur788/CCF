#!/usr/bin/env python3
"""Offline (no Unity) source/fixture checks for the Objective 1 illustrative five-rack figure (D-072 FR-T, issue #296).

  python3 -I -B Tools/Verification/ScenarioOneFiveRackDiagram/run_five_rack_check.py [--out NEW_DIR] [--base SHA] [--no-mutations] [--scope-only]

1. Parses the authority constants (ScenarioOneHeadlandGeometry, StandGeometryModel, RackGeometry, ScenarioOneSuggestedRackLayout ids),
   the ForestTest scene's "Dirt Path" planks and "Forest Clearing", and - when the archive is present - re-reads the five entrance trees from
   the archived Enlarged80 Rack V2 snapshot (read-only, hash-checked).
2. Source guards: the figure is built from the authority constants, reads no save and no live Stand Map, the Objective 1 picture is chosen
   only through the caller's Rack V2 flag, only allowed files changed, and LearningObjectivesView.cs differs from base by exactly the one
   authorised call-site line (Manager #200 ruling, B-1).
3. Builds TeachingDiagramPlan.cs + StandApproachRoute.cs + FiveRackDiagramCheck.cs on the plain .NET SDK and runs the structural checks.
4. Proves the introduction and lessons 2-8 pictures are item-for-item unchanged against --base, via the existing check's dump.
5. Painter probe: compiles the REAL TeachingDiagram.cs + TeachingDiagramPlan.cs against recording stand-ins for the UI Toolkit types it uses
   (PainterProbe/), once at --base and once now. It checks the world selection, that every picture drawn without the Rack V2 flag is
   byte-identical to the base painter's output (elements, inline styles, every Painter2D command), and the five-rack painter semantics. This
   proves the painter's commands; it does not prove Unity's rendering, layout, fonts or fit.
6. Falsification: applies deliberate defects to temporary copies of the plan or the painter and requires a FAIL on each.

Nothing is written inside the repository. --out must be new or empty (the default is a new timestamped folder), so a re-run never deletes
earlier evidence.

Suite markers (#308, F-5). The marker depends on what actually ran, and only one of them means a full pass:
  FIVE_RACK_SUITE_PASS        exit 0  the full suite ran, including all 29 mutations, every assertion passed and every identity matched
  FIVE_RACK_SUITE_FAIL        exit 1  at least one assertion failed, or the checker stopped on an internal error
  FIVE_RACK_SUITE_INCOMPLETE  exit 2  nothing failed but this was not a full run: --no-mutations, a non-default --base, or no archived snapshot
  FIVE_RACK_SUITE_SCOPE_ONLY  exit 2  nothing failed but only the source and scope guards ran (--scope-only)

Exact inventories (#308, F-1 to F-4). The diagram files, the Objective 1 call-site file, this folder's C# checks and the historical handoffs are
pinned by commit, git blob and SHA-256. Every changed path must be named. The assertions that ran are compared by name and tally against
expected_assertions.json. Ignored production files and git index flags fail closed. A sparse checkout fails by design: it needs skip-worktree.
"""
import argparse, datetime, hashlib, json, math, os, re, subprocess, sys
from pathlib import Path

HERE = Path(__file__).resolve().parent
ROOT = HERE.parents[2]
BASE = "1b68ddcd583ab2bddd43b5ce18365171c0bab3b4"
REPAIR_START = "e7bda1f2eea4340a0d8eaae351abb43ffaff8202"   # reviewed #296 state; the original implementation handoff is frozen there
PLAN = Path("Assets/ForestPrototype/UI/TeachingDiagramPlan.cs")
PAINTER = Path("Assets/ForestPrototype/UI/TeachingDiagram.cs")
USS = Path("Assets/ForestPrototype/UI/Resources/ScenarioOneUi.uss")
ROUTE = Path("Assets/ForestPrototype/StandApproachRoute.cs")
SCENE = Path("Assets/Scenes/ForestTest.unity")
ARCHIVE = Path("/media/jer/Files/CCF")
SNAPSHOT_SHA = "e8bb96f483e598e7449f370cf56f928503dc79ec7337307758eff23104b10ac5"
HANDOFF_DIR = "Docs/Handoffs/ScenarioOne-Overnight-Playtest-Readiness-20261106/"
OBJECTIVES = "Assets/ForestPrototype/UI/LearningObjectivesView.cs"
IMPLEMENTATION_HANDOFF = HANDOFF_DIR + "OBJECTIVE-GUIDANCE-TEACHING-MAP-IMPLEMENTATION-HANDOFF.md"
REPAIR_HANDOFF = HANDOFF_DIR + "D072-FIVE-RACK-DIAGRAM-BOUNDED-REPAIR-HANDOFF.md"
# #306 is this repair's own durable report. It is the only new path this task adds outside the five-rack test folder.
STAGE_306_HANDOFF = HANDOFF_DIR + "D072-FIVE-RACK-STAGE-INVENTORY-CHECKER-REPAIR-HANDOFF.md"
# #303 already handed off exactly two StandMap verifier scripts and one historical handoff. They are authorised only as those
# exact paths, pinned to the commit that introduced them and to their byte hashes. This is deliberately not a directory prefix:
# any other StandMap path still fails the inventory.
STAGE_303_SOURCE = "6a8b261fa950efe43f9f20a249fb8c318ded4213"
STAGE_303_HANDOFF_COMMIT = "69b41605968f78e35a5361eec0562ab9f49e853f"
STAGE_303 = {
    "Tools/Verification/StandMap/HeadlandOffline/run_headland_check.py":
        (STAGE_303_SOURCE, "73e111497927e478c89d90484e63ffba24e3dfb4ee984dce96d8c36d80f2eccd"),
    "Tools/Verification/StandMap/HeadlandOffline/test_stage_inventory_guard.py":
        (STAGE_303_SOURCE, "5ae498171f48d4801678de9a12bb53fcf3e261b5100e30279999d0f046220e8b"),
    HANDOFF_DIR + "D072-HEADLAND-STAGE-INVENTORY-CHECKER-REPAIR-HANDOFF.md":
        (STAGE_303_HANDOFF_COMMIT, "b146d2fbf6eacae8ed2f34ae4598d03a3605e879e84f7a3123fb611cf70d098c"),
}
STAGE_303_SCRIPTS = {path for path in STAGE_303 if path.startswith("Tools/Verification/StandMap/")}
# The single LearningObjectivesView.cs statement the Manager #200 ruling allows to change: the Objective 1 diagram call passes the flag the
# view already computes (rackV2Objective). Any other difference in that file fails the scope guard.
CALL_SITE_OLD = ("        body.Add(new TeachingDiagram(ScenarioOneTeachingSequence.ExtensionOffered(TeachingFacts()) ? 4 : CurrentTopicIndex, "
                 "lesson.Caption));")
CALL_SITE_NEW = ("        body.Add(new TeachingDiagram(ScenarioOneTeachingSequence.ExtensionOffered(TeachingFacts()) ? 4 : CurrentTopicIndex, "
                 "lesson.Caption, rackV2Objective));")
FLAG_LINE = "        bool rackV2Objective = TeachingRacks && ui.Manager.RackPlanningV2Active;"
# #296 accepted code identity: the bounded repair commit is the last commit that touched each of these (checked in the #308 handoff).
ANCHOR_296 = "316429dc5173e81e6d903ea86531a60952f2d36a"
CHECKER_DIR = "Tools/Verification/ScenarioOneFiveRackDiagram/"
CHECK_CS = CHECKER_DIR + "FiveRackDiagramCheck.cs"
PROBE_CS = CHECKER_DIR + "PainterProbe/PainterProbe.cs"
STUBS_CS = CHECKER_DIR + "PainterProbe/RecordingUnityStubs.cs"
RUNNER = CHECKER_DIR + "run_five_rack_check.py"
INTEGRITY_TEST = CHECKER_DIR + "test_stage_inventory_guard.py"
ASSERTIONS_FILE = CHECKER_DIR + "expected_assertions.json"
# (label, path, commit, git blob, SHA-256). Each is checked at the pinned commit, at HEAD and in the working tree, so the runner proves these bytes
# on its own and does not rely on the #303 checker having been run as well (#308, F-1).
PINS = [
    ("#296 pin", str(PAINTER), ANCHOR_296, "65d4aba342332bcb41012a0c7b8867b05aa03164", "5062dd7dd1836cdb9663b257c0e4a4b74daf3b6f183923a27e6f86e35be83874"),
    ("#296 pin", str(PLAN), ANCHOR_296, "6e73f9571f7f10a16b11c608fa34de10aab7d0a7", "967b21b8bfd1bedafa3fdb23e8bcda1c90bcbf3696882a918b62a33628514771"),
    ("#296 pin", OBJECTIVES, ANCHOR_296, "6e5d84655346ee6f4355ce6229a94f5a5ba1a3c2", "fb97d3886fffef7285a908120ebea5939f3b61c6dc9ffaff8b2c60e038c5b139"),
    ("#296 verifier pin", CHECK_CS, ANCHOR_296, "04f8a9f08de95670b2cf70a632321c3a1d1825f0", "5cb387f5029f6532c8ce969e54e1960467ee70851268ea636a5012a23094b21f"),
    ("#296 verifier pin", PROBE_CS, ANCHOR_296, "14b87ecc43e57103601d036501b297271ff465f0", "8b1b90b6c7b8d5366985f568eaf12e8f0d4b83f7afe3cd7e2e1c2e7746d017de"),
    ("#296 verifier pin", STUBS_CS, ANCHOR_296, "15d203c48677d7bf70fdc2f7a3f1cd82f270e384", "9241cd65d4d7d716b434a62e1c5504cfa489583303bf2b335c9b100a7acc46c3"),
    ("handoff pin", IMPLEMENTATION_HANDOFF, REPAIR_START, "5ce93c9563d01053246a15a9ae0280d996db07db", "641a69d334c434cfacaa9ef773ba1c0f429256a00c16302f355c72b18cb6fe97"),
    ("handoff pin", REPAIR_HANDOFF, "335ee6b3fde8b9e514cc914b0e5bd6a6e10d70a3", "4321956a89b9060ec4b52f8f0ec9f38bddb44a84", "f5c60afbbb606ecce078ef6b336488836d923fc6e4716da37836a30b8346f388"),
    ("handoff pin", STAGE_306_HANDOFF, "d531f2308a103d022aca3128497b5c3a975228d5", "26e683f6b46287443edf459e118baeb6fe29352a", "09f6dd7c80715571e86bd0c6f60fc212a65e92798506104f92defc03c940aaf4"),
]
# #308's own report. It cannot pin itself, so it is allowed at this one exact path and must be text.
STAGE_308_HANDOFF = HANDOFF_DIR + "D072-FIVE-RACK-CHECKER-F1-F3-REPAIR-HANDOFF.md"
# The whole content of the five-rack folder, by exact name. No prefix and no wildcard: anything else in the folder fails (#308, F-3).
FIVE_RACK_FILES = {CHECK_CS, PROBE_CS, STUBS_CS, RUNNER, INTEGRITY_TEST, ASSERTIONS_FILE}
ALLOWED = ({str(PLAN), str(PAINTER), OBJECTIVES, IMPLEMENTATION_HANDOFF, REPAIR_HANDOFF, STAGE_306_HANDOFF, STAGE_308_HANDOFF}
           | set(STAGE_303) | FIVE_RACK_FILES)
ALLOWED_ASSETS = {str(PLAN), str(PAINTER), OBJECTIVES}
# A checker file cannot contain its own hash, so each pin line is blanked before hashing: this file's digest excludes the SELF_SHA256 value and
# the integrity test's digest excludes its CHECKER_SHA256 value. That test holds this runner's full SHA-256, which closes the loop without a cycle.
SELF_SHA256 = "aec2d4761fe7163eb9f056e6d8f2b557be017107b0bfcebe1c9e90e68b64f688"
INTEGRITY_TEST_SHA256 = "eaa9fc28dbd0b1a5bf99bba031f82669417b3d893e1139d89a3631318eae8fd7"
ASSERTIONS_SHA256 = "6515df087111ebda1c1832dcba72bf938648721cad6a7c5222c96b38055c4dbe"
EXPECTED_STRUCTURAL, EXPECTED_PAINTER, EXPECTED_MUTATIONS, EXPECTED_ORIGINAL = 109, 165, 29, 131
INTERNAL_ERROR_CHECK = "the checker ran to the end of its mode without an internal error"
PROTECTED = ["Assets/ForestPrototype/UI/Map/StandMapSource.cs", "Assets/ForestPrototype/UI/Map/StandMapModel.cs",
             "Assets/ForestPrototype/UI/Map/StandMapDrawPlan.cs", "Assets/ForestPrototype/UI/Map/StandMapLegend.cs",
             "Assets/ForestPrototype/UI/ScenarioOneUiRoot.cs", "Assets/ForestPrototype/UI/UiKit.cs",
             "Assets/ForestPrototype/UI/StandMapView.cs", "Assets/ForestPrototype/UI/WorkPlanView.cs",
             "Assets/ForestPrototype/ScenarioOne/ScenarioOneHeadlandGeometry.cs", "Assets/ForestPrototype/ScenarioOne/ScenarioOneSuggestedRackLayout.cs",
             "Assets/ForestPrototype/StandApproachRoute.cs", "Assets/Scenes/ForestTest.unity",
             HANDOFF_DIR + "OBJECTIVE-GUIDANCE-TEACHING-MAP-HANDOFF.md",
             "Tools/Verification/ScenarioOneUxPedagogy/OfflineLogic/TeachingDiagramPlanCheck.cs",
             "Tools/Verification/ScenarioOneUxPedagogy/OfflineLogic/run_diagram_check.py"]
results = []
names = []
structural_names = []
painter_names = []
caught_mutations = []


def record(ok, name, detail=""):
    results.append(ok)
    names.append(name)
    print(("PASS " if ok else "FAIL ") + "SOURCE " + name + (" :: " + str(detail) if detail != "" else ""), flush=True)


def git(*args):
    return subprocess.check_output(["git", "-C", str(ROOT), *args], text=True)


def git_bytes(*args):
    return subprocess.check_output(["git", "-C", str(ROOT), *args])


def git_quiet(*args):
    """Bytes of a git command, or None when it fails (a missing path or commit is a checked result here, not a crash)."""
    result = subprocess.run(["git", "-C", str(ROOT), *args], stdout=subprocess.PIPE, stderr=subprocess.DEVNULL)
    return result.stdout if result.returncode == 0 else None


def git_paths(*args):
    """Paths from a git command run with -z, so a space or a quoted name cannot change what is compared."""
    return [path for path in git(*args).split("\0") if path]


def sealed_digest(data, constant):
    """SHA-256 of a file with the value of its one `constant = "<hex>"` pin line blanked."""
    blanked, count = re.subn(b"^" + constant + b' = "[0-9a-f]*"', constant + b' = ""', data, flags=re.M)
    return hashlib.sha256(blanked).hexdigest() if count == 1 else "no single " + constant.decode() + " line"


def regular_file_bytes(path):
    """Bytes of a real file in the working tree, or None for a missing path or a symlink."""
    target = ROOT / path
    return target.read_bytes() if target.is_file() and not target.is_symlink() else None


def load_inventory():
    """The expected assertion identities, or None when the file is absent, altered or unreadable. A tampered inventory is never used."""
    data = regular_file_bytes(ASSERTIONS_FILE)
    if data is None or hashlib.sha256(data).hexdigest() != ASSERTIONS_SHA256:
        return None
    return json.loads(data)


def pin_check(label, path, commit, blob, sha):
    """One pinned file, three independent facts: the pinned commit holds it, HEAD holds it, and the working tree holds it."""
    pinned = git_quiet("show", commit + ":" + path)
    pinned_blob = git_quiet("rev-parse", commit + ":" + path)
    record(pinned is not None and pinned_blob is not None and pinned_blob.decode().strip() == blob and hashlib.sha256(pinned).hexdigest() == sha,
           label + ": " + path + " at " + commit[:8] + " has its recorded git blob and SHA-256")
    head_blob = git_quiet("rev-parse", "HEAD:" + path)
    record(head_blob is not None and head_blob.decode().strip() == blob, label + ": " + path + " at HEAD is the pinned git blob",
           "missing" if head_blob is None else head_blob.decode().strip())
    current = regular_file_bytes(path)
    record(current is not None and hashlib.sha256(current).hexdigest() == sha and current == pinned,
           label + ": " + path + " in the working tree has exactly the pinned bytes",
           "missing or not a regular file" if current is None else hashlib.sha256(current).hexdigest())


def stage_pins():
    """Byte identity of the accepted #296 code, its verifier and the historical handoffs, plus the checker's own files (#308, F-1 and F-3)."""
    for label, path, commit, blob, sha in PINS:
        pin_check(label, path, commit, blob, sha)
    record(sealed_digest(Path(__file__).read_bytes(), b"SELF_SHA256") == SELF_SHA256, "this runner matches its own recorded SHA-256 (pin line blanked)")
    test = regular_file_bytes(INTEGRITY_TEST)
    record(test is not None and sealed_digest(test, b"CHECKER_SHA256") == INTEGRITY_TEST_SHA256,
           "the integrity test matches its recorded SHA-256 (its checker pin line blanked)")
    data = regular_file_bytes(ASSERTIONS_FILE)
    record(data is not None and hashlib.sha256(data).hexdigest() == ASSERTIONS_SHA256, "the expected assertion inventory matches its recorded SHA-256")
    inventory = load_inventory()
    table = json.dumps([list(entry) for entry in MUTATIONS + PAINTER_MUTATIONS], ensure_ascii=False, separators=(",", ":")).encode()
    record(len(MUTATIONS) == 15 and len(PAINTER_MUTATIONS) == 14 and len(MUTATIONS) + len(PAINTER_MUTATIONS) == EXPECTED_MUTATIONS,
           "the mutation table holds exactly the 15 plan and 14 painter mutations", "%d + %d" % (len(MUTATIONS), len(PAINTER_MUTATIONS)))
    record(inventory is not None and hashlib.sha256(table).hexdigest() == inventory["mutation_table_sha256"],
           "the mutation table keeps every original name, anchor, replacement and expected text", hashlib.sha256(table).hexdigest())


def head_contains(commit):
    """True when the checked-out HEAD contains the commit (full history, not a shallow or unrelated checkout)."""
    return subprocess.run(["git", "-C", str(ROOT), "merge-base", "--is-ancestor", commit, "HEAD"],
                          stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL).returncode == 0


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
    record(re.search(r"stage = Plan\.ShowsFiveRackFigure\(lesson, rackV2World\) \? Plan\.ObjectiveOneFigure\(SiteFromAuthority\(\)\) : Plan\.For\(lesson\);", painter) is not None,
           "the painter chooses the five-rack figure only through Plan.ShowsFiveRackFigure(lesson, rackV2World); otherwise Plan.For(lesson)")
    record(len(re.findall(r"Plan\.ObjectiveOneFigure\(", painter)) == 1, "the painter builds the five-rack figure in exactly one place")
    record("public TeachingDiagram(int lesson, string caption) : this(lesson, caption, false) { }" in painter,
           "the old two-argument constructor fails closed to the original picture (flag false)")
    record(re.search(r"public static bool ShowsFiveRackFigure\(int lesson, bool rackV2World\) => lesson == 0 && rackV2World;", plan) is not None,
           "selection seam: only lesson 0 with the caller's Rack V2 flag shows the figure")
    record("RackPlanningV2Active" not in painter and "RackPlanningV2Active" not in plan,
           "the passive picture never looks up Rack V2 eligibility itself (no scene-wide lookup, no alternative eligibility logic)")
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


def commit_exists(commit):
    return subprocess.run(["git", "-C", str(ROOT), "cat-file", "-e", commit + "^{commit}"],
                          stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL).returncode == 0


def stage_inventory(changed):
    """#303's two verifier scripts and its handoff are authorised only as those exact paths, pinned by commit and bytes.

    The expected presence is decided by HEAD, not by the working tree: a checkout that contains the #303 commit must carry the
    complete pinned stage, and a checkout that predates it must not carry any part of it. This stops a partial stage, a reverted
    stage or a file with the same name but different content from being mistaken for the authorised one.
    """
    record(commit_exists(STAGE_303_SOURCE) and commit_exists(STAGE_303_HANDOFF_COMMIT),
           "stage 3: both pinned #303 commits are present in history (full clone, not shallow)")
    present = sorted(path for path in STAGE_303 if path in changed)
    if head_contains(STAGE_303_SOURCE):
        record(present == sorted(STAGE_303),
               "stage 3: HEAD contains the #303 stage, so exactly its two verifier scripts and its handoff changed since " + BASE[:8], present)
        for path, (commit, sha) in STAGE_303.items():
            try:
                pinned = git_bytes("show", commit + ":" + path)
            except subprocess.CalledProcessError:
                record(False, "stage 3: pinned #303 blob " + path + " is readable at " + commit[:8], "git show failed")
                continue
            record(hashlib.sha256(pinned).hexdigest() == sha,
                   "stage 3: pinned #303 blob " + path + " at " + commit[:8] + " matches its recorded SHA-256",
                   hashlib.sha256(pinned).hexdigest())
            target = ROOT / path
            current = target.read_bytes() if target.exists() else b""
            record(target.exists() and current == pinned and hashlib.sha256(current).hexdigest() == sha,
                   "stage 3: " + path + " is byte-identical to pinned #303 commit " + commit[:8],
                   hashlib.sha256(current).hexdigest() if target.exists() else "missing")
    else:
        record(not present, "stage 3: HEAD predates #303, so no #303 verifier path may be changed", present)
    standmap = sorted(path for path in changed if path.startswith("Tools/Verification/StandMap/"))
    record(set(standmap) <= STAGE_303_SCRIPTS,
           "stage 3: no Stand Map path outside the two pinned #303 verifier scripts changed", standmap)


def scope_guards(base):
    # --no-renames: a rename would report only the new name and hide the deleted path. The second diff keeps a committed change visible even
    # when the working tree has been put back.
    in_worktree = set(git_paths("diff", "-z", "--no-renames", "--name-only", base)) | set(git_paths("ls-files", "-z", "--others", "--exclude-standard"))
    changed = in_worktree | set(git_paths("diff", "-z", "--no-renames", "--name-only", base, "HEAD"))
    stage_inventory(in_worktree)
    packages = sorted(p for p in changed if p.startswith(("Packages/", "ProjectSettings/")))
    record(not packages, "no package or project-settings change", packages)
    outside = sorted(p for p in changed if p not in ALLOWED)
    record(not outside, "only allowed files changed since " + base[:8], outside)
    folder = set(git_paths("ls-files", "-z", "--", CHECKER_DIR)) | set(git_paths("ls-files", "-z", "--others", "--", CHECKER_DIR))
    missing = sorted(p for p in FIVE_RACK_FILES if regular_file_bytes(p) is None)
    record(folder == FIVE_RACK_FILES and not missing,
           "the five-rack folder holds exactly its six named files (tracked, untracked or ignored)", sorted(folder ^ FIVE_RACK_FILES) + missing)
    handoff = regular_file_bytes(STAGE_308_HANDOFF)
    if handoff is None:
        record(not (ROOT / STAGE_308_HANDOFF).exists() and not (ROOT / STAGE_308_HANDOFF).is_symlink(),
               "the #308 handoff, when present, is a regular UTF-8 text file at its one exact path")
    else:
        try:
            text_ok = b"\0" not in handoff and bool(handoff.decode("utf-8"))
        except UnicodeDecodeError:
            text_ok = False
        record(text_ok, "the #308 handoff, when present, is a regular UTF-8 text file at its one exact path")
    # Plain inventories cannot see ignored files or entries the index has been told to stop checking, so both are looked at directly (#308, F-4).
    ignored = git_paths("ls-files", "-z", "--others", "--ignored", "--exclude-standard", "--", "Assets", "Packages", "ProjectSettings")
    record(not ignored, "no ignored untracked path under Assets, Packages or ProjectSettings", ignored[:5])
    flagged = [line for line in git("ls-files", "-v").splitlines() if not line.startswith("H ")]
    record(not flagged, "the git index holds no assume-unchanged, skip-worktree or other non-normal entry (git ls-files -v)", flagged[:5])
    for path in PROTECTED:
        record(path not in changed, "unchanged: " + path)
    record(not any(p.endswith(".meta") or (p.startswith("Assets/") and p not in ALLOWED_ASSETS) for p in changed), "no new Assets file or .meta")
    # LearningObjectivesView.cs: exactly the one authorised call-site line, nothing else (Manager #200 ruling, B-1).
    diff = git("diff", "-U0", base, "--", OBJECTIVES)
    removed = [l[1:] for l in diff.splitlines() if l.startswith("-") and not l.startswith("---")]
    added = [l[1:] for l in diff.splitlines() if l.startswith("+") and not l.startswith("+++")]
    record(removed == [CALL_SITE_OLD] and added == [CALL_SITE_NEW],
           "LearningObjectivesView.cs differs from base only at the Objective 1 diagram call, which now passes rackV2Objective",
           "removed=%d added=%d" % (len(removed), len(added)))
    objectives = read(OBJECTIVES)
    record(objectives.count(FLAG_LINE) == 1 and CALL_SITE_NEW.strip() in objectives and objectives.index(FLAG_LINE) < objectives.index(CALL_SITE_NEW.strip()),
           "the passed flag is the view's existing rackV2Objective, computed before the call and unchanged")
    record(git("diff", "--name-only", REPAIR_START, "--", IMPLEMENTATION_HANDOFF).strip() == "",
           "the original #296 implementation handoff is unchanged since the reviewed state " + REPAIR_START[:8])
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


def dotnet_project(directory, files, startup, defines=""):
    directory.mkdir(parents=True, exist_ok=True)
    items = "".join('<Compile Include="%s" />' % f for f in files)
    (directory / "check.csproj").write_text(
        '<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net10.0</TargetFramework>'
        '<LangVersion>9.0</LangVersion><Nullable>disable</Nullable><ImplicitUsings>disable</ImplicitUsings>'
        '<EnableDefaultCompileItems>false</EnableDefaultCompileItems><TreatWarningsAsErrors>false</TreatWarningsAsErrors>'
        '<DefineConstants>%s</DefineConstants>'
        '<StartupObject>%s</StartupObject></PropertyGroup><ItemGroup>%s</ItemGroup></Project>' % (defines, startup, items))
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
    stamp = datetime.datetime.now(datetime.timezone.utc).strftime("%Y%m%dT%H%M%SZ")
    parser.add_argument("--out", type=Path, default=ARCHIVE / ("Verification/D072-T3-FiveRackDiagram-Repair-20261106/run-" + stamp))
    parser.add_argument("--base", default=BASE)
    parser.add_argument("--no-mutations", action="store_true")
    parser.add_argument("--scope-only", action="store_true",
                        help="test-only: stop after the source and scope guards (the integrity test uses this for its negative controls)")
    a = parser.parse_args()
    out = a.out.resolve()
    record(not str(out).startswith(str(ROOT)), "all generated projects and logs are outside the repository", out)
    # Never delete earlier evidence: the output folder must be new or empty.
    if out.exists() and any(out.iterdir()):
        print("FIVE_RACK_SUITE_FAIL --out exists and is not empty; choose a new folder: " + str(out), flush=True)
        return 1
    out.mkdir(parents=True, exist_ok=True)
    global RUN_MODE, CANONICAL_BASE
    RUN_MODE = "scope-only" if a.scope_only else "no-mutations" if a.no_mutations else "full"
    CANONICAL_BASE = a.base == BASE
    head = git_quiet("rev-parse", "HEAD")
    print("RUN_IDENTITY mode=%s head=%s dirty_paths=%d" % (RUN_MODE, head.decode().strip() if head else "unknown",
                                                         len(git("status", "--porcelain=v1", "-uall").splitlines())), flush=True)
    try:
        return run_suite(a, out)
    except Exception as error:
        # Fail closed with a named assertion, not only a traceback (#308, F-6).
        record(False, INTERNAL_ERROR_CHECK, repr(error))
        return finish(completed=False)


def run_suite(a, out):
    global SNAPSHOT_NOT_RUN, FULL_SUITE_RAN
    stage_pins()
    consts, size, width, ids = authority()
    print("AUTHORITY", json.dumps({**consts, "StandSizeEnlarged80": size, "DefaultWidthM": width, "EntranceTreeIds": ids}), flush=True)
    planks, yaw, clearing = scene()
    snap = snapshot()
    SNAPSHOT_NOT_RUN = snap == "NOT_RUN"
    source_guards()
    scope_guards(a.base)
    if a.scope_only:
        return finish()
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
        return finish(completed=False)
    print("BUILD WARNINGS", warnings, flush=True)
    result = run(dll, env, str(out / "inputs.txt"))
    print(result.stdout.rstrip(), flush=True)
    structural_names[:] = assertion_names(result.stdout, "")
    STRUCTURAL_TALLY_LINE[:] = [line for line in result.stdout.splitlines() if line.startswith("FIVE_RACK_")]
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

    consts_for_stub = (consts, size, width)
    painter_probe(out, a.base, consts_for_stub)

    if not a.no_mutations:
        mutations(out, consts_for_stub)
        FULL_SUITE_RAN = True
    return finish()


PROBE = HERE / "PainterProbe"


def authority_stub(directory, consts_for_stub):
    """The three authority types the painter reads, as plain constants parsed from their real sources (the real files pull in more Unity)."""
    consts, size, width = consts_for_stub
    names = ["HeadlandX", "SouthZ", "NorthZ", "FeederZ", "PathX", "Width", "PadRadius"]
    directory.mkdir(parents=True, exist_ok=True)
    path = directory / "AuthorityFromSource.cs"
    path.write_text(
        "// Generated by run_five_rack_check.py from the parsed authority sources. Not a production file.\n"
        "public static class StandGeometryModel { public const int Enlarged80 = 1; "
        "public static float StandSizeMeters(int model) => model == Enlarged80 ? %sf : 40f; }\n"
        "public static class ScenarioOneHeadlandGeometry { %s }\n"
        "public static class RackGeometry { public const float DefaultWidthM = %sf; }\n"
        % (size, " ".join("public const float %s = %sf;" % (n, consts[n]) for n in names), width))
    return path


def build_probe(directory, painter, plan, consts_for_stub, check):
    files = [painter, plan, PROBE / "RecordingUnityStubs.cs", PROBE / "PainterProbe.cs", authority_stub(directory, consts_for_stub)]
    return dotnet_project(directory / "build", files, "PainterProbe", "PAINTER_CHECK" if check else "")


def painter_probe(out, base, consts_for_stub):
    base_dir = out / "probe-base"
    base_dir.mkdir()
    base_painter, base_plan = base_dir / "TeachingDiagram.cs", base_dir / "TeachingDiagramPlan.cs"
    base_painter.write_text(git("show", base + ":" + str(PAINTER)))
    base_plan.write_text(git("show", base + ":" + str(PLAN)))
    code, _, dll, env = build_probe(base_dir, base_painter, base_plan, consts_for_stub, False)
    record(code == 0, "base painter (" + base[:8] + ") builds against the recording UI stubs")
    if code:
        print((base_dir / "build" / "build.log").read_text()[-2000:])
        return False
    dump = run(dll, env, "dumpall")
    (out / "probe-base-dump.txt").write_text(dump.stdout)
    record(dump.returncode == 0 and dump.stdout.count("SECTION ") == 81, "base painter dump: 9 pictures x 3 widths x 3 constructor forms",
           "%d sections, sha256 %s" % (dump.stdout.count("SECTION "), hashlib.sha256(dump.stdout.encode()).hexdigest()))
    global PROBE_BASE_DUMP
    PROBE_BASE_DUMP = out / "probe-base-dump.txt"
    code, warnings, dll, env = build_probe(out / "probe-current", ROOT / PAINTER, ROOT / PLAN, consts_for_stub, True)
    record(code == 0, "current painter builds against the recording UI stubs")
    if code:
        print((out / "probe-current" / "build" / "build.log").read_text()[-2000:])
        return False
    result = run(dll, env, "check", str(PROBE_BASE_DUMP))
    print(result.stdout.rstrip(), flush=True)
    painter_names[:] = assertion_names(result.stdout, "PAINTER ")
    PAINTER_TALLY_LINE[:] = [line for line in result.stdout.splitlines() if line.startswith("FIVE_RACK_PAINTER_")]
    (out / "probe-current-run.log").write_text(result.stdout + result.stderr)
    record(result.returncode == 0 and "FIVE_RACK_PAINTER_PASS" in result.stdout, "painter probe: selection, legacy byte-identity and five-rack painter semantics")
    current = run(dll, env, "dumpall").stdout
    (out / "probe-current-dump.txt").write_text(current)
    return True


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


# Painter mutations (#296 bounded repair, M-2 and B-1). Each is (name, old, new, expected FAIL text). Run through the painter probe only.
PAINTER_MUTATIONS = [
    ("P1 planned corridor edges dashed (draft and planned identical)",
     "Line(p, edge, Plan.StrokeNormal, item.X + nx, item.Y + ny, item.X1 + nx, item.Y1 + ny);",
     "DashedLine(p, edge, item.X + nx, item.Y + ny, item.X1 + nx, item.Y1 + ny);", "two continuous solid edges"),
    ("P2 planned corridor fill opaque (trees hidden: looks cleared; the pre-repair painter did this)",
     "Polygon(p, Tint(item.Role, a * Plan.RackFillAlpha), ", "Polygon(p, Colour(item.Role, a * Plan.RackFillAlpha), ", "see-through fill"),
    ("P3 title never added", "if (!string.IsNullOrEmpty(stage.Title)) Add(FigureText(stage.Title, true));", "", "title is the first element"),
    ("P4 notes never added", "foreach (string note in stage.Notes) Add(FigureText(note, false));", "", "note(s) follow the frame"),
    ("P5 figure in every world (the reviewed B-1 defect)", "stage = Plan.ShowsFiveRackFigure(lesson, rackV2World) ?", "stage = lesson == 0 ?",
     "byte-identical to the pre-#296 painter"),
    ("P6 Rack V2 flag ignored, figure never shown", "stage = Plan.ShowsFiveRackFigure(lesson, rackV2World) ?", "stage = Plan.ShowsFiveRackFigure(lesson, false) ?",
     "positive:"),
    ("P7 old constructor defaults to Rack V2", ": this(lesson, caption, false) { }", ": this(lesson, caption, true) { }", "fails closed"),
    ("P8 existing track drawn as a thin line", "Line(p, Colour(item.Role, a), item.Width, item.Points);",
     "Line(p, Colour(item.Role, a), Plan.StrokeNormal, item.Points);", "existing surface"),
    ("P9 route tags and key words dropped", "foreach (Plan.Text text in stage.Labels) AddText(text, false);", "", "route tags A, B, C"),
    ("P10 harvested drawn like planned", "case Plan.CorridorState.Harvested:", "case (Plan.CorridorState)99:", "harvested key swatch"),
    ("P11 property boundary dashed like a draft",
     "Line(p, Colour(item.Role, a), Plan.StrokeNormal * 0.6f, item.X, item.Y, item.X1, item.Y, item.X1, item.Y1, item.X, item.Y1, item.X, item.Y);",
     "DashedLine(p, Colour(item.Role, a), item.X, item.Y, item.X1, item.Y); DashedLine(p, Colour(item.Role, a), item.X1, item.Y, item.X1, item.Y1);",
     "property boundary"),
    ("P12 draft edges drawn solid", "DashedLine(p, edge, item.X + nx, item.Y + ny, item.X1 + nx, item.Y1 + ny);",
     "Line(p, edge, Plan.StrokeNormal, item.X + nx, item.Y + ny, item.X1 + nx, item.Y1 + ny);", "draft key swatch: dashed"),
    ("P13 field mark painted in the Fell colour", "Line(p, Colour(item.Role, a), Plan.StrokeMark, item.X - 3.5f, item.Y - r, item.X - 3.5f, item.Y + r);",
     "Line(p, Colour(Plan.Role.Fell, a), Plan.StrokeMark, item.X - 3.5f, item.Y - r, item.X - 3.5f, item.Y + r);", "no Fell-coloured"),
    ("P14 old-lesson painter regression (crown outline width)",
     "Ellipse(p, item.X, item.Y, item.Radius, item.Radius, Colour(Plan.Role.CanopyEdge, a), Plan.StrokeNormal);",
     "Ellipse(p, item.X, item.Y, item.Radius, item.Radius, Colour(Plan.Role.CanopyEdge, a), Plan.StrokeMark);", "byte-identical to the pre-#296 painter"),
]

# Further plan mutations added by the bounded repair (B-1, M-1, M-3 and the review's surviving R7, R8, R13).
MUTATIONS += [
    ("Q1 key loses confirmed / work pending (M-3)", '"Planned = confirmed: solid,\\nwork pending, trees stand"', '"Planned rack 1–5: solid,\\ntrees stand"',
     "confirmed"),
    ("Q2 long notes restored (M-1)", '" m here) is guidance for this plantation, not a hard rule. Approximate scale, north up.",',
     '" m here) is guidance for this plantation, not a hard rule. Approximate scale, north up.", "Confirming the plan creates pending work only. '
     'A field-marked entrance tree still stands and is not a Fell mark; only Work Plan approval commits the work.",', "M-1 fit budget"),
    ("Q3 R7 OnExistingSurface disabled", "        float reach = site.PathWidthM * 0.5f + TreeRadiusM + TreeClearanceM;\n",
     "        if (x == x) return false;\n        float reach = site.PathWidthM * 0.5f + TreeRadiusM + TreeClearanceM;\n", "independently: no tree disc"),
    ("Q4 R8 approach thinned to three points", "public static readonly float[] FixtureTrackXZ =\n    {",
     "public static readonly float[] FixtureTrackXZ = { -2.714831f, 10.75f, 2.996937f, -4.25f, -2.436166f, -43.5f }; private static readonly float[] UnusedTrack =\n    {",
     "reverse fidelity"),
    ("Q5 R13 one legacy palette byte changed", "new byte[] { 61, 100, 68 },     // Canopy", "new byte[] { 61, 100, 69 },     // Canopy",
     "byte-identical to the pre-#296 painter"),
    ("Q6 selection seam ignores the world", "=> lesson == 0 && rackV2World;", "=> lesson == 0;", "negative:"),
    ("Q7 key row wraps to a third line", '"Draft rack: dashed,\\na proposal only"', '"Draft rack: dashed edges, a proposal only, not yet confirmed"',
     "two explicit lines"),
    ("Q8 title dropped from the plan", "        stage.Title = FigureTitle;\n", "", "title is the first element"),
]


def mutations(out, consts_for_stub):
    plan = read(str(PLAN))
    painter = read(str(PAINTER))
    jobs = [("plan", m) for m in MUTATIONS] + [("painter", m) for m in PAINTER_MUTATIONS]
    for index, (target, (name, old, new, expected)) in enumerate(jobs):
        source = plan if target == "plan" else painter
        record(source.count(old) == 1, "mutation '" + name + "' applies to exactly one place")
        if source.count(old) != 1:
            continue
        directory = out / ("mutation-%02d" % index)
        directory.mkdir()
        mutated = directory / ("TeachingDiagramPlan.cs" if target == "plan" else "TeachingDiagram.cs")
        mutated.write_text(source.replace(old, new))
        mutated_plan = mutated if target == "plan" else ROOT / PLAN
        mutated_painter = mutated if target == "painter" else ROOT / PAINTER
        failed, built = [], False
        if target == "plan":
            code, _, dll, env = dotnet_project(directory / "structural", [mutated_plan, ROOT / ROUTE, HERE / "FiveRackDiagramCheck.cs"], "FiveRackDiagramCheck")
            if code == 0:
                built = True
                result = run(dll, env, str(out / "inputs.txt"))
                (directory / "structural.log").write_text(result.stdout + result.stderr)
                failed += [l for l in result.stdout.splitlines() if l.startswith("FAIL ")]
        code, _, dll, env = build_probe(directory / "probe", mutated_painter, mutated_plan, consts_for_stub, True)
        if code == 0 and PROBE_BASE_DUMP is not None:
            built = True
            result = run(dll, env, "check", str(PROBE_BASE_DUMP))
            (directory / "probe.log").write_text(result.stdout + result.stderr)
            failed += [l for l in result.stdout.splitlines() if l.startswith("FAIL ")]
        if not built:
            record(False, "mutation '" + name + "' builds", "see " + str(directory))
            continue
        hit = [l for l in failed if expected in l]
        record(bool(hit), "mutation '" + name + "' is caught", "%d FAIL lines; e.g. %s" % (len(failed), (hit or failed or ["none"])[0][:160]))
        if hit:
            caught_mutations.append(name)


SNAPSHOT_NOT_RUN = False
PROBE_BASE_DUMP = None
RUN_MODE = "full"
CANONICAL_BASE = True
FULL_SUITE_RAN = False
STRUCTURAL_TALLY_LINE = []
PAINTER_TALLY_LINE = []


def assertion_names(stdout, tag):
    """The ordered names of the PASS/FAIL lines a C# check printed, without their ' :: ' details."""
    found = []
    for line in stdout.splitlines():
        match = re.match(r"(?:PASS|FAIL) " + tag + r"(.*)", line)
        if match and not (tag == "" and match.group(1).startswith(("SOURCE ", "PAINTER "))):
            found.append(match.group(1).split(" :: ")[0])
    return found


def difference(actual, expected):
    """A short readable reason two name lists differ."""
    if actual == expected:
        return ""
    lost, extra = [n for n in expected if n not in actual], [n for n in actual if n not in expected]
    return "%d ran, %d expected; missing %s; unexpected %s" % (len(actual), len(expected), lost[:3], extra[:3])


def identity_checks():
    """Compare what ran with the pinned inventory by name, order and tally, so a weakened checker cannot claim the same pass (#308, F-2 and F-3)."""
    emitted = list(names)
    record(True, INTERNAL_ERROR_CHECK)
    inventory = load_inventory()
    if inventory is None:
        record(False, "the source assertions match the pinned inventory for this mode by name and order", "inventory absent or altered")
        return
    expected = list(inventory["source_scope"])
    if RUN_MODE != "scope-only":
        expected += inventory["source_substantive"]
    if RUN_MODE == "full":
        expected += inventory["source_mutations"]
    record(emitted == expected, "the source assertions match the pinned inventory for this mode by name and order", difference(emitted, expected))
    if RUN_MODE == "scope-only":
        return
    record(structural_names == inventory["structural"] and len(structural_names) == EXPECTED_STRUCTURAL
           and STRUCTURAL_TALLY_LINE == ["FIVE_RACK_PASS checks=%d" % EXPECTED_STRUCTURAL],
           "exactly the 109 pinned structural assertions ran and passed", difference(structural_names, inventory["structural"]) or STRUCTURAL_TALLY_LINE)
    record(painter_names == inventory["painter"] and len(painter_names) == EXPECTED_PAINTER
           and PAINTER_TALLY_LINE == ["FIVE_RACK_PAINTER_PASS checks=%d" % EXPECTED_PAINTER],
           "exactly the 165 pinned painter assertions ran and passed", difference(painter_names, inventory["painter"]) or PAINTER_TALLY_LINE)
    if RUN_MODE != "full":
        return
    record(caught_mutations == inventory["mutation_names"] and len(caught_mutations) == EXPECTED_MUTATIONS,
           "exactly the 29 original diagram mutations ran and each was caught", difference(caught_mutations, inventory["mutation_names"]))
    superseded = inventory["original_131_superseded"]
    original = inventory["original_131"]
    lost = [n for n in original if n not in emitted and not (n in superseded and all(s in emitted for s in superseded[n]))]
    record(len(original) == EXPECTED_ORIGINAL and not lost, "all 131 original source obligations are still asserted, by name or by their named stricter successors", lost[:3])


def suite_verdict(all_passed, snapshot_not_run, mode, canonical_base, ran_to_end, identities_checked, full_suite_ran):
    """The one place a suite marker is chosen. FIVE_RACK_SUITE_PASS needs positive proof that the whole suite ran."""
    if not all_passed:
        return "FIVE_RACK_SUITE_FAIL", 1, ""
    if snapshot_not_run:
        return "FIVE_RACK_SUITE_INCOMPLETE", 2, " (archived snapshot NOT RUN: the fixture was not re-derived; not a full pass)"
    if mode == "scope-only":
        return "FIVE_RACK_SUITE_SCOPE_ONLY", 2, " (source and scope guards only; not a full pass)"
    if mode != "full":
        return "FIVE_RACK_SUITE_INCOMPLETE", 2, " (--no-mutations: the 29 mutations did not run; not a full pass)"
    if not canonical_base:
        return "FIVE_RACK_SUITE_INCOMPLETE", 2, " (non-default --base: not a full pass)"
    if not (ran_to_end and identities_checked and full_suite_ran):
        return "FIVE_RACK_SUITE_INCOMPLETE", 2, " (the full suite did not run to the end; not a full pass)"
    return "FIVE_RACK_SUITE_PASS", 0, ""


def finish(completed=True):
    # A non-default --base or a missing snapshot changes which names run, so those runs skip the comparison and can never be a pass.
    identities = completed and CANONICAL_BASE and not SNAPSHOT_NOT_RUN
    if identities:
        identity_checks()
    elif completed:
        print("NOT RUN assertion identity comparison: this run cannot be a full pass", flush=True)
    marker, code, reason = suite_verdict(all(results), SNAPSHOT_NOT_RUN, RUN_MODE, CANONICAL_BASE, completed, identities, FULL_SUITE_RAN)
    print(marker + " source_checks=%d failed=%d" % (len(results), results.count(False)) + reason, flush=True)
    return code


if __name__ == "__main__":
    sys.exit(main())
