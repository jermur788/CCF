#!/usr/bin/env python3
"""Bounded source-structure guard for the Objective 1 map CTA.

This is intentionally not a Unity/UI behaviour test and not a C# parser. It checks
the real constructor and guidance methods with lexical rules that fail closed:
an ambiguous or unsupported source shape raises ObjectiveCtaSourceError rather
than being accepted. Comments and literal text are never treated as code; code in
an interpolation hole is. The #309 section at the end adds class-wide guards
(precise lexer, pinned reviewed methods, header grammar, occurrence inventories);
README.md lists exactly what they cover and what they do not.

Copy this file, check_baseline.py and test_objective_cta_source_check.py
together; check_baseline.py imports this module.
"""
import bisect
import re
import unicodedata
from collections import namedtuple
from pathlib import Path

VIEW = "Assets/ForestPrototype/UI/LearningObjectivesView.cs"
ROOT = "Assets/ForestPrototype/UI/ScenarioOneUiRoot.cs"
UIKIT = "Assets/ForestPrototype/UI/UiKit.cs"

CTA_NAME = "objective-primary-cta"
PRIMARY_CLASS = "btn-primary"
# Pinned shape of RefreshNextActionGuidance. Any added `target = ...` statement
# changes this count and must be reviewed deliberately (R11-style retargeting).
EXPECTED_REFRESH_TARGET_ASSIGNMENTS = 14
EXPECTED_REFRESH_RETURNS = 3
EXPECTED_APPLY_RETURNS = 2
EXPECTED_VISIBLE_RETURNS = 3

# C# comments and literals; preserve literal positions while matching braces.
_LEX = re.compile(
    r'//[^\n]*|/\*[\s\S]*?\*/|@"(?:[^"]|"")*"|"(?:[^"\\]|\\.)*"|'
    r"'(?:[^'\\]|\\.)*'"
)
_SOLE_LITERAL = re.compile(r'\s*(@"(?:[^"]|"")*"|"(?:[^"\\]|\\.)*")\s*\Z')
# A leading BOM may precede a directive on the first line (#314 S-2: the precise lexer accepts it too).
_COND = re.compile(r'^\ufeff?[ \t]*#[ \t]*(if|elif|else|endif|define|undef)\b', re.M)
_CTOR_SIGNATURE = r"\bpublic\s+LearningObjectivesView\s*\(\s*ScenarioOneUiRoot\s+ui\s*\)"


class ObjectiveCtaSourceError(ValueError):
    """A structural invariant cannot be demonstrated from the given source.

    `rows` carries (path, line, text) evidence for the failing area when it can
    be found; `predicate` names the unmet rule. Neither changes str(error).
    """

    def __init__(self, message, rows=None, predicate=None):
        super().__init__(message)
        self.rows = list(rows or [])
        self.predicate = predicate or message


# ---------------------------------------------------------------- lexical helpers

def _blank(value):
    return "".join("\n" if char == "\n" else " " for char in value)


def _code(source, mask_literals=False):
    def strip(match):
        token = match.group()
        return _blank(token) if mask_literals or token.startswith(("//", "/*")) else token
    return _LEX.sub(strip, source)


def _decode(token):
    """Value of an ordinary or verbatim C# string token (not interpolated)."""
    if token.startswith('@"'):
        return token[2:-1].replace('""', '"')
    simple = {"n": "\n", "r": "\r", "t": "\t", "0": "\0", "a": "\a", "b": "\b",
              "f": "\f", "v": "\v", "\\": "\\", '"': '"', "'": "'"}

    def escape(match):
        text = match.group(1)
        if text[0] in "uUx" and len(text) > 1:
            return chr(int(text[1:], 16))
        return simple.get(text, text)
    return re.sub(r'\\(u[0-9A-Fa-f]{4}|U[0-9A-Fa-f]{8}|x[0-9A-Fa-f]{1,4}|.)', escape, token[1:-1])


def _literals(code):
    """(start, end, decoded value, interpolated) for each string token in comment-free code."""
    found = []
    for match in _LEX.finditer(code):
        token = match.group()
        if token[0] in '"@':
            found.append((match.start(), match.end(), _decode(token),
                          match.start() > 0 and code[match.start() - 1] == "$"))
    return found


def _sole_literal(text):
    """Decoded value when `text` is exactly one non-interpolated string token, else None."""
    match = _SOLE_LITERAL.match(text)
    return _decode(match.group(1)) if match else None


def _matches(source, regex, flags=re.S):
    """Regex matches that begin in code, never inside a comment, string or char literal (R09)."""
    spans = [(m.start(), m.end()) for m in _LEX.finditer(source)]
    return [m for m in re.finditer(regex, source, flags)
            if not any(a <= m.start() < b for a, b in spans)]


def _assignments(masked, name, qualified=False):
    """Count writes to a simple local/field: `x = `, `x += `, `x++`, `out x`, `ref x`.
    With qualified=True a qualified write such as `this.x = ` is counted too (#309 RR-2)."""
    ident = re.escape(name)
    write = re.compile((r'(?<!\w)' if qualified else r'(?<![.\w])') + ident
                       + r'\s*(?:(?:[-+*/%|&^]|<<|>>|\?\?)?=(?![=>])|\+\+|--)')
    byref = re.compile(r'\b(?:out|ref)\s+(?:[A-Za-z_][\w<>\[\]]*\s+)?' + ident + r'\b')
    return len(write.findall(masked)) + len(byref.findall(masked))


def _find_conditional(masked, start, end, label):
    depth = 0
    for match in _COND.finditer(masked):
        if match.start() < start:
            depth += match.group(1) == "if"
            depth -= match.group(1) == "endif"
        elif match.start() <= end:
            raise ObjectiveCtaSourceError(
                f"{label}: conditional compilation (#{match.group(1)}) in a checked method or declaration is unsupported")
    if depth > 0:
        raise ObjectiveCtaSourceError(f"{label}: declared inside conditional compilation (#if)")


def _method_span(source, signature):
    masked = _code(source, mask_literals=True)
    found = list(re.finditer(signature, masked))
    if len(found) != 1:
        raise ObjectiveCtaSourceError(
            f"expected exactly one method matching {signature!r}; found {len(found)}",
            predicate=f"exactly one method {signature}")
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
                _find_conditional(masked, found[0].start(), at, signature)
                return opening + 1, at
    raise ObjectiveCtaSourceError("unbalanced braces in method")


def _method(source, signature):
    start, end = _method_span(source, signature)
    return _code(source[start:end])


def _one(source, regex, label):
    matches = _matches(source, regex)
    if len(matches) != 1:
        raise ObjectiveCtaSourceError(
            f"{label}: expected one structural match, found {len(matches)}", predicate=f"{label}: {regex}")
    return matches[0]


def _ordered(source, patterns, label):
    previous = -1
    for name, pattern in patterns:
        found = _one(source, pattern, f"{label}/{name}")
        if found.start() <= previous:
            raise ObjectiveCtaSourceError(f"{label}/{name}: invalid control-flow order")
        previous = found.start()


def _count(masked, regex):
    return len(re.findall(regex, masked))


def _args(masked, open_idx):
    """Top-level argument spans of the call whose '(' is at open_idx, and the ')' index."""
    depth, start, spans = 0, open_idx + 1, []
    for at in range(open_idx, len(masked)):
        char = masked[at]
        if char in "([{":
            depth += 1
        elif char in ")]}":
            depth -= 1
            if depth == 0:
                spans.append((start, at))
                return spans, at
        elif char == "," and depth == 1:
            spans.append((start, at))
            start = at + 1
    raise ObjectiveCtaSourceError("unbalanced call arguments in the Objectives constructor")


# ------------------------------------------------------- F1: header primary sources

def _allowlist_values(root_code):
    match = _one(root_code, r'\bGuidanceButtonNames\s*=\s*\{(.*?)\}\s*;', "GuidanceButtonNames allowlist")
    _find_conditional(_code(root_code, mask_literals=True), match.start(), match.end(), "GuidanceButtonNames")
    body = match.group(1)
    literals = _literals(body)
    leftover = _code(body, mask_literals=True)
    if any(interpolated for *_, interpolated in literals) or re.search(r"[^\s,]", leftover):
        raise ObjectiveCtaSourceError("GuidanceButtonNames must contain only plain string literals")
    values = [value for _, _, value, _ in literals]
    if len(values) != len(set(values)):
        raise ObjectiveCtaSourceError("GuidanceButtonNames contains a duplicate name")
    if values.count(CTA_NAME) != 1:
        raise ObjectiveCtaSourceError("map CTA missing or duplicated in scoped allowlist")
    # Qualified references (e.g. ScenarioOneUiRoot.GuidanceButtonNames[2] = ...) count too (#309 RR-1).
    uses = _count(_code(root_code, mask_literals=True), r'(?<!\w)GuidanceButtonNames\b')
    if uses != 2:
        raise ObjectiveCtaSourceError(
            f"GuidanceButtonNames must be used only by its declaration and the demotion loop; found {uses} references")
    return set(values)


def _audit_map_identity(ctor_masked, view_code, names, map_var):
    """The map CTA has exactly one literal name, in any spelling, and no other uses (F2/F3)."""
    map_names = [value for receiver, value in names if receiver == map_var]
    if map_names != [CTA_NAME]:
        raise ObjectiveCtaSourceError(
            "Objective CTA name must be unique and bound to the ShowMap button; "
            f"found map names {map_names}")
    if sum(1 for _, _, value, _ in _literals(view_code) if value == CTA_NAME) != 1:
        raise ObjectiveCtaSourceError("Objective CTA literal must appear exactly once in the view (any spelling)")
    if _count(ctor_masked, r'(?<![.\w])' + re.escape(map_var) + r'\b') != 3:
        raise ObjectiveCtaSourceError(
            "map CTA variable may only be declared, named and added to the header row; other uses are unsupported")
    if _assignments(ctor_masked, map_var) != 1:
        raise ObjectiveCtaSourceError("map CTA variable is reassigned")


def _audit_header(ctor, allowed, map_var):
    """Enumerate every static source of btn-primary in the Objectives constructor and
    require each to be a uniquely named button inside the scoped demotion set (F1).
    `map_var` may be None when the map button cannot be found; the primary audit still runs."""
    masked = _code(ctor, mask_literals=True)
    literals = _literals(ctor)
    sources, claimed = [], []

    for call in re.finditer(r'(?<![.\w])UiKit\s*\.\s*Button\s*\(', masked):
        spans, close = _args(masked, call.end() - 1)
        if len(spans) < 2:
            raise ObjectiveCtaSourceError("UiKit.Button call in the header has fewer than two arguments")
        primary = False
        for index, (a, b) in enumerate(spans):
            if index < 2:
                continue
            if re.match(r'\s*\w+\s*:(?!:)', masked[a:b]):
                raise ObjectiveCtaSourceError("named UiKit.Button argument in the header is an unsupported shape")
            if index == 2 and masked[a:b].strip() in ("true", "false"):
                continue
            value = _sole_literal(ctor[a:b])
            if value is None:
                raise ObjectiveCtaSourceError(
                    "UiKit.Button class argument in the header is not a plain string literal (ambiguous)")
            if PRIMARY_CLASS in value.split():
                primary = True
                claimed.append((a, b))
        if primary:
            declared = re.search(r'\b(?:Button|var)\s+([A-Za-z_]\w*)\s*=\s*$', masked[:call.start()])
            if not declared or not re.match(r'\s*;', masked[close + 1:]):
                raise ObjectiveCtaSourceError(
                    "static btn-primary header button is not assigned to a named local (unnamed primary)")
            sources.append((declared.group(1), "UiKit.Button"))

    for call in re.finditer(r'\.\s*(AddToClassList|EnableInClassList|ToggleInClassList)\s*\(', masked):
        spans, _ = _args(masked, call.end() - 1)
        a, b = spans[0]
        value = _sole_literal(ctor[a:b])
        if value is None:
            raise ObjectiveCtaSourceError(
                f"{call.group(1)} in the header uses a non-literal class name (ambiguous)")
        if PRIMARY_CLASS in value.split():
            receiver = re.search(r'(?<![\w.)\]])([A-Za-z_]\w*)\s*$', masked[:call.start()])
            if not receiver:
                raise ObjectiveCtaSourceError(
                    f"{call.group(1)}(btn-primary) on a non-simple receiver in the header (unnamed primary)")
            claimed.append((a, b))
            sources.append((receiver.group(1), call.group(1)))

    for start, _, value, _ in literals:
        in_claimed = any(a <= start < b for a, b in claimed)
        if (PRIMARY_CLASS in value or value.strip() in ("btn-", "-primary", "primary")) and not in_claimed:
            raise ObjectiveCtaSourceError(
                f"btn-primary literal {value!r} in the header is not in a supported UiKit.Button/AddToClassList shape")

    names = []  # (receiver variable or None, value)
    for match in re.finditer(r'(?<![\w])name\s*=(?![=>])', masked):
        rhs = re.match(r'\s*(@"(?:[^"]|"")*"|"(?:[^"\\]|\\.)*")\s*(?=[;,}])', ctor[match.end():])
        if not rhs:
            raise ObjectiveCtaSourceError("an element name in the Objectives constructor is not a single plain literal")
        before = masked[:match.start()].rstrip()
        receiver = None
        if before.endswith("."):
            core = before[:-1].rstrip()
            found = re.search(r'(?<![\w.)\]])([A-Za-z_]\w*)$', core)
            receiver = found.group(1) if found else None
        names.append((receiver, _decode(rhs.group(1))))
    values = [value for _, value in names]
    if len(values) != len(set(values)):
        raise ObjectiveCtaSourceError("duplicate element name in the Objectives constructor")

    primaries = []
    for variable, how in sorted(set(sources)):
        if map_var is not None and variable == map_var:
            raise ObjectiveCtaSourceError(f"map CTA must not be unconditionally primary ({how})")
        if _count(masked, r'\b(?:Button|var)\s+' + re.escape(variable) + r'\s*=') != 1 \
                or _assignments(masked, variable) != 1:
            raise ObjectiveCtaSourceError(
                f"static primary '{variable}' must be declared once and never reassigned in the header")
        own = [value for receiver, value in names if receiver == variable]
        if len(own) != 1:
            raise ObjectiveCtaSourceError(
                f"static primary '{variable}' must have exactly one literal name; found {len(own)}")
        # allowed is None when the allowlist itself is invalid; that specific error is raised by the
        # caller instead of being masked by a membership failure (#309 RR-5).
        if own[0] == CTA_NAME or (allowed is not None and own[0] not in allowed):
            raise ObjectiveCtaSourceError(
                f"static primary button '{own[0]}' is not in the GuidanceButtonNames scoped demotion set")
        primaries.append(own[0])
    return primaries, names, masked


