# Multi — Manager 2 verdict and bounded #566 DRAFT repair packet

**Issued:** 16 November 2026. **Claim:** [#575](https://github.com/jermur788/CCF/issues/575) READY / NOT RUNNING. **Scope:** *standalone 2D Multi-Rack Sitka P1 design only*, explicitly not Scenario One #424/#544, not the existing P0 lab fix #567. **Author of this packet:** OpenAI ChatGPT GPT-6, GitHub/browser-only Manager; no local worktree, Unity/Editor, source modification or physical runtime claims.

## 1. Reviewed evidence and formal Manager disposition

- Opus 5.5 #562 source-grounded design handoff: [original](https://github.com/jermur788/CCF/blob/81470c5955c781752148dae4d6240483c15e3ee7/Docs/Handoffs/MultiRack-Simulation-20261114/Multi-Sitka-Annual-Growth-Technical-Contract-Resolution-Handoff-20261115.md); candidate contains four modified DRAFT files plus one added handoff.
- Independent nonauthor Sonnet 5.5 #566: [report](https://github.com/jermur788/CCF/blob/7bca1903227832d7097f41ad9ce8487e5d42b346/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-Sitka-Annual-Growth-Technical-Proposals-Independent-Review-20261116.md) and [claim](https://github.com/jermur788/CCF/issues/566). Registration before inspection, publication exactly one ADDED Markdown vs the inspected #562 candidate, report read back from GitHub. Effort High is user-stated, not independently observable.
- **MANAGER VERDICT: APPROVED AS DRAFT QUALITY**, **NOT** the P1 SPEC, implementation, data or runtime. Independent T-01…T-19 grades: 16 PASS, 3 PARTIAL (T-01 lineage, T-08 archive/caps and T-16 cross-platform save aspects), zero FAIL and no BLOCKER or MAJOR. The reviewer identifies three MATERIAL documentation residuals F-1/F-2/F-3 and twelve MINOR m-1…m-12.
- Carry T-02…T-07, T-09…T-15, T-17…T-19 as **working design proposals**, contingent on minor edits and later complete-spec approval/verification, not accepted mechanics. T-01, T-08 and save-side T-16 **not adoptable yet**. All four spec documents stay **DRAFT — NOT APPROVED**; C-2b numeric parity and C-6 Unity exclusive time **OPEN**; all **72 requirements unchecked**, N01–N13 **NOT RUN**. No code, compiler, Unity, fixture capture, runtime, lease or Windows build is released.

### Bounded Manager working rulings (not product/spec approval)

1. **Read-only reflection:** admissible as a *proposed verification-only method*, subject to **all four** #566 §5.5 safeguards: (a) only after `AdvanceOneYear` returns; (b) resolve `FieldInfo` exact names once at harness start, source-blob pin, fail closed if absent; (c) copy results sorted by ordinal `TreeId`, never enumeration order; (d) R1 observed versus R0 unobserved bit-exact outputs mandatory. No permission to execute or alter Unity, nor to treat reflection values as independently verified before a real run.
2. **P0 source working reference:** choose the last independently accepted README-only P0 lineage `861b81d3540429d241bba0732a610212626f6828` for *documentation comparisons* (source and fixtures were identical to #562's older `2659db99` at #566's inspection). This does **not** select a permanent P1 implementation branch. Following independent review/disposition of #567's expected P0 Host change, Manager must explicitly choose/re-pin the implementation base; never float to the newest P0 branch automatically. Document the choices and the dependency in C-1.
3. **Portability:** do not promise cross-host save load. The conservative *proposal* is a fingerprinted, fail-closed **distinct platform-mismatch refusal** that preserves the file intact and never misreports mismatch as corruption/tamper. Whether P1 should support compatible cross-host import/replay is an **OPEN owner/Manager policy choice**; leave explicitly unresolved rather than implicitly approving portability or runtime specifics.
4. **Capacity:** keep 5,000 trees, 200 years, 200,000 events and 64 MiB as **unvalidated design proposals**, not accepted supported limits. 200-year experimental need, event/byte consistency, event hashing costs and the achieved replay-time budget remain open until a defensible derivation and measurement gate are supplied. No new arbitrary numeric substitute or silent cap change.
5. **File durability:** retain `fsync` and root owner/mode checking as **reviewer-supported proposed P1 additions**, subject to exact opened-handle and POSIX-filesystem constraints and independent security/runtime qualification; do not represent them as proven P0 properties.

## 2. Author task: targeted technical corrections, not renewed design exploration

**Classification:** PACKET ONLY — four-file DRAFT documentation repair. **Difficulty MEDIUM–HIGH** (complex, cross-referenced replay/float/lineage security semantics but no local code/Unity). **Preferred worker:** Claude Opus 5.5 Medium, original #562 author, if actually available; no promise of subscription quota. Alternative capable source-literate Code worker only after exact #575 re-registration, with one writer. **Review:** focused nonauthor **Sonnet 5.5 High** (the #566 critic) preferred; review ONLY the changed findings and their neighbours, do not repeat the 19-item full source audit unless corrections warrant. Max two bounded review/repair cycles then stop with explicit blocked items.

### F-1: archive caps, replay cost and testable budget (MATERIAL, T-08)

Repair `spec.md` §4.1/§9.3, `plan.md` N02/N08 and their `tasks.md`/requirements cross-references.

- Label `MaxEvents=200,000` and `MaxFileBytes=64 MiB` as P0 existing constants, not P1-derived values; distinguish the other proposed caps. Compute a **documented, reproducible worst-case canonical event byte envelope** including command/event IDs, four 64-character digests, JSON keys and payload; compare reachable event count to the byte cap. If no rigorous bound, mark quantitative event cap **UNDECIDED** rather than invent one.
- Account for **every per-event full-state hash/re-serialization** during full replay (P0 `LabSession.cs:131`) as well as tree-neighbour/Hegyi pairs. The reviewer found an order-of-magnitude replay cost potentially much worse than the current formula. Mark reviewer estimates **unmeasured**, and add a mandatory bounded P1-03 synthetic load benchmark with size/event-count/per-tree-state metrics and an explicit refuse/abort strategy instead of a blocking long synchronous request.
- Reconcile §4.1 “20–40 bytes per tree-year” against §9.3 `200·N₀ + 650·Y + 120·D + W`: keep one carefully explained canonical compact-size model or mark formula as uncertain. Preserve distinct full archive, compact archive and nonauthoritative trace semantics; do not quietly invent incremental-hash semantics as accepted design.
- Tie `MaxYears=200` and `MaxTrees=5000` to a named experiment or flag for owner/Manager choice; never call 200 years empirically required. Preserve rejection-before-write/atomicity and the need to measure runtime.

### F-2: math-library/platform replay and provenance (MATERIAL, T-08/T-16)

- Explain exactly that `Math.Exp`/`Math.Pow` are used by the growth source; different runtime/library versions may produce different bits, **not verified here**. This affects not just Unity capture but P1 **replay-on-load and hash checks**. Require an exact saved/verified platform fingerprint schema proposal (OS/architecture, libc/libm identity+version where available, runtime/.NET/Mono build, CPU feature class, model+source and archive versions). Any unavailable field must fail closed or be transparently specified; never fabricate a measured libm identity.
- Add a **distinct platform mismatch refusal class** separate from wrong-format/tamper/corrupt, with user-facing non-destructive message and preserved bytes; spell out how missing or different fingerprint is treated. Cross-host save portability remains OPEN: give the owner a compact choose-between-options decision and minimum tests; do not hard-code an unapproved cross-host policy.
- Explicitly state **Linux Unity Editor observation is only Linux Editor evidence; it cannot prove Windows Unity Player parity**. C-2b remains OPEN pending actual source/host/Unity captures; no numerical acceptance without evidence.
- Enrich N08 and capture N03 positive/negative tests: same-environment replay; altered library/fingerprint; changed raw bits; clearly reported mismatch rather than corruption; old/unknown versions; no unsafe partial in-memory apply or disk overwrite.

### F-3: P0 sibling lineage and non-mutating proof (MATERIAL, T-01)

- Correct stale “no newer P0 head” text; working reference is qualified README-only `861b81d3`. #567 P0 Host repair has separate ownership and may advance a branch; later implementation base must be re-pinned **explicitly by Manager after independent #567 disposition**, never silently `main` or floating latest.
- Replace false sufficiency of `git diff --quiet <P0 pin> HEAD -- Tools/MultiRackSitkaLab` with layered **pinned commit tree/content comparison**, **fresh `git status --porcelain --ignored -- Tools/MultiRackSitkaLab`** (with documented interpretation of pre-existing ignored files rather than wholesale deletion), plus build-output isolation so a P1 `ProjectReference` does not generate P0 `bin/`/`obj/`. If branch/worktree dirty or output cannot be isolated, STOP and report; never clean user files.
- Address sibling P1's own SDK/build files (its own `global.json`, `.gitignore`, root `.gitignore` re-includes for deliberate `.csproj`/`.sln`) and state which comparison tests commit bytes vs observed physical writes. Require exact before/after P0 tree and working-tree checks, no edits to accepted P0 tracked/untracked/ignored state.
- Keep P1 a sibling/tool, **not a mutation of qualified P0**; maintain P0 public API reference boundary and the qualified WSL2 no-growth baseline.

### All twelve reviewer MINOR corrections (required, not optional)

| ID | Exact correction expected |
|---|---|
| m-1 | Distinguish client envelope `expectedForestRevision` mismatch (refusal) from **plan-base** `RevisionMismatch`/`EpochMismatch` (recorded audit-only rejection); preserve golden-vector semantics |
| m-2 | P0 **runtime** does not read `MANIFEST.sha256`; P0 file-format unit test **does** read it. Trust still cannot rely on the co-located manifest |
| m-3 | Record `ForestEcologyController.timeLapseEnabled == false` as capture precondition; correct the misleading `CropTreeCompetition` caller reference/comment-only distinction |
| m-4 | Correct line citations for `KillDensityVictims` and hazard loop per §7 of review |
| m-5 | Roll year is **post-increment** `ecologicalYear`; RNG seed is source `simulationSeed`, not the capture's own seed |
| m-6 | Capture controller's actual `rngModelVersion` rather than assuming model 1; preserve model 0/1 distinction |
| m-7 | Clarify exact Sterbenz/float cancellation range and that store-precision hazard is practically limited to very young ages (not fixture age 28); keep small-age vector |
| m-8 | Restrict proposed owner/mode/fsync promise to qualifying local POSIX filesystem; refuse DrvFs/9p/network as unsupported where checks unreliable; use `fstat` on the **opened root handle** |
| m-9 | Reconcile two contradictory compact-size models; may share F-1 correction, but explicitly verify both locations |
| m-10 | R2 instrumented copy shares **Editor binary/host**, not the same running Editor session/project |
| m-11 | `MortalityApplied` order is **application**, not evaluation of S/p; footnote/rename `observedAtEvaluation` accordingly |
| m-12 | Client mints `commandId`; an intentional new click has a **new ID**, only a transport retry repeats it; G-02 must reject reused recorded IDs |

## 3. Decision and preservation register

Preserve four files' **nine matching C-register rows** and their owner-approved C-1A/C-7 status; if a needed policy would change a row, **DO NOT** silently alter it. Instead add an explicit **proposed Manager/owner decision** with alternatives, evidence, risk, cost and what work it gates. No new D-082 numbering or changes to accepted D-082 (original verified synthetic input, conditional stale-on-load, synthetic-not-forecast disclosure).

Outstanding after this task: (1) final implementation P0 base after #567; (2) cross-platform save portability (default refuse proposal, owner choice); (3) empirical replay/byte caps and 200-year need; (4) fixture trust-table values; (5) later C-2b Unity numerical proof and C-6 machine booking; (6) remaining unpublished field/GROWFOR evidence. The reviewer-supported reflection contract can be written into §9.5 as conditional proposal but does **not** close C-2b or authorise a Unity run.

## 4. Exact lane, ownership, allowed files and stop conditions

**Existing reviewed content baseline:** `81470c5955c781752148dae4d6240483c15e3ee7` (four-file #562 DRAFT). The #566 report is on a separate review branch `review/multi-sitka-annual-growth-technical-independent-20261116-v1`, read-only reference. **Proposed author branch:** `docs/multi-sitka-p1-566-residuals-repair-20261116-v1`, created from that baseline **only after fresh checks**. **Physical lane/worktree:** a new, isolated `/media/jer/Files/CCF/Operations/CCF-575-SitkaP1DocsRepair-20261116` **proposed only, not created or leased**; worker must verify mount/space/current branch/worktree/claims and substitute a safe new path only if uniquely registered. Worktree, branch and provider account are NOT RUNNING.

**Exactly five tracked writable paths:** the four existing `specs/multirack-sitka-p1-adult-rules-comparator-v1/{spec.md,plan.md,tasks.md,checklists/requirements.md}` and one new `Docs/Handoffs/MultiRack-Simulation-20261116/Multi-Sitka-P1-566-Residuals-Documentation-Repair-Result-20261116.md`. All production `Assets/**`, `Tools/**`, P0, fixtures, `main`, canonical Decision Log/current milestone/board, unrelated working files, read-only Source/Reference, #565 ledger are outside scope. No code, tests, Unity/Editor, save/prefs, local machine lease, source compilation, Windows build or unapproved experiment.

**Before reading/edits** check live `main:Docs/Project/coordination/active-tasks.md`, branch/worktree and #575; post **[RUNNING]** with actual model/provider/effort, account, host, exact full starting HEAD, observed free ownership and exact file list. Task claim alone is not live lock authority. **STOP** on conflict, unapproved cross-host policy becoming necessary, inability to preserve P0, missing source facts, or deviation outside four Markdown files; do not choose arbitrary numbers or write production code.

## 5. Quality gate, handoff and next task

Deliver the exact **five-file diff** and GitHub readback; an F-1/F-2/F-3 + m-1…m-12 disposition table with amended section anchors; a clean T-01/T-08/T-16 evaluation; four-file consistency audit, nine decision rows unchanged, exactly 72 unchecked requirements, N01–N13 still unrun, and explicit decision queue / any honestly unclosed item. Supply source arithmetic and assumptions, but label estimates vs observations and no project test as executed. Report dashboard/Drive **NOT VERIFIED** unless actually checked. Mark [HANDED-OFF] on #575, leave open for Manager verdict.

**Next gate:** Manager checks this exact returned documentation delta and commissions **one focused independent nonauthor review**, preferably Sonnet 5.5 High, against the F-1/F-2/F-3 and twelve-minor corrections only. No repetitive 19-item full audit without evidence of changed semantics. Only after that may Manager decide remaining owner policies and consider full-spec approval, never automatic implementation.

**Paste-ready worker dispatch:** Take ownership of CCF #575, a docs-only repair of the three material and twelve minor residuals from independent Sitka P1 review #566. Binding packet: https://github.com/jermur788/CCF/blob/docs/multi-sitka-p1-566-manager-repair-packet-20261116-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-Sitka-P1-566-MANAGER-DRAFT-QUALITY-VERDICT-AND-REPAIR-PACKET-20261116.md. Register actual worker with branch/worktree/full start HEAD, protect P0 and all other lanes, edit only four existing DRAFT spec files and specified result Markdown, preserve all open decisions/72 unchecked/N01–N13 unrun. No code/Unity/production, no invented numeric caps, no spec approval. Publish GitHub result and [HANDED-OFF] for Manager review. Preferred actual model Claude Opus 5.5 Medium, if accessible. User action: launch this single author worker; no additional product decision needed for the bounded documentation pass.
