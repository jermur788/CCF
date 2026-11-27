namespace SitkaP1.Core.SpeciesWave2.BirchAlder;

/// <param name="SpeciesId">Downy birch or Common alder.</param>
/// <param name="StemSeed">Stable per-stem identity used only for the deterministic alder heart-rot inspection.</param>
/// <param name="VolumeM3">Harvested stem volume in cubic metres.</param>
/// <param name="IsMature">Whether the stem has reached harvest maturity.</param>
/// <param name="DbhCm">Stem diameter at breast height in centimetres.</param>
/// <param name="HasQualityStemForm">
/// Straight and sound for birch; a well-released clean bole for alder. Caller-supplied, because Part 1 has no
/// form-damage model.
/// </param>
/// <param name="StandAgeYears">Stand age at felling; drives the alder heart-rot inspection only.</param>
public readonly record struct BirchAlderTimberRequest(
    string SpeciesId,
    ulong StemSeed,
    decimal VolumeM3,
    bool IsMature,
    double DbhCm,
    bool HasQualityStemForm,
    int StandAgeYears);

/// <summary>
/// Graded result. A quality stem keeps the bulk price and is marked unpriced rather than being given a placeholder
/// premium: there is no local specialist buyer in normal play, and the game says so.
/// </summary>
public readonly record struct BirchAlderTimberResult(
    string SpeciesId,
    Sortment Sortment,
    decimal UnitPriceEurPerTonne,
    ValueBand PriceBandEurPerTonne,
    decimal VolumeM3,
    decimal GreenTonnes,
    decimal GrossValueEur,
    bool QualityStemUnpriced,
    bool HeartRotDowngrade,
    string? BuyerNote,
    string Reason)
{
    public EvidenceTag Evidence => EvidenceTag.Of(EvidenceClass.Calibration);
}

public static partial class BirchAlderEngine
{
    // [Calibration] One provisional teaching bulk band for both species: the midpoint of the archived EUR 25-60/t
    // sensitivity range. Never a Sitka borrowing.
    public const decimal BulkPriceEurPerTonne = 42m;

    // [Calibration/Transfer] Sapling material costs (material only, no planting labour).
    public const decimal BirchSaplingMaterialCostEurPerTree = 0.95m;  // [E] None-So Hardy 2025-26 wholesale
    public const decimal AlderSaplingMaterialCostEurPerTree = 0.95m;   // [C] provisional broadleaf parity

    // [Calibration] Quality-stem inspection triggers. Birch 30-35+ cm comes from the card's band; the alder card leaves
    // this field null, so the owner-locked "toward 50-60 cm" teaching trigger is declared here.
    public const double BirchQualityStemMinimumDbhCm = 30.0;
    public const double AlderQualityStemMinimumDbhCm = 50.0;

    public const string QualityStemBuyerNote = "Quality stem — no local specialist buyer";

    // [Calibration] Alder heart rot: onset at 50, maximum risk from 60. Flags a downgrade only; it never kills a tree.
    public const int AlderHeartRotOnsetYears = 50;
    public const int AlderHeartRotFullRiskYears = 60;
    public const double AlderHeartRotMaximumProbability = 0.35;

    private const ulong AlderHeartRotSalt = 0xE2B7A1D5C93F640BUL;

    public static decimal SaplingMaterialCostEurPerTree(string speciesId) =>
        BirchAlderSpecies.RequireSupported(speciesId, nameof(speciesId)) == BirchAlderSpecies.DownyBirch
            ? BirchSaplingMaterialCostEurPerTree
            : AlderSaplingMaterialCostEurPerTree;

    /// <summary>The card's own green density: 0.93 t/m3 for Downy birch, 0.842 t/m3 for Common alder.</summary>
    public static decimal GreenDensityTPerM3(string speciesId) =>
        (decimal)Profile(speciesId).Timber.GreenDensityTPerM3;

