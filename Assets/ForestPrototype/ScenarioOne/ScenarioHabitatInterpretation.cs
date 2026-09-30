using System.Linq;

// Read-only Century Review language. Habitat signals are interpretations of
// observed structure, not species sightings or new completion requirements.
public static class ScenarioHabitatInterpretation
{
    public static string Describe(ScenarioEcologicalSnapshot start, ScenarioEcologicalSnapshot now,
        float oldWoodlandSourceConfidence)
    {
        if (now == null) return "No habitat observation is available.";
        int sitka = now.species?.FirstOrDefault(s => s.speciesId == "sitka-spruce")?.livingTrees ?? 0;
        int broadleaf = now.species?.Where(s => s.speciesId == "beech" || s.speciesId == "sessile-oak")
            .Sum(s => s.livingTrees) ?? 0;
        string canopy = sitka > 0
            ? $"{sitka} Sitka still give this stand a conifer canopy identity; {broadleaf} Oak/Beech add mixed-woodland structure."
            : $"The living stand has {broadleaf} Oak/Beech; original conifer canopy is no longer present.";
        string floor = start != null && now.meanFerns + now.meanGrasses + now.meanShrubs
            > start.meanFerns + start.meanGrasses + start.meanShrubs + 0.04f
            ? "Gaps now support a more varied fern/grass/shrub-type floor alongside shaded moss and litter."
            : "The observed ground layer remains sparse or shade-dominated; suitable light alone does not establish old-woodland plants.";
        string logs = now.deadwoodCount > 0
            ? $"{now.deadwoodCount} fallen logs provide changing decomposer and moss/fungi-type substrates as they decay."
            : "No retained fallen logs are recorded as coarse-wood substrate.";
        string continuity = oldWoodlandSourceConfidence <= 0f
            ? "Site soil, moisture and connection to older woodland are unverified: no slow-colonising ancient-woodland flora is inferred."
            : "Possible older-woodland sources are a visual cue only; no specialist plant species is claimed without a site survey.";
        return canopy + " " + floor + " " + logs + " " + continuity;
    }
}
