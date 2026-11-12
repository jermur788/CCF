# Acceptance / safety checklist — management cadence and two advance options V1

**DRAFT / UNTESTED:** D-074 and D-075 product direction accepted; implementation not yet authorised or verified.

## Decision and source gates
- [ ] G1 Audit latest actual `AdvanceYear` and UI gate code on newest post-Fable UI lineage; reconcile changes that followed initial source read.
- [ ] G2 Identify and separate qualifying major stand interventions from access first cut, planting, pruning, protection, isolated maintenance and idle/no-work years; no fake CCF prescriptions.
- [ ] G3 Approve read-only target-year algorithm and maximum ticks, with exact display label and reasons; no universal four/five-year “right” interval claimed.
- [ ] G4 Set old-save/Reference Future/Legacy40 behavior and whether new objective predicate is ever allowed. No save/achievement rewrite without separate consent.
- [ ] G5 Final four-part feature spec formally APPROVED and local writer/branch/Unity lock claimed before code.

## Behavior and tests
- [ ] AC-1 Player can always use existing Advance 1 Year (when current authoritative state permits); nothing forces a multi-year skip.
- [ ] AC-2 "Skip to Next Management Year" has visible target year and elapsed years; disabled/explicit reason if no safe future management review point.
- [ ] AC-3 Each skipped year performs existing *exact* annual simulation order; annual reports, events, snapshots, advances, objective evaluations are not compressed or omitted.
- [ ] AC-4 Fixed seed and identical work/decisions manual-vs-batch have same intermediate annual records and final world hash, with only explicitly permitted UI acknowledgment differences.
- [ ] AC-5 No automatic plan approval, Fell mark, MFM treatment, harvesting, planting, pruning or shelter work. Pending/approved orders stop/bypass skip safely.
- [ ] AC-6 Rack/teaching/new objective action and first mandatory Annual Review not bypassed. No fabricated review acknowledgment events, no silently reset historical provenance.
- [ ] AC-7 Stops at earliest genuine need for interaction, model error, terminal/Year25/Year100 boundary or cap; reports partial elapsed years truthfully and does not pretend rolled back work.
- [ ] AC-8 Quiet skipped years still appear in earlier-year review; destination has meaningful aggregated elapsed-year and next-decision explanation without causal overclaim.
- [ ] AC-9 Necessary earlier planting/protection/maintenance work remains valid in one-year mode; early work not punished by an absolute interval lock.
- [ ] AC-10 Final actual 1280×720 and 1920×1080 UI visible and usable, native keyboard/mouse; no repeated-click automation denial or freezing.
- [ ] AC-11 Saves/loads and frozen Reference Future v1, Legacy40, existing world-hash identity and current UI feature/three-species objective behavior pass on exact candidate.
- [ ] AC-12 Independent review when appropriate; proportionate verification; Manager integration verdict; user's playtest/Windows build gate respected.

**No AC is PASS from source reading or approval alone.** Design changes to completion timing require separate authentic historical-save compatibility evidence.
