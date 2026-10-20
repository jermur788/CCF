using System;
using System.Collections.Generic;

// Species response to site descriptors (Oak/Beech site infrastructure v1.1).
//
// DORMANT: nothing in the simulation reads these types. They separate three things that must not be mixed up:
//   SITE DESCRIPTION           what the site is            ForestSiteProfile.cs
//   SPECIES RESPONSE (ordinal) which way a species leans   the SiteAxisResponse tables below (qualitative data only)
//   NUMERICAL GROWTH EFFECT    how much growth changes     always exactly 1.0 and flagged inactive
// A supported DIRECTION (for example "beech is limited by waterlogging") is not a growth coefficient. No curve, weight
// or multiplier other than the neutral 1.0 exists, because the research found no defensible numbers (branch
// research/oak-beech-adult-site-reconciliation-v2, Docs/Research/OakBeechAdultParity/
// OakBeechAdultGrowthSiteSpecification_v2.md, final gate NO; cited below as "research v2"). Any multiplier added later
// is a calibration, even if its sign is sound.

// How well a site class suits a species, as a relative tendency. Limiting < Tolerated < Preferred.
// NotAssessed means "no statement is encoded" - it is not the lowest rank, and it is never implied for Unknown or
// Reference. The Reference site is a compatibility identity, so no species is ever recorded as finding it suitable.
public enum SiteSuitability
{
    NotAssessed = 0,
    Limiting = 1,
    Tolerated = 2,
    Preferred = 3,
}

// Evidence status of the qualitative statements in an axis response. Says nothing about any number.
public enum SiteResponseBasis
{
    // No direction is encoded (the research recognises the factor but gives no per-class direction, or says nothing).
    Deferred = 0,
    // A rank or direction only, without a quantity.
    Ordinal = 1,
    // Direction supported by direct or transferred (for example British or European) species evidence.
    DirectOrTransferEvidence = 2,
    // A model/design choice. Reserved for future numerical work; no current data uses it.
    Calibration = 3,
}

// One species' qualitative response along one site axis. TClass is one of the site-domain enums.
public sealed class SiteAxisResponse<TClass> where TClass : struct, Enum
{
    // Unknown (0) and Reference (1) always lead each domain enum, so descriptive classes start at 2.
    private const int FirstDescriptiveClass = 2;

    public readonly SiteResponseBasis Basis;
    // Where the statement comes from and what it does NOT claim. Part of the provenance record.
    public readonly string Note;
    private readonly Dictionary<TClass, SiteSuitability> assessed = new Dictionary<TClass, SiteSuitability>();

    // A basis other than Deferred needs at least one stated direction, and a stated direction needs a basis, so the
    // data cannot claim evidence it does not carry.
    public SiteAxisResponse(SiteResponseBasis basis, string note, params (TClass siteClass, SiteSuitability suitability)[] entries)
    {
        if (!Enum.IsDefined(typeof(SiteResponseBasis), basis))
            throw new ArgumentOutOfRangeException(nameof(basis));
        if (string.IsNullOrEmpty(note))
            throw new ArgumentException("An axis response needs a provenance note.", nameof(note));
        entries = entries ?? Array.Empty<(TClass, SiteSuitability)>();
        if ((entries.Length == 0) != (basis == SiteResponseBasis.Deferred))
            throw new ArgumentException("Stated directions need a basis other than Deferred, and Deferred states none.");
        foreach ((TClass siteClass, SiteSuitability suitability) in entries)
        {
            if (!Enum.IsDefined(typeof(TClass), siteClass) || Convert.ToInt32(siteClass) < FirstDescriptiveClass)
                throw new ArgumentException("Only descriptive classes can be assessed, never Unknown or Reference.");
            if (suitability == SiteSuitability.NotAssessed || !Enum.IsDefined(typeof(SiteSuitability), suitability))
                throw new ArgumentException("Leave a class out instead of assessing it as NotAssessed.");
            if (assessed.ContainsKey(siteClass))
                throw new ArgumentException("Duplicate site class " + siteClass + ".");
            assessed.Add(siteClass, suitability);
        }
        Basis = basis;
        Note = note;
    }

    // How many descriptive classes carry a stated direction.
    public int AssessedCount => assessed.Count;

