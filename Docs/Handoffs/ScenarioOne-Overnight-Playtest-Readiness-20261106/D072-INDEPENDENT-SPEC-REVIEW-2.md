# D-072 independent spec RE-review (#298, review 2) — targeted re-check of B1–B4

| Field | Value |
|---|---|
| Reviewer | Claude Sonnet 5.5 (`claude-sonnet-5-5`), effort as dispatched: Medium (not independently observable from inside the session). Same model family as review 1; not the spec author (Manager repair). |
| Date | 2026-11-06 |
| Reviewed main commit | `a8443ac8b629b84c8c6cafee99a011d41209e840` (`origin/main` at review time). Spec files read via `git show origin/main:…`; the lane worktree was not fast-forwarded and its working tree holds only this file. |
| Lane | `/media/jer/Files/CCF/Operations/CCF-D072-SpecReview-20261106` (verified clean, HEAD `9aa72d64` = review-1 commit = branch tip, before writing) |
| Branch | `docs/d072-independent-spec-review-20261106-v1` |
| Original review | `D072-INDEPENDENT-SPEC-REVIEW.md` at commit `9aa72d645cff366e1d2e6cd738756c24b42327fb` (NEEDS CHANGES, 4 blocking) — preserved unmodified |
| Files read | `specs/scenario-one-stand-map-headland-teaching-alignment-v1/{spec,plan,tasks}.md`, `checklists/requirements.md`; `MANAGER-D072-SPEC-BLOCKER-REPAIR-20261106.md`; `D072-STAND-MAP-HEADLAND-REPAIR-PACKET.md`; `StandMapSource/DrawPlan/Model/Symbols/Legend`, `ScenarioOneHeadlandGeometry/Presentation`, `ScenarioOneManager.RackPlan.cs`, `ForestStartingStand.cs` (grep) at `main` |

## Verdict: **PASS WITH NON-BLOCKING NOTES — 0 blocking**

All four original blockers are resolved in the amended wording; each now names a binding mechanism, not an "or equivalent". I found no new contradiction with the user's hard constraints and no scope creep. I did find one wording gap inside the new draw layer (NB-1) and a few smaller ambiguities. NB-1 is the one I recommend the Manager folds into the #297 brief or the spec before release. I judged it non-blocking because the spec's own later rendered-junction check and the independent source review would catch it, and the failure is a thin outline arc, not a protected-behaviour break.

**This review is not Manager approval.** #297 stays gated until Manager #200 inspects this review and the current `main` spec, issues an explicit verdict, and registers the live one-writer reservation in `active-tasks.md`. Nothing here releases code.

## B1–B4 disposition

### B1 — separate collection, Full-only, Mini identical, hash, legend: **RESOLVED**
| Requirement | Evidence in amended text | Source cross-check |
|---|---|---|
| `model.Paths` never touched | `spec.md:34` ("NEVER append … element-for-element unchanged in every case"), `tasks.md:18`, `requirements.md:16`, `plan.md:10` | `StandMapSource.cs:345-368` is the only writer of `Paths`; the new collection leaves it alone, so `StandApproachVerification` (`Paths.Count == 2/1`) and `StandMapParity` (`Paths[0]`) from review 1 stay valid |
| New Unity-free `ExistingHeadlandSurfaces` | `spec.md:34`, `plan.md:10`, `tasks.md:18` | `StandMapModel` has no `UnityEngine` (offline harness compiles it) |
| Only `MapDetail.Full` draws it, explicit `detail == MapDetail.Full`, not symbol `InMini` | `spec.md:36` | `BuildStatic` already has `bool full = detail == MapDetail.Full` (`StandMapDrawPlan.cs:71`); `Path` symbol is `inMini: true` (`StandMapSymbols.cs:77`), so rejecting the symbol-flag route is correct |
| HUD Mini **and** embedded Stand-Map Mini identical, element-for-element | `spec.md:36,38,46`, `tasks.md:18`, `requirements.md:17` | Both go through `BuildStatic(…, Mini)` (`HudMinimapView`, `UI/StandMapView.cs`), so one `full` guard covers both |
| Static hash/version includes it | `spec.md:36` (visibility, count, both endpoints, widths, pad centres/radii; Canopy/Dynamic untouched), `requirements.md:18` | `StaticContentHash` (`StandMapModel.cs:199-225`) iterates `Paths` but has no hook for a new list, so the explicit mix-in requirement is needed and present |
| New gated legend row only | `spec.md:36`, `tasks.md:18`, `requirements.md:18`; `StandMapLegend.cs` expressly authorised narrowly (`spec.md:36,48`, `tasks.md:30`) | Existing "forest path" row is `model.Paths.Count > 0` (`StandMapLegend.cs:72`) and stays unchanged. Review-1 N3/N4 are absorbed. |
| No test weakening | `spec.md:34,46`, `requirements.md:16,29` | — |

