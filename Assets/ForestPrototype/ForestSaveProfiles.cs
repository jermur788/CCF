using System;
using System.Collections.Generic;
using System.IO;

// Fixed routing identities, not world definitions. Adding an authored world requires a separate
// approved binding and lifecycle implementation; a reserved filename never makes it playable.
public static class ForestSaveProfiles
{
    public const string ScenarioOneChapterId = "scenario-one";
    public const string ScenarioTwoChapterId = "scenario-two";
    public const string HistoricalProfileId = "o1-historical";
    public const string CompactProfileId = "o1-compact-v1";
    public const string ScenarioTwoProfileId = "o2-working-mixed-v1";

    public const string HistoricalLearningPrefix = "CCF.Learning.v1.";
    public const string HistoricalMenuHelpPrefix = "CCF.MenuHelp.v1.";
    public const string CompactLearningPrefix = "CCF.Learning.scen1-compact-v1.";
    public const string CompactMenuHelpPrefix = "CCF.MenuHelp.scen1-compact-v1.";
    public const string ScenarioTwoLearningPrefix = "CCF.Learning.scenario-two-v1.";
    public const string ScenarioTwoMenuHelpPrefix = "CCF.MenuHelp.scenario-two-v1.";

    // Global accessibility preference, outside every profile's learning/help reset namespace.
    // This inventories the additive Objective 1 key without importing its UI implementation.
    public const string ReduceMotionPreferenceKey = "CCF.UI.ReduceMotion";

    public sealed class Profile
    {
        public string ChapterId { get; }
        public string ProfileId { get; }
        public string WorldId { get; }
        public string GeometryKey { get; }
        public string ContentKey { get; }
        public string PrimaryFileName { get; }
        public string LearningPreferencePrefix { get; }
        public string MenuHelpPreferencePrefix { get; }
        public bool IsAvailable { get; }
        public string UnavailableReason { get; }

        internal Profile(string chapterId, string profileId, string worldId, string geometryKey,
            string contentKey, string primaryFileName, string learningPrefix, string menuHelpPrefix,
            bool isAvailable, string unavailableReason)
        {
            ChapterId = chapterId;
            ProfileId = profileId;
            WorldId = worldId;
            GeometryKey = geometryKey;
            ContentKey = contentKey;
            PrimaryFileName = primaryFileName;
            LearningPreferencePrefix = learningPrefix;
            MenuHelpPreferencePrefix = menuHelpPrefix;
            IsAvailable = isAvailable;
            UnavailableReason = unavailableReason;
        }
    }

    // Historical world/geometry/content meanings remain in the existing definition and save data.
    // Nulls here deliberately avoid inventing or persisting new historical identity fields.
    public static readonly Profile Historical = new Profile(
        ScenarioOneChapterId, HistoricalProfileId, null, null, null, "forest-save.json",
        HistoricalLearningPrefix, HistoricalMenuHelpPrefix, true, null);

    public static readonly Profile Compact = new Profile(
        ScenarioOneChapterId, CompactProfileId, "scenario-one-compact-v1", "compact30-v1",
        "scen1-compact-h1-g1-v1", "forest-save-scenario-one-compact-v1.json",
        CompactLearningPrefix, CompactMenuHelpPrefix, false,
        "Compact Scenario One is unavailable: no signed authored world binding is installed.");

    public static readonly Profile ScenarioTwo = new Profile(
        ScenarioTwoChapterId, ScenarioTwoProfileId, "scenario-two-working-mixed-v1",
        "o2-working-mixed-geometry-v1", "o2-working-mixed-content-v1", "forest-save-scenario-two.json",
        ScenarioTwoLearningPrefix, ScenarioTwoMenuHelpPrefix, false,
        "Scenario Two is unavailable: no signed authored world binding is installed.");

    // Enumeration is for lookup, not presentation order or first-demo promotion.
    public static IReadOnlyList<Profile> All { get; } = Array.AsReadOnly(new[] { Historical, Compact, ScenarioTwo });

    public static bool TryGet(string profileId, out Profile profile)
    {
        foreach (Profile candidate in All)
        {
            if (string.Equals(candidate.ProfileId, profileId, StringComparison.Ordinal))
            {
                profile = candidate;
                return true;
            }
        }
        profile = null;
        return false;
    }

    public static bool CanUse(Profile profile, out string reason)
    {
        // Accept only the compile-time records, never a caller-constructed lookalike.
        if (!IsRegistered(profile))
        {
            reason = "Unknown save profile.";
            return false;
        }
        reason = profile.UnavailableReason;
        return profile.IsAvailable;
    }

    public static ForestSavePaths GetPaths(Profile profile, string persistentDataPath)
    {
        if (!IsRegistered(profile))
            throw new ArgumentException("Unknown save profile.", nameof(profile));
        if (string.IsNullOrEmpty(persistentDataPath))
            throw new ArgumentException("A persistent data directory is required.", nameof(persistentDataPath));
        // This pure query also exposes reserved paths for inspection; it grants no operation permission.
        return new ForestSavePaths(Path.Combine(persistentDataPath, profile.PrimaryFileName));
    }

    // SI-09 namespaces. Only a registered, usable profile gets prefixes, so an unavailable reserved
    // profile can never read, write or reset its keys. Historical prefixes stay exactly as before.
    public static bool TryGetPreferencePrefixes(Profile profile, out string learningPrefix, out string menuHelpPrefix,
        out string reason)
    {
        learningPrefix = null;
        menuHelpPrefix = null;
        if (!CanUse(profile, out reason)) return false;
        learningPrefix = profile.LearningPreferencePrefix;
        menuHelpPrefix = profile.MenuHelpPreferencePrefix;
        reason = null;
        return true;
    }

    // For enumerated resets: true only for a key inside this profile's own learning/help namespace.
    // Other profiles' keys and the global ReduceMotion key are never owned.
    public static bool OwnsPreferenceKey(Profile profile, string key)
    {
        if (!IsRegistered(profile) || string.IsNullOrEmpty(key)) return false;
        return key.StartsWith(profile.LearningPreferencePrefix, StringComparison.Ordinal)
            || key.StartsWith(profile.MenuHelpPreferencePrefix, StringComparison.Ordinal);
    }

    private static bool IsRegistered(Profile profile)
    {
        foreach (Profile candidate in All)
            if (ReferenceEquals(candidate, profile)) return true;
        return false;
    }
}

public sealed class ForestSavePaths
{
    public string Primary { get; }
    public string Temporary => Primary + ".tmp";
    public string Backup => Primary + ".bak";

    internal ForestSavePaths(string primary) { Primary = primary; }
}
