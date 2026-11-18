# Multi #770 A2 — 3D interaction, accessibility, visual acceptance and scoped test design dossier

**18 November 2026 · Phase A, GitHub docs-only · lane A2 · author: Claude Sonnet 5.5 (`claude-sonnet-5-5`) via Claude Code in T3 Code, effort as set by the session (low); harness label NOT VERIFIED.**
**Claim:** https://github.com/jermur788/CCF/issues/770#issuecomment-6989030661 · **Branch:** `docs/multi-770-a2-3d-interaction-acceptance-dossier-20261118-v1` · **Base:** `ff03290a5ef9c53472618aba32591e5822c663e1`

> **STATUS: PROPOSAL / DRAFT FOR OWNER DECISION. NOT AN APPROVED SPECIFICATION.** There is no adopted Multi 3D scene or feature specification, no 3D physical source lane and no validated 3D runtime ([C1], "Task intent"). A proposed 3D view is not an accepted game feature. Nothing here was built, run, rendered or tested. Every test in §7 is a **PROPOSED vector, NOT RUN**. This dossier recommends; it decides no technology, scope, fidelity, runtime or policy (§9 lists the decisions it leaves open). It is not a PASS of anything and it is not self-approved. A1 (architecture/truth boundary) and Q (independent review) are separate lanes. I have not read their files.

---

## 0. Evidence key and what this dossier stands on

| Tag | Meaning |
|---|---|
| **[V]** | Verified: stated in a cited document that I read this session. The citation is given. |
| **[M]** | Manager/owner decision, quoted from a cited document. |
| **[P]** | Proposal by this dossier. Not a decision. |
| **[U]** | UNKNOWN / NOT VERIFIED. I did not see evidence for it. Do not treat it as true. |

**Facts I rely on (all read first-hand):**

