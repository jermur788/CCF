# Sen-1 #820 — Compact30 E2 native scene composition and gameplay qualification: RESULT

**19 November 2026. Verdict: `PARTIAL — NATIVE QUALIFICATION HELD`.**
**One native T3 Code / OpenCode orchestration parent. Source composition and Tier 1 qualified. The custody gate passed under explicit owner authorisation with a bounded, properly released lease. Tier 2 (real Unity Editor/GUI gameplay) was then blocked by Unity licensing — the Hub is signed out and the Personal entitlement is stale — so no gameplay pass is claimed.**

This is the single result handoff required by the #820 binding packet. GitHub is the handoff of record. Dashboard/Drive: **NOT VERIFIED**.

---

## 0. What this task did and did not do

| Did | Did not |
|---|---|
| Read the corrected final #813 scene commit and the final #815 E2 commit from `origin` | Touch, rewrite or force-checkout the #813 or #815 owner branches |
| Built a new, distinctly owned test-only worktree and composed E1 scene + E2 37-path delta there | Edit the E1 scene or placement JSON |
| Ran strict JSON/scene/G1/roster integrity checks and the binary apply-check | Change `Assets/ForestPrototype/Scenes/ScenarioOneCompact30*` bytes |
| Ran the offline 36-check site/roster suite plus a live negative control | Inherit #815 PASS from a different head |
| Compiled all runtime sources and the disposable harness against Unity 6000.6.0f1 assemblies | Install or patch the deployed `unity-lock` script |
| Registered one additive Unity lane, acquired and normally released lease `c5fc24d6…`, and attempted a real Editor start | Leave the Editor running, force-release, or leave the harness copy behind |
| Verified old-world scenes, frozen Reference Future v1 and non-delta paths byte-unchanged | Produce a Compact save/profile, M-A/OD-16 completion, Windows build or main merge |

A single source-authority note: this parent made **no production source edits**. The only file it authored is this result document. No changed-lines reviewer was therefore required or created, per packet §2.

---

## 1. G0 — physical and source preflight: **PASS (source) / HELD (Unity custody)**

### 1.1 Actor, host and session

| Field | Value |
|---|---|
| Harness | Native T3 Code / OpenCode, single parent |
| Model | `deepseek-v4.1-flash` (provider `opencode-go`) |
| Session | `ses_e0b4ca381ffeIAtd4nIH0HJWYE` |
| Host | `jer-laptop`, Linux 7.0.0-42-generic, 4 cores, 15 GiB RAM |
| Unity SDK present | `/media/jer/ZX20/Unity/6000.6.0f1` (Editor changeset `f7f8ed4d1e24`) |
| Reference player | `/home/jer/ccf-verify-player` — `globalgamemanagers` reports `6000.6.0f1` |
| Source worktree | `/media/jer/ZX20/CCF-Sen1-820-E2-Native-20261119` |
| Source branch | `test/sen-1-820-compact30-e2-native-qualification-20261119-v1` |
| Evidence worktree | `/media/jer/Files/CCF/Worktrees/CCF-Sen1-820-E2-Docs` |
| Evidence branch | `docs/sen-1-820-e2-native-parent-20261119-v1` |
| Durable evidence root | `/media/jer/Files/CCF/Verification/Sen1-820-E2-Native-20261119/` |

`/media/jer/ZX20/CCF-Sen1-820-E2-Native-20261119` was a **new** path. No existing lane, primary checkout or stale worktree was overwritten.

### 1.2 Storage and mount identity

| Mount | Device | FS | Free | Verdict |
|---|---|---|---|---|
| `/media/jer/Files` | `/dev/sdb4` | ext4 | 34 GiB | ≥10 GiB floor satisfied |
| `/media/jer/ZX20` | `/dev/sdc2`, UUID `18D6-896E` | exFAT | 325 GiB | matches `lanes.json` UUID and by-id device |
| `/media/jer/Videos` | `/dev/sdb1` | ext4 | 406 GiB | not used for CCF durable output |

`/dev/disk/by-id/ata-NetacPortableSSD_AA20260702512G231151-part2` resolves to `/dev/sdc2`, so the live `lanes.json` storage-identity pin (by-id) is correct on this boot. The stale `/dev/sda2` pin recorded in `Docs/Project/coordination/active-tasks.md` § *Record fidelity checks* is a **record** defect only; the live config is not affected.

### 1.3 Exact inputs (full SHAs)

