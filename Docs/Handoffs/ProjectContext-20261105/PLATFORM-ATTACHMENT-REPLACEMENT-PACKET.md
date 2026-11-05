# CCF #293 — ChatGPT and Claude Project attachment replacement after D-071 Drive acceptance

**Status:** WAITING FOR CAPABLE PROJECT-UI ACTION / NOT RUNNING. **Classification:** PACKET ONLY — publish existing verified generated files, not source regeneration or a new product decision.

**Parent Drive publication:** [#292](https://github.com/jermur788/CCF/issues/292) — 13/13 **APPROVED / independently Manager-verified**. [Manager Drive verdict](https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ProjectContext-20261105/MANAGER-D071-DRIVE-PUBLICATION-ACCEPTANCE-20261105.md) includes direct raw-byte SHA-256 independent download checks and two separately reproduced composite hashes. **Full context set is not complete** while project attachments remain historical.

**Task claim:** [#293](https://github.com/jermur788/CCF/issues/293) — platform attachments only, not another Drive update. **Difficulty: Medium** — authenticated project settings on two platforms with different controls, avoid stale duplicates and distinguish file presence from a byte-verifiable readback.

**Best-suited executor:** the **user via the existing authenticated ChatGPT Game Dev Project and Claude Project interfaces**. A capable delegated ChatGPT Work/desktop or T3 computer-use session may also handle a specified UI **only after separate explicit user authorisation and an actual access check**. The earlier T3 consent was for **Google Drive versions only** and did not authorise ChatGPT/Claude Project file mutations. Overall Manager #200 can review GitHub and project file index but has **no connected tool capable of changing Project attachment membership** in this chat. Do not claim automated Project attachment swaps.

## Exact content identity

The source set was generated in #289, updated in existing Drive objects by #292, then independently checked by Manager. Use ONLY files of the following identity:

| Platform target | Attachment to replace/add | Expected SHA-256 |
|---|---|---|
| ChatGPT Game Dev Project instruction/context attachment | `chatgpt-project-instructions.md` | `eb104ff20e62066aad4c8becfc6de286cfa70cbe322afd6c4e831320d5e4164b` |
| Claude's CCF Project instruction/context attachment | `claude-project-instructions.md` | `8e70a215adbc337cfa060c73151d799d23c69c85b5c2a43d0ce588274aeedc9a` |

**Context:** `81867c8940d7d07c952e8e49ca4a0cb93498e790`; expected timestamp `2026-11-05T16:49:16+00:00`; **not a dirty preview**. `context-manifest.json` hash `d87372edbb9f7b39866a63f31cf186915b848b7af587fe5c570f15ac1fc1f536`.

**Original verified local files:**
`/media/jer/Files/CCF/Verification/ContextRefresh-D071-20261105/ProjectContext/chatgpt-project-instructions.md`
`/media/jer/Files/CCF/Verification/ContextRefresh-D071-20261105/ProjectContext/claude-project-instructions.md`

**Manager independent reconstruction:** downloaded all 12 permanent Drive mirror files and manifest, stripped the stamped `MIRROR — DO NOT EDIT` headers, recomputed each original source hash, rebuilt both composites using the inspected canonical generator format. Both bytes match the above expected composite SHA-256. The Manager conversation supplied the two standalone Markdown files and one ZIP containing both composites, all 12 stamped mirrors and manifest. User may use those downloaded artifacts or their original Files volume copy; compare actual local byte hashes before upload.

## Scope and safe workflow

1. **ChatGPT:** In the existing **Game Dev Project** file/knowledge attachment area, inspect the present context and mirror attachments. Add or replace `chatgpt-project-instructions.md` with the exact hash-verified D-071 composite. Update the configured copies of the 12 canonical mirrors plus `context-manifest.json` **only if that Project currently uses them as attached context files**. Do **not** remove unrelated forestry research papers, task reports, game builds, user files or the Project's unrelated content.
2. **Claude:** In the existing **CCF Claude Project** knowledge/attached-files area, replace the historical context/instructions composite with the exact `claude-project-instructions.md`; apply the configured mirror/manifest replacement in that Project only where applicable. Never interpret uploading to Google Drive as automatic update of Claude Project knowledge.
3. Preserve the correct Project identity and all unrelated attached documents. Where the UI supports replacing a file, use it. Otherwise safely stage the correct D-071 file, inspect that it has arrived, and only then remove the corresponding **obsolete** context copy (avoid leaving both old and new as parallel authorities). Do not alter the Project's *human-authored* compact instructions field unless a separate approved explicit scope requires it; this packet concerns generated file attachments.
4. **Verification:** Record an inventory before and after for each platform, the exact attached filenames, what old file/version was replaced, and inspect the new `Context:` stamp in its accessible content. If the platform provides download/export raw bytes, compute SHA-256 and match the expected digest. If it only shows a filename or first page, record **UPLOADED/PRESENT — BYTES NOT INDEPENDENTLY VERIFIED**, not a false HASH PASS. Verify the configured mirror attachments separately (12 files + manifest if actually used).
5. When finished, publish a durable GitHub Markdown return at `Docs/Handoffs/ProjectContext-20261105/PLATFORM-ATTACHMENT-REPLACEMENT-RESULT.md` on a single owned docs/report branch `docs/platform-context-attachments-d071-20261105-v1` (conditional on unused, clean branch/lane), or have Manager publish from user-provided exact UI evidence. Report ChatGPT vs Claude platform states separately. Only Manager may mark full context publication **COMPLETE** after both required platforms are reconciled.
6. **Do not do:** regenerate or edit source text; re-upload the 13 already accepted Drive objects; touch gameplay/Unity, playtest checkout/saves, Legacy40, frozen Reference Future v1 or Windows build; change browser credentials/sharing or impersonate a different account. A user authorisation to access Drive is not authority over all platform accounts.

## Success/stop conditions

**SUCCESS:** Both appropriate platform composites updated, no old parallel context authorities, related mirror/manifest attachments refreshed where configured, correct D-071 context seen in Project UI. Raw-byte hashes must be separately PASS or explicitly not verifiable. Durable return published and Manager review issued.

**STOP/PENDING:** No platform UI edit capability, wrong project/account, missing files, an attachment cannot be replaced without deleting unrelated files, unverified context stamp or conflicting previous instruction attachment. Return exact missing action without weakening hashes or changing product constraints.

**User action:** Open ChatGPT Game Dev Project and Claude Project knowledge management screens to replace the generated context attachments, or explicitly enable a supported Work/T3 browser automation session for those two specific project surfaces. Send completion evidence for Manager review. **A separate platform browser permission is required** before directing a worker to make those changes.

## Paste-ready handoff (if delegating an authorised platform session)

Perform CCF #293 platform attachment replacement only. Read https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ProjectContext-20261105/PLATFORM-ATTACHMENT-REPLACEMENT-PACKET.md . Exact two files are chatgpt-project-instructions.md and claude-project-instructions.md with the expected SHA-256 values in the packet; context must match the accepted D-071 stamp. Inspect each Project's configured attachments and stage exact new copies, then remove only the corresponding superseded context files. Reconcile mirrors/manifest where attached; leave research reports and unrelated Project instructions unchanged. Read back actual content/attachment list and distinguish hash-verified from presence-only. Publish Docs/Handoffs/ProjectContext-20261105/PLATFORM-ATTACHMENT-REPLACEMENT-RESULT.md on a registered clean docs branch for Overall Manager review. Do not touch Drive, Unity, game files, credentials or Windows build. If platform computer-use is unavailable or authorisation absent, report BLOCKED and stop.
