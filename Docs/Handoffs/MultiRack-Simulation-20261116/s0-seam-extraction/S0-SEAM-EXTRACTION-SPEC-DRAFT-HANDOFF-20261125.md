# S0 Seam Extraction — DRAFT Spec Quartet — Spec-Author Handoff (2026-11-25)

**DRAFT QUARTET PUBLISHED — AWAITING OWNER APPROVAL**

Docs-only delivery. Nothing is implemented, built, tested, served, reviewed or approved. The quartet is `DRAFT` and authorises zero implementation.

---

## 1. Implementation handoff (AgentWorkflow standard field order)

```text
IMPLEMENTATION HANDOFF
Task: Author the DRAFT four-file spec quartet "Pre-2A (S0) Seam Extraction and 2D Integration Trunk"
      (feature id multi-2d-s0-seam-extraction-v1) plus this docs handoff; zero production coding.
Worker: Spec Author — Claude Opus 5.5 lane (identity record in §2; executed identity partly UNKNOWN)
Role: Primary (spec author, docs-only); not reviewer, not integrator, not approver
Branch: docs/multi-2d-s0-seam-extraction-spec-20261125-v1
Base: 5fe2fabe9073d7b72711f4ab48553c785351d7bc (tip of origin/feat/multi-2d-phase1-time-history-core-20261124-v1)
Context commit: 5fe2fabe9073d7b72711f4ab48553c785351d7bc
HEAD: the single commit on this branch that adds these five files ("docs(spec): draft S0 seam-extraction four-file
      quartet"). A file cannot contain its own commit SHA; the exact 40-char SHA is reported in the dispatch return
      and is retrievable with: git log -1 --format=%H origin/docs/multi-2d-s0-seam-extraction-spec-20261125-v1
GITHUB HANDOFF PATH: Docs/Handoffs/MultiRack-Simulation-20261116/s0-seam-extraction/S0-SEAM-EXTRACTION-SPEC-DRAFT-HANDOFF-20261125.md
GITHUB HANDOFF COMMIT: same commit as HEAD (self-reference; see above)
EXTERNAL EVIDENCE ARCHIVE: none (docs-only; all inspection was read-only Git blob inspection, summarised in §3)
WHAT CHANGED:
- Added a DRAFT spec quartet under specs/multi-2d-s0-seam-extraction-v1/ (spec, plan, tasks, traceability checklist
  with every row unchecked) encoding locked decisions A–D, packets S0a→S0b→S0c, acceptance criteria S0-01..S0-09
  + S0-REG, hard exclusions, compatibility contracts, the "adding a subsystem" checklist, the ownership map and six
  Open Questions with recommended defaults.
- Added this handoff.
FILES CHANGED:
- specs/multi-2d-s0-seam-extraction-v1/spec.md (new)
- specs/multi-2d-s0-seam-extraction-v1/plan.md (new)
- specs/multi-2d-s0-seam-extraction-v1/tasks.md (new)
- specs/multi-2d-s0-seam-extraction-v1/checklists/requirements.md (new)
- Docs/Handoffs/MultiRack-Simulation-20261116/s0-seam-extraction/S0-SEAM-EXTRACTION-SPEC-DRAFT-HANDOFF-20261125.md (new)
SERIALIZED UNITY FILES TOUCHED: none
PLAYER-FACING RESULT: none (docs only)
SIMULATION / SAVE IMPACT: none
ASSETS ADDED OR MODIFIED: none
MUST-NOT-CHANGE HASHES / ANCHORS: unchanged — no code, test, fixture or binary was touched; the 9 frozen anchors,
  the v2 golden pin and the .p0ref pin were read from source only, never recomputed or modified.
INTENTIONALLY CHANGED HASHES / ANCHORS: none
VERIFICATION RUN:
- git status --short / git diff --stat --cached before commit (scope check: exactly the five paths)
- grep -nE for a ticked checkbox marker (bracketed lower- or upper-case x) across the five files
- grep for the DRAFT status line in the four spec files and for the canonical status line in this handoff
- git ls-remote after push (remote branch HEAD equals local HEAD)
VERIFICATION RESULTS (observed before commit):
- Ticked checkbox markers: none in any of the five files. Checklist rows: 80, all unchecked.
- DRAFT status line present in spec.md, plan.md, tasks.md and checklists/requirements.md; "SPEC REQUIRED"
  present in all four; canonical status line present in this handoff.
- git status --short before staging: only the two new directories (the five files); nothing else.
- Push and ls-remote results are reported in the dispatch return (they occur after this file is committed).
- NOT RUN (by design and by the dispatch prohibitions): dotnet build/test, any gate or test script, any HTTP
  listener or browser, any Unity or Blender action.
SPEC CONFORMANCE (SPEC REQUIRED only): N/A (authoring the spec)
REMAINING VERIFICATION GATES:
- Owner approval of the quartet (status DRAFT → APPROVED) and answers to OQ-1..OQ-6.
- Independent non-author spec review (AgentWorkflow route: GPT-6.1 Sol, or a suitably independent Opus 5.5 route —
  never this author thread, never a coordination-only route).
- Manager: decision-log entry for locked decisions A–D; active-tasks registration when (and if) lanes are admitted.
- Delivery of the verified, R3-approved, owner-accepted consolidated Phase 1 baseline T0 (Phase 1 Step 2 is still
  in R2/R3), which gates trunk creation (T-00) and every S0 packet.
KNOWN LIMITATIONS:
- Every line number, size and hash in the quartet was measured at adea7af (UI) / 5fe2fabe (Core). T0 will differ
  after Step 2 R2/R3; the quartet keys every table by member/banner name and requires regeneration at T0.
- The three /multi content SHA-256 values in spec §3.1 were computed by the author from the Git blob with the
  raw-literal rule; they are NOT served-response measurements and are informational only.
- The browser tier depends on a Windows host (Edge + Windows node via WSL2 interop), exactly like the existing
  Edge suites; a pure-Linux/cloud host cannot complete S0c acceptance.
- Several S0b/S0c design details (M1/M2 relocations, hooks registry, renderer adapter, reconstruction proof) are
  proposals inside a DRAFT and need independent review before approval.
- Git author metadata on this commit reads "DeepSeek V4.1 Flash orchestrator <jermur788@users.noreply.github.com>".
  That is the repository-local user.name/user.email in the shared WSL .git/config, and it also appears on earlier
  commits by other workers on this lineage (e.g. 5fe2fabe, e3ee9ce0). This worker did not set it and did not change
  any git config. The authoritative authorship record is §2 plus the commit's "Co-Authored-By: Claude Opus 5.5"
  trailer. These docs were not written by a coordination-only route.
UNRELATED WORK PRESERVED:
- The isolated worktree was attached to branch t3/fb8b53c2 at cbf3a6185754a33f4a75b96f8fe775d0041b72a1 at start;
  that branch was neither modified nor deleted. The docs branch was created from the base SHA, not from it.
- Windows Git reported 559 mode-only entries (100755 → 100644, 0 insertions/0 deletions) because the repository's
  git dir lives on the WSL share; they are an environment artifact, not content changes. Every git command used
  per-command "-c safe.directory=* -c core.filemode=false"; no git config (global, system or repo) was written,
  nothing was stashed, and none of those entries was staged.
- The Step 2 branch feat/multi-2d-phase1-time-history-ui-20261125-v1 (adea7af…) was never checked out, never added
  as a worktree and never written; origin/main was never written; src/Ui/wwwroot/, Assets/ and
  Docs/Project/coordination/active-tasks.md were never written.
DECISIONS NEEDED:
- Owner: approve or amend the quartet; answer OQ-1..OQ-6 (spec §14).
- Owner/Manager: record locked decisions A–D in Docs/Project/decision-log.md (no entry exists; main tops out at D-093).
- Owner: confirm whether browser tier B becomes mandatory for every UI-touching trunk merge after S0 (recommended: yes).
- Owner: note the measured contradictions with the brief (§4 below), especially that a literal "no RNG prefix is a
  prefix of another" test cannot pass on the frozen keys (OQ-6), that #7/#9 anchors are not code-pinned (OQ-3), and
  that the locked pipeline order differs from today's statement order (OQ-5 / safety valve).
WORKER ASSESSMENT:
Ready for review: Conditional — ready for independent spec review and owner decision; NOT ready for implementation
(DRAFT; T0 not yet delivered).
RECOMMENDED NEXT STEP:
- Route the quartet to an independent non-author spec review, then to the owner for approval and OQ answers. Do not
  create integration/2d-sim or any feat/ branch until the spec is APPROVED and T0 is owner-accepted.
```

