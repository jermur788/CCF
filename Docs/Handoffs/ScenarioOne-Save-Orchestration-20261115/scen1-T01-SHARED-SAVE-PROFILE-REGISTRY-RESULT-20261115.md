# Scenario One shared-save T-01 — fixed profile registry result

**IMPLEMENTED ON TASK BRANCH — OFFLINE HANDOFF, NOT UNITY-VERIFIED.** No approval, integration, runtime compatibility or whole-feature conformance claim.

## Purpose and authority

Implement only signed T-01's fixed three-profile routing/availability and early selected-context seam, preserving the historical slot. New-profile envelope/evidence classes are **data definitions only**, as expressly included in the direct source-worker dispatch. T-02 admission, T-03 staged lifecycle/prefs/preview, T-04 writer/recovery and T-05 qualification remain separate unreleased stages.

- Claim: [#549](https://github.com/jermur788/CCF/issues/549); parent [#544](https://github.com/jermur788/CCF/issues/544); Manager [#424](https://github.com/jermur788/CCF/issues/424).
- Actual worker: **OpenAI / GPT-6.1 Sol / HIGH**, Codex harness in T3 Code; sole source writer, no delegated agents.
- [Binding Manager ruling](https://github.com/jermur788/CCF/blob/docs/scen1-manager-424-save-t01-ownership-ruling-20261115-v1/Docs/Handoffs/ScenarioOne-Save-Orchestration-20261115/scen1-MANAGER-T00-ACCEPTANCE-T01-BASE-OWNERSHIP-OPS-RULING-20261115.md), read in full after reading #549 first.
- Signed four-file contract: `200add4e6e5116268b79ef63131ff9b836f14c5d`, `specs/scenario-independent-save-isolation-v1/`; external signature/ruling governs its preserved pre-signature banners. No banner/spec/checklist edits.
- T-00 ceiling inspected at `f86e3010b338d627fb8be9272549442e27dd8368`, report §3. This worker uses the narrower list below.
- [Actual pre-edit RUNNING registration](https://github.com/jermur788/CCF/issues/549#issuecomment-6897225127), including identity, base, board, worktree and exact allowlist.
- This result filename follows the latest direct dispatch (`SHARED-SAVE-PROFILE-REGISTRY`), superseding the earlier issue-body proposed `FIXED-PROFILE-REGISTRY` spelling. There is one result document.

## Exact base, context and physical preflight

- Branch: `feature/scen1-shared-three-profile-save-v2`.
- Isolated worktree: `/media/jer/Files/CCF/Operations/CCF-Scen1-SharedSave-T01-20261115`.
- Starting full HEAD / verified remote main: **`f52dec246d8e68608150e60f7e26f3104a38a836`**.
- Audited source baseline: `fc18190b976f1363f1b5d1b0881810db2f8c6e39`.
- Canonical context commit: `62e39ec621e38a84030c08b35a171e297056c9ed`; the live coordination board is read separately.

Before creation, the proposed worktree path did not exist and `git branch --list feature/scen1-shared-three-profile-save-v2` returned no branch. The branch was created from `origin/main` using the exact supplied `git worktree add -b ...` command. After checkout finished, `git status --porcelain=v1` was empty and HEAD matched the pin. No root-hosted worktree was created.

Commands actually executed in `/media/jer/Files/CCF/Repository.git`, then repeated from the task worktree before handoff preparation:

```bash
git fetch origin 'refs/heads/*:refs/remotes/origin/*'
git rev-parse origin/main
git diff --name-only fc18190b976f1363f1b5d1b0881810db2f8c6e39 origin/main
```

Actual output (fetch had no output):

```text
f52dec246d8e68608150e60f7e26f3104a38a836
Docs/Project/coordination/active-tasks.md
```

Thus exactly one coordination file changed and zero Assets files changed. No repin stop was triggered. Live board's top Manager entry expressly reserves the save subsystem to one #544/#549 writer and freezes overlapping #443 writes; #443's latest Manager comment independently agrees HOLD/no new overlapping writes. Registered worktrees and process names were inspected read-only; no Unity/CCF-Ops execution process was observed. This is **not** a Unity lease, and physical lease state was not accessed.

Files had 132G available; root had 294M available / 100% usage. Source, scaffold, compilation output and evidence stayed on Files. Primary's unrelated modified `AGENTS.md` and `Docs/Operations/StorageArchitecture.md` were observed and preserved. No files were edited in Primary or the parent orchestration worktree.

## Narrower allowlist actually used

All nine paths below were declared before source edits; no other repository path is changed:

| Path | Change |
| --- | --- |
| `Assets/ForestPrototype/ForestSaveProfiles.cs` | New immutable fixed records, strict lookup, availability, exact paths/sidecars and preference namespace constants |
| `Assets/ForestPrototype/ForestSaveProfiles.cs.meta` | New GUID `7d0a0235f8e147ee8c48dcbc6a55c8ac` |
| `Assets/ForestPrototype/ForestSaveEnvelope.cs` | New data-only envelope/identity/compact evidence classes |
| `Assets/ForestPrototype/ForestSaveEnvelope.cs.meta` | New GUID `de9f7f8408de4a8fb9597a8ef284c52d` |
| `Assets/ForestPrototype/ForestSaveSessionContext.cs` | New early selection, availability check and seal/bootstrap seam |
| `Assets/ForestPrototype/ForestSaveSessionContext.cs.meta` | New GUID `0001aaf039f242d6948a022c03c85b35` |
| `Assets/ForestPrototype/ForestSaveController.cs` | Remove local filename constant; only three path-property substitutions |
| `Assets/ForestPrototype/UI/StandaloneSessionMenu.cs` | Only its SavePath property uses the same query |
| This result document | Evidence, limits and handoff |

## Implemented behavior and boundaries

`ForestSaveProfiles` contains exactly three compile-time records and two chapter IDs. Lookup is ordinal/case-sensitive and has no default/fallback. Records and the exposed list are read-only; operations/path queries reject caller-created lookalike records, even if they claim availability. Neither labels nor input strings become filenames.

| Profile | Chapter | World / geometry / content | Primary under Application.persistentDataPath | Availability |
| --- | --- | --- | --- | --- |
| `o1-historical` | `scenario-one` | No new persisted identity; existing historical definitions and saved geometry remain authoritative | `forest-save.json` | Existing historical world only |
| `o1-compact-v1` | `scenario-one` | `scenario-one-compact-v1` / `compact30-v1` / `scen1-compact-h1-g1-v1` | `forest-save-scenario-one-compact-v1.json` | UNAVAILABLE: no signed authored world binding installed |
| `o2-working-mixed-v1` | `scenario-two` | `scenario-two-working-mixed-v1` / `o2-working-mixed-geometry-v1` / `o2-working-mixed-content-v1` | `forest-save-scenario-two.json` | UNAVAILABLE: no signed authored world binding installed |

Sidecars are each primary plus `.tmp` and `.bak`. The pure path query exposes reservations for inspection without authorizing operations. There is no runtime binding setter, registration API, geometry support addition, dimension, world content or placeholder. Reserved profiles cannot be selected, begin a destination context, or acquire an operational selected path. No new-profile Start/Continue/Load/Save UI route is offered; the existing UI still addresses historical O1 only. Existing typed loading remains a later admission task, not a new-profile route implemented here.

Both existing filename consumers now use **`ForestSaveSessionContext.CurrentPaths.Primary`**. Controller sidecars use the same query's `Temporary`/`Backup`. The compiled query is asserted equal to `Path.Combine(Application.persistentDataPath, "forest-save.json")` with an explicitly stubbed Application directory; actual Unity persistentDataPath was not read. No file is probed, created, renamed, moved, copied, migrated or loaded by selection/path resolution, and no save/sidecar file was accessed in this task.

A new context has no implicit fallback. Initial bootstrap selects historical O1 only because it is the sole available record, without starting gameplay. `SubsystemRegistration` resets the initial context (including domain-reload-disabled sessions); `BeforeSceneLoad` seals its identity before scene Awake/generation. After `TryBeginWorldCreation`, all selections, including reselection of the same ID, refuse. A refused choice preserves the prior choice; it does not relabel that world as the requested profile. A later T-03 staged destination can use a separate context and seal it before generation. **This is a selection seam, not an implementation of staging, admission or success-only active-world publication.** If several profiles are eventually bound, the current initial selector intentionally leaves selection empty and bootstrap refuses; T-03 must supply the explicit-choice workflow before enabling them. There is no automatic start or menu presentation-order decision.

`ForestSaveEnvelope` declares only the three disjoint root fields `ccfSaveEnvelope`, `ccfWorldPayload`, `ccfCompactTeaching`. Its header has the exact seven fields and signed schema1/payload24 constants. Evidence has the exact six fields, planting rows the exact four fields, and uses `List<int>` for groupOrderIds. O2's teaching field is represented by null. **No serialization route, raw validation, duplicate-key handling, admission, identity rejection, work evidence validation or teaching trigger is wired.** Deserialized defaults are not admission evidence. The preserved DTO is referenced as a nested payload type and is not changed.

SI-09 constants exactly preserve `CCF.Learning.v1.` / `CCF.MenuHelp.v1.` and reserve compact `CCF.Learning.scen1-compact-v1.` / `CCF.MenuHelp.scen1-compact-v1.` and O2 `CCF.Learning.scenario-two-v1.` / `CCF.MenuHelp.scenario-two-v1.`. The additive global `CCF.UI.ReduceMotion` key is inventoried outside these namespaces. Current preference consumers/reset behavior are byte-unchanged; active-prefix wiring belongs to T-03. The sentinel tests namespace separation with an in-memory dictionary, not real PlayerPrefs.

## Offline checks actually run

Evidence root (outside Git): **`/media/jer/Files/CCF/Verification/SharedSaveT01-549-20261115/`**. Contains `Harness.csproj`, `global.json`, `NuGet.Config`, `Program.cs`, `UnityStubs.cs`, logs and `preservation_check.py`; SDK cache/tmp/build artifacts are confined there. No package/dependency was added to CCF. NuGet package sources were cleared in the external scaffold; Roslyn references use installed SDK assemblies.

Tool discovery actually ran:

```bash
command -v dotnet csc mcs mono python3
dotnet --list-sdks
```

Actual output:

```text
/usr/bin/dotnet
/home/jer/.pyenv/shims/python3
8.0.133 [/usr/lib/dotnet/sdk]
10.0.114 [/usr/lib/dotnet/sdk]
```

No standalone csc/mcs/mono executable was found. The external harness pins SDK 8.0.133, targets net8.0, and treats warnings as errors. It compiles the **actual three new helper source files**, using tiny stubs for Unity's Application and runtime-initializer attributes, plus an empty ForestSaveData type stub solely for the envelope field's type reference. The actual historical DTO is read by Roslyn for root disjointness, not compiled in this scaffold. The whole project/controller/menu are **not type-checked against Unity**; their changed files are parsed for C#9 syntax in both Editor and Player preprocessor modes. Callback methods are invoked via reflection with stubbed attributes, not by the engine lifecycle.

Executed from the evidence root:

```bash
env DOTNET_CLI_HOME=/media/jer/Files/CCF/Verification/SharedSaveT01-549-20261115/cli NUGET_PACKAGES=/media/jer/Files/CCF/Verification/SharedSaveT01-549-20261115/nuget TMPDIR=/media/jer/Files/CCF/Verification/SharedSaveT01-549-20261115/tmp DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1 DOTNET_NOLOGO=1 dotnet run --project Harness.csproj --configuration Release > offline-run.log 2>&1
cat offline-run.log
```

Actual output:

```text
PASS fixed table + nine paths + original slot exactness: 50 assertions
PASS negative IDs, unbound worlds, forged records, no fallback: 66 assertions
PASS all eight virtual primary-presence combinations: 64 assertions
PASS selection seal + actual bootstrap callback methods: 26 assertions
PASS SI-09 namespaces + ReduceMotion sentinel: 52 assertions
PASS data-only envelope field names/types + disjoint root: 35 assertions
PASS C#9 syntax (Editor/Player) + shared callers + no IO: 17 assertions
PASS TOTAL: 310 assertions; no Unity, host save, real preferences or runtime load exercised
```

Negative cases executed include null/blank/chapter-only/display/traversal/case-variant/trailing-space IDs; unselected/unavailable contexts; caller-forged available records; invalid path roots; duplicate world-creation entry; and every post-seal selection including same-ID/null. All eight primary-presence combinations use virtual sets, proving independent path visibility and that another slot never changes the historical query or enables an unbound world. They do **not** exercise disk corruption, CanLoad validation or actual UI interaction. The preference sentinel models enumerated-key removal and preserves all other profiles, an unrelated key, and global ReduceMotion; it does **not** claim actual reset code has been switched to profile namespaces.

Executed from the same evidence root after report preparation:

```bash
python3 preservation_check.py > preservation-check.log 2>&1
cat preservation-check.log
```

Actual output:

```text
PASS exact base f52dec246d8e68608150e60f7e26f3104a38a836; audit delta is coordination file only
PASS Assets/ForestPrototype/ForestSaveController.cs: ONLY declared filename/path substitutions; every method byte-preserved
PASS Assets/ForestPrototype/UI/StandaloneSessionMenu.cs: ONLY declared filename/path substitutions; every method byte-preserved
PASS unchanged Assets/ForestPrototype/ForestSaveData.cs: sha256 093cbea99aae9844d2cd3b0cda0260a16d5478a4206e0bf6534838eafa3b3d0e
PASS unchanged Assets/ForestPrototype/ScenarioOne/ScenarioReferenceArchive.cs: sha256 730e9529e4ee925c5b64ee3d67bc23da1ef0e3847ea23661c148feb80f9bf21a
PASS unchanged Assets/ForestPrototype/ScenarioOne/Resources/ScenarioOneReferenceFutureV1.bytes: sha256 789e3eddcf00d3d4d0fd18d078f7ab37a06d44aa9c8672cff564445b2bb041fc
PASS unchanged Assets/ForestPrototype/StandGeometryModel.cs: sha256 603781ae9603b05beb9df07ed3d9ba404aa955c57464cbeaf844b1e7e5cb43f2
PASS unchanged Assets/ForestPrototype/ForestStartingStand.cs: sha256 bc305a2f92f9ad68ef56699ee49423d12f3650165a17fd6d62fbfba3c7e9190b
PASS unchanged Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.cs: sha256 2e414b02652517664399422b9941313fb03f34dfe800c9dbba7efc87f1575f90
PASS unchanged Assets/ForestPrototype/UI/StandaloneSessionMenu.Startup.cs: sha256 8ae0b413928372f1dd083d7a055f5431c041f565d359809d4bbf6b8855fdad36
PASS unchanged Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs: sha256 8592bc01058b5c3b9cadc8689da15b86333d1aaa54441066b3f98bafa410d466
PASS unchanged Assets/ForestPrototype/UI/MenuHelpView.cs: sha256 97d1682d488bc6847880649debd9180b7a31a5dc1613af37cee7760e8dc45c32
PASS unchanged Docs/Project/coordination/active-tasks.md: sha256 e833056ea11b5b41ab00ba8a04a6125be12649a5fbc2fc9287dede92efec51da
PASS frozen Reference bytes match signed SHA-256
PASS immutable contract spec.md: blob 8ac551022adb578abed0d9fbd413c8e9d002c3f9
PASS immutable contract plan.md: blob 4ba91d909a3b8ddbb15c88a8e011f8b6d3720b50
PASS immutable contract tasks.md: blob 88052dc58a9f3128131ec701d39a92c1500caf2b
PASS immutable contract checklists/requirements.md: blob 505ab550a2b5dbdd65ce9dafb1f24c437828d2fb
PASS all 63 signed ISO boxes remain unchecked; no spec copy/edit on task branch
PASS actual tracked/untracked edits inside narrower allowlist (9 paths at check time)
PASS three fresh unique new .meta GUIDs; all existing metadata unchanged
PASS no generated Unity/cache directories; git diff --check clean
PASS preservation/scope checks complete; no save file or physical Unity lease state accessed
```

The initial preservation run passed before staging (8 source/metadata paths). The precommit rerun exposed a checker bookkeeping assumption: its `git diff BASE -- Assets/**/*.meta` assertion expected an empty diff, so it also rejected the three newly staged, allowed metadata files. That rerun failed with `AssertionError` at `assert existing_meta_diff == b''`; full output is retained in `preservation-check-precommit-failed.log`. The external checker was corrected to allow **only the exact three declared new metadata paths**, still checking each GUID against every existing asset and rejecting any existing-metadata edit. The final run above passed with all nine paths. No production source was changed in response.

The preservation check compares each entire existing changed source against the base with only the explicit filename/path substitutions applied. Thus all method bytes (including save/load/capture/preview/writer behavior and startup/session UI actions) are preserved. It also checks the complete actual change set, new GUID uniqueness against all existing asset metadata, protected source/Reference hashes, unchanged live board, four signed blob IDs, 63 unchecked ISO boxes and absence of generated Unity directories. No historical save fixture is opened or hashed; source/Reference preservation is not historical-save runtime equivalence.

External scaffold/log SHA-256 manifest (after the final successful check):

```text
db8ab440cb54228f4584d2355732738c48dfed92d43904aa807a24201416209a  Harness.csproj
d943b0764839bf02262ac81a67343245d5d5993b7676d78650e2bb7d8a9687ac  global.json
9508cae5f76364f56e80eb97cb16493977243cf38e5cca6284e3d74f83b20a2c  NuGet.Config
f59aac5f6ec2b64db307a82a8ce7dc93e0a642c3fa5216d35b15deb9344bc855  Program.cs
4896cf8554c0eb065b753cc3a30d20476eed56e2b59b95087fb5e304a2a724c2  UnityStubs.cs
9badff31498dba95906f4cc302a822b9454bc45c5ff0a1cf0b7c51cee90f42e9  preservation_check.py
002e3329515f7ee9fb43684a878d87f31b928e53e4986ce347bbf0e26d2b5dec  offline-run.log
dc85790d59e6aceec6effbac8c9b6cd2009fd009f306db112ebf76b6659d2451  preservation-check.log
08b69faefadfa04a2ac7c850928122e0e7a068268858369e95acc149d26912db  preservation-check-precommit-failed.log
```

## Deliberately untouched

- `ForestSaveData.cs`, `ScenarioReferenceArchive.cs`, frozen archive/schedule/hash helpers; historical serializer, validators/defaults, geometry0/1, five-rack behavior, ScenarioOneManager and ForestStartingStand.
- All #443 candidate hunks, absent-buildable retirement, annual-review changes, preview-pose repair and UI ReduceMotion implementation; no merge/rebase/cherry-pick/copy from acb4e111.
- T-02 admission/foreign-identity preflight/typed-route closure/pure capture; T-03 staged apply/rollback/preview/prefs/geometry/teaching; T-04 safe writer, recovery, File.Replace/copy fallback. Existing known later-stage gaps remain; this task does not close them.
- Compact dimensions/cells/H1/path/G1, CG-TRIG/ISO-58, numeric geometry support, O2 content/definition and scenario completion. Both authored-world dependencies remain unavailable.
- Signed specification blobs/checklist, all real saves/sidecars, PlayerPrefs/EditorPrefs, scene/prefab/ScriptableObject assets, Packages/ProjectSettings, existing .meta files and generated directories.
- Unity/Editor/Player, physical Unity lease directory, CCF-Ops, protected worktrees, main, active-tasks, Decision Log, milestone, dashboard and throughput ledger.

## Untested and next required gates

No Unity executable was launched, no Unity compilation/import occurred, and no Player/build/native input/rendered review or actual save/load/backup/preview/persistent-world test ran. Focused .NET compile/execution is real, but uses explicit boundary stubs and does not establish Unity integration. JsonUtility serialization/default behavior and exact engine callback ordering are unverified. No genuine historical-fixture same-state/byte equivalence test, full corpus, IO fault/crash test, or all-ISO conformance claim is made. All 63 checklist items remain unchecked.

Independent exact-head source review and Manager disposition are required before any T-02 release or integration. A separately authorized isolated Unity lane/lease and candidate packet must later check actual startup initialization/domain-reload behavior and unchanged historical interaction. Suggested manual observations for that future packet, **not executed here**:

1. Open the exact candidate with Unity 6000.6.0f1 under isolated disposable save/prefs directories; enter Play normally, then with domain reload disabled. Confirm the historical startup menu still opens, with no compact/O2 playable route or automatic gameplay start.
2. Start historical O1, move/interact, use F10 and return. Confirm the same historical profile/path remains selected across scene restart; verify TrySelect refuses after the BeforeSceneLoad boundary through the assigned harness.
3. In that isolated profile only, use F5/F9 and session-menu Save/Continue. Confirm immediate F5, existing replacement confirmation, and historical primary/.tmp/.bak locations; resource/world changes must persist after save/load. Recheck Reference entry/return through the later authorized qualification packet.

These steps require future runtime evidence and must not be run against user saves under this offline claim. No new harness was copied into Assets; none needs disposable-script removal.

**Dashboard / published dashboard / Drive synchronization: NOT VERIFIED.** Their owned records were not edited; #544/#424 must reconcile downstream status. #549 remains open for the Manager.

## Diff summary and handoff assessment

Source/metadata diff before this report: **8 files, 276 insertions, 6 deletions**. Three new C# helpers total 266 lines; three new metadata files total 6 lines. The only existing C# edits are removal of one controller filename constant, its three path properties, and one menu path property. This report is the ninth changed path. No serialized scene/prefab/package/project-setting file changed.

Worker assessment: **ready for independent source review of bounded T-01**, with Unity integration and later-stage requirements explicitly unverified. Implementation is committed/pushed on the task branch; the exact final commit SHA and immutable report URL are recorded in the #549 HANDED-OFF comment rather than embedding a self-referential commit hash here. This is not APPROVED, VERIFIED or INTEGRATED.
