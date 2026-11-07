#!/usr/bin/env python3
"""Tests for the #328 Roslyn symbol-binding stage. Needs git, Python 3, .NET SDK 8.0.133 and the 320 pinned references.

Sections (each prints its own tally; the exit code is 0 only when every section is clean):
  A  rules       evaluate() on synthetic reporter output: every rule has a failing and a passing input (no .NET).
  B  semantic    real Roslyn over the checkout with source overlays: the #321 witnesses S05/S06/S07, the #326 tuple
                 and uncollected-file false greens, further rebinding negatives that must fail for their intended
                 semantic cause, and benign overlays that must stay accepted.
  C  fail-closed missing/corrupt references, wrong SDK, wrong compiler, no dotnet, bad topology, compile error,
                 tool crash, unreadable output. Each must be a completed ObjectiveCtaBindingError.
  D  end-to-end  the REAL check_baseline.py in a disposable clone: negatives exit 1 as 9 PASS/1 FAIL/2 NOT-TESTABLE
                 with only check 08 failing for the intended cause, benign overlays and the candidate exit 0 as
                 10/0/2, and the historical source stays 6/4/2.
Everything reads the checkout and writes only to operating-system temporary directories. The earlier
test_objective_cta_source_check.py suite (884 negatives / 112 benign) is separate and unchanged.

    python3 -B test_objective_cta_roslyn_binding.py [--skip-e2e]
"""
import copy
import json
import os
import re
import shutil
import subprocess
import sys
import tempfile
from pathlib import Path

import objective_cta_roslyn_binding as binding
from objective_cta_roslyn_binding import ObjectiveCtaBindingError
from objective_cta_source_check import ROOT as ROOT_FILE, UIKIT

REPO = Path(__file__).resolve().parents[3]
TOOLS = Path(__file__).resolve().parent
BASE = "72b4efbd1c653fa2231f632a79030a83ff76f833"  # historical source, still red
UI = "Assets/ForestPrototype/UI/"
ANCHOR_USING = "using UnityEngine.InputSystem;"
SHOW = "public void ShowObjectives()"
failures = []


def check(condition, label):
    if not condition:
        failures.append(label)
        print("  FAIL " + label)
    return condition


def live(path):
    return (REPO / path).read_text()


# ======================================================================== A: rules on synthetic output

def _symbol(kind, display, assembly="UnityEngine.CoreModule", metadata=True, containing=None, tuple_element=False):
    return {"kind": kind, "display": display, "containingType": containing, "containingAssembly": assembly,
            "isMetadata": metadata, "isTupleElement": tuple_element}


def _site(name, member, line=10):
    type_name, assembly, member_kind = binding.RECEIVERS[name]
    return {"file": ROOT_FILE, "line": line, "text": f"{name}.{member}", "receiverName": name, "member": member,
            "receiver": {"symbol": _symbol("NamedType", type_name, assembly), "candidateReason": "None", "candidates": []},
            "memberSymbol": {"symbol": _symbol(member_kind, type_name + "." + member, assembly, containing=type_name),
                             "candidateReason": "None", "candidates": []}}


def good_result():
    sites = [_site(n, m) for (n, m), count in binding.EXPECTED_SITES.items() for _ in range(count)]
    return {"label": "BASELINE", "fileCount": 3, "sourceSetSha256": "0" * 64, "errorCount": 0, "errors": [],
            "rootTypes": [{"display": binding.ROOT_TYPE, "kind": "Class", "declaringFiles": [ROOT_FILE],
                           "baseType": _symbol("NamedType", *binding.BASE_TYPE[:1], binding.BASE_TYPE[1]), "baseChain": []}],
            "sites": sites, "otherOccurrences": []}


