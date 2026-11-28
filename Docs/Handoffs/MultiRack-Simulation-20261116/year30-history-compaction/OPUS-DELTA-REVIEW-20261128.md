# Independent Delta Review — Year-30 Deep-History Payload Compaction, Owner Ruling Option 1

- **Reviewer**: Claude Opus 5.5 (`claude-opus-5-5`). Read-only delta reviewer. Provider-separated from the implementer (GPT-6.2 Sol). No self-review.
- **Feature branch**: `feat/multi-2d-year30-history-compaction-v1`
- **Reviewed feature HEAD**: `cea73627434ea290fec940c1e61cbdeab9810270` (docs commit: "docs(handoff): record Year-30 Option 1 repair and verification")
  - Repair code + tests commit: `3b79014c` ("fix(2d-sim): separate view compaction target from HTTP ceiling")
  - Repair base: `9946276a79de55a1cb9a5aead4e2595e97e906fe` (the candidate from the prior review). It is an ancestor of `cea73627`.
  - Original base: `f4dee8023d3492a25e61451c7e2a49515e507ed3`. It is an ancestor of `cea73627`.
- **Protected `origin/main`**: `bdeb6a324acaf4129c10fb0846faac8b64974360`. Re-read at review start and again before writing; it has not changed.
- **Review branch / parent**: `review/multi-2d-year30-history-compaction-v1` @ `c73ed7d52cbaacff4f3993217aba670fe766fbb4`
- **Prior review**: `OPUS-INDEPENDENT-REVIEW-20261128.md` (NEEDS CHANGES: at Year 30 on MA-1-MIXED under steady thinning, `GET /api/multi/view` returned 503 at 525,092 B against the 524,288 B cap).
- **Date**: 2026-11-28.

## INTEGRATION DECISION

**APPROVED**

Option 1 is implemented as the Owner ruled. The change is small and stays inside its file fence. All requested gates reproduced independently:

- The exact Year-30 workload that failed before now returns **HTTP 200 for both the advance and the view**. The view is compact, honestly flagged, and well below 600 KB.
- The idle Stage B maxima are unchanged.
- The hard ceiling still refuses with 503 under a strict test override.
- Builds are clean, Core.Tests passes with zero anchor drift, the smoke suite and the Step 2 harness pass.

The nits in §6 do not block integration.

## 1. File fence and ancestry — PASS

`git diff --name-status 9946276a..cea73627`:

```
M  Docs/Handoffs/MultiRack-Simulation-20261116/year30-history-compaction/IMPLEMENTATION-HANDOFF.md
M  Tools/MultiRackSitkaP1Lab/src/Ui/MultiApi.Stand.cs
M  Tools/MultiRackSitkaP1Lab/src/Ui/MultiApi.View.cs
M  Tools/MultiRackSitkaP1Lab/src/Ui/MultiViewBudget.cs
M  Tools/MultiRackSitkaP1Lab/tests/ui-smoke-multiarea.sh
5 files changed, 268 insertions(+), 105 deletions(-)
```

These are exactly the five expected files.

**Zero changes** under `Tools/MultiRackSitkaP1Lab/src/Core/**`, `src/Ui/wwwroot/**`, `src/Ui/multi/**`, `Assets/**`, `Packages/**`, `ProjectSettings/**`, `Docs/Project/**`, `2D_Simulator_Strategic_Roadmap.md` or `.p0ref/**`. `src/Core` and `tests/Core.Tests` are also unchanged across the whole range `f4dee802..cea73627`.

The fixes from `9946276a` are intact. The Option 1 diff does not touch any of these:

- **`annual.report.trees` omission**: `ProjectAnnualForBudget` at `cea73627` (`MultiApi.View.cs:95-109`) still projects only `yearBefore/yearAfter/before/postGrowth/after/postGrowthDensityPressure/meanVulnerability/deaths`.
- **Envelope-exact budgeting**: every view-carrying route still passes an `envelopeSerializer` and then `WriteBoundedBytesAsync`. Call sites are in Commands, History, Saves, Woodland and View.
- **Honest `historyTruncated` / `pruneLogTruncated`**: both are still computed from the actual keep counts.
- **`/api/multi/history/index` and `/api/multi/history/year`**: `MultiApi.History.cs` is unchanged. Live check at Year 30: `history/index` returned 200 with `totalRecordedYears=31`.
- **Dead-tree projection**: `ProjectViewForBudget` / `ProjectStage4ForBudget` are unchanged. At Year 30 the compact projection still carries `isLiving` and `death` for all 189 dead trees.

## 2. Option 1 source review — PASS

