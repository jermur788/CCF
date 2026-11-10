#!/usr/bin/env python3
"""Exact consumer slices + callsite contracts; no Unity, input, layout or save evidence.
Mutations are applied to in-memory source copies and must fail their named check.
"""
import argparse, pathlib, subprocess, json
from run_checks import member

def contracts(src):
    t,w,a,h,r=[src[n] for n in ['TreeInspectionView.cs','WorkPlanView.cs','AnnualReviewView.cs','WalkingHudView.cs','ScenarioOneUiRoot.cs']]
    checks={
      'SELECTED_YEAR_HEADLINES': 'BuildHeadlines(year);' in member(a,'public void Refresh(') and 'year = FindYear(years, viewYear);' in a,
      'HISTORY_ACK_GUARD': 'if (!showingEarlierYear) ui.AcknowledgeAnnualReview();' in a,
      'BACK_ENABLED': 'walk.SetEnabled(false)' not in a and '"Back to forest [Esc]", () => ui.CloseAll()' in a,
      'YEAR_CASH_ONLY': 'statusTitle.text = $"Year {m.CurrentEcologicalYear}";' in h and 'cash.text = "Cash " + UiKit.Money(m.CashCents);' in h,
      'RACK_FIRST_CALLERS': 'UiKit.KeyPrompts(contextActions, controls, m.RackFirstStageActive)' in h and '!rackFirst && locked && markTarget' in t and '!rackFirst && locked && cropTarget' in t,
      'NONMARKABLE': '!ui.Marking.HasRackEntranceMark(fellTarget)' in t and '!ui.Marking.HasRackEntranceMark(aimed)' in t and 'r.removalTreeIds.Contains(aimed.TreeId)' in t,
      'STATIC_APPROVE_SEPARATE_ADVANCE': '() => { ui.ApproveWorkPlan(); Refresh(true); }' in w[w.index('footer.Clear();'):] and '"Advance one year", OnAdvanceClicked' in w,
      'DETAIL_STATE': 'value = openDetails.Contains("annual-records")' in a,
      'PAGE_NAV_FIRST': t.index('Page {page + 1} of') < t.index('"SELECTED TREE"'),
      'STABLE_GROUP': "heading.Split('·')[0].Trim()" in w,
      'WARNINGS_OPEN': 'specialist harvesting work", alwaysOpen: true)' in w and 'then check the quote.", alwaysOpen: true)' in w,
      'OCCLUSION': 'OccludingPanels.Add(hud.ObjectiveWidget.Root)' in r and 'OccludingPanels.Add(hud.ContextActions)' in r,
    }
    # Restrict the actual assignment block rather than accepting any occurrence elsewhere.
    strip=h[h.index('statusTitle.text ='):h.index('// The objective',h.index('statusTitle.text ='))]
    checks['YEAR_CASH_ONLY']=strip.count('.text =')==2 and 'Browsing' not in strip
    return checks

def dynamic(src,out):
    t,w=src['TreeInspectionView.cs'],src['WorkPlanView.cs']
    hints=t[t.index('        ControlHints ='):t.index('        if (!rebuild) return;')]
    code='''using System;using System.Collections.Generic;using System.Linq;
class E {public bool Enabled=true;public List<E> Children=new();public string Text="";public void AddToClassList(string x){}public void SetEnabled(bool x){Enabled=x;}}
class VisualElement:E{}
static class UiKit {public static VisualElement Box(params string[] x)=>new();public static void Add(E e,string t,string c){e.Children.Add(new E{Text=t});}}
static class ScenarioOneWorkPlanFocus {public const string LaterObjectiveNote="Available in a later objective.";}
class Tree {public bool IsMarkedForFell,IsCropTree;public string TreeId="P1";}
class Context {public string Hint;}
class Probe {
VisualElement scroll=new();string ControlHints;
static string NeighbourHint(int n)=>n>0?"[1] Neighbour":"";
/*LOCKED*/
public void Run(){LockedCard("Planting");if(scroll.Children.Count!=1||scroll.Children[0].Enabled||!scroll.Children[0].Children.Any(x=>x.Text==ScenarioOneWorkPlanFocus.LaterObjectiveNote))throw new Exception("LOCKED_NOTE_VISIBLE");
foreach(bool rackFirst in new[]{false,true}) foreach(bool locked in new[]{false,true}) foreach(bool candidate in new[]{false,true}) {
int listed=0,page=0,PageCount=4;bool markTarget=false,cropTarget=true;var fellTarget=new Tree();var aimed=new Tree();var reviewContext=candidate?new Context{Hint="[C] Confirm as Crop Tree · [N] Reject candidate"}:null;
string rackPrompt="Teaching prose without a key";
/*HINTS*/
if(ControlHints.Contains("Close · Teaching"))throw new Exception("CLEAN_E");
if(candidate&&locked&&!rackFirst&&(!ControlHints.Contains("[N] Reject")||ControlHints.Contains("[C] Crop Tree")))throw new Exception("MFM_ACTUAL_KEYS");
if((rackFirst||!locked)&&ControlHints.Contains("[C]"))throw new Exception("RACK_FIRST_HINTS");
}
Console.WriteLine("PASS consumer sink: locked note and 8 actual hint states");}
static void Main()=>new Probe().Run();}
'''.replace('/*LOCKED*/',member(w,'private void LockedCard(')).replace('/*HINTS*/',hints)
    # The sink uses a list rather than UI Toolkit Add; keep the exact consumer method intact.
    code=code.replace('public void AddToClassList', 'public void Add(E e){Children.Add(e);}public void AddToClassList')
    out.mkdir(parents=True,exist_ok=True);(out/'Probe.cs').write_text(code)
    (out/'probe.csproj').write_text('<Project Sdk="Microsoft.NET.Sdk"><PropertyGroup><OutputType>Exe</OutputType><TargetFramework>net8.0</TargetFramework><UseSharedCompilation>false</UseSharedCompilation></PropertyGroup></Project>')
    r=subprocess.run(['dotnet','run','--project',str(out/'probe.csproj'),'-v','q'],capture_output=True,text=True)
    (out/'result.log').write_text(r.stdout+r.stderr)
    assert r.returncode==0,r.stdout+r.stderr
    print(r.stdout.strip())