def rules_section():
    print("A  rules on synthetic reporter output (no .NET)")
    good = good_result()
    check(binding.evaluate(good) == [], "A-00 the expected report has no unmet rule")
    cases = []

    def case(label, tag, mutate, text=None):
        result = copy.deepcopy(good)
        mutate(result)
        cases.append((label, tag, text, result))

    case("A-01 compile error", "[compile]", lambda r: r.update(errorCount=1, errors=[{"id": "CS0103", "file": "x.cs", "line": 1, "message": "m"}]))
    case("A-02 no root type", "[root]", lambda r: r.update(rootTypes=[]))
    case("A-03 two root types", "[root]", lambda r: r["rootTypes"].append(copy.deepcopy(r["rootTypes"][0])))
    case("A-04 root declared in another file", "[root]", lambda r: r["rootTypes"][0].update(declaringFiles=[ROOT_FILE, "Assets/X.cs"]))
    case("A-05 root in a namespace", "[root]", lambda r: r["rootTypes"][0].update(display="Foo.ScenarioOneUiRoot"))
    case("A-06 base type from source", "[base type]", lambda r: r["rootTypes"][0]["baseType"].update(isMetadata=False, containingAssembly="Assembly-CSharp", display="ZHiddenBase"))
    case("A-07 base type wrong metadata type", "[base type]", lambda r: r["rootTypes"][0]["baseType"].update(display="UnityEngine.Behaviour"))
    case("A-08 base type from another assembly", "[base type]", lambda r: r["rootTypes"][0]["baseType"].update(containingAssembly="Other"))
    case("A-09 Time receiver is a source type", "[receiver]", lambda r: r["sites"][0]["receiver"].update(symbol=_symbol("NamedType", "Time", "Assembly-CSharp", metadata=False)))
    case("A-10 Time receiver is a tuple field", "[receiver]", lambda r: r["sites"][0]["receiver"].update(symbol=_symbol("Field", "ScenarioOneUiRoot.Time", "Assembly-CSharp", metadata=False)))
    case("A-11 Time receiver unresolved", "unresolved or ambiguous", lambda r: r["sites"][0]["receiver"].update(symbol=None, candidateReason="Ambiguous"))
    case("A-12 DisplayStyle receiver is a source enum", "[receiver]", lambda r: r["sites"][-1]["receiver"].update(symbol=_symbol("NamedType", "DisplayStyle", "Assembly-CSharp", metadata=False)))
    case("A-13 DisplayStyle receiver is the wrong Unity type", "[receiver]", lambda r: r["sites"][-1]["receiver"]["symbol"].update(display="UnityEngine.UIElements.Visibility"))
    case("A-14 member is a tuple element", "[member]", lambda r: r["sites"][0]["memberSymbol"]["symbol"].update(kind="Field", isTupleElement=True))
    case("A-15 member is unresolved", "[member]", lambda r: r["sites"][0].update(memberSymbol={"symbol": None, "candidateReason": "None", "candidates": []}))
    case("A-16 DisplayStyle member is a property", "[member]", lambda r: r["sites"][-1]["memberSymbol"]["symbol"].update(kind="Property"))
    case("A-17 a site is missing", "[site count]", lambda r: r["sites"].pop(0))
    case("A-18 an extra pinned site", "[site count]", lambda r: r["sites"].append(_site("Time", "frameCount")))
    case("A-19 an unpinned member on a reviewed receiver", "[site count]", lambda r: r["sites"].append(_site("Time", "frameCount") | {"member": "deltaTime"}))
    case("A-20 identifier used outside a member access", "[site]", lambda r: r["otherOccurrences"].append(
        {"file": ROOT_FILE, "line": 1, "name": "Time", "parentKind": "Argument", "text": "f(Time)"}))
    for label, tag, text, result in cases:
        problems = binding.evaluate(result)
        check(problems and any(tag in p for p in problems), f"{label}: expected a {tag} problem, got {problems[:2]}")
    print(f"   rules: {len(cases)} negatives each rejected for its tag; the expected report accepted")


# ================================================================================ B: semantic overlays

time_stub = ("\ninternal static class Time { public static int frameCount => -2; public static float unscaledTime => 0f; "
             "public static float deltaTime => 0f; public static float unscaledDeltaTime => 0f; public static float time => 0f; "
             "public static float realtimeSinceStartup => 0f; public static double realtimeSinceStartupAsDouble => 0d; "
             "public static float fixedDeltaTime => 0f; public static float timeScale { get; set; } }\n")
hidden = "\npublic abstract class ZHiddenBase : UnityEngine.MonoBehaviour { protected static class Time { public static int frameCount => -2; public static float unscaledTime => 0f; } }\n"
clock_host = ("\npublic static class ZClockHost { public static ZFrameSource Time => default; }\n"
              "public struct ZFrameSource { public int frameCount => -2; public float unscaledTime => 0f; }\n")