| Requirement | Verified in code at `cea73627` |
|---|---|
| Soft target still drives Tier 0 → 1 → 2 and history/prune-log trimming | `TryFitTier` and the binary search compare against `maxViewBytes = budget.MaxViewBytes` (soft). This logic is unchanged. |
| Tier 3 is accepted as 200 when it fits under the hard ceiling | `MultiApi.View.cs:312-328`: Tier 3 serializes with `exceeded:false`. The response is flagged exceeded only when `finalBytes.Length > hardMaxViewBytes`, and only then is it re-serialized with `exceeded:true`. |
| Only bodies above the hard ceiling return 503 | `WriteBoundedBytesAsync` (`View.cs:455-463`) refuses when `alreadyExceeded \|\| bytes.Length > budget.HardMaxViewBytes`. It is the single sink for every view-carrying 200/409 route; `WriteBoundedJsonAsync` delegates to it. |
| No accepted body carries `payloadBudgetExceeded:true` | The flag is `true` only when the response is above the hard ceiling, and that path always goes to 503. Live: 31/31 advances and 31/31 views had `payloadBudgetExceeded=false`. |
| No 200/409 body can exceed the hard ceiling | The sink checks the actual wire bytes against `HardMaxViewBytes` on every call. |
| Lane-filtered stand path uses the same hard ceiling | `MultiApi.Stand.cs`: the Tier-3 fallback returns `overBytes.Length > budget.HardMaxViewBytes`. The 200 and 409 branches go through the bounded writers with the same `ActiveLaneBudget` (`ForLaneFiltered` → `ForStand`). |
| Production hard ceiling | `MultiViewBudget.DefensiveHardCeilingBytes = 4_194_304`; `hardViewBytes = Math.Max(softViewBytes, 4_194_304)`. The scaled soft target is at most 2,621,440, so production hard is always 4,194,304. |
| Override can only lower, and it clamps both soft and hard | `soft = min(soft, override)`, `eco = min(eco, override)`, `hard = soft`. A large override (e.g. 10 MB) gives hard = 524,288, which is still lower than the production hard ceiling, so the legacy strict behaviour is kept. |
| Headers are accurate | `X-Multi-View-Soft-Limit = budget.MaxViewBytes` and `X-Multi-View-Hard-Limit = budget.HardMaxViewBytes` are set on every bounded response, including 503. Live default: `524288` / `4194304`. M17 override: `3000` / `3000`. |

## 3. Year-30 steady-thinning reproduction — PASS

I wrote my own probe, separate from the implementer's M16. It ran against a fresh Debug UI process built from the disposable clone, with a scratch fixture and scratch XDG, on an OS-assigned port (35317). `CCF_TEST_MAX_VIEW_BYTES` was unset, the stand was `--multi-experiment=ma-1-mixed`, and economy was Off.

Each year, for each area (MA-A, MA-B), the probe sent 2 × `POST pending/add` with a 500-character operator reason and 3 × `POST prune/add`. It then sent `plan/approve`, `plan/execute`, `POST year/advance` (`consentToCancelWork`) and `GET view`.

Totals: 120 fells accepted, 123 prunes accepted, 57 prunes refused with 409 by Core (`PruneIntervalTooShort`, the expected engine rule).

| Year | `POST year/advance` | `GET view` | View flags |
|---|---|---|---|
| Y25 | 200, 524,199 B | 200, 524,278 B | hist, proj, s4 |
| Y29 | 200, 521,166 B | 200, 524,080 B | hist, proj, s4, prune |
| **Y30** | **200, 520,439 B** | **200, 525,089 B** | `historyTruncated=true`, `projectionCompacted=true`, `stage4Compacted=true`, `pruneLogTruncated=true`, `payloadBudgetExceeded=false` |

- Headers on every response in Years 0–30: `X-Multi-View-Soft-Limit: 524288`, `X-Multi-View-Hard-Limit: 4194304`.
- The Year-30 view is the Tier-3 compact envelope. It is 801 B above the soft target, about 75 KB below 600 KB, and about 3.67 MB below the hard ceiling.
- Every year from 0 to 30 returned 200 for both advance and view. The server's stderr was empty (0 B), and it stopped cleanly on SIGINT.
- The implementer's M16 replay in my smoke run reported advance 200 at 520,443 B and view 200 at 525,093 B. The 3–4 B difference from my probe is due to different reason text of the same length. The prior review's failing case was 503 at 525,092 B, so the same envelope is now accepted.

## 4. Idle Stage B — PASS

I ran `tests/phase1-time-history-ui/run-step2-edge.sh` in the disposable clone. It uses real WSL2 UI processes and installed Microsoft Edge through Windows node.

- **OVERALL: PASS (67 passed, 0 failed)**. There was no NB1c flake, so no retry was needed.
- `G5 ma-1` Years 0–30: raw maximum **517,010 B at Y27**, ecology maximum 43,320 B.
- `G5 ma-1-mixed` Years 0–30: raw maximum **523,424 B at Y29**, ecology maximum 49,685 B.
- The worked MA-1-MIXED segment for Years 16–30 peaked at 523,621 B raw and 48,425 B ecology.
- Every year had `historyTruncated` honest and kept at least 1 event and at least 1 annual record.

All idle view maxima are ≤ 524,288 B, and all ecology maxima are ≤ 153,600 B. Both match the expected values exactly.

## 5. Hard ceiling, builds and tests — PASS

