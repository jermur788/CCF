#!/usr/bin/env python3
"""Integrity test for the #306 stage-3 inventory guard in run_five_rack_check.py.

Runs the real checker end to end in a disposable sparse clone OUTSIDE the repository. First it checks that an unmutated
clone passes in full. Then, for each mutation, it resets the clone, applies one forbidden change and requires the checker
to exit 1 with exactly the intended assertion. A mutation that passes, or that is caught by a different assertion, fails
this test. The guard must reject each change at the rule written for it, not by luck.

The mutations cover the #303 stage boundary (the two pinned verifier scripts, the historical handoff, an injected StandMap
path and a same-named script in a new subdirectory), the #296 source/scope boundary (diagram, Objective call site, map,
rack, save, scene, Packages, ProjectSettings) and the checker's own inventory. A weakened copy of the checker can be
passed with --checker; the handoff records those false-green controls separately.

Never edits the checkout it is run from. The clone borrows objects from the shared store (git clone --shared) and is
deleted at the end. Logs stay under --work.
"""
import argparse
from pathlib import Path
import shutil
import subprocess
import sys

HERE = Path(__file__).resolve().parent
ROOT = HERE.parents[2]
CHECKER_IN_REPO = Path("Tools/Verification/ScenarioOneFiveRackDiagram/run_five_rack_check.py")
SPARSE = ("Assets", "Docs", "Tools", "Packages", "ProjectSettings")
FAIL_PREFIX = "FAIL SOURCE "
STAGE3 = "stage 3: "
HEADLAND = "Tools/Verification/StandMap/HeadlandOffline/run_headland_check.py"
STAGE_TEST = "Tools/Verification/StandMap/HeadlandOffline/test_stage_inventory_guard.py"
STAGE_HANDOFF = ("Docs/Handoffs/ScenarioOne-Overnight-Playtest-Readiness-20261106/"
                 "D072-HEADLAND-STAGE-INVENTORY-CHECKER-REPAIR-HANDOFF.md")
DIAGRAM = "Assets/ForestPrototype/UI/TeachingDiagram.cs"
OBJECTIVES = "Assets/ForestPrototype/UI/LearningObjectivesView.cs"
MAP = "Assets/ForestPrototype/UI/Map/StandMapSource.cs"
RACK = "Assets/ForestPrototype/Racks/RackGeometry.cs"
SAVE = "Assets/ForestPrototype/ForestSaveData.cs"
SCENE = "Assets/Scenes/ForestTest.unity"
PACKAGES = "Packages/manifest.json"
SETTINGS = "ProjectSettings/EditorBuildSettings.asset"
CALL_OLD = ("stage = Plan.ShowsFiveRackFigure(lesson, rackV2World) ? "
            "Plan.ObjectiveOneFigure(SiteFromAuthority()) : Plan.For(lesson);")
CALL_NEW = "stage = lesson == 0 ? Plan.ObjectiveOneFigure(SiteFromAuthority()) : Plan.For(lesson);"


def git(root, *arguments):
    return subprocess.run(["git", "-C", str(root), *arguments], check=True, capture_output=True, text=True).stdout


def append(path, text):
    with open(path, "a") as handle:
        handle.write(text)


