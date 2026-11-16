# Sen-1 #626 — Linux-local Compact30 core implementation admission: read-only result

**Status: HANDED-OFF — READ-ONLY AUDIT. Unity NOT RUN. No lease acquired. No source, scene, asset, save or Prefs change.**

| Field | Value |
| --- | --- |
| Claim | [#626](https://github.com/jermur788/CCF/issues/626), parent Overall Manager #425 and Scenario One delivery #424. Paired with browser [#625](https://github.com/jermur788/CCF/issues/625) (handed off) and geometry [#622](https://github.com/jermur788/CCF/issues/622) (READY / NOT RUNNING). |
| Worker | Anthropic Claude Opus 5.5 (`claude-opus-5-5`), Claude Code harness in T3 Code. The packet suggested GPT-6.1 Sol High / Codex High; this is a different provider, recorded on the issue before any write. |
| Host | `jer-laptop`, Linux Mint 22.3 (Ubuntu 24.04 base; the [RUNNING] comment said "Ubuntu 24.04", which is the base, not the distribution name), kernel 7.0.0-42-generic, x86_64. |
| Worktree | `/media/jer/Files/CCF/Operations/CCF-626-Compact-Admission-Audit-20261116` (docs-only, Files ext4, no `Library/`, not a registered Unity lane). |
| Branch / starting HEAD | `docs/sen-1-626-linux-compact-admission-audit-20261116-v1` at `37cd047ed39fd649563f70602d458e2970f6780f` (= `origin/main` throughout this audit). |
| Other source line inspected | `origin/feature/scen1-shared-three-profile-save-v2` at `09ade4c4` (the #544 shared-save line, T-01 → T-03A). |
| Write allowlist | This one file. |
| Observation window | 2026-11-16, final pre-write check 17:20Z, boot id `35271dae…`. |

**How to read this report.** Each fact is labelled with how I know it:

- **CODE** — read in source at the cited commit and line.
- **HOST** — observed directly on this machine (files, process table, disk).
- **SPEC** — a design contract (DRAFT or signed), not implementation.
- **INFERENCE** — my reasoning from those facts.

Nothing in this report is a runtime, Editor, collision, walkability or save/load PASS.

---

## 1. Answer in plain English

1. **The machine can host the work, but the operations layer is not clean.** Unity 6000.6.0f1 is installed and matches the project pin. Nobody holds the global Unity lease and no Editor is running. But the lease tool's repair (#548) is **not deployed**: its self-test is still red. The live Ops board has been stale since 13 Nov (#553), and no lane is registered for Compact work. Any native Tier-2 run needs a Manager admission that names these gaps.
2. **There is no Compact world in source, only reserved names.** Main knows two stand geometries (Legacy40 = 0, Enlarged80 = 1). The unmerged #544 save line reserves Compact identities and the number 2, but keeps Compact **unavailable** by design.
3. **The 14 H1 tree IDs and the 189-stem count are not reproducible from production code.** They come from `C-B-30`, which its own file labels a *"TEMPORARY RUNTIME DERIVATIVE: NOT A SAVE/PRODUCTION MODEL"*. It was produced by running the legacy lattice generator inside ForestTest, with ForestTest's plank and clearing exclusions and a temporary 2.4 m rack corridor. A distinct Compact scene would generate a *different* omission set. The signed world must therefore be **authored data** (IDs and positions), not "re-run the generator in a new scene".
4. **The failed-guard O1 overlap is visual, not physical, in current source.** The guard tube's collider is destroyed when the tube is created, so a leaning tube cannot block the player today. The 0.141 m intrusion remains a legibility and footpath-design problem. It is still not natively measured.
5. **Recommended first Linux implementation task:** a **zero-behaviour-change geometry dispatch hardening** (L-1, §7). Every place that branches on "Enlarged80 or else Legacy40" becomes exhaustive and fails closed, so a third model can later be enabled safely. It needs no DRAFT gameplay decision and no signed world. It can be verified mostly offline (Tier-1), plus one short native regression. Everything that creates a scene, enables Compact or adds teaching stays gated, as listed in §8.

---

## 2. Physical Linux host, Unity and CCF-Ops (HOST unless marked)

| Item | Observation | Consequence |
| --- | --- | --- |
| Project pin | `ProjectSettings/ProjectVersion.txt`: `6000.6.0f1 (f7f8ed4d1e24)`. **CODE** | — |
| Installed Editor | `/media/jer/ZX20/Unity/6000.6.0f1/Editor/Unity` present. Managed reference DLLs present (`Editor/Data/Managed/UnityEngine.dll` and 502 module DLLs). | The version matches. The reference DLLs allow an offline Roslyn typecheck with **no Editor launch**. |
| URP / Input / Test pins | `Packages/manifest.json:9` URP `17.6.0` (lock `packages-lock.json:121-122` 17.6.0, depth 0); `:8` Input System `1.20.0`; `:12` test-framework `1.8.0`. `QualitySettings.asset:51,105` assign URP assets (GUIDs `5e6cbd92…`, `4b83569d…`); `GraphicsSettings.asset:40` default is null, so URP comes per quality level. **CODE** | A Compact task must not touch Graphics/Quality/URP assets; spec P2 already forbids it. |
| Unity processes | `pgrep -x Unity` = 0 at 17:20Z. | Point-in-time only. |
| Global lease state | `/media/jer/ZX20/CCF-Ops/state/unity/lock.json` **absent**. `history.jsonl` has 415 lines; the last event is a `release` at 2026-11-15T22:44:25Z by `Codex-Ops548-durability` (lane `CCF-S1-UI-Presentation-V1-Qual`). Read as files only. I did **not** run `unity-lock status`, to avoid even the guard `flock`. | The lease was FREE at observation. This is **not** a reservation (Manager #424, #548, 07:56Z). |
| Live lease tooling | `/media/jer/ZX20/CCF-Ops`, `deployment.json` schema 2, `provenance_status: RECONCILED_OBSERVATION_NOT_SOURCE_ACCEPTANCE`. Live `_ccf.py`, `lane-status`, `environment.sh`, `README.md` and `AGENTS.md` are pinned **overrides** of the historical source `dfe445e5`. The #548 candidate (`ops/unity-lease-persistent-identity-20261115-v1` @ `60f97b06`, worktree `CCF-OpsLeaseFix-20261115`, HEAD `343ae990`) is `deployment_status: NOT_DEPLOYED`. #548 remains **NEEDS CHANGES**: the full self-test exits 1 after 33 checks. | The live lock still functions; history shows completed acquire/release cycles through 15 Nov. Its device check now uses a persistent `/dev/disk/by-id/…` path in live `lanes.json`. But the repaired, independently tested version is not live. **Manager must decide** whether a Compact Tier-2 may run on the live pinned tooling, as #460 and #490 did. |
| In-repo `CCF-Ops/` | `/media/jer/Files/CCF-Primary/CCF-Ops` (tracked on `ops/workflow-foundation-v1`; last commit `ad176b66`, 2026-10-21). Its `lanes.json` still names `/media/jer/ZX20/CCF-Repository.git`. | Historical source only. **Do not run it**; the live deployment is `/media/jer/ZX20/CCF-Ops`. |
| Live board | `/media/jer/ZX20/CCF-Ops/board/active.md` last modified 2026-11-13T15:49:55Z. It has 0 mentions of #544/#551/#626 and 1 of "compact". | Confirms #553 (BLOCKED): the physical board is stale. |
| Registered lanes | Live `lanes.json` (modified 2026-11-13T20:27Z): 16 lanes, repository `/media/jer/Files/CCF/Repository.git`, storage UUID `18D6-896E` exFAT, floor 15 GiB. **No Compact lane.** | A new lane registration is an Ops-owner write; this audit did not and could not make it. |
| Lane condition (`git --no-optional-locks`) | `CCF-U1` feature/standmap-species… `e7f086a1`, **8 dirty**, Library 6.8 G. `CCF-U2` feature/scenario-two-species… `e26516ff`, clean, **no Library**. `CCF-U3` qualify/…annual-review-reset-370 `4db486db`, **8 dirty**, Library 6.8 G. `CCF-S1-Objective1UX-20261112` `acb4e111`, clean, Library 6.6 G (#443 input, protected). `CCF-Q10` 2 dirty, no Library. `CCF-Integration` 1 dirty. | U1/U3 hold someone's uncommitted work: **never borrow**. No existing lane is both clean and unassigned with a warm Library. Expect a cold import (memory of earlier runs: tens of minutes, black captures until warm). |
| Storage | ZX20 `/dev/sdd2` 477 G, **52 G free** (floor 15 GiB). Files 73 G free. Root `/` 24 G free (90%); the #548 "root 100%" report no longer holds at this time. | One more Library (~6.7 G) fits above the ZX20 floor. |
| Player profile | Live profile `~/.config/unity3d/DefaultCompany/CCF/` (`ProjectSettings.asset:15-16` company `DefaultCompany`, product `CCF`). It holds `prefs`, `forest-save.json.bak`, `save-backup-20261104/`; **no** live `forest-save.json`. | Any native run must use a fresh per-run `XDG_CONFIG_HOME`, as `Tools/Verification/RackV2Persistence/run_gate.py:15,118` does. That runner claims this isolates `persistentDataPath`; I did not verify where Linux PlayerPrefs go under that override. |
| Licence | The Unity Hub Flatpak copy `~/.var/app/com.unity.UnityHub/config/unity3d/Unity/licenses/UnityEntitlementLicense.xml` exists (mtime 2026-11-10). | Presence only; validity not checked. Earlier runners failed (exit 198) on a stale `$HOME` licence, so pre-seed from the Hub copy. |
| Offline toolchain | `dotnet` SDK 8.0.133 and 10.0.114. `unity_typecheck.py` from T-02R/T-03A exists under `/media/jer/Files/CCF/Verification/SharedSaveT0{2R-563,3A-570}-20261116/`. | Tier-1 (offline harness + Roslyn typecheck against Unity 6000.6 DLLs) is possible **without** a lease. |
| Concurrent physical workers | #627 [RUNNING] (Claude Opus 5.5, same host): Oak scratch asset plus *Unity/Blender resource coexistence measurements*. #628 [RUNNING] is browser-only. | A Compact Tier-2 must check #627's lease and Editor use first. |

**Ops verdict:** **NOT ADMITTED for native work as of this audit.** It is not blocked by hardware or version. It is blocked by three Manager/Ops decisions: (a) the #548 tooling acceptance, or an explicit exception to run on the pinned live tooling; (b) a #553 board sync, or an explicit exception; (c) a registered clean Compact lane with its own Library and its own per-run profile. Tier-1 offline work needs none of these.

---

## 3. Source seam matrix — what exists, what is a contract, what is missing

Main is `37cd047`; the save line is `09ade4c4`. Main's 35 commits since the save line's merge base `f52dec24` change **no** `Assets/`, `ProjectSettings/` or `Packages/` path. None of the save line's 12 changed `Assets/` files has been touched on main since (**CODE**, `git diff`). That save line is **source-accepted but not merged and in no playable candidate** (#544 HANDED-OFF comment, 01:15Z).

| Seam | Code truth (CODE) | Design contract (SPEC) | Missing for Compact (INFERENCE) |
| --- | --- | --- | --- |
| Build scenes | `EditorBuildSettings.asset:8-9`: only `Assets/Scenes/ForestTest.unity`. Other scenes exist (`ForestryAssetReview`, `MixedSpeciesTest`, `SampleScene`) but are not in the build. | Parent plan P0 (#604): **proposed** neutral no-world title scene at build index 0, ForestTest and a Compact scene routed separately. Candidate path `Assets/Scenes/ScenarioOneCompact30.unity` "only if path confirmed". DRAFT. | A title scene, a Compact scene, BuildSettings entries, and a scene ↔ profile map. All are Unity-YAML, single-writer, need a lease. |
| Session menu lifetime | `UI/StandaloneSessionMenu.cs:28` `[RuntimeInitializeOnLoadMethod(AfterSceneLoad)]`, `:37` `DontDestroyOnLoad(host)`, `:148` `Restart()` → `SceneManager.LoadScene(GetActiveScene().buildIndex)`. Unchanged on the save line (`:164`). | P0: dedicated boot-only picker; Restart recreates the selected profile's scene without the title. | A picker that does not depend on gameplay objects. Restart must route by profile, not active build index. |
| Profile registry | **Main: none.** Save line: `ForestSaveProfiles.cs:9-24` (chapter/profile/prefix literals), `:58-60` Historical (`forest-save.json`, available), **`:62-66` Compact** (`o1-compact-v1` / `scenario-one-compact-v1` / `compact30-v1` / `scen1-compact-h1-g1-v1` / `forest-save-scenario-one-compact-v1.json`, `IsAvailable = false`, reason "no signed authored world binding is installed"), `:68-72` Scenario Two (unavailable). | Save spec V2 `200add4e` §4.1 `:148`: the Compact row is **PROPOSED**. `:259` B-08 and `:262` B-11 are **AWAITING SIGNATURE**. The save-line comment `ForestSaveValidation.Geometry.cs:22` (`09ade4c4`) states that "the signed contract reserves the number 2". I did not locate that signature record myself. | Flipping Compact's availability is the **final** step, not the first (see next row). |
| Profile seal at launch | Save line `ForestSaveSessionContext.cs:81-93`: auto-selects only when **exactly one** profile is available. `:104-110` `[BeforeSceneLoad]` calls `TryBeginWorldCreation` and **throws** if nothing is selected. `:17-23` `TrySelect` refuses after world creation has started. `:50-68` `TryAuthorizeWorldConstruction`, called from `ScenarioOneManager.cs:466` (save line). | Code comment `:4-5`: "T-03 can create a fresh context for a staged destination". | **Availability-flip trap (#550 F-3).** If Compact becomes available before an explicit picker exists, two profiles are available, auto-selection returns an empty context, and the BeforeSceneLoad seal **throws on every launch, breaking historical O1**. A title scene also needs the seal to happen at gameplay-scene entry, not at app launch. The staged-context API does not exist yet (`Current` has a private setter). |
| Save paths | Main: `ForestSaveController.cs:9,15,17,19` hard-codes `forest-save.json` (+ `.tmp` and `.bak`). `StandaloneSessionMenu.cs:77` has a **second** hard-coded `forest-save.json`, used at `:123,125` and `Startup.cs:156`. Save line: both route through `ForestSaveSessionContext.CurrentPaths` (`ForestSaveController.cs:169-173`, `StandaloneSessionMenu.cs:77`). | P4 / ISO-07/08/42/44/48/54/55, ISO-15: preflight before scene, Prefs or file mutation. | Already solved on the save line; **the save line must merge before any Compact path work**, or the work forks the save system. |
| Save version / newer save | `ForestSaveData.cs:7` `CurrentVersion = 24`. `ForestSaveController.cs:346-347` loads a **newer** version best-effort with a warning. | W-03/FR-07 explicit foreign/newer refusal. | Save line T-02/T-02R adds admission refusals (not re-verified here). |
| Stand geometry | `StandGeometryModel.cs:15-17` Legacy40 = 0, Enlarged80 = 1, `Latest` = Enlarged80. `:32` `IsKnown` accepts only 0/1. **`:34` `StandSizeMeters` returns `80f` if Enlarged80, else `40f`.** `:60` `ForSave` treats saves before v20 as Legacy40. `:72` `NewGameModel = Enlarged80`; `:76-110` Editor-only `CCF_STAND_GEOMETRY` override. Entry gates refuse unknown values: `ForestEcologyController.cs:94`, `ScenarioOneDefinition.cs:117`, `ForestSaveValidation.Geometry.cs:18`, `ForestSaveController.cs:302`. Save line adds `ForestSaveValidation.Geometry.cs:24,37` `ReservedCompactGeometryModel = 2`, refused. | B-11: Compact30 = 2 is a **numeric dispatch reservation only**, with "exhaustive geometry/generator/consumer dispatch and unknown rejection" (GAP MUST CLOSE). 30 m / 36 cells are proposed, outside that signature. | Besides `StandGeometryModel.cs` itself, **18 files** branch as binary "Enlarged80 vs. otherwise" (pattern-based inventory; the L-1 author must re-inventory implicit uses such as `CellsPerAxis`): `StandApproachPath.cs` (7), `ScenarioOneObjectives.cs` (3), `ScenarioOneManager.RackPlan.cs` (3), `ForestStartingStand.cs` (3), `StandWorldBoundary.cs`, `ScenarioReferenceArchive.cs`, `ScenarioOneSuggestedRackLayout.cs`, `ScenarioOneDefinition.cs`, `ForestEcologyController.cs` (2 each), plus `StandMapView.cs`, `StandMapTreatmentReview.cs`, `LearningObjectivesView.Sequence.cs`, `ScenarioOneRackLayout.cs`, `ScenarioOneManager.RackEditing.cs`, `ScenarioOneManager.PrescriptionApproval.cs`, `RackPlanner.cs`, `ForestSaveValidation.RackV2.cs`, `ForestSaveValidation.Geometry.cs` (1 each). The moment `IsKnown(2)` becomes true, every one of them silently treats Compact as Legacy40 (or E80). **This is the L-1 target.** |
| Starting stand | `ForestStartingStand.cs:52-70` Awake: skip if trees exist, otherwise `ApplyStandGeometry(NewGameModelForSession)` then `Generate()`. `:84-86` E80 branch. `:88-…` legacy lattice: `P{r:D2}{c:D2}` IDs, `FnvJitter(id)`, clearance omission **before** the mortality budget. `:325-345` `CollectClearancePoints` reads scene objects named `Dirt Path*` (`:330`) and the clearing. Save line: `ForestStartingStand` has **no** construction-gate call (only `ScenarioOneManager.Awake` is gated). | P1/P3: Compact geometry from a signed manifest; "source-authored historical exclusions instead of fall events". | A Compact generator that **reads a signed roster**, not scene objects. Note the ungated generator path for the later router task. |
| Stand Map / approach | `UI/Map/StandMapSource.cs:15,45,341` builds paths from `Dirt Path` planks. `StandApproachPath.cs:10-27` clones `Dirt Path 00` / `Dirt Path X…`. `ScenarioOneHeadlandPresentation.cs:30` `GameObject.Find("Dirt Path 00")`, added unconditionally by `ForestStartingStand.Awake:54-55`. `Editor/ForestSceneBuilder.cs:513`. | P2/P3: Compact must avoid the 2.6 m planks and the 20 H1 planks; Stand Map consumes the Compact route. | A Compact route source that is not plank-named. The headland component must **not** auto-attach in Compact. |
| Rack-dependent flow | `ScenarioOneManager.PrescriptionApproval.cs:255-257` refuses without a confirmed Rack Planning V2 plan; `:260-261` derives rack geometry from stand size; `:339` reads `Dirt Path`. | P3: Compact rack/MFM UI does not apply; prescriptions refuse or omit, with no Compact bypass. | An explicit Compact branch in the refusal text and UI hiding (not a bypass). |
| Work Plan | `ScenarioOneManager.cs:1169-1183` `RemovePendingOrder` (rack orders refused). **`:1196-1219` `CancelApprovedOrder`** removes the Approved order, emits `OrderCancelled` and frees cash/stock. `:1221-1250` `ApprovePendingWork`. `:377` `SetPendingFellingOutcome`. UI: `WorkPlanView.cs:587-593` shows **"Remove"** for Pending and **"Cancel approval"** for Approved. `ScenarioOneWorkOrder.cs:13-19` statuses Pending/Approved/Completed/Failed (no Cancelled status; cancellation deletes the order and records an event). | OPEN-C8 W-A/W-B (#625 §W). | Confirms #625: "Approved is edit-locked" is **not** current behaviour. Cancel approval exists, and the batch setters (next row) change open orders. |
| Planting execution | `ScenarioOneManager.cs:875` `SetPlantingExecution` and `:884` `SetPlantingShelters` apply to **all** open planting orders (batch). | CIP-016 requires **per-tree** actor and protection. | Per-order setters plus a Compact rule for Approved orders. Not L-1. |
| Annual review acknowledgement | `ScenarioManagementEvent.cs:16` `AnnualReviewAcknowledged = 9`. `ScenarioOneManager.cs:391-401` `MarkAnnualReviewSeen` records `year = CurrentYear` when `ShouldRecordAcknowledgement` allows (`ScenarioOneCompletionRules.cs:62`). `AnnualReviewView.cs:13,35` browsing an older year never acknowledges. | #625 T1-E: binding to covering report year Y* is a **proposal**; ISO-58 / CG-TRIG BLOCKED. | The event stores the year it was **recorded**, not the report year it **covers**. A covering-year binding needs either a new field (save schema) or a gate that forces acknowledgement in the right year. Owner decision first. |
| Test fixtures | No `.asmdef`, no `Assets/**/Tests`, no Unity Test Framework suites. `Tools/Verification/` holds 103 entries: disposable harnesses copied into `Assets/ForestPrototype/`, run in batchmode and removed (e.g. `ScenarioOneCompletionVerification.cs`, `SaveHardeningVerification.cs`, `ScenarioOnePlantingVerification.cs`, `RackV2Save/`, `Parity08Save/`). The save line's Tier-1 used an offline dotnet harness with stubs (T-03A: 748 PASS) and left the Roslyn typecheck **NOT RUN by the author** (T-03A handoff `:169-170`). | Plan tier discipline: Tier-1 fast, Tier-2 bounded feature, Tier-3 once at milestone. | A Compact Tier-1 can reuse the T-02R/T-03A offline harness pattern. Tier-2 harnesses must follow the copy/run/remove rule and use an isolated profile. |

---

## 4. Identities and content: what is signed, what is not

| Item | Status | Source |
| --- | --- | --- |
| Product baseline: 30 × 30 m, no internal rack, all-Sitka start, H1 ≈ 12 × 10 m south entry, **G1 = 2 sessile oak + 2 European beech** | **Owner-ratified product direction.** #424 convergence: "C2/C9: Owner already fixed baseline and count; do not re-ask." Still recorded inside DRAFT specs; canonical D-083 is pending at #585. | #424 convergence handoff (`docs/sen-1-424-implementation-gates-convergence-20261116-v1`) |
| Save identity literals for O1-historical and O2 | Signed (T-01 review #550). | Save spec V2 §4.1 `:147,149` |
| Compact profile/world/geometry/content keys, Compact30 = 2 | Reserved in source, unavailable. Spec row **PROPOSED**; B-08/B-11 **AWAITING SIGNATURE** in `200add4e`; the save-line comment claims the "2" reservation is signed. | §3 above |
| Compact dimensions / cells (30 m / 36) | **Not signed** ("outside this signature", spec `:282`). | Save spec V2 |
| H1 14 omitted IDs, 8 buffer IDs, 175 remaining, Route A 8 anchors, G1 four positions | **Unsigned.** #622 READY / NOT RUNNING. #576 manifest DRAFT. | §5 |
| World content hash / roster | **None exists.** I have not invented one. | — |

---

## 5. H1 / Route A / G1 inputs: provenance warning

Used only as verification inputs. Nothing here signs them.

- **HOST** The #508 ledger `/media/jer/Files/CCF/Verification/scen1-508-EntrancePathPlanting-20261114/ledger.json` (24,746 bytes, sha256 prefix `aa10f54f2fac9ccf`) reports:
  - `H1.inside` 14, `H1.buffer1m` 8, `H1.remaining` 175;
  - `H1.path.samples` 8 (Route A), and every `conflicts_w1.2` … `w3.0` list empty;
  - `composition.living` 189 = north 99 + south 71 + `rackStems` 19;
  - `track.n` 52 (the ForestTest plank track).

  Its input `C-B-30.json` has sha256 `3e7c37ad…`.
- **HOST** `C-B-30.json` lives at `/media/jer/Files/CCF/Verification/CompactOneRackNativeFeasibility460-20261113T145748Z/native-20261113T152528Z/C-B-30.json`. It declares:
  - `geometryClass: "TEMPORARY RUNTIME DERIVATIVE: NOT A SAVE/PRODUCTION MODEL"`;
  - `generatorMethod: "unmodified ForestStartingStand.Generate legacy lattice branch with scratch instance inputs"`;
  - `size 30`, `lattice 16`, `target 189`, `seed 20260914`, `cells 36`, `rackZ -0.25`, `rackWidth 2.4`;
  - `area.corridor ≈ 72 m²`, `area.otherClearance ≈ 136.45 m²`.

  The harness `CompactOneRack460Native.cs:135-141` set `latticePerAxis` and `targetTreeCount` on the ForestTest generator and called `Generate()`.
- **CODE + INFERENCE** The legacy lattice omits clearance slots first, from scene objects named `Dirt Path*` and the clearing (`ForestStartingStand.cs:100,325-345`), and only then fills the mortality budget from the lowest-hash slots. So the 189 surviving IDs, and therefore which 14 fall inside H1, **depend on ForestTest's plank/clearing objects and the #460 temporary rack corridor**. A distinct Compact scene without those objects, as the spec requires, would produce a different survivor set from the same generator. Also, the 19 `rackStems` belong to a one-rack world, not the ratified no-rack world.
- **Consequence for #622 / P1 / P3.** The signed manifest must list the actual tree IDs, positions and species as **authored data**. The Compact generator must consume that list, with a recorded content hash. It must not regenerate from the lattice and "expect" the #508 IDs. If the owner wants the #508 IDs exactly, #622 must freeze them from `C-B-30` as data and say so.

---

## 6. Guard representation compared with #583 / #588

| Aspect | CODE (`Assets/ForestPrototype/ScenarioProtectionVisuals.cs`) | #583/#588 model finding | What a native check must still measure after signature |
| --- | --- | --- | --- |
| States | Three states derived from `Browsing.Shelters`, nothing saved (`:4-8`). Effective = upright, coloured `:14`. Expired = upright grey `:15`. Failed = leaning brown `:16`. Selection at `:68-70`. There is **no separate "damaged"** state. | Same three. | — |
| Size | `TubeHeightM 1.2`, `TubeRadiusM 0.1` (`:11-12`), primitive scaled `:89`. | Same. | — |
| Failed lean | `:91-94`: `Euler(0, 0, 28°)` plus a world offset `(+0.25, −0.1, 0)`. The lean is **always toward world ±X**, whatever the path direction. From these constants, the horizontal extent from the shelter point is about −0.12 m to +0.62 m in X, and ±0.1 m in Z (my geometry, not native). | O1 −0.141 m into the 1.2 m core; O2 −0.155 m into the 0.20 m verge. | The rendered extent against the signed Route A edge, at signed G1 positions, at 720p/1080p. |
| Physics | **`:81-83` the primitive's collider is destroyed.** | Not stated as physics. | Whether a final guard design should gain a collider. If it does, the 0.141 m becomes a real walkability blocker. |

**INFERENCE.** In current source the O1 overlap cannot stop the player. It is a visual and readability intrusion into the walking core. The "never hide the O1 overlap with an art-only move" rule (#424) still applies, because the real fix is a signed position or lean rule. Not natively measured or fixed here.

---

## 7. Recommended first Linux implementation task: L-1 geometry dispatch hardening

**Why this one first.** Every later Compact step (generator, Stand Map, save router, picker) needs a third geometry value that existing code cannot mis-handle. Today 18 consumer files assume "not Enlarged80 means Legacy40" (§3). B-11 itself names exhaustive dispatch as the gap that must close. L-1:

- changes **no behaviour** for 0/1;
- needs **no DRAFT gameplay decision**, **no signed world** and **no scene**;
- can be verified mostly offline.

| Field | Proposal (for Manager #424 approval; not authorised by this report) |
| --- | --- |
| Base | `feature/scen1-shared-three-profile-save-v2` @ `09ade4c4`, or main **after** that line merges. `ForestSaveValidation.Geometry.cs` and `ScenarioOneManager.cs` already changed on the save line, so a main-based L-1 would conflict. |
| Scope | Replace each binary `== Enlarged80 ? … : …` (Legacy40 default) in the 18 consumer files with an exhaustive form: an explicit 0 branch, an explicit 1 branch, and a **fail-closed** default (log and refuse, or throw where the caller is already guarded by `IsKnown`). `StandGeometryModel.cs:34` `StandSizeMeters` and its siblings get the same treatment. **Do not add 2 to `IsKnown`**, do not add Compact dimensions, do not change `NewGameModel`. |
| Exact allowlist | `Assets/ForestPrototype/StandGeometryModel.cs` plus the 18 files listed in §3, geometry-dispatch lines only. No `.meta`, scene, prefab, asset, ProjectSettings or Packages change. No new files under `Assets/` (no new `.meta`). |
| Protections | Legacy40 / Enlarged80 / Reference Future v1 (frozen) / v1–v24 saves give byte-identical results. The `CCF_STAND_GEOMETRY` Editor override keeps rejecting unknown values. Rack V2 / MFM unchanged. |
| Tier-1 (no lease) | (1) An offline dotnet harness in the T-02R/T-03A pattern. For every touched function it asserts identical results for 0 and 1, and refusal for 2, −1 and `int.MaxValue`. (2) A Roslyn typecheck of all non-Editor `Assets/**/*.cs` against `Unity/6000.6.0f1` reference DLLs with player defines **and** `UNITY_EDITOR` defines (`unity_typecheck.py`). (3) A grep gate: zero remaining `? 80f : 40f` / `== StandGeometryModel.Enlarged80 ?` binary forms. (4) Mutants: re-introduce one binary fallback; the harness must fail. |
| Tier-2 (only after a Manager-admitted lease, §2) | One batchmode run in a registered clean lane with fresh `XDG_CONFIG_HOME`: the existing `ScenarioOneCompletionVerification` (compare its printed fingerprint explicitly; it only asserts a round trip), one E80 new-game generation count, one Legacy40 save load, and the pinned Reference continuation anchor from the regression runner. No screenshots, no UI matrix. |
| Review | An independent non-author source review of the diff plus the Tier-1 logs before Tier-2. |
| Approvals needed | #424 task approval and single-writer reservation on the save line (it is #544's subsystem, so #544's owner must agree or hand over). No owner gameplay choice is needed. |

---

## 8. Ordered Compact sequence after L-1, with gates

| # | Task | Gated on (all must be true before start) | Heavy Unity? |
| --- | --- | --- | --- |
| L-0 | Merge the #544 save line into main (no main-side `Assets/` overlap; §3). | #424/#425 integration decision; integration gate as the Manager defines it. Not a Compact change. | Integration-gate dependent |
| L-1 | Geometry dispatch hardening (§7). | #424 approval only. | Tier-2 short regression |
| L-2 | Compact geometry model 2 known, with **signed** dimensions/cells and a pure-C# Compact generator that consumes a **signed authored roster** (§5), not lattice regeneration. Starting stand gated by `TryAuthorizeWorldConstruction`. | B-11 signature; #622 signed manifest (IDs, positions, Route A, G1, guard remedy) with content hash; D-083 canonical (#585). | Tier-1 mostly; Tier-2 generation witness |
| L-3 | Neutral title scene, Compact scene, BuildSettings, staged profile context (seal at gameplay-scene entry), profile-routed Restart. Compact stays **unavailable** until the end of this task. | P0 scene/boot decision **APPROVED** (currently DRAFT, #604/#596); a scene single-writer with a lease; ZX20 lane registration (Ops owner). | Yes: Unity YAML + Tier-2 |
| L-4 | Stand Map / approach / prescription refusal for Compact (no plank names, no rack flow, no headland auto-attach). | L-2, L-3; parent spec P3 APPROVED. | Tier-2 map at 720p/1080p |
| L-5 | Flip Compact `IsAvailable`; save router admission and negatives (foreign / newer / corrupt / cross-world). | L-3 picker in place (avoids the §3 seal trap); B-08 signed; ISO set verified. | Tier-2 save negatives |
| L-6 | G1 2+2 per-tree planting, per-order actor/shelter, Approved-edit rule, covering-review acknowledgement, funding, completion. | Owner choices T1/T2, W-A/W-B, F-A/B/C, M-A/B (#625); CG-TRIG signed; ISO-58 unblocked. | Tier-2 two+ annual cycles |
| L-7 | Integrated candidate, Tier-3 once, owner personal Editor test. | All above. No Windows build without separate owner authorisation. | Tier-3 |

Art (#619/#620), Windows #611/#621 and D-032 final polish are **not** on this path. Binary imports still need legal source and provenance before they enter `Assets/`. The neutral built-in primitives that already exist in source (e.g. the guard tube) need nothing new.

---

## 9. Risks that the Manager and #622/#625 should absorb

1. **Availability-flip trap** (§3): enabling Compact before an explicit picker throws on launch and breaks historical O1.
2. **Roster is not reproducible** (§5): treat the H1 roster as authored data with a hash.
3. **Geometry fallback** (§3, §7): `StandSizeMeters` and 18 other files would make Compact a 40 m stand silently.
4. **Second save-path writer on main**: `StandaloneSessionMenu.cs:77` is independent of `ForestSaveController`. It is fixed only on the unmerged save line, so any Compact work on main's save paths forks the persistence owner.
5. **Ungated generator**: `ForestStartingStand.Awake` builds a world without the construction gate on the save line. Close this in L-2/L-3.
6. **Headland and plank coupling**: six source files (one Editor-only, `Editor/ForestSceneBuilder.cs:513`) locate world features by the name `Dirt Path`. A Compact scene must not contain look-alike names.
7. **Work Plan reality**: "Cancel approval" and batch setters exist (#625 confirmed independently here at the cited lines).
8. **Ops**: lease repair not deployed, board stale, no Compact lane, U1/U3 dirty with others' work, #627 sharing the host.

---

## 10. What was not done or not verified

- No Unity Editor or batchmode launch; no `unity-lock` invocation of any kind; no lane registration.
- No source, scene, asset, `.meta`, Packages, ProjectSettings, save or Prefs read-modify-write. The live profile directory was **listed**, not opened.
- No Roslyn typecheck or offline harness run. I did not re-verify the save line's 748 T-03A assertions or T-02R refusals.
- I did not find the signature record behind the save-line comment that "the signed contract reserves the number 2".
- I did not verify PlayerPrefs location under an `XDG_CONFIG_HOME` override, licence validity, or the cold-import time for a new lane.
- The guard extents in §6 are arithmetic from source constants, not rendered or collided.
- Dashboard / Drive not checked.
