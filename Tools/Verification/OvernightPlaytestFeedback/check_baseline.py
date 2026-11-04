#!/usr/bin/env python3
"""Read-only lexical checks for #260. No Unity, imports, saves or dependencies."""
import re
import subprocess
from collections import Counter
from pathlib import Path

ROOT = Path(__file__).resolve().parents[3]
BASE = "72b4efbd1c653fa2231f632a79030a83ff76f833"
UI = "Assets/ForestPrototype/UI/"
CURRICULUM = UI + "LearningObjectivesView.Curriculum.cs"
HELP = UI + "MenuHelpView.Text.cs"
VIEW = UI + "LearningObjectivesView.cs"
EDITOR = UI + "Map/StandMapRackEditor.cs"
SOURCE = UI + "Map/StandMapSource.cs"
DRAW = UI + "Map/StandMapDrawPlan.cs"
MINI = UI + "HudMinimapView.cs"
# Comments are consumed before strings, so comments do not become player copy.
# This is a lexical inventory, not a C# compiler or a reachability analysis.
TOKENS = re.compile(r'//[^\n]*|/\*[\s\S]*?\*/|@"(?:[^"]|"")*"|"(?:[^"\\]|\\.)*"|\'(?:[^\'\\]|\\.)*\'')
results = []


def read(path):
    return (ROOT / path).read_text(encoding="utf-8")


def literals(path):
    text = read(path)
    return [(path, text.count("\n", 0, m.start()) + 1, m.group())
            for m in TOKENS.finditer(text) if m.group().startswith(('"', '@"'))]


def evidence(path, needle):
    hits = [(path, n, line.strip()) for n, line in enumerate(read(path).splitlines(), 1)
            if needle in line and not line.lstrip().startswith("//")]
    if not hits:
        raise ValueError(f"source anchor missing: {path}: {needle}")
    return hits


def report(status, name, note, rows):
    results.append(status)
    print(f"SOURCE-STATIC {status} {name}: {note}")
    for path, line, value in rows:
        print(f"  SOURCE-STATIC {path}:{line}: {value}")