# ------------------------------------------------------------------- diagnostics

def _diagnostic_rows(view_source, root_source):
    """Best-effort evidence for a failed check; never raises."""
    rows = []
    try:
        start, end = _method_span(view_source, _CTOR_SIGNATURE)
        first = view_source.count("\n", 0, start) + 1
        last = view_source.count("\n", 0, end) + 1
        lines, code_lines = view_source.splitlines(), _code(view_source).splitlines()
        for number in range(first, last + 1):
            if re.search(r'UiKit\s*\.\s*Button|btn-primary|\bname\s*=|ClassList', code_lines[number - 1]):
                rows.append((VIEW, number, lines[number - 1].strip()))
    except Exception:  # noqa: BLE001 - diagnostics must not mask the real failure
        pass
    try:
        lines, code_lines = root_source.splitlines(), _code(root_source).splitlines()
        for number, text in enumerate(code_lines, 1):
            if "GuidanceButtonNames" in text:
                rows.append((ROOT, number, lines[number - 1].strip()))
    except Exception:  # noqa: BLE001
        pass
    return rows


def _line_of(source, regex):
    match = next(iter(_matches(_code(source), regex)), None)
    return source.count("\n", 0, match.start()) + 1 if match else 0


# ----------------------------------------------------------------------- verifier

def verify_objective_cta(view_source, root_source, companions=None):
    """Raise ObjectiveCtaSourceError on a missing/ambiguous CTA, otherwise return evidence rows.

    Accept source *text* so mutation tests can operate solely on temporary
    in-memory copies. No production files are modified by this function.
    `companions` maps repository paths to the text of the other files that may
    declare LearningObjectivesView partials, ScenarioOneUiRoot or UiKit (see
    collect_companions). Without it the class-wide guards cannot run, so the
    source is rejected rather than passed (#309 N-3).
    """
    try:
        return _verify(view_source, root_source, companions)
    except ObjectiveCtaSourceError as error:
        if not error.rows:
            error.rows = _diagnostic_rows(view_source, root_source)
        raise
    except Exception as error:  # noqa: BLE001 - an unexpected scanner failure is a FAIL, not a crash or a pass
        raise ObjectiveCtaSourceError(
            f"scanner could not analyse the source ({type(error).__name__}: {error})",
            rows=_diagnostic_rows(view_source, root_source)) from error


def _verify(view_source, root_source, companions=None):
    # N-4 first: every check below relies on the lexer telling code from comments and literals.
    view_spans = _lexically_sound(view_source, VIEW)
    root_spans = _lexically_sound(root_source, ROOT)
    view_code = _code(view_source)
    view = _method(view_source, _CTOR_SIGNATURE)
    root = _code(root_source)
    try:
        allowed, allowlist_error = _allowlist_values(root), None
    except ObjectiveCtaSourceError as error:
        allowed, allowlist_error = None, error
    # Enumerate static primaries first so a competing/unnamed primary is reported as such,
    # even for historical sources that have no map button or allowlist at all.
    tolerant = re.search(r'\bButton\s+([A-Za-z_]\w*)\s*=\s*UiKit\.Button\s*\(\s*"Stand map \[M\]"',
                         _code(view, mask_literals=False))
    static_primaries, names, ctor_masked = _audit_header(view, allowed, tolerant.group(1) if tolerant else None)
    if allowlist_error:
        raise allowlist_error

    constructor = _one(
        view,
        r'\bButton\s+([A-Za-z_]\w*)\s*=\s*UiKit\.Button\s*\(\s*'
        r'"Stand map \[M\]"\s*,\s*\(\s*\)\s*=>\s*ui\.ShowMap\s*\(\s*\)\s*\)\s*;',
        "named map button with exact caption and ShowMap click")
    map_var = constructor.group(1)
    _ordered(view, [
        ("root modal", r'\bRoot\.Add\s*\(\s*modal\s*\)\s*;'),
        ("header", r'\bVisualElement\s+header\s*=\s*UiKit\.Box\s*\(\s*"modal-header"\s*\)\s*;'),
        ("button row", r'\bVisualElement\s+buttons\s*=\s*UiKit\.Row\s*\(\s*header\s*\)\s*;'),
        ("map click", r'\bButton\s+' + re.escape(map_var) + r'\s*=\s*UiKit\.Button'),
        ("map identity", r'\b' + re.escape(map_var) + r'\.name\s*=\s*"objective-primary-cta"\s*;'),
        ("row binding", r'\bbuttons\.Add\s*\(\s*' + re.escape(map_var) + r'\s*\)\s*;'),
        ("header attached", r'\bmodal\.Add\s*\(\s*header\s*\)\s*;'),
    ], "Objectives constructor")
    _audit_map_identity(ctor_masked, view_code, names, map_var)

    refresh = _method(root_source, r'\bpublic\s+void\s+RefreshNextActionGuidance\s*\(\s*\)')
    refresh_masked = _code(refresh, mask_literals=True)
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
    # Bounded override guards: no extra target writes, early exits or state resets.
    after_chain = _one(refresh, r'\belse\s+return\s*;', "end of target chain")
    if _count(refresh_masked[after_chain.end():], r'(?<![.\w])target\s*=(?![=>])'):
        raise ObjectiveCtaSourceError("target is reassigned after the routing chain")
    targets = _assignments(refresh_masked, "target")
    if targets != EXPECTED_REFRESH_TARGET_ASSIGNMENTS:
        raise ObjectiveCtaSourceError(
            f"RefreshNextActionGuidance writes target {targets} times; expected "
            f"{EXPECTED_REFRESH_TARGET_ASSIGNMENTS} (routing change needs checker review)")
    for button_id in re.findall(r'"((?:objective|map|workplan|annual)-[\w-]+)"', refresh):
        if button_id not in allowed:
            raise ObjectiveCtaSourceError(f"guidance target '{button_id}' is not in GuidanceButtonNames")
    if _count(refresh_masked, r'\breturn\b') != EXPECTED_REFRESH_RETURNS:
        raise ObjectiveCtaSourceError("RefreshNextActionGuidance has an unexpected early return")
    for name, expected in (("scope", 1), ("guidanceCandidate", 2), ("guidanceScope", 2), ("owned", 1)):
        if _assignments(refresh_masked, name) != expected:
            raise ObjectiveCtaSourceError(f"RefreshNextActionGuidance writes '{name}' an unexpected number of times")

    clear = _method(root_source, r'\bprivate\s+void\s+ClearNextAction\s*\(\s*\)')
    _ordered(clear, [
        ("only existing nextAction", r'\bif\s*\(\s*nextAction\s*!=\s*null\s*\)'),
        ("remove primary", r'\bnextAction\.RemoveFromClassList\s*\(\s*"btn-primary"\s*\)\s*;'),
        ("remove guide class", r'\bnextAction\.RemoveFromClassList\s*\(\s*"next-action"\s*\)\s*;'),
        ("clear reference", r'\bnextAction\s*=\s*null\s*;'),
    ], "ClearNextAction")
    clear_masked = _code(clear, mask_literals=True)
    if _count(clear_masked, r'\breturn\b'):
        raise ObjectiveCtaSourceError("ClearNextAction must not return early")
    if _count(clear_masked, r'(?<![=!<>])=(?![=>])') != 1 \
            or not set(re.findall(r'\.\s*(\w+)\s*\(', clear_masked)) <= {"RemoveFromClassList", "RemoveFromHierarchy"}:
        raise ObjectiveCtaSourceError("ClearNextAction may only drop the guide classes, badge and nextAction")
    # Guidance state is written only by the reviewed methods, never by an unrelated handler.
    root_masked = _code(root, mask_literals=True)
    for name, expected in (("guidanceCandidate", 2), ("guidanceScope", 2), ("nextAction", 2)):
        if _assignments(root_masked, name, qualified=True) != expected:
            raise ObjectiveCtaSourceError(
                f"'{name}' must be written only by RefreshNextActionGuidance/ApplyNextActionGuidance/ClearNextAction")

    apply = _method(root_source, r'\bprivate\s+void\s+ApplyNextActionGuidance\s*\(\s*\)')
    apply_masked = _code(apply, mask_literals=True)
    _ordered(apply, [
        ("preview/menus/help suspension",
         r'\bbool\s+suspended\s*=\s*manager\s*==\s*null\s*\|\|\s*manager\.ReferencePreviewActive'
         r'\s*\|\|\s*StandaloneSessionMenu\.IsOpen\s*\|\|\s*help\s*==\s*null\s*\|\|'
         r'\s*help\.IsOpen\s*\|\|\s*helpClosedFrame\s*==\s*Time\.frameCount\s*;'),
        ("candidate copy", r'\bButton\s+candidate\s*=\s*guidanceCandidate\s*;'),
        ("candidate scoped visibility",
         r'\bbool\s+eligible\s*=\s*!suspended\s*&&\s*candidate\s*!=\s*null\s*&&'
         r'\s*candidate\.enabledInHierarchy\s*&&\s*GuidanceVisible\s*\(\s*candidate\s*,\s*scope\s*\)\s*;'),
        ("unchanged candidate is kept", r'\bif\s*\(\s*nextAction\s*==\s*candidate\s*&&\s*eligible\s*\)\s*return\s*;'),
        ("clear stale candidate", r'\bClearNextAction\s*\(\s*\)\s*;'),
        ("no styling if ineligible", r'\bif\s*\(\s*!eligible\s*\)\s*return\s*;'),
        ("set current candidate", r'\bnextAction\s*=\s*candidate\s*;'),
        ("apply primary", r'\bcandidate\.AddToClassList\s*\(\s*"btn-primary"\s*\)\s*;'),
        ("apply next-action", r'\bcandidate\.AddToClassList\s*\(\s*"next-action"\s*\)\s*;'),
    ], "ApplyNextActionGuidance")
    if _count(apply_masked, r'\breturn\b') != EXPECTED_APPLY_RETURNS:
        raise ObjectiveCtaSourceError("ApplyNextActionGuidance has an unexpected early return")
    for name in ("suspended", "candidate", "scope", "eligible", "nextAction"):
        if _assignments(apply_masked, name) != 1:
            raise ObjectiveCtaSourceError(f"ApplyNextActionGuidance writes '{name}' more than once (override)")
    members = set(re.findall(r'(?<![.\w])candidate\s*\.\s*(\w+)', apply_masked))
    if not members <= {"enabledInHierarchy", "parent", "AddToClassList"}:
        raise ObjectiveCtaSourceError(
            "ApplyNextActionGuidance uses an unexpected candidate member: "
            + ", ".join(sorted(members - {"enabledInHierarchy", "parent", "AddToClassList"})))

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
    if _count(_code(visible, mask_literals=True), r'\breturn\b') != EXPECTED_VISIBLE_RETURNS:
        raise ObjectiveCtaSourceError("GuidanceVisible has an unexpected return")

    # The cached guidance must be re-applied after layout and honour the Help-close frame.
    late = _method(root_source, r'\bprivate\s+void\s+LateUpdate\s*\(\s*\)')
    late_call = _one(late, r'\bApplyNextActionGuidance\s*\(\s*\)\s*;', "LateUpdate applies cached guidance")
    if re.search(r'\b(?:return|if|else|for|foreach|while|switch|throw|goto|yield)\b|[{}]',
                 _code(late[:late_call.start()], mask_literals=True)):
        raise ObjectiveCtaSourceError("LateUpdate may only apply guidance unconditionally before any branch or return")
    close_help = _method(root_source, r'\bpublic\s+void\s+CloseHelp\s*\(\s*\)')
    _ordered(close_help, [
        ("help closes", r'\bhelp\.Close\s*\(\s*\)\s*;'),
        ("help-close frame recorded", r'\bhelpClosedFrame\s*=\s*Time\.frameCount\s*;'),
    ], "CloseHelp")
    between = close_help[_one(close_help, r'\bhelp\.Close\s*\(\s*\)\s*;', "CloseHelp").end():
                         _one(close_help, r'\bhelpClosedFrame\s*=\s*Time\.frameCount\s*;', "CloseHelp").start()]
    if re.search(r'\b(?:return|if|else|throw|goto)\b|[{}]', _code(between, mask_literals=True)):
        raise ObjectiveCtaSourceError("helpClosedFrame is set conditionally in CloseHelp")
    if _assignments(_code(root, mask_literals=True), "helpClosedFrame", qualified=True) != 2:
        raise ObjectiveCtaSourceError("helpClosedFrame must be written only by its initialiser and CloseHelp")

    # #309: class-wide RR-1/RR-2/RR-3 guards that the per-statement checks above cannot see.
    _class_wide_guards(view_source, root_source, view_spans, root_spans, companions)

    def evidence(source, regex):
        return _line_of(source, regex)

    primary_text = ", ".join(static_primaries) if static_primaries else "none"
    return [
        (VIEW, evidence(view_source, r'\bButton\s+' + re.escape(map_var) + r'\s*=\s*UiKit\.Button'),
         "Named Stand map [M] button -> ui.ShowMap(), added to Objectives header row; never statically primary"),
        (VIEW, evidence(view_source, r'\bUiKit\s*\.\s*Button\s*\([^;]*btn-primary'),
         f"Static btn-primary header sources: {primary_text}; each uniquely named and in the scoped demotion set"),
        (ROOT, evidence(root_source, r'\bif\s*\(\s*objectives\s*\)\s*target\s*=\s*"objective-primary-cta"'),
         "Conditional Rack V2 / unconfirmed Objective 1 target"),
        (ROOT, evidence(root_source, r'\bforeach\s*\(\s*string\s+name\s+in\s+GuidanceButtonNames'),
         "Scoped class demotion with original primary restoration"),
        (ROOT, evidence(root_source, r'\bbool\s+suspended\s*=\s*manager\s*==\s*null'),
         "Reference/menus/Help and help-close suspension"),
        (ROOT, evidence(root_source, r'\bhelpClosedFrame\s*=\s*Time\.frameCount'),
         "Help-close frame recorded; LateUpdate re-applies cached guidance"),
        (ROOT, evidence(root_source, r'\bbool\s+eligible\s*=\s*!suspended'),
         "Enabled, visible, descendant-of-scope eligibility"),
        (ROOT, evidence(root_source, r'\bcandidate\.AddToClassList\s*\(\s*"btn-primary"\s*\)'),
         "Dynamic primary and next-action class application"),
        (VIEW, evidence(view_source, r'\bUiKit\s*\.\s*Button\s*\(\s*"Back to forest'),
         "Exactly one static primary (Back to forest, bound caption/click/name) in a straight-line header; "
         "no other btn-primary source or class-list call in the view class or its partials"),
        (ROOT, evidence(root_source, r'\bpublic\s+void\s+RefreshNextActionGuidance\s*\(\s*\)'),
         "Pinned guidance/help/screen methods (comment- and whitespace-normalised) and guarded-state "
         "occurrence inventory; the UiKit members the constructor calls are pinned"),
    ]


