# Access Spine & Diagonal Rack Network — Draft Requirements and QA Checklist

**Status: DRAFT; NOT YET AN IMPLEMENTATION/PRODUCTION ACCEPTANCE GATE. Revised 5 November 2026.** Checkboxes distinguish **source/research evidence** from **future product/Unity acceptance**. #283 browser research provides no runtime qualification.

## A. Authority, decisions and provenance

- [x] AS-R01 — D-064/D-065 remain controlling; D-067–D-069 are direction/hypothesis; D-070 confirms hidden workbench retirement goal only. The new E entire-spine start option is **not approved**.
- [x] AS-R02 — Inspect live ownership and branch before #283 docs-only authoring; isolated branch, no user worktree/Unity/Windows activity.
- [x] AS-R03 — Source-map current authored route, south approach, fixed headland/feeder, map/minimap readers, rack authoring/entrance and MFM sample/mask source.
- [x] AS-R04 — Preserve corrected evidence: #248 45° best sampled 18.274 m; #250 same-side 16.623/17.510/17.855 m; 0.214214 m R1/R2 rectangle gap despite failed 2.4 m editor centreline clearance; #253 ownership/reach evidence remains research, not production.
- [ ] AS-R05 — Independent read-only review of browser spec result and #271 unmerged experimental comparison. Manager research verdict and user decisions recorded before spec approval.
- [ ] AS-R06 — Approve E (established new-world full spine), P (paid/proposed extension) or F (constrained current fallback) explicitly. The 3+2 45° five branches and 16–20 m are reference/gameplay calibration only, not a universal forestry rule.

## B. Initial infrastructure, starting trees and removed workbench

- [ ] AS-R07 — For adopted E, one accurate existing feeder is present in world, Stand Map and minimap at new Enlarged80 start, continuous through the northern extension, with known status and real collision/terrain access. A decorative plank is not proof of machine clearance.
- [ ] AS-R08 — For P, northern route is proposed-only (ghost/no ground/mask/removed trees/no work order before approved policy). If unavailable to build, disable construction approval rather than infer clearing.
- [ ] AS-R09 — Under F, retain current headland-only Rack V2 flow, no fabricated north route or hidden changed treatment area.
- [ ] AS-R10 — Remove new-game Forestry Workbench scene placement **and** Site/initial-tree-clearance footprint as approved by D-070, with code/assets preserved. Runtime disabled collider is not a demonstrated obstacle; check no phantom Site exclusion or construction affordance.
- [ ] AS-R11 — Audit saved `workbench-01` and all serialized `requiredBuildable` references (Saw Pit, Timber Sledge, Basic Shelter, Log Rack). Preserve old-save/historical meaning or document tested compatible migration. Decide separately how other hidden buildables affect Site masks.
- [ ] AS-R12 — Produce reproducible baseline tree count, stable ID/position distribution, origin and **separate** new-world `Site` / `Track` / `Headland` / `Clearing` mask and treatment-unit deltas. No silent rebaseline or automatic Fell state.
- [ ] AS-R13 — A new northern spine has explicit new-world/save identity; old Enlarged80 saves do not gain it because they already contain a southern approach. Legacy40 and Reference Future v1 archive bytes/semantics unaffected.

## C. Five-rack network and forestry decisions

- [ ] AS-R14 — The feeder spine is not counted among **five separate** extraction racks. Study two-sided 3+2 / 45° *off north* as a candidate only; verify exact lengths and conditional 16–20 m spacing against real boundaries.
- [ ] AS-R15 — Entrances on a spine are actually legal, connected and marked on the correct accessible *first* rack-removal trees. Present headland-only snapping/edit validation must be deliberately and safely amended; avoid accidental field acceptance of unrelated trunks.
- [ ] AS-R16 — Resolve R1/R2 2.4 m editor centreline clearance, far-end R1 trunk, all physical/corridor intersections, north-end turning/yard and real player/forwarder access. Removing the workbench alone does not prove any other fix.
- [ ] AS-R17 — Poor/disconnected sketches remain visibly proposed with reason, but Confirm and authoritative confirmed-plan mutation fail closed or require proper reopen/reconfirm. Executed geometry/history remains immutable.
- [ ] AS-R18 — Rack proposal, Confirm, physical entrance Mark, Work Plan Approval, and Execution use one saved rack set, exactly one authorised order path and no speculative Fell marks or unexecuted ground exclusions. Existing reopening/approved-work warning and consent remain.
- [ ] AS-R19 — Explicit Work Plan economics: E is pre-existing sunk infrastructure with **no new-work payment**; P requires genuine costs/orders/commitment and execution if ever selected; no duplicate tree charges.

