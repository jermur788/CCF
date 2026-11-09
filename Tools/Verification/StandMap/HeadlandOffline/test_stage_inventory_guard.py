#!/usr/bin/env python3
"""Integrity test for the #303 two-stage inventory guard in run_headland_check.py.

Runs the real checker end to end in a disposable sparse clone OUTSIDE the repository. First it checks that an
unmutated clone passes in full. Then, for each mutation, it resets the clone, applies one forbidden change and
requires the checker to exit 1 with exactly the intended assertion. A mutation that passes, or that is caught by a
different assertion, fails this test. The guard must reject each change at the rule written for it, not by luck.

Never edits the checkout it is run from. The clone borrows objects from the shared store (git clone --shared)
and is deleted at the end. Logs stay under --work.
"""
import argparse
from pathlib import Path
import shutil
import subprocess
import sys

HERE = Path(__file__).resolve().parent
ROOT = HERE.parents[3]
CHECKER_IN_REPO = Path("Tools/Verification/StandMap/HeadlandOffline/run_headland_check.py")
SPARSE = ("Assets/ForestPrototype/UI", "Assets/ForestPrototype/Racks", "Assets/ForestPrototype/ScenarioOne",
          "Assets/Scenes", "Packages", "ProjectSettings", "Tools/Verification/StandMap")
FAIL_PREFIX = "HEADLAND_SOURCE_AND_OFFLINE_FAIL: "
STAGE1 = "stage 1: present and byte-identical to accepted #297 content Assets/ForestPrototype/UI/Map/"
STAGE2 = "stage 2: present and byte-identical to accepted #296 content Assets/ForestPrototype/UI/"
OBJECTIVES = "stage 2: LearningObjectivesView.cs differs from #297 anchor only by the authorised Objective 1 diagram call-site argument"
INVENTORY = "stage 2: since #297 anchor exactly the two #296 diagram files and the Objectives call site changed (tracked or untracked)"
PRE_MAP_BASE = "7f5278bfd42997da055da63423b6b4c30c7bb8f3"
ANCHOR_297 = "1b68ddcd583ab2bddd43b5ce18365171c0bab3b4"


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


def restore_from(root, commit, path):
    (root / path).write_bytes(subprocess.run(["git", "-C", str(root), "show", f"{commit}:{path}"],
                                             check=True, capture_output=True).stdout)


def commit_new_file(root, path):
    (root / path).write_text("public static class Unapproved { }\n")
    git(root, "add", "--sparse", path)
    git(root, "-c", "user.name=mutation", "-c", "user.email=mutation@invalid", "commit", "-q", "-m", "mutation", "--", path)


