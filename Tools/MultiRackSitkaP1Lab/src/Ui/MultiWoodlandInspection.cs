using System.Collections.Immutable;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using SitkaP1.Core.WoodlandImport;
using SitkaP1.Core.WoodlandImport.Inspection;

namespace SitkaP1.Ui;

/// <summary>
/// Real-woodland survey inspection on <c>/multi</c> (spec <c>multi-2d-woodland-inspection-v1</c>, Part 2).
/// The built-in catalogue (Donadea first, Mersch second) is read from fixture files embedded in this assembly, so it is
/// fully offline: no upload, no file path and no network. Loading a plot shows its Year-0 survey exactly as the Part 1
/// projector built it; the playable woodland underneath is kept untouched. While a plot whose
/// <see cref="WoodlandInspectionView.CanAdvanceTime"/> is false is being inspected, <see cref="HandleAsync"/> refuses
/// every request that could change the simulation (see <see cref="InspectionAllowedPosts"/>).
/// </summary>
internal sealed partial class PlayerApi
{
    internal const string InspectionReadOnlyCode = "WoodlandInspectionReadOnly";
    private const string InspectionPlotUnknownCode = "WoodlandInspectionPlotUnknown";
    private const string InspectionUnavailableCode = "WoodlandInspectionUnavailable";
    private const string FixtureResourcePrefix = "SitkaP1.Ui.woodland_fixtures.";

    private static readonly string[] InspectionLoadMembers = { "plotId" };

    /// <summary>
    /// POST routes that stay open while a read-only inspection is active: the inspection's own load/exit, the two ways to
    /// replace the woodland (New woodland, Load save — both clear the inspection when they succeed), and the POST routes
    /// that only read. Every other POST, including any route added later, is refused: the list is fail-closed.
    /// </summary>
    private static readonly IReadOnlySet<string> InspectionAllowedPosts = new HashSet<string>(StringComparer.Ordinal)
    {
        "api/multi/woodland/inspection/load",
        "api/multi/woodland/inspection/exit",
        "api/multi/woodland/new",
        "api/multi/load",
        // Read-only POSTs (no engine state changes).
        "api/multi/woodland/preview",
        "api/multi/hit",
        "api/multi/cell",
        "api/multi/scots-pine/tree",
        "api/multi/competition",
        "api/multi/year/preview-review",
        "api/multi/history/year",
        "api/multi/economy/quote",
        "api/multi/economy/history/year",
        "api/multi/stand",
        "api/hit",
        "api/competition",
    };

    // Plain-English names for the refused action, so the refusal message never shows a route.
    private static readonly IReadOnlyDictionary<string, string> InspectionRefusedActionNames = new Dictionary<string, string>(StringComparer.Ordinal)
    {
        ["api/multi/year/advance"] = "Advance one year",
        ["api/multi/year/skip"] = "Skip to the review year",
        ["api/multi/plan/approve"] = "Approve the Work Plan",
        ["api/multi/plan/execute"] = "Execute the Work Plan",
        ["api/multi/plan/cancel"] = "Cancel the approved Work Plan",
        ["api/multi/save"] = "Save the game",
        ["api/multi/saves/recover"] = "Recover a save",
        ["api/multi/economy/session"] = "Switch Money mode",
    };

    // Built once, on first use, from the embedded fixtures. A failure is kept and reported as a refusal (never a 500 loop).
    private static readonly Lazy<(ImmutableList<BuiltInWoodlandEntry>? Entries, string? Failure)> BuiltInInspectionCatalogue =
        new(LoadBuiltInInspectionCatalogue, LazyThreadSafetyMode.ExecutionAndPublication);

    // The inspected plot, or null in normal play. Written only under multiHost.SyncLock. The admission guard reads it with
    // Volatile as a fast path; the authoritative check is InspectionBlockerLocked(), made inside the same lock as each commit.
    private BuiltInWoodlandEntry? activeInspection;

    private const string InspectionRouteItemKey = "ccf.inspection.route";
    private const string AdmissionCheckpoint = "admission";
    private const string CommitCheckpoint = "commit";

    private void AddWoodlandInspectionRoutes()
    {
        routes.Add("api/multi/woodland/inspection/catalogue", new("GET", ReadInspectionCatalogueAsync));
        routes.Add("api/multi/woodland/inspection/current", new("GET", ReadCurrentInspectionAsync));
        routes.Add("api/multi/woodland/inspection/load", new("POST", LoadInspectionAsync));
        routes.Add("api/multi/woodland/inspection/exit", new("POST", ExitInspectionAsync));
    }

