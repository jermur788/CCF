#!/usr/bin/env python3
"""MFM-09 offline / static qualification for "Extend your treatment" on the plain .NET SDK (no Unity).

Compiles the PRODUCTION ScenarioOneTeachingSequence.cs with the pure rack-area types, the read-only latch members extracted from
ScenarioOneManager.cs, and two methods (OrderClass, PrescriptionOriginNote) plus one constant extracted verbatim from the PRODUCTION
WorkPlanView.cs, in a temporary project outside the repository. It also audits the source text:

  * the eight accepted lessons, Current/CurrentUnlatched/CrossesFirstRackReview/RecruitmentAndLater and WorkPlanView.OrderClass are
    byte-identical to the exact start commit;
  * the Work Plan group labels are still exactly Approved / Pending new / Pending carried over (no fourth status);
  * no line MFM-09 ADDED calls a Work Plan approval, prescription write, mark, advance-year or latch API, or adds a saved field;
  * the production change is limited to the four owned files; no save/schema/hash/reference/MFM-08/Rack V2 file changed.

--mutations then re-runs the same checks against deliberately broken copies and requires every mutant to be caught.
This proves pure rules and source text only: it is NOT Unity-verified.

Usage: run_offline.py --out DIR [--base SHA] [--mutations]      (DIR must be under /media/jer/Files/CCF/Verification)
"""
import argparse, json, os, re, shutil, subprocess, sys
from pathlib import Path

HERE = Path(__file__).resolve().parent
ROOT = HERE.parents[2]
GAME = ROOT / "Assets" / "ForestPrototype"
START = "34e360f85835afd15f5323f287372e0f2ee83dd9"
SEQ = "Assets/ForestPrototype/UI/ScenarioOneTeachingSequence.cs"
WP = "Assets/ForestPrototype/UI/WorkPlanView.cs"
OBJ = "Assets/ForestPrototype/UI/LearningObjectivesView.cs"
OBJSEQ = "Assets/ForestPrototype/UI/LearningObjectivesView.Sequence.cs"
OWNED_PRODUCTION = {SEQ, WP, OBJ, OBJSEQ}
OWNED_OTHER_PREFIXES = ("Tools/Verification/Mfm09ExtensionFlow/", "Docs/Handoffs/MFM-09-Teaching-WorkPlan-Integration/")
FORBIDDEN_ADDED = re.compile(
    r"ApprovePendingWork|ApprovePrescription|CreatePrescription|AddMarkedTreesToWorkPlan|AdvanceYear|ConfirmCropTree|ConfirmRack|"
    r"TryRecordFirstGuidedRackReview|firstGuidedRackReviewComplete\s*=|SetMark|MarkForFell|CancelApprovedOrder|RemovePendingOrder|"
    r"PlayerPrefs\.Set|\[SerializeField\]|\[System\.Serializable\]|JsonUtility|CaptureSaveData|\.Add\(\s*new\s+Prescription", re.I)
PROJECT = """<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType><TargetFramework>net8.0</TargetFramework><LangVersion>9.0</LangVersion>
    <Nullable>disable</Nullable><ImplicitUsings>disable</ImplicitUsings><EnableDefaultCompileItems>false</EnableDefaultCompileItems>
    <StartupObject>Mfm09OfflineCheck</StartupObject>
  </PropertyGroup>
  <ItemGroup>
{items}
  </ItemGroup>
</Project>
"""
SOURCES = [
    GAME / "UI" / "ScenarioOneRackBayText.cs", GAME / "ScenarioOne" / "ScenarioOneRackBay.cs", GAME / "ScenarioOne" / "CompetitionShare.cs",
    GAME / "Racks" / "RackGeometry.cs", ROOT / "Tools" / "Verification" / "ScenarioOneNonUi" / "OfflineLogic" / "OfflineStubs.cs",
    HERE / "Mfm09OfflineCheck.cs",
]

