#!/usr/bin/env python3
"""Bounded source-structure guard for the Objective 1 map CTA.

This is intentionally not a Unity/UI behaviour test and not a C# parser. It checks
the real constructor and guidance methods with lexical rules that fail closed:
an ambiguous or unsupported source shape raises ObjectiveCtaSourceError rather
than being accepted. Comments and string literals are never treated as code.

Copy this file, check_baseline.py and test_objective_cta_source_check.py
together; check_baseline.py imports this module.
"""
import re

VIEW = "Assets/ForestPrototype/UI/LearningObjectivesView.cs"
ROOT = "Assets/ForestPrototype/UI/ScenarioOneUiRoot.cs"

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
_COND = re.compile(r'^[ \t]*#[ \t]*(if|elif|else|endif|define|undef)\b', re.M)
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


def _assignments(masked, name):
    """Count writes to a simple local/field: `x = `, `x += `, `x++`, `out x`, `ref x`."""
    ident = re.escape(name)
    write = re.compile(r'(?<![.\w])' + ident + r'\s*(?:(?:[-+*/%|&^]|<<|>>|\?\?)?=(?![=>])|\+\+|--)')
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
                f"{label}: conditional compilation (#{match.group(1)}) in a checked method is unsupported")
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
    match = _one(root_code, r'\bGuidanceButtonNames\s*=\s*\{(.*?)\}\s*;', "T2-owned allowlist")
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
    uses = _count(_code(root_code, mask_literals=True), r'(?<![.\w])GuidanceButtonNames\b')
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
        if own[0] == CTA_NAME or own[0] not in allowed:
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

def verify_objective_cta(view_source, root_source):
    """Raise ObjectiveCtaSourceError on a missing/ambiguous CTA, otherwise return evidence rows.

    Accept source *text* so mutation tests can operate solely on temporary
    in-memory copies. No production files are modified by this function.
    """
    try:
        return _verify(view_source, root_source)
    except ObjectiveCtaSourceError as error:
        if not error.rows:
            error.rows = _diagnostic_rows(view_source, root_source)
        raise
    except Exception as error:  # noqa: BLE001 - an unexpected scanner failure is a FAIL, not a crash or a pass
        raise ObjectiveCtaSourceError(
            f"scanner could not analyse the source ({type(error).__name__}: {error})",
            rows=_diagnostic_rows(view_source, root_source)) from error


def _verify(view_source, root_source):
    view_code = _code(view_source)
    view = _method(view_source, _CTOR_SIGNATURE)
    root = _code(root_source)
    try:
        allowed, allowlist_error = _allowlist_values(root), None
    except ObjectiveCtaSourceError as error:
        allowed, allowlist_error = set(), error
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
        if _assignments(root_masked, name) != expected:
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
    if _assignments(_code(root, mask_literals=True), "helpClosedFrame") != 2:
        raise ObjectiveCtaSourceError("helpClosedFrame must be written only by its initialiser and CloseHelp")

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
    ]
