# Multi — Standalone 2D Sitka P0 lab: operator launch and 15-minute regression readiness

**Date:** 16 November 2026  
**Task:** [CCF #568](https://github.com/jermur788/CCF/issues/568) — browser/GitHub-only operator documentation.  
**Worker:** OpenAI ChatGPT GPT-6, cloud/browser GitHub connector, **no local worktree or runtime access**.  
**Status:** **OPERATOR RUNBOOK / INSTRUCTIONS ONLY**. Every action in the 15-minute checklist below is **NOT RUN by #568**; the historical qualification results are cited and kept separate.  
**Scope:** standalone **2D, synthetic, no-growth** Multi-Rack Sitka P0 laboratory. **Not** Scenario One, Unity, a Windows game build, an Irish forestry forecast, or P1 annual growth/mortality. No production code, README, tests, fixtures, schemas, saves or user settings were changed by this task.

## 1. Source selection: use the qualified executable, not this documentation branch

| Role | Git reference | Meaning |
|---|---|---|
| **Qualified executable P0 source** | [Fixed #567 source at `a7f41983c2f2f233b8fe999d1a5b10e72bdd2866`](https://github.com/jermur788/CCF/tree/a7f41983c2f2f233b8fe999d1a5b10e72bdd2866/Tools/MultiRackSitkaLab), branch `fix/multi-sitka-p0-save-response-r3-20261116-v1` | **Use this exact commit for operator execution.** Manager accepted its independent source/security review and targeted Windows Edge/Ubuntu WSL2 qualification; branch names may move, so verify the full commit. |
| **#568 writing branch, NOT qualified fixed executable** | `docs/multi-sitka-p0-operator-readiness-20261116-v1`, starting `861b81d3540429d241bba0732a610212626f6828` | Deliberately based on the **older README-only P0**. It is allowed to gain **this one Markdown document**, but it **does not contain the #567 Host correction**. Do not launch it to claim the fixed behaviour. |
| **Prior P0 README-only reference** | `861b81d3540429d241bba0732a610212626f6828` | Historical pre-#567 source. Its former **R-3: successful save reported as 403 if the later listing failed** is historical on this version; it is **fixed in the qualified candidate above**. |
| **#589 report commit** | [Targeted Windows/WSL2 report](https://github.com/jermur788/CCF/blob/10d400040b93cc53667b98d5eec4bb0bf0837489/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-Sitka-P0-567-Targeted-WSL2-Edge-Regression-Result-20261116.md) | Qualification **evidence**, not the implementation base. The report was added without modifying the qualified source. |

The tested reference is the code at the qualified commit above, paired with its checked-in synthetic fixtures, schema, `global.json` and UI. **Do not substitute `main`, the #568 docs branch, or a floating latest branch checkout.** The P1 four-file specification is still **DRAFT / NOT APPROVED**; the Manager's selection of this P0 base for a *future authorised* P1 does not authorise P1 implementation.

**Primary reads:** [P0 README at qualified source](https://github.com/jermur788/CCF/blob/a7f41983c2f2f233b8fe999d1a5b10e72bdd2866/Tools/MultiRackSitkaLab/README.md), [file/schema contract](https://github.com/jermur788/CCF/blob/a7f41983c2f2f233b8fe999d1a5b10e72bdd2866/Tools/MultiRackSitkaLab/schemas/lab-file-v1.md), [#567 Host source](https://github.com/jermur788/CCF/blob/a7f41983c2f2f233b8fe999d1a5b10e72bdd2866/Tools/MultiRackSitkaLab/src/Host/LabHost.cs), [#589 acceptance and exact-base ruling](https://github.com/jermur788/CCF/blob/docs/multi-sitka-p0-589-manager-acceptance-and-p1-base-20261116-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-Sitka-P0-589-MANAGER-WSL2-EDGE-ACCEPTANCE-AND-P1-BASE-RULING-20261116.md).

## 2. Known-working machine route and preflight

**The tested configuration** was the owner's Windows 11 PC → **Ubuntu 26.04.1 WSL2 x86_64**, a Git working directory on **WSL's native ext4 Linux filesystem**, user-local **.NET SDK 8.0.134**, and real **Microsoft Edge 157** on Windows. The Host listened on `127.0.0.1:5187`; Windows-to-WSL localhost relay served `http://127.0.0.1:5187/`. This is **not** a certification of native Windows .NET saving, other browsers, other WSL distributions, other architectures or WSL distro shutdown/restart. Use existing SDK 8 on the qualified PC; **no reinstall, sudo, apt, `.bashrc` edit, firewall change, wide bind or system setting change**.

In **Windows PowerShell**, check the installed distro, then enter it:

```powershell
wsl.exe --list --verbose
# Confirm Ubuntu is VERSION 2, then:
wsl.exe -d Ubuntu
```

In **Ubuntu**, use the existing Linux-native repository/worktree if it already contains the **exact qualified source** and is owned for this purpose. Do **not** use a Windows GitHub Desktop checkout reached through `/mnt/c` as if it were the verified ext4 execution route. GitHub Desktop on Windows is fine for viewing the GitHub branch, files and commit, but does not make a Windows filesystem checkout or native Windows Host qualify for Linux no-follow saving.

If an **existing WSL-native Git repository** is available (the #589 worker identified `/home/jer/Development/CCF-HP` on the qualified machine), an operator can create a **new, unused** dedicated worktree without modifying another worker's checkout. These commands are **illustrative future operator actions, not executed in #568**:

```bash
# Inside Ubuntu WSL2; replace this directory only if your existing WSL-native repo differs.
cd "$HOME/Development/CCF-HP"
git fetch origin fix/multi-sitka-p0-save-response-r3-20261116-v1
# Choose a fresh, unused target path. Do not remove/overwrite an existing worktree.
test ! -e "$HOME/ccf-worktrees/multi-sitka-p0-operator-qualified" || { echo "STOP: path already exists"; exit 1; }
git worktree add --detach "$HOME/ccf-worktrees/multi-sitka-p0-operator-qualified" a7f41983c2f2f233b8fe999d1a5b10e72bdd2866
cd "$HOME/ccf-worktrees/multi-sitka-p0-operator-qualified"
git rev-parse HEAD
findmnt -T . -o TARGET,FSTYPE
```

If there is **no existing WSL-native repository**, use a **new path under `$HOME`** instead of a Windows GitHub Desktop clone:

```bash
mkdir -p "$HOME/ccf-worktrees"
# Only if the destination does not already exist; repository may be large.
git clone https://github.com/jermur788/CCF.git "$HOME/ccf-worktrees/multi-sitka-p0-operator-qualified"
cd "$HOME/ccf-worktrees/multi-sitka-p0-operator-qualified"
git checkout --detach a7f41983c2f2f233b8fe999d1a5b10e72bdd2866
git rev-parse HEAD
findmnt -T . -o TARGET,FSTYPE
```

**STOP if** the printed HEAD does not exactly match the qualified source commit, the new directory is on `/mnt/c`/NTFS rather than WSL ext4, the destination belongs to another worker, the working tree has unrelated work, or an unrelated Host already owns the port. Do not force checkout, clean, reset, kill processes or borrow another owner's files. The worktree example intentionally uses a *new* folder and a detached exact commit.

From that **qualified worktree**, in the Ubuntu shell:

```bash
export DOTNET_ROOT="$HOME/.dotnet"
export PATH="$DOTNET_ROOT:$PATH"
cd Tools/MultiRackSitkaLab
cat global.json               # SDK 8.0.133, rollForward latestPatch
dotnet --version              # 8.0.134 on the tested PC (an 8.0.x SDK is required)
dotnet run --project src/Host
# Expected: Listening on http://127.0.0.1:5187 (loopback only; Ctrl+C to stop)
```

Do not launch the Host from the #568 documentation branch. Keep the **same Ubuntu shell/process** open and open **Microsoft Edge on Windows** at **http://127.0.0.1:5187/**. The health endpoint is `http://127.0.0.1:5187/api/health` and should report `status:"ok"`, `bind:"127.0.0.1"`, `port:5187`, `banner:"SYNTHETIC TEST FOREST"`. The UI must display both **SYNTHETIC TEST FOREST** and **NO BIOLOGICAL GROWTH**. **Do not use `localhost:5187`: it deliberately returns 403.** Port 5187 is the verified route; the README documents a configurable fallback port, but no port change is needed or claimed qualified here.

**Stop safely:** focus the Ubuntu shell that you personally started with `dotnet run --project src/Host` and press **Ctrl+C**. Confirm that **your** process exits and 5187 is no longer served; do not run `killall dotnet`, stop another worker's Host, close other Edge windows, shut down/terminate `Ubuntu`, or edit user saves. A **Host process restart** is part of the qualified smoke path. A **WSL distribution shutdown/restart is not tested** and requires separate owner permission.

## 3. 15-minute *operator* smoke and regression checklist — NOT RUN by #568

**Assumptions:** setup/checkout/SDK already prepared; dedicated worktree and new isolated `.lab-data/`; Windows Edge can reach the Host. Allow extra time for first build if needed. Treat this as a **repeatable checklist**, not retrospective #568 PASS evidence. Capture a screenshot/receipt at each boundary; only mark actual outcomes after performing them.

| Minutes | Operator action (real Windows Edge unless noted) | Expected evidence / pass criterion |
|---|---|---|
| 0–2 | Verify Ubuntu VERSION 2, ext4 working directory, exact qualified commit, `dotnet --version`, Host listening and `/api/health`. Open `127.0.0.1:5187` in **Edge**. | Exact pin and environment noted; synthetic/no-growth banners visible; no unexpected 403, host binds loopback only. |
| 2–3 | In **Checked-in fixture**, select `sitka-flat-demo-v1`, click **Reset to fixture… → Confirm reset**. Use **Fit plot**, zoom/pan and select an eligible live adult using **Select ID** `T0062`. | 162 synthetic trees fixture; inspector tree ID and Core-computed selection preview match; selecting does not fell. Screenshot UI metrics and selection. |
| 3–5 | Click **Mark selected → Create plan from marked trees**; in the new `P0001` plan, **Propose → Approve → Queue** one by one. | Visible `DRAFT → PROPOSED → APPROVED → PENDING_WORK`; until Execute, eligible N and BA unchanged. Record the plan ID/state and metrics. |
| 5–6 | Click **Execute test work** for `P0001`. | `P0001 EXECUTED`; `T0062` becomes a felled stump, eligible N falls by **one**, BA falls by that tree's Core-calculated amount; other stems unchanged. Record post-execution metrics/event. |
| 6–8 | Select `T0017` via **Select ID**; **Mark selected → Create plan from marked trees → Propose → Approve**, deliberately **do not queue/execute**. | Second plan `P0002 APPROVED`; `T0017` remains live/marked, forest N and BA unchanged. This is the intentional stale-on-load witness. |
| 8–10 | Set **Save as** to a new safe bare name such as `smoke568-p0`; click **Save to lab data**, then **Verify event replay**. Optionally save same name once more to check overwrite/list de-duplication, **only in the dedicated disposable lab folder**. | Status **`OK: saved smoke568-p0.sitkalab.json`**, replay says **`Replay OK`** and matches current state; corresponding single listed lab filename; no felling or event added by Save. Record UI and response, not just a screenshot of the dropdown. |
| 10–12 | Press **Ctrl+C** in **your Host shell only**; confirm no listener. Run `dotnet run --project src/Host` again in that same qualified directory. Return to Edge and press **F5**. | Fresh in-memory Host state (default fixture, no executed work) **until** a saved file is loaded; `GET /api/saves`/dropdown still finds the lab save. **No distro restart**. |
| 12–14 | In **Lab file**, choose `smoke568-p0.sitkalab.json`, click **Load selected**. Inspect plan states and felled tree. Click **Verify event replay**. | Executed work and stand metrics restored; `P0001 EXECUTED` remains. Old `P0002 APPROVED` becomes **STALE**, `T0017` is **released/unmarked**, and a `PlanStale` audit event changes the post-load hash. Replay still matches **current post-load** state. |
| 14–15 | Attempt **Execute** on stale `P0002` to see a refusal. If time permits, exercise **Advance experiment step (calendar only)** on a fresh unexecuted approved plan; stop only your own Host. | Expected `Refused [PlanStale]` / **no additional felling** (audit-only rejection). Calendar step changes counter/revision and stales approvals, **not DBH/height/age**. Save last screenshot/notes. |

**Real #589 reference witness (not a required new exact hash):** after felling `T0062`, that worker observed **137 eligible adults**, **8.88468556194034 m²** BA, `P0001 EXECUTED`, and `P0002 APPROVED` before save. After process restart+load, `P0002 STALE`, the prior `T0017` mark released, one additional `PlanStale` event and successful replay. The **change** in event count/hash is correct when an outstanding approval exists at load. Do not demand a byte-identical post-load hash in that situation.

**Evidence to retain for an actual operator run (do not invent):** selected Git commit/working filesystem, OS/distro/SDK/Edge versions, Host console listener and health JSON, UI start banner/fixture and before/after N+BA, plan states before and after execute, save response (`httpStatus, ok, code, fileName, files, listRefreshed, listCode`), filename observed in `.lab-data/` without reading other saves, restart/reload/load screenshots, stale-plan refusal, replay result, and cleanup of **only** disposable files/processes the operator owns. A checkbox with no observation is **NOT RUN**, not PASS.

## 4. Save, reload, listing and trust boundary — current corrected P0

1. **A save is committed at native `renameat` into the already opened lab folder handle.** It is not a claim about `fsync`, power-loss durability or permanent path/inode identity. P0 uses Linux `openat`/`statx`/`renameat`/`unlinkat` and refuses unsupported platforms rather than weakening file safety.
2. **Normal save (qualified #567 source):** HTTP **200**, `ok:true`, `code:"ok"`, the correct `fileName`, `files:[...]`, `listRefreshed:true`, `listCode:null`. The real #589 Edge save, overwrite and second save all received this response.
3. **Committed save but *later optional list refresh fails*:** HTTP **200**, `ok:true`, the correct `fileName`, **`files:null`**, **`listRefreshed:false`**, and a non-path `listCode` such as `SymlinkRejected` or generic `ListFailed`. The UI must report a **successful save with a list warning** and **retain the prior dropdown list**; it must not display a failed save or clear the list. A freshly committed file might not appear in the dropdown until a later successful list refresh/page reload. If listing continues to refuse, investigate the folder rather than bypassing a guard.
4. **Pre-commit refusal remains a refusal**, e.g. symlink/root exchange can produce **403** and a refused status; malformed bare name such as `../evil` produces **400 `PathRejected`**. A refusal is not proof of a successful write. **Historical only:** the older README-only P0 could incorrectly return 403 *after* a successful rename during a race (#517 R-3). **Do not describe that fixed bug as present in the qualified #567 candidate.**
5. Save input is a bare **1–64-character** `[A-Za-z0-9_-]` name; the Host creates `<name>.sitkalab.json` **only** in this worktree's Git-ignored `Tools/MultiRackSitkaLab/.lab-data/`. There is no arbitrary destination path or game-save import. Symlinks, special files, hard-linked targets, a linked lab-root and noncanonical/wrong-format files are refused; same-user real-directory replacement **before** opening and symlinked **ancestor** components remain disclosed limits, not grounds for assuming absolute directory immutability.
6. **Restart semantics:** the Host keeps the active experiment in memory; restarting **only the Host process** returns to the default fixture. Previously saved lab files remain on the Linux filesystem. On loading a file, `APPROVED` and `PENDING_WORK` plans become **STALE**, and their marks are released. Already **EXECUTED** work persists. Staleness adds an auditable event and changes the hash; if no outstanding approvals exist, the file may restore without that state change. Browser F5 is a page refresh, not a WSL distro restart.
7. **One local operator only.** Saved file integrity is checked against the canonical `ccf-sitka-lab` v1 Synthetic format and deterministic replay from its checked-in fixture, not against real-world forestry measurements. Do not manually edit, reformat or copy in game saves.

**Evidence granularity:** #589 saw the **real 200/list-refreshed** responses in Windows Edge and a **simulated one-time `files:null` response** in Edge to check UI presentation. The actual *production Host* post-commit-list-failure case was exercised in WSL Host deterministic injection and unmodified-file-layer stress (226 such 200 responses), **not by inducing a live Host failure during the Edge Save click**. Do not conflate those observations.

## 5. Failure triage without altering the safety policy

| Symptom | Likely explanation / safe operator response |
|---|---|
| `dotnet --version` selects 10.x or reports missing compatible SDK | `global.json` pins **8.0.133/latestPatch**. Set `DOTNET_ROOT` and the **per-shell** PATH to existing `~/.dotnet`, retry. On the qualified PC SDK 8.0.134 already exists; **do not** edit `global.json` or install system-wide. A different machine requires owner-approved SDK setup. |
| Host never prints loopback listener / Edge cannot connect | Confirm the **qualified** source directory, Host shell is still running, and `127.0.0.1:5187/api/health` responds. Check 5187 ownership before doing anything. Stop **only your own** previous Host; do not kill somebody else's process, expose `0.0.0.0` or change firewall/CSRF policy. |
| URL `http://localhost:5187/` reports 403 | Expected Host check. Change the **URL**, not the Host guard: use exactly **`http://127.0.0.1:5187/`**. |
| POST reports 403 / 415 or curl differs from Edge | Host requires exact Host/Origin, same-origin browser POST, `X-Sitka-Lab: 1`, and JSON content type. Do not relax checks or use a foreign Origin. Windows PowerShell 5.1 can strip quotes in inline curl JSON; use a JSON file for controlled requests if an authorised technician needs diagnostics. |
| Lab saves/list/load return **501 `PlatformUnsupported`** | Usually native Windows/macOS Host, wrong OS/architecture, or Linux file self-test failure. Use the **known Ubuntu WSL2 x86_64/ext4 route**; do not bypass the native no-follow checks. Linux arm64 remains untested. |
| Save shows **`OK: saved …; the lab file list could not be refreshed`** | On qualified source, that indicates a committed save with **optional list failure**, not a failed rename. `files:null` keeps the previous dropdown. When the folder/list service is healthy, try a clean page refresh; investigate repeat `SymlinkRejected` rather than deleting or replacing the folder. Never infer that a current dropdown is authoritative if refresh failed. |
| `Refused [PathRejected]` on Save | Save field takes only a bare name; no slash, traversal, extension or spaces. Use `smoke568-p0` rather than `../evil` or a path. |
| `Refused [SymlinkRejected]` / `NotRegularFile` / `NotListed` | Safety refusal. Check only the dedicated scratch lab folder and correct the **operator-controlled setup**; do not follow links, modify protected files or disable guards. |
| Approved/pending plan turns STALE, mark disappears after Load | **Expected** session safety rule: loading invalidates old approvals and releases their trees. Reassess/re-mark and create a new plan for further work; do not try to force-execute a stale plan. |
| After **Advance experiment step**, tree diameter/height/age do not change | **Expected P0**; this step is calendar/revision only, **no biology**. |
| Overlapping trees seem hard to select at fit zoom | #514 saw four overlapping hit targets where the front-most SVG stem was selected. Zoom in or use **Select ID**, then confirm ID in the inspector before marking. Do not infer a selection bug from a misidentified stem. |
| Replay mismatch, unexpected felling, changed source pin or unexpected file write | **STOP**, preserve the disposable evidence and report exact source/version/steps. Do not reset protection anchors or call this a pass. |

## 6. Qualification inheritance and *not-run* register

| Evidence source | Actual historical result (not a #568 rerun) | Scope boundary |
|---|---|---|
| [#514 accepted Windows/WSL P0 baseline](https://github.com/jermur788/CCF/blob/docs/manager-multi-008-wsl-acceptance-readme-20261114-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MANAGER-MULTI-008-WINDOWS-WSL2-QUALIFICATION-ACCEPTANCE-20261114.md) | Owner PC: real Edge+Ubuntu WSL2, Core **82/82**, Host **13/13**, live security matrix **63/63**, save/restart/load/replay, 2,000-tree UI selection **max 62 ms** over 30 clicks. Manager accepted the tested route. | **Earlier source**, before #567 save-response fix. Its R-3 warning was accurate **then**. Not source verification of the later fix. |
| [#567 author](https://github.com/jermur788/CCF/issues/567) and [#586 independent security review](https://github.com/jermur788/CCF/blob/review/multi-sitka-p0-567-save-response-security-20261116-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-Sitka-P0-567-Independent-Source-Security-Review-20261116.md) | Author reported Host **16/16**, Core **82/82**, Chromium **15/15**, mutation **35/35 + new 6/6** on native Linux. Independent GPT-6 **source-only** review approved normal production file-access fix, zero blocking/major. | Author-side runs **not** independently re-executed by #586; two nonblocking source/security minors and a diagnostic nit remain. |
| [#589 targeted delta](https://github.com/jermur788/CCF/blob/10d400040b93cc53667b98d5eec4bb0bf0837489/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-Sitka-P0-567-Targeted-WSL2-Edge-Regression-Result-20261116.md) and [Manager acceptance](https://github.com/jermur788/CCF/blob/docs/multi-sitka-p0-589-manager-acceptance-and-p1-base-20261116-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-Sitka-P0-589-MANAGER-WSL2-EDGE-ACCEPTANCE-AND-P1-BASE-RULING-20261116.md) | **Corrected exact source**: WSL Host **16/16**, schema 3 fixtures + six negatives, **887,138** same-user folder exchanges with **zero alternate writes** and **226** `200/listRefreshed:false`, real Windows Edge saves/overwrite/load/replay/guards, Host-process restart and intentional STALE release. Manager **APPROVED** targeted tested-configuration P0 delta. | No new Core 82/82 or 63-case security rerun; real live Edge UI saw a **simulated** postcommit-list-failure response; no distro shutdown/restart, native Windows save, another browser or power-loss durability. |
| **#568** — this document | **Documentation authored and GitHub source read only; NO runtime, browser, .NET, Linux, Windows, WSL, Unity, schema or mutation tests run.** | Checklist above remains **NOT RUN** until an operator performs it and captures evidence. |

**Current P0 function:** accurate individual synthetic-tree geometry, area/BA/N/N/ha/BA/ha/Dq arithmetic, mark → plan → approve → queue → atomic execute, deterministic event history/replay, lab-only save/load, 2D map. **Excluded:** adult annual growth, crown dynamics, natural mortality, regeneration, species mix, multi-rack road/extraction geometry, economics, wind, terrain, real inventory ingestion, hazard validation, forecast confidence, Unity/Scenario One, production saves. The three-tree quarter-hectare oracle is arithmetic-only and **cannot demonstrate realistic hazard or CCF ecological dynamics**. A step is never growth. No unreviewed P1 DRAFT behaviour can be represented as shipped P0.

**Open technical/product limits:** no `fsync`/power-loss assurance; same-user real-directory replacement before open; ancestor symlink handling; test-only hook exception after rename; generic fallback list error; portability beyond Windows Edge/Ubuntu WSL2 x86_64 and native Linux x64; limited accessibility; no player-facing release; P1 policies, spec approval, platform/time budget and future field/GROWFOR validation unresolved. None is repaired or decided by writing an operator document. No full legacy test repeat is prescribed without a substantive new regression.

## 7. #568 delivery and management handback

- **What changed:** exactly this operator Markdown document in the precreated **older-source docs branch**; no executable code changes or runtime operation.
- **Verification for this task:** GitHub source/README/schema/UI/Host and #514/#586/#589 report **inspected**; GitHub one-file comparison/readback to be recorded in the [#568 claim](https://github.com/jermur788/CCF/issues/568). Runbook steps **NOT RUN**, so no #568 smoke PASS.
- **Meaning:** operator instructions target the **already independently reviewed and accepted corrected P0 code**, without misrepresenting the documentation branch as containing that fix. The qualification belongs to the exact tested reference, not to this report's Git branch.
- **Next:** #568 claim [HANDED-OFF] and notify [Multi 2D manager/orchestrator #565](https://github.com/jermur788/CCF/issues/565) and [Overall portfolio #425](https://github.com/jermur788/CCF/issues/425). No further qualification or test loop needed solely because this doc was written; P1 remains gated separately.
- **User action:** **none required for documentation closure**. When choosing to run the lab, the operator must use the exact qualified source in an owned WSL ext4 checkout; no distro termination or Windows build is authorised.
- **Dashboard and Drive mirror sync:** **NOT VERIFIED**; this documentation task does not update either and makes no canonical-context source change.
