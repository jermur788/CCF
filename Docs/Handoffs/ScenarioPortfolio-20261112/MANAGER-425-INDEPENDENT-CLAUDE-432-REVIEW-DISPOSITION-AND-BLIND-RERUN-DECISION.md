# Overall Development Manager #425 — disposition of independent Claude #432 and remaining eight-scenario decisions

**Date:** 12 November 2026  
**Manager status:** REVIEW COMPLETE; owner order and added-feature decisions UNACCEPTED / NOT AUTHORIZED FOR PRODUCTION.  
**Independent review source:** https://github.com/jermur788/CCF/blob/docs/claude-eight-scenario-independent-critique-result-20261112-v1/Docs/Handoffs/ScenarioPortfolio-20261112/CLAUDE-INDEPENDENT-EIGHT-SCENARIO-CURRICULUM-CRITIQUE-RESULT.md  
**Current accepted authority:** main D-051–D-054 and D-078; first complete external Scenario One demo under #424, its Rack V2/MFM/multi-year/persistence/frozen-reference boundary and Windows-build personal-playtest gate unchanged. Scenario count **eight** accepted as working long-term Stage 1 target; exact chapter order and mechanics remain proposals.

## 1. Formal review verdict and model provenance

**APPROVED AS A SUBSTANTIVE INDEPENDENT RESEARCH CRITIQUE WITH DISCLOSED PARTIAL ANCHORING**, including acceptance of its **NEEDS CHANGES** verdict on the *prior underlying research*. This is **not** research-author #428's product feature approval and not an approval to start Scenario Two. Actual reviewer reports **Claude Opus 5.5, medium** in a separate Claude Code cloud session. This is not Sonnet 5.5 high. The initial dispatch included the original #428 preferred option B and full A/B/C table, so its before-reading-#428 provisional order is **NOT truly blinded**; it correctly discloses the limitation. The review's provisional order was written before opening the full #428 result, and adds specific source/architecture discoveries, but its ordering preference can still have been primed.

**Why accept the critique despite that limitation:** It supplies an explicit A/B/C/D comparison, concrete per-case overlap, a testable reordering rationale, and five major corrections with source/engine references, without claiming Unity verification, formal Irish growth coefficients or final product acceptance. Its ordering recommendation is a **conditional strategic hypothesis**, not evidence from learner tests.

## 2. Findings confirmed and pending verification

| Critical item | Manager status and consequence |
|---|---|
| **Single save filename and warn-only cross-scenario mismatch** | **DIRECTLY CONFIRMED IN GITHUB SOURCE AT MAIN:** `Assets/ForestPrototype/ForestSaveController.cs` has `SaveFileName = "forest-save.json"`; `Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.cs` near RestoreSaveData line 1481 logs a `Debug.LogWarning` on mismatched `scenarioId` and then continues. Before an additional independently playable scenario, explicit scenario/save isolation and pre-mutation rejection/old-world routing need an approved spec and bounded negative tests. This does **not** imply those risky loads were runtime-reproduced in Unity. |
| **Scots pine, downy birch, common alder source status** | **DIRECTLY CONFIRMED IN GITHUB SOURCE:** `Assets/ForestPrototype/Species/ScenarioTwo/{ScotsPine,DownyBirch,CommonAlder}.asset` exist, are intentionally `HOLD` in critical simulation fields; `Docs/ScenarioTwoSpeciesFoundation.md` says none is simulation ready or registered into a scene/save. Presence of an asset is not a playable-species release. Research ready Scots pine is not the same as an implemented growth curve. Larch candidate absent according to #432, not independently repo-searched in this review. |
| **O4-before-O3/O7 establishment dependency** | **MANAGER DESIGN DERIVATION:** credible ash-dieback reconstitution and windblow reforestation may reuse actual O4 planting/establishment. Makes the proposed D swapping O4 ahead of O3 **meaningfully different from B**, especially to break the O1–O2–O3 marking sequence. This dependency is design-level, not verified code prerequisite; a bounded O3 could defer its reconstitution if approved. |
| **COFORD simultaneous oak nurse example and Scots pine caveat** | **SOURCE-ATTRIBUTED BY #432, NOT YET PRIMARY-PDF MANAGER-VERIFIED:** COFORD 2012 purportedly states Scots pine is an unsuitable simultaneously planted oak nurse under observed conditions, and describes larch/spruce competition risks. Do not claim Scots pine as a proven Irish simultaneous nurse. Need COFORD exact-page/figure readback and real site/species suitability before feature/spec. Specific webinar utterance not transcribed by #430 or #432. |
| **Hazel coppice guidance attribution** | **#432 SOURCE FINDING, NOT NPWS FIRSTHAND VERIFIED:** Teagasc WIS Element 4 supports specified woodland improvement/copice practice, not a named hazel-coppice species claim. NPWS 2017 hazel pages were not checked by #432. Revise the earlier design addendum's source label or check original NPWS before making hazel-stool/cycle rules. |
| **Cost of “optional” coppice** | **VALID DESIGN/ENGINEERING CAUTION:** Optional player choice still requires a real persisted stool/regrowth system and its core tests if interactive. Optionality decreases curriculum compulsion, not automatically feature implementation cost. |
| **Remaining source gaps** | #432 did **not** inspect full NPWS guidelines, Pommerening & Murphy full paper, webinar recording, existing attached ecology PDFs, or Drive/dashboard. Do not claim those covered. Re-run should target the material gaps, not unbounded literature. |

