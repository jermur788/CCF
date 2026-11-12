# CCF #436 — Independent Scenario Save Isolation V1: DRAFT specification handoff

**Date:** 12 November 2026
**Status:** **HANDED OFF FOR MANAGER REVIEW — DRAFT / NOT APPROVED / NO IMPLEMENTATION AUTHORITY**
**Difficulty:** HIGH (save provenance, cross-world mutation, old-format/Reference Future compatibility).
**Route and actual provider:** OpenAI GPT-6 ChatGPT, connected GitHub browser/doc-authoring lane; requested High, actual runtime reasoning-effort setting not independently attestable.
**Source branch:** docs/o2-save-isolation-four-part-draft-spec-20261112-v1 (new, based on the exact binding packet branch).
**Local worktree / machine / Unity:** None. GitHub-only. No local machine lease, editor, code, build or Player run.
**Claim:** https://github.com/jermur788/CCF/issues/436
**Binding packet:** https://github.com/jermur788/CCF/blob/docs/overall-manager-o2-save-isolation-draft-spec-packet-20261112-v1/Docs/Handoffs/ScenarioPortfolio-20261112/MANAGER-O2-INDEPENDENT-SAVE-DRAFT-SPEC-PACKET-20261112.md
**Ownership:** #424 remains sole Scenario One Delivery Manager and writer of live Docs/Project/coordination/active-tasks.md; #425 Overall Development Manager reviews later approval. No ownership transfer, source/Unity lock or integration action performed.

## 1. Deliverables (all NEW Markdown files only)

1. [spec.md](../../specs/scenario-independent-save-isolation-v1/spec.md) — behavioural identity, slot/backup, fail-before-mutation load and historical compatibility contract plus open questions.
2. [plan.md](../../specs/scenario-independent-save-isolation-v1/plan.md) — minimal additive implementation sequence with source ownership, Tier 1/2 qualification and gates.
3. [tasks.md](../../specs/scenario-independent-save-isolation-v1/tasks.md) — blocked future bounded tasks T-00…T-05 and dispatch/verification constraints.
4. [checklists/requirements.md](../../specs/scenario-independent-save-isolation-v1/checklists/requirements.md) — ISO-01…ISO-36 observable checks, all unchecked/unverified.

**Canonical file links on this result branch:**
- https://github.com/jermur788/CCF/blob/docs/o2-save-isolation-four-part-draft-spec-20261112-v1/specs/scenario-independent-save-isolation-v1/spec.md
- https://github.com/jermur788/CCF/blob/docs/o2-save-isolation-four-part-draft-spec-20261112-v1/specs/scenario-independent-save-isolation-v1/plan.md
- https://github.com/jermur788/CCF/blob/docs/o2-save-isolation-four-part-draft-spec-20261112-v1/specs/scenario-independent-save-isolation-v1/tasks.md
- https://github.com/jermur788/CCF/blob/docs/o2-save-isolation-four-part-draft-spec-20261112-v1/specs/scenario-independent-save-isolation-v1/checklists/requirements.md

Note: relative links above from this handoff's directory are not the canonical navigation route; use the complete branch URLs.

## 2. Source inspection and critical findings (READ-ONLY main, before drafting)

| Source / function | Observed evidence and consequence |
| --- | --- |
| Assets/ForestPrototype/ForestSaveController.cs, SaveFileName, SavePath, Save(), WriteSaveAtomically | One canonical forest-save.json in Application.persistentDataPath, with .tmp/.bak; File.Replace or platform fallback copy. Keep legacy O1 at this exact root/name. O2 must use distinct fixed name and sidecars. |
| ForestSaveController.cs, Update / Load / CanLoad / TryReadSave / FindLoadProblem / LoadData | F5/F9, F10/startup routes converge on controller; current preflight content checks precede ecology geometry mutation in LoadData. Chapter-ID check is missing there. A future unsupported version is logged as best-effort in LoadData, so new multi-scenario validation must fail closed. Public LoadData callers must be included, not just disk UI. |
| Assets/ForestPrototype/ForestSaveData.cs, CurrentVersion / ScenarioOneSaveData | CurrentVersion 24. No top-level scenario identity; nested ScenarioOneSaveData.scenarioId defaults "scenario-one", which is not proof that JSON explicitly contained the key. v1–v9 have no ScenarioOne state. v21 racks, v22 objective achievements, v23 prescriptions, v24 teaching completion and pre-v20 Legacy40 defaults are protected. |
| Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.cs, RestoreSaveData | Mismatch of nested scenario ID currently only Debug.LogWarning and continues **after other mutations**, then restores economy/history/orders. Must not be relied on as foreign-world rejection. |
| Assets/ForestPrototype/ScenarioOne/ScenarioOneDefinition.cs, ScenarioId | Existing default scenario-one ID and v13 definition identity; actual new O2 scene/definition unknown. |
| Assets/ForestPrototype/ForestSaveValidation.cs, Validate | Duplicate tree IDs, stage/index integrity and other preflight guards already exist. Do not weaken them or accept "{}" deserialization defaults as a real forest. |
| Assets/ForestPrototype/UI/StandaloneSessionMenu.cs / .Startup.cs, F10/ContinueSavedForest/StartNewScenario | UI explicitly names Scenario One and checks legacy SavePath; startup Continue closes UI and calls Load after CanLoad. O2 new/continue selection and refusal-safe UI remain future work; current UI is not proof of chapter selection. |
| Assets/ForestPrototype/ScenarioOne/ScenarioReferenceArchive.cs + ScenarioOneManager.TryBeginReferencePreview/EndReferencePreview | Frozen v12 reference uses verified archive and in-memory CaptureData/LoadData restoration; must remain a specifically trusted internal route without rewriting the archive or save. |
| Tools/Verification/SaveHardeningVerification.cs, ScenarioReferenceVerification.cs; Tools/Verification/SaveCellIndexHardening/, Parity08Save/, RackV2Save/, RackV2Persistence/, TeachingPersistence/, Startup/ | Existing test/harness entry points inspected. No tests executed in this docs-only lane. Future tests must be run on exact authorised candidate. |