Residual: none blocking. See NB-2 (where in `BuildStatic` the layer sits relative to rack bands) and NB-3 (legend/symbol registration).

### B2 — pure predicate, real V2 flag, positive surface proof, fail-closed, cadence, V2↔non-V2 clearing: **RESOLVED**
| Requirement | Evidence in amended text | Source cross-check |
|---|---|---|
| Pure `ShouldShowExistingHeadland(v2, enlarged80, surfaceActive)`, only TRUE/TRUE/TRUE draws | `spec.md:26`, `plan.md:12`, `tasks.md:20`, `requirements.md:19` | Predicate is Unity-free, so offline-testable |
| `RackPlanningV2Active` read from the manager, no second formula | `spec.md:26` | `ScenarioOneManager.RackPlan.cs:30-40`; `StandMapSource.cs:264` already reads it the same way |
| Surface proof is observed, not a template name | `spec.md:28`: cached `ScenarioOneHeadlandPresentation`; child `"Scenario One V2 Headland"` `activeSelf` + active-in-hierarchy + enabled `MeshRenderer`; no new accessor | Matches `Presentation.cs:19-26,48-57`: child is created under the presentation's own transform only after `CreateSurface` succeeds (needs `Dirt Path 00` + renderer), and `SetActive(active)` is driven by the same flag. The component is attached at `ForestStartingStand.cs:54-55`. The `private` field is not needed. |
| Fail-closed default | `spec.md:26` ("Any proof unresolved ⇒ runtime hide"), `requirements.md:19` | Closes review-1 B2(c) |
| 0.25 s cadence / explicit invalidation; no per-frame, no all-transform scan | `spec.md:28,40,42`, `plan.md:12`, `requirements.md:20` | `RackRefreshSeconds = 0.25f`, `StandMapSource.cs:41,163-169`; `Refresh` ends with `Restamp()` (`:187`) |
| Clears on V2↔non-V2 swap with unchanged scenery key | `spec.md:28,40,42` ("regardless of unchanged geometry/scenery cache key"; "Do not rely solely on the existing scenery key") | Matches the hazard at `StandMapSource.cs:147` (key = geometry version × 1e6 + approach version, no rack model) |
| STOP if proof needs forbidden files | `spec.md:28,48` | See stop-condition review below |

Residual: none blocking. See NB-4 (retry lookup wording) and NB-5 (redundant `enlarged80` input).

