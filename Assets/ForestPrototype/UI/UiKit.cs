using System;
using System.Globalization;
using UnityEngine.UIElements;

// Small element builders shared by the Scenario One screens. Styling lives in
// Resources/ScenarioOneUi.uss; these only create elements and attach classes.
public static class UiKit
{
    public static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
    // Mirrors the canonical USS tokens for Painter2D/world feedback outside USS.
    public static readonly UnityEngine.Color AccentColour = new UnityEngine.Color(110f / 255f, 163f / 255f, 157f / 255f, 1f);
    public static readonly UnityEngine.Color WaypointColour = new UnityEngine.Color(239f / 255f, 180f / 255f, 78f / 255f, 1f);
    public static readonly UnityEngine.Color SelectionColour = new UnityEngine.Color(222f / 255f, 164f / 255f, 58f / 255f, 1f);
    public static readonly UnityEngine.Color WarningColour = new UnityEngine.Color(229f / 255f, 182f / 255f, 106f / 255f, 1f);
    public static readonly UnityEngine.Color ForestryColour = new UnityEngine.Color(151f / 255f, 188f / 255f, 165f / 255f, 1f);
    public static readonly UnityEngine.Color RackColour = new UnityEngine.Color(182f / 255f, 118f / 255f, 232f / 255f, 1f);
    private static UnityEngine.Font displayFont;

    // At 720p keep physical reading sizes useful and reflow dense screen layouts.
    public static void SetCompact(VisualElement root) => root.EnableInClassList("compact", UnityEngine.Screen.height < 900);

    public static VisualElement Box(params string[] classes)
    {
        var element = new VisualElement();
        foreach (string c in classes) element.AddToClassList(c);
        return element;
    }

    // A modal screen: the dimmed backdrop is the topmost element, takes every pointer event itself and lets none through to the
    // forest, the HUD or another screen drawn behind it. (Keyboard gameplay is already off while a panel is open.)
    public static void MakeModal(VisualElement backdrop)
    {
        backdrop.pickingMode = PickingMode.Position;
        backdrop.RegisterCallback<PointerDownEvent>(e => e.StopPropagation());
        backdrop.RegisterCallback<PointerUpEvent>(e => e.StopPropagation());
        backdrop.RegisterCallback<WheelEvent>(e => e.StopPropagation());
    }

    public static Label Text(string text, params string[] classes)
    {
        var label = new Label(text);
        foreach (string c in classes) label.AddToClassList(c);
        // The reference's editorial serif belongs to major titles, never dense values/body.
        // FontDefinition.FromFont lets UI Toolkit generate its normal dynamic font internally.
        if (Array.IndexOf(classes, "title") >= 0 && Array.IndexOf(classes, "money") < 0)
        {
            if (displayFont == null)
                displayFont = UnityEngine.Resources.Load<UnityEngine.Font>("Fonts/LiberationSerif-Regular");
            if (displayFont != null)
                label.style.unityFontDefinition = FontDefinition.FromFont(displayFont);
        }
        return label;
    }

    public static Label Add(VisualElement parent, string text, params string[] classes)
    {
        Label label = Text(text, classes);
        parent.Add(label);
        return label;
    }

    public static Button Button(string text, Action onClick, bool enabled = true, params string[] classes)
    {
        var button = new Button(onClick) { text = text };
        button.AddToClassList("btn");
        foreach (string c in classes) button.AddToClassList(c);
        button.SetEnabled(enabled);
        // Keyboard focus would let gameplay keys (Tab, M, N) act on the button.
        button.focusable = false;
        return button;
    }

    public static VisualElement Row(VisualElement parent, params string[] classes)
    {
        VisualElement row = Box(classes.Length > 0 ? classes : new[] { "row" });
        parent.Add(row);
        return row;
    }

    // Label/value pair on one line (value right-aligned). Pass valueBelow for a value that is a sentence, so it
    // goes under the name instead of squeezing it.
    public static void Line(VisualElement parent, string name, string value, string valueClass = "value", bool valueBelow = false)
    {
        VisualElement row = Box(valueBelow ? new[] { "table-row", "table-row-stacked" } : new[] { "table-row" });
        row.Add(Text(name, "cell-name", "body"));
        row.Add(Text(value, "line-value", valueClass));
        parent.Add(row);
    }

