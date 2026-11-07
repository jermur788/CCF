"""Offline admission/false-green/cleanup controls. No Unity process or real licence is used.

RunnerControls holds the 13 original #318 controls (N1-N3 included). The R1-R5 classes below add the
#323 controls: post-run lane drift (mock status and a real temporary git repository), truthful receipts
when a write fails, and exact terminal PASS markers per gate. R324 adds the receipt-finalisation controls:
a surviving markers file must say PROVISIONAL / INCOMPLETE until the FINAL line is committed after the JSON.
Set CCF_TEST_SCRATCH to choose where the
temporary folders go; by default they go to the system temporary directory, never into retained evidence.
"""
import contextlib
import errno
import hashlib
import importlib.util
import io
import json
import os
from pathlib import Path
import shutil
import subprocess
import tempfile
import unittest
from unittest.mock import patch

SPEC = importlib.util.spec_from_file_location("runner", Path(__file__).with_name("run_workbench_retirement.py"))
runner = importlib.util.module_from_spec(SPEC)
SPEC.loader.exec_module(runner)
PASS = "WORKBENCH_RETIREMENT_VERIFY_PASS"
SHA = "a" * 40


class RunnerHarness(unittest.TestCase):
    """Shared mocked lane, lease, licence and Editor. Holds no tests itself."""
    def setUp(self):
        scratch = Path(os.environ.get("CCF_TEST_SCRATCH", tempfile.gettempdir()))
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
        self.postrun = b""  # mocked post-run `git status --porcelain=v1 -uall -z` output (clean by default)
        self.on_editor = None

    def output(self, cmd, cwd=None):
        if cmd[0] != "git":
            return json.dumps(self.status)
        if cmd[1] == "rev-parse": return self.actual_head
        if cmd[1] == "branch": return self.branch
        if cmd[1] == "status": return self.dirty
        if cmd[1] == "ls-files": return str(self.source)
        raise AssertionError(cmd)

    def invoke(self, text=PASS + " checks=1\n", editor_code=0, hash_error=False, cleanup_error=False,
               gate="WorkbenchRetirementVerification", token=PASS, write_error=None):
        """Run the real runner.main with mocks; returns (exit code, printed JSON). No receipt assertions."""
        def editor(cmd, **kwargs):
            if cmd[0] == "git":  # post-run HEAD blob lookup for drifted paths
                return type("Result", (), dict(returncode=0, stdout="d" * 40 + "\n"))()
            self.assertEqual(cmd[0], runner.UNITY)
            self.launched = True
            Path(cmd[cmd.index("-logFile") + 1]).write_text(text)
            Path(str(self.root / "Assets/ForestPrototype" / self.source.name) + ".meta").write_text("mock meta")
            if self.on_editor: self.on_editor()
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
        real_write = Path.write_text
        def write_text(path, *args, **kwargs):
            if write_error and path.name.startswith(gate + write_error):
                raise OSError("mock receipt write failure")
            return real_write(path, *args, **kwargs)
        stdout = io.StringIO()
        with patch.object(runner, "ROOT", self.root), patch.object(runner, "EVIDENCE", self.base), \
                patch.object(runner, "output", self.output), patch.object(runner.Path, "home", return_value=self.home), \
                patch.object(runner.subprocess, "run", editor), patch.object(runner, "licence_hashes", hash_copy), \
                patch.object(runner, "lane_status", lambda: self.postrun, create=True), patch.object(runner.Path, "write_text", write_text), \
                patch.object(runner.shutil, "rmtree", cleanup), contextlib.redirect_stdout(stdout):
            code = runner.main([gate, str(self.source), str(self.out),
                                "--expected-head", SHA, "--expected-branch", "candidate", "--owner", "test",
                                "--lock-id", "test-lock", "--pass-token", token])
        self.stdout_text = stdout.getvalue()
        return code, json.loads(stdout.getvalue())

    def run_case(self, text=PASS + " checks=1\n", editor_code=0, hash_error=False, cleanup_error=False):
        code, _ = self.invoke(text, editor_code, hash_error, cleanup_error)
        stdout = io.StringIO(self.stdout_text)
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


