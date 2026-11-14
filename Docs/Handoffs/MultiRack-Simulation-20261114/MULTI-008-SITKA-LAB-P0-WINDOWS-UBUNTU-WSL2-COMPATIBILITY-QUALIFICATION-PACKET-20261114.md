# MULTI-008 — Make the standalone Sitka 2D Lab P0 usable on Windows via the owner's installed Ubuntu WSL2

**Issued:** 14 November 2026  
**Claim:** [CCF #514](https://github.com/jermur788/CCF/issues/514), **WAITING ON MULTI-007 / NOT RUNNING**; no WSL worker, machine lock or local worktree has been registered.  
**Product authority:** **latest direct owner instruction, 14 November 2026:** "There needs to be a windows version or a version that works in the windows sub system with Ubuntu I have installed already".  
**Manager scope decision:** choose the **Ubuntu WSL2 route first**, to satisfy Windows usability without prematurely porting Linux-specific secure filesystem operations to native Windows. A **genuinely usable Windows browser on the owner's machine**, connected to the Sitka lab hosted in their WSL Ubuntu, is the actual acceptance criterion. Merely running C# inside a generic Linux environment is **insufficient**. Native Windows executable is not separately required if this route passes.  
**Difficulty:** **MEDIUM**, targeted real Windows + Ubuntu WSL2 environment qualification with secure loopback browser integration, possible small deployment/instruction corrections, not forestry simulation redesign.  
**Best-suited worker:** **Sol/Codex High in local T3 Code with actual access to the owner's Windows host and existing Ubuntu WSL**, or equivalent capable local implementation/qualification worker. A worker operating only on the separate Linux laptop cannot certify this user's Windows installation.  
**Strict dependency:** [MULTI-007 #511](https://github.com/jermur788/CCF/issues/511) is **RUNNING** (independent review of MULTI-006 R-1/R-2). **Do not start mutable WSL qualification or use unreviewed code as an accepted baseline until MULTI-007 returns a published Manager-accepted APPROVED.** If NEEDS CHANGES, stop this task as BLOCKED and reconcile candidate after the security fix. It is safe to discuss/inspect the Windows prerequisites read-only, if physically authorised, but do not register an implementation PASS or modify shared source while #511 remains unresolved.

## 1. Approved lab authority, candidate and platform boundary

- [Direct-user-approved P0 four-file specification](https://github.com/jermur788/CCF/blob/docs/multi-001-sitka-lab-p0-approved-spec-20261114-v1/specs/multirack-sitka-2d-reference-lab-v1/spec.md): standalone artificial flat Sitka forest, C# Core and loopback UI, explicit mark/propose/approve/queue/execute, synthetic fixtures, replay and isolated lab saves; **NO BIOLOGICAL GROWTH**. This task verifies a new deployment environment, not a new species, ecological model, game mode or 3D integration.
- [Latest MULTI-006 writer implementation handoff](https://github.com/jermur788/CCF/blob/feature/multi-006-sitka-lab-p0-handle-anchored-writes-20261114-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-006-SITKA-LAB-P0-WRITE-SAFETY-REPAIR-HANDOFF-20261114.md). The tested code was handoff-final branch \`feature/multi-006-sitka-lab-p0-handle-anchored-writes-20261114-v1\`. **Current pinned candidate HEAD \`c445f8700610a7e6a835fef9ecec39eb9432128d\` is STILL IN INDEPENDENT REVIEW**, not yet accepted for the security repair.
- Created **prospective execution branch, initially untouched**: \`feature/multi-008-sitka-lab-wsl2-ubuntu-qualification-20261114-v1\` from that candidate, **conditional** on the #511 independent verdict and eventual Manager candidate selection. If review requires source edits, do not quietly build from this branch: ask the Manager to rebase/replace the authorised starting point without destroying work.
- Proposed WSL Linux-native worktree (NOT CREATED / NOT RESERVED): \`~/ccf-worktrees/CCF-MULTI-008-SitkaLab-WSL2-20261114\` inside the user's Ubuntu distribution's ext4 filesystem. Do not use a guessed Windows account name, \`/mnt/c\` repository root, another author's worktree, or the separate Linux laptop's Files-drive paths.
- Proposed off-repo test logs in a **reviewer-owned, user-approved WSL local directory**, to be recorded at preflight. No evidence has yet been generated.
- Native Linux x64 on the separate laptop passed previous engineering P0 review; **Windows WSL2 and Windows browser have not been run**, so no Windows-facing compatibility assertion is justified yet. Linux arm64 also untested. Native Windows/macOS \`NoFollowFiles\` intentionally fails closed with HTTP 501 for lab saves; WSL2 runs a Linux kernel and might satisfy Linux x64 requirements, but this **must be tested on the actual installed distro**.

**Official environment documentation**: [Microsoft WSL install and version checks](https://learn.microsoft.com/windows/wsl/install), [Microsoft Windows↔WSL localhost networking](https://learn.microsoft.com/windows/wsl/networking). Windows-to-WSL localhost forwarding is documented, but not guaranteed on a particular user's configuration. **The lab's Host-header security check requires literal \`127.0.0.1\`, not \`localhost\`**, so test the Windows browser at **\`http://127.0.0.1:5187/\`**. Do not weaken Host/Origin/CORS, bind to \`0.0.0.0\`, change Windows firewall or expose the app to the LAN as a workaround.

## 2. Preflight — exact user's Windows and Ubuntu WSL, not a substitute machine

**Before tests/edits:** refresh source issue [#514](https://github.com/jermur788/CCF/issues/514), [#511](https://github.com/jermur788/CCF/issues/511), live [active-tasks.md](https://github.com/jermur788/CCF/blob/main/Docs/Project/coordination/active-tasks.md), actual Linux/Windows host, git branch/worktree/dirty state and the native toolchain. Do not infer Windows access from the separate \`jer-laptop\` record. If #511 RUNNING, hold this task **WAITING**, no tests touching its candidate. If #511 NEEDS CHANGES, mark #514 **BLOCKED** pending updated candidate.

Read-only check on **Windows PowerShell** (not inside Ubuntu):
\`\`\`powershell
wsl.exe --list --verbose
wsl.exe --status
\`\`\`
Use the actual distro name returned; do not assume \`Ubuntu\` if the installed distribution is named \`Ubuntu-24.04\`. Require **WSL VERSION 2** (kernel-backed Linux). If it is WSL1 or WSL unavailable, report BLOCKED and request approval **before** converting/updating the owner's WSL installation; do not run \`wsl --set-version\` automatically. Do not reinstall Ubuntu, change the user's default distro, modify \`.wslconfig\`, Windows Firewall or networking settings without explicit owner approval.

Read-only checks **inside that Ubuntu**:
\`\`\`bash
uname -a
uname -m
cat /etc/os-release
dotnet --info
command -v git
test -d /proc/self
\`\`\`
Record host Windows build/WSL version, Ubuntu release/kernel, architecture (prefer x86_64; aarch64 remains **conditional/untested**), glibc/kernel statx ability, available .NET **SDK 8 compatible with \`global.json\`**, Node/browser/testing tools, drive/filesystem, port 5187 conflicts, free disk and existing source ownership. If dependency install/upgrade is needed, request owner's approval before OS/global package changes. Avoid edits to \`/mnt/c\`, user's CCF/Unity game saves, personal files or original worktrees.

**Owner and lock registration:** after #511 approval and a safe host preflight, use a separate WSL worktree/branch, verify clean starting FULL HEAD, and register **actual provider/model, exact Windows machine + WSL distro, branch/worktree, HEAD, read/write scope and phase as [RUNNING] on #514 BEFORE tests or source changes**. The issue is not a file ownership grant; live board and physical workstation owner control collisions. If conflicting host or active lock is observed, stop and mark BLOCKED. Never stash/reset/clean/kill unrelated work or self-grant board rights.

## 3. Minimum WSL acceptance (must be **performed**, not inferred)

**A. Ubuntu Linux Core/Host:** after approved candidate selected, in the isolated WSL ext4 checkout:
\`\`\`bash
cd Tools/MultiRackSitkaLab
dotnet build MultiRackSitkaLab.sln --no-incremental
dotnet run --no-build --project tests/Core.Tests
dotnet run --no-build --project tests/Host.Tests
(cd fixtures/synthetic && sha256sum -c MANIFEST.sha256)
python3 -I -B tests/check-schema.py fixtures/synthetic/*.sitkalab.json
\`\`\`
Writer baseline on native Linux: clean build, **82 Core / 13 Host** tests, fixtures **3/3**, schema pass. WSL must independently reproduce supported actual results; no need to rerun all 35 mutation controls if the same candidate has passed independent MULTI-007 and no source changes occur. If platform-specific failures arise, record them and route precise fixes; never weaken security tests.

**B. WSL safe persistence:** create a synthetic lab session under \`Tools/MultiRackSitkaLab/.lab-data\` on Ubuntu's native filesystem; test new save, list, reload, replay canonical hash, overwrite and reload, cancellation/stale rejection and forbidden symlink/FIFO/traversal. Confirm WSL kernel supports \`openat\`/\`statx\` self-test and handle-anchored save transaction (no HTTP 501). Check \`.lab-data\` resides only under the WSL lab checkout, not \`/mnt/c\` or a production CCF game folder. No real or archived forest save may be read or written. Verify lab files remain after restarting WSL/host as appropriate (without forcing machine shutdown). Keep synthetic identifiers only.

**C. Windows browser ↔ Ubuntu Host:** inside WSL, run \`dotnet run --project src/Host\` and observe the bound host/port. Verify it listens on **\`127.0.0.1:5187\` in Ubuntu only**, no \`0.0.0.0\` or LAN binding. Then **open Microsoft Edge or Chrome running on Windows** at:
\`\`\`
http://127.0.0.1:5187/
\`\`\`
Do **not** use \`http://localhost:5187/\` as the acceptance URL because the lab's Host guard requires the IPv4 literal. If Windows cannot connect, diagnose WSL2 \`localhostForwarding\` / NAT / mirrored networking and port conflicts from official Microsoft guidance; propose a bounded fix, but do not alter system firewall, security guard or WSL global settings without explicit owner permission.

Use real Windows mouse/keyboard and browser network records to verify:
1. 162-tree default stand and label **SYNTHETIC TEST FOREST — NO BIOLOGICAL GROWTH** are rendered.
2. Select actual tree → inspector matches Core ID/XY/DBH and main metrics; clear selection and zoom/pan.
3. Mark → propose → approve → queue → execute one valid eligible adult; confirmation, counts, BA and event history match Core and nothing is felled before Execute.
4. Save \`.sitkalab.json\` in WSL lab, restart/reset as safe → load and replay succeeds; expected approved-plan staleness remains explicit.
5. Invalid/malicious file load, path traversal or hostile Host/Origin is refused; no external network requests, no broad LAN listener, no hidden writes outside lab folder.
6. **Advance experiment step** changes only calendar/event/revision, not age/DBH/height.
7. Record at least one genuine Windows browser screenshot and host stdout/log, noting Windows version, browser, WSL distro, SDK/runtime and actual test source HEAD.

**D. Performance and evidence:** use the 2,000-tree fixture from the **actual Windows browser**, measure selection→inspector→Core preview response on that host, target **≤2 s**, and record repetitions and max; do not substitute earlier laptop Chromium 186 ms for a Windows result. A Windows browser smoke is required even if Ubuntu headless Chromium tests pass. Host/API security on 127.0.0.1 must remain intact.

## 4. Allowed corrections vs blockers

**If all WSL tests pass without code changes:** publish a GitHub Markdown **compatibility qualification + user-start quick guide**, noting exact commands, provenance and limits. Optional **README-only** clarification to replace its misleading "Linux/macOS; Windows/WSL users run same commands" statement with validated WSL2 Ubuntu x64 instructions and native Windows/macOS 501 restriction is allowed **only once source ownership is established**. No broad code change is needed. A docs-only deployment clarification requires no Unity qualification.

**If bounded WSL-specific breakage:** record exact failing environment/port/permission/kernel/runtime before changing code. Code changes, if needed, may only touch **\`Tools/MultiRackSitkaLab/**\`** within a separately claimed single-writer local WSL lane, preserve secure no-follow read/write invariants and go through a **focused independent security/code recheck**. If it requires disabling \`NoFollowFiles\`, loosening host security, writing to Windows files, changing WSL machine configuration, native Windows porting, P1 work or a product-level user workflow redesign, mark BLOCKED and publish a precise Manager decision packet rather than improvising.

**Disallow:** Windows Unity Player/game builds, CCF/Reference Future saves, \`Assets/**\`, \`Packages/**\`, \`ProjectSettings/**\`, main, \`specs/**\` changes, native Windows .NET port as fallback without a separate spec/approval, unapproved global WSL/Firewall installs/settings, real marteloscope data, P1 ecology, hidden external network bind, or editing live \`active-tasks.md\` without authorised board writer. The user only requested the separate lab to work on their Windows system; the Unity Windows game build prohibition remains unaffected.

## 5. Required final handoff and exact completion status

**One specified GitHub report** on the WSL qualification branch:
\`Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-008-SITKA-LAB-P0-WINDOWS-WSL2-UBUNTU-QUALIFICATION-HANDOFF-20261114.md\`.

Include:
- Actual Windows machine and Ubuntu distro/WSL version/architecture, exact model/provider, host/worktree/branch/starting HEAD/source candidate; how ownership and #511 dependency were satisfied.
- Steps and commands with **actual PASS/FAIL/NOT TESTED**, Windows browser screenshot, Core/Host, safe save/replay, security, full Windows UI workflow and 2,000-tree response timing.
- Exact supported route **Windows browser + Ubuntu WSL2 host** (or BLOCKED with precise reason); distinguish native Windows unsupported and Linux arm64 untested. If \`localhost\` aliases fail, explain why literal \`127.0.0.1\` works or does not, without weakening Host security.
- List of code/docs changes, if any, and scoped GitHub diff; headless Linux tests **do not** substitute for real Windows browser verification.
- Direct **user-start recipe** for already installed Ubuntu (PowerShell distro command and Ubuntu dotnet run, Windows browser URL), no passwords, invented account paths, or system-wide changes.
- Screenshot/log paths and hashes off-repo if available; preserve raw evidence. Report **WSL WINDOWS-USABLE PASS / NEEDS CHANGES / BLOCKED** without exaggeration.
- Source/no-Unity/no-production-save integrity and exact GitHub readback/diff, mark #514 HANDED-OFF and close only after durable evidence, notify Manager/#511. If source changed, add a focused independent delta review before marking WSL qualified. Dashboard/Drive sync remains **NOT VERIFIED** unless directly proven.

## 6. Paste-ready dispatch after MULTI-007

> **MULTI-008 / CCF #514 — Windows usability through my installed Ubuntu under WSL2.** Read the entire GitHub packet at https://github.com/jermur788/CCF/blob/docs/multi-008-windows-wsl2-ubuntu-qualification-dispatch-20261114-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-008-SITKA-LAB-P0-WINDOWS-UBUNTU-WSL2-COMPATIBILITY-QUALIFICATION-PACKET-20261114.md . **Do not start mutable work until MULTI-007 #511 has returned a Manager-accepted independent APPROVED**; if it returned NEEDS CHANGES, mark BLOCKED pending updated candidate. Use Sol/Codex High or a capable local T3 Code worker with **real access to my Windows PC and existing Ubuntu WSL installation**, not the other Linux laptop. Difficulty MEDIUM. First read live coordination and actual Windows/WSL physical host/worktree/branch status; Windows PowerShell \`wsl.exe --list --verbose\` must show Ubuntu **VERSION 2**. No machine/WSL/Firewall changes or installs without my permission. Proposed untouched source branch \`feature/multi-008-sitka-lab-wsl2-ubuntu-qualification-20261114-v1\`, conditional on #511 candidate; use a safe new WSL-native ext4 worktree, not \`/mnt/c\`. Register actual worker/model/provider/host/distro/branch/worktree/full starting HEAD as RUNNING on #514 before tests/edits. Build and run the lab under Ubuntu WSL2; reproduce Core/Host and save/replay/security tests; then use **an actual Windows Edge or Chrome browser** at **\`http://127.0.0.1:5187/\`** to prove 2D map, marking→proposal→approval→execution, save/load/replay and 2,000-tree responsiveness ≤2s. Keep guest Host/Origin/CORS security and 127.0.0.1 bind unchanged; don't call native Windows 501 support. Publish exact \`Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-008-SITKA-LAB-P0-WINDOWS-WSL2-UBUNTU-QUALIFICATION-HANDOFF-20261114.md\` with real screenshots/commands and WSL PASS/NEEDS CHANGES/BLOCKED verdict, readback/diff, close #514 HANDED-OFF and return to Manager. No Unity, Windows game build, P1 ecology, real data, protected saves, approved spec or unapproved WSL system changes.
