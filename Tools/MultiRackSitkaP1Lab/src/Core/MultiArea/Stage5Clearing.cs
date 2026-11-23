namespace SitkaP1.Core;

/// <summary>
/// The selective ClearUndergrowth work (spec §5, owner decision 2): on each targeted cell bramble and bracken become 0 and
/// every Sitka band is kept unchanged. This is an intentional divergence from Unity's destructive whole-cell
/// RemoveRegeneration. Clearing is temporary: the next annual step regrows cover toward its light target at gain 0.30.
/// No cost (there is no economy in this stage). Validation and the fenced lifecycle live in MultiAreaContinuation.
/// </summary>
internal static class Stage5Clearing
{
    /// <summary>
    /// The state after clearing every target, and one record per target in (area, column, row) order. The caller has already
    /// validated the whole batch; this builds new records only and never mutates <paramref name="state"/>.
    /// </summary>
    public static (MultiAreaStage5State Next, IReadOnlyList<MultiAreaClearUndergrowthRecord> Records) Apply(MultiAreaStage5State state,
        IReadOnlyCollection<MultiAreaClearUndergrowthOrder> targets, int year, int workRevision, string executionKey, string planId)
    {
        var wanted = targets.Select(order => (order.AreaId, order.Column, order.Row)).ToHashSet();
        var records = new List<MultiAreaClearUndergrowthRecord>();
        var areas = state.Areas.Select(area =>
        {
            if (!targets.Any(order => order.AreaId == area.AreaId)) return area;
            var cells = area.Cells.Select(cell =>
            {
                if (!wanted.Contains((area.AreaId, cell.Column, cell.Row))) return cell;
                records.Add(new MultiAreaClearUndergrowthRecord(area.AreaId, cell.Column, cell.Row, year, workRevision, executionKey,
                    planId, cell.Bramble, cell.Bracken, cell.Bands.Count, cell.Abundance));
                return cell with { Bramble = 0f, Bracken = 0f };
            }).ToArray();
            return area with { Cells = Array.AsReadOnly(cells) };
        }).ToArray();
        var ordered = records.OrderBy(r => r.AreaId, StringComparer.Ordinal).ThenBy(r => r.Column).ThenBy(r => r.Row).ToArray();
        return (state with
        {
            Areas = Array.AsReadOnly(areas),
            ClearLog = Array.AsReadOnly(state.ClearLog.Concat(ordered).ToArray()),
        }, Array.AsReadOnly(ordered));
    }
}

/// <summary>The Stage 5 lineage in the staleness token (owner Q3).</summary>
internal static class Stage5Identity
{
    /// <summary>
    /// The area-model version with the regeneration version appended ONLY when it is not 0, so every 0/0/0, 0/0/1 and
    /// 1/1/1 token (and every hash built from one) stays byte-identical.
    /// </summary>
    public static string AreaModelVersion(string stage4OrEarlier, MultiAreaRegenerationModelVersion regeneration) =>
        regeneration == MultiAreaRegenerationModelVersion.Off
            ? stage4OrEarlier
            : stage4OrEarlier + "+regeneration-v" + ((int)regeneration).ToString(System.Globalization.CultureInfo.InvariantCulture);
}
