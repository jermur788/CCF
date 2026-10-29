using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Cov = ScenarioOneCoverage;
using Pres = ScenarioOnePrescription;
using Cand = ScenarioOneCropTreeCandidates;
using Cal = ScenarioOnePrescriptionCalibration;
using App = ScenarioOnePrescriptionApproval;

// Plain .NET checks of MFM-07, the proposal state and prescription-approval boundary (ScenarioOnePrescriptionApproval). Decision rules only:
// no Unity, no save, no scene. The fixture is the MFM-03 frozen Enlarged80 Rack V2 stand used by the MFM-04 core check, with the 17 candidate
// Crop Trees confirmed, so the boundary is exercised on real proposals (84 removals) rather than a toy.
//
// What this proves, by test group:
//   A. a proposal is plain data and never a mark, order or saved state (behaviour + static guards)
//   B. proposals are deterministic and order-independent
//   C. manual player state wins and the conflict is surfaced, not dropped silently
//   D. the mandatory MFM-06 condition: every prescription / coverage-unit / Crop Tree link fails closed with a bounded reason
//   E. an approval is all-or-nothing: Build returns every order or none
//   F. the Work Plan code paths cannot reach proposals (static)
public static class ApprovalBoundaryOfflineCheck
{
    private static int checks, failures;
    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

    private static void Check(bool ok, string name, string detail = "")
    {
        checks++;
        Console.WriteLine((ok ? "PASS " : "FAIL ") + name + (detail.Length > 0 ? " [" + detail + "]" : ""));
        if (!ok) failures++;
    }

    private static readonly float[] RackZ = { -35.2711754f, -16.4217644f, 0.2382353f, 15.7935295f, 35.2323532f };
    private const float EntranceX = -35.4f, FarX = 40f, Half = 40f;
    private static readonly string[] SampleCrops = { "P0810", "P1206", "P1214" };
    private const double LiveInitialReleaseBaShare = 0.17898431420326233;

    private sealed class Fixture
    {
        public ScenarioOneRackSampleGeometry.Result Geometry;
        public List<Cov.Stem> Stems;
        public List<ScenarioOneRackBay.Order> Orders;
        public List<Cov.Mask> Masks;
        public Cov.Result Coverage;
        public List<string> Confirmed;
        public List<string> CoveredUnits;
    }

    public static int Main(string[] args)
    {
        if (args.Length < 3) { Console.Error.WriteLine("usage: <repo-root> <fixture-stand.csv> <fixture-infrastructure.json>"); return 2; }
        string root = args[0];
        repoRoot = root;
        Fixture fx = Load(root, args[1], args[2]);
        if (fx == null) { Console.WriteLine("MFM07_APPROVAL_BOUNDARY_FAIL fixture"); return 1; }

        ProposalIsPlainData(fx);
        Determinism(fx);
        ManualStateWins(fx);
        LinkValidationFailsClosed(fx);
        AllOrNothing(fx);
        WorkPlanBoundaryStatic(root);

        Console.WriteLine((failures == 0 ? "MFM07_APPROVAL_BOUNDARY_PASS" : "MFM07_APPROVAL_BOUNDARY_FAIL") + " checks=" + checks + " failed=" + failures);
        return failures == 0 ? 0 : 1;
    }

    // ----------------------------------------------------------------------------------------------------------------------- fixture

    private static Fixture Load(string root, string csv, string infraPath)
    {
        var raw = new List<(string Id, float X, float Z, float Dbh)>();
        foreach (string line in File.ReadAllLines(csv).Skip(1))
        {
            if (string.IsNullOrWhiteSpace(line)) continue;
            string[] c = line.Split(',');
            raw.Add((c[0], float.Parse(c[1], Inv), float.Parse(c[2], Inv), float.Parse(c[3], Inv)));
        }
        Check(raw.Count == 1344, "fixture: 1344 starting stems", raw.Count.ToString());
        var racks = new List<ScenarioOneRackSampleGeometry.RackInput>();
        for (int i = 0; i < RackZ.Length; i++)
            racks.Add(new ScenarioOneRackSampleGeometry.RackInput(i + 1, new RackPoint(EntranceX, RackZ[i]), new RackPoint(FarX, RackZ[i]), RackGeometry.DefaultWidthM));
        var geometry = ScenarioOneRackSampleGeometry.Derive(racks, Half);
        Check(geometry.Valid && geometry.TeachingRack.Id == 3, "fixture: geometry valid, teaching rack 3", geometry.Problem);
        if (!geometry.Valid) return null;
        var claimed = new HashSet<string>(StringComparer.Ordinal);
        foreach (var rack in racks)
        {
            var input = raw.Select(s => new RackTree(s.Id, s.X, s.Z, true, false, claimed.Contains(s.Id))).ToList();
            foreach (RackTree t in RackGeometry.Select(input, rack.Entrance, rack.Far, rack.WidthM).Trees) claimed.Add(t.Id);
        }
        var stems = raw.Select(s => new Cov.Stem(s.Id, s.X, s.Z, s.Dbh, !claimed.Contains(s.Id), Cov.SitkaSpeciesId, SampleCrops.Contains(s.Id))).ToList();
        var orders = claimed.OrderBy(id => id, StringComparer.Ordinal)
            .Select(id => new ScenarioOneRackBay.Order(id, false, true, ScenarioOneRackBay.Disposition.Sold, 0)).ToList();
        var fx = new Fixture { Geometry = geometry, Stems = stems, Orders = orders, Masks = Masks(root, infraPath) };
        var input0 = new Cov.Input { Geometry = geometry, Stems = stems, Orders = orders, Masks = fx.Masks };
        fx.Coverage = Cov.Classify(input0, Cal.V1);
        Check(fx.Coverage.Valid, "fixture: coverage valid", fx.Coverage.Problem);
        if (!fx.Coverage.Valid) return null;
        fx.CoveredUnits = fx.Coverage.Areas.Where(a => !a.IsSource && a.State == Cov.State.Covered).Select(a => a.Id).ToList();
        Cand.Result candidates = Cand.Suggest(fx.Coverage, stems, SampleCrops, null, null, Cal.V1);
        fx.Confirmed = candidates.Candidates.Select(c => c.TreeId).ToList();
        fx.Stems = stems.Select(s => fx.Confirmed.Contains(s.Id) ? With(s, crop: true) : s).ToList();
        Check(fx.Confirmed.Count == 17 && fx.CoveredUnits.Count == 13, "fixture: 17 confirmed Crop Trees, 13 covered units", fx.Confirmed.Count + "/" + fx.CoveredUnits.Count);
        return fx;
    }

