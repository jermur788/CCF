namespace SitkaP1.Core;

/// <summary>
/// The three mutually exclusive P1 source-claim domains (spec §0 B01, D-084). A domain is always carried explicitly and
/// never inferred from stand size, tree IDs or a file name.
/// </summary>
public enum P1Domain
{
    /// <summary>F0: P1-owned synthetic 30 × 30 m experiment. Source-rule inspired; NOT Unity parity; NOT an Irish forecast.</summary>
    DerivedP1ThirtyMetreExperimentV1,
    /// <summary>Later P1-01: legal Unity 40/80 m oracle fixtures. Not available in F0.</summary>
    DerivedUnitySquareParityV1,
    /// <summary>F0: read-only import and inspection of an independently trusted original P0 fixture. Annual advance refused.</summary>
    OriginalPolygonExploratoryV1,
}

public static class P1DomainWire
{
    public static string Name(P1Domain d) => d switch
    {
        P1Domain.DerivedP1ThirtyMetreExperimentV1 => "DerivedP1ThirtyMetreExperimentV1",
        P1Domain.DerivedUnitySquareParityV1 => "DerivedUnitySquareParityV1",
        P1Domain.OriginalPolygonExploratoryV1 => "OriginalPolygonExploratoryV1",
        _ => throw new P1Refusal("UnknownDomain", "unknown domain value"),
    };

    /// <summary>Exact, case-sensitive match of one of the three names. Anything else is refused.</summary>
    public static P1Domain Parse(string? wire) => wire switch
    {
        "DerivedP1ThirtyMetreExperimentV1" => P1Domain.DerivedP1ThirtyMetreExperimentV1,
        "DerivedUnitySquareParityV1" => P1Domain.DerivedUnitySquareParityV1,
        "OriginalPolygonExploratoryV1" => P1Domain.OriginalPolygonExploratoryV1,
        _ => throw new P1Refusal("UnknownDomain", "domain tag is not one of the three P1 domain names"),
    };
}

/// <summary>A refused input or operation. <see cref="Code"/> is a stable machine-readable token used by tests.</summary>
public sealed class P1Refusal : Exception
{
    public string Code { get; }
    public P1Refusal(string code, string message) : base(message) { Code = code; }
}

/// <summary>Fixed disclosure text shown on every scientific output (owner-confirmed wording, spec §0).</summary>
public static class P1Disclosure
{
    public const string Derived30 =
        "SYNTHETIC CLOSED ADULT SITKA EXPERIMENT. NOT AN IRISH FIELD FORECAST. NOT UNITY NUMERICAL OR GEOMETRY PARITY.";
    public const string Original =
        "ORIGINAL P0 SYNTHETIC FIXTURE, READ-ONLY INSPECTION. NOT AN IRISH FIELD FORECAST. ANNUAL ADVANCE AND WORK REFUSED.";
}
