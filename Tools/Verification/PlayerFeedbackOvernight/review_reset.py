#!/usr/bin/env python3
"""#370 offline annual-review lifecycle check, NOT Unity runtime. Compiles exact production slices
(MarkAnnualReviewSeen, RecordEvent, the AdvanceYear report tail, Capture/Restore lines, NeedsAnnualReview) into a plain-data
fixture and drives them with the exact #367 consumers. Usage: review_reset.py <check.dll> <out dir> <source root> [--mutant NAME]"""
import argparse, pathlib, re, subprocess, sys
sys.dont_write_bytecode = True
here = pathlib.Path(__file__).resolve().parent
sys.path.insert(0, str(here))
import consumer_paths
RESET = '        annualReviewSeen = false;\n'
NOTE = '        // Each generated report (a no-work year included) is unread until explicitly acknowledged; reuses the saved flag.\n'
MUTANTS = {'advance-year-omits-unread-reset': ('        annualReports.Add(report);\n' + NOTE + RESET, '        annualReports.Add(report);\n')}

def source_contract(manager, uiroot):
    advance = consumer_paths.member(manager, 'public bool AdvanceYear()')
    add = advance.index('annualReports.Add(report);')
    problems = []
    if advance.count('annualReviewSeen') != 1 or advance.count(RESET) != 1: problems.append('R370_SOURCE_ONE_RESET_IN_ADVANCEYEAR')
    elif advance.index(RESET) < add or advance.index(RESET) > add + 400: problems.append('R370_SOURCE_RESET_AT_REPORT_APPEND')
    if advance.rfind('return false') > add or 'throw' in advance[advance.index('ecology.AdvanceOneYear();'):add]: problems.append('R370_SOURCE_RESET_AFTER_EVERY_FAILURE_EXIT')
    if advance.index('ecology.AdvanceOneYear();') > add: problems.append('R370_SOURCE_RESET_AFTER_ECOLOGY_ADVANCE')
    if len(re.findall(r'annualReviewSeen\s*=\s*false', manager)) != 2: problems.append('R370_SOURCE_NO_OTHER_UNREAD_WRITES')
    if not re.search(r'public bool NeedsAnnualReview => manager != null && manager\.AnnualReports\.Count > 0 && !manager\.AnnualReviewSeen;', uiroot): problems.append('R370_SOURCE_NEEDS_ANNUAL_REVIEW_PREDICATE')
    return problems

def generate(root, mutant=None):
    manager = (root/'Assets/ForestPrototype/ScenarioOne/ScenarioOneManager.cs').read_text()
    if mutant:
        old, new = MUTANTS[mutant]
        assert manager.count(old) == 1, 'R370_MUTATION_SITE_MISSING'
        manager = manager.replace(old, new)
    uiroot = (root/'Assets/ForestPrototype/UI/ScenarioOneUiRoot.cs').read_text()
    found = [] if mutant else source_contract(manager, uiroot)
    advance = consumer_paths.member(manager, 'public bool AdvanceYear()')
    tail = advance[advance.index('        annualReports.Add(report);'):advance.index('        RecordEcologicalSnapshot();')]
    needs = re.search(r'public bool NeedsAnnualReview => [^;]+;', uiroot).group(0)
    capture = re.search(r'^\s*annualReviewSeen = annualReviewSeen,\s*$', manager, re.M).group(0)
    restore = re.search(r'^\s*annualReviewSeen = data\.annualReviewSeen;\s*$', manager, re.M).group(0).replace('annualReviewSeen =', 'l.annualReviewSeen =')
    tokens = {'NEEDS': needs, 'RECORD_EVENT': consumer_paths.member(manager, 'private void RecordEvent('),
              'MARK': consumer_paths.member(manager, 'public void MarkAnnualReviewSeen()'), 'TAIL': tail, 'CAPTURE': capture, 'RESTORE': restore}
    text = (here/'ReviewReset.cs').read_text()
    for k, v in tokens.items(): text = text.replace('/*'+k+'*/', v)
    return found, text

def run(assembly, out, root, mutant=None):
    out.mkdir(parents=True, exist_ok=True)
    found, probe = generate(root, mutant)
    for f in found: print(f)
    if found: return 1
    parts = consumer_paths.generate(root)
    consumers = (here/'ConsumerPaths.cs').read_text()
    for key, value in zip(['PROPERTIES','CARD','TITLE','WINDOW_BRANCH','WINDOW_OUTPUT'], parts): consumers = consumers.replace('/*'+key+'*/', value)
    consumers = consumers.replace('static ConsumerView View(', 'public static ConsumerView View(').replace('static void Review(', 'public static void Review(').replace('static void Main()', 'static void ConsumerMain()')
    (out/'ConsumerProbe.cs').write_text(consumers); (out/'ReviewResetProbe.cs').write_text(probe)
    (out/'reset.csproj').write_text(f'''<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net8.0</TargetFramework><EnableDefaultCompileItems>false</EnableDefaultCompileItems><StartupObject>ResetChecks</StartupObject><NoWarn>CS0436;CS0649;CS0169</NoWarn></PropertyGroup><ItemGroup><Compile Include="{out/'ConsumerProbe.cs'}"/><Compile Include="{out/'ReviewResetProbe.cs'}"/><Reference Include="{assembly}"/></ItemGroup></Project>''')
    return subprocess.run(['dotnet', 'run', '--project', str(out/'reset.csproj'), '-v', 'q']).returncode

if __name__ == '__main__':
    ap = argparse.ArgumentParser(description=__doc__)
    ap.add_argument('assembly', type=pathlib.Path); ap.add_argument('out', type=pathlib.Path); ap.add_argument('root', type=pathlib.Path)
    ap.add_argument('--mutant', choices=sorted(MUTANTS))
    a = ap.parse_args()
    sys.exit(run(a.assembly.resolve(), a.out.resolve(), a.root.resolve(), a.mutant))