| Input | Ref | SHA |
|---|---|---|
| Corrected E1 scene (final) | `origin/sen-1/813-compact30-e1-visual-layout-fix-20261119-v1` | `0709d05a79756d3b72ccbd0452bd0252a75c3804` |
| E2 management loop (final) | `origin/feature/sen-1-815-compact30-e2-loop-reuse-20261119-v1` | `397fff9f1261550baec934cc52af38b256d4d049` |
| E2 registered base | — | `159f0ff8b075e275045ddd1ab0925cb447364b77` |

**Preflight finding (disclosed, not repaired):** the local non-remote branch `sen-1/813-compact30-e1-visual-layout-fix-20261119-v1` in the coordination anchor still points at the **pre-correction** commit `be60c174f8609107447c6cbf4dd223da8d7b8cdf`, whose placement JSON is malformed. Only `origin` carries the corrected `0709d05a`. This task used the exact SHA, not the branch name. Any later worker that writes `git rev-parse` of the *local* branch would pick up the malformed JSON. This is a live reproducer for the packet's "do not reuse the earlier pre-comma scene" warning.

### 1.4 Corrected #813 delta is one comma

`git diff be60c174 0709d05a` = 1 file, 1 insertion, 1 deletion:
`Assets/ForestPrototype/Scenes/ScenarioOneCompact30Placement.json`, line 130, `}` → `},`.
The whole E1 line over base `159f0ff8` is **additive only** (5 added paths: the scene, the placement JSON, `Scenes.meta` and two `.meta` files). No existing file was modified by #813.

### 1.5 Placement JSON and scene composition (strict, independent re-derivation)

| Check | Result | Evidence |
|---|---|---|
| Strict JSON parse | PASS | `placements.json` parsed by Python `json` |
| Tree rows | PASS — 175 | |
| Unique tree IDs | PASS — 175 unique, 0 duplicates | |
| Species | PASS — all 175 `sitka-spruce` | |
| G1 sites | PASS — exactly 4; 2 `sessile-oak`, 2 `beech` | |
| Scene authored Sitka prefab instances under `Trees` | PASS — **175** | class-1001 instance parents |
| Cosmetic outside shell under `Outside_Woodland_Continuation` | PASS — **226**, separate parent, zero logical identity | |
| Scene authored tree coordinates == JSON `x_m/z_m` | PASS — 175/175 exact to 3 dp | |
| G1 scene markers == placement G1 coordinates | PASS — O1 `(0.65,−2.65)`, O2 `(0.2,−1.1)`, B1 `(−1.2,−2.45)`, B2 `(−0.45,−4.3)` | |
| G1 off-path clearance (centre-distance − 0.8 − 0.6) | PASS — O1 +0.774 m, O2 +0.767 m, B1 +2.492 m, B2 +1.996 m | independent polyline computation |

The 226 outside-shell trees are prefab instances of the same four first-party Sitka prefabs but live under a different parent with no roster IDs and no ecology binding; the 175 authored trees are the ones the runtime binds to roster identity. Roster digest recorded in the JSON (`dac06a3b…cca1`) reproduces the #676 candidate in the offline suite.

**Not demonstrated by this check:** that the 226 shell instances gain no `ForestTree` component at runtime. Their prefabs were not component-stripped in the scene YAML (`m_RemovedComponents` empty for all three parent groups). Whether the runtime filter excludes them is a **Tier 2** assertion and is therefore **NOT RUN**, not PASS.

### 1.6 Source ownership and reservations

- No live writer was found on either input branch: both `origin` tips equal the packet pins.
- The new test branch was created from the corrected SHA and contains only the 37-path E2 delta.
- Shared-file reservations respected: no `ForestSaveController*`, `ForestSaveData*`, `ForestSaveValidation*`, `ForestStartingStand.cs`, routing, `Assets/Scenes/ForestTest.unity`, `Packages/`, `ProjectSettings/` or Reference Future path is in the delta.
- #690 save profile custody and #718 scene custody were not entered.

**G0 verdict: PASS for source. Unity custody: HELD (see §4).**

---

## 2. Source composition: **PASS**

```
base     0709d05a79756d3b72ccbd0452bd0252a75c3804   (corrected E1 scene)
applied  159f0ff8..397fff9f  (full final E2 delta, 37 paths)
result   8cb22de077b93b4c7af076e19936f3170d6ab48b   on test/sen-1-820-…-20261119-v1
```

All 37 required paths, exactly, with no extras:

