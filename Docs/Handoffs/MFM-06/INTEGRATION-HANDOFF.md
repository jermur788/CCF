# MFM-06 integration handoff — claim #113

MFM-06 V23 INTEGRATED — READY FOR MFM-07

Integration decision: **APPROVED** for this bounded foundation (G4); required post-merge qualification **PASS**. This does not release MFM-07: #121 additionally requires browser #120's explicit readiness verdict. No MFM-07 implementation or Windows build occurred.

## Exact identity and authority

- Executor: T3 Code / Codex harness / OpenAI **gpt-6.1-sol**, medium reasoning.
- Packet: `origin/main:Docs/Handoffs/MFM-06/INTEGRATION-TASK-PACKET.md`, routing commit `3864c0634e59d5fb1f392dc651114d1d06a89849`.
- Locked programme context: `63e95ba2175f7e239a278c616681b286719821f8`; live coordination refreshed from `origin/main` at `db157ac` after fetch. Manager #113 and #91 explicitly accept the independent Opus round-2 review and supersede the extra #112 cross-provider gate.
- Approved specification: `specs/multiscale-forestry-management-v1/`, MFM-06, K1–K10 and persisted J/I6 provenance foundation. Bounded conformance PASS; later workflow behaviour remains outside this task.
- Integration worktree: `/media/jer/Videos/CCF-PlayerTestIntegration`.
- Branch: `integration/scenario-one-player-test-ready-v1`.
- Freshly inspected pre-integration umbrella: **`ac876a64969752a3864034521f42559671264b30`**, clean; starting save schema v22.
- Exact reviewed input: **`13fd85452b8d3a55946fcb459af8b00e0326a579`**.
- Exact merged and Unity-qualified umbrella: **`b971f4da0540e606bfee9702b0c99f81a0a89355`**, schema v23.
- Method: `git merge --no-ff 13fd85452b8d3a55946fcb459af8b00e0326a579`; clean `ort` merge, no conflicts. Merge parents are the two full SHAs above. The entire merged Git tree equals the exact reviewed input tree; no unreviewed descendant was absorbed. This handoff is a subsequent documentation-only commit; the tested candidate remains the merge SHA above.

## Change and protected repairs

The reviewed delta adds v23 prescription IDs/allocator, rule/calibration/source/sample/coverage/approval records, work-order provenance and management-event linkage. Capture/restore/migration and typed/JSON pre-mutation validation are wired to the existing save controller. Old saves acquire no prescription state. Hash layout strips default v23 fields to preserve historical identities. Existing verification pins/comparators support v23; focused offline and disposable Unity gates are included.

Ten changed Assets paths comprise the save controller/data/validator, manager capture/restore plus new `ScenarioOneManager.Prescriptions.cs` and its meta, migration, order/event DTOs, and reference hash layout. No scene, prefab, material, Reference resource, package or ProjectSettings change. No dependency added. Production and verification trees match reviewed `13fd854` exactly.

Both reviewed repairs are preserved:

- `HashLayout` removes objective achievements even when prescription state is non-default; the merged-candidate offline history-only identity check passes.
- `PrescriptionReasonCode.None == 0`, real reason == 1; generated orders with reason 0 fail validation. Offline vocabulary/default/refusal checks and the live disk no-mutation case pass.

## Post-merge verification

All six candidate gates ran in clean detached U1 at **b971f4d**, under normal lock `8a636d64a89146fb92ebe42a087afbec`, Unity **6000.6.0f1**. Every runner result is `pass:true`, normal Editor exit 0, no gate failure/compiler error, clean lane, host saves unchanged, staged harness/metas/licence copies removed.

