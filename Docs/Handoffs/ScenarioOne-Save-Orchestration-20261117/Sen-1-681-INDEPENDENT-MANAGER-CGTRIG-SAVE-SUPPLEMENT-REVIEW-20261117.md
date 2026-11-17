# Sen-1 #681 — Independent review of the #677 Manager Compact30 CG-TRIG / save-policy supplement

**INDEPENDENT DOCUMENT/POLICY REVIEW ONLY. Not a signature, not a spec approval, and no source, Unity, save, Ops or Windows authority.**

## 1. Identity and custody

| Field | Observed value |
| --- | --- |
| Reviewer | Anthropic Claude Opus 5.5 (`claude-opus-5-5`), Claude Code harness inside T3 Code |
| Independence | Different model family from the author (OpenAI GPT-6, Overall Manager #675). I wrote no part of the candidate. **Lineage disclosure:** a separate Claude Opus 5.5 session reviewed the earlier #673 candidate. That is overlap with an input document, not authorship of this text |
| Host / registration | `jer-laptop`; registered 2026-11-17 16:59 UTC, [#681 receipt](https://github.com/jermur788/CCF/issues/681#issuecomment-6954948276), before the first write |
| Starting main HEAD | `a9fb2ca7f1f28ce475efd6ecc3c540146b627495` (live `origin/main` at start) |
| Review branch / worktree | `review/sen-1-681-compact-save-supplement-independent-20261117-v1` / `/media/jer/Files/CCF/Orchestration/CCF-681-Supplement-Review-20261117` |
| Sole writable file | This new file |
| Candidate (frozen) | `c866e3c926aca6c6ad4f472c5122194c31314826`, blob `b66205695ae72c48afda82cd3122b0c3f133de0e`, 26,402 characters (26,457 bytes UTF-8). Verified |
| Candidate diff | `5398ca21..c866e3c9`: exactly one added file (101 lines). Packet commit `17102b3d` adds only the review handoff |

### Source aliases (line numbers from `nl -ba`)

| Alias | Exact source |
| --- | --- |
| C | the candidate at `c866e3c9` |
| SAVE | `200add4e:specs/scenario-independent-save-isolation-v1/spec.md` (signed §4.2 = lines 153–167; FR-11 = 214) |
| R673 | `0cf07062:…/Sen-1-673-COMPACT30-CGTRIG-AND-MAE-SAVE-OWNER-CONTRACT-RULING-20261117.md` |
| DL | `a9fb2ca7:Docs/Project/decision-log.md` (D-077 = 135, D-084 = 151) |
| M / WO / EV / CR | `ScenarioOneManager.cs` / `ScenarioOneWorkOrder.cs` / `ScenarioManagementEvent.cs` / `ScenarioOneCompletionRules.cs` at `a9fb2ca7` |
| UI | `Assets/ForestPrototype/UI/ScenarioOneUiRoot.cs` at `a9fb2ca7` |

The nine source files cited by #673 have identical blob IDs at #673's base `afd5484f` and at `a9fb2ca7`. Every source citation below is static reading. Nothing was compiled or run.

## 2. Verdict

# **NEEDS CHANGES**

| CRITICAL | MAJOR | MINOR | NOTE |
| --- | --- | --- | --- |
| 0 | 2 | 2 | 3 |

**There are two separate verdicts:**

- **Document fidelity:** NEEDS CHANGES. The supplement matches signed §4.2 on every schema, identity, historical-admission and refusal point I checked. But two of its own policy choices combine into a state the document describes wrongly (F-01), and it leaves open a question that §4.2 requires CG-TRIG to answer (F-02).
- **Future effective signature / implementation:** NOT ELIGIBLE. The Manager should not write the exact-byte D-A attestation until a bounded correction gets a changed-lines recheck that returns APPROVED. Even then, nothing here authorises source, Unity, ISO-58, #652 Ops or Windows work.

Policy B, T-04 strict refusal, the order↔juvenile pairing rule, W-A and the S1/M-A separation are sound, as far as these documents show (§4). Most of the findings come from how the CT-3 hold interacts with the admit-incomplete choices.

## 3. Findings

| ID | Severity | Location | Evidence | Defect | Required correction (Manager's choice; no fix is invented here) |
| --- | --- | --- | --- | --- | --- |
| F-01 | MAJOR | C:28, C:30, C:38, C:52, C:79–81 | C:52 holds advance "Once CT-1 true and correct CT-2 false". C:51 / SAVE:167 make CT-2 valid only with event9.year = field = Y\*, written together. C:38 says a later act "cannot be backdated" | For bare state (ii) (CT-1 true, review −1, current year > Y\*), CT-2 can never become valid, so by C:52's own predicate **the hold never lifts and the admitted forest can never advance another year**. The same applies to state (iii) whenever a matching event9+field pair cannot be written (see F-02). C:38 and C:80 call the result "no T1 credit" with a "diagnostic requiring a valid covering review". That review cannot be supplied, so the document hides a permanent annual-advance deadlock behind the words "admit incomplete". T-05 picked admission to avoid trapping players, but as written the slot loads into a frozen world | State the real player consequence of (ii) and (iii) under the selected rules. Then choose and label one explicit disposition: for example, whether CT-3 applies to a state where CT-2 has become impossible, or whether such states refuse instead (#673's strict option, R673:166–168), or whether this goes to the owner as material gameplay. No migration or late-credit rule is required, and none is proposed here |
| F-02 | MAJOR | C:51, C:56, C:81; SAVE:167 | SAVE:167: "Whether existing annual-review acknowledgement supplies that proof, and the exact binding/read signal if not, must be specified by signed CG-TRIG; implementers must not choose a substitute." Today the only event9 writer is UI:438–444 → M:391–401, once per year, deduplicated by CR:62–68. DL:151 (D-084) names "authentic event-9 acknowledgement of the Annual Review that covers the final planting year" | C never says whether the existing per-year Annual Review acknowledgement **is** the covering acknowledgement in Compact. If it is, it must write `reviewAcknowledgedYear=Y*` in the same mutation whenever CT-1 holds and CurrentYear = Y\*. If it is not, C must define a separate control and say how CR:62–68's one-event9-per-year rule treats it. Without this binding, ordinary play can produce state (iii): a generic acknowledgement at Y\* records event9, and then deduplication stops a later covering event9 in the same year. C:81 forbids repairing the field, so the result is the F-01 deadlock | Answer the delegated SAVE:167 question explicitly: bind the covering acknowledgement to the existing control or to a named separate control, and state how the field write and event9 deduplication fit together. Then restate C:81's outcome for (iii) when the current year is still Y\* |
| F-03 | MINOR | C:30, C:65, C:77–78 | SAVE:165 allows replacement only of a "failed group reference"; C:65 covers Failed/cancelled only | Bare state (i) (Completed designated order, no row) and four valid 3+1 rows each fill all four group slots with Completed orders that can never be replaced. T1 is then **permanently unreachable**, with no deadlock. C states this kind of limit for the late-review case (C:38) but not for these two states. Neither can come from a correct producer (C:36, C:48), which is why this is MINOR | Add one sentence each that T1 cannot be reached and that no replacement or repair route exists |
| F-04 | MINOR | C:36, C:78, C:81, C:84 | C:36 "If an assertion of completion is persisted…" | The line between "claim → refuse" and "bare → admit" is the core of T-05, but C never lists what a persisted **claim** is. C:78's "false completion-evidence *claim*" and C:81's "asserted acknowledgement claim" suggest more claim forms than the schema has. By my reading of SAVE:165, the only persisted claims are `reviewAcknowledgedYear ≥ 0` and each `plantings` row. Event9 alone is ordinary annual history (D-077 and the historical game write it every year), never a teaching claim. Two implementers could draw this line differently | List the claim set exhaustively, or state the rule that decides it, so validators agree |
| N-01 | NOTE | C:11–12 | [#677 receipt](https://github.com/jermur788/CCF/issues/677#issuecomment-6954721854), 16:51 UTC | The owner's words "Yes, that's fine, continue." and the 16:48 time are reported by the Manager. The owner's original reply is not visible in GitHub, so I cannot verify it independently. Scope is described correctly as technical policy only, excluding world content and material gameplay changes | None required |
| N-02 | NOTE | C:48 | — | "Exact zero planar tolerance" compares float positions exactly after a JsonUtility save/load round trip. Whether that round trip is exact is UNVERIFIED and needs a native witness. It is a tolerance choice, not a new wire key | Carry into the native witness list |
| N-03 | NOTE | C:16, C:20 | `git merge-base --is-ancestor 200add4e c866e3c9` is false. The four signed files exist only at `200add4e` and three side branches | The signed contract is not on main or in the candidate's lineage. I checked fidelity against `200add4e` directly. The candidate changes no file in that directory | None |

## 4. Answers to the ten required questions

1. **Signer authority — PASS (N-01).** C:11–12 and C:18 separate the Manager's technical choices from world-content signature and material gameplay. The banners at C:3–5 and the procedure at C:98 keep "selected" separate from a later exact-byte signature.
2. **Policy B and §4.2 — PASS.** C:26 and C:83 admit empty and nonempty groups with false booleans as incomplete with no credit. This matches SAVE:167's "simply remains incomplete" and goes beyond R673:183, which covered nonempty groups only. C:20 and C:58 keep the closed graph, the (schema1, payload24) tuple and v1–v24 admission (SAVE:155–160, 200–212). No field is added to the historical DTO.
3. **Three bare states — NEEDS CHANGES.** I agree that none of the three is a *claim* under SAVE:165–167, so admitting them does not conflict with signed refusal (but see F-04). The producer-atomicity warning (C:36) and the ban on backdating (C:38) correctly rule out fake repair. However, (ii) and (iii) deadlock under CT-3 (F-01); (iii) can be reached in ordinary play while F-02 is open; and (i) is undisclosed as permanent (F-03). Static source shows that a correct producer cannot reach (ii): M:1280 sets `report.year = EcologicalYear + 1`, M:1921 sets `resolvedYear = report.year`, and M:1313–1315 dates WorkResolved to that year, so after the final resolving advance CurrentYear = Y\* while the hold is active.
4. **Read signals — PASS except the F-02 binding.** C:56 rightly says that explicit interaction plus validated route is not cryptographic proof of reading. EV:40–70 has no attestation field, and C adds no wire key, so the seven envelope keys and the evidence keys are unchanged.
5. **Chronology — PASS.** C:50 says completionYear is the resolution/report year, never createdYear, consistent with M:157, 1902 and 1921. C:51 orders event9 after WorkResolved, consistent with CR:62–68. C:54 says CT-3 is not a fifth success conjunct. C:52 keeps the hold across reload and covers any skip route, consistent with D-077 (DL:135).
6. **S03 atomicity — PASS.** C:62 bans pairing by list tail or coordinate. M:1896–1910 indexes the last juvenile in the list (`plantedJuveniles.Count - 1`), and C:62 correctly forbids carrying that over to pairing. C:63 lists the atomic effect cluster. C:62 also records that the current `void` resolution (M:1861–1924; stock deducted at :1891 before checked settlement at :1917) is not proven exception-safe, so source gap and future policy stay separate.
7. **S04 W-A — PASS.** The gap is confirmed at M:879–880 and 887–888, where `IsOpen` includes Approved (WO:72). C:64 requires UI **and** service guards and limits this to Compact. Cancellation reuses the existing OrderCancelled removal (M:1210–1218) with no new status. C:32 labels the confirmation and field freeze as Manager game calibration under D-084 (DL:151), not owner wording.
8. **S1/M-A separation — PASS.** C:33 and C:94: no S1 key, version, sidecar or fourth root; OD-16 stays deferred; three *retained* crop trees, with no Fell quota.
9. **Compatibility and witnesses — PASS.** Every matrix row is marked NOT RUN (C:70, C:90). No signed file was changed (N-03). #652 and ISO-58 stay held (C:96). I found no runtime or test claims.
10. **Consistency — NEEDS CHANGES only where F-01/F-02 affect it.** Elsewhere, the register (C:24–34), matrix (C:70–88), headline and status (C:3–5, C:101) agree. The old #673 banner is untouched. F-03 and F-04 are MINOR and need only the changed-lines recheck, not a full re-review.

## 5. Red/green checks run (static only)

| Check | Result |
| --- | --- |
| Candidate commit, blob and character count match the packet | GREEN |
| Candidate changes only its own file; no spec, Docs/Project, source or asset change | GREEN |
| Source blobs identical at `afd5484f` and `a9fb2ca7` | GREEN |
| No new wire key / root / DTO field / enum value in C | GREEN |
| completionYear = resolution year (M:1280/1315/1902/1921) | GREEN |
| W-A demotion gap still present in source (M:879–880, 887–888) | GREEN (gap confirmed) |
| CT-3 hold can be released in admitted states (ii) and (iii) | **RED** (F-01) |
| §4.2's delegated question about the existing acknowledgement is answered | **RED** (F-02) |

**NOT RUN:** compiler, Unity Editor/Player, tests, save/load, failure injection, native read signals, Ops, Windows. **NOT DONE:** signature, spec approval, edits to the candidate, board or canonical docs.

## 6. Handoff

- **To Manager #675 (author):** make one bounded correction on the #677 docs branch covering F-01 and F-02, plus F-03 and F-04 if convenient. Then ask for a changed-lines-only recheck pinned to the new commit/blob. Do **not** sign `c866e3c9`.
- **If F-01's disposition changes player-facing gameplay** (for example a world that can never advance, or slot refusal), C:38 and C:90 already say this goes back to the owner.
- The recheck should look only at changed lines and at C:30/38/51–52/78–81, and confirm that N-01–N-03 have not become errors.

---

## 7. Delta re-check — changed lines only (2026-11-17)

| Field | Observed value |
| --- | --- |
| Reviewer | Same original reviewer: Anthropic Claude Opus 5.5 (`claude-opus-5-5`), Claude Code in T3 Code, `jer-laptop`. Not an author of #677 |
| Registration | 2026-11-17 17:43 UTC, [#681 receipt](https://github.com/jermur788/CCF/issues/681#issuecomment-6956071418), before this append |
| Starting HEAD / branch | `a47ddd6f1762efdaaace04523cb466accc253e23` on `review/sen-1-681-compact-save-supplement-independent-20261117-v1` (matched remote, clean); live main `ce533b2a`, with `Assets/` unchanged since `a9fb2ca7` |
| Original / corrected | `c866e3c9` (blob `b6620569`) → `bfa01f86310bd013b87ff6686958e8b73315b591`, blob `bb9327fe6c9b1ae1f7d5888e385dc9edc3ad072b`, 33,198 characters. Verified |
| Changed lines | `git diff -U0` shows hunks exactly at 27, 30, 36, 38, 51–52, 54, 58, 77–82, 84, 90 and 101: **17 lines**, matching the packet. `c866e3c9..bfa01f86` touches only the supplement and the review handoff. No signed spec, source, Docs/Project or #673 file changed |

"D" below means the corrected candidate at `bfa01f86`.

### Finding dispositions

| ID | Result | Evidence |
| --- | --- | --- |
| F-01 | **RESOLVED** | D:30, D:38, D:80 and D:81 now **refuse Save capture and Load before any mutation** in both deadlock states: (a) CT-1 true, review −1, current year > Y\*; (b) CT-1 true, an event9 already present for Y\*, field −1. They state the player consequence plainly ("cannot load this slot", D:80) instead of "no credit". D:52 and D:54 now say the hold can be reached and released in every *admitted* CT-1 true / CT-2 false state. The refusal is labelled a delegated technical integrity choice taken from #673's explicit signer alternatives (D:30, D:90; R673:166–169), not a reading of §4.2. Any backup offer goes only through the signed FR-06 validated backup-to-memory route (D:38; SAVE:198) |
| F-02 | **RESOLVED** | D:27, D:51 and D:54 bind the **existing** acknowledgement (UI:438–444 → M:391–401) as the only Compact covering control. It must write event9 and `reviewAcknowledgedYear=Y*` in one logical commit, with CR:62–68 deduplication applied to the combined operation, a repeat press idempotent, and no second button or event9. Before CT-1, event9 stays generic history (D:51, D:58). This answers the question §4.2 delegated to CG-TRIG (SAVE:167) and matches D-084's "authentic event-9 acknowledgement" (DL:151) |
| F-03 | **RESOLVED** | D:77 and D:78 now say T1 is permanently unreachable for 3+1 Completed rows and for a Completed member with no row, because §4.2 lets only failed/cancelled references be replaced (SAVE:165). Ordinary management continues, because CT-1 is false and so CT-3 never engages |
| F-04 | **RESOLVED** | D:36 lists the full positive-claim set as each `plantings` row and `reviewAcknowledgedYear >= 0`. The booleans, group IDs, a Completed order and event9 alone are each classified as not being claims. This is consistent with D:58, D:78 and D:84 and with the SAVE:165 key set |

### Is the new refusal reachable in ordinary play? (static, NOT RUN)

I checked whether ordinary play could produce either refused state, which would be a hidden material gameplay change:

- Rows are added only at resolution, and their year is the year of the advance being resolved (M:1280, M:1921). Designation is allowed only for Pending orders (D:48; SAVE:165). So CT-1 can become true only during an advance, and its last row carries the maximum year. That means Y\* is always the year in which CT-1 became true, and CurrentYear = Y\* right after that advance.
- The only event9 writer requires an existing report (M:393) and dates the event to CurrentYear (M:398). Report Y\* is created by that same advance. So no event9 dated Y\* can exist before CT-1 becomes true, and under D:51 the first event9 at Y\* is the combined write.
- State (a) needs an advance past Y\* while CT-3 is active. D:52 forbids that on every route, including any future skip (consistent with D-077, DL:135).

**Conclusion:** if the future producer honours the D:51 atomic binding and the D:52 all-route hold, neither refused state can come from ordinary play. They come only from producer defects or edited files. D:38 and D:90 say to stop and escalate to the owner if native evidence ever shows otherwise. **I found no material gameplay consequence that needs an owner decision now.** These are prospective requirements: current source has no Compact producer and no CT-3 hold (original review §4).

**Compatibility with signed §4.2:** compatible. §4.2 contains no requirement to admit these two states, and it gives the exact review binding and edge cases to CG-TRIG (SAVE:163, SAVE:167). SAVE:167's "an incomplete … group simply remains incomplete" still governs every admitted partial state, and Policy B is unchanged (D:36, D:58). No key, root, tuple, enum value or historical v1–v24 rule is added or changed (D:36, D:58, D:90).

### New items

| ID | Severity | Location | Observation | Disposition |
| --- | --- | --- | --- | --- |
| D-N1 | NOTE | D:30 vs D:51, D:81 | D:30 limits refusal case (b) to an event9 at Y\* "at or after the final WorkResolved". D:51 and D:81 leave that qualifier out. An event9 dated Y\* but *before* the final WorkResolved cannot be produced (see above). CR:62–68 would still allow a valid covering event9 in that case, so admitting it would not deadlock. The two readings differ only for that edited-file state, and neither creates a trap | Non-blocking. A future spec can align the wording; no further recheck needed |

**Earlier notes:** N-01 unchanged. N-02 is now carried into the native-qualification list (D:90), so it is addressed. N-03 unchanged (the signed files are still only at `200add4e`).

**Counts for this delta:** CRITICAL 0 / MAJOR 0 / MINOR 0 / NOTE 1 (new).

# **DELTA APPROVED**

This approves the document fidelity of the exact corrected bytes `bfa01f86` / blob `bb9327fe6c9b1ae1f7d5888e385dc9edc3ad072b` only. It is **not** a signature. Manager #675 may now write its separate out-of-band exact-byte D-A attestation pinning those bytes. That attestation would not authorise source, Unity, the Compact specs, ISO-58, #652 Ops, #676 world content, S1/OD-16 or Windows work.

**NOT RUN:** compiler, Unity, tests, save/load, native read-signal or deadlock witnesses, Ops, Windows. This is static document and source reading only.
