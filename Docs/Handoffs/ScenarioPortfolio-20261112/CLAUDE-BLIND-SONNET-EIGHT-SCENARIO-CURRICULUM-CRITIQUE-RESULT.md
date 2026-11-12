# Claude blind (Sonnet) eight-scenario curriculum critique — RESULT (in progress)

> First commit contains section 1 only, written before any researcher/manager material was opened. Later commits add the rest.

## 1. Blind provisional judgement (written before opening any researcher/manager material)

**Written:** 2026-11-12, after reading only accepted canon on `main` (Decision Log incl. D-051–054, D-059/060, D-064/065, D-071, D-078; Game Brief; Current Milestone; Unity Project Overview save/species facts; `Docs/ScenarioTwoSpeciesFoundation.md`). The "original eight-case report" could not be located on `main` or in the project files, so the O1–O8 labels and one-line descriptors in my task brief were used as the case definitions.

**Provisional order (from first principles):** O1, O2, O3, O4, O7, O6, O5, O8.

Reasoning in brief:
- **O2 second** because it is the only scenario that needs *zero* new biology: Sitka, Oak and Beech are already parity-accepted (D-059), planting, browsing, deadwood and the economy exist. That isolates the one genuinely new thing a second scenario must prove, independent scenarios (a scenario loader, a per-scenario save identity), from any new-species or new-mechanic risk. Today there is one `forest-save.json` and a scenarioId mismatch only warns.
- **O3 third** because it reuses Oak/Beech form, Crop Trees, pruning and deadwood on a new authored start state; ash dieback and hazel coppice are the only new mechanics and both can be kept out of the minimum.
- **O4 fourth**: first scenario with a bare-land start, site suitability and establishment decisions. Pioneer/nurse species (Scots pine, downy birch, common alder) are on HOLD, so an honest establishment scenario with only Sitka/Oak/Beech is thin on the nurse question.
- **O7 fifth**: StormModel 1 exists but is dormant; D-071 parked complex access/extraction for "future independent scenarios", so O7 is its natural home. Needs activation qualification and standalone-Player profiling first.
- **O6 before O5**: O6 builds on existing browsing machinery and adds invasives/continuity; O5 needs hydrology, waterlogging and riparian species (alder/birch partial evidence) and is the heaviest new subsystem.
- **O8 last** as a finite capstone: needs compartments inside one scenario, the only place a shared ledger is allowed (D-078).

**Key questions I wanted answered by the other material:**
1. Is a scenario loader + per-scenario save identity really the smallest honest "second scenario" proof, and who owns it?
2. Can O4's nurse/pioneer teaching be honest with only Sitka/Oak/Beech sim-ready, and what Irish (not transferred) evidence supports simultaneous year-zero nurse + target planting?
3. Can O3 ship without ash or hazel (no ash species asset exists; no coppice regrowth mechanic), and what is the minimum honest ash-dieback representation?
4. Can StormModel 1 be activated per scenario without touching Scenario One's protected anchors, and does O7 force the deferred access/extraction work?
5. What is the smallest finite O8 architecture (compartment count, shared-resource boundary) that is not a generic property sim?

**Exposure before this section was written (disclosed in section 2):** while loading project memory I saw one line recording the earlier #432 result (its order and verdict), and I read the body of issue #425, which states a five-scenario lean recommendation by another reviewer. Neither was used as evidence, but the blind condition is therefore **partial, not clean**.