style_host = ("\npublic static class ZStyleHost { public static ZSwappedStyle DisplayStyle => default; }\n"
              "public struct ZSwappedStyle { public UnityEngine.UIElements.DisplayStyle Flex => UnityEngine.UIElements.DisplayStyle.None; "
              "public UnityEngine.UIElements.DisplayStyle None => UnityEngine.UIElements.DisplayStyle.None; }\n")


def semantic_cases():
    """id -> dict(kind, edits, expect, e2e). kind: witness | neg | benign. `expect`: substrings that must all appear in the
    joined problems for a negative. `e2e`: the full-checker expectation, ('lexical', text) or ('semantic', text) for
    negatives or None to skip it."""
    kit, root = live(UIKIT), live(ROOT_FILE)
    cases = {}

    def add(cid, kind, why, *edits, expect=(), e2e=None):
        cases[cid] = dict(kind=kind, why=why, edits=list(edits), expect=list(expect), e2e=e2e)

    # exact #321 witnesses: the lexical stage rejects them first (cause preserved); Roslyn must reject them on its own too
    add("R321-S05", "witness", "global Time class in the UiKit companion", (UIKIT, kit, kit + time_stub),
        expect=["[receiver]", "receiver `Time` binds to NamedType Time (source in this project)"],
        e2e=("lexical", "collected companion type 'Time' redefines a reviewed root identifier"))
    add("R321-S06", "witness", "MonoBehaviour alias to a base that hides Time",
        (ROOT_FILE, ANCHOR_USING, ANCHOR_USING + "\nusing MonoBehaviour = ZHiddenBase;"),
        (UI + "ZHiddenBase.cs", None, "// ScenarioOneUiRoot: all files in this test are included companions.\npublic abstract class ZHiddenBase : UnityEngine.MonoBehaviour { protected static class Time { public static int frameCount => -2; public static float unscaledTime => 0f; } }"),
        expect=["[base type]", "derives from ZHiddenBase", "[receiver]"],
        e2e=("lexical", "using alias 'MonoBehaviour' shadows a name the reviewed text reads"))
    add("R321-S07", "witness", "MonoBehaviour alias, base declared in UiKit",
        (ROOT_FILE, ANCHOR_USING, ANCHOR_USING + "\nusing MonoBehaviour = ZHiddenBase;"), (UIKIT, kit, kit + hidden),
        expect=["[base type]", "derives from ZHiddenBase", "[receiver]"],
        e2e=("lexical", "using alias 'MonoBehaviour' shadows a name the reviewed text reads"))
    # #326 false greens: the lexical stage accepts these, only Roslyn can reject them
    add("A326-N06", "neg", "root tuple-typed FIELD named Time",
        (ROOT_FILE, SHOW, "private (int frameCount, float unscaledTime) Time = (-2, 0f);\n    " + SHOW),
        expect=["[receiver]", "receiver `Time` binds to Field ScenarioOneUiRoot.Time (source in this project)"],
        e2e=("semantic", "receiver `Time` binds to Field ScenarioOneUiRoot.Time"))
    add("A326-N07", "neg", "root tuple-typed PROPERTY named Time",
        (ROOT_FILE, SHOW, "private (int frameCount, float unscaledTime) Time => (-2, 0f);\n    " + SHOW),
        expect=["[receiver]", "receiver `Time` binds to Property ScenarioOneUiRoot.Time (source in this project)"],
        e2e=("semantic", "receiver `Time` binds to Property ScenarioOneUiRoot.Time"))
    add("A326-N08", "neg", "root tuple-typed field named DisplayStyle whose Flex is None",
        (ROOT_FILE, SHOW, "private static readonly (UnityEngine.UIElements.DisplayStyle Flex, UnityEngine.UIElements.DisplayStyle None) DisplayStyle = "
                          "(UnityEngine.UIElements.DisplayStyle.None, UnityEngine.UIElements.DisplayStyle.None);\n    " + SHOW),
        expect=["[receiver]", "receiver `DisplayStyle` binds to Field ScenarioOneUiRoot.DisplayStyle (source in this project)"],
        e2e=("semantic", "receiver `DisplayStyle` binds to Field ScenarioOneUiRoot.DisplayStyle"))
    add("L326-U01", "neg", "global Time class in a NEW file with no companion hint text (never collected)",
        ("Assets/ForestPrototype/ZClock.cs", None, time_stub.lstrip("\n")),
        expect=["[receiver]", "receiver `Time` binds to NamedType Time (source in this project)"],
        e2e=("semantic", "receiver `Time` binds to NamedType Time (source in this project)"))
    # further rebinding routes, semantic stage only (the lexical stage may or may not see them)
    add("X328-N01", "neg", "alias @MonoBehaviour to a source base", (ROOT_FILE, ANCHOR_USING, ANCHOR_USING + "\nusing  @MonoBehaviour  =  ZOtherBase ;"),
        (UIKIT, kit, kit + hidden.replace("ZHiddenBase", "ZOtherBase")), expect=["[base type]", "derives from ZOtherBase"])
    add("X328-N02", "neg", "source type UnityEngine.Time in a UiKit namespace block (source beats metadata)",
        (UIKIT, kit, kit + "\nnamespace UnityEngine { " + time_stub.replace("internal static class", "public static class") + " }\n"),
        expect=["[receiver]", "source in this project"])
    add("X328-N03", "neg", "global enum DisplayStyle with swapped values in UiKit", (UIKIT, kit, kit + "\npublic enum DisplayStyle { None = 0, Flex = 1 }\n"),
        expect=["[receiver]", "receiver `DisplayStyle` binds to NamedType DisplayStyle (source in this project)"])
    add("X328-N04", "neg", "using static imports a Time property", (ROOT_FILE, ANCHOR_USING, ANCHOR_USING + "\nusing static ZClockHost;"),
        (UIKIT, kit, kit + clock_host), expect=["[compile]", "CS0229", "unresolved or ambiguous"])
    add("X328-N05", "neg", "using static imports a DisplayStyle property", (ROOT_FILE, ANCHOR_USING, ANCHOR_USING + "\nusing static ZStyleHost;"),
        (UIKIT, kit, kit + style_host), expect=["[compile]", "CS0229", "unresolved or ambiguous"])
    add("X328-N06", "neg", "root alias Time to a source class", (ROOT_FILE, ANCHOR_USING, ANCHOR_USING + "\nusing Time = ZFakeTime;"),
        ("Assets/ForestPrototype/ZFakeTime.cs", None, "public static class ZFakeTime { public static int frameCount => -2; public static float unscaledTime => 0f; }\n"),
        expect=["[receiver]", "source in this project"])
    add("X328-N07", "neg", "root alias DisplayStyle to a source enum", (ROOT_FILE, ANCHOR_USING, ANCHOR_USING + "\nusing DisplayStyle = ZFakeStyle;"),
        ("Assets/ForestPrototype/ZFakeStyle.cs", None, "public enum ZFakeStyle { None = 0, Flex = 1 }\n"),
        expect=["[receiver]", "source in this project"])
    add("X328-N08", "neg", "nested type Time inside the root", (ROOT_FILE, SHOW, "private static class Time { public static int frameCount => -2; public static float unscaledTime => 0f; }\n    " + SHOW),
        expect=["[receiver]", "source in this project"])
    add("X328-N09", "neg", "a second type named ScenarioOneUiRoot in a namespace",
        ("Assets/ForestPrototype/ZSecondRoot.cs", None, "namespace Foo { public class ScenarioOneUiRoot : UnityEngine.MonoBehaviour { } }\n"), expect=["[root]", "found 2"])
    add("X328-N10", "neg", "Time ambiguous between two imported namespaces and UnityEngine",
        (ROOT_FILE, ANCHOR_USING, ANCHOR_USING + "\nusing ZA;\nusing ZB;"),
        ("Assets/ForestPrototype/ZAmbiguous.cs", None, "namespace ZA { public static class Time { public static int frameCount => 1; public static float unscaledTime => 1f; } }\nnamespace ZB { public static class Time { public static int frameCount => 2; public static float unscaledTime => 2f; } }\n"),
        expect=["[compile]", "CS0104", "unresolved or ambiguous"])
    add("X328-N11", "neg", "a reviewed Time.frameCount site is replaced (silent missing site)",
        (ROOT_FILE, "helpClosedFrame = Time.frameCount;", "helpClosedFrame = 0;"), expect=["[site count]", "Time.frameCount: Roslyn found 2", "expected 3"])
    add("X328-N12", "neg", "a new Time.deltaTime site appears", (ROOT_FILE, SHOW, "private float ZDelta() { return Time.deltaTime; }\n    " + SHOW),
        expect=["[site count]", "Time.deltaTime: Roslyn found 1", "expected 0"])
    add("X328-N13", "neg", "the source set stops compiling", (ROOT_FILE, SHOW, "private int ZBad = \"x\";\n    " + SHOW), expect=["[compile]", "CS0029"])
    # benign overlays: valid C# that must not be rejected
    add("A326-B01", "benign", "UiKit-local alias to the real Time", (UIKIT, "using System;", "using System;\nusing Z = UnityEngine.Time;"), e2e=("accept", None))
    add("A326-B03", "benign", "unrelated global types in UiKit", (UIKIT, kit, kit + clock_host), e2e=("accept", None))
    add("A326-B04", "benign", "hinted companion with an unrelated nested class Clock",
        (UI + "ZNotes.cs", None, "// ScenarioOneUiRoot\ninternal static class ZNotes { internal static class Clock { internal static int Frame => 0; } }\n"), e2e=("accept", None))
    add("X328-B05", "benign", "root tuple member with an unrelated name",
        (ROOT_FILE, SHOW, "private (int frameCount, float unscaledTime) Clock = (0, 0f);\n    " + SHOW), e2e=("accept", None))
    add("X328-B06", "benign", "an unrelated new file declaring a type ZClock with no reviewed name",
        ("Assets/ForestPrototype/ZClock.cs", None, "internal static class ZClock { public static int frameCount => -2; public static float unscaledTime => 0f; }\n"), e2e=("accept", None))
    add("X328-B07", "benign", "fully qualified UnityEngine.Time at one reviewed site",
        (ROOT_FILE, "helpClosedFrame = Time.frameCount;", "helpClosedFrame = UnityEngine.Time.frameCount;"))
    add("X328-B08", "benign", "root alias Time to the real UnityEngine.Time", (ROOT_FILE, ANCHOR_USING, ANCHOR_USING + "\nusing Time = UnityEngine.Time;"))
    add("X328-B09", "benign", "root using static UnityEngine.Mathf", (ROOT_FILE, ANCHOR_USING, ANCHOR_USING + "\nusing static UnityEngine.Mathf;"))
    return cases


