# CCF Multi #869 — Blind A original 2D forestry UI prototype — result

**Date:** 2026-11-20. **Task:** [#869](https://github.com/jermur788/CCF/issues/869). **Responsible Manager:** [#863](https://github.com/jermur788/CCF/issues/863). **Disposition:** SOURCE IMPLEMENTED / REMOTE-READBACK VERIFIED / V8 MOCK-DOM FUNCTIONAL SMOKE PASS; **REAL BROWSER RUNTIME AND SCREENSHOT QUALIFICATION NOT RUN**; no independent review, integration, acceptance or merge.

**Binding handoff:** [Multi-869-BLIND-CHATGPT-2D-UI-PROTOTYPE-HANDOFF-20261120.md](https://github.com/jermur788/CCF/blob/docs/multi-869-blind-chatgpt-ui-prototype-packet-20261120-v1/Docs/Handoffs/MultiRack-Simulation-20261116/869-orchestration/Multi-869-BLIND-CHATGPT-2D-UI-PROTOTYPE-HANDOFF-20261120.md).

## Actual actor, registration, isolation

- **Actor:** OpenAI ChatGPT GPT-6 via connected GitHub tools, authenticated GitHub user \`jermur788\`; exact internal chat ID, effort and provider quota UNKNOWN. [RUNNING registration](https://github.com/jermur788/CCF/issues/869#issuecomment-7040641204).
- **Fresh starting main:** \`2e9fd8cb63a69adfbd76ced054c46e1a84b9412e\`, independently checked against \`main\` as identical (ahead 0, behind 0) before beginning.
- **Live coordination read:** \`Docs/Project/coordination/active-tasks.md\`, blob \`2356bf6a1140c1982229f20ffa9cd39d7a21c4cd\`; at start, no #869/BlindA row. No board edits.
- **Sole source branch:** \`experiment/multi-869-blind-chatgpt-ui-v1\` freshly created from the above main, never from the other experiment.
- **Implementation source tip before this handoff:** \`b82de865dc3896b3852b09296d28d79726acb0a0\`. Four additive commits; no original or protected file changed.
- **Permitted source paths only:** \`Tools/UiExperiments/BlindA/{index.html,style.css,app.js,README.md}\`. This is the single permitted new result Markdown; no competing prototype, Unity visual files/screenshots or other design packet opened. Originality is a declared implementation-process constraint, not a claim that independent similarity review was run.
- **Physical/Unity/engine authority:** none. No real #849 C# API connections, remote service, asset edits, compile-linked source, G1, save, P0 or production spec changes. No main merge.

## What was implemented

**Canopy Ledger** is an offline, self-contained survey-notebook interface: an exact-ID tree register, north-up charted 30 × 30 m illustrated stand, and a separate action/history ledger. The map uses 100 deterministic, distinct Sitka IDs (\`S001\`–\`S100\`) and illustrative DBH, height and coordinate data. Its authored canvas symbols distinguish Standing (triangle), Pending (exclamation), Approved (diamond/tick), and Felled (ringed stump); status appears in text too. Click/tap the map or use the focusable 100-entry list and exact-ID search.

Controls: compact four-way arrows; +/- zoom; fit; drag and wheel; arrow/+/-/Home when canvas focused. Desktop workspace confines roster scroll, with narrow-screen stacking. Control and row target minimum heights are authored as 44 CSS px. Semantic form labels, status/feedback live region, focus-visible ring, keyboard tree list and exact search are present. No runtime accessibility audit is claimed.

Management flow:
1. Select exactly one standing tree; choose \`Competitor release\`, \`Poor stem quality\`, \`Tree health or damage\`, \`Regeneration light gap\` or \`Other / custom\`. For Other, non-whitespace custom text is mandatory. The reason is attached to the particular tree order.
2. Add one or multiple draft tree orders; duplicate, already-felled and empty-reason commands fail. A draft order can be removed.
3. **Approve** freezes the ordered tree IDs/reasons for this mock cycle and **fells zero trees**. To change it, withdraw approval, edit and reapprove.
4. **Execute** is a separate deliberate action with native browser confirmation, ID/reason consistency guards, one-shot receipt protection and a mock felling transformation. Approved standing symbols become stumps, standing count decreases, felled count increases, pending queue is consumed and history records the event.
5. **Year +1** changes the mock timeline only, with a log explaining that no growth calculation ran. It is blocked while draft/approved orders remain. Reset restarts the deterministic in-memory scenario.

All quantities and actions are labelled as **PROTOTYPE — NOT THE REAL FORESTRY ENGINE** / synthetic or mock. No network API calls, persistence, real biology, actual C# execution keys, work costs or save logic.

## Launch / review

Clone or download [the isolated BlindA branch](https://github.com/jermur788/CCF/tree/experiment/multi-869-blind-chatgpt-ui-v1/Tools/UiExperiments/BlindA) and open \`Tools/UiExperiments/BlindA/index.html\` in a recent browser. Keep \`style.css\` and \`app.js\` adjacent. No build step, package manager, server or internet access needed. The GitHub file viewer is source preview, not an app host. [README instructions and expected walkthrough](https://github.com/jermur788/CCF/blob/experiment/multi-869-blind-chatgpt-ui-v1/Tools/UiExperiments/BlindA/README.md).

## Executed checks — evidence and limits

**Remote GitHub source readback at the implementation tip:**

| File | Verified Git blob SHA | Lines |
|---|---|---:|
| \`index.html\` | \`c7f76ca63dc84eedfda17683dd7d4439732cb16d\` | 126 |
| \`style.css\` | \`9024aea570fc2dd14a721f2a1d3d00bfbebb7d33\` | 192 |
| \`app.js\` | \`404d7ff2283ccd705e7dd9043cb4f6e2f4b2d8c2\` | 581 |
| \`README.md\` | \`45b32232aadb0ae514686071ed6dba19d7094c79\` | 58 |

GitHub comparison starting main → implementation tip: **ahead 4 / behind 0**, exactly four **added** files, all under \`Tools/UiExperiments/BlindA/\`; 0 deleted/modified pre-existing paths. After publishing this document, final comparison should report exactly those four files plus this single added Markdown.

**V8 parser:** \`new Function(app.js)\` parsed without syntax error, not executed as a real browser.

**In-memory simulated DOM action smoke:** 14/14 checks PASS from the exact fetched GitHub \`app.js\`, executed in a JavaScript isolate using stub DOM/canvas and affirmative stub confirmation. Covered (1) 100 initially standing, (2) zero felled, (3) roster 100, (4) whitespace Other blocked, (5) valid Other reason queues, (6) duplicate queue blocked, (7) pending blocks Year, (8) approval changes zero fell, (9) execute changes 100/0 to 99/1, (10) duplicate execution leaves count 1, (11) Year +1 and history, (12) invalid S101 rejected, (13) exact S046 selection, (14) approval withdrawal returns draft. This is **logic simulation only**; canvas painting and actual browser DOM/layout are *not* tested by these stubs.

**NOT RUN / UNKNOWN:** actual page in Chrome/Firefox/Safari, visual screenshot at 1280×720 or 1920×1080, layout at 200% or 360 CSS px, touch/drag physical interaction, colour contrast measurement, automated accessibility test, screen-reader traversal, engine contract test, independent review or user playtest. The code contains responsive rules for <=1120/740 CSS px and keyboard paths, but their effectiveness remains to be verified in an actual browser.

## Visual decisions, tradeoffs, open considerations

- The three-part **survey sheet** puts spatial choice in a dedicated central map and keeps action execution separate from navigation. Advantage: clear authoring/approval boundary; tradeoff: at 1280×720 the right ledger is intentionally internally scrollable, and smaller screens stack sections rather than fitting everything simultaneously.
- Drawn survey contours/graticule and geometric tree marks require no copied image assets, but they are not photorealistic forest visuals.
- One tree per symbol preserves precise ID selection; a separate keyboard list avoids pretending the canvas is a screen-reader-accessible grid. The map may need later density/cluster treatment at other scales, not authorised or implemented here.
- All sample dimensions/site quantities are invented. The mock does not claim biological fidelity and does not provide production authorization.

## Outcome and manager decision required

**Implementation artifact complete as a source candidate** and remote-readback / mock-logic verified. **NOT ACTUAL BROWSER-QUALIFIED, NOT INDEPENDENTLY REVIEWED, NOT INTEGRATED, NOT ACCEPTED.** Manager [#863](https://github.com/jermur788/CCF/issues/863) should arrange an actual desktop/zoom/keyboard/browser review and blind compare with the other independent design without exposing one implementer to the other design. Owner selects the desired direction; any replacement of the real #849 UI requires an approved four-file production spec and separate authorised integration/security review. Preserve this experiment unmerged.

**Direct handback:** [#869](https://github.com/jermur788/CCF/issues/869) → [Manager #863](https://github.com/jermur788/CCF/issues/863); [source branch](https://github.com/jermur788/CCF/tree/experiment/multi-869-blind-chatgpt-ui-v1/Tools/UiExperiments/BlindA).