# =====================================================================================
# #309 third pass: bounded class-wide guards (RR-1..RR-5, N-1..N-4).
#
# Still not a C# parser or compiler. Three bounded techniques are added, each failing closed:
#   * a precise lexer for comments, preprocessor lines and string/char literals, including
#     interpolation holes; a shape it cannot follow is rejected rather than guessed (N-4);
#   * comment- and whitespace-normalised token pins of the reviewed root and UiKit methods
#     (a deliberate edit to one of them needs a matching checker update);
#   * a straight-line statement grammar for the Objectives header plus occurrence inventories
#     of the locals, fields, literals and APIs that can reach the CTA or its guidance state.
# What remains outside these guards is listed in README.md ("Known limits").
# =====================================================================================

_COMPANION_HINT = re.compile(r'LearningObjectivesView|ScenarioOneUiRoot|UiKit|objective-primary-cta|'
                             r'objective-work-plan|objective-back-to-forest|\\[uUx]|\.\s*Learning\b')


def _without_format_characters(text):
    """The text as the C# compiler reads identifiers: Unicode format characters (category Cf)
    removed, so `Learning<U+200D>ObjectivesView` still names the class (#314 S-2)."""
    return re.sub(r'[^\x00-\x7f]', lambda m: "" if unicodedata.category(m.group()) == "Cf" else m.group(), text)


def is_companion_source(text):
    """True when a C# file could declare a LearningObjectivesView partial, ScenarioOneUiRoot or UiKit,
    name an Objectives header button or reach `.Learning`, including through a \\u or \\x escape or a
    format character inside a name."""
    return bool(_COMPANION_HINT.search(text) or _COMPANION_HINT.search(_without_format_characters(text)))


def collect_companions(repo_root):
    """Read-only: every C# file under Assets/ (other than VIEW and ROOT) for which
    is_companion_source holds. Pass the result to verify_objective_cta."""
    repo = Path(repo_root)
    found = {}
    for path in sorted((repo / "Assets").rglob("*.cs")):
        rel = path.relative_to(repo).as_posix()
        if rel not in (VIEW, ROOT):
            text = path.read_text(encoding="utf-8")
            if is_companion_source(text):
                found[rel] = text
    return found


# ------------------------------------------------------------- precise lexer (N-4)

_CHAR_LITERAL = re.compile(
    r"'(?:[^'\\\n]|\\(?:[\\'\"0abfnrtv]|u[0-9A-Fa-f]{4}|U[0-9A-Fa-f]{8}|x[0-9A-Fa-f]{1,4}))'")
_LEX_STOP = re.compile(r'[/"\'@$\\\n#{}()\[\]:]')
_STRING_START = re.compile(r'(\$@|@\$|\$|@)?"')


class _Unlexable(Exception):
    def __init__(self, at, reason):
        super().__init__(reason)
        self.at = at


def _row(source, path, at):
    line = source.count("\n", 0, at) + 1
    lines = source.split("\n")  # not splitlines(): it also splits at characters C# does not treat as line ends
    return line, [(path, line, lines[line - 1].strip() if line <= len(lines) else "")]


def _fail(source, path, at, message):
    line, rows = _row(source, path, at)
    raise ObjectiveCtaSourceError(f"{path}:{line}: {message}", rows=rows, predicate=message)


def _scan_code(src, i, out, hole):
    """Append comment/directive/char/string/format spans found in code starting at i.
    `hole` is None at top level, otherwise "regular"/"verbatim" inside an interpolation hole;
    then the index of the hole's closing brace is returned."""
    n, depth = len(src), 0
    while True:
        found = _LEX_STOP.search(src, i)
        if not found:
            if hole:
                raise _Unlexable(i, "unterminated interpolation hole")
            return n
        i = found.start()
        char = src[i]
        if char == "\\":
            raise _Unlexable(i, "backslash outside a literal (Unicode-escaped identifier)")
        if char == "\n" and hole == "regular":
            raise _Unlexable(i, "line break inside a regular interpolated string")
        if char == "#" and hole is None:
            # C# allows '#' only at the start of a preprocessor line. Roslyn also accepts other whitespace
            # (form feed, no-break space, ...) before or after it, which the older regex checks do not, so
            # anything but spaces and tabs (and a BOM at the very start) fails closed (#314 S-2).
            line_start = src.rfind("\n", 0, i) + 1
            if not re.fullmatch(r'\ufeff?[ \t]*' if line_start == 0 else r'[ \t]*', src[line_start:i]):
                raise _Unlexable(i, "'#' that does not start a plain preprocessor line (other whitespace "
                                    "before it, or not a directive)")
            end = src.find("\n", i)
            end = n if end < 0 else end
            if not re.match(r'#[ \t]*(?:[A-Za-z]|\r?$)', src[i:end]):
                raise _Unlexable(i, "whitespace other than spaces and tabs after '#' in a preprocessor line")
            # A conditional/define directive may end in a // comment; #region, #pragma etc. keep free text.
            comment = src.find("//", i, end) if re.match(r'#\s*(?:if|elif|else|endif|define|undef|nullable)\b',
                                                         src[i:end]) else -1
            out.append(("directive", i, end if comment < 0 else comment))
            if comment >= 0:
                out.append(("comment", comment, end))
            i = end
            continue
        if src.startswith("//", i):
            end = src.find("\n", i)
            end = n if end < 0 else end
            out.append(("comment", i, end))
            i = end
            continue
        if src.startswith("/*", i):
            end = src.find("*/", i + 2)
            if end < 0:
                raise _Unlexable(i, "unterminated block comment")
            out.append(("comment", i, end + 2))
            i = end + 2
            continue
        if char == "'":
            literal = _CHAR_LITERAL.match(src, i)
            if not literal:
                raise _Unlexable(i, "unsupported character literal")
            out.append(("char", i, literal.end()))
            i = literal.end()
            continue
        start = _STRING_START.match(src, i) if char in '"@$' else None
        if start:
            prefix = start.group(1) or ""
            quote = start.end() - 1
            if "@" not in prefix and src.startswith('"""', quote):
                raise _Unlexable(i, "raw string literal (C# 11) is unsupported")
            i = _scan_string(src, i, quote + 1, "@" in prefix, "$" in prefix, out)
            continue
        if hole is not None:
            if char in "([{":
                depth += 1
            elif char in ")]":
                depth -= 1
            elif char == "}":
                if depth == 0:
                    return i
                depth -= 1
            elif char == ":" and depth == 0:
                end = src.find("}", i)
                if end < 0 or any(c in src[i:end] for c in '"\n{'):
                    raise _Unlexable(i, "unsupported interpolation format clause")
                out.append(("format", i, end))
                return end
        i += 1


def _scan_string(src, start, i, verbatim, interpolated, out):
    n, holes = len(src), []
    while i < n:
        char = src[i]
        if char == '"':
            if verbatim and src.startswith('""', i):
                i += 2
                continue
            out.append(("string", start, i + 1, verbatim, interpolated, holes))
            return i + 1
        if char == "\n" and not verbatim:
            raise _Unlexable(start, "line break inside a regular string literal")
        if char == "\\" and not verbatim:
            if src.startswith("\\\n", i):
                raise _Unlexable(start, "line break inside a regular string literal")
            i += 2
            continue
        if interpolated and char in "{}":
            if src.startswith(char * 2, i):
                i += 2
                continue
            if char == "}":
                raise _Unlexable(i, "unbalanced '}' in an interpolated string")
            inner = []
            close = _scan_code(src, i + 1, inner, "verbatim" if verbatim else "regular")
            holes.append((i + 1, close, inner))
            i = close + 1
            continue
        i += 1
    raise _Unlexable(start, "unterminated string literal")


