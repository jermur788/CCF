"""Unit tests for focal_boundary_ci.py. Standard library only.

Run:  python3 -I -B -m unittest discover -s Tools/Research/ScenarioOneFocalBoundaryCI -p 'test_*.py' -v
(-I drops the current directory from sys.path, so run with  -s  as above and the
 test file inserts its own directory explicitly.)

The optional raw-evidence test runs only if the pinned #460 evidence root exists.
"""
import copy
import json
import math
import os
import sys
import tempfile
import unittest

sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))
import focal_boundary_ci as fb  # noqa: E402
from focal_boundary_ci import Stem  # noqa: E402


def _read_bytes(path):
    with open(path, "rb") as fh:
        return fh.read()


def row(tid, x, z, dbh, side="north", half=10.0, ci=0.0):
    return {"id": tid, "species": "sitka-spruce", "age": 20, "x": x, "z": z, "dbh": dbh, "ci": ci,
            "edge": half - max(abs(x), abs(z)), "side": side}


class HegyiArithmetic(unittest.TestCase):
    def test_hand_calculated_term(self):
        # (20 cm neighbour / 10 cm target) / 4 m = 0.5
        self.assertEqual(fb.hegyi_term_f64(20.0, 10.0, 4.0), 0.5)
        self.assertEqual(fb.hegyi_term_f32(20.0, 10.0, 4.0), 0.5)

    def test_floors_match_source(self):
        # target DBH floor 1 cm: (10 / max(1,0.2)) / 2 = 5
        self.assertEqual(fb.hegyi_term_f64(10.0, 0.2, 2.0), 5.0)
        # distance floor 0.5 m: (10/10)/max(0.5,0.1) = 2
        self.assertEqual(fb.hegyi_term_f64(10.0, 10.0, 0.1), 2.0)
        self.assertEqual(fb.hegyi_term_f32(10.0, 10.0, 0.1), 2.0)

    def test_target_and_neighbour_dbh_not_swapped(self):
        big, small = Stem("A", 0, 0, 20.0), Stem("B", 5, 0, 10.0)
        ci_big, _ = fb.finite_ci(big, [big, small], "float64")      # neighbour 10 on target 20 -> 0.5/5
        ci_small, _ = fb.finite_ci(small, [big, small], "float64")  # neighbour 20 on target 10 -> 2/5
        self.assertAlmostEqual(ci_big, 0.1, places=12)
        self.assertAlmostEqual(ci_small, 0.4, places=12)

    def test_cutoff_inclusive_at_exactly_8m_and_exclusive_beyond(self):
        t = Stem("T", 0, 0, 10.0)
        for arith in ("float64", "float32"):
            at = Stem("N1", 8.0, 0, 10.0)
            inside = Stem("N2", 7.9999, 0, 10.0)
            beyond = Stem("N3", 8.0001, 0, 10.0)
            self.assertEqual(len(fb.contributions(t, [at], arith)), 1, arith)
            self.assertEqual(len(fb.contributions(t, [inside], arith)), 1, arith)
            self.assertEqual(len(fb.contributions(t, [beyond], arith)), 0, arith)

    def test_self_pair_excluded_but_coincident_other_tree_counts(self):
        t = Stem("T", 1, 1, 12.0)
        self.assertEqual(fb.finite_ci(t, [t], "float64")[0], 0.0)
        twin = Stem("U", 1, 1, 12.0)  # distance 0 -> floor 0.5 -> (12/12)/0.5 = 2
        self.assertEqual(fb.finite_ci(t, [t, twin], "float64")[0], 2.0)

    def test_pair_exclusion_neighbour_to_neighbour_not_counted(self):
        # Two neighbours 1 m apart must not add a mutual term to the focal tree's CI.
        t = Stem("T", 0, 0, 10.0)
        a, b = Stem("A", 4, 0, 10.0), Stem("B", 4, 1, 10.0)
        ci, terms = fb.finite_ci(t, [t, a, b], "float64")
        self.assertEqual(len(terms), 2)
        self.assertAlmostEqual(ci, 1 / 4 + 1 / math.hypot(4, 1), places=12)

    def test_hash_helpers_known_vectors(self):
        # FNV-1a 32-bit of "a" and the public murmur3 fmix32(1) test vector.
        self.assertEqual(fb.fnv1a32("a"), 0xE40C292C)
        self.assertEqual(fb.fnv1a32(""), 2166136261)
        self.assertEqual(fb.mix32(1), 0x514E28B7)
        self.assertEqual(fb.mix32(0), 0)

    def test_float32_emulation_rounds(self):
        self.assertNotEqual(fb.f32(0.1), 0.1)
        self.assertEqual(fb.f32(fb.f32(0.1)), fb.f32(0.1))
        self.assertEqual(fb.f32(0.5), 0.5)


