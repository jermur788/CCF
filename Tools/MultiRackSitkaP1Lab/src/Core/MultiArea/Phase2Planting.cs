namespace SitkaP1.Core;

/// <summary>
/// The fenced Plant / Shelter / SpotClear rules (spec §7). Not a port of Unity ResolvePlanting: there is no nursery stock,
/// cash, owner-minutes or offer identity; only Unity's geometry, nursery defaults and browse rules are borrowed. Validation
/// is all-or-nothing: any failure refuses the whole batch with a typed refusal and nothing changes. The lifecycle (Pending →
/// Approve → Execute, the fence, the engine-minted key, idempotent replay and zero calendar advance) is the existing one in
/// <see cref="MultiAreaContinuation"/>.
/// </summary>
internal static class Phase2Planting
{
    /// <summary>The engine-minted order ID of the n-th planting order of a session ("P2W0001", ordinal = numeric order).</summary>
    public static string OrderIdFor(int number) => "P2W" + number.ToString("0000", System.Globalization.CultureInfo.InvariantCulture);

    public static string JuvenileIdFor(int number) => "PJ" + number.ToString("0000", System.Globalization.CultureInfo.InvariantCulture);

    public static string PatchIdFor(int number) => "SP" + number.ToString("0000", System.Globalization.CultureInfo.InvariantCulture);

    /// <summary>Unity's shelter ID convention: "S-J" + juvenile ID (PJ0001 → S-JPJ0001).</summary>
    public static string ShelterIdFor(string juvenileId) => "S-J" + juvenileId;

    /// <summary>The fixed resolve order inside one Execute: SpotClear → Plant → Shelter, then ordinal order ID.</summary>
    public static IReadOnlyList<MultiAreaPlantingOrder> InResolveOrder(IEnumerable<MultiAreaPlantingOrder> orders) =>
        orders.OrderBy(order => order.ResolveRank).ThenBy(order => order.OrderId, StringComparer.Ordinal).ToArray();

    private static double DistanceSquared(float ax, float az, float bx, float bz)
    {
        double dx = (double)ax - bx, dz = (double)az - bz;
        return dx * dx + dz * dz;
    }

