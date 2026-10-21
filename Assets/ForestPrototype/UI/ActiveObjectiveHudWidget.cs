using System;
using UnityEngine.UIElements;

// "What am I meant to be doing now?" A small panel that stays on the walking HUD after the introduction is closed: the current
// objective, one short instruction and a clearly marked NOW state.
//
// Deliberately independent: it needs only a Model and (optionally) a callback for its Details button. It reads no forest or map
// state and knows nothing about the HUD minimap; WalkingHudView feeds it and decides where it sits, so it can be tested alone and
// composed with other HUD pieces later. Presentation only: no save, ecology or objective-completion state.
public sealed class ActiveObjectiveHudWidget
{
    // Plain data, so the text rules can be checked without UI Toolkit (ActiveObjectiveHudWidget.Describe).
    public sealed class Model
    {
        public int Number;          // 1-based position in the teaching sequence
        public int Total;
        public string Title = "";
        public string Instruction = "";
        public string Progress = "";
        public bool Complete;       // the whole sequence is finished; the panel then reads as an ongoing process, not a task
    }

    public VisualElement Root { get; }
    private readonly Label now, step, title, instruction, progress;
    private string shown = "";

    public ActiveObjectiveHudWidget(Action onDetails)
    {
        Root = UiKit.Box("panel", "objective-widget");
        Root.name = "active-objective-widget";
        VisualElement header = UiKit.Row(Root, "objective-header");
        now = UiKit.Add(header, "NOW", "objective-now");
        now.name = "active-objective-now";
        step = UiKit.Add(header, "", "objective-step");
        step.name = "active-objective-step";
        title = UiKit.Add(Root, "", "objective-title");
        title.name = "active-objective-title";
        instruction = UiKit.Add(Root, "", "objective-instruction");
        instruction.name = "active-objective-instruction";
        progress = UiKit.Add(Root, "", "objective-progress");
        progress.name = "active-objective-progress";
        Button details = UiKit.Button("Objective details [O]", onDetails ?? (() => { }), true, "btn-ghost", "objective-details");
        details.name = "active-objective-details";
        Root.Add(details);
    }

    // The words shown for a model; separate from the elements so the offline check can read them.
    public static string Describe(Model model) =>
        model == null ? "" : (model.Complete ? "ONGOING" : "NOW") + "|" + Step(model) + "|" + model.Title + "|" + model.Instruction + "|" + model.Progress;

    public static string Step(Model model) =>
        model == null || model.Total <= 0 ? "" : $"Objective {model.Number} of {model.Total}";

    public void Apply(Model model)
    {
        string key = Describe(model);
        if (key == shown) return;
        shown = key;
        if (model == null) return;
        now.text = model.Complete ? "ONGOING" : "NOW";
        step.text = Step(model);
        title.text = model.Title;
        instruction.text = model.Instruction;
        progress.text = model.Progress;
        progress.style.display = string.IsNullOrEmpty(model.Progress) ? DisplayStyle.None : DisplayStyle.Flex;
        instruction.style.display = string.IsNullOrEmpty(model.Instruction) ? DisplayStyle.None : DisplayStyle.Flex;
    }
}