    public SiteSuitability SuitabilityOf(TClass siteClass) =>
        assessed.TryGetValue(siteClass, out SiteSuitability value) ? value : SiteSuitability.NotAssessed;
}

// Everything the model records about how one species relates to site. Immutable.
public sealed class SpeciesSiteResponse
{
    // The only numerical growth effect that exists in v1: no change. Inherited by every species and every site.
    public const float NeutralGrowthMultiplier = 1f;

    public readonly string SpeciesId;
    public readonly SiteAxisResponse<MoistureClass> Moisture;
    public readonly SiteAxisResponse<DrainageAerationClass> DrainageAeration;
    public readonly SiteAxisResponse<FertilityClass> Fertility;
    public readonly SiteAxisResponse<ExposureClimateClass> Exposure;
    // True for Sitka spruce: its existing site-class height, density and productivity routes stay authoritative, so
    // growth code wired to this seam later must bypass it for Sitka instead of multiplying by one (research v2 section 11).
    public readonly bool UsesLegacySiteRoute;

    public SpeciesSiteResponse(string speciesId, SiteAxisResponse<MoistureClass> moisture,
        SiteAxisResponse<DrainageAerationClass> drainageAeration, SiteAxisResponse<FertilityClass> fertility,
        SiteAxisResponse<ExposureClimateClass> exposure, bool usesLegacySiteRoute)
    {
        if (string.IsNullOrEmpty(speciesId)) throw new ArgumentException("A species id is required.", nameof(speciesId));
        SpeciesId = speciesId;
        Moisture = moisture ?? throw new ArgumentNullException(nameof(moisture));
        DrainageAeration = drainageAeration ?? throw new ArgumentNullException(nameof(drainageAeration));
        Fertility = fertility ?? throw new ArgumentNullException(nameof(fertility));
        Exposure = exposure ?? throw new ArgumentNullException(nameof(exposure));
        UsesLegacySiteRoute = usesLegacySiteRoute;
    }

    // Always false in v1. A future calibrated response must change this deliberately, in its own reviewed packet.
    public bool NumericallyActive => false;

    // The site is accepted so the signature will not change when a calibrated response exists. Today it has no effect.
    public float GrowthMultiplier(SiteConditionProfile site) => NeutralGrowthMultiplier;
}

// The three species the project currently carries. Ids match the species assets' speciesId values (checked by the
// offline verification, which reads them from the assets; this file stays free of any Unity or growth dependency).
public static class SpeciesSiteResponses
{
    public const string SitkaSpruceId = "sitka-spruce";
    public const string SessileOakId = "sessile-oak";
    public const string BeechId = "beech";

    // Sitka keeps its own site-class route; nothing about it is assessed here.
    public static readonly SpeciesSiteResponse SitkaSpruce = new SpeciesSiteResponse(SitkaSpruceId,
        new SiteAxisResponse<MoistureClass>(SiteResponseBasis.Deferred,
            "Sitka keeps its existing site-class height, density and productivity routes (research v2 section 11); no moisture row is assigned."),
        new SiteAxisResponse<DrainageAerationClass>(SiteResponseBasis.Deferred,
            "Sitka keeps its existing site-class routes (research v2 section 11); no drainage/aeration row is assigned."),
        new SiteAxisResponse<FertilityClass>(SiteResponseBasis.Deferred,
            "Sitka keeps its existing site-class routes (research v2 section 11); no fertility row is assigned."),
        new SiteAxisResponse<ExposureClimateClass>(SiteResponseBasis.Deferred,
            "Sitka keeps its existing site-class routes (research v2 section 11); no exposure row is assigned."),
        usesLegacySiteRoute: true);

