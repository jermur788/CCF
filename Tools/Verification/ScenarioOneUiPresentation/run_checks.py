#!/usr/bin/env python3
"""Compile/execute exact presentation method slices against a plain element sink.
This is NOT Unity layout, native-input, persistence or render evidence. No Assets writes.
Run: python3 -B run_checks.py --src /absolute/source --out /durable/evidence
"""
import argparse, hashlib, json, pathlib, subprocess, sys
sys.dont_write_bytecode = True

def member(text, signature):
    assert text.count(signature) == 1, signature
    start = text.index(signature)
    brace, arrow = text.find('{', start), text.find('=>', start)
    if 0 <= arrow < brace:
        return text[start:text.index(';', arrow)+1]
    depth = 0
    for end in range(brace, len(text)):
        if text[end] == '{': depth += 1
        elif text[end] == '}':
            depth -= 1
            if depth == 0: return text[start:end+1]
    raise AssertionError('Unclosed member '+signature)

def generate(root):
    ui = root/'Assets/ForestPrototype/UI'
    kit = (ui/'UiKit.cs').read_text()
    tree = (ui/'TreeInspectionView.cs').read_text()
    annual = (ui/'AnnualReviewView.cs').read_text()
    code = (pathlib.Path(__file__).parent/'PresentationChecks.cs').read_text()
    for key, value in {
        'NAV': member(kit, 'public static VisualElement MenuNavigation('),
        'KEYS': member(kit, 'public static void KeyPrompts('),
        'PAGE': member(tree, 'public static int NextPage('),
        'RANKS': member(tree, 'public static string NeighbourHint('),
        'HEADLINES': member(annual, 'private void BuildHeadlines(').replace('private void', 'public void', 1),
        'COLUMN': member(annual, 'private static VisualElement Column('),
    }.items(): code = code.replace('/*'+key+'*/', value)
    return code

def main():
    ap=argparse.ArgumentParser(description=__doc__)
    ap.add_argument('--src',type=pathlib.Path,required=True)
    ap.add_argument('--out',type=pathlib.Path,required=True)
    a=ap.parse_args(); root=a.src.resolve(); out=a.out.resolve(); out.mkdir(parents=True,exist_ok=True)
    source=generate(root)
    def run(name, code):
        dest=out/name; dest.mkdir(exist_ok=True)
        (dest/'Probe.cs').write_text(code)
        (dest/'probe.csproj').write_text('<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net8.0</TargetFramework><ImplicitUsings>disable</ImplicitUsings><NoWarn>CS0649</NoWarn></PropertyGroup></Project>')
        r=subprocess.run(['dotnet','run','--project',str(dest/'probe.csproj'),'-v','q'],capture_output=True,text=True)
        (dest/'result.log').write_text(r.stdout+r.stderr)
        return r
    result=run('positive',source); print(result.stdout.strip())
    if result.returncode: print(result.stderr); return 1
    mutants=[
        ('rack-first-keys-leak','if (suppressMarking && (key == "X" || key == "C")) continue;','if (false) continue;','RACK_FIRST_SUPPRESSES_BOTH_MARKS'),
        ('duplicate-prompts','if (!seen.Add(key)) continue;','seen.Add(key);','CONTEXT_KEYS_ARE_UNIQUE'),
        ('v-traps-at-last-page','((current + direction) % count + count) % count','direction > 0 ? Math.Min(current + direction, count - 1) : ((current + direction) % count + count) % count','V_REACHES_OVERVIEW_AGAIN'),
        ('wrong-year-money','UiKit.Money(totals.TimberRevenueCents)','UiKit.Money(1240000)','MONEY_FROM_SELECTED_YEAR'),
        ('missing-snapshot-invents-zero','AddTagged(forest, HistoryProvenance.NotRecorded, "No ecological snapshot recorded for this year.")','AddTagged(forest, HistoryProvenance.NotRecorded, "Canopy 0 %")','MISSING_SNAPSHOT_IS_EXPLICIT'),
    ]
    for name,before,after,assertion in mutants:
        assert source.count(before)==1, (name,'mutation site')
        r=run(name,source.replace(before,after))
        assert r.returncode != 0 and 'FAIL '+assertion in r.stderr and 'error CS' not in r.stdout, (name,r.stdout,r.stderr)
        print(name+': KILLED by '+assertion+' (compiled source slice)')
    # Original guard consumers are verified by PlayerFeedbackOvernight separately.
    uiroot=(root/'Assets/ForestPrototype/UI/ScenarioOneUiRoot.cs').read_text()
    assert 'root.Add(inspection.CompetitorPanel)' not in uiroot and 'PlaceSidePanel(' not in uiroot
    assert 'hud.ShowStatus(true);' in uiroot
    assert 'inspection.StepDetail(keys.leftShiftKey.isPressed || keys.rightShiftKey.isPressed ? -1 : 1);' in uiroot
    assert 'new ScrollView' not in (root/'Assets/ForestPrototype/UI/TreeInspectionView.cs').read_text()
    assert 'BuildWork(m, year, Column(columns, "Work done"));\n        BuildMoney(m, year, Column(columns, "Money"));\n        BuildForest(m, year, Column(columns, "Forest changes"), historical);' in (root/'Assets/ForestPrototype/UI/AnnualReviewView.cs').read_text()
    print('Composition/source contracts PASS (not render proof)')
    (out/'source-hashes.json').write_text(json.dumps({str(p.relative_to(root)):hashlib.sha256(p.read_bytes()).hexdigest() for p in sorted((root/'Assets/ForestPrototype/UI').rglob('*')) if p.is_file()},indent=2))
    return 0
if __name__=='__main__': sys.exit(main())
