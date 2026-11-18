# Sen-1 #783 — Independent full-package review of the #782 Compact30 canonical DRAFT

**Verdict: APPROVED — DRAFT DOCUMENT QUALITY ONLY.** Severity counts: **0 BLOCKER / 0 MAJOR / 4 MINOR / 8 NOTE.**

This verdict covers documentary quality of one frozen candidate. It does **not** approve the spec, gameplay, a canonical promotion, a merge to main, source, Unity, world content, W §12, save activation or any implementation. It does not check or pass any criterion. Manager #731 decides separately. **No self-approval:** I wrote none of the reviewed files. **No implementation approval:** nothing here authorises source or Unity work.

## 1. Reviewer, independence and target

| Field | Value |
| --- | --- |
| Reviewer | Claude Opus 5.5 (`claude-opus-5-5`), Anthropic, Claude Code harness (Claude Agent SDK) inside T3 Code. Session id not exposed to me; the harness scratchpad id is `51a7c1a1-dd1b-414e-9668-63671f224b2b` (observed, not a verified session id) |
| Claim | [#783 comment 6992711553](https://github.com/jermur788/CCF/issues/783#issuecomment-6992711553), `[RUNNING] #783 independent full review`, posted and read back before any review write |
| Independence | Different provider, model and session from the #782 author (OpenAI gpt-6.1-sol, Codex harness). **Not model-independent of #775:** the #775 changed-lines recheck was also done by Claude Opus 5.5, in a separate session. Finding m-3 below corrects a #775 note |
| Frozen target | `e823f356a0f65d3d6cd94a1006924aa5938a2ea6` (result commit), parent `1399355fe4fffd7c6dc7dcccc1fbf9795b1c8635` (content commit). `git ls-remote origin` shows `refs/heads/docs/sen-1-782-compact30-canonical-draft-stage-20261118-v1` = `e823f356…` |
| Frozen base | `fcfd43c31b60f92277470aef9c490e92fa59cf3b` (#617). Remote `docs/sen-1-617-616-draft-approval-readiness-20261116-v1` still = `fcfd43c3…`; #774 `docs/sen-1-774-compact30-draft-corrections-20261118-v1` still = `d34fa89a…` (frozen branches untouched) |
| Other pins read | #774 `d34fa89a`; #775 `3faa2fbd`; #771 `d043efe1`; Manager #731 adjudication and #782 packet at `2f17e073` (blobs `8d63d60c…` and `30c306da…`, identical at the spec's link pin `792f9544`); signed #677 `bfa01f86` (I re-hashed the file: blob `bb9327fe6c9b1ae1f7d5888e385dc9edc3ad072b`); signed save V2 §4.2 `200add4e`; Decision Log D-074, D-077, D-083…D-091 and S1-D status on `origin/main` `a69b3e2e` |
| Method | Read-only `git show <sha>:<path>` / `git diff`. No checkout of the author branch. Two throwaway verifiers in my scratchpad (not tracked). Review branch `review/sen-1-783-canonical-draft-full-review-20261118-v1` from `origin/main` `a69b3e2e`; worktree `/media/jer/Files/CCF/Operations/CCF-783-CanonicalReview-20261118` |

## 2. Commands actually run and their output

```text
$ git rev-parse HEAD; git status --porcelain     # my worktree, before writing
a69b3e2e90829505c792d95f5a3eba3e7d8d8132
<empty>
$ git log --oneline fcfd43c3..e823f356
e823f356 docs: record Sen-1 #782 staging reconciliation and static evidence
1399355f docs: stage Sen-1 #782 Compact30 canonical DRAFT consolidation
$ git diff --name-status fcfd43c3 e823f356
A  Docs/Handoffs/ScenarioOne-Compact-Ecology-20261118/Sen-1-782-CANONICAL-DRAFT-STAGING-AND-SOURCE-RECONCILIATION-RESULT-20261118.md
M  specs/scenario-one-compact-complete-loop-v1/checklists/requirements.md
M  specs/scenario-one-compact-complete-loop-v1/plan.md
M  specs/scenario-one-compact-complete-loop-v1/spec.md
M  specs/scenario-one-compact-complete-loop-v1/tasks.md
M  specs/scenario-one-compact-initial-planting-v1/checklists/requirements.md
M  specs/scenario-one-compact-initial-planting-v1/plan.md
M  specs/scenario-one-compact-initial-planting-v1/spec.md
M  specs/scenario-one-compact-initial-planting-v1/tasks.md
$ git diff --numstat fcfd43c3 1399355f     # content commit
295 61  …complete-loop-v1/checklists/requirements.md
49  124 …complete-loop-v1/plan.md
109 145 …complete-loop-v1/spec.md
39  130 …complete-loop-v1/tasks.md
10  15  …initial-planting-v1/checklists/requirements.md
10  15  …initial-planting-v1/plan.md
10  15  …initial-planting-v1/spec.md
11  16  …initial-planting-v1/tasks.md
$ git diff --numstat 1399355f e823f356     # result commit
391 0   Docs/Handoffs/…/Sen-1-782-CANONICAL-DRAFT-STAGING-AND-SOURCE-RECONCILIATION-RESULT-20261118.md
$ git diff --check fcfd43c3 e823f356 && echo CHECK_CLEAN
CHECK_CLEAN
$ git merge-base --is-ancestor fcfd43c3 e823f356 → yes
$ git merge-base --is-ancestor a69b3e2e e823f356 → NO (main not merged in)
$ git merge-base --is-ancestor e823f356 origin/main → NO (target not in main)
$ git merge-base --is-ancestor fcfd43c3 origin/main → NO (base itself not in main)
$ git diff --stat fcfd43c3 e823f356 -- specs/scenario-independent-save-isolation-v1 Docs/Handoffs/ScenarioOne-Save-Orchestration-20261117
<empty>   (signed §4.2 and #677 paths unchanged)
$ git hash-object <(git show bfa01f86:…/Sen-1-677-MANAGER-COMPACT-CGTRIG-AND-SAVE-POLICY-SUPPLEMENT-20261117.md)
bb9327fe6c9b1ae1f7d5888e385dc9edc3ad072b

$ python3 -I v783.py     # my verifier, reads objects with git show
<8 spec files + result>: checked=0 in every file; unchecked boxes: parent requirements 191, child requirements 33, all others 0
R   base 115 tgt 115 identical True ordered True allunchecked True
CIP base 33  tgt 33  identical True ordered True allunchecked True
CL defs 76 seq True unchecked True; CL definitions outside parent requirements: none
CL refs max: spec 050, plan 075, tasks 076, requirements 076, result 076 (no CL-077+)
overlay CL defs at #774 = 76; CL-001…070 identical True; CL-071…076 each SAME as #774
rows 148 ordered True (R-001…R-115 then CIP-001…CIP-033)
Counter: retained unchanged 69, provenance updated 62, Manager process 13, deferred 4
result §4 snapshot rows identical to parent §H: True
total unchecked boxes in 8 files: 224
$ python3 -I links.py     # relative links + #fragments (GitHub slug) + pinned GitHub blob/tree links via git cat-file
links checked 79 problems 0
$ git grep -n -A5 "enum WorkItemStatus" a69b3e2e -- Assets
ScenarioManagementHistory.cs:125 public enum WorkItemStatus { Succeeded = 1, Failed = 2, Cancelled = 3 }
```

**Allowlist diff:** the eight paths above are exactly the packet's author allowlist (four parent files + four subordinate planting files, all present at `fcfd43c3`), plus the one named result. No `Assets/**`, `CCF-Ops/**`, `Sen-1-763-DRAFT-PACKAGE/**`, graphics, signed save/§4.2/#677, coordination, mirror or other path changed. Whitespace check clean. Main was not merged; the target does not claim it was.

## 3. Findings

### MINOR

**m-1 — Stale "current" status prose still contradicts the settled register in the active parent and the subordinate.**
*Where:* parent `specs/scenario-one-compact-complete-loop-v1/checklists/requirements.md:153` ("All other OPEN-C and proposed D-083 Compact-first-beta exception … are still unapproved"; "OPEN-C1 still requires first-use/teaching UX"). Subordinate `specs/scenario-one-compact-initial-planting-v1/spec.md:131` ("No such open item is silently closed by this Manager's hypothesis", listing OPEN-C1/C4/C5/C8 as incomplete), `…/plan.md:90-91` ("#425 must independently reconcile proposed D-083"; "Unresolved owner/save choices still required … OPEN-C1 … OPEN-C4 … OPEN-C5"), `…/tasks.md:22` and `:58` ("Resolve paused D-083 [proposed compact exception]").
*What is wrong:* these are not checkbox lines, so custody does not require them verbatim. They say the opposite of D-083 on main ("CONFIRMED DIRECT OWNER PRODUCT BASELINE") and of parent `spec.md` §11 (OPEN-C1/C4/C5/C7/C8 settled by D-084 and signed #677). The packet asked the author to "correct earlier stale candidate/draft banners that incorrectly say accepted D-083 is pending". The banners were corrected (`requirements.md:5`, child banners line 5 in all four files), but these body paragraphs were left. Plan `:14` and tasks `:54` make "no residual P0 contradiction" the exit test.
*Why it matters:* a reader who lands mid-file sees two current statuses. The blanket banners ("historical provenance only where superseded") mitigate it, which is why this is MINOR and not MAJOR.
*Smallest fix:* prefix each listed paragraph or table cell with "Historical #617 status — superseded by parent §11:" (or delete `requirements.md:153`, which is not a criterion). No checkbox changes.

**m-2 — #775 N-1 is explained, not made consistent: R-097 and R-039 still get opposite classes for the same mixed duty.**
*Where:* `requirements.md:378` (R-097 row, class "deferred"), `:320` (R-039 row, class "provenance updated"), notes at `:130` and `:276`; result `:49`.
*What is wrong:* both rows are mixed. Each has a deferred C6 overlay half and a retained Work Plan BA/volume/denominator/expected-vs-settled half. R-097's text is mostly the retained accounting duty. R-039 is classed by its retained half; R-097 by its deferred half. The added notes say plainly that the accounting duty survives through CL-050, so nothing is lost. But the Manager instruction was to classify R-097 "consistently with R-039", and the classes are still asymmetric.
*Why it matters:* someone closing "deferred" rows in bulk could skip R-097's accounting half. The adjacent note makes that unlikely.
*Smallest fix:* reclassify R-097 as "retained with design provenance updated" (counts become 63 / 3; update `requirements.md:431`, result `:70`, plan `:83`). Or keep it and add one sentence saying mixed rows are deliberately classed by their deferred half, then apply the same rule to R-039.

**m-3 — "Ill-typed" refusal is labelled a *derived* interpretation, but signed #677 says it directly.**
*Where:* `requirements.md:9`, `spec.md:71`, `spec.md:123`, result `:53`.
*What is wrong:* signed #677 (`bfa01f86`, blob `bb9327fe…`, line 36, the Policy B paragraph) says: "Always reject structurally duplicate/unknown/foreign group/order/juvenile IDs, unknown keys, **wrong types**, invalid required field shapes …". So refusing an ill-typed key is signed text, not a derived reading. #775 N-5 (also Claude Opus 5.5) wrongly said "'Ill-typed' is not literal text there", and #782 carried that into the canonical package. The requirement itself is correct (CL-073/074 refuse before mutation). Only its evidence class is understated.
*Why it matters:* evidence-class honesty is a review criterion. Calling a signed rule "derived" invites a later reader to treat it as negotiable.
*Smallest fix:* replace "derived raw/typed structural interpretation, not a new signed literal" with "signed #677 §1 ('wrong types'); no new key or wire literal" in the four places.

**m-4 — Removing the #617 prose overlays dropped two duties that are not preserved anywhere in the package.**
*Where:* base `fcfd43c3:specs/scenario-one-compact-complete-loop-v1/spec.md:60` (WORLD-1) and `:48` ("Owner decision versus implementation"); target parent `spec.md` §3 (`:25-29`) and §12 (`:129-179`).
*What is wrong:* (a) the WORLD-1 caution "Do not repurpose either old enum/identity or switch the global new-game default [`StandGeometryPolicy.NewGameModel`] for every scene" is absent from all eight target files (`NewGameModel` count: base 1, target 0). R-090 and the §12 geometry row cover fallback, but not the global-default switch, which would silently change old-world New Game. (b) The owner's accepted world/visual directions in base `:48` (W worn-earth path, neutral daylight, restrained edge context, H1 "without fresh stumps") and D-085 OD-02 (deterministic tree-ID Sitka variation) and OD-06 (open, minimally furnished H1, no permanent trucks/equipment) are not restated in the parent. `neutral daylight` and `restrained edge` appear nowhere in the eight files; `stumps` appears only in child plan/requirements. Parent §3 lists world contents but omits these. §1 cites "D-085 seventeen … choices" only generically.
*Why it matters:* item 10 of the brief asks where each removed duty went. These two have no home. The world items are owned by #764, which softens (b).
*Smallest fix:* add one bullet to §12 restoring the WORLD-1 global-default caution. Add one sentence to §3 listing OD-02/OD-05 (no visible stumps)/OD-06 and D-090 neutral daylight/worn-earth as owner directions to be carried by the #764 manifest.

### NOTE

- **N-1 — S1-D status string.** `requirements.md:255` (CL-075, verbatim #774) still embeds the abridged status. The full status is given at `:7`, `spec.md:15`, `:47`, `:114`, `plan.md:29`, `tasks.md:31`. Those match the Manager-prescribed wording exactly. The source line `a69b3e2e:specs/scenario-one-repeated-interventions-v1/spec.md:4` is upper-case, with a full stop before "NO T-04P". Same meaning, but not byte-identical. Acceptable, since CL text is frozen and the header overrides it.
- **N-2 — Linux-host provenance note lost.** Base `requirements.md:20` said "the Linux Editor host requirement comes from the 16 November owner direction, not from R-079's original wording". The R-079 row (`requirements.md:360`) cites only "D-083 Windows gate", and D-083 says "personal Editor test" with no "Linux". "Linux" is still stated as a duty (`tasks.md:5`, `plan.md:61`, CL-001), but its provenance is gone.
- **N-3 — R-079 class.** "No Windows build before personal test" is a substantive release negative, classed "Manager process". The §H preamble (`:276`) says process ≠ waiver, so this is acceptable.
- **N-4 — Controlling-source precision.** The R-015 row (`:296`) and similar "#677 §0 preserving signed …" rows point at #677. The explicit-foreign pre-mutation rule actually comes from signed save V2 C-25/H-05/SI-05 (`200add4e:…/spec.md:38,87,177`). The content is correct.
- **N-5 — #677 §4 JsonUtility caution.** #677 line 90 requires native qualification of "floating-point exact G1 site identity roundtrip (OD-12 zero planar placement is not a demonstrated JsonUtility bitwise equality)". The package has "exact stored x/z" (CL-019) and "same exact x/z persisted" (`tasks.md:28`) but does not mention the float/JsonUtility trap.
- **N-6 — Result-document understatements.** Result `:62`/`:280` say "CL-001…070 text byte-identical to #774". In fact all 76 are identical. Result `:66` says "CL-068 now requires #783". The CL-068 text is unchanged (it still says "#763"); the redirect lives in `requirements.md:7`. The author's verifier checks link files but not `#fragments`; my link check covered fragments (79 links, 0 problems).
- **N-7 — Graphics DRAFT location.** "Graphics untouched / not superseded" is trivially true: no Compact graphics spec path exists at `fcfd43c3`, `e823f356` or `a69b3e2e`. The package never says where that DRAFT lives. CL-067's "relevant 66 graphics requirements" therefore has no in-package anchor.
- **N-8 — Cosmetic/citation.** `plan.md:5` links the #731 parallel plan by mutable branch name. This is the #771 m-4 pattern; the other Manager links are SHA-pinned. `tasks.md:54` has no final full stop. `spec.md:15` has `approves.”**.`. Result `:242` has a stat line missing its leading space. The adjacent notes at `requirements.md:72` and `:130` split the checkbox list in rendering (harmless).

## 4. Brief checklist — what I verified

| # | Check | Result |
| --- | --- | --- |
| 1 | Diff scope = 8 existing paths + 1 result; no `Assets/**`; no main merge; frozen #617/#774 refs unchanged; `diff --check` clean | **PASS** (§2) |
| 2 | 224 tracked / 0 checked; 115 R + 33 CIP byte-identical to `fcfd43c3` and ordered; CL-001…076 sequential, unique, no CL-077+ | **PASS** |
| 3 | 148 §H rows complete, ordered, one class each, 69/62/4/13, controlling source real | **PASS**. Every controlling item cited (D-074, D-077, D-083, D-084 T1/W-A/F-A/M-A/C6/C10/Geometry A, D-085 OD-01/05/06/07/08/09/11/12/14/15/16/17, D-090, D-091, #677 §0/§2/§3/§4 and T-02/03/06/07/08) exists at its pin. Precision note N-4 |
| 4 | Every R/CIP row and every CL read against originals and policy | **Done** (§5, §6). No requirement silently dropped, weakened or duplicated in the checklist. m-2 and N-3 are classification points. m-4 is prose-level loss outside the criteria |
| 5 | Manager decisions M-3 / M-4 / M-5 | **PASS.** M-3: one parent, subordinate planting with in-place CIP, no third spec, graphics untouched, no supersession (`spec.md:3`, `:101`). M-4: `spec.md:37`, `:119`; `plan.md:36`; `tasks.md:32`. No gate/latch/save field/history credit/auto-replacement; death/felling/None-Fell consequences correct; timing/past credit/reselection OPEN; STOP to owner. M-5: `spec.md:47`, CL-075. No rack/five-rack/v24/new latch, synthetic year, hard gap or completion credit; full S1-D string retained (N-1) |
| 6 | Signed #677 fidelity | **PASS.** Closed three-root envelope (`spec.md:69`, CL-031). Present-and-false only (CL-036, `spec.md:71`). Missing/ill-typed/unknown refuse before mutation (CL-073/074; evidence class m-3). CT-3 all-route hold incl. reload (`spec.md:45`, CL-034, `:123`). Existing event9 + field atomic, no second control (`spec.md:45`, CL-033/035). Forward dual-ack gate (`spec.md:39`, CL-071/072). Pending/Approved/Completed/Failed + `OrderCancelled` / `WorkItemStatus.Cancelled` (CL-028; the enum exists in source). Approved immutability on all routes (CL-025/026). Max 4 active IDs, no Completed-slot replacement (CL-076). D-N1 OPEN with STOP/escalate (`spec.md:75`, `:89`, `:113`). D-077 one tick/frame and stop events (`spec.md:59`, CL-051) |
| 7 | Open items visibly open | **PASS.** OD-16 (`spec.md:65`, `:107`), M-A S1 persistence (`:49`, `:112`), world/W §12/digest/G1 coords (`:5`, `:28`, `:103`), save activation (`:69`), E1/E2 (`:81-83`), D-032 (`:91`, `:135`), physical Ops/Unity (`plan.md:18`, `:67`) |
| 8 | Old-world preservation | **PASS.** R-021…025, R-064, R-087, R-101, R-103, CL-010, CL-062, CL-065, `spec.md:69`, `:135`. Signed §4.2 bytes unchanged (empty diff). Old `CCF.Learning.v1.*` / `CCF.MenuHelp.v1.*` protected |
| 9 | Evidence-class honesty; D-083 provenance; #775 N-1/N-2 | **PASS with m-2, m-3.** Signed / static-reviewed (#676) / derived / calibration (€12,000, D-077 4/5) are separated (`spec.md:93`). D-083 provenance correct against main (`spec.md:5`). N-2 (R-049) fixed by an adjacent note (`requirements.md:72`); the text is accurate (D-083 confirms 30 m; no exterior trees/donors still required) |
| 10 | Internal consistency; links; removal of the #617 overlays | **PASS with m-1, m-4.** 79 links / fragments resolve. No CL definition outside the parent. Where the removed overlays went is in §7 |
| 11 | Result document truthful, no self-approval | **PASS with N-6.** File list, numstat, counts, SHAs, blob, NOT RUN list and absence from main all reproduce. It explicitly does not self-approve (result `:3`, `:385`) |

## 5. Per-item audit — all 148 inherited R/CIP dispositions

The "Result" column is my independent check of each row: the original line at `fcfd43c3` against the staged class, the controlling source at its pin, and the CL links. "OK" means the class is defensible, the cited source exists and says what the row claims, and no original duty or negative is lost.

| ID | §H class (as staged) | Reviewer check against original + policy | Result |
| --- | --- | --- | --- |
| R-001 | provenance updated | D-084 T1; #677 T-02 / §2 | OK |
| R-002 | Manager process | AgentWorkflow independent-review gate; #771 | OK |
| R-003 | Manager process | AgentWorkflow pre-write checks; D-091 | OK |
| R-004 | Manager process | D-083; D-084 | OK |
| R-005 | provenance updated | D-083; D-085 OD-05/09/14 | OK |
| R-006 | provenance updated | D-085 OD-15 | OK |
| R-007 | provenance updated | D-083; D-084 Geometry A; D-085 OD-09/12 | OK |
| R-008 | provenance updated | D-084 F-A/M-A; D-085 OD-16 | OK |
| R-009 | unchanged | Original parent R-009 | OK |
| R-010 | Manager process | AgentWorkflow Git/Drive synchronization | OK |
| R-011 | provenance updated | #677 §0/§2 preserving signed §4.2 | OK |
| R-012 | unchanged | Original parent R-012 | OK |
| R-013 | unchanged | Original parent R-013 | OK |
| R-014 | unchanged | Original parent R-014 | OK |
| R-015 | provenance updated | #677 §0/§3 preserving signed foreign-ID guards | OK; controlling source is really signed save V2 C-25/H-05/SI-05 (N-4) |
| R-016 | unchanged | Original parent R-016 | OK |
| R-017 | unchanged | Original parent R-017 | OK |
| R-018 | provenance updated | #677 §0/§4 strict structural mismatch | OK |
| R-019 | unchanged | Original parent R-019 | OK |
| R-020 | unchanged | Original parent R-020 | OK |
| R-021 | unchanged | Original parent R-021 | OK |
| R-022 | unchanged | Original parent R-022 | OK |
| R-023 | unchanged | Original parent R-023 | OK |
| R-024 | unchanged | Original parent R-024 | OK |
| R-025 | unchanged | Original parent R-025 | OK |
| R-026 | provenance updated | D-083; D-085 OD-05/06/08; D-090 | OK |
| R-027 | provenance updated | D-084 T1; #677 T-02/T-03 | OK |
| R-028 | provenance updated | #677 §2 designated planting group | OK |
| R-029 | provenance updated | D-083; D-085 OD-05 | OK |
| R-030 | provenance updated | D-085 OD-07/08/09; D-090 | OK |
| R-031 | unchanged | Original parent R-031 | OK |
| R-032 | unchanged | Original parent R-032 | OK |
| R-033 | unchanged | Original parent R-033 | OK |
| R-034 | provenance updated | D-084 W-A; #677 T-07/§3 | OK |
| R-035 | unchanged | Original parent R-035 | OK; non-E80 rack seams preserved at spec.md:141,147 |
| R-036 | unchanged | Original parent R-036 | OK; pinned MFM tests via CL-010/CL-062 |
| R-037 | provenance updated | #677 T-07/§3 | OK |
| R-038 | unchanged | Original parent R-038 | OK |
| R-039 | provenance updated | D-084 C6 deferral and M-A intent | OK; overlay half deferred, accounting via CL-050 |
| R-040 | provenance updated | D-084 F-A/T1; #677 T-06 | OK |
| R-041 | unchanged | Original parent R-041 | OK |
| R-042 | provenance updated | D-084 F-A/M-A | OK |
| R-043 | unchanged | Original parent R-043 | OK |
| R-044 | unchanged | Original parent R-044 | OK |
| R-045 | unchanged | Original parent R-045 | OK |
| R-046 | unchanged | Original parent R-046 | OK |
| R-047 | unchanged | Original parent R-047 | OK |
| R-048 | unchanged | Original parent R-048 | OK |
| R-049 | unchanged | Original parent R-049; D-083 provenance correction (#775 N-2) | OK; adjacent D-083 note requirements.md:72 |
| R-050 | unchanged | Original parent R-050 | OK |
| R-051 | provenance updated | D-084 M-A; D-085 OD-16 | OK |
| R-052 | provenance updated | D-084 M-A; D-085 OD-16 | OK |
| R-053 | provenance updated | D-074; D-084 M-A | OK |
| R-054 | unchanged | Original parent R-054 | OK |
| R-055 | unchanged | Original parent R-055 | OK |
| R-056 | provenance updated | #677 T-02/§2 | OK |
| R-057 | provenance updated | D-084 T1; #677 T-03 | OK |
| R-058 | unchanged | Original parent R-058 | OK |
| R-059 | provenance updated | D-077 | OK vs D-077 P-2/P-3 |
| R-060 | provenance updated | D-077 | OK vs D-077 stop list / one tick per frame |
| R-061 | unchanged | Original parent R-061 | OK |
| R-062 | provenance updated | D-084 M-A; #677 T-08 | OK |
| R-063 | provenance updated | #677 T-08 | OK |
| R-064 | unchanged | Original parent R-064 | OK |
| R-065 | provenance updated | D-083; D-085 OD-05/08/09; D-090 | OK |
| R-066 | unchanged | Original parent R-066 | OK |
| R-067 | deferred | D-084 C6 | OK |
| R-068 | deferred | D-084 C6 | OK |
| R-069 | unchanged | Original parent R-069 | OK |
| R-070 | unchanged | Original parent R-070 | OK |
| R-071 | unchanged | Original parent R-071 | OK |
| R-072 | provenance updated | D-085 OD-07/08/09/11; D-090 | OK |
| R-073 | unchanged | Original parent R-073 | OK; former #614 asset mapping kept |
| R-074 | unchanged | Original parent R-074 | OK; former #614 perf mapping kept |
| R-075 | unchanged | Original parent R-075 | OK |
| R-076 | provenance updated | D-091 | OK |
| R-077 | unchanged | Original parent R-077 | OK |
| R-078 | Manager process | AgentWorkflow review/integration gates | OK |
| R-079 | Manager process | D-083 Windows gate; AgentWorkflow release | OK; class arguable (N-3); Linux-host provenance note lost (N-2) |
| R-080 | provenance updated | #677 §0/§3 preserving signed isolation | OK |
| R-081 | unchanged | Original parent R-081 | OK |
| R-082 | Manager process | AgentWorkflow dashboard synchronization | OK |
| R-083 | provenance updated | #677 §0/§3 preserving signed three-profile registry | OK |
| R-084 | provenance updated | #677 §0 historical-policy preservation | OK |
| R-085 | unchanged | Original parent R-085 | OK |
| R-086 | unchanged | Original parent R-086 | OK |
| R-087 | unchanged | Original parent R-087 | OK |
| R-088 | unchanged | Original parent R-088 | OK |
| R-089 | provenance updated | #677 §0/§2 preserving signed §4.2 and SI-09 | OK |
| R-090 | provenance updated | D-083; #677 §0 preserving geometry reservation | OK |
| R-091 | provenance updated | #677 §0/§2 preserving signed §4.2 | OK |
| R-092 | unchanged | Original parent R-092 | OK |
| R-093 | provenance updated | D-084 T1; #677 T-02/T-03 | OK |
| R-094 | provenance updated | D-084 M-A; D-085 OD-16 | OK |
| R-095 | provenance updated | D-084 M-A; D-085 OD-16 | OK |
| R-096 | provenance updated | D-077 | OK |
| R-097 | deferred | D-084 C6; R-039 retained accounting duty | MINOR m-2: class asymmetric with R-039 |
| R-098 | provenance updated | D-084 W-A; #677 T-07 | OK |
| R-099 | unchanged | Original parent R-099 | OK |
| R-100 | Manager process | AgentWorkflow review gate; #771 | OK |
| R-101 | unchanged | Original parent R-101 | OK |
| R-102 | provenance updated | D-083; D-085 OD-17 | OK |
| R-103 | unchanged | Original parent R-103 | OK |
| R-104 | unchanged | Original parent R-104 | OK |
| R-105 | provenance updated | D-083; D-084 T1; #677 T-06 | OK |
| R-106 | unchanged | Original parent R-106 | OK |
| R-107 | provenance updated | D-084 Geometry A/T1; D-085 OD-09/12; #677 §2 | OK |
| R-108 | unchanged | Original parent R-108 | OK |
| R-109 | unchanged | Original parent R-109 | OK |
| R-110 | unchanged | Original parent R-110 | OK |
| R-111 | unchanged | Original parent R-111 | OK |
| R-112 | unchanged | Original parent R-112 | OK |
| R-113 | provenance updated | D-074; D-084 M-A; D-085 OD-16 | OK |
| R-114 | deferred | D-084 C10 | OK; D-084 C10 |
| R-115 | Manager process | D-083; D-084; D-085 | OK; verbatim stale D-083 text, row reconciles |
| CIP-001 | provenance updated | D-083; D-085 OD-17 | OK |
| CIP-002 | provenance updated | D-083; D-085 OD-14 | OK |
| CIP-003 | provenance updated | D-085 OD-05/15 | OK |
| CIP-004 | provenance updated | D-083; D-085 OD-05/06 | OK |
| CIP-005 | provenance updated | D-085 OD-08/09; D-090 | OK |
| CIP-006 | provenance updated | D-085 OD-05/15 | OK |
| CIP-007 | provenance updated | D-084 Geometry A; D-085 OD-09/12 | OK |
| CIP-008 | unchanged | Original child CIP-008 | OK |
| CIP-009 | unchanged | Original child CIP-009 | OK |
| CIP-010 | unchanged | Original child CIP-010 | OK |
| CIP-011 | unchanged | Original child CIP-011 | OK |
| CIP-012 | unchanged | Original child CIP-012 | OK |
| CIP-013 | provenance updated | D-085 OD-08/09; D-090 | OK |
| CIP-014 | unchanged | Original child CIP-014 | OK |
| CIP-015 | provenance updated | D-084 T1; #677 T-06/§2 | OK |
| CIP-016 | unchanged | Original child CIP-016 | OK; per-tree duty reinforced in child tasks.md |
| CIP-017 | unchanged | Original child CIP-017 | OK |
| CIP-018 | provenance updated | D-084 F-A | OK |
| CIP-019 | provenance updated | #677 T-06/§3 | OK |
| CIP-020 | unchanged | Original child CIP-020 | OK |
| CIP-021 | provenance updated | D-084 M-A; D-085 OD-16 | OK |
| CIP-022 | unchanged | Original child CIP-022 | OK |
| CIP-023 | Manager process | D-083; D-084; AgentWorkflow authority | OK; verbatim stale D-083 text, row reconciles |
| CIP-024 | Manager process | D-085 OD-15; AgentWorkflow evidence gate | OK |
| CIP-025 | Manager process | AgentWorkflow independent-review gate; #771 | OK |
| CIP-026 | Manager process | D-083 personal-test/Windows gate | OK |
| CIP-027 | provenance updated | D-083; D-085 OD-01/05 | OK |
| CIP-028 | unchanged | Original child CIP-028 | OK |
| CIP-029 | unchanged | Original child CIP-029 | OK |
| CIP-030 | provenance updated | D-085 OD-08/09; D-090 | OK |
| CIP-031 | provenance updated | D-085 OD-05/15 | OK |
| CIP-032 | provenance updated | D-083; D-085 OD-05/06/09 | OK |
| CIP-033 | provenance updated | D-084 T1/M-A/F-A; #677 §2 | OK |

## 6. Per-item audit — all 76 CL criteria

Every CL definition sits once in the parent checklist (`requirements.md:160-256`), unchecked. Each was compared with the #774 text (all 76 are byte-identical) and with D-083…D-091, the M-3/M-4/M-5 decisions and signed #677/§4.2.

| CL | Opening words (verbatim) | Result |
| --- | --- | --- |
| CL-001 | Accepted Compact baseline is exactly distinct 30×30m, no internal … | OK; byte-identical to #774; no conflict found |
| CL-002 | Real, owner-personally-signed versioned whole-world manifest/digest binds 175 candidate tree … | OK; byte-identical to #774; no conflict found |
| CL-003 | All 14 H1 source omissions are pre-world layout and … | OK; byte-identical to #774; no conflict found |
| CL-004 | D-085 all-Sitka unmarked start; zero pre-authored Oak/Beech/juveniles/seed donors/three CropTree … | OK; byte-identical to #774; no conflict found |
| CL-005 | Flat y=0 continuous 30m plane, no authored hydrology, 36 … | OK; byte-identical to #774; no conflict found |
| CL-006 | Native E1 walk: H1 start facing path; natural pedestrian … | OK; byte-identical to #774; no conflict found |
| CL-007 | Four authored G1 exact zero-tolerance site coordinates pass real … | OK; byte-identical to #774; no conflict found |
| CL-008 | First-party image/material/URP/script/render rights, exact W§12 / Tier S signed … | OK; byte-identical to #774; no conflict found |
| CL-009 | E1 is classified strictly environment-only, not E2 playable game … | OK; byte-identical to #774; no conflict found |
| CL-010 | The separately reviewed historical E80 Rack V2 / MFM/Stand … | OK; byte-identical to #774; no conflict found |
| CL-011 | H1 explicit arrival acknowledgement occurs only after real active … | OK; byte-identical to #774; no conflict found |
| CL-012 | Real current-world/year inspect panel opened/read and explicit assessment acknowledgement … | OK; byte-identical to #774; no conflict found |
| CL-013 | D-085 OD-17 calls for the player personally to choose … | OK as overlaid; "owner-decisions §C" resolved by requirements.md:7 + spec.md §11 C (M-4) |
| CL-014 | Crop-tree semantics must be explicitly settled through owner-decisions §C … | OK as overlaid; same redirect; no new gate/key |
| CL-015 | Real player can inspect actual individual Sitka and Oak/Beech … | OK; byte-identical to #774; no conflict found |
| CL-016 | All first lesson work remains real Pending Work Plan … | OK; byte-identical to #774; no conflict found |
| CL-017 | No no-rack new-world MFM prescription, sample, proposal, orange entrance … | OK; byte-identical to #774; no conflict found |
| CL-018 | Player can undo/correct permitted draft marks/orders without fake cash/biological … | OK; byte-identical to #774; no conflict found |
| CL-019 | Exactly four player-created designated G1 orders: 2 sessile Oak … | OK vs #677 §2 max 2/species, zero tolerance |
| CL-020 | Four positions are unique, bounded, within real ecology cells … | OK; byte-identical to #774; no conflict found |
| CL-021 | CIP-016 per-tree actor/method and optional shelter/protection are genuinely individually … | OK; byte-identical to #774; no conflict found |
| CL-022 | Player purchase, reservation, pending placement, order approval and actual … | OK; byte-identical to #774; no conflict found |
| CL-023 | Work Plan quotes actual selectable owner/contractor method, money/material/expected time … | OK; byte-identical to #774; no conflict found |
| CL-024 | All orders remain Pending before explicit player review/approval; before-approval … | OK; byte-identical to #774; no conflict found |
| CL-025 | Approved work fields cannot mutate directly by spray, individual … | OK; byte-identical to #774; no conflict found |
| CL-026 | To change Approved work, explicit user-confirmed audited cancellation removes … | OK; byte-identical to #774; no conflict found |
| CL-027 | One annual resolution settles each Approved outcome at most … | OK; byte-identical to #774; no conflict found |
| CL-028 | Work-order statuses Pending/Approved/Completed/Failed and actual cost/stock/time changes survive Compact … | OK; `WorkItemStatus.Cancelled` exists in main source (ScenarioManagementHistory.cs:125-130); #677 §3 no new status |
| CL-029 | Real original P0004… historical H1 omissions, site designer and … | OK; byte-identical to #774; no conflict found |
| CL-030 | Optional valid other work may coexist with G1, but … | OK; byte-identical to #774; no conflict found |
| CL-031 | Use signed §4.2 exact three-root save/profile shape and Compact … | OK; byte-identical to #774; no conflict found |
| CL-032 | Actual per-order success rows are exact linked eligible 2+2 … | OK; byte-identical to #774; no conflict found |
| CL-033 | Y=max completionYear for final valid 2+2; existing authentic report … | OK vs #677 T-02 |
| CL-034 | CT-3 hold blocks every UI, service, F-key, optional skip … | OK vs #677 T-03 |
| CL-035 | Repeat covering ack idempotent and opening/dismissal alone never credits; … | OK; byte-identical to #774; no conflict found |
| CL-036 | Signed Policy B: otherwise valid real Completed orders with … | OK vs #677 T-01 (present-and-false) |
| CL-037 | Real Completed designated order missing a positive teaching row … | OK vs #677 T-05/§4 |
| CL-038 | Forged/mismatched claimed planting rows, species/order/juvenile/year, structurally invalid IDs and … | OK; byte-identical to #774; no conflict found |
| CL-039 | Both #677 CT3 deadlock cases REFUSE: CT1 true with … | OK vs #677 T-05; D-N1 kept OPEN |
| CL-040 | Save/reopen each lifecycle point—arrival, assessment, 1–3 Pending, Approved pre-resolve, … | OK; byte-identical to #774; no conflict found |
| CL-041 | Every genuine annual advancement performs real growth/crown/light, seed/cohort/juvenile, competition, … | OK; byte-identical to #774; no conflict found |
| CL-042 | Real separately identified Oak, Beech and Sitka model depth … | OK; byte-identical to #774; no conflict found |
| CL-043 | Real G1 planted individual ID and species remain inspectable … | OK; byte-identical to #774; no conflict found |
| CL-044 | €12,000 is finite with no automatic grant, fabricated fee … | OK; byte-identical to #774; no conflict found |
| CL-045 | Genuine failed/declined/unaﬀordable/no-work, cancel and incomplete plan paths have correct … | OK; byte-identical to #774; no conflict found |
| CL-046 | S1-C prior-year/year-by-year and intervention history read-only without world mutation, … | OK; byte-identical to #774; no conflict found |
| CL-047 | S1-D re-entry can compare previously executed site/tree decisions with … | OK; byte-identical to #774; no conflict found |
| CL-048 | No-work is a legitimate player-chosen year/inspection state, not a … | OK; byte-identical to #774; no conflict found |
| CL-049 | Annual review must identify real source-driven financial, ecology and … | OK; byte-identical to #774; no conflict found |
| CL-050 | Work Plan comparison shows expected/settled costs, legal eligibility and … | OK; byte-identical to #774; no conflict found |
| CL-051 | D-077 normal year remains available under ordinary guards and … | OK vs D-077 |
| CL-052 | Distinct direct no-ack N-step state/hash oracle and UI clicked … | OK; byte-identical to #774; no conflict found |
| CL-053 | A true later spatially traceable assessed retention/release/stand treatment after … | OK; byte-identical to #774; no conflict found |
| CL-054 | The player subsequently re-inspects current changed forest/three species and … | OK; byte-identical to #774; no conflict found |
| CL-055 | After owner-approved OD-16 classifier, evidence record proves actual distinct … | OK; byte-identical to #774; no conflict found |
| CL-056 | Initial four G1 plantings, any shelter, pruning, H1 pre-world … | OK; byte-identical to #774; no conflict found |
| CL-057 | No automatic requirement for 3+ separate substantial interventions, 3 … | OK; byte-identical to #774; no conflict found |
| CL-058 | Completion shows once with specific truthful evidence and derivation/provenance, … | OK; byte-identical to #774; no conflict found |
| CL-059 | Missing S1 schema or owner threshold cannot be filled … | OK; byte-identical to #774; no conflict found |
| CL-060 | Completion, save/reopen, continued observation/management and no duplicate reward/history all … | OK; byte-identical to #774; no conflict found |
| CL-061 | User personal playable Editor test judges physical judgement/teaching and … | OK; byte-identical to #774; no conflict found |
| CL-062 | Historic Legacy40, Enlarged80/five-rack/MFM, frozen Reference Future v1 (world/schedule/identity) and … | OK; byte-identical to #774; no conflict found |
| CL-063 | Wrong Compact/legacy/O2 profile or forged ID, invalid geometry/version/duplicate or … | OK; byte-identical to #774; no conflict found |
| CL-064 | Compact primary, tmp and bak isolated; explicit same-slot backup … | OK; byte-identical to #774; no conflict found |
| CL-065 | Original model0/pure-Sitka numeric and frozen reference anchors validated exact, … | OK; byte-identical to #774; no conflict found |
| CL-066 | E1 independent signature/render/collision/status checked separately from E2 gameplay; #764/#718 … | OK; byte-identical to #774; no conflict found |
| CL-067 | Original R-001…R-115 and CIP-001…CIP-033 and relevant 66 graphics requirements … | OK; byte-identical to #774; no conflict found |
| CL-068 | Separate nonauthor independent review of this new #763 DRAFT … | OK as overlaid; text still names #763, requirements.md:7 redirects to #783 (N-6) |
| CL-069 | Tier1 focused, Tier2 feature and Tier3 one integrated candidate … | OK; byte-identical to #774; no conflict found |
| CL-070 | Actual manager handoff, no branch/main/board/source-overlap, dashboard ingestion/Drive mirrors verified … | OK; byte-identical to #774; no conflict found |
| CL-071 | Positive forward G1 designation: with BOTH arrivalAcknowledged and assessmentAcknowledged … | OK; byte-identical to #774; no conflict found |
| CL-072 | Negative forward G1 designation: test each acknowledgement false independently … | OK; byte-identical to #774; no conflict found |
| CL-073 | Missing mandatory arrival/assessment key or an ill-typed acknowledgement key … | OK; "ill-typed" is signed, not derived (m-3) |
| CL-074 | Any other missing mandatory key, ill-typed signed-type key or … | OK; same as CL-073 |
| CL-075 | Manager implementation wording for Compact S1-D: signed T1 true … | OK vs M-5/D-074; embedded S1-D status abridged (N-1) |
| CL-076 | A Failed or cancelled designated member may be replaced … | OK vs #677 §3/§4 |

## 7. Where the removed #617 prose overlays went

The target dropped these parent blocks present at `fcfd43c3`: the requirements "#614 Evidence overlay" and "#600/#604 witness guide" (base `requirements.md:10-42`), the "#576 cross-spec register" sections in all four parent files, spec "#614 core-first", "#600/#604 bridge" with SCENE-1/WORLD-1/LOOP-1/SAVE-1 (base `spec.md:10-83`), base spec S-01…S-11, S-13…S-17, S-20, S-21, and the plan/tasks #614/#600 schedules and risk lists. For each, the duty is either restated in the target or the original R/CIP checkbox still carries it:

| Removed duty (base location) | Where it is preserved at `e823f356` | Status |
| --- | --- | --- |
| #614 core physical scene: signed H1/14 omissions/8 buffers/route/G1, capsule/guard collider, O1 0.141 m / O2 0.155 m / B2, no collider suppression (base req `:12`) | `spec.md:137-139` (relabelled historical original-polyline per #767 m-2), `plan.md:77`, CL-002/003/006/007, R-005…007/R-026…030 rows | Preserved |
| #614 true teaching/forestry (base req `:14`) | CL-019…057; spec §4 C1–C8, §7 | Preserved |
| #614 shared admission: #544 single registry, no scene-bound `CanLoad`, ISO-07/08/42/44/48/54/55 + ISO-15, ISO-58 BLOCKED (base req `:16`) | `spec.md:141`, `:145`, `:151`; `plan.md:73-75`; R-011…025/R-083…092 rows | Preserved |
| **R-035/036** non-Enlarged80 rack-seam refusal, no Compact bypass (base req `:16`) | `spec.md:141` (#606), `:147` (#604 rack correction); rows R-035/R-036 → CL-010/017/062 | Preserved (not tied to the IDs by name) |
| Asset/quality mapping: R-070/071 stage-accurate, **R-073** provenance/licence/LOD, **R-074** measured performance, reuse only after rights inspection, 0.6 m placeholder, final art not a core blocker, protected materials, D-032 open (base req `:18`) | §H rows R-070…R-074 (`requirements.md:351-355`); `spec.md:133-135`; `plan.md:79`; child `spec.md` #614 block (Plantation02 reuse) | Preserved |
| Test and user gate: R-076/077 Tier 1/2/3 and lease, R-079 personal test before Windows, R-081, R-082 (base req `:20`) | Rows R-076…R-082; `plan.md:54-63`; CL-069/070 | Preserved; **Linux-host provenance sentence lost (N-2)** |
| #600/#604 startup/scene witnesses (neutral index-0 boot, boot-only picker, one persistent host, profile-bound Restart, F5/F9/F10, SavePath, no buildIndex identity) (base req `:28`, spec `:38-46`) | `spec.md:143-151`; `plan.md:73`; `tasks.md:48` | Preserved |
| #604 world/map: no headland/approach injection, geometry-keyed rack seams, StandMap path source (base spec `:42`) | `spec.md:147` | Preserved |
| #604 completion/finance: `TargetAreaFactor` throw/rounding, batch planting setters vs CIP-016, €20/€20/€30 (base spec `:44`) | `spec.md:149`; `tasks.md:49`; child `tasks.md` CP-S3 | Preserved |
| SCENE-1: proposed scene path, never clone ForestTest planks/2.6 m route, EditorBuildSettings fact, Restart buildIndex, `CCF_TEST_DIRECT_ENTRY` (base spec `:52-56`) | `spec.md:135` ("Do not inherit old track geometry"), `plan.md:73`; scene path and EditorBuildSettings kept in child | Preserved (partly only in the subordinate) |
| **WORLD-1: do not repurpose old geometry identity or switch global `NewGameModel` default** (base spec `:60`) | — | **Not preserved (m-4a)** |
| WORLD-1 manifest/FR-06/Stand Map suppression (base spec `:61-63`) | CL-002, `spec.md:139`, `:147`, R-065/069 | Preserved |
| **Owner world/visual directions: worn-earth path, neutral daylight, restrained edge, H1 without fresh stumps** (base spec `:48`) and D-085 OD-02/OD-06 | Generic "D-085 seventeen choices" only (`spec.md:11`); "stumps"/"worn-earth" only in child | **Not restated in parent (m-4b)** |
| LOOP-1 / S-09 / S-10 / S-11 (no faked rack, real annual loop, latest-only review ack, finance, training-site non-profit, no invented local provider) | CL-017/033/035/044/045; `spec.md:53`, `:106`; R-008/R-040/R-042 rows | Preserved |
| SAVE-1 / S-01…S-05 (explicit stored identity, disjoint slot, pre-mutation preflight, single router, fixture gate) | `spec.md:69-77`; R-011…R-025, R-083…R-092 rows; CL-063/064 | Preserved |
| S-06/S-07/S-08 (no-rack arrival, whole-woodland access, paint/order lifecycle) | `spec.md:35-41`; R-026…R-034 rows; CL-006/015/018/024 | Preserved |
| S-13/S-14/S-15 (substantive treatment, cadence, completion) | `spec.md:47-49`, `:59-65`; CL-051…CL-060 | Preserved (superseded by D-084 M-A / D-077 / #677) |
| S-16/S-17/S-20 (two scales of truth, C6 overlay, feature routing) | R-065/R-066 rows, CL-015; §11 OPEN-C6; R-090, `spec.md:69` | Preserved |
| Tasks risk list (one-surviving-broadleaf return, no natural-recruit dependency, mesh vs biology, #496 deadwood, profile repair not old-test edits, STOP on unsafe save) (base tasks `:113-125`) | R-106, R-110, R-111 checkboxes; CL-043/065; `spec.md:91` STOP list | Preserved via checkboxes |
| #576 cross-spec OPEN-C registers (all four parent files, base req `:183+`) | `spec.md` §11 table, row by row | Preserved/superseded; the stale paragraph at `requirements.md:153` remains (m-1) |

## 8. What is still unsigned, open or NOT RUN

**Unsigned/open, correctly left so by the package:** OD-16 substantive-work classifier (M-A-SCOPE); M-A S1 durable completion persistence and any S1 save amendment; the whole-world manifest, W §12 owner attestation, world digest (null) and exact G1 coordinates (0 signed / 80 candidate / 8 blocked); Compact save/profile activation (#690); #677 D-N1 fidelity review; crop-tree timing, past-selection credit and reselection duty; D-032; E1 and E2; physical Ops/Unity custody (#652/#718/#781); M-5 wording (provisional, subject to Manager review); Manager #731 disposition of this candidate; any APPROVED status or main integration.

**Not checked by me:** current production source behaviour beyond one read-only `git grep` of `WorkItemStatus` on `a69b3e2e`; any claim about the author's local worktree, `/tmp/ccf782_verify.py` runs or the "FileNotFoundError" note (worker-reported, not reproduced); Dashboard/Drive mirrors; the graphics DRAFT (not in scope, location not given); #764/#765/#767 content beyond what the Manager adjudication quotes; GitHub rendering of the Markdown.

**NOT RUN (none claimed):** compiler, Unity Editor, runtime/gameplay, save/load, rendering/screenshots, performance, Tier 1/2/3 tests, E1/E2, personal playtest, Windows build.

## 9. Boundary

This review approves **draft document quality only**. Its four MINOR findings are bounded wording/classification fixes that a single same-author correction could make, with a changed-lines recheck, if Manager #731 asks for one. None blocks Manager consideration. It is not a spec approval, canonical promotion, main merge, implementation or source permit, world/save signature, or a pass of any criterion. All 224 criteria remain unchecked. I edited no `specs/**` file, no author branch, no other handoff and no other worktree; this file is my only write.
