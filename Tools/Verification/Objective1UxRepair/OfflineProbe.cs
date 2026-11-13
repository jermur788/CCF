using System;
using System.Linq;
using System.Collections.Generic;

// Fake neighbours only: production visit, prompt, target, spray association and guidance selection are extracted verbatim.
public struct RackPoint { public float X,Z; public RackPoint(float x,float z) {X=x;Z=z;} }
public class RackPlanRack { public int Id; public RackPoint Entrance; public bool EntranceMarked,IsLocked; public string EntranceTreeId; }
public class ForestTree { public string TreeId; public bool IsCropTree,IsMarkedForFell; }
public enum RackPlanOperationStatus { Applied, Invalid }
public class RackPlanOperationResult { public RackPlanOperationStatus Status; public string Message; public static RackPlanOperationResult Invalid(string s) => new RackPlanOperationResult{Status=RackPlanOperationStatus.Invalid,Message=s}; }
public enum CursorLockMode { Locked, None }
public static class Cursor { public static CursorLockMode lockState=CursorLockMode.Locked; }
public class ScenarioOneManager {
 public bool RackPlanningV2Active=true,RackPlanConfirmed=true; public List<RackPlanRack> ConfirmedRacks=new List<RackPlanRack>(); public bool WorkPlanOpen,ReferencePreviewActive; public int InvalidId;
 public bool TryGetRackEntranceTarget(int id,out ForestTree tree,out string problem) { problem=id==InvalidId?"invalid":""; tree= id==InvalidId ? null : new ForestTree{TreeId=ConfirmedRacks.Single(r=>r.Id==id).EntranceTreeId}; return tree!=null; }
 public RackPlanOperationResult MarkRackEntrance(int id) { ConfirmedRacks.Single(r=>r.Id==id).EntranceMarked=true; return new RackPlanOperationResult{Status=RackPlanOperationStatus.Applied}; }
}
public class Cue { public void SetActive(bool b) {} }
public partial class ForestTreeMarkingManager {
 public ScenarioOneManager RackScenario=new ScenarioOneManager(); public bool RackSprayEquipped=true; public ForestTree Aim; private Cue entranceCue; public int CueRackId;
 private bool TryRackSprayAim(out ForestTree tree) {tree=Aim; return tree!=null;} public bool HasRackEntranceMark(ForestTree tree)=>RackScenario.ConfirmedRacks.Any(r=>r.EntranceTreeId==tree.TreeId&&r.EntranceMarked);
 private void CreateRackEntranceMarker(ForestTree t) {} private void PlaySprayFeedback() {} private void SetMessage(string s) {}
}
public enum DisplayStyle { Flex, None }
public class Style { public DisplayStyle display; }
public class VisualElement {
 public string name,Text,Class; public VisualElement parent; public Style style=new Style(); public List<VisualElement> Children=new List<VisualElement>();
 public void Add(VisualElement element) {element.parent=this;Children.Add(element);}
 public T Q<T>(string targetName) where T:VisualElement => Children.Select(child=>child.name==targetName?child as T:child.Q<T>(targetName)).FirstOrDefault(child=>child!=null);
}
public class Button:VisualElement {}
public static class UiKit { public static VisualElement Box(params string[] cls)=>new VisualElement(); public static VisualElement Add(VisualElement p,string text,params string[] cls) { var e=new VisualElement{Text=text,Class=string.Join(" ",cls)};p.Add(e);return e; } }
public class HelpProbe { public bool IsOpen; }
public class LearningProbe { public int CurrentTopicIndex; public bool NeedsReviewAcknowledgement; }
public class EditorProbe { public bool ConsentPending,ResetPending; }
public class MapProbe { public EditorProbe RackEditor=new EditorProbe(); public bool ReopenPending; }
public class WorkProbe { public bool ConfirmingAdvance; }
public class ReviewProbe { public VisualElement Root=new VisualElement(); }
public class HudProbe { public Widget ObjectiveWidget=new Widget(); public class Widget { public VisualElement Root=new VisualElement(); } }
public enum UiScreen { None,Map,Objectives }
public static class Time { public static float timeScale=1; public static int frameCount=20; }
public static class StandaloneSessionMenu { public static bool IsOpen; }
public static class ScenarioOneTeachingSequence { public static bool SafePulseTarget(string name)=>name=="map-plan-rack"; }
public partial class GuidanceProbe {
 public ScenarioOneManager manager=new ScenarioOneManager(); public HelpProbe help=new HelpProbe(); public int helpClosedFrame=-1;
 public LearningProbe learning=new LearningProbe(); public MapProbe map=new MapProbe(); public WorkProbe workPlan=new WorkProbe(); public HudProbe hud=new HudProbe();
 public ReviewProbe review=new ReviewProbe();
 public VisualElement nextAction; public bool objectiveOneDetailsOpened,NeedsAnnualReview; public UiScreen screen;
}
public static class OfflineProbe {
 static int checks;
 static void Check(bool ok,string why) {if(!ok) throw new Exception(why); checks++; Console.WriteLine("PROBE_PASS "+why);}
 public static void Main() {
  var m=new ForestTreeMarkingManager(); var ids=new[]{9,2,7,1,5};
  for(int i=0;i<5;i++)m.RackScenario.ConfirmedRacks.Add(new RackPlanRack{Id=ids[i],Entrance=new RackPoint(-35,-30+i*15),EntranceTreeId="tree"+ids[i]});
  Check(ForestTreeMarkingManager.RackVisitOrder(m.RackScenario.ConfirmedRacks).Select(r=>r.Id).SequenceEqual(ids),"south to north differs from stable ID order");
  for(int i=0;i<5;i++) Check(m.RackVisitIndex(ids[i])==i+1,"physical ordinal "+(i+1));
  Check(m.TryGetRackFieldTarget(out var rack,out var tree,out _)&&rack.Id==9,"before first spray: bottom target, without equipping prerequisite");
  m.RackSprayEquipped=false; Check(m.TryGetRackFieldTarget(out rack,out tree,out _)&&rack.Id==9,"target before spray equipped"); m.RackSprayEquipped=true;
  m.Aim=new ForestTree{TreeId="tree9"}; Check(m.RackSprayPrompt().StartsWith("Rack 1 entrance"),"bottom prompt ordinal"); m.TrySprayRackEntrance();
  Check(m.TryGetRackFieldTarget(out rack,out tree,out _)&&rack.Id==2,"after bottom mark next ordinal 2");
  m.Aim=new ForestTree{TreeId="tree5"}; Check(m.RackSprayPrompt().StartsWith("Rack 5 entrance"),"out-of-order top prompt retains ordinal 5"); m.TrySprayRackEntrance();
  Check(m.RackSprayPrompt().StartsWith("Rack 5 entrance · Orange"),"already-marked far tree still says Rack 5");
  Check(m.RackScenario.ConfirmedRacks.Count(r=>r.EntranceMarked)==2&&m.RackScenario.ConfirmedRacks.Single(r=>r.Id==5).EntranceMarked,"out-of-order spray marks its own stable rack");
  Check(m.TryGetRackFieldTarget(out rack,out tree,out _)&&rack.Id==2&&m.RackVisitIndex(rack.Id)==2,"after top as second mark cue stays Rack 2");
  Check(m.RackScenario.ConfirmedRacks.Select(r=>r.Id).SequenceEqual(ids),"spraying does not mutate saved identities or plan order");
  m.RackScenario.InvalidId=2; Check(!m.TryGetRackFieldTarget(out rack,out tree,out var problem)&&problem=="invalid","invalid first candidate is disclosed, never silently redirected");
  m.RackScenario.RackPlanConfirmed=false; Check(!m.TryGetRackFieldTarget(out rack,out tree,out _),"unconfirmed negative");
  m.RackScenario.RackPlanConfirmed=true; m.RackScenario.RackPlanningV2Active=false; Check(!m.TryGetRackFieldTarget(out rack,out tree,out _),"Legacy negative");
  var tie=new[]{new RackPlanRack{Id=8,Entrance=new RackPoint(0,1)},new RackPlanRack{Id=3,Entrance=new RackPoint(0,1)}};
  Check(ForestTreeMarkingManager.RackVisitOrder(tie).First().Id==3,"equal entrance Z tie uses stable ID");
  var g=new GuidanceProbe(); Check(g.Select()==g.hud.ObjectiveWidget.Root,"initial Objective 1 cue before Details");
  g.objectiveOneDetailsOpened=true; Check(g.Select()==null,"initial cue ends on Details"); g.objectiveOneDetailsOpened=false;
  g.help.IsOpen=true; Check(g.Select()==null,"help suspension");g.help.IsOpen=false;g.helpClosedFrame=20;Check(g.Select()==null,"same-frame help closure suspension");g.helpClosedFrame=-1;
  g.screen=UiScreen.Objectives;Check(g.Select()==null,"walking cue hidden in Objectives");g.screen=UiScreen.None;
  g.nextAction=new VisualElement{name="map-plan-rack"};Check(g.Select()==g.nextAction,"existing Plan Racks safe pulse");
  g.nextAction=new VisualElement{name="workplan-approve"};Check(g.Select()!=g.nextAction,"Approve never pulses");g.nextAction=null;
  g.manager.RackPlanningV2Active=false;Check(g.Select()==null,"legacy cue negative");
  var acknowledgement=new VisualElement(); var acknowledge=new Button{name="annual-review-acknowledge"};
  acknowledgement.Add(acknowledge); g.review.Root.Add(acknowledgement);
  const string previousInstruction="Previous learning instruction";
  g.NeedsAnnualReview=true;
  Check(g.ReviewReason(g.review.Root,previousInstruction)==ScenarioOneRackBayText.ReviewOnScreenInstruction,"pending latest review gets on-screen acknowledgement instruction");
  Check(g.ReviewReason(new VisualElement(),previousInstruction)==previousInstruction,"non-review scope keeps previous instruction");
  acknowledgement.style.display=DisplayStyle.None;
  Check(g.ReviewReason(g.review.Root,previousInstruction)==previousInstruction,"earlier-year read-only review keeps previous instruction even with latest acknowledgement pending");
  acknowledgement.style.display=DisplayStyle.Flex; g.NeedsAnnualReview=false;
  Check(g.ReviewReason(g.review.Root,previousInstruction)==previousInstruction,"acknowledged latest review keeps previous instruction");
  g.NeedsAnnualReview=true; g.review.Root=new VisualElement();
  Check(g.ReviewReason(g.review.Root,previousInstruction)==previousInstruction,"missing acknowledgement control keeps previous instruction");
  Check(FeedbackProbe.RackNumbersMatchOrdinal("Marked the entrance tree tree9 for rack 1.",1),"feedback accepts matching visit ordinal, ignoring tree ID");
  Check(!FeedbackProbe.RackNumbersMatchOrdinal("Marked the entrance tree tree9 for rack 5.",1),"feedback rejects F-1 reversed-ID Rack 1 / stored rack 5");
  Check(!FeedbackProbe.RackNumbersMatchOrdinal("Marked the entrance tree tree9 for rack 1.",5),"feedback rejects F-1 reversed-ID Rack 5 / stored rack 1");
  Check(FeedbackProbe.RackNumbersMatchOrdinal("Marked the entrance tree tree9.",5),"feedback accepts neutral copy without a rack number");
  Check(!FeedbackProbe.RackNumbersMatchOrdinal("Rack 1 marked; RACK 5 next.",1),"feedback checks every rack number case-insensitively");
  Check(FeedbackProbe.RackNumbersMatchOrdinal("Rack 1 entrance marked; 1 / 5 entrances.",1),"feedback ignores non-rack counts");
  string summary=MenuHelpView.Summary(MenuHelpView.Menu.StandMap), detail=MenuHelpView.Explanation(MenuHelpView.Menu.StandMap), marks=MenuHelpView.Explanation(MenuHelpView.Menu.TreeMarks);
  Check(MenuHelpView.Title(MenuHelpView.Menu.StandMap)=="Stand Map"&&!summary.Contains("The map helps")&&!detail.Contains("The map helps"),"approved Stand Map copy deletions");
  var prose=new VisualElement();KeycapProbe.AddKeycapProse(prose,detail);
  var words=prose.Children.SelectMany(p=>p.Children).ToList();
  Check(words.Where(w=>w.Class=="keycap").Select(w=>w.Text).SequenceEqual(new[]{"M","M","Esc"}),"actual help renderer styles genuine keys only");
  var normalize=new Func<string,string>(s=>System.Text.RegularExpressions.Regex.Replace(s,@"\s|\[|\]", ""));
  Check(normalize(string.Concat(words.Select(w=>w.Text)))==normalize(detail),"actual help renderer preserves complete prose and punctuation");
  Check(detail.Contains("[M], [Esc] or Back to forest")&&!detail.Contains("[Back to forest]"),"only genuine keys bracketed");
  Check(!marks.Contains("A tree carries only one")&&marks.EndsWith("Marking cuts nothing: the rack's removal work still needs approval in the Work Plan [Tab]."),"exact exclusivity deletion preserves marking-cuts-nothing sentence");
  Check(MenuHelpView.TreeMarksOrangeText=="Orange vertical stripes identify rack entrance trees associated with machine-path removal in Scenario 1.","unchosen orange wording unchanged");
  Check(MenuHelpView.AnnualReviewReadPrompt=="One thinning does not finish continuous-cover forestry: when you have read them, walk back, inspect how the forest responded and reassess before the next intervention.","only annual opening sentence removed");
  var transform=new StandMapTransform(-40,-40,80,310);var at=transform.WorldToPixels(new RackPoint(-35,-30));
  Check(Math.Abs(at.X-19.375)<.001&&Math.Abs(at.Y-271.25)<.001,"south entrance minimap projection north-up");
  var restored=transform.PixelsToWorld(at);Check(Math.Abs(restored.Z+30)<.001&&Math.Abs(restored.X+35)<.001,"shared transform roundtrip");
  Console.WriteLine("OBJECTIVE1_UX_PROBE_PASS checks="+checks);
 }
}
