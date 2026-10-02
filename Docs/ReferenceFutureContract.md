# Reference Future v1: verification contract

Reference Future v1 is a **frozen, authored historical archive** of one managed Scenario One century. It was produced by the v12-era ecology and save schema:

| Artefact | Value |
|---|---|
| `ScenarioOne/Resources/ScenarioOneReferenceFutureV1.bytes` | Archive file |
| `ScenarioOneReferenceScheduleV1.json` | Schedule file |
| Year 0 | `A564039D9B7CE31D` |
| Year 20 | `F7DF7DAB53B6FD32` |
| Year 50 | `D5E75D6D21D631AC` |
| Year 100 | `7AD177B3CC2F73C7` |
| Schedule | `56C8B99FA1E8DDD1` |

The archive is immutable. Its stored contents and hashes are the authoritative historical record, and later ecology or schema versions never rewrite it. A changed model gets a new reference (for example v2), never an edit to v1.

## What is verified

| Term | Meaning | Required? | Verified by |
|---|---|---|---|
| **ARCHIVE** | The stored milestones and schedule are intact. Each milestone's *original embedded JSON* hashes to its stored hash and to the pinned anchor above. The schedule file matches the embedded schedule. | Yes | `ReferenceArchiveIntegrity.Verify` (in `Tools/Verification/ScenarioReferenceVerification.cs`, used by `ReportFrozen` and `Begin`); the production loader `ScenarioReferenceArchive.Load` (`verifiedFrozenWorld`); stored-hash checks in the Interaction and Habitat gates |
| **PREVIEW** | Current code loads each frozen milestone and shows it exactly as stored: tree identities, species, positions, stages, ages and dimensions. Ending the preview restores the player's world unchanged. | Yes | `ScenarioReferenceVerification.Begin`; Habitat gate (Years 20/50/100); Interaction gate (Year 20) |
| **CONTINUATION** | A historical save (the v12 Year-50 milestone) loads under the current schema and continues to Year 100 with the **current** ecology. See below for what is asserted. | Yes | `ScenarioReferenceVerification.Begin`; `ScenarioOneInteractionVerification` |
| **REPLAY** | Current code regenerating, or continuing to, the *historical* Year-100 biology exactly. | **No.** Not part of the contract. | Divergence is logged as a diagnostic only |

**Continuation assertions:**
- Tree identities, species and positions are preserved, and harvested stumps stay harvested.
- No duplicate tree ids appear, and no tree dies biologically without an explicit cause.
- Historical management events are preserved.
- The output is current-schema with a Year-100 Century Review.
- The archive's known P0601 same-year prune/fell exception is still recorded.
- The run is deterministic: two independent continuations agree, and so does a resume from a Year-75 save.
- The Year-100 world round-trips through save/load.

**Why ARCHIVE never re-serialises.** Deserialising a v12 world into the current save classes and serialising it again adds the fields of later schemas (for example the v14 mortality fields). The result hashes differently even though the archive is untouched. Integrity is therefore always checked on the original embedded text, never on a current-schema representation.

**Why REPLAY is excluded.** Later ecology calibration (for example C8 growth and k10a10 light) deliberately changes the biology. Continuing the frozen Year-50 world under current code therefore diverges from the frozen Year-100 biology: different diameters, heights and recruits. That divergence is expected. The project does not keep a versioned historical ecology engine to reproduce old trajectories.

## Entry points

The tooling is in `Tools/Verification/ScenarioReferenceVerification.cs`. Copy it into `Assets/ForestPrototype/` for the run, then remove the copy and its `.meta`.

| Entry point | Mode | What it does |
|---|---|---|
| `ScenarioReferenceVerification.Begin` | Play mode | Archive, preview and continuation gate. Ends with `REFERENCE_FUTURE_V1_CONTRACT_PASS`. |
| `ScenarioReferenceVerification.ReportFrozen` | No play mode; add `-quit` | Archive integrity plus a report of the frozen milestones, read from stored data |
| `ScenarioReferenceVerification.BeginAuthoringCandidate` | Play mode | Authors a *new* candidate with the current model. Differences from v1 are logged (`REFERENCE_AUTHORING_VS_V1`). `Freeze` refuses to overwrite v1. |
