# RACK-V2 OPEN-3 — Canonical Authority Sync

**Class:** bounded canonical documentation sync only  
**Overall Manager:** ChatGPT Game Dev  
**Implementation impact:** none  
**Unity:** prohibited / unnecessary

## Why

The user explicitly accepted the Rack V2 OPEN-3 recommendation on 2026-10-28, and that boundary has since been implemented and qualified through RACK-V2-05 and RACK-V2-07. Some canonical authority text still says OPEN-3 is unresolved. The next context snapshot must not freeze that contradiction.

## Accepted OPEN-3 rule

Record exactly this product boundary, preserving existing wording/style where possible:

- a rack with **zero executed removals** may be reopened/edited through the authoritative rack reconciliation/cancellation path;
- reopening itself cancels nothing;
- where editing would cancel approved-but-unexecuted work, the player receives a warning and explicit consent is required before cancellation/reconciliation;
- any rack with **one or more executed removals** is geometry-locked and represented as harvested;
- executed removals are never reversed;
- only racks whose geometry changes lose geometry-bound entrance-mark state; unrelated racks retain theirs.

This is the accepted OPEN-3 decision. Do not invent additional reopen policy.

## Required edits

Synchronise only stale OPEN-3 authority/status text in:

1. `Docs/Project/decision-log.md` — D-064 must no longer say OPEN-3 remains open. Record the accepted boundary above, with direct-user authority date 2026-10-28. Preserve D-064's other evidence labels and product rules.
2. `specs/scenario-one-objective-1-rack-planning-field-marking-v2/spec.md` — update provenance/§39 references, remove OPEN-3-as-unresolved stop language, and replace §39.7 OPEN-3 `OPEN` with the accepted rule. Do not rewrite Sections 1–38 beyond any minimal cross-reference required for consistency.
3. `specs/scenario-one-objective-1-rack-planning-field-marking-v2/tasks.md` — update the top decisions note and historical RACK-V2-05/RACK-V2-07 OPEN-3 gating/stop wording so it reflects that OPEN-3 is resolved; preserve task scope and exit tokens.

Search the approved Rack V2 spec directory and Decision Log for additional literal `OPEN-3` references. Change only those that incorrectly state unresolved/open status. Historical statements that are clearly dated as historical may remain if explicitly labelled superseded/resolved.

## Evidence / implementation references

- Direct user acceptance: 2026-10-28, `accept open 3 rec`.
- `Docs/Handoffs/RACK-V2-05/OPEN-3-MANAGER-DECISION.md` if present/canonical.
- `Docs/Handoffs/RACK-V2-05/MANAGER-REVIEW.md` — accepted editing boundary.
- `Docs/Handoffs/RACK-V2-07/MANAGER-REVIEW.md` — accepted confirm/reopen/consent behavior and G3 review.
- RACK-V2-07 integrated through PR #48 / merge `c5a0532ac0d6a3729baeca0a1bf6d1d85df46fb9`.

## Restrictions

- No production code.
- No Unity.
- No save/schema changes.
- No new product behavior.
- No generated context/mirror/dashboard edits.
- Do not broaden D-064 or reword unrelated decisions/spec sections.

## Verification

- Diff contains only the three canonical authority files above unless another stale OPEN-3 literal in the same approved Rack V2 spec directory requires a minimal consistency edit.
- Search confirms no current-authority statement still says OPEN-3 is open/unresolved.
- Accepted boundary remains identical to the direct user decision and integrated RACK-V2-05/07 behavior.
- Publish a handoff with exact changed files and before/after OPEN-3 references.

## Handoff

`Docs/Handoffs/RACK-V2-OPEN3-Authority-Sync/HANDOFF.md`

Success exit:

`OPEN-3 AUTHORITY SYNCED — READY FOR MANAGER REVIEW`