| # | Fact | Tag | Source |
|---|---|---|---|
| F-1 | The Multi reference is a synthetic 30×30 m pure-Sitka experiment with 100 rows at Year0. It has an independently qualified, locally protected Year1 numeric continuation. Year2 is a DRAFT. | [V] | [C1] "Task intent" |
| F-2 | No adopted Multi 3D scene/feature specification, 3D physical source lane or validated runtime exists. Phase A may recommend but not decide technology, scope, fidelity, runtime or policy. | [V][M] | [C1], [C2] |
| F-3 | The 2D H0-A viewer is an owner-approved, non-public, self-contained offline HTML/Canvas Year0 viewer with the verified fixture embedded. It has no runtime fetch, server, file chooser, upload, CDN, telemetry, localStorage or saved forest. It targets current desktop browsers. | [V][M] | [C3] |
| F-4 | H0-A scope is frozen synthetic 30×30 m / 100 Sitka, static `Genesis` Year0 only. It has zoom/pan/fit, stem-centre selection and a complete accessible inspector/list. It has no annual computation, health claims, Irish forecast, Unity parity, Year1 toggle, marks/Work Plans/Fell, history, save or network. | [V][M] | [C3] |
| F-5 | **No implementation, browser rendering, build or test of H0-A has been done.** The 65 requirements are unverified. | [V] | [C3] "NOT DONE" |
| F-6 | `ccf.h0a.core-read` v1.0.0 is approved as the local H0-A implementation interface, **restricted to the H0-A four-file spec**. It is not approval of Year1/Year2/marking/history APIs. | [V][M] | [C4] |
| F-7 | Contract content cited: one production `CcfYear0Core` with a fixed 32,567-byte / pinned-SHA-256 trust root. Wrong bytes are rejected before JSON parse. No alternative Core decoder in the Viewer. Genesis-only truth. All 100 IDs. Source `status="live"`. binary32 codec and full observation identity. Strict `dbhMm < 170` is an **annotation only**, with **no "eligible" label at the boundary**. No annual/mark/save functions. | [V] | [C4] |
| F-8 | The H0 owner choice is private offline local evaluation only. The public CCF GitHub may receive only safe docs, claims, verdicts and hashes, never viewer HTML/JS/fixture copies unless separately authorised. L4 FAIL38 is UNWAIVED. | [V][M] | [C3], [C4], [C1] |
| F-9 | Year1 (#725 source, #729 review, #732 native qualification), Year2+ (#760 DRAFT) and marking (#761 DRAFT) exist as named issues. **I did not open or read them.** Their status here is as the packet states it. | [V] (packet text only) | [C1] |
| F-10 | The existing local H0 source lanes are solely for 2D H0-A. No 3D physical lane or source claim is available. | [V] | [C1] (quoting the #766 report) |
| F-11 | Repo rules: do not invent ecological facts; do not add packages or external dependencies without approval. | [V] | [C5] `AGENTS.md` lines 34 and 43 at the base SHA |

**Deliberately NOT read or used (prohibited or out of lane):** fixtures, protected P0 trees, #687/#710/#725 source, Scenario One #718/#690/#652/#763/#765, **any #769 file**, the #750 four spec files, the full #758 DRAFT result, #756/#757, and the A1/Q dossiers. Where a fact would need those I write **[U]**.

---

## 1. Scene, camera, tree positioning and ID mapping (PROPOSAL)

### 1.1 Governing rule

**[P]** The 3D scene is a pure, deterministic function of the already-trusted Year0 read projection. The scene never owns, computes or invents biology. If a value the scene needs is absent from the projection, the view refuses (§7 T-17) or omits that visual element. It never fills the gap with a plausible guess. Whether the 3D view consumes the *same* read projection as H0-A is an owner decision (§9 D-11). `ccf.h0a.core-read` v1.0.0 is **not** an approved 3D API ([C4], F-6).

### 1.2 Positioning the 100 identities

| Element | Proposal [P] | Known / UNKNOWN | Truthfulness rule |
|---|---|---|---|
| Source of positions | Each tree's horizontal position is the source stem-centre coordinate in source units, passed through unchanged. | [V] H0-A selects by stem-centre ([C3]). **[U]** field names, units, origin corner, axis orientation and whether the extent is closed or half-open. All are **[U]** until someone reads the projection schema at G1. | No coordinate is generated, jittered, snapped, "spread out" or relaxed to avoid overlap. Overlap is data. |
| Tree count | Exactly the number of records in the projection. Expected 100 ([V] F-1, F-4). | 100 is verified for Year0. | If count ≠ the source-declared count, refuse. Do not pad or truncate. |
| Ground plate | A flat neutral plate sized to the source-declared extent (30×30 m, [V] F-1). It is labelled "schematic ground plane, not a survey". | **[U]** that the projection declares an extent or origin. | Draw the plate only if the extent and origin come from the source. Otherwise show no metric plate and no metric scale. |
| Elevation / terrain | Flat. | No terrain data is known. | Do not add slope, relief, soil, horizon, sky photograph, compass or geographic marker. The experiment is synthetic with no geographic truth ([C1]). |
| Axis mapping | Source horizontal axes map to scene horizontal axes by one declared, tested mapping. Source handedness is not assumed. | **[U]** | The mapping is shown in the provenance panel (§6). Do not label an axis "north" unless the source states orientation. |
| Out-of-extent | Any position outside the source extent means refuse. | **[U]** boundary convention | No clamping. |
| Species | Scene-level caption "synthetic pure-Sitka experiment" ([V] F-1, F-4). A per-tree species label appears only if the projection carries one. | **[U]** per-tree field | No second species is ever drawn. |
| Stem marker | A vertical cylinder at the position, whose **diameter is the source `dbhMm`** ([V] the field exists, [C4]). | The cylinder height is not a datum. **[U]** whether the projection has a height. | If a height is not in the source, use one fixed marker height and label it "marker height, not tree height". |
| Crown / canopy | **Not drawn in the baseline.** | A crown would need height/crown data and an allometric rule. Neither is known and none may be invented ([C5] line 43). | A schematic crown is a separate owner-gated fidelity tier (§9 D-4). |

**Scale honesty [P].** A DBH under 170 mm (that value is a verified annotation boundary, [C4]) is under 0.17 m. In a 30 m plate it is a hairline, nearly invisible. Two honest options exist: a **declared exaggeration** (a persistent "diameters exaggerated ×k" badge showing the factor actually applied) or a **true-scale toggle**. Silent exaggeration is a misrepresentation. The factor k is not chosen here (§9 D-5).

### 1.3 Camera

| Aspect | Proposal [P] | Why |
|---|---|---|
| Default pose | A deterministic fit-all pose computed only from the source extent (no stored or random pose). | Reproducible first render; fits the "fit" action H0-A already has ([V] F-4). |
| Presets | (a) **Plan** (top-down, axis-aligned, comparable to a 2D plan view); (b) **Oblique** (one fixed elevated angle). Both are view-only. | The plan preset is the easiest cross-check against the 2D view. |
| Free navigation | Orbit + zoom about the plate centre, with min/max distance derived from the extent. No fly-through, first-person, walk, VR, auto-orbit or auto-tour in the baseline. | Cognitive load, motion sensitivity and avoiding a "forest walk" that implies ground-level realism. |
| Persistence | Camera pose is volatile view state. It is never written anywhere. | F-3 forbids localStorage/saved state ([C3]). |
| Clip planes | Derived from the extent so no tree is clipped in any permitted pose. | Completeness: all 100 must be reachable and visible. |
| Reset | One always-available reset to the default pose (button and key). | Recoverability for keyboard and low-vision users. |

### 1.4 Identity mapping: data ID ↔ stable visual identity ↔ ordinal

**[P] Three distinct things must never be conflated:**

| Concept | Definition | Authoritative? | Where it appears |
|---|---|---|---|
| **Data ID** | The tree identifier verbatim from the source projection. All 100 are required ([V] F-7). The format is **[U]**. | **Yes.** It is the only identity. | Inspector, list, ARIA names, test oracles, provenance. It is never truncated, reformatted or renumbered. |
| **Visual identity** | The scene instance (mesh instance index and pick key) bound to exactly one data ID. | No. It is derived. | Internal only. Never user-visible. Never used as an ID in the DOM, URL or logs. |
| **Ordinal** | The 1-based position in the current display order, e.g. "list position 17 of 100". | No. It is a presentation convenience. | Always shown **together with** the data ID and labelled "display order, not an ID". |

**Mapping rules [P]:**
1. **Canonical order.** The scene builder takes records in the order the trusted Core projection emits them if that order is a documented guarantee (**[U]**). Otherwise it sorts by raw data-ID bytes. The rule chosen is recorded in the provenance panel and in the scene-manifest digest (§6). No sort may depend on screen position, camera or randomness.
2. **Bijection.** data ID ↔ instance index ↔ DOM list row ↔ pick key is one-to-one and onto the 100 records (test T-01, T-02).
3. **No cosmetic variation in the baseline.** Every marker is identical apart from position and source diameter. Per-tree tint or rotation variety would read as biological difference. If the owner later wants cosmetic variation, it must be a pure function of (fixed constant, data ID), recorded in provenance, and labelled cosmetic.
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
| Keyboard focus | n/a | Tab reaches: skip link, status banner, list/table, inspector, viewport, camera controls, in a documented order | Visible focus indicator that does not rely on colour alone | Focus only |
| Clear | Click empty ground | Esc | Selection cleared | Selected ID |
| Camera | Drag-orbit, wheel-zoom | Arrow keys orbit, +/- zoom, Home/R reset (active only while the viewport is focused) | Pose changes | Camera pose (volatile) |
| Find by ID (optional) | n/a | Text box filtering the list by an ID substring | List narrowed. The count is shown as "k of 100 shown". | Filter text (volatile) |

Single-character shortcuts are active only when the viewport has focus and must not collide with screen-reader quick keys (WCAG 2.1.4; established standard, cited from general knowledge, not fetched this session).

### 2.2 What is displayed per tree, and its authority label

**[P]** The inspector shows **only fields the trusted projection actually carries**. A missing field is omitted or displayed as "not provided". It is never shown as 0, blank-looking-like-zero or a default. H0-A already distinguishes typed numeric availability ([C3], "F1 typed numeric/availability"). The 3D inspector must preserve that distinction **[V]** that it exists, **[U]** how it is represented.

| Inspector row | Shown value | Authority label | Notes |
|---|---|---|---|
| Data ID | Verbatim | **SOURCE** | Verified field ([C4]). |
| Diameter at breast height | `dbhMm` decoded from binary32, in mm, fixed locale-independent formatting | **SOURCE** | Units mm ([C4] field name). Do not convert silently. |
| Position | Source coordinates and units | **SOURCE** | Units/frame **[U]**. |
| "dbhMm < 170 mm" marker | A text badge "below 170 mm DBH (annotation only)" if shown at all | **DERIVED** (strict comparison on a SOURCE value) | [C4] requires annotation only and no "eligible" wording at the boundary. It must not read as a treatment recommendation, health state or removal flag. |
| List position | "n of 100 (display order)" | **PRESENTATION** | Not an ID. |
| Marker diameter factor | "×k exaggeration" or "true scale" | **PRESENTATION** | Always visible when ≠ true scale. |
| Camera/zoom state | Optional | **PRESENTATION** | Not a datum. |
| Anything else (height, crown, growth, health, competition, regeneration, hazard) | **Not shown** unless the projection carries it, and then labelled by its source | n/a | Unsupported concepts are not shown as empty or zero fields (§6.3). |

**Label vocabulary [P]:** **SOURCE** (verbatim from trusted fixture bytes) · **DERIVED** (computed by the viewer; the formula is named) · **PRESENTATION** (view choice only) · **NOT PROVIDED** (the projection lacks it). Each label is a text prefix plus a shape or icon, so it is not colour-only (§3).

### 2.3 Explicit non-features (the view must not offer any of these)

| Non-feature | Why |
|---|---|
| Marking, Work Plans, Fell, thin, "select for removal" | #761 marking is a DRAFT ([C1]). H0-A excludes marks ([C3]). |
| Year progression, play/step/advance, growth animation, year slider | Year2+ is DRAFT (#760). No annual computation in H0-A ([C3]). |
| Any treatment, what-if, forecast, score, recommendation, productivity or habitat figure | No ecology is modelled in this view ([C5] line 43; [C3] "no health claims, Irish forecast"). |
| Save, load, export, import, file chooser, drag-drop upload, download, screenshot-to-disk, clipboard export | F-3: no saved forest or upload ([C3]). A download is a write. |
| Persistence: cookie, localStorage, sessionStorage, IndexedDB, Cache Storage, URL-hash state | F-3 ([C3]). |
| Network: fetch, XHR, WebSocket, beacon, CDN, web fonts, telemetry | F-3 ([C3]). |
| Editing values, dragging or moving trees, hiding trees | Pick is read-only. The data object is deep-frozen **[P]**. |
| Health, risk or "good/bad" colour ramps; heat-maps | Would imply modelled ecology. |
| Arbitrary data input | H0-A scope ([C3]). |

---

## 3. Accessibility (PROPOSAL)

The bar itself (best-effort vs WCAG 2.2 AA-targeted vs AA plus real assistive-technology testing) is an **owner decision (§9 D-6)**. The table states what each requirement would be and how it would be checked. WCAG/ARIA references are established standards cited from general knowledge, **not fetched or audited this session**.

| Area | Proposed requirement [P] | Checked by | Does not cover |
|---|---|---|---|
| **Keyboard-complete path** | Every function (select any of the 100 trees, read every inspector field, clear, reset camera, change preset, read banner and provenance) is reachable and operable without a pointer. No keyboard trap. Visible focus. A documented, logical Tab order. | T-11 | Pointer or touch ergonomics. |
| **Screen-reader / ARIA equivalent** | The 3D canvas is a **visual enhancement**, not the only access path. A native, complete `<table>` (or equivalent semantic list) holds all 100 rows: full data ID, `dbhMm`, position, authority labels. The canvas has an accessible name and a description pointing to that table. A polite live region announces selection ("Selected `<ID>`, list position n of 100"). Prefer native elements over ARIA roles. | T-12 | Behaviour on assistive technology not tested. Automated checkers catch only a subset of issues. |
| **Colour-independent cues** | Selection, hover, focus and the "<170 mm" annotation each use a non-colour cue (outline weight/shape, text badge, icon). The status banner and authority labels are text. | T-13 | Individual colour-vision deficiencies not simulated. |
| **Contrast** | Text and UI-component contrast against their actual backgrounds, including text drawn over the 3D scene, targets WCAG 1.4.3 (4.5:1 text) and 1.4.11 (3:1 non-text). Colours are not chosen here. | T-14 | Perceived legibility in all lighting. |
| **Reduced motion** | Default has no animation: no auto-orbit, no easing. Under `prefers-reduced-motion: reduce`, camera changes are instantaneous. | T-15 | Vestibular comfort of user-driven orbit. |
| **Cognitive load** | A single purpose; a short fixed control set; the banner in plain language; progressive disclosure of provenance; the list is the always-available simple view; 100 identical markers are differentiated by search and list, not by invented colour. | Human review checklist (T-26) | Learning outcomes. |
| **Zoom / reflow** | Text and list reflow at browser zoom and narrow windows without loss (WCAG 1.4.10 intent). H0-A targets desktop browsers only ([C3]), so touch is out of scope unless the owner extends it. | T-23 | Mobile and touch. |
| **WebGL unavailable** | See §3.1. | T-16 | n/a |

### 3.1 Graceful degradation when WebGL is unavailable

| Situation | Required behaviour [P] | Forbidden |
|---|---|---|
| WebGL context cannot be created at load | A plain-text notice "3D view unavailable in this browser/configuration" **plus** the full accessible list/inspector and banner, all working. | A blank canvas; a silent partial render; a different-fidelity 3D that changes meaning; substituting invented sample data. |
| Context lost mid-session | Notice, retain the selection in the DOM, offer retry if the browser allows. The list stays usable. | Losing the selection silently; auto-reload that discards state without notice. |
| Software/slow renderer detected | State the renderer in the provenance panel (for honest performance reading, §4). | Claiming performance (§4.3). |
| Wish for a 2D fallback picture | Defer to the existing separate 2D H0-A viewer as a **separate artifact**. Do not duplicate or reimplement it inside the 3D page. Whether H0-A may be linked from or shared with 3D is **[U]** and an owner decision (§9 D-7, D-11). | Embedding a second renderer that diverges from H0-A's truth. |
| JavaScript disabled | A `<noscript>` message. No tree data is ever implied. | n/a |

---

## 4. Performance and honesty of presentation (PROPOSAL)

**Scale of the workload [V]:** 100 trees at Year0 ([C1], [C3]). That is small, but this dossier makes **no performance claim**. The reasoning "it's only 100 trees so it will be fast" is an assumption and is not evidence.

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
1. The status banner and provenance panel are visible *before or with* the first tree. Trees never appear without their source-status context.
2. Exactly the verified number of markers (100) are drawn. The list shows the same 100 IDs.
3. Zero console errors, zero network requests, zero storage writes (T-06, T-07).
4. Time to first complete render ≤ `T_FIRST_RENDER` (owner-set).
5. Orbit script p95 frame time ≤ `T_FRAME_P95` (owner-set), on the named reference environments only.
6. The measurement record in §4.1 is attached.

### 4.3 What must never be claimed without measurement

| Never claim | Because |
|---|---|
| "60 fps", "smooth", "fast", "real-time" | Needs §4.1 data on a named environment. |
| "Runs in all browsers / on all machines / on low-end hardware" | Only tested environments count. F-3 says no browser has passed ([C3]). |
| "Accessible", "WCAG compliant", "screen-reader friendly" | Needs T-11..T-15 and an audit. Automated checks alone cannot prove it. |
| "Deterministic" or "identical output" | Only the dataset/scene manifest can be identical across machines. Rendered pixels may differ across GPUs and drivers (§6.1). |
| "Offline", "no network", "no data leaves the machine" | Needs T-06. |
| "Realistic", "accurate forest", "forecast", "growth", "health", "habitat", "Irish" | Not modelled ([C3]). |
| "Verified", "qualified", "approved", "secure", "release-ready" | Nothing here is. L4 FAIL38 is UNWAIVED ([C3], [C4]). No implementation exists (F-5). |
| "Same as the 2D view" | Only specific fields can be compared (T-24, §8). |

---

## 5. Technology notes relevant to interaction and acceptance only

This dossier does not choose a runtime. The table below states, for the options named in [C1], what each would change for the interaction, accessibility and test designs above. All of it is **[P]/[U]**. None of it was prototyped.

| Option | Interaction / picking | Accessibility consequence | Test consequence | Rights/policy note |
|---|---|---|---|---|
| **Browser WebGL (raw)** | Pick by ray-cast, ID-colour buffer or analytic cylinder test | The DOM list/table, ARIA and live regions come naturally and sit beside the canvas | Browser automation can read the DOM, network log and storage. GPU availability in a headless run is **[U]** | Offline single-file packaging is the H0-A pattern ([C3]). Any 3D library would be an external dependency needing approval ([C5] line 34) and in-package vendoring. Vendoring is **[U]**. |
| **Browser Canvas-2D projected "pseudo-3D"** | Analytic picking in 2D | Same as above | Closest to H0-A tooling. | It is not true 3D. Whether it satisfies the owner's "3D" is a scope question (§9 D-1). |
| **Separate Unity prototype** | Unity raycasts | A parallel accessible UI must be built; Unity's own accessibility support for a custom tree list is **[U]** | Unity/Editor/Windows is not authorised in Phase A ([C1]). Network and storage observation differs. | A Unity runtime is a separate decision and lease ([C1] "Later Phase B"). |
| **Other native** | Varies | Varies | Varies | **[U]** |

---

## 6. Determinism and provenance (PROPOSAL)

### 6.1 What can and cannot be deterministic

| Layer | Deterministic? | Proposal [P] |
|---|---|---|
| Source bytes | Yes, by pinning. | The embedded fixture is identified by a digest set at G0 packaging ([C3] "G0 final technical packaging"). The actual value is **[U]** and is not invented here. The H0-A Core has a fixed 32,567-byte / pinned-SHA-256 trust root ([V], [C4]). |
| Decoded values | Yes (IEEE binary32 decode per [C4]). | Reuse the trusted Core path. Do **not** write a second decoder ([C4] F-7). |
| **Scene manifest** (the ordered list of: data ID, position, diameter, applied exaggeration factor) | Yes. | Serialise canonically (a fixed byte format, to be specified at spec stage), hash with SHA-256, and show the digest. This is the "byte identity of the rendered dataset". |
| Display text | Only if formatting is locale-independent. | Numbers use one fixed, locale-independent formatter. `toLocaleString`-style output varies by machine and breaks text identity (T-25). |
| Random numbers | None in baseline. | No RNG anywhere. If cosmetic variation is ever added, its seed is a fixed documented constant combined with the data ID, recorded in provenance. |
| **Rendered pixels** | **No guarantee.** GPU, driver, antialiasing and rasteriser can differ. | Never assert pixel identity across machines. Screenshot comparison, if used, is tolerance-based on one pinned environment and says so. |

### 6.2 Provenance panel and source-status banner

**Banner [P]. Always visible, never dismissible, present in every state (loading, error, fallback, selected).** Proposed wording template (text only, no colour dependence):

> *Synthetic 30×30 m pure-Sitka experiment · Year 0 (Genesis) · source status: `<value verbatim from source>` · schematic inspect-only view: not a forecast, recommendation, survey or accepted game feature.*

**Provenance panel [P] (collapsible, but its existence and summary line are always present):**

| Field | Value source | Note |
|---|---|---|
| Source identity | Fixture digest as pinned at G0 | **[U]** value |
| Core trust root | The pinned Core hash/size ([V] 32,567 bytes per [C4]) | Reported, not re-derived by a second decoder |
| Source `status` | Verbatim (H0-A expects `"live"`, [V] [C4]) | Any other value means refuse |
| Scene manifest digest | SHA-256 of the canonical manifest (§6.1) | Recomputed independently by the test harness (T-24) |
| Axis/orientation mapping | The declared mapping | **[U]** until the schema is read |
| Marker-diameter factor | k or "true scale" | §1.2 |
| Build identity | Build ID/version | Set at packaging |
| Renderer | Browser, WebGL renderer string | For honest performance reading |

Computing a digest in the browser: whether the platform crypto API is available when a file is opened from disk is **[U]** (secure-context rules are not verified here). The alternatives are reusing the Core's own hashing path or vendoring a pure-JS implementation (a dependency decision, [C5] line 34).

### 6.3 Year status and "unsupported" distinctions without implying unmodelled ecology

The sources contain **no** formal year-status term "eligible". I use these descriptive labels only. "Display-eligible" is a **dossier-local term**: *an owner-approved decision to show that year in 3D exists*. **Today that is true for no year in 3D**, because no 3D scope exists (F-2).

| Year / item | Status per sources | Proposed 3D handling [P] |
|---|---|---|
| Year0 (Genesis) | Approved as the 2D H0-A viewer scope ([C3]). **Not** approved for 3D. | Baseline candidate. The banner says "Year 0 (Genesis)". |
| Year1 | Independently qualified, locally protected numeric continuation (#725/#729/#732, as stated by [C1]; **content not read by me**). Excluded from H0-A ([C3]). | Not in the baseline. If the owner later wants it (§9 D-3), it is a separately sourced immutable dataset with its own banner "LOCAL-ONLY, historical, qualified numerically". It is shown as a **static alternative view, never an animated or computed step from Year0**. This is the one place the "no year progression" rule needs careful wording. |
| Year2+ | DRAFT (#760, [C1]) | Not shown. If ever mentioned: "DRAFT, not available". |
| Marking | DRAFT (#761, [C1]) | Not shown and no affordance. |

**Unsupported / hazard / regeneration / canopy distinctions [P].** The packet asks the design to distinguish "unsupported CI/hazard/regeneration" ([C1]). I have no schema for what the source supports of these (**[U]**). The rule is therefore structural:

| Class | Meaning | Display rule |
|---|---|---|
| SOURCE-PROVIDED | Present in the trusted projection | Shown with a SOURCE label |
| DERIVED | Computed by the viewer from SOURCE | Shown with a DERIVED label and the named formula |
| NOT PROVIDED | The concept exists in the project but the projection lacks it | Not drawn. No empty bar, no zero, no greyed placeholder. A single static "Not modelled in this view" disclosure names categories (e.g. canopy, regeneration, habitat, hazard) by text only |
| OUT OF SCOPE | A non-feature (§2.3) | Absent |
| DRAFT | Exists only as a draft spec | Absent, or labelled "DRAFT, not available" |

**Visual rules that avoid implying ecology [P]:** neutral flat ground; no understorey, grass, soil or leaf-litter imagery; no horizon, sky, sun position or compass; no crown geometry in the baseline; shadows off or labelled "cosmetic, not light modelling"; no heat-map or health ramp; no growth or season cue; no real-place or geographic imagery. This follows the repo rule against inventing ecological facts ([C5] line 43) and the experiment being synthetic ([C1]).

---

## 7. Scoped test and acceptance design — PROPOSED VECTORS, NONE RUN

**None of these were executed, and no result is claimed.** Execution is not authorised in Phase A ([C1]). Each vector needs an approved 3D spec, source lane and runtime first (§9). Tooling is not chosen. Browser-automation packages would be new external dependencies needing approval ([C5] line 34), so a manual procedure is the fallback for every vector. The "does not prove" column is mandatory.

| ID | Setup | Action | Expected observable result | Does NOT prove |
|---|---|---|---|---|
| **T-01** Count & ID completeness | Trusted Year0 projection loaded; harness holds the independently derived expected ID set | Enumerate: scene instances, DOM list rows, pick keys | Each set has exactly the expected count (100) and equals the source ID set exactly: no duplicates, none missing, none extra; instance ↔ ID ↔ row is one-to-one | That the IDs are ecologically meaningful; that the source is correct |
| **T-02** Pick round-trip | Default camera, then each preset | For every tree, programmatically pick at its projected stem centre; also pick empty ground | Each pick returns that tree's own data ID (all 100); empty ground returns none | Mouse accuracy for a human; behaviour under every camera pose |
| **T-03** Ordinal truthfulness | List sort/filter available | Re-sort and filter; select a tree before and after; inspect ARIA names | The ID stays constant; ordinal changes and is labelled "display order"; no code or DOM key uses the ordinal as an ID; labels always carry the full ID | That users will not misread ordinals |
| **T-04** Deterministic manifest | Same package | Load cold twice, on two browsers, on two machines; read the manifest digest | Identical digest everywhere; no pixel assertion | Pixel identity; correctness of the source |
| **T-05** Position fidelity | Harness reads trusted Core output | Compare the scene description to Core values before GPU upload | Exact numeric equality; no jitter, snap or clamp | On-screen pixel position (GPU/projection); true field geometry |
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
| **T-17a–f** Negative: bad or missing source | Variants: (a) no fixture; (b) empty; (c) truncated; (d) wrong digest; (e) 99 and 101 records, or a duplicate ID; (f) NaN/Inf or out-of-extent position, or `status` ≠ `live` | Load each | A **refusal screen** naming the specific reason; **zero** markers; no plate with placeholder trees; no sample or demo data; the list empty; the banner still shows "no valid source loaded"; no console exception | All corruption classes; the Core's own trust-root behaviour (the Core's separate test, [C4]) |
| **T-18** Banner present | Walk every state | Load, error, fallback, selection, filter, reset | The banner is visible and non-dismissible in each state; its text matches the template; `status` is verbatim | Reader comprehension |
| **T-19** Authority labelling and wording | DOM text audit | List every inspector field; grep all text for the forbidden wording | Each field carries SOURCE/DERIVED/PRESENTATION; derived fields name a formula; the string "eligible" (and thin/remove/health) does not appear at the 170 mm boundary ([C4]) | That the labels are understood |
| **T-20** Scale honesty | Exaggeration on, then true-scale (if provided) | Read the badge and the applied factor; compare marker geometry with `dbhMm` | The badge shows the factor actually applied; true-scale cylinder diameter equals `dbhMm` ÷ 1000 m within tolerance (geometry data, not pixels) | That the user perceives scale correctly |
| **T-21** First-render performance | A named reference environment (§4.1 fields) | Cold-load ×N, record per §4.1 | The record exists and meets owner-set `T_FIRST_RENDER` | Any other environment; sustained use |
| **T-22** Interaction performance | Same | Fixed orbit/hover script | Frame-time distribution recorded; meets owner-set `T_FRAME_P95` | Other hardware; worst-case drivers |
| **T-23** Reflow/zoom | Browser zoom at an extreme and a narrow window | Operate list, inspector, banner | Content reflows with no loss and no two-dimensional scroll trap for text | Touch devices |
| **T-24** Provenance panel | Harness recomputes digests independently | Compare the displayed values to recomputed ones | The displayed manifest digest, fixture digest and Core pin equal the recomputed values | That the fixture is ecologically valid |
| **T-25** Locale independence | Two browser locales | Compare the DOM text of every numeric field | Byte-identical text | Rendering fonts |
| **T-26** No implied ecology (human review checklist) | Review checklist from §6.3 | Inspect all states and presets | No crowns, understorey, sky, compass, terrain, health ramps, growth cue or geographic imagery. Reviewer name recorded | Anything subjective beyond the checklist. This is a review, not an automated test |

**Coverage map to the required topics:** no network → T-06 · no writes/save → T-07 · no year progression → T-08 · no marking → T-09/T-10 · count and ID completeness → T-01/T-02 · accessibility paths → T-11..T-16, T-23 · negative missing/invalid source → T-17a–f · honesty/provenance → T-04, T-18, T-19, T-24 · performance → T-21/T-22.

---

## 8. Comparison with one 2D H0-A view, at interface and provenance level only

Scope caution: this compares only what the **owner-approved receipts describe** ([C3], [C4]). H0-A is not implemented or rendered (F-5), so there is no actual 2D UI to inspect. **NOT VERIFIED:** the four H0-A spec files, the full #758 DRAFT result, and any rendered view. This dossier does not redesign, edit or depend on #769 and introduces no executable marking or year progression.

| Dimension | 2D H0-A as documented | Proposed 3D counterpart | Gap / risk |
|---|---|---|---|
| Delivery | One self-contained offline HTML/Canvas page, fixture embedded ([V] [C3]) | Would need WebGL or another runtime (undecided, §5) | Packaging and the single-file pattern may not carry over **[U]** |
| Data entry | Core read projection `ccf.h0a.core-read` v1.0.0 ([V] [C4]) | The same projection **only if** the owner approves sharing | The contract is restricted to H0-A, **not** an approved 3D API (F-6). Do not assume reuse. |
| Source trust | Fixed 32,567-byte pinned Core; wrong bytes rejected before parse ([V] [C4]) | Reuse the trusted Core (no second decoder) | A separate 3D codec would breach the "no alternative decoder" principle ([C4]) |
| Identity | All 100 IDs; full observation identity ([V] [C4]) | The same data IDs; ordinal ≠ ID (§1.4) | Ordinal or order differences between 2D and 3D are a risk; display order must be declared |
| Selection | Stem-centre selection ([V] [C3]) | Ray/analytic pick returning the data ID | Overlap and occlusion in 3D can make picks ambiguous (T-02) |
| Inspector | Complete accessible inspector/list ([V] [C3]) | The same accessible table is the primary path; 3D is an enhancement | Parity of fields is not verified |
| Navigation | Zoom/pan/fit ([V] [C3]) | Orbit/zoom/reset + a plan preset | 3D adds camera freedom and motion-sensitivity concerns (§3) |
| Status surfaced | Source `status="live"`; Genesis-only truth ([V] [C4]) | A permanent banner + provenance panel (§6.2) | How the 2D page actually displays status is **NOT VERIFIED** |
| Annotation | Strict `dbhMm < 170`, annotation only, no "eligible" label ([V] [C4]) | The same wording rule (T-19) | An exaggerated marker could visually imply removal or size ranking |
| Excluded | Annual, health, forecast, Year1 toggle, marks, save, network ([V] [C3]) | The same exclusions (§2.3) | A 3D "feel" invites a growth or what-if expectation; the banner must counter it |

---

## 9. Owner decisions (numbered; this dossier does not decide any of them)

| # | Decision | Options | Consequences |
|---|---|---|---|
| **D-1** | Does a Multi 3D view exist, and in what role? ([C1] gate) | (a) No 3D for now; (b) separate non-public prototype; (c) part of a later simulator | (a) closes this track. (b) is the least-coupled option and needs its own spec, physical lane and rights. (c) couples to unspecified simulator work. None can start without the four-file spec and G1/G2 ([C1]). |
| **D-2** | Interaction scope | (a) Inspect-only per §2; (b) (a) + find-by-ID/filter; (c) anything beyond (marking, year progression) | (a)/(b) need no new truth. (c) is **not** within this dossier: it depends on #760/#761 drafts being approved and changes the non-feature list, T-08, T-09 and the security surface. |
| **D-3** | Time scope | (a) Year0 only; (b) Year0 plus Year1 as a separate static dataset | (b) needs a second banner and dataset identity, local-only rights over the Year1 source and fresh qualification. It must remain "static alternative", never "progression". (a) is the smallest verified match to H0-A. |
| **D-4** | Fidelity tier | (a) Stem markers with source DBH; (b) schematic crown or height (needs source data and an authorised, labelled modelling rule); (c) more realistic art | (a) invents nothing. (b) needs data or a rule that this project has not recorded, and is **[U]**. (c) risks the implied-ecology problem ([C5] line 43). |
| **D-5** | Scale policy | (a) Declared exaggeration with a badge; (b) true-scale toggle as well; (c) true scale only | (a) is legible, but must be disclosed. (c) is honest but nearly invisible at 30 m. The factor k is not set here. |
| **D-6** | Accessibility bar | (a) Best effort; (b) WCAG 2.2 AA-targeted with an audit; (c) AA plus testing with a named real screen reader and keyboard-only users | Cost rises (a)→(c). Only (b)/(c) may justify the word "accessible". T-11..T-16 and T-23 map to this. |
| **D-7** | WebGL-unavailable policy | (a) Notice + accessible table; (b) (a) + link to the separate 2D H0-A; (c) refuse entirely | (a) is the proposed minimum. (b) depends on H0-A being implemented and approved (F-5). (c) removes the accessible path. |
| **D-8** | Runtime (implications only) | Browser WebGL / Canvas pseudo-3D / Unity / other | Browser: DOM accessibility is natural, offline single-file is the H0-A pattern ([C3]), and any library is a dependency needing approval. Unity: a parallel accessible UI is needed, Editor/Windows and a lease are required, and accessibility support is **[U]**. This is coordinated with A1's options, which I have not read. |
| **D-9** | Performance thresholds and reference machines | Owner sets `T_FIRST_RENDER`, `T_FRAME_P95` and the named environments | Without them T-21/T-22 cannot pass or fail, and no performance claim may be made. |
| **D-10** | Determinism claim level | (a) Dataset/manifest identity only; (b) (a) + tolerance-based screenshots on one pinned environment | Pixel identity across machines should not be promised (§6.1). |
| **D-11** | Share vs duplicate the H0-A Core projection ([C1] gate) | (a) Share the trusted Core; (b) a separate 3D data path | (a) keeps one trust root but extends a contract limited to H0-A ([C4]), so it needs an explicit approval. (b) would create a second decoder and fresh trust and test needs. |
| **D-12** | Year-status vocabulary and "eligible" | Adopt the §6.3 labels, or an owner-chosen set | The sources define no year-level "eligible" term, and [C4] bans "eligible" at the 170 mm boundary. A single owner-approved vocabulary avoids confusion. |
| **D-13** | Camera complexity | Plan + oblique + orbit (proposed), or plan-only, or fully free | Less freedom lowers cognitive and motion load and the number of tests. |
| **D-14** | Test tooling | Manual procedure only, or scripted browser automation | Scripted tools are new dependencies needing approval ([C5] line 34), and GPU availability for headless runs is **[U]**. |
| **D-15** | Coordinate frame/orientation/extent source | Read the projection schema at G1 and record the axis mapping and extent | Until then §1.2's plate, scale and axis rows stay **[U]**. This needs a lawful read grant that does not exist for 3D (F-10). |

---

## 10. Not verified (explicit)

- Nothing was built, run, rendered or tested, and no browser, Unity, network or test tool was used.
- I did not read any fixture, projection schema, P0/#710/#725/#687 source, #769 file, the four H0-A spec files, the full #758 DRAFT, #756/#757, A1 or Q. The tree record fields beyond ID, `dbhMm`, position and `status`, the ID format, coordinate frame/units/extent, height or crown data, and display order are therefore **[U]**.
- Issues #725, #729, #732, #760 and #761 were **not opened**. Their status is taken only from the packet text.
- WCAG/ARIA/WebGL references are established standards cited from general knowledge, **not fetched or audited**.
- Whether `crypto.subtle` is usable from a local file, how WebGL behaves headless on the reference machine, and any Unity accessibility capability are **[U]**.
- The actor's harness label and the exact effort level beyond "session low" are not verified. No dashboard/Drive sync is claimed.

---

## Citations (sources actually used)

- **[C1]** Corrected binding #770 packet (commit `894129616802daf7b647d48734c03adf2c700ae6`): https://github.com/jermur788/CCF/blob/docs/multi-730-770-corrected-tg-fork-3d-sim-trial-20261118-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-770-DEEPSEEK-TG-FORK-MULTI-3D-SIMULATOR-REAL-WORK-ORCHESTRATION-PACKET-20261118.md (read in full; also source of the packet's statements about #725/#729/#732/#760/#761/#766/#769, taken as stated, not opened).
- **[C2]** Manager #730 Phase A adoption comment: https://github.com/jermur788/CCF/issues/770#issuecomment-6988856214 ; #770 issue body and thread (read; the thread includes the A2 claim https://github.com/jermur788/CCF/issues/770#issuecomment-6989030661).
- **[C3]** Multi #750 owner H0-A choice and four-file spec approval receipt (branch tip `a9579188d1e527f4e94d476c0e02e4285bab0035`): https://github.com/jermur788/CCF/blob/docs/multi-750-h0-a-owner-approved-four-file-spec-20261118-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-750-OWNER-H0-A-CHOICE-AND-FOUR-FILE-SPEC-APPROVAL-RECEIPT-20261118.md (read in full).
- **[C4]** Multi #758 contract verdict and owner local-only JS security approval (branch tip `a7e9228b8429e6b51c1fcb493e02af0a4148452a`): https://github.com/jermur788/CCF/blob/docs/multi-730-758-owner-js-security-approval-20261118-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-730-H0-A-758-CONTRACT-VERDICT-AND-OWNER-LOCAL-ONLY-JS-SECURITY-APPROVAL-20261118.md (read in full).
- **[C5]** Repository rules `AGENTS.md` (lines 34 and 43) at base commit `ff03290a5ef9c53472618aba32591e5822c663e1`.
- External standards (general knowledge, not fetched): W3C WCAG 2.x success criteria 1.4.3, 1.4.10, 1.4.11 and 2.1.4; WAI-ARIA native-element-first guidance; WebGL context-loss behaviour.
