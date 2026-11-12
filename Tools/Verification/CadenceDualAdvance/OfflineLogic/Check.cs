using System;
using System.Collections.Generic;
public enum ScenarioWorkType { FellTree, PlantJuvenile }
public enum ScenarioWorkStatus { Pending, Approved, Completed, Failed }
public enum ScenarioManagementEventType { WorkResolved }
public enum ScenarioManagementOutcome { Succeeded, Failed }
public class ScenarioOneWorkOrder { public int workOrderId, resolvedYear, createdYear; public ScenarioWorkType type; public ScenarioWorkStatus status; public string validationMessage="",targetTreeId="tree"; }
public class ScenarioManagementEvent { public int eventId,workOrderId,year; public ScenarioManagementEventType eventType; public ScenarioManagementOutcome outcome; public ScenarioWorkType taskType; public string targetTreeId="tree"; }
class Check {
 static int count;
 static void Equal(int actual,int expected) { count++; if(actual!=expected) throw new Exception($"{actual} != {expected}"); }
 static void Main() {
 var orders=new List<ScenarioOneWorkOrder>(); var events=new List<ScenarioManagementEvent>();
 int Target(int year,int end=100)=>ScenarioManagementCadence.Compute(year,end,orders,events,out _);
 Equal(Target(0),4); Equal(Target(23),25); Equal(Target(25),29); Equal(Target(98),100); Equal(Target(100),100);
 var o=new ScenarioOneWorkOrder{workOrderId=1,resolvedYear=10,status=ScenarioWorkStatus.Completed};orders.Add(o);
 var e=new ScenarioManagementEvent{eventId=1,workOrderId=1,year=10};events.Add(e);
 Equal(Target(11),14);Equal(Target(14),18);
 o.status=ScenarioWorkStatus.Pending;Equal(Target(11),15);o.status=ScenarioWorkStatus.Approved;Equal(Target(11),15);o.status=ScenarioWorkStatus.Completed;
 o.validationMessage="invalid";Equal(Target(11),15);o.validationMessage="";
 events.Add(e);Equal(Target(11),15);events.RemoveAt(1);
 orders.Add(o);Equal(Target(11),15);orders.RemoveAt(1);
 e.targetTreeId="wrong";Equal(Target(11),15);e.targetTreeId="tree";
 e.outcome=ScenarioManagementOutcome.Failed;Equal(Target(11),15);e.outcome=ScenarioManagementOutcome.Succeeded;
 e.year=9;Equal(Target(11),15);e.year=10;
 e.eventId=0;Equal(Target(11),15);e.eventId=1;
 o.targetTreeId="";e.targetTreeId="";Equal(Target(11),15);o.targetTreeId="tree";e.targetTreeId="tree";
 o.resolvedYear=12;e.year=12;Equal(Target(11),15);o.resolvedYear=10;e.year=10;
 o.type=ScenarioWorkType.PlantJuvenile;Equal(Target(11),15);
 Console.WriteLine($"CADENCE CALCULATOR PASS {count} assertions (minimal record stubs; no runtime parity claim)");
 }
}
