# S1-D T-04 — Sequencing Amendment (2026-10-29)

This amendment supplements `TASK-PACKET.md`; all technical scope and verification requirements remain unchanged.

## Updated start/sequencing rule

- #103 and #102 execution claims are now `[HANDED-OFF]` and their live ownership is released.
- Their later umbrella integration remains reported on local/unpushed `ac876a6...`; therefore T-04 still **must not start from that report alone**. Start only from an exact current integration candidate that the local worker can identify and that is published/GitHub-resolvable before Manager canonical integration claims are made.
- MFM-06 currently owns the active save/persistence lane. T-04 does not overlap that save work, but for a stable programme base it should begin only after MFM-06 integration/publication or an explicit #91 release stating the teaching files can safely branch from the same exact candidate.
- **T-04 must land before MFM-09**. It owns the guide-monotonicity change on `ScenarioOneTeachingSequence.Current` / adjacent current-state files; MFM-09 must consume the resulting API/state rather than concurrently rewriting it.
- At dispatch, use the current `Docs/Handoffs/PlayerTestReadiness-Overnight-01/CROSS-FEATURE-OWNERSHIP-AND-SEQUENCING.md` in addition to the original packet.

No Windows build.

## Updated copy/paste dispatch line

`Run S1-D T-04 using Docs/Handoffs/S1-D-Implementation-04/TASK-PACKET.md plus SEQUENCING-AMENDMENT-20261029.md and the current cross-feature sequencing disposition. Start only from the exact current integration candidate after #91 releases the teaching files; T-04 must land before MFM-09. Use Claude Sonnet 5.5 Medium and require OpenAI 6.1 Sol independent review. Preserve stable IDs, use persisted history only, add no schema/persistence, and publish the exact candidate/handoff. No Windows build.`