class RunnerControls(RunnerHarness):
    """The 13 original #318 controls, unchanged."""
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


GATES = {
    "WorkbenchRetirementVerification": ("WorkbenchRetirement", "WORKBENCH_RETIREMENT_VERIFY_PASS"),
    "Enlarged80Verification": ("StandGeometry", "ENLARGED80_VERIFY_PASS"),
    "RackV2PersistenceVerification": ("RackV2Persistence", "RACKV2PERSIST_VERIFY_PASS"),
}


class R1MockedPostRunDrift(RunnerHarness):
    """#319 probe P7 shape: the lane was clean at admission, then the Editor left tracked changes."""
    def setUp(self):
        super().setUp()
        (self.root / "Packages").mkdir()
        (self.root / "Packages/manifest.json").write_text('{"dependencies": {"MOCK-PRIVATE-BYTES": "1.1.1"}}\n')
        (self.root / "Packages/packages-lock.json").write_text('{"MOCK-PRIVATE-BYTES": 1}\n')

    def test_R1_P7_postrun_manifest_and_lock_rewrite_fails(self):
        def rewrite():
            self.postrun = b" M Packages/manifest.json\0 M Packages/packages-lock.json\0"
        self.on_editor = rewrite
        code, receipt = self.run_case()
        self.assertEqual(code, 1)
        self.assertIn("postrun_lane_drift:2", receipt["failures"])
        self.assertEqual(receipt["postrun_lane_status"], "drift")
        self.assertEqual([d["path"] for d in receipt["postrun_drift"]], ["Packages/manifest.json", "Packages/packages-lock.json"])
        expected = hashlib.sha256((self.root / "Packages/manifest.json").read_bytes()).hexdigest()
        self.assertEqual(receipt["postrun_drift"][0]["worktree_sha256"], expected)
        markers = (self.out / "WorkbenchRetirementVerification.markers.txt").read_text()
        self.assertIn('POSTRUN_LANE_DRIFT  M path="Packages/manifest.json" worktree=file sha256=' + expected, markers)
        self.assertIn("POSTRUN_LANE_STATUS drift entries=2", markers)
        for text in (markers, self.stdout_text, (self.out / "WorkbenchRetirementVerification.json").read_text()):
            self.assertNotIn("MOCK-PRIVATE-BYTES", text)

    def test_R1_clean_postrun_still_passes(self):
        code, receipt = self.run_case()
        self.assertEqual((code, receipt["postrun_lane_status"], receipt["postrun_drift"]), (0, "clean", []))
        self.assertIn("POSTRUN_LANE_STATUS clean entries=0", (self.out / "WorkbenchRetirementVerification.markers.txt").read_text())

    def test_R1_postrun_status_error_fails(self):
        def broken():
            raise subprocess.CalledProcessError(128, "git status")
        with patch.object(runner, "postrun_drift", broken):
            code, receipt = self.run_case()
        self.assertEqual(code, 1)
        self.assertIn("postrun_status_error:CalledProcessError", receipt["failures"])
        self.assertEqual(receipt["postrun_lane_status"], "error")

    def test_R1_drift_not_checked_without_clean_admission(self):
        self.dirty = " M Packages/manifest.json"
        code, receipt = self.run_case()
        self.assertEqual(code, 1)
        self.assertFalse(self.launched)
        self.assertEqual(receipt["postrun_lane_status"], "not_checked_no_clean_admission")


