#!/usr/bin/env python3
"""#443 focused contracts + executable production method probes. No Unity; output stays on Files.

Small fake runtime neighbours let the real visit/target/spray methods execute on .NET.
They do not qualify real raycasting, orders, rendering or native input (G5 does).
"""
import argparse
import os
from pathlib import Path
import re
import subprocess
from xml.sax.saxutils import escape

ROOT = Path(__file__).resolve().parents[3]
UI = ROOT / 'Assets/ForestPrototype/UI'
BASE = '55863f1b'
ALLOWED = {'Assets/ForestPrototype/' + p for p in (
    'ForestTreeMarkingManager.RackEntrances.cs', 'UI/ScenarioOneUiRoot.cs', 'UI/LearningObjectivesView.cs',
    'UI/ScenarioOneRackBayText.cs', 'UI/StandMapView.cs', 'UI/Map/StandMapSource.cs', 'UI/HudMinimapView.cs',
    'UI/WalkingHudView.cs', 'UI/ActiveObjectiveHudWidget.cs', 'UI/MenuHelpView.cs', 'UI/MenuHelpView.Text.cs',
    'UI/WorkPlanView.cs', 'UI/Resources/ScenarioOneUi.uss', 'ScenarioOne/ScenarioOneManager.RackPlan.cs')}
checks = []
def check(ok, why):
    checks.append(bool(ok)); print(('PASS ' if ok else 'FAIL ') + why, flush=True)
def git(*args):
    return subprocess.check_output(['git', '-C', str(ROOT), *args], text=True)
def method(text, signature):
    start = text.index(signature)
    brace = text.index('{', start)
    arrow = text.find('=>', start, brace)
    if arrow >= 0: return text[start:text.index(';', arrow) + 1]
    depth = 0
    for i in range(brace, len(text)):
        depth += (text[i] == '{') - (text[i] == '}')
        if depth == 0: return text[start:i + 1]
    raise ValueError(signature)

def rack_plan_copy_only(candidate, baseline):
    # Direct user F-1 decision: this fourteenth production file permits exactly two copy lines.
    replacements = (
        ('        feedback = $"Marked the entrance tree {rack.entranceTreeId} for rack {rackId}.";',
         '        feedback = $"Marked the entrance tree {rack.entranceTreeId}.";'),
        ('        feedback = $"Cleared the entrance mark for rack {rackId}.";',
         '        feedback = "Cleared the entrance mark.";'))
    for old, new in replacements:
        if baseline.count(old) != 1:
            return False
        baseline = baseline.replace(old, new)
    return candidate == baseline

def planning_border(styles, planning=True):
    ancestors = [{'modal-map', 'modal'}]
    card = {'card', 'rack-panel'} | ({'map-planning-instructions'} if planning else set())
    winner = (-1, -1, -1, '')
    for order, rule in enumerate(re.finditer(r'([^{}]+)\{([^{}]*)\}', styles)):
        selectors, declarations = rule.groups()
        for selector in selectors.split(','):
            parts = selector.strip().split()
            if not parts or any(not re.fullmatch(r'(?:\.[\w-]+)+', part) for part in parts):
                continue
            classes = [set(re.findall(r'\.([\w-]+)', part)) for part in parts]
            if not classes[-1] <= card:
                continue
            remaining = iter(reversed(ancestors))
            if not all(any(required <= ancestor for ancestor in remaining) for required in reversed(classes[:-1])):
                continue
            for declaration, item in enumerate(declarations.split(';')):
                name, separator, value = item.partition(':')
                if separator and name.strip() in ('border-color', 'border-left-color'):
                    winner = max(winner, (sum(len(group) for group in classes), order, declaration, value.strip()))
    return winner[-1]

