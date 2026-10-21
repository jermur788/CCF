#!/usr/bin/env python3
"""Build and run the progressive-diagram offline check on the plain .NET SDK (no Unity, nothing written inside the repo).

  run_diagram_check.py [--design STAND_JSON] [--out DIR]

Compiles Assets/ForestPrototype/UI/TeachingDiagramPlan.cs (plain C#) with OfflineLogic/TeachingDiagramPlanCheck.cs in a temporary
project, runs the structural checks, and - when --design points at the approved design data (stand.json from the diagram design package) -
also compares every drawn item, state, mark, label and legend entry with that data. Exit 0 and DIAGRAM_PLAN_PASS on success.
Two label wording corrections are applied deliberately to the design data before comparing (see CORRECTIONS).
"""
import argparse, json, os, shutil, subprocess, sys, tempfile

HERE = os.path.dirname(os.path.abspath(__file__))
ROOT = os.path.abspath(os.path.join(HERE, "..", "..", "..", ".."))
PLAN = os.path.join(ROOT, "Assets", "ForestPrototype", "UI", "TeachingDiagramPlan.cs")
# The design data says "Forego" (to go before); the verb meant is "forgo" (to do without), as the lesson text itself says. "browse" -> the game's "browsing".
CORRECTIONS = {"Forego this timber sale": "Forgo this timber sale", "Light, browse, vegetation": "Light, browsing, vegetation", "Read browse, vegetation": "Read browsing, vegetation"}

PROJECT = """<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>{tfm}</TargetFramework>
    <LangVersion>9.0</LangVersion>
    <Nullable>disable</Nullable>
    <ImplicitUsings>disable</ImplicitUsings>
    <EnableDefaultCompileItems>false</EnableDefaultCompileItems>
    <StartupObject>TeachingDiagramPlanCheck</StartupObject>
  </PropertyGroup>
  <ItemGroup>
    <Compile Include="{plan}" />
    <Compile Include="{check}" />
  </ItemGroup>
</Project>
"""


def n(v):
    s = ("%.3f" % round(float(v), 3)).rstrip("0").rstrip(".")
    return s if s not in ("", "-0") else "0"


def canonical_from_design(path):
    d = json.load(open(path))
    elements = {e["id"]: e for k in ["trees", "racks", "openings", "seedlings", "plantedTrees", "shelters", "logs", "decorations"] for e in d[k]}

    def line(prefix, item):
        e = elements[item["id"]]
        kind = {"tree": "crown", "rack": "rack", "opening": "opening", "log": "log", "young": "young", "shelter": "shelter", "clock": "clock", "repeat": "repeat"}[e["kind"]]
        x = y = x1 = y1 = r = rx = ry = w = h = 0
        shape = "-"
        if kind == "crown": x, y, r = e["x"], e["y"], item.get("crownR", e["crownR"])
        elif kind == "rack": x, y, x1, y1, w = e["x0"], e["y0"], e["x1"], e["y1"], e["width"]
        elif kind == "opening": x, y, rx, ry = e["x"], e["y"], e["rx"], e["ry"]
        elif kind == "log": x, y, x1, y1, w = e["x0"], e["y0"], e["x1"], e["y1"], e["width"]
        elif kind == "young": x, y, r = e["x"], e["y"], e["r"]; shape = {"circle": "circle", "ellipse": "oval", "diamond": "diamond"}[e["shape"]]
        elif kind == "shelter": x, y, w, h = e["x"], e["y"], e["w"], e["h"]
        else: x, y, r = e["x"], e["y"], e["r"]
        marks = ",".join(sorted(m["type"] for m in item.get("marks", []))) or "-"
        return "|".join([prefix, item["id"], kind, n(x), n(y), n(x1), n(y1), n(r), n(rx), n(ry), n(w), n(h), shape,
                         item["state"], "failed" if item.get("failed") else "-", marks, str(item["z"])])

    out = []
    for st in d["stages"]:
        N = st["number"]
        out.append("STAGE|%d|%s|%s" % (N, n(st["canvas"][0]), n(st["canvas"][1])))
        for item in st["elements"]:
            out.append(line("S%d" % N, item))
        for pi, panel in enumerate(st.get("panels", [])):
            out.append("PANEL|%d|%d|%s|%s|%s" % (N, pi, n(panel["x"]), n(panel["y"]), n(panel["scale"])))
            for item in panel["elements"]:
                out.append(line("S%dP%d" % (N, pi), item))
        for l in st["labels"]:
            out.append("|".join(["LABEL%d" % N, CORRECTIONS.get(l["text"], l["text"]), n(l["x"]), n(l["y"]), n(l["width"]), n(l["height"]), n(l["fontSize"]), l.get("colourRole", "accent")]))
        for l in st["legend"]:
            out.append("|".join(["LEGEND%d" % N, CORRECTIONS.get(l["text"], l["text"]), n(l["x"]), n(l["y"]), n(l["width"]), n(l["height"]), n(l["fontSize"]), "muted"]))
    return out


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument("--design")
    ap.add_argument("--out")
    args = ap.parse_args()
    work = args.out or tempfile.mkdtemp(prefix="ccf-diagram-check-")
    os.makedirs(work, exist_ok=True)
    tfm = "net%d.0" % int(subprocess.check_output(["dotnet", "--version"], text=True).split(".")[0])
    check = os.path.join(HERE, "TeachingDiagramPlanCheck.cs")
    with open(os.path.join(work, "check.csproj"), "w") as f:
        f.write(PROJECT.format(tfm=tfm, plan=PLAN, check=check))
    env = dict(os.environ, DOTNET_CLI_TELEMETRY_OPTOUT="1", DOTNET_NOLOGO="1")
    build = subprocess.run(["dotnet", "build", "-c", "Release", "-o", os.path.join(work, "bin"), "--nologo", "-v", "q"], cwd=work, env=env, capture_output=True, text=True)
    if build.returncode != 0:
        print(build.stdout[-3000:], build.stderr[-1000:]); sys.exit("BUILD FAILED")
    dll = os.path.join(work, "bin", "check.dll")
    run = subprocess.run(["dotnet", dll], cwd=work, env=env, capture_output=True, text=True)
    print(run.stdout.strip())
    code = run.returncode
    if args.design:
        dump = subprocess.run(["dotnet", dll, "dump"], cwd=work, env=env, capture_output=True, text=True).stdout.splitlines()
        want = canonical_from_design(args.design)
        have_set, want_set = set(dump), set(want)
        missing, extra = sorted(want_set - have_set), sorted(have_set - want_set)
        for l in missing[:12]: print("DESIGN_ONLY  " + l)
        for l in extra[:12]: print("PLAN_ONLY    " + l)
        ok = not missing and not extra
        print("DIAGRAM_DESIGN_MATCH_%s lines=%d" % ("PASS" if ok else "FAIL", len(want)))
        code = code or (0 if ok else 1)
    if not args.out:
        shutil.rmtree(work, ignore_errors=True)
    sys.exit(code)


if __name__ == "__main__":
    main()
