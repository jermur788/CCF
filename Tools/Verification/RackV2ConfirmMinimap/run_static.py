#!/usr/bin/env python3
"""Read-only static verification for RACK-V2-07 (confirm/reopen/minimap)."""
import argparse
import fnmatch
import os
import re
import subprocess
import sys
from pathlib import Path
DEFAULT_ROOT = str(Path(__file__).resolve().parents[3])
DEFAULT_BASE = "36d82f027e1112529f8d167e20fb67985d28b79d"
A = "Assets/ForestPrototype"
results = []
def report(ok, message):
    results.append((ok, message))
    print(("PASS " if ok else "FAIL ") + message)
def git(root, *args):
    return subprocess.run(["git", "-C", str(root)] + list(args), capture_output=True, text=True)
def read(path):
    try:
        return path.read_text(encoding="utf-8", errors="replace")
    except OSError:
        return None
LITERAL = re.compile(
    r'//[^\n]*|/\*[\s\S]*?\*/|(?:\$@|@\$|\$|@)?"(?:[^"\\]|\\.|"")*"|\'(?:[^\'\\]|\\.)*\'')
def mask_code(text):
    """Blank comments and string/char literals, preserving offsets and newlines."""
    return LITERAL.sub(lambda m: re.sub(r"[^\n]", " ", m.group(0)), text)
def extract_body(text, signature_re, label):
    """Return the original text of a method/property body found by brace matching."""
    m = re.search(signature_re, text)
    if not m:
        return None, "signature not found: " + label
    masked = mask_code(text)
    start = masked.find("{", m.end())
    if start < 0:
        return None, "no opening brace for " + label
    depth = 0
    for i in range(start, len(masked)):
        if masked[i] == "{":
            depth += 1
        elif masked[i] == "}":
            depth -= 1
            if depth == 0:
                return text[start:i + 1], None
    return None, "unbalanced braces for " + label
def split_args(inner):
    args, depth, current = [], 0, []
    for ch in inner:
        if ch == "," and depth == 0:
            args.append("".join(current).strip())
            current = []
        else:
            depth += ch in "([{"
            depth -= ch in ")]}"
            current.append(ch)
    if current:
        args.append("".join(current).strip())
    return args
def add_call_args(text, symbol):
    pos = text.find("Add(MapSymbol." + symbol)
    if pos < 0:
        return None
    open_paren = text.find("(", pos)
    masked, depth = mask_code(text), 0
    for i in range(open_paren, len(masked)):
        depth += masked[i] == "("
        depth -= masked[i] == ")"
        if depth == 0:
            return split_args(text[open_paren + 1:i])
    return None
def check_1(root):
    text = read(root / A / "ScenarioOne/ScenarioOneManager.RackEditing.cs")
    body, err = (None, "file missing") if text is None else extract_body(
        text, r"bool\s+CanConfirmRackPlan\s*\([^)]*\)", "CanConfirmRackPlan")
    if body is None:
        return report(False, "1 " + err)
    ok = ("RackPlanTargetCount" in body and "racks.Count != RackPlanTargetCount" in body
          and "ValidateRackEdit(" in body)
    report(ok, "1 CanConfirmRackPlan reports target count and validates editable racks")
def check_2(root):
    text = read(root / A / "ScenarioOne/ScenarioOneManager.RackEditing.cs")
    body, err = (None, "file missing") if text is None else extract_body(
        text, r"RackPlanOperationResult\s+ConfirmRackPlanFromMap\s*\([^)]*\)", "ConfirmRackPlanFromMap")
    if body is None:
        return report(False, "2 " + err)
    calls = "CanConfirmRackPlan(" in body and "ConfirmRackPlan(" in body
    banned = [t for t in ["ApprovePendingWork", "MarkRackEntrance", "Completed",
                          "rackPlanConfirmed =", "workOrders.Add"] if t in body]
    report(calls and not banned,
           "2 ConfirmRackPlanFromMap delegates only" if calls and not banned
           else "2 calls=%s banned=%s" % (calls, banned))
