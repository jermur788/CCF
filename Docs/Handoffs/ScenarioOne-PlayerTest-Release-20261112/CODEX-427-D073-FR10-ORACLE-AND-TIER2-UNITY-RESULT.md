# #427 D-073 FR-10 rendered oracle and targeted Tier-2 Unity result

Date: 12 November 2026. Worker: OpenAI / Codex **gpt-6.1-sol**, high reasoning, on Predator. Child claim: #427; parent: #412. No subagents.

> **Follow-up 1 (appended at the bottom of this file, 2026-11-12) supersedes the cleanup-blocked state and re-examines F-1/F-2; the first-run text below is kept unchanged.** Follow-up verdict: PASS (qualified: the F-1 trigger is inferred, not proven). See "Follow-up verdict".

## Verdict and release boundary

**FAIL; cleanup BLOCKED pending explicit restoration authorisation.** The required repaired-view rendered provenance oracle passes at both resolutions, including every stand/species measure, sources ON/OFF, missing snapshots, ambiguous snapshots and event-only warnings. The complete targeted matrix ran to completion without stopping after failures. This is **not** an all-green Tier-2 qualification or permission to integrate/release.

- Candidate run: **47 cases: 42 PASS / 5 FAIL**. New assertions: **5,068 = 5,063 PASS / 5 FAIL**. Inherited assertions: **237 = 233 PASS / 4 FAIL**. The mapper also ran its 132 inner controls on actual Unity production types; these are not added again to the outer assertion counts.
- One failed native foldout-close probe at **1280x720** remains unresolved; the same probe passes at **1920x1080**. The existing native foldout-open/scroll/year/ACK sequence passes at both sizes. A product regression versus native-coordinate/picking/scale issue has **not** been isolated. The failure is retained, not waived.
- The other four failed cases are additional malformed duplicate-event-ID fixture loads. Save validation correctly rejects them before rendering; those fixture loads are FAIL, their rendered UI checks are **NOT RUN**, and they do not establish a product defect. Required ambiguous-snapshot and event-only cases ran and passed.
- The Editor unexpectedly changed six Bilberry/Fungi materials in addition to the four permitted drift paths. The allowlist guard stopped. All ten dirty file bytes and the full diff are archived, none of the candidate-run drift is restored or committed. Host save/Prefs custody is PASS; final 4,011-file worktree custody and lane-clean gate are **FAIL/BLOCKED** until authorised restoration.
- Editor exited normally with verdict exit 1, both leases released normally, lock FREE, no Editor running, all seven disposable scripts and generated `.meta` files removed. No Player/Windows build, main change, production-code commit, or other Unity lane use.

## Exact identities and scope

| Item | Identity |
|---|---|
| Candidate source | `339517ed2d49bb3d5716fc9cf029d9ef0eaeee72` |
| Candidate tip (handoff-only addition) | `55863f1b7719011bba16b3f3ab377e379c0b988a` |
| Candidate branch | `feature/scenario-one-d073-fr10-visible-provenance-20261112-v1` |
| New test branch | `qualify/scenario-one-d073-fr10-visible-provenance-unity-20261112-v1` |
| Exact runner used in both Editors | `87405a562f1973de80e1c1830df1765368ca72b4` |
| Lane | `/media/jer/ZX20/CCF-S1-ResearchCopy-V1-Qual` (`ResearchCopyQual391`) |
| Lane starting branch/tip | `qualify/scenario-one-d073-year-comparison-unity-20261112-v1` / `2b0bbf1f` |
| Prior #417 final runner (read-only witness) | `0d624c61bb8852850a07b4b33d83e740a032c86c` |
| Negative-control production view | `77a56a4461f8ab2d313082af50442ce24dc54471` |
| Evidence root (all relative evidence paths below) | `/media/jer/Files/CCF/Verification/D073-FR10-427-20261112/` |

The branch was newly created from the exact candidate tip. Only verification files were committed in the runner; this report is a later documentation-only commit. `git diff --stat 339517ed2d49bb3d5716fc9cf029d9ef0eaeee72 87405a562f1973de80e1c1830df1765368ca72b4 -- Assets Packages ProjectSettings` is **empty**. `git diff --stat 55863f1b..87405a56` lists ten files, all under `Tools/Verification/YearComparison/`. `source-identity.json` preserves the proof. This **committed-tree** equality must not be confused with the ten uncommitted Editor-drift files at handoff.

Read authority: Manager #412 packet Phase 1 oracle/Phase 2 scope, approved FR-10/AC specification, independent #419 B-1 finding, #422 candidate handoff, candidate AGENTS/AgentWorkflow/project context/live coordination, and #417 report, sources and drivers. The coordination anchor's pre-existing `AGENTS.md` and `Docs/Operations/StorageArchitecture.md` modifications were reported and left untouched. No production architecture, stylesheet, scene, prefab, package, Reference Future data or gameplay rule was changed.

## Oracle design and inherited evidence

New files are `Tools/Verification/YearComparison/Fr10RenderedOracle.cs`, `run_unity_427.py` and `custody_427.py`. All twelve inherited #417 verification files remain byte-identical to their source branch (including old assertions). The seven missing inherited files were brought over from #417 rather than reimplemented. The new runner uses the old `D073Runner` on an inactive GameObject as a library; it does **not** call the full old `Start`/`Run` or the #408 cadence suite.

For `P = annual-year-comparison-<row.Key>`, the rendered oracle interrogates the **actual live AnnualReviewView visual tree** and mapper output. For all 17 full-card measures (5 stand, 12 species), it checks:

1. Unique names throughout the card; each `P-values` has a distinct `P-year-a`, `P-year-b`, `P-difference` entry for each available value, parented to that container.
2. Separate `-value` Labels exactly equal `SelectedValue`, `LatestValue`, `Difference`; no legacy Label equal to the combined mixed observation/difference `row.Text` remains.
3. `-heading` texts equal the corresponding table-header mapper fields, not hardcoded years.
4. `-tag` text exactly follows `ValueProvenance` (RECORDED observations) and `DifferenceProvenance` (DERIVED differences). Presence, resolved display/visibility along ancestors, nonzero world bounds and containment inside both the scroll viewport and panel are checked after revealing each entry. Missing entries cannot invent value/tag children.
5. Sources OFF hides chips while the actual values/headings remain visible; warning text remains and missing/uncertain observations never acquire false RECORDED tags. The appropriate NOT RECORDED warning chips appear only when sources are ON.
6. Full canonical saved state, world hash and management-event count stay equal before/after viewing, toggling, expanding and scrolling. Fixture-specific missing/ambiguous/event-only states are asserted rather than inferred from screenshots.

Visual inspection covered all 108 new-source exact-size images via contact sheets and representative full-size stand/species ON/OFF, caveat and missing-data images. No tested value/tag/heading overlap or inaccessible horizontal clipping was observed at 720p or 1080p. Neighbouring content naturally clips at the ScrollView boundary; each asserted entry was revealed inside it. The unresolved 720p native close probe is not excused by these geometry passes. Both negative-control contact sheets were also reviewed; they show the legacy mixed presentation, while the tree oracle reports missing separate entries.

`prior417-witness.json` records the original four failed runs and final historical worker PASS, all five releases, eighteen old screenshot hashes, native evidence and baseline-copy identity. Original receipts are preserved **read-only**; this is inherited evidence, not new-source approval.

### Old checks run / superseded / not run

| Unchanged inherited method or helper | This run's scope/result |
|---|---|
| `Restore` | In-memory load assertions throughout all cases; 56 PASS, 4 FAIL for the additional invalid duplicate-event fixture |
| `NativeSize` | 35 assertions PASS at each size: exact rendered pixels, latest hint, navigation/ACK geometry, real native earlier/later, no earlier ACK, foldout open, source chips show/hide, persistent expansion, original ScrollView, caveat reachability, real wheel movement, exact world/event nonmutation, Escape/reopen/latest ACK/back button |
| `NativeClick` | Retained target-present/enabled assertions also used by the new close/latest probe; one extra click was needed at 720p, without changing the assertion |
| `ReadOnlyAB` | 13 assertions PASS per size: stable card instance, hidden/guarded early-year ACK, read-only canonical state/hash, exact pre-feature baseline A/B, only original ACK event |
| `CurrentDisk` | 3 assertions PASS per size: restore and isolated current-save round trip, exact canonical state/hash |
| `OldSave` | 9 assertions PASS per size: authentic v9, Legacy40/year39/identities, no migration, read-only history, ordinary annual advance |
| `Reference` / `ReferenceArchiveIntegrity.Verify` | 22 outer assertions PASS per size; pinned raw archive integrity/tamper check, 0/20/50/100 actual preview, Legacy40, read-only browsing, exact return |
| `NegativeViews` | 13 assertions PASS per size: missing selected/latest, ambiguous snapshots, species absence not zero; all original gap-row assertions retained |
| `Provenance` | Once after both sizes: 132 inner mapper controls on real Unity types and duplicate-order exclusion; 2 outer assertions PASS |

**Superseded assertions: NONE.** No old mixed-row text assertion in the selected methods needed changing; no old assertion was edited, deleted or weakened. The full old `Run` isolation assertion hardcodes #417's output directory and is NOT RUN; the new oracle has its own #427 isolation assertion, while the original remains intact. Exact inherited assertion texts/counts are inventoried in Appendix B and `candidate1-old-check-inventory.txt`.

Old `Empty`, fresh rack/work/mortality history setup, `HistoryParity` (AC8), `WorkParity` (AC12), the complete old `Run`/99-check suite and #408 143-case cadence suite are **NOT RUN**. Authentic Year6 history is reused instead of spending another session reconstructing unchanged simulation history.

## Unity sessions and negative control

Only two Editor sessions were used; both ran on runner `87405a56` using the licensed Hub-managed `/media/jer/ZX20/Unity/6000.6.0f1/Editor/Unity`, with `-projectPath <authorised lane> -job-worker-count 2 -logFile <own evidence>/FR10.log -executeMethod Fr10RenderedOracle.Begin`. Private Xephyr/openbox/native xdotool input was used, not simulated UI callbacks for the native checks. The A/B ACK helper separately uses its existing callback path, and is labelled as such.

| Run | Source override | Duration | Editor exit | C# compiler errors | Raw result | Lease / release |
|---|---|---|---|---|---|---|
| `negative1` | Only AnnualReviewView temporarily equals exact `77a56a44` file | 140.6 s | 1 (expected oracle failure) | 0 | 2 FAIL cases; new 39 PASS / 170 FAIL; inherited 2 PASS | `698c3cd4f00e4832b2336ae2d138592b` / normally released |
| `candidate1` | Exact repaired candidate view | 1,358.9 s | 1 | 0 | 42 PASS / 5 FAIL cases; new 5063 PASS / 5 FAIL; inherited 233 PASS / 4 FAIL | `712402bec458411da1349f0c04db8cb8` / normally released |

