using UnityEngine;

// Fixed starting-world infrastructure. The 0.15 m guard is for stem centres,
// not extra visible track width. This is authored geometry, not save state.
public static class ScenarioOneHeadlandGeometry
{
    public const float HeadlandX = -35.4f;
    public const float SouthZ = -37.7f;
    public const float NorthZ = 37.7f;
    public const float FeederZ = -27.4f;
    public const float PathX = -2.436166f;
    public const float Width = 2.6f;
    public const float PadRadius = 2.2f;
    public const float StemGuard = 0.15f;

    public static bool ContainsStemCentre(Vector3 position)
    {
        float stripRadius = Width * 0.5f + StemGuard;
        float padRadius = PadRadius + StemGuard;
        return DistanceSquared(position.x, position.z, HeadlandX, SouthZ, HeadlandX, NorthZ) <= stripRadius * stripRadius
            || DistanceSquared(position.x, position.z, HeadlandX, FeederZ, PathX, FeederZ) <= stripRadius * stripRadius
            || DistanceSquared(position.x, position.z, HeadlandX, FeederZ, HeadlandX, FeederZ) <= padRadius * padRadius
            || DistanceSquared(position.x, position.z, PathX, FeederZ, PathX, FeederZ) <= padRadius * padRadius;
    }

    private static float DistanceSquared(float x, float z, float ax, float az, float bx, float bz)
    {
        float dx = bx - ax, dz = bz - az;
        float lengthSquared = dx * dx + dz * dz;
        float t = lengthSquared > 0f ? Mathf.Clamp01(((x - ax) * dx + (z - az) * dz) / lengthSquared) : 0f;
        float ex = x - (ax + t * dx), ez = z - (az + t * dz);
        return ex * ex + ez * ez;
    }
}