- **13 existing shared files (modified):** `ForestEcologyController.cs`, `ForestPlayer.cs`, `ForestTree.cs`, `ForestTreeSpawner.cs`, `StandGeometryModel.cs`, `ScenarioOne/ScenarioOneDefinition.cs`, `ScenarioOne/ScenarioOneManager.cs`, `UI/ScenarioOneUiRoot.cs`, `UI/WorkPlanView.cs`, `UI/AnnualReviewView.cs`, `UI/MenuHelpView.cs`, `UI/StandMapView.cs`, `UI/WalkingHudView.cs`.
- **20 new Compact paths:** `ScenarioOne/Compact30.meta`, `Compact30/{Compact30Bootstrap,Compact30Session,Compact30SiteRules}.cs` + metas, `Compact30/Resources.meta`, and `Resources/{Compact30Definition.asset, Compact30SitkaSpruce.asset, Compact30SessileOak.asset, Compact30Beech.asset, Compact30Roster.json, Compact30Sites.json}` + metas.
- **4 new verification paths:** `Tools/Verification/Sen1CompactE2Reuse/{Compact30RulesOfflineCheck.cs, Sen1CompactE2ReuseVerification.cs, run_offline.py, README.md}`.

The last three of the 13 (`MenuHelpView.cs`, `StandMapView.cs`, `WalkingHudView.cs`) are **outside the #815 author's historical ten-file self-grant**. They are used here under the Manager's **prospective #820 qualification admission with disclosure**, as required. This result does not backdate or ratify any earlier writer permission.

### 2.1 Apply-check before any writable composition

```
git diff --binary 159f0ff8 397fff9f > e2.diff          # 188,941 bytes
git -C <new E1 worktree> apply --check --verbose e2.diff
→ exit 0, zero rejected hunks, zero collisions
```

`--3way`, stashing, force checkout and silent conflict resolution were not used.

### 2.2 Post-apply exact-hash reconciliation

| Object | Result |
|---|---|
| 37/37 delta paths | **byte-identical to `397fff9f`** |
| `ScenarioOneCompact30.unity` | `5e948e68…17806` — identical to the E1 input |
| `ScenarioOneCompact30Placement.json` | `dc9d72a8…6cd7c` — identical to the E1 input |
| Changes vs corrected E1 base | exactly 37 paths, nothing else |

### 2.3 Protected old-world and frozen bytes

| Path | sha256 (first 16) | vs base |
|---|---|---|
| `Assets/Scenes/ForestTest.unity` (five-rack) | `e21507bb907a93ca` | unchanged |
| `Assets/Scenes/MixedSpeciesTest.unity` | `9ef1193039af9539` | unchanged |
| `Assets/Scenes/ForestryAssetReview.unity` | `d2d4852230efc1a4` | unchanged |
| `ScenarioOne/Resources/ScenarioOneReferenceFutureV1.bytes` | `789e3eddcf00d3d4` | unchanged |
| `Assets/ForestPrototype/Docs/ScenarioOneReferenceFutureV1.md` | `fd9e3c79d81484a0` | unchanged |
| `Docs/ReferenceFutureContract.md` | `239ed039b8aeaacd` | unchanged |

### 2.4 Changed-lines sanity read

The 13-file delta is 327 insertions / 60 deletions. Removals are replacements, not weakened guards:

- `StandGeometryModel`: `IsKnown` **unchanged** (still Legacy40/Enlarged80 only) so save admission is not widened; the new `IsRuntimeGeometry` adds Compact30 for runtime/ecology only, and `ForSave` still refuses Compact 30×30 saves. This is a correct, conservative #690 boundary.
- `ForestEcologyController`: Compact30 explicitly skips the Legacy40/Enlarged80 `StandWorldBoundary`.
- `ForestPlayer`: F/G/1/2/U/R shortcuts are gated only by `Compact30Session.IsActive`; old worlds keep legacy keys.
- The three out-of-grant UI files gate every change on `Compact30Session.IsActive` and leave the rack/objectives/save paths intact for old worlds.

---

## 3. Tier 1 — composed-candidate source qualification: **PASS (7/7)**

Every gate ran on the **real composed worktree**, not a scratch mirror, except the deliberate negative control, which is labelled.

