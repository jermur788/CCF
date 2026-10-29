using System;
using System.Collections.Generic;
using System.Text;
using Stem = ScenarioOneCoverage.Stem;

// The treatment prescription derived from the player's executed sample (MFM-04, D-065, spec section 7). Plain data, no UnityEngine.
//
// Three pure questions, answered from present-state, inspectable information only:
//   * Derive   - what did the player demonstrate? Their accepted Crop Trees, their executed in-footprint removals, the least release they
//                gave any Crop Tree and the basal-area share they removed, read through the existing lesson rules (ScenarioOneRackBay).
//   * CheckFit - an honesty device, not a score: replay the derived rule on the original sample and list exactly which of the player's
//                decisions it would have made differently.
//   * ProposeRelease - the release the rule WOULD propose around Crop Trees the player has confirmed, inside covered areas, never
//                above each area's basal-area ceiling.
//
// Nothing here writes state. A proposal is plain data: it is not a TreeMarkType.Fell mark, not a work order and not approved. Storing,
// showing and approving proposals belong to later layers (MFM-05/06/07), and the Work Plan stays the commitment gate.
//
// The closed rule vocabulary (spec 7.2): competitor order is the authoritative CompetitionShare inside the 8 m Hegyi reach, strongest
// first, ties by tree id; the release target is the player's least executed release, never below the protected 30% teaching minimum;
// the per-area removal ceiling is the sample's own removed basal-area share, never above the protected 40% safeguard. No DBH rank,
// dominance, stem quality or any hidden variable takes part. V1 removes only Sitka adults.
public static class ScenarioOnePrescription
{
    // Shares within this of a target or ceiling are treated as equal: the lesson rules report shares as float.
    private const double ShareTolerance = 1e-6;

    public enum Status { Ready, InvalidGeometry, SampleNotQualified, SampleHasOpenWork, SampleHasNonSitka }

    public readonly struct ReleaseRule
    {
        public readonly double TargetShare, BasalCeilingShare;
        private ReleaseRule(double target, double ceiling) { TargetShare = target; BasalCeilingShare = ceiling; }
        // The protected values bound every rule: the target is never below 30%, the ceiling never above 40%.
        public static ReleaseRule Create(double targetShare, double basalCeilingShare)
            => new ReleaseRule(Math.Max(targetShare, ScenarioOneRackBay.FirstReleaseShare),
                Math.Min(Math.Max(basalCeilingShare, 0.0), ScenarioOneRackBay.MaxRemovedBasalShare));
        public string Signature() => "target=" + ScenarioOneCoverage.F(TargetShare) + " ceiling=" + ScenarioOneCoverage.F(BasalCeilingShare);
    }

    // ------------------------------------------------------------------------------------------------------------------- derivation

    public sealed class Derivation
    {
        public Status Status;
        public string Problem = "";
        public string RuleVersion = ScenarioOnePrescriptionCalibration.RuleVersion, CalibrationVersion = "";
        public int TeachingRackId;
        public float AnchorX, AnchorZ;
        // The lesson's accepted Crop Trees (spacing-checked, by id) and the executed in-footprint removals, by id.
        public readonly List<string> SampleCropTreeIds = new List<string>();
        public readonly List<string> SampleRemovalIds = new List<string>();
        public double PlayerMinimumReleaseShare, SampleRemovedBasalShare;
        // What the player chose for the wood, for removals serving the Crop Trees. Recorded only; proposals assign no disposition.
        public int SoldRemovals, DeadwoodRemovals;
        public ReleaseRule Rule;
        public bool Ready => Status == Status.Ready;

