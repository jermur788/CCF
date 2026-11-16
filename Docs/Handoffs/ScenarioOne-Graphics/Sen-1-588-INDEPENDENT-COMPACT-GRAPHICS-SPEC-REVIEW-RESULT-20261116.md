# Sen-1 #588 — Independent review of the Compact30 graphics/asset four-file DRAFT

**VERDICT: NEEDS CHANGES (DRAFT DOCUMENT QUALITY ONLY).** 0 BLOCKER · 2 MAJOR · 6 MINOR · 2 NIT.

This is an independent, nonauthor review of document quality. It is **not** spec approval, owner look acceptance, asset or licence approval, a Tier 1/Tier 2 result, or an implementation release. **Unity NOT RUN.** No scene, asset, save, Prefs or Windows build was touched.

The DRAFT is careful about its evidence limits. It keeps owner choices with the owner, and it correctly gates every production task. Two source facts that the draft does not record change what the 1.2 m candidate and FR-09 can promise:

- **M-1:** Route A runs exactly along the old 2.6 m `Dirt Path` plank line. The spec forbids hiding the planks, but it never says whether the Compact world contains them.
- **M-2:** the existing failed-shelter visual already crosses the 1.2 m core at Oak O1.

Both need short text changes, which Manager #424 can make directly as docs-only edits.

