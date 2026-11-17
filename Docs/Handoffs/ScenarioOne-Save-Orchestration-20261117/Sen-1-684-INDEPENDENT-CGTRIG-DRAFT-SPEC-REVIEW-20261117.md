# Sen-1 #684: Independent review of the Compact30 DRAFT spec reconciliation to signed #677 CG-TRIG policy

**VERDICT: NEEDS CHANGES. CRITICAL 0 / MAJOR 2 / MINOR 3 / NIT 4.**

**Scope:** static document and policy-fidelity review only. This is **not** spec approval, implementation authority, a Unity result, a save/native result or Windows evidence. All eight specs remain DRAFT. Every behavioural witness named in them, and in this review, is **NOT RUN**.

## 1. Reviewer identity and custody

| Field | Value |
| --- | --- |
| Reviewer | **Anthropic Claude Opus 5.5** (`claude-opus-5-5`), Claude Code in the T3 Code harness, local Linux host |
| Author under review | DeepSeek V4.1 Flash (`opencode-go/deepseek-v4.1-flash`). This is a different model family and provider |
| Registration | [#684 comment 6956683476](https://github.com/jermur788/CCF/issues/684#issuecomment-6956683476), posted before any write |
| Branch | `review/sen-1-684-compact-draft-cgtrig-independent-20261117-v1` |
| Worktree | `/media/jer/Files/CCF/Worktrees/CCF-684-Review-20261117` |
| Starting HEAD | `b1150a5f37ead311ebe7149e69c8e421d494f160` (equal to the remote author branch tip at review start) |
| Sole written file | This document |

**Disclosure (read this before relying on the review).**

- I did not author any #684 text.
- In earlier sessions, a Claude Opus 5.5 instance (the same model family as me) did write some related material:
  - the #657 and #663 parent/planting DRAFT text that #684 starts from (`1e0fba5f`). GPT-6 independently accepted that text in #665;
  - the #681 review of the #677 supplement, including note **D-N1**.
- So my findings on D-N1, and on the retained #663 sections, are not fully independent of that earlier work. I have kept them to what the signed text says, and I cite exact lines so the Manager can check them.
- My registration comment said I authored none of the specs under review. That is true of the #684 changes, but this disclosure corrects it for the inherited base text.

## 2. Frozen target and normative sources

I verified every identifier below myself.

| Item | Verified value / method |
| --- | --- |
| Review target | `b1150a5f` (result file only) over `5f22e8a5` (eight specs); parent `1e0fba5f` is an ancestor |
| Signed supplement | `bfa01f86310bd013b87ff6686958e8b73315b591`; `git rev-parse bfa01f86:<path>` = `git hash-object` of the shown bytes = **`bb9327fe6c9b1ae1f7d5888e385dc9edc3ad072b`**. Match |
| Out-of-band signature | Branch tip `94c9835a` (checked with `git ls-remote`). Its parent is `bfa01f86`, and it adds only the attestation file (`git diff --stat bfa01f86 94c9835a`: 1 file, +52). It pins the same commit and blob |
| Policy review / D-N1 | `1e923e54`, #681 §7 "New items" D-N1 row; DELTA APPROVED 0/0/0/1 |
| Parent save §4.2 | `200add4e:specs/scenario-independent-save-isolation-v1/spec.md` blob `8ac55102…`. The file does not exist on `1e0fba5f` or `b1150a5f`, so #684 cannot have changed it |
| Dispatch packet | `58bf539c` on the dispatch branch (checked with `ls-remote`) |
| Source of the existing ack dedup | `Assets/ForestPrototype/ScenarioOne/ScenarioOneCompletionRules.cs:62-68` and `ScenarioOneManager.cs:391-401` at `b1150a5f` (static read) |
| D-085 OD-12 | `origin/main` `254d8b0e`, `Docs/Project/decision-log.md:153` |

## 3. Recomputed preservation evidence (all reproduce)

| Check | Result |
| --- | --- |
| Parent `- [ ] R-###` lines, `1e0fba5f` vs `b1150a5f` | 115 lines both sides; 115 unchecked; 0 checked; IDs R-001…R-115 complete and unique; `cmp` **identical** (sha256 prefix `3d6334b9f97780ff`) |
| Planting `- [ ] CIP-###` lines | 33 / 33 unchecked / 0 checked; CIP-001…CIP-033 complete and unique; `cmp` **identical** (`181911abc3c4e767`) |
| All checkbox lines in the two checklists | 115→115 and 33→33; no new checkbox of any form |
| `[x]` anywhere in the nine changed files | 0 |
| `specs/scenario-one-compact-graphics-v1/**` | Tree `42388b4a…` is identical at `1e0fba5f` and `b1150a5f`; 66 unchecked in `checklists/requirements.md`; 0 checked |
| New requirement IDs (R-116+, CIP-034+) in added lines | none |
| Changed paths `1e0fba5f..b1150a5f` | exactly 8 M (the allowlisted specs) + 1 A (author result); `5f22e8a5..b1150a5f` = the result file only |
| Before/after blob IDs in author result §2 | all 16 short IDs **match** |
| Author per-file +/− counts | parent `spec.md` and child `spec.md` **do not match**; see N-1 |
| Requirement assertions weakened | none. No checkbox line changed. In-place prose edits replaced "undecided" or "refuses" text with signed dispositions. They removed no requirement |

## 4. Findings

### MAJOR

**M-1: The child spec turns a signed cannot-load state into a loadable no-credit state.**

- **Where:** `specs/scenario-one-compact-initial-planting-v1/spec.md:39` (the #684 bullet "No-credit history is normal, not a bug to heal").
- **What it says:** a group "that **never gets its covering review in time**, keeps honest history and an explicit no-credit diagnosis; T1 is unreachable for it."
- **Why that is wrong:** "Never got its covering review in time" means CT-1 true, `reviewAcknowledgedYear=-1` and current year > Y\*. That is refusal case (a). The signed text requires **REFUSE Save capture/Load before mutation** for it, with a real **cannot load slot** consequence:
  - supplement T-05 (D:30), F-01 (D:38) and matrix row D:80;
  - signature §2 item 5, which says to "**Do not transform it into no-credit-but-loadable**".
- **Internal conflicts:** the bullet contradicts the line just above it (`:38`), parent `spec.md` §684-2 (`:260` ff.) and parent CT-7 case 13 (`:86`).
- **Second problem in the same bullet:** it calls "loses a row" normal. The supplement says incorrect forward producer states "must be prevented, not excused by this admission" (signature §2 item 6) and "fresh producer must never persist missing-row settlement" (D:78). Line 36 of the same file says the same.
- **Repair:**
  - delete "or that never gets its covering review in time" (or move it under the refusal bullet);
  - reword the heading and "loses a row" so that admitting the state is not presented as normal producer behaviour;
  - add the signed owner-escalation clause to `:38`, as parent §684-2 already has it.

**M-2: Invented rule: "must never create a row for a `Completed` 3+1/other non-2+2 group".**

- **Where:** `specs/scenario-one-compact-initial-planting-v1/spec.md:36`. The text reads: "…and must never create a row for a `Completed` 3+1/other non-2+2 group — those states load only as truthful history with **no** T1."
- **Signed text:**
  - Rows are per order: "only a **successful Completed** real designated order adds its own matching signed-shape planting row" (D:49).
  - A 3+1 group is admitted with its rows verified: "ADMIT genuinely valid 3+1 Completed work/history … **every positive row verified under signed schema**" (D:77; T-05 D:30).
  - Parent CT-1 (`spec.md:58`) and PJ-4 (`:104-106`) agree: each order in G gets its row at its own successful resolution.
- **Why it matters:** no signed text bars rows for a 3+1 group. Rows are written per order, before the group's final makeup is known. An implementer could read this child rule as "rows present in a 3+1 save are invalid". That would tighten a signed-admitted state.
- **Repair:** delete the clause. If needed, restate the signed rule: forward designation already refuses a third same-species order (D:48), and a historical 3+1 group loads with its verified rows and no T1.

### MINOR

**m-1: D-N1 is handled legitimately overall, but the "used consistently" claim is not true and the admitted outcome is not stated.**

- **Inconsistent wording:** the #684 sections say the "at or after the final `WorkResolved`" qualifier is used consistently: parent §684-5 (`spec.md:288`), parent checklist `:48`, child checklist `:39`, child spec `:41` and author result `:98`. But the new CT-2 sentence at parent `spec.md:59` leaves it out. It refuses "a split state (event9 for Y\* already present while the field is still −1)" with no "covering" or "after final `WorkResolved`" qualifier. This is the exact pattern D-N1 flagged.
- **Unqualified one-per-year rule:** "one event9 per year" appears unqualified at `spec.md:251`, checklist `:38`, child `spec.md` C84-3 and child checklist `:34`.
  - The real dedup (`ScenarioOneCompletionRules.cs:62-68`) is one per year **per resolution boundary**. It allows a fresh event9 after a later `WorkResolved`.
  - Under the chosen narrow reading of §684-2(2), an edited-file pre-final event9 at Y\* is admitted, so CT-3 holds. Release then needs exactly that fresh covering event9.
  - The unqualified CT-2 refusal and the "one per year" wording would block it. That re-creates the trap that T-03 (D:52, "no permanently trapped admitted CT-1=true state") forbids.
- **Outcome and witness missing:** §684-5 says the case is "a separate negative-test and spec-clarity matter". It does not state the expected outcome (admit, hold, then a fresh covering ack) and does not cite the existing CT-7 case 7 (`spec.md:80`) as that witness.
- **What is fine:** these parts of the D-N1 disposition are legitimate:
  - a false positive claim is always refused;
  - the case is not treated as source proof;
  - no wire rule is invented;
  - there is a route to Manager BLOCKED;
  - the qualifier chosen matches signed T-05 (D:30), the retained CT-2 "covering" definition (`spec.md:59`) and #681's analysis.
- **Repair:**
  - add "covering (after the final `WorkResolved`)" to the `:59` split-state sentence;
  - define "one event9 per year" once as the existing per-boundary dedup;
  - add one sentence giving the admitted pre-final case's expected outcome, citing CT-7 case 7 (NOT RUN);
  - correct author result `:98`.

**m-2: Two native-qualification obligations from the signed text are not carried into the #684 witness mapping.**

- **Failure injection:** "Native failure-injection evidence required before this can be considered implemented" (signature §2 item 7). The supplement also says "inject faults" (D:88).
  - PJ-4a (`spec.md:108`), the §684-1 T-06 row (`:255`), parent tasks D84-SC-RV1…4 and child CP-S6 (`tasks.md:24`) do not require fault-injected settlement evidence.
  - Author result §4.3 maps "Interrupted settlement" to PJ-4a/5, which do not contain that requirement.
- **Exact G1 site round-trip:** D:90 requires a native check of the exact G1 site identity round-trip, noting that OD-12 zero planar placement "is not a demonstrated JsonUtility bitwise equality". No #684 section or task names this check.
- **Repair:** one clause in PJ-4a / CP-S6 (fault-injected settlement, complete-old-or-new) and one in the §684-6 mapping or CP-S6 (exact G1 x/z round-trip). Both stay NOT RUN.

**m-3: Retained PJ-1 still says the site tolerance is undecided, which conflicts with signed and canonical text.**

- **Where:** `specs/scenario-one-compact-complete-loop-v1/spec.md:101`, PJ-1: "The site-match tolerance belongs to the content signature and is not chosen here."
- **Signed text:** "exact zero planar tolerance" (D:48).
- **Canonical text:** D-085 OD-12 A on `main` reads "**zero planar placement tolerance** — an order uses the selected G1 site's exact stored x/z".
- **Why it is in scope:** the packet's §3 D asks for this reconciliation ("four G1 exact sites with 0 planar displacement"). The #684 reading rule (`:244`) supersedes only "undecided" CT-4/D84-O wording, so PJ-1 was missed.
- **Repair:** change PJ-1 to "zero planar tolerance (signed #677 D:48; D-085 OD-12 A); exact coordinates remain unsigned (#676)".

### NIT

- **N-1:** Author result `:34` says parent `spec.md` is +87/−2; it is actually **+82/−7**. Author result `:38` says child `spec.md` is +19/−1; it is actually **+17/−3** (`git diff --numstat 1e0fba5f 5f22e8a5`). The totals happen to match, but the split does not.
- **N-2:** `specs/scenario-one-compact-complete-loop-v1/tasks.md:15`: the D84-CF-1R "Depends on" cell was changed in place to "Satisfied by #665…".
  - The underlying fact checks out: [#665 comment 6933906312](https://github.com/jermur788/CCF/issues/665#issuecomment-6933906312) returns APPROVED FOR DOCUMENT QUALITY ONLY.
  - But this in-place edit is missing from the author result's §2.1 in-place inventory.
  - #665 itself says "Manager #640 accepts/rejects". Write "accepted by #665 (document quality only)" rather than "satisfied".
- **N-3:** The CT-7 preamble (`spec.md:73`) still says refusals happen "where §4.2 says 'refuses'". New case 13's refusal comes from #677 T-05, not §4.2. Add "or signed #677 T-05".
- **N-4:** Two crosswalk overlaps:
  - Parent §684-6 (`spec.md:300`) and parent checklist `:43` call R-051–R-063 "unchanged by #684". The same tables re-read R-056–R-061 under T-02/T-03 (`:38-39`). Narrow the range to R-051–R-055, R-059, R-062, R-063.
  - Checklist `:40` adds R-092, but spec §684-6 `:297` does not list it. (The child's CIP-027/CIP-032 save mapping follows the existing #614 overlay convention at child checklist `:68`, so I do not flag it.)

## 5. Signed-policy fidelity audit (scope item 1)

Representation in the eight files, checked against the signed text:

| Signed item | Represented? | Notes |
| --- | --- | --- |
| T-01 Policy B (empty and non-empty; no fabrication; identity/fraud still refused) | Yes | CT-4 `:70`, §684-1, CT-7 case 12, C84-3 |
| T-02 existing Annual Review control as the only covering ack; atomic event9 + `reviewAcknowledgedYear=Y*`; no second button/event9; idempotent | Yes | CT-2 `:59`, plan (9), C84-3. Wording issue m-1 |
| One event9/year | Yes, as signed wording | Signed text says "one-per-year" (D:51). The source rule is per boundary (m-1) |
| T-03 CT-3 hold on all routes incl. F-keys/skip/service, across Save/Load; not a fifth conjunct | Yes | CT-3 `:66`, §684-1 |
| T-04 strict refusal of fabricated/mismatched positive claims | Yes | §684-4, CT-7 cases 2/3/5 |
| T-05 / F-01 two deadlock refusals, Save **and** Load, before mutation, real cannot-load, no backdating/bypass, validated backup only, owner escalation | Yes in parent (§684-2, CT-7 case 13, plan (10), RV2) | **Contradicted in child (M-1)** |
| T-05 / F-03 missing-row and 3+1 admitted, T1 permanently unreachable, no backfill, no Completed replacement | Yes | PJ-7 `:119`, §684-3, case 14. **Child adds an invented rule (M-2)** |
| F-04 exhaustive positive-claim set | Yes | §684-4 matches D:36 |
| T-06 direct pairing, atomic cluster, idempotent | Yes | PJ-4a; current source correctly marked not exception-atomic. Fault-injection witness missing (m-2) |
| T-07 W-A freeze on UI **and** all service/bulk setters; confirmed cancel = audited removal; new Pending order; no hidden demotion; decline mutates nothing; no new `Cancelled` status | Yes | W-1/W-3, C84-5, RV4/CP-S9 |
| T-08 M-A S1 unsigned; OD-16 deferred; retained ≠ Fell | Yes | §684-1, §684-7, checklists |
| T-09 policy only | Yes | Throughout; no "implemented" upgrade found |
| Claims of a passed test | **None found** | Every witness says NOT RUN or proposed |
| Invented beyond the signed text | M-2 (child 3+1 row rule); M-1 (refusal state recast as loadable) | No new key/event/DTO/root/profile/sidecar found |

## 6. D-N1 assessment (scope item 2)

**Legitimate in substance; minor wording repair needed (m-1).**

- A false positive review claim is always refused, and this is stated four times.
- The pre-final event9 case is classed as a negative-test/spec-clarity matter, not as admitted source proof.
- No wire rule is invented, and the route back to the Manager as BLOCKED is preserved.
- Choosing the T-05 "at or after the final `WorkResolved`" reading is a defensible alignment, not a policy expansion:
  - it is the qualifier in the signed refusal register itself (D:30);
  - it matches the retained CT-2 "covering" definition;
  - #681 showed that neither reading traps an ordinary-play state;
  - the signature asked for exactly this kind of spec-level alignment (signature §2 "NOT ACCEPTED", D-N1 bullet).
- I found no material difference that needed a BLOCKED return.
- The defects are cosmetic but real:
  - the claim of consistent use is wrong at `spec.md:59`;
  - the admitted pre-final outcome is not written down;
  - the unqualified "one event9 per year" could, if read literally, trap that admitted state.

## 7. Boundary checks (scope item 4)

| Boundary | Result |
| --- | --- |
| No claim of spec approval / implementation / Unity / save / native / Windows | PASS: banners say DRAFT, NOT APPROVED |
| OD-16 M-A-SCOPE deferred; no three-Fell quota; no treated-stem minimum; retained crop trees ≠ Fell | PASS |
| 175-row roster unsigned; O1 0.141 m intrusion unresolved | PASS (§684-7, plan, child) |
| 14 H1 pre-world omissions not revenue/Fell/work | PASS. No #684 text changes this; retained text and the author result keep it |
| Historical v1–v24, Legacy40/Enlarged80 Rack V2/MFM, frozen Reference Future v1, §4.2 unchanged | PASS. The §4.2 file is not on the branch; S-03/S-04 are untouched |
| No new save key/event/DTO/root/profile/sidecar | PASS |
| Graphics untouched | PASS (tree identical) |

## 8. Accuracy of the author result (scope item 5)

These reproduce:

- the supplement blob;
- the commit IDs `1e0fba5f`/`5f22e8a5`;
- all 16 before/after blob IDs;
- 115/33/66 and 0 checked;
- byte-identical checkbox lines;
- the graphics are unchanged;
- the nine-path inventory;
- the registration comment 6956470348 (author `jermur788` account, 18:00:35Z, DeepSeek V4.1 Flash);
- `7ae3ad86` is on `main` and carries the #684 READY board row;
- no later commit touches the parent/planting specs.

These do not reproduce:

- per-file +/− for two files (N-1);
- "qualifier used consistently" (m-1);
- the §2.1 in-place inventory, which omits D84-CF-1R (N-2).

The QA statement "internally consistent with the signed #677 supplement" does not hold, because of M-1 and M-2.

## 9. Required bounded repair

One author repair, limited to these lines, then a changed-lines-only recheck:

| Finding | Lines to repair |
| --- | --- |
| M-1 | child `spec.md:38-39` |
| M-2 | child `spec.md:36` |
| m-1 | parent `spec.md:59`, plus one definition sentence and one outcome sentence in §684-5 and the two D-N1 checklist paragraphs; author result `:98` |
| m-2 | PJ-4a / CP-S6 / §684-6 |
| m-3 | PJ-1 `spec.md:101` |
| NITs (optional) | the lines listed in N-1 to N-4 |

- No new requirement ID and no checkbox change is needed.
- No Manager or owner decision is needed. Every repair restores the signed text.

## 10. NOT RUN / NOT VERIFIED

- Unity, the Editor, compile, tests, Save/Load, native, failure injection, Ops and Windows: **NOT RUN** (out of scope).
- The author's live board read at their start time: **NOT VERIFIED**. I confirmed only that `7ae3ad86` exists on `main` with the #684 row.
- Whether #616/#617 clarifications were inherited into #663: **NOT RE-AUDITED** (outside the bounded scope; #665 covered #663).
- Full 115/33/66 semantic audits: **NOT RE-RUN**, as instructed. Only line identity and the #684 crosswalk were checked.
- Dashboard and Drive/context mirrors: **NOT VERIFIED**.
- The source reads (`ScenarioOneCompletionRules.cs:62-68`, `ScenarioOneManager.cs:391-401`) are static only.
