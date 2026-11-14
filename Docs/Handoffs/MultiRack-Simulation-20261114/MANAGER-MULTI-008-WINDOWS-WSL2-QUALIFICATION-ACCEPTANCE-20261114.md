# Manager decision — MULTI-008 Windows/Ubuntu WSL2 usability qualification accepted

**Date:** 14 November 2026  
**Manager:** OpenAI GPT-6, GitHub-source/management inspection only; **did not operate the owner's Windows PC**.  
**Disposition:** **APPROVED — WINDOWS-USABLE via Ubuntu WSL2 + real Windows Edge for the standalone Sitka 2D lab P0**, with disclosed conditions; NOT native Windows execution, Unity/game Windows Player build, P1 ecology, field accuracy, merge or end-user acceptance of a game release.  
**Claim:** [#514](https://github.com/jermur788/CCF/issues/514), worker [HANDED-OFF] and to be closed after this acceptance.  
**Worker source:** [MULTI-008 Windows/WSL2 qualification handoff](https://github.com/jermur788/CCF/blob/feature/multi-008-sitka-lab-wsl2-ubuntu-qualification-20261114-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-008-SITKA-LAB-P0-WINDOWS-WSL2-UBUNTU-QUALIFICATION-HANDOFF-20261114.md).  
**Binding packet:** [MULTI-008 Windows/WSL2 task](https://github.com/jermur788/CCF/blob/docs/multi-008-windows-wsl2-ubuntu-qualification-dispatch-20261114-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-008-SITKA-LAB-P0-WINDOWS-UBUNTU-WSL2-COMPATIBILITY-QUALIFICATION-PACKET-20261114.md).

## 1. Verified GitHub custody and actual test scope

- Manager fetched the complete published handoff on \`feature/multi-008-sitka-lab-wsl2-ubuntu-qualification-20261114-v1\` and compared it to the previously accepted MULTI-006 candidate. The branch is **exactly one commit ahead, with exactly one new Markdown report, no source, fixture, test, approved spec, Unity, board or other tracked changes**.
- The worker was **Claude Sonnet 5.5** in local T3 Code on the **owner's Windows PC**, using \`Ubuntu\` WSL2 x86_64, an isolated WSL ext4 git worktree, and a real Windows Edge 157 window with Windows mouse/keyboard input. Actual RUNNING claim preceded local tests.
- **User-side authorisation:** SDK 8 installed user-locally under \`~/.dotnet\` with owner approval. No sudo, apt, \`.bashrc\`, global WSL configuration, network/firewall or system settings changed.
- **Observed by worker and recorded in handoff:** Windows PowerShell \`wsl.exe --list --verbose\` showed \`Ubuntu\` VERSION 2; Ubuntu kernel had Linux openat/statx/renameat/unlinkat support; SDK 8.0.134 selected via explicit DOTNET_ROOT/PATH (system .NET 10 alone did not satisfy pinned SDK8).
- **Worker-run Tier 1:** clean build 0 warnings/errors, Core **82/82**, Host **13/13**, fixture manifest **3/3**, schema and negative controls pass. Separate run of original mutation 35/35 and Linux headless Chromium were **not repeated** on WSL, correctly scoped because there were no source changes; the earlier MULTI-007 independent Linux x64 review retains its own 35/35 and 15/15.
- **Worker-run live WSL host/security:** **63/63** checks: versioned lab saves, list/reload/replay, overwrite, stale-on-load, malicious path/input/Host/Origin/CSRF requests, special files and linked entries rejected. File mode 0600 and WSL ext4 only; no CCF game saves or Windows paths.
- **Actual Windows GUI:** Windows Edge visually showed synthetic demo, inspector/Core parity, pan/zoom/fit, **Mark → Propose → Approve → Queue → Execute**, no premature felling, stand metric changes consistent with Core, saved file then **HOST PROCESS** restart + Load + successful replay, stale approval, calendar-only step, malformed-file refusal, narrow-layout usability. The worker recorded 17 screenshots under WSL home with its manifest, not committed to Git.
- **Performance:** 2,000-tree selection+inspector+Core preview on Windows Edge **max 62 ms across 30 clicks** (median 51.3ms) versus ≤2s requirement. Four overlapping hit targets at fit zoom selected the topmost neighbour, not the intended stem; inspector/preview still corresponded to what was actually selected.
- **Host exposure:** Linux socket bound **only** \`127.0.0.1:5187\`, Windows-to-WSL localhost relay loopback, LAN/vEthernet/WSL VM address unreachable. Windows browser URL exactly **http://127.0.0.1:5187/**. \`localhost\` returns 403 due to intentional Host guard; do not loosen Host/Origin or bind \`0.0.0.0\`.

**Evidence classification:** This is a **single qualified worker's real Windows/WSL execution evidence**, source handoff and GitHub file-scope independently checked by Manager. The Manager **did not re-run** the Windows browser or directly open the worker's off-repo screenshots/CSV in this GitHub-only session. These facts are not second-worker independent runtime corroboration. No source delta means no additional source-review claim is necessary to accept this bounded platform qualification.

## 2. Manager's SDK decision, docs update and limitations

**Decision: KEEP \`global.json\` pinned to .NET SDK 8; use documentation-only launch instructions.** Do not relax to .NET 10 or change \`TargetFramework\`, dependency resolution, source security or expected canonical file hashes just to avoid a one-time user-local SDK 8 install already completed with approval.

The manager made a **README-only** edit, on this separate documentation branch based on the unchanged WSL-qualified handoff, not on the worker's tested branch:
\`Tools/MultiRackSitkaLab/README.md\`

It now states:
- Windows-supported route is **Edge on Windows → Ubuntu WSL2 x86_64 lab host**, explicitly **NOT** native Windows .NET file access.
- Windows PowerShell \`wsl.exe -d Ubuntu\`, explicit per-shell SDK8 \`export DOTNET_ROOT="$HOME/.dotnet" PATH="$HOME/.dotnet:$PATH"\`, WSL-native checkout and run \`dotnet run --project src/Host\`, open \`http://127.0.0.1:5187/\`, stop with Ctrl+C.
- The SDK 8 was already installed user-locally on this PC, so **no reinstall** is needed. Native Windows/macOS lab file operations return 501; Chrome and WSL distro restart are **not tested**.
- The previous ambiguous "links are never followed" claim is scoped accurately to the opened root/final component. Ancestor symlinks can resolve normally, and replacement by a different real directory **before open** remains an accepted same-user P0 limitation; no permanent inode pinning is claimed.
- One docs-only edit **does not change the qualified C# source, security implementation or WSL test results**. README publication itself was not independently exercised as a runtime build.

**Acceptance limits / remaining optional checks:**
1. **Full \`wsl.exe --terminate Ubuntu\` distro restart NOT TESTED.** The worker correctly left the user's existing shell open. **Do not terminate Ubuntu automatically**; owner may elect to close existing sessions and run the worker's short three-step restart persistence check. A tested **lab host process restart already PASSED** and is sufficient for today's Windows usability qualification.
2. **Microsoft Edge tested; Chrome NOT TESTED.** Use Edge as the known-working browser. No claim of cross-browser certification.
3. Early uninstrumented clicks sometimes did not act; later event-instrumented Windows input showed every click received by the page worked, with one OS-level input delivery retry. No proven app-side bug; record as observation for any later usability testing rather than silently closing it.
4. **R-3 non-blocking**: an HTTP 403 false negative after a successful save under intense same-user folder exchange remains tracked [#517](https://github.com/jermur788/CCF/issues/517), not a blocker. Persistent-folder identity is not promised, nor is protection against a different real directory substituted before each operation.
5. Native Windows host lab saves intentionally refuse HTTP 501. Linux arm64 untested. WSL2 on this specific Windows x64/Ubuntu installation **PASSED**. No Windows Unity Player build authorised or performed.
6. Synthetic fixed fixtures only, **NO BIOLOGICAL GROWTH**, no real Irish inventory, no P1 scope, no Unity/Scenario One integration, no game production save access.

## 3. Publication, next step and user action

**Current Windows user-start recipe:** read \`Tools/MultiRackSitkaLab/README.md\` on this docs branch, or the exact already-tested recipe in MULTI-008 handoff §6. The owner can run the lab now using their existing installed SDK8, without more system changes.

**Task status:** [#514](https://github.com/jermur788/CCF/issues/514) to be marked **HANDED-OFF/CLOSED — WSL WINDOWS-USABLE PASS** now. This does not constitute user playtest acceptance. No ongoing worker, WSL host, Chromium/Edge debug profile or machine lock is claimed by Manager. The application has **not been merged to \`main\`** by this docs-only acceptance.

**Follow-up:** use the working lab as a separate engineering reference tool. R-3 remains deferred [#517](https://github.com/jermur788/CCF/issues/517); a full WSL distro restart is **optional and requires explicit permission** because it terminates existing Ubuntu shells. No immediate new task is needed to make the tested Edge/WSL route usable.

**Dashboard and Drive mirror sync:** NOT VERIFIED; no changes attempted.