    /// <summary>
    /// Admission check, called by <see cref="HandleAsync"/> after the route, method and Origin guards. It refuses early, before
    /// the body is read, when a read-only survey is already open. It is only a fast path: a request admitted here can still
    /// meet a survey that opens while its body is arriving, so every mutating handler also calls
    /// <see cref="InspectionBlockerLocked"/> inside the lock that guards its commit.
    /// </summary>
    private async Task<bool> RefusedByInspectionAsync(HttpContext context, string routeName)
    {
        context.Items[InspectionRouteItemKey] = routeName;
        if (!string.Equals(context.Request.Method, "POST", StringComparison.Ordinal) || InspectionAllowedPosts.Contains(routeName))
            return false;
        var entry = Volatile.Read(ref activeInspection);
        if (entry is null || entry.InspectionView.CanAdvanceTime) return false;
        await WriteInspectionRefusalAsync(context, entry, AdmissionCheckpoint);
        return true;
    }

    /// <summary>
    /// Commit-time check. The caller holds <c>multiHost.SyncLock</c>, has already read its request body, and calls this
    /// immediately before its engine call or file write. Inspection load, exit, New woodland and Load save all change the
    /// inspection under that same lock, so a mutation and a survey opening can never interleave: whichever takes the lock
    /// first is wholly ordered before the other. Returns the survey that now blocks the mutation, or null.
    /// </summary>
    private BuiltInWoodlandEntry? InspectionBlockerLocked() =>
        activeInspection is { InspectionView.CanAdvanceTime: false } entry ? entry : null;

    /// <summary>Writes the commit-time refusal (see <see cref="InspectionBlockerLocked"/>); nothing changed.</summary>
    private Task WriteInspectionCommitRefusalAsync(HttpContext context, BuiltInWoodlandEntry entry) =>
        WriteInspectionRefusalAsync(context, entry, CommitCheckpoint);

    /// <summary>The typed HTTP 409 WoodlandInspectionReadOnly refusal; nothing changed. <paramref name="checkpoint"/> says which check refused.</summary>
    private Task WriteInspectionRefusalAsync(HttpContext context, BuiltInWoodlandEntry entry, string checkpoint)
    {
        string routeName = context.Items[InspectionRouteItemKey] as string ?? "";
        string action = InspectionRefusedActionNames.TryGetValue(routeName, out var name) ? name : "This change";
        string message = action + " is not available in survey inspection mode. " + entry.DisplayName +
            " is a Year-0 survey that cannot be simulated, so nothing changed. Choose \"Return to normal woodland\" to keep playing.";
        var notice = new Notice(InspectionReadOnlyCode, message);
        return WriteJsonAsync(context, StatusCodes.Status409Conflict,
            new InspectionRefusalPayload(notice, notice, InspectionStatusOf(entry), checkpoint));
    }

    /// <summary>Clears the inspection. The caller holds <c>multiHost.SyncLock</c> (New woodland and Load save commit under it).</summary>
    private void ClearInspectionLocked() => Volatile.Write(ref activeInspection, null);

    // GET api/multi/woodland/inspection/catalogue — the fixed offline list, Donadea first.
    private Task ReadInspectionCatalogueAsync(HttpContext context)
    {
        if (context.Request.Query.Count > 0)
            return WriteErrorAsync(context, StatusCodes.Status400BadRequest, "BadRequest",
                "the catalogue does not accept query parameters; nothing changed");
        var status = InspectionStatusOf(Volatile.Read(ref activeInspection)); // one snapshot per reply
        var (entries, failure) = BuiltInInspectionCatalogue.Value;
        if (entries is null)
            return WriteJsonAsync(context, StatusCodes.Status409Conflict, new InspectionCataloguePayload(
                Array.Empty<InspectionCatalogueEntryDto>(), status, new Notice(InspectionUnavailableCode, failure!)));
        return WriteJsonAsync(context, StatusCodes.Status200OK,
            new InspectionCataloguePayload(entries.Select(ProjectCatalogueEntry).ToArray(), status, null));
    }

    // GET api/multi/woodland/inspection/current — lets a reloaded page restore the inspection screen (no stale UI state).
    private Task ReadCurrentInspectionAsync(HttpContext context)
    {
        if (context.Request.Query.Count > 0)
            return WriteErrorAsync(context, StatusCodes.Status400BadRequest, "BadRequest",
                "this read does not accept query parameters; nothing changed");
        // One read: the status and the survey in this reply always describe the same plot.
        var entry = Volatile.Read(ref activeInspection);
        return WriteJsonAsync(context, StatusCodes.Status200OK,
            new InspectionCurrentPayload(InspectionStatusOf(entry), entry is null ? null : BuildInspectionPayload(entry)));
    }