    public static void Stat(VisualElement grid, string name, string value)
    {
        VisualElement stat = Box("stat");
        stat.Add(Text(name, "stat-label"));
        stat.Add(Text(value, "stat-value"));
        grid.Add(stat);
    }

    public static Label Chip(VisualElement parent, string text, string kind = null)
    {
        Label chip = Text(text, "chip");
        if (kind != null) chip.AddToClassList(kind);
        parent.Add(chip);
        return chip;
    }

    // Existing menu routes in one order. Named guidance controls are passed through unchanged.
    public static VisualElement MenuNavigation(ScenarioOneUiRoot ui, string current,
        Button map = null, Button work = null, Button forest = null)
    {
        VisualElement nav = Box("menu-navigation", "row-wrap");
        nav.name = "menu-navigation";
        var objectives = Button("Objectives [O]", () => ui.ShowObjectives());
        map = map ?? Button("Stand Map [M]", () => ui.ShowMap());
        work = work ?? Button("Work Plan [Tab]", () => ui.ShowWorkPlan());
        var review = Button("Annual Review", () => ui.ShowReview());
        map.text = "Stand Map [M]";
        work.text = "Work Plan [Tab]";
        var destinations = new[] { objectives, map, work, review };
        var names = new[] { "Objectives", "Stand Map", "Work Plan", "Annual Review" };
        for (int i = 0; i < destinations.Length; i++)
        {
            if (names[i] == current) { destinations[i].AddToClassList("btn-selected"); destinations[i].SetEnabled(false); }
            nav.Add(destinations[i]);
        }
        nav.Add(Button("Help [F1]", () => ui.ShowCurrentHelp()));
        forest = forest ?? Button("Back to forest [Esc]", () => ui.CloseAll());
        forest.text = "Back to forest [Esc]";
        nav.Add(forest);
        return nav;
    }

    // Prompts remain labels: a locked-cursor player cannot click them. Keys are taken from
    // the live handlers' existing prompt strings; duplicate menu keys occupy only one slot.
    public static void KeyPrompts(VisualElement parent, string text, bool suppressMarking = false)
    {
        parent.Clear();
        var seen = new System.Collections.Generic.HashSet<string>();
        var matches = System.Text.RegularExpressions.Regex.Matches(text ?? "", @"\[([^\]]+)\]");
        if (matches.Count == 0) { Add(parent, text ?? "", "muted"); return; }
        if (matches[0].Index > 0) Add(parent, text.Substring(0, matches[0].Index).Trim(' ', '·', '|'), "context-note");
        for (int i = 0; i < matches.Count; i++)
        {
            string key = matches[i].Groups[1].Value;
            if (suppressMarking && (key == "X" || key == "C")) continue;
            if (!seen.Add(key)) continue;
            int start = matches[i].Index + matches[i].Length;
            int end = i + 1 < matches.Count ? matches[i + 1].Index : text.Length;
            VisualElement item = Box("key-control", "row");
            Add(item, key, "keycap");
            Add(item, text.Substring(start, end - start).Trim(' ', '·', '|', '\n'), "key-verb");
            parent.Add(item);
        }
        foreach (VisualElement child in parent.Query<VisualElement>().ToList()) child.pickingMode = PickingMode.Ignore;
    }

    public static string Money(long cents) => ScenarioOneManager.FormatMoney(cents);
    public static string SignedMoney(long cents) => (cents > 0 ? "+" : cents < 0 ? "−" : "") + ScenarioOneManager.FormatMoney(Math.Abs(cents));
    public static string F(float value, string format) => value.ToString(format, Inv);
    public static string M3(double cubicCentimetres) => (cubicCentimetres / 1000000d).ToString("0.000", Inv) + " m³";

    // 5 m cell label: column letter west→east, row number south→north.
    public static string CellLabel(int index, int perAxis)
    {
        if (index < 0 || perAxis <= 0) return "—";
        int x = index % perAxis, z = index / perAxis;
        return ((char)('A' + x)).ToString() + (z + 1).ToString(Inv);
    }
}
