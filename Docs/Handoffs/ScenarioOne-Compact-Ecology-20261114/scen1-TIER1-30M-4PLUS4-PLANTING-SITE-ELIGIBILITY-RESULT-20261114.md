# scen1 — #508 Tier 1 offline result: 30 m entrance clearing, winding footpath and 4 Oak + 4 Beech site eligibility

**Date:** 14 November 2026 UTC. **Task:** [#508](https://github.com/jermur788/CCF/issues/508). **Packet:** [scen1 T3 Code entrance/path/planting packet](https://github.com/jermur788/CCF/blob/docs/scen1-entrance-clearing-winding-path-20261114-v1/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261114/scen1-T3CODE-TIER1-ENTRANCE-PATH-PLANTING-FOOTPRINT-PACKET-20261114.md) (packet branch tip `39728aa9`).
**Worker:** local T3 Code through the Claude Code harness. Provider Anthropic, model **Claude Opus 5.5** (`claude-opus-5-5`). [Registered on #508](https://github.com/jermur788/CCF/issues/508#issuecomment-6883496596) before any tracked edit.
**Branch / start:** `research/scen1-eight-broadleaf-eligibility-20261114-v1` from freshly fetched `origin/main` **`fc18190b976f1363f1b5d1b0881810db2f8c6e39`**. **Worktree:** `/media/jer/ZX20/CCF-scen1-PlantingEligibility-20261114`, newly created; the path was unoccupied. The worktree is docs-only and has no Unity Library.
**Mode:** **OFFLINE ONLY. NO UNITY, NO LEASE, NO SOURCE, SAVE, SPEC, CANONICAL OR COORDINATION EDIT.** This is the only tracked file. Dashboard and Drive: NOT VERIFIED.

## Verdict

**NEEDS CHANGES.** The owner's concept as stated does not fit a 30 × 30 m property: on-site truck entry, turning, loading and separate car parking cannot all fit. A smaller arrangement does pass spatial screening and can go forward for further design (Section 8): a 12 × 10 m shared entrance apron, the existing winding source track reused as the footpath, and one well-lit planting group. The second planting group has a light/operations trade-off that the Manager must resolve.

Labels used: **DIRECT ORIGINAL SOURCE** · **SOURCE-EQUIVALENT COMPUTED** · **MANAGER/WORKER CALIBRATION** · **UNKNOWN/NOT MEASURED** · **REQUIRES NATIVE** · **REQUIRES OWNER/SAFETY APPROVAL**.

| Question | Result | Label |
|---|---|---|
| Original 189-Sitka Year0 data usable? | Yes. All hashes match. **All 189 IDs and positions were independently reconstructed from source** to ≤ 6×10⁻⁷ m | DIRECT ORIGINAL SOURCE + SOURCE-EQUIVALENT |
| Existing entry/approach in the source? | **Yes, in the game scene only.** The authored Legacy40 track (`Dirt Path 00`–`51`) starts at (−0.93, −14.75), on the 30 m south edge, and winds north to `Forest Clearing` (−2.77, 11.0). A real public road, entrance or rights are **UNKNOWN** | DIRECT ORIGINAL SOURCE (scene); road UNKNOWN |
| Entrance clearing H1, 12 × 10 m (120 m², 13.3 %) | **14 Sitka** conflict; 8 more lie within 1 m outside it; 175 Sitka remain | SOURCE-EQUIVALENT COMPUTED |
| Entrance clearing H2, 15 × 15 m (225 m², 25 %) | **28 Sitka** conflict; 16 more within 1 m; 161 remain; worse for planting light | SOURCE-EQUIVALENT COMPUTED |
| Winding pedestrian path | Reusing the existing track corridor (16.7 m from the H1 exit) needs **0 additional Sitka removed** at any width up to 2.4 m. Nearest trunk centre is 1.59 m | SOURCE-EQUIVALENT COMPUTED; walkability REQUIRES NATIVE |
| On-site truck turning | **Fails.** A regulatory-scale 12.5 m-radius turning envelope covers 491 m² (55 % of the plot) and **84 Sitka**. Off-road loading, rights and vehicle type are not established | **VEHICLE OPERATIONS UNQUALIFIED / NEEDS REDESIGN**; REQUIRES OWNER/SAFETY APPROVAL |
| 4 Oak + 4 Beech, two mixed 2 + 2 groups | **Spatially eligible.** All 8 sites pass the source 0.7 m live-stem guard against **all 189** stems, plus a valid cell, ≥ 2 m between saplings, off the path and outside the apron | SOURCE-EQUIVALENT COMPUTED |
| Light for those groups (Year0 source cell model) | G1 is good (cell light 0.221 ≥ Oak promotion minimum 0.20). The path-side G2 is dark (0.0014). The alternative G2W is lit for Oak (0.468) but only 1 m from the apron | SOURCE-EQUIVALENT COMPUTED static proxy; survival REQUIRES NATIVE |

## 1. Provenance and source guards

**Original evidence (read only):** `/media/jer/Files/CCF/Verification/CompactOneRackNativeFeasibility460-20261113T145748Z/native-20261113T152528Z/`
- `native-result.json` SHA-256 `58524ad4f634378a875313909bb5cf42cfb37468abe0e60a61b63ce00a51de40`: matches the packet pin.
- `executed-harness.cs` SHA-256 `3c0ebae88448d27b0aae4d0345b37a28026f8c9cc3b417d13920288853019863`: matches the packet pin and `harness-provenance.json`.
- `C-B-30.json` `3e7c37ad010d66382722bbd932db6c93b9de1b78914a38c48b4f1a7e1f9d5a27` and `source-clearance-anchors.csv` `b815bd4cb3f17f57a8a74f0bcc0b3e95f0e51e5184281b250361a50474746d8b` both match `evidence-manifest.json`.
- Manifest: **61/62 entries match.** The single mismatch is `closure-receipt.json`. Its mtime (15:51) is later than the manifest (15:50), and its content is lock release, scratch removal and a board-row closure record. It was rewritten after the run and is **not an analysis input**. `NATIVE-HANDOFF.md` and the manifest itself are not listed in the manifest. All three pinned hashes were rechecked after analysis and are unchanged. No original file was written.

**Stage used:** `C-B-30.trees`, recorded **Year0 before the #460 access clearance**: 189 living age-20 Sitka, seed 20260914, 30 m, 36 cells of 5 m. **The 19 #460 rack fells were not subtracted.** The #490 post-rack Year2 Oak/Beech positions were **not** used as design proof.

**Independent reconstruction (SOURCE-EQUIVALENT, positive control).** I re-implemented the unmodified `ForestStartingStand.Generate` legacy lattice branch (`Assets/ForestPrototype/ForestStartingStand.cs:89–160`) in Python. It reproduces the FNV-1a jitter, the ±(size/2 − 0.6) clamp, clearance omissions first and then lowest-hash `omit-{i}` mortality omissions. With the recorded clearance anchors it reproduces **exactly the same 189 IDs, max position error 5.9×10⁻⁷ m**.
- **Discovery:** the scene serialises `latticeSpacingMeters: 2` (`Assets/Scenes/ForestTest.unity:5931`), not the code default 1.9. The harness's `lattice = floor((size−1.2)/1.9)+1 = 16` is only a slot count. With 1.9 m spacing the reconstruction fails: 4 IDs differ and the max error is 0.92 m. **The 189 positions sit on a 2.0 m lattice.**
- Slot composition: 16 × 16 = 256 slots, minus **37 source clearance omissions** (track / clearing / player start / buildables), minus **30 mortality omissions**, giving **189 living**.
- Canopy control: re-implementing `ForestEcologyController.RecomputeCanopy` (`:829–849`, reach = crown × 1 + 2.5 m, vertical = height/8) gives Year0 mean canopy **0.9798264**. The recorded `states[0].meanCanopy` is **0.9798265**.
- Rack control: my corridor test `|z − (−0.25)| ≤ 1.2` gives **19 stems**, and those IDs exactly equal the recorded `access-clearance` trace. This confirms the coordinate frame.

**Source semantics inspected (main `fc18190b`):**
- `ScenarioOneManager.TryDesignateExactPlanting` (`ScenarioOne/ScenarioOneManager.cs:105–160`) requires, in order:
  - a valid execution method;
  - `CanManage`;
  - a shop offer whose species `SupportsRegeneration`;
  - **owned stock > reserved stock**;
  - **no open exact planting or unpromoted living juvenile within 0.5 m**;
  - `GetCellIndex ≥ 0`;
  - **no living tree within 0.7 m (2D)**.

  It creates a **Pending** `PlantJuvenile` work order and a marker only. **The juvenile is created only when approved work resolves.**
- `ForestEcologyController.GetCellIndex` (`:1963–1973`) uses a floor grid from −size/2.
- Juvenile growth and survival use **cell-level `Light`** (`ScenarioOneManager.cs:2029–2043` through `JuvenileEcologyRules`).
- Species data:
  - `SessileOak.asset`: distinct survival curve; `promotionMinimumLight` 0.2.
  - `BeechSpecies.asset`: generic poor-light rule (survival factor 0.8 when the response is < 0.15); no promotion-light minimum serialised.
- `ForestStartingStand` already has a **pre-existing-generation "clearance omission"** concept. Slots inside `Dirt Path` (r 1.5 m), `Forest Clearing`, player-start or `ForestBuildable` (r 2.2 m) radii are never planted, and these omissions **do not consume the mortality budget**. This is the closest existing source pattern for a "pre-existing entrance/path opening". It is **not** approval to reuse it for new geometry (see Section 7).
- There is no source road, public-road, gate or rights field for the 30 m study. `StandApproachRoute`/`StandApproachPath` model a 6 m south-ridge gate and an outside cutting **only for Enlarged80**, not for this 30 m study.

## 2. Existing source geometry inside the 30 m property

| Feature | Source | Position (x east, z north) |
|---|---|---|
| Authored forest track, 52 planks at 0.5 m, 1.5 m tree clearance each side (the code comment calls it a 2.6 m track) | `Dirt Path 00` … `51` in `ForestTest.unity` | (−0.93, −14.75), on the south edge, winding through (3.00, −4.25) and (−0.43, 5.25) to (−2.71, 10.75); 27.5 m long; gentle S-bend |
| Forest Clearing, r 2.2 m | `Forest Clearing` | (−2.77, 11.0), at the track's end |
| Player start clearance, r 2.2 m | `Player` | (−0.43, 5.25), on the track |
| `ForestBuildable` clearances, r 2.2 m (work-area structures; footprints in a compact world UNKNOWN) | anchors | (−2.75, 11.0), (2.2, 12.8), (−0.3, 11.5), (−5.6, 12.2), (−0.2, 9.0), (−2.5, 9.4), (−4.9, 9.3) |
| Mortality-gap clusters (failed establishment) | reconstruction | SW: P0100–P0106, P0200–P0206, P0212–P0215, P0302–P0305. Row 6 (z ≈ −3): P0604–P0608, P0610–P0612 |

**Interpretation:** the source scene already has a winding opening that enters at the south boundary, so the south edge near x ≈ −0.9 is the **evidence-grounded boundary** for an entrance. It is a scene track, not evidence of a public road, a lawful entrance, a bellmouth, sightlines or land rights: **ROAD CONNECTION UNKNOWN.** In Legacy40 the south ridge is closed scenery; the #460 harness disabled its colliders.

## 3. Stage A: entrance-clearing hypotheses (screening only, not road approval)

Both footprints are **hypothetical rectangles on the south edge**. A slide test along the whole edge picked the placement with the fewest Sitka conflicts, and in both cases that placement contains the track entry. A tree conflicts when its stem centre lies inside the footprint. The "1 m buffer" lists trees whose stem centre is ≤ 1 m outside it: a possible vehicle-body or crown clearance concern, MANAGER/WORKER CALIBRATION.

### H1: 12 × 10 m shared apron, x ∈ [−9, 3], z ∈ [−15, −5]; 120 m² (13.3 %)
**Conflicting original Sitka (14), proposed pre-existing omissions if later authorised, NOT Year1 fells:**

| ID | x | z | DBH cm | Height m | Sale m³ |
|---|---:|---:|---:|---:|---:|
| P0004 | −6.62 | −14.40 | 12.4 | 9.5 | 0.0344 |
| P0005 | −5.00 | −14.40 | 13.7 | 11.0 | 0.0687 |
| P0006 | −3.39 | −14.40 | 17.7 | 12.6 | 0.1423 |
| P0207 | −1.17 | −11.28 | 17.0 | 12.0 | 0.1118 |
| P0306 | −2.63 | −8.96 | 14.5 | 11.4 | 0.0790 |
| P0307 | −1.15 | −9.39 | 14.3 | 10.5 | 0.0730 |
| P0403 | −8.75 | −7.26 | 19.9 | 14.3 | 0.2019 |
| P0404 | −6.77 | −6.67 | 21.2 | 15.0 | 0.2365 |
| P0405 | −5.16 | −7.15 | 19.8 | 14.8 | 0.2044 |
| P0406 | −2.64 | −6.71 | 14.8 | 10.8 | 0.0797 |
| P0407 | −1.02 | −7.19 | 11.9 | 9.7 | 0.0319 |
| P0503 | −8.74 | −5.38 | 18.6 | 13.8 | 0.1654 |
| P0505 | −5.14 | −5.26 | 18.4 | 13.0 | 0.1595 |
| P0507 | −1.01 | −5.29 | 11.6 | 8.6 | 0.0289 |

- Within 1 m outside (8): P0003 0.14 m, P0009 0.27, P0103 0.12, P0109 0.11, P0209 0.23, P0504 0.17, P0506 0.14, P0508 0.26.
- Starting Sitka would be **175**, not 189. These 14 trees' 1.617 m³ notional sale volume would **never exist**; it is not harvest revenue. There is no overlap with the #460 19 rack IDs.
- Year0 cell canopy changes (SOURCE-EQUIVALENT): cell 2 0.932 → 0.000, cell 7 0.996 → 0.532, cell 8 0.999 → 0.398, cell 1 0.965 → 0.888, cell 9 0.914 → 0.878, small changes in cells 3/6/13/14. Mean 0.980 → 0.921.
- The track runs inside the apron along its east side from the gate (−0.93, −15) to the apron exit (2.99, −4.75); the existing omissions mean no extra trees there.
- **Possible uses, not engineered (calibration):**
  - **Visitor mode:** up to about 3–4 cars in perpendicular bays on the west part (x −9 … −4.2) at a common 2.4 × 4.8 m bay assumption, with the track side kept as the access and aisle lane.
  - **Timber mode:** the apron is closed to visitors and cars; a small stack area and the loading stand sit within it.
  - Bay count, aisle width, surface, drainage and stack size are **NOT VERIFIED**.

### H2: 15 × 15 m yard, x ∈ [−10.5, 4.5], z ∈ [−15, 0]; 225 m² (25 %)
- **Conflicts (28):** P0003, P0004, P0005, P0006, P0009, P0103, P0109, P0207, P0209, P0306, P0307, P0403, P0404, P0405, P0406, P0407, P0503, P0504, P0505, P0506, P0507, P0508, P0603, P0703, P0704, P0705, P0706, P0707.
  - P0703–P0707 are also #460 rack IDs. That is only a coincidence of location: no rack is cut.
- **Within 1 m (16):** P0002, P0010, P0102, P0110, P0210, P0310, P0402, P0410, P0502, P0510, P0602, P0702, P0710, P0802, P0804, P0806.
- Starting Sitka **161**. Year0 mean canopy 0.845. Sale volume of the conflicting trees 3.027 m³.
- **H2 does not solve truck turning** (Section 3.1). Its lit cells fall mostly inside the apron or its buffer, so the remaining eligible 2 + 2 groups are dark: best Oak cell light 0.034. **H2 is rejected** as larger without a vehicle benefit and worse for planting.

### 3.1 Vehicle operations: VEHICLE OPERATIONS UNQUALIFIED / NEEDS REDESIGN
- **General regulatory envelope, not site evidence:** EU vehicle type-approval requires vehicles to turn within a ring of about 12.5 m outer and 5.3 m inner radius. A forward-in/forward-out turn on site would need roughly that swept circle. Placed tangent to the south edge on the track it covers **491 m² of the 900 m² plot and 84 of 189 Sitka** (negative control). An articulated lorry (EU maximum about 16.5 m long) also needs an approach and loading length. **A full-size timber lorry cannot turn within any defensible 30 m footprint.**
- Packet sources (Teagasc harvesting access, Teagasc forest roads, Coillte timber-removal code) require adequate entrance, loading, turning and exit, forbid public-road loading, and require turning and loading zones to be free of parked cars during haulage. **None of them gives this site's road, bellmouth, sightline, surface or rights.**
- Therefore actual safe truck access, off-road loading and turning, the parking/road connection, legal rights, vehicle type, surface and drainage are all **NOT VERIFIED / REQUIRES OWNER/SAFETY APPROVAL**. A reversing manoeuvre from or onto a road is a road-safety and permission question, not a geometry pass.
- Car/visitor separation is only credible as **time separation**: the apron is closed during timber collection. Geometry cannot certify it.
- Scale context (DIRECT ORIGINAL SOURCE game calibration): the whole Year0 stand's quoted sale volume is **18.9 m³**. The #460 19-stem access clearance quoted €98.22 revenue against €2,500 cost. Timber collection on this plot would be small and infrequent. That suggests a small collection method rather than a haulage lorry, but this is **game economics, not Irish market evidence**.

## 4. Stage B: winding pedestrian path

**Route A (recommended): reuse the existing source track corridor as a people-only footpath.** It starts at the H1 apron exit (2.99, −4.75) and ends at the authored track end and Forest Clearing (−2.71, 10.75). Length **16.7 m**, a gentle S-bend that ends at an interior clearing. It is **not a through-rack** and does not reach the north edge.

| Segment point | x | z | Notes |
|---|---:|---:|---|
| 0 | 2.99 | −4.75 | apron exit (H1) |
| 1 | 2.80 | −2.25 | passes between G1 saplings |
| 2 | 2.06 | 0.25 | |
| 3 | 0.90 | 2.75 | |
| 4 | −0.43 | 5.25 | old player-start clearance; G2N on the right |
| 5 | −1.68 | 7.75 | |
| 6 | −2.59 | 10.25 | |
| 7 | −2.71 | 10.75 | track end; Forest Clearing (−2.77, 11.0) |

- **Conflicts:** none at path widths 1.2, 1.8 or 2.4 m (counting trunk radius = DBH/2). Nearest trunk centre **1.592 m (P1106)**. 3.0 m is the borderline. Width is **UNKNOWN**; 1.2 m is a MANAGER calibration for testing only.
- **Additional Sitka omissions: 0.** The source already guarantees > 1.5 m from every plank centre.
- **Route B (comparison):** a narrower new route through the stand is avoidable. A straight 1.2 m north–south line at x = −7 (negative control) would hit **6** stems. Short optional spurs to the outer saplings are not needed:
  - G1 straddles the path.
  - To reach G2N's centre from the path, a 1.2 m spur would clip **P1108**.
  - To reach G2W along a direct line from the apron exit, a spur would clip P0508. It is better reached from the apron's west edge.

  **No spur is recommended**; reach from the path edge is **NOT VERIFIED**.
- **Walkability:** #460 native `beforeAccess` shows a 0.3 m-radius player capsule could reach 3,278 of the 0.5 m grid samples, with all 170 non-rack trees inspectable. That was **in the #460 scene and layout, not this hypothetical layout**. Slope, surface, colliders, branch clearance, camera sightlines and findability are **REQUIRES NATIVE / player test**.

## 5. Stage C: 4 Oak + 4 Beech sites after entrance/path (H1 + Route A)

**Site rules applied:**
- Source rules: a valid cell; ≥ 0.7 m from **every one of the 189** original stems (stricter than "remaining living"); ≥ 0.5 m duplicate guard.
- Calibration rules: ≥ 2.0 m between new saplings; ≥ 1.0 m from the path centreline; ≥ 2.0 m from the apron (G2W only meets 1.0 m); outside buildable radii; ≤ 4.5 m from the path or apron so the site is reachable; ≥ 1 m inside the boundary.
- Species assignment: Oak goes to the higher-light cells and Beech to the rest. **Established knowledge:** Oak is light-demanding and Beech shade-tolerant. **This is not a survival forecast.**

The light columns below are a Year0 static proxy from the source species curves (SOURCE-EQUIVALENT). Canopy changes every year, and browsing, shelters, site productivity and stochastic rolls also apply: **REQUIRES NATIVE**.

| Group | Species | x | z | Cell | Nearest living Sitka (m) | Nearest new sapling (m) | Path dist (m) | Apron dist (m) | Cell light | Growth response | Annual survival factor |
|---|---|---:|---:|---:|---|---:|---:|---:|---:|---:|---:|
| **G1** | Sessile Oak | 1.17 | −0.47 | 15 | P0707 2.43 | 2.30 | 1.08 | 4.53 | 0.221 | 0.363 | 1.0 (promotion light OK) |
| G1 | Sessile Oak | 1.57 | −2.73 | 15 | P0508 2.07 | 2.30 | 1.29 | 2.27 | 0.221 | 0.363 | 1.0 |
| G1 | Beech | 3.43 | −0.07 | 15 | P0710 1.69 | 2.30 | 1.18 | 4.95 | 0.221 | 0.699 | 1.0 |
| G1 | Beech | 3.83 | −2.33 | 15 | P0710 1.58 | 2.30 | 1.00 | 2.80 | 0.221 | 0.699 | 1.0 |
| **G2N** (path-side) | Sessile Oak | 0.03 | 6.77 | 27 | P1108 1.43 | 2.01 | 1.11 | 11.77 | 0.0014 | 0.0 | **0.214 (promotion light FAILS)** |
| G2N | Sessile Oak | 1.03 | 5.03 | 27 | P1108 1.63 | 2.01 | 1.18 | 10.03 | 0.0014 | 0.0 | **0.214** |
| G2N | Beech | 1.77 | 7.77 | 27 | P1108 1.21 | 2.01 | 3.12 | 12.77 | 0.0014 | 0.0 | 0.8 |
| G2N | Beech | 2.77 | 6.03 | 27 | P1109 1.05 | 2.01 | 3.19 | 11.03 | 0.0014 | 0.0 | 0.8 |
| *G2W* (alternative, clearing margin) | Sessile Oak | −10.00 | −7.90 | 7 | P0402 1.69 (P0403 1.40 if not omitted) | 2.00 | 13.4 | **1.00** | 0.468 | 0.936 | 1.0 |
| *G2W* | Sessile Oak | −10.00 | −9.90 | 7 | P0103 3.05 (P0403 2.92) | 2.00 | 14.0 | **1.00** | 0.468 | 0.936 | 1.0 |
| *G2W* | Beech | −12.00 | −7.90 | 6 | P0401 1.12 | 2.00 | 15.3 | 3.00 | 0.0009 | 0.0 | 0.8 |
| *G2W* | Beech | −12.00 | −9.90 | 6 | P0301 1.40 | 2.00 | 15.9 | 3.00 | 0.0009 | 0.0 | 0.8 |

- **G1** (centre (2.5, −1.4), 2.3 m square, straddling the path in the row-6 mortality gap) is **spatially PASS and light-proxy PASS**. Its cell is the best-lit cell outside the apron because the track and failed-establishment gaps already thin it. The #490 Oak at (1.7, −3.3) that survived to Year40 was in the same cell, but in the post-rack world. That is **supporting context, not proof**.
- **G2N** (centre (1.4, 6.4)) is **spatially PASS** and on the path, but at Year0 the source model makes it **light-FAIL for Oak** and poor for Beech. G2N sites lie 2.24–2.32 m from the buildable anchor (−0.3, 11.5)/(−0.2, 9.0); the radius is 2.2 m and the actual structure footprints are UNKNOWN.
- **G2W** (centre (−11.0, −8.9)) gives the Oaks clearing-edge light but is **1.0 m from the car-bay side of the apron** (fails the 2 m calibration), is reached from the apron rather than the path, and its Beech pair is in a dark cell.
- **The source cannot supply a second fully lit, path-side, operations-buffered 2 + 2 group in 30 m with H1.** This is the key ecology/design gap.

**Cash (DIRECT ORIGINAL SOURCE game calibration, not Irish retail or grant):**
- Stock is 4 × €5.50 Oak + 4 × €4.50 Beech = **€40** (`ScenarioOneDefinition.cs:48–49`).
- Optional shelters are 8 × €5 = **€40** (`treeShelterMaterialCents = 500`).
- Contractor estimate: 10 planting minutes × €45/h gives €7.50 per order, **€60** for 8 orders (`TryDesignateExactPlanting` estimate formula).
- A Pending marker does not create a juvenile.

## 6. Negative controls (all behaved as expected)

| Control | Result |
|---|---|
| Point inside H1 offered as a planting site | rejected (inside apron) |
| Site 0.5 m from P0807 | fails the 0.7 m live-stem guard |
| Second marker 0.3 m from an existing site | fails the 0.5 m duplicate guard |
| Position x = 15.2 | cell −1 (outside the stand) |
| Straight 1.2 m path at x = −7 | 6 stem conflicts (a path crossing a living stem without omission is rejected) |
| Full turning envelope tangent to the south edge | 491 m², 84 Sitka (no truck PASS from occupancy) |
| "189 unchanged" with H1 | contradicted: 175 remain (14 omissions needed) |
| #460 rack geometry reproduced | 19 stems, IDs equal to the recorded trace |
| Stock guard (logic) | the 5th Oak with 4 owned and 4 reserved fails `GetStockQuantity ≤ GetReservedStockQuantity` (source read, not executed) |
| Public-road loading, car/visitor overlap during haulage | not assumed; marked REQUIRES OWNER/SAFETY APPROVAL |

## 7. Starting-tree provenance implication (proposed spec policy, not code authority)

The owner's world cannot have both "189 unchanged" and an entrance clearing. H1 needs **14 named pre-existing omissions**, giving 175 starting Sitka. The path needs none if it reuses the source track.

Source already treats `Dirt Path`/clearing radii as generation-time clearance omissions that are outside the mortality budget. A spec could define the apron the same way, as **authored pre-existing site history** ("entrance formed before the current owner"), with the exact ID list above. That changes stand inventory, canopy and the save/world identity. It needs Manager, save-owner and spec approval. It must **not** be shown as a Year1 player fell.

## 8. Smallest defensible alternative and recommendation

1. **Keep 30 × 30 m only with reduced vehicle ambition.** Use the H1 12 × 10 m shared apron at the source south-edge track entry (14 omissions), Route A footpath (0 omissions), visitor parking for about 3–4 cars, and **no on-site truck turning**. Timber collection would be one of the following, as an owner decision:
   - (a) a small collection vehicle (for example tractor and timber trailer) loading inside the closed apron;
   - (b) extraction to an **independently permitted** off-plot roadside or forest-road loading point;
   - (c) a simplified abstract "timber collected" settlement.

   Each choice remains REQUIRES OWNER/SAFETY APPROVAL. Do not present a smaller vehicle as equal to a timber lorry.
2. **Planting:** G1 as screened. For the second group the Manager should pick one of three, with the trade-off stated in the spec:
   - G2N on the path, accepting the shade risk that the source model predicts for Oak;
   - G2W at the clearing margin, accepting 1 m apron proximity and access off the path;
   - **allocate more of the cohort to G1's lit area** (for example 3 + 3 there and a smaller second group), which needs a cohort-shape change.
3. **If the owner requires a real timber lorry to enter, load and turn on the property**, 30 m is **not advisable**. Use a larger world: the existing 40 m Legacy40 or 80 m Enlarged80 geometry, which already has a source south-gate approach, would need its own screening. Alternatively, accept an approved off-property yard. That is a product decision, not a worker finding.

**Is compact 30 m still advisable?** Yes for the arrive → park → walk → plant → review loop, using the H1 + Route A + G1 core. **No** for on-site articulated-truck operations.

## 9. Not run / not verified

Unity, Editor and import; native canopy or light after omission; juvenile survival and growth over time; player walkability, colliders, slope, camera and findability; 3D crown and branch clearance; actual road, entrance, bellmouth, sightlines and legal rights; vehicle type, swept path, surface and drainage; stack sizing; save/router and old worlds; Reference Future replay; 40-year growth; Windows build. All **NOT RUN**.

Dashboard/Drive mirrors: NOT VERIFIED. `active-tasks.md` was not edited.

## 10. Reproducibility

These are private, untracked offline scripts using pure Python 3 standard library, run with `python3 -I`, reading the original evidence path as an argument. They are kept durably at `/media/jer/Files/CCF/Verification/scen1-508-EntrancePathPlanting-20261114/`, which has a `SHA256SUMS` file.

| File | SHA-256 |
|---|---|
| `scripts/recon.py` (lattice reconstruction, spacing argument `2.0`) | `8d36154632aaebf13b08d4f61d14b2372a4166bbbf45b6a10f2706cfa6b4048d` |
| `scripts/ledger.py` (final ledger, controls, sites, canopy) | `deffea859d4b2427778810beb28a4e8807d376661a9f702e14a4e4bdee71a5ab` |
| `scripts/slide.py`, `sites.py`, `groups.py`, `lightgroups.py`, `explore.py` (search/exploration) | see `SHA256SUMS` |
| `ledger.json` (output; a second run is byte-identical) | `aa10f54f2fac9ccfa23d11a450f29985c39bf966944c665cd822b0ef49b0bcf3` |

Command: `python3 -I scripts/ledger.py <#460 native dir> ledger.json`.
