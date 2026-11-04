# Final independent review of the three overnight DRAFT UX specs (#270)

**Status:** REVIEW COMPLETE. **No spec is approved, no default is decided, no implementation is authorised.**
**Date:** 2026-11-04. **Claim:** [#270](https://github.com/jermur788/CCF/issues/270). **Parent:** [POST-OVERNIGHT-REVIEW-AND-ROUTING-PACKET.md](POST-OVERNIGHT-REVIEW-AND-ROUTING-PACKET.md).
**Orchestrator:** DeepSeek DPC. **Ledger:** [ROUTING-LEDGER.md](ROUTING-LEDGER.md) EVENTS 031–040 (this branch).
**Review principle applied:** each spec was reviewed by the **opposite model family of its author**, read-only, with no repository edit.

## 1. What was reviewed and by whom

| Spec | PR / head | Author family | Independent reviewer | Verdict |
|---|---|---|---|---|
| #257 Player input settings & feedback | [#263](https://github.com/jermur788/CCF/pull/263) `ae7944aa` | Claude/Sonnet | OpenAI GPT-6.1 Medium | **APPROVE-WITH-CHANGES** |
| #258 Next-action guidance | [#265](https://github.com/jermur788/CCF/pull/265) `a173192f` | OpenAI Codex | Claude Sonnet 5.5 Medium | **NEEDS CHANGES** (3 High) |
| #259 Tree-mark language | [#266](https://github.com/jermur788/CCF/pull/266) `e13b1c5f` | OpenAI Codex | Claude Sonnet 5.5 Medium (after a routed OpenAI same-family first pass) | **APPROVE-WITH-CHANGES** |

All three reviews were read-only: no branch, no PR, no file change, no Unity. All three specs remain **DRAFT**; `main` and the playtest candidate were not modified by this claim.

**Routing error recorded, not hidden.** #259 was first sent to an OpenAI reviewer although its author is OpenAI. The reviewer detected the conflict, refused to certify itself as the independence gate, and a Claude review was dispatched. The OpenAI pass is retained as a useful same-family technical review. Full account: ledger EVENT 035.

## 2. Verdicts in one line each

- **#257 — structurally sound draft, two material gaps.** The collision-totality rule survived every attempted counterexample; the remaining problems are **governance** (the spec's §8 ranks *settings sources*, not project authority, while its handoff claims the project-level point fixed) and **tester isolation** (per-device storage does not isolate testers sharing one OS account).
- **#258 — right principle, three blockers before the Manager can decide.** The visibility/occlusion rule genuinely fixes the earlier Boolean-ranking defect, but the spec promises keyboard/focus behaviour the UI does not have, quietly requires restyling destructive consent buttons, and its totality depends on a backstop the text does not present as load-bearing.
- **#259 — arithmetically sound, methodologically incomplete.** Every published colour number reproduces; the real defects are in *how the claim would be tested* (region medians, a memorisation confound, no disposition for human accessibility feedback) and in *what is left ungated* (map-colour collisions, bark-dependent 3:1 feasibility).

## 3. Reproduced numbers (all re-extracted, not copied)

### 3.1 Control inventory (#257 / Method A)

| Base | Occurrences | Lines | Files |
|---|---:|---:|---:|
| candidate `72b4efbd` | 71 | 63 | 16 |
| historical main `8f0f2301` | 68 | 60 | 14 |

Reproduced identically by the reviewer and by DPC with
`git grep -o -h -E 'wasPressedThisFrame|wasReleasedThisFrame|isPressed|\.ReadValue\(' <ref> -- 'Assets/ForestPrototype/*.cs' | wc -l`.
**Method B (73 / 61) remains OPEN.** Three parties have now failed to reproduce the pair; the reviewer's closest reconstruction gives 73 occurrences but **63** lines. The missing artefact is the original command together with its counting definition. Neither figure is withdrawn and neither may be deleted from the spec.

### 3.2 Colour arithmetic (#259)

| Pair vs Fell `(1,0.35,0.05)` | Contrast | ΔE76 |
|---|---:|---:|
| current rack `(0.95,0.28,0.025)` | 1.174844 | **6.004234** (fails the ≥40 floor, correctly) |
| proposed fallback `(1,0.08,0.58)` | 1.157919 | **78.077696** (full-precision matrix) / **78.1025** (truncated IEC matrix) |

**Constant-set correction (supersedes EVENTS 030/035 where they conflict).** `78.10` and `78.0777` are the same result computed from different sRGB→XYZ matrices; across common constant sets the value spans **78.0744–78.1025**. The correct record is: publish the constant set and a **±0.05** tolerance. Neither figure is an error. The ≥40 floor is insensitive to the difference.

**Pink-vs-3:1 correction.** "No recognisably pink colour reaches 3:1" is **false** as a universal claim: a dark rose `(0.45,0.02,0.2)` reaches **3.7287:1**. Only *hot/light* pinks fail (hot pink 1.19:1, light pink 2.32:1). The spec's own correct phrasing — 3:1 requires `L ≥ 0.954` or `L ≤ 0.0616` — is the one to keep.

**Bark sensitivity (reviewer's neutral-grey model, not measured bark).** At 3:1, only bark grey ≈0.1 supports all three marks; at 0.3 no mark reaches 3:1. Because Beech is in the fixture list, **the edge/backing is effectively mandatory, and a single edge cannot satisfy both pale and dark bark.**

**Colour-vision effect on rack vs Fell (approximate, Machado severity 1).** Pink helps protan/deutan dramatically (ΔE 82.7 / 65.8) but **not** grayscale (4.32) or tritan (8.9). Shape, not colour, carries those modes.

### 3.3 Source facts re-verified at `72b4efbd`

Rack = four vertical patches `(0.95,0.28,0.025)`, URP Unlit. Fell = rotated cube + ground disc `(1,0.35,0.05)`, **no diagonal trunk slashes**. Crop = blue `(0.15,0.4,0.95)` diamond/disc **and trunk band**. Full map: Fell red ring `(220,76,62)`, Crop blue ring `(78,140,236)`. Minimap: routes only, no per-tree mark glyphs. **Every UI button is `focusable = false` by design** (`UI/UiKit.cs`).

## 4. Findings that must be fixed before any spec could be approved

### #257
- **R1 (major) — governance.** Add the project authority hierarchy (user instruction → Decision Log → Current Milestone → Game Brief) separately from the settings-source ranking, and correct the handoff's claim that it was already fixed.
- **R2 (major) — tester isolation.** State the actual sharing model. If testers share one OS account, per-device storage isolates nothing; require separate stores/profile selection and acceptance coverage.
- **R3 (minor) — Method B provenance** (above).
- **R4 (minor) —** `UI/StandaloneSessionMenu.cs` explicitly sets `button.focusable = true` "while all forest input is paused"; the blanket "focus handling not found" is overstated. Keep actual focus order/visible focus UNVERIFIED.

### #258
- **R-01 (High) — FR-6 is unachievable as written.** `UiKit.Button` sets `focusable = false` on every button; "visible focus indicator", "Enter/Space activate", "keyboard reachable" cannot exist without a production focus-policy change. Make that change an explicit Manager decision or state the feature is mouse-only for now.
- **R-02 (High) — the destructive control is the primary one.** "Cancel approved work" / "Reset layout" / "Reopen the plan" are `btn-primary`; the safe choices are `btn-ghost`. FR-2 therefore requires a consent-UX restyle that the spec does not acknowledge or assign.
- **R-03 (High) — FR-2 is not total by occlusion alone.** `RequestReset()` sets `ResetPending` without clearing `consentRetry`, and Reset stays enabled under the consent panel; both render in the same card. The "ambiguous → no primary" backstop is load-bearing and must be presented as such, with an AC crossing Consent with Reset.
- **Medium:** "owns guidance" vs "owns input" conflated; any map pointer-down silently clears prompts (so they are not "pending until answered"); first visit auto-opens Help so AC-2's first cue is Help Continue; Help can be force-closed by navigation; `RackPlanProblem` / `RackPlanningAvailable` gate Approve and Plan Racks; `rackMode` is unsaved view state so "derived from saved state" overstates; the 16 physical-px target is below today's 1280×720 baseline; **no screen-reader delivery mechanism exists in source**; diagram acceptance is judgement-only.

### #259
- **R1 — background method.** Region medians can hide an erasing bark patch. Adopt local windows along the stroke, a 10th-percentile worst-case background ring, failure aggregation, and a committed harness with a **negative control that must fail** and a positive control that must pass.
- **R2 — recognition confound.** The same captures are shown in all five modes with one answer key to the same two scorers, so "100%" can be recall. Use different capture sets per mode, separate the sessions, fix per-cell item counts, and either recruit a real colour-vision participant or label the result **"simulated legibility only"**.
- **R3 — accessibility disposition.** A reported barrier alongside a passing score can currently be dismissed. Define barrier severity, log every reported barrier as an open item, retitle FR-4 away from "Accessibility", and never describe a pass as "accessible".
- **R4 — inventory.** Replace prose with a line-cited table (~20 colour-dependent player-facing strings; full list in the review).
- **R5 — edge is mandatory**, with two-tone or fixture-specific edges and pale/dark bark fixtures.
- **R6 — map collisions are ungated:** Crop vs Shelter blue rings ΔE 36.33; orange vs amber planned-route line ΔE 48.96/52.42 at contrast 1.59/1.87; orange Fell vs red-dashed refused-rack line ΔE 30.39.
- **R8 — "orange-first" is nominal:** orange-vs-orange fails by construction, so the real decision is **pink rack, or change Fell**. Pink is a normal-vision-only gain.

## 5. Compact user / Manager decision table

Nothing below is recommended as an approved default; the "recommendation" column is the reviewers' advice, for the Manager and user to accept, modify or reject.

| ID | Decision | Options | Recommendation | Consequence if taken |
|---|---|---|---|---|
| **D-01** | Tester isolation model (#257) | Per OS account/device · shared-account profiles | Establish actual sharing first; require profiles if testers share an account | Determines whether a settings store can isolate testers at all |
| **D-02** | Settings storage (#257) | PlayerPrefs · separate versioned JSON | Separate JSON | No game-save schema change |
| **D-03** | Release scope (#257) | Sensitivity first · larger bundle | Sensitivity first, after the current playtest | Keeps the first change bounded |
| **D-04** | Reserved/recovery keys and conflict policy (#257) | Proposed fixed table · revised policy | Keep Esc/F1/F5/F9/F10 reserved; approve conflict policy explicitly | Defines recovery and migration behaviour |
| **D-05** | Relevance summary/export (#257) | Omit · defer · include | Defer until explicitly requested with approved consent copy | No collection authority implied |
| **D-06** | Gamepad/Map accessibility (#257) | V1 expansion · follow-up | Follow-up | Existing gaps stay explicit |
| **D-07** | Keyboard focus policy (#258) | Reverse `UiKit.Button` focusable policy · declare guidance mouse-only | Reverse only for the new guidance controls and the three rack prompts, with Tab policy settled | AC-4 becomes achievable; gameplay keys must not reach buttons |
| **D-08** | Rack-prompt emphasis (#258) | Restyle safe action as primary · keep destructive primary | Restyle | Consent UX change needs an owner; FR-2 depends on it |
| **D-09** | Consent ∥ Reset co-visibility (#258) | Fix in production · keep "no primary" | Keep "no primary" in spec; log the production fix separately | A reachable state stays guideless until fixed |
| **D-10** | Screen reader (#258) | Claim · defer · UNKNOWN | Defer and say so | Prevents an unsupportable accessibility claim |
| **D-11** | Diagram pass/fail (#258) | Add reference geometry + tolerances + rubric · judgement-only | Add them before approval | Otherwise a later render cannot be reviewed |
| **D-12** | 16 px target scope (#258) | All text · new elements only | New elements only, measured at panel scale | Avoids an immediate regression against today's baseline |
| **D-13** | HUD objective widget scope (#258) | In slice · out of slice | Out of scope for Objective 1 | Keeps the slice small |
| **D-14** | Rack colour (O1) (#259) | Pink rack · keep orange and change Fell | Treat pink as the real candidate and reword O1 accordingly | Orange-vs-orange fails the palette floor by construction |
| **D-15** | Background gate (#259) | 3:1 with edge expected · no edge | Accept 3:1 with a two-tone/fixture-specific edge and pale+dark bark fixtures | Without an edge, 3:1 is unreachable on mid-light bark |
| **D-16** | Recognition protocol (#259) | As drafted · revised protocol | Revise: per-mode captures, spaced sessions, fixed item counts, real CVD participant or "simulated only" | Draft protocol cannot distinguish perception from memory |
| **D-17** | Migration inventory (#259) | Prose audit · line-cited table | Line-cited table, shipped atomically with the visuals | ~20 strings currently untracked |
| **D-18** | Map symbol policy (O3) (#259) | Leave collisions · add adjacency checks | Add numeric colours and mark-vs-adjacent-symbol checks | Three collisions are currently ungated |
| **D-19** | Wording standard (#259) | "Sourced established practice" · "publisher-documented (Pro Silva Ireland)" | Reword, and disclose that the rack/Fell colour split is a game choice | One NGO's published system is not an industry standard |

## 6. What this review does NOT establish

- No Unity compile, import, render, text-fit, input-dispatch or screen-reader run.
- No human perceptual, colour-vision or recognition study; all CVD/grayscale figures are approximations.
- No proof that any proposed default is comfortable, discoverable or accessible.
- No approval of any spec, default, colour, shape, key binding, file format or privacy model.
- Method B provenance is still owed by its originator.
- The unified "accessibility" claim across all three specs remains **unproven**; each spec now carries its own evidence gap.

## 7. Next gates

1. **Authors revise their own DRAFTs** against §4 (each spec has one owner; DPC does not edit specs).
2. **Manager product decisions** on §5, in one batch, after which the specs can move to `APPROVED`.
3. **A future Unity qualification** (separate packet, fresh lock/disk/process preflight) before any of these specs produce visible behaviour.
4. **#269's staged UI branch** remains source-only and separately reviewed; it is not part of this review and is not authorised by it.

**Nothing in this document authorises implementation.**
