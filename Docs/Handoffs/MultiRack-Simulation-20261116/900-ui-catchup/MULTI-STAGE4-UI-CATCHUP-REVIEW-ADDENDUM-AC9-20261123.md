# MULTI Stage 4 UI catch-up ? AC-9 review addendum ? 2026-11-23

**Final verdict: APPROVED (G2 acceptance; focused repaired-candidate re-review).**
**F-1, the sole required G2 finding, is CLOSED.** The candidate is suitable for the bounded owner playtest. Integration to main remains **UNAUTHORISED**; this is neither a G4 integration grant nor owner acceptance.

## Identity and scope

| Item | Value |
|---|---|
| Task | multi-stage4-ui-catchup-20261123 |
| Reviewed candidate | e7c1a57da519ed1ee03ccfbad240196a66df0905 |
| Previous candidate | 4d7267d076603833b1b12fff676c6d3161973f39 |
| Previous review | 42885ebfa4f5ca9d623a776542a21a09c5834326; MULTI-STAGE4-UI-CATCHUP-INDEPENDENT-REVIEW-20261123.md at that commit |
| Source/test HEAD | 541bb898ba27860dd9ff1159f1abd1ab8dccc410 (unchanged) |
| Base / admission | 500a50aea4bd049f231f05d73828fabdc0a7a85c / d3a9f20f |
| Candidate branch | feat/multi-stage4-ui-catchup-20261123-v1 |
| Review branch | docs/multi-stage4-ui-catchup-review-2-20261123-v1 |
| Sole writable checkout | /home/jer/Development/CCF-HP-Worktrees/CCF-Multi-Stage4UiCatchup-Review2-20261123 |
| Reviewer | OpenAI gpt-6.1-sol, high reasoning effort, T3 Code through Codex harness; review role, no subagents |
| Routing decision / attempt | d-900-ui-catchup-review-2 / att-20261123-05 |
| Context-source commit | 62e39ec621e38a84030c08b35a171e297056c9ed |
| Task authority | Latest direct owner review packet, including explicit owner AC-9 reconciliation, 2026-11-23 |
| Initial status | Clean, assigned branch, exact requested candidate HEAD |
| Permitted write | This new review addendum only, committed and pushed on the review branch |

I read the project workflow/context, task admission, approved spec/checklist/tasks, repaired handoff and prior review. Live GitHub coordination was fetched read-only using `gh api repos/jermur788/CCF/contents/Docs/Project/coordination/active-tasks.md`; returned blob SHA `9124d7854d071d2b612de91608ed64d35c1773b7`. Live main was `3a67d0987be0cf40d007892739d2ad295c84f0e0`. The supplied direct packet is this review's narrower write grant. No coordination, main-checkout or implementer-lane write was performed.

## Observed delta

There are **two** commits after the previous candidate, not three: `7dabed929e273373fc169fbfd7b673cf2871eb27` (evidence and documentation) and `e7c1a57da519ed1ee03ccfbad240196a66df0905` (PNG mode normalization). The packet's third numbered item describes the no-source-change constraint, not another commit.

Exact `git diff --name-status 4d7267d0..e7c1a57d` file list:

| Change | Repository path |
|---|---|
| M | Docs/Handoffs/MultiRack-Simulation-20261116/900-ui-catchup/MULTI-STAGE4-UI-CATCHUP-IMPLEMENTATION-RESULT-20261123.md |
| A | Docs/Handoffs/MultiRack-Simulation-20261116/900-ui-catchup/evidence/a-baseline-1920x1080.png |
| A | Docs/Handoffs/MultiRack-Simulation-20261116/900-ui-catchup/evidence/b-light-guide-ground-1920x1080.png |
| A | Docs/Handoffs/MultiRack-Simulation-20261116/900-ui-catchup/evidence/c-prune-workplan-1920x1080.png |
| A | Docs/Handoffs/MultiRack-Simulation-20261116/900-ui-catchup/evidence/c0-prune-pending-1920x1080.png |
| A | Docs/Handoffs/MultiRack-Simulation-20261116/900-ui-catchup/evidence/d-thinning-gaps-deadwood-1920x1080.png |
| M | specs/multi-stage4-ui-catchup-v1/checklists/requirements.md |
| M | specs/multi-stage4-ui-catchup-v1/spec.md |

Five additions, three modifications, **zero deletions**. Markdown delta: 13 insertions, 12 deletions. New binary footprint: **3,311,384 B**, entirely the five declared PNGs. No source, test, fixture, route, page, Assets, PlayerApi.cs, Program.cs, dependency or project-structure change. The complete `Tools/MultiRackSitkaP1Lab` tree object at source/test HEAD and candidate is identical: `17478c2972b9b91151e95c6056a6359b34a99f28`.

## Committed evidence audit

All filenames below are under `Docs/Handoffs/MultiRack-Simulation-20261116/900-ui-catchup/evidence/`. I read each committed blob, compared it with the checkout bytes, parsed the PNG signature and IHDR dimensions, and checked chunk CRCs and terminal IEND. The actual dimensions match every filename.

