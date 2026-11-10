# Inspection HUD Declutter V1 — Plan

**Status: DRAFT; non-executable.** The spec must be explicitly approved before production edits.

## Approach

1. **Source inventory and pixel layout contract:** identify the real inspection/competitor marker drawing owner, all code output paths, current action bars, and existing compact USS constraints. Compare against the current #371 candidate, not old mock-up states.
2. **Pure presentation/data mapping:** map existing competition, share, diagnosis, storm and plan-effect data into one view model without changing ecology or marking calculations; preserve MFM review/proposal status separately.
3. **Card + bottom action bar:** implement a single contextual default card and a genuinely stage-aware bottom keycap/action bar; preserve full information through [V] without overflowing the locked-cursor card.
4. **Optional share overlay exploration:** bounded prototype only after marker ownership/source check. Keep card-based fallback; decide via real Unity captures.
5. **Focused verification:** source/keyboard tests, 720p and 1080p colour/grayscale, old-world regressions, warnings, MFM and Rack-first contexts; record a separate Tier 2 handoff.
6. **Manager review/integration:** inspect evidence and issue verdict; only then integrate when ownership/source lineage permits. Final user visual acceptance is distinct.

## Risks / stop rules

- If a global menu-shell or unrelated game flow is required, stop and separate scope.
- If values are only supplied as combined strings (e.g. treatment summary), ask Manager for an explicitly bounded presentation-facing data adapter, rather than parsing unstable prose.
- If a 720p card becomes unreadable, use responsive sections or [V] pages; do not remove facts.
- If UI source overlaps an active writer, stop. One primary writer per file set.
- Do not run Unity against user-owned sessions or without the existing safety/preflight/lease policy; no Windows build.

## Cost-conscious verification

Tier 1 during iteration. One Tier 2 at feature handoff. Do not rerun whole programme, actual management cycles, ecology calibration or broad old test suites solely for a cosmetic refactor; retain the narrowly relevant protected regressions.
