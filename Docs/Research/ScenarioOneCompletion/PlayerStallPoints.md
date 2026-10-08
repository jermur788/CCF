# Player stall points (Workstream A)

**Status:** audit at `a8596df` [REPO], plus design inference [INF]. Severity is for a beginner with no forestry knowledge.

A *stall point* is any state where the player cannot progress, or does not know why they are not progressing, or receives no signal that they should act.

| ID | Stall | Mechanism (code) | What the player sees | Severity | Fix direction |
|---|---|---|---|---|---|
| **S1** | **Cash trap before the first thinning** | Approval needs cash ≥ cost; harvest cost ≥ €2,500 minimum; timber is the only income (`ApprovePendingWork :1111`). `managed-opening` needs one completed felling | Nothing until Year 100: "Year 100 ended before the continuous-cover objectives were reached." The cash-zero failure (`:2221`) needs exactly €0.00 | **BLOCKER** | Warn in the Work Plan when committing spend would leave cash below the next harvest minimum. Rethink completion (`FailureRecoveryDesign.md`). **Decision required** on whether this is a legitimate failure |
| **S2** | **Objectives not visible where play happens** | HUD shows only "Forest objectives N of 8"; the list exists only in Annual Review › OBJECTIVES | A count without content. The O key opens the *learning* checklist, not the forest objectives | IMPORTANT | One objectives view; HUD lists the next open objective |
| **S3** | **Mandatory broadleaf planting, no reason (main)** | `introduced-beech`, `introduced-sessile-oak` | "Establish planted sessile-oak" (raw ID). Nothing says why | IMPORTANT | P1 explains it (candidate); `ObjectiveRedesign.md` reframes it |
| **S4** | **Year 25 is invisible** | `minimum-year` is hidden among the 8 objectives | Players who complete their actions early do not know they must keep advancing, or why | IMPORTANT | Explain the 25-year horizon at the start ("CCF is a process") |
| **S5** | **Learning "next lesson" is out of step with play** | First unfinished step in list order | After thinning and planting, the HUD still says "Next lesson: Set a waypoint" | IMPORTANT | Stage-based arc (P5) |
| **S6** | **Approve-all approves unintended work** | `ApprovePendingWork` approves every valid pending order | A forgotten planting or clearance order is approved with the thinning | POLISH→IMPORTANT | Confirmation summary before approval |
| **S7** | **Thinning always loses money; reason unclear** | €2,500 minimum on a 0.16 ha property | "Small-job minimum −€2,4xx" every time | IMPORTANT (comprehension) | `EconomyLearningSequence.md` |
| **S8** | **Second Annual Review onwards is skippable** | `NeedsAnnualReview` checks only `annualReviewSeen` | Review opens and is closed unread; the player loses track of change | IMPORTANT | Annual Review v2 "places to inspect" (P4) |
| **S9** | **Clearance offered everywhere** | Preview on every ground aim (`ForestPlayer.cs:438`) | A permanent yellow square and "Clear competing vegetation" | IMPORTANT | Preview only after U (decision #9 from the pedagogy branch) |
| **S10** | **Planted juveniles die, cause unclear** | Browse/light/(M2: vegetation) losses aggregate into review lines | "Planted trees: 3 growing, 0 joined, 9 lost" | IMPORTANT | Per-cause loss line (exists as runtime ledger under M2) |
| **S11** | **No reason to come back** | No later-intervention trigger | The player advances 24 years with nothing to do | IMPORTANT | `SecondInterventionDesign.md` |
| **S12** | **Pruning has no visible payoff** | No premium; no quality state | Money spent, a number on the card | POLISH | Explain honestly; optional quality flag later |
| **S13** | **Wind/competition labels cannot discriminate** | Saturated labels | Every tree "high", almost every tree "crowded" | IMPORTANT | Storm stability bands (storm packet); competitor list (P2) |
| **S14** | **Help lost on a shared machine** | Device-scoped PlayerPrefs | A second learner never sees introductions | IMPORTANT for playtests | Reset learning preferences per test profile (playtest protocol), or move to per-forest progress |
| **S15** | **After completion (Year 25) the goal disappears** | Outcome Completed; play continues to Year 100 | "objectives met"; no new goal | POLISH | Optional "continue managing" framing; Century Review explained |
| **S16** | **No shelters for natural regeneration** | Shelters attach only to exact planting | A player who wants to protect natural Sitka/regen has no tool | POLISH (accepted D-044 deferral) | Explain in copy |

## Most important for a first beginner playtest

S1, S2, S3, S4, S7, S9, S14. All except S1 can be fixed by copy/UI packets without touching simulation. S1 needs a product decision (`FailureRecoveryDesign.md`). Until then, the playtest protocol must prevent or detect it (`BeginnerPlaytestProtocol.md` §3).
