# Scenario One D-073 — consolidated requested wording options and Objective 2 discrepancy

**12 November 2026, Manager #424. Status: OPTIONS PRESENTED / USER CHOICE PENDING; OBJECTIVE 2 PRODUCT RULE NOT YET DECIDED.** This is a documentation-only companion to [the live personal playtest feedback](MANAGER-424-D073-LIVE-PERSONAL-PLAYTEST-FEEDBACK-20261112.md). No C#/Unity/scene/save or protected test candidate changes.

## A. All THREE user-requested three-way wording choices

### 1. Introducing Tree Marking — orange spray paint / entrance-tree instructions

**User's complaint:** the sentence speaks impersonally of 'one' spray-painting vertical stripes on all four sides of an entrance tree. The player is the forester; explain the painting plainly without altering what markings mean.

1. **Option 1:** “As the forester, you mark all four sides of the entrance tree with vertical stripes of orange spray paint.”
2. **Option 2:** “Use orange spray paint to mark the entrance tree with a vertical stripe on each of its four sides.”
3. **Option 3:** “The entrance tree is marked with orange spray-painted vertical stripes on all four sides, making the rack entrance easy to identify.”

**Manager's suggested default: Option 2. User selection not yet received.** Keep entrance-tree marking distinct from Crop/Fell marking and other orange racks. Source exact text and context to be verified before editing.

### 2. Introducing Tree Marking — replace “Marking cuts nothing”

**User's complaint:** remove the unnecessary intervening span from “A tree carries only one of these marks” through “change an orange mark”; preserve first one or two sentences. Replace “Marking cuts nothing” with natural language that still says marking is not removal and Work Plan approval is the commitment gate.

1. **Option 1:** “Marking a tree doesn't mean it will be cut immediately. Rack-clearance work must still be approved in the Work Plan.”
2. **Option 2:** “These markings help plan the work, but no trees are removed just by marking them. You must approve rack clearance in the Work Plan before the work can be carried out.”
3. **Option 3:** “Marking trees doesn't remove them. Rack clearance still needs your approval in the Work Plan.”

**Manager's suggested default: Option 3. User selection not yet received.** Do not imply approval itself cuts trees: it commits pending work; the separate *Advance one year* executes it. Before deleting the requested span, inspect exact source to ensure unique safety instructions remain represented elsewhere.

### 3. Annual Review — replace “One thinning does not finish continuous-cover forestry”

**User's complaint:** the trailing footer sounds unnatural and can incorrectly imply the player just performed a thinning when the first Annual Review may follow rack clearance. Also remove the redundant “Read WORK DONE, MONEY and FOREST.” sentence while retaining actual section headings and acknowledgement control.

1. **Option 1:** “Managing a forest is an ongoing process. See how the forest responds before deciding what to do next.”
2. **Option 2:** “Each year brings new changes. Return to the forest, inspect the results of your work and use what you learn to plan your next steps.”
3. **Option 3:** “Continuous-cover forestry involves managing and adapting the forest over time. Each intervention is followed by observation, reassessment and further decisions.”

**Manager's suggested default: Option 1. User selection not yet received.** Actual source: `Assets/ForestPrototype/UI/MenuHelpView.Text.cs`, `AnnualReviewReadPrompt`; it is rendered inside `AnnualReviewView.cs`. Keep the real “I've read the annual results” button, unread state and advance gate.

## B. Related fourth COPY request — previously one suggested replacement only

For the same **Introducing Tree Marking** last paragraph, the player objected to an abrupt all-caps “CONFIRMED RACK PLAN” introduction. The single suggested rewrite is:

> “Now that you've confirmed your rack plan, walk to a rack entrance and press P to begin marking it.”

**Not one of the three requested alternative groups.** User has not confirmed this exact final wording. Display P using the same restrained keycap as M/Esc/Tab where contextually relevant.

## C. Objective 2 after rack work — user reports old three-tree rule still visible

**User report at 22:09 UTC:** on reaching **Objective 2 (tree marking / choosing trees)**, the objective still refers to “one tree and three of the five racks” (spoken shorthand); user remembers previously discussing a change and requests that it change.

**Verified current protected candidate source, not inferred from phrasing:**
- `Assets/ForestPrototype/UI/ScenarioOneTeachingSequence.cs`: Lesson 2 title “Choose promising trees to retain along the rack”; requires **three Crop Trees**, one in **near/middle/far teaching zones**, >=8m apart, both sides of a rack represented.
- `Assets/ForestPrototype/UI/ScenarioOneRackBayText.cs`: the native lesson HUD says “Choose one Crop Tree in each zone, 8 m apart; include both rack sides” and displays `Selections n/3 · zones n/3`.
- `Assets/ForestPrototype/UI/LearningObjectivesView.Sequence.cs`: the actual V2 teaching **sample** is derived from the **third confirmed rack**, not three separate racks. `ScenarioOneRackBay` restricts the sample footprint and completed selection checks.
- The player paraphrase “one tree and three of the five racks” must **not** be treated as proof that the program literally asks one tree on each of three different racks. The *current rule* is three trees within the designated sample of one rack.
- Earlier independent [Fable #362 UX inspection](https://github.com/jermur788/CCF/blob/0d195232778d5e53e3bf0121f051313d9379604e/Docs/Handoffs/ScenarioOne-Playtest-Feedback-20261110/FABLE-52-SCENARIO-ONE-UX-ACCESS-DESIGN-REVIEW-RESULT.md) confirmed zones are not visible; recommended making zone boundaries and route legible, with changing the actual rule reserved for new spec. [Manager #200 ruling](https://github.com/jermur788/CCF/blob/main/Docs/Handoffs/ScenarioOne-Playtest-Feedback-20261110/MANAGER-362-DESIGN-RULING-AND-SOURCE-POSTSCRIPT-20261110.md) explicitly **retained three trees in three thirds as an interim rule**, pending product decision. Accordingly this remains in the candidate; there is **no evidence the previously discussed UX repair amounted to an approved different learning rule**.

**Current user instruction supersedes the interim direction as a request to revisit Objective 2**. It does not specify a replacement number, distribution or judgement gate. **Do not invent a new number of racks/trees, change source, or pretend the requested update is implemented.** Prepare a separate **SPEC REQUIRED** change analysis through Overall Development Manager [#425](https://github.com/jermur788/CCF/issues/425) covering representative sample, objectives 2–5, MFM v1 D-065 prescription, 8 m competition spacing, lesson review and existing saved progression/Legacy40/frozen Reference. Clear design choices include (a) same 3 trees along one *visibly explained* rack sample; (b) distribute three trees across several racks; or (c) new flexible representative judgement criterion. These are possibilities to clarify with user, **not proposed accepted options or equivalent scope**.

**Exact needed user decision:** what should Objective 2 ask the player to do instead of the current “three Crop Trees in near/middle/far thirds beside the third rack”? Is the requested change only how the lesson explains/locates the trees, or is the selection rule to change across racks? No worker is authorised while personal Editor test remains active.

## D. Execution/status boundaries

These four copy topics belong to the UI repair batch *after* live playtest and saved-data/lease safe handback, with independently reviewed bounded test evidence. Objective 2 rule change is a separate product/spec gate, not casually folded into text edits. Earlier [#438 first-rack entrance navigation/identity triage](https://github.com/jermur788/CCF/issues/438) remains high priority and may be safety relevant; 5/5 UI-marked count alone does not prove rack IDs are correctly mapped.

**No Windows/Player build, no main integration, no user acceptance and no active Editor source changes** are authorised by this file. Dashboard/Drive mirror sync unverified.
