using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

// Compact30 data fixtures and the pure rules that decide whether one of the four fixed G1 sites is
// legal. This file deliberately uses no UnityEngine types so the same rules can be compiled and
// exercised outside the Editor.
//
// Status of the data: E2 CANDIDATE FIXTURE. The 175 rows reproduce the #676 candidate roster digest
// exactly, and the four site positions are the #813 E1 correction_20261119 values. Neither is a
// signed production world; the owner's later attestation and the exact signed coordinates remain
// outstanding. The code refuses to pretend otherwise: the roster digest is verified at load and the
// site clearance is recomputed from the path anchors.

[Serializable]
public sealed class Compact30TreeRow
{
    public string id;
    public string species;
    public float x_m;
    public float z_m;
    public int age_y;
    public float dbh_cm;
    public float height_m;
    public float crown_m;
}

[Serializable]
public sealed class Compact30RosterFile
{
    public string schema;
    public string status;
    public string source;
    public string roster_sha256;
    public int count;
    public List<Compact30TreeRow> rows = new List<Compact30TreeRow>();
}

[Serializable]
public sealed class Compact30AnchorRow
{
    public float x_m;
    public float z_m;
}

[Serializable]
public sealed class Compact30PathRow
{
    public List<Compact30AnchorRow> anchors = new List<Compact30AnchorRow>();
    public float half_width_m;
    public float verge_each_side_m;
}

[Serializable]
public sealed class Compact30H1Row
{
    public float rect_x_min;
    public float rect_x_max;
    public float rect_z_min;
    public float rect_z_max;
}

[Serializable]
public sealed class Compact30SiteRow
{
    public string id;
    public string species;
    public string stock_item;
    public float x_m;
    public float z_m;
    public int cell;
    public float clearance_m;
    public string label;
}

[Serializable]
public sealed class Compact30SiteFile
{
    public string schema;
    public string status;
    public string source;
    public List<Compact30SiteRow> sites = new List<Compact30SiteRow>();
    public Compact30PathRow path = new Compact30PathRow();
    public Compact30H1Row H1 = new Compact30H1Row();
    public float shelter_footprint_radius_m = 0.6f;
    public float stem_clearance_m = 0.7f;
    public float site_separation_m = 0.5f;
}

public struct Compact30Point
{
    public float x;
    public float z;
    public Compact30Point(float x, float z) { this.x = x; this.z = z; }
}

public static class Compact30Rules
{
    public const float StandSizeMeters = 30f;
    public const int CellsPerAxis = 6;
    public const int CellCount = CellsPerAxis * CellsPerAxis;
    public const float CellSizeMeters = 5f;
    public const int ExpectedRosterCount = 175;
    public const string ExpectedRosterSha256 = "dac06a3b7775f884fac9f8df6b95178cbaf9d912b44687082a161eaf0426cca1";

    public static int CellIndex(float x, float z)
    {
        float origin = -StandSizeMeters * 0.5f;
        int cx = (int)Math.Floor((x - origin) / CellSizeMeters);
        int cz = (int)Math.Floor((z - origin) / CellSizeMeters);
        if (cx < 0 || cz < 0 || cx >= CellsPerAxis || cz >= CellsPerAxis)
            return -1;
        return cz * CellsPerAxis + cx;
    }

    public static bool InsideStand(float x, float z, float margin = 0f)
    {
        float half = StandSizeMeters * 0.5f - margin;
        return x >= -half && x < half && z >= -half && z < half;
    }

    public static float DistanceToSegment(Compact30Point p, Compact30Point a, Compact30Point b)
    {
        float dx = b.x - a.x;
        float dz = b.z - a.z;
        float lengthSquared = dx * dx + dz * dz;
        float t = lengthSquared <= 0f ? 0f
            : Math.Max(0f, Math.Min(1f, ((p.x - a.x) * dx + (p.z - a.z) * dz) / lengthSquared));
        float nx = a.x + t * dx;
        float nz = a.z + t * dz;
        return (float)Math.Sqrt((p.x - nx) * (p.x - nx) + (p.z - nz) * (p.z - nz));
    }

    public static float DistanceToPath(Compact30Point p, Compact30PathRow path)
    {
        if (path == null || path.anchors == null || path.anchors.Count == 0)
            return float.NegativeInfinity;
        if (path.anchors.Count == 1)
            return DistanceToSegment(p, ToPoint(path.anchors[0]), ToPoint(path.anchors[0]));
        float best = float.MaxValue;
        for (int i = 0; i < path.anchors.Count - 1; i++)
        {
            float distance = DistanceToSegment(p, ToPoint(path.anchors[i]), ToPoint(path.anchors[i + 1]));
            if (distance < best) best = distance;
        }
        return best;
    }

    private static Compact30Point ToPoint(Compact30AnchorRow row) => new Compact30Point(row.x_m, row.z_m);

    // Clearance rule from the #813 correction: centre-to-polyline distance minus the rendered path
    // envelope (0.8 m) and the shelter footprint (0.6 m). Negative means the site cannot carry a
    // sheltered tree without intruding on the pedestrian route.
    public static float PathClearance(Compact30Point p, Compact30SiteFile data)
    {
        if (data == null) return float.NegativeInfinity;
        float envelope = data.path != null ? data.path.half_width_m : 0.8f;
        return DistanceToPath(p, data.path) - envelope - data.shelter_footprint_radius_m;
    }

    public static bool InsideH1(float x, float z, Compact30H1Row h1, float margin)
    {
        if (h1 == null) return false;
        return x >= h1.rect_x_min - margin && x <= h1.rect_x_max + margin
            && z >= h1.rect_z_min - margin && z <= h1.rect_z_max + margin;
    }

