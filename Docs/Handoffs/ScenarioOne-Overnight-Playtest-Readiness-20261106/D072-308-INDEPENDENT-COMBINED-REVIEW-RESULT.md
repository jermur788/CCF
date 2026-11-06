# D-072 #308 independent combined source-verifier review

**Reviewer verdict: NO BLOCKING FINDINGS. Scope: frozen source-verifier candidate only.** This is a separate reviewer opinion for Manager #200, not Manager acceptance, integration approval, a #287 release, Unity qualification or player acceptance.

## Identity, authority and independence

- Candidate: `feature/scenario-one-next-action-guidance-v1`, **`9789369d02963b4444f8530b071413d066bf1ddf`**; repair start `d531f2308a103d022aca3128497b5c3a975228d5`.
- Reviewer: **OpenAI Codex / GPT-6.1 Sol / Medium reasoning effort**, running directly in T3 Code. The packet recommends High; actual Medium is disclosed, not relabelled. Provider capacity was observed through this working session, not assumed from a future reset date.
- Independence: fresh session, cross-provider relative to the Claude Opus 5.5 repair author. This reviewer authored none of #296/#303/#306/#308, did not direct their repairs, and started no child agents. Author handoff was treated as a claim to check.
- Assignment: [Manager retry packet](https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Overnight-Playtest-Readiness-20261106/MANAGER-D072-308-INDEPENDENT-COMBINED-REVIEW-RETRY-PACKET-20261106.md), [binding ruling](https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Overnight-Playtest-Readiness-20261106/MANAGER-D072-STREAM-A-B-REVIEW-RULING-20261106.md), authoritative published #307 report, and live origin/main coordination.
- Task class: PACKET ONLY, Reviewer; no production implementation or spec change. Packet supplies no separate full context pin. Candidate canonical-context commit independently resolved as `81867c8940d7d07c952e8e49ca4a0cb93498e790`; live Manager packet/coordination were read separately at main `d66d4b652d7f4cddfea97078784317a3dd1fae18`.
- Review branch: `docs/d072-308-independent-combined-review-20261106-v1`; lane `/media/jer/Files/CCF/Operations/CCF-D072-308-IndependentReview-20261106`. Its only authored repository path is this report. Test runs occurred before adding this file, at the exact candidate HEAD.
- [Contemporaneous RUNNING receipt](https://github.com/jermur788/CCF/issues/310#issuecomment-6698019050).

## Fresh preflight and ownership

Before local Git writes, observed origin candidate matched the frozen SHA; requested review branch was absent locally and remotely; worktree registration showed no lane with this new branch/path. Fresh live coordination and #310 showed no active reviewer or feature writer. Created the distinct conditional reviewer lane; its tracked/untracked/ignored status was empty and index entries were normal. Original feature lane was independently observed clean, at the exact candidate, with no index flags; it was never edited. Raw receipt is `preflight.log` and `live-active-tasks.md` under the evidence root below.

Files backing storage was `/dev/sdb4`, ext4, rw, about 62 GiB available at admission. The frozen feature lane remains on ZX20; no Unity worktree/cache was created or borrowed. Original ownership is handed off/no writer; the unpublished Sonnet branch/worktree and #309 lane were preserved and not used as evidence. Existing Primary edits in `AGENTS.md` and `Docs/Operations/StorageArchitecture.md` were preserved.

## Diff, bytes and assertion retention

Independent `git diff --name-status d531f230 9789369d` contains **exactly four permitted paths**:

1. `Tools/Verification/ScenarioOneFiveRackDiagram/run_five_rack_check.py`;
2. `Tools/Verification/ScenarioOneFiveRackDiagram/test_stage_inventory_guard.py`;
3. new `Tools/Verification/ScenarioOneFiveRackDiagram/expected_assertions.json`;
4. new `Docs/Handoffs/ScenarioOne-Overnight-Playtest-Readiness-20261106/D072-FIVE-RACK-CHECKER-F1-F3-REPAIR-HANDOFF.md`.

No production, #303, snapshot, prior-handoff, package or project-settings delta. Independently comparing all `Assets/Packages/ProjectSettings` against `316429dc` gives **no changed path**. Both diagram files, Objectives and the three original C# verifier/probe/stub files equal their historical #296 bytes. Historical blob IDs and hashes were checked against Git objects and filesystem bytes, rather than trusting an embedded pin to validate itself.

| File | Independent SHA-256 at frozen candidate |
|---|---|
| Five-rack runner | `03d927cb4c733a61534ed6b6976b2cdd54d72ec82231b203f3fd1b8b5a48ec0e` |
| Five-rack integrity test | `52734749163585d4cc7d963111a189a5e1233d9769751e9f1655efb75c5c7ab2` |
| Assertion inventory | `6515df087111ebda1c1832dcba72bf938648721cad6a7c5222c96b38055c4dbe` |
| TeachingDiagram.cs | `5062dd7dd1836cdb9663b257c0e4a4b74daf3b6f183923a27e6f86e35be83874` |
| TeachingDiagramPlan.cs | `967b21b8bfd1bedafa3fdb23e8bcda1c90bcbf3696882a918b62a33628514771` |

Actual fresh-run names are **182 source / 109 structural / 165 painter**, exactly matching the pinned inventory in order. The earlier #307 review's captured 140 source names all remain, in the same relative order; this provenance comparison uses its durable raw log, not its author's handoff. AST evaluation of both original mutation tables at `335ee6b3` and the candidate proves all **15 plan + 14 painter tuples** identical: names, old strings, replacements and expected failure texts. The original #296 131-obligation runner was also executed against the candidate working tree as a historical comparison (`historical-original-131.log`): its only 2 failures are the old scope guards (`only allowed files changed since 1b68ddcd`; `no package, project-settings or Stand Map checker change`), which are expected because the candidate legitimately adds the four repair files. Every original obligation is still asserted by name or by a named stricter successor (none lost without a successor).

## Independently executed four-suite matrix

Python **3.12.3**, .NET SDK **10.0.114**. Every required command used `python3 -I -B`, the same exact candidate, and a fresh explicit output directory outside the repository. No Unity process was launched. Final top-level markers are the evidence below; the initial launcher did not separately retain top-level shell exit codes. Integrity child logs retain their measured exits. No unobserved exit status is asserted here.

| Suite | Result | Observed evidence |
|---|---|---|
| Five-rack full | PASS | `FIVE_RACK_SUITE_PASS source_checks=182 failed=0`; structural 109; painter 165; all 29 mutations applied and caught |
| Five-rack integrity | PASS | `STAGE_INVENTORY_INTEGRITY_TALLY MUTATIONS=80 BENIGN=4 CLEAN_RUNS=3 CONTROLS=29 FAIL=0`; `STAGE_INVENTORY_INTEGRITY_PASS`; clean full run 182/109/165; 192-combination verdict table |
| Unmodified #303 headland | PASS | 39 source; `HEADLAND_OFFLINE_TALLY PASS=92 FAIL=0`; fingerprint `1e1bd6388f287525d17793c553a6bc2adc88f9e279b50d67c98ba5cb6fdaa6d1` |
| Unmodified #303 integrity | PASS | `MUTATIONS=20 CLEAN_RUNS=2 FAIL=0`; `STAGE_INVENTORY_INTEGRITY_PASS` |

```bash
python3 -I -B Tools/Verification/ScenarioOneFiveRackDiagram/run_five_rack_check.py --out "$E/five-rack-full"
python3 -I -B Tools/Verification/ScenarioOneFiveRackDiagram/test_stage_inventory_guard.py --work "$E/five-rack-integrity"
python3 -I -B Tools/Verification/StandMap/HeadlandOffline/run_headland_check.py --out "$E/headland-full"
python3 -I -B Tools/Verification/StandMap/HeadlandOffline/test_stage_inventory_guard.py --work "$E/headland-integrity"
```

Here `$E` was `/media/jer/Files/CCF/Verification/D072-308-IndependentReview-20261106`; these directories now contain evidence, so choose new names for a rerun. The observed totals reproduce the repair author's four-suite claims. They are new reviewer executions, not a relabelling of the user's two reported reruns or Manager-independent evidence.

## Distinct falsification and prior false-green reproduction

New reviewer script `independent-negatives.py` constructed N01–N10 without importing the author's fixtures. Each ran the real five-rack checker in a disposable full clone with `--scope-only`; each measured **exit 1 + FIVE_RACK_SUITE_FAIL**, with exactly the expected failing assertion set:

| IDs | Exact files/change | Intended rejection actually observed |
|---|---|---|
| N01/N02 | Append one space to `TeachingDiagram.cs`; append one tab to `TeachingDiagramPlan.cs` | Respective working-tree #296 byte pin only |
| N03/N04 | Painter title margin 6→9; plan entrance size 11→7 | Respective byte pin only |
| N05 | Plan note prepends `Road already open.` | Plan byte pin only |
| N06 | Correlate margin 6→9, entrance 11→7 and the false road claim | Exactly both diagram working-tree pins; no incidental scope or count failure |
| N07 | New `Tools/Verification/ScenarioOneFiveRackDiagram/test_stage_inventory_guard.py.bak` containing a NUL | Exact allowed-path and six-file-folder guards |
| N08 | `Packages/ReviewHidden.cs` hidden through clone `.git/info/exclude` | Ignored-production detector only |
| N09 | `ProjectSettings/HiddenReview/fake.asset` hidden by nested `.gitignore` | Ignored-production detector only |
| N10 | `--assume-unchanged` on painter and `--skip-worktree` on plan | Index-flags guard only |
| N11 | Objectives call passes `!rackV2Objective` | Exit 1, named Objectives byte pin / exact call-site / passed-flag failures; no traceback or internal-error failure |

Reviewer harness correction: initial reset did not clear flags after N10, contaminating the first N11 and subsequent composite attempt. That composite was cancelled and not counted. N11 also initially expected the old parser crash; the repaired checker correctly avoids it. Retained `negative-harness-correction.txt` explains the mistake. A fresh clone and explicit flag reset produced the isolated N11 result; no candidate code was changed to fit the test. Both attempts are retained.

**Prior false-pass composite (corrected, fresh clone, flags reset):** the earlier review's combined false-pass example now exits 1. It fails both diagram byte pins, both verifier byte pins (`FiveRackDiagramCheck.cs`, `PainterProbe.cs`), and the exact-inventory checks (108 of 109 structural, 162 of 165 painter assertions ran). Corrected N11 also passed. `NEGATIVES_DONE 2 failed 0`.

## F-1 through F-8 disposition

| Finding | Disposition | Evidence and limit |
|---|---|---|
| F-1 diagram identity | ADDRESSED | Both diagram files and Objectives pinned to fixed #296 commit/blob/SHA-256 at historical commit, HEAD and working tree. Fresh one-byte/layout/false-text/correlated negatives rejected by intended pins. Composite full-mode result above. |
| F-2 weakened checker detection | ADDRESSED | All 29 integrity controls passed, including reviewer-written W1–W5 (neutered #303 pin, removed hashes, partial stage, directory allowance, deleted guard/mutations) and layered L1–L5 (pins and comparisons both off); each was rejected by the byte pin or independent comparison. |
| F-3 broad allowlist and assertion loss | ADDRESSED | Exact six-file folder inventory and exact changed-path allowlist. Fixed 182 source names/order, original obligations, 109 structural, 165 painter and 29 unchanged mutation tuples. Unknown/binary/test cases and layer-two control results reject weakened checks. |
| F-4 ignored production/index state | ADDRESSED within declared boundary | Assets/Packages/ProjectSettings ignored-file checks and non-normal index flags fail closed. New N08/N09/N10 isolate detector behavior. The unchanged #303 checker alone still lacks this protection; invoke the combined gate. |
| F-5 partial-mode full PASS | ADDRESSED | `--scope-only`, `--no-mutations` and `--skip-clean --skip-controls --only 303a` runs all exit 2 with SCOPE_ONLY/INCOMPLETE/PARTIAL markers and never the PASS marker; the verdict table was checked over 192 combinations. |
| F-6 edited call site/error handling | ADDRESSED | Presence check prevents missing-call `.index` exception. N11 exits 1 with named failures and suite FAIL; unexpected parser errors also fail closed through the named internal-error path. |
| F-7 #303 output overwrite | CARRIED, non-blocking under packet | Unchanged #303 default output may overwrite historical output. Every review call explicitly chose a fresh output path; frozen #303 bytes were not edited. |
| F-8 future commits/integration | CARRIED, intentional boundary | Exact stage scope rejects unrelated future paths, including this report if run at the docs report HEAD. Qualification belongs to frozen candidate 9789369d, not its later docs descendant. Integration requires a newly reviewed pinned stage. |

A17/L5 are deliberately malicious resealed checker controls. A checker run alone is **not** a trust boundary: resealing its self-hash can still permit a false PASS. The independently invoked integrity suite and an external comparison to the hashes above are essential. Its byte pin and independent name/tally comparisons must be reviewed with the test code. Coordinated malicious rewriting of runner, test and inventory is not claimed impossible. Source-level review plus externally fixed candidate identity is the boundary here.

## Conformance and remaining gates

B-1 selection remains `lesson == 0 && rackV2World`, with old constructor default false and the single existing eligibility flag passed from Objectives. M-1 fit guards and original no-scroll tests are unchanged; M-2 real-source painter probe and its original falsifications are preserved; M-3 confirmed/pending wording and shape distinctions are unchanged. These are source/stand-in conclusions only. No new production defect or assertion weakening was found in this four-file repair.

Remaining limitations: Git replacement/config/filters/alternates/environment attacks, wider hostile filesystem behavior, chmod-only edits and intentionally coordinated multi-file forgery are not comprehensively qualified. The #308 handoff is exact-path UTF-8 text but unpinned. Missing-archive full execution was not newly reproduced here; source logic and the integrity verdict-table check cover that condition. The historical T2 38/1 and design-JSON mismatch remain carried reds, not newly green. Historical #306 process nonconformance is not erased by this repair or review.

**NOT RUN:** Unity import/compile; actual scene/painter pixels or fonts; 720p/1080p fit/no-scroll, colour/grayscale comprehension; native input; real Legacy40/Reference Future save/runtime behavior; MFM; #287; integrated qualification; Player/Windows build; player acceptance. No production source, scene, asset, package, save or protected reference was edited.

## Handoff and decision boundary

**Reviewer verdict: NO BLOCKING FINDINGS** for the frozen source-verifier candidate `9789369d`, at Medium effort and within the source-only boundary above. Recommendation to Manager #200: eligible for acceptance as a source-verifier repair. Acceptance itself remains the Manager's decision.

Only this report is committed on the docs review branch; feature branch remains frozen. Manager #200 must separately inspect this result, decide acceptance, reconcile #308/#200/live coordination/dashboard and authorise any next gate. This reviewer does not own `active-tasks.md` and did not rewrite it or generated dashboard state. Closing #310 means review execution handed off, not product approved.

**Evidence:** `/media/jer/Files/CCF/Verification/D072-308-IndependentReview-20261106/`; four top-level logs, per-case integrity logs, `independent-audit.json`, `assertion-comparison.json`, independent negative scripts/logs, correction note and preflight receipt. Final branch SHA is in the #310 HANDED-OFF comment, avoiding a self-referential commit hash.

**DASHBOARD SYNC: NOT AVAILABLE. PUBLISHED DASHBOARD REFLECTS THIS TASK: NOT VERIFIED.** Operational follow-up: Manager must update its exclusively owned authoritative coordination record, run/allow the supported ingestion/publication path, and verify the automatic audit and published readback. This is an explicit outstanding closure issue, not a completed dashboard sync claim.