        public string Signature()
            => RuleVersion + " cal=" + CalibrationVersion + " " + Status + " " + Problem + " teach=" + TeachingRackId
                + " anchor=" + ScenarioOneCoverage.F(AnchorX) + "," + ScenarioOneCoverage.F(AnchorZ)
                + " crops=" + string.Join(",", SampleCropTreeIds) + " removals=" + string.Join(",", SampleRemovalIds)
                + " minRelease=" + ScenarioOneCoverage.F(PlayerMinimumReleaseShare) + " removedBa=" + ScenarioOneCoverage.F(SampleRemovedBasalShare)
                + " sold=" + SoldRemovals + " deadwood=" + DeadwoodRemovals + " " + Rule.Signature();
    }

    // Reads the executed sample through the existing lesson rules. Ready only when the lesson's own release has been executed and passed,
    // and the sample has no unexecuted planned or marked work left (a further release must be carried out before it can be generalised).
    public static Derivation Derive(ScenarioOneRackSampleGeometry.Result geometry, IReadOnlyList<Stem> stems,
        IReadOnlyList<ScenarioOneRackBay.Order> orders, ScenarioOnePrescriptionCalibration calibration)
    {
        var d = new Derivation { CalibrationVersion = calibration?.Version ?? "" };
        if (geometry == null || !geometry.Valid)
            return Stop(d, Status.InvalidGeometry, "There is no valid confirmed rack-served sample" + (geometry != null ? ": " + geometry.Problem : "."));
        d.TeachingRackId = geometry.TeachingRack.Id;
        d.AnchorX = geometry.Anchor.X; d.AnchorZ = geometry.Anchor.Z;

        List<Stem> sorted = ScenarioOneCoverage.SortedStems(stems, out string duplicate);
        if (duplicate != null) return Stop(d, Status.SampleNotQualified, "Tree id " + duplicate + " appears more than once.");
        var trees = new List<ScenarioOneRackBay.Tree>(sorted.Count);
        foreach (Stem s in sorted)
            trees.Add(new ScenarioOneRackBay.Tree(s.Id, s.X, s.Z, s.DbhCm, s.Living, s.Living && s.Crop, s.Living && s.FellMark));
        var orderList = orders ?? new ScenarioOneRackBay.Order[0];
        ScenarioOneRackBay.Analysis lesson = ScenarioOneRackBay.AnalyseTeachingRack(trees, orderList, geometry.Teaching,
            geometry.TeachingRack.Id, geometry.Racks.Count, geometry.TeachingRack.Index);

        var completed = new HashSet<string>(StringComparer.Ordinal);
        var open = new HashSet<string>(StringComparer.Ordinal);
        foreach (ScenarioOneRackBay.Order order in orderList)
        {
            if (order.Completed) completed.Add(order.TargetId);
            else if (order.Open) open.Add(order.TargetId);
        }
        foreach (Stem s in sorted)
        {
            if (!geometry.Teaching.Contains(s.X, s.Z)) continue;
            if (!s.Living) { if (completed.Contains(s.Id)) d.SampleRemovalIds.Add(s.Id); continue; }
            if (!s.IsSitka) return Stop(d, Status.SampleHasNonSitka, "The sample holds a non-Sitka adult (" + s.Id + "); V1 is Sitka-only.");
            if (s.FellMark || open.Contains(s.Id))
                return Stop(d, Status.SampleHasOpenWork, "Sample tree " + s.Id + " still has marked or planned felling; carry it out first.");
        }
        foreach (ScenarioOneRackBay.FutureTree future in lesson.Future) d.SampleCropTreeIds.Add(future.Id);
        d.SampleCropTreeIds.Sort(StringComparer.Ordinal);
        d.PlayerMinimumReleaseShare = lesson.MinExecuted;
        d.SampleRemovedBasalShare = lesson.RemovedBasalShare;
        d.SoldRemovals = lesson.SoldExecuted;
        d.DeadwoodRemovals = lesson.DeadwoodExecuted;
        if (!lesson.ReleaseExecuted)
            return Stop(d, Status.SampleNotQualified, "The sample's own release has not been executed and passed (Crop Trees " + lesson.FutureCount
                + "/" + ScenarioOneRackBay.FutureTreesWanted + ", spread " + lesson.HasEnoughFuture + ", basal area kept " + lesson.CanopyKept
                + ", least executed release " + ScenarioOneCoverage.F(lesson.MinExecuted) + ").");
        d.Rule = ReleaseRule.Create(lesson.MinExecuted, lesson.RemovedBasalShare);
        d.Status = Status.Ready;
        return d;
    }

