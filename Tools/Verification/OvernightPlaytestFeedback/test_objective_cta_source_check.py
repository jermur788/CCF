#!/usr/bin/env python3
"""Negative controls for the Objective CTA source scanner; no Unity or .NET.

Runs against source in the current checkout, then mutates in-memory fixture
copies only. A claimed success requires all deliberately broken cases to be
rejected (the #309 cases also for their intended cause), every benign edit
accepted and the historical baseline rejected. The original numbered historical
checks live in check_baseline.py.

Groups: the original 20, the #304 author's F1 (14) and F2/F3 (21) controls and
4 benign edits, unchanged; the #305 independent reviewer's corpus (21 re-derived
R-IDs, 35 X-F1, 46 X-F2 and 15 benign), copied verbatim below; the #309
RR-1/RR-2/RR-3/N-4 regression and benign controls; and the #309 repair loop 1
groups (the #314 reviewer's cases and this loop's own, with benign edits and
reported accepted-limit/over-rejection probes); and the #320 two-guard groups: all
654 cases of the #314 reviewer's 11 rounds (including the 47 N-1 and 10 N-2 false
greens, which must now fail for that cause) and this repair's own N-1/N-2 variants,
benign edits and probes. #325 adds the three companion/base-name rebinding witnesses,
five nearby negatives and five benign controls, reported separately from those prior counts.
"""
import subprocess
from pathlib import Path
from objective_cta_source_check import (
    ROOT, UIKIT, VIEW, ObjectiveCtaSourceError, collect_companions, is_companion_source, verify_objective_cta
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


# =====================================================================================================
# #309 repair loop 1 (post-#314). The #314 reviewer's 25 in-scope survivors (S-1, S-2, S-3), its 3
# lone-CR cases, its 2 L-06 probes, 21 benign edits and 3 limit probes, transcribed from its
# my_mutations*.py with ascii() (invisible characters appear as escapes; the loop-1 evidence proves each
# transcribed edit identical), then this loop's own attacks, benign edits and probes. Edits name
# repository paths; as in check_baseline.py, a file other than the view and the root reaches the scanner
# only if is_companion_source() selects it. `intended` must appear in the rejection reason. Accepted-limit
# and over-rejection probes are reported, not counted as mutations; one whose documented outcome changes
# fails the run, so the README known limits stay true.
# =====================================================================================================
UI = "Assets/ForestPrototype/UI/"


def lc(mid, group, why, *edits, intended=None, transform=None):
    """A loop-1 case: edits are (repository path, old text or None for a new file, new text)."""
    return dict(id=mid, group=group, why=why, edits=list(edits), intended=intended, transform=transform)


loop1_reviewer_s1 = [
    lc('Z-RR2-T-help',
      'RR-2',
      'tuple deconstruction writes help in an unpinned method',
      (ROOT, 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);', 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);\n        (help, workPlanWasOpen) = (null, false);'),
      intended='RR-2 related state'),
    lc('Z-RR2-T-screen',
      'RR-2',
      'tuple deconstruction writes screen in an unpinned method',
      (ROOT, 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);', 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);\n        (screen, workPlanWasOpen) = (UiScreen.None, false);'),
      intended='RR-2 related state'),
    lc('Z-RR2-T-rackLayout',
      'RR-2',
      'tuple deconstruction writes rackLayout in an unpinned method',
      (ROOT, 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);', 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);\n        (rackLayout, workPlanWasOpen) = (null, false);'),
      intended='RR-2 related state'),
    lc('Z-RR2-T-map',
      'RR-2',
      'tuple deconstruction writes map in an unpinned method',
      (ROOT, 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);', 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);\n        (map, workPlanWasOpen) = (null, false);'),
      intended='RR-2 related state'),
    lc('Z-RR2-T-workPlan',
      'RR-2',
      'tuple deconstruction writes workPlan in an unpinned method',
      (ROOT, 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);', 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);\n        (workPlan, workPlanWasOpen) = (null, false);'),
      intended='RR-2 related state'),
    lc('Z-RR2-T-review',
      'RR-2',
      'tuple deconstruction writes review in an unpinned method',
      (ROOT, 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);', 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);\n        (review, workPlanWasOpen) = (null, false);'),
      intended='RR-2 related state'),
    lc('U-RR2-T-manager',
      'RR-2',
      'tuple deconstruction writes manager (null) in an unpinned method',
      (ROOT, 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);', 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);\n        (manager, workPlanWasOpen) = (null, false);'),
      intended='RR-2 related state'),
    lc('Z-RR2-C-screen',
      'RR-2',
      'count-compensated: TryShowReviewYear write removed, ShowObjectives retargets screen to Map',
      (ROOT, 'screen = UiScreen.Review;\n        CloseHelp();', 'CloseHelp();'),
      (ROOT, 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);\n        SetScreen(UiScreen.Objectives);', 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);\n        SetScreen(UiScreen.Objectives);\n        screen = UiScreen.Map;'),
      intended='RR-2 related state'),
    lc('Z-RR2-C-help',
      'RR-2',
      "count-compensated: Start's help initialiser replaced by null (one write, value changed)",
      (ROOT, 'help = new MenuHelpView(this);', 'help = null;'),
      intended='RR-2 related state'),
    lc('Y-RR2-08',
      'RR-2',
      'screen retargeted inside TryShowReviewYear (count unchanged)',
      (ROOT, 'screen = UiScreen.Review;\n        CloseHelp();', 'screen = UiScreen.None;\n        CloseHelp();'),
      intended='RR-2 related state'),
]
loop1_reviewer_s2 = [
    lc('Y-N-10',
      'novel',
      'U+2028 ends a // comment: hidden call after it',
      (UI + 'LearningObjectivesView.Sequence.cs', 'private int teachingWorldVersion = -1;', 'private int teachingWorldVersion = -1;\n    private void Sneak() { // harmless\u2028 Root.SetEnabled(false); }'),
      intended='(N-4)'),
    lc('Y-N-11',
      'novel',
      "zero-width joiner inside the 'forest' identifier (Cf ignored by the C# compiler)",
      (VIEW, 'buttons.Add(forest);', 'buttons.Add(forest);\n        fo\u200drest.name += "-x";'),
      intended='(N-4)'),
    lc('Y-N-12',
      'novel',
      'zero-width joiner inside a partial class name hides the companion',
      (UI + 'LearningObjectivesView.Cf.cs', None, 'public sealed partial class Learning\u200dObjectivesView\n{\n    private void Sneak() => Root.SetEnabled(false);\n}\n'),
      intended='(N-4)'),
    lc('Y-N-13',
      'novel',
      'NEL (U+0085) ends a // comment: hidden call after it',
      (UI + 'LearningObjectivesView.Sequence.cs', 'private int teachingWorldVersion = -1;', 'private int teachingWorldVersion = -1;\n    private void Sneak() { // harmless\x85 Root.SetEnabled(false); }'),
      intended='(N-4)'),
    lc('Y-N-14',
      'novel',
      'Cf char inside GuidanceButtonNames reference outside the pins',
      (ROOT, 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);', 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);\n        Guidance\u200dButtonNames[2] = "objective-none";'),
      intended='(N-4)'),
    lc('Y-N-15',
      'novel',
      'Cf char inside guarded helpClosedFrame write',
      (ROOT, 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);', 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);\n        helpClosed\u200dFrame = -1;'),
      intended='(N-4)'),
    lc('Z-N-PS',
      'novel',
      'U+2029 ends a // comment: hidden call after it',
      (UI + 'LearningObjectivesView.Sequence.cs', 'private int teachingWorldVersion = -1;', 'private int teachingWorldVersion = -1;\n    private void Sneak() { // harmless\u2029 Root.SetEnabled(false); }'),
      intended='(N-4)'),
    lc('Z-N-CF-buttons',
      'novel',
      'ZWJ identifier clears the header row in the ctor',
      (VIEW, 'modal.Add(header);', 'modal.Add(header);\n        bu\u200dttons.Clear();'),
      intended='(N-4)'),
    lc('Z-N-CF-secondprimary',
      'novel',
      'second STATIC PRIMARY in the view via ZWJ-split UiKit and fragment class (N-1 break)',
      (VIEW, 'scroll.Add(library);\n        ui.RefreshNextActionGuidance();', 'scroll.Add(Ui\u200dKit.Button("Close", () => ui.CloseAll(), true, "btn-" + "primary"));\n        scroll.Add(library);\n        ui.RefreshNextActionGuidance();'),
      intended='(N-4)'),
    lc('Z-N-CF-allowlist-clear',
      'novel',
      'ZWJ-split Array.Clear of the allowlist',
      (ROOT, 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);', 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);\n        System.Array.Clear(Guidance\u200bButtonNames, 0, 3);'),
      intended='(N-4)'),
    lc('Z-N-SHY',
      'novel',
      'soft hyphen (Cf) in forest identifier',
      (VIEW, 'buttons.Add(forest);', 'buttons.Add(forest);\n        for\xadest.name += "-x";'),
      intended='(N-4)'),
    lc('Y-N-09',
      'novel',
      'lone CR ends a // comment: hidden call after it',
      (UI + 'LearningObjectivesView.Sequence.cs', 'private int teachingWorldVersion = -1;', 'private int teachingWorldVersion = -1;\n    private void Sneak() { // harmless\r Root.SetEnabled(false); }'),
      intended='(N-4)'),
    lc('Z-N-CR-region',
      'novel',
      'lone CR ends a #region line: hidden call after it',
      (UI + 'LearningObjectivesView.Sequence.cs', 'private int teachingWorldVersion = -1;', 'private int teachingWorldVersion = -1;\n    #region x\r    private void Sneak() { Root.SetEnabled(false); }\n    #endregion'),
      intended='(N-4)'),
    lc('Z-N-CR-root',
      'novel',
      'lone CR hides a guarded-state write in the root',
      (ROOT, 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);', 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);\n        // reset\r guidanceCandidate = null;'),
      intended='(N-4)'),
]
loop1_reviewer_s3 = [
    lc('W-DEP-01',
      'dep',
      'NeedsAnnualReview forced true: pinned chain always diverts to the annual-review target (X-F2-12 effect)',
      (ROOT, 'public bool NeedsAnnualReview => manager != null && manager.AnnualReports.Count > 0 && !manager.AnnualReviewSeen;', 'public bool NeedsAnnualReview => true;'),
      intended='RR-3 routing input'),
    lc('W-DEP-02',
      'dep',
      'TeachingRacks forced false: Objective 1 never offered a guidance target (X-F2-14 effect)',
      (VIEW, 'public bool TeachingRacks => CurrentTopicIndex == 0;', 'public bool TeachingRacks => false;'),
      intended='RR-3 routing input'),
    lc('W-DEP-03',
      'dep',
      'CurrentTopicIndex forced to 1',
      (VIEW, 'public int CurrentTopicIndex => ScenarioOneTeachingSequence.Current(TeachingFacts());', 'public int CurrentTopicIndex => 1;'),
      intended='RR-3 routing input'),
    lc('W-DEP-04',
      'dep',
      'NeedsAnnualReview routed through a helper returning true',
      (ROOT, 'public bool NeedsAnnualReview => manager != null && manager.AnnualReports.Count > 0 && !manager.AnnualReviewSeen;', 'public bool NeedsAnnualReview => Always();\n    private static bool Always() => true;'),
      intended='RR-3 routing input'),
]
loop1_reviewer_l06 = [
    lc('V-L06-1',
      'limit',
      'companion aliases ui.Learning in a local then restyles every Objectives button (disclosed L-06)',
      (UI + 'Alias.cs', None, 'using UnityEngine.UIElements;\npublic sealed class Alias { public void Go(ScenarioOneUiRoot ui) { var v = ui.Learning; v.Root.Query<Button>().ForEach(b => b.AddToClassList("btn-primary")); } }\n'),
      intended='L-06'),
    lc('V-L06-2',
      'limit',
      'existing companion (WalkingHudView) gets the same alias route',
      (UI + 'WalkingHudView.cs', 'ObjectiveWidget.Apply(ui.Learning.HudModel());', 'ObjectiveWidget.Apply(ui.Learning.HudModel());\n            LearningObjectivesView lv = ui.Learning; lv.Root.Query<Button>().ForEach(b => b.SetEnabled(false));'),
      intended='L-06'),
]
loop1_reviewer_benign = [
    lc('Y-N-08',
      'novel',
      '#region text with quotes and slashes',
      (UI + 'LearningObjectivesView.Sequence.cs', 'private int teachingWorldVersion = -1;', '#region it\'s "quoted" // not a comment\n    private int teachingWorldVersion = -1;\n    #endregion')),
    lc('Y-N-16',
      'novel',
      'BOM / NBSP between tokens in the header (benign whitespace)',
      (VIEW, 'buttons.Add(forest);', 'buttons.Add(forest);\xa0')),
    lc('Y-N-17',
      'novel',
      'unicode letter identifiers elsewhere (benign)',
      (UI + 'LearningObjectivesView.Sequence.cs', 'private int teachingWorldVersion = -1;', 'private int teachingWorldVersion = -1;\n    private int gr\xf6\xdfe = 1;')),
    lc('Y-B-01',
      'benign',
      'new unrelated root method querying another label',
      (ROOT, 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);', 'public Label FindTitle() => RootElement?.Q<Label>("some-title");\n\n    public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);')),
    lc('Y-B-02',
      'benign',
      'new static readonly field in the view',
      (VIEW, 'private readonly ScrollView scroll;', 'private readonly ScrollView scroll;\n    private static readonly string[] Spare = { "a", "b" };')),
    lc('Y-B-03',
      'benign',
      'long lesson text edit in Curriculum partial',
      (UI + 'LearningObjectivesView.Curriculum.cs', 'public const string PreferencePrefix = "CCF.Learning.v1.";', 'public const string PreferencePrefix = "CCF.Learning.v1.";\n    private const string ExtraNote = "Mark rack entrance trees with the spray marker; do not mark Crop Trees.";')),
    lc('Y-B-04',
      'benign',
      'extra using directive in root and view',
      (ROOT, 'using UnityEngine.InputSystem;', 'using UnityEngine.InputSystem;\nusing System.Text;'),
      (VIEW, 'using UnityEngine.UIElements;\n\n// Optional learning progress', 'using UnityEngine.UIElements;\nusing System.Text;\n\n// Optional learning progress')),
    lc('Y-B-05',
      'benign',
      'whitespace/tab and trailing-space changes inside pinned ClearNextAction',
      (ROOT, 'nextAction.RemoveFromClassList("next-action");', 'nextAction.RemoveFromClassList( "next-action" ) ;   ')),
    lc('Y-B-06',
      'benign',
      'XML doc comment with quotes before a root method',
      (ROOT, 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);', '/// <summary>Opens the "objectives" screen; don\'t cache.</summary>\n    public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);')),
    lc('Y-B-07',
      'benign',
      '[SerializeField] field in root',
      (ROOT, 'public sealed class ScenarioOneUiRoot : MonoBehaviour\n{', 'public sealed class ScenarioOneUiRoot : MonoBehaviour\n{\n    [SerializeField] private float spareDelay = 0.25f;')),
    lc('Y-B-08',
      'benign',
      'expression-bodied root property reading learning',
      (ROOT, 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);', 'public bool ObjectivesOpen => learning != null && screen == UiScreen.Objectives;\n\n    public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);')),
    lc('Y-B-13',
      'benign',
      'unrelated new MonoBehaviour that mentions UiKit',
      (UI + 'NewPanelView.cs', None, 'using UnityEngine.UIElements;\npublic sealed class NewPanelView { public NewPanelView() { var b = UiKit.Button("Go", () => { }, true, "btn-primary"); } }\n')),
    lc('Y-B-14',
      'benign',
      'pinned Refresh: comment block inserted mid-body',
      (ROOT, 'string target = null;', '/* choose a target */ string target = null; // none yet')),
    lc('Y-B-15',
      'benign',
      'header: region and extra comment lines',
      (VIEW, 'Button map = UiKit.Button("Stand map [M]", () => ui.ShowMap());', '// the stand map\n        Button map = UiKit.Button("Stand map [M]", () => ui.ShowMap());')),
    lc('Y-B-16',
      'benign',
      'allowlist extended with an extra name and reordered',
      (ROOT, '"objective-primary-cta", "objective-work-plan", "objective-back-to-forest",', '"objective-back-to-forest", "objective-primary-cta", "objective-work-plan", "objective-extra",')),
    lc('Y-B-17',
      'benign',
      'CRLF line endings in every checked file',
      transform="crlf"),
    lc('Y-B-18',
      'benign',
      'new Objectives header extra button, non-primary, new caption',
      (VIEW, 'buttons.Add(forest);', 'buttons.Add(forest);\n        Button help2 = UiKit.Button("Help [F1]", () => ui.CloseAll()); help2.name = "objective-help"; buttons.Add(help2);')),
    lc('Z-N-NBSP-ctl',
      'novel',
      'control: NBSP inside identifier is a different (undeclared) identifier -> not valid; skip',
      (VIEW, 'buttons.Add(forest);', 'buttons.Add(forest);\n        _ = 1;')),
    lc('Z-N-nested-interp-seq',
      'novel',
      'benign nested-literal interpolation in a partial (precise lexer)',
      (UI + 'LearningObjectivesView.Sequence.cs', 'private int teachingWorldVersion = -1;', 'private int teachingWorldVersion = -1;\n    private string Note() => $"{(done.Count > 0 ? "yes" : "no")} / {$"{done.Count}"}";')),
    lc('Z-B-23',
      'benign',
      'unrelated new method in root using document.rootVisualElement and SetEnabled',
      (ROOT, 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);', 'public void Freeze(bool on) { if (document != null) document.rootVisualElement.SetEnabled(!on); }\n\n    public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);')),
    lc('Z-B-24',
      'benign',
      'header extra button with enabled=false',
      (VIEW, 'buttons.Add(forest);', 'buttons.Add(forest);\n        Button soon = UiKit.Button("Soon", () => ui.CloseAll(), false); soon.name = "objective-soon"; buttons.Add(soon);')),
]
loop1_reviewer_limits = [
    lc('W-DEP-05',
      'dep',
      'Objective guidance spot: ReloadProgress marks everything done (lesson stage jumps)',
      (VIEW, 'foreach (string id in StepIds) if (PlayerPrefs.GetInt(PreferencePrefix + id, 0) == 1) done.Add(id);', 'foreach (string id in StepIds) done.Add(id);')),
    lc('Y-RR3-17',
      'RR-3',
      'ShowObjectives opens Map instead (unpinned routing; N-2 class)',
      (ROOT, 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);\n        SetScreen(UiScreen.Objectives);', 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);\n        SetScreen(UiScreen.Map);')),
    lc('Y-RR3-18',
      'RR-3',
      'CloseAll no longer closes Objectives (unpinned routing; N-2 class)',
      (ROOT, 'public void CloseAll()\n    {\n        CloseHelp();\n        manager.OpenWorkPlan(false);\n        SetScreen(UiScreen.None);', 'public void CloseAll()\n    {\n        CloseHelp();\n        manager.OpenWorkPlan(false);')),
]

L1_SHOW_OBJ = 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);'
L1_SHOW_OBJ_FULL = L1_SHOW_OBJ + '\n        SetScreen(UiScreen.Objectives);'
L1_CLOSE_ALL = 'public void CloseAll()\n    {\n        CloseHelp();'
L1_HELP_NEW = 'help = new MenuHelpView(this);'
L1_REVIEW_NEW = 'review = new AnnualReviewView(this);'
L1_MAP_NEW = 'map = new StandMapView(this);'
L1_LEARNING_NEW = 'learning = new LearningObjectivesView(this);'
L1_LEARNING_BLOCK = 'manager.RackConfirmed += OnRackConfirmed;\n        }\n        ' + L1_LEARNING_NEW
L1_HELP_FIELD = 'private MenuHelpView help;'
L1_TRY_WRITE = 'manager.OpenWorkPlan(false);\n        screen = UiScreen.Review;'
L1_NAR = ('public bool NeedsAnnualReview => manager != null && manager.AnnualReports.Count > 0 '
          '&& !manager.AnnualReviewSeen;')
L1_TEACHING = 'public bool TeachingRacks => CurrentTopicIndex == 0;'
L1_STANDMAP = UI + "StandMapView.cs"
L1_STANDMAP_FIELD = 'private readonly ScenarioOneUiRoot ui;'
L1_SEQ = UI + "LearningObjectivesView.Sequence.cs"
L1_SEQ_MEMBER = 'private int teachingWorldVersion = -1;'
L1_CUR = UI + "LearningObjectivesView.Curriculum.cs"
L1_CUR_MEMBER = 'public const string PreferencePrefix = "CCF.Learning.v1.";'
L1_RELOAD = 'public void ReloadProgress()\n    {'
L1_FOREST_ADD = 'buttons.Add(forest);'
L1_LIB_CALL = 'scroll.Add(library);\n        ui.RefreshNextActionGuidance();'
L1_SHOWN = 'shownGuidance = guidance;'
L1_PROGRESS = 'progress.text = "Learn the management process one step at a time. [O] returns to this objective.";'