def nudge(path):
    """Smallest content change: one extra space after the first ';' past the file midpoint."""
    text = path.read_text()
    position = text.index(";", len(text) // 2) + 1
    path.write_text(text[:position] + " " + text[position:])


def replace_once(path, old, new):
    text = path.read_text()
    if text.count(old) != 1:
        raise RuntimeError(f"mutation setup: expected one occurrence in {path}")
    path.write_text(text.replace(old, new))


# (case, mutation(root), exact assertion that must reject it)
MUTATIONS = [
    ("303a headland checker altered by one space", lambda r: nudge(r / HEADLAND),
     STAGE3 + HEADLAND + " is byte-identical to pinned #303 commit"),
    ("303b stage-inventory test altered by one space", lambda r: nudge(r / STAGE_TEST),
     STAGE3 + STAGE_TEST + " is byte-identical to pinned #303 commit"),
    ("303c historical handoff altered by one newline", lambda r: append(r / STAGE_HANDOFF, "\n"),
     STAGE3 + STAGE_HANDOFF + " is byte-identical to pinned #303 commit"),
    ("303d extra .py injected under StandMap",
     lambda r: (r / "Tools/Verification/StandMap/HeadlandOffline/extra.py").write_text("# unapproved\n"),
     STAGE3 + "no Stand Map path outside the two pinned #303 verifier scripts changed"),
    ("303e same-named StandMap script in a new subdirectory",
     lambda r: ((r / "Tools/Verification/StandMap/Unapproved").mkdir(parents=True, exist_ok=True),
                (r / "Tools/Verification/StandMap/Unapproved/run_headland_check.py").write_text("# unapproved\n")),
     STAGE3 + "no Stand Map path outside the two pinned #303 verifier scripts changed"),
    ("303f extra Markdown beside the #303 handoff",
     lambda r: (r / "Docs/Handoffs/ScenarioOne-Overnight-Playtest-Readiness-20261106/D072-UNREVIEWED.md")
     .write_text("# unapproved\n"),
     "only allowed files changed since "),
    ("1a unauthorised rack edit", lambda r: append(r / RACK, "// mutation\n"),
     "only allowed files changed since "),
    ("1b unauthorised save edit", lambda r: nudge(r / SAVE), "only allowed files changed since "),
    ("1c unauthorised scene edit", lambda r: append(r / SCENE, "\n"), "only allowed files changed since "),
    ("1d unauthorised Packages edit", lambda r: append(r / PACKAGES, "\n"),
     "no package or project-settings change"),
    ("1e unauthorised ProjectSettings edit", lambda r: append(r / SETTINGS, "\n"),
     "no package or project-settings change"),
    ("2a unauthorised diagram selection edit", lambda r: replace_once(r / DIAGRAM, CALL_OLD, CALL_NEW),
     "the painter chooses the five-rack figure only through Plan.ShowsFiveRackFigure"),
    ("2b unauthorised Objective edit", lambda r: nudge(r / OBJECTIVES),
     "LearningObjectivesView.cs differs from base only"),
    ("2c unauthorised map edit", lambda r: nudge(r / MAP), "only allowed files changed since "),
]


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--checker", type=Path, default=HERE / "run_five_rack_check.py", help="checker file under test")
    parser.add_argument("--work", type=Path,
                        default=Path("/media/jer/Files/CCF/Verification/D072-FiveRackStageInventory-20261106/integrity-work"))
    parser.add_argument("--commit", default=None, help="commit to clone (default: HEAD of this checkout)")
    parser.add_argument("--skip-clean", action="store_true",
                        help="test-only: skip the two clean runs (used for weakened-checker controls; the default run keeps them)")
    arguments = parser.parse_args()
    work = arguments.work.resolve()
    if work.is_relative_to(ROOT) or ROOT.is_relative_to(work):
        raise SystemExit("work directory must be outside the repository")
    checker = arguments.checker.resolve().read_bytes()
    commit = arguments.commit or git(ROOT, "rev-parse", "HEAD").strip()
    store = Path(git(ROOT, "rev-parse", "--path-format=absolute", "--git-common-dir").strip())
    clone = work / "clone"
    logs = work / "logs"
    out = work / "out"
    if clone.exists():
        shutil.rmtree(clone)
    if out.exists():
        shutil.rmtree(out)
    logs.mkdir(parents=True, exist_ok=True)
    subprocess.run(["git", "clone", "-q", "--shared", "--no-checkout", str(store), str(clone)], check=True)
    git(clone, "sparse-checkout", "set", "--cone", *SPARSE)
    git(clone, "checkout", "-q", "--detach", commit)
    print(f"CLONE {clone} COMMIT {commit} CHECKER {arguments.checker.resolve()}", flush=True)

    def reset():
        git(clone, "reset", "-q", "--hard", commit)
        git(clone, "clean", "-q", "-fdx", "--", *SPARSE)
        (clone / CHECKER_IN_REPO).write_bytes(checker)

    def run(name, mode):
        arguments = [sys.executable, "-I", "-B", str(clone / CHECKER_IN_REPO), "--out", str(out / name)]
        if mode == "scope-only":
            arguments.append("--scope-only")
        elif mode == "no-mutations":
            arguments.append("--no-mutations")
        result = subprocess.run(arguments, capture_output=True, text=True)
        (logs / (name + ".log")).write_text(f"exit={result.returncode}\n--- stdout\n{result.stdout}--- stderr\n{result.stderr}")
        return result

    failures = []

    def clean_run(label, mode):
        reset()
        result = run(label, mode)
        source = sum(line.startswith("PASS SOURCE ") for line in result.stdout.splitlines())
        floor = 131 if mode == "full" else 70
        ok = (result.returncode == 0 and "FIVE_RACK_SUITE_PASS" in result.stdout and "failed=0" in result.stdout
              and source >= floor and "FIVE_RACK_PASS" in result.stdout and "FIVE_RACK_PAINTER_PASS" in result.stdout)
        print(f"{'PASS' if ok else 'FAIL'} CLEAN {label}: exit={result.returncode} PASS_SOURCE={source} "
              f"{'full+mutations' if mode == 'full' else 'scope+substantive'}", flush=True)
        if not ok:
            failures.append(label)

    if not arguments.skip_clean:
        clean_run("clean-before", "full")
    for case, mutate, expected in MUTATIONS:
        reset()
        mutate(clone)
        name = "mutation-" + case.split()[0]
        result = run(name, "scope-only")
        failed = [line for line in result.stdout.splitlines() if line.startswith(FAIL_PREFIX)]
        hit = [line for line in failed if expected in line]
        if result.returncode == 1 and hit:
            print(f"PASS CAUGHT {case}\n    {hit[0]}", flush=True)
        else:
            verdict = "NOT CAUGHT" if result.returncode == 0 else "CAUGHT BY WRONG ASSERTION"
            print(f"FAIL {verdict} {case}\n    exit={result.returncode} {(failed or ['(checker passed)'])[0]}\n"
                  f"    expected: {expected}", flush=True)
            failures.append(case)
    if not arguments.skip_clean:
        clean_run("clean-after", "no-mutations")
    shutil.rmtree(clone)
    clean_count = 0 if arguments.skip_clean else 2
    print(f"STAGE_INVENTORY_INTEGRITY_TALLY MUTATIONS={len(MUTATIONS)} CLEAN_RUNS={clean_count} FAIL={len(failures)}")
    if failures:
        print("STAGE_INVENTORY_INTEGRITY_FAIL: " + "; ".join(failures), file=sys.stderr)
        return 1
    print("STAGE_INVENTORY_INTEGRITY_PASS; logs " + str(logs))
    return 0


if __name__ == "__main__":
    sys.exit(main())
