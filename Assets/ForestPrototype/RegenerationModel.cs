using System;
using System.Collections.Generic;

// Regeneration representation versions (saved as ForestSaveData.regenerationModel).
//   0 Legacy: one cohort per species per cell, seed-independent infill,
//     whole-cohort promotion. Kept for v1-15 saves and Reference Future v1.
//   1 AgeBands: separate bands per species + origin + establishment year,
//     seed-only recruitment, non-shrinking capacity, oldest-band promotion.
// Juvenile abundance is an abstract relative abundance (occupancy), never a
// stem count; promotion is a representation handoff to one exact tree.
public static class RegenerationModel
{
    public const int Legacy = 0;
    public const int AgeBands = 1;
    public const int Latest = AgeBands;

    // Model 1 representation threshold for one species + origin population in
    // a cell. Abundance below it is held in a single sub-threshold accumulator
    // band that cannot promote and is not drawn. It is never a loss, a
    // mortality probability or a stem count. (Model 0 keeps its legacy reset.)
    public const float RepresentationThreshold = 0.01f;

    public static int Normalize(int model)
    {
        return model < Legacy ? Legacy : model > Latest ? Latest : model;
    }
}

// One species' regeneration flows for the most recent annual step, in
// relative-abundance units. Diagnostic only: never saved, never read by the
// simulation. Seed arrival is an unnormalised dispersal intensity and must not
// be compared with abundance.
public sealed class RegenerationSpeciesAccount
{
    public float SeedArrival;
    public float EstablishmentRequested;
    public float EstablishmentAccepted;
    public float CapacityRejected;
    // Model 0 only: seed-independent infill and the capacity clamp that can
    // shrink existing stock.
    public float InfillAccepted;
    public float CapacityContraction;
    public float LightLoss;
    public float BrowseLoss;
    // Model 0 only: abundance removed by the legacy 0.01 reset.
    public float ThresholdExtinction;
    // Model 1: accepted recruitment below the representation threshold, and
    // sub-threshold records reclassified as represented bands.
    public float SubThresholdRecruitment;
    public int ThresholdCrossings;
    public int BandsCreated;
    public int BandMerges;
    public int PromotedBands;
    public float PromotionExported;
    public int ExactTreesCreated;
}

public sealed class RegenerationAnnualAccount
{
    public int Year;
    public int Model;
    public readonly SortedDictionary<string, RegenerationSpeciesAccount> Species =
        new SortedDictionary<string, RegenerationSpeciesAccount>(StringComparer.Ordinal);

    public RegenerationSpeciesAccount For(string speciesId)
    {
        if (!Species.TryGetValue(speciesId, out RegenerationSpeciesAccount account))
            Species[speciesId] = account = new RegenerationSpeciesAccount();
        return account;
    }
}