### B3 — exactly two segments + two pads, widths, feeder stops at PathX, draw under Paths: **RESOLVED**
Geometry matches the 3D mesh and constants exactly:
- Constants (`ScenarioOneHeadlandGeometry.cs:7-13`): `HeadlandX −35.4`, `SouthZ −37.7`, `NorthZ 37.7`, `FeederZ −27.4`, `PathX −2.436166`, `Width 2.6`, `PadRadius 2.2` — all as in `spec.md:19,30`.
- 3D mesh (`Presentation.cs:41-46`): strip (west, N–S), feeder rectangle `HeadlandX→PathX` at `FeederZ`, plus discs at the two strip ends (radius `Width/2`) and **exactly two pad discs** at `(HeadlandX,FeederZ)` and `(PathX,FeederZ)` of radius `PadRadius`. The amended contract (two segments, two 2.2 m pads, 2.6 m widths, round caps from the stroke renderer, feeder stops at `PathX`, no spine) is a faithful map of that. The strip-end discs are covered by round stroke caps; the feeder's ends are covered by the pads.
- The approach is "the third already-drawn leg" (`spec.md:30`, `plan.md:14`, `tasks.md:22`), consistent with review 1's finding that `PathX` lies on `model.Paths[1]`.
- Draw order: `spec.md:32` places the whole new layer (casing **and** tan) beneath existing Paths casing/tan; `plan.md:14`, `requirements.md:22` agree. This removes review 1's "dark bar across the approach tan".
- "No extra centreline/extension/northern spine" is stated in `spec.md:30,32`, `requirements.md:21`; the offline draw-plan item count/geometry must prove it (`spec.md:32`).

Residual: **NB-1** (order *within* the new layer is not pinned; the specified order is correct relative to `Paths` only).

### B4 — Unity-free projection, truth table, constants parity, counter, Unity gates NOT RUN: **RESOLVED**
- Projection only in `StandMapSource`; model/draw/predicate `UnityEngine`-free: `spec.md:40`, `plan.md:16`, `tasks.md:24,30`, `requirements.md:25`. This is consistent with `ScenarioOneHeadlandGeometry.cs` importing `UnityEngine` (`:1`), which review 1 showed cannot be compiled into the offline harness.
- Truth table with named negatives (Legacy40, frozen Reference Future v1 preview, non-V2 Enlarged80, V2-with-absent-surface, null/no property, plus V2→non-V2→V2 transitions): `spec.md:26,44`, `tasks.md:24`, `requirements.md:27`.
- Constants parity by source-text regex, no second literal authority, repo precedent named (`MfmCoverageWorld/OfflineLogic/CoverageWorldOfflineCheck.cs`): `spec.md:44`, `requirements.md:26`. The precedent exists at that path on `main`.
- `HeadlandProjectionReads`, incremented only on real projection, shown static under steady walking/refresh, mirroring `TreeScans`/`CanopyChanges` (`StandMapSource.cs:55-57`): `spec.md:42,44`, `requirements.md:28`.
- Unity-dependent gates explicitly NOT RUN and not to be weakened: `spec.md:44,46`, `plan.md:16`, `tasks.md:24`, `requirements.md:29`.

Residual: none.

## Additional checks asked for

