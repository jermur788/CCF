using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

public static class S1CGuidanceOfflineCheck
{
    private static int checks, failures;
    private static void Check(bool ok, string name)
    {
        checks++; if (!ok) failures++;
        Console.WriteLine((ok ? "PASS " : "FAIL ") + name);
    }
    public static int Main()
    {
        foreach (ScenarioEcologicalTreatment treatment in Enum.GetValues(typeof(ScenarioEcologicalTreatment)))
        {
            var item = new WorkItemDetail { Status = WorkItemStatus.Succeeded, Treatment = treatment, CellIndex = 7, Year = 2, EventId = 42 };
            string before = Fields(item);
            string copy = ManagementHistoryText.PriorTreatmentGuidance(item);
            Check((treatment == ScenarioEcologicalTreatment.None) == (copy.Length == 0), "only known biological treatment gets guidance: " + treatment);
            Check(!Regex.IsMatch(copy, @"\b(because|caused|causes|due to|led to|always|must|should fell|thin every|improved|guarantee)\b", RegexOptions.IgnoreCase), "no unsupported causality or prescription: " + treatment);
            Check(ManagementHistoryText.PriorTreatmentWaypointCell(item, 256) == (treatment == ScenarioEcologicalTreatment.None ? -1 : 7), "recorded cell passed unchanged: " + treatment);
            Check(before == Fields(item), "guidance query does not mutate input: " + treatment);
            item.Status = WorkItemStatus.Failed;
            Check(ManagementHistoryText.PriorTreatmentGuidance(item) == "" && ManagementHistoryText.PriorTreatmentWaypointCell(item, 256) == -1, "failed work never claims treatment: " + treatment);
            item.Status = WorkItemStatus.Cancelled;
            Check(ManagementHistoryText.PriorTreatmentGuidance(item) == "", "cancelled work never claims treatment: " + treatment);
        }
        var plant = new WorkItemDetail { Status = WorkItemStatus.Succeeded, Treatment = ScenarioEcologicalTreatment.JuvenilePlanted, CellIndex = -1, TargetTreeId = "CURRENT-TREE-EXISTS", PositionRecorded = true, PositionX = 10, PositionZ = 10 };
        Check(ManagementHistoryText.PriorTreatmentWaypointCell(plant, 256) == -1, "no cell means no waypoint even with target or position");
        plant.CellIndex = 256;
        Check(ManagementHistoryText.PriorTreatmentWaypointCell(plant, 256) == -1, "out of range cell never accepted");
        plant.CellIndex = 0;
        Check(ManagementHistoryText.PriorTreatmentWaypointCell(plant, 256) == 0, "zero cell valid (not missing sentinel)");
        Check(ManagementHistoryText.PriorTreatmentWaypointCell(plant, 0) == -1, "no live grid means no waypoint");
        Check(ManagementHistoryText.PriorTreatmentGuidance(null) == "" && ManagementHistoryText.PriorTreatmentWaypointCell(null, 256) == -1, "null safe");
        plant.ShelterInstalled = true;
        Check(!ManagementHistoryText.PriorTreatmentGuidance(plant).Contains("shelter"), "missing shelter provenance does not claim a shelter");
        plant.ShelterRecorded = true;
        Check(ManagementHistoryText.PriorTreatmentGuidance(plant).Contains("shelter's condition"), "recorded shelter offered for inspection only");
        string root = Environment.GetEnvironmentVariable("CCF_REPO_ROOT");
        string ui = File.ReadAllText(Path.Combine(root, "Assets/ForestPrototype/UI/AnnualReviewView.cs"));
        string guidance = ui.Split(new[] { "private void BuildPriorTreatmentGuidance" }, StringSplitOptions.None)[1].Split(new[] { "private void BuildStorm" }, StringSplitOptions.None)[0];
        Check(guidance.Contains("ScenarioManagementHistory.DescribeWork(year)") && guidance.Contains("work.Done"), "uses existing read-only year history and successful work");
        Check(guidance.Contains("L4 MANAGEMENT GUIDANCE") && guidance.Contains("L4 GUIDANCE"), "guidance visibly separated from facts");
        Check(guidance.Contains("HistoryProvenance.Recorded") && guidance.Contains("HistoryProvenance.NotRecorded"), "recorded and absent source fields classified separately");
        Check(!Regex.IsMatch(guidance, @"GetCellIndex|FindTree|CaptureSaveData|Acknowledge|AdvanceYear|PlayerPrefs|JsonUtility"), "no world lookup to fill gaps or history writeback");
        Console.WriteLine($"S1C_T06_GUIDANCE_{(failures == 0 ? "PASS" : "FAIL")} checks={checks} failures={failures}");
        return failures == 0 ? 0 : 1;
    }
    private static string Fields(WorkItemDetail i) => $"{i.Status}|{i.Treatment}|{i.CellIndex}|{i.Year}|{i.EventId}|{i.PositionX}|{i.TargetTreeId}";
}