| # | Gate | Result | Detail |
|---|---|---|---|
| T1.1 | Strict placement JSON + roster/G1/scene composition | **PASS** | 14/14 checks, `exit=0` |
| T1.2 | Full 37-path binary apply-check + hash reconciliation | **PASS** | `apply --check` exit 0; 37/37 + scene/JSON exact |
| T1.3 | `Tools/Verification/Sen1CompactE2Reuse/run_offline.py` | **PASS** | `checks=36 failures=0`, `exit=0` |
| T1.4 | Negative control — path-clearance refusal disabled | **PASS (control fires)** | `FAIL site on the path refused`, `checks=36 failures=1`, `exit=1` |
| T1.5 | Unity 6000.6.0f1 managed-assembly compile of runtime sources | **PASS** | `sources=192 refs=116 exit=0 errors=0` |
| T1.6 | Disposable harness compile against Unity 6000.6.0f1 + `UnityEditor.dll` | **PASS** | `Build succeeded. 0 Warning(s) 0 Error(s)` |
| T1.7 | CloudTypeCheck `--mode prod` (approximate secondary) | **PASS** | `[prod] distinct errors: 6 (baseline 6)` → `TYPECHECK_CLEAN` |

Notes on provenance, to avoid the Multi #789 V-02 failure mode:

- T1.5/T1.6 are **type/API gates**, not Unity's own compile; they prove the sources bind against the real 6000.6.0f1 assembly surface, nothing about behaviour.
- T1.7 is an approximate 2021.3 NuGet reference surface with checked-in stubs; a clean diff means "no new errors", not "Unity compiles it".
- T1.4 ran on `/tmp/opencode/sen820/negctl`, a scratch copy of `Compact30SiteRules.cs` with `if (clearance < 0f)` changed to `if (false && clearance < 0f)`. The worktree source was not mutated.

Evidence: `/media/jer/Files/CCF/Verification/Sen1-820-E2-Native-20261119/04-tier1.txt` (plus `verify_scene.py`).

---

## 4. G2 — machine-wide Unity exclusive custody: **PASS (owner-authorised bounded lease)**

### 4.1 First position, and why it changed

On first inspection no lease was taken. The machine was idle — `unity-lock status` `free`, zero Editor PIDs, 325 GiB free — but the **installed** tool could not record the custody this project requires:

| Fact | Value |
|---|---|
| Deployed tool | `/media/jer/ZX20/CCF-Ops/scripts/unity-lock`, sha256 `54cf0a95561d0a8c…20f048e5` |
| `deployment.json` recorded hash | same hash — match (source head `dfe445e5`, deployed 2026-10-15) |
| `no_expiry` support in deployed tool | **0 occurrences**; `acquire` mandates a future `--eta` |
| D-091-capable candidate | `CCF-OpsReadiness-652-20261116/CCF-Ops/scripts/unity-lock`, sha256 `8fa0c5ad…b220ac8`, 6 `no_expiry` occurrences — **not deployed** |
| Board row `CCF-652-OPS-READINESS` | source-only ceiling; "NO LIVE OPS INSTALL"; task itself BLOCKED / NOT RUNNING |

Acquiring on that basis would have meant either an arbitrary ETA against D-091 or a live Ops install the packet excludes. The packet explicitly permits the HELD outcome in that case, so the run stopped before Unity.

### 4.2 Owner authorisation resolved the custody gate

The owner then stated: *"unity is free I authorize you to continue."* That is the authority the packet was waiting on, and it converts the tool limitation from a blocker into a **disclosed, bounded** choice: a legacy-ETA lease taken knowingly, with an honest near-term ETA rather than a manufactured far-future timestamp, and a normal identity-matched release.

### 4.3 Lease actually acquired and released

| Event | Value |
|---|---|
| Lane registration | `Sen1-820-E2Native` → `/media/jer/ZX20/CCF-Sen1-820-E2-Native-20261119`, `unity: true`, additive (27 lanes), `lanes.json` backed up to `lanes.json.bak-sen820-20261119T170900Z` |
| Storage preflight | `READY / PASS`, `/dev/sdc2`, rw, 322.82 GiB free vs 15 GiB floor |
| Acquire | **succeeded** — lock_id `c5fc24d68c60415784280c901ba64877` |
| Bound identity | task `sen-1-820-compact30-e2-native-qualification`, branch `test/sen-1-820-…-v1`, HEAD `8cb22de0…`, ETA `2026-11-19T23:09:20Z` (ETC +6 h, not far-future) |
| Release | **normal**, matching owner/task/lock-id; `forced: false`; `unity_editor_pids: []` |
| Final state | lock **free**, no Editor process, worktree clean |