    private static List<Cov.Mask> Masks(string root, string infraPath)
    {
        string text = File.ReadAllText(Path.Combine(root, "Assets", "ForestPrototype", "ScenarioOne", "ScenarioOneHeadlandGeometry.cs"));
        double C(string name)
        {
            var m = System.Text.RegularExpressions.Regex.Match(text, "const float " + name + " = (-?[0-9.]+)f;");
            if (!m.Success) throw new InvalidOperationException("headland constant " + name);
            return double.Parse(m.Groups[1].Value, Inv);
        }
        using System.Text.Json.JsonDocument doc = System.Text.Json.JsonDocument.Parse(File.ReadAllText(infraPath));
        System.Text.Json.JsonElement infra = doc.RootElement;
        double rPath = infra.GetProperty("pathCorridorRadiusM").GetDouble(), rSite = infra.GetProperty("clearingRadiusM").GetDouble();
        var masks = new List<Cov.Mask>();
        foreach (var p in infra.GetProperty("scenePlanks").EnumerateArray())
            masks.Add(Cov.Mask.Circle(Cov.MaskKind.Track, p.GetProperty("x").GetDouble(), p.GetProperty("z").GetDouble(), rPath));
        foreach (var p in infra.GetProperty("approachExtension").EnumerateArray())
        {
            double z = p.GetProperty("z").GetDouble();
            if (z >= -Half - rPath && z <= Half + rPath) masks.Add(Cov.Mask.Circle(Cov.MaskKind.Track, p.GetProperty("x").GetDouble(), z, rPath));
        }
        var cl = infra.GetProperty("clearing");
        masks.Add(Cov.Mask.Circle(Cov.MaskKind.Clearing, cl.GetProperty("x").GetDouble(), cl.GetProperty("z").GetDouble(), cl.GetProperty("visualRadiusM").GetDouble()));
        var pl = infra.GetProperty("player");
        masks.Add(Cov.Mask.Circle(Cov.MaskKind.Site, pl.GetProperty("x").GetDouble(), pl.GetProperty("z").GetDouble(), rSite));
        foreach (var b in infra.GetProperty("buildables").EnumerateArray())
            masks.Add(Cov.Mask.Circle(Cov.MaskKind.Site, b.GetProperty("x").GetDouble(), b.GetProperty("z").GetDouble(), rSite));
        double strip = C("Width") * 0.5 + C("StemGuard"), pad = C("PadRadius") + C("StemGuard");
        double hx = C("HeadlandX"), fz = C("FeederZ"), px = C("PathX");
        masks.Add(new Cov.Mask(Cov.MaskKind.Headland, hx, C("SouthZ"), hx, C("NorthZ"), strip));
        masks.Add(new Cov.Mask(Cov.MaskKind.Headland, hx, fz, px, fz, strip));
        masks.Add(Cov.Mask.Circle(Cov.MaskKind.Headland, hx, fz, pad));
        masks.Add(Cov.Mask.Circle(Cov.MaskKind.Headland, px, fz, pad));
        return masks;
    }

    private static Cov.Stem With(Cov.Stem s, bool? living = null, string species = null, bool? crop = null, bool? fellMark = null)
        => new Cov.Stem(s.Id, s.X, s.Z, s.DbhCm, living ?? s.Living, species ?? s.SpeciesId, crop ?? s.Crop, fellMark ?? s.FellMark);