    // POST api/multi/woodland/inspection/load {"plotId": "..."} — enter read-only Year-0 survey inspection.
    private async Task LoadInspectionAsync(HttpContext context)
    {
        var request = await ReadMultiRequestAsync<InspectionLoadRequest>(context, InspectionLoadMembers);
        if (request is null) return;
        string plotId = request.PlotId!;
        if (plotId.Length is 0 or > MaxIdLength || plotId.Any(ch => char.IsWhiteSpace(ch) || char.IsControl(ch)))
        {
            await WriteErrorAsync(context, StatusCodes.Status400BadRequest, "BadRequest",
                FormattableString.Invariant($"plotId must be 1-{MaxIdLength} characters without whitespace; nothing changed"));
            return;
        }

        var (entries, failure) = BuiltInInspectionCatalogue.Value;
        if (entries is null)
        {
            await WriteJsonAsync(context, StatusCodes.Status409Conflict, new InspectionLoadPayload(null,
                InspectionStatusOf(Volatile.Read(ref activeInspection)), new Notice(InspectionUnavailableCode, failure!)));
            return;
        }
        var entry = entries.FirstOrDefault(e => string.Equals(e.PlotId, plotId, StringComparison.Ordinal));
        if (entry is null)
        {
            await WriteJsonAsync(context, StatusCodes.Status409Conflict, new InspectionLoadPayload(null,
                InspectionStatusOf(Volatile.Read(ref activeInspection)),
                new Notice(InspectionPlotUnknownCode, "That plot is not in the built-in catalogue of real woodlands; nothing changed.")));
            return;
        }

        lock (multiHost.SyncLock) Volatile.Write(ref activeInspection, entry);
        // Both halves of the reply come from the entry this request committed, never from a later re-read, so an
        // overlapping load of another plot cannot make "current" and "inspection" name different plots.
        await WriteJsonAsync(context, StatusCodes.Status200OK,
            new InspectionLoadPayload(BuildInspectionPayload(entry), InspectionStatusOf(entry), null));
    }

    // POST api/multi/woodland/inspection/exit {} — return to the playable woodland, which was never changed.
    private async Task ExitInspectionAsync(HttpContext context)
    {
        var request = await ReadMultiRequestAsync<InspectionExitRequest>(context, Array.Empty<string>());
        if (request is null) return;
        bool wasActive;
        lock (multiHost.SyncLock)
        {
            wasActive = activeInspection is not null;
            ClearInspectionLocked();
        }
        // The status describes this commit (inactive), not a later load that may already have raced in.
        await WriteJsonAsync(context, StatusCodes.Status200OK, new InspectionExitPayload(wasActive, InspectionStatusOf(null)));
    }

    private static InspectionStatusDto InspectionStatusOf(BuiltInWoodlandEntry? entry) => entry is null
        ? new InspectionStatusDto(false, null, null, true)
        : new InspectionStatusDto(true, entry.PlotId, entry.DisplayName, entry.InspectionView.CanAdvanceTime);

    private static InspectionPayloadDto BuildInspectionPayload(BuiltInWoodlandEntry entry)
    {
        // The Part 1 canonical JSON bytes are written into the reply unchanged, so the bytes a client receives hash to
        // exactly the Part 1 golden SHA-256.
        var view = entry.InspectionView;
        byte[] canonical = WoodlandInspectionSerializer.SerializeUtf8(view);
        var trees = view.ObservedYear0.Trees;
        int plotted = trees.Count(t => t.PositionLocalM.Value is not null);
        return new InspectionPayloadDto(
            ProjectCatalogueEntry(entry),
            new CanonicalJson(canonical),
            WoodlandImportSerializer.ComputeSha256Hex(canonical),
            new InspectionSummaryDto(trees.Count, plotted, trees.Count - plotted),
            InspectionDisplayLabels.Instance);
    }

