# MFM-10 — Species-Parity Integration Guard

**Parent:** #91  
**Spec:** `specs/multiscale-forestry-management-v1/` MFM-10  
**Status:** PREPARED / BLOCKED on MFM-09 and PARITY-08  
**Purpose:** prove Multi-Scale Forestry Management V1 remains quantitatively Sitka-adult-only after parity/save work and that Oak/Beech cannot silently enter Sitka-specific prescription rules.

## Difficulty and routing

- **Difficulty:** Medium — focused routing/eligibility verification with high product-honesty importance.
- **Preferred worker:** Claude Sonnet 5.5 Medium / Anthropic.
- **Independent review:** OpenAI 6.1 Sol if a production fix is required; otherwise browser/Manager static conformance may review published evidence.

## Start gate / base

Start after MFM-09 is accepted/integrated and PARITY-08 has established current save/species compatibility. Record one exact published integration SHA and verify no parity/MFM species writer is active.

Suggested branch/worktree: `verify/mfm-v1-species-guard-v1` / `/media/jer/Videos/CCF-MFM-10`.

Create a claim only when dispatchable.

## Required scope

Verify:

- quantitative prescription/candidate/release treatment is restricted to eligible Sitka adult stems in V1;
- Oak/Beech cannot enter Sitka-only quantitative rules because of shared enum/default/fallback behaviour;
- unknown species remain governed by PARITY-08 pre-mutation rejection, not a Sitka fallback;
- broadleaf units/candidates are excluded with accurate player-readable reasons;
- species identifiers alone are not presented as an evidence-backed broadleaf prescription model;
- no Sitka density/release/calibration coefficients are generalized to Oak/Beech;
- manual management of Oak/Beech remains available through ordinary supported mechanics.

If multi-species prescription treatment is desired or required to pass, STOP for a spec amendment; do not implement it here.

## Exclusions / invariants

- no broadleaf prescription coefficients;
- no new species growth/density/economy mechanics;
- no MFM save-schema bump;
- no hidden quality ranking;
- no change to protected pure-Sitka anchors;
- preserve Legacy40/Reference Future;
- no Windows build.

## Verification

- focused source/offline eligibility matrix for Sitka/Oak/Beech plus malformed/unknown species path;
- current MFM sample/prescription fixture remains unchanged for Sitka;
- Oak/Beech units return explicit excluded/manual status, not proposals;
- UI/reason copy states the V1 scope honestly;
- applicable MFM determinism/non-mutation regressions pass;
- Unity compile/interaction only if production code/UI changed.

## Handoff

Publish `Docs/Handoffs/MFM-Implementation-10/HANDOFF.md` with exact SHA, eligibility matrix, changed files (if any), tests and exclusions.

Success token: `SPECIES GUARD QUALIFIED`

## Copy/paste dispatch line

`Run MFM-10 from Docs/Handoffs/MFM-Implementation-10/TASK-PACKET.md after MFM-09 and PARITY-08 are accepted on one exact published integration candidate. Use Claude Sonnet 5.5 Medium by default. Prove the quantitative MFM V1 path is Sitka-adult-only, Oak/Beech are explicitly excluded/manual rather than processed with Sitka rules, and unknown species never fall back to Sitka. Stop for a spec amendment if broadleaf prescription support is required. No Windows build.`

**User action required now:** none; blocked on MFM-09/PARITY-08.
