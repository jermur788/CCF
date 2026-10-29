# Browser Handoff — S1-D T-04 Persistence Amendment

## Task identity

- **Claim:** #136
- **Parent:** #91 `PLAYER-TEST-OVERNIGHT-01`
- **Task class:** browser/GitHub-only Manager/spec amendment; **no production C#/Unity implementation**
- **Status at publication:** **PREPARED FOR DISPATCH**
- **Prepared against `main`:** `e3c993a441782665f18362899175e49e36fd04a1` — refresh live `main` before writing; do not assume this SHA is still current
- **Purpose:** encode the user's accepted Option A for S1-D T-04: preserve full guide-monotonicity intent by adding the **smallest durable aggregate record** needed to remember that the teaching sample/review has historically completed across save/load and later world drift. **Do not persist individual `[J]` / `[K]` decisions.**
- **Difficulty:** **3/5** — no local code/Unity work, but the browser worker must reconcile an approved spec, save/schema sequencing, PARITY-08 ownership, and exact downstream packets without creating a second persistence writer.
- **Best-suited model/provider:** **OpenAI GPT-5.6 Sol browser/GitHub lane** or an equivalent strong browser-capable reasoning model.
- **Lane / branch / worktree:** canonical GitHub browser lane on `main`; **no local worktree**. While RUNNING, this browser claim is the single writer for the canonical S1-D spec/amendment/Decision Log/handoff files it touches.

## Accepted product decision

The user explicitly selected **Option A** after review of pushed S1-D T-04 candidate `b9d204c`.

The accepted direction is:

1. Keep the approved FR-B1 / AC-3 intent: a historically completed teaching stage must not regress because of Crop Tree count/loss, later in-footprint felling, executed-share drift or save/load.
2. Permit the **minimum durable aggregate state** required to remember that the teaching sample/review itself has historically completed.
3. **Do not persist the individual `[J] Retain here` / `[K] Release further` decisions, inspected-tree list, stand-portion review detail, or a replayable session-review transcript.**
4. The persisted record is authority only for historical teaching-sample completion / guide monotonicity and the T-03 shared sample-complete interpretation. It must not become a forestry prescription, management result, completion predicate, MFM state, or substitute for current-world inspection.
5. Preserve the frozen Reference Future v1 contract, Legacy40 compatibility rules, protected completion/economy/ecology behaviour, and the no-Windows-build gate.

This latest direct user decision supersedes only the previous S1-D V1 prohibition on *any* new persisted field to the minimum extent necessary for this aggregate teaching-completion record. It does **not** reopen broad persistence or authorise individual `[J]/[K]` storage.

## Why this amendment is required

Pushed candidate `b9d204c` correctly removed the rejected surrogate proofs, but then exposed the approved packet's stop condition: existing persisted facts do not faithfully prove the session-only release-review boundary. Its handoff states that a reload from lessons 7/8 can fall back to lesson 5 until `[J]/[K]` is repeated, and Crop Tree loss can expose earlier lessons again. That is narrower than approved FR-B1 / AC-3.

The Manager therefore blocked #124 on a product/spec decision. The user selected the persistence option rather than weakening AC-3.

## Required browser work

### 1. Refresh authority and ownership first

Before editing:

- refresh `Docs/Project/coordination/active-tasks.md` live from GitHub;
- refresh claims #91, #106, #124, #126 and #128;
- inspect current `main`, current post-MFM-07 integration truth, and any newer save/schema owner;
- mark **#136 `[RUNNING]`** with actual model/provider and starting `main` SHA;
- register browser ownership in live `active-tasks.md` for the canonical S1-D spec/Decision Log/amendment files being edited;
- do **not** claim production C#/Unity/save implementation ownership in this browser task.

At handoff publication time, PARITY-08 #126 has been Manager-blocked from starting until this amendment fixes the save/schema sequencing. Do not silently unblock it before an exact lineage and ownership order are specified.

### 2. Record the accepted decision canonically

Update `Docs/Project/decision-log.md` using the next valid decision ID after refreshing the live file. Record, in substance:

- full S1-D guide monotonicity remains required;
- one minimal durable aggregate teaching-sample/review-complete record is authorised;
- individual `[J]/[K]` decisions and session review detail remain non-persistent;
- the field exists for historical teaching progression only and does not change scenario completion, objective predicates, forestry treatment semantics, or MFM;
- save/schema compatibility and independent review are mandatory.

Do not invent a decision number from this handoff; use the next valid live ID.

### 3. Amend the approved S1-D spec package

