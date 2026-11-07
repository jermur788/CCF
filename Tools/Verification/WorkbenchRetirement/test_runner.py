"""Offline admission/false-green/cleanup controls. No Unity process or real licence is used."""
import contextlib
import importlib.util
import io
import json
import os
from pathlib import Path
import shutil
import tempfile
import unittest
from unittest.mock import patch

SPEC = importlib.util.spec_from_file_location("runner", Path(__file__).with_name("run_workbench_retirement.py"))
runner = importlib.util.module_from_spec(SPEC)
SPEC.loader.exec_module(runner)
PASS = "WORKBENCH_RETIREMENT_VERIFY_PASS"
SHA = "a" * 40


class RunnerControls(unittest.TestCase):
    def setUp(self):
        scratch = Path(os.environ.get("CCF_TEST_SCRATCH", "/media/jer/Files/CCF/Verification/Workbench287MorningClosure-20261107/additions"))
        scratch.mkdir(parents=True, exist_ok=True)
        self.temp = tempfile.TemporaryDirectory(dir=scratch)
        self.addCleanup(self.temp.cleanup)
        self.base = Path(self.temp.name)
        self.root = self.base / "lane"
        self.source = self.root / "Tools/Verification/WorkbenchRetirement/WorkbenchRetirementVerification.cs"
        self.source.parent.mkdir(parents=True)
        self.source.write_text("// mock source\n")
        (self.root / "Assets/ForestPrototype").mkdir(parents=True)
        self.home = self.base / "home"
        self.lic = self.home / ".config/unity3d/Unity/licenses"
        self.lic.mkdir(parents=True)
        (self.lic / "UnityEntitlementLicense.xml").write_text("MOCK-NOT-A-LICENCE")
        (self.lic / "packages").mkdir()
        (self.lic / "packages/mock").write_text("MOCK-NESTED")
        self.out = self.base / "evidence"
        self.metadata = dict(head=SHA, branch="candidate", lane=str(self.root), task_id="287", owner="test", lock_id="test-lock")
        self.status = dict(status="held", metadata=self.metadata, unity_editor_pids=[])
        self.dirty = ""
        self.branch = "candidate"
        self.actual_head = SHA
        self.launched = False

    def output(self, cmd, cwd=None):
        if cmd[0] != "git":
            return json.dumps(self.status)
        if cmd[1] == "rev-parse": return self.actual_head
        if cmd[1] == "branch": return self.branch
        if cmd[1] == "status": return self.dirty
        if cmd[1] == "ls-files": return str(self.source)
        raise AssertionError(cmd)

    def run_case(self, text=PASS + " checks=1\n", editor_code=0, hash_error=False, cleanup_error=False):
        def editor(cmd, **kwargs):
            self.assertEqual(cmd[0], runner.UNITY)
            self.launched = True
            Path(cmd[cmd.index("-logFile") + 1]).write_text(text)
            Path(str(self.root / "Assets/ForestPrototype" / self.source.name) + ".meta").write_text("mock meta")
            return type("Result", (), dict(returncode=editor_code))()
        real_rmtree = shutil.rmtree
        def cleanup(path, *args, **kwargs):
            if cleanup_error and Path(path).name == "licenses":
                raise PermissionError("mock refusal")
            return real_rmtree(path, *args, **kwargs)
        hashes = runner.licence_hashes
        def hash_copy(path):
            if hash_error: raise IsADirectoryError("mock N3")
            return hashes(path)
        stdout = io.StringIO()
        with patch.object(runner, "ROOT", self.root), patch.object(runner, "EVIDENCE", self.base), \
                patch.object(runner, "output", self.output), patch.object(runner.Path, "home", return_value=self.home), \
                patch.object(runner.subprocess, "run", editor), patch.object(runner, "licence_hashes", hash_copy), \
                patch.object(runner.shutil, "rmtree", cleanup), contextlib.redirect_stdout(stdout):
            code = runner.main(["WorkbenchRetirementVerification", str(self.source), str(self.out),
                                "--expected-head", SHA, "--expected-branch", "candidate", "--owner", "test",
                                "--lock-id", "test-lock", "--pass-token", PASS])
        receipt = json.loads((self.out / "WorkbenchRetirementVerification.json").read_text())
        self.assertEqual(code, receipt["exit_code"])
        self.assertEqual(code, json.loads(stdout.getvalue())["exit_code"])
        self.assertTrue((self.out / "WorkbenchRetirementVerification.markers.txt").exists())
        self.assertFalse((self.root / "Assets/ForestPrototype" / self.source.name).exists())
        self.assertFalse(Path(str(self.root / "Assets/ForestPrototype" / self.source.name) + ".meta").exists())
        self.assertNotIn("MOCK-NOT-A-LICENCE", stdout.getvalue())
        self.assertNotIn("MOCK-NESTED", stdout.getvalue())
        if not cleanup_error:
            self.assertFalse((self.out / "config/unity3d/Unity/licenses").exists())
        print(self.id().split(".")[-1] + " mocked_runner_stdout=" + stdout.getvalue().strip())
        return code, receipt

    def test_N1_zero_exit_without_terminal_marker(self):
        code, receipt = self.run_case("WORKBENCH_PASS intermediate\n")
        self.assertEqual(code, 1)
        self.assertIn("missing_terminal_pass", receipt["failures"])

    def test_N2_zero_exit_with_FAIL_and_PASS(self):
        code, receipt = self.run_case("WORKBENCH_FAIL case\n" + PASS + " checks=1\n")
        self.assertEqual(code, 1)
        self.assertIn("fail_marker", receipt["failures"])

    def test_N3_nested_licence_directory_is_safe(self):
        code, receipt = self.run_case()
        self.assertEqual(code, 0)
        self.assertEqual(len(receipt["licence_sha_prefixes"]), 2)

    def test_N3_hash_IsADirectoryError_still_deletes_and_writes_receipts(self):
        code, receipt = self.run_case(hash_error=True)
        self.assertEqual(code, 1)
        self.assertIn("licence_hash_error:IsADirectoryError", receipt["failures"])

    def test_cleanup_refusal_fails_with_both_receipts(self):
        code, receipt = self.run_case(cleanup_error=True)
        self.assertEqual(code, 1)
        self.assertIn("licence_cleanup_error:PermissionError", receipt["failures"])

    def test_compiler_error_with_PASS_fails(self):
        code, receipt = self.run_case("error CS1002: mock\n" + PASS + "\n")
        self.assertEqual(code, 1)
        self.assertIn("compiler_error", receipt["failures"])

    def test_nonzero_exit_with_PASS_fails(self):
        code, receipt = self.run_case(editor_code=7)
        self.assertEqual(code, 1)
        self.assertEqual(receipt["editor_exit_code"], 7)

    def test_dirty_lane_is_never_allowed(self):
        self.dirty = " M Packages/manifest.json"
        self.assertEqual(self.run_case()[0], 1)
        self.assertFalse(self.launched)

    def test_wrong_actual_branch(self):
        self.branch = "wrong"
        self.assertEqual(self.run_case()[0], 1)
        self.assertFalse(self.launched)

    def test_wrong_actual_head(self):
        self.actual_head = "b" * 40
        self.assertEqual(self.run_case()[0], 1)
        self.assertFalse(self.launched)

    def test_wrong_actual_lease(self):
        self.metadata["lock_id"] = "other-owner-lock"
        self.assertEqual(self.run_case()[0], 1)
        self.assertFalse(self.launched)

    def test_postrun_lease_change_fails(self):
        original = self.output
        reads = 0
        def changed(cmd, cwd=None):
            nonlocal reads
            if cmd[0] != "git":
                reads += 1
                if reads == 2: self.metadata["head"] = "b" * 40
            return original(cmd, cwd)
        self.output = changed
        self.assertEqual(self.run_case()[0], 1)
        self.assertTrue(self.launched)

    def test_FAIL_only_and_bare_FAIL_and_quoted_PASS_are_rejected(self):
        for text in ("WORKBENCH_RETIREMENT_VERIFY_FAIL checks=1\n", "FAIL\n" + PASS + "\n", "info: " + PASS + "\n"):
            self.assertTrue(runner.verdict(0, text, PASS))


if __name__ == "__main__":
    unittest.main(verbosity=2)