    // Sessile oak (Quercus petraea), not interchangeable "oak spp.".
    // The oak evidence is about DRAINAGE, not about water availability, so the two are kept apart: freer drainage being
    // favoured must not become "dry sites are good" (research v2 sections 5 and 8).
    public static readonly SpeciesSiteResponse SessileOak = new SpeciesSiteResponse(SessileOakId,
        new SiteAxisResponse<MoistureClass>(SiteResponseBasis.Deferred,
            "Research v2 section 5 expressly does NOT establish a universal dry-site advantage or greater drought tolerance than beech at every stress level, "
            + "and gives no other water-availability direction for sessile oak. None is encoded; the freer-draining tendency is recorded under drainage/aeration."),
        new SiteAxisResponse<DrainageAerationClass>(SiteResponseBasis.DirectOrTransferEvidence,
            "Research v2 sections 5 and 8: a relatively freer-draining, lighter-soil niche than pedunculate oak; avoid compacted or wet sites. "
            + "A relative tendency, not a measured preference. Impeded drainage lies between the two stated ends and is not stated, so not assessed.",
            (DrainageAerationClass.WellAerated, SiteSuitability.Preferred),
            (DrainageAerationClass.WaterloggedRisk, SiteSuitability.Limiting)),
        new SiteAxisResponse<FertilityClass>(SiteResponseBasis.DirectOrTransferEvidence,
            "Research v2 section 8: tolerates some poorer mineral soils. An ordinal tolerance, not a claim that poor soil is best or that rich soil gives maximal productivity. Other classes not assessed.",
            (FertilityClass.Low, SiteSuitability.Tolerated)),
        new SiteAxisResponse<ExposureClimateClass>(SiteResponseBasis.Deferred,
            "Research v2 section 8 recognises exposure/form and frost considerations but gives no per-class direction. None is encoded."),
        usesLegacySiteRoute: false);

    // Beech is where the split matters: moist is preferred and drought is limiting (water availability), while good
    // aeration is preferred and waterlogging is limiting (drainage/aeration). "Moist wet soil is not waterlogging"
    // (research v2 section 8), so Wetter is NOT recorded as limiting just because waterlogging is.
    public static readonly SpeciesSiteResponse Beech = new SpeciesSiteResponse(BeechId,
        new SiteAxisResponse<MoistureClass>(SiteResponseBasis.DirectOrTransferEvidence,
            "Research v2 sections 6 and 8: moist soils preferred and drought a recognised limitation. Wetter is not stated, and wet is not waterlogging, so not assessed.",
            (MoistureClass.Moist, SiteSuitability.Preferred),
            (MoistureClass.Drier, SiteSuitability.Limiting)),
        new SiteAxisResponse<DrainageAerationClass>(SiteResponseBasis.DirectOrTransferEvidence,
            "Research v2 sections 6 and 8: deep, well-aerated soils preferred and waterlogging a recognised limitation. "
            + "A neutral numerical multiplier does not make waterlogged sites favourable for beech. Impeded drainage is not stated, so not assessed.",
            (DrainageAerationClass.WellAerated, SiteSuitability.Preferred),
            (DrainageAerationClass.WaterloggedRisk, SiteSuitability.Limiting)),
        new SiteAxisResponse<FertilityClass>(SiteResponseBasis.Deferred,
            "Research v2 section 8 supports site suitability but gives no per-class direction and no universal ranking over oak. None is encoded."),
        new SiteAxisResponse<ExposureClimateClass>(SiteResponseBasis.Deferred,
            "Research v2 section 8 recognises climatic/exposure considerations; the precise response is ordinal and unspecified. None is encoded."),
        usesLegacySiteRoute: false);

    private static readonly SpeciesSiteResponse[] all = { SitkaSpruce, SessileOak, Beech };

    // Fixed order: Sitka spruce, sessile oak, beech.
    public static IReadOnlyList<SpeciesSiteResponse> All => all;

    public static bool TryGet(string speciesId, out SpeciesSiteResponse response)
    {
        foreach (SpeciesSiteResponse candidate in all)
        {
            if (string.Equals(candidate.SpeciesId, speciesId, StringComparison.Ordinal))
            {
                response = candidate;
                return true;
            }
        }
        response = null;
        return false;
    }
}

// The pure lookup seam a future growth model would call. In v1 it returns exactly 1.0 for every species and every site,
// including Unknown sites and species with no response data.
public static class SiteGrowthResponse
{
    public static float Resolve(string speciesId, SiteConditionProfile site) =>
        SpeciesSiteResponses.TryGet(speciesId, out SpeciesSiteResponse response)
            ? response.GrowthMultiplier(site)
            : SpeciesSiteResponse.NeutralGrowthMultiplier;
}