Inspect and amend at minimum:

- `specs/scenario-one-repeated-interventions-v1/spec.md`
- `specs/scenario-one-repeated-interventions-v1/plan.md`
- `specs/scenario-one-repeated-interventions-v1/tasks.md`
- `specs/scenario-one-repeated-interventions-v1/checklists/requirements.md`

The amendment must reconcile all statements that currently say S1-D V1 adds no persisted field / T-07 is cancelled / FR-F2 forbids persistence.

Required semantic result:

- preserve FR-B1 and AC-3 rather than weakening them;
- authorise only the minimum aggregate completion record;
- specify exactly when it becomes `true`: only when the existing in-session teaching sample/review has **actually completed under the authoritative pre-amendment lesson rules**, not from planting, arbitrary felling, a later review, current state, or any surrogate;
- once true, it is monotone historical state; later world changes do not unset it;
- loading an older save with the field absent must default conservatively to **not historically complete**, never fabricate completion;
- the individual `[J]/[K]` decisions remain session-only and are still repeated as current inspection input where appropriate before historical completion;
- `ScenarioOneCompletionRules`, the eight scenario objective predicates/targets, economics, ecology, Rack V2 geometry/lifecycle, and MFM logic stay out of scope;
- no Reference Future v1 rebaseline or content change.

### 4. Choose and document the bounded implementation task structure

Do **not** leave the local worker to rediscover task architecture.

Because #124's current packet excludes save/schema work, create an explicit authorised persistence implementation unit in the S1-D task structure. Preserve historical task records honestly. Preferred approach is a **new bounded persistence prerequisite/subtask** rather than silently pretending the already-cancelled T-07 was never cancelled; if you reactivate or supersede T-07 instead, make the history/status transition explicit and justified in the spec/tasks.

The resulting execution order must be unambiguous, for example:

`browser amendment accepted -> minimal persistence implementation + save/load qualification -> #124 guide candidate rebased/finished using that field -> independent review -> integration`

The persistence unit must require independent review because it changes the shared save/load contract.

### 5. Reconcile save/schema ownership with PARITY-08

This is a critical deliverable.

At publication time:

- integrated MFM-07 umbrella is `c8cfae8c4025cec6e960a050edf4bfde2a981de5` with save schema v23;
- #126 PARITY-08 was originally prepared as the prospective sole save/validator/hash/reference writer and is now **BLOCKED on this amendment**;
- #128 is its independent review lane and remains blocked.

Refresh live state, then decide and record the safe single-writer order. The browser worker must not create two concurrent save/schema writers.

Expected policy unless live evidence requires a different safe order:

- S1-D minimal persistence gets one explicitly owned local save/schema lane first (or is deliberately merged into a single coordinated save writer if that is cleaner and ownership-safe);
- PARITY-08 is repinned/released only after the resulting exact candidate/integration lineage is known, so its compatibility checks cover the new field rather than qualifying an obsolete v23-only state;
- do not preassign a schema version until the live repository/ownership state is inspected; use the next safe version exactly once;
- old v23 and earlier saves must remain loadable according to accepted compatibility policy, with the new aggregate defaulting false/absent;
- Reference Future v1 remains frozen.

If a currently running writer has acquired save/schema ownership by the time this browser task starts, **STOP and return to Manager** rather than overlapping.

### 6. Amend the #124 implementation packet / Manager review

Update the authoritative S1-D T-04 packet and review trail so Ultra Code/local implementation receives one exact contract:

- `Docs/Handoffs/S1-D-Implementation-04/TASK-PACKET.md`
- `Docs/Handoffs/S1-D-Implementation-04/MANAGER-REVIEW.md`

Make clear that `b9d204c` remains useful implementation evidence but is **not accepted/qualified** and will require rebase/continuation after the persistence prerequisite.

Remove the ambiguity that led to the product stop: full AC-3 remains binding and the new aggregate is the authorised faithful durable proof.

### 7. Prepare the exact local follow-on packet(s)

Publish one or more GitHub Markdown task packets for the local Code/Unity lane. Each must include:

- exact base SHA / integration lineage current at browser completion;
- exact branch/worktree suggestion;
- file ownership and explicit exclusions;
- save version/migration/default behaviour only after live inspection determines the safe number;
- required offline checks;
- `CanLoad` / `LoadData` parity and pre-mutation/load safety as applicable;
- old-save, current-save, Legacy40 and frozen Reference Future compatibility gates;
- guide save/reload witnesses for lessons 6–8;
- Crop Tree loss/count change, later felling and executed-share drift witnesses after historical completion;
- proof that individual `[J]/[K]` choices are not serialized;
- N3/protected completion/hash checks with **no rebaseline**;
- TeachingCopy/MenuTutorial/S1-C narrow regressions;
- mandatory independent review;
- no Windows build.

