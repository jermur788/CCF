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
        return StandGeometryModel.IsKnown(data.standGeometryModel)
            ? null
            : $"unknown stand geometry model {data.standGeometryModel}";
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
