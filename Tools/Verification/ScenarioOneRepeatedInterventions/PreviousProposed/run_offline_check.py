#!/usr/bin/env python3
"""T-05: execute production UI method slices with a headless UI/manager shim, never Unity.

Uses real work-order/history types and unchanged method text. The Annual Review Refresh prefix
includes its first/new-latest selection reset and acknowledgement visibility; other rendering is
outside this gate. MFM creation uses the real CreateFellingOrder and provenance-writing loop,
not the entire prescription transaction. Run the existing MFM-07 gate alongside this check.
Outputs, generated slices, source hashes and optional deliberately broken copies stay on Files.
"""
import argparse
import hashlib
import json
import os
from pathlib import Path
import subprocess
import sys
from xml.sax.saxutils import escape

HERE = Path(__file__).resolve().parent
ROOT = HERE.parents[3]
FP = ROOT / "Assets/ForestPrototype"
sys.path.insert(0, str(ROOT / "Tools/Verification/S1CManagementHistory"))
import run_offline_check as history


def method(source, signature):
    start = source.index(signature)
    brace = source.index("{", start)
    arrow = source.find("=>", start, brace)
    if arrow >= 0:
        return source[start:source.index(";", arrow) + 1]
    depth, end = 1, brace + 1
    while depth:
        if source[end] == "{":
            depth += 1
        elif source[end] == "}":
            depth -= 1
        end += 1
    return source[start:end]


def generate(work, annual, root, plan):
    work_methods = ["private VisualElement Card(", "private void BuildOrderGroups(",
                    "private static string OrderClass(", "private static bool HasOriginHistory(",
                    "private void AddOrderOrigin(", "private string Status("]
    # Exact Refresh prefix through visibility, including its real early-out and latest reset.
    refresh = method(annual, "public void Refresh(bool force)")
    prefix = refresh[:refresh.index("// Completion guidance only:")]
    view = "\n".join(method(annual, s) for s in ["public void ShowLatest()", "public bool TryShowYear(",
                      "private static YearHistoryView FindYear(", "private string ChangeKey("])
    # Keep initial selection state tied to production declarations, not hand-copied expectations.
    fields = "\n".join(line.strip() for line in annual.splitlines()
                       if any(line.strip().startswith("private " + s) for s in
                              ["int viewYear =", "int lastLatestYear =", "string shownKey =", "bool showingEarlierYear;"]))
    manager = (FP / "ScenarioOne/ScenarioOneManager.cs").read_text()
    writer = (FP / "ScenarioOne/ScenarioOneManager.PrescriptionApproval.cs").read_text()
    creation = writer[writer.index("var created = new List<ScenarioOneWorkOrder>(plan.Orders.Count);"):
                      writer.index("foreach (ScenarioOneWorkOrder order in created)")]
    generated = """using System; using System.Collections.Generic; using System.Linq;
using UnityEngine; using UnityEngine.UIElements; using CCF.Forestry.WorkEconomy;
public sealed partial class WorkPlanView {
private readonly ScenarioOneUiRoot ui; public readonly VisualElement scroll = new VisualElement();
public WorkPlanView(ScenarioOneUiRoot ui) { this.ui = ui; }
public void Build() { scroll.Clear(); BuildOrderGroups(ui.Manager); }
public static string Class(ScenarioOneWorkOrder o, int y) => OrderClass(o,y);
public string ReadStatus(ScenarioOneWorkOrder o) => Status(o);
""" + "\n".join(method(plan, s) for s in work_methods) + "\n}\n"
    generated += """public sealed partial class AnnualReviewView {
private readonly ScenarioOneUiRoot ui;
private readonly HashSet<string> openDetails = new HashSet<string>();
private readonly VisualElement scroll = new VisualElement(), acknowledgement = new VisualElement();
public AnnualReviewView(ScenarioOneUiRoot ui) { this.ui = ui; }
public int SelectedYear => viewYear; public int RenderedYear; public int Refreshes;
public bool FooterVisible => acknowledgement.style.display == DisplayStyle.Flex;
""" + fields + "\n" + view + "\n" + prefix + "\nRenderedYear = year?.Year ?? -1; Refreshes++; }\n}\n"
    generated += """public sealed partial class ScenarioOneUiRoot {
public enum UiScreen { None, Map, Review, Objectives }
private UiScreen screen; private readonly AnnualReviewView review;
private readonly ScenarioOneManager manager;
private readonly MapStub map = new MapStub(); private readonly LearningStub learning = new LearningStub();
public ScenarioOneUiRoot(ScenarioOneManager m) { manager=m; review=new AnnualReviewView(this); }
public ScenarioOneManager Manager => manager; public LearningStub Learning => learning;
public bool NeedsAnnualReview => !manager.AnnualReviewSeen;
public UiScreen CurrentScreen => screen; public AnnualReviewView Review => review;
public bool HelpOpen=true; private void CloseHelp() { HelpOpen=false; }
""" + "\n".join(method(root, s) for s in ["public void SetScreen(", "public void ShowReview()", "public bool TryShowReviewYear("]) + "\n}\n"
    generated += "public sealed partial class ScenarioOneManager {\n" + method(manager, "private ScenarioOneWorkOrder CreateFellingOrder(") + """
public List<ScenarioOneWorkOrder> MakePrescriptionOrders(Plan plan, Dictionary<string, ForestTree> living) {
""" + creation + "return created; }\n}\n"
    (work / "ProductionSlices.cs").write_text(generated)
    shim = (ROOT / "Tools/Verification/S1CManagementHistory/UnityShim.cs").read_text()
    shim = shim.replace("public static class Mathf\n    {", "public static class Mathf\n    {\n        public static int CeilToInt(float v) => (int)Math.Ceiling(v);")
    (work / "UnityShim.cs").write_text(shim)
    filename, bridge = history.bridge_ui_facts()
    (work / filename).write_text(bridge)
    sources = [Path(s) for s in history.SOURCES if not s.endswith(("S1CPerYearHistoryOfflineCheck.cs", "UnityShim.cs"))]
    sources += [work / "ProductionSlices.cs", work / "UnityShim.cs", work / filename,
                HERE / "HeadlessUi.cs", HERE / "PreviousProposedOfflineCheck.cs"]
    items = "\n".join('<Compile Include="' + escape(str(s), {'"': '&quot;'}) + '" />' for s in sources)
    (work / "check.csproj").write_text(history.PROJECT.replace("S1CPerYearHistoryOfflineCheck", "PreviousProposedOfflineCheck").format(items=items))