---

## 2. Provenance (separate Requested / Selected / Resolved / Executed rows)

An identity that this worker could not observe is recorded as **UNKNOWN**. No Requested value was copied into Selected, Resolved or Executed.

| Party | Field | Value | Source / observation |
|---|---|---|---|
| **Coordinator** (this dispatch) | Requested | Provider `opencode`; model `opencode-go/deepseek-v4.1-flash`; variant `high`; role coordination / read-only | stated in the dispatch brief |
| Coordinator | Selected | UNKNOWN | not observable by the Spec Author |
| Coordinator | Resolved | UNKNOWN | not observable by the Spec Author |
| Coordinator | Executed | UNKNOWN | not observable by the Spec Author |
| Coordinator | Thread identifier | T3 thread `a4bb84b8-fdaa-4b4a-ad3a-bc6aaadcdb26` | from the T3 delivery header of the dispatch message |
| **Spec Author** (this worker) | Requested | Provider `claudeAgent`; model `claude-opus-5-5`; effort `xhigh` | stated in the dispatch brief |
| Spec Author | Selected | UNKNOWN | the dispatcher-side selection record is not visible inside the worker |
| Spec Author | Resolved | Model `claude-opus-5-5` (Opus 5.5); effort UNKNOWN; provider label UNKNOWN | the worker runtime's own system prompt self-identifies the model as Opus 5.5 / `claude-opus-5-5`, running in T3 Code through the Claude Code harness; effort level and T3 provider id are not exposed |
| Spec Author | Executed | Model `claude-opus-5-5` per the same runtime self-report (not independently attested); effort UNKNOWN; harness Claude Code in T3 Code | runtime self-report, the only available observation; it agrees with Requested on model only and is not a copy of it |
| Spec Author | Thread identifier | T3 thread id UNKNOWN (not exposed to the worker). Worktree `C:\Users\hp\.t3\worktrees\CCF-HP\t3-fb8b53c2-0e498524a2a11594` (`t3_worktree_status`: initial branch `t3/fb8b53c2`, project root `\\wsl.localhost\Ubuntu\home\jer\Development\CCF-HP`) | observed via `t3_worktree_status`; distinct from the Coordinator's thread |

