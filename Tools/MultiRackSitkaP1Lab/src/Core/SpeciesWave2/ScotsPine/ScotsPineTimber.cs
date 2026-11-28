namespace SitkaP1.Core.SpeciesWave2.ScotsPine;

/// <param name="VolumeM3">Harvested stem volume in cubic metres.</param>
/// <param name="IsMature">Whether the stem has reached harvest maturity.</param>
/// <param name="PrunedHeightM">Recorded clear-bole pruning height in metres, or zero when unpruned.</param>
/// <param name="HadTightEarlyStocking">Whether early stand history supports the tight-stocking quality route.</param>
/// <param name="SuitableForSmallRoundwood">Whether the stem can use the Small Roundwood fallback.</param>
public readonly record struct ScotsPineTimberRequest(
    decimal VolumeM3,
    bool IsMature,
    double PrunedHeightM,
    bool HadTightEarlyStocking,
    bool SuitableForSmallRoundwood);

public readonly record struct ScotsPineTimberResult(
    Sortment Sortment,
    decimal UnitPriceEurPerTonne,
    ValueBand PriceBandEurPerTonne,
    decimal VolumeM3,
    decimal GreenTonnes,
    decimal GrossValueEur,
    string Reason)
{
    public EvidenceTag Evidence => EvidenceTag.Of(EvidenceClass.Calibration);
}

public static partial class ScotsPineEngine
{
    // [Calibration] Provisional teaching prices in EUR per green tonne and material cost in EUR per sapling.
    public const decimal SawlogPriceEurPerTonne = 68m;
    public const decimal SmallRoundwoodPriceEurPerTonne = 45m;
    public const decimal PulpChipPriceEurPerTonne = 38m;
    public const decimal SaplingMaterialCostEurPerTree = 0.45m;

    // [Calibration] Use the species-specific card density (1.02 green t/m³) as the teaching conversion.
    public static decimal GreenDensityTPerM3 => (decimal)Profile.Timber.GreenDensityTPerM3;

    public static ScotsPineTimberResult GradeTimber(ScotsPineTimberRequest request)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(request.VolumeM3);
        if (!double.IsFinite(request.PrunedHeightM) || request.PrunedHeightM < 0.0)
            throw new ArgumentOutOfRangeException(nameof(request), "Pruning height must be finite and non-negative.");

        Sortment sortment;
        decimal price;
        string reason;
        // [Calibration] Reaching the lower end of the clear-bole target qualifies the mature stem.
        if (request.IsMature && Profile.Silviculture.PruningTargetHeightM is { } target &&
            request.PrunedHeightM >= target.Low)
        {
            sortment = Sortment.Sawlog;
            price = SawlogPriceEurPerTonne;
            reason = "Mature stem reached the clear-bole pruning target.";
        }
        else if (request.IsMature && request.HadTightEarlyStocking)
        {
            sortment = Sortment.Sawlog;
            price = SawlogPriceEurPerTonne;
            reason = "Mature stem grew under tight early stocking.";
        }
        else
        {
            sortment = request.SuitableForSmallRoundwood ? Sortment.SmallRoundwood : Sortment.PulpChip;
            price = request.SuitableForSmallRoundwood ? SmallRoundwoodPriceEurPerTonne : PulpChipPriceEurPerTonne;
            string sawlogReason = request.IsMature
                ? "The mature stem missed the pruning target and grew without tight early stocking."
                : "The stem has yet to reach harvest maturity.";
            string fallbackReason = request.SuitableForSmallRoundwood
                ? "It suits small roundwood."
                : "It is suitable for pulp or chips.";
            reason = sawlogReason + " " + fallbackReason;
        }

        ValueBand band = Profile.Timber.SortmentBands.Single(entry => entry.Sortment == sortment).EurPerTonne;
        decimal greenTonnes = request.VolumeM3 * GreenDensityTPerM3;
        return new ScotsPineTimberResult(
            sortment, price, band, request.VolumeM3, greenTonnes, greenTonnes * price, reason);
    }
}