| Filename | Actual PNG dimensions | Bytes |
|---|---|---:|
| a-baseline-1280x720.png | 1280?720 | 306,591 |
| a-baseline-1920x1080.png | 1920?1080 | 837,166 |
| b-light-guide-ground-1280x720.png | 1280?720 | 231,626 |
| b-light-guide-ground-1920x1080.png | 1920?1080 | 553,371 |
| c-prune-workplan-1280x720.png | 1280?720 | 223,893 |
| c-prune-workplan-1920x1080.png | 1920?1080 | 537,973 |
| c0-prune-pending-1280x720.png | 1280?720 | 224,676 |
| c0-prune-pending-1920x1080.png | 1920?1080 | 538,097 |
| d-thinning-gaps-deadwood-1280x720.png | 1280?720 | 297,061 |
| d-thinning-gaps-deadwood-1920x1080.png | 1920?1080 | 844,777 |
| **1280?720 subtotal (5 files)** | | **1,283,847** |
| **1920?1080 subtotal (5 files)** | | **3,311,384** |
| **Complete set (10 files)** | | **4,595,231** |

Using conservative decimal MB, every image is below **900,000 B** and the complete set is below **5,000,000 B**. Largest: `d-thinning-gaps-deadwood-1920x1080.png`, **844,777 B**.

I visually opened all five newly committed 1920?1080 images at original resolution, including the two largest (deadwood and baseline). They show the claimed five states, with separate Pending and Approved Prune images. No capability token, address bar or URL is visible. The Light Guide image still exhibits the previously reported F-3 halo. Prior visual inspection of all five unchanged 1280?720 images is reused. No image was regenerated.

All ten PNGs have Git mode **100644** and checkout permissions **0644**. The mode-only commit changes precisely the five new files from 100755 to 100644; their blob IDs are unchanged from `7dabed92`. A literal `/s/<64-hex>` capability-pattern scan of all eight changed files returned **zero matches**. This byte scan supplements the visual inspection; it is not presented as OCR or proof against arbitrary secret formats.

## Required check matrix

| Required check | Result | Evidence and limits |
|---|---|---|
| 1. Delta integrity | PASS | Exact eight-path list above; no deletions or source/test/fixture/Assets/PlayerApi.cs/Program.cs changes. Whole application tree identical to source/test HEAD. |
| 2. Evidence | PASS | Ten committed PNG blobs; all actual header dimensions correct; 4,595,231 B total, 844,777 B largest; all within amended bounds. All five added images visually inspected, including both largest; no visible token, URL or address bar. |
| 3. AC-9 consistency | PASS for operative criterion/artifact; nonblocking summary residue F-7 | Spec:105, checklist:53, implementation handoff ?8:174?183 and ?10:210 agree on ten committed images, both resolutions and corrected bounds; byte claims exactly match. No current evidence section says the 1920 set is external or AC-9 PARTIAL. Scratch copies are explicitly no longer evidence of record. Older checklist footer and T-07 wording remain stale; see F-7. |
| 4. Spec authority / other criteria | PASS | Direct owner packet expressly confirms the owner's choice to commit both resolutions and amend the orchestration cap. Spec:105 attributes this to the owner on 2026-11-23; checklist:53 and handoff:183 agree. This is owner reconciliation, not reviewer waiver or implementer/orchestrator product authority. FR-8:71 and AC-5:101 are byte-identical to the previous candidate; all other acceptance criteria are unchanged. |
| 5. Standing F-2 / F-3 | PASS, remain MINOR/nonblocking | PlayerMultiApi.cs is unchanged. Re-inspected null-readout/empty-Prune paths at :117, :1230, :1550, :2993 and halo styling/drawing at :937, :2175. Prior risk assessment stands; no fix required in this cycle. Both remain disclosed below and in the previous review. |
| 6. Residue / permissions / unrelated changes | PASS | Only five new PNGs in the existing evidence family; no other binary family, deletion, source edit or capability-pattern hit. Final PNG modes/permissions normal; all Markdown modes 100644. `git diff --check 4d7267d0..HEAD` passes. Checkout was clean before the sole addendum write. |

## Authority, closure and remaining findings

**F-1 ? CLOSED.** The literal committed-evidence gap is repaired and the owner has explicitly reconciled AC-9. The amended requirement is met by the committed artifact. No external archive exception or reviewer waiver is needed. The settled owner choices (standing-dead crown + D alongside the log; Unity 2.5 / 5 / 6.5 m pruning schedule) are now also recorded in spec:119?120 and match the prior reviewed implementation.

**F-2 ? MINOR, nonblocking, unchanged:** unavailable Stage 4 readout falls back to legacy wording/empty Prunes. Evidence: `Tools/MultiRackSitkaP1Lab/src/Ui/PlayerMultiApi.cs:1230`, `:1550`, `:2993`; full reasoning is in the previous review. The stale fence prevents unseen ecological execution in that assessed path. No fresh concurrency reproduction is claimed.

**F-3 ? MINOR, nonblocking, unchanged:** oversized light-band text halo. Evidence: `Tools/MultiRackSitkaP1Lab/src/Ui/PlayerMultiApi.cs:937`, `:2175` and committed `evidence/b-light-guide-ground-1920x1080.png`. Numbers remain present; the colour ramp is obscured. The repair adds evidence of the existing issue and does not alter rendering.

