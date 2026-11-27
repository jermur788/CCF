using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using SitkaP1.Core;

namespace SitkaP1.Ui;

internal sealed partial class PlayerApi
{
    // GET api/multi/woodland/options — returns Core-authoritative built-in stands, presets, display-only bounds,
    // and current active session state so the New Woodland dialog invents zero forestry constants or bounds (D-020).
    private Task ReadMultiWoodlandOptionsAsync(HttpContext context)
    {
        if (context.Request.Query.Count > 0)
            return WriteErrorAsync(context, StatusCodes.Status400BadRequest, "BadRequest",
                "woodland options does not accept query parameters; nothing changed");

        MultiWoodlandCurrentDto currentDto;
        lock (multiHost.SyncLock)
        {
            var active = multiHost.Current;
            var view = active.Adapter.Read();
            currentDto = new MultiWoodlandCurrentDto(
                active.ExperimentKey,
                view.Experiment.ExperimentId,
                active.Badge,
                view.Areas.Count,
                active.Recipe is not null,
                active.Session.SelectedModels.IsEconomyEnabled,
                active.HasUnsavedProgress,
                active.Recipe is null ? null : ProjectRecipeDto(active.Recipe));
        }

        var builtIns = new[]
        {
            new MultiWoodlandBuiltInDto(MultiAreaDemoExperiments.Ma1Key, "MA-1 (2-area pure Sitka)", MultiAreaDemoExperiments.Ma1Badge),
            new MultiWoodlandBuiltInDto(MultiAreaDemoExperiments.Ma1MixedKey, "MA-1 Mixed (2-area Sitka + Oak/Beech)", MultiAreaDemoExperiments.Ma1MixedBadge),
        };

        var presets = ForestMakerPresets.All.Select(preset =>
        {
            var generated = ForestMakerGenerator.Generate(preset);
            string title = preset.PresetId == ForestMakerPresets.PrimaryPresetId
                ? "0.64 ha Sitka + edge broadleaves (5 lanes)"
                : "0.64 ha pure Sitka control (5 lanes)";
            return new MultiWoodlandPresetDto(
                preset.PresetId!,
                title,
                generated.Experiment.BadgeOverride ?? ForestMakerPresets.BadgeFor(preset, generated.TotalAreaHa),
                generated.Experiment.ExperimentId,
                generated.TotalStems,
                generated.TotalAreaHa,
                ComputeGroundCellCount(preset),
                ProjectRecipeDto(preset));
        }).ToArray();

        var bounds = new MultiWoodlandBoundsDto(
            ForestMakerBounds.GeneratorId,
            ForestMakerBounds.GeneratorVersion,
            ForestMakerBounds.MinSideM,
            ForestMakerBounds.MaxSideM,
            ForestMakerBounds.MinSideM,
            ForestMakerBounds.MaxSideM,
            ForestMakerBounds.MinLanes,
            ForestMakerBounds.MaxLanes,
            ForestMakerBounds.MinAgeYears,
            ForestMakerBounds.MaxAgeYears,
            ForestMakerBounds.MinStemsPerHa,
            ForestMakerBounds.MaxStemsPerHa,
            ForestMakerBounds.MaxLivingStems,
            new[]
            {
                new MultiWoodlandSpeciesMixOptionDto("SitkaPlusEdgeBroadleaves", "Sitka + edge broadleaves"),
                new MultiWoodlandSpeciesMixOptionDto("PureSitka", "Pure Sitka"),
                new MultiWoodlandSpeciesMixOptionDto("CustomSplit", "Custom species split"),
            });

        return WriteJsonAsync(context, StatusCodes.Status200OK, new MultiWoodlandOptionsPayload(
            builtIns,
            presets,
            bounds,
            ForestMakerBounds.Disclosure,
            currentDto));
    }

