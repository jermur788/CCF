// Offline check for S1-D T-04 (FR-B1, FR-B2, FR-B3, AC-3): the Scenario One guide must not regress behind the
// completed response boundary once the shared #143 historical completion latch is true, while pre-latch behaviour
// stays exactly as accepted. Compiles the PRODUCTION ScenarioOneTeachingSequence.cs with the pure rack-area types
// and a generated partial manager for the latch write method. It proves the pure decision rules and source wiring
// only; it does not prove Unity behaviour, rendering or a real serialized save.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

public static class TeachingSequenceMonotonicityOfflineCheck
{
    private static int checks, failures;

    private static void Check(bool ok, string name)
    {
        checks++;
        Console.WriteLine((ok ? "PASS " : "FAIL ") + name);
        if (!ok) failures++;
    }

    private static ScenarioOneRackBay.Analysis Bay(int cropCount = 3, bool releasePlanned = true, bool releaseExecuted = true,
        bool mixedExecuted = true, float minExecuted = .31f, float removedShare = 0f)
    {
        var bay = new ScenarioOneRackBay.Analysis
        {
            Applies = true,
            AreaCropCount = cropCount,
            NearSelected = cropCount >= 1,
            MiddleSelected = cropCount >= 2,
            FarSelected = cropCount >= 3,
            BothSides = cropCount >= 2,
            ReleasePlanned = releasePlanned,
            ReleaseExecuted = releaseExecuted,
            MixedPlanned = releasePlanned && mixedExecuted,
            MixedExecuted = mixedExecuted,
            ReleaseYear = releaseExecuted ? 2 : -1,
            MinPlannedAndExecuted = releasePlanned ? minExecuted : 0f,
            MinExecuted = releaseExecuted ? minExecuted : 0f,
            RemovedBasalShare = removedShare,
            Geometry = new ScenarioOneRackBay.Frame(new RackPoint(-40, 0), new RackPoint(40, 0), 8)
        };
        for (int i = 0; i < cropCount; i++)
            bay.Future.Add(new ScenarioOneRackBay.FutureTree("T" + i, (i - 1) * 10, i == 1 ? -3 : 3, 30,
                releasePlanned ? .30f : 0f, releaseExecuted ? .31f : 0f, 0f));
        return bay;
    }

    // The complete pre-response path: every earlier lesson holds and the real session review is complete.
    private static ScenarioOneTeachingSequence.Facts Facts()
        => new ScenarioOneTeachingSequence.Facts
        {
            AccessOpen = true, AccessReviewed = true, CropTrees = 3,
            ReleasePlanned = true, ReleaseExecuted = true, DeadwoodExecuted = true,
            ReleaseReviewed = true, RackReviewComplete = true,
            Bay = Bay(), Year = 5, PlantingYear = -1
        };

    // Release results read, but the first guided rack review has not been completed in this session.
    private static ScenarioOneTeachingSequence.Facts ResponseFacts()
    {
        var f = Facts();
        f.RackReviewComplete = false;
        return f;
    }

    private static ScenarioOneTeachingSequence.Facts Copy(ScenarioOneTeachingSequence.Facts f)
    {
        var copy = new ScenarioOneTeachingSequence.Facts();
        foreach (var field in typeof(ScenarioOneTeachingSequence.Facts).GetFields())
            field.SetValue(copy, field.GetValue(f));
        return copy;
    }

    private static string Snapshot(ScenarioOneTeachingSequence.Facts f)
        => string.Join("|", typeof(ScenarioOneTeachingSequence.Facts).GetFields().Select(field =>
        {
            object value = field.GetValue(f);
            if (value == null) return field.Name + "=null";
            if (value is ScenarioOneRackBay.Analysis bay)
                return field.Name + "=bay:" + bay.GetHashCode() + ":" + bay.AreaCropCount + ":" + bay.Future.Count
                    + ":" + bay.RemovedBasalShare + ":" + bay.ReleaseYear;
            return field.Name + "=" + value;
        }));

