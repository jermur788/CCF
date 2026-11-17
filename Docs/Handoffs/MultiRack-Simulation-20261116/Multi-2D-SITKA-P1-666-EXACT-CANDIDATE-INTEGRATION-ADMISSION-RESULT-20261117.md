# Multi #666 — independent exact-candidate P1-F0 integration admission

**Date:** 17 November 2026. **Verdict: ADMISSION APPROVED FOR SEPARATE PHYSICAL INTEGRATION PREPARATION — CONDITIONAL, NOT FOR MERGE.**  
**Hard blockers to actual source incorporation/qualification:** P0 source absent from current main; qualified P0 commit is not an ancestor of main; the unchanged P1 L0 isolation test assumes that ancestry; separate physical Ops/write custody has not been granted. **Runtime** default .NET L4 remains **FAIL / 38 unresolved** and never becomes PASS by this planning verdict.  
**Worker:** OpenAI ChatGPT GPT-6, independent browser/GitHub reviewer; original P1 worker was Anthropic Claude Opus 5.5. **No local worktree/native run.**

## 1. Authority, reservation and pinned candidate

- [Binding #666 packet](https://github.com/jermur788/CCF/blob/docs/multi-p1-666-integration-admission-handoff-20261117-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-P1-666-EXACT-CANDIDATE-INTEGRATION-ADMISSION-PACKET-20261117.md), [claim #666](https://github.com/jermur788/CCF/issues/666), [actual pre-edit RUNNING registration](https://github.com/jermur788/CCF/issues/666#issuecomment-6934232177). Refreshed live main board, claim, current main/feature and checked this exact output file absent before edit. Manager #641 had prospectively reserved **only this one new Markdown**; no P1/P0/main/source/board write ownership was conferred.
- **Frozen review base/current target:** `85f2b35deb751d78e9486d4db0f68d8d14b5ea96` on `main`, rechecked unchanged before publication; review branch `review/multi-p1-666-integration-admission-20261117-v1` freshly created at that exact commit. **Source feature tip:** `e302f15bf6188605a2c2cbeddbd973b3d8ac0f66`, likewise rechecked unchanged. **Exact accepted #662 source commit:** `a5574924a03e2590d7954e56e8925e2e17714213`, followed by only the author handoff document update at source tip. The accepted P1 directory tree at both source commit and final feature tip is `2c017956577583d629f9020beafb6c0e757b9b51`.
- [Manager #641 final exact source acceptance](https://github.com/jermur788/CCF/blob/docs/multi-p1-641-662-final-source-acceptance-20261117-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-P1-641-662-FOUR-RECORD-SOURCE-ACCEPTANCE-VERDICT-20261117.md) accepts #662's two-file status/test update on its feature branch only; [independent #664](https://github.com/jermur788/CCF/blob/review/multi-p1-664-trust-status-changed-lines-20261117-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-P1-664-INDEPENDENT-662-TRUST-STATUS-CHANGED-LINES-REVIEW-RESULT-20261117.md) approved four identity-only records and preserved refusals. This is **not** overall F0 main integration or native qualification.
- **P0 qualified provenance:** qualified commit `a7f41983c2f2f233b8fe999d1a5b10e72bdd2866`; exact `Tools/MultiRackSitkaLab` tree `f212c8f64fb28b83aa870c7b5b7c72e03051e46a`. Direct GitHub tree inspection shows the P1 source tip and qualified P0 commit have **identical P0 subtree SHA**, including three original synthetic fixtures (full pinned blob values). However **current main has no `Tools/MultiRackSitkaLab` subtree** and **no `Tools/MultiRackSitkaP1Lab` subtree**. These two absences are direct repo evidence, not just assumptions from an ahead/behind count.
- **Divergence:** `main` ↔ feature merge base `fc18190b976f1363f1b5d1b0881810db2f8c6e39`; compared current main → feature: **44 feature-side commits / 67 target-side commits**, 99 changed paths in the GitHub comparison; reverse comparison shows main-only canonical coordination, decision/milestone and historical docs. P0 pin → feature: **18 linear commits** and P0 subtree untouched. P0 pin and main diverge (not ancestor). **DO NOT merge the feature branch wholesale or mechanically replay its 18-commit history.**

## 2. Branch/diff classification and exact P0 integration dependency

| Scope observed in GitHub compare | Classification | Safe handling |
|---|---|---|
| **27 files** under `Tools/MultiRackSitkaP1Lab/**`, all absent on reviewed main | P1 F0 implementation + build/test/manifest (accepted-source final tree) | Candidate for **exact file-tree extraction** onto an isolated approved physical integration branch **after P0 prerequisite**, no changes to content/blobs |
| `Tools/MultiRackSitkaLab/**`, **52 tracked blobs / 68 nested entries** beneath qualified P0 source | P0 dependency, original simulator/Host/UI/tests/fixtures, **not P1-owned** | **No ad hoc overwrite**. Requires a *separate Manager-authorised, custody-verified P0 source/ancestry admission* preserving exact qualified subtree; original P0 operation/work remains barred in P1 |
| Four files under `specs/multirack-sitka-p1-adult-rules-comparator-v1/**` on feature, absent on current main | Approved task-level P1 technical spec/plan/tasks/checklist | **Distinct docs/canonical publication decision**, not silently included in a source-only physical import; preserve source approval and reconcile current product decisions |
| P1 author results, old P0 reports, #636/#639/#662 documentation on feature | Historical evidence/handoffs | Read by exact refs. Do not blanket-import branch docs or write over target |
| `Docs/Project/decision-log.md` modified on both histories; current main `decision-log.md`, `current-milestone.md`, live `active-tasks.md`, and Scenario One D-083/Compact30 | Canonical/Scenario One and live ownership; conflicting history | **TARGET AUTHORITY.** Never use the feature version or feature branch merge to overwrite/roll back; any later spec sync is separate explicitly approved work |
| All `Assets/**`, `Packages/**`, `ProjectSettings/**`, frozen Reference, saved worlds, physical Ops and unrelated files | Protected/excluded | Unchanged and out of scope |

**Important newly identified lineage trap (actual source check):** the accepted `build/build-p0-core-ref.sh` refuses unless `git rev-parse HEAD:Tools/MultiRackSitkaLab` **equals** the qualified tree; `tests/p0-isolation.sh` also asserts L1 tree equality **and L0** `git log a7f41983c2f2f233b8fe999d1a5b10e72bdd2866..HEAD -- Tools/MultiRackSitkaLab` must be empty. Merely copying byte-identical P0 files onto a main-based branch fixes L1 **but creates a post-pin P0 import commit visible to L0**, so that path does **not** qualify unchanged P1 F0. The qualified P0 commit is not a current-main ancestor.

**Admission requirement before P1 local source qualification:** Manager #641 must arrange a separately authorised P0 lineage integration that makes **both** `HEAD:Tools/MultiRackSitkaLab == f212c8...` **and** the original L0 ancestry/path-history guard pass against the exact target. One possible technical plan is a carefully controlled *P0-qualified history* integration on a newly isolated **main-based candidate branch**, protecting all current-main canonical documents and proving the merge does not alter the P0 tree. This needs an actual separate source/physical Ops reservation, conflict review and native L0 test, and it must **not** be confused with merging the entire P1 feature branch. If a history-preserving route cannot satisfy L0, **STOP**; propose a separate approved, independently reviewed ancestry-aware verification-policy change rather than silently disabling or weakening L0. Do not call an exact-tree copy alone sufficient.

The P1 Core project explicitly requires a **binary** `SitkaLab.Core.dll` built from P0 in an external temporary directory, stored only under ignored P1-owned `.p0ref/`, with project-enforced expected DLL SHA-256 `9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3`; no project reference or P0 in-place build. If a different SDK/runtime produces a different binary hash, **STOP and investigate** instead of modifying the accepted hash or source/fixture. Required SDK source pin is `global.json` 8.0.133 (latestPatch). External TMPDIR, non-link output guards and no P0 writes remain mandatory.

## 3. Complete exact P1 F0 tracked bill of materials (27 of 27 files)

**Evidence method:** enumerate the GitHub **final accepted source feature tree** recursively, type `blob`; **current main has none of these paths**. Every row is a required **new unchanged file** when/if the P1 physical incorporation is separately authorised. The expected per-file Git blob is the immutable hash below. These represent the entire tracked P1 directory—do not take just the two #662 files; do not omit hidden `.gitignore`, tests, security scripts, binary reference builder, deterministic JSON manifest or hand-authored project configuration. No tracked `.p0ref/`, `bin/`, `obj/`, data or evidence scratch is part of this BOM.

| Relative path under `Tools/MultiRackSitkaP1Lab/` | Dependency / category | Git blob at accepted source tip | Future target operation |
|---|---|---|---|
| `.gitignore` | Project/metadata | `f268f4f08c66c0a7c36c3ba43c3864751196f10d` | New (absent on target), extract verbatim |
| `build/build-p0-core-ref.sh` | Build isolation | `0daee7d4d316a04806d59a37df4677a00017eff2` | New (absent on target), extract verbatim |
| `Directory.Build.props` | Project/metadata | `70b4bd65bb983ca9a593d3e03cca04ef54e22e7d` | New (absent on target), extract verbatim |
| `evidence/README.md` | Project/metadata | `8ec8b5da564d53fe3ea2ff7516f2a3e6ac21a945` | New (absent on target), extract verbatim |
| `fixtures/derived/p1d-30m-sitka-adult-v1.sitkap1fx.json` | Manifest/fixture | `8bf81a4364a5a6bacf104d72d354e74635d3b26c` | New (absent on target), extract verbatim |
| `global.json` | Project/metadata | `9abca2da51978fdef575185d0dc0406272d4dae0` | New (absent on target), extract verbatim |
| `MultiRackSitkaP1Lab.sln` | Project/metadata | `c2bc5c47e4ed8e942886622fddc528e2e7a207e9` | New (absent on target), extract verbatim |
| `README.md` | Project/metadata | `e3b312b2cfd48081298e43dcbb5c3c4400d6ec50` | New (absent on target), extract verbatim |
| `src/Cli/Program.cs` | CLI/source | `4a4953d08e33084a7c91e64a0e631d2de371cc21` | New (absent on target), extract verbatim |
| `src/Cli/SitkaP1.Cli.csproj` | CLI/source | `712fa56770d17064be0cd923b0f530a308a49ffc` | New (absent on target), extract verbatim |
| `src/Core/CcfSourcePins.cs` | Core/source | `205d975c4680e5b29ab886afad9b40b9956a4981` | New (absent on target), extract verbatim |
| `src/Core/DerivedManifest.cs` | Core/source | `b8d8bd7a6eedf8a6f013eea209cc7d3ebd2471d3` | New (absent on target), extract verbatim |
| `src/Core/Domain.cs` | Core/source | `9086c03b66dcb84d1bd1e6a99b61b714d088ba8b` | New (absent on target), extract verbatim |
| `src/Core/F32.cs` | Core/source | `7e0696b4bd03f6f83ad123c228fe2eec6816d55b` | New (absent on target), extract verbatim |
| `src/Core/FixtureAccess.cs` | Core/source | `ea54183d08a004b7032e16341b7dc009f334ebad` | New (absent on target), extract verbatim |
| `src/Core/ManifestGenerator.cs` | Core/source | `cadcc287b68965ed48c2d1c58df19dee539c7e76` | New (absent on target), extract verbatim |
| `src/Core/NoFollowRead.cs` | Core/source | `670deb1dcd24f9dd5df7ab600abad36085f53491` | New (absent on target), extract verbatim |
| `src/Core/SitkaP1.Core.csproj` | Core/source | `ee8504726e13a5a5fe129728b9594a5cafe6b96b` | New (absent on target), extract verbatim |
| `src/Core/StrictJson.cs` | Core/source | `81e42f6a3d658182e8913ff517a16efa004092ee` | New (absent on target), extract verbatim |
| `src/Core/TrustTables.cs` | Core/source | `b4017188718fe091ca36fe86b5e36a7e96e547f9` | New (absent on target), extract verbatim |
| `tests/build-output-safety.sh` | Test/safety | `c1e8371e76171d3295b924220609ab81ee6e69b9` | New (absent on target), extract verbatim |
| `tests/cli-safety.sh` | Test/safety | `a8fa3489940ad0152c3b4cc4d331aea5941718e3` | New (absent on target), extract verbatim |
| `tests/Core.Tests/Program.cs` | Test/safety | `d7d88afa3a467a94b1f88c37224bbefc706f3368` | New (absent on target), extract verbatim |
| `tests/Core.Tests/Runner.cs` | Test/safety | `8c10dddc09aa61e427970b598f566e312409e159` | New (absent on target), extract verbatim |
| `tests/Core.Tests/SitkaP1.Core.Tests.csproj` | Test/safety | `9b66edd692ea4fc0fcfb06556c2b3f6542508496` | New (absent on target), extract verbatim |
| `tests/l4_analyze.py` | Test/safety | `6def25790f259dc2af10474eab7592c4bb1fe1e7` | New (absent on target), extract verbatim |
| `tests/p0-isolation.sh` | Test/safety | `3f0befe8d3a98b31c09f956a02a32ff435259cff` | New (absent on target), extract verbatim |

**Whole-directory invariant:** reconstructed `Tools/MultiRackSitkaP1Lab` tree must be `2c017956577583d629f9020beafb6c0e757b9b51` at the reviewed tip and future local candidate after the extract. A Git tree compare proves tracked content identity but **not** ignored files, physical permissions, runtime or native test PASS; local integrator must verify both.

**Commit provenance of these bytes:** initial `#639` scaffold `21bf97b80f182ed53ba8793dc8932ec04b5eaec5` introduced 24 P1 files; #644 safety correction `24805388e88cf9463ff89eb1c1c9ced18660953b` corrected existing files and introduced three safety scripts; #649 `9fc3a88b378b0cfff55d87afdd74f28a2dbf411c` repaired `l4_analyze.py` and `p0-isolation.sh`; #662 `a5574924a03e2590d7954e56e8925e2e17714213` modified only `src/Core/TrustTables.cs` and `tests/Core.Tests/Program.cs` (+57/−18), now accepted identity-only. Earlier/later commits in the 18-commit sequence also edit decision log, specs, author handoffs and receipts, so **blind cherry-pick of the entire lineage is not acceptable**. Exact final tree extraction is the recommended P1 phase once the independent P0 prerequisite has been qualified. If using any individually selected source commit route, verify its patch file-by-file and final P1 tree exact identity before considering it equivalent; no accidental docs/history replay.

## 4. Target CCF source pins, identity and behaviour ceiling

Directly fetched the **four target-main blobs** used by `src/Core/CcfSourcePins.cs` and compared with the immutable source pin list:

| `Assets/ForestPrototype/` file | Expected Git blob | Current main match |
|---|---|---|
| `SimulationRandom.cs` | `bb83d7f8468002ab13a6aec6caf548e0bf3ad34b` | **PASS** |
| `SitkaGrowthModel.cs` | `e4c73402d46bb3b75a9cba2e4622857a913fac92` | **PASS** |
| `Species/SitkaSpruce.asset` | `e638e28ede09afc201d9e01fed938184b7b282f8` | **PASS** |
| `TreeSpeciesDefinition.cs` | `51faaa0de0d218886935509ff64cb4f40598dd03` | **PASS** |

The two code files are linked read-only into P1; the asset/species file is a provenance/parameter citation. **No source adjustment required at reviewed main**, but exact pins must be checked again after the real target is selected; if drift occurs, stop for source-rule reconciliation rather than silently updating constants. P1 uses synthetic 30m geometry/100-tree data; it does not demonstrate Irish biological precision or Unity numeric/geometry equivalence.

**Four Accepted identity records on accepted feature tree:** original P0 `oracle-quarter-ha`, `sitka-flat-demo-v1`, `sitka-flat-perf-2000-v1` = immutable original synthetic **read-only inspection**; distinct `p1d-30m-sitka-adult-v1` = **synthetic 30m fixture identity** only. All four exact #661 independent review and #641 Manager references remain in compiled records. Their status is not a work or annual grant. The post-status original trust digest is `15dca9f18a4e93dbb3a5df9baaefda08f811476df22622ce396ca24788253b05`; derived catalog digest `143a253a81818b5867acba87041d3a54618b3d4c4f3d9a5f1dd5b3d1d4cfa6f7`—**directly recalculated in #664**, not independently recomputed again during this admission. P1 catalog fixture file SHA is `bcaaa5c3d9b3d36ad8319ffcc361a6225a02b90281cef8435de7c52f8ff757c4`; content hash is `b7dee8a34b4e66b4cc443b533604b5962f5fe4c1d2f28832270a0c2b02ba6627`; Float32 area `3db851ec` (0.09ha, 900m²).

## 5. Qualification matrix — before future incorporation vs on exact target

| Qualification field | Present evidence | Required on separately authorised integration candidate |
|---|---|---|
| Scope, file-tree identity, exact #662 source | **DIRECT STATIC PASS**: 27 P1 target-absent files, final tree pinned; two-file #662 review/Manager source-accepted | Reconfirm main/feature HEAD; exact 27 file blobs, complete P1 tree and no protected target paths changed. Capture base→candidate diff and clean worktree |
| P0 original source ownership and L0/L1 | **DIRECT STATIC P0 source tree match on feature**; **NO-GO on current main:** P0 directory absent, pin not ancestor | Explicitly approved separate P0 ancestry admission/custody; prove original subtree hash, P0 pin ancestry/path-log L0 and L1 exact. Verify no tracked/ignored P0 or source fixtures touched before/after |
| Source reuse CCF pins | **DIRECT STATIC PASS** all four named target-main blobs | Re-read on actual integration target; unchanged linked source and cited species parameter blobs |
| P0 Core binary | #639 author captured qualified binary hash; project builder/hash guard inspected | External archive build from pinned tree; verify expected DLL hash, output isolation and external TMPDIR. Never build beside original P0 |
| P1 compile and 33 core tests | **AUTHOR-NATIVE** #662 build and **33/33** PASS, four refusal mutants caught; #664 static review | One proportionate clean-build + 33/33 test run **on exact integrated candidate**; no invented replay, no weakened tests or deletion |
| P0 prior 52-test baseline | Prior P0 native qualification exists; **not rerun here** | Since P0 is newly entering target ancestry, one baseline native 52-test/fixture immutability run as applicable, strictly in approved isolated lane without unplanned in-place P0 build outputs |
| Synthetic derived manifest | Prior #643/#661 independent source/forest evidence: 100 distinct living 30m Sitka, 12 below 170mm (88 action-eligible) | Verify exact 32,567-byte fixture, 100 unique seeded IDs/locations, 12 below threshold, exact full-file/body hashes, deterministic `generate-manifest --check` |
| Trust refs and refusal order | #664 **DIRECT STATIC PASS**, Manager #641 source accepted; author native accepted read/CLI checks | Check four accepted status refs, both digests and normal read/inspection; all local Pending/null/null test copies retain `OriginalTrustNotAccepted`, `ManifestNotAccepted` including **before nonexistent file open** |
| Annual gates/Host/work/save | **DIRECT STATIC:** `AnnualGate.Check` always refuses; original `OriginalAnnualRefused`; derived Accepted `AnnualEngineNotImplemented`; Unity square `DomainUnavailable` | Assert three denial codes and no first ecological year, no Host/server, no work/lifecycle, no saves or P0 mutation. CLI refusal smoke is not an actual ecological year |
| P0 L2/L3/L3+ and L4 | Author tests and earlier independent receipt audits exist, but no current native integrated witness | Capture tracked hashes and ignored path/size/timestamp pre/post. If default L4 traced, record its genuine result; **do not claim PASS** when 38 unresolved persist |
| DEFAULT .NET L4 | **FAIL: 38 unresolved**, from prior default trace (19 memfd ftruncate + 19 diagnostic-socket fchmod); **not resolved** by four Accepted records | Separate approved runtime remediation/disposition and independent default qualification **before operational P1 use or first-year execution** |
| Verification-only switched L4 | Earlier controlled `DOTNET_EnableWriteXorExecute=0` + `DOTNET_EnableDiagnostics=0` **PASS 0** in bounded capture | Can be cited as **conditional verification environment evidence** only; no waiver of ordinary/default tracing or P0 isolation policy |
| Unity, P1-POLY, C-2b/40–80 parity, Windows, ecological year | Not performed/authorised in F0 | No execution or acceptance within this admission or immediate P1-F0 source integration prep |

**Safe native preparation is possible only after physical custody + P0 ancestry gates.** Isolated read-only source inspection, external P0 Core build and P1 compile/test/CLI *refusal-only* checks are ordinary local qualification operations, not ecological advances or production runtime. They do not depend on labelling default L4 PASS, but an unresolved default L4 must be reported honestly as a red independent gate and may prevent claiming the full originally specified P0-isolation script PASS. Do not change the analyzer or suppress diagnostic syscalls simply to make it green.

**Nonblocking documentation issue:** the existing P1 `README.md` table still says the starting manifest and original records are "pending review" despite #662 source acceptance. This is stale explanatory text, not code privilege. Preserve exact source-tree identity during initial extraction; any later wording correction needs its own small authorised update and expected tree/hash change, not a silent source edit by integrator.

## 6. Exact conditional local next task for Manager #641 — proposed, NOT dispatched

**Decision requested of Manager:** authorise a **separate physical integration-preparation claim and handoff**. **Difficulty HIGH** (P0/main divergent ancestry, historical protections, strict local dotnet/P0 isolation). Best-suited local model/provider: an isolated **OpenAI Codex/gpt-6.1-sol High** Git/.NET integration worker with native Ubuntu/physical Ops access; capable Claude Code alternative only after explicit single-writer handover. The current browser reviewer cannot verify or reserve real physical locks and has not done so.

**Proposed, not existing or reserved** integration lane: branch `integrate/multi-p1-f0-main-target-20261117-v1` forked from the then-fresh approved target main; isolated worktree `/home/jer/ccf-worktrees/multi-p1-f0-main-integration-20261117`. The source author worktree `/home/jer/ccf-worktrees/multi-p1-639-f0-30m` remains untouched. Scope should be **one writer**, with a prospective tracked allowlist of (A) **`Tools/MultiRackSitkaP1Lab/**` exact 27-file addition only,** and (B) **`Tools/MultiRackSitkaLab/**` ONLY if a distinct P0 import/ancestry approval and physical exclusive owner are first recorded**. All target Decision Log/milestone/board/specs/Scenario One assets/saves/Unity, original source branch, Reference, Windows and real data roots remain protected. Local ignored `.p0ref`/P1 build output and external scratch/evidence need separate explicit physical Ops permission; **do not** register a conflicting P0 writer by inference.

**Phased STOP gates:**

1. Manager separately resolves P0 qualified-history-to-main admission (merge-base, target canonical conflicts and exact P0 tree) and **obtains physical Ops reservation/lease** for exact paths, an identifiable claim, one owner and real worktree. Before any mutation, local worker reads both live GitHub and physical Ops, verifies clean worktree/branch/HEAD, installed SDK/FS, no other P0/P1/Unity owner/runner; posts actual [RUNNING] receipt. If missing -> **BLOCKED**.
2. On isolated candidate only, establish **exact P0 qualified tree AND L0 qualified ancestry/path-history** without violating current target docs; do not silently copy only files and call it done. Stop on any nontrivial Git conflict, unexpected P0 changes, protected canonical edits or unsatisfied L0. This first phase may require its *own* separate P0 integration handoff before P1 files are touched.
3. After approved P0 prerequisite, add **only the exact 27 P1 tracked blobs** from immutable `e302f15bf6188605a2c2cbeddbd973b3d8ac0f66` P1 tree; compare whole tree and four CCF source pins. Do not replay source author's historical D-084/decision-log/doc commits. Never modify the #662 trust source or tests.
4. Run one scoped **candidate-only** native build/33 tests, fixture/CLI refusals, applicable P0 baseline and L0–L3 attestation; preserve evidence to external approved verification storage. If default L4 remains FAIL 38, report it as FAIL, not as permission to run or waive. No ecological year. Stop on hash/compiler/tests/tree/guard failures; don't weaken a red test, overwrite unrelated dirty work or adjust P0 identity.
5. Publish an exact candidate handoff and separate independent qualification/integration review. Manager #641 then decides any **future** source integration into main. There is **no automatic main merge**, no P1 operational release, no Unity/Windows.

**Proposed paste-ready future dispatch (Manager must publish an authorised GitHub packet and claim with a direct URL first; this line is a planning recommendation, not an active job):**  
“Multi — prepare the exact accepted P1-F0 source integration candidate on a freshly authorised isolated main-based lane. First resolve and qualify P0 ancestry/tree and physical single-writer custody; then incorporate only the 27 pinned P1 tracked files from the #662 accepted feature tip, with exact tree/digest tests, 33 core tests, original annual-refusal and DEFAULT L4 FAIL 38 disclosed. Stop on missing gates; no main merge, P0/Scenario One mutation, ecological year, Unity or Windows.”

## 7. Distinct status and final admission

| State | Exact evidence-backed result |
|---|---|
| P1 F0 IMPLEMENTED on feature | **YES**, exact 27 tracked-file synthetic F0 scaffold/fixture and refusal gates |
| #662 identity-status SOURCE REVIEWED | **YES**, #664 independent changed-lines approved |
| #662 identity-status MANAGER SOURCE ACCEPTED | **YES**, Manager #641 separate verdict |
| Entire P1 F0 INTEGRATED on main | **NO**, neither P0 nor P1 Tools tree is currently on main |
| Exact current-main candidate NATIVE VERIFIED | **NO**, no integration candidate built/run |
| DEFAULT .NET L4 QUALIFIED | **NO**, prior **FAIL / 38 unresolved** |
| First ECOLOGICAL YEAR READY | **NO**, engine absent and all domain annuals refused |

**Final verdict: ADMISSION APPROVED FOR SEPARATE PHYSICAL INTEGRATION PREPARATION ONLY**, conditional on the explicit P0 qualified ancestry/source import plan and real physical ownership. **DO NOT interpret this as a GO to merge, cherry-pick, execute an ecological year or declare F0/default L4 qualified.** The strongest source-level positive is that exact P1 27-file extraction can be specified without changing Scenario One or original P0 blobs; the strongest present blocker is that **current main lacks P0 and P1, and simple P0 byte import would not satisfy L0's history assumption**.

**Manager next action:** read this handoff, adjudicate P0 ancestry path and publish a separately claimed/physically reserved integration-prep packet; hold operational gates. **User action:** none for #666 admission. **Dashboard, Drive, context mirrors and physical Ops synchronization:** not verified from this browser review; Manager/physical custodian must reconcile after substantive project status change. **This report changes no simulator source, no source branch, no main, no physical Ops and no runtime.**
