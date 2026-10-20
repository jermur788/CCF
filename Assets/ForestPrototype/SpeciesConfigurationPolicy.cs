using System;
using System.Collections.Generic;

// Explicit species configuration (Scenario Two species-model foundation, SCENARIO-TWO-SPECIES-MODEL-FOUNDATION-V1).
//
// WHY THIS EXISTS. A TreeSpeciesDefinition that leaves a field untouched keeps the field's C# default, and several of
// those defaults are Sitka behaviour: growingSpaceDemand = Reference routes a pure stand through the British Sitka
// size-density line, browsePalatability = 0 makes the species browse-proof, and the seed kernel, maturity ramp, mast
// regime, crown envelope and juvenile light arrays are all Sitka-like. A new species that is only partly filled in
// would therefore run, but as a disguised Sitka. This file lets a definition say, field area by field area, whether
// the values are real, a labelled placeholder, or deliberately on HOLD, and refuses to treat an undeclared area as
// ready.
//
// WHAT IT DOES NOT DO. It adds no ecological value and no number. It never changes a Sitka, Oak or Beech result: those
// three Scenario One species are named below as established and are not asked to declare anything, because their
// serialized assets pre-date this file and the accepted Scenario One behaviour (and its deterministic anchors) must not
// move. The Sitka asset, for example, does not serialize growingSpaceDemand at all - its Reference class is the C#
// default - so the default itself must never be changed; the rule below is keyed on the species id instead.
//
// No UnityEngine types: the policy runs and is verified outside the Editor (see
// Tools/Verification/ScenarioTwoSpeciesFoundation/).

// How a field area was filled in. Unspecified is the default and is never ready for a species that is not established.
public enum SpeciesParameterState
{
    // Nothing was declared. A new species in this state is NOT usable: it would be running on inherited defaults.
    Unspecified = 0,
    // Accepted values for this area (for example a value the project specification sets directly).
    Set = 1,
    // A deliberate stand-in inside a stated evidence bracket. Visible as a placeholder in the parameter document;
    // never a claim of measurement.
    Placeholder = 2,
    // Deliberately not set because the evidence cannot support a number yet. The species is not simulation-ready.
    Hold = 3,
}

// How a species' seed reaches the ground in the simulation. None is planting-only: the species can be planted but
// produces no natural seed rain.
public enum NaturalSeedDispersal
{
    // Nothing declared. Established Scenario One species keep their existing wind-kernel behaviour; any other species
    // does not disperse at all (fail-safe).
    Unspecified = 0,
    // Planting only. No seed rain is computed for this species.
    None = 1,
    // Wind kernel (the existing exponential seed-rain kernel).
    Wind = 2,
    // Wind kernel plus a declared water-dispersal CAPABILITY. The simulation has no water or spatial carrier, so
    // water propagation is not implemented (SpeciesConfigurationPolicy.WaterDispersalPropagationImplemented is false):
    // the capability is recorded and exposed, and no seed is moved by water.
    WindAndWater = 3,
}

// One age phase of adult growth. A species with phases multiplies its potential DBH and height increments by the
// factors of the phase its tree age falls in. The factors are RELATIVE to the species' own potential increments: they
// are not growth rates, and no phase table is supplied by any species asset yet.
[Serializable]
public struct GrowthAgePhase
{
    // First tree age (years) at which this phase applies. The first phase must start at 0.
    public float startAgeYears;
    public float dbhIncrementFactor;
    public float heightIncrementFactor;

    public GrowthAgePhase(float startAgeYears, float dbhIncrementFactor, float heightIncrementFactor)
    {
        this.startAgeYears = startAgeYears;
        this.dbhIncrementFactor = dbhIncrementFactor;
        this.heightIncrementFactor = heightIncrementFactor;
    }
}

