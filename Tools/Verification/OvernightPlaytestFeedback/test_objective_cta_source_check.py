#!/usr/bin/env python3
"""Negative controls for the Objective CTA source scanner; no Unity or .NET.

Runs against source in the current checkout, then mutates in-memory fixture
copies only. A claimed success requires all deliberately broken cases to be
rejected (the #309 cases also for their intended cause), every benign edit
accepted and the historical baseline rejected. The original numbered historical
checks live in check_baseline.py.

Groups: the original 20, the #304 author's F1 (14) and F2/F3 (21) controls and
4 benign edits, unchanged; the #305 independent reviewer's corpus (21 re-derived
R-IDs, 35 X-F1, 46 X-F2 and 15 benign), copied verbatim below; and the #309
RR-1/RR-2/RR-3/N-4 regression and benign controls.
"""
import subprocess
from pathlib import Path
from objective_cta_source_check import (
    ROOT, UIKIT, VIEW, ObjectiveCtaSourceError, collect_companions, verify_objective_cta
)

REPO = Path(__file__).resolve().parents[3]
BASE = "72b4efbd1c653fa2231f632a79030a83ff76f833"  # historical pre-guidance source; must stay rejected
# In-memory fixture keys for the files a mutation may change. "extra" is a new, test-only partial.
FILES = {"view": VIEW, "root": ROOT, "seq": "Assets/ForestPrototype/UI/LearningObjectivesView.Sequence.cs",
         "cur": "Assets/ForestPrototype/UI/LearningObjectivesView.Curriculum.cs", "kit": UIKIT,
         "walk": "Assets/ForestPrototype/UI/WalkingHudView.cs", "standmap": "Assets/ForestPrototype/UI/StandMapView.cs",
         "extra": "Assets/ForestPrototype/UI/LearningObjectivesView.Extra.cs"}
V, R, SEQ, KIT = "view", "root", "seq", "kit"   # names used by the #305 reviewer's corpus below


def replace_one(source, old, new, label):
    count = source.count(old)
    if count != 1:
        raise RuntimeError(f"{label}: expected exactly one live source fixture anchor, got {count}: {old!r}")
    return source.replace(old, new, 1)


# =====================================================================================================
# #305 independent reviewer's corpus, lines 33-295 of its review_mutations.py, copied verbatim
# (sha256 9a7a8fd22026cfc8abcc0c6d30b176a1f5244c709a6cde7d1f9a0f076aacb231). `intended` is the cause
# the rejection reason must contain. B-X-14/15 (CRLF, BOM) are whole-file transforms added in run().
# =====================================================================================================
# ---- anchors ------------------------------------------------------------------------------------
MAP_NAME = 'map.name = "objective-primary-cta";'
ADD_MAP = 'buttons.Add(map);'
WORK_NAME = 'work.name = "objective-work-plan";'
FOREST_NAME = 'forest.name = "objective-back-to-forest";'
ADD_FOREST = 'buttons.Add(forest);'
FOREST_BTN = 'Button forest = UiKit.Button("Back to forest [O / Esc]", () => ui.CloseAll(), true, "btn-primary");'
MAP_BTN = 'Button map = UiKit.Button("Stand map [M]", () => ui.ShowMap());'
CLASS_MEMBER = 'private readonly ScrollView scroll;'
VIEW_REFRESH_CALL = 'ui.RefreshNextActionGuidance();'
ALLOW = '"objective-primary-cta", "objective-work-plan", "objective-back-to-forest",'
FIRST_GUARD = 'if (learning == null || map == null || workPlan == null || manager == null) return;'
OBJECTIVES = 'bool objectives = scope == learning.Root;'
ANNUAL_IF = 'if (NeedsAnnualReview)\n                target = scope == review.Root'
CONSENT = 'bool consent = mapQuestions && map.RackEditor.ConsentPending;'
MAP_QUESTIONS = 'bool mapQuestions = scope == map.Root && map.RackMode;'
RESET_ROUTE = 'else if (reset) target = "map-edit-reset-cancel";'
TEACHING_GATE = 'else if (manager.RackPlanningV2Active && (learning.TeachingRacks'
CHAIN_END = '                }\n            }\n        }\n        else return;'
GUIDANCE_SCOPE_SET = 'guidanceScope = scope;'
BIND = 'guidanceCandidate = target != null ? scope.Q<Button>(target) : null;'
LATE = 'ApplyNextActionGuidance();\n        if (competitors == null || manager == null) return;'
LATE_END = 'indicator?.LateUpdate();'
APPLY_SIG = 'private void ApplyNextActionGuidance()'
ADD_PRIMARY = 'candidate.AddToClassList("btn-primary");'
ADD_NEXT = 'candidate.AddToClassList("next-action");'
CLEAR_THEN_GUARD = 'ClearNextAction();\n        if (!eligible) return;'
ELIGIBLE_RETURN = 'if (!eligible) return;'
CANDIDATE_COPY = 'Button candidate = guidanceCandidate;'
HELP_SIG = 'public void CloseHelp()\n    {\n        if (help == null || !help.IsOpen) return;'
HELP_SET = 'helpClosedFrame = Time.frameCount;'
VISIBLE_LOOP = 'for (VisualElement current = element; current != null; current = current.parent)\n        {'
SHOW_MAP = 'public void ShowMap()\n    {\n        manager.OpenWorkPlan(false);\n        SetScreen(UiScreen.Map);'
SEQ_MEMBER = 'private int teachingWorldVersion = -1;'
KIT_BTN = 'button.AddToClassList("btn");'

CLOSE_UNNAMED = 'Button close = UiKit.Button("Close", () => ui.CloseAll());'
CLOSE_PRIMARY = 'Button close = UiKit.Button("Close", () => ui.CloseAll(), true, "btn-primary");'


def m(mid, group, why, *edits, intended=None):
    """edits: (path, old, new). `intended` is a substring the rejection reason must contain for the cause to count."""
    return dict(id=mid, group=group, why=why, edits=list(edits), intended=intended)


# ---- R-IDs re-derived from the #302 report table (not imported from the author's fixture list) ---
reproductions = [
    m("R01", "R-F1", "map statically primary", (V, MAP_NAME, MAP_NAME + '\n        map.AddToClassList("btn-primary");'),
      intended="map CTA must not be unconditionally primary"),
    m("R02", "R-prior", "inline primary on the map constructor",
      (V, MAP_BTN, 'Button map = UiKit.Button("Stand map [M]", () => ui.ShowMap(), true, "btn-primary");'),
      intended="map CTA must not be unconditionally primary"),
    m("R03", "R-F1", "Back to forest dropped from the allowlist",
      (R, ALLOW, '"objective-primary-cta", "objective-work-plan",'),
      intended="'objective-back-to-forest' is not in the GuidanceButtonNames"),
    m("R04", "R-F1", "Back to forest renamed out of the allowlist",
      (V, FOREST_NAME, 'forest.name = "objective-forest";'),
      intended="'objective-forest' is not in the GuidanceButtonNames"),
    m("R05", "R-F1", "new CloseAll primary",
      (V, ADD_FOREST, ADD_FOREST + '\n        ' + CLOSE_PRIMARY + ' buttons.Add(close);'),
      intended="static primary 'close' must have exactly one literal name"),
    m("R06", "R-prior", "duplicate CTA name, no spaces", (V, WORK_NAME, 'work.name="objective-primary-cta";'),
      intended="duplicate element name"),
    m("R07", "R-F3", "verbatim duplicate CTA name", (V, WORK_NAME, 'work.name = @"objective-primary-cta";'),
      intended="duplicate element name"),
    m("R08", "R-F2", "later map rename", (V, ADD_MAP, ADD_MAP + '\n        map.name = "objective-map";'),
      intended="Objective CTA name must be unique"),
    m("R09", "R-F2", "guard moved into a string",
      (R, ELIGIBLE_RETURN, 'UnityEngine.Debug.Log("if (!eligible) return;");'),
      intended="no styling if ineligible"),
    m("R10", "R-prior", "guard commented out", (R, ELIGIBLE_RETURN, '// if (!eligible) return;'),
      intended="no styling if ineligible"),
    m("R11", "R-F2", "target override to CloseAll before binding",
      (R, BIND, 'if (scope == learning.Root) target = "objective-back-to-forest";\n        ' + BIND),
      intended="target is reassigned after the routing chain"),
    m("R12", "R-F2", "candidate nulled in Apply", (R, CANDIDATE_COPY, CANDIDATE_COPY + '\n        candidate = null;'),
      intended="writes 'candidate' more than once"),
    m("R13", "R-F2", "return after tracking clear",
      (R, 'guidancePrimaries.Clear();', 'guidancePrimaries.Clear();\n        return;'),
      intended="unexpected early return"),
    m("R14", "R-F2", "primary class behind #if UNITY_EDITOR",
      (R, ADD_PRIMARY, '#if UNITY_EDITOR\n        ' + ADD_PRIMARY + '\n#endif'),
      intended="conditional compilation"),
    m("R15", "R-F2", "map disabled after binding", (V, ADD_MAP, ADD_MAP + '\n        map.SetEnabled(false);'),
      intended="map CTA variable may only be declared"),
    m("R16", "R-prior", "final return made true", (R, 'return false;\n    }\n\n    // Inline questions', 'return true;\n    }\n\n    // Inline questions'),
      intended="out-of-scope candidate rejected"),
    m("R17", "R-prior", "guard/assign swapped",
      (R, 'if (!eligible) return;\n        nextAction = candidate;', 'nextAction = candidate;\n        if (!eligible) return;'),
      intended="invalid control-flow order"),
    m("R18", "R-prior", "V2 gate negated",
      (R, TEACHING_GATE, 'else if (!manager.RackPlanningV2Active && (learning.TeachingRacks'),
      intended="rack teaching gate"),
    m("R19", "R-prior", "objectives flag widened", (R, OBJECTIVES, 'bool objectives = scope == learning.Root || true;'),
      intended="objectives-scoped branch"),
    m("R20", "R-F2", "help-close setter removed", (R, HELP_SET, ''), intended="help-close frame recorded"),
    m("R21", "R-F2", "LateUpdate apply removed",
      (R, LATE, 'if (competitors == null || manager == null) return;'),
      intended="LateUpdate applies cached guidance"),
]

