#!/usr/bin/env python3
"""Bounded source-structure guard for the Objective 1 map CTA.

This is intentionally not a Unity/UI behaviour test. It checks the real
constructor and guidance methods without treating comments, duplicate anchors,
or an unrelated primary button as evidence of a working Objective CTA.
"""
import re

VIEW = "Assets/ForestPrototype/UI/LearningObjectivesView.cs"
ROOT = "Assets/ForestPrototype/UI/ScenarioOneUiRoot.cs"

# C# comments and literals; preserve literal positions while matching braces.
_LEX = re.compile(
    r'//[^\n]*|/\*[\s\S]*?\*/|@"(?:[^"]|"")*"|"(?:[^"\\]|\\.)*"|'
    r"'(?:[^'\\]|\\.)*'"
)


class ObjectiveCtaSourceError(ValueError):
    """A structural invariant cannot be demonstrated from the given source."""


def _blank(value):
    return "".join("\n" if char == "\n" else " " for char in value)


def _code(source, mask_literals=False):
    def strip(match):
        token = match.group()
        return _blank(token) if mask_literals or token.startswith(("//", "/*")) else token
    return _LEX.sub(strip, source)


def _method(source, signature):
    masked = _code(source, mask_literals=True)
    found = list(re.finditer(signature, masked))
    if len(found) != 1:
        raise ObjectiveCtaSourceError(
            f"expected exactly one method matching {signature!r}; found {len(found)}"
        )
    opening = masked.find("{", found[0].end())
    if opening < 0:
        raise ObjectiveCtaSourceError("method has no opening brace")
    depth = 0
    for at in range(opening, len(masked)):
        if masked[at] == "{":
            depth += 1
        elif masked[at] == "}":
            depth -= 1
            if depth == 0:
                return _code(source[opening + 1:at])
    raise ObjectiveCtaSourceError("unbalanced braces in method")


def _one(source, regex, label):
    matches = list(re.finditer(regex, source, re.S))
    if len(matches) != 1:
        raise ObjectiveCtaSourceError(f"{label}: expected one structural match, found {len(matches)}")
    return matches[0]


def _ordered(source, patterns, label):
    previous = -1
    for name, pattern in patterns:
        found = _one(source, pattern, f"{label}/{name}")
        if found.start() <= previous:
            raise ObjectiveCtaSourceError(f"{label}/{name}: invalid control-flow order")
        previous = found.start()


