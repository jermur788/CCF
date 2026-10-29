#!/usr/bin/env python3
"""RACK-V2-10 bounded offline/static check: Tree Marks help, its unlock wiring and the V2 Objective 1 copy.

Two parts, no Unity process, nothing written inside the repository:

  static   reads the UI files that cannot compile on plain .NET (UI Toolkit/PlayerPrefs) and checks how they use the pure
           rules: the Tree Marks help context, the V2 Rack help, the Objective 1 concept/practice/instruction/step wording, and
           that no rack marking, geometry, save/schema, scene, prefab, package or settings file changed since the base.
  offline  compiles the production pure-wording files (MenuHelpView.Text.cs, LearningObjectivesView.Curriculum.cs,
           ControlsReference.cs) with RackV2HelpTeachingOfflineCheck.cs on plain .NET and runs it against the approved spec text.

  run_offline.py [--out DIR] [--base SHA]   exit 0 and RACK_V2_HELP_TEACHING_PASS on success. Needs `dotnet` (SDK 8+).

It proves wording, entry set, unlock rule and flow order. Rendering, modal timing in play and text fit are RACK-V2-13's.
"""
import argparse
import os
import re
import shutil
import subprocess
import sys
import tempfile
from pathlib import Path
from xml.sax.saxutils import escape

HERE = Path(__file__).resolve().parent
ROOT = HERE.parents[2]
UI = ROOT / "Assets/ForestPrototype/UI"
DEFAULT_BASE = "6d3228c"  # integration/scenario-one-player-test-ready-v1 with RACK-V2-09 merged (RACK-V2-10 starting HEAD)
PURE = [UI / "MenuHelpView.Text.cs", UI / "LearningObjectivesView.Curriculum.cs", UI / "ControlsReference.cs"]

results = []


def report(ok, message):
    results.append(ok)
    print(("PASS " if ok else "FAIL ") + message)


def git(*args):
    return subprocess.run(["git", "-C", str(ROOT)] + list(args), capture_output=True, text=True)


def body(text, signature):
    """Text of the first brace-delimited block after a signature regex (comments/strings are not special: bodies here are simple)."""
    m = re.search(signature, text)
    if not m:
        return ""
    start = text.find("{", m.end())
    depth = 0
    for i in range(start, len(text)):
        depth += text[i] == "{"
        depth -= text[i] == "}"
        if depth == 0:
            return text[start:i + 1]
    return ""


