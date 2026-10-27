// Which Work Plan sections are offered as usable at the current teaching stage. Presentation only: it never changes the forestry
// mechanic, an order, the economy or an objective rule. Later stages unlock more of the plan so Objective 1 (access racks) is not
// buried under planting, nursery and pruning controls it does not use. Plain C# (no UI Toolkit) so the rules can be checked offline.
//
// Stage numbers are indexes into ScenarioOneTeachingSequence.Lessons (0 = Create access racks).
public static class ScenarioOneWorkPlanFocus
{
    public const string LaterObjectiveNote = "Available in a later objective.";
    // "Secure the next generation": the lesson that asks for planting.
    public const int PlantingFromLesson = 5;
    // "Continue the transformation": the stage where any further work is the player's judgement. No earlier lesson teaches pruning.
    public const int PruningFromLesson = 7;

    // A section is never locked if it already holds something: existing planting or pruning work and owned stock stay usable, so a
    // player who got there another way (or loaded an older forest) is never locked out of their own plan.
    public static bool PlantingOpen(int lesson, bool plantingOrStockExists) => lesson >= PlantingFromLesson || plantingOrStockExists;
    public static bool PruningOpen(int lesson, bool pruningExists) => lesson >= PruningFromLesson || pruningExists;
}
