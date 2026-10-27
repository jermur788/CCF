#!/usr/bin/env python3
"""Shared source-normalization for verification source-integrity guards.

A verification guard that hashes a source file by raw bytes fails on comments and non-semantic
whitespace, which can suppress the useful checks behind it. This module normalizes a source file so
that comments and non-semantic whitespace are ignored while a meaningful code/calibration change is
still detected.

C# is normalized by removing comments and collapsing whitespace OUTSIDE string/char literals
(literal text is preserved verbatim, including interpolated-string expressions). Non-C# protected
files (Unity YAML) only normalize line endings, trailing whitespace and blank lines.

Used by:
  Tools/Calibration/OakBeechGrowth/check_integrated_v1.py  (F3 integrated-calibration guard)
  Tools/Verification/ScenarioOneCompletion/N3/run_n3_offline.py  (N3 completion-rules guard)

Positive/negative tests: Tools/Calibration/OakBeechGrowth/test_source_normalization.py
"""
import hashlib
import re
from pathlib import Path


def _read_char(text, i):
    frag = []; n = len(text)
    while i < n:
        c = text[i]
        if c == '\\' and i + 1 < n:
            frag.append(c); frag.append(text[i + 1]); i += 2; continue
        frag.append(c); i += 1
        if c == "'": break
    return ''.join(frag), i


def _read_string(text, i, verbatim, interpolated):
    """i points just after the opening quote. Returns (fragment_including_closing_quote, next_index)."""
    frag = []; n = len(text)
    while i < n:
        c = text[i]
        if verbatim:
            if c == '"':
                if i + 1 < n and text[i + 1] == '"':
                    frag.append('""'); i += 2; continue
                return ''.join(frag) + '"', i + 1
            if interpolated and c == '{':
                if i + 1 < n and text[i + 1] == '{':
                    frag.append('{{'); i += 2; continue
                expr, i = _norm_code(text, i + 1, stop_on_brace=True)
                frag.append('{' + ''.join(expr) + '}'); i += 1; continue
            if interpolated and c == '}':
                if i + 1 < n and text[i + 1] == '}':
                    frag.append('}}'); i += 2; continue
            frag.append(c); i += 1
        else:
            if c == '\\' and i + 1 < n:
                frag.append(c); frag.append(text[i + 1]); i += 2; continue
            if c == '"':
                return ''.join(frag) + '"', i + 1
            if interpolated and c == '{':
                if i + 1 < n and text[i + 1] == '{':
                    frag.append('{{'); i += 2; continue
                expr, i = _norm_code(text, i + 1, stop_on_brace=True)
                frag.append('{' + ''.join(expr) + '}'); i += 1; continue
            if interpolated and c == '}':
                if i + 1 < n and text[i + 1] == '}':
                    frag.append('}}'); i += 2; continue
            frag.append(c); i += 1
    return ''.join(frag), i


def _norm_code(text, i, stop_on_brace):
    out = []; n = len(text); pending = False; depth = 0

    def flush():
        nonlocal pending
        if pending and out: out.append(' ')
        pending = False

    while i < n:
        c = text[i]
        if stop_on_brace and c == '}' and depth == 0: return out, i
        if c in ' \t\r\n\f\v':
            pending = True; i += 1; continue
        if c == '/' and i + 1 < n and text[i + 1] == '/':
            pending = True; i += 2
            while i < n and text[i] not in '\r\n': i += 1
            continue
        if c == '/' and i + 1 < n and text[i + 1] == '*':
            pending = True; i += 2
            while i + 1 < n and not (text[i] == '*' and text[i + 1] == '/'): i += 1
            i = min(i + 2, n); continue
        flush()
        if c == '$' and i + 1 < n and text[i + 1] == '"':
            frag, i = _read_string(text, i + 2, False, True); out.append('$"' + frag); continue
        if c == '@' and i + 1 < n and text[i + 1] == '"':
            frag, i = _read_string(text, i + 2, True, False); out.append('@"' + frag); continue
        if c == '$' and i + 2 < n and text[i + 1] == '@' and text[i + 2] == '"':
            frag, i = _read_string(text, i + 3, True, True); out.append('$@"' + frag); continue
        if c == '@' and i + 2 < n and text[i + 1] == '$' and text[i + 2] == '"':
            frag, i = _read_string(text, i + 3, True, True); out.append('@$"' + frag); continue
        if c == '"':
            frag, i = _read_string(text, i + 1, False, False); out.append('"' + frag); continue
        if c == "'":
            frag, i = _read_char(text, i + 1); out.append("'" + frag); continue
        if c == '{': depth += 1; out.append('{'); i += 1; continue
        if c == '}': depth -= 1; out.append('}'); i += 1; continue
        out.append(c); i += 1
    return out, i


def normalize_csharp(text):
    out, _ = _norm_code(text, 0, stop_on_brace=False)
    return ''.join(out).strip()


def normalize_other(text):
    text = text.replace('\r\n', '\n').replace('\r', '\n')
    lines = [line.rstrip() for line in text.split('\n')]
    return re.sub(r'\n\s*\n+', '\n', '\n'.join(lines)).strip()


def normalized_text(path):
    text = Path(path).read_text(encoding='utf-8')
    return normalize_csharp(text) if Path(path).suffix == '.cs' else normalize_other(text)


def normalized_sha256(path):
    return hashlib.sha256(normalized_text(path).encode('utf-8')).hexdigest()