class R1RealGitPostRunDrift(unittest.TestCase):
    """Real temporary git repository: real `git rev-parse/branch/status` at the exact committed HEAD.
    Only the lease query and the Unity Editor process are mocked."""
    def setUp(self):
        scratch = Path(os.environ.get("CCF_TEST_SCRATCH", tempfile.gettempdir()))
        scratch.mkdir(parents=True, exist_ok=True)
        self.temp = tempfile.TemporaryDirectory(dir=scratch)
        self.addCleanup(self.temp.cleanup)
        self.base = Path(self.temp.name)
        self.root = self.base / "lane"
        files = {
            "Tools/Verification/WorkbenchRetirement/WorkbenchRetirementVerification.cs": "// mock source\n",
            "Packages/manifest.json": '{"dependencies": {"com.unity.toolchain.linux-x86_64": "1.1.0"}}\n',
            "Packages/packages-lock.json": '{"dependencies": {"com.unity.sysroot.base": {"version": "1.1.0"}}}\n',
            "Assets/Materials/Bark.mat": "%YAML 1.1\n_AlphaToMask: 0\n",
            "Assets/ForestPrototype/Keep.cs": "// tracked production stand-in\n",
        }
        for name, body in files.items():
            (self.root / name).parent.mkdir(parents=True, exist_ok=True)
            (self.root / name).write_text(body)
        env = dict(os.environ, GIT_CONFIG_NOSYSTEM="1", HOME=str(self.base))
        self.git_env = env
        for cmd in (["init", "-q", "-b", "candidate"], ["add", "-A"],
                    ["-c", "user.name=mock", "-c", "user.email=mock@invalid", "commit", "-q", "-m", "mock base"]):
            subprocess.run(["git"] + cmd, cwd=self.root, env=env, check=True)
        self.head = subprocess.check_output(["git", "rev-parse", "HEAD"], cwd=self.root, text=True).strip()
        self.source = self.root / "Tools/Verification/WorkbenchRetirement/WorkbenchRetirementVerification.cs"
        self.home = self.base / "home"
        lic = self.home / ".config/unity3d/Unity/licenses"
        lic.mkdir(parents=True)
        (lic / "UnityEntitlementLicense.xml").write_text("MOCK-NOT-A-LICENCE")
        self.out = self.base / "evidence"
        self.status = dict(status="held", unity_editor_pids=[], metadata=dict(
            head=self.head, branch="candidate", lane=str(self.root), task_id="287", owner="test", lock_id="test-lock"))

    def run_real(self, mutate=None):
        real_output, real_run = runner.output, subprocess.run
        def output(cmd, cwd=None):
            return json.dumps(self.status) if cmd[0] != "git" else real_output(cmd, cwd)
        def run(cmd, *args, **kwargs):
            if cmd[0] != runner.UNITY:
                return real_run(cmd, *args, **kwargs)
            Path(cmd[cmd.index("-logFile") + 1]).write_text(PASS + " checks=60\n")
            Path(str(self.root / "Assets/ForestPrototype" / self.source.name) + ".meta").write_text("mock meta")
            if mutate: mutate(self.root)
            return type("Result", (), dict(returncode=0))()
        stdout = io.StringIO()
        with patch.dict(os.environ, {"GIT_CONFIG_NOSYSTEM": "1"}), \
                patch.object(runner, "ROOT", self.root), patch.object(runner, "EVIDENCE", self.base), \
                patch.object(runner, "output", output), patch.object(runner.Path, "home", return_value=self.home), \
                patch.object(runner.subprocess, "run", run), contextlib.redirect_stdout(stdout):
            code = runner.main(["WorkbenchRetirementVerification", str(self.source), str(self.out),
                                "--expected-head", self.head, "--expected-branch", "candidate", "--owner", "test",
                                "--lock-id", "test-lock", "--pass-token", PASS])
        receipt = json.loads((self.out / "WorkbenchRetirementVerification.json").read_text())
        markers = (self.out / "WorkbenchRetirementVerification.markers.txt").read_text()
        self.assertEqual(code, receipt["exit_code"])
        self.assertEqual(code, json.loads(stdout.getvalue())["exit_code"])
        self.assertEqual(receipt["identity"]["head"], self.head)
        self.assertFalse((self.root / "Assets/ForestPrototype" / self.source.name).exists())
        self.assertFalse(Path(str(self.root / "Assets/ForestPrototype" / self.source.name) + ".meta").exists())
        for text in (markers, stdout.getvalue()):
            self.assertNotIn("MOCK-NOT-A-LICENCE", text)
            self.assertNotIn("MOCK-PRIVATE-BYTES", text)
        self.assertNotIn("MOCK-PRIVATE-BYTES", json.dumps(receipt))
        print(self.id().split(".")[-1] + " real_git_runner_stdout=" + stdout.getvalue().strip())
        return code, receipt, markers

    def head_blob(self, path):
        return subprocess.check_output(["git", "rev-parse", "HEAD:" + path], cwd=self.root, text=True).strip()

    def assert_drift(self, mutate, path, status):
        before = self.head_blob(path) if status != "??" else None
        code, receipt, markers = self.run_real(mutate)
        self.assertEqual(code, 1)
        self.assertEqual(receipt["postrun_lane_status"], "drift")
        self.assertIn("postrun_lane_drift:1", receipt["failures"])
        [entry] = receipt["postrun_drift"]
        self.assertEqual((entry["status"], entry["path"], entry["head_blob"]), (status, path, before))
        local = self.root / path
        digest = hashlib.sha256(local.read_bytes()).hexdigest() if local.is_file() else None
        self.assertEqual(entry["worktree_sha256"], digest)
        self.assertIn("POSTRUN_LANE_DRIFT " + status + " path=" + json.dumps(path), markers)
        self.assertIn("sha256=" + str(digest) + " head_blob=" + str(before), markers)
        # No auto-restore or auto-clean: the drift is still in the lane for a human to inspect.
        self.assertTrue(subprocess.check_output(["git", "status", "--porcelain=v1", "-uall"], cwd=self.root, text=True))
        return entry

    def test_R1_real_clean_run_passes_at_exact_head(self):
        code, receipt, markers = self.run_real()
        self.assertEqual((code, receipt["failures"], receipt["postrun_lane_status"]), (0, [], "clean"))
        self.assertIn("POSTRUN_LANE_STATUS clean entries=0", markers)
        self.assertEqual(subprocess.check_output(["git", "status", "--porcelain=v1", "-uall"], cwd=self.root), b"")

    def test_R1_real_editor_rewrites_manifest(self):
        def bump(root):
            (root / "Packages/manifest.json").write_text('{"dependencies": {"com.unity.toolchain.linux-x86_64": "1.1.1"}} MOCK-PRIVATE-BYTES\n')
        self.assert_drift(bump, "Packages/manifest.json", " M")

    def test_R1_real_editor_rewrites_packages_lock(self):
        def bump(root):
            (root / "Packages/packages-lock.json").write_text('{"dependencies": {"com.unity.sysroot.base": {"version": "1.1.1"}}}\n')
        self.assert_drift(bump, "Packages/packages-lock.json", " M")

    def test_R1_real_editor_rewrites_material(self):
        def rewrite(root):
            (root / "Assets/Materials/Bark.mat").write_text("%YAML 1.1\n_AlphaToMask: 1\n")
        self.assert_drift(rewrite, "Assets/Materials/Bark.mat", " M")

    def test_R1_real_editor_leaves_untracked_file(self):
        def stray(root):
            (root / "Assets/ForestPrototype/Stray.asset").write_text("MOCK-PRIVATE-BYTES\n")
        entry = self.assert_drift(stray, "Assets/ForestPrototype/Stray.asset", "??")
        self.assertIsNone(entry["head_blob"])

    def test_R1_real_editor_deletes_tracked_file(self):
        def delete(root):
            (root / "Assets/ForestPrototype/Keep.cs").unlink()
        entry = self.assert_drift(delete, "Assets/ForestPrototype/Keep.cs", " D")
        self.assertEqual(entry["worktree"], "absent")

    def test_R1_real_staged_change_and_several_paths(self):
        def several(root):
            (root / "Packages/manifest.json").write_text("{}\n")
            (root / "Assets/Materials/Bark.mat").write_text("changed\n")
            subprocess.run(["git", "add", "Assets/Materials/Bark.mat"], cwd=root, check=True)
        code, receipt, markers = self.run_real(several)
        self.assertEqual(code, 1)
        self.assertEqual(sorted((d["status"], d["path"]) for d in receipt["postrun_drift"]),
                         [(" M", "Packages/manifest.json"), ("M ", "Assets/Materials/Bark.mat")])
        self.assertIn("postrun_lane_drift:2", receipt["failures"])
        self.assertEqual(markers.count("POSTRUN_LANE_DRIFT "), 2)


