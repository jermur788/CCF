# Species Wave 2 Moisture Table and Native Profile Cards Bounded Tasks

- **Spec ID:** SPEC-multi-2d-species-wave2-profiles-V1
- **Spec status:** IMPLEMENTING
- **Task list owner:** Overall Manager
- **Last updated:** 2026-11-26

## T-01 — Moisture vocabulary and table

- **Task ID:** T-01
- **Status:** DONE
- **Goal:** `SiteMoistureClass`, `EvidenceClass`, `SiteResponse`, `SpeciesMoistureTable` with all 6 species.
- **Scope:** `src/Core/SpeciesWave2/SpeciesWave2Types.cs`, `SpeciesMoistureTable.cs`.
- **Exclusions:** no wiring into the annual loop.
- **Owner / routing:** implementation sub-agent (Claude Opus 5.5).
- **Dependencies:** —
- **Verification:** W2-01, W2-02.
- **Stop conditions:** forbidden edit required.
- **Completion state:** tests pass.

## T-02 — Native profile cards

- **Task ID:** T-02
- **Status:** DONE
- **Goal:** `FutureSpeciesCatalog` with Scots pine, downy birch, common alder.
- **Scope:** `src/Core/SpeciesWave2/FutureSpeciesCatalog.cs`.
- **Exclusions:** no economy, save or UI use.
- **Owner / routing:** implementation sub-agent.
- **Dependencies:** T-01
- **Verification:** W2-03..W2-08.
- **Stop conditions:** Sitka constant reuse required.
- **Completion state:** tests pass.

## T-03 — Tests

- **Task ID:** T-03
- **Status:** DONE
- **Goal:** 8 author tests registered with `T.Add`.
- **Scope:** `tests/Core.Tests/SpeciesWave2/SpeciesWave2Tests.cs`.
- **Exclusions:** no edits to `Program.cs` or other tests.
- **Owner / routing:** implementation sub-agent.
- **Dependencies:** T-01, T-02
- **Verification:** Core suite ≥ 291 passed, 0 failed.
- **Stop conditions:** count drop.
- **Completion state:** 299/299 at author time.

## T-04 — Gate, commit, handoff

- **Task ID:** T-04
- **Status:** see handoff
- **Goal:** headless 2D gate G0–G7 PASS, 9/9 frozen anchors, commit and push, GitHub-tracked handoff.
- **Scope:** handoff folder.
- **Exclusions:** no merge, no PR, no browser tier.
- **Owner / routing:** implementation sub-agent.
- **Dependencies:** T-03
- **Verification:** gate summary in handoff evidence.
- **Stop conditions:** any G0–G7 failure.
- **Completion state:** pushed branch; independent review pending.

## Task summary

| Task | Status | Owner / routing | Depends on |
|---|---|---|---|
| T-01 | DONE | implementation sub-agent | — |
| T-02 | DONE | implementation sub-agent | T-01 |
| T-03 | DONE | implementation sub-agent | T-01, T-02 |
| T-04 | see handoff | implementation sub-agent | T-03 |
