# PLAYER-TEST-OVERNIGHT-01 — Orchestration Evidence Ledger — 2026-10-30

This is the append-oriented evidence source for the final `OVERNIGHT-RESULT-20261030.md` Orchestration Decision Appendix. Record events as they occur. Do not reconstruct unknown quota/capacity values later.

## Evidence vocabulary

- **OBSERVED FACT:** directly supported by user/provider/repository/runtime/test evidence.
- **ORCHESTRATOR CLAIM:** what the orchestrator believed or why it chose a route.
- **RETROSPECTIVE INFERENCE:** conclusion drawn later from outcomes.

Never relabel an orchestrator claim as an observed fact.

## Starting policy record

**Timestamp:** 2026-10-30T00:10Z approximate handoff-preparation time.

- **OBSERVED FACT — user routing instruction:** DeepSeek is to be the overnight orchestrator and default provider for every subtask it can reasonably handle.
- **OBSERVED FACT — user routing instruction:** the two Claude subscriptions are reserved for harder work; default Claude model is Sonnet 5.5 Medium, with escalation to Opus at Low/Medium effort when justified.
- **OBSERVED FACT — user capacity report:** a Claude subscription has recently been repeatedly booted from login. Exact affected pool, remaining quota and reset time were not supplied. **Capacity: UNKNOWN. Source: user.**
- **OBSERVED FACT — user routing instruction:** the orchestrator must track five-hour subscription availability, switch providers when a slot is unavailable/exhausted, and reuse a slot when the provider makes it available again. No quota/authentication bypass is authorised.
- **OBSERVED FACT — user workflow instruction:** all work that can be done in browser must be separated from local Code/Unity work and performed in the browser lane.
- **OBSERVED FACT — user reporting instruction:** final handoff must include the full Orchestration Decision Appendix covering policy, material routing decisions, capacity events, worker outcomes, retrospective assessment, evidence classification and mandatory final summary.
- **OBSERVED FACT — repository:** dedicated browser lane claim #162 exists; local C#/Unity umbrella is #106; parent orchestrator is #91.
- **OBSERVED FACT — repository:** overnight continuation handoff is `Docs/Handoffs/PlayerTestReadiness-Overnight-01/OVERNIGHT-CONTINUATION-20261030.md` published at `4112718310cc842d9ba2d677bc55c4cb450e8bcb`.

## Starting provider ledger

| Provider/pool | State | Capacity evidence | Reset/window | Source | Current work |
|---|---|---|---|---|---|
| DeepSeek | UNKNOWN until orchestrator starts | UNKNOWN | UNKNOWN | none yet | overnight orchestrator intended |
| Claude A | UNKNOWN; recent Claude login instability reported | UNKNOWN | UNKNOWN | user | none assigned by this handoff |
| Claude B | UNKNOWN; recent Claude login instability may or may not affect this slot | UNKNOWN | UNKNOWN | user | none assigned by this handoff |

The orchestrator must replace `UNKNOWN` only with directly observed provider/T3/user evidence.

## Starting task/routing facts

- **#143 S1-D T-04P:** OpenAI gpt-6.1-sol is the implementation provider. Pushed review/runtime candidate `b9bf11aaf8f6fd7ea43e33e39258560408b88b12`; latest durable worker record says Unity chain running. Independent review pending under #161. Browser-first review is preferred if final pushed evidence is sufficient.
- **#151 S1-D T-05:** local offline implementation ready from `cfdc93bf3943125d21570bc5749a9338f52d1cce`; final qualification needs serialized Unity. Browser review/prep should be split out once a pushed candidate exists.
- **#156 MFM-08:** local offline implementation ready from Manager-repinned `cfdc93bf3943125d21570bc5749a9338f52d1cce`; final Tier-2 qualification needs serialized Unity. Browser review/prep should be split out once a pushed candidate exists.
- **#124:** blocked on accepted/integrated #143.
- **#126:** blocked on accepted S1-D persistence/curriculum lineage and must be browser-repinned to the later actual v24 integration before local work.
- **#128:** blocked on exact handed-off #126 candidate.

## Event log

Append entries below in chronological order using this template.

### EVENT — <timestamp>

- **Work unit:**
- **Evidence class:** OBSERVED FACT / ORCHESTRATOR CLAIM / RETROSPECTIVE INFERENCE
- **Previous provider/model:**
- **Selected provider/model:**
- **Alternatives considered:**
- **Reason / routing rule:**
- **Capacity evidence:**
- **Capacity source:** provider / T3 / user / inferred / unknown
- **Reported reset/window:**
- **Previous failure/outcome influencing decision:**
- **Branch/worktree or browser lane:**
- **Result/evidence:**
- **Handoff/commit/claim reference:**
- **Accepted / rejected / unresolved:**
- **Failure classification if any:**
- **Next action:**