    // The accepted pre-amendment sequence, copied verbatim from c8cfae8c, is the independent pre-latch oracle.
    private static int OldAccepted(ScenarioOneTeachingSequence.Facts f)
    {
        if (!f.AccessOpen) return 0;
        if (!f.AccessReviewed || (f.CropTrees == 0 && !f.ReleaseExecuted) || (f.Bay != null && !f.Bay.HasEnoughFuture)) return 1;
        if (f.Bay != null && !f.Bay.CanopyKept) return 2;
        if (!f.ReleasePlanned && !f.ReleaseExecuted) return 2;
        if (!f.ReleaseExecuted || !f.DeadwoodExecuted) return 3;
        if (!f.ReleaseReviewed || (f.Bay != null ? !f.RackReviewComplete : !f.ReviewedTwoCycles || !f.AreaReleased)) return 4;
        if (!f.BeechEstablished || !f.OakEstablished) return 5;
        if (f.PlantingYear < 0 || f.Year <= f.PlantingYear || !f.EstablishmentReviewed) return 6;
        return 7;
    }

    public static int Main(string[] args)
    {
        string root = args.Length > 0 ? args[0] : Environment.GetEnvironmentVariable("CCF_REPO_ROOT") ?? ".";
        StableIds();
        PreLatchUnchanged();
        AuthoritativeTransition();
        PostLatchFloor();
        LaterFactsStillDrive();
        SaveLoadRebuild();
        RepeatedReadsNeverWrite();
        SourceWiring(root);
        Console.WriteLine(failures == 0
            ? "TEACHING_SEQUENCE_MONOTONICITY_OFFLINE_PASS checks=" + checks + " NOT_UNITY_VERIFIED"
            : "TEACHING_SEQUENCE_MONOTONICITY_OFFLINE_FAIL failures=" + failures + " checks=" + checks);
        return failures == 0 ? 0 : 1;
    }

    // FR-B2: eight stable lesson IDs, in order, no duplicates and no reuse.
    private static void StableIds()
    {
        string[] expected = { "access", "future-trees", "release", "habitat", "response", "recruitment", "establishment", "transformation" };
        Check(ScenarioOneTeachingSequence.Lessons.Length == 8, "exactly eight lessons");
        Check(ScenarioOneTeachingSequence.Lessons.Select(l => l.Id).SequenceEqual(expected), "eight lesson IDs unchanged and in order");
        Check(ScenarioOneTeachingSequence.Lessons.Select(l => l.Id).Distinct().Count() == 8, "no lesson ID reused");
    }

    // FR-B1: with the latch false the guide is exactly the accepted pre-amendment sequence, including a read-but-
    // unreviewed response lesson.
    private static void PreLatchUnchanged()
    {
        var response = ResponseFacts();
        Check(!response.FirstGuidedRackReviewComplete, "latch is false by default on a missing/old save");
        Check(ScenarioOneTeachingSequence.Current(response) == 4,
            "ReleaseReviewed true but RackReviewComplete false stays at response");
        Check(ScenarioOneTeachingSequence.CurrentUnlatched(response) == 4, "unlatched response boundary is still response");

        var releaseUnread = Facts();
        releaseUnread.ReleaseReviewed = false;
        Check(ScenarioOneTeachingSequence.Current(releaseUnread) == 4, "unacknowledged release stays at response");

        var reached = Facts();
        Check(ScenarioOneTeachingSequence.Current(reached) == 5, "complete response reaches recruitment pre-latch");

        int rows = 0, mismatches = 0;
        string[] names = { "AccessOpen", "AccessReviewed", "ReleasePlanned", "ReleaseExecuted", "DeadwoodExecuted",
            "ReleaseReviewed", "RackReviewComplete", "ReviewedTwoCycles", "BeechEstablished", "OakEstablished",
            "EstablishmentReviewed", "AreaReleased" };
        for (int bits = 0; bits < 8192; bits++)
        {
            var f = Facts();
            for (int i = 0; i < names.Length; i++)
                typeof(ScenarioOneTeachingSequence.Facts).GetField(names[i]).SetValue(f, (bits & (1 << i)) != 0);
            if ((bits & 4096) != 0) f.Bay = null;
            foreach (int crop in new[] { 0, 3 })
            {
                f.CropTrees = crop;
                rows++;
                if (ScenarioOneTeachingSequence.Current(f) != OldAccepted(f)
                    || ScenarioOneTeachingSequence.CurrentUnlatched(f) != OldAccepted(f)) mismatches++;
            }
        }
        Check(rows == 16384 && mismatches == 0,
            "16384 pre-latch rows equal the exact accepted pre-amendment sequence and unlatched path");
    }

