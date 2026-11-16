using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// Stand geometry validation (save v20, D-056). Geometry decides which ecology grid every cell-indexed
// record in a save is checked against, so it is validated first and never inferred from the data.
public static partial class ForestSaveValidation
{
    // Rejects an unknown model on a v20+ save, and any older save that claims a model (older saves are
    // always Legacy40 and cannot carry one). Runs before any cell-indexed record is looked at.
    public static string ValidateGeometry(ForestSaveData data)
    {
        if (data == null)
            return null;
        if (data.version < 20)
            return data.standGeometryModel == StandGeometryModel.Legacy40
                ? null
                : "a save older than version 20 cannot carry a stand geometry model";
        // Only the historical slot reaches this validator; new-profile envelopes are refused earlier.
        return ValidateGeometryBinding(data.standGeometryModel, ForestSaveProfiles.Historical);
    }

    // The signed contract reserves the number 2 for Compact30. This is a reservation in a switch, not a
    // world: it names no dimensions, cells or content, and StandGeometryModel.IsKnown stays false for it.
    internal const int ReservedCompactGeometryModel = 2;

    // Exhaustive geometry dispatch. Every value is named here, so adding a model cannot silently fall back
    // to the 40 m helper defaults. Value 2 refuses for every profile until a signed world manifest is
    // bound to it, and none exists. The historical slot only ever accepts 0 and 1.
    internal static string ValidateGeometryBinding(int model, ForestSaveProfiles.Profile profile)
    {
        bool historical = ReferenceEquals(profile, ForestSaveProfiles.Historical);
        switch (model)
        {
            case StandGeometryModel.Legacy40:
            case StandGeometryModel.Enlarged80:
                return historical ? null : $"stand geometry model {model} belongs to the historical save slot";
            case ReservedCompactGeometryModel:
                return historical
                    ? "stand geometry model 2 is reserved for a separately signed world and cannot be used by a historical save"
                    : "stand geometry model 2 is reserved, and no signed world manifest is bound to it";
            default:
                return $"unknown stand geometry model {model}";
        }
    }

    // JsonUtility turns a missing field into 0 (Legacy40). The disk-load path therefore checks that a v20
    // save states its geometry explicitly, as an integer, before parsed state can touch the world.
    public static string ValidateGeometryJson(string json, ForestSaveData data)
    {
        if (data == null || data.version < 20)
            return null;
        try
        {
            var root = JObject.Parse(json, new JsonLoadSettings { DuplicatePropertyNameHandling = DuplicatePropertyNameHandling.Error });
            JToken token = root["standGeometryModel"];
            if (token?.Type != JTokenType.Integer || token.Value<long>() != data.standGeometryModel)
                return "version 20 requires an explicit integer stand geometry model";
        }
        catch (System.Exception)
        {
            return "invalid stand geometry JSON";
        }
        return null;
    }
}
