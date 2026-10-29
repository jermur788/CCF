using System.Globalization;

// Versioned numbers for the Multi-Scale Forestry Management prescription core (MFM-04, D-065). Plain data, no UnityEngine.
//
// Every value here is GAMEPLAY CALIBRATION measured on ONE deterministic stand and ONE rack layout (MFM-03, handoff 60792dc, independent
// review 73356e3). None of them is a forestry standard and none may be shown to the player as one. They are kept in one versioned object so
// a later recalibration is an explicit new version, never a silent change to an old prescription (spec section 7.4).
//
// Protected values are NOT here, because they are not calibration of this feature: the 30% first-release minimum and the 40% cumulative
// basal-area safeguard (ScenarioOneRackBay.FirstReleaseShare / MaxRemovedBasalShare), the 8 m teaching Crop Tree spacing
// (ScenarioOneRackBay.FutureTreeSpacingM) and the 8 m Hegyi neighbourhood (ForestEcologyController.HegyiCutoffMeters).
public sealed class ScenarioOnePrescriptionCalibration
{
    // How a single derived sample is allowed to stand for the comparable areas it covers.
    // OPEN USER QUESTION (MFM-03 section 14 item 3, review section 5): on the default even-aged Sitka stand one sample covers about 13 of 14
    // comparable areas. Whether that is acceptable or the game should require a second sample is a product decision that has NOT been made.
    // The default keeps the approved spec's behaviour (no extra requirement); it is a parameter, not an accepted claim.
    public enum SecondSamplePolicy
    {
        NotRequired,
        // A second sample is reported as required when the covered fraction of non-source areas is above SecondSampleCoveredFractionLimit.
        RequiredAboveCoveredFraction
    }

    // The rules (what is computed and in what order). Changing an algorithm is a new rule version.
    public const string RuleVersion = "MFM-V1-RULE-1";

    // The numbers fed to those rules. Changing any number is a new calibration version.
    public readonly string Version;
    // Each rack-served unit is split along the rack axis at the sample anchor +/- this distance into three areas (MFM-03 "anchor-aligned
    // thirds"). It equals the teaching half-length, so the centre area of the teaching rack is exactly the sample and its corridor.
    public readonly double SegmentHalfLengthM;
    // An area is comparable when each relative difference from the sample is at most this (inclusive). Density is living stems per
    // MASKED NET hectare; competition is the mean authoritative CompetitionShare/Hegyi index. There is deliberately no crowded-tail (P90)
    // guard: MFM-03 tested it and found it not robust.
    public readonly double DensityTolerance, MeanDbhTolerance, MeanCompetitionTolerance;
    // Fewer stems than this and an area is not compared at all.
    public readonly int MinimumStems;
    // Presentation only (spec section 19, "UI fit thresholds"): a compared area whose largest normalised tolerance use ("margin") lies in
    // [BoundaryMarginLow, 1] is a boundary fit and in (1, BoundaryMarginHigh] a boundary fail. These reproduce MFM-03's section 8 example
    // labels on its fixture (R4C 0.68 boundary, R2W 0.44 clear; R1 strip 1.03 boundary, R1W 1.66 clear). They decide no coverage state.
    public readonly double BoundaryMarginLow, BoundaryMarginHigh;
    // Minimum distance between a spacing-only Crop Tree candidate and any Crop Tree or earlier candidate. Its own parameter: it is not the
    // 8 m teaching spacing or the 8 m Hegyi reach. 16 m is fixture-specific (MFM-03 section 10); a real sample sets a different basal-area
    // ceiling, so release feasibility is re-checked by ScenarioOnePrescription.ProposeRelease every time rather than assumed from 16 m.
    public readonly double CandidateSpacingM;
    // Strip width used to measure net areas (corridors, infrastructure masks and the sample removed): midpoint strips along the rack axis,
    // exact intervals across each strip, as MFM-03 measured them (ScenarioOneCoverage Frame.MeasureAreas).
    public readonly double AreaStepM;
    public readonly SecondSamplePolicy SecondSample;
    public readonly double SecondSampleCoveredFractionLimit;

    public ScenarioOnePrescriptionCalibration(string version, double segmentHalfLengthM, double densityTolerance, double meanDbhTolerance,
        double meanCompetitionTolerance, int minimumStems, double boundaryMarginLow, double boundaryMarginHigh, double candidateSpacingM,
        double areaStepM, SecondSamplePolicy secondSample, double secondSampleCoveredFractionLimit)
    {
        Version = version ?? "";
        SegmentHalfLengthM = segmentHalfLengthM;
        DensityTolerance = densityTolerance;
        MeanDbhTolerance = meanDbhTolerance;
        MeanCompetitionTolerance = meanCompetitionTolerance;
        MinimumStems = minimumStems;
        BoundaryMarginLow = boundaryMarginLow;
        BoundaryMarginHigh = boundaryMarginHigh;
        CandidateSpacingM = candidateSpacingM;
        AreaStepM = areaStepM;
        SecondSample = secondSample;
        SecondSampleCoveredFractionLimit = secondSampleCoveredFractionLimit;
    }

    // MFM-03 recommended set, accepted by the Manager as versioned calibration input for MFM-04. Not frozen as production constants until
    // the MFM-03 Unity residual (live post-Lesson-5 state, CI parity, cross-year drift) has passed (review section 6).
    public static readonly ScenarioOnePrescriptionCalibration V1 = new ScenarioOnePrescriptionCalibration(
        "MFM03-CAL-1", ScenarioOneRackBay.HalfLengthM, 0.20, 0.15, 0.20, 30, 0.65, 1.5, 16.0, 0.1,
        SecondSamplePolicy.NotRequired, 1.0);

    public ScenarioOnePrescriptionCalibration WithCandidateSpacing(double spacingM)
        => new ScenarioOnePrescriptionCalibration(Version + "+spacing" + F(spacingM), SegmentHalfLengthM, DensityTolerance, MeanDbhTolerance,
            MeanCompetitionTolerance, MinimumStems, BoundaryMarginLow, BoundaryMarginHigh, spacingM, AreaStepM, SecondSample,
            SecondSampleCoveredFractionLimit);

    public ScenarioOnePrescriptionCalibration WithSecondSample(SecondSamplePolicy policy, double coveredFractionLimit)
        => new ScenarioOnePrescriptionCalibration(Version + "+second" + policy + F(coveredFractionLimit), SegmentHalfLengthM, DensityTolerance,
            MeanDbhTolerance, MeanCompetitionTolerance, MinimumStems, BoundaryMarginLow, BoundaryMarginHigh, CandidateSpacingM, AreaStepM,
            policy, coveredFractionLimit);

    // Every number as exact text, so two prescriptions can be compared and history can say which calibration produced them.
    public string Signature()
        => "rule=" + RuleVersion + " cal=" + Version + " segment=" + F(SegmentHalfLengthM) + " density=" + F(DensityTolerance)
            + " dbh=" + F(MeanDbhTolerance) + " ci=" + F(MeanCompetitionTolerance) + " tailGuard=none minStems=" + MinimumStems
            + " boundary=[" + F(BoundaryMarginLow) + "," + F(BoundaryMarginHigh) + "] candidateSpacing=" + F(CandidateSpacingM)
            + " areaStep=" + F(AreaStepM) + " secondSample=" + SecondSample + ":" + F(SecondSampleCoveredFractionLimit);

    private static string F(double v) => v.ToString("R", CultureInfo.InvariantCulture);
}
