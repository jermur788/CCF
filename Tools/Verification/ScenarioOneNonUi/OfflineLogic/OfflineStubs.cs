// The one production type the pure rack-area rules reach for that is not pure: the competition term of the game's own ecology. The real
// ForestEcologyController needs Unity, so the offline build uses this stand-in, and RackBayOfflineCheck.SourceParity() fails if the two ever
// disagree (it reads the real source text). The Unity gate then compares the real value with the live competition index.
public static class ForestEcologyController
{
    public const float HegyiCutoffMeters = 8f;

    public static float HegyiTerm(float neighbourDbhCm, float targetDbhCm, float distanceMeters)
    {
        return (neighbourDbhCm / System.Math.Max(1f, targetDbhCm)) / System.Math.Max(0.5f, distanceMeters);
    }
}