Read the live active-tasks record without writing and confirmed #424's sole coordination-writer status. Checked the intended result branch did not exist before creation and the intended spec directory was absent. GitHub result branch starts from binding packet branch; main was read-only.

## 3. Proposed contract and positive / negative cases

- **O1 unchanged:** Application.persistentDataPath/forest-save.json (+ existing .tmp/.bak) remains the only historical O1 slot. No automatic renaming, migration, overwrite or archive metadata insertion.
- **O2 additive:** proposed canonical forest-save-scenario-two.json (+ independent .tmp/.bak), fixed allowlisted chapter routing; no O1 fallback or backup transfer.
- **Early rejection:** actual on-disk raw identity + version + content and expected selected scene/definition validated in CanLoad, Load and public LoadData **before** ecology, tree, scene, player, economy, Work Plan or history change. JsonUtility defaults are not identity evidence. Missing old v1–v9 identity is allowed only through controlled O1 legacy route; no O2 classification.
- **Safeguards:** wrong-way O1/O2 file copies, missing/duplicate/null IDs, newer/unsupported versions, partial JSON, corrupted primary/foreign backup, same IDs in distinct worlds and current/old save/backups tested proportionately; no destructive recovery or false success.
- **Preserve reference/legacy:** v1–v24; Legacy40/Enlarged80; frozen Reference Future v1 archive and preview-return; v24 historical teaching latch and S1-C/MFM history. No protected-hash rebaselining.
- **Proof sequencing:** Gate 1 is **INTERNAL ONLY**, reusing O1's Sitka/sessile oak/beech for a genuinely authored irregular O2 stand. Full O2 keeps SIX species: Sitka, sessile oak, beech, Scots pine, downy birch, common alder. The three additions remain HOLD pending separate later modelling/visual/teaching qualification. No chapter declared completed by these documents.

## 4. Open technical matters for Manager review

1. Confirm actual O2 fixed identity (proposed scenario-two), authored stand/scene bootstrap and lifecycle; current inspected O1 startup has no proven second-world loader.
2. Select the narrow explicit new-format top-level identity/version and raw JSON validation approach; inspect genuine old saves (especially v1/v9/v10/v12/v20/v24) before accepting fallback rules for missing nested scenarioId. Do not infer explicit JSON fields from JsonUtility defaults.
3. Determine whether current scene transition design can guarantee pre-mutation/atomic rejection after a scene change; pick a stage/commit seam only after local source/Unity proof.
4. Test platform-specific File.Replace fallback and backup-recovery UX; current disk reader reports .bak availability but does not automatically load it. Preserve valid bytes and require explicit opt-in recovery rather than assumed automatic restoration.
5. Inventory all internal direct LoadData callers (notably preview return); create a narrow authenticated internal reference route, not an unrestricted identity bypass.
6. Establish adequate fixture provenance for every historical version; no claim that all v1–v24 were runtime-tested here. Resolve unknown-field/future-version policy without stripping content.

These are **open technical/design reviews**, not permission to invent the answer in production code. Material changes to the preserved six-species O2 acceptance scope require a separate direct owner decision.

## 5. Verification and honest status

**Document-only work:** five new Markdown files authored on new branch; inspectability and branch-diff/readback to be recorded as a final issue comment/claim close. **No Tier 1 test run, no Tier 2 Editor qualification, no builds, no real file writes, no main merge, no spec approval, no O2 code or six-species model readiness.** There is no local clean-worktree or Unity evidence to assert. The future checklist remains fully unchecked.

**Expected follow-up:** Overall Development Manager #425 performs focused draft review, resolves important open questions with #424's save/source owner, and where necessary commissions independent persistence review. Only a separately approved status with registered implementation claims permits code. O1 source delivery and first complete external-demo/player-test/Windows gates remain unchanged.

**User action required now:** None for draft review. A later explicit owner decision is needed only if changing the six-species complete-O2 requirement or another accepted product boundary.
**Dashboard and Drive mirror:** NOT VERIFIED; no synchronization claim. Canonical git source changes here are new draft artifacts only and do not establish game state or an updated platform attachment.

## 6. Scope audit and completion criteria

Final author task is complete **only if** GitHub readback finds the four drafts plus this handoff, and compare to packet branch shows exactly **five added Markdown files, zero modifications/deletions**, all on the new result branch. Then issue #436 may be marked [HANDED-OFF] and closed; status remains DRAFT throughout. That is documentation delivery, not implementation acceptance.
