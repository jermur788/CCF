# Tutorial copy bank (proposed beginner-facing text)

**Status:** PROPOSED COPY ONLY. **Not in production.** For user approval before Packet 1. Every line is checked against the simulation at `3e4ee40`: no text claims a mechanism the game lacks (D-020). Lines marked † describe real-world practice and must keep their framing ("in real forests…").

Style: second person, present tense, short sentences, no jargon without a gloss, never "correct"/"wrong".

## 1. Core terms (first-use explanations)

| Term | Copy |
|---|---|
| **CCF** | **Continuous-cover forestry:** cultivate the forest through repeated selective management while keeping continuous tree cover and renewal. You never clear the whole stand. You work tree by tree, come back, and work again. |
| **Crop Tree** | A **Crop Tree** is a tree you choose to keep and help grow into valuable timber. Foresters often call it a *quality tree*. Choose healthy trees with room to grow. † In real forests you would also check that the stem is straight and the branches are fine; this game does not model stem shape yet. |
| **DBH** | **DBH** is the trunk's diameter at breast height (1.3 m). Foresters measure it because it is quick and shows how fast a tree is growing. |
| **Competition** | **Competition:** how strongly neighbours hold back a tree's growth. Bigger and closer neighbours count most; small or distant ones count little. |
| **Regeneration** | **Regeneration:** young trees growing up from seed or from planting. In CCF this is how the next forest arrives — a little at a time, not all at once. |
| **Browsing** | **Browsing:** deer eating the leading shoots of young trees. It can hold a young tree at deer-mouth height for years. In this game the deer pressure is fixed, and tree shelters protect single planted trees. |
| **Enrichment planting** | **Enrichment planting:** planting a few trees of species your forest cannot supply by itself. Here, there are no oak or beech seed trees, so planting is the only way to add them. Plant where there is enough light for them to grow. |
| **Selective thinning** | **Selective thinning:** removing chosen trees so the trees you keep have more room. In CCF it is light and repeated, not one heavy cut. |
| **Pruning** | **Pruning:** removing the lower branches of a Crop Tree so the new wood grows without knots. It is for future timber quality, not for the tree's health. (This scenario does not yet pay more for pruned timber.) |
| **Clearance** | **Clearance:** a contractor cuts back everything low in a 5 × 5 m square — ferns, brambles, **and any young trees there, including seedlings you may want to keep**. Standing trees are not touched. Plants grow back in later years. This version of the game does not yet simulate weeds slowing young trees, so clearing does not make seedlings grow faster here. † In real forests clearance is used to stop bracken or bramble smothering newly planted trees. |
| **Annual Review** | **Annual Review:** what actually happened this year — the work done, the money, and how the forest changed. Use it to decide where to look next. |
| **Stand Map** | **Stand Map:** a map that helps you find where to look. It does not make the forestry decision for you. Set a waypoint, then walk there and judge the trees yourself. |
| **Work Plan** | **Work Plan:** where your marks become jobs. Check what each job costs and what you will leave behind, then approve. Nothing happens until you advance the year. |
| Basal area † | **Basal area:** the total cross-section of all the trunks at breast height, per hectare. A quick measure of how much growing stock is standing. |
| Seed source | **Seed source:** a tree old enough to produce seed. In this game your Sitka start producing seed at about age 20 and are in full production by 30 — they are just reaching that age now. |
| Deadwood | **Deadwood:** dead trees left in the forest. Many fungi, insects and birds depend on it. Leaving a felled stem earns no money but keeps that habitat. |
| Fell mark | **Red Fell mark:** a proposal to remove this tree. Nothing is cut until you approve the job and advance the year. Press X again to remove the mark. |

## 2. Stage copy (title · what to do · why it matters)

Stages follow `ProposedTutorialArc.md`.

