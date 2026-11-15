# MULTI-013: docs-only repair of the four-file Sitka P1 adult-rules DRAFT for review findings R-1…R-10

**Date:** 15 November 2026
**Claim:** [#536](https://github.com/jermur788/CCF/issues/536)
**Binding packet:** `docs/multi-013-p1-draft-repair-dispatch-20261115-v1` @ `7b9dc1b2`, `Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-013-SITKA-P1-DRAFT-R1-R10-DOCS-REPAIR-PACKET-20261115.md`
**Ruling repaired against:** [Manager MULTI-012 NEEDS CHANGES decision](https://github.com/jermur788/CCF/blob/docs/manager-multi-012-p1-draft-needs-changes-20261115-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MANAGER-MULTI-012-P1-DRAFT-NEEDS-CHANGES-AND-REPAIR-DECISION-20261115.md) and the [MULTI-012 review](https://github.com/jermur788/CCF/blob/review/multi-012-sitka-p1-four-file-draft-spec-20261115-v1/Docs/Handoffs/MultiRack-Simulation-20261114/MULTI-012-P1-ADULT-SITKA-FOUR-FILE-INDEPENDENT-SPEC-REVIEW-20261115.md)
**Status:** **DRAFT REPAIRED — NOT APPROVED — NO CODE.** The next step is a focused independent R-1…R-10 delta review.

## 1. Worker, host, worktree and preflight

| Item | Value |
|---|---|
| Actual worker | Anthropic **Claude Opus 5.5** (`claude-opus-5-5`), Claude Code harness inside local T3 Code. The packet recommended Codex Sol High, but the user dispatched this task directly to this session. |
| **Independence disclosure** | The same Claude Opus 5.5 lineage wrote the MULTI-012 review that raised R-1…R-10, so this repair is by the findings' author, not by the DRAFT's author (GPT-6). **The delta review therefore must not be done by Claude Opus.** A different provider or model, for example Codex Sol High, should check whether these repairs are adequate. The GPT-6 draft author would also not be independent. |
| Host | `jer-laptop` (Linux), user `jer` |
| Worktree | `/media/jer/Files/CCF/Operations/CCF-MULTI-013-SitkaP1-DraftRepair-20261115`. It was absent beforehand, and I created it for this task. |
| Branch / starting HEAD | `docs/multi-013-p1-draft-r1-r10-docs-revision-20261115-v1` @ `50178f9df0641a75547fbeb29033d18a69a5cbb7`. This equals the remote and the packet pin. Status was clean. |
| Registration | #536 was fresh-read as `[READY / NOT RUNNING]`. Live `origin/main` (`fc18190b`) `active-tasks.md` had no MULTI-013/#536 row and no Sitka P1 lock, and no other worktree used the path or branch. I posted [RUNNING](https://github.com/jermur788/CCF/issues/536#issuecomment-6892550742) and retitled the issue **before** inspecting or editing. No Unity lease was needed or taken. |
| Files touched | Four existing DRAFT files under `specs/multirack-sitka-p1-adult-rules-comparator-v1/` (modified) and this handoff (added). Nothing else. |

## 2. Per-finding crosswalk

| Finding | Where repaired | What changed |
|---|---|---|
| **R-1 MAJOR** numeric reference / C-2 / Unity scheduling | **spec** §3 "Numerical reference of record", §5 C-2a/C-2b, §6 item 2 (P1-01 route, owner scheduling gate), §6 item 4, crosswalk m-4 row, §7 C-2a/C-2b/C-6 rows, §7.1, AC-P1-02/03. **plan** §2 diagram and P1-01 ownership, §3 G3/G4, §4 items 2 and 6, §5 scalar baseline and numeric equality. **tasks** P1-01 and P1-02 rows, §1 decision list, §3 V13 note, §4 numeric bullet, §7 m-4 row. **requirements** REQ-C01/C02/C03/F01/F03, §H C-2a/C-2b/C-6. | The reference of record is now a **pinned Unity Editor (Mono) per-ID capture**. A .NET harness may be used only after it reproduces the same capture vectors. C-2 is split into **C-2a** (before capture: units, exact inputs, **raw float-bit recording**, identity fields including source commit and Editor version, serialisation and display rounding) and **C-2b** (after capture: exact bits where shown achievable, else reviewed tolerances with branch margins and identical death/plan/selection IDs). C-2b is explicitly **not** a P1-01 prerequisite. "Binary32 = exact parity" is rewritten as a **hypothesis**. The integer mortality roll is marked runtime-portable; CI, DBH, height, crown and RD/p are marked float-sensitive. An **owner-level scheduling gate** for an exclusive Unity lane (competing with Scenario One) is added, and the text states that no machine or lock is booked. |
| **R-2 MAJOR** split-run equivalence vs approval history | **spec** §4 C-1A (new approval-binding sub-decision) and new "Split-run equivalence" bullet with Mode A/B, §6 item 5, §7 C-1 row, §7.1, AC-P1-05. **plan** §3 G5, §4 item 1, §5 Replay. **tasks** P1-03 row, §4 approval bullet and replay bullet with negative cases. **requirements** REQ-B04, REQ-E03, REQ-E06, §H C-1. | Biological and work-outcome equivalence is now separated from raw event-chain hash equality, which holds only for identical histories. **Mode A** (no open APPROVED/PENDING_WORK plan): identical canonical hashes as well, allowing only for an approved, named load event. **Mode B** (open plans): under C-1A Option 1, exactly one `PlanStale(SessionLoaded)` per affected plan, with the lifecycle compared by named event semantics, and raw hashes expected to differ. **C-1A owner choice** added: reuse P0's session-epoch binding (`LabSession.cs:69–70`, `:75–85`, `:144–149`, `:221–225`) or a distinct reviewed P1 policy. Silently persisting approvals is rejected, and P0 is unchanged. |
| **R-3** rendering | All four files | All backslash escapes before backticks removed: spec 82, plan 28, tasks 32, requirements 34. Nothing else in those lines changed. The plan diagram is a real fenced `text` block again. Render evidence is in §5. |
| **R-4** gate IDs | spec end-of-§8 crosswalk; plan §3 crosswalk and gate labels `G1 = P1-S0` … `G7 = P1-05`; tasks crosswalk, row labels, §1 note, readiness line; requirements §H note | `P1-G0` renamed to **`P1-G2`**. tasks l.17's "G0 … BLOCKED" now says P1-G2 and states that plan G0 is a different, completed gate. `grep` finds no `P1-G0` left, and `G0` now appears only as the completed evidence gate. |
| **R-5** decision timing and owners | New **spec §7.1** table; plan §4 intro; tasks §1 list and P1-01 dependency cell; requirements §H link | Gives each decision's "must be decided before" point and owner level: C-1A and C-6 are product-owner decisions; C-2a before P1-01; C-2b after the Unity capture; C-3, C-4 and C-5 geometry before fixtures; C-5 SI/H before P1-04; C-7 before user-visible output, with the owner approving wording; C-1 format before P1-03. **All seven remain OPEN**, and none is a circular prerequisite of P1-01. |
| **R-6** stale verifier | spec §3 last source bullet, §6 item 1, §7 C-6; plan §2 P1-01 owner, §4 item 6; tasks P1-01 row (repair removed from its work cell), §3 "No weakened historical verifier"; requirements REQ-F02 | The `version==23` repair is now a **separate optional/conditional** task. It gates P1-01 only if the capture reuses that harness, and a new capture harness need not wait for it. Weakening the assertion is forbidden, and no harness is edited here. |
| **R-7** oracle configuration and causal order | spec §3 hazard bullet (pre-growth CI, post-growth RD) and clamp sentence, §5 C-4, §6 item 2 configuration contract; plan §4 item 6, §5 oracle cases; tasks C06, new C11 (clamps) and C12 (configuration); requirements REQ-C09 (reworded), new REQ-C12, REQ-F01 | Specifies a valid regeneration route (model 2 throws without `ScenarioOneManager`, `ForestEcologyController.cs:483–486`); no spawner recruitment (`PromoteCohorts`, `:1699–1703`); storm 0 (`:472`); pinned RNG model and seed; an explicit `ecologicalYear` mapping (it increments before the roll, `:467`/`:1103`, and is independent of P0 `inventoryYear` 2026); Mature/Reference/no pruning; and a one-year configuration A/B proving adult-output neutrality with no production-code change. Mortality uses the **pre-growth CI** (`:475`, `:1088–1092`) and **post-growth RD**. Clamps are DBH `[1,200]` cm, height `[0.1,60]` m and crown floor 0.1 m (`ForestTree.cs:626–627`, `RelaxCrownRadius`). Nothing is executed. |
| **R-8** independent approval | spec header "Approval separation" and §7 approval paragraph; plan §3 G2; tasks P1-G2 row; requirements REQ-A07 | The text now says plainly that the GPT-6 Manager authored the DRAFT and may coordinate but cannot self-approve. Approval needs an independent review plus delta review **and** an explicit product-owner decision. |
| **R-9** P0 lineage and shared ignore file | spec header "P0 lineage" and §4 shared-file note; plan §2 paragraph; tasks P1-02 and P1-03 dependency cells; new requirements REQ-B07 | States that `main` `fc18190b` has 0 paths under `Tools/MultiRackSitkaLab/`, against 52 at `2659db99`. P0 integration lineage and the P1 base branch must be decided before P1-02. The `.lab-p1/` ignore entry would edit the **P0-owned** `.gitignore`, so it needs explicit ownership or an isolated alternative. No P0 edit is authorised. |
| **R-10** editorial | spec crosswalk preamble (DRAFT vs OPEN defined), §5 C-5 (`oracle-quarter-ha`), AC-P1-02 typo | DRAFT and OPEN status meanings are now defined. The text notes that `oracle-quarter-ha` is a 50 m square in corner coordinates and a re-centring candidate. The typo **`current-CFF` was fixed**; see the discrepancy note below. |

**`current-CFF` discrepancy (R-10).** The Manager ruling and packet say a fresh fetch of the pinned DRAFT shows `current-CCF`, and warn not to manufacture a typo. I checked the exact pinned blob before editing:

```text
gh api "repos/jermur788/CCF/contents/specs/multirack-sitka-p1-adult-rules-comparator-v1/spec.md?ref=50178f9df0641a75547fbeb29033d18a69a5cbb7"
  -> blob sha 6e23feaf83503a3a6fd3fec57475f9a10580a788; line 126 contains "current-CFF"
```

The local checkout at `50178f9d` agrees. The typo was real at the pin, so it was corrected (AC-P1-02 now reads `current-CCF`). The Manager's readback was probably of a rendered view or a different string.

## 3. The two repaired major tests (as now written)

**R-1 parity test (AC-P1-02/03, REQ-C01–C03):**
- **Pass requires:** a Unity Editor capture under the approved configuration contract, with raw float bits, followed by a C-2b target reviewed **after** capture.
- **Fail or negative cases:** a .NET-only or Python-only oracle; a .NET harness that has not reproduced the Unity vectors; any C-2b tolerance that lets death sets, boundary-victim order, plan IDs, 8 m inclusion, RD 0.6/1 branches or `roll < p` outcomes differ.

**R-2 restart test (AC-P1-05, REQ-E06, tasks §4):**
- **Mode A** (no open plan at the split): biological and work-outcome equivalence, plus identical canonical hashes, allowing only for an approved named load event.
- **Mode B** (open plan, C-1A Option 1): biological equivalence; exactly one `PlanStale(SessionLoaded)` per affected plan; lifecycle compared by named semantics; raw hashes asserted **different, never equal**.
- **Negative cases:** an approval persisting across load without an approved C-1A policy; a missing or duplicated `SessionLoaded` stale event; any biological divergence after reload.

## 4. C-1…C-7 status, timing and owners

| Decision | Status | Needed before | Owner level |
|---|---|---|---|
| C-1A fork source + approval binding | OPEN | Scheduling P1-01 (if snapshots are in scope), else P1-03 | **Product owner** |
| C-1 format/extension/root | OPEN | P1-03 | Technical; shared `.gitignore` is a coordination decision |
| C-2a capture/serialisation | OPEN | P1-01 | Technical |
| C-2b parity target | OPEN — **cannot close before P1-01** | P1-02 acceptance | Technical + independent numeric review |
| C-3 population | OPEN | P1-01 fixtures | Technical; owner only for a non-parity mode |
| C-4 initial state | OPEN | P1-01 fixtures | Technical |
| C-5 geometry / SI-H | OPEN | Fixtures / P1-04 | Technical + science review |
| C-6 oracle route + Unity lane | OPEN | Scheduling P1-01 | **Product owner** + Manager coordination |
| C-7 labels | OPEN | Any user-visible output | **Product owner** approves wording |

The C-2a/C-2b split is a proposed gate-timing policy, not an adopted numeric policy. No default was adopted.

## 5. Markdown rendering evidence (GitHub GFM renderer)

Each edited file was rendered with `gh api -X POST markdown -f mode=gfm -f context=jermur788/CCF` from the final local bytes:

| File | `<code>` spans | Literal backticks outside code | Backslash-before-backtick sequences | `<pre>` blocks | Tables: rendered rows = source rows |
|---|---|---|---|---|---|
| spec.md | 98 | 0 | 0 | 0 | 5 tables: 7/9/9/11/5 = 7/9/9/11/5 |
| plan.md | 25 | 0 | 0 | 1 (diagram, alignment kept) | 2 tables: 5/10 = 5/10 |
| tasks.md | 27 | 0 | 0 | 0 | 3 tables: 5/8/9 = 5/8/9 |
| requirements.md | 34 | 0 | 0 | 0 | 1 table: 9 = 9 |

Before this repair, the MULTI-012 render check showed literal backticks and a collapsed, `<br>`-joined diagram. The new `../spec.md#71-when-each-decision-is-needed-and-who-owns-it-r-5` anchor follows GitHub's heading-slug rule for `### 7.1 When each decision is needed, and who owns it (R-5)`. The renderer API does not emit heading ids, so I did not check that anchor in a browser. The existing plan link to `spec.md#7-draft-decision-register-c-1c-7--not-yet-adopted` is unchanged because that heading is unchanged. No requirement checkbox is checked (`grep -c '\[x\]'` = 0; 60 unchecked).

## 6. Evidence limits and what this task did NOT do

- The source line citations were re-read at `fc18190b` (CCF) and `2659db99` (P0 lab) during MULTI-012 and this task. They are **static source observations**, not runtime results.
- The 94/153 and 981/1,893 figures come from the MULTI-012 two-semantics **scratch probe**. They are **not Unity output**; no Unity, adapter or .NET parity has been established.
- Fixture and population figures (m-8) remain **synthetic reviewer arithmetic**.
- The topics MULTI-012 already qualified were not reopened: authority, adult equations, the model 2/model 1 route, the GROWFOR stand-level boundary, P0 immutability and the distinct P1 state. m-1…m-8 stay as they were, except that m-4 is marked superseded in part by R-1.
- **Not done:** no P1 code, no Unity Editor or Player run, no compilation or tests, no P0 source/fixture/spec edit, no edit to the MULTI-011 handoff or the review report, no `Assets/**`, `Tools/**`, `Docs/Project/**` or board write, no Drive or dashboard sync, no Windows build. **Dashboard/Drive sync: NOT VERIFIED.**

## 7. Next step

A **focused independent R-1…R-10 delta review** by a non-Claude-Opus, non-GPT-6-author reviewer. It should check:
- whether each repair matches the Manager ruling;
- that every C-item is still OPEN;
- whether the Mode A/B tests are falsifiable;
- the rendering.

Even an approving delta review would support only DRAFT QUALITY. Product-owner approval, the C-1A/C-6/C-7 owner decisions and any Unity scheduling are separate future gates. P1-01…P1-05 remain **NOT AUTHORISED**.

GitHub publication proof (readback of all five files and the four-modified/one-added compare against `50178f9d`) is in the #536 HANDED-OFF comment; this file cannot prove its own publication.
