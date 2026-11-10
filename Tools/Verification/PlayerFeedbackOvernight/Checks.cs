using System;
using System.Linq;
using System.Collections.Generic;
class Checks {
 static int count;
 static void Check(bool b,string s) { if(!b) throw new Exception(s); count++; }
 static void Main() {
  foreach(float angle in new[]{0f,0.6f,1.2f}) {
   float ax=(float)Math.Cos(angle),az=(float)Math.Sin(angle);
   var f=ScenarioOneRackBay.Frame.Anchored(new RackPoint(1,2),ax,az,9,new ScenarioOneRackBay.Corridor[0]);
   RackPoint At(float a,float b)=>new RackPoint(1+ax*a-az*b,2+az*a+ax*b);
   var m=new StandMapModel { HasProperty=true, SizeM=80, MinX=-40,MinZ=-40,TeachingFrame=f };
   var plan=StandMapDrawPlan.Build(m,600,MapDetail.Full);
   Check(plan.Count(p=>p.Symbol==MapSymbol.TeachingArea && p.Text==null)==3,"outline and two separators");
   Check(plan.Count(p=>p.Text!=null)==3,"third labels");
   foreach(int i in new[]{0,1,2}) { var p=At(-8+i*8,5); Check(f.Zone(p.X,p.Z)==i,"rotated zone"); }
   var outside=At(13,5);Check(!f.Contains(outside.X,outside.Z),"outside rejected");
   var trees=new List<ScenarioOneRackBay.Tree>();
   for(int i=0;i<3;i++){var p=At(-8+i*8,i==1?-5:5);trees.Add(new ScenarioOneRackBay.Tree("t"+i,p.X,p.Z,30,true,true));}
   var a=ScenarioOneRackBay.AnalyseTeachingRack(trees,new ScenarioOneRackBay.Order[0],f,3,5,2);
   Check(a.HasEnoughFuture,"three spread both sides");
   var q=At(-7,5);trees.Add(new ScenarioOneRackBay.Tree("extra",q.X,q.Z,30,true,true));
   a=ScenarioOneRackBay.AnalyseTeachingRack(trees,new ScenarioOneRackBay.Order[0],f,3,5,2);Check(!a.HasEnoughFuture && a.TooClose.Count>0,"fourth and close fail");
   trees.RemoveAt(3); trees[1]=new ScenarioOneRackBay.Tree("fell",At(0,-5).X,At(0,-5).Z,30,true,false,true);
   Check(!ScenarioOneRackBay.AnalyseTeachingRack(trees,new ScenarioOneRackBay.Order[0],f,3,5,2).HasEnoughFuture,"Fell never Crop");
   m.TeachingFrame=null;Check(!StandMapDrawPlan.Build(m,600,MapDetail.Full).Any(p=>p.Symbol==MapSymbol.TeachingArea),"no invented sample");
  }
  foreach(var n in new[]{"workplan-approve","workplan-advance","map-rack-plan-confirm","approve-prescription","Fell","map-consent-decline","Help",""}) Check(!ScenarioOneTeachingSequence.SafePulseTarget(n),"commitment/modal never pulses "+n);
  Check(ScenarioOneTeachingSequence.SafePulseTarget("map-teaching-waypoint"),"safe waypoint");
  Check(ScenarioOneTeachingSequence.SafePulseTarget("annual-review-acknowledge"),"safe ack");
  Check(ScenarioOneTeachingSequence.SourceQualifier(HistoryProvenance.Reconstructed).Contains("reconstructed"),"reconstructed visible without chips");
  Check(ScenarioOneTeachingSequence.SourceQualifier(HistoryProvenance.Current)=="now","current never past fact");
  Check(ScenarioOneTeachingSequence.SourceQualifier(HistoryProvenance.NotRecorded)=="not recorded","absence explicit");
  Check(ScenarioOneTeachingSequence.SourceQualifier(HistoryProvenance.Recorded)=="","recorded no uncertainty invented");
  var sameSideFrame=ScenarioOneRackBay.Frame.Anchored(new RackPoint(0,0),1,0,9,new ScenarioOneRackBay.Corridor[0]);
  var sameSide=new[]{new ScenarioOneRackBay.Tree("a",-8,5,30,true,true),new ScenarioOneRackBay.Tree("b",0,5,30,true,true),new ScenarioOneRackBay.Tree("c",8,5,30,true,true)};
  Check(!ScenarioOneRackBay.AnalyseTeachingRack(sameSide,new ScenarioOneRackBay.Order[0],sameSideFrame,3,5,2).HasEnoughFuture,"one side absent never completes");
  var sameThird=new[]{new ScenarioOneRackBay.Tree("a",-9,-8,30,true,true),new ScenarioOneRackBay.Tree("b",-8,8,30,true,true),new ScenarioOneRackBay.Tree("c",8,5,30,true,true)};
  Check(!ScenarioOneRackBay.AnalyseTeachingRack(sameThird,new ScenarioOneRackBay.Order[0],sameSideFrame,3,5,2).HasEnoughFuture,"duplicate third never completes");
  var inputs = Enumerable.Range(0,5).Select(i=>new ScenarioOneRackSampleGeometry.RackInput(i+1,new RackPoint(-35.4f,-32+i*16),new RackPoint(40,-32+i*16),2.4f)).ToArray();
  var derived=ScenarioOneRackSampleGeometry.Derive(inputs,40);
  Check(derived.Valid && derived.TeachingRack.Id==3,"real Derive selects third confirmed rack");
  Check(!ScenarioOneRackSampleGeometry.Derive(inputs.Take(4).ToArray(),40).Valid,"invalid plan no sample");
  var site=new TeachingDiagramPlan.Site { StandSizeM=80,HeadlandX=-35.4f,HeadlandSouthZ=-37.7f,HeadlandNorthZ=37.7f,FeederZ=-27.4f,FeederEndX=-2.436166f,PathWidthM=2.6f,PadRadiusM=2.2f,RackWidthM=2.4f };
  var fig=TeachingDiagramPlan.ObjectiveOneFigure(site);
  Check(fig.Title=="Suggested rack locations","figure title");
  var suggestions=fig.Items.Where(i=>i.Kind==TeachingDiagramPlan.Kind.Corridor && i.SourceId!="").ToArray();
  Check(suggestions.Length==5 && suggestions.All(i=>i.State==TeachingDiagramPlan.CorridorState.Draft),"five dashed suggestions");
  Check(!TeachingDiagramPlan.ShowsFiveRackFigure(0,false) && !TeachingDiagramPlan.ShowsFiveRackFigure(1,true),"legacy/other lesson figure guard");
  Check(fig.Labels.Any(l=>l.Value.Contains("Entrance marked")) && fig.Labels.Any(l=>l.Value.Contains("20m")),"lifecycle and scale");
  Check(!MapHeadlandSurface.ShouldShowExistingHeadland(false,true,true) && !MapHeadlandSurface.ShouldShowExistingHeadland(true,false,true) && !MapHeadlandSurface.ShouldShowExistingHeadland(true,true,false),"legacy/inactive surface guards");
  foreach(int side in new[]{480,800}) {
   var map=new StandMapModel {HasProperty=true,MinX=-40,MinZ=-40,SizeM=80};
   var surface=new MapHeadlandSurface(new RackPoint(-35.4f,-37.7f),new RackPoint(-35.4f,37.7f),new RackPoint(-35.4f,-27.4f),new RackPoint(-2.436166f,-27.4f),2.6f,2.2f);
   map.ExistingHeadlandSurfaces.Add(surface);
   var path=new MapPath {WidthM=2.6f};path.Points.Add(new RackPoint(-2.436166f,-40));path.Points.Add(new RackPoint(-2.436166f,11));map.Paths.Add(path);
   var plan=StandMapDrawPlan.Build(map,side,MapDetail.Full);
   map.Racks.Add(new MapRack {RackId=3,Start=new RackPoint(-35.4f,0),End=new RackPoint(40,0),Symbol=MapSymbol.RackPlayer});
   var hollow=StandMapDrawPlan.BuildDynamic(map,side,MapDetail.Full).Single(i=>i.Symbol==MapSymbol.RackEntrance);
   Check(hollow.Colour.A==0 && hollow.StrokePx>0,"unmarked target hollow");
   map.Racks[0].EntranceMarked=true;
   Check(StandMapDrawPlan.BuildDynamic(map,side,MapDetail.Full).Single(i=>i.Symbol==MapSymbol.RackEntrance).Colour.A>0,"field marked target filled");
   var link=plan.Single(i=>i.Symbol==MapSymbol.LinkTrack);
   Check(link.DashPx>0 && link.GapPx>0,"link pattern reaches canvas");
   var t=map.TransformFor(side);var start=t.PixelsToWorld(link.Points[0]);var end=t.PixelsToWorld(link.Points[1]);
   Check(Math.Abs(start.X-surface.Feeder.Start.X)<.0001 && Math.Abs(end.X-surface.Feeder.End.X)<.0001 && Math.Abs(end.Z-surface.Feeder.End.Z)<.0001,"map source endpoint parity");
   Check(plan.IndexOf(link)>plan.FindLastIndex(i=>i.Symbol==MapSymbol.Path),"link above original path");
   Check(StandMapDrawPlan.Build(map,196,MapDetail.Mini).Any(i=>i.Symbol==MapSymbol.LinkTrack),"minimap source link");
   Check(StandMapLegend.Entries(map,MapOverlay.None).Any(i=>i.Symbol==MapSymbol.OldPath),"old path distinct key");
   System.IO.File.WriteAllText(System.IO.Path.Combine(Scratch,"plan-"+side+".json"),System.Text.Json.JsonSerializer.Serialize(plan,new System.Text.Json.JsonSerializerOptions{IncludeFields=true}));
  }
  int original=count;
  Repair365();
  Console.WriteLine("Focused checks PASS "+count+" ("+original+" original #363 + "+(count-original)+" new #365)");
 }

