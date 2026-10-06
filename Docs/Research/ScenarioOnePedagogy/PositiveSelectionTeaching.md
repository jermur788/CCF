# Positive selection and the "do not clean up" lesson (Workstreams B3, B4)

**Status:** NEW PROPOSAL [INF]. Tree ids and numbers are from the deterministic Scenario One Year-0 stand, measured by `ResidualStandEvaluation` [HARNESS] (RNG 1, regeneration 1; `Evidence/residual-stand-neighbours.csv`).

## 1. Should the tutorial explicitly teach "start with what you want to keep"?

**Yes.** Recommendation, with reasons:

- The Irish marking guide describes selection as **positive**: identify the quality (Q) or bio tree, assess its actual competitors, then remove only where release is worthwhile [PRAC §11.5]. Spanish, French and Dutch material agree [PRAC §12, §14.2, Netherlands].
- The game already has the right mark. Crop Tree (blue) corresponds closely to the Irish Q-tree intent [PRAC §11.6], and D-013/D-014 accept it.
- The current build's order teaches the opposite: the tree prompt offers Fell first, and the checklist treats Crop Tree and Fell as parallel marks (audit A2).
- The harness shows that removal choices judged against Crop Trees give more release per tree removed. T2: 30 trees, Crop-Tree competition −19.7 %. T1: 84 small trees, −16.6 % [HARNESS].

**Proposed rule of thumb shown to the player** (copy in `TutorialCopyBank.md`):

> Start with the trees you want to keep. Then ask, for each neighbour: is it really taking space from that tree?

## 2. What "competitor" means in this simulation

The game's competition index is the Hegyi index [REPO `ForestEcologyController.HegyiTerm`]:

```
term(neighbour → crop tree) = (DBH_neighbour / DBH_crop) / distance_m,   for neighbours within 8 m
CI(crop tree) = sum of terms
predicted DBH growth = potential × site × size × 1 / (1 + CI / Ci50)        (Sitka Ci50 = 5)
```

What the formula supports:

- A **bigger** neighbour competes more, and so does a **closer** one.
- A small neighbour far away competes very little. This is the "do not clean up" case below.

What the formula does **not** represent:

- Crown position (dominant, co-dominant, suppressed) or crown overlap. A small tree directly under the Crop Tree's crown is counted by DBH and distance only. Under Hegyi, a 10 cm stem 1.9 m from a 20 cm Crop Tree has about half the weight of an equal-sized neighbour at the same distance. Irish practitioners say small trees "often compete little" [PRAC §11.4]; whether the game's weight for close, small stems is too high is a **research question for the ecology owner, not a tutorial issue**. The tutorial must not claim more precision than the index has.
- Bole protection or microclimate benefits of small trees [PRAC §12.2]. These are not simulated, so do not claim them.

**Second finding: competition in this stand is diffuse.** Each Crop Tree has 25–48 neighbours within 8 m. The single strongest supplies only about 8–13 % of its competition [HARNESS]. Removing one competitor gives a small but real release. Removing the top two around each Crop Tree (T2) gives about −20 % competition and +11.7 % predicted growth. The tutorial should say this plainly: "In a dense young plantation no single neighbour dominates; release comes from removing the strongest few."

## 3. The "do not clean up" teaching case (B4)

### 3.1 The case (real ids, Year 0)

Crop Tree **P0707**: DBH 20.7 cm, height 15.2 m, competition index 8.15 ("crowded"), 48 neighbours within 8 m.

| Candidate | DBH | Height | Distance | Share of P0707's competition | Rank | HUD label | Novice instinct |
|---|---|---|---|---|---|---|---|
| **P0706** | 20.5 cm | 14.5 m | 1.54 m | **7.9 %** (term 0.643) | 1 of 48 | crowded | "It's a good tree too — keep it?" |
| **P0710** | 10.7 cm | 8.9 m | 5.99 m | **1.1 %** (term 0.087) | 43 of 48 | crowded | "It's small and suppressed — clean it up" |

