# Manager review — D-070 hidden workbench retirement (CCF #284)

**Date:** 2026-11-05.  
**Verdict:** **APPROVED FOR THE BOUNDED SOURCE/OFFLINE IMPLEMENTATION HANDOFF; NOT APPROVED FOR INTEGRATION OR RUNTIME/FEATURE ACCEPTANCE.** Status remains **IMPLEMENTED / SOURCE-REVIEWED / UNITY UNQUALIFIED**. Do not merge, rebuild the candidate, or authorise a Windows build.  
**Owner:** Overall Manager / browser GitHub-only lane #200.  
**Implementation claim:** [#284](https://github.com/jermur788/CCF/issues/284).  
**Worker handoff:** [T3-WORKBENCH-RETIREMENT-IMPLEMENTATION-HANDOFF.md](https://github.com/jermur788/CCF/blob/feature/scenario-one-workbench-retirement-20261105/Docs/Handoffs/ScenarioOne-Access-Spine-20261105/T3-WORKBENCH-RETIREMENT-IMPLEMENTATION-HANDOFF.md).  
**Candidate:** branch `feature/scenario-one-workbench-retirement-20261105`, implementation commit `57f10ff09a5e1b4e1e776ed7be2b0d46d6f7c411`, current reviewed handoff tip `8b1946dcec00587971d1f7c80004a47ebaa88515`. Derived from `main` `50042bbe97734b031210ddebb603b5ec00ff459d`.  
**Next claim:** [#287 — Phase-3 Unity qualification](https://github.com/jermur788/CCF/issues/287), WAITING, with [exact packet](T3-WORKBENCH-PHASE3-UNITY-QUALIFICATION-PACKET.md).

## 1. Evidence inspected by Manager

- Compared current remote feature tip with its actual common base through the GitHub commit API. It is an ahead-only four-commit branch with **seven changed production/verification files plus one newly published handoff**. No unrelated tracked changes in the branch comparison.
- Inspected the source patches in `ForestBuildable.cs`; `ForestStartingStand.cs`; `ForestSaveController.cs`; `ScenarioOneManager.PrescriptionApproval.cs`; `ForestSceneBuilder.cs`; `ForestTest.unity`; `MfmCoverageWorldUnityGate.cs`. Source change is seven files with **+34/−9**; `ForestTest.unity` gains exactly one serialized flag line. No deleted GameObject, changed workbench position, four prerequisite links, rack code, `Dirt Path`, MFM partition algorithm or schema.
- `ForestBuildable` serializes default-false `retiresFromStartingWorld`, initialises runtime `retiredFromStartingWorld` in `Awake`, and uses the runtime value to suppress visibility, colliders and interactions. All non-workbench scene buildables retain the default policy; dev scene generator sets the one intended workbench instance flag.
- `ForestStartingStand.CollectClearancePoints` and `ReadCoverageWorld` skip only runtime-retired buildables; `ForestSaveController.CaptureData` omits them from new saves. `LoadData` collects saved buildable IDs before restoring built status and reactivates a retired-by-policy buildable when its ID is in a historical save. This is a real state-dependent load contract, not deletion of the old component.
- Reviewed full published worker handoff `1–9 and the independent Claude Sonnet 5.5 Medium findings embedded in `7`; static/measurement receipts are **worker-reported and reviewed**, not rerun by the browser Manager.
- Checked live ownership authority `Docs/Project/coordination/active-tasks.md` and #284. Isolated clone is a worker-reported implementation lane; the browser Manager has not verified local cleanliness, live editor state, mount identity or Unity lease.

## 2. Manager disposition

1. **Source scope — APPROVED.** The seven source changes follow the user-approved narrowly scoped new-world retirement contract. They keep `workbench-01`, the object/assets and its four prerequisite links for old saves/Legacy40/Reference Future, while omitting it from future new-game Site/clearance/save state. They do not implement or pre-approve the central access spine or the five-rack herringbone (separate DRAFT #283/#286).
2. **Historical consistency — plausible from source, not runtime-qualified.** An older save with `workbench-01` restores its old workbench behaviour. A save without the ID leaves it retired. Pre-buildables/empty-list older saves therefore also retire it: that limitation is explicitly disclosed and requires a negative test, not a hidden migration. Reference Future v1 embedded bytes and old saved history must remain unchanged.
3. **Offline evidence — ACCEPTED AS WORKER EVIDENCE, not remeasured here.** The worker's deterministic scene/JSON check reports preserved 72-object subtree, four linked buildables, five planting lattice slots all already covered by Forest Clearing, zero newly exposed lattice placements, removed Site-only crescent ~0.0969 m² and zero uniquely exposed measured stems. **No Unity planting, coverage or physics PASS can be inferred**.
4. **Independent review — SOURCE APPROVED.** Claude reviewer found no source defect but identified four absent real Unity lifecycle assertions and two verifier-alignment improvements. The Manager confirms these become binding Phase-3 checks. Reviewer report is embedded in the tracked implementation handoff; no unseen runtime evidence is assumed.
5. **Exact current-world anchors — remeasurement required.** Future new Enlarged80 captures omit exactly one buildable ID by design. Old start/year-3 hash labels must **not** be overwritten as if the same world; qualify and record new labelled hashes if the actual runtime follows the intended source path. Legacy40 and frozen Reference Future v1 hashes are protected.
6. **Integration — NOT APPROVED.** Source/offline confidence is sufficient to proceed to a focused Tier-2 feature gate once safe Unity occupancy becomes available, not to merge the branch. This Manager review does not change status to VERIFIED/ACCEPTED.

## 3. Exact gates before integration

- **Playtest/lock blocker:** user's personal Unity playtest has not been explicitly released. Worker reports `scripts/unity-lock status` refuses due to configured ZX20 device `/dev/sda2` versus actual `/dev/sdc2`; common exfat UUID alone does not justify forcing a lock or editing lease records blindly. Require explicit user release, safe mount/UUID ownership validation and a legitimate exclusive lease before Editor.
- **Four mandatory Unity assertions:** fresh new game omits `workbench-01` on capture, historical save containing the ID restores workbench, entering/exiting Reference preview leaves a new-game world retired again, and new-game save round-trip remains retired.
- **Additional masks/test independence:** explicit MFM Site membership = intended live nonretired buildables, target six new-game sites versus legacy historical count, no changed Track/Headland/Clearing sources; align `Tools/Verification/StandGeometry/Enlarged80Verification.cs` where its exclusion oracle remains a superset rather than current policy. Do not weaken assertions.
- **World baseline / compatibility:** verify starting tree count and identities, location/clearance, new start and Year-3 `CurrentWorldHash` labels, old versions including empty-buildables, Legacy40 and frozen v1 reference/preview/continuation; no archive mutation. Check Rack V2/Objective 1, Site map and Work Plan nonregressions.
- **Player-facing checks:** representative 1280×720 and 1920×1080 screenshots showing no stray build site/collider/interaction, otherwise faithfully displaying existing scene. No Windows or Player build.
- **Evidence:** publish exact runnable commands, head commit, logs/assertions, saved test fixtures/captures and independent result; no tests waived or relabelled from FAIL to PASS.

## 4. Next bounded task and work ownership

**#287 — focused Phase 3** is **WAITING / NOT RUNNING** until the two independent gates (user session release, lock identity/lease resolution) are satisfied. Recommended returning T3 GPT-6.1 Sol Medium (actual availability unknown); exact isolated clone `/media/jer/ZX20/CCF-WorkbenchRetirement-T3-20261105` and branch `feature/scenario-one-workbench-retirement-20261105`. The worker must verify current branch/HEAD, compare to this candidate and own only the seven existing workbench files plus new/adjusted focused verifier paths; no overlap with other production writers. The original #284 source implementation claim is handed off/closed, not running concurrently. #200 alone owns live coordination and integration verdict.

**Next required handoff:** `Docs/Handoffs/ScenarioOne-Access-Spine-20261105/T3-WORKBENCH-PHASE3-UNITY-QUALIFICATION-RESULT.md` on the feature branch. It is not proof of completion until actual exact-head Unity evidence is present and reviewed.

**User action required for running Unity Phase 3:** when finished with the personal test session, explicitly confirm it has ended and that the machine is free; this is **not** itself an authorisation for a Windows build.

## 5. Outcome summary

- **Changed:** narrowly scoped future new-game workbench state, not scene deletion; source/handoff published in isolated Git branch.
- **Passed:** published offline/static checks and independent source review; Manager source-scope review APPROVED.
- **Unrun/blocked:** Unity import/compile, saved-world/Reference/Legacy40, WorldHash and scene visual gates, user playtest release and exclusive lease.
- **Meaning:** feature candidate is ready for a *later* exact-head Tier-2 qualification, **not integration**.
- **Dashboard:** update Manager coordination and `jermur788/Overall-Dev/updates/ccf/latest.md`; verify read-back and report downstream ingestion separately.
