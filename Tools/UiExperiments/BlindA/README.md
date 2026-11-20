# Canopy Ledger — Blind A interface experiment

**Task:** [CCF Multi #869](https://github.com/jermur788/CCF/issues/869) · original independent 2D UI prototype.

**Status:** Standalone HTML/CSS/JS **MOCK PROTOTYPE**; not connected to the protected #849 C# forestry engine. No simulation, network, persistence, economics, inventory import, external assets, packages, or Unity dependencies. Everything in the map is invented solely to evaluate interface behaviour.

## Open

1. Obtain the files from branch \`experiment/multi-869-blind-chatgpt-ui-v1\` (clone the repository or download this branch as an archive).
2. Open \`Tools/UiExperiments/BlindA/index.html\` directly in a modern desktop browser (Chrome, Firefox, Edge or Safari). The relative \`style.css\` and \`app.js\` must stay beside it.
3. No server, build step, npm, permissions, internet access or secret is required. Refreshing the page resets the demonstration. The GitHub source-file preview is **not** a hosted runnable application.

## Explore

- The 30 × 30 m north-up stand has exactly 100 synthetic Sitka trees, identified S001–S100. Circle/triangle = standing, exclamation = pending draft, diamond/tick = approved but not felled, concentric stump = executed fell. Selected tree is ringed and labelled.
- Click/tap a map symbol or use the accessible register. Type an exact ID such as S045 (or 45) and press Enter/Find. The register filters while typing and can be used with the keyboard; pressing \`/\` outside a form focuses the ID field.
- The compact arrow pad pans by 2.5 illustrative metres. \`+\`, \`−\`, and **Fit** change the map view. You can also drag, wheel zoom, or focus the map and use arrow keys / +/- / Home.
- Choose a predefined felling reason. For **Other / custom**, enter non-whitespace text. Add a per-tree draft order; add more trees if desired, or remove orders before approval.
- **Approve** locks current orders and fells **zero** trees. To change the approved plan, withdraw approval first.
- **Execute** requires approved matching non-felled orders and a browser confirmation. It updates the mock standing/stump counts, shows visible stumps, records history and consumes the approval, preventing duplicate execution.
- **Year +1** is an illustrative time marker with an audit entry. It does not calculate growth or species ecology. Outstanding orders block it. **Reset demo** starts again with 100 trees after confirmation.

## Suggested acceptance walkthrough

| Action | Expected prototype response |
|---|---|
| Find S045, inspect selection | Individual ID, species, DBH (cm), height (m), E/N position and textual status |
| With no reason, add | Error message; no order |
| Select Other, enter whitespace, add | Error; still no order |
| Enter a custom explanation, add S045 | Pending count 1, reason appears, status Pending, standing 100 |
| Add S045 again | Duplicate blocked (button disabled and state guard) |
| Approve | Status Approved; standing still 100, stumps still 0 |
| Withdraw approval | Orders return to draft; removing/editing possible |
| Re-approve, Execute, confirm | S045 now Felled/stump; standing 99, stumps 1; event recorded |
| Click Execute again | No second execution possible; approval consumed and control disabled |
| Try ordering S045 again | Felled tree cannot be ordered |
| Year +1 | Year 2 shown with history; no growth calculation |
| Input S101 or random text, submit | Invalid-ID error |
| Add pending S046, then Year +1 | Time advance blocked until plan resolved |
| Focus a roster item using Tab and Enter | Selection and details change without map pointer |
| Zoom/pan/fit, resize to 1280×720, 1920×1080, 360 CSS px or 200% browser zoom | Map and controls should remain reachable; check actual layout in a browser |

## Design choices and boundaries

This original concept uses a **field-survey ledger** metaphor: individual-tree register at left, an abstract charted block in the middle, and a work/history ledger at right. The dark survey header, pale plotted canvas, ink-like conifer stamps and numbered action verbs are intentionally authored for this BlindA experiment, without opening prior Unity/2D designs or the competing prototype. Field information is available both visually and as text; pending, approved and felled are distinguished by words and symbol shapes, not colour alone.

The dense desktop workspace confines 100 tree entries to an internal roster scroller and avoids a page-long tree list. A narrower view stacks the map, ledger and register. Buttons and row targets are at least 44 CSS pixels high, focus states are visible, and selection has an exact-ID keyboard path. The canvas remains pointer-interactive but **is not a complete screen-reader tree grid**; use the companion register instead.

**Limitations:** No forestry calculation or factual DBH/height claim, no real cutting, no save/load, no professional approval policy, no real server fence, no actual economic cost, no habitat/browse response, no browser-hosted deployment. The confirmation dialog is a local UX precaution, not the protected engine's execution key. Screen-reader, colour-contrast, responsive 200% zoom and specific browser viewport checks require actual browser verification; do not treat static source inspection as those runtime tests. No proprietary or commercial graphics are used.

## Files

- \`index.html\` — semantic structure, forms, map and accessible tree register.
- \`style.css\` — original responsive presentation and control sizing.
- \`app.js\` — deterministic 100-tree mock, canvas renderer, actions, guards, history and keyboard navigation.
- \`README.md\` — opening steps and evaluation walkthrough.

**Integration:** None. Owner and Manager #863 may compare this against a separately produced blind design; any connection to the real C# engine requires later owner choice, approved production UI specification, security-contract review and independent qualification.