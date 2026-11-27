namespace SitkaP1.Core.NaturalProcesses;

/// <summary>
/// Player- and log-facing text for the natural-process engines. Plain constants, no formatting, so output never depends
/// on culture. Feedback is qualitative only: no hazard percentage or failure probability is ever shown.
/// </summary>
public static class NaturalProcessText
{
    public const string BandStable = "Stable";
    public const string BandWatch = "Watch";
    public const string BandExposed = "Exposed";

    public const string BandExplanation =
        "Storm exposure is a relative model judgement from height, stem slenderness, shelter and recent nearby openings. " +
        "It is not a failure probability. Band thresholds are gameplay calibration.";

    public const string StormsOff = "Storm events are off in this forest.";

    public const string FallGlyphConvention =
        "Fallen-tree glyphs point away from the storm (storm direction + 180 degrees). This is a display convention, " +
        "not a recorded fall bearing for any tree.";

    public const string JayDispersalExplanation =
        "In an oak mast year, jays carry some acorns away from their parent oaks. A carried acorn is only a candidate: " +
        "it still needs suitable ground and light, and it faces bramble, bracken and deer browsing like any other acorn. " +
        "Acorns carried outside the woodland are lost to it.";

    public const string NearbyOakWoodlandExplanation =
        "A nearby oak woodland lies beyond one edge of this forest. In oak mast years jays may bring a few of its acorns in.";

    /// <summary>
    /// Dated teaching note (written 2026-11-27). Market context only: it is not a forecast, not a price model and not a
    /// causal explanation of any simulated outcome.
    /// </summary>
    public const string WindblowMarketTeachingNote =
        "Teaching note (written November 2026; market context, not a forecast or causal model): Storm Darragh " +
        "(December 2024) and Storm Eowyn (January 2025) caused widespread windblow in Irish and British forests. " +
        "Salvage of that windblown timber was part of the 2026 timber market. In this game, salvage prices and the " +
        "contractor minimum are not changed by storms; the note explains real-world context only.";

    public const string RefusalInvalidYear = "year must be 1 or later";
    public const string RefusalInvalidWoodland = "woodland boundary needs at least three finite vertices and a positive area";
    public const string RefusalInvalidEdge = "unknown outer edge";
    public const string RefusalNearbyNeedsJay = "Nearby Oak Woodland needs Jay Acorn Dispersal to be on";
    public const string RefusalUnknownStormModel = "unknown storm model";
    public const string RefusalStormYearResolved = "a storm has already been resolved for this year or a later one";
    public const string RefusalSiteNotNeutral = "StormModel 1 requires the neutral site hazard 1 (D-050)";
    public const string RefusalEdgeNotNeutral = "StormModel 1 requires a neutral or absent external edge (D-050)";
    public const string RefusalNeedsWorkPlan = "salvage by sale or for use needs an approved Work Plan";
    public const string RefusalAlreadyExtracted = "this deadwood has already been extracted";

    public static string BandLabel(StormExposureBand band) => band switch
    {
        StormExposureBand.Stable => BandStable,
        StormExposureBand.Watch => BandWatch,
        StormExposureBand.Exposed => BandExposed,
        _ => throw new ArgumentOutOfRangeException(nameof(band)),
    };
}
