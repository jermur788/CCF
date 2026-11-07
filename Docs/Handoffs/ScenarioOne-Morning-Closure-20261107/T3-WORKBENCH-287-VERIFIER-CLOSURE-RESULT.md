# #318 Workbench Tier-2 verifier and provenance closure — source result

**UNQUALIFIED SOURCE PUBLICATION pending independent review #319. #287 remains NEEDS CHANGES.**

This is a bounded source repair and read-only attribution report, not Unity qualification, integration, Manager approval, or closure of the tree-substitution blocker. No Unity Editor/import/lease, native-input session, Player/Windows build or user-save access occurred.

## Candidate and ownership

- Source candidate: **`fb5eeb98053deab10f922437df7fe5c19fefd880`** (six source/test files). A subsequent report-only commit publishes this document; its SHA is available in branch history and the handoff. The source candidate SHA names the exact tested code independently of that documentation commit.
- Base: **`4d95e45245d660e24b91e26754d9c57949bf3384`**, exactly the published feature/result base specified in the packet. No merge/rebase or production delta.
- Branch: **`fix/workbench-tier2-verifier-closure-20261107-v1`**.
- Sole writable source lane: `/media/jer/Files/CCF/Operations/CCF-WB287VerifierRepair-20261107`, newly created from `/media/jer/Files/CCF/Repository.git`.
- Observed runtime instructions: T3 Code, Codex harness, **gpt-6.1-sol, high reasoning effort**, OpenAI provider family. Actual account/quota/backend routing: **UNKNOWN**; no credential/configuration inspection used to infer it. No subagents were dispatched.
- Canonical context at the pinned base: `63044dfbbeb869996ceecec8a47619dcba048b0b`; live coordination was read at task start. The direct #318 binding packet controls this source-only scope. `Docs/Project/coordination/active-tasks.md` was not edited.
- Preflight: requested local/remote branch and path were unused; original orchestrator worktree had clean status and was treated read-only. New source lane index/status was clean at the exact base. Files is `/dev/sdb4`, ext4, mounted read/write, approximately 50 GiB available. No occupied branch was forced.

Exact changed files (and no others):

1. `Tools/Verification/WorkbenchRetirement/WorkbenchRetirementVerification.cs`
2. `Tools/Verification/WorkbenchRetirement/run_workbench_retirement.py`
3. `Tools/Verification/WorkbenchRetirement/test_runner.py` (new)
4. `Tools/Verification/StandGeometry/Enlarged80Verification.cs`
5. `Tools/Verification/RackV2Persistence/RackV2PersistenceVerification.cs` (confirmed real authorised path)
6. `Tools/Verification/RackV2Persistence/test_historical_projection.py` (new)
7. `Docs/Handoffs/ScenarioOne-Morning-Closure-20261107/T3-WORKBENCH-287-VERIFIER-CLOSURE-RESULT.md` (this report, new)

Additional evidence was written only below `/media/jer/Files/CCF/Verification/Workbench287MorningClosure-20261107/additions/`. No Assets, Packages, settings, frozen reference, fixture, shared coordination or unrelated checkout was edited. Reading existing DLLs for Roslyn type checking did not create or share a Library.

## Per-item verdicts

| Packet item | Source/offline result | Unity-dependent result / remaining gate |
|---|---|---|
| Phase 0 package attribution, B1 | **PASS** attribution: exact three direct packages and one transitive package identified; manifest/lock/cache agree on resolved 1.1.1 | **NOT RUN** exact-package requalification; package-policy choice remains Manager-owned. Prior runtime evidence used 1.1.1 despite tracked 1.1.0. |
| Phase 0 material attribution, B1 | **PASS** all seven changed hunks classified as Editor rewrites with exact earlier imported/restored hash pairs and installed URP source evidence | **NOT RUN** rendering equivalence; no waiver. Eight dirty paths remain protected and unchanged. |
| Phase 1.1 lifecycle/check 36/CanLoad/fresh scene, B2 | **PASS** source repair and Roslyn type check: exact four workbench dependencies, exact two unmet Saw-Pit dependencies; null-safe diagnostic text; actual scene reload before disk fixture | **NOT RUN** lifecycle and C–G execution. No claim that the old pinned check-36 failure qualified C–G. |
| Phase 1.2 tree identity, B3 | **FAIL — BLOCKED unresolved.** Both preserved pairs confirm the one-tree substitution. Relevant source inputs identified; exact cause not proved. Final equality remains intact. | **NOT RUN** pose/state investigation in Unity. No tree movement, retuning, hash change, assertion deletion or determinism waiver. |
| Phase 1.3 save pins/projections/teaching policy, B4 | **PASS** source repair, historical schema audit, 28 executed C# projection controls and Roslyn type check | **NOT RUN** current/v20/v21/Legacy40/coldstart controller behaviour. Synthetic historical-layout tests are not genuine archived saves. |
| Phase 1.4 runner false green and cleanup, M1 | **PASS** 13 executed mock tests including exact N1–N3 and cleanup/admission controls; Python compilation | **NOT RUN** actual Editor/lease invocation. Dirty admission remains strict. |
| Phase 1.5 native old-site witness and Year-3 claim, M2 | **NOT RUN** native 720p/1080p witness; exact future protocol below. Year-3 claim explicitly narrowed. | No new paired stripped-record Year-3 proof implemented or claimed. |