UI = Path("Assets/ForestPrototype/UI")
LOV = UI / "LearningObjectivesView.cs"
CALL = "lesson.Caption, rackV2Objective));"
# (case, mutation(root), exact assertion that must reject it)
MUTATIONS = [
    ("1a unauthorised rack edit", lambda r: append(r / "Assets/ForestPrototype/Racks/RackGeometry.cs", "// mutation\n"), INVENTORY),
    ("1b unauthorised save edit", lambda r: nudge(r / "Assets/ForestPrototype/ForestSaveData.cs"), INVENTORY),
    ("1c unauthorised scene edit", lambda r: append(r / "Assets/Scenes/ForestTest.unity", "\n"), INVENTORY),
    ("1d unauthorised Packages edit", lambda r: append(r / "Packages/manifest.json", "\n"), INVENTORY),
    ("1e unauthorised ProjectSettings edit", lambda r: append(r / "ProjectSettings/EditorBuildSettings.asset", "\n"), INVENTORY),
    ("1f unauthorised T2 USS edit", lambda r: append(r / UI / "Resources/ScenarioOneUi.uss", "\n"), INVENTORY),
    ("2a original #297 map file missing", lambda r: (r / UI / "Map/StandMapLegend.cs").unlink(), STAGE1 + "StandMapLegend.cs"),
    ("2b original #297 map file altered", lambda r: nudge(r / UI / "Map/StandMapDrawPlan.cs"), STAGE1 + "StandMapDrawPlan.cs"),
    ("2c original #297 map delta reverted to pre-map", lambda r: restore_from(r, PRE_MAP_BASE, str(UI / "Map/StandMapModel.cs")),
     STAGE1 + "StandMapModel.cs"),
    ("3a Objectives V2 gate statement changed (line 152)", lambda r: replace_once(r / LOV,
     "bool rackV2Objective = TeachingRacks && ui.Manager.RackPlanningV2Active;", "bool rackV2Objective = true;"), OBJECTIVES),
    ("3b Objectives unrelated edit elsewhere", lambda r: nudge(r / LOV), OBJECTIVES),
    ("3c Objectives call site with different argument", lambda r: replace_once(r / LOV, CALL, "lesson.Caption, true));"), OBJECTIVES),
    ("3d Objectives call-site delta reverted", lambda r: restore_from(r, ANCHOR_297, str(LOV)), OBJECTIVES),
    ("3e Objectives file missing", lambda r: (r / LOV).unlink(), OBJECTIVES),
    ("4a extra untracked file under Assets", lambda r: (r / UI / "Unapproved.cs").write_text("public class Unapproved { }\n"), INVENTORY),
    ("4b extra untracked .meta under Assets", lambda r: (r / UI / "Unapproved.cs.meta").write_text("fileFormatVersion: 2\n"), INVENTORY),
    ("4c extra file committed under Assets", lambda r: commit_new_file(r, "Assets/ForestPrototype/Racks/Unapproved.cs"), INVENTORY),
    ("5a #296 TeachingDiagram.cs altered by one space", lambda r: nudge(r / UI / "TeachingDiagram.cs"), STAGE2 + "TeachingDiagram.cs"),
    ("5b #296 TeachingDiagramPlan.cs reverted to #297 anchor", lambda r: restore_from(r, ANCHOR_297, str(UI / "TeachingDiagramPlan.cs")),
     STAGE2 + "TeachingDiagramPlan.cs"),
    ("5c #296 TeachingDiagram.cs missing", lambda r: (r / UI / "TeachingDiagram.cs").unlink(), STAGE2 + "TeachingDiagram.cs"),
]


