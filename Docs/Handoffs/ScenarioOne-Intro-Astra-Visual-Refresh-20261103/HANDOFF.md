# Scenario One — Astra three-page introductory visual refresh

**Date:** 2026-11-03  
**Authority:** direct user instruction and three attached user-approved Astra screenshots; subordinate to protected CCF decisions only where no conflict.  
**State:** APPROVED REFERENCES + APPROVED COPY DIRECTION / READY TO DISPATCH — **NOT IMPLEMENTED, NOT UNITY-VERIFIED**.  
**Claim:** [#243](https://github.com/jermur788/CCF/issues/243) ([READY] until Astra starts).  
**Prior manager transition:** [2026-11-03 Playtest Visuals](../Manager-Chat-Transition-20261103-Playtest-Visuals/HANDOFF.md).

## 1. Plain-English task and limits

Replace the **visual appearance of the three introductory pages** in the Scenario One Unity simulation with the **user-approved Astra results** shown in the three supplied 1672×941 PNG screenshots. Match the in-game portion of each screenshot as closely/exactly as the real interactive Unity UI permits. **Do not substitute an entire screenshot for the UI**: preserve live accessible text, page controls, input/Continue behaviour, and independent forestry artwork regions. Treat the screenshots as the new approved visual reference, superseding the 25 October reference visuals for the affected presentation only.

The user separately and explicitly directs **one additional line on Page 1**, in the same blue/teal accent used by other headings. This new instruction **overrides the screenshots wherever that line is absent**:

> **The forest is renewed gradually over time, creating a mixed-aged, more resilient and productive woodland.**

**Exact wording and punctuation are mandatory.** Do not paraphrase, capitalise differently or silently convert “mixed-aged” to “multi-aged”. Insert it as a **standalone line in the top explanatory area, after the existing explanatory paragraph and before the imagery/divider**, visually emphasized with the already-qualified heading/accent blue/teal. Do not introduce a new colour token. The original definition must remain intact. Adjust layout/spacing conservatively to prevent overlap/overflow, and ensure the line remains readable at 1280×720 and 1920×1080; its single conceptual line may wrap naturally only if required at the smaller resolution.

The prior two-sentence Page 1 copy-density limit is superseded **only by this user's added one-line direction**. No other redesign or copy rewrite is approved beyond fidelity to the attached screenshots and the listed corrections.

## 2. Authoritative visual inputs: exact screenshot mapping

These images were uploaded to the **originating Manager conversation** on 2026-11-03. Their *original upload bytes* are preserved inside the downloadable conversation attachment `ScenarioOne-Astra-Intro-Approved-References-20261103.zip` and identified by SHA-256 here. The ZIP must be **attached/transferred into the Astra execution chat**, because this GitHub Markdown file does not itself contain the image binaries. Do not guess, remake or claim you have the images merely because you can read this handoff.

| Intended live page | ZIP filename | Original upload name | 1672×941 PNG SHA-256 |
|---|---|---|---|
| Page 1 — What is Continuous-Cover Forestry? | `Page-1-What-Is-CCF-APPROVED.png` | `ChatGPT Image 3 Nov 2026, 17_06_57.png` | `c082fd5a6ab3c7da513f69ad033cf9ca9a17a6171ad8085a88e1587117379aad` |
| Page 2 — How CCF works over time | `Page-2-How-CCF-Works-APPROVED.png` | `ChatGPT Image 3 Nov 2026, 17_07_10.png` | `ee3a8fb0c82f0969d0691bbe12f001db38e8af615a651c662e57000ba53e24d8` |
| Page 3 — Scenario One — Transforming a Sitka Plantation | `Page-3-Scenario-One-APPROVED.png` | `ChatGPT Image 3 Nov 2026, 17_06_46.png` | `8e289b21459c4ee6cf49660bc6e0943decefd412e2dea9783169deb11186059c` |

**ZIP SHA-256:** `5d20e59dda3c32a635186b34fa30165904653486c67fbb17898fcda22325431f`. Original screenshots include the Unity Editor frame. **Only the in-game visual area is normative**; do not display toolbars, taskbars or rasterised app chrome. Screens are visual references; final assets may need art-only isolation to retain the live UI architecture. If exact fidelity is not possible with screenshots alone, report the specific discrepancy and seek Manager/user acceptance rather than silently weakening the visual target.

The user has approved **these** three Astra versions; do not resume generating alternate mockups or request another approval of the reference images. The one Page 1 addition above is also approved.

## 3. Page-specific acceptance

### Page 1 — definition / progression

- Keep the two existing explanatory sentences verbatim and append the **new exact sentence** as a separate blue/teal line in the top text zone.
- Use the approved screenshot's three forests: dense uniform Sitka starting stand → selective transformation with mature Sitka retained throughout → developed, continuously wooded mixed-age and mixed-species structure.
- **Each panel is a continuous forest slice**. Do not put all saplings in the foreground and all mature trees at the rear. Multiple tree sizes/ages should occur across front, middle and back; do not create an obvious clearfelled front section.
- Preserve the three captions and four short principles; match the screenshot's composition, shapes, colors and relative spacing.
- Keep the Page 1 title and footer and navigation as live UI.

### Page 2 — repeated management cycle

- Match five cards/imagery in the approved screenshot, including the revised small icons and their positioning.
- The retained/selected sapling must **not** have an unintended blue glow; no doubled/stacked process icons in cards 2 and 3. Keep recognisable distinct selection and canopy-opening imagery.
- The Page 2 card-4 text shown in the approved screenshot is the target: **“Encourage natural regeneration and establish other species for a diverse forest.”** It must fit entirely inside card 4; no clipped glyphs or overlap.
- Preserve the five-step order, the Oak/Beech broadleaf illustration, footer, buttons and explanatory meaning; no new controls or scenario-specific lesson text on this generic page.
- Verify card text readability and page controls at both resolutions.

### Page 3 — Scenario One task

- Match the approved screenshot's before / repeated selective management / target-forest composition.
- The right forest must clearly contain **several tall, imposing, retained mature Sitka** alongside identifiable established Oak/Beech and smaller regeneration. Do not make all Sitka juvenile or all saplings front-loaded. It remains a productive mixed-species forest, not a clearfell-and-replant/native-only outcome.
- Preserve live Scenario One goal, initial/target state and key-idea text and Back/Begin interaction; no text clipping.

## 4. Existing implementation and expected file set

Existing approved onboarding spec and references:

- `specs/scenario-one-three-page-onboarding-v1/spec.md`
- `specs/scenario-one-three-page-onboarding-v1/production-copy.md`
- `specs/scenario-one-three-page-onboarding-v1/references/reference-manifest.md`
- `specs/scenario-one-three-page-onboarding-v1/implementation-notes.md`

Current Unity implementation:

- `Assets/ForestPrototype/UI/ScenarioOneOnboardingCopy.cs`
- `Assets/ForestPrototype/UI/ScenarioOneOnboardingView.cs`
- `Assets/ForestPrototype/UI/OnboardingIcon.cs` (if needed)
- `Assets/ForestPrototype/UI/Resources/Onboarding/page-1.png`, `page-2.png`, `page-3.png` and matching `.meta` files

**Important architecture:** the current PNG pages are forestry-art atlases, but body copy, titles, buttons and icons are drawn as **live UI Toolkit elements**. Do not bake editable text/buttons into PNGs. Preserve existing GUIDs/meta and the qualified V2 charcoal/neutral/teal styling, with only the user-authorised visual changes. Source and reference records should distinguish the **prior frozen 25 October references** (preserved for history) from **new approved 3 November screenshots and post-change production assets**; do not overwrite historical hashes as though they always referred to the new images.

Exact asset path set to be changed is to be enumerated by Astra after inspection; if it turns out a screenshot is not directly reusable as a runtime art atlas, use an art-only preparation stage with recorded crops/provenance and visual proof. Keep one writer per file set.

## 5. Routing, lane and safety

- **Purpose:** implement the now-approved visuals and copy in the personal-playtest candidate lineage.
- **Difficulty:** **Medium** — bounded three-screen UI/art change, but exact visual matching, atlas extraction, live-UI positioning and multi-resolution checks require careful Unity rendering.
- **Best-suited provider/model:** **OpenAI Astra**, as expressly requested by the user, using a local code + Unity Editor capable lane. Astra must actually have those capabilities; a browser-only chat cannot claim Unity verification.
- **Task class:** **PACKET ONLY**, refinement of an already-approved/spec'd three-page onboarding flow. If inspection reveals substantial UX/persistence/simulation scope, stop and return to Manager.
- **Base:** published `integration/scenario-one-player-test-ready-v1`, inspected GitHub head on handoff creation `72b4efbd1c653fa2231f632a79030a83ff76f833`. Reconfirm exact base and context at task start; do not silently repin to an unrelated head.
- **Proposed branch:** `feature/scenario-one-intro-astra-visual-refresh-20261103`; **proposed isolated worktree:** `/media/jer/ZX20/CCF-Intro-Astra-20261103`. Create only after checking existing dirs/worktrees/locks; if unavailable, register a non-overlapping alternative before production writes.
- **Ownership:** Astra owns only the approved onboarding files + task verification/handoff in its own lane after the #200 Manager assigns them. `Docs/Project/coordination/active-tasks.md` is **sole-writer #200 Manager**; Astra **must not edit it**.
- **Claim:** [#243](https://github.com/jermur788/CCF/issues/243) is **[READY]**. Upon actual start, inspect live coordination, mark claim **[RUNNING]** with provider/model/branch/worktree/starting HEAD. If blocked by assets/ownership/capability, mark **[BLOCKED]**. At handoff, mark **[HANDED-OFF]** and link GitHub result; Manager reviews/closes.
- **Unity serialization:** verify current machine-wide owner/lease before starting Editor; never kill another owner's process or overwrite dirty work.
- **No production implementation is claimed by this handoff.** The user approval gate is satisfied for these screenshots and the one new line, *not* for unreviewed alternatives.

## 6. Verification, report and integration gate

1. Compare all three actual Unity pages against the screenshots at **1280×720 and 1920×1080**, with side-by-side comparison images and explicit evidence for the new Page 1 line's colour/fit and Page 2 card-4 overflow.
2. Verify screenshot-level hierarchy, exact approved visual changes, forestry correctness and illustration distribution across depth; no unwanted blue glow/stacked process icons.
3. Retest New Scenario Page 1→2→3→forest; Back; Next/Begin; focus/input/cursor; and existing-save Continue does not replay onboarding; report actual Editor/runtime results, not assumed outcomes.
4. Preserve 3-species sim semantics, economy/history, save format, Reference Future v1 bytes, Legacy40 compatibility, protected world anchors, Stand Map and MFM. Because this is a **UI/art-only** change, use proportionate focused tests. Do not rerun full Tier-3 merely to get another green report.
5. Supply the exact changed-file list and screenshot/asset hashes, visual comparisons, test commands/results and limitations. A substantive result requires a **GitHub Markdown handoff** under `Docs/Handoffs/ScenarioOne-Intro-Astra-Visual-Refresh-20261103/IMPLEMENTATION-HANDOFF.md`.
6. The Manager then performs review and issues APPROVED / NEEDS CHANGES / REJECTED; **do not automatically merge** into the published playtest candidate. Independent review required before final integration, proportionate to actual changes.
7. Update canonical documentation/screenshot reference manifest only for accepted new visuals and copy, preserving old history. Dashboard sync/readback must be reported; never manually edit generated dashboard snapshots.

**Build and player-test gates:** The current published Scenario One candidate remains qualified for user personal testing, which is **IN PROGRESS, not complete/accepted**. **No Windows build**, even if this task becomes qualified, until user completes the personal playtest and *then explicitly authorises* Windows. No fresh standalone Player build is part of this task. Do not alter already-accepted Tier-3, #232, Rack V2, parity, S1-C/D or MFM decisions or silently rebaseline tests. Known MFM-11 Covered CI parity not demonstrated and three pure-Beech R2 misses remain accepted limitations.

## 7. Paste-ready Astra dispatch

> **Astra — implement the user-approved Scenario One three-page introductory visuals in Unity.** Read the complete durable task at https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Intro-Astra-Visual-Refresh-20261103/HANDOFF.md and claim https://github.com/jermur788/CCF/issues/243. The three approved screenshots are attached as `ScenarioOne-Astra-Intro-Approved-References-20261103.zip` and must be your exact in-game visual targets. **Additional user instruction:** on “What is Continuous-Cover Forestry?” keep the existing paragraph and insert exactly “The forest is renewed gradually over time, creating a mixed-aged, more resilient and productive woodland.” as a separate highlighted line at the top, in the existing blue/teal heading accent. Preserve live UI, match the screens, verify at 720p/1080p, publish a GitHub implementation handoff, and do not merge without Manager review. Refresh live locks/claim before edits. The published Scenario One candidate remains under personal playtest; **no Windows build**.

**Exact user action required:** open the Astra execution chat/lane, paste the dispatch above, and attach the ZIP with the three screenshots. There is **no further image/copy approval required** for the exact references/directive already accepted here. If Astra cannot reach the local repository/Unity worktree or cannot reproduce the images faithfully from the screenshots, it must mark the claim blocked and report the discrepancy.

## 8. Manager release amendment — 2026-11-03, after Astra preflight

**Ownership blocker resolved in canonical coordination.** The Overall Manager (#200) has assigned #243 as **sole writable owner** of the onboarding code, page-1/2/3 runtime art, matching metadata only where necessary, amended production copy, new non-destructive reference records, focused onboarding verification files, and this task's implementation handoff. The allocation, exact paths, branch/worktree plan and conditional Unity gate are in live `Docs/Project/coordination/active-tasks.md`; refresh it at start. No production files have been changed by the Manager.

**Unity execution:** Astra #243 is authorised as the next isolated onboarding Unity execution owner **only after** a local preflight proves no personal-playtest session or other machine-wide Unity owner is active, and Astra acquires the actual machine lock. This remote Manager decision **does not claim that lock is free, held, or acquired**. Do not interrupt the user playtest, kill processes, re-use another person's lane, or start Unity while another lease exists.

**New independent blocker (Astra #243's newer comment):** Astra's existing execution chat has only the read-only Game Dev project mirror attached. Its managed-worktree operation cannot use the real CCF checkout until **the actual CCF repository is attached/opened as that chat's writable workspace root**. Astra must treat this as an environment-level BLOCKED status, not infer permission to bypass managed-worktree safety. The existing integration checkout has unrelated untracked `Scenario1V1/` and must remain untouched. After correct workspace attachment, use an isolated worktree and then mark #243 [RUNNING] with actual provider, worktree, branch and starting HEAD. The user may need to open Astra at the correct repository workspace; no new visual approval is needed.

**Latest artwork-source precedence:** Astra reports the three latest screenshot originals generated in its execution chat are accessible locally and **their exact bytes differ from the earlier reference ZIP/hashes in this handoff**. Latest direct user instruction approves those latest three Astra originals. For the implementation, those latest local originals take precedence over the earlier ZIP **if they are the three images the user most recently identified**; preserve both source sets and record hashes of actual selected originals in the returned handoff. Do not falsely claim the earlier hashes describe the newer bytes. The exact highlighted Page 1 sentence remains unchanged. If visual identity of a supplied image remains genuinely unclear, stop only for that specific discrepancy.

**Status after amendment:** FILE OWNERSHIP ASSIGNED; UNITY USE CONDITIONALLY RELEASED / NOT ACQUIRED; **WORKSPACE ATTACHMENT BLOCKED**. This amendment does not change accepted implementation or verification decisions. No Windows build.
