# Plan — Scenario One Research-Informed Management Learning V1

**Status:** DRAFT (not production-authorised)  
**Parent:** [spec.md](spec.md)  
**Work checklist:** [tasks.md](tasks.md) and [checklists/requirements.md](checklists/requirements.md)  
**Prepared:** 2026-11-12 by Overall Manager

## 1. Implementation philosophy

**Do not make the six-paper research a second forestry simulation.** Work within existing forestry/tree/Work Plan/Annual Review architecture. Introduce no new objective thresholds, unvalidated ecological parameters, history authority, or numeric player score.

**Research uncertainty should not block already well-specified UI truthfulness.** Separate:
- **Implementable from current sources:** explain Share limits; avoid a single correct competitor; clearer woodland young-growth terms; accurate distinction between natural and planted origins **when recorded**; simple default wind-model limitation statement; reuse existing history layers.
- **Implementable after one additional product decision:** durable optional player intention for a retained competitor, recorded through append-only history, with save schema review.
- **Require fresh public-source research before final wording:** specific Irish/UK wind/stability practice claims, non-obvious habitat/shelter language, stage-specific Irish forestry terminology.
- **Require authentic fixture data before player UI:** spatial species mingling and diameter-difference signals; Hegyi versus relascope-based alternatives.
- **No credible implementation basis:** imported Finnish ingrowth/rates, Swiss crowns, calibrated wind penalties from P1, Bayesian predictions, biodiversity indices used as score.

## 2. Preliminary integration surface inventory (verify at start)

