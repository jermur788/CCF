# D-073 #414 — cadence/readout base reconciliation result

Child of [#412](https://github.com/jermur788/CCF/issues/412); worker claim [#414](https://github.com/jermur788/CCF/issues/414). Tasks T-01/T-02 only. **Implemented on the assigned branch; offline gates passed. Unity NOT RUN.** No Manager integration/acceptance verdict is claimed.

## Worker and exact state

- Actual worker: Codex, OpenAI `gpt-6.1-sol`, high reasoning effort, through the T3 Code Codex harness. Work done directly; no subagents.
- Sole writable worktree: `/media/jer/Files/CCF/Worktrees/CCF-D073-AnnualReview-V1`.
- Branch: `integration/scenario-one-d073-cadence-readout-base-20261112-v1`.
- Starting HEAD / qualified cadence base: `e759193354b4613f88e3370a62ccaccf02c556cf`.
- Context snapshot on that base: `63044dfbbeb869996ceecec8a47619dcba048b0b`; live GitHub coordination and issues #412/#414 inspected read-only. The latest direct #414 packet bounds this worker's writes.
- Final implementation HEAD: `d86a68a18208bab6939bee9e41aa916d6f998fa7`. Its sole parent is the starting HEAD. The handoff-only successor contains this report; no merge or cherry-pick was used.
- Base tree: `00c736b933c09e3c25ac347d13657e5372821007`.
- New combined implementation tree: `c63d127fa51c1da15c53d2d1f38fd846f5823cef`.
- Final publication HEAD/tree: the commit containing this report and its tree, resolved after this handoff-only commit by `git rev-parse HEAD` and `git rev-parse 'HEAD^{tree}'`. Full immutable publication SHA/tree are recorded in external `final-audit.json` and `publication.json`, and in the #414 publication comment. A committed report cannot embed its own commit/tree hash without changing those hashes; the implementation SHA/tree above are literal, independently reviewable values.
- External evidence root: `/media/jer/Files/CCF/Verification/D073-BaseReconciliation-414-20261112/`.

## Change and file identity

Copied the helper and exactly five tooling files from `0ce4793f2a2a600ea01bbcf3e9800a2ac39293f4` using `git show <sha>:<path>` redirected to each destination. All six copied blobs match that commit. No source branch commit was merged/cherry-picked. No fetch was needed: the object was already present.

Created only the helper's `.meta`, mirroring `ScenarioManagementCadence.cs.meta` byte-for-byte except its GUID. Both files contain exactly `fileFormatVersion` and `guid`, with the same newline format. Fresh UUID-derived lowercase GUID: `bd8034988bda43aeb0e153015a3715eb`. Before creation `git grep` returned 1 with no match; after staging the only repository match was the new `.meta`. The final asset-only GUID search must still have one match; this handoff intentionally also records the GUID as text.

| Added file | Git blob |
|---|---|
| `Assets/ForestPrototype/ScenarioOne/ScenarioOneManagementFollowonReadout.cs` | `2c7bdb3c6b6f13790a325ebabc187c497b335b52` |
| `Tools/Verification/ManagementFollowonReadout/ManagementFollowonReadoutVerification.cs` | `5b6b0259423f26b55446868220e6c5b3f523b9f5` |
| `Tools/Verification/ManagementFollowonReadout/OfflineContractTypes.cs` | `a957bf822ee513765f0478d8ef4f3f3bad4d3294` |
| `Tools/Verification/ManagementFollowonReadout/README.md` | `04b2d18f1dcf82867ef204e400314479cf253b43` |
| `Tools/Verification/ManagementFollowonReadout/run_mutations.py` | `5ce5d861c24c268e177b3b1b3cb1c7a2b0a389e7` |
| `Tools/Verification/ManagementFollowonReadout/run_offline.py` | `59d73b7ea0ebac5afcf56246951335ee0fcbfe77` |
| `Assets/ForestPrototype/ScenarioOne/ScenarioOneManagementFollowonReadout.cs.meta` | `beff2f7991470741ee63d12fabe5d809020a59db` |

The only eighth changed path is this result handoff. Existing baseline files are unchanged, including all save, achievement, objective, event and hash sources; the cadence source itself is unchanged. No scene, prefab, material, ProjectSettings, package/dependency file, frozen Reference Future, spec, imported Evidence406 directory or source-branch handoff was copied/edited. The seven additions and this report are the full candidate-relative scope. No UI call site or player workflow was added.

## Executed gates

| Gate | Actual result | Python/wrapper exit | Underlying dotnet exit |
|---|---|---:|---:|
| Unchanged offline suite, exact requested command | PASS **53/53** focused assertions | 0 | explicit build 0; run 0 |
| Real production-signature syntax/typecheck | PASS **191 sources / 116 references / 0 errors** | 0 | build 0 |
| Blob identity, metadata format and unique asset GUID | PASS | audit 0 | n/a |
| Scope, unchanged existing sources, first-parent ancestry, no merges | PASS on implementation commit; final handoff-only audit recorded after commit | audit 0 | n/a |
| Worktree clean; no `__pycache__` or `.pyc` | PASS after implementation commit; repeated after handoff commit in final receipt | audit 0 | n/a |
| Optional mutation checks | **NOT RUN** | n/a | n/a |

Read `README.md`, `run_offline.py`, `run_mutations.py`, the fixture/adapter and `compile_runtime_sources.py` before execution. Commands ran from the assigned worktree root:

```sh
python3 -I -B Tools/Verification/ManagementFollowonReadout/run_offline.py
python3 -I -B Tools/Verification/ScenarioOneNonUi/compile_runtime_sources.py --src /media/jer/Files/CCF/Worktrees/CCF-D073-AnnualReview-V1 --out /media/jer/Files/CCF/Verification/D073-BaseReconciliation-414-20261112/typecheck-production
```

The first uses the real helper plus the copied minimal test adapter and fixture, targeting net8.0/C# 9. Its observed output was `ManagementFollowonReadout: PASS 53 focused assertions (when actually run).` This was actual execution, not interpretation of the source's expected message. Synthetic legacy cases do not load a genuine old save.

The second automatically includes the helper in Assets with the real cadence-branch production classes. Its generated project contains 191 runtime sources, no `OfflineContractTypes.cs`, and 116 references, targeting netstandard2.1/C# 9. Observed output: `sources=191 refs=116 exit=0 errors=0`. **This is a .NET production-signature typecheck, not a Unity compile.** The script's Python exit alone is insufficient because it does not propagate the build return code; the separate captured real dotnet build exit is 0.

Managed references used the unchanged script's existing default `/home/jer/ccf-verify-player/CCFVerify_Data/Managed`, also used by the #406 source evidence commands. No Player was built. `typecheck-reference-manifest.json` records each reference path and SHA-256, and `typecheck-source-list.txt` records every source. Unity 6000.6.0f1 assembly paths were found read-only under `/media/jer/ZX20/Unity/6000.6.0f1`; no Editor was launched, no install/reference/library was modified. SDK observed: .NET 10.0.114; net8 runtime present.

To retain real subprocess evidence without changing the imported scripts, external `audit-bin/dotnet` forwards calls to `/usr/bin/dotnet`, records stdout/stderr/argv/cwd/exit, and for the offline invocation first runs a separate explicit `dotnet build <generated-project> -c Release -v minimal`, then forwards the runner's original `dotnet run --project <generated-project> -c Release` unchanged. That explicit build had 0 warnings and 0 errors. A preliminary build failure would have been returned as failure. The production gate's original `dotnet build -nologo -v q -c Release '-clp:NoSummary;ErrorsOnly'` is forwarded unchanged. All audit Python runs use `python3 -I -B`. Exact selected environment is in `offline-environment.json` / `typecheck-environment.json`; `TMPDIR` is under the external evidence root. The unchanged offline script deletes its temporary project after running; its project text snapshot and process logs are retained externally. No fixture/assertion/helper source was modified for instrumentation.

## Evidence and command exits

- Offline build: `offline-dotnet/prebuild.{json,stdout.log,stderr.log}`.
- Offline run: `offline-dotnet/dotnet-run.{json,stdout.log,stderr.log}`; wrapper: `commands/offline.*`.
- Production build: `typecheck-dotnet/dotnet-build.{json,stdout.log,stderr.log}`; wrapper: `commands/typecheck.*`.
- Import/hash/initial GUID checks: `import-commands.json`; seven blobs: `file-blobs.json`.
- Implementation ancestry, scope, metadata/cache/cleanliness checks: `payload-audit.json`, `commands/payload-*`.
- Final handoff-only ancestry, exact `git diff --name-status e7591933 HEAD`, base/new final tree and clean status: `final-audit.json`, `commands/final-*` after this documentation commit.
- Final publication commands and their real exits: `commands/commit-handoff.*`, `commands/push.*`, `commands/remote-head.*`, `commands/issue-comment.*`, and `publication.json` after the handoff commit. These post-commit operations cannot be represented as already-run commands inside their own immutable handoff. The #414 result comment publishes the final exact SHA/tree, push/readback and gate outcomes; the issue stays open.

All paths above are relative to the external evidence root. Captured JSON contains command, cwd and real exit; stdout/stderr are separate. The command ledger through implementation verification follows. `git grep` exit 1 on the initial GUID search is its expected no-match outcome.

| Receipt / purpose | Command | Exit | Outcome |
|---|---|---:|---|
| `base-tree` | `git rev-parse 'HEAD^{tree}'` | 0 | PASS |
| `commit-payload` | `git commit -m 'integration: add #406 readout helper byte-identical onto qualified cadence base' -m 'Copy only the approved helper and five offline tooling files from 0ce4793f2a2a600ea01bbcf3e9800a2ac39293f4; add matching script metadata. Offline readout 53/53 and runtime-source typecheck 191 sources / zero errors passed without source changes.' -m 'Co-Authored-By: Codex <noreply@openai.com>'` | 0 | PASS |
| `context-commit` | `git log -1 '--format=%H %s' -- Docs/Project/AgentWorkflow.md Docs/Project/decision-log.md Docs/Project/current-milestone.md Docs/Project/game-brief.md Docs/Project/unity-project-overview.md Docs/Project/instructions AGENTS.md CLAUDE.md Tools/ProjectContext/generate_project_instructions.py` | 0 | PASS |
| `diff-check` | `git diff --cached --check` | 0 | PASS |
| `dotnet-info` | `dotnet --info` | 0 | PASS |
| `guid-search-staged` | `git grep -n bd8034988bda43aeb0e153015a3715eb` | 0 | PASS |
| `issue-412` | `gh api repos/jermur788/CCF/issues/412 --jq .body` | 0 | PASS |
| `live-active-tasks` | `gh api repos/jermur788/CCF/contents/Docs/Project/coordination/active-tasks.md -H 'Accept: application/vnd.github.raw+json'` | 0 | PASS |
| `managed-path` | `readlink -f /home/jer/ccf-verify-player/CCFVerify_Data/Managed` | 0 | PASS |
| `offline` | `python3 -I -B Tools/Verification/ManagementFollowonReadout/run_offline.py` | 0 | PASS |
| `payload-ancestry` | `git rev-list --first-parent --parents e759193354b4613f88e3370a62ccaccf02c556cf..HEAD` | 0 | PASS |
| `payload-audit` | `python3 -I -B /media/jer/Files/CCF/Verification/D073-BaseReconciliation-414-20261112/audit_scope.py payload` | 0 | PASS |
| `payload-base-tree` | `git rev-parse 'e759193354b4613f88e3370a62ccaccf02c556cf^{tree}'` | 0 | PASS |
| `payload-blob-0` | `git hash-object Assets/ForestPrototype/ScenarioOne/ScenarioOneManagementFollowonReadout.cs` | 0 | PASS |
| `payload-blob-1` | `git hash-object Tools/Verification/ManagementFollowonReadout/ManagementFollowonReadoutVerification.cs` | 0 | PASS |
| `payload-blob-2` | `git hash-object Tools/Verification/ManagementFollowonReadout/OfflineContractTypes.cs` | 0 | PASS |
| `payload-blob-3` | `git hash-object Tools/Verification/ManagementFollowonReadout/README.md` | 0 | PASS |
| `payload-blob-4` | `git hash-object Tools/Verification/ManagementFollowonReadout/run_mutations.py` | 0 | PASS |
| `payload-blob-5` | `git hash-object Tools/Verification/ManagementFollowonReadout/run_offline.py` | 0 | PASS |
| `payload-blob-6` | `git hash-object Assets/ForestPrototype/ScenarioOne/ScenarioOneManagementFollowonReadout.cs.meta` | 0 | PASS |
| `payload-branch` | `git branch --show-current` | 0 | PASS |
| `payload-committed-blob-0` | `git rev-parse d86a68a18208bab6939bee9e41aa916d6f998fa7:Assets/ForestPrototype/ScenarioOne/ScenarioOneManagementFollowonReadout.cs` | 0 | PASS |
| `payload-committed-blob-1` | `git rev-parse d86a68a18208bab6939bee9e41aa916d6f998fa7:Tools/Verification/ManagementFollowonReadout/ManagementFollowonReadoutVerification.cs` | 0 | PASS |
| `payload-committed-blob-2` | `git rev-parse d86a68a18208bab6939bee9e41aa916d6f998fa7:Tools/Verification/ManagementFollowonReadout/OfflineContractTypes.cs` | 0 | PASS |
| `payload-committed-blob-3` | `git rev-parse d86a68a18208bab6939bee9e41aa916d6f998fa7:Tools/Verification/ManagementFollowonReadout/README.md` | 0 | PASS |
| `payload-committed-blob-4` | `git rev-parse d86a68a18208bab6939bee9e41aa916d6f998fa7:Tools/Verification/ManagementFollowonReadout/run_mutations.py` | 0 | PASS |
| `payload-committed-blob-5` | `git rev-parse d86a68a18208bab6939bee9e41aa916d6f998fa7:Tools/Verification/ManagementFollowonReadout/run_offline.py` | 0 | PASS |
| `payload-committed-blob-6` | `git rev-parse d86a68a18208bab6939bee9e41aa916d6f998fa7:Assets/ForestPrototype/ScenarioOne/ScenarioOneManagementFollowonReadout.cs.meta` | 0 | PASS |
| `payload-diff-check` | `git diff --check e759193354b4613f88e3370a62ccaccf02c556cf HEAD` | 0 | PASS |
| `payload-existing-changes` | `git diff --name-status --diff-filter=CDMRTUXB e759193354b4613f88e3370a62ccaccf02c556cf HEAD` | 0 | PASS |
| `payload-guid` | `git grep -n bd8034988bda43aeb0e153015a3715eb -- Assets` | 0 | PASS |
| `payload-head` | `git rev-parse HEAD` | 0 | PASS |
| `payload-merges` | `git rev-list --merges e759193354b4613f88e3370a62ccaccf02c556cf..HEAD` | 0 | PASS |
| `payload-scope` | `git diff --name-status e7591933 HEAD` | 0 | PASS |
| `payload-source-blob-0` | `git rev-parse 0ce4793f2a2a600ea01bbcf3e9800a2ac39293f4:Assets/ForestPrototype/ScenarioOne/ScenarioOneManagementFollowonReadout.cs` | 0 | PASS |
| `payload-source-blob-1` | `git rev-parse 0ce4793f2a2a600ea01bbcf3e9800a2ac39293f4:Tools/Verification/ManagementFollowonReadout/ManagementFollowonReadoutVerification.cs` | 0 | PASS |
| `payload-source-blob-2` | `git rev-parse 0ce4793f2a2a600ea01bbcf3e9800a2ac39293f4:Tools/Verification/ManagementFollowonReadout/OfflineContractTypes.cs` | 0 | PASS |
| `payload-source-blob-3` | `git rev-parse 0ce4793f2a2a600ea01bbcf3e9800a2ac39293f4:Tools/Verification/ManagementFollowonReadout/README.md` | 0 | PASS |
| `payload-source-blob-4` | `git rev-parse 0ce4793f2a2a600ea01bbcf3e9800a2ac39293f4:Tools/Verification/ManagementFollowonReadout/run_mutations.py` | 0 | PASS |
| `payload-source-blob-5` | `git rev-parse 0ce4793f2a2a600ea01bbcf3e9800a2ac39293f4:Tools/Verification/ManagementFollowonReadout/run_offline.py` | 0 | PASS |
| `payload-status` | `git status --porcelain` | 0 | PASS |
| `payload-tree` | `git rev-parse 'HEAD^{tree}'` | 0 | PASS |
| `stage-payload` | `git add -- Assets/ForestPrototype/ScenarioOne/ScenarioOneManagementFollowonReadout.cs Tools/Verification/ManagementFollowonReadout/ManagementFollowonReadoutVerification.cs Tools/Verification/ManagementFollowonReadout/OfflineContractTypes.cs Tools/Verification/ManagementFollowonReadout/README.md Tools/Verification/ManagementFollowonReadout/run_mutations.py Tools/Verification/ManagementFollowonReadout/run_offline.py Assets/ForestPrototype/ScenarioOne/ScenarioOneManagementFollowonReadout.cs.meta` | 0 | PASS |
| `staged-scope` | `git diff --cached --name-status` | 0 | PASS |
| `staged-tree` | `git write-tree` | 0 | PASS |
| `status-start` | `git status --porcelain` | 0 | PASS |
| `typecheck` | `python3 -I -B Tools/Verification/ScenarioOneNonUi/compile_runtime_sources.py --src /media/jer/Files/CCF/Worktrees/CCF-D073-AnnualReview-V1 --out /media/jer/Files/CCF/Verification/D073-BaseReconciliation-414-20261112/typecheck-production` | 0 | PASS |
| import | `git show 0ce4793f2a2a600ea01bbcf3e9800a2ac39293f4:Assets/ForestPrototype/ScenarioOne/ScenarioOneManagementFollowonReadout.cs > Assets/ForestPrototype/ScenarioOne/ScenarioOneManagementFollowonReadout.cs` | 0 | PASS |
| import | `git hash-object Assets/ForestPrototype/ScenarioOne/ScenarioOneManagementFollowonReadout.cs` | 0 | PASS |
| import | `git show 0ce4793f2a2a600ea01bbcf3e9800a2ac39293f4:Tools/Verification/ManagementFollowonReadout/ManagementFollowonReadoutVerification.cs > Tools/Verification/ManagementFollowonReadout/ManagementFollowonReadoutVerification.cs` | 0 | PASS |
| import | `git hash-object Tools/Verification/ManagementFollowonReadout/ManagementFollowonReadoutVerification.cs` | 0 | PASS |
| import | `git show 0ce4793f2a2a600ea01bbcf3e9800a2ac39293f4:Tools/Verification/ManagementFollowonReadout/OfflineContractTypes.cs > Tools/Verification/ManagementFollowonReadout/OfflineContractTypes.cs` | 0 | PASS |
| import | `git hash-object Tools/Verification/ManagementFollowonReadout/OfflineContractTypes.cs` | 0 | PASS |
| import | `git show 0ce4793f2a2a600ea01bbcf3e9800a2ac39293f4:Tools/Verification/ManagementFollowonReadout/README.md > Tools/Verification/ManagementFollowonReadout/README.md` | 0 | PASS |
| import | `git hash-object Tools/Verification/ManagementFollowonReadout/README.md` | 0 | PASS |
| import | `git show 0ce4793f2a2a600ea01bbcf3e9800a2ac39293f4:Tools/Verification/ManagementFollowonReadout/run_mutations.py > Tools/Verification/ManagementFollowonReadout/run_mutations.py` | 0 | PASS |
| import | `git hash-object Tools/Verification/ManagementFollowonReadout/run_mutations.py` | 0 | PASS |
| import | `git show 0ce4793f2a2a600ea01bbcf3e9800a2ac39293f4:Tools/Verification/ManagementFollowonReadout/run_offline.py > Tools/Verification/ManagementFollowonReadout/run_offline.py` | 0 | PASS |
| import | `git hash-object Tools/Verification/ManagementFollowonReadout/run_offline.py` | 0 | PASS |
| import | `git grep -n bd8034988bda43aeb0e153015a3715eb` | 1 | PASS (no pre-existing GUID match) |
| `offline-dotnet/dotnet-run` | `/usr/bin/dotnet run --project /media/jer/Files/CCF/Verification/D073-BaseReconciliation-414-20261112/tmp/ccf-management-readout-vhi97vhp/ManagementReadoutCheck.csproj -c Release` | 0 | PASS |
| `offline-dotnet/prebuild` | `/usr/bin/dotnet build /media/jer/Files/CCF/Verification/D073-BaseReconciliation-414-20261112/tmp/ccf-management-readout-vhi97vhp/ManagementReadoutCheck.csproj -c Release -v minimal` | 0 | PASS |
| `typecheck-dotnet/dotnet-build` | `/usr/bin/dotnet build -nologo -v q -c Release '-clp:NoSummary;ErrorsOnly'` | 0 | PASS |

Optional command `python3 -I -B Tools/Verification/ManagementFollowonReadout/run_mutations.py --out /media/jer/Files/CCF/Verification/D073-BaseReconciliation-414-20261112/mutations`: **NOT RUN**; no optional mutation result is claimed.

## Remaining verification and handoff

Unity import, Unity/Assembly-CSharp compilation, Editor/runtime behaviour, rendered review, UI binding, real old-save load, save/load persistence, harvesting/resource/consumption/regeneration behaviour, protected runtime/hash regressions and Player/Windows behaviour/performance are **NOT VERIFIED by this task**. No Unity, Player or Windows build ran. Cadence's earlier qualified status belongs to the starting commit, not to this new combined candidate's Unity qualification.

Expected direct check in a separately authorised Unity lane: import this exact published candidate using Unity 6000.6.0f1 and confirm the new script's GUID imports and Assembly-CSharp compiles; do not infer gameplay use before the separate D-073 UI implementation/qualification. No disposable harness was put under Assets and no local save was accessed by these offline runners.

Other worktrees, Primary's unrelated dirty work, `main`, Ops board and `Docs/Project/coordination/active-tasks.md` were not modified. No external dependencies/packages/storage policy changes were introduced. Dashboard/Drive synchronization and parent coordination updates are NOT VERIFIED and remain with the orchestrator/Manager outside this writer's allowed files.

Worker assessment: the byte-identical combined source base passes the two requested offline gates and is ready for parent review. Further implementation, Unity qualification and Manager integration/acceptance remain separate gates.
