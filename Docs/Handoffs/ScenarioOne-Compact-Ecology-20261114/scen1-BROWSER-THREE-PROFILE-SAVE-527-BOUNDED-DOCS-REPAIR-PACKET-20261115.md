# scen1 — #530 author-only bounded three-finding repair of three-profile save DRAFT

**Status:** [#530 READY / NOT RUNNING](https://github.com/jermur788/CCF/issues/530). **Packet published:** 15 November 2026 by Scenario One Manager #424, OpenAI GPT-6 (GitHub/browser documents only). **Difficulty:** LOW–MEDIUM: correct three evidence/authority wording contradictions, no save implementation or new policy decisions.
**Best-suited author:** original #524 **Claude Opus 5.5** browser/GitHub author (but an independently #425-delegated docs editor is acceptable); register the **actual** provider/model. The independent #527 reviewer must **not** approve its own work.
**Authority STOP:** Overall Manager #425 is the accountable **single** shared save-policy/spec owner. The earlier #524 delegation was specific to that completed task and **does not by itself authorise #530**. Obtain #425's **explicit #530-specific documentation-only delegation** in GitHub before modifying the spec. If absent, record [BLOCKED — #425 AUTHORISATION] on #530 and stop. Neither #424 nor the new docs worker may decide historical save compatibility or the F5 UX on behalf of owner/save authority.

## 1. Exactly which published evidence is authoritative

- **Independent nonauthor finding:** [#527 immutable research/source-review report](https://github.com/jermur788/CCF/blob/cb2013782238f89a6f9deb9d6e73db4a8a4605d7/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261114/scen1-INDEPENDENT-THREE-PROFILE-SAVE-DRAFT-REVIEW-RESULT-20261115.md); [#527 HANDED-OFF/CLOSED](https://github.com/jermur788/CCF/issues/527). **Verdict: NEEDS CHANGES (DRAFT QUALITY ONLY)**, **2 MAJOR +1 MINOR** genuinely repair-required documentation inconsistencies; 2 nonblocking advisories A-1/A-2 that do not require a larger cycle.
- **Amended source DRAFT to repair:** [#524 branch spec](https://github.com/jermur788/CCF/blob/docs/shared-three-profile-save-policy-20261114-v1/specs/scenario-independent-save-isolation-v1/spec.md) and [63-requirement checklist](https://github.com/jermur788/CCF/blob/docs/shared-three-profile-save-policy-20261114-v1/specs/scenario-independent-save-isolation-v1/checklists/requirements.md), current author head `89bdab4b9f0b137f9837a33232a4eb2430e6e9f6` (confirm fresh). Original pre-#524 #436 DRAFT base `504969298451327aa8a80cc7a4014e5e12953742`. Four specification Markdown files exist on **that docs branch, not on main**; do not create duplicate files or merge unrelated main coordination changes.
- **Read-only direct code evidence:** `Assets/ForestPrototype/ForestSaveData.cs` (`version = CurrentVersion`, currently 24), `ForestSaveController.cs` (nonpositive parsed version normalized, F5 direct save no confirmation), `ForestSaveValidation.Geometry.cs` (pre-v20 model 0 permitted, nonzero rejected), `UI/StandaloneSessionMenu.cs` (F10 save replacement confirmation), `ForestSaveValidation.cs` (raw missing/default trap). Read actual main HEAD before asserting exact line numbers. The [#527 independent report](https://github.com/jermur788/CCF/blob/cb2013782238f89a6f9deb9d6e73db4a8a4605d7/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261114/scen1-INDEPENDENT-THREE-PROFILE-SAVE-DRAFT-REVIEW-RESULT-20261115.md) links pinned code lines and explains runtime was **NOT RUN**.
- **Overall #425 decisions still open:** [#521 bounded intake](https://github.com/jermur788/CCF/blob/467244f3f679ca210e4de1f3c195bd31dfc6f156/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261114/scen1-OVERALL-425-COMPACT-PRODUCT-AND-SAVE-DECISION-RESULT-20261114.md); #524 current spec section 8.2 **B-01…B-15** and 8.3 **Q-01…Q-03**. These are NOT decided by source wording correction. D-082/#471 remains paused, parent/child compact specs still DRAFT. Original five-rack/Rack V2/MFM/Legacy40/model0/v1–v24 and frozen Reference Future v1 remain protected.

## 2. Reviewer findings to repair — no more, no less

### M-1 — Raw-missing historical version versus an explicitly parsed nonpositive version (MAJOR)

**Location:** `spec.md` FR-07 version table at lines **109–115** in pinned #524 head, and associated `requirements.md` **ISO-49** (around line 82). Source facts `ForestSaveData.version` initialises to supported CurrentVersion, while the existing controller normalises **a parsed numeric `version <= 0`** to 1. A raw-omitted key is not source-equivalent to a parsed <=0 value; the draft currently incorrectly says **“Missing / ≤ 0 version → existing: treated as v1 [SOURCE]”**.

**Required correction:**
- Split historical **raw-absent version** from **explicit present numeric 0/negative parsed version** in FR-07 description or version table, with appropriate **[SOURCE OBSERVED] / [PENDING OWNER RULE]** classification. Explicit nonpositive parsed version normalisation is source-observed; raw absent version may default under JsonUtility to current initialized version and then be refused by other raw validators in the disk route. Do not assert every raw-missing historical file loads or maps to v1.
- Add a focused **missing raw key vs explicit zero vs explicit negative** requirement/case to the existing **ISO-49** or immediately relevant existing version-case entry **without renumbering/checking**. Clarify what source currently does and what remains a future profile-policy decision; do not alter B-03/B-04/B-05/B-13, choose a new acceptance rule, or promise a passed runtime witness.
- Leave separate compact/O2 strict missing/newer-version refusal intact as DRAFT design; no global tightening of authentic original O1 higher-version warn+best-effort.

### M-2 — F5 overwrite confirmation contradiction with OPEN Q-03 (MAJOR)

**Locations:** `spec.md` **W-02** (near line 45) describes mandatory overwrite confirmation, **FR-05** (near line 103) and **Q-03** (near line 181) explicitly say whether **F5** needs confirmation is an unresolved owner UX choice; checklist **ISO-03** (near line 12) again mandates it across Save. Existing source F5 save does NOT ask, but F10 replacement UI does.

**Required correction:**
- Make W-02 and ISO-03's **F5-specific confirmation** conditional on explicit **Q-03 owner decision**. Preserve source-current F5 behavior as observed, retain existing F10 overwrite confirmation (and any required user-initiated recovery confirmation), truthful status and strong **same-slot, no-cross-profile-overwrite** safety.
- The documentation author **must not decide Q-03** or recharacterise F5 direct save as a proven UI prompt. The shared save requirements must provide one consistent later PASS/FAIL interpretation once Q-03 is settled.
- Make corresponding narrow adjustment to FR-05 only if needed for coherence. Do not remove the protective no-cross-profile sidecar checks and transaction/failure requirements.

### m-1 — Pre-v20 geometry key overstatement (MINOR)

**Location:** `spec.md` source facts §2 near line **29**. Current wording says pre-v20 save “that claims a model is rejected.” Reviewer's directly cited validator source allows an explicitly represented geometry **zero** for pre-v20, rejects a **nonzero** value, and interprets pre-v20 as **Legacy40**. v20+ disk saves require an explicit valid integer geometry field (per read-only source).

**Required correction:** Change only the inaccurate sentence to say **“pre-v20 nonzero geometry is rejected; pre-v20 is interpreted as Legacy40”**, explicitly permitting zero. Keep known historic 0/1 and v20+ raw geometry/identity validation rules otherwise untouched. No validator/code modification.

### Advisories A-1 and A-2 — not release-blocking for this documentation repair

- A-1: once owner **B-13** chooses payload version lineage, ensure envelope payload-save-version is consistent with nested `payload.version` or approved translation and has an appropriate mismatch test. You **may** add a short advisory cross-reference **without deciding B-13**, but **do not expand into a new schema design** here.
- A-2: envelope root-key disjointness is **not sufficient alone** to stop `JsonUtility` default-deserialisation; the mandatory raw marker/preflight rejection is decisive. Preserve the explicit guard; no additional policy decision or large revision required.

## 3. Exact allowed worker execution

**Task claim:** [#530](https://github.com/jermur788/CCF/issues/530), currently READY / NOT RUNNING.  
**After #425 delegation**, check LIVE `Docs/Project/coordination/active-tasks.md` READ ONLY; don't edit. Check whether #524 author branch or #530 target/output is owned/changed. Create a **NEW** isolated branch `docs/scen1-530-three-finding-save-draft-repair-20261115-v1` from the **current VERIFIED #524 amended four-file DRAFT head** `89bdab4b9f0b137f9837a33232a4eb2430e6e9f6` if unchanged. Record actual full starting HEAD and no local worktree/Unity lease; do not overwrite the #524 or main branch. If HEAD moved, branch exists or owner conflict, STOP and re-route rather than silently using an unrelated base.

**Before ANY tracked edit:** post **[RUNNING]** on #530 with actual provider/model, #425's task-specific authorisation link, new branch/start HEAD, worktree **NONE**, Unity/Editor machine lease **NONE**, and allowed file set. GitHub claim records execution; it does not grant source/board/Unity ownership.

**Only existing writable paths (minimum)**:
- `specs/scenario-independent-save-isolation-v1/spec.md` — three findings.
- `specs/scenario-independent-save-isolation-v1/checklists/requirements.md` — conditional ISO-03 and version-case ISO-49.

**Conditional additional existing paths:** `specs/scenario-independent-save-isolation-v1/plan.md` and `tasks.md` **ONLY IF** you find a directly contradictory sentence caused by M-1/M-2/m-1 and cite it in the report. No unrelated restructuring, checklist weakening or B/Q resolution.

**Exactly one NEW handoff**:
`Docs/Handoffs/ScenarioOne-Compact-Ecology-20261114/scen1-SHARED-SAVE-527-THREE-FINDING-BOUNDED-REPAIR-RESULT-20261115.md`

No changes to `Docs/Project/decision-log.md`, `current-milestone.md`, `coordination/active-tasks.md`, compact parent/child DRAFT, #527 independent report, #524 handoff, C#/Assets/Packages, saved worlds, PlayerPrefs, Reference archives, Unity or dashboard/Drive. No Windows build.

**Checklist and scope exit:** original **ISO-01…ISO-36** keep order/tags; appended **ISO-37…ISO-63** stay unique and contiguous; **63/63 unchecked**, even if one item is wording-corrected. **B-01…B-15 and Q-01…Q-03 remain OPEN**. Both new and original-world save routing stay **PROPOSED / DRAFT** and not implemented. Correct only three document problems, not a new compatibility matrix or acceptance policy.

## 4. Return, evidence and closure

Publish one new **`scen1 —`**-heading result Markdown at `Docs/Handoffs/ScenarioOne-Compact-Ecology-20261114/scen1-SHARED-SAVE-527-THREE-FINDING-BOUNDED-REPAIR-RESULT-20261115.md` with actual model/provider, #425 delegation, branch and true starting HEAD, file:line before/after for M-1/M-2/m-1, explicit note about A-1/A-2, direct pinned source references, preservation of open owner decisions, 63 unique unchecked requirements, expected spec status, list of changed files, and **NOT RUN** (Unity/save/prefs/compile/old-world/reference tests).

Read back modified files/report from GitHub, compare author branch to registered #524 base:
- **Expected minimum:** **2 MODIFIED Markdown** (`spec.md` + `requirements.md`) and **1 ADDED `scen1-` result**.
- If direct M-1/M-2/m-1 contradictions required touching `plan.md` or `tasks.md`, list precise reasons and show only those changes; **no unrelated tracked changes**.

Post #530 **[HANDED-OFF]** with direct GitHub blob URL and diff evidence; close the execution claim; notify [#424](https://github.com/jermur788/CCF/issues/424) and [#425](https://github.com/jermur788/CCF/issues/425). The next step is **at most one independent delta review of exactly these three repairs**—not another full 63-item audit. #425/owner decide all B/Q gates separately after DRAFT quality is approved. Dashboard/Drive status: **NOT VERIFIED**.

## Paste-ready author message

> Take ownership of [CCF #530](https://github.com/jermur788/CCF/issues/530) as the **author of a THREE-FINDING save-policy DRAFT documentation repair**, after obtaining **new express Overall Manager #425 authorisation for #530**. Follow the complete binding `scen1` packet: https://github.com/jermur788/CCF/blob/docs/scen1-527-save-draft-bounded-repair-dispatch-20261115-v1/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261114/scen1-BROWSER-THREE-PROFILE-SAVE-527-BOUNDED-DOCS-REPAIR-PACKET-20261115.md. Register actual provider/model, fresh verified #524 DRAFT starting HEAD, new `docs/scen1-530-three-finding-save-draft-repair-20261115-v1` branch, worktree NONE, Unity lease NONE and the narrow existing-file allowlist before editing. Fix ONLY #527 M-1 (raw missing historical save version is NOT the same as parsed <=0→v1; add focused ISO-49 cases), M-2 (W-02/ISO-03 F5 confirmation conditional on unresolved Q-03 while preserving existing F10 confirmation), and m-1 (pre-v20 geometry permits explicit zero, rejects nonzero). Preserve 63 ISO numbers **UNCHECKED**, B-01..B-15/Q-01..03 OPEN, historical O1 newer-version behavior and protected saves/Reference. No new policy choices, source, Unity, canonical, live board or original fixture changes. Publish ONLY the exact new `scen1-SHARED-SAVE-527-THREE-FINDING-BOUNDED-REPAIR-RESULT-20261115.md` GitHub handoff; read back and compare 2 modified save DRAFT Markdown +1 added report unless directly justified small plan/tasks edit; close #530 HANDED-OFF and notify #424/#425. If #425 has not delegated, mark BLOCKED and do not write.
