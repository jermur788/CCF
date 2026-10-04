# Scenario 1 — final presentation acceptance

Branch `task/scenario-one-final-presentation-acceptance`, based on `main` @ `40ac724` (Scenario 1 implementation `dc975e4`). Presentation-only. No ecology, economy, save, objective or browsing change.

Sol 6.1 started the review: it set up the worktree, the local dependency staging, the review harness, and the first track and HUD corrections. Claude Opus 5.5 completed it after Sol's session ended.

## Method

`Tools/Verification/ScenarioOnePresentationReview.cs` is a disposable harness. It is copied into `Assets/ForestPrototype` only while it runs, by `Tools/Verification/run_final_presentation.py`.
- **How it runs:** in a visible Unity 6000.6.0f1 editor (URP, Vulkan, GTX 980M), with an isolated config so the player save is untouched.
- **What it drives:** the real Scenario One APIs. It marks, approves and resolves work, plants, advances years, saves and loads, and opens the Reference previews.
- **What it captures:** world views and Game-view UI at 1600 × 900, plus 1280 × 720 and 1920 × 1080 for the Work Plan and inspection card.
- **Checks on every capture:**
  - one active display per Sitka;
  - no missing or error materials;
  - fallen-log instances equal deadwood records;
  - residue instances equal completed Sitka fellings;
  - residue bounds compact and grounded.

Capture PASS is evidence only. Acceptance comes from reviewing the images.

## Ten-state review

| State | Result | Notes |
|---|---|---|
| 1 Starting stand | PASS | Uniform Plantation02 Sitka, one display per tree; needle-litter floor; moss reads as low cushions; textured track |
| 2 Inspection / marking | PASS (after fix) | Blue Crop Tree and red Fell marks clearly readable. Inspection card readable at all three resolutions. The mark prompt wrapped its last word; fixed. |
| 3 Crop / pruning | PASS | Designation keeps the same tree model (`ActiveVisualSource` unchanged); one lift raises the crown base to 2.5 m and the lower stem reads clear |
| 4 Immediate thinning | PASS | Stumps, canopy opening and compact residue (≤ 2.6 m, ≤ 0.35 m high, grounded); rows stay readable |
| 5 Timber dispositions | PASS | Annual review lists sold volume and revenue per product and retained timber; the spatial summary shows retained timber; fallen logs mark retained deadwood, and the objectives count it |
| 6 Planting | PASS | Yellow planting markers; oak and beech juveniles at a believable scale; Work Plan shows Contractor vs LandownerSimulated and with/without shelter per order |
| 7 Protection | PASS | Effective (green), expired and failed (grey) tubes are distinct; the inspection line reads "protected by shelter (7 yr left)"; tubes do not block inspection |
| 8 Regeneration | PASS | Planted oak grows through its shelter by Year 6 and joins the canopy by Year 16; natural Sitka regeneration is visible in gaps; ground diagnosis readable |
| 9 Later management | PASS | Second intervention adds new residue beside old; no clutter build-up; Year-30 stand coherent with broadleaves in canopy |
| 10 Save / Reference | PASS | Repeated load gives the identical visual signature (residue, logs, shelters); Year 20/50/100 previews show no player shelters; returning restores the world hash and visuals |

## Changes

| Change | Files | Why |
|---|---|---|
| Track / clearing material uses the existing project soil texture, lighter-tinted (reproducible via **Tools → Forest Prototype → Scenario 1 Track Presentation**) | `Materials/Forest Path.mat`, `Editor/ScenarioOneFloorPresentationSetup.cs` | It was a flat colour next to the textured floor. It is still clearly distinct and lighter. No geometry change. |
| Scenario HUD hidden while the inspection card is open | `ScenarioOne/ScenarioOneManager.cs` (HUD draw only) | The card overlapped the cash/planting HUD |
| Marking summary uses measured width/height | `ForestTreeMarkingManager.cs` | The fixed-width label clipped at the bottom |
| Mark prompt panel sized to its text | `ForestTreeMarkingManager.cs` | Fixed 460-unit width wrapped "Fell" / "Tree" onto a second line |

## Reviewed and left unchanged

- **Dry brash:** subtle but understandable. The same compact patch stays, as thin grey sticks beside the stump. No change; footprint preserved.
- **ForestFloorV1 (Astra):** manifest verified (90/90 SHA-256 OK); provenance is original, project-created, with no third-party content. **Nothing adopted:** with the textured floor, moss cushions and habitat dressing, the review found no ground gap it would solve. Nothing was imported.
- **Dense Sitka lower branches resembling brash:** they are the plantation tree model's dead lower whorls, consistent with an unthinned crop. Not a defect.
- **Moss cushions on the track edge:** plausible on a lightly used forest track. Polish only.
- **Ultimate Nature local dependency:** the gitignored pack is staged locally. Unused files were moved out of the worktree's import folder by Sol to avoid an out-of-memory import. Nothing is tracked.

## Performance (editor gameplay, 1600 × 900, Vulkan, GTX 980M, vSync off, 120 samples)

| State | Median frame | p95 | Triangles (editor stat) |
|---|---:|---:|---:|
| Year 0 | 23.9 ms | 48.2 ms | 14.5 M |
| After thinning | 22.7 ms | 36.6 ms | 14.6 M |
| Planting / shelters | 19.6 ms | 31.7 ms | 15.7 M |
| Later stand (Year 16) | 29.5 ms | 49.4 ms | 25.4 M |

The batch-count stat is unavailable in this editor configuration. A second run (r2) showed a constant 100 ms after Year 0, which is editor background throttling of an unfocused window, not scene cost. The scene content was identical to r1.

## Regression

See the handoff. Anchors are unchanged (neutral `BFC55473C1506067`, normal `3485B6630C9EA448`, completion `568922E1A6D73CDD`, Reference Year 100 `7AD177B3CC2F73C7`).
