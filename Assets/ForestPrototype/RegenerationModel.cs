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
    // Abundance removed by the 0.01 extinction/reset threshold.
    public float ThresholdExtinction;
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
