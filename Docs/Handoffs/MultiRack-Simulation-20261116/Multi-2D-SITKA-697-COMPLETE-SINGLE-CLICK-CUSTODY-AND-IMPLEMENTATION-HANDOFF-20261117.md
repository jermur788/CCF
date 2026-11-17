# Multi #697 — COMPLETE SINGLE-SOURCE EXECUTION HANDOFF (physical custodian → Manager → native Child B writer)

**This document is complete operational context, NOT an unconditional start permit.** There are three different actors and three sequential gates. An agent receiving ONLY this message has the project background, exact paths/hashes/source list, tasks, verification, output and stop conditions. **Current status (2026-11-17 20:23 UTC): BLOCKED; no Child B physical grant, actual writer assignment, or Manager [APPROVED TO START] exists.**

## 1 — Purpose, product and why this is needed

CCF's standalone **Multi-Rack 2D Sitka Simulator** is a separate .NET forestry lab, **not** the Unity Scenario One game. Its original P0 laboratory has 52 qualified tracked files under `Tools/MultiRackSitkaLab/`. #692 constructed a local history-preserving merge H2 bringing P0 into a current-project base M, with original qualified Q as genuine second parent. #696 independent native review passed; dedicated Manager #694 **APPROVED the LOCAL P0 ancestry/byte-identity result only**, then closed #692/#696. No P0 source is yet merged into GitHub `main`.

The P1 F0 foundation adds **exactly 27 tracked files** under `Tools/MultiRackSitkaP1Lab/`. It provides deterministic synthetic 30×30m Sitka test-stand identity (900m²/0.09ha, 100 living trees, 12 stems under 170mm and 88 at or above), derived manifest, P0 reference isolation, four CCF source pins, inspection-only trust records and guarded refusal tests. This is **not an annual growth engine**; original three P0 forests are read-only; P1 synthetic forest is identity/inspection only. There is NO authority to run the first ecological year, simulate work, felling, saves, replay or service operations.

Child B's job is to produce ONE further **LOCAL** restricted-history integration candidate based on H2 which adds precisely those 27 P1 blobs and changes **nothing else**, preserving P0 true ancestry/L0/L1 and all current CCF authority. This is a Git/source-identity operation, not new forestry feature coding. Q..F has 36 changed files (35 added, one modified) and old docs/spec/decision changes, so whole-feature merging or replaying old commits would be unsafe.

## 2 — Fixed source identity and accepted state (verify, never silently repin)

