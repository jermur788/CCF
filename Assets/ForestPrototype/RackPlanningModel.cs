// Rack planning world/workflow identity (save v21, D-064). Independent of the save-file version:
// `ForestSaveData.version` versions the whole schema, while this says which rack workflow the world
// uses. A missing field and every save before v21 are model 0. Only a new Enlarged80 Scenario One
// game starts at model 1; Legacy40 and Reference Future v1 stay model 0 forever.
public static class RackPlanningModel
{
    public const int Legacy = 0;
    public const int RackPlanningFieldMarkingV2 = 1;
    public const int Latest = RackPlanningFieldMarkingV2;

    public static bool IsKnown(int model) => model == Legacy || model == RackPlanningFieldMarkingV2;
}