No `--force`, no patch install, no `no_expiry` spoofing, no board mutation beyond the one additive lane row.

### 4.4 Tier 2 was then blocked by a different gate: Unity licensing

Under the lease, Unity 6000.6.0f1 was launched twice — once `-batchmode -executeMethod`, once GUI — and **both failed before any project import**:

```
[Licensing::Module] Error: Access token is unavailable; failed to update
[Licensing::Module] Error: 'com.unity.editor.headless' was not found.
No valid Unity Editor license found. Please activate your license.
```

| Fact | Value |
|---|---|
| Licence cache | `/home/jer/.config/unity3d/Unity/licenses/UnityEntitlementLicense.xml` — **Unity Personal**, Issued 2026-09-15, last `UpdateDate` 2026-10-15 (**35 days stale**) |
| Unity Hub account store | `/home/jer/.config/unityhub/accounts.db` — **0 accounts, 0 active_account** rows |
| Network to licence service | reachable — `license.unity3d.com` HTTP 200 |
| Batch exit code | 198 |

So the machine, the SDK and the network are all fine; the Hub is simply **not signed in**, so the licensing client has no access token and cannot refresh the expired Personal entitlement. Unity Personal activation requires an interactive Hub sign-in with the owner's account. That is a credentials-only action and was not attempted.

Project-side effect: none. The Editor never reached asset import, so no scene was opened and no gameplay code ran. The disposable harness copy was removed and the worktree returned to `8cb22de0` with no untracked `Assets/` content.

Evidence: `06-tier2-licence-blocker.txt`, `unity/harness-run.log`, `unity/gui-licence-test.log`.

---

## 5. Tier 2 — real Unity Editor + player-complete loop: **NOT RUN**

The Editor was started and immediately refused to licence, so no scene was imported or played. Every Tier 2 assertion is therefore **NOT RUN**, not PASS and not FAIL:

| Packet item | Status |
|---|---|
| Unity import of `ScenarioOneCompact30.unity` at the exact composed HEAD | NOT RUN |
| Zero compile errors / zero missing scripts in the real project | NOT RUN |
| Real H1 arrival **Y**, assessment **J**→**Y**, and their negatives | NOT RUN |
| Player-selected three Crop Trees with **C**, mutually exclusive of Fell | NOT RUN |
| 2 Oak + 2 Beech purchase/hold, G1 designation, 4 distinct Pending PlantJuvenile orders, zero juveniles before execution | NOT RUN |
| Work Plan approval as sole commitment; immutability and two-click cancellation | NOT RUN |
| Year advance → linked juvenile IDs, stock/cash/minutes outcomes; failed/cancelled/partial orders earn no credit | NOT RUN |
| Covering-year Annual Review, CT-3 hold, two same-frame advance attempts, Event9/Y* chronology | NOT RUN |
| S1-C history plus later approved treatment or deliberate no-work year | NOT RUN |
| Visual/UI review: path clearances in-render, no rack/MFM leakage, no save controls, 226-shell exclusion, Crop Tree marking legibility | NOT RUN |
| Old-world regression: five-rack/Enlarged80/Legacy40 rack UI and annual ecology, frozen Reference hashes, historical fixtures read-only | NOT RUN |
| `Sen1CompactE2ReuseVerification.cs` → `COMPACT30_E2_VERIFY_PASS` | NOT RUN (source compiles, §3) |
| Disposable harness copy-in / `.meta` cleanup cycle | NOT RUN (never copied in) |

No screenshots exist for this task. No event IDs, no player-flow trace, no OBS-trace, no performance numbers. **Nothing in §5 is evidenced, and none of it is claimed.**

The disposable harness **was** copied into `Assets/ForestPrototype/` in the leased worktree for the attempt, and was **removed again together with any generated `.meta`**. `git status` on the composition worktree is clean apart from tracked history; no temporary script remains and no `forest-save.json` was touched.

---

## 6. E1 owner acceptance: **PENDING**

Unchanged by this task. The #813 owner second walk has not happened and this result cannot substitute for it. Two owner-judgement items remain open from the #813 addendum:

1. The old "before" image does not show the one-sided PENDING labels.
2. The survey-orange peg corners may be too bright.

Neither was altered here.

---

## 7. Save / Compact profile / CT-3 reload / M-A: **HELD**

