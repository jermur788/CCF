# Implementation plan — Irish Sitka 2D Reference Laboratory V1

**Status:** DRAFT, non-authorising. Derived from [spec.md](spec.md). No actual implementation worker/branch/host/Unity lease exists under this plan.

## Work classification

**SPEC REQUIRED**: new standalone user interaction and per-tree state/persistence, forestry calculations, experimental annual model validation and eventual 2D/3D interface boundary. Do not dispatch as a quick packet bypassing approval. **P0 is independently implementable once APPROVED; P1 needs a separate model-evidence gate.** 

## Parallel delivery boundary

This research/lab lane is separate from Scenario One implementation #424 and portfolio authority #425, existing \`Tools/ContentReview/\` text-editor development and #481 small-stand ecology review. The proposed entirely **new** file namespace is \`Tools/MultiRackSitkaLab/**\`. Do not edit \`Assets/**\`, \`Packages/**\`, existing project code, \`specs/**\` outside this spec, Game Brief/Decision Log/Current Milestone, \`Docs/Project/coordination/active-tasks.md\`, or protected saves. Read current GitHub and physical active ownership before claiming any implementation lane. Initial normal local test build is not a Windows game build; stop if a host/Unity machine conflict.

## Proposed package shape, not yet a created implementation

\`\`\`text
Tools/MultiRackSitkaLab/
  README.md                        # execution, evidence labels, no game integration
  src/Core/                        # standalone C# class library (no Unity deps)
  src/Host/                        # loopback-only local API and lab state store
  src/Ui/                          # local 2D browser canvas/SVG and controls
  tests/Core.Tests/                # deterministic headless fixtures and negatives
  tests/Host.Tests/                # serialisation/API/event tests
  tests/Ui/                        # actual browser path, after implementation
  fixtures/synthetic/              # **clearly synthetic** small auditable inputs
  schemas/                         # versioned lab-only input/event/snapshot contract
  evidence/README.md               # no real/personal data committed
\`\`\`

Actual worker must inspect available .NET, Node, browser, disk and project source before pinning framework/dependencies. Prefer minimal package dependencies, offline/no telemetry and local-only host; record dependency licences and reproducible build. Do not develop a full GIS or cloud service. Avoid copying Unity save or code by assumption.

## Sequence and gates

**G0 – Spec review/approval.** Resolve OPEN-L1..L5 enough to approve *P0* assumptions; ensure UX includes explicit proposal/approval/execution separation and no biology claims. Independent forestry evidence review confirms GROWFOR is only aggregate benchmark (and cannot supply per-tree spatial dynamics); no field-data unlock necessary. Manager records formal APPROVED status before source dispatch.

**G1 – Worker/ownership preflight.** Create dedicated issue claim for P0 implementation. Worker registers actual provider/model, isolated branch/worktree and pinned starting HEAD. Refresh live \`active-tasks.md\` and physical Ops read-only, get actual owner permission if machine/file overlap or any Unity process would be impacted. No simultaneous writers in same new directory. Never move, reset or stash unrelated dirty work.

**G2 – Core and data contract.** Implement synthetic fixture manifest, seed/replay, \`ForestState\` and \`TreeState\` with strict schema and unit guards, polygon area, BA/N/Dq and immutable individual-tree/event identities. Headless hand-calculated test vectors and invalid-input negatives first. Do not implement actual biological growth at this gate.

**G3 – Action authority and histories.** Implement one authoritative mark/propose/review/approve/pending/explicit-execute chain, deduplication and stale-state guards, deterministic events and snapshots, lab-only storage isolated from CCF saves. Negative tests for proposed-only no change and duplicate/executed/dead IDs before UI.

**G4 – 2D viewer.** Bind map to core only, with pan/zoom, selection, tree details, status legend, plan preview vs actual metrics, explicit approval and separate execution. Load only synthetic lab fixtures. Actual browser test with GUI and screenshots, no screen depiction accepted as tree-state authority.

**G5 – Proportionate qualification.** Tier 1: deterministic headless unit/property tests; area/BA/unique ID/event/stale-state and save/replay invariants. Tier 2: local browser pathway at ordinary desktop and constrained viewport, at least one real click/approval/execution/replay and negative path. Do NOT run Unity/Windows Player. Independently review outputs and scope before publication; preserve failed attempts/defects. One bounded pass unless problems require targeted repair, not repeated full integration suites.

**G6 – P0 handoff and review.** Report exact branch, diff, test receipts, actual inspected runtime, small synthetic inventory manifest and known missing processes. Reviewer verdict APPROVED / NEEDS CHANGES / REJECTED for **engineering P0** only. No automatic production integration, source-cherry-pick into Unity or release.

**G7 – P1 separately gated.** Independent source audit and explicit choice of existing CCF Sitka lineage; source/model unit and deterministic parity tests plus Irish GROWFOR domain reference. Write an approved P1 delta/qualification packet if necessary; implement annual model only after owner/manager approval. Separate empirical accuracy programme (marteloscope/longitudinal) remains conditional.

## Effort and provider

**P0 difficulty MEDIUM–HIGH:** C# deterministic event model, polygon calculations, UI, offline host and real browser testing; simpler than forestry growth calibration. Best-suited **Sol/Codex High in local T3 Code** with .NET/Node/browser capability, single dedicated worktree and new directory ownership; independent **Claude Opus High or separate Sol High** source/UX review. No permanent discipline ownership claimed; actual worker registers. Do not send to multiple code writers at once.

**P1 difficulty HIGH:** biological source inspection, scientific versioning, heterogeneous future effects; split research/source and software qualification with explicit model outcome/holdout gates.

## Stop conditions

Stop and report BLOCKED if branch ownership conflicts, host capacity/Unity process conflicts, missing suitable isolated worktree, source state divergence from approved spec, model source not verified for P1, anonymous "real" GBIF plot substituted, CCF production save encountered, unreviewed numerical forestry constants proposed, or non-Sitka/slope/rack/Unity game code appears. Do not weaken tests to pass.

## Publication plan

Each substantive implementation and independent review requires its **own GitHub Markdown handoff** with actual evidence and unchanged linked source lineage, claim status and Manager verdict. This document is only a DRAFT plan; no implementation or test passed merely because these steps are written. Dashboard/Drive mirror sync must be reported NOT VERIFIED unless evidenced.