| Stage | Title | What to do | Why it matters |
|---|---|---|---|
| S0 | **Your forest** | Walk into the trees and look around. | This 40 × 40 m plantation was planted 20 years ago, all Sitka spruce, all the same age. Your job over the coming decades is to turn it into a varied, continuous forest — without ever clearing it. |
| S1 | **Read the plantation** | Look at the ground, then press E on any tree. | The ground is dark and bare: the canopy is closed, and the trees are only now old enough to make seed. Knowing why nothing is growing yet tells you what will change. |
| S2 | **Find a tree worth keeping** | Inspect a few trees. Press C on one you want to keep and grow. | CCF marking starts with the trees you want to keep, not the ones you want to cut. Everything else follows from that choice. |
| S3 | **What is holding it back?** | Inspect the neighbours around your Crop Tree. | Some neighbours take a lot of its space; small ones further away take very little. Removing a tree that isn't really competing costs money and gains nothing. |
| S4 | **Mark a light thinning** | Press X on the neighbours you think should go — or choose "No thinning this year" in the Work Plan. | Thinning gives your Crop Trees room to grow. Light and repeated is the CCF way: you will be back. Doing nothing this year is also a real choice. |
| S5 | **What are you leaving behind?** | Open the Work Plan (Tab) and read "What you are leaving" next to the costs. | A thinning is judged by the forest it leaves, not just the timber it takes. Early thinnings rarely pay for themselves; their value is in the trees that remain. |
| S6 | **Approve and advance** | Approve the job, then advance one year. | Approving commits the plan; advancing carries out the work and grows the forest for a year. |
| S7 | **What happened?** | Read Work Done, Money and Forest, then confirm you've read them. | Linking what you did to what changed is how foresters learn their forest. |
| S8 | **Find your Crop Tree again** | Open the map (M), use the Fell & crop marks layer, set a waypoint and walk to your Crop Tree. | The map finds places; you judge them on foot. Go and see what your thinning did. |
| S9a | **Where does new forest come from?** | Look at a cell with young trees and open the Regeneration map layer. | Seedlings appear where seed reaches the ground and there is enough light. Here, that means Sitka — your forest's own seed. |
| S9b | **Natural seedlings or planting?** | Compare cells with and without seedlings. Plant broadleaves only where there is light, or decide "not now". | Natural regeneration renews the forest for free, but it cannot add species that have no seed trees. Planting a few oak or beech adds diversity, and decades later they become seed trees themselves. |
| S9c | **Deer and young trees** | Decide whether your planted trees need shelters, looking at the cost. | Oak is a deer favourite. Unprotected oaks can be held back for years; a shelter protects one tree for several years. |
| S9d | **What clearance really does** | Read the clearance preview before you approve one. | Clearance removes ground plants *and* young trees in its square, for a cost. Use it on purpose, not everywhere. |
| S9e | **Pruning your Crop Trees** | Plan a pruning lift on an eligible Crop Tree, or decide "not now". | Pruning grows knot-free timber on the trees you plan to keep longest. It costs money now for quality later. |
| S10 | **Come back and look again** | Five years on: revisit your Crop Trees, inspect their neighbours again and mark a second thinning. | The forest has changed: your Crop Trees grew, their neighbours grew too, and the light you opened is closing. CCF is a cycle, not a single job. |
| S11 | **Compare your two thinnings** | Open History in the Annual Review and compare the two intervention years. | Seeing how the forest responded to each intervention is how you plan the next one. |
| S12 | **Your plan** | Plan an intervention entirely your own way and read its review. | There is no single right answer in CCF. Different choices leave different, valid forests. Know what you chose, and why. |

## 3. Short contextual lines

| Where | Copy |
|---|---|
| Ground "Why", dark and no seed | Too dark for seedlings, and no seed is reaching this spot yet. |
| Ground "Why", dark but seeded | Seed is arriving, but it is too dark here for seedlings to establish. |
| Relationship line (P2) | Competes with your Crop Tree P0707: strong — the largest of its 48 neighbours, 8 % of its competition. |
| Forecast line (P2) | Your Crop Trees: competition −20 %, growth +12 % (16 of 16 get more space). Whole stand: growth +7 %. |
| Contractor minimum | This visit costs at least €2,500 however few trees you fell. First thinnings of small trees rarely cover that. |
| Why both broadleaves are planted | There are no oak or beech trees on this property to spread seed, so planting is the only way to add them. |
| Wind (until recalibrated) | *(hide the band; optionally show:)* Height is about 73 times the diameter — a slender tree. |
| Practice banner | PRACTICE COPY — nothing here changes your forest. |
| Long-term preview banner | Practice, Year 20 of Plan A. No further work was done in this preview. |
| History "inspect next" | Cell C4: the oak you sheltered in Year 2 — its shelter has 4 years left. |
