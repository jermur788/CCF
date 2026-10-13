# Oak / Beech adult growth and species × site specification — v1

**Status: PROPOSED — research specification only.** Nothing here is an accepted decision, a canonical-context change or production code. It is supporting evidence and a proposal under `Docs/Project/AgentWorkflow.md` ("Research and agent output are supporting evidence/proposals, not accepted decisions").

| | |
|---|---|
| Role | Forestry Simulation Evidence / Model Specification Reviewer (read-only task) |
| Branch | `claude/ecstatic-archimedes-a06tac` |
| Inspected HEAD | `c1564b857972ee18917e8a461c457533a54cc3f1` (`docs: record integrated cell-index save hardening`) |
| Requested base / context | `321791b8203bc709c57615fd4d6a19614d78db24` — **not retrievable** (see below) |
| Canonical-context commit at inspected HEAD | `c1564b8` (most recent commit touching the canonical set; Decision Log ends at D-058) |
| Files written | this document and `Evidence/` beside it. No production, scene, prefab, asset, package, save-schema or `Docs/Project/*` file was touched |

> **Base discrepancy — read first.** The packet's base `321791b…` is not in this clone and not on any fetched remote (`git cat-file` fails; `git fetch origin` brought 30+ branches, none containing it). The Decision Log at the inspected HEAD has no D-059, so the overnight "D-059" work lies outside this snapshot; its findings enter this document **only as quoted in the packet** (current Oak/Beech laws effectively identical; DBH ≤ ≈37.8 cm at age 112; uniform unsaved productivity scalar; ≈45 cm mid-log premium threshold). I verified every one of those that is derivable from code (§2, §3.G) except the premium threshold's source. The AgentWorkflow stop-rule for an unretrievable locked context is written for mid-task context changes; I judged a read-only research pass at the checked-out canonical-context commit to be safe, and flag it for the Manager (OD-11). If `321791b` carries decisions after D-058 that bear on growth, density, site or Oak/Beech, this specification must be re-read against them.
>
> **Incidental exposure, disclosed.** A `git log --all` subject search surfaced four commit subjects on another worker's branch (`origin/claude/stoic-mccarthy-gtzaub`, including "species-aware mixed density model (growth model 2)"). I read no content from that branch and take no dependency on it. The only consequence for this document is that the numeric value of any new growth-model constant must be allocated by the Manager (OD-3), because "2" may already be claimed in flight.

> **Evidence-access limit — read second.** The eleven project PDFs/reports named in the packet are local to the user's machine (Research Index: "not committed to Git"; Drive `03 — Ecology Research` is unpopulated) and were **not available**. Direct web fetches were blocked by the environment's network policy (DNS/403 on every host); only web-search *summaries* were available. Consequently: (a) no [E] (direct Irish empirical) evidence for adult sessile-oak or beech growth was found or retrievable; (b) every numeric anchor below is either derived from this repository's own documents/code, computed here, or **recalled** and marked unverified; (c) the proposed parameter values are **provisional [C] values chosen inside wide transferred bands**, and Step 0 of §14 (evidence reconciliation by someone holding the PDFs) is a hard gate before they are frozen. The model structure, the diagnosis and the verification design do not depend on the exact numbers.

Evidence labels used throughout:

| Label | Meaning |
|---|---|
| **[E]** | Direct empirical evidence for the stated quantity in the target setting. *None found for adult Irish sessile oak or beech growth.* |
| **[T†]** | Transferred UK/European/Irish-general evidence whose relevant statement I saw as a web-search summary; the primary document was **not opened**. Source named in Appendix A. |
| **[T-unv]** | Transferred value **recalled** from general forestry knowledge (e.g. Forestry Commission yield-class curves); **no source retrieved**. Treat as a wide band, not a point. |
| **[ORD]** | Ordinal trait (ranking only; no magnitude claimed). |
| **[C]** | Calibration / gameplay choice. |
| **[U]** | Unsupported: no evidence found. |
| **[R]** | Repository fact, verified by reading code/docs at `c1564b8`. |
| **[I]** | Inference / assumption needing a decision. |

---

## 1. Executive recommendation

1. **Diagnosis [R, computed].** Oak and Beech adults run through one law (`ForestEcologyController.GrowAdults`, `:993–1024`) with identical asset constants (0.4 cm/yr, 120 cm, 0.4 m/yr, 40 m, Ci50 3). Three defects matter: (i) the repo documents 0.4 cm/yr as a *good-site young/stand-grown* rate (`Docs/BeechSpeciesParameters.md`: "within the researched 0.3–0.5 cm/year range"), yet the law applies it as an *open-grown, zero-competition potential* and then multiplies by the Hegyi competition factor again — a double count; (ii) a linear size brake `(1 − D/120)` takes 32 % off the increment by 38 cm; (iii) a 40 m height ceiling and an age-independent height law make Oak ≈ 3–4 m too tall at age 100 against a (recalled, wide) British band, while Beech height is roughly right already. The packet's "37.8 cm at age 112" **reproduces exactly**: `D(100 adult yr) = 120 − (120 − 5.25)·e^(−0.4·100/120) = 37.78 cm` (§3.G).
2. **Recommended growth architecture — "B+".** Keep the existing *state-based* adult kernel (increment depends on current size, current competition, a site factor; **not** on recorded age), but give Oak and Beech **their own versioned parameter rows held in code** (the precedent is `SitkaGrowthModel.cs`), gated behind a **new value of the existing persisted `growthModel` field**. Species assets are not edited, so growth models 0/1, Reference Future v1 and every legacy save replay bit-for-bit. Sitka's path is not touched at all (§6, §12).
3. **Recommended v1 parameters (provisional [C]):** DBH potential 0.65 cm/yr (both; defined as the zero-competition potential), soft DBH scale 120 cm (unchanged), height potential 0.35 (Oak) / 0.50 (Beech) m/yr, soft height ceiling 33 m (both), Ci50 unchanged (3). Resulting height at Scenario One Year 100 (planted-Year-1 tree, age ≈ 102): Oak 21.8 m, Beech 25.6 m; DBH in a managed stand (CI ≈ 1–2) ≈ 35–41 cm, open-grown ≈ 50 cm (§8).
4. **Oak premium size (≥ 45 cm mid-log ⇒ DBH ≈ 46–52 cm) is a mix of causes and should *not* be fixed by tuning growth.** Under the current law it is unreachable even open-grown before age 147 and never with any competition. Under the recalibrated law it is reached at age ≈ 95 (open-grown) to ≈ 123–150 (CI 1–2) — i.e. at or beyond Scenario One's Century Review and far beyond the Year-25 completion horizon, where planted Oak is ≈ 27 years old at 10–15 cm DBH. That is ecologically and economically honest. **Scenario One should end before premium Oak maturity**; it must not pay a premium or require one (§3.G).
5. **Site: one active generic axis — soil moisture regime — with per-species ordinal response tables; everything else deferred.** The profile is **scenario-definition data, derivable from scenario identity, not persisted** (precedent: `BackgroundBrowsePressure`, `ScenarioOneManager.cs:416–422`). No save-schema change. The reference site has response exactly 1.0 for every species, so Sitka is bit-neutral and *species × site has no observable effect in Scenario One* — it is infrastructure for Scenario Two and for fixtures (§5, §7).
6. **The growth change is what changes Scenario One; the site feature does not.** Sequence the Oak/Beech recalibration first, ship the model **dormant** (not the new-game default), verify, then take a separate default-flip decision — flipping the default changes every JSON-hash anchor merely because `growthModel` is serialised (§11, §13).
7. **Eleven Manager decisions are listed in §16**; the three that gate coding are OD-1 (reference site class), OD-3 (growth-model number) and OD-4 (evidence reconciliation).

---

## 2. Current implementation problem

### 2.1 Verified facts [R]