    // The authoritative #143 boundary: response -> recruitment only after a genuine session review, and only then
    // may the historical read be recorded. Merely reaching recruitment does not claim the latch.
    private static void AuthoritativeTransition()
    {
        var before = ResponseFacts();
        var after = Copy(before);
        after.RackReviewComplete = true;
        Check(ScenarioOneTeachingSequence.CurrentUnlatched(before) == 4
            && ScenarioOneTeachingSequence.CurrentUnlatched(after) == 5,
            "unlatched sequence crosses response -> recruitment at the real review boundary");
        Check(ScenarioOneTeachingSequence.CrossesFirstRackReview(before, after), "authoritative transition predicate accepts the real boundary");
        Check(!ScenarioOneTeachingSequence.CrossesFirstRackReview(before, before), "the same facts cannot earn the latch without completing the review");
        Check(!before.FirstGuidedRackReviewComplete && ScenarioOneTeachingSequence.Current(after) == 5,
            "reaching recruitment does not itself set the durable read; the floor is active only when it is true");

        var manager = new ScenarioOneManager();
        Check(!manager.FirstGuidedRackReviewComplete, "manager latch starts false");
        Check(!manager.TryRecordFirstGuidedRackReview(before, before), "incomplete review cannot write the latch");
        Check(manager.TryRecordFirstGuidedRackReview(before, after) && manager.FirstGuidedRackReviewComplete,
            "the real completion boundary records the latch once");
        for (int i = 0; i < 3; i++)
            Check(!manager.TryRecordFirstGuidedRackReview(before, after) && manager.FirstGuidedRackReviewComplete,
                "repeated calls cannot write or consume another latch " + i);

        var latched = Copy(after);
        latched.FirstGuidedRackReviewComplete = true;
        Check(ScenarioOneTeachingSequence.Current(latched) == 5, "latched guide starts at recruitment");
    }

