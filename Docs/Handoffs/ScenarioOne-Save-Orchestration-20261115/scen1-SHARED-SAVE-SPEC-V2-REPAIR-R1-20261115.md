# Scenario One shared save — APPROVAL CANDIDATE V2 bounded repair R1

**Task:** [#540](https://github.com/jermur788/CCF/issues/540), design sub-agent to #535. **Author:** OpenAI GPT-6.1 Sol, actual reasoning effort **high**, T3 Code/Codex. **Date:** 15 November 2026. No sub-agents used.

**State: APPROVAL CANDIDATE V2 — NOT APPROVED — NO IMPLEMENTATION AUTHORITY.** This is the author's bounded repair, not an independent re-review, owner signature, implementation, integration or runtime qualification. All 63 ISO items remain unchecked. Recommend independent review of the repaired candidate before the authorised owner/manager's package decision.

## Exact inputs and writable scope

| Input | Pin / scope |
| --- | --- |
| Complete task packet | #540, read first with `gh issue view 540`; default view hit GitHub's deprecated Projects-classic field, so the full body/comments were retrieved with `gh issue view 540 --json number,title,body,comments,url,state` |
| Repair base | `117f76179e828244a8d084f63ae10070ace1c8cf`, original #538 candidate |
| Authoritative independent review | #539, commit `ad15e1d68a3545bb05d1214873c5a659a8ab7aa6`, [full findings](https://github.com/jermur788/CCF/blob/ad15e1d68a3545bb05d1214873c5a659a8ab7aa6/Docs/Handoffs/ScenarioOne-Save-Orchestration-20261115/scen1-SHARED-SAVE-SPEC-V2-INDEPENDENT-REVIEW-20261115.md), read in full |
| Canon/source reference | Fetched `origin/main` at `fc18190b976f1363f1b5d1b0881810db2f8c6e39`; D-079/D-081 cross-checked read-only in canonical Decision Log. Context commit `62e39ec621e38a84030c08b35a171e297056c9ed`; AgentWorkflow, Decision Log, milestone, game brief, Unity overview and live coordination consulted |
| Branch | `docs/scen1-save-spec-v2-repair-r1-20261115-v1` |
| Worktree | `/media/jer/Files/CCF/Operations/CCF-540-SpecV2Repair`, created at required base; initial status clean, no unrelated changes |
| Exactly five written files | `specs/scenario-independent-save-isolation-v1/spec.md`, `plan.md`, `tasks.md`, `checklists/requirements.md`, and this report |
| Result pin | The #540 `[HANDED-OFF]` comment records the full commit SHA containing these five files and the published report URL; no self-referential SHA is embedded here |

## Finding-by-finding disposition

All five material, five minor and three nit findings are **FIXED as document repairs**. This does not satisfy any implementation ISO item. The original #538 handoff remains an unchanged historical snapshot; the repaired four-file contract and this finding table supersede its conflicting pre-repair dispositions. C-22–C-34 register the repairs individually by finding ID; C-02/C-08 explicitly correct earlier misleading admission labels instead of leaving contradictory register text.

| Finding | Disposition | Exact repair / limit |
| --- | --- | --- |
| M-1 | **FIXED** | Added prominent spec§0.4 “What changes for a historical player”, H-01–H-09, each with change, necessity, rejection consequence and previously-loadable risk. C-22 cross-references every disclosure. Covers the four named changes plus foreign-ID refusal, staged lifecycle, failed preview return, typed-route closure, truthful messages/pure capture. §0.3 explicitly asks the signer to accept disclosed changes rather than calling all behaviour unchanged. H-02 requires actual platform move/replace/backup/fault/interruption evidence before fallback removal is enabled on supported platforms. |
| M-2 | **FIXED** | §0.3/4.1/B-11/8.4, plan/tasks and ISO-44 distinguish numeric dispatch reservation `Compact30=2` from dimensions/cells/content. Proposed 30m/36-cell world, H1/path/G1 remain separately gated; signed world manifest supplies dimensions. Symbol/key spelling grants no dimension ratification. Legacy40=0/Enlarged80=1 untouched. |
| M-3 | **FIXED** | Q-01 lists proposed labels as enumeration only; no presentation order assigned. Ordering is neutral and subordinated to later owner/#471/D-082 decision, never first-demo/default promotion. No profile auto-starts. §0.3, W-01, plan/tasks and ISO-39/61 agree; H-04 discloses explicit choice when multiple profiles are available. |
| M-4 | **FIXED** | Default now follows accepted **D-079/D-081**: explicitly foreign scenario/profile identity rejects before any world, scene, PlayerPrefs or file mutation, on historical primary/backup and public routes too. W-03 defines supplied explicit identity and forbids inferring foreignness from genuine absent/null/empty fields, defaults or old versions. Genuine O1 admission remains preserved. W-03/SI-08/FR-07/ISO-42 label baseline canonical-envelope refusal UNVERIFIED and require narrow preflight repair if it fails; all prohibitions against that repair removed. Broader inference-based OPTION L remains excluded, but no longer prohibits canon-required rejection. §8.4 retains warn-and-continue only as a labelled, excluded departure requiring explicit owner re-decision of D-079/D-081, authorised canon reconciliation and revised signed candidate. |
| M-5 | **FIXED (explicit dependency block)** | §4.2/FR-11/B-08/8.4 and plan/tasks/ISO-58 name **CG-TRIG**, the not-yet-supplied compact world/teaching interaction contract routed through #535’s separate compact parent/child specification and owner gate. Whole ISO-58 is BLOCKED, not testable/passed by implication. Unblock with separately accepted exact arrival/assessment/designation/cancellation/replacement/review-read controls/events/preconditions, persist/reset rules and positive/negative/midway-reload cases, linked in a revision. Undefined triggers are excluded from package signature; no implementer-invented UI or product rule. Evidence representation retained. |
| m-1 | **FIXED** | C-02 and §0.3 distinguish owner-fixed anchors/visibility from B-03/04/05 admission preserve dispositions, subject to the explicit-foreign canon correction. No extra owner approval claimed. |
| m-2 | **FIXED** | ISO-20 lists mandatory process-termination checkpoints: before temp creation, during partial temp write, after flush, before/during/after first move or replacement, after commit before cleanup/status. First/replacement cases separate; complete old/new or validated recoverable old bytes required, with valid no-primary outcome before first-save commit. Missing mandatory evidence blocks; additional power-loss/kernel/filesystem cases are limits, not passes. FR-05/plan/tasks reference the gate. |
| m-3 | **FIXED** | Dropped undefined visible/audio members from FR-04/ISO-12 sentinel; retained enumerated world/scene/UI/pose/prefs/files set. No new audio architecture or component inventory invented. |
| m-4 | **FIXED** | SI-10/ISO-63 define freeze trigger as first integration to main of a writer for that profile, independent of build/release/personal playtest. Materiality definition otherwise retained. |
| m-5 | **FIXED** | ISO-10 names applicable FR-04 restored-world comparison members; ISO-14 cross-references that set. Genuine O1 admission/restoration remains baseline-preserving; explicit foreign negatives are isolated canon cases. |
| n-1 | **FIXED** | Filename literal/path citations corrected to ForestSaveController.cs:9/:15; backup-message citation corrected to :286–287 in §2/B-09 (and B-01 path citation clarified). Source offsets come from independent review; a read-only source spot-check of manager:1464–1486 also confirmed the existing nonempty-ID late-warning path for M-4. No save bytes inspected. |
| n-2 | **FIXED** | W-02 and related confirmation wording distinguish F9 load, startup Continue load, F10 session-menu toggle, in-session Continue closing the menu, and menu Save replacement confirmation. Immediate F5/no modal remains intact. This is a description correction, not new input behaviour. |
| n-3 | **FIXED** | T-02 now says “static/offline portions of” the mixed-type ISO items. Full pass still requires every named test type. |

## What a signer must know

Historical-player preservation is limited to stated anchors and genuine admission semantics. It is not blanket runtime equivalence. In particular, the backup action and multiple-profile choice are new interactions; atomic-only writing and pre-write checks can newly refuse Save. An explicit foreign nested ID can be **previously loadable** under today's warning/restore path yet must now refuse before mutation under accepted canon. This is disclosed in H-05 rather than hidden inside “unchanged”. Files admitted by old code are not thereby proven genuine O1.

The package proposes safe failure/recovery/staging/capture changes; their platform and runtime safety remains unverified. Genuine historical fixture completeness remains UNPROVEN, O2 unavailable without authored binding, compact dimensions/H1/path/G1 separate, CG-TRIG/ISO-58 BLOCKED, full compact completion separately gated, D-082/default promotion/order unresolved, and permanent historical future-version policy deferred. No ordinary package signature selects warn-and-continue or waives those holds. A signer who rejects a disclosed proposal needs a revised candidate; a canon departure needs explicit owner re-decision.

## Passed areas preserved and refused scope

No finding was declined or partially fixed. No restructuring was needed. No passed requirement was relaxed: B-09/B-10/B-11 remain CONTRADICTED existing guarantees with future gap-closure requirements; genuine v1–v24/absent-ID admission, model/normalization semantics, existing historical geometry2 refusal and historical serialization remain protected. M-4 changes only the explicitly foreign boundary required by #540/canon, not inferred genuine-history admission.

The frozen Reference SHA-256 text `789e3eddcf00d3d4d0fd18d078f7ab37a06d44aa9c8672cff564445b2bb041fc`, original archive/hash-helper rules, `CCF.Learning.v1.*`, `CCF.MenuHelp.v1.*`, Legacy40=0/Enlarged80=1 and D-056/D-064/D-065/D-071 five-rack anchors remain intact. Anchor bytes were not read or rehashed. The 63 ISO IDs/order/test-type labels remain; all are unchecked. The §0.3 signature fields remain blank.

Kept outside this repair: implementation, source changes, Unity/Player/compiler/test-harness execution, save-file access, invented teaching triggers/dimensions/default promotion, canon/board/dashboard/overview edits and broader OPTION L. No writes/accesses to the three forbidden locations or user saves. Setup used only the authorised bare repository/worktree; no shell Unity work or protected worktree mutation.

## Verification actually performed and remaining gates

Document-only verification: `git diff --check`; exact five-path allowlist; baseline-versus-repair ISO sequence/count/unchecked/test-type comparison; changed-ISO allowlist; unchanged protected ISO rows and spec preservation-anchor text; blank signature/status checks; C-01–C-34 register sequence and all 13 finding tags; targeted cross-file contradiction scan and manual diff review. All passed before publication. These are consistency checks, not runtime tests or independent acceptance.

No compiler, Unity, Player, JsonUtility experiment, File.Replace/fault injection, save/load, fixture hash read or Reference byte read was performed. The exact-head compiler/offline/Unity Tier-2 gates in the plan/checklist remain future, separately authorised work. No ISO checkbox is checked and no gate is waived by this document repair.

Next: parent obtains independent review of this exact repaired commit, then authorised owner/manager decides the candidate package. Any later implementation still requires real signature, new owned source claim, accepted dependencies and required runtime gates. **Dashboard ingestion/audit/published readback and Drive mirror synchronization NOT VERIFIED**; parent/authorised custodian must handle them within its own writer permissions. #540 closure records docs handoff only, not approval, integration or synchronized project status.
