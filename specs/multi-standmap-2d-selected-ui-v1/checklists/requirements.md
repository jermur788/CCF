# MULTI-STANDMAP-2D-SELECTED-UI-V1 — Requirements and acceptance checklist

**Status: APPROVED SPECIFICATION / IMPLEMENTATION EVIDENCE UNCHECKED.** Manager #863 approved under explicit owner delegation on 2026-11-20 14:38 UTC. Checkboxes for completed gameplay, final reference and real qualification remain unchecked; do not treat an approved spec as implemented.

## Owner authority / reference
- [x] Owner expressly delegated approval of `spec.md`, `plan.md`, `tasks.md` and `checklists/requirements.md` to Multi Manager #863 at 14:38 UTC; Manager approved the four-file spec for bounded front-end work (not physical source custody).
- [x] Owner chose generated image #1 as preferred **design concept** and richer legend element of generated image #3 (recorded #868 comment 7041359594; **not an actual screenshot**).
- [x] Owner approved predefined reason dropdown + Other/custom with recorded per-tree reason (recorded #868 comment 7040442192).
- [x] Manager #863, under the owner's explicit approval delegation, authorised the five proposed dropdown labels in `spec.md` as the V1 implementation vocabulary; owner may refine them following playtest.
- [ ] Actual image #1 (+ image #3 legend-only if available) permanently stored, SHA-256 and generated-concept provenance verified; fidelity authority frozen.
- [ ] Source implementation sole-writer branch/worktree, live coordination and physical lock verified with exact #849 base; no conflicting source writes.

## Main UI and map
- [ ] Browser at **1280×720 100%** shows dominant map and core selected-tree, reason, add, approve, execute and Year+1 with no long document scroll.
- [ ] Browser at **1920×1080 100%** shows sensible expanded map composition, not a stretched debug form.
- [ ] Full table / keyboard exact-ID alternative, full history and technical SHA/source provenance remain accessible via contained tabs/drawer.
- [ ] Live forest map is truly **30×30m, 100 Sitka**, north up, engine-defined positions/overlap/selection; no invented 1ha, 3-stand or mixed-species metadata.
- [ ] Map richer styling matches frozen concept within feasible canvas/DOM constraints, only as decorative rendering based on authoritative source data; visual reference comparison documented.
- [ ] Compact D-pad arrows, +/− and Fit operate without intercepting tree clicks; 44×44px pointer targets; functional focus/labels.
- [ ] North and metre scale correct at all views; fit never covers selectable tree or makes page horizontally wider.
- [ ] Selected tree ID, DBH, height, live/dead/felled state and plan order are exact values from server.
- [ ] Two distinct legend headings: **Tree species** (Sitka only) and **Management/Felling state**. Pending, Approved but standing, Felled/Stump, NaturalDead (if present) accurate and non-colour-only.
- [ ] No unsupported harvesting/extraction/rack/forest interventions depicted as active.

## Work Plan / actual ecology
- [ ] Reason dropdown placeholder blocks Add; predefined choice records its readable exact reason per tree.
- [ ] “Other” exposes required text; empty/whitespace reason refused; does not silently use fabricated default.
- [ ] Pending add/remove shown with exact TreeID; no tree fells until approved plan explicitly executed.
- [ ] Approve commits **zero Fell**; UI clearly distinguishes approved standing tree from executed stump.
- [ ] Cancel approved plan returns safe state via real API; stale/changed plan cannot execute.
- [ ] Execute confirmation shows matching PlanId, exact approved IDs and per-tree reasons, not merely client state; once-only effect; replay/refusal follows real engine.
- [ ] After execution stump/gap and living count/basal area reflect actual engine, not mock math.
- [ ] Year+1 is actual annual ecology; pending/approved-work consent/cancel rules intact.
- [ ] Live source/session/work revision fences, host+origin+ephemeral capability, error lock/re-read and no-persistence constraints retained.
- [ ] No P0/core/fixture/hash/security guard weakened; all prior frozen qualification evidence still applies or is independently requalified after changed DOM.

## Accessibility / browsers / evidence
- [ ] At actual **200% browser zoom**, no inaccessible clipping of keyboard controls, dialog, reason, plan actions or table path; no sideways page scroll.
- [ ] At 640 CSS px and 360 CSS px, reflows to usable stacked view, with reasonable vertical scrolling allowed.
- [ ] All controls keyboard reachable and semantics, focus return, live announcements, 44px buttons, non-colour-only cues, and readable contrast tested in a **real browser**.
- [ ] At least one real mouse and keyboard run of select→reason→pending→approve→execute→Year +1 on a fresh live engine session, not synthetic clicks only.
- [ ] Targeted Tier 1 during authoring and Tier 2 on final exact candidate; verify positive and negative paths plus narrow fixture/save/regression risk, no broad unrelated Tier 3.
- [ ] Independent nonauthor technical reviewer gives APPROVED; screenshots of actual UI and versions/source digests archived in GitHub handoff.
- [ ] Owner personally playtests and reports G2 acceptance separately; no Windows/Unity Player build or main/G4 merge implied.

## Mandatory stop / decision conditions
- **REFERENCE / VISUAL ACCEPTANCE HELD:** The four-file spec and V1 reason vocabulary have Manager-delegated approval, but the image-reference asset/hash remains unfrozen; do not claim image-specific fidelity or final owner acceptance. Implementation may follow the approved textual visual direction while preserving this hold.
- **NEEDS CHANGES:** core command semantics, source provenance, accessible reflow or genuine server selection regress.
- **HELD ON LANE:** source writer lacks real exclusive physical/coordination permission, exact base or known-good P0.
- **APPROVED:** only for exact reviewed/qualified candidate, without pretending owner G2 or G4 happened.
