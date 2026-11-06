#!/usr/bin/env python3
"""Negative controls for the Objective CTA source scanner; no Unity or .NET.

Runs against source in the current checkout, then mutates in-memory fixture
copies only. A claimed success requires all deliberately broken cases to be
rejected, every benign edit accepted and the historical baseline rejected. The original numbered historical checks live in check_baseline.py.
"""
import subprocess
from pathlib import Path
from objective_cta_source_check import (
    ROOT, VIEW, ObjectiveCtaSourceError, verify_objective_cta
)

REPO = Path(__file__).resolve().parents[3]
BASE = "72b4efbd1c653fa2231f632a79030a83ff76f833"  # historical pre-guidance source; must stay rejected


def replace_one(source, old, new, label):
    count = source.count(old)
    if count != 1:
        raise RuntimeError(f"{label}: expected exactly one live source fixture anchor, got {count}: {old!r}")
    return source.replace(old, new, 1)


def run():
    view = (REPO / VIEW).read_text(encoding="utf-8")
    root = (REPO / ROOT).read_text(encoding="utf-8")
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

    baseline = verify_objective_cta(view, root)
    print("SOURCE-STATIC PASS objective-cta-live-source: "
          f"{len(baseline)} anchored evidence rows (UI runtime NOT TESTED)")

    def apply(label, which, old, new):
        changed = replace_one(view if which == "view" else root, old, new, label)
        return (changed if which == "view" else view), (changed if which == "root" else root)

    groups = [("original", original_cases), ("F1", f1_cases), ("F2-F3", f2_cases)]
    total = rejected = 0
    for group, cases in groups:
        group_rejected = 0
        for label, which, old, new in cases:
            try:
                verify_objective_cta(*apply(label, which, old, new))
            except ObjectiveCtaSourceError as exc:
                group_rejected += 1
                print(f"SOURCE-STATIC PASS mutation-{label}: rejected ({exc})")
            else:
                print(f"SOURCE-STATIC FAIL mutation-{label}: FALSE GREEN")
        print(f"SOURCE-STATIC CTA MUTATION GROUP {group}: rejected={group_rejected}/{len(cases)}")
        total += len(cases)
        rejected += group_rejected

    benign_failed = 0
    for label, which, old, new in benign_cases:
        try:
            verify_objective_cta(*apply(label, which, old, new))
        except ObjectiveCtaSourceError as exc:
            benign_failed += 1
            print(f"SOURCE-STATIC FAIL benign-{label}: WRONGLY REJECTED ({exc})")
        else:
            print(f"SOURCE-STATIC PASS benign-{label}: accepted")
    print(f"SOURCE-STATIC CTA BENIGN CONTROLS accepted={len(benign_cases)-benign_failed}/{len(benign_cases)}")

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

    false_greens = total - rejected
    print(f"SOURCE-STATIC CTA MUTATIONS rejected={rejected}/{total} false_greens={false_greens}")
    return 1 if false_greens or benign_failed or historic_failed else 0


if __name__ == "__main__":
    raise SystemExit(run())
