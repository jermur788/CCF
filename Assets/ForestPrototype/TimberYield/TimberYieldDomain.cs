using System;
using CCF.Forestry.WorkEconomy;

namespace CCF.Forestry.TimberYield
{
    public enum StemProfileKind { VolumeBudgetedParaboloid, MeasuredSquaredDiameterProfile }
    public enum LogAllocationStrategy { GreedyButtToTipConfiguredPriority }
    public enum LogLengthPreference { LongestFirst, ShortestFirst }
    public enum PotentialMaterialUse { SmallRoundwood, PolesAndStakes, RoundwoodForProcessing, SawlogForProcessing }
    public enum StemResidualKind { Stump, CuttingLoss, Unallocated, BelowMerchantableDiameter, SubUnitAllocation }
    [Flags]
    public enum StemQualityFlags { None = 0, PoorForm = 1, StemDefect = 2, WindDamage = 4, BrowseFormDamage = 8, Pruned = 16, Unusable = 32 }

    [Serializable]
    public sealed class StemProfilePoint
    {
        public int HeightMm;
        public int DiameterMm; // Same explicit bark/diameter basis as DBH.
    }

    [Serializable]
    public sealed class StemQualitySection
    {
        public int StartHeightMm;
        public int EndHeightMm;
        public StemQualityFlags Flags;
    }

    [Serializable]
    public sealed class StemMeasurements
    {
        public string TreeId = "";
        public string SpeciesId = "";
        public int DbhMm;
        public int TotalHeightMm;
        public long StemVolumeCm3; // Authoritative input budget, not recomputed from taper.
        public string MeasurementBasis = ""; // e.g. current simulation diameter/volume basis.
        public StemProfileKind ProfileKind;
        public StemProfilePoint[] ProfilePoints = Array.Empty<StemProfilePoint>();
        public StemQualityFlags Quality;
        public StemQualitySection[] QualitySections = Array.Empty<StemQualitySection>();
    }

    [Serializable]
    public sealed class AssortmentDispositionRule
    {
        public TimberAssortment Assortment;
        public TimberDisposition Disposition;
    }

    [Serializable]
    public sealed class StemYieldRequest
    {
        public StemMeasurements Stem = new StemMeasurements();
        public TimberDisposition DefaultDisposition;
        public AssortmentDispositionRule[] DispositionOverrides = Array.Empty<AssortmentDispositionRule>();
        public bool ExtractRetainedToRoadside;
        public bool OwnerPaysHaulage;
    }

    [Serializable]
    public sealed class AllocatedTimberLog
    {
        public string LogId = "";
        public string TreeId = "";
        public string SpeciesId = "";
        public string SpecificationId = "";
        public TimberAssortment Assortment;
        public PotentialMaterialUse PotentialUse; // Not a structural grade or guaranteed construction use.
        public int StartHeightMm;
        public int EndHeightMm;
        public int SmallEndDiameterMm; // Conservative floor; acceptance uses unrounded squared diameter.
        public int LargeEndDiameterMm;
        public long VolumeCm3;
        public StemQualityFlags Quality;
        public TimberDisposition Disposition;
        public bool ExtractToRoadside;
        public bool OwnerPaysHaulage;
    }

    [Serializable]
    public sealed class StemResidualSection
    {
        public StemResidualKind Kind;
        public int StartHeightMm;
        public int EndHeightMm;
        public long VolumeCm3;
    }

    [Serializable]
    public sealed class StemYieldResult
    {
        public string TreeId = "";
        public string SpeciesId = "";
        public string ConfigurationId = "";
        public string MeasurementBasis = "";
        public StemProfileKind ProfileKind;
        public long StemVolumeCm3;
        public int DbhMm;
        public int TotalHeightMm;
        public long GeometricProfileVolumeCm3; // Diagnostic only: budget may differ from ideal shape geometry.
        public int MerchantableTopHeightMm; // Diameter ceiling, not proof a full nominal log fits.
        public long SaleAssortmentVolumeCm3;
        public long RetainedForUseVolumeCm3;
        public long DeadwoodAssortmentVolumeCm3;
        public long ResidualVolumeCm3;
        public int CandidateChecks;
        public AllocatedTimberLog[] Logs = Array.Empty<AllocatedTimberLog>();
        public StemResidualSection[] ResidualSections = Array.Empty<StemResidualSection>();
    }

    [Serializable]
    public sealed class AssortmentYieldSummary
    {
        public string SpeciesId = "";
        public TimberAssortment Assortment;
        public TimberDisposition Disposition;
        public long VolumeCm3;
        public int Pieces;
    }

    [Serializable]
    public sealed class StandOperationYield
    {
        public string OperationId = "";
        public string ConfigurationId = "";
        public long StemVolumeCm3;
        public long SaleAssortmentVolumeCm3;
        public long RetainedForUseVolumeCm3;
        public long DeadwoodAssortmentVolumeCm3;
        public long ResidualVolumeCm3;
        public StemYieldResult[] Stems = Array.Empty<StemYieldResult>();
        public AssortmentYieldSummary[] Assortments = Array.Empty<AssortmentYieldSummary>();
    }
}
