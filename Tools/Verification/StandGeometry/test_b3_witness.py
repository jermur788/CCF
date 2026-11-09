#!/usr/bin/env python3
"""Offline checks for the #347 B3 evidence witness. No Unity, no Git writes, no network.

Run: python3 -I -B Tools/Verification/StandGeometry/test_b3_witness.py
These are static/source checks. They do NOT compile the C# in the Unity Editor and prove nothing about runtime values.
"""
import importlib.util, re, subprocess, sys, unittest
from pathlib import Path

HERE = Path(__file__).resolve().parent
spec = importlib.util.spec_from_file_location("run_enlarged80", HERE / "run_enlarged80.py")
runner = importlib.util.module_from_spec(spec); spec.loader.exec_module(runner)
HARNESS = (HERE / "Enlarged80Verification.cs").read_text(encoding="utf-8")


def good_log():
    lines = ["ENLARGED80_B3W seq=%d frame=1 mode=plain label=t %s" % (i, req) for i, req in enumerate(runner.B3_REQUIRED)]
    lines[-1] = "ENLARGED80_B3W seq=99 frame=9 mode=plain label=t kind=SUMMARY resets=3 witness=EVIDENCE_COMPLETE b3Verdict=NOT_DECIDED_BY_HARNESS"
    return lines


class RunnerStatus(unittest.TestCase):
    def test_complete_log_is_complete_but_never_a_verdict(self):
        r = runner.b3_status(good_log())
        self.assertEqual(r["status"], "EVIDENCE_COMPLETE"); self.assertEqual(r["b3_verdict"], "NOT_DECIDED_BY_HARNESS")

    def test_every_required_kind_is_needed(self):
        for i, req in enumerate(runner.B3_REQUIRED[:-1]):
            log = good_log(); del log[i]
            r = runner.b3_status(log)
            self.assertEqual(r["status"], "EVIDENCE_INCOMPLETE", req); self.assertIn(req, r["missing_record_kinds"])

    def test_unavailable_or_harness_incomplete_or_no_summary_is_incomplete(self):
        log = good_log() + ["ENLARGED80_B3W seq=100 frame=9 mode=plain label=t kind=UNAVAILABLE what=x reason=y"]
        self.assertEqual(runner.b3_status(log)["status"], "EVIDENCE_INCOMPLETE")
        log = good_log(); log[-1] = log[-1].replace("EVIDENCE_COMPLETE", "EVIDENCE_INCOMPLETE")
        self.assertEqual(runner.b3_status(log)["status"], "EVIDENCE_INCOMPLETE")
        r = runner.b3_status([]); self.assertEqual((r["status"], r["harness_summary"]), ("EVIDENCE_INCOMPLETE", "NO_SUMMARY"))

    def test_non_b3_lines_are_ignored(self):
        self.assertEqual(runner.b3_status(good_log() + ["ENLARGED80_CHECK something"])["status"], "EVIDENCE_COMPLETE")

    def test_bad_arguments_are_refused_before_any_unity_work(self):
        for argv in (["x", "--b3-mode", "other"], ["x", "--candidate-sha", "abc"], ["x", "--candidate-sha", "A" * 40]):
            old = sys.argv; sys.argv = ["run_enlarged80.py"] + argv
            try:
                with self.assertRaises(SystemExit): runner.main()
            finally:
                sys.argv = old


class HarnessSource(unittest.TestCase):
    """Section 4d items, as strings the harness must contain. Source presence only."""
    ITEMS = {
        1: ["B3Identity(\"pre\")", "B3Identity(\"post\")", "GetActiveScene().path", "B3GitBlobId", "packages-lock.json", "PackageCache", "Application.unityVersion", "DateTime.UtcNow"],
        2: ["ccEnabled=", "ccCenter=", "ccBoundsCenter=", "autoSyncTransforms", "harness-start", "-entry model=", "-pre-Generate model=", "pre-TryBeginReferencePreview",
            "post-TryBeginReferencePreview", "post-EndReferencePreview+0", "post-EndReferencePreview+1", "post-EndReferencePreview+2"],
        3: ["B3Anchors()", "\"Dirt Path\"", "Forest Clearing", "coreSlotsInside=", "CLEARANCE_OMITTED_CORE", "listSha256="],
        4: ["B3(\"TREES\"", "idsSha256=", "worldHash=", "clearanceOmitted=", "hashBudgetOmitted="],
        5: ["e.SimulationSeed", "e.RngModelVersion", "UnityEngine.Random.state", "UNAVAILABLE"],
        6: ["B3Step(b3Reset, 1, \"destroy trees\")", "B3Step(b3Reset, 2, \"ApplyStandGeometry\")", "B3Step(b3Reset, 3, \"ResetForDeterministicRun\")",
            "B3Step(b3Reset, 4, \"Generate\")", "B3Step(b3Reset, 5, \"InitializeNewScenario\")"],
        7: ["ScenarioOneReferenceFutureV1.bytes", "LEGACY40_ANCHORS", "Reference Future v1 / Legacy40 anchors changed during the run"],
        8: ["authored-pin", "B3PinPlayer", "CCF_B3_MODE"],
        9: ["EVIDENCE_COMPLETE", "EVIDENCE_INCOMPLETE", "NOT_DECIDED_BY_HARNESS"],
    }

    def test_each_protocol_item_has_its_hooks(self):
        for item, needles in self.ITEMS.items():
            for n in needles:
                self.assertTrue(n in HARNESS, "section 4d item %d is missing %r" % (item, n))

    def test_generation_calls_keep_their_original_order(self):
        order = ["e.ApplyStandGeometry(model);", "e.ResetForDeterministicRun();", "stand.Generate();", "m.InitializeNewScenario();", "e.InvalidateCompetition();"]
        pos = [HARNESS.index(o) for o in order]
        self.assertEqual(pos, sorted(pos))

    def test_witness_block_only_observes(self):
        block = HARNESS[HARNESS.index("B3 evidence-only witness"):HARNESS.index("private ForestPlayer player()")]
        block = re.sub(r"//[^\n]*", "", block)   # comments may name the production calls they describe
        for forbidden in ("File.Write", "WriteAllText", "WriteAllBytes", "File.Delete", "SaveData", ".Save(", "LoadData(", "SetRetiredFromStartingWorld", "ApplyStandGeometry(", "Generate()"):
            self.assertTrue(forbidden not in block, "witness block contains " + forbidden)
        # the only transform write in the witness is the authored-start pin
        self.assertEqual(len(re.findall(r"transform\.position\s*=[^=]", block)), 1)
        self.assertTrue("if (b3Mode == \"authored-pin\")" in block)

    def test_scope_is_verification_only(self):
        # Fails closed if git or the base object is unavailable so a missing check is never a silent pass.
        out = subprocess.run(["git", "-C", str(HERE), "diff", "--name-only", "46bc1fd618ae5b214d7de296a03ac7efec27cb7c", "--", "."], capture_output=True, text=True)
        self.assertEqual(out.returncode, 0, out.stderr)
        top = subprocess.run(["git", "-C", str(HERE), "rev-parse", "--show-toplevel"], capture_output=True, text=True).stdout.strip()
        changed = subprocess.run(["git", "-C", top, "diff", "--name-only", "46bc1fd618ae5b214d7de296a03ac7efec27cb7c"], capture_output=True, text=True).stdout.split()
        self.assertTrue(changed)
        for path in changed:
            self.assertTrue(path.startswith("Tools/Verification/"), path)


if __name__ == "__main__":
    unittest.main(verbosity=2)