def static_checks(base):
    root_ui = (UI / "ScenarioOneUiRoot.cs").read_text()
    context = body(root_ui, r"private\s+MenuHelpView\.Menu\s+HelpContext\s*\(\s*\)")
    report("MenuHelpView.TreeMarksUnlocked(manager.RackPlanningV2Active, manager.RackPlanConfirmed)" in context,
           "S1 HelpContext unlocks Tree Marks from saved state only: V2 active and plan confirmed")
    report("MenuHelpView.Introduced(MenuHelpView.Menu.TreeMarks)" in context and "marking.RackSprayEquipped" in context
           and "return MenuHelpView.Menu.TreeMarks;" in context,
           "S2 HelpContext shows Tree Marks until first seen, and while the spray marker is equipped")
    order = [context.find(token) for token in ("Menu.Rack;", "Menu.WorkPlan;", "Menu.StandMap;", "Menu.AnnualReview;",
                                               "Menu.TreeInspection;", "Menu.TreeMarks;", "Menu.WalkingHud;")]
    report(all(i >= 0 for i in order) and order == sorted(order),
           "S3 Tree Marks is a walking-forest context only: after every screen and inspection, before the Walking HUD")
    introduce = re.search(r"if \(screen != UiScreen\.Objectives && context != MenuHelpView\.Menu\.AnnualReview && context != MenuHelpView\.Menu\.Rack\)\s*\n\s*help\.Introduce\(context\);", root_ui)
    report(introduce is not None, "S4 the existing context Introduce path (not a parallel help UI) opens Tree Marks on first use")
    help_view = (UI / "MenuHelpView.cs").read_text()
    render = body(help_view, r"private\s+void\s+Render\s*\(")
    report("ui.Manager.RackPlanningV2Active" in render and "text = RackV2Explanation;" in render,
           "S5 the V2 Rack help uses the checked RackV2Explanation, inside the V2 branch")
    learning = (UI / "LearningObjectivesView.cs").read_text()
    report("bool rackV2Objective = TeachingRacks && ui.Manager.RackPlanningV2Active;" in learning
           and "rackV2Objective ? RackV2Concept : lesson.Concept" in learning and "rackV2Objective ? RackV2Practice : lesson.Practice" in learning,
           "S6 Objective 1 why-it-matters and good-practice use the V2 copy only for the V2 rack objective")
    report("if (ui.Manager.RackPlanningV2Active) text = RackV2StepText(step.Id) ?? text;" in learning,
           "S7 rack lesson texts use RackV2StepText only in a V2 world")
    tracker = (UI / "RackLayoutTracker.cs").read_text()
    instruction = body(tracker, r"public\s+string\s+V2ObjectiveInstruction")
    report("LearningObjectivesView.RackV2Instruction(V2CounterText, m.RackPlanConfirmed, m.MarkedRackEntranceCount," in instruction
           and "ScenarioOneManager.RackPlanTargetCount, WorkPending)" in instruction,
           "S8 the V2 objective sentence (HUD widget and Objectives screen) is the checked RackV2Instruction")
    for path in PURE:
        text = path.read_text()
        # The plain .NET compile below is the real proof (no UnityEngine reference exists there); this names the file that broke it.
        report("using UnityEngine" not in text, "S9 %s stays free of UnityEngine" % path.name)

    changed = git("diff", "--name-only", base, "HEAD").stdout.split() + git("diff", "--name-only", "HEAD").stdout.split() \
        + git("ls-files", "--others", "--exclude-standard").stdout.split()
    if git("rev-parse", "--verify", base + "^{commit}").returncode != 0:
        report(False, "S10 base %s is not a commit here" % base)
        return
    allowed = {
        "Assets/ForestPrototype/UI/MenuHelpView.Text.cs", "Assets/ForestPrototype/UI/MenuHelpView.cs",
        "Assets/ForestPrototype/UI/ScenarioOneUiRoot.cs", "Assets/ForestPrototype/UI/LearningObjectivesView.cs",
        "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs", "Assets/ForestPrototype/UI/RackLayoutTracker.cs",
        "Tools/Verification/MenuTutorialVerification.cs",
    }
    outside = sorted({p for p in changed if p not in allowed and not p.startswith("Tools/Verification/RackV2HelpTeaching/")
                      and not p.startswith("Docs/Handoffs/RACK-V2-10/")})
    report(not outside, "S10 only the RACK-V2-10 copy/wiring files changed since %s%s" % (base, "" if not outside else ": " + ", ".join(outside[:6])))
    protected = [p for p in changed if re.search(r"(ForestTreeMarkingManager|ScenarioOneManager|ForestSave|Racks/|\.unity$|\.prefab$|\.mat$|\.asset$|^ProjectSettings/|^Packages/|\.meta$)", p)]
    report(not protected, "S11 no marking, rack geometry, manager, save/schema, scene, prefab, material, settings, package or .meta change")
    untracked = [p for p in git("ls-files", "--others", "--exclude-standard", "--", "Assets").stdout.split()]
    report(not untracked, "S12 no untracked file (copied harness or generated .meta) under Assets/%s" % ("" if not untracked else ": " + ", ".join(untracked[:4])))


def offline(out):
    dotnet = shutil.which("dotnet")
    if not dotnet:
        report(False, "dotnet SDK not found")
        return
    sources = PURE + [HERE / "RackV2HelpTeachingOfflineCheck.cs"]
    out.mkdir(parents=True, exist_ok=True)
    items = "\n".join('<Compile Include="%s" />' % escape(str(p)) for p in sources)
    (out / "check.csproj").write_text(
        '<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net8.0</TargetFramework>'
        '<LangVersion>9.0</LangVersion><Nullable>disable</Nullable><ImplicitUsings>disable</ImplicitUsings>'
        '<EnableDefaultCompileItems>false</EnableDefaultCompileItems><StartupObject>RackV2HelpTeachingOfflineCheck</StartupObject></PropertyGroup>'
        "<ItemGroup>" + items + "</ItemGroup></Project>")
    env = dict(os.environ, DOTNET_CLI_TELEMETRY_OPTOUT="1", DOTNET_NOLOGO="1")
    build = subprocess.run([dotnet, "build", "-nologo", "-v", "q", "-c", "Release"], cwd=out, env=env, capture_output=True, text=True)
    if build.returncode:
        sys.stdout.write(build.stdout + build.stderr)
        report(False, "offline build")
        return
    run = subprocess.run([dotnet, "run", "--no-build", "-c", "Release", "--", str(ROOT)], cwd=out, env=env, capture_output=True, text=True)
    sys.stdout.write(run.stdout + run.stderr)
    report(run.returncode == 0 and "RACK_V2_HELP_TEACHING_OFFLINE_PASS" in run.stdout, "offline wording/unlock/flow check")


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument("--out", type=Path, default=Path(tempfile.gettempdir()) / "ccf-rack-v2-help-teaching-offline")
    parser.add_argument("--base", default=DEFAULT_BASE)
    args = parser.parse_args()
    static_checks(args.base)
    offline(args.out)
    failed = results.count(False)
    print(("RACK_V2_HELP_TEACHING_PASS" if not failed else "RACK_V2_HELP_TEACHING_FAIL failed=%d" % failed) + " static_and_offline=%d" % len(results))
    return 1 if failed else 0


if __name__ == "__main__":
    sys.exit(main())