class ExternalContextControls(unittest.TestCase):
    def setUp(self):
        self.focal = Stem("F", 0, 0, 10.0)
        self.stems = [self.focal, Stem("N", 3, 0, 10.0)]

    def test_zero_external_trees_is_identity(self):
        for arith in ("float64", "float32"):
            ci_f, ci_c, ext_sum, _, e_terms = fb.context_ci(self.focal, self.stems, [], arith)
            self.assertEqual(ci_c, ci_f)
            self.assertEqual(ext_sum, 0.0)
            self.assertEqual(e_terms, [])

    def test_external_beyond_reach_has_zero_effect(self):
        ext = [Stem("X_far", 8.0001, 0, 15.0)]
        for arith in ("float64", "float32"):
            ci_f, ci_c, ext_sum, _, e_terms = fb.context_ci(self.focal, self.stems, ext, arith)
            self.assertEqual(ci_c, ci_f)
            self.assertEqual(e_terms, [])

    def test_external_in_reach_hand_calculation(self):
        ext = [Stem("X_1", -4.0, 0, 20.0)]  # (20/10)/4 = 0.5
        ci_f, ci_c, ext_sum, _, _ = fb.context_ci(self.focal, self.stems, ext, "float64")
        self.assertEqual(ext_sum, 0.5)
        self.assertAlmostEqual(ci_c - ci_f, 0.5, places=12)

    def test_weight_scales_contribution_linearly(self):
        full = [Stem("X_1", -4.0, 0, 20.0)]
        half = [Stem("X_1", -4.0, 0, 20.0, weight=0.5)]
        _, _, s_full, _, _ = fb.context_ci(self.focal, self.stems, full, "float64")
        _, _, s_half, _, _ = fb.context_ci(self.focal, self.stems, half, "float64")
        self.assertAlmostEqual(s_half, 0.5 * s_full, places=12)

    def test_context_does_not_alter_finite_part(self):
        ext = [Stem("X_1", -4.0, 0, 20.0)]
        a, _ = fb.finite_ci(self.focal, self.stems, "float64")
        b, _, _, _, _ = fb.context_ci(self.focal, self.stems, ext, "float64")
        self.assertEqual(a, b)