## D. Multi-scale forestry-management safety

- [ ] AS-R20 — Preserve existing `ScenarioOneRackSampleGeometry.Derive` unchanged for every previously valid parallel plan, with no opportunistic better-fit switchover.
- [ ] AS-R21 — Additive A derivation only for bounded two-family eligible plans; test group-span, singleton/third-family refusal, finite-segment assignment, stable tie/cell boundaries, all rack orders and endpoint inversions.
- [ ] AS-R22 — At most one coverage owner for each cell/tree, or conservative Manual/NotCovered/Excluded. No double counting/duplicate prescription order; masks precede candidate building.
- [ ] AS-R23 — Explicit verified finite access/reach bound and explanation; 9, 12, 15 and 20 m remain *experimental sensitivities*, not silently approved thresholds. Cross-side demonstrated-sample transfer/contamination requires accepted independent calibration.
- [ ] AS-R24 — Confirm, every confirmed edit, prescription creation and new work approval revalidate selected geometry; old confirmed-invalid saved plan loads safely but suspends unsafe **new** work, preserves completed history and offers safe repair without destructive auto-normalisation.
- [ ] AS-R25 — Coverage unit IDs/meaning, rule and calibration versions, source sample, old prescription links, hashes and safe save round-trips remain valid or explicitly compatibly guarded; absence of new fields does not prove identity.

## E. Truthful status and three-view rendering

- [ ] AS-R26 — Single status-bearing geometry truth shared in 3D, Stand Map, minimap, physical entrance markers, Objective 1 prompts and Work Plan: **Existing / Proposed / Confirmed / Marked / Approved / Executed**.
- [ ] AS-R27 — Existing is visibly traversable; Proposed is ghost/no clearing; Confirmed is pending and not an existing route; Marked is orange entrance spray; Approved is a committed order; Executed alone changes authoritative ground/tree/order. All refresh after load, edit and year change.
- [ ] AS-R28 — Headland/feeder mesh and route-plank depiction reconcile with map; no hard-coded duplicate PathX drift, no unshown formal trails; lesson diagram #258/#282 is not misconstrued as newly constructed track.
- [ ] AS-R29 — Actually captured 3D/Stand Map/minimap at representative viewports and real keyboard/mouse input. A failed render or SVG study cannot qualify visual parity. Physical off-route movement slowdown is later gameplay calibration, not verified practice.

## F. Verification, locks and release

- [ ] AS-R30 — Exact candidate source, isolated worktree, file writer and Unity machine-wide lease preflight confirmed before any local work. No active personal-playtest interruption, unrelated dirty changes or unauthorised scene edit.
- [ ] AS-R31 — Tier 1 during implementation, Tier 2 at feature handoff: real Unity import/compile, scenario new-start, save/load old/new, invalid confirmed plan, five-branch field marking, mask/coverage, Work Plan order transitions and rendered input. Do not weaken tests.
- [ ] AS-R32 — Protected Legacy40 and frozen Reference Future v1 archive/continuation, exact deterministic anchors and saved executed history verified before integration. New-game baseline deviations require explicit acceptance.
- [ ] AS-R33 — Independent code/design review, durable GitHub handoff, Manager integration verdict and dashboard sync. DRAFT spec never by itself authorises production.
- [ ] AS-R34 — User personally playtests full Scenario One candidate, then explicitly authorises any Windows build; no earlier Windows/Player build.

**Evidence boundary:** Checked research facts are not feature PASS. The historical corrected geometry/equivalence data come from #248/#250/#253/#271 and need repeatable original keys/provenance where reused. As of this browser revision no E/P/F selection, full-spine implementation, MFM Option A production, Unity rendered proof, acceptance or build exists.
