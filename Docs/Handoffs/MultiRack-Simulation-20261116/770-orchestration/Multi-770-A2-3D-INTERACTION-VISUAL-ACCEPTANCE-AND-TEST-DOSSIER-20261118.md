# Multi #770 A2 — 3D interaction, accessibility, visual acceptance and scoped test design dossier

**18 November 2026 · Phase A, GitHub docs-only · lane A2 · author: Claude Sonnet 5.5 (`claude-sonnet-5-5`) via Claude Code in T3 Code, effort as set by the session (low); harness label NOT VERIFIED.**
**Claim:** https://github.com/jermur788/CCF/issues/770#issuecomment-6989030661 · **Branch:** `docs/multi-770-a2-3d-interaction-acceptance-dossier-20261118-v1` · **Base:** `ff03290a5ef9c53472618aba32591e5822c663e1`

> **STATUS: PROPOSAL / DRAFT FOR OWNER DECISION. NOT AN APPROVED SPECIFICATION.** There is no adopted Multi 3D scene or feature specification, no 3D physical source lane and no validated 3D runtime ([C1], "Task intent"). A proposed 3D view is not an accepted game feature. Nothing here was built, run, rendered or tested. Every test in §7 is a **PROPOSED vector, NOT RUN**. This dossier recommends; it decides no technology, scope, fidelity, runtime or policy (§9 lists the decisions it leaves open). It is not a PASS of anything and it is not self-approved. A1 (architecture/truth boundary) and Q (independent review) are separate lanes. I have not read A1's file. I read Q's review in revision r1, as the work order for the correction.

---

## Revision r1 — post-review correction

