# Proposal — browse history and form damage (save v16)

**Status: proposal only.** Nothing here is implemented, and it is not part of Scenario 1. It is sequenced **after** OpenCode's v15, which persists fences and shelters (`BrowseProtectedArea` / `BrowseShelter`).

Sources: Browsing & Protection v1 report §13 ("persistent memory") and §20 ("timber quality"); `Docs/BrowsingProtectionV1.md`.

## Why it needs its own schema step

The browsing biology already persists through height, density and alive/promoted state. Two things cannot be derived today:

- **Repeated-browse history.** It drives the report's form-damage risk, and it is what the player wants to know: "has this tree been browsed again and again?"
- **Form damage.** A fork or multiple leader persists for decades after the juvenile stage. It should influence crop-tree choice and, later, timber quality.

Today the per-step diagnostics (`lastYearBrowsed`, `LastBrowsedFraction`) reset on load. Adding fields under an existing version would let an older build silently drop them, which is why this is a version bump.

## Proposed fields (defaults correct for older saves)

| Record | Field | Type / default | Meaning |
|---|---|---|---|
| Planted juvenile | `yearsBrowsed` | int / 0 | Annual steps with a browse event |
| | `consecutiveBrowseYears` | int / 0 | Current run of consecutive browsed steps |
| | `lastBrowseYear` | int / −1 | Last step with a browse event |
| | `formDamage` | int / 0 | 0 none, 1 recoverable, 2 persistent fork/multi-leader |
| Regeneration cohort | `browsedYearsWeighted` | float / 0 | Σ browsed fraction over steps (expected-value history) |
| | `formDamageFraction` | float / 0 | Expected fraction with persistent form damage |
| Tree | `formDamage` | int / 0 | Inherited at promotion (individual: state; cohort: ≥0.5 of `formDamageFraction` → 2) |

No deer entities, bite counts or branch-level damage are saved (report §13).

## Candidate rule

These would be [C] calibration values and would need a calibration gate before production.

- Each browse event sets `consecutiveBrowseYears += 1`; an unbrowsed step resets it to 0.
- **Persistent damage.** When `consecutiveBrowseYears` reaches N (report: "test 2–4 events"; candidate N = 3), mark persistent form damage (2) with probability `q_species`:
  - Sitka: high, because repeated leader loss gives multiple leaders and stems [E transferred];
  - oak and beech: moderate.
  - Use a deterministic `Roll(id + "/form", year, seed)`.
- **Recoverable damage.** A single event sets recoverable damage (1). It clears after K unbrowsed steps (candidate K = 3).
- **Cohorts.** Apply the expected value through `formDamageFraction`.
- **No growth effect.** Form damage does not change growth in this proposal. Its consumer is crop-tree selection: a tree with `formDamage == 2` is "not preferred as a crop tree" in the inspection card and Work Plan. Timber-grade and price effects stay deferred (report §20; OpenCode's pruning/timber-quality diagnostic owns that interface).

## Verification needed

- v15 → v16 load: all fields default to 0 / −1, with identical biology.
- Determinism and save/load continuity with history carried.
- Shared natural/planted rule (equal expected values).
- Calibration matrix for N, q and K.
- The neutral and normal lifecycle hashes must be unchanged, because form damage has no growth effect.