- **New contradiction / ambiguity / scope creep.** No contradiction with D-072/D-071. Scope is unchanged (no geometry, racks, saves, minimap, spine, build). The only genuine additions are `StandMapLegend.cs` as an allowed file and an optional `StandMapSymbols.cs` touch; both are narrow, display-only and justified. Ambiguities are listed as non-blocking notes.
- **N2 provenance.** Pinned in four places: `spec.md:58`, `plan.md:16`, `tasks.md:26`, `requirements.md:32`, to `e5fe1761744d880798fd7cc685362f1e28b7c60b`. Verified: that commit exists (`e5fe1761 docs: repair guidance fallback and prompt focus safety`), contains FR-5/AC-D/AC-V text (6 matches in `specs/scenario-one-next-action-guidance-v1/spec.md`), is **not** an ancestor of `main`, and is reachable from `origin/docs/overnight-objective-guidance-spec-20261104`. The Manager G4/G5 approval document cites the same SHA. The divergent old PR is explicitly "never merge" (`spec.md:58`, `plan.md:16`, `tasks.md:26`, `requirements.md:32`). Resolved.
- **File lists vs mechanism.** Coherent. Allowed: `StandMapSource.cs`, `StandMapDrawPlan.cs`, `StandMapModel.cs`, narrow `StandMapLegend.cs`, and "if provably essential" existing types/style (`spec.md:48`; `StandMapSymbols.cs` is the natural home — see NB-3). Read-only: `ScenarioOneHeadlandGeometry.cs`, `ScenarioOneHeadlandPresentation.cs`, scene/prefab/physics, rack plan/manager, save, MFM, work orders. The mechanism needs only reads of the forbidden files. One small inconsistency: the packet's allowed-code list (`D072-STAND-MAP-HEADLAND-REPAIR-PACKET.md:25`) predates the amendment and does not list `StandMapLegend.cs`; `spec.md:36,48` and `tasks.md:30` do. The packet header (`:3`) defers to the repaired specs, so this is harmless, but the Manager should align it before dispatch (NB-7).
- **Stop conditions.** Sufficient. `spec.md:28` and `:48` both say STOP and return to Manager rather than add an accessor, touch the presentation, or soften the fail-closed gate; `spec.md:46` and `requirements.md:29` say a failed protected gate is NEEDS CHANGES, not force-green; `plan.md:16` says STOP on any need for 3D/scene/save change. I confirmed the read-only proof path exists without forbidden edits (`transform.Find` on the cached presentation component; child is created under it, `Presentation.cs:49`), so the STOP is a true escape hatch, not a likely trap.
- **Regression of earlier PASS items.** None. Review-1 items that passed — option B fidelity, discrepancy diagnosis, geometry constants/orientation, serial gate structure, fixture consistency — are unchanged or strengthened. Review-1 N1 (feature-branch `StandMapView.cs` drift) remains carried to the handoff (`MANAGER-D072-SPEC-BLOCKER-REPAIR-20261106.md:15`); N3, N4 and N2 are now incorporated into the spec; N5 and N6 remain non-blocking (see NB-8).

## Non-blocking notes (none gate #297; NB-1 recommended before release)