class R2TruthfulReceipts(RunnerHarness):
    def test_R2_markers_write_failure_is_in_durable_json(self):
        code, printed = self.invoke(write_error=".markers.txt")
        self.assertEqual(code, 1)
        receipt = json.loads((self.out / "WorkbenchRetirementVerification.json").read_text())
        self.assertEqual(receipt, printed)
        self.assertEqual(receipt["exit_code"], 1)
        self.assertIn("receipt_write_error:markers:OSError", receipt["failures"])
        self.assertFalse((self.out / "WorkbenchRetirementVerification.markers.txt").exists())
        self.assertEqual(sorted(p.name for p in self.out.glob("*.partial")), [])
        self.assertFalse((self.root / "Assets/ForestPrototype" / self.source.name).exists())
        self.assertFalse((self.out / "config/unity3d/Unity/licenses").exists())

    def test_R2_json_write_failure_leaves_no_json_and_marks_markers(self):
        code, printed = self.invoke(write_error=".json")
        self.assertEqual((code, printed["exit_code"]), (1, 1))
        self.assertIn("receipt_write_error:json:OSError", printed["failures"])
        self.assertFalse((self.out / "WorkbenchRetirementVerification.json").exists())
        self.assertEqual(sorted(p.name for p in self.out.glob("*.partial")), [])
        markers = (self.out / "WorkbenchRetirementVerification.markers.txt").read_text()
        self.assertTrue(markers.endswith("RUNNER_RECEIPT_ERROR json:OSError runner_exit=1\n"))
        self.assertFalse((self.out / "config/unity3d/Unity/licenses").exists())

    def test_R2_markers_failure_on_a_failing_gate_keeps_every_failure(self):
        code, printed = self.invoke(text="WORKBENCH_RETIREMENT_VERIFY_FAIL checks=3\n", write_error=".markers.txt")
        receipt = json.loads((self.out / "WorkbenchRetirementVerification.json").read_text())
        self.assertEqual((code, receipt["exit_code"]), (1, 1))
        for failure in ("missing_terminal_pass", "gate_fail_marker", "fail_marker", "receipt_write_error:markers:OSError"):
            self.assertIn(failure, receipt["failures"])

    @unittest.skipIf(hasattr(os, "geteuid") and os.geteuid() == 0, "root ignores directory permissions")
    def test_R2_unwritable_evidence_directory_invents_no_receipt(self):
        def lock_out():
            os.chmod(self.out, 0o555)
        self.on_editor = lock_out
        self.addCleanup(lambda: os.chmod(self.out, 0o755) if self.out.exists() else None)
        code, printed = self.invoke()
        self.assertEqual((code, printed["exit_code"]), (1, 1))
        self.assertIn("receipt_write_error:markers:PermissionError", printed["failures"])
        self.assertIn("receipt_write_error:json:PermissionError", printed["failures"])
        names = sorted(p.name for p in self.out.iterdir())
        self.assertEqual(names, ["WorkbenchRetirementVerification.log", "config"])
        self.assertFalse((self.root / "Assets/ForestPrototype" / self.source.name).exists())
        self.assertFalse((self.out / "config/unity3d/Unity/licenses").exists())


