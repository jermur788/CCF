#!/usr/bin/env python3
"""Validates lab files against schemas/lab-file-v1.schema.json with a small built-in validator
(no pip packages). It implements only the JSON-Schema keywords that file uses:
$ref, type, const, enum, pattern, required, properties, additionalProperties, items, minItems, maxItems,
minimum, maximum, oneOf.

    python3 -I -B tests/check-schema.py <file.sitkalab.json> [...]
"""
import json, re, sys, os

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
SCHEMA = json.load(open(os.path.join(ROOT, "schemas", "lab-file-v1.schema.json"), encoding="utf-8"))

def resolve(ref):
    node = SCHEMA
    for part in ref.lstrip("#/").split("/"):
        node = node[part]
    return node

def type_ok(v, t):
    if t == "null": return v is None
    if t == "integer": return isinstance(v, int) and not isinstance(v, bool)
    if t == "string": return isinstance(v, str)
    if t == "object": return isinstance(v, dict)
    if t == "array": return isinstance(v, list)
    raise ValueError(t)

def validate(v, s, path="$"):
    errs = []
    if "$ref" in s:
        return validate(v, resolve(s["$ref"]), path)
    if "const" in s and v != s["const"] or ("const" in s and type(v) != type(s["const"])):
        errs.append(f"{path}: expected const {s['const']!r}, got {v!r}")
    if "enum" in s and v not in s["enum"]: errs.append(f"{path}: {v!r} not in enum")
    if "type" in s:
        ts = s["type"] if isinstance(s["type"], list) else [s["type"]]
        if not any(type_ok(v, t) for t in ts): errs.append(f"{path}: type {type(v).__name__} not in {ts}"); return errs
    if "oneOf" in s:
        ok = [not validate(v, sub, path) for sub in s["oneOf"]]
        if sum(ok) != 1: errs.append(f"{path}: matches {sum(ok)} of oneOf")
    if isinstance(v, str) and "pattern" in s and not re.search(s["pattern"], v): errs.append(f"{path}: {v!r} !~ {s['pattern']}")
    if isinstance(v, int) and not isinstance(v, bool):
        if "minimum" in s and v < s["minimum"]: errs.append(f"{path}: {v} < {s['minimum']}")
        if "maximum" in s and v > s["maximum"]: errs.append(f"{path}: {v} > {s['maximum']}")
    if isinstance(v, list):
        if "minItems" in s and len(v) < s["minItems"]: errs.append(f"{path}: fewer than {s['minItems']} items")
        if "maxItems" in s and len(v) > s["maxItems"]: errs.append(f"{path}: more than {s['maxItems']} items")
        if "items" in s:
            for i, x in enumerate(v): errs += validate(x, s["items"], f"{path}[{i}]")
    if isinstance(v, dict):
        for r in s.get("required", []):
            if r not in v: errs.append(f"{path}: missing required '{r}'")
        props = s.get("properties", {})
        for k, x in v.items():
            if k in props: errs += validate(x, props[k], f"{path}.{k}")
            elif s.get("additionalProperties") is False: errs.append(f"{path}: unexpected property '{k}'")
            elif isinstance(s.get("additionalProperties"), dict): errs += validate(x, s["additionalProperties"], f"{path}.{k}")
    return errs

def main():
    bad = 0
    for f in sys.argv[1:]:
        errs = validate(json.load(open(f, encoding="utf-8")), SCHEMA)
        print(("OK   " if not errs else "FAIL ") + f)
        for e in errs[:10]: print("     " + e)
        bad += bool(errs)
    # negative controls: the validator must reject deliberately broken documents
    good = json.load(open(sys.argv[1], encoding="utf-8")) if len(sys.argv) > 1 else None
    if good:
        import copy
        for name, mut in [("wrong format", lambda d: d.__setitem__("format", "x")), ("extra root key", lambda d: d.__setitem__("extra", 1)),
                          ("fractional dbh", lambda d: d["snapshot"]["trees"][0].__setitem__("dbhMm", 1.5)), ("bad id", lambda d: d["snapshot"]["trees"][0].__setitem__("treeId", "a b")),
                          ("sloped site", lambda d: d["snapshot"]["siteProfile"].__setitem__("slopeDeg", 3)), ("other species", lambda d: d["snapshot"]["trees"][0].__setitem__("species", "Oak"))]:
            d = copy.deepcopy(good); mut(d)
            rejected = bool(validate(d, SCHEMA))
            print(("OK   " if rejected else "FAIL ") + f"negative control rejected: {name}")
            bad += (not rejected)
    return 1 if bad else 0

if __name__ == "__main__":
    sys.exit(main())
