# S1-D T-04P — Execution Handoff

## Handoff identity

- **Handoff:** `Docs/Handoffs/S1-D-T04-Persistence-Prerequisite-20261029/EXECUTION-HANDOFF-20261029.md`
- **Label:** **LOCAL/CODE REQUIRED**
- **Claim:** **#143**
- **Parent:** #91 / #106
- **Purpose:** implement the minimum durable teaching-completion persistence required by D-066, then qualify it without broadening S1-D state.
- **Difficulty:** **High / 5 of 5** — only one persisted fact is allowed, but the change crosses save/versioning, teaching progression, hash compatibility and protected reference behavior.
- **Best-suited model/provider:** **local SOL/Codex high-reasoning save/validation lane**, or equivalent strong repository/Unity implementation model.

## Authority and release status

Browser repair #145 is complete. Independent Manager review #158 is **APPROVED/CLOSED**. Production implementation is therefore released for #143, subject to a fresh task-start ownership/schema check.

Authoritative inputs:

- `Docs/Handoffs/S1-D-T04-Persistence-Prerequisite-20261029/TASK-PACKET.md`
- `Docs/Handoffs/S1-D-T04-Persistence-Amendment-Repair-20261029/BROWSER-RESULT.md`
- `Docs/Handoffs/S1-D-T04-Persistence-Amendment-Repair-Manager-Review-20261029/BROWSER-RESULT.md`
- `specs/scenario-one-repeated-interventions-v1/`
- live `Docs/Project/coordination/active-tasks.md`

Exact implementation base:

`c8cfae8c4025cec6e960a050edf4bfde2a981de5`

That lineage is the qualified post-MFM-07 umbrella and carries `ForestSaveData.CurrentVersion = 23`.

## Exact lane

- **Branch:** `feature/s1-d-t04-teaching-persistence-v1`
- **Worktree:** `/media/jer/Videos/CCF-S1D-T04-Persistence`
- **Starting HEAD:** `c8cfae8c4025cec6e960a050edf4bfde2a981de5`
- **Claim state before edits:** change #143 to `[RUNNING]` with actual model/provider, branch, worktree and exact starting HEAD.

Before any production edit, refresh `active-tasks.md`, #91, #106, #143, #124, #126 and #128. Confirm:

1. no active save/schema/hash writer exists;
2. v23 is still current and v24 remains free;
3. the worktree is clean at the exact pinned start;
4. no overlapping owner has acquired any required production file.

If any of those differ, stop before editing and return to Manager for a repin. A claim does not grant ownership by itself.

## Product contract — do not broaden

Persist exactly **one boolean-like monotone latch** whose sole meaning is:

> the existing first guided rack teaching sample/review has historically completed at least once.

Allowed write:

- `false -> true` exactly once;
- only when the **unlatched/pre-amendment** rack-world teaching sequence genuinely crosses `response -> recruitment` in the current session;
- all earlier lesson guards must already pass;
- `ReleaseReviewed == true`;
- actual session `RackReviewComplete == true`;
- evaluating the pre-latch sequence would return `recruitment` rather than `response`.

Do **not** set the latch from `ScenarioOneTeachingCap.SampleHistoricallyComplete(...)` alone, planting, arbitrary/in-footprint felling, later review acknowledgement, current Crop Tree/world state, `PlayerPrefs`, load, save age/version or retrospective reconstruction.

Never reset `true -> false`.

Old or missing field means **false / historical completion not recorded**. No backfill.

Do not persist individual `[J]/[K]` decisions, inspected-tree or stand-portion detail, rack-review transcript/order, cycle IDs/keys, lists, counters, guide index, readiness state or broad management-cycle state.

## Shared T-03 / T-04 read contract

Expose one smallest read-only historical-completion API/property.

- **Before latch:** read false. T-03 retains accepted current-world/session safeguard behavior. T-04 retains its existing pre-completion guide behavior.
- **At boundary:** write true once.
- **After latch:** T-03 and T-04 consume the same durable historical fact. Later world drift or save/load must not unset it.

Do not absorb #124 curriculum replay into #143. #124 remains blocked until #143 is accepted/integrated and repinned.

## Save/schema/hash contract

If task-start inspection still confirms v23 is current and free, use **v24** as the next schema. If not, stop and return to Manager.

Full save serialization must round-trip the latch.

The latch is **teaching progression, not world identity**. It must be stripped/ignored from every world/reference/protected identity projection, including:

- `ScenarioReferenceArchive.WorldHash`;
- `CurrentWorldHash`;
- `LegacyCompatibleWorldHash`;
- every `HashLayout` branch;
- specifically the **non-default v23 prescription-state branch**;
- any minimal/protected world hash projection;
- any protected/reference hash comparison consuming those layouts.

No Reference Future v1 content, accepted anchor, fingerprint or protected hash may be rebaselined merely to make the change pass.

Preserve MFM-06/07 prescription/provenance round-trip behavior.

## Expected bounded implementation surface

Inspect exact files before editing. Likely bounded surface:

- `Assets/ForestPrototype/ForestSaveData.cs` — one field and version bump only if live v24 remains free;
- smallest Scenario One capture/restore path required to round-trip the latch;
- smallest teaching helper/write path needed to detect the exact pre-amendment `response -> recruitment` transition;
- one read-only historical-completion API/property;
- `ScenarioReferenceArchive` compatibility stripping required to exclude the field from all identity paths;
- only validation genuinely required by the one-field schema/version change;
- focused verification under `Tools/Verification/`.

Do not change completion rules/objectives, economy/ecology, Rack V2 geometry/lifecycle, MFM prescription semantics, S1-C history presentation, scenes/prefabs/packages/ProjectSettings or frozen Reference Future archive content.

## Verification — offline first

Run offline/static verification before requesting Unity. Prove at minimum:

- exact live schema/version selection and no collision;
- old/missing field => false;
- one-way false->true semantics with no reset path;
- write condition is equivalent to the old pre-latch `response -> recruitment` boundary including `ReleaseReviewed` and real session `RackReviewComplete`;
- T-03 helper alone cannot set the latch;
- repeated reads/refreshes do not write again;
- save -> load -> save preserves both false and true states;
- no `[J]/[K]`, inspection or session-review detail appears in save data;
- T-03 pre-latch behavior is unchanged;
- one shared post-latch read exists for T-03/T-04;
- `ScenarioOneCompletionRules` protected source/hash is unchanged;
- otherwise identical false-vs-true latch worlds produce identical world/reference/protected hashes;
- include a **non-default prescription-state world** in that hash comparison;
- MFM-06/07 save/provenance regressions remain green;
- no protected baseline or reference was rebaselined.

## Unity phase

Unity is machine-wide serialized. At current dispatch time, #129 is the current Unity qualification priority and #147 waits behind it. **Do not launch Unity simply because the offline phase finishes.** Refresh the actual machine lock/process/free-space state immediately before any Unity attempt.

When the lane is genuinely free, qualify the exact candidate:

1. import/compile exact candidate;
2. reach `response` with incomplete rack review and prove latch false;
3. satisfy Annual Review while `RackReviewComplete` remains false and prove latch false;
4. complete the actual rack review so old sequence crosses `response -> recruitment`; prove exactly one latch write;
5. save/load and prove true persists;
6. repeat refresh/load and prove no duplicate write/side effect;
7. mutate later Crop Tree count/loss, in-footprint felling and executed-share state where feasible; prove latch remains true;
8. load representative pre-amendment saves with false/default and no heuristic backfill;
9. rerun required save-hardening/reference/hash/MFM compatibility gates.

No Windows build.

## Required handoff on completion

Publish:

`Docs/Handoffs/S1-D-T04-Persistence-Prerequisite-20261029/HANDOFF.md`

It must include:

- actual model/provider;
- branch/worktree;
- exact start/end SHAs;
- actual schema version;
- changed files;
- latch field and shared read API;
- exact write hook;
- old-save behavior;
- offline verification results;
- Unity verification results;
- proof individual `[J]/[K]`/session detail is not serialized;
- all-hash/no-rebaseline evidence including non-default prescription state;
- independent implementation-review verdict.

At successful handoff, mark #143 `[HANDED-OFF]`, link the handoff and return:

`S1-D T-04P PERSISTENCE QUALIFIED — READY FOR MANAGER REVIEW`

## Stop conditions

Stop and return to Manager if:

- v23 is no longer current or v24 is occupied;
- another save/schema/hash writer is active;
- the exact start lineage changed without a Manager repin;
- one boolean latch proves materially insufficient;
- faithful implementation would require persisted `[J]/[K]`, review transcript or broader cycle state;
- the real `RackReviewComplete` boundary cannot be used without changing product meaning;
- excluding the latch from identity requires changing accepted world semantics rather than preserving them;
- a protected test/hash would require weakening or rebaselining to pass;
- required files overlap another active owner.

## Copy/paste dispatch

`Run claim #143 from Docs/Handoffs/S1-D-T04-Persistence-Prerequisite-20261029/EXECUTION-HANDOFF-20261029.md. Start exactly at c8cfae8c4025cec6e960a050edf4bfde2a981de5 on feature/s1-d-t04-teaching-persistence-v1 in /media/jer/Videos/CCF-S1D-T04-Persistence, after refreshing active-tasks and confirming v23/current plus no competing save/schema/hash writer. Implement exactly one monotone first-guided-rack teaching-completion latch, written false->true only when the unlatched pre-amendment teaching sequence genuinely crosses response->recruitment with ReleaseReviewed and actual session RackReviewComplete after all earlier guards. Old/missing=false; no backfill; no per-cycle state or J/K/session detail. Expose one shared historical read for T-03/T-04 after the latch while preserving T-03 pre-latch semantics. Round-trip the latch in saves but exclude it from every world/reference/protected identity/hash path, including non-default prescription-state HashLayout, with no rebaseline. Run offline verification first. Do not launch Unity until the machine lane is genuinely free; then run the required Unity/save/hash/MFM gates. Publish HANDOFF.md and return S1-D T-04P PERSISTENCE QUALIFIED — READY FOR MANAGER REVIEW. No Windows build.`

## User action

Copy/paste the dispatch line above into the local T3 Code chat assigned to #143. No other user decision is required.