**F-7 ? MINOR, nonblocking documentation residue, newly noted in this re-review:**

- `specs/multi-stage4-ui-catchup-v1/checklists/requirements.md:63` still says ?AC-4 and AC-9 with the recorded exceptions?, despite the operative AC-9 now being satisfied.
- `specs/multi-stage4-ui-catchup-v1/tasks.md:92` still describes T-07's goal as ?Eight PNGs?; amended AC-9 and the actual artifact have ten.
- `Docs/Handoffs/MultiRack-Simulation-20261116/900-ui-catchup/MULTI-STAGE4-UI-CATCHUP-IMPLEMENTATION-RESULT-20261123.md:214`, `:236`, `:237` and `:242` retain provisional/conditional presentation-choice wording after the owner settled both choices and spec:119?120 recorded them.

These unchanged summary lines should be reconciled in later authorised documentation upkeep. They do not negate the owner's explicit instruction, the operative AC-9/checklist line, or the measured committed set, and add **no required fix for this cycle**. The review's one-file write grant forbids editing them here. The earlier F-4/F-5/F-6 observations remain carried forward: printed rather than hard-pinned Stage 4 anchor, intentional-409 console-log sequencing, and unpublished original owner packet. No new blocking or major finding.

## Reused evidence and checks not repeated

This addendum reuses the prior review at `42885ebf`: its two successful builds, **76/76 independently selected existing tests**, four observed simulation fingerprints plus P0 SHA before/after, direct API Prune/approve/execute/replay/refusal probes, harness-integrity analysis and inspection of the author's browser artifacts. The author's full **184/184** and **73/73** browser results remain author qualification, not a fresh reviewer execution.

This cycle freshly checks Git delta/tree/blob/mode identity, PNG signatures/dimensions/chunk CRCs/byte totals, all five added images visually, changed-file literal capability patterns, AC-9 wording/authority, unchanged AC-5/FR-8 and standing F-2/F-3 source locations. It does **not** rerun builds, the 184-case suite, the 76-case subset, servers, browser/CDP qualification, native input, Unity or owner playtest. No source or driver change justifies repeating those runs. Prior smoke failures remain disclosed; no blanket green-smoke claim is made.

Manager dashboard/coordination ingestion and publisher synchronization are **NOT VERIFIED**. Updating those records is outside this review's sole-file grant; this remains an operational handoff item for the Manager. The review branch's pushed addendum is durable review evidence, not integration into main.

## INTEGRATION DECISION

```text
INTEGRATION DECISION
Status: APPROVED
Gate: G2 ACCEPTANCE ? repaired AC-9 PASS; sole F-1 CLOSED.
      G3 MEASUREMENT INTEGRITY ? owner-authorised evidence-bound amendment
      checked; prior harness/evidence assessment retained.
      G4 INTEGRATION/CANON ? NOT PERFORMED / NOT AUTHORISED.
Reviewed branch: feat/multi-stage4-ui-catchup-20261123-v1
Reviewed HEAD: e7c1a57da519ed1ee03ccfbad240196a66df0905
Source/test HEAD: 541bb898ba27860dd9ff1159f1abd1ab8dccc410
Context commit: 62e39ec621e38a84030c08b35a171e297056c9ed;
                latest direct owner review packet controls this bounded task.
SPEC:
- ID / status: SPEC-multi-stage4-ui-catchup-V1 / APPROVED
- Conformance: PASS for repaired AC-9; prior scoped functional acceptance reused,
  including disclosed smoke exceptions and evidence limitations.
Reasons:
- Exactly five evidence additions and three Markdown modifications; source unchanged.
- Ten committed PNGs meet corrected count, dimensions and decimal size bounds.
- Owner attribution explicit; no other acceptance criterion weakened.
- No token exposure seen, new binary family, deletion or final permission anomaly.
- F-2/F-3 unchanged and nonblocking; F-7 is nonblocking stale summary wording.
Required fixes / remaining gates:
- Required candidate fixes for this cycle: NONE.
- Owner playtest/acceptance remains pending; candidate is suitable for that playtest.
- Carry F-2/F-3 and F-7 as nonblocking follow-up, plus prior F-4/F-5/F-6 observations.
- Separate Manager G4 integration authorisation and exact integrated-candidate gates.
- Manager coordination/dashboard synchronization remains unverified.
Integration target: designated authorised Multi integration candidate, then main
                    only under a separate G4 grant. Main integration remains UNAUTHORISED.
Integrated HEAD: none.
Post-integration verification:
- On an expressly authorised exact integrated SHA, build solution and Ui, run
  the full Core suite and compare all four fingerprints plus P0 SHA before/after.
- Run/triage both UI smokes without weakening assertions or hiding known failures.
- Bounded browser check of Prune Pending/Approve/Execute/refusal, local/cross-area
  light and deadwood/counters; repeat both layouts if presentation changes.
- Verify committed evidence retention, protected files/manifest and clean scope;
  publish integration handoff and verify Manager dashboard synchronization.
```
