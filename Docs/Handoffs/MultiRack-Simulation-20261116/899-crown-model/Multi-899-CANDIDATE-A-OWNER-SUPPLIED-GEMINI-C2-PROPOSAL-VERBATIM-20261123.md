<!--
Provenance record (added by Claude; everything below the rule is the owner-supplied text, unedited):
- Supplied by the project owner in the Claude independent-comparison session on 2026-11-23 as the
  "Proposal A" handoff, together with the paste-ready dispatch for packet
  Multi-899-CLAUDE-INDEPENDENT-LEON-VS-DIRECTIONAL-CROWN-MODEL-COMPARISON-PACKET-20261123.md.
- Author line in the text: "Trial Overall Manager (Gemini) on behalf of Project Owner".
- No earlier GitHub copy was found (searched remote branches for 899/crown/leon/neon and the #899 issue
  comments on 2026-11-23). This file exists so the comparison report can link an exact Candidate A source.
- Identity note: #899 comment 2026-11-22T23:28:51Z names "Gemini Neon" as the Candidate A author. The
  comparison packet calls Candidate A "Leon's solution". This review treats the text below as that Candidate A.
  If the owner meant a different "Leon" document, the A-side verdict of the report does not apply to it.
-->

---

# DESIGN COMPARISON HANDOFF — CCF #899 Realistic Crown Sizing (Shared 2D/3D Engine)

**To:** Claude (Independent Technical & Ecological Design Comparison)
**From:** Trial Overall Manager (Gemini) on behalf of Project Owner
**Repository:** `https://github.com/jermur788/CCF`
**Status:** Design comparison only (read-only — do **not** edit code, push branches, or touch Unity). Production implementation will be `SPEC REQUIRED` once the owner selects a direction.

---

