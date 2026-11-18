# Scenario One Tree Marking Language V1 Draft Tasks

- **Spec ID:** SPEC-S1-TREE-MARK-LANGUAGE-V1
- **Spec / task-list status:** DRAFT — ALL PRODUCTION TASKS BLOCKED
- **Owner:** Overall Manager; **updated:** 2026-11-04

Tasks derive from [spec.md](spec.md) and [plan.md](plan.md), not new product decisions. This research packet produces documents only; none of these future tasks is dispatched. No Unity/Player/build tonight.

## T-01 — Resolve approval and baseline

- **Status:** BLOCKED — spec approval / O1–O4.
- **Goal:** approve exact meanings, visual migration and visibility policy without changing saved semantics.
- **Scope:** Manager-owned narrow D-013/Rack V2 reconciliation; inspect current candidate and #255 copy/#256 UI results; publish exact code baseline, ownership and acceptance budget.
- **Exclusions:** coding, geometry/MFM changes, automatic state migration or user-test promotion.
- **Owner / routing:** Manager/user; worker model/capacity TBD from live evidence.
- **Dependencies:** independent forestry/source review and O1–O4 decisions.
- **Verification / DONE:** documented approval, source-backed baseline, assigned disjoint files, resolved checklist; no implied implementation authority from DRAFT.
- **Stop:** any semantics/schema/category/legacy-scope expansion returns to Manager.

## T-02 — Scoped field paint and work-state transition

- **Status:** BLOCKED — T-01.
- **Goal:** FR-1–FR-4, FR-8–FR-10; distinguish upright rack paint, individual removal slashes and Crop band.
- **Scope:** future assigned marking/PlannedFellMarkers files; retire duplicate scoped cues while preserving current pending/approved work feedback; keep legacy path.
- **Exclusions:** new persisted marks, order creation/approval, input remapping, packages, source-asset deletion.
- **Owner / routing:** TBD; one production writer, exact file grant required.
- **Dependencies:** T-01; coordinate atomic delivery with T-03.
- **Verification / DONE:** source/compile checks and AC-1/AC-5/AC-6 evidence after admission; FR-4 normal-colour ΔE76 >=40, mark/background contrast >=3:1 and both scorers 100% correct, with exact conditional pink rack fallback and full rerun; follow the documented redesign path on recognition failure; state-derived geometry with unchanged model values; no copy describing unqualified paint.
- **Stop:** cue migration loses work visibility, needs state rewrite, conflicts with MFM/legacy or exceeds approved performance budget.

## T-03 — Matching map shapes, legend and teaching

- **Status:** BLOCKED — T-01 and coordinated T-02.
- **Goal:** FR-5–FR-7 and accurate current-state labels.
- **Scope:** assigned read-only map model/source/draw/legend and help/curriculum/HUD widgets; expose actual marked entrance state only if approved; retain route-only minimap. Audit real legend/swatch drawing, not just its strings: orange Fell, orange-or-fallback-pink rack and blue Crop must match the world. Migrate RackEntrances.cs:175–176 blue/red designation prompts to Crop Tree/Fell designation; audit colour-sensitive entrance prompts too. Keep Crop Tree explicitly a game label inspired by high-timber-quality rings.
- **Exclusions:** map marking actions, geometry/rack validation changes, another map data owner, new paint meaning, independent copy-only release.
- **Owner / routing:** TBD after #255/#256 ownership clears; no current file grant.
- **Dependencies:** T-01 plus T-02 visual contract.
- **Verification / DONE:** offline geometry/filter/adapter tests, AC-2/AC-3/AC-4 render evidence; exactly three swatches, upright/diagonal/ring silhouettes, proper marked versus confirmed distinction.
- **Stop:** mini/full/3D disagree, first-use gating must change beyond approved scope, or source attribution overclaims.

## T-04 — Bounded qualification and migration evidence

- **Status:** BLOCKED — T-02/T-03 and fresh Unity admission.
- **Goal:** demonstrate AC-1–AC-7 on exact candidate.
- **Scope:** meaningful offline checks now available in future packet; later compiler/import, both-resolution matrix, genuine old/current save comparisons, input and performance delta. Inspect every harness and protect saves; disposable scripts follow copy/run/remove plus generated `.meta` cleanup.
- **Exclusions:** Unity tonight, full unrelated Tier-3 repetitions, Player/Windows build, weakening/rebaselining a gate.
- **Owner / routing:** assigned qualifier; model/effort/lease/capacity TBD.
- **Dependencies:** exact code head and approved policy; no code writer may self-accept evidence.
- **Verification / DONE:** GitHub-tracked handoff with raw-evidence keys/files, measured pass/fail and unknowns for every AC row; comparison configuration recorded. Native Windows remains a separate release gate.
- **Stop:** disk/lease/process conflict; save/hash drift; unresolved FR-4 failure after the documented fallback/background correction/shape redesign path, or work-state consistency fails; no unchanged rerun or weaker gate can erase failure. Record failures as failures.

## T-05 — Independent conformance and disposition

- **Status:** BLOCKED — T-04.
- **Goal:** independent source/evidence/spec review, then Manager decision.
- **Scope:** review published exact head, evidence table and all FR/AC rows; identify material corrections; Manager alone assigns safe integration/coordination/dashboard follow-up.
- **Exclusions:** worker self-approval, automatic merge, marking this DRAFT accepted from authored text alone.
- **Owner / routing:** independent other-provider reviewer if available; Manager final authority. Actual provider/capacity UNKNOWN until assigned.
- **Dependencies:** approved spec plus completed evidence package.
- **Verification / DONE:** durable independent verdict and explicit Manager disposition; integration/runtime state reported only from actual gates.
- **Stop:** unavailable independence or unresolved mandatory gate is reported, not replaced with author assurance.