| Field | Value |
|---|---|
| Task | Sen-1 #588 independent compact graphics/asset spec review; parent Manager [#424](https://github.com/jermur788/CCF/issues/424); author claim [#587](https://github.com/jermur788/CCF/issues/587) |
| Actual worker | Anthropic **Claude Opus 5.5** (`claude-opus-5-5`) in Claude Code CLI. The session does not expose its effort setting, so none is claimed here. |
| Independence | Not the #587 OpenAI GPT-6 author. Not the #573 Sonnet reviewer or the #577 Opus asset author session; no shared context with either. |
| Tools actually used | Local shell, `git`, `gh` / GitHub API, offline Python 3 (`-I -B`) for geometry and YAML reads. No Unity, Editor, Blender, save or Prefs access. |
| Reviewed candidate | `docs/sen-1-587-compact-graphics-asset-spec-20261116-v1` @ `bdfd4fdeafce7d5431ec2c9330658330cdb090f0` (fresh fetch; base `a8939e85`) |
| Review branch / worktree | `review/sen-1-588-compact-graphics-spec-20261116-v1` (checked vacant on origin, forked from `bdfd4fde`) at `/media/jer/Files/CCF/Operations/CCF-588-Graphics-Spec-Review` (docs-only, no Unity Library, no lease) |
| Starting HEAD | `bdfd4fdeafce7d5431ec2c9330658330cdb090f0`. `main` at review `375c40f0`. |
| Registration | [#588 [RUNNING] comment](https://github.com/jermur788/CCF/issues/588#issuecomment-6913850292) posted before any tracked edit, with a one-file write allowlist |
| Write allowlist | This file only |

## 1. Scope and method

I read all four DRAFT files in full: `specs/scenario-one-compact-graphics-v1/spec.md` (123 lines), `plan.md` (99), `tasks.md` (142) and `checklists/requirements.md` (104). I checked them against these sources:

- **#583** Route A/G1 result (full read; I recomputed every number independently).
- **#573** seven MAJOR findings, M-1 to M-7 (full read).
- **#577** asset report: inventory, F-1 to F-7, Direction W/N, and the §5 hazard table.
- Current source on `origin/main`: `ScenarioProtectionVisuals.cs`, `ScenarioHabitatVisuals.cs`, `ScenarioOneClearance.cs`, `ScenarioHabitatPresentationVerification.cs` and `Assets/Scenes/ForestTest.unity`, plus the plank consumers by name.
- `Docs/Project/decision-log.md` and `specs/_template/`.
- The #580 parent/child compact DRAFTs (searched for scene/plank/Route A text only).

**Not independently inspected:** #508 CSV/ledger, #577 Blender scratch scenes/renders and hashes, any native capture. I did not run anything in Unity.

## 2. Findings

### MAJOR

**M-1 — FR-09 cannot be satisfied as written in the shared scene: Route A is the old plank line, and the spec never decides whether the Compact world contains the plank/approach/headland systems.**

*Evidence (source, reproducible offline from `Assets/Scenes/ForestTest.unity` on `main`):*

- All eight Route A vertices match plank centres to within 0.005 m:
  - A0 = `Dirt Path 20` (2.986, −4.75)
  - A1 = `Dirt Path 25` (2.800, −2.25)
  - A2 = `Dirt Path 30` (2.057, 0.25)
  - A3 = `Dirt Path 35` (0.905, 2.75)
  - A4 = `Dirt Path 40` (−0.428, 5.25)
  - A5 = `Dirt Path 45` (−1.676, 7.75)
  - A6 = `Dirt Path 50` (−2.590, 10.25)
  - A7 = `Dirt Path 51` (−2.715, 10.75)
- The planks are root objects with `m_LocalScale.x = 2.6`, so the old track is **2.6 m wide**. All 52 lie inside ±15 m: 32 along Route A (z −4.75 to 10.75) and **20 south of A0, across the H1 apron area** (z −14.75 to −5.25).
- A visible 2.6 m plank strip on Route A is wider than the 2.4 m case. #583 shows that case puts the O1 and B2 stem centres on the surface and overlaps all four upright shelters.
- `spec.md:83` and `plan.md:43` confirm that `ForestTest.unity` is shared by the historical worlds.
- The planks also drive gameplay, by name and active state:
  - `ForestStartingStand.cs:330`, the no-tree corridor.
  - `StandMapSource.cs:45`.
  - `ScenarioOneManager.PrescriptionApproval.cs:339`.
  - `StandApproachPath.cs:26` and `ScenarioOneHeadlandPresentation.cs:30` (template/clones; see #573 M-2).
- The #577 §5 hazard table, row M-2, asked the new spec to "**state whether the compact world instantiates the plank/headland/approach systems at all; if it does, the full #573 M-2 rule list applies**." The DRAFT did not carry this forward:
  - FR-09 (`spec.md:59`) and T-04 (`tasks.md:155`) forbid touching those objects.
  - Non-goals (`spec.md:36`) forbid recycling them.
  - GQ-07 (`spec.md:121`) mentions only "whether new compact scene/camera is required".

*Failure scenario:* a T-04 worker obeys FR-09 literally in a Compact world loaded in `ForestTest`. The new 1.2 m ribbon then sits on top of a visible 2.6 m plank track that runs across H1 and through G1. Alternatively, the worker hides the planks with `SetActive(false)`, which FR-09 forbids. That also breaks the corridor and Stand Map consumers and the approach clones (#573 M-2).

*Smallest remedy:*

1. Add a requirement next to FR-09: "The Compact world MUST NOT render or consume the historical `Dirt Path ##`, `Dirt Path X###`, `Approach Track ###`, cutting or headland/feeder objects. Either (a) it uses a separate scene or hierarchy where they do not exist, or (b) if it shares `ForestTest`, a profile-gated exclusion follows the full #573 M-2 rule list and leaves Legacy40/E80 states unchanged."
   - The #573 M-2 rule list: `Renderer.enabled` only, never `SetActive` or `Destroy`; a defined order relative to `StandApproachPath` cloning and headland `CreateSurface`; rebuild on `StandApproachPath.Version`, Rack V2 toggle and Legacy40 load; the protected set includes the clones and the cutting.
2. Note that the gameplay consumers (corridor, Stand Map, prescription approval, coverage) are parent-spec scope.
3. Make the choice a **blocking** GQ-07 row.
4. Add to AC-02/AC-05: "no historical plank/track surface is visible or gameplay-active in the Compact world, and all old states are unchanged."

**M-2 — The existing failed-shelter visual already crosses the 1.2 m core at O1. The spec's limiting margin (B2 +0.121 m) is true only for upright shelters.**

*Evidence:* `ScenarioProtectionVisuals.cs:89-95` on `main`. The failed state (`state == 2`) takes the 1.2 m × r 0.10 m cylinder, rotates it `Euler(0, 0, 28°)` and translates it **+0.25 m along world x**. That gives a horizontal footprint from about −0.12 m to +0.62 m in x around the site. I sampled the tilted cylinder surface above ground against the raw Route A polyline, the same 2D method as #583:

| G1 | Upright r 0.10, 1.2 m core / +0.20 verge (#583) | **Failed state**, 1.2 m core / +0.20 verge (this review) |
|---|---:|---:|
| O1 | +0.358 / +0.158 | **−0.141 / −0.341** |
| O2 | +0.563 / +0.363 | +0.045 / **−0.155** |
| B1 | +0.523 / +0.323 | +0.503 / +0.303 |
| B2 | +0.321 / +0.121 | +0.301 / +0.101 |

The +x offset is a fixed world direction, so it pushes the Oak tubes (west of the path) onto it. The overlap is visual only, since the collider is destroyed (`:81-83`).

Several parts of the DRAFT treat the 1.2 m margin as the upright case only:

- FR-05 (`spec.md:55`) quotes only the upright margin.
- FR-04 recommends 1.2 m.
- FR-15 calls the primitive "source comparison".

However, AC-06 requires the genuine failed shelter state to be displayed. As written, a native gate with today's renderer would fail AC-02 at O1 by construction, and nothing in the spec shows that this is already known. #583 §3 flagged the risk ("can lean and shift sideways"), but did not compute it.

*Smallest remedy:*

1. In FR-05, add: "With the current renderer's failed state (28° tilt, +0.25 m world-x offset), O1 already intrudes 0.141 m into the 1.2 m core and O2 has −0.155 m with the 0.20 m verge. The limiting case is O1 failed, not B2 upright."
2. In FR-15/T-06, require that any damaged/failed shelter visual have a bounded, path-aware envelope. For example, lean away from the path, or stay within the upright radius plus X. Treat this as a GQ-02 input.
3. Add the failed state to NEG-02.

### MINOR

**m-1 — Wrong protected path and a hidden character in `plan.md:46`.**

- The line cites `Assets/ForestPrototype/ScenarioHabitatVisuals.cs`. The file is `Assets/ForestPrototype/ScenarioOne/ScenarioHabitatVisuals.cs`.
- `ScenarioOneClearance.cs`, `ClearancePreview.cs` and `VegetationClearance.cs` are given without folders. All three are under `Assets/ForestPrototype/ScenarioOne/`.
- "VegetationSites" on that line contains a **U+200B zero-width space**, so `grep VegetationSites` misses it.

*Remedy:* use the full paths and retype the word.

**m-2 — Parts of the #573 M-5 fix are missing.** FR-11 and T-05 keep the state-level invariant and allow full-cell suppression (good). Three practical points from #573 M-5 were not carried over:

1. `ScenarioHabitatPresentationVerification.cs:202-203` asserts that rendered grass instances equal `GrassPatchCount`. Honest suppression will turn this red. The spec says "not weakened for green", but it does not say how the verifier is scoped. Options: a Compact-specific assertion of rendered + suppressed = state count, with legacy assertions unchanged.
2. Relocation must be deterministic, reusing the existing `Jitter(cell, kind, plant, axis)` hash rather than `UnityEngine.Random`, so the visuals stay stable across save/load.
3. Each vegetation class needs a footprint radius, and rush anchors need defined behaviour.

*Remedy:* add these three lines to FR-11/T-05.

**m-3 — The W comparison preference does not match the FR-05 verge bound.** FR-16 names W as the Manager's comparison preference. #577 rendered W as a ~1.1 m worn core plus a **~0.35 m** soft edge. #583 §3 shows that geometry leaves B2 only about +0.021 m before any spline shift; at the 1.2 m core it reaches −0.029 m. FR-05 caps the fringe near G1 at 0.20 m.

*Remedy:* in FR-16/T-04, state that any W/N2 candidate must be re-proportioned to FR-05, for example by tapering the shoulder near G1, before the native comparison.

**m-4 — T-06 readability target pushes the art toward exaggeration.** `tasks.md:180` asks for a "correct size-of-leaf read at 2–6 m". #577 finding 5 reports that real-scale 0.6 m juveniles are distinguishable only at about 2–3 m and become faint stems at 4–6 m. It concludes that a marker or inspection affordance is needed, not only art. FR-14 forbids miniaturising and calls for honest morphology.

*Remedy:* reword T-06/AC-06 as follows. Species are identifiable in art at about 2–3 m. At 4–6 m the plants are findable through an owner-approved marker/inspection affordance, not enlarged foliage.

**m-5 — Ambiguous save-gate references in `spec.md:83`.** The line reads "T-03B/T-04/T-05/63 ISO native checks remain".

- T-04 and T-05 collide with this spec's own task IDs.
- `T-03B` does not exist in `specs/scenario-independent-save-isolation-v1/` at the signed `200add4e`; it appears only in `active-tasks.md`.
- "63 ISO" presumably means ISO-63.

*Remedy:* qualify each reference with its spec or issue, for example "save spec ISO-58/ISO-63; parent compact T-…".

**m-6 — The trace table and width facts are not in the spec itself.**

- T-01 verification (`tasks.md:119`) calls for a "four-file trace table", but none exists. I checked by hand that every FR-01..20 maps to at least one AC (FR-01/02→AC-01; FR-04/05/06→AC-02; FR-03/07/16→AC-03; FR-10/11→AC-04; FR-08/09→AC-05; FR-14/15→AC-06; FR-12/13/18→AC-07; FR-17→AC-08; FR-19/20→AC-09/12). The checklist mirrors all of them, so there is no coverage gap; the table is just missing.
- The spec states only the 1.2 m margins. The facts that rule out 1.8/2.4 m appear only implicitly in FR-04/NEG-01. From #583: at 1.8 m, B2 has +0.021 m with zero verge; at 2.4 m, the O1 and B2 centres are inside the path and all four upright shelters overlap.

*Remedy:* add a short FR→AC→task→checklist table and one line of #583 width facts in FR-04.

### NIT

- **n-1:** Checklist G573-M4 (`requirements.md:70`) maps to FR-17/AC-08. FR-08 also holds the camera/URP/fog prohibition, so list it too. In the `tasks.md:238` table, "APP spec" should read "APPROVED spec".
- **n-2 (packet, not spec):** the #588 packet R-3 says "1.8 too narrow for realistic shoulder". The constraint is the reverse: 1.8 m is too **wide** to keep a shoulder clear of the existing G1. The DRAFT itself states this correctly.

## 3. Packet checks R-1..R-6

| Check | Result | Evidence |
|---|---|---|
| **R-1** filenames / status / IDs / counts | **PASS** | The four files are at the template paths; all share `SPEC-SEN1-COMPACT-GRAPHICS-V1` and DRAFT status. Counts match: 20 FR, 12 AC, 9 tasks. **66 unchecked, 0 checked** (FR 20 + WF 6 + SA 5 + AC 12 + G573 7 + NEG 8 + V 8). Template sections are present. All 8 GitHub links in the four files resolve. |
| R-1 gating of T-02..T-09 | **PASS** | `tasks.md:108` (header: APPROVED spec, real [RUNNING] claim, exclusive files/worktree, Unity lease). Each task's Dependencies start with **SPEC APPROVED**. The release table (`:229-239`) marks only T-01 as allowed. |
| R-1 reviewer PASS ≠ Manager APPROVED | **PASS** | `spec.md:123`, `tasks.md:121`, checklist V-01/V-03. |
| R-1 scope versus owner count versus 1.2 m candidate | **PASS** | `spec.md:74-79` keeps the owner 2+2 count separate from unsigned sites/width. FR-04 labels 1.2 m as "Manager-recommended source geometry ONLY". |
| **R-2.1** display-only mask after `VegetationSites` | **PASS** (see m-1, m-2) | FR-10, AC-04, NEG-03, plan Phase 4, T-05 exclusions. Clearance targets, preview, summary, cell light and eligibility are all named. |
| **R-2.2** 52 planks / clones / template `sharedMaterial` | **PARTIAL → M-1** | FR-09 protects the full set, including `X###`/`Approach Track`. The question of whether the Compact world contains them was not resolved. |
| **R-2.3** shared `.mat` + Editor tool | **PASS** | FR-08, plan `:47`, T-04 exclusions, AC-05, NEG-04, material hash in plan Tier 1. Both `.mat` files and `Editor/ScenarioOneFloorPresentationSetup.cs` exist at the cited places on `main`. |
| **R-2.4** Solid Colour / no URP camera data | **PASS** | Plan `:20` (verified: `ForestTest.unity:4326 m_ClearFlags: 2`), FR-17, Phase 5, T-07, AC-08, NEG-08. Neutral default; no global change. |
| **R-2.5** fully masked cells honest | **PASS** (m-2) | FR-11, AC-04, T-05: no cross-cell moves, no fake cover, suppression logged. |
| **R-2.6** every production task waits for APPROVED + physical lock | **PASS** | As in R-1. V-03/V-04. |
| **R-2.7** Ultimate Nature / local-only sources not adopted | **PASS** | FR-12, AC-07, NEG-05, T-03 exclusions, FR-19 pins pack presence/fallback. |
| **R-3** 1.2 / 1.8 / 2.4 m and B2 margins | **PASS for #583 numbers; M-2 for failed state** | I recomputed Route A length **16.671961 m** and d(O1, O2, B1, B2) = **1.057755 / 1.262838 / 1.222835 / 1.020976**. Limiting 1.2 m upright margins: **+0.121** (B2), **+0.021** under a 0.10 m adverse shift. FR-05 keeps the 1.2 m core and the 0.20 m illustrative verge separate. |
| R-3 planting count / census | **PASS** | FR-02, AC-01: 189 source, 14 pre-world omissions (not player fells), 175 conditional, 8 buffer survivors; the 2+2 count is fixed (FR-06). |
| R-3 no source vertex / G1 coordinate changed | **PASS** | The DRAFT adds no coordinates; the diff `a8939e85..bdfd4fde` is 6 added Markdown files. |
| **R-4** style not selected for the owner | **PASS** (m-3) | GQ-01/02/05, FR-16/17. W is a "comparison preference only"; no-stumps and neutral lighting are "Manager defaults only". Blender grayscale is labelled as worker-reported. |
| R-4 transplant / shelter / ecology honesty | **PASS** (m-4) | FR-14/15, AC-06/10, NEG-06. `plantedJuvenileHeightM: 0.6` verified at `ForestTest.unity:5793`; uniform scale verified at `ScenarioHabitatVisuals.cs:340`. No invented recruits. |
| R-4 D-032 / licence | **PASS** | FR-18, GQ-03, AC-07. `decision-log.md:78` D-032 **Open** on `main`. D-083 is absent from `main`, as the spec says. |
| **R-5** save / legacy / Reference | **PASS** (m-5) | `spec.md:83`, plan `:61`, AC-05/11, NEG-07. No schema/Prefs; ISO-58 hold. |
| R-5 test proportionality / perf baseline | **PASS** | FR-19/20, AC-09, plan Tier 1–3, T-08. No E80 ~40 ms and no unapproved +15% cap; Tier 3 reserved for the milestone. |
| **R-6** author registration / diff | **PASS** | #587 body edit `[RUNNING]` at **2026-11-16T10:11:45Z**, before the first commit `0d6040b3` at **10:14:33Z**. Base→head: exactly 6 `A` Markdown files (4 specs + result + packet); no Assets/Packages/ProjectSettings/Unity files. |

## 4. What this review does not establish

- No spec approval: that needs Manager #424 after these findings, the owner GQ decisions and D-032.
- No Unity or native evidence of any kind; the M-2 footprint is a source-plane computation on the raw polyline.
- No inspection of #508 data or #577 Blender files and renders; their figures are quoted as worker-reported.
- No licence opinion on any asset.
- Dashboard/Drive mirror: **NOT VERIFIED**.

## 5. Handoff

**Recommended next step:** #424 applies M-1, M-2 and m-1..m-6 as docs-only edits on the author branch. M-1's scene choice (separate Compact scene versus profile-gated exclusion in `ForestTest`) may need routing to the parent compact spec owner. A changed-lines-only delta re-check is then enough; a full re-review is not needed.

**Repository mutation by this review:** this one file on `review/sen-1-588-compact-graphics-spec-20261116-v1`, plus #588 claim comments. Author files are untouched.
