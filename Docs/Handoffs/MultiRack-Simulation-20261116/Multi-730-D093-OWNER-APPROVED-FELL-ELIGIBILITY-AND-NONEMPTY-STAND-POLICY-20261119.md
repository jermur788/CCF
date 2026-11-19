# Multi #730 — D-093 owner approval: 170 mm eligibility metadata and last-live Fell refusal

**19 November 2026 | OWNER PRODUCT DECISIONS CONFIRMED | G2 SOURCE IMPLEMENTATION HELD**

## Exact owner decision

At 21:36 UTC the product owner replied **“Approved.”** to Manager #730's **two choices and specific recommended options** in the immediately preceding Game Dev conversation:

1. **170 mm action threshold (option B accepted):** `actionEligibleDbhThresholdMm = 170` in the **synthetic** 30 m / 100-Sitka fixture is **informational metadata only**. Do not impose it as a player-Fell eligibility prohibition; every genuinely live, exact, source-bound tree is potentially selectable through deliberate manual judgement and Work Plan approval. This is **not** an empirical Irish legal, tree-size or forestry practice assertion.
2. **Last-live Fell (option A accepted):** if any proposed approved Fell batch would leave **zero living trees**, the entire batch is **REFUSED atomically**. Use a clearly understandable user-facing refusal equivalent to `WouldLeaveNoLivingStems`. No partial changes to live/dead roster, natural-death/Fell cause, work result, session history/event, execution-key map, workRevision or annual year/revision. No Stage5 terminal clearfell mode.

**Manager disposition:** **APPROVED DIRECT OWNER PRODUCT POLICY**. Record as [D-093 in staged canonical Decision Log](https://github.com/jermur788/CCF/blob/docs/multi-730-d093-owner-g2-fell-policy-20261119-v1/Docs/Project/decision-log.md), subordinate only to a newer explicit owner decision. The product approval is **not** an implementation authorisation, source grant, physical lease, approval for existing #789 historical V-01/V-02 variance, L4 waiver or merge permission.

## Technical contract integration

Manager #730 updated the existing four-file Stage5 G2 contract **proposal** on its own isolated branch, leaving the implementation-held status intact:

- [spec.md — §8 G2 atomic contract and D-093 resolved choices](https://github.com/jermur788/CCF/blob/docs/multi-730-837-g2-contract-decision-draft-20261119-v1/specs/multirack-sitka-2d-intervention-workflow-v1/spec.md)
- [plan.md — source admission first](https://github.com/jermur788/CCF/blob/docs/multi-730-837-g2-contract-decision-draft-20261119-v1/specs/multirack-sitka-2d-intervention-workflow-v1/plan.md)
- [tasks.md — bounded T-02a and later one-writer native work](https://github.com/jermur788/CCF/blob/docs/multi-730-837-g2-contract-decision-draft-20261119-v1/specs/multirack-sitka-2d-intervention-workflow-v1/tasks.md)
- [checklists/requirements.md — unchanged 26 R + 24 X, all unchecked](https://github.com/jermur788/CCF/blob/docs/multi-730-837-g2-contract-decision-draft-20261119-v1/specs/multirack-sitka-2d-intervention-workflow-v1/checklists/requirements.md)

The intended source contract preserves **legacy bit-serialised no-work `{Snapshot, Report}` shapes**, `EcologicalYear` and annual `Revision`. It proposes an **additional session work revision outside those old serialised shapes but within one immutable atomic commit root** with the Fell roster, typed intervention cause, one event and session replay records. Exact C# ABI and qualifying test scope are still pending actual source/physical admission; source owner may return a precise constraint.

## Remaining blockers — separate from decisions now closed

- [#837 G0 source lineage handoff](https://github.com/jermur788/CCF/blob/review/multi-837-g2-lineage-contract-readonly-20261119-v1/Docs/Handoffs/MultiRack-Simulation-20261116/837-orchestration/Multi-837-G2-SOURCE-CANDIDATE-ADMISSION-AND-CONTRACT-G0-RESULT-20261119.md) confirms local candidate/archived bundle ancestry, but the authoritative ZX20 Ops board, Files #725 original and physical #791 source custody were **not mounted or directly read**. Never assume lease free.
- [Scenario One Manager #824's prospective V-01 request](https://github.com/jermur788/CCF/issues/824#issuecomment-7022773936) is **OPEN** for an owner and #824 **read/compile-input-only grant** to unchanged `Assets/ForestPrototype/SitkaGrowthModel.cs` and `SimulationRandom.cs`. The approval of the two *gameplay* policies does **not** grant this source permission. No `Assets` write.
- V-02 final exact candidate mirror/reproducible full-repo .NET build, 50-test/source/fault checks and ten protected no-work digests require an **independent native** actor under a subsequently approved real custody/physical grant; the archived intermediate 37/45 is **not** the separately reported final 50/50 proof.
- #804 main PARKED/BLOCKED; V-01/V-02 unwaived; L4 FAIL38 unwaived; G1 multi-area comparator #828 DRAFT, no source area identity; 26 R / 24 X tests NOT RUN, zero acceptance claims.
- No source code, 100-ID fixture, goldens, P0 DLL, physical Ops, source/asset bytes, Unity, Windows or main merge was changed by this owner-decision recording.

## What next

**Do not ask the owner to decide D1/D2 again.** Obtain the **current physical board/source custody readback** and #725 original ownership with a named authorised local reader; separately await #824 source-owner V-01 review and product-owner source grant (different from this D-093 product approval). After actual source/physical admission, one independent V-02 exact-candidate qualification and a single writer/independent-verifier lane can be authorised for G2. Manager #730 issues any technical-source contract approval and implementation release only after those hard gates.

**Publication boundary:** this D-093 decision-log edit and handoff are on a **documentation branch**, not yet merged into main. The prior Stage5 implementation remains HELD even when the D-093 docs are published. Dashboard/Drive and platform-context refresh require the separate canonical publication process; no per-task dashboard claim.
