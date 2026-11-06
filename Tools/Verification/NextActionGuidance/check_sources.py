#!/usr/bin/env python3
"""T2 source contracts only. No Unity, input, render, save or gameplay execution.
Run from any directory: python3 Tools/Verification/NextActionGuidance/check_sources.py
Existing historical qualification assertions are deliberately not rewritten.
"""
from pathlib import Path
import re
import subprocess

ROOT = Path(__file__).resolve().parents[3]
UI = ROOT / 'Assets/ForestPrototype/UI'
root = (UI / 'ScenarioOneUiRoot.cs').read_text()
learning = (UI / 'LearningObjectivesView.cs').read_text()
stand = (UI / 'StandMapView.cs').read_text()
work = (UI / 'WorkPlanView.cs').read_text()
uss = (UI / 'Resources/ScenarioOneUi.uss').read_text()
selector = root[root.index('public void RefreshNextActionGuidance()'):root.index('private void HandleKeys()')]
results = []

def check(ok, name):
    results.append(bool(ok))
    print(('PASS' if ok else 'FAIL') + ' [SOURCE ONLY] ' + name)

check('private Button nextAction;' in root and 'candidate.AddToClassList("btn-primary")' in selector
      and selector.count('candidate.AddToClassList("btn-primary")') == 1, 'single selected cue reference and assignment')
check('candidate.enabledInHierarchy && GuidanceVisible(candidate, scope)' in selector,
      'eligibility checks enabled state and ancestor visibility')
check('view.Query<Button>(className: "btn-primary")' in selector and 'RemoveFromClassList("btn-primary")' in selector,
      'competing static primaries removed in owned views')
check('candidate.text = "Next: " + candidate.text' in selector and '.btn.next-action { font-size: 20px; border-width: 3px;' in uss,
      'text plus heavy border cue and 20-unit source floor')
check('.next-action-reason { font-size: 20px;' in uss and 'explanation.text = reason;' in selector,
      'visible reason/instruction with 20-unit source floor')
check(all(x in selector for x in ['manager.ReferencePreviewActive', 'StandaloneSessionMenu.IsOpen', 'help.IsOpen',
                                'helpClosedFrame == Time.frameCount']), 'reference/session/Help/same-frame suspension')
check('Button map = UiKit.Button("Stand map [M]", () => ui.ShowMap());' in learning
      and 'map.name = "objective-primary-cta";' in learning,
      'Objectives map destination is existing ShowMap action')
check('if (objectives) target = "objective-primary-cta";' in selector, 'unconfirmed Objectives prioritise map')
check('if (!map.RackPlanningAvailable)' in selector and 'else if (!map.RackMode) target = "map-plan-rack";' in selector,
      'Plan Racks entry only before planning and with availability gate')
check('map.RackEditor.Dragging || map.RackEditor.Ghost.HasValue' in selector,
      'manual gesture suppresses Confirm cue')
check('manager.CanConfirmRackPlan(out string problem)) target = "map-rack-plan-confirm";' in selector
      and 'else reason = problem;' in selector, 'valid Confirm vs existing invalid/count validator reason')
check('can && !editor.ConsentPending && !editor.ResetPending' in stand,
      'Confirm remains disabled under either rack question')
check('manager.MarkedRackEntranceCount < ScenarioOneManager.RackPlanTargetCount' in selector
      and 'manager.TryGetRackEntranceTarget' in selector and 'rack.EntranceMarked || rack.IsLocked' in selector,
      'field return requires an unmarked unlocked markable entrance')
check('string problem = RackPlanProblem;' in selector and 'problem = RackApprovalProblem;' in selector,
      'approval cues use both existing teaching gates')
check('m.CanEditWorkPlan && string.IsNullOrEmpty(rackProblem) && string.IsNullOrEmpty(approvalProblem)' in work,
      'existing approval enabled predicate preserved')
check('rackLayout.WorkPending' in selector and 'including jobs outside the racks' in selector,
      'approval follows rack-owned Pending and discloses whole-plan commitment')
check('!quote.Eligible || cost > manager.CashCents' in selector and 'reason = order.validationMessage' in selector,
      'blocked approval uses quote/cash/order reasons')