    // A Compact site is legal only if every one of these holds. The caller has already checked the
    // player's teaching gates, stock and the group's 2+2 slot rules.
    public static bool ValidateSite(Compact30SiteRow site, Compact30SiteFile data,
        IReadOnlyList<Compact30Point> livingStems, IReadOnlyList<Compact30Point> occupiedSites,
        out string reason)
    {
        reason = "";
        if (site == null || data == null) { reason = "Site data is missing."; return false; }
        if (string.IsNullOrEmpty(site.id)) { reason = "Site has no id."; return false; }
        if (site.species != "sessile-oak" && site.species != "beech")
        { reason = site.id + ": species is not sessile oak or European beech."; return false; }
        string expectedStock = site.species == "sessile-oak" ? "sessile-oak-sapling" : "beech-sapling";
        if (site.stock_item != expectedStock)
        { reason = site.id + ": stock item does not match the fixed species."; return false; }
        if (float.IsNaN(site.x_m) || float.IsInfinity(site.x_m) || float.IsNaN(site.z_m) || float.IsInfinity(site.z_m))
        { reason = site.id + ": position is not finite."; return false; }
        if (!InsideStand(site.x_m, site.z_m, data.shelter_footprint_radius_m))
        { reason = site.id + ": shelter footprint crosses the stand edge."; return false; }
        int cell = CellIndex(site.x_m, site.z_m);
        if (cell != site.cell || cell < 0 || cell >= CellCount)
        { reason = site.id + ": stored cell does not match the Compact30 grid."; return false; }
        float clearance = PathClearance(new Compact30Point(site.x_m, site.z_m), data);
        if (clearance < 0f)
        { reason = site.id + ": the shelter footprint intrudes on the pedestrian path."; return false; }
        if (Math.Abs(clearance - site.clearance_m) > 0.05f)
        { reason = site.id + ": path clearance has drifted from the recorded E1 value."; return false; }
        if (InsideH1(site.x_m, site.z_m, data.H1, data.shelter_footprint_radius_m))
        { reason = site.id + ": site is inside the H1 arrival and parking area."; return false; }
        if (livingStems != null)
        {
            foreach (Compact30Point stem in livingStems)
            {
                float dx = stem.x - site.x_m;
                float dz = stem.z - site.z_m;
                if (Math.Sqrt(dx * dx + dz * dz) < data.stem_clearance_m)
                { reason = site.id + ": too close to a standing tree stem."; return false; }
            }
        }
        if (occupiedSites != null)
        {
            foreach (Compact30Point other in occupiedSites)
            {
                float dx = other.x - site.x_m;
                float dz = other.z - site.z_m;
                if (Math.Sqrt(dx * dx + dz * dz) < data.site_separation_m)
                { reason = site.id + ": another planting position is too close."; return false; }
            }
        }
        return true;
    }

    // The #676 roster digest is over the fixed CSV form, not over this JSON file. Recomputing it at
    // load makes a copied/edited candidate fixture fail loudly instead of quietly changing the world.
    public static string RosterSha256(Compact30RosterFile roster)
    {
        if (roster == null || roster.rows == null) return "";
        var text = new StringBuilder();
        text.Append("ccf-compact30-authored-roster-candidate;v=0-UNSIGNED;");
        text.Append("fields=id,species,x_m,z_m,age_y,dbh_cm,height_m,crown_m;order=id-ascending;lf\n");
        foreach (Compact30TreeRow row in roster.rows)
        {
            text.Append(row.id).Append(',').Append(row.species).Append(',')
                .Append(row.x_m.ToString("0.000", CultureInfo.InvariantCulture)).Append(',')
                .Append(row.z_m.ToString("0.000", CultureInfo.InvariantCulture)).Append(',')
                .Append(row.age_y.ToString(CultureInfo.InvariantCulture)).Append(',')
                .Append(row.dbh_cm.ToString("0.00", CultureInfo.InvariantCulture)).Append(',')
                .Append(row.height_m.ToString("0.00", CultureInfo.InvariantCulture)).Append(',')
                .Append(row.crown_m.ToString("0.000", CultureInfo.InvariantCulture)).Append('\n');
        }
        using (SHA256 sha = SHA256.Create())
        {
            byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(text.ToString()));
            var hex = new StringBuilder(hash.Length * 2);
            foreach (byte b in hash) hex.Append(b.ToString("x2"));
            return hex.ToString();
        }
    }

    public static bool RosterIsValid(Compact30RosterFile roster, out string reason)
    {
        reason = "";
        if (roster == null || roster.rows == null || roster.rows.Count != ExpectedRosterCount)
        { reason = "Compact30 roster must contain exactly 175 rows."; return false; }
        var ids = new HashSet<string>(StringComparer.Ordinal);
        var previous = "";
        foreach (Compact30TreeRow row in roster.rows)
        {
            if (row == null || string.IsNullOrEmpty(row.id)) { reason = "Roster row has no id."; return false; }
            if (!ids.Add(row.id)) { reason = "Duplicate roster id " + row.id + "."; return false; }
            if (string.CompareOrdinal(previous, row.id) >= 0)
            { reason = "Roster ids are not strictly ascending at " + row.id + "."; return false; }
            previous = row.id;
            if (row.species != "sitka-spruce") { reason = row.id + " is not Sitka spruce."; return false; }
            if (!InsideStand(row.x_m, row.z_m)) { reason = row.id + " is outside the Compact30 bounds."; return false; }
            if (row.height_m <= 0f || row.dbh_cm <= 0f || row.crown_m <= 0f)
            { reason = row.id + " has a non-positive dimension."; return false; }
        }
        string digest = RosterSha256(roster);
        if (digest != ExpectedRosterSha256)
        { reason = "Compact30 roster digest mismatch: " + digest + "."; return false; }
        return true;
    }
}