def apply_case(case):
    files = {}
    for path, before, after in case["edits"]:
        if before is None:
            files[path] = after
            continue
        current = files.get(path, live(path))
        if current.count(before) != 1:
            raise RuntimeError(f"fixture anchor not unique in {path}: {before!r} ({current.count(before)})")
        files[path] = current.replace(before, after, 1)
    return files


_CHAIN = {}


def toolchain():
    if "chain" not in _CHAIN:
        _CHAIN["work"] = tempfile.mkdtemp(prefix="ccf-cta-roslyn-test-")
        _CHAIN["chain"] = binding.build_toolchain(_CHAIN["work"])
    return _CHAIN["chain"]


def semantic_section(cases):
    print("B  semantic overlays (real Roslyn, C# 9, 320 pinned references)")
    chain = toolchain()
    with tempfile.TemporaryDirectory(prefix="ccf-cta-roslyn-src-") as src, tempfile.TemporaryDirectory(prefix="ccf-cta-roslyn-ovl-") as ovl:
        binding.materialize_source_set(REPO, src)
        for cid, case in cases.items():
            for path, text in apply_case(case).items():
                target = Path(ovl) / cid / path
                target.parent.mkdir(parents=True, exist_ok=True)
                target.write_text(text, newline="")
        document = chain.report(src, ovl)
    results = {r["label"]: r for r in document["results"]}
    base = results["BASELINE"]
    check(binding.evaluate(base) == [], "B-00 the checkout itself meets every semantic rule: " + str(binding.evaluate(base)[:2]))
    check(base["errorCount"] == 0 and base["fileCount"] > 100, "B-01 the checkout compiles with 0 errors under the pinned references")
    negatives = benign = 0
    for cid, case in cases.items():
        problems = binding.evaluate(results[cid])
        text = " | ".join(problems)
        if case["kind"] == "benign":
            benign += check(not problems, f"B {cid} benign overlay must stay accepted: {problems[:2]}")
        else:
            negatives += check(bool(problems) and all(e in text for e in case["expect"]),
                               f"B {cid} must be rejected for {case['expect']}; got {text[:400]!r}")
    print(f"   semantic: {negatives} negatives rejected for their intended cause; {benign} benign overlays accepted")
    return document


