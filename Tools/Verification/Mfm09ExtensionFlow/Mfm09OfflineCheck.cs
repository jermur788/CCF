// Offline check for MFM-09 "Extend your treatment". Compiles the PRODUCTION ScenarioOneTeachingSequence.cs with the pure rack-area types,
// the generated read-only latch members from ScenarioOneManager.cs, and two methods extracted verbatim from the PRODUCTION WorkPlanView.cs
// (OrderClass and PrescriptionOriginNote, see run_offline.py). It proves pure decision rules only; it does not prove Unity import,
// rendering, input or a real serialized save.
using System;
using System.Collections.Generic;
using System.Linq;

// Minimal stand-ins for the two production types the extracted Work Plan methods read. Their member names and the IsOpen rule are the
// production ones (ScenarioOneWorkOrder.cs); run_offline.py fails if that source text ever differs.
public enum ScenarioWorkStatus { Pending, Approved, Completed, Cancelled, Failed }
public sealed class ScenarioOneWorkOrder
{
    public ScenarioWorkStatus status;
    public int createdYear, prescriptionId;
    public bool IsOpen => status == ScenarioWorkStatus.Pending || status == ScenarioWorkStatus.Approved;
}

public static class Mfm09OfflineCheck
{
    private static int checks, failures;
    private static void Check(bool ok, string name)
    {
        checks++;
        Console.WriteLine((ok ? "PASS " : "FAIL ") + name);
        if (!ok) failures++;
    }
    private static readonly string[] OldIds = { "access", "future-trees", "release", "habitat", "response", "recruitment", "establishment", "transformation" };
    private static readonly string[] Names = { "AccessOpen", "AccessReviewed", "ReleasePlanned", "ReleaseExecuted", "DeadwoodExecuted",
        "ReleaseReviewed", "RackReviewComplete", "ReviewedTwoCycles", "BeechEstablished", "OakEstablished", "EstablishmentReviewed", "AreaReleased" };

    private static ScenarioOneRackBay.Analysis Bay()
    {
        var bay = new ScenarioOneRackBay.Analysis
        {
            Applies = true, AreaCropCount = 3, NearSelected = true, MiddleSelected = true, FarSelected = true, BothSides = true,
            ReleasePlanned = true, ReleaseExecuted = true, MixedPlanned = true, MixedExecuted = true, ReleaseYear = 2,
            MinPlannedAndExecuted = .31f, MinExecuted = .31f, RemovedBasalShare = 0f,
            Geometry = new ScenarioOneRackBay.Frame(new RackPoint(-40, 0), new RackPoint(40, 0), 8)
        };
        for (int i = 0; i < 3; i++) bay.Future.Add(new ScenarioOneRackBay.FutureTree("T" + i, (i - 1) * 10, i == 1 ? -3 : 3, 30, .30f, .31f, 0f));
        return bay;
    }
    // Every earlier lesson held and the real session review complete: the accepted pre-latch path has reached recruitment.
    private static ScenarioOneTeachingSequence.Facts Reached() => new ScenarioOneTeachingSequence.Facts
    {
        AccessOpen = true, AccessReviewed = true, CropTrees = 3, ReleasePlanned = true, ReleaseExecuted = true, DeadwoodExecuted = true,
        ReleaseReviewed = true, RackReviewComplete = true, Bay = Bay(), Year = 5, PlantingYear = -1
    };
    private static ScenarioOneTeachingSequence.Facts Copy(ScenarioOneTeachingSequence.Facts f)
    {
        var copy = new ScenarioOneTeachingSequence.Facts();
        foreach (var field in typeof(ScenarioOneTeachingSequence.Facts).GetFields()) field.SetValue(copy, field.GetValue(f));
        return copy;
    }
    private static string Snapshot(ScenarioOneTeachingSequence.Facts f)
        => string.Join("|", typeof(ScenarioOneTeachingSequence.Facts).GetFields().Select(x => x.Name + "=" + (x.GetValue(f) is ScenarioOneRackBay.Analysis a ? "bay" + a.AreaCropCount : x.GetValue(f))));