// The values the policy reads, as plain data, so it can be tested without a ScriptableObject. The initial values
// mirror the TreeSpeciesDefinition field defaults on purpose: a new SpeciesConfigurationFacts with only an id set
// behaves like a minimally defined new species (the offline check asserts the mirror against the source text).
public sealed class SpeciesConfigurationFacts
{
    public string SpeciesId = "";
    public GrowingSpaceDemandClass GrowingSpaceDemand = GrowingSpaceDemandClass.Reference;
    public SpeciesParameterState GrowthState;
    public SpeciesParameterState BrowseState;
    public SpeciesParameterState JuvenileLightState;
    public SpeciesParameterState SeedKernelState;
    public NaturalSeedDispersal SeedDispersal;
    public bool SupportsRegeneration = true;
    public bool UsesDistinctJuvenileLightResponses;
    public float BrowsePalatability;
    public float PotentialDbhGrowthCmPerYear = 0.9f;
    public float PotentialHeightGrowthMPerYear = 0.45f;
    public float MaxDbhCm = 120f;
    public float MaxHeightM = 35f;
    public float CrownRadiusIntercept = SpeciesConfigurationPolicy.DefaultCrownRadiusIntercept;
    public float CrownRadiusPerCmDbh = SpeciesConfigurationPolicy.DefaultCrownRadiusPerCmDbh;
    public bool UsesPowerLawCrown;
    public float MaturityOnsetYears = SpeciesConfigurationPolicy.DefaultMaturityOnsetYears;
    public float MaturityFullYears = SpeciesConfigurationPolicy.DefaultMaturityFullYears;
    public float MastGoodProbability = SpeciesConfigurationPolicy.DefaultMastGoodProbability;
    public float MastPoorProbability = SpeciesConfigurationPolicy.DefaultMastPoorProbability;
    public float MastGoodMultiplier = SpeciesConfigurationPolicy.DefaultMastGoodMultiplier;
    public float MastNormalMultiplier = SpeciesConfigurationPolicy.DefaultMastNormalMultiplier;
    public float MastPoorMultiplier = SpeciesConfigurationPolicy.DefaultMastPoorMultiplier;
    public float SeedDispersalScaleM = 20f;
    public float SeedDispersalCutoffM = 150f;
    public GrowthAgePhase[] AgePhases;
}

public sealed class SpeciesConfigurationReport
{
    // Things that make the definition invalid. A species with errors must not run.
    public readonly List<string> Errors = new List<string>();
    // Areas declared Hold. Valid, but the species is not simulation-ready until they are filled in.
    public readonly List<string> HoldAreas = new List<string>();

    public bool HasErrors => Errors.Count > 0;
    public bool IsReady => Errors.Count == 0 && HoldAreas.Count == 0;
}

public static class SpeciesConfigurationPolicy
{
    // Duplicated on purpose (the dormant site data does the same) so this file needs no Sitka code.
    public const string SitkaSpruceId = "sitka-spruce";
    public const string SessileOakId = "sessile-oak";
    public const string BeechId = "beech";

    // Always false. There is no water or spatial carrier in the simulation, and building one is hydrology, which
    // this foundation does not do. A species may declare the capability; nothing propagates by water.
    public static bool WaterDispersalPropagationImplemented => false;

    // The TreeSpeciesDefinition field defaults that are Sitka behaviour. A species that declares an area Set or
    // Placeholder must not still be sitting on them: that would be inheritance, whatever the state says.
    public const float DefaultCrownRadiusIntercept = 0.9415f;
    public const float DefaultCrownRadiusPerCmDbh = 0.07635f;
    public const float DefaultMaturityOnsetYears = 20f;
    public const float DefaultMaturityFullYears = 30f;
    public const float DefaultMastGoodProbability = 0.15f;
    public const float DefaultMastPoorProbability = 0.35f;
    public const float DefaultMastGoodMultiplier = 1.5f;
    public const float DefaultMastNormalMultiplier = 1f;
    public const float DefaultMastPoorMultiplier = 0.4f;

    private static readonly string[] establishedScenarioOneSpeciesIds = { SitkaSpruceId, SessileOakId, BeechId };

    // The three species whose serialized assets define accepted Scenario One behaviour. They are not asked to declare
    // field states: their values are protected by the deterministic anchors. Every other species must.
    public static bool IsEstablishedScenarioOneSpecies(string speciesId)
    {
        foreach (string established in establishedScenarioOneSpeciesIds)
            if (string.Equals(established, speciesId, StringComparison.Ordinal))
                return true;
        return false;
    }

    // Reference is the Sitka reference class and means "keep the original Sitka size-density route". Only Sitka may
    // hold it, so a non-Sitka pure stand can never be routed through the Sitka line by accident.
    public static bool IsReferenceDemandAllowed(string speciesId) =>
        string.Equals(speciesId, SitkaSpruceId, StringComparison.Ordinal);

