using System.Collections.Generic;

// S1-D T-04 (FR-B1/FR-B2, AC-3): a history-only monotonic floor for the Scenario One guide. Plain data in, plain data out (no
// UnityEngine), so it is checked on the plain .NET SDK. The floor is the earliest lesson the guide may show, proven ONLY from facts
// that persist: the management event log (WorkResolved outcomes, Annual Review acknowledgements, event ids) and the recorded
// objective-achievement years. It is recomputed from scratch on every call, never stored, and never reads PlayerPrefs or the
// session-only [J]/[K] rack review. Where no persisted fact proves completion the floor stays 0 and the existing rules decide.
public static class ScenarioOneTeachingHistory
{
    public enum Kind { Other, Acknowledged, SoldFelling, DeadwoodFelling, BeechPlanted, OakPlanted, OtherPlanted }

    // One persisted management event reduced to what the floor needs. Only WorkResolved events with a Succeeded outcome map to the
    // felling/planting kinds; felling kinds also carry whether the work lay inside the teaching footprint.
    public readonly struct Entry
    {
        public readonly int Id, Year;
        public readonly Kind Kind;
        public readonly bool InFootprint;
        public Entry(int id, int year, Kind kind, bool inFootprint = true) { Id = id; Year = year; Kind = kind; InFootprint = inFootprint; }
    }

    // Lesson indexes (ScenarioOneTeachingSequence.Lessons): the floor takes only these values or 0 (no latch).
    public const int Recruitment = 5, Establishment = 6, Transformation = 7;

    // Event id of the first Annual Review acknowledgement that follows the in-footprint sold AND deadwood felling, or -1. This is the
    // persisted shape of "the release lesson's results have been read"; it needs no Crop Tree, share or [J]/[K] state.
    public static int ReleaseReviewEventId(IReadOnlyList<Entry> events)
    {
        int sold = int.MaxValue, deadwood = int.MaxValue;
        foreach (Entry e in events)
        {
            if (!e.InFootprint) continue;
            if (e.Kind == Kind.SoldFelling && e.Id < sold) sold = e.Id;
            else if (e.Kind == Kind.DeadwoodFelling && e.Id < deadwood) deadwood = e.Id;
        }
        if (sold == int.MaxValue || deadwood == int.MaxValue) return -1;
        int both = System.Math.Max(sold, deadwood), review = int.MaxValue;
        foreach (Entry e in events)
            if (e.Kind == Kind.Acknowledged && e.Id > both && e.Id < review) review = e.Id;
        return review == int.MaxValue ? -1 : review;
    }

    public static int Floor(IReadOnlyList<Entry> events, bool beechRecorded, bool oakRecorded)
    {
        if (events == null) return 0;
        int review = ReleaseReviewEventId(events);
        if (review < 0) return 0;
        int firstPlanting = int.MaxValue, beech = int.MaxValue, oak = int.MaxValue;
        foreach (Entry e in events)
        {
            if (e.Id <= review) continue;
            if (e.Kind == Kind.BeechPlanted || e.Kind == Kind.OakPlanted || e.Kind == Kind.OtherPlanted)
                firstPlanting = System.Math.Min(firstPlanting, e.Id);
            if (e.Kind == Kind.BeechPlanted) beech = System.Math.Min(beech, e.Id);
            if (e.Kind == Kind.OakPlanted) oak = System.Math.Min(oak, e.Id);
        }
        if (firstPlanting == int.MaxValue) return 0;
        // Lesson 6 is under way once planting has been carried out after the release results were read.
        int floor = Recruitment;
        if (beech == int.MaxValue || oak == int.MaxValue || !beechRecorded || !oakRecorded) return floor;
        floor = Establishment;
        // Lesson 7 is complete when a later Annual Review, in a year after every planting resolved before it, was acknowledged. A planting
        // resolved AFTER that review does not undo it: the replay judges each review only against the plantings that preceded it.
        int needed = System.Math.Max(beech, oak);
        foreach (Entry ack in events)
        {
            if (ack.Kind != Kind.Acknowledged || ack.Id <= needed) continue;
            int plantingYear = -1;
            foreach (Entry e in events)
                if (e.Id < ack.Id && e.Id > review && (e.Kind == Kind.BeechPlanted || e.Kind == Kind.OakPlanted || e.Kind == Kind.OtherPlanted)
                    && e.Year > plantingYear) plantingYear = e.Year;
            if (ack.Year > plantingYear) return Transformation;
        }
        return floor;
    }
}
