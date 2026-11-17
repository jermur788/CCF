# Multi #697 Child B — exact 27-file P1 incorporation

**PASS — local Git source and ancestry proof.** Candidate `719ccbf87f0644a1a3fb9487ce7b991f4b867424` is retained locally on `integrate/multi-p1-h2-f-child-b-20261117-v1`. The candidate has not been pushed or merged into GitHub main. Independent read-only review and Manager disposition are pending.

## Authority and custody

- Writer: T3 Code / OpenAI Codex GPT-6.2 Sol Medium on `jer-laptop`, sole writer in `MultiP1ChildB697` through 2026-11-19 18:00 UTC.
- [Physical custody grant](https://github.com/jermur788/CCF/issues/697#issuecomment-6960318965), [direct Manager release recorded on #697](https://github.com/jermur788/CCF/issues/697#issuecomment-6960373373), and [writer RUNNING registration](https://github.com/jermur788/CCF/issues/697#issuecomment-6960373846). The user directly instructed the writer to start and confirmed Manager authority. The writer recorded that approval on #697 before registration and source operations.
- Separate Files/ext4 store `/media/jer/Files/CCF/Operations/CCF-697-P1-Child-B-20261117/git/CCF-697-Child-B.git`. Worktree `/media/jer/Files/CCF/Operations/CCF-697-P1-Child-B-20261117/ccf-697-child-b`.
- Local native evidence `/media/jer/Files/CCF/Verification/CCF-697-P1-Child-B-20261117`, including the custody receipt, construction and verification logs, protected state snapshots, parent and tree listings, complete file comparisons, CCF source pins, and `SHA256SUMS`.

## Native proof

| Check | Observed result |
| --- | --- |
| True ordered parents | H2 `ce7bfeb6b439d935b2e00094f79338d3b8a658a0` first, F `e302f15bf6188605a2c2cbeddbd973b3d8ac0f66` second. H2 and original Q `a7f41983c2f2f233b8fe999d1a5b10e72bdd2866` both passed the ancestry check. |
| Restricted merge | Recorded F as the genuine second parent with the ours merge strategy, restored only the P1 directory from F, verified the staged names and modes/blobs, then committed once. Candidate root tree `5c7919568b4f580fd9ab1edc86f623caddcd6b93`. |
| P0 L0 | `git log Q..C -- Tools/MultiRackSitkaLab` empty, 0 bytes. |
| P0 L1 | 52/52 paths, modes, types and blobs equal in Q, H2 and C. Subtree `f212c8f64fb28b83aa870c7b5b7c72e03051e46a` agrees with the formal P0 acceptance and native H2/Q. This is the full ID used in the proof. The operator handoff truncated the ID. |
| P1 | Subtree `2c017956577583d629f9020beafb6c0e757b9b51`. All 27 paths, modes, types and blobs equal pinned F, with paths and blob IDs independently matched against the handoff list. |
| First-parent diff | Exactly 27 additions under `Tools/MultiRackSitkaP1Lab/`. |
| Remaining tree | Every tracked entry outside P1 equals H2, including 52 P0 entries and all 5,862 entries of M `5b858031e2cd79be148864a62692653548e8b4af`. This also preserves missing paths and excludes all historical docs/spec/authority changes outside P1. |
| Four CCF pins | 4/4 exact in H2 and C. Full IDs below. |
| Integrity and cleanliness | `git fsck --full --strict` exited 0. Clean candidate, genuine nonshallow history, no replacement refs, grafts, or alternates. |
| Protection | Original #692 H2 ref, clean checkout, index, object inventory and lease fingerprints matched before/after. Primary status and hashes of its two pre-existing modified files matched. |
| Main and publication boundary | At native proof, GitHub main was `ce5c944eddb32ec4fce5a1c1a4a6f484923a00d1`. M-to-main net diff was only `Docs/Project/coordination/active-tasks.md`. Candidate local ref absent on GitHub. This result Markdown is the sole published branch delta. |

## Exact P1 paths, modes, types and blobs

```text
100644 blob f268f4f08c66c0a7c36c3ba43c3864751196f10d	Tools/MultiRackSitkaP1Lab/.gitignore
100644 blob 70b4bd65bb983ca9a593d3e03cca04ef54e22e7d	Tools/MultiRackSitkaP1Lab/Directory.Build.props
100644 blob c2bc5c47e4ed8e942886622fddc528e2e7a207e9	Tools/MultiRackSitkaP1Lab/MultiRackSitkaP1Lab.sln
100644 blob e3b312b2cfd48081298e43dcbb5c3c4400d6ec50	Tools/MultiRackSitkaP1Lab/README.md
100644 blob 0daee7d4d316a04806d59a37df4677a00017eff2	Tools/MultiRackSitkaP1Lab/build/build-p0-core-ref.sh
100644 blob 8ec8b5da564d53fe3ea2ff7516f2a3e6ac21a945	Tools/MultiRackSitkaP1Lab/evidence/README.md
100644 blob 8bf81a4364a5a6bacf104d72d354e74635d3b26c	Tools/MultiRackSitkaP1Lab/fixtures/derived/p1d-30m-sitka-adult-v1.sitkap1fx.json
100644 blob 9abca2da51978fdef575185d0dc0406272d4dae0	Tools/MultiRackSitkaP1Lab/global.json
100644 blob 4a4953d08e33084a7c91e64a0e631d2de371cc21	Tools/MultiRackSitkaP1Lab/src/Cli/Program.cs
100644 blob 712fa56770d17064be0cd923b0f530a308a49ffc	Tools/MultiRackSitkaP1Lab/src/Cli/SitkaP1.Cli.csproj
100644 blob 205d975c4680e5b29ab886afad9b40b9956a4981	Tools/MultiRackSitkaP1Lab/src/Core/CcfSourcePins.cs
100644 blob b8d8bd7a6eedf8a6f013eea209cc7d3ebd2471d3	Tools/MultiRackSitkaP1Lab/src/Core/DerivedManifest.cs
100644 blob 9086c03b66dcb84d1bd1e6a99b61b714d088ba8b	Tools/MultiRackSitkaP1Lab/src/Core/Domain.cs
100644 blob 7e0696b4bd03f6f83ad123c228fe2eec6816d55b	Tools/MultiRackSitkaP1Lab/src/Core/F32.cs
100644 blob ea54183d08a004b7032e16341b7dc009f334ebad	Tools/MultiRackSitkaP1Lab/src/Core/FixtureAccess.cs
100644 blob cadcc287b68965ed48c2d1c58df19dee539c7e76	Tools/MultiRackSitkaP1Lab/src/Core/ManifestGenerator.cs
100644 blob 670deb1dcd24f9dd5df7ab600abad36085f53491	Tools/MultiRackSitkaP1Lab/src/Core/NoFollowRead.cs
100644 blob ee8504726e13a5a5fe129728b9594a5cafe6b96b	Tools/MultiRackSitkaP1Lab/src/Core/SitkaP1.Core.csproj
100644 blob 81e42f6a3d658182e8913ff517a16efa004092ee	Tools/MultiRackSitkaP1Lab/src/Core/StrictJson.cs
100644 blob b4017188718fe091ca36fe86b5e36a7e96e547f9	Tools/MultiRackSitkaP1Lab/src/Core/TrustTables.cs
100644 blob d7d88afa3a467a94b1f88c37224bbefc706f3368	Tools/MultiRackSitkaP1Lab/tests/Core.Tests/Program.cs
100644 blob 8c10dddc09aa61e427970b598f566e312409e159	Tools/MultiRackSitkaP1Lab/tests/Core.Tests/Runner.cs
100644 blob 9b66edd692ea4fc0fcfb06556c2b3f6542508496	Tools/MultiRackSitkaP1Lab/tests/Core.Tests/SitkaP1.Core.Tests.csproj
100644 blob c1e8371e76171d3295b924220609ab81ee6e69b9	Tools/MultiRackSitkaP1Lab/tests/build-output-safety.sh
100644 blob a8fa3489940ad0152c3b4cc4d331aea5941718e3	Tools/MultiRackSitkaP1Lab/tests/cli-safety.sh
100644 blob 6def25790f259dc2af10474eab7592c4bb1fe1e7	Tools/MultiRackSitkaP1Lab/tests/l4_analyze.py
100644 blob 3f0befe8d3a98b31c09f956a02a32ff435259cff	Tools/MultiRackSitkaP1Lab/tests/p0-isolation.sh
```

## CCF source pins

```text
bb83d7f8468002ab13a6aec6caf548e0bf3ad34b Assets/ForestPrototype/SimulationRandom.cs
e4c73402d46bb3b75a9cba2e4622857a913fac92 Assets/ForestPrototype/SitkaGrowthModel.cs
e638e28ede09afc201d9e01fed938184b7b282f8 Assets/ForestPrototype/Species/SitkaSpruce.asset
51faaa0de0d218886935509ff64cb4f40598dd03 Assets/ForestPrototype/TreeSpeciesDefinition.cs
```

## Review and remaining gates

Manager #694 should commission an independent different-provider read-only review of H2 to C under a separate physical review grant. The local evidence supports source identity and ancestry. Runtime correctness, Tier-2 native qualification, and default L4 provenance/disposition remain open. Existing default L4 FAIL38 and the first-year hold remain in effect. No .NET run, Unity session, Windows run, simulation year, or main integration was performed.
