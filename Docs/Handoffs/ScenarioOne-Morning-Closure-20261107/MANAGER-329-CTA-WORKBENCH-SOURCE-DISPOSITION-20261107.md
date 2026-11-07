# Manager #200 — #329 exact-candidate independent review disposition (CTA and Workbench)

**Date:** 2026-11-07 (Ireland). **Authority:** Overall Manager #200. **Review execution verdict:** **APPROVED as completed genuinely independent evidence handoff; close #329 reviewer claim.** Two source outcomes deliberately DIFFER: **CTA NEEDS CHANGES**; **Workbench APPROVED AT SOURCE SCOPE ONLY**. Neither is a Unity/runtime/integration qualification or permission to merge/build.

## Evidence and independent identity

Inspected:
- [#329 final exact independent review](https://github.com/jermur788/CCF/blob/2b608ff3dd5a1fd94c6860aaf47bacbf1c76098b/Docs/Handoffs/ScenarioOne-Morning-Closure-20261107/VERIFIER-CLOSURE-INDEPENDENT-REVIEW-RESULT.md) and [#329 completed handoff comment](https://github.com/jermur788/CCF/issues/329#issuecomment-6714510396).
- [#328 Stage A CTA original author result](https://github.com/jermur788/CCF/blob/fix/cta-roslyn-symbol-binding-20261107-v1/Docs/Handoffs/ScenarioOne-Morning-Closure-20261107/CTA-ROSLYN-PINNED-BINDING-RESULT.md) and [Stage B Workbench receipt result](https://github.com/jermur788/CCF/blob/fix/workbench-receipt-finalization-20261107-v1/Docs/Handoffs/ScenarioOne-Morning-Closure-20261107/WORKBENCH-RECEIPT-R2-FINALIZATION-RESULT.md).
- [Manager #328 author-only checkpoint and #329 release](https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Morning-Closure-20261107/MANAGER-328-AUTHOR-HANDOFF-DISPOSITION-AND-329-RELEASE-20261107.md), latest task issue and live coordination.
- Reviewer source target matched the published heads with only handoff Markdown beyond the actually tested source: CTA branch `fix/cta-roslyn-symbol-binding-20261107-v1`, tested `288ba3863793e3775327fb7322dc06af28f7835a`, published `6adb20318b80e8a81560bc58a63c8bdbfa15d7ca`; Workbench branch `fix/workbench-receipt-finalization-20261107-v1`, tested `a4019e6b96b50e03d2b71d92c44fc30a0f7ace29`, published `03b484d10b36382ab86d5e345ece9acac96c9620`.

The reviewer used **OpenAI GPT-6.1 Sol, Medium effort**, disclosed rather than invented High; author was **Claude Sonnet 5.5**. Genuinely cross-provider, independent reviewer. This Manager verdict is based on inspected durable GitHub handoff/raw evidence descriptions; **browser Manager did not itself rerun compiler/Unity tests or inspect actual local worktrees**. Reviewer's local runner evidence was in reviewer-owned Files clones; no Unity/Editor/lease or production change was claimed.

## G2/G3 Stage A: CTA — NEEDS CHANGES, repair only the real method/site boundary

**A329-01 MAJOR valid false green.** Reviewer moved the exact Objectives presentation assignment

`learning.Root.style.display = !preview && screen == UiScreen.Objectives ? DisplayStyle.Flex : DisplayStyle.None;`

from `ScenarioOneUiRoot.Update()` into an **uncalled** private `ReviewUnusedPanel(bool preview)`. This compiled as valid C#9, preserved root `UnityEngine.MonoBehaviour` metadata and the 5 Time / 22 DisplayStyle receiver-site identities, but original Roslyn reporter still counted 27, and real `check_baseline.py` returned **SOURCE-STATIC 10/0/2 exit0** instead of rejecting through check 08. This was explicitly in #329 binding Stage A method/site obligation. It is **a checker oracle gap under a hypothetical mutation**, not evidence the unchanged production UI behaves incorrectly.

**What #329 independently DID pass:** SDK8.0.133 and two compiler hashes, 320/320 pinned references, 189 C#9 player-like input files with zero compiler errors, root/base and 27 real metadata sites, existing A326/R321 invalid mutations rejected, normal 10/0/2, historical intentional red 6/4/2, old lexical 884 negatives/112 benign, six actual cwd SDK8 SnapFar. All protect prior work; they do not resolve missing enclosing-method identity.

**Manager decision:** **NEEDS CHANGES at CTA SOURCE / G2/G3.** Release **one bounded method/site source repair [#330](https://github.com/jermur788/CCF/issues/330)** with [complete packet](https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Morning-Closure-20261107/MANAGER-329-CTA-OBJECTIVES-UPDATE-SITE-CLOSURE-PACKET-20261107.md), then [#331 independent source review](https://github.com/jermur788/CCF/issues/331) with [review packet](https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Morning-Closure-20261107/MANAGER-330-CTA-UPDATE-SITE-INDEPENDENT-REVIEW-PACKET-20261107.md). Require that actual `ScenarioOneUiRoot.Update()` owns the intended visibility site; reject unchanged statement moved to an uncalled member **for the right reason**, not changed parser defaults. Bounded author/review attempt, not an unlimited new C# parser hunt or general caller analysis. Preserve old source/test evidence, do not modify accepted UI production or PR#300 ancestry. Do NOT Manager-approve CTA checker, #309 or integrated A–O yet.

## G2/G3 Stage B: Workbench receipt — APPROVED AT SOURCE SCOPE ONLY

The independently tested exactly pinned Workbench source is **APPROVED for source-level integration consideration**: `fix/workbench-receipt-finalization-20261107-v1` tested `a4019e6b96b50e03d2b71d92c44fc30a0f7ace29`, published `03b484d10b36382ab86d5e345ece9acac96c9620`. This is narrow approval of the Workbench **verifier/runner correction chain**, including necessary inherited #318/#323 source fixes only to the extent those were rechecked by #329; not a new approval of underlying Unity behaviour. Do not create another Workbench author or source-review task for this already independently approved scope absent new evidence.

**Independent proof includes:**
- **41/41** complete runner tests, original **13** mocks, **28** historical synthetic C# projection/schema controls and separately checked R1–R5; none skipped.
- R1 true tracked package/material post-Editor drift now fails with diagnostics and no false green; prior #318 false-green independently re-demonstrated as a control.
- Genuine non-root directory/marker permission failure plus simulated double ENOSPC leave only **PROVISIONAL / INCOMPLETE** markers and process exit1, not bare `Editor PASS`; ordinary pass requires **process exit0, JSON exit0/empty failures and first-line FINAL marker with matching JSON SHA-256**. Stale/corrupted/nonfinal/final-rename failures fail closed; old #323 R324-01 case independently reproduced.
- Original terminal marker, historical labels, teaching test controls and unchanged projection identity passed. Reviewer found no registered consumer relying on obsolete marker format in inspected CCF/CCF-Ops source, but unknown external consumers must adopt the new complete authority rule.
- No Asset, package, scene, save, Reference Future or user-file source change in this Stage B repair.

**Limitations that DO NOT vanish under source approval:** This is offline source/mock/temporary Git evidence, not a successful real Unity import, Workbench C–G, native former-site 720p/1080p witness, Legacy40/frozen Reference save/load or Enlarged80 repeated deterministic reset. #287 **STILL NOT UNITY QUALIFIED** and tree identity P0604→P1209 still lacks causal pose/clearance witness. **No Unity Editor, no lease, no original registered ZX20 worktree cleanup/HEAD switch, no main/integration merge and no Windows build** is authorised by this ruling.

## G4 storage, packages, tree and integration — STILL BLOCKED

- **Eight original dirty tracked Workbench files:** keep protected/losslessly backed up and untouched. Tracked three direct Linux SDK/toolchain packages 1.1.0 vs Editor resolved 1.1.1 (transitive sysroot also changed); five SectionFive materials were Editor property rewrites, not proven equivalent formatting. No Manager-authorised restore, delete, cache clear, package upgrade or equivalence waiver. A future **separately approved exact registered-lane environment plan** must address package provenance and byte-verified recovery in safe ownership before Unity work.
- **B3 tree substitution:** observed 1,343 shared unchanged, one lost P0604 and one gained P1209 across supposed return-to-start. Player pose is a plausible clearance input but original captures lacked pose; require separately authorised bounded runtime logging of player pose/clearance/scene reload/tree IDs before and after. Preserve tree equality/hash anchors; no rebaseline or speculative production fix.
- **Storage:** ZX20 ~22 GiB free last worker-reported, below 27 GiB cold-import entry and too little for another ~6.8 GiB Library while respecting 15 GiB hard floor (and 20 GiB Manager STOP). Original Workbench Library retained; no second cold Library/lease authorised. Validate live free/owners and decide a clean/backup verified storage option only in future explicit operations packet.
- **Integration:** #315 read-only dossier remains preparation, #316 review waiting; no integrated candidate exists. **All 15 A–O integrated gates NOT RUN**. Frozen Reference Future v1, Legacy40, RackV2/MFM and Work Plan policy remain mandatory. No Windows/Player build before user personally playtests a Manager-approved exact candidate and explicitly authorises build.

## Ownership / next action / process discipline

- **#329 independent review claim:** reviewer completed and HANDED-OFF, Manager accepts execution/evidence and closes review claim, **not** full CTA source.
- **#328 Stage B Workbench source:** now G2/G3 source-approved; no duplicate author pass. Stage A CTA source remains NEEDS CHANGES.
- **#330 author and #331 reviewer:** conditional proposed distinct Files branch/worktree; no local writer actually started by publishing this GitHub decision. After actual author handoff, #331 separate independent read-only review, then Manager G2/G3 verdict.
- **Efficiency:** Review introduced one actual method-site defect even after all symbol identifiers were correct. Close that *concrete* site requirement with a small Roslyn method-ancestry check, reuse unchanged accepted negatives rather than repeating unbounded mutation campaigns. Do not waive known false green to speed acceptance, and do not force full Unity until source and environment admission.
- **Status:** update live `Docs/Project/coordination/active-tasks.md`, #328/#329/#330/#331 claims, #287/#309 parent and shared Overall-Dev current CCF. No downstream mirror/dashboard claim without observed publication/ingestion evidence.

**User action:** dispatch #330 using its direct packet; no new product design decision. #331 waits for real author handoff. A future Unity environment and B3 runtime investigation will require a separate explicit Manager release after clean provenance/storage.