    private static Derivation Stop(Derivation d, Status status, string problem)
    {
        d.Status = status;
        d.Problem = problem;
        return d;
    }

    // ------------------------------------------------------------------------------------------------------------------------- fit

    public sealed class FitTree
    {
        public string Id = "";
        public bool PlayerRemoved, RuleRemoved;
        // The Crop Tree this removal served (the one it took the largest Share from, or the one the rule picked it for); "" when it lay
        // outside every Crop Tree's 8 m reach.
        public string ServesCropTreeId = "";
        public double ShareOfCropTree, BasalAreaM2;
    }

    public sealed class FitCropTree
    {
        public string Id = "";
        public double PlayerShare, RuleShare;
        public bool RuleReachedTarget;
    }

    public sealed class FitResult
    {
        public bool Valid;
        public string Problem = "";
        public ReleaseRule Rule;
        public readonly List<FitTree> Agreements = new List<FitTree>();
        public readonly List<FitTree> PlayerOnly = new List<FitTree>();
        public readonly List<FitTree> RuleOnly = new List<FitTree>();
        public readonly List<FitCropTree> CropTrees = new List<FitCropTree>();
        public double SampleBasalAreaM2, PlayerBasalShare, RuleBasalShare;
        public bool RuleLimitedByCeiling;

        public string Signature()
        {
            if (!Valid) return "INVALID " + Problem;
            var sb = new StringBuilder(Rule.Signature());
            void List(string name, List<FitTree> trees)
            {
                sb.Append(' ').Append(name).Append('=');
                foreach (FitTree t in trees) sb.Append(t.Id).Append('>').Append(t.ServesCropTreeId).Append(':').Append(ScenarioOneCoverage.F(t.ShareOfCropTree)).Append(';');
            }
            List("agree", Agreements); List("playerOnly", PlayerOnly); List("ruleOnly", RuleOnly);
            foreach (FitCropTree c in CropTrees)
                sb.Append(" crop ").Append(c.Id).Append(' ').Append(ScenarioOneCoverage.F(c.PlayerShare)).Append('/').Append(ScenarioOneCoverage.F(c.RuleShare))
                    .Append(c.RuleReachedTarget ? " reached" : " short");
            sb.Append(" ba=").Append(ScenarioOneCoverage.F(PlayerBasalShare)).Append('/').Append(ScenarioOneCoverage.F(RuleBasalShare))
                .Append(" limited=").Append(RuleLimitedByCeiling);
            return sb.ToString();
        }
    }

    public static FitResult CheckFit(Derivation derivation, ScenarioOneRackSampleGeometry.Result geometry, IReadOnlyList<Stem> stems,
        IReadOnlyList<ScenarioOneCoverage.Mask> masks)
    {
        if (derivation == null || !derivation.Ready)
            return new FitResult { Problem = "The prescription is not derived: " + (derivation?.Problem ?? "none.") };
        return CheckFit(derivation.Rule, derivation.SampleCropTreeIds, derivation.SampleRemovalIds, geometry, stems, masks);
    }

