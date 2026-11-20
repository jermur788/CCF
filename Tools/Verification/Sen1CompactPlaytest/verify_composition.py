#!/usr/bin/env python3
"""Read-only, exact-input composition checks; no Unity or save access."""
import hashlib
import json
from pathlib import Path
import re
import subprocess

ROOT = Path(__file__).resolve().parents[3]
BASE = "8cb22de077b93b4c7af076e19936f3170d6ab48b"
COMMON = "0709d05a79756d3b72ccbd0452bd0252a75c3804"
VISUAL = "5cbc26a4073ffa60db0a3c442e237eaa473ad3ff"
SCENE = "Assets/ForestPrototype/Scenes/ScenarioOneCompact30.unity"


def git(*args):
    return subprocess.check_output(["git", "-C", str(ROOT), *args])


def main():
    visual = git("diff", "--name-only", COMMON, VISUAL, "--", "Assets", "Packages", "ProjectSettings").decode().splitlines()
    gameplay = set(git("diff", "--name-only", COMMON, BASE, "--", "Assets", "Packages", "ProjectSettings").decode().splitlines())
    assert len(visual) == 74 and not gameplay.intersection(visual), "input overlap"
    for path in visual:
        assert (ROOT / path).read_bytes() == git("show", VISUAL + ":" + path), path
    extra = {"Assets/Editor.meta", "Assets/Editor/Sen1CompactPlaytestEntry.cs", "Assets/Editor/Sen1CompactPlaytestEntry.cs.meta"}
    changed = set(git("diff", BASE, "--name-only").decode().splitlines())
    changed.update(git("ls-files", "--others", "--exclude-standard").decode().splitlines())
    assert all(p in visual or p in extra or p.startswith("Tools/Verification/Sen1CompactPlaytest/")
               or p == "Docs/Handoffs/ScenarioOne-Player-Test-Readiness-20261120/Sen-1-873-NATIVE-IMPLEMENTATION-RESULT-20261120.md"
               for p in changed), "unexpected changed path: " + repr(changed - set(visual) - extra)
    # Verify ALL tracked base files outside the approved delta, including gameplay, packages,
    # historical scenes and frozen archives, rather than selecting a few reassuring examples.
    protected = 0
    inherited_refs = set()
    for line in git("ls-tree", "-r", BASE).decode().splitlines():
        metadata, path = line.split("\t", 1)
        if path in visual:
            continue
        expected = metadata.split()[2]
        data = (ROOT / path).read_bytes()
        actual = hashlib.sha1(b"blob " + str(len(data)).encode() + b"\0" + data).hexdigest()
        assert actual == expected, "protected base file changed: " + path
        if path.endswith((".unity", ".prefab", ".mat", ".asset")):
            inherited_refs.update(re.findall(rb"guid: ([0-9a-f]{32})", data))
        protected += 1
    scene = (ROOT / SCENE).read_text()
    ids = re.findall(r"--- !u!\d+ &(-?\d+)", scene)
    assert len(ids) == len(set(ids)), "duplicate serialized object IDs"
    roster_path = ROOT / "Assets/ForestPrototype/ScenarioOne/Compact30/Resources/Compact30Roster.json"
    roster = json.loads(roster_path.read_text())
    names = re.findall(r"propertyPath: m_Name\s+value: (P\d+)_", scene)
    assert len(names) == 175 and set(names) == {row["id"] for row in roster["rows"]}, "authored tree identity mismatch"
    assert scene.count("\nBoxCollider:") == 5 and scene.count("\nCharacterController:") == 1, "scene collider inventory changed"
    for name in ["Compact30World", "Trees", "Sen1_Perimeter_DeerFence", "Sen1_SpruceFloorScatter"]:
        assert "m_Name: " + name in scene, "missing root: " + name
    # Every newly referenced GUID must already resolve to a candidate asset or an unchanged
    # package/built-in reference from the base scene. Unity checks component types at import.
    base_refs = {value.decode() for value in inherited_refs}
    known = set()
    for meta in (ROOT / "Assets").rglob("*.meta"):
        match = re.search(r"^guid: ([0-9a-f]{32})$", meta.read_text(), re.M)
        if match:
            assert match[1] not in known, "duplicate asset GUID: " + str(meta)
            known.add(match[1])
    for path in visual:
        if path.endswith((".unity", ".prefab", ".mat", ".asset")):
            for guid in re.findall(r"guid: ([0-9a-f]{32})", (ROOT / path).read_text()):
                assert guid in known or guid in base_refs or guid.startswith("0000000000000000"), "unresolved GUID: " + path + " " + guid
    print(json.dumps({"result": "PASS", "visual_paths_byte_exact": len(visual),
                      "protected_base_files_byte_exact": protected, "authored_trees": len(names),
                      "scene_colliders": 6, "new_guid_references_resolve": True}, indent=2))


if __name__ == "__main__":
    main()