    // A context for prescription 7 that accepted every covered unit, with the lesson's rule at the live initial release.
    private static App.Context Context(Fixture fx, IEnumerable<Cov.Stem> stems = null, IEnumerable<string> accepted = null, IEnumerable<string> manual = null,
        IEnumerable<string> openOrders = null, IEnumerable<string> openFelling = null)
    {
        var choices = new Dictionary<string, int>(StringComparer.Ordinal);
        foreach (string id in accepted ?? fx.CoveredUnits) choices[id] = PrescriptionCoverageChoice.AcceptedCovered;
        foreach (string id in manual ?? new string[0]) choices[id] = PrescriptionCoverageChoice.LeftManual;
        var d = new Pres.Derivation
        {
            Status = Pres.Status.Ready, RuleVersion = Cal.RuleVersion, CalibrationVersion = Cal.V1.Version, TeachingRackId = fx.Geometry.TeachingRack.Id,
            AnchorX = fx.Geometry.Anchor.X, AnchorZ = fx.Geometry.Anchor.Z, Rule = Pres.ReleaseRule.Create(0.30, LiveInitialReleaseBaShare)
        };
        d.SampleCropTreeIds.AddRange(SampleCrops.OrderBy(x => x, StringComparer.Ordinal));
        return new App.Context
        {
            PrescriptionId = 7, RuleVersion = Cal.RuleVersion, CalibrationVersion = Cal.V1.Version, ApprovalState = PrescriptionApprovalState.Accepted,
            SourceRackId = d.TeachingRackId, SourceAnchorX = d.AnchorX, SourceAnchorZ = d.AnchorZ,
            SampleCropTreeIds = d.SampleCropTreeIds.ToList(), SampleRemovalIds = new string[0], CoverageChoices = choices, Derivation = d,
            Coverage = fx.Coverage, Stems = (stems ?? fx.Stems).ToList(),
            OpenOrderTreeIds = (openOrders ?? new string[0]).ToList(), OpenFellingTreeIds = (openFelling ?? openOrders ?? new string[0]).ToList()
        };
    }

    // ------------------------------------------------------------------------------------------------------------- A. plain data

    private static void ProposalIsPlainData(Fixture fx)
    {
        App.Context ctx = Context(fx);
        string before = Snapshot(ctx);
        App.ProposalSet set = App.Propose(ctx);
        Check(set.Valid && set.Links.Count == 84, "A1 proposals: 84 removals around the 17 confirmed Crop Trees (the MFM-04 release)", set.Links.Count + " " + set.Message);
        Check(Snapshot(ctx) == before, "A2 proposing changes nothing: stems, marks, choices, open orders and approval state are untouched");
        Check(ctx.Stems.All(s => !s.FellMark) && set.Links.All(l => ctx.Stems.First(s => s.Id == l.TargetTreeId).FellMark == false),
            "A3 a proposed tree carries no Fell mark: proposing never marks");
        Check(set.Links.All(l => l.PrescriptionId == 7 && l.ReasonCode == PrescriptionReasonCode.CompetitorHighShareAroundConfirmedCropTree
              && l.CropTreeId.Length > 0 && l.CoverageUnitId.Length > 0), "A4 every proposal names prescription, coverage unit, Crop Tree and a closed reason");
        Check(set.Safeguard.AllWithin && !set.Safeguard.InvalidInput, "A5 the proposal is within every unit's basal-area ceiling");
        Check(Enum.GetValues(typeof(Pres.ProposalReason)).Length == 1 && App.ReasonCodeOf(Pres.ProposalReason.CompetitorHighShareAroundConfirmedCropTree)
              == PrescriptionReasonCode.CompetitorHighShareAroundConfirmedCropTree, "A6 reason vocabulary: one closed reason, code = enum + 1, never 0 (an ordinary order)");

        // Approval is a separate step: Build decides, it does not approve; an approved prescription is not offered again.
        App.ApprovalPlan plan = App.Build(ctx);
        Check(plan.Ok && plan.Orders.Count == 84 && ctx.ApprovalState == PrescriptionApprovalState.Accepted, "A7 Build returns the orders to write but leaves the stored state Accepted (the writer flips it)");
        App.Context approved = Context(fx);
        approved.ApprovalState = PrescriptionApprovalState.Approved;
        App.ApprovalPlan again = App.Build(approved);
        Check(!again.Ok && again.Refusal == App.Refusal.NotAccepted && again.Orders.Count == 0, "A8 an already approved prescription proposes and approves nothing again", again.Message);
        Check(!App.Propose(approved).Valid, "A9 an approved prescription has no live proposal state");

        // Static: the pure boundary cannot write a mark, order or save.
        string code = StripComments(File.ReadAllText(Path.Combine(Root(), "Assets", "ForestPrototype", "ScenarioOne", "ScenarioOnePrescriptionApproval.cs")));
        foreach (string token in new[] { "UnityEngine", "TreeMarkType", "ForestTreeMarking", "ScenarioOneWorkOrder", "ScenarioOneManager", "ForestTree ", "SaveData",
                     "PlayerPrefs", "Random", "DateTime", "File.", "static readonly List", "static readonly Dictionary", "static readonly HashSet" })
            Check(!code.Contains(token), "A10 static: ScenarioOnePrescriptionApproval does not use " + token);
        string manager = StripComments(File.ReadAllText(Path.Combine(Root(), "Assets", "ForestPrototype", "ScenarioOne", "ScenarioOneManager.PrescriptionApproval.cs")));
        foreach (string token in new[] { "TreeMarkType", "ForestTreeMarkingManager", "MarkForFell", "SetMarked", "ScenarioWorkStatus.Approved", "ApprovePendingWork",
                     "AddMarkedTreesToWorkPlan", "SetWorkPlanOpen", "OpenWorkPlan", "AdvanceYear", "ResolveOrder", "cashCents -=" })
            Check(!manager.Contains(token), "A11 static: the prescription writer never uses " + token);
    }

