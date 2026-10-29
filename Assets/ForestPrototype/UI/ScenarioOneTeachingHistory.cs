using System.Collections.Generic;

// S1-D T-04 (FR-B1/FR-B2, AC-3): a history-only monotonic floor for the Scenario One guide, plus the ONE definition of "the teaching
// sample is historically complete" that T-03's cap and the guide share. Plain data in, plain data out (no UnityEngine).
//
// Only boundaries that persisted facts faithfully prove are latched: lesson 6 (planted beech and sessile oak established) and lesson 7
// (the Annual Review after the planting year was acknowledged). The proof is the recorded objective-achievement years plus the
// management event log. Lessons 1-5 are deliberately NOT latched: Crop Tree choice, the Crop-Tree-serving release relation and the
// [J]/[K] review are not persisted, and ordinary actions such as felling or planting are not lesson-gated, so they prove nothing.
// The floor is recomputed on every call, never stored, and never reads PlayerPrefs or session review state.
public static class ScenarioOneTeachingHistory
{
    public enum Kind { Other, Acknowledged, BeechPlanted, OakPlanted, OtherPlanted }

    // One persisted management event: Annual Review acknowledgements and WorkResolved/Succeeded plantings only.
    public readonly struct Entry
    {
        public readonly int Id, Year;
        public readonly Kind Kind;
        public Entry(int id, int year, Kind kind) { Id = id; Year = year; Kind = kind; }
    }

    // Lesson indexes (ScenarioOneTeachingSequence.Lessons). The floor is 0 (no latch) or one of these.
    public const int Recruitment = 5, Establishment = 6, Transformation = 7;

    // The earliest lesson index the guide may show, valid only while the lessons before Recruitment currently hold
    // (ScenarioOneTeachingSequence.Current applies it only then): it keeps lessons 7 and 8 from being lost to a later planting
    // (which moves the planting year), a lost seedling, or a reload, none of which un-establishes what was observed.
    public static int Floor(IReadOnlyList<Entry> events, bool beechRecorded, bool oakRecorded)
    {
        if (events == null || !beechRecorded || !oakRecorded) return 0;
        int beech = int.MaxValue, oak = int.MaxValue;
        foreach (Entry e in events)
        {
            if (e.Kind == Kind.BeechPlanted && e.Id < beech) beech = e.Id;
            if (e.Kind == Kind.OakPlanted && e.Id < oak) oak = e.Id;
        }
        if (beech == int.MaxValue || oak == int.MaxValue) return 0;
        // A review counts only against the plantings that preceded it, so a later planting never undoes it.
        int needed = System.Math.Max(beech, oak);
        foreach (Entry ack in events)
        {
            if (ack.Kind != Kind.Acknowledged || ack.Id <= needed) continue;
            int plantingYear = -1;
            foreach (Entry e in events)
                if (e.Id < ack.Id && (e.Kind == Kind.BeechPlanted || e.Kind == Kind.OakPlanted || e.Kind == Kind.OtherPlanted) && e.Year > plantingYear)
                    plantingYear = e.Year;
            if (ack.Year > plantingYear) return Transformation;
        }
        return Establishment;
    }

    // THE shared definition (T-03 and the guide): the executed in-footprint release served the retained trees with both sold and
    // deadwood outcomes, and the Annual Review after that work was acknowledged. The cap is deliberately not an input. The guide's
    // rules at Recruitment or later require exactly these facts, so the guide being past lesson 5 implies this is true.
    public static bool SampleHistoricallyComplete(ScenarioOneRackBay.Analysis bay, bool releaseReviewed)
        => bay != null && bay.Applies && releaseReviewed && bay.ReleaseYear >= 0 && bay.MixedExecuted
           && bay.MinExecuted >= ScenarioOneRackBay.FirstReleaseShare * ScenarioOneRackBay.ExecutedTolerance;
}
