# CCF Multi Stage 4 Owner UX Repair — Acceptance Checklist
**Spec status:** DRAFT / UNCHECKED  
**Evidence standard:** Source and actual browser evidence required. Author claims do not equal independent review or owner acceptance.

## Phase A — Player workflow
- [ ] AC01 At 1280×720, clicking any suitable living tree shows all ranked competitors up to five AND Add selected to Fell plan without inspector scrolling; pruning is lower/collapsed.
- [ ] AC02 Clicking numbers 1–5 toggles only the *matching ID* local Fell draft, without tree re-selection or Work Plan API mutation; clicking again deselects; multi-select works.
- [ ] AC03 Inspector checkbox and map rank states stay synchronised both directions, also after new reference/area selection.
- [ ] AC04 Selection count/button accurately reflect draft; Add queues actual chosen IDs, not all competitors; Pending/Approve/Execute remain separate.
- [ ] AC05 Visible Close inspection button and Escape restore normal (non-enlarged) tree display and clear temporary selection, preserving pan/zoom, area, Crop designation, queued/approved orders and year.
- [ ] AC06 When modal/dialog is open, Escape dismisses modal first and does not close underlying inspection in the same key action.
- [ ] AC07 One short synthetic stand bar and Details retains full actual caveats without duplicating them across map.
- [ ] AC08 Navigation has N top, W E middle, S bottom; horizontal −/Fit/+ and horizontal Area A/B below; every button works and pointer targets reasonable.
- [ ] AC09 Light Guide default OFF; ON shows no numbered-band disks, old dark overlays or glowing blobs; trees remain visually readable.
- [ ] AC10 Coral/peach graduated light renders mathematically continuously from actual `L`: 0% invisible, 1% extremely subtle but nonzero, monotonic gradual change, 100% brightest. No nonzero threshold/no forced openings.
- [ ] AC11 Colour choices have adequate forest contrast and are not identical to green canopy; no amber/green substitute.
- [ ] AC12 Light ON still supports tree crown and numbered rank selection; separate explicit ground inspect action cannot steal tree clicks.
- [ ] AC13 Ground inspector shows level, correct engine light number, actual separate canopy-cover measure (when present), contributing tree count; expanded IDs and technical provenance available.
- [ ] AC14 No repeated “not measured sunlight” disclaimer on map; full technical limitations remain accessible on demand.
- [ ] AC15 Null/unavailable `stage4` does not masquerade as model 0 or offer unsupported Execute; stale fence refusal stays fail closed.
- [ ] AC16 1280×720 and 1920×1080, legend show/hide and A/B, no horizontal overflow or key elements hidden by map/dock; accessible selected/focus/keyboard status.

## Phase A — Safety, regression and evidence
- [ ] AC17 Diff limited to actual D-091 granted UI/test paths; `PlayerApi.cs`, `Program.cs`, all Core numeric files, Assets and fixtures untouched.
- [ ] AC18 Exact approved candidate derived from `e7c1a57d`; no reviewed branch movement, lock collision or `main` merge.
- [ ] AC19 Builds/tests, frozen fingerprints and P0 pin unchanged; existing baseline smoke failures reported honestly, not papered over.
- [ ] AC20 Real Edge interactive proof and screenshot evidence at 1280×720 and 1920×1080, no CSP violation/5xx, capability URL never committed.
- [ ] AC21 Work Plan executes actual engine semantics; switching Light/inspection has zero ecological or queue side effects.
- [ ] AC22 Independent GPT-6.1 Sol review of exact candidate, clear verdict; owner separately playtests and accepts or requests corrections.

## Phase B (separately approved)
- [ ] AC23 Owner/G5 approves exact history comparison, state capture, code paths and no-save policy **before source authoring**.
- [ ] AC24 Before references a real pre-Execute state; After references successful actual execution; pending/approved-only cause no change.
- [ ] AC25 Year+1/+3 maps actual ecological evolution, with correctly aligned same-area grid cells and version IDs; no synthetic invented comparison.
- [ ] AC26 `Change` is mathematically signed and uses a *distinct* understandable visual encoding from absolute coral/peach light; handle increases/decreases/no change.
- [ ] AC27 Re-read, failures, multiple interventions, area switching, lack of baseline or incompatible model version are handled truthfully.
- [ ] AC28 No simulated no-work control claimed without a genuinely deterministic matched control run and separately approved implementation.
- [ ] AC29 Relevant checks/independent Sol review/owner acceptance recorded distinct from integration.

## Verdict slots (to be filled by actual actors)
**Opus implementation:** NOT STARTED  
**Sol independent review:** NOT STARTED  
**Owner playtest:** PENDING  
**G4 integration:** UNAUTHORISED
