# Dashboard-CCF — owner's per-scenario independent saves and Continue decision (G5 receipt)

**20 November 2026 ~02:49 UTC · Owner product decision ACCEPTED · Scope: all CCF scenario identities · Implementation NOT GRANTED · Current Canonical decision-log / Project-context mirror update NOT YET PUBLISHED**

## 1. Exactly what the owner approved

The Overall Manager asked:

> "I recommend resolving the save policy before any Compact30 persistence implementation. My proposal is that **each scenario has its own independent save**, while the original five-rack **`forest-save.json` remains untouched and compatible with historical saves**. Would you approve that direction, with **Continue returning to the correct scenario's saved forest**? This would establish the product policy, not authorise coding yet."

The owner replied: **"Approved."** (20 November 2026, 02:49 UTC).

**Owner-approved G5 product rule:**

1. **Separate scenario save ownership:** each independent scenario has its **own independent saved forest / save namespace**; player world/trees/annual state, Work Plans, economy and history must not accidentally transfer or overwrite another scenario's state. This applies in principle to later O1–O8 cases and any separately approved sandbox context as relevant; the precise count or format of user-visible save slots and any sandbox policy remain to be specified, not invented here.
2. **Legacy original five-rack save:** preserve `Application.persistentDataPath/forest-save.json` for the historical original Scenario One exactly as already authorised. No rename, relocation, overwrite by Compact/other scenario, silent migration, reset, or retroactive reclassification. Retain v1–v24, Legacy40, Enlarged80, Rack V2/MFM and frozen Reference Future v1 compatibility/preview contracts.
3. **Continue is world-correct:** the **Continue** operation must return the player to the **correct scenario's own saved forest**, never silently load or mutate a different world. This establishes the outcome, **not** whether a global menu resumes last played versus a scenario-selected menu; that precise picker, multiple slots per scenario and missing/corrupt save UI are still **OPEN**.
4. **Fail closed:** wrong-scenario/unknown/ambiguous/corrupt data must be refused **before mutation**, with meaningful player feedback and unchanged historical files/state. Preserve atomic save and recovery semantics already specified. No cross-scenario auto-switch inferred from the one-word owner approval.
5. **Interim guard until new profile route qualifies:** on current pinned `main`, `ForestSaveController` and `StandaloneSessionMenu` address the single old `forest-save.json`; the #855 architecture DRAFT §6.7 documents the temporary requirement to **block Save, Load and saved Continue against it from Compact/other nonhistorical scenes (both UI buttons and F5/F9 keyboard paths)** until an independently approved and verified isolated route is installed. Compact may progress in memory in an authorised test, but **do not claim persistence**. **The guard is documented, NOT IMPLEMENTED or native tested.**

**This is product approval, not code/source/Unity/specification acceptance.**

## 2. Reconcile, do not duplicate, the existing approved three-profile work

The above direction is **consistent with and extends** the previously owner-approved 15 November 2026 **three separated profile defaults**: historical original Scenario One, compact Scenario One, and independent Scenario Two, with original `forest-save.json` untouched, Continue visible, F5 immediate with no extra modal, existing F10 replacement confirmation, and Reference Future v1 preview only for historical original O1. The owner has **now expressly reaffirmed / generalised separation for every future scenario**, not directed a competing serializer/router implementation.

