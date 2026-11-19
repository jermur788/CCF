# Sen-1 #824 — Approve #829 Flatpak diagnosis; resume #820 Unity qualification via #830

**19 November 2026 — SCENARIO ONE ONLY.**

**Management decision:** **APPROVED** #829 for read-only, laptop-sourced differential diagnosis with the bounded technical finding **Flatpak Unity Hub sandbox/config-root mismatch**. **NOT** a native Editor startup or gameplay pass. Source composition and Tier 1 #820 remain previously PASS; **Tier 2 NOT RUN**.

**Task next:** [#830 READY — physical G0 only / Unity launch HELD](https://github.com/jermur788/CCF/issues/830), one continuous **Scene One native Editor restart and actual E2 gameplay qualification**, not a new implementation project. Manager [#824](https://github.com/jermur788/CCF/issues/824). Dashboard/Drive NOT VERIFIED.

## 1. Manager's evidence review and verdict

I inspected:
- [Final on-`jer-laptop` #829 result](https://github.com/jermur788/CCF/blob/dd531f8cd9c0359d5f86d425857caa113f3fc2aa/Docs/Handoffs/ScenarioOne-Player-Test-Readiness-20261119/Sen-1-829-LAPTOP-UNITY-LAUNCH-CONTEXT-READONLY-DIAGNOSIS-RESULT-20261119.md), its real Claude Opus 5.5 / Anthropic local T3 Code registration and [HANDED-OFF](https://github.com/jermur788/CCF/issues/829#issuecomment-7018679825).
- GitHub diff for final #829 commit: **one NEW Markdown result path**, no changes to the Editor, game source, assets, save or physical coordination in that Git commit. On-host read-only claims are worker-evidenced, not independently executed by this GitHub-only manager.
- [#820 result](https://github.com/jermur788/CCF/blob/3ff6f406f2766db007a8d48b1bdd2d1c5d487549/Docs/Handoffs/ScenarioOne-Player-Test-Readiness-20261119/Sen-1-820-E2-NATIVE-SCENE-COMPOSITION-GAMEPLAY-AND-REGRESSION-QUALIFICATION-RESULT-20261119.md) and exact owner statement that Unity had worked normally in their own desktop session before the worker failure.

**Observed by #829 local worker:** on same Linux `jer-laptop`, same user and **same Unity 6000.6.0f1 Editor binary**, owner Hub-launched GUI Editor at 16:40 and 16:43 successfully connected to the existing `Unity.Licensing.Client`, updated access token and resolved entitlement. Worker direct-host GUI/batch launches at 17:09 and 17:11 reported the channel did not exist, launched a second licensing client and failed with no access token. Hub itself is a Flatpak `com.unity.UnityHub` with an app-private XDG configuration root and still-running sandboxed licensing client. Sandbox licence-file metadata was updated after owner's refresh; host-default licence root remained stale.

**Conclusion:** **APPROVED: the two launch contexts and their licensing result differ, consistent with and strongly explained by the Flatpak sandbox/config-root boundary.** Same-binary/host alternatives disproven. The exact IPC socket location and the terminated worker's complete environment were **not** independently captured; don't claim more than the logs establish. Distinct missing display/DBus variables are not needed. Owner needs **no additional licence activation**.

**Accepted minimal technical direction, NOT TESTED:** use the existing authorised Flatpak Hub's running instance to launch the **same** Editor inside its sandbox or open the project using the Hub GUI. Official Flatpak reference documents `flatpak enter INSTANCE COMMAND [ARG...]` (https://docs.flatpak.org/en/latest/flatpak-command-reference.html). This validates command **existence**, **not** success for this app, its licence, Unity GUI, `-batchmode`, mounted project or Editor qualification. Do not copy credentials, license XML, databases or cache directories; don't attempt a host-side activation or arbitrary environment-variable redirect.

**Disposition #829:** [MANAGER APPROVED / CLOSED](https://github.com/jermur788/CCF/issues/829#issuecomment-7018807158), with Phase B not authorised by the close.

## 2. #830 exact task assignment

**Purpose in plain English:** Get corrected Compact30's already-composed real Unity candidate past the licensing-startup problem and **actually verify the purchased-Oak/Beech/Work-Plan/year/Annual-Review/old-world game loop**. No new forest model or source coding.

**Difficulty:** **MEDIUM–HIGH** — Flatpak sandbox IPC/Personal entitlement unknown, exclusive machine-wide Unity lock, then full real Unity scene/GUI qualification. Avoid a second source-implementation orchestration.

**Best-suited actor:** **original #820 native T3 Code/OpenCode DeepSeek v4.1 Flash worker on `jer-laptop`**, if the actual session and provider remain available, or a fresh capable **Linux-native T3 Code/OpenCode worker**; not browser/Windows/WSL. No automatic reuse of #820's old identity, claim, Unity lease or source ownership. One parent, no fork needed unless a genuinely substantial new defect is discovered.

**Claim:** [#830 — READY G0 / UNITY HELD](https://github.com/jermur788/CCF/issues/830).
**Packet branch:** `docs/sen-1-flatpak-editor-resume-packet-20261119-v1`. Do **not** write results to the Manager's packet branch. A worker must check out a unique worker-owned **docs-result** branch and verify the one-writer file reservation before writing.
**Composed test worktree:** `/media/jer/ZX20/CCF-Sen1-820-E2-Native-20261119`. Candidate source branch `test/sen-1-820-compact30-e2-native-qualification-20261119-v1`, at the exact composed commit documented by #820. Verify branch/HEAD/status before Unity; **do not reset, stash, clean or overwrite any dirt**, and do not move #813/#815 source branches.
**Separate result writing lane:** propose `docs/sen-1-830-flatpak-unity-native-result-20261119-v1`, and verify an unclaimed non-Unity docs worktree under `/media/jer/Files/CCF/Worktrees/CCF-Sen1-830-FlatpakUnityResult`; do not assume path free.
**Exact final GitHub Markdown result:** `Docs/Handoffs/ScenarioOne-Player-Test-Readiness-20261119/Sen-1-830-FLATPAK-LICENSE-SMOKE-AND-820-NATIVE-TIER2-QUALIFICATION-RESULT-20261119.md`.

**Registration and gates:** This dispatch permits **read-only G0 preflight**. Before any original candidate, physical Unity or game-state write: actual model/provider/session, `jer-laptop` hostname, branch/current exact HEAD, source + test worktree cleanliness, live `active-tasks.md`, board/lanes, Editor PIDs, available workspace and owner conflict checks must be recorded on #830. If physical ownership, directory or single-writer status mismatches, **STOP** with `[BLOCKED]`. No issue title `RUNNING` or physical claim before actual registration.

### Phase G0: read-only admission, authorised now

- Read original #820 packet/result and #829 verified diagnosis, refresh live task/physical constraints, inspect whether Flatpak Hub sandbox instance still exists **without launching or changing it**. `flatpak ps` and benign metadata are acceptable.
- Inspect whether the existing deployed `unity-lock` can record the **owner D-091 no-arbitrary-expiry** exclusive hold; reconcile current live board and lane/task owner. A previously **released, one-off +6h legacy lease** does not authorise subsequent time-bound renewals. The #652 no-expiry version was previously reviewed in source but **NOT VERIFIED DEPLOYED**. No lock bypass, invented far-future `--eta`, forced release or live Ops patch/install as part of this task.
- Confirm the same corrected E1 + final E2 37-path source candidate and original scenes/reference hashes. **G0 is not a source rewrite**.
- If no lawful exclusive Unity lease can be obtained, provide an exact single HOLD finding and stop; do not start Unity or branch into broad Ops development. If the installed tool is no-expiry capable and rights actually present, a new explicitly authorised owner/physical lease may be acquired only under its own procedures and recorded separately; do not infer that authority from this docs handoff.
- Current READY G0 claim is **not** a blanket physical start permit. The Manager must be shown the fresh physical/lease admission evidence and grant actual Editor-start permission in issue #830 (or existing exact delegated approval where valid) **before** the Editor launch.

### Phase G1: Flatpak-context Editor licence smoke, HELD pending G0 permission

Only after exclusive lawful Unity custody and an actor-specific authorisation:

1. Check `flatpak ps` for the running instance of `com.unity.UnityHub` and the owner-signed-in licensing-client process; do not restart Hub/change activation.
2. Attempt a **single minimal Unity startup** using `flatpak enter` into that instance and the **original** Unity executable and #820 candidate project path (with safe log path under the worker's own evidence directory). Use the existing licensed UI context; command-line `-batchmode` is a test, **not assumed licensed**. Avoid `sudo`, exporting root authentication variables, printing session tokens or full Editor logs. No Windows build.
3. **Proof of licence PASS must be from actual log**, with sensitive token substrings excluded: existing client joined or successful equivalent, access token refresh successful, entitlement resolution, **no** `No valid Unity Editor license found`. Absence of a licensing error alone is insufficient.
4. If `flatpak enter` fails, report the error and attempt at most **one** GUI-from-existing-Hub fallback under the SAME lease/owner permissions, only if the fallback is covered by the admitted Editor-start scope. If batch-mode Personal entitlement fails but GUI works, report it as **GUI-only**, not headless PASS; carry gameplay via permitted GUI methods if qualified. **No licence/client/Hub reinstall, shell-in-sandbox token dumping, licence-file copying, loose XDG variables or hidden credential access.**
5. If licensing fails, **STOP**, clean only self-created disposable content and release the lock normally by owner identity, with one precise finding.

### Phase G2: exact real Unity Scenario One Tier 2, HELD pending G1 PASS

On the same composed candidate, with no source/code edits:

- Unity 6000.6.0f1 project import, compile/missing-script evidence (native, not just previously run assembly compile);
- corrected Compact30 scene render: playable 30×30m, original H1 clearing/C0 path, **no internal extraction racks**, 175 simulated Sitka instances, four off-path G1 sites, 226 outside cosmetic-only shells;
- real arrival and assessment gates; actual three player-selected Crop Trees; visual legibility and permitted/forbidden marks;
- real exact purchased **two Sessile Oak + two European Beech**, four independent pending juvenile orders at 4 G1 points, no early juveniles before actual Work Plan approval/execution; actual stock/cash and negative cases;
- Work Plan approve/cancel/immutable proposal semantics; annual advance effects, juvenile identity/finance/minutes and Annual Review/CT-3 chronology including two same-frame year advances and failure/cancel/partial negative controls;
- S1-C history plus later substantive adaptive intervention or deliberate no-work year if implemented in current source scope;
- historical Enlarged80 five-rack / Legacy40 / frozen Reference Future v1 compatibility: protected file bytes and real Editor behaviour, no involuntary saved-world format widening;
- run existing `Sen1CompactE2Reuse` verification harness only within its exact disposable harness admit/cleanup rules and only if real native Editor allows it;
- capture proof via timestamps/event traces/redacted logs/screenshots and explicit **PASS / FAIL / NOT RUN** by gate, rather than extrapolate from Tier1.

**Explicit exclusions:** #690 Compact save profile/CT-3 reload, #676 signed world identity, #796 M-A meaningful durable completion, new source implementation, #785 art creation, main merge, Windows build, owner personal E1 second walk, and all matters outside Scenario One. Preserve standalone historical rack scene/reference assets.

### Phase G3: handoff, mandatory cleanup and manager intake

Normal identity-matched Unity lock release; verify **no Editor PID or abandoned disposable harness**, worktree clean excluding pre-existing recorded dirt, return board/lanes untouched except genuinely authorised lane state, preserve actual source/test HEAD, no silent editor session left open. Publish **one** GitHub Markdown result at the exact path; inspect GitHub readback; post [HANDED-OFF] on #830 with complete final state. Manager #824 accepts/rejects only what evidence demonstrates and routes next gameplay/fix job if needed.

## 3. Paste-ready dispatch message

> **Sen-1 #830 — resume Scenario One corrected Compact30's existing Unity qualification using the Flatpak Hub licensing context.** Run on **jer-laptop** in native T3 Code/OpenCode. Full approved #829 verdict and exact #830 G0→G1→G2 handoff: https://github.com/jermur788/CCF/blob/docs/sen-1-flatpak-editor-resume-packet-20261119-v1/Docs/Handoffs/ScenarioOne-Player-Test-Readiness-20261119/Sen-1-824-829-MANAGER-APPROVAL-AND-830-FLATPAK-UNITY-NATIVE-QUALIFICATION-RESUME-PACKET-20261119.md . Register actual model/provider/session, branch/HEAD/source worktree/physical status on #830; **G0 read-only only at first**. Stop if D-091 exclusive no-expiry Unity ownership cannot be lawfully secured. Do not launch Unity until actual manager/physical Editor-start approval. Then, only under an authorised exclusive lease, test one Unity launch inside the existing Flatpak Hub sandbox and continue original #820 gameplay/old-world Tier2 if licensing passes. No licence files/tokens, Ops patches, source edits, merge or Windows. Publish ONE result GitHub Markdown to exact specified path and hand back to Scenario One Manager #824.

**User action:** paste the dispatch from a **native `jer-laptop`** session. If the existing Unity exclusive no-expiry mechanism is unavailable, further actor/owner authorisation will be needed for the physical gate, not another licence login. **Dashboard/Drive:** NOT VERIFIED.