    // AC-3 (i)-(iv): later Crop Tree count changes/loss, further in-footprint felling and executed-share drift may
    // move the pre-latch sequence backwards, but never the latched guide.
    private static void PostLatchFloor()
    {
        var atRecruitment = Facts();
        atRecruitment.FirstGuidedRackReviewComplete = true;
        Check(ScenarioOneTeachingSequence.Current(atRecruitment) == 5, "latched guide starts at recruitment");

        var increased = Copy(atRecruitment);
        increased.CropTrees = 4;
        increased.Bay = Bay(4);
        Check(ScenarioOneTeachingSequence.Current(increased) == 5
            && ScenarioOneTeachingSequence.CurrentUnlatched(increased) < 5,
            "post-latch Crop Tree count increase cannot drop behind recruitment");

        var decreased = Copy(atRecruitment);
        decreased.CropTrees = 2;
        decreased.Bay = Bay(2);
        Check(ScenarioOneTeachingSequence.Current(decreased) == 5
            && ScenarioOneTeachingSequence.CurrentUnlatched(decreased) < 5,
            "post-latch Crop Tree count decrease cannot drop behind recruitment");

        var lost = Copy(atRecruitment);
        lost.CropTrees = 0;
        lost.Bay = Bay(0);
        Check(ScenarioOneTeachingSequence.Current(lost) == 5
            && ScenarioOneTeachingSequence.CurrentUnlatched(lost) < 5,
            "post-latch Crop Tree loss cannot drop behind recruitment");

        var lostAndClosed = Copy(lost);
        lostAndClosed.AccessOpen = false;
        lostAndClosed.AccessReviewed = false;
        lostAndClosed.Bay = null;
        Check(ScenarioOneTeachingSequence.Current(lostAndClosed) == 5
            && ScenarioOneTeachingSequence.CurrentUnlatched(lostAndClosed) == 0,
            "post-latch guide still ignores a fully regressed access state");

        var laterFelling = Copy(atRecruitment);
        laterFelling.ReleasePlanned = false;
        laterFelling.ReleaseExecuted = false;
        laterFelling.DeadwoodExecuted = false;
        laterFelling.Bay = Bay(3, releasePlanned: false, releaseExecuted: false, mixedExecuted: false, minExecuted: 0f);
        Check(ScenarioOneTeachingSequence.Current(laterFelling) == 5
            && ScenarioOneTeachingSequence.CurrentUnlatched(laterFelling) == 2,
            "post-latch later in-footprint felling cannot drop behind recruitment");

        var shareDrift = Copy(atRecruitment);
        shareDrift.ReleasePlanned = false;
        shareDrift.ReleaseExecuted = false;
        shareDrift.Bay = Bay(3, releasePlanned: false, releaseExecuted: false, mixedExecuted: true,
            minExecuted: .20f, removedShare: .30f);
        Check(ScenarioOneTeachingSequence.Current(shareDrift) == 5
            && ScenarioOneTeachingSequence.CurrentUnlatched(shareDrift) == 2,
            "post-latch executed-share drift cannot drop behind recruitment");

        var sessionOnly = Copy(atRecruitment);
        sessionOnly.RackReviewComplete = false;
        sessionOnly.RevisitedTrees = 0;
        sessionOnly.ReviewDecisions = 0;
        sessionOnly.StandPortionsReviewed = 0;
        Check(ScenarioOneTeachingSequence.Current(sessionOnly) == 5,
            "post-latch session-only review state is not progression authority");

        var reset = new ScenarioOneTeachingSequence.Facts { FirstGuidedRackReviewComplete = true };
        Check(ScenarioOneTeachingSequence.Current(reset) == 5, "post-latch floor holds even when every pre-response fact is false");
    }

    // The latch is a floor, not a freeze: persisted later facts must still drive recruitment -> establishment ->
    // transformation, and drift in earlier facts must not pull those later lessons back.
    private static void LaterFactsStillDrive()
    {
        var atRecruitment = Facts();
        atRecruitment.FirstGuidedRackReviewComplete = true;
        Check(ScenarioOneTeachingSequence.Current(atRecruitment) == 5, "unestablished species stay at recruitment");

        var atEstablishment = Copy(atRecruitment);
        atEstablishment.BeechEstablished = true;
        atEstablishment.OakEstablished = true;
        atEstablishment.PlantingYear = 3;
        atEstablishment.Year = 4;
        atEstablishment.EstablishmentReviewed = false;
        Check(ScenarioOneTeachingSequence.Current(atEstablishment) == 6, "planted species advance the latched guide to establishment");

        var sameYear = Copy(atEstablishment);
        sameYear.Year = 3;
        Check(ScenarioOneTeachingSequence.Current(sameYear) == 6, "planting-year review still waits for a later year");

        var atTransformation = Copy(atEstablishment);
        atTransformation.EstablishmentReviewed = true;
        Check(ScenarioOneTeachingSequence.Current(atTransformation) == 7, "reviewed establishment advances the latched guide to transformation");

        var transformedDrift = Copy(atTransformation);
        transformedDrift.CropTrees = 0;
        transformedDrift.Bay = null;
        transformedDrift.AccessOpen = false;
        transformedDrift.ReleaseExecuted = false;
        transformedDrift.DeadwoodExecuted = false;
        transformedDrift.RackReviewComplete = false;
        Check(ScenarioOneTeachingSequence.Current(transformedDrift) == 7,
            "later current facts keep driving transformation despite earlier-world drift");
    }

