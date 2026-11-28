using SitkaP1.Core.SpeciesWave2;

namespace SitkaP1.Core.CustomWoodlandBuilder.Tests;

internal static class DeerTests
{
    public static void Register()
    {
        TestRunner.Add("Deer", "DEER-01 the four presets map to the exact approved values", ExactValues);
        TestRunner.Add("Deer", "DEER-02 every preset carries [E] calibration provenance", Provenance);
        TestRunner.Add("Deer", "DEER-03 an undefined preset is refused", UndefinedRefused);
    }

    private static void ExactValues()
    {
        TestRunner.Eq(4, DeerBrowsePresets.All.Count, "there are exactly four presets");

        DeerBrowseSetting none = DeerBrowsePresets.Get(DeerBrowsePreset.None);
        DeerBrowseSetting low = DeerBrowsePresets.Get(DeerBrowsePreset.Low);
        DeerBrowseSetting standard = DeerBrowsePresets.Get(DeerBrowsePreset.Standard);
        DeerBrowseSetting heavy = DeerBrowsePresets.Get(DeerBrowsePreset.Heavy);

        TestRunner.Eq(0.0f, none.BackgroundBrowsePressure, "None is 0.0");
        TestRunner.Eq(0.1f, low.BackgroundBrowsePressure, "Low is 0.1");
        TestRunner.Eq(0.2f, standard.BackgroundBrowsePressure, "Standard is 0.2, the existing baseline");
        TestRunner.Eq(0.4f, heavy.BackgroundBrowsePressure, "Heavy is 0.4");

        TestRunner.Eq(DeerBrowsePreset.None, DeerBrowsePresets.All[0].Preset, "preset order starts at None");
        TestRunner.Eq(DeerBrowsePreset.Low, DeerBrowsePresets.All[1].Preset, "then Low");
        TestRunner.Eq(DeerBrowsePreset.Standard, DeerBrowsePresets.All[2].Preset, "then Standard");
        TestRunner.Eq(DeerBrowsePreset.Heavy, DeerBrowsePresets.All[3].Preset, "then Heavy");

        TestRunner.Eq("No deer", none.DisplayName, "None label");
        TestRunner.Eq("Low", low.DisplayName, "Low label");
        TestRunner.Eq("Standard", standard.DisplayName, "Standard label");
        TestRunner.Eq("Heavy", heavy.DisplayName, "Heavy label");

        foreach (DeerBrowseSetting setting in DeerBrowsePresets.All)
        {
            TestRunner.True(setting.BackgroundBrowsePressure >= 0.0f && setting.BackgroundBrowsePressure <= 1.0f,
                "every preset stays inside the existing 0-1 browse pressure validation");
            TestRunner.True(setting.PlayerDescription.Length > 20, "every preset carries a plain-English description");
        }
    }

    private static void Provenance()
    {
        foreach (DeerBrowseSetting setting in DeerBrowsePresets.All)
        {
            TestRunner.Eq(EvidenceClass.Calibration, setting.Provenance.Primary,
                "deer presets are [E] gameplay calibration");
            TestRunner.True(!setting.Provenance.Includes(EvidenceClass.Empirical),
                "no preset claims to be an empirical deer measurement");
            TestRunner.True(
                setting.ProvenanceNote.Contains("Not an Irish deer density threshold", StringComparison.Ordinal),
                "the note refuses the deer-per-km2 reading");
            TestRunner.True(
                setting.ProvenanceNote.Contains("no individual deer are simulated", StringComparison.Ordinal),
                "the note refuses a deer-entity reading");
        }
    }

    private static void UndefinedRefused()
    {
        TestRunner.Throws<ArgumentOutOfRangeException>(
            () => DeerBrowsePresets.Get((DeerBrowsePreset)7), "an undefined preset is refused");

        CustomWoodlandRecipe recipe = Fixtures.Recipe(deer: (DeerBrowsePreset)7);
        TestRunner.True(CustomWoodlandBuilder.Validate(recipe).Count > 0,
            "recipe validation refuses the undefined preset");
        TestRunner.Throws<ArgumentException>(
            () => CustomWoodlandBuilder.Build(recipe), "the build fails closed");
    }
}
