# Sen-1 — Compact Scenario One world: open decisions card

**Date:** 16 November 2026. **Task:** research sub-agent for orchestrator [#544](https://github.com/jermur788/CCF/issues/544), under the Sen-1 amendment section B/P3 "Compact" (`A:48`). **Author:** Claude Sonnet 5.5 (reasoning effort not attested). **Docs only:** no source, scene, prefab, save schema, availability flag, board or ledger was touched; Unity was not run; nothing here was tested.

> **STATUS: DECISION AID, NOT A DECISION RECORD.** Nothing on this card is approved. The owner decides. Lines marked **Rec (mine)** are my recommendation and carry no authority. The compact world is **PAUSED**; this card does not say it is playable, close to playable or approved.

## 0. How to read this

Every row below is an item the project already tracks. I used the project's own identifiers (OPEN-C1…C10 and D-082) and did not make up new ones. All claims come from `git show <ref>:path`, with line numbers taken from the pinned commits in the key. I read nothing from the working tree.

**Two corrections to what you may have been told:**
- The amendment says "OPEN-C1…C8" (`A:48`). The compact parent spec actually registers **ten**, OPEN-C1…C10 (`P:114-123`). Of those, **C3 is RESOLVED** and **C9 is PARTLY RESOLVED**, so **8 are fully open, 1 is partly open, 1 is closed**. One more gate, the canonical **D-082** exception, has no OPEN-C number but blocks the same work, so it is in the table too.
- The label **D-082 is now taken**. `main` records D-082 as the *Sitka annual-growth experiment* decision of 15 Nov (`D:146`). The compact first-demo exception is still called "D-082 / #471" in the compact papers (`P:4`, `R:119-127`, `S:64`). It will need a different decision number, and I could not establish which one (see section 6).

**Key (all pinned, read via `git show`):**

| Key | File | Pinned at |
| --- | --- | --- |
| `P` | `specs/scenario-one-compact-complete-loop-v1/spec.md` (parent DRAFT, #516) | `bce49d7a` (branch `docs/scen1-parent-no-rack-g1-2plus2-draft-revision-20261114-v1`) |
| `K` | `specs/scenario-one-compact-initial-planting-v1/spec.md` (G1 child DRAFT, #515) | `2a24a969` (branch `docs/scen1-manager-513-2plus2-planting-access-verdict-20261114-v1`) |
| `R` | `Docs/Handoffs/ScenarioOne-Compact-Ecology-20261114/scen1-OVERALL-425-COMPACT-PRODUCT-AND-SAVE-DECISION-RESULT-20261114.md` (#521 Manager intake) | `467244f3` |
| `S` | `specs/scenario-independent-save-isolation-v1/spec.md` (signed save contract) | `200add4e` (immutable, signed) |
| `ISO` | `specs/scenario-independent-save-isolation-v1/checklists/requirements.md` | `200add4e` |
| `G` | `Docs/Handoffs/ScenarioOne-Save-Orchestration-20261115/scen1-MANAGER-SHARED-SAVE-V2-IMMUTABLE-SPEC-SIGNATURE-20261115.md` | `c761d72e` |
| `H` | `Docs/Handoffs/ScenarioOne-Save-Orchestration-20261115/scen1-DEEPSEEK-SHARED-SAVE-AND-COMPACT-IMPLEMENTATION-ORCHESTRATOR-HANDOFF-20261115.md` | `b5ae52d4` |
| `A` | `Docs/Handoffs/ScenarioOne-Save-Orchestration-20261115/Sen-1-OVERNIGHT-PLAYER-TEST-READINESS-AMENDMENT-20261115.md` | `b777efe6` |
| `D` | `Docs/Project/decision-log.md` | `origin/main` `660492c6` |

**What "state" means below.** Column "Spec state" is what `P` says. `P` was last changed on 14 Nov, before both the #521 Manager intake (`R`) and the 15 Nov save signature (`G`). I show what `R` later *selected* as a "Manager-selected default". A Manager-selected default is a reversible proposal, not an owner decision (`R:17`).

## 1. Open decisions

Key to columns: **Q** = exact question; **Blocks** = why it blocks; **Artefact** = what cannot be approved or written until it is answered.

| ID | Q (one sentence) | Blocks | Realistic options → consequence | Artefact blocked | Cite |
| --- | --- | --- | --- | --- | --- |
| **OPEN-C1** | What must the player see or do on arrival (entrance → orientation → stand/G1 assessment) for that to count as the compact first-use lesson? | Compact has no rack, so the old P/A rack choice no longer applies (`P:114`). The signed schema's arrival and assessment acknowledgements have no defined trigger (`S:163`). | **1.** One short in-world entrance → orientation → assessment intro, one acknowledgement each (`R:34`, Manager-selected). Cheapest; the signed schema already stores both acknowledgements (`S:167`).<br>**2.** Longer guided walk with a minimum-inspection step. Richer, but `R:34` forbids an arbitrary inspection quota and the extra state is not in the signed schema, so it needs an amendment.<br>**3.** No arrival gate. Simplest, but the signed completion rule needs both acknowledgements (`S:167`), so it needs an amendment.<br>**Rec (mine): 1**, the only option that needs no change to the signed contract. | Compact teaching contract (CG-TRIG, see OPEN-C7); parent S-06/S-14 | `P:114`, `P:64`, `R:34`, `S:163`, `S:167` |
| **OPEN-C2** | Do you ratify the compact world as 30×30 m (6×6 = 36 five-metre cells), an H1 12×10 m entrance clearing with the 14 listed original-tree omissions (189 → 175 Sitka), and the existing winding Route A path? | The signature reserves `Compact30=2` as a number only and ratifies no dimensions, cells, H1, path or omissions (`G:31`, `S:68`). The generator and save identity need a signed world manifest (`S:282`). | **1.** Ratify the `R` §4 baseline as-is (`R:49-57`). Smallest rework; the #508 numbers (14 conflicts, G1 Year0 light 0.221) were measured on exactly this H1. 175 holds only if exactly these 14 are omitted; any later change is a new versioned identity (`R:55`, `R:86`).<br>**2.** Keep 30×30 m but change the H1 footprint or roster. The 14-conflict count and 0.221 light no longer apply, so a new source-only screen is needed first.<br>**3.** Choose a different size (e.g. 20 m: less space, more near-edge trees, `K:13`). Reopens #508/#460 comparisons that `R:35` says not to reopen. Longest delay.<br>**Rec (mine): 1**, with the caveat that 3D path appearance, walking time and post-H1 light are all NOT RUN (`P:61`). | Signed world manifest; parent + child APPROVED; any compact generator; G1 coordinates | `P:115`, `P:61`, `R:35`, `R:47-57`, `S:282`, `S:148`, `K:13`, `H:50` |
| **OPEN-C4** | Do you accept the signed save contract's identity literals, envelope and learning/help prefs as the compact identity, leaving only world-content binding and the permanent legacy future-version policy open? | `P` still lists C4 as open because it predates the 15 Nov signature. The signature approved the literals and envelope for the persistence contract only (`G:31`); content binding awaits the world manifest (`S:148`). | **1.** Treat C4 as closed by the signed contract. Fold the residual (bind `scen1-compact-h1-g1-v1` to a manifest) into OPEN-C2. The permanent legacy future-version policy stays deferred (`S:287`) and does not block compact.<br>**2.** Reopen the literals. The signature is immutable; any content change needs a fresh bounded acceptance (`G:6`).<br>**Rec (mine): 1.** *This is my reading of the documents, not a Manager ruling.* | Parent register row only; manifest binding | `P:117`, `R:37`, `R:83`, `G:6`, `G:31`, `S:64`, `S:148`, `S:287` |
| **OPEN-C5** | How is the compact training site funded, and what counts as the "substantive" repeat intervention that completes the scenario (instead of the draft's unratified three-cycle quota)? | `H:50` requires both a budget for affordable later interventions and a testable non-token completion predicate before any compact H1/path/G1 code. The signed schema deliberately defines no full-scenario completion (`S:220`). | **1.** Manager minimum (`R:109-113`): finite labelled training budget on existing Work Plan actors (keep €12,000 start cash and the €2,500 minimum harvest job), no profit requirement, an initial plus at least one later substantive intervention, a 3-year minimum as a *candidate* calibration. The classifier is still unwritten.<br>**2.** Sponsored/absorbed optional training operations (`P:75`). Removes cost friction but needs a funding mechanism not in source.<br>**3.** A "local tree surgeon" small-job service (`P:75`). A new service type; its scope and safety claims are not established.<br>(Keeping the draft's three-cycle quota is declined by `R:113` as arbitrary.)<br>**Rec (mine): 1**, once the classifier is written. All € figures are game calibrations, not Irish rates (`P:75`). | Parent S-11/S-13/S-15; compact completion rule (not in signed schema); a save-evidence extension | `P:118`, `P:75`, `P:81`, `P:85`, `R:38`, `R:109-113`, `H:50`, `S:220` |
| **OPEN-C6** | Is the opening-removal overlay (SelectedOpeningRemovalSet) left out of the first compact demo? | Only blocks the overlay requirement (S-17). Nothing else depends on it. | **1.** Defer entirely (`R:39`, Manager-selected). No input-status contract needed.<br>**2.** Include with exactly one status source (draft / Pending / Approved / selector). Needs native geometry and render evidence first.<br>**Rec (mine): 1.** | Parent S-17 only | `P:119`, `P:90`, `R:39` |
| **OPEN-C7** (= **CG-TRIG**) | Which exact player actions or events count as: arrival acknowledged, assessment acknowledged, G1 group designated/cancelled/replaced, and the covering annual review opened/acknowledged? | The signed contract names this "CG-TRIG" and holds **ISO-58 BLOCKED** until a separate, owner-accepted compact teaching contract defines the controls, events, persist/reset rules and positive/negative/midway-reload cases (`S:285`). Implementers may not choose the triggers (`S:163`). | **1.** Adopt the `R:105` chain: acknowledge arrival → assess stand → four owned-stock Pending G1 orders → Work Plan approval → four completed `PlantJuvenile` → first covering annual review opened. Matches the signed schema (`S:167`); what remains is naming the exact controls and reload cases.<br>**2.** A shorter chain (drop assessment or review-read). Conflicts with `S:167` → needs a signed-contract amendment and fresh review.<br>**3.** No teaching-complete signal. ISO-58 stays BLOCKED, so whole-feature qualification cannot pass (`S:285`).<br>**Rec (mine): 1.** A "yes" in chat is not enough: the trigger contract itself must be written as a spec. | Compact teaching contract (new/revised spec); ISO-58; skip eligibility (S-14); T-03 whole-feature acceptance | `P:120`, `P:83`, `S:163`, `S:167`, `S:285`, `ISO:104`, `R:40`, `R:105` |
| **OPEN-C8** | When an already **Approved** Work Plan order must change, does the game block the edit or offer withdraw → requote → reapprove? | Needed before Stage C (Work Plan). Not needed for save routing. | **1.** Visibly block edits to Approved orders with explanatory text (`R:41`, Manager-selected). No new API.<br>**2.** Explicit withdraw/requote/reapprove. Friendlier but `R:41` warns against an invented cancel API; needs a source-capability check first.<br>**Rec (mine): 1.** | Parent S-08; Stage C | `P:121`, `P:68`, `R:41` |
| **OPEN-C9** (partly resolved) | Do you ratify the first planting episode as G1 = 2 sessile Oak + 2 European Beech, at the first genuinely eligible opportunity (window Year 0–2), as the *reversible* default? | The owner already accepted one early Oak-and-Beech episode (`R:42`). Still unratified: the 2+2 number and the four coordinates (candidates only, `R:59-68`). The signed contract hard-codes exactly 2 oak + 2 beech (`S:220`). | **1.** Ratify 2+2 as default. One native witness after approval checks post-H1 light, admission, stock, protection. A failed admission pauses the lesson; it never adds fells or boosts survival (`R:42`).<br>**2.** A different count or layout (e.g. the former 4+4). The #508/#513 screen found no defensible second healthy patch (`K:24`, `K:53`), and any count change amends signed `S:220`.<br>**3.** Ratify the episode but leave numbers and year to after the native witness. Delays the CG-TRIG group definition.<br>**Rec (mine): 1.** | Child G1 spec APPROVED; ISO-58 group content | `P:122`, `R:42`, `R:59-68`, `K:24`, `S:220`, `H:50` |
| **OPEN-C10** | Is the guided-example plus independent-replay (paired same-start run) feature left out of the first compact demo? | Blocks nothing if deferred. If included it needs separate run IDs and slots beyond the signed three-profile scope. | **1.** Defer (`R:43`, Manager-selected). Ordinary revisiting of the same saved forest stays.<br>**2.** Include now. Needs a separate replay/history/save spec and save-owner approval of one router approach before any production change.<br>**Rec (mine): 1.** | Parent register; a future separate spec | `P:123`, `R:43` |
| **D-082 exception** (no OPEN-C id; canon #471) | Do you ratify the proposed canonical wording that makes the new compact profile the first complete external demo, and under which decision number? | Without an accepted exception, the historical Enlarged80 five-rack lessons stay mandatory and compact cannot be promoted. #471 is PAUSED (`R:127`). The label D-082 is already used on `main` (`D:146`). | **1.** Ratify `R:123` as written; #425 prepares a fresh patch under a non-colliding number; then one independent #471 review.<br>**2.** Ratify with edited wording, same route.<br>**3.** Do not ratify. Historical five-rack stays the demo; compact stays a research/design item.<br>**Rec (mine): 1**, because every other row assumes it. | Parent + child APPROVED; any compact "first demo" claim; menu order | `R:119-127`, `H:50`, `P:4`, `D:146`, `S:64` |

**Closed, for the record:** **OPEN-C3** is RESOLVED by the owner (Sitka-only start, no authored Oak or Beech) (`P:116`, `R:36`). It is not re-asked.

## 2. Minimum viable decision set

**Honest limit first.** No document defines a "minimum". `P:105` says to resolve *all* of C1/C2/C4–C10 before APPROVED, and `H:50` is the Manager's own pre-code gate. The set below is **my derivation** from those two plus the signed holds (`S:68`, `S:282-285`). It is necessary, not sufficient.

**Must answer before ANY compact world/gameplay source work could lawfully begin (5 answers):**

| # | Answer needed | Why it cannot wait |
| --- | --- | --- |
| M1 | **D-082 exception** (ratify wording; a new decision number is assigned) | `H:50`: canonical exception is a precondition to marking either compact spec APPROVED. |
| M2 | **OPEN-C2** (ratify or change 30×30 m / H1 / 14 omissions / Route A) | Signature withholds it (`G:31`, `S:282`); the world manifest cannot exist without it. |
| M3 | **OPEN-C1 + OPEN-C7** (the arrival-to-review trigger chain, i.e. CG-TRIG) | ISO-58 is blocked on it (`S:285`); one answer feeds both rows. |
| M4 | **OPEN-C5** (funding mechanism and the substantive-intervention rule) | `H:50` lists budget and completion predicate as pre-code gates. |
| M5 | **OPEN-C9** (ratify G1 = 2 Oak + 2 Beech as the reversible default) | The signed schema already hard-codes 2+2 (`S:220`); the child spec cannot be approved without it. |

**Not sufficient.** After M1–M5, the project still needs, in order: revised parent and child specs; a compact teaching contract that names the CG-TRIG controls and cases (`S:285`); a signed world manifest (`S:282`); an independent review of the changed gates and of the canonical #471 patch; Manager APPROVED on both compact specs; then #424 grants one source writer, a file allowlist and a real Unity lease (`H:50`, `R:143-144`, `A:28`). Only then is compact source work lawful.

**Already answered, confirm only:** OPEN-C4 (signed save contract, my reading above) and OPEN-C3 (resolved).

**Can wait (as decisions):** OPEN-C6, OPEN-C8, OPEN-C10. `R` gives each a Manager-selected default (defer, block edits, defer). C8 is needed only before Stage C. The permanent legacy future-version policy is deferred by the signature (`S:287`). A caveat: `P` still lists all three as OPEN, so the spec must be revised to show them as deferred or defaulted before APPROVED. That revision is the Manager's job and is not a fresh owner question.

**Not blocked by any of this:** the save-routing/staging slice (T-03A) already authorised under the signed contract and the amendment. It reserves `Compact30=2` as a number only and keeps Compact and O2 refusing (`A:29`). It is not compact source work.

## 3. What is NOT being asked

- Answering these does **not** approve the compact world as playable, near-playable or first-demo.
- It does **not** authorise any source, scene, prefab, asset, save-schema or availability-flag edit, now or automatically afterwards. Compact and O2 stay unavailable (`A:29`).
- It does **not** replace, amend or reopen the signed shared-save contract at `200add4e`. That contract stays immutable; any content change needs its own fresh acceptance (`G:6`).
- It does **not** start a claim, lease, Unity run, merge, Windows build or personal-playtest request.
- It does **not** promote compact over the protected historical five-rack Scenario One, Legacy40, Enlarged80 or the frozen Reference Future v1 (`A:9`).
- A Manager-selected default (`R`) or "Rec (mine)" above is not an owner decision.

## 4. Dependency note

| Kind | Rows | Notes |
| --- | --- | --- |
| **Purely product / world-authoring** | C2, C6, C9, C10, C1 (presentation), C5 (funding framing), D-082 wording | Can be answered as a choice. C2 and C9 still end up as numbers inside a signed world manifest. |
| **Touch save/persistence semantics (so the signed contract applies)** | C7/CG-TRIG, C1 (arrival acknowledgement), C9 (exactly 2+2), C5 (completion state), C2 (new identity on material change), C4, C10 | The signed schema stores arrival and assessment acknowledgements, the four designated order IDs, and the review acknowledgement (`S:167`). It stores no full-scenario completion (`S:220`). It has no paired-run slots. |
| **Need a spec amendment, not a chat answer** | C7/CG-TRIG (new compact teaching contract, `S:285`); C2 (signed world manifest, `S:282`); C5 completion classifier (testable rule + a save-evidence extension); D-082 (Decision Log + milestone patch via independent #471 review, `R:127`); C10 if ever included (separate spec); *any* change to exactly 2+2, to the acknowledgement set, or to literals (amends signed `S`) | A chat "yes" can be the owner decision, but the Manager must write it into a spec and have it reviewed before it counts. |
| **Answerable in chat, then recorded by the Manager** | C6 (defer), C8 (block edits), C9 (ratify 2+2), C4 (confirm closed), C1 choice | Still needs the register in `P` revised. |

## 5. Suggested one-reply format for the owner (optional)

```
D-082: ratify R:123 as written / edit / decline
C2:    ratify R:49-57 baseline / change H1 / change size
C1+C7: adopt R:105 chain / shorter chain / none
C5:    Manager minimum R:109-113 / sponsored / tree-surgeon
C9:    ratify G1 2+2 default / other
C6,C8,C10,C4: accept Manager defaults (defer / block / defer / closed)
```

## 6. What I could not establish

1. **The decision number** for the compact exception. `main` already uses D-082 for the Sitka annual-growth decision (`D:146`), and the owner confirmed those D-082 choices on the #425 thread at 2026-11-16T00:14Z. The signed save spec, the amendment and the compact papers still say "D-082". Nothing I read assigns a new number.
2. **Whether the owner or #425 has answered any item since 15 Nov.** I read only the most recent comments on #471, #425, #424 and #544 (as of 2026-11-16 ~00:15Z) and found no compact-world answer. I did not read every comment on those issues, nor #521, #471's older comments or #519 in full.
3. **Whether `P`/`K` have been updated since 14 Nov.** I scanned every `origin/*` branch carrying either spec path; the newest parent change is `3606dc62` (14 Nov 22:11) and the newest child change is `f71abf65`. `P`'s register does not reflect `R` or `G`, so the C4/C6/C8/C10 status I give is `P` + `R` + my reading.
4. **Who may answer.** `R` routes C2, C4 and C5 to "owner/#425/save owner", C7 to "#424 + teaching/save owner". #425 is "AWAITING OWNER / NOT RUNNING" and #424 is the coordination manager. I did not find a rule saying the Manager may answer on the owner's behalf. The 15 Nov save signature was signed by the user-delegated Manager and says it is not the owner's own signature (`G:4`).
5. **Whether this work is still wanted tonight.** The #425 comment of 2026-11-16T00:14Z says the owner re-scoped overnight work to the standalone 2D multi-rack Sitka lab and that "Scenario One #424/#544 [are] NOT TOUCHED". I read that as "not changed by that notice". I was explicitly instructed to produce this card, so I did, but I am flagging it in case the instruction is stale.
6. **Real-world and runtime facts.** 3D path appearance, human walking and marking time, post-H1 and Year 2 G1 light, juvenile survival, truck-staging realism and any Unity behaviour are unmeasured (`P:61`, `K:80`). €-figures are game calibrations, not Irish prices (`P:75`). I added no ecological or forestry claim of my own.
