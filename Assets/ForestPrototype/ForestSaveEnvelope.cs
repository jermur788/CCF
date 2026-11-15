using System;
using System.Collections.Generic;

// New-profile DATA DEFINITIONS ONLY (signed save-isolation contract 4.2).
// These types are not admission evidence. T-02 must check exact raw keys/types/duplicates,
// identity, versions and payload content before deserialised values can authorise anything.
// Historical ForestSaveData and its serializer remain unchanged.
[Serializable]
public sealed class ForestSaveEnvelope
{
    public ForestSaveEnvelopeIdentity ccfSaveEnvelope;
    public ForestSaveData ccfWorldPayload;
    // Compact object; explicit null for O2. Never present in a historical disk root.
    public ForestCompactTeachingEvidence ccfCompactTeaching;
}

[Serializable]
public sealed class ForestSaveEnvelopeIdentity
{
    public const int SupportedSchemaVersion = 1;
    public const int SupportedPayloadVersion = 24;

    public int schemaVersion;
    public string chapterId;
    public string profileId;
    public string worldId;
    public string geometryKey;
    public string contentKey;
    public int payloadVersion;
}

// Representation only. CG-TRIG remains unsigned; no acknowledgement, group designation,
// completion predicate or interaction is implemented or inferred from these fields.
[Serializable]
public sealed class ForestCompactTeachingEvidence
{
    public const int SupportedEvidenceVersion = 1;

    public int evidenceVersion;
    public bool arrivalAcknowledged;
    public bool assessmentAcknowledged;
    public List<int> groupOrderIds = new List<int>();
    public List<ForestCompactPlantingEvidence> plantings = new List<ForestCompactPlantingEvidence>();
    public int reviewAcknowledgedYear = -1;
}

[Serializable]
public sealed class ForestCompactPlantingEvidence
{
    public int workOrderId;
    public string speciesId;
    public string juvenileId;
    public int completionYear;
}
