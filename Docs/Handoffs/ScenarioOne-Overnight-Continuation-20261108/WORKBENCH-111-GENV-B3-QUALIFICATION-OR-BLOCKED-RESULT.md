# CCF #337 — BLOCKED at Phase 0 / HANDED-OFF to Manager #200

Date: 2026-11-08. Actual local worker: T3 Code / Codex / gpt-6.1-sol; reasoning effort UNKNOWN. One local owner, no delegated workers. Task class PACKET ONLY.

## Result and exact blocker

**BLOCKED: the deployed exclusive Unity lease cannot admit the protected dirty registered lane. No Unity was run.** Source approval of #335/#336 stands; this is an operational admission failure, not a package-source failure.

Read-only inspection of `/media/jer/ZX20/CCF-Ops/scripts/unity-lock`, acquisition code (clean-lane check), shows an unconditional `git status --porcelain` clean-lane requirement followed by `ValueError('Unity lane is dirty; inspect before execution')`. The actual lane has exactly eight genuine tracked working-byte modifications. Even a successful package fast-forward would retain the protected five Materials and `.vscode/settings.json` modifications, so the deployed lease remains unavailable for this environment. No acquisition was attempted, no refusal bypassed, no lock records rewritten, and no lease was claimed. Current lock status is FREE with zero Unity Editor PIDs. Availability of an empty lock is distinct from eligibility to acquire it.

The packet requires this lease before any registered-lane transition, forbids cleaning protected files, and orders BLOCKED on any admission failure. Accordingly Phase 0 stopped before RUNNING, verifier edits, transition rehearsal, actual transition or Unity. A disposable rehearsal was **NOT RUN** because Phase 0 failed; feasibility of the Git transition is **NOT VERIFIED**. Do not interpret ancestry success as a successful rehearsal.

## Identity, preservation and storage

Live GitHub Manager-only `active-tasks.md` and #337 plus comments were freshly read. Latest designation is #337 READY / NOT RUNNING, conditional preparation only; runtime lane NOT OWNED/admitted. #335/#336 historical lanes were not modified. Manager #200 remains sole coordination writer. No RUNNING receipt posted.

Registered lane: `/media/jer/ZX20/CCF-WorkbenchTier2-Registered-20261106`, registration `WorkbenchTier2Reg287`; branch `feature/scenario-one-workbench-retirement-20261105`; historical HEAD remains `4d95e45245d660e24b91e26754d9c57949bf3384`; common store `/media/jer/Files/CCF/Repository.git`. Candidate `46bc1fd618ae5b214d7de296a03ac7efec27cb7c` is an ancestry descendant (merge-base exit 0), not admitted. `git --no-optional-locks status` and diff returned 0: exactly eight modified paths, no staged or untracked files. Ignored state includes Library, Logs, UserSettings and generated solution/project files; retained untouched. No inode/stat noise was treated as content drift and no index refresh/surgery was performed.

All eight original working files were copied with metadata into the new durable before-image. Their bytes match #318 `dirty-backup/` **8/8**; actual historical HEAD blobs match `head-versions/` **8/8**; original backup manifest validates **16/16** and manifest SHA256 remains `9fb805968899cb6cd369a6d627ef1c01a7afece2368ff9d2093db1bfa9961f74`. File sizes/modes and full hashes are in protected-before.json. Final working-byte checks remain **8/8 exact**. The original backup chain is untouched.

ZX20 free: **23,216,128,000 bytes / 21.622 GiB** at admission; above the 20 GiB STOP threshold by 1.622 GiB, below the 27 GiB cold-entry minimum. 15 GiB hard floor unchanged. No cold import, Library move or cleanup. Disk STOP monitor and licence validation were **NOT VERIFIED / NOT STARTED** after the decisive lease blocker; no claim of complete Phase-0 admission. Package-cache/runtime identity and rendered material state were not qualified.

## Publication-only preparation lane

Before creation, the prescribed branch and path were absent. Created only to publish this mandatory BLOCKED handoff: `verify/workbench-111-env-b3-20261108-v1` at `/media/jer/Files/CCF/Operations/CCF-Workbench111EnvPrep-20261108`, starting exact candidate `46bc1fd618ae5b214d7de296a03ac7efec27cb7c`. This is not Phase-1 execution or runtime admission. Only this result document changes; no instrumentation, Assets, packages, materials, ProjectSettings or `.vscode` changes. No source integration, PR or build.

