using System;
using UnityEngine;

// Stand geometry is independent of the RNG, regeneration, growth and storm model versions (D-056).
// It names the authoritative property size and ecology grid a world was made in, because every
// cell-indexed record (ecology cells, understorey, juveniles, deadwood, history, work orders) is
// only meaningful against that grid. It is always explicit: never inferred from cell counts,
// cell indices, tree positions or the scenario definition version.
//
//   0 = Legacy40    the historical 40 x 40 m property, 5 m cells, 8 x 8 = 64 cells. Every save
//                   written before v20, a missing field and Reference Future v1 (permanently).
//   1 = Enlarged80  80 x 80 m, 5 m cells, 16 x 16 = 256 cells.
public static class StandGeometryModel
{
    public const int Legacy40 = 0;
    public const int Enlarged80 = 1;
    public const int Latest = Enlarged80;

    public const float CellSizeMeters = 5f;

    public static bool IsKnown(int model) => model == Legacy40 || model == Enlarged80;

    public static float StandSizeMeters(int model) => model == Enlarged80 ? 80f : 40f;

    public static int CellsPerAxis(int model) => Mathf.Max(1, Mathf.CeilToInt(StandSizeMeters(model) / CellSizeMeters));

    public static int CellCount(int model)
    {
        int axis = CellsPerAxis(model);
        return axis * axis;
    }

    // Same indexing as ForestEcologyController.GetCellIndex: grid centred on the origin, index = z * axis + x.
    // Used where a save must be checked against the geometry it was made in before the live grid is changed.
    public static int CellIndex(int model, Vector3 worldPosition)
    {
        float size = StandSizeMeters(model);
        float origin = -size * 0.5f;
        int axis = CellsPerAxis(model);
        int x = Mathf.FloorToInt((worldPosition.x - origin) / CellSizeMeters);
        int z = Mathf.FloorToInt((worldPosition.z - origin) / CellSizeMeters);
        if (x < 0 || z < 0 || x >= axis || z >= axis)
            return -1;
        return z * axis + x;
    }

    // The geometry a save is read as. Only v20+ saves carry an explicit model; every older save,
    // and a missing field, is Legacy40. No automatic migration (same convention as growthModel).
    public static int ForSave(ForestSaveData data) => data != null && data.version >= 20 ? data.standGeometryModel : Legacy40;

    public static string Label(int model) => model == Legacy40 ? "Legacy40" : model == Enlarged80 ? "Enlarged80" : "unknown(" + model + ")";
}

// Which geometry a NEW Scenario One game starts in. Production authority is exactly: this new-game
// policy, or the standGeometryModel persisted in a save. An environment variable is never part of
// that authority; the verification override below exists only inside the Unity Editor.
public static class StandGeometryPolicy
{
    // 80A: new games stay Legacy40 (zero player-facing change). 80B switches this to Enlarged80.
    public const int NewGameModel = StandGeometryModel.Legacy40;

#if UNITY_EDITOR
    // Verification only. Compiled out of every Player, never serialized into a save, and honoured only
    // by NewGameModelForSession (which is what new-game setup consults). Set by a gate directly, or by
    // the runner through CCF_STAND_GEOMETRY (read below, Editor only).
    private static int? verificationOverride;

    public static int? VerificationOverride
    {
        get => verificationOverride;
        set
        {
            if (value.HasValue && !StandGeometryModel.IsKnown(value.Value))
                throw new ArgumentOutOfRangeException(nameof(value), "unknown stand geometry model " + value.Value);
            verificationOverride = value;
        }
    }

    private static bool environmentRead;

    private static void ReadVerificationEnvironment()
    {
        if (environmentRead) return;
        environmentRead = true;
        string text = Environment.GetEnvironmentVariable("CCF_STAND_GEOMETRY");
        if (!verificationOverride.HasValue && int.TryParse(text, out int parsed) && StandGeometryModel.IsKnown(parsed))
            verificationOverride = parsed;
    }
#endif

    // The model a new game uses in this session.
    public static int NewGameModelForSession
    {
        get
        {
#if UNITY_EDITOR
            ReadVerificationEnvironment();
            if (verificationOverride.HasValue) return verificationOverride.Value;
#endif
            return NewGameModel;
        }
    }
}
