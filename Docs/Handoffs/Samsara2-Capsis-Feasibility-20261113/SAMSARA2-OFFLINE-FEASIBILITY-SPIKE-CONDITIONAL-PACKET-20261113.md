# CONDITIONAL T3 Code packet — Samsara2 offline feasibility spike (not released)

**Status:** WAITING / NOT RUNNING / REQUIRES MANAGER PRIORITY + OWNERSHIP RELEASE  
**Presence claim:** [#456](https://github.com/jermur788/CCF/issues/456) — WAITING, no worker registered  
**Research basis:** [Samsara2 CAPSIS research handoff](SAMSARA2-CAPSIS-RESEARCH-FEASIBILITY-HANDOFF-20261113.md)  
**Purpose:** establish whether a legally obtained, version-pinned Samsara2/CAPSIS setup can run and export reproducible independent forest-management experiments suitable as an **offline reference**, not replace CCF's Unity engine.  
**Difficulty:** Medium — external Java/R/CAPSIS version, module legal status, opaque script formats and reproducibility/performance checks.  
**Best-suited execution:** GPT-6.1 Sol via T3 Code Linux local lane; use an equally capable local Code agent if unavailable. Browser-only procurement/licence checking may precede the local phase; no Unity editor is required.  
**Code lane/branch (reserved names, not verified/created):** `research/samsara2-offline-trial-20261113-v1`; proposed isolated local worktree `/media/jer/ZX20/CCF-Samsara2-OfflineTrial-20261113`. Create only after Manager authorises, checking actual existence/storage/locks. No agent has this worktree today.  
**Source baseline:** latest safe, explicitly re-pinned `main` at actual task start; capture its full commit internally. Never assume current source position from research date.  
**Single writer / collisions:** read live `Docs/Project/coordination/active-tasks.md` before start; Manager #424 currently owns that file. The actual worker must coordinate without writing live coordination unless assigned. Never overlap Objective 1/Objective 2 branches/files or launch Unity.  
**User action:** none until Manager release; to deliberately dispatch, paste the line at the end into the selected T3 Code chat.  

## Gate 0 — registration and safety (mandatory)
- Check claim #456 and live ownership; if no explicit Manager release/priority and lane agreement, remain WAITING and do not install, create local worktree, or edit.
- If released, verify candidate branch, HEAD, local worktree path, unrelated dirty work, disk availability; register **actual** provider/model, branch, worktree, starting HEAD and [RUNNING] in #456 before writing.
- No modifications to Unity, `Assets/**`, packages/project settings, production CCF simulation, existing approved specs, saves, PlayerPrefs, user gameplay test candidate, frozen Reference Future, existing test fixtures, active-tasks, or other tasks. No Windows build or current Editor launch. Use dedicated isolated system/software install paths.
- Do not demand or occupy the machine-wide Unity lease for a research process; if high CPU/RAM use would interfere with Objective 1 qualification, defer/stop.

## Gate 1 — legal/access reproducibility
- Inspect the exact installer and Samsara2 **module-specific** licence. `martelosim` MIT and CAPSIS core LGPL are NOT sufficient to prove module rights. If absent, contact/record requirement for permission; do not bypass, decompile, or redistribute model source/binaries.
- Note Java 8/installer compatibility from official doc; obtain software only from authorized sources. Pin CAPSIS, Samsara2, R, martelosim versions and their source URLs/hashes. No global toolchain changes to CCF.
- Check whether Samsara2 supports the documented command file and general CAPSIS Java/Groovy script modes in the exact installed version. Record any missing access/documentation as a blocker.
- If installer/licence unavailable, stop with a short documented **BLOCKED** result; do not simulate equivalent results from guesses.

## Gate 2 — native example run
- Use supplied official **Prelenfrey** marteloscope example, not an invented CCF stand and not user saved worlds.
- Build inventory and periphery/saplings as published; track generated vs measured values. Run a baseline marking scenario through R/CAPSIS and capture source command, logs, output/report, elapsed runtime, peak memory if actually observed, and restartability.
- Repeat identical input and controlled seed if seed API exists; if it does not, quantify stochastic variation from repeat runs and note that seed control is unavailable rather than asserting determinism.
- No Unity runtime testing is necessary or authorised.

## Gate 3 — controlled treatment comparison
- Create and archive *research-only* no-cut, local competitor-release and broad-thinning variants using the same input plot. An additional multi-intervention variant is optional **only if supported by the exact script interface**, not by assumed functionality.
- Compare observed output direction and magnitudes for stocking, basal area, diameter classes, crown light/ground light and seedlings/recruitment when actually exposed. Keep model's Alpine species identity explicit; do NOT relabel Norway spruce/silver fir as Irish Sitka spruce or assign Irish probabilities.
- Record mapping completeness, missing state, spatial edge handling, generated peripheral trees and generated saplings; provide a simple machine-readable CSV/JSON summary only if underlying outputs support it.
- Check runtime and memory for one actual small plot. No unsupported extrapolation to CCF's full stand.

## Gate 4 — result and closure
- Publish a durable GitHub Markdown result at `Docs/Handoffs/Samsara2-Capsis-Feasibility-20261113/SAMSARA2-OFFLINE-SPIKE-RESULT-20261113.md` on the isolated research branch; large logs/data remain off-repo with manifest and preserved access path, no copyrighted binaries.
- Include actual provider/worker, exact versions, legal finding, every command, example run/outputs, repeated-run reproducibility, observable resource usage, research controls, missing capabilities, and **GO/NO-GO for offline benchmark only**.
- If licence/installer unavailable, publish **BLOCKED — NOT EXECUTED** accurately, close claim only with durable explanation; do not invent tests.
- At handoff set claim [HANDED-OFF], link result and close; Manager reviews with **APPROVED / NEEDS CHANGES / REJECTED** and decides whether any later spec is justified. No inference of production implementation, independent scientific validation, or user acceptance.
- Dashboard sync: report VERIFIED or NOT VERIFIED with evidence, never assume it.

## Binding boundaries
- **No** production code integration, Java-to-C# port, parameter import or gameplay calibration in this task.
- **No** change to D-059 three-species depth, D-061 ordinal mixed density, D-065 staged judgement/Work Plan approval, Irish ecology, S1-C/S1-D, Rack V2, save compatibility or frozen Reference Future.
- **No** script pretending to model Irish Sitka on Alpine spruce coefficients. The value sought is architecture, trend comparison and reproducibility.
- Separate science from gameplay: label results as observed executable output, transfer evidence, comparison hypothesis, or unknown.

## Paste-ready dispatch (only after Manager releases #456)

> Take ownership of CCF #456 after confirming it is released. Follow the complete conditional T3 Code packet at https://github.com/jermur788/CCF/blob/docs/samsara2-capsis-feasibility-20261113-v1/Docs/Handoffs/Samsara2-Capsis-Feasibility-20261113/SAMSARA2-OFFLINE-FEASIBILITY-SPIKE-CONDITIONAL-PACKET-20261113.md . Register your actual worker and safe isolated lane before editing. Establish licences and run an independently reproducible official Samsara2/CAPSIS example, then a bounded thinning comparison if supported. Research only; no Unity, CCF gameplay, saves, active coordination edits or Windows build. Publish the specified GitHub handoff, or a truthful BLOCKED result.
