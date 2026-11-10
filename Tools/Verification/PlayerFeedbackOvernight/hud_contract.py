#!/usr/bin/env python3
"""#365 source contract (NOT a HUD screenshot): the walking objective panel must take its words from the one pure selector
that Checks.cs drives (ScenarioOneRackBayText.HudWords), and nothing else may call the Objective 2 Crop Tree checklist.
Fail-closed: a missing file or pattern is a failure. Used by run_checks.py and, with a deliberate regression, by mutations.py."""
import hashlib, pathlib, re, sys

def problems(ui: dict) -> list:
    """ui maps a path relative to Assets/ForestPrototype/UI (posix) to that file's text."""
    out = []
    seq = ui.get('LearningObjectivesView.Sequence.cs', '')
    m = re.search(r'public ActiveObjectiveHudWidget\.Model HudModel\(\)\s*\{(.*?)\n    \}\n', seq, re.S)
    body = m.group(1) if m else ''
    if not m: out.append('HudModel() not found')
    call = 'ScenarioOneRackBayText.HudWords(rackV2, index, NeedsReviewAcknowledgement, ui.NeedsAnnualReview, complete,'
    if body.count(call) != 1: out.append('HudModel must call HudWords once with the lesson index and both review predicates')
    if 'bool rackV2 = ui.Manager.RackPlanningV2Active && !ui.Manager.ReferencePreviewActive;' not in body: out.append('HudModel must gate on the Rack V2 world')
    for field in ('Title = title,', 'Instruction = instruction,', 'Progress = progress,'):
        if body.count(field) != 1: out.append('HudModel must pass through ' + field)
    if 'out string title, out string instruction, out string progress' not in body: out.append('HudModel must take all three words from HudWords')
    if 'Walking(' in body or 'Mark Crop Tree' in body: out.append('HudModel must not build the Crop Tree checklist itself')
    text = ui.get('ScenarioOneRackBayText.cs', '')
    calls = [l.strip() for l in text.splitlines() if re.search(r'(?<![A-Za-z_])Walking\(', l) and 'public static string Walking(' not in l and not l.strip().startswith('//')]
    if calls != ['else if (bay != null && hasPlayer && lesson == 1) progress = Walking(bay, x, z, aimed);']:
        out.append('the Crop Tree checklist must be called exactly once, for lesson index 1: ' + repr(calls))
    for rel, t in ui.items():
        if rel != 'ScenarioOneRackBayText.cs' and (re.search(r'(?<![A-Za-z_])Walking\(', t) or 'Mark Crop Tree"' in t):
            out.append(rel + ' must not call or spell the Crop Tree checklist')
    root = ui.get('ScenarioOneUiRoot.cs', '')
    if 'else if (!suspended && manager.RackPlanningV2Active && screen == UiScreen.None' not in root: out.append('walking panel border must be limited to a Rack V2 world')
    if 'reason = scope == learning.Root ? "" : learning.CurrentShortInstruction;' not in root: out.append('later objectives must show the short instruction, once')
    view = ui.get('LearningObjectivesView.cs', '')
    if 'instruction != extension' not in view or view.count('ExtensionInstruction()') != 1: out.append('the extension checklist must be added to the card once')
    # #367: pin the actual consumers, not just HudWords. Exact signatures fail closed on missing/rewired paths.
    contracts = (
        ('R367_REVIEW_FIRST_BOTH_PREDICATES', 'public bool ReviewFirst => ScenarioOneRackBayText.ReviewFirst(ui.Manager.RackPlanningV2Active && !ui.Manager.ReferencePreviewActive,\n        NeedsReviewAcknowledgement, ui.NeedsAnnualReview);'),
        ('R367_SHORT_SHARED_PRIORITY', 'public string CurrentShortInstruction => ReviewFirst ? ScenarioOneRackBayText.ReviewInstruction'),
        ('R367_FULL_SHARED_PRIORITY', 'if (ReviewFirst) return ScenarioOneRackBayText.ReviewInstruction;'),
        ('R367_PROGRESS_SHARED_PRIORITY', 'public string CurrentProgress => ReviewFirst ? ScenarioOneRackBayText.ReviewProgress : TeachingProgress(TeachingFacts());'),
        ('R367_CARD_SHORT_CONSUMER', 'AddObjectivePart(words, "What to do", shortInstruction, "current-objective-instruction");'),
        ('R367_CARD_FULL_CONSUMER', 'if (instruction != shortInstruction && instruction != extension) AddObjectivePart(detail, "Full instructions", instruction, "objective-full-instruction");'),
        ('R367_CARD_REVIEW_BANNER', 'if (ReviewFirst) UiKit.Add(words, "First: Review the year — read and acknowledge the annual results.", "heading").name = "current-objective-review-first";'),
        ('R367_CARD_PROGRESS_CONSUMER', 'UiKit.Add(words, CurrentProgress, "heading").name = "current-objective-progress";'),
        ('R367_CARD_NO_PARALLEL_EXTENSION', 'string extension = !ReviewFirst && ExtensionActive ? ExtensionInstruction() : null;'),
        ('R367_CARD_NO_PARALLEL_PRACTICE', 'if (!ReviewFirst && !string.IsNullOrEmpty(practice))'),
    )
    for name, pattern in contracts:
        if view.count(pattern) != 1: out.append(name)
    if root.count('if (learning.ReviewFirst)') != 1: out.append('R367_WINDOW_TARGET_SHARED_PRIORITY')
    if root.count('reason = scope == learning.Root ? "" : learning.CurrentShortInstruction;') != 1: out.append('R367_WINDOW_REASON_SHORT_CONSUMER')
    if '=> rackV2 && (lessonReviewPending || annualReviewUnread);' not in text: out.append('R367_SELECTOR_NEWEST_UNREAD')
    if 'bool review = ReviewFirst(rackV2, lessonReviewPending, annualReviewUnread);' not in text: out.append('R367_HUD_SHARED_PRIORITY')
    # Objective1's confirmation/marking/approval navigation is outside this repair; pin its exact existing branch.
    start = root.find('        else if (manager.RackPlanningV2Active && (learning.TeachingRacks')
    end = root.find('        else if (manager.RackPlanningV2Active)\n', start)
    if start < 0 or end < start or hashlib.sha256(root[start:end].encode()).hexdigest() != '5fd30222c998e60fceca03d19b7f1450265d257497896916e4b9dbd72999a7f9':
        out.append('R367_OBJECTIVE1_NAVIGATION_UNCHANGED')
    return out

def read(src_root) -> dict:
    base = pathlib.Path(src_root) / 'Assets' / 'ForestPrototype' / 'UI'
    return {p.relative_to(base).as_posix(): p.read_text(encoding='utf-8') for p in sorted(base.rglob('*.cs'))}

if __name__ == '__main__':
    found = problems(read(sys.argv[1]))
    for f in found: print('HUD SOURCE CONTRACT FAIL: ' + f)
    if found: sys.exit(1)
    print('HUD source contract PASS (source text only, not a rendered HUD)')