# (name, file key, find, replace) — each must match exactly once in the clean file.
MUTATIONS = [
    ("renamed old ID", SEQ, 'new Lesson("access",', 'new Lesson("access-renamed",'),
    ("renamed old ID (response)", SEQ, 'new Lesson("response",', 'new Lesson("review-response",'),
    ("reused old ID for the extension", SEQ, 'public const string ExtensionId = "extend-treatment";', 'public const string ExtensionId = "recruitment";'),
    ("extension placed before response", SEQ, "Lessons[0], Lessons[1], Lessons[2], Lessons[3], Lessons[4], Extension, Lessons[5]",
     "Lessons[0], Lessons[1], Lessons[2], Lessons[3], Extension, Lessons[4], Lessons[5]"),
    ("extension offered without response", SEQ, "&& !f.ExtensionApproved && Current(f) == 5;", "&& !f.ExtensionApproved;"),
    ("D-066 latch ignored when the extension applies", SEQ,
     "=> f.FirstGuidedRackReviewComplete ? RecruitmentAndLater(f) : CurrentUnlatched(f);",
     "=> (f.FirstGuidedRackReviewComplete && !f.ExtensionApplies) ? RecruitmentAndLater(f) : CurrentUnlatched(f);"),
    ("extension reopens the review (latch reinterpreted)", SEQ, "if (ExtensionOffered(f)) return 5;", "if (ExtensionOffered(f)) return 4;"),
    ("extension/outside-sample work back-credits the original lesson", SEQ,
     "if (!f.ReleaseReviewed || (f.Bay != null ? !f.RackReviewComplete",
     "if (!(f.ReleaseReviewed || f.ExtensionApproved) || (f.Bay != null ? !f.RackReviewComplete"),
    ("approved extension never returns to recruitment", SEQ, "public static bool ExtensionOffered(Facts f) => f.ExtensionApplies && !f.ExtensionApproved",
     "public static bool ExtensionOffered(Facts f) => f.ExtensionApplies"),
    ("prescription work classified Approved", WP,
     'if (order.status == ScenarioWorkStatus.Approved) return "Approved";',
     'if (order.status == ScenarioWorkStatus.Approved || order.prescriptionId > 0) return "Approved";'),
    ("prescription work gets a fourth status", WP,
     'if (!order.IsOpen) return "";\n        if (order.status == ScenarioWorkStatus.Approved)',
     'if (!order.IsOpen) return "";\n        if (order.prescriptionId > 0) return "Prescription";\n        if (order.status == ScenarioWorkStatus.Approved)'),
    ("provenance changes new vs carried over", WP,
     'order.createdYear == currentYear) return "Pending new";',
     'order.createdYear == currentYear && order.prescriptionId == 0) return "Pending new";'),
    ("fourth group label added", WP, 'new[] { "Approved", "Pending new", "Pending carried over" }', 'new[] { "Approved", "Pending new", "Pending carried over", "From prescription" }'),
    ("teaching action calls Work Plan approval", OBJSEQ,
     "var accepted = ui.Manager.Prescriptions.LastOrDefault(", "ui.Manager.ApprovePendingWork(); var accepted = ui.Manager.Prescriptions.LastOrDefault("),
    ("teaching action approves the prescription", OBJSEQ,
     "var accepted = ui.Manager.Prescriptions.LastOrDefault(", "ui.Manager.ApprovePrescription(1); var accepted = ui.Manager.Prescriptions.LastOrDefault("),
    ("Work Plan note action commits work", WP,
     'UiKit.Add(card, PrescriptionGroupNote, "muted").name', 'm.ApprovePendingWork(); UiKit.Add(card, PrescriptionGroupNote, "muted").name'),
    ("extension completion derived by writing the latch", OBJSEQ,
     "ExtensionApplies = m.RackPlanningV2Active && m.RackPlanConfirmed,", "ExtensionApplies = m.RackPlanningV2Active && m.RackPlanConfirmed && m.TryRecordFirstGuidedRackReview(null, null),"),
]


def git(*a):
    return subprocess.run(["git", "-C", str(ROOT), *a], capture_output=True, text=True)


def base_text(path, base):
    r = git("show", base + ":" + path)
    if r.returncode: sys.exit("cannot read %s at %s" % (path, base))
    return r.stdout


