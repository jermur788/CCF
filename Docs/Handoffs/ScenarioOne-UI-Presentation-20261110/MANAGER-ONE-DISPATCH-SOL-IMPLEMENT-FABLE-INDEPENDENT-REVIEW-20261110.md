# SINGLE DISPATCH — Sol implements, then initiates an independent Fable review

**Status:** AUTHORISED FOR DISPATCH; Sol and Fable are **NOT RUNNING** until actual local preflight.  
**Date:** 10 November 2026.  
**Manager:** Overall Manager, issue #200.  
**User request:** One message to Sol, with Sol responsible for *automatically handing the completed candidate to Fable for review* where the connected T3/provider orchestration supports a separate reviewer job. Do **not** ask the user to copy a second review message by default.

This packet **supersedes the two earlier Manager Sol and Fable packets for dispatch mechanics only**; they and the approved spec remain incorporated as technical detail. It does not merge the implementation and independent-review roles, authorise parallel edits, or bypass Manager acceptance.

## Objective and difficulty

Deliver a consistent, simpler Scenario One UI covering **walking/inspection HUD, contextual bottom action/keycaps, top-right current Year/Cash, shared Objectives/Stand Map/Work Plan/Annual Review menu navigation, Work Plan costs/approval, and Annual Review Work done/Money/Forest changes**. Then arrange **separate, read-only, cross-provider Fable review** of the completed source and genuine screenshots.

- **Sol implementation:** **HIGH** — stateful Unity UI, locked-cursor input, dynamic competition display and Work Plan/review authority; **OpenAI Sol/Codex High**, local T3 Code implementation lane.
- **Fable review:** **HIGH** — independent UI/source/render/spec conformance, annual-year truth and guard audit; **Anthropic Claude Fable Code**, as a **separate peer job** with its own execution claim and read-only lane, not a Sol child/subagent.

## One-click starting sources

