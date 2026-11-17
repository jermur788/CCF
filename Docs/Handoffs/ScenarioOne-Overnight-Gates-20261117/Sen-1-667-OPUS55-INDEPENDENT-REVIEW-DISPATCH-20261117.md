# Sen-1 #667 — Claude Opus 5.5 independent nonauthor review dispatch

**Published by:** the registered #667 DeepSeek orchestrator (`opencode-go/deepseek-v4.1-flash`, T3 Code/OpenCode, host `jer-laptop`), 2026-11-17.
**Review child claim:** [#669](https://github.com/jermur788/CCF/issues/669). **Author claim:** [#668](https://github.com/jermur788/CCF/issues/668). **Parent:** [#667](https://github.com/jermur788/CCF/issues/667).
**Assigned reviewer:** Anthropic **Claude Opus 5.5 (`claude-opus-5-5`)**, separate T3 `delegate_task` session. **You did not author these documents and must not edit them.**
**Role:** Independent nonauthor reviewer. Your verdict is an **evidence-quality** verdict for the dossiers. It is **not** a content signature, save-contract signature, spec approval, Unity qualification, Manager acceptance or integration authorisation.

## 1. What you are reviewing — exact immutable artifact

| Item | Value |
|---|---|
| Author branch | `docs/sen-1-667-compact-world-save-evidence-20261117-v1` |
| Author base | `1e0fba5ffd5a07704eed3e44c34b3a960f61ac60` |
| **Reviewed author tip (immutable)** | **`d05207782bbb8274dbf73eeb383119aa0854adbe`** |
| Orchestrator-confirmed diff vs base | exactly **two added files, 699 insertions, 0 deletions, nothing else** |
| Author model | OpenAI GPT-6.1 Sol (`gpt-6.1-sol`), effort high — a **different model family** from you |

| Document (read at the exact tip above) | SHA-256 (orchestrator-recomputed) | Lines |
|---|---|---|
| `Docs/Handoffs/ScenarioOne-Overnight-Gates-20261117/Sen-1-667-COMPACT30-COMPLETE-WORLD-CONTENT-PROVENANCE-AND-SIGNATURE-READY-CANDIDATE-20261117.md` | `824c503c156741338670730ee3623270e9d658531e2e5ec27cf1145488208f3b` | 342 |
| `Docs/Handoffs/ScenarioOne-Overnight-Gates-20261117/Sen-1-667-COMPACT30-CGTRIG-MAE-SAVE-EVIDENCE-DECISION-CANDIDATE-20261117.md` | `791f243b3ff5d826e2df1b6da771c90368c9a8a33be1b4ab515a1892cf959f4f` | 357 |

Read them from Git, not from any working copy the author may have left behind: `git fetch origin && git show d05207782bbb8274dbf73eeb383119aa0854adbe:<path>`. Recompute both SHA-256 values yourself and report any mismatch as a CRITICAL finding.

## 2. Lane, base, output

- Branch (already exists on `origin`): `review/sen-1-667-independent-gate-evidence-20261117-v1`, expected base `85f2b35deb751d78e9486d4db0f68d8d14b5ea96`.
- Start inside your prepared checkout with `git fetch origin && git checkout -B review/sen-1-667-independent-gate-evidence-20261117-v1 origin/review/sen-1-667-independent-gate-evidence-20261117-v1`.
- Add exactly **one** file: `Docs/Handoffs/ScenarioOne-Overnight-Gates-20261117/Sen-1-667-INDEPENDENT-COMPACT-WORLD-AND-SAVE-GATE-DOSSIER-REVIEW-20261117.md`.
- Nothing else may be added or modified. You must not touch the author branch, the DRAFT specs, `Docs/Project/*`, code, scenes, or any third-party file. No force push.

## 3. Hard exclusions

No Unity/Editor/Ops/lease/Player/Windows, no personal save or PlayerPrefs access, no production or serialized-asset edit, no signature/approval/status change, no `main` write, no editing the author's documents, no inventing a test run. Static reading, Git reads, hashing and read-only arithmetic only. Do not start a Unity process.

## 4. Required review scope

The dossiers are **PRE-IMPLEMENTATION / OWNER-AND-SAVE-OWNER-DECISION-READY** evidence candidates with an explicit `SIGNATURE-NOT-READY` verdict. Judge whether that is an accurate, complete, source-faithful evidence package — not whether the game works.

Check, at minimum, and cite the document line(s) and the source line(s) for every finding:

1. **Factual fidelity to current source.** Spot-check and then extend: `Assets/ForestPrototype/ScenarioOne/ScenarioOneWorkOrder.cs` (no `juvenileId`), `ForestSaveData.cs` (`PlantedJuvenileSaveData` has no `workOrderId`), `ScenarioOneManager.cs` (`ResolvePlanting` sequence, the exact in-line juvenile creation returning void, `WorkResolved` fields, `MarkAnnualReviewSeen`, `CanAdvanceYearNow` lacking an unread-review gate, `AdvanceYear` report year), `ScenarioOneCompletionRules.cs`, `UI/AnnualReviewView.cs` earlier-year guard, `UI/ScenarioOneUiRoot.cs` acknowledgement preconditions, `ForestEcologyController.cs` `HegyiCutoffMeters = 8f`, `ScenarioProtectionVisuals.cs` failed pose and destroyed collider, `ForestSaveValidation*.cs` geometry rejection. Report any claim the documents assert about current behaviour that the source does not support, and any omitted current-source fact that materially changes a conclusion.
2. **Signed save boundary.** The documents must treat the §4.2 schema at signed commit `200add4e6e5116268b79ef63131ff9b836f14c5d` as immutable, quote keys accurately, and not smuggle a new key/sidecar/root or a schema change. Verify quoted signed rules against the spec text. Check that the S1 proposal is clearly marked as requiring a separate signed revision and that the closed three-root rule is respected in the argument.
3. **Roster / world provenance integrity.** Independently recompute the 175-row roster digest `dac06a3b…` from the canonical GitHub annex text (the roster-only canonical rule in the annex), and the 189-row reference digest `e649b2aa…`. Verify the 14 H1 omissions, the 8 buffer IDs, the 19 historical rack-labelled stems still standing, the 175 count and the 36-cell occupancy sum = 175. Verify the source byte hashes independently on this host: `/media/jer/Files/CCF/Verification/CompactOneRackNativeFeasibility460-20261113T145748Z/native-20261113T152528Z/C-B-30.json` (`3e7c37ad…`) and `/media/jer/Files/CCF/Verification/scen1-508-EntrancePathPlanting-20261114/ledger.json` (`aa10f54f…`), plus `sha256sum -c SHA256SUMS` in the #508 folder (expect 9/9 OK; do not execute the scripts). Confirm the documents correctly label what a roster-only digest does **not** prove and correctly report the absent duplicate ledger path rather than inventing a hash.
4. **Missing world fields.** The field matrix must cover every field group the umbrella packet demands (tree identity/species/position/y/age/DBH/height/crown/prefab-yaw/stage/status/suppression/mark/mortality; terrain height/normal/surface; 36 cells incl. flora/seed/browse/deadwood; H1 surfaces/spawn/visitor/parking/collection; path 8 anchors/spline/interpolation/projection; north A7 alternatives; G1 four sites + tolerance proposal; shelter four states incl. r ≤ 0.10 m failed footprint; player collider/1.2 m core/0.20 m verge; serialization content-key/version + historical refusal). Name any silently dropped field or any row whose evidence class is wrong (e.g. a proposal labelled verified, or an unsupported value with no gap).
5. **Deferred owner decisions.** `M-A-SCOPE` (one treated stem) must stay **DEFERRED/UNDECIDED** with no quota, no assumed eligibility, no classifier that presumes it. A7 north terminus must present alternatives with **no** decision. The impossibility of signing tonight must survive in the prose, not just the banner. Flag any place where a proposal is effectively decided by the author.
6. **Proposed vs accepted markers.** Consistent evidence labels throughout; no "APPROVED"/"SIGNED"/"satisfies" drift; the three spec families stay DRAFT, ISO-58/CG-TRIG BLOCKED, #652 HELD, roster UNSIGNED.
7. **Save-owner decision readiness.** Both admission alternatives (refuse vs admit-incomplete) present with threat model, back-compat, player experience and consistency with signed text; a recommendation clearly addressed to the save owner and clearly **not** a decision; proposed ruling texts clearly unsigned. Pairing/atomicity analysis must not claim the current code path already captures the pairing. W-A preservation must be accurate.
8. **M-A evidence route.** S1 elements all marked unsigned with the storage/closed-root constraint; S2 unsupportability reasoned from the actual absences; the 8 m Hegyi cutoff correctly treated as a model constant, not an owner choice; no invented CI/light threshold.
9. **Blocker completeness and internal consistency.** W01–W14 and S01–S08 present, dependencies named rather than double-counted, the stated counts (88 field rows / 36 unsupported / 36 cell rows / 72 unsupported table rows / 22 register entries) internally consistent with the tables. Report arithmetic or count errors.
10. **Whether the umbrella acceptance is actually met**, i.e. whether this is a complete decision-ready dossier or is materially thin somewhere. A truthful `SIGNATURE-NOT-READY` is not a defect; an incomplete but confident-looking matrix is.

You may run read-only shell/git/hash/arithmetic checks. You may **not** add code tests to decorate the review, and you must not re-derive the whole world (that is the author's evidence; you verify it).

## 5. Verdict and output format

Your single file must contain:

- Header: reviewer model and provider, session independence statement (different model family and separate session from `gpt-6.1-sol`; no authorship), date, reviewed branch/tip/base, the two recomputed file hashes.
- **Verdict: `APPROVED EVIDENCE QUALITY` / `NEEDS CHANGES` / `REJECTED`** — one of these exactly.
- Findings table: `ID | severity (CRITICAL / MAJOR / MINOR / NOTE) | document line(s) | source evidence | what is wrong | required correction`. Severity discipline matters: a CRITICAL blocks the dossier's claim of decision-readiness (fabricated/contradicted fact, invented value, decided deferred policy, signature drift); a MAJOR is a materially incomplete or misleading field/gap/blocker; a MINOR is a precision/labelling/count defect; a NOTE is an improvement.
- What you verified and **how** (commands/reads), and what you could **not** verify (with reason).
- An explicit statement of what your APPROVED verdict would and would not mean (it never signs the world, the save contract or the spec, and never authorises Unity/Ops).
- `REVIEW INDEPENDENCE LIMITATION` section: state honestly that both sessions run on the same host and the same project store, and that independence here means a different model family, a separate session, no authorship overlap, and reading only the pushed artifact — not separate hardware or separate organisational custody.
- If `NEEDS CHANGES`, a bounded correction list that the **same author** can fix on its own branch, with the expectation that only the changed lines are re-checked afterwards (no whole rerun).

## 6. Handoff

1. Commit the one file, push normally to `review/sen-1-667-independent-gate-evidence-20261117-v1`.
2. Verify `git diff --name-only <base>..HEAD` shows exactly one added file.
3. Post one comment on [#669](https://github.com/jermur788/CCF/issues/669): verdict, exact review HEAD SHA, file path + sha256 + line count, findings counts by severity, what was verified vs not, and the explicit "this is not a signature/approval" statement. If `gh` is unavailable, put the same block in your final message marked `handoff not posted: gh unavailable`.
4. Return a final message with: review HEAD SHA, pushed yes/no, verdict, severity counts, and the top blockers.

**Do not** open extra issues, start a second writer, contact other owners, or attempt to fix the author's documents yourself. The orchestrator routes bounded corrections back to the same author.