**Existing authoritative path (use rather than recreate):**
- [D-079/D-081 and D-083 in main Decision Log](https://github.com/jermur788/CCF/blob/7758e17c882fad01970d2f2fb8c09586bbcab5c1/Docs/Project/decision-log.md): already require per-scenario slots, pre-mutation wrong-world refusal and original historical save protection.
- [#538 three-profile save specification candidate](https://github.com/jermur788/CCF/issues/538) and [#543 reviewed **immutable V2 SPEC SIGNATURE**](https://github.com/jermur788/CCF/issues/543), pinned original V2 contract `200add4e6e5116268b79ef63131ff9b836f14c5d`. The manager's external signature must be checked, not inferred from any older DRAFT banner in source files.
- [#544 post-signature source handoff](https://github.com/jermur788/CCF/issues/544), [#690 conditional D-073 save-source incorporation](https://github.com/jermur788/CCF/issues/690) and source branch `feature/scen1-shared-three-profile-save-v2` (actual candidate only, not declared integrated/main qualified here).
- [#855 revised shared-foundation architecture DRAFT §6.7](https://github.com/jermur788/CCF/blob/ff81d577326ddb95ddb1720ce5068d6e4e11341e/specs/ccf-shared-foundation-architecture-v1/spec.md) and [#854 independent design-document verdict](https://github.com/jermur788/CCF/issues/854#issuecomment-7028683945), adopted by [Overall #425](https://github.com/jermur788/CCF/issues/425#issuecomment-7028742031). Its quality verdict is not implementation approval.

**Implementation policy:** implement or extend **one existing save-routing owner/contract**, only after inspecting actual approved immutable V2, live work/physical custody and required scoped SPEC REQUIRED amendments for extending profiles beyond the initial three. This user decision does not authorise unbounded new save source, future O3–O8 schema/slot literal fabrication, automatic merges or duplicated save policy.

## 3. Remains OPEN or separately gated

- **UX:** whether global Continue loads the most recently played scenario or Continue follows an explicitly selected scenario; multi-slot UX, no-save and corrupt-save messaging. Whichever is chosen must return to the correct source world. The user did not approve a particular menu route or autoswitch.
- **Exact technical policy:** literal new filenames, slot count, profile identifiers, versioned envelopes/field keys, migrated legacy v10–v24 cases, signed Compact world identity, O2 and future profile expansion, atomicity/restore/refusal test contract. Use existing signed V2 where it already settles these; do not overturn it without a separate approved amendment.
- **Scope of authoring starter:** blank 30 × 30m configurable starter is an authoring preview, **not** a newly authorised save profile or a signed Compact world.
- **Source/Unity gates:** single-writer ownership, actual source branch/HEAD, physical Unity lease, independent review, targeted save and historical-data tests, accepted integrated exact game candidate, owner playtest and explicit Windows approval all remain separate. A GitHub receipt cannot prove that UI buttons/F5/F9 are already safe in Compact.
- **Canonical propagation:** Overall #425 / authorised canonical steward should reconcile this new owner receipt into `Docs/Project/decision-log.md` and affected approved save policy at a **single clean context commit** with generator/manifest/mirror/project-attachment publication workflow. **Not done in this browser-only receipt.** Do not independently write the canonical record and leave its composite/mirror stale.

## 4. Disposition and handoff

- **Manager verdict:** **ACCEPT OWNER G5 SAVES DIRECTION**, consistent with current D-079/D-081 and the signed three-profile contract; reserve a separate later product choice for exact global New Game/Continue UX.
- **Current gameplay first:** #844 Compact personal Editor GUI and #849 Multi 2D continue on existing authorised lanes. Neither gains a source/merge grant from this receipt.
- **Immediate safety:** until real per-world routing is integrated and qualified, do **not** use Compact's historical-file Save/Load/Continue paths; protect the original five-rack file.
- **Next action:** route this receipt to [Overall #425](https://github.com/jermur788/CCF/issues/425), [Scenario One 3D #844](https://github.com/jermur788/CCF/issues/844) and existing [shared-save parent #544](https://github.com/jermur788/CCF/issues/544) / [#690](https://github.com/jermur788/CCF/issues/690). Exact future names, new slots beyond first three, technical source tasks and canonical sync require their existing named owners and approved gates.

**Evidence/claim limits:** GitHub/decision/issue source read only. **NOT RUN** Unity/Editor, C# build, actual save/restore/hash, multi-slot UI, legacy regression, original/Compact player test or Windows build. No implementation, main merge, save-profile mutation or canonical decision-log edit performed.