| Item | Status |
|---|---|
| Compact save write/load/CT-3 reload | **HELD** — #690 source custody; not in this delta |
| Third signed save profile / world signature | **NOT IMPLEMENTED** |
| M-A / OD-16 full Compact completion | **NOT AUTHORISED** — remains incomplete by design |
| Save-continuity claims of any kind | **NONE MADE** |

The code itself keeps this honest: `StandGeometryModel.IsKnown` still refuses Compact30 for save admission, so a Compact save is rejected rather than silently written into the historical admission set. That is a source-level observation only; it is not a runtime verification.

---

## 8. Verdict and the one next executable blocker

**Verdict: `PARTIAL — NATIVE QUALIFICATION HELD`.**

- Source composition of the **final corrected #813 scene + final #815 E2 delta**: **PASS**, exact hashes, no collisions, no collateral change.
- Tier 1 source qualification: **PASS 7/7**.
- G2 exclusive custodian custody: **PASS** — owner-authorised bounded lease acquired and released in good order.
- Tier 2 native gameplay qualification: **NOT RUN** — the Unity Editor could not licence.
- Owner-playtest readiness: **NOT YET**. A user walking E2 today would still be testing a scene that has never been imported or played by this task. That is not readiness.

**One next executable blocker (single, ordered):**

> Sign in to the Unity Hub on `jer-laptop` with the owner's Unity account and re-activate the Unity Personal licence (Hub → Settings → Licenses → activate). The Hub account store is empty and the cached entitlement is 35 days stale, so the Editor refuses to start for any project. Then re-acquire the lease and re-run #820 Tier 2 unchanged against:
> `test/sen-1-820-compact30-e2-native-qualification-20261119-v1` @ `8cb22de0`.

Nothing else is blocking. Storage, SDK version, lane registration, lock/release procedure, source inputs, module imports and hash integrity are all already satisfied and evidenced.

---

## 9. Source pins required for a re-run

| Item | Value |
|---|---|
| Unity | 6000.6.0f1 (`f7f8ed4d1e24`) |
| Corrected E1 base | `0709d05a79756d3b72ccbd0452bd0252a75c3804` |
| E2 final | `397fff9f1261550baec934cc52af38b256d4d049` |
| E2 registered base | `159f0ff8b075e275045ddd1ab0925cb447364b77` |
| Composed candidate | `8cb22de077b93b4c7af076e19936f3170d6ab48b` |
| Composed scene | sha256 `5e948e68fad41a0e9669bf0d18efe73aab2e8d2dd2148b2ab2824f4915c17806` |
| Composed placement JSON | sha256 `dc9d72a87f62fba81e420ac6f0cc5fb21f22f59c36ee29ef9522274dba36cd7c` |
| Roster digest | `dac06a3b7775f884fac9f8df6b95178cbaf9d912b44687082a161eaf0426cca1` |

---

## 10. Result block

```
SEN1_COMPACT30_E2_NATIVE_820
g0_source_preflight:              PASS
g0_unity_lease:                   PASS  (owner-authorised; lane registered; lock free at end)
source_composition:               PASS  (0709d05a + 37-path 159f0ff8..397fff9f -> 8cb22de0)
e1_scene_corrected_input:         PASS  (175 authored / 226 cosmetic / 4 G1, 1-comma fix)
apply_check_collisions:           0
tier1_json_scene_g1:              PASS  (14/14)
tier1_offline_rules:              PASS  (36/36)
tier1_negative_control:           PASS  (1 intended failure)
tier1_unity6000_assembly_compile: PASS  (192 sources, 0 errors, 0 warnings)
tier1_harness_editor_compile:     PASS  (0 errors, 0 warnings)
tier1_cloud_typecheck_prod:       PASS  (6 baseline, 0 new)
tier1_old_world_bytes:            PASS  (scenes + frozen Reference Future unchanged)
g2_unity_physical_lock:           PASS  (lock c5fc24d6..., released normally)
tier2_editor_import:              NOT RUN  (blocked: Unity Personal licence stale, Hub signed out)
tier2_gui_gameplay:               NOT RUN
tier2_old_world_regression:       NOT RUN
harness_run:                      NOT RUN
e1_owner_acceptance:              PENDING
save_compact_profile_ct3:         HELD / NOT IMPLEMENTED
compact_full_completion:          NOT AUTHORISED (M-A/OD-16)
windows_build:                    NOT AUTHORISED
production_source_edits:          NONE
dashboard_drive:                  NOT VERIFIED
verdict:                          PARTIAL — NATIVE QUALIFICATION HELD
```
