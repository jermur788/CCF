# Sen-1 #672 — Compact30 owner decision packet, round 1 (bounded)

**DECISION SUPPORT ONLY. NO OPTION BELOW IS CHOSEN. SIGNING, SPEC APPROVAL, IMPLEMENTATION AND NATIVE QUALIFICATION REMAIN SEPARATE GATES.**

Owner-directed follow-on to the reviewed W1 candidate at [`aab606ca`](https://github.com/jermur788/CCF/blob/aab606ca02f3c1099997c140aafe3a4748dfba9e/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261117/Sen-1-672-COMPACT30-COMPLETE-WORLD-CONTENT-SIGNATURE-CANDIDATE-AND-OWNER-GATES-20261117.md). Round 1 contains **five** decisions only: the signing authority and the four choices that unblock the largest share of the world. The remaining OD items (02, 03, 05, 06, 07, 08, 09, 10, 11, 12, 14, 17) are deliberately deferred to a later round; OD-16 (M-A-SCOPE) is still **not open** and is not asked here.

Basis: the world is `WORLD SIGNATURE BLOCKED` with **SIGNED 0 · CANDIDATE 36 · BLOCKED 52** fields and **36 BLOCKED** ecology cells. A recommendation is offered for each item and is **not** a decision: only the owner's recorded answer changes status.

| Item | Value |
| --- | --- |
| Date | 2026-11-17 UTC |
| Prepared by | DeepSeek V4.1 Flash orchestrator A (documentation), on the owner's direct instruction after the #672 handback |
| Branch | `docs/sen-1-672-owner-decision-round1-20261117-v1` |
| Base | `afd5484fc11697e6bde64f99d57d22e98f572f1f` |
| Parent claim / Manager | [#672](https://github.com/jermur788/CCF/issues/672) (closed orchestration) / [#671](https://github.com/jermur788/CCF/issues/671) |
| Scope | One new Markdown. No source, spec, asset, scene, save, Pref, roster, Unity, Ops or Windows action |

## How to answer

Answer each item with a letter. An answer selects a **direction**, not exact authored bytes: exact values, meshes, recipes and witnesses are still produced and reviewed afterwards. "Unlocks" lists which W1 rows stop being BLOCKED in principle; it does not make them SIGNED.

---

## D1 = OD-00 — Who is the authorised content owner

**Question:** whose recorded attestation, over exact bytes, turns CANDIDATE into SIGNED BY AUTHORISED CONTENT OWNER?

| Option | Meaning | Consequence |
| --- | --- | --- |
| **A** | The product owner personally | The owner signs directly; the attestation record names the owner and the exact digest |
| **B** | A named human designee with recorded delegation | Requires a written delegation naming the person, the scope and the expiry; the designee's attestation is recorded the same way |

**Unlocks:** any SIGNED status at all.
**Cannot be released by this answer:** native qualification, save admission, spec approval.
**Not an option:** any AI worker, model, agent or session — including this orchestrator — signing content, or a dossier being treated as a signature.

**Orchestrator note (recommendation, not a decision):** **A** for this first beta. A single named owner keeps the accountability chain short; delegation can be added later without changing anything already signed.

---

## D2 = OD-01 — Terrain and hydrology

**Question:** is the Compact ground a declared flat plane, or a new authored heightfield (optionally with drainage)?

| Option | Meaning | Consequence |
| --- | --- | --- |
| **A** | Declared flat plane `y = 0`, no hydrology layer | Matches the source's flat ground precedent; every computed margin, route distance and G1 clearance stays exactly as reviewed; no terrain bytes to author or hash |
| **B** | Authored heightfield with pinned source bytes, transform and sampling rule, plus optional drainage | Every tree, path anchor and G1 site gains a `y`; slope changes planting aim and the walk witness; **B2's core+verge margin is only +0.1209757 m**, so a heightfield may force site re-checks or re-positioning |

**Unlocks (either way):** W1 rows 9, 28, 29, 32, 60 and the `y` component of the path/G1/player rows.
**Cannot be released by this answer:** collider/walk witness, native slope behaviour.
**Evidence class:** A is owner choice + source precedent (flat `y = 0` in `ForestStartingStand`); B is unsupported until bytes exist.

**Orchestrator note:** **A** for the first beta. On a 30×30 m plot a heightfield adds authoring, hashing and re-verification cost and can invalidate the reviewed clearances, without adding teaching value that the canopy/light lesson needs. Hydrology has no source at all for this world; inventing it would be an ecological claim the project has not made.

---

## D3 = OD-04 — Initial ecology for all 36 cells

**Question:** how are the 36 cells' initial states produced — pin the existing source initialisation, or author a reviewed 36-cell table?

| Option | Meaning | Consequence |
| --- | --- | --- |
| **A** | Pin the installed source model/species parameters, seed and initialisation recipe, applied after the exact H1 omission | Fastest route to complete cells; inherits the existing `[C]`/`[D]` calibrations **as content**, so those calibrations become part of the signed world instead of remaining research knobs |
| **B** | Author and ecologically review an explicit 36-cell table (flora, bramble/bracken, cohorts, browse, deadwood, canopy/light, site drivers) | Maximum control and legibility; needs an ecological author and a reviewed table, and every number becomes a claim someone must stand behind |

**Sub-questions inside D3 (answer with your letter if you have a preference):**
- **Grid edge:** half-open `[−15, +15)` as the source indexer does, or a different boundary rule for the world edge?
- **Dormant site profile:** the dormant `Reference` site profile sets moisture/drainage; should it apply to Compact, or is Compact explicitly neutral?

**Unlocks:** W1 rows 34–43 and 86, and all 36 cells.
**Cannot be released by this answer:** Year-1+ ecology evidence, regeneration/survival witnesses.
**Evidence class:** A is a reproducible recipe but not measured ecology; B is unsupported until authored.

**Orchestrator note:** **A, with the recipe, seed and selectors pinned in the manifest**, and the grid edge fixed at the source's half-open rule. It gives an honest, reproducible starting state quickly, and it keeps the "these are model calibrations, not field measurements" statement explicit in the signed text. Choosing B is defensible but turns the first beta into an ecological authoring project; if you want B, it should be its own bounded task with a named ecological author.

---

## D4 = OD-13 — GQ-07 scene architecture and asset rights

**Question:** is Compact a separate scene/hierarchy, or the shared scene with profile gating — and what is the rights position for any new binary assets?

| Option | Meaning | Consequence |
| --- | --- | --- |
| **A** | Separate Compact scene and hierarchy; no old planks, rack, corridor or consumers | Strong isolation: Legacy40/Enlarged80/Reference cannot regress; needs its own scene construction and its own asset set |
| **B** | Shared scene with profile gating | Less duplication, but the shared scene is the highest-conflict surface in the project and every gate would have to prove the old worlds are untouched |

**Rights sub-question:** adopt the D-032 first-party asset disposition for any new Compact-only mesh/material, with a recorded source/licence and no LFS or new dependency without approval?

**Unlocks:** W1 rows 10, 30, 72, 88 and all presentation rows; must precede any world/graphics signature.
**Cannot be released by this answer:** old-world regression evidence, performance, native visual acceptance.

**Orchestrator note:** **A + yes on the D-032 disposition.** A separate scene is the cheapest way to keep the frozen references provably frozen, and it matches the DRAFT graphics spec's stated preference. On rights, reusing verified first-party assets avoids the licensing question entirely; anything new should be recorded with provenance before it enters a signature.

---

## D5 = OD-15 — Roster adoption

**Question:** adopt the authenticated 175-row roster verbatim, or amend it?

| Option | Meaning | Consequence |
| --- | --- | --- |
| **A** | Sign the roster as it stands: digest `dac06a3b7775f884fac9f8df6b95178cbaf9d912b44687082a161eaf0426cca1`, 175 rows including the 8 one-metre buffer stems and the 19 former rack-labelled stems | The only authenticated 175 candidate; every reviewed distance, occupancy figure and G1 neighbour set stays valid |
| **B** | Return an amendment | Needs a new roster, a new digest, a new provenance reproduction, new cell occupancy and new clears — and it can change G1 neighbour distances and cell-15 light |

**Unlocks:** W1 rows 1–8, 27, 33, 48, 49.
**Cannot be released by this answer:** native census readback, survival or regeneration.
**Non-negotiable either way:** no rack object, Year-1 access clearance, Fell event, timber sale, H1 revenue or grant travels into the new world; the 14 H1 omissions remain pre-world history.

**Orchestrator note:** **A.** The roster is the only byte-exact candidate, it has been reproduced independently three times, and an amendment restarts the provenance chain for a cosmetic gain nobody has asked for.

---

## What happens after your answers

1. Answers are recorded verbatim in this packet (a follow-on commit), not interpreted.
2. Items answered here stop being BLOCKED **as choices**; the data each one requires (recipe pin, scene identity, rights record, roster attestation) is then produced as a bounded authoring task.
3. The remaining OD items are put to you in a later round, in the same format.
4. Nothing becomes SIGNED until D1 exists and every BLOCKED row is cleared with actual values; no spec is approved, no Unity run is authorised, and M-A-SCOPE stays deferred.

**Still outstanding and independent of these answers:** the D-N01 cross-reference nit in the W1 document, the native manifest `closure-receipt.json` mismatch (61/62 members), dashboard synchronization (NOT VERIFIED), ISO-58 (BLOCKED) and #652 live Ops (HELD).
