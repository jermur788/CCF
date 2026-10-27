# RACK-V2-04 — Map planning mode: blocked at the required API boundary

**Exit: `CROSS-LANE API REQUIRED`. Recommendation: `NEEDS CHANGES` for RACK-V2-04 only.**

The packet's explicit suggested-layout stop condition applies. No production code was changed. The Plan Racks mode and its suggested layer are **not implemented or qualified** by this handoff.

## Execution identity and claim

- Worker: T3 Code through the Codex harness; `gpt-6.1-sol`, xhigh reasoning, configured Codex provider; Primary role.
- Starting current GitHub main: `fe8199441b5ab69c7644797b3e3792cc3c3674eb`; verified descendant of core integration `f23549b18b17b3542ec8f633d7309b11020990d4`.
- Dedicated clean worktree: `/media/jer/ZX20/CCF-RackV2-04`.
- Branch: `feature/scenario-one-rack-v2-map-planning-mode-v1`.
- Canonical-context commit on that main lineage: `eb218eab8b8e200a81cc99bfef7e8906bc21a88d`. The approved feature spec, packet and live coordination were also inspected at starting main; the coordination file is outside the context snapshot.
- Final source-inspected HEAD: `fe8199441b5ab69c7644797b3e3792cc3c3674eb`. No Unity-tested HEAD exists for this task. The handoff commit changes this Markdown file only.
- [Claim #15](https://github.com/jermur788/CCF/issues/15) was changed from `[DISPATCHED]` to `[RUNNING]` and read back with the actual worker/model/provider, worktree, branch and starting SHA before any production edit. No production edit subsequently occurred.
- Live GitHub coordination was refreshed twice. No conflicting writer for Stand Map / `UI/Map/*` was recorded. S1-C owns `AnnualReviewView.cs` and narrow `ScenarioOneUiRoot.cs`; neither was touched. The dirty local Primary coordination copy was not used to override GitHub state.

## Why the suggested-layout prerequisite fails

The [packet](TASK-PACKET.md) requires geometry from qualified V2/headland/layout foundations and says: “STOP with `CROSS-LANE API REQUIRED`” if no defensible suggested-layout API is exposed without duplicating authoritative geometry.

The inspected production APIs do not expose that layout:

- [ScenarioOneManager.RackPlan.cs](../../../Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.RackPlan.cs): `RackPlan` and `ConfirmedRacks` describe the persisted **player** rack records through `DescribeRacks()`. A fresh V2 world clears the collection and begins with zero racks. These snapshots cannot supply five suggestions. `AddRack` allocates an ID and inserts a player record; invoking it for display would violate the packet's no-mutation/no-counting rule.
- [ScenarioOneRackLayout.cs](../../../Assets/ForestPrototype/ScenarioOne/ScenarioOneRackLayout.cs): `Centres()` still derives the older every-eighth-row authored layout (`RowsBetween = 7`). Its applicability check tests Enlarged80 geometry, not the V2 workflow identity. It does not expose the qualified headland entrance/far-end segments.
- [RackPlanner.cs](../../../Assets/ForestPrototype/Racks/RackPlanner.cs): `LayoutLines` comes from that older `Centres()` result. `LayoutStatus()` clips its lines boundary-to-boundary. `ProposeBetween()` requires callers to supply endpoints; it does not choose the qualified five-rack V2 layout.
- [ScenarioOneHeadlandGeometry.cs](../../../Assets/ForestPrototype/ScenarioOne/ScenarioOneHeadlandGeometry.cs): exposes infrastructure constants and `ContainsStemCentre()`, but no suggested rack layout. The constants alone do not specify the five routes.
- [RackLayoutTracker.cs](../../../Assets/ForestPrototype/UI/RackLayoutTracker.cs): explicitly keeps `Lines` as the older authored layout for map presentation while V2 progress reads player records. [StandMapSource.ReadRacks](../../../Assets/ForestPrototype/UI/Map/StandMapSource.cs) consumes those lines and legacy session rack groups; it does not supply a hidden V2 suggestion API.

The distinction is explicit in [RackV2CombinedVerification.cs](../../../Tools/Verification/RackV2Combined/RackV2CombinedVerification.cs):

- `AddQualifiedHeadlandRacks()` uses the qualified entrance-tree fixture IDs `PO0203`, `PO1204`, `PO2003`, `PO2803`, `PO3803`, their live Z positions, the headland X and the stand extent. Its source comment distinguishes this fixture from the older authored layout. It then calls `manager.AddRack(...)`, creating authoritative player racks.
- The misleadingly named `AddSuggestedRacks()` is explicitly documented there as **legacy authored lines**, used only for an informational coupling probe. It also calls `AddRack(...)`.

Those are verification helpers, not read-only production APIs. Copying their fixture IDs or constructing another layout inside the UI would duplicate geometry policy. Reusing the legacy map lines would substitute a different layout. Neither satisfies the packet.

## Narrow API needed before resuming

Have the Manager assign the rack/headland foundation owner a bounded read-only API, for example `GetSuggestedRackLayout()`; the name is a proposal, not a new product requirement. It must:

1. Return the qualified five suggested centreline segments, with entrance, far endpoint and corridor width, from the shared V2 layout/headland foundation. Geometry selection stays outside the UI; no copied fixture IDs, offsets or row-count policy in map code.
2. Apply to the applicable V2 world and return no V2 suggestions for Legacy40, Reference Future or pre-V2 continuations.
3. Return detached read-only geometry, without adding player rack records, allocating rack IDs, changing confirmation/marks, creating work orders or altering save data.
4. Supply focused evidence that repeated reads preserve player rack counts, IDs, confirmation, work orders and saved state, and that its fresh-world geometry matches the qualified headland layout.

This handoff does not implement that API, decide OPEN-3, or reopen frozen foundation ownership. Once it is integrated/available on the assigned base, redispatch RACK-V2-04 with the same UI scope.

## Verification and evidence

External evidence: `/media/jer/Files/CCF/Verification/RackV2-04-20261027/`.

- `preflight.json`: exact source identity, inspected API paths, claim-before-edit confirmation and zero production edits.
- `claim-running.md` and `claim-running-readback.json`: the issue update and successful readback.
- `live-coordination.md`: fetched authoritative GitHub coordination snapshot.
- `unity-lock-observed.json`: read-only machine-wide lock observation.

Executed source/preflight checks:

```bash
git fetch origin main
git merge-base --is-ancestor f23549b18b17b3542ec8f633d7309b11020990d4 fe8199441b5ab69c7644797b3e3792cc3c3674eb
git diff --exit-code fe8199441b5ab69c7644797b3e3792cc3c3674eb -- Assets Packages ProjectSettings Tools
git status --porcelain
gh issue view 15 --repo jermur788/CCF --json title,body,state,url
```

The lineage check and production-tree identity check passed; the worktree was clean before this handoff. Read-only source inspection established the call paths described above. No harness was run or modified.

Unity lock was observed with `scripts/unity-lock status --json` from `/media/jer/ZX20/CCF-Ops`. It was held by `S1-C-IMPL-02`, owner `T3 Code / Claude Sonnet 5.5 S1-C-IMPL-02`, lock ID `f471c04ed595441ea540b3ef18fb8a49`, U1 candidate `0d3ac9208d4a36813ae0bef3d9064157a69511e6`. This task acquired/released no lock, launched no Editor, and left S1-C's priority intact. The receipt is an observation, not a claim about later lock state.

**Not run:** compilation/import, Tier-1 functional smoke, navigation/tool interaction, suggestion-display non-mutation runtime checks, or rendered review. There are no RACK-V2-04 captures at 1280×720 or 1920×1080. Those gates remain required after the API prerequisite and implementation; no prior foundation screenshots substitute for them. Toolbar/tab separation is inspected as an existing defect, not fixed or verified here.

## Change and preservation boundary

- Files changed: `Docs/Handoffs/RACK-V2-04/HANDOFF.md` only.
- Production/serialized Unity files, assets, packages, settings, save/schema, ecology, economy, player rack state, OPEN-3 and protected reference/hash definitions: unchanged. No runtime anchor was rerun or rebaselined.
- No player-facing result is delivered. Spec conformance for RACK-V2-04: **NOT VERIFIED / BLOCKED**.
- Primary's pre-existing modified `AGENTS.md` and `Docs/Project/coordination/active-tasks.md`, plus untracked `Docs/Operations/StorageArchitecture.md` and `Docs/ScenarioTwoPreProductionAudit.md`, remain untouched. Other workers' lanes and files remain untouched.
- No integration, whole-feature qualification, acceptance, demo/MFM readiness or Windows build is claimed. No subagents were used.

## Manager handoff and operational follow-up

Recommendation: **NEEDS CHANGES** — provide the read-only qualified suggested-layout API, then resume this bounded UI task. This is an explicit packet stop, not a proposed scope expansion or a request to weaken verification.

The handoff is published on the task branch at `Docs/Handoffs/RACK-V2-04/HANDOFF.md`; its exact publication commit is recorded in claim #15 and is obtainable with `git log -1 --format=%H -- Docs/Handoffs/RACK-V2-04/HANDOFF.md`. The claim is handed off/closed for this stopped execution; the feature task remains blocked, not complete. No worker edit to the Manager-owned live-lock file was made.

**DASHBOARD SYNC: NOT DONE / NOT AVAILABLE. PUBLISHED DASHBOARD REFLECTS THIS TASK: NOT VERIFIED.** The current live coordination already records dashboard unavailability. The Manager must record the blocked RACK-V2-04 state in authoritative coordination and synchronize the dashboard when publication is available; no generated dashboard state was hand-edited.
