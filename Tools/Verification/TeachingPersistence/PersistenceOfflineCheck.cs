using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

public static class PersistenceOfflineCheck
{
    static int checks;
    static void Check(bool ok, string name) { checks++; if (!ok) throw new Exception("FAIL " + name); Console.WriteLine("PASS " + name); }
    static ScenarioOneRackBay.Analysis Bay()
    {
        var bay = new ScenarioOneRackBay.Analysis { Applies = true, AreaCropCount = 3, NearSelected = true, MiddleSelected = true,
            FarSelected = true, BothSides = true, ReleaseExecuted = true, MixedExecuted = true, ReleaseYear = 2, MinExecuted = .31f,
            Geometry = new ScenarioOneRackBay.Frame(new RackPoint(-40, 0), new RackPoint(40, 0), 8) };
        for (int i = 0; i < 3; i++) bay.Future.Add(new ScenarioOneRackBay.FutureTree("T" + i, (i - 1) * 10, i == 1 ? -3 : 3, 30, 0, .31f, 0));
        return bay;
    }
    static ScenarioOneTeachingSequence.Facts Facts() => new ScenarioOneTeachingSequence.Facts { AccessOpen = true, AccessReviewed = true,
        CropTrees = 3, ReleasePlanned = true, ReleaseExecuted = true, DeadwoodExecuted = true, ReleaseReviewed = true, Bay = Bay() };
    static ScenarioOneTeachingSequence.Facts Copy(ScenarioOneTeachingSequence.Facts f)
    {
        var copy = new ScenarioOneTeachingSequence.Facts(); foreach (var field in f.GetType().GetFields()) field.SetValue(copy, field.GetValue(f)); return copy;
    }
    static int Old(ScenarioOneTeachingSequence.Facts f)
    {
        var old = new OldTeachingSequence.Facts(); foreach (var field in old.GetType().GetFields()) field.SetValue(old, f.GetType().GetField(field.Name).GetValue(f));
        return OldTeachingSequence.Current(old);
    }
    public static int Main()
    {
        string root = Environment.GetEnvironmentVariable("CCF_REPO_ROOT");
        int rows = 0;
        for (int bits = 0; bits < 8192; bits++)
        {
            var f = Facts();
            string[] names = { "AccessOpen", "AccessReviewed", "ReleasePlanned", "ReleaseExecuted", "DeadwoodExecuted", "ReleaseReviewed", "RackReviewComplete", "ReviewedTwoCycles", "BeechEstablished", "OakEstablished", "EstablishmentReviewed", "AreaReleased" };
            for (int i = 0; i < names.Length; i++) f.GetType().GetField(names[i]).SetValue(f, (bits & (1 << i)) != 0);
            if ((bits & 4096) != 0) f.Bay = null;
            foreach (int crop in new[] { 0, 3 })
            {
                f.CropTrees = crop;
                if (ScenarioOneTeachingSequence.Current(f) != Old(f) || ScenarioOneTeachingSequence.CurrentUnlatched(f) != Old(f)) throw new Exception("old sequence mismatch " + bits);
                var after = Copy(f); after.RackReviewComplete = true;
                bool expected = f.Bay != null && !f.RackReviewComplete && after.ReleaseReviewed && Old(f) == 4 && Old(after) == 5;
                if (ScenarioOneTeachingSequence.CrossesFirstRackReview(f, after) != expected) throw new Exception("boundary mismatch " + bits);
                rows++;
            }
        }
        Check(rows == 16384, "16384 pre-latch rows equal exact base sequence and independent transition oracle");
        var before = Facts(); var afterFacts = Copy(before); afterFacts.RackReviewComplete = true;
        Check(ScenarioOneTeachingCap.SampleHistoricallyComplete(before.Bay, true) && !ScenarioOneTeachingSequence.CrossesFirstRackReview(before, before), "T-03 helper alone cannot earn the latch");
        var manager = new ScenarioOneManager();
        Check(!manager.FirstGuidedRackReviewComplete && !manager.TryRecordFirstGuidedRackReview(before, before), "starts false; incomplete review cannot write");
        Check(manager.TryRecordFirstGuidedRackReview(before, afterFacts) && manager.FirstGuidedRackReviewComplete, "genuine response to recruitment records true");
        for (int i = 0; i < 10; i++) Check(!manager.TryRecordFirstGuidedRackReview(before, afterFacts) && manager.FirstGuidedRackReviewComplete, "repeat cannot write again " + i);
        // Drive the real pure session review, including all three decisions and all six stand portions.
        var review = new ScenarioOneRackReview(); var bay = before.Bay;
        review.Synchronize(bay, false, 0); Check(!review.Complete(bay), "unacknowledged Annual Review cannot complete session evidence");
        review.Synchronize(bay, true, 0); int serial = 1;
        foreach (var tree in bay.Future) { review.Observe(bay, tree.Id, tree.X, tree.Z, serial++); Check(review.Decide(bay, tree.Id, tree.X, tree.Z, ScenarioOneRackReview.Decision.RetainHere), "real crop decision " + tree.Id); }
        Check(!review.Complete(bay), "three decisions without all stand portions remain incomplete");
        for (int i = 0; i < 6; i++) { float x = (i / 2 - 1) * 10, z = i % 2 == 0 ? -3 : 3; review.Observe(bay, "S" + i, x, z, serial++); Check(review.Decide(bay, "S" + i, x, z, ScenarioOneRackReview.Decision.RetainHere), "real stand portion " + i); }
        Check(review.Complete(bay), "real session review reaches completion with three trees/six portions");
        review.Reset(serial); Check(!review.Complete(bay), "session detail resets without touching the historical manager latch");
        for (int i = 0; i < 2; i++)
        {
            bool value = i != 0; var data = new ForestSaveData { scenarioOne = new ScenarioOneSaveData { firstGuidedRackReviewComplete = value } };
            string json = JsonUtility.ToJson(data); var loaded = JsonUtility.FromJson<ForestSaveData>(json);
            Check(loaded.scenarioOne.firstGuidedRackReviewComplete == value && JsonUtility.ToJson(loaded) == json, "false/true save-load-save bytes " + value);
            string old = json.Replace("\"firstGuidedRackReviewComplete\":" + value.ToString().ToLowerInvariant() + ",", "");
            Check(!JsonUtility.FromJson<ForestSaveData>(old).scenarioOne.firstGuidedRackReviewComplete, "missing field defaults false regardless of other history " + value);
        }
        var drift = Facts(); drift.FirstGuidedRackReviewComplete = true; drift.AccessOpen = false; drift.CropTrees = 0; drift.ReleaseExecuted = false;
        Check(ScenarioOneTeachingSequence.Current(drift) == 5 && ScenarioOneTeachingSequence.CurrentUnlatched(drift) == 0, "shared historical read keeps first completed boundary despite world drift");
        drift.Bay.RemovedBasalShare = .9f; drift.Bay.ActionableRemovals = 5; drift.Bay.MixedExecuted = false;
        Check(ScenarioOneTeachingCap.Classify(drift.Bay, true, false) == ScenarioOneTeachingCap.Phase.Hard && ScenarioOneTeachingCap.Classify(drift.Bay, true, false, true) == ScenarioOneTeachingCap.Phase.Advisory, "pre-latch cap unchanged; post-latch cap advisory under drift");
        Check(TeachingCapOfflineCheck.Main(new[] { root }) == 0, "accepted T-03 complete offline decision table remains green");
        var members = typeof(ScenarioOneSaveData).GetFields().Select(f => f.Name).ToArray();
        Check(members.Count(n => n.Contains("GuidedRack")) == 1 && !members.Any(n => n.Contains("RackReviewComplete") && n != "firstGuidedRackReviewComplete"), "exactly one teaching field; no session review detail DTO");
        string view = File.ReadAllText(Path.Combine(root, "Assets/ForestPrototype/UI/LearningObjectivesView.Sequence.cs"));
        string factsBody = view.Substring(view.IndexOf("private ScenarioOneTeachingSequence.Facts TeachingFacts()"), view.IndexOf("public void ObserveRackInspection") - view.IndexOf("private ScenarioOneTeachingSequence.Facts TeachingFacts()"));
        Check(!factsBody.Contains("TryRecordFirstGuidedRackReview") && view.Split(new[] {"TryRecordFirstGuidedRackReview"}, StringSplitOptions.None).Length == 2, "facts refresh cannot write; one live decision call site");
        Console.WriteLine("TEACHING_PERSISTENCE_OFFLINE_PASS checks=" + checks); return 0;
    }
}