    // Everything a proposal could have changed, as one string.
    private static string Snapshot(App.Context ctx)
        => string.Join("|", ctx.Stems.Select(s => s.Id + s.Living + s.Crop + s.FellMark)) + "#" + string.Join(",", ctx.CoverageChoices.OrderBy(k => k.Key).Select(k => k.Key + k.Value))
           + "#" + string.Join(",", ctx.OpenOrderTreeIds) + "#" + ctx.ApprovalState;

    // ---------------------------------------------------------------------------------------------------------------- B. determinism

    private static void Determinism(Fixture fx)
    {
        string a = App.Propose(Context(fx)).Signature(), b = App.Propose(Context(fx)).Signature();
        Check(a == b && a.StartsWith("ok "), "B1 the same state gives the same proposal signature");
        var reversed = fx.Stems.AsEnumerable().Reverse();
        Check(App.Propose(Context(fx, stems: reversed)).Signature() == a, "B2 the order stems are supplied in does not change the proposal");
        Check(App.Propose(Context(fx, accepted: fx.CoveredUnits.AsEnumerable().Reverse())).Signature() == a, "B3 the order coverage choices are supplied in does not change the proposal");
        App.ProposalSet set = App.Propose(Context(fx));
        var ids = set.Links.Select(l => l.TargetTreeId).ToList();
        Check(ids.SequenceEqual(ids.OrderBy(x => x, StringComparer.Ordinal)) && ids.Distinct().Count() == ids.Count, "B4 proposals are unique and ordered by tree id");
        Check(set.Links.Select(l => l.Key).Distinct().Count() == set.Links.Count, "B5 each proposal has a distinct deterministic identity key");
        Check(App.Build(Context(fx)).Orders.Select(o => o.Key).SequenceEqual(set.Links.Select(l => l.Key)), "B6 the approval writes exactly the previewed proposals in the previewed order");
    }

    // ------------------------------------------------------------------------------------------------------- C. manual state wins

    private static void ManualStateWins(Fixture fx)
    {
        App.ProposalSet baseline = App.Propose(Context(fx));
        string t1 = baseline.Links[0].TargetTreeId, t2 = baseline.Links[1].TargetTreeId, t3 = baseline.Links[2].TargetTreeId;

        // A manual Fell mark holds its tree and is surfaced.
        var marked = fx.Stems.Select(s => s.Id == t1 ? With(s, fellMark: true) : s).ToList();
        App.ProposalSet withMark = App.Propose(Context(fx, stems: marked));
        Check(withMark.Valid && withMark.Links.All(l => l.TargetTreeId != t1), "C1 a manually marked tree is never proposed");
        Check(withMark.Conflicts.Any(c => c.TreeId == t1 && c.Kind == App.ConflictKind.ManualFellMark), "C2 the held-back marked tree is surfaced as a conflict, not dropped silently");
        Check(marked.First(s => s.Id == t1).FellMark, "C3 the player's mark is left exactly as it was");

        // A manual (or any other) open order holds its tree.
        App.ProposalSet withOrder = App.Propose(Context(fx, openOrders: new[] { t2 }, openFelling: new[] { t2 }));
        Check(withOrder.Valid && withOrder.Links.All(l => l.TargetTreeId != t2) && withOrder.Conflicts.Any(c => c.TreeId == t2 && c.Kind == App.ConflictKind.OpenWorkOrder),
            "C4 a tree with an open order is never proposed and is surfaced as a conflict");
        App.LinkCheck stale = App.ValidateLink(Context(fx, openOrders: new[] { t2 }), baseline.Links.First(l => l.TargetTreeId == t2));
        Check(!stale.Ok && stale.Refusal == App.Refusal.TargetHasManualState, "C5 a previously previewed order for a tree the player has since planned is refused", stale.Message);

        // A player Crop Tree designation wins over a proposed removal.
        var designated = fx.Stems.Select(s => s.Id == t3 ? With(s, crop: true) : s).ToList();
        Check(App.Propose(Context(fx, stems: designated)).Links.All(l => l.TargetTreeId != t3), "C6 a tree the player designated as a Crop Tree is never proposed for removal");

        // A unit the player left to manual management is not served, and a stale order for it is refused.
        string unit = baseline.Links[0].CoverageUnitId;
        App.Context left = Context(fx, accepted: fx.CoveredUnits.Where(u => u != unit), manual: new[] { unit });
        App.ProposalSet withLeft = App.Propose(left);
        Check(withLeft.Valid && withLeft.Links.All(l => l.CoverageUnitId != unit), "C7 a unit left for manual management receives no proposal");
        App.LinkCheck staleUnit = App.ValidateLink(left, baseline.Links.First(l => l.CoverageUnitId == unit));
        Check(!staleUnit.Ok && staleUnit.Refusal == App.Refusal.UnitNotAccepted, "C8 an order previewed for a unit since left manual is refused", staleUnit.Message);

        // No choice means no proposal: acceptance is deliberate, never implied.
        App.ProposalSet none = App.Propose(Context(fx, accepted: new string[0]));
        Check(none.Valid && none.Links.Count == 0 && !App.Build(Context(fx, accepted: new string[0])).Ok
              && App.Build(Context(fx, accepted: new string[0])).Refusal == App.Refusal.NothingToApprove, "C9 with no accepted unit nothing is proposed and approval has nothing to approve");

        // The player's own pending felling counts against the unit ceiling together with the proposal.
        var fellingIn = fx.Coverage.Find(baseline.Links[0].CoverageUnitId).StemIds.Where(id => !SampleCrops.Contains(id) && !fx.Confirmed.Contains(id)).ToList();
        App.Context heavy = Context(fx, openOrders: fellingIn, openFelling: fellingIn);
        App.ApprovalPlan overCeiling = App.Build(heavy);
        Check(!overCeiling.Ok && overCeiling.Refusal == App.Refusal.SafeguardExceeded && overCeiling.Orders.Count == 0,
            "C10 proposal plus the player's own planned felling above a unit's ceiling is refused whole", overCeiling.Message);

        // Quantitative rules stay Sitka-only: a broadleaf stem is never proposed.
        var mixed = fx.Stems.Select(s => s.Id == t1 || s.Id == t2 ? With(s, species: "beech") : s).ToList();
        Check(App.Propose(Context(fx, stems: mixed)).Links.All(l => l.TargetTreeId != t1 && l.TargetTreeId != t2), "C11 a broadleaf tree is never proposed (no quantitative extrapolation)");
    }