    // Allocation-free check for hot paths (species registration): is this one of the established three, whose
    // behaviour is accepted as it stands, with the one rule that applies to EVERY species satisfied?
    public static bool IsEstablishedAndValid(string speciesId, GrowingSpaceDemandClass demand) =>
        IsEstablishedScenarioOneSpecies(speciesId)
        && (demand != GrowingSpaceDemandClass.Reference || IsReferenceDemandAllowed(speciesId));

    // Does a species put seed on the ground by itself? Established species with no declaration keep their existing
    // behaviour (they disperse when supportsRegeneration is on). A species that is not established and has declared
    // nothing never disperses, a planting-only (None) species never does, and a wind species needs a kernel that is
    // Set or Placeholder (never Hold or Unspecified). Water is a declared capability only: no seed is moved by water
    // because the simulation has no water carrier (WaterDispersalPropagationImplemented).
    public static bool DispersesSeedNaturally(string speciesId, NaturalSeedDispersal mode, bool supportsRegeneration,
        SpeciesParameterState seedKernelState)
    {
        if (mode == NaturalSeedDispersal.Unspecified)
            return IsEstablishedScenarioOneSpecies(speciesId) && supportsRegeneration;
        if (!supportsRegeneration || mode == NaturalSeedDispersal.None)
            return false;
        return seedKernelState == SpeciesParameterState.Set || seedKernelState == SpeciesParameterState.Placeholder;
    }

    public static SpeciesConfigurationReport Evaluate(SpeciesConfigurationFacts facts)
    {
        var report = new SpeciesConfigurationReport();
        if (facts == null)
        {
            report.Errors.Add("No species facts were supplied.");
            return report;
        }
        if (string.IsNullOrEmpty(facts.SpeciesId))
            report.Errors.Add("speciesId is empty.");

        // Rules that apply to every species, established or not.
        if (facts.GrowingSpaceDemand == GrowingSpaceDemandClass.Reference && !IsReferenceDemandAllowed(facts.SpeciesId))
            report.Errors.Add("growingSpaceDemand is Reference, which is reserved for " + SitkaSpruceId
                + " (it keeps the Sitka size-density route). Use Intermediate or High as an ordinal placeholder.");
        if (!GrowthAgePhases.TryValidate(facts.AgePhases, out string phaseProblem))
            report.Errors.Add("growthAgePhases: " + phaseProblem);

        if (IsEstablishedScenarioOneSpecies(facts.SpeciesId))
            return report;

        // Rules for every species that is not established: each material area must be declared, never inherited.
        switch (facts.GrowthState)
        {
            case SpeciesParameterState.Unspecified:
                report.Errors.Add("growthState is Unspecified: declare Set, Placeholder or Hold (growth, size bounds, Ci50, crown and form would otherwise be inherited defaults).");
                break;
            case SpeciesParameterState.Hold:
                report.HoldAreas.Add("growth");
                break;
            default:
                if (!(facts.PotentialDbhGrowthCmPerYear > 0f && facts.PotentialHeightGrowthMPerYear > 0f
                      && facts.MaxDbhCm > 1f && facts.MaxHeightM > 1f))
                    report.Errors.Add("growthState is " + facts.GrowthState + " but the growth fields still hold the inert HOLD values.");
                if (!facts.UsesPowerLawCrown && facts.CrownRadiusIntercept == DefaultCrownRadiusIntercept
                    && facts.CrownRadiusPerCmDbh == DefaultCrownRadiusPerCmDbh)
                    report.Errors.Add("growthState is " + facts.GrowthState + " but the crown envelope is still the open-grown British Sitka default; declare a species envelope or keep growth on Hold.");
                break;
        }

        switch (facts.BrowseState)
        {
            case SpeciesParameterState.Unspecified:
                report.Errors.Add("browseState is Unspecified: a default palatability of 0 would make the species browse-proof. Declare Set, Placeholder or Hold.");
                break;
            case SpeciesParameterState.Hold:
                report.HoldAreas.Add("browse susceptibility");
                break;
            case SpeciesParameterState.Placeholder:
                if (!(facts.BrowsePalatability > 0f))
                    report.Errors.Add("browseState is Placeholder but browsePalatability is 0 (browse-proof). Use Set to declare browse-proof deliberately.");
                break;
        }

        switch (facts.JuvenileLightState)
        {
            case SpeciesParameterState.Unspecified:
                report.Errors.Add("juvenileLightState is Unspecified: the legacy (Sitka-calibrated) juvenile light response would be inherited. Declare Set, Placeholder or Hold.");
                break;
            case SpeciesParameterState.Hold:
                report.HoldAreas.Add("juvenile light response");
                break;
            default:
                if (!facts.UsesDistinctJuvenileLightResponses)
                    report.Errors.Add("juvenileLightState is " + facts.JuvenileLightState + " but useDistinctJuvenileLightResponses is off, so the legacy Sitka-calibrated response would still be used.");
                break;
        }

        switch (facts.SeedDispersal)
        {
            case NaturalSeedDispersal.Unspecified:
                report.Errors.Add("naturalSeedDispersal is Unspecified: declare None (planting-only), Wind or WindAndWater.");
                break;
            case NaturalSeedDispersal.None:
                break;
            default:
                if (!facts.SupportsRegeneration)
                    report.Errors.Add("naturalSeedDispersal needs supportsRegeneration (the juvenile layer that receives the seed).");
                switch (facts.SeedKernelState)
                {
                    case SpeciesParameterState.Unspecified:
                        report.Errors.Add("seedKernelState is Unspecified: the Sitka kernel, maturity ramp and mast regime would be inherited. Declare Set, Placeholder or Hold.");
                        break;
                    case SpeciesParameterState.Hold:
                        report.HoldAreas.Add("seed dispersal kernel");
                        break;
                    default:
                        if (!(facts.SeedDispersalScaleM > 1f && facts.SeedDispersalCutoffM > facts.SeedDispersalScaleM))
                            report.Errors.Add("seedKernelState is " + facts.SeedKernelState + " but the kernel fields still hold the inert HOLD values.");
                        if (facts.MaturityOnsetYears == DefaultMaturityOnsetYears && facts.MaturityFullYears == DefaultMaturityFullYears)
                            report.Errors.Add("seedKernelState is " + facts.SeedKernelState + " but the maturity onset and ramp are still the Sitka default (20 / 30 years).");
                        if (facts.MastGoodProbability == DefaultMastGoodProbability && facts.MastPoorProbability == DefaultMastPoorProbability
                            && facts.MastGoodMultiplier == DefaultMastGoodMultiplier && facts.MastNormalMultiplier == DefaultMastNormalMultiplier
                            && facts.MastPoorMultiplier == DefaultMastPoorMultiplier)
                            report.Errors.Add("seedKernelState is " + facts.SeedKernelState + " but the mast regime is still the Sitka default.");
                        break;
                }
                break;
        }
        return report;
    }
}