    // AC-3 (v): save/load/rebuild at lessons 6-8. The latch is historical; the session review and order/fact cache
    // reset on load, but the persisted later facts still select the same lesson.
    private static void SaveLoadRebuild()
    {
        foreach (int expected in new[] { 5, 6, 7 })
        {
            var saved = Facts();
            saved.FirstGuidedRackReviewComplete = true;
            if (expected >= 6)
            {
                saved.BeechEstablished = true;
                saved.OakEstablished = true;
                saved.PlantingYear = 3;
                saved.Year = 4;
                saved.EstablishmentReviewed = expected == 7;
            }

            var rebuilt = Copy(saved);
            rebuilt.RackReviewComplete = false; // session review is not restored
            rebuilt.RevisitedTrees = 0;
            rebuilt.ReviewDecisions = 0;
            rebuilt.StandPortionsReviewed = 0;
            rebuilt.Bay = Bay(0); // rebuilt after a later Crop Tree loss/drift
            rebuilt.CropTrees = 0;
            Check(ScenarioOneTeachingSequence.Current(saved) == expected
                && ScenarioOneTeachingSequence.Current(rebuilt) == expected,
                "save/load/rebuild at lesson " + (expected + 1) + " keeps the latched guide at the same lesson");
        }

        var missingLatch = ResponseFacts();
        Check(!missingLatch.FirstGuidedRackReviewComplete && ScenarioOneTeachingSequence.Current(missingLatch) == 4,
            "a missing-latch save stays conservative at response until the review is newly completed");
    }

    // Display refresh and repeated reads must be pure: no fact, session review, save field or latch changes.
    private static void RepeatedReadsNeverWrite()
    {
        var f = Facts();
        f.FirstGuidedRackReviewComplete = true;
        string before = Snapshot(f);
        int first = ScenarioOneTeachingSequence.Current(f);
        bool stable = true;
        for (int i = 0; i < 512; i++)
        {
            if (ScenarioOneTeachingSequence.Current(f) != first) stable = false;
            if (ScenarioOneTeachingSequence.CurrentUnlatched(f) != OldAccepted(f)) stable = false;
        }
        Check(stable && first == 5, "repeated reads return the same latched lesson 512 times");
        Check(Snapshot(f) == before, "repeated reads mutate no fact or latch");

        var g = ResponseFacts();
        string gBefore = Snapshot(g);
        ScenarioOneTeachingSequence.CrossesFirstRackReview(g, g);
        Check(Snapshot(g) == gBefore, "transition predicate is read-only");
    }

    private static string Read(string root, string relative) => File.ReadAllText(Path.Combine(root, relative));

