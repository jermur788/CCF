# Manager #200 — Independent Fable #373 review disposition and bounded repair scope

**Date:** 10 November 2026. **Manager verdict: NEEDS CHANGES (G2 source and evidence), not REJECTED.** No source acceptance, G4 integration, player acceptance or Windows build authorised. This is a Manager ruling separate from the reviewer's verdict.

## Evidence inspected

- [Fable independent review #373](https://github.com/jermur788/CCF/blob/4dffb37d6d8d440218e8bd12062eb4ea2d06b790/Docs/Handoffs/ScenarioOne-UI-Presentation-20261110/FABLE-UI-PRESENTATION-V1-INDEPENDENT-REVIEW.md).
- [Sol #372 implementation and reported tests](https://github.com/jermur788/CCF/blob/199b5a6edae36f27d554cdc142a8d7797d11350e/Docs/Handoffs/ScenarioOne-UI-Presentation-20261110/SOL-UI-PRESENTATION-V1-IMPLEMENTATION-AND-QUALIFICATION-RESULT.md).
- Reviewed source: 749c5c4f93ede68356a90c42dfaeff9df0aec060 (base 4db486db7a132bd01725442ef33e00f702c4dff9).
- Manager fetched current TreeInspectionView.cs, WorkPlanView.cs, ScenarioOneUiRoot.cs and ScenarioOneUi.uss at reviewed source. Confirmed a disabled Work Plan Foldout nests its explanation in the inaccessible section; the tree inspector appends treatment, storm and rack content to its first page and appends the page hint last with overflow hidden; MFM candidate controls appear only on a later page, not the immediate key prompt. **F-03 overflow remains a plausible source-derived risk, not a measured Unity failure.**

Sol's offline typed compile and focused tests reproduced in independent source review; actual Unity rendered, native-input and save/reload evidence is absent. The reviewer's eight disposable mutants surviving earlier offline checks demonstrate test blind spots, not eight defects in the candidate. Reviewer is provider-independent of Sol's new implementation but previously participated in earlier UI design/repairs; weight aesthetic comments accordingly.

## Finding-by-finding Manager disposition

| Finding | Decision | Bounded repair |
|---|---|---|
| F-01 Locked Work Plan note | **BLOCKING** | Render the “Available in a later objective” note visibly outside any disabled Foldout; do not enable locked work |
| F-02 MFM candidate keys | **BLOCKING** | Expose actual Confirm/Reject/Reopen C/N keys on initial inspection/card and contextual keybar, accurately gated |
| F-03 Inspector height risk | **ADDRESS BEFORE RENDER** | Move long Fell forecast/storm details to dedicated draft page, keep material first-page warning, put page indicator above variable content; still verify fit in real Unity later |
| F-04 Teaching prose glued to E | **FIX** | Keep teaching status in its appropriate card/objective location; keybar parses only actual bracketed key commands |
| F-05 Foldout state/warnings | **FIX** | Track expansion by stable ID rather than changing count in title; keep “needs attention”, ineligible and minimum-cost warnings visible without opening a collapsed body |
| F-06 Missing model/detail notes | **FIX** | Restore broadleaf storm-model limitation, competition distribution sentence, signed competition change where source supports, and unmarked-tree Crop Tree discoverability; keep timber estimate reachable without recommending Fell |
| F-07 Test gaps | **TARGETED TEST REPAIR** | Small named consumer/path checks and effective mutants covering locked note, MFM keys, E prompt parse, Year/Cash-only, review selected year, Rack-first, Back+ACK and approval vs advance; no inflated assertion counts |
| F-08 Old Unity harnesses | **G3 TEST MEASUREMENT DECISION: NOT WAIVED** | Do not edit/weakening old harnesses in source repair. Record outdated structural pinning from intentionally removed panels; run a new exact-candidate presentation matrix alongside protected behavior checks when Unity is available. Any actual harness replacement needs independent review and equivalent negative controls; do not silently drop or relabel failures |
| F-09 Invalid mark hint | **FIX** | UI key prompts follow real orange entrance/rack-removal and current marking-handler guards; marking logic unchanged |
| F-10 Marker overlap | **FIX SOURCE GUARD / RENDER STILL REQUIRED** | Exclude newly positioned objective and bottom keybar areas as occlusion regions, preserve rank semantics |
| F-11 Pending count | **FIX** | Count actually pending orders rather than open minus valid approved when status would be wrong; no saved job changes |
| F-12 Annual detail foldout | **FIX** | Keep open state when toggling data sources or refreshing, with historical ACK rules untouched |
| F-13 Annual source labels/units | **FIX RELEVANT FACTS** | Distinguish recorded vs derived, use consistent canopy units and accurate closing cash if shown, no invented full net cash |
| F-14 Fell mark vs open job | **FIX IF BOUNDED** | Distinguish a user Fell mark from a valid queued job in neighbour row; no model modifications |
| F-15 Misc UX | **SELECTIVELY FIX** | Help discoverability, failure-state cue, truthful Tab verbs, allowed salvage; no full HUD redesign |
| F-16 Stale comments/shims | **NONBLOCKING** | Update touched comments safely, no wide cleanup |
| F-17 Old static checkers | **PRE-EXISTING FAIL, DEFER** | Both already failed on base; no silent checker patch/rebaseline in UI source task. Separate reviewed verifier-only work if a release gate requires it |
| F-18 Colour token | **NONBLOCKING** | Keep text distinction; optional palette refinement within scope |

## Verification and integration policy

**One bounded source repair**, one **short independent delta review**, then **one guarded focused Unity Tier-2** only when user Editor ownership and machine access are safe. No repeated full end-to-end programme and no test weakening to attain green. Type compile and focus tests must show actual callsite guards, meaningful mutation kills, and byte-stable gameplay/ecology/marking/economy/save/scene/reference files. Existing 160+219+18 focused suite preserved.

Sol's previously reported source PASS does not substitute for real 720p/1080p colour/grayscale display, native keyboard/cursor and save isolation. Fable source failure on acceptance UI-AC07 and UI-AC11 must be repaired; UI-AC19 and UI-AC20 remain NOT VERIFIED; user source/playtest #361 is distinct. ZX20 was unmounted and Editor handback unverified at review: no Unity, no machine-lease acquisition, no Windows.

## Sequencing and ownership

1. [#374 Sol bounded UI source repair](https://github.com/jermur788/CCF/issues/374) — READY / NOT RUNNING, dedicated branch **fix/scenario-one-ui-presentation-fable-review-20261110-v1** at reviewed source, proposed worktree **/media/jer/Files/CCF/Operations/CCF-S1-UI-Presentation-V1-Repair**, not yet created/owned. Worker refreshes live active-tasks.md, confirms no overlapping UI writer, registers single writer and RUNNING claim before source edits.
2. [#375 Fable focused independent delta](https://github.com/jermur788/CCF/issues/375) — WAITING until Sol source/report handed off and writer released, new docs-only reviewer branch/worktree at exact repaired candidate.
3. Manager inspects the delta result and decides next guarded Unity Tier-2 task; no integration until G2/G3/Unity evidence and G4 Manager approval.

**Final integration verdict: NEEDS CHANGES — DO NOT INTEGRATE.**

**Next packet:** Docs/Handoffs/ScenarioOne-UI-Presentation-20261110/MANAGER-373-TO-374-BOUNDED-UI-SOURCE-REPAIR-AND-DELTA-REVIEW-PACKET.md.

**Dashboard:** GitHub source records updated, downstream ingestion/readback NOT VERIFIED.

**User action:** paste one GitHub-backed dispatch message into a capable Sol local T3 Code session. Sol attempts to launch a separate Anthropic Fable peer after its own source handoff; if unavailable, report BLOCKED and provide a follow-up handoff rather than claiming review completed.