## What changed and why

### Check 36 and fresh-world truth

The scene's actual `requiredBuildable` references identify these four workbench dependents: `log-rack-build-01`, `saw-pit-01`, `shelter-01`, `timber-sledge-01`. The two Saw-Pit dependents are `grinding-stone-01` and `log-rack-build-02`. The verifier reads those references, checks each exact set, then checks their prerequisite outcomes with **only the historical workbench built**. It does not build other structures to obtain a pass. Raw scene reference mapping is in `additions/forensics.json:serialized_dependencies`.

`Check` normalises a null detail to an empty string, so successful `CanLoad(out problem)` with `problem == null` cannot crash while logging success.

After the historical built-workbench case, the disposable runner survives `SceneManager.LoadScene("ForestTest", Single)`, reacquires the new scene components and checks the real scene's retired/unbuilt/hidden workbench and fresh Enlarged80 V2 identity. ForestTest is enabled in the inspected build settings. This runs the scene's actual `Awake` paths; the verifier does not impersonate them by assigning two workbench fields. After that proof, the disk comparison explicitly constructs the historical **rack-model-0 scratch fixture**, matching the earlier baseline comparison. `ResetWorld` is now documented as a scratch fixture retaining scene/player state, not a restarted game. Reset input logs record live pose and workbench state.

All original C–G paths remain: isolated disk Save/CanLoad/load/re-save, empty/null buildable lists, frozen Reference enter/exit at 0/20/50/100, Year-0/Year-3 resumed/uninterrupted comparison, and Legacy40 accounting/final return. Their runtime behaviour is NOT RUN here. The actual fresh V2 scene assertion and the model-0 comparison fixture are distinct claims.

### Tree substitution: unresolved, not explained away

Read-only comparison of original diagnostic run4 **and** run5 captures finds 1,344 tree records on each side, 1,343 shared identities with every shared record identical, **P0604 lost and P1209 gained**. The full records, positions, file hashes and paths are in `additions/forensics.json:tree_diagnostics`. `scenarioOne` also differs outside the tree list; the complete world-hash delta cannot be attributed solely to that tree.

`ForestStartingStand.CollectClearancePoints()` includes the live player position and each nonretired buildable. The fixed-count omission algorithm can substitute another hash-ranked tree when a clearance omission changes. The verifier's scratch reset neither restores player pose nor reloads authored scene state. Reference entry moves the player with its CharacterController disabled; return assigns the saved position with it enabled. This establishes plausible input sensitivity, **not the cause of the observed P0604/P1209 swap**. The old captures do not serialize player pose and old reset logs do not record it. A specific source cause cannot be proved offline from those records.

**BLOCKED:** exact causal attribution requires missing contemporaneous pose/clearance/state evidence or a separately authorised runtime investigation. New reset-input diagnostics assist that future investigation. The final Enlarged80 equality check is unchanged; production code, tree locations, omission rules and hashes are unchanged. If a production fault is established, stop for separate Manager authority.

### Save-schema meaning

The current-save assertions explicitly pin audited **v24**, rather than following any future schema automatically. The old string replacement that only recognised `"version":23` is removed.

