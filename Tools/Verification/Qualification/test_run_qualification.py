"""Receipt regression checks: run with unittest; no Editor, project or save is touched."""
import unittest

from run_qualification import evaluate_log
from qualification_results import PERF_LABELS, density_request, evaluate_receipt, read_longrun_evidence
from pathlib import Path
import tempfile


class QualificationReceiptTests(unittest.TestCase):
    marker = "UX_QUAL_DONE_MARKER"

    def test_recorded_f10_failure_is_not_a_pass_despite_exit_zero(self):
        # Excerpt from the integrated ux-resume.log (102 successes, one assertion failure).
        log = "\n".join([
            "UXQ_FAIL INPUT_SESSIONMENU_OPENS_AND_CLOSES SessionMenu: opened=False closed=True playerEnabled=True cursorLock=Locked cursorVisible=False playerMoved=False",
            "UX_QUAL_FAILED shots=24 hardChecks=102 exception= failures=[INPUT_SESSIONMENU_OPENS_AND_CLOSES]",
            self.marker,
        ])
        status, _, failures, _ = evaluate_log(0, self.marker, log, [])
        self.assertEqual("FAIL", status)
        self.assertEqual(2, len(failures))

    def test_failed_assertion_without_summary_still_fails(self):
        self.assertEqual("FAIL", evaluate_log(0, self.marker, "UXQ_FAIL CLIPPED_CONTROL\n" + self.marker, [])[0])

    def test_unverified_input_is_explicit_and_does_not_hide_other_failures(self):
        log = "UXQ_UNVERIFIED F10_SYNTHETIC_DELIVERY opened=False closed=True; native/Player F10 remains UNVERIFIED\n" + self.marker
        status, _, _, unverified = evaluate_log(0, self.marker, log, [])
        self.assertEqual("PASS_WITH_UNVERIFIED", status)
        self.assertEqual(1, len(unverified))
        self.assertEqual("FAIL", evaluate_log(0, self.marker, log + "\nUXQ_FAIL SESSION_MENU_DIRECT_OPEN_ISOLATES_FOREST", [])[0])

    def test_existing_success_marker_stays_a_pass(self):
        self.assertEqual("PASS", evaluate_log(0, "DENSITY_QUAL_PASS", "DENSITY_QUAL_PASS geometry=E80 hardChecks=20", [])[0])

    def test_partial_delivery_failure_overrides_earlier_unverified(self):
        log = "\n".join([
            "UXQ_UNVERIFIED F10_OPENS_THE_SESSION_MENU beforeOpen=False afterOpen=False",
            "UXQ_PASS MENU_CLOSES_WITH_F10 beforeOpen=True afterOpen=False",
            "UXQ_FAIL INPUT_SESSIONMENU_OPENS beforeOpen=False afterOpen=False",
            self.marker,
        ])
        status, _, failures, unverified = evaluate_log(0, self.marker, log, [])
        self.assertEqual("FAIL", status)
        self.assertEqual(1, len(failures))
        self.assertEqual(1, len(unverified))

    def test_launch_compile_and_tree_failures_remain_failures(self):
        for code, log, changes in [(1, self.marker, []), (0, "", []), (0, "error CS1002\n" + self.marker, []), (0, self.marker, [" M Assets/Unexpected.cs"])]:
            with self.subTest(code=code, log=log, changes=changes):
                self.assertEqual("FAIL", evaluate_log(code, self.marker, log, changes)[0])

    def test_density_zero_missing_and_insufficient_hard_checks_fail_closed(self):
        marker = "DENSITY_QUAL_PASS"
        for suffix in ("hardChecks=0", "", "hardChecks=invalid"):
            result = evaluate_receipt(0, marker, marker + " geometry=E80 " + suffix, [])
            self.assertEqual("FAIL", result["status"])
            self.assertIn("selected nothing", result["coverage_errors"][0])
        self.assertEqual("FAIL", evaluate_receipt(0, marker, marker + " hardChecks=0", [], 0)["status"])
        self.assertEqual("FAIL", evaluate_receipt(0, marker, marker + " hardChecks=1", [], 2)["status"])
        self.assertEqual("PASS", evaluate_receipt(0, marker, marker + " hardChecks=1", [])["status"])

    def test_expected_hard_checks_for_another_gate_are_enforced(self):
        for suffix in ("checks=0", ""):
            self.assertEqual("FAIL", evaluate_receipt(0, "GATE_PASS", "GATE_PASS " + suffix, [], 1)["status"])
        self.assertEqual("PASS", evaluate_receipt(0, "GATE_PASS", "GATE_PASS assertions=2", [], 1)["status"])

    def test_failed_density_assertion_cannot_hide_behind_a_success_marker(self):
        text = "QUAL_FAIL E80 LONGRUN_SELECTION filter selected nothing\nDENSITY_QUAL_PASS hardChecks=1"
        self.assertEqual("FAIL", evaluate_receipt(0, "DENSITY_QUAL_PASS", text, [])["status"])

    def test_marker_mentioned_in_an_error_is_not_a_verdict(self):
        self.assertEqual("FAIL", evaluate_receipt(0, "GATE_PASS", "expected marker GATE_PASS", [])["status"])

    def test_post_verdict_shutdown_crash_retains_failure_and_exit(self):
        marker = "MIXED_DENSITY_VERIFY_PASS"
        text = marker + " checks=34\nGot a SIGSEGV while executing native code\nShutdownControllerHIDAPI()"
        result = evaluate_receipt(255, marker, text, ["?? mono_crash.blob"])
        self.assertEqual("FAIL", result["status"])
        self.assertEqual("PASS", result["gate_verdict"])
        self.assertEqual(255, result["exit_code"])
        self.assertEqual("VERDICT_PASS_POST_SHUTDOWN_CRASH", result["process_outcome"])
        clean = evaluate_receipt(0, marker, marker + " checks=34", [])
        self.assertEqual("PASS", clean["status"])
        self.assertEqual("PASS_CLEAN_SHUTDOWN", clean["process_outcome"])

    def test_crash_before_verdict_and_unattributed_exit_remain_failures(self):
        marker = "MIXED_DENSITY_VERIFY_PASS"
        for text in ("Got a SIGSEGV\nShutdownControllerHIDAPI()", marker + " checks=34"):
            result = evaluate_receipt(255, marker, text, [])
            self.assertEqual("FAIL", result["status"])
            self.assertEqual("PROCESS_OR_GATE_FAILURE", result["process_outcome"])
        text = "Got a SIGSEGV\nShutdownControllerHIDAPI()\n" + marker + " checks=34"
        self.assertEqual("PROCESS_OR_GATE_FAILURE", evaluate_receipt(255, marker, text, [])["process_outcome"])

    def test_failed_assertion_prevents_post_verdict_pass_annotation(self):
        text = "QUAL_FAIL E80 FAILED_CHECK\nDENSITY_QUAL_PASS hardChecks=1\nSIGSEGV\nShutdownControllerHIDAPI()"
        result = evaluate_receipt(255, "DENSITY_QUAL_PASS", text, [])
        self.assertEqual("FAIL", result["gate_verdict"])
        self.assertEqual("PROCESS_OR_GATE_FAILURE", result["process_outcome"])

    def test_receipt_retains_actual_game_view_dimensions(self):
        result = evaluate_receipt(0, "U4_GUIDANCE_PASS", "U4_NOTE GAME_VIEW actual=1280x720 requested=1280x720\nU4_GUIDANCE_PASS checks=112 screen=1280x720", [])
        self.assertEqual(["1280x720"], result["game_view_dimensions"])


    def test_unknown_density_phase_and_partially_invalid_selector_fail(self):
        for env in ({"CCF_QUAL_PHASES": "LONGRUN,UNKNOWN"}, {"CCF_QUAL_SCENARIOS": "B,UNKNOWN"}, {"CCF_QUAL_SCENARIOS": "B_BEECH_DENSE"}):
            with self.subTest(env=env):
                with self.assertRaises(ValueError):
                    density_request(env)
                self.assertEqual("FAIL", evaluate_receipt(0, "DENSITY_QUAL_PASS", "DENSITY_QUAL_PASS hardChecks=90", [], env=env)["status"])

    def test_measurement_only_longrun_requires_real_complete_rows(self):
        env = {"CCF_QUAL_PHASES": "LONGRUN", "CCF_QUAL_REPLAY": "0", "CCF_QUAL_SCENARIOS": "B,F", "CCF_STAND_GEOMETRY": "1"}
        log = "QUAL_START geometry=E80 phases=LONGRUN\nDENSITY_QUAL_PASS hardChecks=0"
        with tempfile.TemporaryDirectory(prefix="ccf-longrun-receipt-") as tmp:
            path = Path(tmp) / "longrun-E80.csv"
            header = "scenario,geo,year\n"
            rows = [f"{scenario},E80,{year}\n" for scenario in ("B_BEECH_DENSE", "F_BEECH_YOUNG") for year in range(1, 101)]
            path.write_text(header + ''.join(rows))
            evidence = read_longrun_evidence(tmp, env, log)
            self.assertTrue(evidence['complete'])
            result = evaluate_receipt(0, "DENSITY_QUAL_PASS", log, [], env=env, measurement_evidence=evidence)
            self.assertEqual("PASS", result['status'])
            self.assertTrue(result['measurement_only'])
            self.assertEqual("PENDING_DOWNSTREAM_ANALYSIS", result['measurement_verdict'])
            self.assertEqual(0, result['hard_checks'])
            replay = dict(env, CCF_QUAL_REPLAY="1")
            self.assertEqual("FAIL", evaluate_receipt(0, "DENSITY_QUAL_PASS", log, [], env=replay, measurement_evidence=evidence)['status'])
            self.assertEqual("FAIL", evaluate_receipt(0, "DENSITY_QUAL_PASS", log, [], env=env)["status"])
            for bad_rows in ([], rows[:100], rows[:-1] + [rows[0]], [row.replace('B_BEECH_DENSE', 'A_OAK_DENSE') for row in rows]):
                path.write_text(header + ''.join(bad_rows))
                bad = read_longrun_evidence(tmp, env, log)
                self.assertFalse(bad['complete'])
                self.assertEqual("FAIL", evaluate_receipt(0, "DENSITY_QUAL_PASS", log, [], env=env, measurement_evidence=bad)['status'])

    def test_existing_perf_only_phase_requires_its_measurements_not_hard_checks(self):
        env = {"CCF_QUAL_PHASES": "PERF"}
        log = '\n'.join('QUAL_MEASURE E80 ' + label + ' meanMs=1' for label in sorted(PERF_LABELS)) + '\nDENSITY_QUAL_PASS hardChecks=0'
        self.assertEqual("PASS", evaluate_receipt(0, "DENSITY_QUAL_PASS", log, [], env=env)['status'])
        self.assertEqual("FAIL", evaluate_receipt(0, "DENSITY_QUAL_PASS", "DENSITY_QUAL_PASS hardChecks=0", [], env=env)['status'])

    def test_shutdown_annotation_is_limited_to_observed_exits_and_crash_artifacts(self):
        marker = "MIXED_DENSITY_VERIFY_PASS"
        log = marker + " checks=34\nSIGSEGV\nShutdownControllerHIDAPI()"
        for code in (255, 139, -11):
            result = evaluate_receipt(code, marker, log, ["?? mono_crash.mem.123.blob"])
            self.assertEqual("VERDICT_PASS_POST_SHUTDOWN_CRASH", result['process_outcome'])
            self.assertEqual("FAIL", result['status'])
            self.assertEqual(code, result['exit_code'])
        for code, artifacts in ((1, []), (124, []), (198, []), (255, [" M Assets/Unexpected.cs"]), (255, ["?? Assets/mono_crash.blob"])):
            result = evaluate_receipt(code, marker, log, artifacts)
            self.assertEqual("PROCESS_OR_GATE_FAILURE", result['process_outcome'])
            self.assertEqual("FAIL", result['status'])


if __name__ == "__main__":
    unittest.main()