    // Source audit for the parts a pure table cannot prove: one shared durable read, no PlayerPrefs or session
    // transcript authority, one live write hook, conservative old-save restore and no persisted guide index.
    private static void SourceWiring(string root)
    {
        string sequence = Read(root, "Assets/ForestPrototype/UI/ScenarioOneTeachingSequence.cs");
        Check(Regex.IsMatch(sequence, @"public static int Current\(Facts f\)\s*=>\s*f\.FirstGuidedRackReviewComplete\s*\?\s*RecruitmentAndLater\(f\)\s*:\s*CurrentUnlatched\(f\);"),
            "Current applies the shared latch as a recruitment floor");
        Check(!Regex.IsMatch(sequence, @"\bPlayerPrefs\b|\bUnityEngine\b|\bKeyboard\b|\bInspectionSerial\b|\bManagementEvents\b|\bScenarioOneRackReview\b|\bFile\.|\bEnvironment\."),
            "teaching sequence is a pure projection of injected facts");
        Check(!Regex.IsMatch(sequence, @"FirstGuidedRackReviewComplete\s*=\s*true"),
            "teaching sequence never writes the latch");
        Check(!sequence.Contains("HistoricalFloor") && !sequence.Contains("ScenarioOneTeachingHistory"),
            "no T-04-only history store or second historical interpretation");
        Check(sequence.Contains("Individual inspection decisions are not stored.")
            && sequence.Contains("Once this first review is complete, the lesson remembers completion after loading."),
            "response copy still says J/K choices are not stored and completion is remembered");

        string view = Read(root, "Assets/ForestPrototype/UI/LearningObjectivesView.Sequence.cs");
        Check(view.Contains("FirstGuidedRackReviewComplete = m.FirstGuidedRackReviewComplete,"),
            "view bridge supplies the single #143 shared historical read");
        Check(Regex.Matches(view, @"\bTryRecordFirstGuidedRackReview\b").Count == 1,
            "exactly one live latch write call site");
        int factsStart = view.IndexOf("private ScenarioOneTeachingSequence.Facts TeachingFacts()", StringComparison.Ordinal);
        int factsEnd = view.IndexOf("public void ObserveRackInspection", factsStart, StringComparison.Ordinal);
        Check(factsStart >= 0 && factsEnd > factsStart
            && !view.Substring(factsStart, factsEnd - factsStart).Contains("TryRecordFirstGuidedRackReview"),
            "facts/display refresh cannot write the latch");
        Check(view.IndexOf("TryRecordFirstGuidedRackReview", StringComparison.Ordinal)
            > view.IndexOf("rackReview.Decide", StringComparison.Ordinal),
            "the latch is offered only after a real session decision");
        Check(Regex.Matches(view, @"ScenarioOneTeachingSequence\.Current\(f\) != 4").Count >= 2,
            "post-latch guide cannot enter or record session review state");
        Check(!Regex.IsMatch(view, @"\bPlayerPrefs\b"), "sequence view reads no PlayerPrefs progression authority");

        string manager = Read(root, "Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.cs");
        Check(manager.Contains("private bool firstGuidedRackReviewComplete;")
            && manager.Contains("public bool FirstGuidedRackReviewComplete => firstGuidedRackReviewComplete;"),
            "manager exposes one read-only durable latch");
        Check(manager.Contains("if (firstGuidedRackReviewComplete || referencePreviewActive"),
            "manager refuses to write an already-true or preview latch");
        Check(manager.Contains("firstGuidedRackReviewComplete = false; // A new forest has its own history."),
            "a new scenario starts with a conservative false latch");
        Check(manager.Contains("firstGuidedRackReviewComplete = saveVersion >= 24 && data.firstGuidedRackReviewComplete;"),
            "old/missing-latch saves restore false; no retrospective reconstruction");
        Check(manager.Contains("firstGuidedRackReviewComplete = firstGuidedRackReviewComplete,"),
            "save capture round-trips the latch");

        string save = Read(root, "Assets/ForestPrototype/ForestSaveData.cs");
        int dtoStart = save.IndexOf("public sealed class ScenarioOneSaveData", StringComparison.Ordinal);
        string dto = dtoStart >= 0 ? save.Substring(dtoStart) : "";
        Check(save.Contains("public const int CurrentVersion = 24;"), "save schema is the integrated v24");
        string[] dtoFields = Regex.Matches(dto, @"^\s*public\s+[A-Za-z0-9_<>\[\],\.]+\s+([A-Za-z_][A-Za-z0-9_]*)\s*(?:=|;)",
            RegexOptions.Multiline).Cast<Match>().Select(match => match.Groups[1].Value).ToArray();
        Check(dtoFields.Count(name => name == "firstGuidedRackReviewComplete") == 1,
            "exactly one persisted teaching latch field");
        Check(!dtoFields.Any(name => Regex.IsMatch(name, @"\b(lesson|guide|step)(Index|Id|State|Progress)?\b",
                RegexOptions.IgnoreCase)), "no persisted guide/lesson index or progress authority");
        Check(!Regex.IsMatch(dto, @"\bPlayerPrefs\b"), "save DTO has no PlayerPrefs authority");

        string archive = Read(root, "Assets/ForestPrototype/ScenarioOne/ScenarioReferenceArchive.cs");
        Check(archive.Contains("firstGuidedRackReviewComplete"),
            "the teaching latch is excluded from reference/world identity projections");
    }
}