## Task summary

| Task | Status | Depends on | Authority |
|---|---|---|---|
| T-01 | BLOCKED | O1–O4 / independent research review | Manager/user |
| T-02 | BLOCKED | T-01 | New production packet |
| T-03 | BLOCKED | T-01, coordinated T-02 | New production packet |
| T-04 | BLOCKED | T-02/T-03, fresh Unity admission | New qualification packet |
| T-05 | BLOCKED | T-04 | Independent reviewer / Manager |

## Review response — round 2 (Option A)

**DRAFT; same PR #266. Finding 1 was not fixed by the previous revision:** its mark-versus-mark >=3:1 rule was infeasible for the intended hot hues. This round replaces that rule; no implementation, Unity run or O1–O4 approval is claimed.

**Choice and trade-off:** Option A separates perceptual colour difference from background legibility. Normal-colour rack/Fell interiors require **CIE76 ΔE*ab >= 40**, plus mandatory shape redundancy. This is a deliberate game calibration floor, not an accessibility standard or guarantee. Grayscale/colour-vision modes rely on the scored shape-recognition gate, not this normal-colour floor. Contrast **>=3:1** applies to each mark against bark/map/legend/help backgrounds, with a measurable edge/backing allowed. No >=3:1 requirement remains between rack and Fell.

### Arithmetic independently recomputed for this round

Inputs are **encoded sRGB [0,1]**, not linear-light Unity shader values. The exact fallback is **(1.000000, 0.080000, 0.580000)**; do not quantise to an 8-bit approximation for this reference computation. Fell remains **(1.000000, 0.350000, 0.050000)**.

| Colour | Encoded sRGB | Relative luminance L | CIELAB D65 (L*, a*, b*) | Contrast versus Fell (diagnostic only) | ΔE76 versus Fell |
|---|---|---:|---|---:|---:|
| Fell reference | (1, 0.35, 0.05) | 0.284749 | (60.3214, 60.4653, 67.9663) | 1.0000:1 | 0.0000 |
| Current rack orange | (0.95, 0.28, 0.025) | 0.234930 | (55.5828, 63.0698, 65.3562) | 1.1748:1 | 6.0042 — FAIL <40 |
| Exact pink fallback | (1, 0.08, 0.58) | 0.239095 | (56.0036, 84.5737, -6.1705) | 1.1579:1 | 78.0777 — PASS >=40 |

Reproduction: linearise each channel `c` as `c/12.92` if `c <= 0.04045`, otherwise `((c+0.055)/1.055)^2.4`. Contrast uses `L=0.2126r+0.7152g+0.0722b` and `(max(L1,L2)+0.05)/(min(L1,L2)+0.05)`. For Lab use linear-sRGB→XYZ rows `(0.4124564,0.3575761,0.1804375)`, `(0.2126729,0.7151522,0.0721750)`, `(0.0193339,0.1191920,0.9503041)`, D65 white `(0.95047,1,1.08883)`, `f(t)=cuberoot(t)` above `(6/29)^3`, otherwise `t/[3(6/29)^2]+4/29`; Lab is `(116f(Y)-16,500[f(X)-f(Y)],200[f(Y)-f(Z)])` using white-normalised XYZ. ΔE76 is Euclidean Lab distance. Compute at full precision, round only for display.

For example, pink/Fell contrast is `(0.2847485507+0.05)/(0.2390949674+0.05)=1.1579`, while ΔE76 is `sqrt((-4.3178)^2+24.1085^2+(-74.1368)^2)=78.0777`. Against Fell, 3:1 would require `L >= 3(0.2847485507+0.05)-0.05 = 0.954246` or `L <= (0.2847485507+0.05)/3-0.05 = 0.061583`. This explains the discarded rule. Background contrast is feasible: against black (`L=0`), Fell gives **6.6950:1**, pink **5.7819:1**; these examples do not prove rendered bark/background performance.

### Disposition and non-circular failure action

- **Finding 1:** replaced the infeasible pair-contrast gate with normal-colour ΔE76 >=40, separate >=3:1 background contrast, mandatory distinct shapes and binding recognition. Current orange fails the numeric floor now, so the exact pink is the candidate for future qualification. Its numeric pass does not qualify renders.
- **FR-1 / fallback:** pink is conditional on orange failing the perceptual floor or recognition. Switch rack paint, full-map glyph, legend, help swatch and colour wording together; retain orange Fell. Background-only failure first requires edge/backing/visibility correction, not an automatic hue switch.
- **Recognition:** two non-author people independently score every required capture within 5 seconds; each must score 100% in every mode/resolution/required distance. Unsure, missing or late = fail. Orange failure triggers the specified pink candidate once. Pink recognition failure requires a documented shape/size/edge/occlusion redesign and complete fresh matrix with new random order; unchanged reruns cannot erase a failure. If passing needs weaker thresholds, changed meanings or scope expansion, stop and return to Manager/user. No return to orange merely because pink's diagnostic pair contrast is below 3:1.
- **Accepted findings 2–5 preserved:** map/legend follow fallback; blue/red designation prompts are replaced with Crop Tree/Fell designation; two non-author scorers retain 5 s/100%/unsure-or-late-fails; Crop Tree remains a game label inspired by the source's high-quality timber ring.

Source-reported practice stays attributed guidance; the publisher's “widely applied” claim is not adopted. No enum/schema/save field, code, Unity asset or saved meaning changes. Only the four spec files and handoff change. Static arithmetic is computed above; rendered contrast, recognition, input and performance remain UNVERIFIED/UNMEASURED. Independent re-review and O1–O4 approval remain outstanding.
