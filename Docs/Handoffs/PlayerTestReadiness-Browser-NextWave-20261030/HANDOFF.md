# Player-Test Readiness — Browser Next Wave — 2026-10-30

**Parent:** #91 `PLAYER-TEST-OVERNIGHT-01`  
**Browser lane:** #162  
**Label:** BROWSER-CAPABLE  
**Child claims:** #177 PARITY-09 readiness; #178 MFM-08 static pre-review  
**Published integration truth at preparation:** `8fa83bff22ea1b4943689bee1074eed8eae65d2f`

## Purpose

Use browser/GitHub capacity while the serialized Unity lane advances #151 and then #156. Do not consume a local Code/Unity worker for work that can be completed from pushed repository evidence.

This handoff contains two independent browser-safe work units. Keep their claims and outputs separate.

## Start gate

Before substantive work:

1. Refresh `Docs/Project/coordination/active-tasks.md`, #91, #106, #151, #156, #162, #177 and #178.
2. Refresh `integration/scenario-one-player-test-ready-v1`; do not assume `8fa83bff...` is still current if a newer exact integration has landed.
3. Treat stale issue titles/bodies as historical when newer comments, handoffs, reviews or the live ledger supersede them.
4. Make no local filesystem, Unity-lock, Editor-process or runtime assertions from GitHub evidence.

If #151 publishes a completed Unity handoff while this browser session is running, its final Manager/browser review becomes higher priority. Finish the current atomic GitHub write, then review #151 under its existing feature claim before resuming lower-priority readiness work.

---

## Work unit A — #177 PARITY-09 dependency/readiness audit

**Claim:** #177  
**Difficulty:** 3/5 — historical parity state is spread across stale issues, later comments, handoffs and integration history.  
**Best suited:** DeepSeek for evidence reconciliation. Use Claude Sonnet 5.5 Medium or an OpenAI Sol-class independent reviewer only if a conformance judgement cannot be resolved from durable evidence.  
**Local lane/worktree:** none.

### Authority

Approved `specs/scenario-one-three-species-parity-p0/`, especially PARITY-09:

- combined three-species qualification;
- pure/mixed density;
- adult/juvenile/browse/pruning;
- timber/economy;
- save/load;
- player-facing truth;
- completion/solvency;
- MFM Sitka-only guard;
- protected anchors;
- Tier-2 qualification plus independent review;
- no Windows build.

### Required browser audit

For PARITY-03 through PARITY-08, establish an exact matrix with at least:

- implementation candidate SHA;
- Manager/independent review result;
- Unity qualification evidence where required;
- integration SHA or proof that integration is still missing;
- remaining residuals and whether each is blocking/non-blocking.

Pay particular attention to:

- PARITY-05: durable Unity qualification evidence exists; verify the subsequent Manager/integration disposition rather than relying on the old `[WAITING]` issue body.
- PARITY-06: durable Unity qualification exists; verify the exact integration lineage rather than the old `[RUNNING]` issue title.
- PARITY-07: execution claim #102 is closed and older evidence says wording correction/rendered qualification was locally integrated on an umbrella that at that time was not independently GitHub-published. Determine whether later published integration history now proves the exact PARITY-07 production bytes are present. Do not infer this from claim closure alone.
- PARITY-08: independently approved and integrated; exact reviewed candidate `3eb9c954c363659a1b012e06ce58a35ab87f4303`, merge `7d2049e862df5c9311748167423b981e713d1fa9`, docs successor `8fa83bff22ea1b4943689bee1074eed8eae65d2f` at handoff preparation.
- D-047/D-048 model-0 completion debt remains historical verification debt; do not rebaseline `6F84AF319D301F87`.
- Frozen Reference Future v1 and protected Sitka anchors remain unchanged.
- Identify the exact evidence needed for PARITY-09's MFM Sitka-only guard. If it is not already durably proven, specify the smallest local verification gate; do not invent behaviour or make MFM production edits.

### Output

