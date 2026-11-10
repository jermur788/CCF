using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

// Plain sink: deliberately no Unity DLL, window, PlayerPrefs, forest or save access.
public enum PickingMode { Position, Ignore }
public class VisualElement {
 public string name = ""; public PickingMode pickingMode;
 public readonly List<VisualElement> Items = new List<VisualElement>();
 public readonly List<string> Classes = new List<string>();
 public void Add(VisualElement e) { Items.Add(e); }
 public void Clear() { Items.Clear(); }
 public void AddToClassList(string c) { Classes.Add(c); }
 public List<T> Query<T>() where T:VisualElement => Items.SelectMany(e=>new[]{e}.Concat(e.Query<VisualElement>())).OfType<T>().ToList();
 public string Words => (this is Label l ? l.text+"|" : this is Button b ? b.text+"|" : "")+string.Join("",Items.Select(e=>e.Words));
}
public class Label:VisualElement { public string text; public Label(string t) { text=t; } }
public class Button:VisualElement { public string text; public bool Enabled=true; public Action Click; public void SetEnabled(bool v) { Enabled=v; } }
public class ScenarioOneUiRoot {
 public string Last="";
 public void ShowObjectives(){Last="Objectives";} public void ShowMap(){Last="Stand Map";}
 public void ShowWorkPlan(){Last="Work Plan";} public void ShowReview(){Last="Annual Review";}
 public void ShowCurrentHelp(){Last="Help";} public void CloseAll(){Last="Forest";}
}
public static class UiKit {
 public static VisualElement Box(params string[] c){var e=new VisualElement();foreach(var x in c)e.AddToClassList(x);return e;}
 public static Label Add(VisualElement p,string t,params string[] c){var l=new Label(t);p.Add(l);return l;}
 public static Button Button(string t,Action a){return new Button{text=t,Click=a};}
 public static string Money(long x)=>x.ToString(CultureInfo.InvariantCulture)+" cents";
 public static string SignedMoney(long x)=>Money(x);
 public static string F(float x,string f)=>x.ToString(f,CultureInfo.InvariantCulture);
 /*NAV*/
 /*KEYS*/
}
public static class InspectionProbe { /*PAGE*/ /*RANKS*/ }
public enum HistoryProvenance { Recorded, Derived, NotRecorded }
public class ScenarioAnnualReport { public int completedTasks,failedTasks;public float harvestedVolumeM3; }
public class YearMoneySummary { public ScenarioAnnualReport Authority;public long TimberRevenueCents,ContractorCostCents,StockCostCents; }
public class ScenarioEcologicalSnapshot { public float meanCanopy,deadwoodVolumeM3;public int occupiedRegenerationCells,livingTrees; }
public class YearHistoryView { public ScenarioAnnualReport Report;public YearMoneySummary Money=new YearMoneySummary();public ScenarioEcologicalSnapshot Snapshot,PreviousSnapshot; }
public class AnnualProbe {
 public VisualElement scroll=new VisualElement();
 static void AddTagged(VisualElement p,HistoryProvenance k,string t,string c="body"){UiKit.Add(p,t,c);}
 static void TaggedLine(VisualElement p,HistoryProvenance k,string t,string v,string c="value"){UiKit.Add(p,t+": "+v,c);}
 /*COLUMN*/
 /*HEADLINES*/
}
class Checks {
 static int n;
 static void Check(bool b,string name){if(!b)throw new Exception("FAIL "+name);n++;}
 static void Main(){
  foreach(string active in new[]{"Objectives","Stand Map","Work Plan","Annual Review"}){
   var ui=new ScenarioOneUiRoot();var plan=UiKit.Button("original",ui.ShowWorkPlan);
   var nav=UiKit.MenuNavigation(ui,active,work:plan);
   var buttons=nav.Items.OfType<Button>().ToList();
   Check(buttons.Select(b=>b.text).SequenceEqual(new[]{"Objectives [O]","Stand Map [M]","Work Plan [Tab]","Annual Review","Help [F1]","Back to forest [Esc]"}),"MENU_SAME_ORDER");
   Check(buttons.Count(b=>!b.Enabled)==1 && !buttons[new[]{"Objectives","Stand Map","Work Plan","Annual Review"}.ToList().IndexOf(active)].Enabled,"MENU_ACTIVE_IS_DISABLED");
   Check(object.ReferenceEquals(plan,buttons[2]),"EXISTING_NAMED_HANDLER_IS_REUSED");
   buttons[5].Click();Check(ui.Last=="Forest","BACK_NEVER_ACKNOWLEDGES_OR_ADVANCES");
   buttons[3].Click();Check(ui.Last=="Annual Review","REVIEW_DESTINATION_EXISTS");
  }
  var bar=new VisualElement();
  UiKit.KeyPrompts(bar,"[X] Plan salvage · [C] Crop Tree · [O] Objectives · [Tab] Review quote · [Tab] Work Plan",true);
  Check(!bar.Words.Contains("X|")&&!bar.Words.Contains("C|"),"RACK_FIRST_SUPPRESSES_BOTH_MARKS");
  Check(bar.Query<Label>().Count(l=>l.text=="Tab")==1,"CONTEXT_KEYS_ARE_UNIQUE");
  Check(bar.Query<Button>().Count==0&&bar.Query<VisualElement>().All(e=>e.pickingMode==PickingMode.Ignore),"LOCKED_CONTROLS_ARE_PROMPTS");
  UiKit.KeyPrompts(bar,"[X] Fell mark · [C] Crop Tree",false);
  Check(bar.Words.Contains("X|")&&bar.Words.Contains("C|"),"LATER_STAGE_MARK_PROMPTS_REMAIN");
  for(int count=1;count<=40;count++){
   int page=0;var seen=new HashSet<int>();
   for(int i=0;i<count;i++){seen.Add(page);page=InspectionProbe.NextPage(page,count);}
   Check(page==0,"V_REACHES_OVERVIEW_AGAIN");Check(seen.Count==count,"V_REACHES_EVERY_PAGE");
   Check(InspectionProbe.NextPage(0,count,-1)==count-1,"SHIFT_V_REACHES_LAST_PAGE");
  }
  Check(InspectionProbe.NextPage(0,0)==0,"EMPTY_PAGE_SET_SAFE");
  Check(InspectionProbe.NeighbourHint(0)=="","NO_INVALID_NEIGHBOUR_KEYS");
  Check(InspectionProbe.NeighbourHint(1)=="[1] Neighbour","ONE_NEIGHBOUR_KEY");
  Check(InspectionProbe.NeighbourHint(3)=="[1–3] Neighbours","REAL_RANK_RANGE");
  var report=new ScenarioAnnualReport{completedTasks=7,failedTasks=2,harvestedVolumeM3=3.25f};
  var year=new YearHistoryView{Report=report,Money=new YearMoneySummary{Authority=report,TimberRevenueCents=123456,ContractorCostCents=40000,StockCostCents=2711},Snapshot=new ScenarioEcologicalSnapshot{meanCanopy=.73f,occupiedRegenerationCells=19,deadwoodVolumeM3=4.5f,livingTrees=101},PreviousSnapshot=new ScenarioEcologicalSnapshot{livingTrees=111}};
  var annual=new AnnualProbe();annual.BuildHeadlines(year);var words=annual.scroll.Words;
  Check(words.IndexOf("WORK DONE")<words.IndexOf("MONEY")&&words.IndexOf("MONEY")<words.IndexOf("FOREST CHANGES"),"HEADLINE_DESCRIPTIVE_ORDER");
  Check(words.Contains("7 task(s) completed · 2 failed"),"WORK_FROM_SELECTED_REPORT");
  Check(words.Contains("Timber income: 123456 cents")&&words.Contains("Contractor work: 40000 cents")&&words.Contains("Recorded work materials: 2711 cents"),"MONEY_FROM_SELECTED_YEAR");
  Check(words.Contains("Income less contractor work: 83456 cents")&&words.Contains("Before materials and nursery purchases"),"NO_FALSE_FULL_NET");
  Check(words.Contains("73 %")&&words.Contains("19 map square(s)")&&words.Contains("111 → 101"),"SELECTED_SNAPSHOT_AND_PREVIOUS");
  annual=new AnnualProbe();annual.BuildHeadlines(new YearHistoryView());words=annual.scroll.Words;
  Check(words.Contains("No ecological snapshot recorded for this year."),"MISSING_SNAPSHOT_IS_EXPLICIT");
  Check(words.Contains("Annual money totals not recorded.")&&!words.Contains("0 cents"),"MISSING_MONEY_IS_NOT_ZERO");
  Check(words.Contains("Annual work totals not recorded."),"MISSING_WORK_IS_NOT_ZERO");
  Check(!words.Contains("biodiversity",StringComparison.OrdinalIgnoreCase),"NO_INVENTED_BIODIVERSITY");
  Console.WriteLine("Presentation source-slice checks PASS "+n+" (plain sink; no Unity/render/input claims)");
 }
}