## Verification and remaining gates

- G-env: **NOT RUN / BLOCKED**.
- B3 witness: **INCOMPLETE / NOT RUN**; two plain and one authored-pose run not executed; Manager B3 verdict remains UNRESOLVED, no B3 PASS claimed.
- Workbench C–G: **NOT RUN**. Integrated A–O: **0/15 RUN**.
- Original registered HEAD/branch and all protected bytes unchanged; no save/Legacy40/Reference Future mutation, restore, stash, reset, forced checkout or deletion.
- No lease acquired, so none to release. Final normal status query confirms FREE / no Editor. Sole heavy Unity ownership was never established and no heavy Unity process started.

## Manager decision and safe continuation proposal

Manager #200 must resolve the operational incompatibility between the required exclusive lease and the deliberately retained diagnostic drift in a separately reviewed packet. No clean/ignore/force workaround is authorised here. Any future mechanism must explicitly retain fail-closed provenance, prove exact allowed eight-path hashes and ownership, preserve unrelated drift rejection and R1 failure, and undergo review before deployment. This report proposes no implementation and changes no lease tooling. Once a legitimate method exists, repeat live Phase 0, licence/cache/disk-monitor checks and the disposable Git rehearsal before any original-lane operation. No safe transition method has yet been proven.

Durable hash-indexed evidence: `/media/jer/Files/CCF/Verification/CCF-Workbench111GenvB3-20261108/` (admission.json, live ownership snapshot, binding packet, deployed lock source/config, before-image, protected before/after, final lock/disk). Original coordination-anchor unrelated `AGENTS.md` and `Docs/Operations/StorageArchitecture.md` edits preserved.

DASHBOARD SYNC: NOT AVAILABLE / NOT VERIFIED. PUBLISHED DASHBOARD REFLECTS THIS TASK: NO verified readback. Worker publishes result and #337 receipt only; Manager #200 owns active-tasks and Overall-Dev latest update. Manager must reconcile these and verify downstream audit/publication; a Git push is not complete dashboard sync.

**HANDED-OFF. Stop for Manager #200; not accepted, integrated or Unity-qualified.**

## Eight protected working-byte hashes

| Path | SHA256 |
|---|---|
| `.vscode/settings.json` | `61d36e47afacc0f0bde6987fcf5a49ac6ce18bd514085c8754ce0ba72c29e4a6` |
| `Assets/ForestPrototype/Art/SectionFive/Bilberry_Type_Cover_01/Materials/Bilberry_Leaves.mat` | `106f237bf5d8bcbb3d05cfc4c3eae57e6e90eaee729114bde15485a4cc81d3fd` |
| `Assets/ForestPrototype/Art/SectionFive/Bilberry_Type_Cover_01/Materials/Bilberry_Stems.mat` | `1fd084251b82cc78b44e9458aed2d031c91e97ccc6df7d069b5c317f6554906f` |
| `Assets/ForestPrototype/Art/SectionFive/Woodland_Forbs_01/Materials/Forb_Flower_Centre.mat` | `39385672532b45322cee228e43886fc6006814e44f466ca228e1d4f6c8483774` |
| `Assets/ForestPrototype/Art/SectionFive/Woodland_Forbs_01/Materials/Forb_Leaves.mat` | `e2d8889265191156241a16516c28d30255be9ffaa127dbcfc4f2da920fb17a41` |
| `Assets/ForestPrototype/Art/SectionFive/Woodland_Forbs_01/Materials/Forb_Stems.mat` | `b32b7b5f507c4f1dc049a08d97ca48beea36e7027eed0eeb6afdb41f2eb4dcd3` |
| `Packages/manifest.json` | `f2ff5f95f9e93716947391a8818dbe642d81f0b49068516441a58def966c2c1c` |
| `Packages/packages-lock.json` | `f43d9894af3c6dc054e86e98335dbd248a9fb97527b49c7b5952d96e037e52b1` |