class PropertyGeometry(unittest.TestCase):
    def test_boundary_classification_20m_and_30m(self):
        for size in (20.0, 30.0):
            h = size / 2
            self.assertFalse(fb.is_outside_property(h, 0, h))          # boundary line is inside
            self.assertFalse(fb.is_outside_property(-h, h, h))         # corner is inside
            self.assertTrue(fb.is_outside_property(h + 1e-4, 0, h))
            self.assertTrue(fb.is_outside_property(0, -h - 0.45, h))
            self.assertFalse(fb.is_outside_property(0, 0, h))

    def test_edge_and_outside_distance(self):
        self.assertEqual(fb.edge_distance_inside(-8.0, 3.0, 10.0), 2.0)
        self.assertEqual(fb.distance_outside_property(13.0, 0.0, 10.0), 3.0)
        self.assertAlmostEqual(fb.distance_outside_property(13.0, 14.0, 10.0), 5.0, places=12)
        self.assertEqual(fb.distance_outside_property(1.0, 1.0, 10.0), 0.0)

    def test_20m_vs_30m_lattice_identity(self):
        o20, s20 = fb.lattice_geometry(10)
        o30, s30 = fb.lattice_geometry(16)
        self.assertAlmostEqual(o20, -8.55, places=9)
        self.assertAlmostEqual(o30, -14.25, places=9)
        e20 = fb.build_external_stems(10, 10.0, 15.0, "primary")
        e30 = fb.build_external_stems(16, 15.0, 15.0, "primary")
        self.assertTrue(e20 and e30)
        self.assertNotEqual({(round(s.x, 6), round(s.z, 6)) for s in e20},
                            {(round(s.x, 6), round(s.z, 6)) for s in e30})

    def test_external_stems_all_outside_never_source_ids_and_deterministic(self):
        for per_axis, half in ((10, 10.0), (16, 15.0)):
            a = fb.build_external_stems(per_axis, half, 15.0, "primary")
            b = fb.build_external_stems(per_axis, half, 15.0, "primary")
            self.assertEqual(a, b)
            self.assertTrue(all(fb.is_outside_property(s.x, s.z, half) for s in a))
            self.assertTrue(all(s.id.startswith("X_") for s in a))
            self.assertEqual(len({s.id for s in a}), len(a))

    def test_thinning_fraction_close_to_stocking_target(self):
        full = fb.build_external_stems(16, 15.0, 15.0, "expected")
        kept = fb.build_external_stems(16, 15.0, 15.0, "primary")
        p = fb.stocking_fraction()
        self.assertAlmostEqual(p, 2100.0 / (10000.0 / 1.9 ** 2), places=9)
        self.assertLess(abs(len(kept) / len(full) - p), 0.05)
        self.assertTrue(all(s.weight == p for s in full))

    def test_interior_focal_far_from_boundary_gets_zero_external(self):
        # 30 m property: a stem 10 m from the nearest boundary is >8 m from every external node.
        focal = Stem("F", 0.0, 0.0, 15.0)
        ext = fb.build_external_stems(16, 15.0, 15.0, "primary")
        _, ci_c, ext_sum, _, e_terms = fb.context_ci(focal, [focal], ext, "float64")
        self.assertEqual(e_terms, [])
        self.assertEqual(ext_sum, 0.0)


class DataGuards(unittest.TestCase):
    def good_rows(self):
        return [row("P0000", 0, 0, 15.0), row("P0001", 3, 0, 14.0)]

    def test_accepts_good_rows(self):
        self.assertEqual(len(fb.stems_from_rows(self.good_rows(), "t")), 2)

    def test_rejects_duplicate_ids(self):
        rows = self.good_rows() + [row("P0000", 5, 5, 12.0)]
        with self.assertRaises(fb.DataGateError):
            fb.stems_from_rows(rows, "t")

    def test_rejects_nan_inf_and_nonpositive_dbh(self):
        for bad in (float("nan"), float("inf"), 0.0, -3.0):
            rows = self.good_rows()
            rows[0]["dbh"] = bad
            with self.assertRaises(fb.DataGateError, msg=str(bad)):
                fb.stems_from_rows(rows, "t")

    def test_rejects_missing_field_and_empty(self):
        rows = self.good_rows()
        del rows[0]["x"]
        with self.assertRaises(fb.DataGateError):
            fb.stems_from_rows(rows, "t")
        with self.assertRaises(fb.DataGateError):
            fb.stems_from_rows([], "t")

    def test_select_focal_is_geometry_only_and_keeps_anchors(self):
        rows = [row(f"P{i:04d}", x, 5.0, 15.0, side="north") for i, x in enumerate([-9, -6, -3, 0, 2])]
        rows.append(row("P9999", 0, 0, 15.0, side="rack"))
        picked = fb.select_focal(rows, ("P0000", "P0001"))
        ids = [p[0] for p in picked]
        self.assertEqual(ids[:2], ["P0000", "P0001"])
        self.assertEqual(len(set(ids)), 4)
        self.assertNotIn("P9999", ids)  # corridor stems are not retained stems
        self.assertEqual(ids[2], "P0002")  # edge 10-max(|x|,|z|)=5 for P0002..P0004; tie -> lowest id

    def test_select_focal_missing_anchor_raises(self):
        with self.assertRaises(fb.DataGateError):
            fb.select_focal([row("P0000", 0, 0, 15.0)], ("P0000", "PXXXX"))


