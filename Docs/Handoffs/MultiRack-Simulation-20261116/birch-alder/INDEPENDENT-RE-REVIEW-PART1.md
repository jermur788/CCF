# Downy Birch and Common Alder V1 — Part 1 Delta Re-Review (Repair Round 1)

## Review identity

| Field | Value |
| --- | --- |
| Reviewer | GPT-6.2 Sol, Codex pool, `reasoningEffort: high` |
| Reviewer thread | Fresh delegated thread, no shared context with the implementer or the first reviewer |
| Implementer | OpenCode / DeepSeek V4.1 Flash (`opencode-go`), sole lane writer under the owner packet |
| Review type | Focused delta re-review, Tier 1 Minimum Necessary Testing |
| Date | 2026-11-27 |
| New HEAD | `8d097f3414533e46a72af76c861ac83dabdf0754` |
| Previous HEAD (rejected) | `73ed9fe38ce473068803443e841915b8495ad4dc` |
| Base | `24871551ae906b6911b48bb17c252c023bdea502` |
| Context commit | `bdeb6a324acaf4129c10fb0846faac8b64974360` |
| Verdict | **NEEDS CHANGES** — technical conformance **PASS**; independent sign-off blocked on review eligibility |

## Verbatim integration decision

```text
INTEGRATION DECISION
Status: NEEDS CHANGES
Gate: G2 / G3 technical evidence passes, independent sign-off pending. G4 applies at integration and remains open.
Reviewed branch: feat/multi-2d-birch-alder-engine-v1
Reviewed HEAD: 8d097f3414533e46a72af76c861ac83dabdf0754
Context commit: bdeb6a324acaf4129c10fb0846faac8b64974360
SPEC ID / STATUS: SPEC-multi-2d-birch-alder-v1 / APPROVED-IMPLEMENTING
Reasons:
- AC-4 and all three minor repair findings are closed. Part 1 conformance passes on the reviewed HEAD.
- The pinned gate passed 40/40 twice with byte-identical output, zero build warnings and errors, and a clean file fence.
- The implementing thread dispatched this review as its child, which the locked workflow excludes from independent sign-off.
Required fixes / remaining gates:
- Obtain standalone independent G2/G3 sign-off at this HEAD and have the Manager resolve the recorded implementation routing discrepancy before G4.
- The moisture-summary correction and broader per-stem digest coverage are non-blocking follow-ups. AC-7 remains queued for Part 2.
Integration target: origin/integration/2d-sim (NOT merged by this review; single integrator only)
Integrated HEAD: not applicable
Post-integration verification: not applicable
SPEC CONFORMANCE: PASS
```

## Verification receipts

From `Tools/MultiRackSitkaP1Lab`, pinned SDK `8.0.133`:

```text
dotnet restore ...            All projects are up-to-date for restore. Exit 0.
dotnet build ... --no-restore Build succeeded. 0 Warning(s), 0 Error(s). Exit 0.
dotnet run ... (twice)        Both exited 0.
  SUITE CommonAlder: 11/11 passed, 0 failed
  SUITE Determinism: 1/1 passed, 0 failed
  SUITE DownyBirch: 9/9 passed, 0 failed
  SUITE SiteAndZone: 12/12 passed, 0 failed
  SUITE Timber: 7/7 passed, 0 failed
  TOTAL: 40/40 passed, 0 failed
  Run 1 DIGEST canonical-sha256 ca2e61bd3432329352cdd63a5c2c2b97bc7ba1824f284aee7728e627f1740f23
  Run 2 DIGEST canonical-sha256 ca2e61bd3432329352cdd63a5c2c2b97bc7ba1824f284aee7728e627f1740f23
  cmp: exit 0, byte-identical full output
```

Fence: `git diff --name-status 24871551..8d097f34` returned **18 additions**, all inside the four permitted prefixes. Targeted name-status diffs were empty for the three foundation files and for `Assets/`, `Packages/`, `ProjectSettings/`, `src/Core/MultiArea/`, `src/Core/RngSalts.cs`, `src/Ui/`, `tests/Core.Tests/` and `build/`. `git status --porcelain` was empty and HEAD unchanged before and after the runs. The handoff's 40-check count and digest match both runs.

**NOT TESTED** by the packet's limits: Unity, browser/Playwright, `dotnet test`, `2d-gate.sh`, `selftest.sh`. AC-7 (Part 2) remains queued.