# ---- reviewer's own: constructor/header shapes (F1) ----------------------------------------------
f1 = [
    m("X-F1-01", "F1", "R05 via a helper that builds the primary (literal outside the constructor)",
      (V, CLASS_MEMBER, CLASS_MEMBER + '\n    private static Button MakePrimary(string text, System.Action click) => UiKit.Button(text, click, true, "btn-primary");'),
      (V, ADD_FOREST, ADD_FOREST + '\n        Button close = MakePrimary("Close", () => ui.CloseAll()); buttons.Add(close);')),
    m("X-F1-02", "F1", "R05 via a helper that promotes an existing button",
      (V, CLASS_MEMBER, CLASS_MEMBER + '\n    private static void Promote(Button b) { b.AddToClassList("btn-primary"); }'),
      (V, ADD_FOREST, ADD_FOREST + '\n        ' + CLOSE_UNNAMED + ' Promote(close); buttons.Add(close);')),
    m("X-F1-03", "F1", "map made primary from a method outside the constructor, name split to dodge the literal count",
      (V, VIEW_REFRESH_CALL, VIEW_REFRESH_CALL + '\n        Root.Q<Button>("objective-" + "primary-cta").AddToClassList("btn-primary");')),
    m("X-F1-04", "F1", "unnamed extra primary created outside the constructor, appended to the header row",
      (V, VIEW_REFRESH_CALL, VIEW_REFRESH_CALL + '\n        Root.Query<Button>().First().parent.Add(UiKit.Button("Close", () => ui.CloseAll(), true, "btn-primary"));')),
    m("X-F1-05", "F1", "partial class in another file promotes the map (scanner never reads that file)",
      (SEQ, SEQ_MEMBER, SEQ_MEMBER + '\n    public void Sneak() { UnityEngine.UIElements.UQueryExtensions.Q<UnityEngine.UIElements.Button>(Root, "objective-primary-cta").AddToClassList("btn-primary"); }')),
    m("X-F1-06", "F1", "UiKit.Button itself makes every button primary (scanner never reads UiKit)",
      (KIT, KIT_BTN, KIT_BTN + '\n        button.AddToClassList("btn-primary");')),
    m("X-F1-07", "F1", "interpolated class name on the map", (V, MAP_NAME, MAP_NAME + '\n        map.AddToClassList($"btn-primary");')),
    m("X-F1-08", "F1", "interpolated class name on an inline primary",
      (V, ADD_FOREST, ADD_FOREST + '\n        Button close = UiKit.Button("Close", () => ui.CloseAll(), true, $"btn-primary"); buttons.Add(close);')),
    m("X-F1-09", "F1", "C# 11 raw string class name (not valid at Unity's C# level)",
      (V, MAP_NAME, MAP_NAME + '\n        map.AddToClassList("""btn-primary""");')),
    m("X-F1-10", "F1", "map primary inside an if", (V, MAP_NAME, MAP_NAME + '\n        if (ui != null) map.AddToClassList("btn-primary");')),
    m("X-F1-11", "F1", "map primary deferred in a lambda",
      (V, MAP_NAME, MAP_NAME + '\n        modal.RegisterCallback<AttachToPanelEvent>(_ => map.AddToClassList("btn-primary"));')),
    m("X-F1-12", "F1", "map primary by ToggleInClassList", (V, MAP_NAME, MAP_NAME + '\n        map.ToggleInClassList("btn-primary");')),
    m("X-F1-13", "F1", "verbatim identifier @map", (V, MAP_NAME, MAP_NAME + '\n        @map.AddToClassList("btn-primary");')),
    m("X-F1-14", "F1", "unicode-escaped identifier", (V, MAP_NAME, MAP_NAME + '\n        m\\u0061p.AddToClassList("btn-primary");')),
    m("X-F1-15", "F1", "escaped class literal \\x", (V, MAP_NAME, MAP_NAME + '\n        map.AddToClassList("b\\x74n-primary");')),
    m("X-F1-16", "F1", "new Button primary, no name",
      (V, ADD_FOREST, ADD_FOREST + '\n        Button close = new Button(() => ui.CloseAll()) { text = "Close" }; close.AddToClassList("btn-primary"); buttons.Add(close);')),
    m("X-F1-17", "F1", "Q-chained primary", (V, ADD_FOREST, ADD_FOREST + '\n        buttons.Q<Button>().AddToClassList("btn-primary");')),
    m("X-F1-18", "F1", "row indexer primary", (V, ADD_FOREST, ADD_FOREST + '\n        buttons[0].AddToClassList("btn-primary");')),
    m("X-F1-19", "F1", "two-step declaration of a primary",
      (V, ADD_FOREST, ADD_FOREST + '\n        Button close; close = UiKit.Button("Close", () => ui.CloseAll(), true, "btn-primary"); close.name = "objective-work-plan2";')),
    m("X-F1-20", "F1", "class assigned to the forest variable after declaration (reassignment)",
      (V, ADD_FOREST, ADD_FOREST + '\n        ' + CLOSE_UNNAMED + ' forest = close;')),
    m("X-F1-21", "F1", "duplicate allowlist spelling (ordinary + verbatim)",
      (R, ALLOW, '"objective-primary-cta", "objective-work-plan", "objective-back-to-forest", @"objective-back-to-forest",'),
      intended="duplicate"),
    m("X-F1-22", "F1", "allowlist entry commented out (R03 with the text still present)",
      (R, ALLOW, '"objective-primary-cta", "objective-work-plan", /* "objective-back-to-forest", */'),
      intended="'objective-back-to-forest' is not in the GuidanceButtonNames"),
    m("X-F1-23", "F1", "allowlist entry behind #if (R03 in a Player build)",
      (R, ALLOW, '"objective-primary-cta", "objective-work-plan",\n#if UNITY_EDITOR\n        "objective-back-to-forest",\n#endif'),
      intended="conditional compilation"),
    m("X-F1-24", "F1", "R04 by compound assignment: Back to forest's runtime name leaves the allowlist",
      (V, FOREST_NAME, FOREST_NAME + '\n        forest.name += "-x";')),
    m("X-F1-25", "F1", "R04 by cast: second name write the receiver parse cannot attribute",
      (V, FOREST_NAME, FOREST_NAME + '\n        ((VisualElement)forest).name = "objective-renamed";')),
    m("X-F1-26", "F1", "R04 by alias: second name write through another local",
      (V, FOREST_NAME, FOREST_NAME + '\n        Button alias = forest; alias.name = "objective-renamed";')),
    m("X-F1-27", "F1", "allowlist element overwritten at run time through a qualified reference (R03 at run time)",
      (R, HELP_SET, HELP_SET + '\n        ScenarioOneUiRoot.GuidanceButtonNames[2] = "objective-none";')),
    m("X-F1-28", "F1", "method-group + class-level const: no call shape and no literal in the constructor",
      (V, CLASS_MEMBER, CLASS_MEMBER + '\n    private const string PrimaryClass = "btn-primary";'),
      (V, ADD_FOREST, ADD_FOREST + '\n        ' + CLOSE_UNNAMED + ' System.Action<string> add = close.AddToClassList; add(PrimaryClass); buttons.Add(close);')),
    m("X-F1-29", "F1", "lexer desync: char literal inside an interpolation hole hides a map primary",
      (V, MAP_NAME, MAP_NAME + '\n        _ = $"{\'"\'}"; map.AddToClassList("btn-primary"); /*\'*/')),
    m("X-F1-30", "F1", "lexer desync: @$ verbatim-interpolated backslash hides a map primary",
      (V, MAP_NAME, MAP_NAME + '\n        _ = @$"\\"; map.AddToClassList("btn-primary"); _ = "\\"";')),
    m("X-F1-31", "F1", "second static primary on an allowlisted button (demoted in guided state, doubled outside it)",
      (V, WORK_NAME, WORK_NAME + '\n        work.AddToClassList("btn-primary");')),
    m("X-F1-32", "F1", "new CloseAll primary reusing another view's allowlisted name (demoted in guided state, doubled outside it)",
      (V, ADD_FOREST, ADD_FOREST + '\n        ' + CLOSE_PRIMARY + ' close.name = "map-back-to-forest"; buttons.Add(close);')),
    m("X-F1-33", "F1", "names swapped between Work Plan and Back to forest (both allowlisted): later-state targets route to the wrong button",
      (V, WORK_NAME, 'work.name = "objective-back-to-forest";'),
      (V, FOREST_NAME, 'forest.name = "objective-work-plan";')),
    m("X-F1-34", "F1", "second non-primary button with the map caption that closes the screen",
      (V, MAP_BTN, 'Button spoof = UiKit.Button("Stand map [M]", () => ui.CloseAll()); buttons.Add(spoof);\n        ' + MAP_BTN)),
    m("X-F1-35", "F1", "spoof captioned map + real map primary (tolerant lookup picks the spoof)",
      (V, MAP_BTN, 'Button spoof = UiKit.Button("Stand map [M]", () => ui.CloseAll());\n        ' + MAP_BTN),
      (V, MAP_NAME, MAP_NAME + '\n        map.AddToClassList("btn-primary");')),
]

