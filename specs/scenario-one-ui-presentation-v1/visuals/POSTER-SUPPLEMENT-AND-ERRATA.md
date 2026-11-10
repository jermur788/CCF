# User-supplied three-poster visual supplement — 10 November 2026

**Status:** APPROVED AS **VISUAL STYLE / COMPOSITION REFERENCE ONLY**. This document records the three 2× PNGs uploaded to the Manager chat following publication of the integrated UI spec. **Not Unity renders, not an implementation report, not source data, not game controls.**

## Latest visual preference — user-selected image at 19:40 UTC

The user supplied a **new image in chat** at approximately 19:40 UTC on 10 November 2026 and said **“this is even better.”** This is the **preferred visual-composition direction** over the three earlier posters **where their aesthetics/layout differ**, provided Sol can inspect the actual image. It does **not** override the accepted requirements, authoritative Unity controls, valid state handling, or the 11 content errata in this document.

**Asset-custody limitation:** This newly preferred inline image was visible in the conversation but has **not been committed as a file in this Git branch**, and the Manager could not verify a named local/transferable backing file for it. Its pixel dimensions, hash and filename are **not established**. Do not imply Sol/Fable can open it from Git. If an exact user-provided copy becomes available in the local verification folder or worker input, record its observed filename/hash/dimensions in the worker handoff and use it as the foremost *visual style* reference. If not available, proceed from the four committed SVGs and three verified poster references; mark exact-new-image comparison **NOT INSPECTED** rather than inventing fidelity. No delay to source work is required.

## Access and custody

The user reports the original PNG files are locally available under:

`/media/jer/Files/CCF/Verification/CCF-362-FableUXReview-20261110/spec-posters/`

| Poster | Actual supplied pixels | Upload SHA-256 | Role |
|---|---|---|---|
| `poster-1-visual-reference.png` | 2048×1365 | `94c9a096f88d7f12270ea0e5bf0677cafaf93537cf45cbb4b197c60d2a2be836` | Combined HUD + Work Plan + Annual Review and journey |
| `poster-2-update-specification.png` | 2048×1365 | `3a03eea5ab64ba602922abee0a8c19973638d4a6c9cdfdf0bdef3786cb8c6147` | UI components/detail screen, bottom action examples, consistent typography |
| `poster-3-ui-update.png` | 2048×1152 | `972318053cc4e1a2586765d81dfd686ce22cd4b1cbc1a821d5334230f33e58c9` | Three-panel visual hierarchy and Work Plan/Annual Review examples |

**Source-of-truth caution:** the local filesystem paths above are *user-reported* and are **not verified by the GitHub Manager**. Sol and Fable must `test -f`, inspect dimensions and compute hashes before treating their own local copies as these exact visuals. Do not assume the GitHub documentation branch stores the binary PNGs (it does not). The Manager chat also has the three uploaded PNGs and a ZIP copy to pass into a local worker if needed. If unavailable locally, continue from the **four committed SVGs** and explain missing poster access; do not block all source work or pretend they were inspected.

## Priority of visual guidance

1. **Current, inspected Unity/game source and accepted product rules**, especially actual controls, calculations, annual history and Work Plan commitment.
2. [Approved integrated specification](../spec.md), [requirements](../checklists/requirements.md) and current [four committed SVGs](./01-hud-inspection.svg) (also [Work Plan](./02-work-plan.svg), [Annual Review](./03-annual-review.svg), [Navigation](./04-navigation-and-guards.svg)).
3. **Three uploaded PNGs** for visual finish, typographic hierarchy, card/table/button styling, spacing, palette and examples of concise information.
4. Older Fable design-only concepts / unverified screenshots as background only.

Do **not** copy a poster illustration over a current authoritative gameplay state. None of the images proves the new UI has been rendered in the Editor. All illustrative values must come from the current candidate at runtime.

## Adopt from the posters — visual direction

- Unified forest-green/cream visual system across HUD, Work Plan, Annual Review, and existing full-screen shell; restrained use of blue Crop state, red confirmed player Fell mark, amber **proposal** (distinct from Fell), and neutral unmarked state, *always with text*.
- Consistent accessible text sizes and row spacing; amounts right-aligned in real tables; costs/revenue/net labelled with correct sign; totals visually distinct.
- Selected-tree card with ranked neighbour rows and computed rest count, on-demand detailed view under **`[V]`**, and one current-plan effect summary with caution where relevant.
- Current action/real objective upper-left, **current Year + Cash only upper-right**, helpful bottom action/keycap bar filtered by active gameplay state.
- Work Plan presents *actual proposed* operations with authoritative quote/cash outlook and a readable commitment footer; all existing job groups remain accessible.
- Annual Review quickly presents **Work done | Money | Forest changes** with descriptive labels, then real detailed recorded history. Use actual record/snapshot provenance.
- Quiet primary CTA, no unnecessary page decoration, screen-to-screen typography consistency.