class R324FinalReceipt(RunnerHarness):
    """R324-01: a surviving markers receipt must never read as a runner PASS unless the FINAL line was committed.

    Receipt order is PROVISIONAL markers -> JSON -> FINAL markers. The faults below are injected AFTER the real
    provisional markers write, which is exactly where the #324 reviewer reproduced the defect."""
    GATE = "WorkbenchRetirementVerification"

    def markers(self):
        return (self.out / (self.GATE + ".markers.txt")).read_text()

    def assert_only_provisional(self, markers):
        lines = markers.splitlines()
        self.assertTrue(lines[0].startswith("RUNNER_RECEIPT_STATE PROVISIONAL INCOMPLETE - NOT A RUNNER VERDICT"))
        self.assertFalse([l for l in lines if l.startswith("RUNNER_RECEIPT_STATE FINAL")])
        # No raw Editor line and no git-status line may stand on its own where a reader could take it for a verdict.
        for line in lines[1:]:
            self.assertTrue(line.startswith(("PROVISIONAL> ", "RUNNER_RECEIPT_STATE PROVISIONAL END", "RUNNER_RECEIPT_STATE INCOMPLETE",
                                             "RUNNER_RECEIPT_ERROR ")), line)
        self.assertIn("PROVISIONAL> " + PASS + " checks=1", markers)
        self.assertIn("PROVISIONAL> POSTRUN_LANE_STATUS clean entries=0", markers)

    def full_after_first_write(self, fail_from=2):
        """write_receipt that really writes the first fail_from-1 receipts, then reports ENOSPC; and append reports ENOSPC."""
        real, calls = runner.write_receipt, []
        def write(path, body):
            calls.append(path.name)
            if len(calls) >= fail_from:
                raise OSError(errno.ENOSPC, "No space left on device")
            return real(path, body)
        real_open = runner.Path.open
        def open_(path, mode="r", *args, **kwargs):
            if "a" in mode and path.name.endswith(".markers.txt"):
                raise OSError(errno.ENOSPC, "No space left on device")
            return real_open(path, mode, *args, **kwargs)
        return write, open_, calls

    def test_R324_clean_run_ends_with_FINAL_markers_bound_to_the_JSON(self):
        code, receipt = self.run_case()
        self.assertEqual(code, 0)
        markers = self.markers()
        raw = (self.out / (self.GATE + ".json")).read_bytes()
        self.assertEqual(markers.splitlines()[0], "RUNNER_RECEIPT_STATE FINAL runner_exit=0 json=%s.json json_sha256=%s"
                         % (self.GATE, hashlib.sha256(raw).hexdigest()))
        self.assertIn(PASS + " checks=1", markers.splitlines())  # final form keeps the raw Editor line, unquoted
        self.assertNotIn("PROVISIONAL", markers)
        self.assertIn("RUNNER_RECEIPT_STATE FINAL runner_exit=0", receipt["receipt_authority"])
        self.assertEqual(runner.authoritative_success(self.out, self.GATE), (True, "authoritative success"))

    def test_R324_failed_gate_is_a_FINAL_failure_not_a_success(self):
        code, receipt = self.run_case(text="WORKBENCH_PASS intermediate\n")
        self.assertEqual(code, 1)
        self.assertTrue(self.markers().splitlines()[0].startswith("RUNNER_RECEIPT_STATE FINAL runner_exit=1 json="))
        self.assertEqual(runner.authoritative_success(self.out, self.GATE), (False, "JSON receipt records a failure"))

    def test_R324_01_ENOSPC_json_and_fallback_append_both_fail(self):
        """The #324 residual: JSON creation AND the fallback marker append both fail (simulated ENOSPC)."""
        write, open_, calls = self.full_after_first_write()
        with patch.object(runner, "write_receipt", write), patch.object(runner.Path, "open", open_):
            code, printed = self.invoke()
        self.assertEqual((code, printed["exit_code"]), (1, 1))
        self.assertIn("receipt_write_error:json:OSError", printed["failures"])
        self.assertEqual(calls, [self.GATE + ".markers.txt", self.GATE + ".json"])  # the FINAL write is never attempted
        self.assertFalse((self.out / (self.GATE + ".json")).exists())  # no fabricated JSON
        self.assertEqual(sorted(p.name for p in self.out.glob("*.partial")), [])  # no partial-success artifact
        markers = self.markers()
        self.assert_only_provisional(markers)
        self.assertTrue(markers.endswith(runner.PROVISIONAL_LAST + "\n"))  # the append failed, so the file ends as provisional
        self.assertEqual(runner.authoritative_success(self.out, self.GATE), (False, "receipt missing or unreadable"))
        self.assertFalse((self.root / "Assets/ForestPrototype" / self.source.name).exists())
        self.assertFalse((self.out / "config/unity3d/Unity/licenses").exists())

    @unittest.skipIf(hasattr(os, "geteuid") and os.geteuid() == 0, "root ignores file permissions")
    def test_R324_01_real_permission_failure_json_and_fallback_append_both_fail(self):
        """Same residual with real filesystem permissions: no operation is mocked after the real markers write."""
        real, written = runner.write_receipt, []
        def write_then_lock(path, body):
            real(path, body)
            written.append(path.name)
            if len(written) == 1:  # the real PROVISIONAL markers exist; now refuse every later write
                os.chmod(path, 0o444)
                os.chmod(path.parent, 0o555)
        self.addCleanup(lambda: os.chmod(self.out, 0o755) if self.out.exists() else None)
        with patch.object(runner, "write_receipt", write_then_lock):
            code, printed = self.invoke()
        self.assertEqual((code, printed["exit_code"]), (1, 1))
        self.assertEqual(printed["failures"], ["receipt_write_error:json:PermissionError"])  # real error, not a mock
        self.assertEqual(written, [self.GATE + ".markers.txt"])
        self.assertFalse((self.out / (self.GATE + ".json")).exists())
        self.assertEqual(sorted(p.name for p in self.out.glob("*.partial")), [])
        self.assert_only_provisional(self.markers())
        self.assertEqual(runner.authoritative_success(self.out, self.GATE), (False, "receipt missing or unreadable"))

    def test_R324_final_markers_write_failure_exits_nonzero_and_stays_provisional(self):
        """The new third write can fail too: JSON committed, FINAL markers not. Not a success; no cross-file atomicity is claimed."""
        write, open_, calls = self.full_after_first_write(fail_from=3)
        with patch.object(runner, "write_receipt", write):
            code, printed = self.invoke()
        self.assertEqual(code, 1)
        self.assertIn("receipt_write_error:markers_final:OSError", printed["failures"])
        self.assertEqual(printed["exit_code"], 1)
        self.assertEqual(calls, [self.GATE + ".markers.txt", self.GATE + ".json", self.GATE + ".markers.txt"])
        on_disk = json.loads((self.out / (self.GATE + ".json")).read_text())
        self.assertEqual(on_disk["exit_code"], 0)  # committed before the failure and not amendable: the markers decide
        markers = self.markers()
        self.assertTrue(markers.startswith(runner.PROVISIONAL_FIRST))
        self.assertTrue(markers.endswith("RUNNER_RECEIPT_ERROR markers_final:OSError runner_exit=1\n"))
        self.assertEqual(runner.authoritative_success(self.out, self.GATE), (False, "markers are not FINAL for exactly this JSON"))

    def test_R324_authoritative_success_needs_matching_JSON_and_FINAL_markers(self):
        self.run_case()
        json_path, markers_path = self.out / (self.GATE + ".json"), self.out / (self.GATE + ".markers.txt")
        good_json, good_markers = json_path.read_bytes(), markers_path.read_text()
        self.assertTrue(runner.authoritative_success(self.out, self.GATE)[0])
        json_path.write_bytes(good_json + b" ")  # FINAL line for different JSON bytes
        self.assertFalse(runner.authoritative_success(self.out, self.GATE)[0])
        json_path.write_bytes(good_json)
        markers_path.write_text(good_markers.replace("FINAL", "PROVISIONAL", 1))  # raw PASS and clean status alone
        self.assertFalse(runner.authoritative_success(self.out, self.GATE)[0])
        markers_path.write_text("\n".join(good_markers.splitlines()[1:]) + "\n")  # old-style file with no state line
        self.assertEqual(runner.authoritative_success(self.out, self.GATE), (False, "markers are not FINAL for exactly this JSON"))
        markers_path.unlink()
        self.assertEqual(runner.authoritative_success(self.out, self.GATE), (False, "receipt missing or unreadable"))


