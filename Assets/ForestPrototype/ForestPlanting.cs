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
