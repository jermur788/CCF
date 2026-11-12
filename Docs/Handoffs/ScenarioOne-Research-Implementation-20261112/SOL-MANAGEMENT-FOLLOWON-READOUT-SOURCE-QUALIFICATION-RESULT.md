# #406 — Management follow-on readout source qualification

**Worker assessment: SOURCE QUALIFIED OFFLINE — ready for Manager review.** This is not Unity qualification, feature acceptance or integration. Task class PACKET ONLY; T3 Code / Codex GPT-6.1 Sol Medium, configured Codex provider, no delegation.

## Source and authority

Original Manager #406 packet and its physical-board reconciliation amendment govern this work. Freshly fetched browser source `feature/scenario-one-management-followon-browser-v1-20261112` was pinned at **2b1de2c81016a874390c183350ea20eef807c01a**, unchanged on remote recheck. Manager docs branch was never used as production base. Source context commit: **63044dfbbeb869996ceecec8a47619dcba048b0b**. Qualification branch: `qualify/scenario-one-management-followon-readout-20261112-v1`. New source-only worktree: `/media/jer/Files/CCF/Worktrees/CCF-ManagementFollowonReadout-Qual`. Implementation/tests/evidence commit: **58fb9d4716b41ca2504e5e399a17ae23e067b996**; this report is the subsequent documentation commit.

## Ownership reconciliation — PASS

Read GitHub main's explicit Manager #406 assignment, local designated coordination, physical Ops board, actual worktree inventory and claims #402/#408/#382 before writes. Files mounted ext4, 175 GiB free; requested branch/path absent. Added only the #406 physical board row under the amendment; no tooling requirement to modify stale local coordination, so it was preserved. GitHub main remains assignment authority. Native `/media/jer/ZX20/CCF-Ops/scripts/packet-lint --board /media/jer/ZX20/CCF-Ops/board/active.md` exited **0**, `PASS: board schema and duplicate checks (21 live tasks)`, before work and at handoff. New clean worktree created at exact pinned source, then #406 marked RUNNING. The local legacy `/home/jer/CCF/.git` store is absent; existing canonical registered worktrees were inventoried, none reused.

Before/after physical board copies and preservation proof live at `/media/jer/Files/CCF/Verification/ManagementFollowonReadout406-20261112/`. Removing the inserted row restored every original byte; hashes are [recorded in evidence](../../../Tools/Verification/ManagementFollowonReadout/Evidence406/board-preservation.txt). At handoff only that same row changed to DONE-PENDING-LEDGER, no active writer/lane assignment. Every other row remained byte-identical across each update, including Fable's research lane. Primary's unrelated dirty `AGENTS.md` and `Docs/Operations/StorageArchitecture.md` preserved. No Fable worktree, personal lane, UI or save touched. No Unity lease acquired or Editor launched by this worker.

## Reproduced defects and bounded repairs

Original helper first passed **37** assertions. Three independent negative-case processes against the unchanged helper then failed their expected assertions (exit **-6**, SIGABRT from unhandled assertion exception):

1. `WorkResolved` ID colliding with `OrderCreated` in complete `Events` was incorrectly trusted. The guard now also checks global IDs across complete supplied event streams, separately from work projections so a projection is not double-counted. Both OrderCreated and YearAdvanced collisions are fixtures.
2. Matched order/type/status/year/prescription with different target tree was incorrectly trusted. The link now requires equal recorded target IDs; tree work needs nonblank IDs on both sides. Available conflicting species reject linkage. Missing old tree identities remain unverified/inconsistent and cannot create executed coverage; no identity is fabricated.
3. Successful fell/salvage without an annual report or settlement incorrectly set `GroupedHarvestMoney`. That existing helper flag now requires both `Report` and `HarvestSettlement`; it exposes no per-order financial allocation. Positive fixtures explicitly supply both records.

Only production file changed: `Assets/ForestPrototype/ScenarioOne/ScenarioOneManagementFollowonReadout.cs`. Other changes are confined to `Tools/Verification/ManagementFollowonReadout/**` and this report. The original 37 assertions remain, with positive fixture target and settlement data made explicit. The adapter now mirrors the additional real production signatures; it never enters Assets. No producer, manager, saved field, package, scene, prefab, meta, UI, reference or DRAFT spec changed. Exact implementation diff is the qualification branch versus pinned source; mutations retain their exact patches in evidence.

## Executed gates

Commands ran from the new worktree; durable external output root was `/media/jer/Files/CCF/Verification/ManagementFollowonReadout406-20261112`. [Exact command arrays, exits and stdout](../../../Tools/Verification/ManagementFollowonReadout/Evidence406/commands.json) and [mutation output](../../../Tools/Verification/ManagementFollowonReadout/Evidence406/mutations.log) are committed.

