"""Deterministic offline tests for BroadLynch2006_Sitka. No Unity, no dependencies.

Run:  python3 Tools/Verification/Growfor/test_broad_lynch_2006_sitka.py
Exit 0 = all checks pass.
"""
import math
import sys
import unittest
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import broad_lynch_2006_sitka as m  # noqa: E402

IDENTITY = [[1.0, 0.0, 0.0], [0.0, 1.0, 0.0], [0.0, 0.0, 1.0]]


def max_abs_diff(a, b):
    return max(abs(a[i][j] - b[i][j]) for i in range(3) for j in range(3))


class MatrixTests(unittest.TestCase):
    def test_P_times_Pinv_is_identity(self):
        for model in (m.THINNED, m.UNTHINNED):
            with self.subTest(model=model["name"]):
                prod = m.mat3_mul(model["P"], model["Pinv"])
                scale = max(abs(v) for row in model["P"] for v in row)
                # The pair is printed to ~4 decimals; the residual is printed-precision
                # rounding (relative), not a fitting/inversion error.
                self.assertLess(max_abs_diff(prod, IDENTITY) / scale, 5e-4)

    def test_C_times_Cinv_is_identity(self):
        for model in (m.THINNED, m.UNTHINNED):
            with self.subTest(model=model["name"]):
                prod = m.mat3_mul(model["C"], model["Cinv"])
                scale = max(abs(v) for row in model["C"] for v in row)
                self.assertLess(max_abs_diff(prod, IDENTITY) / scale, 5e-4)

    def test_recomputed_inverse_matches_published(self):
        # The published inverse should be (approximately) the analytic inverse.
        for model in (m.THINNED, m.UNTHINNED):
            with self.subTest(model=model["name"]):
                self.assertLess(max_abs_diff(m.mat3_inverse(model["P"]), model["Pinv"]), 1e-3)
                self.assertLess(max_abs_diff(m.mat3_inverse(model["C"]), model["Cinv"]), 1e-3)

    def test_power_transform_round_trip(self):
        x = [30.0, 1500.0, 18.0]
        for model in (m.THINNED, m.UNTHINNED):
            with self.subTest(model=model["name"]):
                y = m._power(x, model["C"])
                back = m._power(y, model["Cinv"])
                self.assertLess(max(abs(back[i] - x[i]) / x[i] for i in range(3)), 3e-3)


class HeightSiteTests(unittest.TestCase):
    def test_height_at_30_reproduces_site_index(self):
        for S in (14.0, 18.0, 20.4, 24.0, 28.0):
            with self.subTest(site_index=S):
                self.assertAlmostEqual(m.height(30.0, S), S, places=9)

    def test_site_index_from_height_round_trip(self):
        for S in (16.0, 20.0, 26.0):
            for age in (15.0, 30.0, 45.0):
                with self.subTest(S=S, age=age):
                    H = m.height(age, S)
                    self.assertAlmostEqual(m.site_index_from_height(age, H), S, places=6)

    def test_height_monotonic_and_bounded_by_asymptote(self):
        prev = -1.0
        for age in range(0, 121, 5):
            h = m.height(age, 20.0)
            self.assertGreater(h, prev)
            self.assertLess(h, m.HEIGHT_A)
            prev = h


