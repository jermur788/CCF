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
- **Verification / DONE:** source/compile checks and AC-1/AC-5/AC-6 evidence after admission; FR-4 >=3:1 contrast and both scorers 100% correct, with hot-pink rack fallback and full rerun if orange fails; state-derived geometry with unchanged model values; no copy describing unqualified paint.
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
- **Stop:** disk/lease/process conflict; save/hash drift; pink fallback still fails FR-4 contrast/recognition, or work-state consistency fails; orange-pair failure first triggers the specified fallback and complete rerun. Record failures as failures.

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

## Review response

**DRAFT revision for PR #266; no production or Unity change, no approval of O1–O4.**

| Finding | Disposition / change |
|---|---|
| 1 — MAJOR: near-orange rack/Fell separation and missing fallback | FIXED in draft: FR-4 requires >=3:1 measured interior luminance separation at every required view, including thin textured trunks at 15 m, independently of stripe direction; complete-matrix pink fallback defined. Measurements remain UNMEASURED. |
| 2 — map/legend colours unspecified | FIXED: explicitly orange Fell glyph/legend (replacing current red), orange rack glyph/legend or selected fallback pink, and blue Crop glyph/legend; each matches its qualified world colour. |
| 3 — in-world designation prompts omitted | FIXED: migration inventory includes `ForestTreeMarkingManager.RackEntrances.cs:175–176` blue/red designation prompts → Crop Tree/Fell designation; also audit colour-sensitive entrance prompts. Rejection behaviour stays unchanged. |
| 4 — recognisable depended on reviewer verdict | FIXED: FR-4 sets five-second, label-hidden, answer-key scoring by two independent people, each 100% correct per resolution/mode/distance; unsure/late/missing fails, raw answers/timings retained. AC-1/2/4 reference it; other AC rows retain observable state assertions and AC-7 requires an approved numeric budget. |
| 5 — source quality ring conflated with Crop Tree | FIXED: source blue/white ring denotes high timber quality; “Crop Tree — retain/favour” remains an explicit game calibration label inspired by that practice, not its definition. |

**New fallback rule:** if orange rack versus orange Fell fails either >=3:1 luminance separation or scored recognition in any required view, switch rack paint/glyph/legend/help swatch/colour wording atomically to luminance-adjusted hot pink, keep Fell orange, and rerun the full matrix; stop for redesign if pink fails.

Source claims remain attributed practice, with no adoption of the publisher’s “widely applied in private and state forests” claim. No enum/schema/save field, loaded meaning, gameplay or current marker colour changes; 3D/full-map/minimap/help/curriculum remain covered. This resolves draft wording defects only, pending independent re-review and O1–O4.