    public static decimal SaplingMaterialCostEur(string speciesId, int treeCount)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(treeCount);
        return treeCount * SaplingMaterialCostEurPerTree(speciesId);
    }

    /// <summary>
    /// [Calibration] Rising deterministic downgrade risk: 0 before 50 years, linear to the maximum at 60, then flat.
    /// </summary>
    public static double AlderHeartRotRiskProbability(int standAgeYears)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(standAgeYears);
        if (standAgeYears < AlderHeartRotOnsetYears)
            return 0.0;
        if (standAgeYears >= AlderHeartRotFullRiskYears)
            return AlderHeartRotMaximumProbability;

        double progress = (standAgeYears - AlderHeartRotOnsetYears) /
                          (double)(AlderHeartRotFullRiskYears - AlderHeartRotOnsetYears);
        return AlderHeartRotMaximumProbability * progress;
    }

    /// <summary>
    /// A pure per-stem inspection roll under a dedicated salt. The same stem identity and age always give the same
    /// answer, in any call order, with no RNG stream consumed. Before age 50 the roll is never a downgrade.
    /// </summary>
    public static bool InspectAlderHeartRot(ulong stemSeed, int standAgeYears)
    {
        double risk = AlderHeartRotRiskProbability(standAgeYears);
        if (risk <= 0.0)
            return false;

        ulong mixed = SplitMix64(stemSeed ^ ((ulong)standAgeYears * 0x9E3779B97F4A7C15UL), AlderHeartRotSalt);
        double roll = (mixed >> 11) * (1.0 / 9007199254740992.0); // uniform on [0, 1)
        return roll < risk;
    }

    public static BirchAlderTimberResult GradeTimber(BirchAlderTimberRequest request)
    {
        string speciesId = BirchAlderSpecies.RequireSupported(request.SpeciesId, nameof(request));
        ArgumentOutOfRangeException.ThrowIfNegative(request.VolumeM3);
        ArgumentOutOfRangeException.ThrowIfNegative(request.StandAgeYears);
        if (!double.IsFinite(request.DbhCm) || request.DbhCm < 0.0)
            throw new ArgumentOutOfRangeException(nameof(request), "DBH must be finite and non-negative.");

        bool birch = speciesId == BirchAlderSpecies.DownyBirch;
        bool heartRot = !birch && InspectAlderHeartRot(request.StemSeed, request.StandAgeYears);
        double minimumQualityDbh = birch ? BirchQualityStemMinimumDbhCm : AlderQualityStemMinimumDbhCm;

        Sortment sortment;
        bool qualityUnpriced = false;
        string? buyerNote = null;
        string reason;

        if (heartRot)
        {
            sortment = Sortment.PulpChip;
            reason = $"Common alder heart-rot inspection downgraded the stem to bulk firewood/pulp at age {request.StandAgeYears}.";
        }
        else if (request.IsMature && request.HasQualityStemForm && request.DbhCm >= minimumQualityDbh)
        {
            sortment = Sortment.Bulk;
            qualityUnpriced = true;
            buyerNote = QualityStemBuyerNote;
            reason = birch
                ? "Straight, sound Downy birch quality stem: bulk value only until a specialist buyer appears."
                : "Well-released Common alder clean bole: bulk value only until a specialist buyer appears.";
        }
        else
        {
            sortment = Sortment.Bulk;
            reason = request.IsMature
                ? "Mature stem did not reach the quality-stem trigger: bulk firewood/pulp."
                : "Immature stem: bulk firewood/pulp value only.";
        }

        ValueBand band = Profile(speciesId).Timber.SortmentBands.Single(entry => entry.Sortment == Sortment.Bulk).EurPerTonne;
        decimal greenTonnes = request.VolumeM3 * GreenDensityTPerM3(speciesId);
        return new BirchAlderTimberResult(
            speciesId,
            sortment,
            BulkPriceEurPerTonne,
            band,
            request.VolumeM3,
            greenTonnes,
            greenTonnes * BulkPriceEurPerTonne,
            qualityUnpriced,
            heartRot,
            buyerNote,
            reason);
    }
}
