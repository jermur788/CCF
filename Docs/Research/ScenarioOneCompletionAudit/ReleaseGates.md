# Minimum Scenario One release definition

Three gates with different purposes. They are reached in the order **B → C → A**: real beginners should see the game before the second half of the loop is polished, and foresters should see the whole loop before Scenario One is called complete.

Item numbers refer to `RemainingWorkMatrix.md`.

## B. BEGINNER-TEST READY (earliest gate)

*A person with no forestry knowledge can install a private build, play the first management cycle, and any failure we observe is about understanding, not broken flow, false statements, or a hidden trap.*

Must be true:

| # | Condition | Check |
|---|---|---|
| B1 | P3 integrated on main | P3 gates on the merge result; anchors unchanged |
| B2 | MenuTutorial Help-Escape failure classified and resolved (harness or production) | Interactive MenuTutorial PASS ×2 on the tested SHA |
| B3 | P2 and P3 human smokes done; findings fixed or recorded as test questions | Signed-off manual lists |
| B4 | S1-A integrated: CCF defined; positive selection framed; **no false clearance claim for the active model**; species display names; forest objectives visible with the Year-25 horizon and the broadleaf reason; pre-purchase cash warning | Teaching-copy gate (batch + rendered), string sweeps, clearance gate, MenuTutorial |
| B5 | S1-B integrated: reproducible standalone build with the build SHA on screen; visible save/load, quit, "Start a new forest"; per-tester help reset; Player profiling recorded; Asset Store licence checked | Build record; save → quit → relaunch → load round trip; profiling numbers |
| B6 | Full regression on the build SHA from **tracked** harness sources: all anchors unchanged | Regression results JSON |
| B7 | One internal pilot (non-forester) plays 45–60 minutes through the protocol without facilitator help beyond controls | Pilot notes |

Not required for B (deliberately): Annual Review v2, diary, place history, second-look trigger, completion redesign, stage engine, plan comparison, storms, asset packets, recovery route, per-forest progress.

**Test scope at B:** Years 0 to roughly 10. Questions: can testers explain why they kept a tree, what they removed and left, why the thinning lost money, and what they would look at after a year? Where do they stall? Do the "storms off" wind labels or the bramble/bracken visuals confuse them? (Observe, do not pre-fix.)

## C. FORESTER-REVIEW READY

*A forester or CCF practitioner can play a full first-to-second cycle and judge whether anything is misleading.*

Must be true (in addition to B):

| # | Condition | Check |
|---|---|---|
| C1 | S1-C integrated: Annual Review v2 with PLACES TO INSPECT, self-thinning named, Forest Diary, cell history | S1-C gates |
| C2 | S1-D integrated: second-look trigger; completion means two separated cycles with guardrails; no hard fail; completion view without a score | S1-D gates; new completion anchor authorised and recorded |
| C3 | Mixed-species SDI mortality diagnosed; fixed by the ecology owner **or** listed as a known limitation in the reviewer sheet | Diagnostic record; ecology decision |
| C4 | Beginner test 1 copy/UI fixes integrated (P8 round 1) | P8 handoff |
| C5 | A one-page reviewer sheet: what is modelled, what is [C] calibration, what is absent (no harvest damage, no fencing, no quality premium, storms off, Hegyi competition, 0.16 ha with €2,500 minimum) | Docs |

Not required for C: stage engine, plan comparison, recovery route, art polish, storms.

## A. FEATURE COMPLETE

*Scenario One is a complete teaching foundation: the player decides, waits, observes, reassesses and acts again at least twice; history and consequences are readable; completion describes demonstrated management; nothing lies or traps.*

Must be true (in addition to B and C):

| # | Condition | Check |
|---|---|---|
| A1 | Forester review: no unresolved **MISLEADING** item | Review summary |
| A2 | Beginner test 2 (after S1-C/S1-D) shows the second-look loop is understood by most testers; round-2 fixes integrated | Test report; P8 round 2 |
| A3 | Decisions recorded: E1 (no single score), N1/N2/L3 (completion), O1 (no hard fail), O2 (recovery route or explicitly none), sapling price authority, J3 | Decision Log |
| A4 | Canonical docs updated (Overview, Milestone, Decision Log) and context regenerated | Generator manifest |
| A5 | Full regression green on main; Reference Future v1 and lifecycle anchors unchanged; the new completion anchor recorded; lowest cash and completion year logged under the current stack | Regression results |
| A6 | Standalone Player profile re-run on the final SHA | Profiling record |

**Explicitly not part of feature complete** (see `ImplementationSequence.md` §4): S1–S12 stage engine, per-forest progress save record, "no work needed" event, plan comparison, training stand, assistance tiers, storms in Scenario One, sample plots, fencing, quality state/pruning premium, broadleaf market, a cash recovery mechanic unless O2 chooses one, asset polish beyond what testers demonstrably fail on.

## Anti-expansion rule

After this definition is accepted, a new item enters Scenario One only if **(a)** a beginner or forester finding shows players misunderstand or are misled without it, or **(b)** the Decision Log records it. Otherwise it is queued for Scenario Two or later. "It would be nicer" is not a reason.
