# PLAYER-TEST-BROWSER-LANE-01 — PARITY-07 player-facing species truth review

## Identity

- Parent: #91 `PLAYER-TEST-OVERNIGHT-01`
- Browser lane: #105
- Feature claim: #102
- Execution/review lane: #106
- Browser inspection model/provider: OpenAI GPT-5.6 Sol, GitHub/browser-only manager lane.
- Live PARITY-07 task is **player-facing species truth**. It is not a save-migration task.
- Implementation base: `23fe4b88827665c9707bfcc36778b8cfe98d5ffe`.
- Reviewed code candidate: `1e7b9413da770fcf9ee83b46024c82034deb2fd3` on `feature/parity-07-player-facing-species-truth-v1`.

## Sources inspected

- Live #102 body/comment.
- `Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-07/HANDOFF.md`.
- Approved `specs/scenario-one-three-species-parity-p0/spec.md`, especially FR-3, FR-6 through FR-9 and the accepted limitations.
- GitHub compare `23fe4b8..1e7b941`.
- Candidate `ScenarioOneUiFacts.cs` and the affected `TreeInspectionView.cs` wiring.

## Static findings

1. The candidate is exactly one implementation commit ahead of its declared base. The production changes are limited to four UI/curriculum files plus a new offline wording checker. No ecology, economy, save/schema, Rack, MFM, S1-C or S1-D production file is in the compare.
2. The candidate removes the specifically prohibited Oak wording (`large and straight enough` / large-straight-log implication) and consistently states that represented broadleaf quality is based on size plus pruning history while fork/defect shape is not modelled.
3. The premium-Oak sentence is explicitly conditional, late, favourable, low-competition and pruning-dependent, and says the outcome is neither promised nor required. That is aligned with FR-9.
4. The pruning teaching text distinguishes the existing Sitka three-lift schedule from the accepted two-lift broadleaf schedule rather than presenting one shared schedule.
5. Storm wording distinguishes storms-off from the simplified shared storm model and does not claim a species-calibrated wind-tolerance band.
6. Reported offline checks are green (`PARITY07_COPY` 77/77 and existing timber suite 73), but the candidate is explicitly not Unity-compiled and not rendered.
7. **Independent wording check required:** `SharedSpeciesRulesNote` says site suitability, canopy shading and wind exposure “use the same simplified rules for all three species; they are not calibrated per species.” FR-3 says canopy/light equations are shared **but existing crown geometry/relaxation can carry species differences**. The sentence may be defensible as “shared equations”, but an independent reviewer must confirm it does not imply that represented crown-driven shading differences are absent. Do not silently broaden the claim if production state differs.

## Browser verdict

**APPROVED FOR BOUNDED T3 COMPILE/INDEPENDENT WORDING REVIEW/RENDERED QUALIFICATION.**

Static scope and the primary FR-9 corrections are acceptable. Final player-facing acceptance remains unearned until the wording nuance above is resolved and the required compile/rendered checks pass.

## Exact bounded packet for T3 Code claim #106

1. Refresh integration, #91/#102/#105/#106 and live `active-tasks.md`; preserve ownership/Unity locking.
2. Independently review exact candidate `1e7b941` against PARITY spec FR-3/FR-6–FR-9. Use OpenAI 6.1 Sol as the cross-provider reviewer if available in the T3 lane, per the user routing instruction.
3. Specifically inspect the current crown geometry/relaxation implementation and determine whether `ScenarioOneUiFacts.SharedSpeciesRulesNote` accurately communicates the shared canopy equations without falsely denying represented species-specific crown geometry. If wording is inaccurate, make only the minimum player-facing wording correction; do not retune ecology or add new mechanics.
4. Unity import/compile the exact reviewed descendant. Do not treat the worker's failed standalone compile helper as a code failure or as a compile pass.
5. Run the existing PARITY-07 copy checker and the affected timber/parity lifecycle/fingerprint gates required by the approved spec; do not weaken existing checks.
6. Render and inspect at both 1280×720 and 1920×1080 the handoff's required states: unpruned Oak; pruned Oak below/above thresholds; Beech; More Detail on Oak/Beech with storms off/on and Sitka; Work Plan harvest/pruning cards; Learning Objectives pruning topic/read step. Check clipping/overflow and semantic correctness.
7. Preserve pure-Sitka/Legacy40/frozen Reference Future and existing parity anchors; this slice has no authority to rebaseline them.
8. Publish the exact qualified SHA and evidence. Integrate only after the independent wording review, Unity compile, required runtime gates and rendered review are green. No Windows build.

## Next executable action

#106 performs the cross-provider wording/source review first, then compile/runtime/rendered qualification and integration review if green.