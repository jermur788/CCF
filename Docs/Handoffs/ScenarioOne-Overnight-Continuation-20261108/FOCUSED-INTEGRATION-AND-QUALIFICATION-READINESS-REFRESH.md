# #334 Focused integration and qualification readiness refresh (read-only)

> **STATUS: CONDITIONAL — CTA SOURCE WAITING #331 + MANAGER G2/G3.** This is read-only evidence planning. It is **not** an integration, a qualification, a review verdict or a Manager decision. It creates no integration branch, worktree, PR or Unity run. **No integrated candidate exists. All 15 gates A–O are NOT RUN on any integrated candidate.** Nothing here is a PASS for a future integrated HEAD.

| Item | Value |
|---|---|
| Claim | [#334](https://github.com/jermur788/CCF/issues/334), [RUNNING comment](https://github.com/jermur788/CCF/issues/334#issuecomment-6715188970), admitted 2026-11-08T00:42:37Z (host `date -u`) |
| Binding packet | [MANAGER-READONLY-EXACT-INTEGRATION-AND-TEST-PLAN-PACKET-20261108.md](https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Overnight-Continuation-20261108/MANAGER-READONLY-EXACT-INTEGRATION-AND-TEST-PLAN-PACKET-20261108.md), under orchestrator [#332](https://github.com/jermur788/CCF/issues/332) |
| Worker (actual) | Anthropic **Claude Opus 5.5** (`claude-opus-5-5`) via the "Claude B — Pro" provider instance, Claude Code harness inside T3 Code, effort **high**; T3 child research sub-agent of the #332 orchestrator. Single authorised reroute after an OpenAI GPT-6.1 Sol dispatch failed at provider quota before doing any work (it posted no claim; it left a clean, unused T3 host worktree `/home/jer/.t3/worktrees/CCF-Primary/t3-5133d049` on `t3/ccf-334-integration-gate-plan`, which I did not touch). |
| Docs lane (mine) | branch `docs/ccf-integration-gate-plan-20261108-v1` from `origin/main` `c0513d6ac84dce4750747596bd9dacfddfd2da36` (fetched 00:41:57Z); worktree `/media/jer/Files/CCF/Operations/CCF-IntegrationReadiness20261108`; store `/media/jer/Files/CCF/Repository.git` |
| Evidence (mine) | `/media/jer/Files/CCF/Verification/CCF-IntegrationReadiness20261108/` (heads, delta lists, merge-tree trials, byte-identity witness, CTA transfer manifest) |
| Heads fetched | 00:41:57Z; re-fetched 00:47:35Z (picked up #330 publication) |

**Measured** = I ran the Git command and recorded output in the evidence directory. **Derived** = I read source and reasoned about it; nothing was executed. **Cited** = taken from a published handoff or Manager ruling; I did not reproduce it.

---

## 1. Summary

1. **Textual integration still works with the latest tips (measured).** `merge-tree` trials of guidance `9789369d` with Workbench tested `a4019e6b` / published `03b484d1`, and of either with CTA `990ec43e`, all finish with no conflicts. Guidance and Workbench share no changed path since their merge-base `50042bbe`. On the trial tree, all 67 guidance paths match `9789369d` byte for byte and all 17 Workbench paths match `a4019e6b`. The trial objects went only into a scratch object directory: the shared store's object count and refs were the same before and after.
2. **#315 missed three verifier-level problems (derived from source).** A clean merge does not mean the checkers will pass:
   - The guidance five-rack checker fails closed on the merged tree because its `PROTECTED` list says `Assets/Scenes/ForestTest.unity` must not change. Workbench adds one line to that scene (`retiresFromStartingWorld: 1`). The checker also accepts only its own allowlisted paths.
   - The #303 headland checker requires the production delta to equal exactly the D-072 stages. Workbench's six production paths fail that check.
   - The CTA checker reads the live `Assets/` tree. Twelve `.cs` files differ between the CTA-tested tree and the prospective integrated tree, so the CTA checker must run again on the integrated HEAD.

   Manager's #308 ruling already carries F-8 ("exact integrated lineage needs its own new stage-aware qualification"). This dossier spells out what that means for the files.
3. **PR #300 ancestry, corrected by measurement.** PR #300's head is the CTA verify branch and its base is the guidance branch. Its "historical Objective UI ancestors" are guidance's own first five commits (`ffe7c21c…7f5278bf`), which are ancestors of the approved tip `9789369d`. After `7f5278bf`, the CTA lineage changes **no `Assets/` file**. A trial merge of guidance with CTA produces an `Assets` tree identical to guidance's (`3547317e…`). So the whole-branch hazard concerns **authority and history**, not bytes. The ban on whole-branch merges still applies: a merge would bring in 23 superseded or NEEDS-CHANGES checker commits as ancestry, and on GitHub it would read as merging PR #300. File-level transfer stays the recommendation.
4. **CTA status: CONDITIONAL — CTA SOURCE WAITING #331 + MANAGER G2/G3.** #330's author handed off at 00:45:11Z (tested `990ec43e`, published `9867144c`). When I checked at 00:48Z, #331's independent reviewer (OpenAI GPT-6.1 Sol, high) was still being admitted. See §8 for the final check.
5. **The D-072 guidance UI has never been compiled in Unity, on any candidate (cited).** Every guidance handoff says Unity was NOT RUN. The only real Unity evidence is #287 at `5085e8af`, a Workbench-only lineage that has no D-072 UI, no approved verifier bytes and no CTA. That makes Gate A the first gate that can never be waived.

---

## 2. Exact source heads and status classes

All SHAs are measured (`git rev-parse` after fetch; file: `heads-20261108T0045Z.txt`). "Accepted" means a published Manager #200 ruling only.

| Lineage | Branch | Tested source | Published head | Class | Authority |
|---|---|---|---|---|---|
| D-072 Stream A: #295 cues, #297 Full Stand Map headland/feeder, #296 five-rack diagram, #303/#306/#308 checkers | `feature/scenario-one-next-action-guidance-v1` | `9789369d02963b4444f8530b071413d066bf1ddf` | same (origin tip) | **Accepted feature source: APPROVED SOURCE ONLY.** Not Unity-compiled, not integrated. | [#308/#310 Manager ruling](https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Overnight-Playtest-Readiness-20261106/MANAGER-D072-308-INDEPENDENT-REVIEW-DISPOSITION-AND-STAGE-C-RELEASE-20261106.md) |
| #284 Workbench retirement production (`57f10ff0`, the only Assets commit in the lineage) + #287 harness `5085e8af` | `feature/scenario-one-workbench-retirement-20261105` | `8b1946dc` (production) | `4d95e452` | Production **source-accepted (#284)**. Harness `5085e8af` NOT QUALIFIED (#287) and superseded by later verifier bytes. | #308 ruling (#287 release); #287 result |
| #318 → #323 → #328 Workbench verifier/runner chain | `fix/workbench-receipt-finalization-20261107-v1` (contains `6b7d2d45` #318 and `2c4aea1c` #323; strict ancestry measured) | **`a4019e6b96b50e03d2b71d92c44fc30a0f7ace29`** | `03b484d10b36382ab86d5e345ece9acac96c9620` (adds the result Markdown only, measured) | **Workbench source-VERIFIED / runtime-UNQUALIFIED.** No Unity run with these verifier bytes. B3 tree substitution unresolved. | [#329 Manager split disposition](https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Morning-Closure-20261107/MANAGER-329-CTA-WORKBENCH-SOURCE-DISPOSITION-20261107.md) Stage B |
| CTA checker #299…#328 | `fix/cta-roslyn-symbol-binding-20261107-v1` | `288ba3863793e3775327fb7322dc06af28f7835a` | `6adb20318b80e8a81560bc58a63c8bdbfa15d7ca` | **Superseded.** #329: NEEDS CHANGES (A329-01). | #329 disposition Stage A |
| CTA #330 Update-site repair | `fix/cta-objectives-update-site-20261107-v1` | **`990ec43ec6bf5ec7063fb530f80b82e37501c582`** | `9867144c30d059433fb124aa016af6a5b20e1085` (adds the result Markdown only, measured) | **CTA checker source-UNAPPROVED: WAITING #331 + MANAGER G2/G3.** Author handoff only. | [#330 HANDED-OFF](https://github.com/jermur788/CCF/issues/330#issuecomment-6715211836) |
| PR #300 (draft) | head `verify/scenario-one-feedback-cta-anchor-20261106` @ `c83fae1a`, base = guidance branch | — | — | **PROHIBITED as a whole-branch merge.** Its 18 commits are verification-only on top of guidance ancestor `7f5278bf`. | Manager rules; PR title "DO NOT MERGE WHOLE" |
| `main` | `origin/main` | — | `c0513d6ac84dce4750747596bd9dacfddfd2da36` | Coordination/docs authority only. Since `e1be15c1`: 55 `Docs/` + 4 `specs/` paths, **zero** Assets/Tools/Packages/ProjectSettings paths (measured) | — |

**Test-only tooling** (no production effect): `Tools/Verification/{WorkbenchRetirement, RackV2Persistence, StandGeometry/Enlarged80Verification.cs, MfmCoverageWorld/MfmCoverageWorldUnityGate.cs}` from Workbench; `Tools/Verification/OvernightPlaytestFeedback/*` from CTA; `Tools/Verification/{ScenarioOneFiveRackDiagram, StandMap/HeadlandOffline}` from guidance. **Report-only commits:** `03b484d1`, `2c4aea1c`, `6b7d2d45`, `4d95e452`, `6adb2031`, `9867144c` and every `docs:`/`handoff:` commit in the CTA lineage.

**Measured ancestry:** `merge-base(G, WB) = merge-base(main, WB) = 50042bbe`; `merge-base(main, G) = e1be15c1`; `merge-base(G, CTA) = 7f5278bf` (guidance's T3 blocker commit). The CTA lineage therefore already contains guidance commits `ffe7c21c…7f5278bf` (#295 T2 cues). None of guidance, Workbench or CTA is an ancestor of `main`.

---

## 3. Proposed file-level assembly manifest for a future integration (no implementation)

**Starting point:** the Manager-approved guidance source `9789369d` (§2). Application order is deliberate: the whole-branch Workbench merge first, so ancestry and provenance stay intact, then the verification-only CTA transfer by explicit paths, then the docs freshness merge (optional).

| # | Source branch | Candidate tested head | Required paths | Approved scope | Collision / risk | Verification needed |
|---|---|---|---|---|---|---|
| 0 | `feature/scenario-one-next-action-guidance-v1` | `9789369d` | Base of the integration branch (all 31 paths changed since `e1be15c1`) | APPROVED SOURCE ONLY | None at base. Never Unity-compiled. `Scene.handle` m-3 carried | Gates A–I on the integrated HEAD |
| 1 | `fix/workbench-receipt-finalization-20261107-v1` | tested `a4019e6b`; merge published `03b484d1` (bytes identical except its result `.md`) | **Production:** `Assets/ForestPrototype/Editor/ForestSceneBuilder.cs`, `Assets/ForestPrototype/ForestBuildable.cs`, `Assets/ForestPrototype/ForestSaveController.cs`, `Assets/ForestPrototype/ForestStartingStand.cs`, `Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.PrescriptionApproval.cs`. **Scene:** `Assets/Scenes/ForestTest.unity` (+1 line). **Verifiers:** `Tools/Verification/WorkbenchRetirement/{WorkbenchRetirementVerification.cs, run_workbench_retirement.py, test_runner.py}`, `Tools/Verification/RackV2Persistence/{RackV2PersistenceVerification.cs, test_historical_projection.py}`, `Tools/Verification/StandGeometry/Enlarged80Verification.cs`, `Tools/Verification/MfmCoverageWorld/MfmCoverageWorldUnityGate.cs`. **Docs:** 5 handoff `.md` files | #284 production source-accepted; verifier chain source-approved (#329 Stage B); **runtime unqualified** | **No textual collision** (measured; 0 shared paths). **Verifier collisions S-1 and S-2 (§4)**: the scene line breaks the guidance checkers' frozen-scope assertions. No new Assets file, so no `.meta` risk (measured: modifications only) | Gate A compile; L; K; N; Tier-1 B3 |
| 2 | `fix/cta-objectives-update-site-20261107-v1` **(CONDITIONAL)** | tested `990ec43e` | **Only these 8 tool files**, by `git checkout 990ec43e -- <path>` or equivalent explicit transfer: `Tools/Verification/OvernightPlaytestFeedback/{ObjectiveCtaRoslynBinding.cs, objective_cta_roslyn_binding.py, roslyn-reference-manifest.txt, objective_cta_source_check.py, test_objective_cta_source_check.py, test_objective_cta_roslyn_binding.py, check_baseline.py, README.md}`. Blob ids and SHA-256 values are in `cta-transfer-manifest.txt` (e.g. manifest `978123241d3d…` = the in-code `MANIFEST_SHA256`). Optional provenance: the 7 CTA handoff `.md` files plus #330's result `.md` | **Verification-only. NOT APPROVED** until #331 and Manager G2/G3. If those reject, transfer nothing; Gate M stays NOT RUN | `README.md` and `check_baseline.py` exist in guidance with the same blob as `7f5278bf` (measured), so the CTA versions replace them with no three-way conflict. **Never** `git merge` the CTA branch or PR #300: no byte harm (Assets identical, measured), but it would import 23 superseded or NEEDS-CHANGES checker states as ancestry and would read as merging PR #300 | Re-run Gate M **once** on the integrated HEAD (S-3) |
| 3 | `main` (optional, docs freshness) | `c0513d6a` or later | Docs/specs only (59 paths, measured) | Coordination docs; no production effect | `merge-tree` clean (measured). Must not touch `active-tasks.md` content semantics. Manager-owned files arrive read-only | None for Unity. Integrator must still read live `main` coordination |
| 4 | Integration-specific verification (NEW, needs authorisation) | — | A small **integrated byte-identity witness** (Git-only; see §5) **or** a Manager-authorised stage-3 inventory amendment for the two guidance checkers | Not yet authorised. Either option is a Manager decision (D-1) | Changing guidance checker inventories is a source change and needs independent review | — |

**Excluded by contract:** PR #300 / whole CTA branch; old PR #265; superseded CTA heads `6adb2031`, `288ba386`, `c83fae1a`; Workbench harness `5085e8af` as a stand-alone pin (its bytes are replaced inside step 1); any unreviewed nearby HEAD; any change to packages, materials, saves, Reference Future v1 or Legacy40 fixtures.

---

## 4. Dry audit — #315 claim rechecked against the latest approved tips

### 4.1 Method and non-mutation proof (measured)

`git merge-tree --write-tree --name-only --messages` (Git 2.43.0), run with `GIT_OBJECT_DIRECTORY=<scratchpad>` and `GIT_ALTERNATE_OBJECT_DIRECTORIES=<store>/objects`, so any new tree objects land only in my session scratch directory. The shared store's `git count-objects -v` and a SHA-256 of `git for-each-ref` were identical before and after each trial batch. Nineteen trial objects were written to scratch only. No index, worktree, HEAD or ref was touched, and no source revision was checked out anywhere. (Between batches, the store's loose count rose from 3969 to 3993 because of my own `git fetch`, not because of merge-tree.)

| Trial | Result tree | Conflicts |
|---|---|---|
| guidance `9789369d` × Workbench published `03b484d1` | `2501e03a…` | none |
| guidance × Workbench tested `a4019e6b` | `4ebbc37f…` | none |
| guidance × CTA `6adb2031` (diagnostic only) | `9908a7c8…`, `Assets` = guidance `3547317e…` | none |
| guidance × CTA #330 `990ec43e` (diagnostic only) | `0e47308b…`, `Assets` = guidance `3547317e…` | none |
| Workbench × CTA (`03b484d1`×`6adb2031`; `a4019e6b`×`990ec43e`) | `d930ae9f…`; `7f92e897…` | none |
| guidance × `main`; Workbench × `main` | `764ac66e…`; `5ad4ab9a…` | none (docs only) |

**#315's "clean guidance + Workbench textual merge" still holds at the latest approved tips.** Byte-identity witness on `4ebbc37f`: 67/67 guidance paths equal `9789369d`, 17/17 Workbench paths equal `a4019e6b`, and the union is exactly 84 = 67 + 17 (measured).

### 4.2 Collisions textual merge cannot see — future Manager/integrator decisions

| ID | Collision | Basis | Consequence | Decision owner |
|---|---|---|---|---|
| **S-1** | `run_five_rack_check.py` `PROTECTED` includes `Assets/Scenes/ForestTest.unity` ("unchanged: …"), and "only allowed files changed since base" uses a fixed `ALLOWED` set. Workbench changes the scene (+1 line) and adds 17 paths; CTA adds 8 tool paths | Derived (source read at `9789369d`, lines 108–118, 361–389) | On the integrated tree, the five-rack suite exits **FAIL by design**. This is not a product regression, but it is not a PASS either | Manager **D-1** |
| **S-2** | `run_headland_check.py` `check_stage_inventory`: `changed_production(BASE) == STAGE1 ∪ stage2` ("world/save/racks/Reference/Mini consumers untouched") | Derived (lines 64–84) | Workbench's 5 `.cs` files and the scene make it **FAIL by design** | Manager **D-1** |
| **S-3** | CTA checker and its suites read the live tree (`check_baseline.py` lines 164 and 252; tests read `LearningObjectivesView.cs` and others from `REPO`; Roslyn compiles the player-like `Assets` set) | Measured: 12 `Assets/ForestPrototype` `.cs` differ between CTA-tested `990ec43e` and trial `4ebbc37f` (5 Workbench, plus `LearningObjectivesView.cs` with 1 line, `TeachingDiagram{,Plan}.cs`, `StandMap{Source,DrawPlan,Model,Legend}.cs`). `ScenarioOneUiRoot.cs`, `RackEditing.cs` and `RackGeometry.cs` are identical | #330/#329 tallies **cannot be reused** for Gate M on the integrated HEAD. One re-run is needed: full checker, Roslyn suite, lexical suite. No corpus expansion | Integrator (after Manager CTA approval) |
| **S-4** | Workbench runner R1 fails on any tracked package/material drift after Editor exit. The Editor resolves Linux SDK packages to 1.1.1 against tracked 1.1.0 and rewrote 5 SectionFive `.mat` files on #287 | Cited (#329 G4; #287 result) | **Any** Editor run on a fresh integrated lane may fail R1, correctly, before Workbench gates are meaningful | Manager + **#333** package/provenance plan |
| **S-5** | `ForestTest.unity` is the one production file both read by guidance checkers (52 `Dirt Path` planks, `Forest Clearing`, starting-stand/ecology attachment) and changed by Workbench | Derived: Workbench's line is a `retiresFromStartingWorld` field on the workbench `ForestBuildable`, not the planks or clearing | Probably harmless to geometry parsing, but **only a run proves it**. The guidance checker's scene parse must run against the integrated scene bytes | Integrator |
| **S-6** | `NextActionGuidance/check_sources.py` (T2 stage checker): allowlist of 5 UI files since `e1be15c1` | Derived: at `9789369d` itself the production diff already includes `Map/*` and `TeachingDiagram*` (11 files) | A superseded stage checker. It fails at the approved guidance tip too, so it cannot be an integration gate. Cite #310 instead | Manager to confirm as historical |
| **S-7** | Stale hash/label provenance: #284 labels vs measured `94589C74…/92F96355…` (old start/Year-3) and `71481A02…/044C4578…` (new Enlarged80 start/Year-3); B3 P0604→P1209 | Cited (#287 §5, #329 G4) | Labels must not be silently rebaselined on the integrated HEAD | Manager |

---

## 5. A–O matrix on the exact future integrated HEAD

**Required head for every gate:** the single future integrated commit, made from steps 0–2 (and optionally 3) in §3 and recorded before any Editor start. No gate may cite another HEAD as its PASS.

**Shared-session groups:**
- **U0** = one cold import on that HEAD (one Library).
- **U1** = batchmode harnesses reusing the U0 Library.
- **U2** = one windowed native session (Xephyr, as #287 used) at 1280×720 and 1920×1080, reusing the U0 Library.
- **X** = offline/Git only.
- **R** = human or independent review.

| Gate | Antecedent evidence (different candidate, not a PASS) | Exact integration | Owner / harness (future) | 720p / 1080p / native | Legacy40 / frozen v1 / save | Workbench B3 / Work Plan / MFM | Group |
|---|---|---|---|---|---|---|---|
| **A** Import/compile | Unity: #287 cold import exit 0, 0 `error CS`, 6.6 GiB Library at `5085e8af` (**no D-072 UI**, SDK 1.1.1 drift). Offline: CTA Roslyn C#9 compile of 189 files at CTA tree | **NOT RUN** | Integrator; Editor log | — | Frozen-bytes SHA-256 before/after | — | U0 (once) |
| **B** V2 surface lifecycle (m-4) | Offline only: #303 headland 39 source + 92 offline at `9789369d` (#310) | **NOT RUN** | Integrator. No dedicated Unity harness for V2↔non-V2 load transitions found in this read-only scan (derived; **possible harness gap**) | — | V2/non-V2 save transitions | — | U1 |
| **C** Map geometry (m-2) | Offline only: #297 geometry tests | **NOT RUN** | `UxPedagogyShots` / Full Stand Map + 3D paired captures; m-2 dark-edge pixel check (harness status unverified) | **both, native** | Enlarged80 V2 | — | U2 |
| **D** Full-vs-Mini | Offline only: #297 Mini preservation | **NOT RUN** | `Tools/Verification/StandMap/StandMapParity.cs` + rendered side-by-side | both | Same saved world old/new | — | U1 + U2 |
| **E** Legend/paths (m-5) | Source only: #297 legend source approved | **NOT RUN** | U2 colour/grayscale captures + **independent observer** | both | — | — | U2 + R |
| **F** Performance (m-1) | **None** | **NOT RUN** | No D-072 Profiler harness identified (derived: Profiler used only by older `UxQualification`, `StandExpansionAudit`, `DensityQualification`). **Harness gap; needs authorised verification-only source** | native | Representative V2 forest | — | U2 |
| **G** Five-rack diagram | Source/offline: #310 at `9789369d`: five-rack 182 source / 109 structural / 165 painter / 29 mutations; integrity 80/4/3/29; headland 39 + 92; integrity 20 (cited) | **NOT RUN** (render). Source part blocked by S-1 | Render: `UxPedagogyShots` lesson captures; source: byte-identity witness (§6) | both, colour + grayscale | — | — | X + U2 |
| **H** Old-world diagram guard | Source: B-1 guard (#307/#310). Unity: `REFERENCE_FUTURE_V1_PASS` (with `CCF_STAND_GEOMETRY=0`) at `5085e8af` — **not a diagram check** | **NOT RUN** | U2 Objective 1 screens in Legacy40, frozen Reference v1 preview and pre-V2 save | both | **Legacy40 + frozen v1 + pre-V2** | — | U2 |
| **I** Card fit / cues | Source: #295 approved; M-1 card budget (source) | **NOT RUN** | `ScenarioOneRackUx/GuidedOnboardingVerification.cs`, `ScenarioOneUxPedagogy/UxPedagogyShots.cs`; measured card bounds | **both, native** | — | — | U2 |
| **J** Native controls / safe default | Unity: `RACKV2_O1E2E_PASS checks=173 shots=24` at `5085e8af` (**no D-072 cues/map/diagram**) | **NOT RUN** | `RackV2Objective1/RackV2Objective1EndToEnd.cs` + focus/consent trace | **both, native** | Real `Save/CanLoad/Load` | Work Plan approve/execute in flow | U2 |
| **K** Work Plan / MFM boundary | Unity: `MFM08A_UNITY_PASS checks=57` (149-task Work Plan) at `5085e8af` | **NOT RUN** | `MfmCoverageWorld/MfmCoverageWorldUnityGate.cs` (Workbench-modified bytes) | — | Old/new management states | **Work Plan + MFM non-commitment; six-vs-seven Site masks** | U1 |
| **L** Workbench retirement | **Antecedent FAIL:** `WorkbenchRetirementVerification` 35 PASS / FAIL at check 36; C–G NOT RUN at `5085e8af`. Diagnostic copies only: 103 PASS. Approved verifier `a4019e6b`: offline 41/41 runner tests, 13 mocks, 28 projection controls (#329, cited) | **NOT RUN.** Never run in Unity with approved verifier bytes | `WorkbenchRetirement/run_workbench_retirement.py` (R1–R5 receipt protocol; first-line FINAL marker required) | Former-site native witness 720p/1080p (**harness gap**: #287 found none) | Reference v1 preview enter/exit; Legacy40 accounting | **B3 P0604→P1209 unresolved** | U1 (+U2 witness) |
| **M** CTA checker integrity | Offline: #330 author at `990ec43e`: 10/0/2, A329-01 → 9/1/2, 884/884 + 112/112, 6/4/2 red, 320 refs, SDK 8.0.133, 6 SnapFar (cited; **author only**). #329 at `288ba386` (superseded) | **NOT RUN; CTA SOURCE WAITING #331 + MANAGER G2/G3** | `OvernightPlaytestFeedback/check_baseline.py` + both test suites, once (S-3) | — (source-static) | — | — | X |
| **N** Save/load compatibility | Unity at `5085e8af`: frozen Reference v1 bytes identical before/after; `REFERENCE_FUTURE_V1_PASS` + continuation; `Enlarged80Verification` FAIL (stale v23 pin); `RackV2PersistenceVerification` v2 FAIL at entry; legacy40/coldstart NOT RUN | **NOT RUN** | `StandGeometry/Enlarged80Verification.cs`, `RackV2Persistence` (v2/legacy40/coldstart), `ScenarioReferenceVerification.cs` (`CCF_STAND_GEOMETRY=0`), isolated `XDG_CONFIG_HOME` | — | **Legacy40 + frozen v1 + Enlarged80 V2 + old v19/v20/v24 fixtures; no user saves** | Enlarged80 return-to-start is the **B3 symptom** | U1 |
| **O** Final integrated regression | **None** (#316 waiting) | **NOT RUN** | Independent integration reviewer + Manager verdict | Evidence pack covers both | All | All | R |

**Gate count (by antecedent type):**
- **Executed on an exact integrated candidate: 0 of 15.**
- **Unity runtime antecedent passes on a different candidate (`5085e8af`): 4** (A, J, K, N-partial). None includes D-072 UI.
- **Source/offline-only antecedents: 8** (B, C, D, E, G, H, I, M).
- **Antecedent FAIL: 1** (L).
- **No antecedent at all: 2** (F, O).
- **Never run in Unity on any tree containing the D-072 UI or approved Workbench verifier bytes: 15 of 15.**

**Expensive work shared on one HEAD:**
- **One** cold import (U0) serves every Unity gate.
- **One** batchmode block (U1) covers B, D-parity, K, L, N and Tier-1 B3.
- **One** native windowed session (U2) at both resolutions covers C, D-capture, E, F, G-render, H, I and J.

#287 already showed that a single Library can be reused across all gates on one HEAD. No second cold Library is needed, provided the HEAD does not change. **Any HEAD change invalidates U0–U2.**

**No Windows/Player build** until the user has personally playtested a Manager-approved exact candidate and has explicitly authorised a build afterwards.

---

## 6. Proportionate tiers — cut redundant work, not safeguards

### Tier definitions (all on the single future integrated HEAD)

| Tier | Contents | Entry | Stop rule |
|---|---|---|---|
| **Tier-1: short Workbench B3 diagnostic** | U0 import, then a bounded, authorised logging run around Enlarged80 `ResetWorld` return-to-start: player pose, clearance inputs, scene reload and tree IDs before and after. Design comes from #333 (§8). Never rebaseline or edit trees | Manager source approval + ops admission (storage, packages, lease) | If B3 shows production nondeterminism → **STOP** before Tier-2 and return to Manager. A pose-explained verifier artefact → Manager rules |
| **Tier-2: focused feature qualification** | A, L, K, N, J, M, plus D-072 I/G/H/C captures | Tier-1 not stopped | Any FAIL → STOP and report. No waivers |
| **Tier-3: full A–O after candidate** | Remaining B, D, E, F, then O independent review and Manager verdict | Tier-2 complete | Independent reviewer + Manager **APPROVED / NEEDS CHANGES / REJECTED** |

Putting Tier-1 on the integrated HEAD rather than on the dirty registered Workbench lane is a **proposal** (D-3). Its basis is measured: Workbench production bytes in the integrated tree equal `a4019e6b`'s exactly. This avoids both a HEAD switch of the protected dirty lane and a second Library.

### Reuse ledger

| Category | Items | Basis |
|---|---|---|
| **Reusable byte-identical evidence (cite, do not re-run)** | #310 five-rack/headland suites: diagram, map and checker bytes are identical on the integrated tree (67/67 guidance paths). #329 Workbench offline runner evidence (41/41, 13 mocks, 28 projection controls): tool bytes identical and **every `Assets/` input the tests read is identical** (measured: 5/5 referenced paths; the only Assets differing `a4019e6b`→trial are guidance's 11 UI files). #329/#330 adversarial histories (A326/R321 witnesses, historical controls) as history only | Measured byte-identity (`byte-identity-witness.txt`) |
| **Must re-run once on the integrated HEAD (source context changed)** | CTA full checker + Roslyn + lexical suites (S-3; 12 input files differ). An integrated byte-identity witness replaces running the guidance suites, which would fail by design (S-1/S-2) unless D-1 chooses a stage-3 amendment. `git merge-tree`/ancestry record of the actual integration commit | Inputs differ (measured) |
| **No repeated broad corpora** | Do not regenerate the 884/112 lexical corpus, the 654-case or 80-mutation integrity campaigns, or the 29 painter mutations for unchanged tool bytes. One run of the existing suites at the new HEAD is enough. No second cold Library | Packet §5 |
| **Never run; cannot be waived** | Unity compile with D-072 UI (A). All D-072 native renders (C, E, G, H, I). Profiler (F). Workbench C–G with approved verifier (L). Legacy40 and RackV2Persistence legacy40/coldstart (N). B3 causal witness. J/K at the integrated HEAD. Independent integrated review (O) | Cited handoffs: all say NOT RUN |

---

## 7. Measured vs derived evidence

| Measured (commands run, output in evidence dir) | Derived (source read, not executed) | Cited (published, not reproduced) |
|---|---|---|
| All branch heads; strict ancestry WB `4d95e452 ⊂ 6b7d2d45 ⊂ 2c4aea1c ⊂ a4019e6b ⊂ 03b484d1`, `6adb2031 ⊂ 990ec43e`. Merge-bases. Path deltas. Zero shared paths. 8 merge-tree trials, all clean, with non-mutation proof. 67/67 + 17/17 byte identity. CTA manifest blobs/SHA-256. 12 CTA-input `.cs` differ. Workbench test inputs identical. Main has no non-docs delta. PR #300 head/base/commits (`gh pr view`). Disk at 00:48:16Z: Files 25.98 GiB, ZX20 21.62 GiB. No `/Editor/Unity` or Unity Hub process | S-1, S-2, S-5, S-6 checker behaviour on an integrated tree. Harness gaps for B, C-m-2, F and the L native witness | #287 results at `5085e8af`; #310 tallies; #329 Workbench and CTA results; #330 author tallies; Manager rulings |

**NOT RUN by me:** every checker, every test suite, every Unity/Editor/import step, every native capture, every save/load. I made no source checkout, merge, cherry-pick or apply into any lane.

---

## 8. Conditional status, #333 linkage, stop conditions and Manager decisions

**CTA (#330/#331), final GitHub check at 2026-11-08T00:51:41Z (host `date -u`):** #330 is **[HANDED-OFF]** at 00:45:11Z (author Xiaomi MiMo-V2.6-Pro; tested `990ec43e`, published `9867144c`; [comment](https://github.com/jermur788/CCF/issues/330#issuecomment-6715211836)). #331 is **[RUNNING] since 00:48:41Z with no [HANDED-OFF]** (independent reviewer OpenAI GPT-6.1 Sol, reasoningEffort high; [claim](https://github.com/jermur788/CCF/issues/331#issuecomment-6715245496)). No #331 result exists, and there is no Manager G2/G3 verdict. **CTA SOURCE WAITING #331 + MANAGER G2/G3.** This plan is not approved.

**#333 (storage/packages/tree), same check:** **[RUNNING] since 00:39:49Z, no [HANDED-OFF] and no published result** ([claim](https://github.com/jermur788/CCF/issues/333#issuecomment-6715162789); branch `docs/ccf-unity-provenance-readiness-20261108-v1` not on origin at 00:51Z). Its inputs are therefore **MISSING/CONDITIONAL**: the package 1.1.0/1.1.1 provenance plan (S-4), the verification of the eight protected dirty files, live Library and storage options, and the Tier-1 B3 pose/clearance diagnostic design. Tier-1 in §6 is a placeholder until #333 publishes and Manager rules on it.

**Stop conditions for any future integration implementation:**
1. Manager G2/G3 CTA approval is absent → transfer no CTA files, and Gate M stays NOT RUN. If Manager accepts the integration without M, it must say so explicitly; an unapproved checker must never count as M.
2. No Manager-registered integration owner, branch or worktree in `active-tasks.md` → no integration.
3. Storage below the 27 GiB cold-import entry (ZX20 21.62 GiB measured) or any package-provenance plan missing → no U0.
4. No exclusive Unity machine lease → no Editor.
5. Any HEAD change after U0 → U0–U2 evidence void.
6. Tier-1 shows production B3 nondeterminism → stop.

**Decisions only Manager #200 can make:**
- **D-1:** S-1/S-2 handling: (a) byte-identity witness + cited #310 (recommended: no source change, measured identity), or (b) a stage-3 inventory amendment with independent review.
- **D-2:** CTA verdict after #331.
- **D-3:** Tier-1 B3 on the integrated HEAD vs the registered dirty lane.
- **D-4:** Storage and package provenance option (#333).
- **D-5:** Include optional `main` docs merge step 3 or not.
- **D-6:** Authorise verification-only harnesses for the gaps in B, C-m-2, F and the L native witness, or accept them as explicit NOT RUN.
- **D-7:** #316 independent integration reviewer assignment.

**Proposed future implementation packet (only after D-1…D-7 and all entry gates):** "Integrator: on a new Manager-registered `integration/scenario-one-focused-…` branch from `9789369d`, merge `03b484d1`, transfer exactly the 8 CTA tool paths from the Manager-approved CTA head, record the integration SHA and a byte-identity witness, then run Tier-1 → Tier-2 → Tier-3 in one U0 Library under an exclusive lease. Publish one A–O evidence table, then hand to #316." **This packet is a proposal only. It is not created, reserved or authorised.**

---

*No acceptance is implied. Untouched: frozen Reference Future v1, Legacy40, user saves, packages, materials, Libraries, `active-tasks.md`, other workers' lanes, the dirty primary `AGENTS.md` / `Docs/Operations/StorageArchitecture.md`. No Windows/Player build.*
