using System;
using UnityEngine;

// Selection is distinct from admission or a successful load. T-03 can create a fresh context
// for a staged destination, select it before generation, then seal it at that boundary.
// No file or preference access, scene creation, or active-world publication happens here.
public sealed class ForestSaveSessionContext
{
    public ForestSaveProfiles.Profile SelectedProfile { get; private set; }
    public bool WorldCreationStarted { get; private set; }

    public static ForestSaveSessionContext Current { get; private set; } = CreateInitialSelection();

    // Both existing filename consumers use this one selected-profile query.
    public static ForestSavePaths CurrentPaths => Current.GetSelectedPaths(Application.persistentDataPath);

    public bool TrySelect(string profileId, out string reason)
    {
        if (WorldCreationStarted)
        {
            reason = "The save profile must be selected before world creation.";
            return false;
        }
        if (!ForestSaveProfiles.TryGet(profileId, out ForestSaveProfiles.Profile profile)
            || !ForestSaveProfiles.CanUse(profile, out reason))
        {
            reason = profile != null ? profile.UnavailableReason : "Unknown save profile.";
            return false;
        }
        SelectedProfile = profile;
        reason = null;
        return true;
    }

    public bool TryBeginWorldCreation(out string reason)
    {
        if (WorldCreationStarted)
        {
            reason = "World creation has already started for this context.";
            return false;
        }
        if (!ForestSaveProfiles.CanUse(SelectedProfile, out reason)) return false;
        WorldCreationStarted = true;
        return true;
    }

    // Construction gate for code that builds a world (ScenarioOneManager.Awake today). Pure: it reads
    // only this context. A world may be built only after its profile has been sealed (BeforeSceneLoad,
    // or a later staged context) and only for that profile's chapter.
    public static bool TryAuthorizeWorldConstruction(string chapterId, out string reason)
        => Current.TryAuthorizeConstruction(chapterId, out reason);

    public bool TryAuthorizeConstruction(string chapterId, out string reason)
    {
        if (!WorldCreationStarted)
        {
            reason = "The save profile was not sealed before world creation.";
            return false;
        }
        if (!ForestSaveProfiles.CanUse(SelectedProfile, out reason)) return false;
        if (!string.Equals(SelectedProfile.ChapterId, chapterId, StringComparison.Ordinal))
        {
            reason = "The scenario being built does not match the selected save profile.";
            return false;
        }
        reason = null;
        return true;
    }

    // SI-09: learning/help preference keys belong to the selected profile's own namespace.
    public bool TryGetPreferencePrefixes(out string learningPrefix, out string menuHelpPrefix, out string reason)
        => ForestSaveProfiles.TryGetPreferencePrefixes(SelectedProfile, out learningPrefix, out menuHelpPrefix, out reason);

    public ForestSavePaths GetSelectedPaths(string persistentDataPath)
    {
        if (!ForestSaveProfiles.CanUse(SelectedProfile, out string reason))
            throw new InvalidOperationException(reason);
        return ForestSaveProfiles.GetPaths(SelectedProfile, persistentDataPath);
    }

    private static ForestSaveSessionContext CreateInitialSelection()
    {
        var context = new ForestSaveSessionContext();
        ForestSaveProfiles.Profile onlyAvailable = null;
        foreach (ForestSaveProfiles.Profile profile in ForestSaveProfiles.All)
        {
            if (!ForestSaveProfiles.CanUse(profile, out _)) continue;
            // Multiple bound profiles will require an explicit choice through the T-03 bootstrap.
            if (onlyAvailable != null) return context;
            onlyAvailable = profile;
        }
        if (onlyAvailable != null) context.TrySelect(onlyAvailable.ProfileId, out _);
        return context;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetInitialSelection()
    {
        // Also reset when Editor domain reload is disabled; never inherit a previous Play session.
        Current = CreateInitialSelection();
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void BeginInitialWorldCreation()
    {
        // BeforeSceneLoad runs before scene Awake, unlike the existing AfterSceneLoad menu.
        // Only historical O1 is bound in T-01. This seals identity, not a staged-load transaction.
        if (!Current.TryBeginWorldCreation(out string reason))
            throw new InvalidOperationException(reason);
    }
}