_LEXED = {}


def _lexed(source, path):
    """Precise top-level spans. Fails closed on any shape it cannot follow; no literal or
    comment may run across a preprocessor line, so inactive #if text cannot desynchronise it."""
    key = (path, source)
    if key not in _LEXED:
        if len(_LEXED) > 256:
            _LEXED.clear()
        _LEXED[key] = _lex_uncached(source, path)
    return _LEXED[key]


def _lex_uncached(source, path):
    # #314 S-2: Roslyn ends a line (so a // comment or a preprocessor line) at U+0085, U+2028, U+2029
    # and a lone CR; this lexer ends one only at LF. Reject them anywhere rather than model them.
    found = _FOREIGN_LINE_BREAK.search(source)
    if found:
        _fail(source, path, found.start(), f"U+{ord(found.group()):04X} is a line break for the C# compiler but "
                                           "not for this lexer (it can end a // comment or a preprocessor "
                                           "line); fail closed (N-4)")
    spans = []
    try:
        _scan_code(source, 0, spans, None)
    except _Unlexable as problem:
        _fail(source, path, problem.at, f"the lexer cannot follow this source ({problem}); "
                                        "unsupported literal/comment shapes fail closed (N-4)")
    # Roslyn also treats a line indented with other whitespace (form feed, no-break space, BOM ...) as a
    # directive, which matters when the literal or comment opener sits in inactive #if text (#314 loop 1).
    for item in spans:
        if item[0] in ("comment", "string") and re.search(r'\n[\s\ufeff\x1a]*#', source[item[1]:item[2]]):
            _fail(source, path, item[1], "a multi-line literal or comment spans a preprocessor line; "
                                         "fail closed (N-4)")
    _check_code_characters(source, path, spans)
    return spans


# Outside comments and literals, the C# compiler ignores Unicode format characters (category Cf, for
# example U+200D and U+00AD) inside an identifier, so `fo<U+200D>rest` is `forest`; this lexer would see
# two tokens. Code may therefore hold only printable ASCII, tab, LF, CR (as CRLF), a leading BOM and
# non-ASCII letters, digits, combining/connecting marks and space separators (#314 S-2).
_FOREIGN_LINE_BREAK = re.compile('[\x85\u2028\u2029]|\r(?!\n)')
_NOT_PLAIN_ASCII = re.compile(r'[^\t\n\r\x20-\x7e]')
_CODE_CATEGORIES = {"Lu", "Ll", "Lt", "Lm", "Lo", "Nl", "Mn", "Mc", "Nd", "Pc", "Zs"}


def _text_regions(source, spans):
    """Sorted (start, end) spans of comment and literal text, and the free-text message of #region,
    #endregion, #warning and #error lines; interpolation-hole code is excluded."""
    regions = []

    def walk(items):
        for item in items:
            kind, a, b = item[:3]
            if kind in ("comment", "char", "format"):
                regions.append((a, b))
            elif kind == "directive":
                message = re.match(r'#[ \t]*(?:region|endregion|warning|error)\b', source[a:b])
                if message:
                    regions.append((a + message.end(), b))
            elif kind == "string":
                cursor = a
                for start, close, inner in item[5]:
                    regions.append((cursor, start))
                    walk(inner)
                    cursor = close
                regions.append((cursor, b))
    walk(spans)
    return sorted(regions)


def _check_code_characters(source, path, spans):
    regions = None
    for found in _NOT_PLAIN_ASCII.finditer(source):
        at, char = found.start(), found.group()
        if at == 0 and char == "\ufeff":
            continue
        if regions is None:
            regions = _text_regions(source, spans)
            starts = [a for a, _ in regions]
        k = bisect.bisect_right(starts, at) - 1
        if k >= 0 and regions[k][0] <= at < regions[k][1]:
            continue
        category = unicodedata.category(char)
        if char > "\x7f" and category in _CODE_CATEGORIES:
            continue
        _fail(source, path, at, f"character U+{ord(char):04X} (category {category}) in code: the C# compiler "
                                "reads it differently from this lexer (a format character inside an identifier "
                                "is ignored); fail closed (N-4)")


def _lexically_sound(source, path):
    """Precise spans for a file that the legacy regex checks also read: both lexers must agree
    on every literal/comment boundary, otherwise a nested literal could hide code (N-4)."""
    spans = _lexed(source, path)
    legacy = {(m.start(), m.end()) for m in _LEX.finditer(source)}
    precise = set()
    for item in spans:
        if item[0] == "directive":
            continue
        start = item[1]
        if item[0] == "string":
            prefix = source[start:source.index('"', start)]
            start += {"": 0, "@": 0, "$": 1, "$@": 1}.get(prefix, len(prefix))
        precise.add((start, item[2]))
    if legacy != precise:
        _fail(source, path, min(a for a, _ in legacy ^ precise),
              "literal/comment boundaries are ambiguous (for example a literal nested in an "
              "interpolation hole, or a quote in a preprocessor line); fail closed (N-4)")
    return spans


def _precise_mask(source, spans, mask_literals):
    """Same-length text with comments blanked and, optionally, literal and preprocessor-line text
    blanked. Code in interpolation holes always stays visible, so it is never mistaken for text."""
    chars = list(source)

    def blank(a, b):
        for k in range(a, b):
            if chars[k] != "\n":
                chars[k] = " "

    def walk(items):
        for item in items:
            kind, a, b = item[:3]
            if kind == "comment" or (kind in ("char", "format", "directive") and mask_literals):
                blank(a, b)
            elif kind == "string":
                cursor = a
                for start, close, inner in item[5]:
                    if mask_literals:
                        blank(cursor, start - 1)  # the hole's braces stay visible, so commas inside nest
                    walk(inner)
                    cursor = close + 1
                if mask_literals:
                    blank(cursor, b)
    walk(spans)
    return "".join(chars)


def _string_items(spans):
    for item in spans:
        if item[0] == "string":
            yield item
            for _, _, inner in item[5]:
                yield from _string_items(inner)


def _string_value(source, item):
    """Decoded text of a string span; interpolation holes become NUL separators."""
    _, a, b, verbatim, interpolated, holes = item
    quote = source.index('"', a)
    if not interpolated:
        return _decode(source[a:b] if verbatim else source[quote:b])
    parts, cursor = [], quote + 1
    for start, close, _ in holes:
        parts.append(source[cursor:start - 1])
        cursor = close + 1
    parts.append(source[cursor:b - 1])

    def text(part):
        part = part.replace("{{", "{").replace("}}", "}")
        return part.replace('""', '"') if verbatim else _decode('"' + part + '"')
    return "\0".join(text(part) for part in parts)


def _string_values(source, spans):
    """(start, decoded value, interpolated) for every string literal, nested ones included."""
    return [(item[1], _string_value(source, item), item[4]) for item in _string_items(spans)]


# ------------------------------------------------------------- tokens and pins

_Tok = namedtuple("_Tok", "text kind start end value")
_TOKEN = re.compile(r'@?[A-Za-z_]\w*|\d\w*(?:\.\d\w*)*|\?\?=|<<=|=>|==|!=|<=|>=|&&|\|\||\?\?|\?\.'
                    r'|\+\+|--|\+=|-=|\*=|/=|%=|&=|\|=|\^=|<<|->|::|\S')


def _tokens(source, spans, start=0, end=None):
    """Tokens of source[start:end] with comments dropped. A whole literal is one token
    (kind str/istr/char); a preprocessor line is one token (kind dir)."""
    end = len(source) if end is None else end
    out = []

    def code(a, b):
        for match in _TOKEN.finditer(source, a, b):
            text = match.group()
            kind = "id" if text[0].isalpha() or text[0] in "_@" else "num" if text[0].isdigit() else "op"
            out.append(_Tok(text, kind, match.start(), match.end(), text.lstrip("@") if kind == "id" else text))
    cursor = start
    for item in spans:
        kind, a, b = item[:3]
        if b <= start or a >= end:
            continue
        if a < start or b > end:
            _fail(source, "source", a, "a literal or comment straddles a checked region")
        code(cursor, a)
        if kind == "string":
            out.append(_Tok(source[a:b], "istr" if item[4] else "str", a, b, _string_value(source, item)))
        elif kind == "char":
            out.append(_Tok(source[a:b], "char", a, b, source[a:b]))
        elif kind == "directive":
            out.append(_Tok(" ".join(source[a:b].split()), "dir", a, b, None))
        cursor = b
    code(cursor, end)
    return out


def _texts(tokens):
    return [token.value if token.kind == "id" else token.text for token in tokens]


_FULL_TOKENS = {}


def _file_tokens(source, spans):
    """Tokens and their texts for a whole file, cached by text (performance only)."""
    if source not in _FULL_TOKENS:
        if len(_FULL_TOKENS) > 64:
            _FULL_TOKENS.clear()
        tokens = _tokens(source, spans)
        _FULL_TOKENS[source] = (tokens, _texts(tokens))
    return _FULL_TOKENS[source]


_FRAGMENT_TOKENS = {}


def _tokens_of(fragment):
    """Normalised token texts of a reviewed C# fragment (comments and layout ignored)."""
    if fragment not in _FRAGMENT_TOKENS:
        _FRAGMENT_TOKENS[fragment] = _texts(_tokens(fragment, _lexed(fragment, "<pinned text>")))
    return _FRAGMENT_TOKENS[fragment]


def _statements(tokens):
    """Split tokens at semicolons outside brackets. Not a statement parser: a block without a
    trailing semicolon joins the next statement, which only makes the shape checks stricter."""
    out, current, depth = [], [], 0
    for token in tokens:
        current.append(token)
        if token.kind == "op" and token.text in ("(", "[", "{"):
            depth += 1
        elif token.kind == "op" and token.text in (")", "]", "}"):
            depth -= 1
        elif token.kind == "op" and token.text == ";" and depth == 0:
            out.append(current)
            current = []
    if current:
        out.append(current)
    return out


def _split_args(tokens):
    args, current, depth = [], [], 0
    for token in tokens:
        if token.kind == "op" and token.text in ("(", "[", "{"):
            depth += 1
        elif token.kind == "op" and token.text in (")", "]", "}"):
            depth -= 1
        if token.kind == "op" and token.text == "," and depth == 0:
            args.append(current)
            current = []
        else:
            current.append(token)
    if current or args:
        args.append(current)
    return args


def _no_attribute(source, signature, path):
    """A checked method may carry no attribute: [Conditional] would compile its calls away."""
    masked = _code(source, mask_literals=True)
    found = re.search(signature, masked)
    if found and masked[:found.start()].rstrip().endswith("]"):
        _fail(source, path, found.start(), "an attribute on a checked method is unsupported (e.g. [Conditional])")


def _pin(source, spans, path, signature, expected, label):
    """The method body must equal the reviewed text token for token (comments/layout free)."""
    start, end = _method_span(source, signature)
    _no_attribute(source, signature, path)
    got = _tokens(source, spans, start, end)
    want = _tokens_of(expected)
    texts = _texts(got)
    if texts != want:
        at = next((k for k, (a, b) in enumerate(zip(texts, want)) if a != b), min(len(texts), len(want)))
        where = got[at].start if at < len(got) else end
        _fail(source, path, where,
              f"pinned method {label} changed (expected {want[at] if at < len(want) else 'end of body'!r}, "
              f"found {texts[at] if at < len(texts) else 'end of body'!r}); a reviewed method may change "
              "only together with this checker (#309 RR-2/RR-3)")
    return start, end


def _snippet_spans(source, spans, snippet, path, label):
    """Character span of the single exact occurrence of a reviewed token sequence."""
    want = _tokens_of(snippet)
    tokens, texts = _file_tokens(source, spans)
    hits = [k for k in range(len(texts) - len(want) + 1) if texts[k:k + len(want)] == want]
    if len(hits) != 1:
        _fail(source, path, tokens[hits[1]].start if len(hits) > 1 else 0,
              f"{label}: expected exactly one reviewed occurrence of `{snippet}` (missing, changed or "
              f"duplicated), found {len(hits)} (RR-2/RR-3)")
    start = tokens[hits[0]].start
    # #314 loop 1: reviewed text inside #if could be compiled out while still being found here.
    depth = 0
    for item in spans:
        if item[0] == "directive" and item[1] < start:
            word = re.match(r'#[ \t]*(\w+)', source[item[1]:item[2]])
            depth += {"if": 1, "endif": -1}.get(word.group(1) if word else "", 0)
    if depth > 0:
        _fail(source, path, start, f"{label}: reviewed text `{snippet}` lies inside conditional compilation "
                                   "(#if); fail closed (RR-2/RR-3)")
    return start, tokens[hits[0] + len(want) - 1].end