def check_3(root, base):
    paths = {A + p for p in ["/ScenarioOne/ScenarioOneManager.RackPlan.cs", "/Racks/RackGeometry.cs",
                            "/Racks/RackPlanner.cs", "/Racks/RackEditing.cs", "/Racks/RackTool.cs",
                            "/Racks/PlannedFellMarkers.cs"]}
    for pattern in ["ScenarioOneSuggestedRackLayout*.cs", "ForestSave*.cs", "ScenarioOneSave*.cs"]:
        paths.update(str(p.relative_to(root)) for p in (root / A).rglob("*.cs")
                     if fnmatch.fnmatch(p.name, pattern))
    changed = [p for p in sorted(paths) if git(root, "diff", "--quiet", base, "HEAD", "--", p).returncode != 0]
    report(not changed,
           "3 authoritative rack/save sources unchanged since base (%d files)" % len(paths)
           if not changed else "3 changed since base: " + ", ".join(changed))
def check_4(root):
    mutation = re.compile(r"(PlannedRackCount|MarkedRackEntranceCount|RackPlanTargetCount)\s*(\+\+|--|\+=|-=|=[^=>])")
    hits, skipped = [], 0
    for p in (root / A).rglob("*.cs"):
        for lineno, line in enumerate((read(p) or "").splitlines(), 1):
            if mutation.search(line):
                if "const " in line:
                    skipped += 1
                else:
                    hits.append("%s:%d: %s" % (p, lineno, line.strip()))
    tracker = read(root / A / "UI/RackLayoutTracker.cs")
    body, err = (None, "file missing") if tracker is None else extract_body(
        tracker, r"public\s+string\s+V2CounterText\b", "V2CounterText")
    derived = body is not None and all(
        t in body for t in ["PlannedRackCount", "MarkedRackEntranceCount", "RackPlanConfirmed"])
    msg = "4 no rack-counter mutation; V2CounterText is derived"
    if skipped:
        msg += " (%d const declaration skipped)" % skipped
    if hits or not derived:
        msg = "4 mutations=%s derived=%s" % (hits[:3], derived)
        if body is None:
            msg += " (" + err + ")"
    report(not hits and derived, msg)
def check_5(root):
    source = read(root / A / "UI/Map/StandMapSource.cs")
    body, err = (None, "file missing") if source is None else extract_body(
        source, r"private\s+void\s+ReadRacks\s*\(\s*\)", "ReadRacks")
    reads_both = body is not None and "RackPlan" in body and "GetSuggestedRackLayout" in body
    banned = [".AddRack(", ".UpdateRack(", ".DeleteRack(", "ConfirmRackPlan", "ReopenRackPlan",
              "MarkRackEntrance", "ClearRackEntranceMark", "ReshapeRackFromMap",
              "CreateRackFromMap", "DeleteRackFromMap", "ResetRacksToSuggestion", "ApprovePendingWork"]
    files = ["UI/HudMinimapView.cs", "UI/Map/StandMapSource.cs", "UI/Map/StandMapDrawPlan.cs",
             "UI/Map/StandMapModel.cs", "UI/Map/StandMapSurface.cs", "UI/Map/StandMapCanvas.cs"]
    found = []
    for rel in files:
        text = read(root / A / rel)
        if text is None:
            found.append(rel + ":missing")
            continue
        found.extend(rel + ":" + token for token in banned if token in text)
    report(reads_both and not found,
           "5 ReadRacks reads the plan and suggestions; map/minimap sources are read-only"
           if reads_both and not found else "5 reads_both=%s found=%s" % (reads_both, found[:4]))
def check_6(root):
    source = read(root / A / "UI/Map/StandMapSource.cs")
    body, err = (None, "file missing") if source is None else extract_body(
        source, r"private\s+void\s+ReadRacks\s*\(\s*\)", "ReadRacks")
    if body is None:
        return report(False, "6 " + err)
    lines = body.splitlines()
    def guarded(needle):
        for i, line in enumerate(lines):
            if needle in line:
                return any("!v2" in prev for prev in lines[max(0, i - 3):i])
        return False
    guard_ok = (guarded("foreach (RackLayoutLine line in layout.Lines)")
                and guarded("foreach (RackGroup group in ui.Manager.RackGroups)"))
    leaks = []
    for rel in ["UI/LearningObjectivesView.cs", "UI/LearningObjectivesView.Sequence.cs"]:
        text = read(root / A / rel)
        if text is None:
            leaks.append(rel + ":missing")
            continue
        for lineno, line in enumerate(text.splitlines(), 1):
            if ("V2ObjectiveInstruction" in line or "V2CounterText" in line) and "RackPlanV2" not in line:
                leaks.append("%s:%d" % (rel, lineno))
    report(guard_ok and not leaks,
           "6 legacy layouts are behind !v2; V2 objective text is behind RackPlanV2"
           if guard_ok and not leaks else "6 v2_guards=%s leaks=%s" % (guard_ok, leaks[:4]))
