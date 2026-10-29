# S1-D T-04P — one-way teaching completion

**Status: IMPLEMENTED / OFFLINE VERIFIED; UNITY QUALIFICATION IN PROGRESS; INDEPENDENT REVIEW PENDING.**

Claim #143; parent #91 / #106. Executor: T3 Code / Codex harness / OpenAI gpt-6.1-sol, high reasoning. Branch `feature/s1-d-t04-teaching-persistence-v1`, worktree `/media/jer/Videos/CCF-S1D-T04-Persistence`. Exact clean start `c8cfae8c4025cec6e960a050edf4bfde2a981de5`; production implementation `129d0aced1f9935ce683cfcf68169285d1316c4f`; exact runtime candidate `b9bf11aaf8f6fd7ea43e33e39258560408b88b12` (one-line verification diagnostic repair; production identical). Schema **v24**. No integration, Manager acceptance or Windows build.

## Preflight / authority

Refreshed origin/main coordination and claims #91/#106/#143/#124/#126/#128 before production edits. Exact pinned production and the then-current umbrella retained v23; its two subsequent commits added only MFM-07 handoff documentation. No competing save/schema/hash owner; v24 free. Created the requested worktree at the exact start and recorded RUNNING on #143 at `2026-10-29T23:29:35Z` ([receipt](https://github.com/jermur788/CCF/issues/143#issuecomment-6510967372)). Later browser #159 reconciliation incorrectly said no start existed; linked the actual start to #91/#159 for the single-writer Manager to reconcile. No Manager-owned live file was overwritten. Unrelated Primary changes preserved.

Authority: #158 APPROVED and explicit #143 release/execution handoff; repaired D-066 contract/spec on main. Production starts exactly at the required qualified umbrella, rather than merging unrelated later main/feature work.

## Implementation

Only new saved fact: `ScenarioOneSaveData.firstGuidedRackReviewComplete`. Runtime owner/read: `ScenarioOneManager.FirstGuidedRackReviewComplete` (read-only). Manager capture writes that one value; restore reads it only for v24+, otherwise false. New scenarios start false. Loading a different/older save selects that save's own history; it cannot reconstruct completion from its trees/orders/review age. No session decision is restored.

Exact write hook: `LearningObjectivesView.DecideRackInspection`, immediately after a successful actual `ScenarioOneRackReview.Decide`. It rederives facts from the real session review and calls `TryRecordFirstGuidedRackReview(before, after)`. That method returns without writing if already true or in Reference preview. `CrossesFirstRackReview` requires non-null rack bays, a newly completed session rack review, ReleaseReviewed, exact old unlatched index4 before and index5 afterwards. `CurrentUnlatched` preserves the pre-amendment guard order; verification compiles the exact old sequence as a separate oracle. Display/read refresh, load/restore, T-03's helper and later world state never earn the latch.

Before the latch, guide/cap behavior is unchanged. After it, both consume the same manager historical read. Teaching starts at recruitment or the later current planting/establishment state; T-03 remains advisory above the historical cap. #124's separate later curriculum replay remains unqualified/out of scope.

The save DTO diff is exactly one boolean and the version bump. No individual J/K choices, inspected tree IDs, stand portions, transcript/order, counters, cycle keys, guide index or readiness state is serialized. The runtime facts/session objects remain outside the save DTO.

## Identity compatibility / no rebaseline

`ScenarioReferenceArchive.HashLayout` strips the latch before every branch, explicitly including non-default prescription state. That branch also retains its pre-amendment v23 identity header; default-prescription worlds retain the existing v20/v21 layout policy. All v15–v18 compatibility paths strip the latch through `WithoutLaterFields`. Full save JSON still carries false/true. The seven identity APIs (`WorldHash`, Current, LegacyCompatible, V15–V18) are compared against the exact v23 sources on 80 fixture rows, including both geometries, non-default prescriptions/order/event provenance, rack state and old input versions. Output is byte-identical; no anchor/reference rebaseline.

Protected completion-rule normalized hash remains `8166775d17827112282a668f8d0859d6e31c6868fa4724c77c13dbf6d6f80648`. Completion/objectives/Reference resources/Rack geometry/work economy/scenes/packages/ProjectSettings are unchanged against the exact start.

## Offline evidence

Durable root: `/media/jer/Files/CCF/Verification/S1D-T04P-Persistence-20261029/`.

- `offline-final.log`: latch36 PASS, accepted T-03 table48 PASS; 16,384 rows equal old sequence/boundary; 80 v23/v24 all-hash rows identical; protected sources unchanged.
- `offline/final/mfm.log`: MFM-06 save/provenance172 PASS, original pre-v23 hash fixtures retained.
- `mfm07-offline.log`: MFM-07 approval125 PASS, repeated processes byte-identical.
- `n3-offline.log`: N3 88 PASS with unchanged normalized protected source hash.
- `unity-refs-final.log`: production plus focused changed runtime harnesses compile against the installed Unity 6000.6 assemblies, zero errors (322 pre-existing unused/unassigned-field warnings).
- The generic historical cloud tools type-check cannot compile this repository's mutually exclusive verification stubs together and uses older Unity APIs. Its failure is retained in `typecheck.log`; the focused installed-reference compile supersedes it for this change. This is not claimed as a Unity Editor import.

Two initial verification-fixture failures were corrected: old MFM AsV22 conversion hardcoded a v23 input header; the new pure review fixture placed trees outside the existing 12m half-length. Initial logs are retained. No production rule/threshold was changed to repair those fixtures.

## Unity qualification

Pending. Required exact-candidate sequence and commands are documented in `Tools/Verification/TeachingPersistence/README.md`. #147 released its lock normally at `2026-10-29T23:44:03Z`, restored U1 clean detached at c8cfae8, and reported its own successful handoff. Refreshed no-Editor/storage state before acquiring normal task lock `5a6a490584b54e92a30eb0bc601d1320` at `23:49:17Z`, owner `Codex-S1D-T04P`, exact detached candidate129d0ace. First compile PASS (62.99s, Editor0). Initial latch run proved the real false/true boundary, then hit a verification formatter null diagnostic (`CanLoad` succeeded with a null problem string). Repaired the formatter only at b9bf11aa; no assertion removed or production change. First lock released normally with no Editors. Second normal lock `99ad2c8ece674b40bd1507d50c8fbf8c` at 23:55:36Z pins b9bf11aa for the complete ordered gate chain, evidence under `unity-r2/`.

## Independent review / next gate

**PENDING, no verdict claimed.** Requested Manager assignment on #91 after publishing the exact candidate. AgentWorkflow's Reviewer rule says: “A provider-native helper or child of the implementing worker does not count as an independent reviewer for that parent task.” Therefore an implementation helper cannot satisfy this required gate. Manager must assign an independent cross-provider review of the exact candidate and runtime evidence before acceptance/integration.

The qualification success token is withheld until all required gates, including independent review, pass. #124/#126/#128 remain blocked on Manager acceptance/integration and exact repin. Dashboard publisher/ingestion/readback unavailable through this task's repository/tool surface; synchronization remains a Manager operational follow-up, no generated state edited.