Negative-control specifics: the driver admits a dirty source file **only** when it is the sole change, exactly byte-equal to `git show 77a56a44:Assets/ForestPrototype/UI/AnnualReviewView.cs`, and `CCF_FR10_NEGATIVE=1`. The old view SHA-256 is `174617e6442d6279e1570788c89db5487d7aa2e000964d2625099282266b429a`; repaired view SHA-256 is `8e51c9ebe108e4f177acbd9fa85ba5ddfca5ccfd21f11b015584df63ecfeac86`. The same new oracle fails sources ON and OFF at 1280x720 against the legacy mixed rows, including missing separate entry names and legacy combined Labels. This is an **actual rendered Unity failing-before control**, not a mapper-only/offline demonstration. It does not constitute a separate 1080p old-view/native qualification. The exact candidate file was restored before the real candidate run; `restored-candidate-assets.diff` is empty. Negative-run drift was archived/restored under the known four-path allowlist, with all 4,011 hashes identical before acquiring the candidate lease.

The sessions reached real `FR10_DONE` markers; wrapper return codes and loose FAIL-substring counts are not treated as test totals. Anchored `FR10_CHECK`, `D073_CHECK`, `FR10_CASE` lines produce the above counts. There are four PASS setup assertions outside candidate cases (isolation, authentic history and the two requested resolutions); thus per-case new totals sum to 5,064, overall 5,068. No licensing-198 retry, Editor kill, forced release or post-verdict reap occurred.

Candidate log SHA-256: `dcc6e361a6bc2979b7957b044123607b4a02dbcaee7663d45cd6062ee99ca486`. Negative log SHA-256: `d1c9d523a18db6d82ce42a7d47e8d3dab94082a4aeed53ca9bbaa908c27a8d7a`.

Actual log diagnostics: zero C# compiler errors, not a claim of zero log diagnostics. A startup `UnityEditor.Search.SearchDatabase` `ArgumentOutOfRangeException` is present around candidate log line 1606, also seen in #417. There are licensing access-token-update and optional Vulkan-function warnings; the Editor nevertheless licensed and ran both complete sessions. The four captured `InvalidOperationException` failures originate in rejected extra fixture loads. No production cause is inferred from those diagnostics.

## Required matrix (a–g)

PASS/FAIL here describes observed execution, not future acceptance. Both sizes ran in the same exact-source candidate Editor, so compilation/import is a shared session gate.

| Required case | 1280x720 | 1920x1080 | Observation |
|---|---|---|---|
| a. Exact candidate compile/import | PASS | PASS | Real Editor log, 0 C# compiler errors |
| b. Sources ON, separate recorded observations and derived difference, stand/species | PASS | PASS | 409 new assertions per size, all 17 measure rows |
| c. Sources OFF, values/warnings retained, chips hidden | PASS | PASS | 409 new assertions per size |
| d. Native foldout/ScrollView/caveat/year controls/latest hint | **FAIL** | PASS | Existing 35-check native sequence PASS both; added close probe FAIL only at 720p, return-latest PASS both |
| e. Missing snapshots, ambiguous snapshots and event-only warning, ON/OFF | PASS | PASS | Required fixtures all rendered and checked; details below |
| f. ACK/footer priority and read-only state/hash/events | PASS | PASS | Native and baseline A/B agree; only explicit ACK adds original acknowledgement event |
| g. Authentic v9 old-save + frozen Reference anchors | PASS | PASS | 9 + 22 inherited assertions per size, hashes/observations only, save bytes not published |

| Additional breakdown | 1280x720 | 1920x1080 |
|---|---|---|
| Missing selected snapshot ON / OFF | PASS / PASS (14 each) | PASS / PASS (14 each) |
| Missing latest snapshot ON / OFF | PASS / PASS (73 each) | PASS / PASS (73 each) |
| Both snapshots missing ON / OFF | PASS / PASS (18 each) | PASS / PASS (18 each) |
| Ambiguous snapshot ON / OFF | PASS / PASS (15 each) | PASS / PASS (15 each) |
| Event-only warning ON / OFF | PASS / PASS (414 each) | PASS / PASS (414 each) |
| Species present on one side only ON / OFF | PASS / PASS (321 each) | PASS / PASS (321 each) |
| Additional duplicate-event-ID fixture ON / OFF | Fixture load FAIL / FAIL; rendered UI NOT RUN | Fixture load FAIL / FAIL; rendered UI NOT RUN |
| Current isolated disk save/load | PASS (3 old checks) | PASS (3 old checks) |
| Existing negative-view assertions | PASS (13 old checks) | PASS (13 old checks) |

The new native-close case has 2 new assertions per size: close + return-latest. At 720p close fails, latest passes; at 1080p both pass. It also emits 7 inherited load/target checks at 720p versus 6 at 1080p because the extra opening click is attempted when needed. No assertion was changed for this difference.

## Failures and unresolved defects

### F-1: 720p native foldout-close probe (qualification FAIL, root cause unresolved)

After revealing the foldout Toggle in Year3, native requests 15 and 16 both target desktop `(957,512)`; both were delivered, yet the expected closed state is false. The same code passes at 1080p; existing inherited native opening and programmatic expansion/collapse pass. Native before/after screenshots are `candidate1/driver/desktop-before-0015.png`, `desktop-after-0015.png`, `desktop-before-0016.png`, `desktop-after-0016.png`; subsequent game captures are `candidate1/evidence/1280x720-native-extra-closed-latest.png` and `candidate1/evidence/1920x1080-native-extra-closed-latest.png`, both inventoried below. Do not use the later PASS to erase the 720p FAIL.

This needs a bounded owner-led investigation of actual picked element/scale/coordinates versus UI behaviour. No stylesheet/shared UI fix is authorised in this task; none was attempted. There is no demonstrated geometry overflow in the rendered oracle, and no basis to declare a proven source regression or dismiss the failed probe as harmless.

### F-2: invalid extra ambiguous-event fixture (test-fixture defect, not product bug)

The extra fixture duplicates management-event ID 319. The genuine save validator says: **Save rejected before loading: management event ID 319 appears twice (records 318 and 537). The current forest was not changed.** Both sources states at both sizes therefore fail the unchanged inherited load assertion, and the new case captures that exception. Raw 4 + 4 assertion failures are retained. Their UI checks are NOT RUN. The distinct required ambiguous snapshot and valid event-only warning worlds both load and render successfully.

A future valid ambiguous-event fixture would need a distinct ID while exercising the intended ambiguity. It was deliberately **not** rewritten mid-run to manufacture a green result.

### C-1: unexpected Editor drift blocks clean-lane handback

Known allowlist: `.vscode/settings.json` plus `Woodland_Forbs_01/Materials/Forb_Flower_Centre.mat`, `Forb_Leaves.mat`, `Forb_Stems.mat` under `Assets/ForestPrototype/Art/SectionFive/`.

Unexpected additional paths (same SectionFive prefix):

- `Bilberry_Type_Cover_01/Materials/Bilberry_Leaves.mat`
- `Bilberry_Type_Cover_01/Materials/Bilberry_Stems.mat`
- `Deadwood_Fungi_01/Materials/Bracket_Cap.mat`
- `Deadwood_Fungi_01/Materials/Bracket_Pores.mat`
- `Deadwood_Fungi_01/Materials/Fungi_Cap.mat`
- `Deadwood_Fungi_01/Materials/Fungi_Underside.mat`

Inspected diffs show Editor material synchronisation (`_MainTex`, Bilberry alpha-to-mask and tiny `_Color` float changes). Origin alone is not authority to expand the restore allowlist. All ten bytes are in `candidate1-drift-bytes/`; the complete patch is `candidate1-Editor-drift.patch`. No reset/stash/clean/unauthorised restoration was used. Explicit user approval to restore those six additional files from HEAD is outstanding. After approval, restore only the ten archived named paths, generate a **new** restored-after manifest and preserve the original failed manifest/incident. Do not rerun Unity merely to get a green receipt.

## Read-only, compatibility and offline gates

- Full rendered Year1-versus-Year6 world's before/after hash is `8B0BDF75F9BC3816`, with 537 management events. All valid rendered fixtures also passed exact canonical-state equality, hash equality and event-count equality. All per-fixture hashes are preserved as `FR10_WORLD` lines in the real log and `matrix-summary.json`.
- Native earlier/later/toggles/foldout/scroll/close operations add no work or management events; explicit latest ACK adds exactly the pre-existing AnnualReviewAcknowledged event, with canonical baseline A/B parity. Earlier-year hidden ACK callback is guarded; footer priority/latest hint unchanged in the passing checks.
- Authentic inherited Year6 input: prior #417 `runtime1/evidence/skip-final.json`, SHA-256 `cae00068a00ed67be03ba322da021bfbb5a3cd8ad9b965f8a0ee05965d0e25b0`. That directory was not written into; drivers were copied into this task's evidence area.
- Genuine unmodified v9/year39 save: local isolated copy `genuine-host-v9-year39.json`, SHA-256 `c0169790c8bba169ffd27c486e6a83bf67d6c938e170da58d42031bbcdf862a3`. Legacy40, year, all 361 tree IDs, nonmutation and ordinary advance passed. Neither old-save bytes nor generated save bytes are committed/published; only hashes and results are reported.
- Frozen raw v12 Reference anchors: Year0 `A564039D9B7CE31D`; Year20 `F7DF7DAB53B6FD32`; Year50 `D5E75D6D21D631AC`; Year100 `7AD177B3CC2F73C7`; schedule `56C8B99FA1E8DDD1`. Integrity/tamper control and actual preview/return checks passed; no rebaseline or biological continuation was done.
- Cheap offline helper: **53 assertions PASS**, real exit 0 (`helper-offline.log`). Offline mapper: **132 assertions across 43 cards PASS**, real build and fixture exits 0 (`mapper-offline.log`). These are separate from the Unity totals; `UseSharedCompilation=false` was used.

## Custody, isolation and lane end state

Both launches verified lock FREE/no Editor first, placed exact HEAD before acquire, acquired against actual HEAD and registered the authorised lane/branch/head in the Ops board. `negative1-acquire.json`, `candidate1-acquire.json`, corresponding `*-board.md`, `*-release.json` and per-run `lock.json` preserve receipts. Releases are non-forced and report no Unity Editor PIDs. Final recheck: lock FREE/no Editor; ZX20 has about **68 GiB free**, above the 15 GiB floor.

The launched Editor's actual logs report:

- `negative1`: `persistentDataPath=/media/jer/Files/CCF/Verification/D073-FR10-427-20261112/negative1/config/unity3d/DefaultCompany/CCF`, `XDG_CONFIG_HOME=.../negative1/config`.
- `candidate1`: `persistentDataPath=/media/jer/Files/CCF/Verification/D073-FR10-427-20261112/candidate1/config/unity3d/DefaultCompany/CCF`, `XDG_CONFIG_HOME=.../candidate1/config`.

Private config includes `gtk-menu-bar-accel=` so F10 is not intercepted by the Editor menubar; short TMPDIR `/tmp/ccf427` avoids the Unix socket limit. No host Prefs/save path was used. Temporary licence copies were removed (per-run `licence-copy-removed.json`).

