# Manager #200 — Sol #374 UI repair source disposition and next qualification gate

**Date:** 10 November 2026. **Verdict: APPROVED FOR ONE BOUNDED TIER-2 UNITY FEATURE QUALIFICATION ONLY.** This is a Manager direct SOURCE review, **not a second independent Fable review**, and NOT VERIFIED, INTEGRATED, G4 APPROVED, PLAYER ACCEPTED, or Windows-build authorised. Existing overall **integration verdict remains NEEDS CHANGES** until the genuine Unity visual/input evidence and test-measurement integrity are resolved.

## Exact evidence and provenance

- [Sol #374 repair report](https://github.com/jermur788/CCF/blob/6066c9bbbc569257e7a5acad2ac05fe47ea0e8f5/Docs/Handoffs/ScenarioOne-UI-Presentation-20261110/SOL-FABLE-373-BOUNDED-SOURCE-REPAIR-RESULT.md).
- [Original independent Fable #373 review](https://github.com/jermur788/CCF/blob/4dffb37d6d8d440218e8bd12062eb4ea2d06b790/Docs/Handoffs/ScenarioOne-UI-Presentation-20261110/FABLE-UI-PRESENTATION-V1-INDEPENDENT-REVIEW.md) — independently covers the original #372 source, not the #374 changes.
- Exact reviewed source before repair: `749c5c4f93ede68356a90c42dfaeff9df0aec060`; exact repaired SOURCE: `c683e9eecdbf87960b591c8acabec3139a8cca57`; report-only publication `6066c9bbbc569257e7a5acad2ac05fe47ea0e8f5`.
- GitHub compare confirms **one source commit** with **six UI .cs files, one verification README and one additive verification script**; no changes in ecology, economy, manager/gameplay, scene, prefab, material, save, package, source assets, Reference Future or user player-test branch. Source writer #374 closed and reported released. GitHub alone does **not** confirm local cleanliness/machine ownership.
- Manager fetched actual repaired source and base, examined changes in `WorkPlanView`, `TreeInspectionView`, `AnnualReviewView`, `WalkingHudView`, `CompetitorAssessment`, `ScenarioOneUiRoot`, and new `repair_consumers.py`. No local offline suite re-executed by Manager; tests below are **author-run receipts**, not independent replication.

## Findings disposition

| Original finding | Manager source inspection | Qualification residue |
|---|---|---|
| F-01 Locked Work Plan note | **SOURCE FIX ACCEPTED**: disabled note now in visible outer card, no disabled closed Foldout ancestor | Real visibility at 720p still needed |
| F-02 MFM candidate keys | **SOURCE FIX ACCEPTED**: existing candidate C/N hint shown on inspection overview and bottom hints, generic C suppressed there | Native input and stage interaction still needed |
| F-03 Crop+Fell inspection overflow | **MITIGATED, NOT VERIFIED**: page index moved first, long forecast/wind prose moved to draft page; overflow hidden and page fit not measured | Objective3 dense card pages all need genuine 720p/1080p render |
| F-04 Teaching prose mistaken for E action | **SOURCE FIX ACCEPTED**: keybar only uses bracket-key suffix; plain teaching prose remains in card | Physical keybar font/length |
| F-05 Foldouts & warnings | **SOURCE FIX ACCEPTED**: stable heading prefix key; attention/harvest sections start open | Note any default always-open behavior on rebuild after user closes; test edit/remove/native warning placement |
| F-06 Missing detail facts | **SOURCE FIX ACCEPTED**: broadleaf model caveat, distribution note, signed competition index delta, Crop discoverability added | Every V page physically accessible |
| F-07 Test gap | **IMPROVED BUT LIMITED**: new probe executes actual locked-card method and hint assembly, 12 source contracts and ten negatives; **seven negatives test textual contracts rather than runtime callpath**, so not a substitute for Unity or end-to-end guards | Keep historical guards intact; targeted native cases still required |
| F-09–F-14 | **SOURCE FIXES ACCEPTED PROVISIONALLY**: X/C eligibility, rank occlusion boundaries, pending count, annual detail state/provenance/canopy units, marked-vs-open Fell wording | Screenshot and native state verification |
| F-15 | **PARTIAL, NONBLOCKING**: Help key and fail cue restored; remaining bar/nav/typographic polish deferred | Escalate only if actual task blockage/occlusion seen |
| F-08 old harnesses | **G3 UNRESOLVED**: old harnesses assume intentionally removed panels; no existing harness altered | Preserve history; explicitly identify obsolete structure and add source-exact measurement without weakening intent |
| F-17 older checkers | **KNOWN PRE-EXISTING FAILS**, not passed, not charged as new source regression | Report as separate old checker maintenance if it becomes a release gate |
| F-16/F-18 | Nonblocking cleanup/colour suggestion deferred | No mandatory feature rerun |

**No observed SOURCE blocking defect demands a second Sol production repair before first real render.** This ruling does not assert physical layout fit or complete test validity. The Work Plan `alwaysOpen` default may reopen a group on rebuild; treat as a focused native observation, not a new speculative source-blocker.

## Tests / acceptance

The author reports 189 typed runtime sources / 116 existing Player assemblies **compile exit0, errors0**; focused 160/219/18 PASS, original eight compiled mutants killed and four source negatives refused; existing 157 presentation slices + five mutants PASS; new consumer probe with executable hint/locked note cases + twelve source contracts and ten negative controls PASS. Manager observed the new script's scope and confirms **not all controls exercise actual Unity callsites**.

Acceptance UI-AC07/11 have source-targeted repairs but cannot be promoted to visually verified; UI-AC04/19/20/21 still require real Unity. This is **source evidence sufficient to attempt Tier-2**, not overall feature qualification.

## Separate guarded Unity lane

- Next [#376](https://github.com/jermur788/CCF/issues/376): **READY / MACHINE GATED / NOT RUNNING**.
- [Binding qualification packet](MANAGER-374-TO-376-FOCUSED-UI-UNITY-QUALIFICATION-PACKET.md) includes one exact matrix and stop conditions.
- **Difficulty HIGH** (owned Unity machine, source exactness, user saves, visual/navigation and historical data), best **Sol/Codex High local Unity operator** with valid T3 lane.
- **Proposed source branch** `qualify/scenario-one-ui-presentation-v1-20261110-v1` cut from repaired source **if and only if** safely admitted, proposed distinct Unity worktree `/media/jer/ZX20/CCF-S1-UI-Presentation-V1-Qual` — not created, not owned. User #361 personal test separate. No reuse/mutation of player's U3 branch, worktree or saves.
- The last Sol evidence says ZX20 **unmounted**, no user Editor machine handback or verified save isolation. Thus **no current Unity permission** can be inferred. #376 is a runnable packet only after actual current preflight succeeds; otherwise mark BLOCKED without attempting Editor.
- Need machine-wide Unity lease and ownership, candidate-branch/HEAD preflight, adequate available disk, 27 ignored licensed assets, dirty Workbench/frozen Reference preservation, genuine disposable persistentDataPath/PlayerPrefs/F5/F9 coverage before any destructive action. No Windows Player build before user personally playtests and authorises.
- One feature Tier-2 only: 720p/1080p colour/grayscale HUD/Work Plan/Annual Review, Rack-first, MFM C/N, Crop+Fell inspection all pages, available rank targets, a low-value harvest quote and warning/edit, latest unread review Back then ACK, historical year and source-toggle state. Legacy40/reference scoped checks if lawfully available.
- On G3 measurement drift, do NOT weaken old structural tests to green. Reviewer-approved/new source-exact assertions should preserve actual behavioral intent and negative controls; mark old obsolete structural assertions as NOT APPLICABLE TO NEW LAYOUT if justified with source contract, **not PASS** and not silently deleted. Any rewrite of old harness needs separately scoped review.
- Return actual captured PNGs, logs and exact source receipt. Manager later issues separate G4 verdict. Do not claim integration or player acceptance from worker qualification alone.

## Management / ownership / cost

User explicitly declined a second Fable review because of cost. [#375](https://github.com/jermur788/CCF/issues/375) CLOSED NOT PLANNED, no replacement reviewer spend, and no claim of independently re-reviewed repair source. #372 original Sol author and #373 Fable original review also CLOSED; #374 Sol repair CLOSED/HANDED OFF. New work only after separate #376 admission.

**Result: APPROVED FOR FOCUSED UNITY QUALIFICATION, NOT FOR INTEGRATION.**

**Dashboard sync:** GitHub canonical reports and live coordination updated by Manager; downstream Drive/dashboard ingestion and readback **NOT VERIFIED**.

**User action:** Provide a released, safely isolated Unity machine lane when available and dispatch the one #376 packet. No Fable activity. It is acceptable to continue personal player test while #376 remains MACHINE GATED; do not take its lock or saves.