    private static InspectionCatalogueEntryDto ProjectCatalogueEntry(BuiltInWoodlandEntry entry) => new(
        entry.PlotId,
        entry.DisplayName,
        entry.CountryCode,
        entry.TreeCount,
        entry.LivingStemCount,
        entry.SubsetOfGbifRecords,
        entry.Readiness.ToString(),
        InspectionDisplayLabels.ReadinessLabel(entry.Readiness),
        entry.InspectionView.CanAdvanceTime,
        entry.InspectionView.AdvanceTimeStatement,
        entry.LicenceSummary,
        entry.LicenceUrl,
        entry.DatasetTitle,
        entry.Publisher,
        entry.Citation,
        entry.Doi,
        "https://doi.org/" + entry.Doi,
        entry.DatasetKey,
        entry.RetrievalDateUtc,
        entry.ArchiveSha256,
        entry.ObservationDates,
        entry.ModificationsDisclosure,
        entry.Caveats);

    private static (ImmutableList<BuiltInWoodlandEntry>? Entries, string? Failure) LoadBuiltInInspectionCatalogue()
    {
        try
        {
            return (BuiltInWoodlandCatalogue.Load(ReadEmbeddedFixture), null);
        }
        catch (Exception fault) when (fault is InvalidDataException or InvalidOperationException or JsonException)
        {
            Console.Error.WriteLine("WOODLAND CATALOGUE UNAVAILABLE: " + fault.GetType().Name + ": " + fault.Message);
            return (null, "The built-in real woodland catalogue could not be read from this build, so no survey can be inspected.");
        }
    }

    // The qualified fixtures are embedded unchanged from tests/Core/WoodlandImport/Fixtures/Qualified (see SitkaP1.Ui.csproj).
    private static byte[] ReadEmbeddedFixture(string fileName)
    {
        using var stream = typeof(PlayerApi).Assembly.GetManifestResourceStream(FixtureResourcePrefix + fileName)
            ?? throw new InvalidOperationException("the built-in woodland fixture " + fileName + " is not embedded in this build");
        using var copy = new MemoryStream();
        stream.CopyTo(copy);
        return copy.ToArray();
    }

    private sealed record InspectionLoadRequest(string? PlotId);

    private sealed record InspectionExitRequest;

    private sealed record InspectionStatusDto(bool Active, string? PlotId, string? DisplayName, bool CanAdvanceTime);

    private sealed record InspectionCatalogueEntryDto(
        string PlotId,
        string DisplayName,
        string CountryCode,
        int TreeCount,
        int LivingStemCount,
        string SubsetOfGbifRecords,
        string Readiness,
        string ReadinessLabel,
        bool CanAdvanceTime,
        string AdvanceTimeStatement,
        string LicenceSummary,
        string LicenceUrl,
        string DatasetTitle,
        string Publisher,
        string Citation,
        string Doi,
        string DoiUrl,
        string DatasetKey,
        string RetrievalDateUtc,
        string ArchiveSha256,
        IReadOnlyList<string> ObservationDates,
        string ModificationsDisclosure,
        IReadOnlyList<string> Caveats);

    private sealed record InspectionSummaryDto(int TreeCount, int PlottedTreeCount, int UnplottedTreeCount);

    private sealed record InspectionPayloadDto(
        InspectionCatalogueEntryDto Entry,
        CanonicalJson View,
        string ViewSha256,
        InspectionSummaryDto Summary,
        InspectionDisplayLabels Labels);

    private sealed record InspectionCataloguePayload(
        IReadOnlyList<InspectionCatalogueEntryDto> Entries, InspectionStatusDto Inspection, Notice? Refusal);

    private sealed record InspectionCurrentPayload(InspectionStatusDto Inspection, InspectionPayloadDto? Current);

    private sealed record InspectionLoadPayload(InspectionPayloadDto? Current, InspectionStatusDto Inspection, Notice? Refusal);

    private sealed record InspectionExitPayload(bool WasActive, InspectionStatusDto Inspection);

    private sealed record InspectionRefusalPayload(Notice Error, Notice Refusal, InspectionStatusDto Inspection, string Checkpoint);

    /// <summary>Already-serialised canonical JSON, written verbatim by <see cref="CanonicalJsonConverter"/>.</summary>
    [JsonConverter(typeof(CanonicalJsonConverter))]
    private sealed record CanonicalJson(byte[] Utf8);

    private sealed class CanonicalJsonConverter : JsonConverter<CanonicalJson>
    {
        public override CanonicalJson Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
            throw new NotSupportedException("canonical JSON is write-only");

        public override void Write(Utf8JsonWriter writer, CanonicalJson value, JsonSerializerOptions options) =>
            writer.WriteRawValue(value.Utf8, skipInputValidation: false);
    }
}