# ---- reviewer's own: map presence, overrides, ordering, guards (F2/F3) ----------------------------
f2 = [
    m("X-F2-01", "F2", "R08 by row indexer: map renamed through buttons[0]", (V, ADD_MAP, ADD_MAP + '\n        buttons[0].name = "objective-map";')),
    m("X-F2-02", "F2", "R15 by row indexer: map disabled through buttons[0]", (V, ADD_MAP, ADD_MAP + '\n        buttons[0].SetEnabled(false);')),
    m("X-F2-03", "F2", "R15 by parent: whole button row disabled", (V, ADD_FOREST, ADD_FOREST + '\n        buttons.SetEnabled(false);')),
    m("X-F2-04", "F2", "row cleared after binding: Q<Button> finds no map", (V, ADD_FOREST, ADD_FOREST + '\n        buttons.Clear();')),
    m("X-F2-05", "F2", "row binding wrapped in if (false)", (V, ADD_MAP, 'if (false) ' + ADD_MAP)),
    m("X-F2-06", "F2", "map identity wrapped in if (false)", (V, MAP_NAME, 'if (false) ' + MAP_NAME)),
    m("X-F2-07", "F2", "header row hidden", (V, 'modal.Add(header);', 'modal.Add(header);\n        header.style.display = DisplayStyle.None;')),
    m("X-F2-08", "F2", "R13 count-preserving: first guard replaced by a bare return, whole method dead",
      (R, FIRST_GUARD, 'return;')),
    m("X-F2-09", "F2", "R11 by tuple assignment", (R, BIND, '(target, reason) = ("objective-back-to-forest", reason);\n        ' + BIND)),
    m("X-F2-10", "F2", "R11 count-compensated: retarget inside the chain, one other target write removed",
      (R, RESET_ROUTE, 'else if (reset) reason = "";'),
      (R, CHAIN_END, '                }\n            }\n            if (objectives && !NeedsAnnualReview && !manager.RackPlanConfirmed) target = "objective-back-to-forest";\n        }\n        else return;')),
    m("X-F2-11", "F2", "objectives flag overwritten after its pinned declaration", (R, OBJECTIVES, OBJECTIVES + '\n            objectives = false;')),
    m("X-F2-12", "F2", "annual-review branch forced, map target unreachable", (R, ANNUAL_IF, 'if (true)\n                target = scope == review.Root')),
    m("X-F2-13", "F2", "consent forced true, chain never reaches the map target", (R, CONSENT, 'bool consent = true;')),
    m("X-F2-14", "F2", "teaching gate widened after the pinned prefix: map CTA offered beyond Objective 1",
      (R, TEACHING_GATE, TEACHING_GATE + ' || true')),
    m("X-F2-15", "F2", "routing literal swapped, count unchanged", (R, 'else if (!map.RackMode) target = "map-plan-rack";', 'else if (!map.RackMode) target = "map-back-to-forest";')),
    m("X-F2-16", "F2", "Back to forest re-promoted right after the demotion loop (original two-primary defect)",
      (R, GUIDANCE_SCOPE_SET, 'learning.Root.Q<Button>("objective-back-to-forest")?.AddToClassList("btn-primary");\n        ' + GUIDANCE_SCOPE_SET)),
    m("X-F2-17", "F2", "demotion undone by a second loop with a renamed variable",
      (R, GUIDANCE_SCOPE_SET, 'foreach (var kv in guidancePrimaries) kv.Key.EnableInClassList("btn-primary", kv.Value);\n        ' + GUIDANCE_SCOPE_SET)),
    m("X-F2-18", "F2", "CTA primary class removed every frame after the LateUpdate apply",
      (R, LATE, 'ApplyNextActionGuidance();\n        nextAction?.RemoveFromClassList("btn-primary");\n        if (competitors == null || manager == null) return;')),
    m("X-F2-19", "F2", "CTA primary class removed in Apply through nextAction", (R, ADD_NEXT, ADD_NEXT + '\n        nextAction.RemoveFromClassList("btn-primary");')),
    m("X-F2-20", "F2", "R14 by plain if: primary class never applied", (R, ADD_PRIMARY, 'if (false) ' + ADD_PRIMARY)),
    m("X-F2-21", "F2", "R14 by [Conditional]: Apply compiled out of Player builds, no #if anywhere",
      (R, APPLY_SIG, '[System.Diagnostics.Conditional("UNITY_EDITOR")]\n    ' + APPLY_SIG)),
    m("X-F2-22", "F2", "R21 by lambda: LateUpdate call never invoked",
      (R, LATE, 'System.Action skipped = () => ApplyNextActionGuidance();\n        if (competitors == null || manager == null) return;')),
    m("X-F2-23", "F2", "R21 by shadow: local function replaces the method inside LateUpdate",
      (R, LATE_END, LATE_END + '\n        void ApplyNextActionGuidance() { }')),
    m("X-F2-24", "F2", "R13 by goto: eligible candidate skips styling", (R, CLEAR_THEN_GUARD, 'if (eligible) goto done;\n        ' + CLEAR_THEN_GUARD), (R, ADD_NEXT, ADD_NEXT + '\n        done: ;')),
    m("X-F2-25", "F2", "R13 by throw: eligible candidate never styled", (R, CLEAR_THEN_GUARD, 'if (eligible) throw new System.InvalidOperationException();\n        ' + CLEAR_THEN_GUARD)),
    m("X-F2-26", "F2", "R09 by dead lambda: guard present but never executed", (R, ELIGIBLE_RETURN, 'System.Action dead = () => { if (!eligible) return; };')),
    m("X-F2-27", "F2", "R23 with this.: candidate nulled by an unrelated handler", (R, HELP_SET, HELP_SET + '\n        this.guidanceCandidate = null;')),
    m("X-F2-28", "F2", "R20c with this.: help-close frame reset", (R, HELP_SET, HELP_SET + '\n        this.helpClosedFrame = -1;')),
    m("X-F2-29", "F2", "R20 by inverted early guard: setter unreachable while Help is open", (R, HELP_SIG, 'public void CloseHelp()\n    {\n        if (help == null || help.IsOpen) return;')),
    m("X-F2-30", "F2", "R20 by lambda: setter never invoked", (R, HELP_SET, 'System.Action recordFrame = () => helpClosedFrame = Time.frameCount;')),
    m("X-F2-31", "F2", "GuidanceVisible parameter overwritten: every candidate reads as visible", (R, VISIBLE_LOOP, 'element = scope;\n        ' + VISIBLE_LOOP)),
    m("X-F2-32", "F2", "GuidanceVisible skips the candidate's own visibility", (R, VISIBLE_LOOP, VISIBLE_LOOP + '\n            if (current == element) continue;')),
    m("X-F2-33", "F2", "only Objectives caller of Refresh removed: guidance never derived", (V, VIEW_REFRESH_CALL, '')),
    m("X-F2-34", "F2", "only Objectives caller of Refresh behind #if (R14 at the call site)", (V, VIEW_REFRESH_CALL, '#if UNITY_EDITOR\n        ' + VIEW_REFRESH_CALL + '\n#endif')),
    m("X-F2-35", "F2", "ShowMap gutted: CTA still wired by name, opens nothing", (R, SHOW_MAP, 'public void ShowMap()\n    {\n        manager.OpenWorkPlan(false);\n        SetScreen(UiScreen.None);')),
    # Expected-reject probes: the guards the repair did add, attacked from a new angle.
    m("X-F2-36", "F2", "ordering: candidate bound after Apply",
      (R, BIND, 'ApplyNextActionGuidance();\n        ' + BIND), intended="expected one structural match"),
    m("X-F2-37", "F2", "guard in block comment", (R, ELIGIBLE_RETURN, '/* if (!eligible) return; */'), intended="no styling if ineligible"),
    m("X-F2-38", "F2", "guard in verbatim string", (R, ELIGIBLE_RETURN, '_ = @"if (!eligible) return;";'), intended="no styling if ineligible"),
    m("X-F2-39", "F2", "guard in interpolated string", (R, ELIGIBLE_RETURN, '_ = $"if (!eligible) return;";'), intended="no styling if ineligible"),
    m("X-F2-40", "F2", "duplicate CTA name with \\x escape", (V, WORK_NAME, 'work.name = "objective\\x2dprimary-cta";'), intended="duplicate element name"),
    m("X-F2-41", "F2", "duplicate CTA name in an object initializer",
      (V, ADD_FOREST, ADD_FOREST + '\n        Button dup = new Button(() => ui.CloseAll()) { name = "objective-primary-cta" };'), intended="duplicate element name"),
    m("X-F2-42", "F2", "duplicate CTA name via class-level const",
      (V, CLASS_MEMBER, CLASS_MEMBER + '\n    private const string Cta = "objective-" + "primary-cta";'), (V, WORK_NAME, 'work.name = Cta;'),
      intended="not a single plain literal"),
    m("X-F2-43", "F2", "allowlist shadowed by a local in Refresh",
      (R, FIRST_GUARD, FIRST_GUARD + '\n        string[] GuidanceButtonNames = { "objective-primary-cta" };'), intended="GuidanceButtonNames"),
    m("X-F2-44", "F2", "Apply wrapped in #if UNITY_EDITOR", (R, APPLY_SIG, '#if UNITY_EDITOR\n    ' + APPLY_SIG), (R, '    private static bool GuidanceVisible', '#endif\n    private static bool GuidanceVisible'),
      intended="conditional compilation"),
    m("X-F2-45", "F2", "candidate nulled by ref helper in Apply", (R, CANDIDATE_COPY, CANDIDATE_COPY + '\n        ClearRef(ref candidate);'),
      (R, APPLY_SIG, 'private static void ClearRef(ref Button b) { b = null; }\n    ' + APPLY_SIG), intended="writes 'candidate' more than once"),
    m("X-F2-46", "F2", "second Apply overload shadow attempt: call site no longer zero-argument",
      (R, LATE, 'ApplyNextActionGuidance(0);\n        if (competitors == null || manager == null) return;'),
      (R, APPLY_SIG, 'private void ApplyNextActionGuidance(int unused) { }\n    ' + APPLY_SIG), intended="LateUpdate applies cached guidance"),
]

