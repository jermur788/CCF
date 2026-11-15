# scen1 — DeepSeek post-signature continuation: STAGE RESULT (T-00 complete, implementation gated)

**Run:** 2026-11-15, parent [#544](https://github.com/jermur788/CCF/issues/544), continuing closed [#535](https://github.com/jermur788/CCF/issues/535).
**Orchestrator:** DeepSeek `opencode-go/deepseek-v4.1-flash`, variant **high**, ORCHESTRATOR ONLY.
**Branch:** `orchestration/scen1-save-v2-post-signature-20261115-v1`, created from the closed #535 head `cf4f1499`.
**Worktree:** `/media/jer/Files/CCF/Orchestration/CCF-Scen1-SaveV2-PostSig-20261115` (docs-only, isolated).
**Ledger (inherited, not restarted):** [EVENTS 001–015 from #535 preserved, 016–022 appended](https://github.com/jermur788/CCF/blob/orchestration/scen1-save-v2-post-signature-20261115-v1/Docs/Handoffs/ScenarioOne-Save-Orchestration-20261115/scen1-ORCHESTRATION-EVIDENCE-LEDGER-20261115.md).

---

## 1. Status: BLOCKED on ownership — deliberately, not by failure

**Update (2026-11-15T15:10Z): unblocked by the Manager's T-01 ruling, and T-01 has since been implemented and independently reviewed.** This section is kept for continuity; the current state is at the top of the table in §2 and summarised here.

**Current state:** T-00 **APPROVED as audit**. T-01 **IMPLEMENTED** (`da962372`, 9 files) → **REVIEWED (source)** by independent Opus 5.5 Medium → **APPROVED FOR T-02 CONSTRUCTION**, 0 blocker / 0 material / 3 minor / 2 nit. `#549` is **open**, awaiting the **Manager's source verdict** — which is not mine to give. T-02–T-05 remain **unreleased**. Unity remains blocked on the separate `#548` ops claim.

**Original blocked handback (kept as history, superseded):**

T-00 is complete. T-01 was blocked on `#424`'s file-set grant and base ruling, and I did not work around it.

That was the correct stop at the time: the signed contract authorises a design, not a writer, and the continuation packet makes `#424` the sole authority for save-subsystem ownership and integration order. Writing C# on an unresolved baseline — with a live unmerged candidate holding five *semantic* overlaps in the same files — is precisely the failure this gate prevents.

**State distinctions, kept honest:**

| State | Value |
| --- | --- |
| SPEC | **APPROVED** at immutable `200add4e` (four blob IDs re-verified **4/4**) |
| IMPLEMENTED | **NO** — zero source edits |
| REVIEWED (source) | **N/A** — no source exists yet |
| INTEGRATED | **NO** |
| UNITY VERIFIED | **NO** — not attempted; ops gate unresolved |
| USER ACCEPTED | **NO** |
| Windows build | **NOT ATTEMPTED, NOT AUTHORISED** |

---

## 2. What this stage delivered

| Claim | Work unit | Provider | Commit | State |
| --- | --- | --- | --- | --- |
| [#545](https://github.com/jermur788/CCF/issues/545) | **T-00** ownership preflight + current-source delta (read-only) | OpenAI **B** `gpt-6.1-sol` high | `f86e3010` | **HANDED-OFF**, Manager **APPROVED as audit**, closed |
| [#549](https://github.com/jermur788/CCF/issues/549) | **T-01** fixed three-profile registry + early-selection seam | OpenAI **B** `gpt-6.1-sol` high | `da962372` | **IMPLEMENTED → REVIEWED (source)**; **open**, awaiting Manager source verdict |
| [#550](https://github.com/jermur788/CCF/issues/550) | **T-01-R** independent source review | Claude **A** `claude-opus-5-5` **medium** | `08a494dc` | **APPROVED FOR T-02 CONSTRUCTION** — 0 blocker, 0 material, 3 minor, 2 nit; closed |

[Report](https://github.com/jermur788/CCF/blob/f86e3010b338d627fb8be9272549442e27dd8368/Docs/Handoffs/ScenarioOne-Save-Orchestration-20261115/scen1-T00-SAVE-OWNERSHIP-PREFLIGHT-20261115.md) — 212 lines. **Re-verified by me rather than trusted:** 5/5 fixture hashes re-hashed here (host v9 `.bak`, frozen Reference, `v20-y5`, `v21-y1`, `v21-y0-rack`) all **MATCH**; the sixth was verified at three paths in the prior run. `main` is unchanged at `fc18190b`, so G0's `file:line` facts still hold.

**Deliverables that unblock T-01 the moment #424 rules:**

- **Allowlist:** 36 C# files + 3 `.cs.meta`, mapped to T-01…T-04.
- **`#443` overlap:** 15 files — **5 SEMANTIC, 10 TEXTUAL ONLY**, with exact zero-context hunk ranges per file.
- **Fixture coverage:** ISO-58 stays **BLOCKED** on CG-TRIG; O2 stays **unavailable**. Neither presented as green.

---

## 3. I revised my own recommendation on evidence

I first recommended "integrate `#443` first". Sol recommended "base on `main`", arguing that basing on `acb4e111` would **inherit 47 unmerged, unqualified Asset changes as the historical baseline** — silently treating unverified world behaviour as the reference in a save-safety task.

**That objection is correct and my original framing under-weighted it.** The recommendation now standing with #424 is a synthesis:

1. Integrate `#443` → `main` (it is G3-approved; this removes the collision from the highest-risk code).
2. **Repin** the save base to the newly accepted `main`.
3. Redo **only** the changed source/caller/fixture delta — **not** the signed spec review.

That obtains both properties instead of trading one for the other.

---

## 4. Two material findings about the code about to be integrated

**4.1 `#443` changes historical save behaviour, unguarded by version.** At `ForestSaveController.cs:+464–474` it builds the saved-buildable-ID set and calls `SetRetiredFromStartingWorld` for **every** retirement-enabled scene object, treating **absence from the save as retirement**:

- a valid old world with no entry for such an object can newly **retire** it;
- a later capture then **omits** it from the save;
- `ForestStartingStand.cs:+340` and `PrescriptionApproval.cs:+349` stop treating retired buildables as clearing/obstacle points.

**I am not claiming an observed fixture regression** — neither Sol nor I loaded a genuine fixture to demonstrate a visible difference, and I will not manufacture that claim. The behaviour change is real and belongs inside #424's integration decision.

**4.2 A new global preference.** `#443` adds `CCF.UI.ReduceMotion` (`ScenarioOneUiRoot.cs:57–60/217`). It is **not** either protected namespace — `CCF.Learning.v1.*` and `CCF.MenuHelp.v1.*` are verified untouched, and Sol confirmed that independently rather than trusting my earlier check. T-03 must inventory it in the prefs sentinel so a profile switch cannot leak it.

**Also:** `#443`'s preview-return pose repair is **compatible** with the spec's intent and should be **kept** on reconciliation — but it does **not** close the snapshot-loss / false-"Returned" risk, which remains T-03 work.

---

## 5. The Unity ops blocker, now a reviewable decision rather than a complaint

`CCF-Ops/scripts/_ccf.py` binds storage identity to the **Linux device node** (`storage_check()` line 36, `storage_health()` line 45–46) against `lanes.json`'s `/dev/sda2`, while the live mount is `/dev/sdc2`. **The device node is the one part of that identity Linux does not keep stable** — UUID exists exactly to survive that. The check produces a **false negative**.

Demonstrated live (read-only):

| Test | Result |
| --- | --- |
| Identity **with** device binding | **False** — fails today |
| Identity **without** device binding | **True** — passes on the real storage |
| Negative: wrong UUID `DEAD-BEEF` | **False** — protection intact |
| Negative: wrong FSTYPE `ext4` | **False** — protection intact |

**Proposed minimal fix for owner review:** stop binding on `source`; keep `target` + `fstype` + `uuid` as the binding identity in **both** functions (fixing only `storage_check` would leave `storage_health` reporting `WRONG DEVICE`); demote `device` in `lanes.json` to a recorded **advisory** value.

**Not applied. Not approved.** This is removing a false negative, not removing a check — the negative cases above prove the check still rejects a wrong volume. The operations owner decides, under a separate scoped claim with no Editor launches.

---

## 6. Environment facts from this stage

- **Host root filesystem is 100% full** (`/dev/sda2`, 255 MB free). T3 provisions child worktrees under `/home/jer/.t3/worktrees`, which is on it, so the first T-00 dispatch died at `git worktree add` with `No space left on device`. Recovered by running the worker on `/media/jer/Files` (134 GB free). **I did not prune the ~20 other sessions' worktrees** that would have relieved pressure — they are not mine. **Any** `isolation:'worktree'` child in this installation will fail until root space is freed.
- `/media/jer/Files` is healthy: 134 GB free, 7% inodes.

---

## 7. Exact next executable action

**One `#424` ruling opens T-01 immediately.** The paste-ready brief is below; everything it needs is already established.

> **T-01 — fixed profile registry and context.** Base: the `main` head named by #424 (recommended: after `#443` lands, repinned). Exclusive writer over the 36-file allowlist in T-00 §3. Provider: OpenAI GPT-6.1 Sol, one writer, worktree on `/media/jer/Files`. Implement the signed §4.1/§4.2 registry: three profiles, two chapters, historical `forest-save.json` (+ `.tmp`/`.bak`) **byte-untouched**, distinct new compact/O2 reserved slots with derived sidecars, exact new envelope, prefs namespaces. **O2 stays unavailable** without a signed authored world binding. Pair each route with one negative check as it is added. **No Unity** (ops gate open). No Windows build. Handoff to `Docs/Handoffs/ScenarioOne-Save-Orchestration-20261115/`.

Then: **T-02** raw/typed admission (including H-05 pre-mutation foreign rejection per D-079/D-081), **T-03** staged lifecycle/geometry/preview/prefs, **T-04** safe writer and same-slot recovery, **T-05** Opus 5.5 Medium independent source review then Tier1 → exact-head Tier2 Unity **only after** the CCF-Ops lease is repaired and a real lock is held.

---

## 8. Provider and throughput record

OpenAI **A = QUOTA-EXHAUSTED** (provider-stated reset `2026-11-18T13:43`, timezone unstated — no percentage ever observed). OpenAI **B = AVAILABLE and proven: 5/5** Sol tasks across both runs. Claude A/B available; **Sonnet never used** (condition unmet). Opus used twice in the prior run, within preauthorisation. **DeepSeek lease: none. Unity imports: 0. Builds: 0. Source edits: 0.**

| Claim | Start UTC | End UTC | Class | Provider/model | Unity | Confidence |
| --- | --- | --- | --- | --- | --- | --- |
| #544 registration | 13:05 | — | active | DeepSeek Flash high | NONE | HIGH |
| #545 T-00 attempt 1 | 13:24 | 13:38 | **failed at provisioning** | OpenAI B | NONE | HIGH (verbatim error) |
| #545 T-00 attempt 2 | 13:38 | 14:05 | ~27 min | OpenAI B `gpt-6.1-sol` high | NONE | HIGH |
| Runner-last commit | `cf4f1499` (#535 head) | proposed base | | | | |

**Rows above are offered to `#424`/`#425` for the separately-owned append-only project throughput ledger. I did not write that file or the live board.**

**Dashboard/Drive: NOT VERIFIED.**
