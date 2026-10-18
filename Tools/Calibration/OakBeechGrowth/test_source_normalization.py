#!/usr/bin/env python3
"""Positive/negative tests for the comment/whitespace-insensitive source guard used by check_integrated_v1.py.

Positive: comment-only and whitespace-only variation must NOT change the normalized hash.
Negative: a meaningful code, constant, string-literal or interpolated-expression change MUST change it.
Also asserts normalizer idempotence and a stable normalized hash for every protected file.
"""
import importlib.util, sys, tempfile
from pathlib import Path
sys.dont_write_bytecode = True
HERE = Path(__file__).resolve().parent
spec = importlib.util.spec_from_file_location("civ", HERE / "check_integrated_v1.py")
guard = importlib.util.module_from_spec(spec); spec.loader.exec_module(guard)

failures = []
def check(condition, message):
    print(("PASS " if condition else "FAIL ") + message)
    if not condition: failures.append(message)

# --- synthetic C# semantics -------------------------------------------------------------------
base = 'class X { void M(int a,int b){ string s = "a  b"; var t = $"v={a + b}"; } }'
check(guard.normalize_csharp(guard.normalize_csharp(base)) == guard.normalize_csharp(base), "normalizer idempotent")
check(guard.normalize_csharp(base.replace("a  b", "a b")) != guard.normalize_csharp(base), "string-literal whitespace change detected")
check(guard.normalize_csharp(base.replace("a + b", "a - b")) != guard.normalize_csharp(base), "interpolated-expression change detected")
check(guard.normalize_csharp(base.replace("a + b", "a  +  b")) == guard.normalize_csharp(base), "interpolated-expression whitespace ignored")
check(guard.normalize_csharp("int x=1; // c\nint y=2;") == guard.normalize_csharp("int x=1; int y=2;"), "line comment ignored")
check(guard.normalize_csharp("int x=1; /* c */ int y=2;") == guard.normalize_csharp("int x=1; int y=2;"), "block comment ignored")
check(guard.normalize_csharp('var u = "http://x//y";') == guard.normalize_csharp('var u = "http://x//y";'), "comment marker inside string preserved")
check(guard.normalize_csharp('var v = @"a//b";') == guard.normalize_csharp('var v = @"a//b";'), "verbatim string comment marker preserved")

# --- real protected files ---------------------------------------------------------------------
import json
for f in json.loads((HERE / "integrated_source_contract.json").read_text()):
    path = guard.ROOT / f
    check(guard.normalized_sha256(path) == guard.normalized_sha256(path), "stable normalized hash " + f)

target = guard.ROOT / "Assets/ForestPrototype/SitkaGrowthModel.cs"
with tempfile.TemporaryDirectory() as tmp:
    tmp = Path(tmp); original = target.read_text()
    (tmp / "c.cs").write_text("// new leading comment\n" + original + "\n   \n// trailing comment\n")
    check(guard.normalized_sha256(tmp / "c.cs") == guard.normalized_sha256(target), "comment-only edit tolerated (SitkaGrowthModel.cs)")
    (tmp / "w.cs").write_text("\n\n   \n" + original + "\n\t\n  \n")
    check(guard.normalized_sha256(tmp / "w.cs") == guard.normalized_sha256(target), "whitespace-only edit tolerated (SitkaGrowthModel.cs)")
    (tmp / "m.cs").write_text(original.replace("public const int Legacy = 0;", "public const int Legacy = 9;"))
    check(guard.normalized_sha256(tmp / "m.cs") != guard.normalized_sha256(target), "semantic constant change detected (SitkaGrowthModel.cs)")
    (tmp / "m2.cs").write_text(original.replace("public static class GrowthModel", "public sealed class GrowthModel"))
    check(guard.normalized_sha256(tmp / "m2.cs") != guard.normalized_sha256(target), "code token change detected (SitkaGrowthModel.cs)")

# N3 protected file: ScenarioOneCompletionRules.cs (guarded by N3/run_n3_offline.py)
n3 = guard.ROOT / "Assets/ForestPrototype/ScenarioOne/ScenarioOneCompletionRules.cs"
with tempfile.TemporaryDirectory() as tmp:
    tmp = Path(tmp); n3_text = n3.read_text()
    (tmp / "n3c.cs").write_text("// added comment\n" + n3_text + "\n\n// trailing\n")
    check(guard.normalized_sha256(tmp / "n3c.cs") == guard.normalized_sha256(n3), "N3 comment-only edit tolerated (ScenarioOneCompletionRules.cs)")
    (tmp / "n3w.cs").write_text("\n\n   \n" + n3_text + "\n\t\n")
    check(guard.normalized_sha256(tmp / "n3w.cs") == guard.normalized_sha256(n3), "N3 whitespace-only edit tolerated (ScenarioOneCompletionRules.cs)")
    (tmp / "n3m.cs").write_text(n3_text.replace("if (plans.Count < 2) return false;", "if (plans.Count < 3) return false;"))
    check(guard.normalized_sha256(tmp / "n3m.cs") != guard.normalized_sha256(n3), "N3 semantic mutation detected (ScenarioOneCompletionRules.cs)")

print("SOURCE_NORMALIZATION_FAIL" if failures else "SOURCE_NORMALIZATION_PASS")
raise SystemExit(1 if failures else 0)