Routing note: AgentWorkflow assigns complex architecture to Opus 5.5 and substantive independent review to GPT-6.1 Sol (or a suitably independent Opus 5.5 route). This author's thread must not review or approve this quartet. A DeepSeek V4.1 Flash route is coordination-only and cannot review it either.

---

## 3. Measured facts (read-only Git inspection; not runtime evidence)

Refs observed after `git fetch origin --prune`: `origin/main` = `cc2757a730ad9451e3d6e3680dffd123171a5df5`; base = `5fe2fabe9073d7b72711f4ab48553c785351d7bc` (= `origin/feat/multi-2d-phase1-time-history-core-20261124-v1`); Step 2 = `adea7af220259a1808346cd5381babed3e9052fa` (= `origin/feat/multi-2d-phase1-time-history-ui-20261125-v1`; the base is its ancestor; Step 2 changed only `src/Ui/PlayerMultiApi.cs` under `src/`, plus tests, specs and a handoff).

**(a) `Tools/MultiRackSitkaP1Lab/src/Ui/PlayerMultiApi.cs` @ adea7af**

| Item | Value |
|---|---|
| Size | **7,525 lines**; 438,983 bytes; UTF-8, no BOM, LF only |
| `PlayerApi` partial | `internal sealed partial class PlayerApi`, lines 19–1339 (sibling part `src/Ui/PlayerApi.cs`, 449 lines) |
| Constant holder | `internal static class MultiAreaPage`, lines 1346–7525 |
| `internal const string Html = """` | line 1348; content 1349–2224 (876 lines, 57,612 B); closing `""";` at line 2225, column 0 |
| `internal const string Css = """` | line 2227; content 2228–2911 (684 lines, 57,702 B); closing at 2912, column 0 |
| `internal const string Js = """` | line 2914; content 2915–7523 (**4,609 lines**, 246,563 B); closing at 7524, column 0 |
| JS banners | 17 `// ---- … ----` banners at 3018, 3093, 3272, 3379, 3657, 3823, 3902, 4081, 4638, 4793, 5141, 5230, 5383, 5723, 6125, 6376, 6789 (titles in spec §3.1), after a 103-line prelude; `wire()` 7342–7517; bootstrap 7519–7522 |
| Serving | byte fields at lines 53–55 (`Encoding.UTF8.GetBytes(MultiAreaPage.X)`); routes `multi`, `multi.css`, `multi.js` at lines 73–75; 33 routes in total |

