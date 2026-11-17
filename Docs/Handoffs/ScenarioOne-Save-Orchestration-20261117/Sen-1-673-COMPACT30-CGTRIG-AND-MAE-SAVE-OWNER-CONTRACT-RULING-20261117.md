# Sen-1 #673 — CG-TRIG and M-A save-owner contract ruling candidate

**CANDIDATE / UNSIGNED / NOT A SIGNED CONTRACT / NO IMPLEMENTATION AUTHORITY**

**Overall outcome: CONTRACT SIGNATURE BLOCKED.** No authorised save-policy signer executed a signature in this run.
This is decision-ready evidence for the real owner, not a signature, spec approval, implementation or ISO-58 acceptance.
The author is the S1 AUTHOR child, not the save owner. Proposed rulings below are all **PROPOSED — UNSIGNED**.

## 1. Identity, registration and custody

| Identity | Observed value |
| --- | --- |
| Actual author / provider | OpenAI GPT-6.1 Sol (`gpt-6.1-sol`), reasoning effort high |
| Harness / host | T3 Code / OpenCode on physical Linux; `hostname` returned `jer-laptop` |
| Registration UTC | `date -u` returned `Tue 17 Nov 2026 08:43:40 UTC` |
| Starting HEAD | `git rev-parse HEAD` returned `afd5484fc11697e6bde64f99d57d22e98f572f1f` |
| Context-source commit | `a5541b8a193883792db290539b7a3b877b589ff2`, observed latest context-source change |
| Branch | `docs/sen-1-673-save-contract-rulings-20261117-v1` |
| Sole working directory | `/media/jer/Files/CCF/Orchestration/CCF-673-Save-Ruling-20261117` |
| Sole writable file | This new Markdown file; no other repository file authorised |
| Initial status | `git status --short` empty; no unrelated changes observed |
| Parent / current Manager | DeepSeek orchestrator B, #673 / Overall Manager #671 |
| Execution claim | [S1 AUTHOR registration](https://github.com/jermur788/CCF/issues/673#issuecomment-6943166378), posted by parent before this file's first write |
| Authority held | Documentation authorship only; no save-policy signature, implementation, Unity or integration authority |

The target directory was created before the HEAD/UTC/hostname observations; no other worktree was written.
Live main coordination was fetched read-only through GitHub before writing: its leading #671 rows reserve this exact child output and require individual registration.
Those board rows still say prospective/not running; the live #673 parent and child receipts record actual execution, not a retrospective board edit by this author.
AgentWorkflow:255–288 requires pre-write custody and live coordination; :388–391 requires a GitHub-tracked handoff.
No physical Ops/Unity lease was requested or used. Canonical coordination/dashboard changes remain Manager-owned.

## 2. Evidence legend and exact source map

- **SOURCE-VERIFIED:** text/source read directly at the stated revision; static evidence only, never runtime proof.
- **MODEL-DERIVED:** reasoning from inspected facts; not native replay, transaction or causal verification.
- **GAME-CALIBRATION-PROPOSAL:** candidate interaction/integrity rule, not accepted gameplay or empirical ecology.
- **OWNER CHOICE (D-083/D-084):** accepted product intent, not source activation or schema permission.
- **UNSUPPORTED-NOT-IN-SOURCE:** the claimed durable fact/implementation is absent from the inspected records; no invented wire name fills the gap.
- **OWNER CHOICE (later receipt):** the direct-owner crop-tree receipt below refines intent only to its expressly stated extent.

Aliases below are exact paths, with line numbers counted from the original file using `nl -ba`.
Unless separately pinned, C# and canonical documentation citations mean starting HEAD above.

| Alias | Exact source |
| --- | --- |
| SAVE | `200add4e6e5116268b79ef63131ff9b836f14c5d:specs/scenario-independent-save-isolation-v1/spec.md` |
| SIG | `origin/docs/scen1-manager-save-v2-immutable-approval-20261115-v1:Docs/Handoffs/ScenarioOne-Save-Orchestration-20261115/scen1-MANAGER-SHARED-SAVE-V2-IMMUTABLE-SPEC-SIGNATURE-20261115.md` |
| E667 | `a94d3d9198161c5ee236a36f0c8d17bff688f4cd:Docs/Handoffs/ScenarioOne-Overnight-Gates-20261117/Sen-1-667-COMPACT30-CGTRIG-MAE-SAVE-EVIDENCE-DECISION-CANDIDATE-20261117.md` |
| R667 | `9f5712973651d36de39eefe12f1d4c5713510164:Docs/Handoffs/ScenarioOne-Overnight-Gates-20261117/Sen-1-667-INDEPENDENT-COMPACT-WORLD-AND-SAVE-GATE-DOSSIER-REVIEW-20261117.md` |
| P673 | `af3f32537f5b64666d00f367db6ebd252914741d:Docs/Handoffs/ScenarioOne-Manager-Relay-20261117/Sen-1-673-COMPACT30-SAVE-OWNER-CONTRACT-GATE-DECISION-PACKET-20261117.md` |
| O673 | `59280842ee9fb4cd7db1d6531e3ee1c2817654ee:Docs/Handoffs/ScenarioOne-Overnight-Gates-20261117/Sen-1-673-DEEPSEEK-COMPACT30-SAVE-CLOSURE-ORCHESTRATOR-PACKET-20261117.md` |
| DL / CM | `Docs/Project/decision-log.md` / `Docs/Project/current-milestone.md` |
| AW | `Docs/Project/AgentWorkflow.md` |
| M | `Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.cs` |
| WO / EV | `Assets/ForestPrototype/ScenarioOne/ScenarioOneWorkOrder.cs` / `Assets/ForestPrototype/ScenarioOne/ScenarioManagementEvent.cs` |
| SD / SC | `Assets/ForestPrototype/ForestSaveData.cs` / `Assets/ForestPrototype/ForestSaveController.cs` |
| AR / UI / WP | `Assets/ForestPrototype/UI/AnnualReviewView.cs` / `Assets/ForestPrototype/UI/ScenarioOneUiRoot.cs` / `Assets/ForestPrototype/UI/WorkPlanView.cs` |
| CR | `Assets/ForestPrototype/ScenarioOne/ScenarioOneCompletionRules.cs` |
| T / EC / SNAP | `Assets/ForestPrototype/ForestTree.cs` / `Assets/ForestPrototype/ForestEcologyController.cs` / `Assets/ForestPrototype/ScenarioOne/ScenarioEcologicalSnapshot.cs` |

P673 and O673 were read at the requested remote branch refs; their resolved full commits are recorded above.
P673:5 explicitly recognises signed §4.2 despite stale in-file banners; R667:85–90 independently checks the same immutable boundary.
SIG:3–16 was also read directly: the out-of-band signature binds SAVE's exact commit and supersedes stale banners for that immutable snapshot only.
R667:20–33 approves **evidence quality only**, not these new rulings; :163–174 explicitly excludes signature/spec/Unity authority.
E667:338–347 supplies the S01–S08 register. Its analysis is evidence input, not delegated signature power.

## 3. Signed boundary — restatement, not amendment

All rules in this section are **SOURCE-VERIFIED / SIGNED — IMMUTABLE** at SAVE, not claims about implemented Compact behaviour.
Exact quoted fragments below preserve the signed wording. No signed file is changed or reinterpreted.

| Boundary | Exact fragment / fixed meaning | Citation |
| --- | --- | --- |
| Root cardinality | “exactly `ccfSaveEnvelope`, `ccfWorldPayload`, `ccfCompactTeaching` keys, each once”; no fourth root; recursive duplicate-property rejection | SAVE:155 |
| Closed graph | “Unknown keys reject throughout the initial new envelope, evidence and version24 payload type graph”; historical unknown-field handling unchanged | SAVE:155 |
| Numeric/raw validation | “integers must fit signed32-bit unless the existing payload declares a wider type”; nested v24 raw/typed validation, selected-profile dispatch | SAVE:155 |
| Envelope | Exactly seven keys: `schemaVersion`, `chapterId`, `profileId`, `worldId`, `geometryKey`, `contentKey`, `payloadVersion` | SAVE:159 |
| Strict tuple | schemaVersion integer1, payloadVersion integer24, nested version explicitly integer24; nested scenario ID equals chapterId; exact case-sensitive registry strings, no trimming/defaults | SAVE:159 |
| Compact registry binding | `scenario-one` / `o1-compact-v1` / `scenario-one-compact-v1`; `compact30-v1` / `scen1-compact-h1-g1-v1`; reservation does not sign world content | SAVE:148 |
| Payload | “No schema bump or new fields in historical DTO”; existing v24 shape, geometry explicitly2 for Compact, neutral rack fields and no historical rack credit | SAVE:160 |
| Other profiles | Historical disk files never contain teaching key; O2 requires explicit null and unavailable binding remains gated | SAVE:161 /160 |
| Dependency | “CG-TRIG dependency HOLD — ISO-58 BLOCKED”; “Whole ISO-58 remains unchecked/BLOCKED, including otherwise testable persistence portions” | SAVE:163 |
| Evidence types | `evidenceVersion` integer1; `arrivalAcknowledged` and `assessmentAcknowledged` booleans; `groupOrderIds` array; `plantings` array; `reviewAcknowledgedYear` integer, −1 none | SAVE:165 |
| Group | Up to four unique positive integer order IDs, designated progressively while Pending, exact-position real references; not inferred from all plantings | SAVE:165 |
| Completion rows | Exactly `workOrderId` positive integer, `speciesId` exact `sessile-oak` or `beech`, `juvenileId` nonempty string, `completionYear` integer ≥0 | SAVE:165 |
| Cardinality/composition | “Four maximum, unique workOrderId/juvenileId”; final group exactly two sessile oak and two European beech | SAVE:165 |
| Lifecycle | Pending/Approved/Completed/Failed references reconcile with real order/event history; “cancellation is an event, not a new ScenarioWorkStatus value” | SAVE:165 |
| Credit | Rows added “only on successful normal resolution”; match retained real Completed exact `PlantJuvenile` order or authoritative retained completion history, stock/species/year/created juvenile | SAVE:165–167 |
| Later fate | “Juvenile existence is required at creation; later ordinary death does not invalidate execution credit” | SAVE:167 |
| Review | First actual covering review opened and acknowledged; `reviewAcknowledgedYear` “equals that report year (max completionYear), and the report is retained” | SAVE:167 |
| Incomplete/refusal | “A false acknowledgement or incomplete/cancelled/failed group simply remains incomplete; it does not invalidate otherwise legitimate work” | SAVE:167 |
| Invalid evidence | “A review acknowledgement without all required work/report evidence or any fabricated/mismatched link refuses” | SAVE:167 |
| No healing | “No counters/rows are silently repaired into credit” | SAVE:167 |

**Explicit prohibition:** do not add `juvenileId` to WO/EV, `workOrderId` to SD's juvenile DTO, an M-A record to historical v24, or any convenience timestamp/flag under the signed tuple.
SD:5–49 /53–109 /215–228, WO:29–72 and EV:40–70 show the existing shapes actually read.
S1 requires a separate authorised amendment and independent review; a “Compact-only” condition does not evade closed-graph validation.
New-profile version/presence rejection does not tighten genuine historical v1–v24 admission (SAVE:200–212).
Legacy40/Enlarged80, historical Rack V2/MFM, preference namespaces and frozen Reference remain protected (DL:149–151; SAVE:238).

## 4. One coherent disposition of S01–S08

Order of dependency: world/boundary → signed controls/admission → pairing and W-A → approved implementation → native qualification.
M-A storage is a parallel amendment proposal, not a workaround for missing scope authority.

| Group | Candidate decision or exact missing authority | Status | Decision owner | Impossible until relevant authority/gate closes |
| --- | --- | --- | --- | --- |
| S01 | Require independently signed authored world/registry binding and one shared profile-aware admission boundary; this author cannot supply either | BLOCKED | World/content owner + shared-save owner + #671 | Activate Compact or claim real-world/profile isolation |
| S02 | Adopt §§5–6 controls, B admission and explicit review-edge refusal rules as a linked CG-TRIG supplement; no signature occurred | PROPOSED — UNSIGNED | Authorised save-policy signer + teaching/content owner, routed by #671 | CG-TRIG acceptance or ISO-58 release |
| S03 | Capture direct one-to-one identity inside one successful normal resolution, preserve signed rows, prove error safety; no historical fields | PROPOSED — UNSIGNED | Save-policy signer + registered source owner/#671 | Implement pairing on this document's authority or claim atomic/idempotent runtime |
| S04 | Enforce Compact W-A in UI and services/batch setters; confirmed cancellation removes, new designation requires new quote/approval | PROPOSED — UNSIGNED | Work Plan owner/#671 under D-084; save signer for evidence semantics | Treat current batch demotion as compliant or approve a source writer |
| S05 | Prefer separately versioned S1 evidence candidate; S2 cannot meet current intent; exact placement/version/admission remain unsigned | PROPOSED — UNSIGNED | Save-policy signer + #671; owner for changed intent | Persist new S1 fields under current tuple or classify full M-A completion |
| S06 | Carry three selected/retained/favoured crop-tree intent; leave one-treated-stem sufficiency and precise competitor Fell minimum open | DEFERRED | Product owner via #671 | Definitive M-A-SCOPE classifier or numeric substitute |
| S07 | Require authorised exact-candidate native/reload/failure/isolation/historical witnesses; all matrix expectations below NOT RUN | BLOCKED | #671 + future registered verifier/physical custodian | Runtime qualification, release, personal acceptance or Windows build |
| S08 | Submit this new candidate to independent nonauthor review, then actual owner/Manager disposition and synchronization | BLOCKED | Independent reviewer + real signer + #671 | Self-approval, signature claim, main integration or administrative closure |

S08 does not discard R667's approved evidence quality: that verdict concerns the earlier dossier, not this new candidate.
No S01–S08 item is marked implemented, signed or verified. Review quality cannot substitute for signature authority.

## 5. CG-TRIG candidate supplement — all normative text UNSIGNED

These are **GAME-CALIBRATION-PROPOSAL** controls/read signals constrained by SAVE:163–167 and OWNER CHOICE DL:151.
They require separately accepted H1/G1/world content; this document invents no coordinates or collider contract.
Actor is the player in the selected, validated active Compact world; service guards must enforce the same preconditions as visible controls.

| Transition | Observable control and read signal | Preconditions and candidate persisted effect | Must refuse / never count |
| --- | --- | --- | --- |
| Genuine H1 arrival | At accepted H1 arrival interaction, show active forest identity and arrival explanation; player presses “Acknowledge arrival” | Authored H1 interaction validates actual player context; set only `arrivalAcknowledged=true` | Menu dismissal, spawn alone, proximity alone, load, harness/direct credit call, foreign profile |
| Current-world assessment | After arrival, open current stand assessment showing active profile/year and actual stand/species/stock/economy observations; press “I've assessed this stand” | Genuine assessment view for current world has rendered/readable content and explicit callback; set `assessmentAcknowledged=true` | Cached other-world summary, unopened view, automatic acknowledgement from work count |
| Group designation | Work Plan exposes “Designate teaching planting” per exact site/order, showing species, owned/reserved stock, actor, shelter and quote; visible group list | Both booleans true; valid Pending exact `PlantJuvenile`; distinct current-profile ID/site, owned stock, approved G1 site; append ID once, at most4, at most2 of each species | Approved/Completed retrofit, historical/foreign ID, fifth ID, repeated ID, third oak/beech, unowned stock, invalid site |
| Approval | Ordinary Work Plan review and Approve action, with each Pending order's choices visible | Normal affordability/stock/validation; genuine Pending→Approved event; freeze order choices | Automatic planting, credit from quote/approval, silent same-ID Approved editing |
| Success capture | Annual resolution visibly reports actual planting, stock/cash/time/protection and created individual | Successful designated order creates juvenile and one signed-shape row atomically with ordinary settlement/Completed; then genuine WorkResolved | Row before success, inferred pairing by coordinates/list tail/ID arithmetic, duplicate award |
| Covering review open | CT-3 reason offers “Open covering Annual Review”; show retained report year and the four real results | Define Y* = max actual completionYear of final four valid2+2 rows; open actual latest reportY* | Earlier-year browse, report generation alone, hidden callback or later report substituted |
| Covering review acknowledge | In that report's actual review footer press “I've read the annual results” | Latest reportY*, current ecological yearY*, all group resolutions precede acknowledgement; event9.year=Y*, event9.eventId greater than every group WorkResolved.eventId; set `reviewAcknowledgedYear=Y*` in same mutation | Open-only, pre-final-resolution event9, wrong year, direct API/harness credit, replay auto-ack |
| CT-3 hold | Disable advance with reason “Read the covering Annual Review before advancing” and working open-review action | Four valid final rows and no valid covering acknowledgement hold every annual advance/service route and any future skip route; save/reload preserves report/hold | Ecological tick, work settlement, synthetic/backdated review, silent bypass from load or another screen |

The displayed labels above are candidate UI copy, not new serialized keys or assertions that those buttons exist.
Read signal means genuine explicit UI interaction with the correct view/context, not proof that a person cognitively understood text.
EV:40–70 contains no UI-origin attestation field; loaded integers alone cannot cryptographically prove human reading.
Guarantee proposed is controlled mutation provenance plus retained validated chronology, with future negative/native witnesses.

### Chronology, transaction and idempotence candidate

1. Fresh: both booleans false, group/rows empty, review year−1; no credit.
2. Arrival → current-world assessment → progressive designation0…4 → ordinary approval → annual resolution; other genuine work may share batch.
3. Directly capture each created juvenile's identity from the successful creation, never reconstruct it later from all plantings.
4. Prevalidate species/site/stock/quote/cash/time/counters/unique linkage, stage or safely roll back effects, and prevent re-entrant Save observing a half-commit.
5. Juvenile, stock/cash/minutes/clearance/shelter, Completed/resolvedYear and its row must be one consistent success; append WorkResolved once afterwards.
6. Normal annual ecology then retains reportY*. Four-row truth (CT-1) activates CT-3 before any subsequent annual/skip advance.
7. Genuine latest-review acknowledgement records event9 and signed review year together; CT-2 is valid only after every final group resolution.
8. T1 is derived: arrival && assessment && CT-1 && CT-2. CT-3 is an enforcement hold, not another completion conjunct.
9. Reload restores admitted facts only; it is not arrival, assessment, planting, cancellation, replacement, review opening or acknowledgement.
10. Repeated valid acknowledgement/save/load must not add a row/event/resource award/year or silently alter false values.

M:105–164 creates genuine Pending exact orders with stock/site guards; :1221–1250 approves valid Pending batch work.
M:1280 /1313–1333 /1348–1368 constructs report year=current+1, resolves before WorkResolved, then annual ecology/report.
M:1861–1924 is `void`; stock deduction precedes inline juvenile construction and checked settlement.
Thus **current source is not proof of exception atomicity**, and no inspected current field captures order↔juvenile pairing (WO:29–72; SD:215–228; EV:40–70).
The proposed success boundary needs implementation review and failure injection; this documentation does not supply them.

### Review-edge rulings proposed for the real signer

- A claimed nonnegative review year without matching report, successful rows and post-resolution event9 refuses before mutation (signed SAVE:167).
- **UNSIGNED consistency rule:** Completed designated order without its required row refuses; no load-time reconstruction.
- **UNSIGNED lifecycle rule:** CT-1 true, review−1, but current year already beyondY* refuses as impossible CT-3 chronology; earlier browsing cannot repair it.
- **UNSIGNED atomicity rule:** matching post-resolution event9 atY* but review−1 is an inconsistent one-sided mutation and refuses, rather than auto-filling.
- **UNSIGNED chronology rule:** a nonnegative review claim supported only by a later-year or pre-resolution event refuses; no backdating.
- A historical acknowledgement unrelated to this group grants nothing. Signed report-year equality is never relaxed.
- These stricter producer-consistency edges are distinct from the false-arrival/assessment case below and need explicit signer approval.

## 6. Admission ruling candidate — choose B, not A

**Case (a):** otherwise valid nonempty real group with false arrival and/or assessment.
**Chosen proposal: B — admit as incomplete, no teaching credit.**
Reason: SAVE:167 expressly says a false acknowledgement “simply remains incomplete” and does not invalidate legitimate work.
Policy A would add a stricter exception to immutable text; this author neither selects nor silently introduces that exception.
E667:150–175 analyses both alternatives; this candidate selects B for owner decision, not as a signature.

**Candidate normative wording — UNSIGNED:**

> For the Compact profile only, admit an otherwise valid save whose nonempty groupOrderIds has a false arrivalAcknowledged or assessmentAcknowledged as incomplete. Preserve genuine work, completion rows and false acknowledgement values unchanged. Grant no teaching completion while either value is false, and perform no load-time repair or inferred acknowledgement. Permit missing acknowledgements only through genuine approved current-world controls; those later actions do not certify original historical ordering. Refuse new group binding until both acknowledgements are true. All strict identity, uniqueness, order/stock/species/year/juvenile, report and review-chronology checks remain mandatory. CT-3 still holds an unread covering review. Historical-profile admission is unchanged.

**Case (b):** fabricated completion row, duplicate ID, foreign reference, mismatched species/stock/year/juvenile, or review claim lacking report/chronology.
**Always reject before any world, scene, preference or file mutation**, not “admit without credit” and not discard/repair the offending evidence.
The signed refusal rule is SAVE:167; shared pre-mutation boundary is SAVE:188–194. B never relaxes either.
Admission includes complete validation before destination apply; Save validates consistent capture before IO (SAVE:190–196).

Compatibility consequence: B preserves truthful incomplete Compact work without changing historical v1–v24 or inventing migration.
No authenticated shipped-old Compact corpus is asserted. B does not prove earlier acknowledgement ordering when booleans later become true.
The real signer must approve the exact B wording, current-world remedial controls and review-edge rules together with CG-TRIG and its linked cases.
If a supported producer/version needs different admission, specify and separately approve a versioned migration; never rewrite signed bytes in place.
No acceptance signature is entered here; **CONTRACT SIGNATURE BLOCKED** remains the outcome.

## 7. Lifecycle and W-A candidate

Signed statuses remain Pending/Approved/Completed/Failed (WO:13–19); IsOpen includes Approved (WO:72).
Cancellation is documented event + removal, not a persistent Cancelled enum: M:1185–1193 /1210–1218 records OrderCancelled.
Pending/Approved have no completion rows; Failed/cancelled grant no credit; legitimate cancellation history prevents treating removal alone as corruption (SAVE:165).
Keep removed/failed reference incomplete until an explicit new player designation replaces it atomically in the group.
Replacement uses a new Pending order ID/site/stock reservation/quote, then its own approval and normal resolution; group never grows to5.
No automatic planting, credit transfer, resurrection, silent ID reuse or failure-history deletion; successful members' genuine rows remain intact.

**OWNER CHOICE DL:151 / proposed guards:** Approved type/site/species/stock/method/shelter/outcome/quote are immutable.
UI and service/batch routes must refuse editing Approved orders; cancellation requires explicit player confirmation, decline changes nothing.
Confirmed cancellation removes/audits/releases reservations; the replacement is a genuinely new order, not demotion/reapproval of the old ID.
Pending per-tree actor/shelter controls remain required, with ordinary optional shelter costs and no forced protection/free work.

**SOURCE-VERIFIED gap:** M:875–881 loops over IsOpen planting orders and changes execution method plus status to Pending at :879–880.
M:884–889 similarly changes shelter plus status to Pending at :887–888.
WP:450–462 exposes these controls using CanEditWorkPlan; M:333 does not exclude Approved members.
A UI-only disable is insufficient: Compact service/batch guards must preserve Approved members too; historical behaviour must not silently change.
M:1196–1207's rack-owned cancellation refusal remains historical policy, not a Compact rack requirement.
No new persistent status, synthetic Year event, free stock, H1 omission revenue or invisible grant is proposed.

## 8. Compact expected-behaviour matrix — EVERY ROW NOT RUN

All outcomes presuppose other required validators; “save/load yes” means candidate eligibility, not observed implementation.
Advance “ordinary” means only if all other genuine guards pass; it never promises execution of Pending work.
Provenance: **S** signed SAVE:155–167 /188–212; **C** unsigned §5 CG-TRIG/chronology; **B** unsigned §6 admission; **W** D-084 intent + §7 proposed guards.
No row is a test result. Each row is **NOT RUN**, including save, load, advance, failure and isolation expectations.

| Case — NOT RUN | Validate / explicit Save / Load expectation | Advance / credit expectation | Provenance |
| --- | --- | --- | --- |
| Fresh | Accept partial; save/load exact empty group/rows and false booleans | Ordinary; no T1 | S + C |
| Arrival only | Accept; retain true arrival/false assessment | Ordinary; assessment offered, no auto-credit | S + C |
| Assessed, no group | Accept; retain both true, no invented orders | Ordinary; designation offered | S + C |
| 1 Pending | Accept valid ID/site/stock; save/load same one | Ordinary; no planting row | S + C |
| 2 Pending | Accept distinct valid references; retain exact two | Ordinary; no planting row | S + C |
| 3 Pending | Accept distinct valid references; retain exact three | Ordinary; no planting row | S + C |
| 4 Pending,2+2 | Accept final designated composition; retain orders/reservations | Ordinary approval/resolution, no pre-execution credit | S + C |
| Partly Approved | Accept mixed statuses/history; save/load frozen Approved choices | Ordinary, only Approved resolves; no early award | S + C + W |
| Some Completed | Accept only matching genuine rows; persist unfinished members | Ordinary until final4; T1 false | S + C |
| Failed awaiting replacement | Accept incomplete with failure history/no failed row | Ordinary; offer explicit replacement | S + C |
| Cancelled with history | Accept removed order with genuine cancellation history | Ordinary; never resurrect/credit cancelled member | S + C |
| Replacement bound, incomplete | Accept new Pending ID plus retained old history; no transfer | Ordinary approval/resolution, T1 false | S + C + W |
| Four valid rows, review pending | Accept/save/load same rows/report/review−1 | CT-3 hold and visible open-review route | S + C |
| Review opened, not acknowledged | Accept unchanged review−1; opening alone adds no credit | CT-3 hold survives reload | S + C |
| Valid first covering acknowledgement | Accept matching reportY*, event9 after all group resolutions, fieldY* | T1 only if both booleans true; hold lifts | S + C |
| Earlier-year browse | Save/load unchanged genuine evidence; no new review credit | Covering-review hold unchanged | S + C |
| Mismatched chronology / one-sided review mutation | Refuse validation, Save capture or Load before mutation; no repair | No advance from refused candidate | S for false claim; C for extra consistency edges |
| Nonempty real group, false acknowledgement | B accepts otherwise valid work; preserve false values | No T1; CT-3 if four valid rows/review unread; missing genuine controls offered | S + B + C |
| Four linked but3+1 species | No valid final teaching group; proposed designation/strict final-group validation refuses; no species repair | Never T1 | S final2+2 + C refusal/control |
| Duplicate group/order-row/juvenile ID | Refuse before mutation; never drop duplicates | No credit/advance from candidate | S |
| Foreign/historical/fabricated IDs | Refuse; no cross-profile linking despite equal numeric IDs | No credit; prior world/files preserved | S |
| Later G1 juvenile death/promotion | Accept true execution with retained original identity/history; persist actual fate | Do not revoke genuine execution or revive individual | S death; C promotion continuity, M:1514–1521 /2045–2054 |
| Dual-profile / historical isolation | Selected profile only; foreign rejects; genuine historical versions keep signed admission, no added fields | No shared credit/economy/history or Reference preview for Compact | S:169–220; DL:149–151 |
| Power interruption / failed atomic commit | Qualification must show complete old/new or retained recoverable bytes at tested checkpoints; failed precommit keeps primary/backup, selected temp cleanup only | Never false Saved/Loaded; no extra world tick from recovery | S:196–198; no untested power-loss guarantee |

Future witnesses must include each Pending count, false-arrival/false-assessment/both, repeated reload/acknowledge, UI and service annual routes, and any future skip route.
Failure injection must separate in-memory settlement/row/event/report consistency from disk atomic replace and staged Load refusal.
Source now has a File.Copy-over-primary fallback (SC:77–86); this is not evidence of qualified safe writes.
Raw disk checks SC:261–274 and typed route SC:324–340 differ; no public typed/harness bypass may substitute for the signed boundary.
Current CaptureSaveData can initialize understorey (M:1394–1399); do not use it as a supposedly pure refusal sentinel.

## 9. M-A: S2 insufficiency and separate S1 amendment candidate

“S1/S2” here name evidence-design alternatives, not this author's orchestration stage or an approved schema.
OWNER CHOICE DL:151 demands substantive assessed treatment/retention, genuine work/review, then later inspected adaptive act-or-no-work and review.
Current records support some action/cost/year facts, not the full intent and local-consequence predicate (E667:220–282).

| Required proof | Inspected current fact | Why S2 existing-state-only cannot prove it |
| --- | --- | --- |
| Spatial focal declaration before approval | Mutable mark enum/setters T:16–23 /89–100; persisted mark SD:206; acted-on target WO:34 | A current CropTree mark or competitor target is not a dated explicit retained focal declaration |
| Explicit retention | EV:4–17 /40–70 and WO:29–72 lack a retention-choice record | Surviving neighbours do not prove player intent |
| Decision-time neighbourhood | EC:909–925 fixes8m Hegyi cutoff/term; EC:52 /928–937 uses live competition dictionary | Current positions/metrics cannot restore removed neighbours' decision-time membership and dimensions |
| Local before/after | SNAP:20–44 stores stand/species aggregates, not focal local metrics | Stand BA/light trends cannot prove same-focal local change |
| Genuine later adaptation | EV:4–17 has no explicit no-work decision event; action events describe work | An empty year is not an inspected deliberate no-work choice; an action alone does not prove reassessment |
| Complete episode provenance | SD:53–109 has orders/events/reports but no full Compact M-A episode | Cannot reconstruct declaration/retention/observations/choice by joining current state |

The8m cutoff is existing model calibration, not an empirical treatment prescription or owner-selected management threshold.
Do not repurpose manual prescription fields: WO:60–70 defines their prescription provenance, not a spare focal-record container.
S2 is **UNSUPPORTED-NOT-IN-SOURCE for the demanded complete evidence**, not permission to weaken intent.
M:1313–1339 /1377–1389 also shows grouped harvest settlement; never duplicate group cost once per stem when proposing auditable treatment records.

### S1 semantic record — CANDIDATE amendment only, every element UNSIGNED

No exact wire field name, numeric schema literal, enum, filename or sidecar is adopted by this table.
It refines E667:237–267's semantic proposal; it does not insert anything into signed §4.2.

| Candidate record content | Required semantic invariant / actual mutation point |
| --- | --- |
| Format + episode identity | Explicit allowlisted separately versioned Compact format; stable unique episode allocated once under signed allocation rule |
| Profile/world/content binding | Exact authorised active identities, immutable association, no cross-profile reuse |
| Crop/focal declaration | Expressly record the owner-intended three selected/retained/favoured crop-tree identities and identity kinds before treatment approval; not inferred from marks |
| Decision chronology/context | Actual year/order link, focal positions, pinned model/content interpretation; no fabricated declaration event |
| Retained neighbours / decision neighbourhood | Explicit retention choices plus auditable decision-time membership/positions/dimensions or reproducible pinned snapshot |
| Actual non-G1 treatment | Designated real orders, Approved→Completed/history, true settlement and costs/time with honest harvest grouping; qualifying scope still unbound |
| Local observations | Same focal, typed local before and post-annual observation with actual year/model/units; adult CI and juvenile light never interchangeable |
| Treatment covering review | Retained correct report and genuine later event9 after all linked work resolutions |
| Later adaptive decision | Actual later inspected reassessment; explicit act with real work links OR explicit no-work decision, no synthetic Completed order |
| Adaptive covering review | Actual choice-year report and genuine acknowledgement, after adaptive work if any |
| Integrity/reload | Unique identities, immutable captured facts, append once at genuine transitions; no load-time inference, repair or duplicate awards |

**Exact questions the authorised signer must answer before any implementation:**

1. Where may S1 live? Identify an explicitly revised container inside the three-root graph, or authorise a specific new format revision. No fourth root or extra field is valid under the old closed tuple.
2. Which exact keys/types/enums, required/optional states, and version allowlist are approved? Do not assume schema1/payload24 can carry it.
3. What binds episode/crop/juvenile identities and counters, including promotion/death, explicit replacement and content changes?
4. How is same-year declaration→approval→resolution→observation→review chronology witnessed without a fabricated duplicate history stream?
5. What are allowed state transitions, uniqueness, error atomicity, reload idempotence and reject-before-mutation rules?
6. How are current signed-format Compact saves admitted: remain evidence-incomplete, explicit versioned migration, or deliberate incompatible refusal? Name compatibility consequences.
7. What older/newer/unknown versions refuse, what conversion is explicit, and how is historical v1–v24 kept unchanged? No backfilled intent or no-work facts.
8. Does new evidence meaning require a new content/identity binding under SAVE:182's first-integrated-writer freeze rule? Pin the reviewed amendment and actual authority.

These questions are **S05 unsigned format decisions**; S1 does not unlock a classifier while S06 is deferred.

### Later owner receipt and M-A-SCOPE

Read directly: [#673 owner receipt](https://github.com/jermur788/CCF/issues/673#issuecomment-6942962743), created `2026-11-17T08:35:03Z`, headed “2026-11-17 08:34 UTC”.
It records **“yes 3 crop trees”**: three promising crop trees selected/retained/favoured, not three automatically felled, no cap on marking other eligible trees, no required treatment on each.
Approximately three competitor removals is labelled gameplay-calibration proposal, not a mandatory quota or ecological prescription.
**One-treated-stem sufficiency and exact competitor Fell minimum remain unresolved; M-A-SCOPE remains DEFERRED.**
This receipt does not authorise a new S1 field/classifier; canonical DL/CM reconciliation belongs to #671, not this child.
No one-tree threshold or numeric substitute is selected, and no question is reopened to the user here.

## 10. Review corrections, NOT RUN / NOT DONE, handoff

R667 findings carried forward, not misrepresented as new independent approval:

- **F-01:** UI:48 /459–465 and WP:167–169 already provide a first-review UI hold/open-review route. M:391–401 sets the latch; M:660 resets it at scenario reset; manager advance checks M:324–332 /1253–1280 lack CT-3. E667:96 records its one-shot reset limitation. This is not a re-armed all-route covering-year hold.
- **F-02:** world-field default classification/count correction is outside this one-file scope. Do not reuse old unsupported-row totals as established counts here; suggested defaults never upgrade missing authored evidence.
- **F-03:** acknowledgement duplicate suppression is CR:62–68 called at M:395, not the historical completion evaluation citation.
- **F-04:** disk/typed validation-flow citation is SC:261–274 /324–340, not narrow species/objective validators.
- **F-05:** all skip requirements here say **any future skip route**; no current multi-year skip API is asserted.

**Actually done:** registration/custody observations; read-only exact-ref and line-numbered source inspection; live issue/coordination reads; authorship and document/Git scope checks.
These are source/document checks, not independent review or runtime tests.
**NOT RUN:** compiler/import, unit/source tests, Unity/Editor/Player, archived harnesses, native UI/read signals, movement/interactions, annual ecology, harvest-once/resource accounting, regeneration/survival, save/load/replay, failure injection, backup/atomic commit/power interruption, historical/frozen Reference qualification, rendered/performance gates.
**NOT DONE:** save-owner signature, CG-TRIG approval, S1 amendment, M-A classifier choice, ISO-58 acceptance, source/profile activation, content signature, spec approval, physical Ops deployment/lease, main integration, owner playtest or Windows build.
Independent review of this candidate is **NOT DONE by this author**. Dashboard producer/health/render, published synchronization and Drive/context mirrors are **UNVERIFIED / NOT DONE** in this scope.
No C#, serialized Unity asset, .meta, spec, Docs/Project, coordination, save, preferences or other worker file changed; no PR opened or merged.

**Handoff:** DeepSeek B → independent nonauthor reviewer on the exact pushed commit → authorised save-policy signer and Overall Manager **#671**.
Reviewer should check immutable-boundary fidelity, B versus forged-evidence separation, review-edge consistency, W-A, all matrix provenance and S1/SCOPE separation.
Manager must obtain real authority/signature in a separately reserved normative action or retain **CONTRACT SIGNATURE BLOCKED**; later implementation still needs approved specs, ownership and qualification.
No signature block is executed or implied by publication. Three Compact spec families remain DRAFT; ISO-58 remains BLOCKED; no teaching completion is implemented or approved by this result.
