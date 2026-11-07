#!/usr/bin/env python3
"""Source checks and isolated SnapFar calls for #260. No Unity, assets or saves; needs .NET 8 SDK."""
import argparse
import ast
import os
import re
import shutil
import tempfile
from xml.sax.saxutils import escape
import subprocess
from collections import Counter
from pathlib import Path
from objective_cta_source_check import ObjectiveCtaSourceError, collect_companions, verify_objective_cta

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


def rotation_policy(state):
    # Restrict the inspection to the real pointer-move method, excluding comments/strings.
    source = read(EDITOR)
    code = TOKENS.sub(lambda m: "".join("\n" if c == "\n" else " " for c in m.group()), source)
    method = re.search(r"private void OnPointerMove\(PointerMoveEvent e\)\s*\{", code)
    if not method:
        raise ValueError("OnPointerMove method missing")
    depth, end = 1, method.end()
    while depth and end < len(code):
        depth += (code[end] == "{") - (code[end] == "}")
        end += 1
    calls = list(re.finditer(r"\bPropose\(px,\s*(.*?)\s*,\s*out RackPoint entrance", code[method.end():end], re.S))
    if depth or len(calls) != 1:
        raise ValueError("expected one Propose(px, snap, out RackPoint entrance...) in OnPointerMove")
    call = calls[0]
    expression = call.group(1)
    line = source.count("\n", 0, method.end() + call.start()) + 1
    rows = [(EDITOR, line, source.splitlines()[line - 1].strip())]
    observed, expected = [], []
    for gesture in ("Reshape", "Create", "Move"):
        for shift in (False, True):
            value = re.sub(r"gesture\s*(!=|==)\s*Gesture\.(Reshape|Create|Move)",
                           lambda m: str((gesture == m[2]) == (m[1] == "==")), expression)
            value = re.sub(r"ShiftHeld\(\s*e\.modifiers\s*\)", str(shift), value)
            value = value.replace("&&", " and ").replace("||", " or ")
            value = re.sub(r"!(?!=)", " not ", value)
            value = re.sub(r"\btrue\b", "True", value)
            value = re.sub(r"\bfalse\b", "False", value)
            tree = ast.parse(value.strip(), mode="eval")
            # No eval of source: accept only boolean literals and !/&&/|| equivalents.
            def boolean(node):
                if isinstance(node, ast.Constant) and type(node.value) is bool:
                    return node.value
                if isinstance(node, ast.UnaryOp) and isinstance(node.op, ast.Not):
                    return not boolean(node.operand)
                if isinstance(node, ast.BoolOp) and isinstance(node.op, (ast.And, ast.Or)):
                    values = [boolean(v) for v in node.values]
                    return all(values) if isinstance(node.op, ast.And) else any(values)
                raise ValueError("unsupported rotation snap expression: " + expression)
            observed.append(boolean(tree.body))
            expected.append(not shift and (state == "pre-fix" or gesture != "Reshape"))
    return observed == expected, observed, rows


