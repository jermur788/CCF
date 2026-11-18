# Multi #758 — H0-A frozen Core→Viewer and D5 contract corrections (COMPLETE DRAFT v1.0.0)

**18 November 2026 · [#758](https://github.com/jermur788/CCF/issues/758) · Manager [#730](https://github.com/jermur788/CCF/issues/730)**

**Status: COMPLETE CORRECTED CONTRACT — DRAFT, NOT APPROVED.** Proposed \`ccf.h0a.core-read\` version \`1.0.0\`. **D5 BLOCKED** pending specific physical read authorisation and owner/security decision on scoped JS-only proof. No G2 source permission, no tests/build, no public production code, no L4 waiver.

| Field | Recorded |
| --- | --- |
| Actual writer | OpenAI **GPT-6**, ChatGPT GitHub-connector documentation harness. Reasoning effort not exposed. Separate worker session from #757 independent reviewer, with **no right to approve this revision**. |
| [RUNNING] before first edit | [#758 registration](https://github.com/jermur788/CCF/issues/758#issuecomment-6987391502) |
| Fresh GitHub \`main\` start | \`34185b8b71ba280c89091290345e537ef107bdf4\` |
| Sole docs branch | \`docs/multi-758-h0a-contract-revision-result-20261118-v1\`, newly created at the above main |
| Sole result | This Markdown file, based on the **complete** [#756 draft](https://github.com/jermur788/CCF/blob/7aaa7a90fd5134850f3eeda5696410e20c82f286/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-756-H0-A-FROZEN-CONTRACT-AND-D5-PROTECTION-PREFLIGHT-DRAFT-RESULT-20261118.md) and [#757 two-blocker/one-major review](https://github.com/jermur788/CCF/blob/docs/multi-730-757-independent-contract-review-20261118-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-757-INDEPENDENT-GPT6-TARGETED-H0-CONTRACT-REVIEW-RESULT-20261118.md). Original #756 author was Anthropic Claude Opus 5.5, Claude Code/T3 native; its historical physical observations are retained **with source attribution**, not remeasured here. |
| Current rights | GitHub-only documentation. No physical Ops/custody, no local worktree, no Core/A code, no fixture/P0/#710/#725 access, no browser run. |

### Exact targeted changes

**#757 B-1:** production Core has one **immutable fixed-pin** public facade and absolutely no runtime-activated alternate trust root; the caller-selectable pin belongs only to a **physically separate Core-only test artifact** excluded from production. The packager uses an explicit positive input allowlist; an independent reviewer inspects actual emitted production bytes and exposed globals.

**#757 B-2:** D5 B-3 no longer treats \`status\`, size, and \`mtime_ns\` as byte equality. Each **lawfully named in-scope file** needs pre/post SHA-256 of contents and path/type/mode inventory, plus verified clean tracked HEAD/Git object pins where applicable. #710/#725 are excluded, making any proof explicitly **partial**; writer sandbox and absence of other protected writes are **not presumed**.

**#757 M-1:** Core-only negative decoder tests and Viewer-only immutable fixed-schema doubles are disjoint; A never uses test trust hooks or supplies fixture bytes/pins. Production altered bytes refuse **before JSON parsing**. The future tests distinguish what each harness actually qualifies.

**Unchanged from #756:** all 100 source-order \`P30-001\` through \`P30-100\` IDs, source \`status="live"\`, source float32 bit envelopes and conversion, F1 availability and full F3 key, one Core numeric codec, one A shell, immutable responses, fixed bundle pin, **strict \`dbhMm < 170\` annotation only** (the row at exactly 170 is false; no work eligibility rule), Genesis only, no Year1/Host/storage/network, and D5 held.

---

---

## 1. Read-only preflight and provenance

### 1.1 #756 original author's source inventory (not a new #758 physical audit)

| Input | Pinned location |
| --- | --- |
| #756 binding packet | `docs/multi-730-756-preg2-contract-baseline-packet-20261118-v1` @ `53201d7d` (one added Markdown vs main, verified with `git diff --stat`) |
| #750 owner H0-A choice and four-file approval receipt; approved `spec.md`, `plan.md`, `tasks.md`, `checklists/requirements.md` | `docs/multi-750-h0-a-owner-approved-four-file-spec-20261118-v1` @ `a9579188` |
| #748 F1–F5 consolidation | `docs/multi-748-f1-f5-consolidation-20261118-v1` |
| #753 read-only custody and package preflight | commit `f3a15e3e` |
| #755 Phase 1 G1 physical lane setup result | commit `098928f0` |
| Manager #755 physical verdict | [comment 6986466122](https://github.com/jermur788/CCF/issues/755#issuecomment-6986466122) |
| P0 protection authority (L0–L4 layers, P0 pin) | owner-approved P1 four-file spec `docs/multi-p1-637-owner-approved-four-file-20261116-v1`: `spec.md` §9.2 "Lineage and protection of P0" and m02; `checklists/requirements.md` REQ-B07 |
| Main decisions and coordination | `decision-log.md` D-082 and D-091; `active-tasks.md` Multi #694/#710/#715/#725 paragraphs (read only) |

### 1.2 Historical #756 physical observations — not rechecked by #758

| Observation (2026-11-18T17:49:39Z) | Result |
| --- | --- |
| `ZX20/CCF-Ops/board/active.md` SHA-256 | `4fa750eb…3994`, mtime 17:28:31Z. **Equal to the #755 "after" hash.** |
| `ZX20/CCF-Ops/lanes.json` SHA-256 | `c1d14719…7d9e`, mtime 17:28:31Z. **Equal to the #755 "after" hash.** 25 lanes, including `MultiH0ACore` and `MultiH0AViewer`. |
| `ZX20/CCF-Ops/deployment.json` SHA-256 | `8c66b290…7b18`, unchanged since 2026-11-15. |
| Ops scripts run | **None reported by #756.** The #756 author reported hashing these files and reading named lane properties in RAM. #758 did not physically repeat the observation. |

**Historical #756 conclusion at the stated observation time:** physical Ops matched #755; H0 lanes were RESERVED / NOT RUNNING. This is not a fresh physical read, lease or permission.

### 1.3 #756 author's historical public-fixture byte observations — no new import

The #756 author reported reading the public blob at `feature/multi-p1-639-f0-30m-foundation-20261116-v1` tip `e302f15b`, path `Tools/MultiRackSitkaP1Lab/fixtures/derived/p1d-30m-sitka-adult-v1.sitkap1fx.json`, through `gh api` into RAM, with no saved copy. **#758 did not read the blob or original fixture.** The recorded values below remain source-attributed observations to be requalified at future G2.

| Fact | Observed |
| --- | --- |
| Length / SHA-256 | **32,567** bytes / **`bcaaa5c3d9b3d36ad8319ffcc361a6225a02b90281cef8435de7c52f8ff757c4`**. This matches the pin and #753's independent observation. |
| Byte hygiene | First byte `{`, last byte `}`, no BOM, 0 CR bytes |
| Root keys, in source order (16) | `format, derivedSchemaVersion, sourceClass, domain, derivedFixtureId, catalogVersion, disclosure, derivation, geometry, area, ecology, conversions, sourceProvenance, treeCount, trees, contentHash` |
| Row keys: one identical 12-key tuple for all 100 rows | `treeId, xMm, yMm, status, ageYears, dbhMm, heightMm, dbhCm, heightM, crownRadiusM, unityX, unityZ`. The last five are `{"k":"f32","b":"<8 hex>"}`. |
| IDs | Exactly `P30-001`…`P30-100`, in that source order |
| `status` value set | `{"live"}`. The literal source token is **`live`**, not "living". |
| Other typed f32 fields | `area.areaHa` = `3db851ec`; `ecology.siteProductivity` = `3f800000` |
| `ecology.excludedSystems` | `browsing, canopyLight, juvenileLifecycle, mixedSpecies, pruning, recruitment, regeneration, storm` |
| `derivation.generatorSeed` | 6390001. This is **distinct from** `ecology.simulationSeed` 20261116. |
| **DBH distribution at the 170 mm note** | min 131 mm, max 349 mm. **12 rows have `dbhMm` < 170 and exactly one row has `dbhMm` == 170** (13 rows ≤ 170). See finding F-1. |
| Informational codec check | A Python `struct` forward computation of `(float)((mm−15000)/1000.0)` agrees bit-for-bit with all 200 `unityX/unityZ` envelopes. **This is a reviewer observation on public bytes, not H0-AC03 evidence.** The JS Core must reproduce it itself. |

**Finding F-1 (MATERIAL TO WORDING, not to scope).** "12 Year0 stems under 170 mm" holds only under a **strict** `<` comparison. One stem sits exactly on the threshold. #748 F2 left the operator unratified ("`>=` only if confirmed"). H0 needs no eligibility. Its annotation must therefore be labelled "DBH below 170 mm (strict, annotation only)", and that row must not be described as "eligible". §2.6 pins this.

### 1.4 Historical #756 exclusions; same no-touch boundary for #758

- #710 and #725 stores: contents and metadata were **not** accessed.
- The H0 sparse worktrees: not opened.
- The local #639 worktree and any physical P0 tree: not opened.
- No Ops script, lock, board, lanes or lease change.
- No source, fixture copy, test, build, browser, Node, .NET, Unity, Windows or Host.
- No main edit and no L4 policy change.
- The #756 author reported only read-only Git documentation operations in an unrelated dirty checkout. **#758 did not access any local checkout.**

---

## 2. Proposed frozen Core→A contract: `ccf.h0a.core-read` **v1.0.0** (DRAFT)

The contract is normative once adopted. All names below are proposals until #730 adopts them. The contract refines spec §4 and #748 F1/F3 for one fixture-only Genesis frame. It **adds no scope**: no Year1, eligibility (F2), marks, history, Host, network or storage.

### 2.1 Packaging form and global surface

- **Core** is one classic (non-module) script, `core/year0-core.js`. It must not use `import`, `export`, `fetch`, `XMLHttpRequest`, `WebSocket`, `EventSource`, `navigator.sendBeacon`, dynamic `import()`, `eval`, `new Function`, `Worker`, `localStorage`, `sessionStorage`, `indexedDB`, `document.cookie`, `crypto.subtle` or timers for trust work.
- Core defines exactly **one** production global:

```text
globalThis.CcfYear0Core = deepFrozen({
  contract:        "ccf.h0a.core-read",
  contractVersion: "1.0.0",
  openGenesis():                        AdmissionResult          // synchronous; reads the packaged block itself
  keysEqual(a: ObservationKeyV1, b: ObservationKeyV1): boolean   // full-key comparison
  isCurrentSelection(obs: GenesisObservationV1, ev: SelectionNotificationV1): boolean
})
```

- **A never passes bytes to Core** and never parses, hashes or decodes fixture data. `openGenesis()` locates the single element `<script type="application/octet-stream" id="ccf-h0a-fixture-b64">` that the packager inserts. That type is not executed by browsers and is not subject to `script-src`.
- **Version check.** A's first action is `CcfYear0Core.contract === "ccf.h0a.core-read" && CcfYear0Core.contractVersion === "1.0.0"`. On mismatch, A renders only an incompatibility notice and the disclosure banner.
- **Semver rule.** Any change to a field name, type, enum member, refusal code or function signature is a **major** bump and needs Manager re-adoption before A consumes it.
- **Single immutable production trust root (B-1):** shipped \`core/year0-core.js\` provides exactly the three public methods and two version fields in \`CcfYear0Core\`. No second Core global. Production \`openGenesis()\` accepts no input arguments and reads the one embedded block. Its **only** admission pin is private and fixed: \`byteLength=32567\`, \`sha256=bcaaa5c3d9b3d36ad8319ffcc361a6225a02b90281cef8435de7c52f8ff757c4\`. Neither A nor arbitrary page scripts can choose a different pin, supply bytes or change the trust policy. Only admitted, digested bytes reach the JSON decoder.
- **Prohibited from shipped Core and HTML:** the former \`CcfYear0CoreVerification\` global; former \`CCF_H0A_VERIFICATION_HARNESS\` runtime flag or any equivalent global-activated testing branch; \`admitBytes(bytes,pin)\` or any alternate caller-pin admission pathway; a globally callable digest/test hook; runtime test-mode switching, dynamically loaded test script or mutable trust pin. Removing a flag from the shell while leaving bypass code in Core **does not qualify**.
- **Separated Core-owned test build:** the Core owner may create \`tests/core/core-test-only-entrypoint.js\` or an equivalent distinct **test-only build artifact**, not concatenated or imported by production, to call the **same internal admission and codec implementation** with controlled fixture bytes/test pin for schema/roster/numeric negatives. No forked second decoder algorithm. The test seam is contained within the test artifact; it is never a production global or production function selected by a runtime switch. Source and packaging of this seam require non-author review.
- **Separate A test doubles (M-1):** A-owned harness fixtures only construct complete immutable public \`AdmissionResult\`/\`GenesisObservationV1\` objects and foreign full F3 keys to test rendering/error/staleness; they **never** call the Core test artifact, supply an admission pin, parse fixture JSON or mutate the active production observation. Production A calls only the public Core facade.
- **Packaging proof:** only the precise production inputs listed in §5 are ever assembled. The complete final inline JS, script inventory and actual global surface must be independently examined for any alternate trust root. A deny-string scan is insufficient and **may not exempt code found inside Core**.



### 2.2 Result types (all deep-frozen, primitives and frozen plain objects/arrays only)

```text
AdmissionResult =
  | { kind: "Admitted", observation: GenesisObservationV1 }
  | { kind: "Refused",  refusal: { code: RefusalCode, stage: AdmissionStage, detail: string } }
```

- `detail` is presentation-only text: ASCII, at most 160 characters, with no fixture content beyond a field path or tree ID. It is never parsed by A.
- Core **never throws** to A. An unexpected exception inside Core becomes `Refused(InternalInvariant)`.
- There is **no partial observation**: no field, row or key escapes on refusal.

**AdmissionStage** (checked in this order; the first failure wins):
`S0 PackageBlock → S1 Base64 → S2 ByteLength → S3 Digest → S4 Utf8 → S5 Json → S6 RootSchema → S7 GeometryArea → S8 EcologyProvenance → S9 Roster → S10 Codec → S11 Project → S12 Key`.

S3 digest verification runs **before UTF-8 and JSON parsing**. Tampered production bytes refuse at S0–S3 and cannot reach schema negatives. S5–S12 negative cases require a **separate test-only Core build** with a test fixture and test pin; they do not demonstrate production acceptance of changed bytes. A doubles never test admission.

**RefusalCode (closed set; an unknown code is impossible by construction):**

| Code | Stage | Trigger |
| --- | --- | --- |
| `PackageBlockMissing` / `PackageBlockDuplicate` | S0 | zero, or more than one, element with the id/type |
| `Base64Invalid` | S1 | any character outside `[A-Za-z0-9+/]`, `=` other than 0–2 trailing pad, length not a multiple of 4, any whitespace |
| `ByteLengthMismatch` | S2 | decoded length ≠ 32,567 |
| `CryptoSelfTestFailed` | S3 | Core's SHA-256 fails a built-in known-answer vector before hashing the fixture |
| `DigestMismatch` | S3 | SHA-256 ≠ compiled-in pin. Also catches duplicate JSON keys, reordering, whitespace and every same-length alteration |
| `Utf8Invalid` | S4 | `TextDecoder("utf-8", {fatal:true})` fails, or a BOM is present |
| `JsonInvalid` | S5 | `JSON.parse` throws, or the top level is not an object |
| `SchemaMissingKey` / `SchemaUnknownKey` / `SchemaTypeMismatch` | S6–S9 | exact key sets per object (§2.4) |
| `SchemaConstantMismatch` | S6–S8 | any pinned constant differs: format, schema version, class, domain, fixture ID, catalog, geometry model, side, polygon, elevation, transform ID, area bits, seed, models, algorithm, species, stage, demand class, pruning, threshold, excluded-systems list, `initialEcologicalYear` ≠ 0 |
| `RosterCountMismatch` | S9 | `treeCount` ≠ 100 or `trees.length` ≠ 100 |
| `RosterIdInvalid` | S9 | IDs not exactly `P30-001`…`P30-100` in source order: missing, duplicate, case-changed, unknown or reordered |
| `CoordinateOutOfDomain` | S9 | `xMm`/`yMm` not a safe integer in `[0, 30000)` |
| `RowValueInvalid` | S9 | `status` ≠ `"live"`, or `ageYears`/`dbhMm`/`heightMm` not a positive safe integer |
| `NumericEnvelopeInvalid` | S10 | envelope keys ≠ `{k,b}`, `k` ≠ `"f32"`, or `b` not `/^[0-9a-f]{8}$/` |
| `NumericNonFinite` | S10 | decoded NaN or ±Infinity |
| `NumericOutOfRange` | S10 | DBH, height or crown radius ≤ 0; crown radius < 0.1 m (the source rule's `MathF.Max(0.1f, …)` floor) |
| `TransformBitMismatch` | S10 | forward f32 X/Z bits ≠ `unityX`/`unityZ` bits for any row |
| `ConversionBitMismatch` | S10 | **proposed addition:** `fround(dbhMm/10)` ≠ `dbhCm` bits, or `fround(heightMm/1000)` ≠ `heightM` bits. This is the fixture's own `conversions.dbhRule`/`heightRule`. Crown is **never** recomputed, because the .NET float32 expression is not re-implemented. |
| `StageUnsupported` | S11 | reserved. The fixture-only path can only produce Genesis; any other stage token is refused |
| `InternalInvariant` | any | defensive catch-all; never expected |

### 2.3 Codec rules (Core only; the single numeric authority)

- **f32 decode.** Write `parseInt(b, 16) >>> 0` with `DataView.setUint32(0, u, false)` (big-endian, i.e. the hex digits are the IEEE-754 bit pattern read most-significant first), then `getFloat32(0, false)`. Expose `{kind:"f32", bits, value}`, where `value` is the JS Number exactly equal to the binary32 value. No rounding.
- **XZ forward rule** (transform `p1-square30-corner-to-centred-xz-v1`): `X = Math.fround((xMm - 15000) / 1000)` and `Z = Math.fround((yMm - 15000) / 1000)`.
  - The subtraction is exact (|value| ≤ 15000), the division is binary64, and there is exactly one f32 rounding.
  - Re-encode with `setFloat32` + `getUint32` and compare the 8-hex bits to `unityX`/`unityZ`.
  - **No reverse f32→mm equality is ever demanded.**
- **f64** never appears in a Genesis observation. The `f64` kind is reserved in envelopes (§2.5) for future qualified aggregates only.
- **SHA-256: ONE path, chosen now.** Core embeds a dependency-free synchronous SHA-256 (FIPS 180-4) and **does not use `crypto.subtle`**.
  - Reason: whether `file://` documents are secure contexts in the target browsers is unverified, and `subtle.digest` is asynchronous.
  - Before first use, the self-test runs the FIPS 180-4 example vectors for `""`, `"abc"` and the 448-bit `"abcdbcdecdefdefgefghfghighijhijkijkljklmklmnlmnomnopnopq"` message. Failure → `CryptoSelfTestFailed`.
  - **Implementation-time STOP:** if this choice is rejected, Manager must amend the contract. The writer may not ship both paths.
- **Base64.** Strict regex validation, then `atob`, then a byte copy into a `Uint8Array`. `atob`'s whitespace forgiveness never applies, because whitespace already failed S1.

### 2.4 Exact source key sets validated at S6–S9

| Object | Exact keys (set equality; order is fixed by the digest anyway) |
| --- | --- |
| root | the 16 keys of §1.3. `contentHash` is **parsed as a string and ignored for trust**: it is self-declared, its canonicalisation is undocumented, and it is never exposed as an identity. |
| `derivation` | `kind, generatorId, generatorVersion, generatorSeed, originalP0FixtureId, note`. Pinned: `kind="designed"`, `originalP0FixtureId=null`. |
| `geometry` | `geometryModel, sideMm, localFrame, polygonMm, elevationMm, xzTransformId, xzRule`. Pinned: `P1DerivedSquare30m`, 30000, polygon `(0,0),(30000,0),(30000,30000),(0,30000)`, 0, `p1-square30-corner-to-centred-xz-v1`. |
| `area` | `areaM2Exact, areaHaExpression, areaHa, areaPolicy`. Pinned: `"900"`, `areaHa` f32 `3db851ec`. |
| `ecology` | `initialEcologicalYear, firstAdvanceEcologicalYear, sourceInventoryYear, simulationSeed, growthModelVersion, rngModelVersion, p1AlgorithmVersion, siteProductivity, stage, species, demandClass, pruning, actionEligibleDbhThresholdMm, populationRule, excludedSystems`. Pinned: 0, 1, null, 20261116, 2, 1, `p1-f0-adult-sitka-30m-v1`, f32 `3f800000`, `MatureEquivalent`, `Sitka`, `Reference`, 0, 170, the 8-item list of §1.3. |
| `conversions` | `dbhRule, heightRule, crownRuleId, crownRule`. Pinned: `crownRuleId="potential-crown-at-ci0-linear-f32-v1"`. |
| `sourceProvenance` | `ccfInspectedCommit, classification, files`. Each `files[i]` has keys `path, role, gitBlob, sha256`. Carried as provenance strings only. |
| `trees[i]` | the 12 keys of §1.3 |

Free-text fields (`disclosure`, `note`, `localFrame`, `xzRule`, `areaPolicy`, `populationRule`, rule strings, `classification`) are type-checked as strings and carried verbatim as **source provenance text**. They are never interpreted as executable rules.

### 2.5 `GenesisObservationV1` shape (types and fixed values only; no runtime data invented)

`<…>` marks a value Core fills **from the admitted bytes or by its own computation**. Literal constants are the pinned identities from §1.3 and the approved spec. **This is descriptive type notation, not executable JS:** slash-separated property labels and multiplicity placeholders must be expanded into concrete individually typed fields by a future authorised Core implementer.

```text
GenesisObservationV1 = {
  readSchemaId: "ccf.multi.sitka.h0a.genesis-observation", readSchemaMajor: 1, readSchemaMinor: 0,
  observationKey: ObservationKeyV1,                               // §2.7
  source: {
    sourceKind: "TrustedFixtureOnly",
    format: "ccf-sitka-p1-derived-fixture", derivedSchemaVersion: 1,
    sourceClass: "Synthetic", domain: "DerivedP1ThirtyMetreExperimentV1",
    fixtureId: "p1d-30m-sitka-adult-v1", catalogVersion: "p1-f0-derived-catalog-v1",
    byteLength: 32567, sha256: "bcaaa5c3d9b3d36ad8319ffcc361a6225a02b90281cef8435de7c52f8ff757c4",
    sourceDisclosureText: <fixture.disclosure>,                   // provenance text
    provenance: { ccfInspectedCommit: <string>, classification: <string>,
                  files: [ { path, role, gitBlob, sha256 } ×<n> ] },
    derivation: { kind: "designed", generatorId: <string>, generatorVersion: <int>,
                  generatorSeed: <int>, originalP0FixtureId: null }   // generatorSeed ≠ simulationSeed
  },
  experiment: {
    algorithmPolicy: "p1-f0-adult-sitka-30m-v1", simulationSeed: 20261116,
    growthModelVersion: 2, rngModelVersion: 1,
    siteProductivity: { kind:"f32", bits:"3f800000", value:<Number> },
    species: "Sitka", lifeStage: "MatureEquivalent", demandClass: "Reference", pruning: 0,
    excludedSystems: [ <8 source strings in source order> ],
    annualCallMade: false                                         // provenance, never evidence of a run
  },
  geometry: {
    geometryModel: "P1DerivedSquare30m", sideMm: 30000, elevationMm: 0,
    polygonMm: [ {xMm:0,yMm:0}, {xMm:30000,yMm:0}, {xMm:30000,yMm:30000}, {xMm:0,yMm:30000} ],
    localAxes: "SW-origin integer mm; +X east; +Y north (synthetic local, not geographic)",
    frameTransformId: "p1-square30-corner-to-centred-xz-v1",
    areaHa: { kind:"f32", bits:"3db851ec", value:<Number> }, areaM2Nominal: "900"
  },
  time: { sourceStage: "Genesis", stateEcologicalYear: 0,
          stepTargetYear: { state:"NotEvaluated", reason:"NotPartOfThisStage" },
          annualRevision / deathAudit / eventTip / saveOrSessionId:
                          { state:"Unavailable", reason:"NotRecordedInTrustedSource" } },
  inventory: {
    declaredTreeCount: { state:"PresentSourced",  kind:"int", value:<treeCount>, sourceRef:"treeCount" },
    registeredCount:   { state:"PresentComputed", kind:"int", value:<count>,
                         sourceRef:"trees[*]", calculationStage:"GenesisInventoryProjection" },
    livingCount:       { state:"PresentComputed", kind:"int", value:<count status=="live">,
                         sourceRef:"trees[*].status", calculationStage:"GenesisInventoryProjection" }
  },
  standAggregates: {                                              // never recomputed, by Core or A
    stemsPerHa:       { state:"Unavailable", kind:"f32", unit:"stems/ha", reason:"StageAggregateNotSupplied", sourceContext:"StaticGenesisFixture" },
    basalAreaPerHa:   { state:"Unavailable", kind:"f64", unit:"m2/ha",    reason:"StageAggregateNotSupplied", sourceContext:"StaticGenesisFixture" },
    quadraticMeanDbh: { state:"Unavailable", kind:"f64", unit:"cm",       reason:"StageAggregateNotSupplied", sourceContext:"StaticGenesisFixture" },
    relativeDensity:  { state:"Unavailable", kind:"f64", unit:"1",        reason:"StageAggregateNotSupplied", sourceContext:"StaticGenesisFixture" }
  },
  actionNote: { thresholdMm: { state:"PresentSourced", kind:"int", value:<170>, sourceRef:"ecology.actionEligibleDbhThresholdMm" },
                comparison: "dbhMm < thresholdMm (strict), annotation only",
                operatorRatified: false, isEligibility: false },  // F-1
  rows: [ ReadTreeV1 ×100, source order P30-001…P30-100 ]
}

ReadTreeV1 = {
  treeId: <"P30-nnn">, sourceIndex: <0..99>,                       // sourceIndex is provenance, never identity
  lifeStatus: "Live",                                             // mapped 1:1 from source "live"; NOT health
  species: "Sitka", lifeStage: "MatureEquivalent", demandClass: "Reference",
  ageYears: <int>, xMm: <int>, yMm: <int>, dbhMm: <int>, heightMm: <int>,   // raw source integers
  dbhCm:        { kind:"f32", bits:<8hex>, value:<Number>, unit:"cm" },
  heightM:      { kind:"f32", bits:<8hex>, value:<Number>, unit:"m" },
  crownRadiusM: { kind:"f32", bits:<8hex>, value:<Number>, unit:"m" },      // RADIUS
  centredX:     { kind:"f32", bits:<8hex>, value:<Number>, unit:"m", verifiedForward:true },
  centredZ:     { kind:"f32", bits:<8hex>, value:<Number>, unit:"m", verifiedForward:true },
  belowActionNote: <boolean>,                                     // annotation only; see actionNote
  diagnostics: {
    hegyiCi:           { state:"NotEvaluated", kind:"f32", reason:"StaticGenesisNoAnnualEvaluation" },
    neighbourCount:    { state:"NotEvaluated", kind:"int", reason:"StaticGenesisNoAnnualEvaluation" },
    mortalityPressure: { state:"NotEvaluated", kind:"f64", reason:"StaticGenesisNoAnnualEvaluation" },
    hazardProbability: { state:"NotEvaluated", kind:"f64", reason:"StaticGenesisNoAnnualEvaluation" },
    vulnerability:     { state:"NotEvaluated", kind:"f64", reason:"StaticGenesisNoAnnualEvaluation" },
    rngRollNumerator:  { state:"NotEvaluated", kind:"int", reason:"StaticGenesisNoAnnualEvaluation" },
    healthScore:       { state:"Unavailable",  kind:"none", reason:"NotRecordedInTrustedSource", sourceContext:"StaticGenesisFixture" }
  }
}
```

**Availability envelope rule (F1).** Every optional field is exactly one of:
- `PresentSourced{kind,value[,bits],unit?,sourceRef}`
- `PresentComputed{…, calculationStage}`
- `NotEvaluated{kind,reason}`
- `Unavailable{kind,reason,sourceContext}`

The reason vocabulary is the closed set of spec §4.2. A required field is never wrapped in an envelope. In Genesis, **no** envelope carries a numeric value except the three inventory counts, `thresholdMm` and the sourced f32 values. **There is no numeric zero anywhere for an unknown value.**

### 2.6 Responsibilities that stay with A (view-only arithmetic)

- **View plane.** Local metres are `xMm/1000` and `yMm/1000`, computed from the integer fields only. `centredX/Z` are shown in the inspector as verified source values and are **not** used for drawing.
- **CSS projection.** `cssX = ox + (xM − camX)·s` and `cssY = oy − (yM − camY)·s`, with one uniform finite `s` in CSS px/m. The canvas backing store is `round(cssSize × devicePixelRatio)`, with `ctx.setTransform(dpr,0,0,dpr,0,0)`. Hit-testing happens in CSS px only.
- **Crown.** Pixel radius = `crownRadiusM.value × s` (radius, not diameter). Display rounding is A's job only.
- **Ordering.** Crowns are drawn in `rows` order, then stem centres, then the selection/focus glyph. Hit test: nearest squared CSS distance to the **stem centre**; ties broken by ordinal `treeId` comparison (`<` on strings, which is ordinal for `P30-nnn`); overlapping centres offer a chooser.
- **Action note.** A may display the annotation from `belowActionNote` with exactly the wording of `actionNote.comparison`. A **never** compares DBH itself and never says "eligible" or "ineligible".
- **Prohibitions.** A never feeds a display value back to Core, never computes BA/Dq/RD/N/ha, never invents a health colour and never mutates anything Core returned. Every Core object is frozen, so mutation throws in strict mode.

### 2.7 F3 `ObservationKeyV1` and the stale-selection fence

```text
ObservationKeyV1 = deepFrozen({
  token: "h0a-okey-v1:" + <lowercase hex SHA-256 of the canonical component text>,
  components: { readSchemaId, readSchemaMajor, readSchemaMinor,
    observationProvenance:"FixtureOnly", sourceBundleKind:"FixtureOnlyStaticBundle",
    domainTag, fixtureId, fixtureByteSha256, fixtureByteLength, catalogVersion, format, derivedSchemaVersion, sourceClass,
    experimentLineageId:"StaticBundle", originGeneration:"StaticBundle:"+fixtureByteSha256,
    growthModelVersion, rngModelVersion, annualAlgorithmVersion, simulationSeed,
    numericPolicyId:"ccf.h0a.numeric.f32-hex-be.v1",
    geometryModel, frameTransformId, coordinateOriginAxes:"sw-mm-xeast-ynorth", standAreaHaBits:"3db851ec",
    observedEcologicalYear:0, sourceStage:"Genesis",
    stepTargetYear:"NotApplicable", stepTransitionId:"NotApplicable",
    committedRevisionOrFrameId:"StaticGenesisFrame", coreContractVersion:"1.0.0" }
})
```

- **Canonical component text.** One `name=value` line per component in exactly the order above, each followed by `\n`. ASCII only, integers in decimal, no spaces. Core hashes it with its own SHA-256. **No literal token is given here.** It is computed, never typed in.
- **Package hash.** The package output hash is **not** a component: it would be circular, since it cannot sit inside its own package. Package identity is held externally (§5).
- **`keysEqual(a,b)`.** True only if the tokens are equal **and** every component is strictly equal.
- **Selection notification (A-owned emitter, contract-owned type):**
  `SelectionNotificationV1 = deepFrozen({ type:"ccf.h0a.selection", treeId:<string>, observationKey:<ObservationKeyV1>, via:"pointer"|"keyboard"|"list" })`.
  - Inspection only; there is no command semantics.
  - **Every consumer**, including A's own inspector, calls `CcfYear0Core.isCurrentSelection(obs, ev)`. That check is `keysEqual(obs.observationKey, ev.observationKey)` plus `ev.treeId` present exactly (case-sensitive) in `obs.rows`. False → ignore the event.
  - In H0, only one key ever exists in production. Core-only tests may construct an admitted **test-build** key from a different test fixture pin; A-only tests use an independently constructed immutable foreign-key **public-schema double**. No production verification global, alternate trust root or dynamic pin switch.
- **Not part of the key:** UI camera, selection or viewport state.

### 2.8 Atomicity and last-good-view boundary (revised)

**Production:** \`openGenesis()\` is called once. Any \`Refused\` renders only bounded error information plus disclosure, **no** map/list/inspector and no partial observation. There is no reload, alternate fixture admission, mutable production trust or second frame.

**Core test artifact only:** a refused controlled test-pin admission after a good test admission must leave the earlier complete observation deep-equal, frozen and associated with the same full F3 key. No partial row/key is substituted on refusal. This test is not a claim that production retries admission.

**A test doubles only:** A may receive a frozen \`Refused\`, incompatible schema version or foreign immutable F3 key from its separate view-harness factory. A must safely render or ignore it without Core test APIs or mutation of any trusted fixture.



## 3. Exact local paths and ownership (inside the #755 lanes; nothing created now)

The lane root is `R = /media/jer/Files/CCF/Operations/CCF-H0A-Year0Viewer-20261118/` (isolated, remote-less, per #755). The product directory is `Tools/MultiRackSitkaYear0Viewer/`. It does not exist on main and is not ignored (#753 §5.2).

| Owner | Worktree / branch | Exclusive allowlist (repo-relative) |
| --- | --- | --- |
| **Core writer (one)** | `R/ccf-h0a-core` · `feature/multi-h0a-core-year0-admission-20261118-v1` | `Tools/MultiRackSitkaYear0Viewer/core/year0-core.js`, `…/core/CONTRACT.md` (this §2 verbatim + changes only via Manager), `…/fixtures/p1d-30m-sitka-adult-v1.sitkap1fx.json` (byte copy of blob `8bf81a4364a5a6bacf104d72d354e74635d3b26c`, mode 0444), `…/build/assemble-year0-viewer.py`, `…/tests/core/core-verification.html`, `…/tests/core/core-verification.js`, `…/tests/core/core-test-only-entrypoint.js` (isolated test artifact only; never a production input) |
| **A writer (one)** | `R/ccf-h0a-viewer` · `feature/multi-h0a-a-year0-viewer-20261118-v1` | `Tools/MultiRackSitkaYear0Viewer/viewer/shell.template.html`, `…/viewer/viewer.css`, at most three `…/viewer/viewer-*.js`, `…/tests/viewer/viewer-verification.html`, `…/tests/viewer/viewer-verification.js` |
| **Packager** (recommended: the Core writer, sequentially, after A's freeze) | reads A's final head read-only at a recorded commit | output **only** to `R/artifacts/<UTC-id>/ccf-year0-viewer.html` and `manifest.json`. Never committed, never pushed. |
| Everyone | — | read-only: `specs/multirack-sitka-offline-year0-viewer-v1/**` and all other paths. Forbidden: `Assets/**`, `ProjectSettings/**`, `Packages/**`, `Tools/MultiRackSitkaLab/**`, `Tools/MultiRackSitkaP1Lab/**`, `Docs/Project/**`, `.vscode/**`, any `package.json`, lockfile or `node_modules`. |

**One history, no merge.**
1. Core works from `local/h0a-base` (`34185b8b`) and freezes at commit `Cf`.
2. After Manager review of `Cf`, the **custodian** fast-forwards the A branch to `Cf`, so A's tree contains Core files that A must not edit.
3. A commits on top. The mechanical gate is `git diff --name-only 34185b8b..Cf ⊆ Core allowlist` and `git diff --name-only Cf..A_head ⊆ A allowlist`. Any overlap is a STOP for Manager arbitration.

**Precondition (custodian, not a writer).** The #755 worktrees are cone-sparse with three root files. The custodian must run `git sparse-checkout add Tools/MultiRackSitkaYear0Viewer` in both worktrees and read it back before a writer starts. Otherwise cone mode makes `git add` refuse the new paths. This is an Ops-side change that needs its own instruction. It is not done here.

**Fixture copy (D3).** The Core writer needs an explicit Manager-granted read of blob `8bf81a43…` from `/media/jer/Files/CCF/Repository.git` (local and offline). It then asserts 32,567 B and SHA-256 `bcaaa5c3…` before committing. No fetch from #710/#725 and no regeneration.

---

## 4. D5: protected-P0 authority and an external pre/post baseline

### 4.1 Source authority (documentary, named)

| # | Authority | What it defines |
| --- | --- | --- |
| A1 | Owner-approved P1 four-file spec (16 Nov), `spec.md` §9.2 + m02, `requirements.md` REQ-B07 | the protected object (`Tools/MultiRackSitkaLab`, the qualified original P0) and the layered L0 history / L1 committed tree / L2 working tree / L3 ignored-file snapshot / L4 write-observation proof, each with what it does **not** prove |
| A2 | Manager #589 selection, recorded in A1 | P0 pin **commit `a7f41983c2f2f233b8fe999d1a5b10e72bdd2866`, subtree `f212c8f64fb28b83aa870c7b5b7c72e03051e46a`**, not on main. The historic `861b81d3`/`fa25ae6e` is docs-only. |
| A3 | Canonical shared bare store `/media/jer/Files/CCF/Repository.git` (AGENTS storage policy) | the committed-object authority **outside** the H0 store |
| A4 | Physical Ops `lanes.json`/board (hashes in §1.2) | the **names** of physical lanes that may carry P0/P1 trees. The custodian enumerates them; I opened none. |
| A5 | #735 custody records, cited in #748/#750 | #710 (to 23 Nov 18:00Z) and #725 (to 27 Nov 18:00Z) retention. Contents are off-limits to H0 work. |

### 4.2 Why sparse-tree absence proves nothing

The H0 store is a **shallow** fetch of main `34185b8b` only (#755 §3). P0 is not on main (A1 REQ-B07). So `Tools/MultiRackSitkaLab` is absent from the H0 tree **by construction**. That absence says nothing about any real P0 copy.

Protection is therefore a statement about **external** objects and trees (A3/A4) staying equal while the H0 writer works. The H0 candidate must also never contain or name a protected path.

### 4.3 Correct proof definitions and limitations (B-2)

**Sparse-tree absence** of \`Tools/MultiRackSitkaLab\` is *not* evidence that outside P0 bytes were protected. A valid H0-only Git **candidate history** check lists changed paths and verifies the full ancestry and Core→A diff allowlists (§3), with no change under original P0/P1 and unrelated production prefixes. This says nothing about out-of-band physical writes.

**B-1** names an approved canonical P0 commit/tree object pin. That Git identity and independently resolved object evidence can prove the original pin is still present, not that every working copy is unchanged. A ref-list SHA is refs **metadata**, not file-content equality. **B-2** confirms H0's expected lack of the P0 object/path in its shallow store, not real-world P0 integrity.

**B-3 required lawful byte-level evidence:** separately authorised native custodian first enumerates an **explicitly granted list of physical roots and file-content paths**, without following a path outside its read scope. For **every included file**, before and after, record a named SHA-256 **of actual complete file bytes**, its path, kind, mode, relative and absolute root, size and \`mtime_ns\` (latter two only metadata). Record a stable sorted set and count of every included path; require the same files, types, modes and exact per-file SHA-256 at both ends. New, missing, relocated, retargeted, unreadable, type/mode-changed or changed-digest file → STOP/discrepancy; no silent omission of ignored/untracked files. For tracked paths separately verify path, mode, clean tracked Git status, full HEAD/tree/blob pins and consistency at each end; **this does not replace working-byte SHA**. Dirty existing paths are *reported as-is*, never reset. Symlinks/special files or ambiguous root traversal require a separately approved explicit policy or STOP rather than silently following them.

**Metadata is not equality.** \`status --porcelain\`, \`HEAD\`, size, \`mtime_ns\` and path presence may detect changes but do **not** rule out same-size/restored-mtime content alteration. The original #756 B-3 claim "no content hashing needed" is rejected.

**B-4 = EXCLUDED #710/#725:** these two protected stores, their metadata and bytes are never part of H0's B-3 read authorisation. They remain separately owned. A B-3 PASS cannot state either one was unchanged. A partial B-3, B-1 or B-2 cannot prove **whole-P0 no write**.

**B-5 = permitted Ops file digests:** compare approved \`board/active.md\`, \`lanes.json\`, \`deployment.json\` before and after. If there is a legitimate intervening authorised mutation, record and independently reconcile its exact receipt and changed bytes; do not say unchanged.

**Writing constraints are a separate gate.** A future source worker must have actual independently checked filesystem/actor/tool containment and its limits/logs documented, not a promised "sandbox". Git history and per-file baseline observe only their respective surfaces; no unobserved actor or protected path becomes covered by inference. Neither a partial B-1…B-5 proof nor a no-\`dotnet\` statement clears original **.NET L4 FAIL38** or authorises an L4 waiver.

**Permitted outcome language:** "No drift detected **within the named and lawfully byte-hashed B-3 file set** between pre/post snapshots, with separately enumerated excluded stores and unobserved-write risks." Missing bytes/access, no verified included set or an unverified execution boundary means **PARTIAL / INCONCLUSIVE**, never whole-protection PASS.

### 4.4 Proposed custodian pre/post procedure — NOT EXECUTED

| Item | Read-only future pre and post evidence | Access/scope |
| --- | --- | --- |
| **B-1 P0 pin** | Resolve original approved \`a7f41983^{commit}\` and \`a7f41983:Tools/MultiRackSitkaLab\` == \`f212c8f6…\` in canonical store with optional Git locks disabled; capture pin object identity and separate refs-list digest. Reconcile legit refs movement. | Only exact named committed pin, not unbounded tree inspection. |
| **B-2 H0 store** | Confirm protected object not admitted to isolated H0 store; inspect H0 base and Core/A branch changed paths, independently compare allowed file sets. | H0 history/tree only. |
| **B-3 named physical files** | **After specific byte-read grant** enumerate every approved root/file with ordered \`root,path,kind,mode,SHA256(bytes),size,mtime_ns,tracked,HEAD,gitBlob,gitMode,cleanStatus\` fields as applicable; include ignored/untracked within exact approved set. Freeze signed/hashed manifest and non-public evidence path before Core writer edits. After A handoff repeat same list and verify complete content digest/set/mode equality; STOP on any unexplained drift. | Original #756 guesses \`MultiP0P1Integ687\`, \`MultiP1ChildB697\`, #639 are **prospective names only**; not approved physical access. #674 reportedly dirty — never clean. #710/#725 explicitly excluded. |
| **B-4 excluded stores** | No H0 inspection of protected #710/#725 at all; their owner custodians retain their separate integrity obligations. | **NO ACCESS.** |
| **B-5 Ops** | SHA-256 of the three named Ops files, observed dates/locks and any approved intervening transaction receipts. | Read-only approved Ops scope only. |

**No #758 native B-3 hashes, file enumeration, sandbox enforcement or post-baseline exist.** Without an approved per-file byte-read list, this table is a *proposal*, not evidence. Content hashes are not file copies; publication of source bytes remains forbidden.



### 4.5 JS-only substitution for the .NET ladder: requires an owner/security decision

H0 builds nothing against P0. No `ProjectReference`, `dotnet` or MSBuild output exists, so A1's L2–L4 "named build-input tree" is empty for H0.

**Proposal only:** a narrowly scoped JS-only H0 alternative may use actual approved B-3 per-file content SHA-256 and tracked pins, B-1/B-2 candidate evidence, B-5 reconciled Ops files and independently evidenced source-writer confinement. It does **not** establish equivalent L0–L4 whole-P0 proof; only the owner/security can explicitly adopt this narrower substitution for H0 after reviewing exclusions/residual risk (#753 OPEN-5). Default .NET L4 FAIL38 remains UNWAIVED.

The default **.NET L4 FAIL38 stays UNWAIVED**. H0 runs no `dotnet` process, so it neither reproduces nor clears that finding. G4 stays blocked.

### 4.6 D5: **BLOCKED**, separate actor-specific lawful route

No #758 worker has permission to physically read B-1/B-3 or to declare a protected baseline. **Neither a known pin nor GitHub read access grants that right.**

1. Manager #730 and owner/security must name **one actual native custodian/session**, check fresh Ops ownership and physical conflicts, then sign an exact **read-only** grant for B-1 original canonical pin, B-2 isolated H0 tree, each named B-3 root **and every permitted file-content path**, and only the three B-5 Ops files. Explicitly exclude **#710/#725** and reject full P0 scans. D-091: no arbitrary expiry.
2. Custodian first verifies that every included file may lawfully be read **as bytes**; metadata-only authority cannot support a B-3 PASS. Use non-mutating Git options, stop on unreadable/unknown/symlink escapes, and capture hashes with secure non-public evidence/manifest custody.
3. Before any implementation edit, record sorted per-file SHA-256, path/type/mode, tracked Git cleanliness and exact pin evidence, B-1/B-2 state, B-5 Ops snapshots. Publish a **hash-only scope-and-method** receipt and identify excluded/unobservable paths and actors. Do not publish protected source.
4. Obtain a **separate written owner/security decision** on whether this **partial H0 JS-only** procedure plus verified actual writer confinement suffices. No silent substitution or blanket L4 waiver. Until acceptance, **D5 BLOCKED / G2 HELD**.
5. Only if G2 eventually starts, post-handoff custodian repeats lawful B-3 *content* hashing and reconciles actual pre/post equality, Git tracked pins and Ops drift before independent review. If permission or data gaps remain, label **PARTIAL** and return for owner/security decision — never say entire P0/no writes proved.



## 5. Offline assembly, production-positive allowlist, CSP and fixed trust (future only)

1. **Exactly allowed production inputs:** one pinned \`fixtures/p1d-30m-sitka-adult-v1.sitkap1fx.json\`; one \`core/year0-core.js\` **production** script; one \`viewer/shell.template.html\`; one \`viewer/viewer.css\`; and a manifest-enumerated, ordered set of **one to three** \`viewer/viewer-*.js\` files. The assembler \`build/assemble-year0-viewer.py\` is a build tool, not embedded. Shell's \`<!--CCF:CSP-->\`, \`<!--CCF:STYLE-->\`, \`<!--CCF:FIXTURE-->\`, \`<!--CCF:CORE-->\`, \`<!--CCF:VIEWER-->\` placeholders each occur once. Exactly one A shell, one Core script and one fixture block.
2. **Fail-closed negative inputs:** NEVER inline/import \`tests/**\`, \`core-test-only-entrypoint.js\`, test-only alternate-pin artifact, Viewer doubles, an alternate production Core, third-party code, extra script/style, picker/upload or unreviewed module. Manifest exact input names and SHA-256 are **positive authority**. A script token deny-list by itself is insufficient; inspect every actual bundled byte and source graph.
3. **Fixture digest:** before base64 packaging assert exactly 32,567 bytes and SHA-256 \`bcaaa5c3d9b3d36ad8319ffcc361a6225a02b90281cef8435de7c52f8ff757c4\`. Embed as one whitespace-free line inside \`<script type="application/octet-stream" id="ccf-h0a-fixture-b64">\`. Production Core independently verifies that same fixed internal length/digest **before UTF-8 or JSON**. Self-declared fixture \`contentHash\` is never a trust root.
4. **CSP:** compute \`sha256-<base64>\` for each exact emitted inline executable JS and style body; CSP meta first in \`head\`, with \`default-src 'none'; script-src <hashes>; style-src <hashes>; img-src 'none'; connect-src 'none'; font-src 'none'; object-src 'none'; worker-src 'none'; base-uri 'none'; form-action 'none'\`. Hash-CSP and \`file://\` browser execution still need empirical verification.
5. **Static refusal:** reject external URL \`src/href\`, \`http:\`, \`https:\`, network/XHR/WebSocket/EventSource/beacon/fetch, modules/dynamic import, eval/Function/Worker, browser storage/cookies and \`crypto.subtle\`; forbid any \`style=\` or unapproved inline JS. Reject any occurrence of executable \`CcfYear0CoreVerification\`, \`CCF_H0A_VERIFICATION_HARNESS\`, caller-pin \`admitBytes\` or an equivalent trust-changing function **anywhere in final production**. No "inside Core" exemption or acceptance on the strength of shell flag absence.
6. **Independent P-02 production audit:** a non-author must review **actual final HTML bytes**, the exact script/style inventory and source→inline SHA matches, plus actual callable global surface: one recursively frozen \`CcfYear0Core\` with \`contract\`, \`contractVersion\`, \`openGenesis\`, \`keysEqual\`, \`isCurrentSelection\`; no second global, alternate admission pin, runtime switch, test hook, dynamic script loader or test input. Compare final production Core and A JS content to pinned production inputs. Checks must examine semantics, not only named-string absence.
7. **Determinism and independent trust:** no time/path embedded in HTML. External \`manifest.json\` records exact Core and A commit, ordered input paths/hashes, fixture hash, assembler hash, Python version, output SHA-256 and assembly UTC. Independent nonauthor rebuilds byte-identically and #730 posts **only package SHA** as external root; no HTML self-hash.
8. **Future offline browser qualification only:** local \`file://\` opening in fresh temporary Linux Firefox 159 and Chromium 156 profiles with OS network disabled, DevTools showing zero requests, CSP enforced and no alternate globals. If hash-CSP or browser cannot run without server, STOP for new Manager/owner decision; never silently add Host, server or weaken CSP.
9. **Local-only package:** final HTML and manifest under \`R/artifacts/<UTC-id>/\` only, never committed/pushed. Check package SHA against Manager receipt before opening, verify no viewer code appears on public GitHub. Push protections are defence-in-depth, not sufficient write containment proof. Fixture import, code/test work and all package operations require later distinct G2 grants; none performed here.



## 6. Future focused tests and independent acceptance — **NOT RUN**

**Separation:** **PROD** means final fixed-pin Core/package tests. **CORE-TEST** means **separate Core-only test build/entrypoint**, reusing one decoder, deliberately supplying test bytes/test digest for S5–S12 negatives; absolutely not compiled into production. **VIEWER-DOUBLE** means A's immutable fixed public schema doubles; no fixture, alternate pin or Core test hook. Both a test and its actual packaged artifact require independent review, not a report of success invented in this document. Future hosts Firefox 159 and Chromium 156 on Linux, Brave optional; no Node/.NET/Unity/Windows.

| ID | Required future verification (test mechanism explicit) | Owner |
| --- | --- | --- |
| C-01 | **PROD:** SHA-256 FIPS KATs (\`""\`, \`"abc"\`, 448-bit message), exact fixture digest, \`CryptoSelfTestFailed\` on KAT failure | Core |
| C-02 | **PROD:** \`openGenesis()\` admits fixed fixture, 100 source-order \`P30-001\`…\`P30-100\`, 100/100 \`live\`, age 30, deep freeze | Core |
| C-03 | **PROD:** one-byte **same-length** tamper in embedded fixture without changing pin refuses \`DigestMismatch\` at S3 **before parsing**; no observation | Core |
| C-04 | **PROD:** changed byte length ±1, invalid/whitespace base64, missing/duplicate block → correct S0–S2 refusal; parser never reached | Core |
| C-05 | **CORE-TEST:** separate test-pin malformed keys/types/domain/catalog/format/fixture/area bits/transform/model/seed give precise S6–S8 refusals | Core |
| C-06 | **CORE-TEST:** duplicate, missing, case-wrong or reordered ID/101 rows → roster refusals | Core |
| C-07 | **CORE-TEST:** integer coordinate −1 or 30000 → \`CoordinateOutOfDomain\` | Core |
| C-08 | **CORE-TEST:** f64/7-hex/uppercase/NaN/Inf/radius below 0.1 → numeric refusals | Core |
| C-09 | **PROD** all 200 forward X/Z bits; **CORE-TEST** altered \`unityX\` bit → \`TransformBitMismatch\` | Core |
| C-10 | **PROD:** 100 source f32 DBH/height conversions bit-equal; no crown inverse recompute | Core |
| C-11 | **PROD:** F1 NotEvaluated CI/neighbours/pressure/hazard/RNG, Unavailable BA/Dq/RD/N/ha and health, never fabricated numeric zero | Core |
| C-12 | **PROD:** exactly 12 strict \`dbhMm < 170\` notes; \`dbhMm = 170\` false; none "eligible"; all living stems retained | Core |
| C-13 | **PROD + CORE-TEST:** full F3 key token/canonical components and equality; foreign-key/case-wrong/unknown-ID stale rejection, no alternate production pin | Core |
| C-14 | **CORE-TEST ONLY:** refused later controlled test admission cannot mutate an earlier deep-frozen good test observation/key; no production retry exists | Core |
| V-01 | **VIEWER-DOUBLE / PROD:** 3m crown radius ×20 CSS px/m = 60 CSS px; 5m scale recalc | A |
| V-02 | **VIEWER-DOUBLE / PROD:** nearest stem-centre hit, ordinal tie and overlap chooser; crowns don't win | A |
| V-03 | **VIEWER-DOUBLE / PROD:** pan/zoom/fit, DPR1/2 and resize preserving CSS hit map | A |
| V-04 | **VIEWER-DOUBLE / PROD:** accessible 100 IDs through list/search/Prev/Next/Tab/Enter/Space/Escape; visible focus/live announcement | A |
| V-05 | **VIEWER-DOUBLE / PROD:** P30-001 and P30-053 anchors, "Live" not health, CI NotEvaluated/aggregates Unavailable, 100/100 | A |
| V-06 | **PROD:** instrumented zero network/storage APIs through all viewer interactions in isolated offline profile | A |
| V-07 | **VIEWER-DOUBLE:** frozen \`Refused\` and incompatible schema → no map/list/inspector, disclosure always visible; no Core test API used | A |
| V-08 | **VIEWER-DOUBLE:** foreign complete F3 key and stale/case-wrong ID callbacks ignored; A never modifies a Core observation | A |
| P-01 | **PACKAGER:** exact positive production allowlist, no tests, CSP hashes and stable two-run package digest; exclusive branch diffs | packager/reviewer |
| P-02 | **PACKAGER + independent nonauthor:** full final HTML/source JS read proves **no alternate caller pin, test global, activation code, test entrypoint or second trust root**; one immutable Core facade; no string-scan-only PASS | reviewer |
| R-01 | **Future manual reviewer:** 1280×720, 1920×1080, 360 CSS px, 200% text, Firefox/Chromium offline disclosure/rendering | reviewer |
| R-02 | **Independent exact-candidate:** review C/V/P evidence, full packed bytes, nonauthor deterministic SHA, lawful D5 post-manifest and actual writer containment; scoped G2 verdict only | reviewer |

**Interpretation:** altered production bytes **must** be refused at digest-before-parse. Schema-negative tests require a test-owned matching pin in a **separate artifact** and prove internal decoder rejection only. Viewer doubles test A's DTO consumption only. None of these tests has been executed by #758.



## 7. Blockers, decisions needed and proposed G2 routing

**Decisions needed before any G2 source permit:**

| # | Decision | Who | Recommendation |
| --- | --- | --- | --- |
| DN-1 | Adopt contract `ccf.h0a.core-read` v1.0.0 (§2) as written or amended; optionally one **non-Claude** targeted technical review first | Manager #730 | adopt after a short non-author review (I am the author) |
| DN-2 | Browser-JS Core as the sole decoder (vs .NET build-time projection) | Manager | JS Core (#753 OPEN-2) |
| DN-3 | Pure-JS SHA-256, no `crypto.subtle` | Manager | adopt (§2.3) |
| DN-4 | D5 custodian/session **and named per-file content-read grant** (§4.6) | owner / Manager | exact B-1/B-2/B-3/B-5 paths; content SHA-256 on each included B-3 file; exclude #710/#725 |
| DN-5 | H0 **partial content-hashed JS-only** substitution, with observed writer confinement | **owner / security** | written scoped ruling only after lawful B-3 pre-baseline; L4 FAIL38 stays UNWAIVED |
| DN-6 | "Current desktop browsers" evidence matrix = Firefox 159 + Chromium 156 on Linux (Windows/macOS not observable here) | owner | accept for local non-public G2 evidence |
| DN-7 | **Core-only alternate-pin test artifact** separate from **A-only immutable fixed-schema doubles** | Manager / independent reviewer | no production test global; independently inspect final positive-allowlist output; browser harnesses later |
| DN-8 | Packager = Core writer, sequentially after A's freeze | Manager | adopt |
| DN-9 | D3 canonical-store blob read for the fixture copy, granted to the Core writer | Manager | grant at Core G2 |
| DN-10 | Custodian adds the sparse cone `Tools/MultiRackSitkaYear0Viewer` in both worktrees, and the custodian fast-forwards A to `Cf` (§3) | Manager → custodian | adopt |
| DN-11 | Action-note wording at the 170 mm boundary (F-1): strict `<`, annotation only, operator unratified | Manager | adopt |
| DN-12 | Real writer-session write confinement | Manager / security | independently verify actual sandbox/mount/tool scope and residual unobserved writes, never assume it from prose |
| DN-13 | Actor-specific G2 non-release permits for Core and A, separate claims, named non-author reviewer and manual browser reviewer | Manager / owner | per #753 §6 |

**Gates.** G3 is not needed for H0-A as scoped; STOP if any Host, network, upload, storage or picker appears. G4 (main, public, distribution) is **blocked**: default L4 FAIL38 is UNWAIVED and no public-source consent exists. The canonical Multi decision ID/crosswalk blocks promotion only.

**Proposed routing:**
1. **Mandatory nonauthor Manager #730 review** of this #758 DRAFT changed clauses and complete contract; then explicitly resolve DN-1/2/3/7/8/11. No self-adoption.
2. DN-4 separately authorised native custodian + lawfully **content-hashed B-3** D5 pre-baseline. Incomplete grant/byte hashes keep D5 BLOCKED.
3. DN-5 owner/security explicit **partial JS-only** proof ruling and actual writer confinement assessment; DN-6 target browser matrix decision.
4. DN-10 sparse cone.
5. Core G2 claim (T02–T04): `Cf` + C-01…C-14 → Manager check.
6. Custodian fast-forwards A to `Cf`, then the A G2 claim (T05–T07): V-01…V-07.
7. Packager P-01 **and independent P-02**, including actual production emitted-byte/alternate-root inspection.
8. Custodian D5 **post** baseline.
9. Independent R-01 + **R-02** full candidate/package/D5 review.
10. Manager scoped verdict (G2 candidate only).

---

## 8. Declaration, verdict requested and explicit remaining gates

**This result corrects the full proposed H0-A contract as a DRAFT only.** Its proposed adoption remains Manager #730's decision, not the #758 author’s. #757 review findings B-1, B-2 and M-1 are **addressed in text** but not independently re-reviewed, implemented or validated.

**No physical or code execution:** no source edits, public code, fixture import, actual P0 read, #710/#725 metadata or contents, physical Ops, native worktree/board/lease, SHA baseline, tests, build, application browser launch, Unity/Windows, Host/network/storage, main merge or L4 waiver. The historical hashes/fixture observations in §1 are **#756 author reports**, not new #758 findings. Only a new GitHub documentation file is authorised. All H0 acceptance tests remain **NOT RUN**.

**Manager #730 next:** compare this one-file branch against starting main and #756; inspect corrected §§2.1/2.2/2.7–2.8, §3, §§4.3–4.6, §§5–7 and unchanged F1/F3 schema and source pins; give **APPROVED DOCUMENT QUALITY / NEEDS CHANGES / REJECTED** (not source/feature acceptance). Only then obtain a named lawful native D5 grant and B-3 pre-baseline, separate owner/security JS-only ruling, G2 Core/A actual owners/permits, future package review. G1 #755 previously prepared empty application lanes; it did not authorise source. **D5 BLOCKED; G2 HELD; G3 Host OUT OF SCOPE; G4 release/main/public HELD; default .NET L4 FAIL38 UNWAIVED.**

**Dashboard/Drive mirror synchronization: NOT VERIFIED. User action for this docs handoff: none; future owner/security read and substitution decisions remain separately required.**