def block(text, start_marker, open_ch="{", close_ch="}"):
    """Text from start_marker through its matching close brace (or the semicolon-terminated array)."""
    i = text.index(start_marker)
    j = text.index(open_ch, i)
    depth = 0
    for k in range(j, len(text)):
        if text[k] == open_ch: depth += 1
        elif text[k] == close_ch:
            depth -= 1
            if depth == 0: return text[i:k + 1]
    raise ValueError(start_marker)


def method(text, signature):
    """Body of an expression-bodied (=> ... ;) or braced method starting at signature."""
    i = text.index(signature)
    brace = text.find("{", i); arrow = text.find("=>", i); semi = text.find(";", i)
    if arrow != -1 and (brace == -1 or arrow < brace) and arrow < semi + 1:
        # expression body: up to the terminating semicolon at depth 0 of parentheses
        depth = 0
        for k in range(arrow, len(text)):
            c = text[k]
            if c in "([{": depth += 1
            elif c in ")]}": depth -= 1
            elif c == ";" and depth <= 0: return text[i:k + 1]
    return block(text, signature)


def accepted_regions(texts):
    seq, wp = texts[SEQ], texts[WP]
    return {
        "Lessons array": block(seq, "public static readonly Lesson[] Lessons"),
        "Current": method(seq, "public static int Current(Facts f)"),
        "CurrentUnlatched": method(seq, "public static int CurrentUnlatched(Facts f)"),
        "CrossesFirstRackReview": method(seq, "public static bool CrossesFirstRackReview("),
        "RecruitmentAndLater": method(seq, "private static int RecruitmentAndLater("),
        "WorkPlanView.OrderClass": method(wp, "private static string OrderClass("),
    }


def static_audit(texts, base, changed):
    problems = []
    old = {SEQ: base_text(SEQ, base), WP: base_text(WP, base)}
    for name, old_region in accepted_regions(old).items():
        if accepted_regions(texts)[name] != old_region:
            problems.append("accepted region changed vs start: " + name)
    if 'new[] { "Approved", "Pending new", "Pending carried over" }' not in texts[WP]:
        problems.append("Work Plan group labels are no longer exactly Approved / Pending new / Pending carried over")
    if "prescription" in accepted_regions(texts)["WorkPlanView.OrderClass"].lower():
        problems.append("OrderClass reads prescription provenance")
    for path in sorted(OWNED_PRODUCTION):
        added = [l[1:] for l in difflines(base, path, texts[path]) if l.startswith("+") and not l.startswith("+++")]
        for line in added:
            if FORBIDDEN_ADDED.search(line):
                problems.append("forbidden call/field in a line MFM-09 added to %s: %s" % (path.rsplit("/", 1)[1], line.strip()[:90]))
    unowned = sorted(p for p in changed if p not in OWNED_PRODUCTION and not p.startswith(OWNED_OTHER_PREFIXES))
    if unowned: problems.append("unowned files changed vs start: " + ", ".join(unowned))
    return problems


def difflines(base, path, current_text):
    import difflib
    return list(difflib.unified_diff(base_text(path, base).splitlines(), current_text.splitlines(), lineterm="", n=0))


def extract_workplan(wp_text, work):
    group = re.search(r'    public const string PrescriptionGroupNote = [^\n]*\n', wp_text).group(0)
    note = method(wp_text, "public static string PrescriptionOriginNote(")
    order = method(wp_text, "private static string OrderClass(").replace("private static string OrderClass", "public static string OrderClass")
    path = work / "WorkPlanExtract.g.cs"
    path.write_text("public static class WorkPlanExtract\n{\n" + group + "\n    " + note + "\n\n    " + order + "\n}\n")
    return path


def latch_manager(work):
    text = (GAME / "ScenarioOne" / "ScenarioOneManager.cs").read_text()
    start = text.index("    private bool firstGuidedRackReviewComplete;")
    end = text.index("    public void InitializeNewScenario()", start)
    path = work / "LatchManager.g.cs"
    path.write_text("public sealed partial class ScenarioOneManager { private bool referencePreviewActive;\n" + text[start:end] + "}\n")
    return path


