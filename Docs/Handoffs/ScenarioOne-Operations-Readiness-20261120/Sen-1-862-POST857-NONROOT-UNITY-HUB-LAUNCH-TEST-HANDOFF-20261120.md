# Sen-1 #862 — post-#857 Unity release, ordinary-user launch test

**Owner instruction:** 20 November 2026, 10:48 UTC — **TEST AS SOON AS THE ONGOING UNITY TASK FINISHES**, not after an arbitrary scheduling delay. **Do not interrupt current Unity work.** 

**Status at issue creation:** **QUEUED / READY ON VERIFIED PHYSICAL RELEASE / NOT RUNNING**. **Dependency:** actual Unity session/lease belonging to [#857](https://github.com/jermur788/CCF/issues/857), which is now O1/O2 source implementation RUNNING in native T3; current physical release/UID/tool capability not established in browser. **Related parent root-warning problem:** [#861](https://github.com/jermur788/CCF/issues/861). **Programme source/physical manager:** [#844](https://github.com/jermur788/CCF/issues/844). **G5:** owner has specifically instructed this next test; this alone gives no source/permission/lease bypass.

## 1. Handoff trigger — exact and immediate, not an arbitrary deadline

Once the #857 native worker has **finished its current exclusive Editor use and normally released** the live machine-wide Unity lock:
- Check the physical Ops board and `unity-lock status` on **jer-laptop** are **FREE**; no Unity Editor PIDs and no other new custodian. Do **not** infer from GitHub issue closure, an elapsed time estimate, a finished compile, an agent chat message, or an old lease state.
- Verify the current #857 source/serialized scene/file writer no longer holds a conflicting live Unity operation. #857 may continue nonconflicting code review/documentation after release; **do not wait for complete parent orchestration closure** if Unity is truly free.
- Then **immediately hand off to one separately registered test worker** under #844/Overall, check exact actor/candidate/host/supported GUI-control capability, obtain a fresh legitimate exclusive Unity lease, and execute only the narrow test below. Never launch two Editors or steal a held lease. If no agent can actually claim it, report READY-UNASSIGNED rather than saying RUNNING.
- If #857 plans another essential Unity phase before its final release, coordinate an explicit lease handoff with #857 and #844 instead of stealing the session between two active phases.

No new ongoing source implementation, broad Ops investigation, CCF-Ops installation, or ownership-compaction project is a prerequisite for this **test-only** task.

## 2. Firsthand owner observations and existing source-linked evidence

1. Owner's **manual desktop Unity Hub** launch, using normal keyboard or mouse on laptop, does **not** show a root-user warning. Twice an agent launch showed the warning and required owner mouse acknowledgement. **Actual agent invocation/root effective UID remains UNKNOWN**, not proven from screenshots or process records.
2. [#829](https://github.com/jermur788/CCF/issues/829#issuecomment-7018679825) verified **same non-root user and same Unity binary**, but agent's prior *host CLI* launch missed the authenticated Flatpak Hub sandbox/licensing client; owner's GUI Hub launch succeeded. This was a licensing-context diagnosis, not proof of UID 0.
3. [#830](https://github.com/jermur788/CCF/issues/830) later reported successful native Unity import and 71/71 targeted checks through the Flatpak Hub context. Do **not** reset licence, copy tokens or repeat known failed host-mode start.
4. [#861](https://github.com/jermur788/CCF/issues/861#issuecomment-7037533706) records the two alternatives: first reuse existing authenticated non-root Flatpak Hub programmatically; if that fails, an agent with real desktop-control permission can use GUI/mouse/keyboard in the same ordinary user session. Do **not** auto-dismiss the elevated warning.

## 3. Exact bounded test action (PACKET ONLY; complexity Low–Medium)

**Actor:** one native local Sol/Codex / suitable capable worker on **jer-laptop**, genuine registered provider, worktree/branch/HEAD, approved scratch project or disposable scene and actual physical lease. If a separate GUI-control/desktop agent is used, it must be an explicitly permitted specialist and no concurrent Unity launch; a CLI-only ChatGPT GitHub worker has no mouse control by assertion.

**G0 — confirm the launch context without credentials.** Inspect effective UID for current agent and for Unity/Hub process when present, parent launch mechanism (direct CLI/sudo/container versus desktop session), existing Flatpak Hub instance, and XDG/DBus/display availability. Record only sanitized categorical observations (e.g. `UID0 true/false`, `Hub sandbox joined true/false`). Do not read/reveal tokens, licence files/XML, account DB, raw environment, screenshots with secrets, filesystem full paths in public. Record project/cache file ownership without modifying it.

**G1 — ordinary user launch A, preferred.** Under new lawful Unity lease, open exactly an approved disposable Unity project/candidate via the **existing authenticated Flatpak Unity Hub environment**, as the actual ordinary desktop user, without sudo/root or copying credentials. Editor 6000.6.0f1 must launch with nonzero UID, correct licence and no modal. If this works, repeat once (two separate clean launches) and record that automation without mouse is sufficient. No extra proof-of-GUI utility work needed.

**G2 — ordinary user launch B, ONLY if A cannot work.** If an actual available agent is capable of authorised desktop mouse/keyboard control **on that same user session**, launch Editor through Unity Hub GUI exactly as owner does, selecting the approved disposable project. Record actual capability and non-root, no-warning result; if no GUI-control tool is available, report BLOCKED-GUI-CAPABILITY, not assume remote mouse access. Never let a root security warning be automatically acknowledged. No new unrestricted desktop-control or remote-access deployment from this packet.

**Safety stop:** If process effective UID is zero, a root warning appears, Hub licence context is missing, a forbidden old save/project can be touched, a physical lock/owner conflicts, or the local agent identity is unknown, **stop only that launch** and preserve #857's work/lock/branch state. Collect minimal evidence for later #861 narrow actual launcher fix. Do not change Ops scripts, sudoers, global file ownership, installed Unity Hub, license state or game source during this test.

**Success:** two independent Editor launches as verified **non-root** actual desktop user with **zero** root-warning modal, authenticated Hub licence available, clean import/open in authorised scratch/fixture, no root-owned files created, source and historical save unchanged, Editor closes and Unity lock normally released. Note that two successful GUI launches alone don't prove a future headless agent can do the same: distinguish A versus B.

**Verification record:** Native screenshot/console/log only where actually captured (sanitized), actual host, user category/UID nonzero, Unity 6000.6.0f1 and Hub route, branch/head/project, fresh physical pre/post lease status, per-mode PASS/FAIL/NOT RUN, no-false-claims statement, root-owned-file comparison and no-touch old-world invariants. Tier 1 smoke only; no broad gameplay suite, Windows build or main merge.

## 4. One-owner lane, durable handoff and next implementation decision

**Recommended test-only docs branch:** new isolated `docs/sen1-862-nonroot-unity-hub-test-20261120-v1`, starting **actual refreshed current main** rather than any old script's base. Exactly **one NEW** report Markdown in `Docs/Handoffs/ScenarioOne-Operations-Readiness-20261120/Sen-1-862-NONROOT-UNITY-HUB-LAUNCH-TEST-RESULT-20261120.md`. No source/scene/package/ProjectSettings/CCF-Ops/installed scripts/saves edits. If a disposable fixture needs writes, use one explicitly authorised scratch area outside protected game inputs and record clean removal.

On completion fetch/read back the GitHub report, compare start/finish only that Markdown, and post the direct GitHub result to [#862](https://github.com/jermur788/CCF/issues/862), [#861](https://github.com/jermur788/CCF/issues/861), [#857](https://github.com/jermur788/CCF/issues/857), [#844](https://github.com/jermur788/CCF/issues/844) and [Overall #425](https://github.com/jermur788/CCF/issues/425). If A fails and B works, a future bounded launcher/desktop-control bridge task under #861 may be warranted; **do not self-authorise persistent system changes**.

**Current owner action:** None to create this queue. Real execution depends on an actual authorised local worker being available when #857 releases Unity; **this GitHub packet is not an automatic running process**. #857's ongoing work is untouched.

---

## 5. One complete paste-ready native handoff for immediate post-release execution

> **CCF #862 — run this immediate POST-#857 non-root Unity Hub agent-launch test on jer-laptop ONLY AFTER the #857 Editor session and physical unity-lock are verifiably released.** Full direct GitHub packet: https://github.com/jermur788/CCF/blob/Sen-1-Post857-NonRoot-Unity-Hub-Test-Packet-20261120-v1/Docs/Handoffs/ScenarioOne-Operations-Readiness-20261120/Sen-1-862-POST857-NONROOT-UNITY-HUB-LAUNCH-TEST-HANDOFF-20261120.md. Parent issue https://github.com/jermur788/CCF/issues/862; earlier owner warning and known working GUI method https://github.com/jermur788/CCF/issues/861. Difficulty Low–Medium; use an actually available qualified local Sol/Codex worker, or a desktop-capable GUI agent if mode B proves required. Do NOT interrupt #857, take its existing lock, self-grant Ops/source custody, click through root security warnings, touch credentials or launch Unity as UID 0. Read live coordination, register actual provider/model/host/branch/start SHA/test-only disposable project, verify no Editor/PID and current physical lock FREE, then acquire a fresh lawful **exclusive** Unity lease and test preferred non-root programmatic launch through existing authenticated Unity Hub Flatpak sandbox. If it fails and the executor has genuine desktop mouse/keyboard permission, test same-user GUI Hub launch instead. Validate process UID, licensing and no modal, repeat successful route for two launches, preserve saves/project, release lock normally. Publish ONE direct GitHub Markdown result with A/B PASS/FAIL/NOT RUN and follow-up cause/fix, notify #862/#861/#857/#844/#425. Stop only blocked launch; do not alter game source, installed Ops, Hub or system permissions. No Windows build. This packet authorises only the narrowly approved test after live release, not an unattended automatic process.

**Claim status: QUEUED / NOT YET RUNNING; #857 actual physical release and test worker registration remain required.**