**(b) Protected asset trees:** `Tools/MultiRackSitkaP1Lab/src/Ui/wwwroot/{index.html, app.js, app.css}` (embedded as `SitkaP1.Ui.wwwroot.*`; byte-checked by `ui-smoke.sh` G11b) and legacy `Tools/MultiRackSitkaLab/src/Ui/wwwroot/{index.html, app.js, styles.css}` (P0 tree pinned `f212c8f6…` by `build/build-p0-core-ref.sh`). Both are protected. The new `/multi` tree must be physically distinct; the default is `src/Ui/multi/` (OQ-1).

**(c) `/multi` HTTP surface:**

- `Program.cs:58–64`: empty builder, `Listen(IPAddress.Loopback, 0)`.
- `Program.cs:72–78`: bind check, exit 3.
- `Program.cs:53`: 32-byte capability.
- `PlayerApi.cs:116–192`: guard order is 503 → Host 403 → capability 404 (fixed-time compare) → 308 → route 404 → method 405 → Origin 403.
- `PlayerApi.cs:33–34`, `383–391`: CSP and security headers.
- `PlayerApi.cs:375–381`: fixed-MIME asset writer.
- `PlayerApi.cs:82`: ordinal route dictionary.
- The `/multi` assets are served from in-memory byte arrays built from the constants.

**(d) `src/Core/MultiArea/MultiAreaContinuation.cs`** (1,572 lines; identical at base and Step 2):

- Lock `sync` at line 79; `Advance` 993–1155 (`lock (sync)` at 996); `HasUnexecutedWork` on `MultiAreaRoot` at 34–35; single root commit at 1133–1152.
- The step order in the source is: per-area adult step **with Stage 4 deadwood captured inside each area iteration** → year record **built** → regeneration → one commit that appends the year record. The year record is built before regeneration.
- RNG is stateless and keyed per (key, year, seed).

**(e) RNG key-prefix literals:**

- `"ADULT-MORT-"`: `AnnualRules.cs:87`, `Phase2MixedDensity.cs:108`.
- `"ADULT-MORT-GENERIC-"`, `"ADULT-MORT-GENERIC-QUOTA-"`, `"PHASE2-MAST-"`, `"PHASE2-PJ-"` (+ `/vegetation`, `/browse`): `Phase2Rules.cs:158–163`.
- `"STAGE5-MAST"`: `Stage5Mast.cs:11`.
- `"STAGE5-PROMO-…"`: `Stage5Mast.cs:14–15`.
- The `ADULT-MORT-*` family is nested, so these literals are **not prefix-free**.

**(f) Save v2:** `Phase1SaveFormat.SchemaVersion = 2` (line 20); format `ccf-2d-multiarea-save`; envelope keys `format`/`schemaVersion`/`header`/`payload`/`checksum{alg,value}`; checksum `sha256(header||payload)`. **No committed golden save files exist.** The golden is an in-code pin at `tests/Phase1.Save.Tests/Program.cs:727–738` (24,703 B; file SHA `c0ea8a46…b7bee7`; embedded checksum `9629ac16…6c9086`).