| # | Fact | Where |
|---|---|---|
| F1 | Non-Sitka adults: `ΔD = Pd·site·clamp01(1 − D/Dmax)·1/(1 + CI/Ci50)`; `ΔH = Ph·site·clamp01(1 − H/Hmax)`. Height has **no** competition term and **no** age. Sitka under growth ≥ 1 replaces only the height step with the Class III envelope (relative height kept). | `ForestEcologyController.cs:1004–1015` |
| F2 | `SessileOak.asset` and `BeechSpecies.asset` have identical `potentialDbhGrowthCmPerYear 0.4`, `maxDbhCm 120`, `potentialHeightGrowthMPerYear 0.4`, `maxHeightM 40`, `ci50 3`, `formHeightRatio 0.5`. Sitka: 1.2 / 120 / 0.45 / 35 / Ci50 5. | `Assets/ForestPrototype/Species/*.asset` |
| F3 | Oak and Beech differ only in crown model, reproduction, juvenile light/survival, browse palatability — not in adult DBH/height growth. | assets; `SessileOakV1.md`; `BeechSpeciesParameters.md` |
| F4 | `ForestEcologyCell.SiteProductivity` is a per-cell float defaulting to 1, **never set and never saved** (the save writes only `establishmentSuitability`). It multiplies adult DBH/height growth (`GetSiteProductivity`, `:1861–1864`), juvenile height growth (`JuvenileEcologyRules.GrowHeight`) and the Model-2 understorey target (`SiteProductivity × SoilStability`, `UnderstoreyCompetition.cs:26`). | `ForestEcologyCell.cs:48`; `ForestSaveController.cs:192–198` |
| F5 | The only "site identity" in the project is the Sitka Class III decision (`Scenario1SiteProductivityDecision.md`, D-048). Soil acidity/moisture/drainage "have not been defined for this site" (`ScenarioHabitatPresentation.md`). | docs |
| F6 | Asset-held growth constants are **shared by every growth model**. Editing `SessileOak.asset`/`BeechSpecies.asset` would change growth-0/1 behaviour, legacy saves and the Reference Future v1 continuation. The Sitka precedent keeps versioned model constants in code (`SitkaGrowthModel.cs`) and leaves the asset alone. | `SitkaGrowthModel.cs:22–109` |
| F7 | `Ci50` is read by seven consumers: DBH growth (`:1009`), suppression history (`:1019`), adult density-mortality vulnerability (`:1060`), crown relaxation (`:1147`), `GetCurrentSuppression` (`:1734`), the marking-manager growth forecast (`ForestTreeMarkingManager.cs:384,397`) and the competitor UI (`CompetitorAssessment.cs:160`). | grep |
| F8 | Hegyi competition is generic and global: 8 m cutoff, `HegyiTerm = (D_nbr/D_target)/max(0.5, d)`; `ForestEcologyController.cs:902–916,920–989`. | code |
| F9 | Promotion: planted stock age 3, 0.6 m; juvenile height growth `0.35·response·site`; promoted at ≥ 3.5 m and (Oak) ≥ 20 % light; DBH at promotion `clamp(1.5·H, 2, 20)`. In good light a planted tree promotes at **age 12, 3.75 m, DBH 5.625 cm**. | `ForestEcologyController.cs:31–33`; `JuvenileEcologyRules.cs:37`; `SessileOak.asset` |
| F10 | `ForestTree.ApplyGrowth` clamps DBH to [1, 200] cm and height to [0.1, 60] m. | `ForestTree.cs:606–612` |
| F11 | Growth model 1 applies the **Sitka** SDI line (`SDI = N/ha·(Dq/25)^2.063`, RD 0.6 onset, `S⁴` vulnerability) to *every* living tree, so Oak/Beech can die as "self-thinning" under Sitka parameters (separate P1 finding, `P3ImplementationRecord.md`). Out of scope here but it interacts with any change to Oak/Beech DBH (§12.4). | `:1040–1120` |
| F12 | Scenario One horizon: `minimumCompletionYear 25`, `centuryReviewYear 100`. Objectives count broadleaf **presence** (living trees + regeneration cells + planted juveniles), never size. Broadleaf timber is valued at a flat €65/m³ whole-stem; `TimberYield` is Sitka-only ("no automatic broadleaf transfer"); a broadleaf market and pruning-quality premium are deferred (D-044). | `ScenarioOneDefinition.cs:35,76–77`; `ScenarioOneObjectives.cs:165–168`; `TimberYieldConfiguration.cs:10` |
| F13 | Harness contracts: `CCFOakVerification` asserts the Oak asset's `MaxHeightM == 40` and `MaxDbhCm == 120` (`:91–92`) and pins `GrowthModel.Legacy` (`:361`); `CCFBeechVerification` pins `GrowthModel.Legacy`; `SitkaGrowthModelVerification:96` asserts `NewGameGrowthModel == SiteClassDensity`. | `Tools/Verification/` |
| F14 | Save is `CurrentVersion = 20`; `growthModel` (v17) is already a persisted int; `ForestSaveValidation.cs:80` rejects values above `GrowthModel.Latest`. | `ForestSaveData.cs:7,25` |

### 2.2 Findings

* **P-1 — two species, one law.** The identical constants are an unvalidated copy. Beech's are documented as a young-stand range; Oak's carry no oak-specific source anywhere in the repo ("0.4 cm/year favourable DBH increment", `SessileOakV1.md`) → Oak-specific adult growth is **[U]**.
* **P-2 — competition is counted twice.** Stand-grown rates already embed average competition; the law re-applies `1/(1 + CI/Ci50)`. At a managed CI of 1–3 the current law yields 0.17–0.25 cm/yr at 20 cm DBH — *below* the repo's own 0.3–0.5 cm/yr range (§9, table T3).
* **P-3 — the size brake dominates the long run.** DBH approaches 120 cm as `D = 120 − (120 − D₀)e^(−t·Pd/120)`; half the gap closes in ≈ 208 years, but the increment is already down a third at 38 cm. Age never enters.
* **P-4 — height ceiling.** 40 m is the Beech doc's "typical mature height; exceptional trees can exceed" — fine as a rare maximum, wrong as the asymptote of the *typical* tree, and wrong for Oak.
* **P-5 — site is inert and unowned.** One uniform float, shared with juveniles and the understorey, unsaved, with no identity.
* **P-6 — promotion hand-off is discontinuous in height increment** (adult law ignores light): e.g. Oak at 20 % light: juvenile 0.105 m/yr → adult 0.31 m/yr (×3.0); Beech at 10 % light: 0.12 → 0.44 (×3.6) (table T9). Pre-existing, not worsened by the proposal, recorded as limitation L-1 (§10).

---

## 3. Oak evidence synthesis (*Quercus petraea*)

### 3.A Adult height growth
* Current [R]: Mitscherlich, open-grown **8.2 / 15.3 / 25.0 / 30.9 m at age 25 / 50 / 100 / 150** (T5).
* Transferred band [T-unv]: British oak top height roughly **11–16 m at age 50 and 18–25 m at age 100** across yield classes ≈ 4–8 (recalled from Forestry Commission general-yield-class curves, Booklet 34 / Forest Yield; **not re-read**). The *range* "pedunculate oak YC 4–8" was seen in a web summary [T†]. The Irish note that FC yield tables have been used reliably for Irish stands since 1953 [T†] supports transferring FC curves for want of an Irish oak top-height model; **no Irish dynamic oak model was found [U]**.
* Verdict: Oak height is ≈ 3–4 m above band centre at age 100 and the 40 m asymptote is not typical. Proposed `Ph 0.35`, `Hmax 33` → 13.5 / 21.6 / 26.3 m at 50 / 100 / 150.

### 3.B Adult DBH growth
* No oak-specific source in the repo [U]. Beech's documented 0.3–0.5 cm/yr is a *good-site young/stand* range [R, B/C-grade in the repo, primary not seen].
* Method (transparent, not curve-fitted): treat 0.3–0.5 cm/yr as the stand-grown rate at a typical managed competition level and divide by the model's own competition factor. Expected Hegyi CI for equal-sized neighbours in a random stand is `16π·λ`: 300 stems/ha → 1.5; 450 → 2.3; 600 → 3.0 (T3). With Ci50 = 3 the factor is 0.67–0.50, so `Pd ≈ 0.4/0.6 ≈ 0.65` (range 0.5–0.8). That is the whole derivation; it uses a repo range and the model's own arithmetic, no external number.
* Cross-check [T-unv]: the common girth rule of thumb for open-grown oak (≈ 2–3 cm girth per year ⇒ ≈ 0.6–0.95 cm/yr DBH in the first century) is consistent with an open-grown potential of 0.65 and inconsistent with 0.4. Recalled, not verified.