- Repo: `jermur788/CCF`, https://github.com/jermur788/CCF
- Management: [#694](https://github.com/jermur788/CCF/issues/694); Child B claim: [#697](https://github.com/jermur788/CCF/issues/697).
- Historical current-project P0 first parent **M:** `5b858031e2cd79be148864a62692653548e8b4af`
- Qualified original P0 second parent **Q:** `a7f41983c2f2f233b8fe999d1a5b10e72bdd2866`
- Accepted local P0 merge **H2:** `ce7bfeb6b439d935b2e00094f79338d3b8a658a0`
- **Authoritative exact P0 subtree:** `f212c8f64fb28b83aa870c7b5c72e03051e46a`
- Accepted P1 source feature **F:** `e302f15bf6188605a2c2cbeddbd973b3d8ac0f66`
- Accepted P1 subtree: `2c017956577583d629f9020beafb6c0e757b9b51`
- Last browser Manager main preflight: `f0fd55a425cf4b6f56d3fae6061c10b08a9731e4`; **NOT a moving target or a permissible automatic rebase**. Re-read current main before each phase. Previous M→main drift was coordination-only.
- Four immutable CCF source pins (full Git blob IDs):
  - `Assets/ForestPrototype/SimulationRandom.cs`: `bb83d7f8468002ab13a6aec6caf548e0bf3ad34b`
  - `Assets/ForestPrototype/SitkaGrowthModel.cs`: `e4c73402d46bb3b75a9cba2e4622857a913fac92`
  - `Assets/ForestPrototype/Species/SitkaSpruce.asset`: `e638e28ede09afc201d9e01fed938184b7b282f8`
  - `Assets/ForestPrototype/TreeSpeciesDefinition.cs`: `51faaa0de0d218886935509ff64cb4f40598dd03`
- Existing accepted H2 physically retained in isolated `/media/jer/Files/CCF/Operations/CCF-687-P0P1-Integration-20261117/git/CCF-687-Integration.git`, checkout `.../ccf-687-child-a`, local ref `integrate/multi-p0-q-current-main-20261117-v1`, and evidence `/media/jer/Files/CCF/Verification/CCF-687-P0P1-Integration-20261117/candidate-a/`. **Do not mutate this original reference or reuse its lease without explicit custodian continuation approval.** Original writer lease expires 2026-11-19 18:00 UTC; separate #696 read-only reviewer grant expires 2026-11-18 18:00 UTC.
- Full accepted P0 Manager verdict: https://github.com/jermur788/CCF/blob/docs/multi-694-p0-review-acceptance-p1-gates-20261117-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-694-MANAGER-692-696-LOCAL-P0-ANCESTRY-ACCEPTANCE-20261117.md

## 3 — Actor A: physical Ops custodian — ONLY the permission/preflight phase

**This phase is the next executable work. It is not P1 implementation.** An authorised physical Ops custodian must read live `/media/jer/ZX20/CCF-Ops/board/active.md`, `lanes.json`, relevant custody receipts, current leases/processes/branch refs and both configured Git stores. Identify any overlap with existing #692 H2 writer, #639 original Claude author, #674/#683 historical proof/review and other Scenario One/Unity work. Check mount/fs type, available space, Ops physical lock, board schema/lint, branch collision, exact original H2 HEAD and protected clean/dirty state.

Prospectively allocate **one exclusive non-Unity Child B Git-source writer lane**. Choose either (a) a **new isolated Files/ext4 Git store with exclusive branch/worktree/evidence root**, or (b) a serialized continuation in the existing isolated #692 lane **only if its physical custodian expressly authorises that continuation and protects H2 from ref/worktree mutation**. Do NOT guess physical path, branch, hostname, or lease expiry. They must come from an actual custodian receipt, be deployed and read back. Do not assign a second simultaneous writer or borrow the original #692 lease.

Record on #697 a **[PHYSICAL CHILD B CUSTODY GRANTED — SOURCE NOT STARTED]** receipt naming: actual assigned human/agent provider+model, host, single physical owner, mounted FS and UUID, exact store/worktree/branch/evidence paths, expiry, exact allowed Git object/worktree operations, protected H2 preservation method, exclusions, full non-overlap, Ops board/lanes readback and status checks. If no safe lease path exists, post [BLOCKED] with actual reason; do not improvise permission. Physical assignment by itself DOES NOT release implementation.

**Current expected situation:** no #697 receipt and no assigned writer; the previous GPT-6.2 Sol Medium appears only as a preferred model in older planning. It is NOT automatically granted. Primary checkout `/media/jer/Files/CCF-Primary` on `ops/workflow-foundation-v1` with dirty `AGENTS.md` and `Docs/Operations/StorageArchitecture.md` is **NOT** a Child B work area; never stash/clean/reset/write there.

## 4 — Actor B: dedicated Multi Manager #694 — mandatory source release

Once the custodian has posted the complete physical readback on #697, Manager #694 inspects physical identity evidence, full current GitHub coordination, main-vs-M/H2/F path/authority drift, actual proposed writer and the prior P0 approval. If clean, Manager issues a separate **[APPROVED TO START CHILD B]** on #697 identifying the *actual* host/model, exact lane/store/worktree/branch, expiry, source start H2, latest main observed, fixed F and 27-file allowlist, protection of H2 and source-history method.

**Preselected technical recommendation:** one restricted **two-parent Git merge** C with **H2 as first parent and F as second**, using a merge strategy that preserves H2 tree plus restoration of only the 27 P1 files from F, identical to the approved #692 restricted-P0 pattern. Check real Q ancestry and original L0/L1 afterward. The Manager must explicitly confirm this method at [APPROVED TO START]; this handoff itself does not grant source permission. If the existing physical lease cannot safely support a two-parent candidate, STOP; Manager must decide a supported alternative before edits.

Only after these two independent permissions may the actual assigned source worker continue.

## 5 — Actor C: exclusive native source writer — Child B implementation, AFTER two gates

1. Independently read back #697 physical grant and **Manager [APPROVED TO START]**; verify actual provider/model/host exactly match writer registration and no conflicting physical owner/lease expiry or Ops operation. Re-read current main/coordination and M/H2/Q/F, original source pins, original H2 clean status and source store identity. If anything conflicts, STOP without fetching or creating a worktree.
2. Post **[RUNNING — CHILD B] on #697 BEFORE fetch/object/worktree/branch changes**, stating actual model/provider/harness/host, lease, exact branch/worktree/store/evidence, starting H2 HEAD, latest GitHub main, F, chosen merge method and one GitHub result branch.
3. In the granted isolated store only, obtain F with complete genuine Git history (no replace refs/grafts, shallow/alternate tricks). Preserve original H2 commit, branch and checkout unchanged; create exactly one distinct custody-approved candidate branch/worktree at H2. No use of Primary/shared/legacy Git store as write target.
4. Perform **the specific restricted integration method authorised by Manager**. Example sequence conceptually: start from H2, record F as second parent with `merge -s ours --no-ff --no-commit F`, then `git restore --source=F --staged --worktree -- Tools/MultiRackSitkaP1Lab`, assert the exact staged diff and modes are only the 27 BOM entries, and commit once. Use qualified actual refs rather than trusting symbolic placeholders. If merge strategy reveals anything unexpected, STOP before committing; no blind whole-feature merge/cherry-pick.
5. Keep the resulting candidate local; never push source branch to origin or merge into main. No formatting, refactoring, edits to any original P0 or other files, no regeneration of fixtures or canonical docs. Exclude all remaining F-era docs/spec/authority/source changes explicitly.

## 6 — Complete immutable P1 27-file bill of materials (relative to `Tools/MultiRackSitkaP1Lab/`)

Every line is **an added tracked file** with exact expected Git blob ID from accepted F; there are NO other permitted source additions, edits or deletions.

```text
f268f4f08c66c0a7c36c3ba43c3864751196f10d  Tools/MultiRackSitkaP1Lab/.gitignore
0daee7d4d316a04806d59a37df4677a00017eff2  Tools/MultiRackSitkaP1Lab/build/build-p0-core-ref.sh
70b4bd65bb983ca9a593d3e03cca04ef54e22e7d  Tools/MultiRackSitkaP1Lab/Directory.Build.props
8ec8b5da564d53fe3ea2ff7516f2a3e6ac21a945  Tools/MultiRackSitkaP1Lab/evidence/README.md
8bf81a4364a5a6bacf104d72d354e74635d3b26c  Tools/MultiRackSitkaP1Lab/fixtures/derived/p1d-30m-sitka-adult-v1.sitkap1fx.json
9abca2da51978fdef575185d0dc0406272d4dae0  Tools/MultiRackSitkaP1Lab/global.json
c2bc5c47e4ed8e942886622fddc528e2e7a207e9  Tools/MultiRackSitkaP1Lab/MultiRackSitkaP1Lab.sln
e3b312b2cfd48081298e43dcbb5c3c4400d6ec50  Tools/MultiRackSitkaP1Lab/README.md
4a4953d08e33084a7c91e64a0e631d2de371cc21  Tools/MultiRackSitkaP1Lab/src/Cli/Program.cs
712fa56770d17064be0cd923b0f530a308a49ffc  Tools/MultiRackSitkaP1Lab/src/Cli/SitkaP1.Cli.csproj
205d975c4680e5b29ab886afad9b40b9956a4981  Tools/MultiRackSitkaP1Lab/src/Core/CcfSourcePins.cs
b8d8bd7a6eedf8a6f013eea209cc7d3ebd2471d3  Tools/MultiRackSitkaP1Lab/src/Core/DerivedManifest.cs
9086c03b66dcb84d1bd1e6a99b61b714d088ba8b  Tools/MultiRackSitkaP1Lab/src/Core/Domain.cs
7e0696b4bd03f6f83ad123c228fe2eec6816d55b  Tools/MultiRackSitkaP1Lab/src/Core/F32.cs
ea54183d08a004b7032e16341b7dc009f334ebad  Tools/MultiRackSitkaP1Lab/src/Core/FixtureAccess.cs
cadcc287b68965ed48c2d1c58df19dee539c7e76  Tools/MultiRackSitkaP1Lab/src/Core/ManifestGenerator.cs
670deb1dcd24f9dd5df7ab600abad36085f53491  Tools/MultiRackSitkaP1Lab/src/Core/NoFollowRead.cs
ee8504726e13a5a5fe129728b9594a5cafe6b96b  Tools/MultiRackSitkaP1Lab/src/Core/SitkaP1.Core.csproj
81e42f6a3d658182e8913ff517a16efa004092ee  Tools/MultiRackSitkaP1Lab/src/Core/StrictJson.cs
b4017188718fe091ca36fe86b5e36a7e96e547f9  Tools/MultiRackSitkaP1Lab/src/Core/TrustTables.cs
c1e8371e76171d3295b924220609ab81ee6e69b9  Tools/MultiRackSitkaP1Lab/tests/build-output-safety.sh
a8fa3489940ad0152c3b4cc4d331aea5941718e3  Tools/MultiRackSitkaP1Lab/tests/cli-safety.sh
d7d88afa3a467a94b1f88c37224bbefc706f3368  Tools/MultiRackSitkaP1Lab/tests/Core.Tests/Program.cs
8c10dddc09aa61e427970b598f566e312409e159  Tools/MultiRackSitkaP1Lab/tests/Core.Tests/Runner.cs
9b66edd692ea4fc0fcfb06556c2b3f6542508496  Tools/MultiRackSitkaP1Lab/tests/Core.Tests/SitkaP1.Core.Tests.csproj
6def25790f259dc2af10474eab7592c4bb1fe1e7  Tools/MultiRackSitkaP1Lab/tests/l4_analyze.py
3f0befe8d3a98b31c09f956a02a32ff435259cff  Tools/MultiRackSitkaP1Lab/tests/p0-isolation.sh
```

Compare the final candidate `git ls-tree -r` listing against F and against this list with **path, mode, type and blob ID**, not just filenames. The complete directory tree must equal `2c017956577583d629f9020beafb6c0e757b9b51` exactly.

## 7 — Mandatory local Git proof on C (no broad build in this phase)

- Ordered ancestry if approved two-parent method: `git rev-list --parents -n1 C` = C H2 F; `git merge-base --is-ancestor H2 C` and `git merge-base --is-ancestor Q C` both successful. F must be genuine second parent if that method approved.
- P0 original L0: `git log Q..C -- Tools/MultiRackSitkaLab` **empty**, zero bytes/commits; no alteration of historical guard.
- P0 L1: `git rev-parse C:Tools/MultiRackSitkaLab` equals original `f212c8f64fb28b83aa870c7b5c72e03051e46a`. All 52 P0 blobs and modes unchanged vs Q/H2.
- P1 exact source tree: `git rev-parse C:Tools/MultiRackSitkaP1Lab` equals `2c017956577583d629f9020beafb6c0e757b9b51`; all 27 modes and Git blob IDs match F/BOM. No extra tracked P1 paths.
- First-parent diff: `git diff --no-renames --name-status H2 C` yields **exactly 27 A rows**, all under P1, zero changes anywhere else.
- Full tracked tree outside P1 identical H2↔C, including original 52 P0, 5,862 original M entries, Assets/Packages/ProjectSettings/current canonical Docs/specs and all missing paths. Original 11 Q-era Docs/specs and all other F-era extra changes remain absent.
- Four exact CCF source pins match at H2 and C; no unapproved parameter/provenance update.
- Git integrity, clean candidate and protection: nonshallow, no graft/replace/alternates, `git fsck --full --strict` as appropriate; original H2 source ref/worktree unaffected; protected before/after snapshots; no source branch on origin. Check current main drift and document overlap honestly.
- Failure of any check = **FAIL and stop**; no weakening original L0, no changed tests/hashes, no workaround acceptance. The local source candidate must remain for review without cleanup.

## 8 — Evidence, publication and handoff

Produce full native command/output logs, H2/F/C parent/tree and 27-BOM equality, first-parent diff, 5,862 + 52 non-P1 preservation, custody receipt and pre/post protected snapshots, integrity results, actual tested main delta and checksum manifest under the custodian-granted evidence directory.

Push **ONLY ONE NEW GitHub Markdown** on distinct docs-only branch (proposed exact name `docs/multi-697-p1-child-b-exact-tree-result-20261117-v1`, create only after checking it is unused and registered), path:
`Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-697-EXACT-P1-27-FILE-INCORPORATION-RESULT-20261117.md`.

Result must include actual provider/host/physical grant/Manager permit, exact candidate and proof, PASS/FAIL, source stay-local, all limitations and next review. Verify GitHub branch has exactly that one added file. Post [HANDED-OFF] on #697 with permanent link, do not self-approve. Manager then commissions independent different-provider **read-only** H2→C review with separate custodian review grant, gives APPROVED/NEEDS CHANGES/REJECTED and only later releases precise Tier-2 native qualification / default-L4 provenance / target-fresh main integration. Do not repeat full testing now.

## 9 — Absolute prohibitions and next gate

No P0 source reimplementation, 52-file edit, whole F merge, historical Q docs/specs import, authority decision rewrite, direct main push, borrowed lane, changed dirty Primary, #639 source lane, #674/#683 leases, Unity, Windows or first annual ecological year. No dotnet/build/strace or default L4 test policy change in Child B construction phase. Default runtime **L4 FAIL38** (19 memfd doublemapper ftruncate + 19 diagnostics socket fchmod) is outstanding; two verification-only switches do NOT establish production security acceptance. A dedicated read-only fd provenance investigation and subsequent security disposition are separately gated. Frozen Scenario One, saves and references untouched.

**Operational status at publication:** actor A custodian grant STILL MISSING, actor B Manager start permit STILL MISSING, actor C no actual assigned worker/RUNNING. The next legitimate action is **physical custody reservation/readback, not a source edit**. The custodian may use this one document as the complete request; then Manager appends approval to #697; only afterward can the assigned writer implement exactly the steps above. If access or model differs from the reserved receipt, stop rather than switching silently.