| Custody gate | negative1 after restore | candidate1 / handoff |
|---|---|---|
| Host save/Prefs manifest | PASS, 37 identical | PASS, 37 identical |
| Protected worktree manifest | PASS, 4,011 identical | **FAIL**, 10 paths differ from original; all archived |
| Committed production tree versus candidate | PASS | PASS (no Assets/Packages/ProjectSettings commit differences) |
| Disposable source and `.meta` cleanup | PASS | PASS, all 7 copies removed |
| Known drift cleanup / clean lane | PASS | **BLOCKED**, guard stopped before restoring any candidate drift |
| Editor exited / normal release / FREE lock | PASS | PASS |
| Other lanes / protected #382 / main / Player builds | Untouched / not run | Untouched / not run |

Re-checkable original files are `host-before.json`, `negative1-host-after.json`, `candidate1-host-after.json`, `production-before.json`, `negative1-production-after.json`, `candidate1-production-after.json`. The production manifest covers the inherited protected 4,011-file set, including shared settings. `candidate1-custody-result.json` explicitly reports production mismatch, not a concealed PASS. Current qualification branch remains dirty only in those ten tracked drift paths; report-only commit/push does not make the worktree clean.

The 18:08 UTC publication recheck is retained separately in `publication-host-after.json`, `publication-production-after.json` and `publication-custody-result.json`: host37 identical, same ten production differences, no disposable source/metas, FREE lock/no Editor. Original post-run manifests remain unchanged.

## Reproduction and manual follow-up

Do **not** launch again while custody restoration is unauthorised. After the owner resolves custody, use a fresh evidence subdirectory and a new exact-HEAD lease; inspect the drivers again before running. The new oracle entry is `Fr10RenderedOracle.Begin`. Stage these files via the disposable driver (not permanently into Assets): `Fr10RenderedOracle.cs`, `D073Verification.cs`, `D073BaselineAnnualReviewView.cs`, `D073ReferenceIntegrity.cs`, `Cadence408GameSize.cs`, `RackAuthoringNativeInput.cs`, `YearComparisonVerification.cs`. All are under `Tools/Verification/YearComparison/`; their exact staged SHA-256 values are in `candidate1/staged-sources.json`. Pass `CCF_D073_RESUME_HISTORY` for the read-only Year6 input and `CCF_D073_OLD_SAVE` for the own-evidence v9 copy, with `--native --pass-token FR10_DONE`, an own-evidence `--out`, and no negative flag for the repaired candidate. The runner executes both sizes and all cases even after a case fails; real Editor exit remains nonzero if assertions fail.

For a bounded manual follow-up to F-1: use isolated save/Prefs and the authorised lane only; at exactly 1280x720 open Annual Review on Year3, scroll/reveal By species, click to open then close, and record actual picked element and Toggle value. Expected: the foldout closes without affecting year/latest controls or world state. Repeat at 1920x1080. Sources ON should show separate RECORDED / RECORDED / DERIVED chips for each measure; OFF should hide chips only. Earlier/later must return to follow-latest, caveat must be reachable inside the original ScrollView, and only latest ACK may create the existing acknowledgement event. This follow-up is **NOT RUN** beyond the recorded automated probe; it requires a new bounded owner decision, not an unreported production patch.

## NOT RUN / not claimed

- Full #408 143-case cadence suite; full old #417 `Run`/99-check rerun; old `Empty`, fresh setup, AC8 history parity and AC12 work parity; old #417-path isolation assertion.
- Rendered extra duplicate-event-ID fixture after validation rejection (all four combinations); a corrected distinct-ID ambiguous-event fixture.
- 1080p unrepaired negative control; separate negative-control native/compatibility suite.
- Full frozen Reference biological continuation/replay; fresh scenario whole-loop/Tier-3 qualification.
- Independent G2 review, integration/main merge, user personal playtest, Player/Windows build.
- Restoration of six unexpected material files and successful final clean-lane/4,011-identical gate; these await authorisation. No clean handback claim is made.
- Dashboard ingestion/Drive mirror refresh is not verified (parent-owned operational follow-up).

The claim remains **OPEN / BLOCKED** until safe cleanup is authorised/completed; overall result remains FAIL unless the native 720p failure is properly resolved or dispositioned by the owner. Branch/report publication is a worker handoff, not integration or approval.

## Appendix A — complete candidate case inventory

All 47 cases below ran; fixture-load FAIL rows explicitly have rendered UI NOT RUN. `New checks` is a total, not a PASS count. Inherited counts preserve raw load failures. Four global setup PASS assertions are outside this table.

| Case | Result | New checks | Inherited PASS | Inherited FAIL |
|---|---|---:|---:|---:|
| `1280x720/rendered-on` | PASS | 409 | 1 | 0 |
| `1280x720/rendered-off` | PASS | 409 | 1 | 0 |
| `1280x720/inherited-native` | PASS | 0 | 35 | 0 |
| `1280x720/native-foldout-close-and-latest` | FAIL | 2 | 7 | 0 |
| `1280x720/missing-selected-on` | PASS | 14 | 1 | 0 |
| `1280x720/missing-selected-off` | PASS | 14 | 1 | 0 |
| `1280x720/missing-latest-on` | PASS | 73 | 1 | 0 |
| `1280x720/missing-latest-off` | PASS | 73 | 1 | 0 |
| `1280x720/both-missing-on` | PASS | 18 | 1 | 0 |
| `1280x720/both-missing-off` | PASS | 18 | 1 | 0 |
| `1280x720/ambiguous-on` | PASS | 15 | 1 | 0 |
| `1280x720/ambiguous-off` | PASS | 15 | 1 | 0 |
| `1280x720/event-only-on` | PASS | 414 | 1 | 0 |
| `1280x720/event-only-off` | PASS | 414 | 1 | 0 |
| `1280x720/ambiguous-event-on` | FAIL | 1 | 0 | 1 |
| `1280x720/ambiguous-event-off` | FAIL | 1 | 0 | 1 |
| `1280x720/species-one-sided-on` | PASS | 321 | 1 | 0 |
| `1280x720/species-one-sided-off` | PASS | 321 | 1 | 0 |
| `1280x720/inherited-readonly-ACK-AB` | PASS | 0 | 13 | 0 |
| `1280x720/inherited-current-save` | PASS | 0 | 3 | 0 |
| `1280x720/inherited-v9` | PASS | 0 | 9 | 0 |
| `1280x720/inherited-reference` | PASS | 0 | 22 | 0 |
| `1280x720/old-negative-assertions-retained` | PASS | 0 | 13 | 0 |
| `1920x1080/rendered-on` | PASS | 409 | 1 | 0 |
| `1920x1080/rendered-off` | PASS | 409 | 1 | 0 |
| `1920x1080/inherited-native` | PASS | 0 | 35 | 0 |
| `1920x1080/native-foldout-close-and-latest` | PASS | 2 | 6 | 0 |
| `1920x1080/missing-selected-on` | PASS | 14 | 1 | 0 |
| `1920x1080/missing-selected-off` | PASS | 14 | 1 | 0 |
| `1920x1080/missing-latest-on` | PASS | 73 | 1 | 0 |
| `1920x1080/missing-latest-off` | PASS | 73 | 1 | 0 |
| `1920x1080/both-missing-on` | PASS | 18 | 1 | 0 |
| `1920x1080/both-missing-off` | PASS | 18 | 1 | 0 |
| `1920x1080/ambiguous-on` | PASS | 15 | 1 | 0 |
| `1920x1080/ambiguous-off` | PASS | 15 | 1 | 0 |
| `1920x1080/event-only-on` | PASS | 414 | 1 | 0 |
| `1920x1080/event-only-off` | PASS | 414 | 1 | 0 |
| `1920x1080/ambiguous-event-on` | FAIL | 1 | 0 | 1 |
| `1920x1080/ambiguous-event-off` | FAIL | 1 | 0 | 1 |
| `1920x1080/species-one-sided-on` | PASS | 321 | 1 | 0 |
| `1920x1080/species-one-sided-off` | PASS | 321 | 1 | 0 |
| `1920x1080/inherited-readonly-ACK-AB` | PASS | 0 | 13 | 0 |
| `1920x1080/inherited-current-save` | PASS | 0 | 3 | 0 |
| `1920x1080/inherited-v9` | PASS | 0 | 9 | 0 |
| `1920x1080/inherited-reference` | PASS | 0 | 22 | 0 |
| `1920x1080/old-negative-assertions-retained` | PASS | 0 | 13 | 0 |
| `inherited-real-type-mapper` | PASS | 0 | 2 | 0 |

## Appendix B — exact inherited assertion inventory

Identical emitted lines are consolidated only for readability. Counts sum to 237 (233 PASS / 4 FAIL). The full chronological raw inventory is retained externally and was not rewritten.