If the minimal field requires touching files currently reserved for PARITY-08, the packet must make the ownership order explicit and keep #126 blocked until that lane hands off.

### 8. Update live coordination and claims

When the amendment is complete:

- update `active-tasks.md` to reflect the accepted persistence decision, exact new dependency order and which claim is next executable;
- update #124 from `[BLOCKED — PRODUCT DECISION]` to the correct new state (typically blocked on the persistence prerequisite, or prepared if that prerequisite has been packaged and no dependency remains);
- update #126/#128 status consistently;
- create any new substantive implementation claim(s) required by the amended task structure;
- close #136 as `[HANDED-OFF]` only after the canonical amendment and downstream packet(s) are published.

## Evidence / files to inspect

At minimum inspect:

- `Docs/Handoffs/S1-D-Implementation-04/HANDOFF.md` on candidate `b9d204c`
- `Docs/Handoffs/S1-D-Implementation-04/MANAGER-REVIEW.md` on `main`
- `Docs/Handoffs/S1-D-Implementation-04/TASK-PACKET.md`
- full approved `specs/scenario-one-repeated-interventions-v1/` package
- current `Docs/Project/decision-log.md`
- current `Docs/Project/coordination/active-tasks.md`
- claim #124
- claim #126 and its PARITY-08 packet/release amendment
- claim #128
- post-MFM-07 integration handoff/current exact umbrella lineage
- current save schema/validation facts from inspected repository/verified handoffs; do not rely on stale snapshot text if live Git evidence differs.

## Required outputs

1. Canonical Decision Log update recording Option A.
2. Approved S1-D spec/plan/tasks/checklist amendment that authorises the minimal aggregate and preserves FR-B1/AC-3.
3. Updated #124 packet/Manager review trail.
4. Exact local persistence + T-04 continuation packet(s), with one safe save/schema writer order.
5. Live coordination/claim reconciliation, including #126/#128.
6. Durable browser result:
   `Docs/Handoffs/S1-D-T04-Persistence-Amendment-20261029/BROWSER-RESULT.md`

The browser result must state: what changed; exact commits; what is now authorised; save/schema ownership order; which claim is next executable; what remains blocked; user action required or none; dashboard/coordination sync status.

## Stop conditions

Stop and return to Manager if:

- live ownership shows another writer already editing the same canonical spec/Decision Log files;
- a save/schema writer has started and cannot safely yield/sequence with this amendment;
- preserving full AC-3 would require persisting individual `[J]/[K]` choices or substantially more session detail than one minimal aggregate;
- the minimal field would need to alter `ScenarioOneCompletionRules`, objective predicates/targets, ecology/economy, Rack V2 semantics, MFM prescription semantics, or frozen Reference Future v1;
- a legitimate accepted old save cannot be migrated/defaulted conservatively without a new compatibility product decision;
- any proposed implementation would weaken tests or rebaseline protected hashes merely to pass.

## Handoff protocol

At start, comment on #136 with:

`[RUNNING] Browser S1-D T-04 persistence amendment — <model/provider>, canonical main start <SHA>, no local worktree/Unity.`

At completion, publish `BROWSER-RESULT.md`, comment on #136 with the exact result/commit(s), rename it `[HANDED-OFF]`, close it, and return to the Overall Manager.

## Copy/paste dispatch line

`Run browser claim #136 from Docs/Handoffs/S1-D-T04-Persistence-Amendment-20261029/BROWSER-HANDOFF.md. The user chose Option A: preserve full S1-D FR-B1/AC-3 by authorising the smallest durable aggregate teaching-sample/review-complete record, while individual J/K decisions remain non-persistent. This is a browser/GitHub-only spec/decision/coordination task: refresh live ownership and main first; mark #136 RUNNING; amend Decision Log and the approved S1-D spec/plan/tasks/checklist; reconcile save/schema ownership with blocked PARITY-08 #126/#128; update #124's packet/review; prepare exact local implementation/qualification packet(s); update active-tasks; publish BROWSER-RESULT.md and close #136 HANDED-OFF. Do not implement C#/Unity, do not invent a schema version without live inspection, do not weaken AC-3, do not persist individual J/K, do not change frozen Reference Future v1, and do not build Windows.`