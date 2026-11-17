# Sen-1 #667 / #668 — CG-TRIG, D84-PJ and M-A save-evidence decision candidate

**CANDIDATE / UNSIGNED / NOT A SIGNED CONTRACT / NOT A SIGNED WORLD / NOT UNITY-VERIFIED / NO IMPLEMENTATION AUTHORITY.**
**PRE-IMPLEMENTATION / OWNER-AND-SAVE-OWNER-DECISION-READY. ISO-58 STAYS BLOCKED. M-A-SCOPE DEFERRED / UNDECIDED.**

| Identity | Value |
| --- | --- |
| Date / author | 2026-11-17 UTC; OpenAI GPT-6.1 Sol (`gpt-6.1-sol`), reasoning effort high, T3 Code/Codex |
| Claim / parent | [#668](https://github.com/jermur788/CCF/issues/668) / [#667](https://github.com/jermur788/CCF/issues/667) |
| Base, checked before writing | `1e0fba5ffd5a07704eed3e44c34b3a960f61ac60` |
| Branch | `docs/sen-1-667-compact-world-save-evidence-20261117-v1` |
| Own path | `Docs/Handoffs/ScenarioOne-Overnight-Gates-20261117/Sen-1-667-COMPACT30-CGTRIG-MAE-SAVE-EVIDENCE-DECISION-CANDIDATE-20261117.md` |
| Context-source commit | `a5541b8a193883792db290539b7a3b877b589ff2` |
| Static C# pin | Dispatched base; empty `git diff 500ef7acecd39724b8ebd4cec1ebdfc00be45867 HEAD -- Assets` before authoring |
| Companion | [Sen-1-667-COMPACT30-COMPLETE-WORLD-CONTENT-PROVENANCE-AND-SIGNATURE-READY-CANDIDATE-20261117.md](Sen-1-667-COMPACT30-COMPLETE-WORLD-CONTENT-PROVENANCE-AND-SIGNATURE-READY-CANDIDATE-20261117.md), including complete computed raw-source inventory, UTC reads, roster derivation and world field/gap matrix |

This document records proposed decisions for the correct authorities. It makes none of them. Both documents are source evidence authoring, not implementation, independent review or acceptance. The latest direct user dispatch authorises the prepared checkout and the two new files only. [Actual pre-edit receipt](https://github.com/jermur788/CCF/issues/668#issuecomment-6934662071); live coordination blob read `c0edc881eef83fb4f9cbc9386a3451ddc3ce6e1d`. No physical Ops/Unity authority inferred.

## Evidence labels, source references and actual reads

- **SOURCE-VERIFIED:** actual bytes/text/C# read using connected GitHub fetch, `git show`, `nl -ba`, `rg`, and read-only Python byte reads. It establishes source facts, not native guarantees.
- **MODEL-DERIVED:** arithmetic or logical consequence of verified source; runtime parity unverified.
- **GAME-CALIBRATION-PROPOSAL:** an unsigned rule/default offered for review; no selection implied.
- **OWNER CHOICE (D-083/D-084):** accepted product intent. Latest explicit deferral of M-A-SCOPE takes precedence over older recommendations.
- **UNSUPPORTED / NOT IN SOURCE:** a demanded durable fact or implementation is absent in the inspected scope. No new wire-field name, version or quota is invented to disguise it.

Every alias below means the exact **repository path:line** at the base unless separately pinned. The companion's raw inventory authenticates CB/L/harness bytes; those inputs establish world provenance, not existing teaching/save behaviour.

| Alias | Exact path / immutable reference |
| --- | --- |
| SAVE | `200add4e6e5116268b79ef63131ff9b836f14c5d:specs/scenario-independent-save-isolation-v1/spec.md` |
| SIG | `Docs/Handoffs/ScenarioOne-Save-Orchestration-20261115/scen1-MANAGER-SHARED-SAVE-V2-IMMUTABLE-SPEC-SIGNATURE-20261115.md` on `docs/scen1-manager-save-v2-immutable-approval-20261115-v1` |
| P / C / G | `specs/scenario-one-compact-complete-loop-v1/spec.md` / `specs/scenario-one-compact-initial-planting-v1/spec.md` / `specs/scenario-one-compact-graphics-v1/spec.md` |
| M | `Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.cs` |
| WO / EV / PJ | `Assets/ForestPrototype/ScenarioOne/ScenarioOneWorkOrder.cs` / `Assets/ForestPrototype/ScenarioOne/ScenarioManagementEvent.cs` / `Assets/ForestPrototype/ScenarioOne/PlantedJuvenile.cs` |
| SD / SC / VAL / VG | `Assets/ForestPrototype/ForestSaveData.cs` / `Assets/ForestPrototype/ForestSaveController.cs` / `Assets/ForestPrototype/ForestSaveValidation.cs` / `Assets/ForestPrototype/ForestSaveValidation.Geometry.cs` |
| CR / AR / UI | `Assets/ForestPrototype/ScenarioOne/ScenarioOneCompletionRules.cs` / `Assets/ForestPrototype/UI/AnnualReviewView.cs` / `Assets/ForestPrototype/UI/ScenarioOneUiRoot.cs` |
| EC / T / SNAP | `Assets/ForestPrototype/ForestEcologyController.cs` / `Assets/ForestPrototype/ForestTree.cs` / `Assets/ForestPrototype/ScenarioOne/ScenarioEcologicalSnapshot.cs` |

Authoritative product rows: `Docs/Project/decision-log.md:139` D-079, `:143` D-081, `:146` D-082 (separate lab), `:149` D-083, `:151` D-084. The parent P:14–20 records #663's corrections; P:33–122 defines proposed T1/PJ; P:135–184 defines proposed M-A evidence. These and the three DRAFT families' plans/tasks/checklists were inspected, preserving every status. Read-only prior evidence: #657 annex/manifest, #660 independent findings §§2–4, #663 result §§B-1/M-1/M-2/B-2 and open items, #665 changed-lines quality verdict, and #652 Manager's deployment HOLD. This is a new dossier, not a rerun or self-review of those outputs.

## 1. Signed boundary — immutable text, not present implementation

The [out-of-band signature](https://github.com/jermur788/CCF/blob/docs/scen1-manager-save-v2-immutable-approval-20261115-v1/Docs/Handoffs/ScenarioOne-Save-Orchestration-20261115/scen1-MANAGER-SHARED-SAVE-V2-IMMUTABLE-SPEC-SIGNATURE-20261115.md) (SIG:3–16) binds exact commit `200add4e6e5116268b79ef63131ff9b836f14c5d`, whose spec blob is `8ac551022adb578abed0d9fbd413c8e9d002c3f9` (SIG:22). Its stale in-file pre-signature banners are not permission to rewrite it. **All rows below are SIGNED — IMMUTABLE**, including holds. Quoted fragments preserve exact signed keys/rules; surrounding explanation does not enlarge their scope.

| Signed key / rule | Quoted signed constraint and exact scope | Reference / status |
| --- | --- | --- |
| Root | “exactly `ccfSaveEnvelope`, `ccfWorldPayload`, `ccfCompactTeaching` keys, each once”; recursive duplicate rejection | SAVE:155; SIGNED — IMMUTABLE |
| Root overlap | “No root key overlaps any historical ForestSaveData root field”, including version/trees/scenarioOne/standGeometryModel | SAVE:155; SIGNED — IMMUTABLE |
| Closed type graph | “Unknown keys reject throughout the initial new envelope, evidence and version24 payload type graph” | SAVE:155; SIGNED — IMMUTABLE |
| Integer bounds / nested validation | “integers must fit signed32-bit unless the existing payload declares a wider type”; all current raw/typed v24 checks on nested payload, selected-profile dispatch, never root-payload validation of envelope root; historical unknown-field handling unchanged | SAVE:155; SIGNED — IMMUTABLE |
| `ccfSaveEnvelope.schemaVersion` | “schemaVersion=1”; integer, not missing/defaulted | SAVE:159; SIGNED — IMMUTABLE |
| `chapterId` | string, “strings exactly registry values, case-sensitive, no trimming/defaults”; Compact `scenario-one` | SAVE:148 /159; SIGNED — IMMUTABLE |
| `profileId` | same strict rule; Compact `o1-compact-v1` | SAVE:148 /159; SIGNED — IMMUTABLE |
| `worldId` | same strict rule; Compact `scenario-one-compact-v1` | SAVE:148 /159; SIGNED — IMMUTABLE |
| `geometryKey` | same strict rule; Compact `compact30-v1` reservation, not approved terrain | SAVE:148 /159; SIGNED — IMMUTABLE |
| `contentKey` | same strict rule; Compact `scen1-compact-h1-g1-v1` reservation, not roster digest | SAVE:148 /159; SIGNED — IMMUTABLE |
| `payloadVersion` | “payloadVersion=24”; nested payload version explicitly integer24 and nested scenarioId explicitly equals chapterId; exact seven envelope keys | SAVE:159; SIGNED — IMMUTABLE |
| Envelope refusal | “Missing/null/empty/whitespace/unknown/mismatch rejects” | SAVE:159; SIGNED — IMMUTABLE |
| `ccfWorldPayload` | “Existing version24 ForestSaveData shape”; “No schema bump or new fields in historical DTO”; current required raw/typed fields apply | SAVE:160; SIGNED — IMMUTABLE |
| Neutral rack fields | “rackPlanningModel=0, rackPlanConfirmed=false, nextRackId=1, racks=[], no rack-owned orders, firstGuidedRackReviewComplete=false”; no internal extraction rack/historical teaching credit | SAVE:160; SIGNED — IMMUTABLE |
| Geometry / versions | “Compact geometry explicitly2”; O2 unavailable without future approved binding; only initial tuple(schema1,payload24), shared payload lineage, “No independent per-profile counter” | SAVE:160; SIGNED — IMMUTABLE |
| `ccfCompactTeaching` container | “Compact: object below; O2: explicit null”; historical disk never contains this key; O2 null gives no Compact credit | SAVE:161; SIGNED — IMMUTABLE |
| CG-TRIG / ISO-58 | “not-yet-supplied compact world/teaching interaction contract”; exact controls/preconditions/read signal must be separately signed with positive/negative/midway reload cases; “Whole ISO-58 remains unchecked/BLOCKED” | SAVE:163; SIGNED — IMMUTABLE |
| `evidenceVersion` | “integer1” | SAVE:165; SIGNED — IMMUTABLE |
| `arrivalAcknowledged` | “boolean”; genuine trigger still CG-TRIG-gated | SAVE:165 /167; SIGNED — IMMUTABLE |
| `assessmentAcknowledged` | “boolean”; not inferred from map/menu opening | SAVE:165 /163; SIGNED — IMMUTABLE |
| `groupOrderIds` | “up to four unique positive integer workOrderIds”; player-designated Pending group progressively zero–four, genuine exact-position references | SAVE:165; SIGNED — IMMUTABLE |
| Group designation / composition | IDs bound through real Work Plan interaction, never counting arbitrary plantings; “Final group is exactly two sessile oak and two European beech” | SAVE:165; SIGNED — IMMUTABLE |
| `plantings` | array; “Four maximum, unique workOrderId/juvenileId”; initially empty gives no credit; rows reference group and arise only on successful normal resolution | SAVE:165; SIGNED — IMMUTABLE |
| Row `workOrderId` | “positive integer, matching the existing order type” | SAVE:165; SIGNED — IMMUTABLE |
| Row `speciesId` | “exact `sessile-oak` or `beech`” | SAVE:165; SIGNED — IMMUTABLE |
| Row `juvenileId` | “nonempty string” | SAVE:165; SIGNED — IMMUTABLE |
| Row `completionYear` | “integer ≥0”; exactly these four row keys | SAVE:165; SIGNED — IMMUTABLE |
| Midway/cancellation | Pending/Approved/Completed/Failed refs reconcile with real history; “cancellation is an event, not a new ScenarioWorkStatus value”; documented removal remains incomplete/saveable, not corrupt; no order/history evidence rejects | SAVE:165; SIGNED — IMMUTABLE |
| Replacement | “Optional replacement updates the group only through real player designation, not fabricated completion”; failed history gives no credit/automatic planting | SAVE:165; SIGNED — IMMUTABLE |
| Row reconciliation | Retained real Completed exact `PlantJuvenile` order **or authoritative retained completion history**, matching stock/species/year/created juvenile; “No second fabricated event stream” | SAVE:167; SIGNED — IMMUTABLE |
| Later juvenile fate | “Juvenile existence is required at creation; later ordinary death does not invalidate execution credit” | SAVE:167; SIGNED — IMMUTABLE |
| `reviewAcknowledgedYear` | integer, “−1 means none”; only on first actual opened/acknowledged review covering all four; equals report year=max completionYear; report retained | SAVE:165 /167; SIGNED — IMMUTABLE |
| Teaching completion | Derived only from true arrival+assessment, four valid2+2 completion rows and valid covering review; existing acknowledgement's proof/read signal must be specified in signed CG-TRIG | SAVE:167; SIGNED — IMMUTABLE |
| Incomplete vs refusal | “A false acknowledgement or incomplete/cancelled/failed group simply remains incomplete; it does not invalidate otherwise legitimate work”; missing work/report evidence for a review claim or fabricated/mismatched link refuses | SAVE:167; SIGNED — IMMUTABLE |
| No silent repair | “No counters/rows are silently repaired into credit”; “not an already-authored compact implementation” | SAVE:167; SIGNED — IMMUTABLE |

SAVE §4.1:148–151 reserves the three disjoint slots/own sidecars; it does not activate Compact. SAVE:188–210 FR-01…FR-07 requires one validated pre-mutation boundary, pure capture, staged apply/preview return, explicit atomic save/recovery and preserved historical admission. These transaction requirements remain mandatory alongside teaching semantics; an honest boolean classifier cannot replace them.

### Separate CG-TRIG supply table — proposed and currently absent

| Needed interaction / proof | Current source read | Proposed supply; no new signed key |
| --- | --- | --- |
| Compact profile/current world provenance | SC:9–19 uses historical filename; SD:5–49 has no envelope; no teaching-key matches in `rg` over `Assets/ForestPrototype/**/*.cs` at base | Shared save owner binds actual authored world/strict profile context; no borrowing unmerged #544 code assumptions |
| Arrival control | No `arrivalAcknowledged` field/control at base | Explicit H1 acknowledgement, persisted once to signed boolean; no proximity/auto grant (P:65–68) |
| Current-stand assessment | No Compact assessment state/control at base | After arrival, display current profile/year's real stand assessment then explicit confirmation (P:65–68) |
| Group designation/cancel/replace | M:105–174 /1185–1219 supplies ordinary orders and cancellation, no G | Only bind real Pending G1 order after both acknowledgements; cancelled/Failed ref remains until explicit atomic replacement (P:56 /69) |
| Pairing | WO:29–72 /SD:215–228 /EV:40–70 lack reciprocal ID link | D84-PJ success capture to signed row, not post-hoc inference (P:84–122) |
| Latest covering review UI provenance | AR:65–66 guards earlier browse; UI:438–444 requires review screen/report; M:391–401 records CurrentYear, not report identity | Compact-gated latest Y* callback, event9 and reviewAcknowledgedYear in same mutation (P:57–64) |
| Annual advance / any future skip-route hold | Manager API has no unread-review gate (M:324–333 /1253–1280 /2377–2384). Existing UI-only, first-review-only hold uses NeedsAnnualReview / ShowReview+return (UI:48 /462) and disables advance with a review action (`Assets/ForestPrototype/UI/WorkPlanView.cs:167–169`); M:394 sets annualReviewSeen=true, M:660 is its only reset to false. This one-shot hold is never re-armed after acknowledgement, is not CT-3 and cannot substitute for it | CT-3 applies to both service/API and UI annual advance and any future skip route, with visible reason and action to covering review |
| Admission of impossible-control state | No Compact admission in base SC/VAL | Save-owner rules on both alternatives in §3; no worker-selected stricter rejection |
| M-A evidence | Current mutable marks/orders/events/aggregate snapshots insufficient (§5) | S1 separately signed versioned record or owner-redefined intent; not added to signed teaching object |

**Storage boundary:** “separate from `ccfCompactTeaching`” does **not** permit a fourth root key or an extra sidecar under the existing contract. S1 needs a separately reviewed/signed format revision covering container placement, exact key names, version allowlist and any path policy. Old signed bytes are left immutable; no new field/file/schema tuple is chosen here.

## 2. Proposed state machine and truth table

This section is **GAME-CALIBRATION-PROPOSAL**, read from corrected P:33–122 and constrained by SAVE:165–167. It is not existing runtime behaviour.

State transitions:

1. Fresh Compact: arrival=false, assessment=false, G=[], plantings=[], reviewAcknowledgedYear=−1; no teaching credit. Explicit arrival → arrival=true. Assessment offered only thereafter; confirm current-world summary → assessment=true.
2. Explicit per-site designation binds **0…4** genuine Pending exact planting IDs. Refuse binding while either acknowledgement false. Each order separately follows Pending → Approved → Completed **or** Failed in an ordinary annual resolution. Removal of Pending/Approved records `OrderCancelled` and removes it; **Cancelled is a conceptual state/history outcome, not a new enum value** (WO:13–19; M:1185–1219).
3. Successful designated exact planting appends its one-to-one order↔juvenile row during that resolution (§4). Pending/Approved/Failed/cancelled refs give no execution credit. Explicit replacement swaps that failed/cancelled reference for a new player-designated Pending ID in one mutation; never G size5, no automatic sapling or history deletion.
4. **CT-1:** exactly four distinct current-profile Completed exact `PlantJuvenile` references,2 sessile-oak/2 beech, four unique directly captured juvenile identities and validated rows. Y*=max real completionYear of these final four; not designation/approval year.
5. **CT-2:** actual latest covering report year==Y*, active Compact context, real UI acknowledgement emits event9 whose eventId exceeds **every** group's WorkResolved eventId; same mutation writes reviewAcknowledgedYear=Y*. Latest report year, ecological CurrentYear and monotonic eventId are distinct facts. No event ID or assessment-year key is added.
6. **CT-3:** CT-1 && !CT-2 ⇒ hold any annual advance and any future skip route; visible reason and “open covering review” route. Save/reload restores hold and same report. No silent skip, auto-ack, backdate or relaxed year equality. CT-3 is **not** a completion conjunct.
7. **T1-COMPLETE = arrivalAcknowledged && assessmentAcknowledged && CT-1 && CT-2.** It is derived, not persisted as another flag. G1 death/promotion after successful execution does not erase genuine work. The original juvenile identity stays locatable via retained record/promotedTreeId (M:1514–1521 /2045–2054; SD:215–228).

Definitions for the table: A=arrival, B=assessment, C=CT-1, R=CT-2; “admit” always presupposes **all other** identity/schema/history checks. Rows describe future expected outcomes, **NOT RUN**. A review claim with mismatched year/evidence is refused per signed rule; simply lacking a valid review gives no credit.

| Case / persisted state | Admission / derived result proposed | Advance / visible result | Reload witness — NOT RUN |
| --- | --- | --- | --- |
| A0 B0; G0, no rows, review−1 | Legitimate initial partial; T1 false | Arrival prompt, no completion | Exact initial partial restored |
| A1 B0; G0 | Legitimate arrival-only; T1 false | Assessment offered | No automatic assessment on reload |
| A1 B1; G0 | Legitimate assessed partial; T1 false | Owned-stock site designation offered | No orders/rows invented |
| A1 B1; G size1,2,3,4 Pending (each count separately) | Legitimate progressive designation; T1 false | Work Plan quote/approval, not planting credit | Same IDs/species/sites/reservations at each count |
| Some/all group members Approved, fewer than4 Completed | Incomplete, no pairing rows for unfinished orders | Only ordinary permitted annual work; other blockers preserved | Approval frozen, no early juvenile/stock/cash settlement |
| Some Completed, others Pending/Approved | Retain genuine rows only, C false | Ordinary advance allowed when other guards permit; no “T1 passed” | Prior real execution retained exactly |
| Failed member awaiting replacement | C false; failure history retained, no failed planting row | Explicit new designation, not automatic planting | Same incomplete group/failure |
| Cancelled member absent with OrderCancelled history | C false; not corruption solely due cancellation | Player may explicitly replace exact ref with new Pending ID/quote | No cancelled ID resurrection |
| New replacement bound but incomplete | C false; old failure/cancel history retained; G≤4 | Normal approval/resolution only | New ID retained; no transferred credit |
| Four valid Completed2+2 rows; A1 B1; review−1 | C true, R false; T1 false | CT-3 hold; visible reason and open Y* review action | Same hold/report restored before any advance |
| Covering review merely opened, not acknowledged | R false | Hold remains; acknowledgement is explicit | No fake event9 or boolean on load |
| Event9/yearY* recorded before last group WorkResolved | Does not prove CT-2; a claimed reviewAcknowledgedYear without required chronology refuses | No completion/backdating | Refusal preserves world/files; no event repair |
| Latest Y* review acknowledged much later in wall-clock time, no intervening advance | Valid if A/B/C and UI/chronology all hold | R true, T1 true; hold lifts | Idempotent same evidence after repeated load/click |
| Four valid rows; correct post-resolution event9/yearY*, but reviewAcknowledgedYear=−1 | One-sided evidence violates proposed same-mutation contract; cannot claim R | No credit; save-owner must specify handling, no load-time repair | Witness must show such partial mutation never emitted |
| reviewAcknowledgedYear=Y* but no matching report/event9/chronology | Signed review-evidence refusal | No completion; no mutation | No synthesised event/report |
| Event9/yearY*+later, no genuine Y* acknowledgement | Later-year event never satisfies R; review claim for wrong year refuses | No retrospective credit/earlier-browse workaround | Same refusal/incomplete policy as applicable, no backdate |
| Earlier-year review browse | AR:65–66 callback guarded; proposed Compact records no R | Hold unchanged if covering review unread | History browsing remains read-only |
| Harness/direct non-UI acknowledgement, load or skip attempts credit | Cannot satisfy proposed genuine UI-origin requirement | No completion; no auto-ack | Replays never manufacture UI provenance |
| Valid event9/yearY* and matching field, but A or B false with nonemptyG | T1 false under BOTH §3 policies; policyA refuses, policyB admits incomplete | Never show passed; underB expose missing real acknowledgement | No repair from work count; apply chosen policy identically |
| Four linked Completed but3 oak/1 beech or invalid site | C false; invalid site/links refuse where signed binding requires | No completion; no forced species conversion | Exact refusal, no recount into credit |
| Duplicate group ID, row order ID or juvenile ID | Refuse, not incomplete-with-credit | No mutation | No dropping duplicate to salvage credit |
| Foreign/historical/fabricated IDs, species/stock/year/juvenile mismatch | Refuse before mutation | Existing world and disk unchanged | No nearest-position linking |
| Multi-year genuine completions/replacement, Y*=latest actual year | Earlier valid rows remain; Y* recomputed from final4 | Final year's review required; earlier review insufficient | Same maximum and chronology after reload |
| Later ordinary death/promotion after genuine T1 | Valid execution history retained; do not require all juveniles still alive | Honest losses/promotion inspectable | No row deletion or revived plant |
| CT-3 state, direct AdvanceYear or any future N-year skip route attempted | Refuse every path, including API; no ecological tick | Visible reason + action to Y* latest review | Reload remains held |

A loaded state with C true, R false and CurrentYear already pastY* cannot be repaired through earlier-year browsing without violating the latest-review rule. **Additional CG-TRIG admission/lifecycle edge for save owner (S02):** specify refusal of impossible chronology or a strictly incomplete presentation without synthetic credit. This document does not invent recovery/backdating or a fourth key to solve it. Similarly, a one-sided event9/field mutation needs a contract ruling and failure injection, not retrospective healing. Those cases are separate from the explicitly requested false-A/B admission choice.

UI-origin limitation: EV:40–70 has no “UI source” field. A valid loaded event's integer year/ID alone cannot cryptographically prove a real person read the screen. The proposed guarantee is **controlled mutation provenance plus validated retained chronology**, not tamper-proof authorship. No hidden event stream, timestamp or extra signed key is smuggled in; genuine UI path and negative harness cases must be witnessed later.

## 3. Save-owner admission ruling — BOTH alternatives, no author decision

Target state: selected Compact envelope is otherwise structurally/semantically valid; G is nonempty with valid history, but arrivalAcknowledged and/or assessmentAcknowledged is false. Proposed controls cannot produce it (P:65–68). Signed SAVE:167 says a false acknowledgement simply remains incomplete and does not invalidate otherwise legitimate work. No Compact released-old-save lineage is asserted.

| Aspect | A — refuse at admission | B — admit incomplete, no teaching credit |
| --- | --- | --- |
| Precise condition | If G.Count>0 and (!arrivalAcknowledged OR !assessmentAcknowledged), reject **before all mutation**, even when order/history links otherwise reconcile | Admit only if all other strict checks pass; T1 false while either acknowledgement false; preserve valid work/history exactly |
| Threat model | Fail closed on impossible-control chronology or manipulated/transitional producer; avoids normalising bypassed onboarding | Avoid destructive over-rejection where producer cannot prove chronology because timestamps are absent; deny fabricated links separately; preserve real work |
| Back-compat / old saves | No change to historical files; could reject an otherwise truthful Compact file from a future/transitional producer. There is no authenticated pre-existing Compact corpus here to waive that consequence | Historical admission unchanged; more tolerant of partial Compact evidence without awarding credit; not a migration of old O1 |
| Migration / repair | No automatic repair, clearingG or auto-setting booleans. If a real supported producer later exists, save owner must separately define a versioned explicit migration or leave refusal | No repair/load-time backfill. Preserve false values and valid rows. Player can perform missing genuine acknowledgements in the active world if approved controls support it; this is forward progress, not evidence of past ordering |
| Observable player experience | “Save not loaded: teaching group predates required acknowledgements”; current world retained. Explain support/decision path without overwriting file | Forest loads with real work retained; clear missing-arrival/assessment prompt and no “teaching complete”. CT-3 still holds when C true and R false; lifting it does not itself satisfy A/B |
| Security limit | Cannot prove true past ordering merely because booleans are true; the save has no acknowledgement timestamps. Strictness is consistency policy, not cryptographic proof | Does not certify the impossible original ordering; it explicitly withholds credit until predicate true, and cannot accept fabricated/mismatched work |
| Consistency with signed text | Adds a stricter cross-field exception to “a false acknowledgement … simply remains incomplete”; needs explicit separately reviewed save-owner contract revision/attestation, not author inference | Closest reading of SAVE:167, also P:68's existing author recommendation. Still needs a save-owner CG-TRIG ruling and native tests; not authorised by this dossier |
| What neither option changes | Does not authorise a new field, relaxed identity/duplicate validation, historical rejection, fabricated completion or load writeback | Same; no player work lost or invented to make admission look consistent |

**Recommendation to the save owner, not a decision:** choose **B**, because it preserves signed §4.2's explicit treatment of false acknowledgements and truthful work. The proposed forward controls prevent creating this state; the predicate still denies credit. A does not establish historical ordering when booleans are true, but does add a rejection not stated in the signed text. If the owner deliberately prefers strict impossible-state refusal, A is coherent only with an explicit revised binding and compatibility consequence. Neither choice decides M-A-SCOPE.

The save-owner ruling needs an exact immutable reference to one of these candidate additions, reviewed with the signed boundary. Proposed text for **A**:

> For the Compact profile only, admission rejects a nonempty groupOrderIds array when either arrivalAcknowledged or assessmentAcknowledged is false, before any world, scene, preference or file mutation, even when its work references are otherwise legitimate. This is a deliberate stricter exception to §4.2's incomplete-state rule. No automatic migration, acknowledgement, group clearing or repair is permitted. Historical profiles are unaffected. Any supported transitional Compact producer requires a separately specified versioned migration before use.

Proposed text for **B**:

> For the Compact profile only, an otherwise valid save with nonempty groupOrderIds and a false arrivalAcknowledged or assessmentAcknowledged is admitted as incomplete. Genuine work and both false values remain unchanged; no teaching credit or load-time repair is awarded. The player may perform missing explicit current-world acknowledgements through the approved controls; work never supplies acknowledgement automatically. New group binding remains refused until both acknowledgements are true. All other identity, uniqueness, work/juvenile/report and review-chronology checks remain mandatory. CT-3 still governs unread covering reviews. Historical profiles are unaffected.

These are **UNSIGNED candidate ruling texts**. Any actual change to signed policy must be a separately signed revision; do not edit SAVE in place. Save owner must also rule on the impossible late-chronology and one-sided review mutation cases identified above.

## 4. Planting transaction, atomicity and W-A preservation

### Current source facts — static only

| Fact actually read | File:line (aliases above) | Limit |
| --- | --- | --- |
| Exact designation validates species/stock reservation, duplicate position, valid cell and ≥0.7m living-stem distance; creates Pending ID and OrderCreated event | M:105–174 | No Compact G/site/path or acknowledgement gate |
| Status enum is Pending/Approved/Completed/Failed; IsOpen includes Approved | WO:13–19 /72 | Cancellation is event/removal, not enum addition |
| ApprovePendingWork orders valid Pending IDs ascending, checks total affordability, approves all valid Pending work | M:1221–1250 | Batch may include unrelated work; G must track exact4 IDs |
| AdvanceYear collects Approved ascending, report.year=ecologicalYear+1, resolves each before WorkResolved, then annual ecology/report | M:1264–1368 | No Compact review hold; the UI-only first-review hold is one-shot, not CT-3 (see §1); no proof of exception rollback |
| ResolvePlanting validates offer/species/cell/stock/quote before spending; nonexact legacy branch uses TryPlantJuvenile | M:1861–1888; `Assets/ForestPrototype/ForestPlanting.cs:27–49` | Legacy result has no juvenile-ID return; not the Compact exact branch |
| Exact branch deducts stock, applies planting clearance, directly constructs PJ+counter juvenile, optionally creates shelter, settles cash/minutes and sets Completed/resolvedYear | M:1890–1924 | Current method returns void. A direct capture/return of its exact new juvenile identity is proposed, not existing |
| WorkResolved has workOrderId, species/position/stockUsed/cost, no juvenileId | M:1313–1333 /2452–2480; EV:40–70 | Event alone cannot pair the individual |
| Order has no juvenileId; juvenile save record has no workOrderId; capture simply copies separate lists | WO:29–72; SD:215–228; M:1417–1423 | **No current code path captures/persists the order↔juvenile pairing field** |
| nextJuvenileId re-derived as max parsed PJ+1; event counter max(data counter,recorded max+1) | M:1529–1534 | Deterministic replay reasoning depends on same retained records/ordering and future transaction implementation |
| Juveniles remain in retained records, with death and promotedTreeId updates | M:1514–1521 /1997–2054 | Execution credit survives true later loss/promotion, not mismatched identity |
| Latest-review button is guarded for earlier browse; root requires Review screen/report; manager emits CurrentYear event9 | AR:65–66 /108; UI:438–444; M:391–401 | Manager API itself has no UI provenance proof; annualReviewSeen is not CT-2 |
| Historical completion uses HasReviewedTwoCycleSequence; ack helper suppresses duplicate post-resolution event9/year | Historical: CR:36–48; M:2394–2400. Helper: CR:62–68; M:395 | Does not implement Compact T1 or M-A and must stay historical |
| Source DTO version24 and raw/typed validation flow; geometry rejects unknowns | SD:7; SC:261–274 /324–340; VG:10–40 | Reading validators is not new-profile admission/runtime proof; typed load path does not run disk raw JSON checks |

The binding raw/typed distinction matters: SC:324–337 accepts a typed DTO after typed checks, while disk raw validators are SC:261–274. The signed shared-boundary requirement must close public/typed paths under SAVE:188–190; no harness-only trusted bypass is authorised. Existing CaptureSaveData can initialise understorey (M:1394–1399); it must not be called an already pure refusal sentinel.

### Proposed D84-PJ single-success mutation — no current implementation claim

1. At genuine Pending G designation, bind the exact order ID only after approved acknowledgement/site rules. Per-tree actor/shelter and quote remain ordinary fields. Approval freezes them; no auto-execution (P:95–98).
2. At annual resolution for reportY, prevalidate exact site/cell/species, owned stock, cash/quote, owner minutes, absence of an existing row for this order or juvenile, and the transaction's valid Compact context. Allocate/capture the juvenile identity **directly from the same successful creation**, never a list tail, coordinate match, ID arithmetic or adjacency. Current source adds the juvenile inline and returns void; a reviewed Compact mapping must make that identity available without changing historical DTOs (P:99–107).
3. Treat proposed juvenile, stock/cash/minute/shelter/clearance changes, Completed/resolvedYear and one signed-shape `{workOrderId,speciesId,juvenileId,completionYear=Y}` row as one non-reentrant in-memory success transaction. Append the row only after success is established and **before WorkResolved and before saving can observe the state**. Record WorkResolved/Succeeded once, then finish ordinary annual ecology/report. Failure produces no juvenile/row, no stock/cash settlement and no review credit (P:99–118).
4. This ordering is **not proof of exception atomicity in existing C#**. Current stock deduction precedes juvenile creation and checked cash arithmetic follows it (M:1890–1921); a thrown exception could expose partial effects unless the future implementation prevalidates/stages or rolls back safely and prevents save re-entry. Require failure injection around every proposed commit boundary. Do not claim the current method is already transactional because normal validation fails early.
5. Duplicate order row or reused juvenile ID is a refusal, not “ignore and succeed”. A Completed G member missing its row refuses; load never creates, repairs or drops one. A removed cancelled order is legitimate only with its cancellation history; Failed/Pending/Approved remain incomplete (SAVE:165–167; P:103–113).
6. Admission cross-checks row against retained real Completed exact order or authorised retained completion history, matching species, stock use, resolvedYear=completionYear, real juvenile identity/plantingYear/stock and successful WorkResolved/year; ordinary later death/promotion does not invalidate. Current proposal uses the retained Completed order for this candidate and does not silently prune it. Coordinates are an optional extra consistency refusal, never the link.
7. Replay same pre-year state + same input/model/content/order sorting + restored deterministic counters ⇒ expected same IDs/rows/year/cost. This is **MODEL-DERIVED reasoning**, not a native replay PASS. Reload is observation/restoration, never an event or new transaction; repeating ack/save/load grants no extra row, stock, cash or year.
8. Keep mutations in signed teaching rows only. Adding juvenileId to WorkResolved/order or workOrderId to PlantedJuvenileSaveData would alter the closed historical v24 type graph; it needs a separately signed revision and is **not** this proposal. No code path currently captures the signed row at this base.

Disk save atomicity is a separate SAVE:196 FR-05 obligation: validate first, selected own `.tmp`, qualified atomic commit/own backup, no File.Copy-over-primary fallback, no auto-save or cross-slot repair. A synchronous game-state transaction cannot certify filesystem/process-crash safety. Neither was tested here.

### W-A and ordinary work semantics stay intact

**OWNER CHOICE (D-084)** plus unsigned implementation requirements (P:186–203; C:19–25): Approved order type/site/species/stock/method/shelter/outcome/quote immutable. Confirmed cancel removes, records OrderCancelled and releases reservations; a replacement is a **new** designation, ID, quote and approval. No same-ID edit/reapprove story.

Current SetPlantingExecution/SetPlantingShelters touch IsOpen (including Approved), demoting to Pending (M:875–889); WorkPlanView enables both using CanEditWorkPlan (`Assets/ForestPrototype/UI/WorkPlanView.cs:450–461`; M:333). These are source gaps for Compact W-A, not accepted alternatives. Future Compact guards must cover UI and service/batch routes; old-world behaviour and rack-owned refusal stay unchanged (M:1196–1207).

Pending **per-tree** actor/shelter choices remain required (CIP-016/C:17–23); batch-only controls are insufficient. Shelter uses same planting executor and actual optional cost; no forced shelter. Spending and real work settle in the ordinary annual resolution (M:1280–1368 /1890–1924), with no synthetic Year event, automatic G1 planting, free stock, H1 Fell/revenue, invisible grant or assumed affordability. D-084's €12,000 is game calibration; a later legitimate M-A route still needs a real affordability witness. Readable actual survival/loss for both broadleaves is required, not a mandatory later order on each species (P:146 /427–435).

## 5. M-A evidence dossier — scope deliberately unresolved

**Current OWNER INTENT**, `Docs/Project/decision-log.md:151` / P:137: one later genuinely substantive, spatially auditable assessed treatment with actual Approved→Completed work and reviewed annual consequence; then a later inspected adaptive **act or no-work** choice and its review. Token Fell, proposal, automatic recruitment or elapsed three-cycle quota is insufficient. P:139–150 supplies an **unsigned recommended predicate**, not accepted thresholds. The owner explicitly deferred whether one treated stem qualifies. **No definitive completion classifier can be signed tonight.**

| Required fact | What current source persists / computes | Evidence adequacy |
| --- | --- | --- |
| Dated focal declaration before approval | Mutable CropTree mark (T:18–23 /91–100; SD:206); manual work targetTreeId is the acted-on stem (WO:34), not a declared retained focal | UNSUPPORTED / NOT IN SOURCE for dated decision |
| Explicitly retained comparable neighbours | No retained-choice record in EV:4–17 /40–70 or WO:29–72 | UNSUPPORTED / NOT IN SOURCE; survival is not intent |
| Decision-time neighbourhood | EC:912 fixes HegyiCutoffMeters=8f; runtime positions identify current neighbourhood | Rule SOURCE-VERIFIED; historical decision-time membership unsupported |
| Before/after local competition or juvenile light | Runtime competitionIndex dictionary (EC:52 /932); SNAP:20–44 has stand/species aggregates, no per-focal before/after | UNSUPPORTED / NOT IN SOURCE; later live values/BA trend cannot reconstruct it |
| Actual non-G1 treatment and cost | Orders/events name acted-on target/order/year/type and costs; report settlement can be grouped for harvest (M:1313–1339 /1377–1389; EV:47–63) | Partial SOURCE-VERIFIED; not a focal/retention link, and last felling event may hold group cost |
| Covering review and chronology | Event9 enum and year/eventId (EV:16 /42–44; M:391–401 /2475–2479) | Data supports a proposed binding; real UI origin/new Compact semantics still absent |
| Later adaptive act or explicit no-work | Real action order/event exists; no no-work event type or explicit no-work decision in EV:4–17 | UNSUPPORTED / NOT IN SOURCE for no-work; no orders ≠ intentional choice |
| Completion/episode identity and replay safety | Existing order/event counters only (SD:60–65); no Compact M-A record in SD | UNSUPPORTED / NOT IN SOURCE for full episode |

The source **8m Hegyi cutoff is a model calibration constant**, not an owner choice or empirical minimum-treatment radius. EC:920–925 gives the Hegyi term; no newly invented CI/light improvement threshold is supplied. Manual orders cannot misuse prescriptionCropTreeId as a spare focal field: WO:60–70 requires other prescription fields default for manual orders, and VAL:520–522 treats any nonempty link as prescription provenance. The approved MFM spec's sample/rack authority (`specs/multiscale-forestry-management-v1/spec.md:95–120`) cannot silently be repurposed as Compact rack evidence.

### S1 candidate — separate, versioned Compact-only record; every element NEW / UNSIGNED

This table specifies **semantic data requirements**, not adopted JSON field names or numeric schema versions. P:169–176 is the source proposal. Existing proposed names `focalId`, `retainedNeighbourId`, `workOrderId` are quoted only where already present there. Exact container placement, wire names/types/enums/version literals are **UNSUPPORTED / NOT IN SOURCE as a signed schema** and must come from the save owner. In particular, a new outer record cannot bypass the closed three-root policy (§1).

| Proposed data element — ALL UNSIGNED | Type / invariant candidate | Mutation point / ordering / purpose |
| --- | --- | --- |
| Evidence format identity/version | Explicit allowlisted version and type, no value invented here; foreign/unknown/older-incompatible record refuses | Bind when creating a new Compact episode; never default a historical file into S1 |
| Chapter/profile/world/content binding | Exact active signed registry/accepted manifest identities, immutable episode association | Bind before any focal declaration; foreign profile/content cannot share evidence or counters |
| Episode identity | Stable unique identity, allocation/idempotence rule still to be specified | Allocate once from an approved Compact-only mechanism; no new ID string or counter field chosen tonight |
| `focalId` and identity kind | Existing tree ID or G1 juvenileId with unambiguous kind; living eligible focal at declaration | Explicit player decision **before treatment approval**; never infer from current marks or later crop status |
| Declaration year / ordering evidence | Actual current ecological year and approved source ordering link; same-year order must be auditable without synthetic events | Capture at explicit declaration; if eventId cannot express it without new event, revision must define genuine new record chronology |
| Focal decision-time position/model context | Same actual focal, accepted world coordinates and model inputs/recipe identity | Freeze decision-time interpretation; a moving/generated current tree list cannot redefine the episode |
| Explicit `retainedNeighbourId` set | Distinct actually living chosen comparators inside the source8m neighbourhood at declaration, excluding acted-on targets | Player explicitly records retention before approval; no inference from unharvested survivors |
| Decision-time neighbourhood witness | Validated membership/positions/needed dimensions or independently reproducible pinned decision snapshot | Sufficient to audit8m inclusion after felling; exact storage choice needs revision, no fabricated historical DBH |
| Non-G1 treatment order link(s) | Real workOrderId/target/type; qualifying whitelist still owner/spec-gated | Bind designated work to declared episode before approval; proposal/manual mark alone grants nothing |
| Approval and completion chronology | Genuine Approved→Completed transition and Y_t>Y*, matching actual order/events | Append actual resolution link once at success; Failed/cancelled stays in history, no treatment credit |
| Settled cost/time | Real settlement and source accounting scope, units from actual fields | Capture actual cost/owner time at resolution; grouped-harvest charge must be referenced as group total or transparently allocated by a separately approved rule, never charged once per stem |
| Before observation | Same-focal decision-time local Hegyi CI, or juvenile competition/light input as appropriate; actual dated model state | Capture before relevant resolution; if proposal-to-approval world changed, require re-observation under reviewed rule, not stale forecast masquerading as before |
| After observation | Same-focal metric/context at first actual annual snapshot after Y_t | Record actual consequence, not guaranteed improvement; do not substitute stand-average light/BA or changed metric units |
| Treatment covering-review link | Actual retained Y_t report and event9/yearY_t after **all linked** WorkResolved events | Bind through genuine latest-review UI acknowledgement; nothing backdated on load |
| Later reassessment and choice | Explicit inspected choice at Y_a>Y_t; either linked further real action OR an explicit no-work record | Capture at actual player choice; do not encode no-work as Completed order, artificial WorkResolved or elapsed empty year |
| Adaptive action evidence, if action | Actual approved/resolved work IDs, result/cost and year under approved scope | Completion/credit only after normal success; cancellation/failure not a successful adaptive act |
| Adaptive no-work evidence, if no-work | Explicit real decision record, no stock/cost/work settlement invented | Persist the choice as its own approved evidence kind; not a new ScenarioWorkStatus value |
| Adaptive covering review | Actual reportY_a and later genuine event9/yearY_a; action branch after linked resolutions | Both action and no-work branches require their year's acknowledged review; earlier treatment review cannot satisfy it |
| Integrity / completion linkage | Unique ordered entries; episode facts append-only at genuine transitions; any eventual completion is derived from accepted rule/version | Reload/replay cannot append/repair; store final achievement only if a separately signed design requires it, not in closed §4.2 by convenience |

**Temporal/atomic candidate rules:** keep the episode identity immutable; append facts once at declaration, approval/resolution, observations, reassessment and corresponding review boundaries. “Append-only” describes episode evidence, not permission to mutate an already sealed fact or set all fields at final load. Capture the same source local metric and units at both observation points; adult CI and juvenile light are different typed observations, never compared as if interchangeable. Preserve real source order/event/report links; no duplicate event stream fabricated to prove intent. The exact allowed-state/version graph and error atomicity belong in the signed S1 revision (P:169–181), with failure injection between mutations.

**Replay/reload:** same admitted world/episode version+actual input sequence must derive same eligible facts, IDs, metrics and outcome; save/load must neither add a focal, infer retention nor manufacture no-work/review. Validate identity, uniqueness, chronology, neighbourhood membership and matching ordinary history before applying. Unknown record version, wrong content/profile, impossible year/order or duplicate identity refuses the new-profile load. Historical v1–v24 files remain byte/meaning unchanged and do not acquire an empty/default “M-A complete” record. SI-10 materiality begins at first integrated writer, not first beta release (SAVE:182).

**What S1 could unlock:** persistence of currently demanded intent/observation facts after review, revised signature and implementation. **What it cannot unlock:** a decision that one treated stem qualifies, any arbitrary quota/CI improvement number, a signed world, ISO-58, affordable legitimate work, native validation, old-save safety or spec APPROVED status. Every element above stays **UNSIGNED**.

### S2 existing-state-only — not supportable as written

S2 has real targets/costs/events/reviews but lacks dated focal declaration, explicit retention, decision-time neighbourhood, local before/after and explicit no-work choice. WorkResolved proves acted-on work, not purpose; a standing neighbour is not an explicit retention decision; mutable crop marks carry no declaration year; stand aggregates do not reconstruct a felled neighbour's historic DBH/competition; an empty work year is not an adaptive decision. These absences are directly visible in WO/EV/SD/SNAP and EC above. Therefore **S2 cannot evaluate P:MA-2…MA-6 as written** (P:181–182). Saving no new fields is not a licence to weaken owner intent. Only the owner/Manager could separately redefine the demanded evidence and have the save owner bind an actual supportable rule; that decision is not made here.

### M-A-SCOPE — explicit open classifier parameter, no unilateral choice

Represent the unchosen policy symbolically as **ScopeEligible(treatment evidence; owner ruling)**. It is not a wire field or an implemented predicate. Candidate full completion would require T1, valid focal/retention/observations/real non-G1 work+review, later adaptive choice+review, honest three-species inspectability, **and** ScopeEligible. While that ruling is absent, this is an unbound parameter, not executable acceptance logic.

| Deferred branch | Effect on eventual classifier / witness | Present disposition |
| --- | --- | --- |
| Owner later permits a single treated stem when all non-token conditions hold | One-stem case could be eligible only with complete focal/retention/observed consequence/real cost/two reviews and later adaptation; isolated token Fell still fails | **DEFERRED**, not recommended/decided anew here |
| Owner later does not permit that case | One-stem case would fail; what additional scope qualifies remains for owner definition and review | **DEFERRED**; no substitute numerical quota invented |

Do not solicit this deferred choice tonight. Persistence schema analysis can proceed as a proposal; no definitive completion classifier or M-A signature can proceed with that parameter unbound. Preserve historical two-cycle objectives and do not scale old40/80m extensive targets into Compact as a shortcut (`Assets/ForestPrototype/ScenarioOne/ScenarioOneDefinition.cs:113–130`).

## 6. Minimum authority/decision matrix and actual handoff path

**Save-policy signature authority actually evidenced:** user-delegated OpenAI GPT-6 Overall Manager, attestation SIG:3–16 / claim[#543](https://github.com/jermur788/CCF/issues/543). Current routing is Overall Manager[#640](https://github.com/jermur788/CCF/issues/640) / canonical Overall Manager[#425](https://github.com/jermur788/CCF/issues/425), coordinating shared-save workstream[#544](https://github.com/jermur788/CCF/issues/544). The older #424 references are historical provenance, not a separate absent Manager who must answer. The #544 issue's latest inspected comments explicitly transfer current managerial coordination to #425; source custody and signature authority remain different things. No currently active save implementer's personal name/lease is inferred.

[#647](https://github.com/jermur788/CCF/issues/647)'s recent source-only handoff/Manager comment reports approved fixture incorporation at `209a08e9f220841aaec81385d4cb7b437fa5089f` in **unmerged** `feature/scen1-shared-three-profile-save-v2`, not main/native qualification. This is **source-status reported there**, not code run or imported by this author. The next ruling path is #668 → #667 orchestrator → #640/#425 → shared-save policy owner/#544 for a bounded signed revision/CG-TRIG binding; it is not permission to edit that protected lane. This author posts only the requested claim handoff, not messages to the other owners.

| Decision | Options / correct authority | Ordering / unlock | Explicitly cannot unlock |
| --- | --- | --- | --- |
| Complete world binding | Resolve W01…W14 candidates, or return precise amendments; content owner, architecture Manager and rights authority | Before signing path/G1/world and enabling authored binding | Save policy amendment, source release, ecological/player proof |
| CG-TRIG / genuine controls | Bind corrected T1/CT-1…CT-7 and D84-PJ, or return changes; save-policy owner + teaching/content owner + Overall Manager | After exact control/site prerequisites understood; before ISO-58 implementation/acceptance | Spec approval/native GO simply from this dossier |
| Impossible-control admission | A strict refusal vs B incomplete; save owner rules with exact text/compatibility consequences | Before validator and negative/reload cases fixed | Choice of M-A-SCOPE, new fields, old-save tightening |
| Review mutation/lifecycle edge | Specify one-sided event/field failure and late-current-year handling without repair/backdating; save owner + teaching owner | Same CG-TRIG binding before implementation | A fabricated Y* review or relaxed year equality |
| Pairing runtime owner/transaction | Approve concrete same-mutation signed-row mapping or separately revise schema if extra fields unavoidable; save owner + source owner/Manager | Must bind before code writer and failure injection | Historical DTO edit under existing signature |
| W-A/per-tree actor/shelter | Approve concrete Compact guards and cancel/new-quote UI; Work Plan owner/Overall Manager under D-084 | Source packet only after reviewed spec/ownership | Silent Approved demotion or batch-only lesson |
| M-A evidence route | S1 signed separate format proposal; S2 unavailable as written unless owner redefines intent; save owner + Overall Manager, owner for intent changes | Exact keys/version/container/admission review and fresh signature before storage implementation | Closed-root/key bypass or automatic completion |
| M-A-SCOPE | Later owner/content ruling only; **DEFERRED**, no request now | Necessary before final completion classifier/signature | Any quota/one-tree decision by author, reviewer or save owner alone |
| Qualification/release | Future source/fixture/native/old-save/world gates after genuine ownership and authorised Ops; Manager/physical custodian | #652 live deployment remains HELD; #548/native prerequisites not invented | Unity tonight, main merge, personal acceptance or Windows build |

## 7. Future regression witnesses — entire table NOT RUN / FUTURE

No harness/Unity/Editor/compiler/test process was executed. These are concrete later steps with expected observable results, not a test report. The eventual authorised packet must choose exact tooling/candidate/fixtures and safe isolated lane; this dossier intentionally gives no instruction to launch an Editor now.

| Witness action on an eventual approved candidate | Expected observable result | Required evidence |
| --- | --- | --- |
| Start unavailable Compact / missing authored binding | Explicit unavailable/refusal before destination generation; no historical fallback | Identity/world/scene/slot/Prefs before-after sentinels |
| New true Compact, acknowledge arrival, then current assessment | Separate visible controls and persisted signed booleans; no menu/proximity credit | UI path + payload rows, current world/year shown |
| Save/reload at arrival-only and assessed/noG | Exact partial restored, no auto orders/acknowledgements | Byte/typed values and UI state |
| Save/reload G sizes1,2,3,4 Pending; subset Approved | Same IDs/sites/species/stock reservations; no planted juvenile yet; Approved locked | Each cardinality and mixed-status record/observable marker |
|2 oak+2 beech real owned-stock orders with mixed per-tree executor/shelter choices | Work Plan shows each quote, ordinary annual settlement produces exactly4 unique pairs; no forced protection | Order/stock/cash/time/event/juvenile/evidence-row reconciliation |
| Failure before settlement: invalid species/site/insufficient stock/cash | No row/juvenile/stock/cash spend; truthful Failed/refusal, no T1 | Boundary failure injection and exact deltas |
| Inject failure/re-entry between stock, juvenile, cash, Completed, row, event and report boundaries | No capturable inconsistent committed state; safe failure or rollback; no one-sided event/field or pair | Same-mutation failure/snapshot/callback evidence |
| Cancel Pending / confirm or decline Approved cancellation | Decline unchanged; confirm removes/audits/releases; replacement new ID/quote; old event retained | No resurrected work/duplicate execution |
| Duplicate row/order/juvenile, fabricated/foreign/historical ID, wrong stock/species/year | Pre-mutation refusal, no dropping/repair into credit | Primary/temp/backup, live world/Prefs checksums equal |
| Complete final4 across multiple years; reload before latest covering review | Y*=actual max; CT-3 persists and offers correct report | Each partial year, final report/order/event chronology |
| Open latest Y* review without ack; attempt button/API annual advance and any future skip route | No tick/work/event; visible hold reason + working review route | UI and service route negatives, unchanged year/cash |
| Acknowledge real latest Y* review | Event9/yearY* follows all G WorkResolved; fieldY* in same mutation; T1 only if A/B true | Native callback trace + retained report/evidence payload |
| Repeat ack/save/reload then attempt next permitted advance | No duplicate review/row/work/resource award; one legitimate annual tick | Stable identity/counters/history |
| Earlier-year browse, direct harness call, event9 before finalwork or later-year9 | No CT-2; invalid persisted review claim refuses; no backdated credit | Explicit negative fixtures and UI-origin cases |
| NonemptyG with falseA, falseB, bothfalse under each policy fixture | Selected save-owner A or B observed exactly; never teaching-complete | Ruling pin + clear refusal or incomplete prompt; valid work unchanged |
| Late CurrentYear pastY* without valid review; one-sided event/field mutation | Exact save-owner-approved refusal/incomplete rule; no load-time repair | Signed ruling + atomic failure/reload cases |
| True G1 later dies or promotes | Successful execution rows retained; actual dead/promoted state inspectable, no resurrection | Original juvenile ID / promotedTreeId/history continuity |
| S1 declaration/treatment/review/reassessment(act) / review, with each midway reload | Same valid episode evidence and genuine annual work/cost; no synthetic year | Signed new version + exact per-transition comparison |
| Same S1 with explicit no-work branch | Explicit decision persists, ordinary no-work year/review; no fake Completed/order cost | Distinct genuine choice record and real report |
| S1 missing declaration/retention/before metric/no-work record; foreign/unknown/version/content/duplicate | Reject invalid record or deny completion as signed rules require; never infer from live marks/survival/stand average | Native admission and semantic-negative fixtures |
| One-treated-stem classifier case | **No PASS/FAIL expected until later owner ScopeEligible ruling**; then test both ruling-positive and token-negative cases | Deferred rule pin, no fabricated quota |
| Two-way Compact↔historical, O2↔Compact, new envelope at old filename; all public LoadData/F9/F10/Continue/backup/preview routes | Wrong identity denied before world/scene/Prefs/files; genuine/ambiguous historical admission unchanged; O2 unavailable | Full protected sentinels plus same-slot paths only |
| Genuine v1–v24, Legacy40/Enlarged80 Rack V2/MFM, model0 and frozen Reference fixtures | Exact accepted semantics/anchors/archive bytes, no automatic migration/resave/new fields; historical two-cycle/latch unchanged | Provenance-qualified actual old corpus, exact baseline/candidate comparison |
| Selected-save atomic replace/failure/process interruption/backup load | Own slot only, old/new complete file per qualified platform matrix; no File.Copy overwrite fallback or cross-slot repair | Signed FR-05/06/ISO evidence; limits documented |
| Full approved Compact loop and affordability, native720p/1080p H1→A7→G1 walk | Honest ecology/stock/money/guard visibility and later real management/reviews; no H1 revenue/free grant; old world unchanged | World W01…W14 witnesses plus authorised feature qualification |

## 8. Blocker register, conflicts and handoff limits

Eight save/decision blocker groups, **not findings of a failed executed runtime test**:

| ID | What is absent / who must supply it | Status / dependency |
| --- | --- | --- |
| S01 | Actual signed world/registry implementation binding and single shared boundary/source mapping; content + save owner/Manager | BLOCKED; companion W14/W13; base is not unmerged #544 |
| S02 | Signed CG-TRIG T1/controls/read signal/admission policy including both §3 alternatives and review-edge cases; save + teaching owner | ISO-58 BLOCKED; this proposal signs none of it |
| S03 | Implementable atomic/idempotent direct pairing capture and error-safe runtime ownership with exact load reconciliation; source/save owner | UNSUPPORTED / NOT IN SOURCE at base; no new historical fields authorised |
| S04 | Compact W-A guards, cancellation confirmation and per-tree actor/shelter flow; Work Plan owner/Manager | Source presently mutates Approved via batch setters; future approved source task |
| S05 | Signed S1 exact schema/version/container/path/admission rules OR explicit owner redefinition making S2 provable; save owner + Manager | UNSIGNED; current root is closed; S2 unsupported as written |
| S06 | Owner's later M-A-SCOPE classifier ruling | DEFERRED / UNDECIDED; no re-ask or substitute threshold |
| S07 | All native/reload/foreign-profile/historical/regeneration/economy/geometry/failure witnesses in §7 under real authorised custody | NOT RUN; #652 live deployment HELD, no Unity GO inferred |
| S08 | Independent nonauthor dossier review, subsequent Manager/spec/contract/content dispositions and operational synchronization | NOT DONE here; author may hand off, cannot self-review/approve; dashboard/Drive UNVERIFIED |

Together with world W01…W14, there are **22 blocker-register entries**, with overlapping dependencies explicitly named. The companion's matrix has88 fields,44 unsupported field rows and36 incomplete cell-ecology rows (**80 unsupported table rows**); these are content gaps, not80 separate implementation defects. This document adds the8 blocker groups above; its source-adequacy table separately identifies missing M-A evidence rather than pretending to count them as distinct closed tasks.

**Conflicts observed / precedence:** the dispatch's duplicate native-folder ledger path is absent, actual #508 ledger authenticates successfully (companion §1). Signed SAVE:167's incomplete-state wording and stricter impossible-control admission optionA need a real ruling, not silent reinterpretation. The closed root/type graph conflicts with any attempt to add S1 under the old tuple; a new separately signed revision is required. The source's current28° failed shelter conflicts with intended Compact geometry but is not a collision-test result (companion). Historical one-tree recommendations in P:148 are superseded in authority by the owner's explicit deferral; #665 approved changed-lines document quality only. Neither dossier claims those owner/content/save choices are settled.

**Actually performed:** dispatched-base/clean-scope check; static C# and signed-policy reads; raw source hashing and9/9 SHA256SUMS integrity check; GitHub175-row digest/raw reproduction; offline membership/cell/route/guard-envelope arithmetic. No tests were compiled or run. These author preservation/scope checks are not self-review or independent verification of this dossier's quality.

**NOT RUN / NOT DONE:** Unity/Editor/Ops installation/lease, compiler/typecheck/source tests, archived harness/script execution, Player/Windows, runtime interactions/Work Plan/stock/cash/settlement/harvest-once/regeneration/survival, annual/review/skip behaviour, save/load/replay/error/crash/backup or historical/frozen Reference qualification, imported scene/mesh/capsule/guard/render/performance, owner playtest, spec approval/content signature/save amendment/profile activation, independent review, main integration, dashboard/published-state/Drive/context-mirror synchronization. All §7 witnesses remain **FUTURE / NOT RUN**.

No production source, scene/prefab/YAML/.meta/package, personal save/Prefs, spec/plan/checklist/annex, frozen Reference, Docs/Project or other worker file was changed. No approval/signature/status checkbox, invented hash/coordinate/quota/wire key, unilateral M-A-SCOPE decision, force push or main write. Exactly the two authorised new Markdown files are the deliverable. The three Compact spec families remain **DRAFT**, roster **UNSIGNED**, ISO-58 **BLOCKED**, #652 deployment **HELD**. Return through #668/#667 for a separate independent nonauthor review; this is **SIGNATURE-NOT-READY** evidence, not an accepted contract.
