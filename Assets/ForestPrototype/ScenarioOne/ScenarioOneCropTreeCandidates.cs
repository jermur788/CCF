using System;
using System.Collections.Generic;
using System.Text;
using Stem = ScenarioOneCoverage.Stem;

// Spacing-only Crop Tree candidates outside the sample (MFM-04 domain for MFM-05, D-065 P2, spec section 8). Plain data, no UnityEngine.
//
// A candidate says WHERE TO INSPECT, nothing more. Its only inputs are tree positions, species and the player's own state: no diameter,
// height, competition, dominance, stem form or any hidden quality value takes part, so it can never be read as an expert quality
// judgement. A candidate never becomes a Crop Tree here; the player confirms or rejects each one individually after inspecting it in 3D.
//
// V1 order (MFM-03 "unit-centre-first", GAMEPLAY CALIBRATION): covered areas in rack order then Low, Centre, High; inside an area, stems by
// distance to the mean position of the area's eligible stems, ties by x, then z, then id. A stem is suggested when it is at least
// CandidateSpacingM from every Crop Tree and every earlier suggestion.
//
// Batch semantics (V1 default, MFM-03 review section 5 item 4): the suggestion set does not change because the player decided on one of its
// members. Confirmed and rejected candidates keep their place in the sequence (a rejection leaves its position empty and is not silently
// replaced), so the caller must pass back the ids it has recorded. Any other Crop Tree (for example a manual designation) blocks around it.
public static class ScenarioOneCropTreeCandidates
{
    public enum Decision { Open, Confirmed, Rejected }
    public enum Navigation { Previous, Next }

    // A navigation/selection target, never a designation or evidence of player inspection.
    public readonly struct InspectionTarget
    {
        public readonly string TreeId, AreaId;
        public readonly float X, Z;
        public readonly int Order;
        public CandidateReason Reason => CandidateReason.SpacingOnlyInspectToJudge;
        internal InspectionTarget(Candidate candidate)
        {
            TreeId = candidate.TreeId; AreaId = candidate.AreaId;
            X = candidate.X; Z = candidate.Z; Order = candidate.Order;
        }
    }

    // Only undecided candidates are actionable. Confirmed/rejected entries stay in the batch for spacing/history.
    // Callers must regenerate from current authoritative state before acting; selecting a target does not confirm it.
    public static bool TryGetInspectionTarget(Result result, string treeId, out InspectionTarget target)
    {
        target = default;
        if (result == null || !result.Valid || string.IsNullOrEmpty(treeId)) return false;
        foreach (Candidate candidate in result.Candidates)
            if (candidate.TreeId == treeId && candidate.Decision == Decision.Open)
            {
                target = new InspectionTarget(candidate);
                return true;
            }
        return false;
    }

    // No wrap at either end. An empty current id starts at the first/last open entry; an unknown id fails safely.
    // A decided entry can still anchor navigation, so deciding one tree can advance to the next open tree.
    public static bool TryMoveInspectionTarget(Result result, string currentTreeId, Navigation direction, out InspectionTarget target)
    {
        target = default;
        if (result == null || !result.Valid || (direction != Navigation.Next && direction != Navigation.Previous)) return false;
        int step = direction == Navigation.Next ? 1 : -1;
        int index = step > 0 ? -1 : result.Candidates.Count;
        if (!string.IsNullOrEmpty(currentTreeId))
        {
            index = result.Candidates.FindIndex(c => c.TreeId == currentTreeId);
            if (index < 0) return false;
        }
        for (index += step; index >= 0 && index < result.Candidates.Count; index += step)
            if (result.Candidates[index].Decision == Decision.Open)
            {
                target = new InspectionTarget(result.Candidates[index]);
                return true;
            }
        return false;
    }
    // The single closed reason for every V1 candidate.
    public enum CandidateReason { SpacingOnlyInspectToJudge }

    public sealed class Candidate
    {
        // 1-based place in the queue.
        public int Order;
        public string TreeId = "", AreaId = "";
        public float X, Z;
        // Distance to the nearest Crop Tree or earlier suggestion when it was placed (infinite when there was none).
        public double NearestBlockerM;
        public Decision Decision;
        public CandidateReason Reason = CandidateReason.SpacingOnlyInspectToJudge;
    }

    public sealed class Result
    {
        public bool Valid;
        public string Problem = "";
        public string RuleVersion = ScenarioOnePrescriptionCalibration.RuleVersion, CalibrationVersion = "";
        public double SpacingM;
        public readonly List<Candidate> Candidates = new List<Candidate>();
        // Covered areas that received no candidate.
        public readonly List<string> AreasWithoutCandidate = new List<string>();
        // Recorded decisions whose tree is no longer in the suggestion set (felled, died, area no longer covered): the caller reconciles them.
        public readonly List<string> StaleDecisions = new List<string>();

        public string Signature()
        {
            if (!Valid) return "INVALID " + Problem;
            var sb = new StringBuilder(RuleVersion + " cal=" + CalibrationVersion + " spacing=" + ScenarioOneCoverage.F(SpacingM));
            foreach (Candidate c in Candidates) sb.Append(' ').Append(c.Order).Append(':').Append(c.TreeId).Append('@').Append(c.AreaId).Append('=').Append(c.Decision);
            sb.Append(" none=").Append(string.Join(",", AreasWithoutCandidate)).Append(" stale=").Append(string.Join(",", StaleDecisions));
            return sb.ToString();
        }
    }

