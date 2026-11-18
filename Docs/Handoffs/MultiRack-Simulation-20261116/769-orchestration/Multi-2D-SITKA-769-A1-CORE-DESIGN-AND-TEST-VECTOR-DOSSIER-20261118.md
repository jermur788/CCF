# Multi #769 — A1 Core design and static acceptance/test-vector dossier (docs-only)

**18 November 2026 · [Multi #769](https://github.com/jermur788/CCF/issues/769) · Manager [#730](https://github.com/jermur788/CCF/issues/730)**

**Status: COMPLETE DRAFT, docs-only. NOT source, NOT implementation, NOT qualification.** No code was executed, built or hashed; every vector is **NOT RUN**. G2 remains HELD.

| Field | Recorded |
| --- | --- |
| Actual author | OpenAI **GPT-6.1 Sol** (`gpt-6.1-sol`), provider `codex_c75d8114-7118-4224-8bf2-b9b62637e1fb`, reasoning effort **high**, Codex/T3 native child thread |
| Child thread | `thread:delegated-task:command%3Amcp%3A168e4aa7-2aa0-41fd-94c6-fabccfa02532%3Adelegate-task%3Ac3727c1c-7201-4f29-a634-0e141decd40c` |
| Delegating parent | DeepSeek `opencode-go/deepseek-v4.1-flash` (high), T3 thread `3325d55e-b948-4569-b8db-4ea574220d57`, native TG Fork `delegate_task` |
| Docs branch | `docs/multi-769-a1-core-design-dossier-20261118-v1`, base `f6e52be18399637ec93a20a53b937b7bd097b39e` |
| Sole path | `Docs/Handoffs/MultiRack-Simulation-20261116/769-orchestration/Multi-2D-SITKA-769-A1-CORE-DESIGN-AND-TEST-VECTOR-DOSSIER-20261118.md` |
| Authority | Design only. G2 source HELD. No writer, branch, file or run is authorised by this document. |
| Forbidden models | GPT-6.2 Sol/Astra were never selected, escalated or used as fallback. |

---

# Core coding-task design and static acceptance/test-vector dossier

**CCF Multi #769 · Standalone 2D Sitka H0-A Year0 static viewer**  
**Authority:** adopted `ccf.h0a.core-read` v1.0.0.  
**Status:** documentation only; **G2 remains held**. No implementation, inspection, execution, or verification is claimed.

## 1. Scope, authority and non-goals

Core owns fixed-source admission, numeric fidelity, immutable observations, observation identity, and selection comparison. It supplies the Viewer with either one complete admitted observation or one refusal.

The supplied contract governs this design. The complete frozen schema, canonical contract text, stage-local validation rules, ownership records, and execution grants remain prerequisites. Missing details must not be reconstructed from protected material or invented during implementation.

This task excludes rendering, interaction design, ecological progression, harvesting, saving, package additions, and changes to the contract. It grants no access to P0 bytes, #710/#725, fixtures, or private stores.

The principal implementation trade-off is deliberate: private production internals preserve a single admission path; physically separate test instrumentation provides downstream failure coverage. That coverage cannot establish production reachability through the pinned digest.

## 2. Exact file/API mapping

All paths below are relative to `Tools/MultiRackSitkaYear0Viewer/`.

| Allowed path | Responsibility and entrypoints | Forbidden contents |
|---|---|---|
| `core/year0-core.js` | Private synchronous implementation; publishes only `globalThis.CcfYear0Core`. Public members: `contract`, `contractVersion`, `openGenesis()`, `keysEqual(a,b)`, `isCurrentSelection(obs,ev)`. | Other globals, public validators, selectable source/pin, verification hooks, runtime test flags, network access, UI code. |
| `core/CONTRACT.md` | Verbatim generated copy of the complete frozen contract. Documentation only. | Paraphrases presented as authoritative text, locally amended requirements, reconstructed missing sections. |
| `fixtures/p1d-30m-sitka-adult-v1.sitkap1fx.json` | Sole authorised fixture input to assembly. No executable entrypoint. | Replacement data, added scenarios, editable trust settings. Access remains separately gated. |
| `build/assemble-year0-viewer.py` | Build-time assembler: validates approved inputs and emits the agreed package and manifest. Exact CLI requires the frozen packaging specification. | Downloads, source discovery, fixture regeneration, arbitrary file inclusion, alternate-pin options, test-code inclusion. |
| `tests/core/**` | Separate test runner, synthetic vectors, assertions, and test-only access to internal stages. | Production inclusion, protected fixture copies, runtime installation of test hooks. |

`CONTRACT.md` must be copied byte-for-byte from the custodian-designated frozen contract artifact, without generated headers or newline conversion. Its authoritative location, encoding, and identity must be recorded before writing. This dossier is not that complete artifact.

Generated output destinations also require explicit approval; the source allowlist does not itself authorise new output paths.

## 3. Production trust root and global surface

The private constants are:

- Byte length: **32567**
- SHA-256: **`bcaaa5c3d9b3d36ad8319ffcc361a6225a02b90281cef8435de7c52f8ff757c4`**

Neither arguments, URL parameters, package metadata, storage, nor Viewer state may override them. `openGenesis()` accepts no source or trust configuration.

Admission follows exactly:

`S0 PackageBlock → S1 Base64 → S2 ByteLength → S3 Digest → S4 Utf8 → S5 Json → S6 RootSchema → S7 GeometryArea → S8 EcologyProvenance → S9 Roster → S10 Codec → S11 Project → S12 Key`

S3 performs SHA-256 self-tests and verifies the candidate digest before any UTF-8 decoding or JSON parsing. Failed admission returns only:

```text
{kind:"Refused", refusal:{code,stage,detail}}
```

Success returns `{kind:"Admitted", observation}`. No refusal contains an observation. Unexpected exceptions become `Refused(InternalInvariant)` under the frozen stage/detail convention; they never escape to Viewer.

Static review must trace every production publication and admission call site. Runtime acceptance must compare global properties before and after loading, verify exactly the prescribed API members, and recursively verify freezing of the API and returned observation graph.

Tests may instrument an in-memory copy of private functions through a separate module under `tests/core/**`. They must not introduce production branches, public exports, globals, or flags. The packager must positively exclude that module. Test instrumentation must be reviewed against the unmodified production source; instrumented results alone do not prove production behaviour.

## 4. Codec and numeric acceptance

Validate an f32 spelling as **exactly eight lowercase hexadecimal characters before decoding**. Then use precisely:

```text
u = parseInt(b,16) >>> 0
DataView.setUint32(0,u,false)
value = DataView.getFloat32(0,false)
```

Reject decoded NaN and either infinity. Apply each field’s frozen range separately. Re-encoding uses `setFloat32(...,false)` followed by `getUint32(...,false)`, formatted as eight lowercase hexadecimal digits.

For XZ:

```text
expectedX = Math.fround((xMm - 15000) / 1000)
expectedZ = Math.fround((yMm - 15000) / 1000)
```

Compare the re-encoded bits with `unityX` and `unityZ`; do not use epsilon comparisons or reverse reconstruction.

Useful synthetic expectations are:

| Operation | Expected bits/result |
|---|---|
| Coordinate `0` → Unity `-15` | `c1700000` |
| Coordinate `15000` → Unity `+0` | `00000000` |
| Coordinate `30000` | Domain refusal; mathematical transform is not admission |
| `Math.fround(170 / 10)` → `17` | `41880000` |
| `Math.fround(1000 / 1000)` → `1` | `3f800000` |
| Binary32 representation of `0.1` | `3dcccccd` |

Check `dbhMm/10 → dbhCm` and `heightMm/1000 → heightM` by bits; mismatch is `ConversionBitMismatch`. Crown radius is decoded and range-checked, **never recomputed**.

The dependency-free synchronous SHA-256 implementation follows FIPS 180-4 and must pass these KATs:

| Exact ASCII message | Expected digest |
|---|---|
| Empty string | `e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855` |
| `abc` | `ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad` |
| `abcdbcdecdefdefgefghfghighijhijkijkljklmklmnlmnomnopnopq` | `248d6a61d20638b8e5c026930c3e6039a33ce45964ff2167f6ecedd419db06c1` |

No `crypto.subtle`. A failed KAT yields S3/`CryptoSelfTestFailed`.

## 5. Static test-vector dossier

**PROD** means the unmodified production entrypoint. **CORE-TEST** means a separate synthetic stage test. No vector below has been run.

Mutations name contract fields rather than inventing unavailable JSON paths. Stage-local expectations require confirmation against the complete frozen validation rules before execution; disagreement is a STOP, not permission to amend the contract.

| ID / lane | Exact input or mutation | Expected stage / RefusalCode | Observation | Does **not** prove |
|---|---|---|---|---|
| C-01 PROD | Custodian-authorised, unchanged package | Through S12; Admitted | Complete | Current implementation passes |
| C-02 PROD | Remove the sole package block; separately duplicate it | S0 / `PackageBlockMissing`, `PackageBlockDuplicate` respectively | None | Payload validity |
| C-03 PROD | Replace one Base64 character with `!` | S1 / `Base64Invalid` | None | Digest checking |
| C-04 PROD | Encode exactly 32566 decoded bytes | S2 / `ByteLengthMismatch` | None | Source authenticity |
| C-05 PROD | Flip one decoded byte; retain 32567 bytes and valid Base64 | S3 / `DigestMismatch` | None | Downstream validators |
| C-06 CORE-TEST | Substitute one incorrect expected KAT result in the isolated test copy | S3 / `CryptoSelfTestFailed` | None | Production KAT wiring |
| C-07 CORE-TEST | Supply bytes `c3 28` directly to UTF-8 stage | S4 / `Utf8Invalid` | None | Reachability after production digest |
| C-08 CORE-TEST | Supply UTF-8 text `{` directly to JSON stage | S5 / `JsonInvalid` | None | Trusted fixture parsing |
| C-09 CORE-TEST | Separately delete a required root key, add an unknown root key, replace a required number with a string, alter a frozen constant | S6 / `SchemaMissingKey`, `SchemaUnknownKey`, `SchemaTypeMismatch`, `SchemaConstantMismatch` | None | Later semantic checks |
| C-10 CORE-TEST | Set `xMm=-1`; separately `yMm=30000` | S7 / `CoordinateOutOfDomain` | None | Other geometry limits |
| C-11 CORE-TEST | Replace the frozen source-stage value with an unsupported stage | S8 / `StageUnsupported` | None | Provenance authenticity |
| C-12 CORE-TEST | Retain 99 rows; separately append a 101st row | S9 / `RosterCountMismatch` | None | ID/order validation |
| C-13 CORE-TEST | Independently duplicate an ID, change case, swap adjacent IDs, or substitute an out-of-roster ID while retaining 100 rows | S9 / `RosterIdInvalid` | None | Every row value |
| C-14 CORE-TEST | Set one source status away from `live`; separately age away from `30` | S9 / `RowValueInvalid` | None | Ecological realism |
| C-15 CORE-TEST | Replace a valid availability envelope with an unknown tag or prohibited numeric stand-in for unknown | S10 / `NumericEnvelopeInvalid` | None | All envelope metadata |
| C-16 CORE-TEST | Use f32 bits `7fc00000`, `7f800000`, `ff800000`, independently | S10 / `NumericNonFinite` | None | Finite range validation |
| C-17 CORE-TEST | Use seven digits `3f80000`; separately uppercase `3F800000` | S10 / `NumericEnvelopeInvalid` | None | Arithmetic correctness |
| C-18 CORE-TEST | Set crown-radius bits to `00000000` in an otherwise valid numeric envelope | S10 / `NumericOutOfRange` | None | Crown derivation |
| C-19 CORE-TEST | Change only valid finite `unityX` bits away from the forward result | S10 / `TransformBitMismatch` | None | Z conversion |
| C-20 CORE-TEST | Independently change `dbhCm` or `heightM` to valid finite bits differing from the required conversion | S10 / `ConversionBitMismatch` | None | All numeric fields |
| C-21 CORE-TEST | Force an unexpected exception inside one stage | Catch boundary / `InternalInvariant` | None | Every exception source |
| C-22 PROD API | Compare a valid key with a foreign token, case-altered token, or unknown-version token; use each in selection comparison | No admission stage/refusal; comparison false | No new observation | S12 construction |

Each stage vector starts with earlier-stage preconditions satisfied. Pairwise first-failure vectors must additionally combine two faults and expect only the earlier refusal. Synthetic downstream vectors must never receive a production pin override.

## 6. Packaging trust-negative design

Assembly must use an explicit input allowlist: approved production Core source, the single authorised fixture, and separately approved Viewer assets. Directory discovery is insufficient. Reject unexpected files, duplicate logical inputs, path traversal, symlink escapes, unresolved substitutions, test inputs, and unapproved executable blocks.

Inspect **emitted bytes**, not only source files. Confirm package-block uniqueness, fixed trust constants, script inventory, global publication, and absence of alternate admission entrypoints. Deny-string scans are supplementary: aliases, generated code, and encoding can conceal prohibited behaviour.

Assembly must produce identical bytes from identical authorised inputs. Exclude timestamps, randomness, machine paths, locale-sensitive formatting, and network results.

The manifest must identify contract/version, approved source identities, fixture length/digest, assembler identity, output length/digest, deterministic ordering policy, and production exclusion of `tests/core/**`. A manifest documents construction; it cannot replace Core’s private trust root.

## 7. F1, F3 and roster acceptance

**F1:** Require exactly one of `PresentSourced`, `PresentComputed`, `NotEvaluated`, or `Unavailable`, with precisely the frozen member structure. Reject multiple states, unknown tags, missing required provenance, and numeric values where an unknown state forbids them. Zero is not an unknown sentinel; a known zero remains subject to its field’s range.

**F3:** Construct canonical `name=value\n` text in this exact order:

```text
readSchemaId, readSchemaMajor, readSchemaMinor,
observationProvenance, sourceBundleKind, domainTag, fixtureId,
fixtureByteSha256, fixtureByteLength, catalogVersion, format,
derivedSchemaVersion, sourceClass, experimentLineageId,
originGeneration, growthModelVersion, rngModelVersion,
annualAlgorithmVersion, simulationSeed, numericPolicyId,
geometryModel, frameTransformId, coordinateOriginAxes,
standAreaHaBits, observedEcologicalYear, sourceStage,
stepTargetYear, stepTransitionId, committedRevisionOrFrameId,
coreContractVersion
```

The displayed commas and wrapping are documentation only. Canonical bytes use the frozen value serialization, UTF-8, and LF. No extra components, sorting, locale formatting, or omitted final newline.

Compute SHA-256, lowercase its hex output, and prepend `h0a-okey-v1:`. Never type a precomputed complete observation token.

Test component omission, insertion, case changes, ordering changes, and one-value changes. Unknown or case-wrong schema members require the applicable frozen schema refusal; foreign token comparisons return false. These are different acceptance surfaces.

**Roster:** Require 100 IDs, exactly `P30-001` through `P30-100`, in source order; every row is `live`, age 30. Duplicate, missing, case-wrong, reordered, and foreign IDs fail. A 101-row input fails count before ID checking.

Annotate strict `dbhMm < 170`; exactly 12 rows must be true. A synthetic row at 170 must be false. This annotation must never be described as “eligible.”

## 8. STOP conditions and residual risks

**L4 FAIL38 remains unwaived.** This dossier supplies no waiver or source rights.

Stop before implementation if the frozen contract, custodian baseline, exact ownership, branch/base, containment, or grants are absent. Stop on schema/stage ambiguity, unexpected inputs, protected-source requirements, changed pins, or packaging scope outside the allowlist.

Residual risks include implementation errors in SHA-256, divergence introduced by test instrumentation, unspecified malformed-caller behaviour, and incomplete emitted-package inspection. Static vectors do not establish runtime performance, rendered quality, or production acceptance.

## 9. Ready-to-execute Core task brief

- **Task:** Implement only the adopted Core contract and its contained static acceptance suite.
- **Branch:** Not supplied. **STOP until the coordinator provides the exact branch name.**
- **Base:** Not supplied. **STOP until the custodian provides the exact baseline commit SHA.**
- **Allowlist:** The five path entries in Section 2; `tests/core/**` remains test-only.
- **Preconditions:** Custodian-approved baseline and full frozen contract; confirmed single-writer ownership; containment and production-exclusion plan; explicit source-access and write grants; approved output locations; required gate disposition.
- **Execution:** Confirm mappings, implement private admission and helpers, generate the verbatim contract copy, run authorised tests, inspect assembled bytes, and report evidence plus unverified limits.
- **Completion:** Requires actual authorised implementation and verification. This design grants neither execution nor release permission.

Author: GPT-6.1 Sol (Codex), docs-only design; no code executed.
