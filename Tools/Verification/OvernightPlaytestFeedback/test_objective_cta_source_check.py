#!/usr/bin/env python3
"""Negative controls for the Objective CTA source scanner; no Unity or .NET.

Runs against source in the current checkout, then mutates in-memory fixture
copies only. A claimed success requires all deliberately broken cases to be
rejected. The original numbered historical checks live in check_baseline.py.
"""
from pathlib import Path
from objective_cta_source_check import (
    ROOT, VIEW, ObjectiveCtaSourceError, verify_objective_cta
)

REPO = Path(__file__).resolve().parents[3]


def replace_one(source, old, new, label):
    count = source.count(old)
    if count != 1:
        raise RuntimeError(f"{label}: expected exactly one live source fixture anchor, got {count}: {old!r}")
    return source.replace(old, new, 1)


def run():
    view = (REPO / VIEW).read_text(encoding="utf-8")
    root = (REPO / ROOT).read_text(encoding="utf-8")
    cases = [
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
    baseline = verify_objective_cta(view, root)
    print("SOURCE-STATIC PASS objective-cta-live-source: "
          f"{len(baseline)} anchored evidence rows (UI runtime NOT TESTED)")
    failed = 0
    for label, which, old, new in cases:
        changed = replace_one(view if which == "view" else root, old, new, label)
        test_view = changed if which == "view" else view
        test_root = changed if which == "root" else root
        try:
            verify_objective_cta(test_view, test_root)
        except ObjectiveCtaSourceError as exc:
            print(f"SOURCE-STATIC PASS mutation-{label}: rejected ({exc})")
        else:
            failed += 1
            print(f"SOURCE-STATIC FAIL mutation-{label}: FALSE GREEN")
    print(f"SOURCE-STATIC CTA MUTATIONS rejected={len(cases)-failed}/{len(cases)} "
          f"false_greens={failed}")
    return 1 if failed else 0


if __name__ == "__main__":
    raise SystemExit(run())
