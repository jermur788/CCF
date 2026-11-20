# Multi #885 — owner G5-MA1 APPROVED direction, finalised MA-1-only spec and bounded implementation-admission packet

**DIRECT GITHUB HANDOFF:** https://github.com/jermur788/CCF/blob/docs/multi-885-ma1-approved-spec-and-packet-20261120-v1/Docs/Handoffs/MultiRack-Simulation-20261116/885-multi-area-spec/Multi-885-MA1-APPROVED-SPEC-AND-BOUNDED-IMPLEMENTATION-PACKET-20261120.md

**Authority:** Owner explicitly approved **limited G5-MA1 design** at 2026-11-20 21:04:04Z; ordered "Finalise only the MA-1 spec and prepare the bounded implementation packet; **do not start source implementation yet**." CCF standalone 2D Manager #885 has finalised the four-file MA-1 scope and requirements; approval is **product/spec scope**, not execution, physical lease, source-grant, G4 integration, numeric forestry coefficient, owner G2 or source acceptance. **Status: SPEC FINALISED / IMPLEMENTATION PACKET READY BUT NOT DISPATCHED / NOT RUNNING.**

**Git baseline:** `main@c94f2a2b0dee439cbe2a1269fc47d7421a3e1be2` observed at Manager start (not the approved source implementation base); docs-only branch `docs/multi-885-ma1-approved-spec-and-packet-20261120-v1`. No production edits and no merge.

## Approved final MA-1 four-file package