# ==================================================================================== C: fail closed

def expect_error(label, call, text):
    try:
        call()
    except ObjectiveCtaBindingError as error:
        return check(text in str(error), f"{label}: error lacked {text!r}: {str(error)[:200]}")
    except Exception as error:  # noqa: BLE001
        return check(False, f"{label}: raised {type(error).__name__} instead of a completed ObjectiveCtaBindingError")
    return check(False, f"{label}: tooling degradation was accepted")


def reference_source():
    return Path(os.environ.get(binding.REFERENCE_DIR_ENV) or binding.DEFAULT_REFERENCE_DIR)


def fail_closed_section():
    print("C  fail-closed degradations")
    done = 0
    source = reference_source()
    with tempfile.TemporaryDirectory(prefix="ccf-cta-roslyn-fc-") as temp:
        temp = Path(temp)
        # references: every kind of damage to the pinned set is a completed failure
        done += expect_error("C-01 reference folder missing", lambda: binding.reference_files(temp / "nope"), "reference folder missing")
        refs = temp / "refs"
        shutil.copytree(source, refs)
        done += check(len(binding.reference_files(refs)) == 320, "C-02 an exact copy of the references verifies 320/320")
        names = sorted(os.listdir(refs))
        victim = refs / names[7]
        original = victim.read_bytes()
        damaged = bytearray(original)
        damaged[len(damaged) // 2] ^= 0xFF
        victim.write_bytes(bytes(damaged))
        done += expect_error("C-03 one corrupted reference byte", lambda: binding.reference_files(refs), "reference hash mismatch")
        victim.write_bytes(original)
        gone = refs / names[3]
        kept = gone.read_bytes()
        gone.unlink()
        done += expect_error("C-04 one reference missing", lambda: binding.reference_files(refs), "missing or not a regular file")
        gone.write_bytes(kept)
        (refs / "999-extra.dll").write_bytes(b"x")
        done += expect_error("C-05 an unlisted extra file in the reference folder", lambda: binding.reference_files(refs), "outside the manifest")
        (refs / "999-extra.dll").unlink()
        done += check(len(binding.reference_files(refs)) == 320, "C-06 the repaired copy verifies again")
        # the manifest itself is pinned
        original_manifest = binding.MANIFEST
        tampered = temp / "manifest.txt"
        tampered.write_text(original_manifest.read_text().replace("79a49049", "79a49048", 1))
        binding.MANIFEST = tampered
        try:
            done += expect_error("C-07 manifest differs from its pinned hash", lambda: binding.reference_files(refs), "reference manifest hash")
        finally:
            binding.MANIFEST = original_manifest
        # SDK and compiler pins, real dotnet
        for name in ("sdk", "compiler", "nodotnet"):
            (temp / name).mkdir()
        done += expect_error("C-08 wrong SDK version", lambda: binding.build_toolchain(temp / "sdk", reference_dir=refs, sdk_version="8.0.999"), "is required")
        done += expect_error("C-09 wrong Roslyn compiler hash", lambda: binding.build_toolchain(temp / "compiler", reference_dir=refs,
                             roslyn_hashes={"Microsoft.CodeAnalysis.dll": "0" * 64}), "differs from the pinned SHA256")
        path_before = os.environ["PATH"]
        os.environ["PATH"] = str(temp / "nodotnet")
        try:
            done += expect_error("C-10 no dotnet on PATH", lambda: binding.verify_objective_cta_binding(REPO, reference_dir=refs), "could not run")
        finally:
            os.environ["PATH"] = path_before
        # topology, no .NET needed
        tree = temp / "tree"
        for rel in ("Assets/Game/Mod.asmdef", "Assets/Plugins/First.cs", "Assets/Game/Editor/E.asmdef", "Assets/Game/Editor/Tool.cs",
                    "Assets/Game/Ok.cs", "Assets/.Hidden/Skip.asmdef"):
            (tree / rel).parent.mkdir(parents=True, exist_ok=True)
            (tree / rel).write_text("")
        found = binding.topology_problems(tree)
        done += check(len(found) == 2 and any("Mod.asmdef" in f for f in found) and any("Plugins/First.cs" in f for f in found),
                      f"C-11 asmdef and first-pass folder refused, Editor and dot folders ignored: {found}")
        done += expect_error("C-12 topology refused by the public entry point", lambda: binding.verify_objective_cta_binding(tree, reference_dir=refs), "[topology]")
        # tool failures with the real toolchain
        chain = toolchain()
        done += expect_error("C-13 reporter crash on a missing source root", lambda: chain.report(temp / "no-such-root"), "exited 3")
        sub = temp / "empty" / "Assets"
        sub.mkdir(parents=True)
        (sub / "A.cs").write_text('class A { void M() { int x = "s"; } }\n')
        result = chain.report(temp / "empty")["results"][0]
        problems = binding.evaluate(result)
        done += check(result["errorCount"] >= 1 and any("[compile]" in p for p in problems), "C-14 compile errors are reported and rejected")
        done += check(any("[root]" in p for p in problems), "C-15 a tree with no ScenarioOneUiRoot is rejected")
        # unreadable output: the reporter claims success but writes nothing / garbage
        real_run = binding._run
        binding._run = lambda *a, **k: subprocess.CompletedProcess(a[0], 0, "", "")
        try:
            done += expect_error("C-16 reporter wrote no output", lambda: chain.report(REPO), "output unreadable")
            def garbage(command, *args, **kwargs):
                Path(command[5]).write_text("not json")  # [dotnet, dll, source, refs, defines, OUT]
                return subprocess.CompletedProcess(command, 0, "", "")
            binding._run = garbage
            done += expect_error("C-17 reporter output is not JSON", lambda: chain.report(REPO), "output unreadable")
        finally:
            binding._run = real_run
        # the public entry point reports an unexpected exception as a completed failure, not a crash
        done += expect_error("C-18 public entry point wraps an unexpected exception", lambda: binding.verify_objective_cta_binding(REPO, toolchain=object()), "could not run")
    print(f"   fail-closed: {done} degradation checks completed")


# ==================================================================================== D: end to end

CHECKER = "Tools/Verification/OvernightPlaytestFeedback/check_baseline.py"
TOOL_FILES = ("objective_cta_source_check.py", "check_baseline.py", "objective_cta_roslyn_binding.py", "ObjectiveCtaRoslynBinding.cs",
              "roslyn-reference-manifest.txt")


def git(clone, *args):
    return subprocess.run(["git", "-C", str(clone), *args], capture_output=True, text=True, check=True).stdout


def run_checker(clone, tmp, state="post-fix"):
    env = dict(os.environ, PYTHONDONTWRITEBYTECODE="1", TMPDIR=str(tmp), DOTNET_CLI_TELEMETRY_OPTOUT="1", DOTNET_NOLOGO="1",
               UseSharedCompilation="false", MSBUILDDISABLENODEREUSE="1")
    done = subprocess.run([sys.executable, "-B", str(clone / CHECKER), "--rotation-state", state, "--instruction-state", state],
                          cwd=clone, env=env, capture_output=True, text=True, timeout=900)
    tally = re.search(r"SOURCE-STATIC TALLY PASS=(\d+) FAIL=(\d+) NOT-TESTABLE=(\d+)", done.stdout)
    failed = re.findall(r"^SOURCE-STATIC FAIL (\S+)", done.stdout, re.M)
    line = re.search(r"^SOURCE-STATIC (?:PASS|FAIL) 08 objective-primary-cta: (.*)$", done.stdout, re.M)
    return dict(exit=done.returncode, tally=tuple(int(x) for x in tally.groups()) if tally else None, failed=failed,
                text=line.group(1) if line else "", stdout=done.stdout)


def e2e_section(cases):
    print("D  end-to-end: the real check_baseline.py in a disposable clone")
    with tempfile.TemporaryDirectory(prefix="ccf-cta-roslyn-e2e-") as directory:
        clone, tmp = Path(directory) / "clone", Path(directory) / "tmp"
        tmp.mkdir()
        head = git(REPO, "rev-parse", "HEAD").strip()
        git(REPO, "diff", "--quiet", "HEAD", "--", "Assets")  # the Assets under test must equal HEAD
        subprocess.run(["git", "clone", "--shared", "--no-checkout", "-q", str(REPO), str(clone)], check=True)
        git(clone, "sparse-checkout", "set", "--no-cone", "/Assets/**/*.cs", "/Tools/Verification/OvernightPlaytestFeedback/")
        git(clone, "checkout", "--detach", "-q", head)
        for name in TOOL_FILES:  # the tool files under test (committed or not)
            shutil.copyfile(TOOLS / name, clone / "Tools/Verification/OvernightPlaytestFeedback" / name)
        pristine = git(clone, "status", "--porcelain", "-uall")

        def restore(added):
            for path in added:
                path.unlink()
            git(clone, "checkout", "--", "Assets")

        def normal(label, expect_exit, expect_tally, expect_failed, contains=""):
            r = run_checker(clone, tmp)
            ok = (r["exit"], r["tally"], r["failed"]) == (expect_exit, expect_tally, expect_failed) and contains in r["text"]
            check(ok, f"D {label}: exit={r['exit']} tally={r['tally']} failed={r['failed']} check08={r['text'][:260]!r}")
            return ok

        good = normal("candidate", 0, (10, 0, 2), [])
        negatives = benign = 0
        for cid, case in cases.items():
            if not case["e2e"]:
                continue
            added = []
            for path, text in apply_case(case).items():
                target = clone / path
                if not target.exists():
                    added.append(target)
                target.write_text(text, newline="")
            try:
                if case["e2e"][0] == "accept":
                    benign += normal(cid, 0, (10, 0, 2), [])
                else:
                    stage, cause = case["e2e"]
                    r = run_checker(clone, tmp)
                    semantic = "Roslyn semantic binding unmet" in r["text"]
                    ok = (r["exit"], r["tally"], r["failed"]) == (1, (9, 1, 2), ["08"]) and cause in r["text"] and semantic == (stage == "semantic")
                    negatives += check(ok, f"D {cid} ({stage}): exit={r['exit']} tally={r['tally']} failed={r['failed']} semantic={semantic} check08={r['text'][:300]!r}")
            finally:
                restore(added)
            check(git(clone, "status", "--porcelain", "-uall") == pristine, f"D {cid}: clone restored")
        # historical source: still red, and the Roslyn stage is never reached
        git(clone, "checkout", "-f", "--detach", "-q", BASE)
        (clone / "Tools/Verification/OvernightPlaytestFeedback").mkdir(parents=True, exist_ok=True)  # absent at BASE
        for name in TOOL_FILES:  # BASE has older tool files; run the tool under test against the old source
            shutil.copyfile(TOOLS / name, clone / "Tools/Verification/OvernightPlaytestFeedback" / name)
        r = run_checker(clone, tmp, "pre-fix")  # the recorded historical red uses the pre-fix states
        check((r["exit"], r["tally"]) == (1, (6, 4, 2)) and "Roslyn" not in r["stdout"], f"D historical {BASE[:8]}: exit={r['exit']} tally={r['tally']}")
        print(f"   end-to-end: candidate {'10/0/2 exit 0' if good else 'NOT clean'}; {negatives} negatives rejected for their intended cause "
              f"(9/1/2, only check 08); {benign} benign overlays 10/0/2; historical 6/4/2")


def run():
    skip_e2e = "--skip-e2e" in sys.argv
    rules_section()
    cases = semantic_cases()
    semantic_section(cases)
    fail_closed_section()
    if not skip_e2e:
        e2e_section(cases)
    if _CHAIN.get("work"):
        shutil.rmtree(_CHAIN["work"], ignore_errors=True)
    print(("FAILED: " + str(len(failures)) + " check(s)") if failures else "ALL CHECKS PASSED")
    return 1 if failures else 0


if __name__ == "__main__":
    raise SystemExit(run())
