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
   System.IO.File.WriteAllText("/tmp/ccf363-preflight/plan-"+side+".json",System.Text.Json.JsonSerializer.Serialize(plan,new System.Text.Json.JsonSerializerOptions{IncludeFields=true}));
  }
  Console.WriteLine("Focused checks PASS "+count);
 }
}
