// Test doubles only. No Unity runtime: supports production UI method slices, not layout or input.
using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.UIElements
{
    public enum DisplayStyle { Flex, None }
    public sealed class Style { public DisplayStyle display; }
    public class VisualElement
    {
        public string name, text;
        public readonly List<VisualElement> Children = new List<VisualElement>();
        public readonly Style style = new Style();
        public void Add(VisualElement element) => Children.Add(element);
        public void Clear() => Children.Clear();
    }
    public sealed class Foldout : VisualElement { public bool value; }
    public sealed class Button : VisualElement { public Action Click; }
}

public static class UiKit
{
    public static UnityEngine.UIElements.VisualElement Box(params string[] classes) => new UnityEngine.UIElements.VisualElement();
    public static UnityEngine.UIElements.VisualElement Add(UnityEngine.UIElements.VisualElement parent, string text, params string[] classes)
    {
        var label = new UnityEngine.UIElements.VisualElement { text = text }; parent.Add(label); return label;
    }
    public static UnityEngine.UIElements.VisualElement Row(UnityEngine.UIElements.VisualElement parent, params string[] classes)
    {
        var row = Box(); parent.Add(row); return row;
    }
    public static UnityEngine.UIElements.Button Button(string text, Action action, bool enabled = true, params string[] classes)
        => new UnityEngine.UIElements.Button { text = text, Click = action };
}

public sealed class LearningStub
{
    public bool NeedsReviewAcknowledgement;
    public void Record(string id) { }
    public void Refresh(bool force) { }
}
public sealed class MapStub
{
    public bool HasWaypoint;
    public void Refresh(bool force) { }
}
public sealed class EcologyStub
{
    public int EcologicalYear;
    public int GetCellIndex(Vector3 position) => 3;
}
public sealed class SpeciesStub { public string SpeciesId = "sitka-spruce"; }
public sealed class TransformStub { public Vector3 position = new Vector3(1, 0, 2); }
public sealed class ForestTree
{
    public string TreeId;
    public float BiologicalStemVolumeM3 = 0.2f;
    public SpeciesStub Species = new SpeciesStub();
    public TransformStub transform = new TransformStub();
}
public sealed class DefinitionStub { public int FellingBaseMinutes = 5, FellingMinutesPerCubicMetre = 10; }
public static class Approval
{
    // Shape-only input double. Production MFM build/validation is covered by the separate MFM-07 gate.
    public sealed class OrderLink
    {
        public int PrescriptionId, ReasonCode;
        public string TargetTreeId, CoverageUnitId, CropTreeId;
    }
}
public sealed class Plan { public List<Approval.OrderLink> Orders = new List<Approval.OrderLink>(); }

public sealed partial class ScenarioOneManager
{
    public readonly EcologyStub ecology = new EcologyStub();
    private readonly DefinitionStub definition = new DefinitionStub();
    private int nextWorkOrderId = 100;
    private FellingMaterialOutcome planningFellingOutcome;
    public int CurrentEcologicalYear { get => ecology.EcologicalYear; set => ecology.EcologicalYear = value; }
    public int CurrentYear => CurrentEcologicalYear;
    public long CashCents;
    public bool AnnualReviewSeen, NeedsCompletionReviewAcknowledgement;
    public bool WorkPlanOpen = true, AuxiliaryOpen;
    public readonly List<ScenarioOneWorkOrder> WorkOrders = new List<ScenarioOneWorkOrder>();
    public readonly List<ScenarioAnnualReport> AnnualReports = new List<ScenarioAnnualReport>();
    public readonly List<ScenarioManagementEvent> ManagementEvents = new List<ScenarioManagementEvent>();
    public List<YearHistoryView> HistoryYears = new List<YearHistoryView>();
    public void OpenWorkPlan(bool open) { WorkPlanOpen = open; }
    public void SetAuxiliaryPanelOpen(bool open) { AuxiliaryOpen = open; }
}
