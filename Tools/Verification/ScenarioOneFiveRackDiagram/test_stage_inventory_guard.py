#!/usr/bin/env python3
"""Integrity test for run_five_rack_check.py (#306 stage guard, repaired and extended by #308).

Runs the real checker end to end in a disposable full clone OUTSIDE the repository. It proves four things:

1. CLEAN RUNS: on the unmodified candidate the full run, the --no-mutations run and the --scope-only run each print exactly the expected
   assertion names, in order, with exact tallies (109 structural, 165 painter, 29 mutations, the original 131 obligations) and the marker that
   belongs to that mode. Only the full run may print FIVE_RACK_SUITE_PASS.
2. CASES: for each forbidden change it resets the clone, applies one change and requires exit 1 with EXACTLY the declared set of failing
   assertions. A case that passes, that fails a different assertion, or that fails extra assertions fails this test.
3. CONTROLS: it weakens copies of the checker the way a careless or dishonest edit would, re-seals each copy's self-pin (so the control does not
   succeed only because the bytes changed) and requires the predicted detection. Reviewer counterexamples W1-W5 are among them.
4. PINS: the checker under test must hash to CHECKER_SHA256 and the inventory to ASSERTIONS_SHA256.

Partial runs (--skip-clean, --skip-controls, --only) print STAGE_INVENTORY_INTEGRITY_PARTIAL with exit 2, never the PASS marker.

Never edits the checkout it is run from. The clone borrows objects from the shared store (git clone --shared) and is deleted at the end. Logs
stay under --work, which must be new or empty, so a run never overwrites earlier evidence.
"""
import argparse
import datetime
import hashlib
import importlib.util
import itertools
import json
from pathlib import Path
import re
import shutil
import subprocess
import sys

sys.dont_write_bytecode = True

HERE = Path(__file__).resolve().parent
ROOT = HERE.parents[2]
CHECKER_DIR = "Tools/Verification/ScenarioOneFiveRackDiagram/"
CHECKER_IN_REPO = CHECKER_DIR + "run_five_rack_check.py"
TEST_IN_REPO = CHECKER_DIR + "test_stage_inventory_guard.py"
INVENTORY_IN_REPO = CHECKER_DIR + "expected_assertions.json"
EVIDENCE = Path("/media/jer/Files/CCF/Verification/D072-FiveRackCheckerF1F3-20261106")
# Full SHA-256 of the pinned runner. The runner in turn pins this file with this one line blanked, so the two pins do not form a cycle.
CHECKER_SHA256 = "0466c5521db0f7745f49b62c28d0004d20a2441e8f6d6708d90e7a538716a73f"
ASSERTIONS_SHA256 = "6515df087111ebda1c1832dcba72bf938648721cad6a7c5222c96b38055c4dbe"
TALLY_SOURCE_SCOPE, TALLY_SOURCE_NO_MUTATIONS, TALLY_SOURCE_FULL = 108, 122, 182
TALLY_STRUCTURAL, TALLY_PAINTER, TALLY_MUTATIONS, TALLY_ORIGINAL = 109, 165, 29, 131
BASE = "1b68ddcd583ab2bddd43b5ce18365171c0bab3b4"
ANCHOR_296 = "316429dc"
STAGE_303_SOURCE = "6a8b261fa950efe43f9f20a249fb8c318ded4213"
PRE_303 = "335ee6b3fde8b9e514cc914b0e5bd6a6e10d70a3"

HANDOFFS = "Docs/Handoffs/ScenarioOne-Overnight-Playtest-Readiness-20261106/"
HEADLAND = "Tools/Verification/StandMap/HeadlandOffline/run_headland_check.py"
STAGE_TEST = "Tools/Verification/StandMap/HeadlandOffline/test_stage_inventory_guard.py"
HEADLAND_CS = "Tools/Verification/StandMap/HeadlandOffline/HeadlandOfflineCheck.cs"
STAGE_HANDOFF = HANDOFFS + "D072-HEADLAND-STAGE-INVENTORY-CHECKER-REPAIR-HANDOFF.md"
HANDOFF_306 = HANDOFFS + "D072-FIVE-RACK-STAGE-INVENTORY-CHECKER-REPAIR-HANDOFF.md"
HANDOFF_REPAIR = HANDOFFS + "D072-FIVE-RACK-DIAGRAM-BOUNDED-REPAIR-HANDOFF.md"
HANDOFF_IMPLEMENTATION = HANDOFFS + "OBJECTIVE-GUIDANCE-TEACHING-MAP-IMPLEMENTATION-HANDOFF.md"
HANDOFF_308 = HANDOFFS + "D072-FIVE-RACK-CHECKER-F1-F3-REPAIR-HANDOFF.md"
UI = "Assets/ForestPrototype/UI/"
DIAGRAM = UI + "TeachingDiagram.cs"
PLAN = UI + "TeachingDiagramPlan.cs"
OBJECTIVES = UI + "LearningObjectivesView.cs"
MAP = UI + "Map/StandMapSource.cs"
RACK = "Assets/ForestPrototype/Racks/RackGeometry.cs"
SAVE = "Assets/ForestPrototype/ForestSaveData.cs"
SCENE = "Assets/Scenes/ForestTest.unity"
PACKAGES = "Packages/manifest.json"
SETTINGS = "ProjectSettings/EditorBuildSettings.asset"
CHECK_CS = CHECKER_DIR + "FiveRackDiagramCheck.cs"
PROBE_CS = CHECKER_DIR + "PainterProbe/PainterProbe.cs"
STUBS_CS = CHECKER_DIR + "PainterProbe/RecordingUnityStubs.cs"
UNRELATED_TOOL = "Tools/Verification/NextActionGuidance/check_sources.py"
CALL_OLD = ("stage = Plan.ShowsFiveRackFigure(lesson, rackV2World) ? "
            "Plan.ObjectiveOneFigure(SiteFromAuthority()) : Plan.For(lesson);")
CALL_NEW = "stage = lesson == 0 ? Plan.ObjectiveOneFigure(SiteFromAuthority()) : Plan.For(lesson);"
DIAGRAM_SHA = "5062dd7dd1836cdb9663b257c0e4a4b74daf3b6f183923a27e6f86e35be83874"
DIAGRAM_BLOB = "65d4aba342332bcb41012a0c7b8867b05aa03164"
HEADLAND_SHA = "73e111497927e478c89d90484e63ffba24e3dfb4ee984dce96d8c36d80f2eccd"
WRONG_SHA = "f" * 64
WRONG_BLOB = "f" * 40

# Assertion names, spelled here independently of the runner.
ONLY_ALLOWED = "only allowed files changed since 1b68ddcd"
NO_ASSETS = "no new Assets file or .meta"
NO_PACKAGES = "no package or project-settings change"
FOLDER = "the five-rack folder holds exactly its six named files (tracked, untracked or ignored)"
IGNORED = "no ignored untracked path under Assets, Packages or ProjectSettings"
INDEX_FLAGS = "the git index holds no assume-unchanged, skip-worktree or other non-normal entry (git ls-files -v)"
INTERNAL = "the checker ran to the end of its mode without an internal error"
SELF_PIN = "this runner matches its own recorded SHA-256 (pin line blanked)"
TEST_PIN = "the integrity test matches its recorded SHA-256 (its checker pin line blanked)"
INVENTORY_PIN = "the expected assertion inventory matches its recorded SHA-256"
TABLE_COUNT = "the mutation table holds exactly the 15 plan and 14 painter mutations"
TABLE_DIGEST = "the mutation table keeps every original name, anchor, replacement and expected text"
NAMES_SOURCE = "the source assertions match the pinned inventory for this mode by name and order"
NAMES_STRUCTURAL = "exactly the 109 pinned structural assertions ran and passed"
NAMES_PAINTER = "exactly the 165 pinned painter assertions ran and passed"
HANDOFF_TEXT = "the #308 handoff, when present, is a regular UTF-8 text file at its one exact path"
OBJECTIVES_ONLY = "LearningObjectivesView.cs differs from base only at the Objective 1 diagram call, which now passes rackV2Objective"
OBJECTIVES_FLAG = "the passed flag is the view's existing rackV2Objective, computed before the call and unchanged"
SELECTION = "the painter chooses the five-rack figure only through Plan.ShowsFiveRackFigure(lesson, rackV2World); otherwise Plan.For(lesson)"
IMPLEMENTATION_UNCHANGED = "the original #296 implementation handoff is unchanged since the reviewed state e7bda1f2"
ONE_PLACE = "the painter builds the five-rack figure in exactly one place"
STAGE3_ALL = "stage 3: HEAD contains the #303 stage, so exactly its two verifier scripts and its handoff changed since 1b68ddcd"
STAGE3_PATHS = "stage 3: no Stand Map path outside the two pinned #303 verifier scripts changed"
STAGE3_PREDATES = "stage 3: HEAD predates #303, so no #303 verifier path may be changed"


