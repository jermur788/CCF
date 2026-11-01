# Scenario One Verification-Debt Repair — Task Packet

**Claim:** #223  
**Parent:** #221 / #200  
**Local orchestration:** #203  
**Classification:** **PACKET ONLY — verification-only repair/reconciliation**  
**Difficulty:** Medium — several verification assets touch different accepted features, but production behavior is excluded.  
**Best-suited provider/model:** SOL/CODEX high-reasoning local lane; independent static review may use Claude/DeepSeek.  
**Starting production candidate:** `3acdf9cb635cdca79606335801e97f9d7dcce8b0`; if #224 integrates first, repin to the newer exact production candidate before Unity.  
**Suggested branch:** `verify/scenario-one-debt-repair-v1`  
**Worktree:** dedicated clean local worktree assigned by #203.  
**User action required:** none.

## Purpose

Repair stale/broken verification wiring exposed by #219/#199 without changing production semantics, then rerun the affected gates so MFM-12 can start from current, truthful verification assets.

Binding classification: `Docs/Handoffs/ScenarioOne-Verification-Debt-20261101/BROWSER-DISPOSITION.md`.

## Authorised edits

1. Repin the five mechanically stale v23 save-version/hash expectations to the accepted v24 contract. Known affected surfaces from PARITY-09 include:
   - integrated/static protected-file/hash check involving `ForestSaveData.cs`;
   - SiteInfrastructure save-version assertion;
   - pruning verification protected-save pin;
   - MixedDensity Unity `POLICY_SAVE_VERSION_UNCHANGED` assertion;
   - BroadleafTimberEconomy Unity save-version assertion.
2. Add `ScenarioManagementHistory.cs` to the ThreeSpecies history harness/project compile inputs so `WorkItemDetail` resolves.
3. Narrow the `MaxRemovedBasalShare` source scanner so the MFM maximum **removal share** is not treated as a biological species maximum-density/max-basal-area relationship. Preserve the intended species-density guard and its negative controls.
4. Reconcile guided-v2 teaching-sequence index/count expectations against accepted MFM-09 authority. Stable IDs are authoritative; visible numbering may change and the accepted extension step exists after Lesson 5. Update only expectations proven stale.
5. Reconcile the guided-v2 V2 confirmed-rack annual-advance lesson expectation against current accepted Rack V2/MFM-09 product authority. If authority does not prove the expectation stale, stop and report a product defect rather than changing the test.

## Explicit exclusions

- No production C#/UI/gameplay changes.
- Do not alter the accepted pure-Beech R2 values or `0.40` criterion.
- Do not weaken/suppress `current objective fits` or any rendered-fit assertion; that product issue belongs to #224.
- No ecology/economy/calibration changes.
- No save-data semantic change or schema bump.
- No protected anchor/reference rebaseline.
- No Windows build.
- #194 NO ACTION.

## Required verification

After edits, on the exact current downstream candidate:

- relevant integrated/static checks that previously stopped only on v23/hash staleness;
- `ThreeSpecies/run_offline_check.py --suite timber` and density/isolated guards as appropriate;
- `run_pruning_check.py`;
- ThreeSpecies `--suite history`;
- MixedDensityVerification in Unity;
- BroadleafTimberEconomyVerification in Unity for **Legacy40 and Enlarged80**;
- guided-v2.

For guided-v2, the expected post-task state is:

- stale schema/sequence/rack-expectation failures removed only when accepted authority supports the correction;
- the Objectives-window overflow may remain until #224 is resolved and must stay visibly red if still present;
- no new failure family introduced.

If #224 integrates before the final rerun, rerun guided-v2 on that exact post-#224 production candidate and expect its objective-fit family to be green as well.

## Stop conditions

Stop and return to #200 if:

- a changed verification expectation would require guessing product intent;
- a supposedly stale assertion protects an actually required compatibility contract;
- timber/mixed/pruning/history reruns reveal a production failure after the verification wiring is repaired;
- a protected hash/anchor differs for reasons not already accepted.

Do not fix production under this claim.

## Handoff

Publish:

`Docs/Handoffs/ScenarioOne-Verification-Debt-20261101/HANDOFF.md`

Include exact starting/final HEAD, changed verification files, before/after failure families, all gate counts/receipts, Unity lock lifecycle, cleanup, and a clear distinction between repaired verification debt and any remaining product defect.

Success token:

`SCENARIO ONE VERIFICATION DEBT REPAIRED — READY FOR MFM-12 REPIN`

Copy/paste dispatch:

`Run claim #223 from Docs/Handoffs/ScenarioOne-Verification-Debt-20261101/TASK-PACKET.md. This is verification-only: repin the mechanically stale v23 assertions to accepted v24, fix the history harness compile list, narrow the MaxRemovedBasalShare false positive, and reconcile guided-v2 sequence/rack expectations only where accepted authority proves them stale. Do not touch production or weaken the Objectives-window fit assertion. Rerun timber/mixed/pruning/history/guided gates on the exact candidate and stop on any genuine product defect. No Windows build; #194 NO ACTION.`
