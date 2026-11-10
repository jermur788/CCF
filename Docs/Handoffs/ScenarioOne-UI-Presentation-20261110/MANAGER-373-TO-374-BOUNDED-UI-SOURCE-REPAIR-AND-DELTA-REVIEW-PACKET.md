# SINGLE MANAGER DISPATCH — Sol repairs #373 findings; then independent Fable checks the delta

**Status: READY TO DISPATCH, NOT RUNNING.** This is a bounded sequel to the Sol Scenario One UI presentation implementation. The existing implementation remains **NEEDS CHANGES**, not rejected or integrated. **No Unity, Windows build, save, or user Editor access is authorised by this packet.**

## Roles, difficulty, ownership

- **Author:** OpenAI Sol / Codex High in T3 Code, **High difficulty** due multi-surface cursor-locked layout, Work Plan locked state and source-to-consumer test coverage. Claim [#374](https://github.com/jermur788/CCF/issues/374) **READY / NOT RUNNING**.
- **Independent delta reviewer:** Anthropic Claude Fable Code in a distinct **top-level peer session**, **Medium difficulty** because this is a *focused repair review*, not a replay of full #373. Claim [#375](https://github.com/jermur788/CCF/issues/375) **WAITING / NOT RUNNING** until Sol releases source. A Sol child/subagent or Sol self-review is not independent.
- **Manager:** #200. User's personal #361 playtest is separate; maintain protected ownership/Unity locks. Manager's GitHub live coordination is single-writer and must not be casually overwritten.

**Binding Manager verdict:** https://github.com/jermur788/CCF/blob/docs/manager-ui-373-disposition-20261110-v1/Docs/Handoffs/ScenarioOne-UI-Presentation-20261110/MANAGER-373-INDEPENDENT-REVIEW-DISPOSITION-AND-REPAIR-SCOPE.md

**Full independent review:** https://github.com/jermur788/CCF/blob/4dffb37d6d8d440218e8bd12062eb4ea2d06b790/Docs/Handoffs/ScenarioOne-UI-Presentation-20261110/FABLE-UI-PRESENTATION-V1-INDEPENDENT-REVIEW.md

**Original Sol implementation report:** https://github.com/jermur788/CCF/blob/199b5a6edae36f27d554cdc142a8d7797d11350e/Docs/Handoffs/ScenarioOne-UI-Presentation-20261110/SOL-UI-PRESENTATION-V1-IMPLEMENTATION-AND-QUALIFICATION-RESULT.md

**Approved feature spec:** https://github.com/jermur788/CCF/blob/spec/scenario-one-ui-presentation-20261110-v1/specs/scenario-one-ui-presentation-v1/spec.md

## Phase 1 — Sol only, exact local source lane

- **Game source baseline (reviewed):** 749c5c4f93ede68356a90c42dfaeff9df0aec060.
- **New GitHub repair branch prepared:** fix/scenario-one-ui-presentation-fable-review-20261110-v1 **created from that exact source**. Do NOT edit the player's feature branch, prior Sol handed-off branch, main, or warm U3.
- **Proposed isolated local writable worktree:** /media/jer/Files/CCF/Operations/CCF-S1-UI-Presentation-V1-Repair. It is **not yet created, admitted or granted ownership**. Confirm actual disk/mount available; if the proposed path is unsafe, stop for Manager lane decision instead of writing into another owner's worktree.
- **Primary allowed write set:** Assets/ForestPrototype/UI/ including UI Resources/USS and Tools/Verification/ScenarioOneUiPresentation/ **only**; add focused consumer-path verification scripts there. No ecology, economy, marking handlers, World, save, Reference, scene, Prefab, Package or global tests mutation.
- **Required GitHub result path:** Docs/Handoffs/ScenarioOne-UI-Presentation-20261110/SOL-FABLE-373-BOUNDED-SOURCE-REPAIR-RESULT.md.

**Preflight before production edits:** refresh **live** Docs/Project/coordination/active-tasks.md and #374. Check source HEAD/branch and clean untracked/ignored/modified bytes, pre-existing valued files, available disk, all active UI claims/locks. Verify Sol #372 writer RELEASED and Fable #373 review CLOSED/RELEASED; do not infer local cleanliness from GitHub. If another writer owns UI, stop and mark #374 BLOCKED rather than overwriting. Register one primary UI source writer with actual lane/branch/HEAD, and only then mark #374 [RUNNING] with provider/model and starting HEAD.

**Required source changes**, as detailed in Manager disposition:

1. **F-01 blocking:** locked Work Plan explanation readable outside disabled Foldout.
2. **F-02 blocking:** actual MFM candidate Confirm/Reject/Reopen C/N keys visible on first card view and bottom hints, not trapped behind page 3.
3. **F-03 layout risk:** move long Fell-mark/storm forecast from inspection overview to draft detail; keep concise material warning/route on overview, put page indicator at top; fit is **NOT VERIFIED** until Unity.
4. **F-04 / F-05:** parse teaching key prompts so plain prose does not attach to [E] Close; stable Work Plan Foldout keys independent of changing order counts and important problem/minimum note visible while collapsed.
5. **F-06:** restore lost previously available broadleaf storm limitation, competition distribution sentence, signed change, and ordinary-tree Crop discoverability in reachable bounded detail pages. Distinguish current open job from unqueued Fell mark, model-on vs off and forecast from approved work.
6. **F-09/F-10/F-11/F-12/F-13:** truthful marking eligibility, objective/keybar rank overlap guard, actual pending counts, persistent Annual Review detail when toggling sources, truthful source/provenance and canopy units. **F-14/F-15** only selectively if within safe same presentation boundary (e.g. Fall mark vs job, Help/failure cue, task label); no whole-HUD redraw.
7. **F-16/F-18 nonblocking**, avoid wide housekeeping or speculative changes.

**Verification against F-07:** source compilation against current 189 runtime references + existing 160/219/18 regression suites and guard mutants unchanged, then **meaningful new consumer-level tests** that exercise the actual source callsites/outputs (not only isolated helper stubs). At least check: locked-note visibility structure; F-02 exact candidate key context; teaching status cannot pollute [E] action; actual Year/Cash-only HUD; C/X Rack-first and orange/rack-target eligibility; annual current-vs-historical headline source selection, Back vs ACK, and static Approve vs distinct Advance; stable foldout identity after changing a count. Include named adverse mutations that those tests truly kill. Do NOT add 100s of repeated modular arithmetic asserts or weaken old tests to obtain green.

**Manager G3 disposition for F-08:** preserve old Unity harnesses, list each historical test invalidated by the deliberately approved structural redesign, and propose source-exact successor assertions of **equivalent measured intent**. No silent exclusions or rewriting old tests in this task. A separate reviewed *verification-only* packet is required if touching existing checkers; original historical results remain intact. **F-17** two static checker failures reproduce at original base: leave them as known pre-existing checkers, not newly “fixed” by UI task. If discovered true new gameplay failure, stop and ask Manager.

**Unity:** ZX20 was unmounted and user Editor release not verified at review. Thus **source-only default**. Do not mount ZX20, acquire global unity-lock, launch Editor, touch U3, Workbench, frozen Reference, user save/preferences or run Windows build. Report Unity **NOT RUN** and visual/native/save **NOT VERIFIED** if still unavailable. Any later Tier2 requires a new Manager-admitted exact candidate, proper machine-wide lease, 27 ignored licensed asset/Workbench/frozen reference protection, safe disk monitoring, user Editor handback and verified isolated F5/F9/Prefs.

**Sol handoff and release:** commit/push exact repaired source and separate Markdown result including paths, branch/HEAD, all fixes+deferrals, test commands PASS/FAIL, changed assertions and mutation receipts, current image/SVG constraints and source boundary. Close #374 **[HANDED-OFF]** only after actual writer release; report remaining Unity NOT VERIFIED. Do not merge.

## Phase 2 — Sol attempts a separate Fable independent peer delta review

After Sol source candidate is frozen and #374 handed off/released, Sol uses genuine T3 independent **top-level Anthropic Fable peer** launcher if available, with source read-only and separate new claim **#375**. NOT a native Sol subagent or Sol-authored reviewer report.

- **Reviewer source:** the exact **repaired source** emitted in Sol handoff. Do not review moving branch or report-only docs commit as a different game candidate.
- **Planned reviewer branch:** review/scenario-one-ui-presentation-repair-delta-20261110-v1 **created from repaired source only after Sol finishes**.
- **Planned reviewer worktree:** /media/jer/Files/CCF/Operations/CCF-S1-UI-Presentation-V1-DeltaReview. It is NOT created/owned yet; no source writes. Read live coordination and confirm prior writer release at review start, then mark #375 RUNNING with actual independent model/provider/lane/HEAD.
- **Exact reviewer output:** Docs/Handoffs/ScenarioOne-UI-Presentation-20261110/FABLE-373-SOURCE-REPAIR-DELTA-REVIEW.md.
- Compare only F-01 to F-07 plus bounded F-09 to F-15 and Manager's F-08/F-17 verification rules. Inspect actual source and tests, and any available genuine rendered evidence **if it exists**; no new full design assessment. State **PASS / NEEDS CHANGES / REJECTED for source** with no Unity approval or G4 integration inference.
- At handoff mark #375 **[HANDED-OFF]**, close, release docs lane and return to Manager #200.

**Provider cannot run / session limit:** explicitly record independent Fable **BLOCKED/NOT RUNNING** on #375 and Manager #200, with exact reason; do not impersonate Fable, substitute Sol self-review or promise a background restart. Source repair remains valid as a handoff pending independent review.

## Decision after Fable

Manager #200 will inspect Sol repair and independent focused delta review. If source is approved, arrange **one** feature-focused Unity Tier-2 when legally available, with 1280×720/1920×1080 colour/grayscale, native key/focus, Work Plan locked note/quotes/edit, real MFM candidate keys, Crop/Fell inspection overflow and [V] pages, latest unread and historical Review; old-world/Reference+safe save reload as relevant. **NO Integration/Windows/Player build before user personally tests and explicitly authorises.**

**User action:** dispatch this one packet into capable local Sol/Codex. Do not send separate Fable message unless independent peer launch is blocked. No separate Unity approval is assumed.

## ONE USER COPY/PASTE DISPATCH

> Sol / T3 Code (Codex High): Execute this binding Manager follow-up from source repair through independent review handoff: https://github.com/jermur788/CCF/blob/docs/manager-ui-373-disposition-20261110-v1/Docs/Handoffs/ScenarioOne-UI-Presentation-20261110/MANAGER-373-TO-374-BOUNDED-UI-SOURCE-REPAIR-AND-DELTA-REVIEW-PACKET.md . Start claim #374 in the prepared new repair branch and isolated clean Files worktree only after live single-writer preflight; fix the narrowly scoped Fable #373 issues, add meaningful consumer-path verification, publish the exact Sol repair report, release source and close #374. Then if an **independent Anthropic Fable top-level peer** is available, automatically dispatch a **short source-delta review** under #375, with its own read-only review lane and GitHub handoff, and return both to Manager #200. No Unity/Windows/user saves, no merging, no test weakening. If independent provider cannot run, report #375 BLOCKED honestly.