| Repetitions | Unchanged emitted assertion |
|---:|---|
| 56 | `D073_CHECK PASS in-memory reload accepted` |
| 1 | `D073_CHECK PASS exact requested rendered pixel resolution 1280x720; logical panel=(x:0.00, y:0.00, width:1600.00, height:900.00)` |
| 2 | `D073_CHECK PASS actual latest single hint` |
| 1 | `D073_CHECK PASS year navigation inside game (x:225.00, y:85.00, width:136.25, height:31.25)` |
| 1 | `D073_CHECK PASS latest ACK footer inside game (x:287.50, y:823.75, width:293.75, height:35.00)` |
| 25 | `D073_CHECK PASS native target present/enabled` |
| 2 | `D073_CHECK PASS OS earlier selects previous year` |
| 2 | `D073_CHECK PASS OS year selection reaches genuine planting-work Year3` |
| 2 | `D073_CHECK PASS OS earlier cannot ACK latest` |
| 2 | `D073_CHECK PASS species initially closed` |
| 2 | `D073_CHECK PASS OS species foldout opens` |
| 4 | `D073_CHECK PASS native target present/enabled annual-show-data-sources` |
| 2 | `D073_CHECK PASS OS sources shows chips` |
| 2 | `D073_CHECK PASS species expansion survives source rebuild` |
| 2 | `D073_CHECK PASS live card inside the original Annual Review scroll content` |
| 1 | `D073_CHECK PASS caveat reachable inside game (x:242.50, y:835.00, width:1095.00, height:26.25)` |
| 2 | `D073_CHECK PASS native target present/enabled annual-year-comparison-caveat` |
| 1 | `D073_CHECK PASS actual pointer wheel scroll changes position 2840→2516` |
| 2 | `D073_CHECK PASS OS sources hides chips` |
| 2 | `D073_CHECK PASS OS later returns follow-latest and disables Later` |
| 2 | `D073_CHECK PASS all native view actions exact world/event nonmutation` |
| 2 | `D073_CHECK PASS OS Escape back to forest` |
| 2 | `D073_CHECK PASS reopening follows latest` |
| 2 | `D073_CHECK PASS native target present/enabled annual-review-acknowledge` |
| 2 | `D073_CHECK PASS OS actual latest ACK works` |
| 2 | `D073_CHECK PASS OS ACK records only existing acknowledgement event` |
| 2 | `D073_CHECK PASS native target present/enabled annual-review-walk` |
| 2 | `D073_CHECK PASS OS Back to forest button closes review` |
| 4 | `D073_CHECK FAIL in-memory reload accepted` |
| 2 | `D073_CHECK PASS unchanged per-frame refresh keeps same card instance` |
| 2 | `D073_CHECK PASS earlier-year footer hidden` |
| 8 | `D073_CHECK PASS baseline/programmatic action exists` |
| 2 | `D073_CHECK PASS earlier hidden ACK callback guarded` |
| 2 | `D073_CHECK PASS AC7 browsing/toggle/open/close exact saved world and hash nonmutation` |
| 2 | `D073_CHECK PASS AC7 card vs exact pre-D073 view actions including ACK same full state/hash` |
| 2 | `D073_CHECK PASS ACK only existing one acknowledgement event` |
| 2 | `D073_CHECK PASS current disk save/load exact canonical state` |
| 2 | `D073_CHECK PASS current disk save/load world hash` |
| 2 | `D073_CHECK PASS genuine unmodified archived host v9/year39` |
| 2 | `D073_CHECK PASS genuine old save loads Legacy40` |
| 2 | `D073_CHECK PASS old save year retained` |
| 2 | `D073_CHECK PASS genuine old save all tree identities retained` |
| 2 | `D073_CHECK PASS no old save geometry migration` |
| 2 | `D073_CHECK PASS genuine old history browsing leaves world unchanged` |
| 2 | `D073_CHECK PASS genuine Legacy40 ordinary annual advance still works` |
| 2 | `D073_CHECK PASS frozen original Y100 pinned` |
| 2 | `D073_CHECK PASS frozen reference actual preview opens 0` |
| 2 | `D073_CHECK PASS reference preview state/year 0` |
| 8 | `D073_CHECK PASS reference remains Legacy40` |
| 2 | `D073_CHECK PASS reference review browse exact nonmutation 0` |
| 2 | `D073_CHECK PASS reference return exact player world 0` |
| 2 | `D073_CHECK PASS frozen reference actual preview opens 20` |
| 2 | `D073_CHECK PASS reference preview state/year 20` |
| 2 | `D073_CHECK PASS reference review browse exact nonmutation 20` |
| 2 | `D073_CHECK PASS reference return exact player world 20` |
| 2 | `D073_CHECK PASS frozen reference actual preview opens 50` |
| 2 | `D073_CHECK PASS reference preview state/year 50` |
| 2 | `D073_CHECK PASS reference review browse exact nonmutation 50` |
| 2 | `D073_CHECK PASS reference return exact player world 50` |
| 2 | `D073_CHECK PASS frozen reference actual preview opens 100` |
| 2 | `D073_CHECK PASS reference preview state/year 100` |
| 2 | `D073_CHECK PASS reference review browse exact nonmutation 100` |
| 2 | `D073_CHECK PASS reference return exact player world 100` |
| 2 | `D073_CHECK PASS negative actual view card exists missing-selected` |
| 2 | `D073_CHECK PASS negative exact gap row missing-selected` |
| 2 | `D073_CHECK PASS negative actual view card exists missing-latest` |
| 2 | `D073_CHECK PASS negative exact gap row missing-latest` |
| 2 | `D073_CHECK PASS negative actual view card exists ambiguous` |
| 2 | `D073_CHECK PASS negative exact gap row ambiguous` |
| 2 | `D073_CHECK PASS negative actual view card exists species-one-sided` |
| 2 | `D073_CHECK PASS one-sided species absence is not zero` |
| 1 | `D073_CHECK PASS exact requested rendered pixel resolution 1920x1080; logical panel=(x:0.00, y:0.00, width:1600.00, height:900.00)` |
| 1 | `D073_CHECK PASS year navigation inside game (x:665.83, y:50.83, width:128.33, height:31.67)` |
| 1 | `D073_CHECK PASS latest ACK footer inside game (x:290.00, y:814.17, width:290.00, height:33.33)` |
| 1 | `D073_CHECK PASS caveat reachable inside game (x:245.83, y:822.50, width:1088.33, height:26.67)` |
| 1 | `D073_CHECK PASS actual pointer wheel scroll changes position 2815.833→2491.833` |
| 1 | `D073_CHECK PASS 132 mapper controls on actual Unity production types` |
| 1 | `D073_CHECK PASS duplicate order IDs excluded in Unity real-types control` |

## Appendix C — rendered screenshot manifest

Paths are relative to the evidence root above, not repository paths. These are **118 original rendered-game PNGs**: 108 candidate images (54 at each requested size) and 10 negative-control images (720p). Contact sheets and private desktop-driver captures are supplementary, not substitutes for these exact-pixel images. Every dimension and SHA-256 was rechecked during report preparation.