### 3.C Asymptotic size / slowdown
* Keep the form `(1 − D/Dmax)` with `Dmax = 120 cm` (unchanged, [C]; not a biological limit). Within the Scenario One window (D ≤ ~60 cm) it gives a plausible deceleration (ring widths narrow with age in managed oak [T-unv]); removing it would make growth unbounded, replacing it (logistic/Richards) adds parameters the evidence cannot constrain. Boundedness verified to age 512 (T8): `D → 112 cm`, `H → 32.9 m`, never exceeding the soft maxima.

### 3.D Response to competition
* Generic Hegyi + `1/(1 + CI/Ci50)` stays. Oak Ci50 = 3 has no oak-specific calibration [U]; ordinal rank: oak is intermediate-to-light-demanding, i.e. *less* competition-tolerant than beech [ORD, strong]. The existing juvenile light curves already encode this at the regeneration stage. Adult Ci50 differentiation is **deferred** (it has seven consumers, F7) — OD-6.

### 3.E Response to site
* Ordinal [T†]: sessile oak tolerates less rich and lighter-textured soils than pedunculate oak; oaks produce poor timber on excessively drained/sandy soils; Irish guidance calls oak "accommodating in regard to site" with "excellent stability in wind" (IFA species guide; COFORD *Establishing Broadleaves*). UK NFI: oak yield class varies widely with rainfall, exposure, altitude and soil [T†]. Magnitudes: none → ordinal classes only.

### 3.F Age range relevant to Scenario One
* Oak/Beech enter as planted stock or natural recruits after Year 0 (the starting stand is pure Sitka age 20). Adult age therefore runs from **≈ 12 (promotion) to ≈ 102–105 at the Century Review**; planted-Year-1 trees are ≈ 27 at the Year-25 completion horizon. Certification window: **12–110**; fixture-tested to 160; no claim beyond.

### 3.G Does the calibration create an unrealistic commercial-size ceiling?

*Premium threshold as given in the packet:* ≈ **≥ 45 cm mid-log diameter**. I could not locate its source (not in the repo at `c1564b8`); treated as given.

| Cause | Verdict | Quantification |
|---|---|---|
| **Incorrect growth calibration** | **Yes, partly.** | Current open-grown DBH is 35.3 cm at age 102 and 38.1 at 112 (reproduces the reported 37.8). Reaching 47 cm takes **age 147** open-grown; with CI = 1 it is never reached by 152. The DBH potential is a stand-grown rate used as open-grown potential, then competition-reduced again (P-2). Recalibrated (Pd 0.65): 49.8 cm at 102 open-grown. |
| **DBH vs mid-log mismatch** | **Minor, and in the *wrong* direction for the game** (mid-log < DBH). | Under TimberYield v1's DBH-anchored paraboloid [S], mid-log ≈ 0.94–0.99 × DBH, so 45 cm mid-log needs **DBH ≈ 46–48 cm over-bark**, ≈ **50–52 cm** if 45 cm is under-bark with ~8 % bark [I; no bark model exists, documented in `TimberAssortmentYieldV1.md`] (T6). So the gap in DBH terms is ~9–15 cm, not 7. |
| **Scenario horizon** | **Yes, decisive.** | Completion Year 25 ⇒ planted Oak age ≈ 27, DBH 10–15 cm. Century Review Year 100 ⇒ age ≈ 102: DBH ≈ 30–41 cm (CI 3–1), 50 cm only open-grown. Premium is a 95–150-year outcome even after recalibration. |
| **Premium legitimately beyond Scenario One** | **Yes.** | British practice treats quality oak as a multi-generational crop (recalled 100–150+ yr, [T-unv]); early pole/stake/small-sawlog material is the only oak product inside the horizon. |

**Recommendation for the game.**
1. Fix the calibration defect (P-2/P-3/P-4) so Century-age Oak is plausible — **not** to enable a premium class. Sensitivity (T4) shows the qualitative result is robust to `Pd` anywhere in 0.5–0.8: open-grown 47 cm arrives at age 80–120, competed (CI ≥ 2) at 125 to never.
2. **Do not tune growth, thresholds, prices or the horizon to make ≥ 45 cm mid-log reachable.** Scenario One may end before premium Oak maturity.
3. No premium class, price or objective exists in production (flat €65/m³ broadleaf whole-stem; deferred by D-044). Keep it that way. Completion and objectives must not depend on Oak size (they count presence).
4. When a broadleaf deck is later specified: grade on **mid-log diameter from `MerchantableStemModel`**, with an explicit bark basis; let the premium class exist in data but expect it empty in Scenario One; consider a Century-Review line such as "Oak crop trees within X cm of sawlog size — estimated years to size" (teaching: oak is the legacy you leave, not the cash you take). Proposal only.

---

## 4. Beech evidence synthesis (*Fagus sylvatica*)

Beech must not inherit Oak's law merely because both are broadleaves. Behaviours that genuinely differ, ranked by relevance to Scenario One:

| Behaviour | Oak vs Beech | Evidence | Matters in Scenario One? | v1 treatment |
|---|---|---|---|---|
| Soil-moisture tolerance | Beech does not tolerate waterlogging, is drought-sensitive (drier summers cut productivity; wetter winters waterlog); oak is accommodating | [T†] COFORD *Establishing Broadleaves*; Forest Research New Forest ESC presentation | Only off the reference site — **but it is the one robust, evidence-backed contrast** | Site response table (§7) |
| Height trajectory | Beech taller / faster on comparable sites | [T-unv] FC curves (recalled: H50 ≈ 14–19, H100 ≈ 22–29) | Yes: stem volume, canopy top, H/D | Ph 0.50 vs 0.35 |
| Shade tolerance under competition | Beech > Oak | [ORD, strong] | Yes, for suppressed individuals and mixed-stand mortality | **Deferred** (Ci50 seam, OD-6); juvenile curves already differ |
| Fertility preference | Beech prefers base-rich but grows on acid topsoils [T†]; oak accommodating | [ORD, weak] | Barely | Not an active axis (§5) |
| Wind stability | Oak "excellent stability in wind" [T†]; beech shallower-rooted [T-unv] | [ORD] | Storms off by default; `standWindSusceptibility` is 1.0 for both and the storm profile is frozen (D-050) | **Out of scope** — listed as unsupported difference |
| Longevity / senescence | Oak ≫ Beech | [T-unv] | No (beyond 110 yr) | none |
| Crown form, mast, dispersal, browse | already differ | [R] | already modelled | unchanged |

Adult height (T5): current law gives Beech 15.3 / 25.0 / 30.9 m at 50 / 100 / 150; proposal 16.6 / 25.4 / 29.4 — **Beech height barely moves; the current shared law was already close to Beech and wrong mainly for Oak.** Beech DBH uses the same double-count correction as Oak (Pd 0.65). Beech parameters in the repo are classed B/C with primary evidence unseen (`BeechSpeciesParameters.md`); the 0.3–0.5 cm/yr and 0.3–0.5 m/yr "good-site young" ranges are the only Beech-specific numbers available.

---

## 5. Site evidence synthesis

**Framework [T†].** The Forestry Commission's Ecological Site Classification (Pyatt, Ray & Fletcher 2001, FC Bulletin 124) scores sites on four climatic factors — accumulated temperature (AT), moisture deficit (MD), windiness (DAMS), continentality — and two soil factors — soil moisture regime (SMR) and soil nutrient regime (SNR) — and judges suitability by the **limiting factor** (a good factor cannot compensate an unsuitable one). An Irish COFORD note quoting ESC says marginal sites can support productive broadleaves only at a relatively low yield class (< 6) [T†]; the same note said an Irish ESC-equivalent was then being developed (CLIMADAPT) [T†] — its current status was not checked. This gives a defensible template — *generic ordinal axes, per-species tolerances* — without implying Irish coefficients.

