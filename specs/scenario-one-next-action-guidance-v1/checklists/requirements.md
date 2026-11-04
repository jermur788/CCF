# Requirements / acceptance checklist

- **Spec ID:** SPEC-SCENARIO-ONE-NEXT-ACTION-GUIDANCE-V1
- **Spec status:** DRAFT
- **Owner / date:** Overall Manager / 2026-11-04
- **Implementation under test:** none — docs-only draft

Unchecked means NOT VERIFIED; documentation inspection is not implementation evidence.

- [ ] FR-1 / AC-1: one enabled state-derived cue; counters/instruction agree; no second progression source.
- [ ] FR-2 / AC-2: explicit map → plan → confirm → forest entrance marks → Work Plan flow; no automatic operations.
- [ ] FR-3: helper instruction wins; corrections follow the first-eligible allowlist; ambiguous reasons reveal that same instruction.
- [ ] FR-2: ConsentPending, ResetPending, reopenPending and other blocking modals suppress every background cue; safe choice or no-primary exception; re-derive after dismissal.
- [ ] F6: implementation-base symbol citations rechecked by function/entry name; draft cites main 8f0f230, not candidate.
- [ ] FR-4: detailed Objective 1 and correct plan/approve/execute distinctions.
- [ ] FR-5 / AC-3: five-rack map-like diagram, verified headland/spacing/retained rows, real symbol shapes, no false cleared routes.
- [ ] FR-6 / AC-4: non-colour cues, text equivalent, visible focus and real keyboard reachability; Tab policy resolved.
- [ ] AC-1–6: exact-HEAD evidence at 1280×720 and 1920×1080, with role/resolution recorded per spec matrix.
- [ ] AC-5: save/load boundaries, old V2/Legacy40, stable practice IDs and frozen reference unchanged; no schema migration.
- [ ] AC-6: later lesson mapping approved and checked without speculative actions.
- [ ] Q1–Q4 resolved, independent review obtained, Manager approval recorded before implementation.
- [ ] No backend/packages/new guidance engine/auto-navigation or unapproved world write introduced.
- [ ] Approved deviations and failures recorded; no invented ecological claims or runtime evidence.

## Verdict

- **Spec conformance:** NOT VERIFIED — no implementation
- **Implementation correctness:** NOT VERIFIED — no implementation
- **Security/write-back:** future existing-operation consent/validation checks pending; draft writes documents only
- **Save compatibility:** proposed no schema impact; future regression evidence pending

## Review response

- **P1 FIXED:** FR-2 now resolves `editor.ConsentPending`, `editor.ResetPending`, `reopenPending`, then other blocking modals before every background row; only the active scope has primary emphasis/activation. Safe decline/cancel wins, with no-primary fallback when no safe modal choice exists. AC-1 tests each prompt against competing planning/confirmed states and re-derivation after dismissal.
- **P2 FIXED:** FR-3 makes candidate `RackV2Instruction()` authoritative for Objective 1 wording/stage after modal handling, projects its first unfinished step onto the screen, then applies validators. Corrections use a fixed first-eligible allowlist (map, planning, reason-matched undo, over-target selected unlocked delete); otherwise reveal the same instruction/reason. No arbitrary enabled-control selection or second instruction source. AC-6 tests helper branches, ties and ambiguous reasons.
- **P3 FIXED:** F6 was re-derived with `git show` and numbered lines at main `8f0f230190830855112e3e7a1d7544db94a7f591`; every symbol citation now names `StandMapSymbols.Build()` and its `MapSymbol` entry. Racks :86–97, navigation/grid :99–104, tree marks :106–107; not candidate 72b4efbd.

DRAFT remains unchanged. Planned canopy/no-clearing semantics, shape/pattern/text accessibility and saved-state derivation without schema changes remain required. Docs-only; no Unity or production verification claimed.
