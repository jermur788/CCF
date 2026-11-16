# Sen-1 #642 — Shared-save v24/raw-origin verification-fixture repair result

**2026-11-16 — IMPLEMENTED ON ISOLATED FIXTURE BRANCH; scoped offline checks PASS. READY FOR MANAGER / INDEPENDENT FIXTURE REVIEW. NOT UNITY-VERIFIED, NOT INTEGRATED into main or the shared-save branch.**

This completed result supersedes the earlier ownership-blocked report for current task status. The earlier blocked documentation branch remains unchanged historical evidence; no custody or work is backdated.

## Actual worker, authority and pre-write registration

- Worker: OpenAI **gpt-6.1-sol High**, Codex harness through T3 Code, configured Codex provider, Linux host `jer-laptop`; same existing #642 worker. No delegation or additional worker/reviewer.
- Complete [binding Manager packet](https://github.com/jermur788/CCF/blob/docs/sen-1-640-638-verdict-642-fixture-packet-20261116-v1/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261116/Sen-1-640-MANAGER-638-APPROVAL-AND-642-FIXTURE-REPAIR-PACKET-20261116.md) followed. [Actual Manager custody grant](https://github.com/jermur788/CCF/issues/642#issuecomment-6928221954) reserves ONLY the two fixture paths below for this existing worker. #640 is the current sole Scenario One main-board writer following explicit prospective transfer.
- [Actual RUNNING registration](https://github.com/jermur788/CCF/issues/642#issuecomment-6928353845), GitHub timestamp **2026-11-16T20:28:00Z**, preceded fixture edits.
- Fresh initial main ref `eaa245c910f4dc22a523f1735f61b496ede3e328`, coordination blob `7bba5ced70a6b9e8b940a1dbdfdbc6f67171b01f`. The leading #642 exact-custody entry was rechecked before committing; no conflicting current local worker/claim was found. Physical Ops mirroring is explicitly separate from this authorised offline source reservation; no Ops edit or Unity lease is claimed.
- New actual clean worktree: `/media/jer/Files/CCF/Operations/CCF-642-FixtureRepair`. Branch: **`verify/sen-1-642-save-v24-raw-origin-fixtures-20261116-v1`**. Starting HEAD **`410d582ecf63a9a92c9eca2e704c8ddf3f1c3d35`**, matching local and freshly queried remote #638 shared-save final. Context commit `62e39ec621e38a84030c08b35a171e297056c9ed`, with fresh task/ownership authority read separately.

## Candidate and exact change inventory

**Fixture-only commit: [`7ca95a0b2822c9c6cf9ac99cbbbfbe84afe4f07e`](https://github.com/jermur788/CCF/commit/7ca95a0b2822c9c6cf9ac99cbbbfbe84afe4f07e)**; parent exactly `410d582ecf63a9a92c9eca2e704c8ddf3f1c3d35`. Ordinary push/readback to `verify/sen-1-642-save-v24-raw-origin-fixtures-20261116-v1` succeeded. GitHub [base/candidate compare](https://github.com/jermur788/CCF/compare/410d582ecf63a9a92c9eca2e704c8ddf3f1c3d35...7ca95a0b2822c9c6cf9ac99cbbbfbe84afe4f07e) independently shows **ahead 1 / behind 0, exactly two modified fixture C# files, +94/-18**:

| Allowed fixture | Delta | What changed |
|---|---:|---|
| `Tools/Verification/StandGeometry/StandGeometryVerification.cs` | +78/-16 | v24 assertions and historical hash-layout teaching-field exclusion; raw admission helper and cause-specific negative checks; explicit capture/clone controls; genuine pre-v20 archive load in addition to labelled synthetic v19-layout coverage; existing geometry, cells, preview/light/waypoint assertions retained |
| `Tools/Verification/ScenarioReferenceVerification.cs` | +16/-2 | current authoring candidate JSON uses explicit raw admission; frozen Year-50 continuation uses existing authenticated original-resource API, with typed-clone refusal/no-mutation controls and original identity checks |

Only additional repository output is this packet-required Markdown, in a separate documentation-only commit on the same fixture branch. Final publication commit/byte readback/compare are recorded on #642 after publication. The old blocked-report branch is not merged or replayed.

**Production/serialized asset delta: ZERO.** Entire `Assets`, `Packages`, `ProjectSettings` tree remains unchanged from the pinned parent, including production controller/validation, schema, geometry, scene/prefab/material/.meta relationships and frozen Reference resources. No API change, test helper file, package or allowlist expansion was introduced.

## Original failures and why the replacement is legitimate

1. The original geometry fixture required captured/CurrentVersion **23**, JSON version 23 and rewritten version 23, while production is **24**. It also omitted the persisted v24 `firstGuidedRackReviewComplete` flag from its independent historical-layout projection even though production excludes that flag from world identities. The fixture now pins actual v24 and strips both true/false flag values when reconstructing the historical hash layout. No protected hash value was changed.
2. Existing positive paths called `LoadData` on parsed or cloned DTOs. Production intentionally accepts only an unchanged same-session capture on that typed route. Both parsing a capture and cloning a frozen milestone lose its source identity; their refusal never established that geometry or Reference loading was broken.
3. Current JSON and deliberately constructed geometry fixtures now enter existing `ForestSaveController.AdmittedSave.TryCreate` / prepare / apply. The helper reports admission/preparation/application failure; it does not register clones in the capture table, reflect into production private state or weaken admission. Current authoring-candidate JSON is explicitly identified as current captured data, not historical provenance.
4. Frozen continuation now calls existing `saves.TryPrepareReference(50, ...)`, which independently reads/authenticates the original embedded archive bytes before producing an immutable admission. Original v12/year50/Legacy40 identity is asserted before apply, and the existing two independent continuations and tree/species/position/stump/history comparisons remain intact.
5. `V19Layout(start)` remains a **synthetic layout regression fixture**, explicitly labelled as such. It is not claimed as a genuine old save. A separate Year-0 load through `TryPrepareReference(0, ...)` supplies genuine original v12/pre-v20 evidence, asserts its identity and saved tree roster, then restores the current world and original world hash. No historical fixture was fabricated by relabelling current bytes.

## Positive and negative coverage retained

- Known Legacy40/Enlarged80 sizes, 64/256-cell geometry, the 336-tree Legacy40 start, both load directions, cell-200 restoration, geometry event counts, understorey completeness and capture geometry assertions remain.
- Current raw JSON and unchanged same-session capture positives are explicit. Parsed DTO and changed-capture refusals now have explicit no-world-change assertions.
- Existing bad geometry values 2/7/-1/1000 remain; `int.MaxValue` added. Each raw case must return the **same specific validation problem** and leave the world snapshot unchanged, so typed-origin denial cannot masquerade as geometry-validation success. Existing direct typed invalid-input controls remain as separate provenance refusals.
- Pre-v20 claimed geometry, missing/non-integer geometry and raw/parsed disagreement checks remain. Missing/string geometry and foreign scenario identity also exercise actual raw admission and cause-specific refusal without apply.
- Reference compatibility, both preview directions, light/waypoint restoration, immutable archive integrity and frozen Year-50 continuation/determinism assertions remain. **The entire `ReferenceArchiveIntegrity` section, including all pinned values and its tampered-Year100 negative control, is byte-identical to baseline.**

## Actual offline verification

| Gate actually run | Actual result and applicability |
|---|---|
| Real-assembly Roslyn typecheck: production Assets + BOTH fixtures, player configuration | **exit 0, 0 errors, 0 warnings** against installed Unity 6000.6.0f1 managed/package assemblies; no Unity executable, import, ILPP or asmdef-equivalent claim |
| Same fixture typecheck, UNITY_EDITOR configuration | **exit 0, 0 errors, 0 warnings**; compiles Editor entry points and internal admission calls |
| Scoped extracted-boundary/fixture probe | **82 checks PASS, exit 0**; .NET 8, production validation/DTO/profile/context/hash/archive-loading/admission code, actual extracted fixture helper and unchanged Reference integrity gate, with explicit non-Unity adapters |
| Probe compiler | **exit 0, 0 errors, 2 expected unused-UI-field warnings** from the controller shell whose UI methods are omitted |
| Exact fixture scope / whitespace / protected bytes | **PASS**; only authorised two-file patch before this report, `git diff --check` clean, production/serialized tree unchanged, pinned integrity section byte-identical, original dirty anchor/Ops bytes preserved |
| GitHub fixture publication | **PASS**; remote HEAD equals fixture commit; independent compare exactly two fixture files +94/-18 |

The **82 checks** include schema-23 mismatch reproduction against v24; current raw and original-capture positives; clone/mutated-capture no-apply refusals; exact malformed geometry/identity reasons; synthetic v19 positive and forged-old geometry refusal; actual production-vs-fixture current/legacy hash-oracle equality for both teaching flag values; Model-2 Legacy40/E80 raw positives and cell-200/incomplete-understorey negatives; genuine original Year0/20/50/100 archive hash authentication and admission through actual `TryPrepareReference`; v12/year/geometry preservation; clone refusal for every milestone; missing-milestone and seed/identity refusals; and actual unchanged Reference integrity plus tampered-Year100 detection.

**Parser distinction:** the offline Newtonsoft DTO adapter throws `ArgumentException` for fractional geometry before production reaches its raw-token validator. The probe separately confirms the actual raw-token validator refuses that input and records the adapter-specific exception/no-apply observation. It does not count this as native Unity parser/admission qualification. The fixture's existing string non-integer control is retained and passed through the offline raw path. Native parser behavior is still a later gate.

**What is stubbed:** Unity `JsonUtility` is replaced by the inspected fields-only Newtonsoft adapter; scene lookups are an in-memory object registry; native world capture and world application are explicit adapters. Production raw/context/geometry/semantic validation, original archive extraction/hash checks and admission logic execute, but reaching the stub apply adapter does not establish tree restoration, full ecology, preview rendering or year50-to100 native simulation. No disk-save/Prefs route was invoked. The only historical data read by probes were the tracked immutable archive/schedule resources, read-only, not any player's save directory.

Probe scaffolding corrections and the initial fractional-adapter failure are retained in raw logs, not hidden: generated modifier spacing, probe flag placement, and a missing dummy Range attribute were corrected outside the repository. No production or fixture assertion was relaxed to fix those harness issues. Final committed fixture bytes were already green in both real-assembly builds; subsequent adjustments were solely to external probe scaffolding.

## Reproducible evidence

All receipts are under **`/media/jer/Files/CCF/Verification/ScenarioOne642FixtureRepair-20261116T202200Z`** on Files ext4 (UUID `2283b135-bf37-42d6-921f-2fb861d0a8d4`). No new ZX20 evidence/cache path. Important files: `preflight.json`, `running-readback.json`, `main-board-before.md`, `main-board-precommit.md`, original two fixture snapshots, `fixture.diff`, `scope-proof.json`, `typecheck-{player,editor}.txt` and command/exit JSON, `probe/generator/Program.cs`, `probe/ProbeTests.cs`, `probe/build.txt`, `probe/run.txt`, `probe/*-exit.txt`, `fixture-push.txt`, `fixture-github-compare.json` and final publication proof.

Replay the retained inspected source extraction, probe and both fixture typechecks (all generated outputs stay in the external evidence directory):

```bash
python3 -B /media/jer/Files/CCF/Verification/ScenarioOne642FixtureRepair-20261116T202200Z/run_checks.py /media/jer/Files/CCF/Operations/CCF-642-FixtureRepair
```

The actual successful commands are also preserved in the raw receipts. `probe/probe.csproj` links the current production source; generated `FixtureProbes.cs` is extracted from the exact fixture source through Roslyn, and the real Reference archive class is linked. Existing #633 evidence remains prior source evidence; this is not a repeat broad review or a new Unity qualification.

## Remaining gates and Manager next steps

**No unresolved source/fixture ownership conflict or demonstrated production defect remains from this bounded offline work.** Request Manager/independent focused fixture review of exact `7ca95a0b2822c9c6cf9ac99cbbbfbe84afe4f07e` and this report; do not treat implementation as self-approval. Keep the candidate on its verification branch until deliberately integrated/repinned by the authorised owner.

Native fixture execution remains **NOT RUN**. Later #640 must separately authorise a clean exclusive Linux Unity lane at the reviewed exact integrated candidate, with #548 deployed/self-test-green lease and true ownership, own Library/Prefs/XDG_CONFIG_HOME, isolated saves and inspected disposable copy/run/remove workflow. The Reference harness retains its pre-existing backup/restore code; inspect its side effects and only use an isolated throwaway profile under that later permit. No runtime script was staged into Assets here.

Suggested bounded later sequence, after review and all native gates:

1. Same-head import/compile; run repaired `StandGeometryVerification.Begin` with Editor-only `CCF_STAND_GEOMETRY=0`. Expect current-schema/raw/capture positives, genuine pre-v20 source load, specific malformed-source refusals before mutation, correct 64/256-cell load order and unchanged preview/light/waypoint outcomes.
2. Run repaired `ScenarioReferenceVerification.Begin` (never Freeze/BeginAuthoringCandidate for frozen v1 qualification). Expect original archive integrity/tamper controls, milestone previews, original Year-50 raw admission twice, deterministic Year100 continuation and preserved tree/history assertions.
3. Complete the packet's bounded E80 new-game/walk and protected Legacy40 completion/Reference anchors at the same approved candidate/configuration; stop at first failure, no baseline change. Frozen Y100 remains `7AD177B3CC2F73C7`; all other pinned archive values remain unchanged. No gameplay, save, full ISO or player acceptance is claimed by these offline results.

## Preservation / handoff status

Original anchor `AGENTS.md` SHA-256 `1bf4286b3485de018759a5b9b8e19d74beb6385844b56315f22f71b7819897fd` and `Docs/Operations/StorageArchitecture.md` SHA-256 `6bebe49511f310c0f97b000f2f5a6c8db6890bb77fd312b9ec47e47c900a5a22` preserved; anchor branch/HEAD unchanged. Physical Ops board/lanes bytes unchanged by this worker. The source #638 shared-save branch remains clean at `410d582ecf63a9a92c9eca2e704c8ddf3f1c3d35`; main, production validation/schema/geometry, frozen resources, native saves/Prefs and all other owners' work are untouched. No main/shared-save merge, Unity/lease, Compact admission, package/assets changes or Windows build.

**Worker assessment: ready for Manager / independent fixture review; native verification and integration pending.** Main/physical coordination updates remain with #640/authorised Ops custodian. **DASHBOARD / PUBLISHED-STATE / DRIVE SYNC NOT VERIFIED**, explicit operational follow-up; no generated dashboard file was edited. Final candidate/report commit and GitHub content readback are recorded on #642 after publication.
