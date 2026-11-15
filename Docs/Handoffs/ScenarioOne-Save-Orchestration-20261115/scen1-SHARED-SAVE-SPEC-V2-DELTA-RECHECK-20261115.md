# Scenario One shared save — Spec V2 repair R1: focused delta re-check

**Claim:** [#541](https://github.com/jermur788/CCF/issues/541) (V2-E), parent [#535](https://github.com/jermur788/CCF/issues/535). **Date:** 15 November 2026.

## 1. Reviewer identity and independence

- **Reviewer:** Anthropic Claude Opus 5.5 (`claude-opus-5-5`), Claude Code harness inside T3 Code. #541 asked for medium effort. The reviewer cannot observe the effort setting directly. No sub-agents were used.
- **Independence:** this is the same nonauthor reviewer that wrote the #539 review (`ad15e1d6`). The candidate and the R1 repair were written by OpenAI GPT-6.1 Sol under #538 and #540. The reviewer did not write, edit or contribute to any candidate file. **No candidate file was changed.** This report is the only file written.
- **Scope:** a delta check only. It covers the 13 findings from #539 and regressions in the areas the repair touched. It does not re-open areas #539 passed.

## 2. Exact target

| Item | Value |
| --- | --- |
| Reviewed candidate | **`200add4e6e5116268b79ef63131ff9b836f14c5d`** (branch `docs/scen1-save-spec-v2-repair-r1-20261115-v1`) |
| Repair base | `117f76179e828244a8d084f63ae10070ace1c8cf` |
| Prior review | `ad15e1d68a3545bb05d1214873c5a659a8ab7aa6`, `scen1-SHARED-SAVE-SPEC-V2-INDEPENDENT-REVIEW-20261115.md` |
| Delta | `git diff 117f7617 200add4e` touches exactly five files: `spec.md` (291 lines), `plan.md`, `tasks.md`, `checklists/requirements.md` (116) and the R1 repair report. Nothing changed under `Assets/`, `Packages/`, `ProjectSettings/` or `Docs/Project/` |
| Canon check | `origin/main` `Docs/Project/decision-log.md`, read-only. D-081 says "reject wrong-scenario loads **before any mutation**" and D-079 says "pre-mutation rejection of saves belonging to other scenarios". Both are confirmed verbatim |
| Source spot-check | `fc18190b:Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.cs:1481`. The current warning predicate is `!string.IsNullOrEmpty(data.scenarioId) && data.scenarioId != definition.ScenarioId` |

All `file:line` references below are at `200add4e`. Paths are relative to `specs/scenario-independent-save-isolation-v1/` unless stated otherwise.

## 3. Verdict

# DELTA RESOLVED — FIT TO SIGN

All 13 findings are **RESOLVED**: 0 partially resolved and 0 not resolved. **Unresolved count: 0.**

The repair introduced one new defect, a **NIT** (D-1 below). It does not block a signature.

This verdict covers only whether the document is fit for an owner signature. It is **not** Unity verification, runtime evidence, integration acceptance or approval of the spec.

## 4. Finding-by-finding verdict

| ID | Sev. | Verdict | New file:line that resolves it | What was checked in the text |
| --- | --- | --- | --- | --- |
| M-1 | MATERIAL | **RESOLVED** | `spec.md:77–91` (§0.4 H-01–H-09); `spec.md:63` (§0.3 item 1); `spec.md:35` (C-22); `spec.md:273` (Q-02); `plan.md:9` | All four undisclosed changes I raised are now disclosed. (a) Backup offer: H-01 `:83`. (b) Copy fallback removed: H-02 `:84`, with platform evidence required before removal is enabled. (c) Pre-IO capture validation: H-03 `:85`. (d) Explicit choice step: H-04 `:86`. H-05 to H-09 add foreign rejection, lifecycle, preview, public routes and status/capture. §0.3 now says these are "disclosed historical runtime changes proposed for signature, not an assertion that all behaviour is unchanged". See §5.1 for the overclaim check. |
| M-2 | MATERIAL | **RESOLVED** | `spec.md:148`, `:64`, `:68`, `:262` (B-11), `:282` (§8.4); `checklists/requirements.md:75` (ISO-44), `:110` (ISO-61); `tasks.md:46`; `plan.md:9`, `:15` | Compact30=2 is now a "numeric dispatch reservation only". The 30 m / 36-cell size is listed in §0.3 as outside the signature (`:68`: "approval of 30m/36-cell dimensions" is not authorised). Dimensions come from a separately signed world manifest, and ISO-44 keeps runtime dimensions BLOCKED until that manifest exists. |
| M-3 | MATERIAL | **RESOLVED** | `spec.md:272` (Q-01), `:64`, `:68`, `:127` (W-01); `checklists/requirements.md:67` (ISO-39), `:110` (ISO-61) | Q-01 is "enumeration only, no presentation order assigned". Order is deferred to the owner/#471/D-082. §0.3 excludes "compact first-demo/default prominence or presentation order". No profile auto-starts. |
| M-4 | MATERIAL | **RESOLVED** | `spec.md:131` (W-03), `:180` (SI-08), `:177` (SI-05), `:190` (FR-02), `:198` (FR-06), `:208–210` (FR-07), `:254` (B-03), `:284` (§8.4), `:240` (stop rule); `checklists/requirements.md:73` (ISO-42) | See §5.2. Pre-mutation rejection of explicit foreign identity is now the default and cites D-079/D-081. Genuine O1 admission is untouched. Warn-and-continue is a labelled "ALTERNATIVE — not default" that needs an explicit owner re-decision of canon. The rule that forbade a fix is gone, and ISO-42 now says "no clause forbids this repair". |
| M-5 | MATERIAL | **RESOLVED** | `spec.md:163` (CG-TRIG hold), `:167`, `:220` (FR-11), `:259` (B-08), `:285` (§8.4), `:64`; `checklists/requirements.md:104` (ISO-58); `plan.md:15`, `:39`; `tasks.md:48–49`, `:86` | See §5.3. The whole of ISO-58 is BLOCKED on a named dependency, CG-TRIG, with stated unblock conditions. Implementers may not invent the triggers. |
| m-1 | MINOR | **RESOLVED** | `spec.md:14` (C-02), `:63`, `:40` (C-27) | C-02 now says "OWNER-APPROVED BOUNDARIES; ADMISSION PRESERVE DISPOSITION". §0.3 item 1 says admission preservation is "the B-03/04/05 disposition, not a further owner-fixed rule". |
| m-2 | MINOR | **RESOLVED** | `checklists/requirements.md:39` (ISO-20); `spec.md:196` (FR-05) | ISO-20 lists the mandatory process-termination checkpoints. It tests first saves and replacement saves separately, says "All checkpoints must pass", and records power-loss, kernel and filesystem cases as limits, "never credited passes". |
| m-3 | MINOR | **RESOLVED** | `spec.md:194` (FR-04); `checklists/requirements.md:25` (ISO-12) | The "visible/audio" members are removed. The remaining sentinel members are all enumerated. |
| m-4 | MINOR | **RESOLVED** | `spec.md:182` (SI-10); `checklists/requirements.md:112` (ISO-63) | The trigger is now "first integration to main of a writer for that profile", independent of release, build or personal playtest. |
| m-5 | MINOR | **RESOLVED** | `checklists/requirements.md:23` (ISO-10), `:30` (ISO-14) | ISO-10 lists the FR-04 restored-world comparison members, and ISO-14 refers back to that set. |
| n-1 | NIT | **RESOLVED** | `spec.md:109`, `:111`, `:252` (B-01), `:260` (B-09) | `ForestSaveController.cs:9/:15` and `:286–287` are corrected. |
| n-2 | NIT | **RESOLVED** | `spec.md:129` (W-02), `:274` (Q-03), `:15` (C-03); `checklists/requirements.md:13` (ISO-03) | The text now separates F9/startup Continue load, the F10 menu toggle, in-session Continue (which only closes the menu) and the menu Save confirmation. |
| n-3 | NIT | **RESOLVED** | `tasks.md:37` | T-02 now says "static/offline portions of ISO-06–12/…". |

## 5. Specific checks requested by #541

### 5.1 M-1: does §0.4 disclose every behavioural change honestly?

Yes. Each of the nine rows has the same five columns: change, why it is needed, what happens if the signer rejects it, and "Can a previously loadable save fail?". No row claims behaviour is "unchanged" where it is not:

- **H-05 (`spec.md:87`)** answers **"Yes"**. A legacy-shaped file with an explicit foreign nested ID can load today but will now refuse. It also states that old admission does not prove genuine O1 provenance.
- **H-08 (`:90`)** answers **"Yes for a bypass invocation"**, while the genuine disk file stays valid.
- **H-02 and H-03 (`:84–85`)** answer "No direct load refusal" and disclose that Save can now fail.
- **H-06 (`:88`)** discloses that a changed file, or a failed apply, now refuses atomically.
- The remaining "No" answers (H-01, H-04, H-07, H-09) are each limited to no new genuine-file admission rule. They disclose the interaction change in the "What changes" column.

The preamble at `:79` limits what is protected to filename, genuine admission and semantics, Continue **visibility**, preference namespaces and Reference anchors. It says outright that the package "does not preserve every interaction, failure outcome or startup sequence". Q-02 at `:273` no longer presents Continue interaction as unchanged.

The source spot-check matters here. W-03's definition of explicit foreignness ("a nonempty supplied scenario ID differing from the selected chapter") is exactly the predicate that triggers today's warning at `ScenarioOneManager.cs:1481` (`IsNullOrEmpty` plus ordinal `!=`). So the set of saves that will now refuse is exactly the set that warns today. H-05 does not understate it.

### 5.2 M-4: is canon now the default?

- **Default:** W-03 `:131`, FR-02 `:190`, the FR-07 explicit-foreign row `:208` and B-03 `:254` ("CANON DEFAULT") all require rejection before any world, scene, PlayerPrefs or file mutation. They name D-079 and D-081 and cover the historical primary, the backup (FR-06 `:198`) and public routes (H-08). This matches the canon text on `origin/main` quoted in §2.
- **Genuine O1 admission untouched:** W-04 `:133` is unchanged. The FR-07 absent/null/empty row `:209` and SI-05 `:177` forbid inferring foreignness from absent fields, defaults or version. B-04 and B-05 `:255–256` are unchanged.
- **Warn-and-continue demoted:** §8.4 `:284` marks it "**ALTERNATIVE — not default**". It applies only if the owner explicitly re-decides D-079/D-081, and it needs canon reconciliation and a revised signed candidate. §0.3 `:65` and `:68` exclude it from the ordinary signature.
- **The fix-prohibition is removed:** the old `117f7617:spec.md:177` ("verify on candidate. No new marker-based rejection default") is replaced by `:210`, which says a failed ISO-42 *requires* a narrow repair. SI-08 `:180` says the repair "is required and permitted". ISO-42 `:73` says "no clause forbids this repair". §8.4 `:284` says the OPTION L exclusion "does not cover or forbid" it. The stop rules at `spec.md:240`, `plan.md:86` and `tasks.md:39` say the canon correction "is required, not prohibited drift". I found no remaining clause that forbids the fix.
- **Baseline premise:** it is now marked UNVERIFIED in W-03, SI-08, FR-07, B-03, ISO-07 and ISO-42.

### 5.3 M-5: is ISO-58 honestly BLOCKED?

Yes. `checklists/requirements.md:104` is headed "**BLOCKED on CG-TRIG**". It states: "Whole item stays unchecked/BLOCKED, not a partial persistence pass". The unblocking dependency is named and routed at `spec.md:163`: CG-TRIG, a compact world/teaching interaction contract routed through #535's separate compact parent/child specification and owner gate. The unblock conditions are explicit: exact controls/events/preconditions, persist/reset rules, and positive/negative/midway-reload cases for every trigger, owner-accepted and linked by revision.

§0.3 `:64` accepts the hold but not the triggers. The review-read signal is delegated to CG-TRIG, and implementers may not choose a substitute (`:167`). CG-TRIG does not exist as a document yet, and the spec says so ("not-yet-supplied"). That is the honest state.

## 6. Regression check (the five repair areas only)

| Area | Result | Evidence at 200add4e |
| --- | --- | --- |
| Three CONTRADICTED-gap conversions | **Intact** | B-09 `spec.md:111`, `:260` ("GAP MUST CLOSE"). B-10 `:117`, `:261`. B-11 `:118`, `:262`. No claim of existing safety was added. |
| Genuine historical admission | **Intact** | W-04 `:133`; FR-07 `:204–209`; SI-05 `:177`; plain historical geometry2 still rejected (`:123`, ISO-49 `checklists/requirements.md:83`). M-4 narrows only the explicit-foreign case, and §5.1 shows that case equals today's warning predicate. |
| Preservation anchors | **Intact** | `spec.md:238`: SHA-256 `789e3eddcf00d3d4d0fd18d078f7ab37a06d44aa9c8672cff564445b2bb041fc`, `CCF.Learning.v1.*`, `CCF.MenuHelp.v1.*`, **Legacy40=0 / Enlarged80=1**, D-056/D-064/D-065/D-071 five-rack. Also `:147`, `:181` (SI-09), ISO-15 `:31`, ISO-16 `:32`. |
| 63 ISO IDs | **Intact** | A script check found 63 items at both `117f7617` and `200add4e`, IDs ISO-01–63 in order, 0 checked, and no test-type label changed. Acceptance status line `:116` unchanged. |
| §0.3 signature | **Intact** | All fields blank (`spec.md:70–73`). Status is still APPROVAL CANDIDATE V2 / NOT APPROVED. |
| Cross-finding conflicts | **None** | H-01 backup offer and H-05 foreign rejection agree (FR-06 `:198`, ISO-22 `:41`, ISO-55 `:98`). Q-01 enumeration agrees with H-04 and ISO-39. M-2 dispatch-only agrees with ISO-44, ISO-61, B-11 and §8.4. Plan and tasks agree with the spec on every repaired point (`plan.md:9`, `:12`, `:15`, `:38–39`; `tasks.md:11`, `:18`, `:35`, `:46`, `:48–49`). |

### New defect

| ID | Severity | file:line | Defect | Suggested fix |
| --- | --- | --- | --- | --- |
| D-1 | NIT | `spec.md:34` | The repair put a blank line between change-register row C-21 (`:33`) and C-22 (`:35`). In GitHub-flavoured Markdown this ends the table, so rows C-22–C-34 (`:35–47`), which register all 13 repairs, render as one run-on paragraph of pipe characters. The rows are still there and readable in raw text, so this is not material. | Delete the blank line at `:34`. |

Observation, not a finding: FR-05 `:196` says each supported platform needs H-02 qualification "before fallback removal is enabled". Read with the absolute "Do not use File.Copy-over-primary fallback" (same line), ISO-20 and ISO-56, this is a release-qualification gate on the new writer. It does not permit keeping the fallback in a qualified build. It follows the #539 M-1 recommendation.

## 7. What this re-check did not do

- It did **not** edit any candidate file, `specs/`, `Docs/Project/`, `Assets/`, `Packages/`, `ProjectSettings/` or `CCF-Ops/`. It did not fix D-1. It checked no ISO item and changed no status, decision log, milestone, board, dashboard or Drive mirror.
- It did **not** run Unity, the Editor, a Player, a compiler, a test harness, a JsonUtility experiment or fault injection. It did not access any `forest-save*` file, PlayerPrefs, Reference bytes or `/media/jer/ZX20/CCF-Ops/state/unity/`. It did not touch `/media/jer/Files/CCF-Primary` or `/media/jer/Files/CCF/Orchestration/CCF-Scen1-SharedSave-20261115`.
- It did **not** recompute any SHA-256. Anchors were checked as text only.
- It did **not** re-review areas that #539 passed, beyond the regression rows above. It did not re-audit G0. It did not accept the R1 repair report's claims without checking the document text.
- It does **not** approve the spec, verify runtime behaviour, accept integration or authorise implementation. Only a real §0.3 owner/authorised signature can accept the package.