class EndToEndSynthetic(unittest.TestCase):
    """Tiny synthetic candidate: not 'original data'; only checks the pipeline arithmetic."""

    def make(self):
        # property 10 m (half 5). lattice 3 -> origin -1.9. Focal at (4.5, 0) is 0.5 m inside the +x edge.
        stems_xy = [("P0000", 4.5, 0.0, 20.0), ("P0001", 1.0, 0.0, 10.0), ("P0002", -3.0, 2.0, 12.0),
                    ("P0003", 0.0, -4.0, 14.0), ("P0004", -4.0, -4.0, 16.0)]
        rows = []
        for tid, x, z, d in stems_xy:
            rows.append(row(tid, x, z, d, side="north" if z >= 0 else "south", half=5.0))
        stems = [Stem(r["id"], r["x"], r["z"], r["dbh"]) for r in rows]
        for r in rows:  # record exact CI so the whole-stand recreation check has something to match
            r["ci"] = fb.finite_ci(next(s for s in stems if s.id == r["id"]), stems, "float64")[0]
        return {"id": "T", "size": 10.0, "lattice": 3, "trees": rows,
                "states": [{"year": 0, "living": 5}], "initial": 5}, rows

    def test_pipeline_matches_hand_arithmetic_and_negative_control(self):
        data, rows = self.make()
        res = fb.analyse_candidate("T", 10.0, data, ("P0000", "P0003"))
        f0 = next(f for f in res["focal"] if f["focal_id"] == "P0000")
        # Finite CI of P0000 by hand: neighbours within 8 m: P0001 (3.5 m), P0003 (dist hypot(4.5,4)), P0002 7.5 m? hypot(7.5,2)=7.76 -> in
        expect = (10 / 20) / 3.5 + (14 / 20) / math.hypot(4.5, 4.0) + (12 / 20) / math.hypot(7.5, 2.0)
        expect += (16 / 20) / math.hypot(8.5, 4.0) if math.hypot(8.5, 4.0) <= 8 else 0.0
        self.assertAlmostEqual(f0["ci_finite_f64"], expect, places=12)
        self.assertTrue(f0["negctl_zero_external_equal_f64"] and f0["negctl_zero_external_equal_f32"])
        self.assertGreaterEqual(f0["external_neighbour_count"], 1)  # edge stem must see the outer lattice
        self.assertAlmostEqual(f0["delta_ci_f64"], f0["external_contribution_f64"], places=12)
        self.assertEqual(res["negative_control_beyond_cutoff"]["external_sum"], 0.0)
        self.assertLess(res["finite_recreation_check"]["max_abs_diff_float64_vs_recorded"], 1e-12)

    def test_repeated_runs_identical(self):
        data, _ = self.make()
        a = fb.analyse_candidate("T", 10.0, copy.deepcopy(data), ("P0000", "P0003"))
        b = fb.analyse_candidate("T", 10.0, copy.deepcopy(data), ("P0000", "P0003"))
        self.assertEqual(json.dumps(a, sort_keys=True), json.dumps(b, sort_keys=True))

    def test_outputs_written_to_given_dir_only(self):
        data, _ = self.make()
        res = {"candidates": [fb.analyse_candidate("T", 10.0, data, ("P0000", "P0003"))]}
        with tempfile.TemporaryDirectory() as d:
            fb.write_outputs(d, res)
            self.assertEqual(sorted(os.listdir(d)), ["external-stems.csv", "focal-rows.csv", "results.json"])


@unittest.skipUnless(os.path.isdir(fb.DEFAULT_EVIDENCE_ROOT), "pinned #460 evidence root not present")
class RawEvidenceIntegration(unittest.TestCase):
    def test_full_run_is_deterministic_and_leaves_evidence_untouched(self):
        with tempfile.TemporaryDirectory() as d1, tempfile.TemporaryDirectory() as d2:
            self.assertEqual(fb.main(["--out-dir", d1]), 0)
            self.assertEqual(fb.main(["--out-dir", d2]), 0)
            for name in ("results.json", "focal-rows.csv", "external-stems.csv"):
                self.assertEqual(_read_bytes(os.path.join(d1, name)), _read_bytes(os.path.join(d2, name)), name)
            res = json.loads(_read_bytes(os.path.join(d1, "results.json")))
            self.assertTrue(res["custody"]["metadata_unchanged"] and res["custody"]["key_file_hashes_unchanged"])
            self.assertEqual([c["living_stems"] for c in res["candidates"]], [82, 189])
            self.assertEqual(sum(len(c["focal"]) for c in res["candidates"]), 8)

    def test_refuses_output_inside_evidence_root(self):
        self.assertEqual(fb.main(["--out-dir", os.path.join(fb.DEFAULT_EVIDENCE_ROOT, "x")]), 1)


if __name__ == "__main__":
    unittest.main()
