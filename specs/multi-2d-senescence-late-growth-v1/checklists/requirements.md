# Natural Old-Age Tree Death and Smooth Late-Life Growth V1 — Requirements Checklist

- **Spec ID** `SPEC-multi-2d-senescence-late-growth-v1`
- **Spec status** APPROVED / IMPLEMENTING (Part 1); Part 2 DEFERRED
- **Checklist owner** Overall Manager
- **Implementation under test** `feat/multi-2d-senescence-engine-v1`, Part 1 only
- **Date** 2026-11-28

## Part 1 locked decisions and worker evidence

- [x] **FR-1 / AC-2 — Six-species IDs reused:** `SpeciesMoistureTable` constants used directly; no second species vocabulary (`HAZ-01`).
- [x] **FR-1 / AC-2 — Smoothstep hazard & zero onset:** exact zero through onset, `h(onset + 1) <= 0.002`, non-decreasing finite through age 300, max delta `<= 0.002`, declared ceilings `<= 0.05` (`HAZ-02`..`HAZ-05`).
- [x] **FR-1 / AC-2 — Ordinal longevity grouping & pioneer persistence:** `min(alder, birch) > max(beech, sitka) > max(pine, oak)` at 120/150/200/250; strict onset and pair-ceiling ordering; pioneers never killed automatically at 60–70 and 70–150 survival in `(0.05, 0.60)` (`HAZ-06`..`HAZ-08`).
- [x] **FR-1 / AC-2 — Hazard purity & alder heart-rot separation:** hazard has no crowding/growth inputs; Common alder heart rot (`BirchAlderTimber.cs`) remains strictly timber downgrade, never annual mortality (`HAZ-09`, `HAZ-10`).
- [x] **FR-2 / AC-4 — Wave 2 exact in-window delegation:** `scots-pine` (age 42), `common-alder` (age 33), `downy-birch` (age 44) match `ScotsPineEngine` / `BirchAlderEngine` bit-exactly, including the exact cutoff seam under crowding and moisture (`GROW-01`..`GROW-04`, `GROW-11`).
- [x] **FR-2 / AC-4 — Smooth C0/C1 post-cutoff growth continuation:** bit-exact delegated cutoff anchor `A` (`ApproxEq` within `1e-12` of `0.20`, `0.20`, `0.25`, documenting alder's 2-ULP IEEE-754 value), `C1` slope continuity, no cliff (first post-cutoff year falls by less than the window's final year), strictly positive monotone decreasing through age 300, preserved crowding and single moisture-multiplier application (`NeutralLegacy == 1.0`), `null` outside domain (`GROW-05`..`GROW-10`, `GROW-12`); refuses Sitka/Oak/Beech and reads cutoffs from the existing Wave 2 domains (`GROW-13`).
- [x] **FR-3 / AC-1 — Version gate:** `SenescenceModelVersion` defaults to `Disabled` (0); disabled produces zero deaths, `RngDrawCount == 0`, no species/age evaluation; enabled draws one keyed roll per tree (`GATE-01`..`GATE-06`).
- [x] **FR-3 / AC-3 — Deterministic keyed rolls & guards:** stateless FNV-1a + SplitMix64 keyed by `(species, treeId, year)` under versioned module prefix; repeatable, order-independent, key/year/seed sensitive; defensive snapshots; rejects duplicate IDs, negative ages, `Year < 1`, enabled unknown species; residual cohort deaths (`ROLL-01`..`ROLL-05`, `ROLL-08`..`ROLL-10`, `DET-02`).
- [x] **FR-4 & FR-5 / AC-3 — Biological-death semantics & plain wording:** `Cause == "old-age"`, `BiologicalDeath`, `TimberRevenueEur == 0m`, `ClearsCropTreeMarks`, `CountsAsFell == false`; player text exact `"Died of old age"` without evidence tags or disclaimers (`ROLL-06`, `ROLL-07`).
- [x] **FR-6 / AC-5 & AC-6 — Offline console gate, pinned digest & write fence:** BCL-only .NET 8, pinned SDK 8.0.133, 0 warnings / 0 errors, cross-process byte-identical output and pinned SHA-256 digest (`DET-01`), module sources free of the existing JW-T63 banned tokens and G6 determinism tokens (`HYG-01`), zero edits outside the four allowed prefixes.
- [x] **D-059 / Track A provenance:** evidence classification table completed with `[T]`/`[I]`/`[C]` labels, explicit "no fitted Irish mortality curve" disclosure, and zero fabricated citations (`spec.md`).

## Deferred / remaining acceptance (Part 2 and independent review)

- [ ] **Independent review (Part 1):** Fresh reviewer inspects Part 1 spec conformance, calibration disclosures, arithmetic, determinism digest, and write fence on `feat/multi-2d-senescence-engine-v1`.
- [ ] **Part 2 (DEFERRED — AC-7):** Wire senescence into `AnnualPipeline.cs`.
- [ ] **Part 2 (DEFERRED — AC-7):** Register senescence stream salt(s) in `RngSalts.cs`.
- [ ] **Part 2 (DEFERRED — AC-7):** Wire live Wave 2 late-life growth continuation for Scots pine, Common alder, and Downy birch.
- [ ] **Part 2 (DEFERRED — AC-7):** Implement and validate Version 3 six-species deadwood save compatibility while keeping Version 2 Phase1 and all nine frozen baselines untouched.
- [ ] **Part 2 (DEFERRED — AC-7):** Surface `"Died of old age"` and resulting deadwood in History / Habitat UI.

## Verdict

- **Spec conformance (Part 1):** NOT VERIFIED (worker self-check only; pending independent review)
- **Implementation correctness (Part 1):** NOT VERIFIED (worker self-check only; pending independent review)