# Integrated-candidate identity (#355), selected with --integrated. The historical MUTATIONS above are untouched and still describe
# the stage guard; they are meant for the stage fixture, not the combined tree.
INTEGRATED_PASS = "HEADLAND_INTEGRATED_IDENTITY_AND_OFFLINE_PASS"
SOURCE_ASSERTION = "integrated: StandMapSource.cs is the accepted #297 content plus only the approved SceneHandle edits"
PATH_SET = "integrated: production changes since the common ancestor are exactly the 9 Workbench/C2 and 11 D-072 Assets paths"
C2_BYTES = "integrated: the 9 Workbench/C2 paths (incl. scene and Packages) are byte-identical to C2"
D072_BYTES = "integrated: the other 10 D-072 Assets paths are byte-identical to the approved D-072 tip"
STAND_MAP_SOURCE = UI / "Map/StandMapSource.cs"
INTEGRATED_MUTATIONS = [
    ("i1 StandMapSource.cs altered by one space", lambda r: nudge(r / STAND_MAP_SOURCE), SOURCE_ASSERTION),
    ("i2 StandMapSource.cs back to the CS0619 int cache", lambda r: restore_from(r, ANCHOR_297, str(STAND_MAP_SOURCE)), SOURCE_ASSERTION),
    ("i3 scene-change comparison disabled", lambda r: replace_once(r / STAND_MAP_SOURCE, "headlandScene != scene ||",
     "false && headlandScene != scene ||"), SOURCE_ASSERTION),
    ("i4 extra file committed under Assets", lambda r: commit_new_file(r, "Assets/ForestPrototype/Racks/Unapproved.cs"), PATH_SET),
    ("i5 extra untracked .meta under Assets", lambda r: (r / UI / "Unapproved.cs.meta").write_text("fileFormatVersion: 2\n"), PATH_SET),
    ("i6 C2 Packages manifest altered", lambda r: append(r / "Packages/manifest.json", "\n"), C2_BYTES),
    ("i7 C2 scene altered", lambda r: append(r / "Assets/Scenes/ForestTest.unity", "\n"), C2_BYTES),
    ("i8 another D-072 file altered", lambda r: nudge(r / UI / "WorkPlanView.cs"), D072_BYTES),
]


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--checker", type=Path, default=HERE / "run_headland_check.py", help="checker file under test")
    parser.add_argument("--work", type=Path,
                        default=Path("/media/jer/Files/CCF/Verification/D072-HeadlandStageInventory-20261106/integrity-work"))
    parser.add_argument("--commit", default=None, help="commit to clone (default: HEAD of this checkout)")
    parser.add_argument("--integrated", action="store_true",
                        help="test the integrated-candidate identity mode (clean run + INTEGRATED_MUTATIONS) instead of the stage guard")
    arguments = parser.parse_args()
    work = arguments.work.resolve()
    if work.is_relative_to(ROOT) or ROOT.is_relative_to(work):
        raise SystemExit("work directory must be outside the repository")
    checker = arguments.checker.resolve().read_bytes()
    commit = arguments.commit or git(ROOT, "rev-parse", "HEAD").strip()
    store = Path(git(ROOT, "rev-parse", "--path-format=absolute", "--git-common-dir").strip())
    clone = work / "clone"
    logs = work / "logs"
    if clone.exists():
        shutil.rmtree(clone)
    logs.mkdir(parents=True, exist_ok=True)
    subprocess.run(["git", "clone", "-q", "--shared", "--no-checkout", str(store), str(clone)], check=True)
    git(clone, "sparse-checkout", "set", "--cone", *SPARSE)
    git(clone, "checkout", "-q", "--detach", commit)
    print(f"CLONE {clone} COMMIT {commit} CHECKER {arguments.checker.resolve()}", flush=True)

    def reset():
        git(clone, "reset", "-q", "--hard", commit)
        git(clone, "clean", "-q", "-fdx", "--", "Assets", "Packages", "ProjectSettings")
        (clone / CHECKER_IN_REPO).write_bytes(checker)

    def run(name):
        result = subprocess.run([sys.executable, "-I", "-B", str(clone / CHECKER_IN_REPO), *extra, "--out", str(work / "dotnet" / name)],
                                capture_output=True, text=True)
        (logs / (name + ".log")).write_text(f"exit={result.returncode}\n--- stdout\n{result.stdout}--- stderr\n{result.stderr}")
        return result

    failures = []
    extra = ["--integrated"] if arguments.integrated else []
    mutations = INTEGRATED_MUTATIONS if arguments.integrated else MUTATIONS
    pass_marker = INTEGRATED_PASS if arguments.integrated else "HEADLAND_SOURCE_AND_OFFLINE_PASS"
    expected_source = 42 if arguments.integrated else 39

    def clean_run(label):
        reset()
        result = run(label)
        source = sum(line.startswith("PASS SOURCE ") for line in result.stdout.splitlines())
        ok = (result.returncode == 0 and source == expected_source and "HEADLAND_OFFLINE_TALLY PASS=92 FAIL=0" in result.stdout
              and pass_marker in result.stdout)
        print(f"{'PASS' if ok else 'FAIL'} CLEAN {label}: exit={result.returncode} PASS_SOURCE={source} "
              f"{'C#=92/92' if 'PASS=92 FAIL=0' in result.stdout else 'C# tally missing'}", flush=True)
        if not ok:
            failures.append(label)

    clean_run("clean-before")
    for case, mutate, expected in mutations:
        reset()
        mutate(clone)
        name = "mutation-" + case.split()[0]
        result = run(name)
        reported = result.stderr.strip()
        if result.returncode == 1 and reported == FAIL_PREFIX + expected:
            print(f"PASS CAUGHT {case}\n    exit=1 {reported}", flush=True)
        else:
            verdict = "NOT CAUGHT" if result.returncode == 0 else "CAUGHT BY WRONG ASSERTION"
            print(f"FAIL {verdict} {case}\n    exit={result.returncode} {reported or '(checker passed)'}\n    expected: {expected}", flush=True)
            failures.append(case)
    clean_run("clean-after")
    shutil.rmtree(clone)
    label = "INTEGRATED_IDENTITY_INTEGRITY" if arguments.integrated else "STAGE_INVENTORY_INTEGRITY"
    print(f"{label}_TALLY MUTATIONS={len(mutations)} CLEAN_RUNS=2 FAIL={len(failures)}")
    if failures:
        print(f"{label}_FAIL: " + "; ".join(failures), file=sys.stderr)
        return 1
    print(f"{label}_PASS; logs " + str(logs))
    return 0


if __name__ == "__main__":
    sys.exit(main())
