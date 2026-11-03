# CCF Overall Manager transition — personal playtest / intro visuals
**Date:** 2026-11-03. **Status:** personal Scenario One playtest in progress; opening-screen feedback unresolved. **GitHub is canonical.**

## Authority and candidate
Act as Overall Manager: user instructions > `Docs/Project/decision-log.md` > `current-milestone.md` > `game-brief.md`. Check live `Docs/Project/coordination/active-tasks.md` before assigning work; browser Manager owns it under #200. One writer per file set; serialize Unity.

Scenario One three-species complete-loop demo is the milestone. Rack V2, Sitka/Oak/Beech parity, S1-C/S1-D, MFM V1, final Tier-3 (#231/#240), and Stand Map canopy differentiation (#232/#242) are Manager-approved/integrated. The **published player-test candidate** is `integration/scenario-one-player-test-ready-v1`. The user has started the personal playtest, **not completed or accepted it**. **No Windows build** before finished personal playtest and explicit subsequent authorization. Protect frozen Reference Future v1 and Legacy40. MFM-11 post-treatment Covered CI parity remains NOT DEMONSTRATED; three pure-Beech R2 misses are accepted limitations. Keep user-directed test reduction.

## First playtest feedback — intro pages
User supplied three Unity screenshots in the preceding manager chat (files `screen1.png`, `screen2.png`, `screen 3.png`). These screenshots and generated image mockups are **conversation assets, not committed production assets**; request reattachment if unavailable in the new chat.

**“What is Continuous-Cover Forestry?”** Add **one** explanatory line; options:
1. “The forest changes gradually: mature trees remain while younger trees establish beneath and between them.”
2. “Over repeated interventions, different ages and species develop together without resetting the stand.”
3. “Transformation happens in stages, with older trees retained as new generations establish around them.”

Replace the **middle and right** isometric forest illustrations: the middle currently suggests front-half clearfell followed by regeneration; the right seems a more mature repetition. Both must show **gradual selective transformation and retained mature overstorey throughout**. Critical repeated user correction: **do not put all saplings at the front and older trees at the back**. Every panel is a **slice through a continuous forest**, with mature trees, intermediate trees, and regeneration distributed across front, middle, and rear, not arranged as an age gradient or exposed forest edge.

**“How CCF works over time”**: card 2 central sapling has an unwanted blue glow; cards 2/3 have visually doubled/stacked icons; card 4 text overflows. Suggested compact card-4 copy: “Encourage regeneration and establish other species beneath retained trees.” Use clean distinct tree-selection and canopy-opening icons.

**Scenario One introduction**: right target-state image needs **several tall, imposing, retained mature Sitka** alongside established Oak/Beech and scattered young regeneration; do not imply spruce are all juvenile or that saplings occur exclusively at the front.

## Current asset status / next action
Multiple browser-generated composite mockups were iterated, but the user continued correcting foreground-age bias; **none is approved for implementation**. Generate/review **separate replacement illustrations** rather than replacing full UI screens with image-generated text. Retain existing UI layout/copy except approved revisions. Obtain user approval of visuals and selected copy, then create a bounded GitHub task claim + durable implementation packet for a local Unity/UI agent. Require exact asset paths, original/replacement comparison, text overflow fixes, affected rendered checks and independent review. Record subsequent playtest findings separately; keep player-test acceptance and Windows-build approval gated.