def check():
    print(f"SOURCE-STATIC baseline-source-commit: {BASE}")
    diff = subprocess.run(["git", "-C", str(ROOT), "diff", "--quiet", BASE, "--",
                           "Assets/ForestPrototype"], check=False)
    if diff.returncode not in (0, 1):
        raise ValueError("cannot compare tracked production source against baseline")
    print("SOURCE-STATIC tracked-production-vs-baseline: " + ("MATCH" if diff.returncode == 0 else "DIFF"))
    print("SOURCE-STATIC scope: current checkout; no runtime/render/input proof; no automatic baseline update")
    inventory = [row for p in sorted((ROOT / "Assets/ForestPrototype").rglob("*.cs"))
                 for row in literals(p.relative_to(ROOT).as_posix())]

    text = read(CURRICULUM)
    start = text.index("public const string RackV2Concept =")
    end = text.index(";", start)
    concept = [r for r in literals(CURRICULUM)
               if text.count("\n", 0, start) + 1 <= r[1] <= text.count("\n", 0, end) + 1]
    spaced = any(re.search(r"16[–-]20 m", r[2]) for r in concept)
    compact = any(re.search(r"16[–-]20m", r[2]) for r in concept)
    report("PASS" if compact and not spaced else "FAIL", "01 objective-spacing-copy",
           "BASELINE FAILURE expected to flip with #255; spacing in metres, not a duration. Require compact 16–20m/16-20m.", concept)

    headings = [r for r in inventory if r[2] == '"Introducing tree marking"']
    report("PASS" if headings else "FAIL", "02 tree-marking-heading",
           "Require exact heading literal Introducing tree marking; literal presence alone cannot prove display.",
           headings or evidence(HELP, 'case Menu.TreeMarks: return'))
    mentions = [r for r in inventory if "ccf source material" in r[2].lower()]
    report("FAIL" if mentions else "PASS", "03 source-material-copy",
           "No CCF source material phrase in ForestPrototype C# string literals (comments excluded).",
           mentions or evidence(HELP, "TreeMarksOrangeText ="))

    help_text = read(HELP)
    entries = help_text.split("TreeMarkEntries =", 1)[1].split("};", 1)[0]
    orange_rows = evidence(HELP, 'new[] { "Orange')
    orange_fell = re.search(r'new\[\]\s*\{\s*"Orange[^"\n]*"\s*,\s*"[^"\n]*\bFell\b', entries, re.I)
    distinct = ('new[] { "Orange vertical stripes", "Rack / machine-path removal" }' in entries
                and 'new[] { "Red", "Fell" }' in entries)
    report("PASS" if distinct and not orange_fell else "FAIL", "04 orange-entry-is-not-fell",
           "Bounded legend check: orange rack-removal entry is separate from Red/Fell; no claim about rendered paint.",
           orange_rows + evidence(HELP, 'new[] { "Red"') + evidence(HELP, 'spray will not mark a Fell'))
    pro_silva = [r for r in inventory if re.search(r"pro\s+silva", r[2], re.I)]
    report("FAIL" if pro_silva else "PASS", "05 no-formal-pro-silva-copy",
           "Conservative lexical guard: no Pro Silva mention in ForestPrototype C# literals; any mention requires human review, not automatic classification as a standard.",
           pro_silva or evidence(HELP, "TreeMarksSimplificationText ="))

    rotation = (evidence(EDITOR, "&& OnHandle(px, selected)")
                + evidence(EDITOR, "next = Gesture.Reshape;")
                + evidence(EDITOR, "private static bool ShiftHeld")
                + evidence(EDITOR, "=> (modifiers & EventModifiers.Shift)")
                + evidence(EDITOR, "Propose(px,")
                + evidence(EDITOR, "case Gesture.Reshape:")
                + evidence(EDITOR, "SnapFar(bounds, entrance, pointer,")
                + evidence(UI + "StandMapView.cs", "Hold Shift to turn snapping off."))
    report("NOT-TESTABLE", "06 round-handle-input",
           "BASELINE OBSERVATION only: snap argument is !ShiftHeld(e.modifiers), then forwarded to SnapFar for Reshape. SOURCE-STATIC cannot test physical input.", rotation)

    instruction = evidence(CURRICULUM, "if (!confirmed) return counter")
    has_path = all(t in instruction[0][2] for t in ("Plan Racks", "Stand Map [M]", "Confirm Rack Plan"))
    report("PASS" if has_path else "FAIL", "07 objective-next-instruction",
           "Unconfirmed V2 instruction names Stand Map / Plan Racks path; later states are quoted separately.",
           instruction + evidence(CURRICULUM, "Return to the forest and mark")
           + evidence(CURRICULUM, "return counter + (workPending")
           + evidence(CURRICULUM, ': ". Open the Work Plan')
           + evidence(CURRICULUM, 'case "rack.create":'))
    buttons = evidence(VIEW, "buttons.Add(UiKit.Button(")
    primary = [r for r in buttons if '"btn-primary"' in r[2]]
    report("PASS" if primary and all("Plan Racks" in r[2] and "CloseAll" not in r[2] for r in primary) else "FAIL",
           "08 objective-primary-cta", "Primary source button is generic Back to forest; visual prominence, focus and destination behaviour remain unmeasured.", buttons)

    report("NOT-TESTABLE", "09 map-minimap-trail-consistency",
           "NOT STATICALLY TESTABLE: no demonstrated coordinate mismatch for the same confirmed rack/waypoint. Shared source/draw path is evidence, not rendered parity; unconfirmed racks are deliberately FullOnly.",
           evidence(MINI, "StandMapModel model =") + evidence(MINI, "if (show) surface.Sync(model);")
           + evidence(UI + "StandMapView.cs", "StandMapModel model = force")
           + evidence(SOURCE, "foreach (RackPlanRack rack in")
           + evidence(SOURCE, "Start = rack.Entrance") + evidence(SOURCE, "End = rack.Far")
           + evidence(SOURCE, "FullOnly = !confirmed") + evidence(SOURCE, "model.HasWaypoint = ui.Map")
           + evidence(SOURCE, "model.Waypoint =") + evidence(DRAW, "rack.FullOnly && !full")
           + evidence(DRAW, "foreach (RackPoint p in path.Points)")
           + evidence(DRAW, "MapPosition c = t.WorldToPixels(model.Waypoint)")
           + evidence(UI + "Map/StandMapTransform.cs", "return new MapPosition((world.X - MinX)"))
    counts = Counter(results)
    print(f"SOURCE-STATIC TALLY PASS={counts['PASS']} FAIL={counts['FAIL']} NOT-TESTABLE={counts['NOT-TESTABLE']}")
    print("SOURCE-STATIC exit=1 means feedback assertions remain unmet; observations do not count as PASS.")
    return 1 if counts["FAIL"] else 0


if __name__ == "__main__":
    try:
        raise SystemExit(check())
    except (OSError, ValueError, IndexError) as error:
        print(f"SOURCE-STATIC ERROR: {error}")
        raise SystemExit(2)