/// <summary>
/// Fixed plain-English words for the enum values in the inspection JSON, so the page shows no internal code and
/// decides nothing itself. These are display words only; they make no ecological claim.
/// </summary>
internal sealed record InspectionDisplayLabels(
    IReadOnlyDictionary<string, string> Readiness,
    IReadOnlyDictionary<string, string> SpeciesCategories,
    IReadOnlyDictionary<string, string> SpeciesCategoryNotes,
    IReadOnlyDictionary<string, string> Provenance,
    IReadOnlyDictionary<string, string> VitalStatus,
    IReadOnlyDictionary<string, string> Severity,
    IReadOnlyDictionary<string, string> Fields,
    IReadOnlyDictionary<string, string> Units,
    string NotRecorded,
    string BannerTitle,
    string BannerLead)
{
    internal static readonly InspectionDisplayLabels Instance = new(
        Readiness: new Dictionary<string, string>(StringComparer.Ordinal)
        {
            [nameof(WoodlandReadiness.Refused)] = "Refused — the survey data could not be trusted",
            [nameof(WoodlandReadiness.InventoryOnly)] = "Inventory only — the survey can be inspected but not simulated",
            [nameof(WoodlandReadiness.CandidateForSimulationIntegration)] = "Candidate for simulation",
        },
        SpeciesCategories: new Dictionary<string, string>(StringComparer.Ordinal)
        {
            [nameof(SpeciesSupportCategory.ModelledAtBase)] = "Modelled by CCF today",
            [nameof(SpeciesSupportCategory.PendingModelSupport)] = "Planned, not yet modelled",
            [nameof(SpeciesSupportCategory.UnmodelledSpecies)] = "Not modelled by CCF",
            [nameof(SpeciesSupportCategory.UnidentifiedSpecies)] = "Not identified to species",
        },
        SpeciesCategoryNotes: new Dictionary<string, string>(StringComparer.Ordinal)
        {
            [nameof(SpeciesSupportCategory.ModelledAtBase)] = "CCF has a growth model for this species.",
            [nameof(SpeciesSupportCategory.PendingModelSupport)] = "CCF intends to support this species but cannot simulate it yet.",
            [nameof(SpeciesSupportCategory.UnmodelledSpecies)] = "CCF has no growth model for this species. The trees are kept, not replaced.",
            [nameof(SpeciesSupportCategory.UnidentifiedSpecies)] = "No single species was recorded. CCF does not guess one.",
        },
        Provenance: new Dictionary<string, string>(StringComparer.Ordinal)
        {
            [nameof(FieldProvenance.Observed)] = "Observed in the survey",
            [nameof(FieldProvenance.Missing)] = "Not recorded in the survey",
            [nameof(FieldProvenance.Inferred)] = "Inferred by the importer",
            [nameof(FieldProvenance.Simulated)] = "Simulated",
        },
        VitalStatus: new Dictionary<string, string>(StringComparer.Ordinal)
        {
            [nameof(TreeVitalStatus.Alive)] = "Living",
            [nameof(TreeVitalStatus.Dead)] = "Dead",
            [nameof(TreeVitalStatus.Unknown)] = "Status not recognised",
        },
        Severity: new Dictionary<string, string>(StringComparer.Ordinal)
        {
            [nameof(IssueSeverity.Refusal)] = "Import refused",
            [nameof(IssueSeverity.InventoryOnlyBlocker)] = "Blocks simulation",
            [nameof(IssueSeverity.Warning)] = "Warning",
            [nameof(IssueSeverity.Info)] = "Note",
        },
        Fields: new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["speciesName"] = "Species",
            ["vitalStatus"] = "Living or dead",
            ["positionLocalM"] = "Position in plot",
            ["dbhCm"] = "Stem diameter (DBH)",
            ["heightM"] = "Height",
            ["crownRadiusM"] = "Crown radius",
            ["ageYears"] = "Age",
        },
        Units: new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["dbhCm"] = "cm",
            ["heightM"] = "m",
            ["crownRadiusM"] = "m",
            ["ageYears"] = "years",
            ["positionLocalM"] = "m",
        },
        NotRecorded: "Not recorded",
        BannerTitle: "Survey inspection mode — Year 0",
        BannerLead: "You are looking at a real woodland exactly as it was surveyed. Nothing here is simulated, and time cannot advance.");

    internal static string ReadinessLabel(WoodlandReadiness readiness) =>
        Instance.Readiness.TryGetValue(readiness.ToString(), out var text) ? text : readiness.ToString();
}
