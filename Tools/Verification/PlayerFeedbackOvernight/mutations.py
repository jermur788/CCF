#!/usr/bin/env python3
"""Mutate only disposable C# inputs to the same typed compile; require the focused assertions to kill every mutation.
Run from the repository root (or an export of it). #365: each mutant must die on its OWN named assertion, not on any crash,
and one source-text regression must be refused by the HUD source contract.
CCF_FEEDBACK_COMPILE: folder holding the passing check.csproj (default the #363 path). CCF_FEEDBACK_MUTANTS: output folder."""
import os,pathlib,subprocess,sys
sys.dont_write_bytecode=True
here=pathlib.Path(__file__).resolve().parent
sys.path.insert(0,str(here))
import hud_contract
root=pathlib.Path.cwd(); base=(pathlib.Path(os.environ.get('CCF_FEEDBACK_COMPILE') or '/tmp/ccf363-compile-phase4')/'check.csproj').read_text()
out=pathlib.Path(os.environ.get('CCF_FEEDBACK_MUTANTS') or '/tmp/ccf363-mutations');out.mkdir(parents=True,exist_ok=True)
# name, file, original text, mutated text, the assertion message that must reject it
mutants=[('unsafe-pulse','UI/ScenarioOneTeachingSequence.cs','public static bool SafePulseTarget(string name) =>','public static bool SafePulseTarget(string name) => name == "workplan-approve" ||','commitment/modal never pulses workplan-approve'),
 ('no-teaching-overlay','UI/Map/StandMapDrawPlan.cs','if (model.TeachingFrame.HasValue &&','if (false && model.TeachingFrame.HasValue &&','outline and two separators'),
 ('no-feeder','UI/Map/StandMapDrawPlan.cs','plan.Add(HeadlandStroke(t,surface.Feeder,StandMapSymbols.Get(MapSymbol.LinkTrack),Math.Max(1.5f,t.MetresToPixels(surface.Feeder.WidthM)*.3f)));',';','Sequence contains no matching element'),
 # #365 B-1: the #363 behaviour, the Crop Tree checklist for every teaching-area lesson
 ('hud-crop-prompt-in-objectives-3-5','UI/ScenarioOneRackBayText.cs','else if (bay != null && hasPlayer && lesson == 1) progress = Walking(bay, x, z, aimed);','else if (bay != null && hasPlayer && lesson >= 1 && lesson <= 4) progress = Walking(bay, x, z, aimed);','Objective 3: no Objective 2 Crop Tree prompt or tick boxes'),
 # #365 B-2: the review state ignored
 ('hud-crop-prompt-during-unread-review','UI/ScenarioOneRackBayText.cs','bool review = lessonReviewPending || annualReviewUnread;','bool review = false;','unread first-access review: review title'),
 # #365 B-3: the #363 box that hid rack 4
 ('old-existing-clearing-box','UI/TeachingDiagramPlan.cs','new Text("Existing\\nclearing", 300f, 176f, 120f, 60f,','new Text("Existing clearing", 290f, 145f, 205f, 30f,',"figure label clear by 3 units: 'Existing clearing' touches rack4 band, rack number 4"),
 # #365 N-1
 ('legend-entrance-ungrouped','UI/Map/StandMapLegend.cs','e.Kind == LegendKind.Band || e.Symbol == MapSymbol.RackEntrance || ','e.Kind == LegendKind.Band || ','legend: suggestions only (0 of 5) lists the hollow entrance target under Your plan')]
for name,rel,before,after,expected in mutants:
 src=root/'Assets/ForestPrototype'/rel;text=src.read_text(encoding='utf-8');assert text.count(before)==1,(name,'mutation site not found exactly once')
 d=out/name;d.mkdir(exist_ok=True);m=d/src.name;m.write_text(text.replace(before,after),encoding='utf-8')
 assert str(src) in base,(name,'compile project does not name '+str(src))
 (d/'check.csproj').write_text(base.replace(str(src),str(m)))
 b=subprocess.run(['dotnet','build',str(d/'check.csproj'),'-v','q'],capture_output=True,text=True)
 assert b.returncode==0,(name,b.stdout,b.stderr)
 # The unmutated source root: only the compiled mutant differs, so the source contract passes and the assertions decide.
 check=subprocess.run([sys.executable,'-B',str(here/'run_checks.py'),str(d/'bin/Debug/netstandard2.1/check.dll'),str(d/'assertions'),str(root)],capture_output=True,text=True)
 log=check.stdout+check.stderr
 (d/'assertions.txt').write_text(log)
 assert check.returncode!=0, 'SURVIVED: '+name
 assert 'Unhandled exception.' in log and expected in log, 'not rejected by its own assertion: '+name+' (expected "'+expected+'")'
 print(name+': KILLED by "'+expected+'" (typed mutant compiled)')
# Source-text regression: HudModel builds the checklist itself again, as #363 did. The contract must refuse it.
ui=hud_contract.read(root)
assert hud_contract.problems(ui)==[], 'contract must pass on the real source first'
good='            Progress = progress,'
assert ui['LearningObjectivesView.Sequence.cs'].count(good)==1
ui['LearningObjectivesView.Sequence.cs']=ui['LearningObjectivesView.Sequence.cs'].replace(good,'            Progress = TeachingBay != null && ui.Player != null ? ScenarioOneRackBayText.Walking(TeachingBay, at.x, at.z, null) : TeachingProgress(f),')
found=hud_contract.problems(ui)
assert found, 'SURVIVED: hud-model-bypasses-selector'
print('hud-model-bypasses-selector: REFUSED by HUD source contract ('+str(len(found))+' findings; source text, not compiled)')