loop1_s1 = [
    lc("L1-S1-01", "RR-2", "parenthesised assignment target: (help) = null in an unpinned method",
       (ROOT, L1_SHOW_OBJ, L1_SHOW_OBJ + '\n        (help) = null;'), intended="RR-2 related state"),
    lc("L1-S1-02", "RR-2", "nested tuple deconstruction writes help",
       (ROOT, L1_SHOW_OBJ, L1_SHOW_OBJ + '\n        ((help, workPlanWasOpen), nextModalRefresh) = ((null, false), 0f);'),
       intended="RR-2 related state"),
    lc("L1-S1-03", "RR-2", "tuple writes help through another receiver (GetComponent)",
       (ROOT, L1_SHOW_OBJ, L1_SHOW_OBJ + '\n        (GetComponent<ScenarioOneUiRoot>().help, workPlanWasOpen) = (null, false);'),
       intended="receiver other than `this`"),
    lc("L1-S1-04", "RR-2", "tuple writes this.help",
       (ROOT, L1_SHOW_OBJ, L1_SHOW_OBJ + '\n        (this.help, workPlanWasOpen) = (null, false);'), intended="RR-2 related state"),
    lc("L1-S1-05", "RR-2", "prefix increment moves screen off its routed value (write count unchanged)",
       (ROOT, L1_SHOW_OBJ_FULL, L1_SHOW_OBJ_FULL + '\n        ++screen;'), intended="RR-2 related state"),
    lc("L1-S1-06", "RR-2", "help field turned into a property that drops every write",
       (ROOT, L1_HELP_FIELD, 'private MenuHelpView help { get => null; set { } }'),
       intended="RR-2 related state declaration"),
    lc("L1-S1-07", "RR-2", "Start help write made conditional (text and count unchanged)",
       (ROOT, L1_HELP_NEW, 'if (ecology == null) ' + L1_HELP_NEW), intended="top-level statement of Start"),
    lc("L1-S1-08", "RR-2", "Start returns before its help write when a session menu exists",
       (ROOT, L1_HELP_NEW, 'if (FindFirstObjectByType<StandaloneSessionMenu>() != null) return;\n        ' + L1_HELP_NEW),
       intended="may not return"),
    lc("L1-S1-09", "RR-2", "Start help write deferred into a lambda that never runs",
       (ROOT, L1_HELP_NEW, 'System.Action init = () => ' + L1_HELP_NEW), intended="top-level statement of Start"),
    lc("L1-S1-10", "RR-2", "review write moved from Start to ShowObjectives (text and count unchanged)",
       (ROOT, L1_REVIEW_NEW + '\n', ''), (ROOT, L1_SHOW_OBJ, L1_SHOW_OBJ + '\n        ' + L1_REVIEW_NEW),
       intended="top-level statement of Start"),
    lc("L1-S1-11", "RR-2", "learning write moved inside Start's manager != null block",
       (ROOT, L1_LEARNING_BLOCK, 'manager.RackConfirmed += OnRackConfirmed;\n            ' + L1_LEARNING_NEW + '\n        }'),
       intended="top-level statement of Start"),
    lc("L1-S1-12", "RR-2", "TryShowReviewYear returns before its screen write",
       (ROOT, L1_TRY_WRITE, 'manager.OpenWorkPlan(false);\n        if (year > 0) return true;\n        screen = UiScreen.Review;'),
       intended="RR-2 related state 'screen'"),
    lc("L1-S1-13", "RR-2", "help nulled through __makeref/__refvalue (no assignment to the name)",
       (ROOT, L1_SHOW_OBJ, L1_SHOW_OBJ + '\n        System.TypedReference slot = __makeref(help); __refvalue(slot, MenuHelpView) = null;'),
       intended="__makeref"),
    lc("L1-S1-14", "RR-2", "screen reset by a ref-this extension method declared in a new file",
       (UI + "ScreenReset.cs", None, 'public static class ScreenReset\n{\n    public static void Reset(ref this ScenarioOneUiRoot.UiScreen s) '
        '=> s = ScenarioOneUiRoot.UiScreen.None;\n}\n'),
       (ROOT, L1_SHOW_OBJ_FULL, L1_SHOW_OBJ_FULL + '\n        screen.Reset();'), intended="'screen' is used as a member receiver"),
    lc("L1-S1-15", "RR-2", "screen passed whole to an in-parameter helper that writes it through UnsafeUtilityExtensions.AsRef",
       (UI + "ScreenWriter.cs", None, 'public static class ScreenWriter\n{\n    public static void Clear(in ScenarioOneUiRoot.UiScreen s) '
        '=> Unity.Collections.LowLevel.Unsafe.UnsafeUtilityExtensions.AsRef(s) = ScenarioOneUiRoot.UiScreen.None;\n}\n'),
       (ROOT, L1_SHOW_OBJ_FULL, L1_SHOW_OBJ_FULL + '\n        ScreenWriter.Clear(screen);'), intended="passed whole as an argument"),
    lc("L1-S1-16", "RR-2", "Start help write compiled out of Player builds by #if",
       (ROOT, L1_HELP_NEW, '\n#if UNITY_EDITOR\n        ' + L1_HELP_NEW + '\n#endif'), intended="conditional compilation"),
]

loop1_s2 = [
    lc("L1-S2-01", "N-4", "form-feed-indented #if/#endif compile the learning write out",
       (ROOT, L1_LEARNING_NEW, '\n\x0c#if CCF_NEVER_DEFINED\n        ' + L1_LEARNING_NEW + '\n\x0c#endif'), intended="(N-4)"),
    lc("L1-S2-02", "N-4", "no-break space after '#' in #if/#endif around the help write",
       (ROOT, L1_HELP_NEW, '\n#\xa0if CCF_NEVER_DEFINED\n        ' + L1_HELP_NEW + '\n#\xa0endif'), intended="(N-4)"),
    lc("L1-S2-04", "N-4", "no-break-space-indented directives inside an inactive verbatim string make hidden code active",
       (L1_SEQ, L1_SEQ_MEMBER, L1_SEQ_MEMBER + '\n#if CCF_NEVER_DEFINED\n    private string hidden = @"\n\xa0#endif\n'
        '    private void Sneak() { Root.SetEnabled(false); }\n\xa0#if CCF_NEVER_DEFINED\n    ";\n#endif'), intended="(N-4)"),
    lc("L1-S2-05", "N-4", "BOM character (U+FEFF) inside an identifier mid-file",
       (VIEW, L1_FOREST_ADD, L1_FOREST_ADD + '\n        fo\ufeffrest.name += "-x";'), intended="(N-4)"),
    lc("L1-S2-06", "N-4", "vertical-tab-indented #if/#endif around the Objectives caller",
       (VIEW, L1_LIB_CALL, 'scroll.Add(library);\n\x0b#if CCF_NEVER_DEFINED\n        ui.RefreshNextActionGuidance();\n\x0b#endif'),
       intended="(N-4)"),
]

loop1_s3 = [
    lc("L1-S3-01", "RR-3", "NeedsAnnualReview keeps its reviewed text only in the editor; #else forces it",
       (ROOT, L1_NAR, '\n#if UNITY_EDITOR\n    ' + L1_NAR + '\n#else\n    public bool NeedsAnnualReview => true;\n#endif'),
       intended="conditional compilation (#if)"),
    lc("L1-S3-02", "RR-3", "TeachingRacks widened to the first two topics",
       (VIEW, L1_TEACHING, 'public bool TeachingRacks => CurrentTopicIndex <= 1;'), intended="RR-3 routing input"),
]

loop1_l06 = [
    lc("L1-L06-01", "RR-1", "a companion passes ui.Learning to a helper that disables the Objectives Root",
       (L1_STANDMAP, L1_STANDMAP_FIELD, L1_STANDMAP_FIELD + '\n    private static void Keep(LearningObjectivesView v) => '
        'v.Root.SetEnabled(false);\n    private void Sneak() => Keep(ui.Learning);'), intended="L-06"),
    lc("L1-L06-02", "RR-1", "a companion pattern-matches ui.Learning, then disables its Root",
       (L1_STANDMAP, L1_STANDMAP_FIELD, L1_STANDMAP_FIELD + '\n    private void Sneak() { if (ui.Learning is '
        'LearningObjectivesView v) v.Root.SetEnabled(false); }'), intended="L-06"),
    lc("L1-L06-03", "RR-1", "the #309 L-06 probe, now closed: a companion aliases ui.Learning before reaching Root",
       (L1_STANDMAP, L1_STANDMAP_FIELD, L1_STANDMAP_FIELD + '\n    private void Sneak() { var view = ui.Learning; '
        'view.Root.SetEnabled(false); }'), intended="L-06"),
]

loop1_benign = [
    lc("L1-B-01", "benign", "unrelated statement added between Start's reviewed writes",
       (ROOT, L1_MAP_NEW, L1_MAP_NEW + '\n        Debug.Log("Scenario One UI views created");')),
    lc("L1-B-02", "benign", "new root property reading screen",
       (ROOT, L1_SHOW_OBJ, 'public bool OnMap => screen == UiScreen.Map;\n\n    ' + L1_SHOW_OBJ)),
    lc("L1-B-03", "benign", "this.manager read and manager passed as an argument in an unpinned method",
       (ROOT, L1_SHOW_OBJ, L1_SHOW_OBJ + '\n        if (this.manager != null) Debug.Log(manager);')),
    lc("L1-B-04", "benign", "companion reads ui.Learning through a null test and a member access",
       (L1_STANDMAP, L1_STANDMAP_FIELD, L1_STANDMAP_FIELD + '\n    private bool LessonActive => ui.Learning != null '
        '&& ui.Learning.TeachingRacks;')),
    lc("L1-B-05", "benign", "format characters and a no-break space inside string literals (view, partial, root)",
       (VIEW, L1_RELOAD, L1_RELOAD + '\n        _ = "soft\xadhyphen\xa0joined\u200dtext";'),
       (L1_CUR, L1_CUR_MEMBER, L1_CUR_MEMBER + '\n    private const string Joined = "a\u200db\ufeffc";'),
       (ROOT, L1_SHOW_OBJ, L1_SHOW_OBJ + '\n        Debug.Log("zero\u200bwidth");')),
    lc("L1-B-06", "benign", "combining mark inside a new identifier in a partial",
       (L1_SEQ, L1_SEQ_MEMBER, L1_SEQ_MEMBER + '\n    private int cafe\u0301 = 1;')),
    lc("L1-B-07", "benign", "tab-indented #if/#endif in an unpinned root method",
       (ROOT, L1_SHOW_OBJ, L1_SHOW_OBJ + '\n\t#if UNITY_EDITOR\n\t\tDebug.Log("objectives");\n\t#endif')),
    lc("L1-B-08", "benign", "switch on screen in a new root method",
       (ROOT, L1_SHOW_OBJ, 'public string ScreenName() { switch (screen) { case UiScreen.Map: return "map"; '
        'default: return "other"; } }\n\n    ' + L1_SHOW_OBJ)),
    lc("L1-B-09", "benign", "tuple deconstruction of unrelated root fields",
       (ROOT, L1_SHOW_OBJ, L1_SHOW_OBJ + '\n        (workPlanWasOpen, nextModalRefresh) = (false, 0f);')),
    lc("L1-B-10", "benign", "no-break space between tokens of a reviewed Start write",
       (ROOT, L1_HELP_NEW, 'help\xa0= new MenuHelpView(this);')),
    lc("L1-B-11", "benign", "em dash and a format character in a #region name in a partial",
       (L1_SEQ, L1_SEQ_MEMBER, '#region Racks \u2014 V2\u200d\n    ' + L1_SEQ_MEMBER + '\n    #endregion')),
]

# Accepted limits: defective, compilable edits this checker does NOT reject (README "Known limits").
loop1_limits = loop1_reviewer_limits + [
    lc("L-01", "limit", "a class outside the companion set disables the Work Plan button by a constructed name",
       ("Assets/ForestPrototype/CtaProbeL01.cs", None, 'internal static class CtaProbeL01\n{\n    internal static void Hide()\n    {\n'
        '        var document = UnityEngine.Object.FindAnyObjectByType<UnityEngine.UIElements.UIDocument>();\n'
        '        if (document != null) UnityEngine.UIElements.UQueryExtensions.Q<UnityEngine.UIElements.Button>('
        'document.rootVisualElement, "objective-" + "work-plan")?.SetEnabled(false);\n    }\n}\n')),
    lc("L-02", "limit", "root disables every button of its own document from an unpinned method (no class change)",
       (ROOT, L1_CLOSE_ALL, L1_CLOSE_ALL + '\n        document.rootVisualElement.Query<Button>().ForEach(b => b.SetEnabled(false));')),
    lc("L-03", "limit", "non-terminating loop before the Objectives caller", (VIEW, L1_SHOWN, 'while (force) { }\n        ' + L1_SHOWN)),
    lc("L-04", "limit", "reflection writes guarded state by a string name",
       (ROOT, L1_CLOSE_ALL, L1_CLOSE_ALL + '\n        typeof(ScenarioOneUiRoot).GetField("guidanceCandidate", '
        'System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(this, null);')),
    lc("L-07", "limit", "exception thrown by an expression before the Objectives caller",
       (VIEW, L1_SHOWN, 'int unused = int.Parse(guidance);\n        ' + L1_SHOWN)),
    lc("L-08", "limit", "exception thrown by an expression in Start before its help write",
       (ROOT, L1_HELP_NEW, 'int unused = int.Parse(gameObject.name);\n        ' + L1_HELP_NEW)),
]