def stage3_blob(path, commit="6a8b261f"):
    return f"stage 3: pinned #303 blob {path} at {commit} matches its recorded SHA-256"


def stage3_bytes(path, commit="6a8b261f"):
    return f"stage 3: {path} is byte-identical to pinned #303 commit {commit}"


def pin_blob(label, path, commit=ANCHOR_296):
    return f"{label}: {path} at {commit} has its recorded git blob and SHA-256"


def pin_head(label, path):
    return f"{label}: {path} at HEAD is the pinned git blob"


def pin_tree(label, path):
    return f"{label}: {path} in the working tree has exactly the pinned bytes"


CODE, VERIFIER, DOC = "#296 pin", "#296 verifier pin", "handoff pin"


def git(root, *arguments):
    return subprocess.run(["git", "-C", str(root), *arguments], check=True, capture_output=True, text=True).stdout


def sealed_digest(data, constant):
    blanked, count = re.subn(b"^" + constant + b' = "[0-9a-f]*"', constant + b' = ""', data, flags=re.M)
    return hashlib.sha256(blanked).hexdigest() if count == 1 else None


def seal_checker(data):
    """Rewrite the self-pin line so a weakened copy still passes its own self-check, as a coordinated edit would. Copies with no pin line pass through."""
    digest = sealed_digest(data, b"SELF_SHA256")
    if digest is None:
        return data
    return re.sub(b'^SELF_SHA256 = "[0-9a-f]*"', b'SELF_SHA256 = "' + digest.encode() + b'"', data, flags=re.M)


# ---- Mutation helpers. Each works on the clone only. --------------------------------------------------------------------------------------------

def append(root, path, text):
    with open(root / path, "a") as handle:
        handle.write(text)