- **NB-1 (recommended pin) — order inside the new layer.** `spec.md:32` correctly puts the whole new layer beneath `model.Paths`, but does not say how its own casing and tan interleave. The natural implementation copies the existing per-path "casing then tan" loop and uses a single `Disc(…, fill, stroke, …)` item for each pad (`StandMapDrawPlan.cs:364`). Drawn that way, each pad's dark outline ring is painted after the strip's tan and crosses it at ±2.2 m from the junction, and the feeder's casing cap at the west junction sits on the strip tan. Suggested one-sentence fix in `spec.md` FR-M2 (and mirrored in `tasks.md` B3): "Within the new layer, draw all casings (both strokes and both pad outlines) first, then all tan fills (both strokes and both pad fills), so the headland/feeder/pad union reads as one surface; pads therefore need separate casing and fill items." Add the corresponding assertion to the draw-plan item-order test.
- **NB-2 — placement relative to rack bands and clearing.** The `Paths` block sits after rack bands and before rack centre lines (`StandMapDrawPlan.cs:102-142`). "Beneath Paths" is satisfied by inserting immediately before the Paths block, which also paints near-opaque tan (`alpha 240`, `StandMapSymbols.cs:77`) over the first ~1.3 m of each rack corridor fill where the rack meets the headland; rack centre lines and the entrance marker are drawn afterwards and stay visible. Please state the slot explicitly (suggested: immediately before the Paths block) so "do not obscure rack statuses" (`spec.md:32`) has a testable meaning.
- **NB-3 — legend/symbol registration.** Legend text normally comes from the single symbol table (`StandMapLegend.cs:40,72`; header comment). A distinct row needs either a new `MapSymbol` (enum in `StandMapModel.cs`, entry in `StandMapSymbols.cs`, `inMini: false`, and awareness of the symbol list at `StandMapSymbols.cs:125`) or a custom `LegendEntry`. Not blocking: `StandMapSymbols.cs` is "if provably essential" under `spec.md:48`. The #297 handoff should state which route was chosen and show that no existing symbol-table check changed.
- **NB-4 — component lookup wording.** `spec.md:28` allows retry at the 0.25 s cadence while V2 is true and the presentation is unresolved, while `:28`/`:42` also forbid a "new `FindObjectsByType` scan per cadence". `FindFirstObjectByType<ScenarioOneHeadlandPresentation>()` (a component search, not a Transform sweep) is the likely implementation, but if the component never appears this runs at 4 Hz indefinitely. Suggest: bound retries (e.g. a short back-off or give up until a scene/source identity change) and say whether `FindFirstObjectByType` is the permitted call. Alternative `ForestStartingStand`-based lookup is equally fine.
- **NB-5 — `enlarged80` input is redundant.** `RackPlanningV2Active` already requires Enlarged80 (`RackPlan.cs:36-39`), so `V2 && !Enlarged80` is unreachable live. Keeping the third boolean in the pure predicate is harmless and keeps the truth table honest; just do not let the implementer add a second Enlarged80 formula outside the predicate.
- **NB-6 — benign extra repaint.** Mixing the collection into `StaticContentHash` bumps `StaticVersion` (and `Version`) when the headland appears/disappears, which also repaints the HUD minimap and embedded overview. Their draw lists are identical, so this is only a redundant repaint, not a behaviour change. A test expecting "Mini StaticVersion unchanged" would be wrong; the spec correctly tests draw-item equality instead.
- **NB-7 — packet text.** `D072-STAND-MAP-HEADLAND-REPAIR-PACKET.md:25,27,29` still carries the pre-amendment allowed-files list and a looser "full-map-only" description. The header (`:3`) points to the repaired specs as binding; align the body before dispatching #297 to avoid an implementer reading the stale list.
- **NB-8 — carried from review 1, unchanged.** N1 (verify the `StandMapView.cs` refresh key on the feature branch, which already differs from `main`); N5 (if V2 is active but `StandApproachPath` pieces are absent, still draw the headland and stop the feeder at `PathX` — not stated, expected always true in V2 worlds); N6 (T3 fixture Z constants need provenance); N7 (legend must not say "cleared/built/opened"; the amended legend text "Existing V2 headland / feeder — not a planned rack" complies).

## Verified vs not verified

**Verified (static reading, no runtime):** the four amended spec files and the two Manager documents at `a8443ac8`; line-level cross-checks against `StandMapSource.cs` (refresh/cadence/scenery key/`ReadScenery`), `StandMapDrawPlan.cs` (`BuildStatic` order, `full`, `Disc`), `StandMapModel.cs` (`StaticContentHash`, `Restamp`), `StandMapSymbols.cs`, `StandMapLegend.cs`, `ScenarioOneHeadlandGeometry.cs`, `ScenarioOneHeadlandPresentation.cs`, `ScenarioOneManager.RackPlan.cs:30-40`, `ForestStartingStand.cs:54-55`; existence of the offline-check precedent and `StandApproachVerification`/`StandMapParity` paths on `main`; the `e5fe1761` commit content, ancestry and branch reachability.

**Not verified:** no Unity, Editor, compile or test run; no rendering or pixel check of the junction (NB-1/NB-2 are source-order reasoning, not observed output); I did not run any offline harness; `StandMapView.cs` and `HudMinimapView.cs` on the feature branch were not re-read (review 1 covered `main`); the actual runtime behaviour of the surface-proof path (child active/renderer enabled across a V2↔non-V2 load) is a reading of `Presentation.cs`, not an observation; the Manager G4/G5 document was only grepped for the SHA, not re-reviewed; dashboard/issue state was not inspected (no issue or PR comments were made, per instruction).

## Recommended next step (not an approval)

Manager #200 may, at its discretion, add the NB-1 sentence (and ideally NB-2 slot and NB-7 packet alignment) to the spec/packet before release, then issue its own APPROVED / NEEDS CHANGES / REJECTED verdict and register the one-writer reservation. #297 remains **WAITING / NOT RUNNING** until that happens.
