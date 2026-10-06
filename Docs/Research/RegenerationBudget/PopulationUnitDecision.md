# What should regeneration abundance mean?

Decision support, not an accepted mechanic. Base/context and evidence limits: RegenerationFlow.md. The task packet forbids selecting a physical conversion without authority and requires a stop before an incompatible save-schema change.

Current implementation is species-relative density with normalized capacity sum(density/species.RegenDensityMax). It does not define an exact number of stems. One entire cohort becomes one exact tree regardless of its density. A legacy ground description labels density /m2, but that string alone cannot establish a physical population contract; implementing that interpretation would require new promotion and calibration rules.

| Option | Ecology/player meaning | Save impact | Performance | Promotion | Calibration burden / Scenario 2 |
|---|---|---|---|---|---|
| A. Stems/ha equivalent | Explicit expected physical juveniles; convert with cell area | Semantics/versioning and mixed-age state migration needed | Compact age bands; no seedling GameObjects required | Export physical stems with explicit fractional remainder, spatial placement and identities | High: seed-to-stem recruitment, mortality, spatial limits, exact-tree scale; most transferable physical reporting |
| B. Relative abundance/occupancy | Relative regeneration pressure/cover; UI must avoid stems/ha or /m2 | Existing scalar compatible in principle, but corrected mixed-age state still unresolved | Small compact state | Population conservation cannot span a count without a separate representation contract; disclose cohort export as representation conversion | Lower biological target burden, but future comparison with exact stocks still needs a defined bridge |
| C. Hybrid | Abstract juvenile abundance with explicit physical export rule | Conversion/version semantics and potentially age bands/remainders | Bounded cohorts plus exact adult budget | Define how much abundance one exact tree consumes, residual and identity; do not invent now | Medium/high: bridge must remain consistent across species, cell sizes, Scenario 2 |
| D. Bounded recruitment episodes | Treat a cohort as one representative recruitment episode; density measures its relative strength | Can reject biologically dissimilar new recruitment without new fields, but this changes meaning and admission; age bands need new saved records | Bounded episodes; low cost | One episode exports one representative tree; conservation is of episodes, not physical stems | New explicit abstraction/product choice; occupancy loss and exact count reported in separate units |

Recommendation for the next decision: explicitly choose whether the simulation must conserve physical stems, or only reconcile relative abundance and disclose representation conversion. Do not infer that one density unit equals one tree. Avoid displaying a physical unit until it is supported. This recommendation is not implementation authorization.

Mixed-age options:

- Subcohorts/age bands preserve new recruits' actual age/height, with bounded arrays rather than GameObjects. Existing loader collapses duplicate species records; a proper persisted contract/version and migration are required.
- Weighted height/year fit existing fields but erase age distribution. New seedlings would still immediately receive an averaged height, violating the packet's strict age/height requirement; age rounding also loses history.
- Merge only biologically similar recruits, explicitly rejecting other recruitment, can use current fields. It avoids inherited age but blocks late recruitment into an occupied same-species cell; this is a changed admission policy requiring acceptance and viability testing.
- Transient unsaved age bands would diverge after save/load and are unsuitable.

Required decision: approve a bounded same-age admission policy under the existing save contract, or commission/version persisted age bands with a migration proposal. Neither is silently selected here.

Research dependency (update 2026-10-06): the synthesis has since been located locally and is cited as supporting evidence in RepresentationProposal.md. Original note: CCF Primary Literature Synthesis v1 was not supplied or found in Git/local pasted attachments. Its task-packet summary is not a substitute for the complete source. No universal coefficients or physical density targets are proposed from it.