// Age-phased adult growth. Empty phases mean "no phasing": the species keeps its single fixed increment exactly.
public static class GrowthAgePhases
{
    public static bool HasPhases(GrowthAgePhase[] phases) => phases != null && phases.Length > 0;

    // The factors of the phase that tree age falls in. No phases (every established species) returns 1 and 1.
    // Piecewise constant on purpose: this foundation adds the ability to have more than one phase, not a curve.
    public static void FactorsAt(GrowthAgePhase[] phases, float ageYears, out float dbhFactor, out float heightFactor)
    {
        dbhFactor = 1f;
        heightFactor = 1f;
        if (!HasPhases(phases))
            return;
        GrowthAgePhase current = phases[0];
        for (int i = 1; i < phases.Length; i++)
        {
            if (ageYears < phases[i].startAgeYears)
                break;
            current = phases[i];
        }
        dbhFactor = current.dbhIncrementFactor;
        heightFactor = current.heightIncrementFactor;
    }

    // Phases are valid when absent, or when the first starts at age 0, starts strictly increase and every factor is a
    // finite number that is not negative.
    public static bool TryValidate(GrowthAgePhase[] phases, out string problem)
    {
        problem = "";
        if (!HasPhases(phases))
            return true;
        if (phases[0].startAgeYears != 0f)
        {
            problem = "the first phase must start at age 0.";
            return false;
        }
        for (int i = 0; i < phases.Length; i++)
        {
            GrowthAgePhase phase = phases[i];
            if (i > 0 && !(phase.startAgeYears > phases[i - 1].startAgeYears))
            {
                problem = "phase start ages must strictly increase.";
                return false;
            }
            if (!IsFiniteNonNegative(phase.dbhIncrementFactor) || !IsFiniteNonNegative(phase.heightIncrementFactor))
            {
                problem = "increment factors must be finite and not negative.";
                return false;
            }
        }
        return true;
    }

    private static bool IsFiniteNonNegative(float value) => !float.IsNaN(value) && !float.IsInfinity(value) && value >= 0f;
}