    // ----------------------------------------------------------------------------------- D. the mandatory MFM-06 link validation

    private static void LinkValidationFailsClosed(Fixture fx)
    {
        App.Context ctx = Context(fx);
        App.ProposalSet set = App.Propose(ctx);
        App.OrderLink good = set.Links[0];
        Check(App.ValidateLink(ctx, good).Ok && App.ValidateLinks(ctx, set.Links, null).Ok, "D0 every order the prescription itself proposes validates");

        void Expect(string name, App.LinkCheck got, App.Refusal want)
        {
            Check(!got.Ok && got.Refusal == want && got.Message.Length > 0 && got.Message.Length <= 220, name, got.Refusal + " " + got.Message);
        }
        App.OrderLink Link(App.OrderLink l, int? pid = null, string target = null, string unit = null, string crop = null, int? reason = null)
            => new App.OrderLink(pid ?? l.PrescriptionId, target ?? l.TargetTreeId, unit ?? l.CoverageUnitId, crop ?? l.CropTreeId, reason ?? l.ReasonCode);

        // ---- the referenced prescription ----
        Expect("D1 a missing prescription is refused", App.ValidateLink(null, good), App.Refusal.NoPrescription);
        Expect("D2 an order naming a different prescription is refused", App.ValidateLink(ctx, Link(good, pid: 8)), App.Refusal.WrongPrescription);
        Expect("D3 an order with no prescription id is refused", App.ValidateLink(ctx, Link(good, pid: 0)), App.Refusal.WrongPrescription);
        App.Context oldRule = Context(fx); oldRule.RuleVersion = "MFM-V0-RULE-0";
        Expect("D4 a prescription made under another rule version is refused", App.ValidateLink(oldRule, good), App.Refusal.RuleVersionMismatch);
        App.Context oldCal = Context(fx); oldCal.CalibrationVersion = "MFM03-CAL-0";
        Expect("D5 a prescription made under another calibration is refused", App.ValidateLink(oldCal, good), App.Refusal.CalibrationMismatch);
        App.Context done = Context(fx); done.ApprovalState = PrescriptionApprovalState.Approved;
        Expect("D6 a prescription that is not waiting for approval is refused", App.ValidateLink(done, good), App.Refusal.NotAccepted);
        App.Context moved = Context(fx); moved.SourceRackId = 2;
        Expect("D7 a prescription whose source rack is no longer the sample is refused", App.ValidateLink(moved, good), App.Refusal.SampleChanged);
        App.Context cropsChanged = Context(fx); cropsChanged.SampleCropTreeIds = new[] { "P0810" };
        Expect("D8 a prescription whose sample Crop Trees changed is refused", App.ValidateLink(cropsChanged, good), App.Refusal.SampleChanged);
        App.Context removalsChanged = Context(fx); removalsChanged.SampleRemovalIds = new[] { "P-gone" };
        Expect("D9 a prescription whose sample removals changed is refused", App.ValidateLink(removalsChanged, good), App.Refusal.SampleChanged);
        App.Context anchorMoved = Context(fx); anchorMoved.SourceAnchorX += 1f;
        Expect("D10 a prescription whose sample anchor moved is refused", App.ValidateLink(anchorMoved, good), App.Refusal.SampleChanged);
        App.Context notDerived = Context(fx); notDerived.Derivation = new Pres.Derivation { Status = Pres.Status.SampleHasOpenWork, Problem = "x" };
        Expect("D11 a sample that can no longer be derived is refused", App.ValidateLink(notDerived, good), App.Refusal.SampleNotDerived);
        App.Context badCov = Context(fx); badCov.Coverage = new Cov.Result { Valid = false, Problem = "test invalid" };
        Expect("D12 unreadable coverage is refused", App.ValidateLink(badCov, good), App.Refusal.CoverageInvalid);

        // ---- the coverage unit ----
        Expect("D13 an unknown coverage unit is refused", App.ValidateLink(ctx, Link(good, unit: "Z9Z")), App.Refusal.UnitUnknown);
        Expect("D14 an empty coverage unit is refused", App.ValidateLink(ctx, Link(good, unit: "")), App.Refusal.UnitUnknown);
        Cov.Area source = fx.Coverage.Areas.First(a => a.IsSource);
        Expect("D15 the sample itself is never a coverage unit for generated work", App.ValidateLink(ctx, Link(good, unit: source.Id)), App.Refusal.UnitIsSource);
        Cov.Area notCovered = fx.Coverage.Areas.First(a => !a.IsSource && a.State != Cov.State.Covered);
        Expect("D16 a unit that is not a covered area is refused", App.ValidateLink(ctx, Link(good, unit: notCovered.Id)), App.Refusal.UnitNotCovered);
        string unaccepted = fx.CoveredUnits.First(u => u != good.CoverageUnitId);
        App.Context partial = Context(fx, accepted: fx.CoveredUnits.Where(u => u != unaccepted));
        Expect("D17 a covered unit the player did not accept is refused", App.ValidateLink(partial, Link(good, unit: unaccepted)), App.Refusal.UnitNotAccepted);
        string otherUnit = fx.CoveredUnits.First(u => u != good.CoverageUnitId);
        Expect("D18 a target tree that is not in the named coverage unit is refused", App.ValidateLink(ctx, Link(good, unit: otherUnit)), App.Refusal.TargetNotInUnit);
        var inUnit = fx.Coverage.Find(good.CoverageUnitId);
        Check(inUnit.StemIds.Contains(good.TargetTreeId), "D18b the unit that the good order names does contain its tree");
        Expect("D19 a target tree that is not alive is refused", App.ValidateLink(ctx, Link(good, target: fx.Stems.First(s => !s.Living).Id)), App.Refusal.TargetMissing);
        Expect("D20 an unknown target tree is refused", App.ValidateLink(ctx, Link(good, target: "P-none")), App.Refusal.TargetMissing);
        var broadleaf = fx.Stems.Select(s => s.Id == good.TargetTreeId ? With(s, species: "beech") : s).ToList();
        Expect("D21 a target tree that is no longer an eligible Sitka adult is refused", App.ValidateLink(Context(fx, stems: broadleaf), good), App.Refusal.TargetNotEligible);
        var targetCrop = fx.Stems.Select(s => s.Id == good.TargetTreeId ? With(s, crop: true) : s).ToList();
        Expect("D22 a target tree the player has since designated a Crop Tree is refused", App.ValidateLink(Context(fx, stems: targetCrop), good), App.Refusal.TargetNotEligible);
        var targetMarked = fx.Stems.Select(s => s.Id == good.TargetTreeId ? With(s, fellMark: true) : s).ToList();
        Expect("D23 a target tree the player has marked is refused", App.ValidateLink(Context(fx, stems: targetMarked), good), App.Refusal.TargetHasManualState);

        // ---- the Crop Tree ----
        Expect("D24 an order that names no Crop Tree is refused", App.ValidateLink(ctx, Link(good, crop: "")), App.Refusal.CropTreeMissing);
        string unconfirmed = fx.Stems.First(s => s.Living && !s.Crop && s.Id != good.TargetTreeId && s.IsSitka).Id;
        Expect("D25 a tree that is not a confirmed Crop Tree is refused", App.ValidateLink(ctx, Link(good, crop: unconfirmed)), App.Refusal.CropTreeNotConfirmed);
        Expect("D26 a sample Crop Tree cannot be the Crop Tree an order serves", App.ValidateLink(ctx, Link(good, crop: SampleCrops[0])), App.Refusal.CropTreeNotConfirmed);
        Expect("D27 an unknown Crop Tree is refused", App.ValidateLink(ctx, Link(good, crop: "P-none")), App.Refusal.CropTreeNotConfirmed);
        var cropDead = fx.Stems.Select(s => s.Id == good.CropTreeId ? With(s, living: false) : s).ToList();
        Expect("D28 a Crop Tree that has died or been felled is refused", App.ValidateLink(Context(fx, stems: cropDead), good), App.Refusal.CropTreeNotConfirmed);
        var cropUndesignated = fx.Stems.Select(s => s.Id == good.CropTreeId ? With(s, crop: false) : s).ToList();
        Expect("D29 a Crop Tree the player has since un-designated is refused", App.ValidateLink(Context(fx, stems: cropUndesignated), good), App.Refusal.CropTreeNotConfirmed);
        var cropBroadleaf = fx.Stems.Select(s => s.Id == good.CropTreeId ? With(s, species: "beech") : s).ToList();
        Expect("D30 a broadleaf Crop Tree gets no Sitka-calibrated order", App.ValidateLink(Context(fx, stems: cropBroadleaf), good), App.Refusal.CropTreeNotConfirmed);
        string cropHome = fx.Coverage.Areas.First(a => a.StemIds.Contains(good.CropTreeId)).Id;
        App.Context cropUnitLeft = Context(fx, accepted: fx.CoveredUnits.Where(u => u != cropHome));
        App.LinkCheck cropUnit = App.ValidateLink(cropUnitLeft, good);
        Check(!cropUnit.Ok && (cropUnit.Refusal == App.Refusal.CropTreeUnitNotAccepted || cropUnit.Refusal == App.Refusal.UnitNotAccepted),
            "D31 a Crop Tree whose own unit the player did not accept is refused", cropUnit.Refusal + " " + cropUnit.Message);
        string farCrop = set.Links.First(l => l.CropTreeId != good.CropTreeId).CropTreeId;
        Expect("D32 a confirmed Crop Tree that this tree does not compete with is refused (the prescription would not generate it)",
            App.ValidateLink(ctx, Link(good, crop: farCrop)), App.Refusal.NotInProposal);

        // ---- the reason, the whole list, and bounded text ----
        Expect("D33 reason 0 (no prescription reason) is refused", App.ValidateLink(ctx, Link(good, reason: 0)), App.Refusal.ReasonUnknown);
        Expect("D34 an unknown reason code is refused", App.ValidateLink(ctx, Link(good, reason: 2)), App.Refusal.ReasonUnknown);
        Expect("D35 a negative reason code is refused", App.ValidateLink(ctx, Link(good, reason: -1)), App.Refusal.ReasonUnknown);
        var withBad = set.Links.Take(5).Concat(new[] { Link(good, crop: "") }).ToList();
        Expect("D36 one invalid link among valid ones fails the whole list", App.ValidateLinks(ctx, withBad, null), App.Refusal.CropTreeMissing);
        Expect("D37 an empty list has nothing to write", App.ValidateLinks(ctx, new App.OrderLink[0], null), App.Refusal.NothingToApprove);
        string huge = new string('X', 5000);
        App.LinkCheck bounded = App.ValidateLink(ctx, Link(good, target: huge));
        Check(!bounded.Ok && bounded.Message.Length <= 220, "D38 a refusal reason stays bounded whatever the input", bounded.Message.Length.ToString());

        // ---- a link that was valid when previewed but is stale now ----
        var gone = fx.Stems.Select(s => s.Id == good.TargetTreeId ? With(s, living: false) : s).ToList();
        Check(App.ValidateLinks(ctx, set.Links, null).Ok && !App.ValidateLink(Context(fx, stems: gone), good).Ok,
            "D39 an order valid at preview is refused once its tree is gone (stale link)");
    }

