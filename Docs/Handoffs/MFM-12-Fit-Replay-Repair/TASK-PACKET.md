# MFM-12 Fit-Replay Review Integration — Task Packet

**Parent qualification:** #225 MFM-12  
**Browser Overall:** #200  
**Classification:** **PACKET ONLY — bounded production repair**  
**Difficulty:** Medium/High — the pure fit core already exists, but the repair crosses live manager read state, Treatment Review presentation and the prescription approval gate and therefore requires Unity/rendered regression evidence.  
**Best-suited primary:** SOL/CODEX high-reasoning local implementation lane.  
**Independent reviewer:** separate cross-provider reviewer, preferably Claude Sonnet 5.5 or equivalent.  
**Starting lineage:** `integration/scenario-one-player-test-ready-v1` after the MFM-12 STOP Manager review; record the exact starting HEAD in the claim at RUNNING preflight.  
**Suggested branch:** `fix/mfm-fit-replay-review-v1`  
**Worktree:** dedicated clean lane assigned by #203.  
**User action required:** none beyond dispatching the local worker.

## Purpose

Repair blocking MFM-12 defect S1 by connecting the **existing** deterministic `ScenarioOnePrescription.CheckFit` source-sample replay to the existing player-visible Treatment Review flow before wider prescription approval.

This is not a prescription redesign. The fit check is the spec's honesty device: it tells the player how closely the derived rule reproduces the treatment they actually demonstrated in the source sample.

Binding authority:

- `specs/multiscale-forestry-management-v1/spec.md` §7.3;
- checklist E4, E5 and J7;
- `Docs/Handoffs/MFM-12-Integrated-Qualification/MANAGER-REVIEW-STOP-20261102.md`.

## Existing accepted core

`ScenarioOnePrescription.CheckFit` already computes the necessary deterministic replay data, including:

- `Agreements`;
- `PlayerOnly`;
- `RuleOnly`;
- per-Crop-Tree `PlayerShare` and `RuleShare`;
- player/rule basal-area removal Share;
- whether the rule was limited by the accepted basal-area ceiling.

Reuse this core. Do not create a second fit algorithm or duplicate authoritative prescription logic in UI code.

## Required product behavior

Before wider prescription approval, the live player flow must obtain a valid fit replay from the same current authoritative sample/world state used for the prescription and expose it in Treatment Review.

At minimum the player must be able to see:

1. an agreement/disagreement count or equivalent clear summary;
2. which sample decisions the rule would have made differently, distinguishing player-only and rule-only removals;
3. relevant Share/removal consequences, including understandable player-vs-rule Share/removal information;
4. an honest invalid/stale state if the fit can no longer be read from the current authoritative world.

The presentation must make clear that this is a comparison with the player's demonstrated treatment, **not a score of forestry correctness**.

The player must not be able to use the wider prescription approval action while the required current fit replay is unavailable/invalid or otherwise bypassed. Keep this gate separate from Work Plan approval: prescription approval still creates ordinary pending work only, and Work Plan approval remains the commitment gate.

## Preferred architecture boundary

Keep authoritative reads/rules in Scenario One manager/domain surfaces and presentation in the existing Treatment Review UI. A reasonable implementation may add a side-effect-free manager accessor/view for the current fit result and a small presentation model, but the worker must choose the smallest robust path consistent with existing architecture.

Do not persist transient fit UI state merely for convenience. If persistence/history implications arise beyond recomputing from already persisted sample/rule inputs, STOP and return for Manager disposition rather than expanding the save model under this packet.

## Required inspectability

Disagreements must be individually inspectable enough to satisfy checklist E5/J7. At minimum, rows must identify the affected sample tree decision and whether it is player-only or rule-only, with the relevant Crop Tree/Share context available when the core provides it.

A hidden aggregate-only score is insufficient.

## Explicit exclusions

Do **not**:

- change `CheckFit` selection semantics unless a separately proven core bug is found; if so STOP;
- retune TargetShare, first-release 30%, cumulative basal-area 40%, candidate spacing, Hegyi radius or any calibration constant;
- change coverage classification/tolerances;
- change Rack V2 geometry or world identity;
- change Sitka-only quantitative prescription scope or add broadleaf quantitative transfer;
- change candidate generation/confirmation semantics or introduce auto-confirm/bulk automation;
- make proposals into Fell marks;
- change Work Plan approval/commitment semantics;
- change save schema or migrate/fabricate old prescriptions;
- alter frozen Reference Future v1, Legacy40 or protected anchors;
- alter ecology/economy/timber/deadwood/regeneration rules;
- perform a Windows build;
- touch #194.

