// #367 template: tokens are replaced with exact production source slices by consumer_paths.py.
// The sinks below record strings/classes only. They do not model Unity layout, events, saves or PlayerPrefs.
using System;
using System.Linq;
using System.Collections.Generic;
class Element {
 public string name, text; public Style style = new Style(); public List<Element> children = new List<Element>();
 public void Add(Element e) => children.Add(e);
 public IEnumerable<Element> All() => new[]{this}.Concat(children.SelectMany(c=>c.All()));
 public T Q<T>(string n) where T:Element => All().OfType<T>().FirstOrDefault(e=>e.name==n);
}
class Label:Element {}
class Foldout:Element { public bool value; }
class Style { public DisplayStyle display; }
enum DisplayStyle { None, Flex }
static class UiKit {
 public static Label Add(Element parent,string text,params string[] classes) { var l=new Label{text=text};parent.Add(l);return l; }
}
class RackLayoutTracker { public bool RackPlanV2,Complete,WorkPending,HasProgress; public int Done,Total; public string V2ObjectiveInstruction="V2 plan racks",ProgressInstruction="Plan remaining",V2CounterText="Racks 0/5"; }
class Manager { public bool RackPlanningV2Active=true,ReferencePreviewActive; }
class Context { public Manager Manager=new Manager(); public bool NeedsAnnualReview; public RackLayoutTracker RackLayout=new RackLayoutTracker{RackPlanV2=true}; }
class Window { public Element Root=new Element(); public bool TreatmentReviewOpen,HasWaypoint; }
class ConsumerView {
 public Context ui; public int CurrentTopicIndex; public bool NeedsReviewAcknowledgement;
 public bool TeachingRacks => CurrentTopicIndex==0;
 public bool ExtensionActive;
 public ScenarioOneRackBay.Analysis TeachingBay;
 public ScenarioOneTeachingSequence.Lesson CurrentLesson => ScenarioOneTeachingSequence.Lessons[CurrentTopicIndex];
 public string CurrentTitle => CurrentLesson.Heading;
 public Element Root=new Element();
 public string RackV2Concept="Map planned racks",RackV2Practice="Confirm and mark racks";
 public const string ReviewFirstInstruction="First: read and acknowledge the annual results in Work Plan [Tab].";
 public ScenarioOneTeachingSequence.Facts Facts;
 ScenarioOneTeachingSequence.Facts TeachingFacts()=>Facts;
 string TeachingInstruction(ScenarioOneTeachingSequence.Facts f)=>ScenarioOneRackBayText.Instruction(CurrentTopicIndex,f) ?? CurrentLesson.Action;
 string HudInstruction(ScenarioOneTeachingSequence.Facts f,int i,ScenarioOneTeachingSequence.Lesson l)=>ScenarioOneRackBayText.Hud(i,f) ?? l.HudAction;
 string TeachingProgress(ScenarioOneTeachingSequence.Facts f)=>ScenarioOneRackBayText.Progress(CurrentTopicIndex,f) ?? "stage progress "+CurrentTopicIndex;
 string ExtensionInstruction()=>"Extension: inspect and confirm Crop Tree candidates";
 static void AddObjectivePart(Element card,string heading,string text,string name) { UiKit.Add(card,heading);UiKit.Add(card,text).name=name; }
 /*PROPERTIES*/
 public Element Card() {
  var current=new Element(); var words=new Element(); var lesson=CurrentLesson;
  bool rackV2Objective=TeachingRacks && ui.Manager.RackPlanningV2Active && !ui.Manager.ReferencePreviewActive;
  /*TITLE*/
  /*CARD*/
  current.Add(words); return current;
 }
}
class Guidance {
 public ConsumerView learning; public Manager manager; public Window map=new Window(),workPlan=new Window(),review=new Window();
 public bool NeedsAnnualReview=>learning.ui.NeedsAnnualReview;
 public string Target;
 public string Reason(Element scope) {
  string target=null; string reason=learning.CurrentInstruction;
  if (false) {}
  /*WINDOW_BRANCH*/
  else return "";
  /*WINDOW_OUTPUT*/
  Target=target;return scope.Q<Label>("next-action-reason").text;
 }
}
class ConsumerChecks {
 static int count;
 static void Check(bool b,string n) { if(!b) throw new Exception("R367_"+n); count++; }
 static bool Crop(string s)=>s.Contains("Crop Tree")||s.Contains("[C]")||s.Contains("☑")||s.Contains("☐");
 static ScenarioOneRackBay.Analysis Bay() {
  var frame=ScenarioOneRackBay.Frame.Anchored(new RackPoint(0,0),1,0,9,new ScenarioOneRackBay.Corridor[0]);
  // Two real selections: stays Objective2 after ACK, with the middle third still needed.
  return ScenarioOneRackBay.AnalyseTeachingRack(new[]{new ScenarioOneRackBay.Tree("near",-8,5,30,true,true),new ScenarioOneRackBay.Tree("far",8,-5,30,true,true)},new ScenarioOneRackBay.Order[0],frame,3,5,2);
 }
 static ConsumerView View(int lesson=1,bool unread=true,bool lessonPending=false,bool v2=true,bool preview=false) {
  return new ConsumerView { ui=new Context{Manager=new Manager{RackPlanningV2Active=v2,ReferencePreviewActive=preview},NeedsAnnualReview=unread}, CurrentTopicIndex=lesson,NeedsReviewAcknowledgement=lessonPending,TeachingBay=Bay(),Facts=new ScenarioOneTeachingSequence.Facts{AccessOpen=true,AccessReviewed=!lessonPending,ReleaseReviewed=!lessonPending,Bay=Bay()} };
 }
 static string Text(Element e)=>string.Join("\n",e.All().Select(l=>l.text).Where(s=>s!=null));
 static Guidance Guide(ConsumerView v) {
  var g=new Guidance{learning=v,manager=v.ui.Manager};
  foreach(var e in new[]{v.Root,g.map.Root,g.workPlan.Root,g.review.Root}) e.Add(new Label{name="next-action-reason"});
  return g;
 }
 static void Review(ConsumerView v,string row,bool hasPlayer=true) {
  // First assertion is the original #366 adverse case, so a missing gate dies by name before any LINQ lookup.
  Check(v.CurrentShortInstruction==ScenarioOneRackBayText.ReviewInstruction,row+"_CURRENT_SHORT_REVIEW_FIRST");
  Check(v.CurrentInstruction==ScenarioOneRackBayText.ReviewInstruction,row+"_FULL_INSTRUCTION_REVIEW_ONLY");
  Check(v.Summary.Contains("Review the year"),row+"_SUMMARY_REVIEW");
  var card=v.Card();
  Check(card.Q<Label>("current-objective-title")?.text==v.CurrentTitle,row+"_LESSON_TITLE_PRESERVED");
  Check(card.Q<Label>("current-objective-review-first")?.text.StartsWith("First: Review the year")==true,row+"_CARD_REVIEW_BANNER");
  Check(card.Q<Label>("current-objective-instruction")?.text==ScenarioOneRackBayText.ReviewInstruction,row+"_CARD_WHAT_TO_DO_REVIEW");
  Check(card.Q<Label>("objective-full-instruction")==null,row+"_NO_CONTRADICTORY_FULL_DETAIL");
  Check(card.Q<Label>("current-objective-progress")?.text==ScenarioOneRackBayText.ReviewProgress,row+"_CARD_PROGRESS_REVIEW");
  Check(card.Q<Label>("current-objective-practice")==null && !Crop(v.CurrentShortInstruction+v.CurrentInstruction+v.CurrentProgress),row+"_NO_CROP_PROMPT_IN_CARD_OR_DETAIL");
  var g=Guide(v);
  // Objective1 retains its separate rack-plan navigation branch, pinned unchanged by the source contract below.
  if (!v.TeachingRacks) {
  foreach(var surface in new[]{("MAP",g.map.Root,"map-work-plan"),("WORKPLAN",g.workPlan.Root,"workplan-read-results"),("ANNUAL_REVIEW",g.review.Root,"annual-review-acknowledge")}) {
   Check(g.Reason(surface.Item2)==ScenarioOneRackBayText.ReviewInstruction,row+"_"+surface.Item1+"_REASON_REVIEW");
   Check(g.Target==surface.Item3 && ScenarioOneTeachingSequence.SafePulseTarget(g.Target),row+"_"+surface.Item1+"_SAFE_REVIEW_TARGET");
  }
  Check(g.Reason(v.Root)=="" && g.Target=="objective-work-plan",row+"_OBJECTIVES_NO_DUPLICATE_REASON_SAFE_TARGET");
  }
  ScenarioOneRackBayText.HudWords(true,v.CurrentTopicIndex,v.NeedsReviewAcknowledgement,v.ui.NeedsAnnualReview,false,v.TeachingBay,hasPlayer,0,3,null,v.CurrentTitle,"lesson instruction","stage progress",out var title,out var instruction,out var progress);
  Check(title=="Review the year" && !Crop(progress),row+"_HUD_REVIEW_NO_BOXES");
 }
 static void Main() {
  var v=View(); Review(v,"LATEST_UNREAD_EARLIER_ACK");
  Check(ScenarioOneTeachingSequence.Current(v.Facts)==1,"PRESENTATION_DOES_NOT_ADVANCE_OBJECTIVE2");
  v.ui.NeedsAnnualReview=false;
  Check(v.CurrentShortInstruction==ScenarioOneRackBayText.Hud(1,v.Facts) && v.CurrentShortInstruction.Contains("Crop Tree"),"AFTER_ACK_CROP_SHORT_RESTORED");
  Check(v.CurrentInstruction==ScenarioOneRackBayText.Instruction(1,v.Facts),"AFTER_ACK_FULL_LESSON_RESTORED");
  var card=v.Card();Check(card.Q<Label>("current-objective-review-first")==null && card.Q<Label>("objective-full-instruction")?.text==v.CurrentInstruction,"AFTER_ACK_FULL_DETAIL_RESTORED");
  Check(card.Q<Label>("current-objective-progress")?.text==ScenarioOneRackBayText.Progress(1,v.Facts),"AFTER_ACK_STAGE_PROGRESS_RESTORED");
  var g=Guide(v);Check(g.Reason(g.map.Root)==v.CurrentShortInstruction && g.Target=="map-teaching-waypoint","AFTER_ACK_MAP_CROP_REASON_WAYPOINT");
  Check(g.Reason(g.workPlan.Root)==v.CurrentShortInstruction && g.Target==null,"AFTER_ACK_WORKPLAN_NO_COMMIT_PULSE");
  Check(g.Reason(g.review.Root)==v.CurrentShortInstruction && g.Target==null,"AFTER_ACK_REVIEW_NO_ACK_PULSE");
  ScenarioOneRackBayText.HudWords(true,1,false,false,false,v.TeachingBay,true,0,3,null,v.CurrentTitle,v.CurrentShortInstruction,"progress",out var t,out var i,out var p);
  Check(p.Count(c=>c=='☑'||c=='☐')==3 && p.Contains("[C]"),"AFTER_ACK_HUD_THREE_ZONES");
  Review(View(1,false,true),"LESSON_ACK_MISSING_LATEST_READ");
  Review(View(),"NO_PLAYER",false);
  foreach(int stage in new[]{2,3,4}) {
   v=View(stage);Review(v,"OBJECTIVE"+(stage+1));v.ui.NeedsAnnualReview=false;
   card=v.Card();Check(card.Q<Label>("current-objective-review-first")==null && card.Q<Label>("current-objective-progress")?.text==ScenarioOneRackBayText.Progress(stage,v.Facts),"OBJECTIVE"+(stage+1)+"_ACK_RESTORES_PROGRESS");
   Check(v.CurrentShortInstruction==ScenarioOneRackBayText.Hud(stage,v.Facts),"OBJECTIVE"+(stage+1)+"_ACK_RESTORES_SHORT");
  }
  // A review cannot expose extension approvals/checklists at the same time.
  v=View(5);v.ExtensionActive=true;Review(v,"EXTENSION_UNREAD");
  foreach(bool preview in new[]{false,true}) foreach(bool pending in new[]{false,true}) foreach(int stage in new[]{0,1,2,3,4,7}) {
   v=View(stage,true,pending,!preview?false:true,preview);var before=v.Card();var shortBefore=v.CurrentShortInstruction;var longBefore=v.CurrentInstruction;var summary=v.Summary;
   v.ui.NeedsAnnualReview=false;
   Check(Text(before)==Text(v.Card()) && shortBefore==v.CurrentShortInstruction && longBefore==v.CurrentInstruction && summary==v.Summary,"OLD_WORLD_"+preview+"_"+pending+"_"+stage+"_UNREAD_WORDS_UNCHANGED");
   Check(before.Q<Label>("current-objective-review-first")==null,"OLD_WORLD_NO_V2_BANNER_"+preview+"_"+pending+"_"+stage);
  }
  v=View(0,false);Check(v.CurrentInstruction==v.ui.RackLayout.V2ObjectiveInstruction && v.Card().Q<Label>("current-objective-review-first")==null,"OBJECTIVE1_NORMAL_PLAN_INTACT");
  v.ui.NeedsAnnualReview=true;Review(v,"OBJECTIVE1_CARD_UNREAD");
  v=View(7,false);var completed=v.CurrentInstruction;v.ui.NeedsAnnualReview=true;Review(v,"FINAL_LESSON_LATEST_UNREAD");v.ui.NeedsAnnualReview=false;Check(v.CurrentInstruction==completed,"FINAL_LESSON_ACK_RESTORES_WORDS");
  foreach(bool v2 in new[]{false,true}) foreach(bool lesson in new[]{false,true}) foreach(bool unread in new[]{false,true})
   Check(ScenarioOneRackBayText.ReviewFirst(v2,lesson,unread)==(v2 && (lesson||unread)),"SELECTOR_TRUTH_TABLE_"+v2+"_"+lesson+"_"+unread);
  Console.WriteLine("Consumer paths PASS "+count+" (exact source slices; no Unity rendering or persistence)");
 }
}
