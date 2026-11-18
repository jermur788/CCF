# Sen-1 #775 — Independent nonauthor changed-lines recheck of the #774 Compact30 DRAFT correction

**2026-11-18. VERDICT: APPROVED — FOR DRAFT DOCUMENT QUALITY ONLY.**
**Severity of new findings: 0 BLOCKER / 0 MAJOR / 0 MINOR / 6 NOTE.**
**#771 findings rechecked: 14 of 14 ADDRESSED** (M-1…M-6, m-1…m-6, n-1, n-3).

This verdict covers only the documentary quality of the #774 changed lines. It does **not** approve gameplay, the spec, a canonical promotion, source, Unity, world content or save policy. The package stays **DRAFT / NOT APPROVED / NOT AN IMPLEMENTATION PERMIT**. Manager #731 decides the disposition. No #763 or #774 file was edited.

## 1. Reviewer, independence and frozen targets

| Field | Value |
| --- | --- |
| Reviewer | Anthropic **Claude Opus 5.5** (`claude-opus-5-5`), Claude Code (Agent SDK) inside T3 Code, local Linux session. The reasoning setting is not shown to me, so I do not state one. No subagents were used. |
| Independence | Not the #763 author (OpenAI GPT-6, browser) and not the #774 author (OpenAI gpt-6.1-sol, Codex harness). I wrote none of the seven package files or the #774 result. **Disclosure:** the earlier #771 review was also performed by Claude Opus 5.5, in a separate session. This recheck is therefore author-independent and provider-independent of #774, but **not model-independent of the original #771 reviewer**. Manager may weigh that. |
| Claim | REST `gh api repos/jermur788/CCF/issues/775/comments --jq '.[].body' \| grep -c RUNNING` returned **2** before any write. |
| Worktree / branch | `/media/jer/Files/CCF/Operations/CCF-775-DraftRecheck-20261118`, branch `review/sen-1-775-compact30-draft-recheck-20261118-v1`, clean at fresh main `a69b3e2e90829505c792d95f5a3eba3e7d8d8132` before writing |
| OLD (frozen #763) | **`9ab372bab3ca67c4dc2e7da94b2b5b88ed661360`** |
| NEW (#774 head) | **`d34fa89a5471e078eb008036c40660bc5805ee71`**. The seven-file correction is `1e61bd2c7593a939f2c96edca56486f20373ff87`, and `1e61bd2c..d34fa89a` adds only the result file. `git ls-remote` confirms that the remote `docs/sen-1-774-compact30-draft-corrections-20261118-v1` points at `d34fa89a`. `d34fa89a` is **not** an ancestor of main `a69b3e2e`, so nothing has been integrated. |
| Read in full | The #771 review (local copy of `d043efe1`); all seven NEW package files; the #774 result; full OLD↔NEW diffs of all seven files |
| Controlling sources spot-checked (read-only) | Decision Log D-074, D-077, D-084 (Geometry A), and D-085 OD-15 digest on main `a69b3e2e`. S1-D spec status line on main. Signed #677 supplement at `bfa01f86` (rows "Designated planting group", "Arrival only", the strict-mismatch refusal row, T-05 and the failed/cancelled slot rule). #617 originals at `fcfd43c3`. Source enums `ScenarioWorkType`, `ScenarioWorkStatus`, `WorkItemStatus` and `ScenarioManagementEvent.OrderCancelled`. |

## 2. Command-level verification (actual output)

```text
$ git diff --name-status 9ab372ba..d34fa89a
M  .../Sen-1-763-DRAFT-PACKAGE/author-audit.md
M  .../Sen-1-763-DRAFT-PACKAGE/checklists/requirements.md
M  .../Sen-1-763-DRAFT-PACKAGE/independent-review-packet.md
M  .../Sen-1-763-DRAFT-PACKAGE/owner-decisions.md
M  .../Sen-1-763-DRAFT-PACKAGE/plan.md
M  .../Sen-1-763-DRAFT-PACKAGE/spec.md
M  .../Sen-1-763-DRAFT-PACKAGE/tasks.md
A  .../Sen-1-774-GROUPED-DRAFT-CORRECTION-RESULT-20261118.md
   -> exactly 7 modified package files + 1 added result. The eighth original #763 result is not touched.

$ git diff --numstat 9ab372ba d34fa89a
17/8 author-audit | 182/12 requirements | 30/30 review-packet | 16/9 owner-decisions
11/9 plan | 19/11 spec | 9/7 tasks | 104/0 #774 result      (matches the #774 result §5 table)

$ git diff --name-only 9ab372ba d34fa89a -- specs Docs/Project | wc -l      -> 0
$ git diff --check 9ab372ba d34fa89a                                         -> clean

$ grep -Ec '\[[xX]\]' <each of the 7 NEW package files>                      -> 0 in every file
$ grep -c '^- \[ \] \*\*CL-' requirements.md                                 -> 76
  CL IDs sequential 001..076, no gap; CL-001..070 ID list identical to OLD; no CL ref > 076 anywhere
  duplicate **CL-nnn** definitions across the package                        -> none
$ grep -Ec '^\| *(R|CIP)-[0-9]{3}' requirements.md                            -> 148
  duplicate disposition IDs -> 0; R rows 115 = R-001..R-115 in order; CIP rows 33 = CIP-001..CIP-033 in order
  every row has 7 '|' fields (5 columns)
  class counts: 69 retained unchanged / 62 retained with design provenance updated /
                4 deferred by D-084 C6 or C10 (stays unchecked) / 13 process item for Manager to close
                (matches the author's stated 69/62/4/13)

$ git show fcfd43c3:<parent|child>/checklists/requirements.md
  parent: 115 '- [ ]', 0 checked; child: 33 '- [ ]', 0 checked                (originals untouched and unchecked)

$ git rev-parse bfa01f86:Docs/Handoffs/ScenarioOne-Save-Orchestration-20261117/Sen-1-677-MANAGER-COMPACT-CGTRIG-AND-SAVE-POLICY-SUPPLEMENT-20261117.md
bb9327fe6c9b1ae1f7d5888e385dc9edc3ad072b                                      (new #677 pin and link resolve)
$ grep -c dac06a3b7775f884…0426cca1 Docs/Project/decision-log.md              -> 1   (OD-15 digest is real)
```

## 3. Per-finding verdict

Paths are relative to `Sen-1-763-DRAFT-PACKAGE/` at `d34fa89a`.

| #771 finding | Verdict | Where the changed lines are (verified) |
| --- | --- | --- |
| **M-1** 148 per-ID dispositions | **ADDRESSED** | `checklists/requirements.md` §H: 148 rows, one per ID, in order, with no ranges. R-002 and R-003 are present (process items mapped to CL-068…070). Each row has exactly one of the four classes, a controlling row/item, a duty and CL links. The header now says D-083–D-091/#677 supersede design provenance "only as individually dispositioned in §H". The §H preamble keeps "all other original duties and negative witnesses remain", which carries forward the removed sentence "the substantive negative/positive test obligation remains". Originals at `fcfd43c3` are untouched and 0 checked. See NOTE N-1 (R-097 class choice) and N-2 (R-049). |
| **M-2** dual-acknowledgement designation gate | **ADDRESSED** | `spec.md` C3 states "No G1 group designation is allowed before BOTH `arrivalAcknowledged` and `assessmentAcknowledged` are true", refusal comes before any order, reservation or group ID, and Policy B does not relax it. Positive **CL-071**, negative **CL-072** (each flag false and both false). `tasks.md` F1 check (line 26) carries the negative. `plan.md` P3 and `owner-decisions.md` §B "Genuine G1 linked work" match. Wording matches #677 "With both prior acknowledgements true" and "No group designation before both acknowledgements". |
| **M-3** CL-036 present-and-false only | **ADDRESSED** | **CL-036** now reads "mandatory arrival/assessment keys **PRESENT and `false`**". The old "false/missing" is gone. **CL-073** (missing or ill-typed acknowledgement key) and **CL-074** (any other missing, ill-typed or unknown signed-type key) refuse before mutation with live/Prefs/primary/tmp/bak unchanged. `spec.md` §8 paragraph 2, `plan.md` P4 acceptance and the `tasks.md` F2 check match. `owner-decisions.md` §B Policy B row is reworded to "present and false". See N-5 on "ill-typed". |
| **M-4** crop-tree semantics | **ADDRESSED (route b)** | `owner-decisions.md` §C has a new row, "Crop-tree requirement semantics", OPEN, with options A and B and owner escalation only for a material gate. `spec.md` C2, **CL-013/014**, `tasks.md` crop-tree checks and plan P3 state the death, None/Fell re-mark and felling consequences for *current* evidence. Past-selection credit and reselection stay OPEN. No key, quota, automatic replacement or T1 conjunct is introduced. The old CL-014 duty ("true choice history survives… under separately approved rule") is relocated into the §C decision rather than lost (N-3). |
| **M-5** S1-D status and Compact trigger | **ADDRESSED** | `spec.md` §1 and C7, `plan.md` P2, `owner-decisions.md` §C/§E, `author-audit.md`, `tasks.md` and **CL-075** all carry "APPROVED PRODUCT INTENT; D-066 implementation contract pending Manager review #158". Every "approved S1-D" claim is removed (grep returns 0). One entry trigger: the signed Compact T1 predicate true, including same-world re-entry. One refresh rule: a later real advance plus current reassessment. No rack latch, no v24 latch and "No new persisted teaching latch is authorised". Labelled "Manager implementation wording for Compact, subject to review", with owner escalation if product intent changes. The D-074 coaching sentence is faithful to D-074 ("Do not present… consecutive years as the expected or sufficient exemplary CCF cycle"). See N-4 on quote abridgement. |
| **M-6** spec-consolidation gate | **ADDRESSED** | `owner-decisions.md` §C "Spec consolidation path" is "OPEN — Manager chooses; no option pre-selected". Option A merges into `scenario-one-compact-complete-loop-v1`; option B creates a new directory with superseded-by links and an ID carry table. Both must reconcile the planting and graphics DRAFTs. `plan.md` P0.2 says "No path is pre-selected", and the P0 exit and the `tasks.md` dependency edge require the choice before promotion. No text elsewhere picks an option. |
| **m-1** D-N1 and deadlock escalation | **ADDRESSED** | `spec.md` §8 ("Open #677 D-N1"), §10 stop condition "STOP and escalate to the owner". Also CL-039, the §B row, plan P4 "F2 fidelity stop" and tasks "Handoff/review and stop". D-N1 is left unresolved. |
| **m-2** Cancelled is not a status | **ADDRESSED** | `spec.md` C4 lists statuses Pending/Approved/Completed/Failed and describes cancellation as deletion plus an audited OrderCancelled event shown as `WorkItemStatus.Cancelled`. **CL-028** adds "no new Cancelled value". `spec.md` §5 changes "failed/cancelled status" to "failed outcomes and audited cancellations". Matches source enums (`ScenarioWorkStatus` has 4 values; `WorkItemStatus.Cancelled = 3`; `ScenarioManagementEvent.OrderCancelled`). |
| **m-3** coordinates unsigned, fixed species | **ADDRESSED** | `spec.md` C3 and **CL-019** say "the exact stored x/z from the future owner-signed world manifest (currently unsigned)" and give each site a fixed matching species (D-084 Geometry A; #677). The §B row matches. No remaining "signed coordinates" wording (grep; R-107 says "exact future signed sites"). |
| **m-4** pins | **ADDRESSED** | `fcfd43c3` in spec §1, the requirements header and §H, and owner-decisions §E. `bfa01f86` / blob `bb9327fe` in spec §1, owner-decisions §B and §E, and the requirements header. The link now targets the supplement path, which I resolved to blob `bb9327fe`. Roster digest `dac06a3b…` in spec §3, owner-decisions §A and the requirements header, each labelled roster-only. |
| **m-5** slot replacement | **ADDRESSED** | `spec.md` C4 and **CL-076**: a NEW distinct Pending order only; at most 4 active IDs; Completed slots are not replaceable. Negatives cover a Completed slot, a reused ID and a fifth ID. Also plan P3, the tasks F1 check and the §B W-A row. Matches #677 ("never growing beyond 4 active group IDs"; "not Completed"). |
| **m-6** Geometry A and tick rule | **ADDRESSED** | `owner-decisions.md` §A has a new **D-084 Geometry A** row (four original positions; shelter r ≤ 0.10 m; signs nothing), which matches D-084 "no larger than upright r=0.10m". `spec.md` §6 and **CL-051** add "one tick per frame, cancellable only between ticks", as do plan P4 and the tasks cadence checks. |
| **n-1** permanent skip loss | **ADDRESSED** | `spec.md` §8 paragraph 3 and **CL-037**, next to the no-T1 / 3+1 diagnostic. |
| **n-3** work types and OD-16 retention question | **ADDRESSED** | `owner-decisions.md` §D lists FellTree, PlantJuvenile, RemoveRegeneration, PruneTree, SalvageDeadwood, which matches source exactly. It asks OD-16 how retention is evidenced and says "this register does not answer it". |

## 4. Regression and overreach checks

| Check | Result |
| --- | --- |
| 1. Removed or reworded inherited obligation now weaker | **None material.** Each removed OLD line was compared with its replacement (diff `<` lines for all seven files). Rewordings: (a) "One-year advance must always remain possible" became "remains available… subject to the same mandatory review/CT-3 guards" (spec §6, CL-051, plan P4). This aligns the text with the signed #677 CT-3 all-route hold, which the OLD text contradicted, so it is not a weakening of any signed requirement (N-6). (b) The CL-014 persistence sentence moved into the §C decision (N-3). (c) CL-036 "no… state mutation" became "exact bits retained… no load-time backfill", which is equivalent for an admission. (d) The requirements-header obligation sentence is carried by the §H preamble. |
| 2. Newly invented approval, quota, threshold, key, timestamp, signature, PASS or owner decision | **None found.** No numeric threshold, save key, latch, coordinate or digest was added. The only new hash is the pre-existing D-085 OD-15 roster digest, verified in the Decision Log. "Resolved crop-tree semantics" was added to the plan P0 exit. That is a gate on this DRAFT's own promotion that follows the new §C row, not a decision. |
| 3. ID reuse, renumbering or newly checked box | **None.** CL-001…070 IDs are unchanged and in order. Only CL-071…076 are new. 0 checked boxes in every package file. |
| 4. Claims that the DRAFT is APPROVED or that a review/decision has happened | **None.** All banners say DRAFT / NOT APPROVED. The author-audit and review packet say "changed-lines nonauthor recheck has NOT RUN". tasks/plan describe #771 only as NEEDS CHANGES. A grep for unqualified `APPROVED` claims found none. |
| 5. Weakening of signed #677 or §4.2, or scope beyond seven files | **None.** Name-status shows exactly 7 package files plus 1 result. 0 changes under `specs/` or `Docs/Project/`. Changed #677-bound wording (designation gate, refusal rows, replacement cap, cancellation) was checked line-by-line against `bfa01f86` and is faithful or stricter. |
| 6. Author result overstating the work | **No material overstatement.** Counts (148; 69/62/4/13; 76 CL; 0 checked), the numstat table, the commit split and the remote head all reproduced. Minor wording: the result calls the S1-D phrase "exact" although it is abridged (N-4). I could not verify the claim that post-publication checks were re-run before handoff, and I do not rely on it. |

### NOTES (non-blocking; no correction required for document quality)

- **N-1 — R-097 class choice.** R-097 is classed "deferred by D-084 C6 or C10", but most of its text is non-overlay Work Plan accounting (BA/volume, expected vs settled, denominator, entrance/footpath exclusions). The analogous mixed row R-039 is classed "retained with design provenance updated". The duty is explicitly preserved twice (the §H preamble and the row text "no accounting waiver", linked to CL-050), and this follows #771's own wording ("the overlay half of R-097"). The canonical version could reclassify R-097 to match R-039, which would make the counts 63/3.
- **N-2 — R-049 stale fragment.** R-049 (fcfd43c3) contains "No fabricated exterior trees, 25/30m approval". D-083 has since approved 30 m. The row is classed "retained unchanged". The surviving duty (no fabricated exterior trees or donors) is unaffected, but a provenance note citing D-083 would remove the apparent conflict.
- **N-3 — CL-014 persistence duty relocated.** The OLD duty that choice history survives persistence is now an OPEN §C question, with an explicit warning not to read history from current marks. This is the intended route (b), not a loss. Manager should make sure the §C answer covers persistence.
- **N-4 — S1-D status abridged.** The main S1-D status line reads "APPROVED PRODUCT INTENT; D-066 IMPLEMENTATION CONTRACT REPAIRED BY #145 AND PENDING MANAGER REVIEW #158. NO T-04P/T-04 PRODUCTION RELEASE UNTIL #158 APPROVES." The package quotes the shorter form that #771 and the task brief prescribed. The meaning is preserved, and spec §1 adds "not a released Compact implementation contract".
- **N-5 — "ill-typed" is derived.** #677's refusal row lists missing mandatory and unknown signed-type keys under "strict structural mismatch" and a typed validator. "Ill-typed" is not literal text there. #771 requested the wording, and CL-074 cites it as "#677 §4 strict structural mismatch". The canonical version may want to say it is derived.
- **N-6 — "always" softened deliberately.** See check 1(a). D-075's "player can always choose one year" now sits under the signed CT-3 hold. That ordering is correct, but the canonical spec could state the precedence once.

## 5. Overall verdict

**APPROVED** — FOR DRAFT DOCUMENT QUALITY ONLY.

All 14 rechecked #771 findings are addressed in the actual changed lines. Mechanical scope and count invariants hold. No regression or overreach above NOTE level was found. This is not gameplay, spec, canonical, world, save or implementation approval. OD-16, M-A S1 persistence, the owner whole-world/W §12 signature, crop-tree semantics A/B, consolidation A/B, Compact S1-D wording and D-N1 all remain **OPEN/UNSIGNED**, exactly as the package records.

**Minimal corrections:** none required. N-1 and N-2 are optional for whoever writes the canonical version after Manager chooses a consolidation path.

## 6. What I did not verify

- No Unity, compile, native, runtime, save, render or player check. None of these applies to a docs-only recheck, and none was run.
- I did not re-review the **unchanged** lines of the package beyond the semantic context of the changed lines. This was a changed-lines recheck, not a fresh full review. n-2, n-4 and n-5 from #771 were not rechecked.
- I spot-checked §H row accuracy against originals only for the rows whose `fcfd43c3` text contains OPEN/proposed/approval wording (28 IDs) plus R-002, R-003, R-010, R-039, R-052, R-053, R-060, R-067, R-068, R-082, R-083, R-089, R-091, R-097, R-098, R-100, R-113…R-115, CIP-009, CIP-016 and CIP-023…026. I did not semantically audit the other rows' controlling citations or CL links one by one; only their format, uniqueness and CL ID validity were checked mechanically.
- I did not open the #764 dossier, #731 plan, #681 review, the separate #677 signature attestation or the graphics DRAFT. The `bb9327fe` blob was reproduced, but the signature file was not re-read.
- I did not check Board, lanes, Ops, the dashboard or the Drive mirror (**NOT VERIFIED**).
- I did not verify the #774 author's runtime identity or the claim that post-publication gates were re-run.

## 7. Sign-off

| Field | Value |
| --- | --- |
| Verdict | **APPROVED — DRAFT DOCUMENT QUALITY ONLY** |
| New findings | BLOCKER 0 / MAJOR 0 / MINOR 0 / NOTE 6 |
| #771 findings | 14/14 ADDRESSED |
| OLD → NEW | `9ab372bab3ca67c4dc2e7da94b2b5b88ed661360` → `d34fa89a5471e078eb008036c40660bc5805ee71` (correction `1e61bd2c`) |
| Reviewer | Claude Opus 5.5, nonauthor; branch `review/sen-1-775-compact30-draft-recheck-20261118-v1` from `a69b3e2e` |
| Not asserted | Gameplay or spec approval, canonical promotion, OD-16 choice, S1 schema, world signature, native PASS, Manager acceptance, dashboard/Drive sync |
| Next | Manager #731 disposition, then the Manager/owner gates the package already lists |