def build_and_run(dotnet, work, seq_path, wp_text):
    work.mkdir(parents=True, exist_ok=True)
    sources = [seq_path] + SOURCES + [latch_manager(work), extract_workplan(wp_text, work)]
    (work / "check.csproj").write_text(PROJECT.format(items="\n".join('    <Compile Include="%s" />' % s for s in sources)))
    env = dict(os.environ, DOTNET_CLI_TELEMETRY_OPTOUT="1", DOTNET_NOLOGO="1")
    b = subprocess.run([dotnet, "build", "-nologo", "-v", "q", "-c", "Release"], cwd=work, env=env, capture_output=True, text=True)
    if b.returncode: return "BUILD_FAILED", b.stdout + b.stderr
    r = subprocess.run([dotnet, "run", "--no-build", "-c", "Release"], cwd=work, env=env, capture_output=True, text=True)
    return ("OK" if r.returncode == 0 else "CHECK_FAILED"), r.stdout + r.stderr


def run_case(dotnet, work, texts, base, changed):
    """One full evaluation (compile+run+static audit) of a set of file texts. Returns (ok, lines)."""
    work.mkdir(parents=True, exist_ok=True)
    seq_path = work / "ScenarioOneTeachingSequence.cs"
    seq_path.write_text(texts[SEQ])
    status, output = build_and_run(dotnet, work, seq_path, texts[WP])
    problems = static_audit(texts, base, changed)
    failing = [l for l in output.splitlines() if l.startswith("FAIL")] + ["STATIC " + p for p in problems]
    if status == "BUILD_FAILED": failing.append("BUILD_FAILED " + output.strip().splitlines()[-1][:120])
    return (status == "OK" and not problems), output, problems, failing


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument("--out", required=True, type=Path)
    ap.add_argument("--base", default=START)
    ap.add_argument("--mutations", action="store_true")
    a = ap.parse_args()
    out = a.out.resolve()
    if not out.is_relative_to(Path("/media/jer/Files/CCF/Verification")): sys.exit("evidence must be under /media/jer/Files/CCF/Verification")
    dotnet = shutil.which("dotnet") or sys.exit("dotnet not found")
    if git("cat-file", "-e", a.base + "^{commit}").returncode: sys.exit("base not found")
    changed = sorted(set(git("diff", "--name-only", a.base).stdout.split()) | set(git("ls-files", "--others", "--exclude-standard").stdout.split()))
    texts = {p: (ROOT / p).read_text() for p in OWNED_PRODUCTION}
    out.mkdir(parents=True, exist_ok=True)
    ok, output, problems, failing = run_case(dotnet, out / "candidate", texts, a.base, changed)
    (out / "check.log").write_text(output + "\n".join("STATIC_PROBLEM " + p for p in problems) + "\n")
    (out / "production-diff.txt").write_text("base=%s\n%s\n" % (a.base, "\n".join(changed)))
    sys.stdout.write(output)
    for p in problems: print("STATIC_PROBLEM", p)
    print("MFM09_STATIC_AUDIT_%s base=%s changed=%d" % ("PASS" if not problems else "FAIL", a.base[:8], len(changed)))
    if not ok:
        print("MFM09_OFFLINE_FAIL"); return 1
    if not a.mutations: return 0
    survived = []
    lines = []
    for i, (name, key, find, repl) in enumerate(MUTATIONS):
        if texts[key].count(find) != 1: sys.exit("mutation '%s': expected exactly one match in %s, found %d" % (name, key, texts[key].count(find)))
        mt = dict(texts); mt[key] = texts[key].replace(find, repl)
        mok, moutput, mproblems, mfailing = run_case(dotnet, out / ("mutant%02d" % i), mt, a.base, changed)
        if mok:
            survived.append(name); line = "MUTATION SURVIVED %-62s" % name
        else:
            line = "MUTATION CAUGHT   %-62s %s" % (name, (mfailing[0] if mfailing else "?")[:100])
        print(line); lines.append(line)
    (out / "mutations.txt").write_text("\n".join(lines) + "\n")
    print("MFM09_MUTATIONS_%s caught=%d survived=%d" % ("PASS" if not survived else "FAIL", len(MUTATIONS) - len(survived), len(survived)))
    return 0 if not survived else 1


if __name__ == "__main__":
    sys.exit(main())
