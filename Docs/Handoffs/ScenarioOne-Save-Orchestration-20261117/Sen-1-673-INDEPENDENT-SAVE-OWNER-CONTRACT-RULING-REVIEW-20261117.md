# Sen-1 #673 — Independent S3 review of the CG-TRIG / M-A save-owner contract ruling candidate

**REVIEW OF A CANDIDATE DOCUMENT ONLY. Not a signature, not save-policy approval, not spec approval, not an ISO-58 release, not Unity or runtime evidence.**

## 1. Identity, independence and pins

| Item | Observed value |
| --- | --- |
| Reviewer role | S3 independent non-author reviewer, dispatched by DeepSeek orchestrator B (#673); Manager #671 |
| Actual model / provider | Anthropic Claude Opus 5.5 (`claude-opus-5-5`, as the harness reports it), run through the Claude Code harness inside T3 Code. Effort "high" is the dispatched setting; I cannot observe it directly |
| Host / UTC start | `hostname` → `jer-laptop`; `date -u` → `Tue 17 Nov 2026 13:16:30 UTC` |
| Independence | Different model family (Anthropic) and session from the self-reported author, OpenAI GPT-6.1 Sol. I wrote no part of the reviewed file or its sources |
| Review branch / start HEAD | `review/sen-1-673-deepseek-save-ruling-20261117-v1`, created at `afd5484fc11697e6bde64f99d57d22e98f572f1f` (`origin/main` matched) |
| Author branch / reviewed tip / base | `docs/sen-1-673-save-contract-rulings-20261117-v1` @ `a43fbef33589bc2ce2df0e19f557036d8b446c9f` (matched); base `afd5484f` (ancestor confirmed) |
| Reviewed file | `Docs/Handoffs/ScenarioOne-Save-Orchestration-20261117/Sen-1-673-COMPACT30-CGTRIG-AND-MAE-SAVE-OWNER-CONTRACT-RULING-20261117.md` |
| Lines / sha256 (my own run) | 336 / `4cbaa1f99e56b0ea60f767aad35034543d7ce20b67eca18521a7d4acdadd7e45`. Both match the reported values |
| Signed source | `200add4e:specs/scenario-independent-save-isolation-v1/spec.md`, blob `8ac551022adb578abed0d9fbd413c8e9d002c3f9`. This matches the blob listed at SIG:22 |

Below, `D:n` means line n of the reviewed document. `SAVE`, `M`, `WO`, `EV` and the other aliases mean the same files as D:44–59.

## 2. Verdict

# **APPROVED — CONTRACT DOCUMENT QUALITY ONLY**

**Severity counts: CRITICAL 0 · MAJOR 0 · MINOR 3 · NOTE 3.**

The candidate restates signed §4.2 accurately and separates case (a) admission (B) from case (b) forged-evidence refusal. Its chronology is coherent, its W-A and pairing gaps are grounded in source, and it keeps S1 and SCOPE separate. Every C# and source citation I spot-checked resolves exactly.

The three MINOR findings are about how clear the document is for the signer. They are not fidelity defects. I recommend fixing them before the real signer uses the document, but they do not block it.

This verdict does not approve policy B, the CG-TRIG controls, the review-edge rules, W-A guards, S1, or any classifier.

## 3. Findings

| ID | Sev | Doc line | Exact source evidence | What is wrong | Required correction |
| --- | --- | --- | --- | --- | --- |
| F-01 | MINOR | D:165–169 vs D:175–176 | SAVE:167 says "A false acknowledgement or incomplete/cancelled/failed group simply remains incomplete" and "A review acknowledgement without all required work/report evidence or any fabricated/mismatched link refuses". E667:146 offered the signer two options: "refusal of impossible chronology **or** a strictly incomplete presentation without synthetic credit" | D:176 rejects Policy A because it "would add a stricter exception to immutable text". But D:165 and D:166 then propose refusal for two states where `reviewAcknowledgedYear = −1`: (i) CT-1 true while the year is already past Y\*, and (ii) a post-resolution event9 at Y\* with the field still −1. Neither state makes an acknowledgement claim, so on their face both are "incomplete" under SAVE:167. D:169 does label them stricter and needing signer approval, so this is not silent. However, the document does not say why stricter refusal is acceptable here but not in case (a). It also leaves out E667:146's non-refusal alternative | Add one sentence giving the basis for the distinction. For example: SAVE:163/167 delegate the review binding and "exact positive/negative/midway-reload cases" to CG-TRIG, while the meaning of the acknowledgement booleans is already fixed by SAVE:167. Then list E667:146's alternative and its consequence for both states: under refusal the file cannot be loaded; under admit-incomplete the file loads but can never complete. The signer should choose with both options in view |
| F-02 | MINOR | D:242 | SAVE:165 "Final group is exactly two sessile oak and two European beech". The C proposal caps designation at at most 2 per species (D:130) | The 3+1 matrix row says "proposed designation/strict final-group validation refuses". It does not say whether **Validate/Save/Load** refuses before mutation or admits the save as incomplete. It also does not say whether the load verdict comes from S (the signed 2+2 rule) or from C. The signed sentence that an "incomplete … group simply remains incomplete" could support either reading | State one load verdict in the Validate/Save/Load column (refuse-before-mutation or admit-incomplete-no-credit) and attribute it to S or C. If the verdict is refusal derived from C, list it with the D:169 stricter edges that need explicit signer approval |
| F-03 | MINOR | D:222–247 | D:164 says a Completed designated order with no row refuses. D:165 says the late-year state refuses. P673:10 requires partial, older-save and mismatched cases. SAVE:167 says a false acknowledgement remains incomplete | The matrix has no explicit row for three states: (i) a Completed group order with no planting row; (ii) CT-1 true with review −1 while the current year is past Y\*; (iii) `assessmentAcknowledged=true` with `arrivalAcknowledged=false` and an empty group. The "Arrival only" row covers only the opposite order, and B (D:181) addresses only nonempty groups. Rows (i) and (ii) may be implied by "Mismatched chronology", but they are not named, so a later witness could miss them | Add three NOT RUN rows with their expected verdicts and provenance (C for the D:164/D:165 refusals; S for incomplete booleans, and say whether B's wording also covers an empty group) |
| N-01 | NOTE | D:203–204 | DL:151 (D-084 W-A): "block direct edits to Approved orders, retain audited cancel-approval as removal, then designate/quote/approve a NEW order" | The heading "OWNER CHOICE DL:151 / proposed guards" covers both owner intent and author additions: the list of immutable fields and "explicit player confirmation, decline changes nothing". D-084 does not state those additions; they come from E667 S04 | Optional: label the field list and the confirmation rule as GAME-CALIBRATION-PROPOSAL, kept separate from the D-084 text |
| N-02 | NOTE | D:133–134, D:156 | Static source: M:1280 `report.year = EcologicalYear + 1`; M:1921 `resolvedYear = report.year`; M:1315 `WorkResolved.year = report.year`; M:1902 `plantingYear = report.year`; ForestEcologyController.cs:467 `ecologicalYear++`; M:157 `createdYear = EcologicalYear` at designation | The equality Y\* = report year = current year holds in source only if `completionYear` is taken from `resolvedYear` (or from the plantingYear of the same resolution). If it were taken from `createdYear`, it could be a year earlier. The document implies the correct binding but never says it | Optional: state that `completionYear` is the order's resolution or report year and never `createdYear`, labelled MODEL-DERIVED from the cited lines |
| N-03 | NOTE | D:15, D:23 | #673 comment 6943166378 was created `2026-11-17T08:44:55Z`. The author's `date -u` was 08:43:40 | "Posted by parent before this file's first write" fits the timestamps but cannot be checked from Git. The document's first and only commit is the reviewed tip | None required. The claim is recorded here as author-reported and unverified by me |

## 4. Checked for and not found

- **Invented wire keys, fields or versions.** Not found. The S1 table (D:277–292) explicitly adopts no names or literals. The eight signer questions (D:296–303) leave placement, keys and allowlist to the signer. No new enum or Cancelled status appears (D:197, D:213).
- **Fourth root or historical DTO field.** Not found. D:95 forbids `juvenileId` on WO/EV, `workOrderId` on SD, and any M-A or historical-v24 additions. D:296 restates the closed three-root graph.
- **Signature drift.** Not found. Every APPROVED/SIGNED/verified/complete/pass occurrence refers to one of three things: the existing SIG attestation (D:63, D:69), static SOURCE-VERIFIED reading as defined at D:34, or negated or gated status. D:3–7, D:192 and D:326–336 keep CONTRACT SIGNATURE BLOCKED, DRAFT specs and ISO-58 BLOCKED.
- **M-A-SCOPE decision.** Not found. D:113 and D:312–314 keep it DEFERRED. The 08:34Z receipt ("yes 3 crop trees") is quoted faithfully and is not stretched into a Fell quota, a one-stem ruling or S1 authority (D:309–313). Carrying "three … crop-tree identities" into S1 content (D:284) matches the receipt's "Three crop-tree IDs and retained status should be tracked".
- **Weakened old-save rules.** Not found. D:98–99 and D:246 preserve historical v1–v24 admission (SAVE:200–212), Legacy40/Enlarged80, Rack V2/MFM, prefs and the frozen Reference.
- **Credit leak.** Not found. B withholds T1 while either boolean is false (D:181). Case (b) refuses before mutation without dropping or repairing evidence (D:183–186). Opening a review, approving work, or accepting a quote never awards credit (D:131, D:237).

## 5. What I verified and how

- **Scope.** `git diff --name-status afd5484f a43fbef3` shows exactly one `A` entry, the reviewed file. There is one commit and no spec, source, `Docs/Project`, coordination, save, Unity or `.meta` change. `gh pr list --head` for the author branch returns no PR in any state. P673 at `af3f3253` and O673 at `59280842` are byte-identical to their remote branch tips.
- **§4.2 fidelity, field by field (D:72–93 against SAVE:148/155/159–167).** Checked: three roots, each present once, with recursive duplicate rejection; the closed graph; signed32 integers; seven envelope keys with schema1/payload24/nested24 and exact case-sensitive strings; `evidenceVersion` 1; the two booleans; `groupOrderIds` holding up to four unique positive IDs; the four row keys `workOrderId`/`speciesId` (`sessile-oak`|`beech`)/`juvenileId`/`completionYear≥0`; four maximum with unique IDs; final group 2+2; rows added only on successful normal resolution; reconciliation with retained Completed orders or history; later death does not invalidate credit; review year = report year = max completionYear with the report retained; no silent repair; cancellation recorded as an event. All match verbatim or in exact meaning.
- **Admission.** B matches SAVE:167's "simply remains incomplete" and E667:150–175. Case (b) stays as reject-before-mutation (SAVE:167; FR-02 SAVE:190). For the review-edge strictness, see F-01.
- **Chronology and CT-3.** D:133–135 and D:142–153 match source. The report is built at current+1 (M:1280). Work resolves before WorkResolved (M:1312–1315). The ecology year then advances (M:1350) and the report is retained (M:1361). The existing duplicate-acknowledgement guard is at CR:62–68, called from M:395. The existing hold is UI-only and first-review-only (UI:48, UI:462; WP:167–169), reset only at M:660. The manager API has no gate (M:324–333). CT-3 covers every advance route and any future skip route, survives reload, never auto-acknowledges, and is not a completion conjunct (D:151).
- **W-A.** I confirmed the batch demotion: M:879–880 (`order.status = ScenarioWorkStatus.Pending` over `IsOpen` planting orders) and M:887–888. `IsOpen` includes Approved (WO:72). The UI enables both controls through `CanEditWorkPlan` (WP:453, WP:458; M:333), and WP:461–462 itself says "changed approved work needs approval again". Cancellation removes the order and records `OrderCancelled` (M:1185–1218). The rack refusal is at M:1202–1206.
- **Pairing and atomicity.** M:1861–1924 is `void`. Stock is deducted at M:1891 before the juvenile is created at M:1896–1907, and the checked settlement follows at M:1917. The current shelter code infers the juvenile from the list tail (M:1910), which supports D:132's ban on such inference. No current field links an order to its juvenile (WO:29–72, EV:40–70, SD:215–228). The document is honest that this is not proven exception-atomic (D:157–159).
- **M-A.** The S2 insufficiency claims match T:16–23/89–100, SD:206, WO:34/60–70, EV:4–17, EC:52/909–937 and SNAP:20–44. S1 is presented only as an unsigned, separately versioned amendment.
- **Matrix.** All brief cases are present and every row is marked NOT RUN (D:215–220), with S/C/B/W provenance. For gaps, see F-02 and F-03.
- **Citation spot-checks (28, no drift found).** SAVE:148, 155, 159, 160, 161, 163, 165, 167, 182, 188–198, 200–212, 238; M:105–164, 324–333, 391–401, 395, 660, 875–889, 1185–1218, 1221–1250, 1280–1368, 1394–1399, 1514–1521, 1861–1924, 2045–2054; WO:13–19/34/60–72; EV:4–17/40–70; CR:62–68; SD:5–49/206/215–228; SC:77–86/261–274/324–340; UI:48/459–465; WP:167–169/450–462; T/EC/SNAP ranges above; E667:96/146/150–175/220–282/338–347; R667:20–33/85–90/163–174; P673:5; DL:149/151; SIG:3–16 and SIG:22; the #673 receipt (`2026-11-17T08:35:03Z`). The context commit `a5541b8a` exists and is an ancestor of `afd5484f`.

## 6. Explicit limits

Static, read-only, document and source review only. I ran no Unity, Editor, compiler, tests, harness, save/load or Prefs, and made no runtime or atomicity claim. Nothing here is a signature or acceptance of policy B, CG-TRIG, the review-edge rules, W-A, S1, M-A-SCOPE, ISO-58 or any DRAFT spec. I did not edit the reviewed file, any signed source, `Docs/Project`, coordination, specs or C#. I opened no PR.

## 7. Handoff

DeepSeek orchestrator B (#673) → author, for optional bounded corrections F-01..F-03 with a delta check limited to the changed lines → **the real authorised save-policy signer**, who decides B, the CG-TRIG controls, the review-edge rules, W-A and the S1 questions → **Overall Manager #671**. Until a real signature exists, the outcome remains **CONTRACT SIGNATURE BLOCKED** and ISO-58 remains **BLOCKED**.