    /// <summary>
    /// Validates a whole batch against the current committed state (spec §7.2). Orders are checked in ordinal order ID, so for
    /// a spacing conflict the lower work-order ID wins and the later order is named in the refusal; the batch is refused whole.
    /// </summary>
    /// <remarks>
    /// <paramref name="allowScotsPine"/> is true only on a Forest Maker woodland (Save v3), where a Plant order may also name
    /// "scots-pine" (SPEC-multi-2d-scots-pine-moisture-V1 Part 2). Every spacing, shelter and spot-clear rule is unchanged.
    /// </remarks>
    public static void RequireValidBatch(IReadOnlyList<MultiAreaPlantingOrder> orders, Func<string, IReadOnlyList<MultiAreaVertex>> polygonOf,
        IReadOnlyList<AnnualTree> livingAdults, MultiAreaPhase2State state, Func<float, float, (int Area, int Cell)?> locate, int installYear,
        bool allowScotsPine = false)
    {
        var sorted = orders.OrderBy(order => order.OrderId, StringComparer.Ordinal).ToArray();
        var ids = new HashSet<string>(StringComparer.Ordinal);
        var points = new HashSet<(string Kind, string AreaId, float X, float Z)>();
        var plants = new List<MultiAreaPlantOrder>();
        var shelters = new List<MultiAreaShelterOrder>();
        var activeJuveniles = state.Juveniles.Where(j => j.IsActive).ToArray();
        foreach (var order in sorted)
        {
            if (string.IsNullOrWhiteSpace(order.OrderId) || !ids.Add(order.OrderId))
                throw new P1Refusal("Phase2DuplicateOrder", "an order ID appears twice in the batch");
            if (!float.IsFinite(order.XM) || !float.IsFinite(order.ZM))
                throw new P1Refusal("Phase2CoordinatesInvalid", "planting coordinates must be finite metres");
            var polygon = polygonOf(order.AreaId);
            if (!Stage5Promotion.StrictlyInside(polygon, order.XM, order.ZM))
                throw new P1Refusal("Phase2PointOutsideArea", "the point of " + order.OrderId + " is not strictly inside " + order.AreaId);
            if (locate(order.XM, order.ZM) is null)
                throw new P1Refusal("Phase2PointWithoutGroundCell",
                    "the point of " + order.OrderId + " lies in no in-polygon ground cell, so it has no cell light or cover");
            if (!points.Add((order.GetType().Name, order.AreaId, order.XM, order.ZM)))
                throw new P1Refusal("Phase2DuplicateOrder", "two orders of one kind name the same point: " + order.OrderId);

            switch (order)
            {
                case MultiAreaPlantOrder plant:
                    if (Phase2Species.BroadleafOrNull(plant.SpeciesId) is null && !(allowScotsPine && ScotsPineLive.IsPine(plant.SpeciesId)))
                        throw new P1Refusal("Phase2SpeciesUnknown", allowScotsPine
                            ? "only sessile-oak, beech or scots-pine can be planted"
                            : "only sessile-oak or beech can be planted");
                    foreach (var juvenile in activeJuveniles)
                        if (DistanceSquared(plant.XM, plant.ZM, juvenile.XM, juvenile.ZM) <
                            (double)Phase2Constants.MinimumPlantingSpacingM * Phase2Constants.MinimumPlantingSpacingM)
                            throw new P1Refusal("PlantSpacingTooClose",
                                "Another marker occupies this planting position: " + plant.OrderId + " is within 0.5 m of " + juvenile.JuvenileId);
                    foreach (var earlier in plants)
                        if (DistanceSquared(plant.XM, plant.ZM, earlier.XM, earlier.ZM) <
                            (double)Phase2Constants.MinimumPlantingSpacingM * Phase2Constants.MinimumPlantingSpacingM)
                            throw new P1Refusal("PlantSpacingTooClose",
                                "Another marker occupies this planting position: " + plant.OrderId + " is within 0.5 m of " + earlier.OrderId);
                    foreach (var adult in livingAdults)
                        if (DistanceSquared(plant.XM, plant.ZM, adult.XM, adult.ZM) <
                            (double)Phase2Constants.MinimumAdultStemSpacingM * Phase2Constants.MinimumAdultStemSpacingM)
                            throw new P1Refusal("PlantTooCloseToAdult",
                                plant.OrderId + " is within 0.7 m of living adult stem " + adult.TreeId + " (new owner rule)");
                    plants.Add(plant);
                    break;
                case MultiAreaShelterOrder shelter:
                    shelters.Add(shelter);
                    break;
            }
        }
        // Shelters are checked once every Plant of the batch is known (a Shelter may target a same-batch Plant).
        var checkedShelters = new List<MultiAreaShelterOrder>();
        foreach (var shelter in shelters)
        {
            const double match = (double)Phase2Constants.ShelterMatchRadiusM * Phase2Constants.ShelterMatchRadiusM;
            bool hasTarget = activeJuveniles.Any(j => DistanceSquared(shelter.XM, shelter.ZM, j.XM, j.ZM) <= match) ||
                plants.Any(p => DistanceSquared(shelter.XM, shelter.ZM, p.XM, p.ZM) <= match);
            if (!hasTarget)
                throw new P1Refusal("ShelterNoTarget",
                    shelter.OrderId + " has no living planted juvenile or same-batch Plant within 0.25 m");
            if (state.Shelters.Any(s => s.IsEffective(installYear) && DistanceSquared(shelter.XM, shelter.ZM, s.XM, s.ZM) <= match) ||
                checkedShelters.Any(s => DistanceSquared(shelter.XM, shelter.ZM, s.XM, s.ZM) <= match))
                throw new P1Refusal("ShelterAlreadyPresent", shelter.OrderId + ": another effective shelter lies within 0.25 m");
            checkedShelters.Add(shelter);
        }
    }

