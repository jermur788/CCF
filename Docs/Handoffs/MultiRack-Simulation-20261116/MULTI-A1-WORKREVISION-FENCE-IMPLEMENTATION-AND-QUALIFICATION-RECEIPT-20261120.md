# MULTI-A1-WORKREVISION-FENCE — IMPLEMENTATION AND QUALIFICATION RECEIPT

**Status: IMPLEMENTED · INDEPENDENTLY REVIEWED (scope-limited) · NOT INTEGRATED · NOT MERGED · AWAITING #730 ADJUDICATION**

Claim: [CCF #846](https://github.com/jermur788/CCF/issues/846). Binding packet: [Multi-A1-Repair-and-Player-Session-Adapter-Parallel-Dispatch-20261119.md](https://github.com/jermur788/CCF/blob/docs/Multi-A1-and-Player-Adapter-Parallel-Packets-20261119-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-A1-Repair-and-Player-Session-Adapter-Parallel-Dispatch-20261119.md).

Track A only. Track B (`Multi-Player-Session-Adapter-Contract`, #847) is a separate task and is unaffected.

---

## 1. Identity

| Role | Actor | Notes |
|---|---|---|
| Orchestrator (non-authoring) | OpenCode desktop, `opencode-go` / `deepseek-v4.1-flash`, T3 Code | G0 preflight, claim, lane creation, delegation, evidence inspection, publication. Wrote no C#. |
| Single native writer | `gpt-6.1-sol`, Codex pool `codex_3`, `reasoningEffort=max` | Sole author of the two allowlisted paths. |
| Independent reviewer (non-author) | Claude Opus 5.5, `claude-opus-5-5`, Claude pool `claudeAgent`, `effort=max` | Different provider family. Own clean clone at the exact candidate SHA. Did not write, commit or push anything. |

**Provenance disclosure (reviewer finding F-1).** The candidate commit's author/committer metadata reads `DeepSeek V4.1 Flash 1017 <jermur788@users.noreply.github.com>`, inherited from this repository's local Git identity, not from the registered writer. Git history alone therefore cannot attribute the patch to `gpt-6.1-sol`; writer identity rests on the #846 claim and this receipt, and reviewer independence is unaffected (Opus 5.5 is neither identity). The reviewed SHA was deliberately **not** rewritten for a metadata-only correction, because that would destroy the exact qualified artifact.

---

## 2. Candidate

| Item | Value |
|---|---|
| Base | `8741a32843b739b0587e4c6f3a3ee7fc890ea7f6` — the frozen G2 candidate, **not** `main` |
| Base engine tree | `bf4b74395c3c92ebcaa52ff83d1bfe553b4411c4` |
| Candidate commit | `4bb809732fb691f15630e1fbc5ea20dca6dbbe2f` |
| Candidate engine tree | `6059b62dcb9b2159d97ce189597998d70fab8dab` |
| Branch | `fix/multi-a1-workrevision-fence-20261119-v1` (remote ref verified by `git ls-remote`) |
| Commits ahead of base | 1 |
| P0 subtree at both ends | `f212c8f64fb28b83aa870c7b5b7c72e03051e46a` (unchanged) |
| Files changed | 2 |

```
4   1  Tools/MultiRackSitkaP1Lab/src/Core/AnnualContinuation.cs
168 0  Tools/MultiRackSitkaP1Lab/tests/Core.Tests/WorkContractTests.cs
```

(An earlier interim note said `+3/-1`; the actual numstat is **+4/−1**, net +3. Reviewer finding F-5.)

### The defect, re-derived at base

`AnnualContinuation.Advance(int expectedYear, int expectedRevision, Action? beforeCommit = null)` passed the **live** `root.WorkRevision` into `AdvanceLocked` as the *expected* work revision, so the work-revision fence compared a value with itself. A typed Fell keeps the ecological year and the annual revision and moves only `WorkRevision`, so a caller holding a pre-Fell `(year, annualRevision)` still passed both live fences and could advance the year over a roster and event ledger it had never observed. The four-argument overload fenced a caller-supplied revision correctly.

### The change

```diff
             if (root.Pending.Count > 0 || root.ApprovedUnexecuted is not null)
                 throw new P1Refusal("WorkPendingConsentRequired",
                     "pending or approved-but-unexecuted work must be cancelled with explicit consent before the year advances");
-            return AdvanceLocked(expectedYear, expectedRevision, root.WorkRevision, beforeCommit);
+            if (root.WorkRevision != 0)
+                throw new P1Refusal("ContinuationWorkRevisionRequired",
+                    "after a Fell, advancing the year requires an explicit expected work revision; use the work-revision overload");
+            return AdvanceLocked(expectedYear, expectedRevision, 0, beforeCommit);
```

`AdvanceLocked` now has exactly two callers: this guarded literal `0`, and the caller-supplied revision of the four-argument overload, which is byte-identical to base. No production caller feeds `root.WorkRevision` back as an expected value anywhere.

Behaviour: legacy two-clock `Advance` stays fully usable for no-work sessions (`WorkRevision == 0`), keeps the pending/approved consent refusal first, and fails closed with zero mutation once any Fell has occurred. `AnnualContinuation` is `internal`, `InternalsVisibleTo` names only the test assembly, and the CLI's only `Advance()` call is on `AnnualEngine`, so **no production caller changes behaviour**.

---

## 3. Qualification evidence

Independent reviewer, own clean clone at `4bb80973`, no prior `bin/` or `obj/`:

| Check | Result |
|---|---|
| Build (`TreatWarningsAsErrors=true`) | **0 warnings, 0 errors** |
| Tests | **66/66 passed, 0 failed** (62 original + 4 new) |
| Base control on pure `8741a328` | 62/62 passed — confirms the original count |
| Ten Year 0–10 no-work digests | **character-identical** to baseline (`cmp` IDENTICAL; both files `01db908a…`) |
| Allowlist | held: exactly 2 files, nothing else anywhere in the repository |
| Golden digest literal | unchanged (blob `bdb32d11…` at both ends) |
| P0 subtree / pin / build script / specs / `Assets/**` / coordination | unchanged |
| Existing tests | none removed, renamed, skipped or re-baselined; `WorkContractTests.cs` numstat `168 0` (purely additive) |
| Base-vs-candidate blob check, all test files | only `WorkContractTests.cs` differs |

### Mutation proof (reviewer scratch clone, never in the candidate)

| Mutant | Totals | Meaning |
|---|---|---|
| **M1 (required): revert only the production hunk, keep the new tests** | **64/66 — 2 failed** | Both fix-detecting tests fail with *"expected refusal ContinuationWorkRevisionRequired, but the call succeeded"*. The tests are **not vacuous**, and they fail for the right reason. |
| M2: guard kept, old argument restored | 66/66 | Equivalent mutant — the literal `0` is defence-in-depth, not load-bearing. |
| M3: guard removed, literal `0` kept | 64/66 | Caught; refuses with the wrong code (`…Mismatch`), proving exact-code assertions. |
| M4: guard moved before the consent check | 65/66 | Caught by the precedence test. |
| M5a/M5b/M5c: refuse after republishing, in-place mutation, or a partial year advance | 64/66 each | Caught by the same-root-reference check and, for the in-place case, only by the full-byte check. |

A reviewer-only runtime probe also showed the defect directly: at base, legacy `Advance(0,0)` after a real Fell **succeeded** and committed year 1 over 99 survivors; at the candidate the same call refuses with zero mutation.

### Environment and toolchain — declared, not hidden

- `global.json` pins 8.0.133 with `rollForward: latestPatch`; the available SDK is **8.0.134**, so both lanes built with 8.0.134.
- This host cannot byte-reproduce the pinned P0 reference (`9a99f78131a348032a0342b2ec3dc2b671c0455ab60fee504761e0e82e1934b3`) with 8.0.133 or 8.0.134. The git-ignored `.p0ref` was restored by copying the pinned artifact from the previous read-only `y2engine` lane and verifying `sha256sum -c`, which is the established `_lane-rebuild.sh` pattern. The pin is **satisfied**, not edited or bypassed; the pin value, `build-p0-core-ref.sh` and P0 source are untouched.
- The writer lane and the reviewer lane each independently verified the pin before building.

---

## 4. Independent review

Full document: [MULTI-A1-WORKREVISION-FENCE-INDEPENDENT-REVIEW-20261120.md](https://github.com/jermur788/CCF/blob/docs/multi-a1-workrevision-fence-receipt-20261120-v1/Docs/Handoffs/MultiRack-Simulation-20261116/MULTI-A1-WORKREVISION-FENCE-INDEPENDENT-REVIEW-20261120.md)

**Verdict: APPROVED**, scoped to (1) source review of the exact candidate `4bb80973` and (2) Tier-2 delta qualification on `4bb80973`. Not integration, not a merge, not Unity, not UI, not Stage-5 acceptance, not Manager approval.

- **CRITICAL: 0. MAJOR: 0.**
- **F-1 MINOR (provenance):** commit author metadata names the orchestrator engine, not the registered writer — disclosed in §1; manager must judge identity from the claim/receipt.
- **F-2 MINOR (process):** `wsl.exe -d Ubuntu -- bash -lc "<cmd>"` lets the outer shell expand `$VAR`/`$(…)` in the primary checkout before the inner shell runs. It happened once in the review with a read-only `git rev-parse`; the primary checkout was verified unchanged (`main` @ `cbf3a618`, clean). Use `wsl.exe -e bash -c '…'` or a script file for future lanes.
- **NITs:** F-3 per-field assertions in `WorkContractTests.cs:422–432` are unreachable-by-failure after the reference check (the real proof is the reference plus byte check); F-4 reflection into private `Digests`/`root` is acceptable; F-6 the new code should be mapped by Track B to "use the work-revision overload", not "stale work revision"; F-7 the legacy overload still lacks a `<summary>`.

No finding required a candidate change before adjudication, and none was made. NIT repairs were intentionally deferred: editing the approved bytes would invalidate the reviewed SHA for cosmetic gain.

---

## 5. What remains UNVERIFIED

- **Integration / merge / rebase to `main`:** not attempted. The candidate diverges from `main` by design.
- **Unity import, compilation, runtime, rendering:** not applicable and not run. No Unity code changed; the two linked `Assets/ForestPrototype` sources are unchanged and their existing byte-pin test passes.
- **Stage-5 UI, the Track B player-session adapter, connected UI:** out of scope.
- **Lab shell harnesses** (`p0-isolation.sh`, `build-output-safety.sh`, `cli-safety.sh`): inspected but not run, because two of them rebuild `.p0ref` (forbidden here, and not reproducible on this host) and the third exercises only the unchanged CLI.
- **P0 provenance beyond its checksum:** the binary was copied from the previous lane and checksum-verified, not rebuilt.
- **Concurrency after a Fell:** reasoned from the single `sync` lock and per-mutator lock audit, not stress-tested.
- **Actual authorship:** not provable from Git (F-1).

---

## 6. Next action

1. **Manager #730** adjudicates this receipt (APPROVED / NEEDS CHANGES / REJECTED) with an explicit scope. No merge, feature, UI or Stage-5 authority is granted by this document.
2. Optional, non-blocking follow-ups for a later packet: F-3 assertion cleanup, F-7 XML summary, and Track B's refusal-code mapping (F-6).
3. Any future UI/adapter work must build on the four-argument token-fenced path; the legacy two-clock entrypoint is now a no-work-only entrypoint by contract.

No main merge, no Windows build, no Unity, no `Assets/**` change, no coordination-file edit was made or is authorised here.
