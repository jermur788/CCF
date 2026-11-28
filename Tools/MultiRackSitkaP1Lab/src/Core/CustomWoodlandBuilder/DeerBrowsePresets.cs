using SitkaP1.Core.SpeciesWave2;

namespace SitkaP1.Core.CustomWoodlandBuilder;

/// <summary>Plain-English background deer browsing settings. Values feed the existing aggregate browse pressure.</summary>
public enum DeerBrowsePreset
{
    /// <summary>0.0 — no deer browsing pressure.</summary>
    None,

    /// <summary>0.1 — light deer browsing.</summary>
    Low,

    /// <summary>0.2 — standard deer browsing; the existing baseline value.</summary>
    Standard,

    /// <summary>0.4 — heavy deer browsing.</summary>
    Heavy,
}

/// <summary>
/// One resolved deer preset: the closed preset, the exact float the simulation already accepts, a plain-English
/// label and its provenance.
/// </summary>
public sealed record DeerBrowseSetting(
    DeerBrowsePreset Preset,
    float BackgroundBrowsePressure,
    string DisplayName,
    string PlayerDescription)
{
    /// <summary>
    /// `[E]` gameplay calibration. These are four named settings for one aggregate pressure value; they are not Irish
    /// deer-per-square-kilometre density thresholds, and no deer entity is created or simulated.
    /// </summary>
    public EvidenceTag Provenance => EvidenceTag.Of(EvidenceClass.Calibration);

    public string ProvenanceNote =>
        "[E] Gameplay calibration. Not an Irish deer density threshold; no individual deer are simulated.";
}

/// <summary>
/// The four approved deer browsing presets and nothing else. The numeric values match the existing
/// <c>backgroundBrowsePressure</c> field, whose baseline is <c>Standard = 0.2</c>.
/// </summary>
public static class DeerBrowsePresets
{
    public const float NoBrowsingPressure = 0.0f;
    public const float LowBrowsingPressure = 0.1f;
    public const float StandardBrowsingPressure = 0.2f;
    public const float HeavyBrowsingPressure = 0.4f;

    private static readonly DeerBrowseSetting NoneSetting = new(
        DeerBrowsePreset.None,
        NoBrowsingPressure,
        "No deer",
        "Deer browsing is switched off. Young trees establish without browsing pressure.");

    private static readonly DeerBrowseSetting LowSetting = new(
        DeerBrowsePreset.Low,
        LowBrowsingPressure,
        "Low",
        "Light browsing. Some young trees will be checked, but most species establish.");

    private static readonly DeerBrowseSetting StandardSetting = new(
        DeerBrowsePreset.Standard,
        StandardBrowsingPressure,
        "Standard",
        "The usual browsing pressure for this simulator; the existing baseline.");

    private static readonly DeerBrowseSetting HeavySetting = new(
        DeerBrowsePreset.Heavy,
        HeavyBrowsingPressure,
        "Heavy",
        "Heavy browsing. Palatable broadleaves will struggle to establish without protection.");

    public static IReadOnlyList<DeerBrowseSetting> All { get; } = Array.AsReadOnly(new[]
    {
        NoneSetting, LowSetting, StandardSetting, HeavySetting,
    });

    /// <summary>Resolves a preset. An undefined enum value is refused rather than mapped to a default.</summary>
    public static DeerBrowseSetting Get(DeerBrowsePreset preset)
    {
        if (!Enum.IsDefined(preset))
            throw new ArgumentOutOfRangeException(nameof(preset), preset, "unknown deer browsing preset.");

        return preset switch
        {
            DeerBrowsePreset.None => NoneSetting,
            DeerBrowsePreset.Low => LowSetting,
            DeerBrowsePreset.Standard => StandardSetting,
            DeerBrowsePreset.Heavy => HeavySetting,
            _ => throw new ArgumentOutOfRangeException(nameof(preset), preset, "unknown deer browsing preset."),
        };
    }
}