// ---------------------------------------------------------------------------------------------------------------
// Scenario Two species. Appended to this dormant data file (not a new file) so the dormancy checks that scan every
// production file for these type names keep their meaning. The Scenario One catalog above is unchanged.
// ---------------------------------------------------------------------------------------------------------------

// Qualitative site responses for the three Scenario Two species (SCENARIO-TWO-SPECIES-MODEL-FOUNDATION-V1).
//
// DORMANT, exactly like SpeciesSiteResponses: nothing in the simulation reads these tables and every numerical growth
// effect is the neutral 1.0 (SpeciesSiteResponse.GrowthMultiplier). They are a separate catalog on purpose. The
// Scenario One catalog (SpeciesSiteResponses.All) holds exactly Sitka, Oak and Beech and its offline check asserts
// that, so adding rows there would change a protected check; this file reuses only the response TYPES.
//
// ONLY UNCONTESTED RULES ARE ENCODED. A direction is recorded when the rechecked sources state it and none of them
// contradicts it. Anything contested, single-page-and-low-confidence, or merely inferred is left Deferred (no
// direction), and the note says why. Nothing here is a growth coefficient, and "wet" is never taken to mean
// "waterlogged": water availability (Moisture) and drainage/aeration are separate axes (see ForestSiteProfile.cs).
//
// COMMON ALDER AND WATERLOGGING IS ON HOLD. The sources conflict: Forest Research asks for "a high, but not
// stagnant, water table" while also calling alder "remarkably tolerant of wet soil conditions", and the USDA Silvics
// account says seedlings survive flooding that kills most trees but were killed when fully submerged and hardly
// affected when the water stood flush with the soil. The conflict is NOT resolved here: alder's drainage/aeration
// axis encodes no direction at all, for any class, and must not be hardened into a species constant until a later
// decision (OD-07) rules on it.
//
// Provenance tags follow Docs/ScenarioTwoSpeciesFoundation.md. The evidence-pack identifiers (SP-, DB-, CA-) are the
// rows of the Scenario Two species parameter synthesis, which is supporting evidence, not decision authority.
public static class ScenarioTwoSpeciesSiteResponses
{
    public const string ScotsPineId = "scots-pine";
    public const string DownyBirchId = "downy-birch";
    public const string CommonAlderId = "common-alder";

    // Scots pine (Pinus sylvestris).
    public static readonly SpeciesSiteResponse ScotsPine = new SpeciesSiteResponse(ScotsPineId,
        new SiteAxisResponse<MoistureClass>(SiteResponseBasis.Deferred,
            "SP-13: Forest Research and UCC describe light, sandy, stony, low-fertility soils, which is a soil-texture statement, not a water-availability direction. "
            + "No dry-site preference is encoded, so dry is never taken to be good."),
        new SiteAxisResponse<DrainageAerationClass>(SiteResponseBasis.Ordinal,
            "SP-13: the Species Evidence Pack matrix records a poor fit to wet stagnant sites (direction only, one source; the Forest Research page does not mention waterlogging). "
            + "Wetter is not waterlogged, so no moisture class is marked limiting. Other classes not stated.",
            (DrainageAerationClass.WaterloggedRisk, SiteSuitability.Limiting)),
        new SiteAxisResponse<FertilityClass>(SiteResponseBasis.DirectOrTransferEvidence,
            "SP-13: Forest Research (grows well on acid to neutral, light soils of low fertility) and UCC (acidic soils with low fertility). "
            + "An ordinal tolerance, not a claim that poor soil is best: Forest Research adds that pine grows vigorously on more fertile sites. Other classes not stated.",
            (FertilityClass.Low, SiteSuitability.Tolerated)),
        new SiteAxisResponse<ExposureClimateClass>(SiteResponseBasis.Deferred,
            "SP-13: Forest Research gives one ordinal rating (Moderate; suffers from exposure). Single page, low confidence; no per-class direction is encoded."),
        usesLegacySiteRoute: false);

