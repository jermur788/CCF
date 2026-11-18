# Multi #770 A1 — 3D simulation architecture and state/truth dossier

**18 November 2026 · Phase A · DESIGN PROPOSAL / NOT AN IMPLEMENTATION SPECIFICATION**

**A display layer must never become a source of record. No Multi 3D runtime is approved.**

The corrected packet authorises this architecture dossier, not a 3D application, a live simulator connection, a source lane, or a game feature. The recommended next product candidate is one immutable Year0 scene with inspection only. That recommendation is an owner decision input; it grants no implementation rights. [M1] [M2]

### Revision r1 — post-review correction

Amends published A1 commit `c8f430356ec2550424b23a1f34e9fb7f0609e628` after reading the [complete independent Q review at `52b010d754ec12e39998e08a8f53c944e43277d6`](https://github.com/jermur788/CCF/blob/52b010d754ec12e39998e08a8f53c944e43277d6/Docs/Handoffs/MultiRack-Simulation-20261116/770-orchestration/Multi-770-Q-INDEPENDENT-3D-ARCHITECTURE-AND-BOUNDARY-REVIEW-20261118.md) and the complete pinned M4/M5 documents. Corrections are documentary; no fixture or runtime evidence was obtained.

- **Q-04 — ADDRESSED:** §§4.1/4.3/4.4 now name the source `unityX`/`unityZ` f32 envelopes, their forward-transform bit oracle and the absence of established Unity frame/scene semantics; any 3D mapping must cite the transform ID.
- **Q-07 — ADDRESSED:** M2–M5 links now use commit pins, including M5's pre-adoption `7af3add1d23453a92925a404d7dad5899ab0ff7d` text.
- **Q-11 — ADDRESSED:** §§3.4/8.5 now treat H0-A's one-decoder rule as scope-limited precedent and divergence risk; the three static read-arrangement options remain open, alongside the separately scoped live-API option.
- **Q-13 — ADDRESSED:** §13 states explicitly that Phase A contains **NO verified per-tree geometry** and every **W-xx check is NOT RUN**.

No finding is disputed. Other findings are outside this amendment's assigned lane; Q-06's geometry/scale choice remains an owner decision. This revision is not self-approval, Manager acceptance or authority to continue 3D product/spec/source work.

## 1. Handoff identity, authority and evidence vocabulary

| Item | Record |
| --- | --- |
| Lane | A1 — Multi 3D simulation architecture and state/truth boundary |
| Actual worker | OpenAI / `gpt-6.1-sol` / high reasoning effort; T3 Code through the Codex harness, as exposed by the runtime instructions |
| Delegation | Acting as the assigned design worker; no further agents or delegation |
| Repository | `https://github.com/jermur788/CCF.git`; origin inspected before work |
| Checkout | `/home/jer/.t3/worktrees/CCF-Primary/t3-7752aedb-f4a4538a9b68f649` |
| Required and verified starting HEAD | `ff03290a5ef9c53472618aba32591e5822c663e1` |
| Branch | `docs/multi-770-a1-3d-architecture-dossier-20261118-v1` |
| Sole tracked output | `Docs/Handoffs/MultiRack-Simulation-20261116/770-orchestration/Multi-770-A1-3D-SIMULATION-ARCHITECTURE-AND-STATE-TRUTH-DOSSIER-20261118.md` |
| Claim before editing | [A1 RUNNING registration](https://github.com/jermur788/CCF/issues/770#issuecomment-6989053274) |
| Initial status | Clean; no unrelated tracked or untracked changes reported by `git status --short` |
| Context snapshot | Context-set latest change found at `03fade5e4cac5ff6bf0e6c88c8449366b372181e`; repository context inspected at the required starting HEAD |
| Scope of completion | One reviewable design document; no claim of feature acceptance, integration, runtime qualification or owner approval |

The direct task instruction and the corrected Manager #730 adoption are the operative authority for this lane. The obsolete routing/grant is not used. The packet explicitly separates this work from the 2D lane and excludes protected source reads. [M1] [M2]

Live GitHub coordination was consulted at task start. Its older Multi succession narrative is not a fresh physical custody receipt and cannot override the later specific #770 correction. This worker neither changes coordination nor interprets older rows as 3D permission. Physical ownership remains NOT VERIFIED. [M1] [C6]

### 1.1 How to read claims in this dossier

| Label | Meaning here | What it does not establish |
| --- | --- | --- |
| VERIFIED DOCUMENT FACT | The cited text was actually read and says the attributed thing | That its underlying source, measurement or implementation was independently checked here |
| MANAGER / OWNER DECISION | A cited receipt records the named decision within its stated scope | General permission outside that scope |
| DOCUMENT-REPORTED VALUE | A numerical/schema detail comes from an inspected document | A fresh fixture read, byte rehash, source audit or executed test |
| PROPOSAL | This worker's architecture, acceptance or sequencing suggestion | Adopted scope, policy, technology or acceptance |
| UNKNOWN | Required information is not established by the sources inspected for this task | Zero, false, absent, safe or a plausible substitute |
| NOT VERIFIED | A potentially checkable property was deliberately not independently checked | Failure, success, or permission to check it now |

All architecture and future acceptance criteria below are **PROPOSALS** unless a paragraph explicitly attributes an existing decision or reference rule. Every proposed acceptance check is **NOT RUN**. Tables of conditions describe future rejection/acceptance requirements, never results.

### 1.2 Sources and exclusions

The substantive Multi evidence used is the corrected packet, its manager adoption, the H0-A owner approval, the later H0-A interface/security adoption, and the complete corrected interface text. The interface text's historical DRAFT header is read together with its later adoption receipt: it is an adopted **2D reference**, not an adopted 3D interface. [M1] [M2] [M3] [M4] [M5]

Year1 qualification and Year2/marking draft status below are attributed to the corrected packet. This worker did not open the protected Year1 source or independently repeat qualification. Issue numbers mentioned inside that packet are provenance pointers, not additional inspected evidence. [M2]

No fixture, protected P0 tree, protected local numeric continuation, application source, or separate 2D worker output was opened or copied. No unrelated scenario content is a design input. Canonical project context supplied general workflow and separation principles only; it supplies no Multi geometry, ecology coefficients, assets or runtime authority. [M1] [M2] [C1] [C2]

## 2. Scope and the three truth layers

### 2.1 Authority is attached to a state, not to its visual credibility

The corrected packet describes a synthetic pure-Sitka 30 × 30 m experiment with 100 Year0 rows. It describes a separately qualified, locally protected Year1 numeric continuation, and Year2 as DRAFT. It also states that there is no adopted Multi 3D scene/feature specification, physical 3D source lane or validated runtime. These are verified statements of the packet, not independent physical findings by A1. [M2]

| Layer | Existing evidence boundary | Proposed responsibility for any later 3D work | Forbidden promotion |
| --- | --- | --- | --- |
| Authoritative simulated biology and numerics | Pinned experimental inputs/results under their own custody and qualification; actual underlying data NOT VERIFIED here [M2] | Supply explicitly admitted observations, units, availability and lineage | A mesh, renderer calculation or screenshot becoming biological state |
| Approved 2D H0-A reference | Static Genesis Year0 contract `ccf.h0a.core-read` v1.0.0, adopted within H0-A only [M3] [M4] [M5] | Teach semantic preservation and provenance requirements; permit only separately authorised reuse later | Treating its facade as a live 3D API or extending it by convention |
| Proposed 3D display | No adopted feature or runtime [M1] [M2] | Project an admitted snapshot into explicitly schematic geometry and inspection UI | Inventing a time step, ecological process, work result or source fact |

**Proposed invariant:** authoritative state flows toward presentation. Camera/selection changes remain presentation state. There is no reverse biological update path, and no export of an edited scene as a replacement experiment.

The word “authoritative” here is relative to the admitted synthetic experiment and its declared stage. It does not mean measured forest truth, validated Irish forecasting, ecological completeness, game-feature acceptance, or permission to publish source. The H0-A approval expressly excludes forecasting and Unity parity claims. [M2] [M3]

### 2.2 What each layer may own

| Information | Proposed owner | Permitted display transformation | Must remain unchanged |
| --- | --- | --- | --- |
| Tree identity and observation identity | Admitted numeric/read source | Label placement and selected-row presentation | Case, identity binding, lineage and year/stage |
| Coordinates, source dimensions and numeric bits | Admitted source plus separately approved conversion boundary | Documented coordinate basis conversion; text rounding | Raw values, units, bit identity and source semantics |
| Diagnostic availability | Source/read contract | Human-readable explanation with the same meaning | Unevaluated/unavailable cannot become numeric zero |
| Crown/stem mesh | Display asset policy | Schematic shape generated from authorised dimensions | Must not redefine the source height or radius |
| Camera pose, zoom and highlight | Display session memory | Direct interaction | Must not be written into experiment state |
| Proposed performance reductions | Display implementation | Hide costly ornament or use simpler meshes | Every source tree remains inspectable by stable ID |
| Biological change, hazard classification, mortality or regeneration | Separately authorised numeric system | None in the proposed first slice | No display-authored state or transitions |

Proposed geometry should retain a visibly distinct status from numeric truth. A precise-looking cone is still a cone; its silhouette is not evidence of a measured crown. Use a short persistent “schematic geometry” disclosure and detailed provenance on inspection.

### 2.3 Keep unsupported conditions separate from implementation failures

| Condition | Proposed viewer response | What it must not imply |
| --- | --- | --- |
| Valid Year0 observation, diagnostic not evaluated | Show tree and labelled unavailable diagnostic | Diagnostic equals zero or is harmless |
| Required identity/coordinate is invalid | Refuse the scene; show bounded reason | A repaired or partly admitted source |
| Valid geometry, optional ornament unavailable | Show a labelled schematic substitute if approved | Ornament was source-derived |
| Runtime cannot create graphics context | Preserve a clear failure explanation; accessible read view only if specified and lawfully admitted | A 3D render succeeded |
| Protected input lacks lawful read grant | Stop before obtaining it | It is safe to use because it exists locally |
| Product/spec/physical gate is missing | Remain at documentation stage | The architecture dossier authorises coding |
| A future source asks for unsupported year/stage | Refuse that request rather than silently substituting Year0 | The displayed scene corresponds to the requested year |

## 3. Minimal architecture: four responsibilities, one direction

This is a proposed responsibility split, not new managers, services, processes or a public ABI. For a small viewer it can remain ordinary focused code once implementation is authorised. The project workflow favours explicit scope and approved specifications before substantial source work. [C1]

| Responsibility | Proposed inputs | Proposed outputs | Excluded capabilities |
| --- | --- | --- | --- |
| Admission and numeric read boundary | One lawfully supplied, fixed-identity observation package | Entire immutable observation or bounded refusal | Discovering arbitrary files, alternate production trust roots, annual calls |
| 3D projection adapter | Admitted observation plus an approved display mapping | Derived view records keyed to source tree IDs | Numeric/ecological recomputation, guessed fields, changes to 2D contract |
| Scene and accessible inspector | View records and original read values | Schematic scene, labels, complete text/list inspection | Forest edits, eligibility decisions, data export/save |
| Session interaction | Camera input and ID selection | Memory-only camera/selection updates carrying observation identity | Source commands, marks, simulation steps, persistence |

Proposed data flow: **fixed authorised package → admission → immutable observation → display projection → scene and text inspection**. Picking/list input returns only a selected ID bound to that observation; it does not return a biology command.

### 3.1 Admission must finish before scene construction

The 2D reference admits pinned bytes before projection and exposes no partial observation on refusal. It also separates a test-only alternate-pin seam from the production trust root. Those are inspected reference rules, not executed properties here. [M4] [M5]

For a later 3D specification, propose the same fail-closed principle: no provisional forest while integrity/schema/roster checks remain unfinished; no “best effort” tree repair; no silent alternative dataset when admission fails. A refusal should not leave stale geometry under the failed source's title.

A digest can establish equality to a reviewed expected artifact. It cannot prove the trustworthiness of a malicious package that replaces both bytes and embedded expected digest. The independent expected identity and code/package review therefore belong outside the mutable display package, consistent with the H0-A receipt's explicit integrity-root warning. [M3] [M4]

### 3.2 A presentation adapter must not become a second simulator

Proposed adapter work is limited to unit/basis mapping, stable ID binding, display instance records, optional display rounding, and carrying source availability. It must not calculate competition, growth, hazard, regeneration, mortality, thinning suitability or stand aggregates.

This avoids a common trap: a 3D renderer can calculate distances between stems, but that does not grant authority to produce a competition index. A visual shadow is likewise not an authorised canopy-light computation. Their numerical availability must come from an approved source, or stay unavailable.

For a changed 3D mapping, use a separately versioned mapping identity. A mapping correction must not silently alter source identity or imply the underlying experiment changed. A future mapping version would need its own compatibility and review criteria.

### 3.3 Selection and observation identity

The 2D reference binds selection to a full observation key and exact case-sensitive ID; camera and viewport state are outside that key. A tree ID alone is insufficient to identify which experiment/frame is being inspected. [M5]

Propose the same semantic fence for 3D without copying the 2D API surface: selection carries the admitted observation identity and stable tree ID; the inspector rejects a selection from another frame or source. A renderer instance number, array offset, engine object ID or mesh name never substitutes for source identity.

This matters even for a single-frame prototype: asynchronous rendering, delayed pointer callbacks and resource recreation can produce stale selections. Keeping the binding explicit makes a later year feature possible without building it now.

### 3.4 Comparison with the approved 2D reference

| Reference property actually inspected | Proposed 3D consequence | Boundary |
| --- | --- | --- |
| Genesis-only immutable observation [M4] [M5] | Begin with one immutable Year0 scene | No Year1 toggle inherited |
| H0-A has one Core numeric codec and no second decoder in its Viewer [M4] [M5] | Choose the separately authorised 3D read arrangement explicitly from §8.5 | Scope-limited H0-A precedent; decoder divergence is a risk to qualify, not a 3D prohibition |
| Typed availability, full observation key [M5] | Preserve meaning and selection binding | Do not rename absent values to zero |
| 2D renderer draws from integer local coordinates, showing centred bits separately [M5] | Make 3D source/transform choice explicit in its own spec | 2D drawing arithmetic is not automatically the 3D placement rule |
| No annual/mark/save functions [M4] [M5] | Keep inspection capability separate from commands | No hidden “later” action surfaces |
| Local-only policy has actor-specific limits [M4] | Obtain fresh 3D grants and containment evidence | 2D permissions do not transfer |

No separate 2D orchestration files were inspected, duplicated or edited for this comparison. This table uses only the already adopted reference documentation. [M4] [M5]

## 4. Exact Year0 scene: identity, geometry and provenance requirements

“Exact 100-tree world” must mean an exact source-to-view mapping with stated display limitations. It cannot mean guessing a plausible plantation. The corrected packet establishes the intended 100-row Year0 experiment; the corrected reference supplies documented identity and conversion rules. Neither is a fresh byte audit by this worker. [M2] [M5]

### 4.1 What is known from documents, and what remains unverified

| Item | Inspected documentary statement | A1 evidence status | Required before later 3D admission |
| --- | --- | --- | --- |
| Experiment class | Synthetic derived 30 × 30 m pure-Sitka Year0 [M2] [M3] | VERIFIED DOCUMENT FACT | Exact approved source package and scoped read permission |
| Row/ID roster | 100 source-order IDs `P30-001` through `P30-100` [M5] | DOCUMENT-REPORTED; no rows opened | Independently check complete set/order/uniqueness against admitted bytes |
| Source status | Literal `live`, projected as `Live`, not health [M4] [M5] | DOCUMENT-REPORTED | Check exact source token and mapping |
| Source location fields | Integer `xMm`, `yMm`; `yMm` is horizontal local north, not height [M5] | DOCUMENT-REPORTED | Check units, coordinate range and basis for every row |
| Source forward-coordinate fields | The documented 12-key row also includes `unityX`, `unityZ` as f32 envelopes; forward-transform bit comparison uses them as the `TransformBitMismatch` oracle (§4.3) [M5] | DOCUMENT-REPORTED; not checked against bytes here | Validate against `p1-square30-corner-to-centred-xz-v1`; field names establish no Unity frame, handedness, up-axis, pivot or Unity-scene meaning |
| Reference domain | Side 30000 mm; square corners documented; elevation 0 [M5] | DOCUMENT-REPORTED | Verify exact admitted geometry metadata; do not infer geography |
| Numeric representations | Raw integer dimensions and binary32 envelopes for converted values [M5] | DOCUMENT-REPORTED | Check envelope type, bit pattern, finiteness and allowed conversion |
| Crown dimension | Source exposes `crownRadiusM`, explicitly radius [M5] | DOCUMENT-REPORTED | Preserve value/bits and identify the simplifying crown model |
| Source byte identity | Reference documents a 32,567-byte input and fixed SHA-256 [M4] [M5] | NOT REHASHED | Lawful independent comparison to reviewed expected identity |
| Generator versus simulation seed | Distinct documented roles; values recorded in §4.4 [M5] | DOCUMENT-REPORTED | Bind both to the exact source version; never substitute one for the other |
| Individual coordinates/heights/crowns | Not reproduced in this dossier | UNKNOWN to this worker | Obtain only under later lawful source admission |
| Per-tree mesh family, crown base, taper and branching | Not supplied by the inspected read-row definition [M5] | UNKNOWN | Explicit schematic policy or separately evidenced content |
| Cell identity or cell-to-tree map | No per-row cell ID in inspected `ReadTreeV1` [M5] | UNKNOWN | Do not invent a grid; obtain a separately approved map if required |
| 3D source package and API | No adopted Multi 3D specification/runtime in the corrected authority [M1] [M2] | NOT AVAILABLE AS AUTHORITY | Formal owner-approved specification and fresh grants |

### 4.2 Per-tree provenance ledger proposed for a later specification

This is a requirements inventory, not a proposed executable schema or a source-data copy. It separates fields that identify a tree from fields that merely describe its display.

| Required information | Why a 3D scene needs it | Derivation/validation requirement | Refusal or label when missing |
| --- | --- | --- | --- |
| Exact stable tree ID | Maintain selection and numeric correspondence | Match complete admitted source roster, case and uniqueness | Refuse scene admission |
| Source ordinal / serial | Trace a row without making row order identity | Preserve source index as provenance only | Never renumber tree IDs from renderer order |
| Human-readable label | Let the user identify the same tree in scene/list | Derive from stable ID; any alias has an explicit mapping | Use source ID if no separate label is supplied |
| Observation identity | Prevent selection crossing source/year/frame boundaries | Bind tree to full immutable admitted frame identity | Refuse ambiguous frame association |
| Cell ID, if applicable | Distinguish tree identity from spatial/ecological cell identity | Use a sourced, approved cell map and boundary rule | Say “not supplied”; no inferred cells |
| Raw horizontal coordinates and units | Place the same stem as the numeric experiment | Keep integer millimetres if that is the admitted representation | Refuse missing/invalid coordinates |
| Display x/z and transform identity | Explain basis conversion to engine coordinates | Use the exact approved forward rule and precision policy | Refuse unrecognised transform |
| Ground elevation | Avoid inventing terrain conditions | Use sourced constant/surface; distinguish display base from geography | Flat schematic only if approved and disclosed |
| Height and source bits | Represent sourced vertical magnitude | Preserve source value; scale proxy using an explicit mesh axis/pivot | No invented per-tree height |
| DBH and unit | Inspect stem size truthfully | Keep source diameter at breast height distinct from arbitrary full-stem width | Do not imply a measured taper profile |
| Crown radius and provenance | Prevent doubling radius or treating potential crown as measured canopy | Record radius, source rule identity, numeric bits and schematic mapping | No fake crown geometry if required dimension absent |
| Life status and availability | Avoid making healthy-looking art a numeric diagnosis | Copy semantic status, never infer it from geometry | Neutral display with explicit status |
| Asset identity and licence record | Reproduce the representation and lawful use | Bind primitive/mesh/material version, rights and deterministic variation rule | Refuse unlicensed assets; schematic alternative only if approved |
| Source byte/package identity | Tie values to exact admitted input | Reviewed external expected identity plus package manifest | Refuse changed or ambiguous bytes |

### 4.3 Coordinate and extent derivation: requirements, not invented positions

The inspected 2D reference names transform `p1-square30-corner-to-centred-xz-v1`: centred X is `fround((xMm − 15000) / 1000.0)`; centred Z is `fround((yMm − 15000) / 1000.0)`, with binary64 division followed by one binary32 rounding. The source row's `unityX` and `unityZ` are f32 envelopes (`k: f32`, eight-hex-digit `b` bit pattern). M5 reports that all 200 envelopes agree bit-for-bit with those forward results; the adopted contract compares forward-result bits against these envelopes as the `TransformBitMismatch` oracle and projects the verified values as `centredX`/`centredZ`. It does not require reverse float-to-mm equality. These are DOCUMENT-REPORTED statements, not A1 byte checks. It defines local east/north axes as synthetic, not geographic. [M5]

The field names `unityX` and `unityZ` do **not** establish a Unity coordinate frame, handedness, up-axis, pivot or Unity-scene meaning. Any 3D mapping must cite transform ID `p1-square30-corner-to-centred-xz-v1`, not infer a mapping from the field names; a separately approved mapping must identify its relationship to that source transform. No Unity parity or unrelated-scene correspondence follows. [M3] [M5]

Propose that a 3D specification choose one of these explicit mappings rather than letting an engine choose by convention:

| Mapping option | Consequence | Evidence needed |
| --- | --- | --- |
| Use the documented centred binary32 X/Z as the 3D stem base | Preserves the reference's forward-converted positions; easiest to audit against those bits | Authorised source and codec, explicit `p1-square30-corner-to-centred-xz-v1` citation, forward-bit comparison to `unityX`/`unityZ`, and separate axis/pivot proof |
| Use source integer millimetres converted by a separately approved 3D policy | Makes integer spatial provenance primary; may differ at renderer precision boundaries | New mapping identity, rounding policy and expected-value criteria |
| Geographically place, jitter or terrain-fit stems | Changes experiment meaning or adds unsupported information | Separate product/source/spec approval; excluded from the recommended first slice |

A later specification must state renderer handedness, which axis is up, metre scale, plane normal, positive east/north directions, mesh pivot and camera orientation. Source `yMm` must never accidentally become vertical height. Different engine conventions are a mapping problem, not permission to rotate biological coordinates arbitrarily.

The reference range for stem integer coordinates is `[0,30000)` on each horizontal axis, while the polygon includes its outer boundary corners. That distinction prevents the right/top stem boundary check from being confused with polygon closure. It is a documented 2D rule, not a new claim about individual tree positions. [M5]

Proposed checks must distinguish **stem-domain membership** from **crown-envelope extent**. A crown may project beyond the stem-domain polygon; clipping or rescaling it to make a tidy square would alter the representation. The future spec must choose disclosure/clipping for display without rewriting crown data.

### 4.4 Source, seed and byte identity

The corrected reference records generator seed `6390001` and simulation seed `20261116`. It records the trusted input SHA-256 as `bcaaa5c3d9b3d36ad8319ffcc361a6225a02b90281cef8435de7c52f8ff757c4`, with byte length 32,567. These are **document-reported pins only**: A1 did not read, generate, import or hash the fixture. [M5]

Proposed later admission must preserve three different identities:

1. **Input identity:** exact admitted bytes, length, reviewed expected digest, source class and provenance.
2. **Observation identity:** model/RNG/algorithm version, seeds, geometry transform, year/stage and committed observation identity supplied by the approved read boundary.
3. **Display identity:** mapping version, asset/material choices, runtime/toolchain, renderer settings and emitted package identity.

For coordinate provenance, input identity covers the source integers and `unityX`/`unityZ` envelopes; observation identity records transform `p1-square30-corner-to-centred-xz-v1` and its forward-bit validation; display identity records the separately approved 3D basis/axis/pivot mapping. The field names cannot substitute for those identities or establish engine coordinates. None of these per-tree checks was performed in Phase A. [M5]

The generator seed does not prove which bytes were used. A simulation seed does not prove a result if algorithms, call order or model versions differ. A screenshot proves neither input identity nor numeric reproducibility. Keep all three identities visible in a future evidence receipt.

Do not copy the source's self-declared content hash as a trust root. The inspected reference explicitly excludes that field from trusted identity because its canonicalisation is undocumented. [M5]

### 4.5 Spacing and density: what not to infer

One hundred trees inside a 30 m square do **not** establish a 10 × 10 grid, uniform 3 m spacing, a surveyed plantation, or a particular nearest-neighbour distance. No individual coordinate layout was inspected by A1. Exact spacing, edge offsets, coincident stem positions and distribution are **UNKNOWN** here. [M2] [M5]

Proposed spatial validation should operate on authorised exact coordinates, not on an attractive grid. If a design later needs spacing statistics, approve their derivation, units, edge treatment and availability separately. The renderer must not move stems apart to improve visibility or to satisfy a planting-density expectation.

The 2D observation deliberately marks stems/ha and other stand aggregates unavailable. Do not silently fill that field by dividing inventory by nominal area in a 3D adapter. Area and count can support an explanatory dimensional calculation only if the owner separately approves its status as a derived, display-only quantity; it would not become a sourced aggregate or inherit H0-A approval. [M5]

### 4.6 Height, crown and stem fidelity

The reference calls the stage `MatureEquivalent` and supplies crown radius from a named potential-crown rule; it does not define an individually measured branching mesh or a field-verified canopy. Its excluded systems include canopy light. [M5]

Propose simple schematic stem and crown envelopes for the first slice. Height may constrain a vertical proxy extent; radius may constrain a horizontal envelope. Neither supplies crown-base height, crown asymmetry, branch architecture, bark condition or needle density. Those remain UNKNOWN unless separately sourced.

A DBH value is not a complete trunk profile. If a schematic cylinder uses that diameter along its length, disclose the simplification; do not imply measured taper or timber volume. If a crown proxy needs a vertical extent not in the source, use an explicitly approved schematic rule or omit that volume and show a labelled radius envelope instead.

Propose a per-tree inspector that separates “source height/radius/DBH” from “display geometry policy”. A user should be able to tell which dimensions are sourced and which visual choices are schematic without reading developer implementation notes.

### 4.7 Proposed exact-world acceptance checks — all NOT RUN

| Check | Future evidence required | Expected observable result | Fail condition |
| --- | --- | --- | --- |
| W-01 Trusted package | Independent expected-identity record and exact lawful byte comparison | Only the approved package is admitted | Changed/ambiguous bytes accepted or alternate production pin |
| W-02 Roster bijection | Compare admitted roster with display-instance ID map | Exactly one logical tree view for each of 100 source IDs; every ID inspectable | Missing, duplicate, substituted or renderer-only ID |
| W-03 Identity under reordering | Reorder display traversal without changing source observations in an authorised harness | Selection/labels remain bound to the same IDs | Array offset used as identity |
| W-04 Forward spatial mapping | Per-ID raw coordinates, expected converted bits, view bases and mapping identity | All bases correspond under the approved transform | Mirrored axis, wrong units, jitter, unreviewed centring or precision |
| W-05 Extent and boundary | Exact polygon/axis record and explicit half-open stem-domain check | Scene extent and stem membership match the admitted geometry | Polygon closure mistaken for inclusive stem range |
| W-06 Height/radius | Compare sourced dimensions and display policy against per-ID representation | Source inspector is exact; proxies follow disclosed mapping | Radius used as diameter; mesh scale/pivot changes source magnitude |
| W-07 Seed/provenance | Inspect source, observation and display identity records | Each seed/version has its correct role | Seed alone presented as byte identity |
| W-08 No inferred cells/grid | Inspect projection and UI against source schema | No cell or uniform-spacing claims without approved source | A guessed cell ID or plantation grid presented as fact |
| W-09 All-ID access | Select through complete text/list route and scene route | Same observation-bound inspector for every ID | Occluded or low-detail trees become uninspectable |
| W-10 Admission refusal | Use separately authorised negative artifacts and production-integrity checks | No partial forest or “repaired” source on invalid admission | Partial rows or stale forest under failed-source title |

## 5. Stage and year visibility semantics

Within the synthetic experiment, the pinned Year0 input is the authoritative starting baseline; any 3D representation remains a derivative display. [M2] [M5]

Year is part of source meaning, not merely a timeline label. The corrected packet distinguishes Year0, a qualified local Year1 continuation, and draft Year2; the adopted H0-A interface itself is Genesis-only. [M2] [M4] [M5]

### 5.1 Proposed visibility policy

| Stage/year | Status supported by inspected documentation | Proposed first-slice treatment | If considered in a later scope |
| --- | --- | --- | --- |
| Year0 / Genesis | Approved static 2D reference for the synthetic 100-tree experiment [M3] [M4] [M5] | Sole admitted scene; labelled “Synthetic experiment — Year0 / Genesis” | Retain exact input and observation identity |
| Year1 | Packet reports independently qualified, locally protected numeric continuation [M2] | No toggle, loading, animation or access | Separate lawful package/grant, lineage evidence and visible “Historical Year1 — locally qualified numeric continuation; 3D display not yet qualified” until its own gates pass |
| Year2+ | Packet identifies Year2 addendum as DRAFT [M2] | No scene, interpolated future or prediction control | Remain excluded until separately approved, implemented and qualified; DRAFT is not a renderable authority |
| Marking/work | Packet identifies marking as DRAFT [M2] | No marks, treatment preview or work controls | Separate approved product/command specification and rights |

“Historical Year1” refers to a prior simulated observation in the experiment's lineage, not field history. Local numerical qualification does not establish 3D correspondence, general runtime correctness, release safety or an ecological forecast. A1 has not inspected the numeric continuation or repeated its qualification. [M2]

### 5.2 No silent time semantics

Propose that the first scene have no timeline slider, “next year” button, annual clock, automatic growth animation or disabled-looking work action. Even an apparently harmless slider suggests that time observations exist and are supported.

If a later owner-approved historical comparison includes Year1, each scene must carry its own admitted frame identity and status label. Never append “Year1” to Year0 geometry while waiting for data. A missing Year1 package must produce an explicit unavailable/refused state, not a fallback forest.

Interpolate camera transitions freely as display state if approved. Do not interpolate DBH, height, crown, mortality or regeneration between years and imply those intermediate states are simulated observations. Such interpolation would be a separately labelled illustrative feature requiring its own scope decision.

### 5.3 Stage is not interchangeable with year

The reference separates `sourceStage: Genesis` from ecological year 0 and uses explicit unavailable/not-evaluated transition/history fields. Its observation key includes stage, year and revision-related identity. [M5]

Propose that future 3D observations preserve these distinctions. A row from a pre-step state and a row from a post-step state must not be joined merely because their year integers look compatible. If the source does not supply transition, audit or revision identity, show that limitation rather than inventing it.

### 5.4 Proposed future year checks — NOT RUN

| Check | Required future behaviour | Misleading behaviour to reject |
| --- | --- | --- |
| Y-01 First slice | Only exact Genesis Year0 is admitted | Accepting a request for another year and displaying Year0 |
| Y-02 Historical label | If later authorised, Year1 visibly retains local/historical qualification and its own frame key | “Current verified forest” without its limited context |
| Y-03 Draft exclusion | Draft Year2 never appears as an authoritative forest or forecast | Speculative future shown with the Year0 authority banner |
| Y-04 No state blending | Numeric values and source identity change atomically between separately admitted frames | Year1 inspector with Year0 mesh values |
| Y-05 Stale selection | A selection carrying another observation identity is rejected | Same tree ID opening the wrong year's state |
| Y-06 No annual capability | First-slice production surface has no annual/mark/work command route | Hidden handlers reachable through shortcuts or console-exposed application commands |

## 6. Ecological and diagnostic distinctions a 3D view must preserve

This section specifies truthful communication, not ecological formulas. It introduces no coefficients, thresholds, growth rules, quotas or hazard model. Source/reference facts are cited; response policies are proposals.

### 6.1 Numeric availability is part of the value

The adopted 2D reference distinguishes `PresentSourced`, `PresentComputed`, `NotEvaluated` and `Unavailable`. It carries reasons and calculation/source context and explicitly disallows numeric zero as a substitute for an unknown value. Genesis diagnostics include unevaluated Hegyi CI, neighbour count, mortality pressure, hazard probability, vulnerability and RNG roll numerator; health score is unavailable. [M4] [M5]

| Source meaning | Proposed label/representation | Viewer may do | Viewer must refuse to imply |
| --- | --- | --- | --- |
| PresentSourced | Value, unit and source context | Display rounding while preserving raw inspector provenance | Rounding changes source bits or biological meaning |
| PresentComputed | Value plus named source calculation stage | Attribute to the authoritative read boundary | Viewer computed it because it is convenient |
| NotEvaluated | “Not evaluated at this stage” plus reason | Explain why no number exists | Zero, low, safe, not applicable, or estimated |
| Unavailable | “Not supplied by this source” plus reason/context | Distinguish lack of recording from lack of process | No ecological phenomenon exists |
| Unsupported feature | “Not supported in this scope” | Explain scope limit | The feature was evaluated and found absent |
| Blocked input/authority | “Cannot display: required authority/input missing” | Stop safely | Source is malformed or feature has been approved |
| Admission failure | Bounded refusal and retained disclosure | Explain incompatible/untrusted observation | A partially trustworthy forest |

Do not collapse these into a single “N/A” badge. A source-recording limitation, an unevaluated diagnostic and missing permission require different user explanations and different future actions.

### 6.2 CI: a geometric scene is not a competition evaluation

In the inspected reference, CI is explicitly `hegyiCi`, unevaluated at static Genesis; neighbour count is also unevaluated. The name does not refer to a confidence interval in this schema. A different future source would need its own explicit meaning. [M5]

Propose showing “Hegyi CI — not evaluated at static Genesis” in inspection if that field is exposed. Do not calculate a new value from the rendered positions, crown overlaps or nearest visible neighbours. Culling, proxy geometry and occlusion are display concerns, not numeric neighbourhood membership.

Dense-looking crowns must not become a hidden competition colour scale. A sparse-looking view must not imply low competition. If source CI is later admitted, its model/stage/units or dimensionless interpretation and availability must accompany it; the display cannot invent a rating threshold.

### 6.3 Hazard probability, hazard rating and safety claims

The reference exposes unevaluated hazard probability, vulnerability and mortality-pressure diagnostic fields at Genesis, and no sourced health score. It does not supply an adopted 3D hazard-rating scheme. [M5] [M2]

Propose no hazard heatmap in the first slice. A future numeric probability would not automatically justify converting it into “low/medium/high” or “safe/unsafe”. Those labels need an approved mapping, interpretation, scope and evidence of the underlying model. A colour gradient would need equally explicit rules.

Wind animation, leaning meshes, broken branches and storm effects would suggest causal hazard information. Exclude them from the recommended schematic slice. A presentation failure must never be explained as simulated tree failure.

No tree-selection affordance should say “safe to fell”, “eligible”, “healthy” or recommend management. Inspection is evidence access, not a forestry prescription or a work-command boundary.

### 6.4 Regeneration, recruitment and canopy light

The corrected reference lists regeneration, recruitment, juvenile lifecycle and canopy light among excluded systems for this static experimental input. This is a documented scope boundary, not a claim that those processes are absent in real forests. [M5]

Propose a neutral ground plane or explicitly decorative substrate without saplings, seedling clusters, regeneration patches or understory growth cues that might be read as sourced. If future decorative plants are chosen, their decorative status must be unmistakable and their ecological implications reviewed separately.

An empty patch in the scene must not be labelled “no regeneration” when the source did not model regeneration. A shadow gap must not become an establishment opportunity. A visual crown envelope is not a measured light field, habitat score or productivity surface.

This preserves the project principle that art communicates authoritative state rather than creating unsupported ecological state. No botanical inference is supplied by this dossier. [C2]

### 6.5 Mortality, life status and absence

The 2D reference maps source `status="live"` to display `Live` and explicitly says this is not health. It does not supply mortality history, death audit or saved-event provenance for Genesis. [M4] [M5]

| Situation | Proposed permitted communication | Refused inference |
| --- | --- | --- |
| Source says Live | “Life status: Live (source token)” | Healthy, thriving, low risk, indefinitely surviving |
| Tree is occluded or culled | Keep it in the complete ID list; explain visibility if selected | Dead or removed |
| Tree has no display asset | Label rendering failure or schematic representation | Missing source tree or ecological mortality |
| A future frame lacks a prior ID | Require an explicit authorised transition/roster policy | Automatically call it felled, dead or harvested |
| Source supplies death with cause later | Display only admitted status/cause/year with provenance | Infer cause from a fallen mesh |
| Source supplies no mortality audit | Show unavailable audit | Zero mortality or a successful survival test |

Do not add stumps, deadwood or fallen stems to make the scene look realistic unless the source state and separate asset/spec authority justify them. Visual realism must not manufacture a mortality event or resource award.

### 6.6 Action annotations and marking

The adopted 2D contract includes a strict `dbhMm < 170` annotation, explicitly not an eligibility rule, with no annual/mark/save functions. The corrected packet keeps marking DRAFT. [M4] [M5] [M2]

Propose omitting this annotation from the first 3D slice unless the owner wants parity of read-only inspection content. If included, carry the source-provided annotation meaning and exact wording; do not turn it into coloured candidate trees, recommendations, a quota or a selectable Fell action.

There is no “apply changes” control in this proposal. Selection highlights must look like inspection focus, not permanent tree marks. The difference should survive a screenshot and should not depend on colour alone.

### 6.7 Proposed semantic honesty checks — NOT RUN

| Check | Future stimulus/evidence | Expected response |
| --- | --- | --- |
| S-01 Availability preservation | Authorised read-schema doubles covering each availability state | Distinct text/reason, no numeric placeholder for missing value |
| S-02 CI preservation | Unevaluated CI with dense-looking geometry | No computed CI, competition rating or inferred neighbour count |
| S-03 Hazard preservation | Unevaluated hazard fields | No safety colour, “low risk” label or derived probability |
| S-04 Regeneration scope | Source excludes regeneration/juveniles | Explicit scope statement; no sourced-looking recruits |
| S-05 Live versus health | Source Live plus unavailable health | Live label only; no health score/colour |
| S-06 Visibility versus death | Hide an instance through display culling in a lawful harness | ID remains inspectable; no mortality event or count change |
| S-07 Annotation versus action | Read-only below-threshold annotation if included | Annotation cannot trigger marking, eligibility or work |
| S-08 Unsupported/blocked/refused | Exercise separately specified cases | Different bounded explanation for each, no partial scene |

## 7. Camera, selection and accessibility boundaries

This section supplies A1 architectural constraints for the separately authored interaction dossier. It does not choose final controls, key bindings, camera angles, visual style or accessibility acceptance on that lane's behalf.

### 7.1 Recommended interaction shape for owner consideration

Propose one scene with a fit-to-stand overview, bounded orbit/pan/zoom, reset view, and inspect-only selection. A top-down or orthographic inspection preset can clarify coordinates without introducing a second source or a second forest. Free walking, avatar collision and physical interaction are outside the smallest slice.

The choice of camera is a product decision. An oblique view may show height more clearly but conceal stems; an orthographic view supports spatial comparison but reduces physical depth cues. Neither should alter coordinate or status truth. Selecting an occluded tree through the list may frame/highlight it, but must not move that tree.

For coincident or visually overlapping stems, propose a deterministic ID chooser or list route. Do not nudge stems apart. Picking meshes may use a larger accessibility target, but they remain labelled selection proxies and cannot become a measure of tree size or occupancy.

### 7.2 Identity under camera and rendering changes

Propose that a selected tree's inspector remain bound to the same observation/ID through camera movement, resizing, simplification and graphics-resource recreation. If the frame is refused or replaced later, clear/revalidate selection explicitly.

The inspector should distinguish source ID, raw dimensions/status, diagnostic availability, source/year disclosure and schematic geometry policy. The complete ID list must remain available regardless of distance, canopy overlap or detail level.

### 7.3 Accessible information is a parallel presentation of the same observation

W3C's WCAG quick reference covers text alternatives, keyboard access, colour-independent communication, visible focus and ways to disable interaction-triggered motion. It provides relevant web criteria; it does not certify any proposed viewer. [T4]

Propose keyboard-operable inspection, visible focus, readable scalable text, a full list and no colour-only biological status. Provide a reduced-motion choice and avoid requiring dragging, precision pointing, pointer lock or depth perception to inspect an ID. Native runtime candidates must demonstrate equivalent usable access; a rendered text label alone is insufficient evidence of assistive-technology support.

These are design requirements to settle with A2 and the owner, not a declaration of WCAG conformance or native screen-reader compatibility. Exact devices, browser/runtime versions and acceptance procedures remain to be approved and then checked.

## 8. Runtime options — grounded facts and conditional trade-offs

**Technology choice belongs to the owner.** No option is selected, installed, executed or qualified by this document. The corrected packet explicitly holds runtime, product scope, fidelity, sharing and physical lane choices open. [M2]

The comparison separates limited verified platform documentation from design judgement. No benchmark, package size, frame rate, memory use, licence clearance or installed-runtime inventory is claimed.

### 8.1 Option B: in-browser 3D using WebGL

**Grounded platform facts:** the Khronos WebGL specification defines a Canvas rendering context for 3D drawing, allows context creation failure, specifies context-loss behaviour and includes resource/security restrictions. The versioned specification is a platform reference, not evidence that a particular browser/driver or local package works. [T1]

**Proposed implementation shape, if chosen later:** a separately approved local package with all required code/assets embedded or packaged under an explicitly accepted offline loading policy. No server, loopback, CDN, remote font, dynamic dependency fetch, telemetry, upload or forest persistence. A browser graphics context does not by itself enforce those application restrictions.

| Dimension | Conditional assessment / proposed control | Unresolved evidence |
| --- | --- | --- |
| Containment | Keep runtime input fixed and package resources explicit; independently inspect executable bytes and demonstrate no network/storage capability use within the approved scope | Effective writer and browser-profile boundaries; extensions/background activity attribution; exact CSP/package policy |
| Determinism | Compare admitted numeric/view records, not GPU pixels; retain exact bits before display rounding | Exact browser/driver/graphics path; repeated package and mapping evidence |
| Assets/licensing | Schematic primitives reduce media intake; a graphics library would still need owner package/dependency approval and notices | Whether to use direct WebGL or an approved library; exact version and licence inventory |
| Accessibility | A same-observation text/list route can be designed alongside 3D | Actual keyboard and assistive-technology behaviour on the selected browser matrix |
| Performance envelope | Start with all 100 logical tree views and simple proxies; cap visual complexity only through owner-approved budgets | Actual target hardware, startup/interaction/frame/memory measurements; context failure/loss behaviour |
| Maintenance | Small runtime scope is plausible; custom graphics/picking work may increase maintenance compared with an engine | Maintainer capability and chosen library/tooling costs |
| Observability | Separate admission/mapping evidence, ID mapping and rendered inspection evidence | Lawfully scoped network/storage observation and exact final-package review |
| Later authority | A new 3D spec, local source/publication policy, grants and physical lane | H0-A's browser/security approval is not transferable [M4] |

**Assessment, not decision:** this is a credible candidate when the owner prioritises local inspect-only delivery and a text/list companion. It is not established as fastest, safest, smallest or compatible. Do not turn an offline-loading obstacle into a server exception without a new decision.

### 8.2 Option U: a separate Unity prototype

**Grounded platform facts:** Unity publishes distinct platform development guidance, including Web delivery, and warns that hardware, input and deployment differences can require changes between targets. The repository overview records Unity 6000.6.0f1, URP and New Input System project context; those records are not a new local Editor inspection. [T2] [T3] [C3]

**Proposed implementation shape, if chosen later:** an isolated Multi prototype with its own approved source/asset boundary, immutable observation ingestion and no scene-to-biology write-back. The owner must settle whether “separate” means a separate project or another precisely bounded arrangement. Do not adopt an existing unrelated game scene, save system or asset family as an implicit starting point.

| Dimension | Conditional assessment / proposed control | Unresolved evidence |
| --- | --- | --- |
| Containment | Treat native application code as potentially broad filesystem/network capability; require effective controls and scoped observation | Actual process/worktree/asset grants, runtime side effects and applicable security policy |
| Determinism | Keep source and display mapping immutable; physics/frame timing cannot author biology | Editor/player settings, mapping precision, platform/GPU and content manifest |
| Assets/licensing | Primitives first; approve each source/prefab/material and right before reuse | No asset rights or reuse grant for Multi 3D established by this dossier |
| Accessibility | Build equivalent inspection without requiring a first-person controller | Exact UI/input/assistive-technology behaviour and owner acceptance |
| Performance envelope | Review simple representative scene before realistic tree art | Hardware-specific CPU/GPU/memory/startup evidence, import footprint and asset complexity |
| Maintenance | Engine scene tools may help authoring; project/import/package/serialized-state work adds custody and review costs | Actual proposed project structure, installed package versions and maintenance owner |
| Observability | Compare read records, imported representation, scene mapping and rendered inspection separately | Actual import/runtime/render qualification under lawful exclusive custody |
| Later authority | Formal Multi 3D spec, fresh physical lane and actor-specific G1/G2; Unity custody/lease if selected | No Editor launch, scene authoring, build or test permitted in Phase A [M1] [M2] |

**Assessment, not decision:** Unity is credible if the owner explicitly wants to study a later native presentation path and accepts its additional project/asset/physical qualification burden. This prototype would prove only its scoped representation, not parity with another Unity project or acceptance as a gameplay feature.

Unity Web export is not automatically Option B. Its packaging, runtime loading and toolchain would require their own compatibility/no-server decision and evidence. No export or hosting permission follows from comparing it here. [T2] [M2]

### 8.3 Option N: another native runtime, exemplified by Godot

**Grounded platform facts:** Godot's official documentation describes command-line operation and headless/export workflows. Its licence guidance identifies the engine's MIT licence and the need to preserve applicable licence/third-party notices. These facts do not establish an installed version, asset rights, accessibility quality or local qualification. [T5] [T6]

**Proposed implementation shape, if chosen later:** a separately approved native inspect-only application, one fixed observation package and schematic geometry. Godot is a concrete comparison example, not a request to install an engine or add a dependency. An alternative native toolkit would require a new exact toolchain comparison rather than inheriting this assessment.

| Dimension | Conditional assessment / proposed control | Unresolved evidence |
| --- | --- | --- |
| Containment | Use explicit OS/process input/output restrictions and independently reviewed capability boundaries | Exact installed toolchain, sandbox effectiveness and runtime side effects |
| Determinism | Compare source numeric bits and display records; disable physics/random ecology in the first slice | Cross-runtime conversion agreement and deterministic content/mapping identity |
| Assets/licensing | Schematic content may limit asset burden; engine and third-party notices remain separate from data rights | Exact engine/export-component versions and every included asset licence |
| Accessibility | Design a complete same-observation inspection route from the outset | Actual native UI and screen-reader support on the chosen environment |
| Performance envelope | Evaluate simple geometry on the owner's real target machine before assuming a benefit | Startup, frame-time, memory, driver/backend and package footprint measurements |
| Maintenance | Separate native toolchain adds learning and reproducibility work | Long-term maintainer capacity, package/upgrade policy and approval |
| Observability | Command-line capability could support later reproducible checks; headless checks cannot prove rendered honesty | Exact lawful tooling, rendered review and independent source/mapping evidence |
| Later authority | Owner technology/dependency choice, formal spec, fresh source/security/physical grants | No installation, export, source edit or runtime check authorised now [M2] |

**Assessment, not decision:** a native alternative is credible if the owner accepts a separate toolchain and has a concrete reason to prefer it. A permissive engine licence does not make protected data reusable or make a native application contained.

### 8.4 Comparative decision matrix

These are qualitative design judgements under the proposed Year0-only slice, not measured scores or a technology ranking.

| Owner priority | Browser/WebGL candidate | Separate Unity candidate | Other native candidate |
| --- | --- | --- | --- |
| Inspect exact source without walking/gameplay | Plausible with schematic scene and accessible text | Plausible, but avoid loading unrelated game systems | Plausible, with separate UI/mapping work |
| Strict local-only, no server, no forest save | Requires explicit offline package and browser capability proof | Requires native containment and strict source boundaries | Requires native containment and exact toolkit review |
| Future visual/game-engine exploration | Supplies a display experiment; no game-engine adoption | Supplies a native engine representation study if owner chooses | Supplies a separate native presentation study |
| Lowest new asset commitment | Primitives possible | Primitives possible | Primitives possible |
| Source numeric reproducibility | Depends on one authorised codec/mapping and identities | Same requirement; engine use does not prove it | Same requirement; alternative engine does not prove it |
| Accessibility evidence | Same-observation text companion must be checked | Equivalent native route must be checked | Equivalent native route must be checked |
| Operational burden to settle | Browser matrix, local resource policy and package integrity | Project/import/assets/runtime custody plus platform evidence | New toolchain/dependency/exports plus native evidence |
| Proven performance here | NOT VERIFIED | NOT VERIFIED | NOT VERIFIED |
| Approved runtime for Multi 3D | None [M2] | None [M2] | None [M2] |

### 8.5 Share, adapt or duplicate the numeric read boundary?

This decision is separate from graphics technology. The current approved contract is restricted to 2D H0-A; a choice to reuse ideas does not grant code/source/package rights. Its one-Core/no-second-Viewer-decoder rule is a scope-limited H0-A precedent, not a prohibition on a separately approved 3D decoder. Duplication introduces divergence and qualification risks; sharing introduces coupling and custody risks. The three static options below are the reference framing, and share-versus-duplicate remains an owner/spec decision. The live option remains a separate scope. [M4] [M5] [M2]

| Option for a later spec | Potential benefit | Material risk/cost | Required authority/evidence |
| --- | --- | --- | --- |
| Reuse a frozen authorised read artifact through a separate 3D adapter | Avoid two numeric definitions; clear read-only boundary | Coupling to existing interface/version and custody; may not fit chosen runtime | Explicit owner/Manager reuse permission, source-rights boundary and version/pin approval |
| Obtain a separately qualified immutable projection at an authorised preparation boundary | Runtime consumes only required display/read fields | Projection can lose bits, reasons or provenance; creates a new artifact to qualify | Approved producer, exact mapping/schema/trust identity and independent comparison |
| Implement another decoder | Runtime independence | Divergent numeric/availability/trust behaviour and duplicated maintenance | Explicit owner/spec choice and meaningful cross-decoder qualification; no silent fork |
| Connect to a live simulator | Could support later dynamic features | Introduces command/state/network/process and security questions far beyond a static view | Separate accepted live API/product/security scope; excluded from the proposed first slice |

**Recommendation for owner consideration:** preserve one authoritative numeric/read definition and keep the 3D adapter narrow under whichever arrangement is chosen. Whether reuse, a separately qualified projection or a separately qualified decoder is lawful and maintainable remains UNKNOWN until actual source rights, runtime and formal spec are settled. This neither selects sharing nor rejects duplication, and is not a decision to consume `CcfYear0Core` from 3D.

## 9. Containment, reproducibility and performance evidence boundaries

### 9.1 “No save” must be precise

Propose no forest persistence, history file, import/export, file picker, upload, clipboard data transfer, browser storage, screenshots-as-product-export or reloadable session. Camera/selection live in memory and reset on close. Evidence captures may be produced only by a later explicitly authorised reviewer workflow, not an application export feature.

Browsers and native engines can have their own caches, logs and preferences. A no-forest-save product requirement is not a proof of zero process filesystem writes. The later permit must name any unavoidable tool/runtime side effects, allowed locations and containment observation; unexpected writes require a stop. No such inspection or exception is performed here.

### 9.2 No network and local-only are separate requirements

Propose that local-only means no external delivery/publication and no runtime data exchange. No-network means no application-originated remote or loopback communications, including telemetry, dependency loading, debug transport and update checks within the approved runtime scope. Neither requirement can be proved solely by removing a visible button.

For browser candidates, package inspection plus runtime observation should distinguish page activity from browser/extension background behaviour. For native candidates, inspect application and tool side effects within exact actor/process boundaries. Observation tooling must itself be authorised; it cannot become a new Host or write surface.

The H0-A security decision explicitly limits its JS-only proof and leaves L4 FAIL38 unwaived. This dossier does not transplant that policy to browser 3D, a native engine or a different source lane. [M4] [M2]

### 9.3 Three reproducibility claims that must remain separate

| Claim | Proposed proof | What it does not prove |
| --- | --- | --- |
| Same admitted observation | Same reviewed source identity, model/stage lineage and numeric bits | Same package or pixels |
| Same display mapping | Same source-to-instance records, transform/asset policy and stable ID mapping | Pixel identity across GPUs or usability |
| Same emitted artifact | Recorded toolchain/input manifest and repeated output identity under the approved process | Safe execution, lawful source access or ecological validity |

A screenshot comparison can support visual honesty but must not be the only numeric oracle. Numeric equality can coexist with an inverted scene, doubled crown radius or inaccessible inspector. Conversely, graphics differences do not by themselves prove numeric differences.

### 9.4 Performance envelope must be decided, then measured

The only tree-count scope assumed here is the documented 100-row Year0 experiment. There is no verified hardware/performance baseline for a 3D application in the inspected authority. [M2]

Propose an owner-approved envelope that records these items before implementation acceptance:

| Envelope item | Value now | Why the later specification needs it |
| --- | --- | --- |
| Target OS/runtime/browser versions | UNKNOWN | Prevent accidental compatibility claims |
| Target CPU/GPU/RAM/display conditions | UNKNOWN | Make measurements interpretable and repeatable |
| Viewport/text scaling and input modes | Owner decision pending | Ensure usable inspection under representative conditions |
| Startup and first-admitted-scene budget | No threshold proposed as approved | Prevent a long wait being hidden by an untrusted placeholder forest |
| Frame-time/interaction-latency budget | No measured value or approved target | Make camera/selection responsiveness assessable |
| Memory, package and asset footprint | UNKNOWN | Avoid unapproved storage/toolchain burden |
| Proxy detail/shadow/post-process policy | Proposed schematic first | Control visual cost without changing source identity |
| Context/device failure behaviour | Not specified/verified | Preserve refusal and accessibility semantics under failure |

Do not promise a frame rate because the scene has only 100 trees. Asset geometry, transparency, shadows, text and device/runtime choices remain unmeasured. Start with a representative simple scene, measure the approved candidate once scoped, and improve only where evidence identifies a problem.

Detail reduction must never change the admitted inventory count or make tree identity unavailable. Any display culling should be auditable separately from source state. Performance acceptance cannot erase a provenance or accessibility failure.

## 10. Recommended smallest credible vertical slice — owner decision input

**PROPOSAL:** one non-public, local-only, immutable Year0 scene of the exact 100-tree synthetic experiment, using schematic geometry and complete inspect-only ID access. Select one runtime after the owner chooses the product purpose. No source work starts from this recommendation. The packet requires later accepted documentation, product choice, formal specification and fresh actor/physical grants. [M2]

This is the smallest credible slice because it can answer whether a spatial representation preserves source identity, coordinates, dimensions and semantic availability while remaining usable. It intentionally leaves annual simulation, historical comparison and realistic forest content for separately justified decisions.

### 10.1 Scope proposed in

| Capability | Proposed limit | Evidence it would seek |
| --- | --- | --- |
| Input | One lawfully supplied, reviewed, immutable Year0 observation/package | Exact source identity and atomic admission |
| World | Exact admitted square/domain and 100 stable tree IDs | Per-ID source-to-scene correspondence |
| Geometry | Schematic sourced-height/radius representation; missing shape dimensions explicitly disclosed | Honest distinction between sourced dimensions and illustrative shape |
| Navigation | Fit/reset and one bounded inspect camera chosen with A2/owner | Understandable stand/height inspection without game systems |
| Selection | ID-bound scene pick plus complete accessible list | Same observation/row through every route |
| Inspector | Sourced ID/coordinates/dimensions/status plus availability/provenance | No health, hazard or ecological invention |
| Failure handling | Bounded admission/runtime refusal without partial forest | Honest unsupported/error states |
| Evidence | Exact candidate manifest, mapping review, rendered user review, accessibility and scoped containment/performance checks | Separate proof for data, view and runtime |

### 10.2 Scope proposed out

| Excluded scope | Why it is outside the smallest slice |
| --- | --- |
| Year1 loading or comparison | Requires separate lawful historical source/package and display qualification |
| Year2+ scenes or interpolation | Draft material cannot become authoritative future state |
| Annual simulation or any biological update | Would cross the read-only truth boundary |
| Marking, Work Plans, Fell, planting, harvesting or resource awards | Introduces command and management semantics absent from a static view |
| Live simulator API or Host bridge | Adds new state/process/security policy and is not approved |
| Network, loopback, server, upload or arbitrary input | Creates unapproved external/input surfaces |
| Forest save/history/export | Creates unapproved persistence and policy scope |
| First-person controller, avatar/collision or gameplay | Not needed to verify the proposed spatial-read question |
| Photorealistic trees, weather, wind, fauna or regeneration art | Adds unverified ecological cues, asset burden and performance uncertainty |
| Geographic terrain, surveyed location or real-world prediction | Not established by a synthetic local experiment |
| Reuse of unrelated scene, assets, source custody or permissions | Separate authority cannot be inferred from project proximity |
| Public source/package delivery, main integration or release | Explicitly outside this Phase A permission [M1] [M2] |

### 10.3 What the slice could prove after lawful implementation and review

It could establish, for one exact candidate and approved environment, that the admitted Year0 observation maps to the intended tree identities/coordinates/dimensions; users can inspect every tree; disclosure and unavailable diagnostics remain truthful; and the scoped runtime meets its agreed performance/accessibility/containment acceptance criteria.

Even those would be **candidate-specific** conclusions. The runtime evidence must identify hardware, software, package identity, source identity and review scope. A screenshot or compile result alone would not satisfy the whole slice.

### 10.4 What the slice would not prove

It would not prove biological validity, predictive accuracy, a live API, Year1 display correctness, Year2 readiness, safe marking, timber/resource accounting, regeneration, mortality transitions, a complete game loop, numeric parity with another runtime, arbitrary device compatibility, global protected-store integrity, L4 compliance, publication rights or release safety.

These are limits of the proposed question, not defects to hide. Keeping them visible prevents a successful spatial read from being promoted into a simulation or product claim.

### 10.5 Mandatory preconditions before anyone implements it

The corrected packet places implementation after Manager acceptance of A1/A2/Q, owner product choices, a formal four-file Multi 3D specification, independent review/owner approval, and fresh physical/actor-specific grants. These are manager-defined gates, not A1's new approval policy. [M2]

| Sequence | Required artifact/decision | Why it matters | Current A1 status |
| --- | --- | --- | --- |
| P-01 | Manager #730 disposition of A1/A2 and independent nonauthor Q | Establish documentation quality and unresolved objections | This dossier is ready to submit; no self-approval |
| P-02 | Owner choice of product purpose, runtime, year scope and fidelity | Gives implementation a settled user outcome | OPEN; no choice inferred |
| P-03 | Separate formal Multi 3D `spec.md`, `plan.md`, `tasks.md`, `checklists/requirements.md` | Defines intent, boundaries, ownership and acceptance | Not authored or approved by A1 |
| P-04 | Independent spec review and owner approval | Resolves material ambiguity before source work | NOT VERIFIED / future gate |
| P-05 | Lawful exact input/reuse/projection and package rights | Prevents protected-source or 2D-rights inheritance | NOT GRANTED by this document |
| P-06 | Fresh distinct physical source lane/worktree/branch and exact allowed file/asset set | Establishes actual single-writer custody | No 3D lane granted here [M2] |
| P-07 | Actor/session-specific G1/G2 security/runtime permission and fresh preflight | Matches real worker/tool access to the actual scope | No implementation permit here |
| P-08 | Unity custody/lease and precise project/import allowance if Unity is chosen | Prevents engine choice from bypassing physical/serialized-asset gates | No Unity work in Phase A |
| P-09 | Exact later evidence plan and independent reviewer | Separates source mapping, rendered honesty and runtime observations | Proposed categories only; NOT RUN |

The formal four-file spec should have its own feature root selected by the Manager; `specs/multi-3d-year0-inspection-v1/` is a **candidate name only**, not an adopted path or a file created by this task. A1's sole file allowance does not permit creating that spec now.

### 10.6 Contents that the later four-file spec should settle

| Artifact | Proposed decision-ready contents |
| --- | --- |
| `spec.md` | Exact product outcome and non-goals; synthetic-source disclosures; truth layers; Year0-only semantics; per-tree identity/dimension meaning; availability; inspect-only UX; containment and acceptance |
| `plan.md` | Owner-selected runtime; authorised source/read arrangement; versioned coordinate/mesh policy; exact asset/provenance boundary; lawful packaging and evidence strategy |
| `tasks.md` | Named bounded actors/file sets; dependency order; fresh physical admission; source and independent review tasks; explicit stop conditions and handoff identities |
| `checklists/requirements.md` | Traceable criteria for exact roster/mapping, truth labels, accessible inspection, no-command/no-network/no-save, rendered review and performance; all initially unverified |

Do not put unsettled owner choices into a plan as if technical convenience settled them. Do not use a checked requirements box to substitute for an evidence record. Specification approval, implementation, qualification, integration and acceptance remain different states. [C1]

## 11. Proposed later evidence sequence — no execution in Phase A

This is a scoped verification design, not a command list to run now. No test, build, runtime, browser, Editor or CI was executed for this dossier.

| Order | Question to answer later | Evidence category | Stop condition |
| --- | --- | --- | --- |
| E-01 Authority | Are actor, source, lane and exact file/asset rights current and disjoint? | Named grants and fresh physical preflight | Missing or conflicting permission/custody |
| E-02 Source admission | Does the exact package match the independently reviewed expected identity and semantic schema? | Lawful byte/source/read-boundary review | Untrusted bytes, incomplete schema or alternate production trust |
| E-03 Mapping | Does every admitted ID have one correct logical view with correct basis/units/dimensions? | Per-ID records and reviewed mapping evidence | Identity mismatch, coordinate invention, radius/axis/scale error |
| E-04 Semantic honesty | Are unavailable/stage/status meanings preserved? | Focused authorised negative/read-schema cases | Invented numbers, health/hazard/regeneration or wrong-year implication |
| E-05 Representation | Does the exact scene communicate source and schematic limits? | Independent rendered review plus numeric comparison | Attractive but misleading proxies or provenance hidden by UI |
| E-06 Interaction | Can every ID be inspected via approved pointer/keyboard/list routes? | Actual user-visible interaction and accessibility evidence | Occlusion-only access, stale selection or inaccessible focus |
| E-07 Containment | Does actual approved execution respect no-command/no-network/no-forest-save? | Scoped package/process observation under lawful tooling | Unexpected effect, source mutation or insufficient observation coverage |
| E-08 Performance | Does the representative candidate meet owner-agreed budgets on named hardware? | Measured startup/interaction/frame/memory/footprint evidence | Unmet budget or missing environment identity |
| E-09 Handoff | Is independent scoped acceptance recorded for exact source and package? | Non-author disposition with limitations | Author self-approval, missing evidence or implied main/release |

Numeric/mapping checks should use a lawful expected-value source, not screenshots as an oracle. Display tests should exercise read-schema doubles without an alternate production admission pin. The 2D reference's separation between Core admission negatives and Viewer doubles is useful precedent, not a 3D test permission. [M4] [M5]

Rendered review must include representative overlap/occlusion and selected-ID inspection, not just a pleasing overview. A useful review record would link the source ID, its original dimensions, the mapped instance and the displayed inspector in the same exact candidate context.

The independent reviewer must be able to report partial evidence honestly. Absence of a network request in an uninstrumented session is not a global no-network proof. A directory-status observation is not byte-integrity proof. The adopted H0-A receipt specifically warns against overclaiming protection coverage. [M4]

## 12. Owner decisions still required

The following choices are deliberately open. They are a short decision agenda, not accepted recommendations. The Manager can consolidate them with A2 and Q, but A1 cannot close them. The packet expressly leaves these product/technology questions unresolved. [M2]

1. **Purpose:** choose a separate non-public inspection prototype, a separately scoped step toward a later Multi simulator, or defer 3D. The first keeps proof narrow; the second needs clear limits to prevent a viewer being treated as accepted game infrastructure; deferral avoids a new implementation commitment.
2. **Runtime:** choose browser/WebGL, a separate Unity arrangement, or another specified native runtime. Each requires its own exact containment, tooling, licence, accessibility and performance evidence. No option inherits another lane's rights.
3. **Time scope:** choose Year0 only, or commission a separately authorised historical Year1 extension later. Year0 keeps admission and truth claims small; Year1 adds protected input, lineage, transition and independent display qualification. Draft Year2 is not an authoritative rendering option under present authority.
4. **Source/read arrangement:** choose explicitly authorised reuse of a frozen read artifact, a separately qualified immutable projection, or a separately justified decoder. Reuse reduces divergent definitions but adds coupling/custody; projection adds a new artifact to qualify; another decoder adds parity and maintenance burden. A live API would need a new scope decision.
5. **Visual fidelity and interaction:** choose schematic dimensional proxies with bounded inspection, or commission a broader content/camera scope. Proxies make source limits clearer and reduce asset intake; more realism introduces missing morphology, licence, performance and ecological-cue questions. Walking is a separate feature choice.
6. **Acceptance envelope:** choose exact target machine/runtime, accessibility expectations and measurable performance budgets. A narrow local matrix supports a bounded conclusion; a broader matrix costs more evidence and cannot be presumed from one successful render.
7. **Custody, storage and publication:** approve a fresh physical lane, actor-specific grants, exact allowed artifacts and non-public handling; decide any later distribution/source publication separately. Documentation publication here does not grant application publication, protected data transfer, main integration or an L4 waiver.

**A1 recommendation for consideration:** approve writing the formal spec for the narrow Year0 inspection question after Q/Manager review, with runtime choice left to the owner. This is a recommendation about the next decision artifact, not a request to start source work and not an approval of the candidate spec name.

## 13. Unknowns and deliberate verification limits

**Phase A contains NO verified per-tree geometry of any kind. Every W-xx check (W-01–W-10) is NOT RUN.** Document-reported schema, transform and geometry requirements are not verified coordinates, meshes or source-to-scene correspondence.

### 13.1 Outstanding evidence register

| Unknown / not independently verified | Consequence | Appropriate later resolver |
| --- | --- | --- |
| Actual fixture rows/coordinates/numeric bits | No claim that any rendered tree matches the experiment | Authorised source custodian and independent exact-package reviewer |
| Current physical protected-source custody | No source access or transfer permission inferred | Current lawful custodian under a fresh grant |
| Fresh Year1 numeric evidence | Only packet-attributed historical/local status can be stated | Separately authorised continuation reviewer if later product scope requires it |
| Per-tree cell mapping | Cannot claim cell occupancy or attach a guessed cell ID | Approved source/spec author if cell semantics are needed |
| Branching/crown-base/taper/terrain provenance | Photorealistic morphology/terrain cannot be represented as sourced | Owner/content/spec decision with explicit evidence |
| Runtime availability and exact package/toolchain | No technology feasibility or compatibility result | Later named implementation/qualification worker |
| Performance/memory/footprint | No benchmark or numerical target claimed | Owner selects envelope; lawful future measurement supplies evidence |
| Real accessibility/assistive-technology behaviour | No conformance claim | A2/spec plus actual representative independent checks |
| Effective writer/runtime containment | No global protected-file or no-effect claim | Actor-specific permit and independent scoped observation |
| Licence/package/asset rights | No new dependency/asset reuse permission | Owner and exact licence/provenance review |
| Manager disposition of r1 following the cited independent Q review | Corrections are author-reported; no acceptance or self-approval | Manager #730 and any separately assigned nonauthor recheck |
| Formal 3D spec, G1/G2, integration/release | No implementation or deployment authority | Owner/Manager and physical custodians through explicit gates |
| Dashboard/Drive synchronization | Publication of this file proves neither | Designated manager/publisher; not this sole-file worker |

### 13.2 Work actually performed versus not performed

**Performed:** verified repository/origin and clean initial status; fetched origin; created the assigned branch at the exact required SHA and verified HEAD; read the corrected packet and Manager comment; consulted canonical context/live GitHub coordination; read H0-A approval/interface documents; consulted primary platform documentation through read-only web retrieval; posted one claim before editing; authored this sole Markdown dossier.

**Not performed:** fixture/protected-source reads, source import or copying, source implementation, application/HTML/JS/CSS creation, package installation, asset generation/import, geometry execution, browser launch, Editor/Unity/Windows work, build, compiler/test/CI execution, local hash baseline of protected material, physical Ops/lease changes, Host/network/save feature changes, main changes, PR, public application release, owner decision or self-approval.

Read-only GitHub/web retrieval for documentary evidence and the explicitly authorised claim/Git publication are task administration. They are not a tested application network surface. No new application effect is implemented or authorised by this dossier.

**Administrative retrieval limits:** the corrected packet was absent at its same relative path on the required starting commit; a direct local read returned “No such file or directory”. It was then read successfully through the exact supplied GitHub branch URL. Initial combined canonical-context output exceeded the tool output budget; focused relevant reads were used afterward. A pre-commit Git whitespace check also identified one extra blank line at the end of this dossier; it was removed before committing. These administrative issues establish no implementation failure or runtime result.

No “PASS”, completed feature, accepted design or qualified runtime is asserted. Future checks in this document remain NOT RUN. Publication should be assessed only as a one-file design handoff.

## 14. Citation register — sources actually used

Reference links throughout resolve to the actual inspected documents below. Multi factual claims use M sources; canonical context sources establish general workflow/design principles only. Platform sources establish limited technology facts; option trade-offs and proposed controls are this worker's judgement.

### 14.1 Multi authority and approved 2D reference

| Ref | Actual source | Use and evidence limit |
| --- | --- | --- |
| M1 | [Manager #730 binding Phase A adoption, issue #770 comment 6988856214](https://github.com/jermur788/CCF/issues/770#issuecomment-6988856214) | Read actual comment via GitHub. Docs-only authority, superseded routing, no source/runtime/physical permission. |
| M2 | [Corrected Multi #770 orchestration packet](https://github.com/jermur788/CCF/blob/894129616802daf7b647d48734c03adf2c700ae6/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-770-DEEPSEEK-TG-FORK-MULTI-3D-SIMULATOR-REAL-WORK-ORCHESTRATION-PACKET-20261118.md) | Pinned at `894129616802daf7b647d48734c03adf2c700ae6`; complete text reread at this pin for r1. Supplies synthetic experiment, year/draft status, lane boundaries and later gate requirements. Underlying protected sources not inspected. |
| M3 | [#750 H0-A owner choice and four-file approval receipt](https://github.com/jermur788/CCF/blob/a9579188d1e527f4e94d476c0e02e4285bab0035/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-750-OWNER-H0-A-CHOICE-AND-FOUR-FILE-SPEC-APPROVAL-RECEIPT-20261118.md) | Pinned at `a9579188d1e527f4e94d476c0e02e4285bab0035`; complete receipt reread at this pin for r1. Approved static 2D scope and integrity/publication boundaries; not a 3D grant or runtime proof. |
| M4 | [#758 Manager contract adoption and owner local-only JS security receipt](https://github.com/jermur788/CCF/blob/a7e9228b8429e6b51c1fcb493e02af0a4148452a/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-730-H0-A-758-CONTRACT-VERDICT-AND-OWNER-LOCAL-ONLY-JS-SECURITY-APPROVAL-20261118.md) | Pinned at `a7e9228b8429e6b51c1fcb493e02af0a4148452a` (18:32:00Z); complete receipt reread at this pin for r1. Adopts v1.0.0 for H0-A and records limited security permission; no inherited 3D rights, global byte proof or L4 waiver. |
| M5 | [Complete corrected #758 reference contract text](https://github.com/jermur788/CCF/blob/7af3add1d23453a92925a404d7dad5899ab0ff7d/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-758-H0-A-TARGETED-CONTRACT-CORRECTIONS-DRAFT-RESULT-20261118.md) | Pinned at `7af3add1d23453a92925a404d7dad5899ab0ff7d` (18:26:36Z), before M4's 18:32:00Z adoption; complete text reread at this pin for r1. Its historical DRAFT header is qualified by that adoption as `ccf.h0a.core-read` v1.0.0 for H0-A. Reported values are not new fixture measurements. |
| M6 | [Actual A1 RUNNING claim](https://github.com/jermur788/CCF/issues/770#issuecomment-6989053274) | Created once before the first dossier edit; records real worker, base, branch, sole path and docs-only boundary. |

### 14.2 Canonical project context consulted

| Ref | Source | Use and limit |
| --- | --- | --- |
| C1 | [AgentWorkflow at starting HEAD](https://github.com/jermur788/CCF/blob/ff03290a5ef9c53472618aba32591e5822c663e1/Docs/Project/AgentWorkflow.md) | Authority, context, pre-write/single-writer checks, spec lifecycle, handoff and distinction between proposed/verified/accepted. |
| C2 | [Game Brief at starting HEAD](https://github.com/jermur788/CCF/blob/ff03290a5ef9c53472618aba32591e5822c663e1/Docs/Project/game-brief.md) | General separation of simulation and presentation, small improvements and truthful art. No scenario geometry or source reused. |
| C3 | [Unity Project Overview at starting HEAD](https://github.com/jermur788/CCF/blob/ff03290a5ef9c53472618aba32591e5822c663e1/Docs/Project/unity-project-overview.md) | Relevant project/environment and verification context only; recorded configuration is not a fresh installation check. |
| C4 | [Decision Log at starting HEAD](https://github.com/jermur788/CCF/blob/ff03290a5ef9c53472618aba32591e5822c663e1/Docs/Project/decision-log.md) | Authority/status vocabulary and general current workflow; no unrelated feature decision used as Multi authority. |
| C5 | [Current Milestone at starting HEAD](https://github.com/jermur788/CCF/blob/ff03290a5ef9c53472618aba32591e5822c663e1/Docs/Project/current-milestone.md) | Context/scope separation only; not a Multi 3D feature specification. |
| C6 | [Live GitHub active tasks](https://github.com/jermur788/CCF/blob/main/Docs/Project/coordination/active-tasks.md) | Read at task start and after fetch; mutable coordination context, not a new physical lane inspection or grant. Later specific M1/M2 correction governs this task. |
| C7 | [Research Index at starting HEAD](https://github.com/jermur788/CCF/blob/ff03290a5ef9c53472618aba32591e5822c663e1/Docs/Project/research-index.md) | Evidence-versus-decision distinction only; no ecological research coefficients or underlying research sources imported. |

### 14.3 Primary technology references consulted on 18 November 2026

| Ref | Source | Limited factual use |
| --- | --- | --- |
| T1 | [Khronos WebGL specification 1.0.3](https://registry.khronos.org/webgl/specs/1.0.3/) | Canvas 3D context, creation failure, security/resource and context-loss provisions. The [latest editor's draft](https://registry.khronos.org/webgl/specs/latest/1.0/) was also consulted and identifies itself as work in progress; the versioned reference is used for the comparison. Neither selects a future WebGL version. |
| T2 | [Unity 6000.6 Web platform documentation](https://docs.unity.com/en-us/engine/6000.6/manual/platform-specific/webgl) | Existence of distinct Web development/delivery guidance. Not a local-offline compatibility or export result. |
| T3 | [Unity 6000.6 cross-platform considerations](https://docs.unity.com/en-us/engine/6000.6/manual/platform-specific/cross-platform-features/considerations) | Hardware/input/deployment differences and performance portability caveats. Legacy input examples on that page are not adopted; repository New Input System rule remains binding. |
| T4 | [W3C How to Meet WCAG quick reference](https://www.w3.org/WAI/WCAG22/quickref/) | Relevant web accessibility categories; no conformance verdict and no claim this specifies native runtime behaviour. |
| T5 | [Godot stable command-line tutorial](https://docs.godotengine.org/en/stable/tutorials/editor/command_line_tutorial.html) | Documented CLI/headless/export capabilities only; no tool was run. Stable documentation is mutable and would need a version pin later. |
| T6 | [Godot stable licence compliance guidance](https://docs.godotengine.org/en/stable/about/complying_with_licenses.html) | Engine licence/notice obligations and separate third-party asset notices; no project-specific legal clearance. |

No issue/source linked inside these documents is treated as independently inspected merely because it appears in a citation chain. In particular, Year1 and draft follow-on status are attributed to M2; their underlying physical material was not opened.

[M1]: https://github.com/jermur788/CCF/issues/770#issuecomment-6988856214
[M2]: https://github.com/jermur788/CCF/blob/894129616802daf7b647d48734c03adf2c700ae6/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-770-DEEPSEEK-TG-FORK-MULTI-3D-SIMULATOR-REAL-WORK-ORCHESTRATION-PACKET-20261118.md
[M3]: https://github.com/jermur788/CCF/blob/a9579188d1e527f4e94d476c0e02e4285bab0035/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-750-OWNER-H0-A-CHOICE-AND-FOUR-FILE-SPEC-APPROVAL-RECEIPT-20261118.md
[M4]: https://github.com/jermur788/CCF/blob/a7e9228b8429e6b51c1fcb493e02af0a4148452a/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-730-H0-A-758-CONTRACT-VERDICT-AND-OWNER-LOCAL-ONLY-JS-SECURITY-APPROVAL-20261118.md
[M5]: https://github.com/jermur788/CCF/blob/7af3add1d23453a92925a404d7dad5899ab0ff7d/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-758-H0-A-TARGETED-CONTRACT-CORRECTIONS-DRAFT-RESULT-20261118.md
[C1]: https://github.com/jermur788/CCF/blob/ff03290a5ef9c53472618aba32591e5822c663e1/Docs/Project/AgentWorkflow.md
[C2]: https://github.com/jermur788/CCF/blob/ff03290a5ef9c53472618aba32591e5822c663e1/Docs/Project/game-brief.md
[C3]: https://github.com/jermur788/CCF/blob/ff03290a5ef9c53472618aba32591e5822c663e1/Docs/Project/unity-project-overview.md
[C6]: https://github.com/jermur788/CCF/blob/main/Docs/Project/coordination/active-tasks.md
[T1]: https://registry.khronos.org/webgl/specs/1.0.3/
[T2]: https://docs.unity.com/en-us/engine/6000.6/manual/platform-specific/webgl
[T3]: https://docs.unity.com/en-us/engine/6000.6/manual/platform-specific/cross-platform-features/considerations
[T4]: https://www.w3.org/WAI/WCAG22/quickref/
[T5]: https://docs.godotengine.org/en/stable/tutorials/editor/command_line_tutorial.html
[T6]: https://docs.godotengine.org/en/stable/about/complying_with_licenses.html