    // POST api/multi/woodland/preview — pure read-only Core validation & generation preview. Zero JS math, zero state mutation.
    private async Task PreviewMultiWoodlandAsync(HttpContext context)
    {
        var request = await ReadMultiRequestAsync<MultiWoodlandPreviewRequest>(context, MultiWoodlandPreviewMembers);
        if (request is null) return;
        try
        {
            var recipe = BuildRecipeFromInput(request.Recipe!, forceCustomPresetNull: false);
            var generated = ForestMakerGenerator.Generate(recipe);
            var lanes = generated.Lanes.Select(l => new MultiWoodlandPreviewLaneDto(
                l.AreaId, l.Label, l.AreaHa, l.TotalStems, l.SitkaCount, l.OakCount, l.BeechCount)).ToArray();
            var preview = new MultiWoodlandPreviewDto(
                generated.Experiment.ExperimentId,
                generated.RecipeHash,
                generated.ContentHash,
                generated.Experiment.BadgeOverride ?? ForestMakerPresets.BadgeFor(generated.Recipe, generated.TotalAreaHa),
                generated.TotalStems,
                generated.TotalAreaHa,
                ComputeGroundCellCount(generated.Recipe),
                generated.Recipe.WidthM,
                generated.Recipe.LengthM,
                generated.Recipe.LaneCount,
                ForestMakerRecipe.SpeciesMixWire(generated.Recipe.SpeciesMix),
                lanes,
                ForestMakerBounds.Disclosure,
                ForestMakerBounds.GeneratorNote,
                ProjectRecipeDto(generated.Recipe));
            await WriteJsonAsync(context, StatusCodes.Status200OK, new MultiWoodlandPreviewPayload(true, preview, null));
        }
        catch (P1Refusal refusal)
        {
            await WriteJsonAsync(context, StatusCodes.Status409Conflict,
                new MultiWoodlandPreviewPayload(false, null, new Notice(refusal.Code, refusal.Message)));
        }
    }

    // POST api/multi/woodland/new — fenced, unsaved-progress-guarded atomic host replacement.
    // On any refusal (stale fence, unconfirmed unsaved work, invalid custom recipe), old session stays untouched.
    private async Task NewMultiWoodlandAsync(HttpContext context)
    {
        var request = await ReadMultiRequestAsync<MultiWoodlandNewRequest>(context, MultiWoodlandNewMembers);
        if (request is null) return;

        Notice? refusal = null;
        MultiAreaCompositeUiSnapshot outSnap;
        int totalRecordedYears;
        MultiReviewPreviewDto? reviewPreview;
        bool economyEnabled;
        MultiEconomySnapshotDto? economy;
        bool hasUnsavedProgress;
        string experimentBadge;
        MultiWoodlandRecipeDto? activeRecipe;
        int genesisStems;

        try
        {
            multiHost.AtomicReplace(request.Fence!, request.ConfirmDiscardUnsaved, _ =>
            {
                string choice = (request.Choice ?? "").Trim();
                return choice switch
                {
                    "ma-1" => MultiSessionHost.BuildBuiltIn(MultiAreaDemoExperiments.Ma1Key, request.EconomyEnabled),
                    "ma-1-mixed" => MultiSessionHost.BuildBuiltIn(MultiAreaDemoExperiments.Ma1MixedKey, request.EconomyEnabled),
                    "preset" => MultiSessionHost.BuildForestMaker(
                        ForestMakerPresets.ByPresetId(request.PresetId ?? ""),
                        request.EconomyEnabled),
                    "custom" => MultiSessionHost.BuildForestMaker(
                        BuildRecipeFromInput(request.Recipe ?? throw new P1Refusal("ForestMakerRecipeRequired", "custom woodland requires a recipe"), forceCustomPresetNull: true),
                        request.EconomyEnabled),
                    _ => throw new P1Refusal("WoodlandChoiceInvalid", "unsupported woodland choice: " + choice),
                };
            });
        }
        catch (P1Refusal r)
        {
            refusal = new Notice(r.Code, r.Message);
        }

        lock (multiHost.SyncLock)
        {
            var active = multiHost.Current;
            outSnap = active.Adapter.ReadCompositeUiSnapshot();
            totalRecordedYears = active.Session.ReadHistoryIndex(pageSize: 1).TotalRecordedYears;
            reviewPreview = TryPreviewForView(active, outSnap.View);
            economyEnabled = active.Session.SelectedModels.IsEconomyEnabled;
            economy = TryBuildEconomySnapshot(active.Session, outSnap.View);
            hasUnsavedProgress = active.HasUnsavedProgress;
            experimentBadge = active.Badge;
            activeRecipe = active.Recipe is null ? null : ProjectRecipeDto(active.Recipe);
            genesisStems = active.Session.GenesisStemCount;
        }

        var budget = ActiveStandBudget(outSnap.View.Areas.Count, genesisStems);
        var bounded = SerializeBoundedMultiView(outSnap, totalRecordedYears, reviewPreview, out byte[] rawViewBytes,
            economyEnabled, economy, budget, hasUnsavedProgress, activeRecipe);
        await WriteBoundedJsonAsync(context, StatusOf(refusal?.Code), new MultiWoodlandNewPayload(
            bounded.View, bounded.Scenes, MultiLabels, bounded.Stage4, bounded.Ecology,
            refusal, bounded.HistoryTruncated, bounded.TotalRecordedYears, reviewPreview,
            bounded.ProjectionCompacted, bounded.Stage4Compacted, bounded.PayloadBudgetExceeded,
            economyEnabled, economy, hasUnsavedProgress, experimentBadge, activeRecipe),
            budget, bounded.PayloadBudgetExceeded, rawViewBytes.Length);
    }