# ---- benign controls (must be accepted) ----------------------------------------------------------
benign = [
    m("B-X-01", "benign", "map local renamed consistently", (V, MAP_BTN, MAP_BTN.replace('Button map', 'Button standMap')), (V, MAP_NAME, MAP_NAME.replace('map.name', 'standMap.name')), (V, ADD_MAP, 'buttons.Add(standMap);')),
    m("B-X-02", "benign", "forest local renamed consistently", (V, FOREST_BTN, FOREST_BTN.replace('Button forest', 'Button backToForest')), (V, FOREST_NAME, FOREST_NAME.replace('forest.name', 'backToForest.name')), (V, ADD_FOREST, 'buttons.Add(backToForest);')),
    m("B-X-03", "benign", "forest declared with var", (V, FOREST_BTN, FOREST_BTN.replace('Button forest', 'var forest'))),
    m("B-X-04", "benign", "block comment inside the identity statement", (V, MAP_NAME, 'map.name = /* cta */ "objective-primary-cta";')),
    m("B-X-05", "benign", "guard split over two lines", (R, ELIGIBLE_RETURN, 'if (!eligible)\n            return;')),
    m("B-X-06", "benign", "work and forest order swapped in the row", (V, 'buttons.Add(work);', ''), (V, ADD_FOREST, ADD_FOREST + '\n        buttons.Add(work);')),
    m("B-X-07", "benign", "new named non-primary header button", (V, ADD_FOREST, ADD_FOREST + '\n        Button info = UiKit.Button("Info", () => ui.CloseAll()); info.name = "objective-info"; buttons.Add(info);')),
    m("B-X-08", "benign", "extra harmless class on the static primary", (V, FOREST_BTN, FOREST_BTN.replace('"btn-primary"', '"btn-primary", "btn-wide"'))),
    m("B-X-09", "benign", "#region in the constructor", (V, MAP_BTN, '#region buttons\n        ' + MAP_BTN), (V, ADD_FOREST, ADD_FOREST + '\n        #endregion')),
    m("B-X-10", "benign", "comment with apostrophe and quote in the constructor", (V, ADD_MAP, ADD_MAP + ' // don\'t "touch" this')),
    m("B-X-11", "benign", "#if in an unrelated root method", (R, 'public void ShowObjectives()\n    {', 'public void ShowObjectives()\n    {\n#if UNITY_EDITOR\n        Debug.Log("objectives");\n#endif')),
    m("B-X-12", "benign", "string with // in the constructor", (V, 'modal.Add(header);', 'modal.Add(header);\n        UiKit.Add(modal, "see http://example", "muted");')),
    m("B-X-13", "benign", "header label containing the word primary", (V, 'modal.Add(header);', 'modal.Add(header);\n        UiKit.Add(modal, "primary objective", "muted");')),
]

ALL = reproductions + f1 + f2 + benign


# =====================================================================================================
# #309 regression controls: RR-1 (static primary, helper, alias, allowlist, demotion), RR-2 (qualified
# and tuple writes, dead guards, compensated or related-state resets), RR-3 (bounded reachability and
# callers) and N-4 (lexer desynchronisation). `intended` must appear in the rejection reason.
# =====================================================================================================
SHOW_OBJ = 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);'
CLOSE_ALL = 'public void CloseAll()\n    {\n        CloseHelp();'
TRY_REVIEW = 'public bool TryShowReviewYear(int year)\n    {'
RESET_TP = 'public void ResetTeachingProgress()\n    {'
SETSCREEN_SIG = 'public void SetScreen(UiScreen next)'
SETSCREEN_OBJ = 'else if (screen == UiScreen.Objectives)\n            learning.Refresh(true);'
HELP_DECL = 'private int helpClosedFrame = -1;'
LATE_SIG = 'private void LateUpdate()'
DERIVE_COMMENT = '    // Derive only after a genuine card rebuild/view entry, never on an unchanged timer tick.'
ROOT_CLASS = 'public sealed class ScenarioOneUiRoot : MonoBehaviour'
ROOT_USING = 'using UnityEngine.InputSystem;'
VIEW_USING = 'using UnityEngine.UIElements;\n\n// Optional learning progress'
RELOAD = 'public void ReloadProgress()\n    {'
VIEW_REFRESH_SIG = '    public void Refresh(bool force)'
LIB_CALL = 'scroll.Add(library);\n        ui.RefreshNextActionGuidance();'
MODAL_HEADER = 'modal.Add(header);'
READ_BTN = 'Button read = UiKit.Button("I\'ve read and understood this", () => { Record(step.Id); Refresh(true); }, ready);'
WORK_BTN = 'Button work = UiKit.Button("Work Plan [Tab]", () => ui.ShowWorkPlan());'
CUR_MEMBER = 'public const string PreferencePrefix = "CCF.Learning.v1.";'
KIT_MODAL = 'backdrop.pickingMode = PickingMode.Position;'
KIT_ROW_SIG = 'public static VisualElement Row(VisualElement parent, params string[] classes)'
KIT_TEXT = 'foreach (string c in classes) label.AddToClassList(c);'
WALK_ANCHOR = 'ObjectiveWidget.Apply(ui.Learning.HudModel());'
STANDMAP_FIELD = 'private readonly ScenarioOneUiRoot ui;'
RECHECK = "unconditional top-level statement"

