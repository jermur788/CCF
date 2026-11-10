#!/usr/bin/env python3
"""#367 offline source-slice consumer execution, NOT Unity UI/render verification.
Compile exact view property bodies, card emission statements and later-window guidance
branch against plain-data inputs and a minimal label sink. No PlayerPrefs/save/UI runtime.
The original 160 Checks.cs assertions are separately run and remain unchanged.
"""
import argparse, pathlib, subprocess, sys
sys.dont_write_bytecode = True
here = pathlib.Path(__file__).resolve().parent

def member(text, signature):
    if text.count(signature) != 1:
        raise AssertionError('CONSUMER_SOURCE_MEMBER: expected one ' + signature)
    start = text.index(signature)
    brace, arrow = text.find('{', start), text.find('=>', start)
    if 0 <= arrow < brace:
        return text[start:text.index(';', arrow) + 1]
    depth = 0
    for end in range(brace, len(text)):
        if text[end] == '{': depth += 1
        elif text[end] == '}':
            depth -= 1
            if depth == 0: return text[start:end+1]
    raise AssertionError('CONSUMER_SOURCE_MEMBER: unclosed ' + signature)

def generate(root, baseline=None):
    ui = root / 'Assets/ForestPrototype/UI'
    view = (ui/'LearningObjectivesView.cs').read_text()
    source_root = (ui/'ScenarioOneUiRoot.cs').read_text()
    if baseline:
        view = (baseline/'before-LearningObjectivesView.cs').read_text()
        source_root = (baseline/'before-ScenarioOneUiRoot.cs').read_text()
    properties = ['public string CurrentInstruction', 'public string CurrentShortInstruction', 'public string Summary']
    props = '\n'.join(member(view, s) for s in properties)
    if not baseline:
        props += '\n' + member(view, 'public bool ReviewFirst') + '\n' + member(view, 'public string CurrentProgress')
    # Only the baseline lacks the new presentation properties; its real old card/properties run verbatim.
    else: props += '\npublic bool ReviewFirst => false; public string CurrentProgress => TeachingProgress(TeachingFacts());'
    a, b = view.index('        var detail = new Foldout'), view.index('        words.Add(detail);')
    card = view[a:b] + '\nwords.Add(detail);'
    title = next(l.strip() for l in view.splitlines() if 'UiKit.Add(current, CurrentTitle,' in l)
    method = member(source_root, 'public void RefreshNextActionGuidance()')
    branch = member(method, 'else if (manager.RackPlanningV2Active)')
    # The exact assignment and Label output from RefreshNextActionGuidance, not a hand-written reason selector.
    a = method.index('        Label explanation = scope.Q<Label>("next-action-reason");')
    b = method.index('        ApplyNextActionGuidance();', a)
    output = method[a:b]
    return props, card, title, branch, output

def run(assembly, out, root, baseline=None):
    out.mkdir(parents=True, exist_ok=True)
    parts = generate(root, baseline)
    template = (here/'ConsumerPaths.cs').read_text()
    for key, value in zip(['PROPERTIES','CARD','TITLE','WINDOW_BRANCH','WINDOW_OUTPUT'],parts):
        template = template.replace('/*'+key+'*/', value)
    (out/'ConsumerProbe.cs').write_text(template)
    (out/'consumer.csproj').write_text(f'''<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net8.0</TargetFramework><EnableDefaultCompileItems>false</EnableDefaultCompileItems><NoWarn>CS0436;CS0649</NoWarn></PropertyGroup><ItemGroup><Compile Include="{out/'ConsumerProbe.cs'}"/><Reference Include="{assembly}"/></ItemGroup></Project>''')
    return subprocess.run(['dotnet','run','--project',str(out/'consumer.csproj'),'-v','q']).returncode

if __name__ == '__main__':
    ap = argparse.ArgumentParser(description=__doc__)
    ap.add_argument('assembly',type=pathlib.Path); ap.add_argument('out',type=pathlib.Path); ap.add_argument('root',type=pathlib.Path)
    ap.add_argument('--baseline-ui',type=pathlib.Path)
    a = ap.parse_args()
    sys.exit(run(a.assembly.resolve(),a.out.resolve(),a.root.resolve(),a.baseline_ui))
