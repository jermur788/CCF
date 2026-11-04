using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using UnityEngine.UIElements;

public static class PreviousProposedOfflineCheck
{
    private static int checks, failures;
    private static void Check(bool ok, string name)
    {
        checks++; if (!ok) failures++;
        Console.WriteLine((ok ? "PASS " : "FAIL ") + name);
    }
    private static IEnumerable<VisualElement> All(VisualElement node)
        => new[] { node }.Concat(node.Children.SelectMany(All));
    private static string Text(VisualElement node) => string.Join("\n", All(node).Select(n => n.text));
    private static ScenarioOneWorkOrder Order(int id, ScenarioWorkStatus status, int year)
        => new ScenarioOneWorkOrder { workOrderId = id, type = ScenarioWorkType.FellTree, status = status, createdYear = year, targetTreeId = "tree-" + id };
    private static string Fingerprint(ScenarioOneManager m)
        => JsonSerializer.Serialize(new { m.WorkOrders, m.ManagementEvents, m.AnnualReports, m.HistoryYears, m.AnnualReviewSeen },
                                   new JsonSerializerOptions { IncludeFields = true });
    private static void CompleteYears(ScenarioOneManager m, params int[] years)
    {
        m.AnnualReports.Clear();
        foreach (int year in years) m.AnnualReports.Add(new ScenarioAnnualReport { year = year });
        m.HistoryYears = ScenarioManagementHistory.BuildYearViews(m.ManagementEvents, m.AnnualReports,
            new ScenarioEcologicalSnapshot[0], new ScenarioDeadwoodRecord[0], m.WorkOrders, new StormEventRecord[0]);
    }

    public static int Main()
    {
        Matrix(); GroupsAndProvenance(); Navigation(); StaticBoundaries();
        Console.WriteLine($"S1D_T05_PREVIOUS_PROPOSED_{(failures == 0 ? "PASS" : "FAIL")} checks={checks} failures={failures} NOT_UNITY_VERIFIED");
        return failures == 0 ? 0 : 1;
    }

    private static void Matrix()
    {
        // Explicit expectations, including one-year carry-over (younger than StaleAfterYears=2).
        var cases = new[] {
            (ScenarioWorkStatus.Approved, 3, "Approved"), (ScenarioWorkStatus.Approved, 5, "Approved"),
            (ScenarioWorkStatus.Approved, 6, "Approved"), (ScenarioWorkStatus.Pending, 0, "Pending carried over"),
            (ScenarioWorkStatus.Pending, 4, "Pending carried over"), (ScenarioWorkStatus.Pending, 5, "Pending new"),
            (ScenarioWorkStatus.Pending, 6, ""), (ScenarioWorkStatus.Completed, 2, ""), (ScenarioWorkStatus.Failed, 2, "") };
        foreach (ScenarioWorkType type in Enum.GetValues(typeof(ScenarioWorkType)))
        foreach (bool prescription in new[] { false, true })
        foreach (var c in cases)
        {
            var order = Order(1, c.Item1, c.Item2); order.type = type;
            if (prescription) { order.prescriptionId = 7; order.prescriptionCoverageUnitId = "unit-A"; order.prescriptionCropTreeId = "crop-A"; order.prescriptionReason = 1; }
            order.validationMessage = "Present forest needs rechecking";
            string before = JsonSerializer.Serialize(order, new JsonSerializerOptions { IncludeFields = true });
            Check(WorkPlanView.Class(order, 5) == c.Item3, $"matrix {type} {c.Item1} year={c.Item2} prescription={prescription}");
            Check(before == JsonSerializer.Serialize(order, new JsonSerializerOptions { IncludeFields = true }), "classification leaves order untouched");
        }
        Check(ScenarioInterventionHistory.StaleAfterYears == 2 && WorkPlanView.Class(Order(1, ScenarioWorkStatus.Pending, 4), 5) == "Pending carried over",
            "carry-over after one year does not wait for stale attention");
    }

