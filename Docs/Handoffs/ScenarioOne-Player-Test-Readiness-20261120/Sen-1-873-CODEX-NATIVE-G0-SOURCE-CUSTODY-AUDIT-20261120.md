# Sen-1 #873 — Codex native G0 source/custody audit

Status: G0 PARTIAL READ-ONLY AUDIT COMPLETE; SOURCE/UNITY START HELD. No integrated candidate, Unity qualification, or player readiness claimed.

Actor: OpenAI gpt-6.1-sol, medium reasoning, T3 Code through Codex harness, native host jer-laptop. Internal session ID and remaining provider quota/capacity-to-completion are not exposed. No successor or child workers launched.

Read binding successor handoff dd586f20 and full parent packet 08b04e53, refreshed main/live coordination at 624aff40d7df45b941e6d729aa7c68b3af2d587f, and inspected #873 issue and both comments, #872/#425 issue bodies. Current canonical-context candidate observed by path-history scan: 7e24ba0eee05b9af1055c88e423e629fe409bbf1; this is not an assigned implementation context lock. Full canonical/spec/signature and source-custody admission remains required before implementation.

## Verified source findings

- Gameplay: 8cb22de077b93b4c7af076e19936f3170d6ab48b.
- Consolidated visual: 5cbc26a4073ffa60db0a3c442e237eaa473ad3ff.
- Shared blank starter: 511032f0124e28fdf61a8d1b676c81330f7fa5c0.
- Gameplay/visual merge base: 0709d05a79756d3b72ccbd0452bd0252a75c3804; gameplay-only 1 commit / visual-only 16 commits.
- Relative to that common base, production paths under Assets/ProjectSettings/Packages changed by gameplay: 33; by visuals: 74; intersection EMPTY. A bounded combination is plausible. This is path-level evidence, not a performed merge or proof of serialized/runtime compatibility. Comparing branch tips directly misleadingly shows gameplay files absent on the visual branch; those absences must not be used as deletion instructions.
- Both scene versions retain Compact30World and Trees names. Visual scene is substantially revised (7,474 insertions / 20,691 deletions between tips); exact prefab/tree roster/GUID, colliders, path/G1 and bootstrap compatibility require actual review and native gates.
- Gameplay EditorBuildSettings enables only Assets/Scenes/ForestTest.unity. Compact30Bootstrap gates on exact scene name ScenarioOneCompact30. This corroborates the owner-reported entry defect and explains why a harness opening the scene is insufficient.
- Shared #857 and gameplay both change ForestPlayer.cs and UI/{AnnualReviewView,ScenarioOneUiRoot,StandMapView,WalkingHudView,WorkPlanView}. Do not wholesale merge #857. Selective adoption requires source-level necessity/compatibility review; no seam is declared approved by this audit. #857 does not contain the populated Compact scene.

## Physical and preservation findings

- Native Editor executable exists at /media/jer/ZX20/Unity/6000.6.0f1/Editor/Unity; not launched or version-executed in this audit.
- Mounted ZX20: /dev/sdc2, exfat, UUID 18D6-896E. Files: /dev/sdb4, ext4, UUID 2283b135-bf37-42d6-921f-2fb861d0a8d4. Installed Ops is /media/jer/ZX20/CCF-Ops; lanes use persistent by-id storage identity.
- Deployed board has NO #873 source/writer registration. Live GitHub explicitly says READY/UNASSIGNED, no source grant or Unity transfer. These are real admission blockers.
- Direct read of deployed state/unity found guard/history but no current holder file; last recorded release was #862 at 2026-11-20T13:20:59.752039+00:00. Process inspection found Hub, licensing and Hub unity serve, no observed Editor executable. This observation is NOT an acquired lease or a lock-tool FREE certificate. No unity-lock command, release, expiry, board edit or force operation performed.
- Coordination anchor: /media/jer/Files/CCF-Primary, ops/workflow-foundation-v1, a42eaadc42bda6d474365f39b67a78a9b14bb043. Preserved modified AGENTS.md, StorageArchitecture.md, active-tasks.md; three untracked November 18 operations notes and coordination/archive/.
- Existing gameplay lane at exact 8cb22de0 has 10 modified tracked files: .vscode/settings.json, seven SectionFive materials, Packages/manifest.json and packages-lock.json. SDK linux-arm64/linux-x86_64 and Linux toolchain/sysroot differ from committed 1.1.0 to local 1.1.1. No attribution or approval inferred; not cleaned or incorporated. Prior 71/71 remains historical reported evidence, not freshly reproduced exact dependency qualification.
- Visual floor and #857 lanes observed clean at the exact listed commits. Their existing ownership does not transfer.

## Proposed smallest next scope — approval request, not grant

PACKET ONLY is plausible if the work only combines the existing gameplay delta and consolidated visual delta, then adds a clearly labelled Editor scene-opening helper and owner instructions. Start from exact gameplay 8cb22de0 and carry the visual changes relative to the common base; do not replace the populated scene with blank starter or change historical defaults. Actual merge and source/scene review must precede any correctness claim.

Proposed source envelope: existing 33 gameplay and 74 visual production paths as audited, a new narrowly named Editor-only Compact playtest entry with its .meta, targeted verification and one result handoff. This is NOT blanket permission to rewrite those paths. Freeze exact path inventory and helper name in the grant. Shared #857 changes are optional and excluded until separately justified by a concrete player blocker. Exclude new persistence/router/profile semantics, Packages, historical scenes/default build settings, frozen Reference, old saves, Ops tooling/state, unrelated assets, main integration and Windows builds.

Required next gate: #872/#425 and physical custodian assign this actual actor one new unique integration branch/worktree, exact base/context and path allowlist, and confirm any overlapping source reservations. Then acquire fresh lawful serial Unity ownership for the bounded qualification phase. No self-grant from available tools or absence of a holder.

## Verification and limitations

Actually run: Git graph/path diffs, committed settings/bootstrap reads, selected serialized root checks, actual lane HEAD/status, local package diff, deployed board/config/state reads, mount and process inspection. No source edits, merge, compile/import, runtime/GUI, save access, legacy/reference/determinism or build checks. No production .meta touched. Full G0 admission is incomplete, including exact source/spec/signature review and definitive lock/custody check.

Expected later owner route: approved new candidate worktree, Unity 6000.6.0f1 via authenticated normal-UID Hub, explicitly open Assets/ForestPrototype/Scenes/ScenarioOneCompact30.unity, then Play. This route is proposed and untested here; no current project copy is presented as player-ready.

Dashboard/Drive publication NOT VERIFIED; Manager synchronization outstanding. Local evidence report: /media/jer/Files/CCF/Verification/Sen1-873-Codex-G0-20261120/HANDOFF.md. GitHub publication contains this report only; main/coordination/source are unchanged by publication.