Both standalone disposable verifiers contain the same independently tested `HistoricalLayout` method. It checks v24 input, requires false guided-review history, empty objective history and no prescription state/provenance, then removes the known post-target fields. Pre-v21 projection also requires an empty rack plan and ordinary rackId-0 orders; pre-v20 requires Legacy40 and removes geometry. Unsupported targets/current versions and meaningful newer state fail rather than being silently discarded. V21 projection retains V2 records and work-order links. These are **synthetic historical-layout controls**, never relabelled as genuine historical captures.

The new offline audit compares public schema fields against real historical Git sources: `4c40b0ea` (v19), `b0246334` (v20), `cf84d24c` (v21), and pinned production `4d95e452`; the implementation comments also identify the v22/v23/v24 introduction commits. It executes the actual extracted C# projection method, not a Python reimplementation. Boundary/removal details are in `additions/projection-v3/historical-schema-fields.json`.

Enlarged80 now separately exercises current v24 and projected v20, and constructs its v19-layout Legacy40 case by removing later fields instead of changing just the header. Rack persistence's alleged v21 Legacy40/progress fixtures now actually project v21 fields; current re-saves remain v24. Existing malformed-save refusal tests, legacy-rack progress, frozen-archive checks and coldstart mode remain. Coldstart additionally checks the persisted teaching flag. Added in-memory controls preserve true/false in v24, default absent history to false, and ignore a stray true flag on v23, following the existing production policy. A synthetic true flag tests persistence only and is not evidence that a player performed a review. No save-policy decision/change was needed.

### Runner and receipts

The CLI now requires the full expected HEAD, branch, owner, lock ID and terminal PASS token (task ID defaults to `287`). Before staging it checks actual Git identity, actual machine-wide held lease and absence of Editor PIDs, and rejects **all** tracked/untracked dirt. The same lease/branch/HEAD is checked after Editor exit. The workbench gate requires its exact `WORKBENCH_RETIREMENT_VERIFY_PASS` terminal token. Zero exit without that marker, any FAIL marker, compiler diagnostics, nonzero/timeout exit or identity mismatch fails closed.

Licence hashing walks regular files recursively, skipping directories/symlinks. A hash error cannot skip licence deletion. Harness/meta removal and licence deletion are attempted independently; cleanup failures force nonzero status, and both marker and JSON receipts are attempted regardless. Receipt-writing failure also forces nonzero exit. Receipts contain hash prefixes and error types, not licence bytes. Tests use entirely artificial licence bytes. Existing timeout, isolated config, disposable staging/removal and marker extraction are retained. The newest available entitlement file from the two existing local licence locations is selected, without reading it into evidence.

## Phase 0 attribution and protected-dirt proposal

Preservation was already done by the orchestrator and was **not repeated**. Reference: `/media/jer/Files/CCF/Verification/Workbench287MorningClosure-20261107/SHA256SUMS.txt`, SHA-256 **`9fb805968899cb6cd369a6d627ef1c01a7afece2368ff9d2093db1bfa9961f74`**. Read-only hash readback confirms all eight current files still match their preserved dirty bytes. Original README, dirty-backup, head-versions, patches and manifest were not edited.

Package changes: `com.unity.sdk.linux-arm64`, `com.unity.sdk.linux-x86_64`, `com.unity.toolchain.linux-x86_64-linux` are the three depth-0 entries, each 1.1.0 → 1.1.1. **`com.unity.sysroot.base` is the sole changed transitive version entry**, depth 1, also 1.1.0 → 1.1.1. All three direct dependency edges also change to sysroot 1.1.1. Read-only cache `package.json` files confirm all four resolved versions. This corroborates the preserved cold-import patch/timeline; cache contents alone would not identify the author of a manifest change. It does not settle package policy or validate tracked 1.1.0.

All seven material hunks are attributed in `additions/FORENSICS-APPENDIX.md`, supported by `additions/forensics.json`. Both leaf materials acquire their existing BaseMap texture as MainTex and change AlphaToMask 0→1; three other materials change their Color alias floats. Installed URP BaseShaderGUI 937–944, 1027–1034 and 1127–1129 implements the corresponding synchronisation. Every complete dirty material exactly matches an earlier recorded import hash in `Docs/Verification/StormsWindthrowIntegration/editor-normalization.json`, and every baseline exactly matches its restored hash. These are Editor property rewrites with potential rendering effects, **not formatting**, and no rendering-equivalence waiver is issued.

