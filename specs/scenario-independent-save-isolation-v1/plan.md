# Independent Scenario Save Isolation V1 — Implementation Plan

**Status: DRAFT — NOT APPROVED — NO IMPLEMENTATION AUTHORITY.**
Derived from spec.md; does not permit work. Task #436 is documents-only. The Overall Development Manager (#425) must review before any later implementation claim; Scenario One Delivery Manager (#424) controls S1 source integration/Unity and live coordination.

## 1. Design principle and prerequisites

**Minimal additive route:** preserve Scenario One's current Application.persistentDataPath/forest-save.json name and its .tmp/.bak alongside it; introduce a fixed, separately named O2 slot. Route by explicitly selected scenario identity, not by scene name, loaded tree IDs, title string, or old file migration. Share the existing validated save pipeline where possible. Do not extract a generic eight-scenario save framework absent a demonstrated need.

The first commit of this plan is NOT code. Prerequisites for code:
1. Manager reviews spec + checklist, resolves ID/legacy/backup/scene ambiguities and separately marks spec APPROVED.
2. Inspect then-current main/current integration candidate and #424 live sole-writer coordination. Register new task claim and an exclusive source writer for the save controller, save model, scenario restore and relevant UI paths; new branch/worktree and permitted file list must be explicit. Do not edit while #424 or another source owner holds any overlapping file.
3. Establish exact existing O1 save/Reference v1 fixtures and their protected byte/hash anchors, plus the O2 authored-world/definition dependency. The O2 scene/model is a separately approved feature and cannot be invented in this save packet.
4. Unity machine-wide lease, Editor process and exact candidate ownership must be checked immediately before any Unity activity. No cross-device "free" assumption from GitHub alone; no Windows/Player build without the separate personal O1 playtest and user's explicit permission.

## 2. Stage sequence (future, conditional)

| Phase | Purpose and changes | Scope/verification | Go/no-go evidence |
| --- | --- | --- | --- |
| P0 — static map | Audit every current capture, disk Read/Write, F5/F9/F10, startup Continue/New, direct LoadData, Reference Preview, old version and test seam. Confirm O2 identity/root/scene. No code. | GitHub/code inspection, real fixtures, version matrix. | Signed preflight/owner map and resolved identity decisions. |
| P1 — isolated selection + file routing | Small fixed O1/O2 identifier-to-filename mapping and selected-session source of truth. Preserve O1 filename exactly. Separate temp/backup files and no implicit cross-slot fallbacks. | Tier 1: pure path/selection tests, real legacy path assertions, dry-run corruption tests. | O1 slot unchanged, O2 slot disjoint, selected-context routes inventory reconciled. |
| P2 — identity/schema preflight | Add explicit multi-scenario identity where needed; version-aware legacy O1 classification; strict raw JSON presence/mismatch checks; reject foreign/malformed/newer files via shared CanLoad/Load/LoadData preflight BEFORE scene or world mutation. Avoid unsupported future-version best-effort. | Tier 1: table-driven negative cases + static load-entrypoint matrix, untouched-state sentinel. | Both cross-directions rejected identically by CanLoad and actual Load; old O1 readable. |
| P3 — session/scene lifecycle | Wire explicit new/continue selection to actual appropriate authored starting scene and definition. Do not replace current world until target verified. Ensure refused load leaves startup/selection and active forest unmodified; preserve Reference Preview in-memory trusted restore. | Focused Tier 2 Editor with tracked active scene, O1/O2 world differences, preview return. | First O2 save/reload and O1 resume work; wrong-scenario refusal leaves world and UI state intact. |
| P4 — backup and failure qualification | Validate .tmp/.bak per-slot handling, File.Replace and fallback behaviour, missing/corrupt/foreign-backup UX, save failures, read-only old bytes, old-version fixtures, same-ID cross-world history. | Tier 1 offline malformed fixtures, then Tier 2 bounded Editor/IO failure injection on exact candidate. | No data loss, false success, wrong recovery or partial restore. |
| P5 — bounded conformance/integration handoff | Inspect diff against approved paths, requirements trace, independent review proportionate to sensitive persistence, then isolated integration decision. Re-run only impacted gates; do not duplicate whole Tier 3 per packet. | Feature Tier 2; downstream Tier 3 only when O1/O2 release/milestone is separately nominated. | Reviewer APPROVED/NEEDS CHANGES/REJECTED; Manager explicit integration/verification decision. |

## 3. Source ownership and dependency map (tentative, not writable now)