# Documented over-rejections: benign edits that fail closed until the checker is updated deliberately.
loop1_over = [
    lc("O-01", "over", "literal nested in an interpolation hole in LearningObjectivesView.cs (legacy-lexer files)",
       (VIEW, L1_PROGRESS, 'progress.text = $"Step {(force ? "a" : "b")}";')),
    lc("O-02", "over", "apostrophes in two preprocessor lines of ScenarioOneUiRoot.cs (legacy regex pairs them)",
       (ROOT, 'public void ShowObjectives()', "#region Don't panic\n    public void ShowObjectives()"),
       (ROOT, L1_CLOSE_ALL, "#endregion // it's done\n    " + L1_CLOSE_ALL)),
    lc("O-03", "over", "spaces added inside a pinned interpolation hole", (ROOT, '{UiKit.Money(cost)}', '{ UiKit.Money(cost) }')),
    lc("O-04", "over", "null-conditional call of the Objectives guidance refresh",
       (VIEW, L1_LIB_CALL, L1_LIB_CALL.replace('ui.Refresh', 'ui?.Refresh'))),
    lc("O-05", "over", "#if/#endif logging inside Start, which now holds reviewed writes",
       (ROOT, L1_MAP_NEW, L1_MAP_NEW + '\n#if UNITY_EDITOR\n        Debug.Log("views created");\n#endif')),
    lc("O-06", "over", "U+2028 inside a verbatim string (valid C#; Unicode line separators fail closed anywhere)",
       (L1_CUR, L1_CUR_MEMBER, L1_CUR_MEMBER + '\n    private const string Para = @"one\u2028two";')),
    lc("O-07", "over", "a local named like a related-state field in an unrelated root method",
       (ROOT, L1_SHOW_OBJ, L1_SHOW_OBJ + '\n        int review = 0; Debug.Log(review);')),
    lc("O-08", "over", "a companion tests null on the left: null != ui.Learning",
       (L1_STANDMAP, L1_STANDMAP_FIELD, L1_STANDMAP_FIELD + '\n    private bool HasLessons => null != ui.Learning;')),
    lc("O-09", "over", "screen passed whole to a method in an unpinned root method (an in-parameter could write it)",
       (ROOT, L1_SHOW_OBJ_FULL, L1_SHOW_OBJ_FULL + '\n        Debug.Log(screen);')),
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


def run_loop1(view, root, companions):
    """#309 repair loop 1 groups. Returns the counts that run() folds into its totals."""
    base = dict(companions)
    base.update({VIEW: view, ROOT: root})

    def build(case):
        files = dict(base)
        for path, old, new in case["edits"]:
            if old is None:  # a new, in-memory-only file
                if path in files or (REPO / path).exists():
                    raise RuntimeError(f"{case['id']}: {path} already exists")
                files[path] = new
            else:
                if path not in files:
                    files[path] = (REPO / path).read_text(encoding="utf-8")
                files[path] = replace_one(files[path], old, new, case["id"])
        if case["transform"] == "crlf":
            files = {path: text.replace("\n", "\r\n") for path, text in files.items()}
        return files

    def outcome(case):
        """None when accepted, else the rejection reason. As in check_baseline.py, a file other than the
        view and the root reaches the scanner only if is_companion_source() selects it."""
        files = build(case)
        others = {path: text for path, text in files.items() if path not in (VIEW, ROOT) and is_companion_source(text)}
        try:
            verify_objective_cta(files[VIEW], files[ROOT], others)
        except ObjectiveCtaSourceError as exc:
            return str(exc)
        return None

    counts = dict(total=0, rejected=0, wrong_cause=0, benign=0, benign_failed=0, probe_failed=0)
    for group, cases in (("#314 reviewer S-1", loop1_reviewer_s1), ("#314 reviewer S-2", loop1_reviewer_s2),
                         ("#314 reviewer S-3", loop1_reviewer_s3), ("#314 reviewer L-06", loop1_reviewer_l06),
                         ("#309 loop-1 S-1", loop1_s1), ("#309 loop-1 S-2", loop1_s2),
                         ("#309 loop-1 S-3", loop1_s3), ("#309 loop-1 L-06", loop1_l06)):
        group_rejected = 0
        for case in cases:
            reason = outcome(case)
            if reason is None:
                print(f"SOURCE-STATIC FAIL mutation-{case['id']}: FALSE GREEN")
            elif case["intended"] in reason:
                group_rejected += 1
                print(f"SOURCE-STATIC PASS mutation-{case['id']}: rejected ({reason})")
            else:
                counts["wrong_cause"] += 1
                print(f"SOURCE-STATIC FAIL mutation-{case['id']}: rejected for an unexpected cause ({reason}); "
                      f"expected {case['intended']!r}")
        print(f"SOURCE-STATIC CTA MUTATION GROUP {group}: rejected={group_rejected}/{len(cases)}")
        counts["total"] += len(cases)
        counts["rejected"] += group_rejected
    for case in loop1_reviewer_benign + loop1_benign:
        reason = outcome(case)
        counts["benign"] += 1
        if reason is None:
            print(f"SOURCE-STATIC PASS benign-{case['id']}: accepted")
        else:
            counts["benign_failed"] += 1
            print(f"SOURCE-STATIC FAIL benign-{case['id']}: WRONGLY REJECTED ({reason})")
    print(f"SOURCE-STATIC CTA LOOP-1 BENIGN CONTROLS accepted={counts['benign'] - counts['benign_failed']}/{counts['benign']}")
    # Not mutations and not passes: each probe must keep its documented outcome, or README is out of date.
    for case, documented in [(case, "accepted") for case in loop1_limits] + [(case, "rejected") for case in loop1_over]:
        reason = outcome(case)
        observed = "accepted" if reason is None else "rejected"
        kind = "accepted-limit" if documented == "accepted" else "over-rejection"
        if observed == documented:
            print(f"SOURCE-STATIC INFO {kind}-{case['id']}: {observed} as documented ({case['why']})"
                  + (f" [{reason}]" if reason else ""))
        else:
            counts["probe_failed"] += 1
            print(f"SOURCE-STATIC FAIL {kind}-{case['id']}: now {observed}, documented {documented}; update the "
                  f"README known limits ({reason or 'accepted'})")
    print(f"SOURCE-STATIC CTA LOOP-1 PROBES limits={len(loop1_limits)} over-rejections={len(loop1_over)} "
          f"changed={counts['probe_failed']} (reported, not counted as mutations)")
    return counts



# =====================================================================================================
# #320 two-guard closure (N-1 interpolation holes, N-2 owning class). First the #314 reviewer's case
# modules my_mutations*.py, all 11 rounds: rounds 1-6 and 8-11 transcribed with ascii() (each source
# file's sha256 is noted; evidence proves every transcribed case identical to the original), round 7 as
# its generator. `expect` is the reviewer's: rej must be rejected, ben accepted; lim and unk probes, the
# reviewer's 4 L-06 over-rejections and the out-of-scope W-DEP-05 are reported with their documented
# outcome, not counted. The 47 N-1 and 10 N-2 cases must be rejected for that cause. Then this repair's
# own variants, benign edits and probes. A counted rejection may never be a scanner crash.
# =====================================================================================================
CRASH = "scanner could not analyse"
N1 = "inside an interpolation hole (N-1)"
N2 = "(N-2"


def r314(mid, round_name, expect, why, edits, intended=None, documented=None, transform=None):
    return dict(id=mid, group=round_name, expect=expect, why=why, edits=list(edits), intended=intended,
                documented=documented, transform=transform)


r314_rounds = [
    # my_mutations.py sha256 cba96390832b1bc76dc91c4bb9e0d5960f007e2f1cc84441917724f72b950584
    r314('Y-RR1-01', 'my_mutations', 'rej', 'forest.name rewritten from itself (forest.name = forest.name + suffix)', [(VIEW, 'buttons.Add(forest);', 'buttons.Add(forest);\n        forest.name = forest.name + "-x";')]),
    r314('Y-RR1-02', 'my_mutations', 'rej', 'forest name built by Concat (statement not a plain literal binding)', [(VIEW, 'forest.name = "objective-back-to-forest";', 'forest.name = string.Concat("objective-", "back-to-forest");')]),
    r314('Y-RR1-03', 'my_mutations', 'rej', 'names set through a tuple deconstruction', [(VIEW, 'buttons.Add(forest);', 'buttons.Add(forest);\n        (forest.name, work.name) = ("objective-x", "objective-y");')]),
    r314('Y-RR1-04', 'my_mutations', 'rej', 'unreviewed inline new Button appended to the header row', [(VIEW, 'buttons.Add(forest);', 'buttons.Add(forest);\n        buttons.Add(new Button(() => ui.CloseAll()) { text = "Hidden" });')]),
    r314('Y-RR1-05', 'my_mutations', 'rej', 'Back to forest disabled through the enabled argument', [(VIEW, 'Button forest = UiKit.Button("Back to forest [O / Esc]", () => ui.CloseAll(), true, "btn-primary");', 'Button forest = UiKit.Button("Back to forest [O / Esc]", () => ui.CloseAll(), false, "btn-primary");')]),
    r314('Y-RR1-06', 'my_mutations', 'rej', 'map made static primary via a const class argument', [(VIEW, 'Button map = UiKit.Button("Stand map [M]", () => ui.ShowMap());', 'Button map = UiKit.Button("Stand map [M]", () => ui.ShowMap(), true, PrimaryName);'), (VIEW, 'private readonly ScrollView scroll;', 'private readonly ScrollView scroll;\n    private const string PrimaryName = "btn-primary";')]),
    r314('Y-RR1-07', 'my_mutations', 'rej', 'second static primary in the Curriculum partial', [(L1_CUR, 'public const string PreferencePrefix = "CCF.Learning.v1.";', 'public const string PreferencePrefix = "CCF.Learning.v1.";\n    private Button Extra() => UiKit.Button("x", () => ui.CloseAll(), true, "btn-primary");')]),
    r314('Y-RR1-08', 'my_mutations', 'rej', 'primary class through UiKit.Box in a partial', [(L1_CUR, 'public const string PreferencePrefix = "CCF.Learning.v1.";', 'public const string PreferencePrefix = "CCF.Learning.v1.";\n    private VisualElement Extra() => UiKit.Box("btn-primary");')]),
    r314('Y-RR1-09', 'my_mutations', 'rej', 'primary class through UiKit.Add in a partial', [(L1_CUR, 'public const string PreferencePrefix = "CCF.Learning.v1.";', 'public const string PreferencePrefix = "CCF.Learning.v1.";\n    private Label Extra(VisualElement p) => UiKit.Add(p, "t", "btn-primary");')]),
    r314('Y-RR1-10', 'my_mutations', 'rej', 'a second Back to forest primary placed in the scroll area', [(VIEW, 'scroll.Add(library);\n        ui.RefreshNextActionGuidance();', 'scroll.Add(UiKit.Button("Back to forest [O / Esc]", () => ui.CloseAll(), true, "btn-primary"));\n        scroll.Add(library);\n        ui.RefreshNextActionGuidance();')]),
    r314('Y-RR1-11', 'my_mutations', 'rej', 'verbatim spelling of the class on Work Plan', [(VIEW, 'Button work = UiKit.Button("Work Plan [Tab]", () => ui.ShowWorkPlan());', 'Button work = UiKit.Button("Work Plan [Tab]", () => ui.ShowWorkPlan(), true, @"btn-primary");')]),
    r314('Y-RR1-12', 'my_mutations', 'rej', 'escaped spelling of the class on Work Plan', [(VIEW, 'Button work = UiKit.Button("Work Plan [Tab]", () => ui.ShowWorkPlan());', 'Button work = UiKit.Button("Work Plan [Tab]", () => ui.ShowWorkPlan(), true, "btn\\u002dprimary");')]),
    r314('Y-RR1-13', 'my_mutations', 'rej', 'interpolated class on Work Plan', [(VIEW, 'Button work = UiKit.Button("Work Plan [Tab]", () => ui.ShowWorkPlan());', 'Button work = UiKit.Button("Work Plan [Tab]", () => ui.ShowWorkPlan(), true, $"btn-{"primary"}");')]),
    r314('Y-RR1-14', 'my_mutations', 'rej', 'allowlist copied with Clone and edited', [(ROOT, 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);', 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);\n        var names = (string[])GuidanceButtonNames.Clone(); names[2] = "x";')]),
    r314('Y-RR1-15', 'my_mutations', 'rej', 'allowlist aliased by a second static field', [(ROOT, 'private static readonly string[] GuidanceButtonNames = {', 'private static readonly string[] Alias = GuidanceButtonNames;\n    private static readonly string[] GuidanceButtonNames = {')]),
    r314('Y-RR1-16', 'my_mutations', 'rej', 'allowlist sorted in place at runtime', [(ROOT, 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);', 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);\n        System.Array.Sort(GuidanceButtonNames);')]),
    r314('Y-RR1-17', 'my_mutations', 'rej', 'Back to forest retargeted to ShowObjectives (still primary)', [(VIEW, 'Button forest = UiKit.Button("Back to forest [O / Esc]", () => ui.CloseAll(), true, "btn-primary");', 'Button forest = UiKit.Button("Back to forest [O / Esc]", () => ui.ShowObjectives(), true, "btn-primary");')]),
    r314('Y-RR1-18', 'my_mutations', 'rej', 'header statement hidden behind #if', [(VIEW, 'buttons.Add(forest);', 'buttons.Add(forest);\n#if UNITY_EDITOR\n        forest.name = "objective-x";\n#endif')]),
    r314('Y-RR1-19', 'my_mutations', 'rej', 'forest aliased by var', [(VIEW, 'buttons.Add(forest);', 'buttons.Add(forest);\n        var f2 = forest; f2.name = "objective-x";')]),
    r314('Y-RR1-20', 'my_mutations', 'rej', 'local function capturing forest', [(VIEW, 'buttons.Add(forest);', 'buttons.Add(forest);\n        void Hide() => forest.SetEnabled(false);\n        Hide();')]),
    r314('Y-RR1-21', 'my_mutations', 'rej', 'forest callback registered', [(VIEW, 'buttons.Add(forest);', 'buttons.Add(forest);\n        forest.RegisterCallback<ClickEvent>(e => forest.SetEnabled(false));')]),
    r314('Y-RR1-22', 'my_mutations', 'rej', 'this.Root used in a method', [(VIEW, 'public void ReloadProgress()\n    {', 'public void ReloadProgress()\n    {\n        this.Root.SetEnabled(false);')]),
    r314('Y-RR1-23', 'my_mutations', 'rej', 'Root restyled inside the constructor', [(VIEW, 'modal.Add(header);', 'modal.Add(header);\n        Root.style.display = DisplayStyle.None;')]),
    r314('Y-RR1-24', 'my_mutations', 'rej', 'second constructor overload', [(VIEW, 'private readonly ScrollView scroll;', 'private readonly ScrollView scroll;\n    public LearningObjectivesView() : this((ScenarioOneUiRoot)null) { }')]),
    r314('Y-RR1-25', 'my_mutations', 'rej', 'static constructor', [(VIEW, 'private readonly ScrollView scroll;', 'private readonly ScrollView scroll;\n    static LearningObjectivesView() { }')]),
    r314('Y-RR1-26', 'my_mutations', 'rej', 'a second class in the view file tints a button', [(VIEW, 'public sealed partial class LearningObjectivesView\n{', 'internal static class Tint { public static void Go(Button b) { b.AddToClassList("btn-primary"); } }\npublic sealed partial class LearningObjectivesView\n{')]),
    r314('Y-RR1-27', 'my_mutations', 'rej', 'UiKit overload with default argument', [(UIKIT, 'public static VisualElement Row(VisualElement parent, params string[] classes)', 'public static Button Button(string text, Action onClick, bool enabled = true) => Button(text, onClick, enabled, "btn-" + "primary");\n\n    public static VisualElement Row(VisualElement parent, params string[] classes)')]),
    r314('Y-RR1-28', 'my_mutations', 'rej', 'Stand map CTA primary via UiKit class param in the ctor (R02 variant, enabled false)', [(VIEW, 'Button map = UiKit.Button("Stand map [M]", () => ui.ShowMap());', 'Button map = UiKit.Button("Stand map [M]", () => ui.ShowMap(), false, "btn-primary");')]),
    r314('Y-RR1-29', 'my_mutations', 'rej', 'Work Plan renamed to a name outside the allowlist', [(VIEW, 'work.name = "objective-work-plan";', 'work.name = "objective-work";')]),
    r314('Y-RR1-30', 'my_mutations', 'rej', 'extra header button given the Back to forest name', [(VIEW, 'buttons.Add(forest);', 'buttons.Add(forest);\n        Button spare = UiKit.Button("Spare", () => ui.CloseAll()); spare.name = "objective-back-to-forest"; buttons.Add(spare);')]),
    r314('Y-RR1-31', 'my_mutations', 'rej', 'header button class supplied through params array', [(VIEW, 'Button work = UiKit.Button("Work Plan [Tab]", () => ui.ShowWorkPlan());', 'Button work = UiKit.Button("Work Plan [Tab]", () => ui.ShowWorkPlan(), true, new[] { "btn-primary" });')]),
    r314('Y-RR1-32', 'my_mutations', 'rej', 'map CTA primary via named argument', [(VIEW, 'Button map = UiKit.Button("Stand map [M]", () => ui.ShowMap());', 'Button map = UiKit.Button("Stand map [M]", () => ui.ShowMap(), classes: "btn-primary");')]),
    r314('Y-RR1-33', 'my_mutations', 'rej', 'nameof(forest) used after attach', [(VIEW, 'buttons.Add(forest);', 'buttons.Add(forest);\n        _ = nameof(forest);')]),
    r314('Y-RR1-34', 'my_mutations', 'rej', 'foreach over the row children in the ctor', [(VIEW, 'modal.Add(header);', 'modal.Add(header);\n        foreach (var child in buttons.Children()) child.SetEnabled(false);')]),
    r314('Y-RR1-35', 'my_mutations', 'rej', 'ui field reassigned through this', [(VIEW, 'modal.Add(header);', 'modal.Add(header);\n        this.ui = null;')]),
    r314('Y-RR1-36', 'my_mutations', 'rej', 'map CTA primary restored from another view-class method via ui.RootElement', [(VIEW, 'public void ReloadProgress()\n    {', 'public void ReloadProgress()\n    {\n        ui.RootElement.Q<Button>("objective-" + "primary-cta").AddToClassList("btn-" + "primary");')]),
    r314('Y-RR2-01', 'my_mutations', 'rej', 'this.guidanceScope = null in ShowObjectives', [(ROOT, 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);', 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);\n        this.guidanceScope = null;')]),
    r314('Y-RR2-02', 'my_mutations', 'rej', 'guidanceCandidate property wrapper', [(ROOT, 'private static readonly string[] GuidanceButtonNames = {', 'private Button Cand { get => guidanceCandidate; set => guidanceCandidate = value; }\n    private static readonly string[] GuidanceButtonNames = {')]),
    r314('Y-RR2-03', 'my_mutations', 'rej', 'Interlocked.Exchange(ref this.guidanceCandidate)', [(ROOT, 'public void CloseAll()\n    {\n        CloseHelp();', 'public void CloseAll()\n    {\n        CloseHelp();\n        System.Threading.Interlocked.Exchange(ref this.guidanceCandidate, null);')]),
    r314('Y-RR2-04', 'my_mutations', 'rej', 'helpClosedFrame += 1 outside CloseHelp', [(ROOT, 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);', 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);\n        helpClosedFrame += 1;')]),
    r314('Y-RR2-05', 'my_mutations', 'rej', 'helpClosedFrame++ outside CloseHelp', [(ROOT, 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);', 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);\n        helpClosedFrame++;')]),
    r314('Y-RR2-06', 'my_mutations', 'rej', 'dead first guard: || true', [(ROOT, 'if (learning == null || map == null || workPlan == null || manager == null) return;', 'if (learning == null || map == null || workPlan == null || manager == null || true) return;')]),
    r314('Y-RR2-07', 'my_mutations', 'rej', 'ClearNextAction also nulls the candidate', [(ROOT, 'nextBadge.RemoveFromHierarchy();', 'nextBadge.RemoveFromHierarchy();\n        guidanceCandidate = null;')]),
    r314('Y-RR2-08', 'my_mutations', 'rej', 'screen retargeted inside TryShowReviewYear (count unchanged)', [(ROOT, 'screen = UiScreen.Review;\n        CloseHelp();', 'screen = UiScreen.None;\n        CloseHelp();')]),
    r314('Y-RR2-09', 'my_mutations', 'rej', 'tuple swap of guidance fields in an unpinned method', [(ROOT, 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);', 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);\n        (guidanceCandidate, nextAction) = (nextAction, guidanceCandidate);')]),
    r314('Y-RR2-10', 'my_mutations', 'rej', 'guidanceScope declared again as a local in another method (shadow)', [(ROOT, 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);', 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);\n        VisualElement guidanceScope = null; _ = guidanceScope;')]),
    r314('Y-RR2-11', 'my_mutations', 'rej', 'guidancePrimaries replaced by reflection-free new dictionary assignment', [(ROOT, 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);', 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);\n        guidancePrimaries.Clear();\n        guidancePrimaries.Add(null, false);')]),
    r314('Y-RR2-12', 'my_mutations', 'rej', 'ref local aliasing helpClosedFrame', [(ROOT, 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);', 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);\n        ref int hf = ref helpClosedFrame; hf = 0;')]),
    r314('Y-RR2-13', 'my_mutations', 'rej', 'out-style aliasing via local function writing helpClosedFrame', [(ROOT, 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);', 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);\n        void Reset() { helpClosedFrame = -1; }\n        Reset();')]),
    r314('Y-RR2-14', 'my_mutations', 'rej', 'nameof used as string key for guarded state (reflection prep)', [(ROOT, 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);', 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);\n        _ = nameof(guidanceCandidate);')]),
    r314('Y-RR2-15', 'my_mutations', 'rej', 'target literal swapped between two pinned write sites (count unchanged)', [(ROOT, 'target = objectives ? "objective-work-plan" : onMap ? "map-work-plan" : "workplan-approve";', 'target = objectives ? "objective-back-to-forest" : onMap ? "map-work-plan" : "workplan-approve";')]),
    r314('Y-RR2-16', 'my_mutations', 'rej', 'scope condition flipped (screen == Objectives ? map.Root : learning.Root)', [(ROOT, 'screen == UiScreen.Objectives ? learning.Root : screen == UiScreen.Map ? map.Root', 'screen == UiScreen.Objectives ? map.Root : screen == UiScreen.Map ? learning.Root')]),
    r314('Y-RR2-17', 'my_mutations', 'rej', 'ApplyNextActionGuidance suspended flag forced', [(ROOT, 'bool suspended = manager == null ||', 'bool suspended = true || manager == null ||')]),
    r314('Y-RR3-01', 'my_mutations', 'rej', '[Conditional] on ClearNextAction', [(ROOT, 'private void ClearNextAction()', '[System.Diagnostics.Conditional("UNITY_EDITOR")]\n    private void ClearNextAction()')]),
    r314('Y-RR3-02', 'my_mutations', 'rej', '[Conditional] on GuidanceVisible-like new helper called from Apply? (unpinned attr on LateUpdate)', [(ROOT, 'private void LateUpdate()', '[System.Diagnostics.Conditional("UNITY_EDITOR")]\n    private void LateUpdate()')]),
    r314('Y-RR3-03', 'my_mutations', 'rej', 'local function shadows ApplyNextActionGuidance in Start', [(ROOT, 'private void Start()\n    {', 'private void Start()\n    {\n        void ApplyNextActionGuidance() { }')]),
    r314('Y-RR3-04', 'my_mutations', 'rej', 'late removal of the CTA primary after Apply in LateUpdate', [(ROOT, 'ApplyNextActionGuidance();\n        if (competitors == null || manager == null) return;', 'ApplyNextActionGuidance();\n        nextAction?.RemoveFromClassList("btn-primary");\n        if (competitors == null || manager == null) return;')]),
    r314('Y-RR3-05', 'my_mutations', 'rej', 'row cleared at the end of the ctor', [(VIEW, 'modal.Add(header);', 'modal.Add(header);\n        buttons.Clear();')]),
    r314('Y-RR3-06', 'my_mutations', 'rej', 'row disabled', [(VIEW, 'modal.Add(header);', 'modal.Add(header);\n        buttons.SetEnabled(false);')]),
    r314('Y-RR3-07', 'my_mutations', 'rej', 'row indexer rename', [(VIEW, 'modal.Add(header);', 'modal.Add(header);\n        buttons[0].name = "objective-x";')]),
    r314('Y-RR3-08', 'my_mutations', 'rej', 'caller moved to BuildTopic', [(VIEW, 'ui.RefreshNextActionGuidance();', ''), (VIEW, 'UiKit.Add(parent, topic.Steps[0].Id == "clear.read"', 'ui.RefreshNextActionGuidance();\n        UiKit.Add(parent, topic.Steps[0].Id == "clear.read"')]),
    r314('Y-RR3-09', 'my_mutations', 'rej', 'caller inside zero-iteration foreach', [(VIEW, 'ui.RefreshNextActionGuidance();', 'foreach (var skipped in System.Array.Empty<int>()) ui.RefreshNextActionGuidance();')]),
    r314('Y-RR3-10', 'my_mutations', 'rej', 'caller inside while(false)-like if on a constant', [(VIEW, 'ui.RefreshNextActionGuidance();', 'if (position < -1f) ui.RefreshNextActionGuidance();')]),
    r314('Y-RR3-11', 'my_mutations', 'lim', 'caller preceded by infinite loop (disclosed L-03 class)', [(VIEW, 'scroll.Add(library);\n        ui.RefreshNextActionGuidance();', 'scroll.Add(library);\n        for (;;) { if (force) break; }\n        ui.RefreshNextActionGuidance();')], documented='rejected'),
    r314('Y-RR3-12', 'my_mutations', 'rej', 'duplicate Refresh(bool) under #if', [(VIEW, '    public void Refresh(bool force)\n    {', '#if UNITY_EDITOR\n    public void Refresh(bool force) { }\n#else\n    public void Refresh(bool force)\n    {'), (VIEW, 'scroll.schedule.Execute(() => scroll.scrollOffset = new Vector2(0, position));\n    }', 'scroll.schedule.Execute(() => scroll.scrollOffset = new Vector2(0, position));\n    }\n#endif')]),
    r314('Y-RR3-13', 'my_mutations', 'rej', 'view Refresh early return before caller via expression', [(VIEW, 'float position = scroll.scrollOffset.y;', 'if (scroll == null) return;\n        float position = scroll.scrollOffset.y;')]),
    r314('Y-RR3-14', 'my_mutations', 'rej', 'SetScreen no longer refreshes Objectives (pinned)', [(ROOT, 'else if (screen == UiScreen.Objectives)\n            learning.Refresh(true);', 'else if (screen == UiScreen.Objectives)\n            learning.Refresh(false);')]),
    r314('Y-RR3-15', 'my_mutations', 'rej', 'LateUpdate apply wrapped in try/catch swallowing nothing but moved after return', [(ROOT, 'ApplyNextActionGuidance();\n        if (competitors == null || manager == null) return;', 'if (competitors == null || manager == null) return;\n        ApplyNextActionGuidance();')]),
    r314('Y-RR3-16', 'my_mutations', 'rej', 'ShowMap pinned: SetScreen(None)', [(ROOT, 'manager.OpenWorkPlan(false);\n        SetScreen(UiScreen.Map);\n    }', 'manager.OpenWorkPlan(false);\n        SetScreen(UiScreen.None);\n    }')]),
    r314('Y-RR3-17', 'my_mutations', 'lim', 'ShowObjectives opens Map instead (unpinned routing; N-2 class)', [(ROOT, 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);\n        SetScreen(UiScreen.Objectives);', 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);\n        SetScreen(UiScreen.Map);')], documented='accepted'),
    r314('Y-RR3-18', 'my_mutations', 'lim', 'CloseAll no longer closes Objectives (unpinned routing; N-2 class)', [(ROOT, 'public void CloseAll()\n    {\n        CloseHelp();\n        manager.OpenWorkPlan(false);\n        SetScreen(UiScreen.None);', 'public void CloseAll()\n    {\n        CloseHelp();\n        manager.OpenWorkPlan(false);')], documented='accepted'),
    r314('Y-RR4-01', 'my_mutations', 'rej', 'allowlist element is a non-literal expression', [(ROOT, '"objective-primary-cta", "objective-work-plan", "objective-back-to-forest",', '"objective-primary-cta", "objective-work-plan", SomeName(),'), (ROOT, 'private static readonly string[] GuidanceButtonNames = {', 'private static string SomeName() => "objective-back-to-forest";\n    private static readonly string[] GuidanceButtonNames = {')]),
    r314('Y-RR4-02', 'my_mutations', 'rej', 'allowlist missing the map CTA', [(ROOT, '"objective-primary-cta", "objective-work-plan", "objective-back-to-forest",', '"objective-work-plan", "objective-back-to-forest",')]),
    r314('Y-RR4-03', 'my_mutations', 'rej', 'allowlist declared twice', [(ROOT, 'private static readonly string[] GuidanceButtonNames = {', 'private static readonly string[] GuidanceButtonNames = { "x" };\n    private static readonly string[] GuidanceButtonNames = {')]),
    r314('Y-RR4-04', 'my_mutations', 'rej', 'allowlist entry with interpolation', [(ROOT, '"objective-primary-cta", "objective-work-plan", "objective-back-to-forest",', '"objective-primary-cta", "objective-work-plan", $"objective-{"back"}-to-forest",')]),
    r314('Y-RR4-05', 'my_mutations', 'rej', 'allowlist duplicate (ordinary + escaped spelling)', [(ROOT, '"objective-primary-cta", "objective-work-plan", "objective-back-to-forest",', '"objective-primary-cta", "objective-work-plan", "objective-back-to-forest", "objective\\u002dback-to-forest",')]),
    r314('Y-RR4-06', 'my_mutations', 'rej', 'allowlist missing forest AND forest has wrong name', [(ROOT, '"objective-primary-cta", "objective-work-plan", "objective-back-to-forest",', '"objective-primary-cta", "objective-work-plan",'), (VIEW, 'forest.name = "objective-back-to-forest";', 'forest.name = "objective-forest";')]),
    r314('Y-RR4-07', 'my_mutations', 'rej', 'unterminated block comment in the root', [(ROOT, 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);', 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);\n        /* oops')]),
    r314('Y-RR4-08', 'my_mutations', 'rej', 'unbalanced brace in the view ctor', [(VIEW, 'modal.Add(header);', 'modal.Add(header);\n        {')]),
    r314('Y-RR4-09', 'my_mutations', 'rej', 'constructor missing entirely (renamed)', [(VIEW, 'public LearningObjectivesView(ScenarioOneUiRoot ui)', 'public LearningObjectivesViewX(ScenarioOneUiRoot ui)')]),
    r314('Y-RR4-10', 'my_mutations', 'rej', 'empty-ish companion: UiKit.cs deleted from companions', [(UIKIT, None, '')]),
    r314('Y-N-01', 'my_mutations', 'rej', 'interpolation: braces in nested string inside a hole, then class API', [(VIEW, 'public void ReloadProgress()\n    {', 'public void ReloadProgress()\n    {\n        _ = $"{(done.Count > 0 ? "}" : "{")}"; Root.SetEnabled(false);')]),
    r314('Y-N-02', 'my_mutations', 'rej', 'interpolation: escaped braces then hidden call', [(L1_SEQ, 'private int teachingWorldVersion = -1;', 'private int teachingWorldVersion = -1;\n    private void Sneak() { _ = $"{{}}{1}"; Root.SetEnabled(false); }')]),
    r314('Y-N-03', 'my_mutations', 'rej', 'interpolation: nested interpolated string in a hole + trailing call', [(L1_SEQ, 'private int teachingWorldVersion = -1;', 'private int teachingWorldVersion = -1;\n    private void Sneak() { _ = $"a{$"b{1}c"}d"; Root.SetEnabled(false); }')]),
    r314('Y-N-04', 'my_mutations', 'rej', 'verbatim interpolated with doubled quotes in hole then call', [(L1_SEQ, 'private int teachingWorldVersion = -1;', 'private int teachingWorldVersion = -1;\n    private void Sneak() { _ = $@"x{(true ? ""a"" : ""b"")}y"; Root.SetEnabled(false); }')]),
    r314('Y-N-05', 'my_mutations', 'rej', 'format clause with colon then hidden call', [(L1_SEQ, 'private int teachingWorldVersion = -1;', 'private int teachingWorldVersion = -1;\n    private void Sneak() { _ = $"{1:D3}"; Root.SetEnabled(false); }')]),
    r314('Y-N-06', 'my_mutations', 'rej', 'inactive #if region with unbalanced apostrophe prose then real call', [(L1_SEQ, 'private int teachingWorldVersion = -1;', "private int teachingWorldVersion = -1;\n#if CCF_NEVER\n    // it's dead text\n    don't parse this\n#endif\n    private void Sneak() { Root.SetEnabled(false); }")]),
    r314('Y-N-07', 'my_mutations', 'unk', 'inactive #if region containing a class-list call (conservative: treated as code)', [(L1_SEQ, 'private int teachingWorldVersion = -1;', 'private int teachingWorldVersion = -1;\n#if CCF_NEVER\n    private void Tint(Button b) { b.AddToClassList("x"); }\n#endif')], documented='rejected'),
    r314('Y-N-08', 'my_mutations', 'ben', '#region text with quotes and slashes', [(L1_SEQ, 'private int teachingWorldVersion = -1;', '#region it\'s "quoted" // not a comment\n    private int teachingWorldVersion = -1;\n    #endregion')]),
    r314('Y-N-09', 'my_mutations', 'rej', 'lone CR ends a // comment: hidden call after it', [(L1_SEQ, 'private int teachingWorldVersion = -1;', 'private int teachingWorldVersion = -1;\n    private void Sneak() { // harmless\r Root.SetEnabled(false); }')]),
    r314('Y-N-10', 'my_mutations', 'rej', 'U+2028 ends a // comment: hidden call after it', [(L1_SEQ, 'private int teachingWorldVersion = -1;', 'private int teachingWorldVersion = -1;\n    private void Sneak() { // harmless\u2028 Root.SetEnabled(false); }')]),
    r314('Y-N-11', 'my_mutations', 'rej', "zero-width joiner inside the 'forest' identifier (Cf ignored by the C# compiler)", [(VIEW, 'buttons.Add(forest);', 'buttons.Add(forest);\n        fo\u200drest.name += "-x";')]),
    r314('Y-N-12', 'my_mutations', 'rej', 'zero-width joiner inside a partial class name hides the companion', [(UI + 'LearningObjectivesView.Cf.cs', None, 'public sealed partial class Learning\u200dObjectivesView\n{\n    private void Sneak() => Root.SetEnabled(false);\n}\n')]),
    r314('Y-N-13', 'my_mutations', 'rej', 'NEL (U+0085) ends a // comment: hidden call after it', [(L1_SEQ, 'private int teachingWorldVersion = -1;', 'private int teachingWorldVersion = -1;\n    private void Sneak() { // harmless\x85 Root.SetEnabled(false); }')]),
    r314('Y-N-14', 'my_mutations', 'rej', 'Cf char inside GuidanceButtonNames reference outside the pins', [(ROOT, 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);', 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);\n        Guidance\u200dButtonNames[2] = "objective-none";')]),
    r314('Y-N-15', 'my_mutations', 'rej', 'Cf char inside guarded helpClosedFrame write', [(ROOT, 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);', 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);\n        helpClosed\u200dFrame = -1;')]),
    r314('Y-N-16', 'my_mutations', 'ben', 'BOM / NBSP between tokens in the header (benign whitespace)', [(VIEW, 'buttons.Add(forest);', 'buttons.Add(forest);\xa0')]),
    r314('Y-N-17', 'my_mutations', 'ben', 'unicode letter identifiers elsewhere (benign)', [(L1_SEQ, 'private int teachingWorldVersion = -1;', 'private int teachingWorldVersion = -1;\n    private int gr\xf6\xdfe = 1;')]),
    r314('Y-N-18', 'my_mutations', 'rej', 'char literal containing a quote then code', [(VIEW, 'public void ReloadProgress()\n    {', 'public void ReloadProgress()\n    {\n        char q = \'"\'; Root.SetEnabled(false);')]),
    r314('Y-N-19', 'my_mutations', 'rej', 'string containing // then hidden call', [(VIEW, 'public void ReloadProgress()\n    {', 'public void ReloadProgress()\n    {\n        _ = "http://x"; Root.SetEnabled(false);')]),
    r314('Y-N-20', 'my_mutations', 'rej', 'attribute with string containing bracket on a pinned method', [(ROOT, 'private void ClearNextAction()', '[UnityEngine.Scripting.Preserve]\n    private void ClearNextAction()')]),
    r314('Y-N-21', 'my_mutations', 'lim', 'global-namespace type hijacks Button in a new file that mentions no hint (documented not-claimed class)', [(UI + 'Hijack.cs', None, 'public sealed class Button : UnityEngine.UIElements.Button { }\n')], documented='accepted'),
    r314('Y-N-22', 'my_mutations', 'lim', 'non-companion file disables the map CTA via constructed name (disclosed L-01)', [(UI + 'Sneaky.cs', None, 'using UnityEngine; using UnityEngine.UIElements;\npublic sealed class Sneaky : MonoBehaviour { private void Update() { var d = FindAnyObjectByType<UIDocument>(); d.rootVisualElement.Q<Button>("objective-" + "primary-cta")?.SetEnabled(false); } }\n')], documented='accepted'),
    r314('Y-N-23', 'my_mutations', 'lim', 'non-companion file re-promotes Work Plan via constructed names (class re-add from outside)', [(UI + 'Sneaky.cs', None, 'using UnityEngine; using UnityEngine.UIElements;\npublic sealed class Sneaky : MonoBehaviour { private void Update() { var d = FindAnyObjectByType<UIDocument>(); d.rootVisualElement.Q<Button>("objective-" + "work-plan")?.AddToClassList("btn-" + "primary"); } }\n')], documented='accepted'),
    r314('Y-N-24', 'my_mutations', 'lim', 'root unpinned method disables all buttons of its document (disclosed L-02)', [(ROOT, 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);', 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);\n        document.rootVisualElement.Query<Button>().ForEach(b => b.SetEnabled(false));')], documented='accepted'),
    r314('Y-N-25', 'my_mutations', 'lim', 'root unpinned method hides the CTA by constructed name (disclosed L-01/L-02 class)', [(ROOT, 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);', 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);\n        document.rootVisualElement.Q<Button>("objective-" + "primary-cta").style.display = DisplayStyle.None;')], documented='accepted'),
    r314('Y-B-01', 'my_mutations', 'ben', 'new unrelated root method querying another label', [(ROOT, 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);', 'public Label FindTitle() => RootElement?.Q<Label>("some-title");\n\n    public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);')]),
    r314('Y-B-02', 'my_mutations', 'ben', 'new static readonly field in the view', [(VIEW, 'private readonly ScrollView scroll;', 'private readonly ScrollView scroll;\n    private static readonly string[] Spare = { "a", "b" };')]),
    r314('Y-B-03', 'my_mutations', 'ben', 'long lesson text edit in Curriculum partial', [(L1_CUR, 'public const string PreferencePrefix = "CCF.Learning.v1.";', 'public const string PreferencePrefix = "CCF.Learning.v1.";\n    private const string ExtraNote = "Mark rack entrance trees with the spray marker; do not mark Crop Trees.";')]),
    r314('Y-B-04', 'my_mutations', 'ben', 'extra using directive in root and view', [(ROOT, 'using UnityEngine.InputSystem;', 'using UnityEngine.InputSystem;\nusing System.Text;'), (VIEW, 'using UnityEngine.UIElements;\n\n// Optional learning progress', 'using UnityEngine.UIElements;\nusing System.Text;\n\n// Optional learning progress')]),
    r314('Y-B-05', 'my_mutations', 'ben', 'whitespace/tab and trailing-space changes inside pinned ClearNextAction', [(ROOT, 'nextAction.RemoveFromClassList("next-action");', 'nextAction.RemoveFromClassList( "next-action" ) ;   ')]),
    r314('Y-B-06', 'my_mutations', 'ben', 'XML doc comment with quotes before a root method', [(ROOT, 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);', '/// <summary>Opens the "objectives" screen; don\'t cache.</summary>\n    public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);')]),
    r314('Y-B-07', 'my_mutations', 'ben', '[SerializeField] field in root', [(ROOT, 'public sealed class ScenarioOneUiRoot : MonoBehaviour\n{', 'public sealed class ScenarioOneUiRoot : MonoBehaviour\n{\n    [SerializeField] private float spareDelay = 0.25f;')]),
    r314('Y-B-08', 'my_mutations', 'ben', 'expression-bodied root property reading learning', [(ROOT, 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);', 'public bool ObjectivesOpen => learning != null && screen == UiScreen.Objectives;\n\n    public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);')]),
    r314('Y-B-09', 'my_mutations', 'unk', 'root method passing learning to a helper (README documents this as rejected)', [(ROOT, 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);', 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);\n        Describe(learning);'), (ROOT, 'private static readonly string[] GuidanceButtonNames = {', 'private static void Describe(LearningObjectivesView v) { }\n    private static readonly string[] GuidanceButtonNames = {')], documented='rejected'),
    r314('Y-B-10', 'my_mutations', 'unk', 'string with btn-primary text in a log call in the view (README: any btn-primary literal counts)', [(VIEW, 'public void ReloadProgress()\n    {', 'public void ReloadProgress()\n    {\n        Debug.Log("btn-primary is only used by Back to forest");')], documented='rejected'),
    r314('Y-B-11', 'my_mutations', 'unk', 'new added unrelated view-class method using Q<> (README documents Q as banned)', [(L1_CUR, 'public const string PreferencePrefix = "CCF.Learning.v1.";', 'public const string PreferencePrefix = "CCF.Learning.v1.";\n    private Label Find() => scroll.Q<Label>("x");')], documented='rejected'),
    r314('Y-B-12', 'my_mutations', 'unk', 'new editor test mentioning header name (companion rule)', [(UI + 'Editor/ObjectiveTests.cs', None, 'public static class ObjectiveNames { public const string Cta = "objective-primary-cta"; }\n')], documented='rejected'),
    r314('Y-B-13', 'my_mutations', 'ben', 'unrelated new MonoBehaviour that mentions UiKit', [(UI + 'NewPanelView.cs', None, 'using UnityEngine.UIElements;\npublic sealed class NewPanelView { public NewPanelView() { var b = UiKit.Button("Go", () => { }, true, "btn-primary"); } }\n')]),
    r314('Y-B-14', 'my_mutations', 'ben', 'pinned Refresh: comment block inserted mid-body', [(ROOT, 'string target = null;', '/* choose a target */ string target = null; // none yet')]),
    r314('Y-B-15', 'my_mutations', 'ben', 'header: region and extra comment lines', [(VIEW, 'Button map = UiKit.Button("Stand map [M]", () => ui.ShowMap());', '// the stand map\n        Button map = UiKit.Button("Stand map [M]", () => ui.ShowMap());')]),
    r314('Y-B-16', 'my_mutations', 'ben', 'allowlist extended with an extra name and reordered', [(ROOT, '"objective-primary-cta", "objective-work-plan", "objective-back-to-forest",', '"objective-back-to-forest", "objective-primary-cta", "objective-work-plan", "objective-extra",')]),
    r314('Y-B-17', 'my_mutations', 'ben', 'CRLF line endings in every checked file', [], transform='crlf'),
    r314('Y-B-18', 'my_mutations', 'ben', 'new Objectives header extra button, non-primary, new caption', [(VIEW, 'buttons.Add(forest);', 'buttons.Add(forest);\n        Button help2 = UiKit.Button("Help [F1]", () => ui.CloseAll()); help2.name = "objective-help"; buttons.Add(help2);')]),
    # my_mutations2.py sha256 87a67615d7c5bbb6e809306e562af513c5b38eabf10c463f3445b5419ec75d94
    r314('Z-RR1-07', 'my_mutations2', 'rej', 'second static primary in the Curriculum partial (qualified types)', [(L1_CUR, 'public const string PreferencePrefix = "CCF.Learning.v1.";', 'public const string PreferencePrefix = "CCF.Learning.v1.";\n    private UnityEngine.UIElements.Button Extra() => UiKit.Button("x", () => ui.CloseAll(), true, "btn-primary");')]),
    r314('Z-RR1-08', 'my_mutations2', 'rej', 'primary class through UiKit.Box in a partial (qualified)', [(L1_CUR, 'public const string PreferencePrefix = "CCF.Learning.v1.";', 'public const string PreferencePrefix = "CCF.Learning.v1.";\n    private UnityEngine.UIElements.VisualElement Extra() => UiKit.Box("btn-primary");')]),
    r314('Z-RR1-09', 'my_mutations2', 'rej', 'primary class through UiKit.Add in a partial (qualified)', [(L1_CUR, 'public const string PreferencePrefix = "CCF.Learning.v1.";', 'public const string PreferencePrefix = "CCF.Learning.v1.";\n    private UnityEngine.UIElements.Label Extra(UnityEngine.UIElements.VisualElement p) => UiKit.Add(p, "t", "btn-primary");')]),
    r314('Z-N-04', 'my_mutations2', 'rej', 'verbatim interpolated with nested string in hole then call (valid C#)', [(L1_SEQ, 'private int teachingWorldVersion = -1;', 'private int teachingWorldVersion = -1;\n    private void Sneak() { _ = $@"x{(true ? "a" : "b")}y"; Root.SetEnabled(false); }')]),
    r314('Z-B-11', 'my_mutations2', 'unk', 'new view-class method using Q<> (README documents Q as banned)', [(L1_CUR, 'public const string PreferencePrefix = "CCF.Learning.v1.";', 'public const string PreferencePrefix = "CCF.Learning.v1.";\n    private UnityEngine.UIElements.Label Find() => scroll.Q<UnityEngine.UIElements.Label>("x");')], documented='rejected'),
    r314('Z-RR2-T-help', 'my_mutations2', 'rej', 'tuple deconstruction writes help in an unpinned method', [(ROOT, 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);', 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);\n        (help, workPlanWasOpen) = (null, false);')]),
    r314('Z-RR2-T-screen', 'my_mutations2', 'rej', 'tuple deconstruction writes screen in an unpinned method', [(ROOT, 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);', 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);\n        (screen, workPlanWasOpen) = (UiScreen.None, false);')]),
    r314('Z-RR2-T-rackLayout', 'my_mutations2', 'rej', 'tuple deconstruction writes rackLayout in an unpinned method', [(ROOT, 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);', 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);\n        (rackLayout, workPlanWasOpen) = (null, false);')]),
    r314('Z-RR2-T-map', 'my_mutations2', 'rej', 'tuple deconstruction writes map in an unpinned method', [(ROOT, 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);', 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);\n        (map, workPlanWasOpen) = (null, false);')]),
    r314('Z-RR2-T-workPlan', 'my_mutations2', 'rej', 'tuple deconstruction writes workPlan in an unpinned method', [(ROOT, 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);', 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);\n        (workPlan, workPlanWasOpen) = (null, false);')]),
    r314('Z-RR2-T-review', 'my_mutations2', 'rej', 'tuple deconstruction writes review in an unpinned method', [(ROOT, 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);', 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);\n        (review, workPlanWasOpen) = (null, false);')]),
    r314('Z-RR2-C-screen', 'my_mutations2', 'rej', 'count-compensated: TryShowReviewYear write removed, ShowObjectives retargets screen to Map', [(ROOT, 'screen = UiScreen.Review;\n        CloseHelp();', 'CloseHelp();'), (ROOT, 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);\n        SetScreen(UiScreen.Objectives);', 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);\n        SetScreen(UiScreen.Objectives);\n        screen = UiScreen.Map;')]),
    r314('Z-RR2-C-help', 'my_mutations2', 'rej', "count-compensated: Start's help initialiser replaced by null (one write, value changed)", [(ROOT, 'help = new MenuHelpView(this);', 'help = null;')]),
    r314('Z-RR2-P-help', 'my_mutations2', 'rej', 'help gets a property alias writer (property setter)', [(ROOT, 'private static readonly string[] GuidanceButtonNames = {', 'private MenuHelpView HelpAlias { set { help = value; } }\n    private static readonly string[] GuidanceButtonNames = {')]),
    r314('Z-N-CR-region', 'my_mutations2', 'rej', 'lone CR ends a #region line: hidden call after it', [(L1_SEQ, 'private int teachingWorldVersion = -1;', 'private int teachingWorldVersion = -1;\n    #region x\r    private void Sneak() { Root.SetEnabled(false); }\n    #endregion')]),
    r314('Z-N-PS', 'my_mutations2', 'rej', 'U+2029 ends a // comment: hidden call after it', [(L1_SEQ, 'private int teachingWorldVersion = -1;', 'private int teachingWorldVersion = -1;\n    private void Sneak() { // harmless\u2029 Root.SetEnabled(false); }')]),
    r314('Z-N-CR-root', 'my_mutations2', 'rej', 'lone CR hides a guarded-state write in the root', [(ROOT, 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);', 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);\n        // reset\r guidanceCandidate = null;')]),
    r314('Z-N-CF-buttons', 'my_mutations2', 'rej', 'ZWJ identifier clears the header row in the ctor', [(VIEW, 'modal.Add(header);', 'modal.Add(header);\n        bu\u200dttons.Clear();')]),
    r314('Z-N-CF-secondprimary', 'my_mutations2', 'rej', 'second STATIC PRIMARY in the view via ZWJ-split UiKit and fragment class (N-1 break)', [(VIEW, 'scroll.Add(library);\n        ui.RefreshNextActionGuidance();', 'scroll.Add(Ui\u200dKit.Button("Close", () => ui.CloseAll(), true, "btn-" + "primary"));\n        scroll.Add(library);\n        ui.RefreshNextActionGuidance();')]),
    r314('Z-N-CF-secondprimary-ctl', 'my_mutations2', 'rej', 'control: same second primary WITHOUT the ZWJ', [(VIEW, 'scroll.Add(library);\n        ui.RefreshNextActionGuidance();', 'scroll.Add(UiKit.Button("Close", () => ui.CloseAll(), true, "btn-" + "primary"));\n        scroll.Add(library);\n        ui.RefreshNextActionGuidance();')]),
    r314('Z-N-CF-allowlist-clear', 'my_mutations2', 'rej', 'ZWJ-split Array.Clear of the allowlist', [(ROOT, 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);', 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);\n        System.Array.Clear(Guidance\u200bButtonNames, 0, 3);')]),
    r314('Z-N-CF-ctl-allowlist', 'my_mutations2', 'rej', 'control: Array.Clear of the allowlist WITHOUT ZWJ', [(ROOT, 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);', 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);\n        System.Array.Clear(GuidanceButtonNames, 0, 3);')]),
    r314('Z-N-SHY', 'my_mutations2', 'rej', 'soft hyphen (Cf) in forest identifier', [(VIEW, 'buttons.Add(forest);', 'buttons.Add(forest);\n        for\xadest.name += "-x";')]),
    r314('Z-N-CF-ctl-forest', 'my_mutations2', 'rej', 'control: forest.name += without Cf', [(VIEW, 'buttons.Add(forest);', 'buttons.Add(forest);\n        forest.name += "-x";')]),
    r314('Z-N-NBSP-ctl', 'my_mutations2', 'ben', 'control: NBSP inside identifier is a different (undeclared) identifier -> not valid; skip', [(VIEW, 'buttons.Add(forest);', 'buttons.Add(forest);\n        _ = 1;')]),
    r314('Z-N-else', 'my_mutations2', 'rej', '#if/#else with the hostile code only in the #else branch', [(L1_SEQ, 'private int teachingWorldVersion = -1;', 'private int teachingWorldVersion = -1;\n#if UNITY_EDITOR\n    private void A() { }\n#else\n    private void A() { Root.SetEnabled(false); }\n#endif')]),
    r314('Z-N-nested-interp-view', 'my_mutations2', 'unk', 'benign nested-literal interpolation in LearningObjectivesView.cs (documented O-01)', [(VIEW, 'public void ReloadProgress()\n    {', 'public void ReloadProgress()\n    {\n        _ = $"{(done.Count > 0 ? "yes" : "no")}";')], documented='rejected'),
    r314('Z-N-nested-interp-seq', 'my_mutations2', 'ben', 'benign nested-literal interpolation in a partial (precise lexer)', [(L1_SEQ, 'private int teachingWorldVersion = -1;', 'private int teachingWorldVersion = -1;\n    private string Note() => $"{(done.Count > 0 ? "yes" : "no")} / {$"{done.Count}"}";')]),
    r314('Z-N-verbatim-hash', 'my_mutations2', 'unk', 'verbatim string with a line starting # (preprocessor-like)', [(L1_SEQ, 'private int teachingWorldVersion = -1;', 'private int teachingWorldVersion = -1;\n    private string Note() => @"line1\n#not a directive";')], documented='rejected'),
    r314('Z-N-cdata', 'my_mutations2', 'rej', 'string ending in backslash then code: "a\\\\" then call', [(L1_SEQ, 'private int teachingWorldVersion = -1;', 'private int teachingWorldVersion = -1;\n    private void Sneak() { _ = "a\\\\"; Root.SetEnabled(false); }')]),
    r314('Z-N-chars', 'my_mutations2', 'rej', 'char literals of quote/brace/backslash then hidden call', [(L1_SEQ, 'private int teachingWorldVersion = -1;', 'private int teachingWorldVersion = -1;\n    private void Sneak() { char a = \'"\', b = \'{\', c = \'\\\\\', d = \'\\\'\'; Root.SetEnabled(false); }')]),
    r314('Z-RR4-12', 'my_mutations2', 'rej', 'allowlist contains null', [(ROOT, '"objective-primary-cta", "objective-work-plan", "objective-back-to-forest",', '"objective-primary-cta", "objective-work-plan", "objective-back-to-forest", null,')]),
    r314('Z-RR4-13', 'my_mutations2', 'unk', 'allowlist behind #if with an unrelated static primary also present', [(ROOT, 'private static readonly string[] GuidanceButtonNames = {', '#if UNITY_EDITOR\n    private static readonly string[] Unused = { "z" };\n#endif\n    private static readonly string[] GuidanceButtonNames = {')], documented='accepted'),
    r314('Z-B-20', 'my_mutations2', 'unk', 'Back to forest caption copy tweak (plausible product text edit)', [(VIEW, '"Back to forest [O / Esc]"', '"Back to the forest [O / Esc]"')], documented='rejected'),
    r314('Z-B-21', 'my_mutations2', 'unk', 'Work Plan caption copy tweak', [(VIEW, '"Work Plan [Tab]"', '"Open Work Plan [Tab]"')], documented='rejected'),
    r314('Z-B-22', 'my_mutations2', 'unk', 'an unrelated literal tweak inside pinned Refresh (reason text)', [(ROOT, 'reason = "Two rack questions are open; keep or dismiss them before continuing.";', 'reason = "Two rack questions are open; keep or dismiss both before continuing.";')], documented='rejected'),
    r314('Z-B-23', 'my_mutations2', 'ben', 'unrelated new method in root using document.rootVisualElement and SetEnabled', [(ROOT, 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);', 'public void Freeze(bool on) { if (document != null) document.rootVisualElement.SetEnabled(!on); }\n\n    public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);')]),
    r314('Z-B-24', 'my_mutations2', 'ben', 'header extra button with enabled=false', [(VIEW, 'buttons.Add(forest);', 'buttons.Add(forest);\n        Button soon = UiKit.Button("Soon", () => ui.CloseAll(), false); soon.name = "objective-soon"; buttons.Add(soon);')]),
    # my_mutations3.py sha256 368ae70c28d5d9a6852ca40f0936860d65b703f5258b1d7ed43e8b94dfeddf56
    r314('W-DEP-01', 'my_mutations3', 'rej', 'NeedsAnnualReview forced true: pinned chain always diverts to the annual-review target (X-F2-12 effect)', [(ROOT, 'public bool NeedsAnnualReview => manager != null && manager.AnnualReports.Count > 0 && !manager.AnnualReviewSeen;', 'public bool NeedsAnnualReview => true;')]),
    r314('W-DEP-02', 'my_mutations3', 'rej', 'TeachingRacks forced false: Objective 1 never offered a guidance target (X-F2-14 effect)', [(VIEW, 'public bool TeachingRacks => CurrentTopicIndex == 0;', 'public bool TeachingRacks => false;')]),
    r314('W-DEP-03', 'my_mutations3', 'rej', 'CurrentTopicIndex forced to 1', [(VIEW, 'public int CurrentTopicIndex => ScenarioOneTeachingSequence.Current(TeachingFacts());', 'public int CurrentTopicIndex => 1;')]),
    r314('W-DEP-04', 'my_mutations3', 'rej', 'NeedsAnnualReview routed through a helper returning true', [(ROOT, 'public bool NeedsAnnualReview => manager != null && manager.AnnualReports.Count > 0 && !manager.AnnualReviewSeen;', 'public bool NeedsAnnualReview => Always();\n    private static bool Always() => true;')]),
    r314('W-DEP-05', 'my_mutations3', 'lim', 'Objective guidance spot: ReloadProgress marks everything done (lesson stage jumps)', [(VIEW, 'foreach (string id in StepIds) if (PlayerPrefs.GetInt(PreferencePrefix + id, 0) == 1) done.Add(id);', 'foreach (string id in StepIds) done.Add(id);')], documented='accepted'),
    # my_mutations4.py sha256 51468d76c3117e721e158b54872941a9d54fea182b0ee69d69d4cba42113cb93
    r314('V-L06-1', 'my_mutations4', 'lim', 'companion aliases ui.Learning in a local then restyles every Objectives button (disclosed L-06)', [(UI + 'Alias.cs', None, 'using UnityEngine.UIElements;\npublic sealed class Alias { public void Go(ScenarioOneUiRoot ui) { var v = ui.Learning; v.Root.Query<Button>().ForEach(b => b.AddToClassList("btn-primary")); } }\n')], documented='rejected'),
    r314('V-L06-2', 'my_mutations4', 'lim', 'existing companion (WalkingHudView) gets the same alias route', [(UI + 'WalkingHudView.cs', 'ObjectiveWidget.Apply(ui.Learning.HudModel());', 'ObjectiveWidget.Apply(ui.Learning.HudModel());\n            LearningObjectivesView lv = ui.Learning; lv.Root.Query<Button>().ForEach(b => b.SetEnabled(false));')], documented='rejected'),
    r314('V-L01-1', 'my_mutations4', 'lim', 'companion re-primaries every button of the document (no header names, no Learning.Root)', [(UI + 'Alias2.cs', None, 'using UnityEngine.UIElements;\npublic sealed class Alias2 { public void Go(ScenarioOneUiRoot ui) { ui.RootElement.Query<Button>().ForEach(b => b.AddToClassList("btn-primary")); } }\n')], documented='accepted'),
    # my_mutations5.py sha256 74aa34b0a0e96dd33c013085a693387060b16d23226d8a4228eb3684fded7bc0
    r314('U-RR2-T-manager', 'my_mutations5', 'rej', 'tuple deconstruction writes manager (null) in an unpinned method', [(ROOT, 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);', 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);\n        (manager, workPlanWasOpen) = (null, false);')]),
    r314('U-RR2-T-learning-ctl', 'my_mutations5', 'rej', 'control: tuple deconstruction writes learning (caught by the alias rule)', [(ROOT, 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);', 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);\n        (learning, workPlanWasOpen) = (null, false);')]),
    r314('U-RR2-T-helpClosedFrame-ctl', 'my_mutations5', 'rej', 'control: tuple deconstruction writes helpClosedFrame (guarded name)', [(ROOT, 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);', 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);\n        (helpClosedFrame, workPlanWasOpen) = (-1, false);')]),
    # my_mutations6.py sha256 039fb2eb252aaa769b652c09dee7295131023e0c1cd6df75f697b446d1527922
    r314('V-L03', 'my_mutations6', 'lim', 'non-terminating do/while before the Objectives caller (disclosed L-03)', [(VIEW, 'scroll.Add(library);\n        ui.RefreshNextActionGuidance();', 'scroll.Add(library);\n        do { } while (force || !force);\n        ui.RefreshNextActionGuidance();')], documented='accepted'),
    r314('V-L04', 'my_mutations6', 'lim', 'reflection writes guarded state by string name (disclosed L-04)', [(ROOT, 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);', 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);\n        typeof(ScenarioOneUiRoot).GetField("guidance" + "Candidate", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).SetValue(this, null);')], documented='accepted'),
    r314('V-L07', 'my_mutations6', 'lim', 'exception thrown by an expression before the Objectives caller (disclosed L-07)', [(VIEW, 'scroll.Add(library);\n        ui.RefreshNextActionGuidance();', 'scroll.Add(library);\n        _ = Boom(force);\n        ui.RefreshNextActionGuidance();'), (VIEW, 'private readonly ScrollView scroll;', 'private readonly ScrollView scroll;\n    private static int Boom(bool f) { if (f) throw new System.InvalidOperationException(); return 0; }')], documented='accepted'),
    # my_mutations8.py sha256 b043d308191191284333e3450922cfab3985acce73c24a7a88446503dd53becb
    r314('A8-D1-help-prop-decoy', 'my_mutations8', 'rej', 'help becomes a property with a no-op setter (always null); the reviewed field text survives once in a nested decoy class', [(ROOT, 'private MenuHelpView help;', 'private MenuHelpView help { get => null; set { } }\n    private sealed class ZDecoyHelp { private MenuHelpView help; }')], intended='(N-2 owning class)'),
    r314('A8-D2-screen-prop-decoy', 'my_mutations8', 'rej', 'screen becomes a property that always reads Map (no-op setter); the reviewed initialiser survives once in a nested decoy class', [(ROOT, 'private UiScreen screen = UiScreen.None;', 'private UiScreen screen { get => UiScreen.Map; set { } }\n    private sealed class ZDecoyScreen { private UiScreen screen = UiScreen.None; }')], intended='(N-2 owning class)'),
    r314('A8-D3-manager-prop-decoy', 'my_mutations8', 'rej', 'manager becomes a property with a no-op setter; reviewed declaration survives once in a nested decoy', [(ROOT, 'private ScenarioOneManager manager;', 'private ScenarioOneManager manager { get => null; set { } }\n    private sealed class ZDecoyMgr { private ScenarioOneManager manager; }')], intended='(N-2 owning class)'),
    r314('A8-D4-nar-decoy', 'my_mutations8', 'rej', 'NeedsAnnualReview forced true; the reviewed expression survives once in a nested decoy class with its own manager field', [(ROOT, 'public bool NeedsAnnualReview => manager != null && manager.AnnualReports.Count > 0 && !manager.AnnualReviewSeen;', 'public bool NeedsAnnualReview => true;\n    private sealed class ZDecoyNar { internal ScenarioOneManager manager; public bool NeedsAnnualReview => manager != null && manager.AnnualReports.Count > 0 && !manager.AnnualReviewSeen; }')], intended='(N-2 owning class)'),
    r314('A8-D5-teaching-decoy', 'my_mutations8', 'rej', 'TeachingRacks forced true; both reviewed members survive once in a nested decoy class of the view', [(VIEW, 'public int CurrentTopicIndex => ScenarioOneTeachingSequence.Current(TeachingFacts());', 'public int CurrentTopicIndex => 0;'), (VIEW, 'public bool TeachingRacks => CurrentTopicIndex == 0;', 'public bool TeachingRacks => true;\n    private sealed class ZDecoyTeach { private static ScenarioOneTeachingSequence.Facts TeachingFacts() => null; public int CurrentTopicIndex => ScenarioOneTeachingSequence.Current(TeachingFacts()); public bool TeachingRacks => CurrentTopicIndex == 0; }')], intended='(N-2 owning class)'),
    r314('A8-D5b-teaching-decoy-simple', 'my_mutations8', 'rej', 'TeachingRacks forced true; only the TeachingRacks text survives in a nested decoy (CurrentTopicIndex stays real)', [(VIEW, 'public bool TeachingRacks => CurrentTopicIndex == 0;', 'public bool TeachingRacks => true;\n    private sealed class ZDecoyTeach2 { public int CurrentTopicIndex => 0; public bool TeachingRacks => CurrentTopicIndex == 0; }')], intended='(N-2 owning class)'),
    r314('A8-D5c-teaching-decoy-ctl', 'my_mutations8', 'rej', 'control for D5b: TeachingRacks forced true and NO decoy', [(VIEW, 'public bool TeachingRacks => CurrentTopicIndex == 0;', 'public bool TeachingRacks => true;')]),
    r314('A8-D6-ctor-count', 'my_mutations8', 'unk', 'decoy nested class declares a second LearningObjectivesView-like text (sanity: nested class with a field named Root)', [(VIEW, 'public bool TeachingRacks => CurrentTopicIndex == 0;', 'public bool TeachingRacks => CurrentTopicIndex == 0;\n    private sealed class ZNest { public int Root; }')], documented='rejected'),
    r314('A8-S-reorder', 'my_mutations8', 'lim', 'Start: map and workPlan constructions swapped (order of reviewed writes is not pinned; disclosed)', [(ROOT, 'workPlan = new WorkPlanView(this);\n        review = new AnnualReviewView(this);\n        map = new StandMapView(this);', 'map = new StandMapView(this);\n        review = new AnnualReviewView(this);\n        workPlan = new WorkPlanView(this);')], documented='accepted'),
    r314('A8-S-deadwrite', 'my_mutations8', 'lim', 'Start: infinite loop before the help write (non-termination, disclosed L-03/L-08 class)', [(ROOT, 'help = new MenuHelpView(this);', 'while (document == null) { }\n        help = new MenuHelpView(this);')], documented='accepted'),
    r314('A8-S-neverrun', 'my_mutations8', 'rej', 'Start: help write after an unbraced if-guard (conditional)', [(ROOT, 'help = new MenuHelpView(this);', 'if (document != null) help = new MenuHelpView(this);')]),
    r314('A8-L06-isnull', 'my_mutations8', 'over', 'companion: ui.Learning is null (C# 9 idiom) - benign read', [(L1_STANDMAP, 'private readonly ScenarioOneUiRoot ui;', 'private readonly ScenarioOneUiRoot ui;\n    private bool NoLessons => ui.Learning is null;')], documented='rejected'),
    r314('A8-L06-isnotnull', 'my_mutations8', 'over', 'companion: ui.Learning is not null (C# 9 idiom) - benign read', [(L1_STANDMAP, 'private readonly ScenarioOneUiRoot ui;', 'private readonly ScenarioOneUiRoot ui;\n    private bool HasLessons => ui.Learning is not null;')], documented='rejected'),
    r314('A8-L06-localalias-read', 'my_mutations8', 'over', 'companion: local alias of ui.Learning used only for a read', [(L1_STANDMAP, 'private readonly ScenarioOneUiRoot ui;', 'private readonly ScenarioOneUiRoot ui;\n    private int Idx() { var l = ui.Learning; return l == null ? -1 : l.CurrentTopicIndex; }')], documented='rejected'),
    r314('A8-L06-coalesce', 'my_mutations8', 'over', 'companion: ui.Learning ?? x null-coalescing read', [(L1_STANDMAP, 'private readonly ScenarioOneUiRoot ui;', 'private readonly ScenarioOneUiRoot ui;\n    private LearningObjectivesView Get(LearningObjectivesView fallback) => ui.Learning ?? fallback;')], documented='rejected'),
    r314('A8-L06-elementtraversal', 'my_mutations8', 'lim', 'companion reaches Objectives elements only through a public member of the view (documented residual)', [(L1_STANDMAP, 'private readonly ScenarioOneUiRoot ui;', 'private readonly ScenarioOneUiRoot ui;\n    private void Hide() { ui.Learning.Refresh(true); }')], documented='accepted'),
    r314('A8-L06-separator', 'my_mutations8', 'rej', 'companion: ui . Learning with comments/newline before Root', [(L1_STANDMAP, 'private readonly ScenarioOneUiRoot ui;', 'private readonly ScenarioOneUiRoot ui;\n    private void Sneak() { ui /*c*/ .\n Learning /*c*/ . /*c*/ Root.SetEnabled(false); }')]),
    r314('A8-L06-verbatim', 'my_mutations8', 'rej', 'companion: ui.@Learning.@Root', [(L1_STANDMAP, 'private readonly ScenarioOneUiRoot ui;', 'private readonly ScenarioOneUiRoot ui;\n    private void Sneak() { ui.@Learning.@Root.SetEnabled(false); }')]),
    r314('A8-L06-nullcond', 'my_mutations8', 'rej', 'companion: ui?.Learning?.Root', [(L1_STANDMAP, 'private readonly ScenarioOneUiRoot ui;', 'private readonly ScenarioOneUiRoot ui;\n    private void Sneak() { ui?.Learning?.Root.SetEnabled(false); }')]),
    r314('A8-L06-tuple', 'my_mutations8', 'rej', 'companion: tuple deconstruction alias of ui.Learning', [(L1_STANDMAP, 'private readonly ScenarioOneUiRoot ui;', 'private readonly ScenarioOneUiRoot ui;\n    private void Sneak() { var (v, n) = (ui.Learning, 0); v.Root.SetEnabled(false); }')]),
    r314('A8-L06-arg', 'my_mutations8', 'rej', 'companion: ui.Learning passed to an Action<LearningObjectivesView> lambda', [(L1_STANDMAP, 'private readonly ScenarioOneUiRoot ui;', 'private readonly ScenarioOneUiRoot ui;\n    private void Sneak() { System.Action<LearningObjectivesView> a = v => v.Root.SetEnabled(false); a(ui.Learning); }')]),
    r314('A8-L06-generic', 'my_mutations8', 'rej', 'companion: generic helper receives ui.Learning (type inferred)', [(L1_STANDMAP, 'private readonly ScenarioOneUiRoot ui;', 'private readonly ScenarioOneUiRoot ui;\n    private static void Z<T>(T v) { }\n    private void Sneak() { Z(ui.Learning); }')]),
    r314('A8-L06-switch', 'my_mutations8', 'rej', 'companion: switch expression over ui.Learning', [(L1_STANDMAP, 'private readonly ScenarioOneUiRoot ui;', 'private readonly ScenarioOneUiRoot ui;\n    private int Sneak() => ui.Learning switch { null => 0, _ => 1 };')]),
    r314('A8-B-interp-screen', 'my_mutations8', 'ben', 'root: Debug.Log($"screen={screen}") in an unpinned method (interpolation hole, not a whole argument)', [(ROOT, 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);', 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);\n        Debug.Log($"screen={screen}");')]),
    r314('A8-B-screen-tostring', 'my_mutations8', 'unk', 'root: screen.ToString() in an unpinned method (member receiver)', [(ROOT, 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);', 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);\n        Debug.Log(screen.ToString());')], documented='rejected'),
    r314('A8-B-screen-eq', 'my_mutations8', 'ben', 'root: screen comparisons and ternary only', [(ROOT, 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);', 'public bool IsMapOrReview() { return screen == UiScreen.Map || screen == UiScreen.Review ? true : screen != UiScreen.None; }\n\n    public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);')]),
    r314('A8-B-screen-switchexpr', 'my_mutations8', 'ben', 'root: screen switch expression', [(ROOT, 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);', 'public int ScreenIndex() => screen switch { UiScreen.Map => 1, UiScreen.Review => 2, _ => 0 };\n\n    public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);')]),
    r314('A8-B-help-reads', 'my_mutations8', 'ben', 'root: reads of help/manager/map with ?. and is-pattern in an unpinned method', [(ROOT, 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);', 'public bool ZReads() { manager?.OpenWorkPlan(false); return help != null && help.IsOpen || map is null || workPlan is not null; }\n\n    public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);')]),
    r314('A8-B-start-swap-region', 'my_mutations8', 'ben', "root: #region wrapped around part of Start's constructions (no #if)", [(ROOT, 'learning = new LearningObjectivesView(this);', '#region views\n        learning = new LearningObjectivesView(this);'), (ROOT, 'root.Add(learning.Root);', 'root.Add(learning.Root);\n        #endregion')]),
    r314('A8-B-start-try', 'my_mutations8', 'ben', 'root: try/catch around an unrelated Start statement before the reviewed writes', [(ROOT, 'root.AddToClassList("root");', 'try { root.AddToClassList("root"); } catch (System.Exception) { }')]),
    r314('A8-B-nonascii-ident', 'my_mutations8', 'ben', 'partial: non-ASCII letters in a new identifier and in comments/strings (precomposed e-acute, sharp s)', [(L1_SEQ, 'private int teachingWorldVersion = -1;', 'private int teachingWorldVersion = -1;\n    private int gr\xf6\xdfe = 1; // na\xefve caf\xe9 \u2014 ok\n    private const string Cafe = "caf\xe9 \u2014 \U0001f332";')]),
    r314('A8-B-ctrlz-comment', 'my_mutations8', 'ben', 'partial: form feed and SUB inside a // comment (page-break artefact)', [(L1_SEQ, 'private int teachingWorldVersion = -1;', 'private int teachingWorldVersion = -1;\n    // page break \x0c and eof marker \x1a inside a comment\n    private int zc = 1;')]),
    r314('A8-B-ff-line', 'my_mutations8', 'unk', 'partial: a form-feed-only line between members (old GNU style page break) - control char in code', [(L1_SEQ, 'private int teachingWorldVersion = -1;', 'private int teachingWorldVersion = -1;\n\x0c\n    private int zf = 1;')], documented='rejected'),
    r314('A8-B-lone-bom-comment', 'my_mutations8', 'ben', 'partial: U+FEFF inside a comment and a string', [(L1_SEQ, 'private int teachingWorldVersion = -1;', 'private int teachingWorldVersion = -1;\n    // zero width no-break space: \ufeff here\n    private const string Zw = "a\ufeffb";')]),
    r314('A8-B-crlf-all', 'my_mutations8', 'ben', 'CRLF endings in the new root and a new partial (CR followed by LF is fine)', [(ROOT, 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);', 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);\r\n        // crlf')]),
    r314('A8-B-hash-in-string', 'my_mutations8', 'ben', "partial: '#' inside an ordinary string and a char literal", [(L1_SEQ, 'private int teachingWorldVersion = -1;', 'private int teachingWorldVersion = -1;\n    private const string Hash = "#if NEVER";\n    private const char H = \'#\';')]),
    r314('A8-B-multiline-verbatim-indented-hash', 'my_mutations8', 'unk', "partial: verbatim string whose second line starts with whitespace and '#' (e.g. markdown heading) - documented fail-closed", [(L1_SEQ, 'private int teachingWorldVersion = -1;', 'private int teachingWorldVersion = -1;\n    private const string Md = @"title\n    # heading\ntext";')], documented='rejected'),
    r314('A8-B-multiline-comment-hash', 'my_mutations8', 'unk', "partial: block comment with an indented '#' line (e.g. commented-out #if)", [(L1_SEQ, 'private int teachingWorldVersion = -1;', 'private int teachingWorldVersion = -1;\n    /* old code:\n       #if UNITY_EDITOR\n       #endif */\n    private int zk = 1;')], documented='rejected'),
    r314('A8-B-xml-doc-hash', 'my_mutations8', 'unk', "partial: XML doc comment (///) lines containing an indented '#'", [(L1_SEQ, 'private int teachingWorldVersion = -1;', 'private int teachingWorldVersion = -1;\n    /// <summary>\n    ///   # not a directive\n    /// </summary>\n    private int zx = 1;')], documented='accepted'),
    r314('A8-L-nul-code', 'my_mutations8', 'rej', 'NUL char (U+0000) inside code of the view partial hiding nothing but desyncing Python', [(L1_SEQ, 'private int teachingWorldVersion = -1;', 'private int teachingWorldVersion = -1;\n    private int z\x00y = 1;')]),
    r314('A8-L-hangul-filler', 'my_mutations8', 'ben', 'partial: Hangul filler (Lo) in an identifier - a different identifier for Roslyn and for the checker', [(L1_SEQ, 'private int teachingWorldVersion = -1;', 'private int teachingWorldVersion = -1;\n    private int fo\u3164rest = 1;')]),
    r314('A8-L-astral-cf', 'my_mutations8', 'rej', 'astral Cf (U+E0001 language tag) in code of the partial', [(L1_SEQ, 'private int teachingWorldVersion = -1;', 'private int teachingWorldVersion = -1;\n    private int z\U000e0001y = 1;')]),
    r314('A8-L-bidi-comment', 'my_mutations8', 'ben', 'Bidi override U+202E inside a comment and string (display-only; not a parse change)', [(L1_SEQ, 'private int teachingWorldVersion = -1;', 'private int teachingWorldVersion = -1;\n    // \u202e reversed\n    private const string Rev = "\u202eabc";')]),
    # my_mutations9.py sha256 1037732761d8925d43bc98f1919aefbfa5610e1a7c95d6cfe9499cb65facc7a5
    r314('A9-D7-toplevel-decoy-root', 'my_mutations9', 'rej', 'NeedsAnnualReview forced true; the reviewed expression survives once in a SEPARATE top-level class appended to the root file', [(ROOT, 'public bool NeedsAnnualReview => manager != null && manager.AnnualReports.Count > 0 && !manager.AnnualReviewSeen;', 'public bool NeedsAnnualReview => true;'), (ROOT, 'using UnityEngine.UIElements;', 'using UnityEngine.UIElements;\n\ninternal sealed class ZDecoyTop { internal ScenarioOneManager manager; public bool NeedsAnnualReview => manager != null && manager.AnnualReports.Count > 0 && !manager.AnnualReviewSeen; }')], intended='(N-2 owning class)'),
    r314('A9-D8-struct-decoy-view', 'my_mutations9', 'rej', 'TeachingRacks forced true; reviewed text survives once in a nested struct of the view', [(VIEW, 'public bool TeachingRacks => CurrentTopicIndex == 0;', 'public bool TeachingRacks => true;\n    private struct ZDecoyStruct { public int CurrentTopicIndex => 0; public bool TeachingRacks => CurrentTopicIndex == 0; }')], intended='(N-2 owning class)'),
    r314('A9-D9-help-decoy-struct', 'my_mutations9', 'rej', 'help becomes a getter-null property; reviewed field text survives once in a nested struct', [(ROOT, 'private MenuHelpView help;', 'private MenuHelpView help { get => null; set { } }\n    private struct ZDecoyH { private MenuHelpView help; }')], intended='(N-2 owning class)'),
    r314('A9-D10-help-realistic', 'my_mutations9', 'rej', 'REALISTIC shape: help property that returns null only while the walking HUD is up (subtle guidance suppression), decoy nested class keeps the reviewed field', [(ROOT, 'private MenuHelpView help;', 'private MenuHelpView helpBacking;\n    private MenuHelpView help { get => screen == UiScreen.None ? null : helpBacking; set { helpBacking = value; } }\n    private sealed class ZDecoyH2 { private MenuHelpView help; }')], intended='(N-2 owning class)'),
    r314('A9-D11-learning-decoy', 'my_mutations9', 'rej', 'the older snippet pin `private LearningObjectivesView learning;`: learning becomes a property with no-op setter; decoy keeps the text', [(ROOT, 'private LearningObjectivesView learning;', 'private LearningObjectivesView learning { get => null; set { } }\n    private sealed class ZDecoyL { private LearningObjectivesView learning; }')]),
    r314('A9-D12-nested-enum-ctl', 'my_mutations9', 'ben', 'control: an extra nested enum in the root (no decoy) is accepted', [(ROOT, 'public enum UiScreen { None, Map, Review, Objectives }', 'public enum UiScreen { None, Map, Review, Objectives }\n    private enum ZColor { Red, Green }')]),
    # my_mutations10.py sha256 da204312ed5fa08b3cd177a35f39b5cfe915f90668d250121dded4c55bcbae93
    r314('A10-hole-tuple-help', 'my_mutations10', 'rej', 'write to help inside an interpolation hole, shape tuple', [(ROOT, 'public void ShowObjectives()', 'private void ZProbe()\n    {\n        Debug.Log($"{((help, _) = (null, 0))}");\n    }\n    private static int ZOut<T>(out T o) { o = default; return 0; }\n    private static int ZRef<T>(ref T a, T b) { a = b; return 0; }\n    \n    public void ShowObjectives()')], intended='inside an interpolation hole (N-1)'),
    r314('A10-hole-tuple2-help', 'my_mutations10', 'rej', 'write to help inside an interpolation hole, shape tuple2', [(ROOT, 'public void ShowObjectives()', 'private void ZProbe()\n    {\n        Debug.Log($"{(_, help) = (0, null)}");\n    }\n    private static int ZOut<T>(out T o) { o = default; return 0; }\n    private static int ZRef<T>(ref T a, T b) { a = b; return 0; }\n    \n    public void ShowObjectives()')], intended='inside an interpolation hole (N-1)'),
    r314('A10-hole-paren-help', 'my_mutations10', 'rej', 'write to help inside an interpolation hole, shape paren', [(ROOT, 'public void ShowObjectives()', 'private void ZProbe()\n    {\n        Debug.Log($"{(help) = null}");\n    }\n    private static int ZOut<T>(out T o) { o = default; return 0; }\n    private static int ZRef<T>(ref T a, T b) { a = b; return 0; }\n    \n    public void ShowObjectives()')], intended='inside an interpolation hole (N-1)'),
    r314('A10-hole-out-help', 'my_mutations10', 'rej', 'write to help inside an interpolation hole, shape out', [(ROOT, 'public void ShowObjectives()', 'private void ZProbe()\n    {\n        Debug.Log($"{ZOut(out help)}");\n    }\n    private static int ZOut<T>(out T o) { o = default; return 0; }\n    private static int ZRef<T>(ref T a, T b) { a = b; return 0; }\n    \n    public void ShowObjectives()')]),
    r314('A10-hole-ref-help', 'my_mutations10', 'rej', 'write to help inside an interpolation hole, shape ref', [(ROOT, 'public void ShowObjectives()', 'private void ZProbe()\n    {\n        Debug.Log($"{ZRef(ref help, null)}");\n    }\n    private static int ZOut<T>(out T o) { o = default; return 0; }\n    private static int ZRef<T>(ref T a, T b) { a = b; return 0; }\n    \n    public void ShowObjectives()')]),
    r314('A10-hole-coalesce-help', 'my_mutations10', 'rej', 'write to help inside an interpolation hole, shape coalesce', [(ROOT, 'public void ShowObjectives()', 'private void ZProbe()\n    {\n        Debug.Log($"{(help ??= null)}");\n    }\n    private static int ZOut<T>(out T o) { o = default; return 0; }\n    private static int ZRef<T>(ref T a, T b) { a = b; return 0; }\n    \n    public void ShowObjectives()')]),
    r314('A10-hole-verbatim-help', 'my_mutations10', 'rej', 'write to help inside an interpolation hole, shape verbatim', [(ROOT, 'public void ShowObjectives()', 'private void ZProbe()\n    {\n        Debug.Log($@"{((help, _) = (null, 0))}");\n    }\n    private static int ZOut<T>(out T o) { o = default; return 0; }\n    private static int ZRef<T>(ref T a, T b) { a = b; return 0; }\n    \n    public void ShowObjectives()')], intended='inside an interpolation hole (N-1)'),
    r314('A10-hole-nested-help', 'my_mutations10', 'rej', 'write to help inside an interpolation hole, shape nested', [(ROOT, 'public void ShowObjectives()', 'private void ZProbe()\n    {\n        Debug.Log($"a{string.Concat($"{((help, _) = (null, 0))}")}");\n    }\n    private static int ZOut<T>(out T o) { o = default; return 0; }\n    private static int ZRef<T>(ref T a, T b) { a = b; return 0; }\n    \n    public void ShowObjectives()')]),
    r314('A10-hole-tuple-manager', 'my_mutations10', 'rej', 'write to manager inside an interpolation hole, shape tuple', [(ROOT, 'public void ShowObjectives()', 'private void ZProbe()\n    {\n        Debug.Log($"{((manager, _) = (null, 0))}");\n    }\n    private static int ZOut<T>(out T o) { o = default; return 0; }\n    private static int ZRef<T>(ref T a, T b) { a = b; return 0; }\n    \n    public void ShowObjectives()')], intended='inside an interpolation hole (N-1)'),
    r314('A10-hole-tuple2-manager', 'my_mutations10', 'rej', 'write to manager inside an interpolation hole, shape tuple2', [(ROOT, 'public void ShowObjectives()', 'private void ZProbe()\n    {\n        Debug.Log($"{(_, manager) = (0, null)}");\n    }\n    private static int ZOut<T>(out T o) { o = default; return 0; }\n    private static int ZRef<T>(ref T a, T b) { a = b; return 0; }\n    \n    public void ShowObjectives()')], intended='inside an interpolation hole (N-1)'),
    r314('A10-hole-paren-manager', 'my_mutations10', 'rej', 'write to manager inside an interpolation hole, shape paren', [(ROOT, 'public void ShowObjectives()', 'private void ZProbe()\n    {\n        Debug.Log($"{(manager) = null}");\n    }\n    private static int ZOut<T>(out T o) { o = default; return 0; }\n    private static int ZRef<T>(ref T a, T b) { a = b; return 0; }\n    \n    public void ShowObjectives()')], intended='inside an interpolation hole (N-1)'),
    r314('A10-hole-out-manager', 'my_mutations10', 'rej', 'write to manager inside an interpolation hole, shape out', [(ROOT, 'public void ShowObjectives()', 'private void ZProbe()\n    {\n        Debug.Log($"{ZOut(out manager)}");\n    }\n    private static int ZOut<T>(out T o) { o = default; return 0; }\n    private static int ZRef<T>(ref T a, T b) { a = b; return 0; }\n    \n    public void ShowObjectives()')]),
    r314('A10-hole-ref-manager', 'my_mutations10', 'rej', 'write to manager inside an interpolation hole, shape ref', [(ROOT, 'public void ShowObjectives()', 'private void ZProbe()\n    {\n        Debug.Log($"{ZRef(ref manager, null)}");\n    }\n    private static int ZOut<T>(out T o) { o = default; return 0; }\n    private static int ZRef<T>(ref T a, T b) { a = b; return 0; }\n    \n    public void ShowObjectives()')]),
    r314('A10-hole-coalesce-manager', 'my_mutations10', 'rej', 'write to manager inside an interpolation hole, shape coalesce', [(ROOT, 'public void ShowObjectives()', 'private void ZProbe()\n    {\n        Debug.Log($"{(manager ??= null)}");\n    }\n    private static int ZOut<T>(out T o) { o = default; return 0; }\n    private static int ZRef<T>(ref T a, T b) { a = b; return 0; }\n    \n    public void ShowObjectives()')]),
    r314('A10-hole-verbatim-manager', 'my_mutations10', 'rej', 'write to manager inside an interpolation hole, shape verbatim', [(ROOT, 'public void ShowObjectives()', 'private void ZProbe()\n    {\n        Debug.Log($@"{((manager, _) = (null, 0))}");\n    }\n    private static int ZOut<T>(out T o) { o = default; return 0; }\n    private static int ZRef<T>(ref T a, T b) { a = b; return 0; }\n    \n    public void ShowObjectives()')], intended='inside an interpolation hole (N-1)'),
    r314('A10-hole-nested-manager', 'my_mutations10', 'rej', 'write to manager inside an interpolation hole, shape nested', [(ROOT, 'public void ShowObjectives()', 'private void ZProbe()\n    {\n        Debug.Log($"a{string.Concat($"{((manager, _) = (null, 0))}")}");\n    }\n    private static int ZOut<T>(out T o) { o = default; return 0; }\n    private static int ZRef<T>(ref T a, T b) { a = b; return 0; }\n    \n    public void ShowObjectives()')]),
    r314('A10-hole-tuple-map', 'my_mutations10', 'rej', 'write to map inside an interpolation hole, shape tuple', [(ROOT, 'public void ShowObjectives()', 'private void ZProbe()\n    {\n        Debug.Log($"{((map, _) = (null, 0))}");\n    }\n    private static int ZOut<T>(out T o) { o = default; return 0; }\n    private static int ZRef<T>(ref T a, T b) { a = b; return 0; }\n    \n    public void ShowObjectives()')], intended='inside an interpolation hole (N-1)'),
    r314('A10-hole-tuple2-map', 'my_mutations10', 'rej', 'write to map inside an interpolation hole, shape tuple2', [(ROOT, 'public void ShowObjectives()', 'private void ZProbe()\n    {\n        Debug.Log($"{(_, map) = (0, null)}");\n    }\n    private static int ZOut<T>(out T o) { o = default; return 0; }\n    private static int ZRef<T>(ref T a, T b) { a = b; return 0; }\n    \n    public void ShowObjectives()')], intended='inside an interpolation hole (N-1)'),
    r314('A10-hole-paren-map', 'my_mutations10', 'rej', 'write to map inside an interpolation hole, shape paren', [(ROOT, 'public void ShowObjectives()', 'private void ZProbe()\n    {\n        Debug.Log($"{(map) = null}");\n    }\n    private static int ZOut<T>(out T o) { o = default; return 0; }\n    private static int ZRef<T>(ref T a, T b) { a = b; return 0; }\n    \n    public void ShowObjectives()')], intended='inside an interpolation hole (N-1)'),
    r314('A10-hole-out-map', 'my_mutations10', 'rej', 'write to map inside an interpolation hole, shape out', [(ROOT, 'public void ShowObjectives()', 'private void ZProbe()\n    {\n        Debug.Log($"{ZOut(out map)}");\n    }\n    private static int ZOut<T>(out T o) { o = default; return 0; }\n    private static int ZRef<T>(ref T a, T b) { a = b; return 0; }\n    \n    public void ShowObjectives()')]),
    r314('A10-hole-ref-map', 'my_mutations10', 'rej', 'write to map inside an interpolation hole, shape ref', [(ROOT, 'public void ShowObjectives()', 'private void ZProbe()\n    {\n        Debug.Log($"{ZRef(ref map, null)}");\n    }\n    private static int ZOut<T>(out T o) { o = default; return 0; }\n    private static int ZRef<T>(ref T a, T b) { a = b; return 0; }\n    \n    public void ShowObjectives()')]),
    r314('A10-hole-coalesce-map', 'my_mutations10', 'rej', 'write to map inside an interpolation hole, shape coalesce', [(ROOT, 'public void ShowObjectives()', 'private void ZProbe()\n    {\n        Debug.Log($"{(map ??= null)}");\n    }\n    private static int ZOut<T>(out T o) { o = default; return 0; }\n    private static int ZRef<T>(ref T a, T b) { a = b; return 0; }\n    \n    public void ShowObjectives()')]),
    r314('A10-hole-verbatim-map', 'my_mutations10', 'rej', 'write to map inside an interpolation hole, shape verbatim', [(ROOT, 'public void ShowObjectives()', 'private void ZProbe()\n    {\n        Debug.Log($@"{((map, _) = (null, 0))}");\n    }\n    private static int ZOut<T>(out T o) { o = default; return 0; }\n    private static int ZRef<T>(ref T a, T b) { a = b; return 0; }\n    \n    public void ShowObjectives()')], intended='inside an interpolation hole (N-1)'),
    r314('A10-hole-nested-map', 'my_mutations10', 'rej', 'write to map inside an interpolation hole, shape nested', [(ROOT, 'public void ShowObjectives()', 'private void ZProbe()\n    {\n        Debug.Log($"a{string.Concat($"{((map, _) = (null, 0))}")}");\n    }\n    private static int ZOut<T>(out T o) { o = default; return 0; }\n    private static int ZRef<T>(ref T a, T b) { a = b; return 0; }\n    \n    public void ShowObjectives()')]),
    r314('A10-hole-tuple-workPlan', 'my_mutations10', 'rej', 'write to workPlan inside an interpolation hole, shape tuple', [(ROOT, 'public void ShowObjectives()', 'private void ZProbe()\n    {\n        Debug.Log($"{((workPlan, _) = (null, 0))}");\n    }\n    private static int ZOut<T>(out T o) { o = default; return 0; }\n    private static int ZRef<T>(ref T a, T b) { a = b; return 0; }\n    \n    public void ShowObjectives()')], intended='inside an interpolation hole (N-1)'),
    r314('A10-hole-tuple2-workPlan', 'my_mutations10', 'rej', 'write to workPlan inside an interpolation hole, shape tuple2', [(ROOT, 'public void ShowObjectives()', 'private void ZProbe()\n    {\n        Debug.Log($"{(_, workPlan) = (0, null)}");\n    }\n    private static int ZOut<T>(out T o) { o = default; return 0; }\n    private static int ZRef<T>(ref T a, T b) { a = b; return 0; }\n    \n    public void ShowObjectives()')], intended='inside an interpolation hole (N-1)'),
    r314('A10-hole-paren-workPlan', 'my_mutations10', 'rej', 'write to workPlan inside an interpolation hole, shape paren', [(ROOT, 'public void ShowObjectives()', 'private void ZProbe()\n    {\n        Debug.Log($"{(workPlan) = null}");\n    }\n    private static int ZOut<T>(out T o) { o = default; return 0; }\n    private static int ZRef<T>(ref T a, T b) { a = b; return 0; }\n    \n    public void ShowObjectives()')], intended='inside an interpolation hole (N-1)'),
    r314('A10-hole-out-workPlan', 'my_mutations10', 'rej', 'write to workPlan inside an interpolation hole, shape out', [(ROOT, 'public void ShowObjectives()', 'private void ZProbe()\n    {\n        Debug.Log($"{ZOut(out workPlan)}");\n    }\n    private static int ZOut<T>(out T o) { o = default; return 0; }\n    private static int ZRef<T>(ref T a, T b) { a = b; return 0; }\n    \n    public void ShowObjectives()')]),
    r314('A10-hole-ref-workPlan', 'my_mutations10', 'rej', 'write to workPlan inside an interpolation hole, shape ref', [(ROOT, 'public void ShowObjectives()', 'private void ZProbe()\n    {\n        Debug.Log($"{ZRef(ref workPlan, null)}");\n    }\n    private static int ZOut<T>(out T o) { o = default; return 0; }\n    private static int ZRef<T>(ref T a, T b) { a = b; return 0; }\n    \n    public void ShowObjectives()')]),
    r314('A10-hole-coalesce-workPlan', 'my_mutations10', 'rej', 'write to workPlan inside an interpolation hole, shape coalesce', [(ROOT, 'public void ShowObjectives()', 'private void ZProbe()\n    {\n        Debug.Log($"{(workPlan ??= null)}");\n    }\n    private static int ZOut<T>(out T o) { o = default; return 0; }\n    private static int ZRef<T>(ref T a, T b) { a = b; return 0; }\n    \n    public void ShowObjectives()')]),
    r314('A10-hole-verbatim-workPlan', 'my_mutations10', 'rej', 'write to workPlan inside an interpolation hole, shape verbatim', [(ROOT, 'public void ShowObjectives()', 'private void ZProbe()\n    {\n        Debug.Log($@"{((workPlan, _) = (null, 0))}");\n    }\n    private static int ZOut<T>(out T o) { o = default; return 0; }\n    private static int ZRef<T>(ref T a, T b) { a = b; return 0; }\n    \n    public void ShowObjectives()')], intended='inside an interpolation hole (N-1)'),
    r314('A10-hole-nested-workPlan', 'my_mutations10', 'rej', 'write to workPlan inside an interpolation hole, shape nested', [(ROOT, 'public void ShowObjectives()', 'private void ZProbe()\n    {\n        Debug.Log($"a{string.Concat($"{((workPlan, _) = (null, 0))}")}");\n    }\n    private static int ZOut<T>(out T o) { o = default; return 0; }\n    private static int ZRef<T>(ref T a, T b) { a = b; return 0; }\n    \n    public void ShowObjectives()')]),
    r314('A10-hole-tuple-review', 'my_mutations10', 'rej', 'write to review inside an interpolation hole, shape tuple', [(ROOT, 'public void ShowObjectives()', 'private void ZProbe()\n    {\n        Debug.Log($"{((review, _) = (null, 0))}");\n    }\n    private static int ZOut<T>(out T o) { o = default; return 0; }\n    private static int ZRef<T>(ref T a, T b) { a = b; return 0; }\n    \n    public void ShowObjectives()')], intended='inside an interpolation hole (N-1)'),
    r314('A10-hole-tuple2-review', 'my_mutations10', 'rej', 'write to review inside an interpolation hole, shape tuple2', [(ROOT, 'public void ShowObjectives()', 'private void ZProbe()\n    {\n        Debug.Log($"{(_, review) = (0, null)}");\n    }\n    private static int ZOut<T>(out T o) { o = default; return 0; }\n    private static int ZRef<T>(ref T a, T b) { a = b; return 0; }\n    \n    public void ShowObjectives()')], intended='inside an interpolation hole (N-1)'),
    r314('A10-hole-paren-review', 'my_mutations10', 'rej', 'write to review inside an interpolation hole, shape paren', [(ROOT, 'public void ShowObjectives()', 'private void ZProbe()\n    {\n        Debug.Log($"{(review) = null}");\n    }\n    private static int ZOut<T>(out T o) { o = default; return 0; }\n    private static int ZRef<T>(ref T a, T b) { a = b; return 0; }\n    \n    public void ShowObjectives()')], intended='inside an interpolation hole (N-1)'),
    r314('A10-hole-out-review', 'my_mutations10', 'rej', 'write to review inside an interpolation hole, shape out', [(ROOT, 'public void ShowObjectives()', 'private void ZProbe()\n    {\n        Debug.Log($"{ZOut(out review)}");\n    }\n    private static int ZOut<T>(out T o) { o = default; return 0; }\n    private static int ZRef<T>(ref T a, T b) { a = b; return 0; }\n    \n    public void ShowObjectives()')]),
    r314('A10-hole-ref-review', 'my_mutations10', 'rej', 'write to review inside an interpolation hole, shape ref', [(ROOT, 'public void ShowObjectives()', 'private void ZProbe()\n    {\n        Debug.Log($"{ZRef(ref review, null)}");\n    }\n    private static int ZOut<T>(out T o) { o = default; return 0; }\n    private static int ZRef<T>(ref T a, T b) { a = b; return 0; }\n    \n    public void ShowObjectives()')]),
    r314('A10-hole-coalesce-review', 'my_mutations10', 'rej', 'write to review inside an interpolation hole, shape coalesce', [(ROOT, 'public void ShowObjectives()', 'private void ZProbe()\n    {\n        Debug.Log($"{(review ??= null)}");\n    }\n    private static int ZOut<T>(out T o) { o = default; return 0; }\n    private static int ZRef<T>(ref T a, T b) { a = b; return 0; }\n    \n    public void ShowObjectives()')]),
    r314('A10-hole-verbatim-review', 'my_mutations10', 'rej', 'write to review inside an interpolation hole, shape verbatim', [(ROOT, 'public void ShowObjectives()', 'private void ZProbe()\n    {\n        Debug.Log($@"{((review, _) = (null, 0))}");\n    }\n    private static int ZOut<T>(out T o) { o = default; return 0; }\n    private static int ZRef<T>(ref T a, T b) { a = b; return 0; }\n    \n    public void ShowObjectives()')], intended='inside an interpolation hole (N-1)'),
    r314('A10-hole-nested-review', 'my_mutations10', 'rej', 'write to review inside an interpolation hole, shape nested', [(ROOT, 'public void ShowObjectives()', 'private void ZProbe()\n    {\n        Debug.Log($"a{string.Concat($"{((review, _) = (null, 0))}")}");\n    }\n    private static int ZOut<T>(out T o) { o = default; return 0; }\n    private static int ZRef<T>(ref T a, T b) { a = b; return 0; }\n    \n    public void ShowObjectives()')]),
    r314('A10-hole-tuple-rackLayout', 'my_mutations10', 'rej', 'write to rackLayout inside an interpolation hole, shape tuple', [(ROOT, 'public void ShowObjectives()', 'private void ZProbe()\n    {\n        Debug.Log($"{((rackLayout, _) = (null, 0))}");\n    }\n    private static int ZOut<T>(out T o) { o = default; return 0; }\n    private static int ZRef<T>(ref T a, T b) { a = b; return 0; }\n    \n    public void ShowObjectives()')], intended='inside an interpolation hole (N-1)'),
    r314('A10-hole-tuple2-rackLayout', 'my_mutations10', 'rej', 'write to rackLayout inside an interpolation hole, shape tuple2', [(ROOT, 'public void ShowObjectives()', 'private void ZProbe()\n    {\n        Debug.Log($"{(_, rackLayout) = (0, null)}");\n    }\n    private static int ZOut<T>(out T o) { o = default; return 0; }\n    private static int ZRef<T>(ref T a, T b) { a = b; return 0; }\n    \n    public void ShowObjectives()')], intended='inside an interpolation hole (N-1)'),
    r314('A10-hole-paren-rackLayout', 'my_mutations10', 'rej', 'write to rackLayout inside an interpolation hole, shape paren', [(ROOT, 'public void ShowObjectives()', 'private void ZProbe()\n    {\n        Debug.Log($"{(rackLayout) = null}");\n    }\n    private static int ZOut<T>(out T o) { o = default; return 0; }\n    private static int ZRef<T>(ref T a, T b) { a = b; return 0; }\n    \n    public void ShowObjectives()')], intended='inside an interpolation hole (N-1)'),
    r314('A10-hole-out-rackLayout', 'my_mutations10', 'rej', 'write to rackLayout inside an interpolation hole, shape out', [(ROOT, 'public void ShowObjectives()', 'private void ZProbe()\n    {\n        Debug.Log($"{ZOut(out rackLayout)}");\n    }\n    private static int ZOut<T>(out T o) { o = default; return 0; }\n    private static int ZRef<T>(ref T a, T b) { a = b; return 0; }\n    \n    public void ShowObjectives()')]),
    r314('A10-hole-ref-rackLayout', 'my_mutations10', 'rej', 'write to rackLayout inside an interpolation hole, shape ref', [(ROOT, 'public void ShowObjectives()', 'private void ZProbe()\n    {\n        Debug.Log($"{ZRef(ref rackLayout, null)}");\n    }\n    private static int ZOut<T>(out T o) { o = default; return 0; }\n    private static int ZRef<T>(ref T a, T b) { a = b; return 0; }\n    \n    public void ShowObjectives()')]),
    r314('A10-hole-coalesce-rackLayout', 'my_mutations10', 'rej', 'write to rackLayout inside an interpolation hole, shape coalesce', [(ROOT, 'public void ShowObjectives()', 'private void ZProbe()\n    {\n        Debug.Log($"{(rackLayout ??= null)}");\n    }\n    private static int ZOut<T>(out T o) { o = default; return 0; }\n    private static int ZRef<T>(ref T a, T b) { a = b; return 0; }\n    \n    public void ShowObjectives()')]),
    r314('A10-hole-verbatim-rackLayout', 'my_mutations10', 'rej', 'write to rackLayout inside an interpolation hole, shape verbatim', [(ROOT, 'public void ShowObjectives()', 'private void ZProbe()\n    {\n        Debug.Log($@"{((rackLayout, _) = (null, 0))}");\n    }\n    private static int ZOut<T>(out T o) { o = default; return 0; }\n    private static int ZRef<T>(ref T a, T b) { a = b; return 0; }\n    \n    public void ShowObjectives()')], intended='inside an interpolation hole (N-1)'),
    r314('A10-hole-nested-rackLayout', 'my_mutations10', 'rej', 'write to rackLayout inside an interpolation hole, shape nested', [(ROOT, 'public void ShowObjectives()', 'private void ZProbe()\n    {\n        Debug.Log($"a{string.Concat($"{((rackLayout, _) = (null, 0))}")}");\n    }\n    private static int ZOut<T>(out T o) { o = default; return 0; }\n    private static int ZRef<T>(ref T a, T b) { a = b; return 0; }\n    \n    public void ShowObjectives()')]),
    r314('A10-hole-tuple-screen', 'my_mutations10', 'rej', 'write to screen inside an interpolation hole, shape tuple', [(ROOT, 'public void ShowObjectives()', 'private void ZProbe()\n    {\n        Debug.Log($"{((screen, _) = (UiScreen.None, 0))}");\n    }\n    private static int ZOut<T>(out T o) { o = default; return 0; }\n    private static int ZRef<T>(ref T a, T b) { a = b; return 0; }\n    \n    public void ShowObjectives()')], intended='inside an interpolation hole (N-1)'),
    r314('A10-hole-tuple2-screen', 'my_mutations10', 'rej', 'write to screen inside an interpolation hole, shape tuple2', [(ROOT, 'public void ShowObjectives()', 'private void ZProbe()\n    {\n        Debug.Log($"{(_, screen) = (0, UiScreen.None)}");\n    }\n    private static int ZOut<T>(out T o) { o = default; return 0; }\n    private static int ZRef<T>(ref T a, T b) { a = b; return 0; }\n    \n    public void ShowObjectives()')], intended='inside an interpolation hole (N-1)'),
    r314('A10-hole-paren-screen', 'my_mutations10', 'rej', 'write to screen inside an interpolation hole, shape paren', [(ROOT, 'public void ShowObjectives()', 'private void ZProbe()\n    {\n        Debug.Log($"{(screen) = UiScreen.None}");\n    }\n    private static int ZOut<T>(out T o) { o = default; return 0; }\n    private static int ZRef<T>(ref T a, T b) { a = b; return 0; }\n    \n    public void ShowObjectives()')], intended='inside an interpolation hole (N-1)'),
    r314('A10-hole-out-screen', 'my_mutations10', 'rej', 'write to screen inside an interpolation hole, shape out', [(ROOT, 'public void ShowObjectives()', 'private void ZProbe()\n    {\n        Debug.Log($"{ZOut(out screen)}");\n    }\n    private static int ZOut<T>(out T o) { o = default; return 0; }\n    private static int ZRef<T>(ref T a, T b) { a = b; return 0; }\n    \n    public void ShowObjectives()')]),
    r314('A10-hole-ref-screen', 'my_mutations10', 'rej', 'write to screen inside an interpolation hole, shape ref', [(ROOT, 'public void ShowObjectives()', 'private void ZProbe()\n    {\n        Debug.Log($"{ZRef(ref screen, UiScreen.None)}");\n    }\n    private static int ZOut<T>(out T o) { o = default; return 0; }\n    private static int ZRef<T>(ref T a, T b) { a = b; return 0; }\n    \n    public void ShowObjectives()')]),
    r314('A10-hole-coalesce-screen', 'my_mutations10', 'rej', 'write to screen inside an interpolation hole, shape coalesce', [(ROOT, 'public void ShowObjectives()', 'private void ZProbe()\n    {\n        Debug.Log($"{(screen ??= UiScreen.None)}");\n    }\n    private static int ZOut<T>(out T o) { o = default; return 0; }\n    private static int ZRef<T>(ref T a, T b) { a = b; return 0; }\n    \n    public void ShowObjectives()')]),
    r314('A10-hole-verbatim-screen', 'my_mutations10', 'rej', 'write to screen inside an interpolation hole, shape verbatim', [(ROOT, 'public void ShowObjectives()', 'private void ZProbe()\n    {\n        Debug.Log($@"{((screen, _) = (UiScreen.None, 0))}");\n    }\n    private static int ZOut<T>(out T o) { o = default; return 0; }\n    private static int ZRef<T>(ref T a, T b) { a = b; return 0; }\n    \n    public void ShowObjectives()')], intended='inside an interpolation hole (N-1)'),
    r314('A10-hole-nested-screen', 'my_mutations10', 'rej', 'write to screen inside an interpolation hole, shape nested', [(ROOT, 'public void ShowObjectives()', 'private void ZProbe()\n    {\n        Debug.Log($"a{string.Concat($"{((screen, _) = (UiScreen.None, 0))}")}");\n    }\n    private static int ZOut<T>(out T o) { o = default; return 0; }\n    private static int ZRef<T>(ref T a, T b) { a = b; return 0; }\n    \n    public void ShowObjectives()')]),
    # my_mutations11.py sha256 62d1b709da4e7c4510d88d04c05498831b026bddb6f9d4a6d9849ca1f53117a3
    r314('A11-hole-outthis-help', 'my_mutations11', 'rej', 'hole: ZOut(out this.help)', [(ROOT, 'public void ShowObjectives()', 'private void ZProbe()\n    {\n        Debug.Log($"{ZOut(out this.help)}");\n    }\n    private static int ZOut<T>(out T o) { o = default; return 0; }\n    private static int ZRef<T>(ref T a, T b) { a = b; return 0; }\n    \n    public void ShowObjectives()')], intended='inside an interpolation hole (N-1)'),
    r314('A11-hole-refthis-help', 'my_mutations11', 'rej', 'hole: ZRef(ref this.help, null)', [(ROOT, 'public void ShowObjectives()', 'private void ZProbe()\n    {\n        Debug.Log($"{ZRef(ref this.help, null)}");\n    }\n    private static int ZOut<T>(out T o) { o = default; return 0; }\n    private static int ZRef<T>(ref T a, T b) { a = b; return 0; }\n    \n    public void ShowObjectives()')], intended='inside an interpolation hole (N-1)'),
    r314('A11-hole-outthis-manager', 'my_mutations11', 'rej', 'hole: ZOut(out this.manager)', [(ROOT, 'public void ShowObjectives()', 'private void ZProbe()\n    {\n        Debug.Log($"{ZOut(out this.manager)}");\n    }\n    private static int ZOut<T>(out T o) { o = default; return 0; }\n    private static int ZRef<T>(ref T a, T b) { a = b; return 0; }\n    \n    public void ShowObjectives()')], intended='inside an interpolation hole (N-1)'),
    r314('A11-hole-refthis-manager', 'my_mutations11', 'rej', 'hole: ZRef(ref this.manager, null)', [(ROOT, 'public void ShowObjectives()', 'private void ZProbe()\n    {\n        Debug.Log($"{ZRef(ref this.manager, null)}");\n    }\n    private static int ZOut<T>(out T o) { o = default; return 0; }\n    private static int ZRef<T>(ref T a, T b) { a = b; return 0; }\n    \n    public void ShowObjectives()')], intended='inside an interpolation hole (N-1)'),
    r314('A11-hole-outthis-map', 'my_mutations11', 'rej', 'hole: ZOut(out this.map)', [(ROOT, 'public void ShowObjectives()', 'private void ZProbe()\n    {\n        Debug.Log($"{ZOut(out this.map)}");\n    }\n    private static int ZOut<T>(out T o) { o = default; return 0; }\n    private static int ZRef<T>(ref T a, T b) { a = b; return 0; }\n    \n    public void ShowObjectives()')], intended='inside an interpolation hole (N-1)'),
    r314('A11-hole-refthis-map', 'my_mutations11', 'rej', 'hole: ZRef(ref this.map, null)', [(ROOT, 'public void ShowObjectives()', 'private void ZProbe()\n    {\n        Debug.Log($"{ZRef(ref this.map, null)}");\n    }\n    private static int ZOut<T>(out T o) { o = default; return 0; }\n    private static int ZRef<T>(ref T a, T b) { a = b; return 0; }\n    \n    public void ShowObjectives()')], intended='inside an interpolation hole (N-1)'),
    r314('A11-hole-outthis-review', 'my_mutations11', 'rej', 'hole: ZOut(out this.review)', [(ROOT, 'public void ShowObjectives()', 'private void ZProbe()\n    {\n        Debug.Log($"{ZOut(out this.review)}");\n    }\n    private static int ZOut<T>(out T o) { o = default; return 0; }\n    private static int ZRef<T>(ref T a, T b) { a = b; return 0; }\n    \n    public void ShowObjectives()')], intended='inside an interpolation hole (N-1)'),
    r314('A11-hole-refthis-review', 'my_mutations11', 'rej', 'hole: ZRef(ref this.review, null)', [(ROOT, 'public void ShowObjectives()', 'private void ZProbe()\n    {\n        Debug.Log($"{ZRef(ref this.review, null)}");\n    }\n    private static int ZOut<T>(out T o) { o = default; return 0; }\n    private static int ZRef<T>(ref T a, T b) { a = b; return 0; }\n    \n    public void ShowObjectives()')], intended='inside an interpolation hole (N-1)'),
    r314('A11-hole-outthis-rackLayout', 'my_mutations11', 'rej', 'hole: ZOut(out this.rackLayout)', [(ROOT, 'public void ShowObjectives()', 'private void ZProbe()\n    {\n        Debug.Log($"{ZOut(out this.rackLayout)}");\n    }\n    private static int ZOut<T>(out T o) { o = default; return 0; }\n    private static int ZRef<T>(ref T a, T b) { a = b; return 0; }\n    \n    public void ShowObjectives()')], intended='inside an interpolation hole (N-1)'),
    r314('A11-hole-refthis-rackLayout', 'my_mutations11', 'rej', 'hole: ZRef(ref this.rackLayout, null)', [(ROOT, 'public void ShowObjectives()', 'private void ZProbe()\n    {\n        Debug.Log($"{ZRef(ref this.rackLayout, null)}");\n    }\n    private static int ZOut<T>(out T o) { o = default; return 0; }\n    private static int ZRef<T>(ref T a, T b) { a = b; return 0; }\n    \n    public void ShowObjectives()')], intended='inside an interpolation hole (N-1)'),
    r314('A11-hole-outthis-workPlan', 'my_mutations11', 'rej', 'hole: ZOut(out this.workPlan)', [(ROOT, 'public void ShowObjectives()', 'private void ZProbe()\n    {\n        Debug.Log($"{ZOut(out this.workPlan)}");\n    }\n    private static int ZOut<T>(out T o) { o = default; return 0; }\n    private static int ZRef<T>(ref T a, T b) { a = b; return 0; }\n    \n    public void ShowObjectives()')], intended='inside an interpolation hole (N-1)'),
    r314('A11-hole-refthis-workPlan', 'my_mutations11', 'rej', 'hole: ZRef(ref this.workPlan, null)', [(ROOT, 'public void ShowObjectives()', 'private void ZProbe()\n    {\n        Debug.Log($"{ZRef(ref this.workPlan, null)}");\n    }\n    private static int ZOut<T>(out T o) { o = default; return 0; }\n    private static int ZRef<T>(ref T a, T b) { a = b; return 0; }\n    \n    public void ShowObjectives()')], intended='inside an interpolation hole (N-1)'),
    r314('A11-hole-prefixinc-screen', 'my_mutations11', 'rej', 'hole: ++screen', [(ROOT, 'public void ShowObjectives()', 'private void ZProbe()\n    {\n        Debug.Log($"{++screen}");\n    }\n    public void ShowObjectives()')], intended='inside an interpolation hole (N-1)'),
    r314('A11-hole-prefixdec-screen', 'my_mutations11', 'rej', 'hole: --screen', [(ROOT, 'public void ShowObjectives()', 'private void ZProbe()\n    {\n        Debug.Log($"{--screen}");\n    }\n    public void ShowObjectives()')], intended='inside an interpolation hole (N-1)'),
    r314('A11-called-hole-help', 'my_mutations11', 'rej', 'ShowObjectives ends with Debug.Log($"{((help, workPlanWasOpen) = (null, false))}") - the original Z-RR2-T-help effect, hidden in a hole', [(ROOT, 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);', 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);\n        Debug.Log($"{((help, workPlanWasOpen) = (null, false))}");')], intended='inside an interpolation hole (N-1)'),
    r314('A11-called-hole-screen', 'my_mutations11', 'rej', 'ShowObjectives: tuple write of screen in a hole after SetScreen(Objectives): Objectives screen resolves guidance against the map', [(ROOT, 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);\n        SetScreen(UiScreen.Objectives);', 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);\n        SetScreen(UiScreen.Objectives);\n        Debug.Log($"{((screen, workPlanWasOpen) = (UiScreen.Map, false))}");')], intended='inside an interpolation hole (N-1)'),
    r314('A11-called-hole-manager', 'my_mutations11', 'rej', 'ShowObjectives: tuple write of manager (null) in a hole', [(ROOT, 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);', 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);\n        Debug.Log($"{((manager, workPlanWasOpen) = (null, false))}");')], intended='inside an interpolation hole (N-1)'),
    r314('A11-called-hole-paren-help', 'my_mutations11', 'rej', 'ShowObjectives: parenthesised write of help in a hole', [(ROOT, 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);', 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);\n        Debug.Log($"{((help) = null)}");')], intended='inside an interpolation hole (N-1)'),
    r314('A11-called-stmt-ctl-help', 'my_mutations11', 'rej', 'CONTROL: the same tuple write as a plain statement (the original Z-RR2-T-help)', [(ROOT, 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);', 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);\n        (help, workPlanWasOpen) = (null, false);')]),
    r314('A11-called-lambda-hole-help', 'my_mutations11', 'rej', 'ShowObjectives: tuple write of help in a hole inside a lambda that is invoked', [(ROOT, 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);', 'public void ShowObjectives()\n    {\n        manager.OpenWorkPlan(false);\n        System.Func<string> zf = () => $"{((help, workPlanWasOpen) = (null, false))}"; zf();')], intended='inside an interpolation hole (N-1)'),
    r314('A11-hole-tuple-guarded', 'my_mutations11', 'rej', 'hole: tuple write of the guarded field guidanceCandidate (existing #309 mechanism)', [(ROOT, 'public void ShowObjectives()', 'private void ZProbe()\n    {\n        Debug.Log($"{((guidanceCandidate, workPlanWasOpen) = (null, false))}");\n    }\n    public void ShowObjectives()')]),
    r314('A11-hole-tuple-helpClosedFrame', 'my_mutations11', 'rej', 'hole: tuple write of helpClosedFrame', [(ROOT, 'public void ShowObjectives()', 'private void ZProbe()\n    {\n        Debug.Log($"{((helpClosedFrame, workPlanWasOpen) = (0, false))}");\n    }\n    public void ShowObjectives()')]),
    r314('A11-hole-this-assign-nextAction', 'my_mutations11', 'rej', 'hole: this.nextAction = null', [(ROOT, 'public void ShowObjectives()', 'private void ZProbe()\n    {\n        Debug.Log($"{(this.nextAction = null)}");\n    }\n    public void ShowObjectives()')]),
]


def _r314_round7():
    """my_mutations7.py (sha256 72ff0cd3eb4da68e8de0b67fb5afa2bf36a18fc5452739ad733f935ccc81ce08) generator,
    transcribed: the write-shape matrix against the related-state guard. All rej; 3 probes."""
    anch_class, anch_start, anch_destroy = 'public void ShowObjectives()', 'root.Add(help.Root);', 'private void OnDestroy()\n    {'
    fields = {"help": ("MenuHelpView", "null"), "manager": ("ScenarioOneManager", "null"),
              "learning": ("LearningObjectivesView", "null"), "map": ("StandMapView", "null"),
              "workPlan": ("WorkPlanView", "null"), "review": ("AnnualReviewView", "null"),
              "rackLayout": ("RackLayoutTracker", "null"), "screen": ("UiScreen", "UiScreen.None")}
    helpers = ("private static void ZSet<T>(ref T a, T b) { a = b; }\n    private static void ZOut<T>(out T o) { o = default; }\n    ")
    shapes = {
        "plain": "{F} = {V};", "this": "this.{F} = {V};", "paren": "({F}) = {V};", "tupl": "({F}, _) = ({V}, 0);",
        "tupr": "(_, {F}) = (0, {V});", "thistup": "(this.{F}, _) = ({V}, 0);", "nesttup": "(({F}, _), _) = (({V}, 0), 0);",
        "lambda": "System.Action za = () => {F} = {V}; za();", "refarg": "ZSet(ref {F}, {V});", "outarg": "ZOut(out {F});",
        "outthis": "ZOut(out this.{F});", "reflocal": "ref {T} zr = ref {F}; zr = {V};",
        "alias": "{ var zself = this; zself.{F} = {V}; }", "cast": "((ScenarioOneUiRoot)this).{F} = {V};",
        "verb": "@{F} = {V};", "cmt": "{F} /*c*/ = {V};", "nl": "{F}\n        = {V};", "nbsp": "{F}\xa0= {V};",
        "chain": "var zq = {F} = {V};", "forinit": "for ({F} = {V}; false; ) { }", "ifasg": "if (({F} = {V}) == null) { }",
        "localfn": "ZLocal(); void ZLocal() { {F} = {V}; }", "interp": 'Debug.Log($"{({F} = {V})}");',
        "unsafeas": "System.Runtime.CompilerServices.Unsafe.AsRef(in {F}) = {V};", "makeref": "var zm = __makeref({F});",
        "nullcoal": "{F} ??= {V};",
    }

    def case(field, shape, place):
        typ, val = fields[field]
        if shape == "nullcoal" and field == "screen":
            return None
        text = shapes[shape].replace("{F}", field).replace("{V}", val).replace("{T}", typ)
        if place == "m":
            edits = [(ROOT, anch_class, f"private void ZProbe()\n    {{\n        {text}\n    }}\n    {helpers}" + "\n    " + anch_class)]
        else:
            edits = [(ROOT, anch_start if place == "s" else anch_destroy, (anch_start if place == "s" else anch_destroy) + "\n        " + text)]
            if shape in ("refarg", "outarg", "outthis"):
                edits.append((ROOT, anch_class, helpers + "\n    " + anch_class))
        return r314(f"A7-{place}-{shape}-{field}", "my_mutations7", "rej", f"{place}: write shape '{shape}' to related state '{field}'", edits)

    cases = [case(f, s, "m") for f in fields for s in shapes]
    cases += [case(f, s, p) for f in fields for s in ("plain", "paren", "tupl", "lambda", "refarg", "this") for p in ("s", "d")]
    for name, text in (("inc", "screen++;"), ("pre", "++screen;"), ("or", "screen |= UiScreen.None;"), ("plus", "screen += 0;"),
                       ("minus", "screen -= 0;"), ("xor", "screen ^= UiScreen.None;"), ("and", "screen &= UiScreen.Objectives;")):
        cases.append(r314(f"A7-m-screen-{name}", "my_mutations7", "rej", f"screen written by '{text}'",
                          [(ROOT, anch_class, f"private void ZProbe()\n    {{\n        {text}\n    }}\n\n    " + anch_class)]))
    for f in ("help", "screen", "manager"):
        t, v = fields[f]
        for name, body in (
                ("setter", f"private {t} ZP {{ set {{ {f} = value; }} }}"),
                ("init", f"private {t} ZI {{ get => {f}; init {{ {f} = value; }} }}"),
                ("expr", f"private void ZE() => {f} = {v};"),
                ("nested", f"private sealed class ZN {{ public void W(ScenarioOneUiRoot r) {{ r.{f} = {v}; }} }}"),
                ("nestedref", f"private sealed class ZN2 {{ public void W(ScenarioOneUiRoot r) {{ ref {t} x = ref r.{f}; x = {v}; }} }}"),
                ("static", f"private static void ZS(ScenarioOneUiRoot r) {{ r.{f} = {v}; }}"),
                ("refret", f"private ref {t} ZR() => ref {f};")):
            cases.append(r314(f"A7-c-{name}-{f}", "my_mutations7", "rej", f"class-scope shape '{name}' writes or exposes '{f}'",
                              [(ROOT, anch_class, body + "\n\n    " + anch_class)]))
    for g, v in {"helpClosedFrame": "0", "nextAction": "null", "guidanceCandidate": "null", "guidanceScope": "null"}.items():
        for name, text in (("plain", f"{g} = {v};"), ("tup", f"({g}, _) = ({v}, 0);"),
                           ("lambda", f"System.Action zg = () => {g} = {v}; zg();"), ("this", f"this.{g} = {v};")):
            cases.append(r314(f"A7-g-{name}-{g}", "my_mutations7", "rej", f"guarded name {g}: {name}",
                              [(ROOT, anch_class, f"private void ZProbe()\n    {{\n        {text}\n    }}\n\n    " + anch_class)]))
    cases += [
        r314("A7-x-update-setscreen", "my_mutations7", "lim",
             "unpinned Update() sends Objectives to the Map every frame (unpinned screen routing; disclosed class?)",
             [(ROOT, "private void Update()\n    {\n        if (manager == null || document == null)\n            return;",
               "private void Update()\n    {\n        if (screen == UiScreen.Objectives) SetScreen(UiScreen.Map);\n"
               "        if (manager == null || document == null)\n            return;")], documented="accepted"),
        # Accepted at 753143e6 (reviewer: lim). #320 N-2 rejects it: a nested type of the view shadows a name its reviewed text reads.
        r314("A7-x-teachinghijack", "my_mutations7", "lim",
             "nested class in a view partial hijacks ScenarioOneTeachingSequence (Current always 0): TeachingRacks forever true",
             [(L1_SEQ, "private int teachingWorldVersion = -1;", "private int teachingWorldVersion = -1;\n"
               "    private static class ScenarioOneTeachingSequence { public static int Current(object facts) => 0; }")],
             documented="rejected"),
        r314("A7-x-enabled-false", "my_mutations7", "lim", "Awake sets enabled=false in the root (component state; disclosed)",
             [(ROOT, anch_class, "private void Awake() { enabled = false; }\n\n    " + anch_class)], documented="accepted"),
    ]
    return [c for c in cases if c]


# ---- this repair's own variants (derived independently of the reviewer's scratch guards) -------------
N320_CLASS = 'public void ShowObjectives()'
N320_START = 'root.Add(help.Root);'
N320_USING = 'using UnityEngine.UIElements;'
N320_HELPERS = ('private static int ZOut<T>(out T o) { o = default; return 0; }\n    '
                'private static int ZRef<T>(ref T a, T b) { a = b; return 0; }\n    '
                'private static int ZIn<T>(in T a) => 0;\n    ')


def n320(mid, why, *edits, intended=None, documented=None):
    return dict(id=mid, group="#320", why=why, edits=list(edits), intended=intended, documented=documented, transform=None)


def _in_show_objectives(statement):
    return (ROOT, L1_SHOW_OBJ, L1_SHOW_OBJ + "\n        " + statement)


def _probe_method(statement):
    return (ROOT, N320_CLASS, "private void ZProbe()\n    {\n        " + statement + "\n    }\n    " + N320_HELPERS + "\n    " + N320_CLASS)


n320_n1 = [
    n320("N1-01", "parenthesised this-qualified write in a hole, called from ShowObjectives",
         _in_show_objectives('Debug.Log($"{((this.help) = null)}");'), intended=N1),
    n320("N1-02", "@$ prefix order: tuple write of help in a hole",
         _in_show_objectives('Debug.Log(@$"{((help, workPlanWasOpen) = (null, false))}");'), intended=N1),
    n320("N1-03", "multi-line verbatim hole: tuple write of map spread over lines",
         _in_show_objectives('Debug.Log($@"x{\n            ((map, workPlanWasOpen) = (null, false))\n        }y");'), intended=N1),
    n320("N1-04", "statement lambda inside a hole writes review",
         _in_show_objectives('Debug.Log($"{((System.Func<int>)(() => { (review, workPlanWasOpen) = (null, false); return 0; }))()}");'),
         intended=N1),
    n320("N1-05", "nested class writes help through a receiver inside a hole",
         (ROOT, N320_CLASS, 'private sealed class ZHoleWriter { public string W(ScenarioOneUiRoot r) => $"{(r.help = null)}"; }\n\n    '
          + N320_CLASS), intended="receiver other than `this` inside an interpolation hole (N-1)"),
    n320("N1-06", "compound assignment of screen in a hole", _probe_method('Debug.Log($"{(screen |= UiScreen.None)}");'), intended=N1),
    n320("N1-07", "postfix increment of screen in a hole", _probe_method('Debug.Log($"{screen++}");'), intended=N1),
    n320("N1-08", "null-coalescing assignment of manager in a hole", _probe_method('Debug.Log($"{(manager ??= null)}");'), intended=N1),
    n320("N1-09", "screen as the receiver of a ref-this extension inside a hole",
         (ROOT, N320_USING, N320_USING + '\n\ninternal static class ZScreenExt { public static int ZTouch(ref this '
          'ScenarioOneUiRoot.UiScreen s) { s = ScenarioOneUiRoot.UiScreen.Map; return 0; } }'),
         _in_show_objectives('Debug.Log($"{screen.ZTouch()}");'), intended="member receiver or passed whole as an argument inside an interpolation hole"),
    n320("N1-10", "screen passed as an in-argument inside a hole", _probe_method('Debug.Log($"{ZIn(in screen)}");'), intended=N1),
    n320("N1-11", "alignment after a tuple write of rackLayout in a hole",
         _in_show_objectives('Debug.Log($"{((rackLayout, workPlanWasOpen) = (null, false)),8}");'), intended=N1),
    n320("N1-12", "verbatim hole with an empty nested literal (both lexers agree), tuple write of help",
         _in_show_objectives('Debug.Log($@"{"" + ((help, workPlanWasOpen) = (null, false))}");'), intended=N1),
    n320("N1-13", "verbatim hole with an empty nested interpolated literal, tuple write of map",
         _in_show_objectives('Debug.Log($@"{$"" + ((map, workPlanWasOpen) = (null, false))}");'), intended=N1),
    n320("N1-14", "this-qualified write of workPlan in a hole at the end of Start", (ROOT, N320_START, N320_START
         + '\n        Debug.Log($"{(this.workPlan = null)}");'), intended=N1),
    n320("N1-15", "expression-bodied property whose hole writes help", (ROOT, N320_CLASS,
         'private string ZDiag => $"{((help, workPlanWasOpen) = (null, false))}";\n\n    ' + N320_CLASS), intended=N1),
    n320("N1-16", "local function whose hole writes review",
         _in_show_objectives('string ZL() => $"{(review = null)}"; Debug.Log(ZL());'), intended=N1),
    n320("N1-17", "comments inside a hole around a parenthesised write",
         _in_show_objectives('Debug.Log($"{/* c */ ((help) /* c */ = null)}");'), intended=N1),
    n320("N1-18", "this-qualified tuple write of screen in a hole after SetScreen",
         (ROOT, L1_SHOW_OBJ_FULL, L1_SHOW_OBJ_FULL + '\n        Debug.Log($"{((this.screen, workPlanWasOpen) = (UiScreen.Map, false))}");'),
         intended=N1),
    n320("N1-19", "ref argument write of workPlan in a hole (also seen by the write count)",
         _probe_method('Debug.Log($"{ZRef(ref workPlan, null)}");'), intended=N1),
    n320("N1-20", "two-level nested interpolation: the lexers disagree on its boundaries, so it fails closed before the hole rule",
         _in_show_objectives('Debug.Log($@"{$"" + $@"{"" + ((manager, workPlanWasOpen) = (null, false))}"}");'),
         intended="fail closed (N-4)"),
    n320("N1-21", "__makeref of help inside a hole (unsafe keyword; read for the hole rule)",
         _probe_method('Debug.Log($"{__reftype(__makeref(help))}");'), intended="undocumented reference keyword"),
]

n320_n2 = [
    n320("N2-01", "NeedsAnnualReview forced true; reviewed text kept in a nested interface",
         (ROOT, L1_NAR, 'public bool NeedsAnnualReview => true;\n    private interface IZDecoy { ScenarioOneManager manager { get; } '
          + L1_NAR + ' }'), intended=N2),
    n320("N2-02", "NeedsAnnualReview forced true; reviewed text kept in a class inside a namespace appended to the root file",
         (ROOT, L1_NAR, 'public bool NeedsAnnualReview => true;'),
         (ROOT, N320_USING, N320_USING + '\n\nnamespace ZDecoyNs { internal sealed class ZD { internal ScenarioOneManager manager; '
          + L1_NAR + ' } }'), intended=N2),
    n320("N2-03", "help becomes a null property; reviewed field kept in a nested record",
         (ROOT, L1_HELP_FIELD, 'private MenuHelpView help { get => null; set { } }\n    private sealed record ZDecoyRecord { private MenuHelpView help; }'),
         intended=N2),
    n320("N2-04", "#if-only braces make a nested decoy look like a class member",
         (ROOT, L1_HELP_FIELD, 'private MenuHelpView help { get => null; set { } }\n    private sealed class ZDecoyIf\n    {\n'
          '#if ZNEVER\n    }\n#endif\n    private MenuHelpView help;\n#if ZNEVER\n    {\n#endif\n    }'),
         intended="fail closed (N-2)"),
    n320("N2-05", "a static modifier in front of the reviewed help field",
         (ROOT, L1_HELP_FIELD, 'static ' + L1_HELP_FIELD), intended="attribute or modifier in front of it (N-2 owning class)"),
    n320("N2-06", "nested StandaloneSessionMenu stub makes the pinned guidance read IsOpen == true (suspended for good)",
         (ROOT, N320_CLASS, 'private sealed class StandaloneSessionMenu : MonoBehaviour { public static bool IsOpen => true; }\n\n    '
          + N320_CLASS),
         intended="type 'StandaloneSessionMenu' shadows a name the reviewed text reads"),
    n320("N2-07", "a Time member hides UnityEngine.Time for the pinned help-close frame test",
         (ROOT, N320_CLASS, 'private ZFrame Time => default;\n    private struct ZFrame { public int frameCount => -2; '
          'public float unscaledTime => 0f; }\n\n    ' + N320_CLASS), intended="member 'Time' of ScenarioOneUiRoot shadows"),
    n320("N2-08", "a using alias redirects Time for the whole root file (the pinned help-close frame test reads a stub)",
         (ROOT, N320_USING, N320_USING + '\nusing Time = ZTimeStub;\n\ninternal static class ZTimeStub { public static int frameCount => -2; '
          'public static float unscaledTime => 0f; }'), intended="using alias 'Time'"),
    n320("N2-09", "the root derives from a new base type whose nested Time hides UnityEngine.Time",
         (ROOT, 'public sealed class ScenarioOneUiRoot : MonoBehaviour', 'public sealed class ScenarioOneUiRoot : ZRootBase'),
         ("Assets/ForestPrototype/ZRootBase.cs", None, 'public abstract class ZRootBase : UnityEngine.MonoBehaviour\n{\n'
          '    protected static class Time { public static int frameCount => -2; public static float unscaledTime => 0f; }\n}\n'),
         intended="ScenarioOneUiRoot header"),
    n320("N2-12", "TeachingRacks forced true; reviewed text kept in a nested class of another view partial",
         (VIEW, L1_TEACHING, 'public bool TeachingRacks => true;'),
         (L1_SEQ, L1_SEQ_MEMBER, L1_SEQ_MEMBER + '\n    private sealed class ZDecoyTeach { public int CurrentTopicIndex => 0; ' + L1_TEACHING + ' }'),
         intended="RR-3 routing input 'TeachingRacks'"),
    n320("N2-13", "screen initialiser kept only as a comment and inside a #region line",
         (ROOT, 'private UiScreen screen = UiScreen.None;',
          '// private UiScreen screen = UiScreen.None;\n    #region private UiScreen screen = UiScreen.None;\n'
          '    private UiScreen screen { get => UiScreen.Map; set { } }\n    #endregion'), intended="(RR-2 related state)"),
    n320("N2-14", "manager field kept only in a nested struct; real member is a null property",
         (ROOT, 'private ScenarioOneManager manager;', 'private ScenarioOneManager manager { get => null; set { } }\n'
          '    private struct ZDecoyStruct { private ScenarioOneManager manager; }'), intended=N2),
]

n320_benign = [
    n320("NB320-01", "root: an unrelated nested class", (ROOT, N320_CLASS, 'private sealed class ZCache { public int Count; }\n\n    ' + N320_CLASS)),
    n320("NB320-02", "root: an unrelated top-level class in the same file",
         (ROOT, N320_USING, N320_USING + '\n\ninternal static class ZRootDiagnostics { internal static string Describe(int n) => $"n={n}"; }')),
    n320("NB320-03", "view: an unrelated nested struct", (VIEW, L1_TEACHING, L1_TEACHING + '\n    private struct ZPair { public int A, B; }')),
    n320("NB320-04", "view partial: a nested enum and a balanced #if block with a method",
         (L1_SEQ, L1_SEQ_MEMBER, L1_SEQ_MEMBER + '\n    private enum ZStage { One, Two }\n#if UNITY_EDITOR\n'
          '    private int ZEditorOnly() { if (teachingWorldVersion > 0) { return 1; } return 0; }\n#endif')),
    n320("NB320-05", "root: comments quoting reviewed declarations",
         (ROOT, N320_CLASS, '// private MenuHelpView help;\n    /* public bool NeedsAnnualReview => true; */\n    ' + N320_CLASS)),
    n320("NB320-06", "root: reads of related state in interpolation holes with format, alignment, this and ?.",
         _in_show_objectives('Debug.Log($"cash={manager.CashCents:N0} help={(help != null ? help.IsOpen : false),5} '
                             'map={map.RackMode} {this.review} {rackLayout?.Harvested}");')),
    n320("NB320-07", "root: @$ verbatim interpolation reading workPlan and screen across a line",
         _in_show_objectives('Debug.Log(@$"plan={workPlan}\n{screen}");')),
    n320("NB320-08", "root: lambda returning an interpolation that reads manager",
         _in_show_objectives('System.Func<string> zf = () => $"{manager.CashCents}"; Debug.Log(zf());')),
    n320("NB320-09", "root: tuple of state read in a hole and state deconstructed into locals",
         _in_show_objectives('object za = null, zb = null; Debug.Log($"{(help, map)} {((za, zb) = (help, map))}");')),
    n320("NB320-10", "root: verbatim hole with an empty nested literal reading manager",
         _in_show_objectives('Debug.Log($@"{"" + manager.CashCents}");')),
    n320("NB320-11", "root: an interpolation read at the end of Start", (ROOT, N320_START, N320_START
         + '\n        Debug.Log($"views ready: {help != null && map != null}");')),
    n320("NB320-12", "view: an unrelated top-level helper class in the view file",
         (VIEW, 'using UnityEngine.UIElements;\n', 'using UnityEngine.UIElements;\n\ninternal static class ZViewHelpers\n{\n'
          '    internal static int Twice(int x) => 2 * x;\n}\n')),
    n320("NB320-13", "root: nested class with a field named like a member-access name of the reviewed text",
         (ROOT, N320_CLASS, 'private sealed class ZRow { public string text; public int Count; }\n\n    ' + N320_CLASS)),
]

n320_over = [
    n320("O-10", "an attribute in front of a reviewed field declaration",
         (ROOT, L1_HELP_FIELD, '[System.NonSerialized] ' + L1_HELP_FIELD), documented="rejected"),
    n320("O-11", "#if/#else branches that each open a method body (balanced overall)",
         (ROOT, N320_CLASS, '#if UNITY_EDITOR\n    private int ZE() {\n#else\n    private int ZE() {\n#endif\n'
          '        return 0;\n    }\n\n    ' + N320_CLASS), documented="rejected"),
    n320("O-12", "a harmless nested type named like a local of the reviewed text",
         (ROOT, N320_CLASS, 'private sealed class reason { }\n\n    ' + N320_CLASS), documented="rejected"),
    n320("O-13", "a related-state name after a dot inside a hole (nameof of the field)",
         _in_show_objectives('Debug.Log($"{nameof(ScenarioOneUiRoot.map)}");'), documented="rejected"),
    n320("O-14", "screen as a member receiver inside a hole (ToString)", _in_show_objectives('Debug.Log($"{screen.ToString()}");'),
         documented="rejected"),
    n320("O-15", "`::` in a root interpolation hole (not valid C#: the compiler also starts the format clause at the first"
         " colon; kept to show the shape fails closed)", _in_show_objectives('Debug.Log($"{global::System.Math.PI}");'),
         documented="rejected"),
    n320("O-16", "a new view partial adds an (empty) base type, which could hide names the reviewed view text reads",
         (UI + "LearningObjectivesView.Base.cs", None, 'public sealed partial class LearningObjectivesView : ZViewBase\n{\n}\n'),
         ("Assets/ForestPrototype/ZViewBase.cs", None, 'public abstract class ZViewBase\n{\n}\n'), documented="rejected"),
]

n320_limits = [
    n320("L-09", "an unpinned root field the pinned routing reads becomes a null property (ecology)",
         (ROOT, 'private ForestEcologyController ecology;', 'private ForestEcologyController ecology { get => null; set { } }'),
         documented="accepted"),
    n320("L-10", "an unpinned root member the pinned routing reads changes (RackApprovalProblem always empty)",
         (ROOT, 'public string RackApprovalProblem => learning.RackMarkingProblem();', 'public string RackApprovalProblem => "";'),
         documented="accepted"),
]


def _case_outcome(base, case):
    """None when accepted, else the rejection reason (only is_companion_source files reach the scanner).
    An edit with old text None creates a file; in the reviewer's rounds it may also replace one whole."""
    files = dict(base)
    for path, old, new in case["edits"]:
        if old is None:
            if case["group"] == "#320" and (path in files or (REPO / path).exists()):
                raise RuntimeError(f"{case['id']}: {path} already exists")
            files[path] = new
        else:
            if path not in files:
                files[path] = (REPO / path).read_text(encoding="utf-8")
            files[path] = replace_one(files[path], old, new, case["id"])
    if case["transform"] == "crlf":
        files = {path: text.replace("\n", "\r\n") for path, text in files.items()}
    others = {path: text for path, text in files.items() if path not in (VIEW, ROOT) and is_companion_source(text)}
    try:
        verify_objective_cta(files[VIEW], files[ROOT], others)
    except ObjectiveCtaSourceError as exc:
        return str(exc)
    return None


def run_320(view, root, companions):
    """#320 groups. Returns counts that run() folds into its totals."""
    base = dict(companions)
    base.update({VIEW: view, ROOT: root})
    counts = dict(total=0, rejected=0, wrong_cause=0, benign=0, benign_failed=0, probe_failed=0)
    reviewer = r314_rounds + _r314_round7()
    groups = [(f"#314 reviewer {name}", [c for c in reviewer if c["group"] == name and c["expect"] == "rej"])
              for name in dict.fromkeys(c["group"] for c in reviewer)]
    groups += [("#320 N-1 author", n320_n1), ("#320 N-2 author", n320_n2)]
    for group, cases in groups:
        group_rejected = 0
        for case in cases:
            reason = _case_outcome(base, case)
            if reason is None:
                print(f"SOURCE-STATIC FAIL mutation-{case['id']}: FALSE GREEN")
            elif reason.startswith(CRASH) or (case["intended"] and case["intended"] not in reason):
                counts["wrong_cause"] += 1
                print(f"SOURCE-STATIC FAIL mutation-{case['id']}: rejected for an unexpected cause ({reason}); "
                      f"expected {case['intended'] or 'no scanner crash'!r}")
            else:
                group_rejected += 1
                print(f"SOURCE-STATIC PASS mutation-{case['id']}: rejected ({reason})")
        print(f"SOURCE-STATIC CTA MUTATION GROUP {group}: rejected={group_rejected}/{len(cases)}")
        counts["total"] += len(cases)
        counts["rejected"] += group_rejected
    for case in [c for c in reviewer if c["expect"] == "ben"] + n320_benign:
        reason = _case_outcome(base, case)
        counts["benign"] += 1
        if reason is None:
            print(f"SOURCE-STATIC PASS benign-{case['id']}: accepted")
        else:
            counts["benign_failed"] += 1
            print(f"SOURCE-STATIC FAIL benign-{case['id']}: WRONGLY REJECTED ({reason})")
    print(f"SOURCE-STATIC CTA #320 BENIGN CONTROLS accepted={counts['benign'] - counts['benign_failed']}/{counts['benign']}")
    probes = [c for c in reviewer if c["expect"] in ("lim", "unk", "over")] + n320_over + n320_limits
    for case in probes:
        reason = _case_outcome(base, case)
        observed = "accepted" if reason is None else "rejected"
        kind = {"lim": "accepted-limit", "unk": "exploratory", "over": "over-rejection"}.get(case.get("expect"), "")
        kind = kind or ("over-rejection" if case["id"].startswith("O-") else "accepted-limit")
        if observed == case["documented"]:
            print(f"SOURCE-STATIC INFO {kind}-{case['id']}: {observed} as documented ({case['why']})"
                  + (f" [{reason}]" if reason else ""))
        else:
            counts["probe_failed"] += 1
            print(f"SOURCE-STATIC FAIL {kind}-{case['id']}: now {observed}, documented {case['documented']}; update the "
                  f"README known limits ({reason or 'accepted'})")
    print(f"SOURCE-STATIC CTA #320 PROBES {len(probes)} changed={counts['probe_failed']} (reported, not counted as mutations)")
    return counts


def rebinding_cases(kit_source):
    """#325: verbatim #321 S05/S06/S07 source edits, then bounded nearby controls.

    Original: CTA-309-TwoGuardReview-20261107/new_variants.py and new-mutants/.
    The proof driver compares every resulting changed file byte-for-byte with that read-only evidence.
    These tests assert the name-binding cause, so an inventory failure cannot count as a repair.
    """
    ui = "Assets/ForestPrototype/UI/"
    time_stub = '\ninternal static class Time { public static int frameCount => -2; public static float unscaledTime => 0f; public static float deltaTime => 0f; public static float unscaledDeltaTime => 0f; public static float time => 0f; public static float realtimeSinceStartup => 0f; public static double realtimeSinceStartupAsDouble => 0d; public static float fixedDeltaTime => 0f; public static float timeScale { get; set; } }\n'
    base = 'public abstract class ZHiddenBase : UnityEngine.MonoBehaviour { protected static class Time { public static int frameCount => -2; public static float unscaledTime => 0f; } }'
    alias = (ROOT, 'using UnityEngine.InputSystem;', 'using UnityEngine.InputSystem;\nusing MonoBehaviour = ZHiddenBase;')
    f1 = "using alias 'MonoBehaviour' shadows a name the reviewed text reads (N-2)"
    f2 = "collected companion type 'Time' redefines a reviewed root identifier"

    def case(cid, cause, *edits):
        return dict(id=cid, group="#325", edits=list(edits), transform=None, intended=cause)

    repaired = [
        case('R321-S05', f2, (UIKIT, kit_source, kit_source + time_stub)),
        case('R321-S06', f1, alias, (ui + 'ZHiddenBase.cs', None,
             '// ScenarioOneUiRoot: all files in this test are included companions.\n' + base)),
        case('R321-S07', f1, alias, (UIKIT, kit_source, kit_source + '\n' + base + '\n')),
    ]
    nearby = [
        case('N325-F1-alternate-target', f1,
             (ROOT, alias[1], alias[2].replace('ZHiddenBase', 'ZAlternateBase')),
             (UIKIT, kit_source, kit_source + '\n' + base.replace('ZHiddenBase', 'ZAlternateBase') + '\n')),
        case('N325-F1-escaped-alias-global-target', f1,
             (ROOT, alias[1], alias[1] + '\nusing @MonoBehaviour = global::ZHiddenBase;'),
             (UIKIT, kit_source, kit_source + '\n' + base + '\n')),
        case('N325-F2-escaped-Time', f2, (UIKIT, kit_source, kit_source + time_stub.replace('class Time', 'class @Time'))),
        case('N325-F2-hinted-companion', f2,
             (ui + 'ZClock.cs', None, '// ScenarioOneUiRoot\n' + time_stub)),
        case('N325-F2-global-base', "collected companion type 'MonoBehaviour' redefines a reviewed root identifier",
             (UIKIT, kit_source, kit_source + '\npublic abstract class MonoBehaviour : UnityEngine.MonoBehaviour {'
              + time_stub.replace('internal static class Time', 'protected static class Time') + '}\n')),
    ]
    benign = [
        case('B325-root-unrelated-alias', None,
             (ROOT, alias[1], alias[1] + '\nusing ZClockAlias = UnityEngine.Time;')),
        case('B325-kit-unrelated-class', None,
             (UIKIT, kit_source, kit_source + '\ninternal class ZUiNotes { public int Value; }\n')),
        case('B325-kit-file-local-alias', None,
             (UIKIT, 'using System;', 'using System;\nusing Time = System.Int32;')),
        case('B325-hinted-unrelated-base', None,
             (ui + 'ZHiddenBase.cs', None, '// ScenarioOneUiRoot\npublic abstract class ZHiddenBase : UnityEngine.MonoBehaviour { protected static class ZClock { public static int frameCount => -2; } }')),
        case('B325-root-unrelated-class-and-alias', None,
             (ROOT, alias[1], alias[1] + '\nusing ZCounter = System.Int32;'),
             (ROOT, 'public sealed class ScenarioOneUiRoot', 'internal sealed class ZCounterNotes {}\npublic sealed class ScenarioOneUiRoot')),
    ]
    return repaired, nearby, benign


def run_rebinding(view, root, companions):
    base = {VIEW: view, ROOT: root, **companions}
    repaired, nearby, benign = rebinding_cases(companions[UIKIT])
    failures = 0
    for label, cases in (("repaired", repaired), ("nearby", nearby), ("benign", benign)):
        passed = 0
        for case in cases:
            reason = _case_outcome(base, case)
            good = reason is None if label == "benign" else reason is not None and case["intended"] in reason
            if good:
                passed += 1
            else:
                failures += 1
            print(f"SOURCE-STATIC {'PASS' if good else 'FAIL'} #325-{case['id']}: "
                  f"{reason or 'accepted'}; expected={case['intended'] or 'accepted'}")
        print(f"SOURCE-STATIC CTA #325 {label} passed={passed}/{len(cases)}")
    return failures


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
    loop1 = run_loop1(live[V], live[R], companions)
    total += loop1["total"]
    rejected += loop1["rejected"]
    wrong_cause += loop1["wrong_cause"]
    benign_failed += loop1["benign_failed"]
    two_guard = run_320(live[V], live[R], companions)
    total += two_guard["total"]
    rejected += two_guard["rejected"]
    wrong_cause += two_guard["wrong_cause"]
    benign_failed += two_guard["benign_failed"]
    rebinding_failed = run_rebinding(live[V], live[R], companions)

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
    return 1 if (false_greens or wrong_cause or benign_failed or historic_failed or loop1["probe_failed"]
                 or two_guard["probe_failed"] or rebinding_failed) else 0


if __name__ == "__main__":
    raise SystemExit(run())