    // Replays a rule on the sample as it stood before its treatment (the player's removals put back) and compares the two removal sets.
    // The rule may only take what the lesson credits: in-footprint Sitka stems that are not Crop Trees and not under an infrastructure mask.
    public static FitResult CheckFit(ReleaseRule rule, IReadOnlyList<string> cropTreeIds, IReadOnlyCollection<string> playerRemovalIds,
        ScenarioOneRackSampleGeometry.Result geometry, IReadOnlyList<Stem> stems, IReadOnlyList<ScenarioOneCoverage.Mask> masks)
    {
        var fit = new FitResult { Rule = rule };
        if (geometry == null || !geometry.Valid) return FitFail(fit, "There is no valid confirmed rack-served sample.");
        List<Stem> sorted = ScenarioOneCoverage.SortedStems(stems, out string duplicate);
        if (duplicate != null) return FitFail(fit, "Tree id " + duplicate + " appears more than once.");
        var player = new HashSet<string>(playerRemovalIds ?? new string[0], StringComparer.Ordinal);
        var crops = new List<string>(cropTreeIds ?? new string[0]);
        crops.Sort(StringComparer.Ordinal);
        var cropSet = new HashSet<string>(crops, StringComparer.Ordinal);

        // The state before treatment: living trees plus the player's removals as they stood.
        var world = new List<Stem>();
        var byId = new Dictionary<string, Stem>(StringComparer.Ordinal);
        foreach (Stem s in sorted)
            if (s.Living || player.Contains(s.Id)) { world.Add(s); byId[s.Id] = s; }
        foreach (string id in crops)
            if (!byId.ContainsKey(id) || !byId[id].Living) return FitFail(fit, "Crop Tree " + id + " is not a living tree.");
        foreach (string id in player)
            if (!byId.ContainsKey(id) || !geometry.Teaching.Contains(byId[id].X, byId[id].Z))
                return FitFail(fit, "Player removal " + id + " is not a recorded stump in the sample.");

        var removable = new HashSet<string>(StringComparer.Ordinal);
        foreach (Stem s in world)
        {
            if (!geometry.Teaching.Contains(s.X, s.Z)) continue;
            fit.SampleBasalAreaM2 += ScenarioOneCoverage.BasalAreaM2(s.DbhCm);
            if (s.IsSitka && !cropSet.Contains(s.Id) && !s.Crop && !Masked(masks, s.X, s.Z)) removable.Add(s.Id);
        }
        if (fit.SampleBasalAreaM2 <= 0) return FitFail(fit, "The sample holds no basal area.");

        var shares = new Dictionary<string, List<Neighbour>>(StringComparer.Ordinal);
        foreach (string id in crops) shares[id] = Neighbours(byId[id], world);

        // The rule's choices.
        var picked = new Dictionary<string, FitTree>(StringComparer.Ordinal);
        double pickedBa = 0;
        foreach (string id in crops)
        {
            double got = SharesOf(shares[id], picked.ContainsKey);
            foreach (Neighbour n in shares[id])
            {
                if (got >= rule.TargetShare - ShareTolerance) break;
                if (picked.ContainsKey(n.Id) || !removable.Contains(n.Id)) continue;
                double ba = ScenarioOneCoverage.BasalAreaM2(byId[n.Id].DbhCm);
                if ((pickedBa + ba) / fit.SampleBasalAreaM2 > rule.BasalCeilingShare + ShareTolerance) { fit.RuleLimitedByCeiling = true; continue; }
                picked[n.Id] = new FitTree { Id = n.Id, RuleRemoved = true, ServesCropTreeId = id, ShareOfCropTree = n.Share, BasalAreaM2 = ba };
                pickedBa += ba;
                got += n.Share;
            }
        }

        // The player's choices, attributed to the Crop Tree each took the most Share from.
        var all = new SortedDictionary<string, FitTree>(StringComparer.Ordinal);
        foreach (FitTree t in picked.Values) all[t.Id] = t;
        double playerBa = 0;
        foreach (string id in player)
        {
            double ba = ScenarioOneCoverage.BasalAreaM2(byId[id].DbhCm);
            playerBa += ba;
            if (!all.TryGetValue(id, out FitTree t))
            {
                t = new FitTree { Id = id, BasalAreaM2 = ba };
                foreach (string crop in crops)
                    foreach (Neighbour n in shares[crop])
                        if (n.Id == id && n.Share > t.ShareOfCropTree) { t.ServesCropTreeId = crop; t.ShareOfCropTree = n.Share; }
                all[id] = t;
            }
            t.PlayerRemoved = true;
        }
        foreach (FitTree t in all.Values)
            (t.PlayerRemoved && t.RuleRemoved ? fit.Agreements : t.PlayerRemoved ? fit.PlayerOnly : fit.RuleOnly).Add(t);

        foreach (string id in crops)
        {
            double ruleShare = SharesOf(shares[id], picked.ContainsKey);
            fit.CropTrees.Add(new FitCropTree
            {
                Id = id, PlayerShare = SharesOf(shares[id], player.Contains), RuleShare = ruleShare,
                RuleReachedTarget = ruleShare >= rule.TargetShare - ShareTolerance
            });
        }
        fit.PlayerBasalShare = playerBa / fit.SampleBasalAreaM2;
        fit.RuleBasalShare = pickedBa / fit.SampleBasalAreaM2;
        fit.Valid = true;
        return fit;
    }