 // Scratch output for the pixel witness. The #363 default is kept; CCF_FEEDBACK_SCRATCH lets a rerun use its own folder.
 static string Scratch { get {
  string d=Environment.GetEnvironmentVariable("CCF_FEEDBACK_SCRATCH"); if(string.IsNullOrEmpty(d)) d="/tmp/ccf363-preflight";
  System.IO.Directory.CreateDirectory(d); return d; } }

 // ---------------- #365 repair checks ----------------
 struct Words { public string Title, Instruction, Progress; }
 static Words Hud(bool v2,int lesson,bool lessonReview,bool annualUnread,bool complete,ScenarioOneRackBay.Analysis bay,bool hasPlayer,string instruction,string lessonProgress) {
  ScenarioOneRackBayText.HudWords(v2,lesson,lessonReview,annualUnread,complete,bay,hasPlayer,0,3,null,"Lesson heading",instruction,lessonProgress,out string t,out string i,out string p);
  return new Words{Title=t,Instruction=i,Progress=p};
 }
 static bool CropPrompt(string s)=>s.Contains("[C]")||s.Contains("Mark Crop")||s.Contains("☑")||s.Contains("☐");
 static int Boxes(string s)=>s.Count(c=>c=='☑'||c=='☐');

 struct Box { public string Name; public float L,T,R,B; }
 static Box Rect(string n,float l,float t,float r,float b)=>new Box{Name=n,L=l,T=t,R=r,B=b};
 // Separation of two boxes: negative or zero when they overlap, otherwise the larger of the horizontal and vertical gaps.
 static float Gap(Box a,Box b)=>Math.Max(Math.Max(b.L-a.R,a.L-b.R),Math.Max(b.T-a.B,a.T-b.B));
 const float LineUnits=30f, MinGap=3f, CharWidthShare=0.6f;
 static Box LabelBox(string value,float x,float y,float w,float h)=>Rect("label '"+value.Replace("\n"," ")+"'",x,y,x+w,y+Math.Max(h,LineUnits*value.Split('\n').Length));
 // Everything an opaque label must stay clear of: the five suggested rack bands, their numbers, and the existing access surfaces.
 static List<Box> Obstacles(TeachingDiagramPlan.Stage fig) {
  var o=new List<Box>();
  foreach(var i in fig.Items) {
   if(i.Kind==TeachingDiagramPlan.Kind.Corridor && i.Id.StartsWith("rack")) o.Add(Rect(i.Id+" band",i.X,i.Y-i.Width/2,i.X1,i.Y+i.Width/2));
   if(i.Kind==TeachingDiagramPlan.Kind.Track && i.Id.StartsWith("existing-") && i.Id.EndsWith("-casing"))
    for(int k=0;k+3<i.Points.Length;k+=2) o.Add(Rect(i.Id,Math.Min(i.Points[k],i.Points[k+2])-i.Width/2,Math.Min(i.Points[k+1],i.Points[k+3])-i.Width/2,Math.Max(i.Points[k],i.Points[k+2])+i.Width/2,Math.Max(i.Points[k+1],i.Points[k+3])+i.Width/2));
   if(i.Kind==TeachingDiagramPlan.Kind.Pad && i.Id.EndsWith("-casing")) o.Add(Rect(i.Id,i.X-i.Radius,i.Y-i.Radius,i.X+i.Radius,i.Y+i.Radius));
   if(i.Kind==TeachingDiagramPlan.Kind.Clearing) o.Add(Rect(i.Id,i.X-i.Radius,i.Y-i.Radius,i.X+i.Radius,i.Y+i.Radius));
  }
  foreach(var l in fig.Labels) if(l.Value.Length==1 && char.IsDigit(l.Value[0])) o.Add(Rect("rack number "+l.Value,l.X,l.Y,l.X+l.Width,l.Y+l.Height));
  return o;
 }
 static List<string> Collisions(Box label,List<Box> obstacles) {
  var hits=new List<string>(); foreach(var o in obstacles) if(Gap(label,o)<MinGap) hits.Add(o.Name); return hits;
 }