rr1 = [
    m("RR1-01", "RR-1", "static primary on a lesson card button outside the header",
      (V, READ_BTN, READ_BTN.replace('ready);', 'ready, "btn-primary");')), intended="btn-primary attached"),
    m("RR1-02", "RR-1", "lesson card button made primary from string fragments",
      (V, READ_BTN, READ_BTN.replace('ready);', 'ready, "btn-" + "primary");')), intended="not a plain string literal"),
    m("RR1-03", "RR-1", "primary-building helper in the Curriculum partial",
      ("cur", CUR_MEMBER, CUR_MEMBER + '\n    private static UnityEngine.UIElements.Button Promote(System.Action click) => UiKit.Button("Promote", click, true, "btn-primary");'),
      intended="btn-primary attached"),
    m("RR1-04", "RR-1", "class-list helper in the Sequence partial",
      (SEQ, SEQ_MEMBER, SEQ_MEMBER + '\n    private static void Tint(UnityEngine.UIElements.Button b) => b.EnableInClassList("btn-primary", true);'),
      intended="class-list API"),
    m("RR1-05", "RR-1", "Back to forest primary class supplied by a class constant",
      (V, FOREST_BTN, FOREST_BTN.replace('"btn-primary"', 'PrimaryClass')),
      (V, CLASS_MEMBER, CLASS_MEMBER + '\n    private const string PrimaryClass = "btn-primary";'), intended="not a plain string literal"),
    m("RR1-06", "RR-1", "UiKit.Button taken as a method group",
      (V, RELOAD, RELOAD + '\n        System.Func<string, System.Action, bool, string[], Button> make = UiKit.Button;'),
      intended="method group"),
    m("RR1-07", "RR-1", "UiKit aliased by a using directive", (V, VIEW_USING, VIEW_USING.replace(';\n', ';\nusing Kit = UiKit;\n', 1)),
      intended="UiKit used other than"),
    m("RR1-08", "RR-1", "Work Plan made a second static primary (N-1)",
      (V, WORK_BTN, WORK_BTN.replace('ui.ShowWorkPlan());', 'ui.ShowWorkPlan(), true, "btn-primary");')), intended="N-1"),
    m("RR1-09", "RR-1", "Back to forest retargeted to the map", (V, FOREST_BTN, FOREST_BTN.replace('ui.CloseAll()', 'ui.ShowMap()')),
      intended="must be ui.CloseAll()"),
    m("RR1-10", "RR-1", "Back to forest no longer the static primary (no static primary left)",
      (V, FOREST_BTN, FOREST_BTN.replace(', "btn-primary"', '')), intended="the one static primary"),
    m("RR1-11", "RR-1", "allowlist cleared with Array.Clear from an unpinned method",
      (R, SHOW_OBJ, SHOW_OBJ + '\n        System.Array.Clear(GuidanceButtonNames, 0, 3);'), intended="GuidanceButtonNames must be used only"),
    m("RR1-12", "RR-1", "allowlist element overwritten through a qualified reference in CloseAll",
      (R, CLOSE_ALL, CLOSE_ALL + '\n        ScenarioOneUiRoot.GuidanceButtonNames[2] = "objective-none";'),
      intended="GuidanceButtonNames must be used only"),
    m("RR1-13", "RR-1", "Back to forest re-promoted from an unpinned root method",
      (R, SHOW_OBJ, SHOW_OBJ + '\n        learning.Root.Q<Button>("objective-back-to-forest")?.AddToClassList("btn-primary");'),
      intended="Objectives Root"),
    m("RR1-14", "RR-1", "re-promotion from the document root with name and class fragments",
      (R, RESET_TP, RESET_TP + '\n        document.rootVisualElement.Q<Button>("objective-" + "back-to-forest")?.AddToClassList("btn-" + "primary");'),
      intended="class-list call outside"),
    m("RR1-15", "RR-1", "root aliases the view object and disables every Objectives button",
      (R, RESET_TP, RESET_TP + '\n        LearningObjectivesView v = learning; v.Root.Query<Button>().ForEach(b => b.SetEnabled(false));'),
      intended="view field"),
    m("RR1-16", "RR-1", "Objectives Root reached through the public Learning property",
      (R, CLOSE_ALL, CLOSE_ALL + '\n        Learning.Root.SetEnabled(false);'), intended="Objectives Root"),
    m("RR1-17", "RR-1", "another class disables the map CTA by name",
      ("walk", WALK_ANCHOR, WALK_ANCHOR + '\n            ui.RootElement?.Q<Button>("objective-primary-cta")?.SetEnabled(false);'),
      intended="another class names"),
    m("RR1-18", "RR-1", "another class reaches the Objectives Root through ui.Learning",
      ("standmap", STANDMAP_FIELD, STANDMAP_FIELD + '\n    private void Sneak() => ui.Learning.Root.SetEnabled(false);'),
      intended="another class reaches"),
    m("RR1-19", "RR-1", "UiKit.Button overload makes two-argument buttons primary from fragments",
      (KIT, KIT_ROW_SIG, 'public static Button Button(string text, Action onClick) => Button(text, onClick, true, "btn-" + "primary");\n\n    '
       + KIT_ROW_SIG), intended="UiKit must declare exactly one Button"),
    m("RR1-20", "RR-1", "UiKit.MakeModal disables the whole modal", (KIT, KIT_MODAL, KIT_MODAL + '\n        backdrop.SetEnabled(false);'),
      intended="pinned method UiKit.MakeModal"),
    m("RR1-28", "RR-1", "UiKit.Text (called for the header title) disables its parent",
      (KIT, KIT_TEXT, KIT_TEXT + '\n        label.RegisterCallback<AttachToPanelEvent>(e => label.parent?.SetEnabled(false));'),
      intended="pinned method UiKit.Text"),
    m("RR1-21", "RR-1", "ScenarioOneUiRoot made partial (a second file could extend it)",
      (R, ROOT_CLASS, ROOT_CLASS.replace('sealed class', 'sealed partial class')), intended="non-partial"),
    m("RR1-22", "RR-1", "new LearningObjectivesView partial file disables the Objectives Root",
      ("extra", None, 'public sealed partial class LearningObjectivesView\n{\n    private void Sneak() => Root.SetEnabled(false);\n}\n'),
      intended="Objectives Root may be used only"),
    m("RR1-23", "RR-1", "constructor ui parameter reassigned after the click lambdas capture it",
      (V, ADD_FOREST, ADD_FOREST + '\n        ui = null;'), intended="header local 'ui'"),
    m("RR1-24", "RR-1", "header local passed to a helper from inside an interpolation hole",
      (V, MODAL_HEADER, MODAL_HEADER + '\n        _ = $"{Hide(forest)}";'),
      (V, CLASS_MEMBER, CLASS_MEMBER + '\n    private static int Hide(Button b) { b.SetEnabled(false); return 0; }'),
      intended="header local 'forest'"),
    m("RR1-25", "RR-1", "modal cleared after the header is attached", (V, MODAL_HEADER, MODAL_HEADER + '\n        modal.Clear();'),
      intended="header local 'modal'"),
    m("RR1-26", "RR-1", "upward element-tree traversal from the view", (V, RELOAD, RELOAD + '\n        progress?.GetFirstAncestorOfType<VisualElement>()?.SetEnabled(false);'),
      intended="traversal"),
    m("RR1-27", "RR-1", "[Conditional] applied through a ConditionalAttribute alias",
      (R, ROOT_USING, ROOT_USING + '\nusing Cond = System.Diagnostics.ConditionalAttribute;'),
      (R, SETSCREEN_SIG, '[Cond("UNITY_EDITOR")]\n    ' + SETSCREEN_SIG), intended="[Conditional]"),
]

rr2 = [
    m("RR2-01", "RR-2", "this.helpClosedFrame reset in an unpinned method", (R, SHOW_OBJ, SHOW_OBJ + '\n        this.helpClosedFrame = -1;'),
      intended="helpClosedFrame"),
    m("RR2-02", "RR-2", "this.guidanceCandidate nulled in CloseAll", (R, CLOSE_ALL, CLOSE_ALL + '\n        this.guidanceCandidate = null;'),
      intended="guidanceCandidate"),
    m("RR2-03", "RR-2", "this.nextAction nulled in TryShowReviewYear", (R, TRY_REVIEW, TRY_REVIEW + '\n        this.nextAction = null;'),
      intended="nextAction"),
    m("RR2-04", "RR-2", "tuple reset of the guidance state", (R, CLOSE_ALL, CLOSE_ALL + '\n        (guidanceCandidate, guidanceScope) = (null, null);'),
      intended="guidanceCandidate"),
    m("RR2-05", "RR-2", "help-close frame reset through a ref argument",
      (R, CLOSE_ALL, CLOSE_ALL + '\n        System.Threading.Interlocked.Exchange(ref helpClosedFrame, -1);'), intended="helpClosedFrame"),
    m("RR2-06", "RR-2", "primary tracking cleared outside Refresh (restoration lost)",
      (R, SHOW_OBJ, SHOW_OBJ + '\n        guidancePrimaries.Clear();'), intended="guidancePrimaries"),
    m("RR2-07", "RR-2", "dead first guard: if (true) return", (R, FIRST_GUARD, 'if (true) return;'),
      intended="pinned method RefreshNextActionGuidance"),
    m("RR2-08", "RR-2", "target retargeted through a ref helper before binding",
      (R, BIND, 'Retarget(ref target);\n        ' + BIND),
      (R, APPLY_SIG, 'private static void Retarget(ref string t) { t = "objective-back-to-forest"; }\n    ' + APPLY_SIG),
      intended="RefreshNextActionGuidance"),
    m("RR2-09", "RR-2", "map and Back to forest targets swapped (count unchanged)",
      (R, 'if (objectives) target = "objective-primary-cta";', 'if (objectives) target = "objective-back-to-forest";'),
      (R, 'if (markable) target = objectives ? "objective-back-to-forest"', 'if (markable) target = objectives ? "objective-primary-cta"'),
      intended="RefreshNextActionGuidance"),
    m("RR2-10", "RR-2", "Help reference reset: guidance suspended forever", (R, CLOSE_ALL, CLOSE_ALL + '\n        help = null;'),
      intended="assign 'help' exactly 1"),
    m("RR2-11", "RR-2", "manager reference reset through this.", (R, RESET_TP, RESET_TP + '\n        this.manager = null;'),
      intended="assign 'manager' exactly 1"),
    m("RR2-12", "RR-2", "Objectives view reference reset: Refresh returns at its first guard",
      (R, CLOSE_ALL, CLOSE_ALL + '\n        learning = null;'), intended="view field"),
    m("RR2-13", "RR-2", "help-close frame initialiser changed", (R, HELP_DECL, 'private int helpClosedFrame = 0;'),
      intended="helpClosedFrame"),
    m("RR2-15", "RR-2", "screen state reset right after Objectives opens (no Objectives guidance scope)",
      (R, 'SetScreen(UiScreen.Objectives);', 'SetScreen(UiScreen.Objectives);\n        this.screen = UiScreen.None;'),
      intended="assign 'screen' exactly 3"),
    m("RR2-14", "RR-2", "guidance scope reset inside a lambda", (R, CLOSE_ALL, CLOSE_ALL + '\n        System.Action reset = () => guidanceScope = null; reset();'),
      intended="guidanceScope"),
]