def _positions(masked, name, start=0, end=None):
    end = len(masked) if end is None else end
    return [m.start() for m in re.finditer(r'(?<!\w)@?' + re.escape(name) + r'(?!\w)', masked)
            if start <= m.start() < end]


def _inside(position, regions):
    return any(a <= position < b for a, b in regions)


# ------------------------------------------------------------- reviewed text

# Bodies of the reviewed ScenarioOneUiRoot methods at 1585ca20 (comments removed; layout is
# irrelevant). Any change, however small, must be reviewed and copied here deliberately.
_ROOT_PINS = (
    ("RefreshNextActionGuidance", r'\bpublic\s+void\s+RefreshNextActionGuidance\s*\(\s*\)', r'''
if (learning == null || map == null || workPlan == null || manager == null) return;
ClearNextAction();
guidanceCandidate = null;
guidanceScope = null;
foreach (var entry in guidancePrimaries)
    entry.Key.EnableInClassList("btn-primary", entry.Value);
guidancePrimaries.Clear();
foreach (VisualElement view in new[] { learning.Root, map.Root, workPlan.Root, review.Root })
{
    Label note = view.Q<Label>("next-action-reason");
    if (note != null) { note.text = ""; note.style.display = DisplayStyle.None; }
}
VisualElement scope = manager.WorkPlanOpen ? workPlan.Root
    : screen == UiScreen.Objectives ? learning.Root : screen == UiScreen.Map ? map.Root
    : screen == UiScreen.Review ? review.Root : null;
if (scope == null) return;
string target = null;
string reason = learning.CurrentInstruction;
bool mapQuestions = scope == map.Root && map.RackMode;
bool consent = mapQuestions && map.RackEditor.ConsentPending;
bool reset = mapQuestions && map.RackEditor.ResetPending;
if (consent && reset)
    reason = "Two rack questions are open; keep or dismiss them before continuing.";
else if (consent) target = "map-consent-decline";
else if (reset) target = "map-edit-reset-cancel";
else if (mapQuestions && map.ReopenPending) target = "map-reopen-decline";
else if (scope == workPlan.Root && workPlan.ConfirmingAdvance) target = "workplan-advance-return";
else if (manager.RackPlanningV2Active && (learning.TeachingRacks
    || (NeedsAnnualReview && manager.AnnualReports.Count == 1 && rackLayout != null && rackLayout.Harvested)))
{
    bool objectives = scope == learning.Root;
    bool onMap = scope == map.Root;
    if (NeedsAnnualReview)
        target = scope == review.Root ? "annual-review-acknowledge"
            : objectives ? "objective-work-plan" : onMap ? "map-work-plan" : "workplan-read-results";
    else if (!manager.RackPlanConfirmed)
    {
        if (objectives) target = "objective-primary-cta";
        else if (onMap)
        {
            if (!map.RackPlanningAvailable)
                reason = !manager.CanEditWorkPlan ? manager.OutcomeReason + " Work Plan editing is unavailable."
                    : "Rack planning requires the rack tool and forest data to be available.";
            else if (!map.RackMode) target = "map-plan-rack";
            else if (map.RackEditor.Dragging || map.RackEditor.Ghost.HasValue)
                reason = string.IsNullOrEmpty(map.RackEditor.Status) ? learning.CurrentInstruction : map.RackEditor.Status;
            else if (manager.CanConfirmRackPlan(out string problem)) target = "map-rack-plan-confirm";
            else reason = problem;
        }
    }
    else if (manager.MarkedRackEntranceCount < ScenarioOneManager.RackPlanTargetCount)
    {
        bool markable = false;
        string unavailableReason = "";
        foreach (RackPlanRack rack in manager.RackPlan)
        {
            if (rack.EntranceMarked || rack.IsLocked) continue;
            if (manager.TryGetRackEntranceTarget(rack.Id, out ForestTree tree, out string problem)
                && !tree.IsCropTree && !tree.IsMarkedForFell) { markable = true; break; }
            else if (!string.IsNullOrEmpty(problem)) unavailableReason = problem;
        }
        if (markable) target = objectives ? "objective-back-to-forest" : onMap ? "map-back-to-forest" : null;
        else reason = "No unmarked rack entrance is currently available to mark. " + unavailableReason;
    }
    else if (rackLayout != null)
    {
        string problem = RackPlanProblem;
        if (string.IsNullOrEmpty(problem) && rackLayout.WorkPending) problem = RackApprovalProblem;
        if (!string.IsNullOrEmpty(problem)) reason = problem;
        else if (rackLayout.WorkPending)
        {
            target = objectives ? "objective-work-plan" : onMap ? "map-work-plan" : "workplan-approve";
            reason = "Review every pending job and its cost: Approve Work Plan commits all eligible pending work, including jobs outside the racks.";
            if (scope == workPlan.Root)
            {
                ScenarioHarvestJob quote = manager.GetHarvestQuote(false);
                long cost = manager.GetWorkPlanSummary().ExternalCostCents;
                if (!quote.Eligible || cost > manager.CashCents)
                {
                    target = null;
                    reason = !quote.Eligible ? quote.Problem : $"Approval needs {UiKit.Money(cost)} including approved work; available cash is {UiKit.Money(manager.CashCents)}.";
                }
                foreach (ScenarioOneWorkOrder order in manager.WorkOrders)
                    if (order.rackId > 0 && order.status == ScenarioWorkStatus.Pending && !string.IsNullOrEmpty(order.validationMessage))
                    { target = null; reason = order.validationMessage; break; }
            }
        }
        else
        {
            bool approved = false, failed = false;
            foreach (RackPlanRack rack in manager.RackPlan)
            {
                approved |= rack.Orders.Approved > 0;
                failed |= rack.Orders.Failed > 0;
            }
            if (rackLayout.Complete && approved && !failed && manager.CanAdvanceYearNow && ecology != null)
                target = objectives ? "objective-work-plan" : onMap ? "map-work-plan" : "workplan-advance";
            else reason = "Review the rack work and its results in the Work Plan. " + manager.Feedback;
        }
    }
}
else return;
foreach (string name in GuidanceButtonNames)
{
    Button owned = scope.Q<Button>(name);
    if (owned == null) continue;
    guidancePrimaries[owned] = owned.ClassListContains("btn-primary");
    owned.RemoveFromClassList("btn-primary");
}
guidanceScope = scope;
guidanceCandidate = target != null ? scope.Q<Button>(target) : null;
Label explanation = scope.Q<Label>("next-action-reason");
if (explanation != null)
{
    explanation.text = reason;
    explanation.style.display = string.IsNullOrEmpty(reason) ? DisplayStyle.None : DisplayStyle.Flex;
}
ApplyNextActionGuidance();
'''),
    ("ClearNextAction", r'\bprivate\s+void\s+ClearNextAction\s*\(\s*\)', r'''
if (nextAction != null)
{
    nextAction.RemoveFromClassList("btn-primary");
    nextAction.RemoveFromClassList("next-action");
    nextAction = null;
}
nextBadge.RemoveFromHierarchy();
'''),
    ("ApplyNextActionGuidance", r'\bprivate\s+void\s+ApplyNextActionGuidance\s*\(\s*\)', r'''
bool suspended = manager == null || manager.ReferencePreviewActive || StandaloneSessionMenu.IsOpen
    || help == null || help.IsOpen || helpClosedFrame == Time.frameCount;
Button candidate = guidanceCandidate;
VisualElement scope = guidanceScope;
bool eligible = !suspended && candidate != null && candidate.enabledInHierarchy && GuidanceVisible(candidate, scope);
if (nextAction == candidate && eligible) return;
ClearNextAction();
if (!eligible) return;
nextAction = candidate;
nextBadge.AddToClassList("next-action-badge");
candidate.parent.Insert(candidate.parent.IndexOf(candidate), nextBadge);
candidate.AddToClassList("btn-primary");
candidate.AddToClassList("next-action");
'''),
    ("GuidanceVisible", r'\bprivate\s+static\s+bool\s+GuidanceVisible\s*\(\s*VisualElement\s+element\s*,'
                        r'\s*VisualElement\s+scope\s*\)', r'''
for (VisualElement current = element; current != null; current = current.parent)
{
    if (current.style.display == DisplayStyle.None || current.resolvedStyle.display == DisplayStyle.None
        || current.resolvedStyle.visibility == Visibility.Hidden) return false;
    if (current == scope) return true;
}
return false;
'''),
    ("CloseHelp", r'\bpublic\s+void\s+CloseHelp\s*\(\s*\)', r'''
if (help == null || !help.IsOpen) return;
help.Close();
helpClosedFrame = Time.frameCount;
manager.SetAuxiliaryPanelOpen(true);
'''),
    ("SetScreen", r'\bpublic\s+void\s+SetScreen\s*\(\s*UiScreen\s+next\s*\)', r'''
if (screen == next)
    return;
if (screen == UiScreen.Map && next == UiScreen.None && map.HasWaypoint)
    learning.Record("map.return");
screen = next;
CloseHelp();
manager.SetAuxiliaryPanelOpen(screen != UiScreen.None);
if (screen == UiScreen.Review)
{
    review.ShowLatest();
    review.Refresh(true);
}
else if (screen == UiScreen.Map)
{
    learning.Record("map.open");
    map.Refresh(true);
}
else if (screen == UiScreen.Objectives)
    learning.Refresh(true);
if (screen == UiScreen.Review) RefreshNextActionGuidance();
'''),
    ("ShowMap", r'\bpublic\s+void\s+ShowMap\s*\(\s*\)', r'''
manager.OpenWorkPlan(false);
SetScreen(UiScreen.Map);
'''),
)

# Outside the pinned bodies, these identifiers may appear only inside the reviewed snippets
# below, each snippet exactly once: no qualified (this.x), tuple, ref/out, lambda, shadowing
# or count-compensated reference can be added anywhere else in the root (RR-2/RR-3).
_ROOT_GUARDED = ("helpClosedFrame", "nextAction", "guidanceCandidate", "guidanceScope", "guidancePrimaries",
                 "GuidanceButtonNames", "ApplyNextActionGuidance", "ClearNextAction", "GuidanceVisible")
_ROOT_SNIPPETS = (
    "private int helpClosedFrame = -1;",
    "private Button nextAction;",
    "private Button guidanceCandidate;",
    "private VisualElement guidanceScope;",
    "private readonly Dictionary<Button, bool> guidancePrimaries = new Dictionary<Button, bool>();",
    "private static readonly string[] GuidanceButtonNames = {",
    "manager.SetAuxiliaryPanelOpen(screen != UiScreen.None || help.IsOpen || helpClosedFrame == Time.frameCount);",
    "private void LateUpdate() { ApplyNextActionGuidance();",
    "private void ClearNextAction()",
    "private void ApplyNextActionGuidance()",
    "private static bool GuidanceVisible(VisualElement element, VisualElement scope)",
    "root.Add(learning.Root);",
    "private LearningObjectivesView learning;",
    "public LearningObjectivesView Learning => learning;",
    "learning = new LearningObjectivesView(this);",
    "learning.Root.style.display = !preview && screen == UiScreen.Objectives ? DisplayStyle.Flex : DisplayStyle.None;",
)