- [APPROVED feature spec](https://github.com/jermur788/CCF/blob/spec/scenario-one-ui-presentation-20261110-v1/specs/scenario-one-ui-presentation-v1/spec.md)
- [Implementation plan](https://github.com/jermur788/CCF/blob/spec/scenario-one-ui-presentation-20261110-v1/specs/scenario-one-ui-presentation-v1/plan.md)
- [Tasks and single-writer sequencing](https://github.com/jermur788/CCF/blob/spec/scenario-one-ui-presentation-20261110-v1/specs/scenario-one-ui-presentation-v1/tasks.md)
- [Acceptance checklist](https://github.com/jermur788/CCF/blob/spec/scenario-one-ui-presentation-20261110-v1/specs/scenario-one-ui-presentation-v1/checklists/requirements.md)
- [Visual A — HUD/inspection](https://github.com/jermur788/CCF/blob/spec/scenario-one-ui-presentation-20261110-v1/specs/scenario-one-ui-presentation-v1/visuals/01-hud-inspection.svg)
- [Visual B — Work Plan](https://github.com/jermur788/CCF/blob/spec/scenario-one-ui-presentation-20261110-v1/specs/scenario-one-ui-presentation-v1/visuals/02-work-plan.svg)
- [Visual C — Annual Review](https://github.com/jermur788/CCF/blob/spec/scenario-one-ui-presentation-20261110-v1/specs/scenario-one-ui-presentation-v1/visuals/03-annual-review.svg)
- [Visual D — navigation/guards](https://github.com/jermur788/CCF/blob/spec/scenario-one-ui-presentation-20261110-v1/specs/scenario-one-ui-presentation-v1/visuals/04-navigation-and-guards.svg)
- [Visual poster supplement and mandatory errata](https://github.com/jermur788/CCF/blob/spec/scenario-one-ui-presentation-20261110-v1/specs/scenario-one-ui-presentation-v1/visuals/POSTER-SUPPLEMENT-AND-ERRATA.md)
- [Earlier detailed Sol packet](https://github.com/jermur788/CCF/blob/spec/scenario-one-ui-presentation-20261110-v1/Docs/Handoffs/ScenarioOne-UI-Presentation-20261110/MANAGER-SOL-UI-PRESENTATION-V1-IMPLEMENTATION-PACKET.md)
- [Earlier detailed Fable reviewer packet](https://github.com/jermur788/CCF/blob/spec/scenario-one-ui-presentation-20261110-v1/Docs/Handoffs/ScenarioOne-UI-Presentation-20261110/MANAGER-FABLE-UI-PRESENTATION-V1-INDEPENDENT-REVIEW-PACKET.md)

## User-selected visual style — verified upload fingerprint, local path to check

**Most recent user-preferred image:** `poster-3-ui-update-sim.png`, expected at:

`/media/jer/Files/CCF/Verification/CCF-362-FableUXReview-20261110/spec-posters/poster-3-ui-update-sim.png`

The user directly confirmed the **parent directory** after saying this newer image is “even better.” The basename above is the name of the image uploaded to the Manager conversation, **not yet independently checked against the local user's filesystem**.

**In the Manager's conversation-upload copy:** actual PNG **2048 × 1152**, SHA-256 **`b4400d9baa843e3bd7dd61c4f9cc9ae3b4f49d43ac377fb042e9bd8d1523b01c`**. These size/hash facts describe the uploaded bytes; they do **not** prove that the file at the user's local directory has identical bytes.

At Sol preflight:
```bash
ASSET_DIR='/media/jer/Files/CCF/Verification/CCF-362-FableUXReview-20261110/spec-posters'
ls -lah "$ASSET_DIR"
file "$ASSET_DIR/poster-3-ui-update-sim.png"
sha256sum "$ASSET_DIR/poster-3-ui-update-sim.png"
```
If that basename is absent, inspect the directory for the **newly preferred 2048×1152 dark charcoal/teal three-panel poster**, but do not choose a different mock-up silently. Verify filename/hash against the fingerprint and record the actual path. If unavailable, record NOT INSPECTED and continue against the Git-backed SVGs and older posters rather than inventing a matching reference.

**Priority:** this newest dark charcoal/teal visual **leads style/composition**, then the three earlier polished PNGs, then older Fable mock-ups. The **approved spec, simulation data and controls outrank all pictures**. Specifically: real Fell key is **[X]**, not [F]; neighbour shares are not withheld potential diameter growth; the 54%/net/canopy/operations are invented examples; no global browsing/biodiversity number; Work Plan approval must not pulse; Annual Review must not jump straight to next year, must allow Back before review ACK; proposals are not Fell marks; do not present wind model-off as safety.

## STEP 1 — Sol source implementation (claim #372)

**Issue:** https://github.com/jermur788/CCF/issues/372  
**Starting game source:** `4db486db7a132bd01725442ef33e00f702c4dff9` (subject to genuine local recheck).  
**Prepared GitHub source branch:** `feature/scenario-one-ui-presentation-v1-sol`.  
**Planned isolated local writable worktree:** `/media/jer/Files/CCF/Operations/CCF-S1-UI-Presentation-V1-Sol` (not created or owned merely by naming it).  
**Primary owned file set after live claim:** `Assets/ForestPrototype/UI/` presentation sources and USS + focused verification only; source/gameplay outside that requires Manager ruling.  
**Output:** `Docs/Handoffs/ScenarioOne-UI-Presentation-20261110/SOL-UI-PRESENTATION-V1-IMPLEMENTATION-AND-QUALIFICATION-RESULT.md`.

1. **Before mutation** refresh live `Docs/Project/coordination/active-tasks.md`, issue #372, #361 user machine status and any other active UI owner. Check actual branch, starting HEAD, worktree cleanliness/untracked/ignored protected assets and disk; stop on overlapping writer. Register an actual one-writer lock before editing. Mark #372 **[RUNNING]** with actual provider/model, branch, worktree and HEAD. If not admitted mark **[BLOCKED]**, publish reason and stop.
2. Inspect the **current** UI source to avoid undoing the November repair chain. Implement all agreed screens, not just the HUD:
   - current objective top-left, Year/Cash only top-right, valid bottom contextual keys;
   - responsive single selected-tree/competition/plan card and keyboard-accessible `[V]` detail;
   - consistent menu routes, next-action reason, safe Back;
   - Work Plan actual pending/approved/quote/contractor minimum and separate static Approve versus Advance;
   - Annual Review descriptive **Work done / Money / Forest changes** from the selected year, details/provenance and history preserved, latest unread acknowledgement gate correct.
3. Respect MFM proposal vs Fell, real competition denominator, old saves/Legacy40/frozen Reference Future v1, rack-first no [X]/[C], no invented habitat score or browsing global indicator; do not change ecology, economy, save schema or progression.
4. Run focused cheap Tier 1. Attempt **one** relevant Tier-2 real 720p/1080p colour/grayscale Unity qualification **only** with actual user Editor handback, machine-wide exclusive lease, protected Workbench/27 licensed asset/reference check, safe disk and **verified disposable save/Prefs/F5-F9 isolation**. No Windows build. If unsafe, no Unity; classify as SOURCE-ONLY / RENDER NOT VERIFIED rather than inventing PASS.
5. Commit/push the source and the exact Sol Markdown result; record tests, failures/NOT RUN, true screenshot archive, asset hash, file inventory and guard outcomes. Mark #372 **[HANDED-OFF]**, close and release the UI writer/Unity lease **before attempting the next job**.

## STEP 2 — Sol orchestrates Fable as a **separate independent review** (claim #373)

**Trigger:** Only after Step 1 is durably published and Sol source is frozen at the handed-off candidate.  
**Issue:** https://github.com/jermur788/CCF/issues/373  
**Review provider:** Anthropic Claude **Fable**.  
**Branch:** `review/scenario-one-ui-presentation-v1-fable` **to be created FROM THE EXACT SOL SOURCE CANDIDATE**, not from a moving main/spec branch.  
**Planned separate review worktree:** `/media/jer/Files/CCF/Operations/CCF-S1-UI-Presentation-V1-FableReview`; own docs-only writer lane, game source read-only.  
**Exact final review:** `Docs/Handoffs/ScenarioOne-UI-Presentation-20261110/FABLE-UI-PRESENTATION-V1-INDEPENDENT-REVIEW.md`.

**AUTOMATIC ROUTING RULE:** Sol should use its **available T3/orchestrator peer-worker launcher**, if genuinely present, to submit the separate Fable review packet above, including exact Sol candidate/claim, spec/24 checks, four SVGs, latest preferred poster, actual render evidence and the Sol handoff. This is a **new, separately tracked provider session with independent context and output**, not an internal Sol subagent/helper or Sol-authored "Fable" report. The repository's AgentWorkflow explicitly does **not** count a provider-native helper or child of the implementation worker as an independent reviewer; a reviewer launched as a child of Sol is therefore **not acceptable as the review gate**. Use the orchestrator's independent peer-job route only if available; do not violate this rule just to make dispatch automatic.

**Reviewer starting preflight:** Fable independently refreshes live active-tasks, checks that #372 is closed and UI writer released, checks exact committed candidate and source integrity, marks #373 **[RUNNING]** with its *actual* provider, model, branch/worktree/starting HEAD, then performs a review-only diff, focused tests if appropriate, and evidence comparison. No production source fixes, no Unity/lease by default, no same-lane edits.

**Review criteria:** inspect 24 acceptance checks and eleven poster errata; actual input and review-first route; tree competition meaning; proposed vs committed; numeric source truth for Work Plan/Annual Review; safe Year-advance/acknowledgement; [V] reachability; genuine 720p/1080p screenshots (if captured), cursor/keyboard; old-world/frozen reference boundary. Report explicit **PASS / NEEDS CHANGES / REJECTED**, severity/location and missing proof. No imaginary screenshots or compiler outputs. Publish GitHub review, close #373 as **[HANDED-OFF]** and release docs lane, return the actual result to Manager #200.

**If the T3 orchestration route cannot launch a truly independent Fable peer (provider unavailable, subscription/auth/quota limitation, no launcher, no permission or risky shared-worktree custody):** Do **not** impersonate Fable, silently substitute Sol's self-review, consume the user Editor or claim review complete. Mark #373 **[BLOCKED / NOT RUNNING]** with an exact explanation and a durable manager-ready next dispatch; return Sol's source handoff and the blocker to #200. This fallback does not invalidate an otherwise useful source implementation. Sol must not wait indefinitely or spin on provider quota.

## Final stop / deliverable

Both handoffs remain separate to preserve independent evidence and branch ownership, but **the user sends only this single starting handoff**. Sol is accountable for **attempting the peer Fable launch automatically after its own completion**, or visibly reporting why independent launch was impossible. Sol does **not** grant integration approval. Manager #200 subsequently inspects both and issues **APPROVED / NEEDS CHANGES / REJECTED**, protects current #361 playtest and controls integration, and reports dashboard downstream synchronization.

**No production edits, Unity session, model invocation or review occurred merely because this dispatch document was published.**

## ONE USER COPY/PASTE LINE

> Sol / T3 Code (Codex High): Execute this single Manager dispatch end to end: https://github.com/jermur788/CCF/blob/spec/scenario-one-ui-presentation-20261110-v1/Docs/Handoffs/ScenarioOne-UI-Presentation-20261110/MANAGER-ONE-DISPATCH-SOL-IMPLEMENT-FABLE-INDEPENDENT-REVIEW-20261110.md . Use #372 for sole-writer implementation of **all** approved Scenario One UI changes; inspect the user's preferred image in `/media/jer/Files/CCF/Verification/CCF-362-FableUXReview-20261110/spec-posters/`; follow the approved spec/real controls; publish Sol's exact GitHub result and release the UI writer. **Then automatically dispatch #373 to an independent Anthropic Fable peer job through your orchestration tools** (not a Sol child/subagent), have Fable publish its separate review and close claim, then return both results to Manager #200. If that independent peer route/provider is unavailable, publish #373 BLOCKED and the precise reason instead of claiming the review passed. Never bypass machine/save locks or build Windows without user authorization.
