# Independent Scenario Save Isolation V1 — Implementation Plan

**Status: DRAFT — NOT APPROVED — NO IMPLEMENTATION AUTHORITY.**
Derived from spec.md; does not permit work. Task #436 was documents-only, and **the #524 three-profile amendment is documents-only** under #425's express delegation. **Overall Development Manager #425** is the accountable shared save-policy/spec owner and must review before any later implementation claim. **Scenario One Delivery Manager #424** controls later source integration, live coordination, the single source writer and Unity scheduling.

## 1. Design principle and prerequisites

**Minimal additive route (amended #524):** preserve Scenario One's current Application.persistentDataPath/forest-save.json name, its .tmp/.bak alongside it, and its exact serialized layout. Introduce two fixed, separately named new-profile slots: compact O1 and O2 (both PROPOSED, spec table 4.1). Route by the explicitly selected **profile**, not by scene name, loaded tree IDs, title string, dimensions or old file migration. **One shared router and one shared preflight** serve all three profiles. Share the existing validated save pipeline where possible. Do not extract a generic eight-scenario save framework absent a demonstrated need, and do not build a second compact-only save controller (spec FR-13).

The first commit of this plan is NOT code. Prerequisites for code:
1. #425 reviews spec + checklist after one **independent nonauthor persistence/doc review**, resolves or explicitly accepts the section 8.2 blockers (B-01–B-15) with owner/save sign-off where required, and separately marks the spec APPROVED. D-082 (#471) must be accepted before the compact profile is presented as the first demo.
2. Inspect then-current main/current integration candidate and #424 live sole-writer coordination. Register a new task claim and an exclusive source writer for the save controller, save model, scenario restore, learning/help prefs and relevant UI paths; the new branch/worktree and permitted file list must be explicit. Do not edit while #424 or another source owner holds any overlapping file.
3. Establish exact existing O1 save/Reference v1 fixtures and their protected byte/hash anchors, with genuine-vs-synthetic provenance (B-06), plus the O2 and compact authored-world/definition dependencies. The O2 and compact scenes/models are separately approved features and cannot be invented in this save packet.
4. Unity machine-wide lease, Editor process and exact candidate ownership must be checked immediately before any Unity activity. No cross-device "free" assumption from GitHub alone; no Windows/Player build without the separate personal O1 playtest and the user's explicit permission.

## 2. Shared routing and ownership map (new #524) — PROPOSED

| Concern | One owner seam (proposed) | Historical O1 | Compact O1 | O2 |
| --- | --- | --- | --- | --- |
| Profile → slot | fixed compile-time table in the save controller (or one small router helper if review prefers) | forest-save.json (+ .tmp/.bak) | forest-save-scenario-one-compact-v1.json (+ .tmp/.bak) [PROPOSED] | forest-save-scenario-two.json (+ .tmp/.bak) [PROPOSED] |
| Raw identity | shared preflight (spec FR-02) | reject on envelope marker or foreign nested scenarioId (B-03); no new field written | envelope required, exact values | envelope required, exact values |
| Version policy | shared preflight, per profile (FR-07) | existing v1–v24 rules; newer → warn + best-effort preserved | strict; newer/missing → reject | strict |
| Write | existing WriteSaveAtomically, parameterised by slot | byte-identical `JsonUtility.ToJson(data, true)` | envelope + payload | envelope + payload |
| Learning/help prefs | profile-supplied prefix to LearningObjectivesView / MenuHelpView | `CCF.Learning.v1.` / `CCF.MenuHelp.v1.` unchanged | `…scen1-compact-v1.` [PROPOSED] | `…scenario-two-v1.` [PROPOSED] |
| Teaching evidence | inside each save | v24 latch unchanged | compact evidence block (FR-11, B-08) | O2 spec later |
| Reference preview | ScenarioOneManager preview, gated by profile first | unchanged | not offered (B-07) | not offered |
| UI | StandaloneSessionMenu (+ .Startup) queries the router; no path literals | unchanged label | "Compact Scenario One" (Q-01) | "Scenario Two" |

Policy owner: #425. Delivery/integration: #424. One future source writer at a time.

## 3. Stage sequence (future, conditional)

| Phase | Purpose and changes | Scope/verification | Go/no-go evidence |
| --- | --- | --- | --- |
| P0 — static map | Audit every current capture, disk Read/Write, F5/F9/F10, startup Continue/New, direct LoadData, Reference Preview, PlayerPrefs reader/writer/reset, old version and test seam. Confirm O2 and compact identity/root/scene. Resolve or carry B-01–B-15 explicitly. No code. | GitHub/code inspection, real fixtures, version matrix, fixture provenance manifest. | Signed preflight/owner map and resolved identity decisions. |
| P1 — isolated selection + file routing | Small fixed **three-profile** identifier-to-filename mapping and selected-session source of truth. Preserve the O1 filename and bytes exactly. Separate temp/backup files; no implicit cross-slot fallback in any direction. Remove the duplicate UI path literal in favour of the router. | Tier 1: pure path/selection tests (including case-insensitive collisions), real legacy path assertions, dry-run corruption tests. | O1 slot unchanged; compact and O2 slots disjoint; selected-context route inventory reconciled. |
| P2 — identity/schema preflight | Envelope for new profiles only; historical-slot foreign-marker rejection (per B-03 ruling); version-aware legacy O1 classification (per B-04 ruling); strict raw JSON presence/mismatch checks; **profile-specific version policy** (strict compact/O2; historical best-effort preserved). Shared CanLoad/Load/LoadData preflight BEFORE scene, world, prefs or file mutation. | Tier 1: table-driven negative cases for all six cross-placements, the static load-entrypoint matrix and an untouched-state sentinel. | All cross-directions rejected identically by CanLoad and actual Load; old O1 readable; historical newer-version behaviour unchanged. |
| P3 — session/scene lifecycle | Wire explicit new/continue selection to the appropriate authored starting scene and definition for each profile. Do not replace the current world until the target is verified. A refused load leaves startup/selection and the active forest unmodified. Preserve the Reference Preview trusted restore and gate it by profile (FR-12). Route learning/help prefixes by profile. | Focused Tier 2 Editor with tracked active scene, three-world differences, preview return, prefs snapshot. | First compact and O2 save/reload and O1 resume work; wrong-profile refusal leaves world, prefs and UI state intact. |
| P4 — backup and failure qualification | Validate per-slot .tmp/.bak handling, File.Replace and fallback behaviour, missing/corrupt/foreign-backup UX, save failures, read-only old bytes, old-version fixtures, same-ID cross-world history, all-three-slot byte manifests. | Tier 1 offline malformed fixtures, then Tier 2 bounded Editor/IO failure injection on the exact candidate. | No data loss, false success, wrong recovery or partial restore in any profile. |
| P5 — bounded conformance/integration handoff | Inspect the diff against approved paths, trace requirements (ISO-01–ISO-63), run an independent review proportionate to sensitive persistence, then an isolated integration decision. Re-run only impacted gates; do not duplicate the whole Tier 3 per packet. | Feature Tier 2; downstream Tier 3 only when a release/milestone is separately nominated. | Reviewer APPROVED/NEEDS CHANGES/REJECTED; Manager explicit integration/verification decision. |

Compact world construction (H1/path/G1 generator, geometry dispatch) is **not** in these phases. It is the compact feature's own approved packet, which consumes P1–P3.

## 4. Source ownership and dependency map (tentative, not writable now)

| Path / subsystem | Why likely touched | Ownership restriction |
| --- | --- | --- |
| Assets/ForestPrototype/ForestSaveController.cs | Existing single-slot, F5/F9, CaptureData, CanLoad, TryReadSave, FindLoadProblem, LoadData and atomic writer; becomes the one router/preflight owner. | High-risk shared source. One assigned production writer at a time; #424 authorises/relinquishes file ownership before any editing. |
| Assets/ForestPrototype/ForestSaveData.cs | Current v24 layout; historical layout must stay unchanged. New-profile envelope type, if adopted (B-02), is additive and separate. | No frozen archive bytes, hash anchor or silent version policy changes. Coordinate with all S1 save/persistence work. |
| Assets/ForestPrototype/ForestSaveValidation.cs and partials (.Geometry, .RackV2, .Storms) | Preflight contract; duplicates and old schema; raw-JSON precedent. | Only narrow proposed validation; preserve current malformed index, competitor, rack, prescription, event and species checks; no weakening. |
| Assets/ForestPrototype/StandGeometryModel.cs and ForestSaveValidation.Geometry.cs | Compact geometry dispatch and unknown fail-closed (B-11). | Known 0/1 behaviour and pre-v20 → Legacy40 mapping byte-for-byte unchanged. |
| Assets/ForestPrototype/UI/StandaloneSessionMenu.cs and .Startup.cs | Current F10 legacy path literal, Continue/New routes; must obey the selected profile. | Assign with #424/UI owner and inspect newer screen changes before editing. |
| Assets/ForestPrototype/UI/LearningObjectivesView*.cs, MenuHelpView*.cs | Profile-supplied prefix; per-profile reset (SI-09/FR-10). | Legacy prefix constants and keys unchanged; no deletion beyond the active profile's enumerated keys. |
| Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.cs and ScenarioOneDefinition.cs | Current nested ID and late warning; Reference Preview entry/return; compact teaching evidence capture. | Prefer narrow boundary/interface changes; no wholesale manager rewrite. Protected O1 behaviour. |
| Assets/ForestPrototype/ScenarioOne/ScenarioReferenceArchive.cs | Read-only consumer; profile gate placed before IdentityMatches. | Archive bytes, hash helpers and anchors untouched. |
| ForestStartingStand, scene/bootstrap/spawner, compact or O2 authored-world code | Only after the actual startup architecture (B-10) and separately approved world specs are known. | Not automatically in scope for P1/P2; needs a separate owner/claim and staged dependency. |
| Tools/Verification/SaveHardeningVerification.cs; SaveCellIndexHardening/, Parity08Save/, RackV2Save/, RackV2Persistence/, TeachingPersistence/, Startup/, ScenarioReferenceVerification.cs | Existing harness locations; fixtures and guard coverage. | A later claim explicitly allocates the chosen tests. Never rewrite historical frozen fixtures to pass. |

These are tentative file proposals, not an allocation overriding live Docs/Project/coordination/active-tasks.md. New helper file(s), if necessary, require review for stable Unity .meta files and a separate exact allowed list. No concurrency with #424's S1 save-controller edits even if other files are disjoint.

## 5. Focused verification strategy

**Static/contract review:** enumerate the caller graph and establish that no save/open/new/preview/prefs path bypasses the profile guard. Review JsonUtility default-field behaviour and raw identity presence at version boundaries. Inspect exact archive hash compatibility and protected startup controls. Confirm that historical serialization output is byte-identical for the same world before and after the change.

**Tier 1 FAST offline:** deterministic table-driven validator cases:
- O1 valid v1, v9, v10, v12, v20, v24, plus newer-than-supported (historical best-effort preserved);
- compact and O2 valid new; missing/blank/conflicting/duplicate/case-variant envelope keys; envelope with the wrong profile, world, geometry or content;
- all six cross-placements; unknown future envelope or payload version; duplicate trees and work IDs; corrupt/truncated JSON; `{}`; no save; foreign .bak;
- path collision tests and absent .tmp checks.

Use existing repository harness commands only after re-inspection; do not invent an executable name. Record byte hashes of every fixture and all three slots before/after. Use narrowed cases rather than re-running a broad irrelevant matrix at every step.

**Tier 2 FEATURE Editor:** on the exact candidate head:
- historical O1 witness: Legacy40, Enlarged80 and Reference Preview same-world;
- compact authored world: first save/restore and the four genuine G1 orders through review;
- O2 authored irregular stand: first save/restore;
- profile switching and repeated annual advances, including an idle year and Work Plan;
- negative foreign loads in all directions, preserving all observed state;
- backup recovery and failure injection as supported;
- PlayerPrefs namespace snapshot; native UI messages and input.

Capture before/after sentinels for scene, geometry, ecology, trees/marks, money, inventory, Work Plan, chronological history, active profile and selection. Use saved data, and render where spatial behaviour matters. Report exceptions and known gaps, and have the result independently inspected.

**Tier 3 FULL INTEGRATED:** not required for each P1/P2 fix. Reserve it for a later nominated integrated release candidate (compact first demo, or complete O2). Do not mislabel a focused Editor smoke as whole-world acceptance. Preserve existing regressions without changing test expectations to achieve green.

## 6. Reviews and decision gates

1. **Draft gate (now):** four DRAFT files + GitHub handoffs only (#436 original, #524 amendment); no implementation authority.
2. **Independent review gate (#524 next):** one nonauthor persistence/doc reviewer examines the three-profile amendment against source and history, under #425.
3. **Design gate:** #425 reviews legacy provenance, fixed naming, raw identity/envelope, preview exception, prefs and scene lifecycle, and obtains owner/save decisions for B-01–B-15 where required. A separate review may return NEEDS CHANGES. Record approval explicitly before any code.
4. **Ownership gate:** #424 controls Scenario One integration and the live lock. A new production claim must record provider/model, exclusive branch/worktree, starting HEAD, owner, allowed files and expected verification. No one may piggyback the #436 or #524 docs claims for coding.
5. **Qualification gate:** exact-head Tier 1/2 with real legacy fixtures/Unity witness and no change to frozen Reference v1, before integration.
6. **Integration gate:** Manager approval after handoff/review, not an implied merge. The O1 complete external demo (compact or historical, per D-082) and the Windows build remain separately gated. O2 Gate 1 stays internal; all six species and lessons stay on hold until their later specs/qualification.

## 7. Failure/stop protocol

STOP production, preserve evidence and request Manager disposition if any of these occur:
- source changed, or a branch/lock overlaps;
- a source fixture is missing, or older-save classification is ambiguous;
- a loaded foreign save mutates anything;
- backup promotion may overwrite a valid file;
- a historical prefs key changes, or a Reference Future hash changes;
- a second router appears.

Do not silently rename files, auto-migrate O1, "fix" historical files or loosen validation.

**No activity performed under this plan:** no local worktree/Unity/build/tests, no source commits, no main or dashboard/Drive synchronization. Its next status can only change through a separate decision and approved workflow.
