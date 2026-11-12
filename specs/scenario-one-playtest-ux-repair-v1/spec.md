# Scenario One — Unified Personal-Playtest UX Repair V1

**Status: DRAFT / NOT AUTHORISED FOR IMPLEMENTATION.** Prepared by OpenAI ChatGPT GPT-6, GitHub-only, 12 November 2026, claim #440. Input is the twenty non-Objective-2 user findings from protected D-073 Editor playtest, issue #438 and the Manager #424 packet. This spec is subject to Overall Manager review, the latest accepted project decisions, and the protected user playtest exit.

## Authority and boundaries

- Direct user instruction: one accountable non-Objective-2 UX workstream, not independent competing UI fixes.
- Inherits accepted Scenario One UI Visual System V2, Rack Planning & Field Marking V2 / D-064, the three-page onboarding and current D-073 Annual Review guidance. **No source code, scene, save, ecology, MFM or Work Plan mechanics change is authorised by this DRAFT.**
- Objective 2 whole-rack teaching, Crop Tree qualification, sample scaling, four-rack extrapolation and any Objective 2 replacement words belong to #441 / #439. This spec controls only generic presentation/interface affordances, subject to a file/API boundary agreement before source edits.
- Editing geometry to shorten the lower-left rack (#9), changing any stable rack ID or mark/work association (#14), and adding a *new* snap policy (#8) are not presumed approved.
- Keep Legacy40 and frozen Reference Future v1 appearance/behaviour consistent with their actual world; no invented roads, labels, ID mapping or teaching steps.
- Reference evidence: [Manager packet](https://github.com/jermur788/CCF/blob/docs/manager-424-playtest-ux-workstream-20261112-v1/Docs/Handoffs/ScenarioOne-PlayerTest-Release-20261112/MANAGER-424-UNIFIED-PLAYTEST-UX-REPAIR-WORKSTREAM-PACKET-20261112.md), [user findings](https://github.com/jermur788/CCF/blob/docs/scenario-one-d073-live-player-feedback-20261112-v1/Docs/Handoffs/ScenarioOne-PlayerTest-Release-20261112/MANAGER-424-D073-LIVE-PERSONAL-PLAYTEST-FEEDBACK-20261112.md), [copy choices](https://github.com/jermur788/CCF/blob/docs/scenario-one-d073-live-player-feedback-20261112-v1/Docs/Handoffs/ScenarioOne-PlayerTest-Release-20261112/MANAGER-424-D073-WORDING-OPTIONS-AND-OBJECTIVE2-FOLLOWUP-20261112.md).
- Read-only source basis: protected D-073 GitHub candidate integration/scenario-one-personal-playtest-d073-20261112-v1 at 55863f1b7719011bba16b3f3ab377e379c0b988a. **Source inspection is not Unity/native-runtime reproduction.**

## Unified player-facing experience

1. After the third intro page, a novice sees one obvious, actionable Objectives [O] entry. The first-action highlight remains legible without motion; restrained motion is allowed only when Reduce motion is disabled. Opening Objectives leads directly to highlighted Stand Map without an adjacent redundant literal "Next" badge. Continue to a clearly emphasized Plan Racks control on the first relevant guided map visit, without a competing destructive control pulsing.
2. Objectives and Work Plan use readable hierarchy: major page title > navigation/tab names > instructions > supporting metadata. The current Objective 1 diagram and text remain anchored and discoverable across More detail open/close and Esc/reopen; do not blindly restore stale offsets into a rebuilt layout. Map's right instruction panel is distinct from—but does not obscure—the centre overview/legend or map.
3. Key information is represented consistently with a restrained keyboard keycap for genuine key presses (O/M/Esc/P/Tab when they are keys), and ordinary button styling for clickable Back to forest. All instructions retain their actual control meanings; no shortcut remap, automatic action or focus escape.
4. Rack planning communicates a distinct rotate grip with a recognisable hover/drag cursor. Any change to rotation angle snapping first demonstrates a real suggested reference and documented reversible drag-away. The existing tool already snaps **creation** to suggested/other angles while deliberately leaving **rotation free**; a new rotation policy needs Manager approval, not silent coding.
5. After a confirmed plan, a field-marking novice receives an **initial rack entrance target before any spray**, consistent between upper-left HUD and enlarged minimap. Visit labels start at the lower/southern entrance as requested, but are presentation order unless a separately approved identity contract says otherwise. Painting any valid actual entrance must record precisely its own stable rack and tree—not the next suggested rack. After painting, the cue moves to the correct next unmarked entrance; show a useful "all marked" state at completion. Never infer correctness from "5/5".
6. First-use Tree Marking and Work Plan overlays suppress competing underlying UI/text without obscuring their own foreground, stealing forest interactions, or failing keyboard/click containment. Minimap increases significantly in **apparent legibility** from the current 196 UI-unit side, subject to native 720p/1080p bounding-box/occlusion measurements; 2.5× is the player's perceptual target, **not** automatic 2.5× width and height.
7. The Tree Marking introduction becomes shorter, natural and safe; orange entrance marks remain distinct from Fell/Crop Tree marks, marking never removes a tree, Work Plan **Approve** commits intended jobs and separate **Advance one year** executes them. Annual Review is self-contained when already open; no instruction tells the player to open the Work Plan to acknowledge the results they are currently viewing. Preserve the real latest-year acknowledgement, earlier-year read-only behaviour and review-first progression.

## Scope and dispositions by player finding

| # | Category | Intended treatment / status |
|---|---|---|
| 1 | guidance-state + UI | Initial Objectives [O] visible emphasis after third intro; static Reduce-motion alternative; no new persistence assumption |
| 2 | copy/UI | Remove redundant stand-map "Next" badge *in this state only*; keep highlighted actionable target and other appropriate badges |
| 3 | UI/readability | Objectives title, tabs, instructional hierarchy and spacing; distinguish labels by size/weight, not decorative colour |
| 4 | requires native reproduction | Restore diagram layout/scroll invariant on foldout toggle, Esc/reopen at both resolutions; investigate scroll restoration, rackV2 column override |
| 5 | copy + UI | Remove Stand Map "find where to look" heading fragment and exact redundant opening sentence; keycap actual M/Esc, not Back to forest |
| 6 | guidance-state | First guided Plan Rack pulse/static emphasis; only safe target, no action before click |
| 7 | UI/readability | Distinct right-hand Plan Racks instruction panel, preserve map/legend |
| 8 | UI + needs decision | Rotate hover/drag cursor; rotate-angle snap only after reference/threshold/reversibility and authority review |
| 9 | risk/needs decision | Read-only geometry/visual comparison; NO rack length/access alteration |
| 10 | modal UI | Tree Marking intro overlay separation, backdrop input/focus trapping at 720/1080 |
| 11 | copy-only | Natural orange entrance spray instruction, use user-selected option or explicitly approved default |
| 12 | copy + keycap | Contextual lead after rack confirmation, display P as a key, preserve actual spray semantics |
| 13 | copy-only + safety | Requested prose trim with check for unique lost guards; preserve mark-not-cut and approve-versus-advance sequence |
| 14 | safety/guidance-state + runtime | **STOP GATE**: pre-first-mark HUD/minimap, lower-to-upper visit numbering, next target, exact actual rack/tree/order association, enlarged minimap |
| 15 | UI/keycap | Tab keycap after 5/5 without claiming identity validated |
| 16 | modal UI | Work Plan backdrop dims background without click/focus leak |
| 17 | UI/readability | Larger Work Plan tabs/headings/body/spacing/window fit without obscuring approval gates |
| 18 | guidance-state | Suppress/replace Work Plan [Tab] prompt when Annual Review already open; preserve latest-year acknowledgement |
| 19 | copy-only | Remove redundant Annual Review footer phrase, retain WORK DONE/MONEY/FOREST actual sections and acknowledgement |
| 20 | copy-only | Replace "One thinning..." with user-selected phrase; do not falsely say rack clearance is thinning |

## Non-negotiable correctness constraints

- A pending or approved rack work order continues to point at the rack and tree it was created for; cue labels cannot be used to mutate stored identity. If the one-to-one mapping is false or uncertain under native repro, **STOP** affected approval/resolution and escalate to Manager for specific product/data decision.
- Marking requires genuine live-target/range/mark eligibility; no auto-spray, auto-felling, auto-confirm, auto-approval or automatic new-forest geometry. No weakening of D-064 rack plan editing, entry/exit, Legacy40/Reference, supported old-save/current-save, Work Plan approval-versus-year-advance and reference/world identities.
- Show focus, enabled/disabled/action meaning without relying on colour or motion alone. Reduce motion is currently a local preference; any change to first-visit memory scope needs explicit approval.
- Crossscreen semantic, layout and keycap changes must use the existing UI Visual System V2 style authority; no new standalone palette or broad input-remapping project.

## Open product/design decisions (do not default silently)

1. **#8:** whether rotating a rack should magnetise to an existing suggested direction (existing creation snap uses the same candidate angles but rotate explicitly disables it); what tolerance/release cue/override semantics can be safely added without changing geometry policy? Cursor affordance itself may proceed within approved UI scope.
2. **#9:** after a screenshot and source geometry comparison, whether a lower rack may terminate at the central track; this is **outside** UX repair coding pending a new D-064/spec decision.
3. **#14:** whether the observed bottom-first label mismatch is harmless visit-order versus stored ID, a guidance-state omission, or an actual mark/work mapping defect. No ID migration or reassignment without separate approval.
4. **Wording:** three unresolved user-choice groups (orange spray instruction, "marking cuts nothing" substitute, Annual Review closing). Manager defaults 2 / 3 / 1 are proposals, not final accepted source copy.
5. **Minimap layout:** native bounds and overlap determine the proportionate resize and whether 2.5× is perceptual/area/linear; do not precommit a pixel dimension.

## Acceptance and stop rules

The checklist in checklists/requirements.md is binding only **after this spec is approved**. Pass requires source/offline assertions for text, guidance-state and stable mapping, plus targeted **native** 1280×720 and 1920×1080 screens and clicks/keyboard; no full Tier-3 duplication, no Windows build. Runtime findings stay OPEN until observed in a safe isolated Unity lane after user Editor release. User still decides personal UX acceptance; a docs-only verdict cannot claim integrated, verified or accepted gameplay.