    private static void GroupsAndProvenance()
    {
        var m = new ScenarioOneManager { CurrentEcologicalYear = 5 };
        // Execute the production prescription creation/provenance loop and production felling factory.
        var link = new Approval.OrderLink { PrescriptionId = 7, TargetTreeId = "target-7", CoverageUnitId = "unit-7", CropTreeId = "crop-7", ReasonCode = 1 };
        var pending = m.MakePrescriptionOrders(new Plan { Orders = new List<Approval.OrderLink> { link } },
            new Dictionary<string, ForestTree> { [link.TargetTreeId] = new ForestTree { TreeId = link.TargetTreeId } }).Single();
        Check(pending.status == ScenarioWorkStatus.Pending && pending.createdYear == 5 && WorkPlanView.Class(pending, 5) == "Pending new", "MFM creation is ordinary Pending new, not Work Plan approval");
        Check(pending.prescriptionId == 7 && pending.prescriptionCoverageUnitId == "unit-7" && pending.prescriptionCropTreeId == "crop-7" && pending.prescriptionReason == 1,
            "production writer attaches all four recorded provenance values");
        m.WorkOrders.AddRange(new[] { pending, Order(1, ScenarioWorkStatus.Approved, 2), Order(2, ScenarioWorkStatus.Pending, 4),
            Order(3, ScenarioWorkStatus.Pending, 3), Order(4, ScenarioWorkStatus.Pending, 6), Order(5, ScenarioWorkStatus.Completed, 2), Order(6, ScenarioWorkStatus.Failed, 2),
            Order(7, ScenarioWorkStatus.Approved, 6) });
        CompleteYears(m, 0, 2, 4);
        var ui = new ScenarioOneUiRoot(m); var view = new WorkPlanView(ui);
        string before = Fingerprint(m); view.Build();
        Check(Fingerprint(m) == before, "building group list never writes/approves/cancels/acknowledges");
        var folds = All(view.scroll).OfType<Foldout>().ToList();
        Check(folds.Count == 3 && folds.Select(f => f.text).SequenceEqual(new[] { "Approved · 2", "Pending new · 1", "Pending carried over · 2" }), "exactly three explicit groups with faithful counts");
        Check(Text(folds[0]).Contains("#1 · Fell tree-1") && Text(folds[2]).Contains("#2 · Fell tree-2"), "open ordinary orders shown in their own group");
        string text = Text(view.scroll);
        Check(text.Contains("prescription 7 · coverage unit unit-7 · Crop Tree crop-7 · reason code 1"), "MFM provenance displayed under its ordinary Pending order");
        Check(!text.Contains("#5 · Fell") && !text.Contains("#6 · Fell"), "completed/failed records excluded from current proposals");
        Check(text.Contains("Inconsistent record · order #4") && text.Contains("Inconsistent record · order #7"), "future Pending and Approved creation dates surfaced explicitly");
        Check(!Text(folds[1]).Contains("tree-4") && !Text(folds[2]).Contains("tree-4"), "future Pending record never labelled new or carried over");
        Check(view.ReadStatus(m.WorkOrders.Single(o => o.workOrderId == 4)).Contains("inconsistent creation year"), "detailed task status also surfaces malformed year");
        var buttons = All(view.scroll).OfType<Button>().ToList();
        Check(buttons.Select(b => b.name).OrderBy(n => n).SequenceEqual(new[] { "workplan-origin-1", "workplan-origin-2" }), "only exact recorded prior creation years have links");
        Check(text.Contains("Creation-year review not recorded"), "missing earlier year clearly has no recorded review");
        Check(buttons.Single(b => b.name == "workplan-origin-1").text == "Creation-year review · Year 2", "Approved link says creation year, never approval year");
        before = Fingerprint(m); buttons.Single(b => b.name == "workplan-origin-2").Click();
        Check(ui.CurrentScreen == ScenarioOneUiRoot.UiScreen.Review && ui.Review.RenderedYear == 4, "carried-order button opens its exact existing S1-C year");
        Check(Fingerprint(m) == before, "origin link does not mutate or acknowledge history");
        m.CurrentEcologicalYear = 6; CompleteYears(m, 0, 2, 4, 5); view.Build();
        Check(pending.status == ScenarioWorkStatus.Pending && Text(All(view.scroll).OfType<Foldout>().Single(f => f.name == "workplan-pending-carried-over")).Contains("prescription 7"), "MFM work carries over without changing its Pending status or provenance");
        pending.status = ScenarioWorkStatus.Approved; view.Build();
        Check(Text(All(view.scroll).OfType<Foldout>().Single(f => f.name == "workplan-approved")).Contains("prescription 7"), "recorded Work Plan approval moves ordinary MFM work to Approved");
        // Defensive presentation: even if a current-year entry were supplied, no current-origin link is offered.
        var current = Order(99, ScenarioWorkStatus.Pending, 6); m.WorkOrders.Add(current); CompleteYears(m, 0, 2, 4, 5, 6); view.Build();
        Check(!All(view.scroll).Any(e => e.name == "workplan-origin-99"), "Pending new never gets a fabricated completed-year destination");
    }