rr3 = [
    m("RR3-01", "RR-3", "[Conditional] on the view Refresh (the caller vanishes from Player builds)",
      (V, VIEW_REFRESH_SIG, '    [System.Diagnostics.Conditional("UNITY_EDITOR")]\n' + VIEW_REFRESH_SIG), intended="[Conditional]"),
    m("RR3-02", "RR-3", "[Conditional] on SetScreen", (R, SETSCREEN_SIG, '[System.Diagnostics.Conditional("UNITY_EDITOR")]\n    ' + SETSCREEN_SIG),
      intended="[Conditional]"),
    m("RR3-03", "RR-3", "unreviewed attribute on ApplyNextActionGuidance",
      (R, APPLY_SIG, '[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]\n    ' + APPLY_SIG),
      intended="attribute on a checked method"),
    m("RR3-04", "RR-3", "Objectives caller behind if (false)", (V, VIEW_REFRESH_CALL, 'if (false) ' + VIEW_REFRESH_CALL), intended=RECHECK),
    m("RR3-05", "RR-3", "Objectives caller inside a never-invoked lambda",
      (V, VIEW_REFRESH_CALL, 'System.Action deferred = () => ui.RefreshNextActionGuidance();'), intended=RECHECK),
    m("RR3-06", "RR-3", "early return before the Objectives caller",
      (V, LIB_CALL, 'scroll.Add(library);\n        if (scroll != null) return;\n        ui.RefreshNextActionGuidance();'), intended=RECHECK),
    m("RR3-07", "RR-3", "throw before the Objectives caller",
      (V, LIB_CALL, 'scroll.Add(library);\n        if (force) throw new System.InvalidOperationException();\n        ui.RefreshNextActionGuidance();'),
      intended=RECHECK),
    m("RR3-08", "RR-3", "goto over the Objectives caller",
      (V, LIB_CALL, 'scroll.Add(library);\n        goto skip;\n        ui.RefreshNextActionGuidance();\n        skip:'), intended=RECHECK),
    m("RR3-09", "RR-3", "local function named like the guidance method in the view Refresh",
      (V, LIB_CALL, LIB_CALL + '\n        void RefreshNextActionGuidance() { }'), intended="exactly once"),
    m("RR3-10", "RR-3", "SetScreen no longer refreshes the Objectives view",
      (R, SETSCREEN_OBJ, 'else if (screen == UiScreen.Objectives)\n            learning.Record("objectives.open");'),
      intended="pinned method SetScreen"),
    m("RR3-11", "RR-3", "LateUpdate apply behind if (false)",
      (R, LATE, 'if (false) ApplyNextActionGuidance();\n        if (competitors == null || manager == null) return;'), intended="LateUpdate"),
    m("RR3-12", "RR-3", "CTA primary class removed late from CloseAll", (R, CLOSE_ALL, CLOSE_ALL + '\n        nextAction?.RemoveFromClassList("btn-primary");'),
      intended="nextAction"),
    m("RR3-13", "RR-3", "map CTA primary removed from an unpinned method with class fragments",
      (R, RESET_TP, RESET_TP + '\n        learning.Root.Q<Button>("objective-primary-cta")?.RemoveFromClassList("btn-" + "primary");'),
      intended="Objectives Root"),
    m("RR3-14", "RR-3", "header row hidden through a Root query in the constructor",
      (V, MODAL_HEADER, MODAL_HEADER + '\n        Root.Q("modal-header").style.display = DisplayStyle.None;'), intended="header local 'Root'"),
]

n4 = [
    m("N4-01", "N-4", "char literal in an interpolation hole hides a root write",
      (R, CLOSE_ALL, CLOSE_ALL + '\n        _ = $"{\'"\'}"; guidanceCandidate = null; /*\'*/'), intended="N-4"),
    m("N4-02", "N-4", "@$ verbatim interpolation in a partial (lexed precisely, hidden code found)",
      (SEQ, SEQ_MEMBER, SEQ_MEMBER + '\n    private void Sneak() { _ = @$"\\"; Root.SetEnabled(false); _ = "\\""; }'),
      intended="Objectives Root may be used only"),
    m("N4-03", "N-4", "verbatim string opened in an inactive #if region hides active code",
      (V, RELOAD, RELOAD + '\n#if CCF_NEVER_DEFINED\n        _ = @"\n#endif\n        Root.SetEnabled(false);\n#if CCF_NEVER_DEFINED\n        ";\n#endif'),
      intended="N-4"),
    m("N4-04", "N-4", "block comment opened in an inactive #if region hides active code",
      (R, CLOSE_ALL, CLOSE_ALL + '\n#if CCF_NEVER_DEFINED\n        /*\n#endif\n        guidanceCandidate = null;\n#if CCF_NEVER_DEFINED\n        */\n#endif'),
      intended="N-4"),
    m("N4-05", "N-4", "Unicode-escaped identifier writes guarded state", (R, CLOSE_ALL, CLOSE_ALL + '\n        guidanc\\u0065Candidate = null;'),
      intended="N-4"),
]

new_benign = [
    m("NB-01", "benign", "pinned Refresh guard reformatted with a trailing comment",
      (R, 'if (scope == null) return;', 'if (scope == null)\n            return; // nothing to guide')),
    m("NB-02", "benign", "comments added inside pinned ApplyNextActionGuidance",
      (R, CLEAR_THEN_GUARD, '// drop a stale candidate first\n        ClearNextAction();\n        /* then the guard */ if (!eligible) return;')),
    m("NB-03", "benign", "new interpolated string with nested literals in a partial (precise lexer)",
      (SEQ, SEQ_MEMBER, SEQ_MEMBER + '\n    private static string StateNote(bool on) => $"State: {(on ? "on" : "off")}";')),
    m("NB-04", "benign", "additional root caller of RefreshNextActionGuidance", (R, SHOW_OBJ, SHOW_OBJ + '\n        RefreshNextActionGuidance();')),
    m("NB-05", "benign", "#region around LateUpdate",
      (R, LATE_SIG, '#region Per-frame presentation\n    ' + LATE_SIG), (R, DERIVE_COMMENT, '    #endregion\n\n' + DERIVE_COMMENT)),
    m("NB-06", "benign", "non-primary class on a lesson card button", (V, READ_BTN, READ_BTN.replace('ready);', 'ready, "btn-secondary");'))),
    m("NB-07", "benign", "unrelated new UiKit helper",
      (KIT, KIT_ROW_SIG, 'public static Label Hint(VisualElement parent, string text) => Add(parent, text, "muted");\n\n    ' + KIT_ROW_SIG)),
    m("NB-08", "benign", "plain non-primary class literal added in an unpinned root method",
      (R, CLOSE_ALL, CLOSE_ALL + '\n        previewBanner.AddToClassList("banner-closed");')),
    m("NB-12", "benign", "interpolation with an alignment comma in a lesson label",
      (V, 'UiKit.Add(card, text, "body");', 'UiKit.Add(card, $"{text,1}", "body");')),
    m("NB-13", "benign", "#if/#endif with trailing comments in an unpinned root method",
      (R, SHOW_OBJ, SHOW_OBJ + '\n#if UNITY_EDITOR // editor-only trace\n        UnityEngine.Debug.Log("objectives");\n#endif // UNITY_EDITOR')),
    m("NB-09", "benign", "pinned ShowMap re-spaced with a comment",
      (R, SHOW_MAP, 'public void ShowMap()\n    {\n        manager.OpenWorkPlan( false );\n        SetScreen( UiScreen.Map ); // open the stand map')),
]

