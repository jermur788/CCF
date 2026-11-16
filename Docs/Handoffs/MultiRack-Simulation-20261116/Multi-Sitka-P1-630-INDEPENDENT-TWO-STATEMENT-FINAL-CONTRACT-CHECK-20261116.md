# Multi — #630 independent final two-statement P1 input-contract check

**Date:** 16 November 2026. **Issue:** [#630](https://github.com/jermur788/CCF/issues/630).
**Verdict:** **PASS — EXACT TWO-STATEMENT CONTRACT**, with the reviewer-class limitation recorded in §0. No remaining conflicting operative span was found within the corrected statements' domain.

## 0. Reviewer identity and the limitation that must not be hidden

| Field | Actual, disclosed |
|---|---|
| Provider | `opencode-go` |
| Model | `deepseek-v4.1-flash` (DeepSeek V4.1 Flash) |
| Harness / host | OpenCode in T3 Code, Windows 11 |
| Effort | **Not observable in this harness; not claimed** |
| GitHub identity | repository credential acting as `jermur788` |
| Worktree | temporary docs-only checkout outside the repository, removed after push; **no** Unity Editor, **no** `Assets/`, **no** CCF-Ops lease |
| Review branch | `review/multi-p1-630-two-statement-check-20261116-v1` |
| Starting HEAD | `10adc65aea3c0fe74a0ba34bc533ac2c0267b7f8` |

**Authorship independence: PASS.** This session did not author the #623 four-file P1 DRAFT, did not author the #628 review artefact, and is not #424 Manager. Nothing here is author-self-approval.

**Provider-class conformance: FAILING, and stated plainly.** The binding packet names a *"non-Manager, non-#628-document-author independent ... Claude Sonnet/Opus or Codex High"* reviewer; the issue body says *"preferred ... Claude or Codex"*. **This worker is neither Claude nor Codex.** Therefore:

- this is a genuinely independent, source-literate documentation check, but it is **not** a Claude or Codex signoff;
- it must not be cited as satisfying the packet's named reviewer class;
- the Manager must decide whether to accept a DeepSeek V4.1 Flash reviewer for the formal #630 gate, or to reassign the gate to a separate Claude/Codex reviewer.

No cross-provider claim beyond that is made. No capability class is implied by this verdict.

## 1. Preflight and starting GitHub state

- **Live board:** `main:Docs/Project/coordination/active-tasks.md` refreshed read-only from `origin/main`. **No #630 row is present**, and no board edit was made — that file remains Manager single-writer.
- **Writer collision:** none. The only prior comment on #630 was a hard-stop by the session that authored #628, which explicitly released the task and declined to claim it. It is not a live `[RUNNING]` claim. The `[RUNNING]` registration for this session was posted before any write.
- **Branch state at intake:** the precreated review branch resolved to `10adc65aea3c0fe74a0ba34bc533ac2c0267b7f8`, **zero diff** to the completed Manager correction branch `docs/multi-p1-628-two-statement-manager-correction-20261116-v1`. This matches the packet's "compared zero diff to Manager repair base".
- **Comparison base:** `docs/multi-p1-623-30m-operative-alignment-20261116-v1` at `4ca2a416f1090c1808bd99d5c7be5b13ea2dd293`.

## 2. Exact delta verification

Commands (read-only, git plumbing):

```
git diff --stat 4ca2a41 10adc65
git diff -U12 4ca2a41 10adc65 -- specs/.../tasks.md
git rev-parse <rev>:specs/.../<file>
```

| Path | Result |
|---|---|
| `Docs/Handoffs/MultiRack-Simulation-20261116/Multi-Sitka-P1-628-MANAGER-MATERIAL-FINDING-TWO-STATEMENT-REPAIR-630-EXACT-RECHECK-20261116.md` | **added** (30 lines) |
| `specs/multirack-sitka-p1-adult-rules-comparator-v1/tasks.md` | **2 deletions / 2 additions**; blob `3c85d294…` → `7eb48f09…`; changed lines are **33** (P1-01 row) and **79** (§4 first bullet) |
| `specs/multirack-sitka-p1-adult-rules-comparator-v1/spec.md` | **byte-identical** (`ca010345…`) |
| `specs/multirack-sitka-p1-adult-rules-comparator-v1/plan.md` | **byte-identical** (`b69551e3…`) |
| `specs/multirack-sitka-p1-adult-rules-comparator-v1/checklists/requirements.md` | **byte-identical** (`e4363d92…`) |

Total branch delta: **one added Markdown plus exactly two changed operative lines** in `tasks.md`. No source, Unity, save, scene, package, coordination-board or other production paths changed.

## 3. Statement 1 — P1-01 dependency row (`tasks.md` line 33)

**Removed ambiguity (old):** `C-1A owner-confirmed D-082 original-only input with NO P0 work/history (not an open post-intervention option)` — an absolute original-only input condition on a task that is itself the later Unity 40/80m capture.

**Corrected operative span (current):**

> `C-1A owner-confirmed D-082 applies to original P0 import only: independently verified original inventory with NO prior P0 work/history; separately catalogued P1-derived 40/80m Unity oracle references are permitted and are NOT P0 originals`

**Check against the three-mode contract:**

1. **Strict original-P0 verification is preserved, not relaxed.** "Independently verified original inventory", "NO prior P0 work/history". The row did not previously carry the hash wording and does not need to; the file+snapshot hash anchor lives in §4 (statement 2) and `REQ-B08`.
2. **The derived 40/80m permission is domain-correct here.** P1-01 is the later Unity oracle task, gated on a separate owner Unity-scheduling decision and physical exclusive CCF-Ops lock, with C-6 decided. P1-F0's prohibition on 40/80 in the *first* foundation is untouched, because P1-01 ≠ P1-F0.
3. **No identity laundering is introduced.** The derived references are "separately catalogued" and explicitly "NOT P0 originals".
4. **Gate structure is unchanged:** C-2a/C-3/C-4/C-5 geometry/C-6 still must be decided; "C-2b is NOT a prerequisite; it follows from this capture" is retained verbatim.

**Result: PASS.** The change removes the false rejection of the intended derived 40/80m Unity oracle input without weakening original-P0 trust.

## 4. Statement 2 — §4 State and source equivalence, first bullet (`tasks.md` line 79)

**Removed absolute rule (old):** `P1 reads only a verified P0 synthetic source with pinned file+snapshot hashes; never modifies the original fixture/snapshot or P0 Advance experiment step semantics.`

**Corrected operative span (current):**

> **External original-P0 import only:** P1 reads an independently verified original **P0 synthetic** inventory with pinned whole-file and snapshot hashes, without prior work/history, and never modifies the original fixture/snapshot or P0 `Advance experiment step` semantics. **Separately versioned and trusted P1-derived catalogs are also valid in their respective modes:** the 30×30m synthetic experimental stand (`DerivedP1ThirtyMetreExperimentV1`) and the 40/80m Unity-oracle controls (`DerivedUnitySquareParityV1`). Neither derived catalog is a P0 original; no cross-mode relabelling, resizing or trust substitution is permitted.

**Check:**

1. **Original import trust is still strict and still hash-anchored.** "independently verified", "pinned whole-file and snapshot hashes", "without prior work/history", "never modifies the original fixture/snapshot or P0 `Advance experiment step` semantics". Nothing in the trust chain was softened; the rule was **scoped** to external original-P0 import rather than deleted.
2. **The 30m experimental mode is explicitly legal and explicitly separate.** `DerivedP1ThirtyMetreExperimentV1`, "synthetic experimental stand", with the 30×30m area stated.
3. **The 40/80m Unity reference mode is explicitly legal only in its own domain.** `DerivedUnitySquareParityV1`, "Unity-oracle controls" — naming matches the C-5/B01 later phase gated by C-6 and, after real capture, C-2b.
4. **Non-interchangeability is explicit:** "Neither derived catalog is a P0 original; no cross-mode relabelling, resizing or trust substitution is permitted." That is the property the old absolute rule was accidentally over-enforcing.

**Result: PASS.** This is a precision repair, not a relaxation: it separates *trust of imported originals* from *legality of two independently versioned derived catalogs*.

## 5. Cross-file corroboration (read-only; these files are unchanged by the delta)

- **`plan.md` B01 (line 231)** already defines exactly three tags: `DerivedP1ThirtyMetreExperimentV1` as early 30m/0.09ha synthetic P1-only source-rule experimental **not Unity parity**; `DerivedUnitySquareParityV1` as later legal 40/80m Unity oracle with source float area bits, C-6 then C-2b; `OriginalPolygonExploratoryV1` as unchanged trusted original P0 **read-only import first, annual refused until later G4-P**, with original 0.25/0.58125/2 ha area and non-parity labelling.
- **`plan.md` operative release order (line 89):** G2-F0 30m source-rule experiment + original read-only import requires **no** C-6 and **no** C-2b; G3 40/80 capture needs separate C-6; G4 parity follows a reviewed post-capture C-2b; G4-P later enables genuine original polygon annual.
- **`spec.md` §5 "Operative C-5 phase rule" and "30m/Original initial contract (D-084)":** first permitted work is independently catalogued 30m experimental annual plus verified read-only original P0 import/inspection with annual/work commands failing closed; 40/80 is later Unity capture; original polygon remains read-only until P1-POLY.
- **`spec.md` §4.1 (line 171):** "The original-only restriction above governs **imported P0 inputs only**; it does not forbid derived capture fixtures." This pre-existing sentence is why statements 1 and 2 are a *reconciliation* rather than a new policy.
- **`checklists/requirements.md` REQ-B08 / N01 / N05 / N13:** original-only import against a P1-owned reviewed trust table (file SHA + P0 snapshot hash, never a co-located manifest); N01 requires derived 30m and derived 40/80 to be accepted only through separate catalogs/versions; N05 requires three domains; N13 requires three mode tags across Host/client/save/load/replay/export.

Both corrected statements now agree with B01, the release order and the C-5 phase rule. No file has to be reconciled against another.

## 6. Preserved invariants (verified, not assumed)

| Invariant | Verified state |
|---|---|
| Requirement boxes | **72 unchecked, 0 checked** (counted `- [ ]` = 72, `- [x]` = 0) |
| Qualification rows | **N01–N13 still UNRUN** |
| Spec status | **DRAFT / NOT APPROVED** |
| C-2b | still OPEN, still post-capture |
| C-6 | still OPEN, still a separate owner Unity-scheduling decision |
| Original polygon annual | still disabled until P1-POLY |
| Invented artefacts | no fixture manifest, SHA, numeric tolerance or review result added by the delta |
| Production impact | none: no source, Unity, save, scene, package or board change |

## 7. Remaining conflicting operative span

**None found** within the corrected statements' domain. The two former absolute formulations that could reject the intended derived catalogs are the two lines that changed; no third operative statement repeats them.

Two pre-existing passages were read and explicitly judged **not** conflicting (they are byte-unchanged, so they are observations, not findings):

- `spec.md` §4.1 "permit only an independently verified original … P0 fixture" is scoped by the following sentence limiting the original-only restriction to **imported P0 inputs**.
- `REQ-B08` "Original-only import" is import-scoped: it governs the trust check on an imported original, not the existence of separate derived catalogs.

Per the packet, no formatting nits or optional improvements are raised.

## 8. What this is not

- **Not** specification approval, and **not** canonical D-084 integration.
- **Not** a runtime, Unity, compile, save or performance result. Nothing was built or run.
- **Not** an executed original-P0 import or a fixture/trust-table verification.
- **Not** authorisation for P1-F0, P1-01…05, C-2b or C-6, and **not** a permit to copy P0.
- **Not** a Claude/Codex signoff (see §0).

## 9. Verdict

**PASS — EXACT TWO-STATEMENT CONTRACT.**

The Manager correction changes exactly two operative `tasks.md` statements and adds one handoff. Both changes (a) keep external original-P0 import verification strict, including pinned whole-file and snapshot hashes and no prior P0 work/history; (b) make the independently versioned P1-derived 30×30m synthetic experimental stand explicitly legal as the first experimental annual input; and (c) make the derived 40/80m Unity-oracle controls legal only in their own later domain, never as P0 originals, with no cross-mode relabelling, resizing or trust substitution. No conflicting operative span remains in that domain, and the 72 unchecked boxes, N01–N13 UNRUN, DRAFT status, C-2b and C-6 are all preserved.

The Manager must weigh §0: this result is authorship-independent but **not** from the Claude/Codex reviewer class the binding packet names.

## 10. Publication

One added Markdown on `review/multi-p1-630-two-statement-check-20261116-v1`, parent `10adc65aea3c0fe74a0ba34bc533ac2c0267b7f8`. No other path added, modified or deleted. Readback performed against the pushed branch.
