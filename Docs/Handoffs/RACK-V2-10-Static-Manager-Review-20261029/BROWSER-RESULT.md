# RACK-V2-10 Static Manager Review Result

**Verdict: APPROVED — STATIC PHASE ONLY**

- Browser review claim: `#157`
- Implementation claim reviewed: `#94`
- Exact implementation base: `6d3228c6ce7cbb1c3ec7b99339dd7a38c92a4c22`
- Exact production candidate: `db17d5df167d6d13cded0518da67f507a7f771ee`
- Worker docs-only handoff successor inspected: `6c07c21d55b53f92e053f2a83605e031227ba2a3`
- Review mode: GitHub/browser static inspection only. No Unity/C# execution, no rendered qualification, no Windows build.

## Manager determination

The exact candidate is acceptable for the RACK-V2-10 **static/offline help-teaching phase**. The implementation stays within the bounded teaching/help surface, matches the approved Rack V2 wording authority on the reviewed points, preserves existing learning IDs/progress keys, and narrows `MenuTutorialVerification` only for the intentionally conditional Tree Marks screen.

This verdict does **not** establish Unity import/compile, modal timing, text fit, visual quality, or rendered acceptance. Those remain separate qualification concerns.

## Findings

### 1. Exact diff and scope — PASS

`6d3228c6ce7cbb1c3ec7b99339dd7a38c92a4c22..db17d5df167d6d13cded0518da67f507a7f771ee` is exactly one commit ahead. The ten changed paths are limited to:

- `Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs`
- `Assets/ForestPrototype/UI/LearningObjectivesView.cs`
- `Assets/ForestPrototype/UI/MenuHelpView.Text.cs`
- `Assets/ForestPrototype/UI/MenuHelpView.cs`
- `Assets/ForestPrototype/UI/RackLayoutTracker.cs`
- `Assets/ForestPrototype/UI/ScenarioOneUiRoot.cs`
- `Tools/Verification/MenuTutorialVerification.cs`
- `Tools/Verification/RackV2HelpTeaching/README.md`
- `Tools/Verification/RackV2HelpTeaching/RackV2HelpTeachingOfflineCheck.cs`
- `Tools/Verification/RackV2HelpTeaching/run_offline.py`

No Rack geometry/authoring implementation, marking implementation, `ScenarioOneManager`, save/schema, scene/prefab/material, package/ProjectSettings, S1-C, S1-D, MFM, Parity, ecology/economy, or Reference Future production path is changed by this candidate.

### 2. Tree Marks scope — PASS

`MenuHelpView.TreeMarkEntries` contains exactly the three currently relevant meanings:

1. `Orange vertical stripes — Rack / machine-path removal`
2. `Red — Fell`
3. `Blue — Crop Tree / future timber`

No unused future mark category is introduced. The bounded offline check also explicitly rejects a fourth entry and scans for unused/future colour/category wording outside the orange source sentence.

### 3. Orange authority and first-tree simplification — PASS

The candidate uses the approved §28 wording verbatim:

- source material supports removal/machine-route vertical stripes and describes orange or pink;
- **Scenario One chooses orange**;
- the game explicitly says that asking the player to mark the rack entrance tree is a **teaching action/simplification** and that real practice may mark additional trees along a route.

The help further distinguishes the source's four-sided stripe representation from the game's one-spray interaction. It does not present first-tree-only marking as universal forestry practice.

### 4. Spacing and five-rack wording — PASS

The reviewed V2 teaching surfaces keep the evidence/calibration boundary:

- `16–20 m` is described conditionally for early transformation and as Scenario One's guide for this plantation;
- `30 m or more` is presented only as a possible developed-CCF example with `no universal rule`;
- five racks are stated as **this stand's** required first access layout / Scenario One calibration.

The candidate does not teach `18 m ideal`, `always five racks`, a universal five-rack quota, or universal 16–20 m spacing.

### 5. Objective 1 order — PASS

The V2 instruction/help sequence is consistent with the approved §30 / D-064 ordering:

`Plan on Stand Map -> Confirm Rack Plan -> return to forest -> mark rack entrances -> review/approve rack work in Work Plan -> advance one year`.

`RackV2Instruction` does not offer approval while fewer than the target entrances are marked. Once all five entrances are marked and work is pending it directs the player to approve before advancing; after approval it directs the player to advance the year.

`RackV2Explanation`, `rack.layout`, and `rack.approve` copy follow the same order.

### 6. Stable lesson IDs and saved progress semantics — PASS

The candidate does not add, remove, rename, or reorder Objective 1 lesson IDs. The existing sequence remains:

`rack.read, rack.create, rack.layout, rack.approve, rack.result`

and `rack.map` remains the existing optional map lesson ID. The candidate adds V2 text substitution around those IDs rather than new progress keys. `PreferencePrefix` and the existing `Record`/`ReloadProgress` use of `StepIds` are unchanged; the offline check asserts 48 unique step IDs.

A deliberate consequence of preserving progress semantics is that `rack.layout` remains credited from authoritative layout/work state rather than becoming a new persisted field-marking progress key. Field marking is instead kept authoritative through the live V2 counter/instruction before `rack.approve`. This preserves the requested saved-progress contract rather than inventing a new lesson ID or save field.

### 7. `MenuTutorialVerification` narrowing — PASS

At the base, the final preference assertion iterates all help menus except the Rack screen and reports `allFive=true`.

At the candidate, the same assertion excludes only:

- `Menu.Rack` — already intentionally introduced by the rack tool rather than this ordinary flow; and
- `Menu.TreeMarks` — newly conditional on a confirmed Rack Planning V2 plan and therefore absent from this non-V2 verification flow.

The five ordinary menu assertions remain intact. The test setup still enumerates all menu values for preference isolation/restoration. This is a bounded conditional-screen exclusion, not a weakening of the existing five-menu coverage.

### 8. Offline/static evidence — ACCEPTED AS WORKER EVIDENCE

The worker handoff and claim report:

- `RACK_V2_HELP_TEACHING_OFFLINE_PASS checks=97`
- `RACK_V2_HELP_TEACHING_PASS static_and_offline=15`

The committed harness substantively checks the reviewed contract: unlock/context wiring, three-mark entry set, verbatim §28 disclosures, §31/§7 copy, Objective 1 flow states, unchanged lesson IDs, forbidden universal/legacy wording, and protected-path scope from the pinned base.

This browser review inspected the harness and reported outputs but did **not** re-execute them. They are therefore accepted as static/offline worker evidence, not as Unity/rendered proof.

## Residual / next gate

- No Unity import/compile was performed by this review.
- No rendered inspection or text-fit/modal-timing acceptance is inferred.
- No Windows build was performed or authorised.
- Any later integrated or rendered Rack V2 verdict must remain tied to its own exact candidate/evidence.

**Final disposition: APPROVED — STATIC PHASE ONLY.**