## Errata and non-authoritative examples — binding for both workers

1. **Incorrect keys.** The images use `[F] Mark to fell` and other example Work Plan shortcuts. The real current source uses **`[X]` Fell, `[C]` Crop Tree when available**. Do not introduce novel binds from a poster; the bottom bar must reflect actual handlers (with Rack-first suppression).
2. **Two distinct percentage denominators.** Tree-neighbour percentages (e.g. 13%, 9%, 6%) are **shares of local competition index**. Percent potential diameter growth withheld (the sample 54%) is a separate model quantity and cannot be calculated by summing those shares. Some posters show “remaining competition 26%” and “total competition 54%” despite incompatible interpretations; do not copy these totals. Use precise source-backed labels, live values and correct computed remainder.
3. **Plan-effect contradiction.** Poster 3 shows a neighbour visually Fell-marked while the card says “No change yet”; poster 1 uses a speculative 54%→42% change after another tree's felling. Always calculate/retrieve the authoritative effect for the *actual pending marks* or report a supported unavailable/rounded-no-change state. No automatic Fell recommendation.
4. **Unverified operations and forestry outcomes.** Examples such as “Release (ring-bark),” area-based selective thinning, “regeneration support,” “natural regeneration established,” and per-area prices are *not assertions that the current simulation supports these exact jobs/results*. Use actual `ScenarioWorkType`, existing operations, quotes, records, wind/browsing limits. Never add a mechanic to match a mock-up.
5. **Made-up ecological, financial and forest totals.** Year 5, €12,400, income/expenses/net, 72%/78%/82% canopy, regeneration increase, 12/18 m³ deadwood and units/rates are illustrative. Do not hardcode or infer these from poster text. A numeric biodiversity/habitat/forest-health score remains **explicitly excluded** from the global HUD.
6. **Annual Review workflow trap.** Poster 3 shows **“Continue to Year 6”** on Annual Review; this is **not the approved interaction**. The latest unread review uses its existing acknowledgement `I've read the annual results`; `Back to forest [Esc]` remains usable before ACK; **Advance one year belongs to Work Plan** under the existing gates. Historical years never acknowledge latest.
7. **Approval remains a commitment.** “Approve Work Plan” cannot pulse, flash or act as automatic progression. Keep all minimum-cost/quote/eligibility, pending vs approved, Advance warning and safe-return decisions. The poster may omit controls for compositional clarity; do not remove required controls.
8. **On-tree competitor shares remain conditional.** Their illustration does not authorise implementation regardless of readability. Only include after actual 720p 3D evidence of unobstructed labels and proper depth. Preserve rank/share list in card if omitted.
9. **Menu consistency still required.** The new PNGs have fewer visible full-screen navigation tabs than the integrated four-SVG spec. Preserve the agreed consistent Objectives / Stand Map / Work Plan / Annual Review route names and safe NEXT hierarchy, when allowed by existing state. Do not remove menus or review-first priority for appearance.
10. **Color is not sole semantic signal.** Blue/red/amber marking chips need text/state and need not match exact sample positions in the forest background. Foreground mock trees/markers are stitched/composite; positions, ranks and IDs are not a new geometric fixture.
11. **No false source precision.** Wind “unchanged” in poster is not evidence of real exposure. Storm model disabled does not imply safety; only show current authoritative forecast/warning or a clear unavailable/not-simulated label. Work Plan outcome explanations remain hypotheses, not guaranteed biological responses.

## Sol implementation evidence

The implementation report must name this supplement, state whether all three actual local PNGs were inspected and hash-matched, explain visual adoption/deviations, and compare actual 720p/1080p nonblack Unity images against the **approved specification and the four SVGs**. Any PNG-based mistake must be corrected toward the game, not treated as visual acceptance.

## Fable independent review evidence

Fable must independently mark what came from the posters versus source state, and specifically check all eleven errata above. A good aesthetic match cannot compensate for wrong `[F]` keys, fake %/net/canopy, automatic year progression, destructive/commitment CTA changes, or inaccessible `[V]` details.

**Authoritative Sol dispatch:** [Manager Sol packet](../../../Docs/Handoffs/ScenarioOne-UI-Presentation-20261110/MANAGER-SOL-UI-PRESENTATION-V1-IMPLEMENTATION-PACKET.md).

**Authoritative Fable dispatch:** [Manager Fable packet](../../../Docs/Handoffs/ScenarioOne-UI-Presentation-20261110/MANAGER-FABLE-UI-PRESENTATION-V1-INDEPENDENT-REVIEW-PACKET.md).