def verify_objective_cta(view_source, root_source):
    """Raise on missing/ambiguous CTA, otherwise return bounded evidence rows.

    Accept source *text* so mutation tests can operate solely on temporary
    in-memory copies. No production files are modified by this function.
    """
    view = _method(view_source,
                   r"\bpublic\s+LearningObjectivesView\s*\(\s*ScenarioOneUiRoot\s+ui\s*\)")
    root = _code(root_source)
    constructor = _one(
        view,
        r'\bButton\s+([A-Za-z_]\w*)\s*=\s*UiKit\.Button\s*\(\s*'
        r'"Stand map \[M\]"\s*,\s*\(\s*\)\s*=>\s*ui\.ShowMap\s*\(\s*\)\s*\)\s*;',
        "named map button with exact caption and ShowMap click")
    map_var = constructor.group(1)
    names = re.findall(r'\b([A-Za-z_]\w*)\.name\s*=\s*"objective-primary-cta"\s*;', view)
    if names != [map_var]:
        raise ObjectiveCtaSourceError(
            "Objective CTA name must be unique and bound to the ShowMap button"
        )
    _ordered(view, [
        ("root modal", r'\bRoot\.Add\s*\(\s*modal\s*\)\s*;'),
        ("header", r'\bVisualElement\s+header\s*=\s*UiKit\.Box\s*\(\s*"modal-header"\s*\)\s*;'),
        ("button row", r'\bVisualElement\s+buttons\s*=\s*UiKit\.Row\s*\(\s*header\s*\)\s*;'),
        ("map click", r'\bButton\s+' + re.escape(map_var) + r'\s*=\s*UiKit\.Button'),
        ("map identity", r'\b' + re.escape(map_var) + r'\.name\s*=\s*"objective-primary-cta"\s*;'),
        ("row binding", r'\bbuttons\.Add\s*\(\s*' + re.escape(map_var) + r'\s*\)\s*;'),
        ("header attached", r'\bmodal\.Add\s*\(\s*header\s*\)\s*;'),
    ], "Objectives constructor")
    if re.search(r'\bButton\s+' + re.escape(map_var)
                 + r'\s*=\s*UiKit\.Button\s*\([^;]*"btn-primary"', view, re.S):
        raise ObjectiveCtaSourceError("map CTA must not be unconditionally primary")

    allowlist = _one(root, r'\bGuidanceButtonNames\s*=\s*\{(.*?)\}\s*;', "T2-owned allowlist")
    if len(re.findall(r'"objective-primary-cta"', allowlist.group(1))) != 1:
        raise ObjectiveCtaSourceError("map CTA missing or duplicated in scoped allowlist")

    refresh = _method(root_source, r'\bpublic\s+void\s+RefreshNextActionGuidance\s*\(\s*\)')
    _ordered(refresh, [
        ("clear old candidate", r'\bClearNextAction\s*\(\s*\)\s*;'),
        ("restore old primary classes", r'\bforeach\s*\(\s*var\s+entry\s+in\s+guidancePrimaries\s*\)'
                                        r'\s*entry\.Key\.EnableInClassList\s*\(\s*"btn-primary"\s*,\s*entry\.Value\s*\)\s*;'),
        ("clear old primary tracking", r'\bguidancePrimaries\.Clear\s*\(\s*\)\s*;'),
        ("screen scope", r'\bVisualElement\s+scope\s*=\s*manager\.WorkPlanOpen\s*\?\s*workPlan\.Root\s*:'
                         r'\s*screen\s*==\s*UiScreen\.Objectives\s*\?\s*learning\.Root\s*:'
                         r'\s*screen\s*==\s*UiScreen\.Map\s*\?\s*map\.Root'),
        ("no absent screen", r'\bif\s*\(\s*scope\s*==\s*null\s*\)\s*return\s*;'),
        ("modal consent precedence", r'\bif\s*\(\s*consent\s*&&\s*reset\s*\)'),
        ("modal consent routing", r'\belse\s+if\s*\(\s*consent\s*\)\s*target\s*=\s*"map-consent-decline"\s*;'),
        ("rack teaching gate", r'\belse\s+if\s*\(\s*manager\.RackPlanningV2Active\s*&&\s*'
                               r'\(\s*learning\.TeachingRacks\b'),
        ("objectives-scoped branch", r'\bbool\s+objectives\s*=\s*scope\s*==\s*learning\.Root\s*;'),
        ("no rack plan yet", r'\belse\s+if\s*\(\s*!manager\.RackPlanConfirmed\s*\)\s*\{'
                             r'\s*if\s*\(\s*objectives\s*\)\s*target\s*=\s*"objective-primary-cta"\s*;'),
        ("no unrelated guide demotion", r'\belse\s+return\s*;'),
        ("bounded class demotion", r'\bforeach\s*\(\s*string\s+name\s+in\s+GuidanceButtonNames\s*\)'
                                   r'\s*\{\s*Button\s+owned\s*=\s*scope\.Q<Button>\s*\(\s*name\s*\)\s*;'
                                   r'\s*if\s*\(\s*owned\s*==\s*null\s*\)\s*continue\s*;'
                                   r'\s*guidancePrimaries\s*\[\s*owned\s*\]\s*=\s*'
                                   r'owned\.ClassListContains\s*\(\s*"btn-primary"\s*\)\s*;'
                                   r'\s*owned\.RemoveFromClassList\s*\(\s*"btn-primary"\s*\)\s*;\s*\}'),
        ("candidate resolved in its scope", r'\bguidanceCandidate\s*=\s*target\s*!=\s*null\s*\?'
                                            r'\s*scope\.Q<Button>\s*\(\s*target\s*\)\s*:\s*null\s*;'),
        ("apply after binding", r'\bApplyNextActionGuidance\s*\(\s*\)\s*;'),
    ], "RefreshNextActionGuidance")
    if len(re.findall(r'\btarget\s*=\s*"objective-primary-cta"\s*;', refresh)) != 1:
        raise ObjectiveCtaSourceError("map target must be selected exactly once")
    if len(re.findall(r'\bowned\.RemoveFromClassList\s*\(\s*"btn-primary"\s*\)', refresh)) != 1:
        raise ObjectiveCtaSourceError("unexpected global primary demotion")

    clear = _method(root_source, r'\bprivate\s+void\s+ClearNextAction\s*\(\s*\)')
    _ordered(clear, [
        ("only existing nextAction", r'\bif\s*\(\s*nextAction\s*!=\s*null\s*\)'),
        ("remove primary", r'\bnextAction\.RemoveFromClassList\s*\(\s*"btn-primary"\s*\)\s*;'),
        ("remove guide class", r'\bnextAction\.RemoveFromClassList\s*\(\s*"next-action"\s*\)\s*;'),
        ("clear reference", r'\bnextAction\s*=\s*null\s*;'),
    ], "ClearNextAction")

    apply = _method(root_source, r'\bprivate\s+void\s+ApplyNextActionGuidance\s*\(\s*\)')
    _ordered(apply, [
        ("preview/menus/help suspension",
         r'\bbool\s+suspended\s*=\s*manager\s*==\s*null\s*\|\|\s*manager\.ReferencePreviewActive'
         r'\s*\|\|\s*StandaloneSessionMenu\.IsOpen\s*\|\|\s*help\s*==\s*null\s*\|\|'
         r'\s*help\.IsOpen\s*\|\|\s*helpClosedFrame\s*==\s*Time\.frameCount\s*;'),
        ("candidate scoped visibility",
         r'\bbool\s+eligible\s*=\s*!suspended\s*&&\s*candidate\s*!=\s*null\s*&&'
         r'\s*candidate\.enabledInHierarchy\s*&&\s*GuidanceVisible\s*\(\s*candidate\s*,\s*scope\s*\)\s*;'),
        ("clear stale candidate", r'\bClearNextAction\s*\(\s*\)\s*;'),
        ("no styling if ineligible", r'\bif\s*\(\s*!eligible\s*\)\s*return\s*;'),
        ("set current candidate", r'\bnextAction\s*=\s*candidate\s*;'),
        ("apply primary", r'\bcandidate\.AddToClassList\s*\(\s*"btn-primary"\s*\)\s*;'),
        ("apply next-action", r'\bcandidate\.AddToClassList\s*\(\s*"next-action"\s*\)\s*;'),
    ], "ApplyNextActionGuidance")
    visible = _method(root_source, r'\bprivate\s+static\s+bool\s+GuidanceVisible\s*\(\s*'
                                   r'VisualElement\s+element\s*,\s*VisualElement\s+scope\s*\)')
    _one(visible, r'\bfor\s*\(\s*VisualElement\s+current\s*=\s*element\s*;'
                  r'\s*current\s*!=\s*null\s*;\s*current\s*=\s*current\.parent\s*\)',
         "candidate-to-scope ancestry walk")
    _one(visible, r'\bif\s*\(\s*current\.style\.display\s*==\s*DisplayStyle\.None\s*\|\|'
                  r'\s*current\.resolvedStyle\.display\s*==\s*DisplayStyle\.None\s*\|\|'
                  r'\s*current\.resolvedStyle\.visibility\s*==\s*Visibility\.Hidden\s*\)\s*return\s+false\s*;',
         "visibility and display suspension")
    _one(visible, r'\bif\s*\(\s*current\s*==\s*scope\s*\)\s*return\s+true\s*;',
         "candidate must belong to the active scope")
    _one(visible, r'\breturn\s+false\s*;\s*$', "out-of-scope candidate rejected")

    def line_at(source, fragment):
        index = source.find(fragment)
        return source.count("\n", 0, index) + 1 if index >= 0 else 0

    return [
        (VIEW, line_at(view_source, 'Button ' + map_var + ' = UiKit.Button'),
         "Named Stand map [M] button -> ui.ShowMap(), added to Objectives header row"),
        (ROOT, line_at(root_source, 'if (objectives) target = "objective-primary-cta";'),
         "Conditional Rack V2 / unconfirmed Objective 1 target"),
        (ROOT, line_at(root_source, 'foreach (string name in GuidanceButtonNames)'),
         "Scoped class demotion with original primary restoration"),
        (ROOT, line_at(root_source, 'bool suspended = manager == null'),
         "Reference/menus/Help and help-close suspension"),
        (ROOT, line_at(root_source, 'bool eligible = !suspended'),
         "Enabled, visible, descendant-of-scope eligibility"),
        (ROOT, line_at(root_source, 'candidate.AddToClassList("btn-primary");'),
         "Dynamic primary and next-action class application"),
    ]