# #314 S-1: root fields the pinned guidance reads ("related state"). A write count alone let a tuple
# deconstruction or a compensated move through, so each field may be written only at its reviewed site:
# the screen initialiser, one top-level statement of Start for each reference, and the pinned SetScreen
# and TryShowReviewYear bodies. Their declarations are pinned so none can become a property.
_STATE_FIELDS = ("help", "manager", "learning", "map", "workPlan", "review", "rackLayout", "screen")
_STATE_DECLARATIONS = (
    "private ScenarioOneManager manager;", "private WorkPlanView workPlan;", "private AnnualReviewView review;",
    "private StandMapView map;", "private RackLayoutTracker rackLayout;", "private MenuHelpView help;",
    "private UiScreen screen = UiScreen.None;",
)
_START = r'\bprivate\s+void\s+Start\s*\(\s*\)'
_STATE_START_WRITES = (
    "manager = GetComponent<ScenarioOneManager>() ?? FindFirstObjectByType<ScenarioOneManager>();",
    "rackLayout = new RackLayoutTracker(this);",
    "learning = new LearningObjectivesView(this);",
    "workPlan = new WorkPlanView(this);",
    "review = new AnnualReviewView(this);",
    "map = new StandMapView(this);",
    "help = new MenuHelpView(this);",
)
_TRY_SHOW_REVIEW_YEAR = (r'\bpublic\s+bool\s+TryShowReviewYear\s*\(\s*int\s+year\s*\)', r'''
if (!review.TryShowYear(year)) return false;
manager.OpenWorkPlan(false);
screen = UiScreen.Review;
CloseHelp();
manager.SetAuxiliaryPanelOpen(true);
return true;
''')
_ASSIGNMENT_OPERATORS = {"=", "+=", "-=", "*=", "/=", "%=", "&=", "|=", "^=", "<<=", "??="}
# Keywords that may precede '(' without making it a call.
_NOT_CALLEES = {"if", "while", "for", "foreach", "switch", "using", "lock", "catch", "return", "typeof", "sizeof",
                "nameof", "default", "checked", "unchecked", "when", "case", "in", "is", "as", "else", "do",
                "throw", "yield", "await", "fixed"}
_ROOT_UNSAFE = (r'\b(?:unsafe|__makeref|__refvalue|__reftype|__arglist)\b',
                "unsafe code or an undocumented reference keyword (__makeref)")

# #314 S-3: inputs that the pinned routing reads, pinned to their reviewed definitions. The game and
# teaching state they read in turn (ScenarioOneManager, ScenarioOneTeachingSequence, TeachingFacts and
# the lesson progress) is not checked; README "Known limits".
_ROUTING_INPUT_ROOT = ("public bool NeedsAnnualReview => manager != null && manager.AnnualReports.Count > 0 "
                       "&& !manager.AnnualReviewSeen;")
_ROUTING_INPUTS_VIEW = ("public int CurrentTopicIndex => ScenarioOneTeachingSequence.Current(TeachingFacts());",
                        "public bool TeachingRacks => CurrentTopicIndex == 0;")

_KIT_PINS = (
    ("UiKit.Button", r'\bpublic\s+static\s+Button\s+Button\s*\(\s*string\s+text\s*,\s*Action\s+onClick\s*,'
                     r'\s*bool\s+enabled\s*=\s*true\s*,\s*params\s+string\s*\[\s*\]\s+classes\s*\)', r'''
var button = new Button(onClick) { text = text };
button.AddToClassList("btn");
foreach (string c in classes) button.AddToClassList(c);
button.SetEnabled(enabled);
button.focusable = false;
return button;
'''),
    ("UiKit.Box", r'\bpublic\s+static\s+VisualElement\s+Box\s*\(\s*params\s+string\s*\[\s*\]\s+classes\s*\)', r'''
var element = new VisualElement();
foreach (string c in classes) element.AddToClassList(c);
return element;
'''),
    ("UiKit.MakeModal", r'\bpublic\s+static\s+void\s+MakeModal\s*\(\s*VisualElement\s+backdrop\s*\)', r'''
backdrop.pickingMode = PickingMode.Position;
backdrop.RegisterCallback<PointerDownEvent>(e => e.StopPropagation());
backdrop.RegisterCallback<PointerUpEvent>(e => e.StopPropagation());
backdrop.RegisterCallback<WheelEvent>(e => e.StopPropagation());
'''),
    ("UiKit.Text", r'\bpublic\s+static\s+Label\s+Text\s*\(\s*string\s+text\s*,\s*params\s+string\s*\[\s*\]\s+classes\s*\)', r'''
var label = new Label(text);
foreach (string c in classes) label.AddToClassList(c);
if (Array.IndexOf(classes, "title") >= 0 && Array.IndexOf(classes, "money") < 0)
{
    if (displayFont == null)
        displayFont = UnityEngine.Resources.Load<UnityEngine.Font>("Fonts/LiberationSerif-Regular");
    if (displayFont != null)
        label.style.unityFontDefinition = FontDefinition.FromFont(displayFont);
}
return label;
'''),
    ("UiKit.Add", r'\bpublic\s+static\s+Label\s+Add\s*\(\s*VisualElement\s+parent\s*,\s*string\s+text\s*,'
                  r'\s*params\s+string\s*\[\s*\]\s+classes\s*\)', r'''
Label label = Text(text, classes);
parent.Add(label);
return label;
'''),
    ("UiKit.Row", r'\bpublic\s+static\s+VisualElement\s+Row\s*\(\s*VisualElement\s+parent\s*,'
                  r'\s*params\s+string\s*\[\s*\]\s+classes\s*\)', r'''
VisualElement row = Box(classes.Length > 0 ? classes : new[] { "row" });
parent.Add(row);
return row;
'''),
)

# Index range of class-name arguments for each UiKit member that attaches classes; other
# reviewed members attach none of their caller's classes. Unknown members fail closed.
_UIKIT_CLASS_ARGS = {"Box": (0, None), "Row": (1, None), "Text": (1, None), "Add": (2, None),
                     "Button": (3, None), "Chip": (2, 2), "Line": (3, 3)}
_UIKIT_OTHER = {"MakeModal", "SetCompact", "Money", "SignedMoney", "F", "M3", "CellLabel", "Stat", "Inv",
                "AccentColour", "WaypointColour", "SelectionColour", "WarningColour", "ForestryColour", "RackColour"}

# The reviewed default Objectives header (N-1): caption, click method, enabled argument,
# static primary, element name. Exactly these three, plus optional non-primary extras.
_KNOWN_HEADER_BUTTONS = (
    ("Stand map [M]", "ShowMap", None, False, CTA_NAME),
    ("Work Plan [Tab]", "ShowWorkPlan", None, False, "objective-work-plan"),
    ("Back to forest [O / Esc]", "CloseAll", "true", True, "objective-back-to-forest"),
)
_VIEW_REFRESH = r'\bpublic\s+void\s+Refresh\s*\(\s*bool\s+force\s*\)'
_VIEW_REFRESH_GUARD = ("if (!force && shownGuidance == guidance && shownCount == done.Count && "
                       "shownRegenerationModel == regenerationModel && shownMarket == (market ? 1 : 0) && "
                       "shownObjectives == objectiveText) return;")
_VIEW_BANS = (
    (r'\w*ClassList\w*', "a class-list API (only the reviewed header may style a button)"),
    (r'(?<!\w)@?Conditional\w*', "a [Conditional] attribute"),
    (r'\busing\s+static\b', "a using-static directive"),
    (r'\.\s*@?(?:parent|hierarchy|Children|ElementAt|panel|visualTree|contentContainer|Q|Query'
     r'|RootElement|rootVisualElement|GetFirstAncestorOfType|GetFirstOfType|FindCommonAncestor)\b',
     "element-tree traversal or query"),
    (r'(?<![.\w])@?(?:Q|Query|UQueryExtensions|RootElement|rootVisualElement)\b', "element-tree traversal or query"),
)
# After `.Learning` in another class: a member other than Root, or a null test (L-06).
_LEARNING_USE = re.compile(r'\s*(?:\??\.\s*@?(?!Root(?!\w))[A-Za-z_]|[!=]=\s*null(?!\w))')
_DECLARATION = re.compile(r'\b(?:class|struct|interface|record|enum)\s+@?(LearningObjectivesView|ScenarioOneUiRoot|UiKit)\b')


# ------------------------------------------------------------- scope (N-3)

def _scope(view_source, root_source, view_spans, root_spans, companions):
    """Which files declare the view class, the root and UiKit. The root and UiKit must be single,
    non-partial declarations in their reviewed files; every LearningObjectivesView partial is read."""
    files = {VIEW: (view_source, view_spans), ROOT: (root_source, root_spans)}
    for path in sorted(companions):
        if path in files:
            raise ObjectiveCtaSourceError(f"companion source duplicates {path}")
        source = companions[path]
        files[path] = (source, _lexically_sound(source, path) if path == UIKIT else _lexed(source, path))
    declared = {}
    for path, (source, spans) in files.items():
        masked = _precise_mask(source, spans, True)
        for match in _DECLARATION.finditer(masked):
            head = masked[max(masked.rfind(c, 0, match.start()) for c in ";{}]") + 1:match.start()]
            declared.setdefault(match.group(1), []).append((path, bool(re.search(r'\bpartial\b', head))))
    for name, path in (("ScenarioOneUiRoot", ROOT), ("UiKit", UIKIT)):
        if declared.get(name, []) != [(path, False)]:
            raise ObjectiveCtaSourceError(
                f"{name} must be declared exactly once, non-partial, in {path}; found {declared.get(name, [])} (N-3)")
    views = declared.get("LearningObjectivesView", [])
    paths = [path for path, _ in views]
    if VIEW not in paths or len(paths) != len(set(paths)) or not all(partial for _, partial in views):
        raise ObjectiveCtaSourceError(f"unexpected LearningObjectivesView declarations {views} (N-3)")
    # Other files that mention these classes may not name an Objectives header button or reach the
    # Objectives Root through ui.Learning (whole or escaped literals only; other classes are not
    # otherwise analysed).
    header_names = {name for *_, name in _KNOWN_HEADER_BUTTONS}
    for path, (source, spans) in files.items():
        if path in (VIEW, ROOT) or path in paths:
            continue
        masked = _precise_mask(source, spans, True)
        found = re.search(r'(?<!\w)@?Learning\s*\??\.\s*@?Root\b', masked)
        if found:
            _fail(source, path, found.start(), "another class reaches the Objectives Root (RR-1)")
        for start, value, interpolated in _string_values(source, spans):
            if any(name in value if interpolated else name == value for name in header_names):
                _fail(source, path, start, "another class names an Objectives header button (RR-1)")
        # #314 loop 1 (L-06): `.Learning` may only be followed by a member access (not Root) or a null
        # comparison, so the view object cannot be aliased (`var v = ui.Learning;`) or passed on first.
        for found in re.finditer(r'(?:\?\.|\.)\s*@?Learning(?!\w)', masked):
            if not _LEARNING_USE.match(masked, found.end()):
                _fail(source, path, found.start(), "another class reaches the Objectives view other than by a member "
                                                   "access or null test on .Learning (alias or pass-on, L-06) (RR-1)")
    return {path: files[path] for path in paths}, files[UIKIT]


def _class_wide_guards(view_source, root_source, view_spans, root_spans, companions):
    if companions is None:
        raise ObjectiveCtaSourceError(
            "companion sources (LearningObjectivesView partials and UiKit.cs) were not supplied, so the "
            "class-wide guards cannot run; pass collect_companions(repository root) (N-3)")
    view_files, (kit_source, kit_spans) = _scope(view_source, root_source, view_spans, root_spans, companions)
    allowed = _allowlist_values(_code(root_source))
    _guard_view_class(view_source, view_spans, view_files, allowed)
    _guard_root(root_source, root_spans)
    _guard_uikit(kit_source, kit_spans)


# ------------------------------------------------------------- UiKit calls

def _check_uikit_calls(path, source, spans, masked, skip=(), primary_ok=()):
    """UiKit is the only class-attaching helper a checked file may call. Every class-name
    argument must be a plain literal; btn-primary only at the reviewed call sites."""
    commented = _precise_mask(source, spans, False)
    for use in re.finditer(r'(?<!\w)@?UiKit\b', masked):
        if _inside(use.start(), skip):
            continue
        member = re.match(r'\s*\.\s*@?(\w+)', masked[use.end():])
        if not member:
            _fail(source, path, use.start(), "UiKit used other than as UiKit.Member (alias or method group)")
        name = member.group(1)
        if name in _UIKIT_OTHER:
            continue
        if name not in _UIKIT_CLASS_ARGS:
            _fail(source, path, use.start(), f"UiKit.{name} is not a reviewed UiKit member; its class "
                                             "arguments cannot be checked")
        after = use.end() + member.end()
        call = re.match(r'\s*\(', masked[after:])
        if not call:
            _fail(source, path, use.start(), f"UiKit.{name} used as a method group")
        args, _ = _args(masked, after + call.end() - 1)
        if len(args) == 1 and not masked[args[0][0]:args[0][1]].strip():
            args = []
        first, last = _UIKIT_CLASS_ARGS[name]
        for index, (a, b) in enumerate(args):
            text = commented[a:b]
            if re.match(r'\s*@?\w+\s*:(?!:)', text):
                _fail(source, path, a, f"named argument in UiKit.{name} is unsupported")
            if index < first or (last is not None and index > last):
                continue
            value = _sole_literal(text)
            if value is None:
                _fail(source, path, a, f"UiKit.{name} class argument is not a plain string literal")
            if PRIMARY_CLASS in value and use.start() not in primary_ok:
                _fail(source, path, a, f"btn-primary attached by UiKit.{name} outside the reviewed site (RR-1)")