**9/9 frozen anchors:**

- The canonical expected values are listed in `specs/multi-2d-phase1-time-history-core-v1/spec.md` §7. They are **not** in `build/build-p0-core-ref.sh`.
- Code pins exist for #1–#6 and #8 (Core.Tests `internal const` literals; #8 also in `SitkaP1.Core.csproj:7` and `tests/build-output-safety.sh:14`).
- **#7 (Q6) and #9 (MA-1-MIXED 30-year) are printed only** (`Phase2Tests.cs:448`, `:1323`) and asserted only by self-equality.

**(g) Scripts:**

| Script | Mode |
|---|---|
| `tests/ui-smoke.sh` | headless; bash + curl + python3 + node |
| `tests/ui-smoke-multiarea.sh` | headless; hard-codes the classic `multi.js` tag and the `vm.runInContext` loader; holds the payload budgets |
| `tests/phase1-save-ui/run-edge-e2e.sh` | browser: Windows Edge `--headless=new` + Windows node via WSL2 |
| `tests/phase1-time-history-ui/run-step2-edge.sh` | browser, as above; Step 2 only |
| `tests/p0-isolation.sh` | headless; needs strace |
| `tests/build-output-safety.sh` | headless |
| `tests/cli-safety.sh` | headless |

The three safety scripts take a required evidence-dir argument. `evidence/` is git-ignored.

**(h) Conventions:** `specs/README.md` (only APPROVED+ authorises implementation), `specs/_template/*`, and the section-honest quartet `specs/multi-2d-phase1-time-history-core-v1/` were read and followed.

---

## 4. Corrections to the dispatch brief (measured value wins)

| Brief said | Measured |
|---|---|
| "~7,210-line" `PlayerMultiApi.cs` | **7,525** lines (438,983 B) |
| "~4,590 lines of JS" | **4,609** lines (246,563 B) |
| constants belong to `internal sealed partial class PlayerApi` | they belong to **`internal static class MultiAreaPage`** (1346–7525) in the same file; the `PlayerApi` partial (19–1339) holds routes and handlers |
| Advance step order AdultGrowth, Deadwood, Regeneration, YearRecord | that is the ecological / data-dependency order; the **statement** order interleaves deadwood per area and builds the year record before regeneration → relocations M1/M2 and the safety valve (spec §6.2–§6.3, OQ-5) |
| 9/9 anchor list in `build-p0-core-ref.sh` and/or tests | **not** in that script; 7/9 code-pinned in tests (+#8 csproj); #7/#9 printed only (spec §3.8, OQ-3) |
| "committed v2 golden saves" | **none committed**; the golden is an in-code pin (spec §3.7, OQ-3) |
| one allow-listed `NewSessionId` site | the brief's tokens have zero hits; there are **four** entropy/clock sites (spec §3.6) |
| RNG prefixes unique and none a prefix of another | the frozen `ADULT-MORT-*` family is nested; changing it would move RNG streams and anchors (OQ-6) |
| ES split along existing sections | a plain section-by-section split would create import cycles; a `hooks` registry is proposed (spec §7.2 P4) |

---

## 5. Explicit statements

- **No code, no tests, no builds, no servers, no self-approval.** No `.cs`, `.csproj`, `.sln`, `.sh`, `.html`, `.css`, `.js` or JSON file was edited. No `dotnet build/test`, gate script, HTTP listener or browser was run. Nothing was installed. This author does not approve or review the quartet.
- **Coordination:** `Docs/Project/coordination/active-tasks.md` was **not** edited, and this docs branch is **not** registered there. The Coordinator observed the live file at `origin/main` `cc2757a730ad9451e3d6e3680dffd123171a5df5` with no S0 seam-extraction entry. That is a read-only observation, not an authorisation gap invented by this author. The Spec Author's own read-only check at the same SHA found no `2d-sim`, `seam-extraction` or S0-seam entry.
- **Write surface:** exactly the five files listed under FILES CHANGED, on one docs branch, in one commit.

**DRAFT QUARTET PUBLISHED — AWAITING OWNER APPROVAL**
