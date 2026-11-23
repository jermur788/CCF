using SitkaP1.Core;

namespace SitkaP1.Core.Tests;

/// <summary>
/// Multi Stage 4 (D-095): adult Sitka parity, natural mortality with fallen deadwood, and pruning, behind
/// AdultModelVersion 1 + MortalityModelVersion 1 + CrownModelVersion 1. Every other selection must stay byte-identical.
/// </summary>
internal static class Stage4Tests
{
    // Printed by crown C2-03 on base 4e60c8ab (Stage 1–3 HEAD) before any Stage 4 change.
    internal const string PinnedStage3CrownModel1Hash = "814e81650b62428510fe60c3142b51d9055537d6014303074f08733bf8d46287";

    internal static void Register()
    {
        T.Add("S4 V-01: default and explicit model 0 selections reproduce the pinned model 0 MA-1 fingerprint", () =>
        {
            T.Eq(MultiAreaModelSelection.Legacy, new MultiAreaContinuation(MultiAreaGenerator.Generate()).SelectedModels, "default selection");
            T.Eq(Model0FingerprintTests.PinnedMultiAreaHash, Fingerprint(MultiAreaModelSelection.Legacy), "explicit 0/0/0");
        });

        T.Add("S4 V-02: Stage 1-3 CrownModel 1 (adult 0, mortality 0) reproduces its pinned base fingerprint", () =>
        {
            T.Eq(PinnedStage3CrownModel1Hash, Model0FingerprintTests.MultiAreaFingerprint(CrownModelVersion.AreaScaledEquilibriumC2), "crown-only ctor");
            T.Eq(PinnedStage3CrownModel1Hash, Fingerprint(MultiAreaModelSelection.Stage3CrownC2), "explicit 0/0/1");
        });

        T.Add("S4 V-03: partial or unknown Stage 4 selections are refused; Stage 4 options need Stage 4", () =>
        {
            var e = MultiAreaGenerator.Generate();
            T.Refuses("Stage4ModelIncomplete", () => new MultiAreaContinuation(e,
                new MultiAreaModelSelection(AdultModelVersion.ConnectedWoodland, MortalityModelVersion.Legacy, CrownModelVersion.AreaScaledEquilibriumC2)));
            T.Refuses("Stage4ModelIncomplete", () => new MultiAreaContinuation(e,
                new MultiAreaModelSelection(AdultModelVersion.Legacy, MortalityModelVersion.RecordedDeadwood, CrownModelVersion.AreaScaledEquilibriumC2)));
            T.Refuses("Stage4ModelIncomplete", () => new MultiAreaContinuation(e,
                new MultiAreaModelSelection(AdultModelVersion.ConnectedWoodland, MortalityModelVersion.RecordedDeadwood, CrownModelVersion.LegacyFrozen)));
            T.Refuses("AdultModelUnknown", () => new MultiAreaContinuation(e,
                new MultiAreaModelSelection((AdultModelVersion)7, MortalityModelVersion.Legacy, CrownModelVersion.LegacyFrozen)));
            T.Refuses("CrownModelUnknown", () => new MultiAreaContinuation(e,
                new MultiAreaModelSelection(AdultModelVersion.Legacy, MortalityModelVersion.Legacy, (CrownModelVersion)9)));
            T.Refuses("Stage4ModelRequired", () => new MultiAreaContinuation(e, MultiAreaModelSelection.Stage3CrownC2, Stage4Options.Default));
            var s4 = new MultiAreaContinuation(e, MultiAreaModelSelection.Stage4);
            T.True(s4.SelectedModels.IsStage4, "full Stage 4 accepted");
            T.True(s4.ReadView().Source.AreaModelVersion != new MultiAreaContinuation(e).ReadView().Source.AreaModelVersion,
                "Stage 4 has its own area-model lineage in the staleness token");
            T.Eq(new MultiAreaContinuation(e).ReadView().Experiment.ManifestContentHash, s4.ReadView().Experiment.ManifestContentHash,
                "experiment manifest/content hash unchanged");
        });

        Stage4BehaviourTests.Register();
    }

    /// <summary>The model-0 fingerprint recipe (fixed Fells in years 0 and 3, ten years) on any explicit selection.</summary>
    internal static string Fingerprint(MultiAreaModelSelection models, Stage4Options? options = null)
    {
        var c = new MultiAreaContinuation(MultiAreaGenerator.Generate(), models, options);
        return Model0FingerprintTests.MultiAreaFingerprintOf(c);
    }
}
