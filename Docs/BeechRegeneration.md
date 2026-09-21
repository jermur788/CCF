# Beech regeneration v1

Beech regeneration is enabled without changing its ecological calibration.
Generic species cohorts retain separate seed rain, density, height and establishment
history; normalized shared occupancy is capped at one.

## Save continuity

Saves use authoritative simulation dimensions rather than stage display height.
For example, a stump displays at 0.35 m but retains its simulation height internally.
The save schema remains v8: field names and meanings are unchanged.

Seed rain is derived and excluded from the persisted-state comparison. Annual seed
rain is computed before cohort promotion, whereas load reconstructs it from the
restored trees. Those observation points need not have identical seed rain.

`Tools/Verification/CCFBeechVerification.cs` is a disposable Unity runner: copy it
alone into Assets, run `CCFBeechVerification.Begin` in Editor batchmode, then remove
the Assets copy and generated metadata. It preserves the user's save and restores
the species flag after testing. It compares canonical sorted v8 save records,
checks repeated seed-rain recomputation, compares the next annual step against an
uninterrupted run, and repeats the 100-year mixed simulation from its initial save.

Verified mixed results: 373 Sitka and 8 Beech after 100 years; shared cohorts occur
during the run; occupancy never exceeds one (floating-point tolerance 1e-6);
recruit IDs are unique; persisted save/load, next-year state and seed rain, and A/B
determinism pass. Tests use Unity 6000.6.0f1 Editor batchmode.

The Sitka regression uses the canonical legacy-compatible projection in the
existing integration runner. A hash over the expanded generic representation is
not comparable with the established `7E39B70A14959FAD` baseline.