    // Downy birch (Betula pubescens). Not interchangeable with silver birch (B. pendula): the wet-soil tolerance below
    // is the point of difference.
    public static readonly SpeciesSiteResponse DownyBirch = new SpeciesSiteResponse(DownyBirchId,
        new SiteAxisResponse<MoistureClass>(SiteResponseBasis.DirectOrTransferEvidence,
            "DB-09: Forest Research (most moist soils; moister soils of very poor to medium nutrient status) and Teagasc (tolerant of wet or waterlogged conditions). "
            + "Moist preferred, Wetter tolerated. Drier is not stated, so not assessed. No wetness bonus is implied.",
            (MoistureClass.Moist, SiteSuitability.Preferred),
            (MoistureClass.Wetter, SiteSuitability.Tolerated)),
        new SiteAxisResponse<DrainageAerationClass>(SiteResponseBasis.DirectOrTransferEvidence,
            "DB-09: Teagasc says downy birch tolerates wet or waterlogged conditions, heavy clays and wet peatlands, unlike silver birch (the Forest Research page does not state it). "
            + "A tolerance, not a preference. Other classes not stated.",
            (DrainageAerationClass.WaterloggedRisk, SiteSuitability.Tolerated)),
        new SiteAxisResponse<FertilityClass>(SiteResponseBasis.DirectOrTransferEvidence,
            "DB-09: Forest Research gives soils of very poor to medium nutrient status. An ordinal tolerance. High fertility is not assessed.",
            (FertilityClass.Low, SiteSuitability.Tolerated),
            (FertilityClass.Moderate, SiteSuitability.Tolerated)),
        new SiteAxisResponse<ExposureClimateClass>(SiteResponseBasis.Deferred,
            "DB-09: Forest Research rates exposure Moderate and says frost resistant and windfirm. Single source, low confidence; no per-class direction is encoded."),
        usesLegacySiteRoute: false);

    // Common alder (Alnus glutinosa).
    public static readonly SpeciesSiteResponse CommonAlder = new SpeciesSiteResponse(CommonAlderId,
        new SiteAxisResponse<MoistureClass>(SiteResponseBasis.DirectOrTransferEvidence,
            "CA-09: Forest Research (high water table; riparian planting), USDA Silvics (seedlings need constant moisture and are drought-sensitive) and Claessens (natural regeneration needs high light and moisture) agree on water availability. "
            + "Wetter preferred, Moist tolerated, Drier limiting. This is water AVAILABILITY only; it says nothing about waterlogging.",
            (MoistureClass.Wetter, SiteSuitability.Preferred),
            (MoistureClass.Moist, SiteSuitability.Tolerated),
            (MoistureClass.Drier, SiteSuitability.Limiting)),
        new SiteAxisResponse<DrainageAerationClass>(SiteResponseBasis.Deferred,
            "HOLD - OPEN CONTRADICTION (C-13), deliberately not resolved in code. Forest Research: a high but not stagnant water table, yet remarkably tolerant of wet soil. "
            + "USDA Silvics: seedlings survive flooding that kills most trees, but died when fully submerged and were hardly hindered when water stood flush with the soil. "
            + "Survival tolerance and growth under low soil oxygen are different claims. No class, including WaterloggedRisk and WellAerated, is assessed."),
        new SiteAxisResponse<FertilityClass>(SiteResponseBasis.DirectOrTransferEvidence,
            "CA-09: Forest Research requires a medium to rich nutrient supply. Moderate and High preferred; Low not stated. Nitrogen fixation is real but the simulation has no nutrient mechanic, so no soil-nitrogen effect is implied.",
            (FertilityClass.Moderate, SiteSuitability.Preferred),
            (FertilityClass.High, SiteSuitability.Preferred)),
        new SiteAxisResponse<ExposureClimateClass>(SiteResponseBasis.Deferred,
            "CA-09: Forest Research says not tolerant of exposure; Teagasc says moderately resistant to salt spray. Different stressors, low confidence; no per-class direction is encoded (C-17)."),
        usesLegacySiteRoute: false);

    private static readonly SpeciesSiteResponse[] all = { ScotsPine, DownyBirch, CommonAlder };

    // Fixed order: Scots pine, downy birch, common alder.
    public static IReadOnlyList<SpeciesSiteResponse> All => all;

    public static bool TryGet(string speciesId, out SpeciesSiteResponse response)
    {
        foreach (SpeciesSiteResponse candidate in all)
        {
            if (string.Equals(candidate.SpeciesId, speciesId, StringComparison.Ordinal))
            {
                response = candidate;
                return true;
            }
        }
        response = null;
        return false;
    }
}