    private static FitResult FitFail(FitResult fit, string problem)
    {
        fit.Valid = false;
        fit.Problem = problem;
        return fit;
    }

    // --------------------------------------------------------------------------------------------------------------- release proposals

    public enum CropStatus
    {
        Released, LimitedByCeiling, ShortOfTarget, NoCompetitorsInReach,
        // Not used as a Crop Tree for proposals:
        NotFound, NotLiving, NotSitka, NotInCoveredArea, SampleCropTree
    }

    // The single closed reason for every V1 proposed removal.
    public enum ProposalReason { CompetitorHighShareAroundConfirmedCropTree }

    public sealed class Proposal
    {
        public string TreeId = "", AreaId = "", ForCropTreeId = "";
        public double ShareOfCropTree, BasalAreaM2;
        public ProposalReason Reason = ProposalReason.CompetitorHighShareAroundConfirmedCropTree;
    }

    public sealed class CropRelease
    {
        public string Id = "", AreaId = "";
        public CropStatus Status;
        // Share of its current competition the proposals would remove, after all proposals.
        public double PlannedShare;
        public int CompetitorsInReach;
    }

    public sealed class AreaSafeguard
    {
        public string AreaId = "";
        public double BasalAreaM2, ProposedBasalAreaM2, Share, Ceiling;
        public bool Within => Share <= Ceiling + ShareTolerance;
    }

    public sealed class SafeguardReport
    {
        public readonly List<AreaSafeguard> Areas = new List<AreaSafeguard>();
        // Proposed removals that are not living target stems of a covered area: they may not be proposed at all.
        public readonly List<string> OutsideCoveredAreas = new List<string>();
        // True when the coverage input was missing or invalid, so nothing could be checked. This is not the same as an over-ceiling
        // proposal: a caller must never treat an invalid report as approved.
        public bool InvalidInput;
        public string InvalidReason = "";
        public bool AllWithin
        {
            get
            {
                if (InvalidInput) return false;
                if (OutsideCoveredAreas.Count > 0) return false;
                foreach (AreaSafeguard a in Areas) if (!a.Within) return false;
                return true;
            }
        }
    }

    public sealed class ReleasePlan
    {
        public bool Valid;
        public string Problem = "";
        public string RuleVersion = ScenarioOnePrescriptionCalibration.RuleVersion, CalibrationVersion = "";
        public ReleaseRule Rule;
        // Sorted by tree id; each tree at most once.
        public readonly List<Proposal> Proposals = new List<Proposal>();
        // Sorted by Crop Tree id.
        public readonly List<CropRelease> CropTrees = new List<CropRelease>();
        public SafeguardReport Safeguard = new SafeguardReport();