## Delta closure

| Finding | Status | Evidence |
| --- | --- | --- |
| MAJOR AC-4 — alder recruited on `NeutralLegacy` | **CLOSED** | Alder must pass the exact allow-list of `WetMovingAlluvial`, `FreshMoistMineral`, `SeasonallyWetAerated`; `NeutralLegacy` yields `MoistureUnknown`; dry and bog keep their specific refusals; the site is attached before the gates so the bog caution survives an earlier refusal. `ALDER-REGEN-01d` reaches the moisture gate with every other gate satisfied and asserts refusal, plus a birch contrast using a year that is good on birch's own calendar. Both table responses are asserted bit-exact (1.0, 1.0). |
| MINOR 1 — no age-50 zero assertion | **CLOSED** | Risk returns literal `0.0` through age 50; inspection short-circuits on zero; `ROT-01` compares IEEE-754 bits at 50; `ROT-02` checks 1,000 identities at both 49 and 50. |
| MINOR 2 — `default(...)` read as `Eligible` | **CLOSED** | `NotEvaluated` owns zero; `ALDER-REGEN-01e` proves the default result cannot regenerate; seed-tree age documented per requested species; no ordinal or numeric use of the status exists in the lab. |
| MINOR 3 — misleading provenance | **CLOSED** | Comment, FR-5 and provenance row all identify EUR 42 as a rounded provisional figure with EUR 42.50 as the arithmetic midpoint; price unchanged at `42m`; sapling labels retained under a corrected heading. |
| OBSERVATION — digest blind spots | **CLOSED** | The fixture now records both moisture classes for regeneration and ordered per-stem rot flags, which justifies the deliberate re-pin. |

## New findings

1. **MAJOR (process, not code) — review eligibility.** The dispatching thread edited and committed `57aa6893` and then launched this review as its T3 child. The locked workflow excludes a child of the implementing worker from independent sign-off. Smallest fix: the Manager or coordinator commissions a standalone reviewer outside the implementing thread's child tree at the same HEAD. Raised by the reviewer against `Docs/Project/AgentWorkflow.md` §Roles / §Review boundary and consistent with it.
   - **Manager note on the routing sub-point.** The reviewer also observed that the handoff names DeepSeek V4.1 Flash as implementation writer. That assignment came from the owner's task packet, and `AgentWorkflow.md` states that no model family permanently owns code and that routing labels are capability handles. This is recorded for the Manager to confirm or re-route before G4; it is not a lane decision.
2. **MINOR — stale moisture summary.** The handoff's detailed summary still says "non-stagnant moisture (dry refused, bog refused with caution)", and a source comment says alder needs "wet, moving water". After the allow-list both are under-descriptive: three classes are permitted and `NeutralLegacy` is refused for a different reason than dryness. Both should name the three classes and the `NeutralLegacy` refusal.
3. **OBSERVATION — residual rot fixture coverage.** The digest tracks the aggregate across stems 0–99 plus individual flags for 0–11, so a reassignment among 12–99 can preserve both. Recording all 100 flags would close it.

No new runtime defect or spec inconsistency was found. The module remains free of Part 2 wiring, save fields, I/O, clock access and ambient randomness.

## Regression check at the new HEAD

| Criterion | Status |
| --- | --- |
| AC-1 | **PASS** |
| AC-2 | **PASS** |
| AC-3 | **PASS** |
| AC-5 | **PASS** |
| AC-6 | **PASS** |
| AC-7 | **NOT TESTED** (Part 2, queued) |

## Publication notes

- No code, spec or test file changed after this review. The repaired branch remains at `8d097f3414533e46a72af76c861ac83dabdf0754`; this branch records the verdict only.
- **Why the two non-blocking items were not repaired here.** Any further commit would move the reviewed HEAD and invalidate the technical sign-off above, forcing a third review cycle for cosmetic documentation. They are recorded for the Manager and belong with the next packet that touches this lane.
- **The verdict is a governance gate, not a code gate.** Every acceptance criterion Part 1 claims is PASS and spec conformance is PASS. The remaining requirement is an independent sign-off commissioned outside the implementing thread's child tree.
- Nothing was merged. `origin/integration/2d-sim` is untouched by this lane; `origin/main` remains `bdeb6a324acaf4129c10fb0846faac8b64974360`.