## 3. Manager's provisional decision recommendation — not product acceptance

1. **Order: proposed D** `O1, O2, O4, O3, O7, O5, O6, O8` (Sitka → Working Mixed → New Farm → Neglected Broadleaf → Exposed Upland → Wet Valley → Atlantic Oakwood → Estate) **provisional preference**, pending a genuinely new first-pass-unanchored reviewer and audience gate. D's O4 before O3 can support ash replacement and O7 reforestation; O4 offers a new decision verb after O2. Alternatives: A novice-first for small woodland owners; B if O4's causal establishment is too expensive. No chapter order is approved.
2. **O4 simultaneous fast nurse + intended crop** planted **in the same initial year**, progressively creating shelter and competition, then selectively removing some nurses before maturity: **OPTIONAL LATER O4 LESSON** at concept level, not a mandatory first-release objective. Do not conflate with pioneer-first and later underplanting; no positive nurse bonus without real causal field/crown/site/trait/survival evidence. Owner still must approve when ready.
3. **O3 coppicing:** first O3 release can show overstood former coppice as correctly labelled **authored history/observation only**; no player-cut-and-resprout promise. Put a live, multi-year, persisted coppice/stool system as a **DEFERRED** candidate for later O3 expansion or O8 estate-local working coppice, contingent on relevance and costs. An O6 historic-stool no-cut counterexample is informational unless causal conservation stakes are implemented. No automatic new-coppice code.
4. **Stage 1 fruit/nut:** recognise suitable native/nut-bearing forestry trees/heritage conditions **without crop/fruit/nut yield, orchard processing or produce sales**. A static descriptive recognition card is not a playable species model; choosing a grown/planted nut tree later requires a separately supported simulation-ready species. **Full fruit/nut production stays first Stage 2 slice D-054**, not silently pulled earlier.
5. **O8 estate:** candidate **one authored map/world with 3–4 management compartments and one estate-local budget**; each compartment has real distinct constraints, history and a finite resource choice causing a deliberately untreated problem with reviewable effect. This may reuse single world/map rather than a generalized multi-world/property system. Explicit scenario identity/save separation from O1–O7 remains mandatory.

**Common constraint:** Each playable scenario needs a real inspect→choose/work or hold→authoritative annual update→walk/review→revise loop. Not every historical woodland feature needs a new mechanic; when a feature is an observational example, label it honestly rather than suggesting causal simulated results.

## 4. Why one fresh, staged blind critique is justified

#432 was **Opus medium, not Sonnet high**, and its revealed B preference prevents the claimed provisional order from being genuinely unanchored. The final order controls seven later implementation dependencies and novice pacing. One proportionate **NEW Claude Sonnet 5.5 high-effort or comparably capable truly fresh independent chat** is recommended, with a sealed Stage A source-only order **committed before** Stage B reads the #428 three-order researcher study or #432 critique. No need to rerun identical full repo/evidence coverage or eight Tier-3 gates. Stage A should know only accepted canon and the original eight cases, which necessarily disclose the original source's own numbering; it must not receive B, D, preferred/runners-up, or this Manager verdict in the initial dispatch. Stage B may open research rankings and exact #432 source findings after the stage-A commit, and should challenge the proposed D rather than merely repeat prior scorecards.

**Separate independent reviewer #432's own session is not cleanly blinded if reused**; use a **new conversation**, not a continuation. Ask reviewer to distinguish genuine independent Stage A rationale from Stage B amendments. Source evidence checks narrowly target COFORD original page/figure, NPWS hazel/coppicing management passages and full Pommerening & Murphy/webinar only if accessible, with explicit not-verified flags. Verify structural Git claims statically as needed, no Unity.

## 5. Product-owner choices still open (after blind critique)

| Choice | Current Manager recommendation | Not authorised until accepted |
|---|---|---|
| Final 1–8 order | D provisionally, with A novice-first as meaningful challenge and B engineering alternative | No fixed second-scenario implementation queue |
| O4 simultaneous nurse/target method | Optional later case challenge; year-zero planted together | No compulsory O4 first release / new nurse coefficient |
| O3 coppicing | Historical/observation first; working persisted stool mechanic deferred to later O3 or O8 | No faux regrowth from Fell; no immediate stool save schema |
| Stage 1 fruit/nut | Recognition of suitable species only, no produce; Stage 2 unchanged | No fruit yields or orchard/farm economy |
| O8 estate capstone | One world, 3–4 zones and one budget (proposed game calibration) | No multi-world generic estate/sandbox |

**Additional later technical choices** for ash, wet ground, windblow and invasive persistence belong in their respective approved feature specs after the progression decision; don't bundle detailed biomechanics decisions into curriculum acceptance.

## 6. Management status and next step

- Claimed: GitHub **research and source readback only**. #432 was [HANDED-OFF/CLOSED] and reports one Markdown result on own branch; manager will register formal disposition on issues #432/#425.
- **No changes to main, game code, live task coordination, approved specs, Unity, protected world, or Scenario One external demo.**
- Separate [#429 context/Drive/attachment sync](https://github.com/jermur788/CCF/issues/429) remains READY and unverified; dashboard ingestion likewise.
- **Next:** independent staged blind Claude review using new docs-only claim and packet; then manager evaluates and returns bounded order/additions decisions for explicit user acceptance.