**Precise PROPOSED resolution for Manager only — NOT APPLIED:** after #319 and explicit Manager authorisation, restore the following eight paths to their tracked `4d95e452` bytes, retaining all dirty bytes in the existing preservation evidence. Do not stage them with this verifier publication. Before another Editor run, Manager must choose either exact tracked 1.1.0 resolution (stop on rewrite) or a separately authorised/reviewed 1.1.1 package update/requalification. Material restoration recovers baseline bytes, not proof of visual equivalence. Keep any desired local solution override outside shared repository settings. Do not switch protected-lane HEADs.

| Protected dirty path | Proposed disposition after Manager authority |
|---|---|
| `.vscode/settings.json` | Restore shared CCF.slnx setting/trailing-comma bytes; external workspace/user setting for lane-local solution name. |
| `Packages/manifest.json` | Restore tracked 1.1.0 declarations; any legitimate package update is separate scope. |
| `Packages/packages-lock.json` | Restore matching tracked lock; package policy/requalification remains open. |
| `Assets/ForestPrototype/Art/SectionFive/Bilberry_Type_Cover_01/Materials/Bilberry_Leaves.mat` | Restore tracked material; preserve texture/AlphaToMask rewrite evidence. |
| `Assets/ForestPrototype/Art/SectionFive/Bilberry_Type_Cover_01/Materials/Bilberry_Stems.mat` | Restore tracked Color values; preserve exact import evidence. |
| `Assets/ForestPrototype/Art/SectionFive/Woodland_Forbs_01/Materials/Forb_Flower_Centre.mat` | Restore tracked Color values; preserve exact import evidence. |
| `Assets/ForestPrototype/Art/SectionFive/Woodland_Forbs_01/Materials/Forb_Leaves.mat` | Restore tracked material; preserve texture/AlphaToMask rewrite evidence. |
| `Assets/ForestPrototype/Art/SectionFive/Woodland_Forbs_01/Materials/Forb_Stems.mat` | Restore tracked Color values; preserve exact import evidence. |

Both protected ZX20 lanes remain read-only. No preservation resolution was performed.

## Offline proof receipts and limitations

All paths below are relative to `/media/jer/Files/CCF/Verification/Workbench287MorningClosure-20261107/additions/`. Each test command ran from the new source worktree. `.stdout`, `.stderr`, `.exit` and `.command.json` record actual subprocess output/exit/arguments where listed. Unity itself was never run.

| Check / command | Actual result | Raw proof |
|---|---|---|
| `python3 -m py_compile Tools/Verification/WorkbenchRetirement/run_workbench_retirement.py Tools/Verification/WorkbenchRetirement/test_runner.py Tools/Verification/RackV2Persistence/test_historical_projection.py` with external PYCACHEPREFIX | **PASS exit 0** | `final-pycompile.*` |
| `python3 -B Tools/Verification/WorkbenchRetirement/test_runner.py` | **PASS exit 0; 13 tests** | `runner-mocks.*`; stdout includes every mocked runner JSON/returned exit. N1 missing-marker→1, N2 FAIL+PASS→1, N3 real nested mock directory→0, injected IsADirectoryError→1 with cleanup and receipts. |
| `python3 -B Tools/Verification/RackV2Persistence/test_historical_projection.py --out <additions>/projection-v3` | **PASS exit 0; schema audit + 28 C# controls** | `projection-v3.*`, nested `projection-v3/build.*`, `run.*` and schema JSON. Compiler invoked directly with installed .NET references; no package restore. |
| `python3 -B <additions>/compile_harnesses_v2.py` | **PASS exit 0; three changed C# harnesses, UNITY_EDITOR enabled** | `harness-compile-v2/command.json`, `compile.stdout`, `compile.stderr`, `compile.exit`. Existing Unity API deprecation warnings remain. This is Roslyn syntax/type checking against existing read-only DLLs, not an Editor compilation or behaviour gate. |
| `python3 -B <additions>/collect_forensics.py` | **PASS exit 0** | `forensics.json`, `FORENSICS-APPENDIX.md`, final captured `forensics-final.*` |
| `git diff --check` | **PASS exit 0** | `final-diff-check.*` |

Retained failed attempts: `projection.*` exit 1 (new audit parser mistakenly included an expression-bodied property as a field; corrected), `projection-v2.*` (dotnet build/restore stalled and its owned process was terminated; replaced with direct offline Roslyn invocation), `harness-compile/compile.*` exit 1 (duplicate monolithic/modular Editor references in the temporary compiler command; corrected by using modular references). These are test-tool setup failures, not Unity runs or evidence of passing runtime behaviour.