    public static Result Suggest(ScenarioOneCoverage.Result coverage, IReadOnlyList<Stem> currentStems, IReadOnlyCollection<string> sampleCropTreeIds,
        IReadOnlyCollection<string> confirmedIds, IReadOnlyCollection<string> rejectedIds, ScenarioOnePrescriptionCalibration calibration)
    {
        var result = new Result();
        if (calibration == null || (!(calibration.CandidateSpacingM > 0) || double.IsInfinity(calibration.CandidateSpacingM))) return Fail(result, "No positive candidate spacing was supplied.");
        result.CalibrationVersion = calibration.Version;
        result.SpacingM = calibration.CandidateSpacingM;
        if (coverage == null || !coverage.Valid) return Fail(result, "There is no valid coverage result.");
        List<Stem> sorted = ScenarioOneCoverage.SortedStems(currentStems, out string duplicate);
        if (duplicate != null) return Fail(result, "Tree id " + duplicate + " appears more than once.");
        var living = new Dictionary<string, Stem>(StringComparer.Ordinal);
        foreach (Stem s in sorted) if (s.Living) living[s.Id] = s;
        var confirmed = new HashSet<string>(confirmedIds ?? new string[0], StringComparer.Ordinal);
        var rejected = new HashSet<string>(rejectedIds ?? new string[0], StringComparer.Ordinal);
        foreach (string id in confirmed)
            if (rejected.Contains(id)) return Fail(result, "Candidate " + id + " is recorded as both confirmed and rejected.");

        var sampleCrops = new HashSet<string>(sampleCropTreeIds ?? new string[0], StringComparer.Ordinal);

        // The sequence: eligible stems of covered areas. A confirmed candidate is now a Crop Tree but keeps its place, so the set is stable.
        var sequence = new List<KeyValuePair<string, Stem>>();
        var inSequence = new HashSet<string>(StringComparer.Ordinal);
        var coveredAreas = new List<string>();
        var areas = new List<ScenarioOneCoverage.Area>(coverage.Areas);
        areas.Sort((a, b) =>
        {
            int c = a.RackId.CompareTo(b.RackId);
            if (c == 0) c = a.Segment.CompareTo(b.Segment);
            return c != 0 ? c : string.CompareOrdinal(a.Id, b.Id);
        });
        foreach (ScenarioOneCoverage.Area area in areas)
        {
            if (area.IsSource || area.State != ScenarioOneCoverage.State.Covered) continue;
            coveredAreas.Add(area.Id);
            var eligible = new List<Stem>();
            var areaIds = new SortedSet<string>(area.StemIds, StringComparer.Ordinal);
            foreach (string id in areaIds)
                if (living.TryGetValue(id, out Stem s) && s.IsSitka && !s.FellMark && !sampleCrops.Contains(id)
                    && (!s.Crop || confirmed.Contains(id))) eligible.Add(s);
            if (eligible.Count == 0) continue;
            double cx = 0, cz = 0;
            foreach (Stem s in eligible) { cx += s.X; cz += s.Z; }
            cx /= eligible.Count; cz /= eligible.Count;
            eligible.Sort((a, b) =>
            {
                double da = (a.X - cx) * (a.X - cx) + (a.Z - cz) * (a.Z - cz), db = (b.X - cx) * (b.X - cx) + (b.Z - cz) * (b.Z - cz);
                int c = da.CompareTo(db);
                if (c == 0) c = a.X.CompareTo(b.X);
                if (c == 0) c = a.Z.CompareTo(b.Z);
                return c != 0 ? c : string.CompareOrdinal(a.Id, b.Id);
            });
            foreach (Stem s in eligible) { sequence.Add(new KeyValuePair<string, Stem>(area.Id, s)); inSequence.Add(s.Id); }
        }

        // Blockers: the sample's Crop Trees and every other living Crop Tree that is not itself in the sequence.
        var placed = new List<Stem>();
        foreach (Stem s in living.Values)
            if ((s.Crop || sampleCrops.Contains(s.Id)) && !inSequence.Contains(s.Id)) placed.Add(s);
        placed.Sort((a, b) => string.CompareOrdinal(a.Id, b.Id));

        double spacingSq = calibration.CandidateSpacingM * calibration.CandidateSpacingM;
        var withCandidate = new HashSet<string>(StringComparer.Ordinal);
        var shown = new HashSet<string>(StringComparer.Ordinal);
        foreach (KeyValuePair<string, Stem> entry in sequence)
        {
            Stem s = entry.Value;
            double nearestSq = double.PositiveInfinity;
            foreach (Stem p in placed)
            {
                double dx = (double)s.X - p.X, dz = (double)s.Z - p.Z, dSq = dx * dx + dz * dz;
                if (dSq < nearestSq) nearestSq = dSq;
            }
            if (nearestSq < spacingSq) continue;
            placed.Add(s);
            shown.Add(s.Id);
            withCandidate.Add(entry.Key);
            result.Candidates.Add(new Candidate
            {
                Order = result.Candidates.Count + 1, TreeId = s.Id, AreaId = entry.Key, X = s.X, Z = s.Z,
                NearestBlockerM = Math.Sqrt(nearestSq),
                Decision = confirmed.Contains(s.Id) ? Decision.Confirmed : rejected.Contains(s.Id) ? Decision.Rejected : Decision.Open
            });
        }
        foreach (string id in coveredAreas) if (!withCandidate.Contains(id)) result.AreasWithoutCandidate.Add(id);
        var stale = new SortedSet<string>(StringComparer.Ordinal);
        foreach (string id in confirmed) if (!shown.Contains(id)) stale.Add(id);
        foreach (string id in rejected) if (!shown.Contains(id)) stale.Add(id);
        result.StaleDecisions.AddRange(stale);
        result.Valid = true;
        return result;
    }

    private static Result Fail(Result result, string problem)
    {
        result.Valid = false;
        result.Problem = problem;
        return result;
    }
}
