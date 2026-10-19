# Guided rack onboarding: source review

Task: CCF-S1-GUIDED-ONBOARDING-RACK-UX-V1. Implementation BASE: `8f9ed8025243254fe7378b6d7771a5f1e7efaccc`. Locked context: `36cd791b329189e0ae38931d06ae51af1a1173ce`.

Actual source photographs were located under `/home/jer/Documents/CCF Game/CCF BOOK/`. All 45 JPEGs were visually surveyed in contact sheets; the following photographs were opened individually, including upright inspection derivatives, before gameplay implementation:

- `IMG20261006215922.jpg`: page 39, Forest infrastructure / Permanent racks; a forest access route photograph and permanent-access explanation. The surfaced route photograph is not evidence that a rack must be surfaced.
- `IMG20261006215941.jpg`: page 40, Rack spacing; photograph of a corridor along conifer rows and a contrasting less regular stand. The text explicitly makes spacing contextual rather than universal.
- `IMG20261006215946.jpg`: page 41, Rack width / Rack layout and orientation; photograph of a timber-loaded machine travelling through a corridor with standing trees on both sides. The text discusses original planting lines, cross-racking, terrain and avoiding damage to adjacent trees.
- `IMG20261006223003.jpg`: page 66, CCF planting example over time; overhead stand diagrams at years 4 and 20, the latter labelled with installed racks. This shows access routes in the context of retained tree cover rather than an isolated stripe.

The filenames and SHA-256 hashes of all 45 originals are recorded in the external evidence directory `ScenarioOneGuidedOnboardingRack-20261019T184956Z/source-review/manifest.json`. Original photographs and inspection derivatives are not shipped in the game or added to Git.

## Material decisions: evidence, inference and simplification

| Decision | A. Supplied source | B. CCF inference | C. Deliberate usability abstraction |
|---|---|---|---|
| Explain access before individual thinning | Pages 39–41 describe permanent access for harvesting/extraction and show trees retained beside a route. | Explain why the player opens access before other management. | One active lesson, short plain-language text; all original forest success targets remain available separately. |
| Original overhead rack diagram | Pages 40–41 show a corridor with surrounding stems; p.66 supplies stand/row context. | A strip between tree rows is a recognisable teaching cue. | Original triangles, clear strip, selected-origin dot and forward arrow drawn in UI Toolkit. Unscaled; no traced artwork, photo textures, machinery or numerical prescription. |
| Selected origin and forward direction | The photographs depict physical access corridors; they do not prescribe a mouse interaction or where a virtual segment must begin. | Letting a player deliberately choose a starting point is a sensible way to plan such a corridor. | A fixed transient start, pointer/aim rotation and a forward segment clipped to the existing stand boundary. This is an interaction abstraction, not a claim that real racks are one-ended or cannot connect. |
| Keep scenario dimensions and layout | The source discusses contextual spacing, width and orientation, including terrain limits. It does not mandate one universal measurement. | Retain CCF's existing authored rack layout and eligibility/quote rules. | Use the existing bounded-segment selector; no width, min/max length, spacing constant, machine system or forestry prescription is introduced. Map planning retains its existing whole-row operation. |

## Licensing

No licence to reproduce the book artwork has been established. No source pixels or traced geometry enter the shipped feature. The final diagram is original procedural artwork in `LearningObjectivesView.cs`.
