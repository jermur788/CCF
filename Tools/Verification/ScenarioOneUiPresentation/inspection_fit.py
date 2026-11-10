#!/usr/bin/env python3
"""Additive #377 source/consumer check. Does NOT measure pixels or Unity fit."""
import argparse
from pathlib import Path
import subprocess
import sys
sys.dont_write_bytecode = True
from run_checks import member


def contracts(source):
    overview = source[source.index('        if (page == 0)'):source.index('        else if (page == 1)')]
    return {
        'OVERVIEW_ROUTES_TO_REAL_DRAFT': 'DraftOverview(4 + neighbourPages)' in overview,
        'NO_VERBOSE_DRAFT_IN_OVERVIEW': 'No valid open neighbour-felling jobs' not in overview and 'CompetitionAfterPlanned' not in overview and 'Fell marks have forecast effects' not in overview,
        'COMPACT_NEIGHBOURS': 'BuildNeighbours(report, competitors, 0, 2, false)' in overview,
        'MFM_FIRST_PAGE_HINT': 'reviewContext.Hint, "inspect-hint"' in overview,
        'DRAFT_REACHABLE': 'page < 6 + neighbourPages' in source and 'page - 3 - neighbourPages' in source,
        'TIMBER_AFTER_THREE_DRAFT_PAGES': 'PageCount = 6 + neighbourPages + timberPages' in source and '(page - 6 - neighbourPages) * 2' in source,
        'ALL_CATEGORIES_REACHABLE': all(x in source for x in ['BuildSite(tree, eco)', 'BuildCondition(tree, eco, report, reviewContext)', '(page - 3) * 3, 3, true', 'BuildDraft(report, competitors, assessing, treatment', 'timber.Skip(start).Take(2)']),
    }


def main():
    ap=argparse.ArgumentParser();ap.add_argument('--src',type=Path,required=True);ap.add_argument('--out',type=Path,required=True);a=ap.parse_args();a.out.mkdir(parents=True,exist_ok=True)
    source=(a.src/'Assets/ForestPrototype/UI/TreeInspectionView.cs').read_text()
    for name,ok in contracts(source).items():
        assert ok,name
        print('PASS source contract',name)
    for name,old,new in [('NO_VERBOSE_DRAFT_IN_OVERVIEW','UiKit.Add(body, DraftOverview(4 + neighbourPages), "draft-warning");','UiKit.Add(body, "No valid open neighbour-felling jobs");'),('OVERVIEW_ROUTES_TO_REAL_DRAFT','DraftOverview(4 + neighbourPages)','DraftOverview(4)')]:
        assert source.count(old)==1
        assert not contracts(source.replace(old,new))[name]
        print('KILLED source mutation',name)
    code='''using System;using System.Collections.Generic;
class CropTreeCompetitionReport {public float CompetitionNow=2,CompetitionAfterPlanned=1,ChangeFraction=-0.5f,PlannedShare=0.5f;public int PlannedNeighbourCount=3;public bool HasPlannedRemovals;}
class CompetitorAssessment {public float TargetCi50=1;}
static class CropTreeCompetition {public static float GrowthWithheld(float ci,float target)=>ci/(ci+target);}
static class StormModel {public const int None=0;}
class Ecology {public int StormModelVersion;}
class Ui {public Ecology Ecology=new Ecology();}
static class UiKit {public static void Add(List<string> b,string s,string style){b.Add(s);}public static void Line(List<string> b,string k,string v){b.Add(k+": "+v);}public static string F(float f,string fmt)=>f.ToString(fmt);}
class Probe {List<string> body=new List<string>();Ui ui=new Ui();
/*OVERVIEW*/
/*DRAFT*/
static string Percent(float f)=>(f*100).ToString()+" %";
static void Need(bool ok,string label){if(!ok)throw new Exception("FAIL "+label);}
static void Main(){var p=new Probe();var r=new CropTreeCompetitionReport();var c=new CompetitorAssessment();
for(int n=0;n<=60;n++){int first=4+(n+2)/3;Need(DraftOverview(first).Contains($"pages {first}–{first+2}"),"ACTUAL_DRAFT_ROUTE");}
p.BuildDraft(r,c,true,"FORECAST_TOKEN",0);string competition=string.Join(" ",p.body);Need(competition.Contains("3 neighbours")&&competition.Contains("unqueued marks")&&competition.Contains("Potential diameter growth withheld"),"COMPETITION_AND_UNQUEUED_EXPLANATION");
p.body.Clear();p.BuildDraft(r,c,true,"FORECAST_TOKEN",1);string forecast=string.Join(" ",p.body);Need(forecast.Contains("FORECAST_TOKEN")&&forecast.Contains("does not establish wind safety"),"FORECAST_AND_WIND_WARNING");
p.body.Clear();p.BuildDraft(r,c,true,"FORECAST_TOKEN",2);string limits=string.Join(" ",p.body);Need(limits.Contains("MFM proposal")&&limits.Contains("year advance executes approved work")&&limits.Contains("not a saved pre-mark baseline"),"AUTHORITY_AND_BASELINE_LIMITS");
p.body.Clear();p.BuildDraft(r,c,false,"",0);p.BuildDraft(r,c,false,"",1);Need(string.Join(" ",p.body).Contains("No local Crop Tree")&&string.Join(" ",p.body).Contains("No stand-wide"),"MISSING_DATA_EXPLICIT");Console.WriteLine("PASS compiled draft consumers and 61 actual page routes");}}
'''
    code=code.replace('/*OVERVIEW*/',member(source,'public static string DraftOverview(')).replace('/*DRAFT*/',member(source,'private void BuildDraft('))
    def run(name,text):
        dest=a.out/name;dest.mkdir(exist_ok=True);(dest/'Probe.cs').write_text(text);(dest/'probe.csproj').write_text('<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net8.0</TargetFramework></PropertyGroup></Project>');r=subprocess.run(['dotnet','run','--project',str(dest/'probe.csproj'),'-v','q'],capture_output=True,text=True);(dest/'result.log').write_text(r.stdout+r.stderr);return r
    positive=run('positive',code);assert positive.returncode==0,positive.stdout+positive.stderr;print(positive.stdout.strip())
    for name,old,new,label in [('wrong-route','{firstDraftPage}–{firstDraftPage + 2}','1–3','ACTUAL_DRAFT_ROUTE'),('lost-wind-warning','Storm events are off; exposure does not establish wind safety.','Storm events are off.','FORECAST_AND_WIND_WARNING')]:
        assert code.count(old)==1;r=run(name,code.replace(old,new));assert r.returncode!=0 and 'FAIL '+label in r.stderr and 'error CS' not in r.stdout;print('KILLED compiled mutation',name)

if __name__=='__main__': main()
