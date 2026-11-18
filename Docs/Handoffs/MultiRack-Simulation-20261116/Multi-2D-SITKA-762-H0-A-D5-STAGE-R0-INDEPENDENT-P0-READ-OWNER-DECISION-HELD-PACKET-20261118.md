# Multi #762 — D5 Stage R0 narrow independent P0 pin read: HELD pending owner/security consent

**18 November 2026 · Manager #730 · issue [#762](https://github.com/jermur788/CCF/issues/762) HELD / NOT RUNNING · Decision packet, not a read authorisation.**

**Difficulty: High:** accessing protected P0 objects without violating legacy ownership and avoiding a false whole-P0 integrity claim. **Preferred worker once separately granted:** new independent OpenAI Codex Sol High native T3 Code session on `jer-laptop`, excluding Claude Opus #756/#759 author, existing #674/#687/#697/#725 custodians, future H0 writer/packager and later reviewer. No real worker assigned/registered yet; manager does not infer its session, host or permission.

## Binding evidence

- [#759 complete physical authority Phase0 report](https://github.com/jermur788/CCF/blob/5d7d1ba2c0cee6e693b03c00e1b0de3e2b706b5d/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-759-H0-A-D5-P0-PHYSICAL-READ-AUTHORITY-PHASE0-RESULT-20261118.md), [Manager accepted Phase0 but kept D5 blocked](https://github.com/jermur788/CCF/issues/759#issuecomment-6987554907).
- [#756 proposed contract and D5 pre/post procedure](https://github.com/jermur788/CCF/blob/7aaa7a90fd5134850f3eeda5696410e20c82f286/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-756-H0-A-FROZEN-CONTRACT-AND-D5-PROTECTION-PREFLIGHT-DRAFT-RESULT-20261118.md); [#757 independent NEEDS CHANGES](https://github.com/jermur788/CCF/blob/docs/multi-730-757-independent-contract-review-20261118-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-757-INDEPENDENT-GPT6-TARGETED-H0-CONTRACT-REVIEW-RESULT-20261118.md).
- [#755 confirmed H0 local-only physical setup](https://github.com/jermur788/CCF/blob/098928f0fe1a17356268dd862bd9491e7afdaca4/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-755-H0-A-PHASE1-G1-LOCAL-ONLY-PHYSICAL-LANE-SETUP-RESULT-20261118.md), current physical CCF-Ops authority and D-091 no arbitrary lease expiry.
- Owner-approved P1 spec §9.2 P0 isolation/L0–L4 and published original P0 pinned committed subtree. Default L4 .NET **FAIL38 UNWAIVED**.

## Stage R0 — precise candidate minimal read, NOT YET PERMITTED

**Owner/security decision R0-YES/NO required first.** Suggested grant scope (future, actor-specific and recorded separately by Manager):

- **Canonical object read (G only):** `/media/jer/Files/CCF/Repository.git`, using `GIT_OPTIONAL_LOCKS=0` plus read-only `git --no-optional-locks cat-file`, `rev-parse`, `ls-tree`, `for-each-ref`. Only the original pinned P0 commit `a7f41983c2f2f233b8fe999d1a5b10e72bdd2866`, subtree `Tools/MultiRackSitkaLab` at tree `f212c8f64fb28b83aa870c7b5b7c72e03051e46a` (52 tracked Git blobs, reported 777,548 B), and ref-set hash for drift monitoring. Permission concerns *read-only committed objects/metadata* and cannot be expanded to working-tree files. Do not fetch/rewrite canonical refs. Object IDs and path/mode/size inventory are sufficient for R0; no need to publish private source text.
- **Isolated H0 store (G only):** `/media/jer/Files/CCF/Operations/CCF-H0A-Year0Viewer-20261118/git/CCF-H0A-Year0Viewer.git`. Read-only test that the pinned original P0 commit object is absent and both registered empty H0 branches have no protected original-P0 subtree. Git store remains remote-less, with hardening unchanged. Absence alone does not prove any separate physical tree safe.
- **Ops records (O only):** SHA-256 of live `/media/jer/ZX20/CCF-Ops/board/active.md`, `lanes.json`, `deployment.json` to reconcile scope/owners without mutation.

**Not in R0; requires separate R1 grant and actual tree-holder consent:** #639 physical worktree under `/home` (unregistered custody), #687 exclusive worktree, #674 dirty worktree with elapsed advisory date. No `stat`, traversal, per-file SHA, content or ignored-file read of them in this task. #697 excluded by default; #710 and #725 fully protected/excluded. No installed Ops scripts, source tests, Dotnet/Node/Unity, no source change.

**No production source rights.** New H0 Core and Viewer remain Manager-reserved, not running; #758 contract correction separate. R0 is a *narrow observation* and cannot satisfy final D5 physical pre/post proof alone.

## Before assigning actual worker: mandatory gates

1. **Owner/security explicitly approves or rejects R0 read scope**, separate from product H0 approval. Do not infer owner consent from public GitHub objects being accessible.
2. Manager checks current physical board and existing read claims, appoints **one concrete independent actor/session** and publishes an exact G1 read-only grant with copied path/command allowlist, scratch/evidence limits and no expiry D-091. If identity is unknown, DO NOT issue a generic transferable read grant.
3. Worker registers real provider/model/harness/host/session and fresh main and Ops prehash as `[RUNNING — #762 R0]` before any admitted physical read. Physical no-conflict and any physical lease-holder objections take precedence; STOP if changed.
4. Read only the named source objects/metadata, produce no copies of committed source outside approved ephemeral read context and publish a **hash-only** one-file GitHub report, independent self-report of any inability to read or mismatch. No open of B-3 candidate physical worktrees, no P0 write proof from R0. Future R1 physical pre/post baseline and owner/security **JS-only protection substitution** both remain separate OPEN decisions.

## Future result target only after owner/security + actor-specific grant

`Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-762-H0-A-D5-R0-INDEPENDENT-PIN-AND-ISOLATED-STORE-READ-RESULT-20261118.md`, from a new docs-only branch created at fresh main; exactly one new Markdown, GitHub readback/compare, `[HANDED-OFF]` #762 and Manager #730, issue left open for verdict. If not authorised, NO worker should execute R0; no "RUNNING" merely for this publication. G2 still HELD.

**Owner decision requested:** Do you approve a new, independent, named native custodian making the *R0 read-only Git-object and Ops-metadata checks above*? Approval of R0 does **not** grant R1 physical worktree reads, JavaScript verification substitution, local source implementation, public source publication, Unity/Windows/Host, main integration or an L4 waiver.

**Exact future dispatch line (NOT VALID UNTIL ACTOR GRANT):**

> Take CCF Multi #762 as independent D5 R0 READ-ONLY P0 committed-object and isolated-store custodian. First read full Manager GitHub packet https://github.com/jermur788/CCF/blob/docs/multi-730-762-d5-r0-owner-decision-held-packet-20261118-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-762-H0-A-D5-STAGE-R0-INDEPENDENT-P0-READ-OWNER-DECISION-HELD-PACKET-20261118.md . **STOP and do no native P0/store read until explicit owner/security R0 approval AND a separate actor/session-specific Manager read grant are both linked on #762.** Register actual worker, fresh main/physical Ops baseline [RUNNING] only after those gates. Then obey exact G-only/O-only path/command allowlist, publish one hash-only GitHub result and [HANDED-OFF] #762/#730. No #639/#674/#687/#697/#710/#725 contents, physical worktree traversal, source, fixture copy, code/test/build, Ops mutation, Unity, Windows, Host, public production source or L4 waiver.

**Dashboard/Drive:** not verified.