class R3ExactTerminalPass(RunnerHarness):
    def use_gate(self, gate):
        folder, token = GATES[gate]
        self.source = self.root / "Tools/Verification" / folder / (gate + ".cs")
        self.source.parent.mkdir(parents=True, exist_ok=True)
        self.source.write_text("// mock source\n")
        return token

    def refused(self, gate, token):
        stderr = io.StringIO()
        with contextlib.redirect_stderr(stderr), self.assertRaises(SystemExit) as raised:
            self.invoke(gate=gate, token=token)
        self.assertEqual(raised.exception.code, 2)
        self.assertFalse(self.launched)
        self.assertFalse(self.out.exists())
        return stderr.getvalue()

    def test_R3_P8_intermediate_rack_marker_as_pass_token_is_refused(self):
        self.use_gate("RackV2PersistenceVerification")
        self.assertIn("RACKV2PERSIST_VERIFY_PASS", self.refused("RackV2PersistenceVerification", "RACKV2PERSIST_CHECK"))

    def test_R3_arbitrary_or_cross_gate_tokens_are_refused(self):
        for gate in GATES:
            self.use_gate(gate)
            for token in ("ENLARGED80_CHECK", "MY_PASS", "RACKV2PERSIST_VERIFY_PASS_X", "WORKBENCH_PASS"):
                if token == GATES[gate][1]: continue
                self.refused(gate, token)
            for other in GATES:
                if other != gate:
                    self.refused(gate, GATES[other][1])

    def test_R3_unknown_gate_is_refused(self):
        self.refused("SomethingElseVerification", "SOMETHING_VERIFY_PASS")

    def test_R3_rack_log_with_only_intermediate_markers_fails(self):
        token = self.use_gate("RackV2PersistenceVerification")
        code, printed = self.invoke("RACKV2PERSIST_CHECK new game\nRACKV2PERSIST_CHECK draft\n",
                                    gate="RackV2PersistenceVerification", token=token)
        self.assertEqual(code, 1)
        self.assertIn("missing_terminal_pass", printed["failures"])

    def test_R3_each_gate_positive_control_passes(self):
        for gate in GATES:
            with self.subTest(gate=gate):
                token = self.use_gate(gate)
                self.out = self.base / ("evidence-" + gate)
                code, printed = self.invoke("NOTE noise\n" + token + " mode=v2 checks=9\n", gate=gate, token=token)
                self.assertEqual((code, printed["failures"]), (0, []))
                self.assertTrue((self.out / (gate + ".json")).exists())

    def test_R3_duplicate_missing_lookalike_and_fail_markers(self):
        for gate, (_, token) in GATES.items():
            fail = token[:-len("_PASS")] + "_FAIL"
            cases = {
                token + " checks=1\n" + token + " checks=1\n": "duplicate_terminal_pass",
                "": "missing_terminal_pass",
                token + "ED checks=1\n": "missing_terminal_pass",
                token + "_X checks=1\n": "missing_terminal_pass",
                token + ".x\n": "missing_terminal_pass",
                " " + token + " checks=1\n": "missing_terminal_pass",
                "prefix " + token + "\n": "missing_terminal_pass",
                fail + " boom\n" + token + " checks=1\n": "gate_fail_marker",
                token + " checks=1\n" + fail + "ED later\n": "gate_fail_marker",
            }
            for text, expected in cases.items():
                with self.subTest(gate=gate, text=text):
                    self.assertIn(expected, runner.verdict(0, text, token))
            self.assertEqual(runner.verdict(0, token + "\n", token), [])
            self.assertEqual(runner.verdict(0, token + "\tchecks=2\n", token), [])

    def test_R3_terminal_table_matches_the_harness_sources(self):
        repo = Path(__file__).resolve().parents[3]
        for gate, (folder, token) in GATES.items():
            self.assertEqual(runner.TERMINAL_PASS[gate], token)
            text = (repo / "Tools/Verification" / folder / (gate + ".cs")).read_text()
            self.assertIn('"' + token + ' ', text)
            self.assertIn('"' + token[:-len("_PASS")] + '_FAIL', text)
        self.assertEqual(set(runner.TERMINAL_PASS), set(GATES))


if __name__ == "__main__":
    unittest.main(verbosity=2)