        public string Signature()
        {
            if (!Valid) return "INVALID " + Problem;
            var sb = new StringBuilder(RuleVersion + " cal=" + CalibrationVersion + " " + Rule.Signature());
            foreach (Proposal p in Proposals)
                sb.Append(" p ").Append(p.TreeId).Append('@').Append(p.AreaId).Append('>').Append(p.ForCropTreeId).Append(':').Append(ScenarioOneCoverage.F(p.ShareOfCropTree));
            foreach (CropRelease c in CropTrees)
                sb.Append(" c ").Append(c.Id).Append('@').Append(c.AreaId).Append(' ').Append(c.Status).Append(' ').Append(ScenarioOneCoverage.F(c.PlannedShare));
            foreach (AreaSafeguard a in Safeguard.Areas)
                sb.Append(" s ").Append(a.AreaId).Append(' ').Append(ScenarioOneCoverage.F(a.Share)).Append('/').Append(ScenarioOneCoverage.F(a.Ceiling));
            return sb.ToString();
        }
    }

    // The release the rule would propose around each confirmed Crop Tree (confirmed spacing candidates or the player's own designations
    // outside the sample). Crop Trees are served in id order; a removal already proposed counts for every Crop Tree it competes with.
    // Only living Sitka target stems of COVERED areas may be proposed; Crop Trees, confirmed trees, player-marked trees and keepStanding
    // (for example candidates the player has not decided on yet) never are. Each removal's basal area is charged to its own area, and no
    // area may exceed the rule's ceiling.
    public static ReleasePlan ProposeRelease(ReleaseRule rule, ScenarioOneCoverage.Result coverage, IReadOnlyList<Stem> currentStems,
        IReadOnlyCollection<string> confirmedCropTreeIds, IReadOnlyCollection<string> sampleCropTreeIds, IReadOnlyCollection<string> keepStanding = null)
    {
        var plan = new ReleasePlan { Rule = rule };
        if (coverage == null || !coverage.Valid) return PlanFail(plan, "There is no valid coverage result.");
        plan.CalibrationVersion = coverage.Calibration.Version;
        List<Stem> sorted = ScenarioOneCoverage.SortedStems(currentStems, out string duplicate);
        if (duplicate != null) return PlanFail(plan, "Tree id " + duplicate + " appears more than once.");
        var world = new List<Stem>();
        var byId = new Dictionary<string, Stem>(StringComparer.Ordinal);
        foreach (Stem s in sorted) if (s.Living) { world.Add(s); byId[s.Id] = s; }

        // Area membership and basal area in the current state. Coverage must describe this state: a target stem that is gone means it is stale.
        var areaOf = new Dictionary<string, ScenarioOneCoverage.Area>(StringComparer.Ordinal);
        var areaBa = new Dictionary<string, double>(StringComparer.Ordinal);
        foreach (ScenarioOneCoverage.Area area in coverage.Areas)
        {
            if (area.State != ScenarioOneCoverage.State.Covered) continue;
            double ba = 0;
            foreach (string id in area.StemIds)
            {
                if (!byId.TryGetValue(id, out Stem s)) return PlanFail(plan, "Coverage is stale: " + id + " in " + area.Id + " is no longer a living tree.");
                ba += ScenarioOneCoverage.BasalAreaM2(s.DbhCm);
                areaOf[id] = area;
            }
            areaBa[area.Id] = ba;
        }

        var protectedIds = new HashSet<string>(StringComparer.Ordinal);
        if (sampleCropTreeIds != null) foreach (string id in sampleCropTreeIds) protectedIds.Add(id);
        foreach (Stem s in world) if (s.Crop) protectedIds.Add(s.Id);
        var confirmed = new List<string>();
        if (confirmedCropTreeIds != null) foreach (string id in confirmedCropTreeIds) if (id != null && !confirmed.Contains(id)) confirmed.Add(id);
        confirmed.Sort(StringComparer.Ordinal);
        foreach (string id in confirmed) protectedIds.Add(id);
        if (keepStanding != null) foreach (string id in keepStanding) if (id != null) protectedIds.Add(id);
        var sampleCrops = new HashSet<string>(sampleCropTreeIds ?? new string[0], StringComparer.Ordinal);

        var removed = new Dictionary<string, Proposal>(StringComparer.Ordinal);
        var charged = new Dictionary<string, double>(StringComparer.Ordinal);
        foreach (string areaId in areaBa.Keys) charged[areaId] = 0;
        var served = new List<KeyValuePair<CropRelease, List<Neighbour>>>();
        foreach (string id in confirmed)
        {
            var crop = new CropRelease { Id = id };
            plan.CropTrees.Add(crop);
            Stem tree;
            bool found = byId.TryGetValue(id, out tree);
            bool stump = false;
            if (!found) foreach (Stem s in sorted) if (s.Id == id) stump = true;
            if (!found) { crop.Status = stump ? CropStatus.NotLiving : CropStatus.NotFound; continue; }
            if (sampleCrops.Contains(id)) { crop.Status = CropStatus.SampleCropTree; continue; }
            if (!tree.IsSitka) { crop.Status = CropStatus.NotSitka; continue; }
            if (!areaOf.TryGetValue(id, out ScenarioOneCoverage.Area home)) { crop.Status = CropStatus.NotInCoveredArea; continue; }
            crop.AreaId = home.Id;

            List<Neighbour> neighbours = Neighbours(tree, world);
            crop.CompetitorsInReach = neighbours.Count;
            served.Add(new KeyValuePair<CropRelease, List<Neighbour>>(crop, neighbours));
            bool limited = false;
            double got = SharesOf(neighbours, removed.ContainsKey);
            foreach (Neighbour n in neighbours)
            {
                if (got >= rule.TargetShare - ShareTolerance) break;
                if (removed.ContainsKey(n.Id) || protectedIds.Contains(n.Id)) continue;
                if (!areaOf.TryGetValue(n.Id, out ScenarioOneCoverage.Area area)) continue;
                Stem competitor = byId[n.Id];
                if (!competitor.IsSitka || competitor.FellMark) continue;
                double ba = ScenarioOneCoverage.BasalAreaM2(competitor.DbhCm);
                if ((charged[area.Id] + ba) / areaBa[area.Id] > rule.BasalCeilingShare + ShareTolerance) { limited = true; continue; }
                removed[n.Id] = new Proposal { TreeId = n.Id, AreaId = area.Id, ForCropTreeId = id, ShareOfCropTree = n.Share, BasalAreaM2 = ba };
                charged[area.Id] += ba;
                got += n.Share;
            }
            crop.Status = limited ? CropStatus.LimitedByCeiling : neighbours.Count == 0 ? CropStatus.NoCompetitorsInReach : CropStatus.ShortOfTarget;
        }
        // A Crop Tree's final release includes removals proposed later for its neighbours.
        foreach (KeyValuePair<CropRelease, List<Neighbour>> pair in served)
        {
            pair.Key.PlannedShare = SharesOf(pair.Value, removed.ContainsKey);
            if (pair.Key.PlannedShare >= rule.TargetShare - ShareTolerance) pair.Key.Status = CropStatus.Released;
        }

        var ids = new List<string>(removed.Keys);
        ids.Sort(StringComparer.Ordinal);
        foreach (string id in ids) plan.Proposals.Add(removed[id]);
        plan.Safeguard = EvaluateSafeguard(coverage, currentStems, ids, rule.BasalCeilingShare);
        plan.Valid = true;
        return plan;
    }