## 1. What You Are Being Asked to Do
The owner has two competing proposals for fixing unrealistic tree crown sizes in CCF (**Issue #899**):
1. **Proposal A (this handoff):** `CrownModel 1` / **Option C2** (area-scaled competition + Year-0 equilibrium).
2. **Proposal B (attached separately by the owner):** Codex's proposed solution.

Please compare **Proposal A** and **Proposal B** head-to-head and give the owner a clear, plain-English recommendation (or a hybrid if combining strengths makes more sense).

---

## 2. The Problem (Plain English & Architectural Goal)
- **What the owner observed:** On the 2D `/multi` map, tree crowns are so oversized and piled on top of each other that the forest floor never becomes visible—even after felling 10+ trees over 3 years.
- **What the owner wants:** Crowns should be a realistic size relative to the actual size of each tree and how crowded it is, so thinning opens genuine visible gaps that gradually close as surviving neighbours grow.
- **Core architectural constraint:** This simulation model is going to be the **single shared simulation engine** for both the **2D and 3D** versions of CCF. The fix must live in the biological simulation core—not as a fake visual shrink in the 2D UI.
- **Project guardrails to preserve:**
  - **Explicit versioning:** Existing saves, Legacy40, frozen Reference Future v1, and protected pure-Sitka anchors must stay frozen under the current rules (`CrownModel 0`). Any new rule must be an explicit `CrownModel 1`.
  - **Three-species parity (D-059):** Must work cleanly across **Sitka spruce**, **Sessile Oak**, and **European Beech** without silently copying Sitka-only assumptions onto broadleaves or inventing pseudo-empirical constants.
  - **Honest evidence labelling:** Clearly separate empirical forestry data from simulation abstractions and gameplay calibration.

---

## 3. Verified Baseline Facts (`CrownModel 0`)
A read-only audit of the repo (`Tools/MultiRackSitkaP1Lab` at `feat/multi-885-astra-opus-ui-20261122-v1` @ `a42720bd`, Unity `main` @ `cbf3a618`, and `Compact30Roster.json` on `integration/sen1-873…` @ `31fbe46e`) confirmed that the lab and Unity use the **exact same Sitka crown math** (except Unity adds a pruning multiplier `0.92^lifts`):

- **Potential crown radius (open-grown upper bound):**
  - **Sitka (`SitkaSpruce.asset:22-28`, `CcfSourcePins.cs:28-30`):** `max(0.1, 0.9415 + 0.07635 × DBH_cm)` — British *open-grown* Sitka (Tabbush & White 1988), meant as a maximum open-field envelope, not a dense plantation crown.
  - **Oak (`SessileOak.asset:27-34`):** `max(0.1, 0.1 + 0.1045 × DBH_cm)`, `Ci50 = 5`, `k = 0.12`.
  - **Beech (`BeechSpecies.asset:27-34`):** power law `max(0.1, √(exp(0.05 + 1.01 × ln(max(0.1, DBH_cm))) / π))`, `Ci50 = 6.5`, `k = 0.15`.
- **Annual competition & relaxation (`AnnualRules.cs:6-98`, `ForestEcologyController.cs:1229-1245`):**
  - Hegyi `CI = Σ (DBH_neighbour / max(1, DBH_target)) / max(0.5, distance)` within `8 m`.
  - Current target: `r_target = r_pot(DBH) / (1 + CI / Ci50)` (Sitka `Ci50 = 5`, relaxation `k = 0.15/yr`).
- **Two root causes found in `CrownModel 0`:**
  1. **Year-0 crowns ignore competition completely:** Both `DerivedManifest.CrownBitsFor` (lab) and `ForestStartingStand.cs` / `Compact30Roster.json` (Unity) start every tree at full open-grown `r_pot`. Year-0 total crown area ÷ stand area is **4.11×** in Area A (171 trees, 915 m², 1,869 stems/ha, BA 74.0 m²/ha), **1.75×** in Area B (73 trees, 1,500 m², 487 stems/ha, BA 39.5 m²/ha), **2.76×** in Compact30 (175 trees, 900 m², 1,944 stems/ha, BA 37.3 m²/ha), and **~3.1×** (est.) in Enlarged80. Because `k = 0.15/yr`, felling trees in Area A leaves cover at `99.8%` and causes canopy cover to behave **backwards** (shrinking for years after felling as oversized Year-0 crowns deflate, rather than opening on felling and re-closing).
  2. **The existing target formula over-shrinks dense stands once relaxed:** In dense plantations (mean `CI ≈ 7.0–7.25`), dividing radius linearly by `(1 + CI / 5)` shrinks crown *area* by `(1 + CI/5)²`, collapsing a dense 1,944 stems/ha Sitka stand (Compact30) to `0.91 m` mean radius and `43.8%` ground cover (half bare ground).

- **Downstream systems that read `CrownRadius` in Unity (audit findings):**
  - **Does NOT feed** Hegyi competition, DBH growth, or density mortality.
  - **DOES feed:**
    - Canopy & ground light (`ForestEcologyController.cs:841`, `ForestTreeMarkingManager.cs:399`): shade reach `r × 1.0 + 2.5 m`.
    - Seed potential (`ForestEcologyController.cs:1353`): `clamp01(r / 4)`.
    - 3D canopy mesh scale (`ForestTree.cs:280`): `(2r, 2.2r, 2r)`.
    - Unity 2D Stand Map (`StandMapCanopyRaster.cs:115`, `StandMapDrawPlan.cs:174`): note `StandMapDrawPlan` already has a `0.62×` radius shrink factor.
    - Save/load (`ForestSaveData.cs:201`) and deterministic hashes (`ScenarioOneInteractionVerification.cs:561`).

---

## 4. Proposal A — `CrownModel 1` (Option C2)

### Core Change
Keep `CrownModel 0` frozen for backward compatibility. Define **`CrownModel 1`** (explicitly labelled *Simulation Abstraction / Gameplay Calibration*) with two linked changes:

1. **Area-scaled competition target (zero new parameters):**
   Apply the existing competition multiplier `1 / (1 + CI / Ci50)` to crown **area** instead of radius—taking the square root for radius:
   $$r_{\text{target}} = \max\left(0.1,\ \frac{r_{\text{pot}}(\text{DBH})}{\sqrt{1 + \text{CI} / \text{Ci50}}}\right)$$
   *(Unity retains its existing pruning factor `0.92^lifts` on top of this target; annual relaxation stays at existing species `k`: 0.15 Sitka/Beech, 0.12 Oak.)*
2. **Competition-aware Year-0 initialization:**
   Initialize Year-0 crowns at their starting competition equilibrium `r_0 = r_target(CI_0)` instead of open-grown `r_pot`.

### Companion Decisions in Proposal A
- **Keep Unity's light rule (`r × 1.0 + 2.5 m`) unchanged for now:** Offline Compact30 cell-group testing showed untouched interior woods stay dark (`0.010–0.026` mean light, well under the ~0.10 suppression band) while a 20% BA thinning lifts gap cells from `0.043` to `0.237` mean light (crossing the ~0.20 Sitka regeneration threshold) and then tapers back down (`0.223` at Y3, `0.215` at Y5).
- **Do not change stand generation densities in this step:** Area A's `74.0 m²/ha` is genuinely very dense, so starting tight (`96.6%` cover) and opening cleanly on a real thinning (`82.6%`) is ecologically coherent.
- **Stage Unity adoption:** Implement and playtest `CrownModel 1` in the shared C# core / lab first, then adopt in Unity with explicit checks on 3D mesh look `(2r, 2.2r, 2r)`, seed potential `clamp01(r/4)`, and whether Unity's Stand Map `0.62×` draw workaround should be removed under `CrownModel 1`.

---

## 5. Offline Test Evidence Supporting Proposal A
All tests were run read-only in offline Python (`%TEMP%\crownaudit\`) using the exact `MultiAreaGenerator.cs` and `Compact30Roster.json` tree coordinates and DBHs (site productivity `1.0`, `k = 0.15`, `CI` recomputed yearly, no mortality, no height growth, canopy union sampled on a `0.25 m` grid).

### Candidates Tested
- **C0 (Baseline):** `r_0 = r_pot`, target `r_pot / (1 + CI/Ci50)`
- **C1 (Equilibrium only):** `r_0 = target`, target `r_pot / (1 + CI/Ci50)` -> **Rejected** (over-shrinks Compact30 to `43.8%` cover / `0.91 m` mean `r`)
- **C2 (Proposed):** `r_0 = target`, target `r_pot / √(1 + CI/Ci50)` -> **Selected**
- **C3a (Runner-up):** `r_0 = target`, target `r_pot / (1 + CI/Ci50)^0.65` -> **Runner-up** (slightly wider gaps, but leaves visible holes in untouched Area A and adds a calibrated exponent)
- **C3b:** `r_0 = target`, target `r_pot / (1 + CI/(2·Ci50))` -> Very close to C3a, adds a second `Ci50` parameter.

### Key Measured Results (C0 vs. C2 vs. C3a)

| Metric | C0 (Current) | **C2 (Proposed)** | C3a (Runner-up) |
|---|---|---|---|
| **Area A Year 0** (`r` mean · Σπr²/area · union cover) | `2.63 m` · `4.11×` · `99.8%` | **`1.73 m` · `1.81×` · `96.6%`** | `1.53 m` · `1.43×` · `91.6%` |
| **Area B Year 0** (`r` mean · Σπr²/area · union cover) | `3.34 m` · `1.75×` · `92.4%` | **`2.95 m` · `1.37×` · `88.0%`** | `2.84 m` · `1.27×` · `86.2%` |
| **Compact30 Year 0** (`r` mean · Σπr²/area · union cover*) | `2.12 m` · `2.76×` · `84.5%` | **`1.38 m` · `1.20×` · `70.8%`** | `1.21 m` · `0.94×` · `63.6%` |
| **Area A 20% BA thin (37 trees): cover** `pre→post→Y3→Y5` | `99.8→98.8→93.4→89.9%` *(backwards)* | **`96.6→82.6→85.5→87.1%`** | `91.6→76.3→80.0→81.9%` |
| **Area A 20% BA thin: new bare ground (m²)** `post / Y3 / Y5` | `9 / 58 / 90` | **`129 / 108 / 96`** | `140 / 122 / 111` |
| **Area A 20% BA thin: largest gap width (m)** `pre / post / Y3 / Y5` | `0.2 / 1.1 / 1.9 / 2.1` | **`1.4 / 3.2 / 2.8 / 2.6`** | `1.6 / 3.6 / 3.2 / 3.0` |
| **Compact30 10-tree cut (6.4% BA): interior light** | `0.003` (`0.000–0.022`) | **`0.010` (`0.001–0.050`)** | `0.013` (`0.002–0.060`) |
| **Compact30 10-tree cut: gap-cell light** `pre→post→Y3→Y5` | `0.034→0.076→0.109→0.124` | **`0.075→0.147→0.138→0.132`** | `0.090→0.170→0.160→0.153` |
| **Compact30 20% BA thin (40 trees): gap-cell light** `pre→post→Y3→Y5` | `0.019→0.161→0.192→0.203` | **`0.043→0.237→0.223→0.215`** | `0.053→0.254→0.239→0.229` |

*\*Compact30 union cover includes the intentionally treeless H1 arrival clearing (~13% of stand area), so `70.8%` total cover corresponds to ~82% cover inside the forested stand.*

### Three-Species Radius Check Under C2 (m)
- **Oak (`Ci50 = 5`):** DBH 15 cm -> `1.67` (CI=0) / `1.41` (CI=2) / `1.03` (CI=8); DBH 30 cm -> `3.23 / 2.73 / 2.01`; DBH 45 cm -> `4.80 / 4.06 / 2.98`.
- **Beech (`Ci50 = 6.5`):** DBH 15 cm -> `2.27` (CI=0) / `1.99` (CI=2) / `1.52` (CI=8); DBH 30 cm -> `3.22 / 2.82 / 2.16`; DBH 45 cm -> `3.96 / 3.46 / 2.65`.

---

## 6. Known Limitations & Unverified Points in Proposal A
- **Not yet run in Unity:** 3D canopy visuals (`(2r, 2.2r, 2r)`), seed-potential scaling (`clamp01(r/4)`), and multi-decade regeneration/mortality funnels have not been tested in Unity yet.
- **No explicit geometric/neighbour-distance hard cap:** Proposal A relies on Hegyi `CI`, so two very closely spaced trees can still overlap somewhat (though drastically less than `C0`).
- **Broadleaf empirical check not yet done:** Oak and Beech radii were checked via a DBH×CI table, not a multi-year mixed-stand run or published stand-grown broadleaf crown benchmarks.
- **Offline test simplifications:** No mortality, no height growth, constant site productivity `1.0`, and Year-3/Year-5 PNGs were not rendered.

---

## 7. Comparison Questions for Claude
Please evaluate **Proposal A (Option C2)** against **Proposal B (Codex's solution)** on:
1. **Ecological realism:** Which better reflects real Sitka spruce (and Oak/Beech) crowns relative to tree size, crowding, thinning gaps, and gap re-closure without inventing unsupported constants?
2. **2D & 3D shared-engine fit:** Which works cleaner as one authoritative simulation rule shared by both the 2D `/multi` map and the 3D Unity world (including light coupling, 3D visuals, and deterministic saves/versioning)?
3. **Edge cases & failure modes:** Where could each proposal break (e.g., very dense stands like Area A at 74 m²/ha, very sparse stands, edge trees, mixed-size trees, heavy thinnings, or very small/large DBH)?
4. **Simplicity & maintainability:** Parameter count, clarity for a learning developer, and risk of unintended ripple effects in Unity.
5. **Verdict:** Recommend **Proposal A**, **Proposal B**, or a **specific hybrid**, and list the 1–2 concrete checks needed before writing the `SPEC REQUIRED` spec.
