# Sen-1 #731 — Manager review of #733 and bounded #652 no-expiry source continuation

**MANAGER VERDICT: #733 APPROVED for READ-ONLY AUDIT QUALITY. Installed Ops no-expiry eligibility: NEEDS CHANGES.**
**#652 ORIGINAL SOURCE WORKSTREAM: source-only continuation is authorised CONDITIONALLY; LIVE INSTALL HELD. #718 Unity HELD.** 2026-11-18.

## 1. Exact report reviewed

[Actual #733 Sol result](https://github.com/jermur788/CCF/blob/cfc9ba4f381fdd78639697bc9e7180dec70ff0be/Docs/Handoffs/ScenarioOne-Operations-Readiness-20261118/Sen-1-733-SOL-READONLY-OPS-NO-EXPIRY-REAL-CONTRACT-RESULT-20261118.md). Author registered actual OpenAI/Codex gpt-6.1-sol medium, native T3 Code on jer-laptop; documentation from clean GitHub branch. Original physical Primary had two tracked dirty files and one untracked Ops document which the worker preserved. No source/Unity/installation mutation, no physical worktree custody reassignment. This Manager reviews the author's reported physical evidence; does not independently execute machine checks.

**Accepted inspection findings:**
- Live installation is a copied `/media/jer/ZX20/CCF-Ops` tree, lanes config schema 1, board 17-column table. The installed `_ccf.py` and board lint do **not parse or enforce** free-text lease expiry; adding a null to unsupported lane metadata does not change custody.
- Actual `unity-lock` acquisition requires a future ISO `--eta`, and `read_state` requires nonempty timezone-aware timestamp. Inspect reports elapsed overdue, but never automatically releases or kills. Existing owner/lock ID/PID+boot+start-ticks and single machine-wide guard remain meaningful.
- Original `packet-lint --board` exit0 on **42 tasks** and existing #690 `lane-status --preflight` exit0, exact target predicates READY, warnings empty, conflicts empty. These are **historical snapshots** for the inspected state, not Compact30 admission, modified candidate pass, or future installation authorisation.
- Current installed `unity-lock status` was **NOT run** because the command may create guard directories/file on read. Raw lock absent, no Unity PIDs observed. Original full self-test **NOT run**. Do not claim Unity lock lifecycle or source tests PASS.
- Existing #690 physical board row was **BLOCKED / source start HELD**, notwithstanding its prior GitHub [APPROVED TO START — CONDITIONAL] message. Its older advisory date is NOT an active licence extension. #710/#725/#732 Multi physical rows are retained under separate manager; no grant to rewrite them.
- The Compact30 worktree `/media/jer/ZX20/CCF-Sen1-718-Compact30-Content-20261117` was **absent from actual lanes.json**; no Compact preflight, lease, scene worker, Editor run or Windows build took place.

**Verdict detail:** report is precise and grounded; distinction between persistent ownership and mandatory ETA acquisition is reproducible from inspected script/contract and bounded. **APPROVED inspection, not repair**. An explicit non-expiring acquisition mode with safe legacy read and fail-closed corrupt metadata is needed before owner D-091 can be met.

## 2. Manager source continuation approval — SAME #652 claim (no parallel competing Ops writer)

The existing [#652 comprehensive controlled source+review+deployment packet](https://github.com/jermur788/CCF/blob/docs/sen-1-652-end-to-end-ops-readiness-packet-20261116-v1/Docs/Handoffs/ScenarioOne-Operations-Readiness-20261116/Sen-1-652-END-TO-END-UNITY-OPS-READINESS-AND-CONTROLLED-DEPLOYMENT-PACKET-20261116.md) remains the **single writer workstream**. It previously approved a source ceiling containing `CCF-Ops/scripts/unity-lock`, tests, README, role docs where separately authorised, and other files for its distinct existing policy repairs. This amendment adds **one explicit new source feature scope** — owner D-091 no-expiry Unity lock acquisition. Do NOT dispatch a second conflicting Ops source author/branch against #652.

**Best suited:** actual native GPT-6.2 Sol High / Codex Linux Ops maintainer, **HIGH** difficulty because safety-sensitive Python CLI, backward-compatible lock state, concurrency/negative tests, retained previous #652 review findings and controlled physical deployment. Actual provider/model/host/session must be registered, not assumed.

**Source lane/branch/worktree:** retain the existing #652 isolated source branch `ops/sen-1-652-unity-lease-readiness-20261116-v1` and physical location `/media/jer/Files/CCF/Operations/CCF-OpsReadiness-652-20261116` **only if** current custody, clean source status, exact ancestry and legal Files CCF storage are reverified. Do not create a second source writer. Existing #652 physical row and permissions must be refreshed, and if scope expansion is not physically recorded safely under the canonical board flock, mark BLOCKED. No source mutation before actual issue [RUNNING] registration with session, host, starting HEAD and exclusive physical source lane; any current writer conflict or dirty unrelated work STOP.

**No-expiry change source ceiling for THIS sub-feature:**
1. `CCF-Ops/scripts/unity-lock` — explicit `--no-expiry` acquisition, mutually exclusive with optional legacy `--eta`; store `eta:null` **only if set by new explicit option**; status/inspect accepts a legitimate null for a valid lock; legacy timestamp, advisory overdue display retained.
2. `CCF-Ops/tests/self-test.py` and narrowly scoped `CCF-Ops/tests/fixtures/**` **only if proven necessary** — test full acquisition/release legacy+null modes and negatives under isolated approved scratch.
3. `CCF-Ops/README.md`, `CCF-Ops/roles/unity-operator.md` if existing role guide needs direct correction — document explicit release, no timer reclaim, real PID/boot/start identity and actual Editor exit prerequisite.

No edit to `CCF-Ops/scripts/_ccf.py`, `scripts/lane-status`, `scripts/packet-lint`, `lanes.json`, `deployment.json` or `environment.sh` merely to implement no-expiry. Those files are retained under prior #652 source-ceiling for other evidenced #652 findings, but any modification must be separately justified by those existing findings, with exact negative tests. **No broad schema/path/storage changes, Project source, Unity files, saves, old worlds, logs or live Ops files.**

**Semantics and failure paths:**
- For a valid new lock, `eta:null` means **indefinite until explicit safe release**, not auto-extend or forgotten owner. Explicit `--no-expiry` is required to create it; missing eta without valid mode remains malformed and fail-closed. Avoid conflating missing field with valid null.
- Legacy valid timestamp locks remain readable; legacy overdue locks remain held, never reclaimed. Corrupt/missing essential keys, stale/reused/unknown PID and mismatching owner/task/lock ID remain refusal or evidence-only, not silent unlock.
- Only one machine-wide Unity lock; retain OS flock/guard, store/path/branch cleanliness, disk UUID/fstype/capacity, two-store conflict negatives, no force bypass. Status potentially writes guard even when read-only-looking; tests must use legally authorised isolated state/root with no production effects.
- Normal release must be allowed only for matching task/owner/lock ID **after confirming no live Unity Editor/process**; if existing release script does not enforce this, either add a conservative no-release-while-Editor-live check within permitted `unity-lock` scope, or explicitly require independent live-process verification in operator procedure and tests, with a separate Manager safety ruling if the defect risks lock transfer. Do not silently broaden force semantics.
- The board's DONE-PENDING-LEDGER row still occupies duplicate lane/branch counts until its assignment fields are deliberately reconciled under lock. Event-driven release uses supported board states or ledger + cleared assignment fields. Never introduce a schema state that original validator rejects; no stale-heartbeat reclamation.
- Do not auto-migrate live legacy locks or touch #690/Multi lease dates; preserve owner custody and existing retained source. Do not borrow their lease or stop their processes.

**Qualification:** run complete original self-test with no weakening; explicit new null-mode acquire/status/reject second/release, malformed/missing fields, wrong owner/task/ID, retained legacy overdue, real concurrent two-process guard, reused/stale PID, storage root/symlink/UUID/device/fstype/capacity and old #652 Files non-Unity/default-reader negatives. Source-only result is not installed. If tests are red, continue bounded repair under #652 if safe, or STOP on scope conflict. Publish actual command, return codes, complete test counts, hashes and before/after diff. Different model-family **independent nonauthor review APPROVED** on pinned source candidate before *any* deployed change, checking correctness and safety.

## 3. Deployment must be a distinct physical permission and evidence gate

**Not granted by this packet:** copying candidate into `/media/jer/ZX20/CCF-Ops`, changing physical board/lanes/lock/history, installing controlled #652 Ops scripts, running Unity, creating Compact30 production branch or Windows. Only after approved source review may Manager inspect full live installer plan, exact candidate provenance, physically registered **sole Ops installer**, no concurrent Unity/foreign owner/lock, board flock + original lint, current storage/device checks, backup/readback/rollback and null-lock compatibility before giving explicit **[APPROVED TO DEPLOY OPS]** for that specific installer. A prior source-approved #652 is NOT deployment approval. No blanket "fine" can replace exact physical readiness evidence.

Migration rule: current ownership stays held, no rewriting old lock JSON in place. Next valid acquisition may choose null mode only after reviewed deployment. If rollback would make a new null lock unreadable by old scripts, **do not roll back under active null custody**; first arrange safe explicit ownership release after process verification or an independently reviewed compatible fallback. Preserve logs and other board rows.

## 4. Compact30 Tier S return-to-production sequence

1. #652 actual Ops source author finishes reviewed candidate and nonauthor review without breaking original tests.
2. Manager separately authorises reviewed deploy using actual physical readback; installer completes live deployment and verifies original lint, preflight, lock null-mode lifecycle and rollback (without acquiring a real project Unity lock except in separately authorised isolated test).
3. Complete latest [W §12 bounded Tier S grant](https://github.com/jermur788/CCF/blob/docs/sen-1-731-chas-manager-intake-20261118-v1/Docs/Handoffs/ScenarioOne-Compact-Ecology-20261118/Sen-1-731-COMPACT30-TIER-S-W12-OWNER-GRANT-FILLED-SELECTIONS-PENDING-SIGNATURE-20261118.md) with exact non-expiring event-driven custody, provisional A8/C0, natural provisional ground, exact seven paths, actual scene author/asset dependency/rights receipts; obtain owner **personal signature** and focused nonauthor review of **filled** grant. Source rights D-087/088/089 already decided; don't re-ask them.
4. One sole Compact scene producer registers actual Sol model/session/ZX20 worktree/branch/pinned source and physical board row under exclusive flock. Run original untouched board lint and mandatory preflight **READY exit0, zero warnings**; acquire newly reviewed machine-wide Unity no-expiry lock only with actor-specific Manager [APPROVED TO START — CONTENT BYTES ONLY].
5. Create exactly five permanent new scene/placement/meta files and two temporary local-only Editor script/meta files; audited removal, tool-free import, readback and different-worker source content review. Existing historical source, saves and Windows forbidden. World remains unsigned until separate actual Tier H/J/whole-world native gates.

## 5. Current handoff status

- #733: **APPROVED — READ-ONLY AUDIT**; its issue can be marked HANDED-OFF/CLOSED after referencing this verdict. This does not approve physical compatibility.
- #652: Existing sole Ops source workstream **SOURCE CONTINUATION CONDITIONAL**, not RUNNING or deployed. Actual model/session and Git/physical custody required.
- #718: HELD, no scene source/Unity.
- #690: Separate Claude save lane currently reported BLOCKED/HELD by #733 physical board; do not infer source start from old conditional GitHub permit. #730 Multi separate owner.
- Dashboard/Drive sync NOT VERIFIED; no published status claim.

**Next dispatch (to sole #652 original-source worker or new replacement with proven handoff):**
> Take Sen-1 #652 as the single sole Linux Ops source writer for its existing end-to-end readiness workstream. Follow the full original packet and the Manager's binding D-091 amendment at https://github.com/jermur788/CCF/blob/docs/sen-1-731-chas-manager-intake-20261118-v1/Docs/Handoffs/ScenarioOne-Operations-Readiness-20261118/Sen-1-731-MANAGER-733-REVIEW-AND-652-NO-EXPIRY-OPS-SOURCE-CONTINUATION-20261118.md. Verify current live source ownership/physical row, clean existing isolated Files CCF Ops branch/worktree and exact starting HEAD; register YOUR actual model/provider/session/host before any edits. Implement explicit --no-expiry Unity acquisition with backward-compatible null semantics plus complete original and new negative tests, within the exact allowlist, then publish source handoff for independent nonauthor review. Do NOT install or modify live ZX20 Ops, Unity, game source, other workers' leases or Windows. STOP on owner/physical conflict.