    // -------------------------------------------------------------------------------------------------------------- E. all or nothing

    private static void AllOrNothing(Fixture fx)
    {
        App.ApprovalPlan ok = App.Build(Context(fx));
        Check(ok.Ok && ok.Orders.Count == 84 && App.ValidateLinks(Context(fx), ok.Orders, null).Ok, "E1 a valid prescription returns every order, all of which validate");

        // One stale Crop Tree: every order that serves it is invalid, but Build must not return the others alone.
        string crop = ok.Orders[0].CropTreeId;
        var cropGone = fx.Stems.Select(s => s.Id == crop ? With(s, crop: false) : s).ToList();
        App.ApprovalPlan afterChange = App.Build(Context(fx, stems: cropGone));
        // The prescription recomputes around the remaining Crop Trees: a smaller, still self-consistent set is allowed, never a stale link.
        Check(afterChange.Ok == (afterChange.Orders.Count > 0) && afterChange.Orders.All(o => o.CropTreeId != crop),
            "E2 after a Crop Tree is un-designated the approval is recomputed around the others and never carries a stale link", afterChange.Orders.Count.ToString());
        Check(afterChange.Ok && App.ValidateLinks(Context(fx, stems: cropGone), afterChange.Orders, null).Ok, "E2b every recomputed order validates");

        // A stale list is never half-accepted: the old list against the changed world fails as a whole.
        App.LinkCheck stale = App.ValidateLinks(Context(fx, stems: cropGone), ok.Orders, null);
        Check(!stale.Ok && stale.Refusal == App.Refusal.CropTreeNotConfirmed, "E3 the previously previewed list against the changed world fails as a whole", stale.Message);

        // Failure modes return no orders at all.
        var failures3 = new List<(string Name, App.ApprovalPlan Plan)>
        {
            ("rule mismatch", App.Build(WithRule(Context(fx), "OLD"))),
            ("source changed", App.Build(WithRack(Context(fx), 2))),
            ("coverage invalid", App.Build(WithCoverage(Context(fx), new Cov.Result { Valid = false, Problem = "p" }))),
            ("nothing accepted", App.Build(Context(fx, accepted: new string[0]))),
            ("duplicate stem ids", App.Build(Context(fx, stems: fx.Stems.Concat(new[] { fx.Stems[0] })))),
        };
        foreach (var f in failures3)
            Check(!f.Plan.Ok && f.Plan.Orders.Count == 0 && f.Plan.Message.Length > 0, "E4 a failed approval returns no order at all: " + f.Name, f.Plan.Refusal + " " + f.Plan.Message);

        // Idempotence of refusal: a refused Build does not change the context, so a retry sees the same world.
        App.Context stable = Context(fx);
        string before = Snapshot(stable);
        App.Build(WithRule(stable, "OLD"));
        Check(Snapshot(stable) == before, "E5 a refused approval leaves the inputs untouched");
    }