# ------------------------------------------------------------- view class (RR-1, RR-3)

def _guard_view_class(view_source, view_spans, view_files, allowed):
    forest_call = _guard_header(view_source, view_spans, allowed)
    ctor_start, ctor_end = _method_span(view_source, _CTOR_SIGNATURE)
    _no_attribute(view_source, _CTOR_SIGNATURE, VIEW)
    root_decl = _snippet_spans(view_source, view_spans, "public VisualElement Root { get; }", VIEW, "view Root")
    primary_literals, name_literals, caption_literals, constructors = [], [], [], 0
    for path, (source, spans) in sorted(view_files.items()):
        masked = _precise_mask(source, spans, True)
        for pattern, what in _VIEW_BANS:
            found = re.search(pattern, masked)
            if found:
                _fail(source, path, found.start(), f"{what} in the Objectives view class is unsupported (RR-1/RR-3)")
        constructors += len(re.findall(r'(?<![\w.])@?LearningObjectivesView\s*\(', masked))
        for at in _positions(masked, "Root"):
            if not (path == VIEW and (_inside(at, [root_decl]) or ctor_start <= at < ctor_end)):
                _fail(source, path, at, "the Objectives Root may be used only by its declaration and the "
                                        "constructor (RR-1)")
        _check_uikit_calls(path, source, spans, masked, primary_ok=(forest_call,) if path == VIEW else ())
        for start, value, interpolated in _string_values(source, spans):
            if PRIMARY_CLASS in value:
                primary_literals.append((path, start))
            if any(name in value if interpolated else name == value for *_, name in _KNOWN_HEADER_BUTTONS):
                name_literals.append((path, start))
            if any(caption == value for caption, *_ in _KNOWN_HEADER_BUTTONS):
                caption_literals.append((path, start))
    if constructors != 1:
        raise ObjectiveCtaSourceError(f"LearningObjectivesView must have exactly one constructor; found {constructors}")
    if len(primary_literals) != 1:
        path, at = (primary_literals + [(VIEW, 0)])[1 if primary_literals else 0]
        _fail(view_files[path][0], path, at, f"the Objectives view class must contain exactly one btn-primary "
                                             f"literal (Back to forest); found {len(primary_literals)} (RR-1, N-1)")
    if len(name_literals) != 3 or len(caption_literals) != 3:
        raise ObjectiveCtaSourceError(
            "each Objectives header name and caption must appear exactly once in the view class; found "
            f"{len(name_literals)} names and {len(caption_literals)} captions (RR-1)")
    _guard_refresh_caller(view_source, view_spans, view_files)
    # #314 S-3: the pinned routing reads these view properties; their reviewed definitions are pinned too.
    for snippet in _ROUTING_INPUTS_VIEW:
        _snippet_spans(view_source, view_spans, snippet, VIEW, f"RR-3 routing input '{snippet.split()[2]}'")


def _guard_header(source, spans, allowed):
    """Straight-line grammar of the reviewed Objectives header (RR-1, N-1). Returns the start of
    the Back to forest UiKit.Button call, the one reviewed static btn-primary site."""
    start, end = _method_span(source, _CTOR_SIGNATURE)
    tokens = [t for t in _tokens(source, spans, start, end)
              if not (t.kind == "dir" and re.match(r'#\s*(?:region|endregion)\b', t.text))]
    masked = _precise_mask(source, spans, True)
    # `ui` is the constructor parameter captured by the click lambdas; reassigning it would retarget them.
    allowed_at = {"header": set(), "buttons": set(), "modal": set(), "Root": set(), "ui": set()}
    decls, bindings, adds, inline, counts = {}, {}, {}, [], {}

    def note(role, statement, *names):
        """Record a reviewed statement and the exact token positions of the locals it may use."""
        counts[role] = counts.get(role, 0) + 1
        for token in statement:
            if token.kind == "id" and token.value in names:
                allowed_at.setdefault(token.value, set()).add(token.start)

    def plain_args(args):
        return all(len(arg) == 1 and arg[0].kind == "str" for arg in args)

    def button_args(at, args, what):
        if len(args) < 2 or len(args[0]) != 1 or args[0][0].kind != "str":
            _fail(source, VIEW, at, f"{what}: caption is not a plain string literal (RR-1)")
        click = _texts(args[1])
        if len(click) != 8 or click[:5] != ["(", ")", "=>", "ui", "."] or click[6:] != ["(", ")"] \
                or args[1][5].kind != "id":
            _fail(source, VIEW, at, f"{what}: click is not `() => ui.Method()` (RR-1)")
        allowed_at["ui"].add(args[1][3].start)
        enabled = None
        if len(args) > 2:
            if len(args[2]) != 1 or _texts(args[2])[0] not in ("true", "false"):
                _fail(source, VIEW, at, f"{what}: enabled argument is not true/false (RR-1)")
            enabled = _texts(args[2])[0]
        if not plain_args(args[3:]):
            _fail(source, VIEW, at, f"{what}: class argument is not a plain string literal (RR-1)")
        return dict(caption=args[0][0].value, click=click[5], enabled=enabled,
                    classes=[arg[0].value for arg in args[3:]], at=at)

    statements = _statements(tokens)
    for statement in statements:
        t = _texts(statement)
        if t == ["this", ".", "ui", "=", "ui", ";"]:
            note("this ui", statement, "ui")
        elif t == ["VisualElement", "header", "=", "UiKit", ".", "Box", "(", '"modal-header"', ")", ";"]:
            note("header", statement, "header")
        elif t == ["VisualElement", "buttons", "=", "UiKit", ".", "Row", "(", "header", ")", ";"]:
            note("row", statement, "header", "buttons")
        elif t == ["modal", ".", "Add", "(", "header", ")", ";"]:
            note("attach", statement, "header", "modal")
        elif t[:6] == ["UiKit", ".", "Add", "(", "header", ","] and t[-2:] == [")", ";"] \
                and plain_args(_split_args(statement[6:-2])):
            note("title", statement, "header")
        elif t[:6] == ["Root", "=", "UiKit", ".", "Box", "("] and t[-2:] == [")", ";"] \
                and plain_args(_split_args(statement[6:-2])):
            note("root box", statement, "Root")
        elif t == ["UiKit", ".", "MakeModal", "(", "Root", ")", ";"]:
            note("make modal", statement, "Root")
        elif t == ["Root", ".", "Add", "(", "modal", ")", ";"]:
            note("root add", statement, "Root", "modal")
        elif t[:7] == ["VisualElement", "modal", "=", "UiKit", ".", "Box", "("] and t[-2:] == [")", ";"] \
                and plain_args(_split_args(statement[7:-2])):
            note("modal", statement, "modal")
        elif len(t) > 8 and t[0] in ("Button", "var") and statement[1].kind == "id" \
                and t[2:7] == ["=", "UiKit", ".", "Button", "("] and t[-2:] == [")", ";"]:
            local = t[1]
            if local in decls or local in allowed_at:
                _fail(source, VIEW, statement[0].start, f"header button local '{local}' is declared twice (RR-1)")
            decls[local] = button_args(statement[3].start, _split_args(statement[7:-2]), f"header button '{local}'")
            note("button", statement, local)
    for statement in statements:
        t = _texts(statement)
        if len(t) == 6 and t[0] in decls and t[1:4] == [".", "name", "="] and statement[4].kind == "str" \
                and t[5] == ";":
            if t[0] in bindings:
                _fail(source, VIEW, statement[0].start, f"header button '{t[0]}' is named twice (RR-1)")
            bindings[t[0]] = statement[4].value
            note("binding", statement, t[0])
        elif t[:4] == ["buttons", ".", "Add", "("] and t[-2:] == [")", ";"]:
            inner = statement[4:-2]
            if len(inner) == 1 and t[4] in decls:
                adds[t[4]] = adds.get(t[4], 0) + 1
                note("row add", statement, "buttons", t[4])
            elif _texts(inner[:4]) == ["UiKit", ".", "Button", "("] and _texts(inner[-1:]) == [")"]:
                info = button_args(inner[0].start, _split_args(inner[4:-1]), "inline header button")
                if info["classes"]:
                    _fail(source, VIEW, statement[0].start, "an inline header button may not attach classes (RR-1)")
                inline.append(info)
                note("row add", statement, "buttons")
            else:
                _fail(source, VIEW, statement[0].start, "header row addition of a button that is not a reviewed "
                                                           "UiKit.Button declaration (RR-1)")
        elif t[:2] == ["modal", "."] and len(t) == 7 and t[2:4] == ["Add", "("] and statement[4].kind == "id" \
                and t[5:] == [")", ";"]:
            note("modal add", statement, "modal")
    for k, token in enumerate(tokens):
        # The modal may also parent plain labels: UiKit.Add(modal, text, classes...)
        if token.kind == "id" and token.value == "modal" and _texts(tokens[max(0, k - 4):k]) == ["UiKit", ".", "Add", "("] \
                and k + 1 < len(tokens) and tokens[k + 1].text == ",":
            allowed_at["modal"].add(token.start)
    for role in ("header", "row", "attach", "root box", "make modal", "root add", "modal"):
        if counts.get(role, 0) != 1:
            raise ObjectiveCtaSourceError(
                f"Objectives header statement '{role}' must appear exactly once as a straight-line "
                f"statement; found {counts.get(role, 0)} (RR-1/RR-3)")
    # Every use of a header local, the modal and Root must be one of the token positions recorded
    # above: no cast, alias, +=, lambda, if/goto wrapper, indexer, SetEnabled/Clear, hole or restyle.
    for name, positions in allowed_at.items():
        for at in _positions(masked, name, start, end):
            if at not in positions:
                _fail(source, VIEW, at, f"header local '{name}' is used outside the reviewed straight-line "
                                        "header shapes (RR-1/RR-3)")
    known_captions = {caption for caption, *_ in _KNOWN_HEADER_BUTTONS}
    known_names = {name for *_, name in _KNOWN_HEADER_BUTTONS}
    forest = None
    for caption, click, enabled, primary, name in _KNOWN_HEADER_BUTTONS:
        matches = [local for local, info in decls.items() if info["caption"] == caption]
        if len(matches) != 1:
            raise ObjectiveCtaSourceError(
                f"header button '{caption}' must be declared exactly once; found {len(matches)} (RR-1)")
        info = decls[matches[0]]
        classes_ok = (PRIMARY_CLASS in info["classes"]
                      and sum(PRIMARY_CLASS in value for value in info["classes"]) == 1) if primary else not info["classes"]
        if info["click"] != click or info["enabled"] != enabled or not classes_ok \
                or bindings.get(matches[0]) != name or adds.get(matches[0]) != 1:
            _fail(source, VIEW, info["at"],
                  f"header button '{caption}' must be ui.{click}(), "
                  f"{'the one static primary' if primary else 'without classes'}, named '{name}' and added to "
                  "the row once (RR-1, N-1)")
        if primary:
            if name not in allowed:
                _fail(source, VIEW, info["at"], f"static primary '{name}' is not in GuidanceButtonNames (RR-1)")
            forest = info["at"]
    for local, info in decls.items():
        if info["caption"] in known_captions:
            continue
        name = bindings.get(local)
        if any(PRIMARY_CLASS in value for value in info["classes"]) or adds.get(local) != 1 \
                or (name is not None and (name in allowed or name in known_names)):
            _fail(source, VIEW, info["at"],
                  f"extra header button '{local}' must be non-primary, added once and not use a guidance "
                  "name; only Back to forest is statically primary (N-1)")
    for info in inline:
        if info["caption"] in known_captions:
            _fail(source, VIEW, info["at"], "an extra header button reuses a reviewed caption (RR-1)")
    return forest