    // Independent oracle: the accepted pre-MFM-09 sequence (copied from the T-04 oracle at c8cfae8c / the start commit), eight stages.
    private static int OracleTail(ScenarioOneTeachingSequence.Facts f)
    {
        if (!f.BeechEstablished || !f.OakEstablished) return 5;
        if (f.PlantingYear < 0 || f.Year <= f.PlantingYear || !f.EstablishmentReviewed) return 6;
        return 7;
    }
    private static int OracleUnlatched(ScenarioOneTeachingSequence.Facts f)
    {
        if (!f.AccessOpen) return 0;
        if (!f.AccessReviewed || (f.CropTrees == 0 && !f.ReleaseExecuted) || (f.Bay != null && !f.Bay.HasEnoughFuture)) return 1;
        if (f.Bay != null && !f.Bay.CanopyKept) return 2;
        if (!f.ReleasePlanned && !f.ReleaseExecuted) return 2;
        if (!f.ReleaseExecuted || !f.DeadwoodExecuted) return 3;
        if (!f.ReleaseReviewed || (f.Bay != null ? !f.RackReviewComplete : !f.ReviewedTwoCycles || !f.AreaReleased)) return 4;
        return OracleTail(f);
    }
    private static int OracleCurrent(ScenarioOneTeachingSequence.Facts f) => f.FirstGuidedRackReviewComplete ? OracleTail(f) : OracleUnlatched(f);

    public static int Main(string[] args)
    {
        Ids();
        Sweep();
        Latch();
        Numbering();
        Copy();
        WorkPlan();
        Console.WriteLine(failures == 0
            ? "MFM09_OFFLINE_PASS checks=" + checks + " NOT_UNITY_VERIFIED"
            : "MFM09_OFFLINE_FAIL failures=" + failures + " checks=" + checks);
        return failures == 0 ? 0 : 1;
    }

    private static void Ids()
    {
        var old = ScenarioOneTeachingSequence.Lessons.Select(l => l.Id).ToArray();
        Check(old.SequenceEqual(OldIds), "the eight accepted lesson IDs are unchanged, in order (Lessons)");
        Check(ScenarioOneTeachingSequence.Lessons.Length == 8, "Lessons still holds exactly the eight accepted lessons");
        var shown = ScenarioOneTeachingSequence.PresentedLessons.Select(l => l.Id).ToArray();
        Check(shown.Length == 9 && shown.Distinct().Count() == 9, "nine presented lessons, every ID unique");
        Check(ScenarioOneTeachingSequence.ExtensionId == "extend-treatment" && !OldIds.Contains(ScenarioOneTeachingSequence.ExtensionId),
            "the extension ID is new and does not reuse an old ID");
        Check(shown.SequenceEqual(new[] { "access", "future-trees", "release", "habitat", "response", "extend-treatment", "recruitment", "establishment", "transformation" }),
            "presented order places the extension after response and before recruitment");
        Check(ReferenceEquals(ScenarioOneTeachingSequence.PresentedLessons[5], ScenarioOneTeachingSequence.Extension)
            && Enumerable.Range(0, 5).All(i => ReferenceEquals(ScenarioOneTeachingSequence.PresentedLessons[i], ScenarioOneTeachingSequence.Lessons[i]))
            && Enumerable.Range(6, 3).All(i => ReferenceEquals(ScenarioOneTeachingSequence.PresentedLessons[i], ScenarioOneTeachingSequence.Lessons[i - 1])),
            "the eight accepted lessons are reused as-is, not copied or renamed");
    }

    // 262144 rows: the accepted stage is independent of every MFM-09 fact, and the extension is offered only after the genuine response.
    private static void Sweep()
    {
        int rows = 0, bad = 0, offeredRows = 0, earlyOffered = 0, curMismatch = 0;
        for (int bits = 0; bits < 4096; bits++)
            foreach (bool bay in new[] { true, false })
                foreach (int crop in new[] { 0, 3 })
                    foreach (bool latched in new[] { false, true })
                        foreach (int tail in new[] { 0, 1 })
                            for (int ext = 0; ext < 4; ext++)
                            {
                                var f = Reached();
                                for (int i = 0; i < Names.Length; i++) typeof(ScenarioOneTeachingSequence.Facts).GetField(Names[i]).SetValue(f, (bits & (1 << i)) != 0);
                                if (!bay) f.Bay = null;
                                f.CropTrees = crop; f.FirstGuidedRackReviewComplete = latched;
                                if (tail == 1) { f.PlantingYear = 3; f.Year = 5; }
                                f.ExtensionApplies = (ext & 1) != 0; f.ExtensionApproved = (ext & 2) != 0;
                                rows++;
                                int oracle = OracleCurrent(f);
                                if (ScenarioOneTeachingSequence.Current(f) != oracle || ScenarioOneTeachingSequence.CurrentUnlatched(f) != OracleUnlatched(f)) curMismatch++;
                                bool offered = ScenarioOneTeachingSequence.ExtensionOffered(f);
                                bool expectOffered = f.ExtensionApplies && !f.ExtensionApproved && oracle == 5;
                                int presented = ScenarioOneTeachingSequence.Presented(f);
                                int expectPresented = expectOffered ? 5 : oracle >= 5 ? oracle + 1 : oracle;
                                if (offered != expectOffered || presented != expectPresented) bad++;
                                if (offered) { offeredRows++; if (OracleUnlatched(f) < 4 && !latched) earlyOffered++; if (oracle < 5) earlyOffered++; }
                                if (oracle < 5 && presented >= 5) bad++;
                                if (!offered && ReferenceEquals(ScenarioOneTeachingSequence.PresentedLessons[presented], ScenarioOneTeachingSequence.Extension)) bad++;
                            }
        Check(rows == 262144, "swept 262144 fact rows");
        Check(curMismatch == 0, "Current/CurrentUnlatched equal the accepted eight-stage oracle on every row whatever the MFM-09 facts are");
        Check(bad == 0, "ExtensionOffered/Presented follow the rule on every row; no row shows the extension before the response review is complete");
        Check(offeredRows > 0 && earlyOffered == 0, "the extension is offered on some rows, never before the response stage");
    }