| PNG path | Pixels | SHA-256 |
|---|---|---|
| `negative1/evidence/1280x720-full-off-species-beech-standing-trees.png` | 1280x720 | `d9de423a7c6395e932f47f4ca4b48e6a5f6ee619567313206e3edfffc29565da` |
| `negative1/evidence/1280x720-full-off-species-sessile-oak-standing-trees.png` | 1280x720 | `d9de423a7c6395e932f47f4ca4b48e6a5f6ee619567313206e3edfffc29565da` |
| `negative1/evidence/1280x720-full-off-species-sitka-spruce-standing-trees.png` | 1280x720 | `d9de423a7c6395e932f47f4ca4b48e6a5f6ee619567313206e3edfffc29565da` |
| `negative1/evidence/1280x720-full-off-stand-canopy.png` | 1280x720 | `d9de423a7c6395e932f47f4ca4b48e6a5f6ee619567313206e3edfffc29565da` |
| `negative1/evidence/1280x720-full-off-stand-living-trees.png` | 1280x720 | `d9de423a7c6395e932f47f4ca4b48e6a5f6ee619567313206e3edfffc29565da` |
| `negative1/evidence/1280x720-full-on-species-beech-standing-trees.png` | 1280x720 | `500f232c54474d4b2729a46df560f65b5eee82a763cb33d62b8a55653011ab28` |
| `negative1/evidence/1280x720-full-on-species-sessile-oak-standing-trees.png` | 1280x720 | `500f232c54474d4b2729a46df560f65b5eee82a763cb33d62b8a55653011ab28` |
| `negative1/evidence/1280x720-full-on-species-sitka-spruce-standing-trees.png` | 1280x720 | `500f232c54474d4b2729a46df560f65b5eee82a763cb33d62b8a55653011ab28` |
| `negative1/evidence/1280x720-full-on-stand-canopy.png` | 1280x720 | `500f232c54474d4b2729a46df560f65b5eee82a763cb33d62b8a55653011ab28` |
| `negative1/evidence/1280x720-full-on-stand-living-trees.png` | 1280x720 | `500f232c54474d4b2729a46df560f65b5eee82a763cb33d62b8a55653011ab28` |
| `candidate1/evidence/1280x720-both-missing-off-missing-latest.png` | 1280x720 | `8d754ce4ec863f1d2bbcdcde6fecc417fd1edbdb70cc2553d33f8b0b6fedf1c7` |
| `candidate1/evidence/1280x720-both-missing-off-missing-selected.png` | 1280x720 | `8d754ce4ec863f1d2bbcdcde6fecc417fd1edbdb70cc2553d33f8b0b6fedf1c7` |
| `candidate1/evidence/1280x720-both-missing-on-missing-latest.png` | 1280x720 | `ba17ce576257bad43c061c19c9fd5975125eb529fa692a7fb17e8cdccf7cde18` |
| `candidate1/evidence/1280x720-both-missing-on-missing-selected.png` | 1280x720 | `ba17ce576257bad43c061c19c9fd5975125eb529fa692a7fb17e8cdccf7cde18` |
| `candidate1/evidence/1280x720-event-only-off-species-beech-standing-trees.png` | 1280x720 | `793533d23359311181d8484b194e2074fc5e3d797dc8577f3ffe4d57e3fdd1ee` |
| `candidate1/evidence/1280x720-event-only-off-species-sessile-oak-standing-trees.png` | 1280x720 | `3d70b5366c7186cae79a802090bd69cd822319f598f2b0ab9a1fd0706355fcce` |
| `candidate1/evidence/1280x720-event-only-off-species-sitka-spruce-standing-trees.png` | 1280x720 | `09b255b85bcf237204a292cd6b15f18a8f38eaf585d7d6720cd3438fb6978a49` |
| `candidate1/evidence/1280x720-event-only-off-stand-canopy.png` | 1280x720 | `6ef167406fd0cf60e7ae1b764cd3c3a3b87e050b0b2d5255258fd585759ec71e` |
| `candidate1/evidence/1280x720-event-only-off-stand-living-trees.png` | 1280x720 | `41439a2e3aa8c9130e1b5f3d77c793e368cc2237f2e80f7f920ead32a7acfaf8` |
| `candidate1/evidence/1280x720-event-only-off-work-event-only.png` | 1280x720 | `41439a2e3aa8c9130e1b5f3d77c793e368cc2237f2e80f7f920ead32a7acfaf8` |
| `candidate1/evidence/1280x720-event-only-on-species-beech-standing-trees.png` | 1280x720 | `a2a5cb15cca64f46d41817cd5ea19e5f099f34f1000a921f66765ed3c908b628` |
| `candidate1/evidence/1280x720-event-only-on-species-sessile-oak-standing-trees.png` | 1280x720 | `a25675b84af1c768e0df08450b6a32d760d16d6ddf9e475049f12fcf344500d0` |
| `candidate1/evidence/1280x720-event-only-on-species-sitka-spruce-standing-trees.png` | 1280x720 | `8797be2483b1059020162ed2db1eb5391a171ae2bda1cd32faabd27d822343c9` |
| `candidate1/evidence/1280x720-event-only-on-stand-canopy.png` | 1280x720 | `489af28608822a68d67c98ad9439b6516a0b1ecdb7d6a774e15a672adcc941c8` |
| `candidate1/evidence/1280x720-event-only-on-stand-living-trees.png` | 1280x720 | `c25cd241ff014797ba9b19b1e6322465de7c1e8e70eb3477689793bc5ed426f3` |
| `candidate1/evidence/1280x720-event-only-on-work-event-only.png` | 1280x720 | `c25cd241ff014797ba9b19b1e6322465de7c1e8e70eb3477689793bc5ed426f3` |
| `candidate1/evidence/1280x720-full-off-species-beech-standing-trees.png` | 1280x720 | `a637307d92273f7822ee4819c85bfb3621d166c6229e14b2b58a182638247d83` |
| `candidate1/evidence/1280x720-full-off-species-sessile-oak-standing-trees.png` | 1280x720 | `29a97472e181f4da57c596d165ed708d7e4d49ccedf6a4d9fd596df2a43d700e` |
| `candidate1/evidence/1280x720-full-off-species-sitka-spruce-standing-trees.png` | 1280x720 | `c54ef761a7a87373db55ead215532f350e20e7021d64ac7a78086a1556d76eb9` |
| `candidate1/evidence/1280x720-full-off-stand-canopy.png` | 1280x720 | `2d4736cf6d3b8597b7ddf65f6adc20a3546eeb0e27c00f3dc4056174de1c16aa` |
| `candidate1/evidence/1280x720-full-off-stand-living-trees.png` | 1280x720 | `b4b0594881681973f76469f033154d54f20c5050d814dd599bc7e5870ce39d61` |
| `candidate1/evidence/1280x720-full-on-species-beech-standing-trees.png` | 1280x720 | `bc3f670503747553f5be224630a2d2a4f81217dbe7e3f61df7068d0d03e2aa83` |
| `candidate1/evidence/1280x720-full-on-species-sessile-oak-standing-trees.png` | 1280x720 | `9c760b2fd3c656938ea26691eec5523f14b80c9a1b3b7c2866382e0e981aa09a` |
| `candidate1/evidence/1280x720-full-on-species-sitka-spruce-standing-trees.png` | 1280x720 | `b9072e12ba27608bee33df0065a0e6d9d1fa753044b4ab79f3ecd4e7a52cd206` |
| `candidate1/evidence/1280x720-full-on-stand-canopy.png` | 1280x720 | `706ba3b65ac66d5c1bcd4fabcedc15c2a2cb58eebd2350177ca348fd4c71d8e6` |
| `candidate1/evidence/1280x720-full-on-stand-living-trees.png` | 1280x720 | `c85381551ccae15bb20f5ea8781f68c012483b2c6a6e2d5f1fdc240ec5c87d03` |
| `candidate1/evidence/1280x720-missing-latest-off-missing-latest.png` | 1280x720 | `39a8551728e777ef917c48641ff05293efbe8d8ae4c3afe0901977d866f7f01f` |
| `candidate1/evidence/1280x720-missing-latest-off-stand-canopy.png` | 1280x720 | `39c8659bbb7bea392aec7dfbe26faba8cf1c53f1e1d998a80d093abf7fc5692d` |
| `candidate1/evidence/1280x720-missing-latest-off-stand-living-trees.png` | 1280x720 | `39c8659bbb7bea392aec7dfbe26faba8cf1c53f1e1d998a80d093abf7fc5692d` |
| `candidate1/evidence/1280x720-missing-latest-on-missing-latest.png` | 1280x720 | `11bdc045235a69d4d11050d5d3c3e4f696059419fcb310afb3fedd64e616c4b8` |
| `candidate1/evidence/1280x720-missing-latest-on-stand-canopy.png` | 1280x720 | `2310bb67b977bc046ec94078d91c98f1866392fe85666c5aaf83d91d0a8772eb` |
| `candidate1/evidence/1280x720-missing-latest-on-stand-living-trees.png` | 1280x720 | `9fe5970005bf95cf41fdd315c2f47ca1d8d0a8eead30c55f574f6224d1f6e7e2` |
| `candidate1/evidence/1280x720-missing-selected-off-missing-selected.png` | 1280x720 | `f609190ca2501b7e6e634f25fcfa21618c0a2530b1b3f86c5a7c98ac60431204` |
| `candidate1/evidence/1280x720-missing-selected-on-missing-selected.png` | 1280x720 | `c4fd91085356b8b7969eda9774cb9350da0804f72bcf11856b82b254d2cf6582` |
| `candidate1/evidence/1280x720-native-extra-closed-latest.png` | 1280x720 | `0adf0384603a1ea87a64a284d32d2803f7355fcb081748e042a560b3305c69d0` |
| `candidate1/evidence/1280x720-species-one-sided-off-species-beech-not-recorded.png` | 1280x720 | `d5b8eb4dd04001fbfc96eaf198538abb54188216bf826ad88c9f86c7da159719` |
| `candidate1/evidence/1280x720-species-one-sided-off-species-sessile-oak-standing-trees.png` | 1280x720 | `cc7f56de5e996e856a20b479712942f43d14962a83c3d926e5b5ac2ee88555bd` |
| `candidate1/evidence/1280x720-species-one-sided-off-species-sitka-spruce-standing-trees.png` | 1280x720 | `f616666662c0cd6dba755aa1490ff3e7364708c422447d2d49dee39794ba8b47` |
| `candidate1/evidence/1280x720-species-one-sided-off-stand-canopy.png` | 1280x720 | `f0af6801713683ff7fc4e035bf25a94cde20bbc361a7ecfd540454182d189bbe` |
| `candidate1/evidence/1280x720-species-one-sided-off-stand-living-trees.png` | 1280x720 | `23aaad0299df4165a8244d9d19467621b2a47acfc84b7ea19e9d2de7e24c661a` |
| `candidate1/evidence/1280x720-species-one-sided-on-species-beech-not-recorded.png` | 1280x720 | `c30383bee281fdc090dbf681c5df1b063a698a84ccddc96c678fcb479447e23a` |
| `candidate1/evidence/1280x720-species-one-sided-on-species-sessile-oak-standing-trees.png` | 1280x720 | `7fd849bf8e56814e630fc51bf5bff5781b9a3cb67de51c73d87e533306bbe783` |
| `candidate1/evidence/1280x720-species-one-sided-on-species-sitka-spruce-standing-trees.png` | 1280x720 | `5e890606f47f3a36426a7364c95cfe43000ff5d402d3c7ae0a9a590d8d9c6876` |
| `candidate1/evidence/1280x720-species-one-sided-on-stand-canopy.png` | 1280x720 | `24936ef70e95b487a69d4ab9cab57f07845f9dfe1826d25b2b03b103c2bdee67` |
| `candidate1/evidence/1280x720-species-one-sided-on-stand-living-trees.png` | 1280x720 | `d92944651fc0f55e781fa2388b473f3eacb3584a0f5b127e7270a02f5b7bd04e` |
| `candidate1/evidence/1920x1080-both-missing-off-missing-latest.png` | 1920x1080 | `f43e0ced921c8e11d2510c0b04400c6e42b359310f8e7bf41b19f58622622899` |
| `candidate1/evidence/1920x1080-both-missing-off-missing-selected.png` | 1920x1080 | `f43e0ced921c8e11d2510c0b04400c6e42b359310f8e7bf41b19f58622622899` |
| `candidate1/evidence/1920x1080-both-missing-on-missing-latest.png` | 1920x1080 | `85bad3bce839d7304fa21dc18ee644dd0156eaea58c9e38f6bb9b08d5d3b8f55` |
| `candidate1/evidence/1920x1080-both-missing-on-missing-selected.png` | 1920x1080 | `85bad3bce839d7304fa21dc18ee644dd0156eaea58c9e38f6bb9b08d5d3b8f55` |
| `candidate1/evidence/1920x1080-event-only-off-species-beech-standing-trees.png` | 1920x1080 | `6969346a448739457c7e65c53aa2f45b3abc1104bf0fc1bf345808c42bda96a4` |
| `candidate1/evidence/1920x1080-event-only-off-species-sessile-oak-standing-trees.png` | 1920x1080 | `436041b40fdea9e49346d36b15ffd6815e5c4cd62fd7b3a636c3e967eb75c315` |
| `candidate1/evidence/1920x1080-event-only-off-species-sitka-spruce-standing-trees.png` | 1920x1080 | `39a919bc26c7fbb74441727355b4ab13fd1b21739dab1f4f509a19697d247135` |
| `candidate1/evidence/1920x1080-event-only-off-stand-canopy.png` | 1920x1080 | `358593045e03acfcf371d03a7cba9d13816cac2fd64661558311c95cd597c907` |
| `candidate1/evidence/1920x1080-event-only-off-stand-living-trees.png` | 1920x1080 | `c2387195c461e757dcdd94cbc1626a39eea08d1b123429b76cd78d3ab4b7833a` |
| `candidate1/evidence/1920x1080-event-only-off-work-event-only.png` | 1920x1080 | `c2387195c461e757dcdd94cbc1626a39eea08d1b123429b76cd78d3ab4b7833a` |
| `candidate1/evidence/1920x1080-event-only-on-species-beech-standing-trees.png` | 1920x1080 | `dc790ce50cb59dbfd34d974aad5492f34dd4114b20a591484c26dd065f56deaf` |
| `candidate1/evidence/1920x1080-event-only-on-species-sessile-oak-standing-trees.png` | 1920x1080 | `35661a1f83df6e97613bd922cd57dd52af1adc91ffba561a5a70890b41bf281d` |
| `candidate1/evidence/1920x1080-event-only-on-species-sitka-spruce-standing-trees.png` | 1920x1080 | `cbe64982760fa9e4c79753333337d12e4d61ef4479c82bc84d50e8f341b4ff30` |
| `candidate1/evidence/1920x1080-event-only-on-stand-canopy.png` | 1920x1080 | `143f3c95e54a8a4036d3d24392ed36c9d40d4293687216ed3df19bd5327f6890` |
| `candidate1/evidence/1920x1080-event-only-on-stand-living-trees.png` | 1920x1080 | `de6ea32e504ce4bdfd46031efac542cf32b469e97c23b53c34182fe66e7c86f0` |
| `candidate1/evidence/1920x1080-event-only-on-work-event-only.png` | 1920x1080 | `de6ea32e504ce4bdfd46031efac542cf32b469e97c23b53c34182fe66e7c86f0` |
| `candidate1/evidence/1920x1080-full-off-species-beech-standing-trees.png` | 1920x1080 | `6aad7be28244a8d0ffd6e4625a0d4da6512e21f2f557ea6f9cbed3d400981fe0` |
| `candidate1/evidence/1920x1080-full-off-species-sessile-oak-standing-trees.png` | 1920x1080 | `0dbee3951914d10c8bcf00ff04cbb1059b078819137c8c0c3774b0f8dfcd8cf0` |
| `candidate1/evidence/1920x1080-full-off-species-sitka-spruce-standing-trees.png` | 1920x1080 | `9963481d37d03825d2a2ff8defdab6b4332c1614eb35009064c75d034b3cc3b7` |
| `candidate1/evidence/1920x1080-full-off-stand-canopy.png` | 1920x1080 | `568479ec2d2e86ea0538b63e145a333a5b19ab606d61b7a96149dd8bfc465c80` |
| `candidate1/evidence/1920x1080-full-off-stand-living-trees.png` | 1920x1080 | `aad07e0d4bb05d44ff1815f4c69d138c99680ff7029620448178d0cbe3b05c62` |
| `candidate1/evidence/1920x1080-full-on-species-beech-standing-trees.png` | 1920x1080 | `db99369f13a59bb8649e70af4e6ccac4856b4d846b2031fba2e312d4213af4b0` |
| `candidate1/evidence/1920x1080-full-on-species-sessile-oak-standing-trees.png` | 1920x1080 | `380e2b5bb3f663afc856888d350a2abec386ee7b4060f4aba7931fb0a92419fe` |
| `candidate1/evidence/1920x1080-full-on-species-sitka-spruce-standing-trees.png` | 1920x1080 | `1f80bf7f4a053d50403283fe6119ee1a45e73dc4c6c78f83e0d41d16384bc2f8` |
| `candidate1/evidence/1920x1080-full-on-stand-canopy.png` | 1920x1080 | `ddc974571a5093451c758626998b86949041a2633194d2fb0566050ea6e501b8` |
| `candidate1/evidence/1920x1080-full-on-stand-living-trees.png` | 1920x1080 | `70d5085c1ed9df381e113794607809a42ce8a84dba4fa0e6bf7d51754d40b7f0` |
| `candidate1/evidence/1920x1080-missing-latest-off-missing-latest.png` | 1920x1080 | `756930a1078fc0352dfe93fed16255e57c632eb73a9e65d794a33f5b259e3f4c` |
| `candidate1/evidence/1920x1080-missing-latest-off-stand-canopy.png` | 1920x1080 | `d119af17482ce8d5592db7cccd0a66ca42f5d0acae532ab0a12b6a522a20c2e4` |
| `candidate1/evidence/1920x1080-missing-latest-off-stand-living-trees.png` | 1920x1080 | `d119af17482ce8d5592db7cccd0a66ca42f5d0acae532ab0a12b6a522a20c2e4` |
| `candidate1/evidence/1920x1080-missing-latest-on-missing-latest.png` | 1920x1080 | `c27de891849ea635e1235812120fee51c981248cfbb0ed7b3207b351cd724fa7` |
| `candidate1/evidence/1920x1080-missing-latest-on-stand-canopy.png` | 1920x1080 | `6174d28240d1390c1389133ea7511bc9e9d6761615d009454a80cd877456f68a` |
| `candidate1/evidence/1920x1080-missing-latest-on-stand-living-trees.png` | 1920x1080 | `c7a4d2141a266b570292fc6be97dde79d30b053c64c9dab19b4c38df68f47090` |
| `candidate1/evidence/1920x1080-missing-selected-off-missing-selected.png` | 1920x1080 | `0f1c2056d56570d427bf59cbffb2b1cabec664f8cfd5ea7783d3999708fe0d81` |
| `candidate1/evidence/1920x1080-missing-selected-on-missing-selected.png` | 1920x1080 | `a91d3632a1d8ff8d188b7fc03b034915d754d177eb3579f103d6eca9ea7f67ea` |
| `candidate1/evidence/1920x1080-native-extra-closed-latest.png` | 1920x1080 | `f55c5b65167555ba7fb6f3cff423a811615ffcba1035e79a1411da6c2c41751a` |
| `candidate1/evidence/1920x1080-species-one-sided-off-species-beech-not-recorded.png` | 1920x1080 | `fbbaf85627a1a941a048bb75c408b9b8d14159a629668ad0174b02b94f47ed4a` |
| `candidate1/evidence/1920x1080-species-one-sided-off-species-sessile-oak-standing-trees.png` | 1920x1080 | `029fff2e75601c370cfde52793df875fba8217fbb97c23d3be0f612878a9afe1` |
| `candidate1/evidence/1920x1080-species-one-sided-off-species-sitka-spruce-standing-trees.png` | 1920x1080 | `1bb6ddb8bb1cf95e131d20f3c045711d8265fb6b62752f51e9228dfb7ae732a0` |
| `candidate1/evidence/1920x1080-species-one-sided-off-stand-canopy.png` | 1920x1080 | `e04aff73ad8509c997a7646f9a35cb099a91ae68a4abaa772efbe934bba13cc8` |
| `candidate1/evidence/1920x1080-species-one-sided-off-stand-living-trees.png` | 1920x1080 | `65a458564b365adec2ea143777a4c39396386f92a0f0690aa582843eab07cd1c` |
| `candidate1/evidence/1920x1080-species-one-sided-on-species-beech-not-recorded.png` | 1920x1080 | `f24b4fa1c40e662291b3b9ea580286f0718820f64f015902171a3b787d91e9de` |
| `candidate1/evidence/1920x1080-species-one-sided-on-species-sessile-oak-standing-trees.png` | 1920x1080 | `200fbc429eb6b4321b730d2192a4bebef07d9149576d399d2d8d113f13f94bee` |
| `candidate1/evidence/1920x1080-species-one-sided-on-species-sitka-spruce-standing-trees.png` | 1920x1080 | `0dffa95682dd657fcc21dca7143ee2d272c743ce65720d944016f5270a6d8a06` |
| `candidate1/evidence/1920x1080-species-one-sided-on-stand-canopy.png` | 1920x1080 | `16c6c19a772e659a1fa229d00328a09a5fe4904dd7f252e9bc797052fcd732d1` |
| `candidate1/evidence/1920x1080-species-one-sided-on-stand-living-trees.png` | 1920x1080 | `aa2f1ef6289bcb4009bcb6bdcc78b7439dc2bef385f61e588078efe352bc7e48` |
| `candidate1/evidence/native-1280x720-acknowledged.png` | 1280x720 | `0191f30cc0939d2307af38dd29ea704bc3dcabb5f1623a4f15debd2cc6546a64` |
| `candidate1/evidence/native-1280x720-earlier-no-sources.png` | 1280x720 | `a18264e264a230f8a9af03d0335390fed203001a3cbeac4a5f412262ed47c8e7` |
| `candidate1/evidence/native-1280x720-earlier-sources.png` | 1280x720 | `8c9934cf218717629eb7679060eea3199484b85f7232051b808ab77927789493` |
| `candidate1/evidence/native-1280x720-expanded-species-caveat.png` | 1280x720 | `bd257660b355c518748280d33f06c158ccf3664364b9d3c814a6006b80d55569` |
| `candidate1/evidence/native-1280x720-latest-footer.png` | 1280x720 | `06648ecfccce7e2b9c619c4c9e2bc2fd31dddeb00205b3bb74e976fb79956319` |
| `candidate1/evidence/native-1920x1080-acknowledged.png` | 1920x1080 | `c80ecdcd2dae2d6beab0c8bf9baef825ab71876c24db751d5fa639edd5738979` |
| `candidate1/evidence/native-1920x1080-earlier-no-sources.png` | 1920x1080 | `19418e4198643358eac2ff08ca5328010f346cc768f395612065d2ac74f4715b` |
| `candidate1/evidence/native-1920x1080-earlier-sources.png` | 1920x1080 | `d5e0688a21ff29569c92ee30df39e7fffe9d4b9cf1cb43cabfecf0deb5b1c823` |
| `candidate1/evidence/native-1920x1080-expanded-species-caveat.png` | 1920x1080 | `0225f7661ea27c41aea4354e7960dbb30e216189f4ab8149887eb9c96b592984` |
| `candidate1/evidence/native-1920x1080-latest-footer.png` | 1920x1080 | `e0311ff3db9f195219a182e5d2bf0b3c068f8b708f82fc09dcf9d02da8cdf440` |
| `candidate1/evidence/negative-1280x720-ambiguous.png` | 1280x720 | `24652de9e10d98979524d48b8abbceb362ddebb2c08650eaeefd6692e363cb28` |
| `candidate1/evidence/negative-1280x720-missing-latest.png` | 1280x720 | `f15b28fa848e7cf28348236ceec6dc0f8ee9d07ea527fb14bc4cfe34a453b5f7` |
| `candidate1/evidence/negative-1280x720-missing-selected.png` | 1280x720 | `74f67e855e1c983f0f98d76a419792b30ad1e06c65f3ab08dee75d9ca5156566` |
| `candidate1/evidence/negative-1280x720-species-one-sided.png` | 1280x720 | `804a92008c3acff00615fbd713fa9cf5a5efa29bc779d8a77c4bea54f78ff76e` |
| `candidate1/evidence/negative-1920x1080-ambiguous.png` | 1920x1080 | `0471c8deac47ad4d6c876c2e89f790417298b9ecc48c32544509459b1c8cf760` |
| `candidate1/evidence/negative-1920x1080-missing-latest.png` | 1920x1080 | `d414f616a0db1ccf3df9202f505b7f4db8fc0863b8d981ffa1f9dcefbe8df0ef` |
| `candidate1/evidence/negative-1920x1080-missing-selected.png` | 1920x1080 | `bf4d2b6ada2c53c144e78e930490d81e032c23c787c33ba1b49dbe03b297e941` |
| `candidate1/evidence/negative-1920x1080-species-one-sided.png` | 1920x1080 | `501e0ec6d5610c93688b40adbaf1b5986590fd44c07ae20b2e831150484d9420` |