| Gate | Command / observation | Result |
|---|---|---|
| A: original adapter compile/run | `python3 -I Tools/Verification/ManagementFollowonReadout/run_offline.py` before repair | PASS 37 assertions |
| A: baseline real production binding | `python3 -I Tools/Verification/ScenarioOneNonUi/compile_runtime_sources.py --src <new-worktree> --out <external>/typecheck-baseline` | wrapper 0, reported compiler 0; 190 sources, 116 references, 0 errors |
| B: reproduced original defects | three direct `dotnet .../ManagementReadoutCheck.dll global`, `target`, `harvest` runs | Expected FAIL, each exit -6, distinct assertion recorded |
| A/B: final real helper + test adapter | `python3 -I Tools/Verification/ManagementFollowonReadout/run_offline.py` | exit 0; PASS **53** assertions |
| A: final production binding | same production compiler command, `--out <external>/typecheck-production-final` | wrapper 0, compiler 0; 190 sources, 116 references, 0 errors |
| A: real production + fixture binding | same compiler command, `--out <external>/typecheck-final --extra <new-worktree>/Tools/Verification/ManagementFollowonReadout/ManagementFollowonReadoutVerification.cs` | wrapper 0, compiler 0; **191** sources, 116 references, 0 errors; no adapter included |
| C: bounded guard mutations | `python3 -I Tools/Verification/ManagementFollowonReadout/run_mutations.py --out <external>/mutations` | exit 0; both copied mutants compiled (0), then failed expected global/target assertions (-6 each) |
| Scope/diff | `git diff --check`; changed-path inspection | PASS, permitted paths only |
| Unity/import/runtime/render/save/load/Player/reference hashes | prohibited/out of scope | **NOT RUN** |

The existing production compiler wrapper itself returns 0 even on compilation errors, so the reported underlying compiler exit and error count were inspected; both truly show zero. Managed references: `/home/jer/ccf-verify-player/CCFVerify_Data/Managed` (existing Player assemblies, read-only); [reference SHA-256 manifest](../../../Tools/Verification/ManagementFollowonReadout/Evidence406/managed-reference-sha256.json) retained. This checks real production type bindings, including the fixture, but does not execute Unity's compiler/import, source generators, Editor assemblies, graphics or gameplay.

Tools actually used: .NET SDK **10.0.114**, MSBuild **18.0.36**, net8 target runtime **8.0.33** (10.0.14 also installed); Python **3.12.3**, Git **2.43.0**, GitHub CLI **2.45.0**. No packages or external dependencies added to the game.

## Coverage and compatibility boundaries

Retained checks cover duplicate completed years and resolved event/order IDs; missing/no-work years; exact 5-to-9-year gap; copied source scalars; natural occupied-cell/density versus planted-cell/exact-juvenile metrics; missing/duplicate species; old no-prescription records; failed/pending/orphan work; cancellation; repeat reads and input preservation. Rack-owned access felling remains excluded from substantive treatment evidence. Verified non-rack prescription felling remains **candidate only**, with no achievement/completion decision. Missing species stays NotRecorded rather than inferred zero; no historical per-rack ecology or causality is invented.

**Genuine old-save fixture: NO.** Legacy coverage uses synthetic absent-target/species/no-prescription records only. No save file read, written or loaded; no evidence of actual old-world runtime compatibility is claimed. Conservatively rejecting an old record whose tree identity is absent changes only this new readout's trust classification, not the stored record or simulation. Source-only qualification does not authorise larger D-073/D-074/D-076 DRAFT features or UI wiring.

## Handoff and remaining gates

Ready for independent Manager G2/G3 review: **Yes, offline scope only**. Manager must assess repairs/evidence, select any future Unity/UI/MFM gates and authorise integration separately. No main merge, PR, frozen-reference rebaseline, Player/Windows build or gameplay acceptance performed. Worktree retained for review; source writer finished.

**Dashboard sync: NOT AVAILABLE / published dashboard reflects task: NOT VERIFIED.** Physical board lint passed and GitHub claim/result publication supplies the operational handoff. Current deployed Ops scripts expose no dashboard publisher; no dashboard audit advancement/render readback was verified and no generated state edited. Outstanding operational follow-up: Manager updates its authoritative GitHub coordination entry with this return, runs/allows ingestion, confirms automatic audit snapshot advancement and published dashboard readback. Drive mirrors likewise unverified. This explicit synchronization limitation does not claim successful dashboard closure.

Publish/read back this report on the qualification branch, then mark #406 HANDED-OFF and close the execution claim. Closure is handoff, not acceptance or integration.
