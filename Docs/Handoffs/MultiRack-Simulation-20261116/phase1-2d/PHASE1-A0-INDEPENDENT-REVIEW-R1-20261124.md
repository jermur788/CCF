# 2D Phase 1 — A0 Independent Review (R1) and Reconciliation

**Reviewed candidate:** `21df07c2ee984fe19abec9d86e53b88efeb6af6b` (branch `feat/multi-2d-phase1-a0-20261124-v1`, base `cf6fe7a5`).
**Reviewer:** Claude Sonnet 5.5 (fresh context, separate model from the Opus implementer), attempt `att-a0-review-0003`, isolated detached checkout.
**Verdict:** **NEEDS CHANGES** — five blocking findings, none requiring redesign.
**Orchestrator reconciliation:** spot-checked in code and **confirmed** before ordering a repair.
**State after this review:** `IMPLEMENTED: yes` · `TESTED: contract level` · `REVIEWED: NEEDS CHANGES` · `INTEGRATED: no` · `ACCEPTED: no`.

## 1. Review routing history (honest)

| Attempt | Route | Outcome |
|---|---|---|
| `att-a0-review-0001` | Gemini 4 Neon | LAUNCH FAILED — individual quota |
| `att-a0-review-0002` | Orchestrator Neon | LAUNCH FAILED — same provider-wide quota |
| `att-a0-review-0003` | Claude Sonnet 5.5 (owner-selected) | Completed, verdict NEEDS CHANGES |

Separation limitation: attempt 0003 is a fresh context and a different model, but the same provider family as the implementer. A strict cross-provider review remains available later when Neon quota resets.

## 2. Blocking findings (all confirmed by the orchestrator in code)

| Id | Defect | Orchestrator confirmation |
|---|---|---|
| F1 | Slot-name allowlist bypassed by a trailing newline (`^\u2026$`) | CONFIRMED — `Phase1SaveSlots.cs:60` uses `^…$` |
| F2 | Invalid UTF-8 raises untyped `InvalidOperationException`; `Inspect`/`TryRefreshBackup` catch only `P1Refusal`, so a bit-rotted primary cannot be replaced | CONFIRMED — `Decode` calls `StrictJson.Parse` uncaught |
| F3 | `replaceExisting:false` is check-then-rename and can silently overwrite a concurrently created slot | CONFIRMED — existence test then unconditional `AtomicRename` |
| F4 | Validator accepts impossible states: unaligned accounts, unbound mast seeds, mismatched band lattices, revision inconsistency, unlinked death/fell records, inconsistent provenance | CONFIRMED — e.g. `PlanStateRevision` only `>= 0` |
| F5 | Header has no engine/algorithm lineage, so an older algorithm with the same 5-number model vector would load silently | CONFIRMED — header carries no version fields |

## 3. Non-blocking observations accepted into the repair

Vacuous test asserting a pure function "left state untouched"; aliased fixture values that would hide a single-site typo; no pinned golden checksum; symlink ancestor check only on the last path component; `null Models` → `NullReferenceException`; `Inspect` reports a directory at the primary path as "not exists"; crash injection is exception-based rather than process-kill.

Also accepted: B1 — the persisted approved plan must not carry `planId`/`executionKey`/`recordedSource`, because owner policy A discards approval; orders are kept and A1 rehydrates them as an unapproved pending draft. Executed-key receipts keep their provenance token, which the reviewer judged inert.

## 4. Reviewer rulings on the implementer's open points

- **B1** — accept provenance in executed-key receipts; **drop authority-bearing plan fields now** while the schema is open.
- **B2** — A1 must guarantee: fresh random session id per load, verbatim executed map, stale-fence refusal for every command including Execute-replay, new ids that cannot collide with loaded prefixes, exact `PlanStateRevision`, approved orders returned to pending and revalidated, refusal if any cannot be preserved, parity tests ignoring session-prefixed ids.
- **B3** — blocking, promoted to F5.
- **B4** — accept the `f64`/`i64`/`utf8` tags as lossless, deterministic and unambiguous.
- **B5** — A0 is clean; the map-owning writer must change `MultiSessionOnly`, `MultiChipSessionOnly`, `PlayerMultiApi.cs`, `Ui/Program.cs`, `README.md` and their pinned tests before any build exposes saving.

## 5. Reviewer's supporting evidence (beyond A0's own tests)

Out-of-repo probes (deleted afterwards) showed: capture → encode → decode → restore round-tripped bit-for-bit; a real Phase 2 session over 30 years was accepted every year with a 1.22 MB peak save; a year-12 root saved and reloaded through bytes then run 18 more years equalled the uninterrupted control exactly. The reviewer correctly labels this **supporting evidence, not A0 acceptance**, because the probe reached engine internals by reflection.

## 6. Requirement matrix at review time

| Requirement | Result | Basis |
|---|---|---|
| P1-01 Full mutable root | PASS WITH LIMITS | All fields present; real-engine probe parity; A0's own tests are synthetic and mapping is untested in A0 |
| P1-02 Strict compatibility | PASS WITH LIMITS | Strict decode works except invalid UTF-8 (F2); validator gaps (F4); no lineage (F5) |
| P1-03 Durable safe writes | PASS WITH LIMITS | Fault injection at every stage preserves prior save and backup; F1 and F3 open |
| P1-10 Protected boundaries | PASS WITH LIMITS | 12 additions, nothing modified; frozen anchors and Phase 2 suite NOT TESTED in this scope |

## 7. Action taken

Bounded repair `att-a0-repair-0001` dispatched to the **same exclusive writer** (Opus 5.5): fix F1–F5 with tests, plus the accepted non-blocking items; keep the diff inside the existing A0 allowlist; update the handoff with a per-finding PASS/FAIL/NOT TESTED table. Then a focused re-review of the repaired areas.

## 8. What is still not established

No live capture/restore under the engine lock, no session fence, no load policy A, no history/skip, no real power-loss durability, no map-branch compatibility, no owner playtest. A1 remains **BLOCKED BY MAP OWNERSHIP**.