No source assertion was removed to obtain the passing offline results. Roslyn used the existing dirty lane's compiled dependencies; it cannot qualify the package discrepancy. Historical-layout controls do not replace genuine archived-save/controller tests. Actual scene reload, all C–G behaviour, native site witness, rendered material comparison, full v2/Legacy40/coldstart persistence and exact future clean-lane/package execution remain **NOT RUN**.

## Exact future Unity/native protocol — NOT RUN, separately authorised operator required

1. Manager first resolves package/dirt policy, designates one clean registered Unity lane and exact reviewed candidate, then grants the ordinary lease. Do not run in either currently protected dirty checkout. Use existing isolated save/config tooling, never a user save. Record actual branch/HEAD/package resolutions/lease and staged-source hashes. Stage/remove each disposable harness and its generated meta under the documented workflow.
2. For the lifecycle runner, supply `WorkbenchRetirementVerification`, its candidate source, a fresh Files verification output, `--expected-head <full reviewed SHA> --expected-branch <actual branch> --owner <lease owner> --lock-id <actual lock ID> --pass-token WORKBENCH_RETIREMENT_VERIFY_PASS`. Expected: both exact dependency sets, genuine scene Awake checks and all C–G assertions pass with terminal marker and exit 0; otherwise stop and retain FAIL/receipts. For other gates supply their actual terminal token; do not substitute an intermediate PASS. Run Rack persistence v2, separate Legacy40 (`CCF_STAND_GEOMETRY=0`), then a separate coldstart process with the exported partial save and expectation file via the existing dedicated runner/slot-backup protocol. Verify no disposable source/meta survives.
3. Native witness at **1280×720 and 1920×1080**, using real keyboard/mouse delivery and the new-game start: follow the central track to the former site **(−2.75, 0, 11)**. Record approach plus coordinates/resolution. Aim at the old site centre and former frame/collider extents from north/south/east/west at 4.1 m, 3.5 m, 2 m and close range. Dwell several frames at each aim; record unoccluded crosshair/HUD and absence of frame, Build/Requires prompt or selection response. Use E and left mouse while aiming into the empty old-site volume; record unchanged build state and wood/planks. Choose unobstructed aims so unrelated tree actions do not contaminate the observation.
4. Walk through the former root/collider volume from both axes, reverse and strafe across it. Record a native continuous witness, start/end positions and no collision/stall. Read-only renderer/collider counts supplement this but do not replace native movement. Record any other collider encountered by identity rather than attributing it to the retired workbench.
5. Repeat former-site aim/use/traversal after new-game disk save/load, after a historical built-workbench save then **real new-game scene reload**, and after frozen Reference enter/exit. Use an isolated historical built-workbench control to demonstrate that the same site is visible/solid when the saved entry requires it; confirm the new-game state restores absence. At each size retain raw screenshots/video, input/event trace, logs, terminal marker and exit. A source or synthetic-input test alone is not this witness.

**Year-3 claim narrowed:** this repair retains the existing retired-world continuous-versus-resumed three-year comparison and context-labelled diagnostic hashes. Only the existing same-capture stripped-record **Year-0** check proves a buildable-entry-only hash delta. There is no new independent paired stripped-record Year-3 demonstration; none is claimed. Newer/older Year-0/Year-3 hashes are configuration-dependent diagnostics, not accepted baselines. No frozen hash or fixture was changed.

## Handoff / remaining blockers

B3 is **BLOCKED unresolved** for the exact evidence gap above. Package-policy resolution and protected-dirt disposition belong to Manager; rendered equivalence/native witness and all Unity-dependent gates remain open. #319 must independently review this unqualified source publication before repinning any runtime candidate. #287 is not qualified or accepted. No PR was created, edited or touched, including #300; no main merge or force-push is authorised/performed.

Scope deviation: **none**. Retaining B3 as BLOCKED and M2/runtime as NOT RUN follows the packet's explicit stop conditions. Year-3 proof is explicitly narrowed as permitted. Source publication alone does not resolve those gates.

DASHBOARD SYNC: **NOT RUN**; operational coordination/publication remains with the orchestrator/Manager under this sole-source-writer packet. PUBLISHED DASHBOARD REFLECTS THIS TASK: **UNVERIFIED**. No generated dashboard or active-task record was edited.