**Revision of:** `b34d1995d60edba2335727aec38ce05a16795c65` (r0). **Work order:** [Q independent review, commit `52b010d7`](https://github.com/jermur788/CCF/blob/52b010d754ec12e39998e08a8f53c944e43277d6/Docs/Handoffs/MultiRack-Simulation-20261116/770-orchestration/Multi-770-Q-INDEPENDENT-3D-ARCHITECTURE-AND-BOUNDARY-REVIEW-20261118.md) (verdict: BLOCKING FINDINGS, NEEDS AUTHOR REVISION). **Same author, same sole file, no other file touched.** I disagree with no finding, so nothing is marked DISPUTED. Q-01 stands as written.

| Finding | Status | What changed in this file |
|---|---|---|
| **Q-01** BLOCKER | ADDRESSED | I read the adopted contract text (R-758A, R-758C: [C4], [C6]). §0 no longer lists it as "not read", and r0's statement that it was prohibited was wrong. Everything the contract resolves is now **[V] document-stated** instead of **[U]**: ID format, integer-mm coordinates, extent, transform ID, `heightMm`/`heightM`, `crownRadiusM` as a radius under a potential-crown rule, seeds, fixture size and pin (§0 F-12..F-19, §1.2, §1.4, §6). D-4 and §1.2 no longer say height/crown data do not exist. The D-15 "a read grant is needed to know the schema" claim is withdrawn: a grant is needed only to read the **bytes**. |
| **Q-02** MAJOR | ADDRESSED | `status` is the per-row life-status token, projected as `lifeStatus: "Live"`, not health and not dataset status. It is removed from the banner and the dataset provenance (§6.2, T-18). It is shown per tree as "Life status: Live (source token; not health)" (§2.2). The banner now carries "static Genesis Year0 fixture" plus the source-identity pin. |
| **Q-03** MAJOR | ADDRESSED | The viewer never computes the <170 mm comparison. It carries and displays `belowActionNote` / `actionNote.comparison` as a read-boundary `PresentComputed` value. The viewer-side DERIVED label is withdrawn. The base vocabulary is the contract's four availability states plus Unsupported / Blocked / Refused. PRESENTATION sits on top for view-only values. `NotEvaluated` and `Unavailable` are no longer merged (§2.2, §6.3, T-19). |
| **Q-04** MAJOR (my part) | ADDRESSED | `unityX` / `unityZ` are named in §1.2 as document-reported centred forward-transform f32 values. They establish **no** Unity frame, handedness or engine meaning. Any 3D mapping cites the transform ID, never the field name. |
| **Q-05** MAJOR | ADDRESSED | §3 and §7 are relabelled as **browser-candidate** material. A new runtime-neutral vector set (§7.0, N-01..N-08) states required **outcomes** without assuming DOM mechanisms. D-8 now says it must not follow from test convenience. |
| **Q-06** MAJOR (my part) | ADDRESSED | The false premise ("marker height, not tree height / no crown because no data") is withdrawn. §1.2.1 presents markers-only, schematic height+crown envelopes and richer art as owner options with their honesty trade-offs. A diameter exaggeration may stay an option only as a disclosed, named, versioned display-mapping field (§1.2.1, D-4, D-5, T-20). |
| **Q-08** MINOR | ADDRESSED | T-17 is rewritten. The production pin is unchanged. Refusal states reach the viewer through an authorised harness using doubles. Byte-level wrong-digest negatives stay in the Core's own separate suite. |
| **Q-09** MINOR | ADDRESSED | No-save / no-network / no-storage are reworded as 3D **proposals** that follow the H0-A precedent. They are not 2D rules binding 3D (§1.3, §2.3, §4.3). |
| **Q-10** MINOR | ADDRESSED | Year1 label is now "Historical Year1 - locally qualified numeric continuation; 3D display not yet qualified" (§6.3, D-3). |
| **Q-11** MINOR (my part) | ADDRESSED | A separate 3D codec is no longer called a "breach". It is presented as precedent and risk (§8, D-11). |
| **Q-12** MINOR | ADDRESSED | A three-identity model replaces the two-identity wording: one fixture trust root, an observation identity, and a separate future package/code integrity root (§6.1a, §6.2). |
| **Q-15** OBS | ADDRESSED | "Verified" wording that could read as data verification is reworded to "document-stated". The [V] tag is redefined accordingly (§0). |
| **Q-07, Q-13, Q-14, Q-16** | NOT IN MY LANE | Not attempted, as instructed. Q-07 is A1-only. Q-13 and Q-16 are A1 observations. Q-14 (Canvas-2D pseudo-3D and #769 proximity) is left unchanged. |

**What r1 does not claim.** Every value taken from the contract is **document-stated**. I did not read or rehash any fixture byte. Nothing was built, run, rendered or tested. Every §7 vector is still NOT RUN.

---

## 0. Evidence key and what this dossier stands on

| Tag | Meaning |
|---|---|
| **[V]** | **Document-stated:** stated in a cited document that I read this session. The citation is given. It means the document says so. It does **not** mean I checked fixture bytes, ran code or confirmed any data value. |
| **[M]** | Manager/owner decision, quoted from a cited document. |
| **[P]** | Proposal by this dossier. Not a decision. |
| **[U]** | UNKNOWN / NOT VERIFIED. I did not see evidence for it. Do not treat it as true. |

**Facts I rely on (all read first-hand):**

| # | Fact | Tag | Source |
|---|---|---|---|
| F-1 | The Multi reference is a synthetic 30×30 m pure-Sitka experiment with 100 rows at Year0. It has an independently qualified, locally protected Year1 numeric continuation. Year2 is a DRAFT. | [V] | [C1] "Task intent" |
| F-2 | No adopted Multi 3D scene/feature specification, 3D physical source lane or validated runtime exists. Phase A may recommend but not decide technology, scope, fidelity, runtime or policy. | [V][M] | [C1], [C2] |
| F-3 | The 2D H0-A viewer is an owner-approved, non-public, self-contained offline HTML/Canvas Year0 viewer with the fixture embedded (the receipt calls it verified; I did not verify it). It has no runtime fetch, server, file chooser, upload, CDN, telemetry, localStorage or saved forest. It targets current desktop browsers. | [V][M] | [C3] |
| F-4 | H0-A scope is frozen synthetic 30×30 m / 100 Sitka, static `Genesis` Year0 only. It has zoom/pan/fit, stem-centre selection and a complete accessible inspector/list. It has no annual computation, health claims, Irish forecast, Unity parity, Year1 toggle, marks/Work Plans/Fell, history, save or network. | [V][M] | [C3] |
| F-5 | **No implementation, browser rendering, build or test of H0-A has been done.** The 65 requirements are unverified. | [V] | [C3] "NOT DONE" |
| F-6 | `ccf.h0a.core-read` v1.0.0 is approved as the local H0-A implementation interface, **restricted to the H0-A four-file spec**. It is not approval of Year1/Year2/marking/history APIs. | [V][M] | [C4] |
| F-7 | Contract content cited: one production `CcfYear0Core` with a fixed 32,567-byte / pinned-SHA-256 trust root. Wrong bytes are rejected before JSON parse. No alternative Core decoder in the Viewer. Genesis-only truth. All 100 IDs. Per-row source `status` token `"live"` (a life-status token, not health or dataset status). binary32 codec and full observation identity. Strict `dbhMm < 170` is an **annotation only**, with **no "eligible" label at the boundary**. No annual/mark/save functions. | [V] | [C4] |
| F-8 | The H0 owner choice is private offline local evaluation only. The public CCF GitHub may receive only safe docs, claims, verdicts and hashes, never viewer HTML/JS/fixture copies unless separately authorised. L4 FAIL38 is UNWAIVED. | [V][M] | [C3], [C4], [C1] |
| F-9 | Year1 (#725 source, #729 review, #732 native qualification), Year2+ (#760 DRAFT) and marking (#761 DRAFT) exist as named issues. **I did not open or read them.** Their status here is as the packet states it. | [V] (packet text only) | [C1] |
| F-10 | The existing local H0 source lanes are solely for 2D H0-A. No 3D physical lane or source claim is available. | [V] | [C1] (quoting the #766 report) |
| F-11 | Repo rules: do not invent ecological facts; do not add packages or external dependencies without approval. | [V] | [C5] `AGENTS.md` lines 34 and 43 at the base SHA |
| F-12 | **Adopted-contract status.** The Manager adopted the contract text as `ccf.h0a.core-read` v1.0.0 ("APPROVED AS THE LOCAL H0-A IMPLEMENTATION INTERFACE"), restricted to the H0-A four-file spec. The complete text is R-758C. Its header still says "DRAFT", but R-758A adopts that text. | [V][M] | [C4], [C6] |
| F-13 | **IDs and roster.** The row IDs are exactly `P30-001` … `P30-100`, in that source order. `treeCount` is 100. Each row has a `sourceIndex` (0..99) that is provenance, never identity. Every row has the same 12 source keys: `treeId, xMm, yMm, status, ageYears, dbhMm, heightMm, dbhCm, heightM, crownRadiusM, unityX, unityZ`. | [V] | [C6] §1.3, §2.2, §2.5 |
| F-14 | **`status` is a per-row life-status token.** Its only source value is `"live"`. The read projection maps it 1:1 to `lifeStatus: "Live"`, with the explicit note "NOT health". It is not a dataset or provenance status. | [V] | [C6] §1.3, §2.5 `ReadTreeV1` |
| F-15 | **Coordinates and extent.** `xMm` / `yMm` are integers in millimetres, in the half-open range `[0, 30000)`. The geometry model is a 30000 mm square, polygon `(0,0),(30000,0),(30000,30000),(0,30000)`, elevation 0. The contract's local-frame text is "SW-origin integer mm; +X east; +Y north (synthetic local, not geographic)". The transform ID is `p1-square30-corner-to-centred-xz-v1`: `X = fround((xMm − 15000)/1000)`, `Z = fround((yMm − 15000)/1000)`. `unityX` / `unityZ` are f32 envelopes of those centred values. The H0-A 2D view draws from the **integer mm** fields, not from the centred values. | [V] | [C6] §1.3, §2.3, §2.5, §2.6 |
| F-16 | **Height, DBH and crown.** `dbhMm` and `heightMm` are integers. `dbhCm`, `heightM` and `crownRadiusM` are binary32 envelopes. `crownRadiusM` is a **radius**, not a diameter. The crown rule ID is `potential-crown-at-ci0-linear-f32-v1`: it is a **potential** crown, not a measured crown. The Core never recomputes the crown, and the radius has a documented 0.1 m floor. The row carries **no** crown-base, crown-length or crown-shape field. I have **not** seen the text of the `crownRule` free-text field (**[U]**). | [V] | [C6] §1.3, §2.2, §2.4, §2.5 |
| F-17 | **Seeds.** `derivation.generatorSeed` = 6390001 sits in the fixture's derivation block (`kind="designed"`). `ecology.simulationSeed` = 20261116 sits in the ecology block. The contract says explicitly that they are distinct. `annualCallMade` is false: no annual step is recorded for this frame. | [V] | [C6] §1.3, §2.4, §2.5 |
| F-18 | **Fixture identity.** The fixture is 32,567 bytes with SHA-256 `bcaaa5c3…57c4` (full value in [C6] §1.3 and §2.1). The Core admits only bytes matching this pin. The #756 author reported these values from a public blob. #758 did not re-read it. **I did not read or rehash any fixture bytes.** | [V] (document-stated) | [C4], [C6] §1.3, §2.1 |
| F-19 | **Availability states and the <170 mm note.** Optional values are exactly one of `PresentSourced`, `PresentComputed`, `NotEvaluated` or `Unavailable`. `hegyiCi` and the other annual diagnostics are `NotEvaluated`. `healthScore` and the stand aggregates (`stemsPerHa`, `basalAreaPerHa`, `quadraticMeanDbh`, `relativeDensity`) are `Unavailable`. The read boundary supplies `belowActionNote` and `actionNote.comparison` ("dbhMm < thresholdMm (strict), annotation only"). The Viewer "**never** compares DBH itself and never says eligible or ineligible". The #756 author reported 12 rows below 170 mm and exactly one at 170 mm. | [V] | [C6] §2.5, §2.6, §1.3 |

**Deliberately NOT read or used (prohibited or out of lane):** fixtures, protected P0 trees, #687/#710/#725 source, Scenario One #718/#690/#652/#763/#765, **any #769 file**, the #750 four spec files, #756/#757 and the A1 dossier. Where a fact would need those I write **[U]**.

**Read in r1, and wrongly avoided in r0:** the adopted #758 contract documents, which are public Markdown. They are [C4] (R-758A, the Manager's adoption at pin `a7e9228b`) and [C6] (R-758C, the complete contract text at pin `7af3add1`). I also read Q's review [C7] as the work order. In r0 I listed "the full #758 DRAFT result" under "prohibited or out of lane". That was wrong. The packet cites the #758 adopted contract as binding evidence ([C1]), and R-758A adopts that exact text. Reading it is allowed. Everything it resolves is now **[V] document-stated**, not [U].

---

## 1. Scene, camera, tree positioning and ID mapping (PROPOSAL)

### 1.1 Governing rule

**[P]** The 3D scene is a pure, deterministic function of the already-trusted Year0 read projection. The scene never owns, computes or invents biology. If a value the scene needs is absent from the projection, the view refuses (§7 T-17) or omits that visual element. It never fills the gap with a plausible guess. Whether the 3D view consumes the *same* read projection as H0-A is an owner decision (§9 D-11). `ccf.h0a.core-read` v1.0.0 is **not** an approved 3D API ([C4], F-6).

### 1.2 Positioning the 100 identities

| Element | Proposal [P] | Known / UNKNOWN | Truthfulness rule |
|---|---|---|---|
| Source of positions | Each tree's horizontal position comes from the integer `xMm` / `yMm` fields (millimetres). The 3D scene converts them with **one declared, named, versioned mapping** (§9 D-15). The H0-A 2D view also draws from the integer mm fields ([C6] §2.6), so the integers are the natural cross-check against 2D. | [V] `xMm`/`yMm` are integers in the half-open range `[0, 30000)` (F-15). **[U]** which 3D mapping the owner chooses. | No coordinate is generated, jittered, snapped, "spread out" or relaxed to avoid overlap. Overlap is data. |
| `unityX` / `unityZ` | **Not used as a 3D coordinate by default.** They are document-reported f32 values of the centred forward transform `fround((mm − 15000)/1000)` under the transform ID `p1-square30-corner-to-centred-xz-v1`. The Core uses them as its `TransformBitMismatch` oracle ([C6] §2.2, §2.3). | [V] what the contract documents about them. They establish **no** Unity frame, handedness, up axis, pivot or engine meaning, and they say nothing about any Unity scene. The "unity" in the field name is a label only. | Any 3D mapping that uses the centred values must cite the **transform ID**, never the field name. A later implementer must not read `unityX/unityZ` as a ready engine coordinate. |
| Tree count | Exactly the number of records in the projection. Expected 100: `treeCount` and the roster are both 100 ([V] F-13). | Document-stated. The Core refuses a roster count other than 100 at admission. | If count ≠ the source-declared count, refuse. Do not pad or truncate. |
| Ground plate | A flat neutral plate sized to the source-declared extent: a 30000 mm square with the documented polygon ([V] F-15). It is labelled "schematic ground plane, not a survey". | [V] the extent, polygon and elevation 0 are in the contract. | Draw the plate only from the admitted projection's geometry values, never from a literal in the viewer. |
| Elevation / terrain | Flat. The documented elevation is 0. | [V] elevation 0. No terrain data is recorded. | Do not add slope, relief, soil, horizon, sky photograph, compass or geographic marker. The experiment is synthetic with no geographic truth ([C1]), and the contract's own frame text says "synthetic local, not geographic". |
| Axis mapping | Source horizontal axes map to scene horizontal axes by one declared, tested mapping. Handedness, up axis and pivot are stated by that mapping, not assumed from the field names. | [V] the source frame text: SW-origin, +X east, +Y north, synthetic local (F-15). **[U]** the 3D handedness, up axis and pivot (owner/spec). | The mapping is shown in the provenance panel (§6). If an axis is labelled at all, use "local +X / local +Y (synthetic local frame, not geographic)". No compass. |
| Out-of-extent | The authoritative check is at the read boundary: admission refuses a coordinate outside `[0, 30000)` (`CoordinateOutOfDomain`, [C6] §2.2). The scene builder treats an out-of-extent value as a refusal condition too (defence in depth). | [V] the boundary refuses it. | No clamping. |
| Species and stage | Scene-level caption "synthetic pure-Sitka experiment" ([V] F-1, F-4). Each row carries `species` "Sitka", `lifeStage` "MatureEquivalent" and `demandClass` "Reference" ([C6] §2.5). A per-tree label may show them. | [V] document-stated. | No second species is ever drawn. |
| Life status | Shown per tree as "Life status: Live (source token; not health)" (§2.2). **Never** drawn as a colour or ramp and **never** placed in the banner. | [V] F-14 | A per-row source token is not a health signal and not a dataset status. |
| Height, DBH, crown | The projection **does carry** `dbhMm`, `heightMm` and `heightM`, and a `crownRadiusM` radius (F-16). How much of this becomes geometry is an owner choice (§1.2.1, §9 D-4). | [V] fields exist. **[U]** the crown rule's free text, and any crown base, length or shape (the row carries none). | The crown is a **potential crown** under `potential-crown-at-ci0-linear-f32-v1`, not a measured crown. It must never be drawn or labelled as a measured, competition-adjusted or surveyed crown. |

#### 1.2.1 Geometry fidelity options (owner decision, none selected)

r0 framed this as "marker height, not tree height / no crown because no data". That was a false premise: height and crown radius exist in the adopted contract. The three tiers below are options with their honesty trade-offs. **This dossier does not choose one** (§9 D-4).

| Option | What is drawn | Source data used | Honesty trade-off |
|---|---|---|---|
| **G-1 Markers only** | A vertical cylinder at each position. Its diameter comes from `dbhMm`, either at true scale or through a disclosed display mapping (below). The marker height is a **uniform** neutral height, labelled "marker height, not tree height". Height and crown radius appear only in the inspector. | Position, `dbhMm` | It invents no shape detail and draws no crown. It does discard the sourced height and crown radius, so it cannot show the canopy-to-canopy relationship. A uniform height could be misread as equal tree heights, hence the label. |
| **G-2 Schematic height + crown envelopes** | A stem cylinder at the sourced `heightM` and `dbhMm`, plus a schematic crown envelope with horizontal radius `crownRadiusM`. | Position, `dbhMm`, `heightM`, `crownRadiusM` | Height and radius are sourced. The crown's **shape, vertical extent and base height are not source data** (the row carries none), so they are display choices and must be disclosed as schematic. The crown is a *potential* crown (CI 0), so overlapping envelopes show the rule's output and are **not** a competition outcome, and must not read as a measured canopy. The banner, the legend and the inspector must say so. |
| **G-3 Richer art** (trunk taper, branches, foliage texture, lighting) | Detailed tree models. | Same data, plus invented shape detail | Most of the detail has no source. It risks implying health, species vigour or ecological state, which conflicts with the repo rule against inventing ecological facts ([C5] line 43). If chosen, it needs its own owner-set honesty limits. |

**Diameter exaggeration (a modifier on any option, not a baseline).** At 30 m a DBH under 170 mm (that boundary is document-stated, [C6] §2.5) is under 0.17 m. At true scale it is a hairline. If an exaggeration is retained as an option, it must be a **disclosed, named, versioned display-mapping field**, for example a mapping ID, a version and the factor actually applied. It is part of the scene manifest digest (§6.1) and is shown in a persistent badge. It must never be a silent mesh scale or pivot change. Q's review notes that such a mapping would have to be accepted explicitly by A1's W-06 fail condition ("mesh scale/pivot changes source magnitude"), which I have not read. A **true-scale** mode needs no mapping field beyond the unit conversion. The factor is not chosen here (§9 D-5).

### 1.3 Camera

| Aspect | Proposal [P] | Why |
|---|---|---|
| Default pose | A deterministic fit-all pose computed only from the source extent (no stored or random pose). | Reproducible first render; fits the "fit" action H0-A already has ([V] F-4). |
| Presets | (a) **Plan** (top-down, axis-aligned, comparable to a 2D plan view); (b) **Oblique** (one fixed elevated angle). Both are view-only. | The plan preset is the easiest cross-check against the 2D view. |
| Free navigation | Orbit + zoom about the plate centre, with min/max distance derived from the extent. No fly-through, first-person, walk, VR, auto-orbit or auto-tour in the baseline. | Cognitive load, motion sensitivity and avoiding a "forest walk" that implies ground-level realism. |
| Persistence | Camera pose is volatile view state. It is never written anywhere. | **3D proposal** that follows the H0-A precedent (no localStorage or saved state, [C3] F-3). The H0-A rule does not bind 3D. A 3D no-persistence rule needs its own owner decision (§9). |
| Clip planes | Derived from the extent so no tree is clipped in any permitted pose. | Completeness: all 100 must be reachable and visible. |
| Reset | One always-available reset to the default pose (button and key). | Recoverability for keyboard and low-vision users. |

### 1.4 Identity mapping: data ID ↔ stable visual identity ↔ ordinal

**[P] Three distinct things must never be conflated:**

| Concept | Definition | Authoritative? | Where it appears |
|---|---|---|---|
| **Data ID** | The `treeId` verbatim from the source projection. All 100 are required ([V] F-7). The format is `P30-001` … `P30-100` ([V] F-13). | **Yes.** It is the only identity of a tree. A selection or pick is only meaningful together with the observation identity it came from (§6.1a). | Inspector, list, ARIA names, test oracles, provenance. It is never truncated, reformatted or renumbered. |
| **Visual identity** | The scene instance (mesh instance index and pick key) bound to exactly one data ID. | No. It is derived. | Internal only. Never user-visible. Never used as an ID in the DOM, URL or logs. |
| **Ordinal** | The 1-based position in the current display order, e.g. "list position 17 of 100". The source's own `sourceIndex` (0..99) is provenance only and is also never an ID. | No. It is a presentation convenience. | Always shown **together with** the data ID and labelled "display order, not an ID". |

**Mapping rules [P]:**
1. **Canonical order.** The scene builder takes records in the **source order** that the contract documents: `P30-001` … `P30-100` ([V] F-13). If a future projection stops guaranteeing that order, the fallback is an ordinal comparison of the data-ID strings. The rule used is recorded in the provenance panel and in the scene-manifest digest (§6). No sort may depend on screen position, camera or randomness.
2. **Bijection.** data ID ↔ instance index ↔ DOM list row ↔ pick key is one-to-one and onto the 100 records (test T-01, T-02).
3. **No cosmetic variation in the baseline.** Every tree is identical apart from the source-driven values its geometry option (§1.2.1) uses. Per-tree tint or rotation variety would read as biological difference. If the owner later wants cosmetic variation, it must be a pure function of (a fixed documented constant, data ID), recorded in provenance, and labelled cosmetic. That constant must **not** be the fixture's `generatorSeed` (6390001) or `simulationSeed` (20261116), which have their own roles ([V] F-17).
4. **Labels in the scene.** A tree's full data ID appears on hover, focus and selection. An optional "show all IDs" toggle is allowed only if full IDs fit without truncation, otherwise it is omitted.
5. **Re-sorting or filtering the list changes ordinals but never IDs.** No code path may use an ordinal as a key (T-03).
6. **Pick = ID.** Picking returns a data ID. Pick buffers, ray hits and screen-space tests are implementation detail below the ID layer.

---

## 2. Inspect-only interaction model (PROPOSAL)

### 2.1 Interactions that exist

| Interaction | Pointer | Keyboard | Effect | State touched |
|---|---|---|---|---|
| Hover | Move over a marker | n/a (see focus) | Highlight and transient read-only tooltip with the data ID | Hover highlight only |
| Select | Click/tap a marker, or choose a row in the list | Activate a list row, or **[ ]** step through the list order while the viewport has focus | Marker highlighted, inspector shows that tree, selection announced (§3) | Selected data ID (volatile) |
| Keyboard focus | n/a | Tab reaches: skip link, banner, list/table, inspector, viewport, camera controls, in a documented order | Visible focus indicator that does not rely on colour alone | Focus only |
| Clear | Click empty ground | Esc | Selection cleared | Selected ID |
| Camera | Drag-orbit, wheel-zoom | Arrow keys orbit, +/- zoom, Home/R reset (active only while the viewport is focused) | Pose changes | Camera pose (volatile) |
| Find by ID (optional) | n/a | Text box filtering the list by an ID substring | List narrowed. The count is shown as "k of 100 shown". | Filter text (volatile) |

Single-character shortcuts are active only when the viewport has focus and must not collide with screen-reader quick keys (WCAG 2.1.4; established standard, cited from general knowledge, not fetched this session).

### 2.2 What is displayed per tree, and its authority label

**[P]** The inspector shows what the trusted projection carries, **with the availability state the projection gives it**. A value that is not sourced is never shown as 0, blank-looking-like-zero or a default. H0-A already distinguishes typed numeric availability ([C3], "F1 typed numeric/availability"), and the contract fixes four states: `PresentSourced`, `PresentComputed`, `NotEvaluated` and `Unavailable` ([V] F-19). The 3D inspector preserves all four and does **not** merge `NotEvaluated` with `Unavailable`.

**The viewer computes nothing biology-adjacent.** In particular it never compares `dbhMm` with the 170 mm threshold. Values computed at the read boundary are shown as the read boundary's (`PresentComputed`). The viewer's own arithmetic is view-only (projection, camera, unit conversion for drawing, layout) and carries the PRESENTATION label.

| Inspector row | Shown value | Availability / label | Notes |
|---|---|---|---|
| Data ID | `treeId` verbatim | `PresentSourced` | `P30-nnn` ([V] F-13). |
| Life status | "Life status: Live (source token; not health)" | `PresentSourced` (source `status` `"live"` mapped 1:1 to `lifeStatus` `"Live"`, [V] F-14) | A per-row source token. Never a colour, a ramp, a banner value or a dataset status. |
| Age | `ageYears` | `PresentSourced` | Integer years. |
| Diameter at breast height | `dbhMm` (integer) and `dbhCm` (f32 decoded by the Core), fixed locale-independent formatting | `PresentSourced` | Units mm and cm. Do not convert silently. |
| Height | `heightMm` (integer) and `heightM` (f32) | `PresentSourced` | Shown even if the geometry option (§1.2.1) does not draw it. |
| Crown radius | "Potential crown radius, `crownRadiusM`: … m (rule `potential-crown-at-ci0-linear-f32-v1`; not a measured crown)" | `PresentSourced` | A radius, not a diameter ([V] F-16). Never "crown width" or "measured". |
| Position | `xMm`, `yMm` in mm, with the source-frame text "local, synthetic, not geographic" | `PresentSourced` | The centred values `unityX/unityZ` may be listed as "document-reported centred forward-transform values (transform ID …); no Unity or engine meaning". |
| Below-170 mm annotation | The read boundary's `belowActionNote` for that row, using the wording of `actionNote.comparison` verbatim, with a label such as "Read-boundary annotation (strict, annotation only)" | `PresentComputed` (computed by the read boundary, **not** by the viewer) | Never "eligible" or "ineligible", and never a treatment, health or removal reading. A true/false value only. The viewer must not derive its own value, and must not derive a count of flagged rows unless the boundary supplies one. |
| Annual diagnostics (`hegyiCi`, `neighbourCount`, `mortalityPressure`, `hazardProbability`, `vulnerability`, `rngRollNumerator`) | "Not evaluated" plus the boundary's reason text | `NotEvaluated` | Reason from the contract: static Genesis, no annual evaluation. Never 0. |
| `healthScore` and the stand aggregates (stems/ha, basal area/ha, quadratic mean DBH, relative density) | "Unavailable" plus the boundary's reason text | `Unavailable` | Reason from the contract: not recorded in the trusted source, or stage aggregate not supplied. Never 0, and never a health colour. |
| List position | "n of 100 (display order)" | PRESENTATION | Not an ID. |
| Display-mapping values (§1.2.1) | The mapping ID, version and factor, or "true scale" | PRESENTATION | Always visible when ≠ true scale. |
| Camera/zoom state | Optional | PRESENTATION | Not a datum. |

Whether the inspector lists the `NotEvaluated` and `Unavailable` rows at all is a spec-stage choice. If it lists them, it keeps the two states apart. The 3D **scene** never draws them. See §6.3 for the systems the source excludes.

**Label vocabulary [P].** The **base vocabulary** is the contract's four availability states, `PresentSourced` / `PresentComputed` / `NotEvaluated` / `Unavailable`, plus the classes **Unsupported** (a concept this frame or view does not support), **Blocked** (a stage or feature that current authority does not allow) and **Refused** (admission failed). Q's review attributes the last three classes to A1 and the contract's refusal handling. I have not read A1, so their exact definitions follow A1 and the spec. A **PRESENTATION** label may sit on top for view-only values. It never replaces a source or boundary state. The retired r0 labels "SOURCE / DERIVED (viewer-computed) / NOT PROVIDED" are withdrawn. Each label is a text prefix plus a shape or icon, so it is not colour-only (§3).

### 2.3 Explicit non-features (the view must not offer any of these)

**Basis column.** Where a row cites H0-A (`[C3]`), that is a **precedent** for a 3D **proposal**. The H0-A constraints are 2D product decisions and do not transfer to 3D in either direction. Each row needs its own 3D owner decision and, where it applies, a G3 decision (§9).

| Non-feature (proposed for 3D) | Basis |
|---|---|
| Marking, Work Plans, Fell, thin, "select for removal" | #761 marking is a DRAFT ([C1]). H0-A excludes marks ([C3]). |
| Year progression, play/step/advance, growth animation, year slider | Year2+ is DRAFT (#760). No annual computation in H0-A ([C3]). |
| Any treatment, what-if, forecast, score, recommendation, productivity or habitat figure | No ecology is modelled in this view ([C5] line 43; [C3] "no health claims, Irish forecast"). |
| Save, load, export, import, file chooser, drag-drop upload, download, screenshot-to-disk, clipboard export | Precedent: H0-A has no saved forest or upload ([C3] F-3). A download is a write. 3D proposal, owner to decide. |
| Persistence: cookie, localStorage, sessionStorage, IndexedDB, Cache Storage, URL-hash state (browser-candidate list; the neutral outcome is "no persistent write", §7.0 N-05) | Precedent: H0-A ([C3] F-3). 3D proposal. |
| Network: fetch, XHR, WebSocket, beacon, CDN, web fonts, telemetry (browser-candidate list; the neutral outcome is "no network use", §7.0 N-06) | Precedent: H0-A ([C3] F-3). 3D proposal. |
| Editing values, dragging or moving trees, hiding trees | Pick is read-only. The data object is deep-frozen **[P]**. |
| Health, risk or "good/bad" colour ramps; heat-maps | Would imply modelled ecology. |
| Arbitrary data input | H0-A scope ([C3]). |

---

## 3. Accessibility (PROPOSAL; browser-candidate mechanisms)

**Reading note.** The mechanisms in this section (`<table>`, ARIA live regions, `<noscript>`, `prefers-reduced-motion`, WCAG-style checks) are **browser-candidate** material. The runtime is undecided (§9 D-8). What any runtime must achieve are the **outcomes** in §7.0, in particular N-08 (an accessible route to every tree that does not depend on the 3D picture), N-07 (graphics-unavailable behaviour) and N-03 (keyboard-equivalent operation). A Unity or other native runtime would need equivalent mechanisms chosen at spec stage. This dossier does not assume the DOM ones.

The bar itself (best-effort vs WCAG 2.2 AA-targeted vs AA plus real assistive-technology testing) is an **owner decision (§9 D-6)**. The table states what each requirement would be and how it would be checked. WCAG/ARIA references are established standards cited from general knowledge, **not fetched or audited this session**.

| Area | Proposed requirement [P] | Checked by | Does not cover |
|---|---|---|---|
| **Keyboard-complete path** | Every function (select any of the 100 trees, read every inspector field, clear, reset camera, change preset, read banner and provenance) is reachable and operable without a pointer. No keyboard trap. Visible focus. A documented, logical Tab order. | T-11 | Pointer or touch ergonomics. |
| **Screen-reader / ARIA equivalent** | The 3D canvas is a **visual enhancement**, not the only access path. A native, complete `<table>` (or equivalent semantic list) holds all 100 rows: full data ID, `dbhMm`, position, authority labels. The canvas has an accessible name and a description pointing to that table. A polite live region announces selection ("Selected `<ID>`, list position n of 100"). Prefer native elements over ARIA roles. | T-12 | Behaviour on assistive technology not tested. Automated checkers catch only a subset of issues. |
| **Colour-independent cues** | Selection, hover, focus and the read-boundary "below 170 mm" annotation each use a non-colour cue (outline weight/shape, text badge, icon). The banner and availability labels are text. | T-13 | Individual colour-vision deficiencies not simulated. |
| **Contrast** | Text and UI-component contrast against their actual backgrounds, including text drawn over the 3D scene, targets WCAG 1.4.3 (4.5:1 text) and 1.4.11 (3:1 non-text). Colours are not chosen here. | T-14 | Perceived legibility in all lighting. |
| **Reduced motion** | Default has no animation: no auto-orbit, no easing. Under `prefers-reduced-motion: reduce`, camera changes are instantaneous. | T-15 | Vestibular comfort of user-driven orbit. |
| **Cognitive load** | A single purpose; a short fixed control set; the banner in plain language; progressive disclosure of provenance; the list is the always-available simple view; 100 identical markers are differentiated by search and list, not by invented colour. | Human review checklist (T-26) | Learning outcomes. |
| **Zoom / reflow** | (browser-candidate) Text and list reflow at browser zoom and narrow windows without loss (WCAG 1.4.10 intent). H0-A targets desktop browsers only ([C3]), so touch is out of scope unless the owner extends it. | T-23 | Mobile and touch. |
| **WebGL unavailable** | See §3.1. | T-16 | n/a |

### 3.1 Graceful degradation when WebGL is unavailable (browser-candidate; the neutral outcome is §7.0 N-07)

| Situation | Required behaviour [P] | Forbidden |
|---|---|---|
| WebGL context cannot be created at load | A plain-text notice "3D view unavailable in this browser/configuration" **plus** the full accessible list/inspector and banner, all working. | A blank canvas; a silent partial render; a different-fidelity 3D that changes meaning; substituting invented sample data. |
| Context lost mid-session | Notice, retain the selection in the DOM, offer retry if the browser allows. The list stays usable. | Losing the selection silently; auto-reload that discards state without notice. |
| Software/slow renderer detected | State the renderer in the provenance panel (for honest performance reading, §4). | Claiming performance (§4.3). |
| Wish for a 2D fallback picture | Defer to the existing separate 2D H0-A viewer as a **separate artifact**. Do not duplicate or reimplement it inside the 3D page. Whether H0-A may be linked from or shared with 3D is **[U]** and an owner decision (§9 D-7, D-11). | Embedding a second renderer that diverges from H0-A's truth. |
| JavaScript disabled | A `<noscript>` message. No tree data is ever implied. | n/a |

---

## 4. Performance and honesty of presentation (PROPOSAL)

**Scale of the workload [V]:** 100 trees at Year0 ([C1], [C3]). That is small, but this dossier makes **no performance claim**. The reasoning "it's only 100 trees so it will be fast" is an assumption and is not evidence. The environment fields below (browser, WebGL renderer string, and so on) are **browser-candidate**. For another runtime, the equivalent fields are the engine and version, the graphics API, the GPU and driver, the window size, and the power mode.

### 4.1 What must be measured before any performance statement

| Measurement | Method (proposed, not run) | Must be recorded with it |
|---|---|---|
| First complete render | Time from navigation start to the first frame in which all 100 markers are drawn **and** the list is populated and the banner is visible | Cold vs warm cache; browser name/version; OS; GPU and WebGL renderer string (or software renderer); window size; device pixel ratio; power mode |
| Frame-time distribution under a fixed script | A fixed scripted orbit/zoom/hover sequence with per-frame times (median, p95, p99, dropped frames) | Same environment fields; script version/hash |
| Interaction latency | Click to selection-visible; key to inspector-updated | Same |
| Memory | Peak and steady-state | Same |
| Load size | Final package bytes | Build identity |
| Failure rate | Number of runs that failed to render | Run count |

Thresholds are **not set here** (§9 D-9). The owner sets them and the reference machines. The same measurements are required on every environment a claim names.

### 4.2 First-render acceptance (what "good" would look like, with symbolic thresholds)

A first render is **acceptable only if all hold**, with thresholds the owner sets:
1. The banner and provenance panel are visible *before or with* the first tree. Trees never appear without the "static Genesis Year0 fixture" context and the source-identity pin (§6.2).
2. Exactly the document-stated number of markers (100) are drawn. The list shows the same 100 IDs.
3. Zero console errors, zero network requests, zero storage writes (T-06, T-07).
4. Time to first complete render ≤ `T_FIRST_RENDER` (owner-set).
5. Orbit script p95 frame time ≤ `T_FRAME_P95` (owner-set), on the named reference environments only.
6. The measurement record in §4.1 is attached.

### 4.3 What must never be claimed without measurement

| Never claim | Because |
|---|---|
| "60 fps", "smooth", "fast", "real-time" | Needs §4.1 data on a named environment. |
| "Runs in all browsers / on all machines / on low-end hardware" | Only tested environments count. F-5 says no H0-A browser rendering or test has been done ([C3]). |
| "Accessible", "WCAG compliant", "screen-reader friendly" | Needs T-11..T-15 and an audit. Automated checks alone cannot prove it. |
| "Deterministic" or "identical output" | Only the dataset/scene manifest can be identical across machines. Rendered pixels may differ across GPUs and drivers (§6.1). |
| "Offline", "no network", "no data leaves the machine" | Needs N-06 (T-06 in a browser). The no-network rule is itself a 3D **proposal** that follows the H0-A precedent ([C3] F-3). It is not an inherited 2D rule. |
| "Realistic", "accurate forest", "forecast", "growth", "health", "habitat", "Irish" | Not modelled ([C3]). |
| "Measured crown", "canopy", "canopy cover" (for any crown envelope) | The crown radius is a *potential crown* rule output at CI 0, not a measured crown ([V] F-16). |
| "Live" presented as a health or condition claim | `status="live"` is a per-row source token, not health ([V] F-14). |
| "Verified", "qualified", "approved", "secure", "release-ready" | Nothing here is. L4 FAIL38 is UNWAIVED ([C3], [C4]). No implementation exists (F-5). |
| "Same as the 2D view" | Only specific fields can be compared (T-24, §8). |

---

## 5. Technology notes relevant to interaction and acceptance only

This dossier does not choose a runtime. The table below states, for the options named in [C1], what each would change for the interaction, accessibility and test designs above. All of it is **[P]/[U]**. None of it was prototyped. The runtime-neutral outcomes in §7.0 apply to **every** option here. The browser-candidate vectors in §7.1 are only one way to observe them. A choice among these options must not follow from which one is easiest to test (§9 D-8).

| Option | Interaction / picking | Accessibility consequence | Test consequence | Rights/policy note |
|---|---|---|---|---|
| **Browser WebGL (raw)** | Pick by ray-cast, ID-colour buffer or analytic cylinder test | The DOM list/table, ARIA and live regions come naturally and sit beside the canvas | Browser automation can read the DOM, network log and storage. GPU availability in a headless run is **[U]** | Offline single-file packaging is the H0-A pattern ([C3]). Any 3D library would be an external dependency needing approval ([C5] line 34) and in-package vendoring. Vendoring is **[U]**. |
| **Browser Canvas-2D projected "pseudo-3D"** | Analytic picking in 2D | Same as above | Closest to H0-A tooling. | It is not true 3D. Whether it satisfies the owner's "3D" is a scope question (§9 D-1). |
| **Separate Unity prototype** | Unity raycasts | A parallel accessible UI must be built; Unity's own accessibility support for a custom tree list is **[U]** | Unity/Editor/Windows is not authorised in Phase A ([C1]). The §7.0 outcomes (identity, count, refusal, no-write, no-network, accessible route) still apply. The method of observing them (for example no-network and no-write evidence, and the accessible route) would have to be specified for that runtime at spec stage. I do not assert those methods here (**[U]**). | A Unity runtime is a separate decision and lease ([C1] "Later Phase B"). |
| **Other native** | Varies | Varies | Varies | **[U]** |

---

## 6. Determinism and provenance (PROPOSAL)

### 6.1 What can and cannot be deterministic

| Layer | Deterministic? | Proposal [P] |
|---|---|---|
| Source bytes | Yes, by pinning. | One fixture trust root: 32,567 bytes, SHA-256 `bcaaa5c3…57c4` ([V] document-stated, F-18). I did not rehash it. It is reported, not re-derived (§6.1a). |
| Decoded values | Yes (IEEE binary32 decode per [C4], [C6] §2.3). | Reuse the trusted Core path **as a precedent** (H0-A has one numeric codec and no second decoder). Whether 3D reuses it is the owner's decision (§9 D-11). |
| Source seeds | Fixed constants in the fixture, with distinct roles ([V] F-17). | The fixture records `generatorSeed` 6390001 (derivation block) and `simulationSeed` 20261116 (ecology block, `annualCallMade` false). The 3D view consumes **neither** as a random seed and draws no random numbers. Both are carried as provenance only. |
| **Scene manifest** (the ordered list of: data ID, position under the named mapping, geometry-option values used, display-mapping ID/version/factor) | Yes. | Serialise canonically (a fixed byte format, to be specified at spec stage), hash with SHA-256, and show the digest. This is the "byte identity of the rendered dataset". It is a **display derivation** fingerprint. It is not a fourth input identity (§6.1a). |
| Display text | Only if formatting is locale-independent. | Numbers use one fixed, locale-independent formatter. `toLocaleString`-style output varies by machine and breaks text identity (T-25). |
| Random numbers | None in baseline. | No RNG anywhere. If cosmetic variation is ever added, its seed is a fixed documented constant combined with the data ID, recorded in provenance. |
| **Rendered pixels** | **No guarantee.** GPU, driver, antialiasing and rasteriser can differ. | Never assert pixel identity across machines. Screenshot comparison, if used, is tolerance-based on one pinned environment and says so. |

### 6.1a Three identities (fixture trust root, observation identity, package/code integrity root)

r0 listed a "fixture digest" and a "Core trust root hash/size" as two input identities. They are one. The model is now three separate things:

| # | Identity | What it is | Status |
|---|---|---|---|
| 1 | **Fixture trust root** (the only input identity) | The fixture's byte length **32,567** and SHA-256 **`bcaaa5c3…57c4`**, named once. The contract's Core admits only bytes matching this pin, and refuses before JSON parsing otherwise ([V] F-18, [C4], [C6] §2.1). | Document-stated. I did not read or rehash the bytes. A 3D view that consumes a read projection shows this pin as **carried data**. It does not recompute it. |
| 2 | **Observation identity** | The contract's full observation key, `ObservationKeyV1`: a token `h0a-okey-v1:` + a SHA-256 of canonical components. The components include the fixture byte hash, the transform ID, the read-schema version, the stage `Genesis`, year 0, and the contract version. It identifies *this* Genesis frame ([C6] §2.7). | Document-stated. A selection or pick is meaningful only with its observation identity. Whether the 3D view consumes this key or needs its own is open (§9 D-11). I do not type a literal token. The contract says it is computed. |
| 3 | **Package/code integrity root** (a separate **future** item) | The hash of the actual shipped package or code. For H0-A the contract holds it **externally** (a Manager receipt), because a package cannot contain its own hash ([C6] §2.7, §5). | **Does not exist for 3D.** No 3D package or code exists. It is [U] and is not the "Core hash" in the fixture pin. |

The **scene manifest digest** (§6.1) is a fingerprint of the *display derivation*. It is neither of the above and is not an input identity. Field names must keep these apart. In particular, "Core hash" must not be used for the fixture pin, because it could be read as a hash of Core *code*.

### 6.2 Provenance panel and banner

**Banner [P]. Always visible, never dismissible, present in every state (loading, error, fallback, selected).** Proposed wording template (text only, no colour dependence):

> *Synthetic 30×30 m pure-Sitka experiment · Year 0 (Genesis) · static Genesis Year0 fixture · source pin `<byteLength>` B / SHA-256 `<pin prefix…suffix>` · schematic inspect-only view: not a forecast, recommendation, survey or accepted game feature.*

The pin values are **carried from the admitted observation**, not typed as literals in the banner. If the owner picks geometry option G-2 (§1.2.1), the banner adds: "crowns are schematic potential-crown envelopes, not measured crowns". If a display mapping is not true scale, the banner adds the persistent badge of §1.2.1.

**The banner does not carry `status`.** `status` is a per-row life-status token ("Live"), not a dataset status, so a banner reading "source status: live" could be read as "a live, current data feed" or a validated state. The token appears **per tree** (§2.2) as "Life status: Live (source token; not health)".

In the refusal state the banner reads "no valid source loaded" and shows **no** pin value (T-17).

**Provenance panel [P] (collapsible, but its existence and summary line are always present):**

| Field | Value source | Note |
|---|---|---|
| Fixture trust root (identity 1) | `byteLength` and `sha256` carried from the observation (document-stated pin: 32,567 B, `bcaaa5c3…57c4`) | Reported, not re-derived by the 3D view |
| Observation identity (identity 2) | The observation key token | Computed by the read boundary |
| Package/code integrity root (identity 3) | Package hash as published externally | **[U]**. Set at a future packaging step. |
| Fixture ID, catalog version, source class | `p1d-30m-sitka-adult-v1`, `p1-f0-derived-catalog-v1`, `Synthetic` (document-stated, [C6] §2.5) | Carried provenance text and strings |
| Derivation | `kind="designed"`, `generatorSeed` 6390001 | Provenance only (§6.1) |
| Experiment | `simulationSeed` 20261116, `annualCallMade` false, excluded systems list | Provenance only. `annualCallMade` false is **not** evidence of a run. |
| Coordinate mapping | Transform ID `p1-square30-corner-to-centred-xz-v1` as the source's own centred transform, plus the **3D mapping ID/version** if different | §1.2, §9 D-15 |
| Crown rule | `crownRuleId` `potential-crown-at-ci0-linear-f32-v1` | Only if crowns are drawn (G-2) |
| Scene manifest digest | SHA-256 of the canonical manifest (§6.1) | Recomputed independently by the test harness (T-24) |
| Display mapping | Mapping ID, version and factor, or "true scale" | §1.2.1 |
| Build identity | Build ID/version | Set at packaging |
| Renderer | Browser and WebGL renderer string (browser-candidate), or the runtime's equivalent | For honest performance reading |

Computing the scene-manifest digest in a browser: whether the platform crypto API is available when a file is opened from disk is **[U]**. The contract's own Core uses a dependency-free synchronous SHA-256 and avoids `crypto.subtle` for exactly this unverified reason ([C6] §2.3). The 3D alternatives are reusing a shared hashing path or vendoring a pure-JS implementation (a dependency decision, [C5] line 34).

### 6.3 Year status and "unsupported" distinctions without implying unmodelled ecology

The sources contain **no** formal year-status term "eligible". I use these descriptive labels only. "Display-eligible" is a **dossier-local term**: *an owner-approved decision to show that year in 3D exists*. **Today that is true for no year in 3D**, because no 3D scope exists (F-2).

| Year / item | Status per sources | Proposed 3D handling [P] |
|---|---|---|
| Year0 (Genesis) | Approved as the 2D H0-A viewer scope ([C3]). **Not** approved for 3D. | Baseline candidate. The banner says "Year 0 (Genesis)". |
| Year1 | Independently qualified, locally protected numeric continuation (#725/#729/#732, as stated by [C1]; **content not read by me**). Excluded from H0-A ([C3]). | Not in the baseline. If the owner later wants it (§9 D-3), it is a separately sourced immutable dataset with its own banner: **"Historical Year1 - locally qualified numeric continuation; 3D display not yet qualified"**. The label does not say "qualified" alone, because that could be read as covering the 3D view. It is shown as a **static alternative view, never an animated or computed step from Year0**. This is the one place the "no year progression" rule needs careful wording. |
| Year2+ | DRAFT (#760, [C1]) | Not shown. If ever mentioned: "DRAFT, not available". |
| Marking | DRAFT (#761, [C1]) | Not shown and no affordance. |

**Unsupported / hazard / regeneration / canopy distinctions [P].** The packet asks the design to distinguish "unsupported CI/hazard/regeneration" ([C1]). The adopted contract now gives the starting facts ([V] F-19, [C6] §2.4, §2.5). Competition index (`hegyiCi`), neighbour count, mortality pressure and hazard probability are **`NotEvaluated`** (static Genesis, no annual evaluation). `healthScore` and the stand aggregates are **`Unavailable`**. The source also lists eight **excluded systems** (`browsing`, `canopyLight`, `juvenileLifecycle`, `mixedSpecies`, `pruning`, `recruitment`, `regeneration`, `storm`). The fixture's `ecology.excludedSystems` field is the source of that list. I propose to treat these as **Unsupported** in this frame. That mapping is mine **[P]**, and its exact definition follows A1 and the spec. The vocabulary is the §2.2 base vocabulary:

| Class | Meaning | Display rule |
|---|---|---|
| `PresentSourced` | Present in the trusted projection | Shown, with its source label |
| `PresentComputed` | Computed by the **read boundary**, with a named calculation stage (for example the inventory counts and the below-170 mm note) | Shown as the read boundary's value, and **never recomputed by the viewer** |
| `NotEvaluated` | A value the boundary defines but did not evaluate at this stage (for example `hegyiCi`, hazard) | Shown as "Not evaluated" with the boundary's reason. No zero. No bar. No placeholder drawn in the scene |
| `Unavailable` | A value the boundary defines but the trusted source did not supply (for example `healthScore`, stems/ha) | Shown as "Unavailable" with the boundary's reason. Kept **distinct from** `NotEvaluated`. No zero and no health colour |
| Unsupported | A concept the frame or view does not support (for example the excluded systems above) | A single static "Not modelled in this view" disclosure names the categories by text only. Nothing is drawn |
| Blocked | A stage or feature current authority does not allow (Year2+, marking) | Absent, or labelled "DRAFT, not available" |
| Refused | Admission failed | The refusal screen (T-17), with the bounded refusal reason |
| PRESENTATION (layered on top) | A view-only value (mapping factor, list position, camera) | Labelled as a view choice. It never replaces any state above |

**Visual rules that avoid implying ecology [P]:** neutral flat ground; no understorey, grass, soil or leaf-litter imagery; no horizon, sky, sun position or compass; crown geometry only if the owner picks G-2 or G-3 (§1.2.1), and then labelled as a schematic **potential** crown, never a measured or competition-adjusted one; shadows off or labelled "cosmetic, not light modelling" (`canopyLight` is an excluded system); no heat-map or health ramp; no colour keyed to life status; no growth or season cue; no real-place or geographic imagery. This follows the repo rule against inventing ecological facts ([C5] line 43) and the experiment being synthetic ([C1]).

---

## 7. Scoped test and acceptance design — PROPOSED VECTORS, NONE RUN

**None of these were executed, and no result is claimed.** Execution is not authorised in Phase A ([C1]). Each vector needs an approved 3D spec, source lane and runtime first (§9). Tooling is not chosen. Browser-automation packages would be new external dependencies needing approval ([C5] line 34), so a manual procedure is the fallback for every vector. The "does not prove" column is mandatory.

The section has two layers. **§7.0** states the **required outcomes**, which hold for any runtime. **§7.1** is one **browser-candidate** instantiation of those outcomes, using DOM, network-log and storage mechanisms. The choice of runtime (§9 D-8) must not follow from the fact that §7.1 is concrete and a native instantiation is not yet written.

### 7.0 Runtime-neutral required outcomes (N-01 … N-08)

Each row states an outcome and the **observation requirement** that any chosen runtime's spec must meet. The method of observing it is deliberately left open, so no option is favoured. All are **PROPOSED, NOT RUN**.

| ID | Required outcome | Observation requirement (method open) | Browser-candidate vectors in §7.1 |
|---|---|---|---|
| **N-01 Identity** | Exactly the admitted `P30-001` … `P30-100` are present in the scene and in the accessible route. Each scene object maps one-to-one to a data ID. A pick returns a data ID that is valid for the current observation identity. | An independent record of the expected ID set, and an enumeration of every place the 3D view holds identities (scene objects, accessible entries, pick keys). | T-01, T-02, T-03 |
| **N-02 Count and completeness** | Exactly 100 trees are shown. A count mismatch, a missing ID, a duplicate or an extra ID produces a refusal, never a partial forest. | Same, plus a way to present a double with 99 or 101 records through an authorised harness. | T-01, T-17 |
| **N-03 Operation without a pointer** | Every function (select any tree, read every field, clear, reset the camera, read the banner and provenance) is operable with an input device that is not a pointer. No input trap. A visible indication of focus. | A procedure that exercises every function using only that device. | T-11 |
| **N-04 Refusal** | When the source is refused or unsupported, the view shows zero trees, no sample data and no stale geometry under a failed title. It names the bounded refusal reason and shows "no valid source loaded". An unsupported year is refused, not shown as Year0. | An authorised harness that supplies refusal results, incompatible-schema results and unsupported-year results to the view. The **production trust pin is unchanged** (T-17). | T-17 |
| **N-05 No persistent write** | The view performs no write to any persistent store the runtime offers, and creates no file or export. A restart returns the default state and no selection. | A before/after snapshot of whatever persistent stores the runtime has, taken across a full interaction script. | T-07 |
| **N-06 No network use** | The view makes no network request of any kind, and none fails either. | Whatever observation can detect all network use for that runtime, from start through a full interaction script and idle. | T-06 |
| **N-07 Graphics unavailable** | If the graphics path is unavailable or lost, the user is told in plain text. The banner and an accessible route to all 100 trees keep working. The view never shows a blank scene or substitute data. | Disabling or losing the graphics path in the chosen runtime, then checking that route. | T-16 |
| **N-08 Accessible route** | Every tree and every field in §2.2 is reachable through a route that does **not depend on the 3D picture**, and is exposed to the platform's assistive technology. Availability states stay distinct (`NotEvaluated` is not `Unavailable`). Meaning is not carried by colour alone. | At least a structured dump of what the platform exposes to assistive technology, plus a real assistive-technology pass if the owner's accessibility bar requires it (§9 D-6). | T-11, T-12, T-13, T-14, T-23 |

Outcomes the browser vectors cover only partly, for any runtime: the below-170 mm note shows the read boundary's value and the viewer never computes it (T-19); no year progression and no marking affordance (T-08, T-09); no mutation of the admitted data (T-10); the scene-manifest digest is stable (T-04); provenance values match (T-24). These are runtime-independent in intent and need their own neutral wording at spec stage.

### 7.1 Browser-candidate vectors (T-01 … T-26)

These vectors assume a browser DOM, network-log capture, browser storage, `prefers-reduced-motion` and similar mechanisms. They are **one candidate instantiation** of §7.0, not the plan for every runtime.

| ID | Setup | Action | Expected observable result | Does NOT prove |
|---|---|---|---|---|
| **T-01** Count & ID completeness | Trusted Year0 projection loaded; harness holds the independently derived expected ID set | Enumerate: scene instances, DOM list rows, pick keys | Each set has exactly the expected count (100) and equals the source ID set `P30-001` … `P30-100` exactly, in the documented source order: no duplicates, none missing, none extra; instance ↔ ID ↔ row is one-to-one | That the IDs are ecologically meaningful; that the source is correct |
| **T-02** Pick round-trip | Default camera, then each preset | For every tree, programmatically pick at its projected stem centre; also pick empty ground | Each pick returns that tree's own data ID (all 100); empty ground returns none | Mouse accuracy for a human; behaviour under every camera pose |
| **T-03** Ordinal truthfulness | List sort/filter available | Re-sort and filter; select a tree before and after; inspect ARIA names | The ID stays constant; ordinal changes and is labelled "display order"; no code or DOM key uses the ordinal as an ID; labels always carry the full ID | That users will not misread ordinals |
| **T-04** Deterministic manifest | Same package | Load cold twice, on two browsers, on two machines; read the manifest digest | Identical digest everywhere; no pixel assertion | Pixel identity; correctness of the source |
| **T-05** Position fidelity | Harness reads trusted Core output | Compare the scene description to the `xMm`/`yMm` integers under the **declared, named mapping** (§1.2, D-15) before GPU upload | Exact numeric equality to the mapping's definition; no jitter, snap or clamp; the mapping ID/version is the one shown in provenance | On-screen pixel position (GPU/projection); true field geometry |
| **T-06** No network requests | Network log capture (browser devtools/CDP or a policy blocking all egress) from navigation start through a full interaction script | Load, interact with every control, idle, close | Zero requests after the initial document (no fonts, scripts, images, sockets, beacons); no failed requests either | Absence of network in other builds; OS-level traffic; unreachable code paths |
| **T-07** No writes / persistence | Snapshot of cookies, localStorage, sessionStorage, IndexedDB, Cache Storage, URL, downloads before and after | Run the full interaction script, including camera moves and reload | All snapshots unchanged and empty; no download or file-chooser event; no URL-hash change; the reload returns the default pose and no selection | Browser-profile or OS-level writes; absence of write code in dead branches |
| **T-08** No year progression | Banner shows Year 0 | Audit controls; press every key (incl. space, +, ., Enter); run idle ≥ the harness timeout; inspect timers/animation frames that touch data | No year control exists; the banner and data stay Year 0; no timer mutates data | Absence of dormant code |
| **T-09** No marking affordance | Accessibility tree and DOM available | Search roles/labels/strings for mark/fell/thin/remove/"work plan"; try click, double-click, long-press, drag, right-click, Delete, Enter on a selected tree | No such control or string exists; each gesture does only selection or camera work; the data manifest is unchanged | Absence in source; future behaviour |
| **T-10** Interaction does not mutate data | Manifest digest recorded | Run a long pseudo-random interaction script (select, hover, orbit, filter) | The digest is identical before and after; the data object is frozen | Memory safety; that no console-level tampering is possible |
| **T-11** Keyboard-complete | No pointer used | Tab through the full order; select all 100 trees via the list; use Esc, reset, presets, find-by-ID, open provenance | Every function reachable; no trap; visible focus at every stop; the inspector for tree k matches the pointer-selected result | Pointer/touch usability; assistive-technology compatibility |
| **T-12** Screen-reader / ARIA | Accessibility-tree dump plus at least one real screen reader (which one is an owner choice, §9 D-6) | Navigate the table, select a tree, listen to the live region | The canvas has a name and description; the table has a header and 100 data rows; selection is announced with the full ID; no unlabeled controls | Other screen readers; real users; "accessible" generally. Automated checks catch only part of the issues |
| **T-13** Colour independence | Greyscale and forced-colours emulation, plus a colour-vision-deficiency filter | Compare hover, selected, focused and annotated markers | Each state is distinguishable by a non-colour cue; the banner and labels are readable | Every individual colour-vision profile |
| **T-14** Contrast | Computed styles and rendered-pixel sampling over the scene | Measure text and non-text contrast in each state | Text ≥ 4.5:1 and non-text ≥ 3:1, or the owner's stated bar | Perceived legibility under glare; all backgrounds the camera can show |
| **T-15** Reduced motion | OS/browser reduced-motion emulation | Orbit, reset, change preset | Instantaneous camera changes; no auto-motion even without the preference | Vestibular comfort of user-driven orbit |
| **T-16** WebGL unavailable | WebGL disabled, then a forced context loss mid-session | Load; select via the list; reload | A plain notice; banner, table and inspector fully usable; no blank canvas; no uncaught errors; selection retained on loss | Every driver failure mode; a software renderer's performance |
| **T-17a–f** Refusal states reach the viewer through an authorised harness | The **production pin is unchanged**. No alternate production trust seam is added, and the viewer is never given fixture bytes or a different pin. An **authorised harness** supplies *viewer doubles*: complete, immutable, fixed-schema results of these kinds: (a) `Refused` with a no-source reason; (b) `Refused` with an empty-source reason; (c) `Refused` with a truncated-source reason; (d) `Refused` with a digest-mismatch reason; (e) a result with 99 or 101 rows, or a duplicate ID; (f) a result with an out-of-domain coordinate, a non-finite value or a `status` other than `"live"`; plus (g) an **incompatible contract version** and (h) an **unsupported year**. | Present each double to the viewer | A **refusal screen** naming the bounded reason; **zero** markers; no plate with placeholder trees; no sample or demo data; the list empty; the banner shows "no valid source loaded" and no pin; no console exception. An unsupported year is refused, **not** shown as Year0. A 99/101-row or duplicate-ID double is refused, not rendered partially. | Any corruption class at the **byte level**. Byte-level negatives (wrong digest, truncation, tampering, schema negatives) stay in the Core's **own separate test suite**, not in the 3D viewer's tests ([C4], [C6] §6). Nothing here shows what the production Core does with altered bytes. |
| **T-18** Banner present | Walk every state | Load, error, fallback, selection, filter, reset | The banner is visible and non-dismissible in each state; its text matches the template and says "static Genesis Year0 fixture" with the source pin; the banner contains **no** `status` or "live" token; the pin values equal those of the admitted observation | Reader comprehension |
| **T-19** Availability labelling, life-status wording and no viewer-side computation | DOM text audit, plus viewer doubles (as in T-17) | List every inspector field. Grep all text for forbidden wording. Present a double whose `belowActionNote` **contradicts** its `dbhMm` (for example `dbhMm` 100 with `belowActionNote` false, then `dbhMm` 300 with it true). Present doubles with `NotEvaluated` and `Unavailable` fields | Each field carries one of the contract's availability states (or PRESENTATION for view-only values), and `NotEvaluated` and `Unavailable` are visibly distinct; "Life status: Live (source token; not health)" appears per tree and nowhere else; the below-170 mm note equals the double's `belowActionNote` and uses the boundary's `actionNote.comparison` wording, so the **viewer is shown not to compare `dbhMm` itself**; the strings "eligible", "ineligible", thin, remove and health do not appear at the 170 mm boundary ([C4], [C6] §2.6) | That the labels are understood; that the **boundary's** comparison is right (that is the Core's own suite) |
| **T-20** Scale honesty | Display mapping on, then true scale (if provided) | Read the badge and the applied factor; compare marker geometry with `dbhMm`; read the mapping ID, version and factor in provenance and in the scene manifest | The badge shows the factor actually applied; the mapping is a **named, versioned, disclosed field**, not a silent mesh scale or pivot change; true-scale cylinder diameter equals `dbhMm` ÷ 1000 m within tolerance (geometry data, not pixels); if G-2 is chosen, the crown envelope horizontal radius equals `crownRadiusM` (a radius, not a diameter) at the mapping's scale | That the user perceives scale correctly; that A1's own geometry checks accept the mapping |
| **T-21** First-render performance | A named reference environment (§4.1 fields) | Cold-load ×N, record per §4.1 | The record exists and meets owner-set `T_FIRST_RENDER` | Any other environment; sustained use |
| **T-22** Interaction performance | Same | Fixed orbit/hover script | Frame-time distribution recorded; meets owner-set `T_FRAME_P95` | Other hardware; worst-case drivers |
| **T-23** Reflow/zoom | Browser zoom at an extreme and a narrow window | Operate list, inspector, banner | Content reflows with no loss and no two-dimensional scroll trap for text | Touch devices |
| **T-24** Provenance panel | Harness recomputes the scene-manifest digest independently | Compare the displayed values to recomputed ones, and the displayed fixture pin and observation identity to those of the admitted observation | The displayed manifest digest equals the recomputed value. The displayed fixture pin (32,567 B, `bcaaa5c3…57c4`) equals the observation's carried value and the **document-stated** pin. The three identities (§6.1a) are shown as three separate fields | That the fixture bytes match the pin (a byte-level re-hash is the Core's suite or a lawfully authorised custodian, **not** a 3D viewer test); that the fixture is ecologically valid |
| **T-25** Locale independence | Two browser locales | Compare the DOM text of every numeric field | Byte-identical text | Rendering fonts |
| **T-26** No implied ecology (human review checklist) | Review checklist from §6.3 | Inspect all states and presets | No understorey, sky, compass, terrain, health ramps, life-status colours, growth cue or geographic imagery. Crowns appear only if the owner chose G-2 or G-3, and then only labelled as schematic **potential** crowns, never measured. Reviewer name recorded | Anything subjective beyond the checklist. This is a review, not an automated test |

**Coverage map to the required topics** (each row also maps to a runtime-neutral outcome N-01 … N-08 in §7.0): no network → T-06 · no writes/save → T-07 · no year progression → T-08 · no marking → T-09/T-10 · count and ID completeness → T-01/T-02 · accessibility paths → T-11..T-16, T-23 · negative missing/invalid source → T-17a–f · honesty/provenance → T-04, T-18, T-19, T-24 · performance → T-21/T-22.

---

## 8. Comparison with one 2D H0-A view, at interface and provenance level only

Scope caution: this compares only what the **owner-approved receipts and the adopted contract text describe** ([C3], [C4], [C6]). H0-A is not implemented or rendered (F-5), so there is no actual 2D UI to inspect. **NOT VERIFIED:** the four H0-A spec files, any rendered view, and any fixture byte. This dossier does not redesign, edit or depend on #769 and introduces no executable marking or year progression.

| Dimension | 2D H0-A as documented | Proposed 3D counterpart | Gap / risk |
|---|---|---|---|
| Delivery | One self-contained offline HTML/Canvas page, fixture embedded ([V] [C3]) | Would need WebGL or another runtime (undecided, §5) | Packaging and the single-file pattern may not carry over **[U]** |
| Data entry | Core read projection `ccf.h0a.core-read` v1.0.0 ([V] [C4]) | The same projection **only if** the owner approves sharing | The contract is restricted to H0-A, **not** an approved 3D API (F-6). Do not assume reuse. |
| Source trust | Fixed 32,567-byte pinned Core; wrong bytes rejected before parse ([V] [C4], [C6] §2.1). H0-A has one numeric codec and no alternative Core decoder in the Viewer. | Reuse the trusted Core, or a separately qualified projection, or a separate decoder (§9 D-11) | **Precedent and risk, not a prohibition.** The "no second decoder" rule is scoped to the H0-A viewer ([C4]). A separate 3D codec would be a second trust root and would need its own qualification and review. It would also bring a risk that the two codecs diverge. Whether to share or duplicate is the owner's open decision. |
| Identity | All 100 IDs `P30-001` … `P30-100`; full observation identity ([V] [C4], [C6]) | The same data IDs and the observation identity (§6.1a); ordinal ≠ ID (§1.4) | Ordinal or order differences between 2D and 3D are a risk; display order must be declared |
| Selection | Stem-centre selection ([V] [C3]) | Ray/analytic pick returning the data ID | Overlap and occlusion in 3D can make picks ambiguous (T-02) |
| Inspector | Complete accessible inspector/list ([V] [C3]) | The same accessible table is the primary path; 3D is an enhancement | Parity of fields is not verified |
| Navigation | Zoom/pan/fit ([V] [C3]) | Orbit/zoom/reset + a plan preset | 3D adds camera freedom and motion-sensitivity concerns (§3) |
| Life status and Genesis-only truth | Per-row life-status token `"live"` projected as `lifeStatus: "Live"`, "not health"; Genesis-only truth ([V] [C4], [C6]) | Per-tree "Life status: Live (source token; not health)" (§2.2). A permanent banner with "static Genesis Year0 fixture" plus the source pin, and a provenance panel (§6.2) | How the 2D page actually displays the token is **NOT VERIFIED** |
| Annotation | Read-boundary `belowActionNote`, strict `dbhMm < 170`, annotation only, no "eligible" label; the Viewer never compares DBH ([V] [C4], [C6] §2.6) | The same: carry and display the boundary's value (T-19) | An exaggerated marker could visually imply removal or size ranking |
| Geometry | 2D draws stem centres from integer mm and crown circles from `crownRadiusM` as a radius ([V] [C6] §2.6) | Geometry options G-1 … G-3 (§1.2.1) | A 3D crown envelope adds a shape and a vertical extent the source does not carry |
| Excluded | Annual, health, forecast, Year1 toggle, marks, save, network ([V] [C3]) | The same exclusions (§2.3) | A 3D "feel" invites a growth or what-if expectation; the banner must counter it |

---

## 9. Owner decisions (numbered; this dossier does not decide any of them)

| # | Decision | Options | Consequences |
|---|---|---|---|
| **D-1** | Does a Multi 3D view exist, and in what role? ([C1] gate) | (a) No 3D for now; (b) separate non-public prototype; (c) part of a later simulator | (a) closes this track. (b) is the least-coupled option and needs its own spec, physical lane and rights. (c) couples to unspecified simulator work. None can start without the four-file spec and G1/G2 ([C1]). |
| **D-2** | Interaction scope | (a) Inspect-only per §2; (b) (a) + find-by-ID/filter; (c) anything beyond (marking, year progression) | (a)/(b) need no new truth. (c) is **not** within this dossier: it depends on #760/#761 drafts being approved and changes the non-feature list, T-08, T-09 and the security surface. |
| **D-3** | Time scope | (a) Year0 only; (b) Year0 plus Year1 as a separate static dataset | (b) needs a second banner and dataset identity, local-only rights over the Year1 source and fresh qualification. Its label would be "Historical Year1 - locally qualified numeric continuation; 3D display not yet qualified". It must remain "static alternative", never "progression". (a) is the smallest match to the H0-A scope. |
| **D-4** | Fidelity tier / baseline geometry (§1.2.1) | (a) **G-1 markers only**: DBH cylinders at a uniform, labelled marker height, no crown; (b) **G-2 schematic height + crown envelopes** from the sourced `heightM` and `crownRadiusM`; (c) **G-3 richer art** | **The adopted contract carries height and crown radius** ([V] F-16). The question is not whether data exists. (a) invents nothing but discards the sourced height and radius. (b) uses sourced values, but the crown is a **potential crown** under `potential-crown-at-ci0-linear-f32-v1`, not a measured crown. Its shape, vertical extent and base are display choices, because the row carries none. It must be disclosed as schematic, and overlap must not read as a competition outcome. (c) adds mostly unsourced detail and risks the implied-ecology problem ([C5] line 43). Q's review also records that A1 proposes (b). I have not read A1, and this decision stays the owner's. |
| **D-5** | Scale policy | (a) Declared exaggeration as a **named, versioned display-mapping field** with a badge; (b) the same plus a true-scale toggle; (c) true scale only | (a) is legible, but the mapping must be disclosed, named, versioned and part of the scene manifest, and it must not be a silent mesh scale. Q's review notes that A1's W-06 would have to accept it explicitly. (c) is honest but nearly invisible at 30 m for thin stems. The factor is not set here. |
| **D-6** | Accessibility bar | (a) Best effort; (b) WCAG 2.2 AA-targeted with an audit; (c) AA plus testing with a named real screen reader and keyboard-only users | Cost rises (a)→(c). Only (b)/(c) may justify the word "accessible". T-11..T-16 and T-23 map to this. |
| **D-7** | WebGL-unavailable policy | (a) Notice + accessible table; (b) (a) + link to the separate 2D H0-A; (c) refuse entirely | (a) is the proposed minimum. (b) depends on H0-A being implemented and approved (F-5). (c) removes the accessible path. |
| **D-8** | Runtime (implications only) | Browser WebGL / Canvas pseudo-3D / Unity / other | Browser: DOM accessibility is natural, offline single-file is the H0-A pattern ([C3]), and any library is a dependency needing approval. Unity: a parallel accessible UI is needed, Editor/Windows and a lease are required, and accessibility support is **[U]**. **This decision must not follow from test convenience.** The browser vectors in §7.1 are concrete only because they are written first. Every option must meet the same runtime-neutral outcomes (§7.0 N-01 … N-08), and a native option's observation methods are written at spec stage. This is coordinated with A1's options, which I have not read. |
| **D-9** | Performance thresholds and reference machines | Owner sets `T_FIRST_RENDER`, `T_FRAME_P95` and the named environments | Without them T-21/T-22 cannot pass or fail, and no performance claim may be made. |
| **D-10** | Determinism claim level | (a) Dataset/manifest identity only; (b) (a) + tolerance-based screenshots on one pinned environment | Pixel identity across machines should not be promised (§6.1). |
| **D-11** | Share vs duplicate the H0-A read boundary ([C1] gate) | (a) Reuse a frozen, authorised read artifact; (b) a separately qualified 3D projection; (c) a separate decoder; (d) a live API (a new scope, not proposed here). This follows the option framing in Q's review §7 item 4. | (a) keeps one trust root but extends a contract limited to H0-A ([C4]), so it needs an explicit approval. (b) and (c) create a second trust root with fresh qualification, review and test needs, and a divergence risk. The H0-A "no second decoder" rule is **precedent and risk**, not a 3D prohibition (§8). |
| **D-12** | Availability / status vocabulary and year-status words | Adopt the §2.2 base vocabulary (the contract's four states plus Unsupported / Blocked / Refused, with PRESENTATION on top), or an owner-chosen set | The contract already fixes the four availability states, so the base is not an invention of this dossier. The sources define no year-level "eligible" term, and [C4] bans "eligible" at the 170 mm boundary. A single owner-approved vocabulary avoids confusion. |
| **D-13** | Camera complexity | Plan + oblique + orbit (proposed), or plan-only, or fully free | Less freedom lowers cognitive and motion load and the number of tests. |
| **D-14** | Test tooling | Manual procedure only, or scripted browser automation | Scripted tools are new dependencies needing approval ([C5] line 34), and GPU availability for headless runs is **[U]**. |
| **D-15** | 3D coordinate mapping (and axis, handedness, up axis, pivot) | (a) Integer `xMm`/`yMm` ÷ 1000 under a **new, named, versioned 3D mapping**; (b) the centred f32 values per the transform ID `p1-square30-corner-to-centred-xz-v1`; (c) another named mapping | The projection schema, the extent, the integer-mm fields and the transform are **public adopted documentation**, so no read grant is needed to know them ([V] F-15). A grant is needed only to read the **bytes** (F-10). The real decision is which mapping to use. Whatever is chosen must cite a mapping or transform **ID** and must not use the field name `unityX/unityZ`. Those fields establish no Unity frame, handedness or engine meaning (§1.2). 3D handedness, up axis and pivot remain **[U]** until the spec states them. |

---

## 10. Not verified (explicit)

- Nothing was built, run, rendered or tested, and no browser, Unity, network or test tool was used.
- I did **not** read any fixture or fixture bytes, P0/#710/#725/#687 source, #769 file, the four H0-A spec files, #756/#757 or A1. I **did** read (r1) the public adopted contract documents [C4] and [C6], and Q's review [C7]. Everything taken from [C6] is **document-stated**. I did not check it against the fixture bytes and I did not rehash anything. Whether the bytes agree with [C6] is NOT VERIFIED.
- Still **[U]** after r1: the text of the fixture's `crownRule`, `disclosure` and other free-text fields; any crown base, length or shape (the row carries none); the 3D mapping, handedness, up axis and pivot; the exact definitions of Unsupported / Blocked / Refused (they follow A1 and the spec); the package/code integrity root (it does not exist for 3D); and every runtime-specific observation method for a non-browser option.
- Issues #725, #729, #732, #760 and #761 were **not opened**. Their status is taken only from the packet text.
- WCAG/ARIA/WebGL references are established standards cited from general knowledge, **not fetched or audited**.
- Whether `crypto.subtle` is usable from a local file, how WebGL behaves headless on the reference machine, and any Unity accessibility capability are **[U]**.
- The actor's harness label and the exact effort level beyond "session low" are not verified. No dashboard/Drive sync is claimed.

---

## Citations (sources actually used)

- **[C1]** Corrected binding #770 packet (commit `894129616802daf7b647d48734c03adf2c700ae6`): https://github.com/jermur788/CCF/blob/docs/multi-730-770-corrected-tg-fork-3d-sim-trial-20261118-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-770-DEEPSEEK-TG-FORK-MULTI-3D-SIMULATOR-REAL-WORK-ORCHESTRATION-PACKET-20261118.md (read in full; also source of the packet's statements about #725/#729/#732/#760/#761/#766/#769, taken as stated, not opened).
- **[C2]** Manager #730 Phase A adoption comment: https://github.com/jermur788/CCF/issues/770#issuecomment-6988856214 ; #770 issue body and thread (read; the thread includes the A2 claim https://github.com/jermur788/CCF/issues/770#issuecomment-6989030661).
- **[C3]** Multi #750 owner H0-A choice and four-file spec approval receipt (branch tip `a9579188d1e527f4e94d476c0e02e4285bab0035`): https://github.com/jermur788/CCF/blob/docs/multi-750-h0-a-owner-approved-four-file-spec-20261118-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-750-OWNER-H0-A-CHOICE-AND-FOUR-FILE-SPEC-APPROVAL-RECEIPT-20261118.md (read in full).
- **[C4]** Multi #758 contract verdict and owner local-only JS security approval, **R-758A** (Manager adoption, pinned `a7e9228b8429e6b51c1fcb493e02af0a4148452a`): https://github.com/jermur788/CCF/blob/a7e9228b8429e6b51c1fcb493e02af0a4148452a/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-730-H0-A-758-CONTRACT-VERDICT-AND-OWNER-LOCAL-ONLY-JS-SECURITY-APPROVAL-20261118.md (read in full; r0 cited the branch URL of the same tip).
- **[C5]** Repository rules `AGENTS.md` (lines 34 and 43) at base commit `ff03290a5ef9c53472618aba32591e5822c663e1`.
- **[C6]** Multi #758 complete corrected contract text, **R-758C** (`ccf.h0a.core-read` v1.0.0; its header says "DRAFT", and R-758A adopts that text; pinned `7af3add1d23453a92925a404d7dad5899ab0ff7d`): https://github.com/jermur788/CCF/blob/7af3add1d23453a92925a404d7dad5899ab0ff7d/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-758-H0-A-TARGETED-CONTRACT-CORRECTIONS-DRAFT-RESULT-20261118.md (read in full in r1. Section numbers cited as "[C6] §n" refer to this document. Public Markdown, not a fixture. Its fixture values are reports by the #756 author, not re-read by #758 or by me).
- **[C7]** Q independent review of A1/A2, commit `52b010d754ec12e39998e08a8f53c944e43277d6` (read in full in r1 as the work order): https://github.com/jermur788/CCF/blob/52b010d754ec12e39998e08a8f53c944e43277d6/Docs/Handoffs/MultiRack-Simulation-20261116/770-orchestration/Multi-770-Q-INDEPENDENT-3D-ARCHITECTURE-AND-BOUNDARY-REVIEW-20261118.md
- External standards (general knowledge, not fetched): W3C WCAG 2.x success criteria 1.4.3, 1.4.10, 1.4.11 and 2.1.4; WAI-ARIA native-element-first guidance; WebGL context-loss behaviour.
