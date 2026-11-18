# Multi #756 — H0-A frozen Core→Viewer read contract and D5 protected-P0 baseline preflight (DRAFT result)

**18 November 2026 · claim [#756](https://github.com/jermur788/CCF/issues/756) · Manager [#730](https://github.com/jermur788/CCF/issues/730).**
**Status: DRAFT RECOMMENDATION FOR MANAGER TECHNICAL ADOPTION. Contract part: COMPLETE DRAFT. D5 protected baseline: BLOCKED (no lawful baseline read authority has been granted to anyone); narrow custodian route given in §4.6. NOT APPROVED, NOT IMPLEMENTED, NOT TESTED, NO G2.**

| | |
| --- | --- |
| Worker | Anthropic **Claude Opus 5.5** (`claude-opus-5-5`), Claude Code (Claude Agent SDK) inside T3 Code, native Linux host `jer-laptop`, user `jer`. Effort settings are not exposed and not claimed. |
| Registration | [RUNNING comment 6986743865](https://github.com/jermur788/CCF/issues/756#issuecomment-6986743865), posted before any edit. **No physical grant held or requested.** |
| Fresh main | `34185b8b71ba280c89091290345e537ef107bdf4` (`origin/main` after a fetch, 2026-11-18 ~17:40Z) |
| Branch | `docs/multi-756-preg2-contract-protection-result-20261118-v1`, created from that exact commit through the GitHub API. No local branch, worktree or ref was created in any CCF store. |
| Only output | this one Markdown file |
| Independence note | In earlier sessions, Claude Opus 5.5 authored #639 P1-F0 and the #567 P0 save-response repair whose handoff head is `a7f41983`. This document **names** `a7f41983` as the P0 pin authority but makes no judgement of its quality. I also authored this contract, so I must not be its reviewer. A **non-Claude** technical reviewer is recommended before adoption (§7). |

---

## 1. Read-only preflight and provenance

### 1.1 Documents read in full

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

### 1.2 Physical facts observed (read-only)

| Observation (2026-11-18T17:49:39Z) | Result |
| --- | --- |
| `ZX20/CCF-Ops/board/active.md` SHA-256 | `4fa750eb…3994`, mtime 17:28:31Z. **Equal to the #755 "after" hash.** |
| `ZX20/CCF-Ops/lanes.json` SHA-256 | `c1d14719…7d9e`, mtime 17:28:31Z. **Equal to the #755 "after" hash.** 25 lanes, including `MultiH0ACore` and `MultiH0AViewer`. |
| `ZX20/CCF-Ops/deployment.json` SHA-256 | `8c66b290…7b18`, unchanged since 2026-11-15. |
| Ops scripts run | **none.** I hashed the three files, parsed `lanes.json` (lane name, path, purpose only) in RAM, and took no lock. Reading may update the access time on exFAT; nothing was written. |

**Conclusion:** physical Ops is unchanged since the #755 transaction. Both H0 lanes are still RESERVED / NOT RUNNING.

### 1.3 Public fixture bytes: re-observed in RAM only

I fetched the public blob at `feature/multi-p1-639-f0-30m-foundation-20261116-v1` tip `e302f15b`, path `Tools/MultiRackSitkaP1Lab/fixtures/derived/p1d-30m-sitka-adult-v1.sitkap1fx.json`, through `gh api` into a Python process (`-I -B`). Nothing was written to disk and no copy exists.

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

### 1.4 Deliberately not read or touched

- #710 and #725 stores: contents and metadata were **not** accessed.
- The H0 sparse worktrees: not opened.
- The local #639 worktree and any physical P0 tree: not opened.
- No Ops script, lock, board, lanes or lease change.
- No source, fixture copy, test, build, browser, Node, .NET, Unity, Windows or Host.
- No main edit and no L4 policy change.
- The unrelated dirty checkout `CCF-Primary` (branch `ops/workflow-foundation-v1`) was used only for read-only `git fetch`/`git show` of documentation refs. Its working files were not touched.

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
- **Verification-only surface.** The second global `CcfYear0CoreVerification` exists **only** when a Core-owned harness page sets `globalThis.CCF_H0A_VERIFICATION_HARNESS = true` before loading Core. It exposes `admitBytes(bytes: Uint8Array, pin: {byteLength, sha256Hex})` and `sha256Hex(bytes)` (the known-answer test seam).
  - The packager **fails closed** if the packaged output contains the string `CCF_H0A_VERIFICATION_HARNESS` outside Core's own definition, or if the shell sets it.
  - A must never reference either name.
  - Rationale: same-length tamper, schema and stale-key negatives need a different pin without weakening the production path. The production path always uses the compiled-in pin.

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

The digest gate (S3) runs **before** any parse, so the schema gates only ever see the pinned bytes in production. They are codec/interpretation self-checks there, and are exercised as negatives only through the verification seam.

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

`<…>` marks a value Core fills **from the admitted bytes or by its own computation**. Literal constants are the pinned identities from §1.3 and the approved spec.

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
  - In H0, only one key ever exists in production. The fence is exercised by verification-seam keys (a different pin plus altered bytes gives a different fixture SHA, hence a different key).
- **Not part of the key:** UI camera, selection or viewport state.

### 2.8 Atomicity and "retain last good view"

`openGenesis()` runs once on load. On `Refused`, A shows only the refusal `code`, the `detail`, the always-visible disclosure banner and no map, list or inspector.

Because H0 has no reload, upload or second source, "retain prior view" applies to the verification seam only. A refused `admitBytes` after a successful admission must leave the earlier observation deep-equal, still frozen and still the current key (test C-14).

---

## 3. Exact local paths and ownership (inside the #755 lanes; nothing created now)

The lane root is `R = /media/jer/Files/CCF/Operations/CCF-H0A-Year0Viewer-20261118/` (isolated, remote-less, per #755). The product directory is `Tools/MultiRackSitkaYear0Viewer/`. It does not exist on main and is not ignored (#753 §5.2).

| Owner | Worktree / branch | Exclusive allowlist (repo-relative) |
| --- | --- | --- |
| **Core writer (one)** | `R/ccf-h0a-core` · `feature/multi-h0a-core-year0-admission-20261118-v1` | `Tools/MultiRackSitkaYear0Viewer/core/year0-core.js`, `…/core/CONTRACT.md` (this §2 verbatim + changes only via Manager), `…/fixtures/p1d-30m-sitka-adult-v1.sitkap1fx.json` (byte copy of blob `8bf81a4364a5a6bacf104d72d354e74635d3b26c`, mode 0444), `…/build/assemble-year0-viewer.py`, `…/tests/core/core-verification.html`, `…/tests/core/core-verification.js` |
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

### 4.3 Definitions

- **Directory absence (not evidence).** `Tools/MultiRackSitkaLab` does not exist in the H0 tree.
- **Zero-protected-hit (evidence).** All of (a)–(d) hold:
  - (a) **L0-analogue:** `git log --name-only 34185b8b..<head>` in the H0 store lists no path under the protected prefixes `Tools/MultiRackSitkaLab/`, `Tools/MultiRackSitkaP1Lab/`, `Assets/`, `ProjectSettings/`, `Packages/`.
  - (b) **L1-analogue:** both branch diffs ⊆ their allowlists (§3).
  - (c) **External equality:** every pre-baseline item B-1…B-5 equals its post value, byte for byte.
  - (d) **Scope:** the writer session ran no `dotnet`/`node`/`npm`. Writes were confined by the harness filesystem sandbox to the lane worktree and `R/artifacts/`, and the writer's handoff declares the session's tools.

### 4.4 Proposed pre/post procedure (custodian-run only, after a grant; NOT run here)

| Item | Pre (before the Core writer's first edit) and post (after A's handoff, before review) | Access class |
| --- | --- | --- |
| B-1 committed P0 pin | `git -C Repository.git --no-optional-locks rev-parse a7f41983^{commit}` and `rev-parse a7f41983:Tools/MultiRackSitkaLab` == `f212c8f6…`; plus a SHA-256 of `git for-each-ref --format='%(objectname) %(refname)'` (the #755 refs-hash method) | canonical store metadata; read-only |
| B-2 H0 store cannot carry P0 | `git -C R/git/CCF-H0A-Year0Viewer.git cat-file -e a7f41983` **expected to fail** (object absent); `ls-tree local/h0a-base -- Tools/MultiRackSitkaLab` empty | H0 store; read-only |
| B-3 non-protected physical P0/P1-bearing trees | For each tree the custodian names from A4, excluding #710/#725 (candidates by name only: `MultiP0P1Integ687`, `MultiP1ChildB697`, the #639 worktree cited by #753; **#674 is dirty: record it as-is, never clean it**): `rev-parse HEAD`, `status --porcelain --ignored --untracked-files=all -- Tools/MultiRackSitkaLab`, and an L3-style manifest of (path, size, mtime_ns) under that subtree. No content hashing is needed beyond what A1 L3 already allows. | **needs an explicit read grant** |
| B-4 #710 and #725 | **NO ACCESS by this baseline.** Record only the Ops row/lane text already hashed in B-5. Their custodian may, under their own lawful custody, run their existing HEAD/clean check at release. H0 neither requires nor substitutes for it. | none |
| B-5 Ops | SHA-256 of `board/active.md`, `lanes.json`, `deployment.json` (a change made by an authorised Ops transaction is reconciled, not treated as a hit) | read-only |

**During the work:** only the writer's harness filesystem sandbox (write scope = its worktree plus `R/artifacts/`), local `git`, `python3 -I -B` for the assembler and the browsers. No `.NET`, Node or npm.

**Residual risks (disclosed):**
- A process outside the sandbox, or a sandbox escape, could write without B-3 noticing in-place same-size/same-mtime edits.
- #710/#725 integrity during H0 rests on non-access plus the sandbox, **not** on an H0 measurement.
- Hooks and Git config can be bypassed (#755 §3).
- The canonical store's own refs may legitimately move through unrelated authorised pushes. B-1 compares the pin objects strictly, while refs-hash changes are reconciled against GitHub activity.

### 4.5 JS-only substitution for the .NET ladder: requires an owner/security decision

H0 builds nothing against P0. No `ProjectReference`, `dotnet` or MSBuild output exists, so A1's L2–L4 "named build-input tree" is empty for H0.

**Recommendation:** for this JS-only lane, B-1…B-5 plus §4.3 (a)–(d) replace the L0–L4 execution. This document **cannot adopt that**: owner/security must approve the substitution explicitly (#753 OPEN-5).

The default **.NET L4 FAIL38 stays UNWAIVED**. H0 runs no `dotnet` process, so it neither reproduces nor clears that finding. G4 stays blocked.

### 4.6 D5 status: **BLOCKED**, and the narrow safe route

**Blocker.** No document read grants anyone a read-only baseline authority over B-1/B-3. The packet forbids inferring that authority, so I did **not** resolve the pin objects in the canonical store or open any physical tree.

**Narrowest route (in order):**
1. **Owner/Manager #730** names one native custodian session (not the Core/A writer and not this author). The grant covers **only** B-1, B-2, B-3 (an explicitly listed tree set) and B-5, read-only, with `GIT_OPTIONAL_LOCKS=0`/`--no-optional-locks`, and explicitly **excludes** #710/#725. No expiry (D-091).
2. The custodian runs the **pre** baseline and publishes a hash-only GitHub receipt (values plus SHA256SUMS of the evidence under `/media/jer/Files/CCF/Verification/CCF-H0A-Year0Viewer-20261118/d5/pre/`). No source text is published.
3. Owner/security rules on §4.5.
4. Only then may a Core G2 permit be issued. The **post** baseline runs after A's handoff, before the independent review.

---

## 5. Offline assembly, CSP and digest trust plan

1. **Inputs.** Core's fixture file, `year0-core.js`, A's `shell.template.html`, `viewer.css` and `viewer-*.js`, all at recorded commits in clean worktrees. The shell contains exactly the placeholders `<!--CCF:CSP-->`, `<!--CCF:STYLE-->`, `<!--CCF:FIXTURE-->`, `<!--CCF:CORE-->`, `<!--CCF:VIEWER-->`, each exactly once.
2. **Assembler** (`python3 -I -B`, stdlib only, no network):
   - assert the fixture is 32,567 B and SHA-256 `bcaaa5c3…`, then base64-encode it on one line with no whitespace into the octet-stream block;
   - inline Core, then the viewer scripts, then the style;
   - compute `sha256-<base64>` for each inline `<script>` (JS) and `<style>` body;
   - write the CSP `<meta http-equiv="Content-Security-Policy" content="default-src 'none'; script-src 'sha256-…' …; style-src 'sha256-…'; img-src 'none'; connect-src 'none'; font-src 'none'; object-src 'none'; worker-src 'none'; base-uri 'none'; form-action 'none'">` as the first child of `<head>`;
   - **fail closed** if the output contains: `http:`, `https:`, `//` URL forms in `src`/`href`, `fetch(`, `XMLHttpRequest`, `WebSocket`, `EventSource`, `sendBeacon`, `import(`, `<script type="module"`, `localStorage`, `sessionStorage`, `indexedDB`, `document.cookie`, `eval(`, `new Function`, `Worker(`, `crypto.subtle`, a `style="` attribute (which the hash-CSP would block), or `CCF_H0A_VERIFICATION_HARNESS` outside Core's definition.
3. **Deterministic output.** No timestamps or paths inside the HTML. `manifest.json` (outside the HTML) records input commits, per-input SHA-256, output SHA-256, the fixture digest, the assembler file SHA-256, `python3 --version` and the UTC time.
4. **Code-integrity root (H0-R008).**
   - The fixture pin is anchored by the approved specs, #753's independent public re-hash and §1.3's re-hash. #730's adoption should restate it as the Manager-published pin.
   - The **package** root is a **non-author reproducible rebuild**: the reviewer reruns the same assembler at the same commits and obtains a byte-identical output hash.
   - #730 then posts **that hash only** in a GitHub receipt. An in-document self-hash is never the root.
5. **Browser checks: acceptance choice.**
   - CSP-via-`<meta>` and `file://` behaviour are **UNVERIFIED**. CSP is defence-in-depth.
   - The primary no-network proof is the static fail-closed scan plus the reviewer's run in a **fresh temporary profile** with networking unavailable at the OS level, plus the devtools Network panel showing zero requests.
   - **Implementation-time STOP:** if a target browser refuses to run the inline hashed scripts from `file://`, or needs a server to render, the work STOPs and returns to #730/owner. Do not add a server and do not drop the CSP silently.
6. **Opening.** Run `sha256sum` on the file, compare it with the posted hash, then open it locally. No server, Host, picker or network. Nothing is pushed: hooks and `pushInsteadOf` per #755, plus a writer-packet ban on `gh`/`curl`/SSH uploads. The reviewer checks GitHub for any new `Tools/MultiRackSitkaYear0Viewer/` path.

---

## 6. Focused test and independent review matrix (ALL FUTURE / NOT RUN)

**Host.** Browser-opened harness pages (`tests/core/…html`, `tests/viewer/…html`) in **Firefox 159 and Chromium 156 on Linux** (`jer-laptop`). Brave 1.97 is an optional third Chromium check. **No Node runner, no .NET, no Year1 requalification, no Unity or Windows.** Run once per candidate commit.

| ID | Check (positive / negative) | Owner → independent check |
| --- | --- | --- |
| C-01 | SHA-256 KATs (`""`, `"abc"`, 448-bit) and fixture digest equal to pin | Core |
| C-02 | Production `openGenesis()` → `Admitted`; 100 rows in source order; 100/100 live; all ages 30; deep-frozen (recursive `Object.isFrozen`) | Core |
| C-03 | Same-length single-byte flip → `DigestMismatch`, no observation | Core |
| C-04 | Length ±1 → `ByteLengthMismatch`; whitespace or a bad character in base64 → `Base64Invalid`; zero or two blocks → `PackageBlock*` | Core |
| C-05 | Seam with an altered fixture + matching test pin: unknown root key; missing key; wrong type; wrong domain, catalog, format or fixture ID; area bits ≠ `3db851ec`; wrong transform ID; wrong seed or model → exact refusal codes | Core |
| C-06 | Seam: duplicate ID, `p30-001` case swap, missing `P30-100`, 101 rows, reordered IDs → `RosterIdInvalid`/`RosterCountMismatch` | Core |
| C-07 | Seam: `xMm` = 30000 or −1 → `CoordinateOutOfDomain` | Core |
| C-08 | Seam: `k:"f64"`, 7-hex, uppercase hex, `7fc00000` (NaN), `7f800000` (Inf), radius `3dcccccc` (just below 0.1) → codec refusals | Core |
| C-09 | All 200 forward X/Z bits equal; one altered `unityX` bit → `TransformBitMismatch` | Core |
| C-10 | `dbhCm`/`heightM` forward conversion equal for all 100 (proposed addition) | Core |
| C-11 | Availability: CI/neighbours/pressure/hazard/vulnerability/RNG `NotEvaluated(StaticGenesisNoAnnualEvaluation)`; N/ha/BA/Dq/RD `Unavailable(StageAggregateNotSupplied)`; health `Unavailable`; no numeric field equals 0 for an unknown value | Core |
| C-12 | `belowActionNote` true for exactly 12 rows; the `dbhMm`=170 row is false and never labelled eligible (F-1) | Core |
| C-13 | Key: token prefix and 64-hex format; `keysEqual(k,k)`; a seam key from a different fixture SHA is unequal; `isCurrentSelection` false for a foreign key, a case-swapped ID or an unknown ID | Core |
| C-14 | A refused seam admission after a good one leaves the first observation deep-equal and current | Core |
| V-01 | 3 m radius at 20 CSS px/m = 60 CSS px; scale bar labelled 5 m re-measured after zoom/resize | A |
| V-02 | Nearest-centre hit, ordinal tie, overlap chooser; crown size never wins | A |
| V-03 | Fit/pan/zoom clamps; DPR 1/2 and resize keep CSS hit results and source coordinates | A |
| V-04 | Keyboard: all 100 IDs via list/search/Prev/Next/Tab/Enter/Space/Escape; visible focus; live announcement | A |
| V-05 | Inspector truth: P30-001 and P30-053 anchors (spec §5); "Live" is not health; CI "Not evaluated in static Year 0"; health and stand metrics "Unavailable"; 100/100 | A |
| V-06 | Instrumented zero calls: wrap `fetch`, XHR, `WebSocket`, `sendBeacon` and storage setters before load; count stays 0 through every interaction | A |
| V-07 | Refused → no map, list or inspector; disclosure visible; contract-version mismatch notice | A |
| P-01 | Assembler fail-closed scan, CSP hashes, determinism (two runs give the same hash), allowlist diff gates (§3) | Packager → reviewer |
| R-01 | Real captures at 1280×720, 1920×1080, 360 CSS px, 200% text zoom in Firefox 159 and Chromium 156; networking off; banner visible | A captures → **non-author manual reviewer** |
| IR-01 | Non-author review of exact diffs, contract conformance (§2), C/V/P/R evidence, reproducible package hash, D5 post-baseline and GitHub no-new-viewer-path check; verdict scoped to the **isolated G2 candidate only** | reviewer named by #730 |

---

## 7. Blockers, decisions needed and proposed G2 routing

**Decisions needed before any G2 source permit:**

| # | Decision | Who | Recommendation |
| --- | --- | --- | --- |
| DN-1 | Adopt contract `ccf.h0a.core-read` v1.0.0 (§2) as written or amended; optionally one **non-Claude** targeted technical review first | Manager #730 | adopt after a short non-author review (I am the author) |
| DN-2 | Browser-JS Core as the sole decoder (vs .NET build-time projection) | Manager | JS Core (#753 OPEN-2) |
| DN-3 | Pure-JS SHA-256, no `crypto.subtle` | Manager | adopt (§2.3) |
| DN-4 | D5 custodian named + read grant scope (§4.6 step 1) | owner / Manager | grant exactly B-1, B-2, listed B-3 trees and B-5; exclude #710/#725 |
| DN-5 | JS-only baseline substitution for the .NET L0–L4 ladder (§4.5) | **owner / security** | approve for H0 only; L4 FAIL38 stays UNWAIVED |
| DN-6 | "Current desktop browsers" evidence matrix = Firefox 159 + Chromium 156 on Linux (Windows/macOS not observable here) | owner | accept for local non-public G2 evidence |
| DN-7 | Harness pages, not Node, for tests | Manager | adopt (#753 OPEN-6) |
| DN-8 | Packager = Core writer, sequentially after A's freeze | Manager | adopt |
| DN-9 | D3 canonical-store blob read for the fixture copy, granted to the Core writer | Manager | grant at Core G2 |
| DN-10 | Custodian adds the sparse cone `Tools/MultiRackSitkaYear0Viewer` in both worktrees, and the custodian fast-forwards A to `Cf` (§3) | Manager → custodian | adopt |
| DN-11 | Action-note wording at the 170 mm boundary (F-1): strict `<`, annotation only, operator unratified | Manager | adopt |
| DN-12 | Writer harness filesystem sandbox (write scope = own worktree + `R/artifacts/`) as a G2 condition | Manager / security | adopt |
| DN-13 | Actor-specific G2 non-release permits for Core and A, separate claims, named non-author reviewer and manual browser reviewer | Manager / owner | per #753 §6 |

**Gates.** G3 is not needed for H0-A as scoped; STOP if any Host, network, upload, storage or picker appears. G4 (main, public, distribution) is **blocked**: default L4 FAIL38 is UNWAIVED and no public-source consent exists. The canonical Multi decision ID/crosswalk blocks promotion only.

**Proposed routing:**
1. DN-1 (with optional review), DN-2, DN-3, DN-7, DN-8, DN-11.
2. DN-4, then the custodian D5 **pre** baseline receipt.
3. DN-5, DN-6.
4. DN-10 sparse cone.
5. Core G2 claim (T02–T04): `Cf` + C-01…C-14 → Manager check.
6. Custodian fast-forwards A to `Cf`, then the A G2 claim (T05–T07): V-01…V-07.
7. Packager P-01.
8. Custodian D5 **post** baseline.
9. Independent R-01 + IR-01.
10. Manager scoped verdict (G2 candidate only).

---

## 8. Declaration

**NO SOURCE CODE, NO TESTS, NO BUILD, NO BROWSER LAUNCH, NO UNITY / WINDOWS / HOST, NO PHYSICAL MUTATION.** Specifically:
- no Ops board, lanes, lease or lock change;
- no H0 worktree opened;
- no fixture copy written;
- no #710/#725 access;
- no P0 tree or pin-object read;
- no main edit, no public viewer code, no L4 waiver, no self-approval.

The only computations were read-only, in RAM, on public fixture bytes (§1.3): length, SHA-256, key shape, DBH counts and an informational XZ bit check. They are disclosed as observations, not acceptance evidence. All 65 H0 requirements remain unverified.

**Dashboard/Drive sync: NOT VERIFIED.** #756 stays **OPEN** for Manager #730 technical adoption.