def run(work):
    env = dict(os.environ, DOTNET_CLI_TELEMETRY_OPTOUT="1", DOTNET_NOLOGO="1", CCF_REPO_ROOT=str(ROOT))
    build = subprocess.run(["dotnet", "build", "-v", "q", "-c", "Release"], cwd=work, env=env, capture_output=True, text=True)
    if build.returncode:
        return "BUILD_FAILED", build.stdout + build.stderr
    result = subprocess.run(["dotnet", "run", "--no-build", "-c", "Release"], cwd=work, env=env, capture_output=True, text=True)
    return ("PASS" if result.returncode == 0 else "FAIL"), result.stdout + result.stderr


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--out", type=Path, default=Path("/media/jer/Files/CCF/Verification/S1-D-T05-20261030/previous-proposed"))
    parser.add_argument("--mutations", action="store_true")
    args = parser.parse_args()
    work = args.out.resolve()
    if work == ROOT or ROOT in work.parents:
        parser.error("--out must be outside the repository")
    work.mkdir(parents=True, exist_ok=True)
    paths = [FP / "UI" / (name + ".cs") for name in ["AnnualReviewView", "ScenarioOneUiRoot", "WorkPlanView"]]
    annual, root, plan = [p.read_text() for p in paths]
    generate(work, annual, root, plan)
    status, output = run(work)
    (work / "result.log").write_text(output)
    print(output, end="")
    (work / "source-manifest.json").write_text(json.dumps({str(p.relative_to(ROOT)): hashlib.sha256(p.read_bytes()).hexdigest()
         for p in paths + [FP / "ScenarioOne/ScenarioOneWorkOrder.cs", FP / "ScenarioOne/ScenarioOneManager.cs",
                           FP / "ScenarioOne/ScenarioOneManager.PrescriptionApproval.cs"]}, indent=2) + "\n")
    if status != "PASS":
        return 1
    if args.mutations:
        mutations = [
            ("carry-over uses stale threshold", 2, "order.createdYear < currentYear) return", "order.createdYear <= currentYear - 2) return"),
            ("pending prescription called approved", 2, 'if (order.status == ScenarioWorkStatus.Approved) return "Approved";',
             'if (order.prescriptionId > 0 || order.status == ScenarioWorkStatus.Approved) return "Approved";'),
            ("missing year silently selects latest", 0, "if (FindYear(years, year) == null) return false;", "if (FindYear(years, year) == null) { ShowLatest(); return false; }"),
            ("first refresh loses exact year", 0, "lastLatestYear = years[years.Count - 1].Year;", "lastLatestYear = -1;"),
            ("route resets to latest", 1, "screen = UiScreen.Review;", "SetScreen(UiScreen.Review);"),
            ("missing origin year gets link", 2, "years.Any(y => y.Year == order.createdYear)", "years.Any()"),
            ("future pending silently new", 2, "order.createdYear == currentYear) return", "order.createdYear >= currentYear) return"),
        ]
        for index, (name, source, old, new) in enumerate(mutations):
            texts = [annual, root, plan]
            if texts[source].count(old) != 1:
                raise RuntimeError("mutation target must occur once: " + name)
            texts[source] = texts[source].replace(old, new)
            mutant = work / ("mutant-" + str(index))
            mutant.mkdir(exist_ok=True)
            generate(mutant, *texts)
            state, log = run(mutant)
            (mutant / "result.log").write_text(log)
            print("MUTATION", "CAUGHT" if state == "FAIL" else "SURVIVED", name, state)
            if state != "FAIL":
                return 1
        print("S1D_T05_MUTATIONS_PASS caught=7 survived=0")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