- [spec.md](https://github.com/jermur788/CCF/blob/docs/multi-885-ma1-approved-spec-and-packet-20261120-v1/specs/multi-2d-ma1-manual-multi-area-workplan-v1/spec.md)
- [plan.md](https://github.com/jermur788/CCF/blob/docs/multi-885-ma1-approved-spec-and-packet-20261120-v1/specs/multi-2d-ma1-manual-multi-area-workplan-v1/plan.md)
- [tasks.md](https://github.com/jermur788/CCF/blob/docs/multi-885-ma1-approved-spec-and-packet-20261120-v1/specs/multi-2d-ma1-manual-multi-area-workplan-v1/tasks.md)
- [checklists/requirements.md](https://github.com/jermur788/CCF/blob/docs/multi-885-ma1-approved-spec-and-packet-20261120-v1/specs/multi-2d-ma1-manual-multi-area-workplan-v1/checklists/requirements.md)

**Four binding MA-1 decisions:** (1) two truly separate synthetic, clearly disclosed Sitka forest areas with unique tree IDs/current populations/area denominators; (2) isolated within-area annual ecology with no cross-boundary competition, disclosed; (3) one immutable global root/lock/publication and atomic in-process manual Work Plan, with optional no-Fell, zero Fell on approval, real per-area Fell on separate Execute and per-area Year+1; (4) never Fell the last living stem of *any* area, while naturally empty areas remain empty and the remaining populated areas advance. All-area empty experiment refuses further ecological advance; pending/approved unexecuted work requires explicit whole-experiment cancellation. No crash-atomic save promise.

**First genuine player demonstration:** player opens A, marks retained Crop and a neighbour to Fell; switches to B's different tree population and makes independent manual Crop/Fell choice; one combined Work Plan Approve (both remain standing) → one confirmed Execute (actual intended trees in BOTH areas change, one event/receipt) → Year+1 (genuine individual annual ecology and per-area summaries). A one-area-only/zero-Fell manual decision must also be valid. New Core classes with two fake 2D panels without actual owner interaction are **interim**, not completion.

**Not approved:** MA-2 extrapolation, AI/comparator teaching preview, automatically carrying A treatment to B, positive G1 COMPARABLE, ≥2 scientifically qualified targets/control, source-coefficient or threshold guesses, Unity/3D, species/site/light/regeneration/economics, durability/save migration, P0/fixtures/golden changes, source common-core rewrite. Existing [MA-1/MA-2 broader author draft](https://github.com/jermur788/CCF/blob/docs/multi-885-incremental-multi-area-draft-spec-gpt6-20261120-v1/Docs/Handoffs/MultiRack-Simulation-20261116/885-multi-area-spec/Multi-885-GPT6-MA1-MA2-DRAFT-SPEC-RESULT-20261120-V1.md) remains read-only DRAFT for MA-2; [#828](https://github.com/jermur788/CCF/issues/828) remains unfulfilled scientific G1 target contract and #851 positive-G1 scientific NO-GO.

## Local/source implementation packet — PREPARED, NOT AUTHORISED TO START

**Purpose / difficulty / proposed provider:** bounded high-difficulty C# multi-area identity, annual-state and transaction implementation, plus narrow 2D player UI; later local GPT-6.1 Sol/Codex or an equivalently equipped C# worker subject to actual availability and product owner start approval. A one-writer local code lane and a genuinely separate nonauthor reviewer are required; no permanently assigned provider.

**Gate before ANY local worker dispatch:** Owner must separately say START MA-1 IMPLEMENTATION (current explicit instruction is NOT to start). Manager #885 then refreshes (a) live `Docs/Project/coordination/active-tasks.md` plus physical Ops lease, (b) current source/Unity/host ownership and locked #882, #867, #877 private browser servers, (c) actual #882 owner G2 disposition, (d) exact tested source SHA/branch/root and its compile-linked files, and (e) no overlapping dirty work. Original #852 design was pinned to `4c42e2012a4c1bee3a422e41a58535343d63ac9f`; #882 separately reviewed/repaired source `e11dd5192d7a89abf859613ce92814bdd6338ea2` is **PLAYER-TEST-READY per [#882 launcher packet](https://github.com/jermur788/CCF/blob/docs/multi-882-player-test-ready-20261120-v1/Docs/Handoffs/MultiRack-Simulation-20261116/882-orchestration/Multi-882-PLAYER-TEST-READY-HANDOFF.md) but is not owner G2 accepted or necessarily merged**. Source implementation base/branch/worktree/actor/write allowance are therefore **UNASSIGNED**. Do NOT branch from main by default.

**Potential narrowly scoped code envelope, NOT YET GRANTED:** new files under `Tools/MultiRackSitkaP1Lab/src/Core/MultiArea/` for independent manifest/state/cross-area controller, new tests `Tools/MultiRackSitkaP1Lab/tests/Core.Tests/MultiArea*`, new distinct fixture family `Tools/MultiRackSitkaP1Lab/fixtures/multiarea/`, additive bounded registration `tests/Core.Tests/Program.cs`, plus expressly named necessary C# API / HTML / JS / CSS UI paths **only after** native compile-dependency and scope inspection. These candidates come from [#852 engineering design](https://github.com/jermur788/CCF/blob/c23e411555771320cb327e918c7ae80887388889/Docs/Handoffs/MultiRack-Simulation-20261116/852-orchestration/Multi-852-G1-FIXTURE-AREA-IDENTITY-PER-AREA-ENGINE-AND-WORK-PLAN-DESIGN-RESULT-20261120.md), not an inherited edit grant. Existing public player API, AnnualRules, AnnualEngine, WorkContract, Domain, FixtureAccess, frozen P0, old save/Legacy40, P30 and Year1–10 golden are **READ ONLY** absent new G4. Do not modify Unity/3D or main. All linked files are explicitly accounted for, not implicitly permitted by SDK globbing.

**Execution phases once separately started:**
1. `MA1-00` read-only local preflight and exact file-level packet/gate; no code until Manager admits lane.
2. `MA1-01` new independently versioned deterministic A/B experiment, actual separate rosters, manifest hashes/IDs/RNG/geometry; negative validation.
3. `MA1-02` unchanged annual rules separately for each area; shared year; fault-before-publish and protected original oracle.
4. `MA1-03` one global atomic manual Crop/Fell Work Plan with truthful reasons, Approve0, Execute1, per-area receipt, replay/stale/fence/empty-area negative cases.
5. `MA1-04` genuine 2D area selector/map/inspector and combined plan, interactive A/B qualification.
6. `MA1-05` independent nonauthor Tier2 review and owner personal G2 before Manager integration or acceptance claims.

**Verification demanded, NOT RUN:** Core/Tier1 parity, old ten Year1–10 golden, frozen P0 multi-hash, new unique two-area trees and deterministic separate Year+1, adversarial all-or-none fault injection, empty-area and stale/replay refusal with zero mutation, real browser A/B workflow, 1280×720 and 200% zoom/keyboard, exact source provenance and no raw tokens, separate negative security probe. No Tier3/Windows build without applicable gate. Test compilation alone does not prove user gameplay.

**Stop/report:** no fresh authorised start; source/lock conflict; protected/compile-linked file outside granted set; site/edge biology not covered by spec; changed original reference; any fractional cross-area commit; fixture provenance problem; owner #882 private test interrupted; source review/negative gate fails. Stop only blocked action, give Manager #885 a precise evidence-backed change request, do not self-grant. Handoff states IMPLEMENTED / REVIEWED / INTEGRATED / VERIFIED / ACCEPTED separately.

## Next authorised Manager action

**None in production.** Finalise docs, register this G5 scope under #885 issue, and await owner instruction to start a local implementation packet. The owner may continue #882 private G2 browser playtest independently; no process/fixture/lock interaction by this documentation task.

**Docs handoff is complete only after GitHub final readback and branch compare**; source checks, actual agent registration and canonical decision-log/Drive composite publication remain NOT DONE unless separately evidenced.