- **M17** (`CCF_TEST_MAX_VIEW_BYTES=3000`, ma-1-mixed): `GET view` returned **HTTP 503** with `{"code":"MultiViewPayloadBudgetExceeded","bytes":284466,"budget":3000}` and headers `X-Multi-View-Soft-Limit=3000`, `X-Multi-View-Hard-Limit=3000`. Stderr had exactly one line: `PAYLOAD BUDGET EXCEEDED (HTTP 503)`.
- **Production hard ceiling = 4,194,304 B**, confirmed both from the code (§2) and from live headers (§3).
- **`tests/ui-smoke-multiarea.sh`**: **OVERALL: PASS (377 passed, 0 failed)**, including `PASS M16` and `PASS M17`. No server process was left running afterwards.
- **Disposable checkout**: `git clone --shared` into `~/.ccf-orch/evidence/opus-delta-y30-20261128/repo`, detached at `cea73627`. This is outside every worktree. SDK `~/.dotnet/dotnet` 8.0.134; `global.json` pins 8.0.133 with `latestPatch`. SDK 10 was not used.
  - `.p0ref` pin: rebuilding with `build/build-p0-core-ref.sh` on SDK 8.0.134 produced a DLL with hash `bcbdbf05…`, which does not match the pin. The csproj correctly refused it. Following the README's "copy from the authorised pinned copy" procedure, I copied `.p0ref/` read-only from the implementer's worktree. All four values equal **`9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3`**: the DLL sha256, the `.sha256` record, `provenance.txt` (sdk=8.0.133) and `P0CoreExpectedSha256` in the csproj.
  - `dotnet build MultiRackSitkaP1Lab.sln -warnaserror --no-incremental`: **0 warnings, 0 errors**.
  - `dotnet build src/Ui/SitkaP1.Ui.csproj -warnaserror --no-incremental`: **0 warnings, 0 errors**.
  - Core.Tests (`dotnet run --project tests/Core.Tests/SitkaP1.Core.Tests.csproj -c Debug --no-build`): **314/314 passed, 0 failed**. The tests themselves assert the nine frozen anchors. In addition, all 26 output lines containing 64-character hashes are byte-identical to the prior Opus review's log and to the implementer's Option 1 log. Anchor drift is 0, and anchors 10–11 are also exact.
- No server was started on :33767, :43649 or :46593. Every ephemeral server I started stopped cleanly, and no `sitkap1-ui` process remained at the end.

## 6. Non-blocking nits

1. **Ecology soft cap.** `TryFitTier` gives up when projected ecology exceeds the soft ecology cap, so that case now goes to Tier 3 and is accepted up to the 4 MiB hard ceiling. This follows the ruling, but it means the 153,600 B ecology target is also only soft now. Observed ecology stayed under 50 KB idle and about 100 KB worked, so it is not reached in practice.
2. **Leftover asymmetry from the prior review.** The non-composite command branch still uses `WriteBoundedJsonAsync` without tiering. It is unreachable for two-area composite sessions.
3. **Header change on small refusals.** Plain 409 refusal bodies on the stand route now also carry the limit headers. This is harmless and, if anything, more consistent.
4. **Step 2 overdue-fallback label.** The idle Step 2 overdue-fallback label is "anchor 4". This is unrelated to the hash anchors.

## 7. Skipped (as instructed) / not directly verified

- Skipped: `build/2d-gate/selftest.sh`, `Phase1Save.Tests`, `run-map-first-lenses-edge.sh`, `build/2d-gate/module-graph.mjs`, the full `build/2d-gate.sh --browser`, Unity, and the Phase 2A browser gate.
- The implementer's report that the Phase 2A gate's 1 failure plus timeout also reproduces on unmodified `9946276a` is noted as pre-existing evidence only. I did not re-run it, and I do not treat it as an Option 1 regression.
- Not verified: I did not render-review the compact Tier-3 view in a browser at Year 30 of the thinning workload. Step 2 renders the idle and worked Year 16–30 paths only.
- The `.p0ref` DLL was copied from the pinned copy, not rebuilt byte-identically, because SDK 8.0.133 is not installed on this host.

## 8. Provenance

- **Requested**: independent Delta Reviewer, Claude Opus 5.5, provider-separated from the GPT-6.2 Sol implementer.
- **Selected / Resolved / Executed**: Claude Opus 5.5 (`claude-opus-5-5`) through the Claude Code harness in T3 Code. Effort was the session default; no explicit effort override was exposed to the model.
- T3 thread IDs are not exposed to the running model, so none are recorded here.
- Evidence: `~/.ccf-orch/evidence/opus-delta-y30-20261128/` (`probe.log`, `smoke.log`, `step2.log`, `coretests.log`, `build-*.log`, `core-hash-lines.txt`) and `C:\Users\hp\AppData\Local\Temp\opus-delta-step2-20261128\`. The probe script is outside the repository.

## 9. D-030 state

Reviewed on isolated branch; NOT merged to `integration/2d-sim` or `main`; `origin/main` `bdeb6a32` untouched; not user-accepted.
