using System.Globalization;

namespace SitkaP1.Core.Phase1;

/// <summary>
/// The listing header of one save: what a player sees before loading (label, year, identity, model vector, saved UTC) and
/// the save generation counter. The checksum covers the header and the payload together, so a header cannot be swapped
/// onto another payload.
/// </summary>
public sealed record Phase1SaveHeader(string Label, string SavedUtc, long SaveGeneration, int EcologicalYear,
    Phase1ModelVector Models, Phase1GenesisIdentity Genesis)
{
    /// <summary>The only accepted SavedUtc form: second precision, UTC, invariant culture, e.g. 2026-11-24T13:53:37Z.</summary>
    public const string SavedUtcFormat = "yyyy-MM-ddTHH:mm:ssZ";

    public static string FormatUtc(DateTime utc)
    {
        if (utc.Kind != DateTimeKind.Utc) throw new P1Refusal("SaveTimeInvalid", "the saved time must be a UTC time");
        return utc.ToString(SavedUtcFormat, CultureInfo.InvariantCulture);
    }
}

/// <summary>One complete save: header and state. Encoding adds the format marker, schema version and checksum.</summary>
public sealed record Phase1SaveEnvelope(Phase1SaveHeader Header, Phase1SaveState State);

/// <summary>
/// A save that passed every decode gate: size, strict JSON, exact schema shape, canonical bytes, checksum, consistency and
/// (when an expectation was given) exact model and genesis identity. <see cref="Checksum"/> is the verified checksum hex.
/// </summary>
public sealed record Phase1DecodedSave(Phase1SaveEnvelope Envelope, string Checksum, int ByteLength);
