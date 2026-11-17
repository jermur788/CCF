# Multi #674 — Manager amendment: fixed-snapshot scratch proof (no live-main freeze)

**Manager #641, 17 November 2026. APPROVED AS A NARROW CHANGE TO THE #674 PROOF-ONLY STOP POLICY.** Supersedes only the automatic STOP for unrelated live `main` movement after an exact target has been selected. It does not broaden executable source scope or approve integration.

## Why

The assigned independent `opencode-go / DeepSeek V4.1 Flash` worker properly stopped after `main` moved twice during the execution window. The second movement included **D-085 canonical decision/milestone changes**, not merely live task coordination. [#674 exact STOP receipt](https://github.com/jermur788/CCF/issues/674#issuecomment-6953874516) reports no worktree, candidate branch, merge, file output or source modifications; only fetched complete nonshallow refs in the isolated store. Current snapshot `5398ca211ff16354ffbaec9a1af5cb3b6308c519` includes the ratified D-085 content. GitHub compare against previous binding `ab09111681bf1acce3823d6fe3343642a851f3d9` confirms exactly 2 new commits and 3 paths: coordination, Decision Log and milestone (+9/−2). The 52-file P0 directory remains absent from that target.

Running a scratch Git topology proof does not require making live `main` immutable. The correct future-integration gate is **separate freshness reconciliation**, not an automatic abort on every intervening documentation commit.

## New exact target and binding rule

The **full immutable target M** is the commit shown in the subsequent Manager #641 final re-pin comment on [#674](https://github.com/jermur788/CCF/issues/674), after updating the live coordination board to reflect this policy. It must include the already-ratified D-085 canonical change. Worker must fetch/select the **exact M object**, not follow a moving `origin/main` checkout.

**Before any scratch mutation:** assigned worker must verify M exists in the isolated nonshallow Git store; Q pin and exact P0 directory tree unchanged; M contains neither P0 nor P1 directory; original P0 Q is not an ancestor of M; physical #674 lease is still exclusive/valid; live board, actual worker and output paths unchanged. Compare newly published live `main` to fixed M and classify drift. If the target M itself is wrong, P0/P1 path appears at M, Q/source pins differ, custody has changed, or there is relevant new upstream P0/P1 source/integration ownership conflict, STOP for Manager reassessment.

**After selecting fixed M:** further authorised changes on live `main` **do not abort the isolated historical proof merely because the ref advanced**, including board-only or separate Scenario One canonical changes. The worker must record latest live tip and M→tip diff on handoff, and STOP only if the new developments imply a live P0/P1 ownership/source overlap, physical custody conflict, source provenance invalidation, or an explicit Manager HOLD. The proof H remains a historical candidate relative to M, not a qualified or current integrated branch. Any eventual incorporation into a newer `main` **requires a fresh merge-base/target diff, relevant D-085 and later canonical reconciliation, independent H review, and a separate Manager permit**. No automatic rebase or integration.

**No whole-repository freeze** is requested. No user product decision is required. Maintain separation of Scenario One authoritative canonical development from this local P0 source-tree proof.

## Inherited exact scope

All conditions in [original #674 conditional permit](https://github.com/jermur788/CCF/blob/docs/multi-p0-674-conditional-ancestry-proof-permit-20261117-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-P0-674-CONDITIONAL-GENUINE-ANCESTRY-SCRATCH-PROOF-PERMIT-20261117.md) and [custody/re-pin addendum](https://github.com/jermur788/CCF/blob/docs/multi-p0-674-conditional-ancestry-proof-permit-20261117-v1/Docs/Handoffs/MultiRack-Simulation-20261116/Multi-2D-SITKA-P0-674-MANAGER-REPIN-AND-LEASE-RECEIPT-ADDENDUM-20261117.md) remain except the obsolete "any main movement STOP" rule and old M pins.

- **Worker:** independent `opencode-go / DeepSeek V4.1 Flash`; #674 claim must be re-registered [RUNNING] for resumed phase with full M, existing Q, source/context, local branch/store/worktree/lease readback before further writes.
- **Lease:** custodian-reported Files ext4 exclusive `/media/jer/Files/CCF/Operations/CCF-MultiP0-AncestryProof-20261117` plus isolated `git/CCF-MultiP0-AncestryProof.git`, intended checkout `ccf-multi-p0-674`; evidence under `/media/jer/Files/CCF/Verification/CCF-MultiP0-AncestryProof-20261117/`, expires `2026-11-18T18:00:00Z`. Worker must reverify actual deployed physical Ops status and lease expiry; Manager has not inspected local files.
- **Q:** `a7f41983c2f2f233b8fe999d1a5b10e72bdd2866`, P0 **directory** tree `f212c8f64fb28b83aa870c7b5b7c72e03051e46a` (distinct from Q root tree `dfbf5f9df729f2433e1836dbf6c6ee3b33a7043e`). **F:** `e302f15bf6188605a2c2cbeddbd973b3d8ac0f66` read-only; never a merge parent.
- **Local H:** genuine two-parent merge M first / Q second; M→H **exactly 52 unchanged P0 additions** under `Tools/MultiRackSitkaLab/**`, non-P0 M tracked files/modes/absences unchanged. Use complete genuine Git ancestry, original unmodified L0 `git log Q..H -- Tools/MultiRackSitkaLab` empty, L1 P0 directory tree exact, explicit Q ancestor true. No test weakening, fake ancestry or unexpected canonical change.
- **Never authorised:** push/main merge, P1 27-file import, P0 operational edits, builds, dotnet/strace, default runtime L4 waiver (FAIL 38), ecological year (HOLD), Unity, Windows, or destructive cleanup. Preserve shared store, #639 author lane, dirty Primary and all current Scenario One work.
- **Output:** sole pre-reserved #674 GitHub Markdown result, evidence receipts, independent next review and Manager acceptance only after completed proof. If STOP, publish truthful blocker on #674; do not improvise.

## Worker continuation

Resume the previously stopped #674 task under the full exact-M comment and this addendum, not as a new writer in an overlapping worktree. Refresh live board and physical Ops, register resumed RUNNING, then use the already fetched isolated refs and run the one proof. Publish native evidence with M/H/Q parent, exact 52-file diff, original L0/L1 and real ancestry, any later main movement separated as a **freshness gate for future integration**.