(Values from `residual-stand-neighbours.csv`, context Y0.)

### 3.2 What each choice shows (immediate, derivable)

| Choice | Crop-Tree release for P0707 | Timber | Cost |
|---|---|---|---|
| Remove P0710 | competition 8.15 → 8.06 (−1 %) | a small stem, mostly residue | still a contractor visit (€2,500 minimum per visit) |
| Remove P0706 | competition 8.15 → 7.50 (−8 %) | a 20 cm stem with sawlog/pallet assortments | same visit |
| Remove both | −9 % | | |

The release figures follow directly from the terms in the table above (8.15 − 0.087; 8.15 − 0.643). Packet 2 would display them through the same production formula.

### 3.3 The explanation the player sees (no verdict)

> P0710 is small and crowded, but it is 6 m from your Crop Tree and supplies about 1 % of the competition on it. Removing it would cost contractor time and give P0707 almost no extra space.
> P0706 is as large as your Crop Tree and 1.5 m away. It supplies about 8 % of the competition. Removing it gives P0707 more room, and takes a tree that is itself growing well.

The decision stays the player's. The explanation shows reasoning, never "correct".

### 3.4 A rule the lesson must not create

**Not:** "never cut suppressed trees". Removing small trees can be justified — for example, a dense patch where the player wants light at the ground, or a stem that is hampering access. The lesson is: **suppression alone is not a sufficient reason to remove a tree.**

### 3.5 Limitations of this case in the current stand [HARNESS]

- The Year-0 vigour field is spatially smooth, so large trees cluster together and small trees cluster elsewhere. **The stand has no small suppressed stem right beside a Crop Tree**: within 3 m of each Crop Tree, the smallest neighbour is 13.9–20.0 cm DBH (stand DBH range 8–24 cm). The "small tree under the Crop Tree's crown" case — the classic practitioner example — is therefore not present at Year 0.
- Options: (a) teach with the distance version above (true, available now); (b) after the first thinning, regeneration recruits can become small trees near Crop Trees at later interventions; (c) an authored training stand variant (new content). (c) is a **PRODUCT DECISION** and is not needed for a first version.

## 4. Proposed player-facing support (Packet 2)

| Element | Data | Where | Prescriptive? |
|---|---|---|---|
| "Competes with your Crop Tree P0707: strong (rank 1 of 48, 8 %)" | Hegyi term + rank; derivable now | Tree Inspection, when within 8 m of a Crop Tree | No: states a relationship |
| "Your Crop Trees' strongest neighbours" highlight (toggle) | top-k by term; derivable now | Walking view, after a Crop Tree is selected | Borderline. Show **all** neighbours shaded by strength, not just the top k, so it does not read as "cut these". **PRODUCT DECISION REQUIRED** |
| Marking forecast split: "Your Crop Trees: competition −x %, growth +y %; whole stand: growth +z %" | production formulas (harness-validated) | HUD forecast line | No |
| Crop-Tree release count: "Released ≥10 %: 16 of 16 Crop Trees" | per-Crop-Tree ΔCI | Work Plan residual block | No |

Why the count matters: concentrated gap T5 has a *mean* Crop-Tree growth gain (+13.1 %) above T2 (+11.7 %), but **9 of 16 Crop Trees get no release at all** under T5, while all 16 are released ≥10 % under T2 [HARNESS]. A mean alone would mislead.

## 5. Tests

1. Relationship line rank equals a recomputation by `HegyiTerm` over living neighbours within 8 m (harness pattern).
2. No string in the inspection or marking UI contains "correct", "wrong", "should cut" or "recommended" (string test).
3. Removing P0710 alone yields ≤ 2 % Crop-Tree competition change for P0707; removing P0706 yields ≥ 7 % (deterministic Year-0 fixture; re-baseline after Sol's growth integration).
4. Released-count metric: T2 = 16/16 ≥ 10 %, T5 = 7/16 (fixture values; re-baseline after ecology changes).