    // Re-checks any set of proposed removals against the per-area basal-area ceiling (for revalidation before approval, MFM-07). Every
    // covered area is reported; a removal that is not a living target stem of a covered area is listed as not allowed.
    public static SafeguardReport EvaluateSafeguard(ScenarioOneCoverage.Result coverage, IReadOnlyList<Stem> currentStems,
        IReadOnlyCollection<string> proposedRemovalIds, double ceilingShare)
    {
        var report = new SafeguardReport();
        if (coverage == null || !coverage.Valid)
        {
            // Fail closed: an unverifiable proposal is never reported as within the ceiling.
            report.InvalidInput = true;
            report.InvalidReason = coverage == null ? "Coverage is missing." : "Coverage is invalid: " + coverage.Problem;
            return report;
        }
        double ceiling = Math.Min(ceilingShare, ScenarioOneRackBay.MaxRemovedBasalShare);
        List<Stem> sorted = ScenarioOneCoverage.SortedStems(currentStems, out _);
        var living = new Dictionary<string, Stem>(StringComparer.Ordinal);
        foreach (Stem s in sorted) if (s.Living) living[s.Id] = s;
        var proposed = new SortedSet<string>(proposedRemovalIds ?? new string[0], StringComparer.Ordinal);
        var accounted = new HashSet<string>(StringComparer.Ordinal);
        foreach (ScenarioOneCoverage.Area area in coverage.Areas)
        {
            if (area.State != ScenarioOneCoverage.State.Covered) continue;
            var entry = new AreaSafeguard { AreaId = area.Id, Ceiling = ceiling };
            foreach (string id in area.StemIds)
            {
                if (!living.TryGetValue(id, out Stem s)) continue;
                double ba = ScenarioOneCoverage.BasalAreaM2(s.DbhCm);
                entry.BasalAreaM2 += ba;
                if (proposed.Contains(id)) { entry.ProposedBasalAreaM2 += ba; accounted.Add(id); }
            }
            entry.Share = entry.BasalAreaM2 > 0 ? entry.ProposedBasalAreaM2 / entry.BasalAreaM2 : 0.0;
            report.Areas.Add(entry);
        }
        foreach (string id in proposed) if (!accounted.Contains(id)) report.OutsideCoveredAreas.Add(id);
        return report;
    }

