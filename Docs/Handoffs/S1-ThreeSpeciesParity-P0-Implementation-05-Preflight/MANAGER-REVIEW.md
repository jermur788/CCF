# PARITY-05 Preflight — Manager Review

**Task:** `PARITY-05-PREFLIGHT`  
**Claim:** #31  
**Published handoff:** `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-05-Preflight/HANDOFF.md` at `2b93720`  
**Manager verdict:** **APPROVED — MAPPING AUDIT ACCEPTED; BOUNDED NO-BEHAVIOUR-CHANGE PATCH AUTHORISED**

## Decisions

### M1 — early-life numeric table

The simple numeric table introduced by the PARITY-05 dispatch packet (`0.014/0.62/0.93`, `0.0035/0.28/0.88`, `0.0025/0.24/0.86`) is **not accepted implementation authority**. It is not present in the committed PARITY-02 handoff, approved spec, decision log, or current production model, and it cannot be mapped without inventing a new parameter design.

The committed PARITY-02 Q-07/current production policy remains authoritative:

- Oak keeps its distinct juvenile establishment/survival curves;
- Beech keeps the combined response and must be labelled `FALLBACK-CONSERVATIVE`;
- Sitka remains exact under the protected lifecycle contract;
- shared juvenile growth/promotion/density calibration remains unchanged;
- existing species dispersal/maturity/mast/light rules remain unchanged;
- no new annual establishment/survival scalar is introduced.

The packet-only simple values are superseded as an implementation target and may be retained only as unsupported historical dispatch text, not as game calibration.

### M2 — browse susceptibility

The committed/current browse values remain authoritative:

- Sitka `browsePalatability = 0.15`;
- Beech `0.5`;
- Oak `1.0`.

This preserves the accepted qualitative ordering `Oak > Beech > Sitka` and the existing protected Sitka browse anchors. The packet-only `1.0 / 1.8 / 2.2` multipliers are **not adopted** and must not be normalised, multiplied onto current palatability, or otherwise implemented.

### M3 — protection factor

Keep the accepted Browsing & Protection v1 binary-access policy:

- intact/effective protection gives access `0`;
- breached/outside gives access `1`;
- fenced outcomes remain browse-free-baseline equivalent.

The `0.2` value is Scenario One **background browse pressure**, not a protected-browse multiplier. No partial-protection mechanic or Browsing v2 change is authorised in PARITY-05.

## Authorised production scope

PARITY-05 may now proceed only as a **no-behaviour-change documentation and verification closure**.

Allowed:

- attach the `FALLBACK-CONSERVATIVE` label to Beech's combined juvenile response on current documentation surfaces;
- relabel the shared planting stock as a broadleaf stock abstraction rather than Beech-only where stale wording remains;
- state the current `0.15 / 0.5 / 1.0` browse values and binary protection policy accurately;
- strengthen focused verification so current juvenile-response selection, shared calibration, browse ordering/current values, binary protection, and protected Sitka anchors are asserted without rebaselining.

Forbidden:

- species YAML/asset edits;
- production C# behaviour edits;
- scene/prefab/material edits;
- save/schema changes;
- new early-life scalar parameters;
- browse retuning or new multipliers;
- protection-mechanic changes;
- protected-anchor rebaseline;
- adult/density/timber/economy/pruning/Rack/MFM changes;
- Windows build.

## Adjacent issue

The preflight's unknown-species load nonconformance belongs to PARITY-08 and is not to be fixed inside PARITY-05.

## Continuation

Dispatch `PARITY-05-PATCH` under claim #32. The patch should end with non-Unity evidence and a published handoff. A separate focused Unity qualification may be queued afterward when the machine-wide Unity lane is free.

**Verdict:** `APPROVED — PARITY-05 CONFORMANCE PATCH AUTHORISED`

**DASHBOARD SYNC: NOT AVAILABLE**  
**Published dashboard reflects this task: NO**