| Candidate axis | Separates Sitka / Oak / Beech in Scenario One? | Evidence | State cost | Double-count risk | v1 decision |
|---|---|---|---|---|---|
| **Soil moisture / drainage (SMR)** | **Yes** — Beech intolerant of waterlogging *and* drought, Oak between [T†]; Sitka's wet tolerance is general knowledge [T-unv] and is not modelled in v1 | [T†]/[ORD] | one small ordinal, derivable | Low (no other growth term uses moisture) | **INCLUDE — the single active axis** |
| Soil fertility / nutrient (SNR) | Weakly (Oak vs Beech contrast is ordinal-weak; Sitka tolerant) | [ORD] weak | one small ordinal | Low | **Defer.** Add as a second table when a scenario needs it. Absent from v1 so no unsupported numbers are introduced |
| Exposure / wind (DAMS) | Strongly (height growth, form, windthrow) | [T†] | one ordinal | **High** — storm site/edge terms are frozen (D-050), edge bias, H/D | **Exclude from growth;** storm model owns exposure |
| Temperature / climate (AT, MD, CT) | No within one Irish property; matters *between* scenarios | [T†] | scenario constants | none | **Defer** to a multi-scenario climate profile |
| Soil constraints (depth, stoniness, compaction) | Rooting-dependent | none usable | — | — | **Exclude** |
| Light / canopy / neighbours | — | — | — | **Would duplicate Hegyi/canopy** | **Not a site axis** (explicit anti-double-count rule, §6.3) |

**Honest consequence.** Scenario One is *one static site*. The reference class must be declared (OD-1); species parameters are calibrated *for that site*; the response is normalised so the reference is 1.0 for everyone. Species × site therefore changes nothing observable in Scenario One by construction. Its v1 value is (a) a tested, versioned seam for Scenario Two, (b) fixture tests of "favourable vs poor", and (c) the option for the Manager to declare a non-neutral Scenario One reference (e.g. seasonally wet) so that Beech visibly struggles.

---

## 6. Recommended growth architecture

### 6.1 Options compared

| Criterion | **A** Species-specific age curves (Sitka-style envelopes per species) | **B** Shared generic broadleaf kernel + species parameter rows | **C** Existing curve × species/site modifiers | **B+** (recommended) = B + generic site modifier, Sitka untouched |
|---|---|---|---|---|
| Ecological defensibility | Highest *if* site-index curves exist; here transferred and unverified ⇒ false precision | Medium; honest about being a transferred, calibrated envelope | Low: hides the double count behind multipliers | Medium; transparent |
| Determinism | Fine | Fine (no RNG) | Fine | Fine |
| Calibration burden | High (per species × outcome; age-indexed) | Low (5 numbers/species) | Medium (modifiers stack on a miscalibrated base) | Low |
| Code complexity | Highest (a class per species) | Low (one kernel, one table) | Low | Low–medium |
| Future species | Linear code growth | **A table row** | Table row but stacking modifiers | **A table row + a response row** |
| Pure-Sitka compatibility | Wraps cleanly | Wraps cleanly | Wraps | **Wraps; Sitka path byte-identical** |
| Testing | Per-species curve suites | One kernel suite + rows | Awkward (modifier interactions) | One kernel + one response suite |
| Save impact | None beyond version value | None beyond version value | None | None beyond version value |
| Scenario One teaching value | Opaque if curves are fitted | **Legible:** "Oak grows slower and tops out lower than Beech; Beech hates wet feet" | Obscure | Legible |
| Known weakness | Age-indexed envelopes freeze advance-regeneration/suppressed trees at their promotion-time fraction of the envelope (no catch-up after release) | Same-form for all broadleaves | Inherits P-1–P-4 | Same-form for all broadleaves |

**Recommendation: B+.** It fixes the diagnosed defects with the fewest new claims, keeps one legible law for a learner to maintain, keeps recorded age out of the broadleaf kernel (so advance regeneration released from shade catches up along the guide curve instead of staying a fixed fraction of an age-indexed envelope), needs no asset or scene edit, and slots a future third species in as a row. Option A is rejected for v1 because it would require age-indexed Irish/British site-index curves that could not be retrieved; it remains the upgrade path if Irish oak/beech top-height models are later supplied. Option C is rejected because multipliers on an uncorrected base conceal the double count.

### 6.2 The kernel (specification, not code)

For a living Oak or Beech tree under the new growth model, each annual step in `GrowAdults` computes, in this order, using the tree's *current* DBH `D`, height `H`, competition index `CI` (from the existing `UpdateCompetition`) and site factor `s`:

```
s  = cell.SiteProductivity × SiteResponse(species, scenario site profile)     // §7; 1.0 at the reference site
ΔD = Pd(species) · s · clamp01(1 − D / Dmax(species)) · 1 / (1 + CI / Ci50)   // Ci50 = existing asset value
ΔH = Ph(species) · s · clamp01(1 − H / Hmax(species))                          // no CI term, no age term
tree.ApplyGrowth(ΔD, ΔH);  tree.SetAgeYears(age + 1)                          // unchanged bookkeeping
```

Everything else — Hegyi, `RecordSuppressionYear`, crown relaxation, density mortality, annual order (`AdvanceOneYear`, `:470–`) — is unchanged. `Pd, Dmax, Ph, Hmax` come from a code table keyed by species ID, selected only when `growthModel ≥ <NEW>` **and** the species has a row; any species without a row (including Sitka) keeps its existing path.

### 6.3 What stays generic, what adult growth adds, and the double-count rules

* **Stays generic (unchanged):** Hegyi neighbourhood (8 m cutoff), the `1/(1 + CI/Ci50)` form acting on DBH only, no competition in height, crown relaxation, suppression history, SDI mortality, juvenile light/browse/vegetation losses.
* **Adult growth adds:** (i) species rows; (ii) a site factor from a *soil-only* axis; nothing else.
* **Rules that prevent double counting (to be asserted in review):**
  1. `Pd` is *by definition* the zero-competition (open-grown) potential at the reference site; stand-grown literature rates must be divided by the competition factor before use (this corrects the existing double count).
  2. The site axis is soil moisture only — no light, canopy, neighbour or density content.
  3. Site never scales `CI`, `Ci50` or the SDI line.
  4. `cell.SiteProductivity` remains the non-species scalar owned by juvenile growth/understorey (value 1 in v1). If per-cell site variation is ever introduced, derive a cell moisture class and **replace** the product for broadleaf adults rather than multiplying both.
  5. Site response is *not* applied to juveniles in v1 (§10, L-3): juvenile growth already multiplies `cell.SiteProductivity`, and the Model-2/regeneration anchors must not move.

---

## 7. Recommended site architecture