# Reviewer cases that carry no `intended` field: the cause their #309 rejection must name.
REVIEWER_CAUSES = {
    "X-F1-01": "header row addition", "X-F1-02": "header local 'close'", "X-F1-03": "class-list API",
    "X-F1-04": "element-tree traversal", "X-F1-05": "class-list API", "X-F1-06": "UiKit must not attach btn-primary",
    "X-F1-07": "non-literal class name", "X-F1-08": "not a plain string literal", "X-F1-09": "(N-4)",
    "X-F1-10": "map CTA must not be unconditionally primary", "X-F1-11": "map CTA must not be unconditionally primary",
    "X-F1-12": "map CTA must not be unconditionally primary", "X-F1-13": "map CTA must not be unconditionally primary",
    "X-F1-14": "(N-4)", "X-F1-15": "map CTA must not be unconditionally primary",
    "X-F1-16": "must have exactly one literal name", "X-F1-17": "non-simple receiver", "X-F1-18": "non-simple receiver",
    "X-F1-19": "not assigned to a named local", "X-F1-20": "declared once and never reassigned",
    "X-F1-24": "header local 'forest'", "X-F1-25": "header local 'forest'", "X-F1-26": "header local 'forest'",
    "X-F1-27": "GuidanceButtonNames must be used only", "X-F1-28": "header local 'close'", "X-F1-29": "(N-4)",
    "X-F1-30": "(N-4)", "X-F1-31": "header local 'work'", "X-F1-32": "only Back to forest is statically primary (N-1)",
    "X-F1-33": "header button 'Work Plan [Tab]'", "X-F1-34": "header button 'Stand map [M]' must be declared exactly once",
    "X-F1-35": "not in the GuidanceButtonNames scoped demotion set",
    "X-F2-01": "header local 'buttons'", "X-F2-02": "header local 'buttons'", "X-F2-03": "header local 'buttons'",
    "X-F2-04": "header local 'buttons'", "X-F2-05": "header local 'buttons'", "X-F2-06": "header local 'map'",
    "X-F2-07": "header local 'header'", "X-F2-08": "pinned method RefreshNextActionGuidance",
    "X-F2-09": "pinned method RefreshNextActionGuidance", "X-F2-10": "pinned method RefreshNextActionGuidance",
    "X-F2-11": "pinned method RefreshNextActionGuidance", "X-F2-12": "pinned method RefreshNextActionGuidance",
    "X-F2-13": "pinned method RefreshNextActionGuidance", "X-F2-14": "pinned method RefreshNextActionGuidance",
    "X-F2-15": "pinned method RefreshNextActionGuidance", "X-F2-16": "pinned method RefreshNextActionGuidance",
    "X-F2-17": "pinned method RefreshNextActionGuidance", "X-F2-18": "'nextAction' is referenced outside",
    "X-F2-19": "pinned method ApplyNextActionGuidance", "X-F2-20": "pinned method ApplyNextActionGuidance",
    "X-F2-21": "[Conditional]", "X-F2-22": "private void LateUpdate() { ApplyNextActionGuidance();",
    "X-F2-23": "'ApplyNextActionGuidance' is referenced outside", "X-F2-24": "pinned method ApplyNextActionGuidance",
    "X-F2-25": "pinned method ApplyNextActionGuidance", "X-F2-26": "pinned method ApplyNextActionGuidance",
    "X-F2-27": "'guidanceCandidate' must be written only", "X-F2-28": "helpClosedFrame must be written only",
    "X-F2-29": "pinned method CloseHelp", "X-F2-30": "pinned method CloseHelp", "X-F2-31": "pinned method GuidanceVisible",
    "X-F2-32": "pinned method GuidanceVisible", "X-F2-33": "must be called exactly once by the view class",
    "X-F2-34": "conditional compilation", "X-F2-35": "pinned method ShowMap",
}