def snapfar_cases():
    # Compile only these two Unity-free production files and a six-case driver, outside the repo.
    dotnet = shutil.which("dotnet")
    if not dotnet:
        raise ValueError("dotnet SDK required for the reachable Unity-free SnapFar check")
    driver = r"""
using System;
using System.Globalization;
class SnapCheck {
    static int failures;
    static void Case(string name, float x, float z, bool enabled, float[] angles,
                     float wantX, float wantZ, bool wantAngle, bool wantBoundary) {
        var b = new RackEditBounds(-35.4f, -37.7f, 37.7f, 40f);
        var r = RackEditing.SnapFar(b, new RackPoint(0, 0), new RackPoint(x, z), angles, null, enabled);
        bool ok = Math.Abs(r.Far.X-wantX)<0.001f && Math.Abs(r.Far.Z-wantZ)<0.001f
            && r.AngleSnapped==wantAngle && r.BoundarySnapped==wantBoundary && r.ParallelToRackId==0;
        if (!ok) failures++;
        Console.WriteLine("SOURCE-STATIC " + (ok ? "PASS" : "FAIL") + " SnapFar " + name
            + " enabled=" + enabled + " input=(" + x.ToString("F3") + "," + z.ToString("F3")
            + ") far=(" + r.Far.X.ToString("F3") + "," + r.Far.Z.ToString("F3")
            + ") angle=" + RackEditing.DirectedAngleDegrees(new RackPoint(0,0), r.Far).ToString("F3")
            + " angleSnapped=" + r.AngleSnapped + " boundarySnapped=" + r.BoundarySnapped);
    }
    static int Main() {
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        float length = (float)Math.Sqrt(400 + 0.49);
        Case("near-angle",20,0.7f,true,new float[]{0},length,0,true,false);
        Case("near-angle-disabled",20,0.7f,false,new float[]{0},20,0.7f,false,false);
        Case("outside-angle-window",20,3,true,new float[]{0},20,3,false,false);
        Case("near-boundary",39,0,true,null,40,0,false,true);
        Case("near-boundary-disabled",39,0,false,null,39,0,false,false);
        Case("outside-boundary-window",37,0,true,null,37,0,false,false);
        return failures == 0 ? 0 : 1;
    }
}
"""
    with tempfile.TemporaryDirectory(prefix="ccf-feedback-snapfar-") as directory:
        folder = Path(directory)
        (folder / "SnapCheck.cs").write_text(driver)
        items = "".join('<Compile Include="' + escape(str(ROOT / "Assets/ForestPrototype/Racks" / name)) + '" />'
                        for name in ("RackGeometry.cs", "RackEditing.cs"))
        (folder / "check.csproj").write_text('<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup>'
            '<OutputType>Exe</OutputType><TargetFramework>net8.0</TargetFramework>'
            '<EnableDefaultCompileItems>false</EnableDefaultCompileItems></PropertyGroup><ItemGroup>'
            + items + '<Compile Include="SnapCheck.cs" /></ItemGroup></Project>')
        env = dict(os.environ, DOTNET_CLI_TELEMETRY_OPTOUT="1", DOTNET_NOLOGO="1")
        run = subprocess.run([dotnet, "run", "--project", str(folder / "check.csproj"), "--verbosity", "quiet"],
                             capture_output=True, text=True, env=env, timeout=60)
        lines = run.stdout.splitlines()
        if run.returncode not in (0, 1) or len(lines) != 6 or any(not l.startswith("SOURCE-STATIC ") for l in lines) or run.stderr:
            raise ValueError("SnapFar offline execution failed: " + run.stdout + run.stderr)
        return run.returncode == 0, lines


