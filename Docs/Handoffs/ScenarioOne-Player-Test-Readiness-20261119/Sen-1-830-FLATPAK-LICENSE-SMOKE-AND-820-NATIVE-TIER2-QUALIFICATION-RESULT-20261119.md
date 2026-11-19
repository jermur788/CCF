# Sen-1 #830: Flatpak licence smoke and #820 native Tier 2, RESULT

**19 November 2026. Verdict: `G0 PASS (admission facts) → HOLD: no D-091-compliant exclusive Unity custody route is deployed`. G1 Flatpak licence smoke: NOT RUN. G2 #820 Tier 2: NOT RUN. Unity Editor was not launched.**

Manager: [#824](https://github.com/jermur788/CCF/issues/824). Claim: [#830](https://github.com/jermur788/CCF/issues/830) ([G0 registration](https://github.com/jermur788/CCF/issues/830#issuecomment-7019006855)). Packet: [Manager #824 #829 approval and #830 packet @ `c5a049db`](https://github.com/jermur788/CCF/blob/c5a049db7a9bd3efce3989be8318b1b298d4be01/Docs/Handoffs/ScenarioOne-Player-Test-Readiness-20261119/Sen-1-824-829-MANAGER-APPROVAL-AND-830-FLATPAK-UNITY-NATIVE-QUALIFICATION-RESUME-PACKET-20261119.md). Dashboard/Drive: **NOT VERIFIED**.

---

## 0. Worker and lanes

| Field | Value |
|---|---|
| Worker | Claude Code inside T3 Code, `claude-opus-5-5`, Anthropic. Local session, one parent, no fork. It is **not** the #820 DeepSeek/OpenCode worker and inherits none of that worker's identity, lease or ownership |
| Host | `jer-laptop` (verified with `hostname`; Linux 7.0.0-42-generic, Linux Mint 22.3) |
| Result lane | `/media/jer/Files/CCF/Worktrees/CCF-Sen1-830-FlatpakUnityResult` (path was free; tested writable), new branch `docs/sen-1-830-flatpak-unity-native-result-20261119-v1` from packet tip `c5a049db`. Packet branch not written |
| Observed | 2026-11-19 ≈18:55–19:05 UTC (laptop clock GMT = UTC) |

## 1. G0 read-only admission checks

| # | Check | Result | Evidence |
|---|---|---|---|
| G0-1 | Host is `jer-laptop`, actual worker registered | **PASS** | §0. [Registration comment](https://github.com/jermur788/CCF/issues/830#issuecomment-7019006855) |
| G0-2 | #820 composed candidate identity | **PASS** | `/media/jer/ZX20/CCF-Sen1-820-E2-Native-20261119`, branch `test/sen-1-820-compact30-e2-native-qualification-20261119-v1`, HEAD `8cb22de077b93b4c7af076e19936f3170d6ab48b`. Corrected E1 base `0709d05a` is an ancestor |
| G0-3 | Candidate content unchanged since #820 | **PASS** | Re-hashed against #820's `02-source-manifest.txt`: **37/37** E2 delta paths and **7/7** E1-scene/protected old-world scene hashes match. 0 mismatches |
| G0-4 | Candidate worktree cleanliness | **PASS** | `git status --short` is empty. Nothing was reset, stashed or cleaned |
| G0-5 | Unity currently active | **PASS (idle)** | No `6000.6.0f1/Editor/Unity` process |
| G0-6 | Physical lock state (read from state files directly; no `unity-lock` call, because even `status` takes the guard flock) | **FREE** | `CCF-Ops/state/unity/` holds only `guard` and `history.jsonl`, with no lock metadata file. Last event: release `c5fc24d6…` by the #820 orchestrator at 17:13:21Z ("Tier-2 attempt ended before import … No Editor left running … worktree clean") |
| G0-7 | Lanes and board | **NO CONFLICT** | `lanes.json` still registers `Sen1-820-E2Native` (`unity: true`) for the candidate path. Board `active.md` (last written 09:30Z) has no #820/#830 row. Live active-tasks (origin/main `251fd200`) shows #830 **READY G0 / EDITOR HELD**. No other owner claims the candidate path |
| G0-8 | Flatpak Hub sandbox still present (not launched or changed) | **PASS** | `flatpak ps` shows `com.unity.UnityHub` instances running. The sandboxed `Unity.Licensing.Client` has been alive since 10:56Z, the same process #829 observed |
| G0-9 | ZX20 free space | **PASS** | ≈346 GB available, well above the 15 GiB `unity-lock` floor |
| G0-10 | **Can a D-091-compliant exclusive Unity hold be recorded?** | **FAIL → HOLD** | See §2 |

## 2. The HOLD finding (single)

**The only deployed lock tool cannot record a no-expiry hold, and D-091 forbids starting Unity until a compliant route is verified and separately approved.**

- Deployed `/media/jer/ZX20/CCF-Ops/scripts/unity-lock` (sha256 `54cf0a95…`, mtime 2026-10-15) with `_ccf.py` `ab4343ef…`. Its schema requires a non-empty `eta` string. `acquire` rejects any `--eta` that is not a **future** timestamp with a timezone, and `status` reports `eta_overdue`. The tool has **no** `no_expiry` field or mode.
- [D-091](https://github.com/jermur788/CCF/blob/251fd20004d52e1c0c441494892ac7c52dbc40a9/Docs/Project/decision-log.md) (owner, 2026-11-18) rejects arbitrary time-based expiry. It names "fabricated distant date" as forbidden, and states: "**no source/Unity start until a compliant no-expiry custody route is actually verified and separately approved.**" #652 (the reviewed no-expiry `unity-lock`) remains **NOT DEPLOYED** (live install HELD).
- The #830 packet forbids an arbitrary or far-future `--eta`, a forced acquire and live Ops patching. It also states that the released one-off +6h #820 lease does not authorise renewals.

So no lawful exclusive lease can be taken by this worker. Per the packet, **G1 and G2 were not started**: no lock acquire, no Editor or `flatpak enter`, and no board or lanes write.

## 3. What would unblock G1 (decision for Manager #824 and the owner, not taken here)

Either route needs an explicit decision recorded on #830 before any Editor start:

1. **Deploy the reviewed no-expiry `unity-lock` (#652)** through its own custodian and installer process. It is currently HELD by D-091 implementation and the open P-1/P-2/P-3 items. This is the D-091-native route, but it is a separate Ops task.
2. **An explicit, actor-specific owner exception to D-091** for one bounded legacy-ETA lease on the deployed tool, as the owner gave #820 earlier today. It must not be inferred from this packet.

Once one of these is granted, the G1 smoke is ready to run unchanged as the packet specifies. Inside the running Hub instance, run:

`flatpak enter <hub-instance> /media/jer/ZX20/Unity/6000.6.0f1/Editor/Unity -batchmode -quit -projectPath <candidate> -logFile <own evidence dir>`

Judge the result by the #829 §4 pass criterion, reading only redacted log lines. Note that Unity's Editor log prints a **partial access-token prefix** on its "updated the access token" line, which a long-string redaction regex does not catch. Evidence extraction must drop that line's trailing token text.

Unverified risks carried forward:
- `flatpak enter` into this app's instance has not been tried.
- Personal headless entitlement in the sandbox is not known.
- The Flatpak `TMPDIR` points into the app's sandbox cache. The Unity gate memory warns that the licensing client stalls when `TMPDIR` is on exFAT ZX20, so check that the sandbox `TMPDIR` is on the home filesystem; it was observed under `~/.var/app/…/cache/tmp`, which is not on ZX20.

## 4. Verdict block

```
SEN1_830_FLATPAK_SMOKE_AND_820_TIER2
host:                         jer-laptop (verified)
worker:                       claude-opus-5-5 / Anthropic / T3 Code local; not the #820 worker
candidate:                    8cb22de0, clean, 37/37 delta + 7/7 E1/protected hashes match #820 manifest
unity_lock_state:             FREE (last release c5fc24d6 17:13:21Z); no Editor PID
flatpak_hub_instance:         running; sandboxed licensing client alive since 10:56Z
d091_compliant_custody:       NOT AVAILABLE (deployed unity-lock requires future ETA; #652 no-expiry not deployed)
G0:                           admission facts PASS; custody FAIL -> HOLD
G1_flatpak_licence_smoke:     NOT RUN
G2_820_tier2:                 NOT RUN (import, scene, purchase, Work Plan, annual review, CT-3, E80/Legacy40/RF v1 regression all NOT RUN)
unity_launched:               NO
lock_acquired_or_forced:      NO
board_lanes_ops_writes:       NONE
source_scene_save_edits:      NONE
credentials_licence_read:     NONE
dashboard_drive:              NOT VERIFIED
```

#820 Tier 1 7/7 and E1+E2 37/37 remain as previously reported. Player beta is not ready.