def main():
    ap = argparse.ArgumentParser(); ap.add_argument('--out', type=Path, required=True); a = ap.parse_args()
    a.out.mkdir(parents=True, exist_ok=True)
    root = (UI/'ScenarioOneUiRoot.cs').read_text(); hud = (UI/'WalkingHudView.cs').read_text()
    mini = (UI/'HudMinimapView.cs').read_text(); source = (UI/'Map/StandMapSource.cs').read_text()
    marking_path = 'Assets/ForestPrototype/ForestTreeMarkingManager.RackEntrances.cs'
    marking = (ROOT/marking_path).read_text(); baseline = git('show', BASE + ':' + marking_path)
    for signature in ('public RackPlanOperationResult TrySprayRackEntrance()', 'private bool TryRackSprayAim(', 'public bool EquipRackSpray('):
        check(method(marking, signature) == method(baseline, signature), signature + ' byte-for-byte unchanged')
    changed = set(git('diff', '--name-only', BASE+'..HEAD', '--', 'Assets', 'ProjectSettings', 'Packages').splitlines())
    changed |= set(git('diff', '--name-only', 'HEAD', '--', 'Assets', 'ProjectSettings', 'Packages').splitlines())
    check(changed <= ALLOWED, 'production allowlist from git diff --name-only 55863f1b..HEAD (also pending edits): ' + ', '.join(sorted(changed-ALLOWED)))
    rack_path = 'Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.RackPlan.cs'
    rack_plan = (ROOT/rack_path).read_text(); old_rack_plan = git('show', BASE + ':' + rack_path)
    check(rack_plan_copy_only(rack_plan, old_rack_plan), 'F-1 allowlist exception: entire manager file equals base with exactly the two authorised feedback lines')
    check(not rack_plan_copy_only(rack_plan + '// unrelated change\n', old_rack_plan)
          and not rack_plan_copy_only(old_rack_plan, old_rack_plan)
          and not rack_plan_copy_only(rack_plan.replace('Applied(feedback, rackId)', 'Applied(feedback, 1)'), old_rack_plan),
          'F-1 allowlist guard rejects extra edits, unchanged copy and altered result identity')
    feedback_line = lambda signature: re.findall(r'^        feedback = (.+);$', method(rack_plan, signature), re.M)
    check(feedback_line('public RackPlanOperationResult MarkRackEntrance(') == ['$"Marked the entrance tree {rack.entranceTreeId}."']
          and feedback_line('public RackPlanOperationResult ClearRackEntranceMark(') == ['"Cleared the entrance mark."'],
          'F-1 Mark/Clear feedback has exact neutral copy with no stored rack number')
    check(not git('ls-files', '--others', '--exclude-standard', '--', 'Assets').strip(), 'no staged disposable assets or .meta')
    check('private bool objectiveOneDetailsOpened;' in root and 'if (next == UiScreen.Objectives) objectiveOneDetailsOpened = true;' in root
          and 'objectiveOneDetailsOpened = false;' in method(root,'public void ShowScenarioIntroduction()'), 'session latch set on Objectives entry and reset for a new introduction')
    check('(learning.CurrentTopicIndex > 0 || !objectiveOneDetailsOpened)' in root, 'initial objective included before first Details visit')
    check('candidate.name == "objective-primary-cta" && learning.TeachingRacks && manager.RackPlanningV2Active' in root, 'Next badge suppressed only for O1 map CTA')
    check('else if (!map.RackMode) target = "map-plan-rack";' in root and 'ScenarioOneTeachingSequence.SafePulseTarget(nextAction.name)' in root, 'Plan Racks selection uses existing safe pulse allowlist')
    check('PlayerPrefs.GetInt("CCF.UI.ReduceMotion", 1)' in root and '= true;' in root, 'Reduce Motion default remains ON')
    check('arrived && !rackDestination ? DisplayStyle.None : DisplayStyle.Flex' in hud, 'arrival cannot hide an outstanding rack arrow')
    check('TryGetRackFieldTarget(out RackPlanRack rack, out ForestTree tree, out _)' in source
          and 'target = new Vector2(tree.transform.position.x, tree.transform.position.z);' in source
          and 'RackFieldTargetOrdinal = ui.Marking.RackVisitIndex(rack.Id)' in source, 'minimap projects same getter, physical tree and ordinal as HUD')
    check('new StandMapTransform(model.MinX, model.MinZ, model.SizeM, SidePx)' in mini and 'transform.WorldToPixels(model.Waypoint)' in mini, 'destination emphasis uses shared north-up transform')
    side=float(re.search(r'SidePx = ([\d.]+)f',mini)[1]); ratio=(side/196)**2
    check(2.3 <= ratio <= 2.6, 'nominal area ratio %.6f (native rectangles still required)' % ratio)
    check('contextActions.style.right = HudMinimapView.SidePx + 36f;' in hud, 'bottom controls reserve actual minimap width')
    check('ui.ReduceMotion || Time.unscaledTime - firstTargetTime > 6f' in mini, 'minimap keeps static ring with motion ON by default')
    check('model.Number == 1 && model.Instruction.Contains("[Tab]")' in (UI/'ActiveObjectiveHudWidget.cs').read_text(), 'completion keycaps are scoped to Objective 1')
    check('screen == UiScreen.Review' in root and 'ui.AnnualReviewInstruction' in (UI/'LearningObjectivesView.cs').read_text(), 'review instruction is screen-aware without gate edits')
    styles = (UI/'Resources/ScenarioOneUi.uss').read_text()
    check(planning_border(styles) == 'var(--accent)', 'planning card accent wins actual class specificity and source order')
    check(planning_border(styles, planning=False) == 'var(--map-rule)', 'other map rack panels retain map-rule edge')
    check(planning_border(styles.replace('.modal-map .map-planning-instructions {', '.map-planning-instructions {')) != 'var(--accent)', 'specificity contract rejects original F-2 selector')
    work = (UI/'WorkPlanView.cs').read_text(); old_work = git('show',BASE+':Assets/ForestPrototype/UI/WorkPlanView.cs')
    normal = lambda t: t.replace(', "workplan-title"','').replace(', "workplan-secondary"','')
    check(normal(work)==old_work,'Work Plan C# changes are class mapping only')
    check(git('diff',BASE,'--','Assets/ForestPrototype/UI/ScenarioOneTeachingSequence.cs','Assets/ForestPrototype/UI/LearningObjectivesView.Sequence.cs','Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs')=='','teaching, curriculum and safe pulse inventory unchanged')
    # Compile the actual selected production methods, rather than rewriting the visit/spray algorithm in Python.
    signatures=['public static IEnumerable<RackPlanRack> RackVisitOrder(', 'public int RackVisitIndex(', 'public bool TryGetRackFieldTarget(', 'public string RackSprayPrompt()', 'public RackPlanOperationResult TrySprayRackEntrance()']
    extracted='\n'.join(method(marking,s) for s in signatures)
    (a.out/'ExtractedMarking.cs').write_text('using System; using System.Linq; using System.Collections.Generic;\npublic partial class ForestTreeMarkingManager {\n'+extracted+'\n}')
    # Exercise the actual guidance selection statements with lightweight presentation neighbours.
    animate=method(root,'private void AnimateGuidance()'); selection=animate[animate.index('VisualElement target = null;'):animate.index('if (animatedGuidance != null')]
    (a.out/'ExtractedGuidance.cs').write_text('public partial class GuidanceProbe { public VisualElement Select() { '+selection+'return target; } }')
    review_override = re.search(r'if \(scope == review.Root\b[^;]+reason = ScenarioOneRackBayText.ReviewOnScreenInstruction;', root)[0]
    review_copy = re.search(r'public const string ReviewOnScreenInstruction = [^;]+;', (UI/'ScenarioOneRackBayText.cs').read_text())[0]
    (a.out/'ExtractedReview.cs').write_text('public static class ScenarioOneRackBayText { '+review_copy+' }\npublic partial class GuidanceProbe { public string ReviewReason(VisualElement scope, string reason) { '+review_override+' return reason; } }')
    native = Path(__file__).with_name('Objective1UxNative.cs').read_text()
    (a.out/'ExtractedFeedback.cs').write_text('public static class FeedbackProbe { '+method(native, 'private static bool RackNumbersMatchOrdinal(').replace('private static', 'public static', 1)+' }')
    (a.out/'ExtractedKeycaps.cs').write_text('public class KeycapProbe { '+method((UI/'MenuHelpView.cs').read_text(),'private static void AddKeycapProse(').replace('private static','public static',1)+' }')
    sources=[a.out/'ExtractedKeycaps.cs',a.out/'ExtractedMarking.cs',a.out/'ExtractedGuidance.cs',a.out/'ExtractedReview.cs',a.out/'ExtractedFeedback.cs', Path(__file__).with_name('OfflineProbe.cs'), UI/'MenuHelpView.Text.cs',UI/'ControlsReference.cs',UI/'Map/StandMapTransform.cs']
    (a.out/'probe.csproj').write_text('<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net8.0</TargetFramework><EnableDefaultCompileItems>false</EnableDefaultCompileItems></PropertyGroup><ItemGroup>'+''.join('<Compile Include="'+escape(str(p))+'" />' for p in sources)+'</ItemGroup></Project>')
    env=dict(os.environ,DOTNET_CLI_TELEMETRY_OPTOUT='1',DOTNET_NOLOGO='1')
    p=subprocess.run(['dotnet','run','--project',str(a.out/'probe.csproj'),'-c','Release'],env=env,text=True,stdout=subprocess.PIPE,stderr=subprocess.STDOUT)
    print(p.stdout); check(p.returncode==0,'compiled production method probes')
    print('OBJECTIVE1_UX_OFFLINE_'+('PASS' if all(checks) else 'FAIL')+' contracts='+str(len(checks)))
    return 0 if all(checks) else 1
if __name__=='__main__': raise SystemExit(main())