    private static ReleasePlan PlanFail(ReleasePlan plan, string problem)
    {
        plan.Valid = false;
        plan.Problem = problem;
        return plan;
    }

    // ------------------------------------------------------------------------------------------------------------------ shared helpers

    private readonly struct Neighbour
    {
        public readonly string Id;
        public readonly double Share;
        public Neighbour(string id, double share) { Id = id; Share = share; }
    }

    // A tree's living neighbours inside the Hegyi reach, with each one's Share of its current competition (authoritative CompetitionShare),
    // strongest first, ties by id.
    private static List<Neighbour> Neighbours(Stem target, List<Stem> world)
    {
        float reach = ForestEcologyController.HegyiCutoffMeters;
        var samples = new List<CompetitionSample>();
        foreach (Stem other in world)
        {
            if (other.Id == target.Id) continue;
            float dx = other.X - target.X, dz = other.Z - target.Z;
            if (Math.Abs(dx) > reach || Math.Abs(dz) > reach) continue;
            samples.Add(CompetitionShare.SampleFrom(other.Id, other.X, other.Z, target.X, target.Z, other.DbhCm, true, false, false));
        }
        CompetitionShareSet set = CompetitionShare.Measure(target.DbhCm, samples);
        var list = new List<Neighbour>(set.Neighbours.Count);
        foreach (CompetitionContribution c in set.Neighbours)
            list.Add(new Neighbour(c.Id, set.CurrentTotal > 0 ? c.Contribution / set.CurrentTotal : 0.0));
        list.Sort((a, b) =>
        {
            int c = b.Share.CompareTo(a.Share);
            return c != 0 ? c : string.CompareOrdinal(a.Id, b.Id);
        });
        return list;
    }

    private static double SharesOf(List<Neighbour> neighbours, Func<string, bool> taken)
    {
        double sum = 0;
        foreach (Neighbour n in neighbours) if (taken(n.Id)) sum += n.Share;
        return sum;
    }

    private static bool Masked(IReadOnlyList<ScenarioOneCoverage.Mask> masks, float x, float z)
    {
        if (masks != null) foreach (ScenarioOneCoverage.Mask m in masks) if (m.Contains(x, z)) return true;
        return false;
    }
}