    /// <summary>
    /// Applies a validated batch at Execute (resolve order SpotClear → Plant → Shelter, then order ID). Planting and shelter
    /// years are the ecological year of the first annual step after Execute (Unity report.year = year + 1); a new patch's
    /// update guard is that same year, so it stays at 0 through that step. Returns the next state; mutates nothing.
    /// </summary>
    public static MultiAreaPhase2State Apply(MultiAreaPhase2State state, IReadOnlyList<MultiAreaPlantingOrder> orders, int currentYear,
        int workRevision, string executionKey, string planId)
    {
        int year = checked(currentYear + 1);
        var juveniles = state.Juveniles.ToList();
        var shelters = state.Shelters.ToList();
        var patches = state.Patches.ToList();
        var log = state.WorkLog.ToList();
        int nextJuvenile = state.NextJuvenileNumber, nextPatch = state.NextPatchNumber;
        foreach (var order in InResolveOrder(orders))
        {
            switch (order)
            {
                case MultiAreaSpotClearOrder clear:
                {
                    string patchId = PatchIdFor(nextPatch++);
                    patches.Add(new SpotPatch(patchId, clear.AreaId, clear.XM, clear.ZM, Phase2Constants.SpotClearRadiusM, year, year, 0f, 0f));
                    log.Add(new Phase2WorkRecord(clear.OrderId, "SpotClear", clear.AreaId, clear.XM, clear.ZM, null, null, null, patchId,
                        currentYear, workRevision, executionKey, planId));
                    break;
                }
                case MultiAreaPlantOrder plant:
                {
                    string juvenileId = JuvenileIdFor(nextJuvenile++);
                    juveniles.Add(new PlantedJuvenile(juvenileId, plant.AreaId, plant.SpeciesId, plant.XM, plant.ZM, year,
                        Phase2Constants.PlantedJuvenileAgeYears, Phase2Constants.PlantedJuvenileHeightM, true, null, -1, null, false, 0f, 0f, -1));
                    log.Add(new Phase2WorkRecord(plant.OrderId, "Plant", plant.AreaId, plant.XM, plant.ZM, plant.SpeciesId, juvenileId, null, null,
                        currentYear, workRevision, executionKey, planId));
                    break;
                }
                case MultiAreaShelterOrder shelter:
                {
                    const double match = (double)Phase2Constants.ShelterMatchRadiusM * Phase2Constants.ShelterMatchRadiusM;
                    var target = juveniles.Where(j => j.IsActive && DistanceSquared(shelter.XM, shelter.ZM, j.XM, j.ZM) <= match)
                        .OrderBy(j => DistanceSquared(shelter.XM, shelter.ZM, j.XM, j.ZM)).ThenBy(j => j.JuvenileId, StringComparer.Ordinal)
                        .FirstOrDefault() ?? throw new P1Refusal("ShelterNoTarget", shelter.OrderId + " has no target at Execute");
                    string shelterId = ShelterIdFor(target.JuvenileId);
                    shelters.Add(new BrowseShelter(shelterId, target.JuvenileId, target.XM, target.ZM, year,
                        Phase2Constants.ShelterEffectiveYears, -1));
                    log.Add(new Phase2WorkRecord(shelter.OrderId, "Shelter", shelter.AreaId, shelter.XM, shelter.ZM, target.SpeciesId,
                        target.JuvenileId, shelterId, null, currentYear, workRevision, executionKey, planId));
                    break;
                }
            }
        }
        return state with
        {
            Juveniles = juveniles.AsReadOnly(),
            Shelters = shelters.AsReadOnly(),
            Patches = patches.AsReadOnly(),
            WorkLog = log.AsReadOnly(),
            NextJuvenileNumber = nextJuvenile,
            NextPatchNumber = nextPatch,
            ApprovedOrders = Array.Empty<MultiAreaPlantingOrder>(),
        };
    }
}
