# TASK PACKET — PARITY-05 Early-Life / Browse Conformance Closure

**Feature:** Scenario One Three-Species Parity P0  
**Spec:** `specs/scenario-one-three-species-parity-p0/` — APPROVED  
**Task:** `PARITY-05-PATCH`  
**Class:** bounded documentation + verification closure; no production-behaviour change  
**Status:** READY / DISPATCHED  
**Manager:** ChatGPT Game Dev / Overall Manager  
**Task claim:** #32

## Purpose

Close the remaining PARITY-05 evidence-label and verification gaps **without changing current juvenile, browsing, protection, save, or species behaviour**.

The PARITY-05 preflight is accepted by:
`Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-05-Preflight/MANAGER-REVIEW.md`.

## Binding Manager decisions

### M1 — early-life representation

Do **not** implement the packet-only simple numeric table (`0.014/0.62/0.93`, `0.0035/0.28/0.88`, `0.0025/0.24/0.86`). It is not committed PARITY-02 authority and has no defensible mapping into the current architecture.

Preserve exactly:

- Oak distinct juvenile establishment/survival curves;
- Beech combined juvenile response, labelled `FALLBACK-CONSERVATIVE`;
- Sitka protected lifecycle behaviour;
- shared juvenile growth/promotion/density calibration;
- existing dispersal/maturity/mast/light rules;
- no new annual establishment/survival scalar.

### M2 — browsing

Preserve current authoritative `browsePalatability` values:

- Sitka `0.15`;
- Beech `0.5`;
- Oak `1.0`.

Preserve ordering `Oak > Beech > Sitka`. Do not implement, normalise, or multiply the packet-only `1.0 / 1.8 / 2.2` values.

### M3 — protection

Preserve Browsing & Protection v1 binary access:

- intact/effective protection = access `0`;
- breached/outside = access `1`;
- protected outcomes remain browse-free-baseline equivalent.

Scenario One `0.2` is background browse pressure, **not** a protected multiplier.

## Routing

Target: **Claude subscription 2**, strongest available Claude reasoning/coding model.

Recommended branch:
`feature/scenario-one-three-species-parity-earlylife-conformance-v1`

Use a separate clean non-Unity worktree/lane. Before substantive edits, refresh live coordination and mark claim #32 `[RUNNING]` with actual model/provider, branch/worktree and starting HEAD.

**Do not acquire the machine-wide Unity lock in this task.**

## Allowed files

Documentation/evidence labels, limited to the minimum needed:

- `Docs/BeechSpeciesParameters.md`
- `Assets/ForestPrototype/Docs/SessileOakV1.md`
- `Docs/BrowsingProtectionV1.md`
- `Docs/Project/unity-project-overview.md` only for factual current-state wording and only if no live ownership conflict exists

Focused verification, limited to current-behaviour assertions:

- `Tools/Verification/JuvenileMortalityFoundationVerification.cs`
- `Tools/Verification/BrowsingProtectionVerification.cs`
- `Tools/Verification/RegenerationBudget/RegenerationModelVerification.cs`
- optionally one new focused file under `Tools/Verification/ThreeSpecies/` if that is cleaner than broadening existing harnesses

No other file without Manager return.

## Required closure

1. **Beech label:** current combined juvenile response is explicitly described as `FALLBACK-CONSERVATIVE`; do not invent a Beech-specific curve.
2. **Broadleaf stock label:** stale Beech-only planting-stock wording is corrected to the current shared broadleaf stock abstraction where documentation exposes it. If correcting the actual production C# header/comment would require touching a production file, STOP and report it rather than editing production in this packet.
3. **Browse truth:** docs and verification state/assert current values `Sitka 0.15 / Beech 0.5 / Oak 1.0`, qualitative ordering `Oak > Beech > Sitka`, and clearly distinguish Scenario One background pressure `0.2` from protection.
4. **Protection truth:** binary protection remains exact and protected results remain browse-free-baseline equivalent.
5. **Shared juvenile calibration:** verification asserts the accepted current shared juvenile growth/promotion/density parameters and Oak-distinct/Beech-fallback selection without changing values.
6. **Sitka preservation:** existing protected Sitka lifecycle/browse anchors must remain unchanged; no expected value or fingerprint may be rebaselined.
7. **Unknown species:** record only as PARITY-08 follow-up; do not repair it here.

## Forbidden

Do not edit:

- `Assets/ForestPrototype/Species/*.asset` or any species YAML;
- production-behaviour C#;
- scenes/prefabs/materials/settings;
- save/schema/versioning;
- juvenile/browse/protection numeric values;
- response curves;
- adult growth/density/site/canopy;
- timber/economy/pruning;
- Rack/S1-C/S1-D/MFM mechanics;
- protected expected values/fingerprints.

No Unity, Player build, or Windows build.

## Non-Unity verification

At the exact patch HEAD:

- run the directly affected offline/static verification suites available in-repo;
- prove zero species-asset/scene/prefab change;
- prove zero production-behaviour C# change;
- prove no protected expected value/fingerprint change;
- show that any verification edits strengthen/report current accepted behaviour rather than weakening gates;
- report any tool/harness limitation honestly.

## Handoff

Publish:
`Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-05/HANDOFF.md`

Include:

- exact base and patch HEAD;
- actual executor/model/provider and worktree;
- changed-file list;
- evidence-label changes;
- verification assertions added/strengthened;
- commands/results;
- preservation proof;
- confirmation that no production behaviour/species asset/save/schema change occurred;
- any proposed Unity-only follow-up scope.

At publication, mark claim #32 `[HANDED-OFF]`, link the handoff, and close it.

## Exit

`PARITY-05 CONFORMANCE PATCH IMPLEMENTED — READY FOR UNITY QUALIFICATION`

or a precise blocker.