check('rackLayout.Complete && approved && !failed && manager.CanAdvanceYearNow && ecology != null' in selector,
      'annual cue requires complete rack coverage and actual approved unresolved work, no failed work')
check(selector.index('if (NeedsAnnualReview)') < selector.index('else if (!manager.RackPlanConfirmed)'),
      'annual review takes precedence over progression')
check('if (consent && reset)' in selector and 'Two rack questions are open' in selector
      and selector.index('if (consent && reset)') < selector.index('else if (consent) target'),
      'dual prompt has instruction and no selected target')
check(all(x in selector for x in ['target = "map-consent-decline"', 'target = "map-edit-reset-cancel"',
                                'target = "map-reopen-decline"']), 'single rack prompts select safe keep/decline only')
check('choices.Add(no);\n        choices.Add(yes);' in stand
      and stand.count('choices.Add(no);\n            choices.Add(yes);') == 2
      and 'accept.RemoveFromHierarchy();\n            card.Add(accept);' in stand,
      'single safe first; dual consent decline then reset cancel before destructive choices')
check('safeReturn.name = "workplan-advance-return";' in work and 'target = "workplan-advance-return"' in selector
      and 'AdvanceGuard.ReturnLabel, CancelAdvanceConfirmationAndRefresh, true, "btn-primary"' in work,
      'Advance warning retains its safe Return primary')
check('if (focused != null && scope.Contains(focused)) focused.Blur();' in selector
      and 'ScenarioOneUiRoot.ClearPromptFocus(Root)' in stand and 'if (confirmingAdvance) ScenarioOneUiRoot.ClearPromptFocus(Root);' in work,
      'bounded prompt rebuild focus clearing; no restore')
check('Time.unscaledTime >= nextGuidanceRefresh' in root and 'RefreshNextActionGuidance();' in root[root.index('private void LateUpdate()'):root.index('public void RefreshNextActionGuidance()')]
      and all('ui.RefreshNextActionGuidance();' in s for s in [learning, stand, work]),
      'fresh derivation on frame and view refresh paths')

check('NeedsAnnualReview && manager.AnnualReports.Count == 1' in selector and 'rackLayout.Harvested' in selector
      and 'target = scope == review.Root ? "annual-review-acknowledge"' in selector,
      'first resolved rack results remain cued across authoritative teaching-topic transition')
check('new[] { learning.Root, map.Root, workPlan.Root, review.Root }' in selector,
      'review competing primaries also demoted through owned root')

base = 'e1be15c17a403bc78130a7f01a3248d537960fd8'
def git(*args):
    return subprocess.check_output(['git', '-C', str(ROOT), *args], text=True)
added = '\n'.join(l[1:] for l in git('diff', base, '--', 'Assets').splitlines() if l.startswith('+') and not l.startswith('+++'))
check(not re.search(r'(?i)\bF6\b|f6Key|focusable\s*=\s*true|PlayerPrefs\.|SaveData', added),
      'no added F6, focus opt-in, preference or save fields/keys')
check(not re.search(r'\.(Focus|ConfirmRackPlanFromMap|MarkRackEntrance|ApprovePendingWork|AdvanceYear)\(', selector),
      'cue selector never focuses or executes world/plan actions')
check('Review rack guidance' not in added, 'no invented guidance control')
allowed = {f'Assets/ForestPrototype/UI/{p}' for p in ['LearningObjectivesView.cs', 'StandMapView.cs',
    'WorkPlanView.cs', 'ScenarioOneUiRoot.cs', 'Resources/ScenarioOneUi.uss']}
changed = set(git('diff', base, '--name-only', '--', 'Assets', 'Packages', 'ProjectSettings').splitlines())
check(changed <= allowed, 'production diff restricted to five owned source/style files')
check(not git('ls-files', '--others', '--exclude-standard', '--', 'Assets').strip(),
      'no temporary Assets scripts or generated metadata')
print(f'T2_SOURCE_TALLY PASS={sum(results)} FAIL={len(results)-sum(results)}; runtime/render/input NOT RUN')
raise SystemExit(0 if all(results) else 1)
