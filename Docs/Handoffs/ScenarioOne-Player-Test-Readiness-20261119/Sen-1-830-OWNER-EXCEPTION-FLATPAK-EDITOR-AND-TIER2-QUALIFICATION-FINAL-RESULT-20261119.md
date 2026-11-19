# Sen-1 #830 — owner-exception Flatpak Editor start and #820 Tier 2 native qualification, FINAL RESULT

**19 November 2026. Status: automated native Tier 2 gameplay PASS on the unchanged #820 candidate; GUI/rendered review and the owner's personal E2 walk remain outstanding. This is not gameplay acceptance, not integration and not a release.**

Manager: [#824](https://github.com/jermur788/CCF/issues/824). Claim: [#830](https://github.com/jermur788/CCF/issues/830).
Binding authority: [owner exception + Manager APPROVED-TO-START handoff](https://github.com/jermur788/CCF/blob/docs/sen-1-824-830-owner-unity-start-20261119-v1/Docs/Handoffs/ScenarioOne-Player-Test-Readiness-20261119/Sen-1-824-830-OWNER-EXCEPTION-ACTOR-SCOPED-UNITY-START-AND-TIER2-RESTART-20261119.md) and [canonical Decision Log D-092](https://github.com/jermur788/CCF/blob/main/Docs/Project/decision-log.md).
This is a **new** result file; the earlier #830 HOLD result is not rewritten.

---

## 0. Verdict block

```
SEN1_830_OWNER_EXCEPTION_FLATPAK_TIER2
host:                        jer-laptop (verified)
worker:                      T3 Code / OpenCode, opencode-go deepseek-v4.1-flash, session ses_e0abb6d62ffeoxcZEqVpMYvw3Z
candidate:                   test/sen-1-820-compact30-e2-native-qualification-20261119-v1 @ 8cb22de077b93b4c7af076e19936f3170d6ab48b (unchanged, clean)
G0 physical preflight:       PASS
exclusive lease:             ACQUIRED 637106d0397742ec8545187ffdd68274, ETA 2026-11-20T01:30:00Z, no --force
lease release:               NORMAL identity-matched release 2026-11-19T19:45:31Z, forced=false, lock now FREE
G1 Flatpak licence smoke:    PASS  (existing LicenseClient reached inside the Hub sandbox; entitlements resolved; Editor ran to exit 0)
Tier 1 re-verified:          PASS
Tier 2 automated native:     PASS  71/71 checks (Sen1CompactE2Reuse on the real scene)
scene composition:           PASS  175 Sitka / 226 cosmetic-only shells / 4 G1 sites / clearances
GUI + rendered visual review: NOT RUN
old-world regression:        PARTIAL  Legacy40 identity PASS; deeper save-geometry suite NOT RUN (stale harness v23 vs candidate v24)
save / Compact profile:      HELD (no save controller, no save writes)
owner E1/E2 acceptance:      PENDING
production source changes:   NONE
result file:                 exactly one new Markdown (this file)
dashboard / Drive:           NOT VERIFIED
VERDICT:                     PARTIAL — AUTOMATED NATIVE TIER 2 PASS; GUI/RENDERED REVIEW AND OWNER E2 WALK OUTSTANDING
```

---

## 1. Who ran this, and where

| Field | Value |
|---|---|
| Worker | T3 Code native parent / OpenCode; model `deepseek-v4.1-flash`, provider `opencode-go`; one parent, no child agents, no parallel Unity |
| Host | **`jer-laptop`** — `hostname` = `jer-laptop`, Linux 7.0.0-42-generic, Linux Mint 22.3 |
| Session | `ses_e0abb6d62ffeoxcZEqVpMYvw3Z` |
| Registration | [#830 RUNNING registration](https://github.com/jermur788/CCF/issues/830#issuecomment-7019864176) — a new actual actor that explicitly does **not** inherit the earlier Claude Opus 5.5 session, lease or custody |
| Starting HEAD | `8cb22de077b93b4c7af076e19936f3170d6ab48b`, branch `test/sen-1-820-compact30-e2-native-qualification-20261119-v1`, `git status` clean |
| Unity / Hub | Unity `6000.6.0f1` (`/media/jer/ZX20/Unity/6000.6.0f1`); Flatpak `com.unity.UnityHub` 3.22.3, system install, already running and signed in |
| Evidence root | `/media/jer/Files/CCF/Verification/Sen1-830-OwnerException-20261119/` |

---

## 2. G0 physical preflight and the actual lease

Fresh observations at 19:26–19:28Z, before any write:

- deployed `/media/jer/ZX20/CCF-Ops/scripts/unity-lock` `status` = **free**, `metadata: null`, `unity_editor_pids: []`;
- **no Unity Editor process**; only the Flatpak Hub, its Electron helpers and `Unity.Licensing.Client` (started 10:56Z) were running;
- ZX20 mounted `/dev/sdc2`, exFAT, UUID `18D6-896E`, ≈323 GB free (floor 16.1 GB);
- candidate clean at `8cb22de0`; **37/37** E2 path hashes and **8/8** E1/placement/protected hashes matched the #820 manifest;
- lane `Sen1-820-E2Native` registered with `unity: true` in `lanes.json`;
- no existing `#830` row on the physical board; no competing lane/branch claim.

**Board:** one row added — `SEN-1-830-OWNER-EXCEPTION-FLATPAK-TIER2` — with the file backed up (`board/active.md.bak-sen830-*`), written under an exclusive `flock` on the board file, and validated with the deployed `packet-lint` (`PASS: board schema and duplicate checks`). Before/after diff is exactly one added row; no other row or `lanes.json` entry changed.

**Lease:** deployed `unity-lock acquire` succeeded with an honest near-term ETA and **no `--force`**:

```
lock_id          637106d0397742ec8545187ffdd68274
owner            T3 Code/OpenCode sen-1-830 orchestrator (opencode-go deepseek-v4.1-flash)
task_id          sen-1-830-owner-exception-flatpak-tier2
lane             /media/jer/ZX20/CCF-Sen1-820-E2-Native-20261119
branch/head      test/sen-1-820-compact30-e2-native-qualification-20261119-v1 @ 8cb22de0
start            2026-11-19T19:28:38.328754+00:00
eta              2026-11-20T01:30:00Z
```

No #652 install, no Ops script edit, no licence-file or account-DB copy, no other owner's process touched.

---

## 3. G1 — Flatpak-sandbox licence smoke

`flatpak enter 3611728743 …` joins the running Hub sandbox (`mnt:[4026533167]`), whose `/tmp` contains the live `Unity-LicenseClient-jer.sock`. The Editor was started with the sandbox config root and explicit `PATH`; **no token value or prefix is recorded here**.

Outcome: **PASS**

```
Successfully connected to LicensingClient on channel: "LicenseClient-jer" (connect: 0.00s, validation: 0.00s, handshake: 0.00s)
[Licensing::Module] Error: Access token is unavailable; failed to update
[Licensing::Client] Successfully updated license, isAsync: True, time: 0.00
[Licensing::Client] Successfully resolved entitlement details
[Package Manager] Registered 65 packages
[ExitDontLaunchBugReporter] Exiting without the bug reporter. Application will exit with return code 0
```

Precise reading against the #829 pass criterion:

| Criterion | Result |
|---|---|
| no `Channel … doesn't exist` | PASS — the existing channel was found immediately |
| successful LicensingClient connection | PASS |
| token refresh/status | **token refresh reported unavailable**, but `Successfully updated license` followed and entitlements resolved; the Editor stayed licensed for the whole run |
| no `No valid Unity Editor license found` | PASS |
| Editor reaches project import | PASS — 65 packages registered, import completed, exit 0 |

The first attempt failed only because `flatpak enter` supplies an empty `PATH`, so `git` (present at `/usr/bin/git`) was not found for the `com.kitwright.unity.mcp` git dependency. Setting `PATH` fixed it; licensing had already passed on that attempt.

---

## 4. Tier 1 — re-verified on the exact head

| Check | Result |
|---|---|
| 37/37 E2 path SHA-256 against the #820 manifest | PASS |
| 8/8 E1 scene + placement + protected old-world/reference SHA-256 | PASS |
| `Tools/Verification/Sen1CompactE2Reuse/run_offline.py` (site/roster rules + negatives) | PASS `checks=36 failures=0` |
| Runtime source compile by the real Editor import | PASS — no compile errors before play |
| UPM resolution | PASS — 65 packages registered |

---

## 5. Tier 2 — real Editor, real scene, automated native gameplay

Executed `Sen1CompactE2ReuseVerification.Begin` in the Unity Editor against `Assets/ForestPrototype/Scenes/ScenarioOneCompact30.unity`, with the disposable harness copied in only for the run.

**Result: `COMPACT30_E2_VERIFY_PASS checks=71` — 71/71 PASS.** Grouped evidence:

| Gate area | Representative checks | Result |
|---|---|---|
| Real scene bind | session/manager/ecology bound; fixtures accepted; Compact30 geometry active; 36 cells on 6×6 | PASS |
| Roster and world | 175 living trees bound; 175 distinct source ids; valid roster | PASS |
| Start state | no implicit Crop/Fell marks; no rack plan; no rack tool; Compact30 not admitted as a save geometry; no save controller | PASS |
| Old geometry identities | Legacy40 = 40 m, Enlarged80 = 80 m, both still admitted | PASS |
| Crop Trees | three living Sitka Crop Trees selected; crop trees alone award no teaching progress | PASS |
| H1 arrival/assessment negatives | designation refused before arrival/assessment; arrival refused outside H1; assessment refused until opened | PASS |
| Stock purchase | two Sessile Oak + two Beech bought; stock owned | PASS |
| Free-planting negative | free exact planting refused; created no order | PASS |
| Four G1 orders | four distinct exact **Pending** PlantJuvenile orders; zero juveniles before resolution | PASS |
| Pending correction | re-designating B2 removes the old order, creates a fresh Pending id, keeps the four-slot group | PASS |
| Bulk-setter negatives | refused with Pending work; refused against Approved work; nothing demoted | PASS |
| Approval → year 1 | approval succeeds; three orders Approved; year 1 advances; three real juveniles exist; group not yet CT-1 | PASS |
| Review negatives | non-covering review acknowledgement records no Compact credit | PASS |
| Later-year fourth planting | fourth site Pending in year 1 while earlier orders are Completed; approved; year 2 advances | PASS |
| CT-3 | second advance in the same frame blocked; blocked advance does not change the year | PASS |
| Linked outcomes | four successful linked 2 Oak + 2 Beech; covering year = actual resolution year (2); each order links a real juvenile | PASS |
| Visible juveniles | four real planted juveniles, each with a visible neutral representation | PASS |
| Completed-site refusal | completed site cannot be replaced | PASS |
| Teaching completion | T1 complete only after the covering review; advance released to year 3; historical two-cycle completion **not** awarded | PASS |
| Later no-work decision | deliberate no-work year recorded after T1 with a real consequence | PASS |

One benign Editor-internal `ArgumentOutOfRangeException` came from Unity's QuickSearch indexing on startup, not from project or gameplay code; the run continued and passed all 71 checks.

---

## 6. Scene composition (independent, offline)

Re-ran the #820 `verify_scene.py` against the candidate: **all PASS** — placement JSON strict-parses; 175 tree rows / 175 unique ids / all Sitka; exactly 4 G1 sites (2 Sessile Oak, 2 Beech); 175 authored Sitka prefab instances under `Trees`; **226** cosmetic instances under `Outside_Woodland_Continuation`; scene coordinates equal JSON coordinates; all four G1 markers match; off-path clearances +0.774 / +0.767 / +2.492 / +1.996 m.

---

## 7. Protected old-world regression

| Check | Result |
|---|---|
| Legacy40/Enlarged80 stand sizes and admission in the live Editor | PASS (harness §5) |
| `StandGeometryVerification` on `ForestTest.unity` — model identities, 64/256 cells, new games Legacy40, **336-tree Legacy40 starting world unchanged** | PASS (first check) |
| Deeper save-geometry round-trip in that suite | **NOT RUN — stale harness.** It asserts `CurrentVersion == 23`; the candidate is `ForestSaveData.CurrentVersion = 24`. Not edited or rebaselined, per the no-expectation-tweak rule |
| Frozen Reference Future v1 bytes/markdown/contract hashes | PASS — unchanged |
| `forest-save.json` nonmutation | PASS — sha256 `ccb01a01…5d303` identical before, between and after all runs |

The old-world depth here is deliberately proportional: the E2 delta changes gameplay/UI source, not the save schema or the frozen reference, and the save/Compact profile is explicitly out of scope (#690). Full old-world runtime regression belongs to the next integrated Tier 3, not to this #830 session.

---

## 8. Cleanup, side effects and preserved state

- Both disposable harness copies and their generated `.meta` removed; `git status --porcelain` = **0 lines**; no untracked `Assets/` content.
- One real Editor side effect: the first import migrated `com.unity.sdk.linux-*` and `com.unity.toolchain.linux-x86_64-linux` from `1.1.0` to `1.1.1` in `Packages/manifest.json` and `Packages/packages-lock.json`. **Disclosed**, then both files restored byte-exact with `git checkout HEAD --`. Build-time toolchain only; no gameplay, URP, Input System or package-version change remains.
- Candidate preserved clean at `8cb22de0`; 37/37 and 8/8 hashes re-verified after cleanup.
- No `forest-save.json` write, no scene save, no persistent `Library`-external side effect in tracked files.
- No Unity Editor process left; lease released normally by exact owner/task/lock identity at `2026-11-19T19:45:31Z` (`forced: false`); lock now **FREE** with no Editor PIDs.
- Coordination anchor dirty files (pre-existing `AGENTS.md`, `StorageArchitecture.md`, `active-tasks.md` and the three 2026-11-18 operations notes) were not touched.

---

## 9. Limits and exclusions

- **GUI/OS pointer input and rendered visual review are NOT RUN.** The run is a headless batch Editor play-mode pass on the real scene. The one approved Hub-GUI fallback was conditional on batch licensing failing; batch licensing succeeded, so it was not used. No authorised screenshot harness exists for this task and none was authored.
- Harness PASS alone is therefore **not** claimed as complete Tier 2 acceptance. It is strong automated native gameplay evidence on the exact scene and source.
- E1 owner second walk and E2 owner personal walk remain **PENDING**.
- Compact save/write/load (profile 3) remains **HELD** — no #690 source, no save controller in Compact.
- M-A / OD-16 compact completion, whole-world signature (#676), integration, main merge and Windows build are untouched.
- No production source, scene, save, asset or Ops change; no #652 install.
- Dashboard / Drive synchronisation: **NOT VERIFIED**.

---

## 10. Recommended next step

1. Manager #824 intake of this result and of the stale old-world harness expectation (`StandGeometryVerification` v23 vs current v24) as separate, non-blocking test debt — do not rebaseline it inside #830.
2. Owner performs the **E2 personal walk** on the exact candidate `8cb22de0` (Compact30 scene, H1 → assessment → 3 Crop Trees → buy 2 Oak + 2 Beech → 4 G1 orders → approve → years → CT-3 review → later no-work year). That walk is the remaining GUI/rendered proof.
3. If rendered review is wanted before the walk, authorise one bounded GUI visual pass on the same sandboxed launch context; do not treat this headless pass as rendered acceptance.

---

## 11. Evidence

Root: `/media/jer/Files/CCF/Verification/Sen1-830-OwnerException-20261119/`

| File | Contents |
|---|---|
| `EVIDENCE-SUMMARY.md` | condensed raw facts |
| `orchestration/routing-ledger.jsonl` | orchestrate-work routing/readiness ledger (portable fallback, labelled) |
| `unity/g1-licence-smoke.log`, `unity/g1-licence-smoke2.log` | licence smoke attempts |
| `unity/tier2-harness.log` | all 71 checks and `COMPACT30_E2_VERIFY_PASS` |
| `unity/oldworld-standgeometry.log`, `unity/oldworld-standgeometry2.log` | old-world Legacy40 run and stale-harness stop |
| `unity/oldworld-save-before.sha256` | save nonmutation baseline |
| `/media/jer/ZX20/CCF-Ops/state/unity/history.jsonl` | lease acquire/release history (live record) |
| `/media/jer/ZX20/CCF-Ops/board/active.md` | one added #830 row plus backup |

Dashboard/Drive: **NOT VERIFIED**. GitHub is the handoff of record.