### Supplementary native failure captures

The following are desktop before/after evidence for native requests 15/16, not exact-pixel game screenshots and not included in the 118 count.

| PNG path | Pixels | SHA-256 |
|---|---|---|
| `candidate1/driver/desktop-before-0015.png` | 1920x1080 | `551e6dd3080b89a25b7b21d83af81d0537bdb6fdaa9b01d7382ed4739f38fb08` |
| `candidate1/driver/desktop-after-0015.png` | 1920x1080 | `7b575c936bf897cec05d38595071fc3f04ffd532cb666877fab54ed1c19a101b` |
| `candidate1/driver/desktop-before-0016.png` | 1920x1080 | `7b575c936bf897cec05d38595071fc3f04ffd532cb666877fab54ed1c19a101b` |
| `candidate1/driver/desktop-after-0016.png` | 1920x1080 | `47310c01104cad4e84c2ed96ae952928a33024ab8cac1036161b11ad274e00c7` |

## Appendix D — evidence index

- Session logs/receipts/isolation/staging: `negative1/FR10.log`, `candidate1/FR10.log`, each `result.json`, `staged-sources.json`, `lock.json`, `licence-copy-removed.json`.
- Anchored assertions/cases: `matrix-summary.json`, `candidate1-case-inventory.tsv`, `candidate1-old-check-inventory.txt`, `candidate1-failures.txt`, corresponding negative-control inventories, each `evidence/fr10-results.txt` and `inherited-checks.txt`.
- Native actions: `candidate1/driver/` and `driver.stdout`; screenshot-manifest full inventory: `screenshot-manifest.json`.
- Source/provenance/history witness: `source-identity.json`, `fixture-provenance.json`, `prior417-witness.json`, `restored-candidate-assets.diff`.
- Custody: both baseline manifests, both runs' host/production after manifests, acquisition/release receipts, drift patches/byte archives and custody-result JSONs.
- Offline: `helper-offline.log`, `mapper-offline.log`; copied inspected drivers `inherited-start_session.py`, `inherited-finish_session.py`; aggregation tool `summarize427.py`.
- Inspection copies of authority documents: `inspection/`. No save bytes or licence material are added to Git.


---

# Follow-up 1 (2026-11-12, appended; the first-run text, numbers and recorded failures above are unchanged)

Authorised by the parent disposition comment on #427 (18:11 UTC): ten-path drift restoration, root-cause isolation of F-1 (including the same probes on the unrepaired `77a56a44` view), and a valid distinct-ID fixture for F-2. Test-only: no production source was edited, no Player was built, `main` and other lanes were not touched, no subagents.

## Follow-up verdict

**PASS — qualified, with the F-1 trigger inferred rather than proven.** Read the qualification before relying on the word PASS.

- Every required case a–g is PASS at both 1280x720 and 1920x1080 when the corrected native close probe (a NEW test, `Fr10Followup427.CorrectedClose`) is taken as the case-d verdict. The first-run FAIL of the original probe stays on record, unedited, and is not converted to a pass.
- **F-1 is classified HARNESS DEFECT (OS pointer delivery in the original blind probe), not a product defect and not a regression from the repair.** Evidence below. What is **not** proven: the exact event that moved the pointer in the single first-run failure. The first run had no pointer/event tracing, so that one event cannot be replayed, and it was not reproduced in 15 replays of the unchanged probe (9 candidate, 6 base) or 400 stress clicks (200 candidate, 200 base).
- F-2 is resolved as a test-fixture defect; a valid distinct-ID ambiguous-event world passes ON and OFF at both sizes.
- The original raw failures remain on record: F-1 (1) and the four duplicate-ID fixture loads (F-2). They are not recounted as passes.

