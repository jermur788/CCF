# Overnight Multi 2D — acceptance and negative cases
**Status: OWNER-DIRECTED OVERNIGHT ORCHESTRATION SPEC — execution still subject to live claim/source/physical locks and bounded path grants.** Latest owner (2026-11-21 00:35 UTC) prioritises getting improved simulation logic tested and working via ONE long-running DeepSeek-orchestrated effort, with Unity Stand Map visual reuse optional and nonblocking. This supersedes the earlier "do not start source implementation yet" only as a direction to PREPARE/ROUTE a guarded start; it does **not** waive task admission, approved MA-1 contract, independent review, G4 protected paths or physical reservations. No work is running from authoring this specification.
**All checkboxes UNCHECKED. These are future tests, not proof of execution.**

### Admission and custody
- [ ] A-01 Live coordination and #882/#885 status refreshed; exact source SHA and owner playtest custody recorded
- [ ] A-02 One actual source writer, protected/reused file scopes and compile-linked paths separately granted; host/lane/lock recorded
- [ ] A-03 No owner-private token, other agent PID, save, Unity scene or unapproved asset touched

### Simulation-positive
- [ ] R-01 Two truly different synthetic Site A/B tree rosters, IDs, geometry/areaHa, seeds, catalogue/hashes and disclosure
- [ ] R-02 Tree click/table selection returns top living-neighbour competition immediately without Crop marking; no state mutation
- [ ] R-03 Crop designation remains explicit; felling is manually reasoned, never inferred from high CI
- [ ] R-04 Per-area living counts, basal area, Hegyi CI and Year+1 follow the unchanged actual annual engine and area seed
- [ ] R-05 One globally atomic Work Plan, mixed-area Crop/Fell, Approve0, Execute1 on BOTH real trees, one receipt/event with separate area summaries
- [ ] R-06 Area-only work, no-action, and cancellation are allowed; after real Execute Year+1 depends on actual remaining trees
- [ ] R-07 UI area switcher shows real current trees in both, plan+receipt and forest differences; synthetic/isolated disclaimers
- [ ] R-08 Underlying pan/zoom remains functional; optional overlay controls float inside map, usable and labelled

### Negative/refusals
- [ ] X-01 Duplicate/missing JSON/fence fields, wrong-area/dead/Crop-protected tree, duplicate ID and malformed manifest refuse without mutation
- [ ] X-02 Concurrent/stale root and year/source/plan revisions refuse entire operation; no partial A/B publish
- [ ] X-03 Fault after constructing A state before B or beforePublish leaves zero receipt/tree/revision changes anywhere
- [ ] X-04 Same-key replay idempotent, altered-key payload and unminted key refuse; no second Fell
- [ ] X-05 Fell-to-zero of ANY populated area refused; naturally emptied area stays empty while live areas advance; all-empty year refuses
- [ ] X-06 Pending/approved work Year+1 requires whole-plan cancellation consent; no implicit Fell
- [ ] X-07 Any late competitor result for prior selection/year/fence discarded; read-only API no Crop promotion
- [ ] X-08 Original P30, ten no-work golden, frozen P0, historical save semantics and #882 malformed request guards unchanged

### Evidence/qualification
- [ ] V-01 Tier1 Core/negative tests observed with source SHA, exact environment and preserved failed attempts
- [ ] V-02 Tier2 player UI exact A/B browser run, malformed HTTP, 1280×720/200%/keyboard/focus, actual Execute Year+1
- [ ] V-03 Independent reviewer verdict APPROVED/NEEDS CHANGES/REJECTED on exact source; not self-review
- [ ] V-04 Manager G4 integration verdict separate and owner G2 final acceptance separate; no accidental merge or Windows build
- [ ] V-05 Final durable GitHub Markdown handoff with paths, SHA, results, failures, blockers, active process/lock status and next step
- [ ] V-06 Optional Unity visual mapping did not block simulation progress or claim browser runs Unity assets

**Authoritative MA-1 approved spec:** https://github.com/jermur788/CCF/blob/docs/multi-885-ma1-approved-spec-and-packet-20261120-v1/specs/multi-2d-ma1-manual-multi-area-workplan-v1/spec.md . **MA-1 bounded packet:** https://github.com/jermur788/CCF/blob/docs/multi-885-ma1-approved-spec-and-packet-20261120-v1/Docs/Handoffs/MultiRack-Simulation-20261116/885-multi-area-spec/Multi-885-MA1-APPROVED-SPEC-AND-BOUNDED-IMPLEMENTATION-PACKET-20261120.md . **#882 owner G2 feedback:** https://github.com/jermur788/CCF/issues/882#issuecomment-7051229907 and https://github.com/jermur788/CCF/issues/882#issuecomment-7051244047 . **#852 design:** https://github.com/jermur788/CCF/blob/c23e411555771320cb327e918c7ae80887388889/Docs/Handoffs/MultiRack-Simulation-20261116/852-orchestration/Multi-852-G1-FIXTURE-AREA-IDENTITY-PER-AREA-ENGINE-AND-WORK-PLAN-DESIGN-RESULT-20261120.md . **#828 positive G1:** https://github.com/jermur788/CCF/issues/828 . **#885 Manager:** https://github.com/jermur788/CCF/issues/885 .
