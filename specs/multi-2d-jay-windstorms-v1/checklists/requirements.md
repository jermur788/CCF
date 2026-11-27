# CCF Multi 2D — Jay Acorn Dispersal, Nearby Oak Woodland and Windstorms V1 — Requirements Checklist

Part 1 only. Test names are the exact `TestRunner` names (prefix `JW-Txx`).

| ID | Requirement | Verification |
|---|---|---|
| **JW-REQ-01** | Jay Acorn Dispersal and Nearby Oak Woodland are OFF by default; OFF consumes zero RNG draws and returns empty results. | `JW-T01`, `JW-T02` |
| **JW-REQ-02** | Jay runs only in a caller-supplied existing oak mast year; non-mast years give zero input and zero draws. No second mast schedule. | `JW-T03` |
| **JW-REQ-03** | Pure Sitka with Nearby OFF produces zero jay input. | `JW-T04` |
| **JW-REQ-04** | Nearby ON: ≤ 4 external candidates on 0.64 ha, all attributed to NearbyOakWoodland and its edge, no fabricated source tree ID, source line just outside the chosen edge (all four edges). | `JW-T05` (×4), `JW-T14` |
| **JW-REQ-05** | On-map cap ≤ 6 on 0.64 ha; candidates diverted from the supplied list (never added on top), zero double counting, source tree ID retained, `min(cap, supplied)`; delivered + exported == supplied below cap. | `JW-T06`, `JW-T07`, `JW-T14` |
| **JW-REQ-06** | Transport `r = 5 + 195u²` (5–200 m), uniform bearing, dedicated keyed stream `(year, source, candidateIndex)`. | `JW-T09`, `JW-T60` |
| **JW-REQ-07** | Out-of-bounds acorns are exported seed: never redrawn, clamped or retried; stable count across runs. | `JW-T08` |
| **JW-REQ-08** | No microsite bias, no browse immunity, no instant seedlings; delivered candidates require ordinary evaluation by the caller. | `JW-T12` |
| **JW-REQ-09** | Deterministic nearest-integer caps (6.4 → 6, 3.84 → 4). | `JW-T11` |
| **JW-REQ-10** | StormModel 0 is storm-free with zero draws and shifts no other stream. | `JW-T20`, `JW-T21` |
| **JW-REQ-11** | StormModel 1 uses frozen D-050 verbatim (0.02; 0.02/0.06/0.18; 1:1:1; ReducedProposal; BoundedRational; neutral site/edge), bit-identical to the frozen Unity formula and event years. | `JW-T22`, `JW-T60`, `JW-T61`, `JW-T30` |
| **JW-REQ-12** | At most one woodland-wide event per year; never per lane; no invented lane/exposure/thinning multipliers; repeat resolution for a year refused. | `JW-T23`, `JW-T24`, `JW-T30` |
| **JW-REQ-13** | Direction `u · 360` (blow-towards, frozen D-050); `FallGlyphOrientationDegrees = DirectionDegrees` as a UI convention; no per-tree bearing persisted. | `JW-T25` |
| **JW-REQ-14** | Qualitative Stable/Watch/Exposed feedback; no hazard percentage or p exposed. | `JW-T26` |
| **JW-REQ-15** | Two-pass victims keyed `WINDTHROW-v1-<TreeId>`; each victim: cause windthrow, one fallen deadwood record, one canopy opening; no snags/snaps/dominoes. | `JW-T27` |
| **JW-REQ-16** | Determinism across repeated runs; shuffled input ordering yields the same victim set and jay output. | `JW-T28`, `JW-T29`, `JW-T10`, `JW-T08` |
| **JW-REQ-17** | No automatic salvage; Sell/Keep need `ApprovedWorkPlanId`. | `JW-T40`, `JW-T41`, `JW-T46` |
| **JW-REQ-18** | Revenue only for SellAndExtract at unchanged price/contractor minimum; KeepForUse retained timber, no revenue. | `JW-T42`, `JW-T43` |
| **JW-REQ-19** | No duplicate deadwood after extraction (deadwood-ID guard on orders and merges). | `JW-T44`, `JW-T45` |
| **JW-REQ-20** | Dated, non-causal teaching note: Darragh (Dec 2024), Éowyn (Jan 2025), 2026 windblow market. | `JW-T47` |
| **JW-REQ-21** | Stream IDs fixed, versioned and prefix-free against existing RngSalts families. | `JW-T62` |
| **JW-REQ-22** | Static hygiene: no file, network, clock, machine-state or culture-dependent API in `src/Core/NaturalProcesses/**`. | `JW-T63` |
| **JW-REQ-23** | Standalone test exe (single Compile glob, zero references); 0 warnings / 0 errors on SDK 8.0.133; exit code 0; fence diff within four prefixes. | `JW-T64`, restore/build/run, `git diff --name-only` |