## Focused verification required

Before final handoff, prove on the exact repair candidate:

### Pure/static

- existing prescription-core offline suite remains green;
- add focused tests for fit presentation mapping and approval-gate behavior, including:
  - all-agree case;
  - player-only disagreement;
  - rule-only disagreement;
  - Share/removal consequence presentation;
  - invalid/stale fit read;
  - approval blocked when fit is invalid/unavailable;
  - no mutation from reading/showing fit;
  - deterministic repeat/ordering.

### Unity/runtime

Run a focused live MFM treatment-review gate showing:

- fit replay is derived from the live confirmed sample;
- player can see summary plus individual disagreements;
- Share/removal consequences are visible and coherent;
- invalid fit disables wider prescription approval without mutating marks/orders;
- a valid fit allows the existing path to proceed to proposals/spot-check/prescription approval;
- prescription approval still creates pending ordinary work only;
- Work Plan remains the commitment gate.

### Rendered evidence

Capture Treatment Review at **1280×720 and 1920×1080** for at least:

- a valid fit with disagreement(s);
- the fit summary/consequence area fully readable with no critical overlap/clipping;
- approval gate state visible;
- existing sample/coverage/candidate/proposal distinction preserved.

Because the repair touches Treatment Review, rerun proportionate:

- guided-v2;
- Combined HUD;
- PARITY-07 rendered;
- MFM treatment-review/approval gates;
- any Rack/minimap rendered gate only if those surfaces are touched.

### Independent review

Obtain a fresh independent cross-provider review of:

- exact production diff;
- compliance with spec §7.3 and E4/E5/J7;
- absence of a second fit algorithm/hidden automation;
- approval boundary and Work Plan preservation;
- rendered evidence at both target resolutions;
- scope discipline and protected boundaries.

Verdict: `APPROVED / NEEDS CHANGES / REJECTED`.

## Verification-debt observations from stopped MFM-12

The stopped qualification also found stale verification wiring:

- MFM Coverage World Unity gate still asserts save version 23;
- MFM save-provenance mutation M14 still targets `CurrentVersion = 23`, preventing M14–M25 from running;
- MFM-08 treatment-review boundary pin is stale against later accepted lineage.

These are **verification-tool debt, not product defects**. Under this repair claim, the worker may mechanically update only those stale pins/boundaries where current accepted v24/lineage authority makes the correction unambiguous, so the next full MFM-12 qualification can run current gates. Do not weaken assertions or reinterpret expected behavior. If any update requires product intent or a broader baseline decision, STOP and return it separately.

Any such tooling edits must be reported separately from the production fit-replay repair in the handoff.

## Stop conditions

STOP and return to Browser Overall if:

- the existing `CheckFit` core is found semantically wrong for §7.3 rather than merely disconnected;
- satisfying §7.3 would require a new navigation model or substantial UX redesign;
- persistence/save-schema changes appear necessary;
- the repair would broaden automation or bypass player judgement;
- protected Reference/Legacy40/Rack anchors move;
- any focused regression reveals a new production defect;
- a verification-debt change would require guessing or weakening a safety/compatibility assertion;
- independent review returns a blocker.

Do not repair unrelated production under this claim.

## Required durable handoff

Publish:

`Docs/Handoffs/MFM-12-Fit-Replay-Repair/HANDOFF.md`

Include:

- exact base/final commit;
- production vs verification-tool file list separated clearly;
- before/after behavior;
- focused pure/Unity/rendered gate counts;
- screenshots manifest;
- independent review verdict;
- Unity lock lifecycle and cleanup;
- explicit statement that the MFM-12 product token is still not issued.

Success token for this repair only:

`MFM FIT REPLAY REPAIR COMPLETE — READY FOR MANAGER REVIEW`

After Manager acceptance/integration, Browser Overall will freshly repin #225 and require a full MFM-12 qualification from the repaired candidate.