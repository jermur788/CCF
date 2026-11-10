#!/usr/bin/env python3
"""Mutate only disposable C# inputs to the same typed compile; require focused assertions to kill every mutation."""
import pathlib,subprocess,sys
root=pathlib.Path.cwd(); base=pathlib.Path('/tmp/ccf363-compile-phase4/check.csproj').read_text()
out=pathlib.Path('/tmp/ccf363-mutations');out.mkdir(exist_ok=True)
mutants=[('unsafe-pulse','UI/ScenarioOneTeachingSequence.cs','public static bool SafePulseTarget(string name) =>','public static bool SafePulseTarget(string name) => name == "workplan-approve" ||'),
 ('no-teaching-overlay','UI/Map/StandMapDrawPlan.cs','if (model.TeachingFrame.HasValue &&','if (false && model.TeachingFrame.HasValue &&'),
 ('no-feeder','UI/Map/StandMapDrawPlan.cs','plan.Add(HeadlandStroke(t,surface.Feeder,StandMapSymbols.Get(MapSymbol.LinkTrack),Math.Max(1.5f,t.MetresToPixels(surface.Feeder.WidthM)*.3f)));',';')]
for name,rel,before,after in mutants:
 src=root/'Assets/ForestPrototype'/rel;text=src.read_text();assert before in text
 d=out/name;d.mkdir(exist_ok=True);m=d/src.name;m.write_text(text.replace(before,after))
 (d/'check.csproj').write_text(base.replace(str(src),str(m)))
 b=subprocess.run(['dotnet','build',str(d/'check.csproj'),'-v','q'],capture_output=True,text=True)
 assert b.returncode==0,(name,b.stdout,b.stderr)
 check=subprocess.run([sys.executable,str(root/'Tools/Verification/PlayerFeedbackOvernight/run_checks.py'),str(d/'bin/Debug/netstandard2.1/check.dll'),str(d/'assertions')],capture_output=True,text=True)
 (d/'assertions.txt').write_text(check.stdout+check.stderr)
 assert check.returncode!=0, 'SURVIVED: '+name
 assert 'Unhandled exception.' in check.stdout+check.stderr, 'not an assertion rejection: '+name
 print(name+': KILLED by focused assertion (typed mutant compiled)')