def main():
    p=argparse.ArgumentParser();p.add_argument('--src',type=pathlib.Path,required=True);p.add_argument('--out',type=pathlib.Path,required=True);a=p.parse_args()
    src={p.name:p.read_text() for p in (a.src/'Assets/ForestPrototype/UI').glob('*.cs')}
    result=contracts(src);assert all(result.values()),result
    dynamic(src,a.out/'positive')
    mutations=[
      ('AnnualReviewView.cs','BuildHeadlines(year);','BuildHeadlines(latest);','SELECTED_YEAR_HEADLINES'),
      ('AnnualReviewView.cs','if (!showingEarlierYear) ui.AcknowledgeAnnualReview();','ui.AcknowledgeAnnualReview();','HISTORY_ACK_GUARD'),
      ('AnnualReviewView.cs','walk.RemoveFromClassList("btn-primary");','walk.SetEnabled(false);','BACK_ENABLED'),
      ('WalkingHudView.cs','UiKit.KeyPrompts(contextActions, controls, m.RackFirstStageActive)','UiKit.KeyPrompts(contextActions, controls, false)','RACK_FIRST_CALLERS'),
      ('TreeInspectionView.cs','!ui.Marking.HasRackEntranceMark(fellTarget)','true','NONMARKABLE'),
      ('WorkPlanView.cs','() => { ui.ApproveWorkPlan(); Refresh(true); }','() => { ui.ApproveWorkPlan(); AdvanceNow(); Refresh(true); }','STATIC_APPROVE_SEPARATE_ADVANCE'),
      ('WalkingHudView.cs','// The objective','statusTitle.text = "Deer browsing";\n        // The objective','YEAR_CASH_ONLY'),
    ]
    for file,before,after,check in mutations:
        assert src[file].count(before)>=1,(file,before)
        bad=dict(src);bad[file]=bad[file].replace(before,after);assert not contracts(bad)[check],check
        print('KILLED source consumer mutation: '+check)
    for before,after,check in [
      ('ControlHints += " · " + reviewContext.Hint;','ControlHints += "";','MFM_ACTUAL_KEYS'),
      ('rackPrompt.Substring(firstRackKey)','rackPrompt','CLEAN_E'),
      ('UiKit.Add(card, ScenarioOneWorkPlanFocus.LaterObjectiveNote, "muted");','','LOCKED_NOTE_VISIBLE')]:
        bad=dict(src);file='WorkPlanView.cs' if check=='LOCKED_NOTE_VISIBLE' else 'TreeInspectionView.cs'
        if check=='CLEAN_E':
            # A keyed teaching sentence catches the old untrimmed suffix too.
            bad[file]=bad[file].replace('if (firstRackKey >= 0)', 'if (!string.IsNullOrEmpty(rackPrompt))').replace(before,after)
        else: bad[file]=bad[file].replace(before,after)
        try: dynamic(bad,a.out/check)
        except AssertionError as e:
            assert check in str(e) and 'error CS' not in str(e),str(e)
        else: raise AssertionError('SURVIVED '+check)
        print('KILLED executable consumer mutation: '+check)
    (a.out/'contracts.json').write_text(json.dumps(result,indent=2));print('PASS '+str(len(result))+' source contracts, 10 negative controls')
if __name__=='__main__':main()