def _guard_refresh_caller(view_source, view_spans, view_files):
    """The only Objectives caller of RefreshNextActionGuidance is a reachable top-level statement
    of LearningObjectivesView.Refresh(bool), after its one reviewed early return (RR-3)."""
    start, end = _method_span(view_source, _VIEW_REFRESH)
    _no_attribute(view_source, _VIEW_REFRESH, VIEW)
    calls = []
    for path, (source, spans) in view_files.items():
        masked = _precise_mask(source, spans, True)
        calls += [(path, at) for at in _positions(masked, "RefreshNextActionGuidance")]
    if len(calls) != 1 or calls[0][0] != VIEW or not start <= calls[0][1] < end:
        raise ObjectiveCtaSourceError(
            f"RefreshNextActionGuidance must be called exactly once by the view class, from Refresh(bool); "
            f"found {len(calls)} (RR-3 missing/extra caller)")
    texts = [_texts(statement) for statement in _statements(_tokens(view_source, view_spans, start, end))]
    call, guard = ["ui", ".", "RefreshNextActionGuidance", "(", ")", ";"], _tokens_of(_VIEW_REFRESH_GUARD)
    body = _precise_mask(view_source, view_spans, True)[start:end]
    if texts.count(call) != 1 or texts.count(guard) != 1 or texts.index(guard) > texts.index(call) \
            or len(re.findall(r'\breturn\b', body)) != 1 or re.search(r'\b(?:throw|goto|yield)\b', body):
        _fail(view_source, VIEW, calls[0][1],
              "the Objectives RefreshNextActionGuidance call must be an unconditional top-level statement "
              "after the single reviewed early return, with no other return/throw/goto (RR-3)")


# ------------------------------------------------------------- root (RR-1, RR-2, RR-3)

def _guard_root(source, spans):
    masked = _precise_mask(source, spans, True)
    for pattern, what in ((r'(?<!\w)@?Conditional\w*', "a [Conditional] attribute"),
                          (r'\busing\s+static\b', "a using-static directive")):
        found = re.search(pattern, masked)
        if found:
            _fail(source, ROOT, found.start(), f"{what} in ScenarioOneUiRoot is unsupported (RR-3)")
    pinned = {label: _pin(source, spans, ROOT, signature, text, label) for label, signature, text in _ROOT_PINS}
    _no_attribute(source, r'\bprivate\s+void\s+LateUpdate\s*\(\s*\)', ROOT)
    bodies = list(pinned.values())
    snippets = [_snippet_spans(source, spans, snippet, ROOT, "ScenarioOneUiRoot") for snippet in _ROOT_SNIPPETS]
    for name in _ROOT_GUARDED:
        for at in _positions(masked, name):
            if not _inside(at, bodies) and not _inside(at, snippets):
                _fail(source, ROOT, at, f"'{name}' is referenced outside the reviewed methods (qualified, tuple, "
                                        "ref, lambda, shadow or extra use) (RR-2/RR-3)")
    for found in re.finditer(r'(?<!\w)@?[lL]earning\s*\??\.\s*@?Root\b', masked):
        if not _inside(found.start(), bodies) and not _inside(found.start(), snippets):
            _fail(source, ROOT, found.start(), "the Objectives Root is used outside the reviewed root code (RR-1)")
    # The view field may not be reassigned, aliased or passed on (learning = null; var v = learning; Foo(learning)).
    for found in re.finditer(r'(?<!\w)@?[lL]earning(?!\w)(?!\s*\??\.)(?!\s*[!=]=\s*null\b)', masked):
        if not _inside(found.start(), bodies) and not _inside(found.start(), snippets):
            _fail(source, ROOT, found.start(), "the Objectives view field is reassigned, aliased or passed on (RR-1/RR-2)")
    # Fields read by the guidance guards keep their reviewed number of writes (the references once, in
    # Start; screen by its initialiser, SetScreen and TryShowReviewYear); a later reset would silence it.
    for name, expected in (("help", 1), ("manager", 1), ("learning", 1), ("map", 1), ("workPlan", 1),
                           ("review", 1), ("rackLayout", 1), ("screen", 3)):
        if _assignments(masked, name, qualified=True) != expected:
            raise ObjectiveCtaSourceError(
                f"ScenarioOneUiRoot must assign '{name}' exactly {expected} time(s) (RR-2 related state)")
    allowlist = _one(_code(source), r'\bGuidanceButtonNames\s*=\s*\{(.*?)\}\s*;', "GuidanceButtonNames allowlist")
    header_names = {name for *_, name in _KNOWN_HEADER_BUTTONS}
    primaries = 0
    for start, value, interpolated in _string_values(source, spans):
        if PRIMARY_CLASS in value:
            primaries += 1
            if not _inside(start, bodies):
                _fail(source, ROOT, start, "btn-primary literal outside the reviewed guidance methods (RR-1)")
        if any(name in value if interpolated else name == value for name in header_names) \
                and not _inside(start, bodies + [allowlist.span()]):
            _fail(source, ROOT, start, "an Objectives header name is used outside the allowlist and the "
                                       "reviewed routing (RR-1)")
    if primaries != 5:
        raise ObjectiveCtaSourceError(f"ScenarioOneUiRoot must contain the 5 reviewed btn-primary literals; found {primaries}")
    commented = _precise_mask(source, spans, False)
    for found in re.finditer(r'\w*ClassList\w*', masked):
        if _inside(found.start(), bodies):
            continue
        call = re.match(r'\s*\(', masked[found.end():])
        if not call or not masked[:found.start()].rstrip().endswith("."):
            _fail(source, ROOT, found.start(), "class-list API used other than as a direct call (RR-1)")
        args, _ = _args(masked, found.end() + call.end() - 1)
        value = _sole_literal(commented[args[0][0]:args[0][1]])
        if value is None or PRIMARY_CLASS in value:
            _fail(source, ROOT, found.start(), "class-list call outside the reviewed methods must name a plain, "
                                               "non-primary class literal (RR-1)")
    _check_uikit_calls(ROOT, source, spans, masked, skip=bodies)
    _guard_related_state(source, spans, masked, bodies)
    _snippet_spans(source, spans, _ROUTING_INPUT_ROOT, ROOT, "RR-3 routing input 'NeedsAnnualReview'")


def _bracket_map(texts):
    """Matching index of every bracket token, and the innermost open bracket enclosing each token."""
    match, parent, stack = {}, [], []
    for k, text in enumerate(texts):
        parent.append(stack[-1] if stack else None)
        if text in ("(", "[", "{"):
            stack.append(k)
        elif text in (")", "]", "}") and stack:
            opened = stack.pop()
            match[opened], match[k] = k, opened
    return match, parent


def _state_write(tokens, texts, match, parent, k):
    """How token k, a related-state name, is written; None for a read (#314 S-1). Written means: the
    target of any assignment operator, ++ or --, a ref/out/in argument, or an element of a parenthesised,
    tuple or foreach-deconstruction target. A shadowing declaration (`var help = ...`) also counts."""
    start = end = k
    if k >= 2 and texts[k - 1] == "." and texts[k - 2] == "this":
        start = k - 2
    while True:
        after = end + 1
        while after < len(texts) and texts[after] == "!":
            after += 1
        nxt = texts[after] if after < len(texts) else ""
        if nxt in _ASSIGNMENT_OPERATORS or nxt in ("++", "--") or (
                nxt == ">" and after + 1 < len(texts) and texts[after + 1] == ">="
                and tokens[after + 1].start == tokens[after].end):  # `>>=`
            return "assignment"
        before = texts[start - 1] if start > 0 else ""
        if before in ("ref", "out", "++", "--") or (before == "in" and start > 1 and texts[start - 2] in ("(", ",", ":")):
            return f"{before} operand"
        if before == "(" and match.get(start - 1) == after:  # `(help) = x`, `ref (help)`
            start, end = start - 1, after
            continue
        break
    if nxt in (",", ")"):
        opened = parent[start]
        while opened is not None and texts[opened] == "(":
            close = match.get(opened)
            follow = texts[close + 1] if close is not None and close + 1 < len(texts) else ""
            if follow in _ASSIGNMENT_OPERATORS or follow == "in":
                return "tuple or deconstruction target"
            opened = parent[opened]
    return None


def _guard_related_state(source, spans, masked, bodies):
    """#314 S-1: write-site discipline for the root fields the pinned guidance reads."""
    found = re.search(_ROOT_UNSAFE[0], masked)
    if found:
        _fail(source, ROOT, found.start(), f"{_ROOT_UNSAFE[1]} in ScenarioOneUiRoot is unsupported "
                                           "(RR-2 related state)")
    for snippet in _STATE_DECLARATIONS:
        _snippet_spans(source, spans, snippet, ROOT, "RR-2 related state declaration")
    sites = [_snippet_spans(source, spans, _STATE_DECLARATIONS[-1], ROOT, "RR-2 related state declaration")]
    start, end = _method_span(source, _START)
    for snippet in _STATE_START_WRITES:
        a, b = _snippet_spans(source, spans, snippet, ROOT, f"RR-2 related state '{snippet.split()[0]}'")
        before = masked[start:a]
        if not (start <= a and b <= end) or before.count("{") != before.count("}") \
                or before.count("(") != before.count(")") or masked[:a].rstrip()[-1:] not in (";", "{", "}"):
            _fail(source, ROOT, a, f"RR-2 related state: `{snippet}` must be a top-level statement of Start "
                                   "(not moved, wrapped, conditional or deferred)")
        sites.append((a, b))
    last = max(b for _, b in sites[1:])
    early = re.search(r'\b(?:return|throw|goto|yield)\b', masked[start:last])
    if early:
        _fail(source, ROOT, start + early.start(), "RR-2 related state: Start may not return, throw, goto or "
                                                   "yield before its last reviewed related-state write")
    sites += bodies + [_pin(source, spans, ROOT, _TRY_SHOW_REVIEW_YEAR[0], _TRY_SHOW_REVIEW_YEAR[1],
                            "TryShowReviewYear (RR-2 related state 'screen')")]
    tokens, texts = _file_tokens(source, spans)
    match, parent = _bracket_map(texts)
    for k, token in enumerate(tokens):
        if token.kind != "id" or token.value not in _STATE_FIELDS:
            continue
        name = token.value
        if k and texts[k - 1] in (".", "?.", "->", "::") and not (texts[k - 1] == "." and k > 1 and texts[k - 2] == "this"):
            _fail(source, ROOT, token.start, f"'{name}' is reached through a receiver other than `this` "
                                             "(RR-2 related state)")
        how = _state_write(tokens, texts, match, parent, k)
        if how and not _inside(token.start, sites):
            _fail(source, ROOT, token.start, f"'{name}' is written outside its reviewed write site ({how}): "
                                             "tuple, ref/out, qualified, compound, moved or compensated writes "
                                             "fail closed (RR-2 related state)")
        if name == "screen":
            # A value type: a `ref this`/`in this` extension or an `in` parameter could write it unseen.
            first = k - 2 if k >= 2 and texts[k - 1] == "." and texts[k - 2] == "this" else k
            opened = parent[first]
            call = opened is not None and (texts[opened] == "[" or (texts[opened] == "(" and opened > 0 and (
                texts[opened - 1] in (">", ")", "]")
                or (tokens[opened - 1].kind == "id" and texts[opened - 1] not in _NOT_CALLEES))))
            if (k + 1 < len(texts) and texts[k + 1] in (".", "?.")) or (
                    call and first > 0 and texts[first - 1] in ("(", "[", ",") and k + 1 < len(texts) and texts[k + 1] in (",", ")", "]")):
                _fail(source, ROOT, token.start, "'screen' is used as a member receiver or passed whole as an "
                                                 "argument; a ref/in extension or parameter could write it "
                                                 "(RR-2 related state)")


# ------------------------------------------------------------- UiKit (N-3)

def _guard_uikit(source, spans):
    masked = _precise_mask(source, spans, True)
    for start, value, _ in _string_values(source, spans):
        if PRIMARY_CLASS in value:
            _fail(source, UIKIT, start, "UiKit must not attach btn-primary itself (RR-1)")
    # Every UiKit member the Objectives constructor calls is pinned below; an overload could replace one.
    builders = re.findall(r'\bstatic\b[^;{}=()]*?\b(Button|Box|Row|Text|Add|MakeModal)\s*[(<]', masked)
    if sorted(builders) != ["Add", "Box", "Button", "MakeModal", "Row", "Text"]:
        raise ObjectiveCtaSourceError(
            f"UiKit must declare exactly one Button, Box, Row, Text, Add and MakeModal; found {sorted(builders)} (N-3)")
    for label, signature, text in _KIT_PINS:
        _pin(source, spans, UIKIT, signature, text, label)