    private static void Navigation()
    {
        var m = new ScenarioOneManager { CurrentEcologicalYear = 5 };
        CompleteYears(m, 0, 2, 4);
        var ui = new ScenarioOneUiRoot(m); string before = Fingerprint(m);
        Check(ui.TryShowReviewYear(2) && ui.Review.SelectedYear == 2 && ui.Review.RenderedYear == 2, "first open preserves requested historical year through reset");
        Check(ui.CurrentScreen == ScenarioOneUiRoot.UiScreen.Review && !m.WorkPlanOpen && m.AuxiliaryOpen && !ui.HelpOpen, "root exact-year route applies review panel lifecycle");
        Check(!ui.Review.FooterVisible && !m.AnnualReviewSeen, "historical navigation hides acknowledgement without acknowledging");
        int refreshes = ui.Review.Refreshes;
        Check(!ui.TryShowReviewYear(3) && ui.Review.SelectedYear == 2 && ui.Review.RenderedYear == 2 && ui.Review.Refreshes == refreshes, "absent year leaves selection/rendering untouched");
        Check(!ui.TryShowReviewYear(-1) && !ui.TryShowReviewYear(99), "negative/future destinations rejected without fallback");
        Check(Fingerprint(m) == before, "all navigation leaves recorded history and acknowledgement unchanged");
        CompleteYears(m, 0, 2, 4, 5);
        Check(ui.TryShowReviewYear(0) && ui.Review.RenderedYear == 0, "new-latest refresh preserves explicit year zero request");
        Check(ui.TryShowReviewYear(2) && ui.Review.RenderedYear == 2, "route changes year while Review already open");
        ui.SetScreen(ScenarioOneUiRoot.UiScreen.None); ui.ShowReview();
        Check(ui.Review.RenderedYear == 5 && ui.Review.SelectedYear == -1, "ordinary review entry still follows latest");
        Check(ui.Review.FooterVisible && !m.AnnualReviewSeen, "latest still offers existing acknowledgement without auto-ack");
        CompleteYears(m, 0, 2, 4, 5, 6); ui.Review.Refresh(true);
        Check(ui.Review.RenderedYear == 6, "later ordinary annual advance still resets to new latest");
        var empty = new ScenarioOneManager(); var emptyUi = new ScenarioOneUiRoot(empty);
        Check(!emptyUi.TryShowReviewYear(0) && empty.WorkPlanOpen && !empty.AuxiliaryOpen && emptyUi.HelpOpen && emptyUi.CurrentScreen == ScenarioOneUiRoot.UiScreen.None,
            "empty history leaves the whole UI route untouched");
        var freshUi = new ScenarioOneUiRoot(m);
        Check(!freshUi.TryShowReviewYear(3) && freshUi.CurrentScreen == ScenarioOneUiRoot.UiScreen.None && freshUi.Review.SelectedYear == -1,
            "missing year on first request never opens another year");
    }

    private static void StaticBoundaries()
    {
        string root = Environment.GetEnvironmentVariable("CCF_REPO_ROOT");
        string plan = File.ReadAllText(Path.Combine(root, "Assets/ForestPrototype/UI/WorkPlanView.cs"));
        string annual = File.ReadAllText(Path.Combine(root, "Assets/ForestPrototype/UI/AnnualReviewView.cs"));
        string routing = File.ReadAllText(Path.Combine(root, "Assets/ForestPrototype/UI/ScenarioOneUiRoot.cs"));
        string groups = plan.Split("private void BuildOrderGroups(")[1].Split("// Open orders that carry a problem")[0];
        string nav = annual.Split("public bool TryShowYear(")[1].Split("public void Refresh(")[0];
        string route = routing.Split("public bool TryShowReviewYear(")[1].Split("public void ShowMap(")[0];
        Check(groups.Contains("m.HistoryYears") && nav.Contains("ui.Manager.HistoryYears"), "group links and navigation consume the existing S1-C history surface");
        Check(!Regex.IsMatch(groups + nav + route, @"ScenarioManagementHistory|BuildYearViews?|new YearHistoryView|ManagementEvents\.(Add|Clear)|AcknowledgeAnnualReview\(|MarkAnnualReviewSeen\(|CaptureSaveData|RestoreSaveData|PlayerPrefs|JsonUtility"), "no duplicate history query/store or history/acknowledgement writeback");
        Check(!Regex.IsMatch(groups, @"StaleOrders|StaleAfterYears|resolvedYear|approvalYear|designationYear|ApprovePrescription\(|ApprovePendingWork\(|PreviewPrescription\("), "classification has no stale threshold, invented dates or MFM mechanics");
        Check(plan.Contains("BuildOrderGroups(m);") && Regex.Matches(plan, @"Status\(order\)").Count >= 5, "group builder is wired and existing task rows use the same status labels");
        Check(plan.Contains("Append(o.createdYear)") && plan.Contains("Append(o.prescriptionId)") && plan.Contains("Append(m.AnnualReports.Count)"), "redraw signature includes classification/provenance/history inputs");
        Check(groups.Contains("Creation-year review") && !Regex.IsMatch(groups, @"approved in year|designated in year|because|caused|improved", RegexOptions.IgnoreCase), "origin wording makes no approval/designation-time or causal claim");
        Check(routing.Contains("review.ShowLatest();") && routing.Contains("review.Refresh(true);"), "ordinary existing latest-year review route retained");
    }
}