def check_7(root):
    text = read(root / A / "UI/HudMinimapView.cs")
    note = "Dashed lines show suggested rack positions. Solid lines show your confirmed plan."
    ok = text is not None and note in text and "note.pickingMode = PickingMode.Ignore" in text \
        and "Button" not in text
    report(ok, "7 minimap teaching note is present, non-interactive and button-free")
def check_8(root):
    text = read(root / A / "UI/Map/StandMapSymbols.cs")
    suggested = add_call_args(text or "", "RackSuggested")
    player = add_call_args(text or "", "RackPlayer")
    sug_ok = (suggested is not None and len(suggested) >= 9 and suggested[4] == "true"
              and suggested[7].endswith("f") and suggested[8].endswith("f")
              and float(suggested[7][:-1]) > 0 and float(suggested[8][:-1]) > 0)
    player_ok = player is not None and len(player) == 7 and player[4] == "true"
    report(sug_ok and player_ok,
           "8 suggested racks are dashed in the mini map; player racks are solid"
           if sug_ok and player_ok else "8 suggested=%s player=%s" % (suggested, player))
def check_9(root):
    pattern = re.compile(r"(?:FromMap|ResetRacksToSuggestion|UndoLastRackEdit)\([^)]*,\s*true\s*\)")
    bad = []
    for rel in ["UI/Map/StandMapRackEditor.cs", "UI/StandMapView.cs"]:
        text = read(root / A / rel)
        if text is None:
            bad.append(rel + ":missing")
            continue
        for lineno, line in enumerate(text.splitlines(), 1):
            if pattern.search(line) and "=>" not in line:
                bad.append("%s:%d: %s" % (rel, lineno, line.strip()))
    report(not bad, "9 approved cancellation is never granted automatically"
           if not bad else "9 auto-granted consent: " + " | ".join(bad[:3]))
def check_10(root):
    problems = []
    st = git(root, "status", "--porcelain")
    if st.returncode != 0:
        problems.append("git status failed")
    for line in st.stdout.splitlines():
        rel = line[3:].strip().split(" -> ")[-1]
        if rel.startswith(A + "/") and os.path.basename(rel).startswith("RackV2"):
            problems.append("leftover " + rel)
    problems.extend("orphan meta " + str(m.relative_to(root))
                    for m in (root / A).rglob("*.meta") if not m.with_suffix("").exists())
    report(not problems, "10 no leftover RackV2 harness and no orphan .meta under ForestPrototype"
           if not problems else "10 " + "; ".join(problems[:4]))
def main():
    parser = argparse.ArgumentParser(description="Read-only RACK-V2-07 static verification")
    parser.add_argument("--root", default=DEFAULT_ROOT)
    parser.add_argument("--base", default=DEFAULT_BASE)
    parser.add_argument("--out", default=None, help="directory for report.txt")
    args = parser.parse_args()
    root = Path(os.path.abspath(args.root))
    if not (root.is_dir() and (root / A).is_dir()):
        print("FAIL 0 project root not found: " + str(root))
        print("RACK_V2_CONFIRM_MINIMAP_STATIC_FAIL failed=1")
        return 1
    for check in (lambda: check_1(root), lambda: check_2(root), lambda: check_3(root, args.base),
                  lambda: check_4(root), lambda: check_5(root), lambda: check_6(root),
                  lambda: check_7(root), lambda: check_8(root), lambda: check_9(root),
                  lambda: check_10(root)):
        check()
    failed = sum(1 for ok, _ in results if not ok)
    if args.out:
        os.makedirs(args.out, exist_ok=True)
        with open(os.path.join(args.out, "report.txt"), "w", encoding="utf-8") as f:
            for ok, message in results:
                f.write(("PASS " if ok else "FAIL ") + message + "\n")
    if failed:
        print("RACK_V2_CONFIRM_MINIMAP_STATIC_FAIL failed=%d" % failed)
        return 1
    print("RACK_V2_CONFIRM_MINIMAP_STATIC_PASS checks=%d" % len(results))
    return 0
if __name__ == "__main__":
    sys.exit(main())