class TransitionTests(unittest.TestCase):
    x0 = [25.0, 1600.0, 15.0]

    @staticmethod
    def _published_orientation(model, y, b, dt, swapped=False):
        """Independently assemble x2 = {a + P^-1 exp[Λ b Δt] P (x^C - a)}^{C^-1} with an explicit
        3x3 product, so the result does not depend on the implementation's internal ordering.
        With swapped=True the conjugation is reversed (P exp[Λ b Δt] P^-1), a plausible wrong order."""
        left, right = (model["P"], model["Pinv"]) if swapped else (model["Pinv"], model["P"])
        exp_diag = [math.exp(model["Lambda"][i] * b * dt) for i in range(3)]
        middle = [[exp_diag[i] if i == j else 0.0 for j in range(3)] for i in range(3)]
        matrix = m.mat3_mul(m.mat3_mul(left, middle), right)
        shifted = [y[i] - model["a"][i] for i in range(3)]
        transformed = [model["a"][i] + sum(matrix[i][j] * shifted[j] for j in range(3)) for i in range(3)]
        return m._power(transformed, model["Cinv"])

    def test_transition_matches_the_published_orientation(self):
        # Independent numeric fixture: the correct order is P^-1 exp(ΛbΔt) P acting on (x^C - a).
        for model in (m.THINNED, m.UNTHINNED):
            with self.subTest(model=model["name"]):
                b = m.b_from_site_index(22.0)
                y = m._power(self.x0, model["C"])
                expected = self._published_orientation(model, y, b, 7.0)
                got = m.transition(self.x0, model, b, 7.0)
                self.assertLess(max(abs(got[i] - expected[i]) / expected[i] for i in range(3)), 1e-9)

    def test_swapped_matrix_order_is_distinguished(self):
        # The reversed conjugation must produce a materially different state, so a matrix-order
        # mutation cannot survive this fixture.
        b = m.b_from_site_index(22.0)
        y = m._power(self.x0, m.UNTHINNED["C"])
        correct = self._published_orientation(m.UNTHINNED, y, b, 7.0)
        swapped = self._published_orientation(m.UNTHINNED, y, b, 7.0, swapped=True)
        rel = max(abs(correct[i] - swapped[i]) / abs(correct[i]) for i in range(3))
        self.assertGreater(rel, 1e-3)
        self.assertLess(max(abs(m.transition(self.x0, m.UNTHINNED, b, 7.0)[i] - correct[i]) / correct[i]
                            for i in range(3)), 1e-9)

    def test_split_vs_combined_time_step(self):
        # The transition is linear in x^C, so splitting a step must agree with the
        # combined step to the residual of the published rounded P/P^-1 pair. The
        # remaining discrepancy is printed-coefficient rounding, not exact semigroup
        # behaviour; do not tighten beyond that precision.
        for model in (m.THINNED, m.UNTHINNED):
            with self.subTest(model=model["name"]):
                b = m.b_from_site_index(20.0)
                combined = m.transition(self.x0, model, b, 5.0)
                split = m.transition(m.transition(self.x0, model, b, 2.0), model, b, 3.0)
                rel = max(abs(combined[i] - split[i]) / abs(split[i]) for i in range(3))
                self.assertLess(rel, 5e-3)

    def test_transition_finite_positive(self):
        b = m.b_from_site_index(20.0)
        x = m.transition(self.x0, m.UNTHINNED, b, 1.0)
        self.assertTrue(all(v > 0 and math.isfinite(v) for v in x))


class ThinningTests(unittest.TestCase):
    # Published reference case (review): B0=30 m2/ha, N0=1800 stems/ha, H=16 m, N=1200 stems/ha.
    REFERENCE = 23.7697

    def test_reference_example_matches_the_printed_coefficients(self):
        # Independent numeric reference, not derived from the implementation: the printed paper
        # coefficients a=10.0991,b=0.2371,c=-0.2877,d=-0.5585 give B ~= 23.7697 m2/ha.
        self.assertAlmostEqual(m.thin_basal_area(30.0, 1800.0, 16.0, 1200.0), self.REFERENCE, places=3)

    def test_no_stocking_reduction_returns_the_initial_basal_area(self):
        # N == N0 must return B0 exactly (within floating point): no thinning, no basal-area change.
        self.assertAlmostEqual(m.thin_basal_area(30.0, 1800.0, 16.0, 1800.0), 30.0, places=9)
        self.assertAlmostEqual(m.thin_basal_area(42.5, 900.0, 22.0, 900.0), 42.5, places=9)

    def test_moderate_thinning_is_finite_and_physically_plausible(self):
        B = m.thin_basal_area(30.0, 1800.0, 16.0, 1500.0)
        self.assertTrue(math.isfinite(B))
        self.assertGreater(B, 0.0)
        self.assertLess(B, 30.0)

    def test_immediate_composition_of_stocking_reductions(self):
        B0, N0, H = 30.0, 1800.0, 16.0
        N1, N2 = 1500.0, 1200.0
        combined = m.thin_basal_area(B0, N0, H, N2)
        step1 = m.thin_basal_area(B0, N0, H, N1)
        composed = m.thin_basal_area(step1, N1, H, N2)
        self.assertAlmostEqual(combined, composed, places=9)

    def test_positive_sign_mutation_is_killed(self):
        # A mutant with the old (wrong) positive c,d must FAIL the reference regression. The mutant is
        # created transiently in-process and restored, never committed.
        original_c, original_d = m.THIN_C, m.THIN_D
        try:
            m.THIN_C, m.THIN_D = 0.2877, 0.5585
            mutated = m.thin_basal_area(30.0, 1800.0, 16.0, 1200.0)
            self.assertFalse(abs(mutated - self.REFERENCE) < 1e-2,
                             "positive-sign mutant must not reproduce the published reference")
        finally:
            m.THIN_C, m.THIN_D = original_c, original_d
        # The restored implementation still passes.
        self.assertAlmostEqual(m.thin_basal_area(30.0, 1800.0, 16.0, 1200.0), self.REFERENCE, places=3)

    def test_thinning_reduces_basal_area(self):
        self.assertLess(m.thin_basal_area(30.0, 1800.0, 16.0, 1200.0), 30.0)