    private static ForestMakerRecipe BuildRecipeFromInput(MultiWoodlandRecipeInput input, bool forceCustomPresetNull)
    {
        if (input.WidthM is not int widthM || input.LengthM is not int lengthM ||
            input.LaneCount is not int laneCount || input.StartAgeYears is not int startAgeYears ||
            input.StemsPerHa is not int stemsPerHa || input.Seed is not int seed ||
            string.IsNullOrWhiteSpace(input.SpeciesMix))
        {
            throw new P1Refusal("ForestMakerRecipeIncomplete",
                "recipe requires integer widthM, lengthM, laneCount, startAgeYears, stemsPerHa, seed and speciesMix");
        }
        string generatorId = string.IsNullOrWhiteSpace(input.GeneratorId) ? ForestMakerBounds.GeneratorId : input.GeneratorId;
        int generatorVersion = input.GeneratorVersion ?? ForestMakerBounds.GeneratorVersion;
        string? presetId = forceCustomPresetNull ? null : (string.IsNullOrWhiteSpace(input.PresetId) ? null : input.PresetId);
        var mix = ForestMakerRecipe.ParseSpeciesMix(input.SpeciesMix);
        int oakPct = input.OakPercent ?? 0;
        int beechPct = input.BeechPercent ?? 0;
        var recipe = new ForestMakerRecipe(generatorId, generatorVersion, presetId, widthM, lengthM, laneCount,
            startAgeYears, stemsPerHa, seed, mix, oakPct, beechPct);
        return recipe.RequireValid();
    }

    private static int ComputeGroundCellCount(ForestMakerRecipe recipe)
    {
        long cols = (recipe.WidthMm + 2_499L) / 2_500L;
        long rows = (recipe.LengthMm + 2_499L) / 2_500L;
        return checked((int)(cols * rows));
    }

    private static MultiWoodlandRecipeDto ProjectRecipeDto(ForestMakerRecipe r) => new(
        r.GeneratorId,
        r.GeneratorVersion,
        r.PresetId,
        r.WidthM,
        r.LengthM,
        r.LaneCount,
        r.StartAgeYears,
        r.StemsPerHa,
        r.Seed,
        ForestMakerRecipe.SpeciesMixWire(r.SpeciesMix),
        r.OakPercent,
        r.BeechPercent);

    private sealed record MultiWoodlandRecipeInput(
        string? GeneratorId,
        int? GeneratorVersion,
        string? PresetId,
        int? WidthM,
        int? LengthM,
        int? LaneCount,
        int? StartAgeYears,
        int? StemsPerHa,
        int? Seed,
        string? SpeciesMix,
        int? OakPercent,
        int? BeechPercent);

