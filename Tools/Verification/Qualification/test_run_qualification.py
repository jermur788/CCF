"""Receipt regression checks: run with unittest; no Editor, project or save is touched."""
import unittest

from run_qualification import evaluate_log


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


if __name__ == "__main__":
    unittest.main()
