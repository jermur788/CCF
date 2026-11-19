namespace SitkaP1.Core;

/// <summary>The pure-adult source route, detached from Unity objects and from the committed experiment state.</summary>
internal static class AnnualRules
{
    private const float HegyiCutoffM = 8f;
    private const float HegyiDistanceFloorM = 0.5f;
    private const float Ci50 = 5f;
    private const float PotentialDbhGrowthCm = 1.2f;
    private const float DbhGrowthScaleCm = 120f;
    private const float CrownRelaxation = 0.15f;

    internal static AnnualReport Calculate(AnnualState state, int simulationSeed, float areaHa)
    {
        int year = checked(state.EcologicalYear + 1);
        var before = state.Trees.Where(tree => tree.IsLiving).OrderBy(tree => tree.TreeId, StringComparer.Ordinal).ToArray();
        var competition = before.Select(tree => Competition(tree, before)).ToArray();
        var grown = before.Select((tree, i) => Grow(tree, competition[i].Ci)).ToArray();
        var postGrowth = Stand(grown, areaHa);
        double pressure = SitkaGrowthModel.DensityPressure(postGrowth.RelativeDensity);

        var audit = new AnnualTreeAudit[before.Length];
        var deaths = new List<AnnualDeath>();
        var survivors = new List<int>();
        double? meanVulnerability = null;
        double[]? suppression = null;
        if (pressure > 0.0)
        {
            suppression = competition.Select(entry => 1.0 - 1.0 / (1.0 + entry.Ci / Ci50)).ToArray();
            double sum = 0.0;
            foreach (double value in suppression) sum += SitkaGrowthModel.Vulnerability(value);
            meanVulnerability = sum / suppression.Length;
        }

        for (int i = 0; i < before.Length; i++)
        {
            var (ci, neighbours) = competition[i];
            double? s = suppression?[i];
            double? probability = s is double value
                ? SitkaGrowthModel.AnnualDeathProbability(pressure, value, meanVulnerability!.Value) : null;
            float? roll = null;
            if (probability > 0.0)
                roll = SimulationRandom.Roll(DerivedManifest.RngModelVersion, "ADULT-MORT-" + before[i].TreeId,
                    year, simulationSeed);

            var after = grown[i];
            if (roll < probability)
            {
                var death = new AnnualDeath(after.TreeId, year, "self-thinning", "hazard");
                deaths.Add(death);
                after = after with { Death = death };
            }
            else
                survivors.Add(i);
            audit[i] = new AnnualTreeAudit(before[i], after, neighbours, ci, s, probability, roll);
        }

        if (suppression is not null)
            ApplyBoundary(audit, survivors, areaHa, year, deaths, suppression);
        return new AnnualReport(state.EcologicalYear, year, Stand(before, areaHa), postGrowth,
            Stand(audit.Select(row => row.After).Where(tree => tree.IsLiving), areaHa), pressure, meanVulnerability,
            Array.AsReadOnly(audit.Concat(state.Trees.Where(tree => !tree.IsLiving)
                .Select(tree => new AnnualTreeAudit(tree, tree with { Death = tree.Death }, 0, 0f, null, null, null)))
                .OrderBy(row => row.Before.TreeId, StringComparer.Ordinal).ToArray()), deaths.AsReadOnly());
    }

    private static (float Ci, int Neighbours) Competition(AnnualTree target, AnnualTree[] trees)
    {
        float ci = 0f;
        int neighbours = 0;
        foreach (var tree in trees)
        {
            if (!tree.IsLiving || tree.TreeId == target.TreeId) continue;
            float dx = target.XM - tree.XM;
            float dz = target.ZM - tree.ZM;
            float distance = (float)Math.Sqrt(dx * dx + dz * dz);
            if (distance > HegyiCutoffM) continue;
            ci += (tree.DbhCm / MathF.Max(1f, target.DbhCm)) / MathF.Max(HegyiDistanceFloorM, distance);
            neighbours++;
        }
        return (ci, neighbours);
    }

    internal static AnnualTree Grow(AnnualTree tree, float ci)
    {
        if (!tree.IsLiving) return tree;
        // ForestEcologyController.GrowAdults and RelaxCrowns, for the unpruned Sitka asset.
        float response = 1f / (1f + ci / Ci50);
        float site = F32.FromBits(DerivedManifest.SiteProductivityBits);
        float dbhPotential = PotentialDbhGrowthCm * site * Math.Clamp(1f - tree.DbhCm / DbhGrowthScaleCm, 0f, 1f);
        float dbh = Math.Clamp(tree.DbhCm + dbhPotential * response, 1f, 200f);
        float heightGrowth = SitkaGrowthModel.NextHeight(tree.HeightM, tree.AgeYears) - tree.HeightM;
        float height = Math.Clamp(tree.HeightM + heightGrowth, 0.1f, 60f);
        float potential = MathF.Max(CcfSourcePins.CrownRadiusFloorM,
            CcfSourcePins.CrownRadiusIntercept + CcfSourcePins.CrownRadiusPerCmDbh * dbh);
        float target = potential * response;
        float radius = MathF.Max(CcfSourcePins.CrownRadiusFloorM,
            tree.CrownRadiusM + (target - tree.CrownRadiusM) * CrownRelaxation);
        return tree with { AgeYears = checked(tree.AgeYears + 1), DbhCm = dbh, HeightM = height, CrownRadiusM = radius };
    }

    private static AnnualStand Stand(IEnumerable<AnnualTree> living, float areaHa)
    {
        int count = 0;
        double squaredDbhSum = 0.0;
        foreach (var tree in living)
        {
            count++;
            squaredDbhSum += (double)tree.DbhCm * tree.DbhCm;
        }
        if (count == 0) return new AnnualStand(0, 0, 0, 0, 0);

        // The controller divides int by float before widening stems/ha to double.
        double stemsPerHa = count / areaHa;
        double dq = Math.Sqrt(squaredDbhSum / count);
        double basalAreaPerHa = squaredDbhSum * (Math.PI / 40000.0) / areaHa;
        return new AnnualStand(count, stemsPerHa, basalAreaPerHa, dq, SitkaGrowthModel.RelativeDensity(stemsPerHa, dq));
    }

    private static void ApplyBoundary(AnnualTreeAudit[] audit, List<int> survivors, float areaHa, int year,
        List<AnnualDeath> deaths, double[] suppression)
    {
        int count = survivors.Count;
        if (count <= 1) return;
        double squaredDbhSum = 0.0;
        foreach (int i in survivors)
            squaredDbhSum += (double)audit[i].After.DbhCm * audit[i].After.DbhCm;
        double RelativeDensity() => SitkaGrowthModel.RelativeDensity(count / areaHa, Math.Sqrt(squaredDbhSum / count));

        if (RelativeDensity() <= SitkaGrowthModel.MaximumRelativeDensity) return;
        // The audit is already in ordinal ID order, which is the second sort key.
        foreach (int i in survivors.OrderByDescending(i => suppression[i]).ThenBy(i => i))
        {
            if (count <= 1 || RelativeDensity() <= SitkaGrowthModel.MaximumRelativeDensity) break;
            var tree = audit[i].After;
            count--;
            squaredDbhSum -= (double)tree.DbhCm * tree.DbhCm;
            var death = new AnnualDeath(tree.TreeId, year, "self-thinning", "boundary");
            deaths.Add(death);
            audit[i] = audit[i] with { After = tree with { Death = death } };
        }
    }
}
