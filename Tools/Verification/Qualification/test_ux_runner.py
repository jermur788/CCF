"""Exercise UX licence argument/config and launch construction with a fake Editor process."""
from contextlib import redirect_stdout
import importlib.util
import io
from pathlib import Path
from types import SimpleNamespace
import tempfile
import unittest
from unittest.mock import patch

UX_RUNNER = Path(__file__).resolve().parents[1] / "ScenarioOneUx/Qualification/run_qualification.py"
spec = importlib.util.spec_from_file_location("ux_runner_under_test", UX_RUNNER)
ux = importlib.util.module_from_spec(spec)
spec.loader.exec_module(ux)


class UxRunnerTests(unittest.TestCase):
    def test_license_option_defaults_and_existing_isolated_config(self):
        args = ux.parse_args(["--label", "test", "--out", "unused", "--method", "UxQualification.Begin", "--marker", "UX_QUAL_DONE_MARKER"])
        self.assertEqual(ux.LICENCES, args.license_dir)
        with tempfile.TemporaryDirectory(prefix="ccf-ux-config-") as tmp:
            target = Path(tmp) / "config/unity3d/Unity/licenses"
            target.mkdir(parents=True)
            with patch.object(ux.shutil, "copytree") as copy:
                self.assertEqual(Path(tmp) / "config", ux.prepare_config(tmp, Path("chosen-source")))
                copy.assert_not_called()

    def test_custom_license_source_and_unity_command_use_isolated_config(self):
        with tempfile.TemporaryDirectory(prefix="ccf-ux-command-") as tmp:
            out = Path(tmp)
            (out / "test.log").write_text("UX_QUAL_DONE_MARKER\n")
            source = out / "fake-hub-license-source"
            argv = [str(UX_RUNNER), "--label", "test", "--out", str(out), "--method", "UxQualification.Begin",
                    "--marker", "UX_QUAL_DONE_MARKER", "--interactive", "--license-dir", str(source)]
            with patch.object(ux.sys, "argv", argv), patch.object(ux.shutil, "copytree") as copy, \
                    patch.object(ux, "git_state", return_value=set()), \
                    patch.object(ux.subprocess, "check_output", return_value=b"test-head\n"), \
                    patch.object(ux.subprocess, "run", side_effect=[SimpleNamespace(stdout=""), SimpleNamespace(returncode=0)]) as run, \
                    redirect_stdout(io.StringIO()):
                with self.assertRaises(SystemExit) as exit_result:
                    ux.main()
                self.assertEqual(0, exit_result.exception.code)
                copy.assert_called_once_with(source, out / "config/unity3d/Unity/licenses")
                command = run.call_args_list[1].args[0]
                self.assertIn("UxQualification.Begin", command)
                self.assertNotIn("--license-dir", command)
                self.assertNotIn(str(source), command)
                self.assertEqual(str(out / "config"), run.call_args_list[1].kwargs["env"]["XDG_CONFIG_HOME"])


if __name__ == "__main__":
    unittest.main()