    private static App.Context WithRule(App.Context c, string rule) { c.RuleVersion = rule; return c; }
    private static App.Context WithRack(App.Context c, int rack) { c.SourceRackId = rack; return c; }
    private static App.Context WithCoverage(App.Context c, Cov.Result r) { c.Coverage = r; return c; }

    // ------------------------------------------------------------------------------------------------- F. the Work Plan cannot reach it

    private static void WorkPlanBoundaryStatic(string root)
    {
        string dir = Path.Combine(root, "Assets", "ForestPrototype", "ScenarioOne");
        string manager = StripComments(File.ReadAllText(Path.Combine(dir, "ScenarioOneManager.cs")));
        foreach (string method in new[] { "private void SetWorkPlanOpen(", "public void OpenWorkPlan(", "public int AddMarkedTreesToWorkPlan(", "private int AddMarkedTreesToWorkPlan(", "public bool ApprovePendingWork(" })
        {
            string body = Body(manager, method);
            Check(body != null, "F0 found " + method.Trim());
            if (body == null) continue;
            Check(!body.Contains("rescription") && !body.Contains("roposal") && !body.Contains("Approval."), "F1 " + method.Trim() + " cannot reach prescription or proposal state");
        }
        string marking = Path.Combine(root, "Assets", "ForestPrototype");
        foreach (string file in Directory.GetFiles(marking, "ForestTreeMarking*.cs", SearchOption.AllDirectories))
        {
            string code = StripComments(File.ReadAllText(file));
            Check(!code.Contains("rescription") && !code.Contains("ScenarioOnePrescriptionApproval"), "F2 " + Path.GetFileName(file) + " knows nothing of prescriptions");
        }
        // The writer validates before it creates anything, and approves nothing.
        string writer = StripComments(File.ReadAllText(Path.Combine(dir, "ScenarioOneManager.PrescriptionApproval.cs")));
        string approve = Body(writer, "public PrescriptionOperationResult ApprovePrescription(");
        Check(approve != null, "F3 found ApprovePrescription");
        if (approve != null)
        {
            int build = approve.IndexOf("Approval.Build(", StringComparison.Ordinal), pre = approve.IndexOf("Approval.ValidateLinks(", StringComparison.Ordinal);
            int create = approve.IndexOf("CreateFellingOrder(", StringComparison.Ordinal), add = approve.IndexOf("workOrders.Add(", StringComparison.Ordinal);
            Check(build >= 0 && pre > build && create > pre && add > create, "F4 order: Build, then the pre-write link validation, then order creation, then the first write");
            Check(!approve.Contains("ScenarioWorkStatus.Approved") && !approve.Contains("cashCents") && !approve.Contains("ApprovePendingWork") && !approve.Contains("ResolveOrder"),
                "F5 prescription approval does not approve, fund or execute work");
            Check(approve.Contains("catch (Exception") && approve.Contains("RemoveRange"), "F6 an unexpected failure rolls the writes back");
        }
        // Old saves and the schema: no new schema member and no version bump in this task.
        string saveData = File.ReadAllText(Path.Combine(root, "Assets", "ForestPrototype", "ForestSaveData.cs"));
        Check(saveData.Contains("public const int CurrentVersion = 23;") || saveData.Contains("CurrentVersion = 23"), "F7 the save schema is still v23 (no bump in MFM-07)");
    }

    // The text of a method from its signature to its matching closing brace; null if absent.
    private static string Body(string code, string signature)
    {
        int at = code.IndexOf(signature, StringComparison.Ordinal);
        if (at < 0) return null;
        int open = code.IndexOf('{', at);
        if (open < 0) return null;
        int depth = 0;
        for (int i = open; i < code.Length; i++)
        {
            if (code[i] == '{') depth++;
            else if (code[i] == '}' && --depth == 0) return code.Substring(at, i - at + 1);
        }
        return null;
    }

    private static string repoRoot = "";
    private static string Root() => repoRoot;

    private static string StripComments(string text)
        => string.Join("\n", text.Split('\n').Select(line => { int i = line.IndexOf("//", StringComparison.Ordinal); return i >= 0 ? line.Substring(0, i) : line; }));
}