def check(rotation_state="pre-fix", instruction_state="pre-fix"):
    print(f"SOURCE-STATIC baseline-source-commit: {BASE}")
    diff = subprocess.run(["git", "-C", str(ROOT), "diff", "--quiet", BASE, "--",
                           "Assets/ForestPrototype"], check=False)
    if diff.returncode not in (0, 1):
        raise ValueError("cannot compare tracked production source against baseline")
    print("SOURCE-STATIC tracked-production-vs-baseline: " + ("MATCH" if diff.returncode == 0 else "DIFF"))
    print("SOURCE-STATIC scope: current checkout; no runtime/render/input proof; no automatic baseline update")
    print(f"SOURCE-STATIC selected-states: rotation={rotation_state} instruction={instruction_state}")
    inventory = [row for p in sorted((ROOT / "Assets/ForestPrototype").rglob("*.cs"))
                 for row in literals(p.relative_to(ROOT).as_posix())]

    text = read(CURRICULUM)
    start = text.index("public const string RackV2Concept =")
    end = text.index(";", start)
    concept = [r for r in literals(CURRICULUM)
               if text.count("\n", 0, start) + 1 <= r[1] <= text.count("\n", 0, end) + 1]
    spaced = any(re.search(r"16[–-]20 m", r[2]) for r in concept)
    compact = any(re.search(r"16[–-]20m", r[2]) for r in concept)
    report("PASS" if compact and not spaced else "FAIL", "01 requested-spacing-presentation-format",
           "Requested presentation format, not a functional defect; expected to flip with #255. Spacing in metres, not a duration; require compact 16–20m/16-20m.", concept)

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

    instruction_sentences = {
        "pre-fix": "Drag a rack to slide it along the headland. Drag its round handle to rotate it. Hold Shift to turn snapping off.",
        "post-fix": "Drag a rack to slide it along the headland. Its round handle rotates freely. Hold Shift while drawing to turn snapping off.",
    }
    instruction_rows = [r for r in literals(UI + "StandMapView.cs")
                        if r[2].startswith('"Drag a rack to slide it along the headland.')]
    rotation = (evidence(EDITOR, "&& OnHandle(px, selected)")
                + evidence(EDITOR, "next = Gesture.Reshape;")
                + evidence(EDITOR, "private static bool ShiftHeld")
                + evidence(EDITOR, "=> (modifiers & EventModifiers.Shift)")
                + evidence(EDITOR, "Propose(px,")
                + evidence(EDITOR, "case Gesture.Reshape:")
                + evidence(EDITOR, "SnapFar(bounds, entrance, pointer,")
                + [(path, line, read(path).splitlines()[line - 1].strip())
                   for path, line, _ in instruction_rows])
    report("NOT-TESTABLE", "06 round-handle-input",
           "BASELINE OBSERVATION only: snap argument is !ShiftHeld(e.modifiers), then forwarded to SnapFar for Reshape. SOURCE-STATIC cannot test physical input.", rotation)

    try:
        matches, observed, rows = rotation_policy(rotation_state)
        report("PASS" if matches else "FAIL", "06a rotation-snap-policy",
               f"Expected state={rotation_state}; observed Reshape/Create/Move (Shift up,down)={observed}. "
               "Pre-fix requires [True, False, True, False, True, False]; after #256 require "
               "[False, False, True, False, True, False] with --rotation-state post-fix. "
               "PASS confirms the selected source baseline, not physical input; Move receives the argument but does not call SnapFar.", rows)
    except (ValueError, SyntaxError) as error:
        report("FAIL", "06a rotation-snap-policy", " ".join(str(error).splitlines()), [])
    ok, cases = snapfar_cases()
    report("PASS" if ok else "FAIL", "06b snapfar-offline-cases",
           "Six direct calls to production SnapFar on plain .NET; 0.001m tolerance; no Unity or gesture dispatch proof.",
           evidence("Assets/ForestPrototype/Racks/RackEditing.cs", "public static RackSnapResult SnapFar"))
    for case in cases:
        print(case)

    expected_sentence = '"' + instruction_sentences[instruction_state] + '"'
    matches_instruction = len(instruction_rows) == 1 and instruction_rows[0][2] == expected_sentence
    observed_states = [state for state, sentence in instruction_sentences.items()
                       if len(instruction_rows) == 1 and instruction_rows[0][2] == '"' + sentence + '"']
    report("PASS" if matches_instruction else "FAIL", "06c rack-edit-instruction-copy",
           f"Expected state={instruction_state}; observed state={observed_states[0] if observed_states else 'neither (missing, changed or ambiguous sentence)'}. "
           f"Require exact sentence: {instruction_sentences[instruction_state]} Source copy only, not displayed text or input proof.",
           instruction_rows)

    instruction = evidence(CURRICULUM, "if (!confirmed) return counter")
    has_path = all(t in instruction[0][2] for t in ("Plan Racks", "Stand Map [M]", "Confirm Rack Plan"))
    report("PASS" if has_path else "FAIL", "07 objective-next-instruction",
           "Unconfirmed V2 instruction names Stand Map / Plan Racks path; later states are quoted separately.",
           instruction + evidence(CURRICULUM, "Return to the forest and mark")
           + evidence(CURRICULUM, "return counter + (workPending")
           + evidence(CURRICULUM, ': ". Open the Work Plan')
           + evidence(CURRICULUM, 'case "rack.create":'))
    try:
        rows = verify_objective_cta(read(VIEW), read(UI + "ScenarioOneUiRoot.cs"), collect_companions(ROOT))
        report("PASS", "08 objective-primary-cta",
               "Static, lexical source-structure check (nothing compiled or run): one named Objectives map CTA "
               "invokes ui.ShowMap(), is bound to the header row and, as far as the pinned routing text and its "
               "pinned inputs (bound to their owning class) decide, selected only for unconfirmed Rack V2 Objective 1 "
               "(unpinned root members and callers and the game/teaching state are not checked); "
               "Back to forest is the only static btn-primary source in the view class and is in the scoped "
               "demotion set; pinned scoped primary-class restoration/demotion, Help/preview/visibility guards and "
               "dynamic btn-primary/next-action application are checked. See README known limits. "
               "Stateful Unity input, focus and rendered prominence remain NOT TESTED.", rows)
    except ObjectiveCtaSourceError as error:
        # Completed FAIL with the unmet predicate and any source evidence found (exit 1, never a crash).
        report("FAIL", "08 objective-primary-cta",
               "Source-structure contract unmet: " + str(error)
               + ("" if error.predicate == str(error) else ". Predicate: " + " ".join(error.predicate.split())[:240])
               + ". This is not a Unity runtime verdict.", error.rows)

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
        parser = argparse.ArgumentParser(description=__doc__)
        parser.add_argument("--rotation-state", choices=("pre-fix", "post-fix"), default="pre-fix")
        parser.add_argument("--instruction-state", choices=("pre-fix", "post-fix"), default="pre-fix")
        args = parser.parse_args()
        raise SystemExit(check(args.rotation_state, args.instruction_state))
    except (OSError, ValueError, IndexError, subprocess.TimeoutExpired) as error:
        for line in str(error).splitlines():
            print(f"SOURCE-STATIC ERROR: {line}")
        raise SystemExit(2)
