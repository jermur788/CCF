# Multi #692 Child A P0 ancestry result

**PASS for local Git ancestry and file identity. Independent review pending.** Candidate `ce7bfeb6b439d935b2e00094f79338d3b8a658a0` is retained on `integrate/multi-p0-q-current-main-20261117-v1`. Its first parent is the immutable base M `5b858031e2cd79be148864a62692653548e8b4af` and its second parent is qualified P0 Q `a7f41983c2f2f233b8fe999d1a5b10e72bdd2866`. The tracked tree differs from M by exactly 52 additions. Its P0 directory is identical to Q.

## Custody and target

- Actual writer was OpenAI Codex GPT-6.2 Sol Medium in native T3 Code on Linux host `jer-laptop`. The user explicitly revoked the GPT-6.1 Sol High lease and reassigned this session as sole writer. The deployed `MultiP0P1Integ687` board row, lane configuration, and custody receipt were read back after the update. The board passed deployed lint with 35 live tasks. The lease expires 2026-11-19 18:00 UTC. [Pre-fetch RUNNING registration](https://github.com/jermur788/CCF/issues/692#issuecomment-6959288923)
- Store `/media/jer/Files/CCF/Operations/CCF-687-P0P1-Integration-20261117/git/CCF-687-Integration.git`, device/inode `2068:10902740`, ext4 on `/dev/sdb4`, UUID `2283b135-bf37-42d6-921f-2fb861d0a8d4`. Preflight found zero objects, zero refs, zero linked worktrees, and an absent checkout and remote branch. The Unity guard reported free and no Unity, dotnet, or strace process was observed during custody preflight.
- The only source checkout is `/media/jer/Files/CCF/Operations/CCF-687-P0P1-Integration-20261117/ccf-687-child-a`. Its final status output is empty. The source branch remains local under the existing lease for independent review.
- GitHub main at result publication preflight was `55422ee4210fe87bc1b71bdfea350251860108e2`, 5 commits ahead of M and 0 behind. The compare listed only `Docs/Project/coordination/active-tasks.md`. Its #692 changes record the original stop and custody history. The direct user reassignment supersedes the old writer reservation for this run. M remained fixed. This documentation branch starts from the read-back main.

## Construction

The isolated bare store fetched M and Q by full SHA from the canonical GitHub repository, without depth or object filtering. The plain-merge preview succeeded and identified 11 extra Q-era Docs and specs additions. The worktree merge used the `ours` strategy to retain M's tree while recording Q in `MERGE_HEAD`. It then restored the entire P0 directory from Q to the index and worktree and committed the merge. This retained Q's actual commit as second parent and kept the 11 extra paths out of the checkout and index.

```sh
git --git-dir="$store" -c fetch.fsckObjects=true fetch --no-tags --no-write-fetch-head --no-auto-maintenance https://github.com/jermur788/CCF.git "$M" "$Q"
git --git-dir="$store" rev-parse --verify "$M^{commit}"
git --git-dir="$store" rev-parse --verify "$Q^{commit}"
git --git-dir="$store" merge-tree --write-tree "$M" "$Q"
git --git-dir="$store" worktree add -b "$branch" "$wt" "$M"
cd "$wt"
git -c core.hooksPath=/dev/null merge -s ours --no-ff --no-commit "$Q"
git rev-parse MERGE_HEAD
git restore --source="$Q" --staged --worktree -- Tools/MultiRackSitkaLab
git -c core.hooksPath=/dev/null -c commit.gpgSign=false commit -m 'Integrate qualified P0 Sitka simulator with original ancestry'
```

Fetch readback returned M and Q above. The preview tree was `d2c20e9e0f01b02c15a8e0945110cf9daa051528`. The checkout started at M, and `MERGE_HEAD` returned Q.

<details>
<summary>11 historical additions excluded from H2</summary>

```text
A	Docs/Handoffs/MultiRack-Simulation-20261114/MANAGER-MULTI-008-WINDOWS-WSL2-QUALIFICATION-ACCEPTANCE-20261114.md
A	Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-001-SITKA-2D-LAB-P0-USER-APPROVAL-RECORD-20261114.md
A	Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-002-SITKA-2D-LAB-P0-IMPLEMENTATION-HANDOFF-20261114.md
A	Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-004-SITKA-LAB-P0-HARDENING-IMPLEMENTATION-HANDOFF-20261114.md
A	Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-006-SITKA-LAB-P0-WRITE-SAFETY-REPAIR-HANDOFF-20261114.md
A	Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-008-SITKA-LAB-P0-WINDOWS-WSL2-UBUNTU-QUALIFICATION-HANDOFF-20261114.md
A	Docs/Handoffs/MultiRack-Simulation-20261116/Multi-Sitka-P0-Accurate-Post-Save-HTTP-Response-Repair-20261116.md
A	specs/multirack-sitka-2d-reference-lab-v1/checklists/requirements.md
A	specs/multirack-sitka-2d-reference-lab-v1/plan.md
A	specs/multirack-sitka-2d-reference-lab-v1/spec.md
A	specs/multirack-sitka-2d-reference-lab-v1/tasks.md
```

</details>

## Native Git acceptance

| Command | Observed result |
| --- | --- |
| `git rev-parse HEAD HEAD^1 HEAD^2` | H2, M and Q in order |
| `git rev-list --parents -n 1 "$H2"` | Exact output below |
| `git merge-base --is-ancestor "$Q" "$H2"` | Exit 0 |
| `git rev-list --count "$Q"` | 1941 |
| `git merge-base --all "$M" "$Q"` | `fc18190b976f1363f1b5d1b0881810db2f8c6e39` |
| `git rev-parse "$H2:Tools/MultiRackSitkaLab"` | `f212c8f64fb28b83aa870c7b5b7c72e03051e46a` |
| `git log "$Q..$H2" -- Tools/MultiRackSitkaLab` | Exit 0, stdout empty (0 bytes). Original L0. |
| `git ls-tree -r "$Q" -- Tools/MultiRackSitkaLab` and the same command for H2 | `cmp` exit 0. All 52 paths, modes, types and blob IDs match. |
| `git diff --name-status "$M" "$H2"` | Exactly 52 A rows. Full output below. |
| `git diff --exit-code "$M" "$H2" -- . ':(exclude)Tools/MultiRackSitkaLab'` | Exit 0, empty |
| `git ls-tree -r` for M and H2, excluding only P0 paths | `cmp` exit 0 on 5862 tracked entries. No path additions, deletions or mode/blob changes outside P0. |
| `git rev-parse --is-shallow-repository` and `git replace -l` | `false` and empty. Shallow, graft and alternate files absent. Object/history override environment variables unset. |
| `git fsck --full --no-reflogs --no-dangling` on M/Q and H2 | Both exit 0 |
| `git rev-list --objects --missing=error "$M" "$Q"` | Exit 0, 20026 object entries |
| `git status --porcelain=v1 --untracked-files=all` | Empty |
| `git ls-remote https://github.com/jermur788/CCF.git "refs/heads/$branch"` | Empty |

```text
ce7bfeb6b439d935b2e00094f79338d3b8a658a0 5b858031e2cd79be148864a62692653548e8b4af a7f41983c2f2f233b8fe999d1a5b10e72bdd2866
```

The P0 and P1 directories were absent at M. P1 is absent at H2. The four checked CCF source pins at M were:

```text
100644 blob bb83d7f8468002ab13a6aec6caf548e0bf3ad34b	Assets/ForestPrototype/SimulationRandom.cs
100644 blob e4c73402d46bb3b75a9cba2e4622857a913fac92	Assets/ForestPrototype/SitkaGrowthModel.cs
100644 blob e638e28ede09afc201d9e01fed938184b7b282f8	Assets/ForestPrototype/Species/SitkaSpruce.asset
100644 blob 51faaa0de0d218886935509ff64cb4f40598dd03	Assets/ForestPrototype/TreeSpeciesDefinition.cs
```

<details>
<summary>Full first-parent diff by path</summary>

```text
A	Tools/MultiRackSitkaLab/.gitattributes
A	Tools/MultiRackSitkaLab/.gitignore
A	Tools/MultiRackSitkaLab/Directory.Build.props
A	Tools/MultiRackSitkaLab/MultiRackSitkaLab.sln
A	Tools/MultiRackSitkaLab/README.md
A	Tools/MultiRackSitkaLab/evidence/README.md
A	Tools/MultiRackSitkaLab/fixtures/synthetic/MANIFEST.sha256
A	Tools/MultiRackSitkaLab/fixtures/synthetic/oracle-quarter-ha.sitkalab.json
A	Tools/MultiRackSitkaLab/fixtures/synthetic/sitka-flat-demo-v1.sitkalab.json
A	Tools/MultiRackSitkaLab/fixtures/synthetic/sitka-flat-perf-2000-v1.sitkalab.json
A	Tools/MultiRackSitkaLab/global.json
A	Tools/MultiRackSitkaLab/schemas/lab-file-v1.md
A	Tools/MultiRackSitkaLab/schemas/lab-file-v1.schema.json
A	Tools/MultiRackSitkaLab/src/Core/CanonicalJson.cs
A	Tools/MultiRackSitkaLab/src/Core/Engine.cs
A	Tools/MultiRackSitkaLab/src/Core/FixtureCatalog.cs
A	Tools/MultiRackSitkaLab/src/Core/Geometry.cs
A	Tools/MultiRackSitkaLab/src/Core/LabDataStore.cs
A	Tools/MultiRackSitkaLab/src/Core/LabFile.cs
A	Tools/MultiRackSitkaLab/src/Core/LabSession.cs
A	Tools/MultiRackSitkaLab/src/Core/Metrics.cs
A	Tools/MultiRackSitkaLab/src/Core/Models.cs
A	Tools/MultiRackSitkaLab/src/Core/NoFollowFiles.cs
A	Tools/MultiRackSitkaLab/src/Core/SitkaLab.Core.csproj
A	Tools/MultiRackSitkaLab/src/Core/SnapshotValidator.cs
A	Tools/MultiRackSitkaLab/src/Core/Views.cs
A	Tools/MultiRackSitkaLab/src/Host/LabHost.cs
A	Tools/MultiRackSitkaLab/src/Host/Program.cs
A	Tools/MultiRackSitkaLab/src/Host/SitkaLab.Host.csproj
A	Tools/MultiRackSitkaLab/src/Ui/wwwroot/app.js
A	Tools/MultiRackSitkaLab/src/Ui/wwwroot/index.html
A	Tools/MultiRackSitkaLab/src/Ui/wwwroot/styles.css
A	Tools/MultiRackSitkaLab/tests/Core.Tests/FileFormatTests.cs
A	Tools/MultiRackSitkaLab/tests/Core.Tests/Fx.cs
A	Tools/MultiRackSitkaLab/tests/Core.Tests/LabDataStoreTests.cs
A	Tools/MultiRackSitkaLab/tests/Core.Tests/LayeredGuardTests.cs
A	Tools/MultiRackSitkaLab/tests/Core.Tests/MetricsAndGeometryTests.cs
A	Tools/MultiRackSitkaLab/tests/Core.Tests/OperationFuzzTests.cs
A	Tools/MultiRackSitkaLab/tests/Core.Tests/PersistenceAndReplayTests.cs
A	Tools/MultiRackSitkaLab/tests/Core.Tests/Program.cs
A	Tools/MultiRackSitkaLab/tests/Core.Tests/SitkaLab.Core.Tests.csproj
A	Tools/MultiRackSitkaLab/tests/Core.Tests/StateMachineTests.cs
A	Tools/MultiRackSitkaLab/tests/Host.Tests/HostTests.cs
A	Tools/MultiRackSitkaLab/tests/Host.Tests/Program.cs
A	Tools/MultiRackSitkaLab/tests/Host.Tests/SitkaLab.Host.Tests.csproj
A	Tools/MultiRackSitkaLab/tests/Shared/MiniTest.cs
A	Tools/MultiRackSitkaLab/tests/Ui/cdp.mjs
A	Tools/MultiRackSitkaLab/tests/Ui/run-browser-tests.mjs
A	Tools/MultiRackSitkaLab/tests/check-schema.py
A	Tools/MultiRackSitkaLab/tests/run-mutation-controls.py
A	Tools/MultiRackSitkaLab/tools/FixtureTool/Program.cs
A	Tools/MultiRackSitkaLab/tools/FixtureTool/SitkaLab.FixtureTool.csproj
```

</details>

<details>
<summary>Complete identical Q and H2 P0 file manifests</summary>

```text
100644 blob d67550237d6468ff0899a8e54b239eaef3f4caa0	Tools/MultiRackSitkaLab/.gitattributes
100644 blob 6a5645f31cc1dfd5f8ba7b9afd2ab6ad3e11f10b	Tools/MultiRackSitkaLab/.gitignore
100644 blob a27594d9bc7689bd1d173442718fa875ce73db18	Tools/MultiRackSitkaLab/Directory.Build.props
100644 blob 75c59628edecb41d275bec0197cc70e88893ea19	Tools/MultiRackSitkaLab/MultiRackSitkaLab.sln
100644 blob 047d649d01cb1a64ae24dc0168631776e658f0ea	Tools/MultiRackSitkaLab/README.md
100644 blob f028978b36839609cbcba41acc3bef89c110aa93	Tools/MultiRackSitkaLab/evidence/README.md
100644 blob a1dcb19d445b3cda4fc88b4578db4e3181f596de	Tools/MultiRackSitkaLab/fixtures/synthetic/MANIFEST.sha256
100644 blob 9b088ef4d2b0180bab1654f70da7e8c9949525c0	Tools/MultiRackSitkaLab/fixtures/synthetic/oracle-quarter-ha.sitkalab.json
100644 blob e8cce4bc6b0cd579733a19d456c5e1da0cb658b5	Tools/MultiRackSitkaLab/fixtures/synthetic/sitka-flat-demo-v1.sitkalab.json
100644 blob cbb0570db3dbe36ffa48ab6628f1c77e55e83706	Tools/MultiRackSitkaLab/fixtures/synthetic/sitka-flat-perf-2000-v1.sitkalab.json
100644 blob 9abca2da51978fdef575185d0dc0406272d4dae0	Tools/MultiRackSitkaLab/global.json
100644 blob a9905712bc0672531a8137de7d4f52505aba342c	Tools/MultiRackSitkaLab/schemas/lab-file-v1.md
100644 blob a0041461373e2115aef1a2c5e0ce860721041648	Tools/MultiRackSitkaLab/schemas/lab-file-v1.schema.json
100644 blob b7c1a8f5e50693518213adf1bc669244d4f58328	Tools/MultiRackSitkaLab/src/Core/CanonicalJson.cs
100644 blob c14e73c7a59e5e40cf1a4ca4efc2d83a7fe3b65f	Tools/MultiRackSitkaLab/src/Core/Engine.cs
100644 blob af7e2b0684838145d97c02f85043bd3b216615f4	Tools/MultiRackSitkaLab/src/Core/FixtureCatalog.cs
100644 blob 31628f87860c0d1531ff79e46d9a24edd301391e	Tools/MultiRackSitkaLab/src/Core/Geometry.cs
100644 blob f0c6030ab4e4821f8d1c05b767d9f5c5992f5884	Tools/MultiRackSitkaLab/src/Core/LabDataStore.cs
100644 blob 3c6728a3614b93324f156ec0aea5246a6c153ed3	Tools/MultiRackSitkaLab/src/Core/LabFile.cs
100644 blob 062dd7ed1e7b7198881c2febcb40f9dd02acedd5	Tools/MultiRackSitkaLab/src/Core/LabSession.cs
100644 blob 41d865c32e8f17f733bd55ca9df2bda5b09a6257	Tools/MultiRackSitkaLab/src/Core/Metrics.cs
100644 blob 845fb469a90b0a4648aa492eb3a2dd3aaa5ad288	Tools/MultiRackSitkaLab/src/Core/Models.cs
100644 blob 4bb637b861ff8ec8895b60fdb1dab449f0d12098	Tools/MultiRackSitkaLab/src/Core/NoFollowFiles.cs
100644 blob 142c6585cfda20bf75df98edf31917f9b5cf99d6	Tools/MultiRackSitkaLab/src/Core/SitkaLab.Core.csproj
100644 blob 510573ea1b35e0e52f0f95a1d4c236a7f8b9315f	Tools/MultiRackSitkaLab/src/Core/SnapshotValidator.cs
100644 blob 493420201cc038afc256ffbf9be6935e86d0a799	Tools/MultiRackSitkaLab/src/Core/Views.cs
100644 blob 095878e4438620ce7e77ba73a08bc6f67125a119	Tools/MultiRackSitkaLab/src/Host/LabHost.cs
100644 blob 41d68a45a97d0a664f6c8c9c59fe14205a7f036c	Tools/MultiRackSitkaLab/src/Host/Program.cs
100644 blob 1aa46efe51d98393fff84c29de3e7362f38aae42	Tools/MultiRackSitkaLab/src/Host/SitkaLab.Host.csproj
100644 blob 50c157eb461632f9323bf2679fa370dcc11cda55	Tools/MultiRackSitkaLab/src/Ui/wwwroot/app.js
100644 blob 502e2f45124d149bb70b54819075ab95ee74cee3	Tools/MultiRackSitkaLab/src/Ui/wwwroot/index.html
100644 blob fac5864ab487d5b5bbff9f6ef39b953d4fb722c4	Tools/MultiRackSitkaLab/src/Ui/wwwroot/styles.css
100644 blob 6c2770b743e1077869bba5834152ca7f3cf8ca20	Tools/MultiRackSitkaLab/tests/Core.Tests/FileFormatTests.cs
100644 blob 290047aa0bc2c801ea0870f9c9cb5747529c7d74	Tools/MultiRackSitkaLab/tests/Core.Tests/Fx.cs
100644 blob 8bccd251150d779d322f35c0e2f0bda6c8570579	Tools/MultiRackSitkaLab/tests/Core.Tests/LabDataStoreTests.cs
100644 blob 5597af04f581abfdb29c1d6822f5ed31558e52af	Tools/MultiRackSitkaLab/tests/Core.Tests/LayeredGuardTests.cs
100644 blob e346128b10e72002134fea23fbf9677ae7169d68	Tools/MultiRackSitkaLab/tests/Core.Tests/MetricsAndGeometryTests.cs
100644 blob 3d0969358bb28cb808751b2fc1a9d4c9e0d625f0	Tools/MultiRackSitkaLab/tests/Core.Tests/OperationFuzzTests.cs
100644 blob d0581de7854c4053fe99aa245b483d5798ec5c08	Tools/MultiRackSitkaLab/tests/Core.Tests/PersistenceAndReplayTests.cs
100644 blob 2c5d4a548fea567b52fc798d3868d7d9277a0d73	Tools/MultiRackSitkaLab/tests/Core.Tests/Program.cs
100644 blob 1a0604fe65f88dfb44485e64c587b9add6e6fd63	Tools/MultiRackSitkaLab/tests/Core.Tests/SitkaLab.Core.Tests.csproj
100644 blob 4761ebf3ffe1bf100b40d327dff731d368d4a19e	Tools/MultiRackSitkaLab/tests/Core.Tests/StateMachineTests.cs
100644 blob d744624cca01acd3033d838d6b3568f575326497	Tools/MultiRackSitkaLab/tests/Host.Tests/HostTests.cs
100644 blob 2c5d4a548fea567b52fc798d3868d7d9277a0d73	Tools/MultiRackSitkaLab/tests/Host.Tests/Program.cs
100644 blob cf896d4c205aee6183ec1cfef22c421dd4252b66	Tools/MultiRackSitkaLab/tests/Host.Tests/SitkaLab.Host.Tests.csproj
100644 blob 5a70580361c259942e95f36d9c6fb0ff4befcb0d	Tools/MultiRackSitkaLab/tests/Shared/MiniTest.cs
100644 blob 0f57d367904858237cbffb5fd34693f703643582	Tools/MultiRackSitkaLab/tests/Ui/cdp.mjs
100644 blob 03bfa30678834cc0b9b4796e64b68dcf17e92d90	Tools/MultiRackSitkaLab/tests/Ui/run-browser-tests.mjs
100644 blob 6c4f2458e420c92fd4c0d9e53f84f7d382d65055	Tools/MultiRackSitkaLab/tests/check-schema.py
100644 blob 64df89c370c34ad5267108adffdf23398028e59b	Tools/MultiRackSitkaLab/tests/run-mutation-controls.py
100644 blob ff639b085dcf7a5eb01e6c2a2ebdea93720fa018	Tools/MultiRackSitkaLab/tools/FixtureTool/Program.cs
100644 blob 8f91087b1be3aaf9679862861d1943196ec8b1da	Tools/MultiRackSitkaLab/tools/FixtureTool/SitkaLab.FixtureTool.csproj
```

</details>

## Preservation and handoff

The protected-state before and after files matched byte for byte (SHA-256 `65e8fbe1185f9747f2fa90e27b2eb2c6fe88aa944f58f0b3d0cf7820dfbf49f8`). The protected check compared ref and worktree-list digests and object inventory in the shared, legacy and #674 stores. It also compared HEAD, branch, status, tracked-diff digest and untracked-file hashes in Primary, the historical #674/#683 proof checkout, and the #639 P1 checkout. Primary's pre-existing modifications to `AGENTS.md` and `Docs/Operations/StorageArchitecture.md` were preserved. Scenario One assets, settings, and every other tracked entry outside P0 were identical to M in the candidate. Scenario One worktrees were outside the write scope. The scoped snapshot did not hash all individual files across every registered worktree.

Full native evidence, including commands and outputs, protected snapshots, manifests, the custody readback and SHA-256 inventory, is retained under `/media/jer/Files/CCF/Verification/CCF-687-P0P1-Integration-20261117/candidate-a`. The isolated store and checkout are retained under the existing lease. Only this Markdown is published from the result branch.

Independent different-provider review and approval remain pending. The prior default .NET security verification remains at 38 unresolved failures and the ecological year remains on hold. This task ran Git checks only. Dashboard and Drive synchronization were unverified at handoff.
