using System.Collections.Generic;

// Biological provenance of a regeneration cohort. Planted origin is diagnostic
// metadata only: it must never change growth, mortality, competition or
// reproduction. Natural is the zero value so legacy saves and new natural
// cohorts read as Natural without a migration step.
public enum RegenerationOrigin
{
    Natural = 0,
    Planted = 1
}

// Why a planting action did or did not create a juvenile. The ecology decides
// survival afterwards; planting never rolls for success.
public enum PlantingOutcome
{
    Success,
    OutsideStand,
    SpeciesUnavailable,
    SpeciesCannotRegenerate,
    AlreadyOccupied,
    NoCapacity
}

// Clear success/failure information for a future Survival/UI caller. The
// message is player-readable; the outcome is the machine-readable reason.
public readonly struct PlantingResult
{
    public readonly bool Success;
    public readonly PlantingOutcome Outcome;
    public readonly int CellIndex;
    public readonly string Message;

    private PlantingResult(bool success, PlantingOutcome outcome, int cellIndex, string message)
    {
        Success = success;
        Outcome = outcome;
        CellIndex = cellIndex;
        Message = message;
    }

    public static PlantingResult Planted(int cellIndex)
    {
        return new PlantingResult(true, PlantingOutcome.Success, cellIndex, "Beech juvenile planted");
    }

    public static PlantingResult Failed(PlantingOutcome outcome, string message, int cellIndex = -1)
    {
        return new PlantingResult(false, outcome, cellIndex, message);
    }
}

// Immutable, player-facing snapshot of one live regeneration cohort. Callers
// can inspect biology without receiving the mutable ForestEcologyCell or cohort.
public readonly struct RegenerationCohortInfo
{
    public readonly TreeSpeciesDefinition Species;
    public readonly string SpeciesId;
    public readonly string DisplayName;
    public readonly float Density;
    public readonly float Height;
    public readonly int EstablishYear;
    public readonly RegenerationOrigin Origin;
    public readonly int OriginYear;

    public RegenerationCohortInfo(ForestRegenerationCohort cohort)
    {
        Species = cohort != null ? cohort.Species : null;
        SpeciesId = cohort != null ? cohort.SpeciesId : "";
        DisplayName = Species != null ? Species.DisplayName : "Unknown species";
        Density = cohort != null ? cohort.Density : 0f;
        Height = cohort != null ? cohort.Height : 0f;
        EstablishYear = cohort != null ? cohort.EstablishYear : -1;
        Origin = cohort != null ? cohort.Origin : RegenerationOrigin.Natural;
        OriginYear = cohort != null ? cohort.OriginYear : -1;
    }
}

public enum RegenerationQueryOutcome
{
    Success,
    OutsideEcologyArea,
    NoRegeneration
}

public readonly struct RegenerationQueryResult
{
    private static readonly RegenerationCohortInfo[] Empty = new RegenerationCohortInfo[0];

    public readonly bool Success;
    public readonly RegenerationQueryOutcome Outcome;
    public readonly int CellIndex;
    public readonly IReadOnlyList<RegenerationCohortInfo> Cohorts;
    public readonly string Message;

    private RegenerationQueryResult(bool success, RegenerationQueryOutcome outcome, int cellIndex,
        IReadOnlyList<RegenerationCohortInfo> cohorts, string message)
    {
        Success = success;
        Outcome = outcome;
        CellIndex = cellIndex;
        Cohorts = cohorts ?? Empty;
        Message = message;
    }

    public static RegenerationQueryResult Found(int cellIndex, RegenerationCohortInfo[] cohorts)
    {
        return new RegenerationQueryResult(true, RegenerationQueryOutcome.Success, cellIndex,
            cohorts, "Regeneration found");
    }

    public static RegenerationQueryResult Failed(RegenerationQueryOutcome outcome, string message, int cellIndex = -1)
    {
        return new RegenerationQueryResult(false, outcome, cellIndex, Empty, message);
    }
}

public enum UprootingOutcome
{
    Success,
    OutsideEcologyArea,
    SpeciesNotPresent,
    NoRegeneration,
    InvalidSpecies
}

public readonly struct UprootingResult
{
    public readonly bool Success;
    public readonly UprootingOutcome Outcome;
    public readonly int CellIndex;
    public readonly string Message;

    private UprootingResult(bool success, UprootingOutcome outcome, int cellIndex, string message)
    {
        Success = success;
        Outcome = outcome;
        CellIndex = cellIndex;
        Message = message;
    }

    public static UprootingResult Uprooted(int cellIndex, string displayName)
    {
        return new UprootingResult(true, UprootingOutcome.Success, cellIndex,
            $"{displayName} regeneration uprooted");
    }

    public static UprootingResult Failed(UprootingOutcome outcome, string message, int cellIndex = -1)
    {
        return new UprootingResult(false, outcome, cellIndex, message);
    }
}