### 7.1 Data model
* `SiteProfile`: one ordinal `SoilMoistureClass ∈ {1…5}`. A second field (`NutrientClass`) is **not** added in v1.
* `SpeciesSiteResponse`: pure static lookup `(speciesId, profile) → float` from a per-species 5-entry table of ordinal response classes, **normalised to the profile's reference class** so the reference returns exactly `1f` (use an early-out for `profile.IsReference` to guarantee bit-exactness, not a division).
* Sitka has **no row**: `UsesSiteAxes = false`, returning 1 and bypassing the lookup entirely.
* Moisture classes (collapsed from ESC's eight SMR classes — mapping approximate; verify against Bulletin 124 [T†]):

| Class | Meaning | Rough ESC SMR | Typical Irish soil |
|---|---|---|---|
| 1 | Droughty / excessively drained | very/moderately dry | shallow or sandy brown earth/podzol |
| 2 | Free-draining, slightly dry | slightly dry | deep well-drained brown earth |
| **3** | **Fresh–moist, well to moderately drained (reference)** | fresh–moist | brown earth / podzolic |
| 4 | Imperfectly drained, seasonally wet | very moist–wet | surface-water gley |
| 5 | Waterlogged / peaty | wet–very wet | peaty gley / peat |

### 7.2 Where the data lives (§D of the packet)
| Option | Verdict |
|---|---|
| A. scenario-definition data only | **Chosen.** A `[Serializable]` profile field on `ScenarioOneDefinition` whose code initializer is the reference class; an old `ScenarioOne.asset` YAML lacking the key deserialises to the default, so the asset need not be re-saved. Alternatively a plain code constant for Scenario One. |
| B. saved persistent state | **Rejected for v1.** Redundant: the site is static and `scenarioOne.scenarioId` is already saved. |
| C. derivable from scenario identity | **Yes — this is how it works.** `scenarioId` (saved) → definition → site. |
| D. combination | Only if a future scenario lets the player alter/choose a site; then persist a `siteModel`/profile id (a save bump at that time). |

Injection point: `ScenarioOneManager` sets `ecology.SiteProfile = definition.Site` beside `ApplyBrowsingConfiguration` (`:416–422`), re-applied on every load. Non-scenario scenes (`ForestTest`, `MixedSpeciesTest`) keep the default (reference) profile ⇒ response 1. **Guard:** any future change to Scenario One's profile needs a new definition version *and* a new growth-model value, otherwise old saves silently re-interpret; document this in the definition's tooltip.

### 7.3 Application points
`GrowAdults` (above) and the marking-manager growth forecast (`ForestTreeMarkingManager.cs:379–397`) must call **one shared helper** (`GetAdultSiteFactor(tree)`), otherwise the forecast would show growth under different parameters than the simulation. The forecast is a *percent change*, so the discrepancy would be small but is avoidable.

---

## 8. Exact proposed v1 parameters

**Provisional.** All values are [C] chosen inside transferred bands (see §9); they must be re-confirmed or refitted in Step 0. Regenerate every table with `Evidence/reproduce_tables.py` after editing its `PARAMS` block.

### 8.1 Adult growth rows (code-held, versioned, species-keyed)

| Species | `Pd` DBH potential (cm/yr, zero competition, reference site) | `Dmax` soft DBH scale (cm) | `Ph` height potential (m/yr as H→0) | `Hmax` soft height ceiling (m) | `Ci50` |
|---|---:|---:|---:|---:|---:|
| `sessile-oak` | **0.65** | 120 (unchanged) | **0.35** | **33** | 3 (asset, unchanged) |
| `beech` | **0.65** | 120 (unchanged) | **0.50** | **33** | 3 (asset, unchanged) |
| `sitka-spruce` | *no row — existing path* | | | | 5 (asset) |

Permitted refit band, to be applied as pairs: `Pd` 0.5–0.8; Oak `(Ph, Hmax)` from (0.24, 34.5) to (0.46, 34.4); Beech from (0.38, 31.7) to (0.60, 37.1) — i.e. the pair is **refit from two anchors (age 50, age 100), never nudged singly** (T5 gives the closed-form-by-search refit).

### 8.2 Site response (moisture axis; normalised to reference class 3)

Response classes [C]: **Optimal 1.0 · Good 0.8 · Moderate 0.6 · Poor 0.4 · Very poor 0.2** (five named values, no further decimals).

| Moisture class | 1 droughty | 2 slightly dry | **3 reference** | 4 seasonally wet | 5 waterlogged |
|---|---:|---:|---:|---:|---:|
| Oak | Moderate 0.6 | Good 0.8 | Optimal 1.0 | Moderate 0.6 | Poor 0.4 |
| Beech | Poor 0.4 | Good 0.8 | Optimal 1.0 | Poor 0.4 | Very poor 0.2 |
| Sitka | not modelled in v1 (1.0 everywhere) | | | | |

Declared ordinal claims the tests must encode: Beech ≤ Oak at classes 1, 4, 5; Beech = Oak at 2, 3; both peak at 3; no class below 0.2 (no zero growth).

### 8.3 Reference site (Scenario One)
`SoilMoistureClass = 3` — **[I], Manager decision OD-1.** Nothing in the repo classifies the Scenario One soil; Class III Sitka occurs across the moisture range. Class 3 means "do not claim Oak/Beech are handicapped on this site", consistent with the scenario's broadleaf-introduction objectives. If the Manager prefers class 4 (the more typical Irish Sitka situation), Beech's absolute calibration must be re-derived downward, because the normalisation makes the parameters describe *the reference site*.

### 8.4 Expected outputs (planted Year 1 ⇒ age 27 / 52 / 77 / 102 at Years 25 / 50 / 75 / 100)

DBH cm / height m (full tables: `Evidence/reproduce_tables_output.txt` T2; oracle: `Evidence/expected_trajectories.csv`):

| Law | CI | Yr 25 | Yr 50 | Yr 75 | Yr 100 |
|---|---:|---|---|---|---|
| Current (Oak = Beech) | 0 | 11.2 / 8.8 | 19.9 / 15.7 | 27.9 / 21.1 | 35.3 / 25.3 |
| Current | 2 | 9.0 / 8.8 | 14.4 / 15.7 | 19.6 / 21.1 | 24.5 / 25.3 |
| Current | 5 | 7.8 / 8.8 | 11.2 / 15.7 | 14.6 / 21.1 | 17.8 / 25.3 |
| **Proposed Oak** | 0 | 14.6 / 8.1 | 28.0 / 13.9 | 39.6 / 18.4 | 49.8 / 21.8 |
| Proposed Oak | 1 | 12.4 / 8.1 | 22.8 / 13.9 | 32.2 / 18.4 | 40.7 / 21.8 |
| Proposed Oak | 2 | 11.1 / 8.1 | 19.6 / 13.9 | 27.4 / 18.4 | 34.7 / 21.8 |
| Proposed Oak | 3 | 10.2 / 8.1 | 17.4 / 13.9 | 24.1 / 18.4 | 30.4 / 21.8 |
| Proposed Oak | 5 | 9.1 / 8.1 | 14.6 / 13.9 | 19.8 / 18.4 | 24.8 / 21.8 |
| **Proposed Beech** | 0 / 2 / 5 | as Oak DBH | as Oak DBH | as Oak DBH | as Oak DBH |
| Proposed Beech height | any | 9.7 | 17.1 | 22.2 | 25.6 |

Reading: under a Sitka canopy (CI ≳ 5–10) broadleaves stay small, as intended; in a thinned mixed stand (CI 1–3) Oak/Beech are 30–41 cm at the Century Review; only crowns with little competition approach 50 cm.

---

## 9. Evidence / calibration table

Columns: species · parameter · value · units · status · source basis · confidence · calibration allowed? · expected gameplay effect.

| Species | Parameter | Value | Units | Status | Source basis | Confidence | Calibration allowed? | Expected gameplay effect |
|---|---|---|---|---|---|---|---|---|
| Oak, Beech | `Pd` zero-competition DBH potential | 0.65 | cm/yr | **[C]** | [R] repo range 0.3–0.5 cm/yr (`BeechSpeciesParameters.md`, primary unseen) ÷ model competition factor at managed CI 1.5–3 (computed, T3); cross-check [T-unv] open-grown girth rule of thumb | Low–moderate | **Yes, 0.5–0.8** | Managed broadleaves reach 30–41 cm at Yr 100 instead of 18–29; premium timing robustly > 95 yr |
| Oak, Beech | `Dmax` soft DBH scale | 120 | cm | [C] (unchanged) | [R] asset; not a biological limit | n/a | Keep | Deceleration only; bounded growth |
| Oak | `Ph` | 0.35 | m/yr | **[C]** within [T-unv] | Pair-fit to recalled FC oak top-height band (H50 ≈ 11–16, H100 ≈ 18–25); [T†] "pedunculate oak YC 4–8" | Low–moderate | **Yes, as a pair** (§8.1) | Oak 21.8 m at Yr 100 vs 25.3 now; shorter, slower canopy top |
| Oak | `Hmax` | 33 | m | **[C]** within [T-unv] | as above | Low–moderate | Pair only | Oak stops looking like a 30 m+ tree inside the horizon |
| Beech | `Ph` | 0.50 | m/yr | **[C]** within [T-unv] | Pair-fit to recalled FC beech band (H50 ≈ 14–19, H100 ≈ 22–29); repo Beech range 0.3–0.5 m/yr | Low–moderate | Yes, as a pair | Beech 25.6 m at Yr 100 (≈ unchanged), clearly taller than Oak |
| Beech | `Hmax` | 33 | m | **[C]** within [T-unv] | as above | Low–moderate | Pair only | as above |
| Oak, Beech | `Ci50` | 3 | Hegyi index | [R]/[C] (unchanged) | asset; no species-specific calibration [U]; ordinal Beech ≥ Oak [ORD] | Low | **Deferred** (needs a seam, OD-6) | none in v1 |
| Oak, Beech | Height has no CI / age term | — | — | [C] | Consistent with accepted Sitka direction (D-048 "no competition term in height"); keeps advance regeneration catch-up | Moderate | Fixed in v1 | Suppressed trees are short in DBH, not slender-by-design; see L-1 |
| Oak, Beech, Sitka | Kernel is age-independent | — | — | [C] | Avoids age-indexed envelope artefacts for mixed-age recruits | Moderate | Fixed | Promoted late recruits are not penalised by age |
| Oak | Moisture table (0.6, 0.8, 1.0, 0.6, 0.4) | 5 classes | × | **[ORD]→[C]** | [T†] sessile tolerates lighter, poorer soils; "accommodating"; poor timber on excessively drained soils (IFA guide; COFORD) | Ordering medium; magnitudes low | **Yes** | Inert at reference; stunting off-reference |
| Beech | Moisture table (0.4, 0.8, 1.0, 0.4, 0.2) | 5 classes | × | **[ORD]→[C]** | [T†] intolerant of waterlogging; drier summers cut productivity (COFORD; Forest Research) | Ordering medium-high; magnitudes low | **Yes** | Inert at reference; strongest species contrast |
| — | Response classes O/G/M/P/V | 1.0/0.8/0.6/0.4/0.2 | × | [C] | Equal ordinal steps; no decimals beyond five names | Low | Yes (the five values) | Legibility |
| — | Reference moisture class | 3 | class | **[I]** | No Scenario One soil classification in repo | n/a | **Manager (OD-1)** | If ≠ 3, rederive Beech baseline |
| — | Combination rule (single axis) | — | — | [T†] ESC limiting-factor logic | Pyatt et al. 2001 | Medium | When a 2nd axis is added: use `min`, not product | — |
| — | `PlantedJuvenile` age/height, promotion DBH `1.5·H` | 3 yr / 0.6 m / 5.6 cm | | [R]/[C] (unchanged) | existing | n/a | Not touched | defines the start state of every trajectory |

No row is [E].

---

## 10. What remains unsupported

* **[U] Irish dynamic top-height or DBH models for adult sessile oak or beech.** The transferred bands are wide, recalled, and unverified here.
* **[U] Oak-specific adult DBH growth**; the only Beech figure is a young/stand range of unseen primary origin.
* **[U] Species-specific Hegyi tolerance (`Ci50`)** — only an ordinal Beech ≥ Oak.
* **[U] Any magnitude for the site responses** — ordinal ranks only.
* **[U] Fertility, exposure, climate and soil-depth effects** (axes deferred, not modelled).
* **[U] Longevity/senescence, bark thickness (premium mid-log under-bark), butt swell, stem form, and sawlog quality** — TimberYield v1 is a DBH-anchored paraboloid with no bark model.
* **[U] Wind-stability difference** between Oak and Beech.
* **L-1 (limitation).** Adult height ignores light and competition, so a tree promoted at 20 % light steps from ≈ 0.1 to ≈ 0.31 m/yr (T9), and suppressed whips become slender (H/D > 100 at CI ≥ 10). Pre-existing; the proposal does not worsen it (at CI 10, age 52: current DBH/H 9.1/15.7 → proposed 11.2/13.9). Optional later extension: a light factor for H < ~8 m (the canopy light model saturates at 8 m). Gate on evidence.
* **L-2.** Mixed-species density mortality applies Sitka's SDI line to Oak/Beech (F11). Higher broadleaf DBH raises the stand's Dq and slightly raises density pressure; effect is small while broadleaves are few, but must be *reported* (§13, E12). The fix belongs to the separate density work, not here.
* **L-3.** Site response is applied to adult growth only. A Beech planted on a waterlogged class will establish and promote normally, then stagnate; it will not fail to establish or die of site. Extending the same table to juvenile height growth (the `siteProductivity` argument already exists) is a candidate follow-up, but it moves regeneration anchors and needs its own packet.
* **L-4.** Open-grown trajectories assume the promotion state of planted stock (age 12, 3.75 m). Naturally recruited and shade-delayed trees promote later at 3.5 m; because the kernel ignores age they follow the same height-versus-DBH curve shifted in time.

---

## 11. Save / version implications

* **No new persisted field.** `growthModel` (v17) is already saved as an int and identifies the law; the site is derivable from `scenarioId` + definition; `cell.SiteProductivity` stays unsaved.
* **One new `GrowthModel` value** (name: `GrowthModel.<NEW>`, numeric value allocated by the Manager — OD-3). Required edits: `GrowthModel.Latest` (so `Normalize` and `ForestSaveValidation.cs:80` accept it) and, at the default-flip step only, `ScenarioOneManager.NewGameGrowthModel` (`:472`). `UsesSiteClassGrowth` is `≥ 1`, so Sitka's Class III height and density mortality remain active in the new model: the new value is a strict superset of model 1.
* **Compatibility.** Saves with growthModel 0 or 1 and an absent field load exactly as before (asset constants untouched); Reference Future v1 stays model 0; in-progress v20 games stay on model 1 with the old Oak/Beech curves (no migration — precedent D-047/D-048). `ScenarioReferenceArchive.LegacyV15/V16WorldHash` strip only `"growthModel":0,` and are unaffected.
* **Older builds** loading a save with the new value reject it via the `> Latest` check (graceful refusal); a `CurrentVersion` bump 20→21 would add only a warning and is **not recommended** (OD-9) unless the Manager wants an explicit marker — it would force churn in version-pinned harnesses with no schema change to justify it.
* **Anchors.** `growthModel` is serialised, so flipping the new-game default changes **every** JSON-hash anchor of the new-game stack (start, year-N, completion) even for a pure-Sitka world. Therefore: ship dormant (selectable by `CCF_GROWTH_MODEL`-style override and by harness), verify, and flip the default as a *separate, explicitly anchored* decision (OD-2). Pure-Sitka equality must be tested on state *excluding* the version field (§13).

---

## 12. Pure-Sitka compatibility strategy

1. **Wrap, don't rewrite.** The new branch in `GrowAdults` is guarded by `growthModel ≥ <NEW>` **and** `species has a row`; Sitka has none ⇒ the existing `SitkaGrowthModel.NextHeight` path and the unmodified DBH expression run verbatim. No constant in `SitkaGrowthModel` changes; the file is touched only to add the `GrowthModel` constant.
2. **Neutral site by construction.** `UsesSiteAxes = false` for Sitka, and the reference-class early-out returns the literal `1f`; multiplication by 1f is exact in IEEE arithmetic, and Sitka's path does not multiply it at all.
3. **Fallback is the legacy law.** A species with no row (any future species) keeps asset constants — never silently inherits Oak/Beech.
4. **Remaining coupling, stated honestly.** Sitka trees feel Oak/Beech only through (a) Hegyi neighbours within 8 m (DBH ratios), (b) canopy light and seed rain (juvenile/regeneration side), (c) the stand SDI used by density mortality. "Pure Sitka" equality therefore means: *no living or juvenile Oak/Beech ⇒ state identical to growth model 1 apart from the serialised version field*; and *with Oak/Beech > 8 m from a Sitka tree ⇒ that tree's adult DBH/height series identical*.
5. **Mixed-stand effect is intended and attributable.** Scenario One anchors containing promoted Oak/Beech will move when the default flips; the attribution check is that every difference traces to Oak/Beech state or their Hegyi/density neighbours.

---

## 13. Verification specification

Adopt the model only when all pass. Oracle for numeric cases: `Evidence/expected_trajectories.csv` (regenerate after Step 0). Pure-kernel cases can run in plain .NET like `run_timber_assortment_yield_verification.py`; ecology cases need Unity batch (follow the documented copy/run/remove harness workflow, back up `forest-save.json`, never leave an auto-start script in a build). **Tolerances:** double-precision oracle vs float32 production — 1e-3 relative or 0.05 cm / 0.05 m absolute.

| ID | Group | Test | Pass criteria |
|---|---|---|---|
| K1 | Pure species | Oak and Beech recurrences over species × CI {0,1,2,3,5,10} × moisture classes {1–5} | match oracle within tolerance |
| K2 | Boundaries | Monotonicity over the full grid | `ΔD ≥ 0`, `ΔH ≥ 0` every year; min increments > 0 (T8: ≥ 0.04 cm, ≥ 0.0002 m at site 1.0) |
| K3 | Boundaries | 500-year runs, site {0.2, 1.0} | `D < Dmax`, `H < Hmax`; no NaN/∞; max annual `ΔD ≤ Pd`, `ΔH ≤ Ph` |
| K4 | Site | Response table integrity | values ∈ {0.2…1.0}; peak at class 3; Beech ≤ Oak at classes 1, 4, 5 and = at 2, 3; reference returns exactly `1f` |
| K5 | Boundaries | Age independence | perturb recorded age ±10 ⇒ identical growth |
| K6 | Sitka | Row lookup | Sitka and unknown IDs return "no row" |
| E1 | Pure species | **Oak open-grown** — one isolated tree, 100 yr, ecology loop | equals oracle (CI 0) |
| E2 | Pure species | **Oak competed** — fixed neighbour ring ⇒ CI ≈ 2; recompute kernel from the *recorded* CI each year | residual ≤ tolerance; Oak DBH(102) ∈ [33, 45] at CI 1.5 |
| E3 | Pure species | **Beech open-grown** | equals oracle |
| E4 | Pure species | **Beech competed** | as E2 |
| E5 | Site | **Oak: favourable vs poor** (class 3 vs 5; also 1, 2, 4) via injected profile | DBH/height ratios match `R(species, class)` within tolerance; class 3 ≡ no-profile run **bit-exact** |
| E6 | Site | **Beech: favourable vs poor** | as E5; Beech(4), Beech(5) growth < Oak(4), Oak(5) |
| E7 | Sitka | **Canonical site neutral** — pure Sitka Scenario One start stand, 25 yr, every year | state hash excluding the version field identical to growth model 1 |
| E8 | Sitka | Sitka beside far (> 8 m) Oak/Beech, 50 yr | Sitka adult DBH/height series identical to growth-1 run |
| E9 | Mixture | **Deterministic mixed stand** (Sitka + Oak + Beech, ≥ 100 yr), two independent processes | identical hash; species IDs preserved each year; per-tree residual vs the species' own kernel = 0; **no Sitka curve leakage**: Oak/Beech height ≠ `SitkaGrowthModel.NextHeight` path and equals the Mitscherlich row |
| E10 | Boundaries | **Promotion/age transition** — spawn/promote at 3.5 m, record before/after | DBH and height continuous at hand-off; log height-increment ratio (T9) and alarm if > 4×; no jump in DBH |
| E11 | Boundaries | No abrupt jumps | max single-year `|ΔD|` ≤ `Pd`, `|ΔH|` ≤ `Ph` for every tree, every year, across the mixed run |
| E12 | Diagnostic | Mixed-density interaction | report Oak/Beech "self-thinning" deaths by species before vs after; **no gate**, Manager input to the density work |
| E13 | Save | Save at year 30, reload, continue vs uninterrupted | identical; `growthModel` round-trips; growth-1 save loads as 1 and Oak/Beech follow the **old** law (hash equal to pre-change build); out-of-range value rejected |
| E14 | Legacy | Growth 0/1 and Reference | all MUST-NOT-CHANGE anchors below exact |
| E15 | Scenario | Completion gate on the new-game stack with the new model (via override) | completes by Year 25, all objectives, solvent; minimum cash change vs previous **reported** (not gated); objectives never read Oak/Beech size |
| E16 | Scenario | **Premium need not be achieved** | Century Review runs; Oak/Beech DBH distribution reported; no assertion that any tree reaches 47 cm; at Year 25 planted Oak DBH ∈ [8, 16] cm |
| E17 | Performance | Annual step time, 336 and 1,344 trees | no measurable regression (kernel is O(n), table lookups) |

**Provisional acceptance bands (replace after Step 0):** Oak open-grown H50 ∈ [11, 16], H100 ∈ [18, 25]; Beech H50 ∈ [14, 19], H100 ∈ [22, 29]; Beech height ≥ Oak height at every age ≥ 25 [ORD/T-unv]; stand-grown increment at 20 cm DBH, CI 1–3 ∈ [0.25, 0.50] cm/yr; open-grown first age ≥ 47 cm ≥ 80 years.

**MUST-NOT-CHANGE anchors (growth ≤ 1 / pure Sitka):** Reference Future v1 Year 100 `7AD177B3CC2F73C7`, continuation `9CDF21A541C5968D`, schedule `56C8B99FA1E8DDD1`, Y20 `F7DF7DAB53B6FD32`, Y50 `D5E75D6D21D631AC`; legacy canonical `BFC55473C1506067` / `3485B6630C9EA448` / `568922E1A6D73CDD`; growth-1 `7E57B9DAEF5BF4D0` / `35E2BF1C2F55C2DE` / `D7C4DDD36B53FCCE`; Model-2 `FA855239CDDA32D8` / `02334804F65C0234` / `702766DECE591E21`; Enlarged80 start `04A78188A6F8E6F9`, Year 3 `081DB58CE3034648`; P2 `F58FB0B1A421D28B`, P3 `F7C2FC966816BBAD`. (All are produced under growth ≤ 1, so they hold while the new model is dormant.)

**INTENTIONALLY CHANGED (only when the default flips or a fixture selects the new model):** every new-game-stack JSON-hash anchor (version field) and any fixture containing promoted Oak/Beech. Each change must be recorded with an attribution check (E7/E8).

**Harness contracts expected to need edits (not regressions):** `SitkaGrowthModelVerification.cs:96` (asserts the new-game constant); `ScenarioOneCompletionVerification.cs` (`CCF_GROWTH_MODEL` handling). `CCFOakVerification` `:91–92` and the Beech/Oak harnesses should **not** change, because the species assets are untouched.

---

## 14. Implementation sequence (bounded steps for the next worker)

| Step | Work | Files (likely) | Notes / conflict risk |
|---|---|---|---|
| **0** | **Evidence reconciliation (non-code, hard gate).** A worker with the PDFs confirms or replaces the four anchors (Oak H50/H100, Beech H50/H100), the managed-stand DBH range, the Hegyi CI-at-managed-density assumption and the moisture ordinal claims; re-run `reproduce_tables.py`; Manager answers OD-1, OD-3, OD-4. | `Evidence/*` only | Blocks frozen values, not Step 1 |
| **1** | Infrastructure: pure `BroadleafAdultGrowth` (rows + kernel + provenance comments), pure `SiteProfile` + `SpeciesSiteResponse`; `GrowthModel` constant/`Latest`; pure K1–K6 harness (.NET runner). **No ecology wiring.** | NEW `Assets/ForestPrototype/BroadleafAdultGrowth.cs`, NEW `…/SiteProfile.cs`; EDIT `SitkaGrowthModel.cs` (the `GrowthModel` class only); NEW `Tools/Verification/…` | Low risk; mostly new files |
| **2** | Oak wiring: gated branch in `GrowAdults`; `GetAdultSiteFactor`; Oak row only | EDIT `ForestEcologyController.cs` (`:993–1024`, `:1861`) | **Highest conflict**: central file also touched by density, storm and regeneration work — take a lock |
| **3** | Oak verification E1, E2, E7–E11, E13–E14, E16 | NEW harness | |
| **4** | Beech row + verification E3, E4, E9 | table row; harness | |
| **5** | Site response: profile on `ScenarioOneDefinition`, injection in `ScenarioOneManager`, shared helper used by the marking forecast; E5, E6 | EDIT `ScenarioOneDefinition.cs`, `ScenarioOneManager.cs` (`:416–422`), `ForestTreeMarkingManager.cs` (`:379–397`) | `ScenarioOneManager.cs` is a high-contention file |
| **6** | Integrated verification: E12, E15–E17 under the explicit override; document changed-anchor attribution | harness; docs under `Docs/Verification/…` | |
| **7** | **Separate Manager decision:** flip `NewGameGrowthModel`; update `Docs/Project/*` (Manager-only) | `ScenarioOneManager.cs:472` | Changes all new-game JSON-hash anchors |

**Not touched by any step:** species `.asset` files, scenes, prefabs, `ScenarioOne.asset` (default initializer suffices), `ProjectSettings`, `Packages`, save schema, `ForestSaveValidation.cs` (Latest-driven), `TreeSpeciesDefinition.cs`, TimberYield, WorkEconomy.

**Ownership hazards to avoid in parallel:** `ForestEcologyController.cs` and the `GrowthModel` class are also natural homes for the separate mixed-density work (`ApplyAdultDensityMortality`, `StandRelativeDensity`) and for any version-number allocation; serialise those or fix the interface (a pure kernel in its own file) first.

---

## 15. Explicit non-goals

No production code in this task. No change to canonical docs, Scenario One objectives, scenario choice, timber prices, TimberYield or WorkEconomy decks. No premium timber made reachable. No DEC/DMC, density-model, mortality or storm change. No Sitka growth change. No save-format change. No new species. No juvenile growth/survival/establishment change and no change to Model-2 understorey or regeneration. No `Ci50` change. No fertility, exposure or climate axis. No UI. No asset, scene, prefab or `ProjectSettings` edit. No conversion of calibration into empirical fact.

---

## 16. Open Manager decisions

| ID | Decision | Recommendation |
|---|---|---|
| **OD-1** | Scenario One reference soil-moisture class | **3** (fresh–moist, "not handicapped"). Class 4 is the more typical Irish Sitka soil but would require a downward Beech re-baseline and makes Beech the "wrong species on wet feet" lesson. |
| **OD-2** | Ship dormant, or flip the new-game default immediately? | **Dormant first;** flip after integrated verification as its own anchored decision. |
| **OD-3** | Growth-model number | Allocate against in-flight work (a density model may already claim "2"). This document uses `<NEW>`. |
| **OD-4** | Who reconciles evidence with the PDFs (Step 0), and may anchors be moved? | Required before freeze; bands in §13 are the editable gate. |
| **OD-5** | Accept re-defining `Pd` as zero-competition potential (0.4 → 0.65)? | Yes — it corrects a double count; conservative alternative `Pd` 0.5 (premium timing slips ≈ 25 years). |
| **OD-6** | Competition-tolerance differentiation (Beech `Ci50` > Oak) | Defer; needs one `CompetitionTolerance(species)` seam covering the seven consumers (F7). |
| **OD-7** | Confirm premium policy: Scenario One may end before premium Oak; no payout, no requirement | **Yes.** |
| **OD-8** | Extend the moisture response to juvenile height growth/establishment/mortality | Separate packet; moves regeneration anchors. |
| **OD-9** | Bump `ForestSaveData.CurrentVersion` for a value-only change? | **No.** |
| **OD-10** | Build species × site now although it is inert in Scenario One? | **Yes, dormant**, because it is cheap, schema-free and needed for Scenario Two — but it can be deferred without harming Scenario One. |
| **OD-11** | Unretrievable context `321791b` | Confirm no decision after D-058 conflicts; otherwise re-run against the correct snapshot. |

---

## Appendix A — Evidence log

**Repository material read (HEAD `c1564b8`):** `AGENTS.md`, `CLAUDE.md`, `Docs/Project/{AgentWorkflow, decision-log, current-milestone, game-brief, research-index, unity-project-overview}.md`, `coordination/active-tasks.md`; `Docs/BeechSpeciesParameters.md`, `BeechBaseline.md`, `TimberAssortmentYieldV1.md`, `Stage1WorkEconomyFoundation.md`, `Assets/ForestPrototype/Docs/SessileOakV1.md`, `ScenarioHabitatPresentation.md`; `Docs/Research/SitkaGrowthMortality/{CurrentModelAudit, SitkaGrowthModel1, Scenario1SiteProductivityDecision, SaveVersioningAssessment}.md`, `UnderstoreyRecruitment/EvidenceReview.md`, `ScenarioOneCompletion/P3ImplementationRecord.md`; production code listed in §2.1.

**Project research not accessible:** CCF Species Evidence Pack v1; CCF Academic Paper Consolidation (and Comprehensive Evidence Edition); Irish Continuous-Cover Forestry Transformation Evidence; CCF Primary Literature Synthesis v1; Irish CCF ecology; Atlantic woodland/rainforest reports; Species-Specific Density; Multi-Species Density & Mixed Occupancy Analysis; European Pro Silva CCF Practice Synthesis (not in the repo); Oak/Beech Species Parity Audit. Drive was searched for the named reports and its shared-context folder listed (context snapshots only, no research). One broad PDF-title query also returned titles of unrelated personal files; none were opened or used.

**Web (summaries only; primary documents not opened; direct fetch blocked by network policy):**
* Forestry Commission Bulletin 124, *An Ecological Site Classification for Forestry in Great Britain* (Pyatt, Ray & Fletcher 2001): https://cdn.forestresearch.gov.uk/2001/03/fcbu124.pdf — six factors, limiting-factor logic.
* Forest Research, New Forest inclosures ESC case study (beech on wet/poor soil; drier summers; waterlogging): https://www.forestresearch.gov.uk/ (via search result "Applying the ecological site classification in the lowlands").
* COFORD, *Establishing Broadleaves*: https://www.coford.ie/media/coford/content/publications/projectreports/cofordconnects/Establishing_Broadleaves.pdf — beech waterlogging/frost/base-rich; oak accommodating, wind-stable; ESC marginal sites < YC 6.
* IFA Tree Species Guide: https://www.ifa.ie/wp-content/uploads/2020/10/TreeSpeciesGuide.pdf — sessile oak tolerates lighter, less rich soils; poor oak timber on excessively drained soils.
* Forest Research NFI Action Oak report: https://cdn.forestresearch.gov.uk/2022/02/fr_nfi_actionoak_report.pdf — wide yield-class variation with rainfall, exposure, altitude, soil; indicative ranges.
* Forest Advisory Service quick guide to broadleaved species: https://fas.scot/downloads/quick-guide-to-broadleaved-tree-species — pedunculate oak YC 4–8.
* FC Booklet 34 *Forest Management Tables (Metric)*: https://cdn.forestresearch.gov.uk/1971/03/fcbk034.pdf; Forest Yield: https://www.forestresearch.gov.uk/tools-and-resources/fthr/forest-yield/how-forest-yield-works/ — identified as the source to open in Step 0.
* Society of Irish Foresters journal result(s) on FC yield tables' reliability for Irish stands since 1953: https://journal.societyofirishforesters.ie/ (specific article not confirmed).

**Other workers:** none consulted. Only four commit subjects of one other branch were seen incidentally (disclosed above).

## Appendix B — Reproduction

`Evidence/reproduce_tables.py` (stdlib only; `python3 -I reproduce_tables.py --csv expected_trajectories.csv`) regenerates `Evidence/reproduce_tables_output.txt` (tables T0–T9 quoted above) and `Evidence/expected_trajectories.csv` (annual DBH/height, species × moisture class {3, 5} × CI {0, 2, 5}, ages 12–172). It mirrors the *form* of `GrowAdults` in double precision. After Step 0, edit only its `PARAMS`/class tables and re-run.
