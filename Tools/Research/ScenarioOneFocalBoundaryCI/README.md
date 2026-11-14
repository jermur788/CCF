# Scenario One focal boundary CI (Tier 1, offline) — #484

Pure-Python (standard library only, tested on CPython 3.12.3) comparison of the **current 8 m Hegyi competition index (CI)** of a few identified focal Sitka trees in the original #460 scratch 20 × 20 m and 30 × 30 m forests, between

* the **actual finite forest** (what #460's Unity run generated), and
* **one explicitly hypothetical** continuously stocked surrounding forest added outside the property.

No Unity, no game, no saves/Prefs, no repository imports. The raw #460 evidence is read-only and is **not** copied here. Report: `Docs/Research/ScenarioOne-Compact-Ecology-20261114/FOCAL-BOUNDARY-CI-TIER1-RESULT-20261114.md`.

## Files

| File | Purpose |
|---|---|
| `focal_boundary_ci.py` | Data gate, source-equivalent Hegyi arithmetic (float64 and a float32 emulation), focal selection, hypothetical external lattice, outputs |
| `test_focal_boundary_ci.py` | 30 unit tests (hand-calculated Hegyi terms, 8 m cutoff, zero-external control, pair exclusion, DBH direction, boundary classification, 20 m vs 30 m identity, NaN/duplicate guards, determinism). One class runs against the real evidence only if the pinned root exists |
| `results/` | Derived outputs of the committed script on the verified evidence (focal rows only; no raw tree arrays) |

## Run

```bash
# from the repository root; -I isolates from the current directory, -B stops __pycache__ appearing in the worktree
python3 -I -B Tools/Research/ScenarioOneFocalBoundaryCI/focal_boundary_ci.py --out-dir <new-empty-or-scratch-dir>
python3 -I -B -m unittest discover -s Tools/Research/ScenarioOneFocalBoundaryCI -p 'test_*.py' -v
```

`--evidence-root` defaults to the pinned #460 root
`/media/jer/Files/CCF/Verification/CompactOneRackNativeFeasibility460-20261113T145748Z/native-20261113T152528Z`.
`--out-dir` is refused if it is inside the evidence root. Exit codes: `0` ok, `2` **BLOCKED / DATA GATE** (nothing is calculated), `1` refused/usage.

Outputs: `results.json` (everything), `focal-rows.csv` (one row per focal tree), `external-stems.csv` (every hypothetical stem that reaches a focal tree, with its distance and Hegyi term per focal), `run-meta.json` (Python version, script SHA-256; kept separate so the other three files are byte-identical between runs).

## What the data gate checks (all must pass, else exit 2)

Per-file SHA-256 against `evidence-manifest.json` and, for `native-result.json` and `executed-harness.cs`, against the hashes pinned in the published #460 report; the candidate file equals its copy inside `native-result.json`; candidate id/size; rows = recorded `initial` = Year-0 living; unique IDs; all `sitka-spruce`, all age 20; recorded `edge` = `size/2 − max(|x|,|z|)`; every stem inside the square; the executed harness (hash-verified) shows the tree list is living-only, is captured by `MeasureInitial` after `Generate` and before access felling, and takes `ci` from `eco.GetCompetitionIndex`; finite numeric fields; positive DBH; the recomputed finite CI reproduces the recorded CI (tolerance 1e-4, otherwise "stage mismatch"). It also snapshots size + mtime of every file in the evidence root, and the hashes of the five key files, before and after reading, and fails if they differ.

## Raw schema used (`C-A-20.json`, `C-B-30.json`, key `trees[]`)

`id, species, side (north|south|rack), age, x, z, dbh (cm), height, crown, ci, edge, ba, volume, saleVolume, fellable, crop` plus top-level `id, size, lattice, initial, states[0] (year 0), cropIds`. `x`/`z` are world metres with the property centred on the origin; positions and DBH are float32 values stored as JSON doubles. `side == "rack"` stems are those inside the #460 access corridor (felled at Year 1); they are living and counted as competitors at the Year-0 stage used here.

## Formula locked to source

`ForestEcologyController.cs` (blob `0290a7be…`, identical at #460's baseline `6c13a9e1` and at `main` `fc18190b`): living targets only, neighbours in ordinal-`TreeId` order, `distance = Vector2.Distance`, skip when `distance > 8`, `ci += (dbh_j / max(1, dbh_i)) / max(0.5, distance)`. The float32 path rounds after every operation. This is an **approximation check, not a Unity bit-equality claim**: no native comparison run was made.

## The hypothetical external context (the only assumption set)

Single primary configuration, all stems plainly invented and labelled `X_r±RR_c±CC` (never a source `Pxxxx` ID): the source planting lattice (same origin `−span/2`, 1.9 m spacing, same axis mapping) extended outward to every node outside the property square; no jitter; kept with probability `p = 2100 / (10000/1.9²) = 0.7581` by `murmur3_fmix32(FNV-1a(id))` (same finaliser the source uses for the Enlarged80 outer ring); Sitka, age 20; DBH = the candidate's own mean Year-0 DBH (15.5696 cm for 20 m, 15.4336 cm for 30 m). One sensitivity only (`s1_*` columns): every node kept with fractional weight `p` (realisation-free expected value). Contribution is linear in the assumed external DBH.

## Not claimed

Not a production-plot proof, not a real external-forest survey, not a growth/canopy/seed/wind effect, not a two-versus-three species test, no Year-25/40 run, no recommendation that a buffer is mandatory. An open-land boundary makes the finite result the correct one.