    private sealed record MultiWoodlandRecipeDto(
        string GeneratorId,
        int GeneratorVersion,
        string? PresetId,
        int WidthM,
        int LengthM,
        int LaneCount,
        int StartAgeYears,
        int StemsPerHa,
        int Seed,
        string SpeciesMix,
        int OakPercent,
        int BeechPercent);

    private sealed record MultiWoodlandPreviewRequest(MultiWoodlandRecipeInput? Recipe);

    private sealed record MultiWoodlandNewRequest(
        MultiAreaFence? Fence,
        string? Choice,
        string? PresetId,
        MultiWoodlandRecipeInput? Recipe,
        bool EconomyEnabled,
        bool ConfirmDiscardUnsaved);

    private sealed record MultiWoodlandBuiltInDto(string Key, string Title, string Badge);

    private sealed record MultiWoodlandPresetDto(
        string PresetId,
        string Title,
        string Badge,
        string ExperimentId,
        int TotalStems,
        double TotalAreaHa,
        int CellCount,
        MultiWoodlandRecipeDto Recipe);

    private sealed record MultiWoodlandSpeciesMixOptionDto(string Value, string Label);

    private sealed record MultiWoodlandBoundsDto(
        string GeneratorId,
        int GeneratorVersion,
        int MinWidthM,
        int MaxWidthM,
        int MinLengthM,
        int MaxLengthM,
        int MinLaneCount,
        int MaxLaneCount,
        int MinStartAgeYears,
        int MaxStartAgeYears,
        int MinStemsPerHa,
        int MaxStemsPerHa,
        int MaxLivingStems,
        IReadOnlyList<MultiWoodlandSpeciesMixOptionDto> SpeciesMixes);

    private sealed record MultiWoodlandCurrentDto(
        string ExperimentKey,
        string ExperimentId,
        string ExperimentBadge,
        int AreaCount,
        bool IsForestMaker,
        bool EconomyEnabled,
        bool HasUnsavedProgress,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] MultiWoodlandRecipeDto? Recipe);

    private sealed record MultiWoodlandOptionsPayload(
        IReadOnlyList<MultiWoodlandBuiltInDto> BuiltIns,
        IReadOnlyList<MultiWoodlandPresetDto> Presets,
        MultiWoodlandBoundsDto Bounds,
        string Disclosure,
        MultiWoodlandCurrentDto Current);

    private sealed record MultiWoodlandPreviewLaneDto(
        string AreaId,
        string Label,
        float AreaHa,
        int TotalStems,
        int SitkaCount,
        int OakCount,
        int BeechCount);

    private sealed record MultiWoodlandPreviewDto(
        string ExperimentId,
        string RecipeHash,
        string ContentHash,
        string Badge,
        int TotalStems,
        double TotalAreaHa,
        int CellCount,
        int WidthM,
        int LengthM,
        int LaneCount,
        string SpeciesMix,
        IReadOnlyList<MultiWoodlandPreviewLaneDto> Lanes,
        string Disclosure,
        string GeneratorNote,
        MultiWoodlandRecipeDto Recipe);

    private sealed record MultiWoodlandPreviewPayload(
        bool Valid,
        MultiWoodlandPreviewDto? Preview,
        Notice? Refusal);

    private sealed record MultiWoodlandNewPayload(
        object View,
        object Scenes,
        IReadOnlyDictionary<string, string> Labels,
        object? Stage4,
        MultiAreaEcologyDto? Ecology,
        Notice? Refusal,
        bool HistoryTruncated = false,
        int TotalRecordedYears = 0,
        MultiReviewPreviewDto? ReviewPreview = null,
        bool ProjectionCompacted = false,
        bool Stage4Compacted = false,
        bool PayloadBudgetExceeded = false,
        bool EconomyEnabled = false,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] MultiEconomySnapshotDto? Economy = null,
        bool HasUnsavedProgress = false,
        string? ExperimentBadge = null,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] MultiWoodlandRecipeDto? Recipe = null);
}