- Import/compile: `MFM06_COMPILE_PASS`, saveVersion=23 (97.18 s).
- Real-controller save/load: `MFM06_UNITY_PASS checks=159` (208.88 s). Three byte-identical disk save/load cycles preserve IDs/world/hash/links. A v22-shaped ordinary Rack V2 file reloads with identical world JSON/hash and no fabricated provenance. **24 malformed JSON cases + 7 typed corruptions** refuse before mutation; a good file still loads afterward. Corruptions perturb wood to detect partial application. Isolated save and backup are hashed then removed.
- Hash dump: `MFM06_HASHDUMP_PASS`, **18 lines byte-identical** to freshly executed pre-merge **ac876a6** baseline (108.28 s). Covers Legacy40 model variants, Enlarged80 empty/rack state, parsed v21/v22 inputs and history, WorldHash/Current/LegacyCompat/V15–V18 as applicable. No anchor rebaseline.
- Frozen Reference: `REFERENCE_FUTURE_V1_PASS`, archive/preview/continuation PASS (117.68 s). Frozen v12 years 0/20/50/100 match pins; tampering negative control detected. Year100 archive **7AD177B3CC2F73C7**, schedule **56C8B99FA1E8DDD1**, archive file FNV **1A42C7BD0439E72F**; deterministic continuation current diagnostic **EBD49ADEF18C3F36**, protected v15 layout **BFFF66015A6619DA**. Resources byte-identical to pre-merge baseline.
- Interaction/Legacy compatibility: `SCENARIO_ONE_INTERACTION_VERIFY_PASS` (114.15 s), unchanged production interaction gate via forwarder and explicit historical fixture setup. This is an automated Editor gate; native human-input testing was not performed.
- N3: `N3_U4_VERIFY_PASS`, 34 named checks (101.51 s), original world restored. Synthetic completion-policy fixtures, not a full physical forest playthrough.
- Additional merged-candidate offline check: **172 PASS**, explicit baseline full ac876a6 SHA, Reference resources unchanged. Includes old/missing fields, v22 history, stable IDs, no fabricated state, malformed rejection, canonical/hash rules and both protected fixes. Reviewed mutation evidence (25/25 killed) was not rerun; no harness/threshold/expected-value change was made here.
- Fresh baseline compile/hash also ran in U1 at ac876a6 under lock `2ec37b132bfb4e01bc8b7ad1987c9339`, both normal exit 0 and `pass:true`. Candidate comparison uses these fresh receipts, not earlier worker receipts.

## Reproduction and evidence

Durable root: `/media/jer/Files/CCF/Verification/MFM-06-Integration-20261029/`.

`integration-provenance.json`, `integration.diff`, `integration-diff-stat.txt`, `gate-summary.json`, `hash-comparison.json`, `offline-result.txt`, `candidate-driver.log`, accepted independent review copy, baseline/candidate gate `command.json`, `process-result.json` and `unity.log` files contain exact identities, commands, environment and cleanup receipts. Each hash dump is retained in `hash-dump.txt`.

To rerun, first reserve clean detached U1 at the exact SHA and acquire a fresh normal lock. From the merged checkout:

```bash
python3 -I -B Tools/Verification/MfmSaveProvenance/run_offline_check.py --base ac876a64969752a3864034521f42559671264b30 --out /media/jer/Files/CCF/Verification/NEW-MFM06-offline
python3 -B Tools/Verification/MfmSaveProvenanceUnity/run_gate.py --project /media/jer/ZX20/CCF-U1 --expected-head b971f4da0540e606bfee9702b0c99f81a0a89355 --owner OWNER --task-id TASK --lock-id LOCK --gate compile --out /media/jer/Files/CCF/Verification/NEW-MFM06/01-compile
```

Use the same owner/task/lock/HEAD for `save`, `hash-dump`, `reference`, `interaction`, `n3`, each at a new sibling output directory. `compile` must be first at `01-compile`. Repeat baseline compile/hash on exact ac876a6 under its own correct lock; compare both 18-line dump files byte-for-byte. Inspect the runner/harness before running. Expected observations are the markers and unchanged-world results above.

## Residuals and boundaries

Historical `progress`/`density-save` runner failures were inspected and classified; neither is called green or silently waived. On 60f17ef, progress exits 0 with its pass marker but leaves isolated `forest-save.json.bak`: source cleanup handles only the primary file. Density-save emits its pass marker with no failed assertions, then times out after 1500 s during Editor shutdown and leaves an isolated save; the exact shutdown cause is unestablished. Both preserve host saves. Details: `historical-residual-classification.md` and original `MFM-06-Unity/run2/03-*/` receipts. Accepted round-2 review explicitly did not require their rerun. They were not rerun or repaired in this bounded integration task; broader integrated qualification remains a later task.

Two reviewed stale comments remain unchanged. **Mandatory MFM-07 condition:** before any generated order is written, validate its coverage unit and Crop Tree against the referenced prescription, failing closed without partial mutation. MFM-06 has no production prescription writer; this task introduces none. #120 must review readiness and pin the post-#113 base before #121 starts.

## Cleanliness, publication and operational follow-up

Both normal locks released; final machine lock FREE with no Editor. U1 remains clean detached at exact qualified b971f4d; no lane reservation retained. Integration was clean before this handoff. Primary's existing modified `AGENTS.md` / `active-tasks.md` and three untracked documents were untouched. No temporary auto-start harness/meta remains. No Windows or other Player build.

The umbrella integration ref is published to origin, making exact reviewed input and exact merged candidate reachable. The final handoff is published as a docs-only successor; no production verification is attributed to that later commit.

Dashboard sync: **NOT AVAILABLE / publication not verified**. No supported dashboard publisher/ingestion tool was available in this session; no generated dashboard state was edited. #113 and parent #91/#106 receive the exact result and publication receipt. Browser/Manager #120 must reconcile the live coordination record (which it owns while running), review this handoff, and verify dashboard publication/readback if in use. This operational follow-up does not claim final programme acceptance or release MFM-07.