| Path / subsystem | Why likely touched | Ownership restriction |
| --- | --- | --- |
| Assets/ForestPrototype/ForestSaveController.cs | Existing single-slot, F5/F9, CaptureData, CanLoad, TryReadSave, FindLoadProblem, LoadData and atomic writer. | High-risk shared source. One assigned production writer at a time; #424 authorises/relinquishes file ownership before any editing. |
| Assets/ForestPrototype/ForestSaveData.cs | Current v24 layout; optional new explicit identity with backward compatibility. | No frozen archive bytes, hash anchor or silent version policy changes. Coordinate with all S1 save/persistence work. |
| Assets/ForestPrototype/ForestSaveValidation.cs and partials | Preflight contract; duplicates and old schema. | Only proposed narrow validation; preserve current malformed index, competitor, rack, prescription, event and species checks; no weakening. |
| Assets/ForestPrototype/UI/StandaloneSessionMenu.cs and .Startup.cs | Current F10 legacy path, Continue/New routes; must obey selected scenario. | Assign with #424/UI owner and inspect newer screen changes before editing. |
| Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.cs and ScenarioOneDefinition.cs | Current nested ID and late warning; Reference Preview return. | Prefer narrow boundary/interface changes; no wholesale manager rewrite. Protected O1 behaviour. |
| ForestStartingStand, scene/bootstrap/spawner or O2 authored-world code | Only after actual O2 startup architecture and separate approved O2 authored-forest spec are known. | Not automatically in scope for P1/P2; may need a separate owner/claim and staged dependency. |
| Tools/Verification/SaveHardeningVerification.cs; SaveCellIndexHardening/, Parity08Save/, RackV2Save/, RackV2Persistence/, TeachingPersistence/, Startup/, ScenarioReferenceVerification.cs | Existing harness locations; fixtures and guard coverage. | Later claim explicitly allocates chosen tests. Never rewrite historical frozen fixtures to pass. |

These are tentative file proposals, not an allocation overriding live Docs/Project/coordination/active-tasks.md. New helper file(s), if necessary, require review for stable Unity .meta files and a separate exact allowed list. No concurrency with #424's S1 save-controller edits even if other files are disjoint.

## 4. Focused verification strategy

**Static/contract review:** enumerate caller graph and establish that no save/open/new/preview path bypasses the expected-scenario guard. Compare current-only target and source directory behaviour. Review JsonUtility default-field behavior and raw identity presence at version boundaries. Inspect exact archive hash compatibility and protected startup controls.

**Tier 1 FAST offline:** deterministic table-driven validator cases (O1 valid v1, v9, v10, v12, v20, v24; O2 valid new; missing/blank/conflicting/duplicate JSON identity keys; wrong chapter in correct and copied file; unknown future version; duplicate trees and work IDs; corrupt/truncated JSON; no save; foreign .bak); path collision tests and absent .tmp checks. Explicitly use existing repository harness commands only after re-inspection; no invented executable name. Save fixture byte hashes before/after. Use narrowed cases rather than re-running a broad irrelevant matrix at every step.

**Tier 2 FEATURE Editor:** exact candidate head, O1 legacy/Enlarged80/Reference Preview same-world witness; O2 authored irregular stand first save/restore; two chapter switching and repeated annual advances including idle year and Work Plan; negative foreign load both ways preserving all observed state; backup recovery and failure injection as supported; native UI messages and input. Capture before/after sentinel for scene, geometry, ecology, trees/marks, money, inventory, Work Plan, chronological history and current selected world; use saved data and render where spatial behaviour matters. Report exceptions/known gaps and independently inspect result.

**Tier 3 FULL INTEGRATED:** not required for each P1/P2 fix. Reserve for a later nominated integrated release candidate/complete O2; do not mislabel a focused Editor smoke as whole-world acceptance. Preserve existing regressions without changing test expectations to achieve green.

## 5. Reviews and decision gates

1. **Draft gate (now):** four DRAFT files + GitHub handoff only; no implementation authority.
2. **Design gate:** #425 reviews legacy provenance, O2 fixed naming, raw identity, preview exception and scene lifecycle; obtains user product decision only if scope changes. Separate review may return NEEDS CHANGES. Record approval explicitly before any code.
3. **Ownership gate:** #424 controls current Scenario One integration and live lock. A new production claim must record provider/model, exclusive branch/worktree, starting HEAD, owner, allowed files, expected verification; no one may piggyback #436 docs claim for coding.
4. **Qualification gate:** exact-head Tier 1/2 with real legacy fixtures/Unity witness and no change to frozen Reference v1, before integration.
5. **Integration gate:** Manager approval after handoff/review, not an implied merge. O1 complete external demo and Windows build remain separately gated. O2 Gate 1 stays internal; all six species and lessons stay on hold until their later specs/qualification.

## 6. Failure/stop protocol

If source changed, branch/lock overlaps, source fixture is missing, older save classification is ambiguous, a loaded foreign save mutates anything, backup promotion may overwrite a valid file, or Reference Future hash changes: STOP production, preserve evidence, request Manager disposition. Do not silently rename files, auto-migrate O1, "fix" historical files or loosen validation.

**No activity performed under this plan:** no local worktree/Unity/build/tests, no source commits, no main or dashboard/Drive synchronization. Its next status can only change through a separate decision and approved workflow.