def run():
    live = {key: (REPO / path).read_text(encoding="utf-8") for key, path in FILES.items() if (REPO / path).exists()}
    companions = collect_companions(REPO)

    def verify(files):
        merged = dict(companions)
        merged.update({FILES[key]: text for key, text in files.items() if key not in (V, R)})
        return verify_objective_cta(files[V], files[R], merged)

    def apply(label, edits):
        files = dict(live)
        for which, old, new in edits:
            if old is None:  # a new, in-memory-only file
                if which in files:
                    raise RuntimeError(f"{label}: {which} already exists")
                files[which] = new
            else:
                files[which] = replace_one(files[which], old, new, label)
        return files

    original_cases = [
        ("map-button-name", "view",
         'map.name = "objective-primary-cta";', 'map.name = "objective-wrong-cta";'),
        ("map-click-destination", "view",
         '() => ui.ShowMap());', '() => ui.CloseAll());'),
        ("map-caption", "view",
         '"Stand map [M]"', '"Next"'),
        ("map-not-added-to-row", "view",
         'buttons.Add(map);', 'buttons.Add(work);'),
        ("duplicate-map-identity", "view",
         'map.name = "objective-primary-cta";',
         'map.name = "objective-primary-cta";\n        map.name = "objective-primary-cta";'),
        ("target-renamed", "root",
         'if (objectives) target = "objective-primary-cta";',
         'if (objectives) target = "objective-wrong-cta";'),
        ("objective-condition-removed", "root",
         'else if (!manager.RackPlanConfirmed)',
         'else if (manager.RackPlanConfirmed)'),
        ("rack-v2-gate-removed", "root",
         'else if (manager.RackPlanningV2Active && (learning.TeachingRacks',
         'else if (true && (learning.TeachingRacks'),
        ("help-guard-removed", "root",
         '|| help == null || help.IsOpen || helpClosedFrame == Time.frameCount;',
         '|| help == null || helpClosedFrame == Time.frameCount;'),
        ("reference-preview-guard-removed", "root",
         'manager == null || manager.ReferencePreviewActive || StandaloneSessionMenu.IsOpen',
         'manager == null || StandaloneSessionMenu.IsOpen'),
        ("candidate-enabled-check-removed", "root",
         'candidate.enabledInHierarchy && GuidanceVisible(candidate, scope)',
         'GuidanceVisible(candidate, scope)'),
        ("scope-visibility-check-removed", "root",
         'candidate.enabledInHierarchy && GuidanceVisible(candidate, scope)',
         'candidate.enabledInHierarchy'),
        ("hidden-visibility-check-removed", "root",
         '|| current.resolvedStyle.visibility == Visibility.Hidden',
         ''),
        ("allowlist-primary-dedup-removed", "root",
         'guidancePrimaries[owned] = owned.ClassListContains("btn-primary");',
         'guidancePrimaries[owned] = false;'),
        ("unrelated-global-demotion", "root",
         'Button owned = scope.Q<Button>(name);',
         'Button owned = document.rootVisualElement.Q<Button>(name);'),
        ("primary-restore-removed", "root",
         'entry.Key.EnableInClassList("btn-primary", entry.Value);',
         'entry.Key.RemoveFromClassList("btn-primary");'),
        ("primary-class-not-added", "root",
         'candidate.AddToClassList("btn-primary");',
         'candidate.RemoveFromClassList("btn-primary");'),
        ("next-action-class-not-added", "root",
         'candidate.AddToClassList("next-action");',
         'candidate.RemoveFromClassList("next-action");'),
        ("eligibility-return-removed", "root",
         'if (!eligible) return;',
         'if (eligible) return;'),
        ("target-not-bound-to-scoped-button", "root",
         'scope.Q<Button>(target) : null;',
         'learning.Root.Q<Button>("objective-back-to-forest") : null;'),
    ]

    # F1 (#304): every static btn-primary source in the Objectives header must be uniquely named and
    # inside the scoped demotion set; the map may never be primary outside guidance.
    close_primary = ('Button close = UiKit.Button("Close", () => ui.CloseAll(), true, "btn-primary");\n'
                     '        buttons.Add(close);')
    f1_cases = [
        ("R01-unconditional-map-primary", "view",
         'map.name = "objective-primary-cta";',
         'map.name = "objective-primary-cta";\n        map.AddToClassList("btn-primary");'),
        ("R01b-map-primary-via-enable-class", "view",
         'map.name = "objective-primary-cta";',
         'map.name = "objective-primary-cta";\n        map.EnableInClassList("btn-primary", true);'),
        ("R01c-map-primary-verbatim-spelling", "view",
         'map.name = "objective-primary-cta";',
         'map.name = "objective-primary-cta";\n        map.AddToClassList(@"btn-primary");'),
        ("R01d-map-class-non-literal", "view",
         'map.name = "objective-primary-cta";',
         'map.name = "objective-primary-cta";\n        string cls = "btn-" + "primary";\n        map.AddToClassList(cls);'),
        ("R03-back-to-forest-removed-from-allowlist", "root",
         '"objective-primary-cta", "objective-work-plan", "objective-back-to-forest",',
         '"objective-primary-cta", "objective-work-plan",'),
        ("R04-back-to-forest-renamed-out-of-allowlist", "view",
         'forest.name = "objective-back-to-forest";', 'forest.name = "objective-forest";'),
        ("R05-new-unnamed-closeall-primary", "view",
         'buttons.Add(forest);', 'buttons.Add(forest);\n        ' + close_primary),
        ("R05b-new-closeall-primary-named-outside-allowlist", "view",
         'buttons.Add(forest);',
         'buttons.Add(forest);\n        ' + close_primary + '\n        close.name = "objective-close";'),
        ("R05c-inline-unnamed-primary", "view",
         'buttons.Add(forest);',
         'buttons.Add(forest);\n        buttons.Add(UiKit.Button("Close", () => ui.CloseAll(), true, "btn-primary"));'),
        ("R05d-new-closeall-primary-reusing-allowlisted-name", "view",
         'buttons.Add(forest);',
         'buttons.Add(forest);\n        ' + close_primary + '\n        close.name = "objective-back-to-forest";'),
        ("R05e-named-argument-class", "view",
         'buttons.Add(forest);',
         'buttons.Add(forest);\n        Button odd = UiKit.Button("Odd", () => ui.CloseAll(), classes: "btn-primary");'),
        ("R05f-primary-class-in-unsupported-shape", "view",
         'buttons.Add(forest);', 'buttons.Add(forest);\n        UiKit.Add(modal, "x", "btn-primary");'),
        ("R05g-forest-primary-reassigned", "view",
         'buttons.Add(forest);', 'buttons.Add(forest);\n        forest = null;'),
        ("allowlist-duplicate-name", "root",
         '"objective-primary-cta", "objective-work-plan", "objective-back-to-forest",',
         '"objective-primary-cta", "objective-work-plan", "objective-back-to-forest", "objective-work-plan",'),
    ]

    # F2/F3 (#304): bounded override, spoof, ordering and spelling variants.
    f2_cases = [
        ("R07-verbatim-duplicate-cta-name", "view",
         'work.name = "objective-work-plan";', 'work.name = @"objective-primary-cta";'),
        ("R07b-escaped-duplicate-cta-name", "view",
         'work.name = "objective-work-plan";', 'work.name = "objective\\u002dprimary-cta";'),
        ("R07c-concatenated-name", "view",
         'work.name = "objective-work-plan";', 'work.name = "objective-" + "work-plan";'),
        ("R08-later-map-name-overwrite", "view",
         'buttons.Add(map);', 'buttons.Add(map);\n        map.name = "objective-map";'),
        ("R09-anchor-spoofed-in-string-literal", "root",
         'if (!eligible) return;', 'UnityEngine.Debug.Log("if (!eligible) return;");'),
        ("R09b-view-anchor-spoofed-in-string-literal", "view",
         'buttons.Add(map);', 'string spoof = "buttons.Add(map);";'),
        ("R11-target-override-to-closeall", "root",
         'guidanceCandidate = target != null ? scope.Q<Button>(target) : null;',
         'if (scope == learning.Root) target = "objective-back-to-forest";\n'
         '        guidanceCandidate = target != null ? scope.Q<Button>(target) : null;'),
        ("R12-candidate-nulled-in-apply", "root",
         'Button candidate = guidanceCandidate;', 'Button candidate = guidanceCandidate;\n        candidate = null;'),
        ("R13-unreachable-return-in-refresh", "root",
         'guidancePrimaries.Clear();', 'guidancePrimaries.Clear();\n        return;'),
        ("R14-primary-hidden-behind-unity-editor", "root",
         'candidate.AddToClassList("btn-primary");',
         '#if UNITY_EDITOR\n        candidate.AddToClassList("btn-primary");\n#endif'),
        ("R14b-cta-name-hidden-behind-conditional", "view",
         'map.name = "objective-primary-cta";',
         '#if UNITY_EDITOR\n        map.name = "objective-primary-cta";\n#endif'),
        ("R15-map-disabled-after-binding", "view",
         'buttons.Add(map);', 'buttons.Add(map);\n        map.SetEnabled(false);'),
        ("R16b-extra-return-true-in-visibility", "root",
         'for (VisualElement current = element;', 'if (element == null) return true;\n        for (VisualElement current = element;'),
        ("R17b-candidate-disabled-in-apply", "root",
         'candidate.AddToClassList("next-action");',
         'candidate.AddToClassList("next-action");\n        candidate.SetEnabled(false);'),
        ("R20-help-close-frame-setter-removed", "root",
         'helpClosedFrame = Time.frameCount;', ''),
        ("R20b-help-close-frame-conditional", "root",
         'helpClosedFrame = Time.frameCount;', 'if (false) helpClosedFrame = Time.frameCount;'),
        ("R20c-help-close-frame-reset", "root",
         'helpClosedFrame = Time.frameCount;', 'helpClosedFrame = Time.frameCount;\n        helpClosedFrame = -1;'),
        ("R22-candidate-nulled-in-clear", "root",
         'nextAction.RemoveFromClassList("next-action");',
         'nextAction.RemoveFromClassList("next-action");\n            guidanceCandidate = null;'),
        ("R23-candidate-nulled-by-unrelated-handler", "root",
         'helpClosedFrame = Time.frameCount;', 'helpClosedFrame = Time.frameCount;\n        guidanceCandidate = null;'),
        ("R21-late-update-apply-removed", "root",
         'ApplyNextActionGuidance();\n        if (competitors == null || manager == null) return;',
         'if (competitors == null || manager == null) return;'),
        ("R21b-late-update-apply-after-return", "root",
         'ApplyNextActionGuidance();\n        if (competitors == null || manager == null) return;',
         'if (competitors == null || manager == null) return;\n        ApplyNextActionGuidance();'),
    ]
    # Benign edits must still PASS; they prove the guards are not matching on layout or comments.
    benign_cases = [
        ("B01-trailing-comment-after-eligibility-return", "root",
         'if (!eligible) return;', 'if (!eligible) return; // unchanged behaviour'),
        ("B02-map-constructor-reformatted", "view",
         'Button map = UiKit.Button("Stand map [M]", () => ui.ShowMap());',
         'Button map = UiKit.Button(\n            "Stand map [M]",\n            () => ui.ShowMap());'),
        ("B03-comment-mentions-primary-and-conditional", "view",
         'buttons.Add(forest);',
         'buttons.Add(forest); // forest.AddToClassList("btn-primary"); #if UNITY_EDITOR'),
        ("B04-new-nonprimary-header-button-unnamed", "view",
         'buttons.Add(forest);',
         'buttons.Add(forest);\n        buttons.Add(UiKit.Button("Info", () => ui.CloseAll()));'),
    ]

    baseline = verify(live)
    print("SOURCE-STATIC PASS objective-cta-live-source: "
          f"{len(baseline)} anchored evidence rows (UI runtime NOT TESTED)")

    def plain(cases):  # original/author format: (label, which, old, new); any rejection counts
        return [(label, [(which, old, new)], None) for label, which, old, new in cases]

    def corpus(cases):  # #305 and #309 format; the rejection must name the intended cause
        return [(c["id"], c["edits"], c["intended"] or REVIEWER_CAUSES.get(c["id"])) for c in cases]

    groups = [("original", plain(original_cases)), ("F1", plain(f1_cases)), ("F2-F3", plain(f2_cases)),
              ("#305 R-IDs", corpus(reproductions)), ("#305 X-F1", corpus(f1)), ("#305 X-F2", corpus(f2)),
              ("#309 RR-1", corpus(rr1)), ("#309 RR-2", corpus(rr2)), ("#309 RR-3", corpus(rr3)),
              ("#309 N-4", corpus(n4))]
    total = rejected = wrong_cause = 0
    for group, cases in groups:
        group_rejected = 0
        for label, edits, cause in cases:
            try:
                verify(apply(label, edits))
            except ObjectiveCtaSourceError as exc:
                if cause is None or cause in str(exc):
                    group_rejected += 1
                    print(f"SOURCE-STATIC PASS mutation-{label}: rejected ({exc})")
                else:
                    wrong_cause += 1
                    print(f"SOURCE-STATIC FAIL mutation-{label}: rejected for an unexpected cause ({exc}); "
                          f"expected {cause!r}")
            else:
                print(f"SOURCE-STATIC FAIL mutation-{label}: FALSE GREEN")
        print(f"SOURCE-STATIC CTA MUTATION GROUP {group}: rejected={group_rejected}/{len(cases)}")
        total += len(cases)
        rejected += group_rejected

    def whole_file(label, keys, transform):
        files = dict(live)
        for key in keys:
            files[key] = transform(files[key])
        return files
    benign_runs = ([(f"B-{label}", apply(label, [(which, old, new)])) for label, which, old, new in benign_cases]
                   + [(c["id"], apply(c["id"], c["edits"])) for c in benign + new_benign]
                   + [("B-X-14", whole_file("B-X-14", (V, R), lambda s: s.replace("\n", "\r\n"))),
                      ("B-X-15", whole_file("B-X-15", (V, R), lambda s: "\ufeff" + s)),
                      ("NB-10", whole_file("NB-10", (SEQ, "cur", KIT), lambda s: s.replace("\n", "\r\n"))),
                      ("NB-11", whole_file("NB-11", (SEQ, "cur", KIT), lambda s: "\ufeff" + s))])
    benign_failed = 0
    for label, files in benign_runs:
        try:
            verify(files)
        except ObjectiveCtaSourceError as exc:
            benign_failed += 1
            print(f"SOURCE-STATIC FAIL benign-{label}: WRONGLY REJECTED ({exc})")
        else:
            print(f"SOURCE-STATIC PASS benign-{label}: accepted")
    print(f"SOURCE-STATIC CTA BENIGN CONTROLS accepted={len(benign_runs)-benign_failed}/{len(benign_runs)}")

    # F6: the historical pre-guidance source must be reported as a completed, evidenced FAIL, never a crash.
    historic_failed = 0
    try:
        old_view, old_root = (subprocess.run(["git", "-C", str(REPO), "show", f"{BASE}:{path}"],
                                             capture_output=True, text=True, check=True).stdout
                              for path in (VIEW, ROOT))
        verify_objective_cta(old_view, old_root)
    except ObjectiveCtaSourceError as exc:
        print(f"SOURCE-STATIC PASS historical-baseline-rejected: {exc} (evidence rows={len(exc.rows)})")
        if not exc.rows:
            historic_failed += 1
            print("SOURCE-STATIC FAIL historical-baseline-evidence: no source evidence rows")
    except (OSError, subprocess.CalledProcessError) as exc:
        historic_failed += 1
        print(f"SOURCE-STATIC FAIL historical-baseline: cannot read {BASE}: {exc}")
    else:
        historic_failed += 1
        print("SOURCE-STATIC FAIL historical-baseline: FALSE GREEN")

    false_greens = total - rejected - wrong_cause
    print(f"SOURCE-STATIC CTA MUTATIONS rejected={rejected}/{total} false_greens={false_greens} "
          f"wrong_cause={wrong_cause}")
    return 1 if false_greens or wrong_cause or benign_failed or historic_failed else 0


if __name__ == "__main__":
    raise SystemExit(run())