    // D-066: nothing about the extension reopens, resets or re-earns the first guided review.
    private static void Latch()
    {
        var before = Reached(); before.RackReviewComplete = false; before.ExtensionApplies = true;
        var after = Copy(before); after.RackReviewComplete = true;
        Check(ScenarioOneTeachingSequence.CrossesFirstRackReview(before, after), "the genuine response review crossing is still detected with the extension present");
        var m = new ScenarioOneManager();
        Check(!m.FirstGuidedRackReviewComplete && m.TryRecordFirstGuidedRackReview(before, after) && m.FirstGuidedRackReviewComplete, "the crossing earns the latch exactly once");
        Check(!m.TryRecordFirstGuidedRackReview(before, after) && m.FirstGuidedRackReviewComplete, "the latch is one-way");
        Check(ScenarioOneTeachingSequence.Presented(after) == 5 && ScenarioOneTeachingSequence.ExtensionOffered(after), "after the genuine review the extension is offered next");
        var onlyExtension = Copy(before); onlyExtension.ExtensionApproved = true; onlyExtension.RackReviewComplete = false;
        Check(!ScenarioOneTeachingSequence.CrossesFirstRackReview(before, onlyExtension), "approving the extension cannot earn the first guided review");
        var m2 = new ScenarioOneManager();
        Check(!m2.TryRecordFirstGuidedRackReview(before, onlyExtension) && !m2.FirstGuidedRackReviewComplete, "extension facts cannot write the latch");

        // Reload: only the latch survives; every world fact is gone or drifted. The review stays complete, the extension is merely offered.
        var reloaded = new ScenarioOneTeachingSequence.Facts { FirstGuidedRackReviewComplete = true, ExtensionApplies = true, Year = 9, PlantingYear = -1 };
        Check(ScenarioOneTeachingSequence.Current(reloaded) == 5 && ScenarioOneTeachingSequence.Presented(reloaded) == 5,
            "latched reload with no other facts stays past the review and offers the extension, not response");
        var reloadedDone = Copy(reloaded); reloadedDone.ExtensionApproved = true;
        Check(ScenarioOneTeachingSequence.Current(reloadedDone) == 5 && ScenarioOneTeachingSequence.Presented(reloadedDone) == 6,
            "an approved extension returns the sequence to recruitment (presented index 6)");
        var legacy = new ScenarioOneTeachingSequence.Facts { FirstGuidedRackReviewComplete = true, Year = 9, PlantingYear = -1 };
        Check(!ScenarioOneTeachingSequence.ExtensionOffered(legacy) && ScenarioOneTeachingSequence.Current(legacy) == 5
            && ScenarioOneTeachingSequence.PresentedLessons[ScenarioOneTeachingSequence.Presented(legacy)] == ScenarioOneTeachingSequence.Lessons[5]
            && ScenarioOneTeachingSequence.Number(ScenarioOneTeachingSequence.Lessons[5], legacy) == 6,
            "a world without the extension (Legacy40/injected facts) shows exactly the accepted lesson and number");
        var planted = Copy(reloaded); planted.BeechEstablished = planted.OakEstablished = true; planted.PlantingYear = 4; planted.Year = 9; planted.EstablishmentReviewed = true;
        Check(!ScenarioOneTeachingSequence.ExtensionOffered(planted) && ScenarioOneTeachingSequence.Presented(planted) == 8,
            "a latched player already past recruitment is not pulled back to the extension");

        var snap = Snapshot(reloaded);
        for (int i = 0; i < 50; i++) { ScenarioOneTeachingSequence.Current(reloaded); ScenarioOneTeachingSequence.Presented(reloaded); ScenarioOneTeachingSequence.ExtensionOffered(reloaded); }
        Check(Snapshot(reloaded) == snap, "repeated reads change nothing");
    }

