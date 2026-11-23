# MULTI Stage 4 — Independent review, repair and re-review

**Final review status: `APPROVED` — focused G3 Stage 4 repair.** Not integration, not owner acceptance.

| Item | Value |
|---|---|
| Feature | Adult Sitka parity: woodland DBH growth, per-rack density mortality, biological death and fallen deadwood, pruning |
| Base | `4e60c8ab806df56441497b682ccf783badc6e0ea` |
| First candidate | `592dc7bdc3abe6e3f3b4a16505c83c88db3906ed` |
| Repaired candidate | `500a50aea4bd049f231f05d73828fabdc0a7a85c` |
| Branch | `feat/multi-stage4-adult-parity-20261123-v1` |
| Authority | APPROVED Stage 4 quartet + D-095 at `115817c7` |
| Implementer | Claude Opus 5.5, medium (one writer, D-091 lane) |
| Reviewer | GPT-6.1 Sol, medium — different provider/context, read-only |

## 1. First review — NEEDS CHANGES (G3)

No blocking finding and no confirmed production defect. Source conformance was found correct for growth, mortality, death/deadwood and pruning; zero drift, versions, contracts, cross-rack isolation and determinism passed.

| # | Severity | Finding |
|---|---|---|
| 1 | Major | Boundary deaths never reached the deadwood integration assertions: the boundary test used `AnnualRules` directly, and the session test stayed below RD 1.0. |
| 2 | Major | Mortality oracles partly trusted reported outputs (`PreGrowthCi`, `report.Deaths`) instead of deriving expectations independently. |
| 3 | Minor | P-04 "Execute revalidates and refuses all" overstated what it tested. |

UI smoke J01/J99: 107/2 identical on base and candidate; pre-existing relative to Stage 4, environmental cause not established, page witness incomplete.

## 2. Repair — `500a50ae` (tests + handoff only; no `src` change)

- **Boundary deadwood, end to end.** Synthetic two-rack Stage 4 session: Year 1 Area A RD **4.1653**, 58 hazard + **18 boundary** deaths; Years 2–3 add further hazard deaths. Three-year totals **63 hazard + 18 boundary = 81 victims / 81 records**, IDs `DW0001…DW0081`, none reused. Every victim asserted for one record, all 11 fields, float32 volume, biological death (not stump), no Fell/Crop/work event, absence from next-year woodland CI, and cover/light equal to an evaluation with victims deleted (and unequal when restored).
- **Independent mortality oracle.** `ExpectMortality` derives rack/woodland CI, grown dimensions, RD, S, mean V, P, p, hazard victims (prescribed `ADULT-MORT-` roll) and boundary victims from input trees only. M-02: RD 0.6371, 0 deaths. M-03: RD 2.1438 → 46 hazard + 8 boundary, final 0.9920. Report matches bit for bit.
- **P-04** renamed accurately; Execute-time guard revalidation/atomic refusal is disclosed as untested.

## 3. Focused re-review — APPROVED

**APPROVED — G3, focused Stage 4 repair only. Reviewed HEAD `500a50ae`.**

- Finding 1 **FIXED** — boundary records are independently expected; omitting them fails the victim-count/identity assertions.
- Finding 2 **FIXED** — no report field builds the expectation; sharing `NextHeight` and `SimulationRandom.Roll` is acceptable (frozen height law and prescribed RNG).
- Finding 3 **FIXED** — claim matches the test; revalidation is a disclosed carry-forward, with source inspection showing revalidation before mutation.
- No new blocking or major issue. Minor: the fixture comment "whatever the rolls" is mathematically too strong; the fixed-seed result is verified and asserted.
- Reviewer ran the suite independently: **180/180 passed**, solution 0/0, four fingerprints matched, P0 pin unchanged, diff tests/handoff only.

## 4. Orchestrator verification

| Check | Result |
|---|---|
| Core suite | **180/180 passed** |
| Builds | Core / Cli / Ui 0/0 |
| Hashes | `7e3a2a86…`, `a692a6d5…`, `814e8165…`, `8a926776…` unchanged |
| P0 pin | `9a99f781…` |
| `/multi` | page 200, API 200, 0 exceptions |
| Scope | no Ui/Assets/fixtures/saves/`main` change |

## 5. Honest state

- **IMPLEMENTED:** yes, on the task branch.
- **NUMERICALLY QUALIFIED:** yes — author, reviewer and orchestrator agree on offline evidence.
- **REVIEWED:** yes — independent different-provider review and focused re-review.
- **INTEGRATED:** no.
- **ACCEPTED:** no.

Carry-forward: Execute-time Prune guard revalidation/atomic refusal has source but no test; J01/J99 UI smoke remains pre-existing/unexplained; no Unity or browser verification; the "whatever the rolls" comment overstates its fixture.

## 6. Next

Owner/Manager integration decision. A UI surface for Stage 4 and a test for Execute-time revalidation remain separate approved slices.