class AssortmentTests(unittest.TestCase):
    def _check(self, probs, label):
        vals = list(probs.values())
        self.assertTrue(all(math.isfinite(v) for v in vals), label)
        self.assertTrue(all(-1e-12 <= v <= 1.0 + 1e-12 for v in vals), label)
        self.assertAlmostEqual(sum(vals), 1.0, places=9)

    def test_crop_assortment_probabilities_valid(self):
        self._check(m.assortment(35.0, 1200.0, 22.0), "crop")

    def test_thinning_assortment_probabilities_valid(self):
        self._check(m.assortment(35.0, 1500.0, 22.0, B1=24.0, N1=900.0), "thinning")

    def test_invalid_domains_are_reported_not_fabricated(self):
        # Published UNML predictors divide by (D1-D0); a thinning assortment with D1 == D0 is undefined.
        with self.assertRaises(ValueError):
            m.assortment(35.0, 1500.0, 22.0, B1=35.0, N1=1500.0, thinning=True)
        # Nonphysical inputs must be rejected clearly rather than silently producing values.
        for args in ((0.0, 1200.0, 22.0), (35.0, 0.0, 22.0), (35.0, 1200.0, 0.0)):
            with self.subTest(args=args):
                with self.assertRaises(ValueError):
                    m.assortment(*args)
        with self.assertRaises(ValueError):
            m.assortment(35.0, 1500.0, 22.0, B1=-1.0, N1=900.0, thinning=True)

    def test_assortment_finite_across_a_grid(self):
        for B in (10.0, 25.0, 45.0, 70.0):
            for N in (400.0, 1000.0, 1800.0, 3000.0):
                for H in (8.0, 15.0, 24.0, 33.0):
                    with self.subTest(B=B, N=N, H=H):
                        self._check(m.assortment(B, N, H, B1=B * 0.70, N1=N * 0.50), "grid")


class VolumeQmdTests(unittest.TestCase):
    def test_volume_finite_positive(self):
        for B, N, H in ((20.0, 1200.0, 14.0), (40.0, 800.0, 24.0), (55.0, 600.0, 28.0)):
            with self.subTest(B=B, N=N, H=H):
                v = m.volume(B, N, H)
                self.assertTrue(math.isfinite(v) and v > 0)

    def test_qmd_formula(self):
        # B/N = mean tree basal area; Dq = sqrt((B/N) * 40000/pi)
        self.assertAlmostEqual(m.qmd(30.0, 1000.0), math.sqrt(0.03 * 40000.0 / math.pi), places=9)

    def test_low_stocking_volume_is_raw_negative_not_clamped(self):
        # The published 2006 volume equation has no protective low-stock asymptote: at N ~ 5 stems/ha it
        # returns a negative finite volume. The model must expose that raw prediction, never silently clamp
        # it to zero (domain guards only apply to nonpositive B/N/H).
        v = m.volume(30.0, 4.69, 30.0)
        self.assertTrue(math.isfinite(v))
        self.assertLess(v, 0.0)
        self.assertNotEqual(v, 0.0)


if __name__ == "__main__":
    unittest.main(verbosity=2)