    private static void Numbering()
    {
        var with = new ScenarioOneTeachingSequence.Facts { ExtensionApplies = true };
        var without = new ScenarioOneTeachingSequence.Facts();
        var shown = ScenarioOneTeachingSequence.PresentedLessons;
        Check(Enumerable.Range(0, 9).All(i => ScenarioOneTeachingSequence.Number(shown[i], with) == i + 1) && ScenarioOneTeachingSequence.Total(with) == 9,
            "with the extension the lessons are numbered 1 to 9 in presented order");
        Check(ScenarioOneTeachingSequence.Lessons.Select((l, i) => ScenarioOneTeachingSequence.Number(l, without) == i + 1 && ScenarioOneTeachingSequence.DisplayTitle(l, without) == l.Title).All(x => x)
            && ScenarioOneTeachingSequence.Total(without) == 8,
            "without the extension every lesson shows its authored title and number");
        Check(ScenarioOneTeachingSequence.DisplayTitle(ScenarioOneTeachingSequence.Extension, with) == "6. Extend your treatment"
            && ScenarioOneTeachingSequence.DisplayTitle(ScenarioOneTeachingSequence.Lessons[5], with) == "7. Secure the next generation",
            "visible numbering moves after the extension while the IDs do not");
    }

    private static void Copy()
    {
        var e = ScenarioOneTeachingSequence.Extension;
        string all = (e.Concept + " " + e.Action + " " + e.Practice + " " + e.HudAction).ToLowerInvariant();
        string[] needed = { "derives a prescription", "carried out", "covered", "not covered", "excluded", "manual", "candidate", "confirm", "proposed release", "3d", "spot-check",
            "approve the prescription", "pending", "work plan", "origin", "not approving the work plan", "not a fell mark", "spacing" };
        var missing = needed.Where(k => !all.Contains(k)).ToList();
        Check(missing.Count == 0, "the extension copy teaches every required step" + (missing.Count > 0 ? " (missing: " + string.Join(", ", missing) + ")" : ""));
        Check(all.Contains("not a stocking prescription"), "the copy says the sample is not a stand-wide stocking prescription");
        Check(all.Contains("nothing is committed, funded or carried out until you approve it there"), "the copy states Work Plan approval is the commitment gate");
    }

    private static void WorkPlan()
    {
        string[] allowed = { "Approved", "Pending new", "Pending carried over", "" };
        int rows = 0, bad = 0, provenanceDiffers = 0;
        foreach (ScenarioWorkStatus status in Enum.GetValues(typeof(ScenarioWorkStatus)))
            foreach (int offset in new[] { -2, -1, 0, 1 })
                foreach (int cur in new[] { 0, 1, 7 })
                {
                    string expected;
                    bool open = status == ScenarioWorkStatus.Pending || status == ScenarioWorkStatus.Approved;
                    if (!open) expected = "";
                    else if (status == ScenarioWorkStatus.Approved) expected = "Approved";
                    else expected = offset == 0 ? "Pending new" : offset < 0 ? "Pending carried over" : "";
                    string[] results = new string[2];
                    int k = 0;
                    foreach (int rx in new[] { 0, 7 })
                    {
                        var o = new ScenarioOneWorkOrder { status = status, createdYear = cur + offset, prescriptionId = rx };
                        results[k++] = WorkPlanExtract.OrderClass(o, cur);
                        rows++;
                        if (WorkPlanExtract.OrderClass(o, cur) != expected || !allowed.Contains(WorkPlanExtract.OrderClass(o, cur))) bad++;
                    }
                    if (results[0] != results[1]) provenanceDiffers++;
                }
        Check(rows == 120 && bad == 0, "OrderClass equals the accepted #151 table (Approved / Pending new / Pending carried over / future = none) on every row, with or without prescription origin");
        Check(provenanceDiffers == 0, "prescription provenance never changes the operational class; there is no fourth status");
        var pending = new ScenarioOneWorkOrder { status = ScenarioWorkStatus.Pending, createdYear = 3, prescriptionId = 4 };
        var approved = new ScenarioOneWorkOrder { status = ScenarioWorkStatus.Approved, createdYear = 3, prescriptionId = 4 };
        string pn = WorkPlanExtract.PrescriptionOriginNote(pending), an = WorkPlanExtract.PrescriptionOriginNote(approved);
        Check(pn.Contains("ordinary Pending") && pn.Contains("approve it in this Work Plan") && !pn.Contains("since approved"), "a prescription's Pending task is explained as uncommitted ordinary Pending work");
        Check(an.Contains("since approved in this Work Plan") && !an.Contains("Still ordinary Pending"), "an approved prescription task says the Work Plan approved it");
        Check(WorkPlanExtract.PrescriptionGroupNote.Contains("not committed, funded or carried out until you approve them in this Work Plan"), "the group note states the commitment gate");
    }
}