| Subsystem | Current authority / inspected baseline | Planned touch |
|---|---|---|
| Crop Tree / marking | \`ForestTree\` mark enum, existing Scenario One teaching sample; approved MFM | Explanatory text/read-only context; no mark semantics change |
| Share competition | Existing authoritative \`CompetitionShare\` and 8 m Hegyi area | Display existing value; never recalculate |
| Objective evaluation | \`ScenarioOneObjectives.Evaluate\`, \`ScenarioOneDefinition\` | Wording only; keep objective IDs, numeric thresholds and achieved bools |
| Ecological review data | \`ScenarioOneManager.RecordEcologicalSnapshot\`, \`ScenarioEcologicalSnapshot\` | Existing current and year-stamped records only; no new ecology |
| Planting / regeneration | Exact planted juveniles and abstract natural-origin cohorts | Language and species/origin presentation only |
| S1-C Annual Review | Existing four-layer historical-fact/provenance contract | Bounded review callouts using existing history |
| S1-D repeated management | Advisory later cycles, first teaching-completion latch | Read-only history integration, no new progression gate |
| MFM V1 | Crop Tree candidates, proposal/approval provenance, Work Plan | Reuse explanations; retention intention does not auto-exclude or rewrite |
| \`ScenarioManagementEvent\` and saved Scenario One | Existing event identity/chronology | **Only if persistence approved:** additive retention-intent event type/fields, next schema |
| Rack V2, D-072 UI | Current 3D/map layouts and image/cue qualification | **No change**; protect focus/1280 label readability |
| Storm Model 0 | Default no simulated storm damage | Factual limitation only; no activation |

Prior published UI names and save versions are dated. **Local implementer must inspect exact current integration and source code** before writing. GitHub-only review proves neither local cleanliness nor a currently running Unity process.

## 3. Branching, ownership and sequencing

**NOW — Manager research/spec preparation:** Browser-only tasks are independently writable on their **own** remote GitHub documentation branches: marking/wind #387 and young-growth #388. This specification is on \`docs/scenario-one-research-informed-spec-20261112-v1\`; no local worktree is allocated. Each is a sole primary writer for its own documents. No new Unity lock is claimed.

**CURRENT DEMO — continue already-approved delivery:** Complete existing Rack V2 + D-072 presentation, species parity, S1-C, S1-D, MFM and current personal playtest/qualification. No research-derived production change to current candidate without a specific user scope revision. Current-year user playtest and Windows gates remain unchanged. Newly completed local cleanup #385 does not grant any Source/UI/Unity ownership for this work.

**POST-DEMO / once explicitly approved:** Dispatch separable work packets in this order; do not parallelise overlapping UI/history source writes. Each gets an issue claim, single writable branch/worktree, start HEAD, owner/lease and bounded source paths. Suggested providers below are recommendations, not assumed capacities.

### Wave A — Small truthful explanations and copy (short critical path)

**A1: source/UI inventory and UX copy contract** — one browser/GitHub docs-only task, re-read all live objective/Help/review/inspection labels, actual fit constraints, MFM reason vocabulary and S1-C source fields; file evidence with string locations. Use browser research #387/#388 for source-dependent wording. May run during source freeze only as read-only research, without local Unity work.

**A2: competitor/Crop Tree explanation** — local Sol/Codex source/Unity UI lane, difficulty **Medium**: bind existing model Share and state to concise reason labels. No Grade/Share logic edits.

**A3: young-tree language and regeneration light-condition explanation** — same UI owner **sequentially** after A2 (or a separate no-overlap UI file set after explicit Manager signoff), difficulty **Low–Medium**: replace grid jargon across objective, help, review and Century Review; do not change calculation.

**A4: wind limitation note** — Manager writes external-test limitation now; post-demo UI owner adds a short factual default-model note once approved. Full "release vs windfirmness" interpretation needs returned #387 evidence; it may be omitted if evidence is inconclusive.

**Wave A exit:** Acceptance G3 for copy fidelity, model unchanged, check at 1280×720 and 1920×1080. Combine source changes in one bounded UI feature candidate; avoid a new full integrated test campaign for each text string.

### Wave B — Optional recorded competitor-retention decision (new feature)

**B0: UX comparison / choice** — compare two concrete tree-neighbour cases under *existing Share and 30% lesson* with (i) no annotation and (ii) optional reason. Ask whether later recall/reassessment changes. No new save, no Unity needed for initial mock-up. One product verdict: reject, session-only prototype, or durable intent.

**B1: persistence contract review if durable feature chosen** — inspect exactly current \`ScenarioManagementEvent\` enum/fields, S1-C history filters, WorkResolved aggregation, S1-D rules, current save version/migration and MFM conflicts. Confirm no parallel history store, reserved enum-value collision, event-ID breakage. Amend/spec-approve if actual source does not support this additive approach.

**B2: event model/serialization** — local source writer for Scenario One events/save only, difficulty **High** because persistent state and cross-system provenance. Type-specific records for reason add/revise/clear, stable target and neighbour IDs, year and code, append-only; no re-rating, no work order creation. Old saves lack events; no backfilled history. Add dedicated tiny deterministic fixture harness, not an expensive full-year regression.

**B3: inspect-tree optional action** — local UI writer after B2 event schema is source-qualified, difficulty **Medium**: reason prompt within existing inspection panel, pairing tree identities, idempotent clicks, clear/edit controls; no new hotkey/physical colour.

**B4: history / later-management explanation** — integrate S1-C L1 source fact plus L2 current status plus any evidence-complete L3 derived data. The later management loop may refer to the previous intent but does not make it mandatory.

**B5: MFM interaction proof** — verify annotation alone does not create Fell mark, protect a tree, hide a proposal, change proposal Share, approve Work Plan or alter fit logic. If a *display-only* "recorded intention" alert is warranted, handle as an optional separate follow-up, not an implied automatic veto.

**Wave B exit:** source/performance clean on dedicated feature candidate; Tier-2 save/history/old-save + coexistence tests; independent review for persistent/event changes; Manager APPROVED before integration.

### Wave C — optional offline research and descriptive metrics (parallel with docs, not release critical)

**C1: boundary-aware structural metric test** — separate code/data analysis, not Unity, no game file modifications; authentic saved world/years, stable sorting, 3-neighbour inclusion, edge treatment, living trees only, timing of broadleaf juvenile-to-tree transition. Result may be **NO USEFUL SIGNAL**; that is a successful evidence outcome.

**C2: Hegyi vs Finnish RBA comparison** — offline method comparison with equal input and documented measurement assumptions; no replacement of production coefficients.

**C3: question of a player structure panel** — only after C1 shows meaningful changes and product owner wants it. New separate spec; not in the current buildable core.

## 4. Strong default design choices to minimise rework

1. **Keep lesson credit unchanged** in V1; do not equate an optional retention reason to a release action.
2. **Persist only authored decisions that genuinely need later recall**, using one existing event timeline rather than a second state/history store. No hidden simulation state.
3. **Source-derived versus player-authored** must be visibly distinct: "You recorded..." vs "Current model shows..." vs "For consideration...".
4. **No forced annotations**, because MFM already requires many individual candidate inspections and extra clicks could be unacceptable.
5. **No wind-benefit inference** in any reason; player expresses an intention, the default simulation lacks wind consequence.
6. **No new biological scores/thresholds** based on six same-network papers; source support is methods/transferred evidence.

## 5. Source-update boundaries and stop conditions

- **STOP / Manager required** if a task proposes any of: change to 30% Share or 40% cap; an imported Irish coefficient; change in annual ecology or cash; a new persistence model replacing S1-C; new objective pass predicate; MFM override; frozen-reference change; unqualified Year-100 projection; numerical wind risk; tree-quality auto-selection; or a second user navigation environment.
- **STOP / local safety** if branch/HEAD or live source ownership differs from packet, if another UI writer has files, or Unity machine-wide heavy operations are leased by another owner. Do not kill processes, clean/rebase unrelated local work or modify protected personal player saves.
- **STOP / source honesty** if a history record is absent and a UI would need to guess its year/cause; show "not recorded".
- **STOP / product choice** if durable retention event would need added policy about overriding Work Plan/MFM. Such a policy is **not** in this spec.

## 6. Verification economy

**Tier 1 (FAST):** Docs/source inventory and copy comparisons, Hegyi value remains exact and read-only, event record roundtrip logic, no false objective/lesson state change, old-save missing-fields treated conservatively. Apply as small focused checks.

**Tier 2 (FEATURE QUALIFICATION):** Validate one complete manual sample judgement, two alternate Share-compliant removal sets, another path where neighbour retained intentionally (no credit shortcut), a complete save/load-and-later-year intent history, MFM candidate/proposal + Work Plan separation, 1280 and 1920 readable UI. Verify specific protected compatibility fixtures in a bounded manner. Distinguish source-only review from actual Editor/player behaviour.

**Tier 3 (FULL INTEGRATED QUALIFICATION):** Once per approved milestone/release candidate after feature integration, not after each copy change or an offline structure calculation. Preserve the qualified exact candidate, inspect results and respect user authorisation before any Windows build.

## 7. Review and publication plan

- A **DRAFT** does not authorise code. Manager accepts research outcomes into design only after checking cited primary documents, not merely extracting conclusions.
- Product owner approves desired feature scope, retention event persistence, temporal placement and final wording choice.
- Publish SPEC APPROVED before local source implementation, then create bounded GitHub task packet and issue claim per substantive dispatch.
- Every source/research result gets Markdown handoff published to GitHub and a clear Manager APPROVED / NEEDS CHANGES / REJECTED verdict; claims record [RUNNING] and [HANDED-OFF].
- If accepted canonical context changes merge to main, regenerate one-context-SHA instructions/hashes/manifest, read back permanent Drive mirrors and refresh platform attachments; currently no sync is claimed.

## 8. Proposed ownership after approval

| Work | Primary writer | Lane | Best provider | Difficulty |
|---|---|---|---|---|
| Browser source verification #387 | Research worker A | GitHub remote docs branch \`docs/research-ccf-marking-wind-20261112-v1\`; no worktree | ChatGPT Deep Research | High: primary Irish/UK evidence |
| Browser terminology #388 | Research worker B | GitHub remote docs branch \`docs/research-ccf-young-growth-language-20261112-v1\`; no worktree | ChatGPT Deep Research | Medium–High: semantic/scientific |
| Spec/decision review | Overall Manager then independent reviewer | current spec branch; no local worktree | GPT-6 High / independent capable reviewer | Medium: cross-system acceptance |
| Wave A UI/copy | One local UI/source owner | **TBD after locks**: dedicated branch/worktree with explicit allowed files | Sol/Codex | Medium: focus/overlap preservation |
| Wave B event persistence | One local save/history owner | **TBD after locks**: dedicated source branch/worktree | Sol/Codex / alternative proven Unity-capable provider | High: save and history |
| Wave B UI/historical integration | Same owner sequentially or separate disjoint file/API owner | **TBD after event API** | Sol/Codex | Medium–High: UX and S1-C |
| Wave C offline metrics | Separate research/code analyst | **TBD**, no Unity unless a later approved task requires it | Data/analysis capable provider | Medium: boundary and fixture fidelity |

Do not allocate or name an invented local worktree/Unity lease now. The dispatch packet will give an exact actual lane when a source implementation phase is approved.

## 9. Next decision

The **first unblocked actions today** are dispatch #387 and #388 as browser research and review this DRAFT in a separate docs/spec lane. Their results can refine source-dependent copy. **No game work has been started and no user playtest or external demo is represented as passed.**