Publish:

`Docs/Handoffs/S1-ThreeSpeciesParity-P0-Implementation-09/BROWSER-READINESS-20261030.md`

If every PARITY-03…08 prerequisite is durably accepted/integrated, create the substantive PARITY-09 local qualification claim and exact Code packet pinned to the then-current integration SHA. The packet must name Tier-2 gates, independent-review requirement, protected-anchor/Reference constraints, branch/worktree guidance and success token `THREE-SPECIES PARITY P0 VERIFIED`.

If any prerequisite is missing, do **not** release local PARITY-09. Publish the exact blockers and smallest next actions.

Return one of:

- `PARITY-09 READY — EXACT LOCAL QUALIFICATION PACKET PUBLISHED`
- `PARITY-09 NOT READY — EXACT PREREQUISITE GAPS PUBLISHED`

---

## Work unit B — #178 MFM-08 static pre-review

**Claim:** #178  
**Difficulty:** 3/5 — bounded source/UI semantics review; runtime/rendered questions must remain honestly unverified.  
**Best suited:** Claude Sonnet 5.5 Medium for independent UI/source review. DeepSeek is acceptable if Claude availability is impaired. Escalate to Opus only for a concrete architecture ambiguity.  
**Local lane/worktree:** none.

### Exact target

- Production candidate: `d55ec86a3f0bf13820f2080197b6b0f43d832e06`
- Docs/handoff tip: `d70cec1781c104794a6f225c8609847c55df1510`
- Handoff: `Docs/Handoffs/MFM-08-Map-3D-Review-UX/HANDOFF.md`

### Review contract

Inspect exact pushed diff and accepted MFM-08 contract. Confirm or reject, from source/static evidence only:

- bounded Stand Map / MFM review / inspection scope;
- no Annual Review/history or generic S1-D Work Plan-history ownership;
- no save/schema/hash/reference/MFM-09/calibration/world-adapter/proposal→Fell/bulk-confirmation changes;
- `ScenarioOneManager.ReadCoverage` is the authoritative coverage read and no second coverage classifier is recreated in UI;
- sample and target coverage are distinct;
- Covered / Not Covered / Excluded / Manual meanings are faithful;
- spacing candidates are inspection prompts, not quality ranking;
- proposals remain noncommitted;
- prescription approval remains separate from Work Plan approval;
- visual semantics are not colour-only and source design cannot be mistaken for Fell/Crop/Rack management marks;
- wording remains `apply your sample treatment to comparable areas` and does not claim `operator selection` as formal Irish terminology.

Classify each open question from the worker handoff as one of:

1. source-resolved;
2. genuine Unity/rendered residual;
3. architecture/product blocker.

Open questions include rack-sample geometry read, living-tree stem list, card height at 1280×720, Painter2D path and tab placement.

Do not invent runtime evidence. A source-static approval does not qualify or integrate #156.

### Output

Publish:

`Docs/Handoffs/MFM-08-Map-3D-Review-UX/BROWSER-PRE-REVIEW-20261030.md`

Verdict must be exactly one of:

- `APPROVED — STATIC/OFFLINE PHASE ONLY; UNITY TIER-2 STILL REQUIRED`
- `NEEDS CHANGES`
- `REJECTED`

Update #178 and #156 with the verdict and exact evidence inspected. Do not close #156 unless later Unity qualification + final Manager review/integration support that action.

---

## Browser boundaries

- No production or verification C# edits.
- No local worktree assertions.
- No Unity/import/compile/runtime/rendered/save execution.
- No branch integration under #177/#178 unless a later explicit Manager review handoff assigns it.
- No Windows build.
- Browser work must not claim that #151 or #156 is Unity-qualified from offline evidence.

## End state

After both work units, update #162 and #91 with:

- exact outputs and verdicts;
- any newly created local qualification claim/packet;
- remaining blockers;
- exact next browser action and exact next local action;
- whether dashboard/live coordination requires a Manager sync.
