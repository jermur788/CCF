using System;
using System.Globalization;
using UnityEngine.UIElements;

// Small element builders shared by the Scenario One screens. Styling lives in
// Resources/ScenarioOneUi.uss; these only create elements and attach classes.
public static class UiKit
{
    public static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

    public static VisualElement Box(params string[] classes)
    {
        var element = new VisualElement();
        foreach (string c in classes) element.AddToClassList(c);
        return element;
    }

    public static Label Text(string text, params string[] classes)
    {
        var label = new Label(text);
        foreach (string c in classes) label.AddToClassList(c);
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

    // Label/value pair on one line (value right-aligned).
    public static void Line(VisualElement parent, string name, string value, string valueClass = "value")
    {
        VisualElement row = Box("table-row");
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