def nudge(root, path):
    """Smallest content change: one extra space after the first ';' past the file midpoint."""
    target = root / path
    text = target.read_text()
    position = text.index(";", len(text) // 2) + 1
    target.write_text(text[:position] + " " + text[position:])


def replace_once(root, path, old, new):
    target = root / path
    text = target.read_text()
    if text.count(old) != 1:
        raise RuntimeError(f"mutation setup: expected one occurrence of {old!r} in {path}")
    target.write_text(text.replace(old, new))


def write(root, path, data):
    target = root / path
    target.parent.mkdir(parents=True, exist_ok=True)
    if isinstance(data, bytes):
        target.write_bytes(data)
    else:
        target.write_text(data)


def delete(root, path):
    (root / path).unlink()


def restore_from(root, commit, path):
    write(root, path, subprocess.run(["git", "-C", str(root), "show", f"{commit}:{path}"], check=True, capture_output=True).stdout)


def commit_paths(root, *paths):
    git(root, "add", "-A", "--", *paths)
    git(root, "-c", "user.name=mutation", "-c", "user.email=mutation@invalid", "commit", "-q", "-m", "mutation", "--", *paths)


def drop_statement(root, path, start):
    """Remove the whole bracketed statement beginning with start (a deleted check, as a weakening edit would delete it)."""
    target = root / path
    text = target.read_text()
    if text.count(start) != 1:
        raise RuntimeError(f"mutation setup: expected one occurrence of {start!r} in {path}")
    begin = text.index(start)
    position, depth, quoted, escaped = begin, 0, False, False
    while True:
        character = text[position]
        if quoted:
            if escaped:
                escaped = False
            elif character == "\\":
                escaped = True
            elif character == '"':
                quoted = False
        elif character == '"':
            quoted = True
        elif character in "([{":
            depth += 1
        elif character in ")]}":
            depth -= 1
            if depth == 0:
                break
        position += 1
    position += 1
    if text[position:position + 1] == ";":
        position += 1
    target.write_text(text[:begin] + text[position:])


def patch_checker(root, old, new):
    """Change a recorded constant inside the clone's checker and re-seal it, so only the constant's own check can object."""
    replace_once(root, CHECKER_IN_REPO, old, new)
    write(root, CHECKER_IN_REPO, seal_checker((root / CHECKER_IN_REPO).read_bytes()))


def set_flag(root, flag, path):
    git(root, "update-index", flag, path)
    append(root, path, "// hidden edit\n")


def hidden_ignored(root, directory, name, content="public class Hidden { }\n"):
    write(root, directory + "/.gitignore", "*\n")
    write(root, directory + "/" + name, content)


def rename_into_handoff(root, source):
    """Launder a deletion: git reports a rename under its new name only, unless renames are turned off."""
    git(root, "mv", "-f", source, HANDOFF_308)


def commit_then_put_back(root, path):
    original = (root / path).read_bytes()
    append(root, path, "// committed mutation\n")
    commit_paths(root, path)
    write(root, path, original)


def pre_303_head_with_one_file(root):
    """A checkout that predates #303 but carries one #303 file. Only the HEAD-aware rule can tell this from the real stage."""
    runner = (root / CHECKER_IN_REPO).read_bytes()
    stage_test = subprocess.run(["git", "-C", str(root), "show", f"{STAGE_303_SOURCE}:{STAGE_TEST}"], check=True, capture_output=True).stdout
    git(root, "reset", "-q", "--hard", PRE_303)
    write(root, CHECKER_IN_REPO, runner)
    write(root, STAGE_TEST, stage_test)


def symlink_diagram(root):
    data = (root / DIAGRAM).read_bytes()
    write(root, DIAGRAM + ".real", data)
    delete(root, DIAGRAM)
    (root / DIAGRAM).symlink_to("TeachingDiagram.cs.real")


# (id, description, mutation(root), exact failing assertions, options). options: "crash" = the checker stops early with its internal-error assertion;
# "subset" = the set must be contained in the failures (only for a checkout at another commit, where many pins are expected to fail too).
CASES = [
    # Original #306 cases, same ids and same mutations. Their expected sets now include the #308 pin that also fires.
    ("303a", "headland checker altered by one space", lambda r: nudge(r, HEADLAND), {stage3_bytes(HEADLAND)}),
    ("303b", "stage-inventory test altered by one space", lambda r: nudge(r, STAGE_TEST), {stage3_bytes(STAGE_TEST)}),
    ("303c", "historical handoff altered by one newline", lambda r: append(r, STAGE_HANDOFF, "\n"), {stage3_bytes(STAGE_HANDOFF, "69b41605")}),
    ("303d", "extra .py injected under StandMap", lambda r: write(r, "Tools/Verification/StandMap/HeadlandOffline/extra.py", "# unapproved\n"),
     {STAGE3_PATHS, ONLY_ALLOWED}),
    ("303e", "same-named StandMap script in a new subdirectory",
     lambda r: write(r, "Tools/Verification/StandMap/Unapproved/run_headland_check.py", "# unapproved\n"), {STAGE3_PATHS, ONLY_ALLOWED}),
    ("303f", "extra Markdown beside the #303 handoff", lambda r: write(r, HANDOFFS + "D072-UNREVIEWED.md", "# unapproved\n"), {ONLY_ALLOWED}),
    ("1a", "unauthorised rack edit", lambda r: append(r, RACK, "// mutation\n"), {ONLY_ALLOWED, NO_ASSETS}),
    ("1b", "unauthorised save edit", lambda r: nudge(r, SAVE), {ONLY_ALLOWED, NO_ASSETS}),
    ("1c", "unauthorised scene edit", lambda r: append(r, SCENE, "\n"), {ONLY_ALLOWED, NO_ASSETS, "unchanged: " + SCENE}),
    ("1d", "unauthorised Packages edit", lambda r: append(r, PACKAGES, "\n"), {NO_PACKAGES, ONLY_ALLOWED}),
    ("1e", "unauthorised ProjectSettings edit", lambda r: append(r, SETTINGS, "\n"), {NO_PACKAGES, ONLY_ALLOWED}),
    ("2a", "unauthorised diagram selection edit", lambda r: replace_once(r, DIAGRAM, CALL_OLD, CALL_NEW), {SELECTION, pin_tree(CODE, DIAGRAM)}),
    ("2b", "unauthorised Objective edit", lambda r: nudge(r, OBJECTIVES), {OBJECTIVES_ONLY, pin_tree(CODE, OBJECTIVES)}),
    ("2c", "unauthorised map edit", lambda r: nudge(r, MAP), {ONLY_ALLOWED, NO_ASSETS, "unchanged: " + MAP}),

    # F-1: the five-rack runner alone proves the #296 diagram bytes.
    ("S03", "one comment byte in TeachingDiagram.cs", lambda r: replace_once(r, DIAGRAM, "// A thin continuous outline:", "// a thin continuous outline:"),
     {pin_tree(CODE, DIAGRAM)}),
    ("S04", "title margin 6 to 60 UI units", lambda r: replace_once(r, DIAGRAM, "if (title) label.style.marginBottom = 6f;", "if (title) label.style.marginBottom = 60f;"),
     {pin_tree(CODE, DIAGRAM)}),
    ("S05", "note adds the false claim Routes open.", lambda r: replace_once(r, PLAN, 'Approximate scale, north up.",', 'Approximate scale, north up. Routes open.",'),
     {pin_tree(CODE, PLAN)}),
    ("S22", "confirmed leaks into the draft key row", lambda r: replace_once(r, PLAN, '"Draft rack: dashed,\\na proposal only"', '"Draft rack: dashed,\\nconfirmed proposal"'),
     {pin_tree(CODE, PLAN)}),
    ("S23", "entrance square 11 to 1 design unit", lambda r: replace_once(r, PLAN, "EntranceSide = 11f", "EntranceSide = 1f"), {pin_tree(CODE, PLAN)}),
    ("F1a", "opening claim says planning clears the routes",
     lambda r: replace_once(r, PLAN, "Planning or confirming does not clear the routes.", "Planning or confirming clears the routes."), {pin_tree(CODE, PLAN)}),
    ("F1b", "one trailing byte on the plan", lambda r: append(r, PLAN, " "), {pin_tree(CODE, PLAN)}),
    ("F1c", "diagram edit committed", lambda r: (nudge(r, DIAGRAM), commit_paths(r, DIAGRAM)), {pin_head(CODE, DIAGRAM), pin_tree(CODE, DIAGRAM)}),
    ("F1d", "plan edit committed", lambda r: (nudge(r, PLAN), commit_paths(r, PLAN)), {pin_head(CODE, PLAN), pin_tree(CODE, PLAN)}),
    ("F1e", "TeachingDiagram.cs deleted", lambda r: delete(r, DIAGRAM), {pin_tree(CODE, DIAGRAM), INTERNAL}, "crash"),
    ("F1f", "plan reverted to the pre-#296 anchor", lambda r: restore_from(r, BASE, PLAN), {pin_tree(CODE, PLAN), INTERNAL}, "crash"),
    ("F1g", "wrong recorded SHA-256 for the diagram", lambda r: patch_checker(r, DIAGRAM_SHA, WRONG_SHA),
     {pin_blob(CODE, DIAGRAM), pin_tree(CODE, DIAGRAM)}),
    ("F1h", "wrong recorded git blob for the diagram", lambda r: patch_checker(r, DIAGRAM_BLOB, WRONG_BLOB),
     {pin_blob(CODE, DIAGRAM), pin_head(CODE, DIAGRAM)}),
    ("F1i", "diagram replaced by a symlink to identical bytes", symlink_diagram, {pin_tree(CODE, DIAGRAM), ONLY_ALLOWED, NO_ASSETS}),

    # F-2: the #303 stage, committed edits, deletion, wrong hash and historical guards.
    ("S10", "#303 test script deleted (partial stage)", lambda r: delete(r, STAGE_TEST), {STAGE3_ALL, stage3_bytes(STAGE_TEST)}),
    ("S11", "headland runner reverted to its #297 bytes", lambda r: restore_from(r, BASE, HEADLAND), {STAGE3_ALL, stage3_bytes(HEADLAND)}),
    ("S12", "#303 runner one-space edit, committed", lambda r: (nudge(r, HEADLAND), commit_paths(r, HEADLAND)), {stage3_bytes(HEADLAND)}),
    ("S13", "#303 handoff deleted", lambda r: delete(r, STAGE_HANDOFF), {STAGE3_ALL, stage3_bytes(STAGE_HANDOFF, "69b41605")}),
    ("S14", "original-30 guard deleted from the headland runner",
     lambda r: drop_statement(r, HEADLAND, 'require(re.search(r"RackRefreshSeconds = 0\\.25f;", source)'), {stage3_bytes(HEADLAND)}),
    ("S15", "original-92 C# check deleted", lambda r: drop_statement(r, HEADLAND_CS, "Check(model.ContentHash() == stableHash"), {STAGE3_PATHS, ONLY_ALLOWED}),
    ("S16", "look-alike StandMap2 and standmap paths",
     lambda r: (write(r, "Tools/Verification/StandMap2/x.py", "#\n"), write(r, "Tools/Verification/standmap/x.py", "#\n")), {ONLY_ALLOWED}),
    ("S20", "#303 integrity test == 39 loosened to >= 30", lambda r: replace_once(r, STAGE_TEST, "source == 39", "source >= 30"), {stage3_bytes(STAGE_TEST)}),
    ("F2a", "wrong recorded SHA-256 for the headland runner", lambda r: patch_checker(r, HEADLAND_SHA, WRONG_SHA),
     {stage3_blob(HEADLAND), stage3_bytes(HEADLAND)}),
    ("F2b", "#303 test script deleted and committed", lambda r: (delete(r, STAGE_TEST), commit_paths(r, STAGE_TEST)), {STAGE3_ALL, stage3_bytes(STAGE_TEST)}),
    ("F2c", "pre-#303 HEAD carrying one #303 file", pre_303_head_with_one_file, {STAGE3_PREDATES}, "subset"),

    # F-3: exact inventory, no folder allowance, no unrelated path, and the protected checker files.
    ("S17", "unrelated binary in the five-rack folder", lambda r: write(r, CHECKER_DIR + "unrelated.bin", b"\x00\x01"), {ONLY_ALLOWED, FOLDER}),
    ("S18", "structural C# check deleted", lambda r: drop_statement(r, CHECK_CS, "Check(stage.Items.Count(i => i.Kind == TeachingDiagramPlan.Kind.NorthArrow) == 1"),
     {pin_tree(VERIFIER, CHECK_CS)}),
    ("S19", "painter assertion deleted", lambda r: drop_statement(r, PROBE_CS, "Result(north.Count == 2"), {pin_tree(VERIFIER, PROBE_CS)}),
    ("S21", "integrity test accepts any exit code", lambda r: replace_once(r, TEST_IN_REPO, "exit_ok = observed.exit " + "== 1", "exit_ok = observed.exit " + ">= 0"), {TEST_PIN}),
    ("F3a", "new unrelated test file in the folder", lambda r: write(r, CHECKER_DIR + "test_other.py", "print('ok')\n"), {ONLY_ALLOWED, FOLDER}),
    ("F3b", "author test replaced by an unrelated one", lambda r: write(r, TEST_IN_REPO, "print('STAGE_INVENTORY_INTEGRITY_PASS')\n"), {TEST_PIN}),
    ("F3c", "author test deleted", lambda r: delete(r, TEST_IN_REPO), {TEST_PIN, FOLDER}),
    ("F3d", "the inventory edited", lambda r: replace_once(r, INVENTORY_IN_REPO, "Do not edit to make a check pass.", "Edited."),
     {INVENTORY_PIN, TABLE_DIGEST, NAMES_SOURCE}),
    ("F3e", "inventory deleted", lambda r: delete(r, INVENTORY_IN_REPO), {INVENTORY_PIN, TABLE_DIGEST, NAMES_SOURCE, FOLDER}),
    ("F3f", "recording stubs edited", lambda r: nudge(r, STUBS_CS), {pin_tree(VERIFIER, STUBS_CS)}),
    ("F3g", "unrelated NextActionGuidance check_sources.py edited", lambda r: append(r, UNRELATED_TOOL, "# mutation\n"), {ONLY_ALLOWED}),
    ("F3h", "ignored binary hidden in the five-rack folder", lambda r: (write(r, CHECKER_DIR + ".gitignore", "*\n"), write(r, CHECKER_DIR + "hidden.bin", b"\x00")), {FOLDER}),
    ("F3i", "#306 handoff altered by one newline", lambda r: append(r, HANDOFF_306, "\n"), {pin_tree(DOC, HANDOFF_306)}),
    ("F3j", "#296 repair handoff altered", lambda r: append(r, HANDOFF_REPAIR, "\n"), {pin_tree(DOC, HANDOFF_REPAIR)}),
    ("F3k", "#296 implementation handoff altered", lambda r: append(r, HANDOFF_IMPLEMENTATION, "\n"),
     {pin_tree(DOC, HANDOFF_IMPLEMENTATION), IMPLEMENTATION_UNCHANGED}),
    ("F3l", "binary at the #308 handoff path", lambda r: write(r, HANDOFF_308, b"\x00\x01binary"), {HANDOFF_TEXT}),
    ("F3m", "near-name #308 handoff", lambda r: write(r, HANDOFF_308.replace(".md", "2.md"), "# unapproved\n"), {ONLY_ALLOWED}),
    ("F3n", "production file deleted through a rename into the handoff path", lambda r: rename_into_handoff(r, SAVE), {ONLY_ALLOWED, NO_ASSETS}),
    ("F3o", "rack edit committed, then the working tree put back", lambda r: commit_then_put_back(r, RACK), {ONLY_ALLOWED, NO_ASSETS}),
    ("F3p", "runner edited without re-sealing its self-pin", lambda r: append(r, CHECKER_IN_REPO, "# weakened\n"), {SELF_PIN}),

    # F-4: paths and index state that ordinary inventories cannot see.
    ("S08", "untracked script behind a nested .gitignore under Assets", lambda r: hidden_ignored(r, UI + "Hidden", "Script.cs"), {IGNORED}),
    ("S09", "rack edit hidden with assume-unchanged", lambda r: set_flag(r, "--assume-unchanged", RACK), {INDEX_FLAGS}),
    ("F4a", "ignored file under Packages", lambda r: hidden_ignored(r, "Packages/Hidden", "x.json", "{}\n"), {IGNORED}),
    ("F4b", "ignored file under ProjectSettings", lambda r: hidden_ignored(r, "ProjectSettings/Hidden", "x.asset", "x\n"), {IGNORED}),
    ("F4c", "script hidden through .git/info/exclude", lambda r: (append(r, ".git/info/exclude", UI + "Excluded.cs\n"), write(r, UI + "Excluded.cs", "public class E { }\n")), {IGNORED}),
    ("F4d", "file under Assets matching an existing root ignore rule", lambda r: write(r, UI + "Generated.csproj", "<Project/>\n"), {IGNORED}),
    ("F4e", "rack edit hidden with skip-worktree", lambda r: set_flag(r, "--skip-worktree", RACK), {INDEX_FLAGS}),
    ("F4f", "diagram edit hidden with assume-unchanged", lambda r: set_flag(r, "--assume-unchanged", DIAGRAM), {INDEX_FLAGS, pin_tree(CODE, DIAGRAM)}),

    # The rest of the reviewer's 32 S cases, committed so they stay checked.
    ("S06", "selection seam also fires on the introduction", lambda r: replace_once(r, PLAN, "=> lesson == 0 && rackV2World;", "=> lesson <= 0 && rackV2World;"),
     {pin_tree(CODE, PLAN), "selection seam: only lesson 0 with the caller's Rack V2 flag shows the figure"}),
    ("S07", "untracked png and .meta under UI/Resources", lambda r: (write(r, UI + "Resources/unrelated.png", "x"), write(r, UI + "Resources/unrelated.png.meta", "fileFormatVersion: 2\n")),
     {ONLY_ALLOWED, NO_ASSETS}),
    ("S24", "draft swatch fill made opaque", lambda r: replace_once(r, DIAGRAM, "Polygon(p, Tint(item.Role, a * Plan.RackFillAlpha * 0.5f), ", "Polygon(p, Colour(item.Role, a * Plan.RackFillAlpha * 0.5f), "),
     {pin_tree(CODE, DIAGRAM)}),
    ("S25", "map corridors drawn half width", lambda r: replace_once(r, PLAN, "Y1 = FigureY(site, rackZ[rack]), Width = site.RackWidthM * FigurePxPerM, Z = 30 });", "Y1 = FigureY(site, rackZ[rack]), Width = site.RackWidthM * FigurePxPerM * 0.5f, Z = 30 });"),
     {pin_tree(CODE, PLAN)}),
    ("S26", "shared USS edited", lambda r: replace_once(r, UI + "Resources/ScenarioOneUi.uss", ".diagram-caption { white-space: normal;", ".diagram-caption { white-space: nowrap;"),
     {ONLY_ALLOWED, NO_ASSETS, "shared stylesheet (T2-protected bytes) unchanged: the figure's title/notes reuse existing diagram classes"}),
    ("S27", "no-scrolling audit removed", lambda r: drop_statement(r, "Tools/Verification/ScenarioOneUxPedagogy/UxPedagogyShots.cs", 'AuditContained($"OBJECTIVE_CARD_NEEDS_NO_SCROLLING_{state}"'), {ONLY_ALLOWED}),
    ("S28", "onboarding fit assertion removed", lambda r: replace_once(r, "Tools/Verification/ScenarioOneRackUx/GuidedOnboardingVerification.cs",
                                                                         'Check(ui.RootElement.worldBound.Contains(current.worldBound.min) && ui.RootElement.worldBound.Contains(current.worldBound.max), "current objective fits " + size);', "// removed"),
     {ONLY_ALLOWED}),
    ("S29", ".meta GUID of TeachingDiagram.cs changed", lambda r: replace_once(r, DIAGRAM + ".meta", "guid: ", "guid: 0"), {ONLY_ALLOWED, NO_ASSETS}),
    ("S30", "near-name production file committed", lambda r: (write(r, UI + "TeachingDiagramExtras.cs", "public static class TeachingDiagramExtras { }\n"), commit_paths(r, UI + "TeachingDiagramExtras.cs")),
     {ONLY_ALLOWED, NO_ASSETS}),
    ("S31", "StandMapTransform.cs edited", lambda r: nudge(r, UI + "Map/StandMapTransform.cs"), {ONLY_ALLOWED, NO_ASSETS}),
    ("S32", "real caption gains a headland claim", lambda r: replace_once(r, UI + "ScenarioOneTeachingSequence.cs", '"Racks provide access while the rows between them remain standing."', '"Racks start at the western headland while the rows between them remain standing."'),
     {ONLY_ALLOWED, NO_ASSETS}),

    # F-6: an edited call site reports named failures and a suite marker, not a traceback.
    ("S01", "second Objectives line edited", lambda r: replace_once(r, OBJECTIVES, 'AddObjectivePart(words, "What to do", CurrentInstruction,', 'AddObjectivePart(words, "What to do next", CurrentInstruction,'),
     {OBJECTIVES_ONLY, pin_tree(CODE, OBJECTIVES)}),
    ("S02", "call site passes !rackV2Objective", lambda r: replace_once(r, OBJECTIVES, "lesson.Caption, rackV2Objective));", "lesson.Caption, !rackV2Objective));"),
     {OBJECTIVES_ONLY, OBJECTIVES_FLAG, pin_tree(CODE, OBJECTIVES)}),
]

CASE_BY_ID = {case[0]: case for case in CASES}

# Benign controls: these must stay accepted, or the guard would be crying wolf. (id, description, mutation, extra arguments, expected marker)
BENIGN = [
    ("B01", "unmodified candidate", lambda r: None, (), "FIVE_RACK_SUITE_SCOPE_ONLY"),
    ("B02", "ignored Unity caches and generated project files outside the production folders",
     lambda r: [write(r, path, "x") for path in ("Library/x", "Temp/x", "obj/x", "Logs/x", "UserSettings/x", "Assembly-CSharp.csproj", "CCF.sln")], (), "FIVE_RACK_SUITE_SCOPE_ONLY"),
    ("B03", "extra newline in the one allowed #308 handoff", lambda r: append(r, HANDOFF_308, "\n"), (), "FIVE_RACK_SUITE_SCOPE_ONLY"),
    ("B04", "archive redirected to an absent path", lambda r: patch_checker(r, 'ARCHIVE = Path("/media/jer/Files/CCF")', 'ARCHIVE = Path("/nonexistent/ccf-archive")'),
     (), "FIVE_RACK_SUITE_INCOMPLETE"),
]


# ---- Weakening edits for the controls. Each applies to text of the pinned runner and must apply exactly once. ---------------------------------------

def edit(text, old, new):
    if text.count(old) != 1:
        raise RuntimeError(f"control setup: expected one occurrence of {old[:70]!r}, found {text.count(old)}")
    return text.replace(old, new)


def drop_table_entry(text, key):
    begin = text.index("    (" + json.dumps(key)[:-1])
    position, depth, quote = begin, 0, None
    while True:
        character = text[position]
        if quote:
            if character == "\\":
                position += 1
            elif character == quote:
                quote = None
        elif character in "\"'":
            quote = character
        elif character == "(":
            depth += 1
        elif character == ")":
            depth -= 1
            if depth == 0:
                break
        position += 1
    return text[:begin] + text[text.index("\n", position) + 1:]


STAGE3_SHOW = 'pinned = git_bytes("show", commit + ":" + path)'
STAGE3_HASH = "record(hashlib.sha256(pinned).hexdigest() == sha,"
STAGE3_TREE = "record(target.exists() and current == pinned and hashlib.sha256(current).hexdigest() == sha,"
STAGE3_ALL_OR_NOTHING = "record(present == sorted(STAGE_303),"
STAGE3_LOOP = "for path, (commit, sha) in STAGE_303.items():"
OUTSIDE = "outside = sorted(p for p in changed if p not in ALLOWED)"
PIN_TREE_CHECK = "record(current is not None and hashlib.sha256(current).hexdigest() == sha and current == pinned,"
PIN_HEAD_CHECK = "record(head_blob is not None and head_blob.decode().strip() == blob, label"
PIN_SHOW = 'pinned = git_quiet("show", commit + ":" + path)'
PIN_COMMIT_CHECK = "record(pinned is not None and pinned_blob is not None and pinned_blob.decode().strip() == blob and hashlib.sha256(pinned).hexdigest() == sha,"
ONE_PLACE_CHECK = '    record(len(re.findall(r"Plan\\.ObjectiveOneFigure\\(", painter)) == 1, "the painter builds the five-rack figure in exactly one place")\n'
IDENTITY_SOURCE_CHECK = "record(emitted == expected, "
TABLE_COUNT_CHECK = "record(len(MUTATIONS) == 15 and len(PAINTER_MUTATIONS) == 14 and len(MUTATIONS) + len(PAINTER_MUTATIONS) == EXPECTED_MUTATIONS,"
TABLE_DIGEST_CHECK = 'record(inventory is not None and hashlib.sha256(table).hexdigest() == inventory["mutation_table_sha256"],'
STRUCTURAL_CHECK = """record(structural_names == inventory["structural"] and len(structural_names) == EXPECTED_STRUCTURAL
           and STRUCTURAL_TALLY_LINE == ["FIVE_RACK_PASS checks=%d" % EXPECTED_STRUCTURAL],"""
PAINTER_CHECK = """record(painter_names == inventory["painter"] and len(painter_names) == EXPECTED_PAINTER
           and PAINTER_TALLY_LINE == ["FIVE_RACK_PAINTER_PASS checks=%d" % EXPECTED_PAINTER],"""
MUTATIONS_CHECK = """record(caught_mutations == inventory["mutation_names"] and len(caught_mutations) == EXPECTED_MUTATIONS,"""
ORIGINAL_CHECK = "record(len(original) == EXPECTED_ORIGINAL and not lost,"
NO_MUTATION_REASON = """return "FIVE_RACK_SUITE_INCOMPLETE", 2, " (--no-mutations: the 29 mutations did not run; not a full pass)\""""


def w1_head_following(text):
    return edit(edit(edit(text, STAGE3_SHOW, 'pinned = git_bytes("show", "HEAD:" + path)'), STAGE3_HASH, "record(True,"),
                STAGE3_TREE, "record(target.exists() and current == pinned,")


def w2_hash_removed(text):
    return edit(edit(text, STAGE3_HASH, "record(True,"), STAGE3_TREE, "record(target.exists() and current == pinned,")


def w3_partial_stage(text):
    return edit(edit(text, STAGE3_ALL_OR_NOTHING, "record(set(present) <= set(STAGE_303),"),
                STAGE3_LOOP, "for path, (commit, sha) in ((p, STAGE_303[p]) for p in present):")


def w4_headland_prefix(text):
    return edit(edit(text, OUTSIDE, OUTSIDE[:-1] + ' and not p.startswith("Tools/Verification/StandMap/HeadlandOffline/"))'),
                "record(set(standmap) <= STAGE_303_SCRIPTS,", 'record(all(p.startswith("Tools/Verification/StandMap/HeadlandOffline/") for p in standmap),')


def w5_guard_and_mutations(text):
    """Reviewer W5: one original guard deleted, four of the 29 mutations deleted, two mutation expectations emptied."""
    text = edit(text, ONE_PLACE_CHECK, "")
    for key in ("outer south band missing", "P11 property boundary dashed", "P13 field mark painted", "Q7 key row wraps"):
        text = drop_table_entry(text, key)
    text = edit(text, '"two continuous solid edges"),', '""),')
    return edit(text, '"title is the first element"),\n    ("P4', '""),\n    ("P4')


def forger(text):
    """W5 with every checker-side identity, tally and table comparison neutered: this copy prints the full PASS marker by itself."""
    text = w5_guard_and_mutations(text)
    for check in (IDENTITY_SOURCE_CHECK, TABLE_COUNT_CHECK, TABLE_DIGEST_CHECK, STRUCTURAL_CHECK, PAINTER_CHECK, MUTATIONS_CHECK, ORIGINAL_CHECK):
        text = edit(text, check, "record(True, ")
    return text


def all_pins_neutered(text):
    return edit(edit(edit(text, PIN_TREE_CHECK, "record(True,"), PIN_HEAD_CHECK, "record(True, label"), PIN_COMMIT_CHECK, "record(True,")


def not_caught(evaluation):
    return evaluation.exit == 2 and not evaluation.fails


def lost_assertion(name):
    """The checker still fails or passes, but the failing set misses the named assertion: it no longer holds the rule."""
    return lambda evaluation: name in evaluation.missing


def scope_identity_loses(*lost):
    return lambda evaluation: all(name in evaluation.names_missing for name in lost)


def fails_with(*names):
    return lambda evaluation: evaluation.exit == 1 and all(name in evaluation.fails for name in names)


def marker_is_full_pass(evaluation):
    return evaluation.exit == 0 and any(marker.startswith("FIVE_RACK_SUITE_PASS") for marker in evaluation.markers)


def prints_pass_but_is_rejected(evaluation):
    """The copy prints the full marker by itself, and the clean-run comparison still refuses it."""
    return marker_is_full_pass(evaluation) and bool(evaluation.clean_problems)


def structural_lost(evaluation):
    return bool(evaluation.structural_missing)


def painter_lost(evaluation):
    return bool(evaluation.painter_missing)


def forger_rejected(evaluation):
    """The copy prints its own full PASS. The independent comparison must still reject it by source count, lost names and lost mutations."""
    return (prints_pass_but_is_rejected(evaluation) and evaluation.source_count != TALLY_SOURCE_FULL and ONE_PLACE in evaluation.names_missing
            and sum(" is caught" in name for name in evaluation.names_missing) == 4)


# (id, description, weakening edit, [(case id or clean-<mode>, mode, predicate, what is proved)])
CONTROLS = [
    ("W1", "reviewer W1: #303 pin follows HEAD, no hash check", w1_head_following,
     [("S12", "scope-only", not_caught, "a committed #303 edit is no longer caught")]),
    ("W2", "reviewer W2: #303 SHA-256 comparisons removed", w2_hash_removed,
     [("F2a", "scope-only", not_caught, "a wrong recorded hash is no longer caught")]),
    ("W3", "reviewer W3: partial #303 stage accepted", w3_partial_stage,
     [("S10", "scope-only", fails_with(NAMES_SOURCE), "the runner's own name comparison rejects the checks the copy no longer runs"),
      ("S10", "scope-only", scope_identity_loses(stage3_blob(STAGE_TEST), stage3_bytes(STAGE_TEST)), "the independent comparison sees the two lost #303 checks"),
      ("F2b", "scope-only", scope_identity_loses(stage3_blob(STAGE_TEST), stage3_bytes(STAGE_TEST)), "the same for the committed deletion")]),
    ("W4", "reviewer W4: HeadlandOffline directory allowance", w4_headland_prefix,
     [("303d", "scope-only", not_caught, "an injected StandMap script is no longer caught")]),
    ("W5", "reviewer W5: guard deleted, four mutations deleted, two expectations emptied", w5_guard_and_mutations,
     [("clean-scope-only", "scope-only", fails_with(TABLE_COUNT, TABLE_DIGEST, NAMES_SOURCE), "the runner itself rejects it"),
      ("clean-scope-only", "scope-only", scope_identity_loses(ONE_PLACE), "the independent name comparison sees the lost guard")]),
    ("D4", "dispatch W4: all-or-nothing equality removed", lambda t: edit(t, STAGE3_ALL_OR_NOTHING, "record(True,"),
     [("S10", "scope-only", lost_assertion(STAGE3_ALL), "deleting a #303 script no longer fails the all-or-nothing rule"),
      ("S13", "scope-only", lost_assertion(STAGE3_ALL), "deleting the #303 handoff no longer fails the all-or-nothing rule")]),
    ("D5", "dispatch W5: allowance widened to the whole Tools/ tree", lambda t: edit(t, OUTSIDE, OUTSIDE[:-1] + ' and not p.startswith("Tools/"))'),
     [("F3g", "scope-only", not_caught, "the unrelated NextActionGuidance edit is no longer caught")]),
    ("A1", "#296 working-tree byte comparison removed", lambda t: edit(t, PIN_TREE_CHECK, "record(True,"),
     [("S03", "scope-only", not_caught, "a one-byte diagram change is no longer caught"), ("S04", "scope-only", not_caught, "the title margin is no longer caught"),
      ("S05", "scope-only", not_caught, "the false claim is no longer caught"), ("S23", "scope-only", not_caught, "the entrance size is no longer caught")]),
    ("A2", "#296 pin follows HEAD instead of the fixed commit",
     lambda t: edit(edit(edit(t, PIN_SHOW, 'pinned = git_quiet("show", "HEAD:" + path)'), PIN_TREE_CHECK, "record(current is not None and current == pinned,"),
                    PIN_HEAD_CHECK, "record(True, label").replace(PIN_COMMIT_CHECK, "record(True,"),
     [("F1c", "scope-only", not_caught, "a committed diagram edit is no longer caught")]),
    ("A3", "#296 HEAD blob check removed", lambda t: edit(t, PIN_HEAD_CHECK, "record(True, label"),
     [("F1c", "scope-only", lost_assertion(pin_head(CODE, DIAGRAM)), "a committed edit no longer fails the HEAD pin")]),
    ("A4", "ignored production path detector removed", lambda t: edit(t, "record(not ignored,", "record(True,"),
     [("S08", "scope-only", not_caught, "the hidden Assets script is no longer caught"), ("F4c", "scope-only", not_caught, "the info/exclude script is no longer caught")]),
    ("A5", "index flag detector removed", lambda t: edit(t, "record(not flagged,", "record(True,"),
     [("S09", "scope-only", not_caught, "the assume-unchanged edit is no longer caught"), ("F4e", "scope-only", not_caught, "the skip-worktree edit is no longer caught")]),
    ("A6", "folder prefix allowance restored", lambda t: edit(t, OUTSIDE, OUTSIDE[:-1] + " and not p.startswith(CHECKER_DIR))"),
     [("S17", "scope-only", lost_assertion(ONLY_ALLOWED), "the folder no longer fails the allowed-files rule")]),
    ("A7", "exact folder check removed as well as the allowance",
     lambda t: edit(edit(t, OUTSIDE, OUTSIDE[:-1] + " and not p.startswith(CHECKER_DIR))"), "record(folder == FIVE_RACK_FILES and not missing,", "record(True,"),
     [("S17", "scope-only", not_caught, "an unrelated binary is no longer caught"), ("F3a", "scope-only", not_caught, "an unrelated test is no longer caught")]),
    ("A8", "rename detection left on", lambda t: edit(t, 'set(git_paths("diff", "-z", "--no-renames", "--name-only", base)) |', 'set(git_paths("diff", "-z", "--name-only", base)) |'),
     [("F3n", "scope-only", not_caught, "a deleted production file hidden by a rename is no longer caught")]),
    ("A9", "committed changes no longer compared", lambda t: edit(t, 'changed = in_worktree | set(git_paths("diff", "-z", "--no-renames", "--name-only", base, "HEAD"))', "changed = in_worktree"),
     [("F3o", "scope-only", not_caught, "a committed rack edit with the working tree put back is no longer caught")]),
    ("A10", "integrity test pin removed", lambda t: edit(t, 'record(test is not None and sealed_digest(test, b"CHECKER_SHA256") == INTEGRITY_TEST_SHA256,', "record(True,"),
     [("S21", "scope-only", not_caught, "a loosened integrity test is no longer caught"), ("F3b", "scope-only", not_caught, "a replaced integrity test is no longer caught")]),
    ("A11", "self-pin removed", lambda t: edit(t, 'record(sealed_digest(Path(__file__).read_bytes(), b"SELF_SHA256") == SELF_SHA256,', "record(True,"),
     [("F3p", "scope-only", not_caught, "an unsealed runner edit is no longer caught")]),
    ("A12", "scope-only marker claims the full pass", lambda t: edit(t, 'return "FIVE_RACK_SUITE_SCOPE_ONLY", 2,', 'return "FIVE_RACK_SUITE_PASS", 0,'),
     [("clean-scope-only", "scope-only", prints_pass_but_is_rejected, "the copy prints the full marker and the clean-run comparison rejects it")]),
    ("A13", "no-mutations marker claims the full pass", lambda t: edit(t, NO_MUTATION_REASON, 'return "FIVE_RACK_SUITE_PASS", 0, ""'),
     [("verdict-table", "scope-only", None, "the verdict table rejects a pass without the mutations")]),
    ("A14", "map immutability assertion removed", lambda t: edit(t, 'PROTECTED = ["Assets/ForestPrototype/UI/Map/StandMapSource.cs", ', "PROTECTED = ["),
     [("clean-scope-only", "scope-only", fails_with(NAMES_SOURCE), "the runner itself rejects the lost name"),
      ("clean-scope-only", "scope-only", scope_identity_loses("unchanged: " + MAP), "the independent name comparison sees the lost map assertion")]),
    ("A15", "diagram assertion removed", lambda t: edit(t, '    record(not re.search(r"^\\s*using\\s+Unity", plan, re.M), "TeachingDiagramPlan stays Unity-free (offline-checkable)")\n', ""),
     [("clean-scope-only", "scope-only", scope_identity_loses("TeachingDiagramPlan stays Unity-free (offline-checkable)"), "the independent name comparison sees it")]),
    ("A16", "source identity comparison neutered and a guard deleted", lambda t: edit(edit(t, IDENTITY_SOURCE_CHECK, "record(True, "), ONE_PLACE_CHECK, ""),
     [("clean-scope-only", "scope-only", scope_identity_loses(ONE_PLACE), "the runner stays quiet, the independent comparison does not")]),
    ("A17", "recording failures as passes (keeps every name)", lambda t: edit(t, '    results.append(ok)\n    names.append(name)\n    print(("PASS " if ok else "FAIL ")', '    ok = True\n    results.append(ok)\n    names.append(name)\n    print(("PASS " if ok else "FAIL ")'),
     [("303a", "scope-only", not_caught, "behaviour no longer rejects anything; only the runner's byte pin can object")]),
    # Second layer: the byte pins are switched off, so the exact C# tallies must hold on their own. These run the substantive suite.
    ("L1", "#296 pins off, structural check deleted", all_pins_neutered,
     [("S18", "no-mutations", fails_with(NAMES_STRUCTURAL), "the runner's exact 109 structural comparison rejects it")]),
    ("L2", "#296 pins off, painter assertion deleted", all_pins_neutered,
     [("S19", "no-mutations", fails_with(NAMES_PAINTER), "the runner's exact 165 painter comparison rejects it")]),
    ("L3", "#296 pins and the structural comparison off, structural check deleted", lambda t: edit(all_pins_neutered(t), STRUCTURAL_CHECK, "record(True, "),
     [("S18", "no-mutations", structural_lost, "the independent comparison sees the lost structural assertion")]),
    ("L4", "#296 pins and the painter comparison off, painter assertion deleted", lambda t: edit(all_pins_neutered(t), PAINTER_CHECK, "record(True, "),
     [("S19", "no-mutations", painter_lost, "the independent comparison sees the lost painter assertion")]),
    # The full-mode proof: this copy runs everything it still has and prints FIVE_RACK_SUITE_PASS by itself.
    ("L5", "reviewer W5 with every checker-side comparison neutered, full mode", forger,
     [("clean-full", "full", forger_rejected, "the weakened copy's own PASS has a different source count and loses the guard and 4 caught mutations")]),
]


class Observed:
    """What one checker run printed, reduced to the facts the comparisons use."""

    def __init__(self, result):
        lines = result.stdout.splitlines()
        self.exit = result.returncode
        self.stdout = result.stdout
        self.stderr = result.stderr
        self.names = [line.split(" ", 2)[2].split(" :: ")[0] for line in lines if line.startswith(("PASS SOURCE ", "FAIL SOURCE "))]
        self.fails = {line.split(" ", 2)[2].split(" :: ")[0] for line in lines if line.startswith("FAIL SOURCE ")}
        self.structural = [line.split(" ", 1)[1].split(" :: ")[0] for line in lines
                           if line.startswith(("PASS ", "FAIL ")) and not line.startswith(("PASS SOURCE ", "FAIL SOURCE ", "PASS PAINTER ", "FAIL PAINTER "))]
        self.painter = [line.split(" ", 2)[2].split(" :: ")[0] for line in lines if line.startswith(("PASS PAINTER ", "FAIL PAINTER "))]
        self.markers = [line for line in lines if line.startswith("FIVE_RACK_SUITE_")]
        self.tally_lines = [line for line in lines if line.startswith(("FIVE_RACK_PASS", "FIVE_RACK_FAIL", "FIVE_RACK_PAINTER_"))]


class Evaluation:
    """An Observed run compared with what the unmodified checker should have done."""

    def __init__(self, observed, expected_fails, expected_names, inventory, clean_problems):
        self.clean_problems = clean_problems
        self.exit = observed.exit
        self.fails = observed.fails
        self.markers = observed.markers
        self.missing = expected_fails - observed.fails
        self.extra = observed.fails - expected_fails
        self.source_count = len(observed.names)
        self.names_missing = [name for name in expected_names if name not in observed.names]
        self.names_extra = [name for name in observed.names if name not in expected_names]
        self.structural_missing = [name for name in inventory["structural"] if name not in observed.structural]
        self.painter_missing = [name for name in inventory["painter"] if name not in observed.painter]


def expected_names(inventory, mode):
    names = list(inventory["source_scope"])
    if mode != "scope-only":
        names += inventory["source_substantive"]
    if mode == "full":
        names += inventory["source_mutations"]
    return names + inventory["source_final"][mode]


def clean_problems(observed, mode, inventory, commit):
    """Everything wrong with a run that should be a clean run of this mode. Empty means the exact identities, tallies and marker all hold."""
    wanted = expected_names(inventory, mode)
    problems = []
    if f"RUN_IDENTITY mode={mode} head={commit} " not in observed.stdout:
        problems.append(f"no RUN_IDENTITY line for mode {mode} and head {commit}")
    if observed.names != wanted:
        problems.append(f"source names differ: {len(observed.names)} ran, {len(wanted)} expected; missing {[n for n in wanted if n not in observed.names][:3]}; "
                        f"unexpected {[n for n in observed.names if n not in wanted][:3]}")
    if observed.fails:
        problems.append(f"FAIL lines: {sorted(observed.fails)[:3]}")
    if mode == "full":
        line = f"FIVE_RACK_SUITE_PASS source_checks={TALLY_SOURCE_FULL} failed=0"
        if observed.exit != 0 or observed.markers != [line]:
            problems.append(f"exit {observed.exit}, markers {observed.markers}, wanted exactly {line!r}")
        caught = [name for name in observed.names if name.endswith("' is caught")]
        if len(caught) != TALLY_MUTATIONS:
            problems.append(f"{len(caught)} mutations caught, wanted {TALLY_MUTATIONS}")
        superseded = inventory["original_131_superseded"]
        lost = [n for n in inventory["original_131"] if n not in observed.names and not (n in superseded and all(s in observed.names for s in superseded[n]))]
        if lost:
            problems.append(f"original obligations lost: {lost[:3]}")
    else:
        wanted_marker = "FIVE_RACK_SUITE_SCOPE_ONLY" if mode == "scope-only" else "FIVE_RACK_SUITE_INCOMPLETE"
        if observed.exit != 2 or len(observed.markers) != 1 or not observed.markers[0].startswith(wanted_marker + " "):
            problems.append(f"exit {observed.exit}, markers {observed.markers}, wanted exit 2 and {wanted_marker}")
        if "FIVE_RACK_SUITE_PASS" in observed.stdout:
            problems.append("a partial run printed the full PASS marker")
    if mode != "scope-only":
        if observed.structural != inventory["structural"]:
            problems.append(f"structural names differ: {len(observed.structural)} ran")
        if observed.painter != inventory["painter"]:
            problems.append(f"painter names differ: {len(observed.painter)} ran")
        for line in (f"FIVE_RACK_PASS checks={TALLY_STRUCTURAL}", f"FIVE_RACK_PAINTER_PASS checks={TALLY_PAINTER}"):
            if line not in observed.tally_lines:
                problems.append(f"missing {line!r}, got {observed.tally_lines}")
    return problems


def verdict_table_problems(path):
    """Every combination of run facts through the checker's own verdict function: exactly one may be the full pass."""
    spec = importlib.util.spec_from_file_location("five_rack_under_test", path)
    module = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(module)
    verdict = getattr(module, "suite_verdict", None)
    if verdict is None:
        return ["the checker has no suite_verdict function"]
    problems = []
    for mode in ("full", "no-mutations", "scope-only"):
        for flags in itertools.product((False, True), repeat=6):
            all_passed, snapshot_not_run, canonical, ran_to_end, identities, full_ran = flags
            marker, code, _ = verdict(all_passed, snapshot_not_run, mode, canonical, ran_to_end, identities, full_ran)
            is_full = (all_passed and not snapshot_not_run and mode == "full" and canonical and ran_to_end and identities and full_ran)
            if is_full != (marker == "FIVE_RACK_SUITE_PASS") or is_full != (code == 0):
                problems.append(f"{mode} {flags} gave {marker} exit {code}")
            if not all_passed and (marker != "FIVE_RACK_SUITE_FAIL" or code != 1):
                problems.append(f"{mode} {flags} should fail, gave {marker} exit {code}")
    return problems


def integrity_marker(failure_count, partial):
    """The integrity test's own marker. A run that left anything out can never print the PASS marker."""
    if failure_count:
        return "STAGE_INVENTORY_INTEGRITY_FAIL", 1
    if partial:
        return "STAGE_INVENTORY_INTEGRITY_PARTIAL", 2
    return "STAGE_INVENTORY_INTEGRITY_PASS", 0


class Lane:
    """The disposable clone and the runs made in it."""

    def __init__(self, clone, commit, work):
        self.clone, self.commit = clone, commit
        self.logs = work / "logs"
        self.out = work / "out"
        self.runs = 0

    def reset(self, checker):
        flagged = [line[2:] for line in git(self.clone, "ls-files", "-v").splitlines() if not line.startswith("H ")]
        for path in flagged:
            # Two calls: update-index applies only the first of these two options when both are given on one line.
            git(self.clone, "update-index", "--no-assume-unchanged", path)
            git(self.clone, "update-index", "--no-skip-worktree", path)
        git(self.clone, "reset", "-q", "--hard", self.commit)
        git(self.clone, "clean", "-q", "-fdx")
        (self.clone / ".git/info/exclude").write_text("")
        write(self.clone, CHECKER_IN_REPO, seal_checker(checker))

    def run(self, label, mode, extra=()):
        self.runs += 1
        name = f"{self.runs:03d}-{label}"
        arguments = [sys.executable, "-I", "-B", str(self.clone / CHECKER_IN_REPO), "--out", str(self.out / name)]
        if mode == "scope-only":
            arguments.append("--scope-only")
        elif mode == "no-mutations":
            arguments.append("--no-mutations")
        result = subprocess.run(arguments + list(extra), capture_output=True, text=True)
        (self.logs / (name + ".log")).write_text(f"exit={result.returncode}\n--- stdout\n{result.stdout}--- stderr\n{result.stderr}")
        if (self.out / name).exists():
            shutil.rmtree(self.out / name)  # the dotnet projects of a full run are large; the log above is the evidence
        return Observed(result)


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    stamp = datetime.datetime.now(datetime.timezone.utc).strftime("%Y%m%dT%H%M%SZ")
    parser.add_argument("--checker", type=Path, default=HERE / "run_five_rack_check.py", help="checker file under test")
    parser.add_argument("--work", type=Path, default=EVIDENCE / ("integrity-" + stamp), help="new or empty folder outside the repository")
    parser.add_argument("--commit", default=None, help="commit to clone (default: HEAD of this checkout)")
    parser.add_argument("--skip-clean", action="store_true", help="partial: skip the clean runs; prints the PARTIAL marker")
    parser.add_argument("--skip-controls", action="store_true", help="partial: skip the weakened-checker controls; prints the PARTIAL marker")
    parser.add_argument("--only", nargs="*", default=None, help="partial: run only these case ids; prints the PARTIAL marker")
    parser.add_argument("--reuse-clone", type=Path, default=None, help="test-only: use and keep an existing clone (the own-marker check uses this)")
    arguments = parser.parse_args()
    work = arguments.work.resolve()
    if work.is_relative_to(ROOT) or ROOT.is_relative_to(work):
        raise SystemExit("work directory must be outside the repository")
    if work.exists() and any(work.iterdir()):
        raise SystemExit("--work exists and is not empty; choose a new folder: " + str(work))
    (work / "logs").mkdir(parents=True, exist_ok=True)
    partial = arguments.skip_clean or arguments.skip_controls or arguments.only is not None
    failures = []

    def fail(what):
        failures.append(what)

    inventory_bytes = (HERE / "expected_assertions.json").read_bytes()
    inventory_ok = hashlib.sha256(inventory_bytes).hexdigest() == ASSERTIONS_SHA256
    print(f"{'PASS' if inventory_ok else 'FAIL'} PIN expected_assertions.json matches ASSERTIONS_SHA256", flush=True)
    if not inventory_ok:
        fail("inventory pin")
    inventory = json.loads(inventory_bytes)
    tallies_ok = (len(inventory["structural"]) == TALLY_STRUCTURAL and len(inventory["painter"]) == TALLY_PAINTER
                  and len(inventory["mutation_names"]) == TALLY_MUTATIONS and len(inventory["original_131"]) == TALLY_ORIGINAL
                  and len(expected_names(inventory, "scope-only")) == TALLY_SOURCE_SCOPE
                  and len(expected_names(inventory, "no-mutations")) == TALLY_SOURCE_NO_MUTATIONS
                  and len(expected_names(inventory, "full")) == TALLY_SOURCE_FULL)
    print(f"{'PASS' if tallies_ok else 'FAIL'} PIN inventory holds exactly {TALLY_STRUCTURAL} structural, {TALLY_PAINTER} painter, {TALLY_MUTATIONS} mutations, "
          f"{TALLY_ORIGINAL} original obligations and {TALLY_SOURCE_SCOPE}/{TALLY_SOURCE_NO_MUTATIONS}/{TALLY_SOURCE_FULL} source names", flush=True)
    if not tallies_ok:
        fail("inventory tallies")
    checker_bytes = arguments.checker.resolve().read_bytes()
    checker_ok = hashlib.sha256(checker_bytes).hexdigest() == CHECKER_SHA256
    print(f"{'PASS' if checker_ok else 'FAIL'} PIN checker under test matches CHECKER_SHA256 ({hashlib.sha256(checker_bytes).hexdigest()})", flush=True)
    if not checker_ok:
        fail("checker pin")

    commit = arguments.commit or git(ROOT, "rev-parse", "HEAD").strip()
    store = Path(git(ROOT, "rev-parse", "--path-format=absolute", "--git-common-dir").strip())
    clone = arguments.reuse_clone.resolve() if arguments.reuse_clone else work / "clone"
    if not arguments.reuse_clone:
        subprocess.run(["git", "clone", "-q", "--shared", "--no-checkout", str(store), str(clone)], check=True)
        # A full checkout on purpose: sparse checkout sets skip-worktree, which the checker rightly rejects.
        git(clone, "checkout", "-q", "--detach", commit)
    print(f"CLONE {clone} COMMIT {commit} CHECKER {arguments.checker.resolve()}", flush=True)
    lane = Lane(clone, commit, work)
    pinned_runner = subprocess.run(["git", "-C", str(clone), "show", f"{commit}:{CHECKER_IN_REPO}"], check=True, capture_output=True).stdout
    # The runner pins the committed copy of this test. This line closes the gap to the copy actually running, so an uncommitted edit here cannot pass.
    own_copy = subprocess.run(["git", "-C", str(clone), "show", f"{commit}:{TEST_IN_REPO}"], capture_output=True).stdout
    own_ok = own_copy == Path(__file__).resolve().read_bytes()
    print(f"{'PASS' if own_ok else 'FAIL'} PIN this running integrity test equals its copy at the commit under test", flush=True)
    if not own_ok:
        fail("own copy")

    def clean_run(label, mode):
        lane.reset(checker_bytes)
        observed = lane.run(label, mode)
        problems = clean_problems(observed, mode, inventory, commit)
        print(f"{'PASS' if not problems else 'FAIL'} CLEAN {label}: exit={observed.exit} source={len(observed.names)} structural={len(observed.structural)} "
              f"painter={len(observed.painter)} markers={observed.markers} {problems or ''}", flush=True)
        if problems:
            fail(label)
        return not problems

    def evaluate_case(case_id, checker, mode):
        """Apply one case to the given checker. Returns (Observed, Evaluation) so a control can read what happened."""
        lane.reset(checker)
        if case_id.startswith("clean-"):
            expected_fails = set()
        else:
            case = CASE_BY_ID[case_id]
            case[2](clone)
            expected_fails = case[3]
        observed = lane.run(case_id, mode)
        return observed, Evaluation(observed, expected_fails, expected_names(inventory, mode), inventory, clean_problems(observed, mode, inventory, commit))

    def judge_case(case):
        case_id, description, _, expected = case[:4]
        options = case[4] if len(case) > 4 else ""
        observed, evaluation = evaluate_case(case_id, checker_bytes, "scope-only")
        fails_ok = expected <= observed.fails if options == "subset" else observed.fails == expected
        exit_ok = observed.exit == 1
        marker_ok = len(observed.markers) == 1 and observed.markers[0].startswith("FIVE_RACK_SUITE_FAIL ")
        # A crash stops the run, and another commit has other names. Everywhere else the failing run must still have run every check by name.
        identity_ok = options in ("crash", "subset") or (not evaluation.names_missing and not evaluation.names_extra)
        if exit_ok and fails_ok and marker_ok and identity_ok:
            print(f"PASS CAUGHT {case_id} {description}", flush=True)
            for name in sorted(expected):
                print(f"    {name}", flush=True)
        else:
            verdict = "NOT CAUGHT" if not observed.fails else "CAUGHT BY WRONG ASSERTION"
            print(f"FAIL {verdict} {case_id} {description}\n    exit={observed.exit} markers={observed.markers}\n    missing={sorted(evaluation.missing)}\n"
                  f"    extra={sorted(evaluation.extra)}\n    names_missing={evaluation.names_missing[:3]} names_extra={evaluation.names_extra[:3]}\n"
                  f"    stderr={observed.stderr.strip()[-300:]}", flush=True)
            fail(case_id)

    def judge_benign(case):
        case_id, description, mutate, extra, marker = case
        lane.reset(checker_bytes)
        mutate(clone)
        observed = lane.run(case_id, "scope-only", extra)
        ok = observed.exit == 2 and not observed.fails and len(observed.markers) == 1 and observed.markers[0].startswith(marker + " ")
        print(f"{'PASS' if ok else 'FAIL'} BENIGN {case_id} {description}: exit={observed.exit} markers={observed.markers} fails={sorted(observed.fails)[:3]}", flush=True)
        if not ok:
            fail(case_id)

    def judge_control(control):
        control_id, description, weaken, expectations = control
        try:
            weak_text = weaken(pinned_runner.decode())
            weak = seal_checker(weak_text.encode())
            compile(weak_text, control_id, "exec")
        except Exception as error:  # a broken setup must never look like a detection
            print(f"FAIL CONTROL {control_id} setup: {error}", flush=True)
            fail("control " + control_id)
            return
        pin_differs = hashlib.sha256(weak).hexdigest() != CHECKER_SHA256
        results = []
        for target, mode, predicate, proved in expectations:
            if target == "verdict-table":
                lane.reset(weak)
                problems = verdict_table_problems(clone / CHECKER_IN_REPO)
                results.append((bool(problems), f"verdict-table: {proved} ({len(problems)} wrong combinations)"))
                continue
            _, evaluation = evaluate_case(target, weak, mode)
            results.append((predicate(evaluation), f"{target}/{mode}: {proved} [exit={evaluation.exit} fails={len(evaluation.fails)} "
                                                    f"missing={sorted(evaluation.missing)[:1]} names_missing={evaluation.names_missing[:1]}]"))
        ok = pin_differs and all(detected for detected, _ in results)
        print(f"{'PASS' if ok else 'FAIL'} CONTROL {control_id} {description}; byte pin rejects it: {pin_differs}", flush=True)
        for detected, text in results:
            print(f"    {'detected' if detected else 'NOT DETECTED'} {text}", flush=True)
        if not ok:
            fail("control " + control_id)

    clean_count = 0
    if not arguments.skip_clean:
        clean_run("clean-before", "full")
        clean_count += 1
    cases = CASES if arguments.only is None else [case for case in CASES if case[0] in arguments.only]
    for case in cases:
        judge_case(case)
    benign_count = 0
    if arguments.only is None:
        for case in BENIGN:
            judge_benign(case)
        benign_count = len(BENIGN)
    if not arguments.skip_clean:
        clean_run("clean-after", "no-mutations")
        clean_run("clean-scope", "scope-only")
        clean_count += 2

    lane.reset(checker_bytes)
    problems = verdict_table_problems(clone / CHECKER_IN_REPO)
    print(f"{'PASS' if not problems else 'FAIL'} VERDICT TABLE: FIVE_RACK_SUITE_PASS only for a complete full run; 192 fact combinations; {problems[:3] or ''}", flush=True)
    if problems:
        fail("verdict table")

    control_count = 0
    if not arguments.skip_controls:
        for control in CONTROLS:
            judge_control(control)
        control_count = len(CONTROLS)

    # The integrity test's own partial switches must not be able to print its PASS marker.
    own = [integrity_marker(0, True), integrity_marker(0, False), integrity_marker(1, True), integrity_marker(1, False)]
    table_ok = own == [("STAGE_INVENTORY_INTEGRITY_PARTIAL", 2), ("STAGE_INVENTORY_INTEGRITY_PASS", 0), ("STAGE_INVENTORY_INTEGRITY_FAIL", 1), ("STAGE_INVENTORY_INTEGRITY_FAIL", 1)]
    # Reports only the result: printing the marker texts here would put the PASS marker in the output of a partial run.
    print(f"{'PASS' if table_ok else 'FAIL'} OWN MARKER TABLE: a failure is always FAIL, a partial run is never the PASS marker", flush=True)
    if not table_ok:
        fail("own marker table")
    if not partial:
        lane.reset(checker_bytes)
        result = subprocess.run([sys.executable, "-I", "-B", str(Path(__file__).resolve()), "--skip-clean", "--skip-controls", "--only", "303a",
                                 "--reuse-clone", str(clone), "--commit", commit, "--checker", str(arguments.checker.resolve()), "--work", str(work / "own-partial-run")],
                                capture_output=True, text=True)
        ok = result.returncode == 2 and "STAGE_INVENTORY_INTEGRITY_PARTIAL" in result.stdout and "STAGE_INVENTORY_INTEGRITY_PASS" not in result.stdout
        print(f"{'PASS' if ok else 'FAIL'} OWN PARTIAL RUN --skip-clean --skip-controls --only 303a: exit={result.returncode}, PARTIAL marker and no PASS marker", flush=True)
        if not ok:
            print(result.stdout[-800:] + result.stderr[-400:], flush=True)
            fail("own partial run")

    if not arguments.reuse_clone:
        shutil.rmtree(clone)
    print(f"STAGE_INVENTORY_INTEGRITY_TALLY MUTATIONS={len(cases)} BENIGN={benign_count} CLEAN_RUNS={clean_count} CONTROLS={control_count} FAIL={len(failures)}")
    marker, code = integrity_marker(len(failures), partial)
    if failures:
        print(marker + ": " + "; ".join(failures), flush=True)
    elif partial:
        print(marker + " (limited run: not a full integrity pass); logs " + str(lane.logs), flush=True)
    else:
        print(marker + "; logs " + str(lane.logs), flush=True)
    return code


if __name__ == "__main__":
    sys.exit(main())