 static void Repair365() {
  // ---- B-1 / B-2: which words the walking objective panel shows, per lesson and review state ----
  var frame=ScenarioOneRackBay.Frame.Anchored(new RackPoint(0,0),1,0,9,new ScenarioOneRackBay.Corridor[0]);
  var three=new List<ScenarioOneRackBay.Tree>{new ScenarioOneRackBay.Tree("near",-8,5,30,true,true),new ScenarioOneRackBay.Tree("mid",0,-5,30,true,true),new ScenarioOneRackBay.Tree("far",8,5,30,true,true)};
  var bay=ScenarioOneRackBay.AnalyseTeachingRack(three,new ScenarioOneRackBay.Order[0],frame,3,5,2);
  var empty=ScenarioOneRackBay.AnalyseTeachingRack(new List<ScenarioOneRackBay.Tree>(),new ScenarioOneRackBay.Order[0],frame,3,5,2);
  string read="Read the Annual Review, then walk back and look at the opened stand.";

  var w=Hud(true,1,true,true,false,empty,true,read,"lesson progress");
  Check(w.Title==ScenarioOneRackBayText.ReviewTitle,"unread first-access review: review title");
  Check(w.Instruction=="Next: "+read,"unread first-access review: lesson's own review instruction");
  Check(w.Progress=="Rack work done · read the annual results" && !CropPrompt(w.Progress),"unread first-access review: review progress, no Crop Tree prompt");
  w=Hud(true,1,true,false,false,empty,true,read,"lesson progress");
  Check(w.Title==ScenarioOneRackBayText.ReviewTitle && !CropPrompt(w.Progress),"lesson acknowledgement missing alone: no Crop Tree prompt");

  w=Hud(true,1,false,false,false,empty,true,"Choose one Crop Tree in each zone","lesson progress");
  Check(w.Title=="Lesson heading","Objective 2 after review: lesson title");
  Check(Boxes(w.Progress)==3 && w.Progress.Contains("Near") && w.Progress.Contains("Middle") && w.Progress.Contains("Far"),"Objective 2 after review: exactly three zone boxes");
  Check(w.Progress.EndsWith("[E] Inspect / [C] Mark Crop Tree"),"Objective 2 after review: Crop Tree prompt offered");
  Check(w.Progress.Contains("0/3"),"Objective 2 after review: count from the real bay");
  w=Hud(true,1,false,false,false,bay,true,"x","lesson progress");
  Check(w.Progress.Contains("☑ Near") && w.Progress.Contains("☑ Middle") && w.Progress.Contains("☑ Far") && w.Progress.Contains("3/3"),"Objective 2: ticks follow the bay");

  string[] lessonProgress={"","","Initial release: 0% minimum","To sell 1 · deadwood 1 · work not done","Revisited 1/3 · decisions 0/3 · stand portions 2/6"};
  foreach(int lesson in new[]{2,3,4}) {
   w=Hud(true,lesson,false,false,false,bay,true,"lesson instruction",lessonProgress[lesson]);
   Check(!CropPrompt(w.Progress),"Objective "+(lesson+1)+": no Objective 2 Crop Tree prompt or tick boxes");
   Check(w.Progress.EndsWith("\n"+lessonProgress[lesson]),"Objective "+(lesson+1)+": keeps its own lesson progress");
   Check(w.Progress.StartsWith("Inside teaching area") && w.Progress.Split('\n').Length==2,"Objective "+(lesson+1)+": one location line only");
   Check(w.Title=="Lesson heading" && w.Instruction=="Next: lesson instruction","Objective "+(lesson+1)+": lesson title and instruction");
   w=Hud(true,lesson,false,true,false,bay,true,"lesson instruction",lessonProgress[lesson]);
   Check(w.Title==ScenarioOneRackBayText.ReviewTitle && w.Instruction=="Next: "+ScenarioOneRackBayText.ReviewInstruction && !CropPrompt(w.Progress),"Objective "+(lesson+1)+" with unread Annual Review: review words, no Crop Tree prompt");
  }
  w=Hud(true,4,true,true,false,bay,true,"Read the Annual Review, then return to the retained trees.",lessonProgress[4]);
  Check(w.Title==ScenarioOneRackBayText.ReviewTitle && w.Progress=="Release work done · read the annual results" && !CropPrompt(w.Progress),"unread release-year review: review progress, no Crop Tree prompt");
  w=Hud(true,1,false,true,false,bay,true,"Choose one Crop Tree in each zone","lesson progress");
  Check(w.Title==ScenarioOneRackBayText.ReviewTitle && w.Instruction=="Next: "+ScenarioOneRackBayText.ReviewInstruction && !CropPrompt(w.Progress),"Objective 2 with a later unread Annual Review: review first, no Crop Tree prompt");
  w=Hud(true,1,false,false,false,bay,false,"x","lesson progress");
  Check(w.Progress=="lesson progress","no player: lesson progress");
  w=Hud(true,1,false,false,false,null,true,"x","lesson progress");
  Check(w.Progress=="lesson progress","no teaching area: lesson progress, no invented checklist");
  foreach(int lesson in new[]{0,5,6,7}) { w=Hud(true,lesson,false,false,false,bay,true,"x","lesson progress"); Check(w.Progress=="lesson progress","lesson index "+lesson+": never the teaching-area lines"); }
  w=Hud(true,6,true,false,false,null,true,"Read the review and compare the planted trees with last year.","Planting year 3 · current year 4");
  Check(w.Title==ScenarioOneRackBayText.ReviewTitle && w.Progress=="Planting year 3 · current year 4" && w.Instruction.StartsWith("Next: Read the review"),"establishment review: lesson progress kept, review title");
  w=Hud(true,7,false,false,true,null,true,"Scenario goals achieved.","lesson progress");
  Check(w.Instruction=="Scenario goals achieved.","completed scenario: no Next cue");
  // N-5: a world without Rack Planning V2 keeps exactly the words it was given, whatever else is passed.
  foreach(int lesson in new[]{0,1,2,3,4,6}) foreach(bool pending in new[]{false,true}) {
   w=Hud(false,lesson,pending,pending,false,bay,true,"legacy instruction","legacy progress");
   Check(w.Title=="Lesson heading" && w.Instruction=="legacy instruction" && w.Progress=="legacy progress","non-V2 world lesson "+lesson+" pending="+pending+": words unchanged, no Next cue");
  }
  // The scoring rule that made the wrong prompt harmful is unchanged: a fourth Crop Tree still sends the lesson back.
  three.Add(new ScenarioOneRackBay.Tree("fourth",8,-8,30,true,true));
  var four=ScenarioOneRackBay.AnalyseTeachingRack(three,new ScenarioOneRackBay.Order[0],frame,3,5,2);
  Check(ScenarioOneTeachingSequence.Current(new ScenarioOneTeachingSequence.Facts{AccessOpen=true,AccessReviewed=true,CropTrees=0,Bay=four})==1,"scoring unchanged: fourth Crop Tree returns to Objective 2");

  // ---- B-3: opaque figure labels against racks, rack numbers and existing access (design units, the same at every screen size) ----
  var site=new TeachingDiagramPlan.Site { StandSizeM=80,HeadlandX=-35.4f,HeadlandSouthZ=-37.7f,HeadlandNorthZ=37.7f,FeederZ=-27.4f,FeederEndX=-2.436166f,PathWidthM=2.6f,PadRadiusM=2.2f,RackWidthM=2.4f };
  var fig=TeachingDiagramPlan.ObjectiveOneFigure(site);
  var obstacles=Obstacles(fig);
  Check(obstacles.Count(o=>o.Name.EndsWith(" band"))==5 && obstacles.Count(o=>o.Name.StartsWith("rack number"))==5,"obstacles: five rack bands and five rack numbers");
  Check(obstacles.Any(o=>o.Name=="existing-headland-casing") && obstacles.Any(o=>o.Name=="existing-feeder-casing") && obstacles.Count(o=>o.Name.StartsWith("existing-pad"))==2 && obstacles.Any(o=>o.Name=="existing-approach-casing"),"obstacles: headland, link track, pads and track casings");
  var backed=fig.Labels.Where(l=>l.Backed).ToArray();
  Check(backed.Length==5,"five backed map labels");
  foreach(var l in backed) {
   var hits=Collisions(LabelBox(l.Value,l.X,l.Y,l.Width,l.Height),obstacles);
   Check(hits.Count==0,"figure label clear by "+MinGap+" units: '"+l.Value.Replace("\n"," ")+"' touches "+string.Join(", ",hits));
   float need=l.Value.Split('\n').Max(s=>s.Length)*CharWidthShare*l.FontSize;
   Check(l.Width>=need,"figure label wide enough for its words: '"+l.Value.Replace("\n"," ")+"' has "+l.Width+", needs "+need);
   Check(l.X>=0 && l.X+l.Width<=TeachingDiagramPlan.FigureKeyX-MinGap && l.Y+Math.Max(l.Height,LineUnits*l.Value.Split('\n').Length)<=fig.Height,"figure label inside the map column: '"+l.Value.Replace("\n"," ")+"'");
  }
  foreach(string name in new[]{"Headland","Link track","Forest track","Old path"}) Check(backed.Any(l=>l.Value==name),"label kept: "+name);
  Check(backed.Any(l=>l.Value.Replace("\n"," ")=="Existing clearing"),"label kept: Existing clearing");
  // Negative control: the #363 boxes must be reported by the same measure (so the measure is not vacuous).
  var oldClearing=LabelBox("Existing clearing",290f,145f,205f,30f);
  var oldHits=Collisions(oldClearing,obstacles);
  Check(oldHits.Contains("rack4 band") && oldHits.Contains("rack number 4"),"control: old Existing clearing box is detected on rack 4 and its number");
  var rack4=obstacles.Single(o=>o.Name=="rack4 band");
  float hidden=(Math.Min(oldClearing.R,rack4.R)-Math.Max(oldClearing.L,rack4.L))/(rack4.R-rack4.L);
  Check(hidden>0.45f && hidden<0.52f,"control: old box hid about 48% of rack 4 ("+hidden+")");
  Check(Collisions(LabelBox("Headland",0f,180f,80f,70f),obstacles).Contains("existing-headland-casing"),"control: old Headland box is detected on the headland");
  Check(Collisions(LabelBox("Link track",170f,TeachingDiagramPlan.FigureY(site,site.FeederZ)-34f,145f,30f),obstacles).Contains("existing-feeder-casing"),"control: old Link track box is detected on the link track");
  Check(80f<8*CharWidthShare*TeachingDiagramPlan.FigureFontSize,"control: old Headland width is detected as too narrow");
  // The figure's content is otherwise as it was.
  Check(fig.Items.Count(i=>i.Kind==TeachingDiagramPlan.Kind.Corridor && i.Id.StartsWith("rack") && i.State==TeachingDiagramPlan.CorridorState.Draft)==5,"five dashed suggestions kept");
  Check(fig.Items.Count(i=>i.Kind==TeachingDiagramPlan.Kind.Entrance && i.Id.StartsWith("entrance") && i.Marks==TeachingDiagramPlan.Marks.None)==5,"five hollow entrance targets kept");
  // N-9: the lifecycle strip has a shape for each state, above its words and inside the picture.
  var states=new[]{"suggested","planned","entrance-marked","harvested"}.Select(s=>fig.Items.Single(i=>i.Id=="lifecycle-"+s)).ToArray();
  Check(states[0].Kind==TeachingDiagramPlan.Kind.Corridor && states[0].State==TeachingDiagramPlan.CorridorState.Draft && states[1].State==TeachingDiagramPlan.CorridorState.Planned
   && states[2].Kind==TeachingDiagramPlan.Kind.FieldMark && states[3].State==TeachingDiagramPlan.CorridorState.Harvested,"lifecycle: one distinct shape per state");
  Check(fig.Items.Count(i=>i.Id.StartsWith("lifecycle-harvested-stump"))==2,"lifecycle: harvested shows stumps");
  var strip=fig.Labels.Where(l=>l.Value.Contains("\n") && !l.Backed && l.Y==TeachingDiagramPlan.LifecycleTextY).ToArray();
  Check(strip.Length==4 && strip.All(l=>l.Y+l.Height<=fig.Height) && states.All(s=>s.Y+12f<=TeachingDiagramPlan.LifecycleTextY && s.Y-12f>=575f+30f),"lifecycle: shapes sit between the heading and the words, words inside the picture");

  // ---- N-1 / N-2 / N-3: legend ----
  var map=new StandMapModel{HasProperty=true,MinX=-40,MinZ=-40,SizeM=80};
  map.Racks.Add(new MapRack{Start=new RackPoint(-35.4f,0),End=new RackPoint(40,0),Symbol=MapSymbol.RackSuggested});
  var entries=StandMapLegend.Entries(map,MapOverlay.None,true);
  var hollow=entries.SingleOrDefault(e=>e.Kind==LegendKind.Square);
  Check(hollow!=null && hollow.Group=="Your plan","legend: suggestions only (0 of 5) lists the hollow entrance target under Your plan");
  Check(!entries.Any(e=>e.Kind==LegendKind.FilledSquare),"legend: no filled entrance row before any field mark");
  map.Racks.Add(new MapRack{RackId=3,Start=new RackPoint(-35.4f,16),End=new RackPoint(40,16),Symbol=MapSymbol.RackPlayer,EntranceMarked=true});
  entries=StandMapLegend.Entries(map,MapOverlay.None,true);
  Check(entries.Single(e=>e.Kind==LegendKind.FilledSquare).Group=="Your plan","legend: filled entrance row under Your plan");
  map.Racks.RemoveAt(0);
  Check(!StandMapLegend.Entries(map,MapOverlay.None,false).Any(e=>e.Kind==LegendKind.Square),"legend: no hollow row when every drawn entrance is field marked");
  var surface=new MapHeadlandSurface(new RackPoint(-35.4f,-37.7f),new RackPoint(-35.4f,37.7f),new RackPoint(-35.4f,-27.4f),new RackPoint(-2.436166f,-27.4f),2.6f,2.2f);
  map.ExistingHeadlandSurfaces.Add(surface);
  var path=new MapPath{WidthM=2.6f};path.Points.Add(new RackPoint(-2.436166f,-40));path.Points.Add(new RackPoint(-2.436166f,11));map.Paths.Add(path);
  map.Racks.Add(new MapRack{RackId=4,Start=new RackPoint(-35.4f,32),End=new RackPoint(40,32),Symbol=MapSymbol.RackHarvested,Locked=true,EntranceMarked=true});
  entries=StandMapLegend.Entries(map,MapOverlay.None,true);
  string[] groups=entries.Select(e=>e.Group).ToArray();
  int Order(string g)=>g=="Already on the ground"?0:g=="Your plan"?1:2;
  Check(Enumerable.Range(1,groups.Length-1).All(i=>Order(groups[i-1])<=Order(groups[i])) && groups.Distinct().Count()==3,"legend: three groups, each contiguous and in order");
  var ground=entries.Where(e=>e.Group=="Already on the ground").Select(e=>e.Symbol).ToList();
  Check(ground.IndexOf(MapSymbol.Path)<ground.IndexOf(MapSymbol.OldPath) && ground.IndexOf(MapSymbol.OldPath)<ground.IndexOf(MapSymbol.Headland) && ground.IndexOf(MapSymbol.Headland)<ground.IndexOf(MapSymbol.LinkTrack) && ground.IndexOf(MapSymbol.LinkTrack)<ground.IndexOf(MapSymbol.Property),"legend: rows keep their insertion order inside a group");
  var plan=entries.Where(e=>e.Group=="Your plan").Select(e=>e.Text).ToList();
  Check(plan.IndexOf("Dashed = suggested")<plan.IndexOf("Solid = your planned rack"),"legend: Your plan rows keep their insertion order");

  // ---- N-4 / N-7 / N-8 ----
  Check(StandMapCanvas.TextSizePx>=17f,"map words at or above the 17-unit small reading size");
  Check(StandMapDrawPlan.Build(new StandMapModel{HasProperty=true,SizeM=80,MinX=-40,MinZ=-40,TeachingFrame=frame},600,MapDetail.Full).Count(p=>p.Text!=null)==3,"three teaching-area words still planned");
  Check(WorkPlanView.QuoteTotalLabel(true).Contains("before approval") && !WorkPlanView.QuoteTotalLabel(false).Contains("before approval"),"quote total says 'before approval' only while work is pending");
  Check(WorkPlanView.QuoteScopeNote(true).Contains("commits every eligible pending job") && !WorkPlanView.QuoteScopeNote(false).Contains("Approving"),"quote scope note matches the approval state");
  Check(WorkPlanView.QuoteScopeNote(true).Contains("all eligible harvest and salvage stems") && WorkPlanView.QuoteScopeNote(false).Contains("all eligible harvest and salvage stems"),"quote scope stated in both states");
  string marking=LearningObjectivesView.RackV2Instruction("Racks planned: 5 / 5",true,0,5,true);
  Check(marking.Contains("at the headland-side entrance of each rack") && !marking.Contains("rack entrance at each rack entrance") && !marking.Contains("nearest the path"),"entrance wording said once, never 'nearest the path'");
 }
}