## Workers, cut-offs and custody events

| Step | Worker | Outcome |
|---|---|---|
| First run (`negative1`, `candidate1`), report `87b1d157` | Codex gpt-6.1-sol (high) | 47 cases, 42 PASS / 5 FAIL; stopped at the C-1 restoration boundary |
| Follow-up Step A and first diagnostic session (`candidate-diagnostic`) | Codex gpt-6.1-sol (high), 2nd claim worker | Restored the ten archived drift files, re-proved the 4,011-file manifest (`restoration-verification.json`), added runner `b219ca70`; **cut off by provider usage limit at ~18:25 UTC** after the Editor had exited with `FOLLOWUP_DONE checks=19 failures=0` |
| Everything below | Claude Sonnet 5.5 (Claude A), successor | Closed the orphaned session, isolated F-1, ran F-2, wrote this section |

There were two Codex usage-limit interruptions on this claim (one before the first-run report was finalised, one mid follow-up). The successor found the machine-wide lock still held by the orphaned lease `aadacab876644ec9b3023a3e96c98928` (task `CCF-427-D073-FR10`, head `b219ca70`, lane ResearchCopyQual391), no Unity Editor process, only a leftover `VBCSCompiler` (pid 526897, not touched), the four known drift files dirty, no staged harness copies. It then ran that session's own closing steps: host 37-file manifest compared with `host-before.json` (identical), four drift files archived (`candidate-diagnostic-drift-bytes/`, `…-Editor-drift.patch`), allowlisted `git checkout --` of exactly those four paths, 4,011-file production manifest identical, lane clean, and a normal `unity-lock release` (not forced; receipt `followup1/candidate-diagnostic-release.json`). `b219ca70` was then pushed with a normal `git push`. The four-path allowlist applied in every later session; no unexpected drift paths appeared in any follow-up session.

### Leases (all released normally, none forced, Editor never killed)

| Session | Lease id | Runner HEAD | Evidence directory (`followup1/`) |
|---|---|---|---|
| `candidate-diagnostic` | `aadacab876644ec9b3023a3e96c98928` (Codex; released by successor) | `b219ca70` | `candidate-diagnostic/` |
| `base-diagnostic` | `7340053066b04893a6111a1d1c30c7be` | `f0b79863` | `base-diagnostic/` |
| `candidate-full` | `cf509a5d9f664377ab14ca361ca5e769` | `3e281aff` | `candidate-full/` |
| `candidate-faithful` | `9c1163ac97f84f37a44ec5c745744c8b` | `27e1b680` | `candidate-faithful/` |
| `candidate-stress` | `1c5f18c9e97e46208895f693e71220e3` | `d9970c06` | `candidate-stress/` |
| `base-replica-stress` | `b46baf3d2e9544d290301f5605f8d5d4` | `e2a91857` | `base-replica-stress/` |

Each session: lock FREE and no Editor beforehand; lease taken with the exact lane HEAD; production tree equal to candidate (`git diff --stat 339517ed HEAD -- Assets Packages ProjectSettings` empty, re-proved by the driver); isolated save/config under the evidence directory (checked by `FOLLOWUP_ISOLATION` log line); licence copy removed after exit; Editor exited by itself (`EditorApplication.Exit`); 0 C# compiler errors in every log; host 37-file and production 4,011-file manifests identical after each session (`<session>-custody-result.json`, `-host-after.json`, `-production-after.json`); lease released normally (`<session>-release.json`).

Runner commits added by the successor (all under `Tools/Verification/YearComparison/`, test-only): `2539782c` replica probe and panel-level tracing; `f0b79863` owner override for the custody script; `3e281aff` shared native request counter and `replica` phase; `27e1b680` `faithful` phase and rebuild polling; `d9970c06` focus-loop A/B stress, fast native driver, window/focus tracing; `73d7876f`+`e2a91857` driver `--sync` removal, cursor tracing, `replica-stress` phase, compile fix. Each version was type-checked with the Unity-bundled Roslyn `csc` against the project's assemblies before use (`followup1/typecheck*/compile.log`; the first had one CS0104 that was fixed in the next commit before it was ever run).

**Successor mistakes, recorded:** (1) `base-diagnostic` ran with a harness bug of mine — two native request counters wrote the same `request-0001.json`, so its `first-run-order-replica` case threw `IOException` at both sizes (`FOLLOWUP_CHECK FAIL … already exists`, session exit 1). Its diagnostic probes (the other 16 checks) are valid and passed. Fixed in `3e281aff`. (2) In `candidate-stress`, a pointer-parking helper I started mid-run (`stress-pointer-parker.sh`, private display :84 only) moved the pointer away while `xdotool mousemove --sync` was waiting, so 2 clicks landed off the toggle: those are the only 2 "lost" clicks in the whole follow-up and are my own tooling's fault, not F-1 evidence (see below). (3) My progress comment at 19:2x UTC said "220" stress clicks; the true number is 200 per variant.

## F-1 root-cause analysis

### What the first run showed (re-read from the raw captures)

`candidate1/driver/desktop-{before,after}-{0015,0016}.png` (full 1920x1080 Xephyr captures), cropped:

| Capture | Foldout | "By species" header tint |
|---|---|---|
| before 15 | closed (▸) | plain |
| after 15 | open (▾), rows visible | blue (pointer hovering the toggle) |
| before 16 | open | blue |
| after 16 | **open** (unchanged) | **plain — hover tint gone** |

A pixel difference (`desktop-before-0016` vs `desktop-after-0016`, threshold 40/255) is confined to a 29x6-pixel box at (776,505)–(805,511), i.e. the header text only: 94 pixels, nothing else on the screen changed (no rebuild flash, no scroll, no layout change). The probe therefore started from the state it assumed (closed), request 15 landed and opened it, and request 16 changed nothing except removing the hover tint. In the follow-up sessions, after a **successful** close with the pointer left on the toggle, the header **stays** tinted (`candidate-diagnostic/driver/desktop-after-0002.png`: closed ▸ and tinted). Hover loss without any toggle means the pointer was no longer over the toggle at the end of request 16.

### Hypotheses tested

| # | Hypothesis | Verdict | Evidence |
|---|---|---|---|
| 1 | Two quick clicks at one point coalesce into a double click / second click swallowed | **Rejected** | Requests 15 and 16 were ~5 s apart in the first run (xdotool log `candidate1/driver/native-actions.jsonl`). Deliberate 100 ms two-click bursts register **both** clicks (`clickCount=1` then `2`) and toggle twice (open then closed) on candidate and base at both sizes (`*/evidence/click-trace.txt`, probe `two-clicks-100ms`). |
| 2 | Probe's assumed start state wrong at 720p; extra-opening-click logic mis-sequenced | **Rejected** | Before-15 capture shows closed; the probe's `if (!value) click; click;` ran as written (click 15 opened). Replays of the unchanged probe in the same order start closed and pass. |
| 3 | Coordinates/picking wrong at 0.44x Game-view scale | **Rejected** | Panel `Pick` at every traced click is the toggle's input element (`unity-toggle__input`), at 720p and 1080p, at three scroll positions (top/middle/bottom of viewport); the identical desktop point (957,512) hit the toggle for request 15. |
| 4 | View rebuild after a toggle loses a click / taller rows change behaviour | **Not supported** | Polling for card/foldout instance and `ChangeKey` inputs (`rebuild-poll` lines) shows rebuilds only on explicit actions (year buttons), never between a toggle and the next click; identical on base and candidate. The first-run pixel diff shows no rebuild signature. |
| 5 (added) | OS pointer displaced between the move and the click | **Best-supported** | Matches the after-16 signature; a displaced click reproduces it (below); the unchanged probe and ~400 clicks do not fail otherwise. |

### What the follow-up sessions prove (counts are real result lines)

| Probe | Candidate `339517ed` | Unrepaired base `77a56a44` |
|---|---|---|
| Single human-paced closed→open and open→closed, at 3 scroll positions, per size (8 checks/size; `foldout-diagnostic`) | 16/16 PASS (`candidate-diagnostic`) | 16/16 PASS (`base-diagnostic`) |
| Original-order sequence, fresh review (`original-sequence`), per size | PASS both sizes | PASS both sizes |
| Two clicks 100 ms apart | both register, toggles twice, both sizes | identical |
| **Unchanged** original probe `Fr10RenderedRunner.NativeExtra`, replayed right after the unchanged inherited native sequence (3 repeats/size) | **6/6 PASS** (`candidate-full`) | **6/6 PASS** (`base-replica-stress`) |
| Full first-run 720p order (rendered ON → rendered OFF → inherited native → original probe x3) | **3/3 PASS**, rendered oracle 409/0 ON and OFF (`candidate-faithful`) | not run in this order |
| Fast native toggle stress, 40 clicks x4 blocks at 720p + 20 x2 at 1080p, focus loop ON/OFF blocks alternating | 200 clicks, 0 genuine losses; 2 tool-induced (below) | **200 clicks, 0 lost** |
| Corrected native close probe (NEW test) | PASS both sizes (5 checks each) | not run |

Base and candidate therefore behave identically: no probe distinguishes the repaired view from the unrepaired one, which also rules out "regression from the repair". The failure is not pre-existing product behaviour either, because it never occurs in any traced run.

The one reproduction of the F-1 signature came from the successor's own tooling: during `candidate-stress` block 0 (focus loop ON) my pointer-parking helper moved the pointer while the driver's `xdotool mousemove --sync` was waiting; the click then landed at the parked point. Trace for those two clicks: **no** `panel-pointer-down` on the toggle, a `focus-out` of the toggle, `value` unchanged — "click issued, nothing toggled, hover gone". This shows the signature is what OS-level pointer displacement looks like in this setup; it is not evidence that the same displacement happened in the first run. The remaining candidate blocks (parker still running but only parking a pointer that had sat still >0.6 s, after the click was done) and every base block (no parker: the driver was changed to `mousemove` without `--sync` in `e2a91857`) show zero losses with the focus loop both ON and OFF, so the driver's 2-second window re-activation loop is **not** supported as the cause either.

Also noted: when the pointer is already at the target, `xdotool mousemove --sync` blocks ~15 s before the click (visible as 15.x s gaps in `native-actions.jsonl` of every follow-up session). The original probe, written without that in mind, sends a blind absolute-move-then-click with no check that the pointer is where it should be.

### Conclusion

**HARNESS DEFECT (probe design), qualified.** The original `native species foldout closes` probe is a blind OS click that never verifies pointer position; the only evidence about the failed click (hover tint lost, nothing else changed) says the pointer was not over the toggle for that click. A human-paced single click closes the foldout on the candidate at 720p (and at 1080p), repeatedly and identically on the unrepaired base. A corrected probe in a NEW file passes at both sizes. The precise trigger of the single first-run displacement is **not identified** (it would need pointer tracing in a repeat that did not happen); no production change is indicated and none is proposed. If the parent prefers a stricter bar, the honest alternative reading is "F-1 not reproduced in more than 400 trials; cause unproven", which leads to the same release recommendation: no product fix.

### Corrected close probe and where it lives

`Fr10Followup427.CorrectedClose` (new file `Tools/Verification/YearComparison/Fr10Followup427.cs`, committed in `b219ca70` by the cut-off worker, exercised here). It: restores the Year-6 history, shows Year 3, scrolls the foldout to mid-viewport, **asserts the closed starting state**, moves the pointer away and lets it settle (1.1 s), sends one native click and asserts it opened, settles, sends one native click and asserts it closed, asserts the whole world is byte-equal before/after (read-only), then returns to follow-latest through real native "Later year" clicks. The original probe (`Fr10RenderedOracle.NativeExtra`) and its recorded FAIL are untouched.

## F-2 valid distinct-ID fixture

Check of the fixture before running: the save validator (`ForestSaveValidation.ValidateManagementEventIds`) rejects only repeated or non-positive **event IDs**; it does not look at work-order IDs. The readout (`ScenarioOneManagementFollowonReadout`) separately flags resolved work whose `workOrderId` repeats (`duplicateOrders`), producing the `work-ambiguous` row ("2 work record(s) have ambiguous provenance and are not counted."). The new fixture clones a Year-1 `WorkResolved` event (ID 319, order 1) with a **new unique** event ID 538 and `nextManagementEventId` 539, so the world loads (`in-memory reload accepted`) and legitimately exercises the same-order-resolved-twice ambiguity.

| Case | 1280x720 | 1920x1080 |
|---|---|---|
| Fixture is a valid loadable world; distinct IDs 319/538, same order 1 | PASS | PASS |
| Loaded world's mapper emits `work-ambiguous` row ("2 work record(s)…") | PASS | PASS |
| Unchanged rendered oracle (all measure rows, chips, warning) sources **ON** | PASS | PASS |
| Same, sources **OFF** | PASS | PASS |

The four fixture renders together contributed 1,652 rendered-oracle checks, 0 failures (`candidate-full/evidence/oracle-checks.txt`, 1,664 lines including the 12 replay checks).
| Actual rendered warning label retained, sources ON / OFF | PASS / PASS | PASS / PASS |

The four original duplicate-ID-319 raw failures from the first run stay on record; they are rejected by the validator exactly as before and are not changed by this result.

## Per-case table: original result and follow-up result

| Case | 720p original | 720p follow-up | 1080p original | 1080p follow-up |
|---|---|---|---|---|
| a. Exact compile/import | PASS | PASS (6 more Editor sessions: 4 on the exact candidate, 2 with the base view swapped in; 0 CS errors) | PASS | PASS |
| b. Sources ON, separate RECORDED/RECORDED/DERIVED | PASS 409/0 | PASS 409/0 re-run (`candidate-faithful`) + fixture ON PASS | PASS 409/0 | not re-run (first run stands); fixture ON PASS |
| c. Sources OFF, values/warnings kept | PASS 409/0 | PASS 409/0 re-run + fixture OFF PASS | PASS 409/0 | not re-run; fixture OFF PASS |
| d. Native foldout/scroll/caveat/year/latest | **FAIL** (original close probe) | **PASS**: inherited native sequence PASS; corrected close probe PASS; original-probe replays 9/9 PASS candidate, 6/6 base; **original FAIL stays on record** | PASS | PASS (corrected probe + 3 replays) |
| e. Missing/ambiguous snapshots, event-only | PASS | not re-run, plus F-2 ambiguous-event fixture PASS ON/OFF | PASS | same |
| f. ACK/footer, read-only state/hash/events | PASS | not re-run; corrected probe's read-only world equality PASS | PASS | same |
| g. Authentic v9 + frozen Reference | PASS | not re-run (first run stands) | PASS | not re-run |
| Extra duplicate-ID fixture | FAIL x4 (load rejected) | replaced by valid distinct-ID fixture, PASS ON/OFF | FAIL x4 | PASS ON/OFF |

## 720p readability, looked at directly

I viewed the exact-pixel 1280x720 captures (`candidate1/evidence/native-1280x720-expanded-species-caveat.png`, `1280x720-full-on-stand-canopy.png`, `1280x720-full-off-species-beech-standing-trees.png`, and the new `candidate-full/evidence/1280x720-distinct-ID-ambiguous-on.png`, `…-corrected-native-closed.png`). With the species foldout open, a player can scroll through every stand and species row; each measure shows its name, three columns (End of Year N / End of Year M / Difference) with the value under each heading, RECORDED tags on the two observations and a DERIVED tag on the difference when sources are ON (absent but "calculated from records" kept when OFF), the ambiguity warning where applicable, and the fixed caveat ("These are recorded stand totals for two years…") is reachable at the bottom of the card by scrolling. I saw no clipped value, tag or caveat. This is a rendered-screenshot review by an AI, not a human playtest.

## New screenshots (exact pixels, no scaling)

76 PNGs under `followup1/<session>/evidence/`, all exactly 1280x720 or 1920x1080; the full list with dimensions and SHA-256 is `followup1/followup-screenshot-manifest.json`. New unique captures:

| File | Size | SHA-256 |
|---|---|---|
| `candidate-full/evidence/1280x720-corrected-native-closed.png` | 1280x720 | `404294125e5438e2b20f70a1cc951cb7a85255b09360cf52fedc37a2fb540fb9` |
| `candidate-full/evidence/1920x1080-corrected-native-closed.png` | 1920x1080 | `f13bcfb07bdb19d873fcb829aa70e49048fa1a6e75b8fb6ab3d3543f0dbb3e91` |
| `candidate-full/evidence/1280x720-distinct-ID-ambiguous-on.png` | 1280x720 | `1af19bbe410bd836d404c70f23762f0ce2df2a11a0af50c45b2dc7d43f1307c4` |
| `candidate-full/evidence/1280x720-distinct-ID-ambiguous-off.png` | 1280x720 | `6f07866a1463e22f8a97b0a0910123acfe9bbe355daaa3d3a526e262c4213a8a` |
| `candidate-full/evidence/1920x1080-distinct-ID-ambiguous-on.png` | 1920x1080 | `eb27152b35c01363afcae9e6b008313b83b9f8a515b13557520dd7a94cf5eb1d` |
| `candidate-full/evidence/1920x1080-distinct-ID-ambiguous-off.png` | 1920x1080 | `823952d49101f596fb5bfff8a5034204fb07d8fc6125325ca2b28e1e07fc16dd` |
| `candidate-diagnostic/evidence/1280x720-diagnostic-closed.png` | 1280x720 | `269940a39fa7807461c9b897fd52a6b4467aa78ea538c52cd3a6153cc3a451a4` |
| `candidate-diagnostic/evidence/1920x1080-diagnostic-closed.png` | 1920x1080 | `aa9d9930aa27c02ad4a89c5064a7479d9e26e5c509b1a170704b3ac019daab28` |
| `base-diagnostic/evidence/1280x720-diagnostic-closed.png` | 1280x720 | `2c2f076786683b91c6536f9e99b7f523e1040fe8242d37dfa8408003b17014a8` |
| `base-diagnostic/evidence/1920x1080-diagnostic-closed.png` | 1920x1080 | `9382fcaea0783664500b2b7fb68f046d6648d422d0cda40f3b689ea42930a2a3` |

Another 362 full-desktop Xephyr captures (`*/driver/desktop-{before,after}-*.png`, 1920x1080 desktop) back the native requests; they are not Game-view screenshots.

## Session results (real `FOLLOWUP_DONE` / case lines)

| Session | Phase | Driver wrapper | Editor exit | Real result |
|---|---|---|---|---|
| `candidate-diagnostic` | diagnostic | pass | 0 | `FOLLOWUP_DONE checks=19 failures=0`, 2 cases PASS |
| `base-diagnostic` (77a56a44 swapped in) | diagnostic | FAIL | 1 | probe checks 16/16 PASS; 2 cases FAIL = my request-counter bug (see above) |
| `candidate-full` | full | pass | 0 | `checks=35 failures=0`; oracle 1,664 checks / 0 failures; inherited 122 |
| `candidate-faithful` | faithful | pass | 0 | `checks=6 failures=0`; oracle 824 / 0; inherited 55 |
| `candidate-stress` | stress | FAIL | 1 | 2 FAIL lines, both the tool-induced mis-clicks in block 0; clean blocks 0 lost |
| `base-replica-stress` (77a56a44 swapped in) | replica-stress | pass | 0 | `checks=15 failures=0`; replicas 6/6; stress 0/200 lost |

After each base session the candidate file was restored with path-limited `git checkout -- Assets/ForestPrototype/UI/AnnualReviewView.cs`, and `git diff 339517ed2d49bb3d5716fc9cf029d9ef0eaeee72 -- Assets Packages ProjectSettings` was shown empty (`base-diagnostic-restored-candidate-assets.diff`, `base-replica-stress-restored-candidate-assets.diff`, both 0 bytes).

## End state

Editor exited; no Unity process left (the leftover `VBCSCompiler` pid 526897 found at the start is Unity's own compiler server and was not touched); `unity-lock` FREE; all eight disposable scripts and generated `.meta` removed from `Assets/ForestPrototype/` after every session; four Editor-drift paths archived then restored via the ten-path allowlist after every session; lane `git status --short` empty; host 37-file manifests identical; 4,011-file production manifest identical to `production-before.json`; every manifest is kept as a file in `followup1/`.

## NOT RUN in Follow-up 1

- No new run of cases b, c (1080p), e, f, g beyond what is stated; the first-run results for those stand.
- No Windows/Player build; no full #417 99-check or #408 143-case reruns; no Reference biological replay, G2, integration.
- No human-played or human-paced desktop session; "human-paced" here means the OS click arrives after a 1.1 s settle with the pointer moved in first, delivered by `xdotool` on a private Xephyr display.
- The first-run F-1 event itself was not replayed with pointer tracing (it cannot be); its cause is inferred.
- No Editor was started on the user's normal desktop; no standalone Windows input behaviour was tested.

## Follow-up 1 evidence index

All paths under `/media/jer/Files/CCF/Verification/D073-FR10-427-20261112/followup1/`: per-session directories (`candidate-diagnostic`, `base-diagnostic`, `candidate-full`, `candidate-faithful`, `candidate-stress`, `base-replica-stress`) each with `FOLLOWUP.log`, `evidence/click-trace.txt`, `evidence/followup-results.txt`, `evidence/oracle-checks.txt`, `evidence/inherited-checks.txt`, `driver/` request/native-action logs and desktop captures, `result.json`, `summary-lines.txt`; per-session custody files `<session>-{acquire,release,custody-result,host-before,host-after,production-before,production-after,Editor-drift.patch}`; `<session>-drift-bytes/`; `restoration-allowlist.json`, `restoration-verification.json`, `production-restored.json`; `followup-screenshot-manifest.json`; `stress-pointer-parker.sh`; `typecheck*/compile.log`.
